---
layout: post
title: "WPF - Temel Animasyon İşlemleri - 2"
date: 2007-10-01 12:00:00 +0300
description: "Bir önceki makalemizde Windows Prensetation Foundation(WPF) uygulamalarında animasyon işlemlerinin temel animasyon tipleri(Basic Animation Types) yardımıyla nasıl gerçekleştirilebileceğini incelemeye başlamıştık. Bu makalemizde animasyon işlemleri üzerindeki yönetimin biraz daha fazla olmasını sağlamak için farklı teknikleri göz önüne alıyor olacağız..."
categories:
  - wpf
tags:
  - wpf
  - xml
  - csharp
  - xaml
  - http
  - performance
  - delegates
  - generics
---
# WPF - Temel Animasyon İşlemleri - 2
Değerli Okurlarım Merhabalar,

Bir önceki makalemizde Windows Prensetation Foundation (WPF) uygulamalarında animasyon işlemlerinin temel animasyon tipleri (Basic Animation Types) yardımıyla nasıl gerçekleştirilebileceğini incelemeye başlamıştık. Bu makalemizde animasyon işlemleri üzerindeki yönetimin biraz daha fazla olmasını sağlamak için farklı teknikleri göz önüne alıyor olacağız. Her zaman olduğu gibi konuyu daha iyi kavrayabilmek adına örnekler üzerinden ilerlemekte fayda olduğu kanısındayım. Dilerseniz hiç vakit kaybetmeden ilk örneğimiz ile başalayım. Hatırlanacağı üzere, WPF uygulamalarında bileşenlerin rotasyon işlemleri için RotateTransform, ScaleTransform ve benzeri tiplerin kullanıldığını görmüştük.

Bu tip transformasyon işlemlerini animasyon tipleri ile birlikte kullanmak isteyebiliriz. Söz gelimi bir Button kontrolünün 3 saniye içerisinde kendi ekseninde 360 derece dönmesini ve bu sırada boyutlarınında 2 katına çıkarak tekrardan eski haline dönmesini istediğimizi düşünelim. Bu tip bir animasyon işleminin Buttton kontrolünün üzerine mouse ile gelindiğinde başlatılmasını ve 3 saniyelik zaman dilimi içerisinde mouse ile kontrolün terk edilmesi halinde de durmasınıda sağlayabiliriz. Burada temel animasyon tiplerinden olan DoubleAnimation oldukça işe yarayacaktır. Herşeyden önce animasyon tipinin RotateTransform ve ScaleTransform tiplerindeki uygun özellikleri kontrol edecek şekilde ayarlanması gerekir. Bu senaryoyu gerçekleştirmek için XAML içeriği aşağıda verilen bir pencere (Window) oluşturulması yeterli olacaktır.

![mk225_1.gif](/assets/images/2007/mk225_1.gif)

XAML içeriği;

```xml
<Window x:Class="AnimasyonIslemleri.RotateAnimasyon" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Rotate ve Sacle Animasyon" Height="220" Width="289">
    <Grid>
        <Button Name="btnMerhaba" Width="75" Height="40" Content="Merhaba" Background="Gold" Foreground="Black" FontSize="14" FontWeight="Bold">
            <Button.LayoutTransform>
                <TransformGroup>
                    <RotateTransform x:Name="RTrans" Angle="0"/>
                    <ScaleTransform x:Name="STrans" CenterX="0" CenterY="0" ScaleX="1" ScaleY="1"/>
                </TransformGroup>
            </Button.LayoutTransform> 
            <Button.Triggers>
                <EventTrigger RoutedEvent="Button.MouseEnter">
                    <EventTrigger.Actions>
                        <BeginStoryboard Name="strBrd">
                            <Storyboard>
                                <DoubleAnimation Storyboard.TargetName="RTrans" Storyboard.TargetProperty="Angle" To="360" Duration="0:0:3"/>
                                <DoubleAnimation Storyboard.TargetName="STrans" Storyboard.TargetProperty="ScaleX" To="2.5" Duration="0:0:1.5"/>
                                <DoubleAnimation Storyboard.TargetName="STrans" Storyboard.TargetProperty="ScaleY" To="2.5" Duration="0:0:1.5"/>
                                <DoubleAnimation Storyboard.TargetName="STrans" Storyboard.TargetProperty="ScaleX" To="1" Duration="0:0:3"/>
                                <DoubleAnimation Storyboard.TargetName="STrans" Storyboard.TargetProperty="ScaleY" To="1" Duration="0:0:3"/>
                            </Storyboard>
                        </BeginStoryboard>
                    </EventTrigger.Actions>
                </EventTrigger>
                <EventTrigger RoutedEvent="Button.MouseLeave">
                    <EventTrigger.Actions>
                        <StopStoryboard BeginStoryboardName="strBrd"/>
                    </EventTrigger.Actions>
                </EventTrigger>
            </Button.Triggers>
        </Button>
    </Grid>
</Window>
```

Burada dikkat edilmesi gereken noktalardan birisi, RotateTransform ve ScaleTransform elementlerinin DoubleAnimation tiplerinde kullanılabilmesi için x:Name nitelikleri ile isimlendirilmiş olmalarıdır. Diğer taraftan RotateTransform, Button bileşenin verilen açı kadar döndürülmesini sağlamaktadır. Buna göre ilk DoubleAnimation elementi 3 saniyelik zaman çizgisi (TimeLine) içerisinde açının değerinin 360 derece olmasını sağlamaktadır. Sonrasında gelen dört DoubleAnimation tipi ise ilk 1.5 saniyelik zaman dilimi içerisinde ScaleTransform elementinin ScaleX ve ScaleY değerlerini önce iki buçuk katına çıkarmakta sonrasında ise tekrar eski haline döndürmektedir.

Söz konusu animasyon işlemleri MouseEnter olayı tetiklendiğinde başlatılmaktadır. Diğer taraftan MouseLeave olayı tetiklendiğinde, bir başka deyişle mouse ile Button kontrolü üzerinden çıkıldığında animasyon işlemi durdurulmaktadır. Durdurma işlemi için StopStroyboard elementi kullanılır. Bu elementin hangi animasyon işlemini durduracağını belirtmek için BeginStroyboardName niteliğine (attribute) ilgili değerin atanması gerekir. Örneğimizde bu değer, BeginStroyboard elementindeki Name niteliğinin değeri olan strBrd'dir. Buna göre istersek birden fazla Storyboard'un olduğu bir senaryoda Name niteliğinden yararlanarak hangisinin durdurulacağını, duraksatılacağını (Pause) yada çıkartılacağını (Remove) belirleyebiliriz.

WPF (Windows Presentation Foundation) mimarisinde farklı Storyboard tipleri vardır.

- BeginStoryboard: Storyboard'un başlatılmasını sağlar.
- PauseStoryboard: Storyboard duraksatılır.
- ResumeStoryboard: Duraksatılan storyboard kaldığı yerden devam eder.
- RemoveStoryboard: Storyboard kaldırılır.
- SetStoryboardSpeedRatio: Storyboard içerisinde animasyonların hız oranı değiştirilebilir.
- SkipStoryboardToFill: Eğer Stroyboard içerisinde tanımlanmış bir Fill periyodu varsa animasyonun otomatik olarak buraya atlaması sağlanır.

Örneği yürüttüğümüzde çalışma zamanında (Run-time) aşağıdaki Flash görselinde yer alan etkileri izleyebiliriz. (Flash dosyasının boyutu 264 Kb olduğundan yüklenmesi zaman alabilir. Dosyanın boyutunun küçük olması için kalitesi düşürülmüştür. Bu nedenle kitap animasyon hareketi sırasında iz bırakmaktadır. Gerçek uygulamada elbetteki böyle bir iz yoktur.)

Benzer işlemleri kod tarafında yapmakta oldukça kolaydır. Bunun için aşağıdaki kodlara ve XAML içeriğine sahip pencereyi ele alabiliriz.

XAML içeriği;

```xml
<Window x:Class="AnimasyonIslemleri.KodlaRotateAnimasyon" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Kodla Rotate Animasyon" Height="250" Width="325">
    <Grid>
        <Button Name="btnMerhaba" Width="75" Height="40" Content="Merhaba" Background="Black" Foreground="Gold" FontSize="14" FontWeight="Bold">
            <Button.LayoutTransform>
                <TransformGroup>
                    <RotateTransform x:Name="RTrans" Angle="0"/>
                    <ScaleTransform x:Name="STrans" CenterX="0" CenterY="0" ScaleX="1" ScaleY="1"/>
                </TransformGroup>
            </Button.LayoutTransform>
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
using System.Windows.Media.Animation; // Animasyon tiplerinin yer aldığı isim alanıdır(Namespace)

namespace AnimasyonIslemleri
{
    public partial class KodlaRotateAnimasyon : Window
    {
        private void AnimasyonuOlustur()
        {
            // Storyboard oluşturulur
            Storyboard strBrd = new Storyboard();

            // Birinci DoubleAnimation tipi oluşturulur.
            // İlk parametre To değeridir ve burada 360 dereceyi işaret etmektedir. İkinci parametre ise Duration değeridir.
            DoubleAnimation dblAni1 = new DoubleAnimation(360, new Duration(TimeSpan.FromSeconds(3))); 
            // Animasyon tipinin RTrans adına sahip elementin AngleProperty özelliğine uygulanacağı belirtilir.
            Storyboard.SetTargetName(dblAni1, "RTrans");
            Storyboard.SetTargetProperty(dblAni1, new PropertyPath(RotateTransform.AngleProperty));
            // Animasyon tipi Storyboard' a eklenir.
            strBrd.Children.Add(dblAni1);

            // İkinci DoubleAnimation tipi oluşturulur.
            // İlk parametre To değeridir ve burada 2.5 katını işaret etmektedir. İkinci parametre ise Duration değeridir. Buna göre zaman çizgisinin ilk 1.5 saniyesi içerisinde Button bileşeninin ScaleX değeri 2.5 kat artmaktadır.
            DoubleAnimation dblAni2 = new DoubleAnimation(2.5, new Duration(TimeSpan.FromSeconds(1.5)));
            // Animayonun STrans isimli ScaleTransform elementi içerisindeki ScaleX özelliğine uygulanacağı belirtilir.
            Storyboard.SetTargetName(dblAni2, "STrans");
            Storyboard.SetTargetProperty(dblAni2, new PropertyPath(ScaleTransform.ScaleXProperty));
            // Animasyon tipi Storyboard' a eklenir.
            strBrd.Children.Add(dblAni2);

            // Üçüncü DoubleAnimation tipi oluşturulur.
            // İlk parametre To değeridir ve burada 2.5 katını işaret etmektedir. İkinci parametre ise Duration değeridir. Buna göre zaman çizgisinin ilk 1.5 saniyesi içerisinde Button bileşeninin ScaleY değeri 2.5 kat artmaktadır.
            DoubleAnimation dblAni3 = new DoubleAnimation(2.5, new Duration(TimeSpan.FromSeconds(1.5)));
            // Animayonun STrans isimli ScaleTransform elementi içerisindeki ScaleY özelliğine uygulanacağı belirtilir.
            Storyboard.SetTargetName(dblAni3, "STrans");
            Storyboard.SetTargetProperty(dblAni3, new PropertyPath(ScaleTransform.ScaleYProperty));
            // Animasyon tipi Storyboard' a eklenir.
            strBrd.Children.Add(dblAni3);

            // Zaman çizgisinin 3ncü saniyesine gelindiğinde Button kontrolünün ScaleX değeri ilk haline döner.
            DoubleAnimation dblAni4 = new DoubleAnimation(1, new Duration(TimeSpan.FromSeconds(3)));
            Storyboard.SetTargetName(dblAni4, "STrans");
            Storyboard.SetTargetProperty(dblAni4, new PropertyPath(ScaleTransform.ScaleXProperty));
            // Animasyon tipi Storyboard' a eklenir.
            strBrd.Children.Add(dblAni4);

            // Zaman çizgisinin 3ncü saniyesine gelindiğinde Button kontrolünün ScaleY değeri ilk haline döner.
            DoubleAnimation dblAni5 = new DoubleAnimation(1, new Duration(TimeSpan.FromSeconds(3)));
            Storyboard.SetTargetName(dblAni5, "STrans");
            Storyboard.SetTargetProperty(dblAni5, new PropertyPath(ScaleTransform.ScaleYProperty));
            // Animasyon tipi Storyboard' a eklenir.
            strBrd.Children.Add(dblAni5);
    
            // Mouse ile Button alanı üzerinde gelindiğinde animasyon başlatılır. 
            btnMerhaba.MouseEnter += delegate(object sender, MouseEventArgs e)
                                                    {
                                                        /* İkinci parametre Storyboard' un kontrol edilebileceğini gösterir. Bir başka deyişle programatik olarak animasyonun durdurulması, duraksatılması ve benzeri işlemlerin yapılabilmesi sağlanır. */
                                                        strBrd.Begin(this,true);
                                                    };

            btnMerhaba.MouseLeave += delegate(object sender, MouseEventArgs e)
                                                    {    
                                                        // Storyboard' un başlattığı animasyon çıkartılır. Dolayısıyla Button açısal konum ve büyüklük olarak ilk değerlerine döner.
                                                        strBrd.Stop(this); 
                                                    };
        }

        public KodlaRotateAnimasyon()
        {
            InitializeComponent();
            AnimasyonuOlustur();
        }
    }
}
```

Kod yapısında dikkat edilmesi gereken önemli noktalardan birisi, MouseLeave olayının gerçekleşmesi halinde var olan amimasyonun durdurulması (Stop) işleminin Storyboard nesne örneğine ait Stop metodu ile gerçekleştirimiş olmasıdır. Lakin burada Stop metodunun işe yarayabilmesi için animasyon Begin metodu ile başlatılırken ikinci parametrenin true olarak verilmesi şarttır. Bu parametrenin true olarak verilmesi halinde yanlızca Stop metodu değil, Pause, Resume, Remove gibi yönetsel fonksiyonelliklerinde kullanılabilir hale gelmesi sağlanmaktadır.

Animasyon işlemlerinde zaman çizgileri (Timeline) içerisinde yer alan KeyFrame'lerin yönetilmesi ve bu sayede daha güçlü hareketlerin sağlanabilmeside mümkündür. Bu amaçla WPF yapısı içerisine 3 temel KeyFrame tipi konulmuştur. Bunlar LinearDoubleKeyFrame, DiscreteDoubleKeyFrame, SplineDoubleKeyFrame tipleridir. Söz konusu tiplerin ne şekilde kullanıldıklarını örnekler ile incelediğimizde konuyu daha rahat kavrayabiliriz. Sıradaki örneğimizde LinearDoubleKeyFrame tipini kullanıyor olacağız. Bu amaçla ilk olarak aşağıdaki XAML çıktısını içeren bir pencere (Window) hazırladığımızı düşünelim.

XAML içeriği;

![mk225_2.gif](/assets/images/2007/mk225_2.gif)

```xml
<Window x:Class="AnimasyonIslemleri.MerhabaKeyFrameAnimasyon" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Merhaba Key Frame Animasyon" Height="300" Width="300">
    <Canvas>
        <Rectangle Name="Dortgen" Canvas.Top="0" Canvas.Left ="100" Height="125" Width="93">
            <Rectangle.Fill>
                <ImageBrush ImageSource="Kitap.jpg"/>
            </Rectangle.Fill>
            <Rectangle.Triggers>
                <EventTrigger RoutedEvent="Rectangle.MouseEnter">
                    <EventTrigger.Actions>
                        <BeginStoryboard>
                            <Storyboard RepeatBehavior="Forever" AutoReverse="True">
                                <DoubleAnimationUsingKeyFrames Storyboard.TargetName="Dortgen" Storyboard.TargetProperty="(Canvas.Top)">
                                    <LinearDoubleKeyFrame Value="160" KeyTime="0:0:2"/>
                                    <LinearDoubleKeyFrame Value="0" KeyTime="0:0:4"/>
                                    <LinearDoubleKeyFrame Value="100" KeyTime="0:0:6"/>
                                    <LinearDoubleKeyFrame Value="40" KeyTime="0:0:8"/>
                                    <LinearDoubleKeyFrame Value="80" KeyTime="0:0:10"/>
                                </DoubleAnimationUsingKeyFrames>
                            </Storyboard>
                        </BeginStoryboard>
                    </EventTrigger.Actions>
                </EventTrigger>
            </Rectangle.Triggers>
        </Rectangle>
    </Canvas>
</Window>
```

Dikkat edileceği üzere örnekte kullanılan LienarDoubleKeyFrame elementleri DoubleAnimationUsingKeyFrames elementi içerisinde yer almaktadır. Buna göre KeyFrame tiplerinin TipAdıUsingKeyFrames notasyonu ile tanımlanmış tipler içerisinde olması gerektiği ortadadır. Peki burada yer alan LinearDoubleKeyFrame elementleri ne işe yaramaktadır?

Value özelliği ile tahmin edileceği gibi DoubleAnimationUsingKeyFrames elementinde belirtilen kontrol ve ilgili özelliğin yeni değeri belirlenir. KeyTime niteliği ilede, özelliğin zaman çizelgesi içerisinde belirtilen anda değerini alması sağlanır. Bir başka deyişle örnekte kullanılan Dortgen isimli Rectangle elementinin pencerenin (Window) üst kısmından olan uzaklığı, zaman çizelgesi (timeline) içerisinde Frame noktalarında farklı değerlere set edilmektedir. Aslında durumu daha iyi kavramak için pencerenin çalışma zamanındaki çıktısına bakmakta fayda vardır. Aşağıdaki Flash animasyonunda bu durum işaret edilmektedir.(Flash dosyasının boyutu 322 Kb olduğundan yüklenmesi zaman alabilir. Dosyanın boyutunun küçük olması için kalitesi düşürülmüştür. Bu nedenle kitap animasyon hareketi sırasında iz bırakmaktadır. Gerçek uygulamada elbetteki böyle bir iz yoktur.)

Görüldüğü gibi dörtgenimiz zaman çizelgesinin 2nci saniyesinde pencerenin üst kenarının 160 piksel, 4ncü saniyesinde tekrardan 0 piksel, 6ncı saniyesinde 100 piksel, 8nci saniyesinde 40 piksel, 10ncu saniyesinde ise 80 piksel uzağına gelmektedir. Sonrasında ise bu animasyon hareketini tersine doğru yapmaktadır. Bunun için Storyboard elementinin AutoReverse özelliğine true değerinin atanması yeterli olmaktadır. Durumu aşağıdaki şekil ile daha net bir şekilde anlayabiliriz.

![mk225_3.gif](/assets/images/2007/mk225_3.gif)

Aynı örneği kod yardımıylada gerçekleştirebiliriz. Bunun için aşağıdaki XAML içeriğine ve kodlarına sahip bir pencere (Window) geliştirmemiz yeterli olacaktır.

XAML içeriği;

```xml
<Window x:Class="AnimasyonIslemleri.KodlaMerhabaKeyFrameAnimasyon" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Kodla Merhaba KeyFrame Animasyon" Height="241" Width="218">
    <Canvas x:Name="Bolge">
        <Rectangle Name="Dortgen" Canvas.Top="0" Canvas.Left="65" Height="65" Width="55">
            <Rectangle.Fill>
                <ImageBrush ImageSource="Kitap.jpg"/>
            </Rectangle.Fill>
        </Rectangle>
    </Canvas>
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
using System.Windows.Media.Animation;

namespace AnimasyonIslemleri
{
    public partial class KodlaMerhabaKeyFrameAnimasyon : Window
    {
        private void AnimasyonuHazirla()
        {
            // DoubleAnimationKeyFrames nesnesi örneklenir.
            DoubleAnimationUsingKeyFrames dblKeyFrames = new DoubleAnimationUsingKeyFrames();
            // Animasyon tipi, kontrol ve özelliğini ilişkilendirecek olan StoryBoard oluşturulur.
            Storyboard strBrd = new Storyboard();
            // Animasyon sona erdiğinde aynı rotadan geriye doğru dönmesi için AutoReverse özelliğine true değeri atanır
            strBrd.AutoReverse = true;
            // Animasyonun sürekli olması için RepeatBehavior özelliğine Forever değeri atanır.
            strBrd.RepeatBehavior = RepeatBehavior.Forever;
            // Hedef kontrol olarak Rectangle tipinden olan Dortgen seçilir
            Storyboard.SetTargetName(dblKeyFrames, "Dortgen");
            // Animasyon değerlerinin uygulanacağı özellikl olarak Canvas' ın Top özelliği seçilir.
            Storyboard.SetTargetProperty(dblKeyFrames, new PropertyPath(Canvas.TopProperty));
            // KeyFrame değerleri belirlenir.
            // İlk 1 saniyede Canvas dikeylemesine 120 pikselinci uzaklığa gelecektir
            dblKeyFrames.KeyFrames.Add(new LinearDoubleKeyFrame(120, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 1))));
            // 2nci saniyede Canvas dikeylemesine 0 pikselinci uzaklığa gelecektir
            dblKeyFrames.KeyFrames.Add(new LinearDoubleKeyFrame(0, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 2))));
            // 3ncü saniyede Canvas dikeylemesine 100 pikselinci uzaklığa gelecektir
            dblKeyFrames.KeyFrames.Add(new LinearDoubleKeyFrame(100, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 3))));
            // 4ncü saniyede Canvas dikeylemesine 50 pikselinci uzaklığa gelecektir
            dblKeyFrames.KeyFrames.Add(new LinearDoubleKeyFrame(50, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 4))));
            // 5nci saniyede Canvas dikeylemesine 75 pikselinci uzaklığa gelecektir.
            dblKeyFrames.KeyFrames.Add(new LinearDoubleKeyFrame(75, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 5))));
            // 6nci saniyede Canvas dikeylemesine 25 pikselinci uzaklığa gelecektir.
            dblKeyFrames.KeyFrames.Add(new LinearDoubleKeyFrame(25, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 6))));
            // Animasyon tipi StoryBoard nesnesine eklenir
            strBrd.Children.Add(dblKeyFrames);
            // Animasyonun, Mouse ile Dortgen in üzerine gelindiğinde başlaması sağlanır.
            Dortgen.MouseEnter += delegate(object sender, MouseEventArgs e)
                                                {
                                                    strBrd.Begin(this);
                                                };
        }
        public KodlaMerhabaKeyFrameAnimasyon()
        {
            InitializeComponent();
            AnimasyonuHazirla();
        }
    }
}
```

Kod tarafında dikkat edilmesi gereken noktalardan birisi LinearDoubleKeyFrame nesne örneklerinin DoubleAnimationUsingKeyFrames tipinin KeyFrames koleksiyonunda tutuluyor olmasıdır. KeyFrames özelliği üzerinden çağırılan Add metodu ile ilgili nesnelerin koleksiyona eklenmesi sağlanır. Oldukça eğlenceli değil mi? Gelin LinearDoubleKeyKeyFrame nesnelerinin kullanıldığı başka bir örnek ile devam edelim. Bu yeni örnekte bir öncekinden farklı olarak Canvas'ın Top ve Left özelliklerini rastegele oranlarda değiştiriyor olacağız. İşte örnek XAML içeriği ve kodlarımız.

![mk225_4.gif](/assets/images/2007/mk225_4.gif)

XAML içeriği;

```xml
<Window x:Class="AnimasyonIslemleri.KodlaKeyFrameAnimasyon2" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="KodlaKeyFrameAnimasyon2" Height="400" Width="400" Background="LightBlue">
    <Canvas x:Name="alan">
        <Rectangle Name="dortgenim" Canvas.Top="0" Canvas.Left="0" Height="65" Width="55">
            <Rectangle.Fill>
                <ImageBrush ImageSource="Kitap.jpg"/>
            </Rectangle.Fill>
        </Rectangle>
    </Canvas>
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
using System.Windows.Media.Animation;

namespace AnimasyonIslemleri
{
    public partial class KodlaKeyFrameAnimasyon2 : Window
    {
        Random rnd = new Random();
        Storyboard strBrd = new Storyboard();

        public void AnimasyonuYurut()
        {
            double randomT = rnd.Next(1, 250);
            double randomL = rnd.Next(1, 250);
            Title=String.Format("İlk nokta {0}:{1} İkinci Nokta {2}:{3}",randomT.ToString(),randomL.ToString(),(randomT - 25).ToString(),(randomL - 25).ToString());

            strBrd.AutoReverse = false;
            strBrd.RepeatBehavior = new RepeatBehavior(1);

            DoubleAnimationUsingKeyFrames keyFrm1 = new DoubleAnimationUsingKeyFrames();
            keyFrm1.KeyFrames.Add(new LinearDoubleKeyFrame(randomT, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 3))));
            keyFrm1.KeyFrames.Add(new LinearDoubleKeyFrame(randomT-25, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 5))));

            Storyboard.SetTargetName(keyFrm1, "dortgenim");
            Storyboard.SetTargetProperty(keyFrm1, new PropertyPath(Canvas.TopProperty));
            strBrd.Children.Add(keyFrm1);
    
            DoubleAnimationUsingKeyFrames keyFrm2 = new DoubleAnimationUsingKeyFrames();
            keyFrm2.KeyFrames.Add(new LinearDoubleKeyFrame(randomL, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 3))));
            keyFrm2.KeyFrames.Add(new LinearDoubleKeyFrame(randomL-25, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 5))));
        
            Storyboard.SetTargetName(keyFrm2, "dortgenim");
            Storyboard.SetTargetProperty(keyFrm2, new PropertyPath(Canvas.LeftProperty));
            strBrd.Children.Add(keyFrm2);
        
            strBrd.Begin(this);
        }
    
        void dortgenim_MouseEnter(object sender, MouseEventArgs e)
        {
            AnimasyonuYurut();
        }

        public KodlaKeyFrameAnimasyon2()
        {
            InitializeComponent();
            dortgenim.MouseEnter += new MouseEventHandler(dortgenim_MouseEnter);
        }
    }
}
```

Tahmin edileceği üzere dortgenin Top ve Left özelliklerini animayon içerisinde kullanmak istediğimizden iki farklı DoubleAnimationUsingKeyFrames nesne örneği kullanılması gerekmektedir. Bu nesne örnekleride kendi içlerinde farklı LinearDoubleKeyFrame nesnelerine sahiptir. Örneğin çalışma zamanındaki (run-time) çıktısı aşağıdaki Flash görselindeki gibi olacaktır. (Flash dosyasının boyutu 454 Kb olduğundan yüklenmesi zaman alabilir. Dosyanın boyutunun küçük olması için kalitesi düşürülmüştür. Bu nedenle kitap animasyon hareketi sırasında iz bırakmaktadır. Gerçek uygulamada elbetteki böyle bir iz yoktur.)

LinearDoubleKeyFrame tipi söz konusu değerlere ulaşılırken akıcı bir görselliğin olmasını sağlar. Bunun dışında DiscreteDoubleKeyFrame tipi kullanılaraktan nesnenin belirtilen animasyonda bir sonraki değerine sıçrayarak geçmesi sağlanabilir. Aslında bu durumu anlamanın en iyi yolu örnek bir senaryo üzerinde ilerlemektir. Bu amaçla aşağıdaki XAML çıktısına sahip bir pencere (Window) tasarladığımızı düşünelim.

![mk225_5.gif](/assets/images/2007/mk225_5.gif)

XAML içeriği;

```xml
<Window x:Class="AnimasyonIslemleri.DiscreteKullanimi" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="DiscreteDoubleKeyFrame ile Animasyon" Height="400" Width="400">
    <Canvas>
        <Rectangle Name="Dortgen" Canvas.Top="0" Canvas.Left="0" Height="75" Width="55">
            <Rectangle.Fill>
                <ImageBrush ImageSource="Kitap.jpg"/>
            </Rectangle.Fill>
            <Rectangle.Triggers>
                <EventTrigger RoutedEvent="Window.Loaded">
                    <EventTrigger.Actions>
                        <BeginStoryboard>
                            <Storyboard AutoReverse="True" RepeatBehavior="Forever">
                                <DoubleAnimationUsingKeyFrames Storyboard.TargetName="Dortgen" Storyboard.TargetProperty="(Canvas.Top)">
                                    <DiscreteDoubleKeyFrame KeyTime="0:0:1" Value="150"/> 
                                    <DiscreteDoubleKeyFrame KeyTime="0:0:2" Value="30"/>
                                    <DiscreteDoubleKeyFrame KeyTime="0:0:3" Value="120"/>
                                    <DiscreteDoubleKeyFrame KeyTime="0:0:4" Value="60"/>
                                    <DiscreteDoubleKeyFrame KeyTime="0:0:5" Value="90"/>
                                    <DiscreteDoubleKeyFrame KeyTime="0:0:6" Value="75"/>
                                </DoubleAnimationUsingKeyFrames>
                                <DoubleAnimationUsingKeyFrames Storyboard.TargetName="Dortgen" Storyboard.TargetProperty="(Canvas.Left)">
                                    <DiscreteDoubleKeyFrame KeyTime="0:0:1.5" Value="150"/>
                                    <DiscreteDoubleKeyFrame KeyTime="0:0:2.5" Value="30"/>
                                    <DiscreteDoubleKeyFrame KeyTime="0:0:3.5" Value="120"/>
                                    <DiscreteDoubleKeyFrame KeyTime="0:0:4.5" Value="60"/>
                                    <DiscreteDoubleKeyFrame KeyTime="0:0:5.5" Value="90"/>
                                    <DiscreteDoubleKeyFrame KeyTime="0:0:6.5" Value="75"/>
                                </DoubleAnimationUsingKeyFrames>
                            </Storyboard>
                        </BeginStoryboard>
                    </EventTrigger.Actions>
                </EventTrigger>
            </Rectangle.Triggers>
        </Rectangle>
    </Canvas>
</Window>
```

Örnekte iki adet DoubleAnimationUsingKeyFrames elementi kullanılmıştır. Bunlardan birisi Canvas tipinin Top özelliği üzerinde diğeri ise Left özelliği üzerinden animasyon işlemlerinin yürütülmesini olanaklı kılmaktadır. Her biri kendi içerisinde birden fazla DiscreteDoubleKeyFrame tipi içermektedir. DiscreteDoubleKeyFrame elementleri KeyTime niteliğinde belirtilen anda, animasyon uygulanan kontrolün ilgili özelliğinin value niteliği ile belirtilen değerde olmasını sağlamaktadır.

Söz gelimi animasyondaki zaman çizelgesinin 2nci saniyesinde Top değeri 30 piksel, 1.5nci saniyesinde Left değeri 150 piksel olacak şekilde belirlenmektedir. Daha öncede belirtildiği gibi kontrolün söz konusu koordinat noktalarına ulaşması sırasında akıcı bir efekt yerine sıçrama (Jump) efekti uygulanmaktadır. Her halde bir yerden bir yere ışınlanmanın WPF'cesinin bu olduğunu söyleyebiliriz. Bu durumu aşağıdaki Flash görselinden daha net bir şekilde analiz edebiliriz.

Elbetteki aynı örneği kod yardımıylada gerçekleştirebiliriz. Bunun için aşağıdaki XAML içeriğine ve kodlara sahip bir pencere hazırlamamız yeterli olacaktır.

XAML içeriği;

```xml
<Window x:Class="AnimasyonIslemleri.KodlaDiscreteKullanimi" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Kod ile Discrete Kullanimi" Height="300" Width="300">
    <Canvas>
        <Rectangle Name="Dortgen" Canvas.Top="0" Canvas.Left="0" Height="75" Width="55">
            <Rectangle.Fill>
                <ImageBrush ImageSource="Kitap.jpg"/>
            </Rectangle.Fill>
        </Rectangle>
    </Canvas>
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
    public partial class KodlaDiscreteKullanimi : Window
    {
        private void AnimasyonuHazirla()
        {
            // Storyboard nesnesi örneklenir
            Storyboard strBrd = new Storyboard();
            // Animasyonun sürekli tekrar edeceği belirtilir
            strBrd.RepeatBehavior = RepeatBehavior.Forever;
            // Animasyonun zaman çizgisini tamamladığında aynı yoldan geriye dönmesi sağlanır
            strBrd.AutoReverse = true;

            // Dortgen nesnesinde Canvas' ın Top özelliği üzerinde Frame bazlı animasyon yapılmasını sağlayacak şekilde DoubleAnimationUsingKeyFrames nesnesi örneklenir
            DoubleAnimationUsingKeyFrames dblA1 = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetName(dblA1, "Dortgen");
            Storyboard.SetTargetProperty(dblA1, new PropertyPath(Canvas.TopProperty));
    
            // Frame' lerdeki Top özelliğinin değerleri belirtilir. Bunun için DiscreteDoubleKeyFrame nesneleri örneklenerek KeyFrames koleksiyonuna eklenir.
            dblA1.KeyFrames.Add(new DiscreteDoubleKeyFrame(150, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 1))));
            dblA1.KeyFrames.Add(new DiscreteDoubleKeyFrame(30, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 2))));
            dblA1.KeyFrames.Add(new DiscreteDoubleKeyFrame(120, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 3))));
            dblA1.KeyFrames.Add(new DiscreteDoubleKeyFrame(60, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 4))));
            dblA1.KeyFrames.Add(new DiscreteDoubleKeyFrame(90, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 5))));
            dblA1.KeyFrames.Add(new DiscreteDoubleKeyFrame(75, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 6))));
            // DoubleAnimationUsingKeyFrames nesne örneği Storyboard' a alt element olarak eklenir.
            strBrd.Children.Add(dblA1);
    
            DoubleAnimationUsingKeyFrames dblA2 = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetName(dblA2, "Dortgen");
            Storyboard.SetTargetProperty(dblA2, new PropertyPath(Canvas.LeftProperty));
            
            dblA2.KeyFrames.Add(new DiscreteDoubleKeyFrame(150, KeyTime.FromTimeSpan(new TimeSpan(0,0, 0, 1,500))));
            dblA2.KeyFrames.Add(new DiscreteDoubleKeyFrame(30, KeyTime.FromTimeSpan(new TimeSpan(0, 0,0, 2,500))));
            dblA2.KeyFrames.Add(new DiscreteDoubleKeyFrame(120, KeyTime.FromTimeSpan(new TimeSpan(0, 0,0, 3,500))));
            dblA2.KeyFrames.Add(new DiscreteDoubleKeyFrame(60, KeyTime.FromTimeSpan(new TimeSpan(0, 0,0, 4,500))));
            dblA2.KeyFrames.Add(new DiscreteDoubleKeyFrame(90, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0,5,500))));
            dblA2.KeyFrames.Add(new DiscreteDoubleKeyFrame(75, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0,6,500))));
            strBrd.Children.Add(dblA2);
    
            // Pencere yüklendikten sonra
            Loaded += delegate(object sender, RoutedEventArgs e)
                            {
                                // Animasyon başlatılır
                                strBrd.Begin(this);
                            };
        
        }
        public KodlaDiscreteKullanimi()
        {
            InitializeComponent();
            AnimasyonuHazirla();
        }
    }
}
```

Frame tabanlı animasyon işlemlerinde LinearDoubleKeyFrame ve DiscreteDoubleKeyFrame tipleri dışında kullanılabilen bir diğer sınıfta SplineDoubleKeyFrame'dir. Bu tip sayesinde, animasyon işlemi sırasındaki ileri ve geri yönlü hareketlerde, hızlanma ve yavaşlama oranları daha net bir şekilde belirlenebilir. Söz konusu hızlanma verilerini belirlemek için, sadece 0.0 ile 1.0 arasında değerler alabilen KeySpline özelliğinden yararlanılır. Aşağıdaki XAML içeriğine sahip örnekte SplienDoubleKeyFrame tipi kullanılmaktadır.

![mk225_6.gif](/assets/images/2007/mk225_6.gif)

XAML içeriği;

```xml
<Window x:Class="AnimasyonIslemleri.SplineKullanimi" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Spline Kullanimi" Height="422" Width="181">
    <Canvas>
        <Rectangle Name="Dortgen" Canvas.Top="0" Canvas.Left="0" Height="75" Width="55">
            <Rectangle.Fill>
                <ImageBrush ImageSource="Kitap.jpg"/>
            </Rectangle.Fill>
            <Rectangle.Triggers>
                <EventTrigger RoutedEvent="Window.Loaded">
                    <EventTrigger.Actions>
                        <BeginStoryboard>
                            <Storyboard AutoReverse="True" RepeatBehavior="Forever">
                                <DoubleAnimationUsingKeyFrames Storyboard.TargetName="Dortgen" Storyboard.TargetProperty="(Canvas.Top)">
                                    <SplineDoubleKeyFrame KeyTime="0:0:3" Value="200" KeySpline="0.3,0.0 0.9,0.3"/>
                                </DoubleAnimationUsingKeyFrames>
                            </Storyboard>
                        </BeginStoryboard>
                    </EventTrigger.Actions>
                </EventTrigger>
            </Rectangle.Triggers>
        </Rectangle>
    </Canvas>
</Window>
```

Bu örneğin çalışma zamanındaki çıktısı aşağıdaki Flash animasyonunda olduğu gibidir.(Flash dosyasının boyutu 298 Kb olduğundan yüklenmesi zaman alabilir.)

Dikkat edilecek olursa düşey düzlemde hareket eden resim 3 saniyelik zaman dilimi içerisinde pencerenin üst noktasından 200 piksel aşağıya gelmektedir. Ancak bu hareketi sırasında KeySpline niteliğinde belirtilen değerler nedeni ile düşey olarak hızlanarak ve ters yönde de yavaşlayarak ilerlemektedir. KeySpline niteliğindeki değerler ile oynayarak daha farklı etkiler de elde edilebilir. Hatta bu değerler ile oynayarak farklı etkilerin nasıl oluşturulabileceğini incelemenizi şiddetle tavsiye ederim. Aynı örnek kod yardımıylada aşağıdaki gibi geliştirilebilir.

XAML içeriği;

```xml
<Window x:Class="AnimasyonIslemleri.KodlaSplineKullanimi" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Kod ile Spline Kullanimi" Height="397" Width="181">
    <Canvas>
        <Rectangle Name="Dortgen" Canvas.Top="0" Canvas.Left="0" Height="75" Width="55">
            <Rectangle.Fill>
                <ImageBrush ImageSource="Kitap.jpg"/>
            </Rectangle.Fill>
        </Rectangle>
    </Canvas>
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
    public partial class KodlaSplineKullanimi : Window
    {
        private void AnimasyonuHazirla()
        {
            // Storyboard nesnesi örneklenir, animasyonun tekrarlı ve geriye dönüşlü olacak şekilde çalışacağı belirlenir
            Storyboard strBrd = new Storyboard();
            strBrd.RepeatBehavior = RepeatBehavior.Forever;
            strBrd.AutoReverse = true;

            DoubleAnimationUsingKeyFrames dblA = new DoubleAnimationUsingKeyFrames();
    
            // SplineDoubleKeyFrame nesnesi eklenir. Üçüncü parametre ile hızlanma ve yavaşlama değerleri belirlenir.
            dblA.KeyFrames.Add(new SplineDoubleKeyFrame(200, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 3)), new KeySpline(0.3, 0, 0.9, 0.3)));
            // Animasyonun Dortgen isimli nesneye ve içinde bulunduğu Canvas' ın Top özelliğine uygulanacağı belirlenir
            Storyboard.SetTargetName(dblA, "Dortgen");
            Storyboard.SetTargetProperty(dblA, new PropertyPath(Canvas.TopProperty));
    
            // Animasyon nesnesi Storyboard' a alt element olarak eklenir
            strBrd.Children.Add(dblA);
        
            // Pencerenin yüklenmesi tamamlandıktan sonra
            Loaded += delegate(object sender, RoutedEventArgs e)
                            {
                                // Animasyon başlatılır
                                strBrd.Begin(this);
                            };
        }
        public KodlaSplineKullanimi()
        {    
            InitializeComponent();
            AnimasyonuHazirla();
        }
    }
}
```

Bu örneklerdende anlaşılacağı üzere farklı veri tipleri için yazılmış olan Frame bazlı animasyon tipleri olduğunu söyleyebiliriz. Geliştirilen örnekler double tipi ile çalışan özellikler üzerinde Frame bazlı animasyonlar geliştirilebilmesi için DoubleAnimationUsingKeyFrames tipini kullanmaktadır. Ancak bu tip dışında ColorAnimationUsingKeyFrames, PointAnimationUsingKeyFrames, ByteAnimationUsingKeyFrames vb... sınıflarda yer almaktadır.

Tüm bu Frame bazlı animasyon tipleri LinearTipAdıKeyFrame, DiscreteTipAdıKeyFrame ve SplineTipAdıKeyFrame sınıflarına ait nesne örnekleri ile çalışabilmektedir. Tabiki bu durum her veri türü için geçerli değildir. Söz gelimi StringAnimationKeyFrames tipi sadece DiscreteStringKeyFrame sınıfına ait nesne örneklerini taşıyabilir. Dolayısıyla enterpoasyon (Interpolation) efektleri gerçekleştirilemez.

Sıradaki örnekte bir Button kontrolünün Text özelliğinin içeriğini Frame bazlı animasyon işleminde nasıl ele alabileceğimizi inceleyeceğiz. Bu amaçla XAML içeriği aşağıdaki gibi olan bir pencere tasarladığımızı düşünelim.

![mk225_7.gif](/assets/images/2007/mk225_7.gif)

XAML içeriği;

```xml
<Window x:Class="AnimasyonIslemleri.StringKeyFrameAnimasyon" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="StringKeyFrameAnimasyon" Height="149" Width="278">
    <Grid>
        <Button Name="btnMerhaba" Width="140" Height="40" FontSize="12" FontWeight="Bold">
            <Button.Triggers>
                <EventTrigger RoutedEvent="Button.MouseEnter">
                    <EventTrigger.Actions>
                        <BeginStoryboard>
                            <Storyboard RepeatBehavior="Forever">
                                <StringAnimationUsingKeyFrames Storyboard.TargetName="btnMerhaba" Storyboard.TargetProperty="Content" Duration="0:0:6">
                                <!-- Eğer Duration süresi belirtilmesse DiscreteStringKeyFrame elementlerinden sonuncusu gösterilmiyor. Sonuncusununda gösterilmesi için Duration süresi son DiscreteStringKeyFrame' inkinden fazla verilmelidir.-->
                                    <DiscreteStringKeyFrame Value="WPF" KeyTime="0:0:1"/>
                                    <DiscreteStringKeyFrame Value="ile" KeyTime="0:0:2"/>
                                    <DiscreteStringKeyFrame Value="Temel" KeyTime="0:0:3"/>
                                    <DiscreteStringKeyFrame Value="Animasyon" KeyTime="0:0:4"/>
                                    <DiscreteStringKeyFrame Value="İşlemleri" KeyTime="0:0:5"/>
                                </StringAnimationUsingKeyFrames>
                            </Storyboard>
                        </BeginStoryboard>
                    </EventTrigger.Actions>
                </EventTrigger>
            </Button.Triggers>
        </Button>
    </Grid>
</Window>
```

Bu örneğin çalışma zamanındaki işleyişi aşağıdaki Flash görselinde olduğu gibidir. DiscreteStringKeyFrame elementlerine ait Value nitelikleri tahmin edileceği üzere, animasyonun bağlandığı kontrolün ilgili özelliğindeki string bilginin t anında ne olacağını belirtmekte kullanılmaktadır. KeyTime niteliğine verilen değerler ile söz konusu t anları belirtilir.

Elbetteki bu örnekteki animasyon etkisi kod yardımıylada geliştirilebilir. Aşağıdaki kod parçalarında bu duruma bir örnek verilmektedir.

XAML içeriği;

```xml
<Window x:Class="AnimasyonIslemleri.KodlaStringKeyFrameAnimasyon" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="KodlaStringKeyFrameAnimasyon" Height="144" Width="300">
    <Grid>
        <Button Name="btnMerhaba" Width="140" Height="40" FontSize="12" FontWeight="Bold"/>
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
using System.Windows.Media.Animation;

namespace AnimasyonIslemleri
{
    public partial class KodlaStringKeyFrameAnimasyon : Window
    {
        string slogan = "CSharp az keyword ile çok iş yapmamızı sağlayan bir dildir";

        private void AnimasyonuHazirla()
        {
            Storyboard strBrd = new Storyboard();
            strBrd.RepeatBehavior = RepeatBehavior.Forever;
    
            StringAnimationUsingKeyFrames strA = new StringAnimationUsingKeyFrames();
            string[] kelimeler = slogan.Split(' ');
            strBrd.Duration = TimeSpan.FromSeconds(kelimeler.Length + 1);
            for(int i=1;i<=kelimeler.Length;i++)
            {
                strA.KeyFrames.Add(new DiscreteStringKeyFrame(kelimeler[i-1],KeyTime.FromTimeSpan(TimeSpan.FromSeconds(i))));
            }
            Storyboard.SetTargetName(strA, "btnMerhaba");
            Storyboard.SetTargetProperty(strA, new PropertyPath(Button.ContentProperty));
            strBrd.Children.Add(strA);
            btnMerhaba.MouseEnter += delegate(object sender, MouseEventArgs e)
                                                    {
                                                        strBrd.Begin(this);
                                                    };
        }
        public KodlaStringKeyFrameAnimasyon()
        {
            InitializeComponent();
            AnimasyonuHazirla();
        }
    }
}
```

Bu kez, bir slogan cümle içerisindeki kelimeler boşluk karakterine göre Split metodu ile ayrıştırılmakta ve elde edilen string dizisindeki her bir kelime için DiscreteStringKeyFrame temelli, saniyede bir artan animasyon uygulanmaktadır. Bu örnek kod parçasının çalışma zamanındaki çıktısı ise aşağıdaki Flash görselindeki gibi olacaktır.

Makalemizde son olarak bir bileşenin belirli bir rota üzerinde hareket etmesinin animasyon teknikleri ile nasıl gerçekleştirilebileceğini incelemeye çalışacağız. Önceki makalelerimizden hatırlayacağınız gibi, Path ve PathGeometry tiplerini kullanarak birbirlerine bağlı yayların çizilmesi mümkündür. Bu tip bir elementin oluşturacağı rotayı takip eden bir bileşen söz konusu olduğunda, animasyon tiplerinden TipAdıAnimationUsingPath isimlendirmesi ile tanımlananlardan yararlanbiliriz. Örneğin bir PathGeometry ile tanımlanan rotada hareket edecek bir nesnenin içinde bulunduğu Canvas'ın Top ve Left özellikleri söz konusu ise DoubleAnimationUsingPath tipini kullanabiliriz. Aşağıdaki XAML çıktısında bu durum örneklenmeye çalışılmaktadır.

XAML içeriği;

```xml
<Window x:Class="AnimasyonIslemleri.PathFrameKullanimi" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="PathFrameKullanimi" Height="300" Width="300">
    <Window.Resources>
        <PathGeometry x:Key="YolGeometrisi">
            <PathFigure>
                <BezierSegment Point1="0,0" Point2="10,120" Point3="70,40"/>
                <ArcSegment Point="150,125" SweepDirection="Clockwise" Size="50,50" IsLargeArc="True" RotationAngle="60"/>
                <ArcSegment Point="200,200" SweepDirection="Counterclockwise" Size="50,50" IsLargeArc="True" RotationAngle="60"/>
            </PathFigure>
        </PathGeometry>
    </Window.Resources>
    <Canvas>
        <Path Data="{StaticResource YolGeometrisi}" Stroke="LightGray" StrokeThickness="2"/>
            <Rectangle Height="65" Name="Dortgen" Width="55">
                <Rectangle.Fill>
                    <ImageBrush ImageSource="Kitap.jpg" />
                </Rectangle.Fill>
                <Rectangle.Triggers>
                    <EventTrigger RoutedEvent="Rectangle.MouseEnter">
                        <BeginStoryboard>
                            <Storyboard AutoReverse="True" RepeatBehavior="Forever">
                                <DoubleAnimationUsingPath Storyboard.TargetName="Dortgen" Storyboard.TargetProperty="(Canvas.Top)" PathGeometry="{StaticResource YolGeometrisi}" Source="Y" Duration="0:0:6" />
                                <DoubleAnimationUsingPath Storyboard.TargetName="Dortgen" Storyboard.TargetProperty="(Canvas.Left)" PathGeometry="{StaticResource YolGeometrisi}" Source="X" Duration="0:0:6" />
                            </Storyboard>
                        </BeginStoryboard>
                    </EventTrigger>
                </Rectangle.Triggers>
            </Rectangle>
        </Canvas>
</Window>
```

Söz konusu örnekte, Storyboard elementi içerisinde DoubleAnimationUsingPath alt elementi kullanılmaktadır. Bu elementlerden iki adet tanımlanmasının sebebi, Dortgen isimli nesnenin Top ve Left özelliklerinin ayrı ayrı değiştirilmesi gerekliliğidir. DoubleAnimationUsingPath tipinde yer alan niteliklerden Storyboard.TargetName ile animasyonun hangi kontrole uygulanacağı belirtilir ki bu örnekte söz konusu kontrol Dortgen isimli Rectangle dır. Storyboard.TargetProperty nitelikleri ise Rectangle'ın içerisinde bulunduğu Canvas'ın Top ve Left özelliklerinin değerleri üzerinde hareketlendirmeler yapılacağını belirtir.

PathGeometry nitelikleri tahmin edileceği üzere rota bilgisini içeren pencere kaynağını (Windows Resource) işaret etmektedir. MSDN kaynakları Resource olarak yapılan bu tip tanımlamaların animasyon işlemlerinde performans yönünden olumlu katkı yapacağını belirtmektedir. Static kaynak olarak tanımlanan YolGeometrisi isimli veri kümesi yardımıyla Path elementinin rota bilgiside verilmektedir. Bunun içinde Path elementinin Data niteliğine değer ataması yapılmıştır. Bu örneğin çalışma zamanındaki çıktısı aşağıdaki Flash animasyonunda olduğu gibidir. (Flash dosyasının boyutu 352 kb olduğundan yüklenmesi zaman alabilir.)

Görüldüğü gibi WPF mimarisinde gerek XAML tarafında gerekse kod tarafında animasyon işlemleri için son derece etkili yollar kullanılabilmektedir. Bence burada önemli olan noktalardan biriside işaretleme dili içerisinde bu tip gelişmiş fonksiyonelliklerin geliştirilebilmesidir. İlerleyen makalelerimizde WPF ile ilgili farklı konularada değinmeye çalışacağız. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](/assets/files/2007/AnimasyonIslemleri.rar)