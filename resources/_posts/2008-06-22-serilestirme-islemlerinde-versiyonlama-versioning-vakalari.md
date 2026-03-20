---
layout: post
title: "Serileştirme İşlemlerinde Versiyonlama(Versioning) Vakaları"
date: 2008-06-22 12:00:00 +0300
categories:
  - wcf
tags:
  - wcf
  - csharp
  - xml
  - dotnet
  - http
  - serialization
  - visual-studio
---
Bir önceki makalemizde WCF (Windows Communication Foundation) mimarisinin kullandığı serileştirici tiplerden bahsetmiş ve son olarak versiyonlama (Versioning) vakalarına değinmiştik. Bu makalemizde ise versiyonlama vakalarının örnek uygulama üzerinden test ederek analiz etmeye çalışacağız. Versiyonlama vakalarının merkezinde veri sözleşmesi (Data Contract) farklılıkları yer almaktadır. Daha öncedende değinildiği üzere üç farklı versiyonlama vakası bulunmaktadır. New Members, Missing Members, Round-Trip.

Bu bölümde ilk olarak yeni üyelerin (New Members) oluştuğu vaka irdelenmeye çalışılacaktır. Söz konusu senaryoda, istemci veya servis tarafının sahip olduğu veri sözleşmesinin yeni bir versiyonunu, karşı taraf ile paylaştığı bir ortam söz konusudur. WCF çalışma zamanı varsayılan olarak böyle bir durum ile karşılaştığında, fazladan gelen üyenin kendisini ve içeriğini görmezden gelmektedir. Ancak yinede fazla üyeleri içeren nesne verisi, karşı tarafa iletilmektedir. Öncelikli olarak tüm versiyonlama örneklerinde kullanılacak olan ve aşağıdaki sınıf şemasında (Class Diagram) görülen tipleri içeren bir WCF servis kütüphanesi (WCF Service Libary) geliştirildiğini düşünelim.

![mk255_1.gif](/assets/images/2008/mk255_1.gif)

Servis kütüphanesinde yer alan Urun isimli sınıf bir veri sözleşmesi (Data Contract) olacak şekilde aşağıdaki gibi tanımlanmıştır. Bu nedenle DataContract ve DataMember nitelikleri kullanılmaktadır.

```csharp
using System;
using System.Runtime.Serialization;

namespace AdventureLib
{
    [DataContract]
    public class Urun
    {
        [DataMember]
        public int Id { get; set; }
    
        [DataMember]
        public string Ad { get; set; }
    
        [DataMember]
        public double Fiyat { get; set; }
    }
}
```

Servis sözleşmesi (Service Contract) ve uygulayıcı tipe ait kod içerikleri ise aşağıdaki gibidir.

IUrunYonetim;

```csharp
using System;
using System.ServiceModel;

namespace AdventureLib
{
    [ServiceContract(Name="Adventure Product Service", Namespace="http://www.bsenyurt.com/AdventureProductService")]
    public interface IUrunYonetim
    {
        [OperationContract]
        void UrunEkle(Urun urn);
    }
}
```

UrunYonetim;

```csharp
using System;

namespace AdventureLib
{
    public class UrunYonetim
                :IUrunYonetim
    {
        #region IUrunYonetim Members

        public void UrunEkle(Urun urn)
        {
            string bilgi = String.Format("{0} {1} {2}", urn.Id.ToString(), urn.Ad, urn.Fiyat.ToString("C2"));
            Console.WriteLine(bilgi);
        }

        #endregion
    }
}
```

UrunYonetim sınıfı içerisinde yer alan UrunEkle metodu, parametre olarak Urun tipinden bir nesne örneği almaktadır. Senaryoda ilgili operasyonun herhangibir değer dönürüp döndürmemesinin bir önemi yoktur. Servis tarafındaki uygulama ise basit bir Console projesi olarak tasarlanabilir. Bu uygulamanın konfigurasyon içeriği ve Main metoduna ait kod bloğu aşağıdaki gibidir.

Servis tarafı konfigurasyon içeriği;

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
                    <add name="ServiceModelMessageLoggingListener">
                        <filter type="" />
                    </add>
                </listeners>
            </source>
        </sources>
        <sharedListeners>
            <add initializeData="c:\vs2005projects\wcf samples\wcfserializationandencoding\sunucu\app_messages.svclog" type="System.Diagnostics.XmlWriterTraceListener, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" name="ServiceModelMessageLoggingListener" traceOutputOptions="None">
                <filter type="" />
            </add>
        </sharedListeners>
    </system.diagnostics>
    <system.serviceModel>
        <diagnostics>
            <messageLogging logEntireMessage="true" logMalformedMessages="true" logMessagesAtServiceLevel="true" logMessagesAtTransportLevel="true" />
        </diagnostics>
        <behaviors>
            <serviceBehaviors>
                <behavior name="ProductServiceBehavior">
                    <serviceMetadata />
                         <serviceDebug includeExceptionDetailInFaults="true" />
                </behavior>
            </serviceBehaviors>
        </behaviors>
        <services>
            <service behaviorConfiguration="ProductServiceBehavior" name="AdventureLib.UrunYonetim">
                <endpoint address="" binding="netTcpBinding" bindingConfiguration="" name="ProductServiceTcpEndPoint" contract="AdventureLib.IUrunYonetim" />
                <endpoint address="Mex" binding="mexTcpBinding" bindingConfiguration="" name="ProductServiceMexTcpEndPoint" contract="IMetadataExchange" />
                <host>
                    <baseAddresses>
                        <add baseAddress="net.tcp://localhost:4500/ProductService" />
                    </baseAddresses>
                </host>
            </service>
        </services>
    </system.serviceModel>
</configuration>
```

Servis tarafında, istemciden gelen mesaj içerikleri izlenmek istendiğinden Diagnostics özelliği açılmış ve Mesaj seviyesinde izleme yapılması için gerekli konfigurasyon ayarları tesis edilmiştir. Böylece istemciden servis tarafına gelen veri sözleşemesi içeriklerine detaylı bir şekilde bakılabilir. Bunların dışında, TCP bazlı MetadaEXchange yayınlaması için ek bir EndPoint noktasıda yer almaktadır.

Servis uygulama Main metodu kodları;

```csharp
using System;
using System.ServiceModel;
using AdventureLib;

namespace Sunucu
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(UrunYonetim));
            host.Open();
            Console.WriteLine("Servis Açık. Kapatman için bir tuşa basınız");
            Console.ReadLine();
            host.Close();
        }
    }
}
```

İstemci tarafında proxy üretimi amacıyla svcutil aracından faydalanılmaktadır. Nitekim servis tarafında TCP üzerinden MEX (MetadataExchange) yayınlaması yapıldığından bu mümkündür.(Üretim işlemi sırasında servis uygulamasının çalışıyor olması gerektiğini hatırlamakta yarar vardır. Aksi takdirde ilgili URL üzerinden Metadata bilgisi çekilemez.) Svcutil aracının kullanımı sonrası üretilen istemci taraflı Proxy dosyasında, Urun sınıfı aşağıdaki şekilde yer almaktadır.

![mk255_2.gif](/assets/images/2008/mk255_2.gif)

```csharp
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
[System.Runtime.Serialization.DataContractAttribute(Name="Urun",   Namespace="http://schemas.datacontract.org/2004/07/AdventureLib")]
public partial class Urun 
    : object, System.Runtime.Serialization.IExtensibleDataObject
{
    private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
    private string AdField;
    private double FiyatField;
    private int IdField; 

    public System.Runtime.Serialization.ExtensionDataObject ExtensionData
    {
        get{return this.extensionDataField;}
        set{this.extensionDataField = value;}
    }

    [System.Runtime.Serialization.DataMemberAttribute()]
    public string Ad
    {
        get{return this.AdField;}
        set{this.AdField = value;}
    }

    [System.Runtime.Serialization.DataMemberAttribute()]
    public double Fiyat
    {
        get{return this.FiyatField;}
        set{this.FiyatField = value;}
    }

    [System.Runtime.Serialization.DataMemberAttribute()]
    public int Id
    {
        get{return this.IdField;}
        set{this.IdField = value;}
    }
}
```

Dikkat edileceği üzere servis tarafında tanımlı veri sözleşmesi içeriğinde yer alan ve DataMember niteliği ile işaretlenmiş olan tüm özellikler burada da yer almaktadır. Bunların yanında Urun sınıfının istemci versiyonunun IExtensibleDataObject isimli arayüzü (Interface) uyguladığı ve ExtensionData isimli bir özelliğe (Property) sahip olduğuda gözden kaçırılmamalıdır. Bu arayüz Round-Trip vakalarında önem kazanmaktadır ve ekstra bilgilerin taşınması amacıyla kullanılmaktadır. (İlerleyen örneklerde bu durum servis tarafı için araştırılmaktadır.) Şimdilik istemci tarafındaki Urun sınıfının aşağıdaki gibi StokMiktari isimli yeni bir özelliğe sahip olduğu varsayılsın. Böylece istemcinin ilgili veri sözleşmesinin yeni bir sürümüne sahip olduğu vakası canlandırılabilir. Bu amaçla manuel olarak proxy dosyasına müdahelede bulunulabilir.

![mk255_3.gif](/assets/images/2008/mk255_3.gif)

```csharp
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
[System.Runtime.Serialization.DataContractAttribute(Name="Urun", Namespace="http://schemas.datacontract.org/2004/07/AdventureLib")]
public partial class Urun 
     : object, System.Runtime.Serialization.IExtensibleDataObject
{
    private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
    private string AdField;
    private double FiyatField;
    private int IdField;
    private int StokMiktariField;

    public System.Runtime.Serialization.ExtensionDataObject ExtensionData
    {
        get{return this.extensionDataField;}
        set{this.extensionDataField = value;}
    }

    [System.Runtime.Serialization.DataMemberAttribute()]
    public string Ad
    {        
        get{return this.AdField;}
        set{this.AdField = value;}
    }

    [System.Runtime.Serialization.DataMemberAttribute()]
    public double Fiyat
    {
        get{return this.FiyatField;}
        set{this.FiyatField = value;}
    }

    [System.Runtime.Serialization.DataMemberAttribute()]
    public int Id
    {
        get{return this.IdField;}
        set{this.IdField = value;}
    }

    [System.Runtime.Serialization.DataMemberAttribute()]
    public int StokMiktari
    {
        get{return this.StokMiktariField;}
        set{this.StokMiktariField = value;}
    }
}
```

Şimdide istemci tarafındaki Console uygulmasında aşağıdaki kodların yazıldığı göz önüne alınsın.

```csharp
using System;
using AdventureLib;

namespace Istemci
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Başlamak için bir tuşa basın");
            Console.ReadLine();
            AdventureProductServiceClient client = new AdventureProductServiceClient("ProductServiceTcpEndPoint");
            Urun mouse = new Urun()
                                {
                                    Id=10,
                                    Ad="Microsoft Optical Mouse",
                                    Fiyat=8.45,
                                    StokMiktari=190
                                };
            client.UrunEkle(mouse); 
        }
    }
}
```

Buna göre istemci uygulama Urun sınıfının StokMiktari özelliğinide kullanaraktan servis tarafına bir operasyon çağrısında bulunmaktadır. Bir başka deyişle istemci tarafından Urun sınıfının yeni versiyonuna ait bir nesne içeriği serileştirilerek servis tarafına gönderilmektedir. Eğer örnek, WCF servis kütüphanesinde yer alan UrunEkle metoduna çalışma zamanında breakpoint konularak incelenirse aşağıdaki ekran görüntüsü ile karşılaşılır.

![mk255_4.gif](/assets/images/2008/mk255_4.gif)

Dikkat edileceği üzere servis tarafındaki Urun tipinde StokMiktari isimli bir özellik bulunmadığından WCF çalışma zamanı, istemciden gelen paketteki Urun verisini ters serileştirdikten (DeSerialization) sonra görmezden gelmiş ve StokMiktari özelliğini atlamıştır. Peki gerçektende istemci uygulama, StokMiktari özelliğini içeren bir veri paketini servis tarafına göndermiş midir? İşte servis tarafında yapılan Diagnostics ayarlarının faydası bu noktada kendini göstermektedir. appmessages isimli svclog dosyası açılırsa, aşağıdaki ekran görüntüsünde yer alan içerik ile karşılaşılır.

![mk255_5.gif](/assets/images/2008/mk255_5.gif)

Dikkat edileceği üzere XML içeriğinde StokMiktari isimli bir elementin ve 190 değerinin servis tarafına gönderildiği açık bir şekilde görülebilir. Ancak, WCF çalışma zamanı tarafından bu element içeriği görmezden gelinmektedir. Bu WCF çalışma zamanının, New Members vakasındaki tipik davranışıdır.

İkinci vakaya (Missing Members) gelindiğindeyse; bu kez taraflardan birisinde (özellikle istemci açısından bakıldığında) ilgili veri sözleşmesinin eski versiyonunun karşı tarafa gönderilmesi durumu söz konusudur. Bu durumu analiz etmek için istemci tarafında yer alan Urun sınıfından Fiyat ve son eklenen StokMiktari özelliklerinin kaldırıldığı düşünülebilir. Bu durumda istemci tarafında Id ve Ad özellikleri olan, servis tarafında ise Id,Ad ve Fiyat özellikleri olan birer veri sözleşme tipi söz konusudur.

![mk255_6.gif](/assets/images/2008/mk255_6.gif)

Şimdi aynı uygulama tekrardan test edilirse çalışma zamanındaki Debug görüntüsünde, servis tarafına ulaşmayan üye özellik (Fiyat özelliği) için varsayılan bir değerin otomatik olarak atandığı görülür.

![mk255_7.gif](/assets/images/2008/mk255_7.gif)

> Missing Members vakasına göre, eksik üyelerin tiplerine göre varsayılan değer atamaları otomatik olarak yapılmaktadır. Buna göre referans tipli değişkenler için null (örneğin String tipi), ilkel değer türleri içinse 0 veya 0.0 değerleri, bool alanı için false değeri atanır. Diğer taraftan istenilirse, veri sözleşmesi içerisinde aşağıdaki kod parçası uygulanaraktan, varsayılan değerin farklı bir şekilde set edilmeside sağlanabilir. Tahmin edileceği üzere bu metod ters serileştirme (DeSerializing) sırasında devreye girmektedir.
> [OnDeserializing]
> void OnDeserializing (StreamingContext context)
> {
> Fiyat=1;
> }

İstemcinin gönderdiği mesaj içeriğine svclog dosyası üzerinden bakıldığında ise, sadece Id ve Ad özelliklerinin değerlerinin gönderildiği açık bir şekilde görülmektedir.

![mk255_8.gif](/assets/images/2008/mk255_8.gif)

Görüldüğü gibi WCF çalışma zamanı servis tarafında yine olayı sessiz bir şekilde örtpas etmiştir. Bu bir anlamda iyi olabilir. Ancak servis tarafındaki veri sözleşmesinde (Data Contract) yer alan bazı üyeler için IsRequired değeri true olarak belirlenirse, durum biraz daha farklılaşır. Söz gelimi son örneğe göre, Fiyat özelliğinin servis tarafının kullandığı WCF Service Library içerisinde aşağıdaki gibi değiştirildiği varsayılsın.

```csharp
[DataContract]
public class Urun
{
    [DataMember]
    public int Id { get; set; }
    
    [DataMember]
    public string Ad { get; set; }
    
    [DataMember(IsRequired=true)]
    public double Fiyat { get; set; }
}
```

Bu duruma göre geliştirilen uygulama test edildiğinde istemci tarafındaki UrunEkle metodunun icrası sırasında istemci tarafına aşağıdaki istisnanın (Exception) fırlatıldığı görülür.(Detaylı Exception bilgisi için servis tarafında ServiceDebug davranışı eklenmiş ve IncludeExceptionDetailOnFaults özelliğinin değeri true olarak belirlenmiştir.)

![mk255_9.gif](/assets/images/2008/mk255_9.gif)

Dikkat edileceği üzere Fiyat isimli bir element değerinin beklendiği ifade edilmektedir. Dolayısıyla IsRequired özelliğine true verilmesi halinde Missing Members vakasında ortama bir istisna (Exception) fırlaması söz konusudur.

> Bilindiği gibi Serializable niteliğine (Attribute) sahip tiplerde serileştirilebildikleri için WCF uygulamalarında taraflar arasında gönderilebilmektedirler. Lakin, Serializable niteliği uygulanmış tipler içerisindeki tüm özellikler aslında IsRequired=true davranışını sergilerler. Ancak, özellikle.Net Remoting ile yazılmış uygulamlardan kalan Serializable tiplerin kullanıldığı WCF senaryolarında, OptionalField niteliği (Attribute) kullanılarak ilgili üyeler için IsRequired=false davranışı tanımlanabilir.
> Tabi bazı hallerde servisin veya istemcinin kullandığı veri sözleşmesi ayrı bir library içerisinde olabilir ve müdahele edilemeyebilir. Bu durumda ilgili kütüphanedeki serializable tiplerin tamamına ait özellikler/alanlar, WCF çalışma zamanı IsRequired=true şeklinde yorumlanacaktır. Bu durumda Missing Members vakasına göre, olası istisnalara karşı gerekli tedbirlerin alınması gerekebilir.

Round-Trip vakasında ise, istemcinin servis tarafına veri sözleşmesinin yeni bir versiyonunu gönderdiği ancak servis tarafındaki operasyon sonrasında da istemciye veri sözleşmesinin eski halinin döndürüldüğü düşünülmektedir. Olayı daha iyi anlamak IUrunYonetim servis sözleşmesine aşağıdaki fonksiyonelliğin eklendiği göz önüne alınsın.

```csharp
using System;
using System.ServiceModel;

namespace AdventureLib
{
    [ServiceContract(Name="Adventure Product Service",Namespace="http://www.bsenyurt.com/AdventureProductService")]
    public interface IUrunYonetim
    {
        [OperationContract]
        void UrunEkle(Urun urn);

        [OperationContract]
        Urun UrunGuncelle(Urun urn);
    }
}
```

Operasyon dikkat edileceği üzere, Urun tipinden bir parametre almakta ve yine Urun tipinden bir nesne örneğini geri döndürmektedir. Senaryo gereği operasyon sırasında istemciden gelen Urun nesnesinde değişiklik yapılmakta ve güncel hali istemci tarafına gönderilmektedir. Lakin istemci tarafından servis tarafına gelen Urun nesne örneğinde, servis tarafının kullandığı Urun tasarımında olmayan StokMiktari isimli bir özellik bulunduğu varsayılmaktadır. Buna göre doğal olarak servis tarafı StokMiktari isimli özelliği sessizce göz ardı etmektedir. Sonrasında ise Urun değişkeninin taşıdığı nesne üzerinde aşağıdaki örnek güncellemeyi yapıp istemci tarafına göndermektedir.

```csharp
using System;

namespace AdventureLib
{
    public class UrunYonetim
        :IUrunYonetim
    {
        #region IUrunYonetim Members

        public void UrunEkle(Urun urn)
        {
            string bilgi = String.Format("{0} {1} {2}", urn.Id.ToString(), urn.Ad, urn.Fiyat.ToString("C2"));
            Console.WriteLine(bilgi);
        }

        public Urun UrunGuncelle(Urun urn)
        {
            urn.Fiyat += 10;
            return urn;
        }

        #endregion
    }
}
```

Bu durumda istemci tarafında set edilen stok miktarı değeri doğal olarak sıfırlanmış olur. Durumu canlandırmak için istemci tarafındaki kodların aşağıdaki şekilde değiştirildiği göz önüne alınsın.(Elbette servis sözleşmesinde bir değişiklik yapıldığı için istemci tarafındaki proxy dosyasının svcutil aracı veya Visual Studio ortamından Add Service Reference seçeneği ile yeniden oluşturulması gerekebilir. Bununla birlikte senaryonun gerçeklenmesi için istemci tarafında sıfırlanan Urun sınıfı içerisine StokMiktari isimli özelliğin yeniden eklenmesi de gerekmektedir.)

```csharp
using System;
using AdventureLib;

namespace Istemci
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Başlamak için bir tuşa basın");
            Console.ReadLine();
            AdventureProductServiceClient client = new AdventureProductServiceClient("ProductServiceTcpEndPoint");
            Urun mouse = new Urun()
                                {
                                    Id=10,
                                    Ad="Microsoft Optical Mouse",
                                    Fiyat=8.45,
                                    StokMiktari=190
                                };
            Urun donenUrun = client.UrunGuncelle(mouse);
            string urunBilgisi = String.Format("{0} {1} {2} {3}", donenUrun.Id.ToString(), donenUrun.Ad, donenUrun.Fiyat.ToString("C2"), donenUrun.StokMiktari.ToString());
            Console.WriteLine(urunBilgisi);
        }
    }
}
```

Servis ve istemci tarafı çalıştırıldığında aşağıdaki sonuçlar gözlemlenir.

![mk255_10.gif](/assets/images/2008/mk255_10.gif)

Dikkat edileceği üzere istemci tarafında 190 olarak set edilen StokMiktari değeri, UrunGuncelle operasyon çağrısından sonra 0 olarak bırakılmıştır. Oysaki servis tarafına ulaşan mesajlara svclog dosyasından bakıldığında aşağıdaki ekran görüntüsünde olduğu gibi 190 değerinin aktarıldığı görülebilir.

![mk255_11.gif](/assets/images/2008/mk255_11.gif)

Peki çözümsel bir yaklaşım var mıdır ve nedir? Çözüm, IExtensibleDataObject arayüzünün (Interface) servis tarafında kullanılan veri sözleşmesine uygulanmasıdır. Bu arayüz sayesinde, istemciden servis tarafına gönderilen ancak servis tarafında var olmayan özellik değerlerinin taşınması ve elde edilmesi mümkün olabilmektedir. Bu amaçla servis tarafının kullandığı Urun sınıfına aşağıdaki gibi IExtensibleDataObject arayüzünün uygulanması yeterlidir.

```csharp
using System;
using System.Runtime.Serialization;

namespace AdventureLib
{
    [DataContract]
    public class Urun
        :IExtensibleDataObject
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Ad { get; set; }
        [DataMember(IsRequired=true)]
        public double Fiyat { get; set; }

        private ExtensionDataObject extensionData;

        #region IExtensibleDataObject Members
    
        public ExtensionDataObject ExtensionData
        {
            get{return extensionData;}
            set{extensionData = value;}
        }

        #endregion
    }
}
```

IExtensibleDataObject arayüzü sadece ExtensionData isimli bir özellik içermektedir. Bu özellik ExtensionDataObject veri türündendir. Servis tarafındaki ilgili operasyon çağrısı debug modda incelendiğinde aşağıdaki verilere ulaşıldığı görülür.

![mk255_12.gif](/assets/images/2008/mk255_12.gif)

Dikkat edileceği üzere, extensionData alanının içeriğinde, istemci tarafından gönderilen StokMiktari özelliğine ait bilgiler ve set edilen 190 değeri yer almaktadır. Buna göre UrunGuncelle metodunun istemciye döndürdüğü Urun nesnesinin içeriğinde StokMiktari özelliği 190 değeri ile korunmaktadır. Çalışma zamanında uygulamaların ekran çıktısı aşağıdaki gibidir.

![mk255_13.gif](/assets/images/2008/mk255_13.gif)

Round-Trip vakasında örnektende görüldüğü üzere IExtensibleDataObject arayüzü ile bir çözüm üretilebilmektedir ki bu Microsoft tarafından Best-Practice olarakta belirtilmektedir.

Versiyonlama farklılıkları dışında serileştirme ile ilişkili olaraktan dikkat edilmesi gereken farklı konularda vardır. Söz gelimi servis tarafından yayınlanan bir veri sözleşmesi içerisinde serileştirilemeyen (NonSerializable) ve geliştirici tarafından doğrudan müdahalede bulunulamayan sonradan tanımlı tipler var olabilir. Bu durumda vekil veri sözleşmeleri (Surrogate DataContract) kullanılarak veri sözleşmesinin serileştirilmesi yoluna gidilebilir. Bu konu bir sonraki makalede çözümleyiciler (Encoding) ile birlikte ele alınmaya çalışılacaktır. Böylece geldik bir makalemizin daha sonuna. Bu makalemizde kısaca serileştirme işlemlerinde ortaya çıkabilecek olan versiyonalama vakaları analiz edilmeye çalışmıştır. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](/assets/files/2008/WCFSerializationAndEncoding.rar)