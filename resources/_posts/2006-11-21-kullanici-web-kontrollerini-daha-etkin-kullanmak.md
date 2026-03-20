---
layout: post
title: "Kullanıcı Web Kontrollerini Daha Etkin Kullanmak"
date: 2006-11-21 12:00:00 +0300
categories:
  - csharp
tags:
  - csharp
  - dotnet
  - aspnet
  - authentication
  - delegates
  - generics
---
Kullanıcı web kontrolleri (web user control) kendi içlerinde birden fazla kontrolü barındırabilen komposit tipteki görsel bileşenlerdir. Asp.Net 1.0/1.1 den veri var olan kullanıcı web kontrollerini çoğunlukla, birden fazla sayfada kullandığımız durumlarda (hatta aynı sayfada bir çok yerde aynı kontrol kümelerini kullanmak istediğimizde) düşünür ve geliştiririz. Bu kontroller tek bir yerde durdukları için güncelleştirilmeleri halinde, kullanıldıkları tüm sayfalara ilgili değişiklikler yansıyacaktır. Güncelleme kolaylığı, kullanıcı web kontrollerini tercih etmemizin en büyük nedenlerinden birisidir.

Ancak bu kontroller bir açıdan bakıldığında, herhangibir web sayfasına eklenen web sunucu kontrollerinden (web server control) yada html sunucu kontrollerinden (html server control) çok da farklı düşünülmemelidir. Nasıl ki bir web sunucu kontrolünün özellikleri (properties) ve olayları (events) oluyorsa bir web kullanıcı kontrolününde özellikleri ve olayları olabilir. Nihayetinde, bir kullanıcı tanımlı web kontrolü aslında System.Web.UI.UserControl sınıfından türerler ve kullanıldığı sayfanın bir parçası olarak sunucu tarafında nesne bazında ele alınırlar. Dolayısıyla kullanıldıkları sayfada yakalanabilecek olayları ve kullanılabilecek özellikleri olabilir. Hatta bir web kontrolünü nasıl dinamik olarak oluşturabiliyorsak bir kullanıcı web kontrolünü de dinamik olarak oluşturabilir ve sayfanın o anki içeriğine ekleyebiliriz. İşte bu makalemizde daha çok bu konular üzerinde durmaya çalışacağız. Temel olarak aşağıdaki üç maddede duracağız.

- Özellik (Property) kullanımı
- Kullanıcı web kontrollerini dinamik oluşturma
- Olay (Event) kullanımı

1. Özellik (Property) Kullanımı

Kullanıcı web kontrollerinin aslında System.Web.UI.Control tipinden türeyen sınıflar şeklinde ifade edilebileceğinden bahsetmiştik. Bu nedenle kullanıcı web kontrollerine özellik ekleyebiliriz. Bu özellikler yardımıyla örneğin bir kullanıcı tanımlı kontrol içerisindeki bileşenlerin bazı özelliklerine dışarıdan erişebilir hatta değiştirebiliriz. Dilerseniz örnek bir senaryo üzerinden gidelim. Aşağıdaki ekran çıktısına sahip olan bir web user control'ümüz olduğunu düşünelim.

![mk181_1.gif](/assets/images/2006/mk181_1.gif)

AdresBilgisi.ascx;

```text
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AdresBilgisi.ascx.cs" Inherits="AdresBilgisi" %>
<table>
    <tr>
        <td >İl</td>
        <td ><asp:DropDownList ID="ddlIl" runat="server" ></asp:DropDownList></td>
    </tr>
    <tr>
        <td >İlçe</td>
        <td ><asp:TextBox ID="txtIlce" runat="server"></asp:TextBox></td>
    </tr>
    <tr>
        <td >Cadde</td>
        <td ><asp:TextBox ID="txtCadde" runat="server"></asp:TextBox></td>
    </tr>
    <tr>
        <td >Sokak</td>
        <td ><asp:TextBox ID="txtSokak" runat="server"></asp:TextBox></td>
    </tr>
    <tr>
        <td >Posta Kodu</td>
        <td ><asp:TextBox ID="txtPostakodu" runat="server"></asp:TextBox></td>
    </tr>
</table>
```

AdresBilgisi.ascx.cs;

```csharp
public partial class AdresBilgisi : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string[] iller = new string[] { "Istanbul", "Ankara", "Izmir" };
        foreach (string il in iller)
            ddlIl.Items.Add(il);
    }
}
```

AdresBilgisi isimli kullanıcı tanımlı web kontrolümüz içerisinde basit olarak İl, İlçe, Cadde, Sokak, PostaKodu gibi bilgiler tutulmaktadır. Elbette burada sade düşünmek zorunda olduğumuzdan kontrolü mümkün olduğunca basit tasarladık. Normal şartlar altında bu kontrole doğrulama (validation) bileşenlerini ilave etmek, il ve ilçe gibi bilgileri bir birleri ile ilişkili olacak şekilde gerekli veri kümelerinden çekmek gibi işlemleri göz ardı ediyoruz. Odaklanacağımız nokta bu kontrol için kendi özelliklerimizi yazmak ve kullanmak olacak. Şimdi bu kontrollerden iki tanesini kullanacağımız aşağıdaki ekran görüntüsüne sahip bir web sayfası olduğunu düşünelim.

![mk181_2.gif](/assets/images/2006/mk181_2.gif)

Aslında senaryomuz oldukça tanıdık gelecektir. Bir alış veriş sitesinde, teslimat adresi ve fatura adresi bilgilerinin aynı olması halinde aynı tipten iki web user control arasında nasıl iletişim kuracağız. (Çok doğal olarak burada bir CheckBox kontrolü kullanıp teslima adresinin fatura adresi olraakta kullanılacağını belirtebilir ve işi tek bir kullanıcı tanımlı web kontrol ile halledebileceğimizi düşünebiliriz. Ancak böyle bir durumdada farklı bir Fatura adresi girilmesi mümkün olmayacaktır.) İşte bu nokta bize web user control içerisindeki kontrollerin dışarıdan erişebilir olmasını sağlayacak özellikler gerekecektir. Bu amaçla, AdresBilgisi isimli kullanıcı tanımlı web kontrollümüze aşağıdaki gibi özellikler eklememiz gerekecektir. Özelliklerimiz kullanıcı web kontrolü içerisindeki bileşenlerin Text, SelectedIndex gibi özelliklerinin değerlerini dış ortama açmaktan sorumludur.

```csharp
public int Il
{
    get { return ddlIl.SelectedIndex; }
    set { ddlIl.SelectedIndex = value; }
}
public string Ilce
{
    get { return txtIlce.Text; }
    set { txtIlce.Text = value; }
}
public string Cadde
{
    get { return txtCadde.Text; }
    set { txtCadde.Text = value; }
}
public string Sokak
{
    get { return txtSokak.Text; }
    set { txtSokak.Text = value; }
}
public string PostaKodu
{
    get { return txtPostakodu.Text; }
    set { txtPostakodu.Text = value; }
}
```

Artık AdresBilgisi kontrolü içerisindeki üyelere kod tarafında erişebilir, dahada önemlisi düşündüğümüz senaryoyu gerçekleyebiliriz. Bunun için tek yapmamız gereken default.aspx.cs içerisinde aşağıdaki kod parçasını yazmaktır.

```csharp
protected void btnAyniAdres_Click(object sender, EventArgs e)
{
    AdresBilgisi2.Il = AdresBilgisi1.Il;
    AdresBilgisi2.Ilce = AdresBilgisi1.Ilce;
    AdresBilgisi2.Cadde = AdresBilgisi1.Cadde;
    AdresBilgisi2.Sokak = AdresBilgisi1.Sokak;
    AdresBilgisi2.PostaKodu = AdresBilgisi1.PostaKodu;
}
```

Şimdi örneğimizi çalıştıralım ve Teslimat adresi bilgilerini girdikten sonra Fatura İçin Aynı Adresi Kullan düğmesine basalım.

İlk Durum;

![mk181_3.gif](/assets/images/2006/mk181_3.gif)

Düğmeye basıldıktan sonra;

![mk181_4.gif](/assets/images/2006/mk181_4.gif)

Sonuçta, AdresBilgisi1 isimli kullanıcı web kontrolü içerisindeki bileşenlere ait değerler AdresBilgisi2 isimli kullanıcı web kontrolü içerisinde bileşenlere aynen gönderilmiştir. Dolayısıyla hem kullanıcıyı aynı bilgileri girme zahmetinden kurtardık hemde aynı web user control'e ait birden fazla örnek arasında nasıl veri transferi yapabileceğimizi aydınlatmış olduk. Gelelim ikinci konumuza.

2. Kullanıcı web kontrollerini dinamik oluşturma

Çalışma zamanında (run time) web kontrollerini dinamik olarak oluşturmak hatta bunları aynı event metodlarına yönlendirmek özellikle portal tarzı sistemlerde önemlidir. Benzer davranışı kullanıcı web kontrolleri içinde gerçekleştirebiliriz. Burada dikkat edilmesi gereken husus, kullanıcı tanımlı web kontrolünün ele alınacağı sayfa içerisinde kayıt edilerek (register) bildirilmesi gerektiğidir. Bunun için Register direktifinden yararlanabiliriz. Örneğin AdresBilgisi.ascx kullanıcı tanımlı web kontrolünü dinamik olarak default2.aspx isimli bir sayfada kullanmak istediğimizi düşünelim. İlk olarak Register direktifini aşağıdaki gibi eklemeliyiz.

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default2.aspx.cs" Inherits="Default2" %>
<%@ Register Src="~/AdresBilgisi.ascx" TagName="Adres" TagPrefix="adr" %>
```

Aslında bir web user control herhangi bir sayfaya eklendiği zaman, bu direktif otomatik olarak eklenmektedir. Bizim şu anki amacımız kontrolden bir kaç tanesini dinamik olarak oluşturup ilgili web sayfasına eklemek olduğundan kod tarafında ilgili kontrol tipine erişebilmemiz gerekmektedir. Bu nedenle ilgili web kullanıcı kontrolünü register ettik. Dinamik olarak gerçekleştirilecek işlemler için sayfanın PageInit olay metodunu tercih edebiliriz. (Bu olay metodu bir web sayfasının yaşam döngüsünde genellikle kontrolün dinamik olarak oluşturulması istendiği durumlarda tercih edilmektedir. Elbetteki bu bir zorunluluk değildir. İstenildiğinde PagePreInit yada PageLoad gibi event metodlarınıda dinamik kontrol oluşturma amacıyla göz önüne alabiliriz.)

```csharp
protected void Page_Init(object sender, EventArgs e)
{
    AdresBilgisi kontrol1=(AdresBilgisi)LoadControl("AdresBilgisi.ascx");
    AdresBilgisi kontrol2 = (AdresBilgisi)LoadControl("AdresBilgisi.ascx");

    kontrol1.Ilce = "İlçe giriniz...";
    kontrol2.PostaKodu = "90000";

    phKontroller.Controls.Add(kontrol1); 
    phKontroller.Controls.Add(kontrol2);
}
```

Dikkat ederseniz, AdresBilgisi isimli kullanıcı web kontrollerini öncelikle yüklememiz gerekmektedir. Bu işlem için LoadControl metodu kullanılmıştır. Bu metod o anki sayfaya (Page) ait bir metoddur ve geriye Control tipinden bir nesne örneği döndürmektedir. Kontrolü bu şekilde yükledikten sonra hemen ilgili taşıyıcı nesneye eklemeyi tercih edebiliriz elbette. Ancak biz kontrolümüzün Ilce, PostaKodu gibi özelliklerinede kod içerisinde erişmek istediğimizden doğru bir tip dönüşümüne ihtiyacımız vardır. Bu nedenle LoadControl ile elde edilen Control nesnesi AdresBilgisi tipine cast operatörü yardımıyla dönüştürülmektedir. Örneğimizi çalıştırdığımızda phKontroller isimli PlaceHolder bileşeninin Controls koleksiyonuna, AdresBilgisi kontrollerinin aşağıdaki gibi eklendiği gözlemlenebilir.

![mk181_5.gif](/assets/images/2006/mk181_5.gif)

3. Olay (Event) Kullanımı

Kullanıcı web kontrollerini, sıradan bir web sunucu kontrolü olarak düşünmeye çalıştığımızda olaylara sahip olabileceğini de söyleyebiliriz. Örneğin bir Button nesnesi bir web sayfasına eklendiğinde ve bu nesneye mouse ile basıldığında Click event'i (eğer yüklenmiş ise) aktif hale gelir. Web tabanlı uygulamaların doğası gereği bu olaya ilişkin metodlar, her ne kadar sayfanın içerisindeki üyeler olsalarda hemen çalıştırılmazlar. Bunun sebebi, sunucu tarafında ilgili web sayfasının nesnel olarak üretilmesi sırasında çalışan yaşam döngüsü (page life cycle) dür. İstemci aslında bir düğmeye bastığında, sayfa istemci tarayıcı penceresinden sunucuya doğru içeriği ile birlikte gönderilir (Bu işlemi genellikle Postback olarak adlandırıyoruz). Bunun üzerine sunucu tarafında gönderilen sayfanın bir örneği oluşturulur ve çok genel hatları ile aşağıdaki yaşam döngüsü çalışır.

Page_PreInit
Page_Init
Page_Load
Button için Click / Change Olay Metodları
Page_PreRender
Page_UnLoad

Peki biz kendi geliştirdiğimiz kullanıcı web kontrollerine olay eklemek istersek ne olur. Öncelikle olarak sayfanın yaşam döngüsü aşağıdakine benzer bir sırada işleyecektir.

Aspx sayfası ilk talep edildiğinde;

```text
Default3.aspx Page_PreInit
  Urun.ascx Init 
  Urun.ascx Init 
Default3.aspx Page_Init
Default3.aspx Load
  Urun.ascx Load
  Urun.ascx Load
  Urun.ascx UnLoad 
  Urun.ascx UnLoad 
Default3.aspx Page_UnLoad
```

Sayfadaki Web User Control içerisinden bir olay tetiklendiğinde;

```text
Default3.aspx Page_PreInit
  Urun.ascx Init 
  Urun.ascx Init 
Default3.aspx Page_Init
Default3.aspx Load
  Urun.ascx Load
  Urun.ascx Load
Default3.aspx içinde çalışan web user control olayı
  Urun.ascx UnLoad 
  Urun.ascx UnLoad 
Default3.aspx Page_UnLoad
```

Yaşam döngüsünün işleyişini kısaca inceledikten sonra kullanıcı web kontrolleri için olayları nasıl yazabileceğimize bir bakalım. İlk olarak, bir temsilci (delegate) tanımlanır. Temsilcinin amacı, ilgili olay tetiklendiğinde devreye girecek olan olay metodunun yapısını tanımlamak ve çalışma zamanında bu metodun adresini işaret etmektir. Sonrasında ise bir event tanımlanır. Tanımladığımız event, user control'ü kullandığımız sayfalarda ele alınabilecektir.

Bir başka deyişle, kullanıcı web kontrolü içerisinde tanımlanan olay gerçekleştiğinde, sayfa içerisinde yazılan olay metodu devreye girecektir. Genel olarak olay metodları iki parametre ile çalışırlar. İlk parametre çoğunlukla olayı meydana getiren nesneyi referans eder. İkinci parametre ise, olay metoduna olay ile ilgili bir takım bilgiler aktarabilir. Örneğin EventArgs gibi. Dolayısıyla kendi parametre sınıflarımızı yazıp, olay metodu içerisine bilgi taşıyabiliriz. Şimdi dilerseniz bu işlemleri bir senaryo üzerinden incelemeye çalışalım.

Örnek senaryomuzda, Urun.ascx isimli bir web user control nesnemiz var. Bu nesne içerisinde dinamik olarak oluşturulan CheckBox kontrollerinde günler tutulmakta. Kullanıcı ürünlerin teslim edilebileceği günleri seçebilmektedir. Kontrol üzerinde yer alan düğmeye basıldığında ise, SecilenleriAl isimli bir olay manual olarak tetiklenmektedir. Önemli olan nokta, SecilenleriAl isimli olayın kontrolün kullanıldığı sayfa içerisinde yakalanabileceği ve ele alınabileceğidir. Bu kontrol içerisinde tanımladığımız temsilcimiz kendi içerisinde KontrolBilgileri isimli bir sınıfı olay parametresi olarak kullanmaktadır. Bir başka deyişle KontrolBilgileri sınıfını EventArgs sınıfı gibi düşünebiliriz. Önce KontrolBilgileri sınıfını tasarlayarak işe başlayalım.

```csharp
public class KontrolBilgileri
{
    private List<string> m_Kontroller;

    public List<string> Kontroller
    {
        get { return m_Kontroller; }
    }

    public KontrolBilgileri(PlaceHolder ph)
    {
        m_Kontroller = new List<string>();
        for (int i = 0; i < ph.Controls.Count; i++)
        {
            if (ph.Controls[i] is CheckBox)
            {
                CheckBox currentChk=ph.Controls[i] as CheckBox;
                if (currentChk.Checked)
                    m_Kontroller.Add(currentChk.Text);
            }
        }
    }
}
```

KontrolBilgileri sınıfının görevi, olay metodu içerisine, kullanıcı web kontrolünde seçili olan CheckBox bileşenlerinin Text bilgilerini string tipinden elemanlar taşıyan generic bir koleksiyon şeklinde döndürmektir. Olay içine bilgi taşıyacak sınıfımızı tasarladıktan sonra kullanıcı web kontrolümüzü aşağıdaki gibi geliştirebiliriz.

Urun.ascx;

```text
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Urun.ascx.cs" Inherits="Urun" %>
<asp:PlaceHolder ID="phGunler" runat="server"></asp:PlaceHolder>
<br />
<br />
<asp:Button ID="btnIsaretle" runat="server" Text="Isaretle" OnClick="btnIsaretle_Click" />
```

Urun.ascx.cs;

```csharp
public partial class Urun : System.Web.UI.UserControl
{
    public delegate void SecilenleriAlHandler(KontrolBilgileri args);
    public event SecilenleriAlHandler SecilenleriAl;

    protected void Page_Load(object sender, EventArgs args)
    {
        string[] gunler = new string[] { "Pazartesi", "Salı", "Çarşamba", "Perşembe", "Cuma", "Cumartesi", "Pazar" };
        foreach (string gun in gunler)
        {
            CheckBox chkBox = new CheckBox();
            chkBox.ID = gun;
            chkBox.Text = gun;
            phGunler.Controls.Add(chkBox);
            LiteralControl lc = new LiteralControl("<br/>");
            phGunler.Controls.Add(lc);
        }
    }
    protected void btnIsaretle_Click(object sender, EventArgs e)
    {
        if (SecilenleriAl != null)
            SecilenleriAl(new KontrolBilgileri(phGunler));
    }
}
```

Kontrolümüzde, SecilenleriAlHandler isimli temsilci (delegate) ve SecilenleriAl isimli olay (event) üyesi yer almaktadır. btnIsaretle isimli düğmemize ait Click olay metounda, SecilenleriAl isimli bir olay kontrolü kullanan bir sayfa tarafından yüklenmiş ise manual olarak tetiklenmektedir. Bu şu anlama gelir. Eğer kontrolü kullandığımız bir web sayfasında, bu kontrole ait SecilenleriAl isimli bir olay yüklenmiş ise, bununla ilişkili olay metodu çalıştırılacaktır. Bu amaçla default3.aspx isimli örnek bir sayfada aşağıdaki kodları geliştirelim.

default3.aspx;

![mk181_6.gif](/assets/images/2006/mk181_6.gif)

![mk181_7.gif](/assets/images/2006/mk181_7.gif)

default3.aspx.cs;

```csharp
public partial class Default3 : System.Web.UI.Page
{
    protected void Secilenler(KontrolBilgileri kb)
    {
        for (int i = 0; i < kb.Kontroller.Count; i++)
            Response.Write(kb.Kontroller[i]+"<br/>");
    }
}
```

Uygulamamız çalıştırdığımızda, user control üzerinde bazı seçenekleri işaretlediğimizde ve düğmeye bastığımızda o sayfa üzerinde tanımladığımız Secilenler isimli olay metodunun çalıştığını ve seçilen günlerin ekrana yazıldığını görebiliriz.

![mk181_8.gif](/assets/images/2006/mk181_8.gif)

Dikkat edilmesi gereken bir nokta vardır. Çoğunlukla kontrolleri dinamik olarak oluşturup bir web sayfasına eklediğimizde bu kontrollerin hepsini tek bir olay metoda yönlendirebiliriz. İlgili olay metodu içerisinde hangi kontrolden gelindiğini anlamak için ise genellike object tipinden gelen ve ismi çoğunlukla sender olarak bilinen değişkeni ilgili tipe cast ederiz. Benzer yapıyı Urun.ascx kontrolü için düşündüğümüzde, SecilenleriAl isimli olay metoduna aynı işlevselliği sağlayacak eklentiyi yapamız gerekecektir. Bunun için temsilciyi, olayı tetikleyen metoddaki çağrıyı ve sınıf üzerindeki kontrole ait olay metodunu aşağıdaki gibi değiştirmemiz yeterli olacaktır.

Urun.ascx.cs'de temsilci;

```csharp
public delegate void SecilenleriAlHandler(object gonderen,KontrolBilgileri args);
```

Urun.ascx.cs'de olayı tetikleyen buttonClick metodu;

```csharp
protected void btnIsaretle_Click(object sender, EventArgs e)
{
    if (SecilenleriAl != null)
        SecilenleriAl(this,new KontrolBilgileri(phGunler));
}
```

default3.aspx.cs'deki Secilenler olay metod;

```csharp
protected void Secilenler(object sender,KontrolBilgileri kb)
{
    for (int i = 0; i < kb.Kontroller.Count; i++)
        Response.Write(kb.Kontroller[i]+"<br/>");
}
```

Bu değişiklik bize, kontrole ait olay metodu içerisinde hangi web kullanıcı kontrolünün tetiklendiğini algılama imkanı verecektir. Böylece birden fazla web kullanıcı kontrolünü aynı olay metoduna yönlendirdiğimizde her birinin özelliklerini ve üyelerini aynı metod içerisinde ele alabiliriz. Örneğin Urun.ascx içerisinde günler dışında ekstradan bilgi girilebilecek notların olduğu bir TextBox olduğunu ve bu TextBox'ın içeriğine dış ortamdan erişebilmemizi sağlayan read only bir özellik eklediğimizi düşünelim.

```csharp
public string Notlar
{
    get{return txtNot.Text;}
}
```

Default3.aspx sayfasında Urun.ascx kontrolünden örneğin 2 tane olduğunu ve bunların aynı olay metoda yönlendirildiğini göz önüne. Bu durumda ilgili olay metodu içerisinde, hangi kontrol tarafından bir tetikleme olduğunu tespit edebilir. Çünkü artık olayı tetikleyen nesneyi taşıyan bir parametremiz vardır. Dolayısıyla bu parametre üzerinden ilgili Notlar özelliğe geçebilir ve hangi kontrolden gelindiyse onun notlarını alabiliriz.

```csharp
protected void Secilenler(object sender,KontrolBilgileri kb)
{
    Urun urn = (Urun)sender;
    Response.Write(urn.Notlar + "<br/>");

    for (int i = 0; i < kb.Kontroller.Count; i++)
        Response.Write(kb.Kontroller[i]+"<br/>");
}
```

![mk181_9.gif](/assets/images/2006/mk181_9.gif)

Bu makalemizde kullanıcı web kontrollerini daha etkin şekilde nasıl kullanabileceğimizi incelemeye çalıştık. Aslında çıkış noktamız bir web kullanıcı kontrolünü normal bir web sunucu kontrolüne benzer bir sınıf olarak düşünmekti. Bu sayede kullanıcı web kontrollerine özellik, olay ekleyebileceğimizi ve dinamik olarak çalışma zamanında oluşturabileceğimizi gördük. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek kod için tıklayın.](/assets/files/2006/UsingWebUserControls.rar)