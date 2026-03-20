---
layout: post
title: "WCF WebHttp Services - Özel Formatta Mesaj Döndürmek"
date: 2010-04-08 07:39:00 +0300
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
  - json
  - http
  - generics
  - visual-studio
  - rc
---
Bu yazımızda bizleri uzun, zorlu ve yorucu bir macera bekliyor. Şimdiden söylemek isterim ki yanınızda tatlı (Mesela kolalı jelibon olabilir), tuzlu yiyecek bir şeyler, boğaz kuruluğunuzu giderecek içecekler veya daha fazla oksijen çekmenizi sağlayacak sakızlar olsun. Unutmadan birde aspirin. Baş ağrısı için

![blg146_Giris.jpg](/assets/images/2010/blg146_Giris.jpg)

![Laughing](/assets/images/2010/smiley-laughing.gif)

Gelelim bu günkü konumuza.

Bu yazımızda, son günlerde sıklıkla üzerinde durduğumuz WCF WebHttp Service'lerinde, istemciden gelen root adres bazlı taleplerin nasıl karşılanacağını ve özel formatta mesajların nasıl döndürüleceğini incelemeye çalışıyor olacağız. Ancak işe başlamadan önce ihtiyacın ne olduğundan bahsetmemizde yarar var. Bu amaçla bir Web uygulaması üzerinden host edilen birden fazla WebHttp servisimiz olduğunu düşünerek ilerleyelim. Bu servisler içerisinde de örneğin HTTP Get taleplerinin karışılığında çeşitli tipte koleksiyonları döndüren operasyonlarımız olduğunu farz edelim. Bu durumda global.asax dosyasındaki kodlarda yönlendirme tablosuna ekleyeceğimiz adres bilgilerine göre, gelen talepleri uygun olan servislere yöndermemiz mümkün olacaktır. Bunu zaten daha önceki bir yazımızda incelemiştik.

Söz gelimi http://makineadı:port numarası/CompanyServices/AdventureWorks/Products ile http://makineadı:port numarası/CompanyServices/Chinook/Albums gibi iki talep gönderildiğini düşünelim. Bu taleplerin aynı web uygulamasından host edilen iki farklı servis tipi tarafından değerlendirildiği bir durumda, doğru yönlendirme tekniği ile uygun olan servis ve operasyonunun çağırılması mümkündür. Oysaki istemciler http://makineadı:port/CompanyServices/ adresine de talepte bulunulabilir. Böyle bir durumda ne olur? Gelin bunu açıklamak için aşağıdaki örnek servis sınıflarını içeren bir WCF REST Service Application projemiz olduğunu düşünelim.

AdventureWorksService sınıfı;

```csharp
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

namespace Lesson8
{ 
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class AdventureWorksService
    {
        [WebGet(UriTemplate = "Products")]
        public List<Product> GetProducts()
        {
            return new List<Product>
            {
                new Product{ Id=1, Name="Büyüteç", ListPrice=1.24M},
                new Product{ Id=2, Name="Stabilo Pen 68", ListPrice=2.35M},
                new Product{ Id=3, Name="Temizleme Spreyi", ListPrice=4.19M}
            };
        }
    }
}
```

Söz konusu sınıf içerisinde yer alan GetProducts isimli operasyon geriye Product tipinden bir ürün listesi döndürmekle görevlendirilmiştir. Tahmin edeceğiniz üzere şu anda kendi kendimizi yetiştirmeye çalıştığımızdan sadece anlamsız bir liste üretimi söz konusudur.

ChinookService sınıfı;

```csharp
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

namespace Lesson8
{ 
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class ChinookService
    {
        [WebGet(UriTemplate = "Artists")]
        public List<Artist> GetArtists()
        {
            return new List<Artist>
            {
                new Artist{ Id=1, Name="Ayrosimit",IsGroup=true},
                new Artist{ Id=2, Name="Megadet",IsGroup=true},
                new Artist{ Id=3, Name="Metalika",IsGroup=true},
                new Artist{ Id=4, Name="Co Satriani",IsGroup=false}
            };
        }
    }
}
```

ChinookService sınıfıda, AdventureWorksService tipine benzer bir şekilde ama bu kez Artist tipinden liste döndüren tek bir operasyon içermektedir. Buraya kadar zaten bir sorun bulunmamaktadır. Ancak aynı web uygulamasında birden fazla servisi host etmek istediğimizde, RouteTable nesnesinin Routes koleksiyonu içerisinde gerekli düzenlemelerin de yapılması gerekmektedir. Bu nedenle global.asax.cs içeriğinin aşağıdaki gibi olduğunu düşünebiliriz.

```csharp
using System;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Routing;

namespace Lesson8
{
    public class Global 
        : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes();
        }

        private void RegisterRoutes()
        {
            RouteTable.Routes.Add(new ServiceRoute("AdventureWorks", new WebServiceHostFactory(), typeof(AdventureWorksService)));
            RouteTable.Routes.Add(new ServiceRoute("Chinook", new WebServiceHostFactory(), typeof(ChinookService)));
        }
    }
}
```

Buna göre istemciden gelecek olan http://localhost:10843/CompanyServices/AdventureWorks/Products ve http://localhost:10843/CompanyServices/Chinook/Artists talepleri sorunsuz bir şekilde karşılanacaktır. Ancak doğrudan Web uygulamasının Root adresine yapılan http://localhost:10843/CompanyServices/ gibi bir talepte aşağıdaki ekran görüntüsü ile karşılaşılacaktır.

![blg146_Begining.gif](/assets/images/2010/blg146_Begining.gif)

Elbette Help sayfalarına gidilerek servislere nasıl talepte bulunulabileceği öğrenilebilir. Ancak elimizde iki servis olduğundan söz konusu yardım sayfalarına gitmek için http://localhost:10843/CompanyServices/AdventureWorks/help veya http://localhost:10843/CompanyServices/Chinook/help gibi taleplerinin gönderilmesi gerekmektedir.

Sanıyorum ki nihayet ne yapmak istediğimize gelebildik. İstediğimiz şey http://localhost:10843/CompanyServices/ adresine yapılan talep ile Web uygulamasından sunulan servisleri bildirmek olacak. Üstelik bu talebe karşılık dönecek mesajın içeriğini kendimiz tasarlayacağız. Yapabilir miyiz? Evet yapabiliriz. Çünkü gerekli tüm tipler Framework içerisinde çoktandır mevcutlar. Özetle talebe uygun bir formatta (XML, JSON, ATOM gibi) kendi veri yayınımızı yapacağımızı ifade edebiliriz.

İşe ilk olarak yeni bir servis sınıfını geliştirerek başlamamız gerekiyor. Bu sınıf içerisinde yer alan operasyonumuz geriye, System.ServiceModel.Channels isim alanında yer alan Message tipinden bir değer döndürüyor olacak. İşte EntranceService isimli yeni sınıfımızın içeriği;

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Syndication;
using System.ServiceModel.Web;

namespace Lesson8
{ 
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class EntranceService
    {
        // İstemci tarafına sunulacak olan kaynakların listesi Resource tipinden generic bir koleksiyonda tutulur.
        public static readonly List<Resource> Resources = new List<Resource>
        {
            new Resource{ Name="Chinook List", Description="Chinook nesneleri listesi", RequestUri=new Uri("Chinook",UriKind.Relative),HelpUri=new Uri("Chinook/help",UriKind.Relative)},
            new Resource{ Name="Adventure Works List", Description="Adventure Works nesneleri listesi", RequestUri=new Uri("AdventureWorks",UriKind.Relative),HelpUri=new Uri("AdventureWorks/help",UriKind.Relative)}
        };

        [WebGet(UriTemplate="")]
        public Message GetResources()
        {
            WebOperationContext optContext = WebOperationContext.Current;
            IncomingWebRequestContext incomingRequest = optContext.IncomingRequest;

            string mType=string.Empty;
            foreach (var acceptType in incomingRequest.GetAcceptHeaderElements())
            {
                // Gelen talebin Header bilgisinde yer alan MediaType değerine göre geriye Xml, Json veya Atom formatında bir içerik döndürülmesi sağlanır.
                 mType= acceptType.MediaType.ToLower();

                 if (mType == "application/xml" || mType == "text/xml")
                     return optContext.CreateXmlResponse(Resources); // Xml formatında dönüş
                 else if (mType == "application/json")
                     return optContext.CreateJsonResponse(Resources);
                 else if (mType == "application/atom+xml") // Json formatında dönüş
                 {
                     // Atom formatında dönüş için SyndicationFeed nesnesinin örneklenmesi gerekmektedir. Resource değişkeninin işaret ettiği koleksiyon bu nesne örneği içerisindeki Items koleksiyonuna atanır
                     return optContext.CreateAtom10Response(
                         new SyndicationFeed(
                             "Company Services Resources", 
                             "Adventure Works & Chinook Kaynakları", 
                             new Uri("", UriKind.Relative), 
                             Resources.Select(r => new SyndicationItem(r.Name, r.Description, r.RequestUri)
                             )));
                 }
            }
            // Varsayılan olarak çıktı XML formatında verilir
            return optContext.CreateXmlResponse(Resources);
        }
    }

    // Servisten sunulan servislerin birer kaynak olduğu düşünüldüğünde bu servislere ait ad, açıklama, root Uri ve help page uri bilgilerinin saklandığı tip
    public class Resource
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Uri RequestUri { get; set; }
        public Uri HelpUri { get; set; }
    }
}
```

GetResources isimli servis operasyonunun en önemli özelliği WebGet niteliğinde boş bir template kullanılması ve tabiki geriye Message tipinden bir değer döndürmesidir. Servis operasyonu dikkatlice incelendiğinde, istemciden gelecek olan taleplere ait Header kısımlarında yer alan mesaj formatı bilgisine göre bir çıktı üretildiği görülebilir. Buna göre standart olarak XML, JSON ve ATOM formatlarında bir üretim söz konusudur. ATOM formatındaki çıktının hazırlanması sırasında bir SyndicationFeed nesnesinin örneklendiğine dikkat edilmelidir. Tüm bu formatlama işlemlerinde o anki Web içeriği referansını taşıyan WebOperationContext tipine ait Create... metodlarından yararlanılmaktadır.

Tabi işimiz bu servis sınıfını yazmakla bitmiş değil. Fark ettiğiniz üzere üçüncü bir servis sınıfımız oldu ve bu sınıfa gelen talepler UriTemplate bilgisine göre Web uygulamasının Root adresine yapılmakta. Dolayısıyla boş template için gerekli yönlendirme bilgisinin global.asax.cs içerisinde bildirilmesi gerekiyor. Aynen aşağıda olduğu gibi.

```csharp
using System;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Routing;

namespace Lesson8
{
    public class Global 
        : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes();
        }

        private void RegisterRoutes()
        {
            RouteTable.Routes.Add(new ServiceRoute("", new WebServiceHostFactory(), typeof(EntranceService)));
            RouteTable.Routes.Add(new ServiceRoute("AdventureWorks", new WebServiceHostFactory(), typeof(AdventureWorksService)));
            RouteTable.Routes.Add(new ServiceRoute("Chinook", new WebServiceHostFactory(), typeof(ChinookService)));
        }
    }
}
```

Buna göre örnek bir tarayıcı uygulama üzerinden http://localhost:10843/CompanyServices/ adresine yapacağımız bir talebin sonucu aşağıdaki gibi olacaktır.

![blg146_Last.gif](/assets/images/2010/blg146_Last.gif)

Her şey yolunda görünüyor. Ancak ufak bir pürüz var. EntranceService tipine boş Uri bilgisi üzerinden bir başka deyişle Web uygulamasına ait Root adresten gidilebildiği için, http://localhost:10843/CompanyServices/help şeklinde gönderilen bir talepte aşağıdaki ekran görüntüsü ile karşılaşılacaktır.

![blg146_HelpPage.gif](/assets/images/2010/blg146_HelpPage.gif)

Oysaki bu Help sayfasının çıkmasına pekte gerek yoktur. Bu bir zorunluluk değildir ama olmasının da bir anlamı yoktur. Dolayısıyla pasif hale getirmemiz gerekmektedir. Bu amaçla WCF 4.0 ile birlikte konfigurasyon dosyasına getirilen yeniliklerden yararlanarak gerekli sonucu elde edebiliriz. Tek yapmamız gereken Web.config dosyasını aşağıdaki gibi düzenlemek.

```xml
<?xml version="1.0"?>
<configuration>  
  <system.web>
    <compilation debug="true" targetFramework="4.0" />
  </system.web>
  <system.webServer>
    <modules runAllManagedModulesForAllRequests="true">
      <add name="UrlRoutingModule" type="System.Web.Routing.UrlRoutingModule, System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" />
    </modules>
  </system.webServer>
  <system.serviceModel>
    <serviceHostingEnvironment aspNetCompatibilityEnabled="true"/>
    <standardEndpoints>
      <webHttpEndpoint>
        <!-- Diğer servislerin help sayfalarının disable olmaması için ilk standartEndpoint elementine dokunulmamalıdır-->
        <standardEndpoint name="" helpEnabled="true" automaticFormatSelectionEnabled="true"/>
        <standardEndpoint name="EntranceServiceEndPoint"/><!-- Varsayılan olarak helpEnabled özelliği false değere sahiptir-->
      </webHttpEndpoint>
    </standardEndpoints>
    <services>
      <service name="Lesson8.EntranceService">        
        <endpoint contract="Lesson8.EntranceService" kind="webHttpEndpoint" endpointConfiguration="EntranceServiceEndPoint"/>
      </service>
    </services>
  </system.serviceModel>
</configuration>
```

Zaten varsayılan web.config dosyası içeriğine göre, tüm servis talepleri otomatik olarak standart bir Endpoint tipine yönlendirilir. Ancak senaryomuza göre ana adres üzerinden yapılan yardım sayfası talebi geçersiz olmalı, diğerleri ile kullanılabilir durumda kalmalıdır. Bu nedenle EntranceService isimli hizmet için de bir Endpoint tanımlaması yapılmış ve webHttpEndpoint içerisindeki farklı bir ayara yönlendirilmiştir. Buradaki düzenlemeye göre EntranceService dışındaki tüm servislerin help sayfalarına ulaşılabilmektedir. Ancak EntranceService için help sayfası gösterilmemektedir. Buna göre http://localhost:10843/CompanyServices/help adresine bir talepte bulunulduğunda aşağıdaki ekran görüntüsü ile karşılaşılacaktır.

![blg146_HelpDisabled.gif](/assets/images/2010/blg146_HelpDisabled.gif)

Servis tarafında pek çok işimizi hallettik. Ancak test etmemiz gereken bir husus daha var. İstemcinin, XML dışında ATOM veya JSON formatlı mesaj taleplerine karşılık olarak nasıl sonuçlar alacağı. Nitekim tarayıcı üzerinden yaptığımız taleplerde standart olarak XML çıktısı aldığımızı gördük ve biliyoruz. Peki ya diğer formatlar? Bu durumu test etmek için yine basit bir Console uygulaması geliştiriyor olacağız. Her zamanki gibi HttpClient tipinden yararlanacağız. Bu sebepten REST Starter Kit ile gelen Microsoft.Http ve Microsoft.Http.Extensions Assembly referanslarını eklemeyi unutmayalım. İşte Console uygulamamıza ait kodlarımız.

```csharp
using System;
using Microsoft.Http;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (HttpClient client = new HttpClient("http://localhost:10843/CompanyServices/"))
            {
                Execute(client,"application/json"); // Json formatında talep gönderilir
                Execute(client, "application/atom+xml"); // atom formatında talep gönderilir
                Execute(client, "noFormat"); // Olmayan bir format için talep gönderilir
            }
        }

        private static void Execute(HttpClient client,string acceptFormat)
        {
            // HttpRequestMessage nesnesi örneklenirken kullanılan ikinci parametreye göre EntranceService tarafından karşılanacak bir talep oluşturulur
            using (HttpRequestMessage requestMessage = new HttpRequestMessage("GET", String.Empty))
            {
                // Accept özelliğinin Add metodu yardımıyla Header' a eklenen bilgiye göre hangi formatta mesaj istendiğin belirtilir.
                requestMessage.Headers.Accept.Add(acceptFormat);
                using (HttpResponseMessage responseMesssage = client.Send(requestMessage))
                {
                    Console.WriteLine("\n{0}\n", responseMesssage.Content.ReadAsString());
                }
            }
        }
    }
}
```

Kodlarımızı çalıştırdığımızda aşağıdaki ekran görüntüsünde yer alan sonuçları elde ederiz.

![blg146_RuntimeLast.gif](/assets/images/2010/blg146_RuntimeLast.gif)

Vuuuvvvv!!! Şu anda masamdaki şekerlerin oranına bakıyorum da...Baya bir tüketmişim.

![Sealed](/assets/images/2010/smiley-sealed.gif)

Artık dinlenmeye çekilmenin vakti geldi sanırım. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Lesson8_RC.rar (181,52 kb)](/assets/files/2010/Lesson8_RC.rar) [Örnek Visual Studio 2010 Ultimate Beta 2 Sürümünde geliştirilmiş ancak RC sürümü üzerinde de test edilmiştir]
