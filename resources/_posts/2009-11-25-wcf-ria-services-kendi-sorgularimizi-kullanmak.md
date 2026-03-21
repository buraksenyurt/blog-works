---
layout: post
title: "WCF RIA Services - Kendi Sorgularımızı Kullanmak"
date: 2009-11-25 04:30:00 +0300
categories:
  - wcf-eco-system
  - wcf-ria-services
tags:
  - wcf-ria-services
  - .net-ria-services
  - windows-communication-foundation
  - wcf-eco-system
---
Bir önceki yazımızda WCF RIA Service'lerine kısa bir giriş yapmış ve ilk [Hello World](/2009/11/24/wcf-ria-services-bir-merhaba-diyelim/)uygulamamızı geliştirmiştik. Bu yazımızda yine Chinook veritabanında yer alan albümlerin alfabetik olarak elde edilebildiği ve bunlara bağlı parçalarında gösterilebildiği bir Silverlight uygulaması yazmaya çalışacağız. Bu örnekteki temel amacımız ise, kendi sorgulama metodlarımızı ilgili DomainService sınıfı içerisinde nasıl geliştirebileceğimizi görmek ve Silverlight uygulamasında göze daha hoş gelecek (Her ne kadar buna kendimde inanamasam da ![Sealed](/assets/images/2009/smiley-sealed.gif)) bir arayüzü tasarlayabilmek olacak. İlk etapta hedefimizin aşağıdaki ekran görüntüsünde yer alan uygulama arayüzü ve fonksiyonelliğine ulaşmak olduğunu ifade etmek isterim.

![blg106_Goal.gif](/assets/images/2009/blg106_Goal.gif)

Dikkat edileceği üzere A...Z'ye kadar sıralanmış bir Button kümesi görülmektedir. Bu düğmelerden herhangibirisine basıldığında, o harf ile başlayan albümlerin isimleri ComboBox bileşenine doldurulmaktadır. Kullanıcı eğer ComboBox bileşeninden bir albümü seçerse, bu albüm içerisinde yer alan şarkı listeside alt tarafta yer alan ve arka planında harikulade

![Cool](/assets/images/2009/smiley-cool.gif)

bir manzaraya sahip olan DataGrid kontrolü içerisine doldurulmaktadır. Bu örnekte baş harfine göre albüm'lerin getirilebilmesi ve seçilen albüme ait olan şarkıların çekilmesi için WCF RIA Service içerisinde gerekli sorgu metodlarının yazılmış olması gerekmektedir.

Bildiğiniz gibi Ado.Net Entity Data Model nesnesi içeriğinden seçilen Table, View yada Stored Procedure'lere göre Domain Service sınıfının içeriğinde hazır metodlar oluşmaktadır. Ancak bu metodlar her zaman için yeterli olmayabilir. Özellikle çok büyük boyutta veri kümelerinin döndürülmesi yerine performans açısından filtrelenmiş içeriklerin tedarik edilmesi tercih edilmelidir. Bu açıdan bakıldığında Domain Service sınıfı içerisine kendi operasyonlarımı eklemek veya var olanları uygun bir şekilde güncelleştirmek kaçınılmazıdır. Bu bilgilerden yola çıkarsak, geliştireceğimiz örnekte ilk hedefimiz üretilen Domain Service sınıfının metodlarını kendi istediğimiz şekilde geliştirmek olacaktır.

> Kişisel Not: Entity Data Model ve DomainService'in nasıl hazırlanması gerektiğini bir önceki yazımızda incelediğimizden burada tekrar edilmeyecektir. Ancak Entity Data Model içerisinde Album ve Track tablolarının karşılıklarının kullanıldığını belirtmek isterim.

İlk olarak, ChinookDomainService adı ile oluşturacağımız Domain Service sınıfının içeriğindeki tüm operasyonları silip aşağıdaki hale getirdiğimizi düşünelim.

```csharp
namespace SilverlightApplication5.Web
{
    using System.Linq;
    using System.Web.DomainServices.Providers;
    using System.Web.Ria;

    [EnableClientAccess()]
    public class ChinookDomainService : LinqToEntitiesDomainService<ChinookEntities>
    {
        public IQueryable<Album> GetAlbumsByFirstLetter(string firstLetter)
        {
            return from albm in ObjectContext.Album
                   where albm.Title.StartsWith(firstLetter)
                   orderby albm.Title
                   select albm;
        }

        public IQueryable<Track> GetTracks(int albumId)
        {
            return from track in ObjectContext.Track
                   where track.AlbumId == albumId
                   orderby track.Name
                   select track;
        }
    }
}
```

GetAlbumsByFirstLetter ve GetTracks isimli metodlar IQueryable tipinden referanslar döndürmektedir. GetAlbumsByFirstLetter metodu parametre olarak string bir bilgi almakta ve Title bilgisi bu içerik ile başlayanların listesini geriye döndürmektedir (Örneğin baş harfi A olan albümlerin elde edilmesi). Diğer yandan GetTracks metodu, albumId isimli parametre sayesinde, bir albüme bağlı olan şarkıların listesini Name alanına göre alfabetik sırada döndürmektedir. Böylece çok basit olsalardan kendi operasyonlarımızı tanımlamış bulunmaktayız. Solution'u bu haliyle derlediğimizde Silverlight uygulaması içerisinde yer alan ChinookDomainContext sınıfında uygun metod çağrılarının oluşturulduğunu açık bir şekilde görebiliriz.

![blg106_ClientSide.gif](/assets/images/2009/blg106_ClientSide.gif)

DomainService sınıfını bu şekilde düzenledikten sonra sıra Silverlight uygulamasını geliştirmeye geldi. Bu amaçla MainPage.XAML içeriğini aşağıdaki gibi geliştirdiğimizi düşünelim.

```xml
<UserControl x:Class="SilverlightApplication5.MainPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    mc:Ignorable="d"
    d:DesignHeight="401" d:DesignWidth="640" xmlns:dataInput="clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Data.Input" xmlns:data="clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Data">

    <Grid x:Name="LayoutRoot" Background="White">
        <ComboBox Height="23" HorizontalAlignment="Left" Margin="11,62,0,0" Name="cmbAlbums" VerticalAlignment="Top" Width="449" SelectionChanged="cmbAlbums_SelectionChanged">
            <ComboBox.ItemTemplate>
                <DataTemplate>
                    <StackPanel>
                        <TextBlock Text="{Binding Title}"/>
                    </StackPanel>
                </DataTemplate>
            </ComboBox.ItemTemplate>
        </ComboBox>
        <dataInput:Label Height="28" HorizontalAlignment="Left" Margin="12,46,0,0" Name="label1" VerticalAlignment="Top" Width="120" Content="Albümler" />
        <StackPanel ScrollViewer.HorizontalScrollBarVisibility="Auto" Height="26" HorizontalAlignment="Left" Margin="11,14,0,0" Name="pnlButtons" VerticalAlignment="Top" Width="617" Background="#FFEBEBB2" Orientation="Horizontal" UseLayoutRounding="True"></StackPanel>
        <data:DataGrid AutoGenerateColumns="True" Height="285" HorizontalAlignment="Left" Margin="15,104,0,0" Name="grdTracks" VerticalAlignment="Top" Width="613" Visibility="Visible">
            <data:DataGrid.Background>
                <ImageBrush ImageSource="/SilverlightApplication5;component/Images/1243016_70835687.jpg" />
            </data:DataGrid.Background>
        </data:DataGrid>
    </Grid>
</UserControl>
```

Belkide dikkat çeken ilk nokta ComboBox kontrolü içerisinde bir DataTemplate kullanılmasıdır. Nitekim DataTemplate kullanarak Title alanını göstermek istediğimizi belirtmediğimiz durumda, AlbumId alanının getirildiğini görürüz. Bu bilgi son kullanıcı açısından çok anlamlı değildir. Ama tabiki DataTemplate içerisinde yer alan StackPanel elementinde istenilen kontroller kullanılarak daha fazla Album bilgisinin ComboBox'ın her bir öğesinde gösterilmeside sağlanabilir.

Gelelim MainPage için kod tarafına;

```csharp
using System.Windows.Controls;
using System.Windows.Ria;
using SilverlightApplication5.Web;

namespace SilverlightApplication5
{
    public partial class MainPage 
        : UserControl
    {
        ChinookDomainContext context= new ChinookDomainContext();

        void LoadButtons()
        {
            for (int i = 65; i < 91; i++)
            {
                Button btn = new Button();
                btn.Width = 20;
                btn.Height = 20;
                btn.Name = "Button_" + i.ToString();
                btn.Content = ((char)i).ToString();
                pnlButtons.Children.Add(btn);

                btn.Click += (o,e) =>
                    {
                        grdTracks.Visibility = System.Windows.Visibility.Collapsed;
                        LoadOperation<Album> albumLoadOpt = context.Load<Album>(context.GetAlbumsByFirstLetterQuery(btn.Content.ToString()));
                        cmbAlbums.ItemsSource = albumLoadOpt.Entities;
                    };
            }
        }

        public MainPage()
        {
            InitializeComponent();

            LoadButtons();
            grdTracks.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void cmbAlbums_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {            
            if (e.AddedItems.Count > 0)
            {
                int albumId = ((Album)e.AddedItems[0]).AlbumId;
                LoadOperation<Track> tracks = context.Load<Track>(
                    context.GetTracksQuery(albumId), 
                    (load) => 
                    { 
                        grdTracks.Visibility = System.Windows.Visibility.Visible; 
                    },
                    null
                    );
                grdTracks.ItemsSource = tracks.Entities;
            }
        }
    }
}
```

MainPage yapıcı metodu içerisinde Button bileşenlerinin oluşturulması işlemi gerçekleştirilmektedir. Bu işlem sırasında her Button bileşeni için Click olayının yüklenmesi sağlanmaktadır. Dikkat edileceği üzere Click olay metodu içerisinde, o anda basılan düğmenin Content bilgisinden yararlanılarak bir LoadOperation oluşturulur. Ayrıca, ChinookDomainContext nesne örneği üzerinden yapılan GetAlbumsByFirstLetterQuery metodu ile gerekli sorgunun elde edilmesi sağlanır. Bundan sonra ise ComboBox kontrolünün ItemsSource özelliğine gerekli veri bağlama işlemi yapılır. ComboBox kontrolünde bir öğenin seçilmesi halinde devreye giren SelectionChanged metodunda ise bu kez GetTracks servis metodunun çalıştırılması için gerekli işlemler yapılmaktadır.

Yanlız bu sefer ki kullanımda ChinookDomainContext referansına ait Load metodunun ikinci parametresine dikkat edilmelidir. Bu parametre söz konusu Load işlemi tamamlandıktan sonra devreye girecek bir metodu işaret edecek Action tipinden bir temsilcidir (delegate). Burada Load işlemi tamamlandığında görünür olmayan DataGrid kontrolünün görünür hale getirilmesi için sembolik bir işlem yapıldığını belirtebiliriz. Ancak ana fikir, Load operasyonunun tamamlanması ile kontrolü ele alabileceğimiz bir metodun işaret edilebiliyor olmasıdır. Bu kodlamanın ardından DataGrid kontrolünün ItemsSource özelliğine gerekli veri bağlama işleminin yapılması yeterlidir. Uygulamayı bu noktadan sonra teste çıkartabiliriz. Sonuç olarak yazımızın başında belirttiğimiz ekran görüntüsüne benzer sonuçları elde ebiliyor olmamız gerekmektedir.

Peki neler öğrendik?

- WCF RIA Service'lerinde sihirbaz yardımıyla Entity Data Model'den otomatik olarak üretilen Domain Service sınıf metodları yerine kendi sorgulama metodlarımızı kullanabileceğimizi, var olanları istersek güncelleştirebileceğimizi,
- İstemci tarafında, Domain Service sınıfı içerisindeki operasyonlara yapılacak olan çağrılarda Callback metodlarının değerlendirilerek yükleme tamamlandıktan sonrasını anlayıp bazı işlemler yaptırabileceğimizi,
- Silverlight uygulaması içerisindeki kontrollerde DataTemplate kullanarak, servis tarafından çekilen Entiy içeriklerinin sadece istediğimiz alanlarının kullanılabileceğini,
- Silverlight tarafında dinamik olarak kontrollerin nasıl üretilip ilgili elementlere eklenebileceğini,

öğrendik.

hatta Background özelliğine Picture ekleyebileceğimizi ve bu sayede daha hoş bir görüntü sunabileceğimizi farkettik demek istesemde, bu önemsenecek bir mevzu değildir.

![Wink](/assets/images/2009/smiley-wink.gif)

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[SilverlightApplication5.rar (5,28 mb)](/assets/files/2009/SilverlightApplication5.rar)
