---
title: "Client Application Services (İstemci Uygulama Servisleri)"
pubDate: 2008-12-16 12:00:00
categories:
  - WCF
tags:
  - WCF
  - C#
  - .NET
  - ASP.NET
  - WPF
  - Windows Forms
  - SOAP
  - JSON
  - Web Service
  - HTTP
  - IIS
  - Authentication
  - Java
  - JavaScript
  - Threading
  - Visual Studio
  - ASMX
---

# Client Application Services (İstemci Uygulama Servisleri)
Pek çok istemci uygulama için önem arz eden konular arasında doğrulama (Authentication), Rol Yönetimi (Roles Management), profile (Profile) göre kişiselleştirme yer almaktadır. Özellikle Web tabanlı uygulamalarda bu kıstaslar daha çok ön plana çıkmaktadır. Nitekim Client/Server mimarinin en güzel uyarlamalarından birisi olan web tabanlı geliştirmelerde, istemcilerin doğrulanması, rollerine göre ne yapabileceklerinin belirlenmesi, profillerine göre istekte bulundukları sayfaların kişiselleştirilmesi önemlidir. Bu noktada Asp.Net 2.0 sürümünden itibaren saymış olduğumuz bu kriterlerin çok daha kolay bir şekilde uygulanabilmesi sağlanmıştır.

Hatırlanacağı üzere Asp.Net 2.0 Web Site Administration Tool veya kod tarafında Membership API içerisinde yer alan tipler yardımıyla, kullanıcı hesaplarının yönetilmesi, çeşitli rollere atanması görsel olarak kolayca yapılabilmektedir. Bununla birlikte Profile API içerisinde yer alan tipler yardımıylada, bir web sitesinin kullanıcı bazında özelleştirilebilmesi son derece kolaylaşmıştır. Ancak bu noktada söz konusu kriterlerin windows tabanlı istemciler (Windows Clients) açısındanda değerlendirilebilir olması oldukça kıymetlidir. İşte tam bu noktada söz konusu kriterlerin servis haline getirilmesi gerekliliği ortaya çıkmaktadır.

Nitekim authentication, role management, profile gibi kıstaslar aslında servis haline getirilirlerse Asp.Net Web uygulamaları dışında da kullanılabilir hale gelirler. İşte bu günkü makalemizin konusu olan İstemci Uygulama Servisleri (Client Application Services) ile,.Net tabanlı Windows uygulamalarının (Windows Presentation Foundation-WPF yada Windows Forms) söz konusu kriterleri,.Net içerisine gömülü olan Asp.Net Uygulama Servisleri (Asp.Net Application Services) üzerinden gerçekleştirebilmeleri mümkündür.

> Asp.Net Application Service'ler olmasada windows istemcilerinin doğrulama, rol ve profil yönetimi için servis bazlı mimarilerinin ele alması mümkündür. Sonuç itibariyle burada basit servisler yazılarak bu ihtiyaçlar karşılanabilir. Ancak Asp.Net Application Service'ler Framework içerisine gömülü olduklarından tüm istemci çeşitleri için bir standardizasyon getirmektedir. Bununla birlikte Visual Studio 2008 sürümünde gelen bir kaç basit yenilikte servislerin kullanımını kolaylaştırmaktadır.

Asp.Net Uygulama Servisleri (Asp.Net Application Services) sadece Asp.Net AJAX web uygulamalarında değil,.Net tabanlı her hangibiri istemci tarafından ele alınabilir. Bu noktada her iki istemci uygulama çeşidide JSON (JavaScript Object Notation) tabanlı bir mesaj iletişimini doğal olarak HTTP üzerinden gerçekleştirmektedir. Ne varki Asp.Net Uygulama Servisleri, SOAP 1.1 (Simple Object Access Protocol) temelli mesaj gönderen istemcilerede hizmet verebilmektedir. Buda çok doğal olarak.Net dışındaki uygulamaların söz konusu servisleri kullanabilmesi anlamına gelmektedir. Bir başka deyişle örneğin Java tabanlı bir uygulama bile Asp.Net Uygulama Servislerini çağırabilir ve doğrulama (Authentication), rol ve profile yönetimlerini (Role and Profile Management) kullanabilir. İstemci çeşitleri ve servisler arasındaki ilişkiler basitçe aşağıdaki şekil ile özetlenebilir.

![mk264_1.gif](images/mk264_1.gif)

Görüldüğü üzere uygulama servisleri aslında birer Web Servisi mantığında olduğundan HTTP üzerinden her tür istemcinin ulaşabilmesi mümkündür.(WCF bazlı bir kullanım şeklide mümkündür. Bunu ilerleyen makalelerimizde yada bir görsel dersimizde ele almaya çalışacağız) Bu servislerin temel işlevleri arasında istemcilerin doğrulanması, doğrulama sonrasında istemci tarafında biletler (ticket) açılmasının sağlanması, istemcinin hangi rolde olduğunun tespit edilmesi ve profiline göre uygulamanın içeriğinin kişiselleştirilmesi sayılabilir.

Dikkat edileceği üzere söz konusu hizmetler için bir veri saklama ortamı olması şarttır. Varsayılan olarak bu ortam bilindiği üzere SQL sunucusu (veya Express sürümü) üzerindeki Membership veritabanlarıdır. Ancak istenirse bu veri kaynaklarını kullanan provider'lar servis tarafında özelleştirilebilir ve farklı depoların kullanılması sağlanabilir. Bunun için basitçe konfigurasyon içeriğinde bir kaç değişiklik yapmak yeterli olacaktır. Servislerin temel görevleri aşağıdaki tabloda olduğu gibi özetlenebilir.

Servis
Görevi

Authentication Service (Doğrulama Servisi)
İstemcinin doğrulanması ve uygulamaya giriş yapabilmesi (Login) amacıyla kullanılır. Login işlemi sonrasında istemci tarafında saklanacak bir bilet (Ticket) oluşturulur. Tahmin edileceği üzere bu bilet bir cookie olarak depolanır ve bir geçerlilik süresi vardır.

Roles Service (Rol servisi)
Uygulama bazlı olaraktan istemcinin hangi rolde olduğunun Asp.Net Role Provider tarafından denetlenmesi hizmetini üstlenir. Buna göre istemci uygulamada, örneğin menülerin veya kontrollerin rol bazlı olarak kullanılabilmesi sağlanabilir.

Profile Service (Profil Servisi)
İstemci uygulamanın sunucu üzerinde tutulan kullanıcı verilerine göre herhangibir zamanda farklı biçimlerde gösterilebilmesine veya davranış sergilemesine hizmet eden servistir. Burada servis tarafında tüm kullanıcılar için ortaklaşa tanımlanmış profil özellikleri söz konusudur. Ancak özellik değerleri her istemci için farklı olarak tutulabilmektedir.

Bu genel açıklamalardan sonra sanıyorumki uygulama servisleri hakkında biraz fikir sahibi olunmuştur. Makalemizin bundan sonraki bölümünde yer alan hedefimiz ise bir windows istemcisi üzerinden söz konusu servislerin kullanılmasını sağlamaktır. Bu noktada Visual Studio 2008 sürümünün İstemci Uygulama Servisilerinin (Client Application Service) ele alınmasında büyük kolaylıklar sağladığıda unutulmamalıdır. Öyleyse hiç vakit kaybetmeden işe başlayalım. Öncelikli olarak doğrulama, rol ve profil yönetimi hizmetlerini üstlenecek bir web servisi geliştiriyor olacağız.

İlk olarak bir Web Service Application projesi oluşturarak başlayabiliriz. Bu uygulama içerisinde varsayılan olarak gelen Service.asmx dosyasının söz konusu senaryoda herhangibir kullanım alanı bulunmamaktadır. Servisin tek görevi istemci uygulamaya Authentication, Roles ve Profile servis hizmetlerini sağlamaktır. Bu sebepten asmx dosyası silinebilir. Şu an için test servisimiz file-based olarak açılmıştır. Ancak tabiki production ortamlarında IIS gibi bir sunucu altında yayınlanması önerilir. File-based kullanım nedeniyle Web Servis uygulamasının özelliklerinden basit olarak port ve sanal yol (Virtual Path) ayarlamalarını aşağıdaki şekildeki gibi yapmamız yeterli olacaktır.

![mk264_2.gif](images/mk264_2.gif)

Burada belirtilen bilgiler istemci uygulamada ele alınacaktır. Şimdi web.config dosyası içerisinde bazı basit ayarlamalar yapılması gerekmektedir. Bu amaçla web.config dosyası içeriğini şimdilik aşağıdaki gibi değiştirelim.

![mk264_3.gif](images/mk264_3.gif)

Konfigurasyon dosyası içeriğinde authentication, role ve profile servislerini etkinleştirmek için system.web.extensions elementi altında bazı tanımlamalar yapılmıştır. Bununla birlikte provider bazında role ve profile yönetimlerininde etkinleştirilmesi amacıyla roleManager ve profile elementleri ele alınmaktadır. Bu basit ayarlamaların hemen ardından kullanıcı ve rol tanımlamalarının yapılmasına başlanabilir. Bunun için web servisi uygulamasında Web Site Administration Tool'dan yararlanılabilir.

> Örneğin geliştirildiği makinede yer alan machine.config (C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\CONFIG) içeriğinde LocalSqlServer isimli connection string bilgisi aşağıdaki gibidir.
> ![mk264_4.gif](images/mk264_4.gif)
> Bu nedenle Asp.Net Web Site Administration Tool, web servisi uygulamasının içerisinde membership yönetimi için gerekli veritabanını (Aspnetdb.mdf) oluşturmaktadır. Elbetteki LocalSqlServer değerinin web servisi uygulamasının web.config dosyası içerisinde ezilmesi ve farklı bir lokasyonun işaret edilmeside sağlanabilir. Eğer makinedeki sunucu üzerinde veritabanının oluşturulması istenirse aspnetregsql komut satırı aracından yararlınmalıdır. Veritabanı oluşturulduktan sonra ise config dosyası içeriğinde oluşturulan veritabanı adresi işaret edilerek devam edilebilir.

Asp.Net Web Site Administration Tool üzerinde From Internet seçeneği ile Form Based Authentication açılır. Testler için iki farklı rol ve kullanıcı oluşturulur. Bu kullanıcılara ait bilgileri örnek senaryomuzda aşağıdaki gibi tanımlayabiliriz.

Kullanıcı
Şifre
Email
Gizli Soru
Gizli Cevap
Rol

buraks
buraks1234.
buraks@azon.com
buraks
buraks
Yonetici

bili
bili1234.
bili@azon.com
bili
bili
Calisan

Bu işlemlerin arkasından AppData klasörü altında aspnetdb.mdf veritabanı dosyasının açıldığı ve yukarıdaki kullanıcı bilgilerinin ilgili tablolara eklendiği görülebilir.

Artık windows tabanlı istemci uygulamanın geliştirilemesine başlanabilir. İstemci uygulama açısından en önemli nokta elbetteki yukarıda tanımlanmış olan web servisine erişilebilmesidir. Bu amaçla windows uygulamasının özelliklerinde aşağıdaki ekran görüntüsünde yer alan ayarların yapılması gereklidir.

![mk264_5.gif](images/mk264_5.gif)

İlk olarak projenin özelliklerinden Services kısmına geçilir. Burada Enable client application services seçeneği işaretlenmelidir. Sonrasında ise Authentication, Roles ve Profile servisleri için erişim adresleri belirtilir. Dikkat edileceği üzere her üç servis içinde SecurityService isimli web servis uygulamasının erişim bilgileri verilmektedir.

> Servis bildirimlerinde her bir kıstas için ayrı adresler verilebilmektedir. DolayısıylaDoğrulama, rol ve profile yönetimi görevlerini üstlenen ve farklı lokasyonlarda duran 3 farklı servisin tanımlanması mümkündür.

Windows uygulamasında basit bir giriş formu yer almaktadır. Bu form üzerinden kullanıcı bilgileri girilmekte ve servise gönderilmektedir. İşte bu noktada önemli bir referansa ihtiyacımız vardır. İstemci uygulamanın System.Web.dll assmebly'ını referans etmesi gereklidir. Nitekim ilk örneğimizde kullanacığımız Membership sınıfı bu assembly içerisinde yer alan System.Web.Security isimalanında (Namespace) yer almaktadır.

![mk264_6.gif](images/mk264_6.gif)

```csharp
using System;
using System.Windows.Forms;
using System.Web.Security;

namespace ClientApp
{
    public partial class LoginForm 
        : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            txtSifre.PasswordChar = '*';
        }

        private void btnGiris_Click(object sender, EventArgs e)
        {
            try
            {
                if (Membership.ValidateUser(txtKullanici.Text, txtSifre.Text))
                {
                    Form1 frm = new Form1();
                    frm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Giriş yetkiniz yok", "Yetkisiz Erişim", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Application.Exit();
                }
            }
            catch (Exception excp)
            {
                MessageBox.Show(excp.Message);
                Application.Exit();
            }
        }
    }
}
```

Giriş formunda kullanıcı adı ve şifre bilgisi istenmektedir. Sonrasında ise Membership sınıfının static ValidateUser metodu ile kullanıcı servis üzerinden doğrulanmaya çalışılmaktadır. ValidateUser metodu true veya false değer döndürmektedir. True dönmesi halinde kullanıcı bilgisi doğrulanmıştır. Windows uygulaması çalıştırıldığında, 4500 numaralı port üzerinden Asp.Net Development Server'ında çalıştığı gözlemlenir. Doğru kullanıcı bilgisi girildiğinde formun açıldığı ancak hatalı bilgi girilmesi halinde ise programın kapandığı gözlemlenebilir.

Örnekte dikkat edilmesi gereken noktalardan biriside istemci tarafında herhangibi proxy tipinin bilinçli bir şekilde fiziki olarak oluşturulmayışıdır. Normal şartlarda özellikle windows tabanlı istemcilerin, servisleri kullanabilmeleri için servise ait proxy tiplerine ihtiyaçları vardır. Oysa Uygulama Servislerinin ele alınmasında esas olan nokta mesaj alış verişidir.

Şimdi örneğimizi biraz daha geliştireceğiz. Bu sefer Credential Provider kullanaraktan bir Login işlemi gerçekleştirmeye çalışacağız. Az önceki örnekte LoginForm isimli bir form tasarlamıştık. Bu form içerisindeki düğmeye bastığımızda Membership sınıfının static ValidateUser metodunu kullanıyorduk. Uygulamanın ana formuna geçiş yapmadan önce bu formun çıkmasını sağlamak içinse Main metodu içerisinde aşağıdaki kodlamayı kullanmıştık.

```csharp
Application.Run(new LoginForm());
```

Crendentail Provider kullanaraktan aslında uygulamaya giriş yapıldığı sırada otomatik olarak Login formunun gösterilmesi sağlanabilir. Olayı daha kolay kavramak için örnek üzerinde adım adım ilerleyelim. Bu amaçla LoginForm üzerinden bazı değişiklikler yapmamız gerekmektedir. İlk olarak formumuzun kod yapısını aşağıdaki gibi düzenlemeliyiz.

```csharp
using System;
using System.Windows.Forms;
using System.Web.Security;
using System.Web.ClientServices.Providers;

namespace ClientApp
{
    public partial class LoginForm
        : Form, IClientFormsAuthenticationCredentialsProvider
    {
        public LoginForm()
        {
            InitializeComponent();
            txtSifre.PasswordChar = '*';
            FormBorderStyle = FormBorderStyle.FixedSingle;
            btnGiris.DialogResult = DialogResult.OK;
            btnIptal.DialogResult = DialogResult.Cancel;
        } 

        #region IClientFormsAuthenticationCredentialsProvider Members

        public ClientFormsAuthenticationCredentials GetCredentials()
        {
            if (this.ShowDialog() == DialogResult.OK)
            {
                return new ClientFormsAuthenticationCredentials(txtKullanici.Text, txtSifre.Text, chkHatirla.Checked);
            }
            else
            {
                return null;
            }
        }
    
        #endregion
    }
}
```

Dikkat edilecek olursa LoginForm sınıfına IClientFormsAuthenticationCrendentialsProvider arayüzü (Interface) uygulanmaktadır. Bu arayüz içerisinde ClientFormsAuthenticationCredentials tipinden nesne örneği döndüren GetCrendentials isimli metod bildirimi yer almaktadır. ClientFormsAuthenticationCredentials sınıfının oluşturulması sırasında parametre olarak kullanıcı adı ve şifre bilgileri gönderilmektedir. Peki bu form uygulama içerisinde otomatik olarak nasıl kullanılacaktır. Bir başka deyişle uygulama çalıştığında LoginForm otomatik olarak nasıl başlatılacaktır. Bunun için projenin özelliklerinde aşağıdaki ekran görüntüsünde olduğu gibi küçük bir bildirim yapılması yeterlidir.

![mk264_7.gif](images/mk264_7.gif)

Burada opsiyonel bir Crendential Provider tanımlaması yapılmaktadır ve LoginForm tipi işaret edilmektedir. Dolayısıyla uygulama başlatıldığında LoginForm tipinin Crendential Provider olarak kullanılacağı belirtilmektedir. Bu işlemin hemen ardından Form1 üzerinde Load metoduna aşağıdaki kodlamalar yapılabilir.

```csharp
using System;
using System.Windows.Forms;
using System.Web.Security;
using System.Net;

namespace ClientApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Membership.ValidateUser(String.Empty, String.Empty))
                {
                    MessageBox.Show("Giriş Yetkiniz Yok", "Yetkisiz giriş", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Application.Exit();
                }
            }
            catch (WebException excp)
            {
                MessageBox.Show(excp.Message);
                Application.Exit();
            }
        }
    }
}
```

Dikkat edilecek olursa Membership.ValidateUser metod çağrısında String.Empty parametre değerleri kullanılmıştır. Bu son derece doğaldır nitekim bu noktada devreye LoginForm sınıfı girecektir. Bir başka deyişle durum uygulama breakpoint'ler yardımıyla debug modda izlendiğinde, Form1Load'daki ValidateUser çağrısına gelindikten sonra LoginForm'un oluşturulduğu ve modal bir pencere olarak çalıştığı gözlemlenebilir.

Uygulama test edildiğinde doğru kullanıcı bilgileri girilmesi halinde ana formun (Form1) açıldığı görülür. Bununla birlikte yanlış kullanıcı bilgileri girilmesi halinde LoginForm'un varsayılan olarak 3 hak tanıdığı görülecektir. 3ncü giriştede başarı sağlanamadığı takdirde yine program sonlanacaktır. Beni Hatırla başlıklı CheckBox kontrolü işaretlendiği takdirde, sonraki açılışlarda uygulamanın kullanıcı bilgilerini sormadığı görülecektir. Yani kullanıcı bileti istemci tarafında belirli süreliğine saklanacaktır. (Bu noktada size güzel bir araştırma konusu sunabiliriz. Saklama halinde istemci bilgisi ne kadar süre ile tutulur. Bu süre nasıl özelleştirilebilir?) Aslında şu noktada web uygulamalarında gerçekleştirdiğimiz Forms Authentication bazlı kullanıcı yönetiminin Windows versiyonunu yazmış bulunuyoruz.

Şimdi windows uygulamamızda role bazlı işlemler yapmaya çalışacağız. Örneğin Form1 üzerinde bulunan iki button bileşeninin sadece ilgili rollerde görünmesini sağlayabiliriz.

![mk264_8.gif](images/mk264_8.gif)

Söz gelimi Yonetici rolünde olan bir kullanıcı sadece Muhasebe işlemleri yapabilecekken, Calisan rolündeki bir kullanıcı ise sadece BI işlemlerini yapabilecektir. Bunun için örnek olarak Form1Load metodu içerisinde aşağıdaki kodlamaları yapmamaız yeterlidir.

```csharp
using System;
using System.Windows.Forms;
using System.Web.Security;
using System.Net;
using System.Web.ClientServices.Providers;
using System.Threading;
using System.Security.Principal;

namespace ClientApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Membership.ValidateUser(String.Empty, String.Empty))
                {
                    MessageBox.Show("Giriş Yetkiniz Yok", "Yetkisiz giriş", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Application.Exit();
                }
                else
                {
                    IPrincipal principal = Thread.CurrentPrincipal;
            
                    if (principal.IsInRole("Yonetici"))
                    {
                        Text = String.Format("{0} ({1})", principal.Identity.Name, "Yönetici");
                        btnMuhasebe.Visible = true;
                        btnBI.Visible = false;
                    }
                    else if (principal.IsInRole("Calisan"))
                    {
                        Text = String.Format("{0} ({1})", principal.Identity.Name, "Çalışan");
                        btnMuhasebe.Visible = false;
                        btnBI.Visible = true;
                    }
                }
            }
            catch (WebException excp)
            {
                MessageBox.Show(excp.Message);
                Application.Exit();
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            ClientFormsAuthenticationMembershipProvider prvd =(ClientFormsAuthenticationMembershipProvider)System.Web.Security.Membership.Provider;
        
            try
            {
                prvd.Logout();
                Application.Restart();
            }
            catch (WebException excp)
            {
                MessageBox.Show(excp.Message);
            }
        
        }
    }
}
```

Kullanıcının hangi rolde olduğunu öğrenmek için Thread sınıfı üzerinden CurrentPrincipal referansına gidilir. CurrentPrincipal, IPrincipal arayüzü tipinden bir refernas taşımaktadır ve IsInRole metodu yardımıyla çalışmakta olan uygulamanın sahibi olan kullanıcının parametre olarak belirtilen rolde olup olmadığı öğrenilebilir. Buna göre windows uygulamasının rol bazlı özellikleri ayarlanabilir. Form1 içerisinde ayrıca Logout işlemi içinde bir kod eklentisi yer almaktadır. Nitekim LoginForm üzerinde Beni Hatırla başlıklı CheckBox tıklandığı takdirde uygulama kullanıcıyı her açılışta hatırlayacaktır. Logout olmak istendiği durumlarda ClientFormsAuthenticationMembershipProvider tipine ulaşmak ve Logout metodunu çağırmak yeterlidir. Uygulama tekrar test edildiğinde önreğin buraks isimli kullanıcı ile giriş yapıldığında aşağıdaki ekran görüntüsü ile karşılaşılır.

![mk264_9.gif](images/mk264_9.gif)

Benzer şekilde bili isimli kullanıcı ile girildiğinde ise aşağıdaki ekran görüntüsü ile karşılaşılır.

![mk264_10.gif](images/mk264_10.gif)

Son olarak makalemizde profil servisinin nasıl kullanılabileceğini ele almaya çalışalım. Burada amaç sunucu tarafında tanımlanan özelliklerin (Properties) her kullanıcı için farklı veriler tutacak şekilde tasarlanmasıdır. Böylece doğrulanan her kullanıcı, aynı isimli özelliğin farklı değerlerini sunucu üzerinde saklayabilir ve kendi istemci uygulaması üzerinde kullanabilir. Bu bir anlamda web uygulamalarında sıklıkla kullanılan kişiselleştirmedir (Personalization). Aynı hizmet İstemci Uygulama Servisleri sayesinde, windows tabanlı programlara da uygulanabilir. Dilerseniz bu basit işlemi nasıl yapabileceğimize bakalım. Öncelikli olarak sunucu tarafında yer alan web.config dosyası içerisinde basit bir kaç bildirimi aşağıdaki gibi yapmamız gerekmektedir.

![mk264_11.gif](images/mk264_11.gif)

Dikkat edeceğiniz üzere profile elementi altında yer alan properties alt boğumu (Child Node) içerisinde iki adet özellik bildirimi yapılmaktadır. GirisMesaji isimli özellik string tipindendir. Varsayılan bir değeri vardır. String formatta serileştirilmektedir. İsimsiz kullanıcılar (Anonymous Users) için bu özelliğe değer atanmasına izin verilmemektedir. Ayrıca yanlız okunabilir (readonly) bir özellikte değildir. Benzer kriterler SonGirisZamani isimli ikinci özellik içinde yer almaktadır. Ancak SonGirisZamani isimli özelliğin veri tipi DateTime'dır. profileService elementi içerisinde ise söz konusu özelliklere okuma ve yazma hakları, readAccessProperties ve writeAccessProperties nitelikleri (Attribute) ile verilmektedir. Burada birden fazla özelliğin aralarına virgüller konularak belirtilebildiğinde dikkat edilmelidir. Bu işlemler sonrasında sunucu tarafında her istemci için farklı değerlere sahip olabilecek özellik tanımlamaları yapılmış olmaktadır.

Şimdi istemci tarafında bazı işlemlerin yapılması gerekmektedir. Öncelikli olarak istemci uygulamada Properties->Settings kısmına geçilir ve Load Web Settings düğmesine basılır.

![mk264_12.gif](images/mk264_12.gif)

Bu işlemin ardından sunucu tarafındaki profile servisine bağlanılmak istenir ki var olan geçerli bir kullanıcı ile bu gerçekleştirilebilir.

![mk264_13.gif](images/mk264_13.gif)

Eğer bağlantı başarılı bir şekilde sağlanırsa, sunucu tarafındaki web.config dosyasında tanımlanan profil özelliklerinin istemci uygulamadaki settings kısmına eklendiği açık bir şekilde görülebilir.

![mk264_14.gif](images/mk264_14.gif)

Bir başka deyişle artık istemci tarafında GirisMesaji ve SonGirisZamani isimli özelliklere tip bazında erişilebilecektir. Kod tarafında, formun yüklenmesi ile birlikte bu özelliklerin değerleri okunabilir ve herhangibir amaçla kullanılabilir. Form kapanırken veya uygulamadan çıkılırkende istenirse, bu özelliklerin yeni değerlerinin set edilip kaydedilmesi, bir başka deyişle servis tarafındaki aspnetdb.mdf veritabanında yer alan aspnetProfile tablosuna yazdırılması sağlanabilir. Bu örneğimizde söz konusu senaryoyu çok basit bir şekilde değerlendirmeye çalışacağız. Örneğin Form1 in Load metodunu aşağıdaki gibi güncellediğimizi düşünelim.

```csharp
private void Form1_Load(object sender, EventArgs e)
{
    try
    {
        if (!Membership.ValidateUser(String.Empty, String.Empty))
        {
            MessageBox.Show("Giriş Yetkiniz Yok", "Yetkisiz giriş", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            Application.Exit();
        }
        else
        {
            txtGirisMesaji.DataBindings.Add("Text", Properties.Settings.Default, "GirisMesaji");
            lblSonGirisZamani.Text = Properties.Settings.Default["SonGirisZamani"].ToString();

            IPrincipal principal = Thread.CurrentPrincipal;
    
            if (principal.IsInRole("Yonetici"))
            {
                Text = String.Format("{0} ({1})", principal.Identity.Name, "Yönetici");
                btnMuhasebe.Visible = true;
                btnBI.Visible = false;
            }
            else if (principal.IsInRole("Calisan"))
            {
                Text = String.Format("{0} ({1})", principal.Identity.Name, "Çalışan");
                btnMuhasebe.Visible = false;
                btnBI.Visible = true;
            }
        }
    }
    catch (WebException excp)
    {
        MessageBox.Show(excp.Message);
        Application.Exit();
    }
}
```

Dikkat edilecek olursa txtGirisMesaji isimli TextBox kontrolüne veri bağlaması (DataBinding) sırasında Properties.Settings.Default veri kaynağı (Data Source) olarak gösterilmiştir. Son parametrede ise Settings kısmında tanımlı olan özellik adları verilmektedir. txtGirisMesaji isimli TextBox kontrolü için çift-yönlü veri bağlama (two-way databinding) söz konusudur. Böylece kontrol içerisinde veri değiştirildiğinde söz konusu değişiklik özellik tarafınada otomatik olarak yansıtılacaktır. lblSonGirisZamani isimli Label kontrolü ise tek yönlü (one-way) olarak SonGirisZamani özelliğine bağlanmıştır.

Form'dan çıkılırken bu verilerin servis tarafına gönderilmesi gerekir ki bir sonraki girişte son değerler kullanılabilsin. Bu durumda örneğin Form1Closing olay metodu içerisinde aşağıdaki kodlamalar yapılabilir.

```csharp
private void Form1_FormClosing(object sender, FormClosingEventArgs e)
{
    if (Thread.CurrentPrincipal.Identity.AuthenticationType.Equals("ClientForms"))
    {
        try
        {
            Properties.Settings.Default["SonGirisZamani"] = DateTime.Now;
            Properties.Settings.Default.Save();
        }
        catch(WebException excp)
        {
            MessageBox.Show(excp.Message);
        }
    }
}
```

İlk olarak kullanıcının ClientForms tipinde authentication gerçekleştirip gerçekleştirmediğine bakılır. Bir başka deyişle kullanıcının Client Application Services kullanarak Login olup olmadığı anlaşılmaya çalışılmaktadır. Eğer bu şekilde bir bağlantı sağlanmış ise SonGirisZamani özelliğinin değeri değiştirilir ve Save metodu çağırılır. Dikkat edileceği üzere GirisMesaji isimli profil özelliği için bir atama yapılmamıştır. Bunun sebebi iki yönlü veri bağlama işleminin txtGirisMesaji kontrolü için kullanılıyor olmasıdır.

Bu metodun çalışması sırasında bazı hatalar oluşabilir. Söz gelimi Save metodu çalıştığı sırada istemcinin cookie bilgisi silinmiş olabilir. Yada kullanıcı Logout olmuş durumdadır ancak halen daha uygulama içerisindedir ve Save metodu çağırılmıştır. Dolayısıyla işlemin gerçekleştirilebilmesi için tekrardan Login işleminin yapılması gerekmektedir. Diğer taraftan kayıt işlemi sırasında servis ile olan bağlantının kurulamama ihtimalide vardır. Bu gibi durumlar ele alınaraktan daha güçlü bir profil kaydetme süreci oluşturulabilir. (Burada nasıl bir tedbir alınabileceğinide bir araştırma konusu haline getirebilirsiniz) Artık uygulamayı bu haliyle test edebiliriz. Bilhassa aynı uygulamadan iki örnek başlatıp iki farklı kullanıcı ile girilmesini tavsiye ederim. Bu durumda her iki kullanıcı içinde farklı profil verileri tutulduğu gözlemlenebilir. Aşağıdaki test ekranlarında olduğu gibi.

![mk264_15.gif](images/mk264_15.gif)

Bu testin sonrasında sunucu tarafındaki aspnetdb.mdf veritabanında yer alan aspnetProfile tablosu içeriğine bakıldığında her iki kullanıcı içinde ilgili satırların oluşturulduğu görülebilir.

Böylece geldik bir makalemizin daha sonuna. Bu makalemizde İstemci Uygulama Servislerini (Client Application Service), Visual Studio 2008 ortamı üzerinde basit bir şekilde ele alarak, Windows tabanlı istemcilerde Authentication,Roles ve Profile hizmetlerinin nasıl ele alınabileceğini basit bir şekilde incelemeye çalıştık. Söz konusu kriterlerin birer servis olarak.Net Framework içerisine dahil edilmesi sonrasında herhangibir.Net istemcisinin bunları kullanabileceğini anladık. Burada.Net tabanlı istemcilerin JSON formatında mesajlaşma yaptığını ancak.Net dışı uygulamalarında SOAP 1.1 formatını kullanarak bu servisleri ele alabileceklerine değindik ki bunu ilerleyen makalelerimizde ele almaya çalışacağız.
