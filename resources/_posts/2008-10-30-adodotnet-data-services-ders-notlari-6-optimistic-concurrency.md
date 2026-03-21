---
layout: post
title: "Ado.Net Data Services Ders Notları - 6 (Optimistic Concurrency)"
date: 2008-10-30 10:00:00 +0300
categories:
  - ado-net-data-services
tags:
  - ado.net-data-services
  - wcf-data-services
  - windows-communication-foundation
---
İstemci-Sunucu (Client-Server) bazlı uygulamalar göz önüne alındığında, istemcilerin aynı veriler üzerinde birbirlerinden habersiz şekilde değişiklikler yapabilme ihtimali oldukça meşhur bir vaka olarak bilinmektedir. Özellikle.Net tarafında bağlantısız katman (Disconnected Layer) uygulamalarında bu tip vakalar son derece önemlidir. Zaman zaman bu tip vakalar ile mücadele etmek ve tedbirler almak gerekir. Vaka aslında şu şekilde ifade edilebilir; "sunucu üzerinden aynı veri içeriklerini çeken istemci programlar, sunucu ile bağlantılarını kestikten sonra kendi uygulama alanları üzerine aldıkları verilerde değişiklik yapabilirler.

Ancak bu noktada sunucu ile sürekli bir bağlantıları olmadığından, başka istemcilerin aynı veriler üzerinde değişiklikler yapıp yapmadıklarını tam olarak bilemezler. Bu sebepten aynı veriler üzerinde birbirlerinden habersiz olacak şekilde yaptıkları değişiklikleri sunucuya gönderebilirler." İşte bu noktada sunucu tarafında durumun nasıl ele alınacağı önem kazanır. Bu amaçla çeşitli denetleme mekanizmaları kullanılabilir. Bu yazımızda hepinizin kulağında bol bol Optimistic Concurrency kelimelerinin çınlayacağını şimdiden söyleyebilirim.

Bahsetmiş olduğumuz bu vaka bazen görmezden gelinebilecek olmasına karşın çoğu durumda kontrol altına alınması gereken bir sorun olarak değerlendirilir. İşin içerisine birde değişikliklerin sunucu üzerindeki veritabanına gönderilmesi sırasında devreye alınan Transaction'lar girerse, vaka kendi içerisinde dahada karmaşıklaşır. Ancak bizim şu anda istemediğimiz tek şey bu vakayı dahada karıştırmaktır. Bunlara karşın söz konusu vakada çözümsel olarak optimistic (iyimser) yada pesimistic (kötümser) yaklaşımların uygulanabilir olduklarınıda bilmek gerekir. Peki bu durumun Ado.Net Data Service'ler ile olan bağlantısı nedir? Herşeyden önce Ado.Net Data Service hangi modeli baz alır? Baz aldığı model nasıl uygulanır?

Bildiğiniz gibi bu ana kadarki ders notlarımızda ve görsel derslerimizde Ado.Net Data Service'lerin, web programlama modeline uygun olarak çalıştığını ve EDM (Entity Data Model) yada Custom LINQ Provider gibi katmanlar üzerinde veri sunumu gerçekleştirdiğine değindik. Ayrıca, Ado.Net Data Service'ler bir sunucu uygulama üzerinden host edilmek zorunda olmakla birlikte, bunları tüketen farklı istemci uygulamalar yazılabilmektedir. Bir başka deyişle tipik bir istemci-sunucu modeli söz konusudur. Bunlara ilaveten işin içerisinde, istemci tarafına çekilebilen veriler ve tabiki CRUD (CreateRetriveUpdateDelete) operasyonları söz konusudur. Bu operasyonlar içerisinde yer alan CUD fonksiyonellikleri ve istemcilerin veriyi kendi uygulama alanlarına çektikten sonra sunucu ile herhangibir bağlantılarının kalmayışı, istemcilerin aynı veriler üzerinde birbirlerinden habersiz değişikliker yapabilecekleri sonucunu doğurmaktadır. Peki bu tarz bir sorun ile nasıl mücadele edilebilir? Ado.Net Data Service çözüm olarak, Optimistic Concurrency yaklaşımının ele alınmasına izin vermektedir.

> Optimistic Concurrency yaklaşımına göre, istemci tarafına çekilen veriler güncelleştirilmek üzere sunucu tarafına gönderildiklerinde ilk hali ile karşılaştırılırlar. Böylece ilk okumadan sonra başka bir istemcinin aynı veriyi değiştirip değiştirmediği kontrol altına alınabilir. Eğer bir değişiklik var ise istemcinin bu konuda uyarılması gerekir. Bu uyarıl üzerinde çalışılmakta olan modele göre çoğunlukla bir istisna (Exception) olarak ele alınır. Söz gelimi Ado.Net tarafından bildiğimiz DBConcurrencyViolation bu tip bir istisnadır. Tabi işin içerisine servis yönelimli bir çözüm girdiğinde bu, çoğunlukla bir Fault Message formatına uygun olacak şekilde hata bilgisi içeren bir XML verisidir. Eğer veriler başkası tarafından değiştirilmemişse tabiki güncelleme işlemi sunucu tarafındada onaylanacaktır.

Peki Ado.Net Data Service tarafında bu yaklaşım nasıl ele alınmaktadır? Artık bu noktadan sonra adım adım basit bir örnek üzerinde ilerlenilmesinde yarar olacağı kanısındayım. Örneğimizi geliştirirken en büyük yardımcılarımızdan biriside Fiddler isimli HTTP Debugging Proxy aracı olacaktır. Nitekim çakışma olması halinde istemciler ve sunucu arasında gidip gelen HTTP paketlerinin incelenmesi gerekmektedir. Test senaryomuz son derece basittir.

Aynı veri satırı üzerinde değişiklik yapacak en az iki istemci uygulamanın çalıştırılması ve bu esnada oluşacak istisnaların (Exceptions) ve HTTP paketlerinin izlenmesi hedeflenmektedir. Bunların yanında SQL tarafında neler olduğunu gözlemlemek adınada SQL Server Profiler aracından yararlanmamız gerekecektir. Tabi öncelikli olarak basit bir WCF Service uygulaması geliştirerek başlamalıyız. Söz konusu servisimiz daha önceden geliştirdiğimiz Azon isimli veritabanını ve içerisinde bir kaç satır veri içeren Kitap isimli tabloyu istemci tarafına sunacak şekilde geliştirilecektir. Bu nedenle WCF Service uygulamımız için gerekli olan ön hazırlıklar aşağıdaki gibidir.

EDM (Entity Data Model) içeriğimiz;

![mk263_2.gif](/assets/images/2008/mk263_2.gif)

Burada hemen bir özelliği vurgulamak gerekiyor. EDM diagramında yer alan Kitap Entity tipi içerisinde yer alan özelliklerin her birisi için Concurrency Mode isimli bir özellik yer almaktadır. Properties penceresinden ulaşılabilen bu özelliğin değeri varsayılan olarak None şeklindedir. Buna Mixed değerini vermemiz halinde Optimistic Concurrency için söz konusu özellik değerlerinin hesaba katılacağı belirtilmiş olunur. Örneğimizde bu amaçla Ad ve Fiyat özelliklerinin Concurrency Mode değerleri Mixed olarak belirlenmiştir. Ki senaryomuzda sadece bu alanların değerleri için Optimistic Concurreny kontrolü yapılacaktır.

![mk263_1.gif](/assets/images/2008/mk263_1.gif)

Kitap tablosuna ait bir kaç satırlık veri içeriği;

![mk263_3.gif](/assets/images/2008/mk263_3.gif)

AzonServices.svc.cs içeriğimiz;

```csharp
using System;
using System.Data.Services;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using AzonModel;

public class AzonServices 
    : DataService<AzonEntities>
{ 
    public static void InitializeService(IDataServiceConfiguration config)
    {
        config.SetEntitySetAccessRule("*", EntitySetRights.All); 
    }
}
```

Tahmin edeceğiniz gibi istemci tarafında CRUD operasyonları yapılabileceğinden EntitySetRights.All enum sabiti değeri kullanılmıştır. Buraya kadar geldikten sonra AzonServices.svc isimli Ado.Net Data Service örneğinin çalıştığından emin olmakta yarar vardır. Bunun için servisi basit bir tarayıcı uygulama içerisinde açmamız yeterli olacaktır. Aşağıdakine benzer bir ekran görüntüsü ile karşılaşırız.

![mk263_4.gif](/assets/images/2008/mk263_4.gif)

Yanlız burada Kitap entity içeriği talep edildiğinde, atom formatında üretilen XML verisinde yeni bir attribute tanımlaması karşımıza gelecektir.

![mk263_5.gif](/assets/images/2008/mk263_5.gif)

Her bir entity elementi içerisinde m:etag isimli bir attribute (nitelik) tanımlanmıştır. m takma adlı isim alanına sahip bu nitelikler içerisinde her bir kitabın Ad ve Fiyat bilgilerinin yer aldığına dikkat edin. Hatırlayacağınız gibi, EDM diagramında bu özelliklerin Concurrency Mode değerlerini Mixed olarak belirlemiştik. Bu nedenle çakışma kontrolü için ilgili özelliklerin değerleri XML çıktısına dahil edilmiştir. Bundan dolayı etag yada entitytag adı verilen nitelikler içerisinde taşınan özellik değerleri, çakışma kontrolü için istemci ile sunucu arasında gidip gelen paketlerde önem kazanmaktadır.

Gelelim istemci uygulamamıza. Amacımız Optimistic Concurrency modelini Ado.Net Data Service'ler üzerinde incelemek olduğundan şimdilik işimizi görecek basit bir program yazmamız yeterli olacaktır. Sanıyorumki ne demek istediğimi anladınız:) Basit bir Console Application geliştiriyoruz. Uygulamamıza aynı solution içerisinde yer alan servisimizide ekledikten sonra istemci uygulama kodlarımızı aşağıdaki gibi geliştirebiliriz.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClientApp.AzonServiceReference;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Proxy nesne örneği oluşturulur.
            AzonEntities proxy = new AzonEntities(new Uri("http://buraksenyurt:1000/AdventureHost/AzonServices.svc"));

            // Concurrency testi için ID si 81 olan Kitap verisi çekilir
            Kitap kitap81 = (from k in proxy.Kitap
                                        where k.KitapId==81
                                            select k).First<Kitap>();
    
            // 81 nolu ID' ye ait kitap bilgileri gösterilir
            Console.WriteLine("{0} : {1} : {2} : {3}",kitap81.KitapId,kitap81.Ad,kitap81.Fiyat,kitap81.StokMiktari);

            // Test amacıyla rastgele bir artış değeri üretilir ve 81 nolu Kitap nesne örneğinin Fiyat değeri değiştirilir
            Random rnd = new Random();
            int yeniFiyatArtisi = rnd.Next(1, 10);
            kitap81.Fiyat = kitap81.Fiyat + yeniFiyatArtisi;

            // Burası test noktası
            Console.WriteLine("{0} in fiyatı {1} olarak değiştirilecek. Onaylamak için tuşa basın.", kitap81.Ad, kitap81.Fiyat);
            Console.ReadLine();

            // Nesne güncellenir
            proxy.UpdateObject(kitap81);
    
            // Bir istisna bloğu içerisinde SaveChanges metodu çağırılır.
            try
            {
                proxy.SaveChanges();
                Console.WriteLine("İşlem tamam");
            }
            catch (Exception excp)
            {
                // Burada beklenen hata mesajı InnerException içerisinde gelir
                Console.WriteLine(excp.InnerException.Message);
            }

            Console.ReadLine();
        }
    }
}
```

Kısaca istemci uygulamada neler yaptığımızdan bahsedelim. Öncelikli olarak proxy nesnesi örnekleniyor. Fiddler aracını Web Development Server üzerinden çalıştıracağımız için daha önceki makalemizde bahsettiğimiz gibi 1000 numaralı portu ve makine adını kullanıyoruz. İlerleyen kısımlarda test amacıyla KitapID değeri 81 olan kitap bilgilerini istemci tarafına LINQ sorgusu üzerinden çekiyoruz. Elde edilen Kitap nesne örneğinin Fiyat özelliğini Random sınıfı ile üretilen rastgele bir değer kadar arttırıyoruz. Sonrasında ise bir Console.ReadLine çağrısı görmekteyiz. Bu çağrının olduğu yer aynı uygulamadan aynı makinede birden fazla çalıştırdığımızda test yapmamızı kolaylaştıracaktır.

Bu çağrının ardından UpdateObject metodu işletiliyor. Sonrasında ise try...catch blokları içerisinde alınmış olan bir SaveChanges metodu çağrısı görüyoruz. Bu çağrı istemci tarafındaki güncelleştirmenin sunucuya iletilmesine neden oluyor. İşte bu noktada eğer bir çakışma söz konusu ise istemci tarafına bir exception döndürülecektir. Gelin hemen bir test yaparak işe başlayalım. Testimizde aynı uygulamadan iki adet çalıştırıyor ve birinde değişiklikleri sunucuya gönderdikten sonra, ikincisi içinde aynı işlemi yapmayı deniyoruz. Sonuç olarak bu test sonrasında aşağıdaki ekran görüntüleri oluşacaktır.

Her iki uygulama açılıp ilk uygulamadaki güncellemeler servis tarafında gönderildiğinde;

![mk263_6.gif](/assets/images/2008/mk263_6.gif)

İkinci uygulamada devam edilip yeni değerler ile aynı veri güncellenmek üzere servise gönderildiğinde;

![mk263_7.gif](/assets/images/2008/mk263_7.gif)

Görüldüğü gibi ikinci uygulamaya bir adet Fault Exception gönderilmiş ve etag değerinin Request Header'daki güncel etag değeri ile uyuşmadığı belirtilmiştir. Bir başka deyişle ikinci uygulamanın güncelleştirmek istediği satır başkası tarafından güncellenmiştir.

Hemen Fiddler aracı ile arka planda olanları inceleyelim. Birinci uygulama çalıştırıldığında ilk olarak 81 numaralı KitapID değerine sahip veri çekilmektedir. Bu tipik olarak HTTP Get çağrısıdır ve Request (İstek) ile Response (Cevap) paketlerine ait Header (Başlık) içerikleri aşağıdaki ekran görüntüsünde olduğu gibidir.

![mk263_8.gif](/assets/images/2008/mk263_8.gif)

Standart bir iletişim olduğu gözlemlenmekle birlikte Respons Header içerisinde ETag isimli bir bilgi daha yer almaktadır. Bu bilgiye göre ilk uygulamaya çekilen kitap satırında Ad değeri Ado.Net Data Services Pro, Fiyat değeri ise 71.0000 dır. İkinci uygulamada aynı talepte bulunacaktır ve yine yukarıdaki ekran görüntüsünde yer alan paket alışverişi söz konusudur. Bu durumda ikinci uygulama için söz konusu olan Response Header içerisindeki ETag değeride aynıdır. Gelelim 3ncü paket alışverişine.

![mk263_9.gif](/assets/images/2008/mk263_9.gif)

Request Header içerisinde If-Match isimli bir bilgi yer aldığı görülmektedir. Bu bilgiye göre Ado.Net Data Services Pro ve 71.0000 değerlerinin doğrulanması istenmektedir. Şu durumda başka bir uygulama yada veritabanı üzerinden doğrudan olacak şekilde, 81 numaları kayıtta bir değişiklik olmadığından Response Headers içerisinde söz konusu verinin güncellenen değerlerine ait bilgiler istemci tarafına gönderilmektedir. Bir başka deyişle şimdi ETag değerinin içeriği Ado.Net Data Services Pro ve 76.0000 dır. Dikkat edileceği üzere Fiyat değişmiştir. Ancak arkada unutmamamız gereken ikinci uygulamamız vardır. Bu uygulmada tuşa basıp devam edildiğinde, Fiddler üzerinden 4ncü paket alışverişi aşağıdaki gibi yakalanmaktadır.

![mk263_10.gif](/assets/images/2008/mk263_10.gif)

Bu kez Request Header içerisindeki ETag değeri 71.0000 değeri için talepte bulunur. Ancak az önceki uygulamada ETag değerinde yer alan Fiyat 76.0000 olarak değişmiştir. Dolayısıyla If-Match karşılaştırması başarılı olmayacaktır. Bunun sonucu olarakta geriye HTTP/1.1 412 kodu (Precondition Failed) döner.

> HTTP 1.1 durum kodları (Status Codes) ve açıklamaları için WC3 üzerinden yayınlanan[adresinden](http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html#sec10.4.13) bilgi alabilirsiniz.

Çok doğal olarak bu hata için istemci tarafına bir exception bilgisi gönderilmiştir. Bu arada SQL tarafında neler olduğunuda bilmekte yarar vardır. Aynı süreç SQL Server Profiler üzerinden incelendiğinde ilk uygulamanın güncelleştirme işleminden hemen önce 81 numaları KitapID için bir Select sorgusu çalıştırıldığı sonrasında ise aşağıdaki SQL ifadesinin devreye girdiği görülür.

```bash
exec sp_executesql N'update [dbo].[Kitap]
set [Ad] = @0, [Fiyat] = @1, [StokMiktari] = @2, [KategoriId] = @3
where ((([KitapId] = @4) and ([Ad] = @5)) and ([Fiyat] = @6))
',N'@0 nvarchar(25),@1 decimal(19,4),@2 int,@3 int,@4 int,@5 nvarchar(25),@6 decimal(19,4)',@0=N'Ado.Net Data Services Pro',@1=76.0000,@2=35,@3=1,@4=81,@5=N'Ado.Net Data Services Pro',@6=71.0000
```

Dikkat edelim! Where ifadesinden sonra KitapID, Ad ve Fiyat alanları hesaba katılmıştır. Bunun en büyük nedeni EDM diagramında Concurrency Mode değeri Fixed olarak belirlenen özelliklerdir. Dolayısıyla bu Where kriteri bozulmadığından güncelleştirme işlemi yapılmıştır. Oysaki ikinci uygulama tuşa basılarak devam ettirildiğinde, Update sorgusunun çalıştırılmadığı onun yerine 81 nolu KitapID için bir Select sorgusunun işlediği görülür. Akabindede zaten ETag verilerindeki uyuşmazlık nedeni ile istemciye bir hata mesajı gönderilmektedir. Buraya kadar anlattıklarımızın özetini aşağıdaki tablo ile özetleyebiliriz.

İşlem
Açıklama (HTTP Trafiği ve SQL tarafı)

Birinci Uygulama Çalışır.

Kitap kitap81 = (from k in proxy.Kitap
where k.KitapId==81
select k).First ();
HTTP Get Paketi Gönderilir.
Select sorgusu 81 no için çalışır.
Response bilgisi HTTP 200 Ok.
ETag bilgisi "Ado.Net Data Services Pro, 71.0000"

İkinci Uygulama Çalışır.

Kitap kitap81 = (from k in proxy.Kitap
where k.KitapId==81
select k).First ();
HTTP Get Paketi Gönderilir.
Select sorgusu 81 no için çalışır.
Response bilgisi HTTP 200 Ok.
ETag bilgisi "Ado.Net Data Services Pro, 71.0000"

Birinci Uygulamada Fiyat bilgisi güncellenir.

Birinci Uygulamada SaveChanges metodu çalışır.
HTTP Merge paketi gider.
If-Match bilgisi "Ado.Net Data Services Pro, 71.0000" dir. Karşılaştırma doğrudur.
SQL tarafında Update sorgusu çalışır güncelleme yapılır.
Response için ETag değeri "Ado.Net Data Services Pro, 76.0000" olur.

İkinci Uygulamada Fiyat bilgisi güncellenir.

İkinci Uygulamada SaveChanges metodu çalışır.
HTTP Merge paketi gider.
If-Match bilgisi "Ado.Net Data Services Pro, 71.0000" dir.
SQL tarafında 81 için veriler istenir.
If-Match bilgisindeki Fiyat verisi için uyuşmazlık vardır.
HTTP/1.1 412 (Precondition Failed) gönderilir.

Tabi işin bir de diğer şeklini ele almak gerekir. Yani Fixed değerlerini kullanmadığımız durum. Burada sadece servis tarafındaki EDM diagramında değişiklik yapmak yeterli olacaktır. Bir başka deyişle istemci tarafında, Concurrency modelinin değiştirildiğine dair bir servis güncellemesi yapılmasına gerek yoktur. Tabi böyle bir durumda her iki uygulamanın güncelleme işlemleride geçerli olacaktır ve buna görede en son yazanın verisi tabloya yansıtılacaktır. Aynı örneği buna göre test ettiğimizde SQL tarafına giden Update sorgularının aşağıdakine benzer olduğu görülmektedir.

```bash
exec sp_executesql N'update [dbo].[Kitap]
set [Ad] = @0, [Fiyat] = @1, [StokMiktari] = @2, [KategoriId] = @3
where ([KitapId] = @4)
',N'@0 nvarchar(25),@1 decimal(19,4),@2 int,@3 int,@4 int',@0=N'Ado.Net Data Services Pro',@1=88.0000,@2=35,@3=1,@4=81
```

Dikkat edileceği üzere sadece Primary Key alanı hesaba katılmıştır. Yine Fiddler aracı ile istemci ve servis arasındaki HTTP trafiği incelendiğinde If-Match yada ETag gibi bilgilerin Request veya Response Header'ları içerisinde yer almadığı görülür. Görüldüğü üzere senaryonun gerektirdiklerine göre servis tarafında Optimistic Concurrency modeli tercih edilebilir veya edilmez. Eğer bu model tercih edilirse istemci tarafındaki uygulamalarda mutlaka Exception kontrolünün yapılması gerekmektedir. Böylece geldik bir yazımızın daha sonuna. Bir sonraki yazımızda görüşünceye dek hepinize mutlu günler dilerim.

[Örneği indirmek için tıklayın](/assets/files/2008/Concurrency.rar)