---
layout: post
title: "Haydi Bir Captcha Kontrolü Yazalım"
date: 2011-09-26 23:11:00 +0300
description: "Olayı daha net bir şekilde kavrayabilmek adına örnek bir uygulama üzerinden gitmemizde yarar vardır. Bu amaçla dilerseniz Visual Studio 2010 ortamı üzerinde basit bir Web Site şablonu açarak işe başlayalım. Söz konusu Captcha kontrolü esas itibariyle Drawing tipleri tarafından çizilen bir resim ve üzerine yerleştirilen bir takım sayısal veya karakter bazlı verilerden oluşmaktadır. Burada bahsedilen çizim işini ise genellikle bir Handler tipi üstlenmektedir. İşte Handler içeriğimiz."
categories:
  - aspnet-4-0
tags:
  - aspnet-4-0
  - csharp
  - xml
  - dotnet
  - aspnet
  - linq
  - http
  - generics
  - visual-studio
---
# Haydi Bir Captcha Kontrolü Yazalım
[![terzi](/assets/images/2011/terzi_thumb.gif)](/assets/images/2011/terzi.gif) Merhaba Arkadaşlar,

Şu sıralarda kurumsal bir Asp.Net eğitimi vermekteyim. Eğitim içeriği oldukça geniş ve güzel konuları içermekte. Bunlardan bir tanesi de Asp.Net uygulamalarında Captcha doğrulamasının kullanımı. Bildiğiniz üzere web ortamı üzerinde özellikle Form veri girişlerinin yapıldığı senaryolarda akıllı robotların gereksiz yere post atma işlemlerini engellemek çok önemlidir.

Örneğin şu an kullanmakta olduğum BlogEngine sürümünde, sisteme monte edilmiş bir Captcha kontrolü veya modülü bulunmamakta (Biliyorum son sürüme geçmeliydim ![Üzgün gülümseme](/assets/images/2011/wlEmoticon-sadsmile_6.png)). Bu nedenle özellikle yorum kısımlarında dünyanın çeşitli bölgelerindeki robot programlarının tacizlerine fazlasıyla maruz kalmaktayım. Anlamsız pek çok bilgiden oluşan spam yorumlar söz konusu.

E tabi diyebilirsiniz ki, "Ya Hocam sen de amma yaptın...Eklesene şu Captcha kontrolünü bloğa...Hayret bişi"

![Açık ağızlı gülümseme](/assets/images/2011/wlEmoticon-openmouthedsmile_14.png)

E ama ne demişler bilirsiniz... Terzi kendi söküğünü dikemezmiş (Aslında ben söküğümü nedense dikmek istemiyorum sanırım. Üşengeçlik bu olsa gerek)

Neyse sözü fazla uzatmadan konumuza devam edelim. Asp.Net uygulamalarında form veri girişlerinin yapılabildiği ve post işlemlerinin gerçekleştirilebildiği her ortamda robot saldırılarına maruz kalmamız olasıdır. Bu sebepten formu dolduran kişinin gerçek insan gözüne sahip olduğunu bir şekilde anlamamız ve bunu doğrulatmamız gerekmektedir. İşte Captcha kontrolleri bu noktada devreye girmektedir. Üretilen resim formatlı metinsel içeriklerin o anki HTTP Context verisinden okunması mümkün değildir. En azından şu an için. Dolayısıyla bu içeriği gören birisinin elle giriş yapması gerekmektedir. Bize kalan ise sadece ve sadece Captcha içeriği ile kullanıcının girdiği verinin eşit olup olmadığını kontrol etmektir.

Olayı daha net bir şekilde kavrayabilmek adına örnek bir uygulama üzerinden gitmemizde yarar vardır. Bu amaçla dilerseniz Visual Studio 2010 ortamı üzerinde basit bir Web Site şablonu açarak işe başlayalım. Söz konusu Captcha kontrolü esas itibariyle Drawing tipleri tarafından çizilen bir resim ve üzerine yerleştirilen bir takım sayısal veya karakter bazlı verilerden oluşmaktadır. Burada bahsedilen çizim işini ise genellikle bir Handler tipi üstlenmektedir. İşte Handler içeriğimiz.

```csharp
<%@ WebHandler Language="C#" Class="CaptchaHandler" %>

using System; 
using System.Web; 
using System.Drawing; 
using System.IO; 
using System.Drawing.Imaging; 
using System.Web.SessionState;

public class CaptchaHandler 
    : IHttpHandler,IReadOnlySessionState 
{     
    public void ProcessRequest (HttpContext context) {

        using (Bitmap bmp = new Bitmap(220, 80)) //120X40 uzunluğunda bir Bitmap tanımlıyoruz. 
        { 
            using (Graphics painter = Graphics.FromImage(bmp)) // Grafik nesnesi söz konusu alan üzerinde çizim yapabilmek için bmp isimli nesneden üretiliyor 
            { 
                painter.Clear(Color.LightGray);

                // Font nesnesi üretiliyor. 
                using (Font writer = new Font("Helvetica", 10, System.Drawing.FontStyle.Bold)) 
                { 
                    string capcthaContent = string.Empty; 
                    // Session nesnesinden Captcha kontrolünün içeriğini oluşturan bilgi alınır  
                     
                        if (context.Session["CaptchaContent"].ToString() != null) 
                            capcthaContent = context.Session["CaptchaContent"].ToString(); 
                    
                    // ve bu içerik belirlenen font, fırça rengi bilgileri ile Graphics tipinden olan painter üzerine çizdirilir 
                    painter.DrawString(capcthaContent, writer, Brushes.Black, 3, 3);

                    using (MemoryStream mStream = new MemoryStream()) 
                    { 
                        bmp.Save(mStream, ImageFormat.Gif);

                        // üretilen captcha resmini HttpContext tipinin Response özelliği üzerinden Binary formatta yazdırarak görünmesini sağlıyoruz. 
                        byte[] bmpBytes = mStream.GetBuffer(); 
                        context.Response.ContentType = "image/gif"; 
                        context.Response.BinaryWrite(bmpBytes); 
                    }                     
                } 
            } 
        } 
        context.Response.End(); 
    } 
  
    public bool IsReusable { 
        get { 
            return false; 
        } 
    } 
}
```

Aslında Handler tipinin temel görevi Captcha resmini içeriği ile birlikte çizmektir. Çizim işlemi sırasında bir Asp.Net sunucu kontrolünden yararlanılması gerekmektedir. Bu sunucu kontrolü yukarıda yazılmış olan Handler tipinden yararlanacaktır. Söz konusu kontrol içeriğini aşağıdaki gibi oluşturabiliriz.

```csharp
using System; 
using System.Web; 
using System.Web.UI; 
using System.Web.UI.WebControls; 
using System.Collections.Generic;

namespace CustomControls 
{ 
    public class CaptchaBox 
        : Control 
    { 
        Image capcthaImage;

        public string Text 
        { 
            get 
            { 
                if (HttpContext.Current.Session["CaptchaContent"] != null) 
                    return HttpContext.Current.Session["CaptchaContent"].ToString(); 
                else 
                    return null; 
            } 
        }

        protected override void OnLoad(EventArgs e) 
        { 
            base.OnLoad(e);

            List<string> strArray=new List<string> { "A", "B", "C", "Ç","D", "E", "F", "G","Ğ", "H", "I","İ", "J", "K", "L", "M", "N", "O","Ö", "P", "R", "S", "Ş","T", "U","Ü", "V","Y", "Z", "a", "b", "c", "ç","d", "e", "f", "g", "h","ı", "i", "j", "k", "l", "m", "n", "o","ö", "p", "r", "s","ş", "t", "u","ü" ,"v", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

            Random rnd = new Random(); 
            string strCaptcha = string.Empty; 
            // 10 haneli rastgele bir Captcha değeri üretmek için aşağıdaki döngüden yararlanıyoruz. 
            for (int i = 0; i < 10; i++) 
            { 
                int j = Convert.ToInt32(rnd.Next(0, strArray.Count-1)); 
                strCaptcha += strArray[j].ToString(); 
            }

            HttpContext.Current.Session.Add("CaptchaContent", strCaptcha);

            capcthaImage = new Image(); 
            capcthaImage.ImageUrl = "~/CaptchaHandler.ashx"; 
            // captcha resmini üretecek olan Handler' ımızı ImageUrl özelliğine set ettikten sonra CaptchaBox kontrolünün Controls koleksiyonuna ekliyoruz. 
            this.Controls.Add(capcthaImage); 
        } 
    } 
}
```

Oldukça basit bir içeriğe sahip olan bu kontrol aslında strArray isimli string tipindeki List koleksiyonu içerisinden rastgele 10 değeri alarak Captcha handler tipine vermektedir. Bunun karşılığında Captcha Handler tipinin ürettiği resim içeriği sayfaya basılmaktadır. CustomControls isim alanında yer alan kontrolümüzü bir ön takı (Tag Prefix) ile kullanabilmek adına web.config dosyasında aşağıdaki bildirimi yapmamız yeterlidir.

```xml
<?xml version="1.0"?> 
<configuration> 
    <system.web> 
        <compilation debug="true" targetFramework="4.0"/> 
        <pages> 
            <controls> 
                <add namespace="CustomControls" tagPrefix="ccntrl"/> 
            </controls> 
        </pages> 
    </system.web> 
</configuration>
```

Artık örnek bir web sayfasında söz konusu kontrolümüzü test edecek içeriği ve kod parçasını geliştirebiliriz. Dilerseniz aspx sayfamızın ön yüz içeriğini aşağıdaki gibi geliştirelim.

[![bei_1](/assets/images/2011/bei_1_thumb.gif)](/assets/images/2011/bei_1.gif)

```xml
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml"> 
<head runat="server"> 
    <title></title> 
</head> 
<body> 
    <form id="form1" runat="server"> 
    <div> 
    <ccntrl:CaptchaBox ID="Captcha1" runat="server" /> 
    <br /> 
    Captcha İçeriğini Giriniz : 
    <asp:TextBox ID="txtContent" runat="server" /> 
    <br /> 
    <asp:Button ID="btnSubmit" runat="server" Text="Validate" /> 
    </div> 
    </form> 
</body> 
</html>
```

Burada dikkat edilmesi gereken bir nokta vardır. O da düğmeye bastığımızda PostBack işlemi sırasında Handler tipinin çok doğal olarak tekrar devreye girmesi ve yeniden taze bir Captcha içeriği üretecek olmasıdır. Dolayısıyla TextBox içeriği ile Capctha kontrolünü karşılaştırırken PreInit olayından yararlanabiliriz. İşte aspx.cs tarafındaki kodlarımız.

```csharp
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Web; 
using System.Web.UI; 
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page 
{ 
    // Postback olması durumunda Captcha içeriği yeniden üretileceğinden PreInit olay metodunu değerlendiriyoruz. 
    protected override void OnPreInit(EventArgs e) 
    { 
        if (IsPostBack) 
            if (Session["CaptchaContent"].ToString() == Request["txtContent"]) // Session' da tutulmakta olan Captcha içeriği ile kullanıcının girmiş olduğu bilgiyi karşılaştırıyoruz. 
                Response.Write("Sen bir robot değilsin :)"); 
            else 
                Response.Write("Seni gidi seni :)");

        base.OnPreInit(e); 
    } 
} 
```

Artık uygulamamızı test edebiliriz ne dersiniz. Hatta edebilirsiniz

![Gülümseme](/assets/images/2011/wlEmoticon-smile_14.png)

Örneğin ben Captcha içeriğini doğru girdikten sonra aşağıdaki çıktıyı elde ettim. Tabi burada post işleminden sonra yeni bir Captcha değeri üretildiğini unutmayalım. Bu yüzden TextBox kontrol içeriği ile şu anki Capctha içeriği farklı görünüyor. En iyisi siz uygulamayı bir çalıştırında ne demek istediğimi kendiniz görün

![Göz kırpan gülümseme](/assets/images/2011/wlEmoticon-winkingsmile_65.png)

[![bei_2](/assets/images/2011/bei_2_thumb.gif)](/assets/images/2011/bei_2.gif)

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[CaptchaKullanimi.rar (3,23 kb)](/assets/files/2011/CaptchaKullanimi.rar)