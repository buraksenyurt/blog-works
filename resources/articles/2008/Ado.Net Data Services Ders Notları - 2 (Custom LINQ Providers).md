---
title: "Ado.Net Data Services Ders Notları - 2 (Custom LINQ Providers)"
pubDate: 2008-09-24 12:00:00
categories:
  - Ado.Net Data Services
tags:
  - Ado.Net Data Services
  - C#
  - .NET
  - ASP.NET
  - ADO.NET
  - LINQ
  - WCF
  - XML
  - REST
  - HTTP
  - Concurrency
  - Performance
  - Generics
  - Visual Studio
---

# Ado.Net Data Services Ders Notları - 2 (Custom LINQ Providers)
Değerli Okurlarım Merhabalar,

Son bahar yada kış gibi mevsimler ile özellikle yağmurlu ve kasvetli günlerde her geliştiricinin araştırma ve öğrenme süreci ve verimliliğinde belirgin bir artış gözlemlenir. Bu herkes için böyle olmasada en azından benim için geçerli bir durumdur. İşte bu felsefe ve ruh haliyle çıktığımız yolda son makalemizde Ado.Net Data Services konusuna değinmeye başlamış ve ders notlarımızı kaleme almıştık. İkinci ders notlarımızın konusu ise LINQ Provider kullanarak özel bir bağlama işleminin nasıl yapılabileceğini görmek.

Hatırlayacağınız gibi Ado.Net Data Service Entry Point'leri istemcilerden gelen HTTP taleplerini ele almak üzere tasarlanmaktadır. Bu tipteki servislerin belirgin amacı, veriler üzerindeki hizmetleri operasyonel bazda istemcilere sunabilmektir. Bu nedenle arka planda bir veri erişim katmanı (Data Access Layer) mutlaka söz konusudur. Temel olarak DAL içerisinde iki farklı açılım olduğundan bir önceki yazımızda kısaca bahsetmiştik. Buna göre Entity Data Model veya LINQ Provider seçenekleri söz konusudur. Bu günkü ders notlarımızda ele alacağımız LINQ Provider, çoğunlukla Entity tiplerinin geliştirici tarafından tasarlanıp farklı veri kaynaklarına bağlanması istendiği durumlarda düşünülür. Bilindiği üzere günümüzde pek çok veri kaynağı için yazılmış LINQ Provider araçları söz konusudur.

Söz gelimi Active Directory üzerinde çalışacak şekilde tasarlanmış [LINQ to Active Directory](http://www.codeplex.com/LINQtoAD) sağlayıcısı mevcuttur. Hatta daha ilginçlerinden biriside kütüphanemde yer alan sayısızı kitabı getirdiğim Amazon sitesi için yazılmış olan [LINQ to Amazon](http://weblogs.asp.net/fmarguerie/archive/2006/06/26/Introducing-Linq-to-Amazon.aspx) sağlayıcısıdır ki kitap arama işlemleri için geliştirilmiştir. Sayısız LINQ sağlayıcısının olduğunu ve [Robert Shelton](http://rshelton.com/archive/2008/07/11/list-of-linq-providers.aspx)un blog adresinden bazılarını inceleyebileceğinizi belirtmek isterim. Madem özel LINQ Provider tipleri yazılabiliyor ve çok farklı veri kaynaklarına bağlanılabiliyor, bağlanırken LINQ mimarisinin nimetlerinden yararlanıyor; bu sistemler neden servis bazlı olacak şekilde istemcilere yayınlanamasınlar? İşte bu noktad devreye Ado.Net Data Services girmektedir.

Sanıyorumki bu kadar laf kalabalığından sonra bir örnek geliştirerek hareket etmekte yarar vardır. İşe yine Ado.Net Data Service örneğinin Host edileceği bir WCF Service uygulaması ile başlanacaktır. Sonrasında ise kendi LINQ Provider ortamımız için gerekli tipler ve üyeleri geliştirilecektir. Tüm bu işlemler tamamlandığında ise her zamanki gibi istemci uygulama yazmaya gerek kalmadan basit bir tarayıcı üzerinde gerekli testler yapılacaktır. Öncelikli olarak DepoServisleri isimli bir WCF Service uygulamasını Visual Studio 2008 Professional Service Pack 1 üzerinde oluşturalım. Sonrasında ise sınıf diagramı ve kod içeriği aşağıdaki gibi olan tipleri projemize ilave edelim.

![mk259_1.gif](images/mk259_1.gif)

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Services.Common;

public class Kategori
{
    public int KategoriID { get; set; }
    public string Ad { get; set; }
    // Kategori bağlı Kitaplara ulaşmak için IList<Kitap> tipinden bir özellik kullanılmaktadır
    public IList<Kitap> Kitaplar { get; set; }
}

// DataServiceKey niteliği ile Kitap sınıfının anahtar-Key özelliğinin Numara olduğu belirtilir.
[DataServiceKey("Numara")] 
public class Kitap
{ 
    public int Numara { get; set; }
    public string Ad { get; set; }
    public double BirimFiyat { get; set; }
    // Bir kitabın yazalarına geçiş yapmak için IList<Yazar> tipinden bir özellik kullanılır
    public IList<Yazar> Yazarlar { get; set; }
}

// Yazar sınıfı için Key özelliği string tipinden olan SicilNo' dur.
[DataServiceKey("SicilNo")]
public class Yazar
{
    public string SicilNo { get; set; }
    public string AdSoyad { get; set; }
}

// Entity tiplerini içerisinde barındıran sınıf
public class DukkanEntities
{ 
    static List<Kategori> _kategoriler;
    static List<Kitap> _kitaplar;
    static List<Yazar> _yazarlar;

    // static yapıcı DukkanEntities' e ait taleplerin sayısı ne olursa olsun ilk seferde bir kereliğine çalıştığı için, bellek üzerinde tutulacak nesne topluluklarının oluşturulması sırasında performans kazanımı sağlamaktadır.
    static DukkanEntities()
    {
        // Örnek kategori verileri oluşturulur.
        _kategoriler = new List<Kategori>
            {
                new Kategori{ KategoriID=1, Ad="Bilgisayar Kitapları"},
                new Kategori{ KategoriID=2, Ad="Bilim Teknik Kitapları"},
                new Kategori{ KategoriID=3, Ad="Çizgi Roman"},
            };

        // örnek kitap verileri oluşturulur.
        _kitaplar = new List<Kitap>
            {
                new Kitap{ Numara=9001, Ad="Red Kit", BirimFiyat=10},
                new Kitap{ Numara=9002, Ad="Ten Ten' in Maceraları", BirimFiyat=9.99},
                new Kitap{ Numara=9003, Ad="Örümcek Adım", BirimFiyat=8.99},
                new Kitap{ Numara=9004, Ad="Batman", BirimFiyat=6.99},
                new Kitap{ Numara=9005, Ad="Barbar Conan", BirimFiyat=13.44},
                new Kitap{ Numara=9006, Ad="Superman", BirimFiyat=12.34},
                new Kitap{ Numara=9007, Ad="Martin Myster", BirimFiyat=23.45},
                new Kitap{ Numara=8002, Ad="Programming C#", BirimFiyat=45},
                new Kitap{ Numara=8003, Ad="LINQ Unleashed", BirimFiyat=44.45},
                new Kitap{ Numara=7450, Ad="Essential WCF For .Net Framework 3.5", BirimFiyat=44.50},
                new Kitap{ Numara=1240, Ad="Fermant' nın Son Teoremi", BirimFiyat=5},
                new Kitap{ Numara=2450, Ad="Sayıların Gücü", BirimFiyat=7.5},
                new Kitap{ Numara=2470, Ad="Evrenin Kısa Tarihi", BirimFiyat=9.99}
            };

        // Kategori ve Kitaplar arasındaki ilişkiler veriler üzerinden sağlanır
        for (int i = 0; i < _kategoriler.Count; i++)
            _kategoriler[i].Kitaplar = new List<Kitap>();
        for (int i = 0; i <= 6; i++)
            _kategoriler[2].Kitaplar.Add(_kitaplar[i]);
        for (int i = 7; i <= 9; i++)
            _kategoriler[0].Kitaplar.Add(_kitaplar[i]);
        for (int i = 10; i <= 12; i++)
            _kategoriler[1].Kitaplar.Add(_kitaplar[i]);

        // Örnek yazarlar oluşturulur
        _yazarlar = new List<Yazar>
            {
                new Yazar{ SicilNo="Y100", AdSoyad="Ali"},
                new Yazar{ SicilNo="Y101", AdSoyad="Veli"},
                new Yazar{ SicilNo="Y104", AdSoyad="Mehmet"},
                new Yazar{ SicilNo="Y103", AdSoyad="Burak"},
                new Yazar{ SicilNo="Y107", AdSoyad="Selim"},
                new Yazar{ SicilNo="Y108", AdSoyad="Kamil"},
                new Yazar{ SicilNo="Y109", AdSoyad="Cemil"},
                new Yazar{ SicilNo="Y110", AdSoyad="Nazlı"},
                new Yazar{ SicilNo="Y120", AdSoyad="Ayşe"},
                new Yazar{ SicilNo="Y111", AdSoyad="Fatma"},
                new Yazar{ SicilNo="Y110", AdSoyad="Melike"}
            };

        // Kitaplar ile yazalar arasındaki verisel bağlantılar sağlanır
        Random rnd = new Random();
        for (int i = 0; i < _kitaplar.Count; i++)
        {
            _kitaplar[i].Yazarlar = new List<Yazar>();
            for(int j=0;j<3;j++) 
                _kitaplar[i].Yazarlar.Add(_yazarlar[rnd.Next(0,_yazarlar.Count-1)]);
        }
    }

    // REST modelinde talep edilebilecek özellikler IQueryable<T> tipinden tanımlanır
    public IQueryable<Kategori> Kategoriler
    {
        get
        {
            return _kategoriler.AsQueryable<Kategori>();
        }
    }
    public IQueryable<Kitap> Kitaplar
    {
        get
        {
            return _kitaplar.AsQueryable<Kitap>();
        }
    }
    public IQueryable<Yazar> Yazarlar
    {
        get
        {
            return _yazarlar.AsQueryable<Yazar>();
        }
    }
}
```

Yukarıdaki kod satırları ilk etapta karmaşık görünebilir ancak işin teorisi oldukça kolaydır. Herşeyden önce REST modeline göre dışarıya sunmak isteyeceğimiz tiplerin bir tasarımının servis tarafında olması gerekir. Bu tasarımın karşılığı tahmin edileceği üzere sınıftır. Örnek senaryoda bir kitap dükkanında olması muhtemel materyallere ait sınıflar tasarlanmıştır. Kategori, Kitap ve Yazar. Elbetteki bunlar tamamen farazi tasarımlardır. Önemli olan ve kavramamız gereken nokta, Entity Data Model'dan bağımsız ve LINQ yapısını kullanarak Ado.Net Services için gerekli Data Access Layer'ın geliştirilmesidir.

Bir önceki makaleyi hatırlıyorsak eğer, URL satırından ProductSubcategory (2) gibi taleplerde bulunabileceğimizi görmüştük. Buradaki 2 sayısalı aslında, ProductSubCategory tablosunun PrimaryKey olarak set edilmiş ProductSubCategoryID alanının değeriydi. Dolayısıyla EDM içerisinde bunu işaret edecek biçimde tasarlanmış Property'ler söz konusudur. Şimdi tekrardan Kategori, Kitap ve Yazar sınıflarına dönelim. Dikkat edileceği üzere Kitap ve Yazar sınıflarının başında DataServiceKey isimli bir nitelik (Attribute) kullanılmaktadır.

Bu nitelik sayesinde sınıfın anahtar özelliği (Key Property) belirlenir. Kitap için bu özellik int tipinden olan Numara iken, Yazar için string tipinden olan SicilNo'dur. Tabi burada diğer bir vaka daha vardır. Kategori sınıfı için bu tip bir nitelik kullanılmamıştır. Bunun sebebi ise, çalışma zamanının Key Property'lere bakarken ya [SınıfAdı][ID] yada ID isiminde özellikler aramasıdır. Dikkat edilecek olursa Kategori sınıfında KategoriID isimli bir özellik bulunmaktadır.

> DataServiceKey niteliği (Attribute) sadece sınıf seviyesinde (Class Level) uygulanabilir ve System.Data.Services.Common isim alanı (Namespace) altında yer almaktadır. İki adet aşırı yüklenmiş yapıcısı (Overloaded Constructor) vardır. İstenirse bir sınıfın birden fazla özelliğine Key Property niteliğinin kazandırılmasını sağlayabilir.

Sınıf tasarımlarında dikkat çekici noktalardan bir diğeri ise IList tipinden özelliklerin kullanılmasıdır. Örneğin Kategori ve Kitap sınıfları içerisinde sırasıyla IList ve IList tipinden özellikler yer almaktadır. Bu yaklaşım EDM'nin kullandığının neredeyse aynısıdır. Amaç; bir entity nesne örneği üzerinden ilişkisel diğer entity nesne örneklerine geçiş yapabilmektir. Söz konusu entity nesne örnekleri birer sınıf olduğundan bu geçişte liste bazlı özelliklerin kullanılması son derece doğaldır.

Buraya kadarki kısımlarda kafa karıştırıcı herhangibir nokta olmadığı düşüncesindeyim. Eğer sıkıldıysanız benim gecenin ilerleyen şu saatlerinde yaptığım gibi sıcak bir nescafeyi yudumlanızı yada demlenmiş güzel bir çayı içmenizi tavsiye ederim.

Artık kalan detaylara bakabiliriz. Servis tarafında dışarıya sunulacak olan tipler birer koleksiyon içerisinde tutulmalıdır. Hatta bu koleksiyonları birer özellik olarak barındıracak bir başka deyişle entity tiplerini içerecek bir taşıyıcının (Container) var olmasıda gerekir. Bu nedenle DukkanEntity isimli bir sınıf tasarlanmalıdır. Bu sınıf içerisinde bellek üzerinde tutulacak olan Entity nesneleri için gerekli üyeler ve kodlar yer almaktadır. Ancak dikkat edilmesi gereken en önemli unsur Entity özelliklerinin IQueryable tipinden tanımlanmış olmamılardır ki Ado.Net Data Services tarafı için bu sorgulanabilmeyi sağlayan küçük bir detaydır. Bu amaçla Kategoriler, Kitaplar ve Yazarlar isimli yanlız okunabilir özellikler (Read Only Properties) oluşturulmuş ve DukkanEntities sınıfı içerisine dahil edilmiştir. Söz konusu özelliklerin get bloklarında AsQueryable metodunun kullanıldığı hemen dikkat çekmektedir. AsQueryable fonksiyonu, IList tipinden olan sınıf için liste bazlı koleksiyonların IQueryable arayüzü (Interface) tarafından taşınabilmesi için geliştirilmiş bir metoddur.

Sanıyorumki yazıyı okumakta olan herkesin içinde (ve hatta yazmakta olan benim) sistemin nasıl çalışacağına dair yoğun bir merak var. Bu merakı gidermeden önce yapmamız gereken küçük bir iş daha var. Oda Ado.Net Data Service'i WCF Service tipindeki projemize Add New Item seçeneği ile eklemek. (Bu noktada hemen şunu vurgulayalım; Ado.Net Data Service tiplerinin illede bir WCF Service şablonunda olması şart değildir. İstenirse bir Web projesine eklenebilir. Hatta istenirse manuel olarak bir host uygulamada yazılabilir ki bu konuya ilerleyen yazılarımızda değiniyor olacağız.)

![mk259_2.gif](images/mk259_2.gif)

Örneğe DukkanServisleri.svc olarak eklenen Ado.Net Data Service'in kod içeriği ise aşağıdaki gibi geliştirilebilir.

```csharp
using System;
using System.Data.Services;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;

public class DukkanServisleri 
     : DataService<DukkanEntities>
{ 
    public static void InitializeService(IDataServiceConfiguration config)
    {
        config.SetEntitySetAccessRule("*", EntitySetRights.AllRead); 
    }
}
```

Her zamanki gibi servis örneklenirken devreye giren InitializeService metodu içerisinde, gerekli erişim hakları tanımlanır. işaretini kullandığımız için DukkanEntities içerisinde tanımlı IQueryable temelli tüm özellikler dışarıya açılmaktadır. AllRead sabit değeri kullanıldığı içinde, söz konusu tiplerin sadece okuma amaçlı olaraktan dış ortama sunulması sağlanır.

Artık örnekler test edilebilir. Bir önceki makalemizdede belirttiğimiz gibi herhangibir istemci uygulama yazmamıza gerek yoktur (Bu noktada biraz tembellik ettiğimi açıkça belirtebilirim). Basit bir tarayıcı uygulama, örneğin Internet Explorer bizim için yeterlidir. Örnekteki kodları doğru olarak geliştirdiysek eğer, DukkanServisleri.svc için çalışma zamanında aşağıdakine benzer bir çıktının alınması gerekir. Eğer sizlerde eş zamanlı geliştirdiğiniz örneğinizde benzer sonuçları alıyorsanız herşey yolunda gidiyor demektir.

![mk259_3.gif](images/mk259_3.gif)

Sistem çalıştığına göre bir kaç sorgulama denemesi yapılabilir. Aşağıda test amaçlı bir kaç sorgu yer almaktadır.

Örnek 1: Tüm Kategorilerin elde edilmesi.

URL: http://localhost:1304/CustomLinqProvider/DukkanServisleri.svc/Kategoriler

![mk259_4.gif](images/mk259_4.gif)

Örnek 2: İki numaralı kategorideki kitapların elde edilmesi

URL: http://localhost:1304/CustomLinqProvider/DukkanServisleri.svc/Kategoriler (2)/Kitaplar

![mk259_5.gif](images/mk259_5.gif)

Örnek 3: Kitaplar içerisinden en pahalı 3 ünün elde edilmesi

URL: http://localhost:1304/CustomLinqProvider/DukkanServisleri.svc/Kitaplar?$orderby=BirimFiyat desc&$top=3
Burada & kullanılarak birden fazla anahtar kelimenin birlikte kullanımı ele alınmaktadır.

![mk259_6.gif](images/mk259_6.gif)

Örnek 4: Kitaplar ve her kitaba ait Yazar listelerinin elde edilmesi

URL: http://localhost:1304/CustomLinqProvider/DukkanServisleri.svc/Kitaplar?$expand=Yazarlar
expand anahtar kelimesi sayesinde her Kitaba ait Yazar listelerinin çekilmesi ve XML içerisine gömülmesi sağlanır.

![mk259_8.gif](images/mk259_8.gif)

Örnek 5: SicilNo değeri Y100 olan yazarın elde edilmesi

URL: http://localhost:1304/CustomLinqProvider/DukkanServisleri.svc/Yazarlar ('Y100')
Burada SicilNo string bir özellik olduğunda parantezler içerisinde tek tırnak işareti kullanılmıştır.

![mk259_7.gif](images/mk259_7.gif)

Örnek 6: 1 numaralı Kategorideki 8002 numaralı kitabın yazalarının elde edilmesi

URL: http://localhost:1304/CustomLinqProvider/DukkanServisleri.svc/Kategoriler (1)/Kitaplar (8002)/Yazarlar
Bu örnektede özellikler arasında iki kademeli geçiş yapılmaktadır. Kategori'den Kitap'a, Kitap'tan Yazara. Bu noktada sınıflar içerisinde tanımlanan özellikleri hatırlamanızı öneririm.

![mk259_9.gif](images/mk259_9.gif)

Lütfen sizlerde değişik sorgulamaları deneyerek konuyu özümsemeye ve başka neler yapılabileceğini görmeye çalışınız.

Görüldüğü gibi EDM'den bağımsız olarak Ado.Net Data Services üzerinden geliştirme yapmak oldukça kolaydır. Yazımızın başlarındada belirttiğimiz gibi, farklı LINQ Provider geliştirmeleri yapabilir ve bunları Ado.Net Data Services üzerinden REST modeline göre sunabiliriz. Biz makalemizde ele aldığımız örnekte bellek üzerinde tutulan (Memory Based) nesne koleksiyonlarını kullandık. Burada verinin kaynağı XML tabanlı dosyalarda olabilirdi. Size tavsiyem XML üzerinde tutulan verileri özel LINQ Provider yardımıyla bir Ado.Net Data Service üzerinden dışarıya sunmaya çalışmanızdır. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örneği indirmek için tıklayın](assets/CustomLinqProvider.rar)
