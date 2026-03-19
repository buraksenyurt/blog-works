---
title: "Data Binding İşlemlerinde Converter Kullanımı"
pubDate: 2014-08-24 06:40:00
categories:
  - WPF
tags:
  - WPF
  - C#
  - XML
  - Workflow Foundation
  - XAML
  - HTTP
  - Generics
  - Windows Phone
  - Visual Studio
---

# Data Binding İşlemlerinde Converter Kullanımı
[![Fuel level](images/Fuel level_thumb.jpg)](images/Fuel level.jpg) Merhaba Arkadaşlar,

Daha [önceki yazılarımızdan birisinde](https://www.buraksenyurt.com/post/WPF-Uzerinde-Data-Bindinge28093-Retro-Bakc4b1s-Acc4b1sc4b1) (Data-Binding Retro Bakış Açısı) özellikle WPF (Windows Presentation Foundation), Windows Phone, WF (Workflow Foundation) gibi XAML tabanlı ara birimlerin sıklıkla kullanıldığı noktalarda veri bağlama (Data Binding) işlemlerinin temellerini kavramaya çalışmış ve çok basit bir örnek ile konuyu irdelemiştik.

Bu yazımızda ise, veri bağlama işlemleri sırasında dönüştürücü tiplerden (Converters) nasıl yararlanabileceğimizi incelemeye çalışacağız. Bu güzel kabiliyet sayesinde aslında var olan çalışma zamanı veri bağlama işlemlerine müdahale edebilmekteyiz ki bu, geliştirici açısından oldukça önem arz eden bir konudur. Öyleki, geliştiricinin standart basma kalıpların dışına çıkarak hareket edebilmesine olanak sağlamakta.

Standart olarak kullanılan veri bağlama tekniklerinde bilindiği üzere kontrolün bir özelliğinin, bağlanılan veri tipinin bir özelliğine eşleştirilmesi işlemi söz konusudur ve bu noktada genellikle içeriğin string tipli olarak ele alındığına şahit oluruz. Bir başka deyişle veriyi göstermek amacıyla geliştirdiğimiz senaryolarda ağırlıklı olarak Context, Text gibi nitelikler veri sunumu için kullanılmaktadır.

Ancak bazı senaryolarda (ki edindiğim tecrübelere göre özellikle WF tarafında) gelen veri tipinin string tabanlı bir kontrol niteliği yerine farklı tipten olan bir kontrol niteliğine bağlanması istenebilir. Örneğin bir kontrolün visibility niteliğinin gelen verinin durumuna göre etkinleştirilmesi veya arka plan renginin veriye göre farklılaştırılması vb. İşte bu gibi gereksinimlerde, Converter tipler devreye girerek, bağlanan veri değerinin, kontrol niteliğinin istediği asıl tipe dönüştürülmesinde rol oynamaktadır.

Dilerseniz konuyu biraz daha iyi kavrayabilmek adına basit bir senaryo üzerinden ilerlemeye çalışalım. WPF tabanlı olarak geliştireceğimiz örnek uygulamamızda ilk etapta aşağıdaki gibi bir POCO (Plain Old CLR Objects) tipinin söz konusu olduğunu düşünelim.

[![dbcvrtr_1](images/dbcvrtr_1_thumb.png)](images/dbcvrtr_1.png)

```csharp
namespace UsingConverters 
{ 
    public class Vehicle 
    { 
        public int VehicleId { get; set; } 
        public string Name { get; set; } 
        public int FuelLevel { get; set; }        
    } 
}
```

> Örnekte Converter tiplerinin kullanımını ele almak istediğimizden basitlik adına söz konusu tipe INotifyPropertyChanged arayüzü (Interface) uygulanmamıştır.

Vehicle sınıfı içerisinde int tipinden VehicleId, FuelLevel ve string tipinden Name özellikleri (Property) bulunmakta. Bu sınıfa ait nesne örneklerinden oluşan bir koleksiyona ait verilerin ise, aşağıdaki XAML içeriğine sahip WPF penceresinde gösterilmek istendiğini düşünelim.

```xml
<Window x:Class="UsingConverters.MainWindow" 
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" 
        Title="MainWindow" Height="350" Width="600" Loaded="Window_Loaded"> 
    <Grid> 
        <ListBox x:Name="lstVehicles"  ItemsSource="{Binding}"> 
            <ListBox.ItemTemplate> 
                <DataTemplate> 
                    <StackPanel Orientation="Vertical"> 
                        <Label  
                               Content="{Binding Path=VehicleId}" 
                               FontWeight="Bold" FontSize="16"/> 
                        <Label   
                               Content="{Binding Path=Name}"/> 
                        <Label Height="24" 
                               HorizontalAlignment="Left" 
                               Background="CadetBlue" 
                               Width="{Binding Path=FuelLevel}"                             
                               Content="{Binding Path=FuelLevel}" 
                               /> 
                        <Separator Width="500"/> 
                    </StackPanel> 
                </DataTemplate> 
            </ListBox.ItemTemplate> 
        </ListBox> 
    </Grid> 
</Window>
```

ListBox bileşeni için standart bir Data Binding işlemi gerçekleştirilmektedir. Bu nedenle ItemsSource özelliği {Binding} olarak işaretlenmiştir. DataTemplate bileşenine baktığımızda ise VehicleId, Name ve FuelLevel özellikleri için çeşitli kontrol niteliklerine (Control Attribute) atamalar yapıldığı görülmektedir. Örneğin aracın adı Label kontrolünün Content niteliğine bağlanmıştır. Benzer durum VehicleId ve FuelLevel özellikleri için de geçerlidir.

FuelLevel özellik değerinin bağlanmasında ise iki niteliğe atama yapıldığı görülmektedir. Bunlardan birisi Content niteliğidir ve aslında benzin değerinin sayısal karşılığını göstermektedir. Diğer yandan Width niteliği de bu sayısal değere bağlanmış ve benzin miktarının görsel olarak boyutu değişen bir yatay bar şeklinde ifade edilmesi sağlanmıştır.

Örnek veriyi doldurabilmek için Window kontrolünün Loaded olay metodunda gerekli bazı düzenlemeler yapılmıştır. Bu kod içeriği aşağıdaki gibidir.

```csharp
using System.Collections.Generic; 
using System.Windows;

namespace UsingConverters 
{ 
    public partial class MainWindow 
        : Window 
    { 
        List<Vehicle> vehicles = null;

        public MainWindow() 
        { 
            InitializeComponent(); 
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) 
        { 
            vehicles = new List<Vehicle> 
           { 
                new Vehicle{ VehicleId=1, Name="Su Todoroki", FuelLevel=75}, 
                new Vehicle{ VehicleId=2, Name="Migel Kamino", FuelLevel=50}, 
                new Vehicle{ VehicleId=3, Name="Francesco Bernulli", FuelLevel=45}, 
                new Vehicle{ VehicleId=4, Name="Meytır", FuelLevel=60}, 
                new Vehicle{ VehicleId=5, Name="Naycıl", FuelLevel=90}, 
                new Vehicle{ VehicleId=6, Name="Şimşek", FuelLevel=23}, 
                new Vehicle{ VehicleId=7, Name="Şolet", FuelLevel=85} 
            };

            lstVehicles.DataContext = vehicles; 
        } 
    } 
}
```

vehicles isimli List koleksiyonu bir kaç Vehicle nesne örneğine sahiptir ve WindowLoaded olay metodu içerisinde bu koleksiyon içeriği DataContext özelliğine set edilmektedir. Uygulamayı bu haliyle çalıştırdığımızda aşağıdak ekran görüntüsündekine benzer bir sonuç ile karşılaşırız.

[![dbcvrtr_2](images/dbcvrtr_2_thumb.png)](images/dbcvrtr_2.png)

Aslında pek de fena bir görüntü değil? En azından benim açımdam. Yine de daha iyisi yapılabilir. Örneğin benzin seviylerini göz önüne alalım. Kutuların uzunluklarına bakıldığında araçların benzin oranlarını görsel olarak daha iyi anlayabiliyoruz. Üstelik içerisinde yazan sayısal değerlerde bize iyi bir istatistik sunmakta.

> Peki ya bu yatay bara benzer kontrollerin renklerini benzin değerlerine göre değiştirmek istesek. Söz gelimi yakıt miktarı 25 birimin altına düştüğünde rengi kırmızı olsa veya 75 ile 100 birim arasında iken elverişli anlamına gelebilecek Yeşil renkte olsa.

Bunun için Label kontrolünün Background özelliğine uygun bir değeri vermemiz yeterli olacaktır. Ancak bu değeri verirken benzin miktarına göre uygun rengin seçilmesi ve atanması gerekmektedir. Bir başka deyişle bir aracının devreye girmesi ve sayısal olarak tutulan FuelLevel değerini, görsel kontrolün niteliğinin istediği tipe dönüştürmesi işlemi icra edilmelidir. Normal şartlar da örneğin bir Label kontrolünün arka plan rengini değiştirmek istediğimizde, kod tarafında aşağıdaki tarzda bir yaklaşımı uygularız.

lbl.Background = new SolidColorBrush (Colors.Red);

Bu kod ifadesinde görüldüğü üzere Background niteliği bir SolidColorBrush ile zenginleştirilmiş ve kırmızı renkte belirlenmiştir. Converter tipi bu tarz bir yaklaşımı uygulamak durumundadır. Yani sayısal tipin aslında arka planın istediği bir Brush türevine dönüştürülmesi gerekmektedir. Şimdi aşağıdaki Converter tipini projeye ekleyereki işlemlerimize devam edelim.

[![dbcvrtr_3](images/dbcvrtr_3_thumb.png)](images/dbcvrtr_3.png)

ve kod içeriği,

```csharp
using System; 
using System.Globalization; 
using System.Windows.Data; 
using System.Windows.Media;

namespace UsingConverters 
{ 
    public class FuelLevelToSolidColorConverter 
        :IValueConverter 
    { 
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) 
        { 
            int fuelLevel = (int)value; 
            SolidColorBrush brush = null;

            if (fuelLevel <= 25) 
                brush = new SolidColorBrush(Colors.Red); 
            else if (fuelLevel > 25 
                && fuelLevel <= 50) 
                brush = new SolidColorBrush(Colors.Orange); 
            else if (fuelLevel > 50 
                && fuelLevel <= 75) 
                brush = new SolidColorBrush(Colors.LightBlue); 
            else if (fuelLevel > 75 
                && fuelLevel <= 100) 
                brush = new SolidColorBrush(Colors.DarkGreen); 
            else 
                brush = new SolidColorBrush(Colors.White);

            return brush; 
        } 
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) 
        { 
            return value; 
        } 
    } 
}
```

IValueConverter arayüzünü implemente etmiş olduğumuz FuelLevelToSolidColorConverter sınıfı iki metodu ezmek zorundadır. Bunlardan birisi veriden kontrole doğru olan yönde devreye giren Convert fonksiyonudur. Söz konusu örnekte bu fonksiyon daha ön plandadır. Nitekim sayısal int tipinden olan FuelLevel özelliğinin çalışma zamanındaki değerinin, uygun olan bir SolidColorBrush tipine dönüştürülmesi sırasında devreye girmektedir.

> .Net Framework içerisinde yer alan pek çok ve sayısız arayüz (Interface) sayesinde, var olan davranışları değiştirmek, ortamı genişletmek ve çalışma zamanına ekstra kabiliyetler kazandırmak pekala mümkündür. IValueConverter ve IPropertyNotifyChanged gibi arayüzler bunlardan sadece ikisidir.

Convert metodu devreye girdiğinde metoda gelen değer object tipinden olan value isimli parametre ile yakalanmaktadır. Bu değer Convert metodu içerisinde ele alındıktan sonra uygun bir SolidColorBrush üretilmiş ve geriye döndürülmüştür. Convert metodunun dönüş tipi yine object’ tir.

ConvertBack metodu ise tahmin edileceği üzere tam ters yönde çalışmaktadır. Yani kontrol içerisindeki ilgili nitelik değerinin, bağlanan veri içeriğine dönüştürülmesi noktasında rol oynamaktadır. Genel olarak çok fazla başvurulan bir metod değildir ve örneğimizde de aslında bir etkinliği bulunmamaktadır. Bu sebepten metoda value ismiyle gelen kontrol değişkeni doğrudan object tipinden geriye döndürülmektedir.

> Tabi bazı kritik senaryolarda ConvertBack metodunun içeriğinin de yazılması ve kontrol içeriğinin ilgili değerinin bir dönüşüm işlemine tabi tutularak veri kaynağına gönderilmesi söz konusu olabilir. Bu, özellikle Workflow Foundation tarafında tasarlanan Custom Designer kontrolleri için söz konusudur.
> Nitekim WF tarafında geliştirilen bu tip kontrollerde, Workflow Designer’ ın üzerinde yapılan kontrol bazlı değişikliklerin arka plandaki bazı tiplerin özelliklerine yansıtılması da gerekebilir.

Kod tarafında gerekli düzenlemeleri yaptıktan sonra artık yeni Converter tipini arayüz tarafında kullanabiliriz. Bunun için Window XAML içeriğini aşağıdaki gibi modifiye etmemiz yeterli olacaktır.

```xml
<Window x:Class="UsingConverters.MainWindow" 
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" 
        Title="MainWindow" Height="350" Width="600" Loaded="Window_Loaded" 
        xmlns:local="clr-namespace:UsingConverters" 
        > 
    <Window.Resources> 
        <local:FuelLevelToSolidColorConverter x:Key="FuelLevelToSolidColor"/> 
    </Window.Resources> 
    <Grid>     

        <ListBox x:Name="lstVehicles"  ItemsSource="{Binding}"> 
            <ListBox.ItemTemplate> 
                <DataTemplate> 
                    <StackPanel Orientation="Vertical"> 
                        <Label 
                               Content="{Binding Path=VehicleId}" 
                               FontWeight="Bold" FontSize="16"/> 
                        <Label 
                               Content="{Binding Path=Name}"/> 
                        <Label Height="24" 
                               HorizontalAlignment="Left" 
                               Background="{Binding Path=FuelLevel, Converter={StaticResource FuelLevelToSolidColor}}" 
                               Width="{Binding Path=FuelLevel}"                             
                               Content="{Binding Path=FuelLevel}" 
                               /> 
                        <Separator Width="500"/> 
                    </StackPanel> 
                </DataTemplate> 
            </ListBox.ItemTemplate> 
        </ListBox> 
    </Grid> 
</Window>
```

Dikkat edilmesi gereken ilk nokta FuelLevelToSolidColorConverter tipinin XAML içeriğinde bir Resource olarak belirtilmiş olmasıdır. local ön eki ile başlayan takıya dikkat edildiğinde, bu Resource’ un ilerleyen elementlerde de kullanılabilmesini sağlamak amacıyla bir de Key değeri verildiği görülmektedir. local takma adı ise Converter tipinin bulunduğu isim alanını (Namespace) işaret etmektedir.

> Resource, Window elementine bağlandığından, Window içerisindeki her alt element tarafından kullanılabilir.

Converter tipinin devreye alındığı yer ise Label kontrolünün Background özelliğidir.

{Binding Path=FuelLevel, Converter={StaticResource FuelLevelToSolidColor}}

kullanılan ifadede yer alan Converter özelliğine atanan değer ile, ilgili Label kontrolünün Background özelliğine yapılan veri bağlama operasyonlarında, FuelLevelToSolidColor takma adı ile belirtilen Resource’ un işaret ettiği IValueConverter türevinin devreye gireceği ifade edilmektedir.

Buraya kadar anlatıklarımızdan yola çıkarsak Converter tipinin ve ilgili fonksiyonlarının çalışma şekli aşağıdaki şekilde görüldüğü gibidir.

[![dbcvrtr_5](images/dbcvrtr_5_thumb_1.png)](images/dbcvrtr_5_1.png)

> .Net Framework tarafında IValueConverter arayüzünü uygulayan Built-In converter tipler de bulunmaktadır. Visual Studio arabirimindeki Object Browser yardımıyla bu tipler incelenebilir.
> [![dbcvrtr_6](images/dbcvrtr_6_thumb.png)](images/dbcvrtr_6.png)

Yapılan son değişikliklere göre uygulamanın yeni çalışma zamanı çıktısına ait sonuçlar şu şekilde olacaktır.

[![dbcvrtr_4](images/dbcvrtr_4_thumb.png)](images/dbcvrtr_4.png)

Sanırım Şimşeğin en kısa sürede pit alanına girmesi ve benzin alması grerekiyor. Gaza fena yüklenmiş belli ki.

Görüldüğü üzere söz konusu kontroller yakıt seviyesine göre arka plan renklerini değiştirmiş ve görsel açıdan kullanıcı deneyimi biraz daha iyi olan bir sonuç ortaya çıkmıştır. Siz de farklı senaryolarda farklı Converter tiplerini geliştirmeyi deneyerek antrenmanlar yapabilirsiniz. IValueConverter arayüzü sayesinde epey bir esnekliğimiz olduğunu fark etmişsinizdir. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[UsingConverters.zip (65,26 kb)](assets/UsingConverters.zip)
