---
title: "WPF - Grafik İşlemlerinde Şekillerin(Shapes) Kullanımı"
pubDate: 2007-09-18 09:00:00
categories:
  - WPF
tags:
  - WPF
  - XML
  - C#
  - LINQ
  - XAML
  - HTTP
  - Generics
  - Visual Studio
---

# WPF - Grafik İşlemlerinde Şekillerin(Shapes) Kullanımı
Değerli Okurlarım Merhabalar,

Windows Presentation Foundation (WPF) ile ilgili bir önceki makalemizde, iki boyutlu (2D) grafiklerin çizilmesi amacıyla kullanılan fırçaları (Brushes) incelemeye çalışmıştık. Bu makalemizde ise iki boyutlu şekilleri (Shapes) araştırıyor olacağız. Vektörel grafiklerde şekillerin (Shapes) büyük önemi vardır. Nitekim temel şekiller kullanılarak asıl resimler ve görüntüler kolaylıkla elde edilebilir. Bir CAD uygulamasının karmaşık çizelgelerinden, eğlenceli çocuk programlarında kullanılan vektörel grafiklere kadar pek çok alanda temel şekiller yeterli olmaktadır. Söz gelimi bir şehrin imar planlamasında kullanılacak bir programda iki boyutlu olarak düşünüldüğünde dörtgenler, daireler, elipsler, poligonlar ve düz çizgiler evlerin, yolların, arsaların, parkların ifade edilmesi için yeterlidir. Senaryolar arttırılabilir ve daha geniş alanlarda düşünülebilir. Ancak temel olarak gereken şekiller bellidir. WPF kendi bünyesinde iki boyutlu çizimlerin gerçekleştirilebilmesi amacıyla aşağıda belirtilen şekilleri (Shapes) sunmaktadır.

- Ellips: Bu tip yardımıyla içi dolu veya boş tam daire yada elipslerin çizilmesi mümkündür.
- Line: Düz çizgilerin çizilmesini sağlayan tiptir. Başlangıç ve bitiş koordinatları düz çizginin çizilmesi için yeterlidir.
- Rectangle: Dört köşeli şekillerin çizilmesinde kullanılan tiptir. İçi boş veya dolu dikdörtgen yada kare gibi şekillerin çizilebilmesini sağlar.
- Polygon: N sayıda köşeden oluşan poligonların çizilmesinde kullanılır. Bir üçgen olabileceği gibi bir çokgen de olabilir. Diğer taraftan düzgün köşeli olmayan bir poligonda oluşturulabilir. Ayrıca poligonların içi boş veya dolu olacak şekilde oluşturulabilmesi de mümkündür.
- Polyline: Birbirlerine bitiş noktalarından bağlı bir başka deyişle uç uca eklenmiş düz çizgilerin (Line) çizilmesini sağlayan tiptir.
- Path: Birbirlerine son noktalarından bağlı olan düz çizgi veya eğri (Curve) gibi toplu şekillerin çizidirilmesini sağlayan tiptir. Farklı şekillerin bir arada kullanılabilmesini sağlamak için geometri (Geometry) tiplerinden yararlanır.

WPF, XAML (eXtensible Application Markup Language) tabanlı bir ortam sunduğundan, grafiksel şekillerin tasarım zamanında element bazlı olarak geliştirilmeleri ve sonuçlarının görülmesi mümkündür. GDI+ mimarisinde aynı durum düşünüldüğünde sonuçların ancak çalışma zamanında (run-time) elde edilebildiği unutulmamalıdır. Bu nedenle WPF bize büyük avantaj sağlamaktadır.

Bu kısa bilgilerden sonra örneklerimizi geliştirerek şekilleri daha yakından tanımaya çalışalabiliriz. Her zamanki gibi örneklerimizi geliştirirken Visual Studio 2008 Beta 2 sürümünden yararlanıyor olacağız. Bu nedenle, final sürümünde özellikle IDE bazlı bazı değişiklikler olma ihtimali olduğunu baştan belirtelim. İlk örneğimizde Ellips tipinden yararlanıyor olacağız. Bu amaçla Window nesnemizin XAML içeriğini aşağıdaki gibi tasarladığımızı düşünelim.

```xml
<Window x:Class="GrafiklerleCalismak.Elipsler" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Background="White" Title="Elipslerin Kullanımı" Height="300" Width="300">
    <Grid>
        <Ellipse Fill="DarkRed" Width="40" Height="150" Stroke="Coral" StrokeThickness="3"/>
        <Ellipse Fill="Red" Width="150" Height="40" Stroke="Black" StrokeThickness="3" Opacity="0.75"/>
        <Ellipse Width="40" Height="40" Fill="Gold" Stroke="Black" StrokeThickness="3"/>
        <Ellipse Width="10" Height="10" Stroke="DarkBlue" StrokeThickness="2" HorizontalAlignment="Left" Margin="64,56,0,0" VerticalAlignment="Top" />
        <Ellipse Height="25" HorizontalAlignment="Right" Margin="0,56,64,0" Stroke="DarkBlue" StrokeThickness="2" VerticalAlignment="Top" Width="25" />
        <Ellipse Height="50" HorizontalAlignment="Right" Margin="0,0,64,56" Stroke="DarkBlue" StrokeThickness="2" VerticalAlignment="Bottom" Width="50" />
    </Grid>
</Window>
```

Bu örnekte altı adet elips çizdirilmektedir. Ellips tipinin Fill niteliği (attribute) yardımıyla dolgu deseni belirtilebilir. Bunun dışında width ve height nitelikleri eşit oldukları takdirde tam bir dairenin çizilmesi söz konusudur. Diğer hallerde ise, yatay doğrultuda veya dikey doğrultuda uzayan bir elips oluşumu söz konusu olmaktadır. Kenar çizgilerini renk ve kalınlık olarak belirlemek amacıyla Stroke ve StrokeTickness niteliklerine değer atamaları yapılmaktadır. Stroke niteliği tahmin edileceği üzere geçerli bir fırça (Brush) ile eşleştirilebilir. Buda çizginin dolu bir renk dışında desenli olabileceği hatta içinde resim barındırabileceği anlamına gelmektedir.

> Stroke ve StrokeTickness nitelikleri diğer şekillerde de ye almaktadır. Bu nedenle tüm şekillerin çizgilerinin olabileceğini söyleyebiliriz.

Yukarıdaki XAML içeriğini Visual Studio 2008 Beta 2 ortamındaki çıktısı aşağıdaki gibi olacaktır.

![mk223_2.gif](images/mk223_2.gif)

Şekillerin bu biçimde yatay veya dikey düzlemlerde oluşturulması haricinde, açısal olaraktanda yerleştirilmesi istenebilir. Bunu bir elips üzerinde GDI+ ile gerçekleştirmek oldukça zor ve zahmetlidir. Oysaki WPF içerisinde yer alan Transform tipleri kullanılarak bu işlemler son derece kolay bir şekilde gerçekleştirilebilir.

> Transoform'dan kasıt, şeklin açısal olarak konumunun değiştirilebilmesi, büyüklüğünün ayarlanabilmesi, kendi ekseni üzerinde veya farklı bir orjine göre döndürülebilmesi, şeklin x veya y düzlemleri üzerinde yer değiştirmesi yada eğilip bükülmesi gibi aksiyonlardır.

İkinci örneğimizde bu durum incelenmektedir. Yeni penceremizin XAML içeriğinin aşağıdaki gibi olduğunu düşünelim.

```xml
<Window x:Class="GrafiklerleCalismak.ElipsTransform" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Background="White" Title="ElipsTransform" Height="300" Width="300" Loaded="Window_Loaded">
    <Grid>
        <Ellipse Width="125" Height="40" Stroke="Red" StrokeThickness="2"/> 
        <Ellipse Width="40" Height="125" Stroke="Black" StrokeThickness="2"/>
        <Ellipse Width="40" Height="125" Stroke="Red" StrokeThickness="2">
            <Ellipse.LayoutTransform>
                <RotateTransform Angle="45"/>
            </Ellipse.LayoutTransform>
        </Ellipse>
        <Ellipse Width="40" Height="125" Stroke="Black" StrokeThickness="2">
            <Ellipse.LayoutTransform>
                <RotateTransform Angle="135"/>
            </Ellipse.LayoutTransform>
        </Ellipse>
    </Grid>
</Window>
```

Burada dikkat edilmesi gereken en önemli nokta, Ellips.LayoutTransform elementinin içeriğidir. Bu elementin altında yer alan RotateTransform elementi içerisinde Angle niteliği ile bir açı değeri belirtilmektedir. Bu açı değeri, şeklin x,y eksenine göre farklı bir derecede döndürülmesini sağlamaktadır. Örnek XAML içeriğinde 45 derece ve 135 derecelik açılar ile döndürülmüş iki elips yer almaktadır. Bu içeriğin tasarım zamanındaki çıktısı aşağıdaki gibi olacaktır.

![mk223_3.gif](images/mk223_3.gif)

Atomu WPF ile daha kolay çizebildiğimizi söyleyebiliriz. Bu tip dönüştürme (Transform) işlemleri sadece RotateTransform ile sınırlı değildir. Yazımızın ilerleyen kısımlarında diğer Transform modellerinede kısaca değinmeye çalışacağız. Çok doğal olarak rotasyonların programatik olarak gerçekleştirilmesi gereken vakkalar olacaktır. Yukarıdaki atom çizelgesininin benzerini kod tarafında oluşturmak istersek, element ve niteliklerin karşılığı olan uygun sınıf (class) ve özellikleri (property) kullanmak yeterli olacaktır. Üçüncü örneğimizde bu durum incelenmektedir. Bu amaçla yeni penceremizin XAML ve kod içeriğini aşağıdaki gibi tasarladığımızı düşünelim.

XAML içeriği;

```xml
<Window x:Class="GrafiklerleCalismak.KodlaElipsTransform" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Background="White" Title="Kod Yardımıyla Elips Transform" Height="300" Width="300">
    <Grid Name="grdEllips"> 
    </Grid>
</Window>
```

Kod içeriği;

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
using System.Windows.Shapes;

namespace GrafiklerleCalismak
{
    public partial class KodlaElipsTransform : Window
    {
        private void ElipsCiz(Ellipse elps, int width, int height, int angle,Color color)
        {
            elps.Width = width;
            elps.Height = height;
            elps.Stroke = new SolidColorBrush(color);
            elps.StrokeThickness = 2;
            elps.LayoutTransform = new RotateTransform(angle);
            grdEllips.Children.Add(elps);
        }
        private void Cizdir()
        {
            ElipsCiz(new Ellipse(), 125, 40, 270,Colors.Red);
            ElipsCiz(new Ellipse(), 40, 125, 90,Colors.Gold);
            ElipsCiz(new Ellipse(), 125, 40, 45, Colors.DarkBlue);
            ElipsCiz(new Ellipse(), 125, 40, 135,Colors.Lavender); 
        }

        public KodlaElipsTransform()
        {
            InitializeComponent();
            Cizdir();
        }
    }
}
```

Bir kaçtane elipsi farklı renk, çizgi, çizgi kalınlığı ve açıda çizdirmek istediğimizden yardımcı olacak ElipsCiz isimli bir metod tasarlanmıştır. Bu metod, parametre olarak gelen Ellips nesne örneğini alıp, genişlik (Width), yükseklik (Height), Çizgi rengi (Stroke), Çizgi kalınlığı (StrokTickness) ve rotasyon için gerekli açı (Angle) değerlerini set etmektedir. Dikkat edilecek olursa, rotasyon işlemi için RotateTransform tipine ait bir nesne örneklenmekte ve yapıcı metoda (Constructor) açı değeri verilmektedir. Sonrasında ise bu nesne örneği, Ellips nesne örneğinin LayoutTransform özelliğine atanmaktadır. Söz konusu kod parçası yürütüldüğünde çalışma zamanında (run-time) aşağıdakine benzer bir ekran görtüntüsü ile karşılaşılır.

![mk223_4.gif](images/mk223_4.gif)

Yine kod yardımıyla elips çizdirmeye örnek olması açısından aşağıdaki pencerede (Window) göz önüne alınabilir.

XAML içeriği;

```xml
<Window x:Class="GrafiklerleCalismak.KodYardimiylaElips" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Background="White" Title="KodYardimiylaElips" Height="300" Width="300">
    <Grid>
        <Grid Name="grdTahta" Margin="0,74,0,8" Background="Gold" />
        <Button Height="23" HorizontalAlignment="Left" Margin="10,20,0,0" Name="btnCiz" VerticalAlignment="Top" Width="75" Click="btnCiz_Click">Çizdir</Button>
    </Grid>
</Window>
```

Kod içeriği;

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
using System.Windows.Shapes;

namespace GrafiklerleCalismak
{
    public partial class KodYardimiylaElips : Window
    {
        private void ElipsCiz()
        {
            grdTahta.Children.Clear();
            Random rnd=new Random();
            for (int i = 0; i < 3; i++)
            {
                Ellipse elps = new Ellipse();
                elps.Width = rnd.Next(50, 100);
                elps.Height = rnd.Next(50, 100);
                elps.Stroke = new SolidColorBrush(Colors.Black);
                elps.StrokeThickness = rnd.Next(1, 5);
                grdTahta.Children.Add(elps);
            }
        }
    
        public KodYardimiylaElips()
        {
            InitializeComponent();
        }
    
        private void btnCiz_Click(object sender, RoutedEventArgs e)
        {
            ElipsCiz();
        }
    }
}
```

Bu kez bir düğmeye basılması ile rastgele üretilen değerlere göre elipslerin çizdirilmesi sağlanmaktadır. Bu amaçla 50 ile 100 arasında rastgele genişlik ve yükselik değerleri elde edebilmek için meşhur Random sınıfına ait nesne örneğinden ve Next metodundan yararlanılmaktadır. Çoğu zaman oygun programlamada şekilsel olarak bazı oyun karakterlerinin saha üzerinde rastgele konumlarda çıkması istenebilir. İşte bu noktada Random sınıfına ait metodlar ve WPF ile gelen yeni şekil çizme teknikleri işimizi oldukça kolaylaştırmaktadır.

Kod yardımıyla gerçekleştirilen örneklerde dikkat edilmesi gereken noktalardan birisi, oluşturulan şekillerin mutlaka bir taşıyıcıya eklenmiş olmalarıdır. Söz gelimi yukarıdaki örneklerde Ellips nesne örnekleri Grid bileşenine alt element olarak, Children özelliğinin Add metodundan yararlanılarak eklenmektedir. Son pencereye (Window) ilişkin olarak aşağıdaki ekran görüntüsünde çalışma zamanında (run-time) oluşabilecek bir örnek çıktı yer almaktadır.

![mk223_1.gif](images/mk223_1.gif)

Sıradaki örneğimizde poligonların nasıl çizilebileceğini incelemeye çalışacağız. Poligonlar, çok sayıda köşeden oluşabilen ve kapalı olarak tasarlanabilen şekillerdir. Poligonlar sayesinde basit bir üçgen çizilebileceği gibi bir onaltıgen'de çizilebilir. Yada düzensiz bir kapalı şekil oluşturulabilir. Burada önemli olan köşe noktalarının belirlenmesidir. Köşe noktalarının belirlenmesinde Point tipinden yararlanılır. Point tipi x ve y koordinatlarını bünyesinde taşımaktadır. Polygon tipi, köşe noktalarını Points isimli bir koleksiyonda taşımaktadır. Şimdi aşağıdaki XAML içeriğini göz önüne alalım.

```xml
<Window x:Class="GrafiklerleCalismak.PolygonKullanimi" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Background="White" Title="Polygon Kullanımı" Height="300" Width="403">
    <Grid>
        <Polygon Fill="Gold" FillRule="Nonzero" Stroke="Black" StrokeThickness="2">
            <Polygon.Points>
                <Point X="20" Y="20" />
                <Point X="120" Y="20" />
                <Point X="120" Y="120" />
                <Point X="220" Y="120" />
                <Point X="220" Y="220" />
            </Polygon.Points>
        </Polygon>
        <Polygon Fill="LightSkyBlue" FillRule="Nonzero" Stroke="Black" StrokeThickness="2">
            <Polygon.Points>
                <Point X="20" Y="20"/>
                <Point X="120" Y="20"/>
                <Point X="120" Y="120"/>
                <Point X="220" Y="120"/>
                <Point X="220" Y="220"/>
            </Polygon.Points>
            <Polygon.LayoutTransform>
                <RotateTransform Angle="-45"/>
            </Polygon.LayoutTransform>
        </Polygon>
    </Grid>
</Window>
```

Örnekte iki adet Polygon elementi tanımlanmıştır. Bunlardan ikincisine -45 derecelik bir açısal döndürme işlemi uygulanmıştır. Her iki Polygon nesne örneğinin köşe noktaları Polygon.Points alt elementi (child element) içerisinde yer alan Point alt elementleri ile belirtilmektedir. Polygon nesnelerinin kenar çizgileri Stroke ve StrokTickness niteliklerine atanan değerler yardımıyla set edilmiştir. Diğer taraftan Polygon'ların dolgu rengi Fill özelliklerine atanan standart fırça (Brush) renkleri ile ayarlanmaktadır. Söz konusu XAML içeriğinin Visual Studio 2008 Beta 2 ortamındaki tasarım zamanı (design-time) çıktısı ise aşağıdaki gibi olacaktır.

![mk223_5.gif](images/mk223_5.gif)

Bir poligon kod yardımıylada oluşturulabilir. Sıradaki örneğimizde bir üçgenin kod yardımıyla oluşturulması ve düğmeye basılaraktan onbeşer derecelik artan açılar ile döndürülmesi örneklenmektedir. Bu amaçla yeni penceremize (Window) ait XAML ve kod içeriklerini aşağıdaki gibi tasarladığımızı düşünelim.

XAML içeriği;

```xml
<Window x:Class="GrafiklerleCalismak.KodYardimiylaPolygonKullanimi" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Background="White" Title="Kod Yardımıyla Polygon" Height="300" Width="300">
    <Grid Name="grdTahta">
        <Button Height="23" HorizontalAlignment="Left" Margin="8,19,0,0" Name="button1" VerticalAlignment="Top" Width="75" Click="button1_Click">Çevir</Button>
    </Grid>
</Window>
```

Kod içeriği;

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
using System.Windows.Shapes;
using System.Timers;

namespace GrafiklerleCalismak
{
    public partial class KodYardimiylaPolygonKullanimi : Window
    {
        Polygon plgy;
        int sayac = 1;

        private void Cizdir()
        {
            plgy = new Polygon();
            plgy.Points.Add(new Point(50, 50));
            plgy.Points.Add(new Point(150, 50));
            plgy.Points.Add(new Point(150, 150));
            plgy.Stroke = new SolidColorBrush(Colors.LightSalmon);
            plgy.StrokeThickness = 2;
            grdTahta.Children.Add(plgy);
        }

        public KodYardimiylaPolygonKullanimi()
        {
            InitializeComponent();
            Cizdir();
        }
    
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            plgy.LayoutTransform = new RotateTransform(sayac*15);
            sayac++;
        }
    }
}
```

Penceremizin yapıcı metodu (constructor) içerisinde Cizdir metodu ile bir Polygon nesne örneği oluşturulmakta ve Grid kontrolünün alt elementi olarak eklenmektedir. Polygon nesne örneği bir üçgeni temsil edeceğinden Points koleksiyonunda sadece üç nokta (Point) eklemesi yapılmıştır. Döndürme işleminin gerçekleştirildiği yer düğmenin Click olay (event) metodudur. Burada dikkat edilecek olursa yine LayoutTransform özelliğine bir değer ataması yapılmaktadır. Bu atama sırasında yeni bir RotateTransform nesnesi örneklenmekte ve parametre olarak artan bir açı değer verilmektedir. Uygulama test edildiğinde çalışma zamanında aşağıdaki Flash animasyonunda yer alan çıktı elde edilecektir. (Flash dosyasını görebilmek için Flasy Player'ın sisteminizde yüklü olması gerekmektedir.)

Şimdiki örneğimizde düz çizgileri nasıl çizdirebileceğimizi incelemeye çalışacağız. Düz çizgi için Line tipi kullanılmaktadır. Bir çizgi için belkide en önemli özellikler Stroke, StrokeTickness, X1, X2, Y1 ve Y2' dir. X ve Y özellikleri yardımıyla çizginin başlangıç ve bitiş noktaları belirlenir. Örneğin aşağıdaki XAML içeriğini ele alalım.

```xml
<Window x:Class="GrafiklerleCalismak.LineKullanimi" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Background="White" Title="Line Kullanimi" Height="211" Width="237">
    <Grid>
        <Line Stroke="RosyBrown" StrokeThickness="2" X1="10" Y1="10" X2="50" Y2="50"/>
        <Line Stroke="Brown" StrokeThickness="3" X1="50" Y1="50" X2="100" Y2="50"/>
        <Line Stroke="BurlyWood" StrokeThickness="4" X1="100" Y1="50" X2="50" Y2="140"/>
        <Line Stroke="CadetBlue" StrokeThickness="5" X1="50" Y1="140" X2="180" Y2="50"/>
    </Grid>
</Window>
```

Bu içeriğin tasarım zamanındaki (design-time) çıktısı aşağıdaki gibi olacaktır.

![mk223_10.gif](images/mk223_10.gif)

Dikkat edileceği üzere farklı kalınlık, renk ve lokasyonlarda yer alan çizgiler elde edilmektedir. Çizgiler özellikle kod tarafındada zaman zaman ele alınırlar. Söz gelimi bir harita üzerinden bir şehirden bir şehire doğru olabilecek hava yolu rotalarının belirlenmesi amacıyla çizgilerden yararlanılabilir. Bunu çok basit olarak sembolize etmek amacıyla aşağıdaki XAML ve kod içeriğini göz önüne alabiliriz.

XAML içeriği;

```text
<Window x:Class="GrafiklerleCalismak.KodYardimiylaLineKullanimi" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Background="White" Title="Kod Yardımıyla Line Kullanımı" Height="249" Width="422" WindowStyle="SingleBorderWindow">
    <Grid Name="grdTahta">
        <Image Name="imgHarita" MouseDown="imgHarita_MouseDown" MouseUp="imgHarita_MouseUp" Source="map_world_destination.gif" />
    </Grid>
</Window>
```

Kod içeriği;

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
using System.Windows.Shapes;

namespace GrafiklerleCalismak
{
    public partial class KodYardimiylaLineKullanimi : Window
    {
        Point basilanNokta;

        public KodYardimiylaLineKullanimi()
        {
            InitializeComponent();
        }

        private void imgHarita_MouseDown(object sender, MouseButtonEventArgs e)
        {
            basilanNokta=e.GetPosition(grdTahta); 
        }

        private void imgHarita_MouseUp(object sender, MouseButtonEventArgs e)
        { 
            Line yol = new Line();
            yol.Stroke = new SolidColorBrush(Colors.Red);
            yol.StrokeThickness = 2;
            yol.X1 = basilanNokta.X;
            yol.Y1 = basilanNokta.Y;
            Point bitisNoktasi = e.GetPosition(grdTahta);
            yol.X2 = bitisNoktasi.X;
            yol.Y2 = bitisNoktasi.Y;
            grdTahta.Children.Add(yol);
        }        
    }
}
```

Bu örnekte haritayı göstermesi için bir Image kontrolü kullanılmaktadır. Image kontrolünün source özelliğine atanan değer ile arka plan dünya haritası olarak gösterilmektedir. Kullanıcılar çalışma zamanında mouse'un tuşuna basıp bir noktadan başka bir noktaya gittiklerinde ve mouse'un tuşunu bıraktıklarında Line nesne örneği oluşturulmaktadır. Bunun için Image kontrolü üzerinde mouse tuşuna basılma ve bırakılma anlarının yakalanması gerekir.

Söz konusu anlar aşina olduğumuz MouseDown ve MouseUp olay metodlarında yakalanırlar. GDI+ ile aynı işlemleri nasıl yaptığımızı hatırlarsak eğer, kontrolün MouseDown olayına gelen parametre ile X ve Y değerlerini ayır ayrı aldığımızı görürüz. Burada durum biraz daha farklıdır. Nitekim MouseUp ve MouseDown olay metodlarında yer alan MouseButtonEventArgs parametresi, mouse tuşuna basıldığında o noktadaki X ve Y koordinatlarını yeni bir Point tipi olarak geriye döndürmektedir. GetPosition metodu X ve Y koordinatları, üzerinden alınmak istenen kontrolüde parametre olarak alır. Örnekte bu parametreye Grid kontrolünün referansı verilmiştir. Dolayısıyla Grid kontrolündeki X ve Y değerleri elde edilebilmektedir.

Mouse üzerinde basılan tuş bırakıldığında ise çizginin çizilme işlemi gerçekleştirilmektedir. Line nesne örneğinin X1 ve Y1 değerleri tahmin edileceği gibi, MouseDown içerisinde yer alan GetPosition ile elde edilen basilanNokta değişkeninden gelmektedir. Çizginin son noktası ise bu kez MouseUp olay metodu içerisindeki GetPosition çağrısı ile alınmakta ve Line nesne örneğinin X2 ve Y2 değerlerine aktarılmaktadır. Son olarak oluşturulan çizgi, Grid bileşenine alt element olarak eklenmektedir. Bu ekleme işleminin bize getirdiği önemli bir avantaj vardır. Yine GDI+ ile Windows programlama yaptığımızı düşünecek olursak, aynı senaryoda ekrana çizgiler çizdirmek için Graphics nesnesninin DrawLine metodundan yararlanıldığını görürüz.

Ne varki bu metod hep son çizginin çizilmesine öncekilerin ise kaybolmasına neden olmaktadır. Oysaki WPF mimarisinde şekiller bir taşıyıcının alt elementi olarak eklendiklerinden son çizilen şekilden öncekiler ekrandan kaybolmamaktadır. Bu durumu GDI+ ile Windows programlamada gerçeklemek için WPF'tekine benzer bir mantık ile hareket edilmekte ve ekranda duran çizgilerin sürekli hatırlanması için koleksiyonalardan yararlanılması gerekmektedir. Yukarıdaki kodun çalışma zamanındaki ekran çıktısı aşağıdaki Flash animasyonunda olduğu gibidir.(Flash dosyasını görebilmek için Flasy Player'ın sisteminizde yüklü olması gerekmektedir.)

Bu örneği daha da geliştirmeye çalışmanızı öneririm. Oldukça fazla eksiği var. Söz gelimi, mouse tuşuna basıp sürüklerken farklı renkte bir çizginin çıkartılarak gidilen rotanın gösterilmesi sağlanabilir. Mouse bırakıldığında ise asıl rengini alan rota ortaya çıkar. Üstelik örnek kodda sağ tuşa veya sol tuş kontrolü yapılmamıştır. Belkide klavyeden tuş kombinasyonları katarak sadece düz çizgi değil eğrilerin çizdirilmesinide sağlayabiliriz. Bu örneğin geliştirilmesini siz değerli okurlarıma bırakıyorum.

Gelelim Path bileşenine. Bu tip aslında kendi içerisinde birden fazla düz çizgi veya eğriyi barındırabilecek şekilde tasarlanmıştır. Temelde şekiller birbirleriyle uç uca eklenecen şekilde yeni bir grafik oluşturmaktadırlar. Dilerseniz örnek üzerinden ilerleyerek konuyu daha iyi anlamaya çalışalım. Bu amaçla aşağıdaki gibi bir XAML içeriği oluşturduğumuzu göz önüne alalım.

```xml
<Window x:Class="GrafiklerleCalismak.PathKullanimi" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Background="White"
Title="Path Kullanimi(Birbirlerine Bağlı Farklı Şekiller)" Height="261" Width="321">
    <Grid>
        <Path Stroke="Black" StrokeThickness="2">
            <Path.Data> 
                <PathGeometry>
                    <PathFigure>
                        <BezierSegment Point1="0,0" Point2="10,120" Point3="70,40"/>
                        <ArcSegment SweepDirection="Clockwise" Point="150,125" Size="50,50" IsLargeArc="True" RotationAngle="60"/>
                        <LineSegment Point="240,40"/>
                        <QuadraticBezierSegment Point1="35,135" Point2="75,75"/>
                        <PolyLineSegment>
                            <PolyLineSegment.Points>
                                <Point X="10" Y="10"/>
                                <Point X="50" Y="75"/>
                            </PolyLineSegment.Points>
                        </PolyLineSegment>
                        <PolyBezierSegment>
                            <PolyBezierSegment.Points>
                                <Point X="100" Y="100"/>
                                <Point X="150" Y="150"/>
                                <Point X="75" Y="180"/>
                            </PolyBezierSegment.Points>
                        </PolyBezierSegment>
                    </PathFigure>
                </PathGeometry>
            </Path.Data>
        </Path>
    </Grid>
</Window>
```

Path elementi içerisinde farklı şekillerin uç uca eklenmesi işini geometri tipleri üstlenmektedir. Bu geometri tipleride kendi içlerinde segmentler halinde şekilleri barındırmaktadır. Kullanılabilecek olan geometri tipleri aşağıdaki gibidir.

![mk223_12.gif](images/mk223_12.gif)

Örnekte söz konusu geometri tiplerinden PathGeometry kullanılmaktadır. PathGeometry elementinin altında yer alan tüm şekiller Segment anahtar kelimesi ile bitmektedir. Buna göre, BezierSegment ile başlayan şekiller dizisi sırasıyla, ArcSegment, LineSegment, QuadraticBezierSegment, PolyLineSegment ve PolyBezierSegment ile devam eder. Tüm bu alt elementleri farklı nitelikleri ile değişik çizgilerin oluşturulması sağlanmaktadır. BezierSegment elementinin nitekikleri sayesinde üç noktadan bükülmüş bir eğri çizimi yapılabilmektedir. ArcSegment elementinin nitelikleri ile, başlangıç koordiantları, genişlik yükseklik değerleri, eğilme açısı ve saat yönü yada saatin ters yönünde çizilecek yaylar oluşturulabilmektedir. Burada PolyLineSegment'i altında belirtilen noktalar uç uca bağlı düz çizgilerin oluşturulmasını sağlarken, PolyBezierSegment elementi altındaki noktalarda, uç uca eklenmiş eğrilerin oluşturulmasını sağlamaktadır. Örneğin tasarım zamanındaki çıktısı aşağıdaki gibi olacaktır.

![mk223_11.gif](images/mk223_11.gif)

Dikkat edilecek olursa, tüm çizgiler ister eğri ister düz olsunlar, uç uca eklenerekten birleştirilmiştir. Bu nedenle Path tipini örneğin harita gibi arka planlarda yolların birleştirilmesi amacıyla kullanabiliriz.

Bu yazımızda son olarak basit anlamda dönüştürme (Transform) işlemlerine bakıyor olacağız. Transform denilince aklımıza gelmesi gerekenler bir şeklin yön, büyüklük, düzlemsel koordinat gibi değerlerinin değiştirilmesidir. Bu sayede bir şekli herhangibir açıda döndürebilir, ebatlarını ayarlayabilir yada herhangibir düzlem üzerinde öteleyebiliriz. Transform işlemlerinde beş farklı tip rol oynamaktadır. Bu tipler aşağıdaki gibidir.

- RotateTransform: Şeklin belirtilen bir açıda, kendi ekseninde yada belirtilen orijine göre farklı bir eksende döndürülmesini sağlamak için kullanılır. Söz gelimi bir şeklin farklı açılardan gösterilmesinin sağlanmasında önemli rol oynamaktadır.
- ScaleTransform: Şeklin ebatlarının eşit oranda yada farklı oranlarda arttırılması yada azaltılmasında kullanılır. Örneğin Zoom işlemlerinde bu tip çok faydalı olacaktır.
- SkewTransform: Şeklin bükülmesini yada eğilmesini sağlamak amacıyla kullanılan tiptir.
- TranslateTransform: Şeklin x veya y düzlemleri üzerinde farklı noktalara ötelenmesi amacıyla kullanılan tiptir.
- MatrixTransform: Resim işlemede önemli bir yere sahip olan matris algoritmalarının iki boyutlu şekiller üzerinde de uygulanabilmesini sağlayan tiptir. Diğer Transform tipleri ile gerçekleştirilmesi zor olan dönüştürmelerde kullanılmaktadır. Bu tipi ilerleyen yazılarımızda ele almaya çalışacağız.

Şimdi basit bir örnek ile RotateTransform ve ScaleTransform tiplerinin nasıl kullanılabileceğini incelemeye çalışalım. Bu amaçla XAML ve kod içeriklerini aşağıdaki gibi geliştirdiğimizi düşünelim.

![mk223_13.gif](images/mk223_13.gif)

XAML içeriği;

```xml
<Window x:Class="GrafiklerleCalismak.TransformKullanimi" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Background="White" Title="Transform Kullanımı" Height="292" Width="330">
    <Grid Name="grdTahta" Width="309" Height="253">
        <Rectangle Fill="Gold" Stroke="Black" StrokeThickness="2" Name="dortgen" Width="100" Height="40" />
        <Slider Minimum="1" Maximum="5" ValueChanged="sldScale_ValueChanged" Height="21" Margin="110,14,89,0" Name="sldScale" VerticalAlignment="Top" />
        <Label Height="23" HorizontalAlignment="Left" Margin="0,12,0,0" Name="label1" VerticalAlignment="Top" Width="92">Scale Transform</Label>
        <Slider Height="21" Margin="110,49,89,0" Maximum="360" Minimum="0" Name="sldRotate" ValueChanged="sldRotate_ValueChanged" VerticalAlignment="Top" />
        <Label Height="23" HorizontalAlignment="Left" Margin="0,47,0,0" Name="label2" VerticalAlignment="Top" Width="92">Rotate Transform</Label>
    </Grid>
</Window>
```

Kod içeriği;

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
using System.Windows.Shapes;

namespace GrafiklerleCalismak
{
    public partial class TransformKullanimi : Window
    {
        public TransformKullanimi()
        {
            InitializeComponent(); 
        }

        private void sldScale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TransformYap();
        }

        private void sldRotate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TransformYap();
        }

        private void TransformYap()
        {
            if (sldRotate != null
                && sldScale != null)
            {
                TransformGroup grp = new TransformGroup();
                grp.Children.Add(new ScaleTransform(sldScale.Value, sldScale.Value));
                grp.Children.Add(new RotateTransform(sldRotate.Value));
                dortgen.LayoutTransform = grp;
            }
        }
    }
}
```

Kullanıcı Slider kontrollerindeki çubuğu hareket ettirdikçe ekran üzerindeki dörtgenin kendi ekseni üzerinde dönmesi veya büyüklüğünün değişmesi amaçlanmaktadır. Bu, aynı şekle birden fazla dönüştürme işleminin uygulanmasını gerektirmektedir. Bir başka deyişle Rectangle nesne örneğinin LayoutTransform özelliğine hem ScaleTransform hemde RotateTransform nesne örneklerinin atanması gerekmektedir. Bunun için TransformGroup adı verilen nesne örneklerinden yararlanılır. TransformGroup tipi, sahip olduğu Children özelliği ile sunduğu koleksiyon içerisinde farklı Transform tiplerini taşıyabilmektedir. Dolayısıyla TransformYap metodu içerisinde bu şekilde bir kodlama yapılmaktadır. Uygulamanın çalışma zamanındaki görüntüsü aşağıdaki Flash animasyonunda olduğu gibidir.(Flash dosyasını görebilmek için Flasy Player'ın sisteminizde yüklü olması gerekmektedir. Dosya boyutu 220 Kb olduğundan yüklenmesi zaman alabilir.)

Böylece geldik bir makalemizin daha sonuna. Bu makalemizde WPF (Windows Presentation Foundation) uygulamalarında iki boyutlu grafik (2D Graphics) işlemlerinde kullanılabilecek şekilleri ele almaya çalıştık. Son olarak basit bir transform işleminin örnek bir dörtgen üzerinde nasıl gerçekleştirileceğine değindik. Buradaki bilgilerden yola çıkarak çok daha kolay grafik işlemleri gerçekleştirebileceğimizi ve eski Windows programlamadaki GDI+ ile zorlandığımız vakkaları daha etkin bir biçimde yapabileceğimizi görmüş bulunuyoruz. İlerleyen makalelerimizde WPF ile ilişkili başka konularada değiniyor olacağız.Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](assets/GrafiklerleCalismak.rar)
