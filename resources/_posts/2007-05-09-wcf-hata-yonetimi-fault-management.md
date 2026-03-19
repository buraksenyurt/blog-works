---
layout: post
title: "WCF - Hata Yönetimi (Fault Management)"
date: 2007-05-09 06:00:00 +0300
description: "Hata yönetimi her programlama dili ve geliştirme ortamı içerisinde yer alan önemli konulardan birisidir. Özellikle kullanıcıların yapmış olduğu işlemler sonucunda oluşan veya sistem üzerinde beklenmeyen durumlardan doğan hataların önüne geçmek amacıyla çeşitli mekanizmalara başvurulmaktadır."
categories:
  - wcf
tags:
  - wcf
  - csharp
  - xml
  - bash
  - dotnet
  - soap
  - web-service
  - xml-web-services
  - http
  - java
  - serialization
  - delegates
  - generics
  - visual-studio
  - dataset
---
# WCF - Hata Yönetimi (Fault Management)
Değerli Okurlarım Merhabalar,

Hata yönetimi her programlama dili ve geliştirme ortamı içerisinde yer alan önemli konulardan birisidir. Özellikle kullanıcıların yapmış olduğu işlemler sonucunda oluşan veya sistem üzerinde beklenmeyen durumlardan doğan hataların önüne geçmek amacıyla çeşitli mekanizmalara başvurulmaktadır. Bunlardan birisi ve aynı zamanda etkili olanıda istisna yönetimidir (Exception Handling). Microsoft.Net ortamında istisna yönetimi CLR (Common Language Runtime - Ortak Dil ÇalışmaZamanı) tarafından gerçekleştirilen bir unsurdur.

Ne varki, basit bir windows veya web uygulamasında etkili bir şekilde ele alınabilen istisnalar, dağıtık mimari uygulamaları (Distributed Applications) göz önüne alındığında daha farklı yaklaşılmak durumundadır. Bu durumu daha iyi anlayabilmek için, dağıtık mimari uygulamalarının taraflarını göz önüne almakta yarar vardır. Biz her ne kadar sunucu tarafında.Net ortamını kullanıyor olsakta, istemci açısından durum aynen geçerli değildir. Örneğin dağıtık mimari uyarlamalarından birisi olan Xml Web Servisleri düşünüldüğünde istemcinin farklı platformda yer alan bir uygulama olması muhtemeldir. Öyleyse düşünülmesi gereken iki önemli nokta vardır.

Bunlardan birincisi, istemcilerin farklı bir platformda olabileceğidir. Bu CLR tarafınan ele alınabilecek bir istisnanın istemci tarafında ele alınamaması için yeter bir sebeptir. Çünkü istemci tarafında bir CLR ortamı bulunmayabilir. İkinci önemli nokta ise sunucu üzerinde bir istisna oluşursa bunun istemci tarafına nasıl bildirileceğidir. Bir başka deyişle sunucu tarafında yakalanan hatanın, farklı bir makinedeki farklı bir bellek bölgesinde çalışan bir uygulamanın yakalayabileceği şekilde gönderilebilmesi gerekmektedir. Bu iki noktanın oluşturduğu sorunu çözmenin tek yolu herkesin kabul ettiği ortak bir standarda göre istisna bilgisi yayınlamaktır. İşte bu noktada devreye SOAP (Simple Object Access Protocol) girer. Bu protokol örnek olarak web servislerinde oluşacak istisnaları istemci tarafına Soap Fault mesajları olarak gönderir ki bu mesaj çeşidi tüm platformlar tarafında kabul edilmiştir.

> Soap Fault hakkında daha fazla bilgi için [http://www.w3.org/TR/soap12-part1/#soapfault](http://www.w3.org/TR/soap12-part1/#soapfault) adresinden faydalanabilirsiniz.

SoapFault, sunucu tarafından fırlatılabilecek istisnaların nasıl bir formata sahip olması gerektiğine dair kuralları içerir. Örneğin tüm SoapFault mesajlarında Code (Hata kodu bilgisi), Reason (Hatanın sebebi) gibi hataya ait detaylı bilgi alınmasını sağlayan özellikler yer almaktadır..Net Framework, J2EE gibi örnek plaftormlar bu kurallara uygun olacak şekilde tasarlanmış sınıflar sunarlar. Böylece, yazacağımız servisleri kullanacak istemcilerin tamamı için ele alınabilecek istisna mesajları oluşturma şansına sahip oluruz. Peki Windows Communication Foundation için durum nasıldır?

WCF içerisindede istemcilerin ele alabileceği şekilde hata mesajları tanımlayabilmek için FaultException isimli sınıf geliştirilmiştir. Aslında bir WCF servisi içerisinde istemci uygulamalara fırlatılabilecek istisna mesajları, karşı tarafa geçtiklerinde her zaman bir FaultException örneği olarak ele alınırlar. Ancak önemli olan bir nokta vardır ki buda, servis üzerinde oluşan istisnanın mutlaka karşı tarafa (istemci uygulamaya) fırlatılması (throw) gerektiğidir.

Çoğu zaman servis tarafında meydana gelen istisnalar için, istemciye daha fazla bilgi taşıyabilecek şekilde kendi istisna nesnelerimizi organize etmek isteyebiliriz..Net tarafından yaklaştığımızda bunun yolu ApplicationException sınıfından bir tip türetmektir. Lakin WCF için durum biraz daha farklıdır. Nitekim oluşturulan istisna nesne örneğinin Soap Fault standartlarına uygun olacak şekilde istemci tarafına bir mesaj olarak gönderilebilmesi ve orada referansının ele alınabilmesi (handle) gerekmektedir.

WCF mimarisini tanımaya çalıştığımız ilk makalemizde dört çeşit sözleşme (contract) olduğundan bahsetmiştik. Bunlardan biriside hata sözleşmesidir (Fault Contract). Hata sözleşmeleri basit olarak, servis tarafında üretilen kullanıcı tanımlı hataların istemciye nasıl taşınması gerektiğini bildiren bir sözleşme çeşidir. Geliştirici olarak kendi hata bildirimlerimizi yapmak için öncelikli olarak hataya ait bilgileri taşıyacak bir veri sözleşmesininde (data contract) tanımlanması gerekmektedir. Makalemizin ilerleyen kısımlarında bu işlemlerin nasıl yapılacağını adım adım inceleyeceğiz.

WCF için, hata yönetimi ile ilişkili olarak dikkat edilmesi gereken bir diğer noktada, servisin açılması ve kapanması arasında meydana gelebilecek bazı beklenmedik tepkilere karşı nasıl tedbir alınması gerektiğidir. Her servis çalışmadan önce nesne olarak örneklenmektedir. Bunun programatik olarak ServiceHost sınıfına ait bir nesne örneğinin oluşturulması olarak düşünebiliriz. Sonrasında servis açılır ve istemcilerden gelecek olan talepler karşılanmaya başlar. Ne varki servis açılırken, açıldığında ve çalışırken servis tarafında bazı beklenmedik hatalar oluşabilir.

Bu gibi durumlarda servise ait Faulted olayı tetiklenmektedir. Dolayısıyla bu olay içerisinde gereken hazırlıklar yapılarak servisin tekrar ayağa kaldırılması denenebilir. Burada servisin yaşam döngüsününde bilinmesinde fayda vardır. Bu konuyuda makalemizin sonunda ele almaya çalışacağız. Şimdi vakit kaybetmeden örnek bir senaryo üzerinden gidelim. Basit olarak Tcp bağlayıcısını kullanan Console tabanlı sunucu ve istemci uygulamaların yer aldığı bir WCF senaryosu geliştireceğiz. Öncelikli olarak servis sözleşmesi ve uyarlamayı yapan tipi içeriside barındıran RemoteLib isimli WCF Servis kütüphanemizi (WCF Service Library) aşağıdaki gibi tasarlayalım.

![mk203_1.gif](/assets/images/2007/mk203_1.gif)

INorthwind arayüzümüz tahmin edeceğiniz gibi servis sözleşmemizi (Service Contract) ifade etmektedir.

```csharp
using System;
using System.Data;
using System.ServiceModel;

namespace RemoteLib
{
    [ServiceContract(Name="NorthManagerService", Namespace="http://www.bsenyurt.com/NorthService")]
    public interface INorthwind
    {
        [OperationContract(Name="GetCustomers")]
        DataSet GetCustomers(); 
    }
}
```

Servis sözleşmemizi uygulayan NorthManager isimli sınıfımızın içeriği ise şu an için aşağıdaki gibidir.

```csharp
using System;
using System.Data;
using System.ServiceModel;
using System.Data.SqlClient;

namespace RemoteLib
{
    public class NorthManager:INorthwind
    {
        #region INorthwind Members

        public DataSet GetCustomers()
        {
            DataSet ds = null;
            SqlConnection conn=null;
            try
            {
                conn= new SqlConnection("data source=.;database=AdventureWorks;integrated security=SSPI");
                SqlDataAdapter da = new SqlDataAdapter("Select CustomerID,CompanyName,ContactName,ContactTitle From Customers", conn);
                ds = new DataSet("CustomersSet");
                da.Fill(ds);
            }
            catch (SqlException excp)
            {
                Console.WriteLine(excp.Message);
            }
            finally
            {
                conn.Close();
            }
            return ds;
        }

        #endregion
    }
}
```

NorthManager sınıfımız içerisinde yer alan GetCustomers isimli metodumuz basit olarak Northwind veritabanında yer alan Customers isimli tablodan veri çekip sonuç kümesini bir DataSet olarak geriye döndürmektedir. Ancak SqlConnection nesne örneği oluşturulurken veritabanı adı olarak Northwind yerine AdventureWorks adı verilmiştir. Bu, çalışma zamanında çok doğal olarak bir istisnaya neden olacaktır.

İstisna servis tarafı açısından düşünüldüğünde GetCustomers metodu içerisindeki catch bloğu tarafından yakalanacaktır. Peki ya sonrasında ne olacaktır? Gerçekten istemci uygulama burada SqlException istisna nesne örneğini yakalayabilecek midir? Bu sorulara net bir cevap verebilmek için önce sunucu uygulamamızı, ardından istemci uygulamamızı geliştirerek incelemelerimize devam edelim. Servis uygulamamız bir konsol programı olarak tasarlanabilir. WCF için gerekli ayarları konfigurasyon dosyasında tutacağız. Konfigurasyon dosyamızın içeriği şu an için aşağıdaki gibi yazılabilir.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <services>
            <service name="RemoteLib.NorthManager">
                <endpoint address="net.tcp://localhost:65002/NorthService" binding="netTcpBinding" bindingConfiguration="" name="NorthServiceEndPoint" contract="RemoteLib.INorthwind" />
            </service>
        </services>
    </system.serviceModel>
</configuration>
```

NetTcpBinding bağlayıcı tipini kullandığımız için TCP protokolü üzerinden binary formatlama gerçekleşek şekilde iletişim kurabiliriz. Servisimize istemcilerin ulaşabilmesi içinde net.tcp://localhost:65002/NorthService adresi kullanılmaktadır. Elbette siz örneği denerken istediğiniz port numarasını ele alabilirsiniz. Servis sözleşmemizde contract niteliği içerisinde bildirilmiştir. Sunucu uygulamaya ait program kodlarımız ise aşağıdaki gibi olacaktır.

```csharp
using System;
using System.ServiceModel;

namespace ServerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(RemoteLib.NorthManager));
            host.Open(); 
            Console.WriteLine("Sunucu  dinlemede...");
            Console.ReadLine();
            host.Close();
        }
    }
}
```

Servis uygulaması önce ServiceHost tipinden bir nesne örneği oluşturmakta ve sonrasında servisi kullanıma açmaktadır. Kullanıcı uygulamayı tuşa basarak kapattıktan sonra açılan servis içinde kapatma emri Close metodu ile verilmektedir. Gelelim istemci tarafına. HTTP üzerinden metadata yayınlaması yapmadığımız için bir önceki WCF makalesinde yaptığımız gibi svcutil.exe aracını RemoteLib.dll assembly'ı üzerinde kullanmamız ve istemci için gerekli proxy sınıfı ile konfigurasyon dosyasını üretmemiz gerekmektedir. Bu amaçla komut satırından svcutil aracını aşağıdaki gibi kullanalım.

Önce;

```bash
svcutil RemoteLib.dll
```

Sonrasında ise;

```bash
svcutil /namespace:*,RemoteLib.NorthManager www.bsenyurt.com.NorthService.wsdl *.xsd
```

Tahmin edeceğiniz gibi komut satırından yaptığımız bu işlemlerin sonrasında istemci için gereken proxy sınıfı ve config dosyaları başarılı bir şekilde oluşturulacaktır. İstemci uygulamamızın konfigurasyon dosyasını isterseniz daha basit olması açısından aşağıdaki gibi oluşturabilirsiniz. Nitekim svcutil ile oluşturulan standart output.config dosyası oldukça fazla element bilgisi içermektedir. Tek dikkat edilmesi gereken nokta contract niteliğine doğru tip bilgisini girmektir.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <client>
            <endpoint address="net.tcp://localhost:65002/NorthService" binding="netTcpBinding" bindingConfiguration="" contract="RemoteLib.NorthManager.NorthManagerService" name="NorthClientEndPoint" />
        </client>
    </system.serviceModel>
</configuration>
```

Bundan sonra istemci uygulamamızın kod satırlarınıda aşağıdaki gibi geliştirelim.

```csharp
using System;
using System.Data;
using System.ServiceModel;
using RemoteLib.NorthManager;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                NorthManagerServiceClient srvClient = new NorthManagerServiceClient("NorthClientEndPoint");
                DataSet customers = srvClient.GetCustomers();
                if(customers.Tables.Count!=0)
                    Console.WriteLine(customers.Tables[0].Rows.Count.ToString());
            }
            catch (FaultException excp)
            {
                Console.WriteLine(excp.Code.Name+ "\n" + excp.Reason.ToString());
            }
        }
    }
}
```

İstemci tarafındaki kod parçamızıda dikkat ederseniz try...catch blokları arasına aldık. Artık testimize başlayabiliriz.

> İstemcinin gereken taleplerde bulunabilmesi için sunucu uygulamanın çalışıyor ve açık olması gerekecektir. Visual Studio içerisinde debug işlemleri yapmak isteyebileceğimizden Solution özelliklerinden Multiple Startup Projects seçeneğini aşağıdaki gibi set ederek F5 (Start) veya Ctrl+F5 (Start without debuging) sonrasında önce sunucunun sonrasında ise istemcinin sırayla çalıştırılmalarını sağlayabiliriz.
> ![mk203_3.gif](/assets/images/2007/mk203_3.gif)

Sonuç olarak aşağıdaki ekran çıktılarını alırız.

![mk203_2.gif](/assets/images/2007/mk203_2.gif)

Dikkat ederseniz, sunucu tarafındaki catch bloğuna girilmiş ve oluşan istisnaya uygun bir biçimde ekrana mesaj çıktısı verilmiştir. Ne varki bu istisna mesajını istemci tarafına gönderebilmiş değiliz. Bunun için baştada belirttiğimiz gibi servis uygulamasında bilinçli bir şekilde FaultException nesne örneğinin oluşturulması gerekmektedir. Bu amaçla NorthManager sınıfımızın GetCustomers metodundaki catch bloğunu aşağıdaki gibi düzenleyelim.

```csharp
catch (SqlException excp)
{
    Console.WriteLine(excp.Message);
    throw new FaultException(excp.Message, new FaultCode("Veritabani istisnası"));
```

FaultException sınıfının farklı şekilde yüklenmiş yapıcı metodlar (constructors) vardır. Burada kullanılan versiyonda ilk parametre olarak hatanın sebebi (Reason) ikinci parametre olarakta hata kodu (FaultCode) verilmektedir. Oluşan istisna sonrasında üretilecek ve istemciye gidecek olan SoapFault mesajı içerisinde, yapıcı metod içerisinde kullandığımız parametre bilgileri yer alacaktır. Buna göre uygulamalarımızı tekrar çalıştırırsak aşağıdaki ekran görüntülerini elde ederiz.

![mk203_4.gif](/assets/images/2007/mk203_4.gif)

Gördüğünüz gibi artık istemci tarafında, sunucudan fırlatılan FaultException nesnesinin içeriğini yakalayabilmekteyiz. Servis tarafında yer alan NorthManager sınıfına ait GetCustomers metodunda istisna oluştuğunda, istemci uygulamalara FaultException tipinden bir nesne örneği fırlatmaktansa, normal bir Excpetion nesnesi fırlatılmasıda denenebilir (yada başka tipte bir istisna nesne örneği). Bu durumda istemci uygulama çok farklı bir mesaj alacaktır. Durumu analiz etmek için GetCustomers metodu içerisinde aşağıdaki değişiklikleri yapalım.

```csharp
throw new Exception("Veritabanı adı yanlış");
```

Testi tekrar yaptığımızda istemci uygulamamız için aşağıdaki ekran görüntüsünü elde ederiz.

![mk203_5.gif](/assets/images/2007/mk203_5.gif)

Dikkat ederseniz istemci tarafına son derece enteresan bir çıktı gelmiştir. Asıl hata mesajının içeriğini alabilmek için servis tarafındaki konfigurasyon bilgilerinde biraz değişiklik yapmak gerekecektir. Öncelikle olarak bir servis davranışı (service behavior) tanımlanmalıdır. Servis davranışının (Service Behavior) serviceDebug özelliğinin includeExceptionDetailInFaults isimli niteliğinede true değeri atanmalı ve son olarak bu davranış servise bildirilmelidir. Söz konusu değişiklikleri servis uygulamasının App.config isimli konfigurasyon dosyasına aşağıdaki gibi ekleyebiliriz.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <behaviors>
            <serviceBehaviors>
                <behavior name="NorthServiceBehavior">
                    <serviceDebug includeExceptionDetailInFaults="true" />
                </behavior>
            </serviceBehaviors>
        </behaviors>
        <services>
            <service behaviorConfiguration="NorthServiceBehavior" name="RemoteLib.NorthManager">
                <endpoint address="net.tcp://localhost:65002/NorthService" binding="netTcpBinding" bindingConfiguration="" name="NorthServiceEndPoint" contract="RemoteLib.INorthwind" />
            </service>
        </services>
    </system.serviceModel>
</configuration>
```

Böylece servis tarafında ürettiğimiz FaultException dışındaki bir istisnanın istemci tarafına taşınması mümkün olabilir ki istemci uygulamayı tekrar test ettiğimizde aşağıdaki ekran görüntüsünü elde edebiliriz.

![mk203_6.gif](/assets/images/2007/mk203_6.gif)

Burada dikkat edilmesi gereken önemli bir nokta daha vardır. Servis uygulaması tarafında her nekadar bir Exception sınıfına ait nesne örneği fırlatsakta, istemci tarafında bu yine FaultException sınıfına ait bir örnek olarak ele alınacaktır. Bunu görebilmek için istemci uygulamada FaultException yerine Exception nesnesi yakalanmaya çalışabilir. Uygulamayı debug ettiğimizde aynen aşağıdaki ekran görüntüsünde olduğu gibi istisna tipinin aslında FaultException olduğu görülecektir.

![mk203_7.gif](/assets/images/2007/mk203_7.gif)

Gelelim daha güçlü hata istisna nesnelerinin nasıl oluşturabileceğimize (Strongly Typed Faults). Yazımızın başındada belirttiğimiz gibi, öncelikle istemci tarafına gönderilecek olan mesajın bir veri sözleşme olarak tanımlanması gerekmektedir. Bu amaçla aşağıdaki gibi bir sınıfı RemoteLib içerisinde tanımlayabiliriz.

![mk203_8.gif](/assets/images/2007/mk203_8.gif)

TabloAdiFault isimli sınıfımız tipik olarak bir veri sözleşmesi (data contract) tanımlamaktadır.

```csharp
using System;
using System.Runtime.Serialization;

namespace RemoteLib
{
    [DataContract]
    public class TabloAdiFault
    {
        private string _faultCode;
        private string _Message;
        private string _Reason;

        [DataMember]
        public string FaultCode
        {
            get { return _faultCode; }
            set { _faultCode = value; }
        }
        [DataMember]
        public string Message
        {
            get { return _Message; }
            set { _Message = value; }
        }
        [DataMember]
        public string Reason
        {
            get { return _Reason; }
            set { _Reason = value; }
        }
        public TabloAdiFault(string faultCode,string message,string reason)
        {
            Reason = reason;
            Message = message;
            FaultCode = faultCode;
        }
    }
}
```

Burada dikkat edilmesi gereken noktalardan biriside DataContract ve DataMember niteliklerinin kullanılabilmesi için System.Runtime.Serialization.dll assembly'ının projeye dahil edilmiş olması gerekmektedir. (Biz uygulamamızı WCF Service Library şablonundan tasarladığımız için bu assembly'lar otomatik olarak referans edilmiş olacaktır.) Artık tanımlamış olduğumuz bu sınıfın, istemcilere FaultException olarak gönderilmesi için gereken hazırlıkları yapabiliriz. Öncelikli olarak bu verinin hangi metodlardan döndürülecekse FaultContract niteliği yardımıyla bildirilmesi gerekmektedir. Bu nedenle servis sözleşmemizde gerekli tanımlamayı aşağıdaki gibi yapmamız yeterli olacaktır.

```csharp
[ServiceContract(Name="NorthManagerService",Namespace="http://www.bsenyurt.com/NorthService")]
public interface INorthwind
{
    [FaultContract(typeof(TabloAdiFault))]
    [OperationContract(Name="GetCustomers")]
    DataSet GetCustomers(); 
}
```

FaultContract niteliği parametre olarak metoddan döndürülebilecek tipe ait bir bilgi içermektedir. Bu bilginin istemci tarafına serileştirilerek (Serializable) gitmesi gerektiği için, zaten veri sözleşmesi olarak tanımlanmıştır. Elbette istisnaları fırlatırkende yapmamız gereken bazı işlemler olacaktır. Bu amaçla GetCustomers metodumuzun içeriğini aşağıdaki gibi değiştirelim.

```csharp
public DataSet GetCustomers()
{
    DataSet ds = null;
    SqlConnection conn=null;
    try
    {
        conn= new SqlConnection("data source=.;database=AdventureWorks;integrated security=SSPI");
        SqlDataAdapter da = new SqlDataAdapter("Select CustomerID,CompanyName,ContactName,ContactTitle From Customers", conn);
        ds = new DataSet("CustomersSet");
        da.Fill(ds);
    }
    catch (SqlException excp)
    {
        Console.WriteLine(excp.Message);
        TabloAdiFault tblFault = new TabloAdiFault("TabloAdi", excp.Message, "Tablo adı yanlış");
        throw new FaultException<TabloAdiFault>(tblFault);
    }
    finally
    {
        conn.Close();
    }
    return ds;
}
```

Öncelikle TabloAdiFault sınıfımızı örnekliyoruz. Bu bir exception sınıfı olmadığından istemci tarafına fırlatılabilmesi için FaultException sınıfının ilgili yapıcı metoduna parametre olarak verilmesi gerekmektedir. Bu işlem için Framework içerisinde FaultException sınıfının generic bir versiyonu kullanılmaktadır. Artık istemci tarafını aşağıdaki gibi kodlayabiliriz. Ancak bu işlemlerin ardından istemciler için gerekli proxy sınıflarını yeniden oluşturmamız gerekecektir. Dolayısıyla svcutil aracını tekrardan ele almalıyız. Svcutil aracını kullandığımız takdirde oluşan proxy sınıfı içerisinede TabloAdiFault sınıfının getirildiğini görebiliriz.

![mk203_9.gif](/assets/images/2007/mk203_9.gif)

Çok doğal olarak sadece public üyeler ve bunlara ilişkin alanlar bu sınıf içerisine dahil edilmiştir. Ama aynı zamanda ExtensionDataObject tipinden değerler ile çalışan bir özellikte ilave edilmiştir ki bu özellik IExtensibleDataObject arayüzünden uygulanmaktadır. Artık istemci tarafındaki catch bloğumuzu aşağıdaki gibi kodlayabiliriz.

```csharp
catch (FaultException<TabloAdiFault> excp)
{
    Console.WriteLine("Hata Kodu : "+excp.Detail.FaultCode.ToString() + "\nHata Mesajı : " + excp.Detail.Message + "\nSebep : " + excp.Detail.Reason);
}
```

Detail özelliği aslında generic tipimize ait referansı bir başka deyişle TabloAdiFault nesne örneğini ele almaktadır.

![mk203_10.gif](/assets/images/2007/mk203_10.gif)

Dolayısıyla Detail üzerinden, TabloAdiFault sınıfı içerisinde tanımlanmış özelliklere erişebiliriz. Öyleki çalışma zamanında bu özelliklerin değerleri servis uygulaması tarafında üretilip istemci tarafına taşınacaktır. O halde uygulamayı deneyip çalışmanın sonuçlarını görebiliriz. Aşağıdaki ekran görüntüsünde bu sonuçlar yer almaktadır.

![mk203_11.gif](/assets/images/2007/mk203_11.gif)

WCF uygulamalarında hata yönetimi adına sunucu tarafındada yapılması gerekenler olabilir. Özellikle servisin açılması, açıldıktan sonra kapatılana kadar geçen süre içerisinde bazı beklenmeyen sistem hataları meydana gelebilir. Bu gibi durumlarda istenirse ServiceHost sınıfına ait Faulted olay metodu kullanılabilir ve hataların ele alınması sağlanabilir. Faulted olayını daha iyi kavrayabilmek için aslında bir servisin yaşam çemberini (life cycle) incelemekte ve hangi durumlarda Faulted olayının tetikleneceğini bilmekte fayda vardır.

Temel olarak bir servis uygulamasının oluşturulması (Create), açılması (Open), iptal edilmesi (Abort) veya durdurulması (Stop) gibi durumlar söz konusudur. Bu durumlar arasında geçişler yapılabilmesi içinde bazı metodların çağırılması gerekmektedir. Söz gelimi servis nesnesi oluşturulduktan sonra açmak için Open metodu çağırılır. Bu gibi metod çağrıları sonrasında servisin durumu (state) sürekli değişecektir. İşte bu geçişler (transitions) sırasında oluşabilecek bazı beklenmedik hatalara karşılık Faulted olayı ele alınabilir.

> ServiceHost sınıfının bir servisin durumunu öğrenebilmek amacıyla CommunicationState isimli enum sabitinden değerler döndüren State isimli sadece okunabilir (read-only) bir özelliği vardır. Bu enum sabitinin alabileceği değerler Created, Opening, Opened, Faulted, Closing, Closed dır.

Aşağıdaki çizelgede bir servisin alabileceği durumlar, bu durumlar arasında geçiş yapılması için gereken metodlar ve Faulted olayının devreye girebileceği zamanlar ifade edilmeye çalışılmaktadır.

![mk203_12.gif](/assets/images/2007/mk203_12.gif)

Şekildende göreceğiniz gibi, Faulted olayının tetiklenmesi sonrasında ele alınabilecek senaryolardan birisi servisin o ana kadar olan tüm işlemlerini iptal etmek (Abort) ve servisi tekrardan oluşturup açmayı denemek olacaktır. Abort metodu çağırıldığında eğer askıda bekleyen talepler varsa bunlara cevap verilmesi beklenmez. Oysaki Close metodu çağrıldığında askıda bekleyen talepler (request) var ise bunlar cevaplanır ama emir verildikten sonra istemciden yeni talepler alınmaz. Yukarıdaki senaryoyu uygulamak istediğimizde servis tarafındaki kodlarımızı aşağıda olduğu gibi geliştirebiliriz.

```csharp
using System;
using System.ServiceModel;

namespace ServerApp
{
    class Program
    {
        static ServiceHost host;

        static void Main(string[] args)
        {
            host = new ServiceHost(typeof(RemoteLib.NorthManager));
            host.Faulted += delegate(object sender, EventArgs e)
            {
                host.Abort();
                host = new ServiceHost(typeof(RemoteLib.NorthManager));
                host.Open();
            };
            host.Open(); 
            Console.WriteLine("Sunucu dinlemede...");
            Console.ReadLine();
            host.Close();
        }
    }
}
```

Dikkat ederseniz Faulted olay metodu içerisinde önce servis Abort metodu ile iptal edilmekte, sonrasında servis nesne örneği oluşturulmakta ve servis tekrar açılmaya çalışılmaktadır. Bu olay metodu içerisinde loglama işlemleride yapılarak hataların daha kolay izlenmesi sağlanabilir.

Bu makalemizde WCF uygulamalarında hata yönetimini farklı şekillerde ele almaya çalıştık. Özellikle hata yönetiminin basit bir istisna yönetiminden çok daha farklı olmamakla birlikte farklı platformların söz konusu olduğu dağıtık bir ortamda daha titiz bir biçimde ele alınması gerektiğini öğrendik. Bunların dışında istemci tarafında oluşacak hataların sunucuya gönderilmesi istenebilir. Ancak istemcilerinin Java gibi farklı platformlar olabileceği göz önüne alınırsa istemcinin sunucuya göndereceği mesajların Soap Fault'a uygun olması gerekecektir. Bu konuyu inceleyip ilerleyen makalelerimizde ele almaya çalışacağız. Böylece geldik bir makalemizin daha sonuna. İlerleyen makalelerimizde WCF mimarisinin detaylarını incelemeye devam ediyor olacağız. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.

[Örnek Uygulama İçin Tıklayınız.](/assets/files/2007/FaultControl.rar)