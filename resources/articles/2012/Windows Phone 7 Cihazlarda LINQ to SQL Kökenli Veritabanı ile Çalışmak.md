---
title: "Windows Phone 7 Cihazlarda LINQ to SQL Kökenli Veritabanı ile Çalışmak"
pubDate: 2012-10-10 21:00:00
categories:
  - Windows Phone 7
tags:
  - Windows Phone 7
  - C#
  - XML
  - .NET
  - Entity Framework
  - LINQ
  - NoSQL
  - Silverlight
  - XAML
  - HTTP
  - Java
  - Windows Phone
  - Visual Studio
---

# Windows Phone 7 Cihazlarda LINQ to SQL Kökenli Veritabanı ile Çalışmak
[![HP-iPAQ-110-classic-handheld](images/HP-iPAQ-110-classic-handheld_thumb.jpg)](images/HP-iPAQ-110-classic-handheld.jpg) Merhaba Arkadaşlar,

Uzun zamandır bilgisayar yazılım teknolojileri ile ilgileniyor olmama rağmen zaman içerisinde belirli konularda uzmanlaşmaya çalıştığımı fark ettim. Bana göre normalde olması gereken bu. Nitekim insanın kapasitesini bilmesi ve her şeyden çok fazla anlamamaktansa, belirli bir konuda çok iyi bilgiye sahip olması daha anlamlıdır diye düşünüyorum

![Smile](images/wlEmoticon-smile_48.png)

Ama tabi zaman zaman uzmanlık alanım dışındaki konulara da merak salmıyor değilim. Örneğin mobil platform üzerine geliştirme yapmak gibi. Her ne kadar Microsoft bu konuda elinden geleni yapıp işi son yıllarda daha da kolaylaştırıp Windows Phone gibi güzel bir zemin hazırlamış olsa da çok nadiren o tarafa gidip geliyorum.

Geçtiğimiz günlerde Feedreader üzerinden blogları şöyle bir tararken Windows Phone üzerinde kullanılabilecek olan veri depolama seçenekleri ile ilişkili kısa bir nota rast geldim. Özellikle Isolated Storage tabanlı depolamalar üzerinde durulmaktaydı. Derken kendimi konuyu araştırır halde buldum. İşte bu yazının amacı elde edilen sonuçlar ve hoşunuza gidecek (hoşuma gidecek) bir örneği kaleme almak

![Winking smile](images/wlEmoticon-winkingsmile_116.png)

> Yıllar yıllar önce değil ama 2006 yılında Netron'da ilk Freelance eğitimimi bir ilaç firması (Boehringer Ingelheim) için vermiş ve Windows Mobile 6.5 üzerinde yazılım geliştirme anlatmıştım.
> Compact.Net Framework ile ilişkili örnekleri ve konuları firmanın sağladığı HP marka akıllı telefonlarda ele almıştık. Styles Pen'ler ile çalışan ve kapasite olarak (bellek, işlemci hızı, ekran çözünürlüğü vb) sınırlı cihazlarda. Kim bilebilirdi ki iş bu noktaya kadar gelecek.
> iPhone'lar, Blackberry'ler, Samsung Galaxy'ler ve tabi Windows Phone'lar. O zamanlarda Java tabanlı akıllı telefonlar yine.Net Compact Framework'lü olanlara göre çok daha iyiydi. Lakin bir süredir Windows Phone tarafının çok daha önemli bir atılım yaptığını ve arayı hızla kapattığını görüyoruz. En azından teknoloji ve yazılım geliştirme yetenekleri açısından.

Veri depolama sistemleri, mobil sistemlerdeki en önemli sıkıntılardan birisi olarak da karşımıza çıkıyor. Her ne kadar günümüz cihazlarında depolama alanlarının boyutu GB'lar cinsinden ifade edilebiliyor ve haricen kolayca genişleyebiliyor olsa da, ortada senkronizasyon gibi sıkıntılı iş senaryoları da bulunmakta. Yine de saha da çalışanların offline veri depolama kabiliyetleri ile çalışabilmesi önemli. Bilindiği üzere Silverlight ile gelen Isolated Storage kavramı mobil taraf için de geçerli. Bu alan içerisinde veriyi saklamak için çeşitli yollara başvurabiliriz.

Örneğin veriyi text tabanlı olarak saklayabiliriz ve hatta bu sebepten NoSQL (Not Only SQL) gibi sistemleri de ele almayı düşünebiliriz. Diğer yandan bazı 3ncü parti kütüphanelerden veya SQL'in mobile taraf için kullanılabilecek sürümlerinden de yararlanabiliriz[(Bu adreste konu ile ilişkili detaylı bilgiye ulaşabilirsiniz)](http://www.windowsphonegeek.com/tips/All-about-WP7-Isolated-Storage---Open-Source-Databases-and-Helper-Libraries)

Biz bu günkü örneğimizde ise LINQ to SQL ve Code First benzeri (benzeri diyorum çünkü tam olarak POCO tipleri söz konusu değil. Attribute'lar ile bezenmiş bir sınıf söz konusu olacak) bir yaklaşımı ele alıyor olacağız. Dilerseniz hiç vakit kaybetmeden örneğimizi adım adım geliştirmeye başlayalım.

İlk olarak Silverlight for Windows Phone şablonunu kullanarak ([Windows Phone SDK'](http://www.microsoft.com/en-us/download/details.aspx?id=27570) yı yüklemiş olduğunuzu varsayıyorum) MobileCustomerHouse isimli bir uygulama oluşturalım.

[![ltswp_1](images/ltswp_1_thumb.png)](images/ltswp_1.png)

Uygulamamızı 7.1 versiyonlu işletim sistemi sürümü için geliştiriyor olacağız.

[![ltswp_2](images/ltswp_2_thumb.png)](images/ltswp_2.png)

Uygulamamızda LINQ to SQL tabanlı bir yapı kullanacağız. Bu sebepten System.Data.Linq assembly'ını projemize referans etmemiz gerekiyor. Bu işlemin ardından uygulamamıza aşağıda sınıf çizelgesi ve kod içeriği görülen Customer sınıfını ekleyebiliriz.

[![ltswp_3](images/ltswp_3_thumb.png)](images/ltswp_3.png)

```csharp
using System.ComponentModel; 
using System.Data.Linq.Mapping;

namespace MobileCustomerHouse 
{ 
    [Table(Name="Customer")] 
    public class Customer 
        :INotifyPropertyChanged,INotifyPropertyChanging 
    { 
        public event PropertyChangedEventHandler PropertyChanged; 
        public event PropertyChangingEventHandler PropertyChanging;

        private void NotifyPropertyChanged(string propertyName) 
        { 
            if (PropertyChanged != null) 
            { 
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); 
            } 
        }

        private void NotifyPropertyChanging(string propertyName) 
        { 
            if (PropertyChanging != null) 
            { 
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName)); 
            } 
        }

        private int _customerId; 
        private string _name; 
        private string _surname; 
        private decimal _salary;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)] 
        public int CustomerId 
        { 
            get { return _customerId; } 
            set 
            { 
                NotifyPropertyChanging("CustomerId"); 
                _customerId = value; 
                NotifyPropertyChanged("CustomerId"); 
            } 
        }

       [Column(DbType = "nvarchar(25) NOT NULL", CanBeNull = false, AutoSync = AutoSync.OnInsert,Name="CustomerName")] 
        public string Name 
        { 
            get { return _name; } 
            set 
            { 
                NotifyPropertyChanging("Name"); 
                _name = value; 
                NotifyPropertyChanged("Name"); 
            } 
        }

        [Column(DbType = "nvarchar(25) NOT NULL", CanBeNull = false, AutoSync = AutoSync.OnInsert,Name="CustomerSurname")] 
        public string Surname 
        { 
            get { return _surname; } 
            set 
            { 
                NotifyPropertyChanging("Surname"); 
                _surname = value; 
                NotifyPropertyChanged("Surname"); 
            } 
        }

        [Column(DbType = "money NOT NULL", CanBeNull = false, AutoSync = AutoSync.OnInsert,Name="CustomerSalary")] 
        public decimal Salary 
        { 
            get { return _salary; } 
            set 
            { 
                NotifyPropertyChanging("Salary"); 
                _salary = value; 
                NotifyPropertyChanged("Salary"); 
            } 
        } 
    } 
}
```

> Aslında örneğimizde View Model tarzı bir yaklaşımda bulunmayacağım. Bu sebepten Customer tipine INotifyPropertyChanged ve INotifyPropertyChanging arayüzlerini implemente etmesek de örneğimiz işlevsel olacaktır.
> Ancak doğru olan, söz konusu tipin bir View ile birlikte ViewModel deseni içerisinde kullanılma ihtimalinin de olacağını göz önünde bulundurmaktır.
> Siz, Model View View Model (MVVM) veya Model View Control (MVC) gibi desenleri göz önüne alarak bu yönde bir geliştirme yapmayı düşünebilirsiniz.

Customer tipi içerisinde dikkat edileceği üzere CustomerId,Name,Surname ve Salary özelliklerine ait Set bloklarına uygulanmış olan Notify çağrımları söz konusudur. Customer tipine INotifyPropertyChanging ve INotifyPropertyChanged arayüzlerini uyarladığımızdan, modelin görsel bir component ile bağlanması ve özelliklerde yapılan değişikliklerde View tarafının uyarılması/tepki verebilecek olması sağlanmaktadır.

Customer tipinin diğer önemli özelliği ise tipin Table ve içeride yer alan özelliklerin de Column nitelikleri ile (System.Data.Linq.Mapping isim alanından geliyorlar) dekore edilmiş olmalarıdır. Bu nitelikler (Attribute) ile tahmin edeceğiniz üzere Customer tipine ait tablo şemasının içeriği belirlenmektedir. Örneğin CustomerId, tablo tarafında otomatik olarak artan integer tipinde bir alandır. Ayrıca Primary Key olarak tanımlanmıştır. Name ve Surname alanları nvarchar tipindeyken, Salary alanı Money tipindedir.

Entity Framework ve LINQ to SQL den bildiğimiz üzere aslında Entity tiplerine ait koleksiyon bazlı özelliklerin tutulduğu ayrı bir Context sınıfı söz konusudur. Code First yaklaşımında DbContext türevli, klasik Entity Framework yaklaşımında ise ObjectContext türevli olan bu yapı LINQ to SQL tarafını göz önüne aldığımızda ise DataContex'den türemek anlamına gelmektedir. İşte biz de örneğimizde asıl veritabanını map eden bir Context tipinden yararlanıyor olacağız. Aşağıda ki sınıf çizelgesi ve kod parçasında görüldüğü gibi.

```csharp
using System.Data.Linq;

namespace MobileCustomerHouse 
{ 
    public class House 
        :DataContext 
    { 
            public House(string connectionString) 
                : base(connectionString) 
            { 
                Customers = this.GetTable<Customer>(); 
            } 
        public Table<Customer> Customers; 
    } 
}
```

> Örneğin basit tutulması amacıyla sadece Customer tipi ve buna ait veri kümesini işaret eden Customers isimli Table sınıfından bir özellik kullanılmıştır. Size tavsiyem, örneği ilişkisel (Relational) bir/bir kaç tabloyu daha işin içerisine katarak ilerlemeye çalışmanız olacaktır. Örneğin müşterilerin hesap bilgilerini tutabileceğiniz ve One-to-Many ilişkiyi ifade eden bir geliştirmeyi nasıl yapacağınızı düşünebilirsiniz.

Peki veritabanımız ne zaman üretilecek? Arayüzümüz nasıl olacak? Dilerseniz uygulamamıza ait XAML içeriğini ve tasarımını aşağıdaki gibi geliştirererk ilerlemeye devam edelim.

MainPage.XAML

[![ltswp_6](images/ltswp_6_thumb.png)](images/ltswp_6.png)

```xml
<phone:PhoneApplicationPage 
    x:Class="MobileCustomerHouse.MainPage" 
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" 
    xmlns:phone="clr-namespace:Microsoft.Phone.Controls;assembly=Microsoft.Phone" 
    xmlns:shell="clr-namespace:Microsoft.Phone.Shell;assembly=Microsoft.Phone" 
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008" 
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" 
    mc:Ignorable="d" d:DesignWidth="480" d:DesignHeight="768" 
    FontFamily="{StaticResource PhoneFontFamilyNormal}" 
    FontSize="{StaticResource PhoneFontSizeNormal}" 
    Foreground="{StaticResource PhoneForegroundBrush}" 
    SupportedOrientations="Portrait" Orientation="Portrait" 
    shell:SystemTray.IsVisible="True">

    <!--LayoutRoot is the root grid where all page content is placed--> 
    <Grid x:Name="LayoutRoot" Background="Transparent"> 
        <Grid.RowDefinitions> 
            <RowDefinition Height="Auto"/> 
            <RowDefinition Height="*"/> 
        </Grid.RowDefinitions>

        <!--TitlePanel contains the name of the application and page title--> 
        <StackPanel x:Name="TitlePanel" Grid.Row="0" Margin="12,17,0,28"> 
            <TextBlock x:Name="ApplicationTitle" Text="Customer House" Style="{StaticResource PhoneTextNormalStyle}"/> 
            <TextBlock x:Name="PageTitle" Text="Customer" Margin="9,-7,0,0" Style="{StaticResource PhoneTextTitle1Style}"/> 
        </StackPanel>
        <!--ContentPanel - place additional content here--> 
        <Grid x:Name="ContentPanel" Grid.Row="1" Margin="12,0,12,0"></Grid> 
        <StackPanel Height="607" HorizontalAlignment="Left" Margin="12,0,0,0" Name="stackPanel1" VerticalAlignment="Top" Width="456" Grid.Row="1"> 
            <TextBlock Height="30" Name="textBlock1" Text="Name" /> 
            <TextBox Height="71" Name="txtCustomerName" Text="customer name" Width="460" /> 
            <TextBlock Height="30" Name="textBlock2" Text="Surname" /> 
            <TextBox Height="71" Name="txtSurname" Text="customer surname" Width="460" /> 
            <TextBlock Height="30" Name="textBlock3" Text="Salary" /> 
            <TextBox Height="71" Name="txtCustomerSalary" Text="1000" Width="460" /> 
            <Button Content="Insert" Height="71" Name="btnInsert" Width="160" HorizontalContentAlignment="Center" Click="btnInsert_Click" /> 
            <ListBox Height="226" Name="lstCustomers" Width="460" ItemsSource="{Binding}"> 
                <ListBox.ItemTemplate> 
                    <DataTemplate> 
                        <StackPanel Orientation="Horizontal"> 
                            <TextBlock Foreground="Gold" Margin="2,2,2,2" Text="{Binding Name}"/> 
                            <TextBlock Foreground="Gold" Margin="2,2,2,2" Text="{Binding Surname}"/> 
                            <TextBlock Foreground="Red" Margin="2,2,2,2" Text="{Binding Salary}"/> 
                        </StackPanel> 
                    </DataTemplate> 
                </ListBox.ItemTemplate> 
            </ListBox> 
        </StackPanel> 
    </Grid>
</phone:PhoneApplicationPage>
```

Görsel arabirim de çok özel bir şey yok aslına bakarsanız. Dikkate değer nokta ListBox kontrolünün bir DataTemplate ile ilişkilendirilmiş olmasıdır. ListBox kontrolü, Context nesnesinin ilgili özelliğine bağlandıktan sonra (Binding), içeride yer alan TextBlock bileşenleri de sırasıyla Name, Surname ve Salary özelliklerine bağlanmıştır (CustomerId’ yi unutmuşum onu da siz ekleyiverin ![Smile](images/wlEmoticon-smile_48.png)) Şimdi arka plan kodlarını yazarak örneğimizi genişletmeye devam edebiliriz.

```csharp
using System; 
using System.Linq; 
using Microsoft.Phone.Controls;

namespace MobileCustomerHouse 
{ 
    public partial class MainPage 
        : PhoneApplicationPage 
    { 
        House houseDb = null;

        public MainPage() 
        { 
            InitializeComponent();

            houseDb = new House("Data Source = 'isostore:/House.sdf'; File Mode = read write");

           if (!houseDb.DatabaseExists()) 
            { 
                houseDb.CreateDatabase(); 
            } 
        }

        private void btnInsert_Click(object sender, System.Windows.RoutedEventArgs e) 
        { 
            Customer newCustomer = new Customer 
           { 
                 Name=txtCustomerName.Text, 
                 Surname=txtSurname.Text, 
                Salary=Convert.ToDecimal(txtCustomerSalary.Text) 
            };

            houseDb.Customers.InsertOnSubmit(newCustomer); 
            houseDb.SubmitChanges();

           lstCustomers.ItemsSource = houseDb.Customers.ToList(); 
        } 
    } 
}
```

Yapıcı metod içerisinde House tipinden bir nesne örneklendiği görülmektedir. Bu nesne verdiğimiz Connection String bilgisine göre Isolated Storage üzerinde House.sdf isimli bir dosya oluşturulacaktır. Dikkat edilecek olursa DatabaseExists () metodu ile veritabanının önceden yaratılıp yaratılmadığı kontrol edilmektedir.

> Eğer veritabanının şema (Schema) yapısında kod tarafında değişiklik yapılması planlanıyorsa Microsoft.Phone.Data.Linq isim alanında bulunda DatabaseSchemaUpdater tipi ve üyelerinden yararlanılabilir.

Button kontrolünde basıldığın TextBox kontrollerindeki verilerden yararlanılarak bir Customer nesnesini örneklenmekte ve houseDb isimli veritabanı içerisine ilave edilkmektedir. Nesnenin önce Context’ eklenmesi için InsertOnSubmit metodundan faydalanılmış ve değişikliklerin veritabanı üzerine yazılması için de SubmitChanges fonksiyonu çağırılmıştır.

Ekleme işlemi tamamlandıktan sonra ise ListBox kontrolünün ItemsSource özelliğine gerekli veri bağlama (Data Binding) işlemi yapılmaktadır. Örneği test ettiğimizde ve bir kaç örnek veri içeriğini girdiğimizde aşağıdaki ekran görüntüsündekine benzer bir çalışma zamanı sonucu ile karşılaşırız.

[![ltswp_5](images/ltswp_5_thumb.png)](images/ltswp_5.png)

Görüldüğü üzere örnek olarak eklenen Customer tipleri ListBox kontrolü içerisine basılmıştır.

Peki gerçekten de Isolated Storage alanı içerisinde sdf veritabanı oluşturulmakta mıdır? Örneğin başarılı bir şekilde çalışması nedeni ile bunun doğru olduğu görülmektedir ama biz yine de bakmak istediğimizi farz edelim

![Winking smile](images/wlEmoticon-winkingsmile_116.png)

Bunu görmek için Isolated Storage içeriğine kod yardımıyla da bakabiliriz. Ama benim önerim Codeplex sitesinde yayınlanan ([http://wp7explorer.codeplex.com/)](http://wp7explorer.codeplex.com/)) Windows Phone 7 Isolated Storage Explorer'ın kullanılması olacak. Bu ürünü kurduktan sonra Visual Studio'nun View menüsüne Isolated Storage Explorer penceresinin ilave edildiği gözlemlenebilir (View->Other Windows->WP7 Isolated Storage Explorer). Tabi buraya çalışmakta olan bir Emulator’ ü ttach’ lamak için uygulamamıza IsolatedStorageExplorer assembly'ını referans etmemiz ve Appl.xaml.cs içerisinde aşağıda görülen kod değişikliklerini yapmamız gerekmektedir.

```csharp
private void Application_Activated(object sender, ActivatedEventArgs e) 
{ 
    IsolatedStorageExplorer.Explorer.RestoreFromTombstone(); 
}
  
private void Application_Deactivated(object sender, DeactivatedEventArgs e) 
{ 
    IsolatedStorageExplorer.Explorer.RestoreFromTombstone(); 
}
```

Uygulamamızı yeniden çalıştırdığımızda ve Isolated Storage Explorer penceresini açtığımızda House.sdf isimli veritabanı dosyasının başarılı bir şekilde üretildiğini ve hatta istenirse Download edilebileceğini görürüz.

[![ltswp_7](images/ltswp_7_thumb.png)](images/ltswp_7.png)

[Bu arada söz konusu ürün makaleyi yazdığım tarih itibariyle Beta sürümündeydi. Dolayısıyla güncellenmiş ve farklı kabiliyetler ile donatılmış olabilir]

Böylece geldik kısa bir maceramızın daha sonuna. Görüldüğü üzere LINQ to SQL’ i çok basit anlamda ele alarak özel depolama alanında bir veritabanının tutulmasını sağlayabildik. Yukarıda belirtiğim gibi aslında örneği MVVM veya MVC çerçvesinde göz önüne alarak arayüz ile daha güçlü entegre olacak şekilde geliştirmeye çalışmanızı öneririm. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[MobileCustomerHouse.zip (133,80 kb)](assets/MobileCustomerHouse.zip)
