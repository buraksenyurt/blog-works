---
layout: post
title: "WCF - MTOM ve Stream Kullanarak Veri Aktarımı"
date: 2007-11-26 12:00:00 +0300
categories:
  - wcf
tags:
  - windows-communication-foundation
  - mtom
---
Günümüzde resim,ses, video, doküman formatında kaynakların yoğun olarak kullanıldığı pek çok sistem bulunmaktadır. Söz gelimi içerik yönetim sistemleri (Content Management Systems) neredeyse sırf bu tip verilerin kullanılması üzerine kurulmuştur. Resim, ses, video formatındaki veri kaynaklarının oluşturduğu en büyük problem ise boyutlarının söz konusu sistemlerde ne kadar etkin bir şekilde ölçeklenebildiğidir. Büyük boyutlu dosyalar çeşitli amaçlarla kullanılabilirler.

Örneğin bir şirketin tüm dökümantasyon alt yapısı bu tip büyük büyük dosyalar üzerine kurulu olabilir. Yada üretim sektöründe görev alan bir firmanın teknik çizimleri ikili (binary) formatta olacak şekilde veritabanı üzerinde saklanıyor olabilir. Bu tarz verilerin (ister dosyalarda ister veritabanı alanlarında saklanıyor olsunlar) aynı bilgisayarda kullanıldığı uygulamalarda boyutların artması çok fazla problem teşkil etmeyebilir. Ancak işin içerisine istemci-sunucu (Client-Server) tabanlı bir ortam girdiğinde boyutların artması özellikle ağ (Network) üzerindeki trafiğe olumsuz etkiler yansıtmaktadır. Dolayısıyla farklı makinelerde koşan uygulamaların arasında bu tip büyük boyutlu verilerin aktarımında dikkat edilmesi gereken bazı önemli noktalar vardır.

Servis yönelimli mimari (Service Oriented Architecture) uygulamaların geliştirilmesinde kullanılan bazı platformalar, istemci (Client) ve servisler (Service) arasında resim, ses, video gibi yüksek boyutlu içeriklere sahip olabilecek verilerin taşınması amacıyla çoğunlukla MTOM (Message Transimision Optimization Mechanism) standardını kullanırlar. WC3 tarafından kabul edilmiş bu standarda göre ikili (binary) formattaki verilerin aktarılması daha performanslıdır. Windows Communication Foundation mimarisi de, mesajların MTOM ile taşınabilmesine olanak sağlamaktadır.

Normal şartlarda HTTP üzerinden hizmet veren herhangibir servis içerisindeki metodların parametrik yapısı ve dönüş tipine bakıldığında, istemci ve servis arasında hareket eden SOAP (Simple Object Access Protocol) paketlerinin ikili formattaki verileri text formatına kodlandığı görülür. Çok doğal olarak bu kodlama (encoding) işlemleri servis tarafına ek bir yük getirmektedir. Aynı yük istemciye ulaşan mesajın içeriğindeki text tabanlı bilginin tekrar çözümlenerek (decoding) orjinal ikili formattaki haline getirilmesi içinde söz konusudr. Buradaki metodun yapısı gereği sahip olduğu sayısal nitelikteki ikili değerleri text olarak kodlamalası (encoding) çok büyük problem değildir. Ne varki bir resim dosyasının bu şekilde ele alınması halinde boyutun artması, kodlama (encoding) işleminin uzaması ve zaman alması anlamınada gelmektedir. Burada ikili formattaki veri içeriğinin text formatlı olması yerine 1 ve 0' lardan oluşacak şekilde kodlanması göz önüne alınabilir. Ancak bu durumdada milyonlarca 1 ve 0' dan ibaret bir veri yığınının oluşmasıda kaçınılmazdır.

WCF (Windows Communication Foundation) mimarisinde servis tarafındaki metodlar varsayılan olarak Base64 tabanlı olacak şekilde bir kodlama (encoding) ve çözümleme (decoding) işlemine tabi tutulurlar. Bu işlem sayesinde ikili (binary) formattaki verinin daha az yer tutması sağlanabilir. Ne varki Microsoft kaynaklarının belirttiğine göre, bu kodlama orjinal veri boyutunu yaklaşık olarak %140 oranında da arttırmaktadır. İşte MTOM (Message Transmision Optimization Mechanism) tabanlı mesajlaşma ile veri içeriğinin kodlama (encoding) ve çözümleme (decoding) işlemine gerek kalmadan taraflar arasında taşınabilmesi sağlanmaktadır. Bunun en büyük nedeni MTOM'un ikili formattaki veri içeriğini orjinal mesaja bir ek (Attachment) olacak şekilde taşıtmasıdır. Aşağıdaki şekilde bu durum biraz daha net bir şekilde ifade edilmektedir.

![mk232_1.gif](/assets/images/2007/mk232_1.gif)

Şekilde dikkat edileceği üzere, binary formattaki içerik bozulmadan bir MIME (Multipurpose Internet Mail Extension) paketi içerisine alınmaktadır. Kalan içerik yine bozulmadan text tabanlı olacak şekilde SOAP Zarfı (Envelope) halinde MIME Mesajı içerisine aktarılır. SOAP gövdesinde (Body) artık MIME mesajına ilave edilen binary içeriğin referansı tutulmaktadır. Böylece servis veya istemi tarafında, binary içeriğin text formatına kodlanması (ve tam tersinin yapılması) işlemleri ortadan kalkmaktadır. Buda çok doğal olarak mesajların hazırlanma, gönderme ve işlenme sürelerinin inanılmaz derecede azalması anlamına gelmektedir.

![dikkat.gif](/assets/images/2007/dikkat.gif)
MTOM (Message Transmision Optimization Mechanism) tabanlı mesajların güvenliğinde imzalar (signature) kullanılır. WCF bu imzaları kontrol ederek alınan veya gönderilen MIME içeriğindeki eklerin (attachments) bozulup bozulmadığını anlayabilir ve buna göre uygun çalışma zamanı istisnalarını fırlatabilir.

Windows Communication Foundation mimarisinde HTTP bazlı olan bağlayıcı tiplerin tamamı MTOM tipinde mesajlaşmayı desteklemektedir. Ne varki TCP, MSMQ gibi protokoller üzerinde çalışan bağlayıcı tiplerin (Binding Type) bu tip bir desteği yoktur. Bunun en büyük sebebi ise, bu protokollerin ikili (binary) formattaki veriler için kendi standartlarını kullanıyor olmalarıdır. Ancak daha öncedende değinildiği gibi, özel bağlayıcı tipler (Custom Binding Types) geliştirilerek TCP gibi bir protokol üzerinde MTOM kullanımı teorik olarak sağlanabilmektedir. Aşağıdaki tabloda MTOM tipinde mesajlaşmaya destek veren (vermeyen) bağlayıcı tipler işaret edilmektedir.

Bağlayıcı Tip
(Binding Types)
Mesaj Kodlama/Çözümleme Tipleri
(Message Encoding/Decoding Types)

BasicHttpBinding
Text / MTOM

NetTcpBinding
Binary

NetPeerTcpBinding
Binary

NetNamedPipeBinding
Binary

WSHttpBinding
Text / MTOM

WSFederationBinding
Text / MTOM

NetMsmqBinding
Binary

MsmqIntegrationBinding
Binary

WSDualHttpBinding
Text / MTOM

Konuyu daha iyi anlayabilmek için örnek bir senaryo üzerinden hareket etmekte fayda vardır. Bu amaçla AdventureWorks veritabanında yer alan ürünlere ait fotoğrafların tutulduğu ProductPhoto tablosundan yararlanılabilir. Söz konusu tabloda varBinary (MAX) SQL tipinden LargePhoto isimli bir alan yer almaktadır. İlerleyen örnekte LargePhoto isimli alanın içeriğinin MTOM ve Stream bazlı olacak şekilde istemci tarafına aktarılması üzerinde durulacaktır. Senaryoda göz önüne alınacak tablolar aşağıdaki veritabanı diagramında (Database Diagram) olduğu gibidir. Öncelikli olarak en azından istemciye ürün adı ve fotoğraf numarası bilgilerinin gönderilmesinde yarar vardır. Sonrasında istemcinin seçtiği ürüne ait fotoğrafın binary içeriği servis tarafından geriye doğru aktarılacaktır.

![mk232_2.gif](/assets/images/2007/mk232_2.gif)

Servis tarafındaki uygulamanın istemciye ürün listesini ve seçtiği ürüne ait resmi veren fonksiyonellikler içerdiği düşünülebilir. Bu fonksiyonellikleri içeren servis sınıfı ve üyeleri her zamanki gibi bir WCF Servis Kütüphanesi (WCF Servis Library) olacak şekilde aşağıdaki gibi tasarlanabilir. Söz konusu kütüphane içerisinde yer alacak temel tiplerin sınıf diagramındaki (Class Diagram) görüntüsü ise aşağıdaki gibidir.

![mk232_3.gif](/assets/images/2007/mk232_3.gif)

Örnekte ürünlere ait fotoğraf bilgilerinin istemciye taşınması için ProductInfo isimli bir sınıftan yararlanılmaktadır. Bu sınıfın içeriği aşağıdaki gibidir.

```csharp
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ProductPhotoServiceLibrary
{
    [DataContract]
    public class ProductInfo
    {
        private int _productPhotoId;
        private string _name;

        [DataMember]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [DataMember]
        public int ProductPhotoId
        {
            get { return _productPhotoId; }
            set { _productPhotoId = value; }
        }

        public ProductInfo(int productId, string name)
        {
            ProductPhotoId = productId;
            Name = name;
        }
    }
}
```

Bu sınıfta dikkat edilmesi gereken nokta, bir veri sözleşmesi (Data Contract) tanımlıyor olmasıdır. Bu nedenle sınıf (Class) DataContract, özellikleri (Properties) ise DataMember nitelikleri ile imzalanmıştır. Servis sözleşmesi (Service Contract) içeriği aşağıdaki gibidir.

```csharp
using System;
using System.ServiceModel;
using System.Collections.Generic;

namespace ProductPhotoServiceLibrary
{
    [ServiceContract(Name="Photo Service",Namespace="http://www.bsenyurt.com/ProductPhotoService")]
    public interface IPhotoService
    {
        [OperationContract(IsInitiating=true)]
        List<ProductInfo> GetProducts();

        [OperationContract]
        byte[] GetPhotoByProductId(int productPhotoId);
    } 
}
```

Arayüzün uygulandığı PhotoService sınıfının içeriği ise aşağıdaki gibidir.

```csharp
using System;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace ProductPhotoServiceLibrary
{
    public class PhotoService : IPhotoService
    {
        string conStr = "data source=.;database=AdventureWorks;integrated security=SSPI";

        #region IPhotoService Members
    
        // İstemci için gerekli olan ürün adları ve fotoğraf numaraları, generic bir List koleksiyonu ile geriye döndürülmektedir.
        public List<ProductInfo> GetProducts()
        {
            // Generic List koleksiyonu oluşturulur
            List<ProductInfo> infos = new List<ProductInfo>();

            using (SqlConnection conn = new SqlConnection(conStr))
            {
                // Sql sorgusunda Join kullanılmasının sebebi ProductPhotoId değerleri ve Name alanlarının değerlerinin elde edilmesidir.
                using (SqlCommand cmd = new SqlCommand("Select PP.ProductPhotoId,[Name] From Production.Product P Join Production.ProductProductPhoto PP on P.ProductId=PP.ProductID", conn))
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        // Elde edilen her bir satır için ProductInfo sınıfına ait bir nesne örneklenip generic koleksiyona eklenir.
                         infos.Add(new ProductInfo(Convert.ToInt16(reader[0]), reader[1].ToString()));
                    }
                    reader.Close();
                }
            }
            return infos;
        }

        // Belirli bir fotoğraf numarasına sahip olan LargePhoto alanının içeriği byte dizisi olacak şekilde geri döndürülür.
        public byte[] GetPhotoByProductId(int productPhotoId)
        {
            byte[] photoContent = null;
        
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                using (SqlCommand cmd = new SqlCommand("Select LargePhoto From Production.ProductPhoto Where ProductPhotoId=@PId", conn))
                {
                    cmd.Parameters.AddWithValue("@PId", productPhotoId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    // Burada GetSqlBytes metodu ilgili alanın içeriğinin binary olarak byte dizisine aktarılmasında yardımcı rol oynamaktadır.
                    if (reader.Read())
                        photoContent = reader.GetSqlBytes(0).Value;
                    reader.Close();
                }
            }
    
            return photoContent;
        }
    
        #endregion
    }
}
```

PhotoService isimli sınıf şu an için iki adet metoda sahiptir. GetProducts isimli metod basit olarak geriye ProductInfo tipinden nesne örneklerinden oluşan generic bir List<> koleksiyonu döndürmektedir. GetPhotoByProductId metodu geriye byte[] tipinden bir dizi döndürmektedir. Bu dizi tahmin edileceği üzere tabloda varbinary (MAX) tipinden olan LargePhoto isimli alanın içeriğini taşımaktadır.

![dikkat.gif](/assets/images/2007/dikkat.gif)
Binary dosyanın boyutunun çok büyük olması halinde parçalı olacak şekilde veri içeriğinin toplanması göz önüne alınabilir. Yapılması gereken servis tarafındaki fonksiyonellik içerisinde resim içeriğini veritabanı tablosunda parçalı olacak şekilde okuyup toparlamaktır.

Şimdilik servis tarafı MTOM (Message Transmision Optimization Mechanism) formatı yerine Text formatında mesaj kodlaması yapmaktadır. Olayın daha net analiz edilmesi amacıylada gerekli Diagnostics seçenekleri aktif hale getirilmektedir. Servis tarafının içeriği basit olarak aşağıdaki gibidir. (Servis IIS üzerinde host edilecek şekilde tasarlanmaktadır.)

Service.svc;

```csharp
<%@ ServiceHost Language="C#" Debug="true" Service="ProductPhotoServiceLibrary.PhotoService" %>
```

Servis (Service) tarafında mesajlaşma ile ilişkili olan içeriği izleyebilmek adına Diagnostics özellikleri açılmıştır. Buna göre web.config dosyasının içeriği aşağıdaki gibidir.

web.config;

```csharp
<?xml version="1.0"?>
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
            <source name="System.ServiceModel" switchValue="Verbose,ActivityTracing" propagateActivity="true">
                <listeners>
                    <add type="System.Diagnostics.DefaultTraceListener" name="Default">
                        <filter type="" />
                    </add>
                    <add name="ServiceModelTraceListener">
                        <filter type="" />
                    </add>
                </listeners>
            </source>
        </sources>
        <sharedListeners>
            <add initializeData="C:\inetpub\wwwroot\AdventureProductService\ web_messages.svclog" type="System.Diagnostics.XmlWriterTraceListener, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" name="ServiceModelMessageLoggingListener" traceOutputOptions="Timestamp">
                <filter type="" />
            </add>
            <add initializeData="C:\inetpub\wwwroot\AdventureProductService\ web_tracelog.svclog" type="System.Diagnostics.XmlWriterTraceListener, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" name="ServiceModelTraceListener" traceOutputOptions="Timestamp">
                <filter type="" />
            </add>
        </sharedListeners>
    </system.diagnostics>
    <appSettings/>
    <connectionStrings/>
    <system.web>
        <compilation debug="true">
        </compilation>
        <authentication mode="Windows" />
    </system.web>
    <system.serviceModel>
        <behaviors>
            <serviceBehaviors>
                <behavior name="PhotoServiceBehavior">
                    <serviceMetadata httpGetEnabled="true" />
                    <serviceDebug includeExceptionDetailInFaults="true" />
                </behavior>
            </serviceBehaviors>
        </behaviors>
        <diagnostics>
            <messageLogging logEntireMessage="true" logMalformedMessages="true" logMessagesAtTransportLevel="true" />
        </diagnostics>
        <services>
            <service behaviorConfiguration="PhotoServiceBehavior" name="ProductPhotoServiceLibrary.PhotoService">
                <endpoint address="http://localhost/AdventureProductService/Service.svc" binding="basicHttpBinding" bindingConfiguration="" name="ProductPhotoHttpEndPoint" contract="ProductPhotoServiceLibrary.IPhotoService" />
            </service>
        </services>
    </system.serviceModel>
</configuration>
```

Servis tarafı basicHttpBinding bağlayıcı tipini kullanan bir EndPoint sunmaktadır. Bununla birlikte istemcilerin HTTP üzerinden proxy sınıflarını elde edebilmeleri için metaData Publishing etkin kılınmıştır. Mesajların içeriğinin daha detaylı bir şekilde analiz edilebilmesi amacıyla Tracing ve Message Logging özellikleri aktif hale getirilmiştir. Bu ayarlar elbetteki Microsoft Service Configuration Editor yardımıyla belirlenmek istenirse aşağıdaki adımlar takip edilebilir.

Öncelikli olarak Enable MessageLoging ve Enable Tracing tıklanarak aktif hale getirilir.

![mk232_4.gif](/assets/images/2007/mk232_4.gif)

Sonrasında ise Diagnostics altındaki Message Logging klasörüne tıklandıktan sonra özellikler penceresinden LogEntireMessage değeri true olarak ayarlanmalıdır.

![mk232_5.gif](/assets/images/2007/mk232_5.gif)

Sources klasörü altında yer alan System.ServiceModel.MessageLogging ve System.ServiceModel elementlerinin Trace Level özelliklerine Verbose değeri atanmalıdır. Böylece mesaj aktivitelerin en ince ayrıntısına kadar izlenmesi olanaklı hale gelecektir.

![mk232_6.gif](/assets/images/2007/mk232_6.gif)

![mk232_7.gif](/assets/images/2007/mk232_7.gif)

Gelelim istemci tarafına. İstemci program basit bir Windows uygulaması olarak tasarlanabilir. Programın arayüzü ve kodlarının içeriği aşağıdaki gibidir.

![mk232_8.gif](/assets/images/2007/mk232_8.gif)

(Servis tarafına ait Proxy sınıfının oluşturulması için Add Service Reference seçeneği kullanılmaldırı.)

Add Service Reference sonrası otomatik olarak bir App.config dosyası oluşturulacak ve içerisine başlangıç değerleri atanacaktır.

Windows uygulamasına ait kodlar ise aşağıdaki gibidir.

```csharp
using System;
using System.IO;
using System.Text;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using Istemci.AdventureService;
using System.Collections.Generic;

namespace Istemci
{
    public partial class Form1 : Form
    {
        PhotoServiceClient client = null;

        public Form1()
        {
            InitializeComponent();
            client = new PhotoServiceClient();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Servis tarafındaki GetProducts metodu ile ProductInfo tipinden dizi elde edilir ve her bir elemanı listBox1 isimli ListBox kontrolüne eklenir.
            ProductInfo[] infos=client.GetProducts();
            foreach (ProductInfo info in infos)
                listBox1.Items.Add(info);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            short photoId = 0;
            // Seçilen ListBox elemanı ProductInfo tipine dönüştürüldükten sonra ProductPhotoId özelliğinin değeri elde edilir.
            Int16.TryParse(((ProductInfo)listBox1.SelectedItem).ProductPhotoId.ToString(), out photoId);
            // GetPhotoByProductId metoduna photoId değeri aktarılarak byte[] dizisi elde edilir ve bir MemoryStream nesnesi örneklenir.
            MemoryStream stream=new MemoryStream(client.GetPhotoByProductId(photoId));
            // MemoryStream örneğinden yararlanılarak Image nesnesi oluşturulur ve PictureBox kontrolünün Image özelliğine atanır.
            pictureBox1.Image=Image.FromStream(stream);
        }
    }
}
```

Burada dikkat edilmesi gereken birkaç nokta vardır. GetProducts metodu geriye List tipinden bir generic koleksiyon yerine ProductInfo tipinden bir dizi döndürmektedir. Diğer taraftan istemci bir Windows uygulaması olduğundan ComboBox kontrolü içerisine dizi doğrudan bağlanamaz. Bir başka deyişle DataSource özelliğini burada kullanmak yeterli olmayacaktır. Bu nedenle ProductInfo tipinden dizi içerisindeki elemanların tek tek dolaşılması ve öğe olarak eklenmesi gerekmektedir. Ancak bu durumdada ListBox içerisinde ürün adlarının görülebilmesi için ToString metodunun ezilmiş olması şarttır. Bilindiği gibi servis tarafındaki geliştirici tanımlı tiplere ait metodlar istemci tarafında serileştirilmemektedir. O nedenle, ToString metodunun ezilmiş hali sadece bu örnekte yardımcı olması açısından istemci tarafındaki uygulamada bilinçli olarak aşağıdaki gibi ezilmektedir. Bir gerçek hayat senaryosunda bu çok kontrol edilebilir bir durum olamayabilir. Nitekim güncelleştirme ve ölçeklendirme zorlaşmaktadır.

![mk232_9.gif](/assets/images/2007/mk232_9.gif)

Bu değişikliğin arkasından ListBox üzerinden SelectedItem özelliği ile elde edilen Object referansı ProductInfo tipine dönüştürülüp ProductPhotoId özelliğinin değeri elde edilebilir. Dolayısıyla örnek çalıştırıldığında aşağıdaki ekran görüntüsündekine benzer sonuçlar elde edilecektir.

![mk232_10.gif](/assets/images/2007/mk232_10.gif)

Gelelim asıl konumuza. Bakalım log dosyalarında ne gibi sonuçlara varacağız. Öncelikli olarak Service Trace Viewer yardımıyla webtracelog.svc dosyasının açılması gerekmektedir. Bu ana kadar yapılanların tek amacı söz konusu dosya içerisinde, istemciye gönderilen mesaj formatının text tabanlı olduğunun ispat edilmesidir. İlk olarak Activity kısmından Process action "http://www.bsenyurt.com/ProductPhotoService/ Photox0020Service/GetPhotoByProductId bölümü seçilir. Bu işlemin ardından A message was written Description bölümüne bakılırsa Encoder isimli özelliğin değerinin aşağıdaki ekran görüntüsünde olduğu gibi text/xml; charset=utf-8 olduğu görülür.

![mk232_11.gif](/assets/images/2007/mk232_11.gif)

Bu aslında istemciye döndürülen byte[] dizisinin text tabanlı olacak şekilde XML içeriğine kodlandığının bir göstergesidir. Servis tarafından MTOM'a uygun olacak şekilde mesaj döndürmek için tek yapılması gereken BasicHttpBinding bağlayıcı tipinin MessageEncoding özelliğinin değerini MTOM olarak değiştirmektir. Bunun için servis tarafında yeni bir Binding Configuration elementinin BasicHttpBinding tipi için aşağıdaki ekran görüntüsünde olduğu gibi eklenmesi gerekmektedir.

![mk232_13.gif](/assets/images/2007/mk232_13.gif)

Sonrasında bağlayıcı tipin (Binding Type) bu konfigurasyon ile eşleştirilmesi yeterli olacaktır. Bunun için ProductPhotoHttpEndPoint özelliklerinden BindingConfiguration elementinin değerinin aşağıdaki ekran görüntüsünde olduğu gibi Microsoft Service Configuration Editor üzerinden ProductPhotoBindingConfiguration olarak set edilmesi yeterlidir.

![dikkat.gif](/assets/images/2007/dikkat.gif)
Elbetteki servis tarafında yer alan konfigurasyon dosyasında da MessageEncoding özelliğinin değerinin true olarak set edilmesi gerekir. Bu yapılmadığı takdirde çalışma zamanında (Run Time) ProtocolException tipinden bir istisna (Exception) alınabilir.

![mk232_14.gif](/assets/images/2007/mk232_14.gif)

Bu işlemin ardından istemci yeniden çalıştırılır ve Service Trace Viewer kullanılarak GetProductPhotoId'den geriye döndürülen mesaj özelliklerine bakılırsa aşağıdaki sonucun ortaya çıktığı görülecektir.

![mk232_15.gif](/assets/images/2007/mk232_15.gif)

Uygulama test edilirken dikkat çekici noktalardan biriside bazı resim dosyaları açılırken aşağıdaki gibi bir çalışma zamanı hata mesajı alınmasıdır.

![mk232_16.gif](/assets/images/2007/mk232_16.gif)

Bu hatanın sebebi aktarılmak istenen veri boyutunun varsayılan MaxArrayLength değerinden büyük olmasıdır. Bu sorunu çözmek için bağlayıcı tipin konfigurasyon ayarlarında yer alan MaxArrayLength değerini değiştirmek yeterlidir. Bu özelliğin varsayılan değeri 16384 byte'dır. Örnekte bu değer sembolik olarak 65536 yapılmaktadır. Elbetteki bu özelliğin değerinin hem istemci hemde servis tarafında yapılması şarttır. Söz konusu özellik aşağıdaki ekran görüntüsünde olduğu gibi Microsoft Service Configuration Editor yardımıyla değiştirilebilir.

![mk232_17.gif](/assets/images/2007/mk232_17.gif)

Bu işlemin ardından örnek uygulama çalıştırılırsa sonuçların başarılı bir şekilde alındığı görülebilir. Elbetteki boyutun dahada artması yine aynı istisnanın (Exception) alınmasına neden olacaktır.

MTOM standardına uygun olarak hazırlanan mesajlar sayesinde büyük boyutlu ikili (Binary) verilerin gereksiz yere kodlanmadan aktarılması ve çözülmeden alınması mümkün olabilmektedir. Yinede veri boyutunun çok fazla olması halinde MTOM tabanlı mesajların hazırlanmasıda zaman kaybına neden olacaktır. Nitekim söz konusu büyük veri dosyasının tek seferde istemciye (veya servise) doğru aktarılması söz konusudur. Ayrıca bu büyük verinin alınması sırasında timeout'lar oluşabilir. Peki çözüm olarak neler yapılabilir? Eğer protokol uygunsa söz konusu büyük verilerin istemci ve servis arasında bir Stream üzerinden taşınması tercih edilmelidir. Böylece söz konusu performans kayıplarıda ortadan kalkacaktır. Nitekim mesajı tek seferde hazırlanıp tamamının gönderilmesi yerine iki taraf arasında açılan hat üzerinden akması sağlanabilmektedir.

WCF (Windows Communication Foundation) içerisinde yer alan basicHttpBinding, netTcpBinding, netNamedPipeBinding tipleri Stream aktarımını desteklemektedir. Tahmin edileceği üzere diğer bağlayıcı tiplerin buna tam desteği olmadığından özel bağlayıcı tipleri (Custom Binding Types) kullanmak gerekmektedir. Stream kullanılması için tek yapılması gereken TransferMode özelliğinin değerini aşağıdaki tabloda yer alan değerlerden birisine set etmektir. (Bağlayıcı tipin TransferMode özelliği, TransferMode enum sabiti tipinden değerler alabilmektedir.)

TransferMode Değeri
Açıklama

Streamed
Serivise gelen ve servisten çıkan mesajlar stream üzerinden hareket ederler.

StreamedRequest
Sadece servis tarafına gelen taleplere (Request) ait mesajların stream üzerinden hareket etmesine izin verilir. Bu mod seçildiğinde servis tarafındaki metodun parametresinin tek ve Stream sınıfından türeyen bir şekilde tasarlanmış olması şarttır.

StreamedResponse
Sadece servis tarafından istemciye geri dönen cevaplara (Response) ait mesajların stream üzerinden hareket etmesine izin verilir. Bu mod seçildiğinde servis tarafında yer alan operasyonun geriye Stream sınıfından türemiş bir tip döndürmesi veya bunu out anahtar sözcüğü ile metod parametrelerinde yapması şarttır. (Burada Stream yerine IXmlSerializable arayüzünü-interface- uyarlamış bir tipin döndürülmeside sağlanabilir.)

Buffered
Mesajlar stream üzerinden hareket etmezler. Hangi taraf olursa olsun, ikili (binary) içeriğin tamamı gönderilmeye hazır hale geldiğinde karşı tarafa aktarılırlar. Karşı tarafa ulaştığında ise tamamı tampona alındıktan sonra uygulamaya işlenmek üzere aktarılırlar. TransferMode özelliğinin varsayılan (Default) değeri Buffered olarak belirlenmiştir.

Her ne kadar stream üzerinden mesaj göndermek avantajlı gözüksede dikkat edilmesi gereken bazı hususlarda vardır. Herşeyden önce Stream kullanılması halinde mesaj seviyesinde güvenlik (Message Level Security) tesis edilememektedir. Bu nedenle iletişim seviyesinde güvenlik (Transport Level Security) kullanılmalıdır. Örneğin HTTPS kullanımı HTTP üzerinden kullanılacak Stream'ler için geçerlidir. Bunların dışında Stream kullanımı içinde bir mesaj alma sınırı vardır. Biraz önceki örnekte olduğu gibi belirlenen boyutun dışına çıkıldığında çalışma zamanı istisnaları alınabilir. Ancak en önemli kısıtlardan birisi güvenilir oturum (Reliable Session) açmanın mümkün olmayışıdır. Nitekim güvenilir oturumlar mesajların tamponlanması ve sıralanması ilkelerine göre çalışmaktadır.

Geliştirilen örnekte Stream kullanımı için tek yapılması gereken servis ve istemci tarafındaki konfigurasyon dosyalarında, bağlayıcı tip ile ilişkili TransferMode özelliğinin değerini aşağıdaki gibi değiştirmektir.(Bu ayarı istemci tarafı içinde yapmak gerekmektedir.)

![mk232_18.gif](/assets/images/2007/mk232_18.gif)

Buraya kadar anlatılanlar göz önüne alındığında MTOM (Message Transmision Optimization Mechanism) sayesinde mesajların içeriklerinin encoding/decoding işlemlerine tabi tutulmadan karşı tarafa aktarılabilmesi sağlanmaktadır. Bu aynı zamanda WCF (Windows Communication Foundation) dışındaki platformlar ile haberleşmedede (Interoperability Desteği) önemli bir yere sahiptir. Diğer taraftan MTOM'un bazı dezavantajlarını ortadan kaldırmak adına Stream kullanımı tercih edilebilir. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](/assets/files/2007/MTOMKullanimi.rar)