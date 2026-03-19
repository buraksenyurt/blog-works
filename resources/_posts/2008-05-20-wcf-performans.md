---
layout: post
title: "WCF - Performans"
date: 2008-05-20 12:00:00 +0300
description: "Uzun bir aradan sonra tekrar birlikteyiz. Windows Communication Foundation mimarisinin geliştirilmesininin tek amacı, var olan dağıtık mimari modellerini bir çatı altında birleştirmek değildir. Buna paralel olaraktan WCF mimarisi, .Net Remoting, Xml Web Servisleri, WSE(Web Service Enhancements), MSMQ(Microsoft Message Queue), COM+ gibi pek çok dağıtık uygulama geliştirme modelinin çalışma zamanı alt yapısının kolayca oluşturulabilmesinide hedeflemektedir."
categories:
  - wcf
tags:
  - wcf
  - csharp
  - xml
  - dotnet
  - workflow-foundation
  - json
  - web-service
  - xml-web-services
  - http
  - performance
  - serialization
  - visual-studio
---
# WCF - Performans
Değerli Okurlarım Merhabalar,

Uzun bir aradan sonra tekrar birlikteyiz. Windows Communication Foundation mimarisinin geliştirilmesininin tek amacı, var olan dağıtık mimari modellerini bir çatı altında birleştirmek değildir. Buna paralel olaraktan WCF mimarisi,.Net Remoting, Xml Web Servisleri, WSE (Web Service Enhancements), MSMQ (Microsoft Message Queue), COM+ gibi pek çok dağıtık uygulama geliştirme modelinin çalışma zamanı alt yapısının kolayca oluşturulabilmesinide hedeflemektedir. Burada dekleratif programlama modelinin benimsenmesinin önemli katkısı vardır. Sonuçta geliştiricinin sıklıkla yapmak zorunda kaldığı alt yapı hazırlıklarının attribute (nitelik) veya konfigurasyon bazlı olacak şekilde ayarlanabilmesi son derece avantajlıdır.

Bu açıdan bakıldığında olayın kilit noktası WCF'in ABC (AddressBindingContract)' sinde yer alan bağlayıcı tiplerdir (Binding Types). Bilindiği üzere WCF mimarisi.Net Framework 3.0 ile birlikte gelmiştir. Bununla birlikte şu an itibariyle.Net Framework 3.5 sürümünde ek yeniliklerde içermektedir (Örneğin Web Programlama Modeli, WorkFlow Foundation ile Tam Entegrasyon, JSON, AJAX desteği gibi). Bağlayıcı tiplerin sayısının çok olmasının, geliştiricilerin karar verme noktasındaki işlerini zorlaştırdığıda bir gerçektir. Bu nedenle çoğunlukla yardımcı tablolardan veya diagramlardan yararlanılmaktadır.

Yazın geldiği bu sıcak günlerde yazdığımız bu makalemizde, biraz hafiften ilerleyecek ve bu bağlayıcı tiplerden çok sık kullanılanlar arasındaki performans farklarını incelemek için nasıl bir yol izleyebileceğimizi incelemeye çalışacağız. Nitekim senaryoya göre seçilebilecek bağlayıcı tiplerin birden fazla olması halinde performans kriterleri ön plana çıkmaktadır. Öncelikli olarak şu anda kullanılan bağlayıcı tipler ve aralarındaki belirgin farklıları göz önüne almakta yarar vardır. İşte aşağıdaki tablo bu farklılıkları dile getirmektedir.

![mk251_2.gif](/assets/images/2008/mk251_2.gif)

Görüldüğü gibi bağlayıcı tiplerin birbirlerine göre farklı kullanım sahaları ve senaryoları bulunmaktadır. Buradaki bağlayıcı tiplerde yeterli gelmiyorsa bu durumda özel bağlayıcı tiplerin (Custom Binding Type) yazılmasıda tercih edilebilir ki bazı senaryolarda (örneğin Front-End Service yazılması gereken durumlar veya Reply Attack gibi vakalarda) bu gereklidir. Yukarıdaki tablo karar vermek için tam olarak yeterli değildir. Karar verirken ele alınması gereken sorular arasında platform bağımsızlık (Interoperability), güvenilirlik (Relaibiliyu), performans, WS- şartnameleri (Specifications), güvenlik (Security) gibi pek çok kriter bulunmaktadır. İşte bu noktada da devreye aşağıdaki gibi bir tablo girmektedir.

![mk251_3.gif](/assets/images/2008/mk251_3.gif)

Görüldüğü gibi bu tabloda yer alan kriterlerden yararlanarak hangi bağlayıcı tipin kullanılacağı kolay bir şekilde kararlaştırılabilir. Söz gelimi çift yönlü (Duplex) iletişimin söz konusu olduğu bir vaka varsa, kriterler sınırlıdır. Bir başka deyişle böyle bir senaryoda WsDualHttpBinding, NetTcpBinding, NetNamedPipeBinding ve NetPeerTcpBinding tiplerinden birisi kullanılabilir. Ancak karar vermek halen daha zor olabilmektedir. Belkide bir başlangıç noktası olsa bu iş daha da kolaylaşabilir. O halde bir akış şeması son derece yerinde olur. Mesela;

![mk251_1.gif](/assets/images/2008/mk251_1.gif)

Öncelikli olarak başka servisler ile uyumlu bir şekilde konuşulup konuşulmayacağına karar verilerek başlayan bu iş akışının farklı versiyonlarıda bulunmaktadır. Ancak Juval Löwy ve Steve Resnic'in önerdiği örnek karar verme adımlarından biriside bu çizelgedir. Karar vermede önemli olan kriterlerden biriside elbetteki Performans'tır. Çoğunlukla gerçek hayat ortamlarında, yazılan uygulamaların gerçek değeri performansları ile ölçülmektedir. Bu durumda akla gelen sorulardan ilki vakaya göre hangi bağlayıcı tipin en iyi performansı verdiğidir. İşte yazımızın bu bölümünden itibaren 4 temel ve sık kullanılan bağlayıcı tip arasındaki saniye başına düşen çağrı (Calls Per Seconds) sayılarını test edeceğimiz örnek bir senaryo üzerinde durmaya çalışacağız. Vakamızda NetTcpBinding, WsHttpBinding, BasicHttpBinding ve NetNamedPipeBinding bağlayıcı tipleri ele alınmaktadır.

Bilindiği üzere NetTcpBinding bağlayıcı tipi çoğunlukla intranet tabanlı sistemlerde yüksek performansı ile göz önüne çıkmaktadır. Ne varki WS destekli servisler ile konuşulması gereken vakalarda WsHttpBinding bağlayıcı tipi sıkılıkla değerlendirilmektedir. Tüm bunların yanında özellikle WS Basic Profile 1.1 ve öncesi yazılmış Web Servisleri ile olan haberleşmede BasicHttpBinding biçilmiş kaftandır. Elbette öyle vakalar vardırki aynı makine üzerinde koşmakta olan servislerin bir birleriyle haberleşmesi söz konusudur. Çok doğal olarak böyle bir durumda NetNamedPipeBinding tipi ön tarafa çıkmaktadır.

> .Net Framework 3.0 ile birlikte, WCF tarafında geliştirilen servislerinin performans değerlerinin ölçülebilmesi amacıyla, ServiceModelEndPoint, ServiceModelOperation, ServiceModelService isimli performans nesneleri (Performance Objects) gelmektedir. Bu değerler Performance Monitor aracı yardımıyla ele alınabilir yada kod tarafından değerlendirilebilir.

Dilerseniz örnek üzerinden devam edelim. Öncelikli olarak kendimize bir adet WCF Servis Library geliştiriyor olacağız. (Söz konusu servis kütüphanesi Visual Studio 2008 kullanılarak.Net Framework 3.5 şablonunda geliştirilmiştir.) Servis tarafına sunulacak olan sözleşme (Contract) ve uygulayıcı tipin içeriği aşağıdaki gibidir.

![mk251_4.gif](/assets/images/2008/mk251_4.gif)

ITester arayüzü (Interface);

```csharp
using System;
using System.ServiceModel;

namespace ProcessLib
{
    [ServiceContract]
    public interface ITester
    {
        [OperationContract]
        string GetString();
    }
}
```

Sözleşmeyi uygulayan Tester sınıf (Class);

```csharp
using System;

namespace ProcessLib
{
    public class Tester
        :ITester
    {
        #region ITester Members
    
        public string GetString()
        {
            return "".PadRight(1000, 'Q');
        }

        #endregion
    }
}
```

Servis sözleşmesinin sunduğu operasyon sadece string döndüren bir metoda sahiptir. Test sırasında bu metod kullanılaraktan istemci tarafından yapılan yüklü operasyon çağrılarının sonuçları irdelenmeye çalışılacaktır. Artık Host uygulamanın yazılmasına başlanabilir. Olayların basit şekilde analiz edilebilmesi için Host uygulama Console olarak tasarlanmaktadır. Bu uygulama kendi içerisinden 4 farklı EndPoint sunmaktadır. Bunlar vakada yer alan bağlayıcı tiplerin her biri için ele alınmaktadır. Bu amaçla konfigurasyon dosyasının içeriği aşağıdaki gibi geliştirilebilir. (Bu aşamaya gelmeden önce Host uygulamaya System.ServiceModel.dll ve WCF Sınıf Kütüphanesine ait assembly nesnelerinin eklenmesi gerektiğinide hatırlayalım.)

Host uygulama tarafındaki konfigurasyon (App.Config) içeriği;

```csharp
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <diagnostics performanceCounters="All" />
        <services>
            <service name="ProcessLib.Tester">
                <endpoint address="net.tcp://localhost:9000/TesterService" binding="netTcpBinding" bindingConfiguration="" name="TesterServiceTcpEndPoint" contract="ProcessLib.ITester" />
                <endpoint address="http://localhost:9001/TesterService" binding="basicHttpBinding" name="TesterServiceHttpEndPoint" contract="ProcessLib.ITester" />
                <endpoint address="http://localhost:9002/TesterService" binding="wsHttpBinding" bindingConfiguration="" name="TesterServiceWsHttpEndPoint" contract="ProcessLib.ITester" />
                <endpoint address="net.pipe://localhost/TesterService" binding="netNamedPipeBinding" bindingConfiguration="" name="TesterServicePipeEndPoint" contract="ProcessLib.ITester" />
            </service>
        </services>
    </system.serviceModel>
</configuration>
```

Konfigurasyon dosyasında en çok dikkat çeken noktalardan biriside diagnostics isimli elementtir. Bu elementin içerisinde yer alan performanceCounters niteliğine atanan All değeri sayesinde Performance Monitor üzerinden ölçümler yapılabilmektedir. Bu sebepten servis tarafındaki konfigurasyon dosyasında mutlaka bildirilmelidir.

Host Uygulama Kodları;

```csharp
using System;
using System.ServiceModel;
using ProcessLib;

namespace ServerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(Tester));
            host.Open();
            Console.WriteLine("Servis dinlemede\nKapatmak için bir tuşa basın");
            Console.ReadLine();
            host.Close();
        }
    }
}
```

Host uygulama 4 farklı EndPoint üzerinden hizmet vermektedir. Tüm EndPoint'ler Tester isimli servis tipini kullanmaktadır. Buna göre 4 farklı tipte istemcinin bu servis üzerinden hizmet alması mümkündür. İstemci tarafını geliştirmeden önce gerekli proxy tipinin üretimi için svcutil aracından aşağıdaki ekran görüntüsünde yer aldığı gibi yararlanılabilir.

![mk251_5.gif](/assets/images/2008/mk251_5.gif)

Artık istemci uygulamanın geliştirilmesine başlanabilir. Buradada örneği basit bir şekilde ele alabilmek için bir Console uygulaması göz önüne alınmaktadır. Yine önemli olan nokta konfigurasyon dosyasının içeriğidir. Dört EndPoint için testler aynı istemci üzerinden yapılacağından konfigurasyon içeriği aşağıdaki gibi tasarlanmıştır.

İstemci uygulama konfigurasyon (App.config) içeriği;

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <system.serviceModel>
        <client>
            <endpoint address="http://localhost:9001/TesterService" binding="basicHttpBinding" contract="ITester" name="ClientHttpEndPoint" />
            <endpoint address="net.tcp://localhost:9000/TesterService" binding="netTcpBinding" bindingConfiguration="" contract="ITester" name="ClientTcpEndPoint" />
            <endpoint address="http://localhost:9002/TesterService" binding="wsHttpBinding" bindingConfiguration="" contract="ITester" name="ClientWsHttpEndPoint" />
            <endpoint address="net.pipe://localhost/TesterService" binding="netNamedPipeBinding" bindingConfiguration="" contract="ITester" name="ClientPipeEndPoint" />
        </client>
    </system.serviceModel>
</configuration>
```

İstemci uygulamanın kodları;

```csharp
using System;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Bağlayıcı tipi seçiniz...");
            Console.WriteLine("NetTcpBinding için 1");
            Console.WriteLine("BasicHttpBinding için 2");
            Console.WriteLine("WsHttpBinding için 3");
            Console.WriteLine("NetNamedPipeBinding için 4");

            string secim=Console.ReadLine();
            Console.WriteLine("Sayaç Değerini Giriniz");
            int sayac;
            if (!Int32.TryParse(Console.ReadLine(), out sayac))
                sayac = 100000; 

            switch (secim){
                case "1":
                    TestiBaslat("ClientTcpEndPoint",sayac);
                    break;
                case "2":
                    TestiBaslat("ClientHttpEndPoint", sayac);
                    break;
                case "3":
                    TestiBaslat("ClientWsHttpEndPoint", sayac);
                    break;
                case "4":
                    TestiBaslat("ClientPipeEndPoint", sayac);
                    break;
                default:
                    TestiBaslat("ClientTcpEndPoint", sayac);
                    break;
            }
        }
    
        private static void TestiBaslat(string baglayici,int testSayisi)
        {
            Console.WriteLine("Test Başladı\nDeneme Sayisi {0}\nSeçilen EndPoint {1}",testSayisi,baglayici);
            TesterClient client = new TesterClient(baglayici);
            for (int i = 0; i < testSayisi; i++)
            {
                string result = client.GetString();
            }
            Console.WriteLine("Test tamamlandı...");
            client.Close();
        }
    }
}
```

İstemci uygulama GetString isimli metodu sembolik olarak girilen değer kadar (varsayılan olarak 100000 (Yüzbin) kere) çağırmaktadır. Bu bize test için yeterli süreyide vermektedir. Diğer taraftan elbetteki bu test değerleri makinenin özelliklerine göre farklılık gösterecektir. Diğer taraftan bağlayıcı tipler arasındada belirgin farklılıklar olduğu gözlenecektir. Bunun en büyük nedenlerinden birisi ilgili bağlayıcı tipin arka tarafta kullandığı kodlama (Encoding) ve iletişim protokolüdür (Transport Protocol). (Makalede yer alan örneğin testlerinin yapıldığı makine 1 Gb RAM kapasiteli olup, Intel Centrino işlemcilidir. Ayrıca Windows XP Service Pack 2 işletim sistemine sahiptir. Testlerde yer alan istemci ve sunucu uygulama aynı makine üzerinde koşmaktadır.)

Artık testlere başlanabilir. Performance Counter üzerinden çalışma zamanı ölçümlerinin yapılabilmesi için servis uygulamasının çalışıyor olması gerekmektedir. Sonrasında ise Performance Monitor kullanılarak ölçümler yapılabilir. Performance Monitor uygulamasına komut satırından perfmon yazılarak ulaşılabilir. Servis uygulaması başlatıldığında Counter olarak eklenebilecek seçeneklerde ilgili servis uygulamasına ait örnek görülebilecektir.

![mk251_6.gif](/assets/images/2008/mk251_6.gif)

Biz testlerimizde ServiceModeService 3.0.0.0 Performans nesnesinin Calls Per Second isimli sayacını kullanıyor olacağız. Bu sayaç servise gelen saniye başına operasyon çağrılarını göstermekte olup bağlayıcı tiplerin alt yapı hazırlıkları nedeni ile ne kadar yavaş veya ne kadar hızlı olduklarını göstermektedir. Performance Monitor sonuçları test programında kullanılan her bağlayıcı tip için aşağıdaki ekran çıktılarında olduğu gibidir.

NetTcpBinding için Calls Per Second sonuçları;

![mk251_7.gif](/assets/images/2008/mk251_7.gif)

BasicHttpBinding için Calls Per Second sonuçları;

![mk251_8.gif](/assets/images/2008/mk251_8.gif)

WsHttpBinding için Calls Per Second sonuçları;

![mk251_9.gif](/assets/images/2008/mk251_9.gif)

NetNamedPipeBinding için Calls Per Second sonuçları;

![mk251_10.gif](/assets/images/2008/mk251_10.gif)

Tüm bunları bir arada değerlendirildiğinde, saniye başına düşen operasyon çağrıları için aşağıdaki grafikte görülen sonuçlar ortaya çıkmaktadır. Bu grafikte tepe noktalarının yaklaşık maksimum ve ortalama değerleri ele alınmaktadır. Bu en azından tahmini olarak, bu dört bağlayıcı tip arasındaki performans farklılıkları hakkında fikir vermektedir.

![mk251_11.gif](/assets/images/2008/mk251_11.gif)

Çok doğal olarak aynı makine üzerinde çalışan servisler arası haberleşmede tercih edilen NetNamedPipeBinding için en yüksek değerler ortaya çıkmaktadır. Diğer taraftan özellikle TCP ve Binary serileştirme kullanan NetTcpBinding değerleride oldukça yüksektir. Bunların yanında WsHttpBinding gerçekten çok düşük değerler ile göze çarpmaktadır. Bunun en büyük nedeni WS standartlarına göre mesajların hazırlanması sırasında geçen süre kayıplarıdır. Tabiki bu kriter tek başına yeterli değildir. Söz gelimi servisin Host edildiği sunucunun operasyon başına maliyet (Cost Per Operation) gibi performans değerleride göz önüne alınabilir. Bu ve daha pek çok performans testi yapılabilir.

Buraya kadar anlatılanlar göz önüne alındığında sadece bağlayıcı tipler arasındaki farklılıkları analiz etmek için basit olarak nasıl bir yol izlenebileceği ele alınmıştır. Bunun dışında WCF ile geliştirilen servislerin Xml Web Serviceleri,.Net Remoting veya COM+ ile geliştirilen modellere göre belirgin performans farklılıkları olduğu bilinmektedir. Bu konu ile ilişkili daha detaylı bilgiye [http://msdn.microsoft.com/en-us/library/bb310550.aspx](http://msdn.microsoft.com/en-us/library/bb310550.aspx) ulaşabilirsiniz. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](/assets/files/2008/Performance.rar)