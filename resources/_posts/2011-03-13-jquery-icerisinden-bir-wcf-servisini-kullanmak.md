---
layout: post
title: "jQuery İçerisinden Bir WCF Servisini Kullanmak"
date: 2011-03-13 21:27:00 +0300
description: "Son yıllarda özellikle Web uygulamalarında jQuery’ nin oldukça fazla yaygınlaştığını görmekteyiz. Özellikle Asp.Net MVC(Model View Controller) disiplinin de…Ben her ne kadar Javascript vey jQuery tarafında uzman olmasam da sonuçta bu istemcilerin çağrıda bulunabileceği WCF(Windows Communication Foundation) servisleri olabileceğini biliyorum Sarcastic smile Dolayısıyla bu günkü yazımızda jQuery içerisinden JSON(JavaScript Object Notation) ve XML(eXtensible Markup Language) formatında veri sunan operasyonlara sahip bir WCF servisinin nasıl kullanılabileceğini incelemeye çalışıyor olacağız."
categories:
  - wcf
  - wcf-4-0
tags:
  - wcf
  - wcf-4-0
  - csharp
  - javascript
  - dotnet
  - aspnet
  - aspnet-mvc
  - xml
  - json
  - http
  - java
  - visual-studio
---
# jQuery İçerisinden Bir WCF Servisini Kullanmak
[![blg234_Giris](/assets/images/2011/blg234_Giris_thumb.jpg)](/assets/images/2011/blg234_Giris.jpg) Kahramanımız Netspecter Malezyadaki müşterisi ile buluşmak üzere café’ de beklerken, detektiflik işine girdiğinden beri en çok sevdiği içecek olan Java Chip Chocolate’ ını keyifli bir şekilde yudumlamaktadır.

Güneş batmış ve hava çoktan kararmıştır. Müşterileri çoğunlukla buluşmalara geç kalır. Aslında çevrede oturanları gizlice gözlemlediğinde onlardan birisinin kalkıp yanına geleceğini ve kendisini müşterisi olarak tanıtacağını gayet iyi bilmektedir. Bir açıdan müşterilerinin aslında kendinden önce geldiğini ama tedrigin oldukları için yaklaşmakta zorlandıklarını bilmektedir. Daha önce bu çok sık başına gelmiştir.

Masadaki metal peçetelikten arkasına doğru baktığında çok fazla dikkat çekmeyen ama müşterisi olabilecek bir kişinin oturduğunu uzun süre önce fark etmiştir. Cam kapıdaki yansımadan okumakta olduğu gazeteye tersten baktığı anlaşılmaktadır. Derken tahmin ettiği gibi olur…Adam ayağa kalkar ve masasına doğru yavaş ve tedirgin adımlarla yürür. Netspecter sakin bir şekilde Java Chip Chocolate’ inden bir yudum daha alır. Elini pardesüsünün cebine götürür ve en gelişmiş silahı 38lik Obfuscator’ unu hazırlar. Derken adam karşısına gelir ve….

“Bay Netspecter…Ben müşteriniz jQuery” der

![Smile with tongue out](/assets/images/2011/wlEmoticon-smilewithtongueout.png)

Merhaba Arkadaşlar,

Son yıllarda özellikle Web uygulamalarında jQuery’ nin oldukça fazla yaygınlaştığını görmekteyiz. Özellikle Asp.Net MVC (Model View Controller) disiplinin de…Ben her ne kadar Javascript vey jQuery tarafında uzman olmasam da sonuçta bu istemcilerin çağrıda bulunabileceği WCF (Windows Communication Foundation) servisleri olabileceğini biliyorum

![Sarcastic smile](/assets/images/2011/wlEmoticon-sarcasticsmile_2.png)

Dolayısıyla bu günkü yazımızda jQuery içerisinden JSON (JavaScript Object Notation) ve XML (eXtensible Markup Language) formatında veri sunan operasyonlara sahip bir WCF servisinin nasıl kullanılabileceğini incelemeye çalışıyor olacağız.

İlk olarak örneğimizde jQuery’ nin güncel 1.4.3 sürümlü versiyonunu kullandığımızı belirtmek isterim. Söz konusu javascript kütüphanesinin en son sürümüne [http://jquery.com/](http://jquery.com/) adresinden ulaşabilirsiniz. Bu kütüphaneyi çok doğal olarak bir Asp.Net Web Application projesi içerisinde kullanıyor olacağız. Aşağıdaki Solution görüntüsünde projenin içerisinde yer alan önemli enstrümanları görebilirsiniz.

[![blg234_Solution](/assets/images/2011/blg234_Solution_thumb.gif)](/assets/images/2011/blg234_Solution.gif)

Solution içeriğinden de anlaşılacağı üzere Scripts klasörü altında jQuery kütüphanemiz yer almakta olup bir de WCF Servis örneği kullanılmaktadır. Dilerseniz yola WCF servisimizi geliştirerek devam edelim.Bu amaçla uygulamamıza AJAX-enabled WCF Service tipinden bir öğe ekliyoruz.

```csharp
using System.ServiceModel; 
using System.ServiceModel.Activation; 
using System.ServiceModel.Web;

namespace JQueryAndWCF 
{ 
    [ServiceContract(Namespace = "http://www.azon.com/Services/Product")] 
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)] 
    public class ProductService 
    { 
        private static Category[] Categories = new Category[]{ 
                new Category{ CategoryId=1, Name="Kitap"}, 
                new Category{ CategoryId=2, Name="Dergi"}, 
                new Category{ CategoryId=3, Name="Dvd"}, 
                new Category{ CategoryId=4, Name="Cd"} 
        };

        [OperationContract] 
        [WebGet(ResponseFormat=WebMessageFormat.Json)] 
        public Category[] GetCategoriesJson() 
        { 
            return Categories; 
        }

        [OperationContract] 
        [WebGet(ResponseFormat = WebMessageFormat.Xml)] 
        public Category[] GetCategoriesXml() 
        { 
            return Categories; 
        } 
    }

    public class Category 
    { 
        public int CategoryId { get; set; } 
        public string Name { get; set; } 
    } 
}
```

ProductService isimli Ajax-Enabled WCF Service içerisinde GetCategoriesJson ve GetCategoriesXml isimli iki servis operasyonu yer almaktadır. Adlarından da anlaşılacağı üzere, JSON ve XML formatında veri döndürmek üzere tasarlanmış bu operasyonlar, Categories isimli Category tipinden bir diziye ait çalışma zamanı örneğini geriye döndürmektedirler.

JSON formatında veri çıktısı üretmek için, dikkat edileceği üzere WebGet niteliğinin ResponseFormat özelliğine WebMessageFormat.Json sabit değeri atanmıştır. Benzer şekilde XML çıktısı üretmek içinde söz konusu sabitin WebMessageFormat.Xml değeri kullanılmıştır. Tabi WebGet niteliğini kullanmış olmamız nedeni ile söz konusu servis operasyonlarına HTTP protokolünün GET metodu yardımıyla erişilmesi söz konusudur. Bu önemlidir nitekim jQuery içerisinden yapacağımız servis çağrısında GET metoduna göre talepte bulunulması gerekmektedir.

Dilerseniz Default.aspx sayfamızı geliştirerek ilerlemeye çalışalım. İlk etapta amacımız jQuery içerisinde JSON ve XML formatlı çıktıları çekebilmek olacaktır. Sonraki aşamada ise gelen veri içeriğini istemci tarafında değerlendirmeye gayret edeceğiz. Asp.Net sayfasını aşağıdaki şekilde görüldüğü gibi tasarlayabiliriz.

[![blg234_DefaultAspx](/assets/images/2011/blg234_DefaultAspx_thumb.gif)](/assets/images/2011/blg234_DefaultAspx.gif)

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="JQueryAndWCF.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script type="text/jscript" src="Scripts/jquery-1.4.3.min.js"> 
</script>

<script language="javascript" type="text/javascript">

    function GetCategoriesJson() {

        $.ajax( 
        { 
            type:"GET", 
            url:"ProductService.svc/GetCategoriesJson", 
            data:"{}", 
            contentType: "application/json; charset=utf-8",  
            dataType: "json",  
            success: OnSuccessJson,  
            error: OnErrorJson 
        });

        }

        function OnSuccessJson(data, status) { 
            var result = data; 
        }

        function OnErrorJson(data, status) { 
            alert("Exception"); 
        }

        function GetCategoriesXml() {

            $.ajax( 
        { 
            type: "GET", 
            url: "ProductService.svc/GetCategoriesXml", 
            data: "{}", 
            contentType: "application/xml; charset=utf-8", 
            dataType: "xml", 
            success: OnSuccessXml, 
            error: OnErrorXml 
        });

        }

        function OnSuccessXml(data, status) { 
            var result = data; 
        }

        function OnErrorXml(data, status) { 
            alert("Exception"); 
        }

</script>

<html xmlns="http://www.w3.org/1999/xhtml"> 
<head runat="server"> 
    <title></title> 
</head> 
<body> 
    <form id="form1" runat="server"> 
    <div> 
       <input type="button" OnClick="GetCategoriesJson()" value="Get Categories (JSON)" /> 
        <br /> 
        Ürün Kategorileri : <br /> 
        <asp:ListBox ID="lstCategoriesJson" runat="server"  Width="150px"/> 
        <br /> 
        <input type="button" OnClick="GetCategoriesXml()" value="Get Categories (XML)" /> 
        <br /> 
        Ürün Kategorileri : <br /> 
        <asp:ListBox ID="lstCategoriesXml" runat="server"  Width="150px"/> 
    </div> 
    </form> 
</body> 
</html>
```

Evetttt

![Open-mouthed smile](/assets/images/2011/wlEmoticon-openmouthedsmile_5.png)

Bakalım burada neler yaptık?

İlk olarak jQuery kullanmak istediğimiz için bu kütüphaneyi bildirimemiz gerekmektedir. Dolayısıla ilk script elementi içerisinde ilgili jQuery dosyasını işaret etmemiz şart. Bunun dışında javascript fonksiyonlarına baktığımızda GetCategoriesJson ve GetCategoriesXml isimli iki önemli operasyon olduğunu görmekteyiz. Her iki fonksiyonda kendi içerisinde $.ajax (ile başlayan bir metod çağrısında bulunmaktadır ki yazının kalbi de bu kod içeriğidir.

Bu çağrı içerisinde tahmin edileceği üzere WCF servisinin ilgili operasyonuna bir talepte bulunulmaktadır. type özelliğine atanan değer ile HTTP Get metoduna göre bir talep gönderileceği işaret edilmektedir. url özelliğinde ise servis adı ve çağırılacak olan servis operasyonu bilgisi tanımlanır. Servis tarafında tasarlamış olduğumuz operasyonlar herhangibir parametre almadığından, data özelliğine bir değer gönderimi yapılmamıştır. Ancak parametre söz konusu olduğunda data özelliğine gerekli değişken bildirimlerinin de yapılması gerekir. contentType ile servisten dönen içerik tipi belirlenirken dataType özelliği ile de dönen verinin tipi belirlenmiş olur.

Diğer yandan yapılan bu servis çağrılarının sonuçlarını iki şekilde değerlendirmemiz gerekmektedir.. Bu amaçla servis operasyonuna yapılan çağrı başarılı bir şekilde tamamlandıysa success, eğer hatalı şekilde sonlandıysa da error özelliklerine atanan javascript fonksiyonlarını çağırılmaktadır. Tabiki bu javacript fonksiyonlarını çağırmak için örneğimizde iki adet Button kontrolü ele alınmıştır. Bu kontroller dikkat edileceği üzere Postback işlemi yapan ASP.NET sunucu kontrollerinden değil aksine tipi Button olan standart HTML input elementleridir. OnClientClick niteliklerine atanan metod adları, jQuery ile ilgili servis çağrısını yapmak üzere tasarlanmış javascript fonksiyonlarını işaret etmektedir.

İlk olarak uygulamamızı debug modda çalıştırıp OnSuccessJson ve OnSuccessXml fonksiyonlarına gelen data parametrelerinin ne şekilde oluşturulduğunu gözlemlemeye çalışalım. İşte JSON çıktısı.

[![blg234_JsonDebug](/assets/images/2011/blg234_JsonDebug_thumb.gif)](/assets/images/2011/blg234_JsonDebug.gif)

Dikkat edileceği üzere Category tipinden 4 adet nesne örneği bir dizi olarak data değişkeni içerisine doldurulmaktadır. Eğer XML formatlı talepte bulunan Button bileşenini kullanırsak bu durumda OnSuccessXml javascript fonksiyonuna gelen data değişkeninin içeriğinin aşağıdaki şekildeki gibi oluşturulduğunu görebiliriz.

[![blg234_XmlDebug](/assets/images/2011/blg234_XmlDebug_thumb.gif)](/assets/images/2011/blg234_XmlDebug.gif)

Dikkat edileceği üzere Category tipinden olan dizinin içeriği XML formatında elde edilmektedir. Tabi JSON ve XML çıktıları farklı şekillerde ele alınmalıdır. JSON tarafında nesne bazlı bir yaklaşım daha kolay bir şekilde ele alınabilirken XML çıktısı için biraz Node, Element, Attribute seviyesinde düşünmek ve Parse işlemlerini buna göre yapmak gerekmektedir.

Şu an geldiğimiz noktaya bakdığımızda, jQuery içerisinden WCF servis operasyonlarının çağırılması ve JSON ile XML formatında veri alınması söz konusudur. Bir de bu verileri kullanabilirsek süper olmaz mı?

![Winking smile](/assets/images/2011/wlEmoticon-winkingsmile_4.png)

Bu amaçla servis çağrılarının başarılı olması sonucu devreye giren Success metodlarındaki javascript kodlarını aşağıdaki gibi değiştirmemiz yeterli olacaktır.

```javascript
<script language="javascript" type="text/javascript">

    function GetCategoriesJson() {

        $.ajax( 
        { 
            type: "GET", 
            url: "ProductService.svc/GetCategoriesJson", 
            data: "{}", 
            contentType: "application/json; charset=utf-8", 
            dataType: "json", 
            success: OnSuccessJson, 
            error: OnErrorJson 
        });

    }

    function OnSuccessJson(data, status) { 
        var ddl = document.getElementById("lstCategoriesJson"); 
        
        //Minik bir temizlik 
        for (i = ddl.options.length - 1; i >= 0; i--) { 
            ddl.remove(i); 
        }

       for (i = 0; i < data.d.length; i++) {

            var option = document.createElement("option"); 
            option.innerHTML = data.d[i].Name.toString(); 
            option.value = data.d[i].CategoryId.toString(); 
            ddl.appendChild(option); 
        } 
    }

    function OnErrorJson(data, status) { 
        alert("Exception"); 
    }

    function GetCategoriesXml() {

        $.ajax( 
        { 
            type: "GET", 
            url: "ProductService.svc/GetCategoriesXml", 
            data: "{}", 
            contentType: "application/xml; charset=utf-8", 
            dataType: "xml", 
            success: OnSuccessXml, 
            error: OnErrorXml 
        });

    }

    function OnSuccessXml(data, status) { 
        var ddl = document.getElementById("lstCategoriesXml");

        //Minik bir temizlik 
        for (i = ddl.options.length - 1; i >= 0; i--) { 
            ddl.remove(i); 
        }

        for (i = 0; i < data.childNodes[0].childNodes.length; i++) { 
            var option = document.createElement("option"); 
            option.value = data.childNodes[0].childNodes[i].childNodes[0].text; 
            option.innerHTML = data.childNodes[0].childNodes[i].childNodes[1].text; 
            ddl.appendChild(option); 
        } 
    }

    function OnErrorXml(data, status) { 
        alert("Exception"); 
    }

</script>
```

Tabi JSON ve XML formatındaki veri dönüşleri birbirlerinden farklı şekilde ele alınmak zorundadır. Özellikle JSON tarafında nesne bazlı bir yaklaşım söz konusu olduğundan data içeriğinden Category tipi için tanımlanmış olan Name veya CategoryId gibi çalışma zamanı özelliklerine ulaşmak oldukça kolaydır. Ne varki XML veri okuması sırasında Node’ lar içerisinde (Özellikle childNodes attribute ile gelen listelerde) dolaşmak gerekir. Her iki fonksiyonda basit anlamda sayfa üzerinde yer alan liste kontrollerini bulmakta ve option tipinden elementler ilave etmektedir. Bu elementlerin innerHTML niteliklerine Name, value niteliklerine ise CategoryId değerleri atanmaktadır. Uygulamamızı bu haliyle çalıştırdığımızda aşağıdaki ekran görüntüsünde yer alan sonuçları elde ederiz.

[![blg234_RuntimeResult](/assets/images/2011/blg234_RuntimeResult_thumb.gif)](/assets/images/2011/blg234_RuntimeResult.gif)

Görüldüğü gibi jQuery kütüphanesi içerisinden bir WCF servis çağrısını gerçekleştirmek ve operasyon çağrısı sonucu üretilen veriyi javascript tarafında ele almak son derece kolaydır. Tabi bu noktada akla gelen sorulardan biriside bu tip bir işlevselliğin ASP.NET MVC tarafında nasıl ele alınabileceğidir

![Winking smile](/assets/images/2011/wlEmoticon-winkingsmile_4.png)

Bunu da ilerleyen yazılarımızdan birisinde ele almaya çalışıyor olacağım. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[JQueryAndWCF.rar (47,04 kb)](/assets/files/2011/JQueryAndWCF.rar) [Örnek Visual Studio 2010 Ultimate sürümü üzerinde geliştirilmiş ve test edilmiştir]