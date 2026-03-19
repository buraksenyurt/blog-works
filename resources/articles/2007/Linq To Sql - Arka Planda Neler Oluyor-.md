---
title: "Linq To Sql : Arka Planda Neler Oluyor?"
pubDate: 2007-12-19 10:00:00
categories:
  - LINQ to SQL
tags:
  - LINQ to SQL
  - C#
  - LINQ
  - SQL Server
  - Performance
  - Generics
  - Visual Studio
---

# Linq To Sql : Arka Planda Neler Oluyor?
Değerli Okurlarım Merhabalar,

Veritabanı (Database) nesnelerinin programatik ortamda sınıf gibi tipler (Type) ve metod benzeri üyeler (Members) ile ifade ediliyor olması, bu tiplere ait nesne örnekleri üzerinden sorgulalamalar yapılabilmesi ihtiyacınıda ortaya çıkartmıştır. Bir veritabanı nesnesinin programatik taraftaki karşılığının nesne yönelimli (Object Oriented) bir dilde geliştirilmesi son derece kolaydır. Örneğin bir tablo (Table) göz önüne alındığında, bu tablonun kendisi bir sınıf (Class) olarak tasarlanabilir. Benzer şekilde, tablo içerisindeki alanlar (Fields) sınıf içinde yer alan birer özellik (Property) olarak düşünülebilir.

Basit CRUD (CreateRetrieveUpdateDelete) işlemleri, varlık sınıfı (Entity Class) diyebileceğimiz tipin birer üye metodu olarak düşünülebilir. Tabloda yer alan kolonların bazı niceleyicileri (örneğin Null değer içerip içermedikleri, primary key olup olmadıkları vb...) sınıfın kendisi ve üyeleri için birer nitelik (Attribute) olarak tasarlanabilir. Ne varki bu eşleştirme kolaylığı dışında, programatik tarafta yer alan nesnel yapılar üzerinde, SQL cümlelerine benzer ifadeler ile sorgulamalar yapmak kolay değildir. Nitekim, programatik tarafın SQL benzeri cümelere karşılık gelen fonksiyonellikleri ele alıyor olması gerekmektedir. LINQ (Language Integrated Query) mimariside, temel anlamda programatik tarafta yazılan ifadeleri arka planda metodlar, temsilciler (Delegates) yardımıyla kurduğu bir modele dönüştürmektedir. LINQ'in kullanıldığı alanlar göz önüne alındığında en popüler seçeneklerden biriside LINQ to SQL mimarisidir.

Language INtegrated Query to SQL mimarisi ile, varlık tipleri (Entity Types) üzerinden sorgular çalıştırılabilir. Basit anlamda, nesneler (Objects) üzerinde uygulanabilen LINQ sorguları, SQL tarafına ulaştıklarında ise bildiğimiz sorgu ifadelerine (Query Expressions) dönüşmektedir. Bilinen LINQ operatörlerinin veya metodlarının tamamının SQL tarafına uygulanamadığı veya henüz uygulanamadığı bir gerçektir. Nitekim programatik ortamın esnekliği nedeni ile, örneğin bir dizinin ters çevrilerek elemanları üzerinde döngüsel anlamda ilerlenebilmesinin, SQL tarafında karşılığının bulunması zordur (ki buda LINQ metodlarından olan Reverse fonksiyonelliğinin neden LINQ to SQL üzerinde kullanılamadığınıda açıklamaktadır).

Bu ana fikirlerden yola çıkarak makalemizdeki ana temamızın, SQL ifadelerine çevrilebilen LINQ operatörlerinin veya fonksiyonelliklerinin, arka planda ne şekilde tasarlandıklarını inceleyebilmektir. Bir başka deyişle basit ve karmaşık LINQ cümlelerinin, SQL tarafında ele alınabilen karşılıklarının ne olduklarını tespit edebilmektir. Bu araştırmadaki en büyük yardımcılarımız ise SQL Server Profiler ve Estimated Execution Plan araçları (Tools) olacaktır. SQL Server Profiler aracı kullanılarak, varlık (Entity) nesneleri üzerinde çalıştırılan LINQ ifadelerinin karşılığı olan SQL sorgu cümlelerini görmek mümkün olabilmektedir. Diğer taraftan Estimated Execution Plan aracı sayesinde, LINQ için arka tarafta çalıştırılan bir sorgu cümlesinin icra planının görülmesi ve alternatif ifadeler ile aralarındaki farklar tespit edilerek daha optimal yolların göz önüne alınması sağlanabilir.

Dilerseniz hiç vakit kaybetmeden örneklerimize geçerek devam edelim. Her zamanki gibi Visual Studio 2008 RTM üzerinden örnek kod parçalarımızı çalıştırıyor olacağız. Basit bir Console uygulaması üzerinden ilerlerken AdventureWorks ve Northwind veritabanlarındaki (Database) bazı tabloları kullanıyor olacağız. Bu anlamda AdventureWorks.dmbl ve Northwind.dmbl isimli DataBase Markup Language içeriklerimiz aşağıdaki ekran görüntülerinde yer aldığı gibi olacaktır.

AdventureWorks.dbml;

![mk236_1.gif](images/mk236_1.gif)

AdventureWorks veritabanından örnek sorgulamalar için ProductCategory, ProductSubCategory, Product, SalesPerson ve SalesOrderHeader tabloları ele alınmaktadır.

Northwind.dmbl;

![mk236_2.gif](images/mk236_2.gif)

Northwind veritabanından ise Customer ve Supplier tabloları ele alınmaktadır.

İlk olarak aşağıdaki gibi basit bir LINQ ifadesi ile başlayalım.

```csharp
AdventureWorksDataContext adw = new AdventureWorksDataContext();

var mClassProducts = from prd in adw.Products
                                    where prd.Class == "M"
                                        select prd; 

int mCount = mClassProducts.Count();
double mSumListPrice = mClassProducts.Sum<Product>(prd => (double)prd.ListPrice); 

Console.WriteLine(mCount.ToString());
Console.WriteLine(mSumListPrice.ToString("C2"));
```

İlk olarak AdventureWorksDataContext nesnesi örneklenmektedir. Sonrasında mClassProducts isimli alana atanan ifadede AdventureWorksDataContext içerisinde yer alan Products özelliğinin karşılığı olan generic Table tipi ele alınmaktadır. Buna göre Product nesne örneklerinden, Class özelliklerinin (Properties) değerleri M olanlar seçilmektedir. Daha önceki makalalerdede belirttiğimiz gibi var anahtar kelimesi ile tanımlanmış olan değişkene atanan bu ifade çalışma zamanında hemen yürütülmemektedir. İcra işlemi için bir for iterasyonu olması yada ifade üzerinden örnekteki gibi Aggregate benzeri fonksiyonelliklerin çalıştırılması gerekmektedir. mCount alanının değeri, hazırlanan LINQ ifadesinde Count metodu uygulanarak elde edilmektedir. Bir başka ifadeyle, sınıfı M olan ürünlerin toplam sayısı bulunmaktadır. mSumListPrice alanına atanan değer ilede, sınıfı M olan ürünlerin liste fiyatlarının (ListPrice) toplamı elde edilir. Sonrasında ise bu sonuçlar ekrana yazdırılır. Çalışma zamanında (run time) uygulamanın çıktısı aşağıdaki gibi olacaktır.

![mk236_3.gif](images/mk236_3.gif)

Gelelim arka tarafta çalıştırılan SQL sorgu cümlelerine. SQL Server Profiler aracı kullanılarak yapılan çalışmada Count ve Sum aggregate metodlarının aşağıdaki gibi icra edildiği görülmektedir. Count fonksiyonunun çağırılması sonucu çalışan sorgu şu şekildedir;

```text
exec sp_executesql N'SELECT COUNT(*) AS [value]
                                    FROM [Production].[Product] AS [t0]
                                        WHERE [t0].[Class] = @p0',N'@p0 nvarchar(1)',@p0=N'M'
```

Burada dikkat edilmesi gereken noktalardan birisi Count () ifadesidir. Normal şartlar altında tavsiye edilen yöntemlerden birisi Count (ProductID) tarzında bir kullanım yapılması yönündedir. Bu tarz bir kullanımın performans yönünde avantaj sağladığı bilinmektedir. Nitekim LINQ tarafından gelen ifadeye göre Count (*) şeklinde bir SQL fonksiyonu kullanılmıştır. Diğer tarafan LINQ sorgusunun aşağıdaki gibi değiştirilmesi düşünülebilir.

```csharp
var mClassProducts = from prd in adw.Products
                                    where prd.Class == "M"
                                        select prd.ProductID;
```

Dikkat edileceği üzere burada sadece ProductID alanları seçilmektedir. Bu ifade üzerinden Count metodu kullanılırsa SQL tarafında icra edilen operasyonun değişmediği, bir başka deyişle Count () çağrısı yapıldığı görülür. Sum fonksiyonunun çağırılması sonucu çalışan sorgu ise aşağıdaki gibidir.

```csharp
exec sp_executesql N'SELECT SUM([t1].[value]) AS [value]
FROM (
    SELECT CONVERT(Float,[t0].[ListPrice]) AS [value], [t0].[Class]
        FROM [Production].[Product] AS [t0]
    ) AS [t1]
WHERE [t1].[Class] = @p0',N'@p0 nvarchar(1)',@p0=N'M'
```

Dikkat edilecek olursa burada Sum işlemi için bir iç Select sorgusu daha yürütülmektedir. Aynı amaca yönelik olaraktan aşağıdaki gibi bir sorguda göz önüne alınabilir.

```text
SELECT SUM(ListPrice) AS [Value] 
FROM Production.Product
GROUP BY Class
HAVING Class='M'
```

Burada Group By kullanımı gerçekleştirilmektedir. LINQ'in Sum metodunu neden farklı bir şekilde yorumladığı tartışılabilir. Sonuç itibariyle her iki sorgu cümlesininde beklenen icra planlarına (Esitamted Execution Plan) bakıldığında Products gibi sadece 504 satıra sahip olan küçük bir tabloda çok fazla bir fark olmadığı görülmektedir. Ancak tablo boyutunun artması halinde bu durumun belirgin performans farklılıklarına yol açıp açmayacağına bakılmalıdır. Aşağıdaki ekran görüntülerinde her iki sorgunun icra planlarına ait icra maliyetleri daha net bir şekilde görülmektedir.

![mk236_4.gif](images/mk236_4.gif)

Diğer LINQ to SQL operatörlerini inceleyerek devam edelim. Sıradaki LINQ ifadesinde, Contains metodu ele alınmaktadır. Contains metodu String sınıfına ait bir fonksiyondur. Aşağıdaki kod parçasında yer alan ifadeye göre Contains metodunun görevi, ProductNumber alanında PA hecesi olan Product nesne örneklerinin tespit edilmesinin sağlanmasıdır.

```csharp
var allProducts = from prd in adw.Products
                            where prd.Class == null && prd.ProductNumber.Contains("PA")
                                select prd;

foreach (Product p in allProducts)
{
    Console.WriteLine(p.ProductNumber + " " + p.Name);
}
```

Sorgu ifadesi null değere sahip olan sınıflara ait ürünlerden, ürün numarasında PA hecesi geçenleri elde etmektedir. Yukarıdaki kod parçasını içeren Console uygulaması çalıştırıldığında aşağıdaki sonuçlar alınacaktır.

![mk236_5.gif](images/mk236_5.gif)

Bizim için asıl önem arz eden konu LINQ to SQL ifadesinin SQL sunucusuna nasıl gönderildiğidir. Hemen SQL Server Profiler aracına bakılırsa aşağıdaki sorgu cümlesinin çalıştırıldığı görülebilir.

```text
exec sp_executesql N'SELECT 
    [t0].[ProductID], [t0].[Name]
    , [t0].[ProductNumber], [t0].[MakeFlag]
    , [t0].[FinishedGoodsFlag], [t0].[Color]
    , [t0].[SafetyStockLevel], [t0].[ReorderPoint]
    , [t0].[StandardCost], [t0].[ListPrice]
    , [t0].[Size], [t0].[SizeUnitMeasureCode]
    , [t0].[WeightUnitMeasureCode], [t0].[Weight]
    , [t0].[DaysToManufacture], [t0].[ProductLine]
    , [t0].[Class], [t0].[Style], [t0].[ProductSubcategoryID]
    , [t0].[ProductModelID], [t0].[SellStartDate]
    , [t0].[SellEndDate], [t0].[DiscontinuedDate]
    , [t0].[rowguid], [t0].[ModifiedDate]
FROM [Production].[Product] AS [t0]
WHERE 
    ([t0].[Class] IS NULL) AND ([t0].[ProductNumber] LIKE @p0)'
,N'@p0 nvarchar(4)',@p0=N'%PA%'
```

Dikkat edileceği üzere programatik tarafta yapılan ==null kontrolü SQL tarafında Is Null olarak, Contains metodu ise Like olarak çevrilmiştir. Buradaki Like ifadesine gönderilen %PA% değeri, içerisinde PA hecesi geçenleri ifade etmektedir. Öyleyse Contains metodu yerine örneğin StartsWith fonksiyonu kullanılırsa ne olacağına bakılmalıdır. Bu amaçla LINQ to SQL ifadesini aşağıdaki gibi değiştirdiğimizi düşünelim.

```csharp
var allProducts = from prd in adw.Products
                            where prd.Class == null && prd.ProductNumber.StartsWith("PA")
                                select prd;
```

Bu durumda SQL tarafına gönderilen sorgu cümlesinin içeriğine bakıldığında LIKE anahtar kelimesine atanan parametrenin PA% şeklinde olduğu görülmektedir. Dolayısıyla beklenildiği gibi PA hecesi ile başlayanların tedarik edilmesi için sorguda gerekli düzenleme yapılmıştır.

Bir önceki LINQ to SQL ifadesinde, SQL sunucusu üzerinde çalışan sorguya bakıldığında Product tablosundaki tüm alanların çekildiği görülmektedir. Oysaki çoğu durumda elde edilip veri kümesi Entity üzerine alındığında yanlızca bir kaç alan üzerinde işlem yapılmaktadır. Söz gelimi elde edilen listenin bir GridView üzerinde gösterilmesi istendiğinde tüm alanlar yerine gerekli olanların gösterilmesi tercih edilir. İşte bu noktada isimsiz tiplerin (Anonymous Types) faydası ortaya çıkmaktadır. Buna göre aşağıdaki LINQ to SQL ifadesini ele alalım.

```csharp
var allProducts = from prd in adw.Products
                            where prd.Class == null && prd.ProductNumber.Contains("PA")
                                select new 
                                    { 
                                        prd.Name
                                        , prd.ProductNumber 
                                    };

foreach (var p in allProducts)
{
    Console.WriteLine(p.ToString());
}
```

Bu örnek kod parçasında PA hecesini içeren ve Class alanının değeri null olan Product tiplerindeki Name ve ProductNumber özellikleri kullanılarak yeni bir tip elde edilmektedir. Burada ihtiyaçlar dahilinde isimsiz tipin (Anonymous Type) hangi özellikleri (Properties) içereceği belirlenebilir. Bu örnek kod parçasının çalışma zamanında üreteceği çıktı aşağıdaki ekran görüntüsündekine benzer olacaktır.

![mk236_6.gif](images/mk236_6.gif)

SQL sunucusu tarafına gönderilen sorgu cümlesinin içeriği ise aşağıdaki gibidir.

```text
exec sp_executesql N'SELECT [t0].[Name], [t0].[ProductNumber]
FROM [Production].[Product] AS [t0]
WHERE 
    ([t0].[Class] IS NULL) AND ([t0].[ProductNumber] LIKE @p0)'
,N'@p0 nvarchar(4)',@p0=N'%PA%'
```

Görüldüğü gibi sadece istenen alanların çekilmesi sağlanmaktadır. Buda isimsiz tiplerin LINQ to SQL tarafında oldukça önemli bir rol oynadığını göstermektedir.

LINQ to SQL tarafında kullanılan ilginç fonksiyonelliklerden ikiside Skip ve Take metodlarıdır. Skip metodu parametre olarak verilen değer kadar atlanılmasını, Take metodu ise atlanılan satırdan itibaren kaç satır alınacağını belirtmektedir. Buda basit olarak bir sayfalamanın (Paging) yapılabilmesine olanak sağlamaktadır. Çok doğal olarak metodlar yardımıyla programatik ortamda kolayca uygulanabilen bu tekniğin SQL tarafına aktarılmasında SQL 2005 ile birlikte gelen RowNumber fonksiyonunun önemli bir rolü vardır. Bu metodları daha iyi analiz etmek için aşağıdaki kod parçasını ele aldığımızı düşünelim.

```csharp
var tenCtg = (from cat in adw.ProductSubcategories select cat)
                        .Skip<ProductSubcategory>(5)
                            .Take<ProductSubcategory>(10);

foreach (ProductSubcategory c in tenCtg)
{
    Console.WriteLine("{0} -> {1} ", c.ProductSubcategoryID.ToString(), c.Name);
}
```

Yukarıdaki kod parçasında yer alan LINQ to SQL ifadesine göre, ProductSubCategories koleksiyonu üzerinden ilk satır atlanarak 6ncı satırdan itibaren 10 satırlık bir ProductSubCategory nesne topluluğunun çekilmesi amaçlanmaktadır. Söz konusu kod parçasının çalıştırılmasının sonucu oluşan program çıktısına ait ekran görüntüsü aşağıdaki gibidir.

![mk236_7.gif](images/mk236_7.gif)

Örnek kod parçasında kullanılan LINQ to SQL ifadesi için SQL sunucusu üzerine gönderilen sorgu cümlesi ise aşağıdaki gibi olacaktır.

```text
exec sp_executesql N'SELECT [t1].[ProductSubcategoryID], [t1].[ProductCategoryID], [t1].[Name], [t1].[rowguid], [t1].[ModifiedDate]
FROM (
    SELECT ROW_NUMBER() OVER 
        (ORDER BY 
            [t0].[ProductSubcategoryID], [t0].[ProductCategoryID],
             [t0].[Name], [t0].[rowguid]
            , [t0].[ModifiedDate]) 
        AS [ROW_NUMBER], [t0].[ProductSubcategoryID], [t0].[ProductCategoryID], [t0].[Name], [t0].[rowguid], [t0].[ModifiedDate]
    FROM [Production].[ProductSubcategory] AS [t0]
) AS [t1]
WHERE [t1].[ROW_NUMBER] BETWEEN @p0 + 1 AND @p0 + @p1
ORDER BY [t1].[ROW_NUMBER]',N'@p0 int,@p1 int',@p0=5,@p1=10
```

Dikkat edileceği üzere RowNumber fonksiyonu burada önemli bir rolü üstlenmektedir. Şimdi son kod parçasında aşağıdaki gibi bir ekleme daha yaptığımızı düşünelim.

```csharp
var tenCtg = (from cat in adw.ProductSubcategories select cat)
                        .Skip<ProductSubcategory>(5)
                            .Take<ProductSubcategory>(10);

foreach (ProductSubcategory c in tenCtg)
{
    Console.WriteLine("{0} -> {1}({2}) ",c.ProductSubcategoryID.ToString(),c.Name,c.Products.Count().ToString()); 
}
```

Yapılan değişikliğe göre, 5nci satırdan itibaren alınan 10 ProductSubCategory nesnesinin her biri için Products özelliğinden yola çıkılarak Count değerleride hesaplanmaktadır. Bir başka deyişle her bir alt kategorideki ürünlerin toplam sayılarıda elde edilmektedir. Bu kod parçasının çalışma zamanı (run time) görüntüsü ise aşağıdaki gibidir.

![mk236_8.gif](images/mk236_8.gif)

ProductSubCategory ve Product sınıfları arasında bire çok (one to many) bir ilişki mevcuttur. Bu ilişki programatik taraftada ilgili varlık sınıflarına (Entity Class) yansıtılmaktadır. Bu nedenle bir ProductSubCategory nesne örneği üzerinden Products özelliği ile bağlı olunan Product nesne topluluğuna geçiş yapmak son derece kolaydır. Bu da gönül rahatlığı ile Count gibi metodları kullanıp istediğimiz tarzda sonuçları alabilmemizi olanaklı kılmaktadır. Ne varki Count çağrısı for döngüsü içerisinde, bir önceki sorgudan elde edilen her bir ProductSubCategory nesne örneği için ayrı ayrı yapılmaktadır. Bunun SQL sunucusu üzerinde oluşturacağı sonuç ise şudur; elde edilen her bir ProductSubCategory için, buna bağlı toplam ürün sayısını döndüren bir sorgu cümlesi çalışmaktadır. Aşağıdaki ekran görüntüsünde bu durumun bir kısmı ifade edilmektedir.

![mk236_9.gif](images/mk236_9.gif)

Sorgu ifadelerine dikkat edilecek olursa Count metodu çağırıldığında aslında SQL tarafında bir Count hesabı yapılmamaktadır. Products özelliğine geçildiğinden, o anki ProductSubCategoryID değerine bağlı Product satırları, programatik taraftaki nesne topluluğuna yüklenmektedir. Bu nesneler yüklendikten sonra bildiğimiz koleksiyonlara ait olan Count metodu çalışmakta ve toplam ürün sayıları bu şekilde elde edilmektedir. Hiç bir durumda bu tarz bir yol ile toplam sayıların elde edilmesi tercih edilmemelidir. Görüldüğü gibi gayet masumane olan ama çok işe yaradığı düşünülen basit bir kod parçası arka tarafta son derece fazla sayıda ve yoğun sorgu cümlelerinin çalışmasına neden olmuştur.

SQL tarafında birden fazla tablo üzerinde bir arada işlem yapılması gerektiği durumlarda çoğunlukla join yapılarından yararlanılmaktadır. Aynı yapı bildiğiniz gibi LINQ ile nesneler üzerindede gerçekleştirilebilmektedir. Sıradaki örnekte LINQ to SQL için join kullanımına bakılmaktadır. Bu amaçla aşağıdaki gibi bir kod parçası geliştirdiğimizi düşünelim.

```csharp
var allList = from pc in adw.ProductCategories
                    join psc in adw.ProductSubcategories
                        on pc.ProductCategoryID equals psc.ProductCategoryID
                            join p in adw.Products
                                on psc.ProductSubcategoryID equals p.ProductSubcategoryID
                                    where p.Class == null && p.ListPrice > 100
                                        select new 
                                        { 
                                            CategoryName = pc.Name
                                            , SubCategoryName = psc.Name
                                            , ProductNumber = p.ProductNumber
                                            , ProductName = p.Name 
                                        };

foreach (var prd in allList)
{
    Console.WriteLine(prd.ToString());
}
```

Buradaki kod parçasına göre ProductCategories, ProductSubCategories ve Products özelliklerine bağlı generic Table koleksiyonları join anahtar kelimesi yardımıyla anahtar özellikler üzerinden birleştirilmekte ve yeni bir isimsiz tipe (Anonymous Type) ait nesne topluluğu elde edilmektedir. Bu nesne topluluğu elde edilirken Class değeri null olan ve ürünlerin ListPrice değeri 100' den büyük olanların elde edilmesi sağlanmaktadır. Buna göre uygulamanın ekran çıktısı aşağıdaki gibi olacaktır.

![mk236_10.gif](images/mk236_10.gif)

Söz konusu LINQ to SQL ifadesinin SQL tarafındaki karşılığı ise aşağıdaki gibidir.

```text
exec sp_executesql N'SELECT [t0].[Name] AS [CategoryName], [t1].[Name] AS [SubCategoryName], [t2].[ProductNumber], [t2].[Name] AS [ProductName]
FROM [Production].[ProductCategory] AS [t0]
    INNER JOIN [Production].[ProductSubcategory] AS [t1] ON [t0].[ProductCategoryID] = [t1].[ProductCategoryID]
        INNER JOIN [Production].[Product] AS [t2] ON ([t1].[ProductSubcategoryID]) = [t2].[ProductSubcategoryID]
WHERE ([t2].[Class] IS NULL) AND ([t2].[ListPrice] > @p0)',N'@p0 decimal(33,4)',@p0=100.0000
```

Görüldüğü gibi join kelimeleri SQL tarafında standart inner join muamelesi görmektedir.

Sıradaki LINQ to SQL ifadesinde DateTime yapısının (struct) parçalarından yararlanılmakta olup, bu ayrıştırmanın SQL tarafına nasıl yansıtıldığı incelenmeye çalışılmaktadır. Bu amaçla uygulamaya aşağıdaki kod parçasını eklediğimizi düşünelim.

```csharp
 var result = from p in adw.Products
                    where p.SellStartDate.Month >= 6 && p.SellStartDate.Month <= 12
                        select new 
                                    { 
                                        p.ProductNumber
                                        , p.Name
                                        , p.ListPrice
                                        , p.SellStartDate
                                        , p.SellEndDate 
                                    };

foreach (var p in result)
{
    Console.WriteLine(p.ProductNumber + " " + p.SellStartDate.Month.ToString() + " ");
}
```

Yukarıdaki kod parçasında yer alan LINQ ifadesinde, her bir Product nesne örneğinin SellStartDate özellikleri üzerinden hareket edilerek Month değerlerine bakılmakta ve 6ncı ila 12nci ay arasında olanlar değerlendirilerek yeni bir isimsiz tip (Anonymous Type) içerisinde toplanmaktadır. Örneğe ait çalışma zamanı ekran çıktısı aşağıdaki gibidir.

![mk236_11.gif](images/mk236_11.gif)

Burada merak edilen konu, Month değerlerinin SQL tarafında nasıl ele alınacağıdır. Bu amaçla örnek çalıştırıldıktan sonra SQL Server Profiler aracına bakılırsa, DatePart SQL fonksiyonunun kullanıldığı açık bir şekilde görülebilir.

```text
exec sp_executesql N'SELECT [t0].[ProductNumber], [t0].[Name], [t0].[ListPrice], [t0].[SellStartDate], [t0].[SellEndDate]
FROM [Production].[Product] AS [t0]
WHERE 
    (DATEPART(Month, [t0].[SellStartDate]) >= @p0) 
        AND (DATEPART(Month, [t0].[SellStartDate]) <= @p1)'
,N'@p0 int,@p1 int',@p0=6,@p1=12
```

Böylece SellStartDate alanlarının içeriği DatePart fonksiyonu kullanılaraktan ayrıştırılmakta ve elde edilen Month kısmına görede değer aralığı kontrolü yapılmaktadır.

Bazı durumlarda sorgulanan verinin string bazlı olması halinde karakter tabanlı kontroller yapılmak istenebilir. Söz gelimi B harfi ile başlayan ürünlerin elde edilmesi gibi. Bu gibi durumlarda string bazlı verilerin karakter katarı olduğu programatik tarafta ele alınması gereken bir durumdur. Aşağıdaki kod parçasında hem bu durum ele alınmakta hemde First metodunun kullanımı incelenmektedir.

```csharp
Product result = (from p in adw.Products select p)
                            .First<Product>(prd => prd.Name[0] == 'C');

Console.WriteLine(result.Name + " " + result.ListPrice);
```

Buradaki ifadeye göre, her bir Product nesne örneğinin Name özelliklerinin ilk harflerine bakılmaktadır. İlk harfi C olan ürünlerden ise sadece ilki First metodu yardımıyla elde edilmektedir. Bu işlevselliği gerçekleştirmek için First metodu içerisinde lambda (=>) operatörü kullanılmaktadır. Lambda operatörü sayesinde eşitliğin sol tarafından sağ tarafına o anki Product nesne örneği geçirilmektedir. Eşitliğin sağ tarafında ise o anki Product nesne örneğinin Name özelliğinin ilk harfine bakılmaktadır. Eğer ilk harf C ise o anda üzerinde durulan Product nesne örneği eşitliğin sağından soluna doğru geri döndürülmektedir. Örneğin çalışma zamanındaki ekran çıktısı aşağıdaki gibi olacaktır.

![mk236_13.gif](images/mk236_13.gif)

Söz konusu LINQ to SQL ifadesinin SQL tarafına aktarılan sorgu cümlesi ise aşağıdaki gibidir.

```text
exec sp_executesql N'SELECT TOP (1) 
        [t0].[ProductID], [t0].[Name], [t0].[ProductNumber]
        , [t0].[MakeFlag], [t0].[FinishedGoodsFlag], [t0].[Color]
        , [t0].[SafetyStockLevel], [t0].[ReorderPoint], [t0].[StandardCost]
        , [t0].[ListPrice], [t0].[Size], [t0].[SizeUnitMeasureCode]
        , [t0].[WeightUnitMeasureCode], [t0].[Weight], [t0].[DaysToManufacture]
        , [t0].[ProductLine], [t0].[Class], [t0].[Style], [t0].[ProductSubcategoryID]
        , [t0].[ProductModelID], [t0].[SellStartDate], [t0].[SellEndDate]
    , [t0].[DiscontinuedDate], [t0].[rowguid], [t0].[ModifiedDate]
FROM [Production].[Product] AS [t0]
WHERE 
    UNICODE(CONVERT(NChar(1),SUBSTRING([t0].[Name], @p0 + 1, 1))) = @p1',N'@p0 int,@p1 int',@p0=0,@p1=67
```

Herşeyden önce First metodunun tam karşılığı olarak TOP (1) söz dizimi kullanılmaktadır. Diğer taraftan programatik ortamda [] indeksleyici operatörünü kullanarak string veri tipinin ilk karakterine geçmemiz ve C harfini kontrol etmemizin karşılığı Unicode, Convert, SubString SQL fonksiyonları olmuştur. Burada 67 değerinin C harfine karşılık geldiğini hatırlayalım. Elbette çekilen veri bir Product tipi olduğundan, Product tablsoundaki tüm alanların Select ifadesine alındığı görülmektedir. Daha öncedende belirtildiği gibi, sadece gerekli alanların çekilmesi adına kod tarafında isimsiz metod (Anonymous Method) kullanımına gidilebilir.

LINQ tarafında çoklu seçimlerde (Select Many) yapılabilmektedir. Bu tarz bir kullanıma örnek olarak aşağıdaki kod parçası ele alınabilir.

```csharp
var result = from p in adw.SalesPersons
                    where p.Bonus >= 1000
                        from h in p.SalesOrderHeaders
                            where h.TerritoryID == 1
                                select new 
                                            {
                                                 p.SalesPersonID
                                                , p.Bonus
                                                , h.SubTotal
                                                , h.AccountNumber 
                                            };

foreach (var r in result)
{
    Console.WriteLine(r.ToString());
}
```

Buradaki sorgu ifadesine göre, SalesPersons koleksiyonunda tutulan SalesPerson nesne örneklerinden Bonus özelliklerinin değeri 1000' in üzerinde olanlar alınmaktadır. Sonrasında ise elde edilen kümedeki her bir SalesOrder üzerinden SalesOrderHeaders koleksiyonuna gidilmekte ve bölge değeri 1 olanlar çekilmektedir. Bir başka deyişle Bonus'u 1000' in üzerinde ve sipariş kalemleri 1 numaralı bölgeye doğru yapılmış olan satış personelinin elde edilmesi söz konusudur. Elde edilen veri kümesi değerlendirilerek yeni bir isimsiz tip (Anonymous Type) içerisinde birleştirilmeleri sağlanmaktadır. Örnek kodun çalışma zamanındaki çıktısı aşağıdaki gibi olacaktır.

![mk236_14.gif](images/mk236_14.gif)

Şu aşamada bizim ilgilendiğimiz kısım SQL tarafına gönderilen sorgu cümlesidir. Bu cümlede aşağıdaki şekildedir.

```text
exec sp_executesql N'SELECT [t0].[SalesPersonID], [t0].[Bonus], [t1].[SubTotal], [t1].[AccountNumber]
    FROM [Sales].[SalesPerson] AS [t0], [Sales].[SalesOrderHeader] AS [t1]
        WHERE ([t1].[TerritoryID] = @p0) AND ([t0].[Bonus] >= @p1) 
                        AND ([t1].[SalesPersonID] = [t0].[SalesPersonID])'
,N'@p0 int,@p1 decimal(33,4)',@p0=1,@p1=1000.0000
```

Burada From kelimesinden sonraki kısma bakıldığında SalesPerson ve SalesOrderHeader tablolarının birlikte ele alındıkları görülmektedir.

Gelelim gruplama fonksiyonelliklerinin SQL tarafına nasıl yansıtıldığında. Bu amaçla aşağıdaki örnek kod parçasını göz önüne alıyor olacağız.

```csharp
var result = from p in adw.Products
                    where p.Class != null
                        group p by p.Class into g 
                            select new
                                            {
                                                ClassName = g.Key
                                                ,TotalListPrice = g.Sum<Product>(p => p.ListPrice)
                                            };

foreach (var r in result)
{
    Console.WriteLine("{0} : {1}", r.ClassName, r.TotalListPrice);
}
```

Örnekteki ifadede, Products koleksiyonundaki her bir Product nesnesinin Class özelliklerine göre gruplara ayrılması ve her bir gruba ait ListPrice özelliklerinin toplam değerlerinin bulunması sağlanmaktadır. Bir başka deyişle sınıfları olan ürünlerin sınıflara göre gruplandıklarında, toplam liste fiyatı değerlerinin ne olduğu elde edilmektedir. Bu kod parçasının icra edilmesi halinde, çalışma zamanında aşağıdakine benzer sonuç ortaya çıkmaktadır.

![mk236_15.gif](images/mk236_15.gif)

Görüldüğü gibi ürünler sınıflara göre gruplanmış ve toplam ürün fiyatlarının değerleri elde edilmiştir. Burada çalışan LINQ to SQL ifadesinin SQL tarafına gönderilen karşılığı ise aşağıdaki gibi olacaktır.

```text
SELECT SUM([t0].[ListPrice]) AS [TotalListPrice], [t0].[Class] AS [ClassName]
FROM [Production].[Product] AS [t0]
WHERE [t0].[Class] IS NOT NULL
GROUP BY [t0].[Class]
```

Aslında üretilen SQL cümlesi tam olarak düşündüğümüz şekildedir. Bununla birlikte dikkat edilmesi gereken bir husus vardır. Buda LINQ sorgusundaki where kelimesinin kullanıldığı yerdir. Örnekte, where ifadesi ile seçilen küme üzerinde gruplama yapılmaktadır. Bu nedenle group by kelimesinden önce where kullanılmaktadır. Ancak aynı ifade aşağıdaki haliylede geliştirilebilir.

```csharp
var result = from p in adw.Products
                    group p by p.Class into g
                        where g.Key!=null
                            select new
                                        {
                                            ClassName = g.Key
                                            ,TotalListPrice = g.Sum<Product>(p => p.ListPrice)
                                        };

foreach (var r in result)
{
    Console.WriteLine("{0} : {1}", r.ClassName, r.TotalListPrice);
}
```

Bu sefer gruplanan nesneye ait Key özelliğinin null olup olmadığına bakılmaktadır. Kod bu haliyle çalıştırıldığında da bir önceki ile aynı sonuçların elde edildiği görülebilir. Ne varki SQL tarafına gönderilen ifadeye bakıldığında aşağıdaki sonuçlar ortaya çıkmaktadır.

```text
SELECT [t1].[Class] AS [ClassName], [t1].[value] AS [TotalListPrice]
FROM (
            SELECT SUM([t0].[ListPrice]) AS [value], [t0].[Class]
            FROM [Production].[Product] AS [t0]
            GROUP BY [t0].[Class]
) AS [t1]
WHERE [t1].[Class] IS NOT NULL
```

Sonuç bir öncekinden oldukça farklıdır. Bu kez devreye ek bir alt sorgu cümlesi daha girmektedir. Önce sınıflara göre gruplanmış ürünlerin ListPrice değelerinin toplamları ve sınıf adlarının olduğu küme elde edilmektedir. Sonrasında ise bu küme üzerinden Class değerleri null olmayanlar çekilmektedir. Bu noktada where kelimesinin kod tarafından yerinin değiştirilmesinin önemli olup olmadığına karar vermek gerekebilir. Ancak geliştirilen örneğe ait oluşturulan sorguların icra planlarına (Execution Plan) bakıldığında bir fark olmadığı açıkça görülmektedir.

![mk236_16.gif](images/mk236_16.gif)

LINQ tarafında yer alan enteresan metodlardan biriside Except metodudur. Bu metoddan yararlanılarak belirli bir şartın dışında kalan nesnel kümelerin elde edilmesi sağlanabilir. Örnek olarak aşağıdaki gibi bir kod parçası geliştirdiğimizi düşünelim.

```csharp
var result = (from c in north.Customers select c.City)
                    .Except(from s in north.Suppliers select s.City);

foreach (var r in result)
{
    Console.WriteLine(r);
}
```

Bu örnekte NorthwindDataContext kullanılmaktadır. Buna göre LINQ ifadesinde ilk parantez içerisinde kalan kısımda Customers koleksiyonunda duran Customer nesne örneklerinden City özellikleri çekilmektedir. Except metodunda yazılan ifadede Suppliers tablosunda yer alan Supplier nesne örneklerinden City özelliklerini çekmektedir. Her iki küme bir arada düşünüldüğünde ortaya çıkan sonuç şudur; Customer nesne örneklerinde olup, Supplier nesne örneklerinde bulunmayan City özellikleri elde edilmektedir. Daha düzgün bir ifadeyle, bir başka deyişle SQL'ce düşünüşdüğünde, müşterilerin yaşayıpta tedarikçilerinin bulunmadığı şehir adlarının elde edildiğini söyleyebiliriz. Programın çalışma zamanındaki çıktısı aşağıdaki gibidir.

![mk236_17.gif](images/mk236_17.gif)

Bu tarz bir ihtiyacı SQL tarafında karşılamak için Not In kullanımı tercih edilebilir. LINQ tarafında metod bazlı yazılan bu örnek ise, SQL tarafına aşağıdaki şekilde aktarılmaktadır.

```text
SELECT DISTINCT [t0].[City]
FROM [dbo].[Customers] AS [t0]
WHERE 
    NOT (EXISTS
            (
        SELECT NULL AS [EMPTY]
            FROM [dbo].[Suppliers] AS [t1]
                WHERE (([t0].[City] IS NULL) 
                    AND ([t1].[City] IS NULL)) 
                        OR (([t0].[City] IS NOT NULL) 
                            AND ([t1].[City] IS NOT NULL) 
                                AND ([t0].[City] = [t1].[City]))
            )
        )
```

Burada önemli olan Exists SQL fonksiyonu ile gereken işlevselliğin sağlanmış olmasıdır. Not konulmasının sebebi, Exists ile belirtilen alt sorgudaki koşula uyanların dışarıda bırakılmasını sağlamaktır. Nitekim t0 ve t1 tablolarındaki City değerlerine bakılarak eşit olanların elde edilmesi sağlanırken Except metodu kullanılması nedeniyle bunların dışarıda tutulmasını ancak Not anahtar kelimesi sağlayabilmektedir. Ayrıca hem Suppliers hemde Customers tablosundaki City alanları için detaylı bir Null kontrolü yapılmaktadır.

Makalemize yine enteresan LINQ fonksiyonları ile devam edelim. Bu kez Any ve All isimli metodları incelemeye çalışıyor olacağız. Bu amaçla ilk olarak Any metodunun kullanımına kısaca bakılım.

```csharp
var result = from p in adw.SalesPersons
                        where p.SalesOrderHeaders.Any(soh => soh.SubTotal >= 224356) 
                            select p;

foreach (SalesPerson person in result)
{
    Console.WriteLine("Person Id: " + person.SalesPersonID + " Bonus: " + person.Bonus + " Sales Last Year : " + person.SalesLastYear);
    foreach (SalesOrderHeader header in person.SalesOrderHeaders)
        Console.WriteLine("\t" + header.AccountNumber + " Sub Total: " + header.SubTotal);
}
```

LINQ to SQL ifadesinde SalesPersons koleksiyonundaki her bir SalesPerson çekilmektedir. Bunlara bağlı olan SalesOrderHeaders koleksiyonundaki SalesOrderHeader nesne örneklerinin ise SubTotal değerlerine bakılarak seçim işlemi koşullandırılmaktadır. Any metodunun buradaki görevi ise şudur; SubTotal değerlerinden herhangibiri 224356' nın üzerinde olan satırlar elde edilebilmektedir. Yani, satış personelinin siparişlerine ait SubTotal değerlerinden herhangibiri 224356' nın üzerinde olanların elde edilmesi sağlanmaktadır. Örnek kod parçasının çalışma zamanındaki çıktısı aşağıdaki gibi olacaktır.

![mk236_18.gif](images/mk236_18.gif)

Bu tarz bir işleyiş için SQL tarafı göz önüne alındığında ortaya karmaşık bir sorgu çıkacağı düşünülebilir. Nitekim LINQ to SQL ifadesinin SQL tarafındaki karşılığı aşağıdaki gibidir.

```text
exec sp_executesql N'
SELECT 
    [t0].[SalesPersonID], [t0].[TerritoryID], [t0].[SalesQuota]
    , [t0].[Bonus], [t0].[CommissionPct], [t0].[SalesYTD]
    , [t0].[SalesLastYear], [t0].[rowguid], [t0].[ModifiedDate]
FROM [Sales].[SalesPerson] AS [t0]
    WHERE EXISTS(
                                SELECT NULL AS [EMPTY]
                                FROM [Sales].[SalesOrderHeader] AS [t1]
                                WHERE ([t1].[SubTotal] >= @p0) AND ([t1].[SalesPersonID] = [t0].[SalesPersonID])
    )'
,N'@p0 decimal(33,4)',@p0=224356.0000
```

Dikkat edileceği üzere Exists anahtar kelimesi kullanılarak SubTotal değeri ele alınmakta ve buna uyanların SalesPerson tablosundan çekilmesi sağlanmaktadır. Gelelim All metoduna. Bu sefer Any'den farklı olarak bağlı olunan kümedeki her bir eleman için belirtilen koşulun sağlanmış olma şartı aranmaktadır. Bunu daha net kavrayabilmek için örneğimizi aşağıdaki gibi değiştirelim.

```csharp
var result = from p in adw.SalesPersons
                    where p.SalesOrderHeaders.All(soh => soh.SubTotal >= 80) 
                        select p;

foreach (SalesPerson person in result)
{
    Console.WriteLine("Person Id: " + person.SalesPersonID + " Bonus: " + person.Bonus + " Sales Last Year : " + person.SalesLastYear);
    foreach (SalesOrderHeader header in person.SalesOrderHeaders)
        Console.WriteLine("\t" + header.AccountNumber + " Sub Total: " + header.SubTotal);
}
```

Bu kodun çalışma zamanı çıktısı ise aşağıdaki gibi olacaktır.

![mk236_19.gif](images/mk236_19.gif)

Bu sefer bir SalesOrder'ın bağlı olduğu SalesOrderHeaders koleksiyonundaki her bir SalesOrderHeader nesne örneğinin SubTotal değerlerinin her biri 80' in üzerinde olanların elde edilmesi sağlanmaktadır. Bir başka deyişle bir SalesOrder üzerinden ulaşılan nesne topluluğunda n tane SalesOrderHeader olduğu düşünülecek olursa, bunların her birine ait SubTotal özelliklerinin değerlerinin 80 ve üzerinde olma şartı konulmaktadır. Söz konusu All metodu için SQL tarafında üretilen çıktı ise aşağıdaki gibidir.

```text
exec sp_executesql N'
SELECT 
    [t0].[SalesPersonID], [t0].[TerritoryID], [t0].[SalesQuota]
    , [t0].[Bonus], [t0].[CommissionPct], [t0].[SalesYTD]
    , [t0].[SalesLastYear], [t0].[rowguid], [t0].[ModifiedDate]
FROM [Sales].[SalesPerson] AS [t0]
WHERE NOT (
                        EXISTS(
                                        SELECT NULL AS [EMPTY]
                                        FROM [Sales].[SalesOrderHeader] AS [t1]
                                        WHERE ((
                                                            (CASE 
                                                                WHEN [t1].[SubTotal] >= @p0 THEN 1 ELSE 0
                                                            END)) = 0) 
                                                            AND ([t1].[SalesPersonID] = [t0].[SalesPersonID])
))'
,N'@p0 decimal(33,4)',@p0=80.0000
```

Bu kez koşulun kontrolü için Case ifadesinden yararlanılmakta ve SubTotal 80' üzerinde ise 1, değilse 0 değeri Where ifadesine katılarak 0 olanların çekilmesi sağlanmaktadır. Yanlız burada yine Not Exist kullanıldığında dikkat etmekte yarar vardır. Buna görede bir Where ifadesinin SalesPerson tablosu için üretilmesi sağlanmaktadır.

Örneklerimize Concat metodu ile devam edelim. Concat metodunu daha çok iki farklı sonuç kümesindeki belirli özelliklerin bir arada ele alınmasını istediğimiz durumlarda göz önüne alabiliriz. Bu bir anlamda iki string'in birleştirilemesine benzer bir durumdur. Tabi şu anda söz konusu olan string değil IEnumerable gibi referanslardır. Concat metodunun SQL tarafında ürettiği çıktıya bakmak için aşağıdaki örnek kod parçasını geliştirdiğimizi düşünelim.

```csharp
var result = (from cust in north.Customers select new { cust.Country, cust.City })
                    .Concat(from supl in north.Suppliers select new { supl.Country, supl.City });
foreach (var r in result)
{
    Console.WriteLine(r.Country + ":" + r.City);
}
```

Bu örnekte NorthwindDataContext tipi kullanılmakta olup Customers ve Suppliers koleksiyonlarındaki nesnelerde Country ve City değerleri birleştirilip çekilmektedir. Sonuçta kodun çalışma zamanı çıktısı aşağıdaki gibi olacaktır.

![mk236_20.gif](images/mk236_20.gif)

Elbetteki SQL tarafına bakıldığında Concat metodunun aşağıdakine benzer bir dönüşüme uğradığı görülmektedir.

```text
SELECT [t2].[Country], [t2].[City]
FROM (
            SELECT [t0].[Country], [t0].[City]
            FROM [dbo].[Customers] AS [t0]
                UNION ALL
            SELECT [t1].[Country], [t1].[City]
            FROM [dbo].[Suppliers] AS [t1]
) AS [t2]
```

Açıkça Union All kullanılaraktan iki Select ifadesinin birleştirildiği ve elde edilen küme üzerinden Country ile City alanlarına ait değerlerin çekildiği söylenebilir. Örnekte dikkati çeken noktalardan biriside tekrarlı alanların olmasıdır. Örneğin ekran çıktısının üst taraflarına bakıldığında iki adet Mexico kentinin olduğu London'un iki kere geçtiği rahat bir şekilde görülebilir. Çok doğal olarak tekrarsız bir listenin elde edilmesi istendiğinde kod tarafında Distinct metodunun kullanılıyor olması yeterli olacaktır. Yani kod parçasında aşağıdaki değişikliğin yapılması yeterlidir.

```csharp
var result = (from cust in north.Customers select new { cust.Country, cust.City }).Concat(from supl in north.Suppliers select new { supl.Country, supl.City }).Distinct();
```

Bu durumda programın çıktısı aşağıdaki gibi olacaktır.

![mk236_21.gif](images/mk236_21.gif)

Diğer taraftan Distinct metodunun kullanılması sonrasında SQL tarafına gönderilen sorgu cümlesinde ise Distinct anahtar kelimesinin kullanıldığıda aşikardır.

```text
SELECT DISTINCT [t3].[Country], [t3].[City]
FROM (
            SELECT [t2].[Country], [t2].[City]
            FROM (
                        SELECT [t0].[Country], [t0].[City]
                        FROM [dbo].[Customers] AS [t0]
                            UNION ALL
                        SELECT [t1].[Country], [t1].[City]
                        FROM [dbo].[Suppliers] AS [t1]
            ) AS [t2]
) AS [t3]
```

Ancak burada bir öncekinden farklı bir sorgunun oluştuğuda gözlerden kaçmamalıdır. Bu kez iç içe alınmış Select sorgusu söz konusudur. Oysaki en dışta yer alan Select kullanımına gerek yoktur. Çünkü Distinct anahtar kelimesi içerideki sorgu cümlesine eklenerekte aynı sonuçların alınması sağlanabilir. Ne varki sorgu cümlesini bu şekilde oluşturup SQL tarafına gönderen LINQ to SQL mimarisidir.

Yine ilginç bir LINQ metodu ve SQL karşılığı ile devam edelim. Bu kez iki farklı veri kümesinin kesişimlerinin elde edilmesinde kullanılabilen Intersect metodu üzerinde duracağız. Bu metodun analizi için aşağıdaki gibi bir kod parçası geliştirdiğimizi düşünelim.

```csharp
var result = (from c in north.Customers select c.City)
                    .Intersect(from s in north.Suppliers select s.City);

foreach (var r in result)
{
    Console.WriteLine(r);
}
```

Öncelikli olarak ilk parantezler arasında Customers koleksiyonundaki her bir Customer nesnesinin City değerleri çekilmektedir. İkinci parantez içerisinde yapılanda benzerdir. Tek farkı Suppliers koleksiyonu için çalışmakta olmasıdır. Intersect metodunun burada getridiği kolaylık ise şudur. Customers ve Suppliers koleksiyonlarında bulundan ortak City özelliklerinin elde edilmesini sağlamaktadır. Bir başka deyişle yine SQL'ciler gibi konuşacak olursak, müşterilerin ve tedarikçilerin bir arada bulunduğu şehirlerin elde edilmesi sağlanmaktadır. Buna göre örneğin çalışma zamanındaki ekran çıktısı aşağıdaki gibi olacaktır.

![mk236_22.gif](images/mk236_22.gif)

Bu sonucun elde edilmesi için arka planda çalıştırılan SQL cümlesi ise aşağıdaki gibidir.

```text
SELECT DISTINCT [t0].[City]
FROM [dbo].[Customers] AS [t0]
WHERE EXISTS(
                            SELECT NULL AS [EMPTY]
                            FROM [dbo].[Suppliers] AS [t1]
                            WHERE 
                                (([t0].[City] IS NULL) 
                                    AND ([t1].[City] IS NULL)) 
                                        OR (([t0].[City] IS NOT NULL) 
                                            AND ([t1].[City] IS NOT NULL) 
                                                AND ([t0].[City] = [t1].[City]))
)
```

Çalıştırılan SQL sorgusu, LINQ tarafında Except metodu kullanıldığı zamankine benzerdir. Tek fark burada kesişim kümesinin bulunması gerektiğinden Not Exists kullanılmamış olmasıdır.

Makalemizde son olarak?: operatörünün kullanıldığı bir durumu ele almaya çalışıyor olacağız. Bu operatör şu aşamada LINQ'e bağımlı olmayan C# programlama dilinin ilk versiyonundan beri var olan bir araçtır. Bu tip bir operatörün LINQ ifadesi içerisinde kullanılması haline SQL tarafında oluşacak olan cümlelere bakmaya çalışıyor olacağız. Bu amaçla aşağıdaki kod parçasını geliştirdiğimizi düşünelim.

```csharp
var result = from prd in adw.Products
                    select new
                                    {
                                        prd.Name
                                        ,prd.SafetyStockLevel
                                        ,LevelOk = prd.SafetyStockLevel >= 50 ? "Seviye İyi" : "Seviye Düşük"
                                    };

foreach (var p in result)
{
    Console.WriteLine(p.Name + " | " + p.SafetyStockLevel + " | " + p.LevelOk);
}
```

Bu kod parçasında kullanılan LINQ ifadesine bakıldığında, LevelOk isimli isimsiz tip özelliğinin değerinin SafetyStockLevel özelliğinin değerine göre belirlendiği görülmektedir. SafetyStockLevel özelliğinin değerinin 50 ve üzerinde olması halinde LevelOk özelliğine Seviye İyi değeri atanmaktadır. Aksi durumda ise Seviye Düşük değeri atanmaktadır. Kodun çalışma zamanındaki çıktısı aşağıdaki gibi olacaktır.

![mk236_23.gif](images/mk236_23.gif)

SQL tarafına baktığımızda ise aşağıdaki sorgu cümlesinin çalıştırıldığı görülmektedir.

```text
exec sp_executesql N'SELECT [t0].[Name], [t0].[SafetyStockLevel], 
    (CASE 
        WHEN [t0].[SafetyStockLevel] >= @p0 THEN CONVERT(NVarChar(12),@p1) ELSE @p2 END
    ) 
    AS [LevelOk]
FROM [Production].[Product] AS [t0]'
    ,N'@p0 int,@p1 nvarchar(10),@p2 nvarchar(12)',@p0=50,@p1=N'Seviye İyi',@p2=N'Seviye Düşük'
```

Dikkat edileceği üzere, LevelOk alanının elde edilmesi sırasında Case When SQL ifadesi kullanılmaktadır.

Buraya kadar anlatılan örneklerde LINQ operatörlerinden veya metodlarından bir kısmının SQL tarafına nasıl aktarıldıkları incelenmeye çalışılmıştır. Diğer taraftan makalemizin başındada belirtildiği üzere programatik tarafta kullanılan her tür LINQ operatörü veya fonksiyonunun SQL tarafına aktarılmasıda mümkün değildir. Söz gelimi aşağıdaki kod parçasını ele aldığımızı düşünelim.

```csharp
var result = (from ctg in adw.ProductSubcategories select ctg)
                   .TakeWhile<ProductSubcategory>(sCtg => sCtg.Name[0] == 'A');

foreach (ProductSubcategory sc in result)
{
    Console.WriteLine(sc.Name + " " + sc.ProductSubcategoryID.ToString());
}
```

Bu kod parçası yürütülmek istendiğinde çalışma zamanında (run time), aşağıdaki ekran görüntüsündende izlenebileceği gibi NotSupportedException tipinden bir istisna (Exception) alınmaktadır.

![mk236_12.gif](images/mk236_12.gif)

Nitekim TakeWhile metodunun SQL tarafında bir karşılığı yoktur. TakeWhile gibi SkipWhile, Last, ElementAt, Reverse gibi pek çok metodunda SQL tarafında karşılığı bulunmadığından desteklenmemektedirler.

Sonuç olarak programatik tarafta varlık katmanı (Entity Layer) üzerinde işlemlerimizi oldukça kolaylaştıran ve nesneler üzerinde sorgular çalıştırabilmemizi sağlayan LINQ to SQL'in gücü ortadadır. Ne varki performansın öne geçmesi gereken durumlarda, yazılan LINQ ifadelerinin arka planda oluşturduğu SQL çıktıları değerlendirilmeli ve en doğru şekilde kullanılmalarına gayret edilmelidir. Zaten zaman içerisinde benzer vakalar için en uygun LINQ söz dizimlerinin ne olacağı daha net bir şekilde ortaya çıkacaktır. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](assets/DahaFazlaLINQSorgusu2.rar)
