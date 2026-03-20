---
layout: post
title: "WPF - Temel Animasyon İşlemleri"
date: 2007-09-26 12:00:00 +0300
categories:
  - wpf
tags:
  - wpf
  - xml
  - csharp
  - dotnet
  - xaml
  - http
  - threading
  - delegates
  - generics
---
Windows uygulamalarında kullanıcılara görsel bir şölen sunmak için animasyon işlemlerinden yararlanılır. Ne varki söz konusu animasyonları gerçekleştirebilmek amacıyla zorlu olan bazı süreçlerin aşılması gerekmektedir. Burada aslında, GDI+(Graphics Device Interface) alt yapısını (Infrastructure) daha etkili kullanmak adına daha çok ve daha karmaşık kodların yazılması, form üzerinde animasyonun zaman çizelgesine göre Timer bileşenleri ile Interval özelliği ve Tick olayı (event) gibi üyelerlerle uğraşılması ve hatta yeri geldiğinde ana iş parçasının (Main Thread) bozabileceği durumların önüne geçmek için özel tedbirler alınması söz konusudur. Hele birde işin içerisine üç boyutlu (3D) nesneler girdiğinde ve bunların üzerinde kullanıcı etkileşimli olayların tetiklenebilmesi de istendiğinde programıcının geliştirme süreci hem zorlaşacak hemde uzayacaktır.

Tabiki günümüzde bu tip işlemleri gerçekleştirmek için ele alına pek çok kolaylaştırıcı yazılım vardır. Ancak bir olaya.Net Windows programcısı olarak bakıyor olacağız. Durumun zorluğunu daha net anlayabilmek adına, basit olarak form üzerinde yer alan bir Button kontrolünün yükseklik (Height) ve genişlik (Width) özelliklerinin sürekli olarak belirli oranlarda büyüyüp küçüldüğünü düşünelim. Bunu Windows programlamada nasıl yapmamız gerektiğini hayal edin. Her şeyden önce sürenin söz konusu olduğu bu senaryoda bir Timer bileşenin var olması ve Tick olayının uygun şekilde ele alınması (Handle) gerekmektedir. Oysaki WPF (Windows Presentation Foundation) mimarisinde bu ve benzeri animasyon işlemleri çok daha kolay, hızlı ve güçlü bir şekilde gerçekleştirilebilmektedir. İşte bu makalemizde söz konusu durumu incemeleye başlayacak ve WPF mimarisinde temel animasyon işlemlerine değineceğiz. WPF mimarisinin söz konusu animasyon geliştirme işlerini nasıl kolaylaştırdığını görmek adına aşağıda XAML (eXtensible Application Markup Language) çıktısı aşağıda görülen örnek ile hızlı bir şekilde başlamakta yarar olacağı kanısındayım.

![mk224_1.gif](/assets/images/2007/mk224_1.gif)

```xml
<Window x:Class="AnimasyonIslemleri.Window1" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Merhaba Animasyon (DoubleAnimation)" Height="300" Width="300">
    <Grid>
        <Button Name="btnMerhaba" Width="100" Height="40" Content="Merhaba" FontSize="12" Foreground="Blue" FontWeight="Bold">
            <Button.Background>
                <LinearGradientBrush>
                    <LinearGradientBrush.GradientStops>
                        <GradientStop Color="Red" Offset="0.5"/>
                        <GradientStop Color="White" Offset="0.5"/>
                    </LinearGradientBrush.GradientStops>
                </LinearGradientBrush>
            </Button.Background>
            <Button.Triggers>
                <EventTrigger RoutedEvent="Button.MouseEnter">
                    <EventTrigger.Actions>
                        <BeginStoryboard>
                            <Storyboard>
                                <DoubleAnimation Storyboard.TargetName="btnMerhaba" Storyboard.TargetProperty="Width" By="5" RepeatBehavior="Forever" From="100" To="150" Duration="0:0:1" AutoReverse="True"/>
                                <DoubleAnimation Storyboard.TargetName="btnMerhaba" Storyboard.TargetProperty="Height" From="40" To="60" Duration="0:0:1" RepeatBehavior="Forever" AutoReverse="True"/>
                            </Storyboard>
                        </BeginStoryboard>
                    </EventTrigger.Actions>
                </EventTrigger>
            </Button.Triggers>
        </Button> 
    </Grid>
</Window>
```

XAML içeriğinde yer alan elementler ve nitelikleri (attributes) hakkında konuşmadan önce bu pencerenin çalışma zamanına bir bakalım. Aşağıdaki Flash animasyonunda çalışma zamanındaki (run-time) durum görülmektedir. Dikkat edilecek olursa pencere (Window) üzerindeki Button kontrolünün Width ve Height özelliklerinde genişlemeler olmaktadır. Dikkat çekici bir diğer noktada animasyonun, mouse ile Button kontrolü üzerine gelindiğinde başladığıdır. Bununla birlikte söz konusu aminasyon sürekli olarak devam etmektedir.

Gelelim XAML içeriğinde neler yapıldığına. Daha önceki makalelerimizden de hatırlanacağı gibi Button bileşeninin arka planında LinearGradientBrush elementi ile geçişli renklerden oluşan bir dolgu efekti kullanılmaktadır. Button elementinin içerisinde animasyon özelliklerinin belirtildiği yer Button.Triggers alt elementi ile başlamaktadır. Hemen altta yer alan EventTrigger elementinin RoutedEvent niteliğine atanan değer ile animasyonun hangi olayın tetiklenmesi sonrası başlatılacağı belirtilmektedir. Örneğimizde MouseEnter olayı bu amaçla kullanılmaktadır. Burada olay adı belirtilirken tipAdı.Özellikadı (örneğin Button.MouseEnter gibi) şeklinde olmasına dikkat edilmelidir. Aksi durumda bir istisna (Exception) alınması muhtemeldir. Dolayısıyla temel anlamda animasyonların başlatılabilmesi için olaylardan yararlanılabildiğini ve bunların ilgili kontrollere birer tetikleyici (Trigger) ile bağlandığını söyleyebiliriz.

> Bir kontrolün animasyon işlemlerinde kullanılabilmesi için IAnimatable arayüzünü uyarlamış (Implement) olması gerekmektedir. Söz gelimi Button bileşeninin sınıf diagramındaki (Class Diagram) görüntüsüne bakıldığında IAnimatable arayüzünü uyarladığı açık bir şekilde görülebilir.
> ![mk224_2.gif](/assets/images/2007/mk224_2.gif)

Animasyon işlemlerinde önemli olan noktalardan birisi kontrolün veya şeklin hangi özelliğinin değerinin, ne şekilde değiştirileceğidir. İlk örnekte Button kontrolünün Width ve Height özelliklerinin değerlerinin her ikisi birden değiştirilmektedir. Peki WPF mimarisinde hangi kontrolün hangi özelliğinin değerinin, ne şekilde değiştirileceği nasıl belirlenmektedir? İşte bu noktada devreye StoryBoard tipi ve alt elementleri girmektedir.

> StoryBoard tipi hangi kontrolün hangi özelliği üzerinde nasıl bir animasyon uygulanacağını belirleyip bunu başlatmak ve belirli bir tetikleyici (Trigger) olaya bağlamak için kullanılan önemli bir sınıftır.

Örnek XAML içeriğinde dikkat edilecek olursa StoryBoard elementi altında iki adet DoubleAnimation elementi tanımlanmıştır.

> WPF içerisinde yer alan temel animasyon tipleri (Basic Animation Types) uygun veri türleri ile çalışacak şekilde tasarlanmışlardır. Bu tiplerin adları DoubleAnimation, ColorAnimation, PointAnimation, VectorAnimation örneklerinde olduğu gibi TürAdıAnimation kelimelerinden oluşmaktadır. Animasyon tipleride IAnimatable arayüzünü uyarlamaktadır. Söz gelimi örnekte kullanılan DoubleAnimation tipinin sınıf diagramındaki (Class Diagram) görüntüsü aşağıdaki gibidir.
> ![mk224_3.gif](/assets/images/2007/mk224_3.gif)
> Ayrıca söz konusu animasyon tipleri Animatable isimli abstract sınıfdan da türemektedir. Buna göre ilgili abstract sınıfı ve arayüzü kullanarak kendi animasyon tiplerimizde yazabiliriz. WPF içerisinde temel animasyon tipleri dışında yer alan animasyon tipleride vardır. ColorAnimationUsingKeyFrames gibi. Bu tipleri ilerleyen makalelerimizde incelemeye çalışıyor olacağız.

Burada animasyon tipinin kullanımı ve özellikleri hakkında biraz konuşmakta yarar vardır. Örnekte yer alan Button kontrolünün animasyon etkisi oluşturan özellikleri Width ve Height üyeleridir. Bu özellikler Double tipinden değerler alabilmektedir. Bu nedenle StoryBoard içerisinde tanımlanan animasyon tipleri DoubleAnimation tipindendir. Dolayısıyla animasyon işleminde kullanılacak olan kontrol özelliği için, uygun olan animasyon tipinin seçilmesi gerekmektedir. DoubleAnimation tipi içerisinde oldukça önemli nitelik (attribute) tanımlamaları yer almaktadır. Bunlardan StoryBoard.TargetName niteliği ile animasyonun uygulanacağı kontrol seçilmektedir ki örnekte bu btnMerhaba isimli Button kontrolüdür. Diğer taraftan StoryBoard.TargetProperty niteliği ile kontrolün hangi özelliğinin animasyon işleminde kullanılacağı belirtilir. From niteliği ile hedef özelliğin hangi değerden başlayacağı, To niteliği ilede hedef özelliğin ilgili değerinin hangi değere kadar gelebileceği belirtilir.

Bir başka deyişle Button kontrolünün genişliği 100 değerinden 150 değerine gelecekse From niteliğine 100, To niteliğine ise 150 değerleri atanır. Burada istenirse By niteliği kullanılarak değerin artış oranıda belirtilebilir. Çok doğal olarak animasyonun ne kadar süre devam edeceği bilgisinin de belirtilmesi gerekmektedir. Bu amaçla DoubleAnimation tipinin Duration niteliği kullanılmaktadır. Duration niteliği temelde saat:dakika:saniye şeklinde bir değer almaktadır. Buna göre örnek XAML içeriğinde yer alan 0:0:1 değeri animasyondaki zaman çizgisinin (Timeline) 1 saniye olacağını belirtmektedir. RepeatBehavior niteliği ile animasyonun tekrar sayısı belirtilir. Örnekte verilen Forever değerine göre animasyon sürekli olarak devam edecektir. Son olarak AutoReverse değerine true atanarak, 1 saniyelik animasyon sona erdiğinde işlemlerin tekrar başlangıç haline tersten bir animasyon ile gitmesi sağlanmaktadır. Bu nedenle Flash animasyonunda görüldüğü gibi kontrol büyüdükten sonra aynı animasyon etkisi ile tersten küçülmüş ve ilk haline gelmiştir.

İlk örnekte söz konusu animasyon işlemleri için XAML elementlerinden ve niteliklerinden (attributes) yararlanılmıştır. Çok doğal olarak kod ile dinamik olarak aynı etkiler oluşturulabilir. Sıradaki örneğimizde kod tarafında animasyon işlemlerinin nasıl yapılabileceğini incelemeye çalışacağız. Bu amaçla penceremize (Window) ait XAML içeriğini ve kodlarını aşağıdaki gibi hazırladığımızı düşünelim.

![mk224_4.gif](/assets/images/2007/mk224_4.gif)

XAML içeriği;

```xml
<Window x:Class="AnimasyonIslemleri.KodlaMerhabaAnimasyon" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Kodla Merhaba Animasyon" Height="300" Width="300">
    <Grid>
        <Button Name="btnMerhaba" Width="100" Height="50" Foreground="Gold" Background="Black" Content="Merhaba" FontSize="14" FontWeight="Bold">
        </Button>
    </Grid>
</Window>
```

Kod içeriği;

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
using System.Windows.Shapes;
using System.Windows.Media.Animation; // Animasyon tiplerini barındıran isim alanıdır

namespace AnimasyonIslemleri
{
    public partial class KodlaMerhabaAnimasyon : Window
    {
        private void AnimasyonuHazirla()
        {
            // Button kontrolünün Width özelliği üzerinde animasyon işlemi yaptıracağımızdan ve Width özelliği Double tipinden değerler aldığından DoubleAnimation tipi kullanılmaktadır.
            // İlk parametre From
            // İkinci parametre To
            // Üçüncüd parametre animasyonun süresi
            DoubleAnimation dblAnmtr = new DoubleAnimation(100, 150, new Duration(new TimeSpan(0, 0, 0, 3)));
            // Animasyonun tekrar sayısı belirtilir.
            dblAnmtr.RepeatBehavior = new RepeatBehavior(2);

            // Yeni bir StoryBoard oluşturulur
            Storyboard strBrd = new Storyboard();
            // DoubleAnimation tipi bu storyBoard' a eklenir
            strBrd.Children.Add(dblAnmtr);
            // Hedef kontrol belirlenir
            Storyboard.SetTargetName(dblAnmtr, btnMerhaba.Name);
            // Animasyon işleminde Button kontrolünün hangi özelliğinin değiştirileceği belirtilir.
            Storyboard.SetTargetProperty(dblAnmtr,new PropertyPath(Button.WidthProperty));
            // Animasyon Begin metodu ile Button üzerine Mouse ile gelindiğinde başlatılır
            btnMerhaba.MouseEnter += delegate(object sender, MouseEventArgs e)
                                                    {    
                                                        strBrd.Begin(this);
                                                    };
        }
    
        public KodlaMerhabaAnimasyon()
        {
            InitializeComponent();
            AnimasyonuHazirla();
        }
    }
}
```

Bu kez animasyon işlemleri kod tarafında gerçekleştirilmektedir. Örnekte ilk olarak DoubleAnimation tipinden bir örnek oluşturulmuş ve yapıcı metodu içerisinde bazı ilk değerlerin (From, To, Duration gibi) verilmesi sağlanmıştır. Sonrasında ise RepatBehavior özelliği ile tekrar sayısı belirtilmektedir. Bu örnekte bir öncekinden farklı olarak Forever değeri yerine 2 kullanılmıştır. Buna göre söz konusu animasyon sadece iki kere tekrar edecektir. Sonrasında ise her zamanki gibi animasyonu, kontrol ve kontrolün özelliği ile ilişkilendirecek StoryBoard nesne örneği oluşturulur.

Animasyonun başlatılması için yine kontrolün MouseEnter olayı göz önüne alınmıştır. Animasyon işlemi için StoryBoard nesne örneğinin Begin metodunu kullanmak yeterlidir. Ancak bu metodun uygun olay için tetiklenmesini sağlamak gerekmektedir. Bu amaçlada isimsiz bir metod (anonymous method) yazılarak hem Button kontrolünün MouseEnter olayı tanımlanmış hemde gerçekleştiğinde işletilmesi istenen kodlar belirlenmiştir. Örneğin çalışma zamanındaki çıktısı aşağıdaki Flash animasyonunda olduğu gibidir.

Gelelim bir diğer örneğe. Önceki örneklerde Button kontrolonün boyutlarını ele alan animasyon tipi kullanılmıştır. Sıradaki örnekte ise ColorAnimation tipi başrolü oynamaktadır. Tahmin edileceği gibi bu tip, bir rengin başka bir renge dönüşümünün animasyon olarak gerçekleştirilmesini sağlamaktadır. Örneğin XAML içeriği aşağıdaki gibidir.

![mk224_5.gif](/assets/images/2007/mk224_5.gif)

```xml
<Window x:Class="AnimasyonIslemleri.RenkliAnimasyon" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Renkli Animasyon(ColorAnimation)" Height="300" Width="300">
    <Grid>
        <Button Name="btnMerhaba" Foreground="White" Content="Merhaba" FontSize="12" FontWeight="Bold" Width="100" Height="50">
            <Button.Background>
                <SolidColorBrush x:Name="DolguRengi" Color="Red"/>
            </Button.Background>
            <Button.Triggers>
                <EventTrigger RoutedEvent="Button.MouseEnter">
                    <EventTrigger.Actions>
                        <BeginStoryboard>
                            <Storyboard>
                                <ColorAnimation Storyboard.TargetName="DolguRengi" Storyboard.TargetProperty="(SolidColorBrush.Color)" From="Red" To="Blue" AutoReverse="True" Duration="0:0:6" RepeatBehavior="Forever"/>
                            </Storyboard>
                        </BeginStoryboard>
                    </EventTrigger.Actions>
                </EventTrigger>
            </Button.Triggers>
        </Button>
    </Grid>
</Window>
```

ColorAnimation tip olarak kullanıldığında kontrollerin renk değeri alabilen özelliklerinin animasyon işlemlerinde kullanılabileceği anlaşılmaktadır. Örnekte Button kontrolünün arka plan dolgusu animasyon içerisine katılmaktadır. Bu nedenle ColorAnimation kontrolünün ele alacağı kontrol ve özellik DoubleAnimation tipinin kullanıldığı örneklerdekinden biraz daha farklıdır. Dikkat edilecek olursa SolidColorBrush elementi içerisinde DolguRengi ismi x:Name niteliği yardımıyla tanımlanmıştır. Bu isim StoryBoard.TargetName niteliğinde kullanılmaktadır. Bir başka deyişle StroyBoard tipinin XAML içeriğindeki SolidColorBrush elementini bulması kolaylaştırılmaktadır.

Bunun en büyük nedenlerinden birisi SolidColorBrush elementinin Button elementinin altında kalmış olması ve Button tipine isim ile erişilebiliyor olmasına rağlem aynı işin söz konusu SolidColorBrush için sağlanamıyor olmasıdır. Diğer taraftan animasyonda kullanılacak özellik değeride StoryBoard.TargetProperty elementi içerisinde ele alınırken (SolidColorBrush.Color) yazım stili kullanılmaktadır. Animasyona göre, Button kontrolünün arka plan rengi kırmızından maviye doğru değişecek ve sonrasında tekrardan maviden kırmızıya dönecektir (AutoReverse=true olduğu için). Animasyondaki zaman çizelgesinin (Timeline) süresi 6 saniyedir. Aşağıdaki Flash animasyonunda uygulamanın çalışma zamanındaki (Run-time) görüntüsü yer almaktadır.

Aynı etki aşağıdaki kod parçası yardımıyla dinamik olaraktan da gerçekleştirilebilir.

XAML İçeriği;

```xml
<Window x:Class="AnimasyonIslemleri.KodlaRenkliAnimasyon" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="KodlaRenkliAnimasyon" Height="300" Width="300">
    <Grid>
        <Button Name="btnMerhaba" Width="100" Height="50" Content="Merhaba" Foreground="White" FontSize="14" FontWeight="Bold">
            <Button.Background>
                <SolidColorBrush x:Name="firca" Color="Red"/>
            </Button.Background>
        </Button>
    </Grid>
</Window>
```

Kod İçeriği;

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
using System.Windows.Shapes;
using System.Windows.Media.Animation; // Animasyon tiplerini içeren isim alanıdır(Namespace)

namespace AnimasyonIslemleri
{
    public partial class KodlaRenkliAnimasyon : Window
    {
        private void AnimasyonuAyarla()
        {
            // Animasyon tipi oluşturulur.
            // İlk parametre başlangıç rengini işaret eder (From)
            // İkinci parametre bitiş rengini işaret eder (To)
            // Üçüncü parametre zaman çizgisinin süresidir
            ColorAnimation clrAnmtr = new ColorAnimation(Colors.Red, Colors.Blue, new Duration(new TimeSpan(0, 0, 6)));
            // Animasyonun sürekli tekrar edeceği belirtilir
            clrAnmtr.RepeatBehavior = RepeatBehavior.Forever;
    
            // Animasyonu kontrol ve özelliği ile ilişkilendirecek StoryBoard nesnesi örneklenir.
            Storyboard strBrd = new Storyboard();
            strBrd.Children.Add(clrAnmtr);
            Storyboard.SetTargetName(clrAnmtr, "firca"); // SolidColorBrush kontrolünün x:name niteliğinin değeri ile animasyon uygulanacak kontrol belirtilmiş olur.
            Storyboard.SetTargetProperty(clrAnmtr, new PropertyPath(SolidColorBrush.ColorProperty)); // SolidColorBrush kontrolünün Color özelliği üzerinde animasyonun uygulanacağı belirtilir.

            // Animasyon yine MouseEnter olay metodunda Begin metodu ile başlatılır.
            btnMerhaba.MouseEnter += delegate(object sender, MouseEventArgs e)
                                                    {
                                                        strBrd.Begin(this);
                                                    };
        }
        public KodlaRenkliAnimasyon()
        {
            InitializeComponent();
            AnimasyonuAyarla();
        }
    }
}
```

Yukarıdaki kodun yer aldığı pencere çalıştırıldığında, bir önceki örnekteki ile aynı etkinin oluştuğu görülecektir.

Yazımıza PointAnimation tipini ele alacağımız son bir örnek ile devam edelim. PointAnimation tipi adındanda anlaşılacağı üzere Point tipinden değerler alabilen kontrol özellikleri (Control Property) üzerinde animasyon işlemleri gerçekleştirilebilmesi amacıyla kullanılır. Örnek olarak aşağıdaki XAML içeriğini ele alabiliriz.

![mk224_6.gif](/assets/images/2007/mk224_6.gif)

XAML içeriği;

```xml
<Window x:Class="AnimasyonIslemleri.NoktaAnimasyon" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Nokta Animasyon" Height="300" Width="300">
    <Grid>
        <Path Name="Daire" Fill="Red" Stroke="Black" StrokeThickness="1">
            <Path.Data>
                <EllipseGeometry x:Name="daireGeo" Center="15,15" RadiusX="10" RadiusY="10" />
            </Path.Data>
            <Path.Triggers>
                <EventTrigger RoutedEvent="Path.Loaded">
                    <BeginStoryboard>
                        <Storyboard>
                            <PointAnimation Storyboard.TargetName="daireGeo" Storyboard.TargetProperty="Center" From="15,15" To="285,15" Duration="0:0:1.75" AutoReverse="true" RepeatBehavior="Forever"/>
                        </Storyboard>
                    </BeginStoryboard>
                </EventTrigger>
            </Path.Triggers>
        </Path>
    </Grid>
</Window>
```

Burada örnek olarak Path tipinden bir şekil (Shape) kullanılmış ve EllipsGeometry tipinden yararlanılarak bir daire oluşturulmuştur. Burada Point tipinden değer alan üye Center özelliğidir. Bu anlamda PointAnimation tipinin hedef aldığı kontrol aslında EllipseGeometry nesnesidir. Bu işaretlemeyi yapabilmek için yine x:Name niteliğinden yararlanılmıştır. Diğer taraftan EllipsGeometry nesne örneğinin Center özelliği animasyon işlemine tabi tutulacağından StoryBoard.TargetProperty niteliğine Center değeri verilmiştir.

PointAnimation tipi basit olarak kontrolün bir zaman çizgisi (Timeline) içerisinde belirli bir koordinata doğru hareket etmesini sağlamaktadır. Buna göre örnekteki daire şekli, 15:15 noktasından 285:15 noktasına doğru 1.75 saniyelik zaman dilimi içerisinde hareket edecek ve AutoReverse özelliğine true değerinin atanması nedeniylede bu süre sonunda tekrardan geriye ilk konumuna gidecektir. Tahmin edileceği üzere RepeatBehavior niteliğinin değerinin true olması bu işlemi sürekli kılacaktır. Burada diğer örneklerden farklı olarak animasyon işleminin tetiklendiği olay olarak Path.Loaded seçilmiştir. Buna göre Path, söz konusu pencere (Window) üzerine yüklendikten sonra animasyon işlemi doğrudan başlayacaktır. Örneğin çalışma zamanındaki görüntüsü aşağıdaki Flash görselinde olduğu gibidir.

Aynı işleyişi kod tarafında gerçekleştirmek istersek aşağıdaki satırları kullanmamız yeterli olacaktır.

XAML içeriği;

```xml
<Window x:Class="AnimasyonIslemleri.KodlaNoktaAnimasyon" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Kodla Nokta Animasyon (PointAnimation)" Height="300" Width="300">
    <Grid>
        <Path Name="Daire" Fill="Red" Stroke="Black" StrokeThickness="1">
            <Path.Data>
                <EllipseGeometry x:Name="daireGeo" Center="15,15" RadiusX="10" RadiusY="10" />
            </Path.Data> 
        </Path>
    </Grid>
</Window>
```

Kod içeriği;

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
using System.Windows.Shapes;
using System.Windows.Media.Animation; // Animasyon tiplerini içeren isim alanıdır

namespace AnimasyonIslemleri
{
    public partial class KodlaNoktaAnimasyon : Window
    {
        private void AnimasyonuBaslat()
        {
            // Nokta animasyonu için gerekli tip oluşturulur
            // İlk parametre başlangıç noktası koordinatlarıdır From
            // İkinci parametre bitiş noktası koordinatlarıdır To
            // Üçündü parametre zaman çizelgesinin süresidir Duration
            PointAnimation pntAnmtr = new PointAnimation(new Point(15, 15), new Point(285, 15), new Duration(new TimeSpan(0, 0, 0,1,75)));

            // Animasyonu kontrol ve özelliği ile ilişkilendirecek olan StoryBoard oluşturulur
            Storyboard strBrd = new Storyboard();
            // Animasyon tipi StoryBoard' a eklenir
            strBrd.Children.Add(pntAnmtr);
            // Animasyonun sonundan tekrardan geriye doğru gidileceği belirtilir
            strBrd.AutoReverse = true;
            // Animasyonun sürekli devam edeceği belirtilir
            strBrd.RepeatBehavior = RepeatBehavior.Forever;
            // Animasyonun uygulanacağı EllipsGeometry tipi seçilir. Buradaki ikinci parametre XAML tarafındaki x:Name niteliğinin değeridir
            Storyboard.SetTargetName(pntAnmtr,"daireGeo");
            // Animasyonun uygulanacağı özellik seçilir.
            Storyboard.SetTargetProperty(pntAnmtr, new PropertyPath(EllipseGeometry.CenterProperty));
            // Animasyonun, Daire isimli Path yüklendikten sonra başlatılması için Loaded olay metodu yüklenir.
            Daire.Loaded += delegate(object sender, RoutedEventArgs e)
                                        {
                                            strBrd.Begin(this);
                                        };
        }
        public KodlaNoktaAnimasyon()
        {
            InitializeComponent();
            AnimasyonuBaslat();
        }
    }
}
```

WPF içerisinde kullanılan temel animasyon tipleri (Basic Animation Types) sadece yazımızda bahsetiklerimiz ile sınırlı değildir. System.Windows.Media.Animation isim alanında (Namespace) yer alan diğer animasyon tiplerinin listesi aşağıdaki gibidir.

- ByteAnimation
- DecimalAnimation
- Int16Animation
- Int32Animation
- Int64Animation
- Point3DAnimation
- QuaternionAnimation
- Rotation3DAnimation
- RectAnimation
- SingleAnimation
- SizeAnimation
- TicknessAnimation
- Vector3DAnimation
- VectorAnimation

Görüldüğü gibi kontrollerin pek çok farklı tipteki özelliği için yazılmış temel animasyon tipleri vardır. Animasyon ile ilgili işlemler bu makalede ele aldıklarımız ile sınırlı değildir elbeteki. 3 boyutlu (3D) animasyon, KeyFrame'lerin kullanımı ve dahası da var. Animasyon işlemleri ile ilgili bu ilk yazımızda temel animasyon tiplerinin tanımaya ve onları anlamaya çalıştık. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](/assets/files/2007/AnimasyonIslemleri.rar)