---
title: "İlk Bakışta WCF"
pubDate: 2007-02-14 06:00:00
categories:
  - WCF
tags:
  - WCF
  - C#
  - XML
  - Bash
  - .NET
  - Workflow Foundation
  - WPF
  - Web Service
  - XML Web Services
  - HTTP
  - IIS
  - Java
  - Performance
  - Serialization
  - Delegates
  - Generics
  - Visual Studio
---

# İlk Bakışta WCF
Değerli Okurlarım Merhabalar,

Bildiğiniz gibi bir süre önce Microsoft.Net Framework'un 3.0 sürümünün son halini yayınladı. Framework 3.0 beraberinde köklü ve güçlü mimari modeller ile birlikte geldi. Aslında Framework 3.0, Framework 2.0 üzerine gelen yeni eklentiler ile oluşmuştur. Bu eklentiler, Windows Presentation Foundation (WPF), Work Flow Foundation (WF), CardSpace ve Windows Communication Foundation (WCF) dır. Biz bu makalemizde WCF'ı çok yüzeysel olarak anlamaya başlayacak ve konu ile ilişkili basit bir örnek geliştirmeye çalışacağız.

> WCF, hızlı bir şekilde servis yönelimli mimariyi baz alan uygulamalar yazabilmek için geliştirilmiş, birleştirilmiş bir Framework API'si olarak düşünülebilir.

WCF aslında dağıtık mimarinin.Net Framework 3.0 ile gelen yeni hali olarak düşünülebilir. Microsoft, bu güne kadar dağıtık mimari uygulamalarının (Distributed Applications) geliştirilebilmesi için COM+,.Net Remoting, XML Web Servisleri, MSMQ gibi sistemleri geliştirmiştir. WCF, temel olarak bu sistemlerin tamamının yeteneklerini bünyesinde barındıran ve tam SOA (Service Oriented Architecture - Servis Yönelimli Mimari) desteği sağlayan güçlü bir Framework API'si olarak tanımlanabilir. Aslında ilk zamanlarda fonksiyonel programlamadan nesne tabanlı programlamaya (Object Oriented Architecture) geçilmiştir. Sonrasında bu modeli nesnelerin bileşen haline getirilebilmesi sayesinden, bileşen yönelimli mimari (Component Oriented Architecture) izlemiştir. Son olarak da, servis yönelimli mimariyi (SOA) kullanılmaya başlanmıştır. İşte WCF, SOA mimarisine tam ve yüksek seviyede destek veren bir API olarak karşımıza çıkmaktadır.

![mk191_4.gif](images/mk191_4.gif)

Temel olarak WCF, servis yönelimli mimariyi doğrudan desteklemekte ve iki önemli özellik içermektedir. Bunlardan birisi, özellikle Microsoft kanadındaki servislerin, farklı platformlar tarafından ele alınabilmesidir (Interoperability). Böylece, karmaşık.Net tiplerini özel olarak Java, Com gibi modelleri destekleyen çeşitli tipteki platformlara yayabiliriz. Dolayısıyla Linux, Unix vb sistemler servislerimizin birer potansiyel tüketicisi olabilirler.

İkinci önemli özellik ise, windows tarafındaki çeşitli dağıtık mimari modeller arasındaki entegrasyonun tek bir çatı altında toplanabilmesinin sağlanmış olmasıdır (Integration). Bu iki özelliğin yanı sıra WCF, CLR (Comman Language Runtime) tiplerini birer servis olarak sunabilmemizi ve hatta servisleride birer CLR tipiymiş gibi ele alabilmemizi sağlayan bir mimari sağlamaktadır. Aşağıdaki şekil, WCF'ının sağladığı mimari yaklaşımı açıklamaya çalışmaktadır.

![mk191_2.gif](images/mk191_2.gif)

Dikkat ederseniz servise,

- Aynı makine içerisinde aynı süreçte (process) yer alan farklı bir uygulama alanı (application domain) üzerinden,
- Aynı makinede yer alan farklı bir süreç (process) içerisindeki farklı bir uygulama alanı (application domain) üzerinden,
- Farklı bir makinedeki bir süreç (process) içerisinde yer alan farklı bir uygulama alanı (application domain) üzerinden,

erişebiliriz. İstemciler hangi uygulama alanı (Application Domain) içerisinde olurlarsa olsunlar, servis ile olan iletişimlerini bir proxy nesnesi üzerinden sağlamak zorundadır. Bununla birlikte proxy nesneleri üzerinden giden mesajlar servis tarafında bir endPoint üzerinden geçerler. Benzer şekilde servis tarafından istemcilere giden mesajlarda bu endPoint üzerinden çıkarlar. Bu resim aslında bildiğimiz dağıtık mimari modelin WCF tarafından bir görünüşüdür.

İngilizce bazı kaynaklarda, WCF'ının ABC'sinden bahsedilmektedir. Burada ABC aslında Addresses (Adresler), Bindings (Bağlayıcılar) ve Contracts (Sözleşmeler) kelimelerinin baş harfleridir. Bu üçleme, WCF'ının çekirdeğinde yer alan en önemli kavramlardır. Öyleki, dağıtık modele göre servis olarak dış ortama sunulan her bir CLR tipi için bir endPoint tanımlanır. Tanımlanmak zorundadır. Aslında endPoint bir servisin dış ortama sunulan arayüzü (Interface) olarak düşünülebilir. Yani istemcilerin, proxy üzerinden gönderecekleri ve alacakları mesajların servis tarafında karşılandığı nokta olarak düşünülebilir. Bir endPoint içerisinde üç önemli parça vardır.

![mk191_1.gif](images/mk191_1.gif)

Dilerseniz bu kavramları kısaca anlamaya çalışalım.

Servis Adresleri (Service Addresses)

WCF'a göre, hizmette bulunan her servis benzersiz bir adrese sahip olmalıdır. Genellikle bir servis adresi, servisin yeri (service location) ve taşıma protokolü (transport protocol) bilgilerinden oluşur. Aslında servis yerinden kasıt,

- Bilgisayarın adı,
- Site adı,
- Network adı,
- İletişim portu adı,
- Pipe adı,
- Queue adı,
- Belirli bir path bilgisi,
- URI adı

olabilir. Taşıma protokollerimiz ise,

- HTTP,
- TCP,
- P2P (Peer To Peer),
- IPC (Inter-Process Communication),
- MSMQ (Microsoft Message Queuing),

olabilir. Bu bilgiler ışığında örnek servis adresi şablonu aşağıdaki gibi olacaktır.

```text
[taşıma protokolü(transport protocol)]://[makine adı]:[opsiyonel port numarası]/[opsiyonel URI bilgisi]
```

Aşağıda bu desene uygun bir kaç örnek servis adı yer almaktadır. Buradaki desenler özellike.Net Remoting ve Xml Web Servisleri üzerinde geliştirme yapanlarada tanıdık gelecektir.

```text
net.tcp://localhost:4578/MatSrv
net.msmq://localhost:6789/MatSrv
http://localhost:9001/MatSrv
```

Sözleşmeler (Contracts)

Temel olarak bir servisin ne iş yaptığının bilinmesi önemlidir. Bu özellikle, istemcilerin ihtiyaç duyduğu proxy sınıflarının yazılmasında önem arz eden bir konudur. Bu nedenle WCF'da tüm servisler dış ortama bir sözleşme (Contract) sunmaktadırlar. Genel olarak dört sözleşme tipi vardır.

- Servis Sözleşmesi (Service Contract): Servis üzerinden hangi operasyonları gerçekleştirebileceğimizi tanımlayan sözleşme çeşididir.
- Veri Sözleşmesi (Data Contract): Servislerden istemcilere giden ve istemcilerden servise gelen veri tiplerini tanımlayan sözleşme çeşididir. Int gibi bilinen tipler için bu sözleşmeler bilinçsiz (implicit) olarak hazırlanır. Ancak karmaşık tiplerde ve özellikle kendi tiplerimizde açık (explicit) bir şekilde tanımlanmaları gerekir. İşte bu sayede, Java gibi platformlar ile konuşabiliriz. Nitekim onların anlayacağı şekilde bir veri sözleşmesini dış ortama sunma şansımız artık vardır.
- Hata Sözleşmesi (Fault Contract): Servis tarafından hangi hataların fırlatılabileceğini ve bunların istemciye nasıl aktarılacağını tanımlayan sözleşme çeşididir.
- Mesaj Sözleşmesi (Message Contract): Servislerin mesajlar ile etkileşimde bulunmasını sağlayan sözleşme çeşidir.

Genellikle servisler bir sözleşme tanımlamak için ServiceContract ve OperationContract niteliklerini kullanırlar. Daha sonra geliştireceğimiz ilk örnekte bu niteliklere tekrardan değineceğiz. Temel olarak bir tipin servis olarak sunulabileceğini belirtmek için ServiceContract niteliği kullanılır. Servis içerisinde sunulabilecek metodlar ise OperationContract adı verilen nitelikler ile işaretlenirler. Bu aslında WCF'ının başka bir özelliğidir. Nitelik tabanlı (Attribute Based) programlama.

> WCF uygulamalarını geliştirebilmek için gereken temel tipler, Framework 3.0 ile gelen System.ServiceModel.dll, System.IdentityModel.dll, System.Runtime.Serialization.dll vb... Assembly'lar içerisinde yer alırlar. Bu nedenle bu Assembly'ları gerektiğinde kullanabilmek için projelere açıkça referans etmemiz gerekmektedir.

Bağlayıcılar (Bindings)

Bağlayıcılar temel olarak servisler ile nasıl iletişim kurulacağını tanımlamak üzere kullanılırlar. Aslında bir bağlayıcı tip (Binding Type) taşıma tipi (transport type), protokol (protocol) ve veri çözümlemesi (data encoding) bildirir. Bunlar aslında servis yönelimli mimari modelde kullanılabilen senaryolar göz önüne alınarak oluşurlar. Bu sebepten dolayıda WCF, bu önceden bilinen senaryoları kullanabilmek için gerekli bağlayıcı tipleri önceden bildirmiştir. Bu tipler aşağıdaki tabloda yer aldığı gibidir.

Binding Tipi
Konfigurasyon
Elementi
Taşıma Çeşidi
(Transport Type)
Veri Çözümlemesi
(Data Encoding)
Platform Desteği
(Inter operatbility)

BasicHttpBinding

HTTP / HTTPS
Text
Var

NetTcpBinding

TCP
Binary
Yok

NetPeerTcpBinding

P2P
Binary
Yok

NetNamedPipeBinding

IPC
Binary
Yok

WSHttpBinding

HTTP/HTTPS
Text/MTOM
Var

WSFederationBinding

HTTP/HTTPS
Text/MTOM
Var

NetMsmqBinding

MSMQ
Binary
Yok

MsmqIntegrationBinding

MSMQ
Binary
Var

WSDualHttpBinding

HTTP
Text/MTOM
Var

Buradaki tiplerden hangisini seçeceğimiz, geliştireceğimiz SOA (Service Oriented Architecture) modelindeki ihtiyaçlarımız doğrultusunda belirlenebilirler. Dikkat ederseniz her bağlayıcı tipin interoperability desteği bulunmamaktadır. Bazılar daha yüksek performans sağlayacak şekilde Binary veri çözümlemesini ele alır. Ama kimiside IIS gibi ortamlar üzerinden internete açılabilecek protokol desteğini sunar. İşte bu tip kriterlere göre uygun olan bağlayıcı tipler seçilebilir. Elbette istersek buradaki tipler dışından kendi bağlayıcılarımızı da yazma şansına sahibiz. Ancak bahsi geçen tipler hemen hemen her dağıtık uygulama senaryosu göz önüne alınarak tasarlanmıştır.Böylece WCF'ın ABC'sine çok kısada olsa değinmiş olduk.

Gelelim WCF servislerini nerelerde barındırabileceğimize. Sonuç itibariyle yazılan servileslerin mutlaka bir windows süreci (Windows Process) üzerinden sunulması gerekmektedir. Artık temel olarak iki farklı barındırma (Hosting) seçeneğimiz vardır. IIS Hosting ve Self Hosting. IIS Hosting sisteminde, geliştirilen servislerin IIS üzerinde barındırılması amaçlanır. Doğal olarak servisler, web üzerinden hizmet verilebilmektedir. Self Hosting modeli ise kendi içerisinde dörde ayrılmaktadır. Windows Aktivasyon Servisi (Windows Activation Service), Windows Servisi, Konsol uygulaması, Windows Uygulaması.

![mk191_3.gif](images/mk191_3.gif)

Windows Aktivasyon Servisi (Windows Activation Service), Vista ile birlikte gelen bir uygulama çeşididir. Özetle Http desteği olmayan host uygulamalar için IIS benzeri bir işlevselliği sağlamakla yükümlüdür. Diğerleri ise özellike.Net Remoting'den aşina olduğumuz host uygulama tipleridir. Elbette servislerin hizmet verebilmesi için host edilmeleri şarttır. Bu da servisi sunan uygulamanın sürekli çalışır olmasını gerektirir. Nitekim uygulama çalışmadığı takdirde istemcilere hizmet veremez. Buraya kadar anlatıklarımızdan yola çıkacak olursak, WCF cephesinden bir servis yönelimli sistem için şu adımları takip etmemiz yeterli olacaktır.

- Servise ait sözleşmeyi barındıran ve asıl fonksiyonelleri içeren bir assembly geliştirilir.
- Servisi istemcilere sunacak olan bir host uygulama geliştirilir.
- İstemcilerin söz konusu servisi kullanabilmeleri için gerekli olan proxy sınıfı üretilir.
- İstemci uygulama geliştirilir.

Bu adımlar sırasında özellikle servis tarafında ve istemci tarafında gereken bir takım ayarlamalar için konfigurasyon dosyalarından faydalanabilir yada programatik olarak gerekli hazırlıkların yapılmasını sağlayabiliriz.

Dilerseniz basit bir örnek üzerinden hareket ederek örnek bir WCF sistemi geliştirmeye çalışalım. İlk olarak bir Class Library projesi geliştireceğiz. Projemiz içerisinde servis sözleşmesi rolünü üstlenecek bir arayüz (interface) tipi ve bu arayüz tipini uygulayan bir sınıfımız olacak. WCF'da, servis sözleşmlerinin tanımlanması için ServiceContract ve OperationContract niteliklerinin kullanılmasını gerekir. Daha önceden de belirttiğimiz gibi bu nitelikler (attributes) System.ServiceModel isim alanı (namespace) altında yer almaktadır. Bu nedenle ilk olarak projemize bu referansı aşağıdaki gibi eklememiz gerekir.

![mk191_6.gif](images/mk191_6.gif)

> WCF uygulamalarını Visual Studio 2005 üzerinde daha kolay geliştirmek için gerekli extension'ları yüklememiz gerekir. Bu extension'lar yüklendiği takdirde, proje şablonları arasına WCF Service Library seçeneğide gelecektir. Bu proje şablonu, servis sözleşmesinide uygulayan ve ön bilgiler veren hazır bir örnek kütüphane üretmektedir.
> ![mk191_5.gif](images/mk191_5.gif)

Şimdi sınıf kütüphanemizin içerisine aşağıdaki tipleri ekleyelim.

![mk191_7.gif](images/mk191_7.gif)

```csharp
using System;
using System.ServiceModel;

namespace MatematikServisLib
{
    [ServiceContract]
    public interface IMatematikServis
    {
        [OperationContract]
        double Toplam(double x, double y);

        void DahiliMetod();
    }

    public class Matematik : IMatematikServis
    {
        #region IMatematikServis Members

        public double Toplam(double x, double y)
        {
            return x + y;
        }

        public void DahiliMetod()
        {
        }

        #endregion
    }
}
```

Burada test amacıyla DahiliMetod isimli metod için OperationContract niteliği kullanılmamıştır. Bu sebepten dolayı bu metodun bilgisi servis sözleşmesine dahil edilmeyecektir. Bir başka deyişle istemciler bu metodu hiç bir şekilde kullanamayacaktır.

Şimdi sırada host uygulamasının geliştirilmesi var. Şu an için WCF'a merhaba demek istediğimizden, host uygulamasınıda basit bir konsol projesi olarak geliştireceğiz..Net Remoting mimarisinden de hatırlanacağı gibi, genellikle sunucu ve istemci tarafındaki ayarları konfigurasyon bazlı dosyalarda tutmayı tercih ederiz. Bu bize, uygulamayı yeniden derlemeden kanal (channel), port numarası, uzak nesne bilgisi gibi ayarların değiştirilebilmesi ve kullanılabilmesi imkanını sunmaktadır. Aynı felsefeyi WCF uygulamalarında da benimsemekte fayda vardır. Bu nedenle, host uygulamamız için gerekli bazı bilgileri (endPoint gibi) App.config dosyasında tutmayı tercih edeceğiz. Tahmin edeceğiniz gibi konfigurasyon dosyası içerisine WCF'nın ABC'sini koymalıyız. Yani adres (address), bağlayıcı (binding) ve sözleşme (contract) bilgilerini dahil ederek gerekli endPoint tipini tanımlamalıyız. Bu amaçla konsol uygulamamızın konfigurasyon dosyasını aşağıdaki gibi geliştirelim.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <appSettings>
        <add key="adres" value="http://localhost:4590/MatSrv"/>
    </appSettings>
    <system.serviceModel>
        <services>
            <service name="MatematikServisLib.Matematik">
                <endpoint address="http://localhost:4590/MatSrv" binding="basicHttpBinding" contract="MatematikServisLib.IMatematikServis"/> 
            </service>
        </services>
    </system.serviceModel>
</configuration>
```

Dilerseniz konfigurasyon dosyasına yazdıklarımızı inceleyelim. Servisimize ait bir endPoint tanımlamamız gerekmektedir. EndPoint servisin sunulduğu adres bilgisini, bağlayıcı tipini ve servis sözleşmesini ilgili niteliklerden almaktadır. endPoint elementleri, service elementleri içerisinde tanımlanır. Bir service elementi içerisine birden fazla endPoint bilgiside konulabilir. Diğer tarafan service elementleride services elementi tarafından sarmalanmıştır. Dolayısıyla birden fazla service elementininde tanımlanabileceğini söyleyebiliriz.

Peki istemcilere sunmak istediğimiz tipi host uygulama içerisinde servise nasıl sunacağız? Bu amaçla, System.ServiceModel isim alanı altında yer alan ServiceHost sınıfını kullanmamız gerekmektedir. Bu sınıfın temel görevi, parametre olarak aldığı tipi, yine parametre olarak aldığı adres üzerinden istemcilere sunmaktır. Dolayısıyla konsol uygulamamız içerisindeki Main metodunda aşağıdaki kodları yazmamız servisi sunmak için yeterli olacaktır. Tekrardan hatırlatalım; Host uygulamanın konfigurasyon dosyasındaki adres değerine erişebilmesi için System.Configuration.dll, ServiceHost tipini kullanabilmesi için System.ServiceModel.dll, Matematik tipini kullanabilmesi içinde MatematikServisLib.dll assembly'larına referansta bulunması gerekir.

![mk191_8.gif](images/mk191_8.gif)

Gelelim host uygulama kodlarımıza;

```csharp
using System;
using System.ServiceModel;
using System.Configuration;
using MatematikServisLib;

namespace HostApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Servisin sunulacağı base address bilgisi konfigurasyon dosyasından alınır.
            Uri adres=new Uri(ConfigurationManager.AppSettings["adres"]);
            // Matematik tipi, Uri üzerinden host edilmek üzere ServiceHost nesne örneğine bildirilir.
            ServiceHost srv = new ServiceHost(typeof(Matematik), adres);
            // Servis açılırken çalışan event metodu
            srv.Opening += delegate(object sender, EventArgs e)
            {
                Console.WriteLine("Servis açılıyor...");
            };
            // Servis açıldıktan sonraki event metodu
            srv.Opened += delegate(object sender, EventArgs e)
            {
                Console.WriteLine("Servis açıldı...");
            };
            // Servis kapanırkenki event metodu
            srv.Closing += delegate(object sender, EventArgs e)
            {
                Console.WriteLine("Servis kapanıyor...");
            };
            // Servis kapandığındaki event metodu
            srv.Closed += delegate(object sender, EventArgs e)
            {
                Console.WriteLine("Servis kapandı...");
            };
            // Servis açılır
            srv.Open();
            Console.ReadLine();
            // Servis kapatılır
            srv.Close();
        }
    }
}
```

Uygulamamızı çalıştırdığımızda aşağıdakine benzer bir ekran görüntüsü elde ederiz. Dolayısıyla servisimiz şu an için başarılı bir şekilde çalışmaktadır.

![mk191_9.gif](images/mk191_9.gif)

Gelelim istemci tarafına. Yazımızın başında da belirttiğimiz gibi, istemcilerin WCF Servislerini kullanabilmeleri için proxy sınıflarına ve gerekli istemci taraflı konfigurasyon ayarlarına ihtiyaçları vardır. Çok doğal olarak bu sınıfların üretilebilmesi için, servise ait bir metadata bilgisinin olması ve dış ortama sunulması gerekmektedir. Şimdi şunu deneyelim. Servisimizi host eden uygulamayı çalıştıralım ve herhangibir tarayıcı penceresinden, http://localhost:4590/MatSrv adresini girelim. (Tarayıcı penceresinden bu adresi girerken host uygulamanın açık olması şarttır.)

![mk191_10.gif](images/mk191_10.gif)

Dikkat ederseniz, servis için metadata yayınlama hizmetinin şu an için geçersiz olduğu ibaresi vardır. İlgili servisin metadata'sına ulaşamadığımızdan, istemciler için gerekli proxy sınıfını üretemeyiz. Çözüm, konfigurasyon dosyasına behavior tipi eklemektir. Bu tipi Metadata Exchange (MEX) davranışını uygulayacak şekilde aktif etmemiz gerekir. Bunun için app.config dosyamızı aşağıdaki hale getirmemiz yeterlidir.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <appSettings>
        <add key="adres" value="http://localhost:4590/MatSrv"/>
    </appSettings>
    <system.serviceModel>
        <services>
            <service name="MatematikServisLib.Matematik" 
                      behaviorConfiguration="MatematikBehavior">
                <endpoint address="http://localhost:4590/MatSrv" binding="basicHttpBinding" contract="MatematikServisLib.IMatematikServis"/> 
            </service>
        </services>
        <behaviors>
            <serviceBehaviors>
                <behavior name="MatematikBehavior">
                    <serviceMetadata httpGetEnabled="true"/>
                </behavior>
            </serviceBehaviors>
        </behaviors>
    </system.serviceModel>
</configuration>
```

Şimdi host uygulamamızı tekrar çalıştırır ve tarayıcı penceresinden servisimizi tekrar talep edersek aşağıdaki çıktıyı elde ederiz.

![mk191_11.gif](images/mk191_11.gif)

Dolayısıyla artık servisimize ait metadata dış ortama sunulabilir ve http üzerinden elde edilebilir haldedir. Bir başka deyişle proxy sınıfını oluşturabilir ve istemcilerin hizmetine sunabiliriz. Burada,?wsdl takısının olduğuna da dikkat edelim. Bu bize Xml Web Servislerinden son derece tanıdık gelecektir. Bildiğiniz gibi WSDL (Web Service Description Language) bir servisin ne yaptığını Xml olarak söyleyebilen çıktıların üretilmesinde rol almaktadır. Bu yapı WCF içerisindede aynen kullanılabilmektedir.

![mk191_12.gif](images/mk191_12.gif)

Artık istemci tarafını kodlayabiliriz. Ama öncesinde proxy sınıfımızı nasıl üretebileceğimize bakalım. Bunun için iki yolumuz vardır. Birincisi.Net Framework 3.0 SDK ile gelen komut satırı araçlarından olan svcutil.exe dir. Diğeri ise, Visual Studio 2005 içerisinde yer alan Add Service Reference seçeneğidir. Biz bu makalemizde svcutil aracı ile proxy sınıfımızı nasıl yazacağımızı inceleyeceğiz. Bunun için Visual Studio 2005 Command Prompt'ta aşağıdaki komut satırı ifadesini çalıştıralım. (svcUtil aracının başarılı bir şekilde proxy sınıfını ve config dosyasını üretmesi için, host uygulamanın çalışır olduğundan emin olun.)

```bash
svcutil http://localhost:4590/MatSrv?wsdl /out:Proxyim.cs /config:app.config
```

![mk191_13.gif](images/mk191_13.gif)

Gördüğünüz gibi Proxyim.cs ve app.config isimli iki dosya üretildi. Artık tek yapmamız gereken bunları istemci uygulamada kullanmaktır. Üretilen proxyim.cs dosyası içerisinde aşağıdaki şekilde görülen tipler yer almaktadır. Dikkat ederseniz servis sözleşmesine göre uygun bir sınıf üretimi gerçekleştirilmiştir. Ayrıca istemci için gereken konfigurasyon ayarlarıda otomatik olarak app.config dosyası içerisine dahil edilmiştir.

![mk191_14.gif](images/mk191_14.gif)

İstemci tarafında kullanacağımız tip MatematikServisClient isimli sınıftır. İstemci uygulamamızı aşağıdaki kodları yazarak test edebiliriz.

```csharp
using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace Istemci
{
    class Program
    {
        static void Main(string[] args)
        {
            MatematikServisClient mat = new MatematikServisClient();
            Console.WriteLine(mat.Toplam(4, 5).ToString());
            Console.ReadLine();
        }
    }
}
```

Elbetteki istemci uygulamanın çalışabilmesi için öncesinde host uygulamanın çalışıyor olması gerekmektedir. Aksi takdirde EndPointNotFoundException tipinden bir istisna alırız. Bu aynı zamanda gerçketende istemcinin bir sunucuya bağlanmaya çalıştığınında bir ispatıdır. Eğer önce sunucuyu sonrada istemciyi çalıştırırsak uygulamanın başarılı bir şekilde yürüdüğünü ve Toplam metodunun sonucunun elde edildiğini rahatlıkla görebiliriz.

![mk191_15.gif](images/mk191_15.gif)

Bu makalemizde WCF'a kısa bir giriş yapmaya çalıştık. Konuyu daha iyi kavrayabilmek içinde basit bir örnek geliştirdik. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.
