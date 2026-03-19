---
layout: post
title: "Ado.Net Data Services 1.5 CTP2 - Data Binding Bölüm 2"
date: 2009-11-15 23:09:00 +0300
description: "Hatırlayacağınız gibi bir önceki yazımızda, Ado.Net Data Service için istemci taraflı veri bağlama işlemlerinde DataServiceCollection<T> kolekisyonunu değerlendirmeye çalışmış ve istemci tarafında bu konuyu ele almak için basit bir WPF uygulaması geliştirmiştik. Bir önceki örneğimiz aslında tek yönlü veri bağlama işlemine örnek olmasında rağmen, iki yönlü modeli de desteklemektedir."
categories:
  - ado-net-data-services
tags:
  - ado-net-data-services
  - csharp
  - xml
  - dotnet
  - ado-net
  - entity-framework
  - linq
  - sql-server
  - wcf
  - wpf
  - xaml
  - http
  - visual-studio
---
# Ado.Net Data Services 1.5 CTP2 - Data Binding Bölüm 2
![blg83_Giris.jpg](/assets/images/2009/blg83_Giris.jpg)

Merhaba Arkadaşlar,

Orta uzunlukta bir yazı için hazır mısınız? Analizi dikkat gerektiren bir Ado.Net Data Service örneği geliştiriyor olacağız. Genellikle bu tarz yazılara ait kodları geliştirirken sıkılmamak ve zihnimi açık tutmak için ya kahve içerim yada mutluluktan havalara uçarmış gibi yazabilmek ve kan şekerimi üst seviyede tutabilmek için bazen değişik şekerlemelerden yerim. Aynen yandaki resimde olduğu gibi.

![Cool](/assets/images/2009/smiley-cool.gif)

Hatırlayacağınız gibi bir önceki yazımızda, Ado.Net Data Service için istemci taraflı veri bağlama işlemlerinde DataServiceCollection kolekisyonunu değerlendirmeye çalışmış ve istemci tarafında bu konuyu ele almak için basit bir WPF uygulaması geliştirmiştik. Bir önceki örneğimiz aslında tek yönlü veri bağlama işlemine örnek olmasında rağmen, iki yönlü modeli de desteklemektedir. Bu yazımızda geliştireceğimiz örneğimizdeki hedefimiz ise, istemci tarafındaki koleksiyonlar üzerinden yapmış olduğumuz güncelleştirme, ekleme ve silme işlemlerini servis tarafına yansıtmaktır.

Aslında süreç son derece basittir. Veri bağlı kontroller üzerinde yapılan güncelleştirme hareketleri, DataServiceCollection koleksiyonu üzerinde de gerçeklenecektir. Sonrasında ise DataServiceContext türevli nesne örneği üzerinden SaveChanges metodunun çağırılması yeterli olacaktır. Bu sayede koleksiyon üzerinden yapılan tüm güncelleştirme, ekleme ve silme işlemlerinin, servis tarafına bir talep olarak gönderilmesi ve sunucu üzerinde de kullanılan veri kaynağına göre (Entity Framework veya Custom LINQ Provider) uygun bir veri işleminin yapılması sağlanır. Biz örneğimizde kendi geliştireceğimiz bir veritabanı içeriğini ve Ado.Net Entity Framework modelini kullanacağımızdan, sunucu üzerinde SQL sorgularının çalıştırıldığını göreceğiz. Dilerseniz vakit kaybetmeden örneğimizi geliştirmeye başlayalım ve işleyişini analiz edelim.

Örneğimizde kendi geliştirdiğimiz BookShop isimli bir veritabanını kullanıyor olacağız. Hayali olarak bir kitapçının veri ambarı olarak tasarladığımızı farz edebiliriz.

![Wink](/assets/images/2009/smiley-wink.gif)

Söz konusu veritabanını oluşturmak için [BookShopDbScripts.sql (13,83 kb)](/assets/files/2009/BookShopDbScripts.sql) dosyasından yararlanabilirsiniz. Bu dosya içerisinde veritabanı, tabloların oluşturulması ve örnek veri girişleri için gerekli SQL Script'leri bulunmaktadır. Bu noktadan yola çıkarak geliştireceğimiz Ado.Net Data Service örneğinde kullanacağımız Entity DataModel diagramını aşağıdaki gibi tasarlayabiliriz. Örneğimizdeki amacımız kitap güncellemek, kitap eklemek ve silmek gibi işlemler olacaktır.

![blg83_Edmx.gif](/assets/images/2009/blg83_Edmx.gif)

Gelelim Ado.Net Data Service örneğimize. Version 1.5 CTP2 versiyonuna göre eklediğimiz servisin kod içeriğini aşağıdaki gibi tasarlayabiliriz.

```csharp
using System.Data.Services;

namespace BookShop
{
    public class BookService 
        : DataService<BookShopEntities>
    {
        public static void InitializeService(DataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("*", EntitySetRights.All);        
            config.DataServiceBehavior.MaxProtocolVersion = System.Data.Services.Common.DataServiceProtocolVersion.V2;
     }
    }
}
```

İstemci tarafını yine bir WPF uygulaması olarak tasarlayıp, iki yönlü veri bağlama kabiliyetlerini etkin bir şekilde kullanmayı planlıyoruz. Yazıyı hazırladığım sıralarda, Visual Studio 2008 üzerinde garip ve gizemli bir sorunla karşılaştım. Öyleki, istemci uygulamada Add Service Reference ile proxy üretimi yapılmasına rağmen, indirilen Entity tiplerinin INotifyPropertyChanged arayüzünü uygulamadıklarını gördüm. Ama tabiki çaresiz değildim. DataSvcUtil aracının version 1.5 CTP 2 sürümünü aşağıda görüldüğü gibi kullanarak, istemci için gerekli olan ve INotifyPropertyChanged arayüzünü implemente etmiş tipleri üretebiliriz.

![blog83_CommandPrompt.gif](/assets/images/2009/blog83_CommandPrompt.gif)

Bu noktadan sonra üretilen sınıfı istemci tarafında kullanmamız yeterli olacaktır. İşte istemci tarafı için üretilen Entity tipleri.

![blg83_ClassDiagram.gif](/assets/images/2009/blg83_ClassDiagram.gif)

Yanlız komut satırından yapılan proxy sınıfını istemcide kullanabilmek için, Ado.Net Data Services Version 1.5 CTP2 ile gelen Microsoft.Data.Services.Client assembly'ının projeye referans edilmesi gerekmektedir. Aynen aşağıdaki ekran görüntüsünde olduğu gibi.

![blg83_ClientSolution.gif](/assets/images/2009/blg83_ClientSolution.gif)

Bu ön hazırlıklardan sonra istemci tarafındaki Window1 içeriğini başlangıçta aşağıdaki gibi tasarladığımızı düşünelim.

![blg83_Window1First.gif](/assets/images/2009/blg83_Window1First.gif)

Burada üst tarafta yer alan ComboBox içerisinde kitap kategorileri, alt tarafta yer alan ListBox kontrolünde ise seçilen kategoriye bağlı kitaplar görüntülenecektir. Yanlız XAML tarafına baktığınızda WPF açısından etkili bazı tekniklerin kullanıldığını görebilirsiniz.

XAML İçeriği;

```xml
<Window x:Class="BookSeller.Window1"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    Title="Book Seller" Height="330" Width="384">
    <Grid Name="grdBook" Height="298">
        <ComboBox Height="42" Margin="2,12,0,0" Name="cmbCategories" VerticalAlignment="Top"  IsSynchronizedWithCurrentItem="true" ItemsSource="{Binding}">
            <ComboBox.ItemTemplate>
                <DataTemplate>
                    <StackPanel Orientation="Horizontal">
                        <TextBlock Name="txtCategoryId" Text="{Binding Path=CategoryId}" FontSize="22" Foreground="RosyBrown"/>
                        <TextBlock Name="txtCategoryName" Text="{Binding Path=Name}"/>
                    </StackPanel>
                </DataTemplate>
            </ComboBox.ItemTemplate>
        </ComboBox>
        <ListBox IsSynchronizedWithCurrentItem="True" Name="lstProducts" Margin="0,60,0,53" ItemsSource="{Binding Book}">
            <ListBox.ItemTemplate>
                <DataTemplate>
                    <StackPanel Orientation="Vertical">
                        <TextBox Name="txtProductName" Text="{Binding Name}" Width="250"/>
                        <TextBox Name="txtListPrice" Text="{Binding ListPrice}" Width="100" Foreground="Gold" Background="Black" HorizontalAlignment="Right"/>
                    </StackPanel>                    
                </DataTemplate>
            </ListBox.ItemTemplate>
        </ListBox>
        <Button Height="23" HorizontalAlignment="Left" Margin="2,0,0,24" Name="btnSaveChanges" VerticalAlignment="Bottom" Width="93" Click="btnSaveChanges_Click">Save Changes</Button>
    </Grid>
</Window>
```

Her şeyden önce, ComboBox içeriğinin her bir öğesinin birer StackPanel olduğunu ve CategoryId ile Name alanlarının içeriklerinin TextBlock'ların Text özelliklerine bağlandıklarını görebiliriz. Benzer durum ListBox kontrolü içinde geçerlidir. Ne varki ListBox kontrolünün içerisinde yer alan ve Text özellikleri Book Entity'sinin Name ile ListPrice alanlarına bağlanmış olan TextBox kontrolleri, aslında kullanıcı tarafından düzenlenebilirde.

![Laughing](/assets/images/2009/smiley-laughing.gif)

Volaaaa!!! Öyleyse akla şu gelebilir.

"Eğer bu kontrollerin Text özellikleri, kod tarafındaki Entity örneklerine bağlanmışlarsa ve çalışma zamanında içerikleri değiştirilirse bu düzenlemeler Entity içeriklerine de yansır mı? Peki diyelim ki yansıdı. SaveChanges metodunu çağırdığımızda bu değişikliler servis tarafına da yansır mı?"

Aslında bu sorularının tamamının cevabı Evet'tir. Ama tabiki bizim bu durumu analiz etmemiz ve gözümüzle görmemiz şart.

![Wink](/assets/images/2009/smiley-wink.gif)

Buna göre kod içeriğimizi aşağıdaki gibi geliştirmemiz yeterlidir.

```csharp
using System;
using System.Data.Services.Client;
using System.Windows;
using BookShopModel;

namespace BookSeller
{
    public partial class Window1 
           : Window
    {
        BookShopEntities bs = null;
        DataServiceCollection<Category> _bookShopCollection = null;

        public Window1()
        {   
            InitializeComponent();

            bs = new BookShopEntities(new Uri("http://localhost:7995/BookService.svc/"));
            _bookShopCollection = DataServiceCollection.CreateTracked<Category>(bs, bs.Category.Expand("Book"));
            grdBook.DataContext = _bookShopCollection;
        }

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            bs.SaveChanges();
        }
    }
}
```

Programı ilk çalıştırdığımızda aşağıdaki bilgilerin geldiğini görebiliriz. Her ne kadar tasarım konusunda zayıf bir örnek olsada, ListBox içerisinde her bir satırda düzenlenebilir, veriye bağlanmış kontrollerin yer alması dahi benim için önemli bir adımdır.

![Smile](/assets/images/2009/smiley-smile.gif)

![blg83_FirstRun.gif](/assets/images/2009/blg83_FirstRun.gif)

Şimdi ListBox içerisindeki verilerde değişiklik yapıldığını varsayalım. Örnek olarak Pro WCF 3.5 isimli kitabın adına Second Edition kelimelerini ilave ettiğimizi ve ListPrice değerini 39 birim olarak değiştirdiğimizi düşünebiliriz. Aynen aşağıdaki ekran görüntüsünde olduğu gibi.

![blg83_Change.gif](/assets/images/2009/blg83_Change.gif)

Şimdi Save Changes başlıklı düğmeye basalım ve BookShopEntities nesne örneği üzerinden SaveChanges metodunun çağırıldığı yerde koyduğumuz break point üzerinde duralım. Yapamamız gereken, BookShopEntities ve DataServiceCollection içeriklerini Watch ile incelemek olacaktır. İlk olarak bookShopCollection içeriğine bir bakalım.

![blg83_Breakpoint1.gif](/assets/images/2009/blg83_Breakpoint1.gif)

Hımmmmm...

![Wink](/assets/images/2009/smiley-wink.gif)

Görüldüğü üzere Category üzerinden gittiğimiz Book özelliğine bağlı koleksiyonda az önce yapılan Name ve ListPrice değişikliklerin gerçekleştirildiği gözlemlenmektedir. Benzer şekilde BookShopEntities nesne örneğinin içeriğine baktığımızda, ilgili Book örneği için aynı değişikliklerin yansıtıldığını görebiliriz.

![blg83_Breakpoint2.gif](/assets/images/2009/blg83_Breakpoint2.gif)

Dolayısıyla iki yönlü veri bağlama (Two Way DataBinding) nedeniyle kontroller üzerine yansıyan verilerde yapılan değişiklikler, istemci tarafındaki ilgili koleksiyonlara da yansıtılmaktadır. Buna göre SaveChanges metoduna yapılan çağrı geçildiğinde, servis tarafına gerekli güncelleştirme talebinin gittiği ve aşağıdaki SQL sorgusunun çalıştırıldığı gözlemlenir.

```text
exec sp_executesql N'update [dbo].[Book]
set [Name] = @0, [ListPrice] = @1, [PageSize] = @2
where ([BookId] = @3)
',N'@0 nvarchar(26),@1 decimal(19,4),@2 smallint,@3 int',@0=N'Pro WCF 3.5 Second Edition',@1=39.0000,@2=500,@3=2
```

Eğer birden fazla Book örneğinin özelliği değiştirilirse, SaveChanges metodu çağrısı sonrasında her bir güncelleştirme için ayrı bir SQL Update sorgusunun çalıştırıldığı da gözlemlenecektir. Şimdi örnek bir veri ekleme işlemi yapalım. Bu amaçla Window1 içeriğini aşağıdaki gibi değiştirdiğimizi düşünebiliriz.

![blg83_SecondDesign.gif](/assets/images/2009/blg83_SecondDesign.gif)

Yapılan bu değişikliklerden sonra ise XAML içeriğinin aşağıdaki gibi oluştuğu gözlemlenebilir.

```xml
<Window x:Class="BookSeller.Window1"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    Title="Book Seller" Height="362" Width="384">
    <Grid Name="grdBook" Height="317">
        <ComboBox Height="42" Margin="2,12,0,0" Name="cmbCategories" VerticalAlignment="Top"  IsSynchronizedWithCurrentItem="true" ItemsSource="{Binding}">
            <ComboBox.ItemTemplate>
                <DataTemplate>
                    <StackPanel Orientation="Horizontal">
                        <TextBlock Name="txtCategoryId" Text="{Binding Path=CategoryId}" FontSize="22" Foreground="RosyBrown"/>
                        <TextBlock Name="txtCategoryName" Text="{Binding Path=Name}"/>
                    </StackPanel>
                </DataTemplate>
            </ComboBox.ItemTemplate>
        </ComboBox>
        <ListBox IsSynchronizedWithCurrentItem="True" Name="lstProducts" Margin="0,60,0,141" ItemsSource="{Binding Book}">
            <ListBox.ItemTemplate>
                <DataTemplate>
                    <StackPanel Orientation="Vertical">
                        <TextBox Name="txtProductName" Text="{Binding Name}" Width="250"/>
                        <TextBox Name="txtListPrice" Text="{Binding ListPrice}" Width="100" Foreground="Gold" Background="Black" HorizontalAlignment="Right"/>
                    </StackPanel>                    
                </DataTemplate>
            </ListBox.ItemTemplate>
        </ListBox>
        <Button Height="23" HorizontalAlignment="Right" Margin="0,0,12,12" Name="btnSaveChanges" VerticalAlignment="Bottom" Width="93" Click="btnSaveChanges_Click">Save Changes</Button>
        <Label Height="28" HorizontalAlignment="Left" Margin="7,0,0,106" Name="label1" VerticalAlignment="Bottom" Width="54">Name</Label>
        <Label Height="28" HorizontalAlignment="Left" Margin="7,0,0,77" Name="label2" VerticalAlignment="Bottom" Width="54">ListPrice</Label>
        <Label Height="28" HorizontalAlignment="Right" Margin="0,0,104,78" Name="label3" VerticalAlignment="Bottom" Width="65">Page Size</Label>
        <TextBox Height="23" Margin="67,0,12,112" Name="txtName" VerticalAlignment="Bottom" />
        <TextBox Height="23" Margin="67,0,0,83" Name="txtListPrice" VerticalAlignment="Bottom" HorizontalAlignment="Left" Width="97" />
        <TextBox Height="23" Margin="0,0,12,83" Name="txtPageSize" VerticalAlignment="Bottom" HorizontalAlignment="Right" Width="97" />
        <Button Height="23" HorizontalAlignment="Left" Margin="12,0,0,48" Name="btnAddBook" VerticalAlignment="Bottom" Width="93" Click="btnAddBook_Click">Add</Button>
    </Grid>
</Window>
```

Kullanıcı bu forma göre yeni bir kitap ekleyebilmelidir. Bir kitabın bir kategori altında olması gerektiğinden, oluşturulacak kitabın hangi kategoriye ekleneceğinin belirlenmesi sırasında ComboBox'ta seçili olan Category nesne örneğinden yararlanabiliriz. Kitabın tabiki öncelikle nesnel olarak oluşturulması gerekmektedir. Sonrasında ise ComboBox'ta seçili olan Category öğesinin Book özelliği yardımıyla ilgili koleksiyona eklenmelidir. Bu ekleme işleminin ardından yapılacak olan SaveChanges çağrısı, ekleme işlemi için Ado.Net Data Service tarafına uygun talebin gönderilmesini sağlayacaktır. Bunun doğal sonucu olarakta sunucu tarafında uygun olan SQL Insert sorgusu çalıştırılacaktır. Bakalım gerçekten böyle mi?

![Wink](/assets/images/2009/smiley-wink.gif)

Bu amaçla Add başlıklı Button kontrolümüzün Click olay metodunu aşağıdaki gibi kodladığımızı düşünelim.

```csharp
private void btnAddBook_Click(object sender, RoutedEventArgs e)
        {
            Category currentCategory = cmbCategories.SelectedItem as Category;

            Book newBook = new Book
            {
                ListPrice = Convert.ToDecimal(txtListPrice.Text),
                Name = txtName.Text,
                PageSize = Convert.ToInt16(txtPageSize.Text) ,
                Category=currentCategory
            };

            currentCategory.Book.Add(newBook);
        }
```

İlk olarak kitabın ekleneceği kategori bulunmaktadır. Burada SelectedItem özelliğinin Category tipine dönüştürüldüğüne dikkat edilmelidir. Sonrasında yeni bir Book nesnesi örneklenir ve ilgili özellikleri kontrollerden alınır.(Burada herhangibir hatalı giriş kontrolü yapmadığımızı belirtelim. Aslında yapmamız gerekiyor ancak şu an için odaklanmamız gereken kısım bu değil. Yinede siz örneği denerken mutlaka olası hataların önüne geçmenizi sağlayacak eklemeleri yapmayı unutmayın ![Wink](/assets/images/2009/smiley-wink.gif))

Dikkat edilmesi gereken noktalardan birisi de, Book nesnesi örneklenirken Category özelliğine currentCategory değişkeninin referansını atamamızdır. Bundan sonraki kısım ise son derece basittir. Örneklenen Book nesne örneği, o an seçili olan Category nesnesinin Book özelliğinin temsil ettiği koleksiyona atanır. Birde çalışma zamanına bakalım. Aşağıdaki örnekte bir veri girişi yapılmak istendiğini görüyoruz.

![blg83_BeforeAdd.gif](/assets/images/2009/blg83_BeforeAdd.gif)

Şimdi Add düğmesine basarsak ve kodu debug edersek currentCategory değişkeninin içeriğinin aşağıdaki gibi güncellendiğini görürüz.

![blg83_AddDebug.gif](/assets/images/2009/blg83_AddDebug.gif)

Görüldüğü gibi yeni Book nesne örneği ilgili kategori altına eklenmiştir. Öyleyse birde DataServiceCollection içeriğimize bakalım.

![blg83_AddWatch.gif](/assets/images/2009/blg83_AddWatch.gif)

Yeni eklenen Book nesne örneğinin DataServiceCollection nesne örneği içerisinde de yer aldığı gözlemlenmektedir. Üstelik çalışma zamanının yeni görüntüsüde aşağıdaki gibidir.

![blg83_AfterAdd.gif](/assets/images/2009/blg83_AfterAdd.gif)

Bağlılık buna denir desek yeridir

![Cool](/assets/images/2009/smiley-cool.gif)

Nitekim yapmış olduğumuz nesne eklemesinden ListBox kontrolüde otomatik olarak etkilenmiş ve içeriğini yenilemiştir. Artık Save Changes başlıklı düğmeye basarak değişiklikleri servis tarafına gönderebiliriz. Bu işlem yapıldığı takdirde SQL tarafında aşağıdaki sorgunun çalıştırıldığı gözlemlenir.

```text
exec sp_executesql N'insert [dbo].[Book]([Name], [ListPrice], [CategoryId], [PageSize])
values (@0, @1, @2, @3)
select [BookId]
from [dbo].[Book]
where @@ROWCOUNT > 0 and [BookId] = scope_identity()',N'@0 nvarchar(28),@1 decimal(19,4),@2 int,@3 smallint',@0=N'Yazılımcılar için SQL Server',@1=45.0000,@2=1,@3=800
```

İşte bu kadar...Sırada silme işlemi var ama uzun olan bir yazının verdiği yorgunluğu yaşayan ben bu kutsal görevi siz değerli okurlarıma bırakıyorum

![Wink](/assets/images/2009/smiley-wink.gif)

Silme operasyonunu uygularken debug işlemlerini yapmayı ve çalışma zamanını analiz edip SQL tarafında neler olup bittiğini incelemeyi unutmayın. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[BindingV2.rar (93,31 kb)](/assets/files/2009/BindingV2.rar)