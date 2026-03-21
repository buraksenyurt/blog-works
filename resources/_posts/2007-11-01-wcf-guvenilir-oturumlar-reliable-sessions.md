---
layout: post
title: "WCF - Güvenilir Oturumlar(Reliable Sessions)"
date: 2007-11-01 12:00:00 +0300
categories:
  - wcf
tags:
  - windows-communication-foundation
  - reliable-session
---
WCF (Windows Communication Foundation) bilindiği üzere bir servis yönelimli mimari (Service Oriented Architecture) yaklaşımıdır. Buda basitçe, birbirleriyle haberleşen istemci (client) ve sunucu (server) uygulamaların var olması anlamına gelmektedir. Bu haberleşme çok doğal olarak bir ağ (network) ortamı üzerinde gerçekleşir. Ağ ortamı intranet gibi bir sistem olabileceği gibi kablolu veya kablosuz (wireless) bir internet ortamıda olabilir. Hal böyle olunca arada hareket etmekte olan mesajların güvenliği önem arz eden bir konudur. Mesaj güvenliğinden kasıt sadece şifreleme yada sertifikalı bir iletişimin sağlanması demek değildir. Bunların sağlanması için WCF mimarisi içerisindede çeşitli teknikler bulunmaktadır. Bu teknikler bir yana istemci ve sunucu (servis) arasında güvenilir bir oturumun (reliable session) var olması gereken durumlarda söz konusudur. Güvenilir bir oturum sağlanması için gereken sebepler arasında aşağıdaki maddeler göz önüne alınabilir;

- Ağ ortamında istemci ve servis arasındaki bağlantının kopması olasıdır.
- Arada hareket etmekte olan mesajlar kesintiye uğrayabilir.
- İstemciden servis tarafına gelmekte olan mesajlar farklı yollar üzerinden hedeflerine ulaşmaktadır. Böyle bir durumda servise farklı sıralarda ulaşmaları söz konusudur. Ancak mesaj sırası önemli olabilir.
- Mesajlar beklenmedik bir şekilde kaybolabilir yada farklı bir yere doğru yönlendirilebilir.

Bu seçenekler göz önüne alındığında istemci ve sunucu arasında güvenilir bir oturum açılma ihtiyacı daha belirgin bir şekilde ortaya çıkmaktadır. Nitekim arada hareket eden mesajların üçüncü kişiler tarafından yakalanması özellikle cevaplama saldırıları (Reply Attacks) ile çok farklı amaçlarla kullanılabilir. Bunun için örnek olarak bir alışveriş sitesinin işleyişi ele alınabilir. Üçüncü şahıs, bir sipariş bilgisine ait mesajı yakalayıp defalarca sunucuya işlenmesi için gönderebilir. Bunun sonucunda alışverişi yapan gerçek kişi, hiç yapmamış olduğu siparişlerle karşı karşıya kalacaktır. Üstelik üçüncü şahıs, bu mesajları istediği zaman gönderme imkanına sahip olabilir.

Cevaplama saldırılarının (Reply Attack) dışında istemcilerin göndereceği taleplere ait mesajların servis tarafında, istemciden gönderildiği sırada ele alınması gerekebilir ki buda güvenilir oturumların sağlanması için yeterli nedenlerdendir. Windows Communication Foundation mimari alt yapısı içerisinde güvenilir oturumlar (Reliable Sessions) açılabilmesi için gereken özellikler yer almaktadır. Nitekim güvenilir oturumların açılabilmesi için gerek ve yeter şart WS-ReliableMessaging protokolüne uygun bir ortamın sağlanmış olmasıdır. WCF bu protokolü doğrudan destekleyen çeşitli bağlayıcı tipler (bindiny types) içermektedir.

> WS-ReliableMessaging, Web servislerine yönelik olarak geliştirilmiş platform bağımsız pek çok standarttan sadece bir tanesidir. Toplu olarak WS- şeklinde ifade edilen tüm Web servisi standartları için [http://en.wikipedia.org/wiki/List_of_Web_service_specifications](http://en.wikipedia.org/wiki/List_of_Web_service_specifications) adresinden bilgi alınabilir.

WS-ReliableMessaging, tek bir kaynak (Source) ve tek bir hedef (Target) arasında güvenilir bir mesajlaşma için gereken şartnameleri içeren Organization for the Advancement of Structured Information Standards (OASIS - [http://www.oasis-open.org/home/index.php](http://www.oasis-open.org/home/index.php)) organizasyonu tarafından kabul edilmiş bir protokoldür. Son olarak 14 Haziran 2007 tarihinde 1.1 versiyonu yayınlanmıştır. WS-ReliableMessaging standardının amacı; güvenilir olmayan bir altyapı (Unreliable Infrastructure) üzerinde koşan bir kaynak uygulamadan hedef uygulamaya doğru güvenilir bir şekilde mesaj gönderilmesini sağlamaktır. Mesajın içeriğinin şifrelenmesi veya iletişim kanalının güvenli hale getirilmesi (örneğin Secure Socket Layer ile) konuları ile ilgilenmez. Bu şartnameye (Specification) göre güvenilir oturumlarda söz konusu olan mimari model aşağıdaki şekilde olduğu gibidir.

![mk229_1.gif](/assets/images/2007/mk229_1.gif)

Şekilde kaynak uygulama ile hedef uygulama arasındaki bir mesajlaşma trafiği yer almaktadır. Burada Uzak Mesajlaşma Kayanağı mesajı gönderirken WS-ReliableMessaging kullanır. Diğer taraftan burada tek bir kaynak ve tek bir hedef vardır. WCF (Windows Communication Foundation) bu protokolün kullanımına destek vererekten aşağıdaki maddelerde görülen kazanımları sağlamaktadır;

- Kaynaktan gönderilen tüm mesajların hedefe varması garantilenir.
- Kaynaktan hedefe gönderilen mesajların tekrar edilmesi önlenir. Bir başka deyişle mesajın sadece bir tane gönderilmesi garanti edilir.
- Kayıp mesajlar tespit edilir ve mümkünse bunlarında kaynaktan hedefe doğru yeniden gönderilmesi sağlanır.
- Kaybolan mesajların geri alınmayacak durumda olması halinde istisna (exception) fırlatılması sağlanır.
- Opsiyonel olarak mesajların gönderildikleri sırada işlenmeleri garantilenir. WCF bu amaçla tampon (buffer) sistemini kullanılır. Buna göre tüm mesajlar tamponda toplanır ve gönderildikleri sıraya göre hedef tarafında işlenir.

WS-ReliableMessaging, mesajların servis tarafında gönderildikleri sırada ele alınmalarını sağlayan şartnameler (Specifications) sunmasına rağmen gerçek anlamda, MSMQ (MicroSoft Messaging Queing) sisteminde olduğu gibi bir mesaj kuyruğu yapısı bildirmez. MSMQ bunun için farklı bir form kullanır.

> WS-ReliableMessaging, Windows Communication Foundation dışında BEA WebLogic, IBM WebSphere, Apache Sandesha gibi sistemler tarafındanda ele alınmaktadır.

WCF mimarisinde aslında söz konusu protokolün sağlanması için gereken teş şey kaynak ve hedef uygulamaların aynı zaman dilimi içerisinde çalışıyor olmalarıdır. WCF sistemi içerisinde yer alan bağlayıcılardan basicHttpBinding, netNamedPipeBinding, netPeerTcpBinding tipleri güvenilir oturumları (Reliable Sessions) desteklememektedir. Bununla birlikte wsDualHttpBinding tipi için güvenilir oturumlar kaldırılamaz. MSMQ desteği veren bağlayıcılardan olan msmqIntegrationBinding ve netMsmqBinding tipleri ise kendi güvenilir oturum şartnamelerini uygularken WS-ReliableSession standardını kullanmazlar. Aslında konu ile ilişkili olarak aşağıdaki tablonun göz önüne alınması önemlidir.

Bağlayıcı Tip
(Binding Type)
Güvenilir
Oturum
Desteği
Varsayılan
Güvenilir
Oturum Hali
Sıralı
Mesaj
Desteği
Varsayılan
Sıralı
Mesaj
Desteği

netNamedPipeBinding
Yok

X

Var

X

netTcpBinding
Var
Açık
Var
Kapalı

netPeerTcpBinding

X

wsDualHttpBinding
Var
Açık
Var
Açık

wsHttpBinding
Var
Kapalı
Var
Açık

wsFederationHttpBinding
Var
Kapalı
Var
Açık

basicHttpBinding

X

netMsmqBinding
(Bu bağlayıcı standart Web Servisi (asmx) modelini sunar. Bu modelde varsayılan olarak güvenilir oturumlar bulunmamaktadır.)

msmqIntegrationBinding
(MSMQ tabanlı bir kuyruk sistemi kullanırlar.)

Bu teorik bilgilerden sonra artık bir örnek ile devam etmekte fayda bulunmaktadır. Örnekte basit olarak netTcpBinding kullanan bir WCF sistemi yer almaktadır. Sistemde yer alan servis sözleşmesi (Service Contract) ve uygulayıcı tipe ait içerikler aşağıda görüldüğü gibi olmakla birlikte, FabrikaLib isimli bir WCF Sınıf Kütüphanesi (WCF Class Library) içerisinde yer almaktadırlar.

![mk229_2.gif](/assets/images/2007/mk229_2.gif)

Servis sözleşmesi;

```csharp
using System;
using System.ServiceModel;

namespace FabrikaLib
{
    [ServiceContract(Name="UretimServisi", Namespace="http://www.bsenyurt.com/FabrikaLib/UretimServisi" , SessionMode=SessionMode.Required)]
    public interface IUretici
    {
        [OperationContract(IsInitiating=true)]
        int BilesenAl(string[] bilesenAdi);
        [OperationContract(IsInitiating=false)]
        void Karistir();
        [OperationContract(IsInitiating=false,IsTerminating=true)]
        bool UretimiYap();
    }
}
```

Servis sözleşmesinde (Service Contract) dikkat edileceği üzere her istemci (Client) için bir oturum (Session) açılmasını garantilemek adına SessionMode özelliğine SessionMode.Required değeri atanmıştır. Güvenilir oturumlarda ilk şartlardan birisi, istemci ile servis arasında bir oturumun söz konusu olmasıdır. Bu nedenle bir oturumun mutlaka hazırlanması isteğinin Servis sözleşmesinde belirtilmesi yerinde bir karardır. Bununla birlikte metodların işleyiş sıralarıda OperationContract niteliklerine (attribute) ait özellikler ile belirlenmiştir. Buna göre istemci tarafından ilk çağrılabilecek metod BilesenAl fonksiyonu iken oturumu sonlandırma işlemini üstlenecek olan işlevse UretimiYap isimli fonksiyondur.

Uygulayıcı sınıf;

```csharp
using System;
using System.ServiceModel;

namespace FabrikaLib
{
    [ServiceBehavior(InstanceContextMode= InstanceContextMode.PerSession)]
    public class Uretici:IUretici
    {
        #region IUretici Members

        public int BilesenAl(string[] bilesenler)
        {
            // Bileşenin eklenme işlemi
            return bilesenler.Length;
        }

        public void Karistir()
        {
            // Bileşenin karıştırılma işlemi
        }

        public bool UretimiYap()
        {
            return true;
        }

        #endregion
    }
}
```

Her bir istemci için bir oturumun açılmasını sağlamak adına Uretici sınıfına ServiceBehavior niteliği ile PerSession ataması yapılmıştır. Servis tarafından sunulmakta olan fonksiyonelliklerin ne iş yaptığı şu aşamada çok önemli değildir. Örnekteki asıl amaç, istemci ve servis arasında güvenilir bir oturum (Reliable Session) açılması ve arada hareket eden mesajların izlenerek (Trace) durumunun detaylı analizinin yapılmasıdır. Servis tarafı basit olması açısından bir Console uygulaması olarak tasarlanmıştır. Servis tarafına ait Main kodları ile konfigurasyon dosyasının içeriği başlangıçta aşağıdaki gibidir.

Servis uygulaması kodları;

```csharp
using System;
using System.ServiceModel;
using FabrikaLib;

namespace Sunucu
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(Uretici));
            host.Opened += new EventHandler(host_Opened);
            host.Closed += new EventHandler(host_Closed);
            host.Open();
            Console.ReadLine();
            host.Close();
        }

        static void host_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("Servis kapatıldı");
        }

        static void host_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("Servis dinlemede");
        }
    }
}
```

Servis tarafı konfigurasyon içeriği;

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <bindings />
        <behaviors>
            <serviceBehaviors>
                <behavior name="UretimServisiBehavior">
                    <serviceDebug includeExceptionDetailInFaults="true" />
                </behavior>
            </serviceBehaviors>
        </behaviors>
        <services>
            <service behaviorConfiguration="UretimServisiBehavior" name="FabrikaLib.Uretici">
                <endpoint address="net.tcp://localhost:9000/Fabrika/UretimServisi.svc" binding="netTcpBinding" name="UretimServisiEndPoint" contract="FabrikaLib.IUretici" />
            </service>
        </services>
    </system.serviceModel>
</configuration>
```

İstemci uygulama üzerinde, servis tarafında oluşabilecek istisnaları (Exception) detaylı bir şekilde ele alabilmek için serviceDebug elementinde includeExceptionDetailInFaults niteliğinin değeri true olarak set edilmiştir. İstemci tarafıda servis tarafı gibi bir Console uygulaması olarak tasarlanabilir.

> İstemci tarafı için gerekli olan proxy sınıfı ve servise göre otomatik oluşturulan konfigurasyon dosyasının üretimi için svcutil.exe aracından aşağıdaki gibi yararlanılması gerekmektedir.
> svcutil FabrikaLib.dll
> svcutil www.bsenyurt.com.FabrikaLib.UretimServisi.wsdl.xsd /out:UretimServisi.cs
> Bu işlemin ardından proxy sınıfı ve konfigurasyon dosyası, istemci uygulamaya taşınır.

İstemci tarafındaki kodlar ve konfigurasyon içeriği ise aşağıdaki gibidir.

İstemci uygulaması kodları;

```csharp
using System;
using System.ServiceModel;

namespace Istemci
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {    
                Console.WriteLine("İşlemi başlatmak için bir tuşa basınız...");
                Console.ReadLine();
                UretimServisiClient cli = new UretimServisiClient("UretimServisiClientEndPoint");
                cli.BilesenAl(new string[] { "C", "O2", "H2SO4" });
                cli.Karistir();
                string durum = cli.UretimiYap()==true?"Üretim gerçekleştirildi":"Üretim yapılamadı";
                Console.WriteLine(durum);
                Console.ReadLine();
            }
            catch (Exception excp)
            {
                Console.WriteLine(excp.Message);
            }
        }
    }
}
```

İstemci tarafı konfigurasyon içeriği;

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <system.serviceModel>
        <bindings />
        <client>
            <endpoint address="net.tcp://localhost:9000/Fabrika/UretimServisi.svc" binding="netTcpBinding" contract="UretimServisi" name="UretimServisiClientEndPoint" />
        </client>
    </system.serviceModel>
</configuration>
```

Normalde svcutil tarafından üretilen konfigurason dosyası içerisinde çok daha fazla özellik yer almaktadır. Bu niteliklerin değerleri şu aşamada önemli olmadığından istemci konfigurasyon dosyası bilinçli olarak yukarıdaki gibi sadeleştirilmiştir.

Asıl işlemler bundan sonra başlamaktadır. Amaç güvenilir bir oturum ortamı hazırlamaktır. Bunu gerçekleştirmek son derece basittir. Tek yapılması gereken bağlayıcı ile ilgili özel konfigurasyonları ve içinde yer alan özelliklerin değerlerini uygun bir şekilde belirlemektir. Servis tarafında netTcpBinding için bir BindingConfiguration eklenmelidir. Söz konusu kısımda, aşağıdaki şekildende görüldüğü gibi ReliableSession Properties bölümünden Enabled özelliği true olarak belirlenmelidir. Böylece güvenilir bir oturumun tesis edileceği belirtilmiş olunur.

InactivityTimeout özelliğinin aldığı değer ile, mesajların kaybolma ihtimali için gereken bekleme süresi ayarlanır. Yani, 10 dakikalık süre içerisinde beklenen mesaj alınmassa ters giden bir şeyler olduğuna karar verilir ve WCF çalışma zamanı (Run Time) bir Fault Exception üreterek bunu istemci tarafına gönderir. Aynı zamanda o ana kadar yapılmış olan işlemler geri alınır (Rollback) ve istemci ile servis arasındaki güncel oturum sonlandırılır. Ordered özelliğine atanan değerin true olarak set edilmesi ile, servise gelen mesajların istemcinin gönderdiği sırada ele alınmaları garanti edilmiş olunmaktadır. Bu zorunlu olmamasına rağmen güvenilir oturumların sağlanması adına önemlidir.

![mk229_3.gif](/assets/images/2007/mk229_3.gif)

Binding ayarlarında dikkat edilmesi gereken noktalardan biriside TransferMode özelliğinin değeridir. netTcpBinding bağlayıcı tipi (Binding Type) için bu değer varsayılan olarak aşağıdaki şekilde görüldüğü gibi Buffered olarak belirlenmiştir.

![mk229_4.gif](/assets/images/2007/mk229_4.gif)

NetTcpBinding kullandığı transfer protokolü (TCP) nedeni ile Stream'lere izin vermektedir. Bir başka deyişle istemcinin gönderdiği mesajların servis tarafında tamamlanmasını beklemeden işlenmesine başlanabilmektedir. Ancak güvenilir oturumlarda, mesajların tamamlandıktan sonra, bir başka deyişle servis tarafına ulaştıktan sonra ele alınmaları doğru sırada işlenmeleri söz konusu olduğunda önemlidir. Bu sebepten bu varsayılan değerin değiştirilmemesi önerilir. Diğer taraftan özellikle WsHttpBinding gibi bağlayıcı tipler, Stream yapısını HTTP protokolü nedeni ile desteklemediklerinden her zaman için Buffered sistemi ile çalışırlar.

Yukarıda yapılan değişiklikler sonrasında servis tarafında yer alan konfigurasyon dosyasının son hali aşağıdaki gibi olacaktır.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <bindings>
            <netTcpBinding>
                <binding name="BConf" transferMode="Buffered">
                    <reliableSession ordered="true" inactivityTimeout="00:10:00"
enabled="true" />
                </binding>
            </netTcpBinding>
        </bindings>
        <behaviors>
            <serviceBehaviors>
                <behavior name="UretimServisiBehavior">
                    <serviceDebug includeExceptionDetailInFaults="true" />
                </behavior>
            </serviceBehaviors>
        </behaviors>
        <services>
            <service behaviorConfiguration="UretimServisiBehavior" name="FabrikaLib.Uretici">
                <endpoint address="net.tcp://localhost:9000/Fabrika/UretimServisi.svc" binding="netTcpBinding" bindingConfiguration="BConf" name="UretimServisiEndPoint" contract="FabrikaLib.IUretici" />
            </service>
        </services>
    </system.serviceModel>
</configuration>
```

Çok doğal olarak istemci uygulama tarafındada servis tarafındakine benzer olacak şekilde konfigurasyon ayarlarının yapılması gerekmektedir. Aynı adımlar izlenildiğinde istemci tarafındaki konfigurasyon dosyasın son halide aşağıdaki gibi olacaktır.

![mk229_5.gif](/assets/images/2007/mk229_5.gif)

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <system.serviceModel>
        <bindings>
            <netTcpBinding>
                <binding name="BConf" transferMode="Buffered">
                    <reliableSession ordered="true" inactivityTimeout="00:10:00" enabled="true" />
                </binding>
            </netTcpBinding>
        </bindings>
        <client>
            <endpoint address="net.tcp://localhost:9000/Fabrika/UretimServisi.svc" binding="netTcpBinding" bindingConfiguration="BConf" contract="UretimServisi" name="UretimServisiClientEndPoint" />
        </client>
    </system.serviceModel>
</configuration>
```

Olaya istemci açısından bakıldığında çalışma aşamasında dikkate değer bazı noktalar vardır. Herşeyden önce istemci uygulama, servis tarafında belirtilen timeout süresi dahilinde mesaj göndermeyi bırakmış olabilir. Bu çoğunlukla istemci uygulama kullanıcısının bu yönde bir aksiyon gerçekleştirmediği durumlarda söz konusu olabilir. Tabi burada istemci ile servis arasında bir oturum açıldıktan sonraki süre zarfı ele alınmaktadır. Bu tarz bir durumda istemcinin ağ üzerinde asılı kaldığı yorumu yapılır.

Dolayısıyla servisin istenmeyen bir şekilde Fault Exception döndürmesi olasıdır. Bu sebepten, istemci taraftaki WCF Çalışma Zamanı (Run Time) belirli periyodlarda servis tarafına canlı olduğuna dair mesajlar gönderir. Böylece servis tarafı, kendisine bağlı oturumun sahibi olan istemcinin halen daha canlı olduğundan haberdar olur. Bununla birlikte istemci uygulama, servis tarafından bir onay mesajı (Acknowledge Message) bekler. Eğer bu mesaj istemci tarafında belirtilen InactivityTimeout süresinde alınamıyorsa, servisin bir şekilde öldüğü sonucuna varılır ve istemci tarafında WCF çalışma zamanı (Run Time) bir istisna (Exception) fırlatır. Bu istisna, istemci tarafında ele alınmalı ve uygulamanın istem dışı şekilde sonlanmasının önüne geçilmelidir.

Artık istemci ve servis arasında güvenilir bir oturum açılması için gereken ayarlar tamamlanmıştır. Bu oturumun tesis edilmesi halinde, arada gidip gelen mesajların incelenebilmesi adına servis tarafında gerekli ayarların yapılması gerekmektedir. Bu amaçla yine konfigurasyon içerisinde Diagnostics ayarları yapılmalıdır. İlk olarak EnableMessageLogging linkine tıklanarak mesaj günlüğü aktif hale getirilir. Sonrasında ise Diagnostics klasöründe yer alan Message Logging kısmına gidilerek LogEntireMessage değeri true, LogMalformedMessages değeride false olarak set edilir.

![mk229_6.gif](/assets/images/2007/mk229_6.gif)

Bu işlemin ardından Listeners klasöründeki ServiceModelMessageLoggingListener kısmına gidilerek InitData özelliğine bir svclog dosyası adı ve tam adresi aşağıdaki şekilde olduğu gibi bildirilir. Tahmin edileceği üzere servis tarafına ulaşan ve istemciye giden mesaj içerikleri bu dosya içerisinde toplanacaktır.

![mk229_7.gif](/assets/images/2007/mk229_7.gif)

Bu işlemlerin ardından servis tarafındaki konfigurasyon dosyasının içeriği aşağıdaki gibi olacaktır.

```csharp
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.diagnostics>
        <sources>
            <source  name="System.ServiceModel.MessageLogging" 
switchValue="Warning, ActivityTracing">
                <listeners>
                    <add type="System.Diagnostics.DefaultTraceListener" 
name="Default">
                        <filter type="" />
                    </add>
                    <add name="ServiceModelMessageLoggingListener">
                        <filter type="" />
                    </add>
                </listeners>
            </source>
        </sources>
        <sharedListeners>
            <add initializeData="c:\app_messages.svclog"  type="System.Diagnostics.XmlWriterTraceListener, System, 
Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" 
name="ServiceModelMessageLoggingListener" traceOutputOptions="Timestamp">
                <filter type="" />
            </add>
        </sharedListeners>
    </system.diagnostics>
    <system.serviceModel>
        <diagnostics>
            <messageLogging logEntireMessage="true" logMalformedMessages="false" logMessagesAtTransportLevel="true" />
        </diagnostics>
        <bindings>
            <netTcpBinding>
                <binding name="BConf" transferMode="Buffered">
                    <reliableSession ordered="true" inactivityTimeout="00:10:00" enabled="true" />
                </binding>
            </netTcpBinding>
        </bindings>
        <behaviors>
            <serviceBehaviors>
                <behavior name="UretimServisiBehavior">
                    <serviceDebug includeExceptionDetailInFaults="true" />
                </behavior>
            </serviceBehaviors>
        </behaviors>
        <services>
            <service behaviorConfiguration="UretimServisiBehavior" name="FabrikaLib.Uretici">
                <endpoint address="net.tcp://localhost:9000/Fabrika/UretimServisi.svc" binding="netTcpBinding" bindingConfiguration="BConf" name="UretimServisiEndPoint" contract="FabrikaLib.IUretici" />
            </service>
        </services>
    </system.serviceModel>
</configuration>
```

Mesajların izlenmesi için Windows SDK ile birlikte gelen Service Trace Viewer programına ihtiyaç vardır. İstemci ve servis uygulaması çalıştırılarak test edildikten sonra Service Trace Viewer yardımıyla mesajlaşma trafiği izlenebilir. İlk testin ardından C klasörü altında appmessages.svclog isimli bir dosya otomatik olarak oluşturulup içeriği doldurulacaktır. Dosya, Service Trace Viewer programı ile açıldığında aşağıdaki ekran görüntüsüne benzer bir şekilde 15 adet mesajın üretildiği görülecektir.

![mk229_8.gif](/assets/images/2007/mk229_8.gif)

Şimdi bu mesajlar kısaca analiz edilebilir. İlk mesajın içeriği aşağıdaki gibidir.

```xml
<MessageLogTraceRecord>
    <s:Envelope xmlns:s="http://www.w3.org/2003/05/soap-envelope" xmlns:a="http://www.w3.org/2005/08/addressing">
        <s:Header>
            <a:Action s:mustUnderstand="1"> http://schemas.xmlsoap.org/ws/2005/02/rm/CreateSequence</a:Action>
            <a:MessageID>urn:uuid:1d14de59-6695-4149-b218-cb41783db18d</a:MessageID>
            <a:To s:mustUnderstand="1">
net.tcp://localhost:9000/Fabrika/UretimServisi.svc</a:To>
        </s:Header>
        <s:Body>
            <CreateSequence xmlns="http://schemas.xmlsoap.org/ws/2005/02/rm">
                <AcksTo>
                    <a:Address> http://www.w3.org/2005/08/addressing/anonymous</a:Address>
                </AcksTo>
                <Offer>
                    <Identifier> urn:uuid:be73ae30-145a-4bd6-bf04-ee6a3b1edfce</Identifier>
                </Offer>
            </CreateSequence>
        </s:Body>
    </s:Envelope>
</MessageLogTraceRecord>
```

Bu mesaj ile istemci ve servis arasında güvenilir bir oturum başlatılmaktadır. Tahmin edileceği üzere mesaj istemci tarafından servise gönderilmiştir. Aynı güvenilir oturumda yer alan tüm mesajlar aynı benzersiz numara kümesini (Unique Identifier Set) kullanırlar. Bu anlamda ilk mesajın içerisinde yer alan MessageID değeri ikinci mesaj içerisinde de ele alınmaktadır. İkinci mesaj, servis tarafından istemciye gönderilen mesajdır ve içeriği aşağıdaki gibidir.

```xml
<MessageLogTraceRecord>
    <s:Envelope xmlns:s="http://www.w3.org/2003/05/soap-envelope" xmlns:a="http://www.w3.org/2005/08/addressing">
        <s:Header>
            <a:Actions:mustUnderstand="1"> http://schemas.xmlsoap.org/ws /2005/02/rm/CreateSequenceResponse</a:Action>
            <a:RelatesTo>urn:uuid:1d14de59-6695-4149-b218-cb41783db18d</a:RelatesTo>
            <a:To s:mustUnderstand="1">http://www.w3.org/2005/08/addressing/anonymous</a:To>
        </s:Header>
        <s:Body>
            <CreateSequenceResponse xmlns="http://schemas.xmlsoap.org/ws/2005/02/rm">
                <Identifier> urn:uuid:27079656-f1cf-460d-9289-28f43f664fbe</Identifier>
                <Accept>
                    <AcksTo>
                        <a:Address> 
net.tcp://localhost:9000/Fabrika/UretimServisi.svc</a:Address>
                    </AcksTo>
                </Accept>
            </CreateSequenceResponse>
        </s:Body>
    </s:Envelope>
</MessageLogTraceRecord>
```

İkinci mesajda üretilen Identifier elementinin değerinin, istemci tarafından sonradan gönderilecek mesajlarda mutlaka sağlanması gerekmektedir. Böylece aradaki mesajların aynı güvenilir oturum (Reliable Session) içerisinde olacağı anlaşılabilir. Buna göre istemci tarafından ilk metod çağrısı için gönderilen üçüncü mesaj içeriği aşağıdaki gibi olacaktır.

```xml
<MessageLogTraceRecord>
    <s:Envelope xmlns:s="http://www.w3.org/2003/05/soap-envelope"  xmlns:r="http://schemas.xmlsoap.org/ws/2005/02/rm" xmlns:a="http://www.w3.org/2005/08/addressing">
        <s:Header>
            <r:AckRequested>
                <r:Identifier> urn:uuid:27079656-f1cf-460d-9289-28f43f664fbe
</r:Identifier>
            </r:AckRequested>
            <r:Sequence s:mustUnderstand="1">
                <r:Identifier> urn:uuid:27079656-f1cf-460d-9289-28f43f664fbe
</r:Identifier>
                <r:MessageNumber>1</r:MessageNumber>
            </r:Sequence>
            <a:Action s:mustUnderstand="1"> 
http://www.bsenyurt.com/FabrikaLib/ UretimServisi/UretimServisi/BilesenAl</a:Action>
            <a:MessageID>urn:uuid:a6127691-9534-4f46-a3ff-97943cd19b30</a:MessageID>
            <a:ReplyTo>
                <a:Address> http://www.w3.org/2005/08/addressing/anonymous</a:Address>
            </a:ReplyTo>
            <a:To s:mustUnderstand="1"> 
net.tcp://localhost:9000/Fabrika/UretimServisi.svc</a:To>
        </s:Header>
        <s:Body>
            <BilesenAl xmlns="http://www.bsenyurt.com/FabrikaLib/UretimServisi">
                <bilesenAdi xmlns:b="http://schemas.microsoft.com/2003/10/Serialization/Arrays" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
                    <b:string>C</b:string>
                    <b:string>O2</b:string>
                    <b:string>H2SO4</b:string>
                </bilesenAdi>
            </BilesenAl>
        </s:Body>
    </s:Envelope>
</MessageLogTraceRecord>
```

Üçüncü mesaj BilesenAl isimli metod için bir çağrı olduğundan SOAP paketinin gövdesi (Body) içerisinde parametre değerleride gönderilmektedir. Diğer taraftan MessageNumber elementi ile gönderilen mesajın sıra numarasıda belirlenmiş olmaktadır. Üçüncü mesaj ile gelen metod çağrısına karşılık olaraktan servis tarafı bir cevap mesajını dördüncü mesaj olarak istemciye gönderecektir. Dördüncü mesajın içeriği aşağıdaki gibidir.

```xml
<MessageLogTraceRecord>
    <s:Envelope xmlns:s="http://www.w3.org/2003/05/soap-envelope" xmlns:r="http://schemas.xmlsoap.org/ws/2005/02/rm" xmlns:a="http://www.w3.org/2005/08/addressing">
        <s:Header>
            <r:SequenceAcknowledgement>
                <r:Identifier> urn:uuid:27079656-f1cf-460d-9289-28f43f664fbe</r:Identifier>
                <r:AcknowledgementRange Lower="1" Upper="1"></r:AcknowledgementRange>
                <netrm:BufferRemaining xmlns:netrm="http://schemas.microsoft.com/ws/2006/05/rm"> 8</netrm:BufferRemaining>
            </r:SequenceAcknowledgement>
            <r:AckRequested>
                <r:Identifier> urn:uuid:be73ae30-145a-4bd6-bf04-ee6a3b1edfce</r:Identifier>
            </r:AckRequested>
            <r:Sequence s:mustUnderstand="1">
                <r:Identifier> urn:uuid:be73ae30-145a-4bd6-bf04-ee6a3b1edfce</r:Identifier>
                <r:MessageNumber>1</r:MessageNumber>
            </r:Sequence>
            <a:Action s:mustUnderstand="1"> http://www.bsenyurt.com/FabrikaLib/ UretimServisi/UretimServisi/BilesenAlResponse</a:Action>
            <a:RelatesTo> 
urn:uuid:a6127691-9534-4f46-a3ff-97943cd19b30</a:RelatesTo>
            <a:To s:mustUnderstand="1"> http://www.w3.org/2005/08/addressing/anonymous</a:To>
        </s:Header>
        <s:Body>
            <BilesenAlResponse xmlns="http://www.bsenyurt.com/FabrikaLib/UretimServisi">
            <BilesenAlResult>3</BilesenAlResult>
            </BilesenAlResponse>
        </s:Body>
    </s:Envelope>
</MessageLogTraceRecord>
```

Bu mesaj içerisindede istemciye biraz önce gönderdiği mesajın başarılı bir şekilde alındığı ve onaylandığı bilgisi iletilmektedir. Sıradaki beşinci mesaj ile istemci servise bir onaylama bildirisi göndermektedir ve içeriği aşağıdaki gibidir.

```xml
<MessageLogTraceRecord>
    <s:Envelope xmlns:s="http://www.w3.org/2003/05/soap-envelope" xmlns:r="http://schemas.xmlsoap.org/ws/2005/02/rm" xmlns:a="http://www.w3.org/2005/08/addressing">
        <s:Header>
            <r:SequenceAcknowledgement>
                <r:Identifier> urn:uuid:be73ae30-145a-4bd6-bf04-ee6a3b1edfce</r:Identifier>
                <r:AcknowledgementRange Lower="1" Upper="1"> </r:AcknowledgementRange>
                <netrm:BufferRemaining  xmlns:netrm="http://schemas.microsoft.com/ws/2006/05/rm">8</netrm:BufferRemaining>
            </r:SequenceAcknowledgement>
            <a:Action s:mustUnderstand="1"> http://schemas.xmlsoap.org/ ws/2005/02/rm/SequenceAcknowledgement</a:Action>
            <a:To s:mustUnderstand="1"> 
net.tcp://localhost:9000/Fabrika/UretimServisi.svc</a:To>
        </s:Header>
    <s:Body></s:Body>
</s:Envelope>
</MessageLogTraceRecord>
```

Bu mesaj dördüncü mesajın ve içeriğinin istemci tarafından alındığını servis tarafına bildirmektedir. SequenceAcknowledgment elementinde yer alan identifier değerine bakılırsa birinci mesajda üretilen identifier ile aynı olduğu görülebilir. Hemen arkasından gelen altıncı mesaja bakıldığında istemcinin ikinci metod çağrısını gerçekleştirdiği görülür.

```xml
<MessageLogTraceRecord>
    <s:Envelope xmlns:s="http://www.w3.org/2003/05/soap-envelope" xmlns:r="http://schemas.xmlsoap.org/ws/2005/02/rm" xmlns:a="http://www.w3.org/2005/08/addressing">
        <s:Header>
            <r:Sequence s:mustUnderstand="1">
                <r:Identifier> urn:uuid:27079656-f1cf-460d-9289-28f43f664fbe</r:Identifier>
                <r:MessageNumber>2</r:MessageNumber>
            </r:Sequence>
            <a:Action s:mustUnderstand="1">  http://www.bsenyurt.com/FabrikaLib/UretimServisi/UretimServisi/Karistir</a:Action>
            <a:MessageID> urn:uuid:180ea9af-ddbc-4249-8880-40c412088ce3</a:MessageID>
            <a:ReplyTo>
                <a:Address> 
http://www.w3.org/2005/08/addressing/anonymous</a:Address>
            </a:ReplyTo>
            <a:To s:mustUnderstand="1"> 
net.tcp://localhost:9000/Fabrika/UretimServisi.svc</a:To>
        </s:Header>
        <s:Body>
            <Karistir xmlns="http://www.bsenyurt.com/FabrikaLib/UretimServisi"></Karistir>
        </s:Body>
    </s:Envelope>
</MessageLogTraceRecord>
```

İçeriktende görüldüğü gibi Karistir metoduna bir çağrı gelmektedir. Ancak burada önemli olan noktalardan biriside MessageNumber değeridir. Dikkat edilecek olursa 2 değeri gelmektedir. Bir başka deyişle bu mesajın ikinci sırada ele alınması gerektiği belirtilmiş olmaktadır. İstemci tarafından metod çağrıları sonucu oluşturulan mesajların aynı güvenilir oturumda yer alması ama farklı mesajlar olarak ele alınması MessageID değerleri sayesinde gerçekleştirilir. Bu nedenle burada üretilen MessageID değeri bir önceki metod çağrısında üretilenden farklı olarak belirlenmektedir. Altıncı mesaj için servisin ürettiği yedinci mesajın içeriği aşağıdaki gibidir.

```xml
<MessageLogTraceRecord>
    <s:Envelope xmlns:s="http://www.w3.org/2003/05/soap-envelope" xmlns:r="http://schemas.xmlsoap.org/ws/2005/02/rm" xmlns:a="http://www.w3.org/2005/08/addressing">
        <s:Header>
            <r:SequenceAcknowledgement>
                <r:Identifier> urn:uuid:27079656-f1cf-460d-9289-28f43f664fbe</r:Identifier>
                <r:AcknowledgementRange Lower="1" Upper="2"></r:AcknowledgementRange>
                <netrm:BufferRemaining xmlns:netrm="http://schemas.microsoft.com/ws/2006/05/rm"> 8</netrm:BufferRemaining>
            </r:SequenceAcknowledgement>
            <r:Sequence s:mustUnderstand="1">
                <r:Identifier> urn:uuid:be73ae30-145a-4bd6-bf04-ee6a3b1edfce</r:Identifier>
                <r:MessageNumber>2</r:MessageNumber>
            </r:Sequence>
            <a:Action s:mustUnderstand="1"> http://www.bsenyurt.com/FabrikaLib/UretimServisi/UretimServisi/KaristirResponse </a:Action>
            <a:RelatesTo> urn:uuid:180ea9af-ddbc-4249-8880-40c412088ce3</a:RelatesTo>
            <a:To s:mustUnderstand="1"> http://www.w3.org/2005/08/addressing/anonymous</a:To>
        </s:Header>
        <s:Body>
            <KaristirResponse xmlns="http://www.bsenyurt.com/FabrikaLib/UretimServisi"> </KaristirResponse>
        </s:Body>
    </s:Envelope>
</MessageLogTraceRecord>
```

Bu mesajlaşma trafiği diğer metod çağrısı içinde benzer şekilde işleyecektir. İstemcinin yaptığı metod çağrıları sona erdikten sonra ise, servis tarafına aşağıdaki mesaj gönderilir. (Örnek uygulamadaki çalışma sistemine göre Service Trace Viewer sonlandırma talebi onuncu mesaj olarak elde edilmektedir.)

```xml
<MessageLogTraceRecord>
    <s:Envelope xmlns:s="http://www.w3.org/2003/05/soap-envelope" xmlns:r="http://schemas.xmlsoap.org/ws/2005/02/rm" xmlns:a="http://www.w3.org/2005/08/addressing">
        <s:Header>
            <r:SequenceAcknowledgement>
                <r:Identifier> urn:uuid:be73ae30-145a-4bd6-bf04-ee6a3b1edfce</r:Identifier>
                <r:AcknowledgementRange Lower="1" Upper="3"></r:AcknowledgementRange>
                <netrm:BufferRemaining xmlns:netrm= "http://schemas.microsoft.com/ws/2006/05/rm">8</netrm:BufferRemaining>
            </r:SequenceAcknowledgement>
            <r:Sequence s:mustUnderstand="1">
                <r:Identifier> urn:uuid:27079656-f1cf-460d-9289-28f43f664fbe</r:Identifier>
                <r:MessageNumber>4</r:MessageNumber>
                <r:LastMessage></r:LastMessage>
            </r:Sequence>
            <a:Action s:mustUnderstand="1"> http://schemas.xmlsoap.org/ws/2005/02/rm/LastMessage
</a:Action>
            <a:To s:mustUnderstand="1"> net.tcp://localhost:9000/Fabrika/UretimServisi.svc</a:To>
        </s:Header>
        <s:Body></s:Body>
    </s:Envelope>
</MessageLogTraceRecord>
```

Son mesaj olduğunun bildirimi için r adındaki xml isim alanındaki (Xml Namespace) LastMessage elementi kullanılır. Bununla birlikte Action elementi içerisinde yapılan çağrıda LastMessage bildirimi yapılır. Bu mesajın oluşması için istemcinin oturumu kapatıyor olması gerekmektedir ki geliştirilen örnekte zaten IsTerminating olarak işaretlenmiş metod çağrısından sonra açık olan oturum (Session) kapatılma sürecine girecektir.

Herşey bittikten sonra üretilen mesajlarda yer alan TerminateSequence elementleri içerisinde yer alan Identifier değerleri ile istemci ve servis birbilerine artık kaynaklarını kullanmayacaklarını ve güvenilir oturumu (Reliable Session) kapatacakları bilgilerini vermektedirler. Yukarıdaki işleyiş aşağıdaki şekil ilede değerlendirilebilir. (Mesajlar üzerinden bu tarz bir görseli hazırlamak oldukça zorlayıcı olmuştur. Bu nedenle gözden kaçan noktalar söz konusu olabilir. Asıl dayanak noktası Service Trace Viewer programının ürettiği Message içerikleri olmalıdır.)

Mesaj 1 ile Mesaj 5 arası durum;

![mk229_9.gif](/assets/images/2007/mk229_9.gif)

Mesaj 5 ile Mesaj 9 arası durum;

![mk229_10.gif](/assets/images/2007/mk229_10.gif)

Mesaj 9 ile Mesaj 15 arası durum;

![mk229_11.gif](/assets/images/2007/mk229_11.gif)

Elbette geliştirici olarak arka tarafta hareket eden mesajların içerikleri çok önemli olmayabilir. Ancak güvenilir oturumlarda söz konusu olan bir dezavantaj vardır. Buda örnektende görüleceği üzere ağ üzerinde istemci ile servis arasında meydana gelen ekstra mesaj yüküdür. Dolayısıyla bu ekstra mesaj yükü, özellikle çok fazla istemcinin olduğu sistemlerde performans kaybı yaşatmaktadır. Dolayısıyla güvenilir oturumları (Reliable Session) kullanmadan önce gerekliliklerin ortaya konması doğru bir çözümsel yaklaşım olacaktır.

Güvenilir oturumlarda mesajlar için benzersiz ve tekrar etmeyen tanımlayıcı değerler kullandığından cevaplama saldırılarının (Reply Attack) azaltılmasıda söz konusudur. Yinede güvenilir oturumlar açılması cevaplama saldırıları için yeterli bir savunma mekanizması sunmaz. Kesin çözüm için özel bir bağlayıcı kullanmak gerekir. Özel bir bağlayıcı yardımıyla cevaplama saldırılarına (Reply Attack) karşı nasıl ayakta kalınabileceğini bir sonraki makalemizde incelemeye çalışıyor olacağız.

Böylece geldik uzun bir makalemizin daha sonuna. Bu makalemizde istemci ve servis arasında güvenilir bir oturumun nasıl sağlanabileceğini ele alırken arada hareket eden mesajlarıda analiz etmeye gayret ettik. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](/assets/files/2007/ReliableMessagingAndWCF.rar)