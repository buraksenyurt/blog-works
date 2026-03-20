---
layout: post
title: "Asp.Net Temelleri - Etkin Hata Yönetimi (Error Management)"
date: 2007-07-18 12:00:00 +0300
categories:
  - aspnet
tags:
  - aspnet
  - csharp
  - dotnet
  - wcf
  - http
  - iis
---
Uzun süredir Windows Communication Foundation ile ilgili yazılar yayınlıyoruz. Sanıyorumki biraz hava değişimine ihtiyacımız olacak. Bu nedenle bu haftaki yazımızda biraz daha hafif ama önemli olan bir konu üzerinde durmaya çalışacağız. Web uygulamalarında sunucu taraflı hata yönetimi (Server Side State Management)..Net ortamında hataların ele alınmasında kullanılan en bilinen yol try...catch...finally bloklarıdır. Ne varki uygulama ortamları çeşitlilik göstermektedir.

Bir sınıf kütüphanesi içerisinde yapılan hata kontrolü ile dağıtık mimari uygulamaları (distributed applications) içerisinde yapılan hata yönetimi farklıdır.(Örneğin WCF içerisindeki Fault Management konusunu hatırlayalım) Bu sebepten Asp.Net uygulamalarındada farklı bir yaklaşımı ele almak gerekmektedir. Web uygulamalarında oluşan hatalar sonucu çok hoş olmayan hata ekranları ile karşılaşmak mümkün olabilmektedir. Ancak hatalar kontrollü bir şekilde yönetilebilirlerse, son kullanıcıyı bilgilendirebilecek şekilde mesajlar verilip hataların düzeltilmesi yönünde daha sağlam ve güçlü adımlar atılabilir. Bu aynı zamanda uygulamanın tutarlılığı ve güvenilirliği açısındanda önemlidir.

Asp.Net ortamı, hataların yönetimi amacıyla istisna (Exception) tiplerini ve hataları yakalayıcı olay (event) metodların göz önüne alır. Söz konusu hata yönetimi metod seviyesinde (Method Level), sayfa seviyesinde (Page Level) ve uygulama seviyesinde (Application Level) gerçekleştirilebilir. Aşağıdaki tabloda söz konusu seviyeler ve aralarındaki temel farklar vurgulanmaya çalışılmaktadır.

Asp.Net Hata Yönetim Seviyeleri (Error Management Levels)

Metod Seviyesinde
Sayfa Seviyesinde
Uygulama Seviyesinde

Toparlanabilir veya bir başka deyişle kurtarılabilir hatalar çoğunlukla metod seviyesinde ele alınır.
Eğer olası hatalar toparlanamayacak cinsten ise bir üst seviyeye yönlendirilir.

Bir sayfa ile ilgili tüm hataların tek bir merkezden yönetilebilmesi sağlanır.
Olası hatalar sonrasında kullanıcılar çoğunlukla özel sayfalara yönlendirilir.
Bu seviyede sayfaların PageError olay metodları ele alınır.
Hata sayfasına yönlendirilmeden önce sayfa ile ilgili log bilgisi yazdırma, fiziki dosyalara bilgi atma veya adminlere mail gönderme gibi işlemler yapılabilir.

Uygulama içerisinde herhangibir sayfada meydana gelen hataların yakalanması sağlanır.
Tüm web uygulamasının hata yönetiminin tek bir merkezden kontrol edilebilmesi sağlanmış olur.
Bu seviyede global.asax dosyasındaki ApplicationError olay metodu ele alınır.
Hata sayfasına yönlendirilmeden önce log bilgisi yazdırma, fiziki dosyalara bilgi atma veya adminlere mail gönderme gibi işlemler yapılabilir.

Metod seviyesinden, uygulama seviyesine doğru çıkıldıkça hataların merkezi olarak yönetilmesi ve tek bir merkezden ele alınması dahada kolaylaşmakta ancak, detay bilgilerinden gittikçe uzaklaşılmaktadır. Nitekim bir metod içerisinde meydana gelecek bir hata ile ilişkili yakalanan detayın, sayfa veya uygulama seviyesine aktarılmadığı sürece merkezi olarak ayrıştırılması zor olmaktadır.

Şimdi gelin bu seviyeleri örnekler yardımıyla incelemeye çalışalım. Metod seviyesinde hata yönetiminde try...catch...finally blokları büyük önem arz etmektedir. Ancak bu bloklar istenirse try...catch veya try...finally şeklindede yazılabilir. Çok doğal olarak bunladan hangisinin kullanılacağının kararını vermek için bazı vakkaların göz önüne alınması gerekmektedir. İlk olarak basit bir örnek ile başlayalım. Bu amaçla kullanıcının bölme işlemi yaptığı aşağıdaki gibi bir aspx sayfası olduğunu göz önüne alabiliriz.

```text
<%@ Page Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

    protected void Hesapla_Click(object sender, EventArgs e)
    {
        double deger1 = Convert.ToDouble(txtDeger1.Text);
        double deger2 = Convert.ToDouble(txtDeger2.Text);
        double sonuc = deger1 / deger2;
    }

</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <title>Hata Yonetimi (Metod Seviyesinde)</title>
    </head>
    <body>
        <form id="form1" runat="server">
            <div>
                Birinci Değer : 
                <asp:TextBox ID="txtDeger1" runat="server"></asp:TextBox> <br />
                İkinci Değer : 
                <asp:TextBox ID="txtDeger2" runat="server"></asp:TextBox>
                <br />
                <asp:Button ID="btnHesapla" runat="server" Text="Hesapla" OnClick="Hesapla_Click" />
            </div>
        </form>
    </body>
</html>
```

Öncelikle hata kontrolü yapmadan sayfayı ele almaya çalışacağız. Bu nedenle örnek olarak ikinci kutucuğu boş bırakıp Hesapla isimli düğmeye basıyoruz. Sonuç olarak kullanıcı açısından pekte hoş olmayacak aşağıdaki ekran görüntüsü ile karşılaşırız. (Elbette geliştirme aşamasında bu mesajlar developer açısından daha kıymetli olabilir;)

![mk214_1.gif](/assets/images/2007/mk214_1.gif)

Dolayısıyla try...catch blokları yardımıyla Hesapla_Click isimli olay metoduna ait kodların aşağıdaki hale getirilmesi daha doğru olacaktır.

```csharp
protected void Hesapla_Click(object sender, EventArgs e)
{
    try
    {
        double deger1 = Convert.ToDouble(txtDeger1.Text);
        double deger2 = Convert.ToDouble(txtDeger2.Text);
        double sonuc = deger1 / deger2;
    }
    catch (FormatException err)
    {
        Response.Write("<b>Değerler sayısal olmalıdır. Lütfen girdiğiniz değerleri kontrol ediniz.</b><br/>Detaylı Mesaj : " + err.Message);
    }
    catch (Exception err)
    {
        Response.Write("<b>Beklenmeyen bir hata oluştu.</b><br/>Detaylı Mesaj : " + err.Message);
    }
}
```

Bunun sonucunda aynı hata tekrar edilmeye çalışılırsa bu sefer mantıklı bir ekran ile karşılaşılacak ve kullanıcı hata ile ilişkili olarak daha doğru bir şekilde bilgilendirilebilecektir.

![mk214_2.gif](/assets/images/2007/mk214_2.gif)

Elbette metod seviyesinde hata yönetimi adına dikkat edilmesi gereken bazı vakkalarda vardır. Eğer oluşan hataların kurtarılabilme (toparlanabilme) ihtimali varsa try...catch blokları döngüler (while, for gibi) içerisinde ele alınabilir. Böylece tekrar sayısına göre istenen rutin bir kaç kez üst üste denenebilir. Diğer taraftan oluşan istisnalar ile ilişikili olarak ekstradan verilebilecek yada kullanılabilecek bilgiler varsa bunların bir üst seviyede (sayfa seviyesinde-page level) ele alınması için catch bloğu içerisinde throw anahtar kelimesine başvurulabilir. Burada özellikle Exception sınıfının aşırı yüklenmiş (overload) yapıcı (constructor) metodlarından faydalanılmaktadır.

Diğer bir vakka metod içinde kullanılan dış kaynakları ele alır. Örneğin ilgili rutinler içerisinde kaynak temizlenmesi gerekiyorsa (bağlantıların veya dosyaların kapatılması, yönetimsiz-unmanaged nesnelerin serbest bırakılması gibi) finally bloklarını kullanmak doğru olacaktır. Burada finally bloklarının kullanılması şart değildir. Nitekim using blokları yardımıylada, IDisposable arayüzünü uygulayan tipler için blok sonunda Dispose çağrıları gerçekleştirilebilir. Son olarak olası hatalar ilgili metod içerisinde ele alınamıyorsa metodu çağıran yerde yakalanmalıdır.

Görüldüğü üzere vakkaların sayısı ve metod içerisindeki hata yönetimi çeşitli şekillerde yapılabilmektedir. Bu sebepten karar verirken aşağıdaki gibi tablodan faydalanmakta yarar vardır.

Öneri
Kaynak temizlemesi gerekiyor mu?
Olası hata var mı?
Olası hatalar kurtarılabilir mi?
Eklenecek ilave hata bilgisi var mı?

Hiç bir kontrole gerek yok
hayır
yok
hayır
yok

hayır
var
hayır
yok

try...finally
evet
yok
hayır
yok

evet
var
hayır
yok

try...catch
hayır
var
hayır
var

try...catch...finally
evet
evet
hayır
var

Örneğin kaynak temizlenmesi gerekiyorsa, olası hatalar var ise ve hatta olası hatalara eklenebilecek ekstra bilgiler var ise try..catch...finally bloklarını kullanmak daha mantıklıdır. Ne varki olası hatalarda, hata mesajına ilave bilgiler eklemek catch blokları içerisinde throw kullanmak ile mümkün olabilir. Çünkü amaç, bu hatayı ele alan bir üst seviyeye bilgi göndermektir. Bunun ele alınabileceği en güzel yer sayfa seviyesidir (Page Level). Öyleyse sayfa seviyesinde hata yönetiminin nasıl yapılacağını inceleyerek devam edelim.

Özellikle belirli bir sayfada meydana gelebilecek tüm hataların tek bir merkezden kontrolünün sağlanması gerektiği durumlarda sayfa seviyesinde hata yönetimi gerçekleştirilebilir. Bu teknikte önemli olan nokta, PageError olay metodunun etkin bir şekilde kullanılmasıdır. Aslında sayfa seviyesinde hatalar ele alınırken izlenen basit bir yol vardır. Aşağıdaki tabloda bu yol gösterilmektedir.

Sayfa Seviyesinde Hata Kontrolü için Tavsiye Edilen Yol

Madde 0
Bir hata sayfası tasarlanır.:)

Madde 1
Sayfaya PageError olay metodu eklenir.

Madde 2
Sayfanın ErrorPage özelliğine hata sayfasının Url bilgisi Page direktifi içerisinde eklenir.

Madde 3
PageError olay metodu içerisinde Server sınıfının static GetLastError () metodu ile son oluşan istisna nesne örneği ele alınır.

Madde 4
İstenirse ErrorPage özelliğine burada değer ve hatta querystring yardımıyla bilgi aktarılması sağlanabilir. Böylece hata sayfasına bazı ekstra bilgilerin taşınmasıda sağlanmış olur.

Madde 4.5
Gerekirse bu aşamada loglama (özellikle sistemdeki event loglara bilgi yazma), fiziki dosyalara bilgi yazdırma, yönetici veya ilgili kişilere mail gönderme gibi işlemler yapılabilir.

Page_Error isimli olay metodu sayfa seviyesinde ele alınır. Normal şartlarda sayfada ele alınmayan bir hata oluştuğunda bu metod otomatik olarak çağırılacaktır. Biz bu metod içerisinde yönlendirmeler yaparak kullanıcıları daha akıllı hata bilgilendirme sayfalarına yönlendirebilir ve loglama gibi işlemleri gerçekleştirebiliriz. PageError olay metodu içerisinden ilgili hata sayfasına yönlendirme yaparken Page sınıfının ErrorPage özelliğine değer atamak gerekebilir. Bu daha çok querystring yardımıyla hata sayfasına ekstra bilgi gönderileceği durumlarda ele alınır. Aksi durumlarda metod içerisinde değilde Page direktifinde bu özelliğin değerinin belirlenmesi yeterlidir. Ancak burada dikkat edilmesi gereken bir nokta vardır. Eğer metod içerisinde ErrorPage özelliğine hata sayfasını atarken querystring kullanılmassa Asp.Net, çalışma zamanında aspxerrorpath isimli bir anahtarı ve değerini otomatik olarak ekleyecektir. Ki buda hatanın oluştuğu sayfanın yakalanabilmesi ve belkide dinamik bir linkin üretilerek tekrar geri gidilebilmesinide sağlayacaktır.

Şimdi yukarıdaki örneğimizi sayfa seviyesinde ele almaya çalışalım. Madde 0' da değindiğimiz gibi öncelikli olarak bir hata sayfası tasarlamakta fayda var. Bu hata sayfası aşağıdaki gibi tasarlanabilir.

HataSayfasi.aspx;

```text
<%@ Page Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string ekBilgi = Request.QueryString["EkBilgi"];
        string sayfa=Request.QueryString["Sayfa"];
        string hataMesaji = Request.QueryString["HataMesaji"];
        Response.Write("<b>Hata sayfası : </b>" + sayfa+"<br/>");
        Response.Write("<b>Ek bilgi : </b>" + ekBilgi + "<br/>");
        Response.Write("<b>Hata Mesajı : </b>" + hataMesaji);
    } 

</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <title>Hata Sayfası</title>
    </head>
    <body>
        <form id="form1" runat="server">
            <div>
            </div>
        </form>
    </body>
</html>
```

Default.aspx sayfasındaki kodları ise aşağıdaki gibi değiştirebiliriz.

```csharp
protected void Page_Error(object sender, EventArgs e)
{
    Exception olusanHata = Server.GetLastError();
    ErrorPage = "HataSayfasi.aspx?EkBilgi=" + olusanHata.Message + "&HataMesaji=" + olusanHata.InnerException.Message + "&Sayfa="+Page.AppRelativeVirtualPath;
}

protected void Hesapla_Click(object sender, EventArgs e)
{
    try
    {
        double deger1 = Convert.ToDouble(txtDeger1.Text);
        double deger2 = Convert.ToDouble(txtDeger2.Text);
        double sonuc = deger1 / deger2;
    }
    catch (Exception excp)
    {
        throw new Exception("Sayısal değer girişinde hata oluştu", excp);
    }
}
```

Hesapla_Click olay metodu içerisinde yer alan catch bloğunda throw anahtar sözcüğü kullanılarak bir Exception nesnesi daha fırlatılmıştır. Bu istisna nesnesinin yakalanacağı yer sayfanın PageError isimli olay metodudur. Bu metod içerisinde, sayfada oluşan son hatayı yakalayabilmek için Server sınıfının GetLastError metodu kullanılmıştır. Dikkat edilmesi gereken noktalardan birisi, throw ile fırlatılan Exception nesnesi örneklenirken ilk parametreye örnek bir ekstra veri konulmasıdır. Eklenen bu bilgi GetLastError ile yakalanan Exception nesne örneğinin Message özelliği ile elde edilebilir. Bu durumda orjinal istisna mesajınının nereden alınabileceği bir soru işaretidir. Cevap InnerException özelliğidir. InnerException özelliği ile fırlatılan asıl Exception nesne örneği yakalanabilir. Bunu çalışma zamanında test ettiğimizde aşağıdaki ekran görüntüsünde olduğu gibi FormatException tipinin InnerException özelliğinde saklandığını görebiliriz.

![mk214_3.gif](/assets/images/2007/mk214_3.gif)

Son olarak ErrorPage özelliği ile hata sayfasına gerekli yönlendirme yapılmaktadır. Elbette ErrorPage kullanılmak zorunda değildir. Bunun yerine Server sınıfının Transfer metodu veya Response sınıfının Redirect metodlarının kullanımıda tercih edilebilir. Özellikle Server sınıfının Transfer metodu gereksiz roundtrip'lerin önüne geçilmesini sağlamakta ama url satırına bakıldığında halen daha aynı sayfada olunduğu izlenimini vermektedir.

Örnek geliştirilirken dikkat edilmesi gereken bir nokta vardır. Eğer örneği aynı makine üzerinde (localhost) test ediyorsak beklediğimiz yönlendirme sayfasına gidemediğimizi hatta eski sarı ekranın (orjinal hata mesajının basıldığı sayfadan bahsediyoruz) üretildiğini görürüz. Bunun nedeni web uygulamasının özel hata modunun aktif olmayışıdır. Bir başka deyişle web.config dosyasında yer alan customErrors elementinin mode niteliğine On değerinin verilmesi gerekir.

> Aslında mode özelliğinin 3 farklı değeri vardır. RemoteOnly, On ve Off. RemoteOnly modu aktif iken özel hata sayfalarını sadece istemciler görebilir. On modunda hem istemciler hemde localhost kullanıcısı özel hata sayfalarını görebilir. Biz örneklerimizdeki sayfalarımızı aynı makine üzerinden test ettiğimiz için bu modu On olarak belirledik. Gerçek bir uygulama ortamına çıkıldığında On yerine RemoteOnly kullanılması tavsiye edilir.

```text
<customErrors mode="On"/>
```

customErrors elementi içerisine error isimli alt elementlerde konulabilir. Bu element sayesinde sunucu seviyesinde meydana gelen hatalar var ise bunların sonucunda özel hata sayfalarına yönlendirmeler yapılabilir. Söz gelimi aşağıdaki bildirimleri ele alalım.

```text
<customErrors mode="On">
    <error statusCode="404" redirect="SayfaYok.aspx"/>
</customErrors>
```

Buna göre sitede olmayan bir sayfa talep edilirse kullanıcılar SayfaYok.aspx'e yönlendirilirer.

> Yanlız burada dikkat edilmesi gereken bir nokta vardır. Sonradan ele alacağımız gibi global.asax dosyasındaki ApplicationError olayı yazılmışsa, uygulama SayfaYok.aspx'e yönlendirilmeden önce buradaki olay metoduna uğrayacaktır ki burada bir yönlendirme yapıyorsak SayfaYok.aspx yerine oraya gidilebilir. Hatta Server.ClearError metodu kullanılmışsa hata sayfasına gidilmeyedebilir. Buda sistemin istediğimiz şekilde çalışmaması anlamına gelmektedir. Gerçi Application_Error içerisinde oluşan hata örneğin sayfa yok hatası yinede yakalanabilir. Örneğin Debug modda bu aşağıdaki şekilde olduğu giri görünecektir.
> ![mk214_12.gif](/assets/images/2007/mk214_12.gif)
> Dolayısıyla bu noktalara dikkat etmekte fayda vardır.

Örneğin şu aşamada iken web uygulamasını çalıştırdıkdan sonra Giris.aspx isimli yazmadığımız bir sayfayı talep edersek aşağıdaki ekran görüntüsü ile karşılaşırır.

![mk214_11.gif](/assets/images/2007/mk214_11.gif)

Özellikle SayfaYok.aspx'ten sonra gelen querystring parametresine dikkat edelim. aspxerrorpath ile gelen değer alınıp kullanıcıya daha anlamlı bir hata sayfası gösterilebilir. Biz tekrardan konumuza geri dönelim ve customErrors elementini aşağıdaki haliyle bırakalım.

```text
<customErrors mode="On"/>
```

Bu değişiklikten sonra uygulama çalışma zamanında test edilirse default.aspx sayfasında hata oluştuktan sonra HataSayfasi.aspx'e gidildiği görülebilir. Tarayıcı penceresindeki url satırına dikkat edilecek olursa querystring parametreleri ve değerleride başarılı bir şekilde aktarılmıştır.

![mk214_4.gif](/assets/images/2007/mk214_4.gif)

Şunu itiraf etmeliyim ki sarı ekranın görüntüsü buradakinden daha güzeldir. Dolayısıyla hata sayfaları hazırlanırken biraz daha özenilmeli, gerektiğinde projenin sahibi olan şirket standartlarına uygun olaraktan tasarlanmalı ve zengin bir bilgi sunacak hale getirilmelidir.

Oluşan hatalara ilişkin kullanıcılara bilgi verilmesi dışında, siteyi tasarlayan veya yönetenlerinde bilgilendirilmesi gerekebilir. Bu bilgilendirme farklı şekillerde yapılabilir. Örneğin var olan işletim sistemi loglarına bilgi yazılabilir. Örneğin Application loglarına. Ya da daha basit olarak fiziki bir dosyaya hatalar ile ilişkili bazı bilgiler gönderilebilir. Hatta gerektiği yerlerde çok kritik hatalar söz konusu ise ilgili kişilere mail bile gönderilebilir. Söz gelimi aşağıdaki kod parçası ile PageError metodu içerisinde, oluşan son hataya ait bilgi fiziki bir dosyaya eklenmektedir.

```csharp
protected void Page_Error(object sender, EventArgs e)
{
    Exception olusanHata = Server.GetLastError();
    ErrorPage = "HataSayfasi.aspx?EkBilgi=" + olusanHata.Message + "&HataMesaji=" + olusanHata.InnerException.Message + "&Sayfa="+Page.AppRelativeVirtualPath;

    using (FileStream stream = new FileStream("C:\\HataLogDosyasi.txt", FileMode.Append, FileAccess.Write))
    {
        StreamWriter writer = new StreamWriter(stream);
        writer.WriteLine("Hata Zamanı " + DateTime.Now.ToString() + " Hata Sayfası " + Page.AppRelativeVirtualPath + " Hata Mesajı " + olusanHata.InnerException.Message);
        writer.Close();
    }
}
```

Burada basit olarak FileStream ve StreamWriter tiplerinden yararlanılarak hata bilgileri C: klasörü altındaki bir text dosyasına yazdırılmaktadır. Sonuç olarak uygulama test edildiğinden oluşturulan hata sonrasında ilgili dosyaya aşağıdaki ekran görüntüsünde olduğu gibi bazı bilgiler eklenecektir.

![mk214_5.gif](/assets/images/2007/mk214_5.gif)

PageError metodu içerisinde hata sayfasına herhangibir şekilde yönlendirme yapılmamasına rağmen gidilmektedir. Bu metodun bir özelliğidir. Metod sonuna gelindiğinde, ErrorPage ile belirlenmiş sayfaya otomatik olarak gidilir. ErrorPage değerinin programatik olarak belirlenmesi haricinde Page direktifi içerisinde ayarlanabileceğini daha önceden söylemiştik. Bu aşağıdaki ekran görüntüsünde olduğu gibi düzenlenebilir.

![mk214_6.gif](/assets/images/2007/mk214_6.gif)

Elbette metod içerisinde ErrorPage özelliği belirtilmişse Page direktifinde yapılan tanımlama geçersiz sayılacaktır.

Gelelim uygulama seviyesinde hata yönetimine. Bu durumda web uygulamasında meydana gelecek hataların ele alınabileceği bir merkez söz konusudur. Söz konusu merkez global.asax dosyası içerisinde yer alan ApplicationError isimli olay metodudur. Bildiğiniz gibi global.asax dosyasında, uygulama genelini ilgilendiren bazı olay metodları yer almaktadır. Örneğin uygulama çalışmaya başladığında devreye giren ApplicationStart, sonlandığında çağrılan ApplicationEnd yada kullanıcıların açtıkları oturumlarda (Session) devreye giren SessionStart gibi. Dolayısıyla ilk yapılması gereken işlem web sitesine, eğer yok ise bir global.asax dosyası eklemek olacaktır. Sayfa seviyesindeki hata yönetiminde olduğu gibi, uygulama seviyesinde yapılacak hata yönetimi içinde tavsiye edilen bir yol haritası vardır ve aşağıdaki tabloda olduğu gibidir.

Uygulama Seviyesinde Hata Kontrolü için Tavsiye Edilen Yol

Madde 0
Bir hata sayfası tasarlanır.:)

Madde 1
Sayfalara PageError olay metodları eklenir.

Madde 2
PageError olay metodlarında, son olarak elde edilen istisna (Exception) nesnesinin referansı aynen metod içerisinde olduğu gibi bilinçli olarak ortama fırlatılır (throw).

Madde 3
global.asax dosyasında yer alan ApplicationError olay metodu kodlanır. Bu metod içerisinde son hata bilgisi yine GetLastError metodu ile alınır.

Madde 3.5
Gerekirse bu aşamada loglama (özellikle sistemdeki event loglara bilgi yazma), fiziki dosyalara bilgi yazdırma, yönetici veya ilgili kişilere mail gönderme gibi işlemler yapılabilir. (Sistem loglarına yazma sırasında dikkat edilmesi gereken durumlardan birisi ASPNET (IIS 5.0 için) veya Network Service (IIS 6.0 için) kullanıcısının Application, System ve Security loglarına yazma hakkı olup olmadığıdır. Söz gelimi ASPNET kullanıcısının varsayılan olarak Application loglarına yazma hakkı varken System ve Security loglarına yazma hakkı yoktur. Bu nedenle ilgili kullanıcıların haklarının özellikle loglara yazma işlemleri sırasında dikkate alınması gerekebilir.)

Madde 4
Kullanıcı hata sayfasına yönlendirilmeden önce Server sınıfının ClearError metodunun çağırılması ve hataların temizlenmesi önerilir.

Madde 5
Server.Transfer metodu ile hata sayfasına yönlendirme yapılır.

Buradaki maddelerde dikkat çekici noktalardan biriside son hatanın sayfalara ait Page_Error olay metodları içerisinde tekrardan fırlatılıyor olmasıdır. Bu bir anlamda hatanın bir üst seviyeye aktarılmasıdır. Diğer taraftan bir gerekliliktir. Nitekim, hata bilinçli olarak uygulama seviyesine gönderilmesse Asp.Net çalışma ortamı (Asp.Net RunTime), hatanın ele alınması için HttpUnhandledException tipinden bir nesne örneği üretecektir. Biz hatayı kontrollü bir şekilde ele almak istiyorsak bilinçli bir şekilde fırlatma işlemini üstlenmeliyiz.

Çok doğal olarak web uygulaması içerisinde birden fazla aspx sayfası olduğu düşünülecek olursa, hepsine bir PageError metodu eklemek ve kodlamak (en azından GetLastError ile elde edilen istisna nesnelerini fırlatmak) uğraştırıcı ve sabrımızı test edici olabilir. Burada nesne yönelimli mimarinin avantajlarından faydalanmak çok daha akılcı bir çözüm olacaktır. Bir başka deyişle tüm sayfaların türediği bir taban sayfa (base page) içerisindeki Page_Error metodu ele alınabilir.

> Alternatif bir yaklaşım olarak MasterPage kullanımı düşünülebilir. Her ne kadar MasterPage içerisine Page_Error isimli bir metod yazılabiliyor olsada, aslında metod seviyesinden throw ile exception fırlatıldığında bu metod herhangibir şekilde tetiklenmez. Eğer Application_Error olay metodu yazılmışsa doğrudan buraya düşülür ve HttpUnhandledException tipinden bir istisna nesnesi yakalanır. Bu sebepten bu tip bir durumda MasterPage içerisindeki Page_Error olay metodunun kullanımı şeklinde bir çözüm ne yazıkki söz konusu değildir.

İlk olarak Application_Error olay metodunu nasıl ele alacağımıza bakalım. Öncelikle aspx sayfalarındaki PageError olay metodlarından yine bir üst seviyeye hata fırlatmak gerekir. Bu nedenle örnek olarak default.aspx sayfasındaki PageError olay metodu aşağıdaki gibi değiştirilmelidir.

```csharp
protected void Page_Error(object sender, EventArgs e)
{
    Exception olusanHata = Server.GetLastError();
    throw olusanHata;
}
```

Uygulama seviyesinde hata kontrolü yapılacağından ilgili olay kodunun global application class içerisinde yer alması gerekir. Bu nedenle bir adet global.asax dosyası web sitesine dahil edilmelidir. global.asax dosyasında ApplicationError olay metodu içerisinde ise aşağıdakine benzer kodlamalar yapılmalıdır.

```csharp
void Application_Error(object sender, EventArgs e) 
{
    Exception excp = Server.GetLastError();
    // Burada loglama, dosyaya yazma, mail gönderme gibi işlemler yapılabilir.
    Server.ClearError();
    Server.Transfer("GenelHataSayfasi.aspx?EkBilgi="+excp.Message+"&HataMesaji="+excp.InnerException.Message);
}
```

Bu sefer global.asax dosyası içerisinden GenelHataSayfasi.aspx isimli web sayfasına querystring yardımıyla bilgi taşınmaktadır. GenelHataSayfasi.aspx içeriği, HataSayfasi.aspx'e benzemekle birlikte tek farkı hatanın meydana geldiği sayfa bilgisini ele almıyor oluşudur. Eğer uygulama bu haliyle test edilir ve yine Default.aspx içerisinde hata yaptırılırsa aşağıdaki ekran görüntüsü ile karşılaşılır.

![mk214_7.gif](/assets/images/2007/mk214_7.gif)

Görüldüğü gibi default.aspx içerisindeki metodda oluşan hata catch bloğunda ek bilgi ile tekrar throw edilmiş ve bunu sayfanın Page_Error olay metodu yakalamıştır. Sonrasında ise Page_Error olay metodunda hata tekrar throw ile fırlatılmış ve bunun sonucunda Application_Error olay metoduna gidilebilmiştir. Aşağıdaki grafikte bu durumun Debug moddaki karşılığı gösterilmektedir.

Önce Metod içi;

![mk214_8.gif](/assets/images/2007/mk214_8.gif)

Sonra Page_Error;

![mk214_9.gif](/assets/images/2007/mk214_9.gif)

Son olarak Application_Error;

![mk214_10.gif](/assets/images/2007/mk214_10.gif)

Çalışma sonrasında dikkati çeken noktalardan birisi tarayıcı penceresindeki url satırında Default.aspx'in görünüyor olmasıdır. Halbuki şu anda üzerinde bulunulan sayfa GenelHataYonetimi.aspx'dir. Bunun sebebi Server.Transfer metodudur. Bunun yerine Response.Redirect'te tercih edilebilir. Ama daha öncedende belirtildiği üzere Transfer metodu ile sunucuya doğru gidiş gelişler azaldığı için özellikle uygulama seviyesindeki hata yönetiminde tercih edilen bir tekniktir. İlgili hata sayfası dahada geliştirilebilir. Örneğin hatanın oluştuğu sayfaya dönülmesi için gerekli bağlantı bilgileri hata mesajı ile birlikte taşınabilir. Bu ve dahası tamamen sizlerin hayal gücüne kalmaktadır.

Böylece geldik bir makalemizin daha sonuna. Bu makalemizde Asp.Net uygulamalarında etkin hata yönetimi adına metod, sayfa ve uygulama seviyesinde neler yapabileceğimizi incelemeye çalıştık. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](/assets/files/2007/HataYonetimi.zip)