---
layout: post
title: ".Net RIA Servisleri - Hello World"
date: 2009-05-13 13:29:00 +0300
categories:
  - dotnet-ria-services
tags:
  - dotnet-ria-services
  - csharp
  - xml
  - dotnet
  - aspnet
  - ado-net
  - entity-framework
  - linq
  - silverlight
  - xaml
  - http
  - generics
  - visual-studio
---
Hatırlayacağınız gibi bir önceki [blog](/2009/05/08/net-ria-servisleri-nedir/)yazımda,.Net RIA Servisleri hakkında edindiğim kısa ve özet teorik bilgileri sizinle paylaşmaya çalışmıştım. Bu yazımda ise, teoriği pratiğe dökmeye gayret edeceğim. Geliştireceğimiz örnek,.Net RIA Servisini kullanan bir Silverlight uygulaması olacak. Geliştirmeyi Visual Studio 2008 üzerinde, Silverlight 3.0 ortamını kullanarak gerçekleştireceğim. Bu nedenle aşağıdaki şekilde görüldüğü gibi, klasik bir silverlight projesi oluşturarak işe başlayabiliriz.

![blg15_1.gif](/assets/images/2009/blg15_1.gif)

Bu işlemin ardından ekrana gelen aşağıdaki pencerede,

![blg15_2.gif](/assets/images/2009/blg15_2.gif)

LINQ to ASP.Net Server Project seçeneğinin işaretli olması önemlidir. Böylece,.NET RIA Servisi için gerekli ön hazırlığın yapılması sağlanmış olur. Elbetteki bunu seçmediğimiz takdirde elimiz kolumuz bağlı değildir. Daha sonradan istenirse, Silverlight uygulamasının özelliklerinden,.Net RIA Servisi destekleyecek şekilde değişiklikler yapılabilir. Yapmış olduğumuz bu işlemlerin sonrasında, HelloRIAServices isimli Silverlight uygulaması sunum mantığını (Presentation Logic) içeren istemci tarafını (client-tier) oluştururken, HelloRIAServices.Web isimli web uygulaması ise, iş mantığını (Business Logic) içeren orta katmanı (mid-tier) oluşturmaktadır. Bu sebepten, veriye erişimi sağlayacak olan LINQ to SQL (veya Ado.Net Entity Framework) öğeleri, Asp.Net Web uygulaması üzerinde yer alacaktır. Aynı şekilde DomainService sınıfıda, Asp.Net Web uygulaması üzerinde konuşlandırılacaktır. Tahmin edileceği üzere, servis için istemci tarafından gönderilecek çağrıları ele alacak olan içerik sınıfı ise (DataContext), Silverlight uygulaması tarafında yer almalıdır.

Bu işlemlerin ardından, DomainService'in erişip istemci tarafına sunacağı veri kümesini oluşturmamız gerekmektedir. Burada, veriye erişmek amacıyla (Data Access Layer tarafı olarak düşünebiliriz), Ado.Net Entity Framework öğesini veya LINQ to SQL sınıflarını kullanabileceğimizi belirtmiştik. Ben örneğimizde, Ado.Net Entity Framework'ü kullanarak, Northwind veritabanı üzerinden aşağıdaki şemaya sahip olan tabloları kullanmayı planlıyorum. Bu noktada şunu hatırlatmakta yarar var. Ado.Net Entity Framework veya LINQ to SQL kullanımı,.Net RIA Servisleri açısından bakıldığında bir zorunluluk yada şart değildir. Dolayısıyla farklı veri kaynaklarını kullanabilir (Örneğin XML tabanlı...) ve istemci tarafına bir DomainService üzerinden sunabiliriz.

![blg15_3.gif](/assets/images/2009/blg15_3.gif)

Şunu hemen belirteyim; EDM içeriğini Asp.Net Web Project üzerinde oluşturmalıyız. EDM diagramından görüldüğü üzere Categories, Products ve Suppliers tabloları için gerekli Entity tipleri otomatik olarak üretilmiştir. Böylece, veriye erişimi sağlayacak olan katmanı bir nevi hazırlamış bulunuyoruz. Bu işlemin ardından proje bir kere derlendikten sonra, istemciye veriyi sunacak olan DomainService içeriğinin hazırlanmasına başlanabilir; ki buda son derece kolaydır

![Laughing](/assets/images/2009/smiley-laughing.gif)

Tek yapmamız gereken, yine web uygulaması projesi içerisine, aşağıdaki şekildende görüldüğü gibi bir DomainService öğesi eklemektir.

![blg15_4.gif](/assets/images/2009/blg15_4.gif)

(Kullandığım sistemdeki kurulumdan kaynaklanan bir sorun olsa gerek, ikon ne yazıkki görünmüyor ![Undecided](/assets/images/2009/smiley-undecided.gif))

Bu seçimin ardından karşımıza aşağıdaki iletişim kutusu gelecektir.

![blg15_5.gif](/assets/images/2009/blg15_5.gif)

Burada Categories ve Products tipleri işaretlenmiştir. Bu tiplerin hiç birisi için Insert, Update veya Delete operasyonu hazırlanmayacaktır. Ancak bu operasyonlarında hazırlanmasını istersek, Enable Editing özelliklerini işaretlememiz yeterlidir. Dikkat edileceği üzere, Available DataContexts/ObjectContexts kısmında az önce oluşturulan NorthwindEntities isimli Ado.Net Entity Framework tipi seçilidir. Taşlar yavaş yavaş yerine oturmaktadır. Artık, DomainService sınıfı hazırdır ve veriyi sunmak için, DAL içerisinde oluşturulan Entity içeriğine bağlanmıştır. Bu noktada biraz durup, oluşturulan tipleri incelemekte yarar olacağını düşünüyorum. Web uygulaması içerisindeki sınıf diagramını (Class Diagram) açtığımızda aşağıdaki şekilde yer alan tiplerin oluşturulduğunu görürüz.

![blg15_6.gif](/assets/images/2009/blg15_6.gif)

Products, Suppliers, Categories isimli sınıflar, Northwind veritabanında seçtiğimiz aynı isimli tabloların karşılıkları olan Entity tipleridir. NorthwindEntities sınıf ise, söz konusu tiplere ait koleksiyonları içerisinde özellik bazında tutmakta ve ekleme gibi temel fonksiyonellikleri içermektedir. Buraya kadarki tipler, veri erişim mantığını içeren parçalar olarak düşünülebilir. NorthwinDomainService sınıfı ise asıl üzerinde odaklanmamız gereken tiptir. Şimdi bu tip ile ilişkili analizlerimizi değerlendirelim.

Herşeyden önce, LinqToEntitesDomainService isimli generic ve abstract bir sınıftan türetildiğini görüyoruz. T tipi olarak örneğimizde, Ado.Net Entity Framework tarafında ürettiğimiz, NorthwindEntities tipi yer almakta. Buna göre, söz konusu DomainService sınıfının, hangi veri içeriğini (DataContenxt) ve üyelerini kullanacağı belirlenmiş oluyor. Burada dikkat çekici noktalardan biriside, DomainService sınıfının türediği tipe ait generic kısıtlamadır.

```csharp
namespace System.Web.DomainServices.LinqToEntities
{
    public abstract class LinqToEntitiesDomainService<T> 
      : LinqToEntitiesDomainService where T : System.Data.Objects.ObjectContext
    {
        protected LinqToEntitiesDomainService();

        protected T Context { get; }
    }
}
```

Koddanda görüleceği üzere T tipinin ObjectContext'ten türeme zorunluluğu bulunmaktadır. Buda geliştiricilere bir bağımsızlık getirmektedir. Yani, ObjectContext sınıfından türeteceğimiz özel tipler sayesinde farklı veri içeriklerinide DomainService içerisinde ele alabiliriz.

Bir diğer nokta, DomainService sınıfı içerisinde sadece GetProducts ve GetCategories isimli metodların yer almasıdır. Her iki metodda IQueryable tipinden referans döndürmektedir. Kod içeriğine baktığımızda durum biraz daha netleşmektedir.

```csharp
namespace HelloRIAServices.Web
{
    using System.Linq;
    using System.Web.DomainServices.LinqToEntities;
    using System.Web.Ria;
    
    [EnableClientAccess()]
    public class NorthwindDomainService : LinqToEntitiesDomainService<NorthwindEntities>
    {
        public IQueryable<Categories> GetCategories()
        {
            return this.Context.Categories;
        }

        public IQueryable<Products> GetProducts()
        {
            return this.Context.Products;
        }
    }
}
```

Her iki metodda basit olarak Context referansına gitmekte ve Categories ile Products koleksiyonlarının içeriklerini istemci tarafına döndürmektedir. Dönüş tipleri IQueryable olduğundan, istemci tarafında LINQ ifadeleri ile sorgulanmaya devam edilmeleri pekala mümkündür. Bunlara ek olarak çok daha önemli bir nokta vardır. Metodlara istenirse parametre verilebilir ve geriye döndürülecek içerik ile ilişkili bazı kısıtlamalar yaptırılabilir.

Bir başka deyişle geliştirici, metodların parametrik yapısı ile oynayabileceği gibi, dönüş içeriğini IQueryable olmasına (IEnumerable da olabilir) dikkat edecek şekilde değiştirebilir. Söz gelimi, belkide istemcinin bulunduğu lokasyondaki tedarikçiye göre bir Products veya Categories içeriğinin döndürülmesi sağlanabilir. Yada çok basit anlamda, içeriklerin örneğin ürün adına veya kategori adına göre sıralanarak döndürülmesi sağlanabilir. Bu nedenle kod içeriğini aşağıdaki gibi değiştirmeye karar verdim.

```csharp
namespace HelloRIAServices.Web
{
    using System.Linq;
    using System.Web.DomainServices.LinqToEntities;
    using System.Web.Ria;

    [EnableClientAccess()]
    public class NorthwindDomainService 
 : LinqToEntitiesDomainService<NorthwindEntities>
    {
        public IQueryable<Categories> GetCategories()
        {
            // Lamda operatörü ve extension method yardımıyla
            return this.Context.Categories.OrderBy(c => c.CategoryName);
        }

        public IQueryable<Products> GetProducts()
        {
            // basit bir LINQ ifadesi yardımıyla
            return (from p in this.Context.Products
                    orderby p.ProductName descending
                    select p);                    
        }
    }
}
```

Tabiki bu değişiklikler ile sınırlı değiliz. İstersek, DomainService sınıfı içerisine farklı fonksiyonelliklerde ekleyebiliriz. Örneğin;

```csharp
public IQueryable<Products> GetProductsByCategory(int categoryId)
{
	return (from p in this.Context.Products
			where p.Categories.CategoryID == categoryId
			orderby p.ProductName
			select p);
}
```

gibi.

Son olarak DomainService sınıfı içerisinde dikkat çeken bir noktayı daha vurgulayalım. Sınıfın kendisine EnableClientAccess isimli bir nitelik (attribute) uygulanmıştır. Bu nitelik, söz konusu sınıfın istemci katmanından görünebileceği anlamına gelmektedir.

Bu adımların ardından Solution tamamıyla derlenirse ve Silverlight uygulamasının öğelerine Show All Files seçeneği ile bakılırsa, aşağıdaki şekilde görülen bir dosyanın üretildiği farkedilebilir.

![blg15_7.gif](/assets/images/2009/blg15_7.gif)

Buradaki kod dosyası, DomainService sınıfı her değiştiğinde ve bu nedenle Web projesi her derlendiğinde otomatik olarak yeniden üretilmektedir. Söz konusu kod dosyası içerisinde, servisten sunulan her bir Entity tipi için karşılık olan bir sınıf bulunmaktadır.

![blg15_8.gif](/assets/images/2009/blg15_8.gif)

Şekildende görüleceği gibi, Ado.Net Entity Framework kullanarak servis üzerinden sunulan Categories ve Products tipleri için, istemci tarafında birer sınıf üretilmiştir. Ayrıca, daha önceki yazımızda da bahsettiğimiz gibi,.NET RIA Servislerinin önemli iki parçasından birisi olan DomainContext türevli bir sınıfda (NorthwindDomainContext) oluşturulmuştur. Servis tarafında (DomainService içerisinde) yer alan GetProducts ve GetCategories metodlarına karşılık olarak, istemci tarafındaki DomainContext tipi içerisine LoadProducts ve LoadCategories fonksiyonları hazırlanmıştır.

Yine özel olarak eklediğimiz GetProductsByCategory metoduna karşılık olarak, DataContext tarafında LoadProductsByCategory isimli fonksiyon üretilmiştir. Dolayısıyla, Silverlight uygulamasında, servis ile konuşulmasını sağlayacak olan proxy içeriği otomatik olarak üretilmiştir. Aslında orta katmanda (mid-tier) yer alan her bir DomainService tipi için, sunum katmanında bir DomainContext tipi var olacaktır. Yani, birden fazla veri kaynağına, farklı DAL öğeleri ile çıkan servisleri barındıran bir sunucu ile bunları ayrı ayrı kullanabilen bir istemci tasarlanması mümkündür. Artık tek yapmamız gereken, DomainContext tipinin ilgili fonksiyonlarını kullanarak istemci tarafını geliştirmektir. Bu amaçla Silverlight uygulamasının MainPage.xaml içeriğini aşağıdaki gibi geliştirdiğimizi düşünelim.

```xml
<UserControl x:Class="HelloRIAServices.MainPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" 
    xmlns:data="clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Data"
    Width="500" Height="320">
    <StackPanel x:Name="LayoutRoot" Background="White" Orientation="Vertical">
        <ComboBox x:Name="cmbCategories" Height="50" VerticalAlignment="Top" SelectionChanged="cmbCategories_SelectionChanged">
            <ComboBox.ItemTemplate>
                <DataTemplate>
                    <StackPanel Orientation="Vertical">
                        <TextBlock x:Name="categoryId" Text="{Binding CategoryID}" FontSize="12" FontFamily="Calibri" Foreground="Blue"/>
                        <TextBlock x:Name="categoryName" Text="{Binding CategoryName}" FontSize="12" FontFamily="Calibri" Foreground="Black"/>
                        <TextBlock x:Name="categoryDescription" Text="{Binding Description}" FontStyle="Italic" FontSize="9" FontFamily="Calibri" Foreground="LimeGreen"/>
                    </StackPanel>
                </DataTemplate>
            </ComboBox.ItemTemplate>
        </ComboBox>
        <data:DataGrid x:Name="grdProducts" Height="250" Background="Lavender" BorderBrush="CadetBlue"/>
    </StackPanel>
</UserControl>
```

UserControl içerisinde bir adet ComboBox ve DataGrid bileşeni bulunmaktadır. DataGrid bileşeninin kullanılabilmesi için Silverlight uygulamasına System.Windows.Controls.Data.dll assembly'ının referans edilmesi gerekmektedir. Ayrıca DataGrid kulanımı için, XAML içerisinde gerekli namespace tanımlamasıda yapılmalıdır. Sayfanın kullanımı son derece basit olacaktır. ComboBox içeriği, MainPage yapıcı metodu içerisinde, kategoriler ile doldurulacaktır. Kullanıcı, ComboBox içerisinden herhangibir kategoriyi seçtiğinde ise, buna bağlı ürün listeside DataGrid kontrolünde gösterilecektir. ComboBox kontrolüne ait veri içeriğinde, bir DataTemplate kullanılmaktadır ve dikkat edileceği üzere Categories isimli Entity tipinin CategoryID, CategoryName ve Description özellikleri kullanılarak bir şablon oluşturulmuştur. MainPage UserControl'üne ait kod içeriği ise aşağıdaki gibidir.

```csharp
using System.Windows.Controls;
using HelloRIAServices.Web;

namespace HelloRIAServices
{
    public partial class MainPage : UserControl
    {
        // DomainContext nesnesi
        NorthwindDomainContext context = null;

        public MainPage()
        {
            InitializeComponent();
            // DomainContext nesnesi örneklenir
            context = new NorthwindDomainContext();
            // ComboBox kontrolüne veri kaynağı olarak, EntityList tipinden olan Categories özeliği bağlanır.
            cmbCategories.ItemsSource = context.Categories;
            // DataGrid kontrolü için veri kaynağı DomainContext nesne örneğindeki Products özelliği ile belirlenir
            grdProducts.ItemsSource = context.Products;

            // Categories listesi LoadCategories metodu ile yüklenir.
            context.LoadCategories();
        }

        private void cmbCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Seçilen öğe Categories tipinden olduğu için CategoryID özelliğine aşağıdaki kod parçasında olduğu gibi ulaşılabilir.
            int selectedCategoryId=((Categories)e.AddedItems[0]).CategoryID;

            // Eğer aşağıdaki temizleme işlemini uygulamassak, Grid kontrolü içerisinde veriler arka arkaya eklenerek çoğalır.
            context.Entities.GetEntityList<Products>().Clear();

            // LoadProductsByCategory metoduna, seçili kategorinin CategoryID değeri gönderilerek, bağlı olan ürün listesinin yüklenmesi sağlanır.            
            context.LoadProductsByCategory(selectedCategoryId);            
        }
    }
}
```

Aslında kod içeriği son derece basittir..Net RIA Servisleri açısından olaya baktığımızda iki önemli nokta göze çarpmaktadır. İlk olarak veri bağlı kontrolleri, Entity içeriklerine bağlamak için DomainContext nesne örneğine ait özelliklerden yararlanılmaktadır (Categories, Products gibi). Diğer taraftan veriyi doldurmak için, bu isteğin sunucu tarafındaki DomainService tipine ulaştırılması gerektiği de ortadadır. Bu sebepten LoadCategories ve LoadProductByCategory metodlarından yararlanılmaktadır. Sonuç olarak uygulama çalışma zamanında test edildiğinde aşağıdaki örnek çıktılar ile karşılaşılacaktır.

Uygulama ilk çalıştırıldığında kategoriler, ComboBox bileşeni içerisine yüklenecektir.

![blg15_9.gif](/assets/images/2009/blg15_9.gif)

Herhangibir kategori seçildiğinde ise...

![blg15_10.gif](/assets/images/2009/blg15_10.gif)

DataGrid kontrolü, bu kategoriye bağlı ürünler ile doldurulacaktır. İşte bu kadar. Görüldüğü gibi, Silverlight uygulamalarında.Net RIA Servislerini kullanılarak, çok katmanlı modelin (n-tier), basitçe iki katmana (2-tier) indergenmesi sağlanabilmektedir. Geliştirdiğimiz örnek göz önüne alındığında, aşağıdaki şekil durumu biraz daha açıklığa kavuşturmaktadır.

![blg15_11.gif](/assets/images/2009/blg15_11.gif)

Böylece geldik bir yazımızın daha sonuna..Net RIA Servisleri ile ilişkili araştırmalarıma devam ettikçe, öğrendiklerimi sizlerle paylaşmaya devam ediyor olacağım. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[HelloRIAServices.rar (1,60 mb)](/assets/files/2009/HelloRIAServices.rar)
