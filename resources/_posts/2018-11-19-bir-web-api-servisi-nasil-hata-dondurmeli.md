---
layout: post
title: "Bir Web API Servisi Nasıl Hata Döndürmeli ?"
date: 2018-11-19 06:28:00 +0300
categories:
  - dotnet-core
tags:
  - http-web-api
  - web-api
  - ietf
  - problem-details
  - .net-core
  - csharp
  - http-status-codes
  - nuget
  - mvc
  - controller
  - .net
  - json
  - BadRequest
---
Bir süre öncesine kadar şirkette yeni bir lakabım vardı. "Bad Request Burak":) Devraldığımız bir kaç projede teknik borçlanmadan ötürü oluşan kirli kodlarla boğuşurken çok fazla detay vermeyen HTTP 400 hataları ile karşılaşıyordum. Bazen bir tanesini çözerken bir başkasının doğmasına neden olacak değişikliklere de sebep olabiliyordum. Hatta hesapta olmayan bu hataların neden oluştuğunu tespit etmek için harcanan zaman kaybı nedeniyle diğer görevlerim de aksıyordu. Bir süre sonra "Bad Request" ünvanı ile hatırlanır oldum:) Hatta bulaşıcı hale gelen bu ünvan ekibimdeki başka arkadaşlara da yayılıverdi. Neyse ki işleri sonunda yoluna koyduk (Her ne kadar ünvanım " Burak 200" olarak değişmese de:D)

![badrequest.jpg](/assets/images/2018/badrequest.jpg)

Neden böyle oluyordu. Bu işin bir orta noktası yok muydu? Bir Web API servisi gerçekte nasıl mesaj döndürmeliydi? İşte hata mesajlarını verirken bile bazı kritrleri göz önüne alarak evresenl bir takım standartları uygulamak gerekiyor. Aslında standartlar yazılım geliştirme yaşam döngüsünde konulması ve uygulanması zor olan kavramların başında geliyor. Özellikle endüstüriyel çözümleri göz önüne aldığımızda çeşitli toplulukların kabul gördüğü ve uygulanmasını beklediği standartlara bağlı kalmak da önem arz ediyor. IETF (Internet Engineering Task Force) bu anlamda standart koyuculardan birisi olarak karşımıza çıkmakta. Geçtiğimiz günlerde Web API servisleri ile ilgili durum kodlarının nasıl olması gerektiğini araştırırken güzel bir RFC dokümanına rastladım. Her ne kadar 2016 yılında yazılmış olsa da güncel ve geçerli bir standart olduğu aşikardı.

[RFC 7807](https://tools.ietf.org/html/rfc7807) maddesinde bir HTTP Web API servisinden hata dönüleceği zaman nasıl bir şablonda olması gerektiğine dair bilgiler yer alıyordu. Konunun özünde Web API servislerinden makinelerin okuyabileceği format ve detayda hata mesajlarının döndürülmesinden bahsediliyor. Buradaki en büyük amaç olası yeni hata kodları için farklı tipte Response içerikleri uydurmayıp belli bir standarda bağlı kalabilmek. Lakin bu gerçekleştirildiği takdirde 7807 standardına uygun her çıktı makineler veya botlar tarafından otomatik olarak algılanabilecek. Bu loglama stratejilerinde, alarm araçlarında otomatikleştirilmiş süreçlerin de çalışabileceği anlamına geliyor.

Peki konuyu örneklemeye çalışırsak nasıl bir standarttan bahsedilmekte? Diyelim ki arayüzdeki bir fonksiyon bir bankacılık işlemi için (örneğin para transferi olsun) Web API çağrısı yapmakta. Ancak müşterinin bakiyesi bu işlemi gerçekleştirmek için yeterli değil. IETF'ye göre buradaki durum çıktısının aşağıdaki gibi olması öneriliyor.

HTTP Durumu,

```text
HTTP/1.1 403 Forbidden
Content-Type: application/problem+json
Content-Language: en
```

Mesaj çıktısı,

```json
{
    "type": "https://example.com/probs/out-of-credit",
    "title": "You do not have enough credit.",
    "detail": "Your current balance is 30, but that costs 50.",
    "instance": "/account/12345/msgs/abc",
    "balance": 30,
    "accounts": ["/account/12345", "/account/67890"]
}
```

Tabii burada dikkat edilmesi gereken iki önemli konu var. Standarda göre type, title, detail, status ve instance nitelikleri çıktıda kullanılabilecek olan detaylar. Bunara ek olarak dilersek konuya özel genişletmeler ile ilave bilgiler de verebiliriz. Örnek çıktıdaki balance ve accounts bu şekilde eklenmişler. En basit haliyle type, title ve status niteliklerinin çıktıda yer alması gerekiyor.

Gördüğünüz üzere RFC, çıktının içinde hangi niteliklerin olabileceğini belirtmekte. type, title, status, detail ve instance makinelerin otomatik olarak ilgilenecekleri alanlar. Peki.Net dünyasında geliştirdiğimiz bir Web API servisinden bu şekilde çıktıları nasıl döndürebiliriz? Aslında JSON içeriklerini oluşturmak oldukça kolay. Lakin MVC'nin.Net içine gömülmüş standart Unauthorized gibi tipler için bu davranış şeklinde kökten değiştirmek daha iyi olabilir. Bu noktada işimizi kolaylaştıracak Nuget paketleri de var. Bunlardan birisi Kristian Hellang tarafından yazılmış ve [şu adreste](https://www.nuget.org/packages/Hellang.Middleware.ProblemDetails) yer alıyor.

Şimdi basit bir örnek ile konuyu deneyimleyelim. Başlangıç olarak aşağıdaki komut ile.Net Core tarafında bir Web API servisi oluşturdum.

```bash
dotnet new webapi -o DummyDataApi
```

Sonrasında hazır olarak gelen ValuesController sınıfını aşağıdaki gibi değiştirdim.

```csharp
using Microsoft.AspNetCore.Mvc;

namespace DummyDataApi.Controllers
{
    [Route("api/[controller]")]
    public class BrandController : Controller
    {
        [HttpGet("{tokenid}")]
        public IActionResult GetBrands(string tokenid)
        {
            string[] brands={"abibas","nayk","niüv balanse"};
            if(Validate(tokenid))
                return Ok(brands);
            else
                return Unauthorized();
        }

        private bool Validate(string tokenid)
        {
            return false;
        }           
    }
}
```

GetBrands isimli fonksiyon parametre olarak gelen tokenid değerine göre bir liste döndürmekte. Eğer geçerli bir tokenid değeri söz konusuysa HTTP 200 statüsünde markaların listesini döndürüyoruz. Ancak aksi durumda (ki örnekte bunu bilinçli olarak yapmaktayız) Unauthorized durumunu göndermekteyiz. Buna göre örneği Postman üzerinden yaptığım denemenin çıktısı aşağıdaki gibi oldu.

![rfc7807_1.gif](/assets/images/2018/rfc7807_1.gif)

Şimdi IETF'nin önerdiği formatta bir mesaj döndürmeye çalışacağız. Bunun için öncelikle ilgili paketi projeye eklemek gerekiyor.

```bash
dotnet add package Hellang.Middleware.ProblemDetails --version 1.0.0
```

Paketi yükledikten sonra bu servisin kullanılacağını kod tarafında belirtmeliyiz. Bir başka deyişle yeni Middleware'i çalışma zamanına bildirmeliyiz. Bunun için Startup.cs dosyasındaki Configure metoduna aşağıdaki ilaveyi yapmamız yeterli.

```text
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
	if (env.IsDevelopment())
	{
		app.UseDeveloperExceptionPage();
	}
	app.UseProblemDetails();
	app.UseMvc();
}
```

Artık Unauthorized çağrısında yeni servis devreye girecek. Postman'den aldığım cevap şöyle oldu.

![rfc7807_2.gif](/assets/images/2018/rfc7807_2.gif)

Görüldüğü üzere tam da istenen formatta bir çıktı söz konusu. Tabii type,title,status,detail,instance gibi bilgileri değiştrebilir ve ek bilgileri de buraya dahil edebiliriz. Bunun için ProblemDetails tipinden bir sınıf türetmemiz gerekiyor. Örneğimiz için aşağıdaki gibi bir sınıfı kullanabiliriz.

```csharp
using System;
namespace DummyDataApi.Controllers
{
    public class UnauthorizedTokenProblemDetails
        : Microsoft.AspNetCore.Mvc.ProblemDetails
    {
        public string IncomingTokenId { get; set; }
        public DateTime Date => DateTime.Now;
    }
}
```

Ek olarak IncomingTokenId ve Date isimli iki özellik ekledik. Şimdi BrandController içerisindeki fonksiyonu değiştirelim.

```csharp
[HttpGet("{tokenid}")]
public IActionResult GetBrands(string tokenid)
{
	string[] brands = { "abibas", "nayk", "niüv balanse" };
	if (Validate(tokenid))
		return Ok(brands);
	else
	{
		var problemDetail=new UnauthorizedTokenProblemDetails()
		{
			Type = "http://fabrikam.com/dummydataapi/",
			Title = "Token bilgisi hatalı.",
			Detail = "Gönderilen token ile marka bilgileri çekilemiyor.",
			Instance = $"/brand/{tokenid}",
			Status = 401,
			IncomingTokenId = tokenid
		};
		return BadRequest(problemDetail);
	}
}
```

Tek sıkıntı Unauthorized yerine BadRequest kullanmış olmamız. Nitekim object türünden parametre alan bir versiyon söz konusu. Bu versiyon Unauthorized ya da Forbid gibi dönüş tiplerinde bulunmadığı için böyle bir durum oluşuyor. BadRequest sınıfının yapıcı metoduna parametre olarak UnauthorizedTokenProblemDetails tipinden bir nesne örneği vererek istediğimiz çıktıyı üretiyoruz. Son değişikliklerden sonra Postman'den aldığım tepki kısmen beklediğim gibi oldu.

![rfc7807_3.gif](/assets/images/2018/rfc7807_3.gif)

En azından Bad Request için uç noktaya daha anlamlı bir mesaj ilettiğimizi ve IETF standartlarına uyduğumuzu ifade edebiliriz. Bu arada paket kodlarını [github](https://github.com/khellang/Middleware/blob/master/samples/ProblemDetails.Sample/Program.cs) üzerinden incelemenizi öneririm. Güzel bir Middeware uyarlaması bulacaksınız. Hatta paketi kullanmak yerine kodlara bakarak kendi ara modülünüzü geliştirmeyi de deneyebilirsiniz. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
