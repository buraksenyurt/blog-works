---
layout: post
title: "WPF - Grafik İşlemlerinde Fırçaların(Brushes) Kullanımı"
date: 2007-09-11 12:00:00 +0300
description: "Windows uygulamalarında özellikle .Net tarafında vektörel grafik işlemleri için çoğunlukla GDI+ alt yapısı(infrastructure) kullanılmaktadır. Ancak Windows Presentation Foundation(WPF) mimarisinde öne çıkan özelliklerden biriside grafik anlamdaki yeteneklerin son derece geliştirilmiş olmasıdır..."
categories:
  - wpf
tags:
  - wpf
  - xml
  - csharp
  - dotnet
  - linq
  - xaml
  - http
  - performance
  - generics
  - visual-studio
---
# WPF - Grafik İşlemlerinde Fırçaların(Brushes) Kullanımı
Değerli Okurlarım Merhabalar,

Windows uygulamalarında özellikle.Net tarafında vektörel grafik işlemleri için çoğunlukla GDI+ alt yapısı (infrastructure) kullanılmaktadır. Ancak Windows Presentation Foundation (WPF) mimarisinde öne çıkan özelliklerden biriside grafik anlamdaki yeteneklerin son derece geliştirilmiş olmasıdır. Kişisel görüşüm grafik yönündeki özelliklerin gelişmesi dışında bu tekniklerin element bazında uygulanabiliyor olması son derece önemli ve yerindedir.

Yani XAML tarafında grafikler ile daha kolay çalışabilme imkanının gelmiş olması önemlidir. Bu yazımız ile birlikte WPF içerisindeki grafiksel öğeleri incelemeye başlıyor olacağız. Grafik konusu oldukça geniş olduğundan bir iki yazı dizisi halinde incelememiz çok daha yerinde olacaktır. Hepimizin GDI+ alt yapısından aşina olduğumuz teknikler aslında WPF içinde geçerlidir. Ancak önemli olan temel bazı kavramlar vardır. Bu kavramlar grafik işlemlerinde önemli bir yere sahiptir. Söz konusu kavramlar ve açıklamaları aşağıda maddeler halinde sunulmaktadır.

- Brushes: Bir alanın farklı biçimlerde boyanabilmesi için fırçalara ihtiyaç vardır. Burada tek renk tonlaması (Solid), gradyan tonlamalar (Gradients), değişik tipteki desenler (Patterns) veya resimler (Image) bir şeklin boyanmasında kullanılabilir. Hatta var olan Windows bileşenlerini, görsel materyalleri (örneğin video dosyalarını) şekillerin içerisini doldurmak amacıyla kullanabiliriz. Windows Presentation Foundation içersinde bu anlamda tasarlanmış pek çok hazır Brush tipi yer almaktadır. LinearGradientBrush yada RadialGradientBrush tipleri bunlara örnek olarak verilebilir.
- Shapes: Grafik uygulamalarının olmassa olmaz parçalarından birisi de şekillerdir. Kare, dikdörtgen, daire, elips, yay, çizgi vb... bu anlamda göz önüne alınabilir. WPF, iki boyutlu (2D) şekilleri doğrudan destekleyen tipler içermektedir.
- Transformations: Grafik öğelerinin ekran üzerinde döndürülmelerinin (Rotation), ebatlarının ayarlanmasının (Scaling) sağlanması ile ilgili konuları kapsar. Söz gelimi bir karenin kendi ekseninde dönmesi (Rotation) ve büyüklüğünün ayarlanması bu konuya örnek olarak verilebilir.
- Imaging: Bitmap resimlerin farklı formatlara dönüştürülmesi veya resimler üzerinde bilinen bazı efektlerin uygulanması gibi konuları kapsar. Söz gelimi bir resmin daha bulanık gösterilmesinin sağlanması bu konuya örnek olarak verilebilir.
- Animations: Grafik nesneleri üzerinde bazı animasyonların yapılabilmesi için gereken teknikleri içeren kavramdır.

İzleyen örneklerimizi yine Visual Studio 2008 Beta 2 sürümünde tasarladığımızı baştan belirtelim. Bu nedenle final sürümünde değişen bazı kullanım kolaylıkları olabilir. Elbette artık relase edilmiş olan WPF içinde IDE bazlı bir değişiklik olmayacağını söyleyebiliriz. Dilerseniz basit bir örnek ile başlayalım. İlk olarak aşağıda XAML (eXtensible Application Markup Language) içeriği belirlenen bir pencere (Window) geliştirelim.

Merhaba Fırçalar;

```xml
<Window x:Class="GrafiklerleCalismak.MerhabaFircalar" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Background="White" Title="Fill, Transparant Fill, SolidFillBrush Kullanımı" Height="300" Width="300">
<Grid>
    <Rectangle Fill="Goldenrod">
         <Rectangle.Height>45</Rectangle.Height>
         <Rectangle.Width>45</Rectangle.Width>
    </Rectangle>
    <Rectangle Fill="#99FFCC66" Width="40" HorizontalAlignment="Left" Margin="94,81,0,81" />
    <Ellipse Margin="78,130,54,42">
        <Ellipse.Fill>
            <SolidColorBrush Color="BlueViolet" Opacity="0.6"/>
        </Ellipse.Fill>
    </Ellipse>
    <Rectangle HorizontalAlignment="Right" Width="82" Margin="0,58,54,0" Height="45" VerticalAlignment="Top">
        <Rectangle.Fill>
            <SolidColorBrush>
                <SolidColorBrush.Color>
                    <Color A="90" R ="50" G="50" B="150"/>
                </SolidColorBrush.Color>
            </SolidColorBrush>
        </Rectangle.Fill>
    </Rectangle>
</Grid>
</Window>
```

Bu örneğin tasarım zamanındaki (Design Time) çıktısıda aşağıdaki gibi olacaktır.

![mk222_1.gif](/assets/images/2007/mk222_1.gif)

Bu örnekte dört farklı şekil yer almaktadır. İlk dörtgenimiz Fill niteliğinde bilinen bir renk tanımlaması ile oluşturulmuş Rectangle nesne örneğidir. Bu dörtgen aslında SolidColorBrush için en temel örnektir. SolidColorBrush, bir şeklin içini boşluk bırakmayacak şekilde doldurmak üzere tasarlanmış bir tiptir. Örnekte bu açık bir şekilde belirtilmesede Fill niteliğine atanan renk değeri bir SolidColorBrush oluşumunu sağlamıştır.

```xml
<Rectangle Fill="Goldenrod">
```

İkinci dörtgende ise Fill niteliğine hexadecimal olarak değer ataması aşağıdaki gibi gerçekleştirilmiştir.

```xml
<Rectangle Fill="#99FFCC66" Width="40" HorizontalAlignment="Left" Margin="94,81,0,81" />
```

Yanlız burada dikkat edilmesi gereken bir nokta vardır. Bu da ilk iki hanededeki değerlerdir. Bu değerler 00 ile FF arasında olabilien alfa (Alpha) değeridir. Bir başka deyişle şeklin saydamlığının (Transparancy) belirlenmesi için kullanılır. 00 olması tam saydamlık anlamına gelir.

Üçüncü dörtgenin çizimi sırasında SolidColorBrush elementi açık bir şekilde aşağıdaki gibi kullanılmıştır.

```xml
<Rectangle.Fill>
            <SolidColorBrush>
                <SolidColorBrush.Color>
                    <Color A="90" R ="50" G="50" B="150"/>
```

Bu kullanıma göre dolgu renginin belirlenmesinde RedGreenBlue kombinasyonuna ve saydamlık içinde Alpha değerine bakılmaktadır. Bu değerler Color elementi içerisinde birer nitelik (attribute) yardımıyla belirlenmektedir. SolidColorBrush elementinin kullanılması halinde, transformasyon işlemlerinin gereçekleştirilmesi dahada kolay olmaktadır. Bu sebepten transformasyon işlemlerinin söz konusu olduğu durumlarda SolidColorBrush elementinin açık bir şekilde örnekteki gibi kullanılması önerilmektedir. Son olarak ilk örneğimizde dörtgenden farklı olarak bir elips (Ellipse) şekli çizdirilmiştir.

```xml
<Ellipse.Fill>
            <SolidColorBrush Color="BlueViolet" Opacity="0.6"/>
```

Elipsin dolgu rengini belirlemek içinse Ellipse.Fill elementi altında SolidColorBrush isimli alt element (Child Element) kullanılmıştır. Color niteliğine atanan değer ile fırça rengi ve Opacity niteliğine atanan değer ilede şeffaflık değeri bildirimiştir. Bu örnekteki işlemlerin çoğu kod tarafında da gerçekleştirilebilir. Söz gelimi elipsin çizimi ve doldurulmasını aşağıdaki kodlar yardımıyla sağlayabiliriz.

```csharp
public partial class MerhabaFircalarKodIle : Window
{
    private void SekilleriCiz()
    {
        Ellipse elips = new Ellipse();
        elips.Margin = new Thickness(78, 130, 54, 42); 
        SolidColorBrush firca = new SolidColorBrush(Colors.BlueViolet);
        firca.Opacity = 0.6;
        elips.Fill = firca;
        grdAlan.Children.Add(elips);
    }
    public MerhabaFircalarKodIle()
    {
        InitializeComponent();
        SekilleriCiz();
    }
}
```

Elbette bu kez sonuçlar tasarım zamanında (Design Time) değil, çalışma zamanında (Run Time) görülebilecektir. Aşağıdaki ekran görüntüsünde bu durum gösterilmektedir. Buradan şu sonuca bir kez daha varabiliriz; XAML deki element ve niteliklerin karşılıkları kod tarafında sınıf ve özelliklere denk gelmektedir.

![mk222_16.gif](/assets/images/2007/mk222_16.gif)

Gradyan Dolgular (Gradient Brush);

İlk örnekte basit olarak SolidColorBrush kullanımına değinmeye çalıştık. Dolgu işlemlerinde popüler olan kullanımlardan biriside Gradyan efektlerin verilmesidir. Bu renkler arasında yumuşak geçişlerin oluşmasına ve bu sayede daha güzel dolguların görünmesine neden olmaktadır. Böyle bir amacı gerçekleştirmek için WPF içerisinde çeşitli tipler yer almaktadır. Sıradaki örneğimizde bu durumu incelemeye çalışacağız. Bu amaçla aşağıdaki XAML içeriğini göz önüne alabiliriz.

```xml
<Window x:Class="GrafiklerleCalismak.LinearGradientFirca" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Background="White" Title="LinearGradientBrush ve RadialGradientBrush Kullanımı" Height="304" Width="362">
<Grid>
    <Rectangle Margin="16,14,21,0" Height="69" VerticalAlignment="Top">
        <Rectangle.Fill>
            <LinearGradientBrush>
                <GradientStop Color="Blue" Offset="0"/>
                <GradientStop Color="Red" Offset="0.25"/>
                <GradientStop Color="White" Offset="0.5"/>
                <GradientStop Color="Black" Offset="0.75"/>
                <GradientStop Color="Gold" Offset="1"/>
            </LinearGradientBrush>
        </Rectangle.Fill>
    </Rectangle>
    <Rectangle Margin="96,101,101,18">
        <Rectangle.Fill>
            <RadialGradientBrush GradientOrigin="0.1,0.75">
                <RadialGradientBrush.GradientStops>
                    <GradientStop Color="Gold" Offset="0"/>
                    <GradientStop Color="Black" Offset="0.50"/>
                    <GradientStop Color="DarkSlateGray" Offset="1"/>
                </RadialGradientBrush.GradientStops>
            </RadialGradientBrush>
        </Rectangle.Fill>
    </Rectangle> 
</Grid>
</Window>
```

Bu pencerenin tasarım zamanındaki (Design Time) çıktısı aşağıdaki gibi olacaktır.

![mk222_2.gif](/assets/images/2007/mk222_2.gif)

Burada üst taraftaki dörtgen içeriği LienarGradientBrush ile, alttaki dörtgen ise RadialGradientBrush fırçaları ile doldurulmuştur. Her iki fırçanında ortak noktalarına bakıldığında farklı renkler ve geçiş noktalarının belirlenmesi amacıyla GraidentStop elementlerine başvurulduğu görülmektedir. Bu elementler renkleri ve bunların geçiş yerlerinin neresi olduğunu belirtmek amacıyla kullanılmaktadır. GradientStop tipine ait nesne örnekleri, GradientStopCollections adı verilen özel bir koleksiyonda tutulmaktadır. Diğer taraftan ikinci şelilde odak noktası dikkat edileceği üzere tam merkez değildir. Bunun belirlenmesi için RadialGradientBrush elementinin GradientOrigin niteliğinden (attribute) yararlanılmaktadır.

Kod Tarafından Fırça (Shape) Kullanımı;

Şu ana kadar geliştirilen örneklerde şekillerin oluşturulması ve içlerinin doldurulması gibi işlemlerde çoğunlukla XAML elementlerinden ve niteliklerinden yararlanılmıştır. Bu geliştirici açısından tasarım zamanı (Design Time) için büyük bir esnekliktir. Nitekim işlemlerin sonuçları anında Visual Studio arabirimi üzerinde görülebilmektedir. Diğer taraftan bazı durumlarda çalışma zamanında dinamik olarak söz konusu şekillerin çizilmesi ve doldurulması istenebilir. Örneğin.Net ile yazılmış grafik uygulamalarında bu mutlaka olması gereken bir özelliktir. Sıradaki örnekte basit olarak bir dörtgenin LinearGradientBrush sınıfı ile kod tarafındaki tiplerden ve üyelerden yararlanılarak nasıl doldurulacağı örneklenmektedir. Bu amaçla yeni bir Window arkasında aşağıdaki kodlar kullanılmıştır.

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
    public partial class KodYardimiylaGradient : Window
    {
        private void LinearGradientDortgenCiz()
        {
            // LinearGradientBrush nesne örneği oluşturulur.
            LinearGradientBrush fircam = new LinearGradientBrush();

            // C# 3.0 ile gelen Object Initializers kullanılmıştır.
            GradientStopCollection noktalar = new GradientStopCollection() { new GradientStop(Colors.WhiteSmoke, 0), new GradientStop(Colors.RosyBrown, 0.25), new GradientStop(Colors.Salmon, 0.50), new GradientStop(Colors.Silver, 0.75), new GradientStop(Colors.Gold, 0.1) };
            // Gradient Stop noktaları set edilir
            fircam.GradientStops = noktalar;
            fircam.Opacity = 0.80; // Saydamlık değeri ayarlanır

            fircam.Freeze(); //Nesnenin değiştirilemeyeceğini belirtir. Bu metod çoğunlukla performans açısından kullanılır.

            // Dörtgen nesnesi örneklenir.
            Rectangle dortgen = new Rectangle();
            // Dörtgenin genişlik ve yükseklik değerleri belirlenir.
            dortgen.Width = 200;
            dortgen.Height = 50;        
            dortgen.Margin = new Thickness(0, 0, 0, 0); // Sağ, sol kenar uzaklıkları belirlenir. Buna göre şekil formun tam ortasında olacaktır.
            dortgen.Fill = fircam; // Dortgenin içinin fircam isimli LinearGradientBrush sınıfının değerleri ile doldurulacağı belirlenir.

            grdAlan.Children.Add(dortgen); // Dortgen nesnesi Grid alanı içerisine eklenir.
        }
        public KodYardimiylaGradient()
        {
            InitializeComponent();
            LinearGradientDortgenCiz();
        }
    }
}
```

Dikkat edilecek olursa XAML elementlerinin tip karşılıkları kullanılarak istenen sonuçlar elde edilebilmektedir. LinearGradientBursh ile örneklenen fırçanın renk tonlarını ve geçiş noktalarını belirlemek için GradientStopCollection koleksiyonuna ait bir nesne örneklenmiştir.

> Bu örnekleme işlemi sırasında C# 3.0 ile birlikte gelen nesne başlatıcılarından (Object Initializers) yararlanılmıştır. Böylece koleksiyona ait nesneyi örneklediğimiz satırda içerisinde olmasını istediğimiz GradientStop nesne örnekleride belirtilebilmiştir. Nesne başlatıcıları sadece koleksiyonlarda değil tiplerin örneklenmesi işlemlerinde de kullanılabilmektedir.

İlerleyen satırlarda fırçanın saydamlık değeri Opacity özelliği ile belirtilmektedir. Freeze metodu özel olarak performansı arttrmak adına kullanılması MSDN dökümanlarında tavsiye edilen bir metoddur. Freeze metodu ile ilgili olarak dikkat edilmesi gereken noktalardan biriside bu metod çağrısından sonra fırça özelliklerinin değiştirilemediğidir. Dortgen nesnesi Rectangle sınıfı yardımıyla oluşturulmaktadır. Dortgenin içinin hangi fırça ile doldurulacağı ise Fill özelliğine atanan değer ile belirlenir. Son olarak, dörtgenin Grid içerisinde gösterilmesini sağlamak için Children özelliğine Add metodu ile Rectangle nesne örneğinin eklenmesi sağlanmıştır. Bu işlemlere göre söz konusu pencerenin çalışma zamanındaki çıktısı aşağıdaki gibidir.

![mk222_3.gif](/assets/images/2007/mk222_3.gif)

Resimleri Fırçalar ile Kullanmak;

Çok doğal olarak bir şeklin içinin resimler ile doldurulması ve farklı bir taban deseni oluşturulması istenebilir. Bunun için WPF içerisinde ImageBrush tipi kullanılmaktadır. Aşağıdaki XAML içeriğinde, dörtgenin içinin örnek bir resim ile doldurulması sağlanmaktadır.

```xml
<Window x:Class="GrafiklerleCalismak.ImageBrushKullanimi" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Background="White" Title="ImageBrushKullanimi" Height="300" Width="300">
<Grid>
    <Rectangle Margin="52,48,54,55">
        <Rectangle.Fill>
            <ImageBrush ImageSource="ArkaPlan.png" TileMode="Tile" Viewport="0,0,0.1,0.1"/>
        </Rectangle.Fill>
    </Rectangle>
</Grid>
</Window>
```

Bu örneğin tasarım zamanındaki (Design Time) çıktısı aşağıdaki gibi olacaktır. Örnekte ArkaPlan.png isimli bir resim kullanılmıştır.

![mk222_4.gif](/assets/images/2007/mk222_4.gif)

ImageBrush elementinin dikkate değer nitelikleri ImageSource, TileMode ve ViewPort'dur. ImageSource ile tahmin edileceği gibi içerik resmi belirlenmektedir. TileMode ile resmin şekil içerisine nasıl döşeneceği belirtilir. ViewPort niteliğine atanan dört sayısal değer bulunmaktadır. Bunlardan ilk ikisi konumu, son ikiside şekil içerisinde kaç adet resim gösterileceğini belirtir. Yukarıdaki örnek göz önüne alındığında 0.1,0.1 değerlerinin verilmesi ile 10X10 resmin şekil içerisinde yer aldığı görülmektedir. TileMode değerlerinin farklı şekillerde ayarlanmasının sonuçları aşağıdakine benzerdir.

TileMode Değerine Göre Dolgu Çeşitleri

FlipX
FlipY

![mk222_5.gif](/assets/images/2007/mk222_5.gif)
![mk222_6.gif](/assets/images/2007/mk222_6.gif)

FlipXY
None

![mk222_7.gif](/assets/images/2007/mk222_7.gif)
![mk222_8.gif](/assets/images/2007/mk222_8.gif)

WPF Bileşenlerini Fırça ile Kullanmak;

Var olan resimleri şekillerin içerisini boyamak amacıyla kullanabileceğimiz gibi, WPF deki pek çok bileşenide dolgu efekti olarak ele alabiliriz. Bunun için VisualBrush tipinin kullanılması yeterlidir. Söz gelimi bir dörtgenin içeriğinin Button nesneleri ile doldurulmasını istediğimizi düşünelim. Bunu gerçekleştirmek için aşağıdaki XAML çıktısı göz önüne alınabilir.

```xml
<Window x:Class="GrafiklerleCalismak.VisualBrushKullanimi" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Background="White" Title="VisualBrush Kullanimi" Height="300" Width="300">
<Grid>
    <Rectangle Margin="40,50,42,50">
        <Rectangle.Fill>
            <VisualBrush TileMode="Tile" Viewport="0,0,0.2,0.2">
                <VisualBrush.Visual>
                    <Grid>
                        <Button Content="Selam!"/>
                    </Grid>
                </VisualBrush.Visual>
            </VisualBrush>
        </Rectangle.Fill>
    </Rectangle>
</Grid>
</Window>
```

Bu sayfanın tasarım zamanındaki (Design Time) çıktısı aşağıdaki gibidir.

![mk222_9.gif](/assets/images/2007/mk222_9.gif)

WPF kontrollerinin dolgu efekti olarak kullanılabilmesini sağlayan, VisualBrush elementinin VisualBrush.Visual alt elementidir. Bu elementin içerisinde bilinen WPF taşıyıcıları (Containers) kullanılabilir. Örnekte bir Grid kontrolü ve içerisidende tek bir Button nesnesi kullanılmıştır. Tabi ViewPort niteliğinde belirtilen değerlere göre Button kontrolünün dörtgen içerisindeki yerleşim sayısı değişecektir. Aynen ImageBrush tipinde olduğu gibi TileMode niteliğinin değerine göre yerleşimler aşağıdaki gibi farklı biçimlerde olabilir.

TileMode Değerine Göre Dolgu Çeşitleri

FlipX
FlipY

![mk222_10.gif](/assets/images/2007/mk222_10.gif)
![mk222_11.gif](/assets/images/2007/mk222_11.gif)

FlipXY
None

![mk222_12.gif](/assets/images/2007/mk222_12.gif)
![mk222_13.gif](/assets/images/2007/mk222_13.gif)

Sistem Renklerinin Fırçalar ile Kullanımı;

Windows işletim sisteminde kullanılan pek çok görünüm vardır. Örneğin masaüstü rengi, aktif olan pencerelerin bar kısımlarındaki renkler ve aralarındaki geçişler vb. Çoğu zaman Windows uygulaması içerisindeki bazı dolguların, sistemdeki hazır dolgulardan alınması istenebilir. Böyle bir durumda kullanıcı, makinesindeki sistem renklerini değiştirdiğinde Windows uygulaması içerisindeki şekillerde buna uygun bir biçimde güncellenebilecektir. Bunu gerçekleştirmek için SystemColors tipi kullanılır. Aşağıdaki XAML içeriğinde bu durum irdelenmektedir.

```csharp
<Window x:Class="GrafiklerleCalismak.SystemColorsKullanimi" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Background="White" Title="SystemColors Kullanimi" Height="300" Width="300">
<Grid>
    <Rectangle Fill="{DynamicResource {x:Static SystemColors.WindowBrushKey}}" Margin="26,31,106,0" Height="86" VerticalAlignment="Top" />
</Grid>
</Window>
```

Burada Fill niteliğine özel bir atama gerçekleştirilmiştir. DynamicResource ifadesini takip eden kısımda SystemColors.WindowBrushKey ile dolgu rengi, Windows işletim sistemi üzerinden alınmaktadır. Buna göre örneğin benim sistemimdeki çıktı aşağıdaki gibidir.

![mk222_14.gif](/assets/images/2007/mk222_14.gif)

SystemColors tipi System.Window isim alanı (Namespace) altında yer almakta olan static bir sınıftır ve aşağıdaki gibi pek çok hazır değere sahiptir.

> Hatırlanacağı gibi static sınıflar (class) örneklenemeyen, türetilemeyen, sadece static üyeler içeren bir tiptir. Normal sınıflara göre daha hızlı çalıştıklarından duruma göre performans amacıyla tercih edilebilirler.

![mk222_15.gif](/assets/images/2007/mk222_15.gif)

Burada dikkat edilmesi gereken noktalardan birisi tüm bu değerlerin Fill işlemlerinde kullanılmadığıdır. Bir başka deyişle Fill özelliği, buradaki static üyelerden her birini kabul etmez. Özellikle sistem tarafında set edilmiş olan ve Key anahtar kelimesi ile bitenleri tercih etmek gerekmektedir.

Drawing ile Fırça Kullanımı;

Dilersek bir şeklin içini çizerekte doldurabiliriz. Söz gelimi videoları, metinleri, başka şekilleri çizim amacıyla dolgularda ele alabiliriz. Bu oldukça geniş bir konu olmasına rağmen bir örnek yapmadan geçmenin doğru olmayacağı kanısındayım. Bu tip bir işlemde DrawingBrush tipi esas olan noktadır. DrawingBrush tipi ile oluşturulan fırçalar söz konusu alanları, GemoetryDrawing, ImageDrawing, GlpyhRunDrawing, VideoDrawing, DrawingGroup gibi tipler yardımıyla farklı şekillerde doldurabilirler. Söz gelimi bir metnin içerisinde geometrik şekiller gösterilmesini (GeometryDrawing ile), yada bir elips içerisinde bir video oynatılmasını (VideoDrawing ile) sağlayabiliriz. Aşağıdaki örnek kod parçasında, bir elips içerisinde intro.wmv isimli videonun oynatılmasını sağlamak amacıyla yazılmış ifadeler yer almaktadır.

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
using System.Windows.Media.Animation;

namespace GrafiklerleCalismak
{
    public partial class DrawingBrushKullanimi : Window
    {
        private void VideoCiz()
        { 
            // Elips oluşturulur
            Ellipse elips = new Ellipse();
            // Yükseklik ve genişlik belirtilir.
            elips.Width = 250;
            elips.Height = 75;
        
            // Video dosyasını oynatacak bir MediaPlayer nesnesi örneklenir.
            MediaPlayer oynatici = new MediaPlayer();
            // intro.wmv dosyasının açılması sağlanır.
            oynatici.Open(new Uri("..\\..\\intro.wmv",UriKind.Relative)); 
    
            // DrawingBrush' ın kullanacağı VideoDrawing nesnesi örneklenir.
            VideoDrawing vd=new VideoDrawing();
            // Oynatıcı set edilir.
            vd.Player=oynatici;
            // Videonun oynayacağı alan belirlenir. Örnekte bu alan elips' inki ile aynı tutulmuştur.
            vd.Rect = new Rect(0, 0, 250, 75); 
    
            // Fırça oluşturulur ve Drawing özelliğine VideoDrawing nesne örneği aktarılır
            DrawingBrush fircam = new DrawingBrush();
            fircam.Drawing = vd;
        
            // Elipsin için dolduracak nesne örneği belirlenir.
            elips.Fill = fircam; 
        
            // Elips Grid içerisine eklenir
            grdAlan.Children.Add(elips);
    
            // Video oynatılır.
            oynatici.Play();
        }

        public DrawingBrushKullanimi()
        {
            InitializeComponent(); 
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VideoCiz();
            }
            catch (Exception excp)
            {
                MessageBox.Show(excp.Message);
            }
        }
    }
}
```

Uygulama test edildiğinde aşağıdaki Flash videosunda görüldüğü gibi düğmeye basıldığında ilgili videonun elips içerisinde oynadığı görülür. (Flash animasyonunun oynatılabilmesi için sisteminizde Flash Player'ın yüklü olması gerekebilir.)

Video efektlerini ilerleyen yazılarımızda incelemeye devam ediyor olacağız. Şimdilik yazımızın sonuna geldik. Bu yazımızda temel olarak grafik işlemlerinde şekillerin fırçalar (Brushes) yardımıyla nasıl doldurulabileceğinin belirlenmesinde kullanılan tipleri, ağırlıklı olarak XAML içerisinde ele almaya çalıştık. Bir sonraki makalemizde grafik işlemlerinde kullanılabilecek şekilleri (Shapes) incelemeye çalışıyor olacağız. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](/assets/files/2007/GrafiklerleCalismak.zip)