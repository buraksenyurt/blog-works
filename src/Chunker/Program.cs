using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Microsoft.SemanticKernel.Text;
using OpenAI;
using Qdrant.Client;
using System.ClientModel;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

var verboseLogging  = args.Contains("--verbose", StringComparer.OrdinalIgnoreCase);
var forceRebuild    = args.Contains("--rebuild", StringComparer.OrdinalIgnoreCase);
var requestedYears  = ParseMultiValueOption(args, "--year");
var maxFiles        = ParseIntOption(args, "--max-files");

using var loggerFactory = LoggerFactory.Create(b => b
    .SetMinimumLevel(verboseLogging ? LogLevel.Debug : LogLevel.Information)
    .AddSimpleConsole(o =>
    {
        o.ColorBehavior    = LoggerColorBehavior.Enabled;
        o.SingleLine       = true;
        o.TimestampFormat  = "HH:mm:ss ";
    }));

var logger = loggerFactory.CreateLogger("Chunker");

logger.LogInformation("Initializing embedding model and vector store");

const string collectionName = "blog_chunks";
const string articlesRoot   = @"C:\Users\burak\Development\blog-works\resources\_posts";

// Bump this version whenever the chunking strategy changes to force a full re-index.
// Current strategy: section-aware chunking with nomic search_document: prefix, summary chunks,
// ProgrammingLanguage extraction, and base64 image stripping.
const string indexerVersion = "2026-03-20-rag-v2";

var stateFilePath = Path.Combine(Directory.GetCurrentDirectory(), "chunker-state.json");
var state = LoadState(stateFilePath, indexerVersion);

var openAIClientOptions = new OpenAIClientOptions { Endpoint = new Uri("http://localhost:1234/v1") };
var openAIClient = new OpenAIClient(new ApiKeyCredential("lm-studio"), openAIClientOptions);

var builder = Kernel.CreateBuilder();
builder.AddOpenAIEmbeddingGenerator(
    modelId: "text-embedding-nomic-embed-text-v1.5",
    openAIClient: openAIClient
);

var kernel = builder.Build();
var embeddingGenerator = kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
logger.LogInformation("Embedding generator ready");

var qdrantClient = new QdrantClient("localhost", 6344);
var vectorStore = new QdrantVectorStore(qdrantClient, ownsClient: true);

if (forceRebuild)
{
    logger.LogInformation("Force rebuild requested. Clearing collection {CollectionName} and local state.", collectionName);
    await vectorStore.EnsureCollectionDeletedAsync(collectionName);
    state = new ChunkerState { IndexerVersion = indexerVersion };
    if (File.Exists(stateFilePath))
        File.Delete(stateFilePath);
}

var collection = vectorStore.GetCollection<Guid, BlogPostChunk>(collectionName);

await collection.EnsureCollectionExistsAsync();
logger.LogInformation("Qdrant collection ready");

string[] mdFiles = Directory.GetFiles(
    articlesRoot,
    "*.md",
    SearchOption.AllDirectories);

// Filter by year using the filename date prefix (e.g. "2024-10-31-title.md" → year 2024).
// All posts live flat in _posts/, so directory-based filtering does not apply.
if (requestedYears.Count > 0)
{
    mdFiles = mdFiles
        .Where(file =>
        {
            var filename = Path.GetFileName(file);
            var yearStr  = filename.Length >= 4 ? filename[..4] : string.Empty;
            return requestedYears.Contains(yearStr, StringComparer.OrdinalIgnoreCase);
        })
        .ToArray();
}

Array.Sort(mdFiles, StringComparer.OrdinalIgnoreCase);

if (maxFiles is > 0)
{
    mdFiles = mdFiles.Take(maxFiles.Value).ToArray();
}

var partialRun = requestedYears.Count > 0 || maxFiles is > 0;
logger.LogInformation("Processing {PostCount} posts", mdFiles.Length);
if (partialRun)
{
    logger.LogInformation("Partial indexing mode is active. Removed-file cleanup is disabled for this run.");
}

int postIndex = 0;
int indexedPostCount = 0;
int skippedPostCount = 0;
int removedPostCount = 0;
int totalProseChunks = 0;
int totalCodeChunks = 0;
int totalSummaryChunks = 0;
int deletedChunks = 0;
var existingSourcePaths = state.Files.Keys.ToHashSet(StringComparer.OrdinalIgnoreCase);
var discoveredSourcePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

foreach (var file in mdFiles)
{
    postIndex++;
    var raw = await File.ReadAllTextAsync(file);
    var contentHash = ComputeSha256(raw);

    // Parse YAML frontmatter
    string title = string.Empty, pubDate = string.Empty, categories = string.Empty, tags = string.Empty;
    int year = 0;
    string body = raw;

    var fmMatch = Regex.Match(raw, @"^---\r?\n(?<fm>.*?)\r?\n---\r?\n", RegexOptions.Singleline);
    if (fmMatch.Success)
    {
        (title, pubDate, year, categories, tags) = ParseFrontmatter(fmMatch.Groups["fm"].Value);
        body = raw[fmMatch.Length..];
    }

    if (string.IsNullOrWhiteSpace(title))
        title = Path.GetFileNameWithoutExtension(file);

    // Fallback: derive year from the filename prefix (e.g. 2003-11-08-stored-procedure.md)
    if (year == 0)
    {
        var fileNameOnly = Path.GetFileNameWithoutExtension(file);
        if (fileNameOnly.Length >= 4) int.TryParse(fileNameOnly[..4], out year);
    }

    // Slug derived from the file name
    string slug = Path.GetFileNameWithoutExtension(file)
        .ToLowerInvariant()
        .Replace(' ', '-');
    string sourcePath = Path.Combine("resources", "_posts", Path.GetRelativePath(articlesRoot, file))
        .Replace('\\', '/');
    discoveredSourcePaths.Add(sourcePath);

    if (state.Files.TryGetValue(sourcePath, out var indexedFile)
        && indexedFile.ContentHash == contentHash
        && indexedFile.IndexerVersion == indexerVersion)
    {
        skippedPostCount++;
        logger.LogInformation("[{Index}/{Total}] Skipping unchanged {SourcePath}", postIndex, mdFiles.Length, sourcePath);
        continue;
    }

    if (string.IsNullOrWhiteSpace(body)) continue;

    logger.LogInformation("[{Index}/{Total}] {Title} ({Year})", postIndex, mdFiles.Length, title, year);

    string programmingLanguage = ExtractProgrammingLanguage(categories, tags);

    // Metadata prefix prepended to every chunk text before embedding.
    // Enables semantic queries like "C# dilinde DI örnekleri", "2010 öncesi ASP.NET".
    var metaParts = new List<string> { $"Makale: {title}" };
    if (!string.IsNullOrEmpty(programmingLanguage)) metaParts.Add($"Dil: {programmingLanguage}");
    if (!string.IsNullOrEmpty(categories)) metaParts.Add($"Kategoriler: {categories}");
    if (!string.IsNullOrEmpty(tags))       metaParts.Add($"Etiketler: {tags}");
    if (year > 0)                          metaParts.Add($"Yıl: {year}");
    string metaPrefix = string.Join(" | ", metaParts) + "\n\n";

    // Split body into heading-aware sections; generate chunks with nomic-embed task prefix.
    var sections = SplitIntoSections(body);
    var generatedChunks = new List<BlogPostChunk>();

    // ── Summary chunk: article-level overview ─────────────────────────────
    var headingsList = sections
        .Where(s => !string.IsNullOrEmpty(s.Heading))
        .Select(s => $"- {s.Heading}")
        .ToList();
    if (headingsList.Count > 0)
    {
        var summaryText = metaPrefix + "[Makale Özeti]\nBölümler:\n" + string.Join("\n", headingsList);
        var summaryEmbedding = await embeddingGenerator.GenerateAsync("search_document: " + summaryText);
        generatedChunks.Add(new BlogPostChunk
        {
            Id                  = CreateDeterministicGuid($"{sourcePath}|summary|0"),
            Title               = title,
            Slug                = slug,
            PubDate             = pubDate,
            Year                = year,
            Categories          = categories,
            Tags                = tags,
            SourcePath          = sourcePath,
            ChunkType           = "summary",
            Language            = string.Empty,
            ProgrammingLanguage = programmingLanguage,
            SectionHeading      = string.Empty,
            Text                = summaryText,
            Vector              = summaryEmbedding.Vector
        });
        totalSummaryChunks++;
        logger.LogDebug("Summary chunk prepared ({Headings} headings)", headingsList.Count);
    }

    // ── Per-section prose and code chunks ─────────────────────────────────
    int proseSectionIdx = 0;
    int codeSectionIdx = 0;
    foreach (var section in sections)
    {
        string sectionMetaPrefix = metaPrefix;
        if (!string.IsNullOrEmpty(section.Heading))
            sectionMetaPrefix += $"Bölüm: {section.Heading}\n\n";

        string proseText = StripMarkdownFormatting(section.Prose);
        if (!string.IsNullOrWhiteSpace(proseText))
        {
#pragma warning disable SKEXP0050
            var proseLines = TextChunker.SplitPlainTextLines(proseText, maxTokensPerLine: 80);
            var proseParagraphs = TextChunker.SplitPlainTextParagraphs(proseLines, maxTokensPerParagraph: 400, overlapTokens: 40);
#pragma warning restore SKEXP0050
            foreach (var paragraph in proseParagraphs)
            {
                proseSectionIdx++;
                var chunkText = sectionMetaPrefix + paragraph;
                logger.LogDebug("Prose chunk {Index} — {Chars} chars", proseSectionIdx, chunkText.Length);
                var embedding = await embeddingGenerator.GenerateAsync("search_document: " + chunkText);
                generatedChunks.Add(new BlogPostChunk
                {
                    Id                  = CreateDeterministicGuid($"{sourcePath}|prose|{proseSectionIdx}"),
                    Title               = title,
                    Slug                = slug,
                    PubDate             = pubDate,
                    Year                = year,
                    Categories          = categories,
                    Tags                = tags,
                    SourcePath          = sourcePath,
                    ChunkType           = "prose",
                    Language            = string.Empty,
                    ProgrammingLanguage = programmingLanguage,
                    SectionHeading      = section.Heading,
                    Text                = chunkText,
                    Vector              = embedding.Vector
                });
            }
            totalProseChunks += proseParagraphs.Count;
        }

        foreach (var (lang, code) in section.CodeBlocks)
        {
            string langLabel = string.IsNullOrEmpty(lang) ? "code" : lang;
            var codeHeader = sectionMetaPrefix + $"[{langLabel} kodu]\n";

#pragma warning disable SKEXP0050
            var codeLines = TextChunker.SplitPlainTextLines(code, maxTokensPerLine: 80);
            var codeFragments = TextChunker.SplitPlainTextParagraphs(codeLines, maxTokensPerParagraph: 400, overlapTokens: 0);
#pragma warning restore SKEXP0050
            foreach (var fragment in codeFragments)
            {
                codeSectionIdx++;
                var chunkText = codeHeader + fragment;
                logger.LogDebug("Code chunk [{Lang}] {Index} — {Chars} chars", langLabel, codeSectionIdx, chunkText.Length);
                var embedding = await embeddingGenerator.GenerateAsync("search_document: " + chunkText);
                generatedChunks.Add(new BlogPostChunk
                {
                    Id                  = CreateDeterministicGuid($"{sourcePath}|code|{langLabel}|{codeSectionIdx}"),
                    Title               = title,
                    Slug                = slug,
                    PubDate             = pubDate,
                    Year                = year,
                    Categories          = categories,
                    Tags                = tags,
                    SourcePath          = sourcePath,
                    ChunkType           = "code",
                    Language            = langLabel,
                    ProgrammingLanguage = programmingLanguage,
                    SectionHeading      = section.Heading,
                    Text                = chunkText,
                    Vector              = embedding.Vector
                });
            }
            totalCodeChunks += codeFragments.Count;
            logger.LogInformation("{Count} code chunk(s) [{Lang}] prepared", codeFragments.Count, langLabel);
        }
    }

    if (proseSectionIdx > 0) logger.LogInformation("{Count} prose chunk(s) prepared", proseSectionIdx);
    logger.LogInformation("{Count} total chunk(s) prepared", generatedChunks.Count);

    var previousChunkIds = indexedFile?.ChunkIds ?? [];
    var currentChunkIds = generatedChunks.Select(chunk => chunk.Id).ToList();
    var staleChunkIds = previousChunkIds.Except(currentChunkIds).ToList();

    if (staleChunkIds.Count > 0)
    {
        await collection.DeleteAsync(staleChunkIds);
        deletedChunks += staleChunkIds.Count;
        logger.LogInformation("Removed {Count} stale chunk(s) for {SourcePath}", staleChunkIds.Count, sourcePath);
    }

    foreach (var chunk in generatedChunks)
    {
        await collection.UpsertAsync(chunk);
    }

    indexedPostCount++;
    state.Files[sourcePath] = new IndexedFileState
    {
        ContentHash = contentHash,
        IndexerVersion = indexerVersion,
        Title = title,
        ChunkIds = currentChunkIds,
        LastIndexedUtc = DateTime.UtcNow
    };
    SaveState(stateFilePath, state);

    logger.LogInformation("Stored {ChunkCount} chunk(s) for {SourcePath}", generatedChunks.Count, sourcePath);
}

if (!partialRun)
{
    foreach (var removedSourcePath in existingSourcePaths.Except(discoveredSourcePaths).ToList())
    {
        if (!state.Files.TryGetValue(removedSourcePath, out var removedFile))
        {
            continue;
        }

        if (removedFile.ChunkIds.Count > 0)
        {
            await collection.DeleteAsync(removedFile.ChunkIds);
            deletedChunks += removedFile.ChunkIds.Count;
        }

        state.Files.Remove(removedSourcePath);
        removedPostCount++;
        logger.LogInformation("Removed {SourcePath} and deleted {ChunkCount} chunk(s)", removedSourcePath, removedFile.ChunkIds.Count);
    }
}

SaveState(stateFilePath, state);

logger.LogInformation("──────────────────────────────────────────────────────────");
logger.LogInformation("Total posts processed : {Posts}", postIndex);
logger.LogInformation("Indexed posts         : {Indexed}", indexedPostCount);
logger.LogInformation("Skipped posts         : {Skipped}", skippedPostCount);
logger.LogInformation("Removed posts         : {Removed}", removedPostCount);
logger.LogInformation("Total prose chunks    : {Prose}", totalProseChunks);
logger.LogInformation("Total code chunks     : {Code}", totalCodeChunks);
logger.LogInformation("Total summary chunks  : {Summary}", totalSummaryChunks);
logger.LogInformation("Deleted stale chunks  : {Deleted}", deletedChunks);
logger.LogInformation("Grand total chunks    : {Total}", totalProseChunks + totalCodeChunks + totalSummaryChunks);
logger.LogInformation("Incremental indexing completed.");

// ── Local helpers ─────────────────────────────────────────────────────────────

/// Parses the YAML frontmatter block and returns structured metadata.
static (string title, string pubDate, int year, string categories, string tags)
    ParseFrontmatter(string fm)
{
    var titleMatch = Regex.Match(fm, @"^title:\s*""?(?<v>[^""\r\n]+)""?\s*$", RegexOptions.Multiline);
    string title   = titleMatch.Success ? titleMatch.Groups["v"].Value.Trim() : string.Empty;

    var dateMatch  = Regex.Match(fm, @"^(?:date|pubDate):\s*(?<v>[^\r\n]+)", RegexOptions.Multiline);
    string pubDate = dateMatch.Success ? dateMatch.Groups["v"].Value.Trim() : string.Empty;
    int year = 0;
    if (pubDate.Length >= 4) int.TryParse(pubDate[..4], out year);

    string categories = ParseYamlList(fm, "categories");
    string tags       = ParseYamlList(fm, "tags");

    return (title, pubDate, year, categories, tags);
}

/// Extracts YAML sequence items under <paramref name="key"/> as a comma-separated string.
static string ParseYamlList(string fm, string key)
{
    var section = Regex.Match(
        fm,
        $@"^{key}:\s*\r?\n(?<items>(?:[ \t]+-[^\r\n]*\r?\n?)*)",
        RegexOptions.Multiline);
    if (!section.Success) return string.Empty;
    var items = Regex.Matches(
        section.Groups["items"].Value,
        @"^\s*-\s*(?<v>[^\r\n]+)",
        RegexOptions.Multiline);
    return string.Join(", ", items.Cast<Match>().Select(m => m.Groups["v"].Value.Trim()));
}

static Guid CreateDeterministicGuid(string value)
{
    var hash = MD5.HashData(Encoding.UTF8.GetBytes(value));
    return new Guid(hash);
}

static string ComputeSha256(string value)
{
    var hash = SHA256.HashData(Encoding.UTF8.GetBytes(value));
    return Convert.ToHexString(hash);
}

static ChunkerState LoadState(string stateFilePath, string indexerVersion)
{
    if (!File.Exists(stateFilePath))
    {
        return new ChunkerState { IndexerVersion = indexerVersion };
    }

    var json = File.ReadAllText(stateFilePath);
    var state = JsonSerializer.Deserialize<ChunkerState>(json);
    if (state is null || state.IndexerVersion != indexerVersion)
    {
        return new ChunkerState { IndexerVersion = indexerVersion };
    }

    return state;
}

static void SaveState(string stateFilePath, ChunkerState state)
{
    state.IndexerVersion = state.IndexerVersion.Trim();
    var json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(stateFilePath, json);
}

static HashSet<string> ParseMultiValueOption(string[] args, string optionName)
{
    var values = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    for (var i = 0; i < args.Length; i++)
    {
        var arg = args[i];
        if (arg.Equals(optionName, StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
        {
            values.Add(args[i + 1]);
            i++;
            continue;
        }

        if (arg.StartsWith(optionName + "=", StringComparison.OrdinalIgnoreCase))
        {
            values.Add(arg[(optionName.Length + 1)..]);
        }
    }

    return values;
}

static int? ParseIntOption(string[] args, string optionName)
{
    for (var i = 0; i < args.Length; i++)
    {
        var arg = args[i];
        if (arg.Equals(optionName, StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length && int.TryParse(args[i + 1], out var valueFromNextArg))
        {
            return valueFromNextArg;
        }

        if (arg.StartsWith(optionName + "=", StringComparison.OrdinalIgnoreCase)
            && int.TryParse(arg[(optionName.Length + 1)..], out var valueFromSameArg))
        {
            return valueFromSameArg;
        }
    }

    return null;
}

/// Splits a markdown document into sections based on ATX headings (##, ###, ####).
/// Each section carries the heading text, prose content (code fences removed), and extracted code blocks.
/// Correctly ignores headings that appear inside fenced code blocks.
static List<(string Heading, string Prose, List<(string lang, string code)> CodeBlocks)>
    SplitIntoSections(string markdown)
{
    var sections = new List<(string Heading, string Prose, List<(string lang, string code)> CodeBlocks)>();
    var headingPattern = new Regex(@"^#{2,4}\s+(?<heading>.+)$");
    var fenceMarker   = new Regex(@"^[ \t]*```");
    var codePattern   = new Regex(
        @"^```(?<lang>[^\r\n]*)\r?\n(?<code>.*?)^```",
        RegexOptions.Multiline | RegexOptions.Singleline);

    string currentHeading = string.Empty;
    var currentLines = new List<string>();
    bool inFence = false;

    void FlushSection()
    {
        if (currentLines.Count == 0 && string.IsNullOrEmpty(currentHeading)) return;
        var sectionText = string.Join("\n", currentLines);
        var codeBlocks = new List<(string lang, string code)>();
        string proseText = codePattern.Replace(sectionText, m =>
        {
            string lang = m.Groups["lang"].Value.Trim().ToLowerInvariant();
            string code = m.Groups["code"].Value.Trim();
            if (!string.IsNullOrWhiteSpace(code))
                codeBlocks.Add((lang, code));
            return "\n\n";
        });
        sections.Add((currentHeading, proseText, codeBlocks));
    }

    foreach (var line in markdown.Split('\n'))
    {
        if (fenceMarker.IsMatch(line))
            inFence = !inFence;

        if (!inFence && headingPattern.IsMatch(line))
        {
            FlushSection();
            currentHeading = headingPattern.Match(line).Groups["heading"].Value.Trim();
            currentLines = [];
        }
        else
        {
            currentLines.Add(line);
        }
    }
    FlushSection();

    return sections;
}

/// Strips markdown formatting from prose text, including base64 image data URIs.
static string StripMarkdownFormatting(string markdown)
{
    // Remove base64 image data URIs first (can be hundreds of KB, pollute embeddings)
    var result = Regex.Replace(markdown, @"!\[.*?\]\(data:[^)]{100,}\)", string.Empty);

    result = Regex.Replace(result, @"^#{1,6}\s+", string.Empty, RegexOptions.Multiline); // headings
    result = Regex.Replace(result, @"!\[.*?\]\(.*?\)", string.Empty);                    // images
    result = Regex.Replace(result, @"\[([^\]]*)\]\(([^\)]*)\)", "$1 ($2)");              // links
    result = Regex.Replace(result, @"\*{1,2}([^*\r\n]+)\*{1,2}", "$1");                 // bold/italic *
    result = Regex.Replace(result, @"_{1,2}([^_\r\n]+)_{1,2}", "$1");                   // bold/italic _
    result = Regex.Replace(result, @"`([^`]+)`", "$1");                                  // inline code
    result = Regex.Replace(result, @"^\s*[-*_]{3,}\s*$", string.Empty, RegexOptions.Multiline); // hr
    result = Regex.Replace(result, @"[^\S\n]+", " ");
    result = Regex.Replace(result, @"\n{3,}", "\n\n").Trim();
    return result;
}

/// Extracts a normalized programming-language identifier from categories and tags strings.
/// Returns empty string when no recognized language is found.
static string ExtractProgrammingLanguage(string categories, string tags)
{
    var combined = (categories + " " + tags).ToLowerInvariant();

    // More-specific patterns first
    if (combined.Contains("c#") || combined.Contains("csharp") || combined.Contains("c-sharp")) return "csharp";
    if (combined.Contains("f#") || combined.Contains("fsharp"))                                  return "fsharp";
    if (combined.Contains("c++") || combined.Contains("cpp"))                                    return "cpp";
    if (combined.Contains("javascript") || combined.Contains(" js ") || combined.EndsWith(" js")) return "javascript";
    if (combined.Contains("typescript") || combined.Contains(" ts ") || combined.EndsWith(" ts")) return "typescript";
    if (combined.Contains("python"))                                                              return "python";
    if (combined.Contains("rust"))                                                                return "rust";
    if (combined.Contains("golang") || combined.Contains("go-lang"))                             return "golang";
    if (combined.Contains("ruby"))                                                                return "ruby";
    if (combined.Contains("kotlin"))                                                              return "kotlin";
    if (combined.Contains("swift"))                                                               return "swift";
    if (combined.Contains("java"))                                                                return "java";
    if (combined.Contains("php"))                                                                 return "php";
    if (combined.Contains("elixir"))                                                              return "elixir";
    if (combined.Contains("erlang"))                                                              return "erlang";
    if (combined.Contains("haskell"))                                                             return "haskell";
    if (combined.Contains("scala"))                                                               return "scala";
    if (combined.Contains("clojure"))                                                             return "clojure";
    if (combined.Contains("zig"))                                                                 return "zig";
    if (combined.Contains("lua"))                                                                 return "lua";
    if (combined.Contains("perl"))                                                                return "perl";
    if (combined.Contains("dart"))                                                                return "dart";
    if (combined.Contains("powershell"))                                                          return "powershell";
    if (combined.Contains("shell") || combined.Contains("bash"))                                  return "shell";
    if (combined.Contains("sql"))                                                                 return "sql";
    if (combined.Contains(".net") || combined.Contains("dotnet") || combined.Contains("asp.net")) return "dotnet";
    if (combined.Contains("html") || combined.Contains("css"))                                   return "web";
    return string.Empty;
}

public sealed class ChunkerState
{
    public string IndexerVersion { get; set; } = string.Empty;
    public Dictionary<string, IndexedFileState> Files { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class IndexedFileState
{
    public string ContentHash { get; set; } = string.Empty;
    public string IndexerVersion { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public List<Guid> ChunkIds { get; set; } = [];
    public DateTime LastIndexedUtc { get; set; }
}