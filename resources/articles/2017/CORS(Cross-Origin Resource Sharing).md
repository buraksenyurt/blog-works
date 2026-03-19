---
title: "CORS(Cross-Origin Resource Sharing)"
pubDate: 2017-12-29 21:01:00
categories:
  - .Net Core
tags:
  - .Net Core
  - Bash
  - C#
  - .NET
  - ASP.NET
  - ASP.NET Core
  - Web API
  - HTTP
  - GitHub
---

# CORS(Cross-Origin Resource Sharing)
![cors_6.gif](images/cors_6.gif)

Merhaba Arkadaşlar,

Geçtiğimiz günlerde Asp.Net Core tarafında SignalR kullanımını incelemeye başladım. O sırada incelediğim kaynakların birisinde UseCors isimli bir fonksiyonla karşılaştım. Uzun zamandır Cross-Site Scripting Error almamış birisi olarak.Net Core tarafında Cross-Origin Resource Sharing nasıl yapılır öğrenmem gerektiğini fark ettim. Sonunda SignalR ile ilgili araştırmalarıma bir kahve molası verip konuyu inceleyeyim dedim. Aslında W3C'un [şu adresinde](https://www.w3.org/TR/cors/) ve IETF (havalı isimleri ile Internet Engineering Task Force) kulübünün [bu adresinde](https://tools.ietf.org/html/rfc6454) konu ile ilgili standartlara ait oldukça detaylı bilgiler mevcut.

Konuyu kısaca özetlemek için aşağıdaki grafiğin yardımcı olabileceğini düşünüyorum. Senaryoda sol taraftaki adrese erişmeye çalışan farklı kaynaklar yer alıyor. İlk senaryoda http://fabrikam.com/api/products adresine http://fabrikam.com/index.html adresinden gidilmekte. Her iki adresin de ortak özelliği aynı orjin (kök, kaynak diyelim) üzerinden sunulmaları.

Ancak izleyen senaryolarda servise doğru gelmek isteyen farklı kökler görüyoruz. Farklı bir alan adı (domain), alt alan adı (sub domain), şema (https ile gelinen) ve port. Bu durumlarda hedef adres gelen talep ile ilişkili olarak bir şüpheye düşüyor. Acaba benim iznim olmayan bir adresten mi geliniyor gibi duruma paranoyakça yaklaşıp isteğe olumlu cevap vermiyor. Bu durumlara özellikle AJAX modelli servis çağrılarının yapıldığı çözümlerde sıklıkla rastlanıyor. Dolayısıyla hedef tarafının belirli bir politikaya göre istekte bulunanlara izin vermesini sağlamamız gerekiyor. Yani bir CORS policy'nin uygulanması gerekmekte.

![cors_1.gif](images/cors_1.gif)

Gelin.Net Core tarafında CORS politikalarının nasıl uygulanabileceğini basit bir örnek ile incelemeye çalışalım. İlk olarak Illegal Cross Site Script vakasını değerlendirelim. Senaryoyu ele alırken iki uygulama yazacağız. İlki 6001 numaralı port üzerinden yayın yapan bir Web API servisi olacak. Bu servisin tüketicisi ise 5000 nolu porttan çalışacak olan boş bir Web projesi. Terminal'den aşağıdaki komutu kullanarak Contoso isimli Web API uygulamasını oluşturalım.

```bash
dotnet new webapi -o Contoso
```

.Net Core'un varsayılan şablonuna göre ValuesController isimli bir hizmetimiz zaten mevcut. Yenisini yazmaya bu senaryo kapsamında gerek yok. Sadece servisin yayınlanacağı adresi değiştirmemiz yeterli. Bunun için Program.cs içeriğini aşağıdaki gibi düzenleyelim.

```csharp
public static IWebHost BuildWebHost(string[] args) =>
	WebHost.CreateDefaultBuilder(args)
		.UseUrls("http://localhost:6001/")
		.UseStartup<Startup>()
		.Build();
```

Şimdi ikinci uygulamamızı oluşturabiliriz. Aşağıdaki komutu çalıştırarak devam edelim.

```bash
dotnet new web -o Customer
```

Oluşan web uygulamasına ait wwwroot klasörüne index.html isimli bir web sayfası ekleyelim. Bu sayfanın görevi butona basıldığı zaman http://localhost:6001/api/values adresine talepte bulunup dönen içeriği ekrana basmak. Bunu yaparken jQuery ve AJAX çağrısı gerçekleştireceğiz.

```text
<!DOCTYPE html>
<meta charset="utf-8"/>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
  <title>Contoso Name List</title>
</head>
<body>
 
  <div>
    <h2>Contoso Names</h2>
    <ul id="values" />
  </div>
  <div>    
    <input type="button" value="Get" onclick="getValues();" />
    <p id="Contoso" />
  </div>
 
  <script src="https://code.jquery.com/jquery-3.2.1.js"></script>
  <script>
   
    function getValues() {
      var uri = 'http://localhost:6001/api/values';
      $('#values').empty();
       
      $.getJSON(uri)
      .done(function (data) {
        $.each(data, function (key, value) {              
          $('<li>', { text: value}).appendTo($('#values'));
        });
      });
    }
     
  </script>
</body>
</html>
```

getValues fonksiyonu içerisinde getJSON jQuery operasyonunu kullanarak webApi adresine talepte bulunuyor, dönüne içerikte dolaşarak herbir veriyi birer listItem olarak values isimli div altına ekliyoruz. jQuery'nin 3.2.1 versiyonunu kullanmaktayız. index.html, static bir web sayfası olduğundan web uygulamasının Startup.cs içeriğinde ufak bir değişiklik yapmamız gerekiyor.

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
	if (env.IsDevelopment())
	{
		app.UseDeveloperExceptionPage();
	}
	app.UseStaticFiles();
}
```

Her iki uygulamada hazır. Servis ve web uygulamalarını ayrı ayrı çalıştıralım ve Get butonuna basarak servise çağrıda bulunmaya çalışalım.

![cors_2.gif](images/cors_2.gif)

İki uygulamayı da dotnet run komutlarımız ile çalıştırdıktan sonra Get ile servisten sunulan değer listesine ulaşamadığımızı görürüz.

![Cors_3.gif](images/Cors_3.gif)

Dikkat edileceği üzere CORS Header 'Access-Control-Allow-Origin'missing şeklinde bir uyarı mesajı alınıyor. Bir başka deyişle Contoso servisine http://localhost:5000 adresinden gelinmesi için gerekli policy belgesinin olmadığı belirtilmektedir. Bu sorunu çözmek için servis tarafındaki Startup.cs içeriğine bir kaç dokunuş gerçekleştirmemiz yeterli.

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Contoso
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(bldr=>bldr.WithOrigins("http://localhost:5000"));
            app.UseMvc();
        }
    }
}
```

ConfigureServices metounda AddCors fonksiyonu ile Cross-Origin Sharing hizmetini çalışma zamanına ekliyoruz. Configure fonksiyonundaki UseCors çağrısı ile de Middleware tarafında ilgili policy'nin etkinleştirilmesini sağlıyoruz. Örneğimizde localhost:5000 adresi için gerekli Header bilgisinin ekleneceğini belirtmekteyiz. Şimdi senaryomuzu tekrar işletelim. Bu kez aşağıdaki görüntüde olduğu gibi değerlerin sayfaya basıldığını görebiliriz.

![cors_4.gif](images/cors_4.gif)

Bu başarılı talebin arka plandaki izlerine baktığımızda servis tarafından döndürülen cevabın içerisinde ek bir Header bilgisi olduğunu da görürüz.

![cors_5n.gif](images/cors_5n.gif)

Dikkat edileceği üzere Access-Control-Allow-Origin isimli header bilgisi için http://localhost:5000 değeri eklenmiştir. Benzer şekilde Request Header içerisinde de Origin bilgisi yer almaktadır.

Aslında CORS politikalarını MVC tabanlı projelerde farklı seviyelerde uygulama şansına da sahibiz. Belli bir Controller operasyonunda uygulamak istersek ilgili fonksiyona EnableCors niteliğini (attribute) belirtilen policy adıyla eklememiz yeterlidir. Benzer şekilde bu niteliği Controller seviyesinde de kullanabiliriz. Bu durumda Controller içerisindeki tüm operasyonlar için nitelik ile belirtilen CORS Policy talimatı uygulanacaktır. Action, Controller bazında uygulanabilen bu yapıyı global seviyede de kullanabiliriz. Ayrıca DisableCors niteliği ile action ve controller bazında CORS dışı bırakılma fonksiyonelliğini de sağlayabiliriz. Bu niteliklerin nasıl kullanılabildiğini araştırmanızı öneririm. (Bazı haller tarayıcıya bağlı olarak farklılıklar gösterebiliyormuş. Ben Firefox kullandım ama Chrome ve IE gibi tarayıcılarda da durumu irdelemekte yarar var)

CORS bu yazıda ele aldığımız kadar basit de değil aslında. Söz gelimi kaynak adres için gelen taleplerin belirli kriterlere göre filtrelenerek farklı kök adresler için kullanıma açılması gerekebilir. Örneğin belirli bir kökten herhangi bir Header içerecek taleplerden sadece HTTP GET, POST metodunu kullananlara izin vermek istediğinizi düşünelim. Bu durumda aşağıdaki gibi kodun yeniden düzenlenmesi gerekebilir.

```csharp
app.UseCors(bldr=>bldr
   .WithOrigins("http://localhost:5000")
   .WithMethods("GET","POST")
   .AllowAnyHeader()
);
```

Böylece geldik bir makalemizin daha sonuna. Bu yazımızda Cross Origin Resource Sharing konusunun.Net Core WebAPI servislerinde nasıl etkinleştirilebileceğini çok basit bir örnekle incelemeye çalıştık. Sanırım SignalR çalışmalarıma geri dönebilirim. Görüşmek ümidiyle hepinize mutlu günler dilerim.

[.Net Core ile ilgili diğer örnekleri GitHub adresinden çekebilirsiniz.](https://github.com/buraksenyurt/dotnetcore)
