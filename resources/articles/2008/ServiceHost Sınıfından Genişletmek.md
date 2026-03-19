---
title: "ServiceHost Sınıfından Genişletmek"
pubDate: 2008-06-05 12:00:00
categories:
  - WCF
tags:
  - WCF
  - C#
  - XML
  - HTTP
  - IIS
  - Serialization
  - Visual Studio
---

# ServiceHost Sınıfından Genişletmek
Değerli Okurlarım Merhabalar,

WCF (Windows Communication Foundation) mimarisinde sunucu (Server) tarafındaki çalışma ortamınının (WCF Runtime) hazırlanması ile ilişkili olaraktan ServiceHost sınıfı kullanılmaktadır. Bir başka deyişle ServiceHost sınıfı uygulama sunucusu (Application Server) üzerindeki gerekli hazırlıkların yapılmasını sağlamaktadır. ServisHost sınıfı çoğunlukla, servis nesnesinin IIS (Internet Information Services) veya WAS (Windows Activation Services) üzerinden yayınlanmadığı durumlarda kullanılmaktadır.

Nitekim IIS ve WAS, ServiceHost tiplerinin çalışma zamanında kendileri ele almaktadır. Bununla birlikte geliştirici tarafından türetilen ServiceHost tiplerinin IIS veya WAS ortamlarında ele alınacak şekilde özelleştirmelerinin yapılmasıda mümkündür. System.ServiceModel isim alanı altında yer alan ServiceHost sınıfı ServiceHostBase isimli abstract sınıftan türemektedir. Doğal olarak sunduğu bazı kurallar değiştirilebilir bir başka deyişle ezilerek (override) yenide yorumlanabilir.

Özellikle çok sayıda servisin yayınlandığı vakalarda, uygulama sunucusu üzerindeki çalışma zamanı davranışlarının farklı ve daha kolay konfigure edilmesi istenebilir. İşte bu tip ihtiyaçlarda geliştirici (Developer) tarafından ServiceHost tipinden türetilen (Inherit) sınıflara ait nesne örneklerinin kullanılması tercih edilebilir. Örneğin, birden fazla EndPoint için varsayılan olarak kapalı olan Metadata yayınlamasının (Metada Publishing) kod tarafında açılması veya EndPoint gibi bilgilerin bir veri kaynağı üzerinden (örneğin bir veritabanı-database) okunmasını sağlamak için özel ServiceHost tiplerinin tasarlanması tercih edilebilir. İşte bu bölümde özel ServiceHost sınıflarının nasıl yazılabileceği incelenmektedir. Öncelikli olarak ServiceHost tipinin sınıf diyagram (Class Diagram) daki görüntüsüne bakmakta ve Framework içerisindeki yerini görmekte yarar vardır.

![mk253_1.gif](images/mk253_1.gif)

Genel olarak türetme doğrudan ServiceHost sınıfı üzerinden yapılmaktadır. Ezilebilen üyeler (overridable members) arasında en çok kullanılanı ApplyConfiguration metodudur. Bu metod ile Host için yüklenen konfigurasyon bilgilerine erişilmesi ve bazı davranışların değiştirilmesi mümkün olabilmektedir. Söz gelimi yazının başında belirtilen vakalardan ilkinde, EndPoint noktalarının her biri için otomatik olarak Metadata Publishing üretilmesinin istendiği bir durumda ApplyConfiguration metodu içerisinde gerekli eklemelerin yapılması sağlanabilir. Böylece servis uygulamasına kaç tane EndPoint eklenirse eklensin her biri için Metadata Publishing otomatik olarak eklenebilmektedir.

> Varsayılan olarak Metadata Publishing seçeneği kapalıdır. Bunun sebebi Metadata bilgisinin istemeden yayınlanmasının önüne geçmektedir. Metadata yayınlamasının açık olması halinde kullanılan EndPoint'ler içerisindeki bağlayıcı tiplerin (Binding Type) çeşitlerine görede farklı protokoller üzerinden servis bilgilerinin çekilmesi sağlanabilmektedir. Bir başka deyişle servisin ne yaptığı, hangi operasyonları sunduğu bilgileri Metadata şeklinde sunulabilir. Bu, istemciler için gerekli olan proxy nesnelerinin üretilmesinde ele alınmaktadır. Söz konusu metadata bilgilerinin elde edilmesi için svcutil aracı komut satırından kullanılabilir. Yada Visual Studio 2008 ortamında Add Service Reference seçeneğinden yararlanılabilir.
> Lakin öyle senaryolar vardırki metadata bilgisinin yayınlanması istenmez. Söz gelimi bir servisi kullanan başka bir servis olduğu göz önüne alınsın. Tüketici servis için proxy nesnesi önemlidir ve manuel olarak üretilip dağıtılabilir. Ancak servisin metadata bilgisinin bu iki servis dışındaki istemciler (Clients) tarafından elde edilmesi istenmemektedir. İşte bu gibi olasılıklar nedeni ile Metadata yayınlaması varsayılan olarak kapalıdır.

Yazıda ilk olarak Metadata yayınlamasının otomatik olarak açılmasını sağlayacak bir örnek üzerinde durulacaktır. Lakin bu örnekte ServiceHost sınıfından türeyen bir tip kullanılmaktadır. Ama öncesinde Metadata yayınlamasının birden fazla EndPoint üzerinden yapıldığı bir senaryoda konfigurasyon dosyasın içerisinde nasıl bir ayarlama yapılması gerektiğini incelemekte yarar vardır. Bu amaçla aşağıdaki servis sözleşmesinin (Service Contract) bulunduğu bir WCF servis kütüphanesi (WCF Service Library) geliştirildiği göz önüne alınsın.

![mk253_2.gif](images/mk253_2.gif)

Servis sözleşmesi ve uygulayıcı tip içeriği;

```csharp
using System;
using System.ServiceModel;

namespace ServisKutuphanesi
{
    [ServiceContract(Name="Cebir Servisi",Namespace="http://www.bsenyurt.com/CebirServisi")]
    public interface ICebir
    {
        [OperationContract]
        double DaireAlan(double r);

        [OperationContract]
        double DaireCevre(double r);
    }

    public class Cebir
        : ICebir
    {
        #region ICebir Members

        public double DaireAlan(double r)
        {
            return Math.PI*r * r;
        }

        public double DaireCevre(double r)
        {
            return 2 * Math.PI * r;
        }

        #endregion
    }
}
```

Senaryoda odaklanılması gereken nokta özel ServiceHost tipi geliştirilmesi olduğundan, servis sözleşmesi ve uygulayıcı tip mümkün olduğunca basit tutulmuştur. Service tarafındaki uygulamada en büyük sorun Cebir servisine ait farklı EndPoint noktaları için Metadata yayınlamasının yapılmak istenmesidir. Bu nedenle uygulama sunucusu görevini üstlenen programdaki konfigurasyon dosyasında, Mex EndPoint tanımlamalarının yapılması gerekir.

> Mex EndPoint tanımlamaları ile TCP, HTTP, HTTPS, NetPipe gibi protokoller üzerinden Metadata yayınlaması yapılabilmektedir. Burada en önemli nokta IMetadataExchange arayüzüdür (interface). Bilindiği gibi bir EndPoint içerisinde Address,Binding ve Contract tanımlamaları yapılmaktadır. IMetadataExchange metadata yayınlaması için gerekli sözleşmeyi (bildirmektedir). Bağlayıcı olarak mexHttpBinding, mexHttpsBinding, mexNamedPipeBinding, mexTcpBinding tipleri kullanılmaktadır. Address bilgisinde ise çoğunlukla baseAddress ile tanımlanan bilgiye ilave olaraktan Mex yazılması yeterli olmaktadır. Burada Mex bir takma ad görevini üstlenmektedir.

Host uygulama basit bir Console projesi olarak tasarlanmıştır ve konfigurasyon dosyası içeriği aşağıda görüldüğü gibidir.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <behaviors>
            <serviceBehaviors>
                <behavior name="MetadataExchangeBehavior">
                    <serviceMetadata/>
                </behavior>
            </serviceBehaviors>
        </behaviors>
        <services>
            <service behaviorConfiguration="MetadataExchangeBehavior" name="ServisKutuphanesi.Cebir">
                <endpoint address="" binding="netTcpBinding" name="CebirTcpEndPoint" contract="ServisKutuphanesi.ICebir" />
                <endpoint address="" binding="basicHttpBinding" name="CebirHttpEndPoint" contract="ServisKutuphanesi.ICebir" />
                <endpoint address="" binding="netNamedPipeBinding" name="CebirPipeEndPoint" contract="ServisKutuphanesi.ICebir" />
                <endpoint address="Mex" binding="mexTcpBinding" name="CebirMexTcpEndPoint" contract="IMetadataExchange" />
                <endpoint address="Mex" binding="mexHttpBinding" bindingConfiguration="" name="CebirMexHttpEndPoint" contract="IMetadataExchange" />
                <endpoint address="Mex" binding="mexNamedPipeBinding" bindingConfiguration="" name="CebirMexPipeEndPoint" contract="IMetadataExchange" />
                <host>
                    <baseAddresses>
                        <add baseAddress="net.tcp://localhost:3400/CebirServisi" />
                        <add baseAddress="http://localhost:3401/CebirServisi" />
                        <add baseAddress="net.pipe://localhost/CebirServisi" />
                    </baseAddresses>
                </host>
            </service>
        </services>
    </system.serviceModel>
</configuration>
```

Dikkat edileceği üzere senaryoya uygun olması açısından birden fazla farklı EndPoint kullanılmakta ve farklı protokollerin her biri için birer Mex EndPoint noktası ilave edilmektedir. Hem EndPoint noktaları hemde Mex EndPoint noktaları için TCP, HTTP ve Named Pipe formatında gereken temel adres (base address) tanımlamaları host elementi içerisinde yapılmaktadır. Buna göre istemciler servis çağrılarında temel adreslerde belirtilen lokasyonlara talepte bulunabilir. Diğer taraftan Mex adres değerlerine sahip EndPoint noktaları üzerinden proxy üretimide sağlanabilir. Servis uygulamasının konfigurasyon bilgilerine göre aslında aşağıdaki şekilde ifade edilen yayınlamalar söz konusudur.

![mk253_4.gif](images/mk253_4.gif)

Özetle servis üzerinde Metadata yayınlaması için sunulan her adres bilgisi üzerinden Proxy ve Config dosyası üretimleri gerçekleştirilebilmektedir. Servis uygulamasına ait program kodları ise aşağıdaki gibidir.

```csharp
using System;
using System.ServiceModel;
using ServisKutuphanesi;

namespace Sunucu
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(Cebir));
            host.Open();
            Console.WriteLine("Sunucu dinlemede");
            Console.ReadLine();
            host.Close();
        }
    }
}
```

Program kodlarında standart olarak ServiceHost örneği oluşturulmuş ve Cebir nesne örneği parametre olarak verilerek gerekli WCF çalışma ortamı tesis edilmiştir. Sonrasında ise servis nesnesi açılarak (Open metodu ile), istemciden gelen talepleri dinlemek üzere konuşlandırılmıştır. Bu durumda servis uygulaması çalışırken, svcutil aracı kullanılaraktan Mex EndPoint noktaları üzerinden proxy ve config dosyası üretimleri gerçekleştirilebilmektedir. Aşağıdaki ekran görüntüsünde bu durum irdelenmektedir.

![mk253_3.gif](images/mk253_3.gif)

Görüldüğü gibi baseAddress elementlerinde belirtilen adreslerin tamamı üzerinden proxy ve config dosyası üretimi yapılabilmektedir. Elbetteki üretilen config dosyalarının her biri NetTcpBinding, BasicHttpBinding ve NetNamedPipeBinding tabanlı EndPoint bildirimlerinin üçünü birden içerecektir.

Artık yazıya konu olan örneğe geçilebilir. Özel olarak yazılan ServiceHost türevli bir sınıf içerisinde, konfigurasyon dosyasından gelen tüm baseAddress bilgileri için otomatik olarak Mex EndPoint eklenmesi sağlanmaya çalışılacaktır. Bunun için aşağıdaki gibi bir sınıf tasarlanması yeterlidir.

![mk253_7.gif](images/mk253_7.gif)

Görüldüğü gibi tek yapılması gereken ServiceHost tipinden bir sınıf türetmek ve bu senaryo için ApplyConfiguration metodunu ezmektir. SmartServiceHost sınıfının kod içeriği aşağıdaki gibidir.

```csharp
using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Sunucu
{
    class SmartServiceHost
        :ServiceHost
    {
        // Yapıcı metoda gelen servis tipi ve adres bilgileri ServiceHost sınıfının eş düşen yapıcı metoduna(Constructor) yönlendirilir
        public SmartServiceHost(Type servisTipi,params Uri[] adresler)
            :base(servisTipi,adresler)
        {
        }

        protected override void ApplyConfiguration()
        {
            base.ApplyConfiguration(); // Konfigurasyon dosyasındaki bilgilerin yüklenmesi için bu metod çağrısı yapılmalıdır

            // Eğer servis için Metadata Publishing açık değilse bunun eklenmesi sağlanır
            ServiceMetadataBehavior servisDavranisi=this.Description.Behaviors.Find<ServiceMetadataBehavior>();
            if (servisDavranisi == null)
            {
                servisDavranisi=new ServiceMetadataBehavior();
                this.Description.Behaviors.Add(servisDavranisi); // MEX erişimi için yeni bir servis davranışı eklenir
            }

            // host elementi içerisindeki tüm baseAddress değerleri dolaşılır
            foreach (Uri temelAdres in this.BaseAddresses)
            {
                string schemaBilgisi = temelAdres.Scheme; // Şema bilgisi alınır
                if (schemaBilgisi == Uri.UriSchemeNetTcp) // Şema Tcp bazlı ise
                {
                    // Tcp için bir Mex EndPoint eklenir
                    // İlk parametre MEX için gerekli servis sözleşmesidir
                    // İkinci parametrede yer alan Create metodu ile gereki Mex EndPoint üretimi sağlanır 
                    this.AddServiceEndpoint("IMetadataExchange", MetadataExchangeBindings.CreateMexTcpBinding(), "Mex");
                }
                else if (schemaBilgisi == Uri.UriSchemeNetPipe) // Şema Pipe bazlı ise
                {
                    // Pipe için Mex EndPoint eklenir
                    this.AddServiceEndpoint("IMetadataExchange", MetadataExchangeBindings.CreateMexNamedPipeBinding(), "Mex");
                }
                else if (schemaBilgisi == Uri.UriSchemeHttp) // Şema Http bazlı ise
                {
                    servisDavranisi.HttpGetEnabled = true; // Http üzerinden yayınlamanın yapılacağı belirtilir
                    // Http için Mex EndPoint eklenir
                    this.AddServiceEndpoint("IMetadataExchange", MetadataExchangeBindings.CreateMexHttpBinding(), "Mex");
                }
                else if (schemaBilgisi == Uri.UriSchemeHttps) // Şema Https bazlı ise
                {
                    servisDavranisi.HttpsGetEnabled = true; // Https üzerinden yayınlamanın yapılacağı belirtilir
                    // Https için Mex EndPoint eklenir
                    this.AddServiceEndpoint("IMetadataExchange", MetadataExchangeBindings.CreateMexHttpsBinding(), "Mex");
                }
            }
        }
    }
}
```

ServiceHost sınıfına ait bir nesne örneklenirken servis nesnesinin tipi ve kullanılacak adres bilgileri parametre olarak verilmektedir. Bu nedenle SmartHostService sınıfının ilgili yapıcı metodundan (Constructor), base anahtar kelimesi ile ServiceHost sınıfında eş düşen yapıcı metoda parametre aktarımı gerçekleştirilmektedir. Diğer taraftan senaryoda istenen, var olan EndPoint noktaları için Mex tanımlamalarının yapılmasıdır. Bu nedenle konfigurasyon dosyasının içeriğine ulaşılması ve servis tanımlamalarında (Service Description) gerekli ilavelerin yapılması gerekmektedir. Söz konusu işlemler için ApplyConfiguration metodu ezilmiştir.

> ApplyConfiguration metodu dışında ezilebilecek olan diğer üyelerde aşağıdaki şekilde görüldüğü gibidir.
> ![mk253_5.gif](images/mk253_5.gif)
> Dikkat edileceği üzere servisi kapatma, açma veya hata oluşması anındaki olay metodların ezilmesi dahi mümkündür.

ApplyConfiguration metodu içerisinde ilk olarak base.ApplyConfiguration () fonksiyonuna çağrı yapılarak konfigurasyon dosyasındaki ayarların ortama yüklenmesi sağlanmaktadır. Sonrasında ise servis davranışında Metadata yayınlaması için gerekli elementin var olup olmadığına bakılır. Eğer yoksa eklenmesi sağlanır. İlerleyen adımlardaysa konfigurasyon dosyasındaki tüm temel adresler tek tek dolaşılır. Her temel adresin şema (Scheme) tipine bakılarak uygun bir Mex EndPoint yüklemesi yapılmaktadır.

Burada şema bilgisi elde edildikten sonra Uri sınıfının UriSchemeHttps, UriSchemeHttp, UriSchemeNetPipe, UriSchemeNetTcp sabit değerleri ile kıyaslama yapılmakta ve duruma göre uygun EndPoint üretimleri gerçekleştirilmektedir. Tüm Mex EndPoint noktaları IMetadataExchange arayüzünü, sözleşme (Contract) tipi olarak kullanmaktadır. Bununla birlikte uygun bağlayıcı tiplerin (Binding Type) üretimi için MetadataExchangeBindings sınıfının CreateMexHttpsBinding, CreateMexHttpBinding, CreateMexNamedPipeBinding ve CreateMexTcpBinding gibi metodları kullanılmaktadır. Artık servis uygulamasında ServiceHost yerine SmartServiceHost sınıfı aşağıdaki kod parçasında olduğu gibi kullanılabilir.

```csharp
using System;
using System.ServiceModel;
using ServisKutuphanesi;

namespace Sunucu
{
    class Program
    {
        static void Main(string[] args)
        {
            // ServiceHost host = new ServiceHost(typeof(Cebir));
            SmartServiceHost host = new SmartServiceHost(typeof(Cebir));
            host.Open();
            Console.WriteLine("Sunucu dinlemede");
            Console.ReadLine();
            host.Close();
        }
    }
}
```

Artık konfigurasyon dosyasında yapılan Mex EndPoint noktalarına ait bildirimler kaldırılabilir. Servis uygulaması çalıştırıldığında debug moddayken, SmartServiceHost sınıfı içerisinde ezilmiş olan ApplyConfiguration metodunun sonunda aşağıdaki ekran görüntüsüne ulaşılabilir.

![mk253_6.gif](images/mk253_6.gif)

Dikkat edileceği üzere altı adet EndPoint tanımlaması yer almaktadır. Bunlardan üçüde Mex EndPoint noktalarıdır. Dolayısıyla istemciler kendileri için gerekli Proxy ve config dosyası üretimlerini yine gerçekleştirebilirler.

Söz konusu örnekte SmartServiceHost nesnesi bir Console uygulaması üzerinde ele alınmaktadır. Oysaki bu tipin IIS veya WAS tabanlı bir host uygulama tarafındanda ele alınması istenebilir. Burada devreye bir fabrika nesnesi (Factory Object) girmektedir. Nitekim IIS üzerinden yayınlanan svc uzantılı dosyalara ait ServiceHost direktifi içerisinde bulunan Factory niteliği (attribute) ile, kullanılacak olan ServiceHost tipi belirtilebilmektedir. Söz konusu senaryoda özel bir ServiceHost tipi olduğundan bunun üretiminden sorumlu bir Factory sınıfınında geliştirilmesi gereklidir.

Yazının bu bölümünde geliştirilen ServiceHost türevli SmartServiceHost sınıfının IIS üzerinden host edilen bir WCF Service üzerinden nasıl kullanılacağı ele alınmaktadır. Bu amaçla öncellikle IIS üzerinde bir WCF servis açılması gerekmektedir. Söz konusu proje servis kütüphanesinide referans etmelidir. Sonrasında ise uygulamaya aşağıdaki sınıfın eklenmesi yeterlidir.

![mk253_8.gif](images/mk253_8.gif)

```csharp
using System;
using System.ServiceModel.Activation;

public class SmartServiceHostFactory
    :ServiceHostFactory
{
    protected override System.ServiceModel.ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
    {
        return new SmartServiceHost(serviceType, baseAddresses);
    }
    public SmartServiceHostFactory()
    {
    }
}
```

SmartServiceHostFactory sınıfı içerisinde CreateServiceHost isimli metod ezilmektedir (overriding). Dikkat edilecek olursa metod geriye SmartServiceHost tipinden bir nesne örneği döndürmektedir. Bir başka deyişle WCF çalışma ortamını tesis edecek nesne örneği üretilmektedir. CreateServiceHost metoduna gelen parametrelerden ilki, svc dosyasında yer alan Service niteliğinin değerini taşımaktadır. Elbette svc dosyası içerisinde hangi Factory nesnesinin kullanılacağının bildirilmeside gereklidir. Bu nedenle svc dosyasının içeriği aşağıdaki gibi düzenlenmelidir.

```text
<%@ ServiceHost Language="C#" Debug="true" Service="ServisKutuphanesi.Cebir" Factory="SmartServiceHostFactory" %>
```

Dikkat edilmesi gereken önemli bir nokta vardır. IIS üzerinden yayınlama yapıldığı ve bu senaryoda WAS kullanılmadığı için web.config dosyası içerisinde net.pipe ve net.tcp formatındaki adreslere izin verilmeyecektir. Dolayısıyla şu aşamada test olması açısından sadece HTTP bazlı bir EndPoint noktasına yer verilmektedir. Web.config dosyası için örnek içerik aşağıdaki gibidir.

```xml
<?xml version="1.0"?>
<configuration>
    <system.serviceModel>
        <behaviors>
            <serviceBehaviors>
            </serviceBehaviors>
        </behaviors>
        <services>
            <service behaviorConfiguration="" name="ServisKutuphanesi.Cebir">
                <endpoint address="" binding="basicHttpBinding" name="CebirHttpEndPoint" contract="ServisKutuphanesi.ICebir" />
                <host>
                    <baseAddresses>
                        <add baseAddress="http://localhost:3401/CebirServisi" />
                    </baseAddresses>
                </host>
            </service>
        </services>
    </system.serviceModel>
</configuration>
```

Bu işlemlerin ardından Service.svc dosyası herhangibir tarayıcı uygulamadan talep edilirse otomatik olarak HTTP Metadata Publishing'in açıldığı görülcektir. Tabiki SvcUtil aracı ilede metadata bilgileri çekilebilir.

Yazıya ikinci bir örnek ile devam edelim. Bu örnekte özel olarak yazılmış olan ServiceHost sınıfı, WCF çalışma ortamı için gerekli EndPoint bilgilerini bir veritabanı tablosundan almaktadır. Böyle bir senaryo çoğunlukla servis tarafındaki bildirimlerin konfigurasyon dosyası dışarısında daha güvenli bir yerden tedarik edilmesi istendiği durumlarda göz önüne alınabilir. Senaryonun çok karmaşık olmaması amacıyla basit bir tablo üzerinde EndPoint oluşumları için gerekli bir kaç alan bilgisi tutulmaktadır. İlk olarak bu tablonun tasarımı ile işe başlanabilir. Tablonun oluşturulması için gerekli sorgu cümlesi aşağıdaki şekilde tasarlanabilir.

![mk253_9.gif](images/mk253_9.gif)

```text
USE [ServiceBase]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE 
    [dbo].[EndPoints]
    (
        [EndPointId] [int] IDENTITY(1,1) NOT NULL,
        [Name] [varchar](20) COLLATE Turkish_CI_AS NOT NULL,
        [ServiceAlias] [nvarchar](20) COLLATE Turkish_CI_AS NOT NULL,
        [Protocol] [nvarchar](10) COLLATE Turkish_CI_AS NOT NULL CONSTRAINT [DF_EndPoints_Protocol] DEFAULT (N'net.tcp'),
        [ServerName] [nvarchar](50) COLLATE Turkish_CI_AS NOT NULL CONSTRAINT [DF_EndPoints_ServerName] DEFAULT (N'localhost'),
        [PortNumber] [int] NULL,
        [Binding] [nvarchar](20) COLLATE Turkish_CI_AS NOT NULL,
        [Contract] [nvarchar](50) COLLATE Turkish_CI_AS NOT NULL,
        CONSTRAINT [PK_EndPoints] PRIMARY KEY CLUSTERED 
        (
                [EndPointId] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
        ) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[EndPoints] WITH CHECK ADD CONSTRAINT [CK_Bindings] CHECK (([Binding]='WS2007FederationHttpBinding' OR [Binding]='WS2007HttpBinding' OR [Binding]='WSFederationHttpBinding' OR [Binding]='WebHttpBinding' OR [Binding]='WSDualHttpBinding' OR [Binding]='WSHttpBinding' OR [Binding]='BasicHttpBinding' OR [Binding]='MsmqIntegrationBinding' OR [Binding]='NetPeerTcpBinding' OR [Binding]='NetMsmqBinding' OR [Binding]='NetNamedPipeBinding' OR [Binding]='NetTcpBinding'))
GO
ALTER TABLE [dbo].[EndPoints] CHECK CONSTRAINT [CK_Bindings]
GO
ALTER TABLE [dbo].[EndPoints] WITH CHECK ADD CONSTRAINT [CK_PortNumber] CHECK (([PortNumber]>=(0) AND [PortNumber]<=(65535)))
GO
ALTER TABLE [dbo].[EndPoints] CHECK CONSTRAINT [CK_PortNumber]
GO
ALTER TABLE [dbo].[EndPoints] WITH CHECK ADD CONSTRAINT [CK_Protocol] CHECK (([Protocol]='net.p2p' OR [Protocol]='net.pipe' OR [Protocol]='https' OR [Protocol]='http' OR [Protocol]='net.msmq' OR [Protocol]='net.tcp'))
GO
ALTER TABLE [dbo].[EndPoints] CHECK CONSTRAINT [CK_Protocol]
```

Bu sorgu ifadesinde hataların önüne mümkün olduğunca geçebilmek için Port numarası, bağlayıcı tip adı ve protokol bilgilerinin girişi kısıtlamalar (Constraints) ile kontrol altına alınmaktadır. Bu şekilde Binding, PortNumber ve Protocol alanlarına girilecek olan değerler sınırlandırılmaktadır. Elbette bu tabloya EndPoint bilgilerinin girilmesi için bir uygulama arayüzü kullanılacaksa, giriş kontrollerinin veritabanı yerine program tarafında yapılmasıda söz konusudur. Nitekim bu sayede WCF alt yapısından tiplerin (Örneğin bağlayıcı sınıflar, enum sabitleri vb...) çalışma zamanında validasyon süreçleri içerisine dahil edilmesi garanti edilmiş olacak ve tutarlı veri girişi sağlanabilecektir. Tabloda örnek olarak aşağıdaki verilerin saklandığı göz önüne alınabilir.

![mk253_10.gif](images/mk253_10.gif)

Bu tabloda ye alan Protocol, ServerName ve PortNumber bilgileri yardımıyla EndPoint için gerekli adres (Address) oluşturulabilir. Adres bilgileri baseAddress olacak şekilde ele alınabilir. Yine EndPoint için gerekli sözleşme (Contract) bilgisi Contract isimli alandan tedarik edilebilir. Son olarak bağlayıcı (Binding) bilgisi için Binding alanındaki metinsel bilgiden yararlanılmaktadır. Tabiki çalışma zamanında ilgili bağlayıcı tipe ait nesne örneği gerektiğinden bu metinsel bilginin karşılığı olan bağlayıcı tip için ek bir kodlama yapılması gerekebilir. EndPoint bilgilerinin saklanacağı bu tabloyu kullanacak olan türetilmiş ServiceHost sınıfının tasarımı ise aşağıdaki şekilde yapılabilir.

```csharp
using System;
using System.ServiceModel;
using System.Data.SqlClient;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.MsmqIntegration;

namespace Sunucu
{
    class TableBasedServiceHost
        :ServiceHost
    {

        public TableBasedServiceHost(Type servisTipi, params Uri[] adresler)
            : base(servisTipi, adresler)
        {
        }

        protected override void ApplyConfiguration()
        {
            // Service için Metadata davranışı olup olmadığına bakılır. Yoksa bir tane oluşturulur.
            ServiceMetadataBehavior servisDavranisi = this.Description.Behaviors.Find<ServiceMetadataBehavior>();
            if (servisDavranisi == null)
            {
                servisDavranisi = new ServiceMetadataBehavior();
                this.Description.Behaviors.Add(servisDavranisi); // MEX erişimi için yeni bir servis davranışı eklenir
            }

            // ServiceBase veritabanındaki EndPoints tablosundan satırlar okunur.
            using (SqlConnection _conn = new SqlConnection("data source=.;database=ServiceBase;integrated security=SSPI"))
            {
                SqlCommand _cmd = new SqlCommand("Select EndPointId,Name,ServiceAlias,Protocol,ServerName,PortNumber,Binding,Contract From EndPoints", _conn);
                _conn.Open();
                SqlDataReader reader = _cmd.ExecuteReader();
                string adres = null;
                while (reader.Read())
                {
                    // Servis için gerekli baseAddress bilgileri oluşturulurken PortNumber alanının değerinin null olup olmadığına bakılır
                    // Bu alan null ise sunucu adından sonra port numarası bilgisi verilmez.
                    if (!String.IsNullOrEmpty(reader["PortNumber"].ToString()))
                        adres = String.Format("{0}://{1}:{2}/{3}", reader["Protocol"].ToString(), reader["ServerName"].ToString(), reader["PortNumber"].ToString(), reader["ServiceAlias"].ToString());
                    else
                        adres = String.Format("{0}://{1}/{2}", reader["Protocol"].ToString(), reader["ServerName"].ToString(), reader["ServiceAlias"].ToString());
                    // Elde edilen adres verisinden bir Uri nesne örneği oluşturulur
                    Uri baseAddress = new Uri(adres);
                    // Oluşturulan baseAddress bilgisi servise eklenir.
                    this.AddBaseAddress(baseAddress);
                    // EndPoint oluşturulur ve eklenir. 
                    // EndPoint noktasının oluşturulması sırasında bağlayıcı tip(Binding Type) için BaglayiciUret isimli yardımcı metod kullanılır.
                    this.AddServiceEndpoint(reader["Contract"].ToString(), BaglayiciUret(reader["Binding"].ToString()), "");
        
                    #region MEX EndPoint Ekleme İşlemleri
                    
                    // Adresteki scheme bilgisine göre gerekli Mex EndPoint noktaları oluşturulur.
                    string schemaBilgisi = baseAddress.Scheme;
                    if (schemaBilgisi == Uri.UriSchemeNetTcp) // TCP için
                    { 
                        this.AddServiceEndpoint("IMetadataExchange",  MetadataExchangeBindings.CreateMexTcpBinding(), "Mex");
                    }
                    else if (schemaBilgisi == Uri.UriSchemeNetPipe) // PIPE için
                    {
                        this.AddServiceEndpoint("IMetadataExchange",  MetadataExchangeBindings.CreateMexNamedPipeBinding(), "Mex");
                    }
                    else if (schemaBilgisi == Uri.UriSchemeHttp) // HTTP için
                    {
                        servisDavranisi.HttpGetEnabled = true;
                        this.AddServiceEndpoint("IMetadataExchange",  MetadataExchangeBindings.CreateMexHttpBinding(), "Mex");
                    }
                    else if (schemaBilgisi == Uri.UriSchemeHttps) // HTTPS için
                    {
                        servisDavranisi.HttpsGetEnabled = true;
                        this.AddServiceEndpoint("IMetadataExchange", MetadataExchangeBindings.CreateMexHttpsBinding(), "Mex");
                    }
                    
                    #endregion
                }
                reader.Close();
            }
        }

        // EndPoints tablosunda bağlayıcı adları string olarak tutulduğundan Binding tipinden nesne üretimi gerçekleştiren bir metoddan yararlanılmaktadır.
        private Binding BaglayiciUret(string baglayiciAdi)
        {
            Binding baglayici = null;
            switch (baglayiciAdi)
            {
                case "NetTcpBinding":
                    baglayici= new NetTcpBinding();
                    break;
                case "NetNamedPipeBinding":
                    baglayici = new NetNamedPipeBinding();
                    break;
                case "WS2007FederationHttpBinding":
                    baglayici = new WS2007FederationHttpBinding();
                    break;
                case "WS2007HttpBinding":
                    baglayici = new WS2007HttpBinding();
                    break;
                case "WebHttpBinding":
                    baglayici = new WebHttpBinding(); // System.ServiceModel.Web referansının ekli olması gerekir
                    break;
                case "WSDualHttpBinding":
                    baglayici = new WSDualHttpBinding();
                    break;
                case "WSHttpBinding":
                    baglayici = new WSHttpBinding();
                    break;
                case "BasicHttpBinding":
                    baglayici = new BasicHttpBinding();
                    break;
                case "MsmqIntegrationBinding":
                    baglayici = new MsmqIntegrationBinding();
                    break;
                case "NetPeerTcpBinding":
                    baglayici = new NetPeerTcpBinding();
                    break;
                case "NetMsmqBinding":
                    baglayici = new NetMsmqBinding();
                    break;
                default:
                    baglayici = new NetTcpBinding();
                    break;
            }
            return baglayici;
        }
    }
}
```

TableBasedServiceHost nesnesi içerisinde yer alan ApplyConfiguration metodu içerisinde, EndPoints tablosundaki satırlardan yararlanılarak base address, EndPoint ve Mex EndPoint nesnelerinin oluşturulması sağlanmaktadır. Elbette bu yeni ServiceHost türevli sınıfa ait nesne örneğinin kullanılması için bir uygulama sunucusuna ihtiyaç vardır. Bu amaçla Console uygulamasının kodlarını aşağıdaki gibi değiştirmek yeterlidir.

```csharp
using System;
using System.ServiceModel;
using ServisKutuphanesi;

namespace Sunucu
{
    class Program
    {
        static void Main(string[] args)
        {
            TableBasedServiceHost host = new TableBasedServiceHost(typeof(Cebir));
            host.Open();
            Console.WriteLine("Sunucu dinlemede");
            Console.ReadLine();
            host.Close();
        }
    }
}
```

Programın başarılı bir şekilde çalıştığı görülür. Debug mod içerisindekyen TableBasedServiceHost sınıfınına ait ApplyConfiguration metodu işleyişini tamamlandığında servise ait içerik aşağıdaki Quick Watch görüntüsünde olduğu gibidir. Dikkat edilecek olursa tabloda örnek olarak girilen tüm base adres değerleri BaseAddress özelliğinin işaret ettiği koleksiyona yüklenmiştir. Bununla birlikte altı adet EndPoint tanımlaması EndPoints özelliği ile işaret edilen koleksiyona eklenmiştir. Bunlardan üçü bir önceki örnektine benzer olacak şekilde Mex EndPoint bilgileridir. Servis tipi zaten yapıcı metod ile bildirilmektedir. Son olarak sözleşme bilgisininde ImplementedContracts özelliğine set edildiği görülmektedir.

![mk253_11.gif](images/mk253_11.gif)

Tabiki buradaki senaryo bir Best Practices olarak algılanmamalıdır. Tablo tasarımı daha farklı bir şekilde düzenlenebilir. Bununla birlikte bağlantı, tablo adı ve alanlarının değişme ihtimali göz önünde bulundurularak bunların ServiceHost türevli sınıf içerisine aktarımı için gerekli önlemlerin alınması gereklidir. Elbette daha ileri bir senaryoda düşünülebilir. Söz gelimi, servisin kullanacağı EndPoint tanımlamaları tablodaki alanlarda serileştirilmiş (Serialize) şekilde tutulabilir. Hatta birden fazla servis nesnesi (Service Instance) için birden fazla EndPoint tanımlamasının yapılabileceği ilişkisel (Relational) bir veritabanı tasarımıda söz konusu olabilir.

Söz konusu servislerin kullandığı EndPoint bilgilerinin tablolardaki ilgili alanlara eklenmesi içinde yetkilendirilmiş (Authorized) bir ekran (örneğin bir Windows uygulaması) tasarlanabilir. Böylece EndPoint girişleri yetki dahilinde yapılır. Diğer taraftan EndPoint bilgileri XML gibi açık bir dosyada durmadıklarından görece daha güvenli bir ortamda saklanırlar. Üstelik bu veritabanının servis uygulamasının host edildiği sunucunun arkasında bir sunucuda yer aldığıda göz önüne alınabilir. Bu vizyon çok daha fazla genişletilebilir.

Böylece geldik bir makalemizin daha sonuna. Bu makalemizde ServiceHost türevli bir sınıfın nasıl geliştirilip kullanılabileceği iki örnek senaryo üzerinden incelemeye çalıştık. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](assets/OzelServiceHost.rar)
