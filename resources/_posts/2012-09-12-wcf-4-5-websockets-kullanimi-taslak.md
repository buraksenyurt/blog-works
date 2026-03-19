---
layout: post
title: "WCF 4.5 WebSockets Kullanımı [Taslak]"
date: 2012-09-12 15:04:00 +0300
description: "Bilindiği üzere HTTP protokolü üzerinden gerçekleştirilmekte olan Request-Response tabanlı çalışma modelinde, istemcilerin göndereceği taleplere karşılık olarak sunucunun vereceği cevaplar söz konusudur. Dolayısıyla istemciler, örneğin borsa hareketliliği gibi anlık değişim gösteren içerikleri elde etmek istediklerinde, çeşitli teknikleri işin içerisine katmak zorundadırlar. Bunun için Polling adı verilen teknik sıklıkla kullanılmaktadır. İstemci belirli periyot aralıklarında sunucudan gerekli veriyi talep eder ve içeriği okur. Polling dışında bir de Streaming tekniği ile verinin çekilmesi sağlanabilir, ancak hangisi olursa olsun istemci ve sunucu arasındaki haberleşme şekli, tek talebe(Request) karşın, tek bir cevap(Response) gelecek şekilde tesis edilir."
categories:
  - wcf-4-5
tags:
  - wcf-4-5
  - csharp
  - xml
  - dotnet
  - aspnet
  - wcf
  - http
  - iis
  - javascript
  - async-await
  - transactions
  - serialization
  - visual-studio
---
# WCF 4.5 WebSockets Kullanımı [Taslak]
[![yoresel1](/assets/images/2012/yoresel1_thumb.jpg)](/assets/images/2012/yoresel1.jpg) Merhaba Arkadaşlar,

Bazen yemek yemek için dışarı çıkar ve daha önceden gitmediğimiz bir yere oturup hiç bakmadığımız tadlara yelken açarız. Bu, bazen çok başarılı sonuçlanır ve bize büyük bir keyif verir. Bazen de yapmış olduğumuz tercihlerimiz için pişmanlık duyarız. Hatta bazı zamanlarda yerken iyi gelen o tadlar, çıkışta büyük sıkıntılara yol açabilir

![Smile](/assets/images/2012/wlEmoticon-smile_53.png)

İşin garip olan ve belki de heyecan verici yanı, sonuçları tam olarak kestiremediğimiz bir deneyimi yaşayacak olmamızdandır. Hatta biz ilk kez denerken, aynı tadları denemiş başkaları var ise onların tavsiyelerine de kulak verir, kimine inanır, kimine inanmayız. İşte bu günkü makalemizin konusu da, hiç tadmadığımız bir yemeğin bilemediğimiz sonuçlarına benzer nitelikte.

Nitekim kodlamayı yapacak bir ortamımız var ama test yapacak bir çevre yok. Nitekim senaryomuz gereği şu anda sadece Windows 8 platformu ve üzerinde yüklü IIS (Internet Information Services) tarafından desteklenen ama.Net Framework 4.5 içerisinde yer aldığı için Windows 7 gibi bir platform üzerinde de geliştirilebilen bir konu söz konusu. Bu noktaya nasıl geldik bir bakalım

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_121.png)

[İzleyen makalede yazılmış olan kodlar test edilememiştir. Çalışma zamanında kuvvetle muhtemel olası hatalar olduğu aşikardır. Yazıyıyı bunu düşünerek yargılamanızı öneririm.]

Web teknolojileri hızla gelişmeye devam etmekte. Bir yıldan fazla bir süredir gündemde olan yeni maceramız ise, WebSockets üzerinden haberleşebilme imkanı. Özellikle HTML 5 standartları arasında da yer alan bu yeni oluşum (ki Google'un Doodle ürünlerinde yer alan pek çok oyunda bu tekniğin kullanımı söz konusudur), HTTP protokolünün özellikle Real-Time veri akışlarındaki kısıtlamalarını, güçlüklerini ortadan kaldırır nitelikte.

Bilindiği üzere HTTP protokolü üzerinden gerçekleştirilmekte olan Request-Response tabanlı çalışma modelinde, istemcilerin göndereceği taleplere karşılık olarak sunucunun vereceği cevaplar söz konusudur. Dolayısıyla istemciler, örneğin borsa hareketliliği gibi anlık değişim gösteren içerikleri elde etmek istediklerinde, çeşitli teknikleri işin içerisine katmak zorundadırlar. Bunun için Polling adı verilen teknik sıklıkla kullanılmaktadır. İstemci belirli periyot aralıklarında sunucudan gerekli veriyi talep eder ve içeriği okur. Polling dışında bir de Streaming tekniği ile verinin çekilmesi sağlanabilir, ancak hangisi olursa olsun istemci ve sunucu arasındaki haberleşme şekli, tek talebe (Request) karşın, tek bir cevap (Response) gelecek şekilde tesis edilir.

Aslında duyulan ihtiyaç özellikle anlık veriler için aynı kanal üzerinden çift yönlü iletişimi sağlayabilmektir. Duplex Channel iletişim olarak da adlandırabileceğimiz bu yapı, WebSockets'ler sayesinde HTTP üzerinden kolayca sağlanabilmektedir.

> WebSockets'i yeni bir protokolden ziyade HTTP protokolü üzerinden kullanılabilen ve Real-Time veri transferleri için aynı kanal üzerinden çift yönlü iletişimi kolaylaştıran bir teknik olarak düşünmeyi daha doğru buluyorum.

WebSockets'in HTTP'nin standart çalışma modeline göre daha önemli avantajları bulunmaktadır. HTTP protokolüne baktığımızda aşağıdaki handikaplara sahip olduğunu görmekteyiz.

- Hızlı bir iletişim olması için tasarlanmamıştır.
- Her bir request'de mesaj başlığı (Message Header) gibi kısımlar veri ile dolduklarından paket hareket eden paket boyutları artmaktadır.
- Özellikle istemci tarafındaki uygulamaya ait verileri güncel tutabilmek için tarayıcı üzerinden sürekli olarak bir talep gönderilmesi gerekir.
- Daha kısıtlı bir Cross Domain desteği bulunmaktadır.
- Bazı vakalarda ve özellike Long Polling ve Streaming tekniklerinin uygulandığı hallerde, Proxy veya Firewall gibi ara katmanlar cevap sürelerinde gecikmelere neden olabilir.
- Diğer yandan Long Polling ve Streaming teknikleri ölçeklenebilir (Scalable) değildir.

WebSockets'in ise bu dezavantajlar karşısında sunduğu önemli avantajlar mevcuttur. Bunları şu şekilde sıralayabiliriz;

- Gerektiği kadar verinin gönderilmesi söz konusudur.
- Bandwith daha efektif olarak kullanılır.
- Cross Domain desteği sağlamaktadır.
- Firewall ve Proxy'ler üzerinden de iletişim sunabilir.
- Şu an için bazı Javascript implementasyonları haricinde Binary veri akışı desteği de sunar.
- TCP tabanlı yük dengeleyicileri (Load Balancer) içerir.

Peki bu güçlü özellikleri hangi hallerde ele almalıyız? Başlarda da belirttiğimiz üzere Real-Time veri transferi gerektiren pek çok senaryoda, WebSockets biçilmiş kaftan olarak görülebilir. Online Oyunlar, finansal uygulamalar, sohbet (Chat) programları, haber akışları vb...Zaten bu, HTML 5 için de ne kadar popüler olduğunun bir göstergesidir.

> Yine de ortada bazı sorunlar mevcut. Yazıyı hazırladığım tarih itibariyle örneğin Asp.Net 4.5 ile olan kullanımında ciddi kısıtlamalar var. Sadece IIS 8 üzerinde host edilen Asp.Net uygulamalarında çalışabilmekte ve bunun için, IIS tarafında WebSockets özelliğinin de etkinleştirilmiş olması gerekmektedir.
> Diğer yandan, tarayıcı uygulamaların bazı versiyonlarının da bu tekniğe tam olarak destek vermediği görülmektedir. IE10, Chrome 13+, Firefox 7, Safari 5+, Opera 11+ şu an için desteklenen tarayıcı versiyonları olarak görünüyor. Ancak bu henüz tam olarak oturmamış HTML 5 protokolünün meyvelerinden birisi. Dolayısıya zaman içerisinde iyice yerleşecek ve bence vazgeçilmez teknolojilerden birisi olacaktır.

Dilerseniz HTTP ve WebSockets üzerinden yapılan iletişimler arasındaki farkı şekilsel olarak da ifade edelim.

[![ws_1](/assets/images/2012/ws_1_thumb.png)](/assets/images/2012/ws_1.png)

Sanırım şimdi biraz daha netleşmiştir. En azından benim kafamda şöyle bir imajı var. Bir Request ile kanalı açarım ve ben tekrardan bir talepte bulunana kadar, sunucu bana mesaj yollamaya ve beni bilgilendirmeye devam eder

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_121.png)

Tabi bizim bu makalemizdeki amacımız WebSockets tekonolojisini derinlemesine incelemekten ziyade bunu.Net Framework 4.5 tarafında nasıl değerlendirebileceğimizi incelemeye çalışmak. Ben Asp.Net 4.5 yerine WCF 4.5 tarafında konuyu irdelemeye çalışmak niyetindeyim. Bu amaçla da basit bir How To çalışması yapmaya gayret ediyor olacağız.

WCF 4.5 ile gelen Binding Type'larından birisi de, NetHttpBinding sınıfıdır. Özellikle, HTTP Request/Response modeli dışında, Duplex Channel iletişimine izin veren WebSockets desteği nedeni ile de ön plana çıkmaktadır. Varsayılan olarak da Binary mesajlaşmaya da izin vermektedir. Dolayısıyla bir WCF servisi ile istemciler arasında Real-Time veri akışını sağlayacak senaryoların gerçekleştirilmesi mümkündür. Dilerseniz konuyu basit bir örnek ile ilerletelim.(Gerçi bu biraz kör uçuşa benziyor ama en azından teoriyi anlamamıza yardımcı olacaktır kanaatindeyim)

İlk olarak bir WCF Service Library projesi oluşturup içerisine aşağıdaki kod parçalarını ekleyerek işe başlayabiliriz.

Sınıf diagramı;

[![ws_3](/assets/images/2012/ws_3_thumb.png)](/assets/images/2012/ws_3.png)

```csharp
using System.ServiceModel; 
using System.Threading.Tasks;

namespace StockServiceLibrary 
{ 
    [ServiceContract(CallbackContract = typeof(IStockTransactionCallback))] 
    public interface IStockTransactionService 
    { 
        [OperationContract(IsOneWay = true)] 
        Task StartSendingTransactions(); 
    }

    [ServiceContract()] 
    public interface IStockTransactionCallback 
    { 
        [OperationContract(IsOneWay = true)] 
        Task SendTransaction(StockTransaction transaction); 
    } 
}
```

Burada görüldüğü gibi bir Callback tekniği söz konusudur. Malum Duplex kanal iletişimde bu tip bir geri bildirim sözleşmesinin de, servisin istemci tarafında tetikleme yapabilmesi için uygulanması gerektiğini biliyoruz. Asıl servis sözleşmesi olan IStockTransactionService geri bildirim sözleşmesi olarak IStockTransactionCallback arayüzünü kullanmaktadır.

Her iki sözleşme de kendi içerisinde tanımladıkları operasyonların asenkron olarak çalıştırılabilmesine olanak tanımaktadır (async anahtar kelimeleri ile imzalandıklarına dikkat edelim) Tabi bu asenkron işleyiş sadece servisin kendi iç çalışmasında söz konusu değildir. Geri bildirim sözleşmelerinin uygulanma kurallarından birisi olarak StartSendingTransaction ve SendTransaction isimli operasyonlar tek yönlü (One Way) çalışacak şekilde işaretlenmiştir.

Servisi implemente ettiğimiz StockTransactionService sınıfının içeriği de aşağıdaki gibidir.

```csharp
using System; 
using System.ServiceModel; 
using System.ServiceModel.Channels; 
using System.Threading.Tasks;

namespace StockServiceLibrary 
{ 
    public class StockTransactionService 
       : IStockTransactionService 
    { 
        static string[] productCodes = { 
                                "PRD1001", "PRD45", "PRD2451", 
                                "PRD2501", "PRD2301", "PRD2001", 
                                "PRD1190", "PRD1007", "PRD1006", 
                                "PRD1004", "PRD1023" };

        public async Task StartSendingTransactions() 
        { 
            var callback = OperationContext.Current.GetCallbackChannel<IStockTransactionCallback>(); 
            var random = new Random(); 
            double price = 29.00;

            while (((IChannel)callback).State == CommunicationState.Opened) 
            { 
               await callback.SendTransaction( 
                    new StockTransaction{ 
                        ProductCode=productCodes[random.Next(0,productCodes.Length)], 
                        TransactionQuantity=random.Next(50,5000) 
                    }); 
                price += random.NextDouble(); 
                await Task.Delay(1000); 
            } 
        } 
    } 
}
```

StartSendingTransactions metodu içerisinde geri bildirim sözleşmesinden yararlanılarak güncel Context elde edilmekte ve istemci ile olan bağlantı açık kaldığı sürece SendTransaction metodundan yararlanılarak rastgele StockTransaction örnekleri hazırlanıp Client uygulamaya gönderilmektedir. İstemci tarafındaki çalışma şeklini daha iyi görebilmek adına buradaki operasyon içerisinde 1 saniyelik bir gecikme işlemi de uygulanmaktadır. Ayrıca, SendTransaction ile Task tipinin static Delay metodlarına yapılan çağrıların await anahtar kelimesi eşliğinde gerçekleştirildiklerine dikkat edilmelidir.

Servis tarafında kullandığımız ve mesajlar içerisinde dolaştırdığımız veri sözleşmesine (Data Contract) ait POCO (Plain Old CLR Object) tipi tasarımı da şu şekildedir.

```csharp
using System.Runtime.Serialization;

namespace StockServiceLibrary 
{ 
    [DataContract] 
    public class StockTransaction 
    { 
        [DataMember] 
        public string ProductCode { get; set; } 
        [DataMember] 
        public int TransactionQuantity { get; set; } 
    } 
}
```

Sadece örnek olması açısından içerisinde ürün kodu ve işleme alınan mitar bilgisi değerlendirilmektedir. Servisin rastgele üreteceği işlemlere ait bilgileri bağlı olan stemciye/istemciler göndereceği beklenmektedir.

Artık servis host uygulamasını yazmaya başlayabiliriz. Örnekte IIS yerine, self-host tekniğini ele almaktayız. Bu nedenle servis kütüphanesini referans eden bir Console uygulamasını aşağıdaki şekilde geliştirebiliriz.

```csharp
using StockServiceLibrary; 
using System; 
using System.ServiceModel;

namespace ServerHostApp 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            ServiceHost host = new ServiceHost(typeof(StockTransactionService)); 
            host.AddServiceEndpoint( 
                typeof(IStockTransactionService) 
               , new NetHttpBinding() 
                , "ws://localhost/StockTransactionService" 
                ); 
            host.Open(); 
            Console.WriteLine("Host durumu {0}. Kapatmak için bir tuşa basınız.",host.State); 
            Console.ReadLine(); 
            host.Close(); 
            Console.WriteLine("Host durumu {0}.", host.State); 
        } 
    } 
}
```

Görüldüğü üzere ServiceHost nesne örneğine eklenen ServiceEndpoint tipi, NetHttpBinding bağlayıcısını kullanmaktadır. Bu nedenle WebSockets desteğini içermektedir.

> Eğer uygulamayı Windows 7 tabanlı bir sistemde çalıştırırsanız host uygulamanın çalışması sırasında aşağıdaki hata mesajını alacağını görebilirsiniz
>
> ![Sad smile](/assets/images/2012/wlEmoticon-sadsmile_15.png)

[![ws_2](/assets/images/2012/ws_2_thumb.png)](/assets/images/2012/ws_2.png)

İstemci tarafını geliştirmek için öncelikli olarak proxy sınıfının üretilmiş olması da gerekmektedir. Örneğimizde Metadata Publishing özelliğini bilinçli olarak etkinleştirmedik. Bu nedenle komut satırından svcutil aracını kullanarak gerekli üretimi gerçekleştirebiliriz. Visual Studio Command Prompt üzerinden bunu aşağıdaki ifadeler ile sağlayabiliriz.

/>svcutil StockServiceLibrary.dll

ve ardından

/>svcutil.wsdl.xsd /out:proxy.cs

Üretilen Proxy.cs dosyasını istemci uygulamaya ekledikten sonra Program sınıfının içeriğini de aşağıdaki şekilde kodladığımızı düşünelim.

```csharp
using StockServiceLibrary; 
using System; 
using System.ServiceModel;

namespace ClientApp 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            Console.WriteLine("İşlemlere başlamak için bir tuşa basınız."); 
            Console.ReadLine();

            var context = new InstanceContext(new CallbackHandler()); 
            var client = new StockTransactionServiceClient(context); 
           client.StartSendingTransactions();

            Console.WriteLine("Çıkmak için bir tuşa basınız."); 
            Console.ReadLine(); 
        } 
    }

    class CallbackHandler 
        : IStockTransactionServiceCallback 
    { 
        public void SendTransaction(StockTransaction transaction) 
        { 
            Console.WriteLine("{0} {1}",transaction.ProductCode,transaction.TransactionQuantity.ToString()); 
        } 
    } 
}
```

Tabi istemci tarafında bir de WCF çalışma zamanı için gerekli konfigurasyon içeriğine ihtiyacımız olacaktır. Yine kör uçuş yaparak bu içeriği belirleyelim

![Confused smile](/assets/images/2012/wlEmoticon-confusedsmile_24.png)

```xml
<?xml version="1.0" encoding="utf-8" ?> 
<configuration> 
  <startup> 
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5" /> 
  </startup> 
  <system.serviceModel> 
    <bindings> 
      <netHttpBinding> 
        <binding name="BindingConfig"> 
          <webSocketSettings transportUsage="Always" /> 
       </binding> 
      </netHttpBinding> 
   </bindings> 
    <client> 
      <endpoint address="ws://localhost/StockTransactionService" 
                binding="netHttpBinding" 
                bindingConfiguration="BindingConfig"          
                contract="IStockTransactionService" 
                name="NetHttpBinding_IStockTransactionService" /> 
    </client> 
  </system.serviceModel> 
</configuration>
```

Mutlaka servisin host edildiği ve istemcinin talepte bulunduğu adres dikkatinizi çekmiştir. ws ile başlayan bir adresleme söz konusudur. İstemci tarafı da çok doğal olarak NetHttpBinding bağlayıcı tipini kullanacaktır. İnce bir ayar olarak da transportUsage niteliği Always olarak set edilmiştir.

Bakalım çalışma zamanında nasıl sonuçlar elde edeceğiz? Aslında nasıl sonuçlar elde edeceksiniz demem gerekiyordu. Nitekim kodu Windows 7 platformunda yazmak zorunda kaldığım için zaten platform desteği olmadığından deneyemedim

![Sad smile](/assets/images/2012/wlEmoticon-sadsmile_15.png)

Bunu deneme şansını ilerleyen zamanlarda umarım bulurum ki o vakit sonuçları ve koddaki hataları da düzelterek size sunarım

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_121.png)

Şimdilik bu haliyle bırakmak zorundayım. Böylece geldik bir makalemizin (Makale adayımızın) daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

Not: Bu arada.Net 4.5 deki WebSockets ler için Windows 8 dışında bir platform desteği olmayacağı söylenmekte. [http://forums.asp.net/t/1732788.aspx/1](http://forums.asp.net/t/1732788.aspx/1) adresindeki tartışma oldukça taze ve bu konu üzerine yapılmış. Tabi burada dikkat edilecek olursa Kaazing WebSockets Gateway aracından yararlanılabileceği söylenmekte. Ben şu an için bunu denemedim ama siz deneyebilirsiniz ![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_121.png)

[HowTo_WCFandWebSockets_.zip (107,55 kb)](/assets/files/2012/HowTo_WCFandWebSockets_.zip) [Her ihtimale karşın oynayabilmeniz için örneği ilave ettim]