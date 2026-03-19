using BlogPilot.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Microsoft.Extensions.AI;
using OpenAI;
using Qdrant.Client;
using System.ClientModel;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();

var openAIClientOptions = new OpenAIClientOptions { Endpoint = new Uri("http://127.0.0.1:1234/v1") };
var openAIClient = new OpenAIClient(new ApiKeyCredential("lm-studio"), openAIClientOptions);

builder.Services.AddSingleton<QdrantClient>(_ => new QdrantClient("localhost", 6344));
builder.Services.AddSingleton<QdrantVectorStore>(sp => new QdrantVectorStore(sp.GetRequiredService<QdrantClient>(), ownsClient: false));

var kernelBuilder = builder.Services.AddKernel();

#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0010
kernelBuilder.AddOpenAIEmbeddingGenerator(
    modelId: "text-embedding-nomic-embed-text-v1.5",
    openAIClient: openAIClient
);

kernelBuilder.AddOpenAIChatCompletion(
    modelId: "qwen/qwen3-14b",
    openAIClient: openAIClient
);
#pragma warning restore SKEXP0010
#pragma warning restore SKEXP0001

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

app.MapPost("/api/chat", async (HttpContext context, Kernel kernel, QdrantVectorStore vectorStore, ILogger<Program> logger) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var requestBody = await reader.ReadToEndAsync();
    var request = JsonSerializer.Deserialize<ChatRequest>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    
    if (string.IsNullOrWhiteSpace(request?.Message))
    {
        context.Response.StatusCode = 400;
        return;
    }

    context.Response.ContentType = "text/event-stream";
    context.Response.Headers.Append("Cache-Control", "no-cache");
    context.Response.Headers.Append("Connection", "keep-alive");

    var embeddingService = kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
    var chatService = kernel.GetRequiredService<IChatCompletionService>();

    var queryEmbedding = await embeddingService.GenerateAsync([request.Message]);
    var blogChunksCollection = vectorStore.GetCollection<Guid, BlogPostChunk>("blog_chunks");

    var retrievalCandidates = new List<(BlogPostChunk Record, double VectorScore)>();
    await foreach (var result in blogChunksCollection.SearchAsync(queryEmbedding[0].Vector, top: 12))
    {
        retrievalCandidates.Add((result.Record, result.Score ?? 0d));
    }

    var selectedChunks = SelectRelevantChunks(retrievalCandidates, request.Message);
    if (selectedChunks.Count == 0)
    {
        var notFound = JsonSerializer.Serialize(new { chunk = "Bu bilgi blog yazılarında bulunmamaktadır." });
        await context.Response.WriteAsync($"data: {notFound}\n\n");
        await context.Response.WriteAsync("data: [DONE]\n\n");
        await context.Response.Body.FlushAsync();
        return;
    }

    logger.LogInformation(
        "Question '{Question}' matched {ChunkCount} chunk(s) from {SourceCount} source(s): {Sources}",
        request.Message,
        selectedChunks.Count,
        selectedChunks.Select(GetSourceKey).Distinct().Count(),
        string.Join(" | ", selectedChunks.Select(chunk => $"{chunk.Title} [{FormatSourceReference(chunk)}]").Distinct()));

    var sbContext = new StringBuilder();
    sbContext.AppendLine("Sen sadece verilen blog kaynaklarına dayanarak cevap üreten bir asistansın.");
    sbContext.AppendLine("Kurallar:");
    sbContext.AppendLine("1. Her zaman Türkçe cevap ver.");
    sbContext.AppendLine("2. Sadece aşağıda verilen kaynak parçalarını kullan.");
    sbContext.AppendLine("3. URL, repo adresi, video adresi, makale adı veya dosya yolu uydurma.");
    sbContext.AppendLine("4. Kaynaklarda olmayan hiçbir iddiayı gerçekmiş gibi yazma.");
    sbContext.AppendLine("5. Kaynak yetersizse tam olarak 'Bu bilgi blog yazılarında bulunmamaktadır.' de.");
    sbContext.AppendLine("6. Bir kaynağa atıf yapacaksan sadece verilen Dosya alanını kullan.");
    sbContext.AppendLine("7. Yanıtın sonunda 'Kaynaklar:' başlığı aç ve kullandığın kaynakları '- Başlık (Yıl) | Dosya' formatında listele.");
    sbContext.AppendLine();
    sbContext.AppendLine("Kaynak parçaları:");

    var sourceIndex = 0;
    foreach (var chunk in selectedChunks)
    {
        sourceIndex++;
        sbContext.AppendLine($"[Kaynak {sourceIndex}] Başlık: {chunk.Title}");
        sbContext.AppendLine($"[Kaynak {sourceIndex}] Yıl: {chunk.Year}");
        if (!string.IsNullOrWhiteSpace(chunk.Categories))
        {
            sbContext.AppendLine($"[Kaynak {sourceIndex}] Kategoriler: {chunk.Categories}");
        }

        if (!string.IsNullOrWhiteSpace(chunk.Tags))
        {
            sbContext.AppendLine($"[Kaynak {sourceIndex}] Etiketler: {chunk.Tags}");
        }

        sbContext.AppendLine($"[Kaynak {sourceIndex}] Slug: {chunk.Slug}");
        sbContext.AppendLine($"[Kaynak {sourceIndex}] Dosya: {FormatSourceReference(chunk)}");
        sbContext.AppendLine($"[Kaynak {sourceIndex}] Parça Türü: {chunk.ChunkType}");
        sbContext.AppendLine($"[Kaynak {sourceIndex}] İçerik:");
        sbContext.AppendLine(chunk.Text);
        sbContext.AppendLine("---");
    }

    var history = new ChatHistory();
    history.AddSystemMessage(sbContext.ToString());
    history.AddUserMessage(request.Message);

    var executionSettings = new OpenAIPromptExecutionSettings
    {
        Temperature = 0.1,
        TopP = 0.3,
        FrequencyPenalty = 0,
        PresencePenalty = 0
    };

    try
    {
        var responseStream = chatService.GetStreamingChatMessageContentsAsync(history, executionSettings, kernel: kernel);
        
        await foreach (var content in responseStream)
        {
            if (!string.IsNullOrEmpty(content.Content))
            {
                var data = JsonSerializer.Serialize(new { chunk = content.Content });
                await context.Response.WriteAsync($"data: {data}\n\n");
                await context.Response.Body.FlushAsync();
            }
        }
    }
    catch (Exception ex)
    {
        var err = JsonSerializer.Serialize(new { error = ex.Message });
        await context.Response.WriteAsync($"data: {err}\n\n");
    }

    await context.Response.WriteAsync("data: [DONE]\n\n");
    await context.Response.Body.FlushAsync();
});

app.Run();

static List<BlogPostChunk> SelectRelevantChunks(
    IEnumerable<(BlogPostChunk Record, double VectorScore)> retrievalCandidates,
    string query)
{
    var queryTerms = ExtractSearchTerms(query);
    var wantsCode = QueryPrefersCode(queryTerms);

    return retrievalCandidates
        .Where(candidate => !string.IsNullOrWhiteSpace(candidate.Record.Text))
        .Select(candidate => new
        {
            candidate.Record,
            CombinedScore = candidate.VectorScore + ComputeKeywordBoost(queryTerms, candidate.Record, wantsCode)
        })
        .OrderByDescending(candidate => candidate.CombinedScore)
        .ThenByDescending(candidate => candidate.Record.Year)
        .GroupBy(candidate => GetSourceKey(candidate.Record))
        .SelectMany(group => group.Take(2))
        .Take(6)
        .Select(candidate => candidate.Record)
        .ToList();
}

static double ComputeKeywordBoost(HashSet<string> queryTerms, BlogPostChunk record, bool wantsCode)
{
    var titleBoost = CountTermHits(queryTerms, record.Title) * 0.45;
    var tagBoost = CountTermHits(queryTerms, record.Tags) * 0.30;
    var categoryBoost = CountTermHits(queryTerms, record.Categories) * 0.20;
    var textBoost = CountTermHits(queryTerms, record.Text) * 0.05;
    var chunkTypeBoost = wantsCode
        ? (string.Equals(record.ChunkType, "code", StringComparison.OrdinalIgnoreCase) ? 0.25 : 0.05)
        : (string.Equals(record.ChunkType, "prose", StringComparison.OrdinalIgnoreCase) ? 0.20 : 0.0);

    return titleBoost + tagBoost + categoryBoost + textBoost + chunkTypeBoost;
}

static int CountTermHits(HashSet<string> queryTerms, string? text)
{
    if (string.IsNullOrWhiteSpace(text))
    {
        return 0;
    }

    var normalizedText = text.ToLowerInvariant();
    return queryTerms.Count(term => normalizedText.Contains(term, StringComparison.Ordinal));
}

static HashSet<string> ExtractSearchTerms(string query)
{
    var stopWords = new HashSet<string>(StringComparer.Ordinal)
    {
        "acaba", "ama", "bir", "bunu", "bu", "da", "de", "daha", "en", "gibi", "hangi",
        "için", "ile", "ilgili", "mi", "mı", "mu", "mü", "nasıl", "nedir", "neler", "olan",
        "olarak", "ve", "veya", "var", "yok", "yazı", "yazıda", "yazılar", "yazılarda"
    };

    var terms = Regex.Matches(query.ToLowerInvariant(), @"[\p{L}\p{Nd}]{3,}")
        .Select(match => match.Value)
        .Where(term => !stopWords.Contains(term))
        .ToHashSet(StringComparer.Ordinal);

    if (query.Contains("oop", StringComparison.OrdinalIgnoreCase))
    {
        terms.Add("oop");
    }

    return terms;
}

static bool QueryPrefersCode(HashSet<string> queryTerms)
{
    return queryTerms.Contains("kod")
        || queryTerms.Contains("örnek")
        || queryTerms.Contains("ornek")
        || queryTerms.Contains("uygulama")
        || queryTerms.Contains("sample");
}

static string GetSourceKey(BlogPostChunk record)
{
    if (!string.IsNullOrWhiteSpace(record.SourcePath))
    {
        return record.SourcePath;
    }

    return $"{record.Year}:{record.Slug}";
}

static string FormatSourceReference(BlogPostChunk record)
{
    if (!string.IsNullOrWhiteSpace(record.SourcePath))
    {
        return record.SourcePath;
    }

    if (record.Year > 0 && !string.IsNullOrWhiteSpace(record.Slug))
    {
        return $"resources/articles/{record.Year}/{record.Slug}.md";
    }

    return record.Title;
}

public class ChatRequest
{
    public string Message { get; set; } = string.Empty;
}
