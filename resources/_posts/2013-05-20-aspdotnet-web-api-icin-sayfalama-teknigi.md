---
layout: post
title: "Asp.Net Web API için Sayfalama Tekniği"
date: 2013-05-20 06:11:00 +0300
description: "Asp.Net Web API alt yapısının popüler olmasının ardında yatan en büyük sebeplerden birisi, HTTP tabanlı servis yayılımına izin vermesidir. Hemen her fonksiyonel birimin veya bütünlüğün servis odaklı teknolojiler ile ele alındığı ve istemcilere sunulduğu bir dünyada, bu ihtiyacı eskiden beri var olan HTTP protokolünün Post, Put, Get, Delete gibi standart metodlarına göre karşılamak elbette önemlidir. Bu sayede Microsoft tabanlı olarak geliştirilen Web API servislerinin, dış dünyadaki herhangibir Client tarafından tüketilmesi de oldukça kolaydır. Üstelik OData(Open Data Protocol) desteği sayesinde, veri odaklı servislerin standart URL bazlı parametreler ile sorgulanabilmesi mümkün hale gelmektedir."
categories:
  - aspnet-web-api
tags:
  - aspnet-web-api
  - csharp
  - dotnet
  - aspnet
  - ado-net
  - entity-framework
  - linq
  - sql-server
  - t-sql
  - web-api
  - http
  - javascript
  - visual-studio
---
# Asp.Net Web API için Sayfalama Tekniği
[![lazy-baby-laptop](/assets/images/2013/lazy-baby-laptop_thumb.jpg)](/assets/images/2013/lazy-baby-laptop.jpg) Merhaba Arkadaşlar,

Bu aralar şirkette işler oldukça kesat. En azından benim bulunduğum departman itibariyle böyle bir durum söz konusu. Sanırım kurumsal kimlik kazanmış firmaların genel sorunu da bu olsa gerek. Kaynak planlaması ve dağıtımının bir türlü istenen şekilde yapılamayışı. Hal böyle olunca aynı firmada hatta aynı departman içerisinde, çok yoğun çalışan insanlara ve beraberinde her hangi bir işi olmayanlara (benim gibi) rastlamak mümkün.

Böyle bir durumda keyif sürmek ve tembel tembel internette gezmek (Video paylaşmak, onun bunun ciklemesine yetişmeye çalışmak vb) yapılabilecek en cazip işlerden birisi gibi gözükse de, hızla ilerleyen teknoloji ne yazık ki buna müsade etmemekte. Neredeyse her hafta yeni konuların ele alındığı bilmem kaç katlık [yottabyte](http://en.wikipedia.org/wiki/Yottabyte)’ lık bilgi denizinde sürekli bir şeyler öğrenmek zorunda olan biz köle geliştiricilerin iş olmasa da kendisine iş yaratması şart. Eğitim şart da diyebiliriz

![Smile](/assets/images/2013/wlEmoticon-smile_97.png)

Öyleyse tembel tembel oturmayalım ve gelin birlikte yeni bir mevzuya dalalım.

Asp.Net Web API alt yapısının popüler olmasının ardında yatan en büyük sebeplerden birisi, HTTP tabanlı servis yayılımına izin vermesidir. Hemen her fonksiyonel birimin veya bütünlüğün servis odaklı teknolojiler ile ele alındığı ve istemcilere sunulduğu bir dünyada, bu ihtiyacı eskiden beri var olan HTTP protokolünün Post, Put, Get, Delete gibi standart metodlarına göre karşılamak elbette önemlidir. Bu sayede Microsoft tabanlı olarak geliştirilen Web API servislerinin, dış dünyadaki herhangibir Client tarafından tüketilmesi de oldukça kolaydır. Üstelik OData (Open Data Protocol) desteği sayesinde, veri odaklı servislerin standart URL bazlı parametreler ile sorgulanabilmesi mümkün hale gelmektedir.

Bir önceki cümlede belirttiğimiz veri odaklı servis (Data-Centric Service) aslında bu yazımızdaki senaryomuzun da ana konularındandır. Veri odaklı servisler tahmin edileceği üzere büyük boyutlu verileri de ele alabilirler. Çok eskilerden de bildiğimiz üzere Asp.Net ile web programlamanın ilk yıllarında yaşanan en büyük sıkıntılardan birisi, verinin sayfalanarak getirilmesiydi. Stored Procedure’ ler de yapılan bazı hamleler ile bu işi çözebiliyor olsak da, SP desteği olmayan bir veri kaynağının kullanılma olasılığını da göz ardı etmemek gerekiyor.

> Hatırlayacağınız gibi web tarafındaki ilk veri bağlı kontrollerde sayfalama sistemi şöyle çalışmaktaydı:
> Sorgulama sırasında tüm veri çekilir ve içinden örneğin Xnci sıradaki 10 luk eleman kümesi getirilirdi. Çok doğal olarak her seferinde tüm veri kümesinin çekilmesi çok da istenen bir yaklaşım değildi. Örneğin 10 milyon satır içinden 3ncü 50lik kümeyi getirmek istediğimizde
>
> ![Confused smile](/assets/images/2013/wlEmoticon-confusedsmile_31.png)
> Ancak ilerleyen sürümler de bu durum değişti ve özellikle SQL tarafında row_number kullanımı ile doğru sayfalama işlemlerinin yapılabilmesinin yolu açıldı. Buna bir de LINQ tarafındaki anahtar kelime desteği eklenince, Entity Framework gibi alanlarda doğru sayfalama stratejilerini kullanabilir olduk.

Peki Asp.Net Web API tarafında sayfalama işlevselliği nasıl karşılanabilir? İşte bu yazımızda cevap bulmaya çalışacağımız soru bu. Şimdi basit bebek adımları ile ilerleyerek senaryomuzu hayata geçirmeye çalışalım.

Proje için Ön Hazırlıklar

İlk olarak Visual Studio 2012 ortamında Empty MVC 4 şablonunda bir web uygulaması açarak yola koyulabiliriz.

[![wapip_1](/assets/images/2013/wapip_1_thumb.png)](/assets/images/2013/wapip_1.png)

Söz konusu senaryomuzda istemci tarafında yazacağımız kodlar oldukça önemlidir. Bu yüzden jQuery ve Knockout.js’ in son sürümlerinin kullanılmasında yarar vardır. Ayrıca OData sorgularını kullanacağımız için Microsoft ASP.NET Web API OData paketini de eklememiz gerekmektedir. Bu referansları NuGet paket yönetim aracı ile projeye kolayca dahil edebiliriz.

jQuery

[![wapip_3](/assets/images/2013/wapip_3_thumb.png)](/assets/images/2013/wapip_3.png)

knockout.js

[![wapip_4](/assets/images/2013/wapip_4_thumb.png)](/assets/images/2013/wapip_4.png)

ve Microsoft ASP.NET Web API OData

[![wapip_5](/assets/images/2013/wapip_5_thumb.png)](/assets/images/2013/wapip_5.png)

Bu eklentilerin yüklenmesi sonrasında jQuery ve knockout.js için scripts klasörü aşağıdaki hale gelecektir.

[![wapip_6](/assets/images/2013/wapip_6_thumb.png)](/assets/images/2013/wapip_6.png)

Buradaki Javascript kütüphaneleri cshtml tarafında kullanılacaktır.

Veri Modelinin Eklenmesi

Çok doğal olarak senaryomuzda veri odaklı bir uygulama öngörülmektedir. Örneğimizde Entity Framework’ den yararlanılabilir. Bu nedenle Model klasörüne yeni bir Ado.Net Entity Data Model öğesi ekleyelim. Örneğimizde kobay veritabanlarımızdan birisi olan Chinook’ a bağlanıyor olacağız. Bu veritabanın yer alan Invoice tablosu 400 satırdan fazla veri içermekte ve senaryomuz için ideal bir veri kümesi sunmaktadır. Bu sebpeten sadece ilgili tabloyu kullansak yeterli olacaktır.

[![wapip_2](/assets/images/2013/wapip_2_thumb.png)](/assets/images/2013/wapip_2.png)

Controller Eklenmesi

Pek tabi veri modelinin oluşturulmasının ardından bir de Controller’ ın eklenmesi gerekmektedir. Model ile View arasındaki köprüyü kuracak olan Controller sınıfının temel özelliklerini aşağıdaki gibi belirleyebiliriz.

[![wapip_7](/assets/images/2013/wapip_7_thumb.png)](/assets/images/2013/wapip_7.png)

Sınıf içeriği bizim için otomatik olarak üretilecektir. Ancak senaryomuz için gerekli olmayan detayları çıkartabiliriz. Buna göre içeriği aşağıdaki kod parçasında görüldüğü gibi değiştirmemiz yeterli olacaktır.

```csharp
using System.Web.Http; 
using MvcApplication5.Models;

namespace MvcApplication5.Controllers 
{ 
    public class InvoicesController : ApiController 
    { 
        private ChinookEntities db = new ChinookEntities();

        // GET api/Invoices 
        [Queryable] 
        public IQueryable<Invoice> GetInvoices() 
       { 
            return db.Invoices.AsQueryable(); 
        } 
        
        protected override void Dispose(bool disposing) 
        { 
            db.Dispose(); 
            base.Dispose(disposing); 
        } 
    } 
}
```

ApiController türevli InvoicesController sınıfı içerisinde yer alan GetInvoices metodu, Queryable tipi ile nitelendirilmiştir. Bu nitelik (Attribute) sayesinde OData sorgu desteği sağlanmış olmaktadır ki bu, sayfalama için kullanılacak olan top,skip ve orderby komutları için gereklidir.

Görsel Taraf (İstemci) için Controller Eklenmesi

ApiController bilindiği üzere Web API servis desteği için gereklidir. Ancak istemci tarafını düşündüğümüzde standart bir MVC Controller’ ının kullanılması gerekecektir. Nitekim cshtml içeriğini kullanarak Web API servisi üzerinden OData anahtar kelimeleri ile sayfalama talebinin gönderilip, sonuçların gösterileceği bir View öğesi gerekmektedir. Bunun için projeye yeni bir MVC 4 Controller ekleyerek ilerleyebiliriz.

[![wapip_8](/assets/images/2013/wapip_8_thumb.png)](/assets/images/2013/wapip_8.png)

Bu işlem sonucunda aşağıdaki sınıf içeriği üretilmiş olacaktır.

```csharp
using System.Web.Mvc;

namespace MvcApplication5.Controllers 
{ 
    public class InvoiceListController 
        : Controller 
    { 
        // 
        // GET: /InvoiceList/

        public ActionResult Index() 
        { 
            return View(); 
        } 
    } 
}
```

## View Öğesinin Eklenmesi (index.cshtml)

Tahmin edileceği üzere bir de View öğesine ihtiyacımız bulunmakta. Bu amaçla Views klasörü içerisinde InvoicesList isimli bir alt klasör açarak içerisine yeni bir View ilave edip devam edebiliriz. (View’ un adını index olarak belirleyip Razor Engine’ i kullanacak şekilde tesis edelim)

İçeriği ise aşağıdaki şekilde düzenleyelim.

```text
@{ 
    ViewBag.Title = "Index"; 
}

<h2>Invoice List</h2>

<script src="~/Scripts/jquery-2.0.0.min.js"></script> 
<script src="~/Scripts/knockout-2.2.1.js"></script>

<div> 
    Gösterilmek istenen satır sayısı<br /> 
    <input type="text" id="txtRowSize" /> 
    <br /> 
    Başlangıç Noktası<br /> 
    <input type="text" id="txtRowIndex" /> 
    <br /> 
  <input type="button" id="btnGetInvoices" value="Get Inovices" data-bind="click: InvoiceModel.GetInvoices"/> 
</div>

<table border="1">

<thead> 
  <tr> 
   <th>InvoiceId</th> 
   <th>CustomerId</th> 
   <th>InvoiceDate</th> 
   <th>BillingAddress</th> 
   <th>BillingCity</th> 
   <th>BillingState</th> 
   <th>BillingCountry</th> 
   <th>BillingPostalCode</th> 
   <th>Total</th> 
  </tr> 
</thead>

<tbody data-bind="template: { name: 'InvoiceDataModel', foreach: InvoiceModel.Invoices }"> 
</tbody> 
</table> 
<script type="text/html" id="InvoiceDataModel"> 
<tr> 
  <td> 
   <span style="width:100px;"  data-bind="text: $data.InvoiceId" /> 
  </td> 
  <td> 
   <span style="width:100px;"  data-bind="text: $data.CustomerId" /> 
  </td> 
  <td> 
   <span style="width:100px;"  data-bind="text: $data.InvoiceDate" /> 
  </td> 
  <td> 
   <span style="width:100px;" data-bind="text: $data.BillingAddress"  /> 
  </td> 
  <td> 
   <span style="width:100px;"  data-bind="text: $data.BillingCity" /> 
  </td> 
      <td> 
   <span style="width:100px;"  data-bind="text: $data.BillingState" /> 
  </td> 
    <td> 
   <span style="width:100px;"  data-bind="text: $data.BillingCountry" /> 
  </td> 
    <td> 
   <span style="width:100px;"  data-bind="text: $data.BillingPostalCode" /> 
  </td> 
    <td> 
   <span style="width:100px;"  data-bind="text: $data.Total" /> 
  </td> 
</tr> 
</script>

<script type="text/javascript">

var InvoiceModel = { 
  Invoices:ko.observableArray([]) 
};

InvoiceModel.GetInvoices= function () 
{ 
    InvoiceModel.Invoices([]);

  var rowSize = $("#txtRowSize").val(); 
  var rowIndex = $("#txtRowIndex").val();

  
  var url = "/api/Invoices?$top=" + rowSize + '&$skip=' + (rowIndex * rowSize) + '&$orderby=InvoiceId'; 
  
  $.ajax({ 
   type: "GET", 
   url: url, 
   success: function (data) 
   { 
    InvoiceModel.Invoices(data); 
   }, 
   error: function (err) 
   { 
    alert(err.status + "," + err.statusCode); 
   } 
  }); 
}; 
ko.applyBindings(InvoiceModel); 
</script>
```

Kısa da olsa neler yaptığımıza bir bakalım. En önemli kısım tabi ki btnGetInvoices isimli düğmeye basıldıktan sonra çalışan kod içeriğidir. Burada kilit nokra url değişkenine atanan ifadedir. Dikkat edileceği üzere burada bir OData sorgusu oluşturulmakta ve top ile skip komutlarından yararlanılarak bir veri çekme işlemi gerçekleştirilmektedir. Elde edilen sonuç kümesinin ilgili veri kontrolüne bağlanması noktasında ise bir Ajax çağrısı söz konusudur. success bloğunda sorgu sonuçlarının ilgili veri kontrolüne doldurulması işlemi icra edilmektedir.

Route Ayarları

Testlere başlamadan önce InvoiceList görünümü için Route ayarlarını güncellememizde yarar vardır. Bunun için App_Start klasöründe yer alan RouteConfig.cs içeriğini aşağıdaki gibi değiştirelim.

```csharp
using System.Web.Mvc; 
using System.Web.Routing;

namespace MvcApplication5 
{ 
    public class RouteConfig 
    { 
        public static void RegisterRoutes(RouteCollection routes) 
        { 
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute( 
                name: "Default", 
                url: "{controller}/{action}/{id}", 
                defaults: new { controller = "InvoiceList", action = "Index", id = UrlParameter.Optional } 
            ); 
        } 
    } 
}
```

Buna göre uygulamamızı çalıştırdığımızda varsayılan olarak InoviceList ile ilişkili View’ a gidilecektir.

Test

İlk olarak Web API servisinin çalıştığından emin olmalıyız. Bu amaçla URL satırına [http://localhost:46672/api/Invoices](http://localhost:46672/api/Invoices) benzer bir ifade girildiğinde, aşağıdaki ekran görüntüsündekine benzer bir içeriğin üretilmiş olması gerekmektedir.

[![wapip_9](/assets/images/2013/wapip_9_thumb.png)](/assets/images/2013/wapip_9.png)

Eğer aşağıya doğru inerseniz tüm Invoice içeriğinin çekildiğini görebilirsiniz.

Ne var ki, Web API servisimiz için test noktasında önem arz eden bir mevzuda top, skip ve orderby komutlarına cevap verebiliyor olmasıdır. Örneğin [http://localhost:46672/api/invoices?$top=3&$skip=10&$orderby=InvoiceId](http://localhost:46672/api/invoices?$top=3&$skip=10&$orderby=InvoiceId) şeklinde bir talep girdiğimizi düşünelim. Aslında bu talep ile servis tarafına şu mesajı iletmiş oluyoruz;

Önce Invoice satırlarını InvoiceId değerine göre bir diz bakalım. Sonra da10ncu indisten itibaren bana ilk 3 sıradakini getir.

Talebin işlenmesi sonrası tarayıcı üzerinde aşağıdakine benzer bir sonuç elde etmemiz gerekmektedir.

[![wapip_10](/assets/images/2013/wapip_10_thumb.png)](/assets/images/2013/wapip_10.png)

Şimdi asıl View içeriğini test ederek asıl senaryomuzu yürütebiliriz. Uygulamamızı varsayılan olarak çalıştırdığımızda Route tanımlaması nedeniyle doğrudan index.cshtml içeriğini görüyor oluruz.

[![wapip_11](/assets/images/2013/wapip_11_thumb.png)](/assets/images/2013/wapip_11.png)

Şimdi bazı veriler girerek örneğimizi test edelim.

[![wapip_12](/assets/images/2013/wapip_12_thumb.png)](/assets/images/2013/wapip_12.png)

Dikkat edileceği üzere 51 numaralı InvoiceId değerinden itibaren 5 adet satır getirilmesi istenmiş ve buna göre bir sonuç kümesi elde edilmiştir. Tabi burada önemli olan bir diğer nokta da fonksiyonun icra edilmesi sırasında SQL tarafında çalıştırılan sorgu ifadesidir. Özellikle bu sorgu ifadesinde doğru bir sayfalama yapılması da çok önemlidir. Bu amaçla SQL Server Profiler aracından yararlanabiliriz. Sonuç itibariyle aşağıdakine benzer bir T-SQL sorgusunun çalıştırılmış olduğunu görürüz.

[![wapip_13](/assets/images/2013/wapip_13_thumb.png)](/assets/images/2013/wapip_13.png)

Dikkat edileceği üzere rownumber komutundan yararlanılarak gerçek anlamda sayfalama işlemi uygulanmıştır.

Sonuç

Görüldüğü üzere OData sorgu desteği sunan Asp.Net Web API servislerini kullanarak sayfalama işlemlerini gerçekleştirmek oldukça kolaydır. Bu iş de başrol oyuncu olan top, skip ve orderby anahtar kelimeleri bir OData standardı olduğundan, istemci tarafı Microsoft dışı bir platform da olabilir. Tabi burada tek bağlayıcı nokta SQL veritabanı ve Entity Framework kullanımıdır. Farklı veri kaynaklarında rownumber gibi bir kullanım şekli söz konusu olmayabilir. Böyle bir vaka da tahmin edileceği üzere Web API Controller içerisindeki ilgili operasyon noktalarında müdahale de bulunmak gerekebilir (Araştırmadım benim yerime siz bu işi yapın ![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_204.png)) Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.