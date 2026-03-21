---
layout: post
title: "Mesaj Sözleşmeleri(Message Contracts)"
date: 2009-02-09 12:00:00 +0300
categories:
  - wcf
tags:
  - windows-communication-foundation
---
Servis tabanlı uygulamalarda en önemli noktalardan biriside aradaki bilgi transferlerinin nasıl ve ne şekilde gerçekleştirildiğidir. Gerçek şuki, bu bilgi transferinin oluşma şekli çoğu zaman geliştiricinin gözünden kaçan yada çok fazla ilgilenmediği bir konu olmaktadır. Nitekim çoğu servis geliştirme aracı buradaki söz konusu içeriğin hazırlanmasını, gönderilmesini veya alınmasını otomatikleştirmektedir. Özellikle Windows Communication Foundation tarafında, bilginin istemci ve servis arasındaki dolaşımında bağlayıcı tiplerin (Binding Type) seçilmesi ile zaten arka tarafta ne şekilde bir haberleşme olacağı ve paketlerin nasıl hazırlanacağı belirlenmiş olur.

Aslında servis ve istemci tarafında mesaj bazlı bir iletişim olduğu son derece açıktır. Farklı platformlar üzerinde koşan servislerin haberleşmeleri yada farklı tipteki istemci uygulamaların servisleri kullanabilmeleri gerektiğinde ise, aradaki haberleşmenin bir standart üzerinde ve esnek olması beklenir. Bu nedenle özellikle SOAP bazlı web servisleri göz önüne alındığında mesajın tipi ve içeriğide bellidir. İşte burada SOAP (Simpe Object Access Protocol) tarzı mesajlardan söz edilebilir. Tipik olarak SOAP mesajları bir zarf olarak temsil edilmekte (SOAP Envelope) ve Header, Body isimli iki parçadan oluşmaktadır. Aşağıdaki şekilde bu içerik temsil edilmeye çalışılmıştır.

![mk269_1.gif](/assets/images/2009/mk269_1.gif)

Peki bu mesajların makalemize konu olmasının sebebi nedir? Bilindiği üzere WCF mimarisinde çeşitli tipte sözleşmeler (Contracts) söz konusudur. Örneğin servislerin ne iş yaptığının, nasıl fonksiyonellikler sunduğunun ifade edilmesinde Sevis Sözleşmeleri (Service Contracts) kullanılmaktadır. Benzer şekilde istemci tarafına aktarılacak serileştirilebilir (Serializable) tipler söz konusu ise Veri Sözleşmeleri (Data Contracts) tanımlanır. Yine istemci tarafına aktarılacak istisna mesajlarının çeşitli durumlar için özelleştirilmesi düşünüldüğünde Hata Sözleşmeleri (Fault Contracts) kullanılır. Ancak bu sözleşme çeşitleri dışında birde Mesaj Sözleşmeleri (Message Contracts) bulunmaktadır. İşte bu yazımızın konusuda budur.

Yazımıza servis odaklı uygulamalarda mesajların yerini konumlandırmaya çalışarak başladık. Özellikle SOAP tabanlı bu mesajlar gerektiğinde özel olarak tasarlanabilirler. WCF tarafında bunu gerçekleştirebilmek için Mesaj Sözleşmelerinden yararlanılır. Mesaj sözleşmelerinin ne zaman kullanılacağına karar verilmesi genellikle zordur. Farklı platformlar için destek verebilme imkanı (Interoperability) ve mesaj kontrolü çoğunlukla karar vermeyi kolaylaştırmaktadır. Gerçektende servis tarafından istemciye gönderilecek veya alınacak mesajların farklı platformlara destek verebilecek şekilde tasarlanması gerektiği durumlarda özel Mesaj Sözleşmeleri göz önüne alınabilir. Diğer taraftan Mesaj Sözleşmeleri ile taşınacak bilginin değişik parçalarının SOAP paketinin Header veya Body kısmına ayrıştırılması ve bu sayede de, gerekli olmayan parçaların mesaj ile birlikte taşınmaması sağlanabilmektedir. Bu tam anlamıyla aradaki mesajlaşmanın kontrol altına alınması anlamına gelmektedir. Hatta, istemci ve servislerin belirli olduğu vakalarda, arada özel bir mesaj formatına göre veri içeriğinin taşınmasıda mümkün olabilir. Diğer taraftan göz ardı edilmemesi gereken bir noktada, mesaj seviyesinde güvenliktir. Mesaj Sözleşmeleri kullanılırken bir tipin SOAP zarfının içerisindeki yayılımı belirlenebildiği gibi (hangi kısımları Header'da olacak vb...) verinin şifrelenmeside (Encryption) özelleştirilebilir. Böylece vakaya göre bir mesaj deseninin oluşturulması ve kullanılması mümkün olabilmektedir.

> Çoğu durumda Mesaj Sözleşmeleri yerine Veri Sözleşmelerininde aynı işi yapıyor olduğu görülür. Ancak genel kanıya göre, eğer bir tip n sayıda mesaj içerisinde kullanılacaksa (yani reusable type olarak düşünülebilirse) Veri Sözleşmesi olarak tanımlanması önerilmektedir. Ancak tip (type) sadece istek/cevap (Request/Respone) modeline göre bir kereliğine kullanılıyorsa, Mesaj Sözleşmesi olacak şekilde tanımlanır.

Mesaj sözleşmelerinin uygulanması son derece kolaydır. Ancak dikkat edilmesi gereken noktalar vardır. Herşeyden önce MessageContract, MessageHeader, MessageBodyMember, MessageHeaderArray gibi niteliklerinden (attributes) yararlanılarak Mesaj Sözleşmesi tanımlanabilmektedir. Bununla birlikte servis operasyonlarında Mesaj Sözleşmelerinin kullanılması söz konusu ise metod yapısında uyulması gereken kurallar vardır. Buna göre metod desenleri aşağıdaki örnekler olduğu gibi olmalıdır. Bu tablodaki örnek kullanımlarda yer alan ProductOrderResponse ve ProductOrderRequest isimli tipler örnek Mesaj Sözleşmesi sınıflarıdır.

Geçerli Mesaj Sözleşme Kullanımları

[OperationContract]
ProductOrderResponse CompleteOrderProcess (ProductOrderRequest request);
Operasyonun dönüş tipi ve parametresi Mesaj Sözleşmesi tipindendir.

[OperationContract]
ProductOrderResponse CompleteOrderProcess ();
Operasyon parametre almamakta ve Mesaj Sözleşmesi tipinden referans döndürmektedir.

[OperationContract]
void CompleteOrderPrococes2 (ProductOrderRequest request);
Operasyon Mesaj Sözleşmesi tipinden parametre almakta ama değer döndürmemektedir.

Geçersiz Mesaj Sözleşme Kullanımları

[OperationContract]
int CompleteOrderProcess (ProductOrderRequest request);
Parametrenin Mesaj Sözleşmesi olduğu durumlarda dönüş tipi olarak Mesaj Sözleşmesi harici bir tip kullanılamaz. Exception üretir.

[OperationContract]
void ComplteOrderProcess (ProductOrderRequest request1, ProductOrderRequest request2);
Birden fazla Mesaj Sözleşmesi parametre olarak kullanılamaz. Exception üretir.

Bu kısa teorik bilgileri devam ettireceğiz ancak dilerseniz basit bir örnek üzerinden ilerleyerek devam edelim. Öncelikli olarak bir WCF Sınıf Kütüphanesi projesi oluşturduğumuzu düşünelim. Bu projemizde yer alacak olan tiplerin sınıf diygramındaki görüntüsü aşağıdaki gibi tasarlanabilir.

![mk269_2.gif](/assets/images/2009/mk269_2.gif)

Sınıf diyagramı (Class Diagram) gözümüzü korkutmasın. Senaryomuz aslında sadece Mesaj Sözleşmelerinin nasıl kullanılacağını göstermeye yönelik olduğundan çok anlamlı olmayan operasyonlar içermekte. Bu yüzden örnek olarak bir sipariş sürecine özel mesajları tasarladığımız bir durum söz konusu. Kullanılan tiplerin içerikleri sırasıyla aşağıdaki gibidir;

Product Sınıfı. (Veri Sözleşmesi-Data Contract olarak tanımlanmıştır)

```csharp
using System;
using System.Runtime.Serialization;

namespace ProductTransferLib
{
    [DataContract(Namespace = "http://Northwind/ProductTransferService/Product")]
    public class Product
    {
        [DataMember(Order=0)]
        public int ProductId { get; set; }
        [DataMember(Order=1)]
        public string Name { get; set; }
        [DataMember(Order=2)]
        public double ListPrice { get; set; }
        [DataMember(Order=3)]
        public DateTime OrderDate { get; set; }
    }
}
```

CustomerNumber yapısı-struct.(Veri Sözleşmesi-Data Contract olarak tanımlanmıştır)

```csharp
using System.Runtime.Serialization;

namespace ProductTransferLib
{
    [DataContract(Namespace = "http://Northwind/ProductTransferService/CustomerNumber")]
    public struct CustomerNumber
    {
        [DataMember]
        public char Region { get; set; }
        [DataMember]
        public int Number { get; set; }
        [DataMember]
        public string LastName { get; set; }
    }
}
```

Receiver Sınıfı (Veri Sözleşmesi olarak tanımlanmıştır)

```csharp
using System.Runtime.Serialization;

namespace ProductTransferLib
{
    [DataContract(Namespace="http://Northwind/ProductTransferService/Receiver")]
    public class Receiver
    {
        [DataMember]
        public int ReceiverId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public CustomerNumber Number { get; set; }
        [DataMember]
        public int RequestedProductCount { get; set; }
    }
}
```

Sender Sınıfı.(Veri Sözleşmesi olarak tanımlanmıştır)

```csharp
using System.Runtime.Serialization;

namespace ProductTransferLib
{
    [DataContract(Namespace = "http://Northwind/ProductTransferService/Sender")]
    public class Sender
    {
        [DataMember]
        public int SenderId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public CustomerNumber SenderNumber { get; set; }
    }
}
```

RequestStatus Enum sabiti.(Veri Sözleşmesi olarak tanımlanmıştır. Enum sabiti söz konusu olduğu için değerler DataMember yerine EnumMember isimli nitelik ile işaretlenmiştir.)

```csharp
using System.Runtime.Serialization;

namespace ProductTransferLib
{
    [DataContract(Namespace = "http://Northwind/ProductTransferService/RequestStatus")]
    public enum RequestStatus
    {
        [EnumMember]
        Ok,
        [EnumMember]
        Error,
        [EnumMember]
        Waiting
    }
}
```

Buraya kadar tanımladığımız tipler içerisinde sınıf, yapı ve enum sabiti tipleri söz konusudur. Bu tipler birer Veri Sözleşmesi olarak tanımlanmıştır ve Mesaj Sözleşmeleri içerisinde ele alınmaktadır. Yazımızın konusu olan Mesaj Sözleşmelerinden iki adet tanımlanmalıdır. Bu tanımlamalardan birisi istek (Request) diğer ise cevap (Response) içeriklerinin yapısını işaret etmektedir. Bir başka deyişle, istemciden servise gelecek veya geriye döndürülecek olan SOAP zarflarının içerikleri kod yardımıyla belirlenmektedir. İstemci tarafından gelecek olan taleplere ait Mesaj Sözleşmesi aşağıdaki kod parçasında olduğu gibi tanımlanmıştır.

```csharp
using System;
using System.ServiceModel;

namespace ProductTransferLib
{
    [MessageContract]
    public class ProductOrderRequest
    {
        #region Header Kısmına yazılacak özellikler

        [MessageHeader]
        public Guid OrderNumber { get; set; }
        [MessageHeader]
        public DateTime OrderDate { get; set; }
        [MessageHeader]
        public Product OrderedProduct { get; set; }

        #endregion

        #region Body kısmına yazılacak özellikler

        [MessageBodyMember(ProtectionLevel=System.Net.Security.ProtectionLevel.None)] // ProtectionLevel için varsayılan değre None' dur.
        public Sender OrderSender { get; set; }
        [MessageBodyMember]
        public Receiver[] Receivers { get; set; }

        #endregion
    }
}
```

ProductOrderRequest isimli sınıf bir Mesaj Sözleşmesi olacak şekilde tanımlanmıştır. Bu nedenle MessageContract niteliği ile imzalanmıştır. Bu nitelik sadece sınıf (Class) veya yapılara (Structs) uygulanabilir. Yazımızın başında mesajın Header ve Body kısımlarından bahsetmiştik. Header kısmında taşınacak olan alan (Field) veya özellikleri (Property) belirtmek için MessageHeader niteliği kullanılmaktadır. Örnektende görüldüğü gibi, Header kısmında Guid, DateTime gibi bilinen tipler dışında Product isimli geliştirici tanımlı bir sınıfada yer verilmiştir. Söz konusu tipler mesaj içerisine alınırken serileştirilmektedir. Bu nedenle Product sınıfı ve diğer geliştirici tanımlı tipler birer Veri Sözleşmesi olarak tanımlanmıştır. Body kısmında yer alacak özellik veya alanlar ise MessageBodyMember niteliği ile tanımlanırlar. Yine Body kısmındada, Sender ve Receiver isimli geliştirici tanımlı Veri Sözleşmelerine yer verilmektedir. Özellikle Receiver tipinden bir Array kullanıldığınada dikkat edilmelidir.

> Header veya Body kısımlarında Array'ler kullanılıyorsa MessageHeader ve MessageBodyMember nitelikleri bu dizilerin elemanlarını bir elementin alt elementleri (Child Element) olacak şekilde konumlandırır. Örneğin;
> içeriği
> içeriği
> Ancak istenirse her bir dizi elemanının ayrı birer boğum olarak ele alınması sağlanabilir. Bunun için MessageHeaderArray niteliği kullanılır.
> içeriği
> içeriği
> Yanlız bu nitelik sadece dizilere uygulanabilir. Bir başka deyişle koleksiyonlara uygulanamamaktadır.
> Eğer SOAP içeriğinde byte tipinden bir diziye yer verilmişse MessageHeader veya MessageBodyMember niteliklerinin kullanılması halinde bunlar doğrudan Base64 tipine dönüştürülürler. Ancak, eğer MessageHeaderArray niteliği kullanılıyorsa, ele alınan serileştirme tipine göre (DataContractSerializer, XmlSerializer gibi) bir aktarım gerçekleştirilir.

MessageHeader ve MessageBodyMember niteliklerinde yer alan ProtectionLevel özelliği kullanılarak dijital olarak imzalama (Sign) veya şifreleme (Encryption) sağlanabilir. ProtectionLevel özelliği System.Net.Security.ProtectionLevel enum sabiti tipinden bir değer alabilir. Bu değerler None, EncryptAndSign, Sign olabilir. Varsayılan değeri None'dur. Sign seçilirse dijital imzalama söz konusudur. EncryptAndSign seçilirsede şifreleme ve dijital imzalama söz konusudur.

Elbette None dışındaki değerlerin işe yaraması için WCF çalışma ortamına yönelik olaraktan gerekli Binding ve Behavior ayarlamalarının yapılması gerekir. Aksi durumda çalışma zamanında doğrulama işlemi sırasında bir istisnası alınır. ProtectionLevel, Header kısmında her bir eleman için ayrı ayrı uygulanmaktadır. Body kısmı söz konusu olduğunda ise kaç eleman olursa olsun hepsi için aynı ProtectionLevel seviyesi söz konusudur. Buna göre MessageBodyMember niteliği içinde seviyesi yüksek olan ProtectionLevel değeri, diğerleri içinde uygulanır. Söz gelimi 3 farklı MessageBodyMember için sırasıyla None, EncryptAndSign, Sign değerleri belirlenmişse, tüm mesaj gövdesi için EncrptyAndSign seçeneği göz önüne alınmaktadır.

> SOAP ile ilişkili web servisi standartlarında 1.1 versiyonu için Actor ve 1.2 için Role adı verilen bir özellik yer almaktadır. Bu özelliğin değerini WCF tarafında ele almak için MessageHeader niteliğinin Actor özelliği kullanılır. Bunun dışında MustUnderstand ve Relay özelliklerindende yararlanılarak, SOAP standarlarına göre bazı niteliklerin mesajlaşma süreçlerine kazandırılması da sağlanabilir.

İstemciye gönderilecek cevap mesajının içeriği ise ProductOrderResponse isimli Mesaj Sözleşmesi ile tanımlanmaktadır.

```csharp
using System.ServiceModel;
using System;

namespace ProductTransferLib
{
    [MessageContract]
    public class ProductOrderResponse
    {
        [MessageBodyMember]
        public RequestStatus Status { get; set; }

        [MessageBodyMember]
        public DateTime ProcessDate{ get; set; }

        [MessageBodyMember]
        public byte[] OrderPicture { get; set; } // Burada byte[] tipinden bir dizi söz konusu olduğu için SOAP body' si içerisinde Base64 tipinden bir kodlama(encoding) söz konusu olacaktır

        [MessageHeader]
        public int OrderdProductCount { get; set; }
    }
}
```

ProductOrderResponse tipi Mesaj Sözleşmesi olarak tanımlanırken amaç istemci tarafına gönderilecek olan SOAP mesajının Header ve Body kısımlarında neler olacağına karar verilmesidir. Dikkat edileceği üzere Body kısmında RequestStatus enum sabiti, DateTime ve byte[] dizisi tipinden bir içerik yer almaktadır. Diğer taraftan Header kısmında ise örnek olarak int veri tipinden bir değer döndürülmektedir.

> Bir tipin hem MessageContract hemde DataContract olacak şekilde tanımlanması da mümkündür. Böyle bir vakada, WCF çalışma zamanında servis operasyonları uygulanırken, söz konusu tip için Mesaj Sözleşmesi kriterleri göz önüne alınmaktadır.

Artık istemci ve servis arasında dolaşacak olan SOAP mesajlarına ait içerikler tanımlanmıştır. Dolayısıyla bu mesajlaşma modelini kullanacak bir Servis Sözleşmesi ve uygulayıcı sınıfı tasarlanabilir. Dikkat edileceği üzere Servis Sözleşmesinde yer alan CompleteOrderProcess isimli operasyonun dönüş tipi ve parametresi birer Mesaj Sözleşmesidir.

```csharp
using System.ServiceModel;

namespace ProductTransferLib
{
    [ServiceContract(
                                Name="ProductTransferService"
                                ,Namespace="http://Northwind/ProductTransferService")]
    public interface IProductTransferService
    {
        [OperationContract]
        ProductOrderResponse CompleteOrderProcess(ProductOrderRequest request);
    }
}
```

Operasyonun uygulanışı içinse aşağıdaki gibi bir kod örneği geliştirilebilir.

```csharp
using System;
using System.IO;

namespace ProductTransferLib
{
public class ProductTransferService
    :IProductTransferService
{
    #region IProductTransferService Members

    public ProductOrderResponse CompleteOrderProcess(ProductOrderRequest request)
    {
        DateTime requestDate = request.OrderDate;
        Guid requestOrderNumber = request.OrderNumber;
        Sender requestSender = request.OrderSender;
        Receiver[] requestReceivers = request.Receivers;

        int orderedProductCount = 0;
        foreach (Receiver receiver in requestReceivers)
        {
            orderedProductCount += receiver.RequestedProductCount;
        }

        // Not : XP_HDD.gif resminin byte içeriğinin dizi boyutu istemci tarafına gönderilebilecek varsayılan dizi limini aşabilir. Bu nedenle istemci tarafındaki konfigurasyon ayarlarında maxArrayLength değerinin bilinçli olarak arttırılması gerekebilir.
        return new ProductOrderResponse
                        {
                            ProcessDate=DateTime.Now,
                            Status= RequestStatus.Ok,
                            OrderPicture=File.ReadAllBytes(System.Environment.CurrentDirectory + "\\XP_HDD.gif"),
                            OrderdProductCount=orderedProductCount
                        };
        }

        #endregion
    }
}
```

Burada request değişkeninden yararlanılarak istemci tarafından gelen SOAP paketindeki mesaj içeriği ele alınmakta ve kullanılmaktadır. Sembolik olarak paket içerisinde gelen Receivers dizisindeki her bir Receiver nesne örneğinin sipariş sayısının toplamı tespit edilmektedir. Ayrıca örnek byte[] içeriği döndürülmesi için küçük bir resim dosyasından (XP_HDD.gif) yararlanılmaktadır. İşlemin tarihi, durumu, sipariş ile ilişkili resim ve toplam sipariş sayısı bilgileri kullanılaraktanda bir cevap mesajı oluşturulmakta ve istemci tarafına gönderilmektedir.

> SOAP mesajlarının içerikleri aslında XML tabanlıdır. Bu içeriği yönetirken Mesaj Sözleşmeleri, nesne tabanlı bir modeli ele alabilmemizi sağlamaktadır. Bir başka deyişle, kod tarafında XML yapısı ile uğraşmak yerine, nesne tabanlı bir modeli kullanarak mesaj içeriğini kolayca oluşturabilmemiz olanaklı hale gelmektedir ki bu geliştirme süreci için önemli bir avantajdır.

Bu işlemlerin tamamlanmasının ardından servis kütüphanesini Host edecek basit bir uygulama geliştirilebilir. Biz örneğimizde her zaman olduğu gibi, sunucu ve istemci tarafları için birer Console uygulaması geliştiriyor olacağız. Sunucu uygulama kodları ve konfigurasyon içeriği aşağıdaki kod parçalarında olduğu gibi tanımlanabilirler.

Sunucu uygulama kodları;

```csharp
using System;
using System.ServiceModel;
using ProductTransferLib;

namespace ServerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(ProductTransferService));
            host.Open();
            Console.WriteLine("Servis dinlemede.\nKapatmak için bir tuşa basınız.");
            Console.ReadLine();
            host.Close();
        }
    }
}
```

Sunucu tarafı konfigurasyon içeriği;

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <behaviors>
            <serviceBehaviors>
                <behavior name="ProductTransferServiceBehavior">
                    <serviceDebug includeExceptionDetailInFaults="true" />
                    <serviceMetadata />
                </behavior>
            </serviceBehaviors>
        </behaviors>
        <services>
            <service behaviorConfiguration="ProductTransferServiceBehavior" name="ProductTransferLib.ProductTransferService">
                <endpoint address="" binding="basicHttpBinding" bindingConfiguration="" name="ProductTransferServiceHttpEndPoint" contract="ProductTransferLib.IProductTransferService" />
                <endpoint address="Mex" binding="mexHttpBinding" bindingConfiguration="" name="ProductTransferServiceMexEndPoint" contract="IMetadataExchange" />
                <host>
                    <baseAddresses>
                        <add baseAddress="http://buraksenyurt:1000/ProductTransferService" />
                    </baseAddresses>
                </host>
            </service>
        </services>
    </system.serviceModel>
</configuration>
```

Sunucu uygulama basit olarak HTTP tabanlı bir sunum yapmakta ve BasicHttpBinding bağlayıcı tipini ele almaktadır. Bununla birlikte istemci tarafının, servise ait Metadata bilgisini çekebilmesi için IMetadataExchange arayüzünü kullanan bir MexHttpBinding EndPoint'ide kullanılmaktadır.

İstemci uygulamamızı kullanırken yine Add Service Reference seçeneği ile aynı solution içerisinde yer alan örnek servise ait referans üretimini gerçekleştirebiliriz. İstemci tarafına ait konfigurasyon içeriği aşağıdaki gibidir (Bu içerik Add Service Reference seçeneğinin kullanılması sonucunda otomatik olarak üretilmektedir.)

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <bindings>
            <basicHttpBinding>
                <binding name="ProductTransferServiceHttpEndPoint" closeTimeout="00:01:00" openTimeout="00:01:00" receiveTimeout="00:10:00" sendTimeout="00:01:00" allowCookies="false" bypassProxyOnLocal="false" hostNameComparisonMode="StrongWildcard" maxBufferSize="65536" maxBufferPoolSize="524288" maxReceivedMessageSize="65536" messageEncoding="Text" textEncoding="utf-8" transferMode="Buffered" useDefaultWebProxy="true">
                    <readerQuotas maxDepth="32" maxStringContentLength="8192" maxArrayLength="163840" maxBytesPerRead="4096" maxNameTableCharCount="16384"/>
                    <security mode="None">
                        <transport clientCredentialType="None" proxyCredentialType="None" realm="" />
                        <message clientCredentialType="UserName" algorithmSuite="Default" />
                    </security>
                </binding>
            </basicHttpBinding>
        </bindings>
        <client>
            <endpoint address="http://buraksenyurt:1000/ProductTransferService" binding="basicHttpBinding" bindingConfiguration="ProductTransferServiceHttpEndPoint"
contract="ProductTransferServiceReference.ProductTransferService" name="ProductTransferServiceHttpEndPoint" />
        </client>
    </system.serviceModel>
</configuration>
```

İstemci tarafındaki örnek kod içeriği ise aşağıdaki gibidir.

```csharp
using System;
using System.IO;
using ClientApp.ProductTransferServiceReference;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ProductTransferServiceClient client = new ProductTransferServiceClient("ProductTransferServiceHttpEndPoint");

            Sender sndr = new Sender
                                {
                                    Name="Burak Selim",
                                    SenderId=10001,
                                    SenderNumber=new CustomerNumber{ Number=1, Region='A', LastName="SENYURT"} 
                                };

            Receiver[] receivers = {
                new Receiver{ Name="Bil", Number=new CustomerNumber{ LastName="Geyts", Region='B', Number=1}, ReceiverId=10002, RequestedProductCount=100},
                new Receiver{ Name="Deyv", Number=new CustomerNumber{ LastName="Masteyn", Region='C', Number=2}, ReceiverId=10003, RequestedProductCount=150},
                new Receiver{ Name="Co", Number=new CustomerNumber{ LastName="Satriyani", Region='C', Number=3}, ReceiverId=10055, RequestedProductCount=75}
            };

            RequestStatus requestStatus;
            DateTime processDate;
            byte[] orderPicture;

            Console.WriteLine("Sipariş için bir tuşa basınız.");
            Console.ReadLine();
    
            int result=client.CompleteOrderProcess(
                                                                        DateTime.Now,
                                                                        Guid.NewGuid(),
                                                                        new Product{ ProductId=1, Name="Her Yönüyle WCF", ListPrice=10, OrderDate=DateTime.Now},
                                                                        sndr,
                                                                        receivers,
                                                                        out orderPicture,
                                                                        out processDate,
                                                                        out requestStatus);

            Console.WriteLine("result {0}",result.ToString());
            File.WriteAllBytes(System.Environment.CurrentDirectory + "\\ResponsePicture.gif", orderPicture);

            Console.WriteLine("İşlemler tamamlandı. Çıkmak için bir tuşa basınız.");
            Console.ReadLine();
        }
    }
}
```

İstemci uygulamada servise ait proxy nesnesi örneklendikten sonra CompleteOrderProcess metodunun ihtiyacı olan parametreler hazırlanmaktadır. CompleteOrderProcess metodu aslında ProcessOrderResponse Mesaj Sözleşmesi tipinden bir parametre almaktadır. Ne varki istemci tarafında metodun uygulanış şekli biraz farklıdır. Herşeyden önce, servise gönderilecek SOAP paketi içerisinde yer alacak Header ve Body elementlerinin her biri, istemci tarafında ayrı birer metod parametresi şekline ele alınmaktadır.

Metodun çağırılması sonucu istemciye dönecek olan SOAP mesajındaki Header kısmında yer alan int değer aslında istemci tarafında, CompleteOrderProcess'in dönüş değeridir. Yine istemciye döndürülen ve Body kısmında yer alan orderPicture,processDate ve requestStatus değişkenleri ise, CompleteOrderProcess metodunun out tipinden parametreleri olarak ele alınmaktadır. orderPicture değişkeni bir byte[] dizisi olarak mesaj içeriğinden toparlanmakta ve fiziki olarak istemci tarafındaki bir dosyaya yazdırılmaktadır. Bu tahmin edileceği üzere servis tarafından gönderilen resimdir. Projemizde hem sunucu hemde istemci uygulamamızı çalıştırdığımızda aşağıdaki ekran görüntüsünde yer alan sonuçları elde ederiz.

![mk269_3.gif](/assets/images/2009/mk269_3.gif)

Aslında burada şaşırtıcı bir sonuç yoktur. Nesne tabanlı olacak şekilde istemci ve servis arasındaki tipler kolay bir şekilde kullanılmıştır. Ancak bizim için önemli olan arka planda hareket eden SOAP mesajlarının içeriklerinin ne hale geldiğidir. Bu amaçla Fiddler isimli HTTP Debugging aracından yararlanırsak, örneğin çalıştırılması sonrasında ağ trafiğinde, aşağıdaki ekran görüntüsünde yer alan mesajlaşmanın oluştuğunu görürüz.

![mk269_4.gif](/assets/images/2009/mk269_4.gif)

Dikkat edileceği üzere, Mesaj Sözleşmelerinde Header ve Body kısımlarında hangi bilgilerin yer almasını istiyorsak buna göre bir ağaç yapısı oluşmuştur. Request kısmına ait olan SOAP zarfının XML içeriği tam olarak aşağıdaki gibidir.

```xml
<s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/">
    <s:Header>
        <h:OrderDate xmlns:h="http://Northwind/ProductTransferService">2009-02-08T01:53:03+02:00</h:OrderDate>
        <h:OrderNumber xmlns:h="http://Northwind/ProductTransferService">9476df3d-0f74-4643-9fcf-b7344d5da37d</h:OrderNumber>
        <h:OrderedProduct xmlns:h="http://Northwind/ProductTransferService" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
            <ProductId xmlns="http://Northwind/ProductTransferService/Product">1</ProductId>
            <Name xmlns="http://Northwind/ProductTransferService/Product">Her Yönüyle WCF</Name>
            <ListPrice xmlns="http://Northwind/ProductTransferService/Product">10</ListPrice>
            <OrderDate xmlns="http://Northwind/ProductTransferService/Product">2009-02-08T01:53:03+02:00</OrderDate>
        </h:OrderedProduct>
    </s:Header>
    <s:Body>
        <ProductOrderRequest xmlns="http://Northwind/ProductTransferService">
            <OrderSender xmlns:a="http://Northwind/ProductTransferService/Sender" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
                <a:Name>Burak Selim</a:Name>
                <a:SenderId>10001</a:SenderId>
                <a:SenderNumber xmlns:b="http://Northwind/ProductTransferService/CustomerNumber">
                    <b:LastName>SENYURT</b:LastName>
                    <b:Number>1</b:Number>
                    <b:Region>65</b:Region>
                </a:SenderNumber>
            </OrderSender>
            <Receivers xmlns:a="http://Northwind/ProductTransferService/Receiver" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
                <a:Receiver>
                    <a:Name>Bil</a:Name>
                    <a:Number xmlns:b="http://Northwind/ProductTransferService/CustomerNumber">
                        <b:LastName>Geyts</b:LastName>
                        <b:Number>1</b:Number>
                        <b:Region>66</b:Region>
                    </a:Number>
                    <a:ReceiverId>10002</a:ReceiverId>
                    <a:RequestedProductCount>100</a:RequestedProductCount>
                </a:Receiver>
                <a:Receiver>
                    <a:Name>Deyv</a:Name>
                    <a:Number xmlns:b="http://Northwind/ProductTransferService/CustomerNumber">
                        <b:LastName>Masteyn</b:LastName>
                        <b:Number>2</b:Number>
                        <b:Region>67</b:Region>
                    </a:Number>
                    <a:ReceiverId>10003</a:ReceiverId>
                    <a:RequestedProductCount>150</a:RequestedProductCount>
                </a:Receiver>
                <a:Receiver>
                    <a:Name>Co</a:Name>
                    <a:Number xmlns:b="http://Northwind/ProductTransferService/CustomerNumber">
                        <b:LastName>Satriyani</b:LastName>
                        <b:Number>3</b:Number>
                        <b:Region>67</b:Region>
                    </a:Number>
                    <a:ReceiverId>10055</a:ReceiverId>
                    <a:RequestedProductCount>75</a:RequestedProductCount>
                </a:Receiver>
            </Receivers>
        </ProductOrderRequest>
    </s:Body>
</s:Envelope>
```

Bu XML içeriği incelendiğinde tam olarak Mesaj Sözleşmesinde belirttiğimiz kriterlere uyulduğu görülmektedir. Söz gelimi Header kısmında OrderDate, OrderNumber ve OrderProduct elementleri yer almaktayken, Body kısmında ProductOrderRequest elementi tarafından sarmalanmış olan, OrderSender ve Receivers elementleri bulunmaktadır. Receivers aslında Receiver[] dizisinin kullanılması nedeni ile kendi içerisinde birden fazla Receiver alt elementi içermektedir. Burada geliştirici tanımlı tiplerin (Product,Receiver,Sender gibi) Veri Sözleşmesi olarak tanımlanmaları nedeniyle XML elemetleri içerisine aktarılmış olmalarıda gözden kaçırılmamalıdır. Yine istemciye dönen mesajın (Response) tam içeriğine bakıldığında aşağıdakine benzer bir SOAP çıktısı ile karşılaşılmaktadır.

```xml
<s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/">
    <s:Header>
        <h:OrderdProductCount xmlns:h="http://Northwind/ProductTransferService">325</h:OrderdProductCount>
    </s:Header>
    <s:Body>
        <ProductOrderResponse xmlns="http://Northwind/ProductTransferService">                 
        <OrderPicture>R0lGODlhIAAgAPcAAAAAAB8hNR4hODMpHCQgLiIkNyYqRysvUC4wQy
0wTi0xUzU3SzE1VjE1WzU5Xjs +VDw/WDU6Yzk+ZTk/bDg5dTFILDl0FzpGRz5BXD5CYz1CbD5EdVYzLXI/SF9OMF1
WP05tK0JoN0pxNVF5JVR1M2lfPn FHKn5GP3VpL31vN31wL3d0OEZIVkZUSUFFY0FGbURIZEZKaklLYExPaEJIek1SaEx
Td1VWalNXd1prWFp1Rl96bWRcS 2JNYGtrRnhuRGdlaGNkdmh5eTc3mDxDxT5P2Dxdxj5d3T9g1z9r40VMg0pRiVZbhFR
bl1hkilljl0Nxr0h2t1tlqFVzqGFmjW ZpgWtthGlti2Npl25xiGd6l25xkG1ym3ByhnN0inl2h3N2kXF2m3R4lnR5mXp8lWNqq2p
zp2xzsXN5q3l+pXp/qnR7tEFW0E Jt0EN85l+fGkmLJ0mSIVOKJFWJNVeXJFqpH1y2GFqiJGOWKWK3Hm2oIV6KVFnFGGn
NF3POGnrSGWHJIHbNInvQIHiDiH qAonyCuH6Qr0qK3kOF7EeP8EqU8Vua6lWb8miW1XGYzWWk8XSq73et8H+x8YM2N
5oxJ7MsG4pfPIN4K5J7LpR0OKFV KaB2HbB+FYNtQpp8eKRNR7JyRtAqE+UsEfg+D/Q2ENRNE9dyH8F0NuxHD/pJDfxWDO
1+HP1lDv12EbeDC7OaFKmUIqm XNruDK72nEKyMfoHSHYTRJMeKCMiRCN+GH9WWBc2xDNqjA8GnIv2GE/uaF/CGIf2W
IOCrBOS1BfqlHPq0HfysIfu3I+vG A/fXBPrcFvzeKfzeOP3gOIKDj4yDlZCCkYaKrYGHs4CGu4SJtYWLvYqPtIiNuo6TvJWXp
pGVu5+ivoiOwIySwpGWxJSZxJ ecyJmdxJmeyJ2hw52iy4i28JK88aGlxqGmzKSpzqmsyaar0Kmt0a6xzK2x07a4zbG11bS4
17a52Lq907m92ZzC8b7B3K3J 7aXH8MTG3MjL4c7R5dHT5gAAAAAAAAAAACH5BAEAAP8ALAAAAAAgACAAAAj/AP8JH
EiwoMGDCNvNsCcvHjx37tixW 5cuHTpy5MSB++atW7UM2BDaszduGxkyXrp0AWMFTBYrYqpY+8bxmxpu0xDSs5eGTIIX
DjRIcCBBgoYAWxCE4ditG45 q1Q7mo0ePEJkFCAoM8GCCA4ECBWAsCAOuo1OPB/nJgzdP3z5auY4Ra6UK1ad3+aaKa1r
NBreoBvM5hLf21K9jv+iOCu VlkLhv1TxWe1ponsF58DK707bKWDFfrFKRopRDzpwdY8II2QMHS2WD9B5C9BRMri9Vojvo
oGOnT58/f/pYoFLonMF0E CeCClZsLm5RN0bw/g2cTwUsa74ZRAdP4jhTxIr9/2KlalSlG3Tq8KHeh88DJ2uobU+QThw0YLI
yWdJUitSk0nX4VosgfcT BhAsNoGHQQ9es8wwLNzjhhA01nNADCXTk0YcftgQCSAhmwGeGQe6MQ006UjhyTzmPJGLEEl
yAcIeGgXTIBwtrPLHGG QXls04Y6URziDn4lMMIIkcM8cUNLcjxhh54iHABE2tg8QSPBOmDDhbsmPFIPfVEwogbR1CQyS2Y
8LAACx9ckoIZa5SBA 5YDtUMOF+JAAQk+5jyiSBJEBMEJL8Mko0wzyiRTghlnYNFEGe0Q5A453VATRTlFPoLkEEDowgsyh
jKTDDIxnHHGE2WU oQ5BF4lDyCL14P8jphtsUNCJLsIkY+gyybzChKlnpGoNQdpoNEUjhmjxBBRtEPHCJroUmswyia5wJTep
SiHfQN885gQLz4 BBhRMwUHDDLZ/q2oyuLEgjTqplSFEGQeBsRA013dCTDzloUBGEK8KAqgyvu1hRDbxSSPEEQZA11c01
3HTjDj/6qNPFD 7DM0kssKDzwaLwJN9HEQO9QY8A13fxVzTTTFFJIN95EcwUONdCwBKogS6GEAiMPhFxEE1Fk0UXhFB0
OTdqEow022i zN9IgDMbCBEgUYoEADEUQwAQ00NKCAAgd8DfbUSpQ9NQ4EZUCDElZnPcEGG3DttQNifz01EzgoYfMVB
ME9sLYBAhj QgAZxr+2113XH7cMKeivhBUHZxBADBA9U/gAGGLjgAgYZYO555pprHgMNCJVu+umop6766gcFBAA7
        </OrderPicture>
        <ProcessDate>2009-02-08T01:53:03.46875+02:00</ProcessDate>
        <Status>Ok</Status>
        </ProductOrderResponse>
    </s:Body>
</s:Envelope>
```

Gözden kaçmayacak olan nokta OrderPicture elementinin içeriğidir:) Tahmin edileceği üzere bu elementin içeriği, servis tarafındaki resmimizin byte[] dizisi haline geldikten sonra, SOAP mesajı içeriğine Base64 kodlamasına göre serileştirilmiş halidir. Bu içerik, istemci tarafında ters serileştirilme işleminden sonra yine byte[] dizisi olacak şekilde ele alınabilmektedir. Bunların haricinde Header kısmında OrderProductCount elementinin, Body kısmında ise ProductOrderResponse elementi ile sarmalanmış olan OrderPicture, ProcessDate ve Status alt elementlerinin olduğu görülmektedir.

Mesaj Sözleşmelerinde ele alınan bir diğer durumda türlendirilmemiş versiyonların kullanılmasıdır (Untyped Message Contracts). Burada System.ServiceModel.Channels isim alanında yer alan Message sınıfı ele alınmaktadır. SOAP 1.1 ve SOAP 1.2 uyumlu mesajları işaret edebilen bu sınıf yardımıyla, istemciden gelen talepler ele alınabilir ve cevaplar oluşturularak Message tipinden örnekler üzerinden karşı tarafa gönderilebilir. Son olarak bu durumu değerlendirip makalemizi tamamlayalım. Bu amaçla Servis Sözleşmemize aşağıdaki ekran görüntüsünde yer alan yeni bir operasyon ilave ettiğimizi düşünelim.

![mk269_5.gif](/assets/images/2009/mk269_5.gif)

RunProcess isimli operasyon parametre ve dönüş değeri olarak Message tipini kullanmaktadır. Söz konusu operasyon metodunun ProductTransferService içerisindeki uyarlaması ise aşağıdaki gibi yapılabilir.

```csharp
// Untyped Message alıp veren örnek servis operasyonu metodu.
public Message RunProcess(Message request)
{ 
    // Servise gelen Untyped Message ' ın Body kısmında yer alan Product dizi içeriğini elde etmek için GetBody metodunun generic versiyonundan yararlanılır.
    Product[] products = request.GetBody<Product[]>();

    // İstemciye döndürelecek Untyped Message' ın Body kısmında yer alacak örnek Product içeriği için dizi oluşturulur.
    Product[] resultSet=new Product[products.Length];

    // Gelen mesajın Body kısmından elde edilen dizi üzerinde örnek işlemler yapılır.
    // Örnekte ListPrice bilgisi 1 birim arttırılmıştır.
    for (int i = 0; i < products.Length; i++)
    {
        products[i].ListPrice += 1;
        resultSet[i] = products[i];
    }

    // Operasyondan döndürelecek olan Untyped Message oluşturulur.
    // İlk parametre SOAP versiyonunu belirtir. Örneğin "SOAP 1.1".
    // İkinci parametre servis operasyonunda ReplyAction özelliğine atanan değerdir.
    // Üçüncü parametre ise Body kısmında yer alacak olan nesne örneğidir.
    Message response = Message.CreateMessage(request.Version, "ReplyAction",resultSet);

    // Untyped Message geriye döndürülür.
    return response;
}
```

RunProcess isimli servis operasyonu, istemciden gelen mesajın Body kısmında yer alan Product nesne verilerini ele almakta ve örnek olarak ListPrice değerlerini 1 birim arttırarak geriye döndürmektedir. Metoda gelen türlendirilmemiş mesajın gövdesindeki veri içeriğini ele alabilmek için GetBody metodundan yararlanılır. Tahmin edileceği üzere metodun kullandığı generic tip üzerinden bir XML ters serileştirme işlemi söz konusudur. Nitekim istemciden gelen mesaj XML tipindedir ve kod içerisinde nesnel olarak kullanılması gerekmektedir.

Bunlara ek olaraktan, servisin istemciye göndereceği türlendirilmemiş mesajın üretimi için, Message sınıfının static CreateMessage fonksiyonundan yararlanılır. Metodun aşırı yüklenmiş (overload) 11 farklı versiyonu bulunmaktadır. Örneğimizde kullandığımız halinde, ilk parametre ile SOAP versiyonu, ikinci parametre ile SOAP Action adı ve son olarak üçüncü parametre ilede Body kısmına gelecek olan nesne örneği belirtilmiştir. Eklenen bu yeni fonksiyonellik nedeniyle istemci tarafında yer alan servis referansınında güncellenmesi gerekmektedir. Bu güncelleme işleminin ardından RunProcess isimli operasyon istemci tarafında örnek olarak aşağıdaki kod parçasında görüldüğü gibi kullanılabilir.

```csharp
Console.WriteLine("\nUntyped Message\n");

// Güncel kanal implementasyonundan yararlanarak OperationContextScope nesnesi örneklenir.
// OperationContextScope nesnesinden yararlanarak gelen ve giden mesajların içerikleri yönetilebilir, Header, Body gibi kısımlarına müdahale edilebilir.
using (new OperationContextScope(client.InnerChannel))
{
    // Untyped mesaj içerisinde gönderilecek olan Product nesneleri için bir dizi hazırlanır.
    Product[] products = 
        {
            new Product{ Name="Programming WCF", ListPrice=12, OrderDate=DateTime.Now, ProductId=19},
            new Product{ Name="Programming C# 3.0", ListPrice=16, OrderDate=DateTime.Now, ProductId=21}
        };

    // İstemciden servise gönderilecek olan Untyped Message hazırlanır.
    // İlk parametre mesaj versiyonudur. (SOAP 1.1 gibi).
    // İkinci parametre servis sözleşmesinde RunProcess operasyonunda belirtilen Action özelliğinin değeridir.
    // Üçüncü parametre ise mesaj içeriğinde gönderilecek olan serileştirilebilir nesne örneğidir. Bu örnekte Product tipinden bir dizi kullanılmaktadır.
    Message request = Message.CreateMessage(OperationContext.Current.OutgoingMessageHeaders.MessageVersion, "RequestAction", products);

    // Operasyon çağrısı yapılır ve parametre olarak hazırlanan Untyped Message örneği gönderilir.
    // Çağrı sonucu yine bir Untyped Message örneğidir.
    Message reply = client.RunProcess(request);

    // Servisten gelen Untyped Message içerisindeki Body kısmında tutulan Product topluluğunu dizi olarak ele almak için GetBody metodunun generic versiyonu kullanılır.     Bunun sonucu olarak elde edilen sonuç Product tipinden bir dizi olacaktır.
    Product[] response = reply.GetBody<Product[]>();

    foreach (Product product in response)
    {
        Console.WriteLine(product.Name+" "+product.ListPrice);
    }
}
```

İstemci tarafında RunProcess metodu çağırılmadan önce gönderilecek mesajın oluşturulması için yine CreateMessage static metodundan yararlanılmaktadır. Yine ilk parametre olarak SOAP versiyonu, ikinci parametre olarak SOAP Action değeri ve üçüncü parametre olarakta Body kısmına serileştirilecek nesne örneği belirtilmiştir. Servis tarafından gelen mesaja ait Body bilgisinin okunması içinde GetBody metodundan yararlanılmaktadır. İstemci tarafında dikkat edilmesi gereken noktalardan biriside tüm bu işlemleri içerisine alan Using bloğunda OperationContextScope nesnesinden yararlanılması ve o anki kanal (Channel) bilgisinin kullanılmasıdır. Örnek uygulamamız bu haliyle test edildiğinde çalışma zamanı görüntüsü aşağıdakine benzer olacaktır.

![mk269_8.gif](/assets/images/2009/mk269_8.gif)

Ancak elbetteki arka planda yer alan mesaj içeriğine Fiddler aracı yardımıyla bakıldığında Body kısmında hareket eden Product verilerinin içeriği açık bir şekilde görülebilmektedir.

![mk269_6.gif](/assets/images/2009/mk269_6.gif)

Dikkat edileceği üzere Request mesajında gönderilen Product nesnelerine ait ListPrice değerleri, Response mesajı içerisinde 1 birim arttırılmıştır. Eğer mesajların RAW içeriklerine bakılırsa SOAP Action bilgisininde set edilmiş olduğu görülebilir. (Size tavsiyem GetBody metodlarına olan çağrılarda BreakPoint kullanarak request ve response değişkenlerinin çalışma zamanı içeriklerini QuickWatch ile izlemenizdir.)

![mk269_7.gif](/assets/images/2009/mk269_7.gif)

> Eğer istemciden talep gönderildikten sonra varsayılan olarak 1 dakikalık zaman dilimi içerisinde servis tarafından cevap gelmezse aşağıdaki ekran görüntüsünde yer alan TimeoutException istisnası ile karşılaşılır.
> ![mk269_9.gif](/assets/images/2009/mk269_9.gif)
> Bu sorun SendTimeout değeri arttırılarak çözümlenebilir. Bu sorun, uzun süren operasyonların söz konusu olduğu durumda dikkate alınması gereken istisnaların başında gelmektedir.

Buraya kadar yaptıklarımıza baktığımızda, istemci ve sunucu arasındaki Mesaj içeriklerinin yönetiminin Mesaj Sözleşmeleri yardımıyla ele alınabildiği sonucu ortaya çıkmaktadır. Buna göre istenirse, istemci ve sunucu arasında taşınacak bir veri tipinin belirli parçalarının SOAP zarfı içerisindek Header veya Body bölümleri arasında ayrıştırılması mümkün olabilmektedir. Hatta, istemci ve servis arasında özel mesaj desenlerinin oluşturulması da söz konusu ve olasıdır. Elbette bu işi tamamlayıcı en önemli nokta şifreleme (Encryption) işlemlerininde hesaba katılmasıdır.

Buda yapıldığı takdirde mesajın daha güvenilir bir şekilde ele alınması ve korunması mümkün hale gelmektedir. Diğer taraftan istemci ve servis arasındaki mesajların türlendirilmemiş olmaları halindede ele alınabildikleri ve içeriklerinin yönetilebildikleride ortadadır. Mesaj Sözleşmeleri ile ilişkili olarak daha detayı bilgi almak için, [MSDN](http://msdn.microsoft.com/en-us/library/ms730255(printer).aspx)' de yayınlanan içeriği takip etmenizi öneririm. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim

[Örneği İndirmek İçin Tıklayın](/assets/files/2009/UsingMessageContracts.rar)