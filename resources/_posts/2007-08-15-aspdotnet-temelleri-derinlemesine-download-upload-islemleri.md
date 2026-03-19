---
layout: post
title: "Asp.Net Temelleri : Derinlemesine Download/Upload İşlemleri"
date: 2007-08-15 06:00:00 +0300
description: "Tatile çıkan herkes, iyi ve dinlendirici geçen günlerin ardından tekrar hayatın akışına kapıldığında kısa süreliğinede olsa adaptasyon problemi yaşar. Tatildeyken hatırlayacağınız gibi hafif ve dinlendirici bir Asp.Net konusu ile ilgilenmeye çalışmıştık..."
categories:
  - aspnet
tags:
  - aspnet
  - csharp
  - xml
  - dotnet
  - sql-server
  - http
  - iis
  - authentication
  - generics
---
# Asp.Net Temelleri : Derinlemesine Download/Upload İşlemleri
Değerli Okurlarım Merhabalar,

Tatile çıkan herkes, iyi ve dinlendirici geçen günlerin ardından tekrar hayatın akışına kapıldığında kısa süreliğinede olsa adaptasyon problemi yaşar. Tatildeyken hatırlayacağınız gibi hafif ve dinlendirici bir Asp.Net konusu ile ilgilenmeye çalışmıştık. Tatil dönüşündeki adaptasyon sürecinde de benzer nitelikte bir konuyu incelemenin uygun olacağı kanısındayım. Bu yazımızda Asp.Net uygulamalarında sıklıkla başvurduğumuz temel dosya giriş/çıkış (Input/Output -IO) işlemlerinden yararlanarak Download ve Upload işlemlerinin nasıl yapılabileceğini ele almaya çalışacağız.

Özellikle web tabanlı içerik yönetim sistemlerinde (Content Management System), kullanıcıların sunucu üzerinde dökümanlar ile etkin bir şekilde çalışabilmeleri sağlanmaktadır. Bu sistemlerde genel olarak kullanıcı kimliği veya rolüne göre istemci bilgisayarlara indirilebilen (Download). Hatta çoğu içerik yönetim sisteminde, istemciler herkesin okuyabileceği yada belirli kişilerin görebileceği şekilde sunucuya döküman aktarma (Upload) işlemleride yapabilirler. Söz gelimi bir yazılım şirketinin içerik yönetim sistemi göz önüne alındığında, yazılım departmanındaki geliştiricilerin hazırladıkları teknik dökümantasyonları Upload veya Download edebilecekleri bir ortam hazırlanabilir.

Hangi açıdan bakılırsa bakılsın, web tabanlı olarak yapılan bu işlemler için şirketler büyük ölçekli sistemler tasarlayıp geliştirmiştir. Fakat temel ilke ve yaklaşımlar benzerdir. Dosya indirme veya gönderme işlemleri, web tabanlı bir sistem göz önüne alındığında HTTP kurallarına bağlıdır. Dolayısıyla bu kuralların sadece uygulanma ve ele alınma biçimleri programlama ortamları arasında farklılıklar gösterebilir. İşte biz bu makalemizde, Asp.Net 2.0 tarafından olaya bakmaya çalışıyor olacağız. İlk olarak dosya indirme işlemlerini ele alacağız. Sonrasında ise Asp.Net 2.0 ile gelen FileUpload aracı yardımıyla sunucuya dosya gönderme (Upload) işlemlerinin nasıl yapılabileceğini inceleyeceğiz. Ek olarak, upload edilen bir dosyanın kaydedilmeden, sunucu belleği üzerinde canlandırılıp işlenmesinin nasıl gerçekleştirilebileceğine bakacağız. Son olarakta, Upload edilen dosyaların bir veritabanı tablosunda alan (Field) olarak saklanması için gereken adımları göreceğiz. Dilerseniz vakit kaybetmeden dosya indirme süreci ile işe başlayalım.

Dosya indirme (Download) işlemlerinde bilinen IO tiplerinden ve Response sınıfının ilgili metodlarından yararlanılır. Hatırlanacağı gibi herhangibir resim dosyasını bir web sayfası içerisinde göstermek için üretilen HTML içeriği ile oynamak gerektiğinden daha önceki [makalemizde](https://www.buraksenyurt.com/post/Asp-Net-Temelleri-Tablo-Bazlc4b1-Resimleri-Ele-Almak-bsenyurt-com-dan) bahsetmiştik. Dosya indirme (Download) işlemindede içeriğin tipi (Content-Type), uzunluğu (Content-Length) gibi bilgiler önem kazanmaktadır. İlk örneğimizde, IIS üzerinde yayınlanan bir web projesindeki Dokumanlar isimli bir klasörde yer alan dosyaların indirilme işlemlerini gerçekleştirmeye çalışacağız. Bu amaçla web uygulamasına ait dokumanlar klasörü altına aşağıdaki şekildende görüldüğü üzere farklı formatlarda örnek dosyalar atılmasında fayda vardır.

![mk218_1.gif](/assets/images/2007/mk218_1.gif)

Web uygulamamızın ilk amacı Dokumanlar klasöründeki dosyaların listelenmesini sağlamak olacak. Bu amaçla sayfada bir GridView kontrolü kullanılabilir. Hatta bu kontrolün içeriği FileInfo tipinden nesnelerden oluşan generic bir liste koleksiyonundan (List) gelebilir. Böylece istemciler indirebilecekleri dosyalarıda görebilir. Download işleminin gerçekleştirilmesi için GridView kontrolünde bir Select Button'dan faydalanılabilir. İndirme işlemi sırasında indirilmek istenen dosyanın fiziki adresi, uzunluğu gibi bilgiler önemlidir. Bu bilgileri ve fazlasını FileInfo sınıfına ait bir nesne örneği yardımıyla elde edebiliriz. Uygulamamıza ait Default.aspx sayfasının içeriği aşağıdaki gibi olacaktır.

```text
<%@ Page Language="C#" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

protected void Page_Load(object sender, EventArgs e)
{
    if (!Page.IsPostBack)
    {
        string[] dosyalar = Directory.GetFiles(Server.MapPath("Dokumanlar"));
        List<FileInfo> dosyaBilgileri = new List<FileInfo>();

        foreach (string dosya in dosyalar)
        {
            dosyaBilgileri.Add(new FileInfo(dosya));
        }
        grdDosyalar.DataSource = dosyaBilgileri;
        grdDosyalar.DataBind();
    }
}

protected void grdDosyalar_SelectedIndexChanged(object sender, EventArgs e)
{
    string dosyaAdi = Server.MapPath("dokumanlar") + "\\" + grdDosyalar.SelectedRow.Cells[0].Text;
    FileInfo dosya = new FileInfo(dosyaAdi);

    Response.Clear(); // Her ihtimale karşı Buffer' da kalmış herhangibir veri var ise bunu silmek için yapıyoruz.
    Response.AddHeader("Content-Disposition","attachment; filename=" + dosyaAdi); // Bu şekilde tarayıcı penceresinden hangi dosyanın indirileceği belirtilir. Eğer belirtilmesse bulunulan sayfanın kendisi indirilir. Okunaklı bir formattada olmaz.
    Response.AddHeader("Content-Length",dosya.Length.ToString()); // İndirilecek dosyanın uzunluğu bildirilir.
    Response.ContentType = "application/octet-stream"; // İçerik tipi belirtilir. Buna göre dosyalar binary formatta indirilirler.
    Response.WriteFile(dosyaAdi); // Dosya indirme işlemi başlar.
    Response.End(); // Süreç bu noktada sonlandırılır. Bir başka deyişle bu satırdan sonraki satırlar işletilmez hatta global.asax dosyasında eğer yazılmışsa Application_EndRequest metodu çağırılır.
}
</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <title>Dosya Indirme Islemleri</title>
    </head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Dosyalar</h2>
            <asp:GridView ID="grdDosyalar" runat="server" AutoGenerateColumns="False" SelectedRowStyle-BackColor="Gold" OnSelectedIndexChanged="grdDosyalar_SelectedIndexChanged">
                <Columns>
                    <asp:BoundField DataField="Name" HeaderText="Dosya Adı" />
                    <asp:BoundField DataField="Length" HeaderText="Dosya Uzunluğu" />
                    <asp:BoundField DataField="Extension" HeaderText="Uzantısı" />
                    <asp:BoundField DataField="CreationTime" HeaderText="Oluşturulma Zamanı" DataFormatString="{0:dd.MMMM.yy}" HtmlEncode="False" />
                    <asp:BoundField DataField="LastWriteTime" HeaderText="Son Yazılma Zamanı" DataFormatString="{0:dd.MMMM.yy}" HtmlEncode="False" />
                    <asp:CommandField ButtonType="Button" SelectText="indir" ShowSelectButton="True" /> 
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
```

Öncelikli olarak sayfamızda neler yaptığımıza kısaca bakalım. Default.aspx sayfası ilk yüklendiğinde (bunu sağlamak için IsPostBack özelliği ile kontrol yapılmıştır) Dokumanlar klasöründeki dosyaların adları elde edilmektedir. Bu işlem için Directory sınıfının GetFiles metodu kullanılmaktadır. Bir web uygulaması söz konusu olduğu için, sanal klasörün karşılık geldiği fiziki adresi bulmak adına Server.MapPath metodu ele alınmaktadır. GetFiles metodu parametre olarak belirtilen klasördeki dosya isimlerinin elde edilmesini sağlamaktır. Bu nedenle geriye string tipinden bir dizi döndürür. GridView kontrolü içerisinde, elde edilen bu dosyalara ait bazı temel bilgilerin gösterilmesi hedeflenmiştir.

Örnekte dosyanın adı (Name), uzunluğu (Length), uzantısı (Extension), oluşturulma (CreationTime) ve son yazılma zamanı (LastWriteTime) bilgileri ele alınmaktadır. Elde edilen dosya adları aslında fiziki adresleride içermektedir. Bu nedenle ilgili dosya adları FileInfo tipinden örneklerin oluşturulmasında kullanılır. Bu nesne örnekleride generic koleksiyonda toplanır. Son olarak GridView kontrolüne veri kaynağı olarak generic liste koleksiyonu atanır. FileInfo ile gelen bilgilerden bazılarını GridView kontrolünde göstermek istediğimizden, AutoGenerateColumns özelliğine false değeri atanmış ve Columns elementi içerisinde ilgili alanlar açık bir şekilde yazılmıştır. BoundField elementlerine ait DataField niteliklerinin değerleri FileInfo ile gelen nesne örneklerindeki özellik adları olarak ayarlanmıştır.

İndirilmek istenen dosya için GridView kontrolüne bir adet CommandField elementi dahil edilmiştir. Burada seçme işlemi ele alınarak aslında küçük bir hile yapılmaktadır. GridView kontrolünde seçim düğmesine basıldıktan sonra devreye giren SelectedIndexChanged olayı içerisinde dosya indirme (Download) işlemi başlatılmaktadır. Teorik olarak Response sınıfının WriteFile metodu ile parametre olarak verilen dosya istemci bilgisayara indirilebilirmektedir. Ancak ön hazırlıklar yapılması gerekmektedir. Bu amaçla, indirilmek istenen dosya adı, GridView kontrolünde seçilen satıra ait ilk hücreden seçildikten sonra, Response sınıfının ilgili metodları ile ön hazırlıklar yapılır. İndirilecek dosya üretilen çıktının Header kısmında ele alınmaktadır.

Benzer şekilde indirilecek dosyanın uzunluğuda Header kısmına eklenir. Daha sonra içerik tipi belirlenir. application/octet-stream değeri, dosyanın ikili (binary) formatta indirileceğini belirtmektedir. Bu işlemlerin arkasındanda Response sınıfının WriteFile ve End metodları sırasıyla çalıştırılır. End metodu, o anki sayfaya ait yaşam sürecinin kesilmesini sağlamaktadır. Bir başka deyişle Response.End çağrısından sonra herhangibir kod satırı var ise işletilmeyecektir. Hatta, global.asax dosyasında yer alan ApplicationEndRequest metoduda devreye girecektir. Bu durumu analiz etmeden önceği örneğimizi test edelim. Uygulama çalıştırıldığında aşağıdakine benzer bir ekran görüntüsü ile karşılaşılacaktır.

![mk218_2.gif](/assets/images/2007/mk218_2.gif)

Burada pek çok ek özellik tasarlanabilir. Örneğin uzantıya göre içerik tipi değiştirilebilir. Hatta download işlemi yerine örnek bir dökümanın sayfaya çıktı olarak verilmesi sağlanabilir. PDF içeriklerinin tarayıcıda gösterilmesi buna örnek olarak verilebilir. Bunların dışında uygulamanın kullanıcıya göre yetkilendirilmesi ve sadece ele alabileceği dosyaları indirebilmesi sağlanabilir. Bu tamamen projenin ihtiyaçlarına ve geliştiricinin kullanıcılara sunmak istediklerine bağlı olarak gelişebilecek bir modeldir. indir başlıklık düğmelerden herhangibirine bastığımızda indirme işleminin aşağıdaki ekran görüntüsünde yer aldığı gibi başladığı görülür.

![mk218_3.gif](/assets/images/2007/mk218_3.gif)

Şimdi gelelim Response.End metodunun etkisine. Bu durumu analiz etmek için, Response.End sonrasına aşağıdaki gibi örnek bir kod satırı ekleyelim.

```csharp
protected void grdDosyalar_SelectedIndexChanged(object sender, EventArgs e)
{
    string dosyaAdi = Server.MapPath("dokumanlar") + "\\" + grdDosyalar.SelectedRow.Cells[0].Text;
    FileInfo dosya = new FileInfo(dosyaAdi);

    Response.Clear();
    Response.AddHeader("Content-Disposition","attachment; filename=" + dosyaAdi);
    Response.AddHeader("Content-Length",dosya.Length.ToString()); 
    Response.ContentType = "application/octet-stream"; 
    Response.WriteFile(dosyaAdi);
    Response.End(); 
    Response.Write("Dosya indirildi");
}
```

Hemen arkasından bilinen yaşam döngüsünü izlemek adına default.aspx sayfasını aşağıdaki gibi değiştirelim.

```csharp
protected void Page_PreInit(object sender, EventArgs e)
{
    Debug.WriteLine("Page_PreInit metodu"); 
}
protected void Page_Init(object sender, EventArgs e)
{
    Debug.WriteLine("Page_Init metodu"); 
} 
protected void Page_Load(object sender, EventArgs e)
{
    // Diğer kod satırları
    Debug.WriteLine("Page_Load metodu");
}
protected void Page_PreRender(object sender, EventArgs e)
{
    Debug.WriteLine("Page PreRender Metodu");
}
protected void Page_Unload(object sender, EventArgs e)
{
    Debug.WriteLine("Page Unload Metodu"); 
} 

protected void grdDosyalar_SelectedIndexChanged(object sender, EventArgs e)
{
    Debug.WriteLine("Dosya indirme işlemi başlıyor");
    // Diğer kod satırları
    Response.End(); 
    Debug.WriteLine("Response.End metodu çağırıldı");
    Response.Write("Dosya indirildi");
}
```

Bu değişikliklere ek olarak projeye global.asax dosyası ekleyip içerisine ApplicationEndRequest metodunu aşağıdaki gibi dahil edelim.

```csharp
void Application_EndRequest(object sender, EventArgs e)
{
    Debug.WriteLine("Application EndRequest Metodu Çağırıldı");
}
```

Şimdi uygulamayı Debug modda çalıştırıp output penceresindeki çıktıları izleyebiliriz. Bir dosya indirme işlemi gerçekleştirildikten sonra sayfanın yaşam döngüsü aşağıdaki gibi çalışacaktır.

```text
Page_PreInit metodu
Page_Init metodu
Page_Load metodu
Dosya indirme işlemi başlıyor
   A first chance exception of type 'System.Threading.ThreadAbortException' occurred in mscorlib.dll
   An exception of type 'System.Threading.ThreadAbortException' occurred in mscorlib.dll but was not handled in user code
Page Unload Metodu
Application EndRequest Metodu Çağırıldı
```

Çok doğal olarak GridView kontrolü üzerindeki düğmeye basıldığında sayfanın sunucuya tekrar gitmesi ve işlenmesi söz konusudur. Bu nedenle süreç PagePreInit ile başlamaktadır. Ancak dikkat edilecek olursa Response.End çağrısından sonraki satırlar devreye girmemiştir. Debug penceresine ve tarayıcıdaki çıktıya herhangibir kod yazılmamıştır. Dahası, bir exception (System.Threading.ThreadAbortException) fırlatılmış ve sayfa yaşam döngüsü PagePreRender metodunu işletmeden doğrudan PageUnload olayını işletmiş ve arkasından global.asax dosyasındaki ApplicationEndRequest devreye girmiştir. Elbetteki üretilen istisna (exception) Asp.Net çalışma ortamı tarafından görmezden gelinmektedir. Bu nedenle istemci herhangibir şekilde hata mesajı ile karşılaşmaz.

Gelelim makalemizin ikinci konusuna. İndirme işlemleri kadar Upload işlemleride önemlidir. Tabi burada istemcilerin her dosya tipini veya çeşidini sunucuya göndermesi doğru olmayabilir. Kapalı ağ (intranet) sistemlerinde bu söz konusu olabilir. Nitekim kimin handi dosyayı Upload ettiğinin belirlenmesi dışında, bu kişiye ulaşılmasıda kolaydır:). Ancak internet tabanlı daha geniş sistemlerde her ne kadar kullanıcılar tespit edilebilsede, kötü niyetli istemcilerin varlığı nedeniyle sistemin genelini tehlikeye atmamak adına tedbirler almak doğru bir yaklaşım olacaktır. Biz tabiki basit olarak Upload işlemlerini ele alacağız ve bahsettiğimiz güvenlik konularını şimdilik görmezden geleceğiz.

Upload işlemlerini kolaylaştırmak adına Asp.Net 2.0, FileUpload isimli bir kontrol getirmektedir. Bu kontrol basit olarak istemcinin Upload etmek istediği dosyayı seçebilmesini sağlamaktadır. Bu seçim işlemi ile birlikte, sunucuya gönderilmek istenen dosyaya ait bir takım bilgilerde FileUpload kontrolünce elde edilir. Örneğin içeriğin tipi kontrol edilerek sadece bazı dosyaların gönderilmesine izin verilebilir. İlk olarak web uygulamamıza aşağıdaki gibi Default2.aspx sayfasını ekleyelim.

![mk218_4.gif](/assets/images/2007/mk218_4.gif)

```text
<%@ Page Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

    protected void btnGonder_Click(object sender, EventArgs e)
    {
        if (uplDosya.HasFile)
        {
            uplDosya.SaveAs(Server.MapPath("Dokumanlar") + "\\" + uplDosya.FileName);
        }
        else
            Response.Write("Upload edilecek dosya yok");
    } 

</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <title>Upload Islemleri</title>
    </head>
<body>
    <form id="form1" runat="server">
        <div>
            Dosyayı Seçin : <asp:FileUpload ID="uplDosya" runat="server" />
            <br />
            <asp:Button ID="bntGonder" runat="server" Text="Gönder" OnClick="btnGonder_Click" />
        </div>
    </form>
</body>
</html>
```

Öncelikli olarak neler yaptığımıza bir bakalım. FileUpload kontrolü ile dosya seçilmesi, gönderme işlemi için yeterli değildir. Sayfanın sunucuya doğru gönderilmesi gerekmektedir. Bu işi basit olarak bir Button kontrolü üstelenebilir. Button kontrolümüze ait Click olay metodunda ise öncelikli olarak seçili bir dosya olup olmadığı HasFile özelliği ile kontrol edilmektedir. Bu kontrol, dosya seçmeden gönderme işlemi yapıldığı takdirde oluşacak hataların önüne geçilmesini sağlamaktadır. Eğer seçili olan bir dosya var ise basit olarak FileUpload sınıfının SaveAs metodu çağırılır. SaveAs metoduna dosyanın yol adresinin fiziki olarak verilmesi gerekmektedir. Bu nedenle yine Server.MapPath ile Dokumanlar klasörünün fiziki adresi elde edilir. FileUpload kontrolünün FileName özelliği ile seçilen dosyanın adı yakalanmakta ve fiziki adresin sonuna eklenmektedir. Eğer kod Debug edilirse, dosya seçildikten sonra Upload işlemi için düğmeye basıldığında FileUpload kontrolü üzerinde, seçilen dosyaya ilişkin çeşitli ek bilgilere ulaşılabildiği görülür.

![mk218_5.gif](/assets/images/2007/mk218_5.gif)

Söz gelimi dosyanın tipi hakkında fikir elde etmek için ContentType özelliğine bakılabilir. Buna göre belirli dosya tiplerinin indirilmesine izin verilmesi istenen durumlara karşı tedbirler alınabilir. Gönderilecek dosyanın boyutu ile ilgili olarak bir kısıtlama getirilmek isteniyorsa içerik uzunluğu ContentLength özelliği ile tedarik edilerek gerekli değişiklikler yapılabilir. Şimdi örneği deniyerek devam edelim. Basit olarak bir döküman dosyasını aşağıdaki gibi seçip Upload etmeyi deneyeceğiz.

![mk218_6.gif](/assets/images/2007/mk218_6.gif)

Görüldüğü üzere Browse işleminden sonra otomatik olarak standart Choose File iletişim penceresi (Dialog Box) ile karşılaştık. Örnekte bir resim dosyası seçilmiştir. Seçim işleminden sonra Gönder düğmesine basılırsa dosyanın başarılı bir şekilde Dokumanlar klasörü altına yazıldığı görülecektir.

> Upload işlemleri sırasında yetki problemi nedeni ile IIS altındaki herhangibir klasöre dosya yazma işlemi sırasında hata mesajı alınabilir. Bu durumda ASPNET kullanıcısına ilgili klasöre yazma hakkı verilmesi gerekebilir.

Upload işlemleri sırasında dikkat edilmesi gereken kritik bir sayı vardır. 4096 byte. Yani 4 megabyte. Boyutu bu değerin üzerindeki bir dosyayı Upload etmek istediğimizde Asp.Net ortamı bir hata üretir ve dosyanın sunucuya gönderilmesine izin vermez. Ne yazıkki hata üretimi kodların işletilmesinden önce gerçekleşir. Bu nedenle kullanıcıya anlamlı bir mesaj gösterilmeside pek mümkün olmamamaktadır. Eğer 4 megabyte üzerinde dosyaların upload edilebilmesini başka bir deyişle izin verilen limite kadar olan dosyaların gönderilebilmesini istiyorsak web.config dosyası içerisinde httpRuntime elementinin ayarlanması gerekmektedir. system.web boğumu içerisinde yer alan httpRuntime elementi sayesinde, http çalışma zamanına ait çeşitli ayarlamalar yapılabilmektedir. Bizim ihtiyacımız olan dosya büyüklüğü sınıfı ayarı için örneğimizde web.config dosyasını aşağıdaki gibi düzenlememiz yeterlidir.

```xml
<?xml version="1.0"?>
<configuration>
    <appSettings/>
    <connectionStrings/>
    <system.web>
        <httpRuntime maxRequestLength="51200"/>
        <compilation debug="true"/>
        <authentication mode="Windows"/>
    </system.web>
</configuration>
```

Burada yapılan ayarlamaya göre istemciler 50 megabyte'a kadar dosyaları sunucuya gönderebilecektir. Upload işlemlerini içerik yönetim sistemlerinde resim, döküman veya örnek uygulamaların, programların gönderilmesinde, grafik kütüphanesi tarzındaki sistemlerde çeşitli formatta resim veya akıcı görüntü formatlarının gönderilmesinde ve buna benzer durumlarda kullanmak yaygındır.

Gelelim makalemizin üçüncü konusuna. Yine istemciden sunucuya doğru bir dosya gönderme işlemi gerçekleştirmeyi hedeflediğimizi düşünelim. Ancak bu sefer XML tabanlı bir dosyayı gönderiyor olacağız. Bu dosyanın özelliği, bizim istediğimiz şekilde tasarlanmış olması dışında, sunucu tarafında anında işlenecek olmasıdır. Söz gelimi, XML dosyası içerisinde dinamik olarak sayfaya yüklenmesi istenen kontrollere ait bilgiler yer alabilir. Bu durumda Upload edilen XML dosyasının sunucu tarafında işlenerek bir çıktı üretilmesi gerekmektedir. Bir başka deyişle istemciden sunucuya gönderilen sayfayı, fiziki olarak yazmadan işlemek ve kullanmak istediğimizi göz önüne alıyoruz. Peki bu sistemi nasıl yazabiliriz? İlk olarak istemcinin göndereceği basit XML dökümanını hazırlayarak başlayalım. Örnek olarak aşağıdaki gibi bir içerik düşünülebilir.

```xml
<?xml version="1.0" encoding="utf-8"?>
<Kontroller>
    <Kontrol tip="MetinKutusu" id="metinKutusu1"/>
    <Kontrol tip="Dugme" metin="Gonder" id="gonder1"/>
    <Kontrol tip="Label" metin="Ad" id="ad1"/>
</Kontroller>
```

Olayı basit bir şekilde ele almak adına Xml içeriğini mümkün olduğu kadar basit düşünmeye çalıştık. Elbetteki çok daha karmaşık ve daha çok parametre sunan bir Xml dökümanı söz konusu olabilir. Şimdi bu dosyayı nasıl ele alacağımıza bakalım. Dikkat edilmesi gereken noktalardan birisi, Upload edilecek dökümanın XML formatında olması gerekliliğidir. Bunu sağlamak için, içerik tipine (ContentType) bakmak gerekecektir. Sonrasında ise Upload edilen dosyanın Framework içerisinde yer alan XML tipleri yardımıyla ele alınması yeterlidir. Sonuç olarak Default3.aspx dosyamızın içeriği aşağıdaki gibi olacaktır.

```text
<%@ Page Language="C#" %>
<%@ Import Namespace="System.Xml" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

    protected void btnGonder_Click(object sender, EventArgs e)
    {
        if (uplKontroller.HasFile)
        {
            HttpPostedFile dosya = uplKontroller.PostedFile;
            if (dosya.ContentType == "text/xml")
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(dosya.InputStream);
                XmlNodeList kontroller=doc.GetElementsByTagName("Kontrol");
                foreach (XmlNode kontrol in kontroller)
                { 
                    switch (kontrol.Attributes["tip"].Value)
                    { 
                        case "Dugme":
                            Button btn = new Button();
                            btn.ID = kontrol.Attributes["id"].Value;
                            btn.Text = kontrol.Attributes["metin"].Value;
                            phlKontroller.Controls.Add(btn); 
                            break;
                        case "MetinKutusu":
                            TextBox txt = new TextBox();
                            txt.ID = kontrol.Attributes["id"].Value;
                            phlKontroller.Controls.Add(txt); 
                            break;
                        case "Label":
                            Label lbl = new Label();
                            lbl.ID = kontrol.Attributes["id"].Value;
                            lbl.Text = kontrol.Attributes["metin"].Value;
                            phlKontroller.Controls.Add(lbl); 
                            break; 
                    }
                } 
            }
            else
                Response.Write("Dosya içeriği XML olmalıdır");
        }
        else
            Response.Write("Dosya bulunamadı");
    } 

</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <title>Upload Edilen Dosyayi O Anda Islemek</title>
    </head>
<body>
    <form id="form1" runat="server">
        <div>
            Xml Dosayasını Seçin : <asp:FileUpload ID="uplKontroller" runat="server" />
            <br />
            <asp:Button ID="btnGonder" Text="Gönder" runat="server" OnClick="btnGonder_Click" />
            <br />
            Yüklenen Kontroller:
            <asp:PlaceHolder ID="phlKontroller" runat="server" />
        </div>
    </form>
</body>
</html>
```

Dilerseniz kod içerisinde neler yaptığımıza kısaca bakalım. İlk olarak düğmeye basıldığında HasFile özelliği ile seçilen bir dosya olup olmadığını kontrol ediyoruz. Sonrasında ise FileUpload kontrolünün PostedFile özelliğinden yararlanıp seçilen dosyaya ait bazı temel bilgileri taşıyan HttpPostedFile tipine ait nesne örneğini elde ediyoruz. Bu nesne örneği üzerinden ContentType özelliğine geçerek içeriğin XML olup olmadığını text/xml eşleştirmesi ile kontrol ediyoruz.

Sonrasında ise okunan dosya içeriğini XmlDocument nesnesine yüklüyoruz. XmlDocument nesne örneğine ait Load metodunun parametresi olarak bir Stream kullanılabildiğinden, HttpPostedFile nesne örneğinin InputStream özelliğinden yararlanıyoruz. Son olarak yüklenen XML dökümanı içerisinde dolaşarak ilgili nitelikleri okuyor ve dinamik olarak oluşturulan kontrolleri, sayfadaki PlaceHolder kontrolünün Controls koleksiyonuna ekliyoruz.

> İşlemlerin daha sağlıklı olması açısından yüklenen XML içeriğinin belirli kurallara uygun olup olmadığı bir şema dosyası (XSD olabilir örneğin) yardımıyla kontrol edilebilir. Söz gelimi gelen XML dökümanının yapısı, element adları, nitelik adları veya tipleri bu şema yardımıyla denetlenip, kontrollerin belirli standartlara göre okunabilmesi sağlanmış olunur. Şema kontrolünün nasıl yapılabileceğine dair daha önceki bir [makalemizden](http://www.bsenyurt.com/MakaleGoster.aspx?ID=172) yararlanabilirsiniz.

Uygulamayı çalıştırdığımızda ve istemci tarafından Kontrollerim.xml dosyasını yüklediğimizde aşağıdaki ekran görüntüsünde olduğu gibi kontrollerin başarılı bir şekilde üretilip sayfaya yüklendiğini görebiliriz.

![mk218_7.gif](/assets/images/2007/mk218_7.gif)

Sıra geldi makalemizin son konusuna. İçerik yönetimi adına, istemcinin sunucuya gönderdiği dosyaların veritabanı üzerindeki bir tabloda tutulması istenebilir. Eğer ilişkisel veritabanı sistemi (Relational Database Management System) söz konusu ise, dosyaların tabloda tutuluyor olması taşıma işlemlerini kolaylaştırabileceği gibi, içerik güvenliğininde de daha etkin bir seviyede yapılabilmesini sağlayacaktır. (Tabi tersine saklanacak dosya boyutlarına göre veritabanı daha hızlı şişecektir.) Bu tarz bir ihtiyacın çıkış noktası son derece basittir.

Her zaman olduğu gibi bir FileUpload kontrolü ile istemciye dosya seçtirilmeli daha sonra ilgili içerik sunucu tarafında işlenerek veritabanındaki ilgili tabloya yazdırılmalıdır. Burada çalıştıralacak komut dışında tablodaki alan tipide önemlidir. Text tabanlı bir içerik gönderilecek olsada, tablo tarafında image veya VarBinary tipinden alanlar tutmak Unicode tutarlılığı açısından daha doğru bir yaklaşım olacaktır. Dilerseniz bir örnek ile bu durumu incelemeye çalışalım. İlk olarak içeriği saklayacağımız basit bir tablo oluşturalım. Bunun için SQL Server 2005 üzerinde aşağıdaki gibi bir tablo göz önüne alınabilir.

![mk218_8.gif](/assets/images/2007/mk218_8.gif)

Dosyalar isimli tabloda, sunucuya gönderilen dosya içeriğini saklamak için image tipinden bir alan kullanılmaktadır. Bunlara ek olarak dosyanın eklenme tarihi,içeriğin tipi ve dosya adı bilgileride yer almaktadır. Söz konusu alanlar dışında, web sitesinde kullanılan doğrulama (Authentication) sistemine göre, Upload işlemini yapan kullanıcının bilgilerinin saklanması hatta varsa Membership gibi kullanıcı tablo sistemleri ile ilişkilendirilmeside mümkün olabilir. Gelelim yükleme işlemini tabloya yazacak kodlarımıza.

```text
<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Configuration" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

    protected void btnGonder_Click(object sender, EventArgs e)
    {
        if (uplDosya.HasFile)
        {
            string conStr = ConfigurationManager.ConnectionStrings["ConStr"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("Insert into Dosyalar (EkleNmeTarihi,DosyaIcerigi,DosyaAdi,IcerikTipi) Values    (@EklenmeTarihi,@DosyaIcerigi,@DosyaAdi,@IcerikTipi)", conn);
                cmd.Parameters.AddWithValue("@EklenmeTarihi", DateTime.Now);
                cmd.Parameters.AddWithValue("@DosyaIcerigi", uplDosya.FileBytes);
                cmd.Parameters.AddWithValue("@DosyaAdi", uplDosya.FileName);
                cmd.Parameters.AddWithValue("@IcerikTipi", uplDosya.PostedFile.ContentType);
                conn.Open();
                int sonuc=cmd.ExecuteNonQuery();
                Response.Write(sonuc + " dosya aktarıldı"); 
            } 
        }
        else
            Response.Write("Dosya seçmelisiniz"); 
    } 

</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <title>Upload Edilen Dosyayı Veritabanına Yazma</title>
    </head>
<body>
    <form id="form1" runat="server">
        <div>
            Dosyayı Seçin : <asp:FileUpload ID="uplDosya" runat="server" />
            <br />
            <asp:Button ID="btnGonder" Text="Gönder" runat="server" OnClick="btnGonder_Click" />
        </div>
    </form>
</body>
</html>
```

Yine kodları kısaca incelemekte fayda var. Her zamanki gibi, istemcinin bir dosya seçtiğinden emin olduktan sonra gerekli işlemleri yapıyoruz. Burada önemli olan nokta çalıştırılacak SQL cümlesinde kullanılan parametrelerin değerlerinin nasıl verildiği. Dikkat edilecek olursa, image tipindeki alanın içeriğini verirken FileUpload kontrolünün FileBytes özelliğinden yararlanıyoruz. Burada dikkat edilmesi gereken noktalardan biriside çok büyük boyutlu dosyaların aktarılması sırasında yaşanabilecek timeout sorunudur.

Böyle bir durumda kalındığı takdirde bağlantı için timeout sürelerinin arttırılması yoluna gidilebilir yada dosyanın parçalanarak sunucuya gönderilmesi ve burada o şekilde ele alınması sağlanabilir. Uygulama çeşitli tipteki dosyalar ile test edildiğinde başarılı bir şekilde çalıştığı görülecektir. Aşağıdaki ekran görüntüsünde bir kaç dosya tipinin upload edilmesi sonrasındaki durum vurgulanmaktadır.

![mk218_10.gif](/assets/images/2007/mk218_10.gif)

Her dosya eklenme işleminden sonrada tarayıcı penceresindeki görüntü aşağıdaki gibi olacaktır.

![mk218_9.gif](/assets/images/2007/mk218_9.gif)

Elbette Upload edilen içeriklerin, istemciler tarafından indirilmeside gerekecektir. Bu durumda tablo alanındaki dosya içeriğinin stream olarak yazdırılması gerekir. Tabi bunun için Response sınıfının WriteFile metodu yerine BinaryWrite metodunu tercih edeceğiz. (Alternatif bir yaklaşım olarak, tablodan okunan dosya içeriğinin bir temp dosyaya atılması ve oradanda WriteFile metodu ile yazdırılmasıda düşünülebilir) Nitekim dosya içerikleri tabloda binary olarak tutulmaktadır. Öyleyse son olarak bu işlemide nasıl yapabileceğimizi inceleyeceğimiz bir örnek sayfa daha ekleyelim. Default5.aspx sayfamızın içeriği aşağıdaki gibi olacaktır.

![mk218_11.gif](/assets/images/2007/mk218_11.gif)

```text
<%@ Page Language="C#" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Configuration" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

    protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
    {
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStr"].ConnectionString))
        {
            SqlCommand cmd=new SqlCommand("SELECT DosyaAdi,DosyaIcerigi,IcerikTipi FROM Dosyalar Where Id=@Id",conn);
            cmd.Parameters.AddWithValue("@Id", GridView1.SelectedValue);
            conn.Open();
            SqlDataReader reader=cmd.ExecuteReader(); 
            if (reader.Read())
            {
                Response.Clear();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + reader.GetString(0));
                byte[] dosyaIcerigi = reader.GetSqlBinary(1).Value;
                Response.AddHeader("Content-Length", dosyaIcerigi.Length.ToString());
                Response.ContentType = "application/octet-stream";
                Response.BinaryWrite(dosyaIcerigi); 
            } 
            reader.Close(); 
        }
        Response.End();
    }

</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <title>Untitled Page</title>
    </head>
<body>
    <form id="form1" runat="server">
        <div>
            Dosyalar<br />
            <br />
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="dsDosyalar" OnSelectedIndexChanged="GridView1_SelectedIndexChanged" DataKeyNames="Id">
                <Columns> 
                    <asp:BoundField DataField="DosyaAdi" HeaderText="DosyaAdi" SortExpression="DosyaAdi" />
                    <asp:BoundField DataField="EklenmeTarihi" HeaderText="EklenmeTarihi" SortExpression="EklenmeTarihi" />
                    <asp:BoundField DataField="IcerikTipi" HeaderText="IcerikTipi" SortExpression="IcerikTipi" />
                    <asp:CommandField ButtonType="Button" SelectText="indir" ShowSelectButton="True" />
                </Columns>
            </asp:GridView>
            <asp:SqlDataSource ID="dsDosyalar" runat="server" ConnectionString="<%$ ConnectionStrings:ConStr %>" SelectCommand="SELECT Id,DosyaAdi, EklenmeTarihi, IcerikTipi FROM Dosyalar">
            </asp:SqlDataSource>
        </div>
    </form>
</body>
</html>
```

Tasarladığımız sayfayı dilerseniz inceleyelim. Basit olarak Dosyalar tablosunun içeriğini göstermek amacıyla SqlDataSource ve GridView kontrollerinden faydalanıyoruz. Yine ilk örneğimizde olduğu gibi indirme işlemini başlatmak adına küçük hilemizi yaptık ve bir Select düğmesi kullandık. Burada dosya içeriği tabloda alan olarak tutulduğu için, SqlCommand ile verinin çekilmesi gerekiyor. Bunu kolaylaştırmak adına GridView içerisinde seçilen satıra ait Id alanının değerini almalıyız. Bu amaçla GridView kontrolünün DataKeyNames özelliğine Id değerini verdik.

Bu değeri SelectedIndexChanged metodu içerisinden alarak sorgu cümlesinde parametre olarak kullanıyor ve böylece indirilmek istenen dosyaya ait içeriğin olduğu tablo satırını çekebiliyoruz. Bizim için önemli olan nokta, binary içeriği okumak için SqlDataReader sınıfının GetSqlBinary metodunu kullanıyor olmamız. Bu metod ile dönen tipin Value özelliğinden faydalanıp elde edilen byte[] dizisini Response sınıfının BinaryWrite metoduna parametre olarak verdiğimizde yazma işlemi gerçekleştirilmiş oluyor. Sonuç olarak çalışma zamanında istediğimiz sonuca ulaşıyor ve dosya indirme işlemlerini gerçekleştirebiliyoruz.

![mk218_12.gif](/assets/images/2007/mk218_12.gif)

Bu makalemizde Asp.Net uygulamalarında Download ve Upload işlemlerini detayları ile incelemeye çalıştık. İlk olarak bir dosyanın indirilme işleminin nasıl yapılabileceğine baktık. Sonrasında ise basit olarak bir Upload işlemi ile sunucuya dosya gönderme olayını ele aldık. Upload işleminin farklı yönlerini ele almak adına, anında sunucu tarafında işleme ve tabloya satır olarak ekleme işlemlerini inceledikten sonra, tablodaki bir binary içeriği indirme sürecine göz attık. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](/assets/files/2007/AspNetIOIslemleri.zip) (Dosyanın çok yer tutmaması açısından mdf dosyası çıkartılmıştır)