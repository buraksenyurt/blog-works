---
layout: post
title: "Asp.Net Core Routing Mekanizmasını Kavramak"
date: 2018-02-02 04:24:00 +0300
categories:
  - asp-dotnet-core
tags:
  - asp.net-core
  - routing
  - .net-core
  - csharp
  - template
  - http
  - html
  - json
  - web-api
  - rest-api
  - webhostbuilder
  - RouteHandler
---
Güzel otomobilleri hepimiz severiz. Özellikle spor olanlarını. Benim favori araçlarımdan birisi ise Audi RS8. 2017 model Türkiye satış fiyatı 430 bin Avro civarındaydı. Gerçekten çok yüksek bir rakam. Ne oldu da birden onunla yollarımız kesişti diye düşünebilirsiniz. Bir deneme sürüşüne çıktım demek isterdim ama... Aslında olay West-World üzerinde Asp.Net Core routing mekanizmasını incelerken meydana geldi. Bir şekilde dile benden ne dilersen tadındaki URL path'in çalışma zamanına Audi RS8 yazmış bulundum. Web sunucusunun bana verdiği cevapsa oldukça hoştu. "Oldu bil!" Onunla aramızda nasıl böyle bir muhabbet gerçekleşti merak ediyor olmalısınız. Gelin Asp.Net Core routing mekanizmasını yakından incelemeye çalışalım.

![corerouting_11.gif](/assets/images/2018/corerouting_11.gif)

Asp.Net dünyasında MVC zamanlarından beri kritik bir yere sahip olan talep yönlendirme mekanizması.Net Core için de etkili bir biçimde kullanılmakta. Farklı yöntemlerle web sunucusuna gelen taleplerin değerlendirilmesi mümkün. Bu senaryoların hali hazırdaki versiyonlarını zaten Web API ve MVC şablonlarında ele alıyoruz. Ancak mekanizmayı tanımak adında bir Console projesinden ayağa kaldırılacak Host örneğinde ne gibi operasyonları kullanabilirize bakmakta yarar var. Dilerseniz hiç vakit kaybetdemen bu varyasyonlardan bir kısmını basit örneklerle ele alalım.

İlk olarak bir Console uygulaması oluşturacağız. Ortamımız her zaman ki gibi Ubuntu ve kodlama için Visual Studio Code kullanıyoruz. Ancak aynı örnekleri MacOS'da ya da Windows'ta da yazıp çalıştırabilirsiniz. Denedim, oluyor. Büyüksün.Net Core!

```bash
dotnet new console -o RouterSamples
```

Yönlendirme, Kestrel sunucusunu ayağa kaldırma, HTML çıktıları üretme gibi operasyonlar için Asp.Net Core'un temel kütüphanelerini projeye eklememiz lazım. Tek tek uğraşabiliriz de ama ben Microsoft.AspNetCore.All paketini ekleyerek ilerlemeyi tercih ettim.

```bash
dotnet add package Microsoft.AspNetCore.All
dotnet restore
```

MapGet Örnekleri

İlk kod parçasında path bilgisine özel olarak gelen HTTP Get taleplerini nasıl ele alabileceğimize bakacağız. Programımıza aşağıdaki kod parçalarını ekleyerek devam edelim.

```csharp
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace RouterSamples
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new WebHostBuilder()
            .UseKestrel()
            .UseUrls("http://localhost:4001")
            .UseStartup<Booster>()
            .Build();

            host.Run();
        }
    }

    class Booster
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
        }

        public void Configure(IApplicationBuilder app)
        {
            var rootBuilder = new RouteBuilder(app);

            rootBuilder.MapGet("", (context) =>
            {
                context.Response.Headers.Add("Content-Type", "text/html; charset=utf-8");
                return context.Response.WriteAsync($"<h1><p style='color:orange'>Hoşgeldin Sahip</p></h1><i>Bugün nasılsın?</i>");
            }
            );

            rootBuilder.MapGet("green/mile", (context) =>
            {
                var routeData = context.GetRouteData();
                context.Response.Headers.Add("Content-Type", "text/html; charset=utf-8");
                return context.Response.WriteAsync($"Vayyy <b>Gizli yolu</b> buldun!<br/>Tebrikler.");
            }
            );

            rootBuilder.MapGet("{*urlPath}", (context) =>
            {
                var routeData = context.GetRouteData();
                return context.Response.WriteAsync($"Path bilgisi : {string.Join(",", routeData.Values)}");
            }
            );

            app.UseRouter(rootBuilder.Build());
        }
    }
}
```

Aslında web, web api veya mvc projesi açsak da benzer kurgu ile karşılaşacağız. Sonuçta belli bir adrese gelen HTTP taleplerini dinleyip bunlara karşılık cevap verecek olan bir web sunucusu yazıyoruz. Dolayısıyla işin başlangıç noktası WebHostBuilder sınıfı. Fluent yapısı sayesinde bir metod zinciri ile belirli özelliklerini etkinleştiriyoruz. Kestrel web motorunun kullanılacağını, localhost:4001 adresinden dinlemede kalınacağını, başlangıç ayarları için Booster sınıfına bakılacağını vs...En nihayetinde de Build ve Run çağrıları ile sunucunun ayağa kaldırılması. Gayet sade, yalın, anlaşılır. Çok sevdiğim bir yapı.

Tabii bizim odak noktamız daha çok Booster sınıfının içeriği. ConfigureServices metodunda route tanımlamaları ile ilgilenecek servisi devreye alıyoruz. Configure fonkisyonunda ise yazımıza konu olan route mekanizmalarının ilk üç örneği bulunuyor. MapGet operasyonunun ilk kullanımında doğrudan http://localhost:4001 talebine karşılık vermekteyiz. İçerik tipinin HTML olacağını Header bilgisine eklerken takip eden WriteAsync çağrısında da örnek bir içerik basıyoruz. Bir sonraki MapGet kullanımında ise http://localhost:4001/green/mile adresine gelen talebi ele almaktayız. Bu sefer bir öncekinden farklı olarak ilk parametrede path bilgisi verildiğine dikkat edelim. Yine bir HTML içeriği basıyoruz. Son çağrıda ise * karakterinin kullanıldığı görülmekte. Yani ilk iki path bilgisinden farklı bir adresle talep gelirse ne yapılacağı ele alınıyor. Örneğin http://localhost:4001/nowhere adresine ait talep bu fonksiyonla karşılanacak.

MapGet fonksiyonunun ilk parametresi path bilgisini kullanırken ikinci parametre RequestDelegate tipinden bir temsilci. Bu temsilcinin aldığı parametreden yararlanarak Request ve Response nesnelerine müdahale etmemiz mümkün. Header bilgisine ilaveler yapmak/okumak, url parametrelerini değerlendirmek, içeriği değiştirmek bunlara örnek olarak verilebilir. Temel olarak Request, Response bloklarını HTTP Path bazında değerlendirdiğimizi ifade edebiliriz.

Gelelim çalışma zamanı sonuçlarına. Eğer 4001 nolu porta doğrudan gidersek aşağıdaki sonucu elde ederiz.

![corerouting_1.gif](/assets/images/2018/corerouting_1.gif)

Eğer green/mile path bilgisini kullanırsak da aşağıdaki sonuçla karşılaşırız.

![CoreRouting_2.gif](/assets/images/2018/CoreRouting_2.gif)

Bu iki path dışında farklı bir path ile gelinirse, {*urlPath} bildirimi nedeniyle aşağıdakine benzer içeriklerle karşılaşırız. Sadece gelen path bilgisini ekrana bastırdığımıza dikkat edelim. Pekala ana sayfaya yönlendirme de yapabilir ya da HTTP 404 NotFound mesajı döndürebilirdik. Bu ikisini deneyin derim.

![CoreRouting_3.gif](/assets/images/2018/CoreRouting_3.gif)

MapGet çağrılarında varsayılan değerleri ele almakta mümkün. Aynı kodun {*urlPath} operasyonunu yorum satırı haline getirip aşağıdaki ilaveyi yaptığımızı düşünelim.

```csharp
rootBuilder.MapGet("whatyouwant/{wanted=1 Bitcoin please}", (context) =>
{
	var values = context.GetRouteData().Values;
	context.Response.Headers.Add("Content-Type", "text/html; charset=utf-8");
	return context.Response.WriteAsync($"İstediğin şey bu.<h2>{values["wanted"]}</h2>OLDU BİL :)");
});
```

Bu kez http://localhost:4001/whatyouwant/something benzeri talepleri karşılıyoruz. Dikkat edilmesi gereken husus {} içeriği. Burada wanted isimli bir değişken tanımladık. Aslında bu değişken içeriği GetRouteData ().Values ile elde edilen listede yer alıyor. Bu nedenle HTML çıktısını üretirken ["wanted"] şeklinde erişerek kullanıcının path'e yazdığı değişken bilgisini yakalayabiliyoruz. = sonrası yapılan 1 Bitcoin please ataması ise varsayılan değer oluyor. Buna göre aşağıdaki iki farklı kullanım da geçerli. İlkinde kullanıcının path içerisine koyduğu örnek bir wanted değişkeni var.

![coreRouting_4.gif](/assets/images/2018/coreRouting_4.gif)

Eğer parametre girmessek de aşağıdaki sonuçla karşılaşırız.

![corerouting_5.gif](/assets/images/2018/corerouting_5.gif)

Varsayılan Handler'ı Kurcalamak

Şimdi ikinci örneğimize geçelim. Bu kez varsayılan HTTP Handler davranışına müdahale edeceğiz. Örnek olarak /products/books path'ine gelen taleplere karşılık çeşitli kitap bilgileri içeren bir JSON içeriği basacağız (Alın size REST bazlı Data Service yolu) Bunun dışındaki talepler içinde sıradan bir HTML sayfası göstereceğiz. İlk senaryo bir nevi Web API simülasyonu gibi olacak. Gelin vakit kaybetmeden kodlarımızı yazalım.

```csharp
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System;

namespace RouterSamples
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new WebHostBuilder()
            .UseKestrel()
            .UseUrls("http://localhost:4001")
            .UseStartup<BoosterV2>()
            .Build();

            host.Run();
        }
    }

    class BoosterV2
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
        }

        public void Configure(IApplicationBuilder app)
        {
            var handler = new RouteHandler(context =>
            {
                var routeValues = context.GetRouteData().Values;
                var path = context.Request.Path;
                if (path == "/products/books")
                {
                    context.Response.Headers.Add("Content-Type", "application/json");
                    var books = File.ReadAllText("books.json");
                    return context.Response.WriteAsync(books);
                }

                context.Response.Headers.Add("Content-Type", "text/html;charset=utf-8");
                return context.Response.WriteAsync(
                    $@" 
                    <html>
					<body>
                    <h2>Selam Patron! Bugün nasılsın?</h2>
					{DateTime.Now.ToString()}
                    <ul>
                        <li><a href='/products/books'>Senin için bir kaç kitabım var. Haydi tıkla.</a></li>
						<li><a href='https://github.com/buraksenyurt'>Bu ve diğer .Net Core örneklerine bakmak istersen Git!</a></li>
                    </ul>                     
                    </body>
					</html>
                    ");
            });
            app.UseRouter(handler);
        }
    }
}
```

BoosterV2 sınıfına ait Configure metoduna odaklanalım. Talep edilen path bilgisini ve değerleri aldıktan sonra bir kıyaslama yapılıyor. Eğer talep /products/books şeklinde gelmişse [şuradaki github adresinden temin ettiğim](https://gist.github.com/nanotaboada/6396437) örnek kitapları içeren books.json dosyasını istemciye gönderiyoruz. Tabii Content-Type değerini de application/json şeklinde set etmeyi ihmal etmiyoruz. Eğer farklı herhangibir talep gelirse de bir HTML şablonu yolluyoruz. Burada iki link yer almakta. Tüm bu yönetim operasyonu RouteHandler temsilcisi ile gerçekleştirilmekte. Onu devreye almak içinse, UseRouter metoduna parametre olarak geçmemiz gerekiyor. İşte çalışma zamanı çıktıları.

Varsayılan sayfamız.

![CoreRouting_6.gif](/assets/images/2018/CoreRouting_6.gif)

ve kitaplarımız.

![CoreRouting_7.gif](/assets/images/2018/CoreRouting_7.gif)

URL Dizilimini Kodla İnşa Etmek

Gelelim bu yazımızda ele alacağımız son örneğe. Bu kez URL dizilimini kod tarafında oluşturmaya çalışacağız. BoosterV3 sınıfının kodlarını programa aşağıdaki gibi entegre edebiliriz.

```csharp
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Linq;
using System;
using Microsoft.AspNetCore.Routing.Template;

namespace RouterSamples
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new WebHostBuilder()
            .UseKestrel()
            .UseUrls("http://localhost:4001")
            .UseStartup<BoosterV3>()
            .Build();

            host.Run();
        }
    }

    class BoosterV3
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
        }

        public void Configure(IApplicationBuilder app)
        {
            var apiSegment = new TemplateSegment();
            apiSegment.Parts.Add(TemplatePart.CreateLiteral("api"));

            var serviceNameSegment = new TemplateSegment();
            serviceNameSegment.Parts.Add(
                TemplatePart.CreateParameter("serviceName",
                    isCatchAll: false,
                    isOptional: true,
                    defaultValue: null,
                    inlineConstraints: new InlineConstraint[] { })
            );

            var segments = new TemplateSegment[] {
                apiSegment,
                serviceNameSegment
            };

            var routeTemplate = new RouteTemplate("default", segments.ToList());
            var templateMatcher = new TemplateMatcher(routeTemplate, new RouteValueDictionary());

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Content-type", "text/html");
                var requestPath = context.Request.Path;
                var routeData = new RouteValueDictionary();
                var isMatch = templateMatcher.TryMatch(requestPath, routeData);
                await context.Response.WriteAsync($"Request Path is <i>{requestPath}</i><br/>Match state is <b>{isMatch}</b><br/>Requested service name is {routeData["serviceName"]}");
                await next.Invoke();
            });

            app.Run(async context =>
            {
                await context.Response.WriteAsync("");
            });
        }
    }
}
```

Configure metodu içerisinde bu kez farklı şeyler söz konusu. İlk olarak apiSegment ve serviceNameSegment isimli iki TemplateSegment örneği oluşturuluyor. İlki Literal tipindeyken ikincisi parametre türünden. Yani http://localhost:4001/api/collateral gibi bir path için api ifadesinin Literal olduğunu, collateral parçasının ise değişken türde parametre olduğunu ifade edebiliriz. serviceName isimli bu parametre'den sonra başka bir path içeriğinin geçerli olmayacağını isCatchAll'a atanan false değeri ile belirtiyoruz (true atayarak takip edecek path bildirimlerini uzatabilirsiniz) Ayrıca api ifadesinden sonra böyle bir değişken gelmek zorunda değil (isOptional=true nedeniyle) Varsayılan bir değeri de bulunmuyor.

Tanımlanan bu iki segmentin ardışıl olarak işe yaraması için bir dizide konuşlandırılması da gerekiyor. segments değişkeni burada devreye girmekte. Talebin karşılandığı yer Use fonksiyonu. Aslında UseMvc metodundan tanıdık gelmiş olabilir. Generic Func temisilcilerini ve Task tipini kullanan bu fonksiyon awaitable operasyonlar içerebilir. Bu nedenle istemciye cevaplar gönderilirken ve Middleware'deki bir sonraki bloğa geçilirken await anahtar kelimesi kullanılmakta. İçeride TemplateMatcher nesne örneği kullanılarak talep olarak gelen adresin geçerli bir segment bileşimi olup olmadığına bakılıyor. Dolayısıyla burada gelen bilginin istenen şablona uygunluğuna göre bir içerik üretimi sağlanabilir. Biz örneğimizde sadece talebin şablona uygun olup olmadığına bakıyoruz. Çalışma zamanı sonuçları aşağıdaki gibi olacaktır.

http://localhost:4001/api adresine yapılan çağrı sonrası

![coreRouting_8.gif](/assets/images/2018/coreRouting_8.gif)

Dikkat edileceği üzere karşılaştırma true dönmüştür. http://localhost:4001/api/wather gibi bir çağrı da geçerlidir. Nitekim belirtilen şablona uygundur. Ki bu sefer serviceName değişkeni de yakalanabilmiştir.

![CoreRouting_9.gif](/assets/images/2018/CoreRouting_9.gif)

Ama tabii şablona uymayan bir path bilgisi için eşleşme false değer dönecektir. Söz gelimi http://localhost:4001/api/weather/v2/soap11 veya http://localhost:4001/rest/collateral için...

![CoreRouting_10.gif](/assets/images/2018/CoreRouting_10.gif)

Bu yazımızda Asp.Net Core tarafındaki Routing mekanizmasının farklı kullanımlarını incelemeye çalıştık. Elbette daha fazlası vardır diye düşünüyorum. Şimdilik öğrenebildiklerim bunlar. Siz örnekleri geliştirmeye çalışarak ilerleyebilirsiniz. Özellikle ikinci örnek koddaki belli kategorideki ürünler mantığını ele alarak bir rest servisinin yazılmasında MapGet operasyonlarını ele alabilirsiniz. Gelen talebe göre çalışma zamanında dinamik web sayfası içeriklerini üretecek bir varsayılan handler da geliştirebilirsiniz. Hatta bir fotoğraf web sunucusu geliştirmeyi deneyebilirsiniz. Basılacak Content-Type bilgisini değiştirebildiğimize göre bu da mümkün. Elinizin altında tüm imkanlar mevcut. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
