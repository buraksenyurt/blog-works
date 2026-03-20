---
layout: post
title: "WCF WebHttp Services - Client Bazlı Cache"
date: 2010-03-30 07:30:00 +0300
categories:
  - wcf-eco-system
  - wcf-webhttp-services
tags:
  - wcf-eco-system
  - wcf-webhttp-services
  - csharp
  - xml
  - linq
  - wcf
  - rest
  - http
  - caching
  - generics
  - visual-studio
  - rc
---
Bir önceki yazımızda ([WCF WebHttp Services - Server Bazlı Cache)](https://www.buraksenyurt.com/admin/post/WCF-WebHttp-Services-Server-Side-Caching) hatırlayacağınız üzere WCF WebHttp Service'lerinde sunucu taraflı ön belleklemeyi (Server-Based Caching) incelemeye çalışmış ve bu işin birde istemci taraflı olanından bahsetmiştik. Aslında sunucu ve istemci taraflı ön bellekleme işleyişleri birbirlerinden tamamen farklıdır. Sunucu taraflı ön bellekleme işleyişinde, tamponlanan veriyi üreten operasyonun duration süresi dolana kadar çalıştırılmaması söz konusudur. Yani istemciden gelen ilk talebin sonucunun ön belleğe alınmasını takiben gelen taleplerde, sunucu tarafındaki operasyon kodları icra edilmemektedir.

Ne varki istemci taraflı ön belleklemenin işleyişine göre sunucu tarafındaki kodlar icra edilir ve ön bellekleme yapılacağı, HTTP Cache-Control bilgisinin istemciye gönderilen cevabın (Response) Header kısmına eklenmesi ile anlaşılır. Bir başka deyişle ilk talepten sonra gelecek taleplerde yine servis kodunun çalıştırılması gündemdedir. Dolayısıyla ispatı ve analizi pekte kolay olmayan bir konu ile karşı karşıyayız. Bu yüzden en azından nasıl hayata geçirilebileceğini görmeye çalışacağız. Elbette bir örnek geliştirerek. Gelin tembellik etmeyerek bir önceki uygulamamızdan devam etmek yerine yeni bir örnek üzerinden istemci taraflı ön belleklemenin nasıl yapılacağını araştıralım. Öncelikle aşağıdaki servis kodlarına sahip olan bir WCF REST Service Application projemiz olduğunu düşünelim.

```csharp
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;

namespace Lesson7
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class FunnyService
    {
        List<Wallpaper> wallpapers = new List<Wallpaper>
            {
                new Wallpaper{ Id=1000, Name="Ahhh!", Image=File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/Images/Ahhh.jpg"))},
                new Wallpaper{ Id=1001, Name="Blue Light", Image=File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/Images/BlueLight.jpg"))},
                new Wallpaper{ Id=1002, Name="My Dear Manager", Image=File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/Images/Manager.jpg"))},
                new Wallpaper{ Id=1003, Name="No Sacrifice No Victory", Image=File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/Images/Sacrifice.jpg"))}
            };
                
        [AspNetCacheProfile("WallpaperCache")] //web.config dosyasında WallpaperCache ismi ile output cache konfigurasyonu yapılmaktadır.
        [WebGet(UriTemplate = "Wallpapers/{Name}")]
        public List<Wallpaper> GetWallpapers(string Name)
        {
            var result = (from w in wallpapers
                         where w.Name.ToLower().Contains(Name.ToLower())
                         select w).ToList();
            return result;
        }
    }

    public class Wallpaper
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Image{ get; set; }
    }
}
```

GetWallpapers isimli operasyonun çalıştırılması sonucunda istemci tarafına, Images klasörü altında yer alan jpg uzantılı resim dosyalarına ait byte[] içeriklerini taşıyan ve Wallpaper tipinden nesne örneklerinden oluşan generic List koleksiyonu döndürülmektedir (Elbette varsayılan mesaj formatına göre XML olarak). AspNetCacheProfile niteliği yardımıyla söz konusu operasyon için Caching işleminin icra edileceği bildirilir. Bu ön belleklemenin hangi kriterlere göre yapılacağına ait tanımlamalar ise web.config dosyası içerisinde aşağıdaki gibi belirlenir.

```xml
<?xml version="1.0"?>
<configuration>  
  <system.web>
    <compilation debug="true" targetFramework="4.0" />
    <caching>
      <outputCache enableOutputCache="true"/>
      <outputCacheSettings>
        <outputCacheProfiles>
          <!-- İstemci taraflı ön bellekleme query string parametresi veya Header kısımlarını dikkate almaz. Bu nedenle varyByParam niteliğine none değerini atanmış ve varyByHeader niteliği kullanılmamıştır. Dikkat edileceği üzere ön belleklemenin istemci taraflı olacağı location niteliği ile belirlenmiştir.-->
          <add name="WallpaperCache" duration="120" varyByParam="none" location="Client"/>
        </outputCacheProfiles>
      </outputCacheSettings>
    </caching>
  </system.web>
<!-- Konfigurasyon dosyasının devamı -->
```

OutputCacheProfiles içerisine eklenen WallpaperCache profiline göre Cache süresi 120 saniyedir. Diğer taraftan istemci taraflı ön bellekleme işlemlerinde Querystring kullanımı bir faktör olmadığından varyByParam niteliğine none değeri atanmalıdır. Bu profilde belkide en önemli nokta location niteliğinin değerinin Client olarak belirlenmesidir. Böylece ön bellekleme işleyişinin istemci taraflı yapılacağı tayin edilimiş olur.

Tabi bu durumu incelemek için istemci tarafının da geliştirilmesi gerekmektedir. Nitekim istemci tarafına gönderilen cevabın Header kısmında, Caching ile ilgili bilgilerin bulunması gerekmektedir. Bu amaçla çok basit olarak aşağıdaki kodlara sahip bir Console uygulaması yazdığımızı düşünebiliriz (HttpClient ve diğer yardımcı tiplerin kullanımı söz konusu olduğundan REST Starter Kit'in parçası olan Microsoft.Http ve Microsoft.Http.Extension assmebly'larının referans edilmesi gerektiğini unutmayalım)

```csharp
using System;
using Microsoft.Http;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // HttpClient nesnesi url adresine göre oluşturulur.
            HttpClient client = new HttpClient("http://localhost:1000/");

            Send(client);

            Console.WriteLine("Kapatmak için bir tuşa basınız");
            Console.ReadLine();

        }
        static void Send(HttpClient client)
        {
            Console.WriteLine("\nAradığınız kelimeyi girin\n");
            string word = Console.ReadLine();
            // Wallpapers için gerekli request string oluşturulur. word bilgisi kullanıcı tarafından girilmektedir.
            string requestString = String.Format("Wallpapers/{0}", word);

            // Get metodundan yararlanılarak sunucu tarafına ilgili talep gönderilir
            using(HttpResponseMessage responseMessage = client.Get(requestString))
               {
                 // Sunucudan gelen cevap içerisinde yer alan Cache bilgisi ve ayrıca HTTP Statü koduna bakılır
                Console.WriteLine("{0}\n{1}\n",responseMessage.Headers.CacheControl.MaxAge,responseMessage.StatusCode);
            
                // İçerik okunur
               Console.WriteLine(responseMessage.Content.ReadAsString());
            }
        }
    }
}
```

İstemci uygulamadan kullanıcının girdiği kelimeye göre HTTP Get formatında bir talebinin gönderilmesi sağlanmaktadır. Sunucu tarafından çağırılan operasyona ait ön bellekleme işleminin istemci bazlı olduğu, istemci tarafına gönderilen cevap (Response) içerisinden öğrenilebilir. Bu amaçla örnek olarak HttpResponseMessage nesnesinin Header.CacheControl özelliği üzerinden yakalanan MaxAge değerine bakılmıştır. Bu değer dikkat edileceği üzere sunucu üzerinde belirtilen duration süresinin Timespan karşılığıdır (örneğimize göre 2 dakika).

![blg144_Runtime.gif](/assets/images/2010/blg144_Runtime.gif)

Tabiki Caching özelliğini sunucu tarafında kapatırsak ki bunun için web.config dosyasında ilgili cache profile'in enabled niteliğine false değeri atamamız yeterlidir,

```text
<add name="WallpaperCache" duration="120" varyByParam="none" location="Client" enabled="false"/>
```

ve aynı örneği yeniden test edersek aşağıdaki sonuçlar ile karşılaşırız.

![blg144_Runtime2.gif](/assets/images/2010/blg144_Runtime2.gif)

çok doğal olarak bir ön bellekleme süresi olmayacaktır. Örneği Fiddler gibi bir araç yardımıyla incelediğimizde ise istemci tarafına dönen cevaba ait Cache bilgisini daha detaylı bir şekilde görebiliriz.

![blg144_FiddlerResult.gif](/assets/images/2010/blg144_FiddlerResult.gif)

WCF WebHttp Service'ler ile ilişkili serimizde yer alan bir yazımızın daha sonuna geldik. Bu yazımızda istemci bazlı ön bellekleme işlemleri için ne gibi hazırlıklar yapılması gerektiğini gördük. En önemli noktanın web.config dosyası içerisinde belirtilen location niteliği olduğunu özetleyebiliriz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Lesson7_RC.rar (192,46 kb)](/assets/files/2010/Lesson7_RC.rar) [Örnek Visual Studio 2010 Ultimate Beta 2 Sürümünde geliştirilmiş ancak RC sürümü üzerinde de test edilmiştir]