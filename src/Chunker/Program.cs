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

var verboseLogging = args.Contains("--verbose", StringComparer.OrdinalIgnoreCase);
var forceRebuild = args.Contains("--rebuild", StringComparer.OrdinalIgnoreCase);
var requestedYears = ParseMultiValueOption(args, "--year");
var maxFiles = ParseIntOption(args, "--max-files");

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
const string articlesRoot = @"C:\Users\burak\Development\blog-works\resources\articles";
const string indexerVersion = "2026-03-20-incremental-v1";

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
    {
        File.Delete(stateFilePath);
    }
}

var collection = vectorStore.GetCollection<Guid, BlogPostChunk>(collectionName);

await collection.EnsureCollectionExistsAsync();
logger.LogInformation("Qdrant collection ready");

string[] mdFiles = Directory.GetFiles(
    articlesRoot,
    "*.md",
    SearchOption.AllDirectories);

if (requestedYears.Count > 0)
{
    mdFiles = mdFiles
        .Where(file => requestedYears.Contains(Path.GetFileName(Path.GetDirectoryName(file) ?? string.Empty), StringComparer.OrdinalIgnoreCase))
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

// Matches fenced code blocks: ```lang\n code \n```
var fencedCodePattern = new Regex(
    @"^```(?<lang>[^\r\n]*)\r?\n(?<code>.*?)^```",
    RegexOptions.Multiline | RegexOptions.Singleline);

int postIndex = 0;
int indexedPostCount = 0;
int skippedPostCount = 0;
int removedPostCount = 0;
int totalProseChunks = 0;
int totalCodeChunks = 0;
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

    // Fallback: derive year from the parent directory name (e.g. .../articles/2003/file.md)
    if (year == 0)
    {
        var dirName = Path.GetFileName(Path.GetDirectoryName(file) ?? string.Empty);
        int.TryParse(dirName, out year);
    }

    // Slug derived from the file name
    string slug = Path.GetFileNameWithoutExtension(file)
        .ToLowerInvariant()
        .Replace(' ', '-');
    string sourcePath = Path.Combine("resources", "articles", Path.GetRelativePath(articlesRoot, file))
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

    // Metadata prefix prepended to every chunk text before embedding.
    // Ensures the embedding vector captures language / topic / year context,
    // enabling semantic queries such as "C# makaleleri", "2010 öncesi ASP.NET".
    var metaParts = new List<string> { $"Makale: {title}" };
    if (!string.IsNullOrEmpty(categories)) metaParts.Add($"Kategoriler: {categories}");
    if (!string.IsNullOrEmpty(tags))       metaParts.Add($"Etiketler: {tags}");
    if (year > 0)                          metaParts.Add($"Yıl: {year}");
    string metaPrefix = string.Join(" | ", metaParts) + "\n\n";

    // Extract fenced code blocks and replace each with a blank line
    var codeChunks = new List<(string lang, string code)>();
    string proseMarkdown = fencedCodePattern.Replace(body, m =>
    {
        string lang = m.Groups["lang"].Value.Trim().ToLowerInvariant();
        string code = m.Groups["code"].Value.Trim();
        if (!string.IsNullOrWhiteSpace(code))
            codeChunks.Add((lang, code));
        return "\n\n";
    });

    // Strip markdown syntax to obtain plain prose text
    proseMarkdown = Regex.Replace(proseMarkdown, @"^#{1,6}\s+", string.Empty, RegexOptions.Multiline); // headings
    proseMarkdown = Regex.Replace(proseMarkdown, @"!\[.*?\]\(.*?\)", string.Empty);                    // images
    proseMarkdown = Regex.Replace(proseMarkdown, @"\[([^\]]*)\]\(([^\)]*)\)", "$1 ($2)");             // preserve link targets
    proseMarkdown = Regex.Replace(proseMarkdown, @"\*{1,2}([^*\r\n]+)\*{1,2}", "$1");                 // bold / italic *
    proseMarkdown = Regex.Replace(proseMarkdown, @"_{1,2}([^_\r\n]+)_{1,2}", "$1");                   // bold / italic _
    proseMarkdown = Regex.Replace(proseMarkdown, @"`([^`]+)`", "$1");                                  // inline code
    proseMarkdown = Regex.Replace(proseMarkdown, @"^\s*[-*_]{3,}\s*$", string.Empty, RegexOptions.Multiline); // hr
    string proseText = Regex.Replace(proseMarkdown, @"[^\S\n]+", " ");
    proseText = Regex.Replace(proseText, @"\n{3,}", "\n\n").Trim();

    var generatedChunks = new List<BlogPostChunk>();

    if (!string.IsNullOrWhiteSpace(proseText))
    {
#pragma warning disable SKEXP0050
        var lines      = TextChunker.SplitPlainTextLines(proseText, maxTokensPerLine: 80);
        var paragraphs = TextChunker.SplitPlainTextParagraphs(lines, maxTokensPerParagraph: 200, overlapTokens: 25);
#pragma warning restore SKEXP0050

        int proseChunkIndex = 0;
        foreach (var paragraph in paragraphs)
        {
            proseChunkIndex++;
            var chunkText = metaPrefix + paragraph;
            logger.LogDebug("Prose chunk {Index}/{Total} — {Chars} chars", proseChunkIndex, paragraphs.Count, chunkText.Length);
            var embedding = await embeddingGenerator.GenerateAsync(chunkText);
            generatedChunks.Add(new BlogPostChunk
            {
                Id         = CreateDeterministicGuid($"{sourcePath}|prose|{proseChunkIndex}"),
                Title      = title,
                Slug       = slug,
                PubDate    = pubDate,
                Year       = year,
                Categories = categories,
                Tags       = tags,
                SourcePath = sourcePath,
                ChunkType  = "prose",
                Language   = string.Empty,
                Text       = chunkText,
                Vector     = embedding.Vector
            });
        }
        totalProseChunks += proseChunkIndex;
        logger.LogInformation("{Count} prose chunk(s) prepared", proseChunkIndex);
    }

    foreach (var (lang, code) in codeChunks)
    {
        var langLabel   = string.IsNullOrEmpty(lang) ? "code" : lang;
        var chunkHeader = metaPrefix + $"[{langLabel} kodu]\n";

#pragma warning disable SKEXP0050
        var codeLines = TextChunker.SplitPlainTextLines(code, maxTokensPerLine: 80);
        var codeFragments = TextChunker.SplitPlainTextParagraphs(
            codeLines, maxTokensPerParagraph: 200, overlapTokens: 0);
#pragma warning restore SKEXP0050

        int codeFragIndex = 0;
        foreach (var fragment in codeFragments)
        {
            codeFragIndex++;
            var chunkText = chunkHeader + fragment;
            logger.LogDebug("Code chunk [{Lang}] {Index}/{Total} — {Chars} chars", langLabel, codeFragIndex, codeFragments.Count, chunkText.Length);
            var embedding = await embeddingGenerator.GenerateAsync(chunkText);
            generatedChunks.Add(new BlogPostChunk
            {
                Id         = CreateDeterministicGuid($"{sourcePath}|code|{langLabel}|{codeFragIndex}"),
                Title      = title,
                Slug       = slug,
                PubDate    = pubDate,
                Year       = year,
                Categories = categories,
                Tags       = tags,
                SourcePath = sourcePath,
                ChunkType  = "code",
                Language   = langLabel,
                Text       = chunkText,
                Vector     = embedding.Vector
            });
        }
        totalCodeChunks += codeFragIndex;
        logger.LogInformation("{Count} code chunk(s) [{Lang}] prepared", codeFragIndex, langLabel);
    }

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
logger.LogInformation("Deleted stale chunks  : {Deleted}", deletedChunks);
logger.LogInformation("Grand total chunks    : {Total}", totalProseChunks + totalCodeChunks);
logger.LogInformation("Incremental indexing completed.");

// ── Local helpers ─────────────────────────────────────────────────────────────

/// Parses the YAML frontmatter block and returns structured metadata.
static (string title, string pubDate, int year, string categories, string tags)
    ParseFrontmatter(string fm)
{
    var titleMatch = Regex.Match(fm, @"^title:\s*""?(?<v>[^""\r\n]+)""?\s*$", RegexOptions.Multiline);
    string title   = titleMatch.Success ? titleMatch.Groups["v"].Value.Trim() : string.Empty;

    var dateMatch  = Regex.Match(fm, @"^pubDate:\s*(?<v>[^\r\n]+)", RegexOptions.Multiline);
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