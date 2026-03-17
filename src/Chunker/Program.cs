using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Microsoft.SemanticKernel.Text;
using OpenAI;
using Qdrant.Client;
using System.ClientModel;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;

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

string[] xmlFiles = Directory.GetFiles(@"C:\Users\burak\Development\blog-works\resources\posts", "*.xml");
logger.LogInformation("Processing {PostCount} posts", xmlFiles.Length);

var prePattern = new Regex(
    @"<pre[^>]*?(?:class=""brush:(?<lang>[^;""]+)[^""]*"")?[^>]*>(?<code>.*?)</pre>",
    RegexOptions.Singleline | RegexOptions.IgnoreCase);

int postIndex = 0;
int totalProseChunks = 0;
int totalCodeChunks = 0;

foreach (var file in xmlFiles)
{
    postIndex++;
    var xDoc = XDocument.Load(file);
    var postNode = xDoc.Element("post");
    if (postNode == null) continue;

    string title = postNode.Element("title")?.Value ?? "";
    string slug  = postNode.Element("slug")?.Value  ?? "";
    string rawContent = postNode.Element("content")?.Value ?? "";
    if (string.IsNullOrWhiteSpace(rawContent)) continue;

    logger.LogInformation("[{Index}/{Total}] {Title}", postIndex, xmlFiles.Length, title);

    string decodedHtml = WebUtility.HtmlDecode(rawContent);

    var codeChunks = new List<(string lang, string code)>();
    string proseHtml = prePattern.Replace(decodedHtml, m =>
    {
        string lang = m.Groups["lang"].Value.Trim().ToLowerInvariant();
        string code = WebUtility.HtmlDecode(
            Regex.Replace(m.Groups["code"].Value, "<.*?>", string.Empty)).Trim();
        if (!string.IsNullOrWhiteSpace(code))
            codeChunks.Add((lang, code));
        return "\n\n";
    });

    string proseText = Regex.Replace(proseHtml,
        @"</?(p|div|h[1-6]|li|blockquote|tr)\b[^>]*>",
        "\n\n", RegexOptions.IgnoreCase);
    proseText = Regex.Replace(proseText, @"<br\s*/?>", "\n", RegexOptions.IgnoreCase);
    proseText = Regex.Replace(proseText, "<.*?>", string.Empty);
    proseText = WebUtility.HtmlDecode(proseText);
    proseText = Regex.Replace(proseText, @"[^\S\n]+", " ");
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
            logger.LogDebug("Prose chunk {Index}/{Total} — {Chars} chars", proseChunkIndex, paragraphs.Count, paragraph.Length);
            var embedding = await embeddingGenerator.GenerateAsync(paragraph);
            await collection.UpsertAsync(new BlogPostChunk
            {
                Title     = title,
                Slug      = slug,
                ChunkType = "prose",
                Language  = string.Empty,
                Text      = paragraph,
                Vector    = embedding.Vector
            });
        }
        totalProseChunks += proseChunkIndex;
        logger.LogInformation("{Count} prose chunk(s) stored", proseChunkIndex);
    }

    foreach (var (lang, code) in codeChunks)
    {
        var langLabel = string.IsNullOrEmpty(lang) ? "code" : lang;
        var header    = $"[{title}] [{langLabel}]\n";

#pragma warning disable SKEXP0050
        var codeLines = TextChunker.SplitPlainTextLines(code, maxTokensPerLine: 80);
        var codeFragments = TextChunker.SplitPlainTextParagraphs(
            codeLines, maxTokensPerParagraph: 200, overlapTokens: 0);
#pragma warning restore SKEXP0050

        int codeFragIndex = 0;
        foreach (var fragment in codeFragments)
        {
            codeFragIndex++;
            logger.LogDebug("Code chunk [{Lang}] {Index}/{Total} — {Chars} chars", langLabel, codeFragIndex, codeFragments.Count, fragment.Length);
            var chunkText = header + fragment;
            var embedding = await embeddingGenerator.GenerateAsync(chunkText);
            await collection.UpsertAsync(new BlogPostChunk
            {
                Title     = title,
                Slug      = slug,
                ChunkType = "code",
                Language  = langLabel,
                Text      = chunkText,
                Vector    = embedding.Vector
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