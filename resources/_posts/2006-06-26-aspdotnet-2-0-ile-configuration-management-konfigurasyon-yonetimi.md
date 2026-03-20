---
layout: post
title: "Asp.Net 2.0 ile Configuration Management (Konfigurasyon Yönetimi)"
date: 2006-06-26 12:00:00 +0300
categories:
  - aspnet-2-0
tags:
  - aspnet-2-0
  - xml
  - csharp
  - dotnet
  - aspnet
  - iis
  - authentication
  - authorization
---
Asp.Net 2.0 özellikle Web.config dosyasını daha kolay yönetebilmek amacıyla beraberinde bir yönetim API'si ile birlikte gelmektedir. Configuration Management API olarak adlandırabileceğimiz bu yapı sayesinde, web.config gibi konfigurasyon dosyalarını ve içeriğini programatik olarak daha etkin bir şekilde yönetebiliriz. Bildiğiniz gibi web.config dosyası xml tabanlı bir içerik sunmakla birlikte entegre edildiği web uygulaması için gerekli temel bir takım ayaları içerir. (Elbette bir web uyguması içerisinde yer alan alt klasörler içinde ayrı web.config dosyalarının ele alınabileceğini hatırlatmakta fayda var.)

AppSettings, ConnectionStrings, Authentication, Authorization, SessionState, Membership, Tracing, Pages gibi pek çok konu ile ilişkili kısımları bu ayarlar arasında düşünebiliriz. Burada saymış olduğumuz tüm kısımlar Configuration Management API içerisinde birer tipe karşılık gelmektedir. Örneğin Authentication ayarları için AuthenticationSection, SessionState ayaları için SessionStateSection gibi pek çok tip geliştirilmiştir. Bu, ilgili kısımların programatik olarak nesnel düzeyde kullanılabilmesi anlamına gelmektedir. Yani, Configuration Management API'si sayesinde, web.config içerisinde yer alan herhangibir kısmı (Section) nesne örneği olarak kullanabilir, içeriğini düzenleyebilir ve tekrardan kayıt edebiliriz. İşte bu makalemizde, basit olarak web.config içeriğini programatik olarak nasıl ele alabileceğimizi incelemeye çalışacağız.

Configuration Management API'si sadece web uygulamalarını esas almaz. Diğer uygulamalar (örneğin Desktop) için kullanılabilecek çeşitli uygulama düzeyindeki konfigurasyon dosyalarınıda ele alabiliriz. Temek olarak bir web.config dosyasını programatik seviyede Configuration sınıfına ait bir nesne örneği ile ele alabiliriz. Configuration Management API'si içerisinde, Configuration tipini elde edebilmemizi sağlayan iki temel sınıf vardır. Bunlar ConfigurationManager ve WebConfigurationManager sınıflarıdır. ConfigurationManager sadece web uygulamalarını değil diğer platformlarıda göz önüne alır. WebConfigurationManager sınıfı ise özellikle web tabanlı sistemler için tasarlanmış üyeler içerir.

ConfigurationManager sınıfı System.Configuration isim alanında, WebConfigurationManager sınıfı ise System.Web.Configuration isim alanında yer almaktadır. WebConfigurationManager tipinin belkide en önemli üyesi static OpenWebConfiguration metodudur. Bu metod sayesinde, web.config dosyalarını herhangibir seviyede açabiliriz. Diğer taraftan WebConfigurationManager sınıfı OpenMachineConfiguration isimli static bir üye metod daha içerir. Bu metod sayesinde de tahmin edebileceğiniz gibi, machine.config dosyasına erişilebilmektedir. Bu bizim makine seviyesindeki bir takım ayarları programatik olarak ele almamızı sağlar. Aşağıdaki tabloda bu metodlara ilişkin örnek bir takım senaryolar verilmiştir.

Örnek Metod
İşlevi

WebConfigurationManager.OpenWebConfiguration ("~/")
Güncel uyulamadaki web.config dosyasını alır.

WebConfigurationManager.OpenWebConfiguration ("/ConfigMngAPI")
ConfigMngAPI isimli Web uygulamasına ait olan web.config dosyasını açar.

WebConfigurationManager.OpenWebConfiguration ("/ConfigMngAPI/Admin")
ConfigMngAPI isimli web uygulamasındaki Admin klasörü altındaki web.config dosyasını açar.

WebConfigurationManager.OpenWebConfiguration ("~/", "siteAdi")
siteAdi ile belirtilen web uygulamasındaki web.config dosyasını açalar. Bu metodun diğer versiyonları kullanılarak ilgili siteye username ve password bilgiside iletilebilinir.

WebConfigurationManager.OpenWebConfiguration ("~/", null, "/Admin")
Var olan web uygulamasındaki Admin klasöründe için web.config'de yazılmış location boğumunu açar.

WebConfigurationManager.OpenMachineConfiguration ()
Framework altındaki machine.config dosyasını açar.

Tabloda belirtilen örnek kullanımlar çoğaltılabilir. Sonuç itibariyle OpenWebConfiguration metodunun altı farklı aşırı yüklenmiş versiyonu vardır. Buradaki önemli noktalardan biriside, OpenWebConfiguration ve OpenMachineConfiguration gibi static metodların geriye Configuration tipinden bir nesne örneği gönderiyor oluşlarıdır. Configuration sınıfına ait bu nesne örnekleri sayesinde açılan config uzantılı dosya üzerinde istediğimiz yönetimsel işlemleri (düzenleme, kaydetme, elde etme gibi) gerçekleştirebiliriz. Şimdi ilk olarak konfigurasyon dosyalarında sıkça kullandığımız appSettings ve connectionStrings kısımlarını nasıl yönetebileceğimizi göreceğiz. Bunun için web.config dosyasındaki appSettings ve connectionStrings kısımlarına aşağıda görüldüğü gibi örnek bir kaç veri gireceğiz.

```xml
<appSettings>
    <add key="mailGonderici" value="on"/>
    <add key="varsayilanMail" value="admin@admin.com"/>
    <add key="varsayilanHataSayfasi" value="hataSayfasi.aspx"/>
</appSettings>
<connectionStrings>
    <add name="AdvConStr" connectionString="data source=localhost;database=AdventureWorks;integrated security=SSPI" providerName="System.Data.SqlClient"/>
    <add name="NorthConStr" connectionString="data source=localhost;database=Northwind;integrated security=SSPI" providerName="System.Data.OleDb"/>
</connectionStrings>
```

Uygulama kodlarımızı da aşağıdaki kod parçasında olduğu gibi düzenleyelim.

```csharp
using System;
using System.Configuration;
using System.Web.Configuration;

public partial class _Default : System.Web.UI.Page 
{
    // İlk olarak bizim için gerekli değişkenleri tanımlıyoruz.
    private Configuration _config;
    private AppSettingsSection _appSec;
    private ConnectionStringsSection _conSec;

    // Uygulamaya ait web.config dosyası içeriği _config isimli Configuration tipine ait nesne örneğine alınır.
    private void GetConfig()
    {
        if(_config==null)
        _config = WebConfigurationManager.OpenWebConfiguration("/ConfigMngAPI");
    }

    // web.config dosyası içerisinden appSettings kısmı çekilerek AppSettingsSection tipinden _appSec isimli bir nesne örneğine atanır.
    private void GetAppSettingsSection()
    {
        if(_appSec==null)
        _appSec = _config.AppSettings;
    }

    // web.config dosyasından connectionStrings kısmı çekilerek ConnectionStringsSection tipinden _conSec isimli nesne örneğine atılır.
    private void GetConnectionSection()
    {
        if (_conSec == null)
            _conSec = _config.ConnectionStrings;
    }
    // appSettings kısmındaki key bilgileri Settings özelliği üzerinden gidilen AllKeys özelliği ile alınır ve DropDownList kontrolüne set edilir.
    private void LoadAppSettings()
    {
        GetConfig();
        GetAppSettingsSection();
        ddlAppSettingsKeys.DataSource = _appSec.Settings.AllKeys;
        ddlAppSettingsKeys.DataBind();
    }

    // connectionString sekmesindeki bilgileri taşıyan _conSec nesnesi kullanılarak Name alanlarını içeren veri seti DropDownList kontrolüne bağlanır.
    private void LoadConSettings()
    {
        GetConfig();
        GetConnectionSection();
        ddlConnectionKeys.DataSource = _conSec.ConnectionStrings;
        ddlConnectionKeys.DataTextField = "Name";
        ddlConnectionKeys.DataValueField = "Name";
        ddlConnectionKeys.DataBind();
    }
    // AppSettings kısmındaki key değerlerini gösteren DropDownList kontrolünde seçilen anahtar bilgisine göre Value değeri elde     edilir.
    private void GetAppSetInfo()
    {
        GetConfig();
        GetAppSettingsSection();
        txtAppSetValue.Text = _appSec.Settings[ddlAppSettingsKeys.SelectedValue].Value;
    }

    // DropDownList kontrolünden seçilen name değerine göre, web.config dosyasındaki connectionStrings sekmesinden ilgili connectionString, name ve providerName bilgileri çekilir.
    private void GetConStrInfo()
    {
        GetConfig();
        GetConnectionSection();
        txtConStrValue.Text = _conSec.ConnectionStrings[ddlConnectionKeys.SelectedValue].ConnectionString;
        lblConStrName.Text = _conSec.ConnectionStrings[ddlConnectionKeys.SelectedValue].Name;
        lblProviderName.Text = _conSec.ConnectionStrings[ddlConnectionKeys.SelectedValue].ProviderName;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            LoadAppSettings();
            LoadConSettings();
            GetAppSetInfo();
            GetConStrInfo();
        }
    }
    protected void ddlAppSettingsKeys_SelectedIndexChanged(object sender, EventArgs e)
    {
        GetAppSetInfo();
    }
    protected void ddlConnectionKeys_SelectedIndexChanged(object sender, EventArgs e)
    {
        GetConStrInfo();
    } 
}
```

Dikkat ederseniz web.config dosyası içerisindeki kısımları ele almak için elde edilen Configuration'tipinden faydalanılmıştır. Özellikle appSettings ve connectionStrings sekmeleri çok sık kullanıldığından bunlara doğrudan erişimi sağlayan özellikler mevcuttur. Bu özelliklerin geri dönüş tiplerine dikkat ederseniz, AppSettingsSection ve ConfigurationStringsSection tipinden olduklarını görebiliriz. Dolayısıyla bu tipler üzerinden de bizim için gerekli temel bilgilere geçiş yapabiliriz. Örneğin seçilen bağlantıya ait connectionString bilgisi, sağlayıcı adı (providerName) vb... gibi. Uygulamamızı bu haliyle ilk çalıştırdığımızda aşağıdaki ekran görüntüsüne benzer bir sonuç elde ederiz.

![mk165_1.gif](/assets/images/2006/mk165_1.gif)

Elbette Configuration API'si içerisinde var olan web.config sekmelerine karşılık gelen tipler tanımlanmıştır. Başka konfigurasyon bazlı dosyalara ekleyeceğimiz kendi sekmelerimizde elde edebileceğimiz teknikler mevcuttur. Bunun için örneğin GetSection metodundan yararlanabiliriz. Şu anki örneğimiz basit olarak web.config dosyası içerisinde yer alan bilinen kısımlardan, ConnectionStrings ve AppSettings parçalarına ait bilgileri okuyabilmemizi sağlamaktadır. Bir sonraki adımımızda ise bu bilgiler üzerinde değişiklik yaparak tekrardan web.config dosyasına yazma işlemini ele alacağız. Bunun için uygulamamıza aşağıdaki metodları eklememiz yeterli olacaktır.

```csharp
 // İlk olarak AppSettingsSection tipine ait nesne örneği üzerinden DropDownList' ten seçilen key bilgisinin value özelliğinin değeri TextBox' tan alınır. Burada kayıt işlemini web.config üzerinde gerçekleştirilmesi için mutlaka Configuration tipinen ait Save metodu çağırılmalıdır.
protected void btnChangeAppSet_Click(object sender, EventArgs e)
{
    GetConfig();
    GetAppSettingsSection();
    _appSec.Settings[ddlAppSettingsKeys.SelectedValue].Value = txtAppSetNewValue.Text;
    _config.Save();
    GetAppSetInfo();
}

// ConnectionString bilgisindede değişiklik yapmak için ConnectionStringsSection tipinden yararlanılır. Bu tipe ait nesne örneği yardımıyla DropDownListe' ten seçilen connectionString bilgisi yakalanır ve ConnectionString özelliğine yeni değeri atanır. Son olarak değişikliklerin web.config dosyasına yansıtılabilmesi için Configuration tipine ait Save metodu çalıştırılmalıdır.
protected void btnChangeConStr_Click(object sender, EventArgs e)
{
    GetConfig();
    GetConnectionSection();
    _conSec.ConnectionStrings[ddlConnectionKeys.SelectedValue].ConnectionString = txtNewConStrValue.Text;
    _config.Save();
    GetConStrInfo();
}
```

Artık uygulamamızda yer alan web.config dosyası içerisindeki connectionStrings ve appSettings sekmelerindeki verilerde değişiklik yapabiliriz. Buradaki hassas nokta erişilen bilgini değişikliğe uğramasını takiben Configuration tipinin Save metodunun çağırılmış olmasıdır. Böylece, yapılan değişiklikler kesin olarak Configuration tipine ait nesne örneğinin o anda işaret ettiği konfigurasyon dosyasına (ki burada web.config dosyasıdır) yazılmaktadır. (Not: Aşağıdaki görüntüyü seyredebilmek için tarayıcınızda Flash Player'ın son sürümünün olması tavsiye edilir. Eğer sisteminizde XP Service Pack 2 yüklüyse ilgili uyarıyı dikkate alıp içeriğe izin vermelisiniz. (Allow Blocked Content). Videoyu yönetmek için sağ tıklayıp çıkan menüyü kullanabilirsiniz. Video Boyu 130 kb.)

Yukarıdaki örneklerimizde çok bilinen appSettings ve connectionStrings kısımlarını ele aldık. Örneğin sessionState kısmınıda ele almak istediğimizi düşünelim. Böyle bir durumda Configuration tipi sessionState için bir özellik sunmaz. Bunun yerine konfigurasyon dosyasındaki herhangibir kısmın içeriğini ConfigurationSection tipinden elde etmemizi ve uygun tipe dönüştürmemizi sağlayan GetSection metodundan yararlanabiliriz. Aşağıdaki kod parçası bu işlemi nasıl yapacağımızı göstermektedir.

```csharp
SessionStateSection _sesSec = (SessionStateSection)_config.GetSection("system.web/sessionState");
lblSessionSecInfo.Text = "<b> Session Mode : </b> " + _sesSec.Mode.ToString() + "<br>" + "<b>Cookieless : </b>" + _sesSec.Cookieless.ToString();
```

Burada sessionState kısmını yönetmemizi sağlayan SessionStateSection tipi mevcuttur. Yanlız GetSection metodu geriye ConfigurationSection tipinden bir üye döndürdüğü için bunun ilgili tiplere dönüştürülmesi (casting) gerekir. Bu yüzden SessionStateSection tipine açıkça bir dönüştürme işlemi uygulanmıştır. Sonrasında ise elde edilen _sesSec isimli tip yardımıyla bir sessionState sekmesi için geçerli standart niteliklere kolayca erişebiliriz. Örneğin Cookiless özelliğine yada Mode özelliğine. Burada bir diğer önemli noktada sessionState sekmesine nasıl eriştiğimizdir. Bu kısım web.config içerisinde system.web boğumu içerisinde yer aldığından system.web/sessionState ifadesi ile yakalanmaktadır.

![mk165_7.gif](/assets/images/2006/mk165_7.gif)

Dilersek web.config içerisindeki kısımları şifreli (encrypt) olarakta kayıt edebilir hatta şifrelenmiş veriyi çalışma zamanında düzgün bir biçimde okuyabiliriz. Bu işlemler için var olan section tiplerinin (AppSettingsSection, ConnectionStringsSection gibi) SectionInformation isimli özelliklerinin ProtectSection isimli bir üye metodu kullanılır. Şifrelenmiş verileri çözebilmek için ise SectionInformation isimli özelliğinin UnprotectSection isimli üye metodundan yararlanabiliriz. Konuyu daha iyi anlayabilmek için aşağıdaki örnek kod parçasını ele alalım. Bu kod parçasında web.config dosyası içerisindeki AppSettingsSection ve ConnectionStringsSection kısımlarının encrypt ve decrypt işlemleri için gerekli düzenlemeler yer almaktadır.

```csharp
// ConnectionStrings sekmesini varsayılan RSA sistemine göre şifreler. Bunun için ilgili kısmın SectionInformation özelliğinin ProtectSection metodu kullanılır.
protected void btnProtectConn_Click(object sender, EventArgs e)
{
    GetConfig();
    GetConnectionSection();
    if (!_conSec.SectionInformation.IsProtected)
    {
        _conSec.SectionInformation.ProtectSection("RsaProtectedConfigurationProvider");
        _config.Save();
    }
    else
        Response.Write("ConnectionStrings zaten şifrelenmiştir...");
}

// Bu kez şifrelenmiş olan ConnectionStrings sekmesi çözülerek web.config tekrar kaydedilir.
protected void btnUnprotectConn_Click(object sender, EventArgs e)
{
    GetConfig();
    GetConnectionSection();
    if (_conSec.SectionInformation.IsProtected)
    {
        _conSec.SectionInformation.UnprotectSection();
        _config.Save();
    }
    else
    Response.Write("ConnectionStrings zaten şifresiz tutulmaktadır...");
}

// AppSettings sekmesini varsayılan RSA sistemine göre şifreler. Bunun için ilgili kısmın SectionInformation özelliğinin ProtectSection metodu kullanılır.
protected void btnProtectAppSet_Click(object sender, EventArgs e)
{
    GetConfig();
    GetAppSettingsSection();
    if (!_appSec.SectionInformation.IsProtected)
    {
        _appSec.SectionInformation.ProtectSection("RsaProtectedConfigurationProvider");
        _config.Save();
    }
    else
        Response.Write("AppSettings zaten şifrelidir...");
}
protected void btnUnprotectAppSet_Click(object sender, EventArgs e)
{
    GetConfig();
    GetAppSettingsSection();
    if (_appSec.SectionInformation.IsProtected)
    {
        _appSec.SectionInformation.UnprotectSection();
        _config.Save();
    }
    else
        Response.Write("AppSettings zaten şifreli değil...");
}
```

Uygulamamızı bu haliyle çalıştırıp örneğin connectionStrings ve appSettings kısımlarını şifrelediğimizde web.config dosyasının aşağıdakine benzer bir yapıya büründüğünü görürüz. Burada Protect metodunda parametre olarak RsaProtectedConfigurationProvider kullanılmıştır. Bu provider seçimi ile RSA algoritmasını baz alarak şifreleme ve çözme işlemlerinin gerçekleştirileceği belirtilmektedir. Elbetteki burada kendi özel koruma sağlayıcılarımızı (Custom Protection Provider) da kullanabiliriz.

![mk165_4.gif](/assets/images/2006/mk165_4.gif)

Görüldüğü gibi connectionStrings ve appSettings kısımları için şifreleme işlemleri yapılmış ve key, value, name, providerName, connectionString gibi niteliklerin (attributes) içeriği okunamaz hale getirilmiştir. Lakin, çalışma zamanında ConnectionStringsSection tipine ait nesne örneği üzerinden ilgili bağlantı bilgilerini kolayca okuyabiliriz. Aynı durum appSettings kısmı içinde geçerlidir. Dolayısıyla buradaki şifreleme işlemleri sadece ve sadece web.config dosyasının dışarıdan çıplak gözle okunması sırasında önem kazanmaktadır. Web.config dosyasını istemcilerin elde etmesi zaten pek mümkün değildir. Yine de web.config dosyasındaki bazı hassas bilgileri korumak adına yukarıdaki tekniklerden yararlanılabilinir.

Bazen konfigurasyon dosylarında yer alan kısımların, Configuration API içerisinde karşılığı olan tipler olmayabilir. Bu çoğunlukla Asp.Net 1.1 den 2.0' a geçirilen projelerde rastlanabilecek bir durumdur. (Dahası web.config dosyası dışarısında kullandığımız xml konfigurasyonları varsa daha geçerli bir durumdur.) Dolayısıyla böyle hallerde ilgili kısımların ham XML içeriğini ele almak gerekebilir. Eğer ilgili kısmın Configuration API içerisinde yer alan bir karşılığı yok ise yine ConfigurationSection tipinden yararlanabiliriz. Temel olarak ConfigurationSection tipine ait nesne örneklerini Configuration tipinin GetSection metodu ile elde edebilmekteyiz. Aşağıdaki kod parçaları bu işlemin nasıl gerçekleştirilebileceğini göstermektedir. Örnek olarak yine appSettings ve connectionStrings parçalarını ele alıyoruz.

```csharp
// AppSettings kısmının ham XML içeriğini elde etmek için SectionInformation özelliğinin GetRawXml metodu kullanılır.
protected void btnGetAppSetRawXml_Click(object sender, EventArgs e)
{
    GetConfig();
    ConfigurationSection cfgSec = _config.GetSection("appSettings");
    lblAppSetRawXml.Text = Server.HtmlEncode(cfgSec.SectionInformation.GetRawXml());
}
// ConnectionStrings kısmının ham XML içeriğini elde etmek için SectionInformation özelliğinin GetRawXml metodu kullanılır.
protected void btnGetConStrRawXml_Click(object sender, EventArgs e)
{
    GetConfig();
    ConfigurationSection cfgSec = _config.GetSection("connectionStrings");
    lblConStrRawXml.Text = Server.HtmlEncode(cfgSec.SectionInformation.GetRawXml());
}
```

Yanlız burada dikkat etmemiz gereken bir nokta vardır. O da GetSection metodunun mutlaka ve mutlaka, ilgili kısmın web.config içerisindeki harf duyarlı (case-sensitive) bilgisini almak zorunda oluşudur. Örneğin connectionStrings yerine ConnectionStrings yazdığımızda çalışma zamanında aşağıdaki hata mesajını alırız.

![mk165_6.gif](/assets/images/2006/mk165_6.gif)

Uygulamamızı bu haliyle çalıştırdığımızda aşağıdaki ekran görüntüsünde olduğu gibi appSettings ve connectionStrings sekmelerinin ham Xml içeriklerini elde edebileceğimizi görürüz.

![mk165_5.gif](/assets/images/2006/mk165_5.gif)

IIS kendi içerisinde bir web uygulmasının ayarlarını değiştirmemizi sağlayacak bir yönetim konsolu (management Console) içerir. Asp.Net Microsoft Management Console Snap-In'i sayesinde web.config içerisindeki ayarları görsel arabirimi kullanarak IIS üzerinden de değiştirebiliriz.

![mk165_2.gif](/assets/images/2006/mk165_2.gif)

Bu konsol sayesinde aşağıdaki resimden de görülebileceği gibi hemen hemen tüm kısımlara (sections) ait değişiklikleri kolayca gerçekleştirebilmekteyiz. Bu elbette IIS üzerinde var olan bir yönetimsel ara birimdir ve zaman zaman tercih edebiliriz. Ama özellikle kendi web uygulamalarımızı uzaktan yönetmek istediğimiz durumlarda, yonetim (admin) sayfalarından web.config gibi dosyalara daha kolay ve güçlü bir şekilde de hükmedebilmeliyiz. Bu sebepten dolayı Configuration API'nin geliştirilmesi ve genişletilmesinin son derece isabetli olduğu kanısındayım.

![mk165_3.gif](/assets/images/2006/mk165_3.gif)

Bu makalemizde temel olarak Asp.Net 2.0 ile gelen yeni konfigurasyon alt yapısını (Configuration API) incelemeye çalıştık. Eğer bu API içerisindeki tipleri ve üyelerini inceleyecek olursanız, aslında bir sistemin tüm konfigurasyon ayarlarını çok kolay ve etkin bir şekilde programatik olarak yapabileceğimizi görürsünüz. Özellikle xml tabanlı konfigurasyon içeriklerini her hangi bir ayrıştırma (parsing) işlemine gerek duymadan yönetebilecek tiplerle sahip olmak son derece etkili ve verimli bir geliştirme ortamı sağlayacaktır. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama İçin Tıklayın.](/assets/files/2006/ConfigMngAPI.rar)