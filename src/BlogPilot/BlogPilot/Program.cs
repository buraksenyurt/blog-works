using BlogPilot.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using OpenAI;
using Qdrant.Client;
using System.ClientModel;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.AI;

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

app.MapPost("/api/chat", async (HttpContext context, Kernel kernel, QdrantVectorStore vectorStore) =>
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

    var sbContext = new StringBuilder();
    sbContext.AppendLine("Aşağıdaki kaynak makale parçalarını kullanarak kullanıcının sorusunu yanıtla. Eğer kaynaklarda yoksa, 'Bu bilgi blog yazılarında bulunmamaktadır' şeklinde belirt.:\n");

    await foreach (var result in blogChunksCollection.SearchAsync(queryEmbedding[0].Vector, top: 5))
    {
        sbContext.AppendLine($"Makale: {result.Record.Title} (Yıl: {result.Record.Year})");
        sbContext.AppendLine(result.Record.Text);
        sbContext.AppendLine("---");
    }

    var history = new ChatHistory();
    history.AddSystemMessage(sbContext.ToString());
    history.AddUserMessage(request.Message);

    try
    {
        var responseStream = chatService.GetStreamingChatMessageContentsAsync(history, kernel: kernel);
        
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

public class ChatRequest
{
    public string Message { get; set; } = string.Empty;
}
