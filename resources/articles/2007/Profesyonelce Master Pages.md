---
title: "Profesyonelce Master Pages"
pubDate: 2007-03-13 08:00:00
categories:
  - Asp.Net 2.0
tags:
  - Asp.Net 2.0
  - C#
  - .NET
  - ASP.NET
  - ADO.NET
  - HTTP
  - Visual Studio
---

# Profesyonelce Master Pages
Değerli Okurlarım Merhabalar,

Asp.Net 2.0 ile birlikte gelen en önemli yeniliklerden biriside Master Page kavramıdır. Master Page kavramını ilk öğrendiğimde aklımda oluşan tanımlama şöyleydi; "bir sitedeki sayfalarının tamamının yada bir kısmının aynı şablon üzerinde oturmasını sağlamak istiyorsak Master Page'lerden faydalanabiliriz". Oysaki Master Page kullanımı ile elde edilen avantajlar sadece görsel açıdan gelen bu kolaylık ile sınırlı değildir. Sonuç itibariye her Master Page aynı zamanda arka planda bir örnek olarak oluşturulan bir sınıf tanımlamasıdır. Bu nedenle Master Page uyarlanan içerik sayfalarının (Content Page) ortaklaşa kullanabileceği fonksiyonellikleri dahi barındırabilir. Buda tam anlamıyla kalıtım (inheritance) ile yapabildiklerimizin bir yansımasıdır. İşte bu makalemizde Master Page kavramının derinlerine gidip diğer avantajlarını ve özellikle dikkat etmemiz gereken noktaların neler olabileceğini incelemeye çalışacağız. Temel olarak ele alacağımız konular aşağıda maddeler halinde sıralanmıştır.

- Master Page uygulanmamış bir web sayfasına sonradan Master Page uygulamaya çalışmak.
- İçerik sayfalarından (Content Page), Master Page üyelerine erişmek. (Özellikler (Properties) yadımıyla erişmek, kontrolleri bulmak (FindControl), MasterType direktifinden yararlanmak)
- Ortak fonksiyonellikleri Master Page altında toplamak.
- İçerik sayfalarından (Content Page) Master Page'leri dinamik olarak değiştirmek.
- İç (Nested) Master Page'ler geliştirmek ve kullanmak.

Bahsetmiş olduğumuz maddeleri örnek bir senaryo üzerinden incelemeye çalışacağız. Bu nedenle web uygulamamızda aşağıdaki gibi bir Master Page tasarlamış olduğumuzu düşünelim.

![mk195_1.gif](images/mk195_1.gif)

```text
<%@ Master Language="C#" AutoEventWireup="true" CodeFile="AzonCityMaster.master.cs" Inherits="AzonCityMaster" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table border="1" cellpadding="3" cellspacing="1" width="640px">
                <tr>
                    <td colspan="2"><img src="images/csharpnedir.gif" /></td>
                </tr>
                <tr>
                    <td valign="top" style="width: 150px"><asp:TreeView ID="TreeView1" runat="server" DataSourceID="SiteMapDataSource1"></asp:TreeView></td>
                    <td style="text-align: right"><asp:ContentPlaceHolder ID="ContentPlaceHolder1" runat="server"></asp:ContentPlaceHolder></td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align: right"><span style="font-size: 10pt; font-family: Verdana; text-decoration: underline">cityadmin@azoncity.com</span>
                    <img src="images/google_coop.gif" /></td>
                </tr>
            </table>
        </div>
        <asp:SiteMapDataSource ID="SiteMapDataSource1" runat="server" />
    </form>
</body>
</html>
```

Geliştirdiğimiz Master Page içerisinde sitedeki diğer sayfalara kolay geçiş yapmamızı sağlayacak şekilde bir TreeView kontrolü ve buna bağlı olacak şekilde tasarlanmış bir SiteMapDataSource bileşeni de vardır. Diğer taraftan sadece göze hoş gelmesi açısından bir kaç resim bileşeni dahil edilmiş ve tek bir ContentPlaceHolder kullanılmıştır.

Master Page'lerin bazı önemli özellikleri vardır. Makalemizin konusu olan maddelere geçmeden önce bunları hatırlamakta fayda olacağı kanısındayım. Herşeyden önce MasterPage'ler aslında UserControl sınıfından türemiştir. Bunu görebilmek için Visual Studio 2005 ortamında, MasterPage kelimesi üzerindeyken sağ tıklayıp Go To Definition diyebiliriz. Bu durumda, MasterPage sınıfının metadatasını görebiliriz. Dikkat ederseniz açık bir şekilde UserControl tipinden türediği ortadadır.

![mk195_2.gif](images/mk195_2.gif)

Bir diğer önemli nokta ise, Master Page'lerin uygulandığı içerik sayfalarının (Content Page) herhangibir şekilde html, head, body, vb... takılar ile birlikte kesinlikle form takısını içermediğidir. İşte bu ayırt edici özellik makalemizin ilk maddesi için önemlidir.

1. Master Page uygulanmamış bir web sayfasına sonradan Master Page uygulamaya çalışmak.

Çoğu zaman projelerimizde sonradan Master Page kullanmaya karar verdiğimiz durumlar olabilir. (Yada buna neden olacak başka vakkalar olabilir) Var olan bir Master Page'den, içerik sayfaları (Content Page) oluşturmamız kolaydır. Hatta Visual Studio 2005 buna tam destek vermektedir. Ancak var olan bir web sayfasına, herhangibir Master Page'i sonradan uygulamak istediğimizde yapmamız gerekenler temel olarak şu şekilde özetlenebilir.

- İçerik sayfasının (Content Page) içerisinde Html, Body, Head, Title vb... ile form elementlerinin olmaması gerekir.
- Sayfanın Page direktifi içerisinde Master Page tanımlaması MasterPageFile isimli nitelik yardımıyla yapılmalıdır.
- Master Page içerisinde yer alan ContentPlaceHolder'ların, içerik sayfasında birer Content bileşeni olarak ele alınması gerekmektedir.

Konuyu daha iyi anlayabilmek için bir örnek üzerinden gidelim ve uygulamamızda kullandığımız sayfalardan herhangibirini ele alalım. Örnek olarak aşağıdaki ekran görüntüsüne sahip olan bloglar.aspx sayfasını kullanabiliriz.

![mk195_3.gif](images/mk195_3.gif)

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Bloglar.aspx.cs" Inherits="Bloglar" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <span style="font-size: 10pt; font-family: Verdana"><strong>Aradığınız Blog sahibinin adını giriniz </strong><br /></span>
            <asp:TextBox ID="TextBox1" runat="server" Width="208px"></asp:TextBox><span style="font-size: 10pt;font-family: Verdana"> </span>
            <asp:Button ID="Button1" runat="server" Text="Ara" Width="65px" /><span style="font-size: 10pt;font-family: Verdana"><br /></span>
            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="http://www.csharpnedir.com">Yardım</asp:HyperLink><br />
            <span style="font-size: 10pt; font-family: Verdana"><strong>Sonuçlar</strong><br /></span>
            <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="True" Width="214px"></asp:DropDownList>
        </div>
    </form>
</body>
</html>
```

Şimdi gelin adım adım bu sayfaya AzonCityMaster isimli Master Page'imizi uygulayalım. İlk olarak Page direktifi içerisinde MasterPageFile niteliği yardımıyla uygulamak istediğimiz Master Page'in fiziki dosyasını belirlemeliyiz. Eğer Visual Studio 2005 kullanıyorsanız intelli-sense özelliği bize yardımcı olacaktır. Aksi takdirde ~ harfinide göz önüne alaraktan aşağıdaki kod parçasında görüldüğü gibi bir yol tanımlaması yapılmalıdır.

![mk195_4.gif](images/mk195_4.gif)

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Bloglar.aspx.cs" Inherits="Bloglar" MasterPageFile="~/AzonCityMaster.master" %>
```

Bir sonraki adımımız form elementleri içerisinde yer alan bileşenlerimizi bir Content elementi içerisine almak olacaktır. Buradan şu sonucada varabiliriz. İçerik sayfaları bileşen olarak sadece Content elementlerini içermektedir. İçerik sayfaları hiç bir şekilde form elementi içermeyeceklerinden (Html, Body vb kısımlarda buna dahildir) Bloglar.aspx isimli sayfamıza, Master Page'deki hangi ContentPlaceHolder bileşenini kullanacaksak ona uygun bir Content bileşenini aşağıdaki gibi eklememiz gerekir.

![mk195_5.gif](images/mk195_5.gif)

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Bloglar.aspx.cs" Inherits="Bloglar" MasterPageFile="~/AzonCityMaster.master" %>
<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" ID="Content1" runat="server">

</asp:Content>
```

Son olarak tek yapmamız gereken Content elementi içerisine, bloglar.aspx sayfasının bir önceki halinde yer alan Asp.Net bileşenlerini dahil etmek olacaktır. Burada dikkat edilmesi gereken nokta, Asp.Net bileşenlerine ait elementlerin, mutlaka ve mutlaka Content elementinin takıları (tags) içerisinde olması gerektiğidir. Biz örneklerimizde tek bir ContentPlaceHolder kullandığımızdan, içerik sayfasındaki Content bileşenide tektir. Dolayısıyla, Master Page içerisindeki ContentPlaceHolder'ların sayısına göre içerik sayfalarında uygulanması gereken Content bileşenlerinin sayısı artabilir.

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Bloglar.aspx.cs" Inherits="Bloglar" MasterPageFile="~/AzonCityMaster.master" %>
<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" ID="Content1" runat="server">
     <span style="font-size: 10pt; font-family: Verdana"><strong>Aradığınız Blog sahibinin adını giriniz </strong><br /></span>
     <asp:TextBox ID="TextBox1" runat="server" Width="208px"></asp:TextBox><span style="font-size: 10pt;font-family: Verdana"> </span>
     <asp:Button ID="Button1" runat="server" Text="Ara" Width="65px" /><span style="font-size: 10pt;font-family: Verdana"><br /></span>
     <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="http://www.csharpnedir.com">Yardım</asp:HyperLink><br />
     <span style="font-size: 10pt; font-family: Verdana"><strong>Sonuçlar</strong><br /></span>
     <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="True" Width="214px"></asp:DropDownList>
</asp:Content>
```

Bu işlemlerin ardından Bloglar.aspx isimli sayfamız, Master Page'i uygulayan bir içerik sayfası haline gelecektir.

![mk195_6.gif](images/mk195_6.gif)

2. İçerik sayfalarından (Content Page), Master Page'in üyelerine erişmek.

Master Page'ler, kendisinden üretilen içerik sayfaları için ortaklaşa kullanılabilecek üyeler (örneğin metodlar) içerebilirler. Özellikle tüm sayfalarda söz konusu olabilecek veritabanı işlemlerine ait hazırlıkların tek bir merkezden yapılabilmesini sağlamak bu ortak fonksiyonellikler için bir örnek olarak düşünülebilir. Diğer taraftan içerik sayfalarındaki süreçlerin işleyişine göre Master Page üzerindeki kontrollerin davranışlarını değiştirmek isteyebiliriz. Tüm bunlar aslında kalıtımın (inheritance) bir etkisi olarak karşımıza çıkmaktadır. Öyleki Asp.Net 1.1 ile geliştirme yapanların, Master Page tarzı mimariler için geliştirdikleri çözümler kalıtım (inheritance) ilkelerine dayanılarak gerçekleştirilmiştir. Ne varki Asp.Net 2.0 ile birlikte gelen Master Page kavramı, sadece görsel açıdan değil kod tarafındanda kalıtımı etkin bir şekilde kullanabilme imkanı sağlamaktadır. Dolayısıyla bazı durumlarda içerik sayfalarından Master Page'lerin ele alınması gerekebilir. Bu durumları bir kaç basit örnek ile incelemekte fayda vardır. İlk olarak, içerik sayfalarında iken, Master Page'in başlık bilgisini (Title) ve hatta MasterPage ile birlikte üretilen metadata bilgilerini değiştirmek istediğimizi düşünelim. Bu amaçla yine bloglar.aspx sayfasını göz önüne alabiliriz. Sayfamızın Load olayı tetiklendiğinde aşağıdaki işlemleri gerçekleştirdiğimizi düşünelim.

```csharp
protected void Page_Load(object sender, EventArgs e)
{
    this.Master.Page.Title = "Azon Şehri Sakinlerinin Blogları";
    HtmlMeta metadatas=new HtmlMeta();
    metadatas.Name="Keywords";
    metadatas.Content="Blog, Azon, City, Azon City, Yemek, Gurme, Kermes";
    this.Master.Page.Header.Controls.Add(metadatas);
    this.Master.Page.SmartNavigation = true;
}
```

İçerik sayfasından eğer Master Page referansına geçiş yapmak istiyorsak, Master özelliğinden faydalanabiliriz. Örnek kodumuzda, Master Page yardımıyla üretilen sayfanın title, metadata, smart navigation gibi özelliklerini değiştiriyoruz. Buna göre Title özelliğini değiştirmek için this.Master.Page.Title söz diziminden yararlanılmıştır. Metadata bilgisini eklemek içinse öncelikli olarak HtmlMeta tipinden bir nesne örneklenmiştir.

Bu nesnenin iki önemli özelliği vardır. Name ve Content. Bu iki özelliğe atadığımız değerlere göre, sayfamızın arama sitelerinde metadata içerisinde belirttiğimiz konu başlıkları altında çıkması muhtemeldir. Üretilen metadata elementinin Master Page'e ait Html çıktısına yazılabilmesi içinde Master.Page.Header.Controls söz diziminden yararlanılmıştır. Bir başka deyişle Head elementi içerisine girilerek Controls koleksiyonuna, oluşturulan Metadata bilgileri eklenmiştir. Son olarak üretilen sayfanın SmartNavigation özelliği ture yapılmıştır. Bloglar.aspx sayfamızı herhangibir tarayıcı penceresinde açtığımızda aşağıdaki ekran görüntüsünü elde ederiz.

![mk195_7.gif](images/mk195_7.gif)

Sayfamızın HTML çıktısına baktığımızda ise yapmış olduğumuz değişikliklerin aşağıdaki gibi yansıdığını görebiliriz. Elbetteki bunu örnek olması açısından sadece tek bir sayfada ele aldık.

```text
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title> Azon Şehri Sakinlerinin Blogları</title>
    <meta name="Keywords" content="Blog, Azon, City, Azon City, Yemek, Gurme, Kermes" />
<style type="text/css">.ctl00_TreeView1_0 { text-decoration:none; }</style>
</head>
<body>
<IFRAME id="__hifSmartNav" name="__hifSmartNav" style="display:none" src="/DerinlemesineMasterPages/WebResource.axd?d=jcUk5uOrm4IZldJ56Y8MhA2&t=632964516087343750"></IFRAME>
<form name="aspnetForm" method="post" action="Bloglar.aspx" id="aspnetForm" __smartNavEnabled="true">
<div>
<input type="hidden" name="ctl00_TreeView1_ExpandState" id="ctl00_TreeView1_ExpandState" value="ennn" />
.
.
.
```

Bir diğer senaryo ise, içerik sayfalarından Master Page içerisindeki her hangibir kontrolün davranışını değiştirmektir. Örneğin Master Page üzerinde yer alan bir Label kontrolü içerisinde, girilen içerik sayfasına ait bir takım özel bilgilerin yazması istenebilir. Kullanıcının adı bu kontrolde gösterilebilir yada yetkisine göre renklendirmeler yapılabilir. Senaryolar elbette çoğaltılabilir. Odaklanılması gereken nokta bu kontrole içerik sayfaları üzerinden nasıl erişilebileceğidir. Bu amaçla bir kaç yöntem ele alınabilir. Örneğin FindControl metodu yardımıyla Label kontrolü bulunabilir yada Master Page içerisine yazılacak bir özellikten (Property) faydalanılabilinir. Hatta kullanım kolaylığı sağlaması bakımından MasterType direktifinden de yararlanılabilir.

Şimdi bunları teker teker ele alalım. Görsel tabanlı uygulamalarda özellikle taşıyıcı (Container) rolü üstlenen bileşenlerin çoğunun FindControl metodu vardır. Bu metod sayesinde, ID veya Name gibi özelliklerine göre ilgili taşıyıcı içerisinden herhangibir kontrol bulunabilir. Bunun sonrasında tek yapılması gereken kontrolün ilgili özelliklerinin değiştirilmesidir. Konuyu daha iyi anlayabilmek için Master Page içerisine bir Label kontrolü atılmış ve lblBilgi olarak isimlendirilmiştir. Buna göre Bloglar.aspx sayfasından bu Label kontrolüne erişip içeriğini değiştirmek için aşağıdaki gibi bir yol izlenebilir. (Kod parçası bloglar.aspx sayfasının PageLoad olay metodu içerisinde ele alınmıştır.)

```csharp
Label lblMaster = (Label)this.Master.FindControl("lblBilgi");
lblMaster.Text = "Bloglar sayfasından geldim";
lblMaster.ForeColor = System.Drawing.Color.Red;
lblMaster.Font.Bold = true;
lblMaster.Font.Name = "Verdana";
lblMaster.Font.Size = 10;
```

Dikkat ederseniz Master Page içerisindeki lblBilgi kontrolünü bulmak için FindControl metodundan faydalanıyoruz. FindControl metodu parametre olarak aranan bileşenin ID özelliğinin değerini almaktadır. Elbette FindControl metodunun geri dönüş değeri Control tipindendir. Bu sebeptende geri dönen kontrol referansının değerinin Label olarak ele alınabilmesi için bilinçli bir şekilde dönüştürme (explicitly cast) işlemi uygulanmıştır. Böylece lblBilgi isimli Label kontrolüne ait çalışma zamanı referansını bloglar.aspx sayfası içerisinde ele alabilir ve özelliklerini değiştirebiliriz. Bunun ardından aşağıdaki ekran görüntüsündeki sonucu elde ederiz.

![mk195_8.gif](images/mk195_8.gif)

Master Page içerisindeki üyelere içerik sayfalarından erişebilmek için özelliklerdende (properties) faydalanabiliriz. Bu durumda, FindControl metodunda olduğu gibi cast işlemleri yapmamızda gerek kalmaz. Örneğin amacımız sadece Master Page'deki lblBilgi bileşeninin metin içeriğini değiştirmekse aşağıdaki gibi bir özellik söz konusu olabilir. Elbetteki ihtiyaca göre yazılan özelliğin yanlız okunabilir (readonly) olması farklı bir tipte değer döndürmesi gerekebilir. Bunlar tamamen projedeki ihtiyaçlar doğrultusunda belirlenebilecek noktalardır.

```csharp
public partial class AzonCityMaster : System.Web.UI.MasterPage
{
    public string Bilgi
    {
        get { return lblBilgi.Text; }
        set {
            if (!String.IsNullOrEmpty(value))
                lblBilgi.Text = value;
            else
                lblBilgi.Text = "Bilinmeyen Bilgi";
        }
    }
}
```

Artık tek yapmamız gereken, içerik sayfasında, Master Page'in referansını yakalamak ve bunun üzerinden Bilgi isimli özelliğe erişip kullanmaktır. Bu amaçla yine bloglar.aspx sayfamızda aşağıdaki kod parçasını kullanabiliriz.

```csharp
AzonCityMaster azonMstr = (AzonCityMaster)this.Master;
azonMstr.Bilgi = "Bloglar sayfasından geldim";
```

Dikkat ederseniz içerik sayfasının uyguladığı Master Page referansını elde etmek için bilinçli bir şekilde (explicitly) dönüşüm işlemi yapılmıştır. Eğer bu dönüştürme işlemini yapmassak, Bilgi isimli özelliğe erişemeyiz. Sonuç itibariyle yapmış olduğumuz tür dönüşümü sayesinde, Master Page içerisinde tanımlanmış olan Bilgi isimli özelliğe (property) erişilebilmiş ve değeri aşağıdaki ekran görüntüsünde olduğu gibi değiştirilebilmiştir.

![mk195_9.gif](images/mk195_9.gif)

Master Page'e erişmenin bir başka yoluda, MasterType direktifini kullanmaktır. Bu direktif sayesinde, Master özelliği üzerinden hiç bir tip dönüşüm işlemi yapmaya gerek kalmadan Master Page referansına erişilebilir. Bunun için öncelikli olarak, içerik sayfasında aşağıdaki direktifi tanımlamamız gerekmektedir.

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Bloglar.aspx.cs" Inherits="Bloglar" MasterPageFile="~/AzonCityMaster.master" %>
<%@ MasterType VirtualPath="~/AzonCityMaster.master" %>
<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" ID="Content1" runat="server">
.
.
.
```

Bu bilgilendirmenin ardından artık this.Master özelliği ile doğrudan AzonCityMaster referansına erişebiliriz.

![mk195_10.gif](images/mk195_10.gif)

Dikkat ederseniz, MasterType direktifi içerisinde VirtualPath tanımlaması ile hangi MasterPage'e ait referansın kullanılacağı belirtilmektedir. Böylece içerik sayfasının Master özelliğinin taşıyacağı referansta belirlenmiş olur.

3. Ortak fonksiyonellikleri Master Page altında toplamak.

Master Page'ler, çalışma zamanında birleştirilecekleri içerik sayfalar adına ortak fonksiyonellikleri tutmak içinde çok ideal bir ortam hazırlar. Söz gelimi, pek çok web uygulamasında sıklıkla başvurduğumuz veri işlemleri bu fonksiyonelliklere örnek olarak verilebilir. Bir başka deyişle her sayfa için sorgu hazırlama gibi işlemlerin tamamını Master Page içerisinde yer alan bir fonksiyonelliğe yıkabiliriz. Böylece hem kod optimizasyonunu hemde bakım kolaylığını sağlamış oluruz. Yine örnek senaryomuzda yer alan bloglar.aspx sayfasını göz önüne alarak devam edelim. Bu sefer veri girişi için çok basit olarak bir kaç kontrol bloglar.aspx sayfasına ilave edilmiştir.

![mk195_11.gif](images/mk195_11.gif)

Kullanıcılar yeni bir blog girişi yapmak isteyebilir. Bu girişe ait kontrollerin iş mantığı, veri girişi için gerekli sorgunun hazırlanması ve ilgili Ado.Net işlevselliklerinin çalıştırılması gibi işlemler, Master Page içerisinde aşağıdaki gibi toplanabilir. Burada işlerin biraz daha kolaylaşması açısından tablo adına göre sorgu oluşturulabilmesi için Tablolar isimli bir enum sabiti düşünülmüştür.

```csharp
public bool Insert(Tablolar tablo,params object[] parametreler)
{
    bool eklendi = false;
    switch (tablo)
    {
        case Tablolar.CitizenBlogs:
            // Burada CitizenBlogs tablosuna veri ekleme işlemi yapılır.
            return true;
            break;
        case Tablolar.Citizen:
            break;
        case Tablolar.Favorites:
            break;
    }
    return eklendi;
}
```

Bizim için önemli olan nokta fonksiyonelliğimize içerik sayfasından (Content Page) erişebilmektir. Hatırlayacağınız gibi bir önceki maddemizde MasterType direktifini kullanmıştık. Bu nedenle Insert isimli metoda, bloglar.aspx isimli sayfamızdan aşağıdaki kod parçasında olduğu gibi erişebiliriz.

```csharp
protected void btnEkle_Click(object sender, EventArgs e)
{
    this.Master.Insert(Tablolar.CitizenBlogs, txtBlogAdi.Text, txtBlogAdresi.Text);
}
```

4. İçerik sayfalarından (Content Page) Master Page'leri dinamik olarak değiştirmek.

Bazı durumlarda senaryo gereği, içerik sayfalarının uygulayacağı Master Page'i çalışma zamanında değiştirmemiz gerekebilir. Örneğin çoğu sayfada yer alan print görünümünü alma işlemi göz önüne alınabilir. Bu tip bir durumda, Print sayfasının içeriğinin farklı olması nedeni ile, ayrı bir Master Page'in çalışma zamanında söz konusu içerik sayfası için uygulanması gerekir. Bir başka senaryo özellikle farklı kültürlere hizmet verecek içerik sayfalarında söz konusu olabilir. Kullanıcının seçtiği ülkeye göre içerik sayfasının farklı bir Master Page uygulaması istenebilir. Hatta, içerik sayfasındaki kullanıcının yetkisine görede farklı Master Page'lerin uygulatılması gerekebilir. Tüm bu durumlar temel olarak çalışma zamanında Master Page'in değiştirilebilmesi halinde gerçekleşebilecek örnek senaryolardır. Bir içerik sayfasının çalışma zamanında uyguladığı Master Page'i değiştirmek için tek yapılması gereken, sayfanın MasterPageFile özelliğine uygun bir değer atamaktır.

> MasterPageFile özelliği sadece içerik sayfasının PreInit olay metodu içerisinde değiştirilebilir. Başka bir yerden değiştirme yapmak istediğimizde (örneğin bir düğmenin Click olay metodu içerisinden), çalışma zamanında aşağıdaki hata mesajını alırız.
> ![mk195_13.gif](images/mk195_13.gif)

Örneğimizde aşağıdaki ekran görüntüsüne sahip AzonCitySummerMaster.master isimli ikinci bir Master Page olduğunu düşünelim.

![mk195_12.gif](images/mk195_12.gif)

Amacımız bloglar.aspx sayfasında yer alan düğmeler yardımıyla Master Page'ler arasında geçiş yapmaktır. Bu düğmelerde tek yapacağımız bloglar.aspx sayfasına doğru yönlendirme yapmak ve QueryString yardımıylada hangi Master Page'e geçileceğine dair bir anahtar-değer (key-value) çifti göndermektir.(Örneği geliştirirken, bloglar.aspx sayfasına eklediğimiz MasterType direktifini kaldırmak gerekebilir. Nitekim, iki farklı tipten Master Page olacağı için, kod içerisinde this.Master söz diziminin olduğu satırlarda hatalar oluşacaktır.)

```csharp
protected void btnYazGeldi_Click(object sender, EventArgs e)
{
    Response.Redirect("~/bloglar.aspx?Yaz=Evet");
}
protected void btnKis_Click(object sender, EventArgs e)
{
    Response.Redirect("~/bloglar.aspx?Yaz=Hayir");
}
```

Az öncede bahsettiğimiz gibi, MasterPageFile özelliğini sadece PreInit olay metodunda değiştirebiliriz. Dolayısıyla tek yapmamız gereken bu olay metodu içerisine aşağıdaki kod parçarlarını eklemek olacaktır.

```csharp
protected void Page_PreInit(object sender, EventArgs e)
{
    if(Request.QueryString["Yaz"]!=null)
    {
        string yazmi = Request.QueryString["Yaz"].ToString();
        if(yazmi=="Evet")
            this.MasterPageFile = "~/AzonCitySummerMaster.master";
        else if(yazmi=="Hayir")
            this.MasterPageFile = "~/AzonCityMaster.master";
    }
}
```

Uygulamamızı çalıştırdığımızda aşağıdaki Flash animasyonunda yer alan sonuçları elde ederiz. (Flash animasyonunu izleyebilmek için Flash Player yüklemeniz gerekebilir)

5. İç (Nested) Master Page'ler geliştirmek ve kullanmak.

Bazı durumlarda, iç master page'ler geliştirmemiz gerekebilir. Örneğin bir şirketin ana Master Page'inin haricinde departmanlara göre farklı şekillerde uygulanabilecek iç Master Page'lerde söz konusu olabilir. Burada önemli olan, iç Master Page'lerin ana Master Page içerisinde yer almasıdır. Teorik olarak her iç Master Page, üstündeki Master Page'de yer alan ContentPlaceHolder'ları uygulayacak şekilde tasarlanır. Dolayısıyla iç Master Page'lerde, içerik sayfaları gibi html, head veya form elementlerini içermezler.

> İç Master Page'lerde, içerik sayfalarında olduğu gibi html, body, form vb elementleri içermezler. Sadece Content elementlerini içerirler.

Bunun yanında Master direktifi içerisinde mutlaka MasterPageFile niteliği ile üst Master Page'in ne olacağı bildirilmelidir. Aşağıdaki şekilde iç Master Page'ler ve içerik sayfalarının genel yapısı şematize edilmeye çalışılmıştır.

![mk195_15.gif](images/mk195_15.gif)

Gelin kendi örneğimizde bir iç Master Page kullanmaya çalışalım. Söz gelimi, AzonCityMaster isimli Master Page içerisinde AzonCountyMasterPage isimli bir iç Master Page uygulayabiliriz. Tek yapmamız gereken projemize AzonCountyMasterPage isimli yeni bir Master Page eklemek ve içeriğini aşağıdaki gibi düzenlemek olacaktır.

```text
<%@ Master Language="C#" AutoEventWireup="true" CodeFile="AzonCountyMasterPage.master.cs" Inherits="AzonCountyMasterPage" MasterPageFile="~/AzonCityMaster.master" %>
<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" ID="SubContent1" runat="server">
    <table>
        <tr>
            <td><b><h2>İlçe Sayfası</h2></b></td>
        </tr>
        <tr>
            <td><asp:ContentPlaceHolder ID="CountyPlaceHolder" runat="server"></asp:ContentPlaceHolder></td>
        </tr>
    </table>
</asp:Content>
```

Dikkat ederseniz Master direktifi içerisinde MasterPageFile niteliğine, AzonCityMaster.master sayfası atanmıştır. Bununla birlikte iç Master Page sanki bir içerik sayfasıymış gibi Content isimli bir element içermektedir. Bu elementin asıl yaptığı, söz konusu iç Master Page'in, üst Master Page içerisindeki hangi ContentPlaceHolder içerisine yerleştirileceğinin belirtilmesidir. İç master page'i uygulamak isteyen içerik sayfalarının bağımsız olarak değiştirebileceği bir bölge olması açısından, örnek ContentPlaceHolder elementi de dahil edilmiştir. Bu durumda, AzonCountyMasterPage sayfasını uygulayan bir içerik sayfası (Content Page) sadece, CountyPlaceHolder isimli ContentPlaceHolder içeriğini değiştirebilecektir.

> Ne yazık ki iç Master Page'lerin uygulanması halinde Visual Studio 2005 üzerinde hem iç Master Page'lerde, hemde bunları uygulayan içerik sayfalarında (Content Page), tasarım modu (Design Mode) kaybedilmektedir.

Bir başka deyişle, AzonCountMaster yada bunu uygulayan bir içerik sayfasında Design tarafına geçmek istersek aşağıdaki hata mesajını alırız.

![mk195_14.gif](images/mk195_14.gif)

Gelelim içerik sayfasına. İçerik sayfamız normal olarak AzonCountyMasterPage isimli iç Master Page'i uygulayacaktır. Buna göre örnek olması açısından aşağıdaki gibi bir tasarım düşünülebilir.

```text
<%@ Page Language="C#" MasterPageFile="~/AzonCountyMasterPage.master" AutoEventWireup="true" CodeFile="IlceSayfasi.aspx.cs" Inherits="IlceSayfasi" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CountyPlaceHolder" Runat="Server">
    <asp:TextBox ID="txtIlceAdi" runat="server"></asp:TextBox>
    <asp:Button ID="btnBul" runat="server" Text="Bul" /> 
</asp:Content>
```

Sayfamızı tasarım modunda (Design Mode) düzenleyemeyeceğimizi daha önceden belirtmiştik. Bu nedenle geliştirme safhasındayken tam olarak tasarıma hakim olamayız. Bu oldukça büyük bir dezavantajdır. Ne varki çalışma zamanında bir sorun olmadığını görebiliriz.

![mk195_16.gif](images/mk195_16.gif)

Böylece geldik bir makalemizin daha sonuna. Bu makalemizde, Master Page kavramını daha detaylı bir şekilde öğrenmeye çalıştık. Master Page'lerin sadece görsel bir yenilik olmadığını, kod tarafındada işimizi kolaylaştıracak, kod optimizasyonu ve bakımını etkileyecek özelliklere sahip olduğunu gördük. Master Page'lerin asıl yararını görmek açısından, Master Page olmadan bilinen nesne yönelimli programlama kurallarından faydalanarak aynı işlemler yapılmaya çalışılabilir. Bu durumda Master Page'lerin işlemlerimizi gerçektenden kolaylaştırdığını daha iyi anlayabiliriz. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.
