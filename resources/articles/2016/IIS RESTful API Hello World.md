---
title: "IIS RESTful API Hello World"
pubDate: 2016-05-19 17:00:00
categories:
  - REST
tags:
  - REST
  - C#
  - .NET
  - ASP.NET
  - ASP.NET Core
  - WCF
  - JSON
  - HTTP
  - IIS
  - Authentication
  - Authorization
  - Java
  - Microservices
  - ASMX
---

# IIS RESTful API Hello World
![iisrestful_3.gif](images/iisrestful_3.gif)

Merhaba Arkadaşlar,

Bugün 20 Mayıs Cuma. Dün 19 Mayıs nedeniyle resmi tatil olan bankamız bugün de toplu olarak tatil. Ancak şirkette kalmak isteyenler durumlarını bildirip gelebiliyor.

Çalıştığım şirkettlerde işlerin hafiflediği dönemler genellikle makale yazdığım dönemler oluyor. Hazır geçtiğimiz gün de IIS Restful API hakkında araştırmalara başlamışken bugün biraz daha üzerine eğileyim, şu Hello World uygulamasını yaparak nedir ne değildir anlayayım istedim. Malzemelerimi hazırladım. En önemlisi elbetteki kafeindi. Her ne kadar çalışma masamdaki kadim dostum [VULK](http://www.lego.com/en-us/mixels/products/series-1/41501-vulk) kahveme sulansa da...

Öğrendiğime göre Microsoft IIS (Internet Information Services) takımı yönetimsel işlemler için Asp.Net Core teknolojisini kullanan RESTFul tipinden bir servis üzerinde çalışmalar yapmakta. Micro Service mimarisi benimsenerek geliştirilen alt yapı, kullanıcılarına IIS Administration ile ilgili gerekli fonksiyonellikleri modern bir API olarak sunuyor. API, Self-Hosted Window Service olarak yayınlanmakta. In-Proc çalışmalar için Hostable Web Core yapısına da sahip. Aşağıdaki şekil teknik mimarisi hakkında biraz daha fikir verecektir. (Takımın çizdiği mimari resim kadar başarılı değil ama bakmadan çizmeye çalışırsanız konuyu güzelce pekiştirebilirsiniz)

![IISRestful_2.gif](images/IISRestful_2.gif)

Bildiğiniz üzere IIS, Microsoft cephesinde kod geliştirenlerin yakından tanıdığı bir host uygulama olarak düşünülebilir. Web sitelerini (asp.net, asp vb), servisleri (asmx, wcf vb), HTTP Handler ve Module'leri bu uygulama üzerinden kolayca yayınlayabiliyoruz. IIS Manager (komut satırından inetmgr aracı ile) gibi arayüzleri de kullanarak istediğimiz yönetsel işlemleri (Application Pool'ların yönetimi, SSL'in açılıp kapatılması, site konfigurasyonlarının yapılması, Session ayarlamaları, yönlendirmeler, Handler yüklemeleri vb) görsel olarak yapabilmekteyiz.

Aslında.Net tarafında IIS'i kod bazlı yönetebiliyoruz. Ancak şu aşamada sunulan RESTFul API ile istemci tarafı teknolojisini de bağımsızlaştırılıyor. Bir başka deyişle herhangi bir türden istemci (Client) IIS ile ilgili yönetsel işlemleri RESTFul servise göndereceği basit HTTP çağrıları ile gerçekleştirebilecek (Gelecek zaman kullanıyorum çünkü proje henüz geliştirilme aşamasında) Yani Powershell'den yönetim yapılabileceği gibi Java ile yazılmış bir istemci de kullanılabilir. Hatta bu yönetsel operasyonlar mobil cihazlara kadar indirgenebilir.

> İşten çıktınız ve eve gidiyorsunuz. Bir uyarı mesajı geldi. Test sunucusundaki sitelerden birisi sorun yaşıyor. IIS Reset değil ama sitenin Recycle edilmesi geçici bir çözüm olabilir. Mobil cihazınızdan o an sorun yaşayan bu siteyi Recycle ettiğinizi, bazı konfigürasyon ayarlarını değiştirebildiğinizi düşünsenize. Tabii güvenilir bir oturum çerçevesinde ki IIS takımı SSL sertifika ve API'ye özel Key:Value bilgileri ile bunu sağlıyor.

IIS takımının bu çalışmalarını test edebilmek için sunulan örnek bir site de var. [Azure üzerinde host edilen siteye](https://www.buraksenyurt.com/admin/app/editor/%20https:/jimmyca-srv2.cloudapp.net:55539) genel kullanıma açık bir Authentication Token ile ulaşılabiliyor.

## Örnek Kod

Peki Microservice mimarisine göre tasarlanmış Asp.Net Core üzerinde oturan bu RESTFul API'yi.Net tabanlı bir istemci ile nasıl kullanabiliriz? Çok zor olmasa gerek değil mi? Altı üstü geçerli bir sertifika oluşturup REST tabanlı çağrı gerçekleştireceğiz. Dikkat edilmesi gereken noktalar, HTTP taleplerinin Header bilgisi içerisinde geçerli Key ve Value değerleri ile gönderilmesi ve sunucu bazlı sertifika doğrulatma işleminin yapılması. Gelin bunları göz önünde bulundurarak örnek Console uygulama kodlarımızı aşağıdaki gibi yazalım.

```csharp
using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace UnitsNetHelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            IISManager manager = new IISManager();
            
            Console.WriteLine("App Pools\n{0}",manager.GetAppPools());
            Console.WriteLine("Default Web Site\n{0}", manager.GetDefaultWebSiteInformations());
            Console.WriteLine("Web Sites\n{0}", manager.GetWebSites());
        }
    }
    class IISManager
    {
        const string thumbprint = "3BEA286D400717ACA726181593B827955115EDFC";
        const string headerKey = "X-Authorization";
        const string headerValue = "Bearer OgMks6N7CtZTptX2DTnLe8JvkmATOuqw1ZJnZzK1RojeYs251Wlfvg";
        const string rootPath = "https://jimmyca-srv2.cloudapp.net:55539/";

        public IISManager()
        {
            ServicePointManager.ServerCertificateValidationCallback 
                = (sender, certificate, chain, sslPolicyErrors) =>
            {
                X509Certificate2 apiCert = certificate as X509Certificate2;
                return apiCert == null ? false : apiCert.Thumbprint == thumbprint;
            };
        }

        public string GetAppPools()
        {
            return Get(String.Concat(rootPath,"api/webserver/application-pools"));
        }
        public string GetWebSites()
        {
            return Get(string.Concat(rootPath,"api/webserver/websites"));
        }
        public string GetDefaultWebSiteInformations()
        {
            return Get(string.Concat(rootPath,"api/webserver/websites/QnizhC7cy49v1mIHNH5Gdw"));
        }
        public string Get(string request)
        {
            string result=string.Empty;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add(headerKey, headerValue);
                var response = client.GetAsync(request).Result;

                if (response.StatusCode == HttpStatusCode.OK)
                    result=response.Content.ReadAsStringAsync().Result;
            }
            return result;
        }
    }
}
```

Çalışma zamanı çıktısı aşağıdaki gibi olacaktır.

![IISRestful_1.gif](images/IISRestful_1.gif)

Tamam kabul ediyorum okunurluğu epey zor. Matrix kodları içerisinde yüzüyor gibiyiz. Sonuçlar JSON formatında gelmiş durumda. Burada Newtonsoft'u devreye sokup daha okunabilir çıktılar elde edebiliriz. Hatta JSON'dan tipe dönüştürüp sonuçların domain içerisinde anlamlı nesneler olarak ele alınmasını da sağlayabiliriz. Bilin bakalım ben bu kutsal görevi kime bırakacağım...

## Peki Kodda Neler Oluyor?

En büyük yardımcımız IISManager isimli sınıfımız. Nesne örneği oluşturulurken ServicePointManager tipinden yararlanarak sunucu bazlı bir sertifika doğrulama işlemi icra ediyoruz. Çekirdek metodumuz Get isimli fonksiyon. Bu metod bir HttpClient nesnesinden yararlanarak HTTP taleplerini asenkron olarak gönderiyor. Kullanımı için HTTP paketinin Header kısmında yer alacak Key ve Value değerlerinin doğru girilmesi şart.

GetAsync metoduna gelen parametre, API ile ilgili geçerli bir HTTP talebini içeriyor. Örneğin sunucuda yer alan Application Pool'ları görmek için buraya api/webserver/application-pools bilgisini vermemiz gerekiyor. Bu bilginin başına da Root Path içeriği eklenince istenilen adres oluşuyor (https://jimmyca-srv2.cloudapp.net:55539/api/webserver/application-pools gibi)

Kolaylık olması açısından bir kaç HTTP çağrısını metodlaştırdık. GetWebSites ile web sitelerini GetDefaultWebSiteInformations ile Default Web Site'a ait temel bilgileri elde edebiliyoruz. Bazı adreslerden dönen sonuçlar kendi içerisinde farklı adresler de içeriyor. Örneğin siteleri çektiğimizde bu siteler ile ilişkili bilgilere ulaşabileceğimiz başka EndPoint adresleri ile de karşılaşıyoruz. Dolayısıyla yönetimsel açıdan aklımıza gelebilecek bir çok senaryoyu uygulamak mümkün. Pek tabi daha farklı talepler de girilebilir. [Tam liste için şu adrese bakabilirsiniz](https://jimmyca-srv2.cloudapp.net:55539/#/api/webserver).

Söz konusu API halen daha geliştirilme aşamasında. Bu API'nin kendi IIS sunucularımızda nasıl kullanılacağına dair henüz bir bilgi edinebilmiş değilim ancak şirketlerin bu API ile kendilerine hoş yönetim araçları yazabileceği kanısındayım. Özellikle yönetsel fonksiyonellikleri mobilize olarak sunabilmek büyük bir nimettir diye düşünüyorum. DevOps kültüründe de kendine güzel bir yer edinecektir kanımca. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

Kaynak: [https://blogs.msdn.microsoft.com/webdev/2016/05/09/introducing-the-iis-administration-api/](https://blogs.msdn.microsoft.com/webdev/2016/05/09/introducing-the-iis-administration-api/)
