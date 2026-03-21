---
layout: post
title: "Tie Fighter Değil, Project Tye!"
date: 2021-03-30 15:00:00 +0300
categories:
  - dotnet-core
tags:
  - .net-core
  - .net
  - kubernetes
  - docker
  - redis
  - rabbitmq
  - csharp
  - pods
  - container
  - yaml
  - sidecar-pattern
  - dağıtık-mimari
  - tye
  - kubectl
  - wsl
  - worker-service
  - service-discovery
---
Star Wars'ın figür kabul edilen gemilerinden birisi imparatorluk güçlerinin Tie Fighter'ıdır. Lord Vader ile özdeşlemiş olan bu figürün kulak tırmalayan ama rahatsız etmeyen sesinin Almanların İkinci Dünya savaşındaki hafif bombardıman uçaklarından birisi olan Junkers Ju-87 Stuka'dan (Sturzkampfflugzeug) geldiği bile söylenir.

![tie-fighter.png](/assets/images/2021/tie-fighter.png)

Aslında ses tasarımcısı Ben Burtt bu efekti oluşturmak için bir filin başka bir file seslenirken çıkardığı bağrış ile ıslak kaldırımda giden araba seslerini birleştirmiştir. Lakin Tie kelimesi okunurken genellikle Tay veya Taiy diye okunur. Belki de okunmaz:P Benzer sesdeşlik Tie ile Tye arasında da vardır. Ancak Tye esasında Microsoft'un deneysel bir çalışmasıdır.

Github'un [şuradaki](https://github.com/dotnet/tye) reposunda açık kaynak olarak yayınlanan Project Tye, Microsoft'un deneysel projelerinden birisi. En azından konuya çalıştığım tarih itibariyle böyleydi. Projenin iki temel amacı var;.Net tabanlı mikroservis çözümlerinin daha kolay geliştirilmesini sağlamak ve söz konusu çözümleri az zahmetle Kubernetes ortamına almak (Deployment) Buna göre birden fazla servisi tek komutla ayağa kaldırmak, Redis, RabbitMQ, Zipkin, Elastic Stack, Ingress vb normalde Sidecar container olabilecek bağımlılıkları kolayca yönetmek, kullanılacak servislerin ortam bağımsız rahatça keşfedilmesini sağlamak (Service Discovery), uygulamaların container olarak evrilmesi için gerekli hazırlıkları otomatikleştirmek, olabildiğince basit ve tekil bir Kubernetes konfigurasyon dosyası desteği vermek, projenin genel amaçları olarak düşünülebilir.

Elbette bu komut satırı aracının faydalarını görebilmek için sahada denemek gerekir. Bu anlamda yararlandığım başlıca iki önemli kaynak var. Amazon'dan kısa süre önce aldığım [Adopting.NET 5: Understand modern architectures, migration best practices, and the new features in.NET 5](https://www.amazon.com/Adopting-NET-Understand-architectures-migration/dp/1800560567) isimli kitap ve Microsoft Program Yöneticisi rolünde çalışan Amiee Lo'nun [şu adresteki](https://devblogs.microsoft.com/aspnet/introducing-project-tye/) giriş makalesi. Her iki kaynaktaki örnekleri de kopyalama yapmadan bizzat yazarak çalıştım ve sonuçta github reposundan bazı notlar birikti. Şu anda bu notları bir araya topladığım yazıyı okumaktasınız.

Örneklere geçmeden önce uygulamaları geliştirdiğim sistemden bahsetmem gerekiyor. Windows 10 üzerinde, Visual Studio 2019 Community Edition kullanıyorum. Ortamda.Net 5 yüklü durumda. Kubernetes özelliği aktif olan bir Docker Desktop var. Dolayısıyla sonradan ihtiyacımız olacak kubectl komut satırı aracı kullanılabilir halde. Ayrıca Windows Subsystems on Linux (WSL), 2.0 sürümüne güncellenmiş durumda. Geliştireceğimiz her iki örnekte Service Discovery için yerel bir adres kullanacak ancak gerçek hayat senaryolarında bunun yerini DockerHub veya Azure Container Registry gibi bir hizmet alması muhtemeldir. Tabii tüm bunların yanında bize tye komut satırı aracının kendisi de lazım:D İşte başlangıç adımları için gerekli terminal komutlarımız.

```bash
# Sisteme tye yüklemek için aşağıdaki terminal komutu kullanılabilir(Son sürüme bakmak lazım. Sonuçta bu şimdilik deneysel bir proje)
dotnet tool install -g Microsoft.Tye --version "0.5.0-alpha.20555.1"

# Kubernetes deployment öncesi Service Discovery için kullanacağımız local registry
docker run -d -p 5000:5000 --restart=always --name registry registry:2

# Docker Desktop tarafında Enable Kubernetes seçeneğinin de işaretli olması lazım
# Kubernetes'in etkin olduğunu anlamak içinse aşağıdaki komut işletilebilir
kubectl config current-context
# Bize docker-desktop cevabını vermeli
```

Hello World Örneği: StarCups

StarCups kod adlı ilk çalışmada bir frontend, bir backend (servis tabanlı) ve birde Redis mevzu bahis. Senaryoda StarCups isimli hayali bir kahve firması var. HeadOffice isimli web arayüzünden İstanbul'un çeşitli semtlerindeki kahve dükkanlarının malzeme taleplerini anlık olarak görebiliyoruz. Malzeme bilgileri StockCollector isimli REST tabanlı çalışan bir Web API servisi üstünden geliyor. Redis ise StockCollector'un çektiği veriyi belli süre cache'lemek için kullanılıyor (Aslında en genel uygulama geliştirme pratiği olarak düşünebiliriz. Önyüz tarafı iş fonksiyonellikleri için arka taraftaki bir servisle konuşur) Bu Hello World kıvamındaki örnekte amaç, Tye aracı ile uygulamaların kolayca ayağa kaldırılması, denenmesi, zahmetsizce dockerize edilmesi, loglarına bakılması, çevre değişkenlerinin yaml bazlı yönetilmesi ve Kubernetes tarafına en basit şekliyle Deploy edilmesi şeklinde özetlenebilir. İlk çözümü oluşturmak için aşağıdaki terminal komutları ile hareket edebiliriz.

```bash
mkdir Starcups
cd Starcups
# Bir tane frontend uygulaması. Razor tipinde.
dotnet new razor -n HeadOffice
# frontend'in konuşacağı bir WebAPI
dotnet new webapi -n StockCollector
dotnet new sln
dotnet sln add HeadOffice StockCollector

tye run
```

Bu komut sonrası solution içerisindeki uygulamalar otomatik olarak kendileri için tahsis edilmiş process ve adreslerden ayağa kalkacaktır.

![Mart_screenshot_1.png](/assets/images/2021/Mart_screenshot_1.png)

Şu haldeyken tye ile çözümü çalıştırıp localhost:8000 adresine gidebiliriz. Her iki uygulama da Dashboard üstünde görünür ve ayrı ayrı incelenebilir ki inceleyin derim:) View kısmına bir bakın, Bindings kısmından sayfalara gitmeye çalışın. Tabii Api servis için bir rest çağrısı şeklinde gitmeniz gerekir.

![Mart_screenshot_2.png](/assets/images/2021/Mart_screenshot_2.png)

Şık ve uygulamaların kolayca erişilip, loglarına bakıldığı arayüz dışında ortada henüz bir numara yok. Örneğin frontend ile backend şu anda birbirlerinden bihaberler. Frontend'in backend ile konuşuyor olması da lazımdı. Şimdi WebAPI tarafına OrderData sınıfını ekleyip WeatherForecastController tipini de OrderController olarak değiştirip kodlayarak ilerleyelim.

OrderData sınıfımız;

```csharp
using System;

namespace StockCollector
{
    public class OrderData
    {
        public string ShopName { get; set; }

        public string ItemName { get; set; }

        public double Quantity { get; set; }
        public DateTime Time { get; set; }
    }
}
```

OrderController sınıfımız;

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace StockCollector.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private static readonly string[] ShopNames = new[]
        {
            "Capitol", "Balat", "Taksim Meydan", "Pendik Marina", "Bebek", "Koşuyolu", "Bakırköy", "Moda", "Beşiktaş Arena", "Maslak 1881"
        };
        private static readonly string[] Items = new[]
        {
            "Peçete (100 * Adet)", "Karıştırma Kaşığı (100 * Adet)", "Şeker (Kilo)","Short Bardak (100 * Adet)"
        };

        private readonly ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<OrderData> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 10).Select(index => new OrderData
            {
                ItemName = Items[rng.Next(Items.Length)],
                Quantity = rng.Next(1, 10),
                ShopName = ShopNames[rng.Next(ShopNames.Length)],
                Time = DateTime.Now
            }).ToArray();
        }
    }
}
```

Kod rastgele OrderData nesneler listesi üretip geri döndüren basit bir operasyona sahip. Frontend tarafının bu servise gelmesini istiyoruz. Normal şartlarda localhost üstündeki ilgili backend adresini alıp kullanan bir HttpClient nesnesi pekala işimizi görebilir. Lakin bu örneği yarın öbür gün Kubernetes'e alacağız. Dockerize edilerek çalışacak Container için adres bilgileri çevre değişkenlerden gelebilir, hatta uzak bir konfigurasyon yöneticisinden bile desteklenebilir. Yani frontend'in hangi servisteki backend uygulaması ile konuşacağını kolayca keşfedebilmesi önemlidir. Bu işi tye üstünden yapmak istediğimiz için frontend tarafında küçük bir hazırlık yapmalıyız. İlk olarak Microsoft.Tye.Extensions.Configuration nuget paketini HeadOffice uygulamasına ekleyelim.

```bash
cd HeadOffice
dotnet add package --prerelease Microsoft.Tye.Extensions.Configuration
cd ..
```

Sonrasında HeadOffice isimli frontEnd uygulamasından REST çağrısı yaparken kullanacağımız OrderClient ve gelen veriyi nesne olarak ele alacağımız OrderData (Backend taraftaki ile aynı yapıdadır) sınıflarını geliştirelim.

OrderClient sınıfı REST çağrısı yapmamızı kolaylaştıran bir tip.

```csharp
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace HeadOffice
{
    public class OrderClient
    {
        private readonly JsonSerializerOptions options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        private readonly HttpClient client;

        public OrderClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<OrderData[]> GetOrdersAsync()
        {
            var responseMessage = await this.client.GetAsync("/order");
            var stream = await responseMessage.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<OrderData[]>(stream, options);
        }
    }
}
```

Derken HeadOffice'deki Index.cshtml (cs ile birlikte) sayfasını da aşağıdaki gibi düzenleyelim.

```text
@page
@model IndexModel
@{
    ViewData["Title"] = "Home page";
}

<div class="text-center">
    <h1 class="display-4">Melaba!!! Kahvenin hası burada.</h1>
    <p>Star Cups Mağzaları...</a>.</p>
</div>

Son Siparişler

<table class="table">
    <thead>
        <tr>
            <th>Tarih</th>
            <th>Dükkan</th>
            <th>İstenen</th>
            <th>Miktar</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var ord in @Model.Orders)
        {
            <tr>
                <td>@ord.Time.Ticks</td>
                <td>@ord.ShopName</td>
                <td>@ord.ItemName</td>
                <td>@ord.Quantity</td>
            </tr>
        }
    </tbody>
</table>
```

Index.cshtml.cs sınıfı

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace HeadOffice.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public OrderData[] Orders { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public async Task OnGet([FromServices] OrderClient client)
        {
            Orders = await client.GetOrdersAsync();
        }
    }
}
```

Tekrar tye tarafına dönelim. Çözüm içerisindeki servislerle ilgili çevre konfigurasyon ayarlamaları için bir yaml dosyasına ihtiyacımız olacak. Bu dosyayı solution klasöründe aşağıdaki terminal komutu ile kolayca oluşturabiliriz.

```bash
tye init
```

Tye.yaml içeriği aşağıdaki gibi oluşur. Buna göre iki servis söz konusudur. Tye,.net odaklı bir enstrüman olduğundan solution içindeki proje dosyalarını otomatik olarak algılayıp gerekli servis bildirimlerini yapar.

```text
name: starcups
services:
- name: headoffice
  project: HeadOffice/HeadOffice.csproj
- name: stockcollector
  project: StockCollector/StockCollector.csproj
```

Bu aşamada çözüm çalıştırılır ve tarayıcı ile HeadOffice uygulamasına gidilirse ekran görüntüsünde olduğu gibi servis tarafıyla konuşulabildiği görülür. Şu noktada HeadOffice tarafında, backend için bir adres bildirimi yapmadığımız dikkatinizden kaçmamalıdır. Tye çalışmaya başladığında backend'i hangi adresten ayağa kaldırdıysa, frontend tarafında da o adres kullanılır.

```bash
tye run
```

## ![screenshot_3.png](/assets/images/2021/screenshot_3.png)
StarCups için Redis Desteğinin Eklenmesi

Dağıtık mimariler söz konusu olduğunda Redis, RabbitMQ gibi hizmetler eğer single node üstünde çalışılıyorsa genellikle Sidecar Container olarak ele alınabilirler. Tye bu konuda bize bazı kolaylıklar sağlar. Ne demek istediğimi anlatamabilmek için backend servisine Redis desteğini ekleyerek devam edelim. Redis desteği'ni de yaml dosyaları ile yöneteceğiz. Öncelikle backend uygulamasında Redis kullanabilmek için gerekli Nuget paketini ilave ediyoruz.

```bash
cd StockController
dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis
cd ..
```

Sonrasında OrderController sınıfındaki Get metodunu Redis'i kullanacak hale getiriyoruz.

```csharp
[HttpGet]
public async Task<string> Get([FromServices] IDistributedCache cache)
{
	var keyOrder = await cache.GetStringAsync("keyOrder");
	if (keyOrder == null)
	{
		_logger.LogInformation("Redis Key boştu");
		var rng = new Random();
		var orders = Enumerable.Range(1, 10).Select(index => new OrderData
		{
			ItemName = Items[rng.Next(Items.Length)],
			Quantity = rng.Next(1, 10),
			ShopName = ShopNames[rng.Next(ShopNames.Length)],
			Time = DateTime.Now
		}).ToArray();

		keyOrder = JsonSerializer.Serialize(orders);
		_logger.LogInformation($"Veri serileştirildi {keyOrder}");

		await cache.SetStringAsync("keyOrder", keyOrder, new DistributedCacheEntryOptions
		{
			AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
		});
	}
	return keyOrder;
}
```

ve Redis için Startup.cs içerisindeki ConfigureService metodunda gerekli düzenlemeyi yapıyoruz.

```csharp
public void ConfigureServices(IServiceCollection services)
{
	services.AddControllers();

	// Redis için aşağıdaki satır eklendi
	// Bağlantı bilgisi yaml üstünden gelecek
	services.AddStackExchangeRedisCache(o =>
	{
		o.Configuration = Configuration.GetConnectionString("redis");
	});

	services.AddSwaggerGen(c =>
	{
		c.SwaggerDoc("v1", new OpenApiInfo { Title = "StockCollector", Version = "v1" });
	});
}
```

Burada altını çizmemiz gereken bir nokta var ki o da GetConnectionString'e gelen redis ifadesi. Normalde projemizin appSettings.json dosyasında redis için bir bölüm bulunmuyor. Tahmin edeceğiniz üzere buradaki redis adres tanımı tye.yaml üstünden okunuyor. Bu nedenle tye.yaml içeriğini aşağıdaki şekilde güncellemeliyiz.

```text
name: starcups
services:
- name: headoffice
  project: HeadOffice/HeadOffice.csproj
- name: stockcollector
  project: StockCollector/StockCollector.csproj
- name: redis
  image: redis
  bindings:
  - port: 6379
    connectionString: "${host}:${port}"
- name: redis-cli
  image: redis
  args: "redis-cli -h redis MONITOR"
```

Güncel yaml içeriğinde redis ve redis-cli isimli iki yeni bildirim görüyorsunuz. Standart olarak 6379 portundan hizmet veren redis sunucusu ve kolay bir şekilde onu monitor etmemizi sağlayan redis-cli hizmeti.

Artık backend uygulaması Redis ile çalışır hale geldi. Bu aşamada yine tye run ile örneği çalıştırıp, redis servislerinin ayağa kalkıp kalkmadığına bakmak ve 10 saniyede bir cache'in düşüp yeni bilgilerin getirildiğini görmek iyi olacaktır. tye run ile sistem ayağa kaldırıldığında aşağıdaki ekran görüntüsünden de görüldüğü gibi redis hizmeti de çalışmaya başlar. Bu arada redis için docker imajı kullanıldığını fark etmiş olmalısınız. Yani redis hizmeti bir Container olarak ayağa kalkar. Aynı işleyip redis-cli hizmeti için de söz konusudur (Buradan terminal komutu loglarını okumanın faydalarını da görebilirsiniz)

![Mart_screenshot_4.png](/assets/images/2021/Mart_screenshot_4.png)

Tye dashboard üstünde de benzer şekilde redis ve redis-cli hizmetlerinin çalışıyor olduğunu görmemiz lazım.

![Mart_screenshot_5.png](/assets/images/2021/Mart_screenshot_5.png)

Hatta redis-cli loglarına gidersek cache'e atılan JSON içeriklerini de takip edebiliriz.

![Mart_screenshot_6.png](/assets/images/2021/Mart_screenshot_6.png)

## StartCups'ın Kubernetes Ortamına Alınması

Gelelim diğer bir hedefimize. Buraya kadar yapılan işlemler sayesinde solution içindeki uygulamaları bağımlı servisleri ile birlikte basitçe çalıştırıp, monitör edebildik. Ancak bunları Kubernetes gibi bir ortama nasıl alırız? Bu aşamada Sidecar gibi görünen redis için ayrı bir yaml dosyasına ihtiyacımız olacak. Bunu redis servisini Kubernetes ortamına ayrıca almak için kullanacağız. Söz konusu dosyayı aşağıdaki gibi oluşturabiliriz.

redis.yaml

```yml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: redis
  labels:
    app.kubernetes.io/name: redis
    app.kubernetes.io/part-of: starcups
spec:
  selector:
    matchLabels:
      app.kubernetes.io/name: redis
  replicas: 1
  template:
    metadata:
      labels:
        app.kubernetes.io/name: redis
        app.kubernetes.io/part-of: starcups
    spec:
      containers:
        - name: redis
          image: redis
          resources:
            requests:
              cpu: 100m
              memory: 100Mi
          ports:
            - containerPort: 6379

---
apiVersion: v1
kind: Service
metadata:
  name: redis
  labels:
    app.kubernetes.io/name: redis
    app.kubernetes.io/part-of: starcups
spec:
  ports:
    - port: 6379
      targetPort: 6379
  selector:
    app.kubernetes.io/name: redis
```

Redis için kubernetesçe bir içerik söz konusu. Kubernetes konusuna çok hakim olmadığım için anladığım kadarıyla ifade etmeye çalışayım. Kubernetes'e redis için kullanacağı docker imajını, replika adedini, port bilgisini, cpu ve memory gibi ayrılması istenen sistem kaynaklarını, kısaca dağıtım ve servis manifestosunu bildiriyoruz. Bu manifestoyu Kubernetes tarafının işletmesi içinse aşağıdaki terminal komutunu kullanmamız gerekiyor (Yazının başlarında kubectl'ye ihtiyacımız olacağını söylemiştim)

```bash
kubectl apply -f redis.yaml
```

Redis'in Kubernetes tarafında ayağa kaldırılması tek başına yeterli değil. Buraya yapılan dağıtım sonrası servislerin keşfi için de bir registry kullanılması gerekiyor. Bunu tye.yaml dosyasında aşağıdaki gibi bildirebiliriz.

```bash
name: starcups
registry: localhost:5000
services:
- name: headoffice
# Diğer kısımlar
```

Tabii bunu söylemek de yeterli değil. localhost:5000 adresinde gerçekten bir Registry servisinin olması lazım. Bunun içinse aşağıdaki terminal komutuna ihtiyacımız var. registry imajını kullanan ve açıkça kapatılana kadar sürekli çalışacak bir container.

```bash
# container registry için aşağıdaki komut kullanılabilir.
docker run -d -p 5000:5000 --restart=always --name registry registry:2
```

Kubernetes deployment işlemi için deploy komutunu aşağıdaki gibi kullanmamız gerekiyor. Harici bir servis olarak Redis kullandığımızdan, ona hangi adresle erişeceğimiz de sorulur. Bu soruyu redis:6379 şeklinde cevaplayarak ilerleyebiliriz.

```bash
tye deploy --interactive

# Aşağıdaki komutlar ile kubernetes deployment ve pod durumları kontrol edilir.

kubectl get deployment
kubectl get svc
kubectl get secrets
kubectl get pods
```

İşlemler sırasında terminal hareketlilikleri takip edilirse, tye.yaml üstünde belirtilen projeler için Dockerize işlemlerinin otomatik olarak yapıldığı da görülebilir. Dikkat ederseniz herhangibir Dockerfile oluşturmadık. Deployment işlemi başarılı ise get pods ile aktif olarak çalışan pod'ları görebilmemiz gerekir. Aynen aşağıdaki ekran görüntüsüne olduğu gibi.

![Mart_screenshot_7.png](/assets/images/2021/Mart_screenshot_7.png)

Frontend uygulamasının web arayüzüne erişmek için port-forward işlemi uygulamamız gerekebilir (Cluster dışından erişmek istediğimiz için) Bunun için aşağıdaki terminal komutunu çalıştırmak yeterli olacaktır.

```bash
kubectl port-forward svc/headoffice 80:80
```

Sonrasında localhost:80 adresine gidilirse web uygulamasına ulaşıldığı ve anlık olarak kahve dükkanlarımızın beklediği malzemeler görülebilir. Aynen aşağıdaki ekran görüntüsünde olduğu gibi.

![Mart_screenshot_8.png](/assets/images/2021/Mart_screenshot_8.png)

Çok doğal olarak şu noktada Kubernetes ortamına yapılan dağıtımı geri almak isteyebilirsiniz. Tye bu işlemi basitleştirir.

```bash
tye undeploy
```

Peki şimdi ne oldu? Normal bir.net çözüm ailesini kullanışlı bir dashboard üstünden izlemeyi, kolayca çalıştırmayı (Solution Run'dan farklı olarak), redis'i hem development hem kubernetes için nerededir diye düşünmeden bağlamayı ama daha da önemlisi bu çözümü kubernetes'e taşımak istersek o ortamda da çalışabileceğini görmüş olduk. Sizde bu örneği güzel bir şekilde tamamladıysanız ikincisine geçebiliriz. Bu kez senaryo [okuduğum kitaptan](https://www.amazon.com/Adopting-NET-Understand-architectures-migration/dp/1800560567) geliyor.

Bonus: SchoolOfMath Senaryosu

Yeni pratiğimizde aşağıdaki şekilde görülen senaryo söz konusu olacak.

![Project_Tye_Senaryo.png](/assets/images/2021/Project_Tye_Senaryo.png)

Çok daha keyifli bir senaryo olduğunu söyleyebilirim. Benim için yeni deneyimler içeriyordu. Kısaca çözümdeki aktörlerin ne işe yaradığını anlatarak devam edelim.

- Einstein, gRPC tabanlı bir servis sağlayıcı. İçinde Palindrom sayıları hesap eden (Kitapta asal sayı buluyordu:P) bir fonksiyon desteği sunuyor. Servis cache stratejisi için Redis'i kullanacak. Cache'te ne mi tutacağız? Daha önceden Palindrome olarak işaretlenmiş bir sayı varsa onu kendi adıyla Cache'e alacağız ve bir saat boyunca saklamasını isteyeceğiz. Aynı sayı tekrar istenirse hesaplanmadan doğrudan cache'den gelecek. Sırf Redis hizmeti bu senaryoda olsun diye. Ayrıca bir mesaj kuyruğu sistemi de var ki bu noktada RabbitMQ'dan yararlanacağız.
- Evelyne, Bruce ve Madeleine aktörleri Worker tipinden istemci servisler (Onları, ayağa kalktıktan sonra sürekli olarak talep gönderen servisler olarak düşünebiliriz) Belli bir sayıdan başlayarak Eintesein'a talep gönderiyorlar ve gönderikleri sayının Palindrom olup olmadığını öğreniyorlar.
- Robert ise RabbitMQ kuyruğunu dinleyen diğer bir Worker servis.

Amacımız bir önceki örnekte olduğu gibi bu çözümü Tye destekli olarak inşa edip az zahmetle Kubernetes'e alabilmek.

## Proje İskeletinin Oluşturulması

Bunun için aşağıdaki adımları icra edelim. Öncelike Palindrom sayı hesaplayan Einstein gRPC servisini geliştirelim.

```bash
mkdir SchoolOfMath
cd SchoolOfMath

dotnet new sln
dotnet new grpc -n Einstein
dotnet sln add Einstein
```

Protos klasöründeki greet.proto ile servis tarafını değiştirmemiz gerekiyor.

palindrome.proto içeriği şöyle oluşturulabilir. long tipinden değer alıp bool olarak cevap veren iki mesaj söz konusu. Fonksiyonumuz ise IsItPalindrome. gRPC için gerekli şemayı bu şekilde tanımlamış olduk.

```csharp
syntax = "proto3";

option csharp_namespace = "SchoolOfRock";

package palindrome;

service PalindromeFinder {
  rpc IsItPalindrome (PalindromeRequest) returns (PalindromeReply);
}

message PalindromeRequest {
  int64 number= 1;
}

message PalindromeReply {
  bool isPalindrome= 1;
}
```

PalindromeFinderServis sınıfı;

```csharp
using Grpc.Core;
using Microsoft.Extensions.Logging;
using SchoolOfRock;
using System.Threading.Tasks;

namespace Einstein
{
    public class PalindromeFinderService
        : PalindromeFinder.PalindromeFinderBase
    {
        private readonly ILogger<PalindromeFinderService> _logger;
        public PalindromeFinderService(ILogger<PalindromeFinderService> logger)
        {
            _logger = logger;
        }

        public override async Task<PalindromeReply> IsItPalindrome(PalindromeRequest request, ServerCallContext context)
        {
            long r, sum = 0, t;
            var num = request.Number;
            for (t = num; num != 0; num /= 10)
            {
                r = num % 10;
                sum = sum * 10 + r;
            }
            if (t == sum)
                return new PalindromeReply { IsPalindrome = true };
            else
                return new PalindromeReply { IsPalindrome = false };
        }
    }
}
```

Servis tarafını şimdilik bırakalım ve ilk istemci uygulama kodlarını yazarak devam edelim.

```bash
dotnet new worker -n Evelyne
dotnet sln add Evelyne
# Evelyne'nin gRPC servisini kullanabilmesi için gerekli Nuget paketleri eklenmelidir.
cd Evelyne
dotnet add package Grpc.Net.Client
dotnet add package Grpc.Net.ClientFactory
dotnet add package Google.Protobuf
dotnet add package Grpc.Tools
# Ayrıca Tye konfigurasyonu için gerekli extension paketi de yüklenir
dotnet add package --prerelease Microsoft.Tye.Extensions.Configuration
cd ..
```

Visual Studio 2019 kullanıyorsak Add new gRPC Service Reference (Connected Services kısmından) ile Einstein'daki proto dosyasının fiziki adresini göstererek gerekli proxy tipinin üretilmesini kolayca sağlayabiliriz. İşte bu noktalarda Visual Studio ile çalışmanın avantajları ortaya çıkıyor. Uygulamanın program.cs ve worker.cs içeriklerini de düzenlememiz lazım.

Program sınıfı;

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SchoolOfRock;
using System;

namespace Evelyne
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // gRPC istemcisini çalışma zamanına ekliyoruz
                    services.AddGrpcClient<PalindromeFinder.PalindromeFinderClient>(options =>
                    {
                        // servis adresini Tye extension fonksiyonu üstünden çekiyoruz
                        // Eğer debug modda çalışıyorsak (tye.yaml olmadan tye run ile mesela) einstein'ın 7001 nolu adresine yönlendiriyoruz.
                        options.Address = hostContext.Configuration.GetServiceUri("einstein") ?? new Uri("https://localhost:7001");
                    });
                    services.AddHostedService<Worker>();
                });
    }
}
```

Program sınıfında gRPC servis adresinin nasıl alındığına dikkat edelim.

Worker sınıfı 100 milisaniyede bir Einstein servisine talep gönderecek şekilde kodlanmış durumda.

```csharp
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SchoolOfRock;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Evelyne
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly PalindromeFinder.PalindromeFinderClient _client;

        // gRPC servisini constructor üzerinden içeriye enjekte ediyoruz
        public Worker(ILogger<Worker> logger,PalindromeFinder.PalindromeFinderClient client)
        {
            _logger = logger;
            _client = client;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Servisin ayağa kalkması için bir süre bekletiyoruz. Makine soğuk. 
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            _logger.LogInformation("### Servis başlatılıyor ###");
            long number = 1; // Evelyne, 1den itibaren sayıları hesap etmeye başlayacak
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var response = await _client.IsItPalindromeAsync(new PalindromeRequest { Number = number });
                    _logger.LogInformation($"{number}, palindrom bir sayıdır önermesinin cevabı = {response.IsPalindrome}\r");
                }
                catch (Exception ex)
                {
                    // Bir exception oluşması halinde Worker'ın işleyişini durduracağız
                    if (stoppingToken.IsCancellationRequested) 
                        return;
                    
                    _logger.LogError(-1, ex, "Bir hata oluştu. Worker çalışması sonlanıyor.");
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }

                number++;

                if (stoppingToken.IsCancellationRequested) 
                    break;

                await Task.Delay(TimeSpan.FromMilliseconds(100), stoppingToken); // İstemci 100 milisaniyede bir ateş edecek :P
            }
        }
    }
}
```

İlk Worker servise benzer şekilde Bruce ve Madeleine isimli Worker servisleri de ekleyerek devam edebiliriz. Buradaki kodlar benzer olduğu için eklemedim ancak github üstünden alabilir ya da aşağıdaki notlarda olduğu gibi Palindrome başlangıç değerleriyle oynayarak yukarıdaki kodu kullanabilirsiniz.

```bash
# Bruce için tek fark Palindrome sayı taleplerine 1den değil de 10000den başlamasıdır
dotnet new worker -n Bruce
dotnet sln add Bruce
cd Bruce
dotnet add package Grpc.Net.Client
dotnet add package Grpc.Net.ClientFactory
dotnet add package Google.Protobuf
dotnet add package Grpc.Tools
dotnet add package --prerelease Microsoft.Tye.Extensions.Configuration
cd ..

# Madeleine de benzer şekilde eklenir
dotnet new worker -n Madeleine
dotnet sln add Madeleine
cd Madeleine
dotnet add package Grpc.Net.Client
dotnet add package Grpc.Net.ClientFactory
dotnet add package Google.Protobuf
dotnet add package Grpc.Tools
dotnet add package --prerelease Microsoft.Tye.Extensions.Configuration
cd ..
```

Yukradaki işlemler tamamlandıktan sonra en azından aşağıdaki terminal komutu ile servislerin ayağa kalkıp kalkmadığına bakmakta yarar var. Bu arada uygulamalarımız için herhangi bir Dockerize işleminin olmadığı dikkatinizden kaçmamıştır diye düşünüyorum. Nitekim henüz Kubernetes hazırlıklarına başlamadık. Bu nedenle tye söz konusu uygulamaları localhost:random_port_number formasyonunda birer process olarak ayağa kaldırmıştır.

```bash
tye run
```

![Nisan_screenshot_2.png](/assets/images/2021/Nisan_screenshot_2.png)

## ![Nisan_screenshot_1.png](/assets/images/2021/Nisan_screenshot_1.png)
Redis ve RabbitMQ Desteğinin Eklenmesi

İlk Hello World örneğinde Redis desteğini eklemiştik. Aynı adımları burada da uygulayacağız. Ayrıca rabbitmq hizmetini de dahil edeceğiz. Özellikle dağıtık mimarinin event-based modelinde uygulamalar arası haberleşmede mesaj bazlı kuyruk sistemleri sıklıkla karşımıza çıkıyor. Kafka ve RabbitMQ sanıyorum ki en çok başvurduklarımız. Dolayısıyla RabbitMQ için aranan Sidecar container'lardan birisi olduğunu ifade etsek yeridir. Şimdi gelin bu iki aktörü sisteme dahil ederek Kubernetes hazırlıklarına geçelim.

```bash
# İşe tye.yaml dosyasının oluşturulmasıyla başlıyoruz.
tye init

# tye.yaml dosyasına redis için gerekli ekleri yaptıktan sonra
# einstein (gRPC API servisimiz) cache desteği için gerekli nuget paketlerini ekleyip devam ediyoruz
cd einstein
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis
#Sonrasında rabbitmq paketini ekliyoruz.
dotnet add package RabbitMQ.Client
cd ..
```

Palindrome sayılar buldukça bunları RabbitMQ'ya mesaj olarak yollayacak bir düzenek ekleyeceğimizi de söylemiştik. RabbitMQ'da, Redis gibi çalışma zamanında ayakta olması beklenen bir servis. Bu nedenle tye.yaml dosyasında RabbitMQ için gerekli eklemeler aşağıdaki gibi yapılmalı.

```bash
name: schoolofmath
registry: localhost:5000 # container registry adresi
services:
- name: einstein
  tags:
    - backend
  project: Einstein/Einstein.csproj
  replicas: 1
  env: #rabbitmq için kullanıcı adı, şifre ve varsayılan kuyruk adı bildirimi
  - RABBIT_USER=guest
  - RABBIT_PSWD=guest
  - RABBIT_QUEUE=palindromes
- name: evelyne
  tags:
    - client
  project: Evelyne/Evelyne.csproj
- name: bruce
  tags:
    - client
  project: Bruce/Bruce.csproj
- name: madeleine
  tags:
    - client
  project: Madeleine/Madeleine.csproj
- name: robert
  tags:
    - middleware
  project: Robert/Robert.csproj
- name: redis
  tags:
    - backend
  image: redis
  bindings:
  - port: 6379
    connectionString: "${host}:${port}"
- name: redis-cli #redis cache tarafında ne olduğunu izlemek için ekledik. Ancak mecburi değil. Opsiyonel.
  tags:
    - backend
  image: redis
  args: "redis-cli -h redis MONITOR"
- name: rabbitmq # RabbitMQ servisini MUI arabirimi ile birlikte ekliyoruz.
# Mui arabirimine aşağıdaki kriterlere göre localhost:15672'den quest/quest log in bilgisi ile erişebiliriz
  tags:
    - middleware
  image: rabbitmq:3-management
  bindings:
  - name: mq-binding # mq_binding veya mui_binding şeklinde kullanınca K8s deploy işleminde kullanılan secret değerlerinde hata alındı. - veya . olarak yazılmalı.
    port: 5672
    protocol: rabbitmq
  - name: mui-binding
    port: 15672
```

Elbette PalindromeFinderService sınıfı ve Startup.cs'in de Redis ve RabbitMQ için yeniden revize edilmeleri gerekiyor.

PalindromeFinderService sınıfı

```csharp
using Einstein.Rabbit;
using Grpc.Core;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using SchoolOfRock;
using System;
using System.Threading.Tasks;

namespace Einstein
{
    public class PalindromeFinderService
        : PalindromeFinder.PalindromeFinderBase
    {
        private readonly ILogger<PalindromeFinderService> _logger;
        private readonly IDistributedCache _cache;
        private readonly PalindromeReply True = new() { IsPalindrome = true };
        private readonly PalindromeReply False = new() { IsPalindrome = false };
        private readonly IMessageQueueSender _mqSender;
        private readonly string _queueName;
        public PalindromeFinderService(ILogger<PalindromeFinderService> logger, IDistributedCache cache, IMessageQueueSender mqSender)
        {
            _logger = logger;
            _cache = cache; //Dağıtık cache servisi olarak Redis konumlanacak. Startup'ta onu ekledik çünkü.
            _mqSender = mqSender; // MQ nesnesini alıyoruz
            _queueName = Constants.GetRabbitMQQueueName(); //MQ adını alıyoruz.
        }

        public override async Task<PalindromeReply> IsItPalindrome(PalindromeRequest request, ServerCallContext context)
        {
            long r, sum = 0, t;
            var number = request.Number;

            var inCache = await _cache.GetStringAsync(request.Number.ToString()); // bu sayı Redis Cache'te var mı?
            if (inCache == "YES")
            {
                _logger.LogInformation($"{request.Number} palindrom bir sayıdır ve şu an Redis'ten getiriyorum. Hesap etmeye gerek yok");
                return True;
            }

            for (t = number; number != 0; number /= 10)
            {
                r = number % 10;
                sum = sum * 10 + r;
            }
            if (t == sum)
            {
                _logger.LogInformation($"{request.Number} palindrom bir sayı ama Redis cache'e atılmamış. Şimdi ekleyeceğim.");
                // Sayı adını Key olarak kullanıp Cache'e atıyoruz ve ona value olarak YES değerini atıyoruz.
                await _cache.SetStringAsync(request.Number.ToString(), "YES", new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
                });
                // Palindrome sayı ise onu Redis Cache'e atıyoruz.

                // Ayrıca RabbitMQ kuyruğuna da sayıyı atıyoruz.
                _mqSender.Send(_queueName, request.Number.ToString());
                return True;
            }
            else
                return False;
        }
    }
}
```

Einstein, Startup.cs'in son hali;

```csharp
using Einstein.Rabbit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Einstein
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            // RabbitMQ Desteği eklendi
            services.AddRabbitMQ();

            // Redis bildirimini yaptık. PalindromeFinderService, consturctor'dan alacak.
            services.AddStackExchangeRedisCache(o =>
            {
                o.Configuration = Configuration.GetConnectionString("redis") ?? "localhost:6379";
            });
        }

       // Diğer kısımlar
```

Kod tarafında RabbitMQ kullanımı için gerekli tipler, GoldenHammer isimli sınıfta yer alıyor. Bunu baştan yazmak biraz zahmetli ama yine de üşenmeyin yazın derim. Yazarken düşünecek ve neden böyle kullanılmış ki diyeceksiniz. Kitabın yönlendirmesi ile ben bu [adrese](https://github.com/PacktPublishing/Adopting-.NET-5--Architecture-Migration-Best-Practices-and-New-Features/tree/master/Chapter04/microservicesapp) gittim ama kendimde teknik borç riskini göze alarak bir [GodObject oluşturdum.](https://github.com/buraksenyurt/tye_sample_v2/tree/main/SchoolOfMath) Eğer sayfadan ayrılmadan kodu kullanmak isterseniz notların sonundaki Yardımcı Kodlar kısmından yararlanabilirsiniz. Bu noktada yine tye run ile ilerlemek önemli. Redis'in çalıştığından ve http://localhost:15672 adresine gittiğimizde RabbitMQ tarafının işler olduğundan emin olmakta fayda var.

![Nisan_screenshot_4.png](/assets/images/2021/Nisan_screenshot_4.png)

![Nisan_screenshot_5.png](/assets/images/2021/Nisan_screenshot_5.png)

![Nisan_screenshot_6.png](/assets/images/2021/Nisan_screenshot_6.png)

## Robert: AMQP İstemcisinin Eklenmesi

Robert isimli Worker tipinden olan son istemci uygulama, RabbitMQ'ya atılan palindrome sayıları içeren mesajları okumakla görevli. Basit bir RabbitMQ Consumer olduğunu söyleyebiliriz. Einstein isimli servis Palindrome sayı buldukça RabbitMQ'ya bunu mesaj olarak yollayacak şekilde ayarlanmıştı. Consumer üstünden bunları yakalamayı bekliyoruz. Aşağıdaki terminal komutları ile Worker servisini oluşturalım.

```bash
dotnet new worker -n Robert
dotnet sln add Robert
cd Robert
# RabbitMQ istemcisi olacağı için eklenecek paket
dotnet add package RabbitMQ.Client
# ve pek tabii Tye özelliklerini kullanabilmesi için de gerekli konfigurasyon paketi
dotnet add package --prerelease Microsoft.Tye.Extensions.Configuration
```

Bu Worker'ın kodlarını da aşağıdaki gibi geliştirebiliriz.

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Robert
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        // Servis çalışmaya başladığı zaman devreye giren metodu ezip kendi istediklerimizi yaptırıyoruz.
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                // RabbitMQ tarafı henüz ayağa kalkmamış olabilir diye burayı 1 dakika kadar duraksatalım
                await Task.Delay(TimeSpan.FromSeconds(60), cancellationToken);

                // Rabbit ile konuşmak için kullanılacak kanal nesnesi alınıyor
                var queue = CreateRabbitModel(cancellationToken);

                // queue tanımlanır
                queue.QueueDeclare(
                    queue: "palindromes",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                    );

                // Tanımlanan kuyruğu dinleyecek nesne örneklenir
                var consumer = new EventingBasicConsumer(queue);

                // dinlenen kuyruğa mesaj geldikçe tetiklenen olay metodu
                consumer.Received += (model, arg) =>
                {
                    var number = Encoding.UTF8.GetString(arg.Body.Span); // mesaj yakalanır
                    _logger.LogInformation($"Yeni bir palindrom sayısı bulunmuş: {number}");
                };

                queue.BasicConsume(
                    queue: "palindromes",
                    autoAck: true,
                    consumer: consumer);
            }
            catch (Exception exc)
            {
                _logger.LogError($"Bir hata oluştu {exc.Message}");
                throw;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(10000, stoppingToken);
            }
        }

        private IModel CreateRabbitModel(CancellationToken cancellationToken)
        {
            try
            {
                // Önce bağlantı oluşturmak için factory nesnesi örneklenir
                var factory = new ConnectionFactory()
                {
                    HostName = Rabbit.Constants.GetRabbitMQHostName(), // Rabbit Host adresi alınır (Environment'ten gelir)
                    Port = Convert.ToInt32(Rabbit.Constants.GetRabbitMQPort()), // Port bilgisi
                    UserName=Rabbit.Constants.GetRabbitMQUser(), // Kullanıcı adı
                    Password=Rabbit.Constants.GetRabbitMQPassword() // ve Şifre
                };

                var connection = factory.CreateConnection(); // Bağlantı nesnesi oluşturulur. Exception yoksa bağlanmış demektir.
                _logger.LogInformation("RabbitMQ ile bağlantı sağlandı");
                return connection.CreateModel(); //Queue işlemleri için kullanılacak model nesnesi döndürülür
            }
            catch (Exception exc) 
            {
                _logger.LogError($"Rabbit tarafına bağlanmaya çalışırken bir hata oluştu. {exc.Message}");
                throw;
            }
        }
    }
}
```

Robert'ın kodları tamamlandıktan sonra tye run ile sistemi çalıştırıp dashboard üzerinden ulaşabileceğimiz logları kontrol etmekte yarar var. Bakalım Robert'ın loglarında RabbitMQ daki palindromes isimli kuyruğa düşen mesajlar var mı?

![Nisan_screenshot_7.png](/assets/images/2021/Nisan_screenshot_7.png)

## Sadece Belli Uygulamaları Çalıştırmak

İlerlemeden önce tye ile sadece belli uygulamaları nasıl çalıştıracağımıza da bir bakalım isterim. tye.yaml dosyasında tag bildirimlerini kullanarak tye run sonrası sadece belli servislerin ayağa kaldırılması sağlanabilir. Bu yaklaşım, Debug işlemleri için idealdir. N tane servisin olduğu bir senaryoda her şeyi ayağa kaldırmak yerine sadece istenenleri kurcalama noktasında çok faydalıdır. Söz gelimi yaml dosyamızda sadece middleware tag'ine sahip servisleri çalıştırmak istediğimizi düşünelim. run komutunu aşağıdaki gibi kullanabiliriz.

```bash
tye run --tags middleware #sadece middleware tag'ine sahip servisleri çalıştırır.
```

![Nisan_screenshot_8.png](/assets/images/2021/Nisan_screenshot_8.png)

Birden fazla namespace'te bir arada ayağa kaldırılabilir. Mesela aşağıdaki kullanım ile backend ve middleware tag'ine sahip servisler ayağa kaldırılacaktır. Şimdi yaml içerisindeki tag elementlerinin ne işe yaradığınız daha iyi anlamış olmalısınız.

```bash
tye run --tags backend middleware
```

### Debug Etmek ve Breakpoint Noktalarına Geçmek

Kod debug etmek adettendir:D Lakin tye ile çalışırken ayağa kaldırılan aktörleri debug etmek için biraz meşakkatli bir yol izlemek gerekiyor. İlk olarak gerekli yerlere breakpoint konulur. Örneğin;

![screenshot_9.png](/assets/images/2021/screenshot_9.png)

Sonrasında aşağıdaki komut ile çözüm çalıştırlır.

```bash
tye run --debug
```

Debug edilmek istenen uygulamanının terminal loglarına düşen process id değeri bulunur.

![screenshot_10.png](/assets/images/2021/screenshot_10.png)

Visual Studio -> Debug -> Attach to Process adımları kullanılarak ilgili process çalışma zamanına alınır.

![screenshot_11.png](/assets/images/2021/screenshot_11.png)

Çayımızdan/kahvemizden bir yudum alınır ve Breakpoint noktasına gelinmesi beklenir.

![screenshot_12.png](/assets/images/2021/screenshot_12.png)

Hepsi bu kadar;) Ya da doğru düzgün tasarladığımız hata yönetim mekanizmasının ürettiği sistem loglarına gidilir ve sorunun ne olduğu anlaşılmaya çalışılır.

## Kubernetes Deploy İşlemleri

Artık notlarımızın sonuna doğru geliyoruz. Bal yapmayan arı olmamak için bu örneği de Kubernetes tarafına almamız lazım. Windows 10 üstündeki Docker Desktop'ın K8s Enabled özelliğinin açık olduğundan emin olalım. Buna göre sistemde tye.yaml tarafındaki servislerin alınabileceği bir Kubernetes Cluster mevcut kabul edilir. İkinci olarak bir container registry'ye ihtiyaç vardır ki ilk Hello World örneğimizde bunu localhost:5000 adresinde konuşlandırmıştık. Güncel örnek iki harici servis kullanmakta; Redis ve RabbitMQ. Bunları şu an için Kubernetes ortamına el yordamıyla kendi manifesto dosyaları üzerinden deploy etmemiz gerekiyor ama bu durum tye'ın ilerleyen sürümlerinde daha da kolaylaşabilir. Hello World örneğinde kullandığımız redis.yaml'ı burada da kullanabiliriz. RabbitMQ tarafı içinse aşağıdaki manifesto içeriği işimizi görecektir.

RabbitMQ.yaml

```yml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: rabbitmq
  labels:
    app.kubernetes.io/name: rabbitmq
    app.kubernetes.io/part-of: schoolofmath
spec:
  selector:
    matchLabels:
      app.kubernetes.io/name: rabbitmq
  replicas: 1
  template:
    metadata:
      labels:
        app.kubernetes.io/name: rabbitmq
        app.kubernetes.io/part-of: schoolofmath
    spec:
      containers:
        - name: rabbitmq
          image: rabbitmq:3-management
          resources:
            requests:
              cpu: 100m
              memory: 100Mi
          ports:
            - containerPort: 5672
            - containerPort: 15672

---
apiVersion: v1
kind: Service
metadata:
  name: rabbitmq
  labels:
    app.kubernetes.io/name: rabbitmq
    app.kubernetes.io/part-of: schoolofmath
spec:
  ports:
    - port: 5672
      protocol: TCP
      targetPort: 5672
  selector:
    app.kubernetes.io/name: rabbitmq
---
apiVersion: v1
kind: Service
metadata:
  name: rabbitmq-mui
  labels:
    app.kubernetes.io/name: rabbitmq
    app.kubernetes.io/part-of: schoolofmath
spec:
  type: NodePort
  ports:
    - port: 15672
      protocol: TCP
      targetPort: 15672
      nodePort: 30072
  selector:
    app.kubernetes.io/name: rabbitmq
```

Hem RabbitMQ hem de onu daha kolay okumamızı sağlayacak görsel MUI arabirimi için iki ayrı deployment tanımı söz konusudur. Bu dosyalardan yararlanarak ilgili servisleri Kubernetes ortamına aşağıdaki terminal komutları ile alabiliriz.

```bash
kubectl apply -f .\rabbitmq.yaml
kubectl apply -f .\redis.yaml
```

![screenshot_13.png](/assets/images/2021/screenshot_13.png)

Kubernetes deployment adımını da aşağıdaki komutla başlatabiliriz.

```bash
tye deploy --interactive
```

Büyük ihtimalle redis ve rabbitmq için adres sorulacaktır. Redis için redis:6379, rabbitmq içinse rabbitmq:5672 (Mui sebebiyle iki kez sorulabilir ki bana öyle oldu) adresleri kullanılabilir. Sonuç olarak Docker Desktop'a baktığımızda dağıtımların yapıldığını görmeliyiz.

![screenshot_14.png](/assets/images/2021/screenshot_14.png)

Yukarıdaki ekran görüntüsünde dikkat edileceği üzere servislerimiz localhost:5000 ön adresi üzerine konumlanmış duruyorlar. Bunun sebebi container registry olarak bu adresi bildirmiş olmamız (yaml dosyasındaki ilgili kısmı hatırlayın)

Tekrar belirtmekte fayda var ki kendi uygulamalarımız dağıtım işlemi sırasında yine otomatik olarak dockerize edilmişlerdir. Robert isimli Worker servise ait tye çalışma zamanının yaptıklarını aşağıdaki ekran görüntüsünde görebilirsiniz (Normalde bunlar için bir Dockerfile hazırlamamız gerekirdi diye düşünüyorum)

![screenshot_16.png](/assets/images/2021/screenshot_16.png)

Oluşan diğer imajları Docker Desktop üzerinde görebiliriz.

![screenshot_17.png](/assets/images/2021/screenshot_17.png)

Şu anda RabbitMQ tarafı da aktif haldedir ve eğer localhost:30072 adresine gidersek o ana kadar ki mesaj trafiğini izleyebiliriz.

![screenshot_15.png](/assets/images/2021/screenshot_15.png)

Yapılan Deployment işlemini geri almak ve Kubernetes dağıtımlarını kaldırmak içinse tye undeploy terminal komutu kullanılır.

![screenshot_18.png](/assets/images/2021/screenshot_18.png)

Bu çalışma deneysel bir projeyi hem basılı hem de çevrimiçi bir kaynaktan yazarak anlamam noktasında bana önemli değerler katmış durumda. Ancak işi burada bırakmamak lazım. Tye projesinin bir geleceği olacaksa diğer örnek kullanımları incelemekte de yarar var. Söz gelimi bir loglama senaryosunu işin içerisine katmak, performans izleme aktörünü dahil etmek gibi konular üstünde de denemeler yapmak yararlı olabilir. Dahası açık kaynak kod reposuna gidip tye run dediğimizde arka planda neler nasıl çalışıyoru anlamaya çalışmak çok daha yararlı olabilir. Bir teknoloji tüketicisi olarak en azından nasıl kullanılır ve ne işe yararı bir nebze olsun anladığımı ve siz değerli okurlarıma aktarabildiğimi düşünüyorum. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

Kaynaklar

[Adopting.NET 5.](https://www.packtpub.com/product/adopting-net-5/9781800560567) By Hammad Arif, Habib Qureshi

[Introducing Project Tye](https://devblogs.microsoft.com/aspnet/introducing-project-tye/) Amiee Lo, Program Manager, Microsoft ASP.NET

[Project Tye Github](https://github.com/dotnet/tye)

[Project Tye: Creating Microservices in a.NET Way](https://www.codemag.com/Article/2010052/Project-Tye-Creating-Microservices-in-a-.NET-Way) Shayne Boyer, CODE Focus Magazine: 2020 - Vol. 17 - Issue 1 -.Net 5.0

[Project Tye: Building Developer Focused Tooling for Kubernetes and.NET](https://youtu.be/prbYvVVAcRs) - David Fowler

Yardımcı Kodlar

Notların dışına çıkmadan GoldenHammer ve Constant sınıflarını almak isterseniz aşağıdaki kod parçalarından yararlanabilirsiniz.

Robert projesindeki Constants.cs sınıfı

```csharp
using System;

namespace Robert.Rabbit
{
    /// Kaynak: https://github.com/PacktPublishing/Adopting-.NET-5--Architecture-Migration-Best-Practices-and-New-Features/tree/master/Chapter04/microservicesapp
    public static class Constants
    {
        public const string RABBIT_HOST = "SERVICE__RABBITMQ__MQ_BINDING__HOST";
        public const string RABBIT_PORT = "SERVICE__RABBITMQ__MQ_BINDING__PORT";
        public const string RABBIT_ALT_HOST = "SERVICE__RABBITMQ__HOST";
        public const string RABBIT_ALT_PORT = "SERVICE__RABBITMQ__PORT";
        public const string RABBIT_ALT2_PORT = "RABBITMQ_SERVICE_PORT";
        public const string RABBIT_USER = "RABBIT_USER";
        public const string RABBIT_PSWD = "RABBIT_PSWD";
        public const string RABBIT_QUEUE = "RABBIT_QUEUE";

        public static string GetRabbitMQHostName()
        {
            var v = Environment.GetEnvironmentVariable(RABBIT_HOST);
            if (string.IsNullOrWhiteSpace(v))
            {
                v = Environment.GetEnvironmentVariable(RABBIT_ALT_HOST);
                if (string.IsNullOrWhiteSpace(v))
                    return "rabbitmq";
                else return v;
            }
            else return v;
        }

        public static string GetRabbitMQPort()
        {
            var v = Environment.GetEnvironmentVariable(RABBIT_PORT);
            if (string.IsNullOrWhiteSpace(v))
            {
                v = Environment.GetEnvironmentVariable(RABBIT_ALT_PORT);
                if (string.IsNullOrWhiteSpace(v) || v == "-1")
                    return Environment.GetEnvironmentVariable(RABBIT_ALT2_PORT);
                else return v;
            }
            else return v;
        }

        public static string GetRabbitMQUser()
        {
            var v = Environment.GetEnvironmentVariable(RABBIT_USER);
            if (string.IsNullOrWhiteSpace(v))
                return "guest";
            else return v;
        }

        public static string GetRabbitMQPassword()
        {
            var v = Environment.GetEnvironmentVariable(RABBIT_PSWD);
            if (string.IsNullOrWhiteSpace(v))
                return "guest";
            else return v;
        }

        public static string GetRabbitMQQueueName()
        {
            var v = Environment.GetEnvironmentVariable(RABBIT_QUEUE);
            if (string.IsNullOrWhiteSpace(v))
                return "primes";
            else return v;
        }
    }
}
```

Einstein tarafındaki GoldenHammer sınıfı

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Text;

namespace Einstein.Rabbit
{
    // Kaynak: https://github.com/PacktPublishing/Adopting-.NET-5--Architecture-Migration-Best-Practices-and-New-Features/tree/master/Chapter04/microservicesapp
    public interface IMQClient
    {
        IModel CreateChannel();
    }

    public interface IMessageQueueSender
    {
        public void Send(string queueName, string message);
    }

    public static class Constants
    {
        public const string RABBIT_HOST = "SERVICE__RABBITMQ__MQ_BINDING__HOST";
        public const string RABBIT_PORT = "SERVICE__RABBITMQ__MQ_BINDING__PORT";
        public const string RABBIT_ALT_HOST = "SERVICE__RABBITMQ__HOST";
        public const string RABBIT_ALT_PORT = "SERVICE__RABBITMQ__PORT";
        public const string RABBIT_ALT2_PORT = "RABBITMQ_SERVICE_PORT";
        public const string RABBIT_USER = "RABBIT_USER";
        public const string RABBIT_PSWD = "RABBIT_PSWD";
        public const string RABBIT_QUEUE = "RABBIT_QUEUE";

        public static string GetRabbitMQHostName()
        {
            var v = Environment.GetEnvironmentVariable(RABBIT_HOST);
            if (string.IsNullOrWhiteSpace(v))
            {
                v = Environment.GetEnvironmentVariable(RABBIT_ALT_HOST);
                if (string.IsNullOrWhiteSpace(v))
                    return "rabbitmq";
                else return v;
            }
            else return v;
        }

        public static string GetRabbitMQPort()
        {
            var v = Environment.GetEnvironmentVariable(RABBIT_PORT);
            if (string.IsNullOrWhiteSpace(v))
            {
                v = Environment.GetEnvironmentVariable(RABBIT_ALT_PORT);
                if (string.IsNullOrWhiteSpace(v) || v == "-1")
                    return Environment.GetEnvironmentVariable(RABBIT_ALT2_PORT);
                else return v;
            }
            else return v;
        }

        public static string GetRabbitMQUser()
        {
            var v = Environment.GetEnvironmentVariable(RABBIT_USER);
            if (string.IsNullOrWhiteSpace(v))
                return "guest"; 
            else return v;
        }

        public static string GetRabbitMQPassword()
        {
            var v = Environment.GetEnvironmentVariable(RABBIT_PSWD);
            if (string.IsNullOrWhiteSpace(v))
                return "guest";
            else return v;
        }

        public static string GetRabbitMQQueueName()
        {
            var v = Environment.GetEnvironmentVariable(RABBIT_QUEUE);
            if (string.IsNullOrWhiteSpace(v))
                return "palindromes"; // Consumer'ın dinyeceği varsayılan kuyruk adı. Normalde RABBIT_QUEUE ile çevre değişken üzerinden gelmezse bu kullanılır.
            else return v;
        }
    }

    public class RabbitMQClient : IMQClient
    {
        public string hostname { get; }
        public string port { get; }
        public string userid { get; }
        public string password { get; }

        private readonly ILogger _logger;
        private readonly IConnection _connection;
        private IModel _channel;

        public RabbitMQClient(ILogger<RabbitMQClient> logger, IConfiguration configuration)
        {
            _logger = logger;

            hostname = Constants.GetRabbitMQHostName();
            port = Constants.GetRabbitMQPort();
            userid = Constants.GetRabbitMQUser();
            password = Constants.GetRabbitMQPassword();

            try
            {
                logger.LogInformation($"RabbitMQ Bağlantısı oluşturuluyor. @ {hostname}:{port}:{userid}:{password}");
                var factory = new ConnectionFactory()
                {
                    HostName = hostname,
                    Port = int.Parse(port),
                    UserName = userid,
                    Password = password,
                };

                _connection = factory.CreateConnection();
            }
            catch (Exception ex)
            {
                logger.LogError(-1, ex, "RabbitMQ Bağlantısı oluşturulması sırasında hata oluştu.");
                throw;
            }
        }

        public IModel CreateChannel()
        {
            if (_connection == null)
            {
                _logger.LogError("RabbiMQ Kanal bağlantısı oluşturulması sırasında hata oluştu.");
                throw new Exception("RabbitMQClient bağlantı hatası.");
            }
            _channel = _connection.CreateModel();
            return _channel;
        }
    }

    public class RabbitMQueueSender : IMessageQueueSender
    {
        private readonly ILogger<RabbitMQueueSender> _logger;
        private readonly IMQClient _mqClient;

        private IModel _mqChannel;
        private string _queueName;

        private IModel MQChannel
        {
            get
            {
                if (_mqChannel == null || _mqChannel.IsClosed)
                    _mqChannel = _mqClient.CreateChannel();
                return _mqChannel;
            }
        }

        public RabbitMQueueSender(ILogger<RabbitMQueueSender> logger, IMQClient mqClient)
        {
            _logger = logger;
            _mqClient = mqClient;
        }

        public void Send(string queueName, string message)
        {
            if (string.IsNullOrWhiteSpace(queueName)) return;

            if (string.IsNullOrWhiteSpace(_queueName)) 
            {
                _logger.LogInformation($"{queueName} isimli kuyruk ilk kez oluşturuluyor.");
                MQChannel.QueueDeclare(queue: queueName,
                                            durable: false,
                                            exclusive: false,
                                            autoDelete: false,
                                            arguments: null);
                _queueName = queueName;
            }

            _logger.LogInformation($"Mesaj kuyruğunu gönderiliyor. Queue Name:{queueName}");

            var body = Encoding.UTF8.GetBytes(message);

            try
            {
                MQChannel.BasicPublish(exchange: "",
                                            routingKey: queueName,
                                            basicProperties: null,
                                            body: body);
            }
            catch (System.Exception ex)
            {
                ex.ToString();
            }
            _logger.LogInformation("Mesaj başarılı bir şekilde kuyruğa aktarıldı.");
        }
    }

    public static class RabbitMQServiceCollectionExtensions
    {
        // Startup.cs'de RabbitMQ'yu servis listesine eklememizi sağlayan genişletme fonksiyonu
        public static IServiceCollection AddRabbitMQ(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.Add(ServiceDescriptor.Singleton<IMQClient, RabbitMQClient>());
            services.Add(ServiceDescriptor.Singleton<IMessageQueueSender, RabbitMQueueSender>());

            return services;
        }
    }
}
```
