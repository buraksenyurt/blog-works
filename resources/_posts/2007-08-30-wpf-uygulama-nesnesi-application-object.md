---
layout: post
title: "WPF-Uygulama Nesnesi (Application Object)"
date: 2007-08-30 09:00:00 +0300
description: "Windows Presentation Foundation windows tabanlı uygulama geliştirmeye çok yeni bir yaklaşım getirdi. Tabiri yerindeyse pek çok yenilik ile karşı karşıyayız. İşte bu makalemizde WPF ile geliştirilen windows uygulamalarında çekirdek nesnelerden birisi olan Application tipini incelemeye çalışacağız. Application nesnesi, WPF uygulamalarının çekirdek nesnesidir..."
categories:
  - wpf
tags:
  - wpf
  - csharp
  - xml
  - bash
  - dotnet
  - linq
  - wcf
  - xaml
  - http
  - transactions
  - generics
  - visual-studio
---
# WPF-Uygulama Nesnesi (Application Object)
Değerli Okurlarım Merhabalar,

Windows Presentation Foundation windows tabanlı uygulama geliştirmeye çok yeni bir yaklaşım getirdi. Tabiri yerindeyse pek çok yenilik ile karşı karşıyayız. İşte bu makalemizde WPF ile geliştirilen windows uygulamalarında çekirdek nesnelerden birisi olan Application tipini incelemeye çalışacağız. Application nesnesi, WPF uygulamalarının çekirdek nesnesidir.

Genel olarak bir windows uygulamasının çalıştırılması işletim sistemi tarafından tetiklenen bir aksiyonla gerçekleşir ve bu uygulama Process dahilinde ayrı bir AppDomain içerisinde çalışır. Buna göre her WPF uygulaması çalıştığı yaşam süresi boyunca yanlızca bir Application nesnesine sahip olur. Buda çok doğal olarak, windows uygulaması içerisindeki tüm sayfa (Page) veya pencerelerin (Windows) ortaklaşa kullanacakları global seviyede bir nesnenin söz konusu olması anlamına gelmektedir. Aşağıdaki şekilde Application nesnesinin WPF uygulamasındaki yeri tasvir edilmeye çalışılmaktadır.

![mk220_5.gif](/assets/images/2007/mk220_5.gif)

Bu, inanıyorumki web uygulaması geliştirenler için tuhaf değildir. Malum web uygulamalarında da Application nesnesi vardır ve özellikle uygulama seviyesinde değişkenlerin tutulması için kullanılır. Aslında bu kavram.Net Framework 3.0 öncesi windows programlamadada vardır. Öyleki hepimiz Main metod içerisinde kullanılan Application sınıfını ve üyelerini az çok biliyoruz. Ne varki, WPF ile geliştirilen uygulamalarda Application nesnesinden yararlanarak daha farklı fonksiyonellikler elde edilebilmektedir. Bunları maddeler halinde aşağıdaki gibi sıralayabiliriz.

- Uygulamanın yaşam süresi izlenebilir. (Application LifeTime)
- Komut satırından uygulamaya gönderilen parametreler (Command Arguments) alınıp işlenebilir.
- Uygulamanın kapatılma süreci ele alınabilir.
- Ele alınamayan istisnalar (Unhandled Exceptions) için özel durum yönetimleri gerçekleştirilebilir.
- Uygulama genelinde kullanılabilecek global özellikler (properties) ve kaynaklar (resources) tanımlanabilir ve kullanılabilir.
- Uygulamanın derleme (build) sürecine ait ayarlar yapılabilir.
- XBAP (Xaml Browser APplication) uygulamalarında navigasyon süreci izlenebilir.

Application nesnesini işletim sistemi ile uygulama kodu arasındaki bir arayüz olarakda düşünebiliriz. Uygulama içerisindeki tüm pencere (Window) veya sayfaların (Page) aynı Application nesnesine erişebilmesini ve bu nesnenin tekliğini sağlamak için tahmin edileceği üzere Singleton Pattern'e uygun bir üretim sistemi söz konusudur. Bu tek nesne üretim işini Application sınıfının Current özelliği üstlenmektedir. Application nesneleri, uygulamanın bulunduğu bilgisayarda saklandıklarından özellikle kaynak (resource) veya özelliklerin (properties) değerlerinin saklanması gibi durumlarda sunucu (server) gibi kaynaklara erişime gerek kalmamaktadır. Buda bir avantaj olarak görülebilir.

Örneklere başlamadan önce, uygulama yaşam sürecini (Application LifeTime) ele almakta ve değerlendirmekte yarar olacağı kanısındayım. Application nesnesi ile birlikte, bir WPF uygulamasının yaşam sürecini daha iyi kontrol edebilmekteyiz. Öncelikli olarak aşağıdaki temsili resmi göz önüne alalım. Bu şekilde temel olarak bir WPF uygulamasının standart yaşam döngüsü, süreçteki temel olaylar ve çevresel bazı etkiler irdelenmeye çalışılmaktadır.

![mk220_1.gif](/assets/images/2007/mk220_1.gif)

Herşeyden önce WPF uygulmasının kullanıcı tarafından tetiklenmesi sonrası işletim sistemi tarafından uygulamanın bir AppDomain içerisine açılması söz konusudur. Bundan sonraki süreçte uygulama çeşitli nedenlerle sonlanıncaya kadar ele alabilecek bazı olaylar vardır. Uygulama çalışmaya başladığında Application nesnesinin Startup olayı tetiklenir. Bu olay içerisinde uygulama başlatılırken yapılması istenenler yazılabilir. Örneğin uygulamanın hangi pencere veya sayfasının yükleneceğine burada karar verilebilir. Startup olayını global seviyedeki özellik ve kaynakların saklanması halinde, yüklenecekleri yer olarakda tasarlayabiliriz. Hatta komut satırı parametrelerininde (Command Line Arguments) bu olay içerisinde ele alınması sağlanabilir. Yazımızın ilerleyen kısımlarında bununla ilişkili bir örnek geliştiriyor olacağız.

Gelelim Activated ve Deactivated olaylarına. Activated olayı, WPF uygulaması ilk çalıştırıldığında ve ilk penceresi açıldığında otomatik olarak tetiklenir. Bundan sonra uygulama pasif moda (Deactivate) geçinceye kadar çalışmaz. İşletim sistemi üzerinde çalışan başka uygulamalara yapılan geçişlerde, Deactivated isimli olay tetiklenmektedir. Tahmin edileceği üzere tekrardan WPF uygulamasına dönülmesi sonrasında Activated olayı yeniden tetiklenecektir. Bu tetikleme, taskbar yardımıyla söz konusu WPF uygulamasındaki formlardan herhangibirinin açılması halinde, Alt+Tab tuş kombinasyonu ile yapılan geçişler sonrasında meydana gelmektedir. Deactivated olayında kaynak tüketimi yüksek olan çalışma zamanı nesnelerinin işleyişlerinin duraksatılması yada uyku moduna geçirilmeleri sağlanarak işletim sisteminin daha az yorulması gerçekleştirilebilir. Activated olayı tetiklendiğindede söz konusu kaynakların tekrardan ayağa kaldırılması işlemleri gerçekleştirilebilir. Bu tip bir senaryo elbetteki tartışmaya açık olmalıdır.

> Activated ve Deactivated olayları XBAP (Xaml Browser APplications) uygulamaları tarafından desteklenmemektedir.

Gelelim DispatcherUnhandledException olayına. Şekildende görüleceği üzere uygulama kodu içerisinden çalışma ortamına fırlayacak bir istisnayı Application nesnesninin bu olayı içerisinde ele alabiliriz. Uygulama içerisindeki kodlarda bir exception oluştuğunda (özellikle ele alınmayan-unhandled exceptions) WPF çalışma ortamı standart bir hata dialog penceresi çıkartacak ve hata raporunun gönderilip gönderilmeyeceği sorulacaktır. Ardındanda uygulama sonlandırılacaktır. Ancak, DispatcherUnhandledException olayı içerisine gelen DispatcherUnhandledExceptionEventArgs parametresininin Handled özelliğini kullanarak uygulamanın kapatılması (Eğer mümkünse tabi) engellenebilir.

SessionEnding isimli olay, işletim sisteminden aşağıdaki aktiviteler gerçekleştiğinde tetiklenmektedir.

- LogOff
- Windows Shutdown
- Restart
- Hibernate

Eğer çalşan WPF uygulamasında kritik işlemler (örneğin halen daha devem eden veya sunucuya bağlı halde iken veritabanı üzerinde transaction bazlı gerçekleşen bir işlem vb. söz konusu olabilir) söz konusu ise, yukarıdaki aktivitelerin gerçekleşmesi halinde sürecin iptal edilmesi arzu edilebilir. Bu nedenle Application sınıfının SessionEnding olayı ele alınır. Bu amaçla, SessionEndingCancelEventArgs parametresi kullanılır. Eğer SessionEnding içerisinde işlem iptali yapılmassa Application sıfının Shutdown metodu çağırılır ve Exit isimli olay tetiklenmiş olur.

> SessionEnding olayı XBAP (Xaml Browser APplications) uygulamları tarafından desteklenmemektedir.

Bir WPF uygulamasını bilinçli olarak sonlandırmak için Application sınıfının Shutdown metodu kullanılabilir. Aslında uygulamanın ana penceresi (Main Window) kapatıldığında, tüm pencereleri kapatıldığında veya yukarıdaki gibi işletim sistemince LogOff, Windows Shutdown, Restart, Hibernate aksiyonları gerçekleştirildiğinde otomatik olarak çalışır. İstenirse bir uygulama içerisindeki ShutDown metodunun hangi durumlarda otomatik olarak çalışacağı ShutdownMode (ShutDownMode enum sabiti tipinden değerler alır) özelliği yardımıyla Application elementi içinden veya uygulama kodundan değiştirilebilir.

ShutDownMode enum sabitinin OnLastWindowClose, OnMainWindowClose ve OnExplicitShutDown isminde üç farklı değeri vardır. OnLastWindowClose varsayılan değerdir ve uygulama içerisinde birden fazla pencere olması halinde en sonuncusu kapatıldığında uygulamanın kapanması söz konusudur. Bir başka deyişle uygulamanın kapatılabilmesi için açık olan tüm pencerelerin kapatılması gereklidir. Eğer OnMainWindowClose değeri verilirse, uygulama başladığında etklinleştirilen pencere kapatıldığında uygulama sonlanır. OnExplicitShutDown seçildiğinde ise uygulamanın kapatılması geliştiriciye bırakılmıştır. ShutDown metodu ile yapılan sonlandırma isteklerinde istenirse işletim sistemine bir çıkış kodu (ExitCode) değeri integer tipinden gönderilebilir. Çıkış kodunun değeri, bu uygulamaya bağlı başka uygulamalar tarafından değerli olabilir. ExitCode için varsayılan değer sıfırdır.

Shutdown metoduna yapılan bilinçli çağırdan sonra veya SessionEnding gerçekleştikten sonra Exit isimli olay tetiklenir. Bu olay içerisinde uygulamanın son durumuna ait bilgilerin saklanması gibi işlemler yapılabilir. Özellikle global özellik (Global Properties) ve kaynakların (Resources) saklanma işlemlerinin gerçekleştirilmesi için ideal bir lokasyondur. Bu olay içerisine gelindiğinde uygulamanın kapatılma süreci artık iptal edilememektedir. Exit olayı içerisine gelen ExitEventArgs parametresi yardımıyla, çıkış kodu (Exit Code) değeride yakalanabilir ve buna göre farklı askiyonlar gerçekleştirilebilir.

> Exit olayı XBAP (Xaml Browser APplications) uygulamalarında da yazılabilir. Ancak ExitCode değeri XBAP uygulamalarında görmezden gelinir. Exit olayı bir XBAP uygulamasında örneğin Internet Explorer 7 ile açılmışsa ilgili tab kapatıldığında, tarayıcı (Browser) uygulama kapatıldığında veya başka bir yere navigasyon ile gidildiğinde tetiklenmektedir.

Bu kadar teorik bilgiden sonra birazda pratiğe geçmekte fayda var. Yazımızın bundan sonraki kısımlarında örnekler üzerinden ilerlemeye çalışacağız. Ancak bu kez Visual Studio 2008 Beta 2 sürümünü kullanıyor olacağız. Bu nedenle yazdığımız kodlarda bir değişiklik olmasada, IDE üzerinde final sürümü çıktığında bazı farklılıklar olabileceğini şimdiden söylemek isterim. İlk olarak Visual Studio 2008 Beta 2 de yeni bir WPF uygulaması açarak işe başlayalım. Uygulama açıldığında Application sınıfından türeyen App isimli bir tipin olduğunu göreceğiz. App sınıfının türediği Application sınıfının.Net Framework 3.0 içerisindeki yeri sınıf diagramdan (class diagram) bakıldığında aşağıdaki gibidir.

![mk220_4.gif](/assets/images/2007/mk220_4.gif)

Uygulamaya ait ayarların hem element hemde kod bazında yönetimi için aşağıdaki şekildende de görüldüğü gibi App.xaml ve App.xaml.cs dosyaları otomatik olarak oluşturulacaktır.

![mk220_6.gif](/assets/images/2007/mk220_6.gif)

Visual Studio 2008 Beta 2 ile bir olayı element seviyesinde yüklemek son derece kolaydır. Burada intellisense desteğinin tam olarak verildiğini söyleyebiliriz. Örneğin Startup olayını aşağıdaki gibi Application elementi içerisinden seçebiliriz. Aşağıdaki ekran görüntüsündende görüldüğü gibi, Application elementi içerisinde boşluk tuşuna basıldığında kullanılabilecek tüm üyeler çıkacaktır.

![mk220_2.gif](/assets/images/2007/mk220_2.gif)

Diğer taraftan bir olay yüklenmek istendiğinde (örneğin Startup) aşağıdaki ekran görüntüsünde olduğu gibi geleneksel olarak tab tuşundan yararlanarak ilgili olay metodun otomatik olarak yüklenmesi sağlanabilir.

![mk220_3.gif](/assets/images/2007/mk220_3.gif)

Bunun sonucunda App.xaml.cs içerisine aşağıdaki olay metodu eklenir.

```csharp
private void Application_Startup(object sender, StartupEventArgs e)
{

}
```

Application elementinin içeriği ise aşağıdaki gibi olacaktır.

```xml
<Application x:Class="UsingApplicationObjects.App"
xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
StartupUri="Window1.xaml" Startup="Application_Startup">
    <Application.Resources>
    </Application.Resources>
</Application>
```

Visual Studio 2008 Beta 2' nin bu yardımlarına değindikten sonra Activated ve Deactivated olaylarını inceleyerek devam edelim. Bu amaçla Application elementi içerisinde aşağıdaki yüklemeler yapılmalıdır.

```xml
<Application x:Class="UsingApplicationObjects.App"
xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
StartupUri="Window1.xaml" Activated="Application_Activated" Deactivated="Application_Deactivated">
    <Application.Resources> 
    </Application.Resources>
</Application>
```

Sonrasında ise App.xaml.cs dosyası içerisinde açılan olay metodları aşağıdaki gibi düzenlenmelidir.

```csharp
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Diagnostics;

namespace UsingApplicationObjects
{
    public partial class App : Application
    {
        private void Application_Activated(object sender, EventArgs e)
        {    
            Debug.WriteLine("Activated");
        }

        private void Application_Deactivated(object sender, EventArgs e)
        {
            Debug.WriteLine("DeActivated");
        }
    }
}
```

Test amaçlı bir kod yazıldığı için System.Diagnostics isim alanında bulunan Debug sınıfın WriteLine metodu ile Output penceresine çıktılar verilmektedir. Uygulama ilk çalıştırıldığında ve output penceresine bakıldığında Activated yazdığı görülecektir. Eğer Alt+Tab tuşları veya mouse ile uygulama dışına çıkılırsa output penceresine DeActivated yazdığı görülecektir. Bir başka deyişle Deactivated olayı tetiklenmiştir. Elbette tekrardan uygulamaya dönülürse Activated olayı yeniden tetiklenecektir.

Sıradaki örneğimizde Startup ve Exit olaylarını birlikte incelemeye çalışacağız. Bunun için bize örnek bir senaryo gerekmektedir. Application nesnesinin tüm uygulama için geçerli olabiliecek özellik (Property) ve kaynakları (Resources) saklayabildiğinden bahsetmiştik. Çok doğal olarak bunları uygulamadan çıkarken saklamak ve uygulama açıldığında yeniden yüklemek isteyebiliriz. İşte bu noktada saklanan bilgileri okuma işlemini Startup olayında, yazdırma işlemini ise Exit olayı içerisinde ele almalıyız. Application nesnesi üzerinden bir özellik tanımı yapmak ve değerini vermek son derece kolaydır. Tek yapılması gereken Application sınıfının Properties özelliği ve indeksleyicisinden yararlanmaktır. Aynı durum kaynaklar içinde geçerlidir. Örneğin aşağıdaki ekran görüntüsünde özellik ekleme adımı gösterilmektedir.

![mk220_7.gif](/assets/images/2007/mk220_7.gif)

Dikkat edilecek olursa Properties özelliği Dictionary bazlı bir koleksiyondur ve object tipinden anahtar-değer (key-value) çiftleri ile çalışmaktadır. Buda kendi tiplerimizi özellik olarak tutabileceğimiz anlamına gelir. Diğer taraftan Resource yüklemek içinde aşağıdaki notasyon kullanılır.

![mk220_8.gif](/assets/images/2007/mk220_8.gif)

Resources özelliğide aslında ResourceDictionary tipinden bir koleksiyon döndürmektedir. Bu koleksiyonda IDictionary arayüzünü uyarlayan ancak içerisinde Hashtable bazlı çalışan hızlı bir koleksiyondur. (Resource ' ları Application elementi içerisinde ApplicationResource alt elementi içerisindede tanımlayabiliriz. Örneğin pencerelerdeki kontroller için ortak stilleri burada belirleyebiliriz. Kaynak yönetimi konusunada ilerleyen yazılarımızda değinmeye çalışacağım.)

Bu bilgilerden sonra StartUp ve Exit olaylarının bildirimlerini yapıp kodlayarak devam edebiliriz. Örnek senaryomuzda kullanıcı sembolik olarak bir pencere (Window) üzerinden ürün bilgisi girecek ve bu bilgiler uygulama seviyesinde yazılmış bir sınıfa ait örnekte saklanacaktır. Tahmin edileceği üzere tipe ait örnek Application nesnesinin özelliği ile elde edilebilecektir. Urun sınıfını ayrı bir fiziki dosyada projeye ekleyip aşağıdaki gibi geliştirebiliriz.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UsingApplicationObjects
{
    class Urun
    {
        public int Id;
        public string Ad;
        public double BirimFiyat;
    }
}
```

Dikkat edilecek olursa, sınıfın içerisinde özellik (property) veya yapıcı metod (constructor) dahil edilmemiştir. Nitekim burada C# 3.0 ile gelen object initializers tekniğinden yararlanılmak istenmektedir. Bu anlamda Visual Studio 2008 Beta 2' nin C# 3.0 içinde tam bir intellisense desteği verdiğini söylemeliyim. Örneğin Window1 üzerinde yer alan bir Button kontrolün Click olay metodunda Urun sınıfına ait bir nesneyi object initializers tekniği ile oluşturmak istediğimizde bu destek açık bir şekilde görülmektedir.

![mk220_9.gif](/assets/images/2007/mk220_9.gif)

Window1.xaml.cs dosyasının içeriğini aşağıdaki gibi tasarlayabiliriz.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;

namespace UsingApplicationObjects
{
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Urun bilgisayar = new Urun() { Id = 1000, Ad = "Bilgisayar", BirimFiyat = 1000 };
            Application.Current.Properties["SonBakilanUrun"] = bilgisayar;
        }
    }
}
```

Application nesnesi üzerinden o anki uygulama referansına erişmek için Current özelliği kullanılır. Sonrasında ise Properties özelliği üzerinden indeksleyici (indexer) yardımıyla, oluşturulan Urun nesne örneği SonBakilanUrun adıyla kaydedilir. Artık uygulama çalışma zamanında burada oluşturulan Application seviyesindeki özelliğe heryerden erişilebilir. Söz gelimi uygulamadan çıkarken bu özelliklerin içeriğini saklamak istersek App.xaml.cs içerisinde Exit olayını aşağıdaki gibi kodlamamız yeterli olacaktır.

```csharp
private void Application_Exit(object sender, ExitEventArgs e)
{
    try
    {
        Urun sonUrun = (Urun)Application.Current.Properties["SonBakilanUrun"]; // Properties koleksiyonuna eklenmiş SonBakilanUrun isimli bir anahtar var ise bunun değeri Urun tipinden elde edilir.
        if (sonUrun != null) // Eğer sonUrun nesne örneği null değilse...
        {
            IsolatedStorageFile storageFile = IsolatedStorageFile.GetUserStoreForDomain(); // Bu uygulamanın ve assembly' ın kimlik (Identity) bilgisine göre izole edilmiş kullanıcı odaklı depolama alanının elde edilmesini sağlar.
            IsolatedStorageFileStream stream = new IsolatedStorageFileStream("GlobalAppProperties.txt", FileMode.Create, storageFile); // Bu uygulama için diğerlerinden ayrılmış olan bir alana path' ten bağımsız olacak şekilde GlobalAppProperties.txt dosyasının açılmasını sağlar.
            StreamWriter writer = new StreamWriter(stream); // izole edilmiş alandaki dosya üzerine yazmak için bir StreamWriter kullanılabilir.
            writer.WriteLine(sonUrun.Id.ToString() + "|" + sonUrun.Ad + "|" + sonUrun.BirimFiyat.ToString()); // Bilgiler text dosyasına yazdırılır.
            writer.Close();
            stream.Close();
        }
    }
    catch (Exception exp)
    {
        MessageBox.Show(exp.Message);
    }
}
```

Tabiki burada illede IsolatedStorageFile kullanılması gerekmemektedir. Bunun yerine ikili (binary) veya xml serileştirmeden yararlanılabilir yada bilinen dosya saklama teknikleri tercih edilebilir. MSDN kaynaklarında bu konu işlenirken genel olarak yukarıdaki IsolatedStorageFile tipinin kullanıldığını söyleyebilirim. Gelelim ApplicationStartup olay metodunun içerisine. Bu olay metodunu ise aşağıdaki gibi kodladığımızı düşünelim.

```csharp
private void Application_Startup(object sender, StartupEventArgs e)
{
    try
    {
        IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();
        IsolatedStorageFileStream stream = new IsolatedStorageFileStream("GlobalAppProperties.txt", FileMode.Open, storage);
        StreamReader reader = new StreamReader(stream);
        while (!reader.EndOfStream)
        {
            string[] keyValue = reader.ReadLine().Split(new char[] { '|' });
            Urun urn = new Urun() { Id = Convert.ToInt32(keyValue[0]), Ad = keyValue[1].ToString(), BirimFiyat = Convert.ToDouble(keyValue[2].ToString()) };
            Application.Current.Properties["SonBakilanUrun"] = urn;
        }
    }
    catch (Exception exp)
    {
        MessageBox.Show(exp.Message);
    }
}
```

Eğer GlobalAppProperties.txt isimli dosya var ise buradan elde edilen değerlerden yararlanarak Urun nesne örneği oluşturulur ve Application nesnesinin özellikler koleksiyonuna eklenir. Söz gelimi yüklenen bu değeri yine bir Window'un yüklenmesi sırasında ele aldığımızı düşünebiliriz. Bunun için test olması açısından aşağıdaki gibi bir kod yazdığımızı düşünebiliriz.

```csharp
private void Window_Loaded(object sender, RoutedEventArgs e)
{
    Urun urn = (Urun)Application.Current.Properties["SonBakilanUrun"];
    if (urn != null)
        Title = urn.Ad.ToString(); // Bu pencerenin başlığında Urun' un adı gösterilir.
}
```

Sonuçta uygulama çalıştırıldığında Window ' un başlık kısmında aşağıdakine benzer bir görüntü elde edilir.

> Yapılan testlerde bilgisayarın kapatılıp açılmasından sonra da Application özelliklerinin yazıldığı dosyadan başarılı bir şekilde okunabildiği görülmüştür.

![mk220_10.gif](/assets/images/2007/mk220_10.gif)

Startup olayını komut satırı parametrelerini almak içinde kullanabiliriz. Burada Startup olay metodundaki StartupEventArgs parametresinin args isimli özelliği string tipinden bir dizi döndürmektedir. Bu özellik komut satırından girilen parametre değerlerini taşımaktadır. Ortam Visual Studio 2008 Beta 2, platformda.Net Framework 3.0 olunca, C# 3.0 ile gelen extension metodlar (örneğin Select, Min, Max, Average, Where, Sum vb...) ve anahtar kelimelerinde doğrudan desteklendiğini görüyoruz. Aşağıdaki ekran görüntüsünde bu durum açık bir şekilde görülmektedir.

![mk220_11.gif](/assets/images/2007/mk220_11.gif)

Örnek senaryomuzda komut satırında, bir veritabanı bağlantısı açmak için gerekli bazı bilgileri aldığımızı düşünebiliriz. Örneğin bu bilgiler sunucu adı, veritabanı adı, kullanıcı adı ve şifre olabilir. Buna göre komut satırından uygulamaya gönderilebilecek 4 farklı parametre söz konusudur. Bu parametre deseninin aşağıdaki gibi olduğunu farz edelim.

```bash
ProgramAdi.exe s:LONDON d:AdventureWorks u:Burak p:1234
```

Buna göre Application sınıfının Startup olay metoduna aşağıdaki kodları eklediğimizi düşünelim.

```csharp
if (e.Args.Length == 4)
{
    if (e.Args[0][0] == 's')
        Application.Current.Properties["Sunucu"] = e.Args[0].Substring(2, e.Args[0].Length-2);
    if (e.Args[1][0] == 'd')
        Application.Current.Properties["Veritabani"] = e.Args[1].Substring(2, e.Args[1].Length-2);
    if (e.Args[2][0] == 'u')
        Application.Current.Properties["Kullanici"] = e.Args[2].Substring(2, e.Args[2].Length-2);
    if (e.Args[3][0] == 'p')
        Application.Current.Properties["Sifre"] = e.Args[3].Substring(2, e.Args[3].Length-2);
}
```

Burada çok daha farklı algoritmalar düşünülebilir. Temel amaç, komut satırından gelecek 4 parametreninde elde edilmesi ve bunlardan var olanların uygulama nesnesinin ilgili özelliklerine set edilmesidir. Bundan sonraki adımımızda, herhangibir pencerenin StatusBar kontrolünde, buradaki bilgilerin içeriğini göstermeye çalışacağız. Söz gelimi Window1 içinde bir StatusBar kontrolünde bu bilgiler gösterilebilir. Üzülerek belirtmeliyimki bu StatusBar bileşenide WPF'den önce bildiğimiz StatusStrip değil. İtiraf etmek gerekirse, StatusBar içerisine kontrolleri atmak için bir süre uğraştım. Sonuçta artık kontrolleri ve içeriklerininde hiyerarşik bir XML yapısında düşünmemiz gerektiğini öğrendim. Buna göre StatusBar içerisinde barındırmak istediğimiz her kontrolü bir StatusBarItem elementi içerisinde göz önüne almalıyız. Dolayısıyla Window1.xaml içeriğini aşağıdaki gibi geliştirmemiz gerekmektedir.

```csharp
<Window x:Class="UsingApplicationObjects.Window1" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Window1" Height="294" Width="452" Loaded="Window_Loaded">
    <Grid>
        <Button Height="23" HorizontalAlignment="Left" Margin="16,57,0,0" Name="button1" VerticalAlignment="Top" Width="75" Click="button1_Click">Button</Button>
        <StatusBar Height="23" Name="stbBilgi" VerticalAlignment="Bottom">
            <StatusBarItem>
                <TextBlock Name="txtSunucu" Text="Sunucu:"></TextBlock> 
            </StatusBarItem>
            <StatusBarItem>
                <Separator/>
            </StatusBarItem>
            <StatusBarItem>
                <TextBlock Name="txtVeritabani" Text="Veritabanı:"></TextBlock>
            </StatusBarItem>
            <StatusBarItem>
                <Separator/>
            </StatusBarItem>
            <StatusBarItem>
                <TextBlock Name="txtKullanici" Text="Kullanıcı:"></TextBlock>
            </StatusBarItem>
            <StatusBarItem>
                <Separator/>
            </StatusBarItem>
        </StatusBar>
    </Grid>
</Window>
```

Dikkat edilecek olursa, her StatusBarItem içerisinde bir kontrol yer almaktadır.TextBlock kontrollerinin Text özelliklerinden yararlanarak gerekli bilgileri gösterebiliriz. Seperator kontrolü ise bu haliyle basit olarak diğer StatusBarItem kontrolleri içerisindeki bileşenlerin arasında bir ayraç görevi üstlenmektedir. (WPF kontrolleri ile ilişkili olarak ilerleyen yazılarımızda detaylı incelemeler yapmayı düşünüyorum.) Window1.xaml.cs dosyasında ise Loaded olay metodunu amacımıza yönelik olarak aşağıdaki gibi kodlayabiliriz.

```csharp
private void Window_Loaded(object sender, RoutedEventArgs e)
{
    Urun urn = (Urun)Application.Current.Properties["SonBakilanUrun"];
    if (urn != null)
        Title = urn.Ad.ToString();

      txtSunucu.Text += Application.Current.Properties["Sunucu"]!=null? Application.Current.Properties["Sunucu"].ToString():"Tanımlı Değil";
      txtVeritabani.Text += Application.Current.Properties["Veritabani"] != null ? Application.Current.Properties["Veritabani"].ToString() : "Tanımlı Değil";
      txtKullanici.Text += Application.Current.Properties["Kullanici"] != null ? Application.Current.Properties["Kullanici"].ToString() : "Tanımlı Değil";
}
```

Artık testimizi gerçekleştirebiliriz. Parametreleri test edebilmek için proje özelliklerine (Project Properties) gidip Command Line Arguments kısmını aşağıdaki gibi doldurmamız yeterli olacaktır.

![mk220_13.gif](/assets/images/2007/mk220_13.gif)

Uygulama test edildiğinde aşağıdakine benzer bir sonuç ile karşılaşırız.

![mk220_14.gif](/assets/images/2007/mk220_14.gif)

Bu son örneğimizde komut satırında WPF (Windows Presentation Foundation) uygulamasına gelen parametreleri nasıl ele alabileceğimizi incelemeye çalıştık. Dilerseniz yeni bir örnekle devam edelim. Bu örneğimizde uygulamanın kapatma sürecini kontrol altına almaya çalışacağız. Daha öncedende belirttiğimiz gibi, işletim sistemi seviyesinde gelebilecek olan kapatma taleplerini uygulama içerisinden geri çevirme şansına sahip olduğumuzu söylemiştik. Bunun için Application nesnesinin SessionEnding olay metodunu ele almamız yeterliydi. SessionEnding olay metodunda kilit nokta SessionEndingEventArgs isimli parametredir.

Bu parametre üzerinden erişilen ResonSessionEnding özelliği ResonSessionEnding enum sabiti tipinden bir değer alır. Bu enum sabitinin değerleri Logoff veya Shutdown'dır. SessionEndingEventArgs'ın bir diğer önemli özelliği ise Cancel üyesidir. Bu özelliğe atanan değere göre sürecin iptal edilmesi bir başka deyişle Shutdown veya Logoff'dan vazgeçilmesi sağlanabilir. Şimdi olay metodumuzu aşağıdaki gibi tasarladığımızı düşünelim. (Elbette SessionEnding olayının yüklenmesi için Application elementi içerisine ilgili niteliği eklemeyi unutmamalıyız.)

```csharp
private void Application_SessionEnding(object sender, SessionEndingCancelEventArgs e)
{
    MessageBoxResult cevap=MessageBox.Show("Bilgisayar " + e.ReasonSessionEnding.ToString() + " nedeniyle kapatılıyor. İptal etmek ister misiniz?", "Kapatma Sorusu", MessageBoxButton.YesNo, MessageBoxImage.Question);
    if (cevap == MessageBoxResult.No)
        e.Cancel = true;
}
```

Buradaki kod parçasında sembolik olarak kullanıcıya soru sorulmaktadır. Eğer kullanıcı hayır cevabını verirse işletim sisteminin kapanma süreci iptal edilir. Eğer uzun bir süre cevap verilmesse Windows'un standart End Program penceresi karşımıza çıkacaktır. Bu penceredeki progress tamamlandıktan sonra bile No denildiğinde Windows kapatma süreci yine iptal edilecektir. Aşağıdaki ekran görüntüsünde End Program çıktısınında ele alındığı durum gösterilmektedir.

![mk220_16.gif](/assets/images/2007/mk220_16.gif)

Session_Ending içerisinde daha önceden de bahsedildiği gibi kritik kaynak kaydetme gibi işlemler yapılabilir. Bir başka deyişle uygulamanın son durumunu (Application State) korumak adına çeşitli tedbirler alınabilir.

Bir WPF uygulamasının kapatılması ile ilgili olarak ShutdownMode özelliğinin değerlerine bakıldığından bahsetmiştik. Bu özelliği doğrudan Application elementi içerisinde belirtebileceğimiz gibi kod yardımıylada değiştirebiliriz. Varsayılan hali OnLastWindowsClose olan bu değeri aşağıdaki gibi OnExplicitShutdown olarak değiştirdiğimizde, uygulamanın kapatılabilmesi için Shutdown metodunun çağırılması gerekmektedir.

![mk220_17.gif](/assets/images/2007/mk220_17.gif)

Şimdi bunu test edelim. Bu amaçla uygulamaya ikinci bir Window daha dahil edilmiş ve ana pencereden Window2 penceresinin açılması sağlanmıştır. Window2 penceresini açma işlemini Window1 üzerindeki bir Button kontrolüne ait Click olay metodu içerisinde aşağıdaki gibi gerçekleştirmekteyiz.

```csharp
private void btnDigerForm_Click(object sender, RoutedEventArgs e)
{
    Window2 wnd2 = new Window2();
    wnd2.Show();
}
```

Şimdi uygulamamızı çalıştıralım. Sonradan açılan Window2 kapatıldığında uygulama doğal olarak sonlanmaz. Lakin ana pencere kapatıldığında da uygulama sonlanmaz. Bu iki aksiyonu yaptığımızda ve arka planda çalışan Process'lere baktığımızda uygulamanın gerçektende sonlandırılmadığını görebiliriz. Aşağıdaki ekran görüntüsünde bu durum açık bir şekilde görülmektedir.

![mk220_18.gif](/assets/images/2007/mk220_18.gif)

Dolayısıyla programdan çıkmak için Shutdown metodunun bilinçli olarak çağırılması gerekmektedir. Bunun için Window1 içerisinden başka bir Button yardımıyla aşağıdaki gibi bir metod çağrısında bulunduğumuzu düşünebiliriz. Dikkat edilecek olursa o anki uygulama nesnesi üzerinden Shutdown metodu çağırılabilmektedir.

```csharp
private void btnKapat_Click(object sender, RoutedEventArgs e)
{
    Application.Current.Shutdown();
}
```

Artık uygulamamız başarılı bir şekilde kapatılabilir. Daha önceden de belirtildiği gibi, işletim sistemine bir çıkış koduda gönderilebilir. Bunun için Shutdown metodunun aşırı yüklenmiş versiyonunu kullanmamız ve bir integer değeri parametre olarak vermemiz yeterlidir.

Yazımızda son olarak kod içerisinden fırlatılan istisnaların DispatcherUnhandledException olayında nasıl yorumlanabileceğini inceleyeceğimiz bir örnek geliştireceğiz. Bu amaçla uygulama içerisinden bilinçli olarak bir istisna (exception) nesne örneği fırlatmamız yeterli olacaktır. Window1 içerisinde bu amaçla iki farklı Button kontrolü yerleştirilmiş ve farklı istisna (Exception) nesne örneklerinin fırlatılması sağlanmıştır. Window1.xaml.cs içerisindeki yeni kodlar aşağıdaki gibidir.

```csharp
private void btnArgumentException_Click(object sender, RoutedEventArgs e)
{
    throw new ArgumentException();
}

private void btnStackOverFlow_Click(object sender, RoutedEventArgs e)
{
    throw new StackOverflowException();
}
```

Şimdi uygulamayı test edersek.Net Framework 3.0 çalışma zamanının (Run-time) aşağıdaki standart pencereyi çıkarttığını görürüz. Üstelik penceredeki cevabımızdan sonra rapor göndersekte, göndermesekte uygulamanın sonlandığını görürüz.

![mk220_19.gif](/assets/images/2007/mk220_19.gif)

İstersek bu pencerenin çıkmasını engelleyebiliriz. Bunun için, kod içerisinden ele alınmamış (Unhandled Exceptions) istisnaların ele alınacağını belirtmemiz gerekmektedir. Bu amacı gerçekleştirmek için DispatcherUnhandledException olayında yer alan DispatcherUnhandledExceptionEventArgs parametresinin Handled özelliğine true değerini atamamız yeterlidir. Ancak bu sadece ele alınmamış istisnalar için geçerlidir. Hatta ele alınamayacak cinsten bir istisna olduğunda da bu kontrol yeterli gelmeyebilir. Ne demek istediğimizi biraz daha net anlayabilmek için olay metodlarımızda bir değişiklik yapalım ve ArgumentException istisnasını yakalayacağımız bir try...catch bloğunu aşağıdaki gibi uygulama koduna dahil edelim.

```csharp
private void btnArgumentException_Click(object sender, RoutedEventArgs e)
{
    try
    {
        throw new ArgumentException();
    }
    catch (Exception excp)
    {
        MessageBox.Show("Bir istisna oluştu");
    }
}
```

Uygulama bu şekilde test edildiğinde ve ArgumentException istisnası oluşturulduğunda, try...catch bloklarından dolayı.Net Framework'ün çalışma zamanı istisna yönetim mekanizması devreye girerek yukarıda yer alan hata mesaj kutusunu göstermeyecektir. Bunun sebebi hatanın uygulama kodu içerisinde kontrollü bir şekilde try...catch blokları ile ele alınmış (Handled) olmasıdır. Ancak ele alınmamış istisnalar varsa ve bunlar oluştuğunda loglama gibi ek işlemler yapılmak isteniyor ve kurtarılabilirse uygulamanın çalışmasına devam etmesi isteniyorsa DispatcherUnhandledException olayı aşağıdaki şekilde olduğu gibi değiştirilebilir.

```csharp
private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
{
    // Burada EventLog dışından bir dosyaya yazdırma işlemleride gerçekleştirilebilir.
    EventLog.WriteEntry("Application", "X Uygulamasında "+DateTime.Now.ToString()+" zamanında "+e.Exception.Message+" hatası alınmıştır.", EventLogEntryType.Error); // e parametresi üzerinden oluşan istisna referansı Exception özelliği ile yakalanabilir.

    e.Handled = true; // Eğer oluşan istisna kurtarılabilecek cinstense, standart hata mesajı kutusunun çıkartılmaması ve uygulamanın çalışmaya devam etmesi sağlanmış olur. Bu özelliğin varsayılan değeri false dur.
}
```

Bu makalemizde WPF uygulamalarında çekirdek nesnelerden birisi olan Application nesnesini incelemeye çalıştık. Makalemizi sonlandırmadan önce sizlere WPF ile ilişkili faydalı olabilecek bir kaç kitap tavsiye etmek isterim.

[![mk220_20.gif](/assets/images/2007/mk220_20.gif)Essential Windows Presentation Foundation](http://www.amazon.com/Essential-Presentation-Foundation-Microsoft-Development/dp/0321374479/ref=pd_bbs_sr_3/103-1238478-4859812?ie=UTF8&s=books&qid=1188466213&sr=8-3)

Addison Wesley yayınlarının Essential serisinde yer alan kitaplar çoğunlukla içerikleri bakımından ileri seviye konuları dahi içeririler. Bu nedenle bu kitap tam anlamıyla bir başvuru kaynağı olarak düşünülebilir. Zaman zaman okura ağır gelebilecek bir içeriğe sahip olsada kitaplığımızda olması gereken bir kitap.

[![mk220_21.gif](/assets/images/2007/mk220_21.gif)Pro WPF: Windows Presentation Foundation in.Net 3.0](http://www.amazon.com/Pro-WPF-Windows-Presentation-Foundation/dp/1590597826/ref=pd_bbs_sr_2/103-1238478-4859812?ie=UTF8&s=books&qid=1188466213&sr=8-2)

Şu sıralarda okumakta olduğum bu kitap oldukşa başarılı. Üstelik son çıkan WPF kitaplarından birisi.1000 sayfa olması nedeniyle okuması biraz zaman alıyor:) Ancak içeriğinde WPF ile ilgili hemen her bilgiye ulaşabiliyoruz. Bu kitabı ısrarla tavsiye ederim.

[![mk220_22.gif](/assets/images/2007/mk220_22.gif)Professional WPF Programming](http://www.amazon.com/Professional-WPF-Programming-Development-Presentation/dp/0470041803/ref=pd_bbs_sr_4/103-1238478-4859812?ie=UTF8&s=books&qid=1188466213&sr=8-4)

Diğerlerine göre daha az sayfadan (480 sayfa) oluşan bu kitap içerisinde Expression Blend ile ilgili bir bölümde yer almakta. Çok kısa sürede okunabilecek bir kitap ama bazı yerlerde çok fazla detaya girilmediği için başka kaynaklara bakmayı gerektirebiliyor.

[![mk220_23.gif](/assets/images/2007/mk220_23.gif)Programming WPF](http://www.amazon.com/Programming-WPF-Chris-Sells/dp/0596510373/ref=pd_bbs_sr_6/103-1238478-4859812?ie=UTF8&s=books&qid=1188466213&sr=8-6)

O'Reilly yayınlarındaki favori yazarım Juval Lowy'ye ait bir kitap olmasada (Onun Programming WCF kitabı bir harika) yeni çıkan bir yayın olması ve 863 sayfalık (kabul edilebilir bir sayfa adedi) içeriğinin bulunması bu kitabın okunması için yeterli. Söz gelimi birinci bölümünü okuduğunuzda, WPF in temel yapısının öğrenmiş ve en büyük elementlerini kavramış oluyorsunuz.

Application nesnesi ile ilgili olarak değinmediğimiz pek çok nokta var. Söz gelimi bu nesneden yararlanarak uygulamadaki tüm pencere yada formları elde edebiliriz. Application'dan türetilen App sınıfı içerisine, tüm uyguladaki nesnelerin erişebileceği üyeler (Metodlar veya Özellikler) koyabiliriz vb. Bu tip konuların ve dahasının araştırılmasını siz değerli okurlarıma bırakıyorum. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](/assets/files/2007/UsingApplicationObjects.zip)