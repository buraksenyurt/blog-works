using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Microsoft.SemanticKernel.Text;
using OpenAI;
using Qdrant.Client;
using System.ClientModel;
using System.Text.RegularExpressions;

using var loggerFactory = LoggerFactory.Create(b => b
    .SetMinimumLevel(LogLevel.Debug)
    .AddSimpleConsole(o =>
    {
        o.ColorBehavior    = LoggerColorBehavior.Enabled;
        o.SingleLine       = true;
        o.TimestampFormat  = "HH:mm:ss ";
    }));

var logger = loggerFactory.CreateLogger("Chunker");

logger.LogInformation("Initializing embedding model and vector store");

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
var collection = vectorStore.GetCollection<Guid, BlogPostChunk>("blog_chunks");

await collection.EnsureCollectionExistsAsync();
logger.LogInformation("Qdrant collection ready");

string[] mdFiles = Directory.GetFiles(
    @"C:\Users\burak\Development\blog-works\resources\articles",
    "*.md",
    SearchOption.AllDirectories);
logger.LogInformation("Processing {PostCount} posts", mdFiles.Length);

// Matches fenced code blocks: ```lang\n code \n```
var fencedCodePattern = new Regex(
    @"^```(?<lang>[^\r\n]*)\r?\n(?<code>.*?)^```",
    RegexOptions.Multiline | RegexOptions.Singleline);

int postIndex = 0;
int totalProseChunks = 0;
int totalCodeChunks = 0;

foreach (var file in mdFiles)
{
    postIndex++;
    var raw = await File.ReadAllTextAsync(file);

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
    proseMarkdown = Regex.Replace(proseMarkdown, @"\[([^\]]*)\]\([^\)]*\)", "$1");                     // links → text
    proseMarkdown = Regex.Replace(proseMarkdown, @"\*{1,2}([^*\r\n]+)\*{1,2}", "$1");                 // bold / italic *
    proseMarkdown = Regex.Replace(proseMarkdown, @"_{1,2}([^_\r\n]+)_{1,2}", "$1");                   // bold / italic _
    proseMarkdown = Regex.Replace(proseMarkdown, @"`([^`]+)`", "$1");                                  // inline code
    proseMarkdown = Regex.Replace(proseMarkdown, @"^\s*[-*_]{3,}\s*$", string.Empty, RegexOptions.Multiline); // hr
    string proseText = Regex.Replace(proseMarkdown, @"[^\S\n]+", " ");
    proseText = Regex.Replace(proseText, @"\n{3,}", "\n\n").Trim();

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
            await collection.UpsertAsync(new BlogPostChunk
            {
                Title      = title,
                Slug       = slug,
                PubDate    = pubDate,
                Year       = year,
                Categories = categories,
                Tags       = tags,
                ChunkType  = "prose",
                Language   = string.Empty,
                Text       = chunkText,
                Vector     = embedding.Vector
            });
        }
        totalProseChunks += proseChunkIndex;
        logger.LogInformation("{Count} prose chunk(s) stored", proseChunkIndex);
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
            await collection.UpsertAsync(new BlogPostChunk
            {
                Title      = title,
                Slug       = slug,
                PubDate    = pubDate,
                Year       = year,
                Categories = categories,
                Tags       = tags,
                ChunkType  = "code",
                Language   = langLabel,
                Text       = chunkText,
                Vector     = embedding.Vector
            });
        }
        totalCodeChunks += codeFragIndex;
        logger.LogInformation("{Count} code chunk(s) [{Lang}] stored", codeFragIndex, langLabel);
    }
}

logger.LogInformation("──────────────────────────────────────────────────────────");
logger.LogInformation("Total posts processed : {Posts}", postIndex);
logger.LogInformation("Total prose chunks    : {Prose}", totalProseChunks);
logger.LogInformation("Total code chunks     : {Code}", totalCodeChunks);
logger.LogInformation("Grand total chunks    : {Total}", totalProseChunks + totalCodeChunks);
logger.LogInformation("All documents have been embedded and stored in Qdrant!");

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

logger.LogInformation("──────────────────────────────────────────────────────────");
logger.LogInformation("Total posts processed : {Posts}", postIndex);
logger.LogInformation("Total prose chunks    : {Prose}", totalProseChunks);
logger.LogInformation("Total code chunks     : {Code}", totalCodeChunks);
logger.LogInformation("Grand total chunks    : {Total}", totalProseChunks + totalCodeChunks);
logger.LogInformation("All documents have been embedded and stored in Qdrant!");