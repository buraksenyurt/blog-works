---
title: "WPF - Sayfa(Page) Kavramı, Navigasyon İşlemleri ve XBAP"
pubDate: 2007-10-04 12:00:00
categories:
  - WPF
tags:
  - WPF
  - XML
  - C#
  - .NET
  - ASP.NET
  - XAML
  - HTTP
  - Authentication
  - Caching
  - Generics
  - Visual Studio
---

# WPF - Sayfa(Page) Kavramı, Navigasyon İşlemleri ve XBAP
Değerli Okurlarım Merhabalar,

Yıllardır ister büyük çaplı ister küçük çaplı olsun pek çok proje web tabanlı olarak geliştirilmektedir. Projelerde bu tip bir seçime gitmenin en büyük nedenlerinden biriside web uygulamalarındaki dağıtım modelinin (Deployment Model) Windows tabanlı olanlara göre çok daha kolay olmasıdır. Her ne kadar.Net 2.0 ile birlikte gelen ClickOnce veya daha öncesinden beri var olan ActiveX gibi dağıtımı kolaylaştırabilecek teknolojiler var olsada bunlar Web uygulamalarına olan yönelimi azaltamamıştır. Sonuç itibariyle web tabanlı uygulamalarda, yazılan parçaları yorumlayacak bir tarayıcı penceresinin (Browser) olması yeterlidir.

Geriye kalan, söz konusu tarayıcı pencerelerinin yorumlayacağı HTML içeriklerinin oluşturulmasıdır. Bu amaçlada son derece gelişmiş sunucu (Server-Side) veya istemci taraflı (Client-Side) uygulama geliştirme modelleri mevcuttur. Asp.Net bu modellerden yanlızca birisidir. Ancak web tabanlı uygulamalarda tarayıcı tarafından bakıldığında dağıtım dışında sağlanan başka avantajlarda vardır. Örneğin, tarayıcı penceresi yardımıyla uygulama (Application) alanı içerisinde bir sayfadan diğerine geçmek bir başka deyişle navigasyon işlemleri ile dolaşmak çok kolaydır. Bu tip bir kullanım kolaylığını Windows uygulamalarına kazandırmak ekstra kodlamayı gerekirmektedir.

Microsoft,.Net Framework 3.0 ile birlikte Windows uygulamalarının tarayıcı pencereleri içerisinde çalıştırılabilmesini sağlayacak bir yenilik getirmektedir. Kısacası XBAP (XAML Browser Applications) olarak adlandırılan bu modelde, kısıtlamaları ile birlikte bir WPF (Windows Prensetation Foundation) uygulamasını bir tarayıcı penceresinde açmak mümkündür. Yazı dizimizde bu konuyada değiniyor olacağız. Ama öncesinde bunların temelini oluşturan sayfa (Page) kavramını anlamak gerekmektedir.

WPF uygulamalarını sayfa tabanlı (Page-Based) olacak şekilde tasarlayabilmekteyiz. Burada sayfadan kasıt Page tipinden bir nesnedir. WPF mimarisinde sayfa tabanlı uygulamalarda kendi içlerinde iki ana parçaya ayrılmaktadır. Bunlardan birincisi XBAP uygulamaları, diğeri ise kendi başına çalışan (Stand-Alone) uygulamalardır. Sayfalar (Pages) aslında daha önceki makalelerimizde de incelediğimiz Window tipine benzetilebilir. Lakin arada çok önemli bir fark vardır. Window tipi temel olarak bir taşıyıcı (Container) görevini üstelenebilmektedir. Bu sebeptende ContentControl tipinden türetilmiştir. Ne varki Page tipi doğrudan FrameworkElement tipinden türemektedir. Dolayısıyla Page tiplerinin kullanılabilmesi için bunu servis edecek bir sunucuya (Host) ihtiyaç vardır. Söz konusu özet bilgilere göre sayfa bazlı (Page-Based) uygulamaları aşağıdaki gibi kategorize edebiliriz.

Standalone (Kendi başına
çalışan uygulamalar)
NavigationWindow içerisinde kullanılabilen sayfalar.

Bir pencerede (Window) yer alan Frame veya Frame'ler içerisinde kullanılabilen sayfalar.

Başka bir sayfa (Page) içerisindeki Frame veya Frame'lerde kullanılabilen sayfalar.

XBAP (Xaml Browser Applications) Uygulamaları
Internet Explorer veya destek veren başka bir tarayıcı (Browser) üzerinde çalışabilen sayfalar. Lightweight olarakta adlandırılan basit web tabanlı dağıtım modeli için uygun bir yapı sunmaktadır.

Sayfa tabanlı (Page-Based) uygulamalarda kullanılan genel tipler aşağıdaki sınıf diagramında (Class Diagram) görüldüğü gibidir.

![mk226_1.gif](images/mk226_1.gif)

Yukarıdaki sınıf diagramında sayfa-tabanlı (Page-Based) uygulamalarda başrol oynayan sınıflardan (class) bazıları yer almaktadır. Window sınıfı daha önceki windows programlamada yer alan Form sınıfının karşılığı olarak düşünülebilir. Bir içerik kontrolüdür (ContentControl). Bu nedenle kendi içerisinde başka elementleride barındırmaktadır. Söz gelimi Window içerisinde bir Frame tanımlanıp bu Frame içerisinde de farklı sayfalar (Page) yer alabilir. Frame tipi aslında bir sayfa içerisinde bağımsız bir parça olaraktanda düşünülebilir. Frame'leri bir Page veya Windows elementi içerisinde kullanabiliriz. Temel görevleri aslında web uygulamalarından bilinen benzeri ile aynıdır. Bir başka deyişle taşıyıcı kontrol içerisinde başka sayfaların (Page) gösterilebilmesini sağlamaktadır. Bu taşıyıcı özelliği nedeni ilede tahmin edileceği gibi ContentControl sınıfından türemektedir. NavigationWindow, içerisinde Page elementlerini içerebilen bir tiptir. Varsayılan olarak Page elementi içeren bir XAML içeriği code-behind dosyası ile birlikte çalıştırıldığında çalışma zamanında otomatik olarak bir NavigationWindow nesnesi örneklenmektedir. NavigationWindow nesneleri çalışma zamanında dinamik olaraktanda örneklenebilir ve sayfa içeriklerini göstermesi sağlanabilir.

Bu kısa teorik bilgilerden sonra dilerseniz basit örnekler yardımıyla konuyu daha iyi anlamaya çalışalım. Eğer aynı pencere üzerinde yer alacak ve aralarında geçişler yapılabilecek sayfalardan bahsediyorsak doğal olaraktan bunların arasında dolaşabilmek gerekmektedir. Dolaşma işlemleri için kullanılabilecek en basit kontrol Hyperlink bileşenidir. Bu bileşenin NavigateUri özelliğinden yararlanılarak başka bir sayfaya geçilmesi, aynı sayfa içerisinde veya başka bir sayfa içerisinde yer alan bir noktaya gidilmesi (burada anchor benzeri bir kullanımdan bahsediyoruz), başka bir NavigationWindow içerisinde bir sayfaya ve hatta var olan geçerli bir Url adresine gidilmesi sağlanabilir. Dolayısıyla ilk örneğimizde bu durumu analiz etmeye çalışıyor olacağız. Bu amaçla Visual Studio 2008 Beta 2 sürümünde yeni bir WPF uygulaması açıp XAML içerikleri başlangıçta aşağıdaki gibi olan iki sayfa (Page) tasarlamamız yeterlidir.

> Page tiplerini bir WPF uygulamasına eklerken öğelerden (Item) yararlanılabilir. Bunun için projeye sağ tıklayıp Add New Item penceresinden yada, doğrudan sağ tıklayınca çıkan menülerden Add->Page ile gerçekleştirebiliriz.
> ![mk226_5.gif](images/mk226_5.gif)

![mk226_2.gif](images/mk226_2.gif)

MainPage.xaml

```xml
<Page x:Class="PageKullanimi.MainPage" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Ana Sayfa" WindowHeight="250" WindowWidth="400" WindowTitle="Azon Giriş Sayfası" Loaded="Page_Loaded">
    <Page.Background>
        <ImageBrush ImageSource="Iceberg.jpg" Opacity="0.4"/>
    </Page.Background>
    <Grid>
        <Label Foreground="Black" FontSize="15" FontWeight="Bold" Height="30" HorizontalAlignment="Left" VerticalAlignment="Top" Width="113" Margin="9,21,0,0">
            <Hyperlink NavigateUri="PageX.xaml" ToolTip="Bir sonraki sayfaya geçmenizi sağlar">Sonraki Sayfa</Hyperlink>
        </Label>
        <Button Name="btnBilgiSayfasi" Background="LightGray" Width="120" Height="30" FontSize="12" FontWeight="Bold" HorizontalAlignment="Left" Margin="9,107,0,0" VerticalAlignment="Top">
            <Hyperlink Foreground="Red" NavigateUri="Page2.xaml">Bilgi Giriş Sayfası</Hyperlink>
        </Button>
        <TextBox Name="txtHosgeldinMesajim" Margin="9,60,33,0" Height="21" VerticalAlignment="Top" />
    </Grid>
</Page>
```

MainPage içerisinde ilk dikkati çeken noktalardan birisi Page elementi ve özellikleridir. WindowHeight ve WindowWidth özellikleri ile çalışma zamanındaki NavigationWindow penceresinin boyutları set edilmektedir. WindowTitle özelliği ile sayfanın NavigationWindow içerisinde gösterilirken sahip olacağı başlık değeri verilmektedir. Title özelliği ise, navigasyon işlemlerinin yapıldığı Combobox içerisindeki başlık bilgisini belirlemektedir. Aşağıdaki şekil çalışma zamanındaki bir ekran görüntüsü olup bahsedilen özellikleri ifade etmektedir.

![mk226_7.gif](images/mk226_7.gif)

> Page sınıfının Window sınıfında olduğu gibi Show veya Hide gibi metodları bulunmamaktadır. Bunun en büyük nedeni Page sınıflarına ait nesne örnekleri aralarında gezinirken navigasyon kontrollerinden yararlanılabilmesidir. Dolayısıyla klasik windows programcılığından bildiğimiz Show veya Hide gibi işlemlere gerek kalmamaktadır.

Bunların dışında MainPage içerisinde Label ve Button kontrollerine ait elementler içerisinde Hyperlink alt elementleri kullanılmıştır. Hyperlink bileşeni bağımsız bir element olarak kullanılamamaktadır. Dolayısıyla Inline-Flow tipinden bir kontroldür. Diğer taraftan bu elementin NavigateUri özelliği ile gidilmek istenen sayfalar belirtilmektedir.

> Hyperlink elementi Page yerine bir Window içerisinde kullanılmak istendiğinde belirtilen adrese otomatik olarak gidilemeyecektir. Böyle bir durumda Window sınıfının RequestNavigate olayının bilinçli olarak ele alınması ve yönlendirme işleminin manuel olarak yapılması gerekmektedir.

Label kontrolünde yer alan Hyperlink elementinde kasıtlı olarak projede yer almayan PageX.xaml'e gidilmeye çalışılmaktadır. Burada amaç olmayan bir adrese gidilmek istendiğinde ne olacağının analiz edilmesidir. Button kontrolü içerisinde yer alan Hyperlink elementinde ise, XAML içeriği aşağıda yer alan Page2 isimli sayfaya gidilmektedir.

![mk226_3.gif](images/mk226_3.gif)

Page2.xaml

```xml
<Page x:Class="PageKullanimi.Page2" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" WindowTitle="Bilgi Giriş Sayfası" Title="Bilgi Girişi" Loaded="Page_Loaded">
    <Grid>
        <Label FontSize="12" FontWeight="Bold" Height="25" HorizontalAlignment="Left" VerticalAlignment="Top" Width="85" Margin="0,24,0,0">Tc Kimlik No</Label>
        <Label FontSize="12" FontWeight="Bold" HorizontalAlignment="Left" Width="85" Margin="0,83,0,0" Height="25" VerticalAlignment="Top">Semt</Label>
        <Label FontSize="12" FontWeight="Bold" Height="25" HorizontalAlignment="Left" VerticalAlignment="Top" Width="85" Margin="0,46,0,0">Aranan Hat</Label>
        <Button FontSize="14" Margin="125,117,62,0" Name="btnKontrolSayfasi" Click="btnKontrolSayfasi_Click" Height="24" VerticalAlignment="Top">
            Kontrol Sayfası
        </Button>
        <TextBox Name="txtTcNo" Height="21" Margin="120,27,60,0" VerticalAlignment="Top" />
        <TextBox Name="txtArananHat" Height ="21" Margin="120,57,60,0" VerticalAlignment="Top" />
        <TextBox Name="txtSemt" Height ="21" Margin="120,89,60,0" VerticalAlignment="Top" />
    </Grid>
</Page>
```

İlk olarak örneği çalışma zamanında test ederek işe başlayalım. Uygulamanın yürütülmeye başlaması halinde MainPage.xaml sayfasının örneklenmesi için App.xaml'e ait Application elementi içerisindeki StartupUri özelliğinin değeri MainPage.xaml olarak ayarlanmıştır. Aşağıdaki Flash görselinde ilk çalışma hali gösterilmektedir.

Dikkat edilecek olursa çalışma zamanında (Run-Time) otomatik olarak bir navigasyon çubuğu oluşturulmuştur. İlk aşamada bu çubuk üzerindeki düğmeler pasiftir. Aktif olmaları için sayfalar arasında geçiş yapılması gerekmektedir. Sayfalar arası geçiş yapıldıktan sonra navigasyon düğmeleri ile ileri ve geri yönlü hareketler yapılabilir. Bununla birlikte Combobox kontrolünden yararlanılaraktanda diğer sayfalara daha kolay bir şekilde geçiş yapılmasıda sağlanabilir. Bu örnekte PageX.xaml sayfasına geçiş yapılmasını sağlayan Label kontrolüne basılmamıştır. Bu yapıldığı takdirde söz konusu sayfa olmadığı için aşağıdaki ekran görüntüsünde yer alan bir çalışma zamanı istisnası (exception) alınacaktır.

![mk226_6.gif](images/mk226_6.gif)

Görüldüğü gibi basit bir IOException alınmıştır. Elbetteki programatik olarak uygulamayı tasarlarken olmayan sayfalara gidilmemesini sağlamak geliştiricinin görevidir. Lakin NavigateUri ile var olan bir sayfa dışında geçerli bir URL adresinede gidilebilmesi sağlanabilmektedir. Bir başka deyişle var olan bir web sayfasını çalışma zamanında oluşturulan NavigationWindow içerisinde bir sayfa olarak göstermek mümkündür. Bu gibi durumlarda gidilmek istenen URL veya Sayfa bilgisinin geçerli olmaması halinde programın istem dışı bir şekilde sonlanmasının önüne geçmek için Application nesnesinin NavigationFailed olayını ele almak ve içerisinde istisna bilgisini kontrollü bir şekilde yakalamak en doğru yaklaşım olacaktır. Yukarıdaki örnekte bunu uygulamak istediğimiz App.xaml ve App.xaml.cs içeriklerinin aşağıdaki gibi tasarlanması yeterlidir.

App.xaml;

```xml
<Application x:Class="PageKullanimi.App" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" StartupUri="MainPage.xaml" NavigationFailed="Application_NavigationFailed">
    <Application.Resources>
    </Application.Resources>
</Application>
```

App.xaml.cs;

```csharp
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows;

namespace PageKullanimi
{
    public partial class App : Application
    {
        private void Application_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            if (e.Exception != null)
            {
                MessageBox.Show(e.Uri.ToString() + " adresi bulunamadı");
                e.Handled = true;
            }
        }
    }
}
```

Örnekte basit olması açısında sadece hataya neden olsan sayfanın Uri bilgisi bir MessageBox içerisinde gösterilmektedir. Olay metoduna gelen NavigationFailedEventArgs tipinden e parametresinin Handled özelliğine true değeri atanmasının sebebi hatanın kontrollü bir şekilde ele alındığının belirtilmesidir. Aksi durumda program yine hata sonrası, kullanıcıya oluşan hatanın gönderilip gönderilmeyeceğini soran hepimizin yakından tanıdığı mesaj kutusu ile sonlandırılacaktır. Burada yakalanan hatalar çok doğal olarak başka amaçlarlada değerlendirilebilir. Örneğin Log'lanarak, oluşan hatalar ile ilişkili genel istatistik ve analizlerin yapılması sağlanabilir.

Hyperlink kontrolünü kullanarak web sayfalarınada gidilebildiğinden bahsetmiştik. Bunun dışında bir sayfa içerisinde yer alan herhangibir konuma gidilmeside sağlanabilirki bu durum parçalı navigasyon (Fragment Navigation) olarak adlandırılmaktadır. Şimdi bu iki kullanım şeklini ele alacağımız bir örnek üzerinden ilerleyelim. Bu amaçla projemize Page3.xaml ve Page4.xaml sayfalarını aşağıdaki içerikleri ile eklediğimizi düşünebiliriz.

![mk226_8.gif](images/mk226_8.gif)

Page3.xaml;

```xml
<Page x:Class="PageKullanimi.Page3" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" WindowTitle="Doğrulama Sayfası" Title="Doğrulama" WindowHeight="250" WindowWidth="400">
    <Grid>
        <Button Name="btnCSHarpNedir" FontSize="14" FontWeight="Bold" Height="25" Margin="20,36,30,0" VerticalAlignment="Top">
            <TextBlock>
                <Hyperlink Foreground="Red" NavigateUri="http://www.csharpnedir.com">C#Nedir?</Hyperlink>Sayfasına Gider.
            </TextBlock>
        </Button>
        <Button Name="btnIceberg" FontSize="14" FontWeight="Bold" Height="25" VerticalAlignment="Top" Margin="20,90,30,0">
            <TextBlock>
                <Hyperlink NavigateUri="Page4.xaml#txtBilgi" Foreground="Red">Iceberg</Hyperlink> 
            </TextBlock>
        </Button>
    </Grid>
</Page>
```

Sayfanın içerisinde kullanılan iki adet Hyperlink kontrolü bulunmaktadır. Bunlardan birisi C#Nedir? sitesine, diğeri ise Page4.xaml sayfası içerisinde txtBilgi isimli bileşenin olduğu yere yönlendirme yapmaktadır. İkinci navigasyon işlemi aslında parçalı navigasyon (Fragment Navigation) işlemi için bir örnektir. Öyleki Page4.xaml sayfası içerisinde yer alan txtBilgi isimli kontrol sayfanın alt kısmında yer almaktadır. Bu nedenle sayfa içerisindeki bu yere navigasyon işlemi ile gidilmesi ve odaklanılması mümkün olabilmektedir.

![mk226_9.gif](images/mk226_9.gif)

Page4.xaml;

```xml
<Page x:Class="PageKullanimi.Page4" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Antartika Iceberg" WindowTitle="Iceberg">
    <Grid>
        <ScrollViewer VerticalScrollBarVisibility="Visible">
            <Canvas Height="299" Width="284">
                <Image Canvas.Left="18" Canvas.Top="51" Height="104" Name="image1" Width="150" Source="Iceberg.jpg" />
                <Label Canvas.Left="18" Canvas.Top="26" Height="23" Name="label1" Width="120">Antartika Hakkında</Label>
                <Label Canvas.Left="18" Canvas.Top="166" Height="23" Name="label2" Width="120">
                    <Hyperlink NavigateUri="Page4.xaml#txtBilgi">Genel Bilgi</Hyperlink>
                </Label>
                <TextBox Height="25" Name="txtBilgi" Canvas.Left="18" Canvas.Top="274" Width="249" />
            </Canvas>
        </ScrollViewer>
    </Grid>
</Page>
```

Page4.xaml içerisinde de bu sayfadaki txtBilgi kontrolüne odaklanılmasını sağlayan bir Hyperlink bileşeni bulunmaktadır. Dikkat edilecek olursa parçalı navigasyon işlemlerinde NavigateUri özelliğinde # işaretinden sonra bir bilgi yer almaktadır. Bu bilgi gidilmek istenen kontrolün Name özelliğinin (Property) değeridir. Örneğin çalışma zamanındaki işleyişi aşağıdaki Flash animasyonunda olduğu gibidir.

Görüldüğü gibi Page3.xaml içerisinde yer alan düğme kontrollerine basıldığında [C#Nedir?](http://www.csharpnedir.com/) sayfasına veya Page4.xaml içerisindeki txtBilgi isimli TextBox kontrolünün olduğu yere gidilmektedir. Herhangibir internet veya intranet adresine gidilmesini sağlayan teknikte mutlaka hata kontrolü yapılmalıdır. Diğer taraftan bir web sayfasına gidildiğinde sayfada görünen kısım klasik windows programlamadan tanıdığımız WebBrowser kontrolünün sunduğu ortama benzemektedir. Bu sebepten dolayı elde edilen sayfa içerisinde arama yapmak, dinamik kod çalıştırmak gibi işlemler yapılamamaktadır.

> Parçalı navigasyon (Fragment Navigation) işleminin olabilmesi için, söz konusu sayfa içerisinde aşağı yukarı hareket edilebilmesi bir başka deyişle scrolling olması gerekmektedir. Bu amaçla ScrollViewer kontrolünden yararlanılabilir. Söz konusu kontrolün VerticallScorllBarVisibility ve HorizontalScrollBarVisibility özelliklerine ilgili değerler atanarak dikey veya yatay yönde kaydırma çubuklarının (Scroll Bar) gösterilmesi (yada tam tersi) sağlanabilir.

Navigasyon işlemleri istenirse manuel olarak kod tarafından da gerçekleştirilebilir. Bu noktada NavigationWindow içerisinde üst kısımda görünen navigasyon kontrolleri ve menünün yaptığı işlerin kod yardımıylada gerçekleştirilmesi mümkündür. Bunun için NavigationService tipinden ve üyelerinden (Members) yararlanılabilir. Sıradaki örnekte yeni bir sayfaya geçiş işlemini kod ile nasıl yapabileceğimize bakıyor olacağız. Page2.xaml içerisindeki btnKontrolSayfasi isimli Button düğmesine tıklandığında Page3.xaml sayfasına geçilmesini sağlamak için ilgili olay metodunda aşağıdaki kodları yazmak yeterli olacaktır.

```csharp
private void btnKontrolSayfasi_Click(object sender, RoutedEventArgs e)
{
    this.NavigationService.Navigate(new Page3());
}
```

NavigationService referansını Button bileşenine ait Click olay metodu içerisine yakaladıktan sonra Navigate fonksiyonuna parametre oalrak Page3 tipinin yeni bir nesne örneği verilmektedir. Böylece Page3 nesne örneği oluşturulup NavigationWindow içerisinde gösterilmesi sağlanmaktadır. Buna göre örneğin çalışma zamanındaki durumu aşağıdaki Flash görselindeki gibi olacaktır.

İstenirse çalışma zamanında yeni bir NavigationWindow oluşturulması ve bir sayfanın bu örnek üzerinde açılması sağlanabilir. Aşağıdaki örnek kod parçası ile, düğmeye basıldığı zaman Page3 isimli sayfanın yeni bir pencere içerisinde açılması sağlanmaktadır.

```csharp
private void btnKontrolSayfasi_Click(object sender, RoutedEventArgs e)
{
    NavigationWindow nvgWnd = new NavigationWindow();
    Page3 pg3 = new Page3();
    pg3.WindowTitle = "Yeni Pencerede Açılan Doğrulama Sayfası";
    pg3.Title = "Doğrulama(Yeni)";
    pg3.WindowWidth = 300;
    pg3.WindowHeight = 240;
    //pg3.ShowsNavigationUI = false;
    nvgWnd.Content = pg3;
    nvgWnd.Show();
}
```

İlk olarak yeni bir NavigationWindow nesne örneği oluşturulmaktadır. Sonrasında ise bu pencerede gösterilmek istenen sayfa örneklenir. Sayfanın WindowTilte, Title, WindowWidth, WindowHeight gibi özellikleri set edildikten sonra NavigationWindow nesne örneğinin Content özelliğine oluşturulan Page3 nesne örneği atanır. Son olarak Show metodu ile yeni pencerenin gösterilmesi sağlanmaktadır. Buradaki yorum satırı açılırsa eğer, yeni pencerede navigasyon kontrollerinin gösterilmemesi sağlanmış olur. Uygulamayı bu şekilde test ettiğimizde aşağıdaki Flash görselindeki etkiler görülecektir.

Sayfalar istenirse Frame'ler içerisinde gösterilebilirler. Böylece bir NavigationWindow içerisinde birden fazla Frame kullanılarak birden fazla sayfanın aynı anda gösterilmesi sağlanabilir. Örneğin, içerisinde harici bir web sitesini, uygulamanın kendisi, yardım dökümanını barındıracak şekilde bir pencere geliştirilebilir. Frame tipi kendi içerisinde çeşitli elementler barındırabilmektedir ancak genel kullanım amacı Page tiplerini taşımasıdır. Bu tanımlamalar Frame tipinin webdeki kullanım şeklini tam olarak andırdığınıda göstermektedir. Konuyu daha net anlayabilmek için bir örnek üzerinden ilerlemekte fayda olacağı kanısındayım. Bu amaçla projeye aşağıdaki XAML içeriğine sahip yeni bir pencere (Window) eklediğimizi düşünelim.

```xml
<Window x:Class="PageKullanimi.FrameKullanimi" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Frame Kullanimi" Height="400" Width="300">
    <Grid Margin="2">
        <Grid.RowDefinitions>
            <RowDefinition/>
            <RowDefinition/>
        </Grid.RowDefinitions>
        <Frame NavigationUIVisibility="Automatic" Grid.Row="0" BorderBrush="Black" BorderThickness="3" Source="Page3.xaml"/>
        <Frame Grid.Row="1" BorderBrush="Gold" BorderThickness="3" Source="Page2.xaml"/>
    </Grid>
</Window>
```

Frame kullanımını kolaylaştırmak için pencere içerisinde iki satırdan oluşan bir Grid kontrolü konulmuştur. Bu amaçla Grid kontrolü içerisine RowDefinition elementleri ile iki satır eklenmiştir. Hangi Frame'in hangi satırda gösterileceğini belirlemek için Grid.Row elementlerinden yararlanılmaktadır. Her Frame elementinin Source niteliklerine (attribute) atanan değerler ile içlerinde gösterecekleri sayfalar belirlenmektedir. NavigationUIVisibility niteliğine atanan değer ile navigasyon kontrolünün Frame içerisinde gösterilip gösterilmeyeceği veya örnekteki gibi bunun otomatik olarak set edilip edilmeyeceği belirlenebilir. Söz konusu Frame'ler dikkat edilecek olursa Page yerine bir Window elementi içerisinde kullanılmıştır. Örnek yürütüldüğünde aşağıdaki Flash animasyonunda olduğu gibi bir çalışma zamanı sonucu elde edilir.

Flash animasyonundan görülebileceği gibi ilk etapta navigasyon kontrolleri gösterilmemektedir. Ancak Frame'ler içerisinde yer alan sayfalar üzerindeki kontroller yardımıyla hareket edildikten sonra navigasyon kontrolleri görülmektedir. Tabi istenirse NavigationUIVisibility özelliğine Visible değeri atanarak navigasyon kontrolünün başlangıçta çıkmasıda sağlanabilir. Yukarıdaki örnekte meydana gelen işlemler aşağıdaki grafikle daha net anlaşılabilir.

![mk226_10.gif](images/mk226_10.gif)

İstenirse sayfalar başka sayfaların içerisinde de kullanılabilir. Böyle bir durumda iç içe sayfalar (Nested-Page) söz konusu olmaktadır. Aşağıdaki XAML içeriğinde bir nested-page tasarımı örneği görülmektedir.

![mk226_11.gif](images/mk226_11.gif)

XAML içeriği;

```xml
<Page x:Class="PageKullanimi.NestedPageKullanimi" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Nested Page Orneği" WindowWidth="300" WindowHeight="300">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="89" />
            <RowDefinition Height="211" />
        </Grid.RowDefinitions>
        <StackPanel Grid.Row="0">
            <Label Foreground="Red" FontSize="14" FontWeight="Bold">Arama</Label>
            <TextBox Text="Aranacak kelimeyi giriniz"/>
            <Button Name="btnAra" Content="Ara" FontSize="12" FontWeight="Bold" Background="Gold" Foreground="Black"/>
        </StackPanel>
        <Frame NavigationUIVisibility="Automatic" Grid.Row="1" BorderBrush="Black" BorderThickness="3" Source="Page3.xaml"/>
    </Grid>
</Page>
```

Dikkat edilecek olursa Page elementi içerisinde normal elementler dışında birde Frame elementi kullanılmaktadır. Bu element içerisinde Source niteliği ile gösterilecek olan sayfa belirtilmektedir. Böylece çalışan sayfa içerisinde birbirlerine bağlı başka sayfalarında gösterilebilmesi sağlanmaktadır. Olayı aşağıdaki grafik ile daha net kavrayabiliriz.

![mk226_12.png](images/mk226_12.png)

Uygulamanın çalışma zamanındaki görüntüsü aşağıdaki Flash animasyonunda olduğu gibidir.

Burada dikkat çekici noktalardan biriside Frame içerisinde sayfalar arasında gezinirken navigasyon kontrolünün sayfanın üst kısmında çıkmış olmasıdır. Bu durumda Frame'lerin kendi navigasyon çubuklarına sahip olması sağlanabilir. Bunun için Frame elementinin JournalOwnerShip adı verilen niteliğinden yararlanılır. Bu nitelik Automatic, OwnsJournal ve UsesParentJournal olmak üzere üç farklı değerden birisini alabilir. OwnsJournal değeri seçildiğinde aşağıdaki ekran görüntüsünde olduğu gibi Frame'in navigasyon kontrolü kendi içerisinde çıkacak, sayfanın kendi navigasyon kontrolü ise en üst tarafta yer alacaktır. Doğal olarak bunlar birbirleriylede karışmayacak şekilde çalışmaktadır.

![mk226_13.gif](images/mk226_13.gif)

UsesParentJournal değerinin seçilmesi halinde ise, Frame elementinin navigasyon kontrolü bu örneğin ilk versiyonunda olduğu gibi üst tarafta yer alacaktır. Aslında burada analiz edilmesi gereken bir durum daha vardır. Buda sayfanın kendisinin navigasyon işlemlerine sahip olması halidir. Yani aşağıdaki grafikteki gibi bir senaryo olduğunu düşünelim.

![mk226_14.gif](images/mk226_14.gif)

Dikkat edileceği üzere Frame içerisinden başka iki sayfaya daha geçilebilmektedir. Diğer taraftan sayfanın kendiside (yani Frame'ide içeren Page) Page2.xaml sayfasına geçiş yapabilmektedir. Bu tip bir durumda UsesParentJournal değerinin seçilmesi özellikle XBAP uygulamalarında oldukça işe yarayacaktır. Söz konusu kullanımda sayfalar çok fazla parçalandığından zaman zaman takibin zorlaştığıda görülmektedir. Dolayısıyla Nested-Page tekniğinin bir alternatif olarak bilinmesinde ve uygun vakkalar bulunduğunda ele alınmasında yarar vardır.

Yazımızın içerisinde belirttiğimiz gibi navigasyon işlemlerini manuel olarak kod tarafından da yapabiliriz. Burada önemli olan nokta NavigationService özelliği ile elde edilecek referans olacaktır.Bununla birlikte bir sayfanın navigasyon süreci içerisinde talep edilmesi halinde neler olduğunun bilinmesinde de yarar vardır. Bir başka deyişle, navigasyon sürecindeki yaşam döngüsünü bilmekte yarar vardır. Temelde süreç bir sayfanın talep edilmesi ile başlar. Bu talep işlemi basit bir Hyperlink kontrolü ile olabileceği gibi, NavigationService sınıfının Navigate metodu ilede olabilir. Sonrasında talep edilen sayfanın yeri tespit edilir. Tahmin edileceği üzere burada bir sorun olması halinde çalışma zamanı istisnası oluşacaktır. Sayfa yeri tespit edildikten sonra bilgileri getirilir. Örneğin talep edilen sayfa bir internet sayfası ise download işlemi gerçekleşir. Bu arada eğer sayfanın indirilmesi sırasında ilişkili kaynaklar var ise bunlarında getirilmeside söz konusudur. Takip eden adımda sayfa için bir ayrıştırma (parsing) işlemi uygulanır ve sayfanın nesne ağacı (Object Tree) oluşturulur. Bu işlemi takiben sayfaya ait Initialized ve Loaded olaylarıda sırasıyla tetiklenir. Son aşamada sayfa Render işlemine tabi tutulur ve gösterilir. Bu noktada eğer parçalı navigasyon (Fragment Navigation) işlemi yapılmışsa ilgili kontrole gidilmesi sağlanır. Aşağıdaki temsili grafik bu işleyişi kısaca özetlemektedir.

![mk226_15.gif](images/mk226_15.gif)

Birde bu süreç içerisinde tetiklenen bazı olaylar söz konusudur. Bu olaylar Application, Frame, NavigationWindow yada NavigationService'in kendisi tarafından ele alınabilir. Daha çok tercih edilen, söz konusu olayları Application nesnesi seviyesinde ele almaktır. Böylece uygulama içerisinde yer alabilecek tüm sayfalar için ortak bir noktada olayların kontrol edilebilmesi sağlanmış olur. Burada bahsedilen olaylar Navigating, Navigated, NavigationProgress, LoadCompleted, FragmentNavigation, NavigationStopped ve NavigationFailed'dır. Bu olayların işleyiş sırası aşağıdaki şekildeki gibidir.

![mk226_16.gif](images/mk226_16.gif)

İlk olarak süreç bir sayfanın talep edilmesi ile başlar. Sonrasında ise ilgili olaylar tetiklenir. Aşağıdaki tabloda söz konusu olaylar ile ilişkili bilgiler verilmektedir.

Olay (Event)
Olay Hakkında Kısa Bilgi

Navigating
Yeni bir navigasyon talebi geldiğinde çalışan olaydır.

Navigated
Navigasyon başlamıştır ve talep edilen sayfa ile ilgili bilgiler gelmektedir. Ancak sayfanın tamamı henüz gelmemiştir.

NavigationProgress
Bu olaya ilişkin metod yazıldığında, sayfanın yüklenmesi tamamlanıncaya kadar bilgilendirme yapılması sağlanabilir. Örneğin sayfanın yüzde olarak kaçının tamamlandığı bilgisi gösterilebilir. Aslında bu olay klasik windows programlamadaki BackgroundWorker kontrolünün ProgressChange olayınınkine benzer bir görev üstlenmektedir. NavigationProgress olayı, talep edilen sayfa ile ilgili her 1Kb bilgi geldiğinde tetiklenmektedir.

LoadCompleted
Talep edilen sayfanın ayrıştırma (Parse) işlemi tamamlanmıştır. Ancak ilgili sayfanın Initialized ve Loaded olayları henüz çalışmamıştır.

FragmentNavigation
Eğer parçalı navigasyon (Fragment Navigation) söz konusu ise bu olay tetiklenir. Bu olay ilgili kontrole doğru gidilirken çalışmaktadır.

NavigationStopped
NavigationService sınıfının static StopLoading metoduna yapılan çağrı sonucu tetiklenen olaydır. Bazı durumlarda uzun süren navigasyon işlemlerinde (örneğin bir web sayfası talebinde) kullanıcı tarafından işlemin durdurulması istenebilir. Böyle bir durumda StopLoading metoduna başvurulması halinde bu olay tetiklenmektedir. (StopLoading'i çalışma zamanında çağırmak son derece kolaydır. Nitekim WPF mimarisinde navigasyon işlemleri asenkron olarak yürütülmektedir.)

NavigationFailed
Navigasyon işlemi sırasında bir hata oluşursa tetiklenen olaydır. Bu olay içerisinde oluşan hata ile ilişkili detaylı bilgiye ulaşılabilir ve söz konusu bilgiler örneğin loglama amacı ile kayıt altına alınabilir yada kullanıcı farklı bir şekilde yönlendirilerek uygulamanın sağlıklı bir şekilde çalışması sağlanabilir.

Doğal olarak bu olayların tetiklenmesi belirli koşullara bağlıdır. Söz gelimi bir sayfaya doğru navigasyon işleminin başlatılabilmesi için Hyperlink kontrolünün NavigateUri özelliğinden yada NavigationService sınıfının Navigate metodundan yararlanılır. Navigate metodunun aşırı yüklenmiş olan versiyonları kullanılarak yukarıda bahsi geçen olaylara veri aktarımıda söz konusu olabilmektedir. Söz gelimi navigasyonun başladığı sürenin gönderilip ilgili olaylarda güncel süre ile arasındaki fark hesap edilerek işlemlerin ne kadar sürdüğü ortalama olarak tespit edilebililir. Yada navigasyon işleminin başlatıldığı sayfanın içerisinde bulunduğu NavigationWindow referansının gönderilmesi sağlanarak, Application seviyesindeki olaylarda ele alınması sağlanabilir.

Burada manuel olarak navigasyon işlemleri adına ismini sıkça duyduğumuz NavigationService sınıfının başka güçlü metodlarıda vardır. Söz gelimi navigasyon sürecinde yer alan sayfalar arasında ileri veya geri doğru gidilebilmesini sağlamak amacıyla GoBack ve GoForward metodlarından yararlanılabilir. Yanlız bu metodlar yardımıyla hareket edilirken herhangibir sebeple hedef sayfa bulunamassa çalışma zamanında InvalidOperationException alınır. Bunun önüne geçmek içinse CanGoBack ve CanGoForward özelliklerinin değerlerine bakılabilir. Bu konu ile ilişkili bir örneği denemenizi şiddetle tavsiye ederim. (Yazımızı çok fazla uzatacağından bu tarz bir örneği şu an için geliştirmeyeceğiz. Bu konuyu başka bir makalede detaylı bir şekilde incelemeye çalışcağız.)

XBAP Hakkında

Gelelim sayfalar ile ilgili bir diğer konuya. Yazımızın başında sayfa bazlı (Page-Based) uygulamaların geliştirilmesinde kullanılan modellerden bahsederken XBAP (Xaml Browser Applications) tekniğinede değinmiştik. XBAP uygulamalarında sayfalar tarayıcı pencerede (browser) gösterilmektedir. Şu an için Internet Exploere 6.0 ve üstü ile Mozilla Firefox'un son sürümünde XBAP uygulamaları çalıştırılabilmektedir. (En azından makaleyi yazdığım sıralarda bu tarayıcılar ile yapılan testlere göre...)

Varsayılan olarak XBAP uygulamalarında bazı kısıtlamalar (Restrictions) vardır. Bunlar tahmin edileceği gibi Code Access Security kurallarının uygulanmasınında bir sonucudur. Örneğin XBAP uygulamalarında istemci taraflı olaraktan dosyalara yazmak, veritabanlarına bağlanmak, registry işlemleri yapmak yada başka pencereleri tarayıcı penceresi içerisinde popup şekline açmak varsayılan olarak yasaklanmıştır. Bu tarz ihtiyaçların olması durumunda WPF uygulamasını normal bir windows uygulaması olarak yazmak ve ClickOnce gibi bir dağıtım modeli ile yaymak daha doğru bir yaklaşımdır. Yinede kısıtlama (Restriction) ayarları ile proje özellikleri üzerinden oynanarak istenirse söz konusu işlemlerin yapılması sağlanabilir.

XBAP uygulamaları varsayılan olarak istemci bilgisayardaki tarayıcı penceresine ait tampona (Cache) atılırlar. Bir başka deyişle ilk talep edildiklerinde istemci bilgisayara indirilirler (Download). Burada bir yükleme (install) işlemi söz konusu değildir. Fakat yine proje ayaları ile oynayaraktan uygulamanın istemciye install edilmesi sağlanabilir. Diğer taraftan install edilmemesinin avantajları vardır. Öyleki, uygulamadaki değişiklikler istemci tarafından otomatik olarak algılanıp son sürümün herhangibir sorgu penceresine gerek kalmadan indirilmesi ve çalıştırılması söz konusudur.

Çok doğal olarak XBAP uygulamalarının çalışabilmesi için indirildikleri bilgisayar sisteminde Microsoft.Net Framework 3.0 sürümünün yüklü olması gerekmektedir. Eğer yüklü değilse ilgili uygulama çalıştırıldığında.Net Framework 3.0 indirilmeye çalışılacaktır.

Visual Studio 2008 Beta 2 ile bir XBAP uygulaması oluşturmak son derece basittir. Tek yapılması gereken proje şablonlarından aşağıdaki resimde görüldüğü gibi WPF Browser Application öğesini seçmektir.

![mk226_17.gif](images/mk226_17.gif)

Bu işlemin sonucunda içerisinde varsayılan olarak bir sayfa (Page) içeren bir uygulama oluşturulur. Test olması amacıyla uygulamaya ikinci bir sayfa (Page) daha ekleyip bu sayfalar arasında Hyperlink ile geçişler yapmaya çalıştığımızı düşünelim. Bu amaçla Page1.xaml ve Page2.xaml içeriklerinin aşağıdaki gibi olduğunu düşünelim.

Page1.xaml;

```xml
<Page x:Class="MerhabaXBAP.Page1" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Ana Sayfa" WindowTitle="Giriş Sayfası" Background="LightGray" WindowHeight="250" WindowWidth="250" VerticalAlignment="Top" HorizontalAlignment="Left">
    <Grid>
        <Label Height="33" HorizontalAlignment="Left" Name="label1" VerticalAlignment="Top" Width="120" FontSize="14" FontWeight="Bold">
            <Hyperlink NavigateUri="Page2.xaml">Sayfa 2</Hyperlink> 
        </Label> 
    </Grid>
</Page>
```

Page2.xaml;

```xml
<Page x:Class="MerhabaXBAP.Page2" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="İkinci Sayfa" WindowTitle="Sayfa 2" Background="Gold" Loaded="Page_Loaded" VerticalAlignment="Top" HorizontalAlignment="Left" WindowHeight="250" WindowWidth="250">
    <Grid>
        <Label Height="35" Name="label1" VerticalAlignment="Top" FontSize="14" FontWeight="Bold" HorizontalAlignment="Left" Width="120"> 
            <Hyperlink NavigateUri="Page1.xaml">Sayfa 1</Hyperlink> 
        </Label>
    </Grid>
</Page>
```

Uygulamayı çalıştırdığımızda yada derleme sonucu ortaya çıkan XBAP uzantılı dosyayı örneğin Internet Explorer 7.0 üzerinde açtığımızda aşağıdaki Flash görselinde yer alan sonuçları elde ederiz.

Dikkat edileceği üzere sayfaların WindowTitle niteliklerine atanan değerler otomatik olarak Tab'da ve tarayıcı penceresinin başlık kısmında görülmektedir. Diğer taraftan burada navigasyon kontrolleri oluşturulmamış otomatik olarak tarayıcının navigasyon kontrolleri işin içerisine dahil edilmiştir. Şimdi güvenlik ile ilişkili kısıtlamaları test etmek amacıyla Page2.xaml sayfasının Loaded olayını yükleyelim ve içerisine aşağıdaki kodları yazdığımızı düşünelim.

Page2.xaml.cs;

```csharp
using System;
using System.Collections.Generic;
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
using System.IO;

namespace MerhabaXBAP
{
    public partial class Page2 : Page
    {
        public Page2()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            using (FileStream stream = new FileStream("C:\\Test.txt", FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.WriteLine(DateTime.Now.ToString() + " ek bilgi");
                }
            }
        }
    }
}
```

Bu durumda uygulamayı çalıştırıp Page2 isimli sayfaya geçmeye çalıştığımızda SecurityException tipinden bir istisna aldığımızı görürüz. Aşağıdaki ekran görüntüsünde bu durum ifade edilmektedir.

![mk226_18.gif](images/mk226_18.gif)

Daha öncedende belirttiğimiz gibi bu istisnanın sebebi varsayılan Code Access Security ayarlarıdır. Hata mesajındanda görüleceği gibi FileIOPermission yetkisi olmadığından söz konusu kodlar çalışmayıp istisna vermiştir. Ancak proje özelliklerine girip ilgili izni (Permission) aşağıdaki şekilde görüldüğü gibi vererek dosyaya yazma işleminin gerçekleştirilebilmesi sağlanabilir.

![mk226_19.gif](images/mk226_19.gif)

Bu ayarlardan sonra uygulama tekrardan test edilirse Test.txt isimli dosyanın C dizini altında oluşturulduğu ve içerisine bilgi yazıldığı görülür. Buradaki seçeneklerden bir diğeri olan This is a full trust application seçilerek, istenirse tüm kısıtlamaların ortadan kaldırılması ve istemci tarafından yürütülebilmesi sağlanabilir. Ancak bu XBAP uygulamalarının birincil amacı değildir.

Bir XBAP uygulamasını dağıtırken (Deploye) uygulamaya ait exe, manifesto ve xbap dosyalarının üçünün birden istemci uygulamaya kopyalanması yeterli bir seçenektir. Elbette söz konusu dosyalar bir intranet sisteminde ortak bir yola (Path) konularaktanda istemcilerin buradan başlatma işlemini gerçekleştirmesi sağlanabilir. Sonuçta varsayılan olarak uygulama, istemci bilgisayarın tarayıcı programının kullandığın ön bellek alanına indirilecektir. Yukarıda geliştirimiş olduğumuz XBAP örneğine baktığımızda Debug klasörü altında aşağıdaki şekilde görülen dosyaların oluşturulduğunu görürüz.

![mk226_20.gif](images/mk226_20.gif)

Burada yer alan dosyalardan exe uzantılı olan, XBAP uygulamasının derlenmiş halini içermektedir. Bir başka deyişle assembly'ın kendisidir e ILDASM, Metadata, manifesto gibi bilgileri içermektedir. Ne varki söz konusu exe tek başına çalıştırılabilen bir dosya değildir. Bu sebepten üzerinde çift tıklandığında hiç bir etkileşim olmayacaktır. Asıl çalıştırıcı dosya XBAP uzantılı olan XML dosyasıdır. Bu dosyanın içeriğinde, uygulamanın giriş noktasını işaret eden bilgi vardır. Açıkçası bu dosya çalıştırıldığında istemci bilgisayardaki varsayılan tarayıcı uygulaması devreye girecek ve exe yürütülmeye başlanacaktır. XBAP uzantılı dosyası içerisinde aynı zamanda program yazılırken üretilen dijital imzada yer almaktadır.

Bu imza özellikle güncelleme işlemlerinde önem kazanmaktadır. (Elbette istenirse uygulamanın proje özelliklerine gidilip Signing kısmında başka bir imza üretilip kullanılabilir.) Manifesto dosyasında ise, uygulamanın çalışması için gerekli olan diğer programlara ait bilgiler yer almaktadır. Söz gelimi uygulamanın çalışması için gerekli.Net Framework versiyonu, bağlı olan diğer assmebly (Class Library gibi)' lar veya uygulamadaki kodların neler yapabileceğini belirten izin (Permission) yetkileri gibi bilgiler yer almaktadır. Ne varki dağıtım işlemi özellikle ilgili uygulamalarda güncellemeler yapıldığı takdirde tekrarlanmak zorunda olabilir. Bu vakka ClickOnce ile ilgilide olduğundan ve makalemizin konusunu aştığından bu yazı dizimizde ele alınmayacaktır. Umuyorumki ilerleyen zamanlarda ele alabiliriz.

Bu makalemizde WPF (Windows Presentation Foundation) ile birlikte hayatımıza giren yeni kavramlardan birisi olan sayfa-tabanlı (Page-based) uygulamaları tanımaya çalıştık. Konunun detayları için sizlere 1000 sayfalık [Pro WPF: Windows Presentation Foundation in.NET 3.0](http://www.amazon.com/Pro-WPF-Windows-Presentation-Foundation/dp/1590597826/ref=pd_bbs_sr_1/104-7292682-3175941?ie=UTF8&s=books&qid=1191519692&sr=8-1)kitabı tavsiye ederim.

Böylece geldik uzun bir makalemizin daha sonuna. Bu cümleye kadar sabırla okuduğunu için son teşekkür ederim. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](assets/PageKullanimi.rar)
