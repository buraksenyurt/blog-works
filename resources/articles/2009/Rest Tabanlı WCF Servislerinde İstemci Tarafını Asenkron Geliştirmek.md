---
title: "Rest Tabanlı WCF Servislerinde İstemci Tarafını Asenkron Geliştirmek"
pubDate: 2009-04-24 09:26:00
categories:
  - WCF
tags:
  - WCF
  - C#
  - .NET
  - LINQ
  - WPF
  - Windows Forms
  - XML
  - REST
  - Web Service
  - XML Web Services
  - HTTP
  - Async/Await
  - Threading
  - Serialization
  - Delegates
---

# Rest Tabanlı WCF Servislerinde İstemci Tarafını Asenkron Geliştirmek
Merhaba Arkadaşlar,

Bir önceki [yazımızda](https://www.buraksenyurt.com/post/Rest-Tabanl%C4%B1-WCF-Servisleri-icin-Istemci-Yazmak)REST bazlı WCF servisleri için, WCF Rest Stater Kit yardımıyla istemci uygulamaların nasıl geliştirilebileceğini incelemeye çalışmıştık. İstemci açısından önemli olan konulardan biriside, uzun sürebilecek request/response operasyonları sırasında uygulamasını kullanmaya devam edebiliyor olmasıdır. Tahmin edeceğiniz üzere istemci tarafında bir request'in asenkron olarak gönderilip, işlenmesi konusunu değerlendiriyor olacağız. Aslında asenkron erişimden kastımız, istemcinin talebi gönderdikten sonra cevabın anında gelmesini beklemeden çalışmasına devam edebilmesidir. Servis tabanlı uygulamalar söz konusu olduğunda, asenkron işlemleri iki lokasyonda tasarlayabiliriz.

Asenkron işlemler servis tarafında uygulanır. Geliştiricinin asenkron modeli kendisinin uygulamasını gerektirebilir.
Asenkron işlemler istemci tarafında uygulanır. Hazır olan temsilci (delegate) tabanlı (Polling, WaitHandle, Callback) veya olay tabanlı (Event Based) modeller kullanılır.

Benim bu yazıda ele alacağım istemci tarafındaki asenkron işlemlerdir. Bir önceki yazımızda geliştirdiğimiz Windows uygulamasında bu amaçla yeni düzenlemeler yapılacaktır. Hazırlıklı olmamız gereken konuların başında, delegate kavramı ve asenkron Callback modeli gelmektedir. Ama sonrasıda buna event based asenkron modeli ve lambda operatörünüde katıyor olacağız. Arada ise bize bonus bir konu çıkacak. Tedbir almassak kaçınılmaz olan Illegal Cross Thread Operations

![Undecided](images/smiley-undecided.gif)

İlk olarak Form arkası kodlarımızı aşağıdaki gibi geliştirdiğimizi düşünelim.

```csharp
using System;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using System.Xml.Serialization;
using Microsoft.Http;
using NorthwindV2;

namespace NorthwindClient
{
    public partial class Form1 : Form
    {
        // İstemci talepleri için kullanacağımız sınıf HttpClient
        HttpClient client = null;
        // Servis adresi (sondaki / işaretini unutmadım bu sefer)
        string serviceUri="http://localhost:1000/Service.svc/";

        public Form1()
        {
            InitializeComponent();
            // HttpClient nesnemizi örnekliyoruz
            client = new HttpClient();            
        }

        private void btnGetProductsAsync_Click(object sender, EventArgs e)
        {
            // İletişim için 10 saniyelik timeout süresini belirliyoruz
            client.TransportSettings.ConnectionTimeout = TimeSpan.FromSeconds(10);
            // BeginSend metodu yardımıyla asenkron çağrıyı başlatıyoruz. İlk parametreye göre http://localhost:1000/Service.svc/ adresine GET metodu ile talepte bulunuyoruz. 
            // İkinci parametre bu işlem bittiğinde devreye girecek asenkron metodu işaret eden meşhur AsyncCallback temsilcimiz.
            // HttpClient nesnemiz sınıf seviyesinde tanımlandığı için 3ncü parametreyi göndermemize gerek yok.
            client.BeginSend(new HttpRequestMessage("GET", serviceUri), new AsyncCallback(GetProductsAsyncCallback), null);
        }

        // BeginSend metodu ile başlatılan asenkron işlemler tamamlandığında devreye girecek olan callback metodudur.
        private void GetProductsAsyncCallback(IAsyncResult iar)
        {
            // EndSend metodu ile tamamlanan operasyon sonucu cevap alınır.
            using (HttpResponseMessage response = client.EndSend(iar))
            {
                // eğer HTTP 200 kodu döndüyse exception fırlatılmadan devam edilebilir
                response.EnsureStatusIs(HttpStatusCode.OK);
                // ItemInfoList tipi ReadAsXmlSerializable metodu ile çekilir
                ItemInfoList products = response.Content.ReadAsXmlSerializable<ItemInfoList>();               

                grdProducts.DataSource = (from p in products.ItemInfo
                                          select p.Item).ToList();                
            }
        }
    }
}
```

Buradaki kod parçasında, standart Asynchronous Callback modelinin bir uyarlaması yer almaktadır. Asenkron desenler temsilcilerin kullanıldığı senaryolarda ele alınabilirler. Genel olarak 3 farklı asenkron tekniği vardır.

- Polling modeline göre asenkron olarak başlatılan işlemin bitip bitmediği sürekli olarak kontrol edilir.
- WaitHandle modeli aslında kendi içerisinde WaitOne, WaitAll, WaitAny gibi 3 farklı tekniğe ayrılmaktadır. Ancak tekniklerin özünde, asenkron başlatılan bir işin, belirli bir noktadan sonra dönüş değerlerine ihtiyaç duyuluyorsa, thread'in ilgili noktada duraksatılması vardır. Öyleki, bazı asenkron işlemler sonucunda gelen veriler daha çekilemeden, program içerisinde onların kullanılacağı yerlere geçişler yapılabilir. Bunu kullanıcıda yapabilir, programın kod akışıda buna müsait olabilir. Dolayısıyla ortada dönen veriler yoksa istenmeyen sonuçlar alınabilir.
- Callback modeli ise en sık kullanılan tekniklerden birisidir. Bu modele göre asenkron başlatılan işleyiş tamamlandığında,.Net Framework'ün Built-In Delegate tiplerinden olan AsyncCallback'in işaret ettiği (geriye dönüş değeri olmayan yani void ve IAsyncResult arayüzü tipinden parametre alan) metod çalıştırılır.

Ancak bir asenkron model WinForms yada WPF gibi görsel bir arabirimde uygulanıldığında çok dikkatli olunmalıdır. Yukarıda yazdığımız program kodunu denediğimizde bu acı gerçekle aşağıdaki ekran görüntüsünde olduğu gibi karşılaşmamız kaçınılmazdır.

![blg7_1.gif](images/blg7_1.gif)

Aslında sebep son derece basittir. Normal şartlarda Form üzerindenki tüm bileşenlerin sahibi olan bir ana iş parçamız vardır (Main Thread). Biliyoruzki bir.Net uygulaması belleğe açıldığında mutlaka bir ana Thread'e sahiptir.(Hatta Process içerisinde çalışan Module (Modules) ve bunlarında içerisindede en az bir ana thread olmak üzere birden fazla thread'de olabilir) Diğer taraftan yazdığımız asenkron modelde açılan farklı bir thread'de bu kontrollerden birisine (DataGridView bileşenimiz

![Embarassed](images/smiley-embarassed.gif)

) erişmek istemektedir. Bu durumda ana thread buna kızar (çünkü bencildir ve kontrollerini kimse ile paylaşmak istemez) ve çalışma zamanına yukarıda gördüğümüz istisna fırlatılır. Bunu çözmek için kolaya kaçabiliriz. Ancak en etkili çözümlerden birisi Method Invoker kullanmaktır. Bu nedenle yukarıdaki kod parçasını aşağıdaki gibi değiştirmemiz gerekmektedir.

```csharp
using System;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using System.Xml.Serialization;
using Microsoft.Http;
using NorthwindV2;

namespace NorthwindClient
{
    public partial class Form1 : Form
    {
        // İstemci talepleri için kullanacağımız sınıf HttpClient
        HttpClient client = null;
        // Servis adresi (sondaki / işaretini unutmadım bu sefer)
        string serviceUri="http://localhost:1000/Service.svc/";

        public Form1()
        {
            InitializeComponent();
            // HttpClient nesnemizi örnekliyoruz
            client = new HttpClient();            
        }

        private void btnGetProductsAsync_Click(object sender, EventArgs e)
        {
            // İletişim için 10 saniyelik timeout süresini belirliyoruz
            client.TransportSettings.ConnectionTimeout = TimeSpan.FromSeconds(10);
            // BeginSend metodu yardımıyla asenkron çağrıyı başlatıyoruz. İlk parametreye göre http://localhost:1000/Service.svc/ adresine GET metodu ile talepte bulunuyoruz. 
            // İkinci parametre bu işlem bittiğinde devreye girecek asenkron metodu işaret eden meşhur AsyncCallback temsilcimiz.
            // HttpClient nesnemiz sınıf seviyesinde tanımlandığı için 3ncü parametreyi göndermemize gerek yok.
            client.BeginSend(new HttpRequestMessage("GET", serviceUri), new AsyncCallback(GetProductsAsyncCallback), null);
        }

        // BeginSend metodu ile başlatılan asenkron işlemler tamamlandığında devreye girecek olan callback metodudur.
        private void GetProductsAsyncCallback(IAsyncResult iar)
        {
            // EndSend metodu ile tamamlanan operasyon sonucu cevap alınır.
            using (HttpResponseMessage response = client.EndSend(iar))
            {
                // eğer HTTP 200 kodu döndüyse exception fırlatılmadan devam edilebilir
                response.EnsureStatusIs(HttpStatusCode.OK);
                // ItemInfoList tipi ReadAsXmlSerializable metodu ile çekilir
                ItemInfoList products = response.Content.ReadAsXmlSerializable<ItemInfoList>();               

                //grdProducts.DataSource = (from p in products.ItemInfo
                //                         select p.Item).ToList();
                LoadGrid(products);
            }
        }

        #region Method Invoker ile Illegal Cross Thread' in önüne geçmek

        private delegate void LoadGridHandler(ItemInfoList list);
        private void LoadGrid(ItemInfoList list)
        {
            if (grdProducts.InvokeRequired)
                grdProducts.Invoke(new LoadGridHandler(LoadGrid), list);
            else
                grdProducts.DataSource = (from p in list.ItemInfo
                                          select p.Item).ToList();
        }

        #endregion
    }
}
```

Bu durumda program kodumuz sorunsuz bir şekilde çalışacak ve içeriğin asenkron olarak çekilmesi sağlanabilecektir. Tabi şunuda düşünmek gerekir..Net Framework 2.0 relase olduğunda Xml Web Servislerinin kullanılması ile ilişkili istemci tarafına gelen yeniliklerden birisi, asenkron modeli olay bazlı uygulayabiliyor olmamızdı. Yani temsilciler ve method invoker'lar ile uğraşmak yerine basit olay (event) yüklemeleri ile işlemler çözülebilmektedir. Modelin özünde Completed kelimesi ile biten bir olay (Event), Async kelimesi ile biten ve asenkron işlemi başlatmamızı sağlayan, AsyncCancel kelimesi ile biten ve asenkron başlatılan işlemin iptal edilmesinde kullanılan birer metod bulunmaktadır.

Aynı modeli Rest bazlı WCF servis istemcilerinde de uygulayabiliriz. Nitekim HttpClient sınıfının bu amaçla tasarlanmış SendCompleted isimli olayı, SendAsync ve SendAsyncCancel isimli metodları bulunmaktadır. Bu noktada kodu ilk etapta

```csharp
private void btnGetProductsEvent_Click(object sender, EventArgs e)
{
      client.SendCompleted += new EventHandler<SendCompletedEventArgs>(client_SendCompleted);
}

void client_SendCompleted(object sender, SendCompletedEventArgs e)
{
      throw new NotImplementedException();
}
```

şeklinde tasarlamaya başlayabiliriz. Oysaki C# 3.0 ile birlikte gelen lambda operatörünü (=>) kullanarak, olayın yüklenmesi, olay sonucu çalıştırılacak olan metod bloğunuz yazılması ve içerisine gerekli parametrelerin aktarılması işini aşağıdaki kod parçasında olduğu gibide gerçekleştirebiliriz.

```csharp
private void btnGetProductsEvent_Click(object sender, EventArgs e)
{
    client.TransportSettings.ConnectionTimeout = TimeSpan.FromSeconds(10);
    client.SendCompleted+=(sndr,arg)=>{
        // İşlem iptal edilmediyse
        if (arg.Cancelled)
            MessageBox.Show("İşlem iptal edildi");
        // İşlem sonucunda bir istisna oluşmamışsa
        else if (arg.Error != null)
            MessageBox.Show(arg.Error.Message);
        else
        {
            ItemInfoList products=arg.Response.Content.ReadAsXmlSerializable<ItemInfoList>();
            grdProducts.DataSource = (from p in products.ItemInfo
                                              select p.Item).ToList();
        }
    };
    client.SendAsync(new HttpRequestMessage("GET", serviceUri));
}
```

Kullanılan bu son teknikte herhangibir şekilde Illegal Cross Thread Operation sorunsalınında yaşanmadığı gözlemlenebilir. Bu olay bazlı asenkron mimarinin bir avantajıdır. Yani Method Invoker kullanmamıza gerek kalmadan asenkron olarak üretilen sonuçlar DataGridView kontrolü içerisine alınabilmektedir.

Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

Örneğin son hali: [NorthwindV2.rar (592,24 kb)](assets/NorthwindV2.rar)
