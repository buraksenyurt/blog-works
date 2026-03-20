---
layout: post
title: "Post Görünümlü Put"
date: 2018-08-02 11:51:00 +0300
categories:
  - dotnet-core
tags:
  - dotnet-core
  - bash
  - csharp
  - dotnet
  - aspnet
  - asp-dotnet-core
  - entity-framework
  - linq
  - wcf
  - rest
  - json
  - web-api
  - http
  - java
  - generics
  - visual-studio
---
Dışarıda çok güzel bir hava var. Büyük ihtimalle tüm sahil şeritlerimiz hınca hınç dolu. Denize girip serinleyenler, çimlere serilip gölgelenenler, arkadaşlarıyla birlikte naneli limonata içip hararet giderenler vs...Kimbilir belki de az sonra ahşap üzerinde çıplak ayaklarınızla yürüyecek ve incecik kumlara basıp bütün senenin yorgunluğunu atmak üzere kendinizi serin okyanus sularına bırakacaksınız. Ama birilerinin de şu yazılım dünyası için içerik üretmesi gerekiyor öyle değil mi? Doğruyu söylemek gerekirse bu tip bir misyonu üstlendiğim için memnunum. Öyleyse gelin bugün ki konumuza başlayalım.

![tun_giris.gif](/assets/images/2018/tun_giris.gif)

Hiç bir REST servisine POST talebi gönderip aslında onun PUT işlemini yapmasını istediğiniz oldu mu? Bir kaç değişik sebepten dolayı tasarlayacağınız REST tabanlı servisin bu tip senaryolara hizmet verebilir olmasını isteyebilirsiniz. Bunun bir kaç sebebi olabilir. Basit haliyle şöyle bir senaryoyu göz önünde bulundurabiliriz;

İstemci tarafı aslında bir güncelleme işlemi yapmak istiyor olsun. Örneğin bir kitabın başlığını değiştirecek. Bunun için tipik olarak HTTP PUT talebini göndermesi yeterli olacaktır. Serviste güncelleme için PUT operasyonunu destekleyen bir fonksiyon olduğunu da düşünelim. Ancak Firewall'a konan katı bir kural, eski bir tarayıcı kullanılıyor olması ya da XmlHttpRequest ile talep gönderen kod parçasının ilgili komutu işlememesi gibi sebeplerden ötürü sadece POST ve GET çağrıları yapan/yapabilen istemciler olduğunu düşünelim. Yani ben PUT ile bir güncelleme talebi göndermek istediğim halde bunu yollayamıyorum. Böyle bir durumla karşılaşma ihtimalimiz epey düşük gibi görünse de olabilir (Karşılaştım o yüzden söylüyorum) Acaba gönderilen POST talebinin aslında bir PUT talebi olması gerektiğini karşı tarafa söyleyebilir miyiz? İşte yazımızın konusu bu. Normalde.Net Framework üzerinde WCF servislerinde ele alınan bir konu olmasına rağmen biz.Net Core açısından olaya bakacağız.

Kobay Web API Servisi

İlk olarak örnek uygulamamızı oluşturalım. Ben her zaman ki gibi konuyu West-World'de ele alacağım. Visual Studio Code üzerindeki terminalden aşağıdaki komutu vererek işe başlayabiliriz.

```bash
dotnet new webapi -o SmartReaderApi
```

SmartReaderApi isimi Web API uygulamasında kitaplara ait bir hizmet sunduğumuzu düşünelim. Aşağıdaki Entity sınıfını bu amaçla değerlendirebiliriz.

```csharp
namespace SmartReaderApi.Models
{
    public class Book
    {
        public int BookID { get; set; }
        public string Title { get; set; }
    }
}
```

BooksController isimi bir sınıfımız daha var. Tahmin edeceğiniz üzere Controllers klasörü içerisinde yer alacak. Temel olarak kitap listesinin getirilmesi, eklenmesi, başlık bilgisinin güncellenmesi ve silinmesi gibi operasyonellikler sunacak. Yani Http GET, POST, PUT ve DELETE gibi taleplere hizmet edecek şekilde tasarlanmış durumda.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmartReaderApi.Models;

namespace SmartReaderApi.Controllers
{
    [Route("smarty/api/[controller]")]
    public class BooksController : Controller
    {
        static List<Book> _books=new List<Book>{
                new Book{BookID=1023,Title="Essential C# 6.0"},
                new Book{BookID=985,Title="Big Java, Early Objects"},
                new Book{BookID=124,Title="Kralın Düşüşü"},
            };

        [HTTPGet]
        public IActionResult Get()
        {
            return Ok(new { Books = _books });
        }

        [HttpPost]
        public IActionResult Create([FromBody]Book book)
        {
            _books.Add(book);
            return Ok(book);
        }

        [HttpPut("{bookId}")]
        public IActionResult Update(int bookID, [FromBody]Book book)
        {
            var findResult = _books.Find(b=>b.BookID==bookID);
            if (findResult == null)
            {
                return NotFound();
            }
            findResult.Title = book.Title;
            return Ok(findResult);
        }

        [HttpDelete("{bookId}")]
        public IActionResult Delete(int bookID)
        {
            var findResult = _books.Find(b=>b.BookID==bookID);
            if (findResult == null)
            {
                return NotFound();
            }
            _books.Remove(findResult);
            return Ok(findResult);
        }
    }
}
```

Örnek bir servis olduğu için çok fazla detaya girmedik. IActionResult dönüşleri sağlayan fonksiyonlar genel olarak sabit bir kitap listesini kullanmakta. Elbette siz örneğinizdeki veri setini Entity Framework üzerinden farklı bir kaynakla bağlayaraktan da ilerleyebilirsiniz.Kitap ekleme ve güncelleme operasyonları talebin gövdesindeki JSON içeriklerini okuyarak gerekli işlemleri yapmaktalar. DELETE operasyonu verilen kitabın ID bilgisine göre koleksiyondan çıkartma işlemi gerçekleştiriyor. GET metodumuz da tahmin edeceğiniz üzere tüm kitap listesini döndürmekte. Bunlara ek olarak benim West-World için yapmam gereken ufak bir değişiklik daha var. 5000 numaralı port başka bir ürünün himayesi altında. Bu nedenle farklı bir port üzerinden uygulamamı çalıştırmak durumundayım. O nedenle program sınıfında UseUrls ile ufak bir değişiklik yapmam gerekiyor.

```csharp
public static IWebHost BuildWebHost(string[] args) =>
    WebHost.CreateDefaultBuilder(args)
        .UseStartup<Startup>()
        .UseUrls("http://localhost:5555")
        .Build();
```

Middleware Sınıfının Geliştirilmesi

Aslında senaryoyu gerçekleştirmek için yapılması gereken şey belli. Bir şekilde istemciden gelen mesajı yakalamalı, header içerisindeki bilgiye bakıp (X-HTTP-Method-Override anahtarının değerine bakacağız) asıl HTTP metodu yerine hangi işlemin uygulanması gerektiğini anlamalıyız. Bunu yaparken de bizim izin verdiğimiz HTTP metodları çerçevesinde gerçekleştirilir olmasına özen göstermeliyiz. Şimdi projeye middlewares isimli bir klasör ekleyelim ve içerisine aşağıdaki sınıfları koyalım (Asp.Net Core tarafında Middleware yazılması ile ilgili olarak [şu yazıya da](https://www.buraksenyurt.com/post/web-api-icin-custom-middleware-yazmak) göz atabilirsiniz)

HttpOverrider sınıfı

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace SmartReaderApi.Middlewares
{
    public class HttpOverrider
    {
        RequestDelegate _nextRequest;
        string _headerName;
        List<string> _allowedHttpMethods;

        public HttpOverrider(RequestDelegate nextRequest, IOptions<HttpOverriderOptions> options)
        {
            _nextRequest = nextRequest ?? throw new ArgumentNullException(nameof(nextRequest));

            if (options?.Value == null)
                throw new ArgumentNullException(nameof(options));

            _headerName = options.Value.HeaderName;
            _allowedHttpMethods = new List<string>();
            foreach (string allowedMethod in options.Value.AllowedMethods)
            {
                _allowedHttpMethods.Add(allowedMethod);
            }
        }
        
        public Task Invoke(HttpContext context)
        {
            if (HttpMethods.IsPost(context.Request.Method))
            {
                if (context.Request.Headers.ContainsKey(_headerName))
                {
                    string xHttpValue = context.Request.Headers[_headerName];
                    if (_allowedHttpMethods.Contains(xHttpValue))
                    {
                        var httpRequestFeature = context.Features.Get<IHttpRequestFeature>();
                        httpRequestFeature.Method = xHttpValue;
                    }
                }
            }
            return _nextRequest(context);
        }
    }
}
```

HttpOverrider sınıfının yapıcı metodunda ilk değişkenlerin içeriklerini belirlemekteyiz. Header bilgisi ve izin verilen HTTP metodlarını burada yüklüyoruz. Buna ek olarak pipeline'daki bir sonraki adımı tutacak RequestDelegate nesnesine de atama yapıyoruz ki Invoke operasyonunun sonunda akışın devam etmesini sağlayabilelim. Task tipinden nesne örneği döndüren Invoke metodunun işlevi önemli. Öncelikle POST tipinde bir talep söz konusu ise, istemcinin gönderdiği Header bilgisine göre ilerlemek gerekiyor. Eğer Header bilgisi bizim beklediğimiz gibi X-HTTP-Method-Override değerini içeriyorsa ve izin verilen operasyonlardan birisiyse, değişiklik yapıyoruz. Bu middleware sınıfını sisteme ekleyebilmek için bildiğiniz üzere IApplicationBuilder arayüzünü uygulayan tipe bir genişletme metodu (Extension method) yazılması gerekiyor. Aynen aşağıdaki kod parçasında görüldüğü gibi.

```csharp
using System;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;

namespace SmartReaderApi.Middlewares
{
    public static class HttpOverriderExtensions
    {
        public static IApplicationBuilder UseHttpMethodOverriding(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            return app.UseMiddleware<HttpOverrider>();
        }

        public static IApplicationBuilder UseHttpMethodOverriding(this IApplicationBuilder app, HttpOverriderOptions options)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            return app.UseMiddleware<HttpOverrider>(Options.Create(options));
        }
    }
}
```

Ah tabii middleware için gerekli seçenekleri içeren sınıfı da unutmamak lazım.

```csharp
using System.Collections.Generic;

namespace SmartReaderApi.Middlewares
{
    public class HttpOverriderOptions
    {
        public string HeaderName { get; set; }
        public string[] AllowedMethods { get; set; }
    }
}
```

Buradaki seçenekler ile Header bilgisini ve izin verilecek HTTP metodlarını orta katmana bildirmeyi hedefliyoruz. Tüm bu işlemler elbette yeterli değil. Yazılan HttpOverrider isimli middleware sınıfının çalışma zamanına da bildirilmesi lazım. Bunun için Startup.cs'deki Configure metodunu aşağıdaki gibi düzenlemek gerekiyor. UseMiddleware metodu HttpOverrider sınıfını kullanmakta. Parametreyi Options tipinin Create metodu ile geçirmekteyiz.

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseMiddleware<HttpOverrider>(Options.Create(new HttpOverriderOptions
    {
        HeaderName = "X-HTTP-Method-Override",
        AllowedMethods = new[] { 
            HttpMethods.Put
            ,HttpMethods.Post
            , HttpMethods.Delete
            , HttpMethods.Get
            }
    }));

    app.UseMvc();
}
```

Dikkat edileceği üzere UseMiddleware fonksiyonuna HttpOverrider sınıfını kullanacağını söylüyor ve parametre olarak istediğimiz seçenekleri sunuyoruz. Buna göre istemciden gelen mesajın Header kısmında X-HTTP-Method-Override anahtarına bakılacak. Burada izin verilen HTTP operasylarını da belirtmekteyiz. Koda göre PUT, POST, DELETE ve GET.

Testler

Sunucuyu

```bash
dotnet run
```

komutuyla çalıştırdıktan sonra bir kaç deneme yapmakta yarar var. Örneğin ilk olarak kitaplar geliyor mu buna bir bakalım. Ben Postman aracını kullanarak işlemleri gerçekleştirdim.

```text
HTTP metodu : GET
Adres: localhost:5555/smarty/api/books
```

![tun_ilk_durum.gif](/assets/images/2018/tun_ilk_durum.gif)

Tüm kitap listesinin çekildiğini görüyorsunuz. Bir güncelleme işlemi yapmak istersek PUT metoduyla gidilmesi yeterlidir.

```text
HTTP metodu : PUT
Content-Type: application/json
Adres: localhost:5555/smarty/api/books/1023
Body: {Title:"Essential C# 7.0"}
```

![tun_normalde.gif](/assets/images/2018/tun_normalde.gif)

Senaryoya göreyse PUT gibi bir metodu sunucuya gönderemiyor olmalıyız. Söz gelimi sunucu tarafından desteklenmeyen patch metodunu, POST'un kolunun altına alıp göndermeye çalışırsak bir HTTP 404 Not Found hatası alabiliriz. Burada elbette Middleware'deki kriterler devreye girmektedir. Hatırlayacağınız üzere HttpOverrider sınıfının POST, PUT, DELETE ve GET metodlarına izin veriyoruz. Dolayısıyla aşağıdaki sonuç oldukça doğaldır.

```text
HTTP metodu : POST
Adres: localhost:5555/smarty/api/books/1023
Content-Type: application/json
X-HTTP-Method-Override: PATCH
Body: {Title:""}
```

![tun_desteklenmeyen_cagri.gif](/assets/images/2018/tun_desteklenmeyen_cagri.gif)

Peki izin verilen PUT metodunu POST olarak göndermeyi denersek ne olur? Öyle ya tüm çabamız bunu görebilmek içindi. İstemci çeşitli sebeplerden ötürü sunucuya PUT talebi yapamıyor. Elinde sadece POST seçeneği var. Bunu analiz etmek için Postman'de aşağıdaki hazırlığı yapabiliriz.

```text
HTTP metodu : POST
Content-Type: application/json
X-HTTP-Method-Override: PUT
Adres: localhost:5555/smarty/api/books/1023
Body: {Title:"X-Men Triology."}
```

İşte beklenen sonuç.

![tun_desteklenen.gif](/assets/images/2018/tun_desteklenen.gif)

Dikkat edileceği üzere Web API'ye HTTP POST çağrısı yapmaktayız. Ancak Header'a eklediğimiz X-HTTP-Method-Override özelliğinin değerine de PUT ifadesini yerleştirdik. Bu, POST talebi gönderdiğimiz halde aslında PUT işlemini gerçekleştirmek istediğimiz anlamına geliyor. HTTP 200 OK sonucunu gördükten sonra hemen tüm kitap listesini tekrar çekerek istenen güncellemenin olup olmadığını kontrol etmekte yarar var. Buna göre 1023 numaralı kitabın başlığının değişmiş olması gerekiyor.

```text
HTTP metodu : GET
Adres: localhost:5555/smarty/api/books
```

![tun_sonuc.gif](/assets/images/2018/tun_sonuc.gif)

Volaaa:)

Görüldüğü gibi küçük bir hile hatta pek çok kaynakta hacking olarak da ifade ediliyor gibi ancak bizim için önemli olan, istemciden gelen Header bilgisini yakalayan ve duruma göre askiyon alan middleware sınıfının yazmış olmamız. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
