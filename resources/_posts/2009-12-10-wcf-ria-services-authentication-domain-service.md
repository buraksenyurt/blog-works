---
layout: post
title: "WCF RIA Services - Authentication Domain Service"
date: 2009-12-10 04:40:00 +0300
categories:
  - wcf-eco-system
  - wcf-ria-services
tags:
  - wcf-ria-services
  - .net-ria-services
  - windows-communication-foundation
  - wcf-eco-system
---
Bazen insanın yapmak zorunda olduğu bazı şeyler gözünde büyür. Örneğin havalimanlarında kontrollerden geçerek uçağa ulaşmak o daracı rahatsız koltuklara sığmak için çabalamak. Önce ilk kapıda bir güvenlik kontrollünden geçilir, ardından Check-In işlemi için kimlikle birlikte bir kontrolden daha geçilir (hatta bagajımız var ise tartılır ve gerekiyorsa ekstra para ödenir), ardından uçağa bineceğimiz kapılara gitmek için bir kontrolden daha geçilir, ardından uçağa binerken bilet ve kimlik ile kontrolden bir kere daha geçilir.

![blg113_Giris.jpg](/assets/images/2009/blg113_Giris.jpg)

Keşke insanoğlu daha barışçıl olsaymış dedirten aramalar ile karşılaşılır zaman içerisinde. Güvenlik kontrollerinin temel amaçlarından birisi de, gerçekten sizin kimliğinizde söylenen kişi olduğunuzu görmek ve hatta elinizdeki biletiniz ile söz konusu uçağa binebilecek yetkiye sahip olduğunu öğrenmektir. Tabi arada sıra Administrator seviyesinde pek çok insan güvenlik kontrollerine takılmadan protokol kapısından giriş yaparak yollarına devam edebilirler. Üstelik yürüdükleri hat üzerinde kırmızı halılarda bulunabilir.

![Wink](/assets/images/2009/smiley-wink.gif)

Özellikle bu son durumda, giriş yapan kişilerin rollerininde büyük önemi vardır.

Sözün özü bir noktaya güvenlik kontrolünden geçerek girmemiz gerektiğinde doğrulanma, yetki kontrolü, rol gibi faktörlerle karşılaşırız. Zaten yazılım dünyasında da üyelik tabanlı (Membership Based) olarak çalışan sistemlerde, kullanıcıların bazı şeyleri yapabilmesi için önce doğrulanmaları (Authenticate) gerekir. Buna ilaveten, doğrulanan kullanıcıların yetkilerine bakaraktan bir takım işlemleri yapıp yapmamalarına izin verilmesi (Authorization), hatta bu sırada rollerinin de değerlendirilmesi söz konusudur. Dahası doğrulanan ve yetkisi dahilinde bir yere ulaşan bireyin kendine has profilini tanımlayan özellikleride bulunabilir.

Web tabanlı uygulamalarda sıkça karşılaştığımız doğrulama (Authentication) işlemlerinin genellikle Form-Based veya Windows-Based olarak yapılabildiğini görürüz.(Hatta Microsoft Passport hizmetininde kullanılması mümkündür) Asp.Net 2.0 ile birlikte getirilen Membership alt yapısı sayesinde SQL üzerinde tutulabilen hazır üyelik sistemlerinden kolaylıkla yararlanabiliriz. Hatta ilk Asp.Net sürümünden bu yana, Active Directory sayesinde intranet tabanlı web uygulamalarında Windows Domain kullanıcılarını ve rollerini değerlendirebiliriz. Temel amaç aslında son derece açıktır; Gerçekten tanınan (Authenticated User) kullanıcıların belirli işlemleri yapabilmesi ve neler yapabileceklerine karar verilirken yetkilerine (Authorization) yada rollerine bakılabilmesi.

Sözü fazla uzatmadan WCF RIA Services tarafındaki duruma bakalım. Sonuç itibariyle RIA (Rich Internet Application) için sıklıkla değerlendirdiğimiz Silverlight uygulamalarınında, Asp.Net Membership veya Windows Domain yapılarından yararlanarak doğrulama, yetkilendirme ve rollendirme işlemlerini yapılabilmesi mümkündür. Bu amaçla WCF RIA Services'ler ile birlikte gelen Authentication Domain Service tipinden yararlanılmaktadır. Bu yazımızdaki amacımız da, çok basit anlamda bir RIA uygulamasında doğrulama işlemlerinin nasıl ele alınabileceğini incelemektir.

İşe ilk olarak Silverlight 4.0 ve WCR RIA Services destekli bir Sliverlight uygulaması oluşturarak başlamalıyız. Bilindiği üzere Silverlight çözümlerinde Web tabanlı bir sunucu ve Silverlight kontrollerini barındıran bir uygulama söz konusudur. Silverlight tarafında geliştirilen kullanıcı kontrollerinin host ediliği Web uygulaması Asp.Net tabanlı olduğundan pekala bir Membership sistemine sahip olabilir. Bu nedenle sunucu uygulama üzerinde Membership API'sini kullanarak işe başlamakta ve Form-Based Authentication'ı kullanacağımızı belirtmekte yarar vardır. Bildiğiniz üzere Membership işlemleri için, Web projesinin Asp.Net Configuration aracı kullanılabilir. Ben örneğimizde SQL Express Edition üzerinde konuşlandırılan Membership veritabanını kullanmayı tercih ettim ve şifreleri 123456. olan iki kullanıcı oluşturdum (buraks ve bill). Form-Based Authentication kullandığımız için web.config dosyasında yer alan authentication elementinin içeriğinin aşağıdaki gibi oluşturulması gerekmektedir (Tabi Asp.Net Configuration aracını kullanarak From Internet seçeneğini işaretlediğimizde bu ayar otomatik olarak web.config içerisine işlenecektir)

```xml
...
<system.web>
        <httpModules>
            <add name="DomainServiceModule" type="System.Web.Ria.Services.DomainServiceHttpModule, System.Web.Ria, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" />
        </httpModules>
        <!-- Form tabanlı doğrulama(Form-Based Authentication) kullanılacağı belirtilir.-->
        <authentication mode="Forms" />          
        <compilation debug="true" targetFramework="4.0" />
    </system.web>
...
```

Bu işlemin ardından yine sunucu uygulamaya Authentication Domain Service tipinden bir sınıf eklenmesi gerekmektedir.

![blg113_Item.gif](/assets/images/2009/blg113_Item.gif)

Örnekte OurAuthenticationService isimiyle eklediğimiz dosyanın içeriği otomatik olarak oluşturulacak ve aşağıdaki kod parçasında görüldüğü gibi olacaktır.

```csharp
using System.Web.Ria;
using System.Web.Ria.ApplicationServices;

namespace SilverlightApplication7.Web
{
    [EnableClientAccess]
    public class OurAuthenticationService : AuthenticationBase<User>
    {
        // To enable Forms/Windows Authentication for the Web Application, 
        // edit the appropriate section of web.config file.
    }

    public class User : UserBase
    {
        // NOTE: Profile properties can be added here 
        // To enable profiles, edit the appropriate section of web.config file.

        // public string MyProfileProperty { get; set; }
    }
}
```

Burada yer alan AuthenticationBase türevli sınıf doğrulama işlemleri sırasında istemci tarafından kullanılacak tiptir. Diğer yandan User isimli tip üyelere has profil özelliklerinin uygulanabilmesi için kullanılmaktadır. Söz gelimi, Membership API üzerinde tanımlanmış ve yetkisi olan bir kullanıcının Silverlight uygulamasını kullanmaya başlaması ile birlikte Title, Birthdate gibi geliştirici tanımlı bir takım bilgilerinin profil özelliği olarak kullanılabilmesi mümkündür. Tabi User tipi içerisinde profil özelliklerinin tanımlanması yeterli değildir.

Web.config dosyasında da aynı profil özelliklerinin bildirilmesi ve profil yönetiminin etkinleştirilmesi gerekmektedir. (Bu yazımızda geliştireceğimiz örnekte doğrulama işlemlerini çok basit bir seviyede anlamak istediğimizden profil özellikleri ile uğraşılmayacaktır). Yapılan ekleme işleminden sonra projenin Build edilmesi halinde istemci tarafında WebContext, User ve OurAuthenticationContext isimli tiplerin oluşturulduğu gözlemlenir. User sınıfına ait nesne örneği ile tahmin edileceği üzere sunucu tarafında tanımlanan User tipine erişilebilmesi mümkündür. Diğer taraftan WebContext tipi, istemci tarafından Authentication Domain Service'e erişilebilmesini ve User bilgisinin alınabilmesini sağlamaktadır. Buraya kadar ki kısımda sunucu tarafı için gerekli aşağıdaki işlemleri yaptığımızı söyleyebiliriz;

- Form Tabanlı doğrulama için Membership veritabanının oluşturulması ve buna bağlı olarak web.config dosyasında gerekli ayarların yapılması,
- Gerekiyorsa role seçeneğinin etkinleştirilmesi,
- Authentication Domain Service tipinin sunucu projeye eklenmesi,
- Gerekiyorsa User tipi için profil özelliklerinin kod ve web.config bazında oluşturulması

Ancak istemci tarafında, sunucu üzerindeki Authentication Domain Service hizmetinin kullanılabilmesi için bir takım işlemler yapılmalıdır. Öncelikli olarak App.Xaml içeriğinin aşağıdaki hale getirilerek Web.Context ve Form tabanlı doğrulamanın kullanılacağının belirtilmesi gerekir (Buradaki kodlamaların aslında ilerleyen sürümlerde otomatik hale getirileceğini ümit etmekteyim ![Wink](/assets/images/2009/smiley-wink.gif))

```xml
<Application xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" 
             xmlns:app="clr-namespace:SilverlightApplication7"
             xmlns:appSrv="clr-namespace:System.Windows.Ria.ApplicationServices;assembly=System.Windows.Ria"
             x:Class="SilverlightApplication7.App"
             >
    <Application.Resources>
        
    </Application.Resources>
    <!-- Aşağıdaki kısım Form-Based authentication kullanımı için eklenmelidir-->
    <Application.ApplicationLifetimeObjects>
        <app:WebContext>
            <app:WebContext.Authentication>
                <appSrv:FormsAuthentication />
            </app:WebContext.Authentication>
        </app:WebContext>
    </Application.ApplicationLifetimeObjects>    
</Application>
```

App.Xaml içerisinde yer alan tanımlamalar da yeterli değildir. WebContext'in XAML içerisinde kullanılabilir olması için App.Xaml.cs kod tarafında aşağıdaki ilavenin yapılması gerekmektedir.

```csharp
private void Application_Startup(object sender, StartupEventArgs e)
        {
            // XAML içerisinde WebContext nesnesinin kullanılabilmesi için Resources koleksiyonuna ilgili WebContext nesne örneğinin eklenmesi gerekir.
            this.Resources.Add("WebContext", WebContext.Current);
            this.RootVisual = new MainPage();
        }
```

Artık Silverlight kontrolünün içeriğinin tasarlanmasına başlanabilir. Örneğimizde Login ve Logout işlemlerinin yapılması irdelenecektir. Buna göre aşağıdaki tasarım görüntüsü ve XAML içeriğine sahip olacak şekilde MainPage.xaml dosyasını düzenleyerek ilerlediğimizi düşünelim.

![blg113_MainPage.gif](/assets/images/2009/blg113_MainPage.gif)

XAML İçeriği;

```xml
<UserControl x:Class="SilverlightApplication7.MainPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    mc:Ignorable="d"
    d:DesignHeight="202" d:DesignWidth="316" xmlns:dataInput="clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Data.Input">

    <Grid x:Name="LayoutRoot" Background="White">
        <dataInput:Label Height="28" HorizontalAlignment="Left" Margin="8,14,0,0" Name="label1" VerticalAlignment="Top" Width="120" Content="Username" />
        <dataInput:Label Height="28" HorizontalAlignment="Left" Margin="10,48,0,0" Name="label2" VerticalAlignment="Top" Width="120" Content="Password" />
        <TextBox Height="23" HorizontalAlignment="Left" Margin="134,14,0,0" Name="txtUsername" VerticalAlignment="Top" Width="170" />
        <PasswordBox Height="23" HorizontalAlignment="Left" Margin="134,50,0,0" Name="txtPassword" VerticalAlignment="Top" Width="170"/>            
        <Button Content="Login" Height="23" HorizontalAlignment="Left" Margin="12,92,0,0" Name="btnLogin" VerticalAlignment="Top" Width="75" Click="btnLogin_Click" />
        <dataInput:Label Height="28" HorizontalAlignment="Left" Margin="10,134,0,0" Name="lblLoginStatus" VerticalAlignment="Top" Width="294" />
        <Button Content="Logout" Height="23" HorizontalAlignment="Left" Margin="229,92,0,0" Name="btnLogout" VerticalAlignment="Top" Width="75" Click="btnLogout_Click" />
        <dataInput:Label Height="28" HorizontalAlignment="Left" Margin="10,170,0,0" Name="lblProcess" VerticalAlignment="Top" Width="294" />
    </Grid>
</UserControl>
```

Gelelim kod kısmına;

```csharp
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ria.ApplicationServices;

namespace SilverlightApplication7
{
    public partial class MainPage : UserControl
    {
        // Kullanıcının doğrulanması, profil özelliklerinin yüklenmesi ve kaydedilmesi için gerekli fonksiyonellikleri sunan tiptir.
        AuthenticationService authSrv = WebContext.Current.Authentication;

        public MainPage()
        {
            InitializeComponent();

            btnLogout.IsEnabled = false;
            
            authSrv.LoggedIn += new System.EventHandler<AuthenticationEventArgs>(authSrv_LoggedIn);
            authSrv.LoggedOut += new System.EventHandler<AuthenticationEventArgs>(authSrv_LoggedOut);
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            lblLoginStatus.Content = String.Empty;
            lblProcess.Content = String.Empty;

            if (!authSrv.IsLoggingIn) // Eğer asenkron olarak devam eden bir Login operasyonu yoksa
            {
                // Login işlemi için AuthenticationService tipinin Login metodu çağırılır.
                // ilk parametre ile kullanıcı adı ve şifre bilgisi gönderilir. Bu metodun aşırı yüklenmiş versiyonları mevcuttur.
                // ikinci parametrede Login metodunun işleyişini tamamlaması sonrası devreye giren metodun işaret edilmesi sağlanmaktadır(Action<LoginOperation> temsilci tipi ile işaretleme yapılır). Bu metod içerisinde işlemin iptali, exception üretmesi gibi durumlarda ele alınmaktadır.
                authSrv.Login(
                    new LoginParameters(txtUsername.Text, txtPassword.Password),
                    opt =>
                    {
                        if (opt.IsCanceled)
                            lblProcess.Content = "Login işleminde iptal";
                        else if (opt.Error != null)
                            lblProcess.Content = opt.Error.Message;
                        else if (opt.IsComplete)
                            lblProcess.Content = "Login işlemi tamamlandı";
                    }
                    , null
                    );
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            lblLoginStatus.Content = String.Empty;
            lblProcess.Content = String.Empty;

            if (!authSrv.IsLoggingOut) // Eğer asenkron olarak devam eden bir Logout operasyonu yoksa
            {
                // Logout işlemi kullanılan metod çağrısı
                // işlem tamamlandığında devreye girecek olan metod Action<LogoutOperation> temsilcisi ile işaret edilir.
                authSrv.Logout(
                   opt =>
                   {
                       if (opt.IsCanceled)
                           lblProcess.Content = "Logout işleminde iptal";
                       else if (opt.Error != null)
                           lblProcess.Content = opt.Error.Message;
                       else if (opt.IsComplete)
                           lblProcess.Content = "Logout işlemi tamamlandı";
                   }
                , null);
            }

        }

        // Kullanıcı başarılı bir şekilde Logout olduğunda tetiklenir
        void authSrv_LoggedOut(object sender, AuthenticationEventArgs e)
        {
            lblLoginStatus.Content = String.Format("{0} {1} zamanında çıkış yaptı", e.User.Identity.Name, DateTime.Now.ToLongTimeString());

            btnLogin.IsEnabled = true;
            btnLogout.IsEnabled = false;
        }

        //AuthenticationService nesnesine ait Login metodunun çalıştırılması sonrasında kullanıcı başarılı bir şekilde doğrulandıysa çalışır
        void authSrv_LoggedIn(object sender, AuthenticationEventArgs e)
        {
            lblLoginStatus.Content = String.Format("{0} {1} zamanında giriş yaptı", e.User.Identity.Name, DateTime.Now.ToLongTimeString());

            btnLogin.IsEnabled = false;
            btnLogout.IsEnabled = true;
        }
    }
}
```

Aslında bütün iş yükünü AuthenticationService nesne örneği almaktadır. Login ve Logout operasyonları için kullanılan bu nesne örneği üzerinden profil ve rol yönetimi için gerekli pek çok operasyonda gerçekleştirilebilir. Örneğin doğrulanan kullanıcı bilgilerine ulaşılabilir (User), kullanıcı bilgilerinin (özellikle profil bilgileri) değiştirilmesi, kayıt edilmesi veya yüklenmesi sağlanabilir. Aşağıdaki şekilde kullanılabilecek özellik ve üye metodlar görülmektedir.

![blg113_AuthenticationService.gif](/assets/images/2009/blg113_AuthenticationService.gif)

Uygulama açıldıktan sonra geçerli bir kullanıcı adı ve şifre ile Login işlemini yapmak istediğimizde aşağıdaki sonuçları elde ettiğimizi görebiliriz.

![blg113_Test1.gif](/assets/images/2009/blg113_Test1.gif)

Dikkat edileceği üzere kullanıcı başarılı bir şekilde tanınmıştır. Login olan kullanıcının Logout olması için gerekli işlemi yaptığımızda ise aşağıdaki sonuç ile karşılaşırız.

![blg113_Test1Logout.gif](/assets/images/2009/blg113_Test1Logout.gif)

Çok doğal olarak geçersiz bir kullanıcı adı veya şifre girişinde Login işleminin gerçekleştirilemediği görülecektir. Elbette bu yazımızdaki senaryomuzda sembolik olarak Login ve Logout operasyonlarının işlevsellikleri ön plana çıkartılmıştır. Dolayısıyla Login olunduktan sonra asıl işlemlerin yapılacağı sayfaya yönlenilmesi ve hatta rol kontrolüne göre ilerlenmesi işlemleri göz ardı edilmiştir. Oysaki gerçek hayat projelerinden bu işlemlerin ele alınması şarttır. Bu tip konuları belki bir görsel ders üzerinden incelemeye çalışabiliriz. Buraya kadar yaptıklarımıza baktığımzıda, istemci tarafında aşağıdaki temel işlemleri yaptığımızı özetleyebiliriz;

- App.Xaml ve App.Xaml.cs içerisinden Form tabanlı doğrulama servisinin kullanılması için gerekli ayarlar,
- Kod tarafında, AuthenticationService nesne örneğinin WebContext.Current ile elde edilmesi ve gerekli kodlamalar,

Görüldüğü üzere WCF RIA Service'ler ile birlikte gelen Authentication Domain Service tipini kullanarak, Silverlight formatlı RIA uygulamalarının, Asp.Net Web uygulamalarında sıklıkla ele alınan doğrulama (Authentication), yetkilendirme (Authorization), rol (Role) ve profil (Profile) yönetimi kabiliyetlerine sahip olması sağlanabilmektedir. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[SilverlightApplication7.rar (626,55 kb)](/assets/files/2009/SilverlightApplication7.rar) [Dosya Boyutunun küçük olması amacıyla, Memberhip için kullanılan ASPNETDB veritabanı silinmiştir]
