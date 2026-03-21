---
layout: post
title: "NoSQL Maceraları - db4o ile Hello World"
date: 2013-11-12 21:01:00 +0300
categories:
  - nosql
tags:
  - db4o
  - object-database
  - nosql
  - not-only-sql
  - .net
  - csharp
  - java
  - language-integrated-query
  - acid
  - objectmanager-enterprise
  - eclipse
  - store
  - iobjectcontainer
  - poco
---
Eğer bir ülke olsaydı, dünyanın 6ncı büyük ekonomisne sahip olurdu. Bu ifade Amerika’ nın en kalabalık nüfusuna sahip olan Kaliforniya eyaleti için [wikipedia](http://tr.wikipedia.org/wiki/Kaliforniya)’ da yazılmış. Birleşik Devletlerin batı kıyısının bir eyaleti olan Kaliforniya eyaletinde aslında pek çoğumuzun gıpta ederek baktığı şehirler var.

[![GoldenGateBridge-001](/assets/images/2013/GoldenGateBridge-001_thumb.jpg)](/assets/images/2013/GoldenGateBridge-001.jpg)

NBA’ in en ünlü takımlarından olan Lakers’ ın vatanı Los Angeles, uzun bir dönem Kings forması giyen Hido’ nun şehri Sacremento (ki aynı zamanda eyalet başkenti olarak geçiyor), küçüklüğümde sokakları ile bir dedektiflik dizisinin ([The Street of San Franciso](http://www.imdb.com/title/tt0068135/)) adeta başrolü haline gelen San Francisco.

Yazılan bilgilere göre değiştirilemeyen bir anayasa kanunu bulunmaktadır. Buna göre ekonominin %40’ı eğitime ayrılmak zorunda. Bu benim gerçekten çok hoşuma gitti. Tabi şehrin önemli sembolleri de var. Bunlardan birisi de Golden Gate köprüsü. Bu gün işleyeceğimiz konunun hikayesinin baş kahramanı ise, Kaliforniya kökenli bir yazılım firmasına olan Versant.

[Apache Cassandra, RavendDb, Dex, StsDb](https://www.buraksenyurt.com/category/NoSQL) derken sıra geldi nesne veritabanlarından (Object Database) birisi olan db4o’ ya (Database for Objects şeklinde çevirebiliriz sanırım) Object tipinden bir NoSQL (Not-Only SQL) veritabanı olan db4o ürününün, Java ve.Net platformları için sürümleri bulunmaktadır (Hatta Mono desteği de mevcuttur). Aslında %100 Java ile geliştirilmiştir ve açık kaynak Sharpen ile C# diline de çevrilmiştir. Bu da onun popüler olmasını sağlayan unsurlardan bir tanesidir (Yazının hazırlandığı tarih itibariyle.Net Framework 3.5/4.0 ve platformlarını hedef alan versiyonları vardı)

Genel Özellikleri

Kaliforniya ikametli Versant isimli şirketin açık kaynak olarak sunduğu ürün, GPL (Generic Public License) haricinde Commercial olan bir versiyona daha sahiptir. Db4o ürününün genel özelliklerini ise aşağıdaki maddeler ile özetleyebiliriz;

- Nesne sorgulamaları için Query By Example (yani nesne örneğinden yararlanmak suretiyle), Native Queries, Soda ve LINQ seçenekleri mevcuttur.
- Native Query desteği sayesinde kullanılan dile özgü sorgulamalar yapılabilir. Ayrıca Native Query’ ler Type Safety ve Code Injection noktalarında avantaj sağlamaktadır.
- TCP/IP protokolü üzerinden mesajlaşma yoluyla çalışan Client/Server moduna da sahiptir. (Performans ağdaki bandwith hızlarına bağımlıdır ancak sorgular Lazy Query’ ler ile daha performanslı çalışacak hale getirilebilir)
- Java'da yazılmış olmasına karşın.Net için de bir API sunmaktadır.
- Embeded bir veritabanı sistemidir ve bu nedenle uygulamaya ait alan (Application Process) içerisinde çalışır.
- dRS (db4o Replication System) olarak adlandırılan ürün sayesinde RDBMS ile aralarında data transfer,migration gibi operasyonlar gerçekleştirilebilir.
- ACID (Atomicity Consistency Isolation Durability) ilkelerine izin vermektedir. Bu Concurrency vakasında önemli bir özelliktir.
- Visual Studio (2008,2010) ve Eclipse için entegre olabilen ObjectManager Enterprise arabirimine sahiptir.
- Diğer yandan açık kaynak olan proje kodu OpenSource Compatibility License altındadır.

> ObjectManager Enterprise için VS 2008 ve VS 2010 versiyonları için bir destek mevcuttur. Install işlemi sonrası dilerseniz bu eklentiyi ayrıca kurabilirsiniz.
> Visual Studio 2012 tarafında ise ObjectManager Enterprise için bir geliştirme yapılmadığı görülmektedir (Güncel durumu kontrol ediniz) [![db4o_2](/assets/images/2013/db4o_2_thumb.png)](/assets/images/2013/db4o_2.png)

Merhaba Dünya Uygulaması

Şimdi dilerseniz bu nesne modelli NoSQL sistemine basitçe merhaba demeye çalışalım. Bunun için ilk olarak ürünü download etmeliyiz.

> Kurulum sonrası oldukça detaylı bir Tutorial dökümanı da gelmektedir. Bu, özellikle geliştiriciler açısından son derece önemlidir. Offline olarak da yüklenen dokümantasyon zengin örnek içeriklerine sahiptir.
> [![db4o_5](/assets/images/2013/db4o_5_thumb.png)](/assets/images/2013/db4o_5.png)
> Aslında bir ürünün teknik dokümantasyonunun doyurucu olmasının ne kadar önemli olduğunu ifade etmemize gerek yok. Fakat daha önceden araştırdığımız STSdb gibi ürünlerdeki dokümantasyon eksikliğini görünce ister istemez bu konuya takılıyoruz.(Güncel durumlarını kontrol ediniz)
> Tabi ürünün teknik dokümantasyonunun bu denli zengin ve doyurucu olmasının başlıca sebeplerinden birisi Commercial lisans ile kullanan referans firmalardır. Biraz zorlayıcı bir neden oluşturduklarını ifade edebiliriz sanırım.

Download işlemi ardından kurulum gerçekleştirildiğinde, geliştirici bilgisayarında aşağıdaki klasör yapısının oluştuğu gözlemlenebilir.

[![db4o_1](/assets/images/2013/db4o_1_thumb.png)](/assets/images/2013/db4o_1.png)

Çok doğal olarak projelerimizde db4o veritabanı sistemini kullanmak için ilgili dll dosyalarının referans edilmesi yeterlidir. Kurulum sonrası oluşan bin klasörü içerisinde.Net Framework 3.5, 4.0, Compact Framework 3.5, silverlight 3.0 ve 4.0 sürümleri için, ayrı dizinler içerisinde konuşlandırılmış Assembly dosyaları olduğu görülebilir.

Örneğimizi.Net 4.5 tabanlı olacak şekilde Visual Studio 2012 üzerinde basit bir Console uygulaması şeklinde geliştireceğiz. Yazacağımız örnekler göz önüne alındığında projemize Db4objects.Db4o ve Db4Objects.Db4o.Linq assembly’ larının referans edilmesi yeterlidir.

> Nesne veritabanlarının özünde nesnelerin (Object Instance) yer aldığını vurgulamalıyız
>
> ![Smile](/assets/images/2013/wlEmoticon-smile_81.png)

Örnek POCO Tiplerinin Oluşturulması

Yani bize bir kaç örnek POCO (Plain Old CLR Objects) tipi gerekiyor. Bu amaçla Product ve Category isimli aşağıdaki sınıfları ele alabiliriz.

[![db4o_3](/assets/images/2013/db4o_3_thumb.png)](/assets/images/2013/db4o_3.png)

```csharp
class Product 
{ 
    public int ProductId { get; set; } 
    public string Title { get; set; } 
    public decimal ListPrice { get; set; } 
    public int CategoryId { get; set; } 
    public int StockSize { get; set; }

    public override string ToString() 
    { 
        return string.Format("[{0}]-{1} {2} {3} {4}", ProductId, Title, ListPrice, StockSize, CategoryId); 
    } 
}

class Category 
{ 
    public int CategoryId { get; set; } 
    public string Title { get; set; }

    public override string ToString() 
    { 
        return string.Format("[{0}]-{1}", CategoryId, Title); 
    } 
}
```

Nesne Eklemek

İlk olarak nesne ekleme işlemlerinin nasıl yapılabileceğine bir bakalım. Bu amaçla aşağıdaki kod parçasından yararlanabiliriz.

```csharp
private static void AddSomeObjects() 
{ 
    // Pek çok NoSQL sisteminde olduğu gibi, bir context nesnesine ihtiyaç vardır. 
    // OpenFile metodu eğer dosya yoksa üretecek var ise sadece açacaktır 
    // container nesne örneğinin içeriği IObjectContainer arayüzü(interface) tarafından taşınabilmektedir 
   using (IObjectContainer container = Db4oEmbedded.OpenFile(fileName)) 
    { 
        // bir kaç nesne örneği ilave edelim. 
        Product mouse = new Product 
        { 
            ProductId = 100, 
            Title = "Optik fare", 
            CategoryId = 1, 
            ListPrice = 40, 
            StockSize = 12 
        }; 
        container.Store(mouse); // Store metodu object tipi ile çalışmaktadır. Dolayısıyla herhangibir nesneyi ilave edebiliriz.

        container.Store(new Product 
        { 
            ProductId = 101, 
            Title = "Optik Klavye", 
            CategoryId = 1, 
            ListPrice = 34, 
            StockSize = 16 
        } 
        );

        container.Store(new Product 
        { 
            ProductId = 102, 
            Title = "LCD Monitor 22inch", 
            CategoryId = 2, 
            ListPrice = 155, 
            StockSize = 24 
        } 
        );

        container.Store(new Category 
        { 
             CategoryId=1, 
             Title="Çevre üniteleri" 
        } 
        );

        container.Store(new Category 
        { 
            CategoryId = 2, 
            Title = "Monitorler" 
        } 
        ); 
    } 
}
```

Aslında örneğin can alıcı noktaları, IObjectContainer arayüzü (Interface) ve Store metodudur. Store metoduna object tipinden herhangibir nesne örneği parametre olarak geçilebilir. Bu nesneler veri depolama alanına doğrudan yazılacaktır. OpenFile metodu, fiziki dosya yok ise üretecek, var ise sadece açarak hizmete sunacaktır. Gelelim veri çekme/sorgulama kısmına.

> Ekleme işlemleri sonrasında disk üzerinde bir dosya oluşacaktır. Dosya uzantısının özellikle ObjectManager Enterprise aracı açısından önemi vardır. Yab veya db4o şeklinde bir uzantı önerilmektedir.

Veri Çekme

Bu amaçla da aşağıdaki örnek kod parçasını göz önüne alabiliriz.

```csharp
private static void ReadSomeObjects() 
{ 
    using (IObjectContainer container = Db4oEmbedded.OpenFile(fileName)) 
    { 
        #region Bir nesne örneği yardımıyla sorgulamak

        // Özellikleri varsayılan değerlere atanmış bir nesne örneği QBE tekniğine göre sorgulama için kullanılabilir 
        Product refProd = new Product(); 
        // QueryByExample metodu IObjectSet tarafından taşınabilir bir içerik döndürmektedir. 
        // Ancak içerik object tipinden döndüğünden, iterasyon sırasında ilerlenirken nesne örneğinin belirgin özelliklerine erişilemez. 
        IObjectSet resultSet = container.QueryByExample(refProd); 
        foreach (var r in resultSet) 
            Console.WriteLine(r.ToString());

        #endregion

        #region Bir nesnenin tipinden yararlanarak sorgulamak

        Console.WriteLine("\nTüm kategoriler\n");

        // Query<T> metoduna parametre olarak bir nesne tipi verilerek, Storage içerisinde o tipe ait nesne örneklerinin sorgulanması sağlanabilir 
        // Üstelik Query metodu geriye IList<T> döneceğinden, sonuç kümesinde gezinirken T tipinin özelliklerine erişebilmek te mümkündür. 
        IList<Category> categories = container.Query<Category>(typeof(Category)); 
        foreach (var r in categories) 
            Console.WriteLine(r.Title);

        #endregion Bir nesnenin tipinden yararlanarak sorgulamak

        #region Nesnenin bir özelliğinin belirgin bir değerine göre sorgulamak 
        Console.WriteLine("\nKategorisi 2 olan ürünler\n");

        // Böyle bir durumda QBE tekniği kullanılacaksa bir nesne örneği oluşturulmalı ve ilgili kritere konu olan özelliğin/özelliklerin değerleri atanmalıdır 
        resultSet = container.QueryByExample(new Product { CategoryId = 2 }); 
        foreach (var r in resultSet) 
            Console.WriteLine(r.ToString());

        #endregion Nesnenin bir özelliğinin belirgin bir değerine göre sorgulamak

        #region LINQ sorgusu kullanmak

        Console.WriteLine("\nStok değeri 12 birim ve altında olan ürünler\n"); 
        // Dikkat edilmesi gereken noktalardan birisi de Db4Objects.Db4o.Linq assembly' ının referans edilmesi gerekliliğidir. 
        // Diğer yandan using kımsında System.Linq satırının da kaldırılmaması gerekir. 
        var products = from Product p in container.AsQueryable<Product>()    
                       where p.StockSize<=12 
                       select p; 
        foreach (var product in products) 
            Console.WriteLine(product.ToString());

        #endregion LINQ sorgusu kullanmak 
    } 
}
```

Veri sorgulama işleminde kullanılabilen bir kaç yöntem vardır. Bunlardan birisi QBE olarak kısaltılan Query By Example’ dır. Bu tekniğe göre IObjectContainer arayüzü üzerinden çağırılan QueryByExample metoduna bir nesne örneğinin gönderilmesi gerekmektedir. Aranan kriterleri taşıyan bir sınıf örneğini kullanılabilir. Dönüş tipi IObjectSet arayüzü tarafında taşınabilmektedir. Tek dezavantaj, object tipinin dönüşü söz konusu olduğundan belirli nesne özelliklerinin bilinmiyor oluşudur ki bu durum intelli-sense noktasında da kendisini gösterir.

> Burada dikkat edilmesi gereken hususlardan birisi de, Product nesne örneğinin varsayılan yapıcı metod ile oluşturulması halinde, QueryByExample metodunun, veri kümesi içerisindeki tüm Product nesne örneklerini döndürecek olmaslıdır.
> Nitekim QueryByExample çağrısının bir diğer kullanımında yer alan Product nesne örneğinde, CategoryId özelliğine 2 değeri verilmiş ve bu nedenle de 2 numaralı kategoride bulunan Product nesne örneklerinin listesi elde edilmiştir.

Query metodu ise T tipinden IList içeriğinin dönmesine neden olmaktadır. Bu sebepten tip güvenlidir (Type Safe). Parametre olarak, arama kriterine dahil olacak tipi alır. Örnekte Category sınıfının tipi typeof operatörü yardımıyla belirtilmiştir.

Arama işlemlerinde.Net geliştiricilerine daha yakın gelen teknik ise elbette LINQ ifadelerinin kullanıldığı senaryodur. LINQ sorgularında dikkat edilmesi gereken nokta, Db4objects.Db4o.Linq.dll assembly’ ının projeye referans edilmesi gerekliliğidir. Ayrıca, IObjectContainer varsayılan olarak sorgulanabilir bir referans sunmamaktadır. Bu nedenle AsQueryable metoduna başvurulur. LINQ ifadelerinin yazılabilmesi ve derlenmesi içinse mutlaka System.Linq isim alanının using kısmında yer alması gerekir.

Uygulamanın bu kod parçası kullanıldığında aşağıdakine benzer bir ekran görüntüsü ile karşılaşılır.(Nesne ekleme işlemlerinin yapıldığını varsayıyoruz)

[![db4o_4](/assets/images/2013/db4o_4_thumb.png)](/assets/images/2013/db4o_4.png)

Nesne Güncellemek

Yazımıza bir nesnenin/nesne topluluğunun güncelleme işlemini ele alarak devam edelim. Aşağıdaki kod parçasını göz önüne alabiliriz.

```csharp
private static void UpdateSomeObject() 
{ 
    using (IObjectContainer container = Db4oEmbedded.OpenFile(fileName)) 
    { 
        #region tüm ürünlerin fiyatlarını %10 arttıralım

        // Güncelleme işlemi için öncelikle güncellenecek nesne örneği/ örnekleri bulunmalıdır 
        var products = from Product p in container.AsQueryable<Product>() 
                       select p; 
        foreach (var product in products) 
        { 
            product.ListPrice *= 1.10M; 
            // Değişiklik sonrası güncellemenin depolama alanına da yansıması için yine Store metodundan yararlanılabilir 
            container.Store(product); 
        }

        #endregion tüm ürünlerin fiyatlarını %10 arttıralım 
    } 
}
```

> Store metodu ile hem ekleme hem de güncelleme işlemlerinin tek bir satırda yapılabildiği rahatlıkla gözlemlenebilir. Aynı durum silme ve veri çekme operasyonları için de söz konusudur. Bu sebepten db4o, one-line-of-code database olarak da bilinir.

Örnekte tüm ürünlerin liste fiyatlarının %10 arttırılması senaryosu ele alınmıştır. Aslında olay gayet basittir. Güncellenmek istenen nesne veya nesne topluluğu bir şekilde filtrelenir. Bunun için bir önceki kod parçasında yer alan QBE ya da LINQ tekniklerinden yararlanılabilir. Nesnelerin elde edilmesinin ardından ilgili özelliklerin (Properties) değerleri değiştirilir. Sonrasında ise Store metodu kullanılır. Store metodu burada akıllıca davranarak, gelen nesnenin yeni eklenen bir içerik olup olmadığını ve sadece güncellenmesi gerektiğini tespit eder.

Peki ya Nesne Silmek

Pek tabi silme işlemi de son derece kolaydır. Yine bir filtreleme yapılarak silinmek istenen nesne veya nesne kümesinin tespit edilmesi, sonrasında ise IObjectContainer arayüzü üzerinden Delete metodunun çağırılması kafidir. Aynen aşağıdaki kod parçasında görüldüğü gibi.

```csharp
private static void DeleteSomeObjects() 
{ 
    using (IObjectContainer container = Db4oEmbedded.OpenFile(fileName)) 
    { 
        Product product = (from Product p in container.AsQueryable<Product>() 
                           where p.ProductId == 100 
                           select p).FirstOrDefault(); 
        if (product != null) 
            container.Delete(product); 
    } 
}
```

Görüldüğü üzere db4o ile çalışmak son derece kolaydır. Üstelik Visual Studio 2010 ve 2008’ e entegre olabilen ObjectManager Enterprise arabirimini kullanarak, veri içeriklerini görsel anlamda da yönetebiliriz. Bu arabirim yardımıyla veritabanının incelenmesi ve daha da önemlisi görsel olarak sorgulanması mümkündür (Arabirimde Veritabanına bağlantı kurabilmek için şartlardan birisi dosya uzantısının yab ya da db4o olması gerekliliğidir)

Visual Studio 2010 Tarafında ObjectManager Enterprise Kullanımı

Eğer ObjectManager Enterprise başarılı bir şekilde kurulursa, Visual Studio arabiriminin Tools menüsünde yerini alacaktır.

[![db4o_x1](/assets/images/2013/db4o_x1_thumb.png)](/assets/images/2013/db4o_x1.png)

Bir db4o veri tabanına bağlantı kurmak için Connect seçeneği ile açılan Connection Info iletişim kutusu kullanılır. İstenirse Remote bağlantı da yapılabilir.

[![db4o_x2](/assets/images/2013/db4o_x2_thumb.png)](/assets/images/2013/db4o_x2.png)

Açılan pencerelerden db4o Browser’ a gelindiğinde, dosyaya atılmış olan nesnelerin şemasına ulaşılır. Burada, tiplerin özelliklerine ait bilgilere yer verilmektedir.

[![db4o_x3](/assets/images/2013/db4o_x3_thumb.png)](/assets/images/2013/db4o_x3.png)

Diğer yandan her hangibir tipin üstünde sağ tıklanıp Show All Objects seçeneği işaretlenirse, ilgili tipe ait tüm nesne örneklerinin listesine ulaşılır. Hatta bu kısımda özellik değerlerinin değiştirilmesi ve anında güncellenmesi de mümkündür.

[![db4o_x4](/assets/images/2013/db4o_x4_thumb.png)](/assets/images/2013/db4o_x4.png)

İstenirse kendi sorgularımızı da çalıştırabiliriz. Bunun için db4o browser penceresinden bir veya daha fazla özelliğin seçilerek Query Builder penceresine sürüklenmesi yeterlidir (Bunu keşfetmem biraz zamanımı aldı) Örneğin aşağıdaki ekran çıktısına göre, CategoryId değeri 1 olup StokSize değeri 20den küçük olan Product nesnelerinin sorgulanması işlemi gerçekleştirilmektedir.

[![db4o_x5](/assets/images/2013/db4o_x5_thumb.png)](/assets/images/2013/db4o_x5.png)

Görüldüğü üzere Visual Studio veya Eclipse ile entegre çalışan ObjectManager Enterprise yardımıyla veri yönetimi görsel olarak da sağlanabilmektedir.

Bu makalemizde Java ve.Net desteği bulanan nesne veritabanlarından db4o ürününü incelemeye çalıştık. Ürünün daha pek çok önemli yeteneği bulunmakta. Bunun için doyurucu teknik dokümantasyonuna bakmanızı öneririm. NoSQL ürünlerini ilerleyen yazılarımızda da incelemeye devam ediyor olacağız. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Örnek kodlar Visual Studio 2012’ de geliştirilmiş ancak ObjectManager Enterprise için Visual Studio 2010 sürümünden yararlanılmıştır. Üründeki güncelleştirmeleri takip etmenizi öneririm.]

[HelloDb4o.zip (459,81 kb)](/assets/files/2013/HelloDb4o.zip)