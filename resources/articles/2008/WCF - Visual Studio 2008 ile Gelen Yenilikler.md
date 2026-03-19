---
title: "WCF - Visual Studio 2008 ile Gelen Yenilikler"
pubDate: 2008-03-14 12:00:00
categories:
  - WCF
tags:
  - WCF
  - C#
  - XML
  - .NET
  - Workflow Foundation
  - WPF
  - Web Service
  - XML Web Services
  - HTTP
  - Async/Await
  - Threading
  - Delegates
  - Generics
  - Debugging
  - Visual Studio
---

# WCF - Visual Studio 2008 ile Gelen Yenilikler
Değerli Okurlarım Merhabalar,

Yazılım dünyası çeşitli ürün gruplarını ve bunların üretimini içeren materyaller içermektedir. Son kullanıcıya (End User) veya geliştiricilere (Developers) yönelik olarak tasarlanan ürünlerin yazılmasında çeşitli program geliştirme arabirimleri kullanılmaktadır. Belkide bunlardan en popüler olanları Microsoft tarafından üretilen Visual Studio ailesidir. Visual Studio.Net ile başlayan serüvende kısa bir süre öncede Visual Studio 2008 sürümü son haliyle yayınlanmıştır. Yeni sürüm özellikle.Net Framework 2.0, 3.0 ve 3.5 için ortak ve tek bir geliştirme ortamı sunmasıyla hemen dikkati çekmektedir. Bu ve benzer özelliklerin yanında Windows Communication Foundation çözümleri içinde ek bir takım yenilikleri gelmektedir.

Göze çarpan yeniliklerden ilki WcfSvcHost.exe ve WcfTestClient.exe isimli yardımcı uygulamalardır. Visual Studio 2008 kurulumundan sonra C:\Program Files\Microsoft Visual Studio 9.0\Common7\IDE klasörü altına eklenen bu programlar sayesinde herhangibir WCF servis kütüphanesi (WCF Service Library) Host ve istemci uygulamalara ihtiyaç duyulmadan test edilebilir. Genellikle bir servis kütüphanesi geliştirilirken ve test edilirken ekstra çaba sarfederek basit bir Host uygulama ve istemci (Client) yazılması gerekmektedir. Ancak Visual Studio 2008 ile gelen yardımcı araçlar sayesinde buna gerek kalmadan basit testler yapılabilmektedir.

Üstelik VS 2008 ile geliştirilen WCF servis kütüphaneleri, IDE içerisinden Start (F5 veya Ctrl+F5-Start Without Debugging) edildiklerinde otomatik olarak WcfSvcHost.exe ve WcfTestClient.exe araçları devreye girmektedir. Bir başka deyişle yazılan WCF servis kütüphaneleri anında çalıştırılıp test edilebilir. Söz konusu araçları komut satırındanda çalıştırmak ve kullanmak mümkündür. Yazıda ilk olarak bu araçlar tanınmaya çalışılacaktır. Elbette test amacıyla bir WCF servis kütüphanesine ihtiyaç vardır. Bu amaçla VS 2008 ortamında içerikleri aşağıdaki gibi olan tipler geliştirilerek işe başlanabilir.

![mk245_1.gif](images/mk245_1.gif)

Servis sözleşmesinin içeriği;

```csharp
[ServiceContract]
interface IAdventureSrv
{
    [OperationContract]
    Urun UrunBul(int id);

    [OperationContract]
    double Topla(double[] sayilar);
}
```

Sözleşmeyi uygulayan sınıfın içeriği;

```csharp
class AdventureSrv
        :IAdventureSrv
{
    #region INorthwindSrv Members

    public Urun UrunBul(int id)
    {
        Urun urn = null;
        using (SqlConnection conn = new SqlConnection("data source=.;database=AdventureWorks;integrated security=SSPI"))
        {
            SqlCommand cmd = new SqlCommand("Select Name,ListPrice From Production.Product Where ProductId=@PrdId", conn);
            cmd.Parameters.AddWithValue("@PrdId", id);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
                // C# 3.0 Object Initializers kullanılarak Urun nesnesi örneklenmektedir.
                urn = new Urun() 
                {
                    Ad=reader["Name"].ToString()
                    ,Fiyat=Convert.ToDouble(reader["ListPrice"])
                };
            reader.Close();
        }
        return urn;
    }

    public double Topla(double[] sayilar)
    {
        return sayilar.Sum<double>(s => s); //C# 3.0 Extension Methods kavramı kullanılmıştır.
    }

    #endregion
}
```

Urun sınıfının içeriği;

```csharp
[DataContract]
class Urun
{
    [DataMember]
    public string Ad;
    
    [DataMember]
    public double Fiyat;
}
```

Servis sözleşmesi (Service Contract) basit olarak iki adet metod içermektedir. UrunBul isimli metod aynı zamanda Urun isimli sınıfa ait nesne örneği döndürmektedir. Diğer taraftan Urun sınıfı veri sözleşmesi (Data Contract) şeklinde tanımlanmıştır. Diğer taraftan metodun parmametrik yapısının test araçlarındaki kullanımını daha kolay irdelemek için Topla isimli fonksiyon, double tipinden bir dizi ile çalışmaktadır. Kod içerisinde Urun sınıfına ait nesne örneklenirken C# 3.0 Object Initializers tekniği kullanılmaktadır.

Topla metoduna gelen double tipinden dizinin içerisindeki sayıların toplamını bulmak içinse Sum metodu (C# 3.0 Extension Methods) ele alınmaktadır. Geliştirilen servis kütüphanesinin özellikle WcfSvcHost için önemli olan kısmı konfigurasyon bilgileridir. Nitekim WcfSvcHost.exe uygulaması kod bazlı Host ayarlama işlemlerini ele alamaz. Bir başka deyişle test için kütüphanenin mutlaka config dosyasının yazılmış olması gerekir. İstemci tarafının, servis üzerinden Host edilecek tiplere ait servis metadata bilgilerini çekebilmek için MEX (Metadata Exchange) EndPoint tanımlanması yapılmamasında da yarar vardır. Bunlara göre örnek olarak bir config içeriği aşağıdaki gibi tasarlanabilir.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <services>
            <service behaviorConfiguration="MexBehavior" name="AdventureSrv">
                <endpoint address="net.tcp://localhost:45001/AdventureSrv" binding="netTcpBinding" name="AdvTcpEndPoint" contract="IAdventureSrv" />
                <endpoint address="http://localhost:45002/AdventureSrv" binding="wsHttpBinding" name="AdvWsHttpEndPoint" contract="IAdventureSrv" />
                <host>
                    <baseAddresses>
                        <add baseAddress="http://localhost:45000/" />
                    </baseAddresses>
                </host>
            </service>
        </services>
        <behaviors>
            <serviceBehaviors>
                <behavior name="MexBehavior" >
                    <serviceMetadata httpGetEnabled="true" />
                    <serviceDebug includeExceptionDetailInFaults="true"/>
                </behavior>
            </serviceBehaviors>
        </behaviors>
    </system.serviceModel>
</configuration>
```

Konfigurasyon dosyasındanda görüldüğü gibi servis kütüphanesi Tcp (NetTcpBinding) ve WsHttp (WsHttpBinding) bazlı iki farklı EndPoint sunmaktadır. Bununla birlikte http://localhost:45000 adresi üzerinden Metadata yayınlamasıda yapılmaktadır. Metadata bilgisinin çekilebiliyor olması, istemciler için gerekli olan Proxy sınıfının üretilmesinde önemli bir yere sahiptir.

WcfSvcHost aracının kullanım amacı bir servis kütüphanesinde tanımlanan hizmetlerin Host edilmesini sağlayacak otomatik bir Windows uygulamasını başlatmaktır. Bu uygulama çalıştırıldığında, parametre olarak verilen servis kütüphanesi ve konfigurasyon dosyasını kullanarak Host işlemini gerçekleştirir. Aynı zamanda konfigurasyon dosyasında tanımlanan EndPoint noktalarında tanımlanan servis sözleşmelerine (Service Contract) ait metadata bilgilerinin yayınlanmasınıda sağlayabilir. Basit olarak yukarıdaki örnek kütüphaneyi Host etmek üzere WcfSvcHost aracı komut satırından aşağıdaki gibi kullanılabilir.

WcfSvcHost /service:GenelIslemler.dll /config:GenelIslemler.dll.config

![mk245_2.gif](images/mk245_2.gif)

Service ile tanımlanan parametreden sonra servis kütüphanesi ismi verilmektedir. Config parametresinde ise konfigurasyon dosyası işaret edilir. Bu işlemin ardından WcfSvcHost uygulamasının Tray Icon'a atıldığı görülür.

![mk245_3.gif](images/mk245_3.gif)

Bir başka deyişle WcfSvcHost uygulaması arka planda Exit seçeneği ile çıkılana kadar çalışmaya ve servisleri yayınlamaya devam edecektir.(Close seçeneği yada X işareti kullanıldığında WcfSvcHost uygulaması Tray Icon olarak çalışmaya devam etmektedir.) WcfSvcHost uygulamasının çalışma zamanındaki görüntüsü ise aşağıdaki gibidir.

![mk245_4.gif](images/mk245_4.gif)

Dikkat edileceği üzere yayınlanan servisin adı, durumu ve Metadata içeriğinin alınabileceği URL adresi bilgileride gösterilmektedir. Servis uygulaması başarılı bir şekilde çalıştırıldıktan sonra istenirse WcfTestClient aracı yardımıyla istemcilerin denenmesine başlanabilir. WcfTestClient aracı aldığı parametrelere göre bir Windows uygulaması başlatır ve servisten aldığı metadata bilgilerine göre kullanılabilecek hizmetleri ve metodları gösterir. En basit kullanımında servis kütüphanesinde belirtilen base address bilgisi aşağıdaki gibi parametre olarak belirtilir.

WcfTestClient http://localhost:45000/

![mk245_5.gif](images/mk245_5.gif)

Bu işlemin arkasından WcfTestClient aracı bir Windows uygulaması çalıştıracak ve parametre olarak verilen adresin metadata bilgisinide kullanarak, çağırılabilecek hizmet noktalarını listeleyecektir. Arabirimde servis ile ilişkili metodlara çift tıklanarak çalıştırılmaları ve sonuçlarının görülmesi de sağlanabilir. Söz gelimi UrunBul isimli metod 1 değeri ile çalıştırıldığında aşağıdaki sonuçlar elde edilir.

![mk245_6.gif](images/mk245_6.gif)

Invoke düğmesine basılması ile birlikte UrunBul metoduna bir çağrı gerçekleştirilir. Bu çağrının sonucunda elde edilen Urun nesne örneği ve veri içeriği Response sekmesinde görülmektedir. Burada talep (Request) ve cevap (Response) paketlerinin içeriklerinin XML tarafındaki hallerinede bakılabilir. Bunun için XML sekmesine geçilmesi yeterlidir. Bu durumda aşağıdaki çıktı elde edilir. (Bu XML içeriklerinin daha önceden belirtilen paketlerin istemci tarafından manuel olarak hazırlanarak gönderilmesinde oldukça işe yarayacağı da göz önüne alınmalıdır.)

![mk245_7.gif](images/mk245_7.gif)

Servis tarafında yer alan metodlardan Topla fonksiyonu parametre olarak double tipinden bir dizi almaktadır. Bu tip bir metodun WcfTestClient aracı ile çalıştırılması esnasında öncelikli olarak kaç adet değer gönderileceği (bir başka deyişle dizinin boyutu) belirlenir. Daha sonra ise değişken değerleri girilir ve fonksiyon çağırılır. Bu durum aşağıdaki şekilde görüldüğü gibi örneklenebilir.

![mk245_8.gif](images/mk245_8.gif)

WcfTestClient aracının kullanımı sırasında eğer Host uygulama (WcfSvcHost) çalışmıyor yada herhangibir nedenle servislere ait metadata bilgileri çekilemiyorsa çalışma zamanı istisnası (Runtime Exception) alınır. Örnekte WcfSvcHost uygulaması kapalıyken WcfTestClient çalıştırılmak istenmiş ve sonuç olarak aşağıdakine benzer bir hata ekranı alınmıştır.

![mk245_9.gif](images/mk245_9.gif)

WcfSvcHost uygulamasının parametrik yapısı kullanılarak istenirse aynı anda WcfTestClient uygulamasınında çalıştırılması sağlanabilir ki Visual Studio 2008 ortamının servis kütüphanesinin çalıştırılması sonrası gerçekleştirilen işlemde budur. Bunun için WcfSvcHost aracını aşağıdaki gibi kullanmak yeterlidir.

WcfSvcHost /service:GenelIslemler.dll /config:GenelIslemler.dll.config
/client:WcfTestClient /clientargs:http://localhost:45000/

![mk245_10.gif](images/mk245_10.gif)

Client parametresinden sonra istemci uygulama olarak WcfTestClient işaret edilmektedir. Bununla birlikte WcfTestClient çalışırken gerekli olan base address bilgiside clientargs parametresinden sonra belirtilmektedir. Bu işlemin ardından önce WcfSvcHost uygulaması, sonrasında ise WcfTestClient uygulaması çalışacak ve hizmetler başlatılarak istemci tarafından kullanılabilir hale getirilecektir. WcfTestClient aracının yararlı özelliklerinden biriside istemci için üretilen konfigurasyon dosyasını gösteriyor olmasıdır. Dolayısıyla bu config dosyası istenirse gerçek istemci uygulamalar içinde kullanılabilir. Örnekte bu içerik config isimli sekme altında yer almaktadır.

![mk245_11.gif](images/mk245_11.gif)

Örnekte yer alan servis kütüphanesinde (WCF Service Library), hem TCP hemde WS bazlı EndPoint noktaları tanımlanmış olduğundan istemci için üretilen config dosyası içerisinde iki farklı binding elementinin yer aldığı açık bir şekilde görülmektedir.

WCF istemcileri geliştirilirken önemli olan noktalardan biriside Proxy üretimidir. Bilindiği gibi Proxy nesneleri yardımıyla istemci tarafından servis operasyonlarına erişebilmek nesne-metod (Object-Method) ilişkisi çerçevesinde olabilmektedir. Proxy üretimi için Visual Studio 2008 tarafında servis referanslarını eklemek gerekmektedir. Bu amaçla Add Service Reference seçeneği kullanılmaktadır. (Bu seçenek zaten WCF servis eklentilerinin Visual Studio 2005 yüklemesinden sonrada çıkmaktadır.)

![mk245_13.gif](images/mk245_13.gif)

Örnekte yer alan servis kütüphanesine ait servis referansını eklemek için öncelikli olarak WcfSvcHost uygulamasının çalıştırılması gerekmektedir. Bu işlemin ardından base address üzerinden WSDL içeriği talep edilebilir. Sonuç olarak basit bir Console uygulamasına Add Service Reference seçeneği yardımıyla, GenelIslemler.dll WCF Service Library projesi içerisinde tanımlanmış olan servis aşağıdaki ekran görüntüsünde olduğu gibi eklenebilir.

![mk245_12.gif](images/mk245_12.gif)

Görüldüğü gibi WCFSvcHost aracı ile çalıştırılan servise ait operasyonlar elde edilebilmektedir. Burada önemli olan fark Advanced sekmesine geçildiğinde servis ile ilişkili detaylı ayarlamaların yapılabileceği bir ekranla karşılaşılmasıdır.

![mk245_14.gif](images/mk245_14.gif)

Access level for generated classes seçeneği kullanıldığında, proxy içerisinde üretilecek olan tiplerin hangi erişim belirleyicisi (Access Modifier) ile oluşturulacağı belirtilmektedir. Tip bazında Public ve Internal olmak üzere iki farklı seçenek yer almaktadır. Bazı durumlarda başka bir servis kütüphanesinin (Söz gelimi başka bir WCF Servis kütüphanesi) elde ettiği proxy tiplerinin sadece o assembly içerisinde geçerli olması istenebilir. Böyle bir vakada Internal seçeneğini işaretlemek gerekmektedir. İkinci olarak Generate Asynchronous Operations seçeneği işaretlendiğinde servis operasyonlarını asenkron olarak çağırabilmek için gerekli fonksiyonel alt yapının yüklenmesi sağlanmaktadır.

> Bazı durumlarda servis operasyonlarının uzun zaman alması söz konusudur. Bu gibi vakalarda istemcilerin operayon sonuçlarını beklemeden çalışmasına devam edebilmesi için bilinen asenkron çağırma tekniklerinden (Polling, WaitHandle, Callbak, Even-Based) yararlanılmaktadır.

Yazının bu bölümünde, Console tipindeki istemci uygulamaya eklenecek olan servis referansı için Internal erişim belirleyicisi ve asenkron erişim seçeneği uygulanmaktadır. Buna göre istemci tarafına eklenen tiplerin sınıf diagramındaki (Class Diagram) görüntüsü ve içerikleri aşağıda olduğu gibidir.

![mk245_15.gif](images/mk245_15.gif)

Hatırlanacağı gibi Xml Web Servisleri, Visual Studio 2005 sürümü ile birlikte olay tabanlı asenkron çağrma (Event-Based Asynchronous Invoking) modelini uygulamaya başlamıştır. Bu basit asenkron modelde, zaman alan servis operasyonları işlemlerini tamamladığında otomatik olarak tetiklenen bir olay (Event) sayesinde sonuçlar uygulama ortamına alınabilmektedir. Böylece istemci tarafında BeginInvoke, EndInvoke gibi metodların kullanıldığı Polling, Callback,WaitHandle gibi tekniklerde görece daha basit bir uygulama şekli ortaya çıkmaktaydı. Modelin uygulanması sırasında istemci tarafında bazı olay (Event) ve temsilci (Delegate) tanımlamaları oluşturulmaktadır. Aynı durum Visual Studio 2008 sayesinde WCF servislerinede kolay bir şekilde uygulanabilir.

AdventureSrvClient isimli proxy sınıfına bakıldığında Topla ve UrunBul isimli metodlar için Async son eki ile biten versiyonlar yer almaktadır. Tahmin edileceği üzere bu metodlar yardımıyla ilgili servis operasyonlarına asenkron çağırlar gerçekleştirilebilir. Diğer taraftan işlemler tamamlandıktan sonra devreye girecek olan olaylar ise ToplaCompleted ve UrunBulCompleted olarak eklenmiştir (Completed kelimesi ile bittiklerine dikkat edilmelidir). Söz konusu olaylar devreye girdiğinde, operasyon sonuçlarını alabilmek içinse CompletedEventArgs kelimesi ile biten argüman sınıflarının oluşturulduğu görülebilir. İstemci tarafına yazılacak aşağıdaki kod parçası yardımıyla servis metodunun olay tabanlı (Event Based) olacak şekilde asenkron olarak çalıştırılması sağlanabilir.

```csharp
using System;
using System.Threading;

namespace Istemci
{
    class Program
    {
        static void Main(string[] args)
        {
            // Proxy nesnesi AdvTcpEndPoint isimli EndPoint noktasını baz alacak şekilde oluşturulur.
            AdvService.AdventureSrvClient proxy = new Istemci.AdvService.AdventureSrvClient("AdvTcpEndPoint");
            // UrunBul metodu tamamlandıktan sonra devreye girecek olan UrunBulCompleted olayı yüklenir
            proxy.UrunBulCompleted += new EventHandler<Istemci.AdvService.UrunBulCompletedEventArgs>(proxy_UrunBulCompleted);
            proxy.UrunBulAsync(1); // UrunBul metoduna yapılan asenkron çağrı

            // Console tarafında sembolik olarak yazılmış kodlar
            // bu döngü devam ederken servis tarafındaki metodun sonuçlarının alınmasıyla birlikte proxy_UrunBulCompleted isimli olay metodu tetiklenmektedir.
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("...");
                Thread.Sleep(1000); // Olayı daha iyi izlemek için ana thread 1 saniye duraksatılır
            }
        }

        // Asenkron olarak çağırılan servis metodu bittiğinde devreye girecek olan olay metodu
        static void proxy_UrunBulCompleted(object sender, Istemci.AdvService.UrunBulCompletedEventArgs e)
        {
            // UrunBul metodu geriye Urun tipinden nesne örneği döndürmektedir. Bu sebepten UrunBulCompletedEventArgs sınıfının Result özelliği Urun tipinden bir değer döndürür. Bu sayede Ad ve Fiyat alanlarına erişilebilmektedir.
            Console.WriteLine(e.Result.Ad+" "+e.Result.Fiyat.ToString("C2"));
        }
    }
}
```

Elbette istenirse Callback, WaitHandle ve Polling modellerine göre asenkron erişimlerde gerçekleştirilebilir. Nitekim Begin ve End kelimeleri ile başlayan metodların eklenmelerinin nedenide budur. Begin ve End metodlarının oluşturduğu sıkıntılardan biriside özellikle Windows veya WPF uygulamalarında ortaya çıkartacağı thread senkronizasyon problemleridir. Meşhur Illegal Cross Thread Exception oluşumuna neden olabilecek bu durumda tedbir olarak metod invoker'lardan yararlanılmaktadır. Bununla birlikte proxy'nin türedigi ClientBase abstract sınıfı kendi içerisinde potected erişim belirleyicisi ile işaretlenmiş InvokeAsync isimli bir metod içermektedir. Bu metod senkronize problemlerini ortadan kaldırmak üzere proxy sınıfı içerisindeki olay tabanlı asenkron fonksiyonlar tarafından kullanılmaktadır.

![mk245_17.gif](images/mk245_17.gif)

Dikkat edilecek olursa UrunBulAsync metodu son olarak ClientBase içerisindeki InvokeAsync metodunu çalıştırmaktadır. Bu metodun parametrik yapısı aşağıdaki gibidir ve istemci tarafında threadlerin senkronize çalışmasını sağlayacak şekilde tasarlanmıştır.

```csharp
protected void InvokeAsync(
    ClientBase<TChannel>.BeginOperationDelegate beginOperationDelegate
    , object[] inValues
    , ClientBase<TChannel>.EndOperationDelegate endOperationDelegate
    , SendOrPostCallback operationCompletedCallback
    , object userState
);
```

Özellikle SendOrPostCallback isimli temsilci (delegate) senkronizasyon ile ilişkili geri bildirim metodunu işaret etmekle görevlidir.

Advanced sekmesinde dikkati çeken noktalardan bir diğeride servis operasyonlarında koleksiyon bazlı dönen türlerin istemci tarafında ele alınış şekillerinin değiştirilebilmesidir. Nitekim şu anda servis referansı eklenmiştir. Var olan referansa ait özellikle nasıl değiştirilebilir? Yine Visual Studio 2008 ile gelen yeniliklerden biriside Configure Service Reference seçeneğidir.

![mk245_18.gif](images/mk245_18.gif)

Bu seçenek sayesinde, daha önceden eklenmiş olan bir servis referansına ait özellikler değiştirilebilmektedir. Koleksiyon döndüren opersayonların istemci tarafındaki durumunu daha net kavrayabilmek için servis tarafına eklenmiş aşağıdaki operasyon ve metodlar göz önüne alınabilir.

```csharp
[ServiceContract]
interface IAdventureSrv
{
    // Diğer tanımlamalar
    [OperationContract]
    List<double> RastgeleSayilar(int baslangic, int bitis);

    [OperationContract]
    Hashtable Isimler();
}

class AdventureSrv
    :IAdventureSrv
{
    // Diğer metod uyarlamaları

    public List<double> RastgeleSayilar(int baslangic,int bitis)
    {
        List<double> liste = new List<double>();
        Random rnd=new Random();
        for (int i = baslangic; i < bitis; i++)
            liste.Add(rnd.NextDouble());
        return liste;
    }

    public Hashtable Isimler()
    {
        Hashtable dict = new Hashtable();
        dict.Add(1001, "Burak");
        dict.Add(1002, "Mayk");
        dict.Add(1005, "Conn");
        return dict;
    }
}
```

Servis tarafında List ve Hashtable tiplerinden değer döndüren iki operasyon yer almaktadır. Normal şartlarda WCF servisine ait proxy sınıfı üretilirken List gibi koleksiyonlar için T tipinden bir dizi (Array) baz alınmaktadır. Hashtable, SortedList gibi koleksiyonlar içinse Dictionary tipi ele alınmaktadır. Bu nedenle istemci uygulamadaki servis içeriği Update Service Reference seçeneği ile güncelleştirilirse Isimler ve RastgeleSayilar adlı fonksiyonların aktarımı aşağıdaki gibi olacaktır.

```csharp
public System.Collections.Generic.Dictionary<object, object> Isimler() {
    return base.Channel.Isimler();
}

public double[] RastgeleSayilar(int baslangic, int bitis) {
    return base.Channel.RastgeleSayilar(baslangic, bitis);
}
```

Ancak istenirse bu aktarım tipleri değiştirilebilir. Bunun için Advanced sekmesinde yer alan Collection Type ve Dictionary Collection Type değerlerini değiştirmek yeterli olacaktır. Aşağıdaki ekran görüntüsünde bu durum gösterilmektedir.

![mk245_19.gif](images/mk245_19.gif)

Bu işlemin ardından ilgili operasyonların Proxy sınıfı içerisindeki yapılarının aşağıdaki gibi değiştirildiği görülebilir.

```csharp
public System.Collections.Generic.List<double> RastgeleSayilar(int baslangic, int bitis) {
    return base.Channel.RastgeleSayilar(baslangic, bitis);
}

public System.Collections.Hashtable Isimler() {
    return base.Channel.Isimler();
}
```

Dikkat edileceği üzere, servis tarafında sunulan operasyonlardaki dönüş tiplerinin aynen istemci tarafındaki proxy sınıfınada aktarılması sağlanmaktadır. Tabi bu noktada istemci tarafındaki proxy sınıfının.Net bağımlı hale geldiğinede dikkat etmek gerekmektedir.

Visual Studio 2008 geliştirme ortamı.Net Framework 3.5 şablonu altında birden fazla WCF kütüphanesi sunmaktadır.

![mk245_20.gif](images/mk245_20.gif)

WCF servisleri, Workflow Foundation içerisindede zaman zaman kullanılmaktadır. Workflow Foundation mimarisinde iki farklı iş akışı tipi vardır. Sequential ve State Machine. Bir WCF servisi bu tipteki iş akışları içerisinden kullanılabilir yada iş akışı bir WCF servisinin parçası haline gelebilir. Bu noktada Sequential Workflow Service Library ve State Machine Workflow Service Library şablonları kullanılabilir. Bilindiği gibi.Net Framework 3.5 ile WCF tarafında RSS, Atom formatlı yayınlamalara destek verilmektedir. Bu tip bir proje şablonu için Syndication Service Library kullanılabilir.

Buraya kadar anlatılanlara göre Visual Studio 2008 ile birlikte gelen WCF yenilikler aşağıdaki tablo ile özetlenebilirler.

Özellik
Açıklama

WcfSvcHost
Herhangibir Host uygulaması yazılmasına gerek kalmadan WCF servis kütüphaneleri test amaçlı olarak yayınlanabilmektedir. Visual Studio 2008 varsayılan olarak WCF Servis kütüphaneleri için bu aracı kullanmaktadır.

WcfTestClient
Yayınlanan servislerin test edilmesi için kullanılan Windows uygulamasıdır. Servise ait operasyonların anında görülmesi, kullanılması, talep (Request) ve cevap (Response) paketlerinin data veya XML formatında okunabilmesi, farklı EndPoint noktalarının test edilebilmesi gibi imkanlar sunmaktadır. Visual Studio 2008 varsayılan olarak WCF Servis kütüphanelerinin çalıştırılmasında WcfSvcHost uygulamasından sonra bu programı çalıştırırak anında testin yapılabilmesini sağlamaktadır.

Add Service Reference - Advanced Sekmesi
Advanced sekmesindeki ayarlar yardımıyla oluşturulacak proxy sınıfı ve ilişkili tiplerine ait pek çok detay ayarlanabilir.

Public/Internal
İstemciye eklenen servis referansı içerisindeki tiplerin erişim belirleyicileri (Access Modifiers) vakaya göre public yada internal olarak set edilebilir.

Koleksiyon Eşleştirme
List gibi koleksiyonlar (IEnumerable, IList vb...) istemci tarafında T[] dizileri şeklinde ele alınırlarken, Hashtable gibi koleksiyonlar Dictionary olarak yorumlanır. Bu eşleştirme Advanced sekmesindeki Collection Type ve Dictionary Collection Type seçenekleri yardımıyla değiştirilebilir.

Asenkron Proxy Metodların Üretimi
Web servislerindeki hazır üretime benzer şekilde olay tabanlı asenkron (event based asynchronous) yürütme desenlerinin eklenmesi sağlanabilir. Üstelik bu uyarlamada thread senkronizasyonunu etkili hale getirmeyi kolaylaştırıcı fonksiyonellikler vardır.

Configure Service Reference
İstemci tarafına eklenmiş olan bir servisin koleksiyon eşleştirme, erişim belirleyicisi gibi pek çok özelliği sonradan Configure Service Reference seçeneği yardımıyla değiştirilebilir.

Yeni Library Şablonları
Özellikle WWF (Windows Workflow Foundation) tarafına ve.Net Framework 3.5 ile WCF'e getirilen Web programlama modelinin bir ürünü olan Syndication yayınlama için ek proje şablonları gelmektedir.

Böylece geldik bir makalemizin daha sonuna. Bu makalemizde yakın zamanda son haliyle yayınlanan Visual Studio 2008 ürününün Windows Communication Foundation için getirdiği yenilikleri basit seviyede ele almaya çalıştık. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](assets/VS2008Yenilikler.rar)
