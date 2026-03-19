---
title: "WCF - Client Callback (İstemci Taraflı Geri Bildirim)"
pubDate: 2007-08-23 12:00:00
categories:
  - WCF
tags:
  - WCF
  - C#
  - XML
  - HTTP
  - IIS
  - Delegates
  - Generics
---

# WCF - Client Callback (İstemci Taraflı Geri Bildirim)
Değerli Okurlarım Merhabalar,

Servis yönelimi mimari (Service Oriented Architecture) üzerine geliştirilen sistemler istemci/sunucu (client/server) tabanlı bir iletişimi olanaklı kılarlar. Bu sistemlerde süreçler çoğunlukla istemciden sunucuya doğru yapılan operasyon talepleri (Request) ve servis tarafından istemciye geri dönen cevaplardan (Response) ibarettir. Oysaki bazı SOA vakkalarında rollerin tam tersine çevrilmesi gerekebilir. Bir başka deyişle servislerin yeri geldiğinde bir istemci gibi hareket etmesi istenebilir. Söz gelimi bir stok sisteminde yer alan bir servis parçasında, istemcilerin stok üzerinde servis yardımıyla yapacağı hareketleri göz önüne alalım.

Bu stok hareketleri istemciden gelecek olan operasyon çağrıları sonucu servis tarafında ele alınıyor olsunlar. Buna göre, servise bağlı diğer istemcilerinde durumdan haberdar olması istenebilir. Bu bir uyarı sistemi olarakda göz önüne alınabilir. Böyle bir sistemde öncelikli olarak servis tarafındaki uygulamanın istemci tarafında yer alan belirli operasyonları çağırabiliyor olması gerekmektedir. Diğer taraftan, servis uygulaması kendisine bağlı istemcilerin hepsinde bir yayınlama yapmak istiyorsa, olay (event) yönelimli bir model geliştirilmesi söz konusu olmalıdır. Bu makalemizde Windows Communication Foundation sisteminde istemci taraflı geri bildirimlerin (Client Callback) nasıl yapılabileceğini çok basit olarak incelemeye çalışacağız.

> WCF (Windows Communication Foundation), standart istemci sunucu modeli dışında rollerin tersine dönebileceği iki modeli daha destekler. Bunlar peer-to-peer modeli ve client callback modelidir. Peer-to-peer modeline göre tüm istemciler aktif ve bağımsız olarak birer servistir ve birbirleriyle mesajlaşabilir. Client Callback modelinde ise servisler, istemci tarafında metod çağrıları gerçekleştirebilecek durumdadır.

İlk hedef, servis uygulamasının istemci tarafındaki bir metodu çağırabilmesini sağlamak olmalıdır. WCF tarafından bakıldığında, bağlayıcı tiplerin (binding types) tamamının geri bildirim işlemlerini desteklemediği görülür. Bunun sebebi kullanılan protokoldür. TCP ve IPC gibi protokoller istemci geri bildirim işlemlerinde doğaları gereği doğrudan destek vermektedir. Bu sebepten NetTcpBinding yada NetNamedPipeBinding gibi bağlayıcı tipler istemci geri bildirim işlemlerini doğal olarak desteklemektedirler. Ancak HTTP protokolü bağlantısız (connectless) olarak çalışan bir model olduğundan geri bildirimi (Callback) doğrudan desteklemez.

Bu nedenle BasicHttpBinding veya WsHttpBinding gibi bağlayıcı tipleri ile Client Callback modeli geliştirilemez. Ancak HTTP protokolününde bu sistemi çift yönlü kanallar açarak gerçekleştirmek mümkündür. Tahmin edileceği gibi bu kanallardan birisi servisden istemciye doğru, diğeri ise ters yönde istemciden servise doğru olmalıdır. Bunu, tek yönlü iki HTTP kanalı olarakta düşünebiliriz. WCF içerisinde yer alan bağlayıcı tiplerinde WsDualHttpBinding tam olarak bu amaçla tasarlanmıştır. Bu nedenle WsDualHttpBinding bağlayıcı tipini kullanarak, HTTP protokolünü baz alacak şekilde Client Callback destekli uygulamalar geliştirilebilir.

İlk olarak bir servisin istemci tarafındaki bir operasyonu nasıl çağırabileceğini örnek bir uygulama üzerinden incelemeye çalışalım. Başlamadan önce servis tarafında ve istemci tarafında yapmamız gerekenlerin neler olduğunu vurgulamakta yarar var. Servis tarafında geri bildirim için bir arayüz (Interface) tanımlanmalıdır. Bu arayüz istemci tarafında, herhangibir sınıfa (Class) uygulanmalıdır. İstemciler, kullanmak istedikleri servise ait bir örnek oluşturduklarında, servis tarafına bir referans verilmelidir. Bu referansı kullanarak servisler, istemciler üzerindeki metodları tetikleyebilirler. Dolayısıyla istemci tarafında geri bildirim tipinin uyguladığı bir arayüzün var olması ve bunun servis tarafında tanımlanmış olması gerekir.

Bu cümleler biraz kafa karıştırıcı olabilir. Bu durum daha kısa bir şekilde; istemcideki bir nesne referansının servis tarafından elde edilebilmesi ve onun sayesinde istemci üzerinde, söz konusu arayüzü uygulayan bir sınıfın metodunun servis tarafından çağırılabilmesi şeklinde de ifade edilebilir. Öncelikle servis sözleşmesini (Service Contract), geri bildirim sözleşmesini (Callback Contract) ve iş yapan servis tipini barındıracak WCF Class Library projesini geliştirerek örneğimize başlayalım. WCF sınıf kütüphanemizdeki tiplerin sınıf çizelgesindeki durumu ve kodları aşağıdaki gibidir.

![mk219_2.gif](images/mk219_2.gif)

IGeriBildirimSozlesmesi isimli arayüz (interface) geri bildirim sözleşmesinin (Callback Contract) tanımını yapmaktadır. Bu tanıma göre, servisin istemciler üzerinden tetikleyebileceği metod, int tipinden parametre alan ve geriye değer döndürmeyen bir şekilde tanımlanmıştır. Geri bildirim sözleşmesinin içeriği ise aşağıdaki gibidir.

```csharp
using System;
using System.ServiceModel;

namespace UrunServisi
{
    // Callback Contract tanımlanırken, ServiceContract niteliği kullanılmaz. 
    public interface IGeriBildirimSozlesmesi
    {
        // Servisin istemciden çağıracağı geri bildirim metodundan geriye cevap beklenmesine gerek olmadığından IsOneWay özelliğine true değeri atanmıştır.
        [OperationContract(IsOneWay = true)]
        void OnStokMiktariDegisti(int degistirilenUrunId);
    }
}
```

Tanımlanan bu sözleşmede dikkat edilmesi gereken en önemli noktalardan biriside ServiceContract niteliğinin uygulanmamış olmasıdır. Bir geri bildirim sözleşmesinde, OperationContract niteliği ile, servisin istemciler üzerinden çağırabileceği operasyonların bildirilmesi yeterlidir.

Servis sözleşmesi IUrunYoneticiSozlesmesi isimli arayüz (interface) ile tanımlanmaktadır. Bu arayüzün kodları ise aşağıdaki gibidir.

```csharp
using System;
using System.ServiceModel;

namespace UrunServisi
{
    // Servis sözleşmesinde, Callback sözleşmesini bildirmek için CallbackContract özelliği kullanılır.
    [ServiceContract(CallbackContract=typeof(IGeriBildirimSozlesmesi))]
    public interface IUrunYoneticiSozlesmesi
    {
        [OperationContract()]
        void StokMiktariniArttir(int gelenMiktar, int urunNo);
    }
}
```

Burada dikkat edilmesi gereken nokta, ServiceContract niteliğinde CallbackContract tanımlamasının yapılmış olmasıdır. Bu sayede, söz konusu arayüzü uygulayan servis sınıfının, geri bildirim işlemleri sırasında hangi arayüz ile taşınan referansları ele alacağı açık bir şekilde bildirilmiş olunur. CallbackContract özelliği, type tipinden bir değer aldığı için typeof operatörüne başvurulmuştur ve geri bildirim sözleşmesi olan IGeriBildirimSozlesmesi parametre olarak verilmiştir. Servis sözleşmesini uygulayan sınıfa ait kodlar ise aşağıdaki gibidir.

```csharp
using System;
using System.Data;
using System.ServiceModel;
using System.Data.SqlClient;

namespace UrunServisi
{
    public class UrunYonetici:IUrunYoneticiSozlesmesi
    {
        #region IUrunYoneticiSozlesmesi Members

        public void StokMiktariniArttir(int gelenMiktar,int urunNo)
        {
            string sorgu = "Update Products Set UnitsInStock=UnitsInStock+@GelenMiktar Where ProductID=@PrdId";
            using (SqlConnection conn = new SqlConnection("data source=.;database=Northwind;integrated security=SSPI"))
            {
                SqlCommand cmd = new SqlCommand(sorgu, conn);
                cmd.Parameters.AddWithValue("@GelenMiktar", gelenMiktar);
                cmd.Parameters.AddWithValue("@PrdId", urunNo);
                conn.Open();
                int guncellenen=Convert.ToInt32(cmd.ExecuteNonQuery());
                if (guncellenen > 0)
                {
                         IGeriBildirimSozlesmesi geriBildirim = OperationContext.Current.GetCallbackChannel<IGeriBildirimSozlesmesi>();
                    if (((ICommunicationObject)geriBildirim).State == CommunicationState.Opened)
                    {
                        geriBildirim.OnStokMiktariDegisti(urunNo);
                    }
                }
            }
        }
    
        #endregion
    }
}
```

UrunYonetici isimli sınıf içerisinde StokMiktariniArttir isimli bir metod yer almaktadır. Bu metod sembolik olarak, gelen ürün numarasına göre Northwind veritabanındaki Products tablosunda yer alan UnitsInStock alanının değerini gelenMiktar parametresinin değeri kadar arttırmaktadır. İşte burada servisin, istemci tarafında stok miktarının arttırıldığına dair bir geri bildirimde bulunması sağlanmaktadır. Bunun için, istemcinin servis tarafında gönderdiği içerik örneğinin (InstanceContext) ele alınması gerekmektedir. İstemciler, servise talepte bulunduklarında WCF çalışma ortamı (WCF Runtime), o anki kapsama ait içerik örneğinin referansınıda mesaj ile birlikte gönderecektir.

OperationContext sınıfının Current özelliği üzerinden çağırılan GetCallbackChannel generic metodu ile, IGeriBildirimSozlesmesi tarafından taşınabilecek bir istemci referansı elde edilir. İşte bu referans, istemci tarafında geri bildirim sözleşmesini uyarlayan sınıfa aittir. Arayüzlerin polimorfik (Polymorphic) yapısı nedeni ile, OnStokMiktariDegisti metoduna yapılacak olan çağrı, aslında istemci tarafındaki metoda yapılmaktadır. Özetle servis, istemci tarafındaki bir geri bildirim metodunu tetiklemektedir. İstemci uygulama, servis ile olan bağlantısını herhangibir anda kesebilir. Özellikle OneWay olarak işaretlenmiş bir callback operasyonunda bu sorun oluşturur.

Bu nedenle, geri bildirim kanalının açık olduğundan emin olup geri bildirim metodunu çağırmak amacıyla, ICommunicationObject arayüzüne dönüştürme (cast) işlemi yapılarak State özelliğinin değerine bakılmaktadır. State özelliği, CommunicationState enum sabiti tipinden bir değer almaktadır ve örnekte Opened olması halinde geri bildirim çağrısının yapılması sağlanmaktadır. Artık servis sözleşmesini yayınlayacak Host uygulamanın yazılmasına geçilebilir. Host uygulama, basit bir Windows uygulaması olarak tasarlanabilir. Bu uygulamanın ekran görüntüsü basit olarak aşağıdaki gibidir.

![mk219_3.gif](images/mk219_3.gif)

Şekildende tahmin edileceği üzere, program kullanıcısı servisi açma ve kapatma işlemlerini manuel olarak yapmaktadır. Sunucu uygulamanın kodları aşağıdaki gibi geliştirilebilir.

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.ServiceModel;
using UrunServisi;

namespace Sunucu
{
    public partial class Form1 : Form
    {
        ServiceHost host; 

        public Form1()
        {
            InitializeComponent();
            btnServisiKapat.Enabled = false; 
        }

        private void ServisiHazirla()
        {
            host = new ServiceHost(typeof(UrunYonetici));
            host.Opening += delegate(object sender, EventArgs e)
                                {
                                    lblServisDurumu.Text = "Servis Açılıyor...";
                                };
    
            host.Opened += delegate(object sender, EventArgs e)
                                {
                                    lblServisDurumu.Text = "Servis Açıldı";
                                    btnServisiAc.Enabled = false;
                                    btnServisiKapat.Enabled = true;
                                };
            host.Closing += delegate(object sender, EventArgs e)
                                {
                                    lblServisDurumu.Text = "Servis Kapatılıyor...";
                                };
            host.Closed += delegate(object sender, EventArgs e)
                                {
                                    lblServisDurumu.Text = "Servis Kapatıldı.";
                                    btnServisiAc.Enabled = true;
                                    btnServisiKapat.Enabled = false;
                                };
        } 

        private void btnServisiAc_Click(object sender, EventArgs e)
        {
            ServisiHazirla();
            host.Open(); 
        }

        private void btnServisiKapat_Click(object sender, EventArgs e)
        {
            host.Close(); 
        }
    }
}
```

Servis uygulamasında, ServisHost örneklemesi yapılmakta ve servisin durumunu daha kolay izleyebilmek açısından gerekli olaylar generic metodlar yardımıyla yüklenmektedir. Böylece program kullanıcıları, servisin açılması ve kapanması durumunu kolay bir şekilde izleyebilecektir. Her zamanki gibi sunucu uygulama, WCF altyapısı için gerekli System.ServiceModel.dll ile servis sözleşmesini içeren UrunServisi.dll isimli sınıf kütüphanesini (WCF Class Library) referans etmektedir. Bunlara ek olarak sunucu uygulamanın WCF için gerekli konfigurasyon bilgileri ise aşağıdaki gibi app.config dosyasında yer almaktadır.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <services>
            <service name="UrunServisi.UrunYonetici">
                <endpoint address="net.tcp://localhost:9001/UrunServisi.svc" binding="netTcpBinding" name="UrunServisiEndPoint" contract="UrunServisi.IUrunYoneticiSozlesmesi" />
            </service>
        </services>
    </system.serviceModel>
</configuration>
```

Bu örnekte NetTcpBinding bağlayıcı tipi kullanılmaktadır. İstemci tarafını yazmadan önce, gerekli proxy sınıfının üretilmesi gerekmektedir. Bu noktada svcutil.exe aracından aşağıdaki gibi yararlanılabilir.

![mk219_1.gif](images/mk219_1.gif)

Servis tarafında geri bildirim sözleşmesi var olduğundan, üretilen proxy sınıfı diğer üretimlere göre biraz daha farklıdır. Bu farkı görmek için proxy sınıfı ve beraberinde üretilen tiplerin sınıf diagramına (class diagram) bakmak yeterlidir.

![mk219_4.gif](images/mk219_4.gif)

Herşeyden önce, istemci uygulamada, servis tarafından çağırılabilecek bir geri bildirim opreasyonun söz konusu olabilmesi için, proxy sınıfının generic DuplexClientBase özet sınıfından (abstract class) türemiş olması gerekmektedir. Diğer taraftan svcutil.exe bu işlemi otomatik olarak yapmaktadır. Örnekte dikkat edilirse, geri bildirim sözleşmesi tipi, DuplexClientBase sınıfının generic parametresi olarak kullanılmaktadır. Bunun dışında, proxy sınıfının aşırı yüklemiş (overload) yapıcı metodlarının (constructor) tamamında ilk parametreler InstanceContext sınıfı tipindendir. Bu son derece doğaldır. Nitekim, servisin istemci üzerindeki bir operasyonu çağırabilmesi için, istemcinin kendisine verdiği içerik referansını kullanması gerekmektedir. Bu nedenle yapıcı metodun ilk parametresi bu referansı istemciden servis tarafına göndermek için kullanılır.

Şimdi gelelim istemci geri bildirim sözleşmesini uyarlayan sınıfa. Bu sınıf geliştirici tarafından yazılmalıdır. Örnekte bu amaçla Uygulayici isimli bir sınıf kullanılmıştır.

![mk219_5.gif](images/mk219_5.gif)

Uygulayici isimli sınıfın en büyük özelliği IUrunYoneticiSozlesmesiCallback isimli arayüzü (interface) uyguluyor olmasıdır. Burada, arayüz adının sonundaki Callback kelimesi dikkat çekicidir. Svcutil.exe aracı bunu otomatik olarak eklemektedir. Bunun dışında Uygulayici sınıfı, IDisposable arayüzünüde uygulamaktadır ki burada amaç proxy örneğinin yok edilmesi sırasında servis ile olan bağlantıyı kapatmaya zorlamaktır. Uygulamaci sınıfına ait kodlar aşağıdaki gibidir.

```csharp
using System;
using System.ServiceModel;

namespace Istemci
{
    /* Servisin, istemci tarafında bir geri bildirim operasyonu çağrısı yapabilmesi için geri bildirim sözleşmesini uygulayan bir sınıfın yazılması gerekir. */
    public class Uygulayici:IUrunYoneticiSozlesmesiCallback,IDisposable
    {
        private UrunYoneticiSozlesmesiClient proxy=null;

        public Uygulayici()
        {           
            proxy = new UrunYoneticiSozlesmesiClient(new InstanceContext(this), "DefaultBinding_IUrunYoneticiSozlesmesi_IUrunYoneticiSozlesmesi"); 
        }

        public void StokMiktariniArttiralim(int artisMiktari,int urunNumarasi)
        {
            proxy.StokMiktariniArttir(artisMiktari, urunNumarasi);
            Console.WriteLine("{0} numaralı ürünün stok miktarında {1} adet artış yapıldı",urunNumarasi,artisMiktari); 
        }
        #region IUrunYoneticiSozlesmesiCallback Members

        public void OnStokMiktariDegisti(int degisenUrunId)
        {
            Console.WriteLine("{0} numaralı ürünün stok miktarında değişiklik oldu.",degisenUrunId.ToString());
        }

        public void Cikart()
        {
             proxy.AboneligiKaldir(); /* Abonelik çıkartma işlemi yapılmadığ takdirde istemci uygulama istisna mesajları alabilir. Bunun için istemcide açılan bir abonelik var ise bunun bilinçli olarak kapatılmasındada yarar vardır. Cikart metodu buna destek vermesi amacıyla geliştirilmiştir. */
        }

        #endregion

        #region IDisposable Members
    
        public void Dispose()
        {
            proxy.Close();
        }

        #endregion
    }
}
```

Sınıfın yapıcı metodu içerisinde proxy sınıfına ait bir nesne örneklenmektedir. Servisin kendisine bağlı istemci üzerinde bir geri bildirim fonksiyonu çağırabilmesi için geri bildirim arayüzünü uygulayan tipe ait referansın, proxy oluşturulurken servise bildirilmesi gerekir. Bunun için proxy sınıfına ait yapıcıların ilk parametreleri daima InstanceContext tipinden bir referans alır. Servis tarafında bu içerik örneğinden (InstanceContext) yararlanılarak metod çağrısı yapıldığında, WCF çalışma zamanı bu sınıf içerisindeki ilgili metodu çağıracaktır ki bu örnekte söz konusu metod OnStokMiktariDegisti dır.

Sınıf aynı zamanda Dispose metodu içerisinde proxy sınıfının Close metodunuda çağırmaktadır. Bunun sebebi şudur; İstemci servis tarafının callback çağrısını beklemeden proxy'yi kapatırsa servis tarafında hatalar oluşabilir. Bu nedenle Dispose metodu içerisinden Close metodu çağırılır. Artık istemci tarafında Console uygulamasına ait Main metodunun içeriğini aşağıdaki gibi geliştirebiliriz.

```csharp
using System;
using System.ServiceModel;

namespace Istemci
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("İşlemlere başlamak için tıklayın");
            Console.ReadLine();

            Uygulayici uyg = new Uygulayici();
            uyg.StokMiktariniArttiralim(10, 1);

            Console.ReadLine();
        }
    }
}
```

Artık testlere başlanabilir. Önce sunucu uygulamanın çalıştırılması ve servisin açılması gerekmektedir. Ardından istemci uygulama çalıştırılarak devam edilirse aşağıdakine benzer ekran görüntüleri ile karşılaşılır.

![mk219_6.gif](images/mk219_6.gif)

Burada görüldüğü gibi, istemci tarafında servis üzerindeki StokMiktariniArttir metodu çağırldıktan sonra, serviste istemci tarafındaki OnStokMiktariDegisti metodunu çalıştırmıştır.

Buraya kadar geliştirilen örnek sadece servis tarafından istemcideki bir metodun client callback modeli ile nasıl çalıştırabileceğidir. Oysaki gerçek hayat senaryolarında, bir istemcinin aksiyonu sonrası diğer tüm istemcilerinde servis tarafından haberdar edilmesi istenebilir. Bu durumda servisin kendisine bağlı olan istemcileri bilmesi, bunların referanslarını tutması gerekmektedir. Ayrıca istemciler diğer istemcilerin aksiyonlarından servis yoluyla haberdar edilip edilmeyeceklerinide belirleyebilmelidirler. Kısacası, istemcinin kendisini servis tarafına istediği zaman abone edebilmesi (Subscribe) ve istediği zamanda abonelikten çıkartabilmesi (Unsubscribe) gerekmektedir. Aşağıdaki şekiller ile durum daha net anlaşılabilir.

İlk olarak istemcilere ait örneklerin servis tarafına bildirilmesi bir başka deyişle abone edilmesi söz konusudur.

![mk219_7.gif](images/mk219_7.gif)

İstemciler servis üzerinde operayon çağrıları gerçekleştirirler. Bu çağrılardan haberdar olmak isteyenler servise abone olurken istemeyenler abonelikten çıkarlar veya hiç abone olmazlar. Servis ise kendisine abone olanlar üzerinden geri bildirim metodlarını çağırabilir ve böylece bilgilendirme yapabilir. Örneğin aşağıdaki senaryoya göre, daha önce abone olmuş olan İstemci A abonelikten çıktıktan sonra, istemci D'nin yapacağı Stok Miktarı Değiştirme işleminden, İstemci B, İstemci C ve İstemci D'nin kendisi haberdar olabilir.

![mk219_8.gif](images/mk219_8.gif)

Şimdi bu işlemi adım adım yapmaya çalışalım. İlk olarak servis sözleşmesine, istemcilerin abone olabilmek ve abonelikten çıkabilmek için çağırabilecekleri iki metod tanımı eklenmesi gerekmektedir. Bunun için IUrunYoneticiSozlesmesi arayüzüne (interface) aşağıdaki eklemelerin yapılması yeterlidir.

```csharp
using System;
using System.ServiceModel;

namespace UrunServisi
{
    [ServiceContract(CallbackContract=typeof(IGeriBildirimSozlesmesi))]
    public interface IUrunYoneticiSozlesmesi
    {
        [OperationContract()]
        void StokMiktariniArttir(int gelenMiktar, int urunNo);

        [OperationContract()]
        void AboneOl();
    
        [OperationContract()]
        void AboneligiKaldir();
    }
}
```

Bu değişiklikler nedeniyle, servis sözleşmesini uygulayan UrunYonetici sınıfı içerisindede AboneOl ve AboneligiKaldir metodlarının yazılması gerekmektedir. İstemcilerin geri bildirim referanslarının servis tarafında saklanabilmesi için generic bir liste koleksiyonu (List) kullanılabilir. AboneOl metodu ile bu koleksiyona istemcinin geri bildirim referansı eklenirken, AboneligiKaldir metodu ilede istemciden gelen geri bildirim referansının ilgili koleksiyondan kaldırılması sağlanır. İstemcilere yayınlama yapacak olan ayrı bir metod ise, istenildiğinde servis tarafından koleksiyonda tutulan tüm istemci geri bildirim referanslarına ait metodların çağırılması görevini üstlenir. Bu bilgiler ışığında UrunYonetici sınıfının aşağıdaki gibi dizayn edilmesi yeterlidir.

```csharp
using System;
using System.Data;
using System.ServiceModel;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace UrunServisi
{
    public class UrunYonetici:IUrunYoneticiSozlesmesi
    {
        // İstemcilerin geri bildirim referanslarını tutacak olan List<T> koleksiyonu.
        private static List<IGeriBildirimSozlesmesi> geriBildirimAboneleri = new List<IGeriBildirimSozlesmesi>();

        #region IUrunYoneticiSozlesmesi Members

        // İstemcilerin servis tarafındaki geri bildirim listesine abone olmasını sağlayan metod.
        public void AboneOl()
        {
            // Servise o anda bağlı olan istemcideki IGeriBildirimSozlesmesi arayüzünü uygulamış sınıf referansı alınır.
            IGeriBildirimSozlesmesi geriBildirim = OperationContext.Current.GetCallbackChannel<IGeriBildirimSozlesmesi>();
            // Eğer List<> kolekisyonunda gelen istemci geri bildirim referansı yoksa eklenir.
            if (!geriBildirimAboneleri.Contains(geriBildirim))
                 geriBildirimAboneleri.Add(geriBildirim);
        }

        // İstemcilerin, servis tarafındaki geri bildirim abonleri listesinden çıkartılmasını sağlayan metod.
        public void AboneligiKaldir()
        {
            IGeriBildirimSozlesmesi geriBildirim = OperationContext.Current.GetCallbackChannel<IGeriBildirimSozlesmesi>();
            // İstemci geri bildirim referansı, List<> koleksiyonundan çıkartılır.
            geriBildirimAboneleri.Remove(geriBildirim);
        }

        private void AboneleriBilgilendir(int urunNumarasi)
        {
            /* List<> koleksiyonundaki tüm geri bildirim referansları dolaşılır ve sahibi olan istemci uygulama halen daha çalışıyorsa (ki bu durum State ile tespit edilmektedir), istemci üzerindeki geri bildirim operasyonu çağırılır. */
            foreach (IGeriBildirimSozlesmesi geriBildirim in geriBildirimAboneleri)
            {
                if (((ICommunicationObject)geriBildirim).State == CommunicationState.Opened)
                    geriBildirim.OnStokMiktariDegisti(urunNumarasi);
                else // Eğer istemci canlı değilse, servis tarafındaki List<> koleksiyonundan da çıkartılır.
                    geriBildirimAboneleri.Remove(geriBildirim);
            }
        }

        public void StokMiktariniArttir(int gelenMiktar,int urunNo)
        {
            string sorgu = "Update Products Set UnitsInStock=UnitsInStock+@GelenMiktar Where ProductID=@PrdId";
            using (SqlConnection conn = new SqlConnection("data source=.;database=Northwind;integrated security=SSPI"))
            {
                SqlCommand cmd = new SqlCommand(sorgu, conn);
                cmd.Parameters.AddWithValue("@GelenMiktar", gelenMiktar);
                cmd.Parameters.AddWithValue("@PrdId", urunNo);
                conn.Open();
                int guncellenen=Convert.ToInt32(cmd.ExecuteNonQuery());
                if (guncellenen > 0)
                {
                    AboneleriBilgilendir(urunNo);                  
                }
            }
        }
            
        #endregion
    }
}
```

Servis tarafındaki bu değişikliklerin ardından, istemci için gerekli proxy sınıfının svcutil.exe aracı yardımıyla yeniden üretilmesi gerekmektedir. Bu üretim işlemi sonrasında istemci tarafına atanan tiplerin son hali aşağıdaki ekran görüntüsündeki gibi olacaktır.

![mk219_9.gif](images/mk219_9.gif)

İstemci tarafında geri bildirim sözleşmesini uygulayan Uygulayici isimli sınıfın yapıcı metodu içerisinde AboneOl metodu çalıştırılmaktadır.

```csharp
using System;
using System.ServiceModel;

namespace Istemci
{
    public class Uygulayici:IUrunYoneticiSozlesmesiCallback,IDisposable
    {
        private UrunYoneticiSozlesmesiClient proxy=null;

        public Uygulayici()
        {
            proxy = new UrunYoneticiSozlesmesiClient(new InstanceContext(this), "DefaultBinding_IUrunYoneticiSozlesmesi_IUrunYoneticiSozlesmesi");
            proxy.AboneOl();
        }
        // Diğer kod satırlar
    }
}
```

Main metodu içerisinde ise aşağıdaki test kodları yazılabilir.

```csharp
static void Main(string[] args)
{
    try
    {
        Console.WriteLine("İşlemlere başlamak için tıklayın");
        Console.ReadLine();

        Uygulayici uyg = new Uygulayici();
        Console.WriteLine("Abonelik başlatıldı.Devam etmek için bir tuşa basın");
        Console.ReadLine();
        uyg.StokMiktariniArttiralim(10, 1);
          uyg.Cikart();
    }
    catch (Exception exp)
    {
        Console.WriteLine(exp.Message);
    }
    Console.ReadLine();
}
```

Uygulamayı bu haliyle test etmeye başlayabiliriz. Servis uygulaması çalıştırıldıktan sonra, bir kaçtane istemcinin çalıştırılması ve bunlardan herhangibirinde ürün stok miktarının değiştirilmesi işleminin yapılması yeterlidir. Yapılan testlerde, bir istemcinin urun stok miktarında yaptığı değişiklik sonrası, diğer istemcilerinde ilgili değişiklikten haberdar edildiği görülmektedir. Aşağıdaki ekran görüntüsünde bu testin sonuçları yer almaktadır.

![mk219_10.gif](images/mk219_10.gif)

Geliştirilen örnek TCP protokolünü kullanan netTcpBinding bağlayıcı tipine ele almaktadır. Oysaki HTTP protokolünde varsayılan olaran client callbak desteği olmadığından yazımızın başında bahsetmiştik. Client Callback desteği için çift yönlü HTTP kanallarına ihtiyaç vardır. Bir başka deyişle tek yönlü iki adet kanal HTTP protokolü üzerinden kullanılarak istenen sistem kurulabilir. WCF içerisinde bu desteği WsDualHttpBinding tipi karşılamaktadır. Yazılan örneği, HTTP destekli hale getirmek için servis ve istemci tarafında yapılması gerekenler vardır. Servis tarafından çift kanallı HTTP desteğini vermesi için WsDualHttpBinding tipini kullanan yeni bir endPoint bildirimi yapılmalıdır. Bu amaçla servis uygulamasındaki App.Config dosyasına aşağıdaki bildirim ile yeni endPoint ilave edilir.

```xml
<endpoint address="http://localhost:4500/UrunServisi/UrunServisi.svc" binding="wsDualHttpBinding" name="UrunServisWsEndPoint" contract="UrunServisi.IUrunYoneticiSozlesmesi" />
```

İstemci tarafındaki konfigurasyon dosyasında da WsDualHttpBinding tipini kullanan bir endPoint bildirimi yapılmalıdır.

```xml
<endpoint address="http://localhost:4500/UrunServisi/UrunServisi.svc" binding="wsDualHttpBinding" contract="IUrunYoneticiSozlesmesi" name="WsDualHttpBinding_IUrunYoneticiSozlesmesi" />
```

Son olarak istemci tarafında yer alan Uygulayici isimli sınıftada bazı önemli değişikliklerin yapılması gerekmektedir. Özellikle proxy sınıfına ait nesneyi örneklediğimiz yerde yapılması gereken değişiklikler aşağıdaki gibidir.

```csharp
public Uygulayici()
{
    //proxy = new UrunYoneticiSozlesmesiClient(new InstanceContext(this), "DefaultBinding_IUrunYoneticiSozlesmesi_IUrunYoneticiSozlesmesi");
    proxy = new UrunYoneticiSozlesmesiClient(new InstanceContext(this), "WsDualHttpBinding_IUrunYoneticiSozlesmesi");
    WSDualHttpBinding baglayici = (WSDualHttpBinding)proxy.Endpoint.Binding;
    baglayici.ClientBaseAddress = new Uri("http://localhost:4500/UrunServisi/" + Guid.NewGuid().ToString());
    proxy.AboneOl();
}
```

İstemciler WsDualHttpBinding bağlayıcı tipini kullandıklarında, iki adet tek yönlü HTTP kanalı açarlar. WCF çalışma zamanı, servis tarafından gelen talepleri varsayılan olarak 80 numaralı port üzerinden dinleyecektir. Oysaki 80 numaralı portu IIS (Internet Information Services) kullanıyor olabilir. Bu durumda istemci uygulama çalışma zamanında hata mesajı verecektir. Nitekim 80 numaralı port başka bir uygulama tarafından kullanılmaktadır. Bunun önüne geçmek için, bağlayıcı tipin üzerinden ClientBaseAddress özelliğine geçici bir adres değeri ataması yapılır. Aynı makine üzerinde birden fazla istemci uygulaması çalıştırılabileceğinden, adreslerin benzersizliğinide garanti altına almak adına örnekte Guid tipinden yararlanılmıştır. Örnek bu son haliyle test edilirse, yine servisin abonelere geri bildirimde bulunabildiği görülecektir. Üstelik bu kez durum HTTP üzerinden gerçekleştirilmektedir. Dolayısıyla sistem internet üzerinden kullanılabilir halede gelmiştir.

Elbette bu senaryoda dikkat edilmesi gereken durumlar vardır. Söz gelimi, istemcilerin aboneliklerini kaldırmadıkları durumlar göz önüne alınabilir. Örneğin 3 farklı istemci servise abone olsun. Bunlardan birisi aboneliğini kaldırmadan uygulamayı kapatmış olabilir. Oysaki servis tarafında yer alan abone koleksiyonunda 3 adet istemci kayıtlıdır. Dolayısıyla diğer istemcilerden birisinin yaptığı çağrı sonrasında, servis tarafındaki aboneleri bilgilendirici metod halen daha var olmayan istemci abone koleksiyonundan çıkartılmadığı için istemci tarafında istisna alınmasına neden olabilir. Bunun önüne geçmek için State kontrolü yapılsada, yapılan testlerde bazı durumlarda istemcideki proxy ile servis arasındaki bağlantının tam olarak kapanmaması halinde bu tarz bir durum olduğu ortaya çıkmıştır. O halde istemci tarafında yer alan aboneliği kaldırma işleminin rolü son derece önemlidir.

Bu makalemizde, WCF uygulamalarında servislerin istemciler üzerinde nasıl metod çağırabileceğini Client Callback tekniği ile incelemeye çalıştık. Bu çağırım işlemini abone bazlı hale getirerek, herhangibir istemcinin bir olay gerçekleştirmesi sonrası servisin diğer istemcileride uyarabilmesini sağladık. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](assets/ClientCallback.zip)
