---
layout: post
title: ".Net RIA Servisleri - CRUD İşlemleri"
date: 2009-05-13 22:50:00 +0300
description: "Bildiğiniz gibi bir süredir .Net RIA Servisleri ile ilişkili araştırmalarıma devam etmekteyim. Bu yazımızda, .Net RIA Servislerinde Insert, Update ve Delete işlemlerini nasıl yapabileceğimizi basit bir örnek üzerinden adım adım aktarmaya çalışacağım."
categories:
  - dotnet-ria-services
tags:
  - dotnet-ria-services
  - csharp
  - xml
  - dotnet
  - aspnet
  - linq
  - sql-server
  - silverlight
  - xaml
  - http
  - concurrency
  - generics
---
# .Net RIA Servisleri - CRUD İşlemleri
Merhaba Arkadaşlar,

Bildiğiniz gibi bir süredir.Net RIA Servisleri ile ilişkili araştırmalarıma devam etmekteyim. Bu yazımızda,.Net RIA Servislerinde Insert, Update ve Delete işlemlerini nasıl yapabileceğimizi basit bir örnek üzerinden adım adım aktarmaya çalışacağım. Daha önceki Hello World örneğimizden farklı olarak, DAL (Data Access Layer) içerisinde LINQ to SQL modelini kullanıyor olacağız. İlk adımımız elbetteki bir Silverlight Application projesi oluşturmak olmalıdır..Net RIA Servisini kullanacağımız için, projenin oluşturulması sırasında Link to ASP.NET Server Project seçeneğinin işaretli olmasına dikkat edelim.

Sonrasında Web projesine bir adet LINQ to SQL öğesi eklemeli ve bağlanmak istediğimiz veri kaynağı üzerinden, kullanmak istediğimiz tablo veya stored procedure'leri diagram üzerine sürüklemeliyiz. Ben Insert, Update ve Delete işlemlerini çok basit bir şekilde ele almak istediğimdeN kullanabileceğim en kolay kobay tabloyu seçtim:) Northwind veritabanında yer alan Categories tablosu. Nitekim sadece CategoryName ve Description alanlarına veri eklemek bizim için yeterli olacaktır.

> Ancak örneği biraz daha ileri seviyede geliştirmeye çalışmanızıda şiddetle tavsiye ederim. Söz gelimi, Categories tablosunda Image tipinden Picture isimli bir alan bulunmaktadır. Bu resim alanı binary tiptedir. Bir başka deyişle Silverlight istemcisinin, kategori resmini seçip sunucu tarafına binary formatta aktarmaya çalışmasıda iyi bir mücadele antrenmanı olarak göz önüne alınabilir. Hatta resmin istemci tarafında bir kontrol içerisinde gösterilmeside söz konusu olabilir.

LINQ to SQL diagramımızın içeriği aşağıdaki şekilde görüldüğü gibi olacaktır.

![blg16_0.gif](/assets/images/2009/blg16_0.gif)

Bundan sonra yapmamız gereken, DomainService tipinin eklenmesidir. Bu seferki örneğimizde, tüm CRUD operasyonuna ihtiyacımız olacağından, Categories Entity'si için, Enable Editing özelliğinin etkinleştirilmiş olması şarttır.

![blg16_1.gif](/assets/images/2009/blg16_1.gif)

Bu işlemlerin arkasından CategoryService isimli DomainService sınıfı içerisinde aşağıdaki fonksiyonelliklerin oluşturulduğu görülür.

```csharp
namespace Editing.Web
{
    using System.Linq;
    using System.Web.DomainServices.LinqToSql;
    using System.Web.Ria;

    [EnableClientAccess()]
    public class CategoryService : LinqToSqlDomainService<NorthwindDataContext>
    {
        public IQueryable<Category> GetCategories()
        {
            return this.Context.Categories;
        }

        public void InsertCategory(Category category)
        {
            this.Context.Categories.InsertOnSubmit(category);
        }

        public void UpdateCategory(Category currentCategory, Category originalCategory)
        {
            this.Context.Categories.Attach(currentCategory, originalCategory);
        }

        public void DeleteCategory(Category category)
        {
            this.Context.Categories.Attach(category, category);
            this.Context.Categories.DeleteOnSubmit(category);
        }
    }
}
```

GetCategories metodu ile LINQ to SQL sağlayıcısı üzerinden kategorilerin çekilmesi sağlanmaktadır. Bir kategorinin eklenmesi sırasında InsertOnSubmit, silinmesi işleminde DeleteOnSubmit ve son olarak güncellenmesinde ise Attach isimli LINQ to SQL tarafından hazır olarak gelen fonksiyonların kullanıldığı görülmektedir. Tabiki projenin Build edilmesi sonrasında, istemci tarafındada uygun metodları içeren CategoryContext isimli DomainContext tipi ile sunucu tarafındaki Category entity sınıfının karşılığı hazırlanmış olacaktır. Artık tek yapmamız gereken istemci tarafını tasarlamak ve kodlamaktır. Ben tasarım konusunda özürlü olduğumdan, ancak aşağıdaki Silverlight UserControl bileşenini oluşturabilmiş bulunuyorum.

![Embarassed](/assets/images/2009/smiley-embarassed.gif)

MainPage.xaml

![blg16_2.gif](/assets/images/2009/blg16_2.gif)

```xml
<UserControl x:Class="Editing.MainPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" 
    Width="400" Height="300">
    <StackPanel x:Name="LayoutRoot" Background="White" Orientation="Vertical">
        <TextBlock Text="Categories" FontSize="12" FontStyle="Italic"/>
        <ComboBox x:Name="cmbCategories" Height="24" SelectionChanged="cmbCategories_SelectionChanged">
            <ComboBox.ItemTemplate>
                <DataTemplate>
                    <StackPanel Orientation="Vertical">
                        <TextBlock x:Name="id" Text="{Binding CategoryID}" Foreground="RoyalBlue"/>
                        <TextBlock x:Name="name" Text="{Binding CategoryName}" Foreground="SeaGreen"/>
                        <TextBlock x:Name="description" Text="{Binding Description}" Foreground="Salmon" FontSize="9" FontStyle="Italic"/>
                    </StackPanel>
                </DataTemplate>
            </ComboBox.ItemTemplate>
        </ComboBox>
        <TextBlock Text="ID"  Foreground="BlueViolet"/>
        <TextBlock x:Name="txtCategoryID"/>
        <TextBlock Text="Name" Foreground="BlueViolet"/>
        <TextBox x:Name="txtCategoryName"/>
        <TextBlock Text="Description" Foreground="BlueViolet"/>
        <TextBox x:Name="txtCategoryDescription"/>
        <StackPanel Orientation="Horizontal" Margin="5">
            <Button x:Name="btnUpdate" Click="btnUpdate_Click" Content="Update" Width="100" Margin="2"/>
            <Button x:Name="btnDelete" Click="btnDelete_Click" Content="Delete" Width="100" Margin="2"/>
            <Button x:Name="btnInsert" Click="btnInsert_Click" Content="Insert" Width="100" Margin="2"/>
        </StackPanel>
    </StackPanel>
</UserControl>
```

Hemen bu sayfadanın tasarlanma amacını açıklıyayım. ComboBox kontrolümüz içerisinde, sayfanın oluşturulması sırasında yüklenen Category nesne örnekleri yer alacaktır. Bu nesne örneklerine ait CategoryID,CategoryName ve Description alanları DataTemplate şablonun içerisindeki TextBlock kontrollerinin Text özelliklerine bağlanmıştır (Binding). Kullanıcı isterse, sayfanın alt kısmında yer alan TextBox kontrollerini kullanarak yeni bir Category ekleyebilir. Tek yapması gereken, Insert başlıklı Button kontrolüne basmaktır. Diğer taraftan ComboBox kontrolünde bir Category seçildiğinde, buna ait CategoryName ve Description bilgileri ile CategoryID değeri, alt tarafta yer alan kontrollere, istemci tarafındaki DomainContext tipinin ilgili Categories özelliği üzerinden getirilmektedir. Kullanıcı bu işleyişi güncelleme sırasında değerlendirebilir. Bilgiler üzerinde gerekli değişiklikleri yaptıktan sonra Update düğmesini kullanması yeterlidir. Benzer şekilde silme işlemi içinde sadece ve sadece Delete düğmesini ele alabilir.

> Tabi bu örnekte, silinmek istenen Category bilgisinin, sunucu tarafında başka birisi tarafından silinmiş olma durumu söz konusu olabilir. Yada var olan bir kaydı güncellemek isteyen kullanıcıdan önce başka birisi güncelleştirmiş olabilir ve o anki kullanıcı eski veriye bakıyor olabilir. Sanıyorumki nereye varmak istediğimi biraz anladınız. Eş zamanlı olarak birbirlerinden habersiz bir şekilde veriyi ektiliyen istemcilerin olduğu vaka. Bu tip bir vaka.Net RIA Servislerinin kullanıldığı bir ortamda son derece olasıdır. Önemli olan noktalardan birisi, SQL tarafında çalıştırılan sorgulardaki Where kriteridir. Where kriterinde varsayılan olarak nasıl bir yaklaşım sergilenmektedir? Bunu ilerleyen kısımlarda görmeye çalışacağız.

Gelelim kod tarafına...

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Editing.Web;

namespace Editing
{
    public partial class MainPage : UserControl
    {
        CategoryContext context = new CategoryContext();

        public MainPage()
        {
            InitializeComponent();

            cmbCategories.ItemsSource = context.Categories;
            context.LoadCategories();
        }

        private void cmbCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count>0
                && e.AddedItems[0]!=null)
            {
                Category selectedCategory = (Category)e.AddedItems[0];

                txtCategoryID.Text = selectedCategory.CategoryID.ToString();
                txtCategoryName.Text = selectedCategory.CategoryName;
                txtCategoryDescription.Text = selectedCategory.Description;
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            // Güncellenmek istenen category nesne örneği, DataContext referansının Categories koleksiyonu üzerinden çekilir.
            if (!String.IsNullOrEmpty(txtCategoryID.Text))
            {
                Category category = context.Categories.Single<Category>(c => c.CategoryID == Convert.ToInt32(txtCategoryID.Text));

                // Güncellenmek istenen Category nesne örneğinin CategoryName ve Description alanlarına ilgili değerler aktarılır
                category.CategoryName = txtCategoryName.Text;
                category.Description = txtCategoryDescription.Text;
            }

            // Değişiklikler sunucu tarafına gönderilir.
            context.SubmitChanges();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Silinmek istenen Category tipi ComboBox içerisinden seçildikten sonra
            // ilk olarak DataContext tipi içerisindeki koleksiyondan çıkartılır.
            context.Categories.Remove((Category)cmbCategories.SelectedItem);

            // Değişiklikler sunucu tarafına gönderilir
            context.SubmitChanges();

            // Silme işleminden sonra sunucu tarafından Categories tablosunun son içeriği alınır
            context.LoadCategories();

            // Kontrollerin içeriği temizlenir
            txtCategoryDescription.Text = "";
            txtCategoryName.Text = "";
            txtCategoryID.Text = "";
        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            // Yeni Category tipi örneklenir
            Category newCategory = new Category { 
                CategoryName = txtCategoryName.Text
                , Description = txtCategoryDescription.Text };

            // Örneklenen Category tipi, DataContext üzerindeki Categories koleksiyonuna eklenir.
            context.Categories.Add(newCategory);

            // Değişiklikler onaylanır ve sunucu tarafına aktarılır
            context.SubmitChanges();
        }
    }
}
```

Artık testlerimize başlayabiliriz. Burada Insert, Update ve Delete gibi işlemler söz konusu olduğundan ve sunucu tarafında SQL kullanıldığından, arka planda çalıştırılan sorgu cümlelerini eminimki sizde en az benim ettiğim kadar merak ediyorsunuzdur. Bu nedenle SQL Server Profiler aracımızda bir yandan açık duruyor olacak.

![Wink](/assets/images/2009/smiley-wink.gif)

İlk karşılaşacağımız ekran görüntüsü aşağıdakine benzer olacaktır.

![blg16_3.gif](/assets/images/2009/blg16_3.gif)

Görüldüğü gibi ComboBox içerisinde, kategorilerin tamamı yer almaktadır. Eğer kullanıcı herhangibir öğeyi seçerse aşağıdaki ekran görüntüsünde olduğu gibi, o kategoriye ait bilgiler gelecektir.

![blg16_4.gif](/assets/images/2009/blg16_4.gif)

Bu sırada ekrana gelen veri içeriğini değiştirdiğimizi ve sonrasında Update tuşuna bastığımızı düşünelim. Örnek olarak kategori adı ve açıklamalarının sonuna üç nokta koyduğumuzu varsayalım. Bu durumda SQL tarafında aşağıdaki sorgunun çalıştırıldığını görebiliriz.

```text
exec sp_executesql N'UPDATE [dbo].[Categories]
SET [CategoryName] = @p2, [Description] = @p3
WHERE ([CategoryID] = @p0) AND ([CategoryName] = @p1)',N'@p0 int,@p1 nvarchar(10),@p2 nvarchar(13),@p3 ntext',@p0=2,@p1=N'Condiments',@p2=N'Condiments...',@p3=N'Sweet and savory sauces, relishes, spreads, and seasonings...'
```

Hemen dikkatimi çeken bir noktayı vurgulamak istiyorum. Sadece CategoryName ve Description alanlarını güncelleştirdik. Bu nedenle SQL tarafında yürütülen Update sorgusunda yanlızca bu alanlar yer almaktadır. Picture alanı için herhangibir ifade bulunmamaktadır. Bu neden önemlidir? n tane alandan oluşan bir tablonun kullanıldığı düşünüldüğünde, sadece bir alan için güncelleştirme yapılıyorsa, tüm alanların sorguya dahil edilmesi yerine sadece ilgili olanın eklenmesi söz konusudur...Mu acaba? Bunu test etmek son derece kolaydır aslında. Sadece CategoryName alanın değerini güncelleştirdiğinizi düşünelim. Bu durumda SQL profiler ile yaklanan sorguya bakarsak eğer, Description özelliğine, txtCategoryDescription kontrolünün değişmeyen içeriğini aktarmış olsak bile, sadece CategoryName alanının sorguya dahil edildiğini görebiliriz. Bu bizim için oldukça iyi bir haber aslında.

Yeni bir kategori eklenmek istendiğindeyse,

![blg16_5.gif](/assets/images/2009/blg16_5.gif)

sunucu tarafında aşağıdaki SQL sorgusu çalıştırılacaktr.

```text
exec sp_executesql N'INSERT INTO [dbo].[Categories]([CategoryName], [Description], [Picture])
VALUES (@p0, @p1, @p2)

SELECT CONVERT(Int,SCOPE_IDENTITY()) AS [value]',N'@p0 nvarchar(5),@p1 ntext,@p2 image',@p0=N'Kitap',@p1=N'Bisiklet nasıl sürülür, nasıl monte edilir :)',@p2=NULL
```

Sorgu cümlesinde standart bir Insert ifadesi olmasının dışında, eklenen kayıt için üretilen Identity değerinin, SCOPEIDENTITY () fonksiyonundan yararlanılarak geriye döndürüldüğü gözden kaçırılmamalıdır. Öyleki, yeni eklenen satıra ait bilgiler ComboBox kontrolüne otomatik olarak bağlanırken, CategoryID değerininde sunucudan alındığı rahatlıkla gözlemlenebilir.

![blg16_6.gif](/assets/images/2009/blg16_6.gif)

Silme işlemi için bir kategorinin seçilmesi gerekmektedir. Seçilen kategoriye ait Category nesne örneği bulunduktan sonra ise Remove metodu ile DomainContext içerisindeki koleksiyondan çıkartılır. Sonrasında ise değişikleri sunucu göndermek için yine SubmitChanges metodundan yararlanılır. Sonuç itibariyle SQL sunucusuna giden sorgu cümlesi aşağıdaki gibidir.

```text
exec sp_executesql N'DELETE FROM [dbo].[Categories] WHERE ([CategoryID] = @p0) AND ([CategoryName] = @p1)',N'@p0 int,@p1 nvarchar(5)',@p0=12,@p1=N'Kitap'
```

Sorguda dikkat çeken en önemli nokta Where kriterine, Image tipinden olan Picture ile ntext tipinden olan Description dışındaki tüm alanların dahil edilmesidir. Bu bir anlamda eş zamanlı çakışmaların önüne geçilmesini sağlamaktadır ki aynı durum Update için çalıştırılan SQL sorgusunda da geçerlidir. Bilmem farketmiş miydiniz?

![Laughing](/assets/images/2009/smiley-laughing.gif)

Böylece geldik bir yazımızın daha sonuna. Bu kısa yazıda,.Net RIA Servislerinde Insert,Update ve Delete işlemlerini basit bir biçimde ele almaya ve arka planda hareket eden SQL cümleciklerine bakıldığında gözümüze çarpan önemli noktaları vurgulamaya çalıştım..Net RIA Servisleri ile ilişkiki araştırmalarıma devam ettikçe sizlerle paylaşıyor olacağım. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Editing.rar (1,14 mb)](/assets/files/2009/Editing.rar)