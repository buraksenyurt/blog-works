---
title: "Microsoft.Net Services - Service Bus için REST Tabanlı Hello World"
pubDate: 2009-11-19 23:45:00
categories:
  - Windows Azure
tags:
  - Windows Azure
  - C#
  - XML
  - .NET
  - WCF
  - REST
  - HTTP
  - Generics
---

# Microsoft.Net Services - Service Bus için REST Tabanlı Hello World
Merhaba Arkadaşlar,

Bir önceki [yazımızda](https://www.buraksenyurt.com/post/Microsoft-DotNet-Services-Service-Bus-Hello-World)Microsoft.Net Services alt yapsının önemli parçalarından birisi olan Service Bus hizmetini incelemeye çalışmış ve basit bir Hello World uygulama koleksiyonu geliştirmiştik. Bu yazımızda ise REST bazlı geliştirilen bir WCF servisine herhangibir tarayıcı yardımıyla HTTP Get metoduna göre, Service Bus üzerinden nasıl ulaşabileceğimizi incelemeye çalışacağız.

REST bazlı modelde bilindiği üzere Web tabanlı olarak yayınlanan servislere HTTP protokolünün Get,Post,Put,Delete gibi metodları yardımıyla erişilebilmektedir. Bu URL bazlı erişim sayesinde herhangibir tarayıcı uygulamanın söz konusu servis operasyonlarını kullanabilmesi mümkündür. Üstelik istemciler arada bir proxy nesnesine ihtiyaç duymadan doğrudan HTTP taleplerini gönderebilir. Bunlara ek olarak birde servis operasyonlarının Syndication (RSS,Atom gibi) tabanlı içerik yayınlayabilme kabiliyetleri eklendiğinde Web programlama modeline uygun bir hizmet üretiminin gerçekleştirilebildiği gözlemlenecektir.

WCF mimarisi,.Net Framework 3.5 sürümünden bu yana Web programlama modelini desteklemektedir. Bu nedenle REST (Representational State Transfer) bazlı WCF servisleri kolayca geliştirilebilir. Bizde bu günkü örneğimizde sunucu tarafında, RSS 2.0 formatında içerik yayınlaması yapan ve REST tabanlı olarak çalışabilen bir WCF servisini geliştirecek ve buna olan istemci erişimleri için Service Bus'tan yararlanacağız. Tahmin edeceğiniz üzere Microsoft.Net Services üzerinde örneğimiz için bir Service Namespace'i oluşturulması gerekmektedir. Ben bu günkü örneğimiz için günlük öneri yemek listesini sunacak bir hizmete uygun aşağıdaki isimlendirmeyi kullanmayı tercih ettim.

![blg98_ServiceNamespace.gif](images/blg98_ServiceNamespace.gif)

Bundan sonraki ilk adımımız servis uygulamasını geliştirmek olacaktır. Söz konusu uygulama WCF Web programlama modelini (Web Programming Model) kullanacağından ve Service Bus üzerinden iletişimi sağlayacağından aşağıdaki şekilde görülen Microsoft.ServiceBus, System.ServiceModel.Web ve System.ServiceModel assembly'larını referans etmelidir.

![blg98_References.gif](images/blg98_References.gif)

Gelelim sunucu uygulama tarafındaki kodlarımıza;

```csharp
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Syndication;
using System.ServiceModel.Web;
using Microsoft.ServiceBus;

namespace ServerApp
{
    // Servis sözleşmesi
    [ServiceContract(Namespace = "http://www.buraksenyurt.com/MyFoodCompany")]
    public interface IDailyFoodListContract
    {
        [OperationContract]
        [WebGet] // Http Get taleplerine cevap verecek bir operasyon olduğunu belirtiyoruz
        Rss20FeedFormatter GetDailyFoodList(string Day);
    }

    // Servisi uygulayan tip
    [ServiceBehavior(Name = "DailyFoodListService", Namespace = "http://www.buraksenyurt.com/MyFoodCompany")]
    public class DailyFoodListService
        :IDailyFoodListContract
    {
        #region IDailyFoodListContract Members
        
        // GetDailyFoodList metodu RSS 2.0 Formatında basit bir Syndication içeriği döndürmektedir.
        public Rss20FeedFormatter GetDailyFoodList(string Day)
        {
            // Öncelikli olarak Feed oluşturulur
            SyndicationFeed foodFeed = new SyndicationFeed();
            foodFeed.Id=String.Format("Day_{0}",Day);
            foodFeed.Title = new TextSyndicationContent("Günlük Yemek Listesi");

            // Feed içerisindeki Item listesi hazırlanır
            List<SyndicationItem> foodItems = new List<SyndicationItem>();
            foodItems.Add(new SyndicationItem( "Aperatifler","Patlıcanlı Musakka", new Uri("http://myfoodcompany/Food/Musakka"), "10001", DateTime.Now));
            foodItems.Add(new SyndicationItem("Çorbalar", "Mercimek Çorbası", new Uri("http://myfoodcompany/Food/MercimekCorba"), "12034", DateTime.Now));
            foodItems.Add(new SyndicationItem("Ana Yemekler", "Mozeralla Peynirli Makarna", new Uri("http://myfoodcompany/Food/MakarnaMozeralla"), "10025", DateTime.Now));

            // Item listesi feed' e eklenir.
            foodFeed.Items=foodItems;

            // RSS 2.0 formatındaki Feed içeriği üretimi için SyndicationFeed nesne örneği Rss20FeedFormatter sınıfının yapıcı metoduna parametre olarak geçirilir.
            return new Rss20FeedFormatter(foodFeed);
        }

        #endregion
    }

    //// Kanal arayüzü
    //public interface IDailyFoodListChannel
    //    : IDailyFoodListContract, IClientChannel
    //{
    //}

    class Program
    {
        static void Main(string[] args)
        {
            Uri address = ServiceBusEnvironment.CreateServiceUri("https", "MyFoodCompany", "Foods");

            // REST bazlı bir WCF Servisi host işlemi gerçekleştirileceğinden WebServiceHost nesne örneğinden yararlanılır
            WebServiceHost host = new WebServiceHost(typeof(DailyFoodListService), address);
            host.Open(); // Servis açılır

            Console.WriteLine("Servis açıldı. Servis durumu {0}\nService Adresi {1}", host.State,address.ToString());
            Console.WriteLine("Operasyon Adı : GetDailyFoodList\n");
            Console.WriteLine("Çıkmak için bir tuşa basınız");
            Console.ReadLine();

            host.Close(); // Servis kapatılır
        }
    }
}
```

WCF Servis sözleşmemiz içerisinde yer alan GetDailyFoodList operasyonu, string parametre alıp RSS 2.0 formatında içerik üreten basit bir fonksiyonellik sunmaktadır. Bu operasyonun HTTP Get taleplerine cevap verebilmesi istendiğinden WebGet niteliği ile imzalanması gerekmektedir. Diğer yandan WCF çalışma zamanının REST tabanlı talepleri değerlendirmesi istendiğinden ServiceHost yerine WebServiceHost tipinin kullanılması gerektiğide aşikardır. Bunun dışında servis operasyonumuz tamamen deneysel olarak günlük öneri yemek listesini üretmektedir.

Pekala dünya üzerindeki restoran zincirlerine veya herhangibir evin mutfağındaki dokunmatik ekranlara bu tip bir servis yardımıyla hizmet götürüyor olsaydık daha fazla parametre alacak ve veri kaynağı olarak Azure üzerinde yer alacak bir Database Service'ini kullanacak bir operasyon da geliştirebilirdik. Kimbilir belki ilerleyen zamanlarda bu tip komple bir örneğide geliştirme fırsatımız olur.

![Wink](images/smiley-wink.gif)

Servis uygulamamız tarafında çalışma zamanı konfigurasyon bilgileri içinde App.config dosyasının aşağıdaki gibi düzenlenmesi yeterlidir.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <system.serviceModel>
    <bindings>
      <webHttpRelayBinding>
        <binding name="DailyFoodListServiceBinding">
          <security relayClientAuthenticationType="None" />
        </binding>
      </webHttpRelayBinding>
    </bindings>
    <services>
      <service behaviorConfiguration="DailyFoodServiceBehavior" name="ServerApp.DailyFoodListService">
        <endpoint address="" behaviorConfiguration="credentialBehavior" binding="webHttpRelayBinding" bindingConfiguration="DailyFoodListServiceBinding" name="RelayEndpoint" contract="ServerApp.IDailyFoodListContract" />
      </service>
    </services>
    <behaviors>
      <endpointBehaviors>
        <behavior name="credentialBehavior">
          <transportClientEndpointBehavior credentialType="SharedSecret">
            <clientCredentials>
              <sharedSecret issuerName="Sizin Issuer Name değeriniz" issuerSecret="Sizin için üretilen Key değeri"/>
            </clientCredentials>
          </transportClientEndpointBehavior>
        </behavior>
      </endpointBehaviors>
      <serviceBehaviors>
        <behavior name="DailyFoodServiceBehavior">
          <serviceDebug httpHelpPageEnabled="false" httpsHelpPageEnabled="false" includeExceptionDetailInFaults="True" />
        </behavior>
      </serviceBehaviors>
    </behaviors>
  </system.serviceModel>
</configuration>
```

Konfigurasyon içeriğinde dikkat çekici noktaların başında kullanılan bağlayıcı tip gelmektedir (webHttpRelayBinding). Bu gereklidir nitekim Service Bus ile iletişim kurulacaktır. Diğer yandan clientCredentials elementi içerisinde, Service Bus projemizdeki ilgili Namespace için üretilen Issuer Name ve Issuer Key değerleri yer almalıdır. Bundan sonrası son derece kolaydır. Servis uygulaması çalıştırıldıktan sonra tarayıcı üzerinden GetDailyFoodList operasyonuna yapılan çağrı aşağıdaki örnek ekran görüntüsünde olduğu gibi karşılanacaktır.

![blg98_Runtime.gif](images/blg98_Runtime.gif)

Görüldüğü üzere Wednesday için bir talepte bulunulmuş ve bir içerik elde edilmiştir. Bu örnekte en çok dikkat edilmesi gereken nokta ise operasyon talebinin yapıldığı adres bilgisidir.

https://myfoodcompany.servicebus.windows.net/Foods/GetDailyFoodList?Day=Wednesday Volaaa!!!

![Laughing](images/smiley-laughing.gif)

Parametreyi nasıl gönderdiğimize, https protokolüne göre bir URL bilgisi yazıldığına dikkat edilmelidir.

Tabiki işin sağlamasını yapmak amacıyla servis uygulamasını kapatıp aynı talebi göndermeyi deneyebilirisiniz. Bu durumda aşağıdaki ekran görüntüsü ile karşılaşılacaktır.

![blg98_ServiceNotRunning.gif](images/blg98_ServiceNotRunning.gif)

Yanlız burada dikkat edilmesi gereken noktalardan birisi de, geriye bir istisna (Exception) mesajının gönderilmeyişidir. Bu çok doğaldır nitekim servis uygulaması çalışmıyor olsa bile Service Bus üzerinde gelen talebin değerlendirildiği bir hizmet bulunmaktadır.

Örneği geliştirirken meraktan yaptığım bir testte yanlış Issuer Key değeri göndermek oldu. Böyle bir vakada gönderilen HTTP Get talebi sonrası aşağıdaki ekran görüntüsü ile karşılaşılacaktır.

![blg98_WrongKey.gif](images/blg98_WrongKey.gif)

Görüldüğü üzere 401 Unauthorized hatasını aldık. Bir başka deyişle Credential bilgilerimiz doğrulanmadı.

Bu hızlı örnek ile Service Bus hizmetinden REST bazlı olarakta nasıl yararlanabileceğimizi görmüş olduk. Bir önceki yazımızda kullandığımız örnekte geliştirdiğimiz istemci uygulamada hatırlayacağınız üzere, sözleşme tanımlamış, kanal oluşturmuş ve TCP bazlı olan iletişim üzerinden kanal bazlı metod çağrısı ile isteğimizi Service Bus tarafına göndermiştik. REST modelinde ise bir istemci geliştirmediğimizi sadece bir URL kullandığımızı hatırlatmak isterim. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[RestBased.rar (25,30 kb)](assets/RestBased.rar)
