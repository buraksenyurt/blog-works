---
title: "WCF - Istemci Taraflı Asenkron Çağırma (Client Side Asynchronous Invoking)"
pubDate: 2007-06-13 12:00:00
categories:
  - WCF
tags:
  - WCF
  - C#
  - Bash
  - XML
  - .NET
  - HTTP
  - Async/Await
  - Threading
  - Delegates
  - Visual Studio
---

# WCF - Istemci Taraflı Asenkron Çağırma (Client Side Asynchronous Invoking)
Değerli Okurlarım Merhabalar,

Windows Communication Foundation ile ilgili bir önceki makalemizde One Way tekniğini uygulayarak istemcilerin asenkron olarak uzak metodları nasıl çağırabileceklerini incelemiştik. One Way tekniğinin elbetteki en büyük dezavantajı geriye değer döndüren metodların ele alınamayışıdır. Oysaki çoğu zaman, işlem süresi uzun zaman alabilecek metodların geriye değer döndürdüğü vakkalarda asenkron erişim tekniklerini kullanmak gerekir. Ancak Windows Communication Foundation göz önüne alındığında asenkron çalıştırma iki farklı şekilde ele alınabilmektedir. Bunlarda birisi istemci taraflı asenkron çağırma (Client Side Asynchronous Invoking) modelidir. Diğeri ise servis taraflı asenkron uyarlama modelidir (Service Side Asynchronous Implementation). Bu makalemizde istemci taraflı asenkron çağırma modelini incelemeye çalışacağız.

İstemci taraflı asenkron çağırma modelinde, proxy sınıfının asenkron desene (Asynchronous Pattern) uygun olacak şekilde Begin ve End ile başlayan standart metodları vardır. Bu metodlar temelinde IAsyncResult arayüzünü ele almaktadır. İstemci taraflı olarak çalışan bu modelin, doğal olarak farklı uygulanabilme çeşitleri vardır..Net tarafında asenkron mimarinin ele alınabilen tüm teknikleri Windows Communication Foundation içinde geçerlidir. Burada bahsedilen teknikler Polling, Callback ve WaitHandle modellerini içermektedir. WaitHandle modelininde kendi içerisinde WaitOne, WaitAny, WaitAll gibi farklı kullanım şekilleri vardır.

Bu modellerin temel etkilerini ve farklılıklarını yazacağımız örnek kod parçaları üzerinde Windows Communication Foundation açısından incelemeye çalışacağız. Ancak başlamadan önce servis taraflı asenkron uyarlama modelinide açıklamakta fayda olacağı kanısındayım. Servis tarafında gerçekleştirilen asenkron uyarlama tekniği istemcinin metodları asenkron olarak işletmesi anlamına gelmemektedir. Bir başka deyişle istemci yine çağırdığı metodu senkronmuş gibi ele alır yani ilerleyebilmek için metodun sonucunun gelmesini bekler. Metodun asenkron olarak çalıştığı yer servis tarafıdır. Aslında servis tarafındaki ilgili sürecin başka bir thread üzerine yıkıldığı düşünülebilir. Ne varki bu model kodlanması zor bir teknik içermektedir. Nitekim servis tarafında asenkron olarak ele alınmak istenen metodların asenkron tasarım desenine göre yazılması gerekmektedir. Az öncede belirttiğimiz üzere söz konusu modeli bir sonraki makalemizde incelemeye çalışacağız.

Dilerseniz örneğimize geçerek işlemlerimize başlayalım. Her zamanki gibi servis tarafında yayınlacak olan fonksiyonellikleri içeren tip ve sözleşme (contract) tanımlamalarını içeren bir WCF Class Library projesi geliştirerek işe başlanabilir. Söz konusu kütüphane içerisindeki sözleşme arayüzü (Interface) ve fonksiyonellikleri içeren sınıf (Class) aşağıdaki gibidir.

![mk208_8.gif](images/mk208_8.gif)

IAdventureManager isimli arayüz içeriği aşağıdaki gibidir.

```csharp
[ServiceContract(Name="AdventureContract",Namespace= "http://www.bsenyurt.com/2007/6/6/AdventureService")]
public interface IAdventureManager
{
    [OperationContract(Name="AverageListPriceByCategory")]
    double AverageListPrice(int subCatId);

    [OperationContract(Name="TotalListPriceByCategory")]
    double TotalListPrice(int subCatId);

    [OperationContract(Name = "GetProductsCountByCategory")]
    int ProductCount(int subCatId);
}
```

AdventureManager isimli sınıfın içeriği aşağıdaki gibidir.

```csharp
public class AdventureManager:IAdventureManager
{
    #region IAdventureManager Members

    public double AverageListPrice(int subCatId)
    {
        Thread.Sleep(5000);
        return 1000;
    }    
    public double TotalListPrice(int subCatId)
    {
        Thread.Sleep(3000);
        return 4500;
    }
    public int ProductCount(int subCatId)
    {
        Thread.Sleep(7000);
        return 504;
    }

    #endregion
}
```

AdventureManager isimli sınıf içerisinde yer alan metodlarda bilinçli olarak Thread sınıfının static Sleep fonksiyonundan faydalanılarak farklı sürelerde duraksatmalar yapılmaktadır. Söz konusu metodlardan sembolik olarak double ve int gibi tiplerden değerler döndürülmektedir. Geliştirilen WCF sınıf kütüphanesini kullanacak olan servis tarafını yine bir Windows uygulaması olarak tasarlayabiliriz. (Windows uygulamasının System.ServiceModel.dll ve AdventureLib isimli WCF Sınıf kütüphanelerini referans etmesi gerektiğini unutmayalım.)

Windows uygulamasının form tasarımı basit olarak aşağıdaki gibidir;

![mk208_9.gif](images/mk208_9.gif)

Windows uygulamasına ait kodlar aşağıdaki gibidir;

```csharp
ServiceHost host;

private void btnStartService_Click(object sender, EventArgs e)
{
    host = new ServiceHost(typeof(AdventureManager)); 
    host.Open();
    lblStatus.Text = host.State.ToString();
}

private void btnStopService_Click(object sender, EventArgs e)
{
    host.Close();
    lblStatus.Text = host.State.ToString();
}
```

Windows uygulamasına ait app.config dosyasının içeriği aşağıdaki gibidir;

```csharp
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <services>
            <service name="AdventureLib.AdventureManager">
                <endpoint address= "net.tcp://localhost:9001/AdventureServices.svc" binding="netTcpBinding" bindingConfiguration="" name="AdventureEndPoint" contract="AdventureLib.IAdventureManager" />
            </service>
        </services>
    </system.serviceModel>
</configuration>
```

Servis TcpBinding tipine göre geliştirilmiştir. Bu nedenle net.tcp://localhost:9001/AdventureServices.svc isimli örnek adres üzerinden sunulmaktadır. Söz konusu konfigurasyon dosyası her zamanki gibi Microsoft Service Configuration Editor yardımıyla Visual Studio 2005 ortamında daha kolay bir şekilde yazılabilir.

Gelelim istemci tarafına. Öncelikli olarak svcutil.exe aracını kullanarak AdventureLib isimli sınıf kütüphanesinden, istemci için gerekli proxy sınıfı ve konfigurasyon dosyasının üretilmesi gerekmektedir. Burada daha önceki örneklerden farklı olarak asenkron desene uygun olacak şekilde metod üretimlerinin yapılması gerekmektedir. Svcutil aracının /async (veya kısaltmalı olarak /a) isimli parametresi bu işi otomatik olarak yapmaktadır. Bu nedenle svcutil aracının aşağıdaki şekilde kullanıması gerekmektedir.

Öncelikli olarak proxy üretimi için gerekli olan wsdl ve schema dosyalarının üretilmesi sağlamak adına aşağıdaki komut kullanılmalıdır.

```bash
svcutil AdventureLib.dll
```

Sonrasında ise svcutil aracı aşağıdaki haliyle çalıştırılmalıdır.

```bash
svcutil www.bsenyurt.com.2007.6.6.AdventureService.wsdl *.xsd /out:AdventureProxy.cs /async
```

![mk208_1.gif](images/mk208_1.gif)

Sonuç olarak üretilen AdventureProxy.cs isimli sınıfın içeriğine aşağıdaki ekran görüntüsünde olduğu gibi, Begin ve End ile başlayan standart asenkron metodlar ilave edilmiş olur. Dikkat edilecek olursa AdventureManager sınıfı içerisindeki tüm metodların hem normal hemde Begin ve End ile başlayan versiyonları ilave edilmiştir.

![mk208_2.gif](images/mk208_2.gif)

Oluşan bu sınıf içerisinden örnek olarak AverageListPriceByCategory isimli fonksiyon için yazılmış asenkron metodlar göz önüne alınabilir.

```csharp
public System.IAsyncResult BeginAverageListPriceByCategory(int subCatId, System.AsyncCallback callback, object asyncState)
{
    return base.Channel.BeginAverageListPriceByCategory(subCatId, callback, asyncState);
}

public double EndAverageListPriceByCategory(System.IAsyncResult result)
{
    return base.Channel.EndAverageListPriceByCategory(result);
}
```

Burada tipik olarak asenkron tasarım desenine uygun metodlar yer almaktadır. BeginAverageListPriceByCategory metodu Polling, Callback ve WaitHandle modellerine destek verecek şekilde IAsyncResult arayüzünün (Interface) taşıyabileceği bir referansı döndürür. Kullanılan modele göre, asenkron çalışan fonksiyonun sonuçlarını almak için EndAverageListPriceByCategory metodu kullanılır. Dikkat edilecek olursa bu metod parametre olarak IAsyncResult arayüzünden bir referans kabul eder ve geriye uygun olan metod çıktısını döndürür. Metod içerikleri asenkron deseni otomatik olarak uygulamaktadır. Bir başka deyişe nesne kullanıcısı (object user) söz konusu modelin içerisindeki detaylar ile ilgilenmez. Sadece uygun olan veya istediği asenkron modeli istemci programa uyarlar.

Şimdi tek tek istemci tarafından asenkron çağırma modellerini uygulamaya çalışalım. İstemci basit bir Console uygulaması olarak ele alınmıştır. Söz konusu uygulamanın konfigurasyon dosyasının içeriği aşağıdaki gibi geliştirilebilir. (İstemci uygulamanında System.ServiceModel.dll assembly'ını referans etmesi gerektiğini unutmayalım.)

İstemci uygulama tarafında ele alınacak app.config içeriği aşağıdaki gibidir.

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <system.serviceModel>
        <bindings/>
        <client>
            <endpoint address= "net.tcp://localhost:9001/AdventureServices.svc" binding="netTcpBinding" bindingConfiguration="" contract="AdventureContract" name="AdventureClientEndPoint" />
        </client>
    </system.serviceModel>
</configuration>
```

Başlamadan önce senkron çalışmanın nasıl bir etkisi olacağını görmekte fayda vardır. Bu amaçla ilk kodlar aşağıdaki gibi geliştirilebilir.

Senkron çalışma durumu;

```csharp
Console.WriteLine("Teste başlamak için bir tuşa basınız");
Console.ReadLine();
AdventureContractClient srv = new AdventureContractClient("AdventureClientEndPoint");

#region Senkron Çalışma

DateTime baslangic = DateTime.Now;

double ortalamaFiyat=srv.AverageListPriceByCategory(1);

DateTime bitis = DateTime.Now;

Console.WriteLine(ortalamaFiyat.ToString("C2"));
TimeSpan fark = bitis - baslangic;
Console.WriteLine("Geçen süre yaklaşık olarak {0} saniyedir.",fark.TotalSeconds.ToString());
Console.ReadLine();

#endregion
```

Burada dikkat edilecek olursa AverageListPriceByCategory metodu çağırıldıktan sonra istemci uygulama bir süreliğine beklemede kalacaktır. Bu TimeSpan ile elde edilen süre farkından da açıkça görülmektedir. Bir başka deyişle istemci uygulamadaki kod akışı ortalamaFiyat değerinin elde edilmesini bekleyecek ve sonuç alındıktan sonra devam edecektir. Program kodu bu haliyle çalıştırıldığında aşağıdakine benzer bir ekran görüntüsü elde edilir.

![mk208_3.gif](images/mk208_3.gif)

Bu tipik olarak senkron çalışma şeklidir. Gelelim diğer tekniklere. İlk olarak WaitHandle modelini ele alalım. Bu model temel olarak asenkron olarak çalıştırılan metodların, uygulamanın belirli bir noktasında sonuçları alınıncaya kadar farklı şekillerde beklenilmesini sağlamaktadır. Dikkat ederseniz metodlar yine asenkron olarak başlatılır ama programın herhangibir noktasında sonuçlarının ortama dönmesi için beklenir. Bu model daha çok asenkron çalışan metodların sonuçlarının uygulamanın belirli bir noktasında girdi olarak kullanılması gerektiği durumlarda işe yaramaktadır. Söz konusu modelin WaitOne, WaitAll ve WaitAny gibi üç farklı uygulanış biçimi vardır. WaitOne tekniği adındanda anlaşılacağı üzere sadece tek bir asenkron metodun sonucunun alınması için bir duraksatma gerçekleştirir. Aşağıdaki örnek kod parçasında WaitOne tekniğinin uygulanış biçimi yer almaktadır.

WaitOne tekniği;

```csharp
IAsyncResult iar=srv.BeginAverageListPriceByCategory(1, null, null);
Console.WriteLine("Bazı işlemler yapılıyor...");

iar.AsyncWaitHandle.WaitOne();

double sonuc=srv.EndAverageListPriceByCategory(iar);
Console.WriteLine(sonuc.ToString("C2"));

Console.ReadLine();
```

Uygulama bu haliyle çalıştırıldığında aşağıdaki ekran görüntüsü elde edilir.

![mk208_4.gif](images/mk208_4.gif)

Görüldüğü gibi BeginAverageListPriceByCategory çağrısından sonra uygulama hemen alttaki satırdan çalışmaya devam etmiştir. Burası tamamen sembolik bir kod parçası içerir. Çok doğal olarak burada farklı işlemler gerçekleştirilmesi veya istemci tarafında yer alacak başka fonksiyonelliklerin ele alınması muhtemeldir. Sonrasında ise iar isimli IAsyncResult referansının AsyncWaitHandle özelliği ile yakalanan WaitHandle nesne örneğinin WaitOne metodu çağırılır. Bu metod, çalıştırılan asenkron fonksiyonun sonucu alınana kadar uygulamanın duraksatılmasını sağlar. WaitOne satırı aşılır aşılmaz artık ilgili metodun sonuçları uygulama ortamına derhal alınabilir. Bu nedenle EndAverageListPriceByCategory metodunun çağırılması ve parametre olarak iar isimli IAsyncResult arayüzünün verilmesi yeterlidir.

Her zaman için istemci tarafından çağırılacak tek bir asenkron metod olması söz konusu değildir. Bazı durumlarda birden fazla metod çağrısı asenkron olarak yürütülmek istenebilir. Çok doğal olarak bu metodlarının tamamının, yine uygulamanın belirli bir noktasında girdi olarak kullanılacak dönüş değerleri olabilir. Öyleyse programın bu ilgili noktasında asenkron olarak çalışan tüm metodların tamamının duraksatılması istenebilir. Bunun için WaitAll tekniği aşağıdaki gibi kullanılır.

WaitAll tekniği;

```csharp
IAsyncResult iar1 = srv.BeginAverageListPriceByCategory(1, null, null);
IAsyncResult iar2 = srv.BeginGetProductsCountByCategory(2, null, null);
IAsyncResult iar3 = srv.BeginTotalListPriceByCategory(5, null, null);

WaitHandle[] handles = new WaitHandle[] { iar1.AsyncWaitHandle, iar2.AsyncWaitHandle,iar3.AsyncWaitHandle };

Console.WriteLine("Bazı işlemler yapılıyor...");

WaitHandle.WaitAll(handles);

double sonuc1 = srv.EndAverageListPriceByCategory(iar1);
int sonuc2 = srv.EndGetProductsCountByCategory(iar2);
double sonuc3 = srv.EndTotalListPriceByCategory(iar3);

Console.WriteLine(sonuc1.ToString("C2"));
Console.WriteLine(sonuc2.ToString());
Console.WriteLine(sonuc3.ToString("C2"));

Console.ReadLine();
```

WaitAll tekniğinde asenkron olarak çalıştırılan metodlardan sorumlu IAsyncResult referanslarından elde edilen her bir WaitHandle örneği bir dizi içerisinde toplanır. Söz konusu dizi WaitHandle sınıfının static WaitAll metoduna devredildiği satırda uygulama tüm asenkron metodların sonuçlarının gelmesi için beklemede kalacaktır. Çok doğal olarak bu çağrıya kadar yapılan tüm işlemler, asenkron metodlarınki ile paralel olarak yürütülmektedir. Bu program kodlarının çalışma zamanında üreteceği çıktı aşağıdaki gibi olacaktır.

![mk208_5.gif](images/mk208_5.gif)

WaitAny modeli asenkron olarak çalışan metodlardan tamamlanını ortama iade edebilme ilkesine dayalı olarak çalışmaktadır. Söz gelimi örnek servisimizdeki metodlar göz önüne alındığında teorik olarak en kısa sürede biten metodun sonucunun ortama alınabilmesi ve sonrasında diğerleri için beklennmesi gerekir. Metod sonuçları ortama döndükçe asenkron işleyişler tamamlanmış olacaktır.

> WaitAny modelini WCF içerisinde uyguladığımızda çalışma zamanındaObjectDisposedException istisnası (Exception) alınmaktadır. Bu istisnanın sebebi araştırıldığında, makalenin yazıldığı tarih itibariyle çok kıstılı bilgiye ulaşılmaktadır. Bir çözüm [Erwyn van der Meer](http://bloggingabout.net/blogs/erwyn/archive/2006/12/09/WCF-Service-Proxy-Helper.aspx) tarafından geliştirilmiştir ve değerlendirilebilir. Buna göre Dispose edilemeyen servis nesnesi için ekstradan bir sınıf geliştirilmiş ve bu sınıfın proxy sınıfı yerine kullanılması önerilmiştir.

İstemci tarafı için asenkron çağırma tekniklerinden biriside Polling ' dir. Bu modelde temel olarak asenkron olarak başlatılan işlemin tamamlanıp tamamlanmadığı kontrol edilir ve bu aralıktaki tüm işlemler paralel olarak işletilir. Polling modelinde, asenkron işleyişin tamamlanmadığını kontrol etmek adına IAsyncResult arayüzünün IsCompleted isimli özelliğinden yararlanılır.

Polling tekniği;

```csharp
IAsyncResult iar=srv.BeginTotalListPriceByCategory(4, null, null);

while (!iar.IsCompleted)
{
    Console.WriteLine("İşlemler devam ediyor");
    Thread.Sleep(1000);
}

double sonuc = srv.EndTotalListPriceByCategory(iar);

Console.WriteLine(sonuc.ToString());
Console.ReadLine();
```

Kodlar bu haliyle çalıştırıldığında aşağıdaki ekran görüntüsü elde edilir.

![mk208_6.gif](images/mk208_6.gif)

Buna göre while döngüsü içerisinde kodlar asenkron olarak yürütülen metoddan sonuç alınıncaya kadar devam edecektir.

Asenkron erişim teknikleri arasında en popüler olanlarından birisi Callback'tir. Bu teknikte asenkron çalışan metodun işleyişi tamamlandığında otomatik olarak bir geri bildirim fonksiyonu devreye girer ve sonuçların uygulama ortamına kolay bir şekilde alınabilmesi sağlanmış olur. Aşağıdaki kod parçasında Callback modelinin uygulanış biçimi gösterilmektedir.

Callback Modeli;

```csharp
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Teste başlamak için bir tuşa basınız");
        Console.ReadLine();
        AdventureContractClient srv = new AdventureContractClient("AdventureClientEndPoint");

        #region Callback Ornek
    
        IAsyncResult iar = srv.BeginAverageListPriceByCategory(3, new AsyncCallback(CallbackMetod), srv);

        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine("İşlemler devam ediyor");
            Thread.Sleep(1000);
        }

        Console.ReadLine();
    
        #endregion
    }

    static void CallbackMetod(IAsyncResult iar)
    {
        AdventureContractClient srv = (AdventureContractClient)iar.AsyncState;
        double sonuc=srv.EndAverageListPriceByCategory(iar);
        Console.WriteLine(sonuc.ToString());
    }
}
```

Callback modelinde kritik olan nokta Begin... metodunun aldığı AsyncCallback tipindeki parametredir. AsyncCallback.Net içerisinde yer alan bir temsilcidir (delegate). Bu temsilcinin temel görevi ise çalışma zamanında otomatik olarak çağırılacak geri bildirim metodunu işaret etmektir. Bir başka deyişle asenkron olarak çağırılan metod işleyişini tamamlandığında bu temsilcinin bildirdiği metod devreye girecektir. Sonuç itibariyle AsyncCallback bir temsilci olduğundan tanımında işaret edeceği metodun yapısıda belirtilmektedir. Buna göre geriye değer döndürmeyen ve IAsyncResult arayüzü tipinden referanslar alan metodlar işaret edilebilir.

Begin metdunun son parametresi object tipinden bir değer alır. Bu parametre çoğunlukla geri bildirim metoduna referans taşımak amacıyla kullanılır. Söz gelimi yukarıdaki örnek kod parçasında, End... metodunun çağırılabilmesi için iar üzerinden AsyncState ile elde edilen referans AdventureContractClient sınıfına cast edilmektedir. Burada AsyncState özelliğinin Begin... çağrısında kullanılan srv isimli referans olmasını sağlamak amacıyla son parametreye srv örneği verilmiştir. Uygulama çalıştığında paralel olarak yürüyen istemci kodları devam ederken aynen aşağıdaki ekran görüntüsünde olduğu gibi arada bir yerde, tamamlanan asenkron metodun sonucu otomatik olarak ortama alınabilmektedir.

![mk208_7.gif](images/mk208_7.gif)

Callback modelinde istenirse C# 2.0 ile birlikte gelen isimsiz metodlardan (Anonymous Methods) da yararlanılabilir. Bu sayede ekstradan Callback metodu yazılmasına gerek kalmamakta ve temsilcinin bağlandığı yerde geri bildirim kodları ele alınabilmektedir. Örneğin aşağıdaki kod parçasında bu işlemin nasıl yapılacağı gösterilmektedir.

Callback modelinde isimsiz metod kullanımı;

```csharp
AsyncCallback async = delegate(IAsyncResult ar)
                                {
                                    double sonuc = srv.EndAverageListPriceByCategory(ar);
                                    Console.WriteLine(sonuc.ToString());
                                };
IAsyncResult iar = srv.BeginAverageListPriceByCategory(3, async, null);

for (int i = 0; i < 10; i++)
{
    Console.WriteLine("İşlemler devam ediyor");
    Thread.Sleep(1000);
}

Console.ReadLine();
```

Bu seferki modelde ekstradan static (Console uygulamasındaki static Main metodundan çağırmamız nedeni ile böyle tanımlanmak zorundadır) olacak şekilde bir geri bildirim metodu yazılmasına gerek kalmamıştır. Bununla birlikte, Begin metodunun son paramertresi ile bir object referansı taşınmasına gerekte yoktur. Bu kod parçası çalıştırıldığında da benzer sonuçlar alınacaktır.

Bu makalemizde WCF için istemci taraflı asenkron çağırma modelini incelemeye çalıştık. Temel olarak kullanabileceğimiz üç modelden bahsettik. Bu modeller ile ilgili olarak kısaca aşağıdaki özet bilgileri söyleyebiliriz.

- Polling modelinde asenkron çağırılar sonucu çalışan metodların tamamlanıp tamamlanmadığı sürekli olarak kontrol edilir. Bu amaçla IsCompleted özelliği ele alınabilir. Bu kontrol aralığındaki tüm işlemler ilgili asenkron çağrılar ile paralel olarak yürümektedir.
- Callback modelinde asenkron olarak yapılan çağrılar ile çalışan metodların sonuçları elde edildiğinde, otomatik olarak bir geri bildirim metodu çalışır. Dolayısıyla asenkron yürüyen metodların tamamlanıp tamamlanmadıklarının sürekli olarak kontrol edilmesine gerek yoktur.
- WaitHandle modeli 3 farklı şekilde uygulanabilmekte olup asenkron olarak çalışan metodların uygulamanın belirli bir noktasında girdi olarak kullanılabilecek değerlerinin alınması için bekleme yapılmasını sağlamaktadır. Bu bekleme tek bir metod için WaitOne, tüm metodlar için WaitAll ve sırayla bitenleri ortama alma ilkesine dayanaraktan WaitAny fonksiyonları ile yapılmaktadır.

Böylece geldik bir makalemizin daha sonuna. Yazımızın başındada belirttiğimiz gibi bir sonraki Windows Communication Foundation makalemizde servis tarafında asenkron uyarlamanın (Service Side Asynchronous Implementation) nasıl yapılabileceğini incelemeye çalışacağız. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama İçin Tıklayınız.](assets/AsenkronErisimler.zip)
