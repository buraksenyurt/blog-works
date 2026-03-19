---
layout: post
title: "WCF 4.5–Built-In UDP Desteği"
date: 2015-01-17 23:00:00 +0300
description: "TCP/IP protokolünün katmanlarını incelediğimizde sırasıyla Ağ(Network), Internet, İletişim ve Uygulama katmanlarından oluştuğunu görürüz. İletişim katmanında TCP ve UDP(User Datagram Protocol) olmak üzere iki farklı protokol seçeneği bulunmaktadır. UDP protokolü, minimum kabiliyet ile hızlı bir iletişimin kullanılabilmesine olanak tanımaktadır. UDP, Transaction yönlendirmeli bir protokoldür ve TCP gibi mesajın teslim edildiğine dair bir garanti beklememektedir. Bu kontrolsüzlük, onun hızlı olmasının nedenlerinden de bir tanesidir. Diğer yandan, bu iletişim şekli nedeni ile verinin doğru iletilip iletilmediğini de kontrol etmemektedir. Pek tabi hız için söz konusu olan bu ödün vermeler güvenli olmayan bir iletişimin de doğal sonucudur."
categories:
  - wcf-4-5
tags:
  - wcf-4-5
  - csharp
  - xml
  - dotnet
  - wcf
  - soap
  - http
  - performance
  - transactions
  - generics
---
# WCF 4.5–Built-In UDP Desteği
[![70050](/assets/images/2015/70050_thumb.jpg)](/assets/images/2015/70050.jpg) Merhaba Arkadaşlar,

> Hızzzzz!!! Ben Hızzımmm! Hızlıdan hızlı…

Bu repliği bu aralar haftada en az 7 kere seyretmek zorunda kaldığım Cars filminden hatırlıyorum (Pixar’ ın efsane çizgi filmlerinden birisi olmakla birlikte serinin 2nci filmi de süperdir. 3ncü çekilir mi bilemem ama çekilse harika olur)

Şimşek McQueen filmin baş ve ana karakteri olarak çok hızlı bir arabadır ve tek derdi çok daha hızlı gitmektir. Hatta serinin ikinci bölümüne kapıştığı Formula 1 arabası dahil pek çok çeşitteki yarış otomobilini sürekli geride bırakır. Bunların arasında ralli araçlarından tutun, LeMans’ da yarışlanlara kadar pek çok çeşit vardır.

Aslında network ortamında da ona benzer bir kahraman bulunmaktadır. Onun gibi biraz güvenlikten ödün verir ama çok da hızlıdır. İşte bu yazımızda McQueen’ in network üzerindeki en büyük rakibi olan UDP protokolünü ve onun WCF 4.5 tarafındaki kullanımını incelemeye çalışıyor olacağız.

User Datagram Protocol

TCP/IP protokolünün katmanlarını incelediğimizde sırasıyla Ağ (Network), Internet, İletişim ve Uygulama katmanlarından oluştuğunu görürüz. İletişim katmanında TCP ve UDP (User Datagram Protocol) olmak üzere iki farklı protokol seçeneği bulunmaktadır. UDP protokolü, minimum kabiliyet ile hızlı bir iletişimin kullanılabilmesine olanak tanımaktadır.

UDP, Transaction yönlendirmeli bir protokoldür ve TCP gibi mesajın teslim edildiğine dair bir garanti beklememektedir. Bu kontrolsüzlük, onun hızlı olmasının nedenlerinden de bir tanesidir. Diğer yandan, bu iletişim şekli nedeni ile verinin doğru iletilip iletilmediğini de kontrol etmemektedir. Pek tabi hız için söz konusu olan bu ödün vermeler güvenli olmayan bir iletişimin de doğal sonucudur.

Dolayısıyla UDP protokolünün kullanım alanları TCP protokolününkine göre biraz daha farklıdır. Özellikle Wide Arena Network (WAN) tipindeki ağlarda, sesli ve görüntülü bilgi aktarımının çok sayıdaki istemciye, simultane olarak yüksek hızlarda aktarılabilmesi gibi vakalarda tercih edilmektedir. Nitekim bu hallerde sadece verinin karşı tarafta bekleyen istemcilere olabildiğince hızlı biçimde akması önemlidir.

> Ancak yine de dikkat edilmesi gereken bir durum vardır. UDP ve TCP, TCP/IP’ nin iletişim katında yer alan iki protokoldür. Bu sebepten ikisini aynı anda kullanabilmek de mümkündür. Böyle bir senaryoda, TCP’ nin yüksek veri transferi nedeniyle UDP’ nin performansı düşebilir.

WCF 4.5 Desteği

Gelelim.Net Framework tarafına. Windows Communication Foundation 4.5 sürümü ile birlikte, UDP protokolü için Built-In bir bağlayıcı tip (Binding Type) desteği sunmaya başlamıştır. Dolayısıyla UDP tabanlı servislerin yazılması çok daha kolay hale gelmiştir. Biz bu yazımızda, hem UdpBinding tipinin nasıl kullanıldığını görmeye çalışacak, hem de UDP ile TCP, Basic HTTP ve WS-HTTP gibi sık kullanılan mesajlaşma protokollerini hız bazında karşılaştıracağız. UDP’ nin daha verimli ve performanslı olacağını düşünebiliriz. Özellikle tek yönlü iletişimde. Ama bunu ispat etmemiz ve sonuçları irdelememiz gerekiyor. Nitekim sürprizler olabilir.

> UDP nin WCF tarafındaki kullanımında dikkat edilmesi gereken noktalar şunlardır:
> MSDN’ deki bu adresten

Örnek Senaryo

Konuyu daha iyi anlayabilmek için basit bir çözüm üzerinden ilerlemeye çalışabiliriz. Solution içeriğimizde 3 farklı proje bulunacaktır. Bunlardan birisi WCF Servis kütüphanesi, diğeri bu servis kütüphanesini kullanan Host uygulaması ve 3ncüsü de istemci program. Servis ile istemci tarafındaki haberleşme de UDP, TCP ve HTTP bazlı iletişimi tercih ediyor olacağız. Amacımız UDP kullanımı ve performans testi olduğundan tek yönlü (OneWay) çalışan ve sadece basit bir string içeriği sunucuya gönderebilmemizi sağlayan bir servis sözleşmesi (Service Contract) tasarlıyor olacağız. Kabaca gerçekleştirmeyi planladığımız senaryo aşağıdaki şekilde görüldüğü gibidir.

[![udpwcf_2](/assets/images/2015/udpwcf_2_thumb.png)](/assets/images/2015/udpwcf_2.png)

Servis Kütüphanesinin Geliştirilmesi

Dilerseniz ilk olarak WCF servis kütüphanesini (WCF Service Library) tasarlayalım.

[![udpwcf_5](/assets/images/2015/udpwcf_5_thumb.png)](/assets/images/2015/udpwcf_5.png)

IEchoService sözleşmesi aşağıdaki kod içeriğine sahiptir.

```csharp
using System.ServiceModel;

namespace HighwayServiceLibrary 
{ 
    [ServiceContract] 
    public interface IEchoService 
    { 
        [OperationContract(IsOneWay=true)] 
        void SendEcho(string content); 
    } 
}
```

Burada dikkat edilmesi gereken nokta hız testi için SendEcho isimli servis operasyonunun OneWay çalışacak şekilde nitelendirilmiş olmasıdır. Buna göre istemciler SendEcho’ dan bir dönüş beklemeyecek ve sadece sürekli veri gönderimi işlemini gerçekleştireceklerdir.

> Hatırlayacağınız gibi OneWay, asenkron servis çağırma tekniklerinden birisidir.

Servis sözleşmesine ait implementasyon ise aşağıdaki gibidir.

```csharp
namespace HighwayServiceLibrary 
{ 
    public class EchoService 
        :IEchoService 
    { 
        public void SendEcho(string content) 
        { 
            //Do Something 
        } 
    } 
}
```

Eeee bu kod hakkında söylenecek pek fazla bir şey yok aslında

![Smile](/assets/images/2015/wlEmoticon-smile_57.png)

Servis Tarafının Geliştirilmesi

Servis tarafını yazarak ilerleyelim. Console uygulaması olarak tasarlayacağımız host programın en önemli özelliği birden fazla EndPoint üzerinden hizmet verecek olmasıdır. Bu nedenle App.config dosyası içeriği oldukça önemlidir.

```xml
<?xml version="1.0" encoding="utf-8" ?> 
<configuration> 
  <system.serviceModel> 
    <behaviors> 
      <serviceBehaviors> 
        <behavior name="StandartBehavior"> 
          <serviceMetadata/> 
          <serviceDebug includeExceptionDetailInFaults="true" /> 
        </behavior> 
      </serviceBehaviors>      
    </behaviors> 
    <services>      
      <service name="HighwayServiceLibrary.EchoService" behaviorConfiguration="StandartBehavior"> 
        <endpoint address="http://localhost:54160/basic/EchoService" binding="basicHttpBinding" contract="HighwayServiceLibrary.IEchoService"/> 
        <endpoint address="http://localhost:54160/ws/EchoService" binding="wsHttpBinding" contract="HighwayServiceLibrary.IEchoService"/> 
        <endpoint address="http://localhost:54160/EchoService/mex" binding="mexHttpBinding" contract="IMetadataExchange"/> 
        <endpoint address="soap.udp://localhost:54162/EchoService" binding="udpBinding" contract="HighwayServiceLibrary.IEchoService"/> 
        <endpoint address="net.tcp://localhost:54161/EchoService" binding="netTcpBinding" contract="HighwayServiceLibrary.IEchoService"/> 
      </service>      
    </services> 
  </system.serviceModel> 
</configuration>
```

Dikkat edileceği üzere Basic HTTP (SOAP 1.1 oluyor), WS HTTP, TCP ve UDP destekli EndPoint tanımlamaları yapılmıştır. Bunlara ek olarak istemci tarafının servise ait metadata bilgisini çekebilmesi için de bir Metadata Exchange erişim noktası ilave edilmiştir.

> Hatırlayalım!
> Metadata paylaşımı için kullanılan sözleşme tipi.Net Framework içerisinde Built-In olarak gelen IMetadataExchange arayüzünde (Interface) tanımlanmıştır.

Host uygulamaya ait kod içeriği ise şu şekildedir.

```csharp
using HighwayServiceLibrary; 
using System; 
using System.ServiceModel;

namespace ServerApp 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            using(ServiceHost host=new ServiceHost(typeof(EchoService))) 
            { 
                host.Open(); 
                Console.WriteLine(@"Uygulama sunucusunu durumu: ""{0}"" ",host.State); 
                Console.WriteLine("Uygulama sunucusunu kapatmak için bir tuşa basınız."); 
                Console.ReadLine(); 
                host.Close(); 
            } 
        } 
    } 
}
```

Standart olarak basit bir Self-Host uygulama kodu görülmektedir. Çok doğal olarak istemcilerin servisi kullanabilmesi için uygulamanın çalışır halde olması ve host bağlantısının açık olması (Host.State=Open) önemlidir.

> İstemci tarafında Add Service Reference seçeneği ile Proxy tipini üretirken, servis uygulamasının da açık olması gerektiğini unutmayalım.

İstemci Tarafının Geliştirilmesi

Gelelim istemci tarafına. Servis referansını ekledikten sonra istemci tarafında aşağıdaki config içeriğinin üretildiği gözlemlenecektir.

```xml
<?xml version="1.0" encoding="utf-8" ?> 
<configuration> 
    <startup> 
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5" /> 
    </startup> 
    <system.serviceModel> 
        <bindings> 
            <basicHttpBinding> 
                <binding name="BasicHttpBinding_IEchoService" /> 
            </basicHttpBinding> 
            <netTcpBinding> 
                <binding name="NetTcpBinding_IEchoService" /> 
            </netTcpBinding> 
            <wsHttpBinding> 
                <binding name="WSHttpBinding_IEchoService" /> 
            </wsHttpBinding> 
            <udpBinding> 
                <binding name="UdpBinding_IEchoService" /> 
            </udpBinding> 
        </bindings> 
        <client> 
            <endpoint address="http://localhost:54160/basic/EchoService" 
               binding="basicHttpBinding" bindingConfiguration="BasicHttpBinding_IEchoService" 
                contract="EchoSpace.IEchoService" name="BasicHttpBinding_IEchoService" /> 
            <endpoint address="http://localhost:54160/ws/EchoService" binding="wsHttpBinding" 
                bindingConfiguration="WSHttpBinding_IEchoService" contract="EchoSpace.IEchoService" 
                name="WSHttpBinding_IEchoService" /> 
            <endpoint address="soap.udp://localhost:54162/UdpServiceHost" 
                binding="udpBinding" bindingConfiguration="UdpBinding_IEchoService" 
                contract="EchoSpace.IEchoService" name="UdpBinding_IEchoService" /> 
            <endpoint address="net.tcp://localhost:54161/EchoService" binding="netTcpBinding" 
                bindingConfiguration="NetTcpBinding_IEchoService" contract="EchoSpace.IEchoService" 
                name="NetTcpBinding_IEchoService">   
            </endpoint> 
        </client> 
    </system.serviceModel> 
</configuration>
```

Görüldüğü gibi 4 Endpoint erişim notkasına ait tanımlamalar config dosyası içerisine eklenmiştir. İstemci tarafındaki test kodlarımız ise aşağıdaki gibidir.

```csharp
using Highway.ClientApp.EchoSpace; 
using System; 
using System.Collections.Generic; 
using System.Diagnostics;

namespace Highway.ClientApp 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            Console.WriteLine("Başlamak için bir tuşa basınız"); 
            Console.ReadLine();

            string testData = CreateRandomString();

            Dictionary<EchoServiceClient,string> proxies = new Dictionary<EchoServiceClient,string> 
            { 
               {new EchoServiceClient("UdpBinding_IEchoService"),"UDP"}, 
                {new EchoServiceClient("BasicHttpBinding_IEchoService"),"BASIC HTTP"}, 
                {new EchoServiceClient("WSHttpBinding_IEchoService"),"WS HTTP"}, 
                {new EchoServiceClient("NetTcpBinding_IEchoService"),"NET TCP"}, 
            };

            foreach (var proxy in proxies) 
            { 
               for (int i = 1; i < 5; i++) 
                { 
                    Executer(proxy.Key, proxy.Value, i * 10000, testData); 
                } 
                proxy.Key.Close(); 
            } 
        }

        static void Executer(EchoServiceClient proxy,string title,int tryCount,string testData) 
        { 
            Stopwatch watcher = new Stopwatch();

            watcher.Start();

            for (int i = 0; i < tryCount; i++) 
            { 
                proxy.SendEcho(testData); 
           }

            watcher.Stop(); 
            Console.WriteLine("{0} Total Milliseconds {1}",title, watcher.ElapsedMilliseconds.ToString()); 
        }

        static string CreateRandomString() 
        { 
            char[] charachters = new char[4096]; 
            for (int i = 0; i < charachters.Length; i++) 
            { 
                charachters[i] = 'S'; 
            } 
            return new string(charachters); 
        } 
    } 
}
```

Ne Yapıyoruz?

Aslında yaptığımız işlem gayet basit. 4096 byte boyutu olan ve içerisinde S yazan metin katarlarımızı paketler halinde defalarca servis tarafına göndermekteyiz. Örnekte bu tekrar sayısı i10000 kadardır. Bu da bize test için gerekli veri aktarım yükünü getirecektir.

> Gönderimde bulunulacak olan UDP paketlerindeki Datagram içeriğinin belirli bir boyut sınırı vardır. En azından WCF tarafında bunun bir limiti bulunmaktadır. Eğer çok büyük boyutlu bir paket gönderilmeye çalışılırsa aşağıdakine benzer bir çalışma zamanı hatasının istemci tarafına çıkması muhtemeldir. Dikkat edileceği üzere 65507 byte’ ın aşılmaması gerektiği ifade edilmektedir.
> [![udpwcf_6](/assets/images/2015/udpwcf_6_thumb.png)](/assets/images/2015/udpwcf_6.png)

Testler

Artık uygulamamızı test edebiliriz. Ben kullanmakta olduğum sistemde yaptığım testler sonucu aşağıdaki ekran görüntüsündekine benzer çalışma zamanı süreleri ile karşılaştım.

[![udpwcf_3](/assets/images/2015/udpwcf_3_thumb.png)](/assets/images/2015/udpwcf_3.png)

Dikkat edileceği üzere UDP ile yapılan haberleşme diğerlerine göre çok daha hızlı. WS tabanlı yapılan iletişim ise yerlerde sürünüyor diyebiliriz bu sonuçlara göre. Nitekim WS iletişiminde paket sayısının belirgin ölçüde arttığını da belirtelim. Sonuçları Excel Chart ile yorumlarsak farkları daha net anlayabiliriz.

[![udpwcf_4](/assets/images/2015/udpwcf_4_thumb.png)](/assets/images/2015/udpwcf_4.png)

UDP’ ye özellikle OneWay erişimde en çok yaklaşan TCP protokolü üzerinden yapılan iletişimdir. Diğer yandan en uzun süreler WS HTTP tabanlı iletişimde gerçekleşmiştir. Bu son derece doğaldır nitekim söz konusu iletişim protokolünde WS standartları gereği Security’ yi sağlamak için yapılan bir dizi zaman kaybettirici işlem söz konusudur.

Peki Ya Request/Response Modunda

Yapılan testler dikkat edileceği üzere OneWay çalışan bir servis operasyonu için geçerlidir. Peki Request/Response modeline göre çalışan operasyonlar için de durum aynı mıdır? Gelin son olarak bu vakayı da test etmeye çalışalım. Bunun için ilk olarak servis tarafına aşağıdaki test operasyonunu ekleyelim.

```csharp
using System.ServiceModel;

namespace HighwayServiceLibrary 
{ 
    [ServiceContract] 
    public interface IEchoService 
    { 
        [OperationContract(IsOneWay=true)] 
        void SendEcho(string content);

        [OperationContract] 
        string GetEcho(int length); 
    } 
}
```

Bu basit operasyonun implementasyonu çok da önemli değil aslında. Geriye tek bir karakter döndürse de olur. Asıl önemli nokta tüm protokoller için eşit olan bu operasyonun Request/Response modeline göre çalışıyor olmasıdır.

> Şu anda servis tarafındaki sözleşme (Service Contract) değişmiş durumdadır. Dolayısıyla istemci tarafındaki Proxy tipinin Update edilmesi gerekir (Update Service Reference)

İstemci tarafında ise aşağıdaki küçük kod değişikliğini yapmamız yeterlidir.

```csharp
static void Executer(EchoServiceClient proxy,string title,int tryCount,string testData) 
{ 
    . 
    . 
    . 
    for (int i = 0; i < tryCount; i++) 
    { 
        //proxy.SendEcho(testData); 
        proxy.GetEcho(1); 
    } 
    . 
    . 
    . 
}
```

Çalışma zamanında aşağıdaki ekran görüntüsündekine benzer sonuçlar alınmıştır.

[![udpwcf_7](/assets/images/2015/udpwcf_7_thumb.png)](/assets/images/2015/udpwcf_7.png)

Her zamanki gibi sonuçları Excel Chart üzerinden değerlendirirsek çok daha iyi analiz edebiliriz.

[![udpwcf_8](/assets/images/2015/udpwcf_8_thumb.png)](/assets/images/2015/udpwcf_8.png)

Liderlik dikkat edileceği üzere TCP bazlı iletişime geçti. İşte başta belirttiğimiz sürpriz durumlardan birisi. Aradaki farklar çok da az olsa UDP iletişiminden daha hızlı sonuçlar elde edildiğini görmekteyiz. Bu sonuçlara göre özellikle tek yönlü (OneWay) olarak tasarlanabilen, ses ile video gibi veri aktarımlarının söz konusu olduğu, güvenli olması gerekmeyen ve hatta içeriğin Streaming şeklinde değil de paketler halinde gönderilip n sayıda istemciye ulaştırılacağı düşünülen senaryolarda UDP tabanlı iletişimi tercih edebiliriz. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[HighwayLast.zip (101,38 kb)](/assets/files/2015/HighwayLast.zip)