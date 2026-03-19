---
title: "Ocelot - .Net Core Tarafında Bir API Gateway Denemesi"
pubDate: 2020-12-16 11:51:00
categories:
  - .Net Core
tags:
  - .Net Core
  - Bash
  - C#
  - JSON
  - .NET
  - LINQ
  - XML
  - Web API
  - HTTP
  - Authentication
  - Authorization
  - Docker
  - Rust
  - Java
  - Node.js
  - Async/Await
  - Performance
  - Caching
  - Generics
  - GitHub
---

# Ocelot - .Net Core Tarafında Bir API Gateway Denemesi
![image.axd](images/image.axd)

Uzun süre önce bankada çalışırken nereye baksam servis görüyordum. Bir süre sonra ana bankacılık uygulaması dahil pek çok ürünün kullandığı bu sayısız servisler ağının yönetimi zorlaşmaya başladı. Bir takım ortak işlerin daha kolay ve etkili yönetilmesi gerekiyordu. Müşterek bir kullanıcı doğrulama ve yetkilendirme kontrolü (authentication & authorization), yük dengesi dağıtımı (load balancing), birkaç servis talebinin birleştirilmesi ve hatta birkaç servis verisinin birleştirilerek döndürülmesi (aggregation), servis verisinin örneğin XML'den JSON gibi farklı formata evrilmesi, servis geliş gidişlerinin loglanması, yönlendirmeler yapılması (routing), performans için önbellek kullanılması (caching), servis hareketliliklerini izlenmesi (tracing), servislerin kolayca keşfedilmesi (discovery), çağrı sayılarına sınırlandırma getirilmesi, bir takım güvenlik politikalarının entegre edilmesi, özelleştirilmiş delegeler yazılması (custom handler/middleware), tüm uygulamalar için ortak bir servis geçiş kanalının konuşlandırılması ve benzerleri. Yazarken yoruldum, daha ne olsun:D Sonunda Java tabanlı WSO2 isimli bir API Gateway kullanılmasına karar verildi.

Geçtiğimiz günlerde de yine konuşma sırasında Ocelot isimli C# ile yazılmış açık kaynak bir ürünün adı geçti ve tabii ki bende bir merak uyandı. Kanımca hafif sıklet mikroservis ortamlarında veya servis odaklı mimari çözümlerinde düşünülebilir. Ama önce denemek ve nasıl işlediğini görmek gerekiyor, öyle değil mi?;) Bu arada Ocelot'un oldukça doyurucu bir [dokümantasyonu](https://ocelot.readthedocs.io/en/latest/index.html) olduğunu da belirteyim. Haydi gelin SkyNet derlememize başlayalım.

## Senaryo

Örnekte şöyle bir senaryoyu icra etmeye çalışacağız; Oyuncu detaylarını getiren, ona öneri oyunları ürün olarak sunan, kazandığı bir promosyonu sisteme kaydetmesini sağlayan üç kobay servis tasarlayacağız. İstemci uygulama (Postman bile yeterli olur) bu birkaç servis çağrısı için API Gateway'e gelecek. Yani istemciler bu servisler için aslında tek bir noktaya gelip API Gateway üzerinden konuşacaklar. İlk etapta ocelot paketini kullanan gateway uygulaması basit bir router olacak. Hatta iki servis çıktısını birleştirip döndüren bir aggregation fonksiyonelliği de katacağız. Sonrasında Load Balancing işlevselliğini entegre edeceğiz.

## Hazırlıklar ve Kodlama

Çok doğal olarak birkaç kobay servise ihtiyacımız var. Tamamını.net core web api olarak tasarlamak doğrusu işime geldi:) Ancak gerçek hayat senaryolarında farklı programlama dilleri ve çatıları ile geliştirilmiş servisler kullanmak daha mantıklı olacaktır.

```bash
mkdir services
cd services
# İlk olarak kobay servislerimizi ekleyelim
# Fonksiyon başına bir servis gibi oldu ama
# amacımız bilindiği üzere Ocelot'un kurgusunu anlamak

# Oyuncu bilgilerini getireceğimiz bir servis
dotnet new webapi -o GamerService

# Oyuncuya önerilecek promosyonların çekileceği bir servis
dotnet new webapi -o PromotionService

# Oyuncunun daha önce satın almış olduğu ürünleri getirecek bir servis
dotnet new webapi -o ProductService

# ve Ocelot Servis Uygulamasının oluşturulup gerekli Nuget paketinin eklenmesi
cd ..
dotnet new web -o Bosphorus
dotnet add package ocelot
# Bu uygulamada kritik olan nokta ocelot konfigurasyonunun durduğu json dosya içerikleri
cd Bosphorus
touch ocelot.json
```

Servislerimiz kobay niteliği taşıdıklarından birşeyler döndürseler yeterli. Yine de sayfanın dışına çıkmadan devam edebilmeniz için aşağıya gerekli kod parçalarını bırakıyorum ([İsteyenler SkyNet github reposuna uğrayıp indirebilirler de](https://github.com/buraksenyurt/skynet/tree/master/No%2037%20-%20Ocelot))

GamerService içindeki PlayerController.cs

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GamerService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly ILogger<PlayerController> _logger;

        public PlayerController(ILogger<PlayerController> logger)
        {
            _logger = logger;
        }

        /*
            HTTP Get taleplerine karşılık verecek metodumuzdan geriye sembolik olarak bir Player nesnesi döndürüyoruz.
            Player/19 gibi gelen taleplere cevap verecek
        */
        [HttpGet("{id}")]
        public Player Get(string id)
        {
            return new Player
            {
                Id = id,
                Fullname = "Megen Enever",
                Level = 58,
                Location = "Dublin"
            };
        }
    }

    public class Player
    {
        public string Id { get; set; }
        public string Fullname { get; set; }
        public int Level { get; set; }
        public string Location { get; set; }
    }
}
```

ve aynı servisi farklı port ile ayağa kaldıracağımızdan Program sınıfındaki UseUrls kullanımı.

```csharp
public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // Farklı bir porttan yayın yapsın
                    webBuilder.UseStartup<Startup>().UseUrls("http://localhost:6501");
                });
```

ProductService içindeki ProductController sınıfı (7501 Numaralı porttan kaldıracak şekilde Program sınıfını değiştirmeyi unutmayın)

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ProductService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;

        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
        }

        /*
            Oyuncu için önerilecek oyunları döndüren bir operasyonmuş gibi hayal edelim.
            api/product/suggestions/1234 gibi HTTP Get taleplerine cevap verecek.
        */
        [HttpGet("suggestions/{id}")]
        public IEnumerable<Product> Get(string id)
        {
            var products = new List<Product>{
                new Product{Id=1,Title="Commandos III",Price=34.50},
                new Product{Id=2,Title="Table Child",Price=23.67},
                new Product{Id=3,Title="League of Heros 2022",Price=145.99},
            };

            return products;
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }
    }
}
```

PromotionService içerisindeki ApplierController sınıfı (Bunu da 8501 nolu porttan ayağa kaldırmayı ihmal etmeyin)

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PromotionService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApplierController : ControllerBase
    {
        private readonly ILogger<ApplierController> _logger;

        public ApplierController(ILogger<ApplierController> logger)
        {
            _logger = logger;
        }

        /*
            Birde HTTP Post deneyelim bari.
            Sembolik olarak promosyon uygulayan bir metot olduğunu varsayalım.
        */
        [HttpPost]
        public IActionResult SetPromotoion(Code promoCode)
        {
            return Ok($"{promoCode.No} için {promoCode.Duration} gün süreli promosyon kullanıcı hesabına tanımlanmıştır");
        }
    }

    public class Code
    {
        public string No { get; set; }
        public int Duration { get; set; }
        public int PlayerId { get; set; }
        public int GameId { get; set; }
    }
}
```

İlk kobay servislerimiz hazır. Şimdi yapmamız gereken Ocelot paketini kullanan uygulamamızı geliştirmek. Basit bir Console olarak geliştirebiliriz. Program sınıfının kodunu aşağıdaki gibi yazarak devam edelim.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
// Ocelot için gerekli bildirimler
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Net.Http;
using System.Threading;

namespace Bosphorus
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureServices(services =>
            {
                services
                    .AddOcelot() // Ocelot'u bildirdik
                    .AddDelegatingHandler<RequestInspector>(); // HttpClient isteklerinde araya girecek delegeyi bildirdik
            }).ConfigureAppConfiguration((host, config) =>
            {
                config.AddJsonFile("ocelot.json"); // Ocelot ayarlarının alınacağı konfigurasyon dosyasını belirttik
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>().Configure(async app => await app.UseOcelot());
            });
    }

    /*
        Aşağıdaki sınıfın yardımıyla Ocelot'taki belirttiğimiz bir Route'a gelen HTTP istekleri işlenmeden önce araya girebiliriz.
        Request içeriğine bakıp akışı değiştirebiliriz.
        Bu temsilci sınıfını kullanacağımızı yukarıdaki AddDelegatingHandler metodunda belirttik.
        Ayrıca ocelot.json içerisinde, örnek olması açısından /eagames/player/{id} adresine gelen taleplerde araya gireceğimizi belirttik.
    */
    public class RequestInspector : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"\nDevam etmeden önce şu gelen Request içeriğini bir inceyelim\n{request.ToString()}\n");
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
```

## İlk Deneme (Aggregation ve Standart Routing)

Öncelikle kobay servislerin ayağa kaldırılması lazım. GamerService, ProductService ve PromotionService isimli servisleri kendi klasörlerinde dotnet run ile çalıştırabiliriz. Kobay servisler aşağıdaki adreslerden devreye girecektir.

```text
GamerService -> http://localhost:6501 
ProductService -> http://localhost:7501 
PromotoionService -> http://localhost:8501
```

Ocelot için çalışma zamanı ayarları bildiğiniz üzere json türünden konfigurasyon dosyasında tutulmaktadır. İlk versiyonunu aşağıdaki gibi yazıp ilerleyelim.

```json
{
  "Routes": [
    {
      "UpstreamPathTemplate": "/eagames/player/{id}",
      "UpstreamHttpMethod": [
        "Get"
      ],
      "DownstreamPathTemplate": "/player/{id}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "localhost",
          "Port": 6501
        }
      ],
      "Key": "Player"
    },
    {
      "UpstreamPathTemplate": "/eagames/product/{id}",
      "UpstreamHttpMethod": [
        "Get"
      ],
      "DownstreamPathTemplate": "/api/product/suggestions/{id}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "localhost",
          "Port": 7501
        }
      ],
      "Key": "Product"
    },
    {
      "UpstreamPathTemplate": "/eagames/applypromo",
      "UpstreamHttpMethod": [
        "Post"
      ],
      "DownstreamPathTemplate": "/applier",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "localhost",
          "Port": 8501
        }
      ]
    }
  ],
  "Aggregates": [
    {
      "RouteKeys": [
        "Player",
        "Product"
      ],
      "UpstreamPathTemplate": "/{id}"
    }
  ]
}
```

Artık Bosphorus uygulamasını çalıştırıp localhost:5000/19 şeklinde bir talep gönderebiliriz. İlk örnek Aggregation durumunu taklit etmekte ve promosyon ekleme için yönlendirme yapmaktadır. Ayrıca GamerService ve ProductService'e ortak çağrı yapıp arka planda çağırılan servis çıktılarını tek bir JSON paketinde birleştirip geriye döndürür;)

![image.axd](images/image.axd)

İlk örnekteki UpstreamPathTemplate tanımlarına göre http://localhost:5000/eagames/player/23 adresine yapılan çağrı esasında http://localhost:6501/player/23 adresine yönlendirilir.

![image.axd](images/image.axd)

Benzer şekilde http://localhost:5000/eagames/product/23 şeklinde yapılacak çağrıda http://localhost:7501/api/product/suggestions/23 adresine yönlendirilir.

![image.axd](images/image.axd)

PromotionService içerisinde bir de POST metodumuz var. Ocelot.JSON için yaptığımız tanıma göre http://localhost:5000/eagames/applypromo adresine gelen talebi, http://localhost:8501/applier adresine yönlendiriyor olmalı. İşte örnek POST içeriği ve sonuç...

```json
{
	"No":"PROMO-12345",
	"Duration":30,
	"GameId":102935,
	"PlayerId":1
}
```

![image.axd](images/image.axd)

## İkinci Deneme (Load Balancer)

Bu kez Dockerize edilmiş bir Web API hizmetinden üç tanesini farklı portlarda ayağa kaldırıp Ocelot'un gelen talepleri bu adreslere dağıtmasını sağlamayı deneyelim. Temel amacımız ocelot konfigurasyonunda bunun nasıl ele alınacağını öğrenmek.

```bash
# Yine Services klasöründe RewardService isimli bir .Net Core Web API var
dotnet new webapi -o RewardService

cd RewardSercice

# Dockerize edeceğimiz
touch Dockerfile

# bin ve obj klasörlerini dışarıda bırakmak için
touch .dockerignore

# Dockerize için
docker build -t rewards .

# Dockerize ettiğimiz servisi çalıştırırken de aşağıdaki komutu kullanabiliriz
# Aynı servisin 3 farklı porttan çalışacak birer örneğini ayağa kaldırıyoruz
docker run -d -p 5555:80 -p 5556:80 -p 5557:80 rewards
```

Servisin RewardController sınıfını ve Dockerfile içeriklerini aşağıdaki gibi yazabiliriz.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace RewardService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalculatorController : ControllerBase
    {
        private static readonly string[] topics = new[]
        {
            "1000 Free Spell"
            , "10 Free Coin"
            , "30 Days Free Trail"
            , "Gold Ticket"
            ,"Legendary Tournemant Pass"
            ,"1000 Free Retro Game"
            ,"One Day All Games Free"
        };

        private readonly ILogger<CalculatorController> _logger;

        public CalculatorController(ILogger<CalculatorController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Reward> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 3).Select(index => new Reward
            {
                Duration = rng.Next(7, 60),
                Description = topics[rng.Next(topics.Length)]
            })
            .ToArray();
        }
    }

    public class Reward{
        public int Duration { get; set; }
        public string Description { get; set; }
    }
}
```

ve Dockerfile;

```bash
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "RewardService.dll"]
```

Bu sefer http://localhost:5555/Calculator, http://localhost:5556/Calculator ve http://localhost:5557/Calculator adreslerinden talep alan bir Web API servisimiz var. Load Balancer ayarlarını ocelot.json'a aşağıdaki gibi ekleyelim ve denemelerimize geçelim.

```json
{
  "DownstreamPathTemplate": "/calculator",
  "DownstreamScheme": "http",
  "DownstreamHostAndPorts": [
    {
      "Host": "localhost",
      "Port": 5555
    },
    {
      "Host": "localhost",
      "Port": 5556
    },
    {
      "Host": "localhost",
      "Port": 5557
    }
  ],
  "UpstreamPathTemplate": "/eagames/rewards",
  "LoadBalancerOptions": {
    "Type": "LeastConnection"
  },
  "UpstreamHttpMethod": [
    "Get"
  ]
}
```

Artık http://localhost:5000/eagames/rewards adresine geldiğimizde

![image.axd](images/image.axd)

Talepler LeastConnection seçimi nedeniyle her seferinde bir sonraki backend servisine yönlendirilecektir.

![image.axd](images/image.axd)

Diğer yandan hatırlayacağınız gibi gelen talepler sırasında araya girebileceğimizden bahsetmiştik. Bu sayede Ocelot'a gelen bir Http isteğine cevap dönmeden önce bir takım iş kurallarını işletmek mümkün olabilir.

![image.axd](images/image.axd)

Gelelim bu SkyNet derlemesinin bomba sorularına:)

- Gateway arkasında XML içerik döndüren bir servis metodu olduğunu düşünelim. Gateway'e bu servis için gelen çağrı karşılığında XML yerine JSON döndürmemiz mümkün olur mu? Bunu Ocelot üzerinde nasıl tanımlarız?
- Dockerize ettiğimiz servisi üç farklı porttan ayağa kaldırdığımız bir container başlattık. Ocelot'un Load Balancer ayarları gereği eagames/rewards'a gelen talepler arkadaki portlara seçilen stratejiye göre dağıtılıyor. Üç portta esas itibariyle aynı container'a (80 portuna) iniyor. Sizce gerçek anlamda bir Load Balancing oldu mu? Arkadaşlarınızla tartışınız.
- Load Balancer senaryolarında Sticky Session dikkat edilmesi gereken bir konudur. Ocelot'ta Sticky Session desteği var mıdır araştırınız?

Soruları düşünerkene örneği geliştirmeye de devam edebilirsiniz. Mesela en az iki servisi daha farklı programlama dilleri ile senaryoya dahil edebilir (NodeJs, Java, Rust, GO) ya da RewardService'in geriye döndürdüğü bedava ödüller listesindeki tekrar eden bilgileri tekleştirmek için gerekli kod düzenlemesini yapabilirsiniz. Bunlara ek olarak ürünü şirketinizde bir POC (Proof of Concept) çalışması olarak değerlendirip yük testi altında nasıl davranış sergileyeceğini araştırabilirsiniz.

Böylece geldik bir [SkyNet](https://github.com/buraksenyurt/skynet) çalışmamızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
