---
title: "Front-End Service Geliştirmek"
pubDate: 2008-01-30 12:00:00
categories:
  - WCF
tags:
  - WCF
  - C#
  - XML
  - SOAP
  - Web Service
  - HTTP
  - IIS
  - Authentication
  - Performance
  - Transactions
  - Generics
  - Visual Studio
---

# Front-End Service Geliştirmek
Değerli Okurlarım Merhabalar,

Windows Communication Foundation (WCF) mimarisinde belkide en kritik unsurlardan birisi EndPoint kavramıdır. EndPoint, Servis Yönelimli Mimari (Service Oriented Architecture - SOA) uygulamaları geliştirmek için kullanılan WCF modelinde, istemciler (Clients) ile servis (Service) arasındaki haberleşmede yer alan kritik bir parçadır. WCF'in temellerini incelediğimiz daha önceki yazılarımızda, EndPoint kavramının aslında WCF mimarisinin ABC'si olduğundan bahsetmiştik. ABC bilindiği üzere adres (Address), bağlayıcı (Binding) ve sözleşme (Contract) bilgilerinden oluşmaktadır. Buna göre bir EndPoint yardımıyla, servisin istemcilere hangi adresten, hangi protokolle, hangi kurallara göre neyi sunacağı bilgisi aktarılabilir. Bununla birlikte, EndPoint'ler istemci tarafından gelecek olan taleplerin karşılanmasında da büyük öneme sahiptir.

Bir WCF servisi, kendi üzerinde birden fazla EndPoint bilgisi taşıyabilir. Üstelik bu EndPoint'ler aynı sözleşme (Contract) veya farklı sözleşmeler (Contract) içinde tanımlanmış olabilirler. Bu noktada WCF servisi aldığı mesajı hangi EndPoint bileşenine ileteceğinede karar vermektedir. Çok doğal olarak geliştiriciler bu karar aşamasına müdahale edebilir ve EndPoint yönlendirmelerini programlayabilirler. İşte bu da Front-End Service adı verilen WCF servislerinin geliştirilebilmesine neden olmaktadır ki makalemizin konusuda budur. Bazı durumlarda bir WCF servisi, istemcilerden (Clients) gelecek olan mesajları asıl servislere yönlendirmek için kullanılabilir. Aşağıdaki şekilde bu durum analiz edilmeye çalışılmaktadır.

![mk240_1.gif](images/mk240_1.gif)

Bu grafiğe göre istemcilerin talepleri (Requests) Front-End Service tarafından karşılanmakta, sonrasında ise bu talepler (Request) ilgili olan asıl WCF servislerine yönlendirilmektedir. Hatta yönlendirme işlemi Front-End Service'in arkasında duran asıl WCF servisi içerisindeki farklı EndPoint noktalarına doğruda olabilir. Bu tarz bir yönlendirme için pek çok sebep vardır. Özellikle istemcilerin kullanmak istedikleri servislere doğrudan erişemediği durumlar göz önüne alınabilir. Örneğin arka tarafta duran WCF servisleri ile istemciler (Clients) farklı ve birbirlerini göremeye ağlarda (Network) bulunabilirler. Diğer yandan, Front-End Service'lerin bazı avantajlarıda vardır. Örneğin yük dengelemesini (Load Balancing) daha iyi yapabilirler. Bu performans ve dengeli yönetim açısından önemli bir unsurdur.

> Load Balancing modelinde, Front-End Service genellikle istemcilere (Clients) tek bir adres üzerinden hizmet vermektedir. İstemcilerden gelen talepler arka servislere eşit şekilde dağıtılmaktadır. Bir başka deyişle istemciler hep aynı adrese talepte bulunurlarken, Front-End Service arka tarafta duran farklı port numaralarına sahip EndPoint noktalarına yükü eşit şekilde yaymaktadır. Bu sayede arka tarafta daha dengeli çalışan bir servis topluluğu tasarlanması mümkün olmaktadır.Tabi bunun için yazılan Front-End Service içerisinde özel kodlama yapılması gerekmektedir.

Ayrıca, kötü niyetli mesajların sızmasının engellemesi içinde tek bir merkez olarak kullanılabilirler. Elbetteki arka tarafta yönlendirilme yapılacak servis ve EndPoint sayısı çok fazla olabileceğinden Front-End Service yönetimi biraz daha zordur.

> Front-End WCF Service'ler istemciden gelen talepleri (Request) değerlendirerek uygun olan gerçek arka WCF Service'lerine yönlendirirler.

Front-End Service'ler çoğunlukla iki ana kategoride ele alınmaktadır. Adres tabanlı yönlendirme (Address-Based Routing) yapanlar ve içerik tabanlı yönlendirme (Content-Based Routing) yapanlar. Adres tabanlı yönlendirme sisteminde, istemcilerin hangi EndPoint noktalarına talep (Request) gönderdikleri önemlidir. Ancak içerik tabanlı yönlendirme sisteminde istemcilerin talep olarak gönderikleri mesaj içeriklerine bakılır. Mesaj içeriklerinden örneğin kullanıcıya ait kimlik bilgileri (Identity Informations), transaction id değerleri vs... elde edilebilir. Bunların durumuna göre uygun olan WCF servislerine yönlendirme işlevini Front-End Service üstlenir. Örneğin bir servise talepte bulunan kullanıcıların iki farklı profilde olduklarını göz önüne alalım. Aşağıdaki şekilde bu durum gösterilmeye çalışılmaktadır.

![mk240_2.gif](images/mk240_2.gif)

Burada özel kullanıcılar taleplerine hizmet alırlarken, Front-End Service tarafından daha hızlı bir sistem üzerinde konuşlandırılmış bir WCF servisine yönlendirilmektedirler. Diğer taraftan normal kullanıcılar için bu tip bir yönlendirme daha farklı yapılmaktadır. İşte bu tam anlamıyla Front-End Service'e gelen kullanıcıların kimlikleri (Identity) ile alakalıdır ve içerik tabanlı yönlendirme (Content Based Routing) sisteminin bir örneğidir. Nitekim kimlik bilgileri çoğunlukla mesaj içeriklerinden elde edilebilmektedir.

WCF mimarisi içerisinde Front-End Service'lerin nasıl yazılacağını incelemeden önce, bir WCF servisinin gelen mesajları kabaca nasıl ele aldığını anlamakta yarar vardır. Özellikle kod tarafında gelen taleplerin ele alınmasında önemli rol oynayan CLR Tipleri (Common Language Runtime Types) bulunmaktadır. Herşeyden önce WCF çalışma zamanı motoru (WCF Runtime Engine) gelen talepleri değerlendirmek için kanal yığınlarını (Channel Stacks) kullanmaktadır. Bu kanal yığınları ChannelDispatcher ve EndPointDispatcher adı verilen tiplere ait nesne örnekleri ile sıkı bir ilişkidedir. En basit anlamda ChannelDispatcher nesne örnekleri, gelen taleplerin doğru EndPointDispatcher bileşenlerine aktarılmasından sorumludur. Bu anlamda bir ChannelDispatcher nesnesi, birden fazla EndPointDispatcher bileşenini ele alabilmektedir. EndPointDispatcher nesnesleri, gelen mesajları çözümlemek ve servis içerisindeki uygun yerlere iletmekten sorumludur. EndPointDispatcher nesneleri çoğunlukla servis içerisinden talepte bulunulan fonksiyonelliğe ait metod çağrılarını gerçekleştirmektedir. Bu son derece basit ve yüzeysel bir bakış açısıdır. Nitekim bu işlemler sırasında çok sayıda ek yardımcı CLR (Common Language Runtime) nesneside devreye girmektedir. Aslında çalışma zamanındaki durum az çok aşağıdaki şekilde görüldüğü gibidir.

![mk240_3.gif](images/mk240_3.gif)

Görüldüğü gibi istemcilerden EndPoint noktalarına doğru gelen mesajlar kanal yığınları (Channel Stack) üzerinden ChannelDispatcher nesne örneklerine ulaşmaktadır. Sonrasında ise talepler (Requests) uygun olan EndPointDispatcher bileşenleri tarafından ele alınmaktadır. Bu noktada dikkat edilmesi gereken bir husus vardır. Gelen mesaj herhangibir EndPointDispatcher tarafından karşılanmassa ServiceHost nesnesinin UnknownMessageReceived olayı tetiklenir. Çok doğal olarak bu olayın kontrolü geliştirici (Developer) tarafından yapılabilir.

Front-End Service'ler içerisinde, istemciden (Clients) gelen talepler gerçek WCF servislerine yönlendirilmeden önce pek çok işlemde gerçekleştirilebilmektedir. Örneğin kimlik doğrulama yada arka servislere parametre aktarma gibi işlemler söz konusu olabilir. Diğer yandan Front-End Service'lerin tasarlanması sırasında dikkat edilmesi gereken bazı noktalar vardır. Yönlendirici servisin, hedef servis sözleşmelerini uyguluyor olma zorunluluğu nedeni ile yönetilebilirlik zorlaşmaktadır. Buna bağlı olarak serileştirilebilir (Serializable) verilere ait sözleşmelerinde yönlendirici servis tarafından bilinme zorunluluğu söz konusudurki bu da yönetilebilirliği (Management) zorlaştırmaktadır.

Bu kısa bilgilerden sonra adım adım bir Front-End WCF Servisinin nasıl oluşturulacağı incelenmeye başlanabilir. Öncelikli olarak iki adet servis geliştirilecek ve bu servisler birer EndPoint noktası barındıracak şekilde tasarlanacaktır. Bu noktada Front-End Service, gelen talepleri (Request) doğrudan arka taraftaki uygun servis ve EndPoint noktalarına yönlendirmekle görevli olacaktır. Senaryo kabaca aşağıdaki şekilde görüldüğü gibidir.

![mk240_4.gif](images/mk240_4.gif)

Yönlendirme işlemlerini kolay bir şekilde anlayabilmek için basit olarak BasicHttpBinding bağlayıcı tipinden (Binding Type) yararlanılmaktadır. Back-End servisler iki adettir ve her biri farklı port numaraları üzerinden HTTP bazlı olacak şekilde yayınlama yapmaktadır. Her iki Back-End Service uygulamasıda Console olacak şekilde tasarlanmaktadır. Elbetteki gerçek hayat senaryolarında bu servisler IIS (Internet Information Service) üzerinden yada bir Windows Servisi içerisinde gömülü olacak şekildede çalışabilirler. Şu durumda her iki istemcide bu farklı HTTP servislerine talepte bulunmaktadır. Bizim amacımız bir Front-End Service yazmak ve üzerinden Load-Balancing yaparak istemcilerden gelecek olan talep yükünü servislere eşit şekilde dağıtmaya çalışmaktır. Front-End Service geliştirilmeye başlamadan önce ilk olarak servis sözleşmesinin (Service Contract) yer aldığı WCF sınıf kütüphanesini (WCF Service Library) tasarlayarak işe başlanmalıdır. Bu kütüphanenin içeriği konunun anlaşılır olması açısından mümkün olduğu kadar basit tutulmuştur.

![mk240_5.gif](images/mk240_5.gif)

ICebir arayüzüne (Interface) ait kod içeriği aşağıdaki gibidir.

```csharp
[ServiceContract]
public interface ICebir
{
    [OperationContract]
    int Topla(int x, int y);
}
```

Cebir sınıfına (Class) ait kod içeriği ise aşağıdaki gibidir.

```csharp
[ServiceBehavior(InstanceContextMode= InstanceContextMode.PerCall)]
public class Cebir : ICebir
{
    #region ICebir Members

    public int Topla(int x, int y)
    {
        return x + y;
    }

    #endregion
}
```

Bu işlemin ardından Back-End Service'lerin tasarlanmasına başlanabilir. Her iki serviste Console uygulaması üzerinden host edilmektedir. Aralarındaki tek fark yayınlama adreslerindeki port numaralarının farklı olmasıdır. Sembolik olarak bu port numaraları 50001 ve 50002 olarak set edilmektedir. Söz konusu Console uygulamaları çok doğal olarak MatematikLib isimli WCF servis kütüphanesini (WCF Service Library) ve System.ServiceModel.dll assembly'ını referans etmelidir.

![mk240_9.gif](images/mk240_9.gif)

Bununla birlikte konfigurasyon dosyasının (App.config) içeriği aşağıdaki gibidir.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <behaviors>
            <serviceBehaviors>
                <behavior name="CebirServiceBehavior">
                    <serviceDebug includeExceptionDetailInFaults="true" />
                </behavior>
            </serviceBehaviors>
        </behaviors>
        <services>
            <service behaviorConfiguration="CebirServiceBehavior" name="MatematikLib.Cebir">
                <endpoint address="http://localhost:50001/Matematik/Cebir.svc" binding="basicHttpBinding" bindingConfiguration="" name="HttpEndPoint" contract="MatematikLib.ICebir" />
            </service>
        </services>
    </system.serviceModel>
</configuration>
```

EndPoint tanımlamaları BasicHttpBinding bağlayıcı tipini kullanmaktadır. Bununla birlikte her iki serviste aynı WCF sözleşmesini (ICebir) sunmaktadır. Yukarıdaki konfigurasyon dosyasının sahibi olan sunucu uygulama HTTP protokolüne göre 50001 numaralı port üzerinden yayınlama yaparken diğer Console uygulamasıda 50002 numaralı port üzerinden hizmet vermek üzere ayarlanmıştır. Özellikle gerçek hayat vakalarında, aynı servis içerisinde yer alan birden fazla farklı EndPoint tanımlaması da söz konusu olabilir. Yada bu farklı tipteki EndPoint bileşenleri farklı servisler üzerine yayılmış olabilir. Servis uygulamalarına ait kod içerikleri aşağıdaki gibi tasarlanabilir.

```csharp
using System;
using System.ServiceModel;
using MatematikLib;

namespace BackEndService1
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(Cebir));
            host.Open();
            Console.WriteLine("Host dinlemede\nKapatmak için bir tuşa basınız");
            Console.ReadLine();
            host.Close();
        }
    }
}
```

Standart olarak servis uygulaması, Cebir tipi üzerinden bir ServiceHost nesnesi örneklemekte ve Open metodu ile hizmete başlamaktadır. Uygulama kapatılırkende Close metodu yardımıyla hizmet sonlandırılmaktadır. Bu basit servisleri kullanacak olan istemcilerde birer Console uygulaması olarak tasarlanabilirler. Söz konusu istemci uygulamalar için gerekli proxy sınıflarının elbette üretilmesi şarttır. Bu amaçla svcutil aracı kullanılarak aşağıdaki ekran görüntüsünde olduğu gibi istemciler için gerekli proxy sınıfları üretilebilir. (HTTP üzerinden yayınlama yapılmasına rağmen servis uygulamaları IIS üzerinden host edilmediklerinden Visual Studio IDE'si içerisinde Add Service Reference seçeneği kullanılamamaktadır. Bu nedenle svcutil aracından yardım alınmaktadır.)

![mk240_10.gif](images/mk240_10.gif)

Bu işlemlerin ardından ilgili proxy sınıfı ve konfigurasyon dosyası istemci uygulamalara eklenmelidir. İstemci uygulamalar için oluşturuluan output.config dosyasının içeriği biraz daha sadeleştirilerek aşağıdaki hale getirilebilir. Bu örnekte IIS üzerinden bir hosting yapılmadığından konfigurasyon dosyasında yer alan EndPoint elementi içerisine address niteliği (Attribute) atılmayacaktır. Bu nedenle adres bilgisininde açık bir şekilde girilmesi şarttır. (Bununla birlikte Output.config adının App.config olarak değiştirilmesi önerilir)

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <system.serviceModel>
        <client>
            <endpoint binding="basicHttpBinding" address="http://localhost:50001/Matematik/Cebir.svc" contract="ICebir" name="CebirClientEndPoint" />
        </client>
    </system.serviceModel>
</configuration>
```

İstemci uygulamanın kod içeriği aşağıdaki gibi tasarlanabilir.

```csharp
using System;
using System.ServiceModel;

namespace HttpClient1
{
    class Program
    {
        static void Main(string[] args)
        {
            CebirClient client = new CebirClient();
            double toplam=client.Topla(4, 5);
            Console.WriteLine(toplam.ToString());
            Console.WriteLine("Çıkmak için bir tuşa basınız.");
            Console.ReadLine();
        }
    }
}
```

İkinci istemci uygulamada aynı türden olmakla birlikte sadece 50002 numaralı port adresi üzerinden talepte bulunacak şekilde ayarlanmalıdır. Böylece elimizde hazır bir sistem mevcuttur. Artık Front-End Service'in yazılmasına başlanabilir. Front-End Service uygulamasıda aslında bir servis uygulaması olduğundan kendi içerisinden istemcilere bir sözleşme (Contract) sunmak durumundadır. Ne varki burada istemcilerden gelen taleplerin, dengeli bir şekilde arka servislere (Back-End Services) yönlendirilmesi söz konusudur. Ayrıca gelen SOAP paketlerinin ayrıştırılarak uygun olan arka servislere aktarılmasıda gereklidir. Dolayısıyla Front-End Service kendi içerisinde özel bir servis sözleşmesi sunmalıdır. Bu nedenle ilk olarak içeriği aşağıdaki gibi olan bir WCF Servis kütüphanesinin (WCF Service Library) geliştirilmesi gereklidir.

![mk240_11.gif](images/mk240_11.gif)

IRouterContract arayüzüne (interface) ait kod içeriği aşağıdaki gibidir.

```csharp
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace RouterLib
{
    [ServiceContract]
    public interface IRouterContract
    {
        [OperationContract(Action="*",ReplyAction="*")]
        Message MesajIsle(Message msg);
    }
}
```

Arayüz içerisinde dikkat çekici noktalardan birisi OperationContract niteliği (Attribute) içerisinde uygulanan Action ve ReplyAction özellikleridir. Her iki özelliğe değerinin verilmesi ile, talep edilen operasyon istekleri ne olursa olsun işlem yapılacağı belirtilmetkedir. Buradaki operasyon isimleri bilindiği üzere WSDL (Web Service Description Language) dökümanınca belirtilen adlardır. Servis tarafında her ne kadar tek bir fonksiyonellik söz konusu olsada birden fazla işlevin olduğu senaryolarda kabul edilen ve cevaplanan operasyon adlarının belirtilmesi yerine seçeneği sıklıkla kullanılmaktadır.

Bunun yanında arayüz (Interface) içerisindeki metod System.ServiceModel.Channels isim alanında yer alan Message tipinden bir parametre almakta ve aynı tipten bir örnek döndürmektedir. Bu metodun amacı, istemciden gelen mesajların arka servislere yönlendirilmesini sağlamaktır. Ayrıca arka servislerden gelen mesajlarında istemcilere ulaştırılmasında önemli bir role sahiptir. Gerçektende Message sınıfının asli görevi EndPoint noktaları arasındaki iletişimi sağlamaktır. Geliştirilen bu arayüzü uygulayan RouterContract isimli sınıfa ait kod içeriği ise aşağıdaki gibidir.

```csharp
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace RouterLib
{
    [ServiceBehavior(ValidateMustUnderstand=false,InstanceContextMode= InstanceContextMode.PerCall)]
    public class RouterContract : IRouterContract
    {
        private static IChannelFactory<IRequestChannel> fabrika = null;
        private EndpointAddress adres1 = new EndpointAddress("http://localhost:50001/Matematik/Cebir.svc");
        private EndpointAddress adres2 = new EndpointAddress("http://localhost:50002/Matematik/Cebir.svc");
        private static int dengeSayaci = 1;

        static RouterContract()
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            fabrika = binding.BuildChannelFactory<IRequestChannel>();
            fabrika.Open();
        }

        #region IRouterContract Members
    
        public Message MesajIsle(Message msg)
        {
           IRequestChannel kanal = null;
            Message cevap = null;
            try
            {
                if (dengeSayaci % 2 == 0)
                    kanal = fabrika.CreateChannel(adres1);
                else
                    kanal = fabrika.CreateChannel(adres2);

                dengeSayaci++;
                kanal.Open();
                cevap = kanal.Request(msg);
                kanal.Close();
            }
            catch (Exception exp)
            {
            }
            return cevap;
        }
        #endregion
    }
}
```

Öncelikli olarak sınıf içerisinde iki adet EndPointAddress değişkeni tanımlandığına dikkat edelim. Bu değişkenler tahmin edileceği üzere Back-End Service'lere erişim adreslerini taşımaktadırlar. Diğer taraftan yönlendirici serviste PerCall modelinde tasarlanmıştır. Buda istemcilerden gelecek her çağrıda bir servis örneği (Service Instance) oluşturulacağı anlamına gelmektedir. Burada sadece tek bir servis referansı olmasını sağlamak için static yapıcı metod (Static Constructor) içerisinde bazı kodlamalar yapılmaktadır.

MesajIsle metodu içerisinde öncelikli olarak IRequestChannel arayüzüne ait bir değişken tanımlanmaktadır. Bu değişkenin üretilmesi için generic IChannelFactory tipinden olan fabrika isimli değişkenin CreateChannel metodu kullanılmaktadır. Bir başka deyişle istemciden gelen talepler sonrasında dengeSayaci değişkeninin içeriğine göre uygun olan kanal nesnesi (Channel Object) oluşturulmakta ve bu kanal açılarak talebin (Request) aktarılması ve sonucunun alınarak geriye döndürülmesi sağlanmaktadır.

Request metodu, MesajIsle metoduna gelen Message tipinden parametre değerini kullanarak, oluşturulan kanal nesnesine bir talepte bulunmaktadır. Bu talep Front-End Service'inarkasında yer alan Back-End servislerden birisine doğru gerçekleştirilir. Request metodu arka servisten gelen cevabı yine bir Message değişkeni tipinden alarak sonucun elde edilmesini sağlamaktadır. (Servis tarafı ile manuel olarak konuşma tekniklerini ilerleyen makalelerimizde incelemeye çalışacağız). Artık yönlendirici servis tasarlanabilir. Bunun için yine bir Console uygulaması göz önüne alınabilir. Console uygulaması bu kez RouterLib WCF servis kütüphanesini (WCF Service Library) referans etmelidir.

![mk240_12.gif](images/mk240_12.gif)

Söz konusu Front-End Service uygulamasının konfigurasyon içeriği aşağıdaki gibi tasarlanabilir.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <behaviors>
            <serviceBehaviors>
                <behavior name="RouterServiceBehavior">
                    <serviceDebug includeExceptionDetailInFaults="true" />
                </behavior>
            </serviceBehaviors>
        </behaviors>
        <services>
            <service behaviorConfiguration="RouterServiceBehavior" name="RouterLib.RouterContract">
                <endpoint address="http://localhost:50003/Matematik/Cebir.svc" binding="basicHttpBinding" bindingConfiguration="" name="RouterEndPoint" contract="RouterLib.IRouterContract" />
            </service>
        </services>
    </system.serviceModel>
</configuration>
```

Bu konfigurasyon dosyası içerisinde dikkat edilmesi gereken en önemli nokta adres bilgisidir. Dikkat edileceği üzere 50003 numaralı porttan hizmet verecek bir EndPoint tanımlaması yapılmaktadır. Buna göre istemciler 50001 veya 50002 için ayrı ayrı talepte bulunmaktansa, sadece 50003' e istekte bulunacaklardır. Onları karşılayan Front-End Service'te Load Balancing algoritmasına göre arka servisler arasında talepleri dengeli bir şekilde paylaştıracaktır. RouterService isimli Front-End Service uygulamasının kod içeriği ise aşağıdaki gibidir.

```csharp
using System;
using System.ServiceModel;
using RouterLib;

namespace RouterService
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(RouterContract));
            host.Open();
            Console.WriteLine("Yönlendirici servis dinlemede\nÇıkmak için bir tuşa basınız");
            Console.ReadLine();
            host.Close();
        }
    }
}
```

Bu işlemin arkasından tek yapılması gereken istemci uygulamalardaki konfigurasyon dosyalarında yer alan adres bilgisini http://localhost:50003/Matematik/Cebir.svc olacak şekilde değiştirmektir. Test aşamasında önce Back-End Service'lerin çalıştırılması, sonrasında Front-End Service'in yürütülmesi gerekmektedir. Bu servislerin tamamı çalıştığı sürece istemciler hizmet alabilirler. Uygulama eğer debug edilerek çalıştırılırsa MesajIsle metodu içerisinde aşağıdaki Flash görselinde yer alan durumun oluştuğu görülür.

Burada dikkat edileceği üzere istemcilerden gelen iki talep sonrasında, if döngüsü farklı arka servis erişimleri gerçekleştirmektedir. Buda zaten kurulan Load-Balancing algoritmasının bir sonucudur. Sonuç olarak Front-End Service'in eklenmesi ile birlikte sistem aşağıdaki şekilde görülen hale gelmiştir.

![mk240_13.gif](images/mk240_13.gif)

Gerçek hayat senaryolarında Back-End Service içerisinde yer alan EndPoint noktaları farklı bağlayıcı tipleri (Binding Type) kullanıyor olabilirler. Bu durumda yönlendirici servislerin tasarlanması biraz daha zorlaşmaktadır. Örneğin WS standartlarına uygun bağlayıcı tiplerin (örneğin WsHttpBinding gibi) kullanıldığı durumlarda güvenlik (Security) ile ilişkili ayarlamaların mutlaka yapılması gerekmektedir. Buda mesaj (Message) yada iletişim (Transport) güvenliği için ek ayarlamaların hem istemciler hemde servisler üzerinde gerçekleştirilmesi anlamına gelmektedir. Bu makalemizde bu tip konular göz ardı edilerek basit anlamda Load-Balancing yapan bir yönlendirici servisin nasıl tasarlanabileceği incelenmeye çalışılmıştır. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](assets/Yonlendirme.rar)
