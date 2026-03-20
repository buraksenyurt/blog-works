---
layout: post
title: "Ado.Net Data Services Ders Notları - 1 (Merhaba)"
date: 2008-09-22 12:00:00 +0300
categories:
  - ado-net-data-services
tags:
  - ado-net-data-services
  - bash
  - csharp
  - dotnet
  - aspnet
  - ado-net
  - linq
  - wcf
  - wpf
  - silverlight
  - xml
  - rest
  - json
  - http
  - iis
  - authorization
  - java
  - generics
  - visual-studio
---
Uzun bir aradan sonra yeni bir makale ile daha birlikteyiz. Sağnak yağışlı ve tamda “bu havada bir makale yazılır” dedirten bir günde hazırladığımız bu yazımızda, daha şimdiden gelecek vaat etmiş görünen,.Net Framework 3.5 Service Pack 1 ile birlikte hazır olarak gelen, Visual Studio 2008 ortamına entegre edilen ve WCF mimarisinin en güzel uyarlamalarından birisi olan ADO.Net Data Services (Astoria) üzerinde konuşuyor olacağız.

Bilindiği üzere Windows Communication Foundation (WCF), Microsoft’ un.Net Framework 3.0 ile duyurduğu ve 3.5 ile getirdiği yeni ilavelerle ön plana çıkardığı yeni Service Yönelimli Mimari (Service Oriented Architecture/SOA) yaklaşımıdır. Bu yaklaşımın etkileri güncel projelerde kendini uzun zamandır göstermektedir. SOA yaklaşımlarının ağırlıklı bir biçimde kabul gördüğü günümüz çözümlerinde veri (Data) ile olan ilişkiler göz önüne alındığında Microsoft cephesinde uzun süre önce getirilen yeni bir proje karşımıza çıkmaktadır. Ado.Net Data Services.

Astoria kod adı ile anılan ve İstanbul Mecidiyeköy’ deki alış veriş merkezinin adaşı olan bu mimarinin uygulanış biçimi Visual Studio 2008 Service Pack 1 ile daha da kolay hale gelmiş ve IDE içerisine başarılı bir şekilde entegre edilmiştir. Peki Astoria neler vaat etmektedir ve nasıl bir mimari modele sahiptir? Dilerseniz kısaca bu konulara değinerek yazımızı sabırla okumaya devam edelim.

Öncelikli olarak Ado.Net Data Services bir WCF servis yaklaşımıdır. Bununla birlikte, WCF mimarisine.Net Framework 3.5 ile getirilen yeniliklerden biriside; servislerin, Web Programlama Modeline uygun olacak şekilde yayınlanabilmeleridir. Bu Representational State Transfer[(REST)](http://en.wikipedia.org/wiki/Representational_State_Transfer) modeline uygun servislerin yazılabileceği anlamına gelmektedir.

> Ebellteki Web programlama modelinin (Web Programming Model) WCF tarafına kazandırdığı tek avantaj QueryString bazlı operasyon desteği değildir. Bunun yanında JSON (Java Script Object Notation) formatında yayınlama ve RSS, Atom bazlı Syndication desteğide gelmektedir.

Aslında kafayı çok fazla karıştırmaya gerek yoktur. Aşağıdaki tablo durumu daha net bir şekilde özetlemektedir.

HTTP İşlemi
CRUD Karşılığı

Post
Create, Update, Delete

Get
Read

Put
Create, Overwrite/Replace

Delete
Delete

Bu tabloda anlatılmak istenen şudur; HTTP üzerinden yapılabilecek olan Post, Get, Put, Delete gibi çağrılar tablonun sağ tarafında yer alan veri operasyonlarına dönüştürülebilirler. O halde işin içersine veri kelimesinin girdiği ortadadır ve ne varki Astoria açılımı Ado.Net Data Services olarak geçmektedir. O halde Astoria için, Ado.Net tabanlı verileri REST modelinin belirttiği kriterlere uygun olacak şekilde dışarıya sunan servis mimarisidir tanımlamasını yapmak yerinde olacaktır.

Servis talepleri (Requests) HTTP protokolüne göre QueryString bazlı olmaktadır. Bu talepler servis tarafına ulaştıklarında ise arka planda bir Data Access Layer tarafından karşılanmakta ve operasyonel olarak CRUD (CreateReadUpdateDelete) işlemlerine dönüştürülmektedir. Sonrasında istemciye gönderilecek olan cevaplar XML bazlı olarak ele alınmaktadır. Standart olarak ATOM formatında bir XML çıktısı istemci tarafına gönderilmektedir. Bu tanımlamalar kısaca bir fikir versede mimari detaylara bakmakta yarar vardır. Aşağıdaki şekil Astoria mimarisini kısaca özetlemektedir.

![mk258_1.gif](/assets/images/2008/mk258_1.gif)

Çok kısaca mimari üzerinden konuşarak devam edelim. Öncelikli olarak internet veya intranet üzerinden talepte bulunabilecek bir istemci (Client) uygulama söz konusudur. Bu uygulama standart bir.Net programı olabilir. Örneğin bir Windows/WPF yada basit bir Console uygulaması. Çok doğal olarak istemci başka bir servisde olabilir. Ancak günümüzde Ado.Net Data Service örneklerini kullanacak en popüler istemciler web tabanlı olanlarıdır. Bir başka deyişle Ajax Based Client ve Silverlight gibi uç birimler örnek olarak verilebilirler. İlerleyen kısımlarda Ajax tabanlı bir istemcinin nasıl geliştirileceğine de değinilecektir. İstemci uygulamalar, servise doğru QueryString benzeri formatta bir talepte bulunabilir. Örneğin;

```bash
http://localhost:4501/AdventureServices/ProductService.svc/ProductSubcategory?$orderby=Name desc
```

gibi.

Aslında bu ifade son derece açıktır. Tahmin edileceği üzere HTTP tabanlı olaraktan localhost isimli yerel makinede 4501 numaralı port üzerinden yayın yapan ProductService.svc isimli bir WCF servisi söz konusudur. Servise giden talep ise şunu ifade etmektedir; “Lütfen ProductSubcategory verilerini Name alanlarına göre ters sırada olacak şekilde gönderin”. İşte svc uzantısından sonra gelen ifade basit bir QueryString tanımlamasıdır. Elbetteki arada kullanılan $ işareti ifadeyi Ado.Net Data Services için biraz özelleştirmektedir. Bu noktada talebin servis tarafına ulaştığını düşünebiliriz. Peki servis bu noktadan sonra ne yapmaktadır?

Service tarafında çalışma zamanında (RunTime) devrede olan DataServiceHost (WebServiceHost sınıfından türeyen ki buda WCF Servislerinde çekirdek olan ServiceHost sınıfından türemektedir.) nesne örneği, gelen talebi arka planda ele alır. Bu noktada elde iki seçenek yer almaktadır. Bunlardan birisi yine Visual Studio 2008 Service Pack 1 ile IDE ortamına dahil olan Entity Data Model (EDM) açılımının kullanılmasıdır. Diğeri ise özel LINQ Provider kullanıldığı seçenektir. LINQ Provider kullanımı yardımıyla REST taleplerinin nesneler üzerindede ele alınması sağlanabilmektedir ki bununla ilişkili bir örneği ilerleyen bölümlerde geliştiriyor olacağız.

EDM modeline göre veritabanı bazlı Entity tipler (Types) ve bu tiplere ait üyeler (Members) söz konusudur. Burada temel amaç veritabanı üzerindeki nesnel yapıların OOP (Object Oriented Programming) ortamında karşılıkları olan sınıf (Class) tiplerinde ve üyelerinde (Her alanın karşılığı olan bir özellik ile) ele alabilmektir. Böylece kod ortamından veritabanı üzerine geçiş yapmaya gerek kalmadan CRUD operasyonları kolayca icra edilebilir. EDM açısından olaya baktığımızda sadece yukarıdaki basit okuma (READ) talebi için aşağıdaki şekil biraz daha aydınlatıcı ve fikir verici olabilir.

![mk258_2.gif](/assets/images/2008/mk258_2.gif)

Görüldüğü üzere REST bazlı talep, Ado.Net Data Service Entry Point’ e ulaştıktan sonra servis çalışma zamanı tarafından veritabanına doğru basit sorgular (Queries) şeklinde gönderilirler. Bunun doğal sonucu olarak bazı veri kümeleri elde edilir. Elde edilen sorgu sonuçları EDM içerisinde yer alan Entity nesneleri ve üyeleri tarafından değerlendirilir. Nitekim veritabanı tarafındaki nesnelerin karşılığı olan varlıklar, EDM içerisinde yer almakta olup çalışma zamanında servis operasyonları tarafından ele alınmaktadır. Bir başka deyişle örnek baz alındığında, ProductSubCategory tablosu içerisindeki herhangibir satırın (Table Row) karşılığı olan ProductSubcategory sınıfına ait nesne örneklerinden oluşan bir koleksiyon (Collection) üretimi gerçekleşir. Bu üretim sonrasında ilgili koleksiyon, servis çalışma ortamı tarafından XML çıktısı haline getirilir ve istemciye gönderilir.

Sanıyorumki artık bir örnek geliştirerek konuyu pekiştirmenin ve makalenin yazıldığı bu yağmurlu günde ekranımızda bir güneş açtırmanın zamanı geldi. İlk olarak örneğin Visual Studio 2008 Professional Service Pack 1 üzerinde geliştirildiğini belirtelim.

Ado.Net Data Service'ler esas itibariyle birer WCF servis öğesi olarak tanımlanırlar. Bu sebepten dolayı söz konusu servislerin bir sunucu uygulama üzerinde host edilmeleri gerekmektedir. Burada istenirse bir WCF Service uygulaması baz alınabilir. Yada herhangibir Asp.Net Web Site/Asp.Net Web Application üzerindede bu işlem gerçekleştirilebilir. Web tarafındaki geliştirme kurallarının buradada geçerli olduğunu ve buna göre dosya tabanlı (File-Based) yada doğrudan IIS üzerinde geliştirme yapabileceğimizi hatırlayalım.

Biz örneğimizde AdventureServices adlı WCF Service şablonunu kullanacağız. AdventureServices isimli proje File-Based olarak geliştirilecektir. Servis uygulaması oluşturulduktan sonra ilk yapılması gereken, Ado.Net Data Service'in kullanacağı Data Access Layer ortamını hazırlamaktır. Burada daha öncedende belirtildiği üzere EDM, LINQ Provider seçenekleri mevcuttur. Örneğimizde Entity Data Model kullanılmaktadır. EDM nesnesini eklemek için projeye sağ tıkladıktan sonra aşağıdaki resimde yer alan Ado.Net Entity Data Model şablonunu seçmek yeterli olacaktır.

![mk258_3.gif](/assets/images/2008/mk258_3.gif)

İsim olarak AdventureModel adı kullanılabilir. edmx uzantılı dosya seçimi yapıldıktan sonra bir dizi adımdan oluşan sihirbaz arabirimi ile karşılaşılır.

![mk258_4.gif](/assets/images/2008/mk258_4.gif)

İlk adımda EDM modelinin var olan bir veritabanından oluşturulacağı seçimi yapılır (Generate from database). Ancak istenirse Empty Model kullanılarak, EDM tiplerinin görsel olarak veritabanından bağımsız tasarlanması ve ilgili sınıfları ve üyelerinin oluşturulması sağlanabilir. Bu çoğunlukla Entity tasarımının önceden yapılıp sonrasında veriye bağlama kararının verileceği durumlarda ele alınabilir. Ki bu şekilde oluşturulan Entity nesneleri içerisinde LINQ Provider'lar kullanılarak farklı sağlayıcılara (örneğin XML veya Object tiplerine) doğru eşleştirmelerde gerçekleştirilebilir.

![mk258_5.gif](/assets/images/2008/mk258_5.gif)

İkinci adımda bağlantı (Connection) seçimi yapılır. Buna göre herhangibir veritabanı bağlantısı kullanılabilir. Söz konusu bağlantılara ilişkin bilgiler ise istenirse Web.config dosyası içerisinde saklanabilir.

![mk258_6.gif](/assets/images/2008/mk258_6.gif)

Üçüncü adımda, oluşturulan bağlantı üzerindeki veritabanı içeriği görülür. Burada tablolar (Tables), görünümler (Views) ve saklı yordamlar (Stored Procedures) yer almaktadır. Dolayısıyla bu adımda, EDM içerisindeki tiplerin eş düştüğü veritabanı objeleri işaretlenir.

![mk258_7.gif](/assets/images/2008/mk258_7.gif)

Örnekte yukarıdaki şekildende görüleceği üzere ProductSubCategory ve Product tabloları ele alınmaktadır. Bu tablolar arasında bire-çok (One to many) ilişki olması nedeni ile ilişkisel yapılarıda inceleyebilme fırsatımız olacaktır. Tüm bu işlemler tamamlandıktan sonra aşağıdaki EDM diagramının oluştuğu görülecektir.

![mk258_8.gif](/assets/images/2008/mk258_8.gif)

Dikkat edilecek olursa ProductSubCategory ve Product tablolarının kendileri birer sınıf olarak oluşturulmuş, alanları birer üye olarak ilave edilmiştir. Bunlara ek olarak her iki tablo arasındaki ilişki (Relation), EDM içerisinde bir Association olarak tanımlanmıştır. Dikkat çekici özelliklerden bir diğeri ise sınıflara ait nesne örnekleri üzerinden birbirlerine geçiş yapılmasını sağlayacak özelliklerin (Properties) eklenmiş olmasıdır. Söz gelimi bir alt kategoriye bağlı ürünleri elde etmek için ProductSubcategory tipine ait nesne örneği üzerinden Product özelliği kullanılabilir. Oluşturulan bu sınıflar ve eş düştükleri veritabanı objeleri arasındaki ilişkiler istenirse Model Browser aracılığıyla aşağıdaki şekilde görüldüğü gibide izlenebilir.

![mk258_9.gif](/assets/images/2008/mk258_9.gif)

Örnekteki Model Browser görselinde, AdventureWorksModel kısmında EDM içeriği haritalanmaktadır. Diğer taraftan AdventureWorksModel.Store boğumu (Node) altında ise, EDM içeriğinin karşılıkları olan veritabanı unsurları listelenmektedir. AdventureModel.Designer.cs dosyasına bakıldığında ise 3 adet sınıfın (Class) oluşturulduğu görülür.

![mk258_10.gif](/assets/images/2008/mk258_10.gif)

Dikkat edileceği üzere Product ve ProductSubcategory tipleri dışında AdventureWorksEntities isimli bir tipin daha olduğu görülmektedir ki üyeleri aşağıdaki şekilde olduğu gibidir.

![mk258_11.gif](/assets/images/2008/mk258_11.gif)

AdventureWorksEntites, ObjectContext sınıfından türemiştir ve Entity koleksiyonlarının yönetimi, EDM nesnelerinin eş düştüğü veri objeleri ile olan fonksiyonelliklerin ele alınması gibi kritik görevleride üstlenmek üzere tasarlanmıştır. Öyleki, Product ve ProductSubcategory nesne toplulukları bu sınıf içerisinde aşağıdaki kod parçasında olduğu gibi tutulmaktadır.

```csharp
public global::System.Data.Objects.ObjectQuery<Product> Product
{
    get
    {
        if ((this._Product == null))
        {
            this._Product = base.CreateQuery<Product>("[Product]");
        }
        return this._Product;
    }
}
private global::System.Data.Objects.ObjectQuery<Product> _Product;

public global::System.Data.Objects.ObjectQuery<ProductSubcategory> ProductSubcategory
{
    get
    {
        if ((this._ProductSubcategory == null))
        {
            this._ProductSubcategory = base.CreateQuery<ProductSubcategory>("[ProductSubcategory]");
        }
        return this._ProductSubcategory;
    }
}
private global::System.Data.Objects.ObjectQuery<ProductSubcategory> _ProductSubcategory;
```

ObjectQuery tipinden birer readonly özellik (Property) yardımıyla! Ve yine bir ürünün veya alt kategorinin eklenmesi için AddToProduct ve AddToProductSubcategory isimli metodlarda yer almaktadır. Bu noktada akla şöyle bir soru gelebilir. Update, Delete işlemleri için niye metodlar bulunmamaktadır? Söz konusu sorunun cevabı ilerleyen bölümlerde verilecektir ve bu nedenle sizlere biraz düşünme ve araştırma süresi kalmaktadır. Bu detayları şimdilik geride bırakarak asıl konumuza geri dönmenin yararlı olacağı kanısındayım. Artık WCF Service uygulamasına bir Ado.Net Data Service eklenebilir. Tek yapılması gereken projeye Add New Item seçeneği ile aşağıdaki şekildede görülen Ado.Net Data Service öğesini eklemektir.

![mk258_13.gif](/assets/images/2008/mk258_13.gif)

Bu işlemin ardından proje şablounan ProductService.svc servis ve ProductService.cs code-behind dosyaları eklenecektir. (Söz konusu işlemlerde biz WCF geliştiricilerini şaşırtan herhangibir nokta bulunmamaktadır. Nitekim Web üzerinden host edilen bir WCF serviside aynı prensiplerde oluşturulmaktadır. Bir svc içeriği ve çoğunlukla code-behind üzerinde tutulan kod içeriği.) ProductService.cs içeriği kısaca incelendiğinde bir başlatma işleminin (Initialization) yapılması gerektiği görülmektedir. Nitekim servis nesnesi örneklendiğinde EDM içerisindeki hangi tiplerin hangi şartlarda yayınlanacağının belirlenmesi gerekmektedir. Bu bir anlamda yetkilendirme süreci olarakta düşünülebilir. Söz konusu ProductService.cs içeriği örnek için aşağıdaki gibi değiştirilmelidir.

```csharp
using System;
using System.Data.Services;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using AdventureWorksModel;

public class ProductService 
    : DataService<AdventureWorksEntities>
{
    public static void InitializeService(IDataServiceConfiguration config)
    {
        config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
    }    
}
```

Burada dikkat edilmesi gereken noktalardan birisi ProductService sınıfının System.Data.Services isim alanında (Namesapce) yer alan DataService generic sınıfından türemiş olmasıdır. Bunun dışından şu an için önem arz eden nokta static InitializeService metodudur. Bu metod, servis örneği ilk oluşturulduğunda bir kereliğine devreye girer. config değişkeni üzerinden yapılan çağrı ise önemlidir. SetEntitySetAccessRule metoduna gönderilen ilk parametrede sembölü kullanılarak, Entity set içerisindeki tüm tiplerin ele alınacağı belirtilmektedir. İkinci parametre ise EntitySetRights enum sabiti tipinden olup aşağıdaki değerleri alabilir.

![mk258_12.gif](/assets/images/2008/mk258_12.gif)

Burada AllRead'in anlamı tüm Entity nesneleri üzerinde her çeşit veri okuma (Read) işleminin yapılabileceğidir. Bir başka deyişle EntitySetRights enum sabitinin değerleri ile, hangi Entity objelerine hangi haklarla erişebileceği belirtilmektedir. Söz gelimi aşağıdaki kod örneğini ele alalım.

```csharp
config.SetEntitySetAccessRule("Product", EntitySetRights.AllRead);
config.SetEntitySetAccessRule("ProductSubcategory", EntitySetRights.AllWrite);
```

Bu ifadelere göre Product nesneleri için sadece okuma işlemi yapılabilirken, ProductSubcategory nesneleri içinde sadece yazma işlemleri yapılabilmektedir. Artık herhangibir istemci yazmadan, ProductService.svc servisi test edilebilir. Nitekim hepimizin iştahının kabarmış olduğunu ve bir an önce sonuçları görmek istediğinizi hissetmekteyim. Öyleyse gelin F5 ile projemizi çalıştıralım. Uygulama ilk çalıştırıldığında ProductService.svc dosyası tarayıcı pencere içerisinde aşağıdaki gibi görünecektir.

![mk258_14.gif](/assets/images/2008/mk258_14.gif)

Buradan çıkartılması gereken ilk sonuç Product ve ProductSubcategory elementleri için taleplerde bulunulabileceğidir. Öyleyse test sorgularına başlanabilir. Sorgulardan kastımız elbetteki URL satırına girilen QueryString ifadeleri ve bunların ATOM tabanlı XML çıktılarının nasıl olacağıdır.

Örnek 1;
Tüm ProductSubcategory satırlarının elde edilmesi

URL Satırı ifadesi:
http://localhost:3030/AdventureServices/ProductService.svc/ProductSubcategory
(Burada hemen bir hatırlatma yapalım. Eğer URL satırında ProductSubcategory yerine ProductSubCategory yazılırsa Case-Sensitive özelliğinden dolayı HTTP 404 Not Found çıktısı alınır. Dolayısıyla QueryString'leri yazarken Case-Sensitive olmalarına dikkat etmek gerekir.)

Sonuç Ekran görüntüsü;

![mk258_15.gif](/assets/images/2008/mk258_15.gif)

Örnek 2: 3 Numaralı ProductSubcategory bilgisinin elde edilmesi

URL Satırı ifadesi:
http://localhost:3030/AdventureServices/ProductService.svc/ProductSubcategory (3)

Sonuç Ekran görüntüsü;

![mk258_16.gif](/assets/images/2008/mk258_16.gif)

Not: İkinci örnekte dikkat edilmesi gereken bir nokta vardır. URL satırında parantez içerisinde 3 değeri yazılarak ProductSubCategoryID değeri 3 olan alt kategori bilgisi elde edilmiştir. Pekiya servis tarafından nasıl olmaktadırda, parantez içerisindeki değerin ProductSubCategoryID alanına işaret ettiği bilinmektedir. Burada anahtar nokta ProductSubcategory sınıfındaki ProductSubCategoryID özelliğidir.
![mk258_17.gif](/assets/images/2008/mk258_17.gif)
Şekildende görüleceği üzere EdmScalarPropertyAttribute niteliğinin içerisinde EntityKeyProperty özelliğine true değeri verilmiştir. Böylece çalışma zamanı, parantez içerisinde gelen ifadenin bu özelliğe ait olduğunu bilmektedir.

Örnek 3: Product tablosundan ListPrice değeri 3500 birim üzerinde olanların elde edilmesi

URL Satırı ifadesi:
http://localhost:3030/AdventureServices/ProductService.svc/Product?$filter=ListPrice gt 3500
(Burada gt=grater then anlamındadır.Buna göre küçüktür için lt=less then)

Sonuç Ekran görüntüsü;

![mk258_18.gif](/assets/images/2008/mk258_18.gif)

Örnek 4: ProductSubcategoryID değeri 1 olan alt kategoriye bağlı ürünlerin listesinin elde edilmesi

URL Satırı ifadesi:
http://localhost:3030/AdventureServices/ProductService.svc/ProductSubcategory (1)/Product

Sonuç Ekran görüntüsü;

![mk258_19.gif](/assets/images/2008/mk258_19.gif)

Örnek 5: Product tablosundan ListPrice değeri 3500 birim üzerinde olanların isimlerine göre ters sırada elde edilmesi

URL Satırı ifadesi:
http://localhost:3030/AdventureServices/ProductService.svc/Product?$filter=ListPrice gt 3500&$orderby Name desc
(İki ayrı sorgu ifadesi birleştirilirken & kullanılır)

Sonuç Ekran görüntüsü;

![mk258_20.gif](/assets/images/2008/mk258_20.gif)

Örnek 6: ProductSubCategoryID değeri 4 olan alt kategorinin bilgilerinin elde edilmesi ve buna bağlı ürünlerinde getirilmesi

URL Satırı ifadesi:
http://localhost:3030/AdventureServices/ProductService.svc/ProductSubcategory (4)?$expand=Product

Sonuç Ekran görüntüsü;
Dikkat edileceği üzere inline elementi altında 4 numaralı alt kategoriyi bağlı Product örneklerinin entry boğumları yer almaktadır. Service tarafı göz önüne alındığında expand komutunun kullanılabilmesini sağlayan üyenin ProductSubcategory sınıfındaki Product özelliği olduğuna dikkat etmek gerekir.

![mk258_21.gif](/assets/images/2008/mk258_21.gif)

Buraya kadar anlatıklarımız umarım sizlere Ado.Net Data Services hakkında biraz fikir verebilmiştir. Eğer buraya kadar makaleyi zevkle okuduysanız işte size yapmanız gereken bir kaç ödev. Öncelikli olarak tarayıcı uygulama üzerinden sorgu gönderdiğinizde SQL tarafında nasıl komutlar çalıştırıldığına bakmanızı öneririm. Kod tarafında ilgili noktalara breakpoint'ler ekleyerek nesnelerin ne zaman örneklendiklerine (REST sorgusu SQL sorgusuna dönüştürülmeden öncemi, sonra mı gibi) bakmanızı öneririm. Başka ne çeşit sorgular yazabileceğinizi filter, orderby, expand, () dışında ne gibi query komutları olabileceğini araştırın. Bu araştırmalarıda başarı ile yaparsanız şöyle güzel bir sütlü nescafe'yi deniz kenarında yudumlamayı hak etmişsiniz demektir, üstelik güneş batarken.

Böylece geldik bir makalemizin daha sonuna. Bu makalemizde kısaca Ado.Net Data Services (Astoria) konusuna değinmeye çalıştık. Özet olarak, ADO.Net EDM (Entity Data Model) veya LINQ Provider seçeneklerini kullanaraktan, verilerin REST (REpresentational State Transfer) modele uygun bir servis üzerinden yayınlanabileceğini gördük. Bu makalemizde herhangibir istemci uygulama geliştirmemiş olmamıza rağmen, sonuçları değerlendirmek adına bir tarayıcı uygulama kullandığımızı unutmayalım. Nitekim tarayıcı uygulamalarda hangi çeşitten olurlarsa olsunlar potansiyel olarak birer servis istemcisidir. Elbette ilerleyen makalelerimizde istemci uygulamaların nasıl geliştirilebileceğine de değinebileceğimizi belirtmek isterim. Ado.Net Data Services ile ilişkili blog bilgilerine Microsoft'un [şu](http://blogs.msdn.com/astoriateam/) adresinden ulaşabilirsiniz. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örneği indirmek için tıklayın](/assets/files/2008/AstoriaHelloWorld.rar)