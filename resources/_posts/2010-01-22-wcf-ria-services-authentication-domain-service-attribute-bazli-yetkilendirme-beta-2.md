---
layout: post
title: "WCF RIA Services - Authentication Domain Service - Attribute Bazlı Yetkilendirme [Beta 2]"
date: 2010-01-22 00:45:00 +0300
categories:
  - wcf-eco-system
  - wcf-ria-services
tags:
  - wcf-ria-services
  - .net-ria-services
  - windows-communication-foundation
  - wcf-eco-system
---
Bildiğiniz üzere bir süredir WCF RIA Service'lerinde doğrulama (Authentication), yetkilendirme (Authorization), Role ve Profile yönetimi konularına değinmekteyiz. WCF RIA Service'lerinin temel amaçlarından birisininde RIA tipindeki uygulamalar için Ado.Net Entity Framework gibi kaynaklar üzerinden CRUD (CreateReadUpdateDelete) operasyonlarını sağlanması olduğu düşünüldüğünde, servis fonksiyonelliklerinin yetkilendirilmeside güvenlik açısından önem arz eden konuların başında gelmektedir. Bu konu, WCF RIA Service'lerinde nitelikler (Attributes) yardımıyla ele alınabilmektedir.

![blg115_Giris.jpg](/assets/images/2010/blg115_Giris.jpg)

Bu noktada iki önemli niteliğin olduğunu söyleyebiliriz. Bunlardan birisi Domain Service sınıfının Authentication işlemleri çerçevesinde değerlendirilmesi gerektiğini çalışma zamanına söyleyen RequiresAuthentication niteliğidir. Bu niteliği Domain Service tipine uygulamak yeterlidir. Diğer taraftan, Domain Service içerisinde tanımlanmış olan operasyonların hangi yetkiler altında çalıştırılabileceğini belirtmek için RequiresRole isimli nitelikten yararlanılmaktadır. RequiresRole niteliği string tipinde birden fazla parametre alabilmektedir. Bu parametre değerleri tahmin edileceği üzere rolleri ifade etmektedir. Bu teoriye göre bir Domain Service operasyonunun rol bazlı olaraktan yetki altında çalıştırılmasının sağlanması mümkündür.

Tabi teoride bahsettiğimiz bu kavramları pratiğe dökmemiz bizim için önemlidir. Bu nedenle yazımızın bundan sonraki kısmında basit olarak Chinook veritabanındaki kobay tablolarımızdan olan Album içeriğine ulaşmak için kullanılan bir operasyon üzerinde, rol bazlı yetkilendirme işlemlerini nasıl uygulayabileceğimizi incelemeye çalışacağız.

Kişisel Not: Başlamadan önce Silverlight uygulamasında daha önceki yazılarda sıklıkla anlattığımız şekilde Form bazlı doğrulama (Form-Based Authentication) için gerekli ayarları yapmamız gerektiğini hatırlatalım. Özellikle role yönetimini etkinleştirmemiz gerektiğini ve hem sunucu hemde istemci tarafında gerekli konfigurasyon ve kod ayarlarını uygulamamız gerektiğini unutmayalım. Örneğimizde yine buraks ve bill isimli kullanıcıları değerlendiriyor olacağız. Bunlardan birisi Employee rolünde iken diğeri Finance rolündedir. Amaçlanan sadece Finance rolündekilerin, albüm listesini çekebilmesini sağlamaktır. Tabiki Chinook isimli veritabanını Silverlight uygulamasında kullanabilmek için gerekli Domain Service (ChinookDomainService) öğesinide eklememiz gerektiğini hatırlatmak isterim. Pek çok hatırlatmada bulundum ama önceki yazıları takip edip uygulayan arkadaşlarımız için örneği bu aşamaya getirmek son derece kolay olacaktır düşüncesindeyim ![Wink](/assets/images/2010/smiley-wink.gif)

Gelelim örneğimize. İlk olarak durumu kuş bakışı değerlendirmeye çalışalım. Buna göre aşağıdaki şekli göz önüne alabiliriz.

![blg115_Case.gif](/assets/images/2010/blg115_Case.gif)

Şekildende görüleceği üzere sunucu uygulama tarafında Chinook veritabanına ulaşılmasını ve üzerinde CRUD işlemleri yapılabilmesini sağlayan (ki bu örnekte sadece veri çekiyor olacağız) ChinookDomainService isimli Domain Service tipi bulunmaktadır. Bu tip arada Ado.Net Entity Framework modelini kullanmaktadır. Diğer taraftan doğrulama işlemleri için ASP.NET Membership alt yapısı kullanılmakta olup istemci tarafının bu hizmeti değerlendirebilmesi için birde Authentication Domain Service (ChinookDomainService) öğesi yer almaktadır. İstemci tarafı veri ve güvenlik işlemleri için bu iki servisten yararlanacaktır. Önemli olan noktalardan biriside hangi operasyonu nasıl yetkilendireceğimizi bilmektir. Bu noktada ChinookDomainService sınıfını aşağıdaki şekilde oluşturduğumuzu göz önüne alalım.

```csharp
namespace SilverlightApplication8.Web
{
    using System.Linq;
    using System.Web.DomainServices;
    using System.Web.DomainServices.Providers;
    using System.Web.Ria;

    [RequiresAuthentication] // Servis operasyonlarında Authentication uygulanacağını belirtiyoruz.
    [EnableClientAccess()]
    public class ChinookDomainService : LinqToEntitiesDomainService<ChinookEntities>
    {
        [RequiresRole("Finance")] // Sadece Finance rolündekilerin aşağıdaki operasyonu kullanabileceğini belirtmekteyiz.
        public IQueryable<Album> GetAlbums(int artistId)
        {
            // Örnek olarak ArtistId bilgisine göre albüm listesini Title bilgisine göre A...Z sırasında döndürüyoruz
            return from a in ObjectContext.Albums
                   where a.ArtistId == artistId
                   orderby a.Title
                   select a;
        }
    }
}
```

Dikkat edileceği üzere ChinookDomainService tipine RequiresAuthentication niteliği uygulanmıştır. Bununla birlikte sadece Finance rolündekilerin kullanımına sunulan GetAlbums isimli bir operasyon yer almaktadır. Yazımızın başında da belirttiğimiz üzere RequiresRole niteliğine parametre olarak birden fazla rol adı verilebilir. Sunucu tarafında rol bazlı yetkilendirme için yapmamız gerekenler sadece bu kadardır. Gelelim istemci tarafına. Bu amaçla MainPage.xaml içeriğini ve kod kısmını aşağıdaki gibi geliştirdiğimizi düşünelim.

MainPage.xaml;

![blg115_MainPage.gif](/assets/images/2010/blg115_MainPage.gif)

```xml
<UserControl x:Class="SilverlightApplication8.MainPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    mc:Ignorable="d"
    d:DesignHeight="300" d:DesignWidth="400" xmlns:data="clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Data" xmlns:dataInput="clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Data.Input">

    <Grid x:Name="LayoutRoot" Background="White">
        <Button Content="Load Albums" Height="23" HorizontalAlignment="Left" Margin="305,18,0,0" Name="btnLoadAlbums" VerticalAlignment="Top" Width="83" Click="btnLoadAlbums_Click" />
        <data:DataGrid AutoGenerateColumns="True" Height="105" HorizontalAlignment="Left" Margin="10,87,0,0" Name="grdAlbums" VerticalAlignment="Top" Width="378" />
        <dataInput:Label Height="80" HorizontalAlignment="Left" Margin="10,208,0,0" Name="lblStatus" VerticalAlignment="Top" Width="378" Content="Olası hata mesajı..." />
        <dataInput:Label Height="28" HorizontalAlignment="Left" Margin="12,18,0,0" Name="label1" VerticalAlignment="Top" Width="59" Content="Username" />
        <TextBox Height="23" HorizontalAlignment="Left" Margin="77,18,0,0" Name="txtUsername" VerticalAlignment="Top" Width="120" />
        <dataInput:Label Height="28" HorizontalAlignment="Left" Margin="14,54,0,0" Name="label2" VerticalAlignment="Top" Width="57" Content="Password" />
        <PasswordBox Height="23" HorizontalAlignment="Left" Margin="77,54,0,0" Name="txtPassword" VerticalAlignment="Top" Width="120" />
        <Button Content="Login" Height="23" HorizontalAlignment="Left" Margin="203,18,0,0" Name="btnLogin" VerticalAlignment="Top" Width="82" Click="btnLogin_Click" />
        <Button Content="Logout" Height="23" HorizontalAlignment="Left" Margin="203,54,0,0" Name="btnLogout" VerticalAlignment="Top" Width="82" Click="btnLogout_Click" />
    </Grid>
</UserControl>
```

MainPage.xaml.cs;

```csharp
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ria;
using System.Windows.Ria.ApplicationServices;
using SilverlightApplication8.Web;

namespace SilverlightApplication8
{
    public partial class MainPage : UserControl
    {
        // DomainContext ve AuthenticationService kullanımı için gerekli örnekler
        ChinookDomainContext context;
        AuthenticationService authSrv;

        public MainPage()
        {
            InitializeComponent();

            btnLoadAlbums.IsEnabled = false;
            btnLogout.IsEnabled = false;
            
            context = new ChinookDomainContext();
            authSrv = WebContext.Current.Authentication;

            // Login işlemi olduğunda devreye girecek olay metodu yüklenir
            authSrv.LoggedIn += new EventHandler<AuthenticationEventArgs>(authSrv_LoggedIn);
            // Logout işlemi tamamlandığında devreye girecek olay metodu yüklenir
            authSrv.LoggedOut += new EventHandler<AuthenticationEventArgs>(authSrv_LoggedOut);
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            // Login işlemi yapılır
            LoginOperation logOp = authSrv.Login(new LoginParameters(txtUsername.Text, txtPassword.Password));
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            // Logout işlemi yapılır. Hata var ise exception fırlatılması sağlanır
            authSrv.Logout(true);
        }

        void authSrv_LoggedIn(object sender, AuthenticationEventArgs e)
        {
            // Login olan kullanıcı için güncel User bilgileri alınır
            Web.User currentUser = WebContext.Current.User;
            // Kullanıcının dahil olduğu roller ve adı bilgilendirme amaçlı öğrenilir
            string roles=String.Empty;            
            foreach (var role in currentUser.Roles)
            {
                roles += String.Format("{0}|", role);
            }
            lblStatus.Content =String.Format("Kullanıcı {0} Rolleri : {1}",currentUser.Name,roles);
            
            btnLoadAlbums.IsEnabled = true;
            btnLogout.IsEnabled = true;
            btnLogin.IsEnabled = false;
        }

        void authSrv_LoggedOut(object sender, AuthenticationEventArgs e)
        {
            btnLogout.IsEnabled = false;
            btnLoadAlbums.IsEnabled = false;
            btnLogin.IsEnabled = true;
        }

        private void btnLoadAlbums_Click(object sender, RoutedEventArgs e)
        {   
            // Login olunduktan sonra Album listesinin yüklenmesi aşamasına geçilir.
            // DomainService üzerinde yer alan GetAlbums operasyonundaki role gerekliliği nedeni ile sadece Finance rolü için yükleme yapıldığı görülür. Aksi durumda ise bir çalışma zamanı hatası oluşacak ve script error olarak tarayıcı üzerinde görülecektir.
            LoadOperation<Album> op = context.Load<Album>(context.GetAlbumsQuery(1));
            grdAlbums.ItemsSource = op.Entities;
        }
    }
}
```

Aslında istemci tarafında yapılan tek şey kullanıcının Login ve Logout olmasını sağlayacak operasyonlar ile örnek olması açısından 1 numaralı şarkıcıya ait albüm listesinin çekilmesini sağlamaktır. Dikkat edileceği üzere yetkilendirme kontrolü istemci tarafında yapılamamaktadır. Bu kontrol sunucu tarafında çalışma zamanı tarafından ele alınan Domain Service sınıfı üzerinden ele alınmaktadır. Buna göre Login olan geçerli kullanıcının Finance rolünde olmaması halinde albüm bilgilerini getirememesi gerekmektedir ki bu gerçektende böyledir. İşte yetkisiz bir kullanıcının albümleri yüklemek istemesi halinde çalışma zamanında oluşacak durum;

![blg115_Error.gif](/assets/images/2010/blg115_Error.gif)

Hata mesajında yer alan Access Denied kelimeleri olayı tüm çıplaklığıyla özetlemektedir. Diğer yandan senaryomuza göre Finance rolünde yer alan bill isimli kullanıcı ile Login olunup albüm listesi çekilmek istendiğinde, bilgilerin başarılı bir şekilde DataGrid kontrolüne çekildiği gözlemlenir. Aynen aşağıdaki şekilde görüldüğü gibi.

![blg115_AuthorizationOk.gif](/assets/images/2010/blg115_AuthorizationOk.gif)

Tabi bu yazımızda yetkilendirme nedeniyle oluşan istisna durumu ele kontrol altına alınmamıştır. Uygulama bu istisna ile karşılaştığında sonlandırılmaktadır ve hata mesajı script error olarak tarayıcı uygulama üzerinden yakalanmaktadır. Ancak en basit haliyle attribute bazlı olaraktan yetkilendirme işlemi sunucu tarafında yer alan servisler kanalıyla gerçekleştirilebilmiştir. Bu noktada vurgulanması gereken durumlardan biriside kendi Authorization niteliklerimizi (Attribute) yazabileceğimizdir. Söz gelimi role göre değil ama başka bir kritere göre yetkilendirme yapmak isteyebiliriz. Bu durumu ilerleyen yazılarımızda ele almaya çalışıyor olacağım. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
