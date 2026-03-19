---
title: "Microsoft.Net Services - Service Bus için Hello World"
pubDate: 2009-11-11 00:45:00
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
  - Authentication
  - Visual Studio
---

# Microsoft.Net Services - Service Bus için Hello World
![blg97_Giris.jpg](images/blg97_Giris.jpg)

Merhaba Arkadaşlar,

Bu gün itibairyle İstanbul'da sağnak yağışlı bir hava hakim. Oysaki bir kaç güne kadar güneşli ve ılıman bir hava vardı. Hal böyle olunca Cloud Computing ile ilgili bir şeyler karıştırmanın tam vaktidir diye düşündüm. Daha önceki [Windows Azure Service Platformu Hakkında İlk İzlenimler](https://www.buraksenyurt.com/post/Windows-Azure-Service-Platform-What-Is)başlıklı yazımızda [Amazon'](http://www.amazon.com)dan Cloud Computing with Windows Azure Platform isimli bir kitabı sipariş ettiğimi ve önümüzdeki dönemlerde bu konu ile ilişkili yazılarımı sizlerle paylaşacağımı belirtmiştim.

Ne var ki aradan geçen uzun süreye ve Amazon'dan kitabı elden teslim eden kurye ile sipariş etmeme rağmen kitap elime bir türlü ulaşmadı

![Undecided](images/smiley-undecided.gif)

ve Amerika'da bir çıkış noktasında takıldı kaldı. Oysaki kütüphanede yer alan sayısız kitap hep sorunsuz ulaşmıştı. Bir kargo firması bir Amazon ile yazışma derken arada gidip gelen sayısız elektronik postanın arkasından en azından paramı geri almaya başardım ama ne varki kitabıma ulaşamadım. Peki yıldım mı? Yılmadım. Bu kez siparişimi [Amazon.co.uk](http://www.amazon.co.uk)sitesinden verdim (Sterlin farklarını hesaba katmanızı öneririm). Kitap siparişin bir sonraki gününde UPS tarafından bana ulaştırıldı.

En azından içim rahatladı ve kitabıma kavuştum.

![Wink](images/smiley-wink.gif)

Gel gelelim şu an okuduğunuz yazıyı yazmak için bu kitabı bir günde bitirmedim elbetteki. Aslına bakarsanız geçtiğimiz günlerde.Net Services SDK'sının Kasım sürümü ve çok doğal olarak dökümantasyonu yayınlandı. İşte bu yazımızda dökümantasyondan edindiğim ilk izlenimler ışığında geliştireceğimiz basit bir Hello World örneği yazmaya çalışıyor olacağız.

Windows Azure platformunun önemli parçalarından birisi olan Microsoft.Net Services, internet tabanlı uygulama servisleri olarak düşünülebilir. Bu anlmada internet tabanlı uygulamaların Cloud üzerinde yer alan uygulamalar veya kaynaklar (Resources) ile iletişimini servis bazlı olarak koordine edebilen bir servis alt yapısı olarak düşünülebilir. Şu aşamada Microsoft.Net Service'lerin iki uygulama biçimi vardır. Service Bus ve Access Control Service. Özellikle Firewall arkasında kalan istemcilerin Cloud üzerinde yer alan bir uygulama ile haberleşmesi sırasında gerekli olan karmaşık konfigurasyon işlemleri, güvenlik gibi konuları Service Bus üzerine alarak kolay bir şekilde çözümlemeye çalışır.

Öyleki Firewall arkasında duran uygulamaların Cloud üzerindeki bir uygulama ile olan haberleşmesinde genellikle port açılması veya VPN kullanılması yolu tercih edilir. Port açılması bir güvenlik riski doğurmakla birlikte, VPN kullanımında da sistem seviyesinde karmaşık konfigurasyon ayarları yapılması gerekmektedir. Oysaki Service Bus bu karmaşıklığı ele alarak istemcilerin Cloud üzerindeki uygulamalar ile konuşabilmesini kolaylaştırmaktadır. Üstelin Authentication gibi işlevlere de sahiptir.

Service Bus çok basit anlamda birbirlerine zayıf bağlı olan (Loosely Coupled) uygulamaların güvenli bir şekilde iletişim kurabilmesini sağlamak, platforma bakılmaksızın gerekli karmaşık konfigurasyon ayarlarını sağlamak amacıyla kullanılmaktadır. İlk etapta Windows Azure platformu üzerinde yer alan Azure ve SQL Service uygulamaları ile olan iletişiminde kullanılabileceği düşünülebilir. Bu açıdan bakıldığında Azure platformu üzerindeki uygulamalar ve veritabanları ile haberleşilirken kullanılan bir servis alt yapısı olarakta görülebilir. Aşağıdaki şekil Service Bus alt yapısını bu açıdan değerlendirmektedir.

![blg97_ArchitectureLast.gif](images/blg97_ArchitectureLast.gif)

Aslında bu şekil bize şunları ifade etmektedir; App A, üzerinde bulunduğu Platform'daki Firewall gibi sorunlara takılmadan Azure Platform'u üzerinde konuşlandırılmış bir Cloud servisine, uygulamasına veya kaynağına (Resource) erişmek için Service Bus alt yapısını kullanmaktadır. Şekilde yer alan App B, Cloud üzerinde yer alan bir uygulama olarak düşünülmektedir. Buna göre App A ile App B'nin haberleşmesinde Service Bus gerekli bağlantıyı, koordinasyonu sağlamakta ve güvenli bir iletişimi tesis etmektedir. Diğer taraftan App A yine Service Bus aracılığıyla başka bir platform üzerinde yer alan App C isimli uygulama ilede güvenli bir iletişim sağlayabilmektedir.

Peki Service Bus alt yapısının en belirgin özellikleri nelerdir?

- Firewall, NAT Gateway gibi unsurların arkasında duran uygulama ve servislerin birbirleriyle haberleşebilmelerini gerekli konfigurasyon ayarlamalarını üstüne aldığı için kolaylaştırır. Geliştiricinin veya tarafların hangi sistem gereksinimlerine ihtiyaçları olduğunu, bulundukları farklı platformlar arasındaki haberleşme sorunlarını düşünmelerine gerek yoktur.
- Standart REST modelini de desteklediğinden kolay bir şekilde kullanılabilir ama istenirse WCF bazlı yaklaşım değerlendirilerek profesyon programcılar tarafından değerlendirilmeside sağlanabilir.
- .Net platformundan olmayan uygulamalara REST ve HTTP tabanlı erişilebilmesini sağlar.
- Cloud üzerindeki servislere Anonymous kullanıcıların erişebilmesi, izin verildiği takdirde mümkündür.
- Servislerin internet üzerinden erişilebilen sabit URL adresleri ile keşfedilmesi (Discovery) lokasyona bakılmaksızın mümkündür.
- Servislere yapılan şüpheli saldırıların bloklanmasına yardımcı olur.

vb...

Bu kısa How To niteliğindeki yazımızda aynı makine üzerinde yer alan bir istemci ile yine aynı makine üzerinde yer alan ve servisi host eden başka bir uygulama arasında Service Bus aracılığıyla Credential bazlı bir iletişimin nasıl kurulabileceği incelenmektedir. Gerçek anlamda Cloud üzerindeki bir uygulama, servis veya kaynak ile haberleşilmese de durumu simule edebileceğimiz bir örnek olacaktır.

Tabiki işin daha çok detayı vardır ancak adet olduğu üzere konuyu kavramının en iyi yolunun çok basit bir örnek geliştirmekle mümkün olabileceği kanısındayım. İşlemlere başlamadan önce [Microsoft.Net Services SDK'sının Kasım 2009](http://www.microsoft.com/downloads/details.aspx?FamilyID=c80ebadf-7eb8-4a62-abcd-0b57fa3855f8&displaylang=en)sürümünü indirmemiz ve sistemimize yüklememiz gerekmektedir. Bu SDK içerisinde.Net Services olanaklarından yararlabilmemiz için gerekli tipler ve üyeleri yer almaktadır. SDK'nın yüklenmesi yeterli değildir. Ayrıca Windows Azure platformu üzerinden bir servis hesabının açılması ve burada kullanacağımız Service Bus için bir Namespace bildirimi yapılması gerekmektedir. Söz konusu hizmetlerden şu an için ücretsiz yararlanılabilmekte olup 2010 içerisinde ücrete tabi olacağına dair bilgiler de yer almaktadır. (Ancak yazıyı hazırladığım bu günlerde Microsoft.Net Service'leri ücretsiz olarak kullanılabilmekteydi.) Kayıt işlemleri için [https://netservices.azure.com/](https://netservices.azure.com/) adresinden yararlanılmaktadır. Örneğin benim Azure üzerinde yer alan projemde bu yazımızdaki örnek için oluşturduğum AlgebraService Namespace ve bilgileri aşağıdaki şekilde görüldüğü gibidir.

![blg97_ServiceNamespace.png](images/blg97_ServiceNamespace.png)

Dilerseniz Namespace detaylarına da bakabilir gerekirse silerek hizmetten kaldırabilirsiniz. Namespace oluşturulduktan sonra istemci ve servis tarafının kullanacağı ehliyet (Credential) bilgilerinin de otomatik olarak oluşturulduğu görülecektir. Service Bus örneğimiz için bunlar Default Issuer Name ve Default Issuer Key değerleridir. Bu bilgiler Service Namespace'ine ait Summary bölgesinde görülebilmektedir.

![blg97_Summary.gif](images/blg97_Summary.gif)

Örneğimizde servis ve istemci uygulamalar aynı makine üzerinde geliştirilecek olsalarda, aralarındaki Credential tabanlı iletişim için yukarıda bilgileri yer alan Service Bus hizmeti kullanılacaktır. Service ve Host uygulamalarımız Visual Studio 2008 ortamında geliştirilmekte olan basit Console uygulamalarıdır ve her ikiside, System.ServiceModel ile.Net Services SDK'sının yüklenmesi sonrasında gelen Microsoft.ServiceBus assembly'larını referans etmektedir.

![blg97_References.gif](images/blg97_References.gif)

Servis tarafındaki kodlarımızı aşağıdaki gibi geliştirebiliriz.

```csharp
using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using Microsoft.ServiceBus;

namespace AlgebraHost
{
    // Servis sözleşmesi
    [ServiceContract(Namespace = "http://algebraservice/ServiceBus/")]
    public interface IAlgebraContract
    {
        [OperationContract]
        double Sum(double x, double y);
    }

    public interface IAlgebraChannel 
        : IAlgebraContract, IClientChannel
    {
    }

    // Sözleşmeyi uygulayan tip
    [ServiceBehavior(Name="Algebra")]
    public class AlgebraService
        : IAlgebraContract
    {
        #region IAlgebraContract Members

        public double Sum(double x, double y)
        {
            return x + y;
        }

        #endregion
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Servis adresi elde edilir
            // İlk parametre schema, ikinci parametre Service Bus üzerinde oluşturulan Solution adı ve üçüncü parametrede servis yoludur
            Uri serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", "AlgebraService", "AlgebraService");

            // İletişim için gerekli olan issuer adı ve şifresi bilgileri TransportClientEndpointBehavior nesne örneğinde toplanır
            TransportClientEndpointBehavior credential = new TransportClientEndpointBehavior();
            credential.CredentialType = TransportClientCredentialType.SharedSecret; // Credential tipi
            credential.Credentials.SharedSecret.IssuerName = "SİZİN ISSUER NAME DEĞERİNİZİ"; // Issuer adı
            credential.Credentials.SharedSecret.IssuerSecret = "SİZİN İÇİN ÜRETİLEN KEY DEĞERİ// Issuer için otomatik üretilmiş olan key değeri

            // ServiceHost nesne örneklenir
            // İlk parametre servis tipidir
            // İkinci parametre ise servis adresidir
            ServiceHost host = new ServiceHost(typeof(AlgebraService), serviceUri);
            IEndpointBehavior serviceRegistrySettings = new ServiceRegistrySettings(DiscoveryType.Public);

            // Config dosyasında tanımlanan tüm Endpoint' lere gerekli Credential davranışı eklenir.
            foreach (ServiceEndpoint endpoint in host.Description.Endpoints)
            {
                endpoint.Behaviors.Add(serviceRegistrySettings);
                endpoint.Behaviors.Add(credential);
            }

            // Servis açılır
            host.Open();

            Console.WriteLine("Servisin orjina adresi \n {0}:\n Service Durumu {1} ",serviceUri,host.State);
            Console.WriteLine("Kapatmak için bir tuşa basınız");
            Console.ReadLine();

            // Servis kapatılır
            host.Close();
        }
    }
}
```

Aslında standart bir WCF Servisi yazılmış ve host edilmiştir. Ancak Service Bus üzerinde tanımlı Service Namespace'in kullanılması içinde bir takım ek işlemler yapılmıştır. Söz gelimi bu iletişim için gerekli Credential bilgileri TransportClientCredentialType tipi yardımıyla servisin üzerindeki tüm endPoint'lere birer çalışma zamanı davranışı (Behavior) olarak bildirilmektedir. Buna göre servis, istemci ile olan tüm iletişiminde Azure üzerindeki projemiz için üretilen Name ve Key değerleri kullanılacaktır.

Diğer yandan servisin adresi belirlenirken, Service Namespace bilgisinin kullanıldığı dikkatten kaçmamalıdır. Nitekim Azure üzerindeki ilgili servisin adresinin bilinmesi gerekmektedir. Bu amaçla sb isimli Service Bus schema adından da CreateServiceUri metodu içerisinde yararlanılmaktadır. Elbette servis tarafında WCF çalışma zamanı (WCF Runtim) için gerekli konfigurasyon ayarlarının da bildirilmesi gerekmektedir. Bu amaçla servis uygulamasına ait App.config dosyası aşağıdaki gibi geliştirilebilir.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <system.serviceModel>
    <services>
      <service name="AlgebraHost.AlgebraService">
        <endpoint contract="AlgebraHost.IAlgebraContract" binding="netTcpRelayBinding"/>
      </service>
    </services>
  </system.serviceModel>
</configuration>
```

Dikkat çekici nokta TCP bazlı iletişim için kullanılan bağlayıcı tiptir (Binding Type).

Gelelim yine Console olarak tasarladığımız istemci uygulama tarafı kodlarına.

```csharp
using System;
using System.ServiceModel;
using Microsoft.ServiceBus;

namespace AlgebraClient
{
    // Sözleşme tipi(Contract Type)
    [ServiceContract(Namespace = "http://algebraservice/ServiceBus/")]
    public interface IAlgebraContract
    {
        [OperationContract]
        double Sum(double x, double y);
    }

    public interface IAlgebraChannel
        : IAlgebraContract, IClientChannel
    {
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Başlamak için bir tuşa basınız");
            Console.ReadLine();
            
            // Bağlantı protokolünün (Http, Tcp) otomatik olarak belirleneceği bildirilir
            ServiceBusEnvironment.SystemConnectivity.Mode = ConnectivityMode.AutoDetect;

            string sbNamespace = "AlgebraService";
            string sbIssuer = "SİZİN ISSUER NAME DEĞERİNİZ";
            string sbIssuerKey = "SİZİN ISSUER KEY DEĞERİNİZ";

            // Servis adresi üretilir
            Uri serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", sbNamespace, "AlgebraService");

            // İletişim için gerekli olan issuer adı ve şifresi TransportClientEndpointBehavior tipinden nesne örneği ile bildirilir.
            TransportClientEndpointBehavior credential = new TransportClientEndpointBehavior();
            credential.CredentialType = TransportClientCredentialType.SharedSecret;
            credential.Credentials.SharedSecret.IssuerName = sbIssuer;
            credential.Credentials.SharedSecret.IssuerSecret = sbIssuerKey;

            // İletişimin kanalını üretecek olan fabrika nesnesi oluşturulur. İlk parametre ile config dosyasındaki Client Endpoint yeri belirtilir, ikinci parametre ilede Service Bus üzerindeki adres bilgisi belirtilir
            ChannelFactory<IAlgebraChannel> channelFactory = new ChannelFactory<IAlgebraChannel>("AlgebraEndpoint", new EndpointAddress(serviceUri));
            // İletişimin hangi kullanıcı adı ve şifre ile gerçekleştirileceği ilgili TransportClientEndpointBehavior nesne örneğinin davranış olarak bildirilmesiyle gerçeklenir
            channelFactory.Endpoint.Behaviors.Add(credential);

            // İletişim kanalı oluşturulur ve açılır
            IAlgebraChannel clientChannel = channelFactory.CreateChannel();
            clientChannel.Open();            
          
            // Sum operasyonuna çağrı yapılır
            Console.WriteLine("Sum Result {0} + {1} = {2} ", 2,4,clientChannel.Sum(2,4).ToString());
          
            // İletişim kanalı ve kanal üretme fabrikası kapatılır
            clientChannel.Close();
            channelFactory.Close();
        }
    }
}
```

İstemci tarafında da dikkat edileceği üzere Servis sözleşmesnin bir kopyası yer almaktadır. Nitekim çalışma zamanındaki proxy üretimi için servis sözleşmesinin sunduğu içeriğin bilinmesi gerekmektedir. Burada devreye ilgili servis sözleşmesini implemente eden kanal arayüz referansıda girmektedir. İstemci tarafı için yapılan adımlar aslında sırasıyla aşağıdaki gibidir;

Bağlantı modu belirlenir (ConnectivityMode)
Host servisin adresi belirlenir (Uri bilgisi belirlenirken Azure Projesi üzerinde oluşturduğumuz Service Namespace adı kullanılır)
Gerekli Crendetial tanımlamaları yapılır ve bağlantı için uygulanması sağlanır (TransportClientEndpointBehavior)
Kanal oluşturulur ve Credential'ı değerlendirmesi davranış (Behavior) eklenmesi yardımıyla belirtilir
Kanal bağlantısı açılır.
Gerekli servis operasyonları icra edilir.(Kobay olarak sıkça kulladığımız Sum operasyonu ![Wink](images/smiley-wink.gif))
Bağlantılar kapatılır.

Çok doğal olarak istemci uygulamanın WCF Çalışma zamanı içinde bir takım konfigırasyon ayarlarının yapılması gerekmektedir. İşte istemci tarafı config dosyası içeriği;

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <system.serviceModel>
    <client>
      <endpoint name="AlgebraEndpoint" contract="AlgebraClient.IAlgebraContract" binding="netTcpRelayBinding"/>
      <!-- Service Bus ile iletişim kurmak için TCP protokolü kullanılacaktır. netTcpRelayBinding bunu belirtmektedir-->
    </client>
  </system.serviceModel>
</configuration>
```

Önce sunucu uygulamamız sonrasında ise istemci uygulamamız çalıştırıldığında aşağıdakine benzer bir ekran görüntüsü ile karşılaşılması muhtemeldir.

![blg97_Runtime.gif](images/blg97_Runtime.gif)

Eğer bu şekilde bir sonuç aldıysak istemci uygulamamızın host edilen servis ile Service Bus üzerinden haberleştiğini düşünebiliriz. Aslında emin olmak için deneme amacıyla oluşturduğumuz Service Namespace'ini kaldırmamız yeterli olacaktır

![Wink](images/smiley-wink.gif)

Lakin bu durumda aşağıdaki sonuç ile karşılaşırız (EndpointNotFoundException)

![blg97_Exception.gif](images/blg97_Exception.gif)

Böylece Azure Service Platformu üzerindeki ilk atılımımızı gerçekleştirmiş olduk arkadaşlar. Umarım birşeyler aktarabilmişimdir. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[AlgebraHost.rar (47,27 kb)](assets/AlgebraHost.rar)
