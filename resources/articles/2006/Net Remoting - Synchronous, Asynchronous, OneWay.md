---
title: ".Net Remoting - Synchronous, Asynchronous, OneWay"
pubDate: 2006-03-31 12:00:00
categories:
  - .Net Remoting
tags:
  - .Net Remoting
  - C#
  - .NET
  - XML
  - Web Service
  - XML Web Services
  - Threading
  - Delegates
---

# .Net Remoting - Synchronous, Asynchronous, OneWay
Değerli Okurlarım Merhabalar,

Remoting gibi mimarilerde bazen istemcilerin (clients) çağırdıkları uzak nesnelere (remote objects) ait metodların dönüş süreleri uzun zaman alabilir. Bu gibi durumlarda istemci doğal olarak, uzak nesne üzerinden çağırdığı metodun geri dönüşünü beklemek durumunda kalır. Dolayısıyla istemci uygulamanın, uzak nesne çağırımına paralel olarak yürütebileceği işlemler var ise bunlarda askıda kalacaktır. Bunun değişik nedenleri olabilir. Gerçektende işlemler uzun sürebilir. Örneğin veritabanı bazlı işlemlerin uzak sunucular üzerinde gerçekleştiği remoting uygulamalarında sıkça rastlanabilen bir durumdur. Varsayılan olarak istemci tarafında eğer hiç bir özelleştirme yapılmaz ise, senkron (Synhronous) çalışan bir yapı söz konusudur. Bu makalemizde Senkron model ile başlayıp, Asenkron ve OneWay metod çağırım modellerini incelemeye çalışacağız.

Aşağıdaki kod parçasında yer alan remoting uygulaması senkron yapıya bir örnektir. Uzak nesnemize ait sınıf modelinde çok basit bir metod yer almaktadır. Bu metodun geri dönüş süresini arttırmak ve istemcinin durumunu analiz edebilmek için Thread tipinin Sleep metodu ile uzak nesne metodu içerisinde güncel iş parçacığı bir süre uyutulmuştur.

![dikkat.gif](images/dikkat.gif)
İstemci tarafında asenkron (Asynchronous) veya tek yön (OneWay) için bir özelleştirme yapılmamışsa, uygulama Senkron (Synchronous) kuralları çerçevesinde işler.

Uzak nesneye ait interface modelimiz,

```csharp
using System;

namespace RemoteLib
{
    public interface IRLib
    {
        int GetTotal(int orderID);
    }
}
```

Sunucu tarafı uygulama kodlarımız,

```csharp
using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using RemoteLib;

namespace Server
{
    // Uzak nesne tipimiz
    public class OrderObj:MarshalByRefObject,IRLib 
    {

        #region IRLib Members
    
        public int GetTotal(int orderID)
        {
            Console.WriteLine("Metod çağırıldı...");
            System.Threading.Thread.Sleep(4000);
            return orderID;
        }

        #endregion
    }

    // Sunucu uygulama
    class Program
    {
        static void Main(string[] args)
        {
            TcpServerChannel srvC = new TcpServerChannel(8777);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(OrderObj), "OrderObj", WellKnownObjectMode.SingleCall);
            Console.WriteLine("Sunucu dinlemede...");
            Console.ReadLine();
        }
    }
}
```

İstemci tarafı uygulama kodlarımız,

```csharp
using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using RemoteLib;

namespace Client
{
    // İstemci uygulama
    class Program
    {
        static void Main(string[] args)
        {
            TcpClientChannel cliC = new TcpClientChannel();
            IRLib rl = (IRLib)Activator.GetObject(typeof(IRLib), "tcp://manchester:8777/OrderObj");
            Console.WriteLine(rl.GetTotal(1001).ToString());
            Console.WriteLine("Metod sonrası kodlar...");
            Console.ReadLine();
        }
    }
}
```

Sunucu ve istemcinin çalışmasına bakalım. (Aşağıdaki video görüntüsü flash player gerektirmektedir.)

Açıkça görüldüğü gibi, istemci uzak nesneye ait GetTotal isimli metodu çağırdıktan sonra, izleyen kod satırına geçebilmek için, uzak nesne metodunun tamamlanmasını beklemek zorunda kalmıştır. Yaklaşık 4 saniyelik gecikmenin ardından, istemci tarafında izleyen kod satırları işletilebilmiştir. İşte bu senkron erişim modelidir. Elbette bu modelin dezavantajlı bir model olduğu konusunda kesin yargıya varmak yanlıştır. Nitekim zaman zaman, istemci tarafının izleyen kod satırlarının işleyişi, uzak nesneden dönecek değerlere bağlı olabilir. Bu gibi durumlarda senkron model dışına çıkmamakta fayda vardır.

![dikkat.gif](images/dikkat.gif)
Senkron modelde, istemcinin uzak nesneden (remote object) dönecek olan sonuçları beklemesi her zaman bir dezavantaj olarak görülmemelidir. Çoğu zaman, uzak nesnenin döndürdüğü değerler, istemcinin izleyen kod satırlarında kullanılacak tipte olabilir.

Gelelim asenkron (asynchronous) metod çağırım modeline. Eğer Xml Web Servislerindeki asenkron erişim modelini hatırlarsanız, proxy sınıfı içerisinde Begin ve End kelimeleri ile başlayan metodlar yer aldığını bilirsiniz. Bu metodlar yardımıyla Web Metodlarına (Web Method) asenkron olarak erişmek mümkündür. Oysaki Remoting mimarisinde, yukarıdaki örneği göz önüne aldığımızda fiziki bir proxy sınıfı yer almamaktadır. Dolayısıyla bizim istemci tarafında, asenkron yürütme için gereken işlevleri bir temsilci (delegate) yardımıyla başarmamız gerekecektir.

Nitekim temsilci tipleri yardımıyla BeginInvoke ve EndInvoke metodlarına erişilebilmektedir. Asenkron erişim modelinde püf nokta, uzak nesne üzerinde yer alan ve asenkron olarak yürütülmek istenen metod modelinin, istemci tarafında bir temsilci (delegate) tipi yardımıyla çalışma zamanında işaret edilecek olmasıdır. İstemci tarafındaki bu temsilci tipini kullanarak BeginInvoke ve EndInvoke metodlarına çağırabilir ve böylece uzak nesne metodunu asenkron olarak yürütebiliriz. Yine yukarıdaki örneğimizi göz önüne alacak olursak yapmamız gereken değişiklikler istemci tarafındadır.

```csharp
namespace Client
{
    delegate int dlgGetTotal(int orID);

    class Program
    {
        static void Main(string[] args)
        {
            TcpClientChannel cliC = new TcpClientChannel();
            IRLib rl = (IRLib)Activator.GetObject(typeof(IRLib), "tcp://manchester:8777/OrderObj");
            dlgGetTotal currGetTotal = new dlgGetTotal(rl.GetTotal);
            IAsyncResult res=currGetTotal.BeginInvoke(1001, null, null);
            Console.WriteLine("Metod sonrası kodlar...");
            Console.WriteLine(currGetTotal.EndInvoke(res).ToString());
            Console.ReadLine();
        }
    }
}
```

Görüldüğü gibi ilk olarak, OrderObj isimli uzak nesnemize ait GetTotal metodunu, istemci tarafında işaret edebilecek bir temsilci tanımladık. Çalışma zamanında bu temsilci tipine ait bir nesne örneği yardımıyla, istemcinin eriştiği o anki uzak nesne referansı üzerindeki asıl GetTotal metodunu işaret edebileceğiz. Bunu sağlamak için currGetTotal isimli delegate örneğini nasıl oluşturduğumuza dikkat edin. Buradaki rl arayüz örneğinin çağırdığı GetTotal metodu, çalışma zamanında aktif olan uzak nesne referansının sahip olduğu GetTotal metodudur.

```csharp
dlgGetTotal currGetTotal = new dlgGetTotal(rl.GetTotal);
```

Artık bu noktadan itibaren BeginInvoke ve EndInvoke metodlarını çağırabiliriz. BeginInvoke metodu bildiğiniz gibi geriye IAsyncResult arayüzü tipinden bir nesne döndürür.

```csharp
IAsyncResult res=currGetTotal.BeginInvoke(1001, null, null);
```

Bunu elde ettikten sonra, istemci tarafında izleyen kod satırları başarılı bir şekilde çalışacaktır. Hemen ardından EndInvoke metodu çağırılıp, ortamdaki IAsyncResult arayüz nesnesi bu metoda devredilerek asıl sonuçlar istemci tarafına alınır.

```csharp
Console.WriteLine(currGetTotal.EndInvoke(res).ToString());
```

Burada sembolik olarak istemci tarafında BeginInvoke operasyonundan sonra sadece tek satırlık kod çalışmaktadır. Bundan sonra hemen EndInvoke metodunu çağırmaktayız. EndInvoke metodu, eğer sunucu tarafındaki uzak nesne metodu halen geriye bir sonuç döndürmemiş ise, bu sonuç gelene kadar da uygulamayı duraksatacaktır. Bunu unutmamamız gerekir. Sunucu ve istemcinin çalışmasına bakalım. (Aşağıdaki video görüntüsü flash player gerektirmektedir.)

Asenkron erişim modelinde genellikle yukarıdaki kullanım tarzı yerine Callback tekniği kullanılır. Bu tekniğe göre, uzak nesne metodu asenkron olarak çalışmasını bitirdiğinde, istemci tarafındaki callback metodu otomatik olarak devreye girerek sonuçları uygulamaya devredecektir. Şimdi yukarıdaki örneğimizi buna göre değiştirelim. Tek yapmamız gereken, BeginInvoke metodunda Callback metodu ile ilgili parametreyi bildirmek olacaktır. Söz konusu parametre AsyncCallback tipinden bir temsilci nesne örneğidir. Bu temsilci nesne örneği, çalışma zamanında asenkron olarak yürüyen metod işleyişini tamamladığı anda devreye girecek olan geri bildirim metodunu işaret etmektedir. Dolayısıyla örneğimizin kodlarını aşağıdaki gibi değiştirerek Callback tekniğini uygulayabiliriz.

```csharp
namespace Client
{
    delegate int dlgGetTotal(int orID);

    class Program
    {
        static dlgGetTotal currGetTotal;

        static void Main(string[] args)
        {
            TcpClientChannel cliC = new TcpClientChannel();
            IRLib rl = (IRLib)Activator.GetObject(typeof(IRLib), "tcp://manchester:8777/OrderObj");
            currGetTotal = new dlgGetTotal(rl.GetTotal);
            AsyncCallback acb = new AsyncCallback(CallbackMetod);
            IAsyncResult res = currGetTotal.BeginInvoke(1001, acb, null);
            Console.WriteLine("Metod sonrası kodlar...");
            for (int i = 0; i < 8; i++)
            {
                System.Threading.Thread.Sleep(1000);
                Console.Write("+");
            }
            Console.ReadLine();
        }

        static void CallbackMetod(IAsyncResult res)
        {
            if(res.IsCompleted)
                Console.Write("Metod sonucu {0}",currGetTotal.EndInvoke(res).ToString());
        }
    }
}
```

AsyncCallback temsilci tipi, IAsyncResult arayüzü tipinden nesne örnekleri alan ve geriye değer döndürmeyen metodları işaret etmektedir. Biz uygulamamıza bu amaçla CallbackMetod isimli bir metod ekledik. Bu metod içerisinde, eğer asenkron olarak yürütülen uzak nesne metodu işleyişini tamamlamışsa (ki bunu metoda parametre olarak gelen IAsyncResult nesne örneğinin IsCompleted özelliği ile anlıyoruz) sonuçları ortama aktarmak için EndInvoke metodu çağırılır. Callback metodunun devreye alınabilmesi için BeginInvoke metoduna AsyncCallback temsilci tipine ait örneğimizi parametre olarak vermemiz gerekir. Sunucu ve istemcinin çalışmasına bakalım. (Aşağıdaki video görüntüsü flash player gerektirmektedir.)

Gelelim OneWay tekniğine. OneWay tekniğinde sadece geri dönüş tipi olmayan uzak nesne metodları söz konusu olabilir. Dahası OneWay niteliği (attribute) ile imzalanan bir metod, istemci tarafından çalıştırıldıktan sonra unutulur.

![dikkat.gif](images/dikkat.gif)
OneWay niteliğine sahip metodlar, at-unut füzeleri gibidir. İstemci tarafında çağırıldıktan sonra istemci kodları işleyişini sürdürürken, OneWay metodu geriye hiç bir bilgi vermez ve sunucu tarafında kendi başına işleyişini tamamlar.

Yani istemci tarafında metod çağırımı sonrası kodlar işlerken, metodun başına ne geldiğinden istemcinin herhangibir şekilde haberi olmaz. Bu metod içerisinde meydana gelebilecek bir istisnanın, istemci tarafından algılanmaması anlamına gelir. Bu nedenle OneWay metodlar çoğunlukla sunucu tarafında log tutma gibi istemci açısından önem arz etmeyen durumlarda, asenkron işleyişi gerçekleştirmek için tercih edilebilir. Uzak nesnede yer alan bir metodun OneWay davranışını gösterebilmesi için, OneWay niteliği (attribute) ile işaretlenmesi gerekir. Bizim örneğimizde bunu interface tarafında yapmamız gerekecektir. OneWay niteliği, System.Runtime.Remoting.Messaging isim alanı altında yer almaktadır. Bu nedenle interface'imizin bulunduğu class library uygulamasına System.Runtime.Remoting referansınıda açıkça eklememiz gerekiyor.

```csharp
using System;
using System.Runtime.Remoting.Messaging;

namespace RemoteLib
{
    public interface IRLib
    {
        int GetTotal(int orderID);
        [OneWay]
        void WriteLog(int orderID);
    }
}
```

Bu değişiklikten sonra uzak nesnemizide interface'imizin yapısına uygun olarak düzenlememiz gerekir. OrderObj sınıfımızın yeni halinde WriteLog isimli ek bir metod olmalıdır. Bu metod içerisinde basit olarak birer saniye aralıklarla sunucu uygulamada mesaj yazdırılmaktadır. Burada amaç, istemcinin metodu çağırmasını takiben, istemci uygulamanın sonlanması halinde sunucun bu kodları çalıştırmaya devam edeceğini ispatlamaktır.

```csharp
public void WriteLog(int orderID)
{
    for (int i = 0; i < 10; i++)
    {
        Console.WriteLine("Log bilgisi yazılıyor...");
        System.Threading.Thread.Sleep(1000);
    }
}
```

Şimdi istemci tarafındaki kodlarımızı aşağıdaki gibi değiştirelim.

```csharp
static void Main(string[] args)
{
    TcpClientChannel cliC = new TcpClientChannel();
    IRLib rl = (IRLib)Activator.GetObject(typeof(IRLib), "tcp://manchester:8777/OrderObj");
    rl.WriteLog(1901);
    Console.WriteLine("Metod sonrası kodlar...");
    Console.WriteLine("Uygulamayı durdurmak için bir tuşa basın...");
    Console.ReadLine();
}
```

Bu sefer uygulamayı metodu çağırdıktan sonra sonlandırıyoruz. Buna göre OneWay tipindeki WriteLog isimli uzak nesne metodu, sunucu üzerinde çalışmasına devam edecektir. Oysaki metod OneWay davranışı göstermeyecek şekilde ayarlansaydı, metod çalışması bitene kadar istemci bekleyecekti. Aşağıdaki flash animasyonunda durumu daha iyi görebilirsiniz. (Aşağıdaki video görüntüsü flash player gerektirmektedir.)

OneWay davranışı gösteren metodların bir diğer sorunu ise exception olması halinde ortaya çıkar. Örneğin, OrderObj isimli uzak nesne sınıfımızın WriteLog isimli metodu içerisinde bir istisna durumu oluştuğunu düşünelim. Bu durumda istemci tarafında çalışan metodu try...catch blokları ile kontrol etsek dahi istisnayı yakalayamayız. Dahası bu istisna sunucu tarafında oluşarak metodun işleyişini bozacaktır. Dolayısıyla istemcinin hiç bir şekilde bu istisnadan haberi olmayacaktır. Bu OneWay davranışını sergileyecek metodların kullanımını azaltan bir etkendir. Kısacası OneWay tekniği her ne kadar mükemmel bir asenkron modeli gibi görünsede, gerçekten istemci açısından sonuçları önemli olmayan durumlarda ele alınabilecek bir yaklaşımdır.

Bu makalemizde.Net Remoting için geçerli olan metod çağırma tekniklerini kısaca incelemeye çalıştık. Senkron mimarinin varsayılan tip olduğunu, asenkron mimarinin avantajlarını ve OneWay tekniğinin dezavantajlarını irdelemeye çalıştık. Görünen o ki, bu modellerden hangisinin kullanılacağına karar vermek için gerçekten ihtiyaçları iyi tespit etmek gerekmektedir. Böylece geldik bir makalemizin daha sonuna bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.
