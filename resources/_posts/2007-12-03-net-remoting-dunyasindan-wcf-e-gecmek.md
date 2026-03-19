---
layout: post
title: ".Net Remoting Dünyasından WCF'e Geçmek"
date: 2007-12-03 12:00:00 +0300
description: "Windows tabanlı olan Servis Yönelimli Mimari(Service Oriented Architecture) tekniklerinden biriside .Net Remoting' dir. .Net Remoting mimarisi ağırlıklı olarak TCP bazlı ve Binary tabanlı paket iletiminde kullanılır. En büyük özelliklerinden birisi, sadece Windows işletim sistemlerinden oluşan ağlarda koşabilmesidir. Elbette HTTP üzerinden SOAP-Simple Object Access Protocol formatına uygun alt yapı kurulmasıda mümkündür. Bu sayede internet ağındada ektin şekilde kullanılabilir. Ancak Windows bağımlı olması platform bağımsızlığını ortadan kaldırmaktadır. Günümüzde WCF(Windows Communication Foundation) gibi daha ölçeklenebilir(Scalable), birleştirilmiş(Unified) bir Servis Yönelimli Mimari(SOA) açılımıda mevcuttur. Bu durumda geliştiricilerin karşısına önemli bazı sorular ve sorunlar çıkmaktadır."
categories:
  - dotnet-remoting
  - wcf
tags:
  - dotnet-remoting
  - wcf
  - csharp
  - xml
  - dotnet
  - soap
  - web-service
  - http
  - iis
  - authentication
  - authorization
  - serialization
  - visual-studio
---
# .Net Remoting Dünyasından WCF'e Geçmek
Değerli Okurlarım Merhabalar,

Windows tabanlı olan Servis Yönelimli Mimari (Service Oriented Architecture) tekniklerinden biriside.Net Remoting'dir..Net Remoting mimarisi ağırlıklı olarak TCP bazlı ve Binary tabanlı paket iletiminde kullanılır. En büyük özelliklerinden birisi, sadece Windows işletim sistemlerinden oluşan ağlarda koşabilmesidir. Elbette HTTP üzerinden SOAP-Simple Object Access Protocol formatına uygun alt yapı kurulmasıda mümkündür. Bu sayede internet ağındada ektin şekilde kullanılabilir. Ancak Windows bağımlı olması platform bağımsızlığını ortadan kaldırmaktadır. Günümüzde WCF (Windows Communication Foundation) gibi daha ölçeklenebilir (Scalable), birleştirilmiş (Unified) bir Servis Yönelimli Mimari (SOA) açılımıda mevcuttur. Bu durumda geliştiricilerin karşısına önemli bazı sorular ve sorunlar çıkmaktadır. İşte bunlardan bir kaçı;

- Var olan.Net Remoting alt yapısı, WCF tabanlı bir hale dönüştürülebilir mi?
- Dönüştürülürse ne gibi düzenlemeler yapmak gerekir?
- Sıfırdan WCF tabanlı bir model geliştirmek yerine,.Net Remoting WCF'e göç etmek mantıklı mıdır?
- .Net Framework 2.0 hatta 1.1 uyumlu sistemler üzerine kurulmuş senaryolarda, var olan.Net Remoting uygulamalarında yapılacak WCF düzenlemeleri geçerli olacak mıdır? Yoksa bu makinelere Framework 3.0 yüklenmeli midir?

Bu sorular farklı açılardan bakıldığında artabilir. İlerleyen kısımlarda bu sorulara ve beraberinde gelen sorunlara cevap bulmamızı kolaylaştıracak şekilde ilerlemeye çalışacağız.

> Windows Communication Foundation,.Net Remoting, Web Servisleri, MSMQ, Named Pipes, WSE gibi pek çok dağıtık mimari modelini tek bir çatı altında birleştirip sunabilmesiyle ön plana çıkmış bir Servis Yönelimli Mimari (Service Oriented Architecture) alt yapısıdır. Bu alt yapı, önceki mimarilerde daha fazla kodlama gerektiren yada geliştiricileri zorlayan standartların (Örneğin WS-Specifications) kolay ve etkin bir şekilde uygulanabilmesini hatta bir arada tutulabilmesine de izin vermektedir.

Bazı durumlarda çalışmakta olan sistemi yeni bir mimariye adapte etmenin maliyeti çok yüksek olabilir. Söz gelimi ölçek olarak çok büyük çaplı projelerde sistemi yeniden tasarlamak son derece sancılı bir süreç olabilir. Ancak böyle bir durum söz konusu değilse karşımızda iki alternatif olacaktır. Var olan sistemi modifiye ederek WCF'e taşımak yada sıfırdan tasarlamak. Aslında var olan bir.Net Remoting alt yapısını bir kaç küçük değişiklik ile WCF'e taşımak son derece kolaydır. Herşeyden önce.Net Remoting içerisinde yer alan temel parçaları göz önüne alarak ilerlemekte yarar vardır.

.Net Remoting ile hazırlanmış bir sistemde istemcilerin (Clients) kullanacakları fonksiyonellikleri barındıran uzak nesneler (Remote Objects) bir sunucu (Server) uygulama üzerinden Client Activated Object yada Server Activated Object modeline uygun olacak şekilde yayınlanırlar. Hatta SAO nesneleride Singleton veya SingleCall olarak tasarlanırlar. İstemci uygulamalar aslında uzak nesne referanslarını kullanırken bu referanslar Marshal By Reference modeline göre sunucu üzerinde örneklenirler. Bir başka deyişle istemci uzak nesneyi sanki kendi uygulama alanı (Application Domain) içerisindeymiş gibi kullanırlarken, tüm işlevler sunucu üzerinde gerçekleşmektedir.

Çok doğal olarak burada istemci ve sunucu arasındaki mesaj trafiğinin ortak bir zemine oturtulması şarttır. Bu nedenle genellikle istemci ve sunucu uygulamalar hizmete ait nesne tasarımını içeren şartnamelerin yer aldığı bir arayüz (Interface) tipini ortaklaşa referans ederler. Böylece servis tarafındaki fonkisyonelliklerin iç yapılarının değiştirilmesi halinde istemciler üzerinde yeniden güncelleme yapılmasına gerek kalmayacaktır. Ayrıca istemciler sadece arayüzü görebildiklerinden, çağırdıkları fonksiyonelliklerin iç yapısını bilmeyeceklerdir ki buda servis kodunun tam olarak istemci tarafından görülmesini engellemektedir. Zaten WCF'in tamamen arayüzlere dayalı bir sistemi önermesinin ve kullanmasının en önemli nedenleride bunlardır.

Bu noktada geliştirici tarafından tanımlanan serileştirilebilir tiplerinde (Serializable Types) ortak bir sınıf kütüphanesi (Class Library) üzerinde olması gerekmektedir. Serileştirilebilir nesneler aslında servis tarafında örneklenip sadece varlıkları (Entities) istemci tarafına aktarılan örnekler olarak düşünülebilir. Söz gelimi istemcinin talep ettiği bir ürünün, Product isimli bir sınıfa ait nesne örneği olacak şekilde servis tarafında doldurulması ve istemciden elde edilmesi buna örnek olarak verilebilir. Burada istemcinin talebi sonrasında servis tarafından dönen veriler Product isimli sınıfın özellik (Property) veya alanlarının (Fields) değerleridir. Serileştirilebilir nesnelerin WCF mimarisinde ele alınması çok daha kolaydır. Özellikle versiyonlamanın (Versioning) WCF üzerinden gerçekleştirilmesi daha esnektir.

> Makalenin konusu.Net Remoting'den WCF'e bir iki adımda taşınmanın nasıl sağlanabileceğini göstermek olduğundan,.Net Remoting ve WCF mimarilerinin çok geniş detayları göz ardı edilmektedir.

Artık bir örnek üzerinden hareket ederek devam edebiliriz. Öncelikli olarak bir.Net Remoting uygulamasını Visual Studio 2005 üzerinden geliştiriyor olacağız. İlk olarak istemci ve servis uygulamasının ortaklaşa kullanacağı sınıf kütüphanesini (Class Library) tasarlayarak işe başlanabilir. Sınıf kütüphanesi içerisindeki tiplere ait sınıf diyagramı (Class Diagram) ve kod içerikleri aşağıdaki gibidir.

![mk233_1.gif](/assets/images/2007/mk233_1.gif)

IProductManager arayüzü (Interface);

```csharp
using System;

namespace AdvLibrary
{
    public interface IProductManager
    {
        Product GetProductInfo(int productId);
    }
}
```

Örneğin daha basit olarak ele alınabilmesi için arayüz (Interface) tanımlamasında, geriye Product tipinden nesne örneği döndürebilen ve integer tipinden bir parametre alabilen GetProductInfo isimli bir metod bildirimi yer almaktadır.

Product sınıfı (Class);

```csharp
using System;

namespace AdvLibrary
{
    [Serializable]
    public class Product
    {
        public int Id;
        public string Name;
        public double ListPrice;
    }
}
```

Product isimli sınıf içerisinde bir ürüne ait Id, Name ve ListPrice değerleri tutulmaktadır. Bununla birlikte Product sınıfı için dikkat çekici en önemli özelliklerden biriside Serializable niteliği (attribute) ile imzalanmış olmasıdır. Burada açık bir şekilde Product sınıfına ait nesne örneklerinin Marshal By Value olarak sunucudan istemciye serileşerek taşınabileceği garanti altına alınmaktadır.

Servis tarafında yer alan program, söz konusu örnekte bir Console uygulaması olarak tasarlanmaktadır. Bu uygulamanın çok doğal olarak yukarıda tasarlanan tipleri içeren AdvLibrary isimli sınıf kütüphanesini (Class Library) referans ediyor olması gerekmektedir. Nitekim sunucu uygulama içerisinde asıl iş yapan sınıfın uyarlayacağı arayüz ve kullanacağı serileştirilebilir tip bu sınıf kütüphanesi içerisinde bulunmaktadır.

> Gerçek hayat senaryolarında en çok tercih edilen.Net Remoting yöntemi, sunucu uygulamasını bir Windows Service olarak tasarlamaktadır. Bu yönetimi daha güçlü olan, ölçeklenebilir ve güvenli bir uygulama seçimidir. Bunun dışında IIS (Internet Information Services) üzerinden barındırma (Host) imkanıda bulunmaktadır. Diğer taraftan sunucu uygulama bir Windows uygulaması hatta bu örnekte olduğu gibi bir Console uygulamasıda olabilir.

Tekrar sunucu uygulamasına dönülecek olursa, IProductManager isimli arayüzü (Interface) uygulayan uzak nesne sınıfının aşağıdaki gibi tasarlanabileceği düşünülebilir.

![mk233_2.gif](/assets/images/2007/mk233_2.gif)

```csharp
using System;
using AdvLibrary;
using System.Data.SqlClient;

namespace ServerApp
{
    public class ProductManager:MarshalByRefObject,IProductManager
    {
        public ProductManager()
        {
            Console.WriteLine("ProductManager nesnesi oluşturuldu...");
        }    
        #region IProductManager Members

        public Product GetProductInfo(int productId)
        {
            Product prd = null;
            using (SqlConnection conn = new SqlConnection("data source=.;database=AdventureWorks;integrated security=SSPI"))
            {
                using (SqlCommand cmd = new SqlCommand("Select ProductId,Name,ListPrice,SellStartDate From Production.Product Where ProductId=@PrdId", conn))
                {
                    cmd.Parameters.AddWithValue("@PrdId",productId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        prd = new Product();
                        prd.Id = productId;
                        prd.Name = reader["Name"].ToString();
                        prd.ListPrice = Convert.ToDouble(reader["ListPrice"]);
                    }
                    reader.Close();
                }
            }
            return prd;
        }

        #endregion
    }
}
```

Bu sınıf içerisinde yer alan metod uyarlamasında AdventureWorks isimli veritabanına gidilmekte ve Production şemasında (Schema) yer alan Product tablosundan parametre olarak gelen ProductID değerine sahip olan ürün bilgisi geriye döndürülmektedir. Nesne oluşumlarının kolay takip edilebilmesi amacıyla ProductManager sınıfı içerisine birde varsayılan yapıcı metod (Default Constructor) ilave edilmiştir. Burada dikkat edilmesi gereken nokta sınıfın IProductManager isimli arayüz dışında MarshalByRefObject sınıfından türemiş olmasıdır.

Sunucu tarafındaki Remoting ayarlarını konfigurasyon bazlı olarak aşağıdaki config dosyasında tanımlayabiliriz.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.runtime.remoting>
        <application>
            <channels>
                <channel ref="Tcp Server" port="4500"/>
            </channels>
            <service>
                <wellknown mode="Singleton" type="ServerApp.ProductManager,ServerApp" objectUri="ProductMng.rem" />
            </service>
        </application>
    </system.runtime.remoting>
</configuration>
```

Konfigurasyon dosyası içerisinde tahmin edileceği üzere Wellknown bir nesne tanımı yapılmıştır. Bu nesne Singleton modeline göre çalışacaktır. Bir başka deyişle tüm istemcilerin talepleri aynı uzak nesne referansı üzerinden karşılanmaktadır. Sunucu uygulama, uzak nesne (Remote Object) için TCP bazlı 4500 numaralı portu kullanıma sunmaktadır. Buradaki konfigurasyon ayarlarının sunucu uygulama üzerinde tesis edilmesi içinde aşağıdaki başlangıç kodlarının yazılması yeterlidir.

```csharp
using System;
using AdvLibrary;
using System.Runtime.Remoting;

namespace ServerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            RemotingConfiguration.Configure("..\\..\\App.config",false);
            Console.WriteLine("Sunucu dinlemede. Kapatmak için bir tuşa basınız...");
            Console.ReadLine();
        }
    }
}
```

Bu noktada şunu hatırlamakta yarar vardır; sunucu uygulama çalıştığı sürece istemcilerden gelecek taleplere cevap verilebilir.

Gelelim istemci uygulamaya. Herşeyden önce istemci uygulamanında arayüzü barındıran ortak kütüphaneyi referans etmesi gerekmektedir. Diğer taraftan istemci uygulamanın kod içeriği aşağıdaki gibi olabilir.

```csharp
using System;
using AdvLibrary;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            IProductManager prdMng = (IProductManager)Activator.GetObject( typeof(IProductManager), "tcp://localhost:4500/ProductMng.rem");
            Product prd=prdMng.GetProductInfo(1);
            Console.WriteLine(prd.Name+" "+prd.ListPrice.ToString("C2"));
            Console.ReadLine();
        }
    }
}
```

Burada en kritik nokta, arayüz (interface) tercihi nedeniyle Activator sınıfının GetObject metodunun kullanılmasıdır. Bu metod ile aslında ikinci parametre ile belirtilen adresten ProductMng.rem isimli tanımlayıcının (Uniform Resource Identifier) işaret ettiği uzak nesne referansı talep edilmektedir. İstemciye döndürülecek olan referansın taşınabileceği tek yer IProductManager arayüzü olduğundan da metodun dönüş tipi bilinçli (Explicit) olarak çevrilmektedir. Buraya kadar yapılan hazırlıklar sonucunda test işlemi yapılırsa aşağıdakine benzer çıktılar elde edilir.

![mk233_3.gif](/assets/images/2007/mk233_3.gif)

Görüldüğü gibi istemciler başarılı bir şekilde sunucu üzerinden taleplerini karşılayabilmektedirler. (Burada oluşturulan sistemin düzgün çalıştığının kontrol edilmesi için sunucu uygulama çalıştırılmadan bir istemcinin çalıştırılması denenebilir. Eğer gerçekten sunucu üzerindeki nesne kullanılmaya çalışılıyorsa çalışma zamanında istemci tarafında "No Connection Could Be Made, Becouse the Target Machine Actively Refused It..." hatası alınır.)

Buraya kadar anlatılanlar basit olarak bir.Net Remoting uygulamasının alt yapısını özetlemektedir. Aşağıdaki şekil üzerinden, yapılanlar ve sonuçları tartışılabilir.

![mk233_4.gif](/assets/images/2007/mk233_4.gif)

Şekilde.Net Remoting içerisinde 50000 metre yukarıdan bakıldığında göze çarpan etkenler betimlenmeye çalışılmıştır. Şimdi bu sistemi Windows Communication Foundation alt yapısına taşımaya çalışıyor olacağız.

İlk yapılması gereken servis tarafında sunulmak istenen fonksiyonellikleri tanımlayacak bir sözleşme (Contract) oluşturmaktır. Burada söz konusu olan servis sözleşmesi (Service Contract) bir arayüze rahatlıkla uygulanabilir. Tek yapılması gereken System.ServiceModel.dll isimli.Net Framework 3.0 assembly'ının referans edilmesi ve ServiceContract ile OperationContract niteliklerinin (attributes) kullanılmasıdır. Aşağıdaki şekilde Visual Studio 2005 üzerinden söz konusu assembly'ın referans edilişi görülmektedir.

![mk233_5.gif](/assets/images/2007/mk233_5.gif)

Bu işlemin ardından IProductManager isimli arayüzün yapısı aşağıdaki gibi değiştirilmelidir.

```csharp
using System;
using System.ServiceModel;

namespace AdvLibrary
{
    [ServiceContract]
    public interface IProductManager
    {
        [OperationContract]
        Product GetProductInfo(int productId);
    }
}
```

Bu değişiklik arayüzün Windows Communication Foundation uyumlu bir serviş sözleşmesi (Service Contract) olması için yeterlidir. ServiceContract niteliği ile tanımlanan servis sözleşmesi içerisinden dışarıya sunulabilecek fonksiyonelliklerin tamamı OperationContract niteliği (attribute) ile imzalanmalıdır. Yapılan bu değişiklik her ne kadar ortak kütüphane içerisinde olsada sonuç itibariyle tüm istemcilere gerekli assembly'ın yeniden dağıtılması gerekmektedir. Ama zaten bu göz önüne alınması gereken bir aşamadır.

Gelelim sunucu tarafına. Sunucu tarafındada çok doğal olarak yapılması gereken bazı işlemler vardır. Herşeyden önce konfigurasyon içeriğinin WCF mimarisine uyumlu olacak şekilde değiştirilmesi gerekmektedir. Bu amaçla sunucu uygulama tarafındaki App.config dosyasının içeriği aşağıdaki gibi yenilenmelidir.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <!--<system.runtime.remoting>
                <application>
                    <channels>
                        <channel ref="Tcp Server" port="4500"/>
                    </channels>
                    <service>
                        <wellknown mode="Singleton" type="ServerApp.ProductManager,ServerApp" objectUri="ProductMng.rem" />
                    </service>
                </application>
            </system.runtime.remoting>-->
    <system.serviceModel>
        <services>
            <service name="ServerApp.ProductManager">
                <endpoint address="net.tcp://localhost:4500/ProductMng" binding="netTcpBinding" contract="AdvLibrary.IProductManager" />
            </service>
        </services>
    </system.serviceModel>
</configuration>
```

Windows Communication Foundation (WCF) mimarisinde servis için belkide en önemli kavram EndPoint'dir. EndPoint içerisinde WCF mimarisinin ABC'si yer almaktadır. AddressBindingConfiguration yardımıyla servisin hangi lokasyondan, hangi protokol ile, hangi nesneyi nasıl ve ne şekilde sunduğu belirtilmektedir..Net Remoting bazlı geliştirilen örnekte TCP bazlı ve Binary serileştirme yapan bir sistem kullanıldığından bu özellikleri bünyesinde barındıran bir tipin WCF tarafında ele alınması gerekmektedir. Bu işi NetTcpBinding isimli sınıf üstlenmektedir. Bu aynı zamanda bağlayıcı tiptir (Binding Type). Contract bilgisi ile tahmin edileceği üzere servisin dış ortama sunduğu sözleşme ve operasyonları belirtilmektedir. Son olarak Address bilgisi ile servis üzerinde sunulan nesneye hangi adres üzerinden erişilebileceği belirtilmektedir.

> WCF mimarisinde bir servis birden fazla EndPoint sunabilir. Bu geliştiricilere senaryoya göre birden fazla bağlayıcı tipin (Binding Type) ve dolayısıyla imkanın sunulabildiği bir ortam hazrılayacaktır. Söz gelimi bir servis uygulaması WCF'e göre yazıldığında, MSMQ, TCP, HTTPS üzerinden vb... hizmet verebilecek mesaj seviyesinde (Message Level) yada iletişim seviyesinde (Transport Level) korumalı hizmetleri sunabilir. Üstelik bunlar için ayrı ayrı uygulama çeşitleri tasarlanmasına gerek kalmaz. Bir başka deyişle bu tip bir senaryo için ayrı bir.Net Remoting, Xml Web Service veya MSMQ uygulaması yazılmasına gerek yoktur. İşte WCF'in birleştirici rolünün önemi burada belirgin bir biçimde ortaya çıkmaktadır.

Elbetteki sunucu uygulamanın kodlarınıda aşağıdaki gibi değiştirmek gerekmektedir. Artık ServiceHost isimli sınıf başlatıcı rolü üstlenmektedir.

```csharp
using System;
using AdvLibrary;
//using System.Runtime.Remoting;
using System.ServiceModel;

namespace ServerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //RemotingConfiguration.Configure("..\\..\\App.config",false);
            ServiceHost host = new ServiceHost(typeof(ProductManager));
            host.Open();
            Console.WriteLine("Sunucu dinlemede. Kapatmak için bir tuşa basınız...");
            Console.ReadLine();
            host.Close();
        }
    }
}
```

Örnek koda göre ServiceHost nesne örneği, ProductManager isimli sınıfı uzak nesne olarak hizmete açmaktadır. Open metoduna yapılan çağrıdan sonra, sunucu uygulama istemciden gelecek olan talepleri dinleyecek konuma gelmektedir. Close metoduna yapılan çağrı sonrasında ise servis kapatılmaktadır. Elbette ServiceHost sınıfının kullanılabilmesi için System.ServiceModel.dll assembly'ının sunucu uygulama tarafınada referans edilmesi gerekmektedir.

Peki istemci tarafında neler yapılmalıdır? Herşeyden önce istemci tarafında gerekli ayarların konfigurasyon bazlı olacak şekilde geliştirilmesi önerilmektedir. Bu amaçla istemci tarafına eklenecek olan bir konfigurasyon dosyasının içeriği, örneğe göre aşağıdaki gibi tasarlanabilir.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <client>
            <endpoint name="ProductMng" address="net.tcp://localhost:4500/ProductMng" binding="netTcpBinding" contract="AdvLibrary.IProductManager"/>
        </client>
    </system.serviceModel>
</configuration>
```

Konfigurasyon içeriğine bakıldığında dikkat edileceği üzere yine bir EndPoint tanımlaması yapılmaktadır. Bu EndPoint aslında istemcinin bağlantı kuracağı servis tarafındaki noktayı tanımlamaktadır. İstemci tarafındaki kod içeriği ise aşağıdaki gibi geliştirilebilir.

```csharp
using System;
using AdvLibrary;
using System.ServiceModel;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //IProductManager prdMng = (IProductManager)Activator.GetObject(typeof(IProductManager), "tcp://localhost:4500/ProductMng.rem");
            //Product prd=prdMng.GetProductInfo(1);

            ChannelFactory<IProductManager> chn = new ChannelFactory<IProductManager>("ProductMng");
            IProductManager prdMng=chn.CreateChannel();
            Product prd = prdMng.GetProductInfo(1);
            Console.WriteLine(prd.Name+" "+prd.ListPrice.ToString("C2"));
            Console.ReadLine();
        }
    }
}
```

ChannelFactory nesne üretimi için gerekli bilgileri, parametre olarak verilen değere göre konfigurasyon dosyasından almaktadır. Böylece servis tarafındaki hangi EndPoint ile konuşabileceğini bilmektedir. CreateChannel metodu önceden hazırlanan ayarlara göre, servis tarafı ile konuşacak olan kanal nesnesini örneklemektedir. Açıkçası CreateChannel metodunun sonucu.Net Remoting için kullanılan Activator.GetObject metodunun ürettiğine benzerdir. Nitekim aşağıdaki ekran görüntüsünden de anlaşılacağı üzere CreateChannel metoduda Transparent bir Proxy üretmektedir. Bu Proxy, çalışma zamanında servis üzerindeki EndPoint ile haberleşmektedir.

![mk233_7.gif](/assets/images/2007/mk233_7.gif)

Çok doğal olarak proxy nesnesinin üretilmesinde de kullanılan ChannelFactory sınıfı System.ServiceModel isim alanı (Namespace) altındadır. Bu nedenle söz konusu isim alanını içeren assembly'ın istemci uygulamayada referans edilmesi gerekmektedir.

Artık var olan.Net Remoting uygulaması bu bir kaç adım ile WCF formatına çevrilmiştir. Uygulamalar test edildiğinde aşağıdakine benzer sonuçlar alınacaktır.

![mk233_6.gif](/assets/images/2007/mk233_6.gif)

Elbetteki servisin ve operasyonların davranışları WCF mimarisi içerisinde çok daha geniş bir şekilde ele alınmaktadır. Örnekte bunlara gerek kalmadanda WCF'e geçilebileceği gösterilmektedir. Ancak ele alınması gereken başka noktalarda olabilir.

Örneğin her istemci için birer uzak nesne referansı sunucu üzerinde oluşturulmaktadır. Oysaki tasarlanan.Net Remoting alt yapısında Singleton modeli benimsenmiştir. Bir başka deyişle her istemci talebi (Request) aynı uzak nesne örneği üzerinden karşılanmaktadır. WCF tarafında bu, servis tarafında sunulan uzak nesnenin bir çalışma zamanı (run-time) davranışıdır (Behavior). Dolayısıyla bu davranışın bir nitelik (attribute) yardımıyla belirlenmesi yada konfigurasyon içeriğinde tanımlanması gerekmektedir. Bu sebepten örnekte yer alan ProductManager sınıfı başında ServiceBehavior niteliğinin (attribute) aşağıdaki gibi kullanılması gerekir.

```csharp
[ServiceBehavior(InstanceContextMode=InstanceContextMode.Single)]
public class ProductManager:MarshalByRefObject,IProductManager
```

InstanceContextMode enum sabitinin Single olarak belirlenen değeri örnekte ele alınan senrayoya uygun olacak şekilde uzak nesne referanslarının oluşturulmasını sağlamaktadır. Bunun sonucunda çalışma zamanındaki durum aşağıdaki gibi olacaktır.

![mk233_10.gif](/assets/images/2007/mk233_10.gif)

Bunun dışında binary olarak serileştirilebilen Product sınıfı için herhangibir ekstra çalışma yapılmadığı ortadadır. Nitekim böyle bir çalışma yapılmasına gerek kalmamıştır. Fakat versiyonlama yapıldığı durumlarda WCF mimarisi.Net Remoting'e göre daha efektif bir çözüm sunmaktadır.

Burada versiyonlamanın (Versioning) nasıl sorunlar yaşatabileceğini anlamak önemlidir. Söz gelimi Product isimli serileştirilebilir sınıfın sonradan yazılan ikinci bir versiyonu olduğu ve bu versiyonda işin içerisine SellStartDate isimli yeni bir alan (Field) katıldığı göz önüne alınsın. Eski Product sınıfını kullanan istemcilerin sorunsuz olarak onunla çalışmaya devam etmelerini, yeni versiyonu kullananların ise yeni eklenen alanı ele alabilmelerini sağlamak için kuvvetle muhtemel tasarımı aşağıdaki gibi değiştirmek gerekecektir.

![mk233_8.gif](/assets/images/2007/mk233_8.gif)

```csharp
using System;
using System.Runtime.Serialization;

namespace AdvLibrary
{
    [Serializable]
    public class Product:ISerializable
    {
        public int Id;
        public string Name;
        public double ListPrice;
        public DateTime SellStartDate; // Yeni versiyonlar için eklenen alan.
    
        public Product()
        {
    
        }

        public Product(SerializationInfo info, StreamingContext context)
        {
            Id = info.GetInt32("Id");
            Name = info.GetString("Name");
            ListPrice = info.GetDouble("ListPrice");
            // Versiyonlama nedeni ile oluşabilecek hataları görmezden gelmek için bir try...catch
            try
            {
                SellStartDate = info.GetDateTime("SellStartDate");
            }
            catch
            {
            }
        }

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);
            info.AddValue("Name", Name);
            info.AddValue("ListPrice", ListPrice);
            info.AddValue("SellStartDate", SellStartDate); // Yeni versiyon için ekenen kod satırı
        }
    
        #endregion
    }
}
```

Burada dikkat edilecek olursa serileştirme işlemleri ISerializable arayüzünün (Interface) kullanılmasıyla özelleştirilerek, değerlerin aktarımı kontrollü bir hale getirilmektedir. Özelleştirme için ISerializable arayüzünün uygulanması haricinde yapıcı metodun (Constructor) aşırı yüklenmiş (Overload) bir versiyonuda ele alınmaktadır. Sorunun detayları çok önemli olmamakla birlikte Windows Communication Foundation içerisinde bu tip versiyonlamalarında kontrol edilebilmesi son derece kolaydır. Tek yapılması gereken aşağıdaki gibi Product sınıfını bir veri sözleşmesi (Data Contract) haline getirmektir.

```csharp
using System;
using System.Runtime.Serialization;

namespace AdvLibrary
{ 
    [DataContract]
    public class Product
    {
        [DataMember(IsRequired=true)] // Mutlaka olmalı
        public int Id;

        [DataMember(IsRequired = true)] // Mutlaka olmalı
        public string Name;

        [DataMember(IsRequired = true)] // Mutlaka olmalı
        public double ListPrice;
        
        [DataMember] // Opsiyonel oldu.
        public DateTime SellStartDate; // Yeni versiyonlar için eklenen alan.
    }
}
```

DataMember niteliği (attribute) varsayılan olarak uygulandığı üyeye opsiyonel bir davranış getirmektedir. Bir başka deyişle çalışma zamanında karışa tarafın bu alanı içermeyen bir tipi kullanması halinde hiç bir problem oluşmayacak ve bu alan görmezden gelinecektir. Tam tersine bu alanı kullanan bir versiyon varsa SellStartDate alanıda hesaba katılacaktır. Bu sayede versiyonlamada çok kolay bir şekilde ele alınabilmektedir. Product sınıfının tasarlanmasında dikkat edilmesi gereken önemli bir nokta vardır. Buna göre DataContract ve DataMember niteliklerinin (attributes) tanımlandıkları isim alanı (Namespace), System.Runtime.Serialization.dll assembly'ı içerisindedir. Dolayısıyla sınıf kütüphanesine bu assembly'ın aşağıdaki ekran görüntüsünde olduğu gibi referans edilmesi gerekmektedir.

![mk233_9.gif](/assets/images/2007/mk233_9.gif)

Son olarak ele alınması gereken bir durum daha olabilir. İstemci ve servis tarafının.Net Framework 2.0 yüklü sistemler olduğu göz önüne alındığında yapılan son değişikliker sonrasında uygulamalar sorunsuz bir şekilde çalışabilecek midir?(Burada iyimser bir yaklaşım güdülerek.Net Framework 1.1 versiyonu hiç hesaba katılmamıştır.)

> Yukarıdaki referans ekleme kısmında dikkat çeken noktalardan biriside Framework 3.0 için geliştirilmiş pek çok assembly'ın çalışma zamanında (Run Time) v2.0.50727 versiyonuna ihtiyaç duymasıdır. Bir başka deyişle System.ServiceModel veya System.Runtime.Serialization gibi 3.0 assembly'ları,.Net Framework 2.0 yüklü bir makinede belleğe yüklenip çalışabilirler. Nitekim ihtiyaçları olan v2.0.50727 versiyonlu Common Language Runtime (CLR)' dır.

Aslında sistemler üzerinde.Net Framework 3.0' ın yüklü olması sorunun çözümü için yeterlidir. Lakin sadece.Net Framework 2.0 yüklü olan bir sistemde değiştirilen uygulamalar çalıştırılabilirse, bir.Net Framework 3.0 yükleme derdinden kurtulunabilinir. Ancak bunun bir dert olup olmadığıda tartışılması gereken bir vakadır.

Söz konusu durumu test etmek için servis veya istemci uygulamaları, üzerinde.Net Framework 2.0 yüklü olan bir sistemde, System.ServiceModel.dll ve System.Runtime.Serialization.dll assembly'larınıda kopyalayarak çalıştırmak yeterli olacaktır. Başlangıçta servis ve istemci tarafında var olması düşünülen exe, config ve dll dosyaları aşağıdaki gibidir.

![mk233_11.gif](/assets/images/2007/mk233_11.gif)

Ne yazıkki buradaki dll'lerin hedef bilgisayara (bilgisayarlara) taşınmaları yeterli olmamaktadır..Net Framework 2.0 yüklü makinede örnek olarak ServerApp uygulaması çalıştırıldığında aşağıdaki ekran görüntüsünde yer alan hata ile karşılaşılabilir.

![mk233_12.gif](/assets/images/2007/mk233_12.gif)

Bunun üzerine SMDiagnostics.dll isimli assembly'ında hedef bilgisayarda ServerApp ve ClientApp program dosyalarının olduğu yere konması gerekir. Söz konusu assembly hedef programların bulunduğu lokasyona taşındıktan sonra bir deneme daha yapılırsa aşağıdakine benzer bir hata mesajı ile daha karşılaşılabilinir.

![mk233_16.gif](/assets/images/2007/mk233_16.gif)

Bu hatanın oluşmasının en büyük nedeni, programın çalıştığı makine üzerinde.Net Framework 3.0 yüklü olmadığı için, machine.config'de olması gereken bazı ayarların bulunmayışından kaynaklanmaktadır. Bu nedenle ServiceModelReg aracının kullanılarak gerekli konfigurasyon bilgilerinin hedef makineye yüklenmesi gerekecektir. Bu sebepten komut satırından aşağıdaki gibi bir çalışma yapılmalıdır.

![mk233_14.gif](/assets/images/2007/mk233_14.gif)

Bu çalışmanın ardından hedef bilgisayarda yer alan konfigurasyon dosyas için gerekli Handler ve Module yüklemeleri gerçekleştirilecektir. Artık herşey halloldu diye düşünülebilir ama bu seferde ServerApp uygulaması çalıştırıldığında aşağıdakine benzer bir hata mesajı daha alınabilir.

![mk233_15.gif](/assets/images/2007/mk233_15.gif)

Çok doğal olarak System.IdentityModel.dll'ininde kopyalanmış olması gerekecektir. Bu hata beraberinde System.IdentityModel.Selectors.dll isimli assembly'ında yüklenmesini gerektirmektedir. Her iki assembly yüklensede bu kez doğrulama (Authentication) ve yetkilendirme (Authorization) problemleri oluşmaktadır. Bunların çözümü adına config dosyalarında gerekli düzenlemele yapılabilir. Yinede bu sancılı süreç hemen çözümlenemeyecektir. Yazının ilerleyen kısımlarında konunun daha fazla dağılmaması adına bu yöntemler göz ardı edilmiştir.

Sonuç olarak her ne kadar.Net Remoting'den Windows Communication Foundation tarafına geçiş yapmak kolay olsada, gerçek çalışma ortamında özellikle.Net Framework 2.0 yüklü sistemlerde bazı ayarlamaların yapılması gerekmektedir. Bu durum.Net Framework 1.1 yüklü makinelerde tam bir kabus haline gelebilir. Nitekim kopyalanan dll'lerin bazıları CLR 1.1 versiyonunda çalışmayacaktır. Bu nedenle.Net Framework 3.0' ın Redistruable paketinin hedef sistemlere yüklenerek çalışmalara devam edilmesi doğru bir davranış olacaktır. Hatta istemciler için bir setup paketinin hazırlanması ve gerekliliklerden birisi olarak.Net Framework 3.0 seçiminin eklenmesi çok yerinde bir davranıştır. O halde cevaplanması gereken bir soru daha vardır. Bu kadar zahmete girilecekse, sistemin WCF için yeninde yazılması yoluna gidilemez midir? Aslında bu sorunun cevabı daha önceden tasarlanıp kullanılılmakta olan.Net Remoting sistemine bağlıdır. Eğer yazılan çok fazla kod ve birbirlerine bağlı parça var ise, bir iki düzenleme ile WCF'e uyumlu bir sistem oluşturmak son derece kolaydır.

Bu noktalarda vakayı doğru değerlendirip karar vermekte ve gerekirse cevap için profesyonel destek almakta yarar olabilir.Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](/assets/files/2007/FromRemotingToWCF.rar)