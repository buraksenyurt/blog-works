---
layout: post
title: "WCF RIA Services - Bir Merhaba Diyelim"
date: 2009-11-24 01:56:00 +0300
categories:
  - wcf-eco-system
  - wcf-ria-services
tags:
  - wcf-eco-system
  - wcf-ria-services
  - csharp
  - xml
  - dotnet
  - ado-net
  - linq
  - wcf
  - silverlight
  - xaml
  - web-service
  - xml-web-services
  - http
  - visual-studio
---
Yağmurlu günler ve kış geldikçe bir blog yazarının ilham gelmesini bekleyerek zaman geçirmesine hiç mi hiç gerek yoktur? (Tabi yandaki resimde görülen köprüye o açıdan uzun uzun baktığınızda bloğunuza yazacak çok güzel fikirler edinebilirsiniz) Aslında bu gün itibariyle blog yazısına konsantre olmak için gerekli şartlar zaten mevcuttur. Bir adet bilgisayar, internet bağlantısı, gerekli referans kitaplar (eğer konu ile ilişkili bulunabilirse), kapalı bir hava, güzel bir müzik ve onu kulaktan beyin hücrelerine kaliteli bir şekilde aktaracak kulaklıklar ile yağmurlu bir gün.

![blg105_Giris.jpg](/assets/images/2009/blg105_Giris.jpg)

Tabi insan bazen bloğuna yazarken bir solukta işe yarayacak eserlerde çıkartmak isteyebilir. İşte ben bu yüzden Hello World uygulamalarını çok severim. Zaten Microsoft pek çok eğitim materyalinde detaya girmeden önce, basit bir Hello World uygulaması ile "aslında biz bu konsept ile neyi yapabiliyoruz" sorusuna cevap vererek başlamayı tercih etmiştir,etmektedir. Örneğin çok çok eski Xml Web Services eğitiminde böyle bir giriş bulunmaktadır. Önce yazılmış olan bir Xml Web Service'inin kullanıdırıması öğretilir. Dikkat edin eğitimin amacı Xml Web Service'lerini geliştirmek. Öyleyse bu gün menümüzde ne var bir bakalım.

Takip eden arkadaşlarımız bir süre önce [Microsoft PDC 2009](http://microsoftpdc.com/) konferansının gerçekeştiğini ve pek çok yeniliğin tanıtıldığını bilirler. Benim açımdan önemli olan gelişmelerden biriside pek çok teknolojinin adının değişmesi olmuştur. Özellikle WCF tabanlı olarak geliştirilen pek çok yardımcı servis modelinin adı, benimde istediğim ve beklediğim gibi tekilleştirildi. Buna göre Ado.Net Data Services'ler WCF Data Services ve Rich Internet Application'lar için n-tier sorununu servis bazlı olarak kolayca aşmamızı sağlayan.Net RIA Services'da WCF RIA Services olarak isim değiştirmiştir. Aslında bu bilgilerden yararlanıldığında bir WCF eko sisteminin oluşturulduğunu ve içerisinde Workflow Services, Data Services, RIA Services, Web HTTP Services ve Core Services gibi kavramların yer aldığını ifade edebiliriz. Şimdilik bu eko sistemin içerisine çok fazla girmeyeceğiz. Bu yazımızdaki amacımız Beta'sı yayımlanan WCF RIA Services'e Bir Merhaba diyebilmek.

Bildiğiniz üzere RIA Service'leri ile Silverlight gibi Rich Internet Application istemcilerinin n-tier modeline göre geliştirilebilmesi oldukça kolaylaştırılmaktadır. Özellikle Silverlight 4.0 ile Visual Studio 2010 tarafına getirilen yeni özellikler (örneğin Windows uygulamalarındaki gibi Data Sources kısmının kullanılabilmesi ve bu sayede sürükle-bırak desteği) ve WCF RIA Services bir araya geldiğinde oldukça güzel sonuçlar ortaya çıktığını söyleyebiliriz. Aslında söz konusu kolaylıkları görsel derslerimizde incelemeye çalışcağımız şimdiden belirtmek isterim.

Ancak hemen öncesinde çok basit bir Hello World uygulaması yaparak, Silverlight uygulaması içerisinden WCF RIA Service'lerin nasıl kullanılabileceğini görelim. Örneğimizi mümkün olduğunda basit bir şekilde gerçekleştireceğiz. WCF RIA Service'imiz arka planda Entity Data Model'i kullanıyor olacak. Bu amaçla kaynak bir veritabanını SQL üzerinde ele alacağız. Ancak bu sefer AdventureWorks değil

![Smile](/assets/images/2009/smiley-smile.gif)

Yihaaaa!!! Bu kez örnek olarak [Chinook](http://chinookdatabase.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=21111)isimli Codeplex üzerinden yayınlanan ve pek çok MVP, Microsoft çalışanı ve profesyonlin konu anlatımlarında kullandığı açık kaynak veritabanını ele alacağız. Haydi bakalım parmakları sıvayalım.

![Wink](/assets/images/2009/smiley-wink.gif)

İlk olarak örneğimizi Visual Studio 2010 Ultimate Beta 2 üzerinde geliştirdiğimizi ifade etmek isterim. Diğer yandan [Silverlight 4.0 Beta ve WCF RIA Services](http://www.silverlight.net/getstarted/riaservices/) kurulumlarının da yapılmış olması gerektiğini hatırlatalım. Eğer bu kurulumlar tamamlandıysa işe basit bir Silverlight Application projesi oluşturarak başlayabiliriz. Oluşturma işlemi sırasında, Enable.NET RIA Services seçeneğini etkinleştirmemiz gerekmektedir ki WCF RIA Service'leri kullanabilelim (Tabi isim değişse de IDE üzerinde henüz değişmemiş olduğu gözden kaçmamalıdır. Belkide isim değişmez. Immm...Bilemiyorum. Değişirse iyi olur tabi)

![blg105_EnableNetRIAServices.gif](/assets/images/2009/blg105_EnableNetRIAServices.gif)

Projemizin bu şekilde oluşturulması sonrasında Silverlight uygulamasının host edileceği ayrı bir Web uygulamasının da oluşturulduğunu görebiliriz. Söz konusu Web uygulaması hem Entity Data Model'i hemde DomainService sınıfını içerecektir. Veri modeli için ChinookModel isimli yeni bir Ado.Net Entity Data Model öğesini Web projesine ekleyerek devam edelim. Başlangıç için aşağıdaki şekilde görülen Entity tiplerinin oluşturulmasını sağlayabiliriz.

![blg105_EntityDataModel.gif](/assets/images/2009/blg105_EntityDataModel.gif)

Web uygulaması tarafında artık bir veri modelimiz bulunmaktadır. Silverlight uygulamasına veri modelinden hizmet sunabilmek için (CRUD-CreateReadUpdateDelete işlemleri), yine Web projesine bir DomainService sınıfı ekleyerek devam etmemiz gerekmektedir. Bunun için Web sekmesinden Domain Service Class öğesini seçmemiz yeterlidir.

![blg105_AddDomainService.gif](/assets/images/2009/blg105_AddDomainService.gif)

Karşımıza çıkacak olan Wizard adımlarında, kullanacağımız veri modelini ve ilgili Entity tiplerini seçerek ilerliyor olacağız. Burada hemen bir ipucunu vermek isterim; Entity Data Model oluşturulduktan sonra projeyi derlemezsek, Available Data Contexts/ObjectContexts ComboBox'ında herhangibir içeriğin çıkmadığı görülecektir. Derleme işleminden sonra ise aşağıdaki şekilde görüldüğü üzere sadece Album Entity tipini seçerek ilerleyelim.

![blg105_AddWizard.gif](/assets/images/2009/blg105_AddWizard.gif)

Album Entity'si üzerinden örneğimizde veri ekleme, çıkartma ve silme işlemlerini bu yazımızda yapmıyor olacağız ancak arka planda oluşturulan servis sınıfı içerisinde ne gibi kodlamalar yapıldığını görmekte yarar var. Bu adımı geçtikten sonra Web projesi tarafında üretilen ChinookDomainService sınıfını kısaca incelemenizi tavsiye ederim. İlk etapta sınıf diagramına baktığımızda aşağıdaki içeriğe sahip olduğunu görebiliriz.

![blg105_ChinookClassDiagram.gif](/assets/images/2009/blg105_ChinookClassDiagram.gif)

Dikkat edileceğiz üzere LinqToEntitiesDomainService tipinden türeyen sınıf içerisinde CRUD işlemleri için üretilmiş dört metod bulunmaktadır. GetAlbums isimli metod IQueryable tipinden bir referans döndürmektedir. Buda istemci tarafından LINQ ifadeleri ile sorgulanabilir bir içeriğin yakalanabileceğini göstermektedir. Ayrıca Delete, Insert ve Update işlemlerinde parametre olarak Album tipinden nesne örneklerine ihtiyaç duyulmaktadır ki bunların tamamı Silverlight uygulaması tarafından da kullanılabilir. Nitekim Silverlight uygulamasındaki gizli dosya içeriklerine bakıldığında gerekli Entity tipinin bu tarafa da taşındığı görülebilir.

![blg105_ClientSide.gif](/assets/images/2009/blg105_ClientSide.gif)

Web tarafında yer alan DomainService sınıfı içerisinde şimdilik GetAlbums metodunu kullanıyor olacağız. Nitekim ilk amacımız Album listesini Silverlight tarafındaki bir DataGrid kontrolüne çekmek olacak.

```csharp
[EnableClientAccess()]
    public class ChinookDomainService 
        : LinqToEntitiesDomainService<ChinookEntities>
    {
        public IQueryable<Album> GetAlbums()
        {
            return this.ObjectContext.Albums;
        }
// KOD DEVAM EDİYOR...
```

Burada önemli olan noktalardan birisi sınıfın başında istemci erişimine izin verildiğini belirten EnableClientAccess isimli niteliğin (Attribute) kullanılmış olmasıdır. Diğer yandan GetAlbums metodu aslında ObjectContext içerisinde Albums özelliği üzerinden album içeriğini döndürmektedir. Dolayısıyla LINQ sorgusu ile Where gibi operatorler kullanılabilir ve metod istendiğinde parametrik olarak çalıştırılabilir (Bu gibi pek çok farklı konuyu görsel derslerimde ele almayı planladığım için burada fazla detaya girmeyeceğiz). Artık Silverlight uygulamamızı tasarlamaya başlayabiliriz. Bu amaçla MainPage.xaml içeriğini aşağıdaki gibi geliştirdiğimizi düşünelim.

```xml
<UserControl x:Class="SilverlightApplication3.MainPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    mc:Ignorable="d"
    d:DesignHeight="300" d:DesignWidth="600" xmlns:data="clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Data">

    <Grid x:Name="LayoutRoot" Background="White">
        <data:DataGrid AutoGenerateColumns="True" 
                       Height="250" 
                       HorizontalAlignment="Left" 
                       Name="grdAlbums" 
                       VerticalAlignment="Top" 
                       Width="600" />
        <Button Content="Get Albums" 
                Height="23" 
                HorizontalAlignment="Left" 
                Margin="12,265,0,0" 
                Name="btnGetAlbums" 
                VerticalAlignment="Top" 
                Width="75" 
                Click="btnGetAlbums_Click" />
    </Grid>
</UserControl>
```

Burada küçük bir ipucu daha vermek isterim; başlangıçta DataGrid bileşeninin AutoGenerateColumns özelliği False değerine sahiptir. Bu nedenle kodlama doğru bir şekilde yapılsa dahi içeriğin DataGrid kontrolüne aktarılmadı görülecektir. Dolayısıyla ilgili niteliğin değerini bu örnek için True yapmayı unutmamalıyız. btnGetAlbums isimli düğmeye basıldığında tüm Album listesinin bir DataGrid içerisine doldurulmasını sağlamak için, kod tarafını aşağıdaki gibi geliştirmemiz yeterlidir.

```csharp
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ria;
using SilverlightApplication3.Web;

namespace SilverlightApplication3
{
    public partial class MainPage 
        : UserControl
    {
        ChinookDomainContext context;

        public MainPage()
        {
            InitializeComponent();
            // DomainContext nesnesi örneklenir
            context = new ChinookDomainContext();
        }

        private void btnGetAlbums_Click(object sender, RoutedEventArgs e)
        {
            // Asenkron Load operasyonunu gerçekleştirecek tip tanımlanır
            // Tip parametre olarak Album' leri çekmek için gerekli sorguyu üreten GetAlbumsQuery metodunu kullanmaktadır.
            LoadOperation<Album> loader = context.Load<Album>(context.GetAlbumsQuery());
            grdAlbums.ItemsSource = loader.Entities; // Load operasyonu tarafından elde edilen Entite yüklenir
        }
    }
}
```

Ve çalışma zamanındaki sonuç;

![blg105_Runtime.gif](/assets/images/2009/blg105_Runtime.gif)

Tabi daha ne kolaylıklar bar Visual Studio tarafında bir bilseniz.

![Wink](/assets/images/2009/smiley-wink.gif)

İştahınızı kabartmış olabilirim ama devamı için görsel videolarımı bekleminizi öneririm. Umarım vakit bulup çekeceğim. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[SilverlightApplication3.rar (1,28 mb)](/assets/files/2009/SilverlightApplication3.rar)
