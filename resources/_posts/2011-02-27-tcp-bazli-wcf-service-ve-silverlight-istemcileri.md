---
layout: post
title: "TCP Bazlı WCF Service ve Silverlight İstemcileri"
date: 2011-02-27 16:10:00 +0300
categories:
  - silverlight-4-0
  - wcf-4-0
tags:
  - nettcpbinding
  - windows-communication-foundation
  - wcf-services
  - Silverlight
  - silverlight
---
Bir yazar, hazırlayacağı hikaye için çoğu zaman çevrede dolaşıp malzeme toplar. Olayın kahramanlarını tasvir etmek için çevredeki insanları göz önüne alır. Hatta gezdiği yerleri inceler. Bu açıdan bakıldığında iyi yazarların aslında çok iyi birer gözlemci olduğu söylenebilir.

[![blg218_Giris](/assets/images/2011/blg218_Giris_thumb_1.jpg)](/assets/images/2011/blg218_Giris_1.jpg)


Sonuç olarak yazarın elinde bir senaryo taslağı oluşur. Artık tek yapması gereken sakin bir köşe bulmak ve daktilosunun başına geçerek (ki günümüde büyük bir olasılıkla bu diz üstü bir bilgisayar olacaktır) yazmaya başlamaktır. Çözülmesi en zor olan parçaların başında kitaba bir isim bulmak ve ilk giriş cümlesini yazmak gelmektedir. Her ne kadar bu güne kadar yazılmış bir kitabım olmasa da böyle olduğunu tahmin etmekteyim.

Bugün yazımız içinde elimizde bir takım malzemelerimiz bulunmakta. Bir adet TCP bazlı olarak çalışan WCF (Windows Communication Foundation) servisi. Bu servisi kullanan Silverlight 4.0 tabanlı bir istemci. TCP bazlı servisimiz son derece zıpkın bir delikanlı aslında. Nitekim şirketin iç ağı üzerinden Binary tabanlı mesaj formatını kullandığı için ondan daha hızlısı neredeyse yok gibi. Diğer yandan Silverlight istemcimiz son derece yakışıklı ve zengin bir kız (Rich Internet Application). İşte bu yazımızda bu iki kişiyi buluşturmaya çalışıyor olacağız. Ne varki arada zıpkın delikanlının bir de ablası var ki o da IIS (Internet Information Services) mahallesinde oturmayı istemeyen ama hep hayal eden Self-Hosted stilde yazılmış bir Console uygulaması. Neredeyse içi kap kara olmuş birisi (Ama biz çalışma zamanı ekranında onun içindeki iyiliği beyaza boyayıp çıkartacağız) Bakalım abla, kızın, erkek arkadaşına ulaşmasına izin verecek mi?

![Laughing](/assets/images/2011/smiley-laughing.gif)

## Varsayılan Olarak

Normal şartlar altında Silverlight istemcilerinin genellikle HTTP bazlı çalışan WCF servislerini kullanması söz konusudur. Hatta WCF RIA Services’ ler en sık kullanılanıdır. Ancak Intranet tabanlı bir sistemde TCP bazlı WCF Servisleri de söz konusu olabilir. Dilerseniz olayı örnekleyerek canlandırmaya çalışalım. İlk olarak elimizin altında aşağıdaki gibi bir WCF Service Library içeriğinin olduğunu düşünelim.

[![blg218_ServiceClassDiagram](/assets/images/2011/blg218_ServiceClassDiagram_thumb_1.gif)](/assets/images/2011/blg218_ServiceClassDiagram_1.gif)

Servis sözleşmemiz oldukça basit bir içeriğe sahip.

```csharp
using System.ServiceModel;

namespace YourMeetingServices 
{ 
    [ServiceContract] 
    public interface IMeetingService 
    { 
        [OperationContract] 
        string FirstHello(string yourName); 
    } 
}
```

Sadece bir Merhaba demek için gerekli operasyon tanımını içermekte. Bu operasyon ise aşağıdaki gibi uygulanmakta.

```csharp
namespace YourMeetingServices 
{ 
    public class MeetingService 
       : IMeetingService 
    { 
        #region IMeetingService Members

        public string FirstHello(string yourName) 
        { 
            return string.Format("Merhaba {0}", yourName); 
        }

        #endregion 
    } 
}
```

Bu servis kütüphanesini referans ederek host eden Console uygulamasının içeriği ise aşağıdaki gibi.

```csharp
using System; 
using System.ServiceModel; 
using YourMeetingServices;

namespace ServerApp 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            ServiceHost host = new ServiceHost( 
                typeof(MeetingService) 
                ); 
            host.Opened += (o, e) => 
                { 
                    Console.WriteLine("Servis dinlemede"); 
                }; 
            host.Closed += (o, e) => 
                { 
                    Console.WriteLine("Servis kapatıldı"); 
                }; 
            host.Open(); 
            Console.WriteLine("Çıkmak için bir tuşa basınız"); 
            Console.ReadLine(); 
            host.Close(); 
        } 
    } 
}
```

Çok doğal olarak config dosyası içeriğinin de aşağıdaki gibi olduğunu söyleyebiliriz.

```xml
<?xml version="1.0" encoding="utf-8" ?> 
<configuration> 
    <system.serviceModel> 
        <behaviors> 
            <serviceBehaviors> 
                <behavior> 
                    <serviceMetadata/> 
                </behavior> 
            </serviceBehaviors> 
        </behaviors> 
        <services> 
            <service name="YourMeetingServices.MeetingService"> 
                <endpoint address="" binding="netTcpBinding" contract="YourMeetingServices.IMeetingService"/> 
                <endpoint address="mex" binding="mexTcpBinding" contract="IMetadataExchange" /> 
                <host> 
                    <baseAddresses> 
                        <add baseAddress="net.tcp://localhost:8342/MeetingRoom/MeetingService/" /> 
                    </baseAddresses> 
                </host> 
            </service> 
        </services> 
    </system.serviceModel> 
</configuration>
```

Buna göre servisimiz TCP bazlı olaraktan net.tcp://localhost:8342/MeetingRoom/MeetingService/ adresi üzerinden yayın yapmaktadır. Ayrıca mexTcpBinding bağlayıcı tipini kullanan ve Mex son ekini var olan Base Address tanımına ekleyerek, service metadata publishing yapan bir EndPoint’ e de sahiptir.

Buna göre basit bir Console istemcisinin söz konusu servisi kullanması için tek yapması gereken, Servis uygulaması çalışırken Add Service Reference seçeneğinde aşağıdaki şekilde görülen adres tanımlamasını kullanmak olacaktır.

[![blg218_AddServiceReference](/assets/images/2011/blg218_AddServiceReference_thumb_1.gif)](/assets/images/2011/blg218_AddServiceReference_1.gif)

Buna göre sıradan bir istemcinin söz konusu servisi kullanması mümkün ve kolaydır.

## İlk Tanışma

Ancak söz konusu istemci bir Silverlight uygulaması ise biraz daha sıkıntılı bir durumla karşı karşıya olabiliriz. Dilerseniz bu durumu analiz etmek için yukarıda yazmış olduğumuz servisimizi bir Silverlight istemcisine referans etmeye çalışalım. Tabiki Sunucu uygulamanın çalışıyor olması gerektiğini hatırlatmayacağım.

Sunucu uygulama çalışıyor ve servis iletişime açık iken, Silverlight uygulamasına referans ekleme işlemi başarılı olacaktır. Ancak aşağıdaki şekildeki gibi iki adet Warning’ in de oluştuğu görülecektir.

[![blg218_Warnings](/assets/images/2011/blg218_Warnings_thumb_1.gif)](/assets/images/2011/blg218_Warnings_1.gif)

Üstelik üretilen config dosyası içeriğine bakıldığında aşağıdaki görüntü ile karşılaşılır.

[![blg218_Config](/assets/images/2011/blg218_Config_thumb_1.gif)](/assets/images/2011/blg218_Config_1.gif)

Uppsss!!! İlginç bir durum. Nitekim Endpoint üretimlerinin yapılmaması bir yana, WCF servisi ile olan iletişim için gerekli hiç bir ayar da bulunmamaktadır. Buna göre Abla’ nın, iletişimi engellediğini ifade edebiliriz.

## İkinci Karşılaşma

İlk karşılaşmanın başarısızlığı üzerine servis ile ilişkili olarak bir takım ilkelerin uygulanması gerektiğini söyleyebiliriz. Her şeyden önce sunucunun, Silverlight istemcilerinin HTTP bazlı olarak 80 portu üzerinden erişebilmelerine izin veriyor olması gerektiği ip ucunu verebiliriz. Bir başka deyişle Cross Domain için ClientAccessPolicy.xml ve doğru içeriğin uygulanması gerekliliği söz konusudur. Bu amaçla servis kütüphanemize aşağıdaki servis sözleşmesini eklediğimizi düşünelim.

```csharp
using System.IO; 
using System.ServiceModel; 
using System.ServiceModel.Web;

namespace YourMeetingServices 
{ 
    [ServiceContract] 
    public interface ITCPPolicy 
    { 
        [OperationContract, WebGet(UriTemplate = "/clientaccesspolicy.xml")] 
        Stream GetPolicy(); 
    } 
}
```

Burada WebGet niteliğinin de uygulandığı (ki bunun için System.ServiceModel.Web.dll assembly’ ının projeye referans edilmesi gerekir) bir operasyon yer almaktadır. Bu operasyon geriye Stream tipi tarafından taşınabilen bir referans döndürmektedir. Söz konusu operasyona ulaşılırken HTTP Get metodunda Clientaccesspolicy.xml son ekinin kullanılacağı da ifade edilmektedir. Bir başka deyişle bu sözleşme istemci için gerekli olan ClientAccessPolicy içeriğini sunacaktır. Çok doğal olarak bu sözleşmenin ilgili tipe uygulanıyor olması gerekmektedir. Aynen aşağıda görüldüğü gibi.

```csharp
using System; 
using System.IO; 
using System.ServiceModel.Web; 
using System.Text;

namespace YourMeetingServices 
{ 
    public class MeetingService 
        : IMeetingService,ITCPPolicy 
    { 
        #region IMeetingService Members

        public string FirstHello(string yourName) 
        { 
            return string.Format("Merhaba {0}", yourName); 
        }

        #endregion

        #region ITCPPolicy Members

        public Stream GetPolicy() 
       { 
            string content = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "PolicyContent.xml")); 
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/xml"; 
           return new MemoryStream(Encoding.UTF8.GetBytes(content)); 
        }

        #endregion 
    } 
}
```

Yapılan bu değişiklikler sonucuda servis kütüphanesinin içeriğinin özetle aşağıdaki şekile görüldüğü gibi olduğunu ifade edebiliriz.

[![blg218_ClassDiagram2](/assets/images/2011/blg218_ClassDiagram2_thumb_1.gif)](/assets/images/2011/blg218_ClassDiagram2_1.gif)

GetPolicy metodunun uygulanışı içerisindeki en önemli nokta sır gibi duran content değişkeninin değeridir. Bu değer içeriği aşağıdaki gibi olan PolicyContent.xml dosyasından getirilmektedir. Söz konusu dosyanın output klasörü sunucu uygulamaya ait exe çıktısının olduğu yer olarak belirtilmiştir.

```xml
<?xml version="1.0" encoding="utf-8"?> 
<access-policy> 
  <cross-domain-access> 
    <policy> 
      <allow-from http-request-headers="*"> 
        <domain uri="*"/> 
      </allow-from> 
      <grant-to> 
        <socket-resource port="8342" protocol="tcp" /> 
      </grant-to> 
    </policy> 
  </cross-domain-access> 
</access-policy>
```

Dikkat edileceği üzre TCP bazlı 8342 numaralı port için garanti verilmiştir. Tabi buna göre sunucu uygulama üzerinde de bir takım değişikliklerin yapılması gerekmektedir. Aslında App.config dosyası içerisinde aşağıdaki değişiklikleri yapmamız yeterli olacaktır.

```xml
<?xml version="1.0"?> 
<configuration> 
    <system.serviceModel> 
        <bindings> 
            <netTcpBinding> 
                <binding name="TcpBindingConfiguration"> 
                    <security mode="None" /> 
                </binding> 
            </netTcpBinding> 
        </bindings> 
        <behaviors> 
            <endpointBehaviors> 
                <behavior name="WebBehavior"> 
                    <webHttp /> 
                </behavior> 
            </endpointBehaviors> 
            <serviceBehaviors> 
                <behavior name=""> 
                    <serviceMetadata /> 
                </behavior> 
            </serviceBehaviors> 
        </behaviors> 
        <services> 
            <service name="YourMeetingServices.MeetingService"> 
                <endpoint address="" binding="netTcpBinding" bindingConfiguration="TcpBindingConfiguration" 
                    name="TcpEndpoint" contract="YourMeetingServices.IMeetingService" /> 
                <endpoint address="mex" binding="mexTcpBinding" name="MexTcpEndpoint" 
                    contract="IMetadataExchange" /> 
                <endpoint address="" behaviorConfiguration="WebBehavior" binding="webHttpBinding" 
                    name="WebHttpEndpoint" contract="YourMeetingServices.ITCPPolicy" /> 
                <host> 
                    <baseAddresses> 
                        <add baseAddress="http://localhost:8080" /> 
                        <add baseAddress="net.tcp://localhost:8342/MeetingRoom/MeetingService" /> 
                    </baseAddresses> 
                </host> 
            </service> 
        </services> 
    </system.serviceModel> 
<startup><supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.0"/></startup></configuration>
```

İlk dikkati çeken nokta WebHttp davranışını destekleyen WebHttpBinding bağlayıcı tipini kullanan ve ITCPPolicy sözleşmesini sunan bir Endpoint’ in eklenmiş olmasıdır. Üstelik bu Endpoint, base olarak http://localhost:8080 adresini kullanmaktadır.

Diğer yandan TCP Bazlı iletişim sağlayan Endpoint için security mode değeri none olarak işaret edilmiştir. Bu durumda Silverlight uygulamamızda net.tcp://localhost/8342/MeetingRoom/MeetingService/mex adresi üzerinden yapılan referans ekleme işlemi sonrasında, daha önceden aldığımız Warning mesajlarının kalktığı ve aşağıdaki istemci config içeriğinin oluştuğu görülecektir.

```xml
<configuration> 
    <system.serviceModel> 
        <bindings> 
           <customBinding> 
                <binding name="TcpEndpoint"> 
                    <binaryMessageEncoding /> 
                    <tcpTransport maxReceivedMessageSize="2147483647" maxBufferSize="2147483647" /> 
                </binding> 
            </customBinding> 
        </bindings> 
        <client> 
            <endpoint address="net.tcp://localhost:8342/MeetingRoom/MeetingService" 
               binding="customBinding" bindingConfiguration="TcpEndpoint" 
                contract="MeetingSpace.IMeetingService" name="TcpEndpoint" /> 
        </client> 
    </system.serviceModel> 
</configuration>
```

Artık Silverlight istemcisi üzerinden bir test gerçekleştirebiliriz. Bu amaçla MainPage içeriğini aşağıdaki gibi oluşturduğumuzu düşünelim.

[![blg218_DesignTime](/assets/images/2011/blg218_DesignTime_thumb_1.gif)](/assets/images/2011/blg218_DesignTime_1.gif)

MainPage.xaml.cs

```csharp
using System; 
using System.Windows; 
using System.Windows.Controls; 
using SilverApp.MeetingSpace;

namespace SilverApp 
{ 
    public partial class MainPage : UserControl 
    { 
        MeetingServiceClient proxy = null;

        public MainPage() 
        { 
            InitializeComponent(); 
            proxy = new MeetingServiceClient("TcpEndpoint"); 
            proxy.FirstHelloCompleted += new EventHandler<FirstHelloCompletedEventArgs>(proxy_FirstHelloCompleted); 
        }

        void proxy_FirstHelloCompleted(object sender, FirstHelloCompletedEventArgs e) 
        { 
            if (e.Error != null) 
                lblCallResult.Content = "Bir sorun oluştu"; 
            else if (e.Cancelled) 
                lblCallResult.Content = "İşlem iptal edildi"; 
            else 
               lblCallResult.Content = e.Result;                
        }

        private void btnCall_Click(object sender, RoutedEventArgs e) 
        { 
            proxy.FirstHelloAsync(txtYourname.Text); 
        } 
    } 
}
```

Görüldüğü üzere, config dosyasında TcpEndpoint için tanımlanmış olan ayarlara göre ilgili WCF servisine asenkron olarak bir çağrı gerçekleştirilmektedir.

Şimdi ilk testimizi yapalım. Önce sunucu uygulamayı ardından da Silverlight Web uygulamamızı çalıştıralım. İlk etapta her şey güllük gülistanlıktır. Aynen aşağıdaki şekilde görüldüğü gibi.

[![blg218_FirstRuntime](/assets/images/2011/blg218_FirstRuntime_thumb_1.gif)](/assets/images/2011/blg218_FirstRuntime_1.gif)

Ancak Call başlıklı Button kontrolüne bastığımızda aşağıda görülen çalışma zamanı istisnasını (Runtime Exception) aldığımızı görürüz.

[![blg218_FirstRuntimeException](/assets/images/2011/blg218_FirstRuntimeException_thumb_1.gif)](/assets/images/2011/blg218_FirstRuntimeException_1.gif)

Aslında buradaki hata mesajının tam içeriği şöyledir;

> Could not connect to net.tcp://localhost:8342/MeetingRoom/MeetingService. The connection attempt lasted for a time span of 00:00:00.2250225. TCP error code 10013: An attempt was made to access a socket in a way forbidden by its access permissions.. This could be due to attempting to access a service in a cross-domain way while the service is not configured for cross-domain access. You may need to contact the owner of the service to expose a sockets cross-domain policy over HTTP and host the service in the allowed sockets port range 4502-4534.

Anlaşılacağı üzere bir Cross Domain Policy sorunsalı baş göstermiştir gibi durmaktadır.

## Son Karşılaşma

Aslında Servisin geliştirilme mantığına göre IIS’ in olmadığı bir durum simüle edilmektedir. Nitekim Siverlight istemcilerinin TCP bazlı bir servisi tüketmesi için bu servisin IIS üzerinde host edilmesi ve ClientAccessPolicy.xml dosyası ile desteklenerek gerekli güvenlik izinlerinin verilmesi yeterlidir. Dolayısıyla bizim geliştirdiğimiz senaryoda Servis uygulamasının IIS gibi çalıştığı varsayılabilir. Buna göre ilk olarak HTTP base address tanımlamasının http://localhost:80 şeklinde değiştirilmesi düşünülmelidir.

Diğer yandan çalışma zamanındaki hata mesajı 4502 ile 4534 numaralı portlar arasında bir değerin kullanılmasını beklemektedir. Bu sebepten servis tarafındanki TCP based address değerinin de örnek olarak net.tcp://localhost:4505/MeetingRoom/MeetingService şeklinde değiştirilmesi düşünülebilir. Yani 4502 ile 4534 arasında bir port değeri atanmalıdır. Ancak bu da yeterli olmayacaktır. Nitekim policy içeriğini teşkil eden XML dosyasında yer alan port numarası da 4505 olarak ayarlanmalıdır. Tüm bu değişiklikler Silverlight istemcisine servis referansının yeniden eklenmesini gerektirecektir.

[![blg218_AddServiceReferenceLast](/assets/images/2011/blg218_AddServiceReferenceLast_thumb.gif)](/assets/images/2011/blg218_AddServiceReferenceLast.gif)

Artık tanışmak için son bir deneme yapılabilir. İşte sonuç.

[![blg218_Final](/assets/images/2011/blg218_Final_thumb.gif)](/assets/images/2011/blg218_Final.gif)

Görüldüğü üzere servis tarafındaki metod başarılı bir şekilde çalışmıştır. Peki bu kadar zahmete girmeye gerek var mıdır? Aslında olmadığını söylersem şu anda bana çok kızabileceğinizi düşünüyorum. Ancak var. Nitekim IIS (Internet Information Services) üzerinde WAS (Windows Process Activation Service) kullanımı sayesinde host edebileceğimiz TCP bazlı bir servisin, doğru ClientAccessPolicy.xml içeriği ile bir Silverlight istemcisi tarafından kullanılabilmesi mümkündür. Şu anda umuyorum ki içinizde @#$½!:=|<> gibi bir şey demiyorsunuzdur

![Embarassed](/assets/images/2011/smiley-embarassed.gif)

Tabi yapılan örneğe göre kafalarda hale soru işaretleri oluşabilir. Söz gelimi 80 yerine örneğin 4508 numaralı bir port üzerinden iletişim geçerli olsa (http://localhost:4508 şeklinde) Yine de hatalar ile karşılaşır mıyız acaba?

![Wink](/assets/images/2011/smiley-wink.gif)

Bu sorunun araştırılmasını siz değerli okurlarıma bırakıyorum. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[TCPandSilverlight.rar (1,80 mb)](/assets/files/2011/TCPandSilverlight.rar) [Örnek Visual Studio 2010 Ultimate üzerinde ve Silverlight 4.0 odaklı olarak geliştirilmiştir]