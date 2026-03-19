---
title: "Bir Web Uygulamasında Gantt Chart Kullanımı"
pubDate: 2019-07-26 13:00:00
categories:
  - Asp.Net Core
tags:
  - Asp.Net Core
  - Bash
  - JSON
  - C#
  - .NET
  - ASP.NET
  - Entity Framework
  - EF Core
  - LINQ
  - REST
  - Web API
  - HTTP
  - JWT
  - JavaScript
  - Generics
  - Microservices
  - Visual Studio
  - GitHub
---

# Bir Web Uygulamasında Gantt Chart Kullanımı
![image.axd](images/image.axd)

Beğenerek dinlediğim Scorpions grubunun en güzel şarkılarından birisidir Wind of Change. Değişim rüzgarları uzun zamandır hayatımın bir parçası aslında. Sanıyorum ilk olarak 2012 yılında o zamanlar çalışmakta olduğum turuncu bankada başlamıştı esintiler. Çevik dönüşüm süreci kapsamında uzun zamandır var olan şelale modelinin ağır ve hantal işleyişi yerine daha hızlı reaksiyon verme kabiliyeti kazanmak içindi her şey. Benzer bir dönüşüm süreci geçtiğimiz sene içerisinde şu an çalışmakta olduğum mavi renkli teknoloji şirketinde de başlatıldı.

Her iki şirketin bu dönüşüm sürecindeki en büyük problemi ise oturmuş kültürel işleyiş yapısının değişime karşı geliyor olmasıydı. Hal böyle olunca her iki firmada dışarıdan yetkin danışmanlıklar alarak dönüşümü daha az sancılı geçirmeye çalıştı. Genelde ölçek olarak büyük bir firmaysanız bu tip dijital dönüşümler hem uzun hem de sancılı olabiliyor.

Lakin bu acıyı azaltmak için yapılanlar bazen bana çok garip gelir. Servisten iner girişe doğru ilerlersiniz. Girdiğiniz andan itibaren masanıza varıncaya dek o dijital dönüşümün bilinçaltınıza yollanan mesajlarını görürsünüz. Koridorun duvarında, bindiğiniz asansörün aynasında, tuvaletin kapısında, tavandan sarkan kartonlarda, takım panolarında, bir önceki gün dağıtılan mouse pad'lerin üzerinde, bardağınızda, bilgisayarınızın duvar kağıdında...Tüm eşyalar çoktan dijitalleşmiş ve çevikleşmiştir esasında ama önemli olan bireyin değişimidir. Kurumsal kimliğin en temel yapı taşı olan çalışanların her birinin dönüşüme ayak uydurması gerekir. Farkındalığı olan takımların bu tip dönüşümleri daha çabuk kabullendiği ve kolayca adapte olduğu gözden kaçırılmamalıdır. Olay renkli temalarla binaları giydirmekten, ilkeleri oyunlaştırarak anlatmaktan çok daha ötedir. Bu esas itibariyle bir felsefe kabülü, ciddi bir dönüşümdür.

Diğer yandan dijital dönüşüm başlar başlamaz bunu sorgulamadan kabul etmek de çok doğru değildir. Değişime direnç göstermek değil ama neden öyle olması gerektiğini sorgulamaktan bahsediyorum. Sorgusuz sualsiz kabullerin sonucu çoğunlukla çevik süreçlerin mükemmel olduğu görüşü ifade edilir ve fakat pekala buna ihtiyaç duyuluncaya kadar şelale modeli ile de başarılar elde edilmiştir. Değişen dünya artık o model tarafından yönetilememekte ve müşteri ihtiyaçları atik bir şekilde giderilememektedir. En basit terk ediş sebebi belki de bu şekilde özetlenebilir.

Pekala ben neden bu kadar felsefik konuşmaya çalışıyorum? Çeviklikten yanayım ama şelale modeli ile de yıllarca çalışmış birisiyim ve o [saturday-night-works seansı](https://github.com/buraksenyurt/saturday-night-works)nda daha çok şelale modelinde karşımıza çıkan bir tablo ile haşır neşirdim. Konu Gantt şemasıydı. Başlayalım mı?

Henry Gantt tarafından icat edilen [Gantt tabloları](https://www.gantt.com/) proje takvimlerinin şekilsel gösteriminde kullanılmaktadır. Temel olarak yatay çubuklardan oluşan bu tablolarda proje planlarını, atanmış görevleri, tahmini sürelerini ve genel olarak gidişatı görmek mümkündür. Excel üzerinde bile kullanılabilen Gantt Chart'lar sanıyorum proje yöneticilerinin de vazgeçilmez araçlarındandır. Benim [23 numaralı saturday-night-works çalışması](https://github.com/buraksenyurt/saturday-night-works/tree/master/No%2023%20-%20Gantt%20Chart%20on%20AspNet%20Core)ndaki amacım ise dhtmlxGantt isimli Javascript kütüphanesinden yararlanarak bir Asp.Net Core projesinde Gantt Chart kullanabilmekti.

Kısaca kurgudan da bahsedeyim. Görevlere ait bilgiler SQLite veri tabanıyla beslenecek. Önyüz bu veriyi kullanırken REST tipinden servis çağrıları gerçekleştirilecek. Malum veri sunucu tarafında, Gant Chart ise kullanıcı etkileşimiyle birlikte HTML sayfasında. Yani dhtmlxGantt kütüphanesi listeleme, ekleme, silme ve güncelleme gibi operasyonlar için Web API tarafına Post, Put, Delete ve Update çağrıları gönderecek. Sunucu tarafında daha çok servis odaklı bir uygulama olacağını ifade edebiliriz. Kütüphanenin kullandığı veri modelini C# tarafında konumlandırabilmek için DTO (Data Transform Object) nesnelerinden yararlanırken, sunucu tarafı operasyonlarında Model ve Controller katmanlarına başvuracağız. Heyecanlandınız, motive oldunuz, hazırsınız değil mi?:) Öyleyse notların derlenmesine başlayalım.

Bu arada örneği her zaman olduğu gibi WestWorld (Ubuntu 18.04 64bit) üzerinde geliştirmişim. İlk olarak boş bir web uygulaması oluşturalım. Ardından wwwroot klasörü ve içerisine index.html dosyasını ekleyerek devam edelim.

```bash
dotnet new web -o ProjectManagerOZ
```

> Örnekteki gantt chart çizimi için kullanılan [CSS dosyasına şu adresten](https://cdn.dhtmlx.com/gantt/edge/dhtmlxgantt.css), [Javascript dosyasına da bu adresten](https://cdn.dhtmlx.com/gantt/edge/dhtmlxgantt.js) ulaşabilirsiniz. Bu kaynakları offline çalışmak isterseniz bilgisayara indirdikten sonra wwwroot altındaki alt klasörlerde (css, js gibi) konuşlandırabilirsiniz.

Index.html

```text
<!DOCTYPE html>
<html>
<head>
    <meta name="viewport" content="width=device-width" />
    <title>Project - 19</title>
    <link href="css/dhtmlxgantt.css"
          rel="stylesheet" type="text/css" />
    <script src="js/dhtmlxgantt.js"></script>
    <script>
        // index.html dokükamanı yüklendiğinde ilgili fonksiyon devreye girerek 
        // proje veri içeriğini ekrana basacak
        document.addEventListener("DOMContentLoaded", function(event) {
            // standart zaman formatını belirtiyoruz
            gantt.config.xml_date = "%Y-%m-%d %H:%i";
            gantt.init("project_map");
 
            // veri yükleme işinin üstlenildiği kısım
            // tahmin edileceği üzere /api/backlog şeklinde bir REST API çağrısı olacak
            // bu kod tarafındaki Controller ile karşılanacak
            gantt.load("/api/backlog");
            // veri işleyicisi (bir web api servis adresi gibi düşünülebilir)
            var dp = new gantt.dataProcessor("/api/");
            dp.init(gantt);
            // REST tipinden iletişim sağlanacak
            dp.setTransactionMode("REST");
        });
    </script>
</head>
<body>
    <h2>Apollo 19 Project Plan</h2>
    <div id="project_map" style="width: 100%; height: 100vh;"></div>
</body>
</html>
```

Uygulamamız grafik verilerini göstermek için SQLite veri tabanını kullanıyor. Bu enstrümanı Entity Framework kapsamında ele alabilmek için projeye Microsoft.EntityFrameworkCore.SQLite paketini eklemeliyiz. Bunun için terminalden aşağıdaki komutu çalıştırabiliriz.

```bash
dotnet add package Microsoft.EntityFrameworkCore.SQLite
```

Sonrasında appsettings.json içeriğine bir bağlantı cümlesi ilave edebiliriz. Bunu ilerleyen kısımlarda startup dosyasında kullanacağız. Apollo.db fiziki veri tabanı dosyamızın adı ve root altındaki db klasörü içerisinde yer alacak.

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "ConnectionStrings": {
    "ApolloDataContext": "Data Source=db/Apollo.db"
  },
  "AllowedHosts": "*"
}
```

Pek tabii modellerimizi, API servis tarafı haberleşmesi için controller tiplerimizi ve hatta gantt chart kütüphanesindeki tiplerle entity modelleri arasındaki dönüşümleri kolaylaştıracak DTO nesnelerimizi geliştirmemiz gerekiyor. Uzun bir maraton olabilir. İlk olarak model sınıflarını yazarak başlayalım. Bir Models klasörü oluşturup altına Context ile model sınıflarımızı (ki gantt chart kütüphanesine göre Link ve Task isimli sınıflarımız olmalı) ekleyerek çalışmamıza devam edebiliriz.

Link sınıfı

```csharp
using System;

/*
Task'lar arasındaki ilişkinin tutulduğu Entity sınıfımız
Eğer iki Task birbiri ile bağlıysa bu sınıfa ait nesne örnekleri üzerinden ilişkilendirebiliriz.
*/
namespace ProjectManagerOZ.Models
{
    public class Link
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int SourceTaskId { get; set; }
        public int TargetTaskId { get; set; }
    }
}
```

Task sınıfı

```csharp
using System;
/* 
proje görevlerinin verisinin tutulduğu Entity sınıfımız
Tipik olarak görevle ilgili bilgiler yer alır. 
Açıklaması, süresi, hangi durumda olduğu, bağlı olduğu başka bir task varsa O, başlangıç tarihi, tipi vs
*/
 
namespace ProjectManagerOZ.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime StartDate { get; set; }
        public int Duration { get; set; }
        public decimal Progress { get; set; }
        public int? ParentId { get; set; }
        public string Type { get; set; }
    }
}
```

ve ApolloDataContext sınıfı ki bu alışkın olduğumuz tipik DataContext tipimiz. Görüldüğü üzere içerisinde görevleri ve aralarındaki ilişkileri temsil eden veri setleri sunmakta.

```csharp
using Microsoft.EntityFrameworkCore;

namespace ProjectManagerOZ.Models
{
    // Entity Framework DB Context sınıfımız
    public class ApolloDataContext
        : DbContext
    {
        public ApolloDataContext(DbContextOptions<ApolloDataContext> options)
            : base(options)
        {
        }

        // Proje görevleri ile bunlar arasındaki olası ilişkileri temsil eden özelliklere sahip
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Link> Links { get; set; }
    }
}
```

## Küçük Bir Middleware Ayarı

Uygulamamız ayağa kalktığında veri tabanının boş olma ihtimaline karşın onu doldurmak isteyebiliriz. Bunun için DataFiller isimli sınıfımız ve içerisinde Prepare isimli static bir metoduz var. Ancak söz konusu metodu host çalışma zamanında ayağa kalkarken çağırmak istiyoruz. Bunun için Program sınıfında kullanılan IWebHostBuilder üzerinden işletilebilecek bir operasyon tesis etmek lazım. Bunun için IWebHost türevlerine uyarlanabilecek bir genişletme metodu (extension method) işimizi görecektir. Bu metod çalışma zamanında entity servisinin yakalanması ve Prepare operasyonun enjekte edilmesi açısından dikkate değerdir.

DataFiller sınıfı

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using ProjectManagerOZ.Models;

namespace ProjectManagerOZ.Initializers
{
    /*
    Bu sınıfın amacı başlangıçta boş olan veritabanı tablolarına
    örnekte kullanabilmemiz için ilk verileri eklemek.
    Bu amaçla örnek task ve link'ler oluşturuluyor.
    Mesela Epic bir Work Item ve ona bağlı User Story'ler gibi
     */
    public static class DataFiller
    {
        public static void Prepare(ApolloDataContext context)
        {
            if (context.Tasks.Any()) //Eğer veritabanında en az bir Task varsa zaten veri içeriyor demektir. Bu durumda initalize işlemine gerek yok.
                return;

            // Parent task'ı oluşturuyoruz (ParentId=null)
            var epic = new Task
            {
                Text = "JWT Implementation for Category WebAPI",
                StartDate = DateTime.Today.AddDays(1),
                Duration = 5,
                Progress = 0.4m,
                ParentId = null,
                Type = "Epic"
            };
            context.Tasks.Add(epic); //Task örneğini context'e ekleyip
            context.SaveChanges(); //tabloyada yazıyoruz
            var story1 = new Task
            {
                Text = "I want to develop tokenizer service",
                StartDate = DateTime.Today.AddDays(1),
                Duration = 4,
                Progress = 0.5m,
                ParentId = epic.Id, //story'yi epic senaryoya ParentId üzerinden bağlıyoruz. Aynı bağlantı Story2 içinde gerçekleştiriliyor
                Type = "User Story"
            };
            context.Tasks.Add(story1);
            context.SaveChanges();

            var story2 = new Task
            {
                Text = "I have to implement tokinizer service",
                StartDate = DateTime.Today.AddDays(3),
                Duration = 5,
                Progress = 0.8m,
                ParentId = epic.Id,
                Type = "User Story"
            };
            context.Tasks.Add(story2);
            context.SaveChanges();

            var epic2 = new Task
            {
                Text = "Create ELK stack",
                StartDate = DateTime.Today.AddDays(3),
                Duration = 3,
                Progress = 0.2m,
                ParentId = null,
                Type = "Epic"
            };
            context.Tasks.Add(epic2);
            context.SaveChanges();

            var story3 = new Task
            {
                Text = "We have to setup Elasticsearch",
                StartDate = DateTime.Today.AddDays(6),
                Duration = 6,
                Progress = 0.0m,
                ParentId = epic2.Id,
                Type = "User Story"
            };
            context.Tasks.Add(story3);
            context.SaveChanges();

            var story4 = new Task
            {
                Text = "We have to implement Logstash to Microservices",
                StartDate = DateTime.Today.AddDays(6),
                Duration = 2,
                Progress = 0.3m,
                ParentId = epic2.Id,
                Type = "User Story"
            };
            context.Tasks.Add(story4);
            context.SaveChanges();

            var story5 = new Task
            {
                Text = "We have to setup Kibana for Elasticsearch",
                StartDate = DateTime.Today.AddDays(6),
                Duration = 2,
                Progress = 0.0m,
                ParentId = epic2.Id,
                Type = "User Story"
            };
            context.Tasks.Add(story5);
            context.SaveChanges();

            // Oluşturduğumuz proje görevleri arasındaki ilişkileri oluşturuyoruz
            List<Link> taskLinks = new List<Link>{
                new Link{SourceTaskId=epic.Id,TargetTaskId=story1.Id,Type="1"},
                new Link{SourceTaskId=epic.Id,TargetTaskId=story2.Id,Type="1"},
                new Link{SourceTaskId=epic2.Id,TargetTaskId=story3.Id,Type="1"},
                new Link{SourceTaskId=story3.Id,TargetTaskId=story4.Id,Type="1"},
                new Link{SourceTaskId=story4.Id,TargetTaskId=story5.Id,Type="1"},
                new Link{SourceTaskId=epic.Id,TargetTaskId=epic2.Id,Type="2"}
            };
            taskLinks.ForEach(l => context.Links.Add(l));
            context.SaveChanges();
        }
    }
}
```

DataFillerExtension

```csharp
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ProjectManagerOZ.Models;

/*
    DataFillerExtension sınıfı InitializeDb isimli bir extension method içeriyor.
    Bu metodu IWebHost türevli nesne örneklerine uygulayabiliyoruz.
    Amaç çalışma zamanında host ortamı inşa edilirken Middleware katmanında araya girip
    veritabanı üzerinde Prepare operasyonunu icra ettirmek.
    Bu genişletme fonksiyonunu Program.cs içerisinde kullanmaktayız.
 */
namespace ProjectManagerOZ.Initializers
{
    public static class DataFillerExtensions
    {
        public static IWebHost InitializeDb(this IWebHost webHost)
        {
            // çalışma zamanını servislerinin üreticisini örnekle
            var serviceFactory = (IServiceScopeFactory)webHost.Services.GetService(typeof(IServiceScopeFactory));

            // Bir Scope üret
            using (var currentScope = serviceFactory.CreateScope())
            {
                // Güncel ortamdan servis sağlayıcısını çek
                var serviceProvider = currentScope.ServiceProvider;
                // Servis sağlaycısından sisteme enjekte edilmiş entity context'ini iste
                var dbContext = serviceProvider.GetRequiredService<ApolloDataContext>();
                // context'i kullanarak veritabanını dolduran fonksiyonu çağır
                DataFiller.Prepare(dbContext);
            }
            // IWebHost örneğini yeni bezenmiş haliyle geri döndür
            return webHost;
        }
    }
}
```

Bu yapılanmayı kullanbilmek için program sınıfını şöyle değiştirelim.

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProjectManagerOZ.Initializers;

namespace ProjectManagerOZ
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args)
            .Build()
            .InitializeDb() // IWebHost için yazdığımız genişletme metodu.
            .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://localhost:5402");
    }
}
```

Dikkat edileceği üzere InitiateDb isimli metod CreateWebHosBuilder dönüşünden kullanılabiliyor.

Çalışmamıza startup sınıfına geçerek devam edelim. Burada Entity Framework servisinin çalışma zamanına enjekte edilmesi, statik web sayfası hizmetinin açılması, Web API tarafı için MVC özelliğinin etkinleştirilmesi, SQLite veri tabanı için gerekli bağlantı bilgisinin konfigurasyon dosyasından alınması gibi işlemlere yer veriyoruz.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore; //EF Core kullanacağımız için eklendi
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectManagerOZ.Models;

namespace ProjectManagerOZ
{
    public class Startup
    {
        /*
        Configuration özelliği ve Startup'ın overload edilmiş Constructor metodu varsayılan olarak gelmiyor.
        ApolloDbContext için gerekli connection string bilgisine ulaşacağımız Configuration nesnesine
        erişebilmek amacıyla eklendiler 
         */
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // appsettings'den SQLite için gerekli connection string bilgisini aldık
            var conStr = Configuration.GetConnectionString("ApolloDataContext");
            // ardından SQLite için gerekli DB Context'i servislere ekledik
            // Artık modellerimiz SQLite veritabanı ile çalışacak
            // Bu işlemler runtime'de gerçekleşecek
            services.AddDbContext<ApolloDataContext>(options => options.UseSqlite(conStr));
            services.AddMvc(); // Web API Controller'ının çalışabilmesi için ekledik
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // wwwroot altındaki index.html benzeri sayfaları kullanabileceğimizi belirttik
            app.UseDefaultFiles();
            // ayrıca wwwroot altındaki css, image gibi asset'lerinde kullanılacağı ifade edildi
            app.UseStaticFiles();
            app.UseMvc(); // Web API Controller'ının çalışabilmesi için ekledik
        }
    }
}
```

Veri modeli, verinin başlangıçta oluşturulması için gerekli adımlar ile çalışma zamanına ait bir takım kodlamaları halletmiş durumdayız. Veri tabanı tarafı ile konuşurken işimizi kolaylaştıracak DTO (Data Transform Object) nesneleri ile bu işin kontrolcülerini kodlayarak ilerleyelim. dto isimli bir klasör oluşturup içerisine aşağıdaki kod parçalarına sahip TaskDTO ve LinkDTO sınıflarını ekleyelim.

TaskDTO

```csharp
using System;
using System.Text.Encodings.Web;
using ProjectManagerOZ.Models;
/*
    DTO sınıfımız WebAPI tarafında, Gantt kütüphanesi ile olan haberleşmedeki mesajlaşmalarda kullanılan modeli tanımlıyor.
    Arka plandaki Task nesnemizden ziyade Gantt kütüphanesinin istediği alan adlarına sahip. Sözgelimi Task tipinde StartDate varken
    burada start_date kullanılmakta.

    Peki tabii API Controller metodlarındaki Task ve TaskDTO arasındaki dönüşümleri kolayaştırmak adına bilinçli olarak operatörlerin
    aşırı yüklendiğini görüyoruz.
*/

namespace ProjectManagerOZ.DTO
{
    public class TaskDTO
    {
        public int id { get; set; }
        public string text { get; set; }
        public string start_date { get; set; }
        public int duration { get; set; }
        public decimal progress { get; set; }
        public int? parent { get; set; }
        public string type { get; set; }
        public string target { get; set; }
        public bool open
        {
            get { return true; }
            set { }
        }

        public static explicit operator TaskDTO(Task task)
        {
            return new TaskDTO
            {
                id = task.Id,
                text = HtmlEncoder.Default.Encode(task.Text),
                start_date = task.StartDate.ToString("yyyy-MM-dd HH:mm"),
                duration = task.Duration,
                parent = task.ParentId,
                type = task.Type,
                progress = task.Progress
            };
        }

        public static explicit operator Task(TaskDTO task)
        {

            return new Task
            {
                Id = task.id,
                Text = task.text,
                StartDate = DateTime.Parse(task.start_date, System.Globalization.CultureInfo.InvariantCulture),
                Duration = task.duration,
                ParentId = task.parent,
                Type = task.type,
                Progress = task.progress
            };
        }
    }
}
```

LinkDTO

```csharp
using System;
using System.Text.Encodings.Web;
using ProjectManagerOZ.Models;
/*
    DTO sınıfımız WebAPI tarafında, Gantt kütüphanesi ile olan haberleşmedeki mesajlaşmalarda kullanılan modeli tanımlıyor.
    Arka plandaki Link nesnemizden ziyade Gantt kütüphanesinin istediği alan adlarına sahip. Peki tabii API Controller metodlarındaki 
    Link ve LinkDTO arasındaki dönüşümleri kolayaştırmak adına bilinçli olarak operatörlerin aşırı yüklendiğini görüyoruz.
*/

namespace ProjectManagerOZ.DTO
{

    public class LinkDTO
    {
        public int id { get; set; }
        public string type { get; set; }
        public int source { get; set; }
        public int target { get; set; }

        public static explicit operator LinkDTO(Link link)
        {
            return new LinkDTO
            {
                id = link.Id,
                type = link.Type,
                source = link.SourceTaskId,
                target = link.TargetTaskId
            };
        }

        public static explicit operator Link(LinkDTO link)
        {
            return new Link
            {
                Id = link.id,
                Type = link.type,
                SourceTaskId = link.source,
                TargetTaskId = link.target
            };
        }
    }
}
```

WebAPI tarafının web sayfası üzerinden gelecek HTTP çağrılarına cevap vereceği yerler Controller sınıfları. Kullanılan gantt chart kütüphanesinin işleyiş şekli gereği Link ve Task tipleri için ayrı ayrı controller sınıflarının yazılması gerekiyor.

LinkController

```csharp
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ProjectManagerOZ.Models;
using ProjectManagerOZ.DTO;
using Microsoft.EntityFrameworkCore;

/*
    Link nesneleri ile ilgili CRUD operasyonlarını üstlenen Web API Controller sınıfımız
 */
namespace ProjectManagerOZ.Controllers
{
    [Produces("application/json")] // JSON formatında çıktı üreteceğimizi belirtiyoruz
    [Route("api/link")] // Gantt Chart kütüphanesinin beklediği Link API adresi
    public class LinkController
        : Controller
    {
        // Controller içerisine pek tabii ApolloDataContext'imizi geçiyoruz.
        private readonly ApolloDataContext _context;
        public LinkController(ApolloDataContext context)
        {
            _context = context;
        }

        // Yeni bir Link eklerken devreye giren HTTP Post metodumuz
        [HttpPost]
        public IActionResult Create(LinkDTO payload)
        {
            var l = (Link)payload;

            _context.Links.Add(l);
            _context.SaveChanges();

            /*
                Task örneğinde olduğu gibi istemci tarafına oluşturulan Link
                örneğine ait Id değerini göndermemiz lazım ki, takip eden Link bağlama,
                güncelleme veya silme gibi işlemler çalışabilsin.
                tid, istemci tarafının beklediği değişken adıdır.
             */
            return Ok(new
            {
                tid = l.Id,
                action = "inserted"
            });
        }

        /*
        Bir Link'i güncellemek istediğimizde devreye giren metodumuz
         */
        [HttpPut("{id}")]
        public IActionResult Update(int id, LinkDTO payload)
        {
            // Gelen payload içeriğini backend tarafındaki model sınıfına dönüştür
            var l = (Link)payload;
            // id eşlemesi yap
            l.Id = id;
            // durumu güncellendiye çek
            _context.Entry(l).State = EntityState.Modified;
            // ve değişiklikleri kaydedip
            _context.SaveChanges();
            // HTTP 200 döndür
            return Ok();
        }

        /*
        HTTP Delete operasyonuna karşılık gelen ve
        parametre olarak gelen id değerine göre silme işlemini icra eden metodumuz
         */
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // Link örneğini bul
            var l = _context.Links.Find(id);
            if (l != null)
            {
                // Entity Context'inden ve
                _context.Links.Remove(l);
                // Kalıcı olarak veritabanından sil
                _context.SaveChanges();
            }

            return Ok();
        }

        // Tüm Link örneklerini döndüren HTTP Get metodumuz
        [HttpGet]
        public IEnumerable<LinkDTO> Get()
        {
            return _context.Links
                .ToList()
                .Select(t => (LinkDTO)t);
        }

        // Belli bir Id değerine göre ilgili Link nesnesinin DTO karşılığını döndüren HTTP Get metodumuz
        [HttpGet("{id}")]
        public LinkDTO GetById(int id)
        {
            return (LinkDTO)_context
                .Links
                .Find(id);
        }
    }
}
```

TaskController

```csharp
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ProjectManagerOZ.Models;
using ProjectManagerOZ.DTO;

namespace ProjectManagerOZ.Controllers
{
    [Produces("application/json")]
    [Route("api/task")] // Gantt Chart kütüphanesinin beklediği Task API adresi
    public class TaskController
        : Controller
    {
        // Controller içerisine pek tabii ApolloDataContext'imizi geçiyoruz.
        private readonly ApolloDataContext _context;
        public TaskController(ApolloDataContext context)
        {
            _context = context;
        }

        // HTTP Post metodumuz
        // Yeni bir Task eklemek için kullanılıyor
        [HttpPost]
        public IActionResult Create(TaskDTO task)
        {
            // Mesaj parametresi olarak gelen TaskDTO içeriğini Task tipine dönüştürdük
            var payload = (Task)task;
            // Task'ı Context'e ekle
            _context.Tasks.Add(payload);
            // Kalıcı olarak kaydet
            _context.SaveChanges();

            /*HTTP 200 Ok dönüyoruz
             Dönerken de oluşan Task Id değerini de yolluyoruz
             Bu Child task'ları bağlarken veya bir Task'ı silerken
             gerekli olan bir bilgi nitekim. Aksi halde istemci
             tarafındaki Gantt kütüphanesi kiminle işlem yapması gerektiğini bilemiyor.
             İnanmıyorsanız sadece HTTP 200 döndürüp durumu inceleyin :)
             */
            return Ok(new
            {
                tid = payload.Id,
                action = "inserted"
            });
        }

        // HTTP Put ile çalıştırılan güncelleme metodumuz
        // Parametrede Task'ın id bilgisi gelecektir
        [HttpPut("{id}")]
        public IActionResult Update(int id, TaskDTO task)
        {
            // Mesaj ile gelen TaskDTO örneğini dönüştürüp id değerini verdik
            var payload = (Task)task;
            payload.Id = id;

            // id'den ilgili Task örneğini bulduk
            var t = _context.Tasks.Find(id);

            // alan güncellemelerini yaptık
            t.Text = payload.Text;
            t.StartDate = payload.StartDate;
            t.Duration = payload.Duration;
            t.ParentId = payload.ParentId;
            t.Progress = payload.Progress;
            t.Type = payload.Type;

            // değişiklikleri veritabanına kaydettik
            _context.SaveChanges();

            // HTTP 200 Ok dönüyoruz
            return Ok();
        }

        // HTTP Delete yani silme işlemi için çalışacak metodumuz
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // Task'ı bulalım ve eğer varsa
            var task = _context.Tasks.Find(id);
            if (task != null)
            {
                // önce Context'ten 
                _context.Tasks.Remove(task);
                // sonra veritabanından silelim
                _context.SaveChanges();
            }

            // HTTP 200 Ok dönüyoruz
            return Ok();
        }

        // HTTP Get karşılığı çalışan metodumuz
        // Tüm Task'ları geri döndürür
        [HttpGet]
        public IEnumerable<TaskDTO> Get()
        {
            return _context.Tasks
                .ToList()
                .Select(t => (TaskDTO)t);
        }

        // HTTP Get ile ID bazlı çalışan metodumuz
        // Belli bir ID'ye ait Task bilgisini verir
        [HttpGet("{id}")]
        public TaskDTO GetById(int id)
        {
            return (TaskDTO)_context
                .Tasks
                .Find(id);
        }
    }
}
```

Web sayfasına HTTP Get ile çekilen görev listesi ve ilişkilerin gant chart'ın istediği tiplere dönüştürülmesi gerekiyor. İşte DTO dönüşümlerinin devreye girdiği yer. Bunun için MainController isimli tipi kullanmaktayız.

```csharp
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ProjectManagerOZ.Models;
using ProjectManagerOZ.DTO;

namespace ProjectManagerOZ.Controllers
{
    [Produces("application/json")]
    [Route("api/backlog")] // Bu adres bilgisi index.html içerisinde de geçiyor. Bulun ;)
    public class MainController : Controller
    {
        // Controller içerisine pek tabii ApolloDataContext'imizi geçiyoruz.
        private readonly ApolloDataContext _context;
        public MainController(ApolloDataContext context)
        {
            _context = context;
        }

        // HTTP Get ile verinin çekildiği metodumuz. Talebi index.html sayfasından yapıyoruz
        [HttpGet]
        public object Get()
        {
            // Task ve Link veri setlerini TaskDTO ve LinkDTO tipinden nesnelere dönüştürdüğümüz dikkatinizden kaçmamıştır.
            // Bunun sebebi Gantt'ın beklediği veri tipini sunan DTO sınıfı ile backend tarafında kullandığımız sınıfların farklı olmasıdır.

            // Dönüş olarak kullandığımız nesne data ve links isimli iki özellik tutuyor.
            // data özelliğinde Task bilgilerini
            // links özelliğinde de tasklar arasındaki bağlantı bilgilerini dönüyoruz
            // bu format özelleştirilmediği sürece Gantt Chart'ın beklediği tiptedir
            
            return new
            {
                data = _context.Tasks
                    .OrderBy(t => t.Id)
                    .ToList()
                    .Select(t => (TaskDTO)t),
                links = _context.Links
                    .ToList()
                    .Select(l => (LinkDTO)l)
            };
        }        
    }
}
```

## SQLite Ayarlamaları

Kodlarımızı tamamladık lakin testlere başlamadan önce SQLite veri tabanının oluşturulması gerekiyor. Tipik bir migration süreci çalıştıracağız. Bunun için terminalden aşağıdaki komutları kullanabiliriz.

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

İlk satır işletildiğinde DataContext türevli sınıf baz alınarak migration planları çıkartılır. Planlar hazırlandıktan sonra ikinci komut ile update işlemi yürütülür ve ilgili tablolar SQLite veri tabanı içerisine ilave edilir.

![image.axd](images/image.axd)

## Çalışma Zamanı

Kod ilk çalıştırıldığında eğer Tasks tablosunda herhangibir kayıt yoksa aşağıdaki gibi bir kaç verinin eklendiği görülecektir.

![image.axd](images/image.axd)

Benzer şekilde Links tablosuna gidilirse görevler arası ilişkilerin eklendiği de görülecektir.

![image.axd](images/image.axd)

> Visual Studio Code tarafında SQLite veri tabanı ile ilgili işleri görsel olarak yapabilmek için [şu eklentiyi](https://marketplace.visualstudio.com/items?itemName=alexcvzz.vscode-sqlite) kullanabilirsiniz.

Uygulamamızı terminalden

```bash
dotnet run
```

komutu ile çalıştırdıktan sonra Index sayfasını talep edersek bizi bir proje yönetim ekranının karşıladığını görebiliriz;) Bu sayfanın verisi tahmin edeceğiniz üzere MainController tipine gelen HTTP Get çağrısı ile sağlanmaktadır.

![image.axd](images/image.axd)

Burada dikkat edilmesi gereken bir nokta var. Gantt Chart için yazılmış olan kütüphane standart olarak Task ve Link tipleri ile çalışırken REST API çağrılarını kullanmaktadır. Yeni bir öğe eklerken POST, bir öğeyi güncellerken PUT ve son olarak silme işlemlerinde DELETE operasyonlarına başvurulur. Eğer örnek senaryomuzda TaskController ve LinkController tiplerinin POST, PUT, DELETE ve GET karşılıklarını yazmassak arabirimdeki değişiklikler sunucu tarafına aktarılamayacak ve aşağıdaki ekran görüntüsündekine benzer hatalar alınacaktır.

![image.axd](images/image.axd)

HTTP çağrıları LinkController ve TaskController sınıflarınca ele alındıktan sonra ise grafik üzerindeki CRUD (CreateReadUpdateDelete) operasyonlarının SQLite tarafına da başarılı bir şekilde aktarıldığı görülebilir. Örnekte üçüncü bir ana görev ile alt işi girilmiş, bir takım görevler üzerinde güncellemeler yapılmış ve görevler arası bağlantılar kurgulanmıştır. WestWorld çalışma zamanına yansıyan örnek ekran görüntüsü aşağıdaki gibidir.

![image.axd](images/image.axd)

Bu oluşumun sonuçları SQLite veritabanına da yansır.

![image.axd](images/image.axd)

Tüm CRUD operasyonları aşağıdaki ekran görüntüsüne benzer olacak şekilde HTTP çağrıları üzerinden gerçeklenir. Bunu F12 ile geçeceğiniz bölümdeki Network kısmından izleyebilirsiniz.

![image.axd](images/image.axd)

Çalışma zamanı testlerini de tamamladığımıza göre yavaş yavaş derlememizi noktalayabiliriz.

## Ben Neler Öğrendim?

Kopyala yapıştır yasağım nedeniyle yazılması uzun süren bir örnekti ama öğrenmek için tatbik etmek en güzel yöntemdir. Üstelik bu şekilde hatalar yaptırıp neyin ne için kullanıldığını ve nasıl olması gerektiğini de anlamış oluruz. Söz gelimi POST metodlarından üretilen task veya link id değerlerini döndürmezseniz bazı şeylerin ters gittiğini görebilirsiniz. Gelelim neler öğrendiğime...

- Gantt Chart'ları xdhtmlGantt asset'leri ile nasıl kolayca kullanabileceğimi
- IWebHost türevli bir tipe extension method yardımıyla yeni bir işlevselliği nasıl kazandırabileceğimi
- Bu işlevsellik içerisinde servis sağlayıcısı üzerinde Entity Context'ini nasıl yakalayabileceğimi
- Gantt Chart'ın ön yüzde kullandığı task ve link tipleri ile Model sınıfları arasındaki dönüşümlerde DTO tiplerinden yararlanmam gerektiğini
- DTO'lar içerisinde dönüştürme (cast) operatörlerinin nasıl aşırı yüklenebileceğini (operator overloading)
- Gantt Chart kütüphanesinin backend tarafı ile REST tipinden Web API çağırıları yaparak konuştuğunu
- Gantt Chart için kullanılan API Controller'larda HTTP Post için tid'nin önemini

Bu uzun ve komplike örnekte ele almaya çalıştığımız Gantt Chart kütüphanesini eminim ki kullanmayacaksınız. Malum bir çoğumuz artık VSTS gibi ortamların bize sunduğu Scrum panolarında işlerimizi yürütüyor ve iterasyon bazlı planlamalar yaptığımızdan bu tip Waterfall'a dönük tabloları çok fazla ele almıyoruz. Yine de örneğe uçtan uca yazılan bir uygulama gözüyle bakmanızı tavsiye edebilirim. Böylece geldik bir maceramızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
