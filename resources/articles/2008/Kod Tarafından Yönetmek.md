---
title: "Kod Tarafından Yönetmek"
pubDate: 2008-03-07 12:00:00
categories:
  - WCF
tags:
  - WCF
  - C#
  - XML
  - .NET
  - SOAP
  - JSON
  - Web Service
  - HTTP
  - Authentication
  - JavaScript
  - Transactions
  - Serialization
  - Visual Studio
---

# Kod Tarafından Yönetmek
Değerli Okurlarım Merhabalar,

Windows Communication Foundation ile geliştirilen Servis Yönelimli Uygulama (SOA-Service Oriented Architecture) çözümlerinde konfigurasyon bazlı (Configuration Based) geliştirme süreci oldukça yaygındır. Konfigurasyon dosyaları içerisinde yer alan bilgiler yardımıyla WCF çalışma zamanı (WCF Runtime) ortamı otomatik olarak bazı işlemler gerçekleştirir. Söz gelimi istemci (Client) ve servis (Service) arasında taşınacak olan mesajların çözümlenmesi (Encoding), bunların seçilen bağlayıcı tipin (Binding Type) belirlediği protokole göre aktarılması gibi alt yapı (Infrastructure) hazırlıkları otomatik olarak gerçekleştirilir. Hatta istemcinin servis üzerinden talep ettiği bir operasyon (Service Operation), servis tarafındaki konfigurasyon bilgilerinden yola çıkılarak hazırlanan çalışma zamanı sayesinde, anlamlı bir metod çağrısı haline dönüşür.

Ne varki bazı vakalarda, WCF altyapı hazırlıklarının konfigursayon bazlı olması tercih edilmez. Bunun en büyük nedenlerinden bir taneside, konfigurasyon bilgilerinin XML (eXtensible Markup Lanuage) bazlı açık text dosyalarında duruyor olmasıdır. Buda ilgili dosyanın dışarıdan değiştirilebileceği anlamına gelmektedir. Her ne kadar servis tarafının Host edileceği uygulama administrator (veya bu gruba dahil kullanıcıların) yetkisindede olsa, söz gelimi HTML Metadata Publishing özelliğinin konfigurasyon içerisinde, program koduna girmeye gerek kalmadan, yanlışlıklada olsa değiştirilmesi istenmeyebilir. Çünkü bunun etkisi sonrasında istemcilerin servise ait metadata bilgisini çekememesi gibi durumlar ortaya çıkmaktadır. Bu gibi bazı sebeplerden dolayı, WCF alt yapı hazırlıklarının özellikle servis tarafında iken, kod bazında (Code Based) gerçekleştirilmesi tercih edilebilir. Bu yazıda ağırılıklı olarak kod tarafında gerekli hazırlıkların nasıl yapılabileceği gibi konulara değinilmektedir.

WCF mimarisi içerisinde çok sayıda CLR tipi (Common Language Runtime-Type) yer almaktadır. Bu tiplerin bazıları genişletilebilir ve çalışma zamanında (Runtime) kullanılabilir. Hatta var olan tiplerden yararlanarak özel durumlar için birleşik tipler tasarlanabilir. Söz gelimi var olan bağlayıcı tiplerden (Binding Types) bir kaçının bir arada kullanılacağı özelleştirilmiş bir bağlayıcı tip tasarlamak mümkündür. Burada CustomBinding isimli CLR tipi önemli rol oynamaktadır.

Bu bölümde çok göze batan WCF tipleri ele alınmaya çalışılmaktadır. Ancak herşeyden önce WCF çalışma zamanını kavramakta yarar vardır. Bu nedenle Host uygulamanın (servis tipinin yayınlayan programın) çalışmaya başladığı andan itibaren ilerlemek daha doğrudur. Host uygulama çalıştığında, servis için tanımlanan EndPoint bilgilerinden yararlanılarak birer ChannelListener nesnesi ve bir kanal yığını (Channel Stack) oluşturulur.

> Bilindiği gibi EndPoint içerisinde servisin Address, Binding ve Contract bilgileri yer almaktadır. Bu bilgiler çalışma zamanı ortamının hazırlanmasında önemli değer sahiptir. Bu sebepten dolayı kanal yığını (Channel Stack) hazırlanırken EndPoint içerisindeki bilgilerden yararlanılır.

ChannelListener nesneleri aslında EndPoint noktalarını ilgili kanallara bağlamakla görevlidir. Öyleki, URI (Uniform Resource Identifier) üzerinden bir mesaj geldiğinde, ChannelListener nesne örneği ilgili mesajı kanal yığınının (Channel Stack) en altında yer alan iletişim kanalına (Transport Channel) aktarır. Gelen mesaj içeriğinin iletişim kanalı açısından bir önemi yoktur. Nihayetinde bu bilgi byte tipinden bir akımdır (Stream).

Bununla birlikte iletişim kanalı gelen mesajı aldıktan sonra, çözümleme kanalına (Encoding Channel) iletir. Çözümleme kanalının, gelen mesajlar içerisindeki operasyonel talepleri alıp nesne-metod ilişkisine dönüştürmek gibi önemli bir görevi vardır. Çözümleme kanalı SOAP (Simple Object Access Protocol), plain text, binary data veya MTOM (Message Transmision Optimization Mechanism) gibi formatları kullanmaktadır. (Hatta.Net Framework 3.5 ile gelen yenilikler ile birlikte JSON-JavaScript Object Notation formatının kullanılabilmeside olanaklı hale gelmiştir.)

İletişim (Transport) ve çözümleme (Encoding) kanalları bağlayıcıların (Bindings) olmassa olmaz parçalarıdır. Bir başka deyişle kanal yığını (Channel Stack) içerisinde mutlaka ve mutlaka var olmaları gerekmektedir. Ancak duruma göre bu kanalların arkasına güvenilir oturumların (Reliable Sessions) sağlanması ve Replay saldırılarının engellenmesi, günveliğin sağlanması, transaction yönetiminin gerçekleştirilmesi gibi işlemler için ek kanallarda ilave edilebilir. Bu durumda zaten birden fazla kanalın bir arada ele alındığı özel bir kanal yığının oluşumu söz konusudur.

> İkili Çözümleme Kanalı (Binary Encoding Channel) WCF'e özeldir. Bu nedenle interoperability desteği olmayan senaryolarda daha çok ele alınır. MTOM (Messsage Transmision Optimization Mechanism) çözümleme kanalı, büyük boyutlu verilerin binary formatta ve interoperability'nin önemli olduğu vakalarda ele alınır.
> Bu açıdan bakıldığında, tüm iletişim kanalları varsayılan olarak bazı çözümleyicileri kullanırlar. Söz gelimi HTTP/HTTPS tabanlı kanallar varsayılan olarak text formatlı çözümleme kanallarını ele alırken, TCP binary formatlı çözümleme kanallarını kullanır. Elbette bu noktada geliştiriciler kanal ve çözümleme kanallarını istedikleri gibi özelleştirebilir ve farklı kombinasyonları ister konfigurasyon bazlı, ister kod bazlı olacak şekilde geliştirebilirler.

Şu ana kadar anlatılanlar göz önüne alındığında kanal yığının içerisinde yer alan tiplerden geçilerek tepeye ulaşıldığı görülür. Kanal yığınının en tepe noktasına ulaşılmasının ardından ilgili mesaj bir ChannelDispatcher nesnesi tarafından karşılanır. ChannelDispatcher nesnesi ilgili mesajı alır ve bir EndPointDispatcher nesnesine aktarır.(ChannelDispatcher arkasında mesajın iletilebileceği birden fazla EndPointDispatcher nesnesi olabilir) EndPointDispatcher nesnesinin görevi, gelen mesajın içeriğine göre uygun olan servis metodunun çalıştırılmasıdır. Bununla birlikte EndPointDispatcher nesnesi gelen mesaj içerisinden servis tarafındaki metod için gerekli parametreleride almaktadır. ChannelDispatcher ve EndPointDispatcher nesneleri, ServiceHost nesnesi örneklendiğinde otomatik olarak üretilirler.

İstemcinin servis tarafından talep ettiği operasyon çağrısı kanal yığınından geçerek metod çağrısı haline geldikten sonra, metod üzerinden alınan sonuçlar aynı yolla geriye dönecektir. İstemci, servisten gelen mesajı ele alırken, WCF çalışma zamanının ürettiği ChannelFactory nesnesinden yararlanır. Bununla birlikte istemci tarafında önem arz eden nesnelerden biriside Proxy bileşenidir. Proxy nesnesi ilgili metod çağrılarının request mesajlarına çevrilmesinden ve gelen cevaplarında (Responses) istemci programın anlayacağı kodlara dönüştürülmesinden sorumludur.

WCF çalışma ortamı ile ilişkili önemli noktalardan biriside davranış (Behavior) tanımlamalarıdır. Davranış tipleri sayesinde WCF altyapısı içerisindeki parçaların nasıl yürüyeceği belirlenebilir..Net Framework 3.0 ve 3.5 içerisinde tanımlanmış olan sayısız davranış tipi (Behavior Type) vardır. Bu tipler sayesinde örneğin, transaction içerisindeki operasyonların toplu olarak nasıl gerçekleştirileceği, verinin nasıl ve ne şekilde serileştirileceği, mesajların gönderilmesi yada alınması sırasında kullanılacak olan ehliyetlerin (credentials) ların tayini, servis üzerinde debug yapılıp yapılamayacağı, HTTP üzerinden metadata publishing aktarımına izin verilip verilmeyeceği, exception detaylarının istemci tarafınada fırlatılıp fırlatılmayacağı ve daha pek çok çalışma zamanı davranışı belirlenebilir. Davranışlar, WCF alt yapısı içerisinde yer alan tiplere veya üyelere (Members) uygulanabilirler. Söz gelimi kimi davranışlar sadece metodlara, kimileri bir servisin tamamına, kimileride sözleşmelere (Contracts) uygulanabilirler. Tabi istenirse özelleştirilmiş davranış tipleri (Behavior Type) tasarlanabilir. Davranışlar kod bazında imperative yada konfigurasyon dosyaları içerisinden declerative tarzda kullanılabilir.

> Davranışlar (Behaviors) geliştiriciler tarafından yazılan bir servisin, daha sonra yöneticiler tarafından farklı şekillerde davranabilmesini sağlamak amacıyla geliştirilmiş yapılardır. Hazır olan davranış tipleri sayesinde, alt yapıya (Infrastructure) müdahale etmeye, ek kod yazmaya veya hazırlık yapmaya gerek kalmadan istenen değişimler gerçekleştirilebilmektedir.

Buraya kadar anlatılanlar göz önüne alındığında WCF (Windows Communication Foundation) mimarisinin, servis ve istemci taraflarınıda içeren alt yapısı kabaca aşağıdaki şekilde görüldüğü gibi ele alınabilir.

![mk244_1.gif](images/mk244_1.gif)

Şekildende görüleceği üzere kanal yığının (Channel Stack) en alt noktasında bulunan Transport Channel ve Encoding Channel kanalarının ardından farklı tipte kanallar gelebilmektedir. Daha öncedende bahsedildiği gibi bu kanallar Transaction, Security, Relaibility gibi konularla ilgili olabilir. Burada söz konusu olan kanal yığını (Channel Stack) kombinasyonu kod tarafında CustomBinding tipi yardımıyla yada konfigurasyon bazında customBinding elementi ile karşılanabilmektedir. Ebeltte eklenecek olan kanalların mantıklı ve düzgün bir sırada olmaları şarttır. Burada söz konusu olan sıra için aşağıdaki şekil göz önüne alınmalıdır.

![mk244_2.gif](images/mk244_2.gif)

Bu kadar teorik bilgiden sonra adım adım basit bir örnek üzerinde ilerleyerek kod bazında WCF alt yapı hazırlıklarını nasıl yapılabileceği incelenmeye başlanabilir. Öncelikli olarak bir WCF servis kütüphanesi (WCF Service Library) geliştirilmelidir. Söz konusu servis kütüphanesi içerisinde yer alacak olan tipler (sözleşme-Service Contract ve uygulayıcı sınıf) basit olarak aşağıda görüldüğü gibidir.

![mk244_3.gif](images/mk244_3.gif)

IHesaplamalar arayüzünün ve Hesaplayici sınıfının içerikleri aşağıdaki gibidir.

```csharp
using System;
using System.ServiceModel;

namespace ServisIslemleri
{
    [ServiceContract]
    public interface IHesaplamalar
    {
        [OperationContract]
        double DaireAlan(double r);
    }

    public class Hesaplayici
        : IHesaplamalar
    {
        #region IHesaplamalar Members

        public double DaireAlan(double r)
        {
            return Math.PI * r * r;
        }

        #endregion
    }
}
```

Konunun basit olarak ele alınması amacıyla sözleşme ve uygulayıcı tipler yalın bir şekilde tasarlanmışlardır. Önemli olan kısım servis uygulaması içerisinde programatik olarak yapılacak ayarlamalardır. Bu amaçla Visual Studio 2008 ortamında geliştirilen Console uygulamasında System.ServiceModel.dll ve servis kütüphanesi (WCF Service Library) assembly'larının referanslarının mutlaka var olması gerekmektedir.

![mk244_4.gif](images/mk244_4.gif)

Servis uygulamasının kodları basit olarak aşağıdaki gibi tasarlanabilir.

```csharp
using System;
using System.ServiceModel.Channels; // Kanal tipleri için eklenen isim alanıdır.
using System.ServiceModel;
using ServisIslemleri; 

namespace Sunucu
{
    class Program
    {
        static void Main(string[] args)
        {
            CustomBinding binding = new CustomBinding(); // Özel bir bağlayıcı tip sınıfı örneklenir. Dikkat edileceği üzere bilinen bir Binding tipi oluşturmaktan bir farkı yoktur.

            // İlk örnek olarak Reliable Session için bir Binding tipi oluşturulur ve eklenir.
            ReliableSessionBindingElement rBinding = new ReliableSessionBindingElement(true); // Mesajların gönderildiği sırada iletilmesini sağlamak için yapıcı metoda true değeri verilmiştir. İtenirse Ordered özelliğine true değeri atanaraktanda bu işlem sağlanabilir.
            rBinding.FlowControlEnabled = true;
            rBinding.MaxRetryCount = 3; // Mesajların başarılı şekilde iletimi için maksimum tekrar sayısı belirlenir.
            binding.Elements.Add(rBinding); // Oluşturulan Bindin elementi eklenir.

            SecurityBindingElement sBinding=SecurityBindingElement.CreateSecureConversationBindingElement( SecurityBindingElement.CreateSspiNegotiationBindingElement());
            sBinding.LocalServiceSettings.DetectReplays = true; // Replay ataklarını kontrol et.
            binding.Elements.Add(sBinding); // Oluşturulan SecureBindingElement, CustomBinding nesnesinin ELements koleksiyonuna eklenir. 

            // Çözümleme kanalı için gerekli binding elementi oluşturulur
            TextMessageEncodingBindingElement tBinding = new TextMessageEncodingBindingElement();
            binding.Elements.Add(tBinding); // CustomBinding nesne örneğinin elements koleksiyonuna eklenir.

            // İletişim kanalı için gerekli element oluşturulur
            TcpTransportBindingElement tcpBinding = new TcpTransportBindingElement();
            tcpBinding.TransferMode = System.ServiceModel.TransferMode.Buffered; // Mesajların transfer modu belirlenir
            binding.Elements.Add(tcpBinding); // Elements koleksiyonuna eklenir.

            /* ServiceHost nesnesi oluşturulur. Parametre olarak servis sözleşmesini uygulayan tipin adı verilir. ServiceHost nesnesi örneklendiğinde, kanalların oluşturulmasınıda sağlar. Aynı zamanda talep edilen servis örneklerinin yaşam sürelerini yönetir. Bunları gerçekleştirirken ChannelListener, ChannelDispatcher, EndPointDispatcher tiplerinin otomatik olarak örneklenmesinide sağlar.*/
            ServiceHost host = new ServiceHost(typeof(Hesaplayici));

            // EndPoint eklenir. İlk parametre sözleşme tipi(Contract), ikinci parametre bağlayıcı(Binding Type), son parametre ise adres(Address) bilgisidir.
            host.AddServiceEndpoint("ServisIslemleri.IHesaplamalar", binding, "net.tcp://localhost:45000/HesaplamaServisi");
            host.Open(); // host açılır. Open metodundan sonra ChannelListener gelen talepleri dinlemeye başlar. Gelen mesajları kanal yığınına devreder. ChannelDispatcher nesnesi ise bu mesajları kanal yığınının en üstünden alır ve uygun olan EndPointDispatcher nesnesine devreder.
            Console.WriteLine("Servis dinlemede...\nKapatmak için bir tuşa basınız...");
            Console.ReadLine();
        }
    }
}
```

Burada sadece örnek olması açısından bazı kanallar için gerekli elementler oluşturulmakta ve CustomBinding nesne örneğinin Elements koleksiyonu içerisinde toplanmaktadır. Söz konusu element nesne örneklerinin pek çok özelliği ve metodu burada göz ardı edilmektedir. Amaç programatik olarak özel bir bağlayıcı tipin oluşturulması ve ServiceHost nesne örneği üzerinden kullanılabilmesidir. Söz konusu uygulama çalıştırıldığında aşağıdaki ekran görüntüsü ile karşılaşılır.

![mk244_5.gif](images/mk244_5.gif)

WCF alt yapısını programalamakla ilişkili olarak Microsoft kaynaklarının gösterdiği örneklerde yapılan çalışmalardan biriside istemci ve servis arasındaki mesajların yakalanmasıdır. (Yakalama işlemini takiben mesaj içeriklerinin örneğin herhangibir algoritmaya göre sıkıştırılması, bazı doğrulama süreçlerinden geçirilmesi gibi ek fonksiyonellikler yapılabilmektedir.) Bu işlem için IDispatchMessageInspector arayüzünü (Interface) uygulayan bir sınıfı yazmak yeterlidir.

Daha sonra söz konusu sınıf örneği servis tarafında davranış (Behavior) olarak ele alınmaktadır. Söz konusu davranışın yazılması içinse IServiceBehavior arayüzünü uygulayan bir sınıfın geliştirilmesi ve içerisindeki uygun metodlarda mesaj yakalamak üzere tasarlanan tipin ele alınması gerekmektedir. Davranışın servis seviyesinde uygulanması halinde, gelen tüm mesajların yakalanması söz konusudur. Mesaj yakalama süreci ile ilişkili tipler istemci tarafınada uygulanabilir.

Yazının bu bölümünde mesaj yakalamak için gerekli işlemlerin programatik olarak nasıl gerçekleştirileceği ele alınmaktadır. Bu amaçla servis uygulamasına aşağıdaki sınıf diagramında (Class Diagram) görülen tiplerin eklenmesi yeterlidir.

![mk244_6.gif](images/mk244_6.gif)

MesajYakalayici isimli sınıf IDispacthMessageInspector arayüzünü (Interface) uygulamaktadır. Söz konusu arayüz AfterReceiveRequest ve BeforeSendReply isimli iki metodun uygulanmasını beklemektedir. Tahmin edileceği üzere AfterReceiveRequest metodu ile, istemcinin talep ettiği operasyonun servis tarafında çalıştırılmasından hemen önce mesajın yakalanması mümkün olmaktadır. Hatta istenirse mesaj içeriği burada değiştirilebilir ki bu çok dikkatli bir şekilde ele alınması gereken bir durumdur. BeforeSendReply isimli metod ise, servis tarafından talep edilen metod tamamlandıktan sonra devreye girmektedir. Örnekteki sınıfta sadece ve sadece gelen ve giden mesajlara ait bilgiler ele alınmaya çalışılmaktadır. MesajYakalayici sınıfına ait kod içerisi aşağıdaki gibidir.

```csharp
using System;
// IDispatchMessageInspector arayünüzün yer aldığı isim alanıdır.
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.Xml;
using System.ServiceModel.Description;

namespace Sunucu
{
    class MesajYakalayici
        :IDispatchMessageInspector
    {    
        #region IDispatchMessageInspector Members

        // Servis üzerinden talep edilen metod çalıştırılmadan hemen önce devreye giren metoddur.
        // ilk parametrenin ref olarak tanımlandığına dikkat edelim. Yani gelen mesajda yapılacak olan değişiklikler çalışma ortamını doğrudan etkileyecektir.
        public object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel, System.ServiceModel.InstanceContext instanceContext)
        {
            // System.Runtime.Serialization referansı eklenmediği takdirde MessageId özelliği için derleme zamanı hatası alınmaktadır.
            Console.WriteLine("\n\n");
            Console.WriteLine("*******Request Bilgisi********");
            Console.WriteLine("Message Id : " + request.Headers.MessageId.ToString());
            Console.WriteLine("MessageVersion Addressing : " + request.Headers.MessageVersion.Addressing);
            Console.WriteLine("To : " + request.Headers.To.AbsolutePath);
            Console.WriteLine("Action : " + request.Headers.Action);
            Console.WriteLine("Encoder : "+request.Properties.Encoder);

            MessageBuffer buffer = request.CreateBufferedCopy(Int32.MaxValue);
            request = buffer.CreateMessage();
            Console.WriteLine("Mesaj Body :"+buffer.CreateMessage().GetBody<XmlElement>().InnerXml); 
    
            return null;
        } 
    
        // Servis metodu tamamlandığında devreye giren metoddur.
        public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            Console.WriteLine("\n\n");
            Console.WriteLine("*******Reply Bilgisi********"); 
            Console.WriteLine("Action : " + reply.Headers.Action);
            Console.WriteLine(reply.ToString());
        }
        #endregion
    }
}
```

Yazılan bu mesaj yakalama sınıfının EndPointDispatcher nesnelerinin her birinde uygulanabilmesi için servis tarafında bir davranış belirtilmesi gereklidir. MesajYakalayiciBehavior sınıfının yazılmasının amaçlarından biriside budur. EndPointDispatcher noktlarına mesaj yakalama işlemini üstlenen sınıfların eklenmesi için ApplyDispatcherBehavior metodundan yaralanılmaktadır.

```csharp
using System;
// IDispatchMessageInspector arayünüzün yer aldığı isim alanıdır.
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.Xml;
using System.ServiceModel.Description;

namespace Sunucu
{
    // Mesaj yakalama sınıfının uygulanması için bir davranış tipi (Behavior Type) geliştirilir
    public class MesajYakalayiciBehavior
        : IServiceBehavior // Davranış tipleri IServiceBehavior arayüzünden türerler.
    {
        #region IServiceBehavior Members

        /* Ek bağlayıcı parametrelerin ilave edilebilmesini sağlayan metoddur. Söz konusu dış ortam parametreleri metoda BindingParameterCollection tipinden aktarılır. WCF çalışma zamanı tarafından servisin dinlediği her bir URI için bir kere çağrılır.*/
        public void AddBindingParameters(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {    
        }

        /* ServisHost nesnesine behavior nesnesinin uygulandığı metoddur. Örnekteki mesaj yakalayıcının uygulanacağı yerdir. Servis tarafından kullanılan her bir EndPointDispatcher nesnesi için birer mesaj yakalayıcı bu metod içerisinden eklenebilir. */
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
            /* Aşağıdaki kod parçası sayesinde, EndPointDispatcher' lara ulaşan her mesaj yada işletilen servis metodundan EndPointDispatcher' a dönen her mesaj MesajYakalayici nesne örneği üzerinden     geçmek durumundadır. */
            // ChannelDispatchers içerisindeki her bir ChannelDispatcher nesnesini ele al
            foreach (ChannelDispatcher cd in serviceHostBase.ChannelDispatchers)
            {
                // O anki ChannelDispatcher içerisindeki her bir EndPointDispatcher nesnesini ele al
                foreach (EndpointDispatcher ed in cd.Endpoints)
                {
                    // MesajYakalayici nesne örneğini MessageInspectors koleksiyonuna ekle
                    ed.DispatchRuntime.MessageInspectors.Add(new MesajYakalayici());
                }
            }
        }

        /* Servisin çalışma zamanında gerekli özellikleri sağlayıp sağlamadığının denetlenebileceği yerdir.Şartlara uymayan durumlarda sözleşmenin geri çevrilmesi amacıyla metod içerisinde exception fırlatılması gibi işlemler yapılabilir */
        public void Validate(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {        
        }

        #endregion
    }
}
```

Artık servis tarafında MesajYakalayıcıBehavior isimli davranışın uygulanma işlemi gerçekleştirilebilir. Bunun için host isimli nesne örneği üzerinden Open metodu çağırılmadan hemen önce aşağıdaki kod parçasının yazılması yeterlidir.

```csharp
host.Description.Behaviors.Add(new MesajYakalayiciBehavior());
```

Söz konusu davranışın eklenmesi için ServiceHost nesne örneğinin Description özelliği üzerinden ulaşılan Behaviors özelliğinin Add metodu kullanılmaktadır. Elbette sonuçları görebilmek için istemci tarafından talep (Request) gelmesi gerekmektedir. İstemci uygulama olayın kolay bir şekilde analiz edilebilmesi ve özellikle mesaj yakalayıcının etkilerinin görülebilmesi için basit bir Console projesi olarak geliştirilmektedir. Yazıdakine benzer senaryolarda çoğunlukla svcutil aracından yararlanılarak istemci (Client) için gerekli olan proxy sınıfı (Class) ve config dosyasının üretilmesi yolu tercih edilir. Yada metadata publishing seçeneği aktif bırakılarak istemcilerin servis referanslarını eklemeleri sağlanabilir. Bazen güvenlik nedeniyle metadata publishing seçeneğinin kapatıldığı durumlar söz konusudur. Örnek senaryoda istemci tarafı için gerekli olan proxy sınıfı svcutil aracı yardımıyla örneklenmekte ve otomatik üretilen config dosyası kullanılmaktadır. Bu amaçla svcutil aracından aşağıdaki ekran görüntüsünde olduğu gibi yararlanılması yeterlidir.

![mk244_7.gif](images/mk244_7.gif)

Bu işlemin arkasından üretilen Proxy.cs ve App.config dosyası örnek bir Client uygulamasında ele alınabilir. Üretilen config dosyası otomatik olarak basicHttpBinding bazlı bir bağlayıcı tipi (Binding Type) kullanmaktadır. Oysaki örnekte yer alan servis tarafı CustomBinding tipini ele almaktadır. Bu sebepten istemci tarafındaki config dosyasının içeriği aşağıdaki gibi değiştirilemelidir.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <bindings>
            <customBinding>
                <binding name="HesaplamaServisiBindingConfig">
                    <reliableSession />
                    <security authenticationMode="SecureConversation" requireSignatureConfirmation="false">
                        <localClientSettings/>
                        <secureConversationBootstrap />
                    </security>
                    <textMessageEncoding />
                    <tcpTransport />
                </binding>
            </customBinding>
        </bindings>
        <client>
            <endpoint address="net.tcp://localhost:45000/HesaplamaServisi" binding="customBinding" bindingConfiguration="HesaplamaServisiBindingConfig" contract="IHesaplamalar" name="HesaplamalarClientEndPoint" />
        </client>
    </system.serviceModel>
</configuration>
```

Özel bağlayıcı tiplerin konfigurasyon dosyası içerisinde tanımlanması için bindings elementi altında yer alan customBinding alt elementinden yararlanılır. Config dosyası içerisinde dikkat edileceği üzere servis tarafındaki CustomBinding bildirimlerine uygun olacak şekilde bazı tanımlamalar yapılmaktadır. Buna göre güvenilir oturumlar için relaibleSession elementi, güvenlik için security elementi, text bazlı mesaj çözümlemesi için textMessageEncoding elementi ve son olarak TCP bazlı iletişimi işaret edecek şekilde tcpTransport elementi kullanılmaktadır. Elbetteki bu elementlerin doğru sırada yazılmalarıda oldukça önemlidir. Console uygulamasının kodları ise aşağıdaki gibi geliştirilebilir.

```csharp
using System;

namespace Istemci
{
    class Program
    {
        static void Main(string[] args)
        {
            HesaplamalarClient proxy =new HesaplamalarClient("HesaplamalarClientEndPoint");

            Console.WriteLine("Devam etmek için bir tuşa basınız...");
            Console.ReadLine();
            Console.WriteLine(proxy.DaireAlan(10).ToString());
        }
    }
}
```

Uygulama test edildiğinde servis ve istemci ekranlarında aşağıdaki sonuçların alındığı görülecektir.

![mk244_8.gif](images/mk244_8.gif)

Dikkat edileceği üzere servis tarafındaki mesaj dinleyici sınıf içerisindeki metodlar devreye girerekten, istemciden gelen ve istemciye gönderilen paket içeriklerinin yakalanması ve bunlar hakkında bilgi alınması işlemleri başarılı bir şekilde gerçekleştirilmiştir. Peki istemci açısından bakıldığında aynı işlemler kod tarafından nasıl yapılabilir. Bunun için öncelikli olarak istemci tarafında, servis operasyonlarının bildirimlerini içeren bir arayüz (Interface) tipinin tasarlanmış olması gerekmektedir. Bu tipin aşağıdaki gibi tasarlandığı düşünülsün.

```csharp
using System;
using System.ServiceModel;

namespace Istemci
{
    [ServiceContract]
    public interface IHesaplamalarV2
    {
        [OperationContract(Action = "http://tempuri.org/IHesaplamalar/DaireAlan", ReplyAction = "http://tempuri.org/IHesaplamalar/DaireAlanResponse")]
        double DaireAlan(double r);
    }
}
```

Bu arayüz tipinin görevi çalışma zamanı için gerekli proxy nesnesi üretildiğinde, ilgili nesne referansını taşıyabilmektir. Ayrıca, DaireAlan metoduna ait operasyonel bilgilerin, OperationContract niteliği içerisinde Action ve ReplyAction özellikleri ile nasıl tanımlandığına dikkat edilmelidir. Servis tarafındaki sözleşme içerisinde herhangibir namespace tanımlanmamış olduğundan, varsayılan olarak http://tempuri.org ön ifadesi tüm operasyon bilgilerinin başında yer almaktadır. İstemci tarafındaki kodlarda dikkat edilmesi gereken en önemli nokta CustomBinding tipinin servis tarafındakine uygun olacak şekilde tasarlanmasıdır. Bu amaçlar istemci tarafındaki kodların aşağıdaki gibi revize edilmesi yeterlidir.

```csharp
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Istemci
{
    class Program
    {
        static void Main(string[] args)
        {
            CustomBinding binding = new CustomBinding();

            ReliableSessionBindingElement rBinding = new ReliableSessionBindingElement();
            binding.Elements.Add(rBinding);

            SecurityBindingElement sBinding = SecurityBindingElement.CreateSecureConversationBindingElement( SecurityBindingElement.CreateSspiNegotiationBindingElement());
            binding.Elements.Add(sBinding);

            TextMessageEncodingBindingElement eBinding = new TextMessageEncodingBindingElement();
            binding.Elements.Add(eBinding);

            TcpTransportBindingElement tcpBinding = new TcpTransportBindingElement(); 
            binding.Elements.Add(tcpBinding);

            IHesaplamalarV2 proxy = ChannelFactory<IHesaplamalarV2>.CreateChannel(binding, new EndpointAddress("net.tcp://localhost:45000/HesaplamaServisi"));

            Console.WriteLine("Devam etmek için bir tuşa basınız...");
            Console.ReadLine();
            Console.WriteLine(proxy.DaireAlan(10).ToString());
        }
    }
}
```

İstemcinin ilgili operasyonel çağrıları yapabilmesi için bir proxy nesne örneğine ihityaç vardır. Bu nesne örneğinin üretilmesi için ChannelFactory sınıfının CreateChannel metodundan yararlanılmaktadır. Dikkat edileceği üzere, bu metodun ilk parametresi bağlayıcı tipi ifade etmektedir. Örnekteki bağlayıcı tipi CustomBinding türünden olup servis tarafındaki bildirime uygun olacak kanal bilgilerini içermektedir. Diğer taraftan ikinci parametre ile servisin adres bilgisi işaret edilir. Uygulama bu haliyle çalıştırıldığındada aynı sonuçların elde ediliği görülebilir.

WCF mimarisinde istenirse istemci tarafından servise gönderilecek olan mesajların manuel olarak hazırlanmasıda mümkündür. Bu Web servislerinde SOAP bazlı mesajların içeriklerinin hazırlanıp gönderilmesi ile benzer bir durumdur. Özellikle WCF istemcilerinin, WCF harici servisler ile haberleşmesi gerektiği durumlarda mesajların manuel olarak hazırlanması söz konusu olabilir. Böyle bir durumda istemci tarafında herhangibir proxy nesnesi olmadan operasyon çağrıları yapılabilmektedir. Bu konuya ilerleyen yazılarda değinmeye çalışacağım. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](assets/ProgramatikYaklasim.rar)
