---
layout: post
title: "WCF Interceptors"
date: 2012-12-02 19:53:00 +0300
categories:
  - wcf-4-5
tags:
  - windows-communication-foundation
  - interceptor
  - parameter-inspector
  - message-inspector
  - custom-behavior
  - iparameterinspector
  - ioperationbehavior
  - attribute
  - iendpointbehavior
  - endpoint-behavior
---
Hepimizin hafızasında yer eden ve defalarca seyretse de asla sıkılmayacağı kült filmler vardır. Hatta nesiller ilerledikçe, her neslin mutlaka en az bir kere uğradığı, uğraması gereken yapımlar vardır.

[![32484720231_large](/assets/images/2012/32484720231_large_thumb.jpg)](/assets/images/2012/32484720231_large.jpg)


The Godfather, Starwars, Matrix, The Good the bad and the ugly, Back to the future vb…Bunlardan birisi de benim için [Mad Max](http://www.imdb.com/title/tt0079501/)’ dir.

Serinin ilginç konusu dışında en çok dikkatimi çeken (pek çok erkeğin de dikkatini çetkiği üzere) filmde kullanılan araçlardır. Bunlardan birisi de o zamanlar yol devriyesi olarak görev yapan V8 motora sahip 1974 model Ford Falcon XB dir. Tabi bunu tahmin edeceğiniz üzere siyah renkli efsanevi Ford GT351 takip etmiştir. [Detaylar için wikipedia’ dan bilgi alabilirsiniz](http://en.wikipedia.org/wiki/Mad_Max#Vehicles)

Peki bizimle ne alakası var

![Who me?](/assets/images/2012/wlEmoticon-whome_2.png)

Bu yazımızda farklı bir Interceptor kavramını göz önüne alıyor olacağız.

WCF (Windows Communication Foundation) alt yapısının popüler olmasının en büyük nedenlerinden birisi de, hemen her seviyede genişletilebilir olmasıdır. Genişleyebilirlik, bir Framework için oldukça önemli bir özelliktir. Nitekim bu yeteneğin olması, geliştiricilerin daha fazla noktada müdahalede bulunabilme ve ihtiyaçları daha fazla yerde çözümleyebilme kabiliyetini kazanabilmesi anlamına gelmektedir.

WCF tarafındaki genişletilebilirlik (extendability) ve hatta yeniden yazabilme yetenekleri, WCF alt yapısı üzerine oturmuş yan ürünlerin de oluşmasına neden olmuştur. Söz gelimi Data Service’ ler veya WCF Web API bu duruma verilebilecek en güzel örneklerdendir. Tabi bunun dışında daha ince ayarlar da yapılabilir. Söz gelimi kendi ServiceHost ortamımızı yazabiliriz veya özel kanal yığını (Channel Stack) oluşturup güvenlik için daha etkin çözümler üretebiliriz. Ya da kendi Binding tipimizi geliştirerek şirketimize ait bir mesajlaşma protokolünün devreye alınmasını sağlayabiliriz.

WCF çalışma mantığı düşünüldüğünde istemci ve servis tarafı (Dispatcher diyelim bu yazımız için) arasındaki iletişim sırasında da araya girebileceğimiz konumlar bulunmaktadır. Sonuçta istemci bir mesajı servis tarafına gönderir ve karşılığında genellikle bir cevap bekler (veya beklemez. OneWay tipindeki operasyonları düşünelim) İşte bu süreçte parametre dahil, mesaj seviyesinde dahi araya girebilir ve istediklerimizi gerçekleştirebiliriz. İşte bu yazımızın konusu da budur. Interceptors

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_144.png)

> Interceptor kelimesi durdurucu, yol kesen, önleme uçağı gibi anlamlara gelmektedir. Lakin ilerleyen kısımlarda göreceğiniz gibi kendi geliştireceğimiz özel kesici tipler Inspector kelimesini içeren arayüzleri (Interface) uygular. Inspector ise kontrolör, denetleyici, denetmen, müfettiş gibi anlamlara gelmektedir. Dolayısıyla kelimeler ve kavramlar birbirleri yerine geçebilirler. Algıda bir karmaşa olmaması adına belirtmek isterim.

Interceptor’ ler bir servis operasyonu öncesinde veya sonrasında devreye girebilmektedirler. WCF tarafında kesmede bulunabileceğimiz 4 yer vardır. Öncelikli olarak bir servis operasyonunun parametrelerini değerlendirebilir ve hatta değiştirebiliriz. Yani bir servis operasyonunun parametresi işlenmeden önce müdahalede bulunma şansına sahibiz.

2nci olarak servis tarafına gelen bir mesajın yakalanması da mümkündür. Bu mesaj ele alınıp yine düzenlenebilir ve bir kontrol sürecine dahil edilebilir. Hatta dönecek olan mesaj içeriğ için de aynı durum söz konusudur. 3ncü olarak bir mesajın formatına müdahalede bulunulabilecek seviyede kesme yapılabilir. 4ncü ve son olarak da çağırılan bir operasyonun kendisi için kesme işlemi uygulanabilir.

Biz bu yazımızda mesaj ve parametre seviyesinde kesme işlemlerinin nasıl uygulanabileceğini incelemeye çalışıyor olacağız. Bu amaçla ilk olarak Visual Studio (2010 veya 2012 olabilir) ortamında basit bir WCF Service Application projesi oluşturalım. Başlangıç için aşağıdaki sınıf diyagramında yer alan tipleri yazabiliriz.

[![wcfi_1](/assets/images/2012/wcfi_1_thumb.png)](/assets/images/2012/wcfi_1.png)

Servis sözleşmesi olan IShipService aşağıdaki gibi yazılabilir.

```csharp
using System.ServiceModel;

namespace SomeServiceApp 
{ 
    [ServiceContract] 
    public interface IShipService 
    { 
        [OperationContract] 
        string SaveWorkItem(WorkItem item);

        [OperationContract] 
        bool ProcessCadImage(byte[] sourceData); 
    } 
}
```

Servis sözleşmesine ait implementasyonu gerçekleştirdiğimiz tip ise şu şekilde yazılabilir.

```csharp
using System;

namespace SomeServiceApp 
{ 
    public class ShipService 
       : IShipService 
    { 
        public string SaveWorkItem(WorkItem item) 
        { 
            return string.Format("{0}[{3}] {1} tarihinde {2} tarafından oluşturuldu" 
                , item.Title 
                ,item.CreatedDate 
                ,item.Owner 
                ,item.Type); 
        }

        public bool ProcessCadImage(byte[] sourceData) 
        { 
            return true; 
        } 
    } 
}
```

Bu basit servis içerisinde iki operasyon uygulanmaktadır. SaveWorkItem aşağıda içeriği verilen WorkItem tipinden bir parametre almakta ve bunu sembolik olarak işlemektedir. ProcessCadImage operasyonu ise byte[] tipinden bir parametre ile çalışmaktadır. Yine bu operasyonun iç yapısı da çok önemli olmadığından varsayılan olarak true döndürecek şekilde geliştirilmiştir.

WorkItem sınıfı içeriği;

```csharp
using System;

namespace SomeServiceApp 
{ 
    public class WorkItem 
    { 
        public string Title { get; set; } 
        public DateTime CreatedDate { get; set; } 
        public string Owner { get; set; } 
        public WorkItemType Type { get; set; } 
    } 
}
```

WorkItemType Enum sabiti;

```csharp
namespace SomeServiceApp 
{ 
    public enum WorkItemType 
    { 
        UserStory, 
        Task, 
        TestCase, 
        Bug 
    } 
}
```

## Parametre Bazlı Kontrolör (Parameter Inspector)

Parametre bazlı kesme tiplerinin kullanılabileceği senaryolardan ilk akla gelen sanıyorumki doğrulama operasyonlarıdır. Söz gelimi örnek servisimizi göz önüne aldığımızda, ProcessCadImage operasyonuna gelen byte[] tipinden dizinin içeriğini kontrol ettirebilir ve gerçekten bir Image olup olmadığını denetleyebiliriz. Ama bu kadar komplike düşünmemize şu aşamada pek gerek yoktur

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_144.png)

Sonuçta basit bir boyut kontrolü de pekala işimizi görecektir. Buna göre ilk yapmamız gereken bir parametre kesicisi yazmaktır. Bunun için System.ServiceModel.Dispatcher isim alanında bulunan IParameterInspector türevli bir tip geliştirmemiz gerekecek. Aynen aşağıda olduğu gibi.

```csharp
using System.ServiceModel; 
using System.ServiceModel.Dispatcher;

namespace SomeServiceApp 
{ 
    public class CADSizeInspector 
        :IParameterInspector 
    { 
        public double DefaultControlSize { get; set; }

        // Operasyon çağrısı tamamlandıktan sonra devreye girecektir. 
        public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState) 
       { 
            
        }

        // Çağırılan operasyon başlatılmadan önce devreye girecektir 
        public object BeforeCall(string operationName, object[] inputs) 
        { 
            // Operasyon adını kontrol ediyoruz 
            if (operationName == "ProcessCadImage") 
            { 
                // Gelen ilk parametre operasyonumuza göre byte[] dizisi. Eğer DefaultControlSize' dan büyükse geriye bir FaultException döndürülmektedir 
                if (((byte[])inputs[0]).Length > DefaultControlSize) 
                    throw new FaultException("CAD çizimine ait mesaj boyutu varsayılandan büyük."); 
            }

            // Herhangibir sıkıntı yoksa null dönerek operasyonun devam etmesini sağlayabiliriz. 
           return null; 
       } 
    } 
}
```

IParameterInspector arayüzü (Interface) ile birlikte gelen iki kritik operasyon vardır. Bunlardan birisi AfterCall diğeri ise BeforeCall’ dur. Geriye bir şey döndürmeyen AfterCall metodu, tahmin edileceği üzere ilgili servis operasyonu işleyişini tamamladıktan sonra devreye girmektedir. Tabiri yerinde ise iş işten geçtikten sonra diyebiliriz

![Smile](/assets/images/2012/wlEmoticon-smile_64.png)

İşin aslı, burada servis operasyon çağrısının tamamlanması sonrası gerçekleştirilecek işlemlere yer verebiliriz. Belki loglama veya performans ölçümleri adına ideal bir yer olabilir.

Geriye object tipinden değer döndüren ve çağrıda bulunulan operasyon adı ile o operasyona gelen parametreler için object tipinden bir diziyi parametre olarak alan BeforeCall metodu ise, kesme işleminin uygulanabileceği en ideal yerdir.

Örnekteki kod parçasında, servis operasyonunun ProcessCadImage olup olmadığı kontrol edilmiş ve bu işlemin ardından gelen ilk parametrenin byte[] tipinden olan karşılığının uzunluğuna bakılmıştır. Söz konusu uzunluk değeri bu sınıfın bir özelliğidir (Property). Dolayısıyla kesme sınıfını ürettiğimiz yerde belirlenebilir. Eğer servis operasyonuna gönderilen byte[] dizisinin boyutu kabul edilebilir olandan çok büyükse ortama bir FaultException fırlatılmaktadır. Çok doğal olarak bu istisna bilgisi istemci tarafına da gidecektir (Eğer includeExceptionDetailInFaults niteliğinin değeri true ise)

> Aslında bu tip boyut kontrolleri için konfigurasyon dosyasında yer alan binding nitelikleri de kullanılabilir. Lakin bu ayarlarda daha çok mesajın boyutu göz önüne alınmaktadır. Bizim senaryomuzda ise dikkat edileceği üzere operasyona gelen bir parametreye özgü olacak şekilde boyut kontrolü gerçekleştirilmektedir.

Peki parametre için gerekli kesme tipini tanımladık. Bunu WCF çalışma zamanı nasıl bilebilir?

![Sarcastic smile](/assets/images/2012/wlEmoticon-sarcasticsmile_11.png)

Söz konusu kesme operasyonu aslında bir servis metoduna ait olduğundan, nitelik bazlı bir davranış (Attribute -ased Behavior) olarak ortama bildirilebilir. Yani, bir Custom Behavior tipi söz konusudur. Bu amaçla aşağıdaki sınıfı geliştirmemiz yeterli olacaktır.

```csharp
using System; 
using System.ServiceModel.Channels; 
using System.ServiceModel.Description; 
using System.ServiceModel.Dispatcher;

namespace SomeServiceApp 
{   
    public class CADSizeOperationBehavior 
        :Attribute,IOperationBehavior 
    { 
        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters) 
        { 
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation) 
        { 
        }

        // Servis tarafındaki dispatch sürecinde araya girilmesi için gerekli tanımlamanın yapıldığı metoddur 
        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation) 
        { 
            // sürece bir Parametre kesici tipi bildirilmektedir 
            dispatchOperation.ParameterInspectors.Add( 
                new CADSizeInspector 
                { 
                    DefaultControlSize = 1024 // Bu kontrol değeri ve benzeri özellikler Behavior tipi içerisinde tanımlanıp geçirilebilir de 
                } 
                ); 
        }

        public void Validate(OperationDescription operationDescription) 
        { 
        } 
    } 
}
```

CADSizeOperationBehavior tipi Attribute sınıfından türemekte ve IOperationBehavior arayüzünü (Interface) uygulamaktadır. Bu arayüz içerisinde gelen ve ezilmesi (override) gereken 4 temel metod bulunmaktadır. Senaryoya uygun olacak şekilde bu fonksiyonelliklerden biri veya bir kaçı ele alınabilir.

Bizim senaryomuzda parametre bazlı bir kesicinin, Dispatch işlemi sırasında devreye girmesi beklenmektedir. Bir başka deyişle servis tarafındaki işlem süreci sırasında yürürlüğe girecek bir kesici tipden bahsedilmektedir. Bu sebepten ApplyDispatchBehavior metodu içerisinde bir tanımlama yapılmıştır. Dikkat edileceği üzere DispatchOperation tipinden olan dispatchOperation parametresinin ParameterInspectors koleksiyonuna yeni bir CADSizeInspector sınıf örneği ilave edilmiştir.

Çok doğal olarak tanımlanan bu davranışın servis operasyonunun implemente edildiği metod içinde bildirilmesi gerekecektir. Attribute türevli bir tip geliştirdiğimizden ProcessCadImage operasyonuna bu davranışı aşağıdaki gibi enjekte edebiliriz.

```csharp
// ProccessCad için özel bir çalışma zamanı davranışı belirtilmiştir. 
[CADSizeOperationBehavior] 
public bool ProcessCadImage(byte[] sourceData) 
{ 
    return true; 
}
```

Özetle servis tarafında Parametre bazlı kesici için aşağıdaki sınıf diagramında görülen tiplerin geliştirildiğini ifade edebiliriz.

[![wcfi_2](/assets/images/2012/wcfi_2_thumb.png)](/assets/images/2012/wcfi_2.png)

Artık istemci tarafı için örnek bir uygulama geliştirebilir ve sonuçları irdeleyebiliriz. Basit bir Console uygulaması işimizi görecektir. Servis referansını istemci tarafına ilave ettikten sonra aşağıdaki örnek kod satırlarını yazabiliriz.

```csharp
using ClientApp.ShipServiceReference; 
using System;

namespace ClientApp 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            ShipServiceClient proxy = null; 
            try 
            { 
                proxy = new ShipServiceClient("BasicHttpBinding_IShipService");

                WorkItem wi = new WorkItem 
                { 
                    CreatedDate = DateTime.Now, 
                    Owner = "Burak", 
                    Title = "Login sayfasına ait Web User Control geliştirilmesi", 
                    Type = WorkItemType.Task 
                };

                string swiResult = proxy.SaveWorkItem(wi); 
                Console.WriteLine(swiResult);

                byte[] someImageData = new byte[2048]; 
                bool pcResult = proxy.ProcessCadImage(someImageData); 
                Console.WriteLine(pcResult); 
            } 
            catch (Exception excp) 
            { 
                Console.WriteLine(excp.Message); 
            } 
            finally 
            { 
                if (proxy != null 
                    && proxy.State == System.ServiceModel.CommunicationState.Opened) 
                    proxy.Close(); 
            } 
        } 
    } 
}
```

İstemci tarafı sırasıyla SaveWorkItem ve ProcessCadImage isimli servis operasyonlarını çağırmaktadır. Söz konusu işlemler bir try…catch…finally bloğu içerisinde icra edilmektedir. İlk önce Fault durumunu kontrol etmek istediğimizden, ProcessCadImage metodu için byte[] tipinden olan dizinin parametresi kasıtlı olarak 1024’ ün üstünde tutulmuştur. Buna göre çalışma zamanında aşağıdakine benzer bir sonuçla karşılaşırız.

[![wcfi_3](/assets/images/2012/wcfi_3_thumb.png)](/assets/images/2012/wcfi_3.png)

Görüldüğü gibi Parametre bazlı çalışan kesici tip devreye girmiş ve servis tarafında üretilen Fault mesajı istemci tarafında da ele alınabilmiştir. (Bu noktada AfterCall metoduna hiç uğranılmayacağını da ifade edebiliriz) Tabi tam tersi durumu da test etmemiz yerinde olacaktır. Yani uygun bir boyut gönderdiğimizde true değerini aldığımızı da görmeliyiz

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_144.png)

## Mesaj Bazlı Kesici (Message Interceptor)

Gelelim mesaj bazında kesme işleminin nasıl yapılabileceğine. Mesaj kesme işlemini istemci veya servis tarafında gerçekleştirebiliriz. Eğer servis tarafında (Dispatch kısmı) bir kesici söz konusu ise öncelikli olarak IDispatchMessageInspector arayüzünü implemente edecek bir sınıfın geliştirilmesi gerekmektedir. İstemci taraflı bir kesme operasyonu için de IClientMessageInspector arayüzü uygulanmalıdır. Bazı senaryolarda her iki arayüzün implemente edildiği tek bir hybrid tip de söz konusu olabilir. Bu sayede hem istemci hem de servis tarafı için çalışabilecek bir mesaj kesicisi geliştirilebilir. Biz örnek senaryomuzda mesajın toplandığı servis tarafını göz önüne alarak ilerleyeceğiz. Bu nedenle IDispatchMessageInspector arayüzünü değerlendiriyor olacağız.

Mesaj kesicilerini yazmanın en zorlu yanı gelen bilginin formatına göre bir parser ihtiyacı olmasıdır. Varsayılan ve bildiğimiz üzere mesajın içeriği tamamen XML formatındadır. Ancak WCF Web API gibi alt yapıların sunduğu JSON formatlı içerikler de servis tarafına gelebilir. Dolayısıyla böyle bir durumda JSON tabanlı bir parsing işlemi de işin içerisine girecektir. Aynı durum TCP bazlı çalışan mesajların olduğu durumda daha farklı bir hal alacaktır.

Fazla vakit kaybetmeden hemen bir implementasyona başlayalım dilerseniz. İlk olarak IDispatchMessageInspector arayüzünü uygulayan bir sınıf ile işe başlıyoruz.

```csharp
using System.ServiceModel; 
using System.ServiceModel.Channels; 
using System.ServiceModel.Dispatcher;

namespace SomeServiceApp 
{ 
    public class ShipServiceMessageInspector 
       :IDispatchMessageInspector 
    { 
        // Operasyon talebi servis tarafına ulaştıkan sonra devreye giriyor olacak 
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext) 
       { 
            // Gelen request nesnesinin içeriği ele alınır ;)

            return null; 
       }

        // Cevap istemci tarafına gönderilmeden önce çağırılacak. Tabi eğer istisnai bir durum oluşmamışsa 
        public void BeforeSendReply(ref Message reply, object correlationState) 
       {             
        } 
    } 
}
```

AfterReceiveRequest tahmin edileceği üzere istemciden ilgili operasyon için gelen talep alındığında devreye girmektedir. Burada mesaj içeriği henüz işlenmemiştir. Bir başka deyişle taleb edilen operasyon henüz çalıştırılmamıştır. Dolayısıyla gelen mesaj alınıp, istenirse değiştirilebilir

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_144.png)

(Message tipinden olan request değişkeninin ref ile tanımlandığına dikkat edelim. Dolayısıyla runtime’ da mesajın geldiği yerde bulunan mesaj içeriği buradan değiştirilebilir)

BeforeSendReply metodu ise istemcinin talep ettiği operasyonun işlenmesi sonrası cevap dönülmeden önce devreye giren fonksiyondur. Burada da dikkat edileceği üzere dönüş için kullanılan mesaj ele alınabilir ve istenirse değiştirilebilir.

Örneğimizde her iki operasyon içeriği de boş bırakılmıştır. Siz, söz konusu implementasyonu yaparken bir senaryo üzerinden gidebilirsiniz. Örneğin gelen mesaj içeriğinde yer alan Türkçe karakterleri ayrıştırma veya dönüş sırasında üretilen mesajı kendi geliştirdiğiniz ve şirketinize özel algoritmaya göre şifreleyerek göndermek gibi

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_144.png)

Mesaj tabanlı kesici tip geliştirildikten sonra, parametre kesicilerindekine benzer olarak, çalışma zamanına bir bildirimde bulunmamız gerekecektir. Bu sefer bu bildirimi konfigurasyon bazlı olacak şekilde gerçekleştireceğiz. Bu amaçla bir Endpoint davranışı belirleyip bunu konfigurasyon dosyasında değerlendirilebilecek şekilde ele almamız yeterlidir.

> Ancak istenirse sözleşme bazlı bir davranış da bildirilebilir. Bunun için de IContractBehavior arayüzünden yararlanılmalıdır.

İlk olarak aşağıdaki tipi geliştirelim.

```csharp
using System.ServiceModel.Channels; 
using System.ServiceModel.Description; 
using System.ServiceModel.Dispatcher;

namespace SomeServiceApp 
{ 
    public class MessageInspectorEndpointBehavior 
        :IEndpointBehavior 
    { 
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) 
        { 
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime) 
        { 
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) 
        { 
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new ShipServiceMessageInspector()); 
        }

        public void Validate(ServiceEndpoint endpoint) 
        { 
        } 
    } 
}
```

IEndpointBehavior arayüzünü uygulayan MessageInspectorEndpointBehavior tipi içerisinde ApplyDispatchBehavior metodunun işlendiği görülmektedir. Bu metod içerisinde WCF çalışma zamanında bir Endpoint davranışının bildirilmesi işlemi icra edilmektedir. Buna göre herhangibir Endpoint için ShipServiceMessageInspector isimli bir davranış tipi belirlenebilecektir. İkinci olarak söz konusu davranışın konfigurasyon dosyasında ele alınabilmesini sağlamak adına bir Element tipi geliştirilmelidir. Aynen aşağıda görüldüğü gibi.

```csharp
using System; 
using System.ServiceModel.Configuration;

namespace SomeServiceApp 
{ 
    public class MessageInspectorExtensionElement 
        :BehaviorExtensionElement 
    { 
        public override Type BehaviorType 
       { 
            get { return typeof(MessageInspectorEndpointBehavior); } 
        }

        protected override object CreateBehavior() 
        { 
           return new MessageInspectorEndpointBehavior(); 
        } 
    } 
}
```

Buraya kadar geliştirdiğimiz tiplerin özeti aşağıdaki sınıf çizelgesinde (Class Diagram) olduğu gibidir.

[![wcfi_4](/assets/images/2012/wcfi_4_thumb.png)](/assets/images/2012/wcfi_4.png)

Artık servis uygulamamızı geliştirdiğimiz WCF Serice Application projesindeki web.config dosyasında aşağıdaki gibi özel bir Endpoint davranışı belirleyebiliriz.

```xml
<?xml version="1.0" encoding="utf-8" ?> 
<configuration> 
    <system.serviceModel> 
      <services> 
        <service name="SomeServiceApp.ShipService"> 
          <endpoint address="" binding="basicHttpBinding" contract="SomeServiceApp.IShipService" behaviorConfiguration="ShipServiceEndpointBehavior" /> 
        <endpoint address="mex" binding="mexHttpBinding" contract="IMetadataExchange" /> 
        </service>          
      </services> 
        <behaviors> 
            <serviceBehaviors> 
                <behavior name=""> 
                    <serviceMetadata httpGetEnabled="true"/> 
                    <serviceDebug includeExceptionDetailInFaults="true" /> 
                </behavior> 
            </serviceBehaviors> 
        <endpointBehaviors> 
        <behavior name="ShipServiceEndpointBehavior"> 
          <messageInspector/> 
        </behavior> 
      </endpointBehaviors> 
     </behaviors> 
    <extensions> 
      <behaviorExtensions> 
        <add name="messageInspector" type="SomeServiceApp.MessageInspectorExtensionElement, SomeServiceApp"/> 
      </behaviorExtensions> 
    </extensions> 
    </system.serviceModel> 
  <system.web> 
    <compilation debug="true"/> 
  </system.web> 
</configuration>
```

İlk olarak extensions elementi altında, MessageInspectorExtensionElement tipi bildirilmiştir. Ancak bu bildirimden sonra messageInspector adıyla bir elementin kullanılması ve geçerli olması söz konusu olabilir. Bu tanımlamanın ardından görüldüğü üzere ShipServiceEndpointBehavior isimli bir behavior bildirilmiş ve içerisinde messageInspector alt elementi kullanılmıştır. Son olarak bu davranışın Endpoint için bildiriminin yapılması yeterli olmuştur.

Yapılan bu ilavelerin ardından istemci tarafındaki proxy tipinin de güncelleştirilmesi gerekir. Eğer istemci ve servis tarafı Debug edilerek ilerlenirse mesaj bazlı kesici içerisinde yer alan AfterReceiveRequest ve BeforeSendReply operasyonlarında aşağıdaki mesaj içeriklerinin hareket ettiği gözlemlenebilir.

İlk çağırılan SaveWorkItem operasyonunda AfterReceiveRequest’ e gelen mesaj içeriği,

```xml
<s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/"> 
  <s:Header> 
    <To s:mustUnderstand="1" xmlns="http://schemas.microsoft.com/ws/2005/05/addressing/none"> 
http://localhost:56119/ShipService.svc</To> 
    <Action s:mustUnderstand="1" xmlns="http://schemas.microsoft.com/ws/2005/05/addressing/none"> 
http://tempuri.org/IShipService/SaveWorkItem</Action> 
  </s:Header> 
  <s:Body> 
    <SaveWorkItem xmlns="http://tempuri.org/"> 
      <item xmlns:a="http://schemas.datacontract.org/2004/07/SomeServiceApp" xmlns:i="http://www.w3.org/2001/XMLSchema-instance"> 
        <a:CreatedDate>2012-10-30T13:21:34.2638133+02:00</a:CreatedDate> 
        <a:Owner>Burak</a:Owner> 
        <a:Title>Login sayfasına ait Web User Control geliştirilmesi</a:Title> 
        <a:Type>Task</a:Type> 
      </item> 
    </SaveWorkItem> 
  </s:Body> 
</s:Envelope>
```

İlk çağırılan SaveWorkItem operasyonunda dönüşe geçildikten sonra, BeforeSendReply metodunda ele alınan mesaj içeriği,

```xml
<s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/"> 
  <s:Header> 
    <Action s:mustUnderstand="1" xmlns="http://schemas.microsoft.com/ws/2005/05/addressing/none"> 
http://tempuri.org/IShipService/SaveWorkItemResponse</Action> 
  </s:Header> 
  <s:Body> 
    <SaveWorkItemResponse xmlns="http://tempuri.org/"> 
      <SaveWorkItemResult>Login sayfasına ait Web User Control geliştirilmesi[Task] 10/30/2012 1:29:39 PM tarihinde Burak tarafından oluşturuldu</SaveWorkItemResult> 
    </SaveWorkItemResponse> 
  </s:Body> 
</s:Envelope>
```

İkinci olarak yapılan ProcessCadImage operasyon için AfterReceiveRequest metoduna gelen mesaj içeriği,

```xml
<s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/"> 
  <s:Header> 
    <To s:mustUnderstand="1" xmlns="http://schemas.microsoft.com/ws/2005/05/addressing/none"> 
http://localhost:56119/ShipService.svc</To> 
    <Action s:mustUnderstand="1" xmlns="http://schemas.microsoft.com/ws/2005/05/addressing/none"> 
http://tempuri.org/IShipService/ProcessCadImage</Action> 
  </s:Header> 
  <s:Body> 
    <ProcessCadImage xmlns="http://tempuri.org/"> 
      <sourceData>AAAAAAAA(burada bi sürü A vardı)=</sourceData> 
    </ProcessCadImage> 
  </s:Body> 
</s:Envelope>
```

İkinci olarak yapılan ProcessCadImage operasyonundan dönülürken ele alınan mesaj içeriği,

```csharp
<s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/"> 
  <s:Header> 
    <Action s:mustUnderstand="1" xmlns="http://schemas.microsoft.com/ws/2005/05/addressing/none"> 
http://tempuri.org/IShipService/ProcessCadImageResponse</Action> 
  </s:Header> 
  <s:Body> 
    <ProcessCadImageResponse xmlns="http://tempuri.org/"> 
      <ProcessCadImageResult>true</ProcessCadImageResult> 
    </ProcessCadImageResponse> 
  </s:Body> 
</s:Envelope>
```

Mesaj bazlı kesici tip, Endpoint seviyesinde bir davranış olarak tanımlandığından, bu Endpoint adına gelen ne kadar operasyon var ise her biri için devreye girecektir. Diğer yandan Parametre bazlı kesicimizin bir FaultException üretmesi halinde, BeforeSendReply metodları hatanın üretildiği servis operasyonu için devreye girmiyor olacaktır.

Görüldüğü üzere parametre ve mesaj seviyesinde kesme işlemlerini uygulamak çok zor olmadığı gibi dikkat gerektiren bir işlemler bütününden oluşmaktadır. Bu yazımızda Parametre ve Mesaj bazlı kesici tiplerin WCF tarafında nasıl yazılabileceğini incelemeye çalıştık. Bu kesicilerin bir diğer implementasyonu da WCF Data Service tarafındadır. Bu konuyu da incelemenizi tavsiye ederim. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Bu makale [Formspring](http://www.formspring.me/BurakSenyurt) üzerinden gelen bir soru üzerine hazırlanmıştır. Soruyu soran arkadaşımıza teşekkür etmek isterim]

[HowTo_WCF_Interceptors.zip (86,34 kb)](/assets/files/2012/HowTo_WCF_Interceptors.zip)