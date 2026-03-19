---
layout: post
title: "WCF - InstanceContextMode"
date: 2007-05-24 12:00:00 +0300
description: "Windows Communication Foundation uygulamalarında istemciler başvurdukları servisler üzerindeki nesne örneklerini kullanırlar. Özellikle kullanılan bağlayıcının (binding) tipine göre servis üzerindeki nesne örneklerinin farklı şekillerde oluşturulup ele alınması söz konusudur. .Net Remoting ile uygulama yazan geliştiriciler, istemcilerin talepte bulunacağı uzak nesne örneklerinin farklı modellerde örneklendiklerini bilirler. Burada bahsi geçen modeller Server Activated Object için Singleton ve SingleCall ile Client Activated Object' dir. Örneğin CAO modeline göre istemciler, uzak nesneyi örneklediklerinde sunucu üzerinde bir referans oluşturulur ve istemciler bunu kullanır. Singleton ve SingleCall modelleri metod çağrıları sonucu referans oluşturulmasını sağlar. Ama Singleton modelinde her istemci için(dolayısıyla her metod çağrısı için) aynı nesne örneği, SingleCall' da ise her metod çağrısı için ayrı bir nesne örneği sunucu üzerinde oluşturulmaktadır."
categories:
  - wcf-webhttp-services
tags:
  - wcf-webhttp-services
  - csharp
  - xml
  - bash
  - wcf
  - http
  - threading
  - concurrency
  - thread-safety
---
# WCF - InstanceContextMode
Değerli Okurlarım Merhabalar,

Windows Communication Foundation uygulamalarında istemciler başvurdukları servisler üzerindeki nesne örneklerini kullanırlar. Özellikle kullanılan bağlayıcının (binding) tipine göre servis üzerindeki nesne örneklerinin farklı şekillerde oluşturulup ele alınması söz konusudur..Net Remoting ile uygulama yazan geliştiriciler, istemcilerin talepte bulunacağı uzak nesne örneklerinin farklı modellerde örneklendiklerini bilirler.

Burada bahsi geçen modeller Server Activated Object için Singleton ve SingleCall ile Client Activated Object'dir. Örneğin CAO modeline göre istemciler, uzak nesneyi örneklediklerinde sunucu üzerinde bir referans oluşturulur ve istemciler bunu kullanır. Singleton ve SingleCall modelleri metod çağrıları sonucu referans oluşturulmasını sağlar. Ama Singleton modelinde her istemci için (dolayısıyla her metod çağrısı için) aynı nesne örneği, SingleCall'da ise her metod çağrısı için ayrı bir nesne örneği sunucu üzerinde oluşturulmaktadır.

Benzer durumlar WCF servislerindeki nesne örnekleri içinde geçerlidir. İstemci uygulama ile servis örneği arasındaki ilişkiyi kontrol altına alabilmek için ServiceBehavior niteliğinin InstanceContextMode özelliğinden yararlanılır. InstanceContextMode özelliği InstanceContextMode enum sabiti tipinden bir değer almaktadır ve alabileceği değerler PerSession, PerCall ve Single'dır. Bu modlardan hangisinin aktif olduğu özellikle WCF uygulamalarında oturum yönetimi (Session Management) açısından da önemlidir.

Varsayılan olarak her istemci için sunucuda bir adet nesne örneği oluşturulur. Servis örneği istemciden gelen ilk operasyon çağrısında (yani ilk metod çağrısında) oluşur. İstemci sunucu ile olan bağlantısını kesene kadarda servis örneği sunucuda kalmaya devam eder. Ancak binlerce kullanıcının servise bağlandığı durumlarda da her istemci için bir servis örneğinin sunucuda oluşturulması ve ne zaman kalkacaklarının istemcinin hareketine bağlı olması sunucu belleğinin gereksiz yere şişirilmesi anlamına gelir. Dolayısıla diğer modeller göz önüne alınabilir.

> Binding Tipine göre Instance Mode türlerinden hangisinin uygulanabileceği değişir. Örneğin BasicHttpBinding, Http protokolünün doğası gereği iletişim seviyesinde (transport level) PerSession mode türünü desteklemez. Bir başka deyişle InstanceContectMode hangi değeri alırsa alsın varsayılan olarak PerCall modda çalışır. Buda gelecek her istemci operasyon çağrısı için servis tarafında bir nesne örneğinin oluşturulması ve operasyon tamamlandığında kaldırılmak üzere Garbage Collector'e devredilmesi anlamına gelir.

İlk olarak PerSession, PerCall ve Single modellerinin nasıl çalıştıklarını teorik olarak bilmekte fayda vardır.

PerSession Modeli;

PerSession modunda istemciler ilk operasyon çağrısında bulunduklarında servis örneği oluşturulur ve istemci Close metodunu kullanana kadar yada uygulamayı kapatana kadar söz konusu örnek sunucuda kalır. İstemci, bir servis örneğini elde ettikten sonra bu örnek sadece ilgili istemciye ait olacak şekilde tahsis edilir. Bir anlamda o istemci için bir oturum (Session) açılmış olur. Farklı istemcilerin aynı oturumu kullanmaları mümkün değildir. İstemci uygulamalarda çok kanallı (multithread) kod parçaları olabileceği düşünüldüğünde eşzamanlı olarak aynı oturuma ait metod çağrıları söz konusu olabilir. Varsayılan olarak bir talep tamamlanmadan başka bir talep gelirse öncekinin tamamlanması beklenmektedir. Ama istenirse bu davranış biçimide ServiceBehavior niteliğinin ConcurrencyMode özelliği ile değiştirilebilir. Yani istenirse eş zamanlı olarak çağrılara cevap verilmesi de sağlanabilir.

Bu mod kullanılırken, istemci tarafından çalıştırılabilecek operasyonlar için bir sıralama belirtilmek istenebilir. Örneğin hangi metod çağrısı ile servis örneğinin oluşturulacağının belirtilmesi veya hangi metod çağrısından sonra servis örneğinin yok edileceğinin belirtilmesi gibi. Bunun için de OperationContract niteliğinin IsInitiating ve IsTerminating özellikleri kullanılır. IsInitiating özelliğine true değeri atandığında, ilgili metoda bir çağrı geldiği zaman servis nesnesi örneklenecektir. Elbetteki aynı metoda oturum süresince yeni bir çağrı gelebilir. Bu durumda yeni bir servis örneği oluşturulmaz. Eğer false değeri verilirse söz konusu metoda yapılan çağrı sonrasında servise ait bir örnek oluşturulmaz. IsTerminating özelliğine true değeri atandığı takdirde, ilgili metoda dair söz konusu operasyon tamamlandığında servis örneği otomatik olarak yok edilir. Yani istemcinin Close metodunu çağırmasına gerek kalmaz.

PerCall Modeli;

PerCall modunda, istemcinin yaptığı her operasyon çağrısında servis uygulamasının çalıştığı sistem üzerinde bir nesne örneği oluşturulur ve operasyon tamamlandığında (bir başka deyişle metod çağrısı sonlandığında) bu örnek yok edilir. Bu sebepten oturum yönetimi PerSession moduna göre daha zordur. Bu nedenle oturum yönetimi adına istemcinin kendisini her operasyon çağrısında servise tanıtabilmesini sağlayacak bir sistem gerekebilir. Ne varki sunucu kaynaklarının idareli kullanılması adına verimli bir modeldir.

Single Modeli;

Single modunda, servis örneği yine istemci tarafından gelecek ilk operasyon çağrısında oluşturulur. Ne var ki PerSession modelinden farklı olarak var olan istemcinin ve diğer istemci uygulamaların metod çağrıları için servis uygulaması üzerindeki aynı nesne örneği kullanılır. Söz konusu servis örneğinin yok edilmesi ise sadece host uygulamanın kapatılması ile gerçekleşebilir. Bu teknik kaynak yönetimi adına maksimum faydayı sağlar. Ayrıca tüm kullanıcıların aynı veriyi paylaşması çok daha kolaylaşır. Lakin burada servisin single thread olup olmadığına dikkat etmek gerekir. Eğer öyleyse istemcilerden gelecek talepler sonrası zaman aşımları (timeout) söz konusu olabilir. Bunun için ConcurrencyMode özelliğine Multiple değeri atanarak thread-safe bir ortam sağlanabilir.

Şimdi örnek bir uygulama üzerinden bu modelleri incelemeye çalışalım. Örnek uygulamada Tcp protokolü baz alınmaktadır ve bu nedenle NetTcpBinding bağlayıcı tipi kullanılmaktadır.

> Tcp protokolü için varsayılan olarak eş zamanlı bağlantı sayısı maksimum 10 dur. Ancak istenirse binding configuration kısmından MaxConfiguration özelliği ile bu değiştirilebilir.
> ![mk205_1.gif](/assets/images/2007/mk205_1.gif)

İlk olarak WCF Library şablonunda bir sınıf kütüphanesi geliştirelim. Bu sınıf kütüphanesi içerisinde yer alan tipler ve içerikleri aşağıdaki gibidir.

![mk205_2.gif](/assets/images/2007/mk205_2.gif)

Servis sözleşmesini tanımlayan IUrunYonetici interface (arayüz) kodları aşağıdaki gibidir;

```csharp
using System;
using System.ServiceModel;

namespace UrunYonetimKutuphanesi
{
    [ServiceContract(Name="UrunYonetimServisi", Namespace="http://www.bsenyurt.com/UrunYonetimServisi")]
    public interface IUrunYonetici
    {
        [OperationContract()]
        void SepeteUrunEkle(string urunId);
        [OperationContract()]
        void SepettenUrunCikar(string urunId);
        [OperationContract()]
        void SiparisVer();
    }
}
```

Arayüzü uygulayan UrunYonetici sınıfının kodları aşağıdaki gibidir;

```csharp
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.ServiceModel;

namespace UrunYonetimKutuphanesi
{
    [ServiceBehavior()]
    public class UrunYonetici:IUrunYonetici,IDisposable
    {
        private FileStream fStr;
        private StreamWriter writer;
        private StringBuilder builder;

        public UrunYonetici()
        {
            Thread.Sleep(500);
            builder = new StringBuilder(); 
            builder.AppendLine(DateTime.Now.ToLongTimeString() + ": UrunYonetici nesnesi oluşturuldu.");
        }
        #region IUrunYonetici Members

        public void SepeteUrunEkle(string urunId)
        {
            Thread.Sleep(500);
            builder.AppendLine(DateTime.Now.ToLongTimeString() + ": Urun ekle metodu çağırıldı.");
        }

        public void SepettenUrunCikar(string urunId)
        {
            Thread.Sleep(500);
            builder.AppendLine(DateTime.Now.ToLongTimeString() + ": Urun çıkart metodu çağırıldı.");
        }

        public void SiparisVer()
        {
            Thread.Sleep(500);
            builder.AppendLine(DateTime.Now.ToLongTimeString() + ": Sipariş ver metodu çağırıldı.");
        }

        #endregion

        #region IDisposable Members
    
        public void Dispose()
        {
            Thread.Sleep(500);
            builder.AppendLine(DateTime.Now.ToLongTimeString() + ": UrunYonetici nesnesi için Dispose metodu çalıştırıldı.");
            builder.AppendLine("");
            fStr = new FileStream("Izleyici.txt", FileMode.Append, FileAccess.Write);
            writer = new StreamWriter(fStr);
            writer.Write(builder.ToString());
            writer.Close();
            fStr.Close();
        }

        #endregion
    }
}
```

UrunYonetici sınıfı içerisinde bir alışveriş sepetine ürün ekleme, ürün çıkarma veya sipariş verme gibi örnek fonksiyonellikler vardır. Özellikle nesneye ait örneklerin ne zaman oluşturulduğunu incelemek adına FileStream, StreamWriter ve StringBuilder tiplerinden yararlanılmakta ve Izleyici.txt isimli dosyaya log bilgileri aktarılmaktadır. Burada dikkat edilmesi gereken noktalardan biriside sınıfa IDisposable arayüzünün uygulanmış olmasıdır. Bunun tek sebebi Garbaga Collector'un nesneyi dispose etmeden önce log dosyasına gereken bilgileri aktarmaktır. ServiceBehavior niteliğinde özel olarak InstanceContextMode değeri verilmemiştir. Nitekim varsayılan değeri PerSession'dır. Şimdi örnek sunucu ve istemci uygulamalarını geliştirerek devam edelim. Servis uygulaması bir windows uygulamasıdır ve aşağıdaki kodlardan oluşmaktadır. (Servis uygulamasının System.ServiceModel ve UrunYonetimKutuphanesi assembly'larını referans etmesi gerektiğini hatırlayalım.)

Servis tarafındaki windows formu;

![mk205_3.gif](/assets/images/2007/mk205_3.gif)

Servis tarafındaki kodlar;

```csharp
namespace ServerApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        ServiceHost host;

        private void btnBaslat_Click(object sender, EventArgs e)
        {
            host = new ServiceHost(typeof(UrunYonetici));
            host.Open();
            lstDurum.Items.Add(DateTime.Now.ToLongTimeString() + host.State); 
        }

        private void btnBitir_Click(object sender, EventArgs e)
        {
            host.Close();
            lstDurum.Items.Add(DateTime.Now.ToLongTimeString() + host.State);
        }
    }
}
```

Host uygulamada servisi çalıştırmak için Open, kapatmak içinse Close metodları kullanılmaktadır. Bununla birlikte servis uygulaması UrunYonetici tipini hizmete sunar. Bağlantı için gereken tüm konfigurasyon ayarları aşağıda çıktısı görülen App.config dosyasından elde edilmektedir.

Servis tarafı için App.config içeriği;

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <behaviors />
        <services>
            <service behaviorConfiguration="" name="UrunYonetimKutuphanesi.UrunYonetici">
                <endpoint address="net.tcp://localhost:4560/UrunYonetim.svc" binding="netTcpBinding" bindingConfiguration="" name="UrunYonetimServiceEndPoint" contract="UrunYonetimKutuphanesi.IUrunYonetici" />
            </service>
        </services>
    </system.serviceModel>
</configuration>
```

Baştada belirtildiği gibi Http yerine Tcp iletişim protokolünü kullanan bir sistem ele alınmaktadır. Bu nedenle NetTcpBinding bağlayıcı tipi kullanılmaktadır.

İstemci tarafı basit bir Console uygulaması olarak düşünülebilir. İstemci tarafında, uzak nesnenin kullanılabilmesi için fiziki proxy sınıfınında üretilmiş olması gerekir. Bu amaçla yine svcutil.exe aracı kullanılmaktadır. Servis Http üzerinden metadata publishing yapmadığından, önce kütüphane üzerinden gerekli wsdl ve schema dosyaları elde edilmeli ve sonrasında bu bilgilerden faydalanarak gereken proxy sınıfı oluşturulmalıdır.

Önce komut satırından UrunYonetimKutuphanesi.dll bulunur ve

```bash
\> svcutil UrunYonetimKutuphanesi.dll
```

çalıştırılır. Ardından

```bash
\>svcutil www.bsenyurt.com.UrunYonetimServisi.wsdl *.xsd /out:UrunYonetimClient.cs
```

komutları çalıştırılmalı ve üretilen UrunYonetimClient.cs ve output.config dosyaları istemci uygulamaya dahil edilmelidir. Output.config dosyasını App.config adıyla kaydetmekte yarar vardır. Nitekim istemci uygulama konfigurasyon ayaları için bu dosyayı arayacaktır. İstemci uygulama konfigurasyon dosyası aşağıdaki şekilde daha kısa olarak modifiye edilebilir.

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <system.serviceModel>
        <client>
            <endpoint address="net.tcp://localhost:4560/UrunYonetim.svc" binding="netTcpBinding" bindingConfiguration="" contract="UrunYonetimServisi" name="DefaultBinding_UrunYonetimServisi_UrunYonetimServisi" />
        </client>
    </system.serviceModel>
</configuration>
```

İstemci uygulama kodları aşağıdaki gibidir;

```csharp
using System;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            UrunYonetimServisiClient srv = new UrunYonetimServisiClient("DefaultBinding_UrunYonetimServisi_UrunYonetimServisi");
            srv.SepeteUrunEkle("1000");
            srv.SepeteUrunEkle("1004");
            srv.SepettenUrunCikar("1000");
            srv.SiparisVer();
            Console.WriteLine("Uygulamadan Çıkmak için Bir Tuşa Basın");
            Console.ReadLine();
        }
    }
}
```

İstemci uygulamada nesne örneği oluşturulduktan sonra bir dizi metod çağırımı gerçekleştirilmektedir. Şimdi uygulamayı bu haliyele test edelim. Öncelikli olarak servis uygulaması çalıştırılmalı ve Başlat düğmesine basılarak servis açılmalıdır. Sonrasında istemci uygulamadan bir kaç tane örnek çalıştırmakta fayda vardır. Böylece farklı istemciler için servis nesnesinin ne şekilde örneklendiği daha kolay takip edilebilir.

![mk205_4.gif](/assets/images/2007/mk205_4.gif)

Bu deneme sonrasında servis uygulamasının bulunduğu klasörde Izleyici.txt isimli bir dosya oluşacak ve içeriği aşağıdaki gibi olacaktır.

![mk205_5.gif](/assets/images/2007/mk205_5.gif)

```text
17:31:53: UrunYonetici nesnesi oluşturuldu.
17:31:53: Urun ekle metodu çağırıldı.
17:31:54: Urun ekle metodu çağırıldı.
17:31:55: Urun çıkart metodu çağırıldı.
17:31:55: Sipariş ver metodu çağırıldı.
17:31:57: UrunYonetici nesnesi için Dispose metodu çalıştırıldı.

17:31:51: UrunYonetici nesnesi oluşturuldu.
17:31:51: Urun ekle metodu çağırıldı.
17:31:52: Urun ekle metodu çağırıldı.
17:31:52: Urun çıkart metodu çağırıldı.
17:31:54: Sipariş ver metodu çağırıldı.
17:31:58: UrunYonetici nesnesi için Dispose metodu çalıştırıldı.

17:31:48: UrunYonetici nesnesi oluşturuldu.
17:31:48: Urun ekle metodu çağırıldı.
17:31:49: Urun ekle metodu çağırıldı.
17:31:49: Urun çıkart metodu çağırıldı.
17:31:50: Sipariş ver metodu çağırıldı.
17:31:59: UrunYonetici nesnesi için Dispose metodu çalıştırıldı.
```

Dikkat edilecek olursa çalıştırılan 3 istemci için 3 ayrı UrunYonetici nesne örneği oluşturulmuş ve bu istemcilerin yapmış olduğu metod çağrılarının tamamı sona erdiğinde nesneler dispose edilmek üzere Garbage Collector'e devredilmeye başlanmıştır. Bu durum NetTcpBinding için varsayılan davranıştır ve PerSession modunun karşılığıdır. Eğer servis nesneleri PerCall veya Single modeline göre çalıştırmak istenirse yapılması gereken ServiceBehavior niteliğini aşağıdaki gibi değiştirmektir. İlk olarak PerCall yapıldığındaki durumu analiz edelim.

UrunYonetici sınıfındaki değişiklik aşağıdaki gibidir.

```csharp
[ServiceBehavior(InstanceContextMode=InstanceContextMode.PerCall)]
public class UrunYonetici:IUrunYonetici,IDisposable
```

Buna göre yine örnek olarak 3 istemci çalıştırıp denersek Istemci.txt dosyasının içeriği aşağıdaki gibi olacaktır.

```text
17:37:09: UrunYonetici nesnesi oluşturuldu.
17:37:09: Urun ekle metodu çağırıldı.
17:37:10: UrunYonetici nesnesi için Dispose metodu çalıştırıldı.

17:37:10: UrunYonetici nesnesi oluşturuldu.
17:37:11: Urun ekle metodu çağırıldı.
17:37:11: UrunYonetici nesnesi için Dispose metodu çalıştırıldı.

17:37:13: UrunYonetici nesnesi oluşturuldu.
17:37:14: Urun ekle metodu çağırıldı.
17:37:14: UrunYonetici nesnesi için Dispose metodu çalıştırıldı.

17:37:15: UrunYonetici nesnesi oluşturuldu.
17:37:15: Urun ekle metodu çağırıldı.
17:37:16: UrunYonetici nesnesi için Dispose metodu çalıştırıldı.

17:37:16: UrunYonetici nesnesi oluşturuldu.
17:37:17: Urun çıkart metodu çağırıldı.
17:37:17: UrunYonetici nesnesi için Dispose metodu çalıştırıldı.

17:37:18: UrunYonetici nesnesi oluşturuldu.
17:37:18: Urun ekle metodu çağırıldı.
17:37:19: UrunYonetici nesnesi için Dispose metodu çalıştırıldı.

17:37:19: UrunYonetici nesnesi oluşturuldu.
17:37:20: Urun çıkart metodu çağırıldı.
17:37:20: UrunYonetici nesnesi için Dispose metodu çalıştırıldı.

17:37:21: UrunYonetici nesnesi oluşturuldu.
17:37:21: Sipariş ver metodu çağırıldı.
17:37:22: UrunYonetici nesnesi için Dispose metodu çalıştırıldı.

17:37:22: UrunYonetici nesnesi oluşturuldu.
17:37:23: Urun çıkart metodu çağırıldı.
17:37:23: UrunYonetici nesnesi için Dispose metodu çalıştırıldı.

17:37:24: UrunYonetici nesnesi oluşturuldu.
17:37:24: Sipariş ver metodu çağırıldı.
17:37:25: UrunYonetici nesnesi için Dispose metodu çalıştırıldı.
```

Dikkat edilecek olursa, istemcilerden gelen her metod çağrısında UrunYonetici sınıfına ait bir örnek oluşturulmuş ve metod işleyişi servis tarafında tamamlandıktan sonra söz konusu örnekler toplanmak üzere Garbage Collector'e devredilmiştir.

Single modunda test işlemini yapmak için yine UrunYonetici sınıfına uygulanan ServiceBehavior niteliğinde aşağıdaki değişiklik yapılmalıdır.

```csharp
[ServiceBehavior(InstanceContextMode=InstanceContextMode.Single)]
public class UrunYonetici:IUrunYonetici,IDisposable
```

Tekrar, örnek olarak 3 istemci çalıştırılıp test edilirse Izleyici.txt dosyasına aşağıdaki bilgilerin yazıldığı görülür.

```text
17:45:13: UrunYonetici nesnesi oluşturuldu.
17:45:16: Urun ekle metodu çağırıldı.
17:45:16: Urun ekle metodu çağırıldı.
17:45:17: Urun çıkart metodu çağırıldı.
17:45:17: Urun ekle metodu çağırıldı.
17:45:18: Sipariş ver metodu çağırıldı.
17:45:18: Urun ekle metodu çağırıldı.
17:45:19: Urun ekle metodu çağırıldı.
17:45:19: Urun çıkart metodu çağırıldı.
17:45:20: Urun ekle metodu çağırıldı.
17:45:20: Sipariş ver metodu çağırıldı.
17:45:21: Urun çıkart metodu çağırıldı.
17:45:22: Sipariş ver metodu çağırıldı.
17:45:29: UrunYonetici nesnesi için Dispose metodu çalıştırıldı.
```

Görüldüğü gibi kaç istemci olursa olsun hepsi için tek bir UrunYonetici nesne örneği oluşturulmuştur.

Makalemizin başında özellikle PerSession modunda hangi metod çağrısı ile nesne örneğinin oluşturulabileceğini veya hangi metod çağrısından sonra servise ait nesne örneğinin yok edilebileceğini IsInitiating ve IsTerminating özellikleri ile belirtebileceğimizi söylemiştik. Bu modların kullanılabilmesi için ayrıca SessionMode özelliğinin değerinin Required olarak işaretlenmiş olması gerekmektedir. Nitekim söz konusu operasyonların takibi için bir oturumun var olması gerekir. Aşağıdaki kod parçasında söz konusu sistemin IUrunYonetici arayüzüne uygulanış şekli gösterilmektedir.

```csharp
[ServiceContract(Name="UrunYonetimServisi",Namespace="http://www.bsenyurt.com/UrunYonetimServisi"
,SessionMode=SessionMode.Required)]
public interface IUrunYonetici
{
    [OperationContract(IsInitiating=true)]
    void SepeteUrunEkle(string urunId);
    [OperationContract(IsInitiating=false)]
    void SepettenUrunCikar(string urunId);
    [OperationContract(IsInitiating=false,IsTerminating=true)]
    void SiparisVer();
}
```

Buna göre nesne örneği sadece SepeteUrunEkle metodu ile oluşturulabilir. Bir başka deyişle SepettenUrunCikar veya SiparisVer metodlarının SepeteUrunEkle metodundan önce çalıştırılamaması garanti altına alınmış olunur. Buna göre istemci uygulama kodlarının aşağıdaki gibi değiştirildiği göz önüne alınsın.

```csharp
UrunYonetimServisiClient srv = new UrunYonetimServisiClient("DefaultBinding_UrunYonetimServisi_UrunYonetimServisi");
srv.SepettenUrunCikar("1000");
srv.SepeteUrunEkle("1000");
srv.SiparisVer();
Console.WriteLine("Uygulamadan Çıkmak için Bir Tuşa Basın");
Console.ReadLine();
```

Dikkat edilecek olursa önce SepettenUrunCikar metodu çağırılmaktadır. Sonrasında ise SepeteUrunEkle ve SiparisVer isimli metodlar çalıştırılmak istenir. Ancak kod bu şekilde denendiğinde çalışma zamanında aşağıdaki ekran görüntüsünde yer alan istisna alınır.

![mk205_6.gif](/assets/images/2007/mk205_6.gif)

Dolayısıyla metodların çalışma sırasıda söz konusu özellikler yardımıyla garanti altına alınmıştır. Elbette IsInitiating özelliğinin true olarak atandığı SepeteUrunEkle metodu bir kere çalıştırıldıktan sonra diğer metodlar istenildiği gibi çalıştırılabilir. Diğer taraftan aşağıdaki gibi bir kod parçasında yine dikkatli olunmalıdır.

```csharp
UrunYonetimServisiClient srv = new UrunYonetimServisiClient("DefaultBinding_UrunYonetimServisi_UrunYonetimServisi");
srv.SepeteUrunEkle("1000");
srv.SepeteUrunEkle("1004");
srv.SepettenUrunCikar("1000");
srv.SiparisVer();
srv.SepeteUrunEkle("1000"); 
Console.WriteLine("Uygulamadan Çıkmak için Bir Tuşa Basın");
Console.ReadLine();
```

Dikkat edilecek olursa SiparisVer metodundan sonra SepeteUrunEkle metodu çalıştırılmıştır. Oysaki SiparisVer metodu için IsTerminating özelliğinin değeri true olarak atanmıştır. Dolayısıyla bu metod çağrısından sonra söz konusu servis nesne örneği yok edilecektir. Buna göre istemci tarafında aşağıdaki ekranda görüldüğü gibi ChannelTerminatedException sınıfı tipinden bir istisna mesajı alınır.

![mk205_7.gif](/assets/images/2007/mk205_7.gif)

Özetle daha öncedende değindiğimiz gibi istemci tarafında istisna yönetimi adına bazı hazırlıklar yapmak (Fault Management) son derece isabetli bir hareket olacaktır.

Bu makalemizde servis tarafındaki nesne örnekleri ile istemci arasındaki ilişkilerin ele alınması adına InstanceContextMode, IsInitiating, IsTerminating gibi özelliklerden nasıl yararlanabileceğimizi ele almaya çalıştık. Oturum yönetimi adına WCF içerisinde ele alınması gereken daha pek çok konu vardır. Örneğin PerCall modda oturum yönetiminin sağlanması gibi. Bu gibi konulara ilerleyen makalelerimizde değinmeye çalışacağız. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama İçin Tıklayınız.](/assets/files/2007/InstanceModes.zip)