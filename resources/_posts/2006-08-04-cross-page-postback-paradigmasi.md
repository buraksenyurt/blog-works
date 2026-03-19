---
layout: post
title: "Cross-Page Postback Paradigması"
date: 2006-08-04 12:00:00 +0300
description: "Asp.Net 2.0 ile gelen en önemli yeniliklerden biriside Cross-Page Postback mimarisidir. Cross-Page Postback, bir sayfanın tüm içeriği ile başka bir sayfaya doğru gönderilebilmesini sağlar. Bunu bir asp.net sayfasının kendi üzerine değilde, hedef gösterilen bir sayfaya doğru gönderilmesi olarakta düşünebiliriz."
categories:
  - aspnet-2-0
tags:
  - aspnet-2-0
  - csharp
  - dotnet
  - aspnet
  - http
  - authentication
  - java
  - javascript
---
# Cross-Page Postback Paradigması
Değerli Okurlarım Merhabalar,

Asp.Net 2.0 ile gelen en önemli yeniliklerden biriside Cross-Page Postback mimarisidir. Cross-Page Postback, bir sayfanın tüm içeriği ile başka bir sayfaya doğru gönderilebilmesini sağlar. Bunu bir asp.net sayfasının kendi üzerine değilde, hedef gösterilen bir sayfaya doğru gönderilmesi olarakta düşünebiliriz. Böylece hedef sayfa içerisinden, kaynak sayfadaki (yada kaynak sayfalardaki) verilere erişebilme imkanı sağlanmış olunur.

Bu aynı zamanda sayfalar arasında veri taşımanın etkili yollarından birisi olarak, web uygulamalarındaki yerini almıştır. Cross-Page Postback oldukça güçlü bir yenilik olmasına karşın kullanırken dikkat edilmesi gereken bazı hususlar vardır. Bu makelemizde dikkat edilmesi gereken noktaları örnekler üzerinden incelemeye çalışacağız. Aşağıdaki tabloda Cross-Page Postback işlemi sırasında dikkat etmemiz gereken noktalar maddeler halinde özetlenmeye çalışılmıştır.

Cross-Page Post Back Kullanırken

1
Cross-Page Postback işlemini yapabilen web tabanlı bileşenler sadece IButtonControl arayüzünü (interface) uygulamış web kontrolleridir. Bu nedenle hedef sayfada Null Reference kontrolü mutlaka yapılmalıdır.

2
Cross-Page Postback işlemi sırasında, kaynak sayfaya (sayfalara) ait referans (referanslar) kullanılmak istendiğinde bu sayfaya (sayfalara) ait nesne örneği (örnekleri) oluşturulur. Dolayısıyla bu kaynak sayfanın yaşam döngüsünde (Web Page Life Cycle) yer alan olay metodlarında çalışması ama görmezden gelinmesi anlamına gelir.

3
Kaynak sayfada tanımlı herhangibir özelliğe hedef sayfa içerisinden erişebilmek için, kaynak sınıfa ait bir nesne örneğine ihtiyaç vardır.

4
Birden fazla sayfadan tek bir sayfaya doğru Cross-Page Postback işlemi yapılabilir. Ancak bu durumda hedef sayfaya hangi sayfadan gelindiğinin anlaşılması kaynak sayfa kontrollerinin doğru bir şekilde tespit edilebilmesi için şarttır.

5
Kaynak sayfa eğer doğrulama kontrolleri (validation controls) içeriyorsa ve istemci taraflı kontrol scriptleri (özellikle java script'ler) çalışıtırılamıyorsa yada kapalıysa, hedef sayfaya yinede geçiş yapılabilir. Bu kaynak sayfaya ait doğrulama işlemlerinin komple atlanması anlamına gelir.

Dilerseniz bu maddeleri teker teker incelemeye çalışalım.

Madde 1:

Web kontrollerinden olan Button, LinkButton ve ImageButton bileşenleri, IButtonControl'den türemişlerdir. Bu sebepten dolayı PostBackUrl isimli özellikleri vardır. PostBackUrl bildiğiniz gibi, Cross-Page Postback sırasında hedef olarak gidilecek url bilgisini içermektedir. Tipik olarak hedef sayfa içerisinden kaynak sayfaya ait herhangibir kontrolün içeriğini almak (örneğin kaynak sayfadaki bir TextBox içerisine girilen bir değeri) için FindControl metodu kullanılır. Lakin hedef sayfaya geçmek için HyperLink yada Response.Redirect metodu gibi tekniklerde kullanılabilir. Bu ise Cross-Page Postback olmaması anlamına gelmektedir.

Madde 1' i daha net anlayabilmek için basit bir örnek geliştirelim. Kaynak1.aspx ve Hedef.aspx isimli iki web sayfamız olduğunu düşünelim. Kaynak1.aspx sayfasından, Hedef.aspx sayfasına geçiş yapmak için iki farklı teknik kullanıyoruz. Birincisinde PostBackUrl özellikleri set edilmiş olan Button, LinkButton ve ImageButton kontrollerini ele alıyoruz. Bu kontrollerin PostBackUrl özelliklerine ilgili değerleri set ederekten Cross-Page Postback işlemini gerçekleştiriyoruz. Diğer seçeneğimizde ise Response.Redirect metodu ve HyperLink kontrolünü kullanarak hedef sayfaya bir geçiş yapmaktayız.

Kaynak1.aspx;

![mk170_1.gif](/assets/images/2006/mk170_1.gif)

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Kaynak1.aspx.cs" Inherits="Kaynak1" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<title>Untitled Page</title>
</head>
<body>
<form id="form1" runat="server">
<div>
    <strong>Gönderilecek Bilgi <br /></strong>
    <asp:TextBox ID="txtInfo" runat="server"></asp:TextBox><br /><br />
    <table border="1">
    <tr>
        <td><strong><span style="color: green; font-family: Verdana">PostBackUrl ile</span></strong></td>
    </tr>
    <tr>
        <td ><asp:Button ID="btnGoTarget" runat="server" PostBackUrl="~/Hedef.aspx" Text="Hedefe Git" /><br/><br/>    
                <asp:LinkButton ID="lnkGoTarget" runat="server" PostBackUrl="~/Hedef.aspx">Hedefe Git</asp:LinkButton><br /><br />
                <asp:ImageButton ID="imbGoTarget" runat="server" Height="64px" ImageAlign="Middle" ImageUrl="~/Beaver.bmp" PostBackUrl="~/Hedef.aspx" Width="64px" /></td>
    </tr>
    <tr>
        <td><strong><span style="color: red; font-family: Verdana">PostBackUrl olmadan</span></strong></td>
    </tr>
    <tr>
        <td><asp:Button ID="btnGoTarget2" runat="server" OnClick="btnGoTarget2_Click" Text="Hedefe Git"/><br/><br />
               <asp:HyperLink ID="hlGoTarget" runat="server" NavigateUrl="~/Hedef.aspx">Hedefe Git</asp:HyperLink></td>
    </tr>
</table>
</div>
</form>
</body>
</html>
```

Kaynak1.aspx.cs;

```csharp
protected void btnGoTarget2_Click(object sender, EventArgs e)
{
    Response.Redirect("~/Hedef.aspx");
}
```

Hedef.aspx.cs;

```csharp
protected void Page_Load(object sender, EventArgs e)
{
    Response.Write(((TextBox)PreviousPage.FindControl("txtInfo")).Text);
}
```

Uygulamayı çalıştırdıktan sonra, HyperLink veya PostBackUrl özelliği set edilmemiş olan kontrollerimizden herhangibiri üzerinden hedef sayfaya geçiş yaptığımızda aşağıdaki hata mesajı ile karşılaşırız.

![mk170_2.gif](/assets/images/2006/mk170_2.gif)

Bu hata mesajının sebebi son derece açıktır. Nitekim HyperLink kontrolü yada Response.Redirect metodu ile hedef sayfaya geçiş yapıldığından PreviousPage referansı null olarak gelmektedir. Yani ortada bir Cross-Page Postback işlemi söz konusu değildir. Bu durum, hedef sayfaya ait Url bilgisinin doğrudan talep edilmesi halinde de geçerlidir. Öyleyse, ilgili yerlerde PreviousPage tipinin null olup olmadığı kontrol edilmeli ve işlemler buna göre yapılmalıdır. Çözüm olarak Hedef.aspx sayfasında aşağıdaki kontrolü yapmak yeterli olacaktır.

```csharp
if (PreviousPage != null)
    Response.Write(((TextBox)PreviousPage.FindControl("txtInfo")).Text);
```

Madde 2:

Bir Cross-Page Postback işlemi meydana geldiğinde, hedef sayfada PreviousPage referansının ilk kullanıldığı yer kaynak sayfaya ait bir nesne örneğinin oluşturulmasına yol açacaktır. Dolayısıyla kaynak sayfanın yaşam döngüsü içerisinde yer alan olaylardan, geliştirici tarafından kodlanmış olanları çalışacaktır. Ancak bu kodlar ve sonuçları görmezden gelinecektir. Bunu daha iyi anlayabilmek için Kaynak1.aspx.cs dosyasına aşağıdaki eklemeleri yapalım.

```csharp
private void Page_Init(object sender, EventArgs e)
{
    Response.Write("Init metodu..."); 
}
protected void Page_Load(object sender, EventArgs e)
{
    Response.Write("Page Load metodu çalışıyor...");
}
protected void btnGoTarget_Click(object sender, EventArgs e)
{
    Response.Write("Go Target düğmesine basıldı...");
}
private void Page_PreRender(object sender, System.EventArgs e)
{
    Response.Write("PreRender çalışıyor...");
}
private void Page_Unload(object sender, System.EventArgs e)
{
}
```

Kodu bu aşamasındayken debug ederek izlersek aşağıdaki şekilde anlatılmaya çalışılan yolu izlediğini görebiliriz.

![mk170_3.gif](/assets/images/2006/mk170_3.gif)

Görüldüğü gibi, kaynak sayfa yüklendikten sonra PostBackUrl özelliği set edilmiş herhangibir kontrole basılması halinde kod ilk olarak Hedef sayfanın yaşam döngüsünden işletilmeye başlanacaktır. Bu döngü Hedef sayfaya aittir. Dolayısıyla bir asp.net web sayfası için geçerli olan yaşam döngüsündeki işleyiş geçerlidir. Ne varki, hedef sayfada PreviousPage referansı ile kaynak sayfaya yapılacak ilk talepte, sunucu tarafında Kaynak sayfaya ait nesne örneğinin oluşturulması işlemleri başlayacaktır. Buda doğal olarak Kaynak sayfanın yaşam döngüsünün tekrar tetiklenmesi, Init, Load, Click, PreRender ve Unload gibi olayların yeniden çalışması anlamına gelmektedir. Bu sadece kaynak sayfaya ait nesne örneğinin elde edilmesi sırasında olması beklenen bir prosedürdür. İşin ilginç yanı, kaynak sayfaya ait nesne örneklenirken, buradaki kodların görmezden gelinmesidir.

![mk170_4.gif](/assets/images/2006/mk170_4.gif)

Bu maddeyi daha net anlayabilmek için klasik bir Asp.Net sayfasının yaşam döngüsünü bilmekte fayda vardır. Yukarıdaki şekil bunu göstermektedir. Burada görülen changed ve click olayları çoğunlukla Web kontrollerine ait olaylardır. Bir sayfa ilk yüklendiğinde Change ve Click olayları devreye girmez. Genellikle bir Asp.Net sayfası üzerinde herhangibir şekilde PostBack işlemi yapıldığında var olan yaşam döngüsü içerisinde sırasıyla Changed ve Click olaylarıda eklenir.

Madde 3:

Bazı durumlarda kaynak sayfadaki herhangibir public üyeye, hedef sayfa üzerinden erişmek isteyebiliriz. Böyle bir durumda eğer herhangibir şey belirtmessek PreviousPage tipi ilgili üyelere doğrudan erişilmesi mümkün değildir. Bu çoğunlukla kaynak sayfaya ait sınıf içerisinde bir özellik (property) tanımlandığı zaman rastlanacak bir durumdur. İlgili özelliğe erişebilmek için öncelikle kaynak sayfaya ait nesne örneğinin hedef sayfada kullanılabilir olması gerekmektedir. Durumu daha iyi anlayabilmek için ilk olarak kaynak sayfa sınıfımıza bir özellik yazacağız. Daha sonra ise PreviousPage yardımıyla bu özelliğe hedef sayfa üzerinden erişmeye çalışacağız. Örneği geliştirmek için Kaynak1.aspx.cs kodlarını aşağıdaki gibi değiştirelim.

Kaynak1.aspx.cs için ek;

```csharp
private DateTime _istekZamani;

public DateTime IstekZamani
{
    get { return _istekZamani; }
}
```

Hedef.aspx.cs sayfasına ait Load metodu içerisinde IstekZamani isimli özelliğe erişmeye çalıştığımızda tasarım zamanında aşağıdaki hata mesajı ile karşılaşırız.

![mk170_5.gif](/assets/images/2006/mk170_5.gif)

Sorun Kaynak1.aspx sayfasına ait arka plan sınıfının nesne örneğinin doğru bir şekilde ele alınmayışından kaynaklanmaktadır. Çözüm olarak hedef sayfaya, kaynak sayfanın tipini söylememiz yeterli olacaktır. Bunun için Hedef.aspx sayfasında Page direktifinin hemen altına aşağıdaki gibi PreviousPageType direktifini eklememiz gerekmektedir.

```csharp
<%@ PreviousPageType VirtualPath="~/Kaynak1.aspx" %>
```

Bu haliyle uygulamayı derlediğimizde hiç bir sorunla karşılaşmadan IstekZamani isimli özelliğe erişebildiğimizi görürüz.

Madde 4:

Cross-Page Postback işlemi özellikle sayfalar arasında veri taşıma işlemleri arasında önemli bir yere sahiptir. Bazı durumlarda hedef sayfaya birden fazla sayfadan Cross-Page Postback işlemi gerçekleştirebiliriz. Ancak böyle bir senaryoda, hedef sayfaya hangi tipten gelindiğinin anlaşılması şart olacaktır.

![mk170_6.gif](/assets/images/2006/mk170_6.gif)

Senaryoyu daha net anlayabilmek için örnek uygulamamıza Kaynak2.aspx isimli yeni bir web sayfası daha ekliyoruz. Bu sayfa da Hedef.aspx sayfasına doğru post işlemini gerçekleştirebilecek button kontrolleri içermektedir.

Kaynak2.aspx;

![mk170_7.gif](/assets/images/2006/mk170_7.gif)

```csharp
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Kaynak2.aspx.cs" Inherits="Kaynak2" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<title>Untitled Page</title>
</head>
<body>
<form id="form1" runat="server">
<div>    
    <strong>Gönderilecek Bilgi<br /></strong>
    <asp:TextBox ID="txtInfo2" runat="server"></asp:TextBox><br /><br />
    <table border="1">
    <tr>
        <td><strong><span style="color: green; font-family: Verdana">PostBackUrl ile</span></strong></td>
    </tr>
    <tr>
        <td><asp:Button ID="btnGoTarget" runat="server" OnClick="btnGoTarget_Click" PostBackUrl="~/Hedef.aspx"
Text="Hedefe Git" /></td>
    </tr>
    </table>
</div>
</form>
</body>
</html>
```

Bu sefer Kaynak2.aspx sayfasındaki TextBox kontrolünün adını Kaynak1.aspx'den farklı olarak txtInfo2 olarak değiştirdik. Şimdi uygulamamızı test ettiğimizde Kontrol1.aspx'den yapılan geçişlerde bir problem olmadığını ancak Kaynak2.aspx'ten yapılan geçişlerde aşağıdaki istisna mesajını aldığımızı görürüz.

![mk170_8.gif](/assets/images/2006/mk170_8.gif)

Bu son derece doğaldır. Nitekim PreviousPage referansının işaret ettiği sayfada txtInfo isimli bir kontrol bulunmamaktadır. Dahası bizim PreviousPage tipimiz şu anda hangi sayfadan buraya gelindiğini de bilmemektedir. Madde 3 ' te yaptığımız gibi PreviousPageType direktifini kullanmayı tercih edebiliriz. Ancak bu direktif sadece bir kez tanımlanabilmektedir. Dolayısıyla aşağıdaki gibi bir kullanım geçersizdir.

![mk170_9.gif](/assets/images/2006/mk170_9.gif)

Öyleyse çözüm? Çözüm olarak, Reference direktifi kullanılabilir.

```text
<%@ Reference Page="~/Kaynak1.aspx" %>
<%@ Reference Page="~/Kaynak2.aspx" %>
```

Elbette kod tarafında da yapılması gereken bir takım değişiklikler vardır. Reference direktifleri sayesinde Hedef sayfada kullanılabilecek referans tiplerini bildirmiş oluruz. Bu durumda Hedef sayfada aşağıdaki değişiklikleri yapmamız yeterli olacaktır.

```csharp
if (PreviousPage != null)
{
    if (PreviousPage is Kaynak1)
    {
        Kaynak1 kyn1 = PreviousPage as Kaynak1;
        Response.Write(((TextBox)kyn1.FindControl("txtInfo")).Text);
    }
    if (PreviousPage is Kaynak2)
    {
        Kaynak2 kyn2 = PreviousPage as Kaynak2;
        Response.Write(((TextBox)kyn2.FindControl("txtInfo2")).Text);
    } 
}
else
    Response.Write("Cross-Page Postback işlemi yok...");
```

Bu durumda uygulama sorunsuz olarak çalışacaktır.

Madde 5:

Gelelim bir diğer önemli konuya. Doğrulama (Validation) işlemleri bildiğiniz gibi istemci tarafından başlar ve sunucu tarafında tekrar edilir. Asp.Net 2.0' ın kullandığı Validation Kontrolleri, hem istemci taraflı script'leri hemde sunucu taraflı kodları otomatik olarak hazırlamaktadır. Istemcilerin script desteğinin olmaması ihtimaline karşılıkta sunucu tarafında mutlaka ve mutlaka doğrulama işlemleri yapılır. Dolayısıla istemci tarafında doğrulama işlemleri başarılı olsa dahi sunucu tarafında bu kontroller tekrar yapılacaktır. Ancak istemci tarafında script desteğinin olmaması halinde Cross-Page postback işleminde yaşanan bir problem vardır. Eğer istemci tarafı script desteği kapalı ise kaynak sayfadaki doğrulama işlemleri Cross-Page işlemi nedeni ile atlanmaktadır. Dilerseniz örnek üzerinden devam ederek konuyu daha net anlamaya çalışalım. Bu amaçla aşağıdaki ekran görüntüsüne sahip Kaynak3.aspx isimli bir web sayfası oluşturuyoruz.

Kaynak3.aspx;

![mk170_10.gif](/assets/images/2006/mk170_10.gif)

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Kaynak3.aspx.cs" Inherits="Kaynak3" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<title>Untitled Page</title>
</head>
<body>
<form id="form1" runat="server">
<div>
<strong>
Email adresinizi girin : </strong>
<br />
<asp:TextBox ID="txtEmail" runat="server"></asp:TextBox>
<br />
<br />
<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtEmail" ErrorMessage="Email girmelisiniz." EnableClientScript="False"></asp:RequiredFieldValidator><br />
<asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtEmail" ErrorMessage="Geçersiz mail adresi" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" EnableClientScript="False"></asp:RegularExpressionValidator><br />
<br />
<asp:Button ID="btnSend" runat="server" Text="Hedef" PostBackUrl="~/Hedef2.aspx" /></div>
</form>
</body>
</html>
```

Dikkat ederseniz doğrulama kontrollerinin script tarafında JavaScript kodu üretmesini EnableClientScript özelliklerine false değerini atayarak önledik. Bu elbette teorimizin ispatlanması için konulmuş küçük bir değişiklik. Senaryomuz gereği Hedef2.aspx isimli bir sayfaya doğru hareket ediyoruz. Eğer textBox kontrolünün içeriğini boş bırakırsak veya geçersiz bir mail adresi yazarsak normal şartlarda sayfanın Cross-Page Postback işlemini gerçekleştirmemesini bekleyebiliriz. Ne yazıkki böyle olmayacaktır. Sonuç aşağıdaki ekran görüntüsünde olduğu gibidir.

Video'dan göreceğiniz gibi hiç bir doğrulama işlemi çalışmadan hedef sayfaya geçilebilmektedir. Bu durumu önlemek için kaynak sayfadan hedef sayfaya hareket etmeden önce Page sınıfının IsValid özelliği kullanılarak doğrulama işlemlerini yaptırmaya çalışabiliriz. Ancak Cross-Page Postback sürecinde meydana gelen yaşam döngüleri nedeni ile yine kontroller atlanacak ve hedef sayfaya gidilecektir. Çözüm olarak hedef sayfa içerisinde, kaynak sayfanın geçerli olup olmadığı kontrol edilebilir ve gerekirse kullanıcı kaynak sayfaya geri gönderilebilir. Bunun için Hedef2.aspx.cs içerisindeki Load metodunu aşağıdaki gibi kodlamak yeterli olacaktır.

```csharp
protected void Page_Load(object sender, EventArgs e)
{
    if (PreviousPage != null)
    {
        if (!PreviousPage.IsValid)
        {
            Response.Write("Önceki Sayfada doğrulanmamış bilgiler var...");
        }
    }
}
protected void btnBack_Click(object sender, EventArgs e)
{
    Response.Redirect("~/Kaynak3.aspx");
}
```

Şimdi tekrardan uygulamamızı çalıştıralım.

Eminimki dikkatli gözlerden kaçmayacaktır. Kaynak sayfaya geri döndüğümüzde TextBox kontrolünün içeriği boşalmıştır. Dahası çalışma esnasında hiç göremediğimiz doğrulama kontrolüne ait hata mesajı hiç çıkmamıştır. Oysaki normal bir PostBack işleminde (yani sayfanın kendi üzerine gönderilmesinde) validation kontrolleri çalışacak, ilgili hata mesajları görünecek ve TextBox gibi diğer bazı kontrollerinde içeriği kaybolmayacaktır. Oysaki Cross-Page Postback işlemi sırasında bu avantaj kaybedilmektedir.

Çözüm olarak kaynak sayfadaki verilerin hedef sayfaya bir şekilde taşınması ve dönüş işlemi sırasında da ilgili kontrollere aktarılması düşünülebilir. Bu amaçla özelliklerden faydalanabiliriz. Daha etkili bir çözüm olarak kaynak sayfa hedef sayfaya gitmeden doğrulama kontrolleri yapılması yoluna gibilebilir. Ancak yukarıdaki madde 2' de gördüğümüz gibi sayfaların yaşam döngüsünden dolayı kaynak sayfadaki bu kodlar göz ardı edilecektir. Diğer taraftan her web sayfası bu örnekte olduğu gibi bir tek TextBox kontrolünde ibaret değildir.

Çok daha fazla kontrolün ve doğrulama işleminin olduğu bir sayfada sayfalar arası kontrol içeriklerini taşıyarak elde edilmeye çalışılacak bir çözüm can sıkıcı olabilir. Yinede, kaynak sayfaya tekrar geri dönüldüğünde eğer kontrollerin içeriklerini yeniden doldurabilirsek sayfaya yeniden bir doğrulama işlemi yaptırabilir ve gerekli validator kontrollerinin hata mesajlarının çıkmasını sağlayabiliriz. Bu durumu daha net anlayabilmek için kaynak3.aspx.cs ve hedef2.aspx.cs kodlarını aşağıdaki gibi değiştirelim. (Hedef2 içerisinde Kaynak3 referansını elde edebilmek için PreviousPageType direktifi kullanılmıştır.)

Hedef2.aspx.cs;

```csharp
protected void Page_Load(object sender, EventArgs e)
{
    if (PreviousPage != null)
    {
        if (!PreviousPage.IsValid)
        {
            Kaynak3 kyn3 = PreviousPage as Kaynak3;
            Response.Redirect("~/Kaynak3.aspx?HatalarVar=1&email="+kyn3.Email);
        }
    }
}
```

Kaynak3.aspx.cs;

```csharp
public string Email
{
    get { return txtEmail.Text; }
}

protected void Page_Load(object sender, EventArgs e)
{
    if (Request.QueryString["HatalarVar"] != null)
    {
        txtEmail.Text = Request.QueryString["email"].ToString();
        Page.Validate();
    }
}
```

Kod şu şekilde çalışmaktadır. Kullanıcı kaynak sayfada hatalı bir veri girişi yaptığında hedef sayfaya yinede gidilir ve PreviousPage tipi üzerinden hedef sayfa içerisinde doğrulama işlemi yapılır. Eğer doğrulama (Validation) başarısız ise, Kaynak3 içerisinde tanımlı EMail isimli özellik yardımıyla kaynak sayfadakiTextBox içeriği alınır ve QueryString'e eklenerek geri gönderilir. Kaynak sayfanın Load metodunda ilgili QueryString parametreleri yakalanır ve içerik alınarak TextBox kontrolüne atanır. Son olarak kaynak sayfa içerisinde Validate metodu çalıştırılarak doğrulama işlemlerinin yeniden yapılması sağlanır ve bu sayede Validator kontrolüne ait uygun hata mesajıda ekranda görünür. Sonucu görmek için örneğimizi tekrar çalıştıralım.

Yukarıdaki videodan gördüğünüz gibi, client side validation script'ler kapalıda olsa, Cross-Page postback olduktan sonra kaynak sayfaya dönüldüğünde hem kontrolün eski içeriği hemde validator bileşenin hata mesajı gösterilebilmektedir. Bu her ne kadar iyi bir çözüm gibi görünsede, sayfadaki kontrol sayısının ve içerik uzunluklarının çok daha fazla olduğu hallerde bizi QueryString yerine başka nesnelerden yararlanmaya zorlayacaktır. Bu anlamda örneğin Session nesnesi ele alınabilir. Böylece geldik bir makalemizin daha sonuna. Bu makalemizde Cross-Page Postback kullanırken dikkat etmemiz gereken noktaları incelemeye çalıştık. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama İçin Tıklayın.](/assets/files/2006/CPPB.rar)