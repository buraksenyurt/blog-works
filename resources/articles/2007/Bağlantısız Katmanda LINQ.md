---
title: "Bağlantısız Katmanda LINQ"
pubDate: 2007-04-02 12:00:00
categories:
  - C#
tags:
  - C#
  - .NET
  - ADO.NET
  - LINQ
  - XML
  - Generics
  - DataSet
  - DataTable
---

# Bağlantısız Katmanda LINQ
Değerli Okurlarım Merhabalar,

Language Integrated Query (Dil ile tümleştirilmiş sorgu) yardımıyla yapabileceklerimiz saymakla bitmiyor. Aslında LINQ projesinin en önemli çıkış nedeni, Anders Hejslberg'ın anlatımıyla veri ve nesne eşitsizliğidir. (data!=objects) Bu ifadeyi, TechEd 2006 sunumlarında kullanan Anders Hejslberg, özellikle veri yapılarının programlama ortamına alınması sonrasında, var olan basit sorgu tekniklerinin uygulanamayışından yakınmaktadır.

LINQ projesinin aslında en temel amacı, uygulamaların çalışma alanlarında (.Net perspektifinden baktığımızda Application Domain'ler içerisinde), bellek üzerinde konuşlanan nesneler üzerinden bildiğimiz veri sorgulama kurallarını uygulayabilmektir. Bir başka deyişle, nesne (object) üzerinde, var olan veritabanı nesnelerini taşıyabilen Entity bileşenleri üzerinde, belleğe alınan Xml veri setleri üzerinde, sorgulamaları bilinen alışılagelmiş söz dizimleri ile tek bir standart altında yapabilmektir. Tüm bu farklı nesnel yapıların ortak bir sorgulama dilini kullanabiliyor olması da LINQ projesinin ana fikirlerinden birisidir aslında.

![mk198_1.gif](images/mk198_1.gif)

Yukarıdaki grafikte, LINQ projesinin odaklandığı temel modeller ifade edilmeye çalışılmıştır. LINQ sorguları bildiğiniz gibi bellek üzerinde herhangibir şekilde IEnumerable arayüzünü uyarlamış olan her tür nesne topluluğuna uygulanabilmektedir. Bu nedenle bellek içi nesnelerden (in memory objects), veritabanı (database) bağlantılı Entity nesnelerine kadar pek çok yerde kullanılabilmektedir.

C# 3.0 ve geleceği ile ilgili olarak önceki makalelerimizde, DLINQ, XLINQ modellerini incelemeye çalışmıştık. Bunların yanında LINQ ile yapabileceklerimizi daha derinlemesine kavrayabilmek maksadıyla bol bol sorgu geliştirdik. Bugünkü makalemizde ise, özellikle bağlantısız katman (disconnected layer) nesneleri üzerinde, yani bildiğimiz DataSet ve DataTable nesne örnekleri üzerinde LINQ sorgularını nasıl yazabileceğimizi basit bir şekilde incelemeye çalışacağız. DataSet ve DataTable gibi bileşenler bildiğiniz gibi herhangibir veri kaynağından yüklenen sonuç kümelerini uygulama belleğinde tutmak amacıyla kullanılmaktadır. Ne varki çalışma zamanında, bağlantısız katman nesneleri üzerindeki verilerde sorgulama yapabilmek için çeşitli yollara başvurmamız gerekir. Örneğin bunlardan birisi Select metodudur.

Bir başka teknikte veri kümelerini DataView bileşenlerine alıp filtreleme amacıyla yardımcı fonksiyonellerden faydalanmaktır. LINQ, felsefe olarak yukarıda bahsettiğimiz tüm veri kümeleri için ortak bir sorgulama ortamı sunmaktadır. Öyleyse bağlantısız katman nesneleri içinde bu tekilleştirilmiş sorgulama modelini nasıl ele alabiliriz?Dilerseniz hiç vakit kaybetmeden örneğimize başlayalım. Bu seferki örneğimizi LINQ Windows Application projesi olarak geliştireceğiz. Nitekim, DataTable içeriğini ekranda görsel olarak ele alabileceğimiz bir ortam olayları daha net algılayabilmemizi sağlayacaktır. Elbetteki bu makalede bahsedilen işlemleri gerçekleştirebilmek için sistemimizde LINQ Preview sürümünün yüklü olması gerektiğini unutmayalım.

![mk198_2.gif](images/mk198_2.gif)

Herhangibir DataTable üzerinden LINQ sorguları çalıştırabilmemiz için System.Data.Extensions isimli kütüphanenin program içerisinde referans edilmiş olması yeterlidir. Çalışmakta olduğumuz LINQ Windows uygulaması bu referansı varsayılan olarak içermektedir.

Herşeyden önce uygulamamızın bellek üzerinde DataSet ve DataTable nesnelerine sahip olması gerekiyor. Bu amaçla makalemizde AdventureWorks ve Northwind veritabanlarından yararlanacağız. DataTable nesnelerimizi doldurmak için başvurabileceğimiz iki yol var. Bunlardan birisi, standart Ado.Net tiplerinden ve fonksiyonelliklerinden yararlanmak. Bir başka deyişle, DataAdapter tipi ve Fill metodundan bahsediyoruz.

Ancak bu kez biraz daha farklı olarak entity nesnelerinden faydalanacağız. Hatırlarsanız DLINQ konusunu incelediğimiz makalemizde, bir database ve içerisindeki tablolar için otomatik olarak entity hazırlayabilmemizi sağlayan SqlMetal isimli bir aracın LINQ Preview projesi ile birlikte geldiğinden bahsetmiştik. Bizim için gereken Entity sınıflarını oluşturması için SqlMetal aracını aşağıdaki gibi kullanıp üretilen.cs dosyalarını projemize eklememiz yeterli olacaktır. (SqlMetal aracına, LINQ Preview'u kurduktan sonra, varsayılan olarak D:\Program Files\LINQ Preview\Bin adresinden ulaşabilirsiniz.)

![mk198_3.gif](images/mk198_3.gif)

Dolayısıyla artık entity nesneleri üzerinden DataTable nesne örnekleri içerisine veri doldurma işlemini gerçekleştirebiliriz. Yazacağımız ilk kod parçası, AdventureWorks veritabanı içerisinde yer alan Production şemasındaki Product tablosundan bazı satırların bir DataTable içerisine LINQ sorguları yardımıyla alınması işlemini gerçekleştirecektir. Bu amaçla aşağıdaki kod parçasında olduğu gibi AdventureWorks isimli sınıfımıza ait bir nesne örneği oluşturmamız gerekmektedir.

```csharp
AdventureWorks adWorks = new AdventureWorks("data source=localhost;database=AdventureWorks;integrated security=SSPI");
```

Artık entity sınıflarımıza ait nesne örneklerini, adWorks üzerinden kullanabiliriz. Aşağıdaki metod ile, global olarak tanımladığımız adWorks nesnesini kullanarak, yine global olarak tanımladığımız dtUrunler isimli DataTable isimli nesne örneğine veri doldurma işlemi yapılmaktadır. Biz LINQ sorgumuz içerisinden belirli alanları alıp yeni bir isimsiz tip (anonymous type) olarak çekmekteyiz. Elbetteki bu sorgu içerisinde bildiğimiz tüm LINQ imkanlarını kullanabiliriz. Where, order by gibi ifadeler bunlara örnek olarak verilebilir.

```csharp
private DataTable LoadProductsTable()
{
    var urunler =from prd in adWorks.Production.Product
                        select new {
                                            prd.ProductID
                                            ,prd.Name
                                            ,prd.ListPrice
                                            ,prd.Class
                                            ,prd.SellStartDate
                                            ,prd.SafetyStockLevel
                                            ,prd.StandardCost
                                        };

    DataTable dtUrunler=new DataTable("Urunler");
    dtUrunler=urunler.ToDataTable();
    return dtUrunler;
}
```

Burada ilk olarak adWorks.Production.Product entity nesnesi üzerinden bir LINQ sorgusu çalıştırılmaktadır. Bunun sonucunda elde edilen veri kümesini bir DataTable içerisine aktarmak için ise tek yapılması gereken ToDataTable isimli metodun çağırılmasıdır. (System.Data.Extensions isim alanı, DataTable ve DataRow'lar için LINQ sorguları hazırlanmasını sağlayan pek çok genişletme metodu içermektedir.)

> Kendi örneklerimizi denerken dikkat etmemiz gereken bir nokta vardır. Özellikle null değer alabilen sayısal ve tarihsel formatlı alanlar için LINQ sorguları aşağıdaki ekran görüntüsünde yer alan çalışma zamanı istisnasına neden olabilmektedir. Örneğin Product tablosunda sayısal ve null değer alabilen bir alan olarak tanımlanmış olan ProductSubCategoryID için bu istisna mesajı elde edilmektedir.
> ![mk198_4.gif](images/mk198_4.gif)
> Aynı durum null değerler alabilen varchar, nvarchar tipli alanlar için geçerli değildir. Bunların program ortamı içerisinde yer alan entity sınıfları içerisinde string olarak kullanıldığına ve string'in özellikle referans tipi olduğu için null değer taşıyabildiğine dikkat edelim.

Artık elde ettiğimiz DataTable nesne örneğini herhangibir görsel taşıyıcıya (container) bağlayabiliriz. Bu amaçla.Net 2.0 ile gelen DataGirdView kontrolü biçilmiş kaftandır. Uygulamada bu durumu test etmek için ana formumuzun Load olay metodu içerisinde aşağıdaki örnek kod parçaları yazılmıştır.

```csharp
adWorks = new AdventureWorks("data source=localhost;database=AdventureWorks;integrated security=SSPI");

dtUrunler = LoadProductsTable();
dgUrunler.DataSource=dtUrunler;

label1.Text = "Ürün Sayısı " + (dgUrunler.Rows.Count-1).ToString();
```

Programın çalışması sonucu aşağıdaki ekran görüntüsünü elde ederiz. Dikkat ederseniz Product tablosundan 504 adet ürün bilgisi yüklenmiştir.

![mk198_5.gif](images/mk198_5.gif)

Asıl amacımız elbetteki DataTable nesne örneğini doldurmak değildir. Özellikle şunu tekrar belirtmekte fayda vardır. Örneğimizde Entity tipleri üzerinden veri çekme işlemi yapılmıştır. Pekala bunu DataAdapter yardımıyla da gerçekleştirebiliriz. Ancak asıl yapmak istediğimiz veriyi bağlantısız katmana nasıl aldığımız değil, bellekte veri taşıyan DataTable üzerinden LINQ sorgularını nasıl çalıştırabileceğimizdir. Nitekim LINQ, DataTable veya DataSet içerisine verinin nasıl çekildiği ile ilgilenmez. Bu amaçla örneğin yukarıdaki sonuçları döndüren DataTable bileşenimizin içerisinde üretim tarihi (SellStartDate alanının değeri) bellirli bir zamandan sonra olanları bulmak istediğimizi düşünelim. Söz konusu sorgu için aşağıdaki gibi bir kod parçasını kullanabiliriz.

```csharp
var sorgulanabilirUrunler = dtUrunler.ToQueryable();

var sonuclar=from prd in sorgulanabilirUrunler
                        where prd.Field<DateTime>("SellStartDate")>=dateTimePicker1.Value
                            select new {
                                                ProductID=prd.Field<int>("ProductID")
                                                ,Name=prd.Field<string>("Name")
                                                ,ListPrice=prd.Field<decimal>("ListPrice")
                                                ,Class=prd.Field<string>("Class")
                                                ,SellStartDate=prd.Field<DateTime>("SellStartDate")
                                                ,SafetyStockLevel=prd.Field<short>("SafetyStockLevel")
                                                ,StandartCost=prd.Field<decimal>("StandardCost")
                                            };

dgUrunler.DataSource=sonuclar.ToDataTable();

label1.Text="Ürün Sayısı "+(dgUrunler.Rows.Count-1).ToString();
```

Dikkat edeceğimiz ilk nokta ToQueryable metodunun kullanılmasıdır. Bu metodun tek amacı DataTable üzerinde LINQ sorgularının çalıştırılabilmesini sağlamaktır. Aslında ToQueryable, ToDataTable, Field gibi metodlar, System.Data.Extensions.dll içerisinde gelen genişletme metodlarıdır. Bunları görmek için her hangibir decompiler aracını kullanabiliriz. Örneğiz XenoCode Fox 2007 Community Edition aracı yardımıyla System.Data.Extensions.dll içeriğine bakacak olursak aşağıdaki sonuçları alırız.

![mk198_6.gif](images/mk198_6.gif)

Gördüğünüz gibi DataTable için ToQueryable ve ToDataTable metodları, DataRow tipi için Field metodu vb... yer almaktadır. Field metodu, sorgulanabilir hale getirlmiş olan DataTable içerisindeki DataRow dizileri üzerinden istenen alanın elde edilebilmesi amacıyla kullanılmaktadır. Dikkat ederseniz generic bir metoddur ve tip olarakta, çekilen alanın veri tipini almaktadır. Bu tipin elbetteki doğru girilmesi şarttır. Aksi takdirde derleme zamanı hataları alırız.

Peki, sorgumuz tam olarak ne yapmaktadır? Tahmin edeceğiniz gibi where anahtar kelimesi sayesinde SellStartDate alanının değeri DateTimePicker kontrolünde seçilen tarihten sonra gelen satırlar çekilmektedir. Buradaki where cümlesinde yer alan prd.Field ("SellStartDate")>=dateTimePicker1.Value ifadesinin söz konusu DataTable içerisindeki her bir DataRow için çalıştığını unutmayalım. Bunu daha kolay idrak edebilmek için bu tip bir gereksinimi LINQ olmadan eski usuller ile yazmak istediğinizi düşünün. Tüm satrıları gezeceğimiz bir döngü yazmamız gerektiğini tahmin edebiliriz. Sonuç itibariyle kodumuzu çalıştırdığımızda aşağıdaki veri kümesini elde ederiz.

![mk198_7.gif](images/mk198_7.gif)

Sorgularımızı çeşitlendirebiliriz. Öyleki artık elimizdeki nesne, DataTable üzerinden elde edilmiş sorgulanabilir bir DataRow kümesinden başka bir şey değildir ve LINQ ifadelerine doğrudan destek vermektedir. Şimdi işlemlerimizi biraz daha ilerletelim. Örneğin birbiriyle ilişkili olabilen iki DataTable üzerinde LINQ yardımıyla bir Join işlemi gerçekleştirmeye çalışalım. Bu amaçla Northwind veritabanında yer alan Order ve OrderDetails tablolarından faydalalanbiliriz. Öncelikle bu tabloları entity nesnelerimize alacağız ve sonrasında ise DataTable nesne örneklerine yükleyeceğiz. Son olarakta bu iki DataTable örneğine ait sorgulanabilir bir nesne üzerinden LINQ yardımıyla bir Join işlemi gerçekleştireceğiz. Bu amaçla programımıza aşağıdaki metodları ekleyelim.

```csharp
private DataTable LoadOrdersTable()
{
    var siparisler=from s in north.Orders 
                            select new {
                                                s.OrderID
                                                ,s.ShipAddress
                                                ,s.ShipCity
                                                ,s.ShipRegion
                                                ,s.ShipPostalCode
                                                ,s.ShipCountry
                                            };

    return siparisler.ToDataTable();
}

private DataTable LoadOrderDetailsTable()
{
    var siparisDetaylari=from d in north.OrderDetails 
                                    select new {
                                                        d.OrderID
                                                        ,d.UnitPrice
                                                        ,d.Quantity
                                                    };
        
    return siparisDetaylari.ToDataTable();
}
```

Metodlarımız sırasıyla north isimli global olarak tanımlanmış entity nesnesi üzerinden hareket ederek Orders ve OrderDetails tablolarından belleğe bazı alanlar için veri çekmektedir. Son olarak elde edilen sonuç kümeleri ToDataTable metodu yardımıyla geri döndürülüyor. Şimdi bu iki veri kümesininde OrderID alanları üzerinden birbirlerine bağlı olduğunu biliyoruz. Dolayısıyla birleştirme işlemini gerçekleştireceğimiz sorgu cümesinde bu durumu göz önüne almamız gerekiyor. Bu amaçla aşağıdaki gibi bir kod parçasından faydalanabiliriz.

```csharp
AdventureWorks adWorks;
Northwind north;
DataTable dtUrunler, dtSiparisler, dtSiparisDetaylari;

private void Form1_Load(object sender, EventArgs e)
{
    adWorks = new AdventureWorks("data source=localhost;database=AdventureWorks;integrated security=SSPI");
    north = new Northwind("data source=localhost;database=Northwind;integrated security=SSPI");

    dtUrunler = LoadProductsTable();
    dgUrunler.DataSource = dtUrunler;

    label1.Text = "Ürün Sayısı " + (dgUrunler.Rows.Count - 1).ToString();

    dtSiparisler = LoadOrdersTable();
    dtSiparisDetaylari = LoadOrderDetailsTable();
    
    dgSiparisler.DataSource = dtSiparisler;
    dgSiparisDetaylari.DataSource = dtSiparisDetaylari;
}

private void btnJoin_Click(object sender, EventArgs e)
{
    var sorgulanabilirOrders = dtSiparisler.ToQueryable();
    var sorgulanabilirOrderDetails = dtSiparisDetaylari.ToQueryable();

    var sonuclar=from o in sorgulanabilirOrders
                            join od in sorgulanabilirOrderDetails
                                on o.Field<int>("OrderID") equals od.Field<int>("OrderID")
                                    select new {
                                                        SiparisID=o.Field<int>("OrderID")
                                                        ,BirimFiyat=od.Field<decimal>("UnitPrice")
                                                        ,Miktar=od.Field<short>("Quantity")
                                                        ,Sehir=o.Field<string>("ShipCity")
                                                        ,Ulke=o.Field<string>("ShipCountry")
                                                    };

    dgJoin.DataSource=sonuclar.ToDataTable();
}
```

LINQ mimarisinde kullandığımız Join kalıbını burada da aynen kullanmaktayız. Tek dikkat etmemiz gereken, generic Field metodunu nasıl ele aldığımızdır. o takma adı ile siparişleri tutan sorgulanabilir DataRow nesne dizisini (sorgulanabilirOrders), od takma adı ilede sipariş detaylarını tutan sorgulanabilir DataRow nesne dizisini (sorgulanabilirOrderDetails) ifade etmekteyiz. Buna göre join işlemini OrderID alanları üzerinden gerçekleştiren ifademiz aşağıdaki gibidir. Burada her iki DataRow dizisindeki ilgili alanların eşitliğine göre bir kıstas getirilmektedir.

```csharp
on o.Field<int>("OrderID") equals od.Field<int>("OrderID")
```

Programımızı çalıştırdığımızda aşağıdakine benzer bir ekran görüntüsü ile karşılaşırız. (TabPage'in üst tarafında yer alan iki DataGridView bileşeni, sırasıyla Orders ve OrderDetails bilgilerini göstermektedir.)

![mk198_8.gif](images/mk198_8.gif)

Dilersek join ile yazmış olduğumuz sorgumuza where ile başka kısıtlamalarda katabiliriz. Örneğin, elde edilen sonuç kümesinde Quantity alanının değeri 10' un üzerinde olanları elde etmek için tek yapmamız gereken sorgumuzu aşağıdaki gibi genişletmek olacaktır.

```csharp
var sonuclar=from o in sorgulanabilirOrders
                            join od in sorgulanabilirOrderDetails
                                on o.Field<int>("OrderID") equals od.Field<int>("OrderID")
                                              where od.Field<short>("Quantity")>10
                                    select new {
                                                        SiparisID=o.Field<int>("OrderID")
                                                        ,BirimFiyat=od.Field<decimal>("UnitPrice")
                                                        ,Miktar=od.Field<short>("Quantity")
                                                        ,Sehir=o.Field<string>("ShipCity")
                                                        ,Ulke=o.Field<string>("ShipCountry")
                                                    };
```

Where ifadesinde ilgili alanın değerinin karşılaştırma işlemine tabi tutmak için yine Field generic metodundan faydalandığımızda dikkat edelim.

İstersek join ile yaptığımız birleştirme işlemini, içerisinde DataRelation nesnesi barındıran bir DataSet üzerinden de gerçekleştirebiliriz. Bu sefer devreye üst tablodaki herhangibir satıra bağlı alt satırların getirilmesini sağlayacak GetChildRows isimli bir fonksiyonellik gelecektir. Durumu daha iyi anlayabilmek için aşağıdaki kod parçasını göz önüne alabiliriz.

```csharp
DataSet ds = new DataSet();
ds.Tables.Add(dtSiparisler);
ds.Tables.Add(dtSiparisDetaylari);

DataRelation drOrdToDtl = new DataRelation("OrdToDetails", dtSiparisler.Columns["OrderID"],dtSiparisDetaylari.Columns["OrderID"]);
ds.Relations.Add(drOrdToDtl);

var sorgulanabilirOrders=dtSiparisler.ToQueryable();

var sonuclar=from o in sorgulanabilirOrders
                        from od in o.GetChildRows("OrdToDetails")
                            select new {
                                                SiparisID=o.Field<int>("OrderID")
                                                ,BirimFiyat=od.Field<decimal>("UnitPrice")
                                                ,Miktar=od.Field<short>("Quantity")
                                                ,Sehir=o.Field<string>("ShipCity")
                                                ,Ulke=o.Field<string>("ShipCountry")
                                            };

dgJoin.DataSource=sonuclar.ToDataTable();
```

DataSet içerisinde yer alan, dtSiparisler ve dtSiparisDetaylari isimli DataTable nesnelerinin işaret ettiği veri kümeleri arasındaki ilişkimiz OrderID alanları üzerinden Orders'dan OrderDetails'e doğrudur. Bunu DataSet içerisinde tanımlayan ise Ado.Net'in ilk çıkışından beri bildiğimiz DataRelation nesnesidir. LINQ sorgumuz, bu nesneyi GetChildRows isimli metod içerisinde parametre olarak kullanmaktadır.

Böylece o takma adı ile temsil edilen sorgulanabilirOrders içerisindeki her bir DataRow için bu ilişki kullanılabilmektedir. Bu da doğal olarak, ilişkinin diğer ucunda yer alan siparişe ait detay bilgisinin elde edilebilmesi anlamına gelmektedir. LINQ sorgumuz iki adet from anahtar kelimesi içerdiğinden sonuç doğal olarak bir Join sorgusunun çıktısı ile aynı olacaktır. Uygulamamızı bu haliyle çalıştırdığımızda ilk yazdığımız join sorgusundakine benzer sonuçları elde ederiz.

![mk198_9.gif](images/mk198_9.gif)

DataTable ve DataSet'ler üzerinde ToQueryable, ToDataTable, Field metodları dışında, LoadSequence, DistinctRows, EqualAllRows, UnionRows, IntersectRows, ExceptRows, SetField isimli metodlarda kullanılabilmektedir. Bu metodların temel amacı, DataTable ve DataRow gibi nesneler üzerinde LINQ tekniklerinin daha da genişletilmesini sağlamaktır. Örneğin LoadSequence metodu sayesinde herhangibir sorgu sonucu elde edilen kümeyi bir var olan bir DataTable içerisine ilave edebiliriz. Bu metod ve diğerleri hakkında daha fazla bilgi almak için LINQ dökümantasyonundan faydalanabilirsiniz.

Böylece geldik bir makalemizin daha sonuna. Bu makalemizde, LINQ'yu DataTable gibi bağlantısız katman nesneleri üzerinde nasıl kullanabileceğimizi incelemeye çalıştık. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.

[Örnek Uygulama İçin Tıklayınız.](assets/LINQonDataSets.rar)
