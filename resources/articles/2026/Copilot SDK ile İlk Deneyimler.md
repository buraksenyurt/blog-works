---
title: "Copilot SDK ile İlk Deneyimler"
pubDate: 2026-03-03 14:40:00
categories:
  - C#
tags:
  - C#
  - Bash
  - .NET
  - REST
  - JSON
  - Web API
  - HTTP
  - Authorization
  - MCP
  - Rust
  - Async/Await
  - Caching
  - Serialization
  - Generics
  - GitHub
---

# Copilot SDK ile İlk Deneyimler
İçimiz dışımız, sağımız solumuz yapay zekadan geçilmiyor. Öyle bir dönemdeyiz ne ona burun kıvırabiliriz ne de her şeyimizle teslim. Halen daha işin özünde problemi anlamanın, parçalara bölebilmenin, doğru metodolojileri ve araçları kullanarak ideal çözüm yollarını geliştirebilmenin önemli olduğunu düşünüyorum. Bunun içinde kendimizi donatmaya devam etmemiz gerektiğini biliyorum. Ayrıca yapay zeka araçlarını tanımanın ve yazılımcılara ne gibi avantajlar ya da tam tersi dezavantajlar getireceğini de araştırmalıyız diyorum. Ne yazık ki yapay zeka denildiğinde onu sadece chat-gpt'den ibaret bir şey olduğunu düşünenler var. Bunun böyle olmadığını deneyimlediğim araçlar bana gösteriyor. Tanımak lazım. İşte bugünkü yazımızın konularından birisi de bu; Microsoft'un [Copilot SDK](https://github.com/github/copilot-sdk) paketini kullanarak neler yapabilirizin basit bir Hello World demosu.

Pek tabii CLI (Command Line Interface) aracı olarak yapay zeka modellerinin nasıl konumlandığına bir bakmak gerekiyor. Kendi sistemlerimizde Claude'nin, Github'ın veya benzerlerinin komut satırı araçlarını kullanarak da birçok şey yapabiliyoruz. Planlama, görev tayin etme, proje üretme vs gibi birçok aksiyonu icra edebilmekteyiz. Peki Copilot SDK ile neler yapabiliriz? İlk bakışta deneyimlediğim örnekleri paylaşmak isterim. Öncelikle sistemimizde Copilot CLI'ın yüklü olması gerekiyor. Bunu öğrenmek için komut satırından versiyon bilgisini alabiliriz velev ki yok o zaman [şu adresten indirip](https://docs.github.com/en/copilot/how-tos/copilot-cli/set-up-copilot-cli/install-copilot-cli) deneyebilirsiniz.

```bash
copilot --version
```

![HelloCopilotSDK_00.png](images/HelloCopilotSDK_00.png)

Teori oldukça basit. SDK, sanki VS Code AI chat pencersini veya Copilot CLI aracını kullanıyormuşuz gibi dil modelleri ile entegre uygulamalar geliştirebilmemizi sağlayan fonksiyonellikleri barındırıyor. Giriş örneğimizde basit bir Console uygulaması oluşturup gerekli Nuget paketini ekleyerek devam edelim.

```bash
# Console projesinin oluşturulması
dotnet new console -n HelloCopilotSDK
cd HelloCopilotSDK
# Copilot SDK Nuget paketinin projeye eklenmesi
dotnet add package GitHub.Copilot.Sdk
```

Resmi github dokümantasyonunun ilk örneğinde gpt-4.1'e 2+2 işlemi soruluyor (Lütfen bildiğiniz şeyler için yapay zeka araçlarını kullanmayın derim:D) Ama sonuçta SDK'yı tanımak için basit bir örnek hazırlamam da gerekiyor. Bu amaçlar en azından belli sayıda Fibonacci değerini Chuck Norris ile birlikte ele abileceğimiz bir sorguyla işi renklendirebiliriz diye düşünüyorum. İşte kodlarımız;

```csharp
using GitHub.Copilot.SDK;

await using var client = new CopilotClient();

var modelName = "claude-sonnet-4.5";
var intro = $"Starting Copilot session with model: {modelName}";
Console.WriteLine(intro);
Console.WriteLine(new string('-', intro.Length));
Console.WriteLine();

await using var session = await client.CreateSessionAsync(
    new SessionConfig
    {
        Model = modelName,
        OnPermissionRequest = PermissionHandler.ApproveAll,
        Streaming = true
    }
);

session.On(e =>
{
    switch (e)
    {
        case AssistantMessageDeltaEvent messageEvent:
            Console.Write(messageEvent.Data.DeltaContent);
            break;
        case SessionIdleEvent messageEvent:
            Console.WriteLine("");
            break;
    }
});

await session.SendAndWaitAsync(
    new MessageOptions
    {
        Prompt = "Give me the first 10 Fibonacci numbers and then find the related Norris Joke."
    }
);
```

Kodda neler yaptığımız şöyle bir bakalım dilerseniz. Bu console uygulaması ile Anthropic'in Claude Sonnet 4.5 modelini kullanarak bir konuşma gerçekleştirmek istiyoruz. Sorumuz da oldukça basit ama birbirleriyle tamamen alakasız iki parçadan oluşuyor:D

Öncelikle bir CopilotClient nesnesi örnekliyoruz ve bu nesne yardımıyla asenkron bir oturum (Session) başlatıyoruz. Malum istemci olarak bizim ve modelin arasında bir ağ trafiği söz konusu - tipik bir Client-Server tasarım diyebiliriz. SessionConfig sınıfını kullanarak oturum için gerekli bazı ayarlamaları yapıyoruz. Hangi modelle çalışacağımız dışında birde tüm yetki taleplerini kabul ediyoruz. Bunu sadece geliştirme aşamasında yaptığımızı belirtmek isterim. Normal şartlarda güvenlik gereği geçerli bir yetkilendirme (Authorize) ile birlikte ilgili modele gidilmesi gerekir. Ben şimdilik kolaya kaçtım zira bunu yapmadığım takdirde bir çalışma zamanı hatası almaktayım.

Diğer yandan açılan oturum sırasında modelden gelen bilgilerin anlık olarak görünmesini de sağlayabiliriz. Tahmin edileceği üzere Streaming özelliğine true değeri atamamızın sebebi bu. Streaming kullanmadığımız takdirde cevabı yine alırız ancak cevabın tamamı gelene kadar beklemek gerekir. Oysa Streaming ile cevabın parçalar halinde gelmesini sağlayarak cevabın o anki kısmını ekrana yazdırabiliriz. Takibimiz kolaylaşır.

Devam eden kısımda ise oturum sırasında gerçekleşen olayları (events) dinlediğimiz bir metod bloğu var. On metodu ile oturum sırasında gerçekleşen olayları dinliyoruz ki bu örnekte iki olayı ele almaktayız. Bunlardan birisi modelden gelen cevabın parçalarını temsil eden AssistantMessageDeltaEvent türünden olanı. Bu olay gerçekleştiğinde cevabın o anki parçasını ekrana yazdırma şansımız var. Diğer yandan SessionIdleEvent türünden bir olay da var ki bu da model cevabı tamamen döndükten sonra gerçekleşiyor ya da sistem Idle'a düştüğü zaman icra ediliyor desek daha mı doğru olur. Bilemiyorum Altan...Bu olay gerçekleştiğinde terminalde bir alt satıra geçip cevabın tamamlandığını belirtmiş oluyoruz.

Son olarak da SendAndWaitAsync metodu yardımıyla modele sormak istediğimiz soruyu gönderiyoruz. Bu metodun asenkron olduğunu ve oturumun sonlanmasını beklediğini de belirtelim. Bir başka deyişle cevap gelene kadar uygulama sonlanmayacaktır. İşte ilk örneğimizin çalışma zamanına ait bir çıktı.

![HelloCopilotSDK_01.png](images/HelloCopilotSDK_01.png)

Şimdi işi bir seviye daha ileri götürelim. Yapay zeka modelleri ile çalışırken çoğunlukla genel dil modellerinin eğitilmiş olduğu veri kümeleri göz önüne alınır. Oysa ki iş dünyasında üzerinde çalıştığımız sistemler ağırlıklı olarak belli domain çerçevelerine sahiptir. Bu nedenle modele gitmeden önce bazen ön hazırlıklar yapılır. Modelin özellikle bakmasını istediğimiz içerikler, sistem prompt'larının üretilmesinde kullanılır ve buda dil modelinin belirlediğimi çerçevedeki verilere bakarak çıktıları üretmesini kolaylaştırabilir.

Yapay zeka araçlarının sadece cevap vermek değil, görevler de icra edebildiği bir ortamda bu girdileri oturumlar sırasında kullanmak halüsinasyon riskini de azaltır. Tabii mevzu burada birkaç cümle ile açıklanabilecek gibi değil. İşin içerisine MCP (Model Context Protocol) sunucular, RAG (Retreival Augmented Generation) kurguları veya Fine-Tunning gibi ciddi kas gücü isteyen birçok mevzu ve dahası da dahil oluyor. Şimdilik bu detaylı ve kapsamlı konuları bir kenara bırakacağım ancak SDK, oturum sırasında modele giderken çeşitli araçları (tool) kullanabilme imkanı da sağlıyor. Bu araç yerel bir metod olabileceği gibi, harici bir servis çağrısı, MCP sunucusu da olabilir elbette. Gelin nasıl yapıldığına kısaca bir bakalım.

Resmi dokümanda bir şehir için rastgele hava sıcaklığı üreten bir fonksiyon kullanılmış. Yine basit ama farklı bir örnekle gidelim. Örneğin modelin bilgisayar sarf malzemeleri ile ilgili stok bilgisi veren bir araçtan yararlanmasını sağlayalım. Bu araç bize stokta bulunan ürünlerin isimlerini ve adetlerini versin. Hatta bunu bayii bazlı yapalım.

- "Ankara Merkez stoğunda hangi ürünlerden kaç adet var?"
- "Şişli bayisinde Envidya GTX 2000 ekran kartlarından kaç adet kaldı?"
- "Hangi bayilerde 32 GB RAM var?"
- "Stoğuna RAM bulunmayan bayiler hangileri?"
- "24 inç ve üstü monitör bulunduran bayiler hangileri?"

Buradaki düşünce tarzım genelde modele sorulacak soruları önceden çıkarıp gerekli araçları buna göre yazmak. Elimizde devasa bir veritabanı ve hatta vektörel olarak kavramlar arası ilişkilerin yakınlıklarını matematiksel ifade edebilen Rust ile yazılmış QDrant gibi bir veri tabanı olduğunda modelin yüksek tutarlıklı cevaplar üretmesi, görevler icra etmesi pekala mümkün oluyor. Bizzat deneyimledim, gözümle gördüm (İlk paragrafta dediğim gibi yapay zeka araçlarını sadece bir çet pencersinden ibaret şeylermiş gibi düşünmemek lazım)

Yaş alınca insanın çenesine vuruyor galiba. Çok konuştum sadede geleyim:D Senaryoyu renklendirmek adına bayi istatistiklerini yine.Net ile yazacağımız minimal bir Web API üzerinden çekebiliriz. Servis içeriğini pekala aşağıdaki gibi kodlayabiliriz.

```csharp
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

Dealer[] data = StockStatsApi.SeedData.GetDealers();

var dataApi = app.MapGroup("/dealers");
dataApi.MapGet("/", () => data)
        .WithName("GetAllStats");

dataApi.MapGet("/{id}", Results<Ok<Dealer>, NotFound> (int id) =>
    data.FirstOrDefault(a => a.Id == id) is { } dealer
        ? TypedResults.Ok(dealer)
        : TypedResults.NotFound())
    .WithName("GetDealerById");

app.Run();

public record Dealer(int Id, string Title, string City, List<Stats> Stats);
public record Stats(int Id, string Product, int Quantity);

[JsonSerializable(typeof(Dealer[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
```

Kodda yer alan SeedData.GetDealers () metodu statik bir kod dosyası ve tamamen hayali veriler içermekte. Yazıda yer alan örnekler ile ilgili github referanslarını son kısımda bulabilirsiniz. HTTP Rest modelinde çalışan servisimiz şu an için 5101 nolu porttan çalışıyor ve dealers, dealers/{id} gibi iki endpoint üzerinden hizmet veriyor.

![HelloCopilotSDK_02.png](images/HelloCopilotSDK_02.png)

Şimdi gelelim Copilot SDK kullanan istemci uygulama tarafına. Bu API'yi çağıran başka bir metodu tool olarak oturuma dahil edeceğiz. Program kodlarını aşağıdaki gibi geliştirerek devam edelim.

```csharp
using GitHub.Copilot.SDK;
using Microsoft.Extensions.AI;
using System.ComponentModel;

await using var client = new CopilotClient();

var getAllDealerStats = AIFunctionFactory.Create(
    () =>
    {
        // Optimizasyon için istemci tarafında servis çıktısı cache'lenebilir.
        var httpClient = new HttpClient();
        var response = httpClient.GetAsync("http://localhost:5101/dealers").Result;
        var content = response.Content.ReadAsStringAsync().Result;
        return content;
    },
    "get_all_dealer_stats",
    "Get the full dealer stats"
);

await using var session = await client.CreateSessionAsync(new SessionConfig
{
    Model = "claude-sonnet-4.5",
    Streaming = true,
    Tools = [getAllDealerStats],
    OnPermissionRequest = PermissionHandler.ApproveAll,
});

session.On(e =>
{
    switch (e)
    {
        case AssistantMessageDeltaEvent messageEvent:
            Console.Write(messageEvent.Data.DeltaContent);
            break;
        case SessionIdleEvent messageEvent:
            Console.WriteLine("");
            break;
    }
});

Console.WriteLine("📊  Dealer Stats Assistant (type 'exit' to quit)");
Console.WriteLine("   Try: 'Ankara Merkez stoğunda hangi ürünlerden kaç adet var?' or 'Şişli bayisinde Envidya GTX 2000 ekran kartlarından kaç adet kaldı?'\n");

while (true)
{
    Console.Write("--> ");
    var input = Console.ReadLine();

    if (string.IsNullOrEmpty(input) || input.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    Console.Write("Thinking...");
    await session.SendAndWaitAsync(new MessageOptions { Prompt = input });
    Console.WriteLine("\n");
}
```

Neler olduğuna kısaca bir bakalım. İşin belki de en can alıcı kısmı AIFunctionFactory.Create metodu ile AIFunction abstract sınıfı tarafından taşınabilen bir nesne örneği oluşturulması. Bu nesne aslında model tarafından kullanılacak olan fonksiyonelliği işaret ediyor. Beraberinde bu araçla ilgili kısa isimlendirme ve açıklama bilgilerine de veriyoruz. Model, gelen soruya istinaden çağrıyı hangi araca delege edeceğine karar verebiliyor ki bunu Session ayarlarında Tools özelliğindeki listede belirtiyoruz. Tam olarak AIFunction türünden bir generic ICollection listesi söz konusu. Dolayısıyla modelimiz, söz konusu oturum sırasında birden fazla aracı da ele alabilir. Örneğin bayi adına göre istatistik çeken bir servis metodumuz daha var dikkat ederseniz. Bunu çağıran bir başka araç da senaryoya dahil edilebilir. Bir deneyim derim.

Kodun akan kısmında sonsuz bir while döngüsü kullandığımız da dikkatlerden kaçmamış olsa gerek. Tipik bir chatbot deneyimi yaratmak istediğimiz için kullanıcıdan sürekli olarak girdi alıp modele gönderiyoruz taa ki kullanıcı exit komutu verene ya da CTRL+C ile uygulamayı sonlandırana kadar.

Pek tabii çalışma zamanında bayi istatitik bilgilerini getiren servisimizin ayakta olması gerektiğini hatırlatalım. Aksi durumda çalışma zamanı hatası alırız demek isterdim ancak işin içerisinde artık bir yapay zeka aracı var. Dolayısıyla sonuç pekala aşağıdaki gibi olabilir:D

![HelloCopilotSDK_04.png](images/HelloCopilotSDK_04.png)

Servisimizi ayağa kaldırdıktan sonra ise aşağıdaki ekran görüntüsündekine benzer bir deneyim yaşamanız olasıdır.

![HelloCopilotSDK_03.png](images/HelloCopilotSDK_03.png)

Aslında herhangibir yapay zeka aracının konuşma penceresindeymişiz gibi değil mi? İşte bunu belki de domain odaklı hale getirip daha işler hale getirmek gerekir. Hemen örnek bir senaryodan bahsedelim. Belli bir domain içerisindeki süreçlerin markdown formatında dokümante edilmiş olduğunu, dokümanlar arasındaki kavramsal ilişkilerin yüksek başarımlı text-embedding araçları ile vektörel bir veritabanına alındığını ve bu veritabanını sorgulayabildiğimiz api noktalarımız olduğunu düşünelim. Pekala bu servis noktaları birer araç haline getirilebilir ve terminal yerine görsel arabirimi olan bir istemci yazılıp analistlere sunulabilir. İşte size şirketinizin dokümanlarını kullanarak merak edilen sorulara cevaplar veren bir chat-bot uygulaması.

Ne yazık ki bu senaryoları kurgulamak oldukça maliyetli olabilir. Hatta şirket dışına çıkmak istemeyenlerin local ortamlarda ele alacağı dil modelleri söz konusu olursa donanım ve işlem zamanı maliyetleri beklenmedik değerlere ulaşabilir. Tabii ki şimdilik. Dokümanları kalitesi, sayısı, formatı, parçalama (chunking) tekniği, veritabanının hızı, ne kadar güncel tutulduğu gibi birçok ufak detayı hiç saymıyorum bile. Lakin birçok optimizasyon tekniği burada ele alınabilir durumda. Yakın geleceekte optimizasyon problemlerine farklı parametrelerin gireceği aşikar...diyerek bu yazımızı da sonlandırmak istiyorum.

Bu çalışmada Copilot SDK'yi.Net tabanlı uygulamalarda nasıl kullanabileceğimize dair çok basit iki örnek ele aldık. Sonraki aşamada bir MCP server'ın kullanımını ele alabiliriz de. Hatta kendi tasarladığımız bir MCP server'da pekala işi renklendirir ve nasıl yapılıyor tadından bir içerik ortaya çıkabilir. Bakalım zaman ayırabilecek miyim. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[İlk örneğimize ait github kodları](https://github.com/buraksenyurt/friday-night-programmer/tree/main/src/HelloCopilotSDK)

[Tool kullandığımız ikinci örneğimize ait github kodları](https://github.com/buraksenyurt/friday-night-programmer/tree/main/src/UseToolOnCopilotSDK)

[Kobay api servisimize ait github kodları](https://github.com/buraksenyurt/friday-night-programmer/tree/main/src/StockStatsApi)
