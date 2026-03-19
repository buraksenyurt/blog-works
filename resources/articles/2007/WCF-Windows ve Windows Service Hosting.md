---
title: "WCF-Windows ve Windows Service Hosting"
pubDate: 2007-05-04 09:00:00
categories:
  - WCF
tags:
  - WCF
  - C#
  - Bash
  - .NET
  - WPF
  - XML
  - Web Service
  - HTTP
  - IIS
  - Performance
  - Transactions
  - Serialization
  - Generics
  - Visual Studio
---

# WCF-Windows ve Windows Service Hosting
Değerli Okurlarım Merhabalar,

Windows Communication Foundation ile ilişikili önceki makalelerimizde mimarinin temellerinden ve bir WCF sevisinin Internet Information Services (IIS) üzerinden nasıl yayımlanabileceğini incelemiştik. Bu makalemizde ise Host uygulama olarak windows uygulamalarını ve windows servislerini ele almaya çalışacağız. Geliştireceğimiz uygulamalarda öncekilerden farklı olarak konfigurasyon dosyalarını kullanmayıp, programatik kod parçalarından yararlanacağız. Ayrıca, HTTP yerine TCP protokolü üzerinden haberleşmeyi kullanıyor olacağız.

Bildiğiniz gibi WCF mimarisinde sözleşmeleri (Contracts) sunan uygulamalar, sadece IIS üzerinden yayınlanmak zorunda değildir. Bu şablon dışında windows uygulamalarını (Hatta WPF uygulamalarını) ve windows servislerinide kullanabiliriz. Özellikle windows servisleri (Windows Service), bulundukları makine üzerinde otomatik olarak başlatılabildiklerinden, yönetilebilirlikleri daha kolaydır ve özellikle windows tabanlı intranet sistemlerinde tercih edilirler. Özellikle.Net Remoting üzerine yazılan sistemlerde windows servisleri yaygın olarak kullanılmaktadır.

WCF servislerinin ABC'sinden (AddressBindingContract) hatırlayacağınız üzere bağlayıcılar (Binding) bizim için pek çok zorluğun üstesinden gelmektedir. Daha önceki makalelerimizde geliştirdiğimiz WCF örnek uygulamalarında sadece BasicHttpBinding tipinden faydalandık. Oysaki projenin ihtiyacına ve şartlara göre diğer bağlayıcı tipleride göz önüne alabilir ve kullanabiliriz. Bu noktada bağlayıcı tipleri (Binding Types) biraz daha tanımakta fayda olacağı kanısındayım. Bağlayıcı tipler temel olarak istemcilerin, bir servis ile iletişime nasıl geçebileceği konusunda bazı esasları otomatik olarak belirlemekte rol alan varlıklar olarak tanımlanabilir.

Söz gelimi, istemcilerden kabul edilebilecek mesajların neler olacağını bağlayıcı tipler yardımıyla belirleyebiliriz. İstemciler, servislere erişirken çeşitli iletişim protokollerini kullanacaktır. Örneğin HTTP veya TCP. HTTP intranet dışında internet üzerindende kullanılabilecek bir ortam sağlarken, TCP özellikle intranet ortamında maksimum performansı sağlayacak nitelikte bir alt yapı sağlamaktadır. Bağlayıcı tipler bu protokolün ne olacağının belirlenmesinde etkin rol oynar. Söz gelimi BasicHttpBinding tipi HTTP protokolü üzerinden iletişime geçilmesi için gerekli olan alt yapıyı sağlamaktadır.

> Bazı WCF servis uygulamalarında, birden fazla endPoint tanımlaması sıklıkla uygulanan bir tekniktir. Söz gelimi intranet tabanlı sistemler için TCP tabanlı bir bağlayıcı tipi (Binding) içerecek bir endPoint ile birlikte, internet üzerinden gelen isteklere yönelik olarak, HTTP tabanlı ve XML formatında çözümleme yapabilecek bir bağlayıcı tipi kullanacak endPoint tanımlamaları bir arada ele alınıp kullanılabilir.

Diğer taraftan istemciler ve servis arasında taşınacak olan verinin nasıl formatlanacağınada bağlayıcı tipler karar vermektedir. Burada söz konusu olan formatlama, verinin XML olarak mı yoksa binary olarak mı serileştirileceğidir. Yine binary formatta serileştirme, özellikle intranet gibi sistemlerde hız ve performans sağlayacaktır. Özellikle image gibi büyük boyutlu veri yapılarının binary formatta transfer edilmesi performans açısından önemli bir kriterdir.

Bağlayıcıların etkin olarak rol aldıkları diğer noktalar servisin güvenilirliğinin nasıl sağlanacağı (reliability) ve servisin transaction içeren bir operasyonda görev alıp almayacağıdır. Bu ve benzeri kriterler göz önüne alındığında, System.ServiceModel isim alanı altında yer alan önceden tanımlı bağlayıcı tipler hazır çözümler sunmaktadır. Bu elbetteki kendi bağlayıcı tiplerimizi yazamayacağımız anlamına gelmemelidir. Dilersek bunuda yapabiliriz. Örneklerimizde TCP prokolünü ve binary serileştirmeyi tercih edeceğimizden, bununla ilgili olan bağlayıcı tipi kullanabiliriz.

Bu kısa bilgilerden sonra dilerseniz örneklerimizi adım adım geliştirmeye başlayalım. İlk olarak servis sözleşmemizi (Service Contract) ve bu sözleşmeyi uygulayacak tipimizi yazmamız gerekiyor. Amacımız kod tarafında servisi tesis etmek ve kullanmak olduğundan mümkün olduğu kadar basit bir sözleşme ve tip tanımlaması yapacağız. Bu amaçla aşağıdaki tipleri içerecek AritmetikLib isimli bir sınıf kütüphanesi (class library) geliştirerek işe başlayabiliriz.

![mk202_1.gif](images/mk202_1.gif)

Servis Sözleşmemiz;

```csharp
using System;
using System.ServiceModel;

namespace AritmetikLib
{
    [ServiceContract(Name="CebirciServisi",
          Namespace="http://www.bsenyurt.com/CebirciServisi")]
    public interface IAritmetikContract
    {
        [OperationContract]
        double Toplam(double x, double y);
    }
}
```

Servis sözleşmemiz herzamanki gibi bir arayüzdür (Interface). Diğer tarafan istemcilere sunulabilecek olan operasyonumuz basit olarak bir Toplama işlemi yapmaktadır. Özel olarak ServiceContract isimli niteliğimizin Name ve Namespace özelliklerine bazı değerler atadık. Name ve Namespace özellikleri, sözleşmeye ait WSDL (Web Service Description Language) elementi içerisindeki ilgili değerleri belirlemekte kullanılmaktadır ve daha sonradan bu sözleşmenin istemciler için gerekli olan proxy sınıfını üretilirken ele alınacaktır.

Sözleşmeyi uyarlayan tipimiz;

```csharp
using System;

namespace AritmetikLib
{
    public class Cebirci:IAritmetikContract
    {
        #region IAritmetikContract Members
        public double Toplam(double x, double y)
        {
            return x + y;
        }
        #endregion
    }
}
```

Cebirci isimli sınıfımız basit olarak servis sözleşmesi olan IAritmetikContract isimli arayüzü (interface) implemente etmektedir. Servis sözleşmemizi ve tipimizi tanımladıktan sonra artık Windows tabanlı Host uygulamamızı geliştirmeye başalayabiliriz. Söz konusu uygulamamızın ön yüzünü aşağıdaki gibi tasarlayabiliriz.

![mk202_2.gif](images/mk202_2.gif)

Başlat başlıklı düğmeye basıldığında, windows uygulamamız ServiceHost sınıfına ait bir nesneyi örnekleyerek istemcilerden (Clients) gelecek olan talepleri dinlemeye başlayacaktır. Tam tersine Durdur başlıklı düpmeye bastığımızda ServiceHost nesne örneği ile açılan kanallar kapatılacak ve Host uygulama artık istemcilere cevap vermeyecektir. Bu işlemler sırasında servisin durumuda label kontrolü içerisinde gösterilecektir. Başlamadan önce WCF tiplerini uygulamamız içerisinde ele alabilmek ve az önce geliştirdiğimiz servis sözleşmesi ve tipimizi kullanabilmek için sırasıyla System.ServiceModel.dll ve AritmetikLib.dll isimli assemblylerımızı uygulamamıza referans etmemiz gerekmektedir.

![mk202_3.gif](images/mk202_3.gif)

Artık uygulamamızın kodlarını aşağıdaki gibi geliştirebiliriz.

```csharp
using System;
using System.ServiceModel;
using System.Windows.Forms;
using AritmetikLib;

namespace HostApp
{
    public partial class Form1 : Form
    {
        private ServiceHost srvHost;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnBaslat_Click(object sender, EventArgs e)
        {
            srvHost = new ServiceHost(typeof(Cebirci));
            srvHost.AddServiceEndpoint(typeof(IAritmetikContract), new NetTcpBinding(), "net.tcp://localhost:4501/CebirciServisi");
            srvHost.Open();
            lblServisDurumu.Text = srvHost.State.ToString();
            btnBaslat.Enabled = false;
            btnDurdur.Enabled = true;
        }

        private void btnDurdur_Click(object sender, EventArgs e)
        {
            srvHost.Close();
            lblServisDurumu.Text = srvHost.State.ToString();
            btnDurdur.Enabled = false;
            btnBaslat.Enabled = true;
        }
    }
}
```

btnBaslat düğmesine ait Click olay metodu içerisinde ilk olarak ServiceHost nesnemiz örneklenmektedir. Yapıcı metoda (Constructor), Cebirci sınıfın tipi verilmiştir. Bir başka deyişle servis sözleşmesini uyarlayan tip bildirilmektedir. Servislerin, istemcilerden gelen talepleri endPoint'ler yardımıyla aldığını biliyoruz. Bir endPoint içerisinde adres, bağlayıcı tip ve sözleşme bilgilerininde yer alması gerekiyor. Bu nedenle, ServiceHost sınıfımıza ait nesne örneğimize AddServiceEndPoint metodu yardımıyla bir endPoint eklemekteyiz.

Bu metodun ilk parametresi servis sözleşmemize ait tipi temsil etmektedir. İkinci parametrede dikkat ederseniz NetTcpBinding tipinden bir nesne örneği verilmektedir. Birbaşka deyişle TCP protokolü üzerinden, binary formatta çözümleme kullanılarak iletişime geçileceğini bildirmiş oluyoruz. Son parametre ise servisimizdeki bu endPoint'e nasıl ulaşılabileceği bilgisini içermektedir. Bu parametrede makine adı, iletişim protokolü, port numarası ve nesne takma adı gibi bilgiler yer almaktadır.

Bu adımlardan sonra tek yapılması gereken servisin dinlemeye başlamasını sağlamaktır. Bu amaçla ServiceHost tipinden nesne örneğimizin Open metodunu kullanıyoruz. btnDurdur isimli düğmemize ait Click olay metodunda ise Close metodu çağırılarak servisin kapatılması sağlanmaktadır. İstemciyi yazmadan önce windows uygulamasını başlatarak test edebilirsiniz. Eğer sistemde yüklü bir Firewall var ise, 4501 numaralı portun açılmasına karşın bir uyarı mesajı verecektir. Bunu kabul ederek devam etmek gerekmektedir. Aksi takdirde portun kullanılması engelleneceğinden, istemcilere cevap verilemez ve servis çalışmaz.

> Bir Host uygulama herhangibir iletişim portunu kullanarak hizmet vermeye başladığında, başka bir Host uygulama aynı makine üzerindeki aynı iletişim portunu kullanamaz. Böyle bir durumuda çalışma zamanı istisnası (Runtime Exception) alınacaktır.

Artık istemci tarafını programlayabiliriz. Bildiğiniz gibi WCF sisteminde yer alan istemcilerimiz, servislere ait endPoint'ler ile haberleşirken proxy sınıflarını kullanmaktadırlar. Şuanki senaryomuzda servisimizi IIS üzerinden host etmediğimiz için HTTP tabanlı olacak şekilde metadata publishing yapamamaktayız. Ancak svcutil.exe aracı ile AritmetikLib isimli dll'imizden yararlanarak istemci için gereken proxy sınıfı ve konfigurasyon dosyalarını ürettirebiliriz. Bu amaçla ArtimetikLib.dll ' ini herhangibir yerden svcutil aracı ile aşağıdaki gibi ele almalıyız.

```bash
svcUtil AritmetikLib.dll
```

![mk202_4.gif](images/mk202_4.gif)

SvcUtil aracını bu şekilde kullandığımızda library içerisinde yer alan servis sözleşmesi ve tiplerden yararlanılaraktan bir takım metadata ve şema dosyaları üretilecektir.

![mk202_5.gif](images/mk202_5.gif)

Çok basit olarak düşünecek olursak, bu dosyaların istemciler için gereken proxy sınıflarını üretmek amacıyla kullanılabileceğini düşünebiliriz. Sonuç itibariyle IIS üzerinden yapılan Host işlemlerindede bir WSDL dökümanı gerekmektedir ki burada bu işi www.bsenyurt.com.CebirciServisi.wsdl isimli dosya üstlenmektedir. Bu işlemin ardından yine svcutil aracını aşağıdaki gibi kullanarak istemciler için gerekli proxy sınıfı ve konfigurasyon dosyasını üretebiliriz. (Buradaki dosya isimlerinin tesadüf olmadığını söyleyelim. Hatırlarsanız ServiceContract niteliğimizin Namespace özelliğine http://www.bsenyurt.com/CebirciServisi değerini vermiştik.)

```bash
svcutil www.bsenyurt.com.CebirciServisi.wsdl *.xsd /out:AritmetikClient.cs
```

![mk202_6.gif](images/mk202_6.gif)

Komut başarılı bir şekilde çalıştığı takdirde istemciler için AritmetikClient.cs ve output.config isimli fiziki dosyalar üretilecektir. Oluşturulan proxy dosyası içerisinde CebirciServisi, CebirciServisiChannel isimli iki arayüz yer almaktadır. İstemci tarafından örneklenerek kullanılabilecek olan CebirciServisiClient isimli sınıf CebirciServisi isimli arayüzü uygulamakatadır. Servis ile iletişimde önemli bir rol üstelenen kanalı temsil eden CebirciServisiChannel isimli tipimiz ise hem CebirciServisini hemde IClientChannel arayüzünü uygulamaktadır. IClientChannel arayüzü temel olarak istemciler için taban arayüzdür (base interface) ve WCF iletişimi için gerekli çalışma zamanı fonkisyonellikleri sağlamaktadır. Örneğin kanalın çalışma zamanında açılması veya dispose edilmesi gibi işlemlerin yapılmasını sağlayan fonksiyonellikler içerir. (Bunun gibi mimari detayları ilerleyen makalelerimizde ele almaya çalışacağız.)

![mk202_8.gif](images/mk202_8.gif)

Artık istemci uygulamamızı geliştirmeye başlayabilir ve az önce üretilen proxy tipimizi burada kullanarak sunucu ile iletişime geçebiliriz. İşlemlerimizi basit bir şekilde ele almak amacıyla istemcimizi sıradan bir Console uygulaması olarak tasarlayacağız. Uygulamamızın yine System.ServiceModel assembly'ını referans etmesi ve komut satırından SvcUtil aracı yardımıyla ürettiğimiz proxy sınıfını içermesi gerekmektedir.

![mk202_7.gif](images/mk202_7.gif)

Output.config dosyasını istemci uygulamaya eklemek zorunda değiliz. Şu aşamada istemci tarafından servisi çağırmak için gerekli ayarları programatik olarak yapıyor olacağız. İlgili referans ve proxy tipini ekledikten sonra kodlarımızı aşağıdaki gibi geliştirebiliriz.

```csharp
using System;
using System.ServiceModel;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Devam etmek için tuşa basın");
            Console.ReadLine();
            CebirciServisiClient client;
            client = new CebirciServisiClient(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:4500/CebirciServisi"));
            Console.WriteLine(client.Toplam(4, 5).ToString());
            Console.ReadLine();
        }
    }
}
```

İlk olarak CebirciServisiClient isimli sınıfımıza ait bir nesne örneği oluşturuyoruz. Bunu gerçekleştirirken yapıcı metodumuza ilk parametre olarak NetTcpBinding tipinden bir nesne örneği atanmaktadır. Sunucu tarafında NetTcpBinding tipini kullandığımız için istemci tarafındada uygun bir bağlayıcı tipin kullanılması gerekmektedir. İkinci parametre olarak bir EndpointAddress nesne örneği oluşturulmaktadır. Bu sınıfın yapıcı metodunda verdiğimiz adres ise, servis tarafında belirttiğimiz erişim adresidir. Son olarak uzak nesne metodumuzu çalıştırmaktayız. Artık istemcimizi test edebiliriz. Elbetteki önce sunucu uygulamanın çalıştırılması ve servisin açılması gerekmektedir. Eğer sunucu uygulama üzerinden servis açılmadan istemci çalıştırılırsa aşağıdaki gibi çalışma zamanı hatası (Runtime Exception) alırız.

![mk202_9.gif](images/mk202_9.gif)

Exception mesajı özellikle.Net Remoting uygulamaları geliştirenler için tanıdık bir cümle içermektedir. No Connection could be made because the target machine actively refused it.:) Bu aynı zamanda istemcimizin gerçekten belirtilen adreste kendisini dinleyecek bir WCF servis uygulaması aradığınında bir göstergesidir. Aşağıdaki flash animasyonunda basit olarak sistemimizin nasıl çalıştığı gösterilmektedir.

Gelelim Windows uygulamasından sunmuş olduğumuz hizmeti bir windows servisi üzerinden nasıl sunacağımıza. Aslında WCF tarafından baktığımızda sadece windows servisi ortamına ayak uydurmamız yeterli olacaktır. Gelin örneğimiz üzerinden devam edelim. İlk olarak solution'ımıza bir Windows Service projesi eklememiz gerekiyor. Projemizi oluşturduktan sonra, her zaman olduğu gibi System.ServiceModel.dll isimli assembly'ın ve servis sözleşemesi ile uzak nesne sınıfını barındıran sınıf kütüphanemizin (AritmetikLib.dll) referans edilmesi gerekmektedir.

Windows servisleri, web servislerine benzer olarak herhangibir görsel arabirime sahip değildir. İş yapan metodları, olay tabanlı çalışmaktadır. Dolayısıyla ilgili servis, windows işletim sisteminin servislerine eklendikten sonra, başlatılması, durudurulması yada hata oluşması gibi durumlarda yapılması gereken kodlar ilgili olay metodlarına yazılmaktadır. Bizim windows servisimiz başlatıldığında uzak nesneleri istemci uygulamaların hizmetine sunacak şekilde dinlemede olmalıdır. Diğer taraftan servis kapatıldığında ise, Host uygulamanın kullandığı serviceHost nesne örneği artık istemcilere cevap vermeyecek şekilde kapatılmalıdır. Bu duruma göre servis sınıfımızın kodları aşağıdaki gibi olmalıdır.

```csharp
using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.ServiceModel;
using AritmetikLib;

namespace AritmetikWinServis
{
    public partial class Service1 : ServiceBase
    {
        private ServiceHost srvHost;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            srvHost = new ServiceHost(typeof(Cebirci));
            srvHost.AddServiceEndpoint(typeof(IAritmetikContract), new NetTcpBinding(), "net.tcp://localhost:65001/CebirciServisi");
            srvHost.Open();
        }

        protected override void OnStop()
        {
            srvHost.Close();
        }
    }
}
```

Dikkat ederseniz makalemizin başında geliştirdiğimiz Windows uygulmasındaki kodlarımızdan çokda farklı bir işlem gerçekleştirmiyoruz. Servis OnStart olay metodu içerisinde oluşturulurken, endPoint'in eklenmesi ve açılmasıda burada yapılmaktadır. Benzer şekilde OnStop olay metodu içerisinde servisimiz kapatılmaktadır. Bundan sonraki adımlarımızda servisimizi windows sistemine install etmek için gerekli işlemleri yapmamız gerekmektedir. Söz konusu adımlar makalemizin konusu dışında olduğundan yüzeysel olarak ele alınacaktır. Öncelikli olarak servisimize Add Installer seçeneği ile gereken yükleyici tipleri ekliyoruz.

![mk202_10.gif](images/mk202_10.gif)

Bu işlemin sonucunda servise ServiceProcessInstaller ve ServiceInstaller tiplerinden iki adet bileşen eklenecektir.

![mk202_11.gif](images/mk202_11.gif)

ServiceProcessInstaller bileşeninin üyelerinden Account özelliğinin değerini Local System olarak ayarlayalım. ServiceInstaller bileşenimizin, Display Name özelliğine Aritmetik Servis değerini atayalım. Bu sistem servislerine baktığımızda, geliştirdiğimiz windows servisini bulmamızı kolaylaştıracaktır. Uygulamamız derlendikten sonra işletim sistemine yüklenmeye hazır hale gelecektir. Yükleme işlemi için Visual Studio 2005 Command Prompt üzerindeyken installUtil aracını aşağıdaki gibi -i parametresi ile kullanmamız gerekmektedir.

![mk202_12.gif](images/mk202_12.gif)

InstallUtil aracı işlemleri başarılı bir şekilde tamamladıktan sonra servislerde, eklemiş olduğumuz windows servisi görülebilir.

> Sisteme yüklediğimiz servisi kaldımak için installUtil aracının -u parametresini kullanmak gerekmektedir.

![mk202_13.gif](images/mk202_13.gif)

Servisimizi buradan çalıştırdığımızda artık istemciler tarafından kullanılabilir hale gelecektir. İstemci tarafında tek yapmamız gereken port bilgisini değiştirmek olacaktır. Örneğin bu makalede geliştirdiğimiz Console uygulamasının kodlarını aşağıdaki gibi değiştirmek yeterlidir.

```csharp
CebirciServisiClient client;
client = new CebirciServisiClient(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:65001/CebirciServisi"));
Console.WriteLine(client.Toplam(4, 5).ToString());
```

Mimarinin yine doğru bir şekilde tesis edildiğini kontrol etmek adına, geliştirilen servis kapalı iken istemci uygulama çalıştırılabilir. Eğer "No Connection could be made because the target machine actively refused it" hatası alınabiliyorsa, istemcinin gerçektende sunucu servisi aradığını anlayabiliriz. Elbette servis çalıştırıldıktan sonrada istemciyi test edip herşeyin yolunda gittiğinden emin olmakta fayda vardır. Aşağıdaki Flash animasyonunda bu durum gösterilmektedir.

Bu makalemizde WCF mimarisinde yer alan Host uygulama modellerinden Windows ve Windows Servislerini kısaca incelemeye çalıştık. Aynı zamanda, konfigurasyon dosyası kullanımı yerine kod tarafında neler yapabileceğimize değindik. Özellikle TCP tabanlı bir binding tipi kullandığımızda (NetTcpBinding), bir başka deyişle HTTP üzerinden Metadata yayınlanması söz konusu olmadığında, istemciler için gerekli olan proxy sınıflarının svcutil aracı yardımıyla nasıl kolay bir şekilde üretilebileceğine bakmaya çalıştık ve böylece bir makalemizin daha sonuna geldik. İlerleyen makalelerimizde WCF mimarisinin detaylarını incelemeye devam ediyor olacağız. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.

[Örnek Uygulama İçin Tıklayınız.](assets/UsingTcpBinding.rar)
