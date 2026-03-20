---
layout: post
title: "WPF Üzerinde Data Binding– Retro Bakış Açısı"
date: 2014-06-18 16:55:00 +0300
categories:
  - wpf
tags:
  - wpf
  - xml
  - csharp
  - dotnet
  - linq
  - windows-forms
  - silverlight
  - xaml
  - http
  - windows-phone
---
Bizim dünyamızda zaman hızla akar ve eskiler eskide kalıp, yerini yeniler almaya başlar. Her ne kadar uzun ömürlü kavramlar söz konusu olsa da genel itibariyle yazılım dünyası böyledir. Bazen durup geriye bakar, eskiden nasıl yaptığımızı hatırlar, sonra yenisine dönerek bir kıyaslama yaparız. İşte bu yazımızda yıllarca eski stilde geliştirme yapmış klasik bir.Net yazılımcısının gözünden, yenilikçi bir konuya (ki çıkalı da çok çok çoook zaman olmuştur) bakmaya çalışacağız. Buyrun bakalım.

[![retro_car-1590](/assets/images/2014/retro_car-1590_thumb.jpg)](/assets/images/2014/retro_car-1590.jpg)


XAML doğduğundan beri gerek WPF (Windows Presentation Foundation), gerek Silverlight, gerek Windows Phone tarafı olsun pek çok yeniliği ve farklı geliştirme bakış açılarını da beraberinde getirmiş oldu. Bu alanlardan birisi de özellikle kontrol odaklı veri bağlama (Data Binding) stratejileri üzerinedir. Bu anlamda pek çok ve farklı veri bağlama tekniğini bulmak mümkün.

Doğruyu söylemek gerekirse yeni nesil veri bağlama işlemleri daha kolay olmasına karşın klasik stilde programlama yapanlara biraz yabancı gelebilmektedir. İşte bu yazımızda klasik bir Windows Forms geliştiricisi olarak, anladığımız kadarı ile Data Binding kavramına giriş yapmaya çalışıyor olacağız. Olayı ilk önce klasik yaklaşım modeline göre ele almakta fayda var. Aslında klasik yaklaşım modelinden yenilikçi XAML modeline geçiş yaparak artıları görmeye çalışacağımızı ifade edebiliriz.

Senaryo

Senaryomuzda WPF tabanlı bir Windows uygulaması söz konusudur. Window kontrolü üzerinde yer alan TextBox, TextBlock gibi kontroller, Product tipinden bir nesne örneğinin özellikleri ile ilişkilendirileceklerdir. Product tipi ilk etapta basit bir POCO (Plain Old CLR Object) sınıfı olarak tasarlanmalıdır. Buna göre çalışma zamanında Product nesne örneklerinde veya kontrol üzerinden yapılan değişikliklerin, karşı tarafa da yansıtılması istenmektedir. Yani senkronizasyon çift taraflı olarak başarılı bir şekilde sağlanabilmelidir.

İlk Tasarım

WPF tabanlı uygulamamızda yer alan MainWindow nesnesinin ilk hali aşağıdaki gibidir.

```xml
<Window x:Class="HowTo_FundementalsOfBinding.MainWindow" 
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" 
        Title="MainWindow" Height="175" Width="400" Loaded="Window_Loaded_1"> 
    <Grid> 
        <Grid.ColumnDefinitions> 
            <ColumnDefinition Width="80"/> 
            <ColumnDefinition/> 
        </Grid.ColumnDefinitions> 
        <Grid.RowDefinitions> 
            <RowDefinition Height="25"/> 
            <RowDefinition Height="25"/> 
            <RowDefinition Height="25"/> 
            <RowDefinition Height="25"/> 
            <RowDefinition Height="25"/> 
            <RowDefinition Height="25"/> 
        </Grid.RowDefinitions> 
        <TextBlock Text="Product ID" Grid.Column="0" Grid.Row="0"/> 
        <TextBlock Text="Product Name" Grid.Column="0" Grid.Row="1"/> 
        <TextBlock Text="Product Price" Grid.Column="0" Grid.Row="2"/> 
        <TextBlock x:Name="txtProductId" Grid.Column="1" Grid.Row="0"/> 
        <TextBox x:Name="txtName" Grid.Column="1" Grid.Row="1" Margin="2,2,2,2"/> 
        <TextBox x:Name="txtListPrice" Grid.Column="1" Grid.Row="2" Margin="2,2,2,2"/> 
        <Button x:Name="btnChange" Grid.Column="1" Grid.Row="3" Content="Change" Width="60" 
                HorizontalAlignment="Right" Margin="2,2,2,2" Click="btnChange_Click_1"/> 
    </Grid> 
</Window>
```

[![ub_1](/assets/images/2014/ub_1_thumb.png)](/assets/images/2014/ub_1.png)

Tasarıma göre pencere üzerinde yer alan kontroller görsel olarak konumlandırılmış ve özellikle kod tarafında erişilebilirlikleri için x:Name nitelikleri (attribute) ile zenginleştirilmişlerdir.

Binding Olmadan Önce

Eğer XAML tarafındaki zengin veri bağlama seçeneklerinin olmadığını düşünürsek, bu durumda büyük ihtimalle aşağıdaki gibi bir kodlama gerçekleştiririz.

```csharp
using System.Windows;

namespace HowTo_FundementalsOfBinding 
{ 
    public partial class MainWindow 
        : Window 
    { 
        Product computer = null;

        public MainWindow() 
        { 
            InitializeComponent(); 
        }

        private void btnChange_Click_1(object sender, RoutedEventArgs e) 
        { 
            // örnek olarak computer nesne örneğinin ListPrice özelliğinin değeri arttırılır 
            computer.ListPrice += 10; 
            // computer nesne örneğinin ListPrice özelliğinin yeni değeri, yapılan değişiklik üzerine ilgili kontrolün text özelliğine YENİDEN atanır 
           txtListPrice.Text = computer.ListPrice.ToString(); 
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e) 
        { 
            // computer isminde Product tipine ait nesne örneklenir 
            computer = new Product 
            { 
                ProductId = 10934 
                , Name = "HP Compaq 1024X" 
               , ListPrice = 999 
            };

            // örneğe ait özelliklerin değerleri Window üzerindeki kontrollerin Text özelliklerine set edilir. 
            txtProductId.Text = computer.ProductId.ToString(); 
            txtName.Text = computer.Name; 
            txtListPrice.Text = computer.ListPrice.ToString(); 
        } 
    } 
}
```

Uygulamanın çalışma zamanına ait örnek bir ekran görüntüsü aşağıdaki gibidir. Düğmeye her basışta ListPrice değeri artacak ve sonuç ilgili TextBox kontrolü içerisine yazılacaktır.

![ub_2](/assets/images/2014/ub_2_thumb.png)

Çalışma zamanında üretilen Product nesne örneği değerleri, kontrollerin ilgili özelliklerine kod yardımıyla basitçe atanmaktadır. Aslında ortada bir sorun yoktur. Olmayacaktır da. Bu şekilde de programlamaya devam edilebilir. Ancak XAML tarafında getirilmiş olan yeni nesil veri bağlama opsiyonlarının bazı artıları vardır. Örneğin MVVM (Model-View-ViewModel) gibi desenlere kolayca enjekte olabilirler. Peki ya diğerleri ne olabilir?

En Basit Haliyle Binding

XAML tarafındaki veri bağlama opsiyonlarını en basit haliyle ele aldığımızda aşağıdaki yeni içeriği üretmemiz yeterlidir.

```csharp
<Window x:Class="HowTo_FundementalsOfBinding.MainWindow" 
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" 
        Title="MainWindow" Height="175" Width="400" Loaded="Window_Loaded_1"> 
    <Grid> 
        <Grid.ColumnDefinitions> 
            <ColumnDefinition Width="80"/> 
            <ColumnDefinition/> 
        </Grid.ColumnDefinitions> 
        <Grid.RowDefinitions> 
            <RowDefinition Height="25"/> 
            <RowDefinition Height="25"/> 
            <RowDefinition Height="25"/> 
            <RowDefinition Height="25"/> 
            <RowDefinition Height="25"/> 
            <RowDefinition Height="25"/> 
        </Grid.RowDefinitions> 
        <TextBlock Text="Product ID" Grid.Column="0" Grid.Row="0"/> 
        <TextBlock Text="Product Name" Grid.Column="0" Grid.Row="1"/> 
        <TextBlock Text="Product Price" Grid.Column="0" Grid.Row="2"/> 
        <TextBlock Grid.Column="1" Grid.Row="0" Text="{Binding Path=ProductId}"/> 
        <TextBox Grid.Column="1" Grid.Row="1" Margin="2,2,2,2" Text="{Binding Path=Name}"/> 
        <TextBox Grid.Column="1" Grid.Row="2" Margin="2,2,2,2" Text="{Binding Path=ListPrice}"/> 
        <Button Grid.Column="1" Grid.Row="3" Content="Change" Width="60" 
                HorizontalAlignment="Right" Margin="2,2,2,2" Click="btnChange_Click_1"                
                /> 
    </Grid> 
</Window>
```

Dikkat edileceği üzere x:Name nitelikleri kaldırılmıştır. Bu bir gereklilik değildir ama senaryomuzun bu kısmında söz konusu değerlere de ihtiyacımız yoktur. Nitekim kod tarafında bu bileşenler erişilmesine ihtiyaç bulunmamaktadır. Önemli olan ProductId, Name, ListPrice isimli Product nesne örneğine ait özellik değerlerinin ilgili kontrollere nasıl bağlandığıdır. Syntax oldukça basittir.

```csharp
Text = {Binding Path=Name}
```

Buna göre Text özelliğine X nesne örneğinin Name özelliğinin değeri basılacaktır.

X Nesne Örneğinin Kim Olduğunu Çalışma Zamanı Nasıl Bilebilir?

Kod bazında bu işlemi gerçekeştirmek için üst XAML kontrollerinden birisinin DataContext özelliğine bir Product nesne örneğini (computer isimli değişken) atamamız yeterlidir.

> Üst kontrolün set edilen DataContext özelliğinin işaret ettiği veri kümesi, alt kontroller tarafından da erişilir niteliktedir. Buna göre Grid veya StackPanel gibi bir container kontrolünün DataContext özelliğine veri içeren bir liste bağlanması, içeride yer alan alt bileşenlerin de (ve hatta onların altındakilerin) bu veri kümesi ile çalışabilmesi anlamına gelmektedir. Ve tüm bu bağlantı işlemleri XAML tarafında dekleratif olarak yapılabilmektedir.

```csharp
using System.Windows;

namespace HowTo_FundementalsOfBinding 
{ 
    public partial class MainWindow 
        : Window 
    { 
        Product computer = null;

        public MainWindow() 
        { 
            InitializeComponent(); 
        }

        private void btnChange_Click_1(object sender, RoutedEventArgs e) 
        { 
            // örnek olarak computer nesne örneğinin ListPrice özelliğinin değeri arttırılır 
            computer.ListPrice += 10; 
            // computer nesne örneğinin ListPrice özelliğinin yeni değeri, yapılan değişiklik üzerine ilgili kontrolün text özelliğine YENİDEN atanır 
            //txtListPrice.Text = computer.ListPrice.ToString(); 
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e) 
        { 
            // computer isminde Product tipine ait nesne örneklenir 
            computer = new Product 
            { 
                ProductId = 10934 
                , Name = "HP Compaq 1024X" 
                , ListPrice = 999 
            };

            // Tüm Window içeriğindeki XAML kontrollerini computer isimli Product nesne örneğine bağlamış oluyoruz. 
            this.DataContext = computer;

            // örneğe ait özelliklerin değerleri Window üzerindeki kontrollerin Text özelliklerine set edilir. 
            //txtProductId.Text = computer.ProductId.ToString(); 
            //txtName.Text = computer.Name; 
            //txtListPrice.Text = computer.ListPrice.ToString(); 
        } 
    } 
}
```

Görüldüğü üzere kontrollerin Text gibi alanlarına, computer isimli örneğe ait özellikler doğrudan kod yardımıyla bağlanılmamıştır. Sadece nesne örneğinin oluşturulması ve this ile Window'un DataContext özelliğine set edilmesi yeterli olmuştur.

[![ub_3](/assets/images/2014/ub_3_thumb.png)](/assets/images/2014/ub_3.png)

Özellik Değeri Değiştiğinde?

Bir önceki ekran görüntüsüne bakıldığında XAML tabanlı yapılan veri bağlama işleminin sorunsuz çalıştığı düşünülebilir. Ancak küçük bir problem vardır. Düğmeye basarak computer isimli değişkene ait ListPrice değerini arttırdığımızda TextBox kontrolü içerisindeki verinin güncellenmediğine şait oluruz.

[![ub_4](/assets/images/2014/ub_4_thumb.png)](/assets/images/2014/ub_4.png)

Demek ki TextBox bileşenini gerçek anlamda Product tipinden nesne örneğine bağlayabilmiş değiliz. Çözümsel yaklaşım olarak, Product nesne örneğinin ilgili özelliklerinde olabilecek değişiklikler sonucunda, ilgili görsel kontrollerin bir şekilde uyarılması ve içeriklerinin güncellenmesi gerektiği düşünülebilir. Yine klasik stilde olaya yaklaşırsak Product tipini şu hale getirmemiz işe yarayabilir.

```csharp
namespace HowTo_FundementalsOfBinding 
{ 
    public class Product 
    {       
        public int ProductId { get; set; }

        private string _name;

        public string Name 
        { 
            get 
            { 
                return _name; 
            } 
            set 
            { 
                _name = value; 
                txtName.Text = value; 
            } 
        } 
        public decimal ListPrice { get; set; } 
    } 
}
```

txtName Nereden Geliyor?

x:Name niteliklerini kaldırmıştık hatırlayacağınız gibi. Hadi bunu geçtik diyelim. Ya Product tipi bir Class Library içerisinde ise ve aslında başka projelerde de kullanılacaksa ve o projelerde TextBox kontrolleri yoksa!? Name değerinin bir Windows Phone uygulamasında TextBlock içerisine basılması söz konusu iken başka bir XAML bazlı uygulama da farklı bir kontrolde gösterilmesi istenirse...

Demek ki özelliklerde olan değişiklikleri, görsel ortama bildirirken kontrolden, adından, tipinden vs tamamen bağımsız olabilmeliyiz. İşte bu, INotifyPropertyChanged isimli arayüzün (Interface) neden var olduğunun açık bir ifadesidir. Dolayısıyla Product tipinin içeriğini şu hale getirdiğimizi düşünebiliriz.

[![ub_5](/assets/images/2014/ub_5_thumb.png)](/assets/images/2014/ub_5.png)

```csharp
using System.ComponentModel;

namespace HowTo_FundementalsOfBinding 
{ 
    public class Product 
        : INotifyPropertyChanged 
    { 
        private int _productId;

        public int ProductId 
        { 
            get { return _productId; } 
            set 
            { 
                if (_productId == value) 
                    return; 
                _productId = value; 
                // Dış dünyayı ProductId özelliğinin değerinin değiştiğine dair bilgilendir 
                OnPropertyChanged("ProductId"); 
            } 
        }

        private string _name; 
        public string Name 
        { 
            get { return _name; } 
            set 
            { 
                if (_name == value) 
                    return; 
                _name = value; 
                // Dış dünyayı Name özelliğinin değerinin değiştiğine dair bilgilendir 
               OnPropertyChanged("Name"); 
            } 
        }

        private decimal _listPrice; 
        public decimal ListPrice 
        { 
            get { return _listPrice; } 
            set 
            { 
                if (_listPrice == value) 
                    return; 
                _listPrice = value; 
                // Dış dünyayı ListPrice özelliğinin değerinin değiştiğine dair bilgilendir 
               OnPropertyChanged("ListPrice"); 
            } 
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) 
        { 
            // Özellik değiştiğine dair bir Event yüklendiyse bunu tetikle 
            if (PropertyChanged != null) 
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); 
        } 
    } 
}
```

Buna göre kod tarafında computer örneğine ait özelliklerde bir değişiklik olduğunda, Window üzerinde veriye bağlanmış olan kontrollerin ilgili içeriklerinin de değiştiği gözlemlenecektir. Diğer yandan tam tersi durumda söz konusudur. Yani kontrol üzerindeki değerlerde bir değişiklik yapıp, odağı farklı bir bileşene kaydırırsak (tab tuşuna basmamız dahi bunun için yeterli olacaktır), bu durumda kontrollere bağlanan nesne örneğinin özellikleri de otomatik olarak güncellenir. Aşağıdaki ekran görüntüsünde bu durum irdelenmiştir.

[![ub_6](/assets/images/2014/ub_6_thumb.png)](/assets/images/2014/ub_6.png)

Biraz Revizyon

Product tipine ait özelliklerin set bloklarında değişikliğe neden olan özellik adlarının, OnPropertyChanged metoduna string olarak geçirildiği dikkatinizden kaçmamıştır. Aslında biraz daha güvenli bir yol tercih edilebilir. Yani string tipte özellik adlarını vermekten kurtulabiliriz. Nasıl mı? İşte aşağıdaki kod parçasında görüldüğü gibi.

```csharp
using System; 
using System.ComponentModel; 
using System.Linq.Expressions;

namespace HowTo_FundementalsOfBinding 
{ 
    public class Product 
        : INotifyPropertyChanged 
    { 
        private int _productId;

        public int ProductId 
        { 
            get { return _productId; } 
            set 
            { 
                if (_productId == value) 
                    return; 
                _productId = value; 
                // Dış dünyayı ProductId özelliğinin değerinin değiştiğine dair bilgilendir 
                OnPropertyChanged(()=>ProductId); 
            } 
        }

        private string _name; 
        public string Name 
        { 
            get { return _name; } 
            set 
            { 
                if (_name == value) 
                    return; 
                _name = value; 
                // Dış dünyayı Name özelliğinin değerinin değiştiğine dair bilgilendir 
                OnPropertyChanged(()=>Name); 
            } 
        }

        private decimal _listPrice; 
        public decimal ListPrice 
        { 
            get { return _listPrice; } 
            set 
            { 
                if (_listPrice == value) 
                    return; 
                _listPrice = value; 
                // Dış dünyayı ListPrice özelliğinin değerinin değiştiğine dair bilgilendir 
                OnPropertyChanged(()=>ListPrice); 
            } 
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) 
        { 
            // Özellik değiştiğine dair bir Event yüklendiyse bunu tetikle 
            if (PropertyChanged != null) 
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); 
        }

        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> propertySelector) 
        { 
            var memberExpression = propertySelector.Body as MemberExpression; 
            if (memberExpression != null) 
           { 
                OnPropertyChanged(memberExpression.Member.Name); 
           } 
        } 
    } 
}
```

Dikkat edileceği üzere özelliklerin Set bloklarında yapılan OnPropertyChanged metod çağrılarında, lambda (=>) operatörü kullanılmış ve değer olarak string yerine tipe ait özellik adları gönderilmiştir (Bu noktada Development ortamında intelli-sense özelliğinin de çalıştığını ifade edebiliriz)

Size Düşen

Böylece geldik bir yazımızın daha sonuna. Bu yazımızda veri bağlama (Data Binding) operasyonlarının bir kaç temel noktasını daha iyi bir şekilde anlamaya çalıştık. Size tavsiyem XAML tarafındaki Binding ifadelerinde kullanılabilen diğer özelliklerin ne anlama geldiklerini ve hangi amaçla kullanıldıklarını öğrenmeye çalışmanız olacaktır. Söz gelimi Mode niteliğine OneWay değerinin atanması ile TwoWay verilmesi arasındaki fark nedir? Ya da OneWayToSource ne işe yaramaktadır. Hatta orada Converter’ lar ile ilişkili bir durum da söz konusudur. Converter’ lara neden ihtiyaç duyarız ki?

Bir sonraki yazımızda görüşünceye dek hepinize mutlu günler dilerim.

H[owTo_FundementalsOfBinding.zip (77,96 kb)](/assets/files/2014/HowTo_FundementalsOfBinding.zip)