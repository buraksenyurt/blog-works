---
title: "Asp.Net 4.5 için Yeni Nesil Doğrulama(Validation)"
pubDate: 2013-06-11 14:09:00
categories:
  - Asp.Net 4.5
tags:
  - Asp.Net 4.5
  - C#
  - XML
  - .NET
  - ASP.NET
  - HTTP
  - Authentication
  - JavaScript
  - Visual Studio
---

# Asp.Net 4.5 için Yeni Nesil Doğrulama(Validation)
[![300px-Opening_crawl](images/300px-Opening_crawl_thumb.jpg)](images/300px-Opening_crawl.jpg) Merhaba Arkadaşlar,

Bilişim sektöründe yer alan ve özellikle 70li yıllarda doğanların neredeyse tamamı bu efsane cümleyi bilir.

> A long tim ago in a galaxy far, far away…

Hikaye hep yazılı bir anlatım ile başlar ve daha sonra ekran yukarıdaki yıldızlardan aşağıya doğru inerek devam eder. Sornasında ya bir uzay mekiğinin kaçıs sahnesi ya da imparatorluk güçleri ile isyancılar arasındaki savaşla karşı karşıya kalırız.

İşte geçen gün özlediğim [Star Wars](http://tr.wikipedia.org/wiki/Y%C4%B1ld%C4%B1z_Sava%C5%9Flar%C4%B1) serilerinden birisini izlerken bir den kendimi bilgisayarımın başında ve başka bir hikayenin giriş noktasında buldum.

Karşımdaki ekran da karalara bağlamış, alacalı bulacalı bir geliştirme penceresi duruyordu. İçerisinde ise HTML ve Asp.Net karışımı bir şeyler…

Hikayenin Başı

Her şey uzun bir zaman önce değil ama kısa bir süre önce Asp.Net 4.5 tabanlı bir Empty Web Application açmamla başlamış ve sonrasında olanlar olmuştu

![Confused smile](images/wlEmoticon-confusedsmile_32.png)

Aslında senaryo gereği çok basit olarak bir web form üzerinde doğrulama kontrollerini kullanacaktım. Bunun için Visual Studio 2012 ortamında Asp.Net Empty Web Application tipinden bir proje oluşturdum ve aşağıdaki Web Form içeriğini tasarladım.

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication2.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml"> 
<head runat="server"> 
    <title></title> 
</head> 
<body> 
    <form id="form1" runat="server"> 
    <div> 
    
        Nickname : 
        <asp:TextBox ID="txtNickname" runat="server"></asp:TextBox> 
 <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtNickname" ErrorMessage="Nickname girilmeli"></asp:RequiredFieldValidator> 
        <br /> 
        Password : 
        <asp:TextBox ID="txtPassword" runat="server"></asp:TextBox> 
 <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtPassword" ErrorMessage="Şifre girilmeli"></asp:RequiredFieldValidator> 
        <br /> 
        <asp:Button ID="btnLogin" runat="server" OnClick="btnLogin_Click" Text="Login" />    
    </div> 
    </form> 
</body> 
</html>
```

Web form içeriğinin görsel tasarımı ise aşağıdaki ekran görüntüsündekine benzemişti.

[![ngv_1](images/ngv_1_thumb.png)](images/ngv_1.png)

Ekranın görevi oldukça basitti. Kullanıcıdan Nickname ve Password bilgisi ile giriş yapması isteniyordu. Eğer bu TextBox kontrollerinin içeriği boş bırakılırsa da RequiredFiledValidator kontrolleri devreye girerek kullanıcıyı uyarmaktaydı.

İlk Çalışma

Herşey bana göre son derece normaldi ancak çalışma zamanı böyle demiyordu. Sonuç aşağıdaki ekran görüntüsünde ki gibi olmuştu

![Thinking smile](images/wlEmoticon-thinkingsmile_6.png)

[![ngv_2](images/ngv_2_thumb.png)](images/ngv_2.png)

Tabi ilk dikkatimi çeken nokta Unobtrusive olarak yazılan ve telafüz etmesini halen daha başaramadığım kelime idi. “Dikkati çekmeyen”, “mütevazi”, “kendi halinde”, “fark edilmeyen”, “kolay görülmeyen” gibi Türkçe karşılıkları olan kelimenin Asp.Net açısından önemini araştırırken bakın neler buldum

![Winking smile](images/wlEmoticon-winkingsmile_205.png)

Peki neden böyle oldu?

İlk olarak zamanı geriye alıp Asp.Net 4.5 öncesine bakmamız gerekiyor (Zamanı geriye almak keşke bir Framework değişikliği kadar kolay olsa değil mi?) Bu nedenle projeyi.Net Framework 4.0 odaklı hale getirip yeniden derledim.

[![ngv_4](images/ngv_4_thumb.png)](images/ngv_4.png)

ve tabi çalıştırdım

![Smile](images/wlEmoticon-smile_98.png)

[![ngv_3](images/ngv_3_thumb.png)](images/ngv_3.png)

Çalışma zamanında hiç bir sorun yoktu. Her şey yolunda görünmekteydi. Doğrulama (Validation) kontrolleri de devreye girmiş durumdaydı. Sayfanın istemci tarafına gönderilen kaynak içeriğine bakıldığında doğrulama işlemleri için üretilmiş bir takım Javascript kod parçaları olduğu ve CDATA olarak entegre edildiği kolaylıkla görülebiliyordu.

[![ngv_5](images/ngv_5_thumb.png)](images/ngv_5.png)

Bilindiği üzere bu sayede istemci tarafı sunucuya gönderilmeden de doğrulama işlemleri yerine getirilebilmektedir.

> Web tarafında doğrulama işlemleri istemci tarafında başlamakta ama sunucu tarafında da bir kontrol yapılmaktadır. Bunun en büyük sebebi istemci tarafının Javascript yürütme gibi bir desteği olmaması halinde karşı tedbir alınmak istenmesidir.

Asp.Net 4.5 ile birlikte ise daha önceden kullanılan javascript odaklı sistem yerine, varsayılan olarak HTML 5’ in data-val-controltovalidate, data-val-errormessage, data-val, data-val-evaluationfunction, data-val-initialvalue gibi nitelikleri (attribute) ve jQuery kütüphanesi ele alınmaktadır. Dolayısıla Asp.Net 4.5 tipinden bir Empty Web Application söz konusu olduğunda ve doğrulama işlemlerini uygulamak istediğimizde, bazı ayarlamaları yapmamız söz konusudur.(Bu işlemlere Web Forms tipinden bir Asp.Net uygulaması açtığınızda ihtiyaç duymayabilirsiniz)

Adımlar

Tekrar Target Framework’ ü.Net Framework 4.5’ e çekelim. Sonrasında ise Unobtrusive Validation akışı için ihtiyacımız olan jQuery kütüphanelerini NuGet paket yönetim aracı ile projemize dahil edelim. (Son sürümleri eklememiz daha akıllıca olabilir)

[![ngv_6](images/ngv_6_thumb.png)](images/ngv_6.png)

Bu install işlemi sonrası ihtiyacımız olan jQuery kütüphaneleri projeye ilave edilmiş ve Scripts klasörü içerisine atılmış olacaktır.

[![ngv_7](images/ngv_7_thumb.png)](images/ngv_7.png)

ScriptResourceDefinition Bildirimleri

Sonrasında tek yapılması gereken uygulamanın başlatıldığı bir yerde doğrulama işlemleri için gerekli path tanımlamalarının ScriptResourceDefinition sınıfı için yapılmasıdır. En uygun yer global.asax.cs içerisindeki ApplicationStart olay metodudur (global.asax varsayılan olarak bu proje şablonunda yer almamaktadır. Bir başka deyişle ilave etmeniz gerekmektedir) Bu metod içeriğini aşağıdaki gibi düzenleyerek devam edelim.

```csharp
using System; 
using System.Web.UI;

namespace WebApplication2 
{ 
    public class Global : System.Web.HttpApplication 
    {

        protected void Application_Start(object sender, EventArgs e) 
        { 
            ScriptResourceDefinition jQuery = new ScriptResourceDefinition(); 
           jQuery.Path = "~/scripts/jquery-2.0.0.min.js"; 
            jQuery.DebugPath = "~/scripts/jquery-2.0.0.js"; 
            jQuery.CdnPath = "http://ajax.microsoft.com/ajax/jQuery/jquery-2.0.0.min.js"; 
            jQuery.CdnDebugPath = "http://ajax.microsoft.com/ajax/jQuery/jquery-2.0.0.js"; 
            ScriptManager.ScriptResourceMapping.AddDefinition("jquery", jQuery); 
        }

…

}
```

Bir takım path tanımlamaları yapıldığı görülmektedir. jQuery için yapılan tanımlamalar haricinde Content Delivery Network ([CDN](https://en.wikipedia.org/wiki/Content_delivery_network)) için de bazı path bildirimleri belirtilmiştir. Senaryo da, Microsoft AJAX CDN’ leri kullanılmaktadır. İlgili path tanımlamaları ScriptResourceDefinition sınıf örneği için yapıldıktan sonra, ilgili nesne örneğinin ScriptResourceMapping özelliğine eklenmesi yeterlidir.

Yeni Bir Test

Uygulamamızı tekrar çalıştırıp test edelim. Bir hata oluşmayacak ve doğrulama kontrollerinin çalıştığı gözlemlenecektir.

[![ngv_8](images/ngv_8_thumb.png)](images/ngv_8.png)

Tabi ki bizim için daha önemli olan istemci tarafına giden kod içeriğidir. Eğer kaynak koda bakarsak aşağıdaki sonuçlarla karşılaşırız.

[![ngv_9](images/ngv_9_thumb.png)](images/ngv_9.png)

Hımmm bir terslik var gibi. Sanki buralar da HTML 5’ den hiç bir eser yok

![Disappointed smile](images/wlEmoticon-disappointedsmile_6.png)

Bu son derece doğal çünkü Web Form’ lar için bu yeni stil doğrulama işleminin yapılacağını bir yerler de belirtmemiz gerekiyor. web.config dosyası tahmin edileceği üzere en uygun yer ve içeriğini aşağıdaki gibi değiştirmemiz bu senaryo için yeterli.

```xml
<?xml version="1.0"?> 
<configuration> 
  <system.web> 
    <compilation debug="true" targetFramework="4.5"/> 
    <httpRuntime/> 
    <pages controlRenderingCompatibilityVersion="4.0"/> 
  </system.web> 
  <appSettings> 
    <add key="ValidationSettings:UnobtrusiveValidationMode" value="WebForms" /> 
  </appSettings> 
</configuration>
```

appSettings sekmesinde yer alan ValidationSettings:UnobtrusiveValidationMode key değeri, Asp.Net çalışma zamanı için anlamlıdır. Value niteliğine WebForms dışında None değeri de verilebilmektedir. None değerinin verilmesi halinde tahmin edileceği üzere eski stil doğrulama sürecine geçilmektedir. Şu anki haliyle de yeni stilin kullanılacağı belirtilmektedir.

Bir Test Daha

Öyleyse uygulama tekrardan çalıştırılır ve istemci tarafına giden kaynak kod içeriğine bakılır.

[![ngv_10](images/ngv_10_thumb.png)](images/ngv_10.png)

İstediğimiz olmuştur ve istemci tarafındaki doğrulama işlemleri için HTML 5 nitelikleri devreye girmiştir. Kontrolün boş geçilmemesi için data-val-evaluationfunction niteliğinin değeri ele alınmaktadır. Hangi kontrolün denetleneceği bilgisi için data-val-controltovalidate niteliği kullanılmaktadır. Hata mesajı ise data-val-errormessage ile belirtilir vb…

> Size tavsiyem diğer doğrulama kontrollerini de işin içine katarak senaryoyu genişletmeniz ve özellikle data-val-evaluationfunction değerlerinin nasıl üretildiğine bakmanız yönünde olacaktır.

Kıssadan Hisse

Yeni nesil doğrulama stratejisi için son teknoloji ürünüdür diyebilir miyiz acaba? Bence evet

![Winking smile](images/wlEmoticon-winkingsmile_205.png)

Hem HTML 5’ e hem de jQuery’ ye yatırım yapmış bir doğrulama süreci söz konusu. Üstelik yeni nesil çıktılar da, doğrulama operasyonları adına Javascript kullanımı (CDATA içerisindeki kısımlar) mevcut değil. Doğrulama bilgisi tamamen HTML 5 içerisine, nitelik-değer (key-value) bazlı olarak yıkılmış durumda. Elbette bunun en büyük artısı sayfa cevap boyutunun (Page Response Size) küçülmüş olması. (Elbette tarayıcı desteği de önem arz eden bir konu)

Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
