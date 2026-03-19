---
layout: post
title: "WCF WebHttp Service, JSON, jQuery, Ajax ve CORS ile Yeni Bir Macera"
date: 2014-09-03 08:27:00 +0300
description: "Bu yazımızda bir kavram ve terim cümbüşü içerisinde yer alacağımızı söyleyebilirim. Yazacağımız basit bir WCF servisini öncelikle REST tabanlı çalışır hale getireceğiz. Ardından söz konusu servise jQuery kütüphanesinden yararlanarak bir Ajax çağrısı gerçekleştireceğiz. Temel hedefimiz ise HTTP Post metoduna göre bir içeriği tarayıcı üzerinden servise göndermek olacak. Lakin JSON(JavaScript Object Notation) tipinden bir nesne kullanacağız. Kabaca aşağıdaki çizelge de görülen durumun söz konusu olduğunu söyleyebiliriz."
categories:
  - wcf
  - wcf-4-5
  - wcf-webhttp-services
tags:
  - wcf
  - wcf-4-5
  - wcf-webhttp-services
  - csharp
  - xml
  - dotnet
  - aspnet
  - windows-forms
  - rest
  - json
  - http
  - iis
  - javascript
  - generics
---
# WCF WebHttp Service, JSON, jQuery, Ajax ve CORS ile Yeni Bir Macera
[![browser_wars](/assets/images/2014/browser_wars_thumb.jpg)](/assets/images/2014/browser_wars.jpg) Merhaba Arkadaşlar,

Bir süredir şirket içerisinde kullanılacak olan web tabanlı bir.Net uygulamasının geliştirilmesinde görev almaktayım. Uygulama, yürütülen süreç gereği her iterasyon sonunda yeni özellikler eklenmiş ve hataları giderilmiş biçimde Üretim (Production) ortamına taşınmakta.

Projede kaynak sıkıntısı nedeniyle uzun bir süre servis katmanı haricinde kalan arayüz tarafı ile de ilgilenmek zorunda kaldım. Arayüz tarafı ile uğraşırken iş biriminden gelen isteklere göre CSS (Cascading Style Sheets) ve bol miktarda Javascript kodlamak benim gibi acemiler için epeyce zorlayıcıydı. Lakin en çok zaman kaybettiğim vaka, şirket içinde kullanılmakta olan eski,yeni ve çeşitli tipteki tarayıcıların uyumlu çalışmasının sağlanabilmesiydi. Kimi lokasyonda Internet Explorer 8, kimi yerlede Google Chrome’ un en güncel sürümü bulunmakta. Hatta global çevrimde Firefox standart olarak her bilgisasyarda yüklü geliyor.

Şunu fark ettim ki, tarayıcı savaşları makalelerde okuduğumuzdan çok daha ciddi boyutta. Havada uçuşan standartları farklı farklı yorumlama biçimleri nedeniyle her tarayıcıya uygun standart çözümler üretmek gerçekten zormuş. En azından benim için

![Smile](/assets/images/2014/wlEmoticon-smile_105.png)

Basit bir CSS'in Internet Explorer'da sorunsuz çalışırken Chrome'da problem çıkarttığına, Chrome'da dertsiz işleyen bir Ajax Control Toolkit kontrolünün, Firefox’ un eski bir sürümünde hiç çalışmadığına şahit oldum. Hal böyle olunca çalışma zamanında, tarayıcıların debug kabiliyetleri ile de haşır neşir olmak zorunda kaldım. Sıkıcı mıydı? Hayır

![Winking smile](/assets/images/2014/wlEmoticon-winkingsmile_216.png)

Aksine benim için farklı ve değer katan deneyimlerdi. İşte bu düşünceler geçtiğimiz günlerde yine internet üzerinde bir şeyler araştırıp öğrenmeye çalışırken kendimi farklı bir macera içerisinde buldum. Sonunda bunu kaleme almanın yararlı olacağını düşündüm ve işte buradayım.

Senaryo

Bu yazımızda bir kavram ve terim cümbüşü içerisinde yer alacağımızı söyleyebilirim. Yazacağımız basit bir WCF servisini öncelikle REST tabanlı çalışır hale getireceğiz. Ardından söz konusu servise jQuery kütüphanesinden yararlanarak bir Ajax çağrısı gerçekleştireceğiz. Temel hedefimiz ise HTTP Post metoduna göre bir içeriği tarayıcı üzerinden servise göndermek olacak. Lakin JSON (JavaScript Object Notation) tipinden bir nesne kullanacağız. Kabaca aşağıdaki çizelge de görülen durumun söz konusu olduğunu söyleyebiliriz.

[![restJquery_7](/assets/images/2014/restJquery_7_thumb_1.png)](/assets/images/2014/restJquery_7_1.png)

Bu toplu senaryo aslına bakılırsa günümüzün popüler pek çok web tabanlı uygulamasında kullanılabilecek türden. Haydi gelin parmaklarımızı sıvayalım...

Servis Tarafının Geliştirilmesi

İlk olarak aşağıdaki servis sözleşmesini içeren bir WCF Service Application projesi açarak yola çıkabiliriz. Söz konusu projede IProductService isimli bir sözleşme (Service Contract) yer alacak.

```csharp
using System.ServiceModel; 
using System.ServiceModel.Web;

namespace AzonServices 
{ 
    [ServiceContract] 
    public interface IProductService 
    { 
        [OperationContract] 
        [WebInvoke(Method = "POST", 
                   RequestFormat = WebMessageFormat.Json, 
                   ResponseFormat = WebMessageFormat.Json, 
                   UriTemplate = "AddProduct")] 
        string PostProduct(Product NewProduct); 
    } 
}
```

Senaryomuzda sadece HTTP Post metodunu ele almak istediğimizden basit bir operasyon söz konusu. Önemli olan servis operasyonunun WebInvoke niteliği (attribute) ile işaretlenmiş olmasıdır. WebInvoke niteliği bu operasyonun HTTP tabanlı taleplere cevap verecek şekilde kullanılabileceğini ifade etmektedir.

Niteliğin içerisinde dikkat edileceği üzere bir kaç özelliğin set edildiği görülmektedir. Method özelliğine atanan değer ile, operasyonun HTTP Post taleplerine cevap vereceği belirtilmektedir. RequestFormat ve ResponseFormat özellikleri ile operasyona gelen ve istemcilere cevap olarak dönen içeriklerin JSON formatında serileştirileceği ifade edilir. Son olarak bir Uri şablonu atanmıştır. UriTemplate'e atanan AddProduct ifadesi, istemci tarafının göndereceği HTTP Post talebinde kullanılacaktır.

Servis metodu Product tipinden bir nesne örneğini alıp geriye string tipte içerik döndürecek şekilde tasarlanmıştır. Product tipi oldukça basit bir içeriğe sahiptir.

```csharp
namespace AzonServices 
{ 
    public class Product 
    { 
        public int ProductId { get; set; } 
        public string Title { get; set; } 
        public decimal ListPrice { get; set; } 
    } 
}
```

Gelelim ProductService.svc öğesinin kodlarına.

```csharp
using System; 
using System.Collections.Generic;

namespace AzonServices 
{ 
    public class ProductService 
        : IProductService 
    { 
        List<Product> productList = new List<Product>();

        public string PostProduct(Product NewProduct) 
        { 
            productList.Add(NewProduct); 
            return Guid.NewGuid().ToString(); 
        } 
    } 
}
```

ProductService.svc içerisinde çok özel bir kod parçası yoktur. Sadece generic bir List örneğine, PostProduct metoduna gelen Product örneğinin eklenmesi işlemi icra edilmektedir. Test sırasında istemcinin doğru bir cevap aldığını kolayca tespit etmek adına metod geriye benzersiz bir Guid değeri döndürmektedir.

EndPoint Bildirimi

Servis tarafı için önem arz eden konulardan birisi de EndPoint tanımlamasıdır. Servis, REST tabanlı olacak şekilde çalışabilmelidir. WCF bu noktada WebHttpBinding isimli Binding tipini sağlamaktadır. Bu sebepten web.config içerisinde gerekli tanımlamaların yapılması gerekmektedir. Aynen aşağıda görüldüğü gibi.

```xml
<?xml version="1.0" encoding="utf-8" ?> 
<configuration> 
    <system.serviceModel> 
        <services> 
            <service name="AzonServices.ProductService"> 
                <endpoint address="" 
                               binding="webHttpBinding" 
                               contract="AzonServices.IProductService" behaviorConfiguration="webBehavior"></endpoint> 
            </service> 
        </services> 
        <behaviors> 
            <endpointBehaviors> 
                <behavior name="webBehavior"> 
                    <webHttp/> 
                </behavior> 
            </endpointBehaviors> 
        </behaviors> 
        <serviceHostingEnvironment multipleSiteBindingsEnabled="true" /> 
    </system.serviceModel> 
</configuration>
```

endpoint elementi içerisinde yer alan binding niteliğine webHttpBinding atanması haricinde bir de HTTP davranışının verilmesi söz konusudur. Bunun için dikkat edileceği üzere bir endPoint Behavior tanımlaması yapılmış ve webHttp değeri eklenmiştir. Eğer bir problem yoksa ProductService.svc dosyasının tarayıcı üzerinde aşağıdaki gibi açılması gerekir.

[![restJquery_1](/assets/images/2014/restJquery_1_thumb_1.png)](/assets/images/2014/restJquery_1_1.png)

> Servisin Metadata Publishing özelliği kapalıdır. Bilindiği üzere REST tabanlı servislere HTTP protokolü ve metodları ile erişilmektedir. Bu yüzden istemci tarafında bir Proxy nesnesi kullanılmasına gerek yoktur.

İstemci Tarafı

Gelelim istemci uygulamanın geliştirilmesi. Servis tüketicisi bir Web uygulaması olarak inşa edilecektir. Detayları bir kenara bırakıp asıl konuya odaklanmak istediğimizden Asp.Net Empty Web Application projesi bizim için biçilmiş kaftandır. Web uygulamamızda jQuery kullanacağımızdan en azından ilgili javascript kütüphanesinin eklenmesi gerekir.

> Bunun için [http://jquery.com/download/](http://jquery.com/download/) adresine giderek istediğiniz bir sürümü seçebilirsiniz. Sürüm seçiminde bu sayfada yazılan notlara dikkat etmenizi öneririm. Eğer kurumunuzun tarayıcılar ile ilişkili bazı kuralları varsa ve özellikle eski tarayıcılar ile çalışıyorlarsa uygun jQuery kütüphanesinin seçilmesi doğru olacaktır.

Ben örnek projemizde jQuery-2.1.0.min.js sürümünü kullanmayı tercih ettim. İlgili Script dosyasını projeye ekledikten sonra Default.aspx sayfasını aşağıdaki gibi geliştirebiliriz.

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ClientApp.Default" %>

<html xmlns="http://www.w3.org/1999/xhtml"> 
<head runat="server"> 
    <title>REST Service Test</title> 
    <meta http-equiv="X-UA-Compatible" content="IE=10" /> 
</head> 
<body> 
    <script type="text/javascript" src="Scripts/jquery-2.1.0.min.js">

    </script> 
    <script type="text/javascript"> 
        function AddNewProduct() {

            var product = { 
                "ProductId": 1220, 
                "Title": "ElCiii 4580 Laptop", 
                "ListPrice": "1499" 
            }; 
                        
            $.ajax({ 
                type: "POST", 
                url: "http://localhost:61954/ProductService.svc/AddProduct", 
                data: JSON.stringify(product), 
                contentType: "application/json; charset=utf-8", 
                dataType: "json", 
                success: function (data, status, xmlRequest) { 
                    alert("JSON içeriği "+JSON.stringify(product)+". "+data + " numaralı ürün eklenmiştir");  
                }, 
                error: function (xmlRequest,status,errorThrown) { 
                    alert(xmlRequest.responseText); 
                } 
            }); 
        } 
    </script> 
    <form id="form1" runat="server"> 
    <div> 
        <input type="button" value="Add New Product" onclick="AddNewProduct()" /> 
    </div> 
    </form> 
</body> 
</html>
```

Çalışma Şekli

Şimdi web sayfasını biraz inceleyelim. Burada koşullara ve şartlara göre uygulanmış bazı hileler de bulunmaktadır. İlk olarak projeye ilave edilmiş jQuery kütüphanesinin kullanılacağı belirtilmiştir. Form üzerinde button tipinden bir input elementi yer almaktadır. İstemci tarafında bu buton tıklandığında ise AddNewProduct isimli javascript fonksiyonu çalıştırılmaktadır. Peki fonksiyon içerisinde neler olmaktadır?

İlk olarak product isimli bir tip oluşturulduğunu ve ProductId, Title, ListPrice özelliklerine bir takım değerler atandığını görebiliriz. Bu tanımlamayı takip eden satırda ise ajax fonksiyon çağrısı gerçekleştirilmektedir. ajax fonksiyonun pek çok parametresi bulunmaktadır. Örnekte HTTP Post çağrısı gerçekleştirileceğinden type özelliğine POST değeri atanmıştır. url özelliği tahmin edileceği üzere HTTP Post talebinin gönderileceği WCF Servis adresini işaret etmektedir. Bu adres tanımında yer alan AddProduct son ekine ayrıca dikkat edilmelidir.

Hatırlanacağı üzere bu bilgi servis operasyonunun WebInvoke niteliğinde belirtilmiştir. data kısmında gerçekleştirilen stringify çağrısı, parametre olarak aldığı product nesne örneğini JSON formatına çevirmek üzere kullanılır. Böylece servise gönderilecek olan JSON içeriği oluşturulur. contentType özelliğine atanan değer ile içerik tipinin JSON olacağı ve karakter seti olarak utf-8 standardının kullanılacağı belirtilmektedir. dataType özelliği POST işlemi sırasında kullanılan veri tipinin JSON olduğunu işaret eder. success ve error değişkenleri tahmin edileceği üzere çağrının başarılı veya hata olması durumlarnda devreye giren fonksiyonları taşımaktadır. Her iki fonksiyon da standart olarak XmlHttpRequest tipini kullanır.

Biz örneğimizde bu fonksiyonellikler içerisinde önemli bir iş yapmıyoruz. Sadece çağrının başarılı olması halinde gönderilen JSON içeriğini ve servisden gelen GUID değerini bir mesaj kutusu içerisine gösteriyoruz. Pek tabi gelen içeriğin sayfa üzerinde yer alan bir takım kontrollere basılması da düşünülebilir.

Web sayfasında dikkat edilmesi gereken noktalardan birisi de title elementinin hemen altında kullanılan meta tag'dir.

Bunu şöyle ifade etmeye çalışalım. Örneği gerçekleştirdiğimiz sistemde Internet Explorer'ın 10 sürümü bulunmakta ve web sayfasının aslında IE Compatibility Mode'da çalıştığı görülmektedir. Nitekim bu bildirimin meta tag olarak bildirilmemesi halinde istemci tarafında bir script hatası ile karşılaşılmaktadır.

[![restJquery_2](/assets/images/2014/restJquery_2_thumb_1.png)](/assets/images/2014/restJquery_2_1.png)

> Bu sorun IE 11' de kendini göstermeyebilir. Ya da jQuery kütüphanesinin daha eski bir sürümü böyle bir hatayı oluşturmayabilir. Hatta bu meta tag açık olduğunda Document Mode'un IE 9, 8 ve 7 olduğu durumlarda kütüphanenin aynı hatayı vermeye devam ettiği de tespit edilmiştir. Bu tarayıcıları anlamak hakikaten zor
>
> ![Confused smile](/assets/images/2014/wlEmoticon-confusedsmile_33.png)

Uyumluluk Sonrası Chrome Öncesi ve CORS

Örneğimizi Internet Explorer ile (en azından sistemde var olan sürümü ile) uyumlu hale getirdik diyebiliriz. Default.aspx sayfasında Add New Product başlıklı butona bastığımızda aşağıdakine benzer bir mesaj kutusu ile karşılaşmamız gerekmektedir.

[![restJquery_3](/assets/images/2014/restJquery_3_thumb_1.png)](/assets/images/2014/restJquery_3_1.png)

Görüldüğü üzere başarılı bir şekilde servis çağrısı yapılmıştır. JSON içeriği üretilmiş ve servisden benzersiz bir GUID değeri elde edilmiştir. Ne var ki örnek Chrome'da çalışmamaktadır

![Surprised smile](/assets/images/2014/wlEmoticon-surprisedsmile_6.png)

(Yine örneğin geliştirildiği makinedeki tarayıcı sürüm için böyle bir durum oluştuğunu ifade edelim)

[![restJquery_4](/assets/images/2014/restJquery_4_thumb_1.png)](/assets/images/2014/restJquery_4_1.png)

Pek de sevimli olmayan bir hata mesajı

![Sad smile](/assets/images/2014/wlEmoticon-sadsmile_16.png)

Eğer Chrome tarafında debug işlemi uygulanırsa aşağıdaki gibi bazı hataların oluştuğuna şahit olunur. İşte buton tıklandıktan sonraki durum.

[![restJquery_5](/assets/images/2014/restJquery_5_thumb_1.png)](/assets/images/2014/restJquery_5_1.png)

3 hata mesajı söz konusudur. Hata mesajlarının ikisi jQuery kütüphanesinden gelmektedir ama ana fikir söz konusu metod çağrısına izin verilmemiş olmasıdır. Aslında dikkatli gözler şunu hemen fark edecektir. Web uygulamasının host ediliği port ile WCF Service uygulamasının host edildiği port birbirinden farklıdır. Bu Cross Domain çağrı Chorme tarafından işlenmemiştir. Çözüm olarak (ki burada istediğimiz sadece servisin Chrome üzerinden IE'de olduğu gibi çağırılabildiğini görmektir) ilgili servisin ve web uygulamasının aynı domain'de host edilmesi sağlanabilir. Yani IIS altına atılmaları halinde her hangibir sorun olmadan çağırılabildikleri görülecektir.

> Modern tarayıcıların bu tip Cross Domain referans çağrılarına izin vermediği bilinmektedir. Servislerin bu noktada çözüm olarak istemciden gelecek olan bu tip Header'ları kabul edecek şekilde tesis edilmesi gerekmektedir. Bu sıkıntı CROS olarak isimlendirilmiştir. Dolayısıyla servis tarafı CORS (Cross-Origin Resource Sharing) özelliğini desteklemelidir. Bir başka deyişle servisin istemciden gelen Header bilgisine göre POST talebini kabul edecek şekilde ayarlanması sorunu çözecektir.

Sorunu Büyüttük

Görüldüğü üzere yeni bir mücade ile karşı karşıyayız. WCF servisini CORS destekli hale getirmek çözümlerden bir tanesi. Ancak oldukça zahmetli olan bu yola yazımızda değinmeyeceğiz. Yine de ilgilenler [http://enable-cors.org/server_wcf.html](http://enable-cors.org/server_wcf.html) adresine uğrayabilirler. Daha basit bir çözüm olarak WCF Service Application'ın aslında bir Web uygulaması gibi davranış gösterdiğini düşünerek hareket edeceğiz. Dolayısıyla bir global.asax dosyası ve gelen uygulamaya gelen taleplerin yakalandığı olay metodları söz konusudur. Bu noktada Application_BeginRequest metodu içeriğini aşağıdaki kod parçasında görüldüğü gibi yazmamız yeterli olacaktır.

```csharp
protected void Application_BeginRequest(object sender, EventArgs e) 
{ 
    HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*"); 
    if (HttpContext.Current.Request.HttpMethod == "OPTIONS") 
    { 
        HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST"); 
        HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type"); 
        HttpContext.Current.Response.End(); 
    } 
}
```

BeginRequest metodu tahmin edileceği üzere WCF servisini host ettiğimiz uygulamaya gelecek her talep için devreye girecektir. jQuery ile gerçekleştirdiğimiz ajax çağrısında ContentType Header bilgisi kullanılmış ve POST metoduna göre talep de bulunulmuştur. BeginRequest metodunun yaptığı pratikte bu şekilde gelen istekleri geri çevirmemek ve istemci tarafına da uygun olan Header bilgisini göndermektir. Söz konusu değişiklik sonrası uygulamanın Chrome üzerinde de sorunsuz bir şekilde çalışabildiği görülecektir.

[![restJquery_6](/assets/images/2014/restJquery_6_thumb_1.png)](/assets/images/2014/restJquery_6_1.png)

Eksikler

Elbette senaryomuzda önemli eksiklikler bulunmaktadır. Örneğin,

- Servis tarafının bir sertifika ile çağırılabileceği durumlarda CORS için nasıl aksiyonlar almak gerekir?
- Son uygulanan pratik, tüm tarayıcılar da çalışmakta mıdır? Örneğin Firefox'ta. Peki ya mobil cihazda bulunan Native kodla geliştirilmiş bir Browser bileşeninde?
- Acaba Windows Forms içerisinde kullanına WebBrowser gibi kontrollerde sonuç nasıl olacaktır?
- Peki HTTP Get ile bir JSON veri kümesi istemci tarafına nasıl çekilebilir?
- Ya veri göndermek için sayfa üzerine konacak kontrollerden nasıl yararlanılabilir?

Bu soruların çözümünü, araştırmasını ve uygulanmasını siz değerli okurlarıma bırakıyorum. Bu mücadeleler inanın size önemli saha tecrübeleri kazandıracaktır. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[HowTo_RestPostJQuery.rar (111,76 kb)](/assets/files/2014/HowTo_RestPostJQuery.rar)