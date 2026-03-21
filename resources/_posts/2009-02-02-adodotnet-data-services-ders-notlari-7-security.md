---
layout: post
title: "Ado.Net Data Services Ders Notları - 7 (Security)"
date: 2009-02-02 12:00:00 +0300
categories:
  - ado-net-data-services
tags:
  - ado.net-data-services
  - wcf-data-services
  - windows-communication-foundation
---
Yazılım dünyasının en önemli zorluklarından biriside uygulamanın kapsamına göre güvenliğin etkili bir şekilde nasıl sağlanacağı ile ilişkilidir. Burada hassas bilgilerin korunması, kullanıcıların tanınması ve yetkilendirilmesi, kodun erişim ilkelerinin belirlenmesi, verinin şifrelenmesi gibi pek çok faktör söz konusudur. Genel anlamda günvelik farklı şekillerde göz önüne alınabilir.

- Kimi zaman uygulama içerisinde kullanılan parametrik dış ortam değişkenlerini korumak gerekir. Örneğin uygulamanın kullandığı değişkenlerin konfigurasyon (app.config, web.config gibi) dosyasında şifrelenerek saklanması önemlidir ki bu pek çok uygulama standardınında ilkeleri arasında yer almaktadır.
- Kimi zaman uygulamanın içerisindeki kodların ne tür işlemler yapabileceğinin belirlenmesi (Kode Erişim Güvenliği-Code Access Security) önemlidir. Örneğin uygulamanın, kurulduğu sistem üzerinde dosya yazma yetkisi olmamasının sağlanması, yada sadece dosya okuma yapmasına izin verilmesi veya uygulama içerisinden ağ ortamına bağlantıya izin verilmemesi gibi.
- Kimi zaman uygulamayı açan kişilerin doğrulanması ve yapabileceklerinin sınırlandırılması gerekir (Authentication/Authorization). Örneğin uygulamayı açma yetkisi olan bir kullanıcının sahip olduğu role göre her menü seçeneğini kullanamaması gibi.

Vakalar ve gereklilikler çoğaltılabilir. Tek bir makine üzerinde kendi başına çalışan uygulamalar için güvenliğin sağlanması nispeten daha kolaydır. Ancak istemci/sunucu (Client/Server) bazlı mimariye geçildiğinde güvenliği sağlamak her zamankinden dahada zor bir hal almaktadır. Bunun en büyük nedenlerinden birisi farklı ortamlar arasında verinin, çeşitli protokollere göre mesajlar üzerinden transfer edilmesi gerekliliği ve bu nedenle iletişiminde güvenli hale getirilmesinin zorluğudur. Öyleki, mesajların şifrelenmesi, iletişim kanalının güvenli hale getirilmesi, aradaki mesajların yakalanması ve değiştirilmesi ihtimaline karşılık gerekli tedbirlerin alınması gibi kıstaslar söz konusudur. Yine durum istemci ve sunucu tarafındaki uygulamaların belirli olmaları halinde biraz daha kolay bir şekilde ele alınabilir. Oysaki sunucu tarafında bir servis uygulamasının bulunması ve buna herhangibir istemcinin bağlanabilecek olması gibi durumlarda ulusal bir takım güvenlik ilkelerine uygun olacak şekilde iletişimi sağlamak ve mesajlaşmak gerekmektedir.

Web servisleri göz önüne alındığında bu tip güvenlik konularını kolay bir şekilde tesis etmek adına WSE (Web Service Enhancements) alt yapısından yararlanılmaktadır..Net Remoting tabanlı dağıtık çözümlerde sorumluluk neredeyse geliştiricinin kendisine aittir. Ancak Windows Communication Foundation uygulamaları göz önüne alındığında güvenlik, daha kolay ve etkili bir şekilde iletişim (Transport) veya mesaj (Message) seviyesinde ele alınabilmektedir ki bu kriterler şu anda konumuz dışındadır:)

Ado.Net Data Service'lerde temel olarak birer WCF servisidir. Bununla birlikte söz konusu servisler bilindiği üzere istemcilere RESTful modele göre hizmet vermektedir. Yani HTTP protokolünün GET,HEAD,DELETE,PUT gibi metodlarını ele alıp ATOM,XML,JSON gibi standartları kullanmaktadır. Basit bir servis olarak göz önüne alındığında güvenlik konusunda henüz yeteri kadar gelişmiş olmadığı düşünülebilir; mi acaba? İşte bu makalemizde daha çok bu soruya cevap bulmaya çalışacağız.

Her şeyden önce en önemli nokta Ado.Net Data Service'lerin veri kaynaklarını istemciye RESTful modeline göre sunmasıdır. Bu açıdan bakıldığında geliştiricilerin daha çok üzerinde duracağı nokta verinin erişilebilirliğinin güvenli hale getirilmesidir. Dolayısıyla Ado.Net Data Service'leri kullanan istemcilerin, servis tarafında bir şekilde doğrulanması (Authenticate) ve sonrasında durumlarına bakılarak yetkilendirilmeleri (Authorization) güvenliğin sağlanması adına önemlidir. Oysaki Ado.Net Data Service örnekleri, aslında herhangibir uygulama üzerinde host edilebilecek şekilde kullanılabilir. WCF kadar geniş bir konsepti yoktur. Bu nedenle kural basittir; doğrulama işlemlerinin sorumluluğu aslında Ado.Net Data Service örneğini host eden uygulamaya aittir. Bu anlamda, servisin bir WCF projesinde veya bir ASP.NET uygulamasında barındırılması doğrulama ve yetkilendirme işlemlerinin daha kolay ele alınabilmesi açısından önemlidir. Ado.Net Data Service'lerinde güvenlik 4 farklı alanda değerlendirilmektedir.

Kriter
Güvenlik Alanı
Açıklama

Host uygulamaya ait doğrulama modeli
Authentication
(Doğrulama)
Servisi kullanacak olan istemcilerin doğrulanması için Host uygulama ortamı ele alınır. Söz gelimiz ASP.NET uygulaması üzerinde yapılan bir hosting işleminde built-in Membership API'si kullanılarak svc dosyalarına olan erişim kısıtlandırılabilir.

Servis Operasyonları
(Service Operations)
Authorization
(Yetkilendirme)
Metod bazlı operasyonlar yazılarak veriye olan erişim kısıtlandırılabilir. Örneğin HTTP Get metoduna göre sadece tek bir sonuçun elde edilmesine izin verilmesi (Single Result) sağlanabilir.

Veri Kesmeleri
(Data Interceptors)
İstemcinin talep ettiği verinin elde edilmesinden (Read) veya değiştirilmesinden (Update,Insert,Delete) önce işlemin kesilerek kısıtlamaların yapılması sağlanabilir. Örneğin kullanıcının yetkisine göre sadece görebileceği ürün listesinin verilmesi gibi.

Entity Görünürlüğü
(Entity Visibility)
Servisin dış ortama sunduğu Entity örneklerinin işlenme şekillerinin sınırlandırılmasıdır. Örneğin Product isimli Entity üzerinde sadece yazma işlemlerine izin verilmesi gibi.

Buradaki kriterlerin uygulanması ile bir Ado.Net Data Service ve içeriğine olan erişim yetkilendirilebilir. Aslında teknik detayları, geliştireceğimiz örneğin aralarına serpiştirerek devam edebiliriz. İlk olarak bize bir test servisi gerekmektedir. Konuyu kolay işlemek adına Ado.Net Entity Framework öğesini kullanaraktan Northwind veritabanındaki tüm tabloları ele aldığımızı düşünelim. Sonrasında ise basit bir Ado.Net Data Service öğesi geliştireceğiz. Peki ama bu öğeyi nerede barındıracağız? İşte burada kullanıcı doğrulamasını (Authentication) kolayca tesis edebileceğimiz bir Asp.Net uygulamasını göz önüne alabiliriz. Elbetteki Asp.Net Web Site Administration Tool'unu kullanaraktan bir kaç test kullanıcısı ve rolü oluşturmaktada yarar olacaktır. Örneklerimizde kullanacağımız kullanıcı bilgileri aşağıdaki gibidir.(Örnekte SQL Express Edition kullanılmış ve bu nedenle ASPNETDB.MDF dosyası web sitesinin olduğu AppData klasörü altında oluşturulmuştur.)

Kullanıcı
Şifre
Rol

dealer1
dealer1.
Dealer

dealer2
dealer2.

dealer3
dealer3.

region1
region1.
Region

region2
region2.

Önemli unsurlardan biriside siteye olan ve amacımız gereği özellikle Ado.Net Data Service öğelerine olan erişimi kısıtlamaktır. Yani siteye isimsiz kullanıcı (Anonymous User) girişi kesin olarak engellenmelidir. Aşağıdaki ekran görüntüsüde, web.config dosyasında söz konusu engelleme için yapılmış olan değişikliler açık bir şekilde görülebilir.

![mk268_1.gif](/assets/images/2009/mk268_1.gif)

Dikkat edileceği üzere Form tabanlı doğrulama kullanılmaktadır ve isimsiz kullanıcıların sisteme girmeleri yasaklanmıştır.

> Elbetteki Form Tabanlı Doğrulama (Form Based Authentication) şart değildir. Özellikle Intranet sistemlerde Windows tabanlı doğrulama (Windows Based Authentication)' da ele alınabilir. Hatta istenirse Passport Tabanlı Doğrulama da etkinleştirilebilir. Hangisi kullanılırsa kullanılsın, servisi host eden web sisteminin doğrulama yetenekleri, Ado.Net Data Service çalışma zamanı tarafından ele alınabilmektedir.

Form tabanlı doğrulama söz konusu olduğundan varsayılan olarak aksi belirtilmedikçe (Asp.Net Güvenlik konularını hatırlayalım) Login.aspx isimli bir giriş sayfasına ihtiyacımız olacaktır. Bu sayfa üzerinde yine Asp.Net bileşenlerinden olan Login kontrolü kullanılabilir.

![mk268_2.gif](/assets/images/2009/mk268_2.gif)

Sonuç olarak servis tarafındaki projenin durumu aşağıdaki şekilde olduğu gibidir.

![mk268_3.gif](/assets/images/2009/mk268_3.gif)

Dikkat edileceği üzere servisi kullanacak olan kişilerin doğrulanması işlemini host uygulamanın kendisine vermiş bulunmaktayız. Buna göre kullanıcılar herhangibir şekilde servis dosyasını talep ettiklerinde, eğer bir bilete sahip değillerse, otomatik olarak Login.aspx sayfasına yönlendirileceklerdir. ASPNETDB.MDF veritabanı dosyasında yer alan ve etkin olan bir kullanıcı ile sisteme girildiğnde ise, kodlama tarafında karar vereceğimiz yetkilendirmeler devreye girecektir. Yani veriye olan erişim istenirse kullanıcıya göre kısıtlandırılabilecektir. Şimdi bu durumları analiz etmeye çalışalım. Konuyu son derece basit bir şekilde ele alacağımızdan NorthwindDataService.cs kod içeriğini aşağıdaki gibi yazmamız şimdilik yeterli olacaktır.

```csharp
using System;
using System.Data.Services;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel.Web;
using System.Web;
using NorthwindModel;
using System.Collections.Generic;

public class NorthwindDataService 
    : DataService<NorthwindEntities>
{ 
    public static void InitializeService(IDataServiceConfiguration config)
    {
        // Rol tabanlı veri çekişi işlemleri örnek olarak gösterilebilir; HttpContext.Current.User.IsInRole();

        config.SetEntitySetAccessRule("*", EntitySetRights.All);
        config.SetEntitySetAccessRule("Employees", EntitySetRights.ReadMultiple);
        config.SetEntitySetAccessRule("Orders", EntitySetRights.WriteAppend);
        config.SetEntitySetAccessRule("Customers", EntitySetRights.None);

        config.SetServiceOperationAccessRule("CustomerCities", ServiceOperationRights.All);
        config.SetServiceOperationAccessRule("MySuppliers", ServiceOperationRights.All);

        // Eğer alttaki satır açılırsa FilterForProducts metodu devre dışı sayılır ve Products entity' sine hiç bir şekilde erişilemez.
        //config.SetEntitySetAccessRule("Products", EntitySetRights.None);

    }

    [QueryInterceptor("Products")]
    public Expression<Func<Products, bool>> FilterForProducts()
    { 
        string name = HttpContext.Current.User.Identity.Name;
    
        if (name == "dealer1")
            return p => p.Suppliers.SupplierID == 1 || p.Suppliers.SupplierID == 2;
        else if (name == "dealer2")
            return p => p.Suppliers.SupplierID == 3;
        else if (name == "dealer3")
            return p => p.Suppliers.SupplierID == 1 || p.Suppliers.SupplierID == 2 || p.Suppliers.SupplierID == 7;
        else
            return p => p.Suppliers.SupplierID != null;
    }

    [ChangeInterceptor("Products")]
    public void ProductChange(Products p, UpdateOperations operation)
    {
        switch (operation)
        {
            case UpdateOperations.Add:
                break;
            case UpdateOperations.Change:
                if(!HttpContext.Current.User.IsInRole("Region")) 
                    throw new DataServiceException(405,"UnitPrice için bu değişikliğe izin verilmedi");
                break;
            case UpdateOperations.Delete:
                break;
            case UpdateOperations.None:
                break;
            default:
                break;
        }
    }

    #region Service Operations
    
    [WebGet] 
    public IQueryable<string> CustomerCities()
    {
        return (from c in this.CurrentDataSource.Customers
            select c.City).Distinct();
    }

    // filter, orderby gibi operatörler ve key bazlı erişim gibi sorgulara izin verilmez. Sadece entity bazlı erişim söz konusudur
    [WebGet]
    public IEnumerable<Suppliers> MySuppliers()
    {
        return from c in this.CurrentDataSource.Suppliers
                orderby c.CompanyName
                select c;
    }

    #endregion
}
```

İlk olarak uygulamamızı test etmeye çalıştığımızda Login.aspx sayfasına yönlendirildiğimizi göreceğiz. Bir başka deyişle herhangibir isimiz kullanıcı, servise doğrudan erişilmek istendiğinde http://localhost:1000/SecuritySolutions/login.aspx?ReturnUrl=%2fSecuritySolutions%2fNorthwindDataService.svc adresine gönderilecektir ve dolayısıyla doğrulama sürecine girilmiş olacaktır. Eğer geçerli bir kullanıcı bilgisi ile giriş yapabilirsek bu durumda standart olarak servise erişebildiğimizi ve izin verilen sorgulamaları yapabildiğimizi göreceğiz. Şimdi kodu biraz analiz etmeye çalışalım. InitializeService metodu içerisine bakıldığında Entity seviyesinde bazı yetkilendirmeler yapıldığı görülmektedir.

```csharp
config.SetEntitySetAccessRule("*", EntitySetRights.All);
config.SetEntitySetAccessRule("Employees", EntitySetRights.ReadMultiple);
config.SetEntitySetAccessRule("Orders", EntitySetRights.WriteAppend);
config.SetEntitySetAccessRule("Customers", EntitySetRights.None);
config.SetServiceOperationAccessRule("CustomerCities", ServiceOperationRights.All);
config.SetServiceOperationAccessRule("MySuppliers", ServiceOperationRights.All);
```

Buna göre tüm Entity kümelerine erişim hakkı izni, ilk satırdaki kod ile verilmiştir. Bu yetkilendirme, ilk satırdaki ve EntitySetRights.All ile sağlanmaktadır. Ne varki ikinci satırda Employees Entity içeriği için, ReadMultiple kısıtlaması yapılmıştır. Buna göre Employees kümesi üzerinde örneğin anahtar bazlı sorgulara izin verilmeyecektir. Yani Employees (2) gibi bir talepte bulunulursa HTTP 403 Forbidden hatası alınır. Gerçektende durum Fiddler aracı yardımıyla izlendiğinde aşağıdaki ekran görüntüsünde yer alan sonuçlar ile karşılaşılır.

![mk268_4.gif](/assets/images/2009/mk268_4.gif)

Dikkat edileceği üzere istemci taraına döndürülen XML içeriğinde Forbidden mesajı yazılmaktadır. Aynı zamanda hata kodu 403' tür. Bu durum istemci uygulama tarafından değerlendirilmelidir. Şu anda ilk yetkilendirmemizi Entity seviyesinde yapmış bulunuyoruz. Görüldüğü üzere, tüm Entity'lere erişim hakkı verilmiş olmasına rağmen Employees üzerinde bir kısıtlama uygulanmıştır. Üçüncü satırdaki kısıtlamaya göre Orders kümesi için sadece yeni öğe eklenmesine izin verilmektedir. Bu nedenle Orders entity içeriği yine tarayıcı üzerinden sorgulanmak istendiğinde HTTP 403 Forbidden hatası alınacaktır. Ancak istemci bir uygulama tarafından, Orders isimli veri kümesine yeni öğelerin eklenmesi işlemine izin verilecektir. Son olarak Customers Entity'sine herhangibir şekilde erişim izni kesinlikle verilmemektedir. Nitekim EntitySeyRights enum sabiti için None değeri verilmiştir.

Lakin burada CustomerCities isimli bir operasyon için izin verildiği gözlemlenmektedir. CustomerCities isimli operasyon string tabanlı IQueryable tipinden bir referans döndürmektedir. İçerideki sorgu cümlesine bakıldığında Customers Entity'si içerisindeki City adlarının Distinct fonksiyonu ile benzersiz olacak şekilde döndürüldüğü görülmektedir. Dolayısıyla Customers veri kümesini dış ortama tamamen kapatıp, kendisine ait şehir adlarını istemciye sunan bir operasyon tanımlaması söz konusudur ki buda bir güvenlik tedbiri olarak düşünülebilir.

Yine devam eden satırda MySuppliers isimli bir servis operasyonuna tüm haklar ile erişim izni verilmiştir. Bu servis operasyonuna bakıldığında ise IEnumerable tipinden bir referans döndürdüğü görülmektedir. Operasyon içerisindeki LINQ sorgusunda özel bir ifade yoktur. Ancak metodun dönüş tipinin IEnumerable olmasının bir anlamı vardır. Buna göre Suppliers tablosu sorgulanırken filter, orderby gibi operatörler kullanılamaz. Ayrıca anahtar bazlı erişimlere de (örneğin Suppliers (2) gibi) izin verilmez. Sadece sonuç kümesinin ham hali istemciye sunulur. Buda sonuç itibariyle bir kısıtlamadır. Nitekim çalışma zamaında örneğin, SupplierId değeri 3 olan Supplier bilgisini almak istediğimizde HTTP 400 Bad Request hatasını aldığımız görebiliriz.

![mk268_5.gif](/assets/images/2009/mk268_5.gif)

Bu IQueryable ve IEnumerable arasındaki farkı biraz daha net bir şekilde görmüş bulunuyoruz. Yanlız dikkat edilmesi gereken bir nokta vardır. Servis operasyonunun döndürdüğü bir Entity içeriği var iken (örneğin IEnumerable) InitializeService metodu içerisinde söz konusu tip için EnititySetRights.None değerinin kullanılması sonrasında servis çalışmayacaktır. Örneğimizde Customers için yapılan kısıtlamaya dikkat edildiğinde MyCustomers isimli operasyonun geriye string bazlı bir sonuç kümesi döndürdüğü görülmektedir. Bu servisin çalışmasına engel olmamaktadır.

Özellikle Entity tipleri ve servis operasyonları için yapılan erişim kısıtlamalarında devreye giren EntitySetRights enum sabitinin alabileceği değerler aşağıdaki tabloda belirtildiği gibidir.

EntitySetRights Değerleri

None
Entity kümesine erişilmesi yasaklanmıştır. Metadata içerisinde görünmez ve üzerine okuma yazma işlemleri yapılamaz.

ReadSingle
Entity üzerinde anahtar bazlı (Key Based) aramalara izin verilir. (Customers ('ALFKI') gibi)

ReadMultiple
Entity içeriğinin sorgulanmasına izin verilir, ancak anahtar bazlı erişimlere izin verilmez. Örneğin Employees (2) için sonuç kümesi elde edilemez.

WriteAppend
Yeni Entity örnekleri eklenebilir.

WriteMerge
Var olan Entity içeriği güncellenirken birleştirilme işlemi uygulanır.

WriteReplace
Var olan Entity içeriği yenisi ile değiştirilerek güncellenir.

WriteDelete
Silme işlemine izin verilir.

AllRead
ReadSingle ve ReadMultiple değerlerinin birleşimidir.

AllWrite
WriteInsert, WriteUpdate ve WriteDelete değerlerinin birleşimidir.

All
Tam okuma ve yazma erişimine izin verilir.

Servis operasyonlarında erişim kısıtlamalarını belirleyen ServiceOperationRights enum sabitinin alabileceği değerler ise aşağıdaki tabloda görüldüğü gibidir.

ServiceOperationRights Değerleri

None
Servis operasyonuna erişim izni yoktur.

ReadSingle
Tek bir veri öğesinin okunmasına izin verilir.

ReadMultiple
Servis operasyonu kullanılarak birden fazla veri öğesinin okunmasına izin verilir.

AllRead
Tekil veya çoğul veri öğelerinin okunmasına izin verilir.

All
Servis operasyonu için tüm haklar sağlanır.

Kodumuzda ilerlediğimizde, FilterForProducts ve ProductChange isimli iki metod ile karşılaşmaktayız. Bu metodlar veri kesme (Data Interceptor) fonksiyonellikleridir. Dikkat edileceği üzere FilterByProducts metodu içerisinde o anki kullanıcının adına bakılaraktan örnek bir içeriğin döndürülmesi sağlanmaktadır. Söz gelimi dealer2 isimli kullanıcı ile sisteme girildiğinde ve Products içeriği talep edildiğinde koddaki kesme metodunun içeriğine göre SupplierID değeri 3 olan listenin elde edildiği görülür. Özellikle durumu SQL Server Profiler aracı yardımıyla incelendiğinde gerçektende kesme metodunun devreye girdiği aşikardır.

![mk268_6.gif](/assets/images/2009/mk268_6.gif)

Burada belkide en önemli nokta kesme işlemi sırasında, kullanıcı bilgisinin HttpContext.Current.User.Identity.Name ifadesi ile alınmasıdır. Bu tahmin edileceği üzere servisi kullanmak için Login olan kullanıcının adıdır.

Örneğimizde senaryoyu çok basit bir şekilde ele almak istediğimizden kullanıcı adlarının dealer1, dealer2, dealer3 olması halleri ele alınmıştır. Oysaki gerçek hayat senaryolarında daha faydalı bir kesme işlemi yapılabilir. Bununla ilişkili olaraktan sizlere bir alıştırma senaryosu örneği vermek isterim. Söz gelimi, kullanıcının hangi ürünlere bakacağı bilgisi, kullanıcının dahil olduğu bölgeye bağlı olabilir. Bu durumda ASPNETDB.MDF veritabanında yer alan kullanıcı bilgisi ile, kullanıcıların dahil olduğu bölgeleri tutan başka bir eşleştirme tablosu bu senaryo için çok yararlı olabilir. Böylece kesme metodu içerisinde, eşleştirme tablosundan yararlanılarak, giren kullanıcının sadece dahil olduğu bölgeye ait ürünleri görmesi sağlanabilir. Bunu kendi başınıza denemenizi ve yapmaya çalışmanızı öneririm.

> Örneğimizdeki veri kesme metodları (Data Interceptors) içerisinde servisin host edildiği uygulama ortamının içeriğinin kullanıldığı görülmektedir. Burada Web ortamında olunmasının büyük bir avantaj sağladığı çok açıktır. Nitekim, o anki HTTP içeriğine HttpContext özelliği üzerinden ulaşılabilmektedir. Bu sebepten sisteme giriş yapmış olan kullanıcıyı tespit etmek son derece kolaydır. Ayrıca Ado.Net Data Service'lerin host edildiği web ortamlarında, Application, Session, Caching gibi yapılarında ele alınması mümkün hale gelmektedir.

Gelelim ProductChange metoduna. Bu metod içerisinde giriş yapan kullanıcın Region rolünde olması halinde değişiklik yapabilmesine müsade edilmektedir. Eğer giren kullanıcı Region rolünde değilse istemci tarafına HTTP Statu Code 405 mesajı gönderilmektedir. Aslında kesme operasyonlarını daha net bir şekilde ele alabilmek için bir istemci uygulama yazılmasında yarar vardır. Veri değiştirme işlemleri sırasında devreye giren bu metodda önemli olan noktalardan biriside UpdateOperations enum sabitinin kullanılmasıdır.

Bu sabitin değerine göre kullanıcının nasıl bir operasyon gerçekleştirmek istediği kolayca tespit edilebilir. Metodun ilk parametresi kesme operasyonunun kime uygulanacağını işaret etmektedir. Buna göre söz konusu kesme operasyonları Products tipine uygulanabilir. Diğer önemli bir noktada istemci tarafına HTTP 405 mesajının DataServiceException tipinden bir nesne örneği fırlatılarak gönderiliyor olmasıdır. Çok doğal olarak bu istisna tipinin istemci uygulama tarafından ele alınıyor olması gerekmektedir. (Yani istemci tarafında try...catch...finally blokları kullanılarak istisna yönetimi yapılmalıdır.)

Söz konusu sistemde istemci uygulamanın, servisten talepte bulunurken belirli bir kullanıcı bilgisini göndermesi de şarttır. Aslında bu noktada Client Application Services'lerden faydalanılabilinir.(Bu konu ile ilişkili olarak daha önceki [makalemi](http://www.bsenyurt.com/makalegoster.aspx?ID=267) takip etmenizi öneririm) Özellikle.Net tabanlı istemcilerde İstemci Uygulama Servisinin kullanılmasını öneririm. Tabi bunun için servis tarafındaki konfigurasyon dosyasında bir takım değişikliklerin yapılması gerekmektedir. Bu sebeple host uygulamanın web.config dosyasında aşağıda yer alan eklemeleri yapabiliriz.

```xml
...
<appSettings/>

<system.web.extensions>
    <scripting>
        <webServices>
            <authenticationService enabled="true"/>
            <roleService enabled="true"/>
        </webServices>
    </scripting>
</system.web.extensions>

<connectionStrings>
...
```

İstemci uygulama tarafında ise proje özelliklerinden (Properties), Services kısmına geçmemiz ve geliştirdiğimiz web uygulamasının adresini işaret etmemiz yeterlidir.

![mk268_8.gif](/assets/images/2009/mk268_8.gif)

Bölyece istemci uygulamanın doğrulama (authentication) ve rol (role) yönetimi için geliştirilen web uygulamasının üyelik sistemini (Membership API) kullanılacağı belirtilmiş olur. Örnek içerisinde Membership sınıfını kullanarak doğrulama yapacağımızdan System.Web.dll assembly'ının servis referansı ile birlikte istemci uygulamaya ekleniyor olmasıda gerekmektedir.

> Servis tarafında isimsiz kullanıcıların (Anonymous Users) sisteme girişini kapattığımız için Add Service Reference kısmından servise ait WSDL içeriğini elde edemediğimizi görürüz. Nitekim Visual Studio ortamında söz konusu servis talep edildiğinde otomatikman web uygulamasının authentication kuralı devreye girmekte ve bizi Login.aspx sayfasına yönlendirmeye çalışmaktadır. Hal böyle oluncada servise ulaşılamamakta ve referansı elde edilememektedir.
> Örnekte servisin host edildiği web uygulamasındaki deny user="?" kısmı, istemci uygulamayla aynı solution içerisindeki servis referansı eklendikten sonra etkin hale getirilmiştir. Bu elbetteki istenen çözüm değildir ve ayrıca daha sonrada servisin güncelleştirilmesi sırasında problemlere neden olmaktadır.
> Diğer taraftan datasvcutil aracı yardımıylada istemci tarafı için gerekli tipler üretilmek istendiğinde benzer sonuçlar ile karşılaşılacaktır. Aşağıdaki ekran görüntüsünde ilk denemede anonymous kullanıcıların geri çevrildiği senaryo sonrası alınan uyarı mesajı görülmektedir. İkinci deneme yapılmadan önce ise web.config dosyasındaki deny users="?" kısmı allow users="?" olarak değiştirilmiş ve gerekli tiplerin üretildiği görülmüştür. Elbetteki buda istenen bir aktarım şekli değildir.
> ![mk268_10.gif](/assets/images/2009/mk268_10.gif)
> Bu noktada belkide servisin kullanılabilmesi için, istemci tarafınca gerek duyulan proxy tiplerinin önceden üretilip, kullanacak olan uygulamalara dağıtılması yöntemi tercih edilebilir. Açıkçası bu, servisi kullanacak olan istemcilerin belli olduğu durumlarda düşünülebilecek bir senaryodur ki pek çok büyük çaplı şirket içi projede göz önüne alınabilir.

Bu arada istemci tarafındaki Console uygulamasının içeriğini aşağıdaki gibi örnekleyebiliriz.

```csharp
using System;
using System.Data.Services.Client;
using System.Linq;
using System.Net;
using ClientApplication.NorthwindServiceReference;
using System.Web.ClientServices;
using System.Threading;
using System.Web.Security;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            NorthwindEntities entities = new NorthwindEntities(
                new Uri("http://buraksenyurt:1000/SecuritySolutions/NorthwindDataService.svc")
                );

            entities.SendingRequest+=delegate(object sender,SendingRequestEventArgs e)
            {
                ClientFormsIdentity identity = Thread.CurrentPrincipal.Identity as ClientFormsIdentity;
                HttpWebRequest webRequest = e.Request as HttpWebRequest;
                if (identity != null)
                    webRequest.CookieContainer = identity.AuthenticationCookies;
            };

            try
            {
                if (Membership.ValidateUser("dealer1", "dealer1."))
                {
                    // QueryInterceptor için istemci kodu
                    var tumUrunler = from urun in entities.Products
                                                select urun;
    
                    foreach (var urun in tumUrunler)
                    {
                        Console.WriteLine(urun.ProductName);
                    }

                    // Http durum kodları(Http Status Code) için link-> http://en.wikipedia.org/wiki/List_of_HTTP_status_codes 
        
                    // ChangeInterceptor için istemci kodu
                    var u = (from urun in entities.Products
                        where urun.ProductID == 1
                            select urun).First<Products>();
                    u.UnitPrice +=1;

                    entities.UpdateObject(u);
                    entities.SaveChanges();
                }
            }
            catch (DataServiceRequestException excp)
            {
                string excpMessage = String.Format("Status Code : {0}\n Inner Exception Message : {1} ",
                        ((DataServiceClientException)excp.InnerException).StatusCode.ToString(), excp.InnerException.Message
                    );
                Console.WriteLine(excpMessage);
            }            
        }
    }
}
```

Burada belkide en can alıcı nokta doğrulama için istemci tarafından kullanıcı ve şifre bilgilerinin nasıl gönderildiğidir. Dikkat edilecek olursa servise talep gönderilmeden önce ilgili doğrulama bilgileri yollanmaktadır. Membership sınıfının ValidateUser metodu yardımıyla kullanıcı doğrulandıktan sonra ise entity talebinde bulunulmakta ve bir güncelleştirme işlemi gerçekleştirilmektedir. Uygulamayı çalıştırdığımızda aşağıdaki ekran görüntüsü ile karşılaşırız.

![mk268_9.gif](/assets/images/2009/mk268_9.gif)

Ürün bilgileri alınırken servis tarafındaki FilterForProducts isimli veri kesme metodu devreye girmiş ve delaer1 için SupplierID değerleri 1 veya 2 olanlar getirilmiştir. Yine dikkat edilecek olursa SaveChanges metodundan sonra servis tarafındaki ProductChange kesme metodunun devreye girmesi sonucu istemciye hata mesajı döndürülmüş ve bir istisna (exception) oluşmuştur. Bu son derece doğaldır nitekim dealer1 kullanıcısı Region rolünde değildir. Ancak örneğin region1 kullanıcısı ile giriş yaparsak bu durumda SupplierID değeri null olmayan ürünleri çekebildiğimizi ve aynı zamanda bunlardan ilkinin UnitPrice değerlerinide değiştirebildiğimizi görürüz. Hatta SQL Server Profiler aracı ile arka plandaki sorgu durumunu izlersek aşağıdakine benzer bir sorgunun işletildiğini kolayca izleyebiliriz.

```text
exec sp_executesql N'update [dbo].[Products]
set [ProductName] = @0, [QuantityPerUnit] = @1, [UnitPrice] = @2, [UnitsInStock] = @3, [UnitsOnOrder] = @4, [ReorderLevel] = @5, [Discontinued] = @6
where ([ProductID] = @7)
',N'@0 nvarchar(4),@1 nvarchar(18),@2 decimal(19,4),@3 smallint,@4 smallint,@5 smallint,@6 bit,@7 int',@0=N'Chai',@1=N'10 boxes x 20 bags',@2=22.0000,@3=39,@4=0,@5=10,@6=0,@7=1
```

Örneklerdende görüldüğü üzere Ado.Net Data Service'lerde güvenliği sağlarken doğrulama (Authentication) ve yetkilendirme (Authorization) adına yapılabilecek belirli işlemler söz konusudur. Bu işlemler için bazı kuralların uygulanması gerekmektedir. Söz gelimi servis operasyonları göz önüne alındığında, yazılacak olan metodlarda dikkat edilmesi gereken kurallar şunlardır.

- Metodun public erişim belirleyicisine sahip olması gerekmektedir.
- Metodun dönüş tipi IQueryable veya IEnumerable olabilir. Buradaki T, Entity tipidir. Eğer operasyonun döndürdüğü sonuç kümesi üzerinde sıralama, sayfalama, filtreleme gibi işlemler yapılacaksa IQueryable tipinin döndürülmesi gerekir.
- HTTP Get metoduna uygun çağrılar için WebGet, HTTP Post,Delete,Put gibi talepler içinse WebInvoke niteliği (Attribute) kullanılmalıdır.

Benzer şekilde okuma işlemleri sırasındaki kesme fonksiyonelliklerininde uygulaması gereken bazı kurallar vardır. Buna göre;

- Metodun public erişim belirleyicisine sahip olması gerekir.
- Metoda [QueryInterceptor ("EntityName")] niteliğinin uygulanması gerekir. EntityName yerine kesme işleminin uygulanacağı Entity tipinin adı verilir.
- Metodun dönüş tipi Expression> olmalıdır. Func temsilcisinde yer alan T entity tipidir.
- Metod parametre almaz.
- Metodun işleyişi sırasında bir istisna (Excpetion) oluşsa bile istemci talebi tamamlanır ve kendisine hata mesajı uygun HTTP Statu Code değeri ile döndürülür.

Eğer kesme operasyonu veri güncellenmesi,eklenmesi veya silinmesi işlemleri sırasında yapılacaksa, izlenilmesi gereken kurallar aşağıdaki gibidir.

- Metodun public erişim belirleyicisi olmalıdır.
- Metoda [ChangeInterceptor ("EntityName")] niteliği uygulanmalıdır. Buradaki EntityName, entity tipinin adıdır.
- Metodun dönüş tipi yoktur. Bu nedenle void olarak tanımlanır.
- Metodun iki parametresi vardır. İlki entity tipi ikincisi ise UpdateOperations enum sabitidir. Bu enum sabiti ile metodu içerisinde değiştirme, silme, ekleme operasyonları ele alınabilir.
- Metodun işleyişi sırasında bir istisna oluşursa, istemciden gelen talep tamamlanır ve kendisine uygun olan Http Statu Code değerine sahip hata gönderilir. Bu istisna sonrasında sunucu tarafındaki asıl veri kaynağında herhangibir değişiklik kesinlikle olmaz.

Makalemizi sonlandırmadan önce önemli bir noktayı daha vurgulamakta yarar vardır. Servisin doğrulanması için istemci tarafından gönderilen kullanıcı adı ve şifre bilgileri açık metinler olarak gitmektedir. Eğer örnekler test edilirken Fiddler aracı yardımıyla arka plandaki paketler izlenirse aşağıdaki ekran görüntüsünde yer alan durum ile karışalışır.

![mk268_11.gif](/assets/images/2009/mk268_11.gif)

Bu nedenle en uygun çözüm servisin HTTPS tabanlı bir iletişim üzerinden hizmet vermesinin sağlanması olarak düşünülebilir.

Bu yazımızda Ado.Net Data Service'lerde doğrulama ve yetkilendirme işlemlerinin nasıl ele alınabileceğini, bir başka deyişle güvenliğin nasıl sağlanabileceğini en temel hatlarıyla incelmeye çalıştık. Ado.Net Data Service konusunda geliştirmeler devam etmektedir. Güvenlik ile ilişkili olaraktan farklı yaklaşımların getirilmeside bu nedenle söz konusu olabilir. Ancak en azından, host uygulamanın bu işte önemli bir rol üstlendiği gözden kaçırılmamalıdır. Bu yazıda kullanılan tekniğe göre, doğrulama işlemini Asp.Net Web uygulaması devralmıştır. Size tavsiyem bunu bir WCF sitesinden host ederkende gerçekleştiriyor olmanızdır. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örneği İndirmek İçin Tıklayın](/assets/files/2009/guvenlik.rar) (Boyutun küçük olması için ASPNETDB.MDF ve log dosyası çıkartılmıştır. Bu nedenle örneği deneyebilmek için söz konusu veritabanını Asp.Net Web Site Administration Tool ile oluşturmanız gerekmektedir.)