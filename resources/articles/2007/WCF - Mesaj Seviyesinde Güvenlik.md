---
title: "WCF - Mesaj Seviyesinde Güvenlik"
pubDate: 2007-05-16 12:00:00
categories:
  - WCF
tags:
  - WCF
  - C#
  - XML
  - Web Service
  - HTTP
  - Authentication
  - Authorization
  - Performance
---

# WCF - Mesaj Seviyesinde Güvenlik
Değerli Okurlarım Merhabalar,

Dağıtık mimariye yönelik olarak geliştirilen uygulamalarda güvenlik son derece önemli bir faktördür. Özellikle farklı süreçler (process) içerisinde yer alan uygulamalar, birbirleriyle haberleşirken aradaki veri trafiği mesajlar üzerine kuruludur. Bu mesajların ağ ortamları üzerinden (Internet-Intranet) hareket etmeside güvenlik ile ilgili olarak dikkat edilmesi gereken noktaların sayısını arttırır. Temel olarak dağıtık mimarilerde güvenlik düşünüldüğünde, kullanıcıların sunucu tarafından doğrulanması (authentication), doğrulanan kullanıcıların hangi fonksiyonellikleri kullanabileceğine bakılması (authorization), arada hareket etmekte olan mesajların ne şekilde şifreleneceğinin (encryption) veya çözümleneceğinin (decryption) belirlenmesi gibi konular yer almaktadır. Bu tip işlemlere ihtiyaç duyulmasının bilinen pek çok nedeni vardır. Bunlardan bir kaçı aşağıda maddeler halinde listelenmiştir.

- İyi geliştiriciler veya sistem bakımından sorumlu olanlar, genellikle ağ üzerindeki trafiği kontrol etmek, performans kayıplarını tespit etmek amacıyla çeşitli programlar kullanırlar.(Örneğin biz makalemizde istemci ile sunucu arasındaki mesajları görmek adına Microsoft Service Trace Viewer aracından faydalanacağız.) Bu programlar sayesinde ağ üzerinde istemciler ve sunucu arasında hareket eden mesajlar görülebilir. Ancak kötü niyetli kişilerde bu paketleri takip edebilirler. Eğer paketler içerisinde hassas bilgiler var ise söz konusu bilgilerin görülmesi istenmeyen bir durumdur. Dolayısıyla mesajların çeşitli algoritmalar ile (TripleDes, SHA vb...) şifrelenmesi çok doğrudur.
- Yine olayların baş kahramanı olan kötü niyetli kullanıcılar istemciler ve sunucu arasındaki mesajları yakalayıp değiştirebilirler. Bu bir önceki durumdan biraz daha farklıdır. Nitekim mesajın orjinal haliyle gitmesi yerine bozulmuş haliyle taşınması söz konusudur. Bu yaklaşım elbetteki veri bütünlüğünü tamamen bozan bir etki yapar. Sertifikalandırma ve dijital imza gibi tekniklerin kullanılması tercih edilerek gereken tedbirler alınabilir.
- Bazı durumlarda istemciler gerçek sunucu yerine, araya alınmış başka bir yalancı sunucuya başvuruda bulunuyor olabilir. Özellikle internet tabanlı bankacılık uygulamalarında zaman zaman duyduğumuz bu senaryo dağıtık mimari uygulamaları içinde söz konusu olabilir. Önlem olarak çift taraflı doğrulama modeli ele alınabilir.
- Kötü niyetli kişilerin yakaladığı mesajlar, sadece bozulmakla kalmaz defalarca sunucuya gönderilebilir. Dolayısıyla sunucunun doğru bir şekilde çalışması engellenmişte olur. Bu gibi bir duruma önlem olarak güvenli bir iletişim ortamı sağlanması gerekmektedir.

Dikkat edilecek olursa bu basit senaryolar bile, bir dağıtık mimari sisteminin çökmesi için yeterlidir. WCF mimarisinde kullanıcıların doğrulanması sırasında veya doğrulama işlemleri sonrasında arada hareket edecek mesajlar söz konusu olduğunda taşınan hassas bilgiler söz konusudur. Söz konusu bilgilerin güvenliğini iletişim seviyesinde (transport level) ve mesaj seviyesinde (message level) olmak üzere iki şekilde sağlayabiliriz. İletişim seviyesinde güvenliği sağlamanın bilinen yollarından birisi HTTPS'dir. Dolayısıyla iletişim seviyesinde sağlanan güvenliğin işletim sistemi ve donanıma bağlı olaraktan daha etkili ve performanslı olduğunu düşünebiliriz. Mesaj seviyesinde sağlanan güvenlik göz önüne alındığında sorumluluk servisin üzerindedir. Diğer taraftan servis ve istemci arasında gidecek bilgilerin şifrelenmesi hem sunucuyu hemde istemciyi ilgilendirmektedir.

Özellikle Web servisleri üzerinde uygulanabilen güvenlik seçenekleri düşünüldüğünde (Web Service Enhancements), Windows Communication Foundation içerisinde benzer imkanları sağlamak çok daha kolaydır. Bu makalemizde ilk olarak mesaj seviyesinde güvenliğin nasıl sağlanabileceğine dair adım adım ilerleyeceğimiz bir örnek üzerinde durmaya çalışacağız. Her zaman olduğu gibi basit bir sınıf kütüphanesi ile işe başlamak gerekiyor. Sınıf kütüphanesi (Class Library) bir WCF Library olarak tasarlanabilir ve içerisinde aşağıdaki servis sözleşmesi ile tip yer alabilir.

![mk204_1.gif](images/mk204_1.gif)

Servis sözleşmemize ait arayüz (Interface) aşağıdaki gibidir;

```csharp
using System;
using System.ServiceModel;

namespace AritmetikLib
{
    [ServiceContract(Name="AritmetikServisi",Namespace="http://www.bsenyurt.com/AritmetikServisi")]
    public interface IAritmetik
    {
        [OperationContract]
        double Toplam(double x, double y);
    }
}
```

Arayüzü uyarlayan sınıf ise aşağıdaki gibidir.

```csharp
using System;

namespace AritmetikLib
{
    public class Aritmetik:IAritmetik
    {
        #region IAritmetik Members
        public double Toplam(double x, double y)
        {
            return x + y;
        }
        #endregion
    }
}
```

Bu noktada istenirse üretilen assembly'dan faydalanarak istemci için gerekli proxy sınıfının yazdırılması sağlanabilir. Daha önceki makalelerden hatırlayacağınız gibi bu amaçla svcutil aracı kullanılabilir.

Artık servis uygulamasını tasarlamaya başlayabiliriz. Amacımız güvenlik konusuna değinmek olduğundan servis ve istemci uygulamayı basit birer Console uygulaması olarak ele alacağız. İlk olarak servis uygulaması ile başlayalım. Her zaman olduğu gibi bu uygulama için kritik olan referans System.ServiceModel.dll isimli assmebly'dir. Servis uygulaması için gereken konfigurasyon bilgileri başlangıç aşamasında aşağıdaki gibi olmalıdır. Bu bilgileri kolay bir şekilde Edit WCF Configuration seçeneği ile açılan Microsoft Service Configuration Editor arabirimi yardımıylada hazırlayabiliriz.

Servis uygulaması için konfigurasyon dosyası;

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <services>
            <service name="AritmetikLib.Aritmetik">
                <endpoint address="net.tcp://localhost:9002/AritmetikServisi" binding="netTcpBinding" name="AritmetikServerEndPoint" contract="AritmetikLib.IAritmetik" />
            </service>
        </services>
    </system.serviceModel>
</configuration>
```

Servis tarafında TCP protokolünü baz alacak şekilde bir ayarlama yapılmıştır. Buna göre istemciler söz konusu endPoint erişimi için net.tcp://localhost:9002/AritmetikServisi adresini kullanacaktır. Diğer taraftan bağlayıcı tip olarak NetTcpBinding tipi ele alınmaktadır. Bu noktadan sonra sunucu uygulamanın kodları aşağıdaki gibi tasarlanabilir.

```csharp
using System;
using System.ServiceModel;
using AritmetikLib;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(Aritmetik), new Uri("net.tcp://localhost:9002/AritmetikServisi"));
            host.Open();
            Console.WriteLine("Host state " + host.State);
            Console.ReadLine();
            host.Close();
        }
    }
}
```

Sunucu uygulamanın çalıştığı süre boyunca istemcilere hizmet verebilmesi ve söz konusu endPoint'e ait nesne referanslarını kullandırtması için her zaman olduğu gibi ServiceHost tipinden bir örnek kullanılmaktadır. Open metodu ile servis açlmakta, uygulama kapatılırken Close metodu ile söz konusu servis sonlandırılmaktadır.

İstemci uygulamamıza ait konfigurasyon dosyasıda başlangıç için aşağıdaki gibi tasarlanabilir.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel> 
        <client>
            <endpoint address="net.tcp://localhost:9002/AritmetikServisi" binding="netTcpBinding" contract="AritmetikServisi" name="TemelMatClientEndPoint" />
        </client>
    </system.serviceModel>
</configuration>
```

İstemci uygulamaya ait kodlar ise aşağıdaki gibidir.

```csharp
using System;
using System.ServiceModel;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                AritmetikServisiClient client = new AritmetikServisiClient("TemelMatClientEndPoint");
                double sonuc = client.Toplam(3, 5);
                Console.WriteLine(sonuc.ToString());
                Console.ReadLine();
            }
            catch (FaultException excp)
            {
                Console.WriteLine(excp.Message);
            }
        }
    }
}
```

İstemci uygulamada yer alan AritmetikServisiClient isimli sınıf, svcutil aracı yardımıyla AritmetikLib.dll isimli assembly üzerinden elde edilmektedir. (Buraya kadarki adımlarımız daha önceki makalelerimizde incelendiğinden detaylı bir şekilde açıklanmamıştır.)

Bu makalemizde bizim için önemli olan mesaj seviyesinde güvenliğin (Message Level Security) nasıl sağlanacağıdır. İlk olarak servis tarafında bazı ayarlamaların yapılması gerekmektedir. Örneğin, mesaj seviyesinde güvenlik uygulanacağının, mesajların belirtilen ve bilinen bir algoritmaya göre şifreleneceğinin belirtilmesi vb... Bu amaçla servis tarafında yer alan App.config dosyası Microsoft Service Configuration Editor yardımıyla açılıp yeni bir bağlayıcı konfigurasyon (Binding Configuration) eklenmeli ve EndPoint ile ilişkilendirilmelidir. Öncelikle New Binding Configuration linkine tıklanarak netTcpBinding tipi seçilir ve eklenir. Bir başka deyişle servis tarafında kullanılan Binding tipi için gereken ayarlamaların yapılması için yeni bir element eklenmektedir.

![mk204_2.gif](images/mk204_2.gif)

Uygulama netTcpBinding bağlayıcı tipini kullandığı için, bağlayıcı konfigurasyon ayarları buna göre yapılmalıdır. Yeni eklenen elementin Security sekmesine geçildiği takdirde gereken güvenlik ayarları ile ilişkili özellikler olduğu görülebilir.

![mk204_3.gif](images/mk204_3.gif)

Mode özelliğinde güveinlik seviyesinin mesaj, iletişim veya bunların kombinasyonu olup olmadığı belirlenir. Örneğimizde mesaj seviyesinde güvenlik gerçekleştirileceğinden Mode özelliğine Message değeri verilmiştir. AlgorithmSuite özelliğinde, mesajların hangi modele göre şifreleneceği belirlenir. Burada oldukça fazla ve yeterli seviyede şifreleme algoritmasına ait tanımlamalar yer almaktadır. Dikkat edilmesi gereken nokta burada belirtilen şifreleme modelinin istemci içinde aynı olması gerektiğidir. Bu durum Mode özelliğinin değeri içinde geçerlidir. Yani servis tarafında mesaj seviyesinde güvenlik kullanılacağı belirtiliyorsa, istemci uygulamada aynı model kullanılmalıdır.

![mk204_4.gif](images/mk204_4.gif)

Biz örnek olarak TripleDesSha256 modelini göz önüne alabiliriz. AlgorithmSuite için varsayılan değer Basic256' dır. Security kısmında yer alan özelliklerden bir diğeri olan MessageClientCredentialType ile, istemcilerin doğrulamasının (Authentication) nasıl yapılacağı belirlenmektedir.

![mk204_5.gif](images/mk204_5.gif)

Ekran görüntüsündende izlenebileceği gibi söz konusu özelliğe, Windows, UserName, Certificate, IssuedToken ve None değerlerinden birisi verilebilir. Örnekte Windows değeri verilmiştir. Bir başka deyişle istemcilerin servise gönderecekleri ehliyet bilgileri (Credentials) windows tabanlı doğrulama modeline göre aktarılacaktır. Örnekte yer alan istemci ve sunucu uygulamalar aynı makine üzerinde yer aldıklarında varsayılan olarak makineyi açan kullanıcı bilgileri ele alınacaktır.

TransportSecurity kısmındaki özellikler iletişim seviyesinde güvenlik modeli için gerektiğinden şu aşamada varsayılan halleri ile bırakılabilirler. Bu işlemlerin ardından bağlayıcı konfigurasyon bilgileri için ServiceBindingConfiguration adı belirtilerek gereken değişiklikler kaydedilebilir.

![mk204_6.gif](images/mk204_6.gif)

Artık tek yapılması gereken servisten sunulan ilgili endPoint için BindingConfiguration özelliğine hazırlanan ServiceBindingConfiguration değerini vermektir. Böylece endPoint içerisinde kullanılan binding tipinin belirttiği güvenlik ayarları aktif olarak set edilmiş olunur.

![mk204_7.gif](images/mk204_7.gif)

Bu işlemlerin ardından servis tarafı için konfigurasyon dosyasının içeriği aşağıdaki gibi yenilenecektir. Dikkat edilecek olursa görsel tarafta yapılan tüm eklentiler buraya element ve nitelikler olarak geçirilmiştir. Dikkat edilmesi gereken nokta, endPoint elementi içerisindeki bindingConfiguration elementinin değerinin, netTcpBinding alt elementindeki name özelliğinin değeri oluşudur.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <bindings>
            <netTcpBinding>
                <binding name="ServiceBindingConfiguration">
                    <security mode="Message">
                        <message algorithmSuite="TripleDesSha256" />
                    </security>
                </binding>
            </netTcpBinding>
        </bindings>
        <services>
            <service name="AritmetikLib.Aritmetik">
                <endpoint address="net.tcp://localhost:9002/AritmetikServisi" bindingConfiguration="ServiceBindingConfiguration" binding="netTcpBinding" name="AritmetikServerEndPoint" contract="AritmetikLib.IAritmetik" />
            </service>
        </services>
    </system.serviceModel>
</configuration>
```

Sırada istemci tarafı için yapılması gereken ayarlar var. Yine istemci tarafındaki konfigurasyon dosyasında, aynen servis tarafındaki konfigurasyon dosyasındakine benzer güvenlik ayarlamaların yapılması gerekmektedir. İlk olarak bir bindingConfiguration elementi oluşturulmalıdır. Bu elementin security ile ilişkili özelliklerinde mesaj seviyesi için AlgorithmSuite değeri servis tarafındaki ile aynı olacak şekilde TripleDesSha256 olarak belirlenmelidir. Diğer taraftan aynı sunucu tarafında yapıldığı gibi güvenlik modu mesaj seviyesine çekilmelidir. Bu değer, konfigurasyon dosyasında security elementi içerisinde yer alan mode niteliği ile set edilmektedir ve Message olarak ayarlanmıştır. Bundan sonra oluşturulan bindingConfiguration elementi istemci tarafındaki endPoint ile bindingConfiguration niteliği yardımıyla ilişkilendirilmelidir. Bunun sonucu olarak istemci uygulama için konfigurasyon dosyasının son hali aşağıdaki gibi olacaktır.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel> 
        <bindings>
            <netTcpBinding>
                <binding name="ClientBindingConfiguration">
                    <security mode="Message">
                        <message algorithmSuite="TripleDesSha256" />
                    </security>
                </binding>
            </netTcpBinding>
        </bindings>
        <client>
            <endpoint address="net.tcp://localhost:9002/AritmetikServisi" binding="netTcpBinding" bindingConfiguration="ClientBindingConfiguration" contract="AritmetikServisi" name="TemelMatClientEndPoint" />
        </client>
    </system.serviceModel>
</configuration>
```

Artık istemci ve sunucu tarafı için mesaj seviyesinde güvenlik ayarları hazırdır. Ancak bunu test ederek analiz etmek gerekmektedir. Microsft Windows SDK tam bu amaç için tasarlanmış ve Windows Communication Foundation uygulamalarında istemci ile sunucu arasındaki mesaj trafiğini izlememizi sağlayan Service Trace Viewer isimli bir araç ile birlikte gelmektedir.

Ancak söz konusu aracın geliştirilen WCF uygulamasını izleyebilmesi içinde servis tarafında Diagnostics ayarlarının tesis edilmesi gerekir. Bu ayarlarıda yine Microsoft Service Configuration Editor yardımıyla kolayca gerçekleştirebiliriz. Burada söz konusu ayarlar üzerinde şu an için çok fazla durmayacağız. Öncelikli olarak amacımız servis ve istemciler arasındaki mesajlaşmayı izlemektir.

İzleme (Trace) işlemi için yapılan ayarlardan sonra servis tarafındaki konfigurasyon dosyasının içeriği aşağıdaki gibi olacaktır. Dikkat edilmesi gereken noktalardan birisi sharedListeners elementi içerisinde yer alan initializeData niteliğinin değeridir. Burada uygulamanın yazıldığı klasör altında svclog (Service Log) uzantılı bir fiziki dosya bildirimi yapılmıştır. Bu bildirim Service Trace Viewer uygulaması tarafından ele alınacak dosyayı işaret etmektedir ve log bilgilerinin tamamı burada tutulmaktadır.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.diagnostics>
        <sources>
            <source name="System.ServiceModel.MessageLogging" switchValue="Verbose,ActivityTracing">
                <listeners>
                    <add type="System.Diagnostics.DefaultTraceListener" name="Default">
                        <filter type="" />
                    </add>
                    <add name="MessageListener">
                        <filter type="" />
                    </add>
                </listeners>
            </source>
        </sources>
        <sharedListeners>
            <add initializeData="E:\Vs2005Projects\WCF Samples\MesajSeviyesiGuvenlikTcp\Server\app_tracelog.svclog" type="System.Diagnostics.XmlWriterTraceListener, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" name="MessageListener" traceOutputOptions="None">
                <filter type="" />
            </add>
        </sharedListeners>
    </system.diagnostics>
    <system.serviceModel>
        <diagnostics>
            <messageLogging logEntireMessage="true" logMessagesAtServiceLevel="true" logMessagesAtTransportLevel="true" />
        </diagnostics>
        <bindings>
            <netTcpBinding>
                <binding name="ServiceBindingConfiguration">
                    <security mode="Message">
                        <message algorithmSuite="TripleDesSha256" />
                    </security>
                </binding>
            </netTcpBinding>
        </bindings>
        <services>
            <service name="AritmetikLib.Aritmetik">
                <endpoint address="net.tcp://localhost:9002/AritmetikServisi" binding="netTcpBinding" bindingConfiguration="ServiceBindingConfiguration" name="AritmetikServerEndPoint" contract="AritmetikLib.IAritmetik" />
            </service>
       </services>
    </system.serviceModel>
</configuration>
```

Bu noktadan sonra istemci ve sunucu uygulamalar test edilebilirler. Çalışma zamanında istemci ve sunucu aktif iken Service Trace Viewer programı çalıştırılıp söz konusu svclog dosyası açılırsa ilk başta aşağıdakine benzer bir ekran ile karşılaşılacaktır.

![mk204_8.gif](images/mk204_8.gif)

Bu ekranda özellikle dikkat edilmesi gereken kısımlar sol taraftaki Message sekmesinde yer alan son dört mesajdır. Bu mesajlar aslında istemciden sunucuya gelen talepleri ve sunucudan istemciye dönen cevapları içeren mesajlardır. İstemci uygulamanın kodları hatırlanacak olursa burada Toplam isimli metoda bir çağır yapılmaktadır. Topla metodu servis tarafında çalıştırılıp sonucu istemci tarafına gelmektedir. Dolayısıyla istemci Topla metoduna çağrı yaptığında aktarılan parametre ve diğer bilgiler servise gönderilecek, serviste ilgili fonksiyon çalıştırılacak ve sonucu istemci tarafına geri bildirilecektir.

![mk204_9.gif](images/mk204_9.gif)

Bu mesajlardan ilki istemciden sunucya gelen bilginin şifreli olarak nasıl geldiğini göstermektedir. Şifrelenmiş veriyi görmek için sağ alt tarafta yer alan Formatter, XML yada Message kısımları kullanılabilir. Formatter kısmına baktığımızda Envelope Information bölümündeki e:ChiperData kısmında, istemciden sunucuya gelen parametrelerin TripleDesSha256 algoritmasına göre şifrelenmiş halinin yer aldığı görülebilir.

![mk204_10.gif](images/mk204_10.gif)

Eğer takip eden mesaj için aynı kısma tekrar bakarsak verinin servis tarafından çözümlenmiş olan hali görülmektedir. Dikkat edilecek olursa söz konusu değerler, istemcideki Toplam metodunun aldığı parametre içerikleridir.

![mk204_11.gif](images/mk204_11.gif)

Bir sonraki mesaj servisin bu metod çağırısına karşılık vereceği cevap hakkında bilgiler içermektedir. Yine Parameters kısmına bakılacak olursa aşağıdaki ekran görüntüsünde olduğu gibi 8 değerinin yer aldığını farkedilir.

![mk204_12.gif](images/mk204_12.gif)

Ancak son mesaj istemciye gönderilecek verinin şifrelenmiş halini işaret etmektedir. Bir başka deyişle, 8 değeri aslında servisten istemciye gönderilmeden önce TripleDesSha256 modeline göre şifrelenecek ondan sonra iletilecektir.

![mk204_13.gif](images/mk204_13.gif)

Eğer aynı örnek güvenlik ayarları yapılmadan test edilirse ve yine Service Trace Viewer yardımıyla istemci ile sunucu arasındaki mesajlaşmalar izlenirse verilerin herhangibir şekilde şifrelenmediği görülebilir. Bu amaçla istemci ve sunucu tarafındaki konfigurasyon dosyalarında security elementlerinin içerisinde yer alan Mode niteliklerinin değerini None olarak belirlemek yeterlidir.

Bu makalemizde özellikle mesaj seviyesinde güvenliği nasıl sağlayabileceğimizi incelemeye çalıştık. İlerleyen makalelerimizde windows tabanlı ve iletişim seviyesinde güvenlik işlemlerini nasıl yapabileceğimizi incelemeye devam edeceğiz. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.

[Örnek Uygulama İçin Tıklayınız.](assets/MesajSeviyesiGuvenlikTcp.rar)
