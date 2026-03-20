---
layout: post
title: "Asp.Net 2.0 ile Cross-Page Posting"
date: 2004-08-31 12:00:00 +0300
categories:
  - aspnet-2-0
tags:
  - aspnet-2-0
  - csharp
  - dotnet
  - aspnet
  - http
  - visual-studio
---
Bu makalemizde, Asp.Net 2.0 (Asp.Net Whidbey) ile, başka sayfalara veri postalama işlemlerinin nasıl ele alındığını ve uygulandığını incelemeye çalışacağız. Bildiğiniz gibi, Asp.Net 1.0 / 1.1 ile gelen yeniliklerden en önemlisi, sayfaların kendi kendilerine form verilerini postalayabilme kabiliyetleridir. Öyleki, Asp.Net ile geliştirilen web sayfaları aslında birer sınıf nesnesi olduklarından, sayfa üzerindeki form kontrollerine ve değerlerine kolayca erişilebilmektedir. Ancak bazı zamanlarda, sayfalarımızda yer alan form verilerini başka sayfalara göndermek isteyedebiliriz. İşte Cross-Page Posting olarak adlandırılan bu işlemlerin, Asp.Net 2.0 ile gerçekleştirilmesi hem daha kolay hemde daha etkili hale getirilmiştir.

Örneklerimizi geliştirdiğimizde bu konuyu çok daha iyi anlayabileceğinize inanıyorum. Hiç vakit kaybetmeden örneğimizi geliştirmeye başlayalım. Bu örnek uygulamamızı, Visual Studio.Net'in 2005 sürümünün Beta versiyonu ile geliştireceğiz. Örneğimizde, basit olarak iki web sayfası kullanacağız. Öncelikle yerel sunucumuzda bir web sitesi oluşturalım. Bunun için, File menüsünden, New alt menüsünü ve buradan da Web Site kısmını seçiyoruz.

![mk83_1.gif](/assets/images/2004/mk83_1.gif)

Şekil 1. Yeni bir Web Site açıyoruz.

Ardından, karşımıza çıkacak dialog penceresinden, Asp.Net Web Site'ı seçelim ve localhost altında CrossPosting isimli sanal klasörümüzün adını girelim.

![mk83_2.gif](/assets/images/2004/mk83_2.gif)

Şekil 2. Web Site'ımızı oluşturuyoruz.

Bu işlemin ardından, Visual Studio.Net standart olarak default.aspx isimli sayfamızı oluşturur. Bu sayfamızı aşağıdaki şekilde olduğu gibi oluşturalım.

![mk83_3.gif](/assets/images/2004/mk83_3.gif)

Şekil 3. Form tasarmımız.

Sayfamızın kodlanmasına geçmeden önce, aspx sayfamızın içeriğine bir göz atalım. Nitekim burada önemli olan btnDiger ID değerine sahip button kontrolümüzün takıları arasındaki içeriktir. Biz burada ufak bir değişiklik yapacağız. Önce kodlarımızın başlangıç haline bakalım.

```text
<%@ Page Language="C#" CompileWith="Default.aspx.cs" ClassName="Default_aspx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Label ID="Label1" Runat="server" Text="Rumuzunuz" Width="86px" Height="19px"
Font-Bold="True"></asp:Label><span style="font-size: 10pt; font-family: Verdana"> </span>
        <asp:TextBox ID="TextBox1" Runat="server"></asp:TextBox><br />
        <span style="font-size: 10pt; font-family: Verdana"></span>
        <asp:Label ID="Label2" Runat="server" Text="Yaş Aralığınız" Font-Bold="True"></asp:Label><span
style="font-size: 10pt; font-family: Verdana"> </span>
        <asp:DropDownList ID="DropDownList1" Runat="server" Width="71px" Height="22px">
            <asp:ListItem>20-25</asp:ListItem>
            <asp:ListItem>25-30</asp:ListItem>
            <asp:ListItem>30-35</asp:ListItem>
        </asp:DropDownList><span style="font-size: 10pt; font-family: Verdana">
<br />
<br />
</span>
        <asp:Button ID="btnBurası" Runat="server" Text="Burası" /><span style="font-size: 10pt;
font-family: Verdana">          
</span>
        <asp:Button ID="btnDiger" Runat="server" Text="Diger" Width="58px" Height="24px" /><span
style="font-size: 10pt; font-family: Verdana">
<br />
<br />
<br />
</span>
        <asp:Label ID="lblKim" Runat="server" Width="270px" Height="19px" BorderColor="#FFC0C0"
BorderWidth="1px" Font-Bold="True"></asp:Label><span style="font-size: 10pt; font-family: Verdana">
</span>
    </form>
</body>
</html>
```

Burada btnDiger, ID değerine sahip button kontrolümüzün olduğu satırı aşağıdaki gibi değiştirelim.

```text
<asp:Button ID="btnDiger" PostBackUrl="Diger.aspx" Runat="server" Text="Diger" Width="58px" Height="24px" />
```

İşte Asp.Net 2.0 ile gelen ilk yeniliklerden birisi. PostBackUrl özelliği. Bu Url, button kontrolüne basıldığında, Form üzerindeki verilerin belirtilen url'deki sayfaya gönderilmesini sağlamaktadır. İşleyiş şekli son derece etkilidir. Şimdi Solution Explorer'dan Add New Item ile, Diger.aspx isimli sayfamızıda uygulamamıza ekleyelim. Ardından, default.aspx sayfamızın Code-Behind kodlarını da aşağıdaki gibi geliştirelim.

```csharp
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Default_aspx
{
    void btnBurası_Click(object sender, EventArgs e)
    {
        lblKim.Text = TextBox1.Text + DropDownList1.SelectedItem.Text;
    }
}
```

Burada yaptığımız değişik bir olay yok. Sadece, TextBox1 kontrolüne girilen ve DropDownList1 kontrolünde seçilen değerleri, Label kontrolüne yazdırdık. İşte bu, Asp.Net 1.0/1.1 sürümünden bildiğimiz, sayfanın kendi kendine verileri postalaması işlemidir. Asp.Net 2.0 ile gelen yeni teknikleri ise, Diger.aspx sayfasında kullanacağız. Bunun için Diger.aspx sayfamıza bir Label kontrolü yerleştirelim ve ardından, sayfamıza ait kodları aşağıdaki gibi düzenleyelim.

```csharp
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Diger_aspx
{
    void Page_Load(object sender, EventArgs e)
    {
        TextBox tbRumuz = (TextBox)PreviousPage.FindControl("TextBox1");
        DropDownList lstYas = (DropDownList)PreviousPage.FindControl("DropDownList1");

        lblDurum.Text = tbRumuz.Text + lstYas.SelectedItem.Text;
    }
}
```

Dikkat edecek olursanız burada son derece güçlü bir teknik kullanılmaktadır. Bir adet TextBox ve bir adette DropDownList kontrolü tanımlanmış ve oluşturulmuştur. TextBox kontrolümüzü oluştururken, PreviousPage sınıfının FindControl metodu ile, bu sayfaya Post işlemi ile form verisi gönderen sayfadaki TextBox1 kontrolü bulunmaktadır. Bu işlem, bulunan kontrolün TextBox kontrolü olarak cast edilmesi ile tamamlanır. Dolayısıyla, Diger.aspx sayfamızın Page_Load olay metodunda, elimizde bir adet TextBox kontrolü olacaktır. Bu TextBox kontrolü aslında, default.aspx sayfasından gelen kontroldür. Böylece, default.aspx sayfasındaki TextBox1 kontrolünün değerine ve özelliklerine bu sınıf içerisinden (Diger.aspx sayfasından) kolayca erişebiliriz. Aynı teknik, DropDownList kontrolümüz içinde geçerlidir. İşte Asp.Net 2.0, başka sayfalara form verisi gönderme işlemlerinde böylesine güçlü bir teknik ile gelmektedir.

Uygulamamızı çalıştırdığımızda ve Burası başlıklı butona tıkladığımızda basitçe sayfa kendi kendisine postalama işlemini uygular. (Postback).

![mk83_4.gif](/assets/images/2004/mk83_4.gif)

Şekil 4. Postback.

Diger başlıklı button kontrolüne bastığımızda ise, Diger.aspx isimli sayfaya gidilir. Bu sayfanın Page_Load olay metodu çalışır ve burada ilgili TextBox ve DropDownList kontrolleri, default.aspx'ten gelen kontroller baz alınarak oluşturulur. Sonuç olarak, default.aspx sayfasındaki kontroller üzerinde yer alan veriler kolayca elde edilir ve bu sayadaki Label kontrolüne içerikleri yazdırılır.

![mk83_5.gif](/assets/images/2004/mk83_5.gif)

Şekil 5. Cross-Page Posting

Cross-Page Posting işleminde elbette sorun yaratabilecek durumlarda söz konusudur. Bunların en önemlisi, bir kullanıcının, default.aspx çalıştırılmadan önce diger.aspx sayfasına ulaşmaya çalışmasıdır. Böyle bir durumda, default.aspx daha önceden çalıştırılmadığı için, buradaki kontroller oluşturulmamış olacaktır. Dolayısıyla, diger.aspx sayfasında, PreviousPage sınıfına ait metodlar, var olmayan kontrolleri bulmaya çalışacak ve referanslar oluşturulamayacaktır. Kullanıcıların böyle bir girişimde bulunması sonucunda aşağıdaki gibi bir hata mesajı ile karşılaşılır.

![mk83_6.gif](/assets/images/2004/mk83_6.gif)

Şekil 6. Oluşan Hata.

Çözüm gayet basittir. Bu sayfa çalıştırıldığında, bu sayfaya başka bir sayfadan form bilgisinin gönderilip gönderilmediği öğrenilmelidir. Bunun için, Page sınıfının IsCrossPagePostBack isimli özelliğinden yararlanılır. Bu özellik, aslında IsPostBack özelliği gibi çalışır. Sadece, başka bir sayfanın mevcut sayfaya veri postalayıp postalamadığını kontrol eden ve boolean değer döndüren bir yapıdadır. Prototipi aşağıdaki gibidir.

```csharp
public bool IsCrossPagePostBack {get;}
```

Dolayısıyla, diger.aspx sayfamızın arka kodlarını aşağıdaki gibi değiştirmemiz sorunun giderilmesini sağlayacaktır.

```csharp
public partial class Diger_aspx
{
    void Page_Load(object sender, EventArgs e)
    {
        if (PreviousPage!=null && PreviousPage.IsCrossPagePostBack)
        {
            TextBox tbRumuz = (TextBox)PreviousPage.FindControl("TextBox1");
            DropDownList lstYas = (DropDownList)PreviousPage.FindControl("DropDownList1");

            lblDurum.Text = tbRumuz.Text + lstYas.SelectedItem.Text;
        }
        else
        {
           Response.Redirect("default.aspx");
        }
    }
}
```

Böylece, eğer kullanıcılar direkt olarak diger.aspx sayfasını çalıştırırlarsa, önceden gelen bir Cross-Page Posting işlemi olmadığından, else bloğundaki kod satırı çalışacak ve default.aspx sayfasına gidilecektir.

Bu makalemizde, Asp.Net 2.0 ile gelen yeniliklerden birisine değinmeye çalıştık. Elbette şu an için, Asp.Net 2.0 henüz beta aşamasında. Yani yazılan kodlar ve kullanılan teknikler değişebilir hatta bazen kaynakların aksine çalışmayabilir. Bunlardan da haberdar oldukça sizleri bilgilendirmeye çalışacağız. Böylece geldik bir makalemizin daha sonuna. İlerleyen makalelerimizde görüşmek dileğiyle hepinize mutlu günler dilerim.