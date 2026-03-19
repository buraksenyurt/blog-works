---
title: "WCF RIA Services - Authentication Domain Service - Profile ve Role"
pubDate: 2010-01-20 16:10:00
categories:
  - WCF Eco System
  - WCF RIA Services
tags:
  - WCF Eco System
  - WCF RIA Services
  - XML
  - C#
  - .NET
  - ASP.NET
  - LINQ
  - WCF
  - Silverlight
  - XAML
  - HTTP
  - Authentication
---

# WCF RIA Services - Authentication Domain Service - Profile ve Role
![blg114_First.jpg](images/blg114_First.jpg)

Merhaba Arkadaşlar,

Yandaki resmin bir renk cümbüşü oluşturup sizlere çok güzel göründüğüne eminim. Hatta bu resmin biraz sonra anlatacağımız konu ile olan ilgisini merak ediyor olabilirsiniz. Ne yazıkki yok. Sadece renk cümbüşünün benide etkilediğini ve yazının hoş görünmesi için eklediğimi itiraf edebilirim. Gelelim asıl mevzumuza.

[Bir önceki yazımızda](../2009/WCF RIA Services - Authentication Domain Service.md) Authentication Domain Service konusunu incelemeye başlamış ve RIA (Rich Internet Application) çeşitlerinden olan Silverlight uygulamalarında Form tabanlı doğrulamanın standart ASP.NET Membership kaynakları üzerinden nasıl sağlanabileceğini görmüştük. RIA uygulamaları ile ilişkili konulardan bir diğeride rol ve profil yönetimidir. WCF RIA Service'lerde kullanılan Authentication Domain Service'lerden yararlanarak Role ve Profile yönetimide yapılabilir. Çok doğal olarak WCF RIA Service'leri, Asp.Net mimarisinin rol ve profil alt yapısını kullanmaktadır.

Doğrulanan bir kullanıcının (Authenticated User), sistem içerisinde yapabileceklerini belirlerken rolüne bakılarak karar verilmesi tercih edilen yöntemlerdendir. Örneğin Administrator rolündeki bir kullanıcı ile Guest rolündeki bir kullanıcının sistem içerisinde yapılabilecekleri kuvvetle muhtemel farklıdır. Burada açık bir şekilde role göre yetkinin mertebesinin belirlendiğini düşünebiliriz. Diğer yandan sistem içerisinde yer alan tüm kullanıcılar için ortak tanımlanabilecek özellikler, çalışma zamanında farklı (bazende benzer, hatta aynı) değerler alarak, her kullanıcının sistem için bir profilinin oluşmasında kullanılabilirler. Çok doğal olarak bu profil özellikleri sistemden sisteme farklı şekillerde tanımlanabilir ve kullanılabilirler. Söz gelimi RIA uygulamasına dahil olan kullanıcıların ünvanları, doğum tarihleri, göz renkleri, cep telefonlarının gsm operatörleri, son giriş zamanları her kullanıcı için birer profil özelliği olarak değerlendirilebilir.

Bu yazımızda bir önceki örneğimizi devam ettirerek rol ve profil özelliklerinin nasıl değerlendirilebileceğini ele almaya çalışıyor olacağız. Temel amacımız rol ve profil bilgilerinin özellikle istemci tarafında nasıl kullanılabileceğini görmek olduğundan çok işe yarar bir örnek geliştirmeyeceğimizi şimdiden belirtmek isterim

![Wink](images/smiley-wink.gif)

Öncelikli olarak sunucu uygulama tarafında rol ve profil yönetimi için gerekli ayarları yapmamız gerekiyor. Bir önceki örneğimizde kullandığımız buraks ve bill isimli kullanıcıları sırasıyla Developer ve Administrator isimli rollere atadığımızı düşünerek devam edeceğiz. Hatta testlerimizi daha iyi yapabilmek adına bill isimli kullanıcının her iki rol atlında da bulunması sağladığımızı düşünelim. Bu şekilde istemci tarafına birden fazla rol bilgisinin nasıl aktarıldığını değerlendirme fırsatımız olacaktır. Bildiğiniz üzere rol atama işlemleri için ASP.NET Configuration aracını kullanabiliriz. Rol atama işlemlerinin ardından örneğimizde kullanacağımız bir kaç profil özelliğini tanımlayarak devam edebiliriz. Bu amaçla Web.config dosyamızın içeriğinde aşağıdaki değişiklikleri yaptığımızı düşünelim.

```xml
<configuration>
  <system.web>
    <httpModules>
      <add name="DomainServiceModule" type="System.Web.Ria.Services.DomainServiceHttpModule, System.Web.Ria, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" />
    </httpModules>
    <authentication mode="Forms" />
    <roleManager enabled="true" />
    <profile enabled="true">
      <properties>
        <add name="Title"/>
        <add name="LastAccessTime" type="System.DateTime"/>
      </properties>
    </profile>
    <compilation debug="true" targetFramework="4.0" />
  </system.web>
...
```

roleManager ve profile elementleri içerisinde enabled niteliklerine true değer atandığına dikkat edelim. Bu sayede RIA uygulamamızda role ve profile kullanımını etkinleştirmiş oluyoruz. profile elementi içerisinde yer alan properties alt elementi içerisinde ise Title ve LastAccessTime isimli özellik tanımlamalarının yapıldığını görmekteyiz. Özellikle LastAccessTime niteliği için DateTime bildiriminin de yapıldığına dikkat edelim. Nitekim profil özellikleri varsayılan olarak string tipindendir. Bu sebepten string harici tipleri bildirmemiz gerekmektedir.

Web.config dosyasında yapılan bu bildirimler ASP.NET tarafındaki role ve profile alt yapıları için gereklidir. Ne varki RIA uygulaması tarafında da ilgili profil özelliklerinin kullanımı için gerekli bildirimlerin yapılması gerekmektedir. Üstelik kullanıcının rol bilgilerinin istemci tarafında yer alan kod kısmında nasıl ele alınabileceği de şu anda soru işaretidir. Panik ve heyecan yapmadan sakin bir şekilde adım adım ilerleyelim. Öncelikli olarak OurAuthenticationService ismiyle oluşturduğumuz Authentication Domain Service dosyasını açalım ve User isimli tipin içeriğini aşağıdaki gibi düzenleyelim.

```csharp
using System;
using System.Web.Ria;
using System.Web.Ria.ApplicationServices;

namespace SilverlightApplication7.Web
{
    [EnableClientAccess]
    public class OurAuthenticationService 
        : AuthenticationBase<User>
    {
    }

    public class User 
        : UserBase
    {
        public string Title { get; set; }
        public DateTime LastAccessTime{ get; set; }
    }
}
```

Dikkat edileceği üzer profile elementi altında tanımlanan özellikler, User isimli tip içerisinde de property olarak bildirilmiştir. Buraya kadar yaptığımız işlemlerin ardından uygulamayı build ettiğimizde, istemci tarafında otomatik olarak üretilen sınıf içerisinde yer alan User tipininde aşağıdaki şekilde görüldüğü gibi oluşturulduğunu fark edebiliriz.

![blg114_AutoUserClassLast.gif](images/blg114_AutoUserClassLast.gif)

Dikkat edileceği üzere sunucu tarafında tanımladığımız Title, LastAccessTime isimli özellikler için istemci tarafındaki User sınıfı içerisinde de gerekli bildirimler yapılmıştır. Üstelik rol işlemleri içinde IEnumerable tipinden bir özellik (Roles) olduğu görülmektedir. Buna göre birden fazla rolün kod tarafında değerlendirilmesi mümkündür. Hatta LINQ ifadeleri ile Roles özelliği üzerinden sorgular atılabilir. Artık istemci tarafında rol ve profil işlemlerini değerlendirebilecek alt yapı hazırlıklarını tamamlamış bulunuyoruz. Şimdi MainPage.xaml içeriğini aşağıdaki gibi güncelleyerek yolumuza devam edebiliriz.

![blg114_MainPage.gif](images/blg114_MainPage.gif)

MainPage.xaml içeriği;

```csharp
<UserControl x:Class="SilverlightApplication7.MainPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    mc:Ignorable="d"
    d:DesignHeight="325" d:DesignWidth="420" xmlns:dataInput="clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Data.Input" xmlns:navigation="clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Navigation">
    <Grid x:Name="LayoutRoot" Background="White" Height="325">
        <dataInput:Label Height="28" HorizontalAlignment="Left" Margin="8,14,0,0" Name="label1" VerticalAlignment="Top" Width="120" Content="Username" />
        <dataInput:Label Height="28" HorizontalAlignment="Left" Margin="10,48,0,0" Name="label2" VerticalAlignment="Top" Width="120" Content="Password" />
        <TextBox Height="23" HorizontalAlignment="Left" Margin="134,14,0,0" Name="txtUsername" VerticalAlignment="Top" Width="197" />
        <PasswordBox Height="23" HorizontalAlignment="Left" Margin="134,50,0,0" Name="txtPassword" VerticalAlignment="Top" Width="197"/>            
        <Button Content="Login" Height="23" HorizontalAlignment="Left" Margin="337,14,0,0" Name="btnLogin" VerticalAlignment="Top" Width="75" Click="btnLogin_Click" />
        <dataInput:Label Height="28" HorizontalAlignment="Left" Margin="12,82,0,0" Name="lblLoginStatus" VerticalAlignment="Top" Width="196" />
        <Button Content="Logout" Height="23" HorizontalAlignment="Left" Margin="337,53,0,0" Name="btnLogout" VerticalAlignment="Top" Width="75" Click="btnLogout_Click" />
        <dataInput:Label Height="28" HorizontalAlignment="Left" Margin="214,82,0,0" Name="lblProcess" VerticalAlignment="Top" Width="198" />
        <Border BorderBrush="#FFFF3B00" BorderThickness="3" Height="185" HorizontalAlignment="Left" Margin="10,124,0,0" Name="brdProfile" VerticalAlignment="Top" Width="395" CornerRadius="10" Background="{x:Null}">
            <Canvas Height="170" Name="canvas1" Width="380">
                <dataInput:Label Canvas.Left="6" Canvas.Top="6" Height="13" Name="label3" Width="43" Content="Title" />
                <dataInput:Label Canvas.Left="8" Canvas.Top="40" Height="15" Name="label4" Width="101" Content="Last Access Time" />
                <TextBox Canvas.Left="56" Canvas.Top="7" Height="23" Name="txtUserTitle" Width="309" />
                <dataInput:Label Canvas.Left="116" Canvas.Top="42" Height="28" Name="lblUserLastAccessTime" Width="249" />
                <dataInput:Label Canvas.Left="10" Canvas.Top="83" Height="15" Name="label5" Width="39" Content="Roles" />
                <dataInput:Label Canvas.Left="60" Canvas.Top="83" Height="28" Name="lblRoles" Width="305" />
                <Button Canvas.Left="290" Canvas.Top="130" Content="Save Profile" Height="23" Name="btnSaveProfile" Width="75" Click="btnSaveProfile_Click" />
            </Canvas>
        </Border>
    </Grid>
</UserControl>
```

Tasarımın biraz fakir olmasına aldırmadan ilerlemeye devam edelim.

![Undecided](images/smiley-undecided.gif)

Senaryomuzu şu şekilde işletiyor olacağız; Kullanıcı Login işlemini başarılı bir şekilde gerçekleştirdiyse eğer, WebContext.Current.User özelliğinden elde edeceğimiz kullanıcı bilgilerini Border alanı içerisindeki kontrollerde göstereceğiz. Sembolik olarak Title ve LastAccessTime değerlerini ve dahil olduğu rollerin adlarını değerlendireceğiz. Sonrasında kullanıcı isterse Save Profile başlıklı düğmeye basaraktan yeni Title ve LastAccessTime bilgilerini kaydedebilecek. Bu basit senaryo için bir önceki yazımızda geliştirdiğimiz MainPage.xaml.cs içeriğini aşağıdaki gibi güncellememiz yeterli olacaktır.

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
            brdProfile.Visibility = Visibility.Collapsed;

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
            brdProfile.Visibility = Visibility.Collapsed;
        }

        //AuthenticationService nesnesine ait Login metodunun çalıştırılması sonrasında kullanıcı başarılı bir şekilde doğrulandıysa çalışır
        void authSrv_LoggedIn(object sender, AuthenticationEventArgs e)
        {
            lblLoginStatus.Content = String.Format("{0} {1} zamanında giriş yaptı", e.User.Identity.Name, DateTime.Now.ToLongTimeString());

            btnLogin.IsEnabled = false;
            btnLogout.IsEnabled = true;
            brdProfile.Visibility = Visibility.Visible;
            lblRoles.Content = String.Empty;
            txtUserTitle.Text = String.Empty;

            // Login işlemi tamamlandıktan sonra WebContext.Current üzerinden giriş yapan User bilgileri alınır
            Web.User currentUser = WebContext.Current.User;
            // Profil özelliklerinin değerleri ilgili kontrol özelliklerine atanır
            lblUserLastAccessTime.Content = currentUser.LastAccessTime.ToLongTimeString();
            txtUserTitle.Text = currentUser.Title;

            // Kullanıcının dahil olduğu tüm roller Label kontrolü içerisinde ardışıl olarak yazdırılır
            foreach (var role in currentUser.Roles)
            {
                lblRoles.Content += role + "|";
            }
        }

        private void btnSaveProfile_Click(object sender, RoutedEventArgs e)
        {
            // Save işleminde yine WebContext.Current üzerinden elde edilen User tipinin özelliklerinden yararlanılır
            Web.User currentUser = WebContext.Current.User;
            
            // Bu kez profile özelliklerine, kontroller üzerindeki değerler atanır
            currentUser.Title = txtUserTitle.Text;
            currentUser.LastAccessTime = DateTime.Now;

            // Kaydetme operasyonu için AuthenticationService nesne örneğinin SaveUser metodu çağırılır. Bu metodun çalıştırılması sırasında bir istisna olduğunda bunun ortama fırlatılması için parametre olarak true değeri verilmiştir.
            SaveUserOperation operation=authSrv.SaveUser(true);
            // Kaydetme operasyonu tamamlandığında SaveUserOperation tipinin Completed olay metodu devreye girer.
            operation.Completed += 
                (snd, arg) => 
                { 
                    lblProcess.Content = "Profil Save is OK"; 
                };
        }
    }
}
```

Hemen şu noktayıda vurgulayalım. İstenirse profil özelliklerinin kontrollere bağlanması sırasında Binding imkanlarından da yararlanılabilir. Bu durumda Login işlemini takiben otomatik olarak User bilgilerinin ilgili kontrollere bağlanması söz konusudur. Biz örneğimizde profil bilgilerinin gösterilmesi işlemlerini kod tarafında değerlendirmeye çalıştık. Ancak Binding işlemi ile aynı fonksiyonelliklerin uygulamaya nasıl kazandırılabileceğini incelemenizi öneririm. Dilerseniz uygulamamızın çalışma zamanını test ederek ilerleyelim. Ben bill isimli kullanıcı için daha önceden bir profil bilgisini test amacıyla kaydetmiştim. Bu durumda login işleminden sonra aşağıdaki görüntüye benzer sonuçlarla karşılaştım.

![blg114_FirstRun.gif](images/blg114_FirstRun.gif)

Şimdi profil bilgilerini değiştirip kaydettiğimizi düşünelim. Bu durumda Save Profile işleminin başarılı bir şekilde gerçekleştirildiğini görebiliriz.

![blg114_SecondRun.gif](images/blg114_SecondRun.gif)

Bu işlemin ardından tekrar Logout olup yeniden Login işlemini gerçekleştirirsek (ki uygulamayı kapatıp yeniden başlatmakta söz konusu olabilir) bill isimli kullanıcı için az önce kaydedilen profil bilgilerinin getirildiğini görebiliriz. Buda çalışmanın başarılı olduğunu bir ispatı olarak düşünülebilir.

![blg114_Proof.gif](images/blg114_Proof.gif)

Böylece geldik bir yazımızın daha sonuna. Bu yazımızda WCF RIA Service - Authentication Domain Service hizmetini kullanaraktan Silverlight uygulamalarında Role ve Profile alt yapılarının nasıl değerlendirilebileceğini incelemeye çalıştık. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[SilverlightApplication7RoleAndProfile.rar (780,46 kb)](assets/SilverlightApplication7RoleAndProfile.rar) [Dosya boyutunun küçük olması için ASPNETDB.mdf içeriği çıkartılmıştır]
