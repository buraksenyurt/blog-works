using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.SemanticKernel;

namespace BlogTagger;

class Program
{
    static readonly string[] ValidCategories = [
        "Yazılım Mimarisi", ".NET & C#", "Yapay Zeka", "Bulut Bilişim",
        "Veritabanı", "Programlama Dilleri", "Diğer"
    ];

    static async Task Main(string[] args)
    {
        var builder = Kernel.CreateBuilder();

        var handler = new HttpClientHandler();
        var httpClient = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromMinutes(5)
        };

        builder.AddOpenAIChatCompletion(
            modelId: "meta-llama-3-8b-instruct",
            apiKey: "lm-studio",
            endpoint: new Uri("http://localhost:1234/v1"),
            httpClient: httpClient
        );

        var kernel = builder.Build();

        string postsDirectory = @"C:\Users\burak\Development\blog-works\resources\_stage\";

        if (!Directory.Exists(postsDirectory))
        {
            Console.WriteLine("Klasör bulunamadı.");
            return;
        }

        var files = Directory.GetFiles(postsDirectory, "*.md");
        Console.WriteLine($"Toplam {files.Length} dosya bulundu. İşlemler başlıyor...\n");

        foreach (var filePath in files)
        {
            await ProcessMarkdownFileAsync(kernel, filePath);
        }

        Console.WriteLine("\nTüm dosyalar işlendi.");
    }

    static async Task ProcessMarkdownFileAsync(Kernel kernel, string filePath)
    {
        string fileName = Path.GetFileName(filePath);
        Console.WriteLine($"İşleniyor: {fileName}");

        string content = await File.ReadAllTextAsync(filePath);

        var match = Regex.Match(content, @"^---\s*\n(.*?)\n---\s*\n(.*)", RegexOptions.Singleline);
        if (!match.Success)
        {
            Console.WriteLine($"  -> Uyarı: {fileName} içinde geçerli bir front-matter bulunamadı. Atlanıyor.");
            return;
        }

        string frontMatter = match.Groups[1].Value;
        string markdownBody = match.Groups[2].Value;

        string contentSample = markdownBody.Length > 2000 ? markdownBody[..2000] : markdownBody;

        string prompt = $@"
Sen uzman bir yazılım mimarı ve içerik editörüsün. Aşağıdaki makale metnini analiz et.
Görevlerin:
1. Makaleyi SADECE şu kategorilerden en uygun olanına ata: {string.Join(", ", ValidCategories)}. Hiçbiri tam uymuyorsa 'Diğer' seç.
2. Makalede geçen teknoloji, dil veya konseptleri yansıtan, arama motoru optimizasyonuna uygun en fazla 7 adet etiket (tag) belirle. (Örn: csharp, ado.net, mcp, rust, docker, sql, zig-lang, go-lang vb)

Çıktıyı SADECE aşağıdaki gibi geçerli bir JSON formatında ver. JSON dışında hiçbir metin, açıklama veya markdown backtick'i (```) kullanma!
{{
  ""category"": ""Seçilen Kategori"",
  ""tags"": [""etiket1"", ""etiket2""]
}}

Makale Metni:
{contentSample}
";

        try
        {
            var result = await kernel.InvokePromptAsync(prompt);
            string jsonResponse = result.GetValue<string>()?.Trim() ?? string.Empty;

            int startIndex = jsonResponse.IndexOf('{');
            int endIndex = jsonResponse.LastIndexOf('}');

            if (startIndex >= 0 && endIndex > startIndex)
            {
                jsonResponse = jsonResponse.Substring(startIndex, (endIndex - startIndex) + 1);
            }
            else
            {
                Console.WriteLine($"  -> Uyarı ({fileName}): Gelen yanıtta geçerli bir JSON bloğu bulunamadı. Atlanıyor.");
                return;
            }

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true
            };

            var aiResult = JsonSerializer.Deserialize<AiClassificationResult>(jsonResponse, jsonOptions);

            if (aiResult != null)
            {
                UpdateFileWithNewFrontMatter(filePath, frontMatter, markdownBody, aiResult);
                Console.WriteLine($"  -> Başarılı: Kategori: {aiResult.Category}, Etiketler: {string.Join(", ", aiResult.Tags)}");
            }
        }
        catch (JsonException jsonEx)
        {
            Console.WriteLine($"  -> JSON PARSE HATASI ({fileName}): {jsonEx.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  -> HATA ({fileName}): {ex.Message}");
        }
    }

    static void UpdateFileWithNewFrontMatter(string filePath, string oldFrontMatter, string body, AiClassificationResult aiResult)
    {
        string newFrontMatter = Regex.Replace(oldFrontMatter, @"^categories:.*?(?=^[a-zA-Z])", $"categories:\n  - {aiResult.Category}\n", RegexOptions.Singleline | RegexOptions.Multiline);

        StringBuilder tagsBuilder = new();
        tagsBuilder.AppendLine("tags:");
        foreach (var tag in aiResult.Tags)
        {
            string cleanTag = tag.ToLowerInvariant().Replace(" ", "-");
            tagsBuilder.AppendLine($"  - {cleanTag}");
        }

        newFrontMatter = Regex.Replace(newFrontMatter, @"^tags:.*?(?=^[a-zA-Z]|\z)", tagsBuilder.ToString(), RegexOptions.Singleline | RegexOptions.Multiline);

        string finalContent = $"---\n{newFrontMatter.TrimEnd()}\n---\n{body}";
        File.WriteAllText(filePath, finalContent, new UTF8Encoding(false));
    }
}

class AiClassificationResult
{
    [System.Text.Json.Serialization.JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [System.Text.Json.Serialization.JsonPropertyName("tags")]
    public string[] Tags { get; set; } = [];
}