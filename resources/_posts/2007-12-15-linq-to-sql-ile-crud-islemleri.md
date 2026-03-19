---
layout: post
title: "LINQ to SQL ile CRUD İşlemleri"
date: 2007-12-15 08:00:00 +0300
description: "Language Integrated Query(LINQ) mimarisi özellikle programatik ortamlarda tasarlanan nesneler üzerinde, SQL cümlelerine benzer ifadeler ile sorgulamalar yapılmasına izin vermektedir. Çok doğal olarak veritabanı(database) tarafında yer alan tablo(Table), saklı yordam(Stored Procedure), görünüm(View), fonksiyon(Function) gibi unsurlarında programatik tarafta birer varlık(Entity) olarak ifade edilebilmesi, LINQ kurallarının SQL üzerindede gerçekleştirilebilmesini sağlamaktadır. Burada varlık katmanı(Entity Layer) olarakda düşünebileceğimiz yapı üzerinde yer alan nesneler, veritabanından çekilen sonuçları saklayabilmektedir. Bunun yanında programatik ortamdaki varlıklar üzerinde yeni varlık oluşturma, güncelleme, silme gibi operasyonlarda yapılabilmektedir."
categories:
  - linq-to-sql
tags:
  - linq-to-sql
  - csharp
  - dotnet
  - linq
  - http
  - concurrency
  - transactions
  - generics
  - visual-studio
---
# LINQ to SQL ile CRUD İşlemleri
Değerli Okurlarım Merhabalar,

Language Integrated Query (LINQ) mimarisi özellikle programatik ortamlarda tasarlanan nesneler üzerinde, SQL cümlelerine benzer ifadeler ile sorgulamalar yapılmasına izin vermektedir. Çok doğal olarak veritabanı (database) tarafında yer alan tablo (Table), saklı yordam (Stored Procedure), görünüm (View), fonksiyon (Function) gibi unsurlarında programatik tarafta birer varlık (Entity) olarak ifade edilebilmesi, LINQ kurallarının SQL üzerindede gerçekleştirilebilmesini sağlamaktadır. Burada varlık katmanı (Entity Layer) olarakda düşünebileceğimiz yapı üzerinde yer alan nesneler, veritabanından çekilen sonuçları saklayabilmektedir. Bunun yanında programatik ortamdaki varlıklar üzerinde yeni varlık oluşturma, güncelleme, silme gibi operasyonlarda yapılabilmektedir. İşte bu makalemizde çoğunlukla CreateRetrieveUpdateDelete (CRUD) işlemleri olarak belirtilen bu operasyonları nasıl yapabileceğimizi, adım adım basit örnekler üzerinden incelemeye çalışıyor olacağız. (Bu makalede geliştirilmekte olan örnek kod parçaları Visual Studio 2008 RTM ortamında yazılmıştır.)

İlk olarak örnek bir SQL veritabanındaki tablo (table) yapılarını programatik ortamda taşıyacak olan sınıfların üretilmesi gerekmektedir. Bu amaçla Visual Studio 2008 üzerinde basit bir Console uygulaması açarak ilerleyebiliriz. Burada dikkat edilmesi gereken önemli noktalardan birisi New Project seçimi sonrası karşımıza çıkacak olan iletişim penceresinden.Net Framework 3.5 versiyonunun işaretlenmiş olmasıdır.

![mk235_1.gif](/assets/images/2007/mk235_1.gif)

Bu şart olmamakla birlikte, LINQ için gerekli olan assembly'ların (örneğin System.Data.DataSetExtensions gibi) otomatik olarak referans edilmesini sağlamaktadır. Bu adımdan sonra entity sınıflarının kolay bir şekilde oluşturulmasını sağlayan LINQ To SQL Class öğesini projemize eklememiz gerekmektedir.(LINQ to SQL sınıflarının oluşturulması ile ilişkili detaylı bilgiyi C#Nedir? yer alan [görsel](http://www.csharpnedir.com/videoindir.asp?id=71) dersten öğrenebilirsiniz.)

![mk235_2.gif](/assets/images/2007/mk235_2.gif)

Böylece veritabanı üzerindeki nesnel yapıları programatik ortamda ifade edebileceğimiz Database Markup Language (dbml) dosyası otomatik olarak oluşturulmaktadır. Adventure.dbml dosyasnın kod tarafına bakıldığında DataContext tipinden türetilmiş olan bir sınıfın yazıldığı görülmektedir. Şimdi yapmamız gereken, üzerinde işlemler gerçekleştirilecek olan veritabanı nesnelerini tasarım ortamına sürükleyip bırakmaktır. İlk etapta örneğin basit olması açısından sadece Production şemasında (schema) yer alan ProductCategory isimli tablo tasarım ortamına, Server Explorer pencersinden sürüklenmiştir.

![mk235_3.gif](/assets/images/2007/mk235_3.gif)

Bu basit operasyonun ardından aşağıdaki sınıf diagramında (Class Diagram) olduğu gibi ProductCategory için bir tipin yazıldığı ve bununla ilişkili olaraktanda, DataContext tipinden türeyen AdventureDataContext sınıfı içerisine ProductCategories isimli bir özelliğin (Properties) atıldığı görülmektedir. ProductCategories özelliği generic Table tipinden bir nesne referansını işaret etmektedir. Bu generic tip tahmin edileceği üzere ProductCategory tabosundaki tüm içeriği taşıyan sınıftır.

![mk235_4.gif](/assets/images/2007/mk235_4.gif)

Artık bu noktadan sonra AdventureDataContext sınıfına ait nesne örneklerini kullanarak kategorilerin elde edilmesi, sorgulanması, yeni kategorilerin oluşturulması (Insert), var olanlardan bir veya bir kaçının silinmesi (Delete) yada güncellenmesi (Update) gibi işlemler kolaylıkla yapılabilir. İlk olarak yeni bir kategoriyi nasıl ekleyebileceğimizi örnek bir kod parçası üzerinden incelemeye çalışalım. Bu amaçla Main metodu içerisine aşağıdaki kod satırlarını eklediğimizi düşünelim.

```csharp
AdventureDataContext adwContext = new AdventureDataContext();

ProductCategory tools = new ProductCategory() 
                                                                        { 
                                                                            Name = "Tools" 
                                                                            , ModifiedDate=new DateTime(2001,1,1)
                                                                        };

adwContext.ProductCategories.InsertOnSubmit(tools);
```

İlk olarak AdventureDataContext tipine ait bir nesne örneği oluşturulmaktadır. Bu işlemin arkadasındanda eklenmek istenen ProductCategory nesne örneği C# 3.0 ile birlikte gelen nesne başlatıcılarından (Object Initializers) yararlanılarak örneklenmektedir. Bu noktada Table generic tipinden olan ProductCategories sınıfının InsertOnSubmit metodu, koleksiyonuna ilave edilmek üzere yeni bir ProductCategory nesne örneğinin eklenmesi için kullanılmaktadır. Burada üzerinde durulması gereken önemli bir nokta vardır. InsertOnSubmit metodu sadece Table koleksiyonuna ilave edilmek üzere bir nesne eklemektedir. Bir başka deyişle ProductCategories koleksiyonu üzerinde yapılacak bir for döngüsünde eklenen nesne (ler) görülmeyecektir. Bunu test etmek için aşağıdaki gibi bir kod parçasını ele alabiliriz.

```csharp
var categories = from category in adwContext.ProductCategories
                            select category.Name; 

foreach(string ctgr in categories)
    Console.WriteLine(ctgr);
```

Bunun sonucu olarak çalışma zamanında (run time) aşağıdaki ekran görüntüsünü elde ederiz.

![mk235_5.gif](/assets/images/2007/mk235_5.gif)

Burada sebep son derece açıktır. var anahtar kelimesinden (keyword) sonra foreach döngüsünün iterasyona başlamasıyla birlikte SQL sunucusu üzerinde bir select sorgusu çalışamaktadır. Bu sorgu SQL Profiler yardımıyla kolay bir şekilde yakalanabilir. Aşağıdaki ekran görüntüsünde bu durum görülmektedir.

![mk235_6.gif](/assets/images/2007/mk235_6.gif)

Dolayısıyla eklenmek (Insert) üzere ilave edilen ProductCategory nesne örneği henüz veritabanına doğru gönderilmemiştir. Bu işlemin nasıl yapıldığını görmeden önce koleksiyona eklenen ve insert işlemi için sırada bekleyen nesne örneklerini nasıl elde edebileceğimize bakalım. Burada DataContext sınıfına ait GetChangeSet metodundan yararlanılmaktadır. Bu metod ile elde edilen ChangedSet nesne örneği üzerinden Inserts,Deletes veya Updates özellikleri (Properties) kullanılarak eklenen, silinen veya güncellenen örnekler yakalanabilir.

![dikkat.gif](/assets/images/2007/dikkat.gif)
Inserts, Deletes ve Updates özellikleri geriye IList tipinden bir referans döndürmektedir. Bu referans bilgisinden yararlanılarak eklenen, silinen veya güncellenen tüm nesnelerin yakalanması mümkündür. IList arayüzü (Interface).Net 2.0 versiyonundan beri mevcuttur. Ayrıca IEnumerable arayüzünden türemiş olduğundan, elde edilen referans üzerinde LINQ sorgularıda yazılabilir. Bu sayede eklenen, silinen veya güncellenen nesnelerin veritabanına yazılmadan önce sorgulanmalarıda mümkün olmaktadır.

![mk235_7.gif](/assets/images/2007/mk235_7.gif)

Örnek uygulamada bu kod parçası denenirse eklenen tool isimli yeni kategorininde elde edildiği görülecektir.

![mk235_8.gif](/assets/images/2007/mk235_8.gif)

Çok doğal olarak ProductCategoryID gibi alanlar veritabanı üzerindeki tabloda otomatik olarak üretildiklerinden ve kod içerisinde herhangibir değer atanmadığından henüz oluşturulmamıştır. Yapılan bu ekleme (Insert) işleminin veritabanına gönderilmesi için tek yapılması gereken ise DataContext tipinin SubmitChanges metodunu çalıştırmaktır.

```csharp
adwContext.SubmitChanges();
```

SubmitChanges metodu çalıştırıldığında Insert, Update veya Delete kuyruğunda bekleyen tüm nesneler için gerekli SQL sorguları (Queries) yürütülmektedir. Söz gelimi yukarıdaki örneğe göre SQL Profiler ile arkada çalışan kodlar izlenirse aşağıdaki sonuçların elde edildiği görülecektir.

```text
exec sp_executesql N'INSERT INTO [Production].[ProductCategory]([Name], [rowguid], [ModifiedDate])
VALUES (@p0, @p1, @p2)

SELECT CONVERT(Int,SCOPE_IDENTITY()) AS [value]',N'@p0 nvarchar(5),@p1 uniqueidentifier,@p2 datetime',@p0=N'Tools',@p1='00000000-0000-0000-0000-000000000000',@p2='2001-01-01 00:00:00:000'
```

Bu çalışan sorgu (Query) basit olarak üretilen varlık nesnesinin (Entity Object) veritabanına doğru yazılmasını sağlamaktadır. İstenirse toplu olarak veri ekleme işlemleride gerçekleştirilebilir. Bunun için tek yapılması gereken InsertAllOnSubmit metodunu kullanmaktadır. Aşağıdaki örnek kod parçasında toplu bir ekleme işleminin nasıl yapılabileceği gösterilmektedir.

```csharp
adwContext.ProductCategories.InsertAllOnSubmit(
                                                            new List<ProductCategory>()
                                                                {
                                                                    new ProductCategory(){Name="Kategori X", ModifiedDate=new DateTime(2006,12,3),rowguid=Guid.NewGuid()}
                                                                    ,new ProductCategory(){Name="Kategori Y",ModifiedDate=new DateTime(2006,5,6),rowguid=Guid.NewGuid()}
                                                                    ,new ProductCategory(){Name="Kategori Z",ModifiedDate=new DateTime(2007,1,4),rowguid=Guid.NewGuid()}
                                                                }
                                                            );

var eklenenler = adwContext.GetChangeSet().Inserts;

foreach (ProductCategory eklenen in eklenenler)
    Console.WriteLine(eklenen.Name);

adwContext.SubmitChanges();
```

Yine nesne başlatıcılarından (Object Initializers) yararlanılarak, InsertAllOnSubmit metodu içerisinde ProductCategory tipinden örnekler alabilecek generic List koleksiyonu oluşturulmuş ve 3 örnek ProductCategory eklenmiştir. İlave edilen satırları elde edebilmek için yine GetChangeSet metodu üzerinden Inserts özelliği kullanılmaktadır. DataContext tipi üzerinden SubmitChanges metodunun çalıştırılması ile birlikte, varlık (Entity) tiplerine eklenen 3 ProductCategory nesnesi içinde birer Insert sorgusu SQL sunucusu üzerinde yürütülmektedir. Bu durumu daha iyi analiz etmek için SQL Profiler aracı kullanıldığında aşağıdakine benzer sonuçlar alındığı görülür.

![mk235_9.gif](/assets/images/2007/mk235_9.gif)

Dikkat edileceği üzere her bir varlık nesnesi (Entity Object) için birer Insert sorgusu yürütülmektedir.

Gelelim silme işlemlerine. Silme (Delete) operasyonlarındada ekleme işlemlerine benzer şekilde DeleteOnSubmit ve DeleteAllOnSubmit gibi metodlar yer almaktadır. Herzamanki gibi varlık nesneleri üzerinden yapılan silme işlemleri GetChangeSet metodu üzerinden ulaşılan Deletes özelliği yardımıyla elde edilebilir. Aşağıdaki örnek kod parçasında tek bir satırın silme işleminin nasıl yapılabileceği gösterilmektedir.

```csharp
ProductCategory silinecekVeri = (from cat in adwContext.ProductCategories
                                                        where cat.ProductCategoryID == 25
                                                            select cat).Single<ProductCategory>();

adwContext.ProductCategories.DeleteOnSubmit(silinecekVeri);

var silinenler = adwContext.GetChangeSet().Deletes;

foreach(ProductCategory silinen in silinenler)
    Console.WriteLine(silinen.Name);

adwContext.SubmitChanges();
```

Burada dikkat edilmesi gerken bazı noktalar vardır. Silinmek istenilen satır veya satırların öncelikli olarak bulunması gerekir. Bu çok doğal olarak Table tipi üzerinden bir LINQ sorgusu ile mümkün olabilir. Dolayısıyla yukarıdaki kod parçasına göre, silinecekVeri isimli koleksiyon elde edilirken arka planda aşağıdaki sorgu cümlesi çalışacaktır.

```text
exec sp_executesql N'SELECT TOP (1) [t0].[ProductCategoryID], [t0].[Name], [t0].[rowguid], [t0].[ModifiedDate]
FROM [Production].[ProductCategory] AS [t0]
WHERE [t0].[ProductCategoryID] = @p0',N'@p0 int',@p0=25
```

Bu adımdam sonra DeleteOnSubmit metoduna, silinmek istenen varlık (Entity) örneği parametre olarak verilmektedir. Bu işlem sadece silinecek olan veriler için kuyruğa bir ekleme yapmaktadır. Öyleki bu metod çağırıldıktan sonra ProductCategories koleksiyonuna bakılırsa, 25 numaralı ProductCategory tipinin halen daha mevcut olduğu görülebilir.

![mk235_10.gif](/assets/images/2007/mk235_10.gif)

Silinmek istenen verinin programatik ortamda elde edilmesi için tek yapılması gereken GetChangeSet metodu üzerinden Deletes özelliğine ulaşmaktır. Çalışma zamanı SubmitChanges metodunu yürüttüğünde ise silinmek istenen entity nesnesi ile ilgili olaraktan aşağıdaki sorgu cümlesi SQL tarafında işletilecektir.

```text
exec sp_executesql N'DELETE FROM [Production].[ProductCategory] 
WHERE 
      ([ProductCategoryID] = @p0) AND ([Name] = @p1) AND ([rowguid] = @p2) AND ([ModifiedDate] = @p3)',N'@p0 int,@p1 nvarchar(10),@p2 uniqueidentifier,@p3 datetime',@p0=25,@p1=N'Kategori Y',@p2='B68CEBC9-CFFF-4AD5-9F5A-ADEC8823E88A',@p3='2006-06-05 00:00:00:000'
```

Sorgu cümlesindende dikkat edileceği üzere Where ifadesinden sonra tüm alanlar hesaba katılmaktadır. Bunun sebebi LINQ to SQL mimarisinin Optimistic (İyimser) Concurrency modelini kullanmasıdır. Bu modellde bilindiği üzere kontrol edilebilir tüm alanlar Where ifadesinden sonra hesaba katılmaktadır. Bir başka deyişle model, veriyi başkasının silip silmediğini, güncelleştirip güncelleştirmediğini araştırmaktadır.

Örnekte 25 numaralı kategoriye ait satırı silmeden önce başka birisi değiştirmişse eğer, çalışma zamanında ChangeConflictException istisnası alınır. Bu durumu daha iyi analiz etmek için 25 numaralı satırı silmek istediğimizi göz önüne alalım. SubmitChanges metodu çağırılmadan öncede veritabanından manuel olarak yada başka bir program üzerinden 25 numaralı satırın Name alanının değerini Kategori X'den Kategori XL'ye değiştirelim. Bunu kolay bir şekilde gerçekleştirmek için SubmitChanges satırına breakpoint koyup ilerlemeden önce tabloda değişiklik yapmak yeterli olacaktır. Böyle bir durumda çalışma zamanında (run time) aşağıdaki gibi bir durum olaşacaktır.

![mk235_11.gif](/assets/images/2007/mk235_11.gif)

Nitekim biz silmek istediğimiz veriyi çektikten sonra Name alanının değeri Kategori X iken, SubmitChanges'den önce Kategori XL'ye değiştirilmiştir. Buda doğal olarak Where ifadesinin geçersiz olması anlamına gelmektedir. Ancak bu durum istenirse değiştirilebilir. Öyleki, varlık (Entity) sınıflarında yer alan özelliklerin (Properties) Column isimli niteliklerine (Attribute) ait ColumnChange özelliğinin değeri Never yapıldığında söz konusu özelliklerin Where ifadelerinden sonrasına katılmadığı görülmektedir. Söz gelimi örnekte yer alan ProductCategory sınıfının Name, rowguid ve ModifiedDate özelliklerinde yer alan Column niteliğinde aşağıdaki gibi bir değişiklik yaptığımızı düşünelim.

```csharp
[Column(Storage="_Name", DbType="NVarChar(50) NOT NULL", CanBeNull=false,UpdateCheck=UpdateCheck.Never)]
public string Name
```

UpdateCheck değerine Never atanması sonucu söz konusu özelliğin değeri WHERE ifadesine parametre olarak dahil edilmeyecektir. Bu işlemin ardından örnek olarak başka bir satırı daha silmek istersek arka tarafta çalışan Delete sorgusunun aşağıdaki gibi oluşturulduğunu görebiliriz.

```text
exec sp_executesql N'DELETE FROM [Production].[ProductCategory] 
     WHERE [ProductCategoryID] = @p0',N'@p0 int',@p0=29
```

Görüldüğü gibi sadece ProductCategoryID alanı WHERE ifadesinden sonrasına katılmıştır. (Kodun bundan sonraki kısımlarında Optimistic Concurrency modeline göre ilerleneceğinden UpdateCheck değişiklikleri geri alınmıştır.)

İstenirse toplu olarak silme işlemleride gerçekleştirilebilir. Örneğin Name özelliğinin içerisinde Kategori kelimesi geçen satırları silmek istediğimizi düşünelim. Bu amaçla aşağıdaki gibi bir kod parçası göz önüne alınabilir. Öncelikli olarak Contains metodu ile Kategori kelimesi geçen ProductCategory nesnelerinin bir listesinin elde edilemsi gerekmektedir. LINQ sorgusu buna göre düzenlenmiştir.

```csharp
var kategoriGecenler = from k in adwContext.ProductCategories
                                        where k.Name.Contains("Kategori")
                                            select k;

adwContext.ProductCategories.DeleteAllOnSubmit<ProductCategory>(kategoriGecenler);

adwContext.SubmitChanges();
```

Söz konusu kod içerisinde SubmitChanges metodu çalıştırıldığında SQL tarafında, silinmek istenen her satır için bir Delete sorgu ifadesinin yürütüldüğü görülecektir. Örnekte bu kategoriye uyan 5 satır bulunmaktadır.

![mk235_12.gif](/assets/images/2007/mk235_12.gif)

Gelelim güncelleme (Update) işlemlerine. Güncelleme süreçlerinde, Insert ve Delete işlemlerindeki gibi metodlar söz konusu değildir. Nitekim güncelleme işlemi aslında varlık nesnesinin herhangibir özelliğinin (özelliklerinin) değerinin değiştirilmesinden başka bir şey değildir. Dolayısıyla tek yapılması gereken değişiklikler tamamlandıktan sonra SubmitChanges metodunu çağırmaktır. Aşağıdaki örnek kod parçasında örnek olarak Product tablosuna ait bir varlık sınıfı (Entity Class) kullanılmaktadır. Bu sınıfı oluşturmak için tek yapılması gereken tahmin edileceği üzere Server Explorer'dan Product tablosunu Adventure.dbml üzerine tasarım zamanında sürükleyip bırakmaktır.

```csharp
var guncellenecekler = from p in adwContext.Products
                                    where p.ProductSubcategoryID == 1
                                        select p;

foreach (Product prd in guncellenecekler)
    prd.ListPrice += 10;

var bekleyenGuncellemeler = adwContext.GetChangeSet().Updates;

adwContext.SubmitChanges();
```

Bu kod parçasında ProductSubCategoryId değeri 1 olan nesneler elde edilmekte ve herbirinin ListPrice değeri 10 birim arttırılmaktadır. Çok doğal olarak yapılan bu güncellemelerde Updates kuyruğuna atılacaktır. Söz gelimi örneğe göre varsayılan olarak kuyruğa 32 Product nesnesi atılmaktadır. Elbette koşula uyan kategorilerin her birinin ListPrice özelliklerinin değerleri değiştirildiğinde bu güncellemeler Products koleksiyonunada yansıyacaktır.

![mk235_13.gif](/assets/images/2007/mk235_13.gif)

SubmitChanges metodunun işletilmesi ile birlikte güncelleme kuyruğunda bekleyen 32 adet Product nesne örneği için veritabanında Update sorguları çalıştırılacaktır. Aşağıdaki SQL Profiler ekran görüntüsünde bu sorguların bir kısmı yer almaktadır.

![mk235_14.gif](/assets/images/2007/mk235_14.gif)

Buraya kadarki kısımda basit olarak ekleme (Insert), silme (Delete) ve güncelleme (Update) işlemlerini nasıl yapabileceğimizi görmeye çalıştık. Önemli olan noktalardan biriside değişikliklerden vazgeçersek ne olacağıdır. Bu önemli bir sıkıntıdır. Nitekim GetChangeSet metodu üzerinden elde edilen ChangeSet tipinin sunduğu Deletes, Inserts, Updates özelliklerinin döndürdğü IList koleksiyonları çalışma zamanında yanlız okunabilir (read-only) şekilde ele alınabilmektedir. Bu nedenle Clear, Remove, RemoveAt gibi metod çağrıları hatta null değer atanması sonrası çalışma zamanı istisnaları alınmaktadır. Söz gelimi son örnek koddaki güncelleştirmeleri onaylamak istemediğimizi düşünelim. Bu amaçla Clear metoduna başvurulması düşünülebilir. Ancak bu durumda çalışma zamanında aşağıdaki ekran görüntüsünde yer alan NotSupportedException istisnası alınacaktır.

![mk235_15.gif](/assets/images/2007/mk235_15.gif)

Bu noktada Table tipi üzerinden ulaşılabilen GetOriginalEntityState metodu alternatif bir yol olarak ele alınabilir. Nitekim bu metod, parametre olarak verilen varlık nesnesinin (Entity Object) değiştirilmeden önceki halini elde etmemizi sağlamaktadır. Örneğin aşağıdaki kod parçasını ele alalım.

```csharp
var bekleyenGuncellemeler = adwContext.GetChangeSet().Updates;

Product prdNew = (Product)bekleyenGuncellemeler[0];
Console.WriteLine(prdNew.ListPrice.ToString());

Product prdOriginal = adwContext.Products
                                 .GetOriginalEntityState((Product)bekleyenGuncellemeler[0]);
Console.WriteLine(prdOriginal.ListPrice.ToString());
```

Burada görüldüğü gibi prdNew nesne örneğinin ListPrice değeri 3419.99 iken prdOriginal'in değeri 3409.00 dur. Aşağıdaki çalışma zamanı görüntüsünde bu durum net bir şekilde görülebilmektedir.

![mk235_16.gif](/assets/images/2007/mk235_16.gif)

Ancak bu teknik yardımıyla güncellenmiş olan tüm satırların geri alınması oldukça zordur. Nitekim Table tipinin yapıcı metodunun (Constructor) kullanılamadığı, bu yüzden new ile üretilemediği ortadadır. Ayrıca var olan DataContext nesnesinin Table tipinden özellikleri ReadOnly'dir. Bir başka deyişle bu özelliklere doğrudan atama da yapılamamaktadır. Sonuç olarak DataContext tipinin yeniden örneklenmesi sorunu çözmek için yeterli olacaktır.

![dikkat.gif](/assets/images/2007/dikkat.gif)
Table generic tipi üzerinden kullanılan GetModifiedMembers metodu ile, parametre olarak verilen varlık (entity) nesne örneğinin değişikliğe uğrayan değerlerinin orjinal (OriginalValue) ve anlık (CurrentValue) hallerinin elde edilmesi sağlanabilmektedir. GetModifiedMembers metodu geriye ModifiedMemberInfo tipinden bir dizi döndürmektedir. Aşağıdaki şekilde örnek olarak güncellenen satırlardan ilki için orjinal ve güncel ListPrice değerlerinin elde edilişi gösterilmektedir.
![mk235_17.gif](/assets/images/2007/mk235_17.gif)

Normal şartlarda SubmitChanges metodunun çağırılmasından sonra güncelleme, ekleme ve silme işlemleri otomatik olarak transaction içerisinde çalıştırılırlar. Bir başka deyişle SubmitChanges metodu, veritabanı üzerinde yapılacak işlemlerin, biz söylemeden otomatik olarak bir transaction içerisinde olmasını sağlamaktadır. Söz gelimi aşağıdaki kod parçasını ele alalım. Bu kod parçasında güncelleme (Update) ve yeni ürün ekleme (Insert) işlemleri söz konusudur.

```csharp
var guncellenecekler = from p in adwContext.Products
                            where p.Class == "M" && p.ProductSubcategoryID==1
                                select p;

foreach (Product prd in guncellenecekler)
    prd.ListPrice += 10;

Product newProduct = new Product() 
    { 
        Name = "Yeni Urun"
        , ProductSubcategoryID = 1, Color = "Red"
        , Class = "M", ListPrice = 100
        , ProductNumber = "PRD-1204", ReorderPoint = 10
        , StandardCost = 90, ProductModelID = 123
        , SafetyStockLevel = 45,SellStartDate=new DateTime(2007,1,1)
        , SellEndDate=new DateTime(2008,1,1), DiscontinuedDate=new DateTime(2006,6,6)
        , ModifiedDate=DateTime.Now
    };

adwContext.Products.InsertOnSubmit(newProduct);

adwContext.SubmitChanges();
```

İlk olarak Class değeri M ve ProductSubCategoryID değeri 1 olan Product tiplerine ait bir koleksiyon çekilmektedir. Daha sonra bu koleksiyon üzerinde dönülerek ListPrice değerleri sembolik olarak 10' ar birim arttırılmaktadır. Hemen arkasıdan örnek bir Product nesnesi oluşturulmakta ve InsertOnSubmit metodu ile eklenecekler listesine aktarılmaktadır. Uygulama çalıştırıldığında SQL Profiler aracılığıyla arkadaki işlemler incelenecek olursa aşağıdaki ekran görüntüsünde yer alan sonuçların elde edildiği görülür.

![mk235_18.gif](/assets/images/2007/mk235_18.gif)

Dikkat edilecek olursa Insert ve Update ifadelerinin tamamı aynı Transaction içerisinde yürütülmektedir. Diğer taraftan aynı kodun aşağıdaki gibi değiştirildiğini düşünelim.

```csharp
var guncellenecekler = from p in adwContext.Products
                            where p.Class == "M" && p.ProductSubcategoryID==1
                                select p;

foreach (Product prd in guncellenecekler)
    prd.ListPrice += 10;
adwContext.SubmitChanges();

Product newProduct = new Product() 
    { 
        Name = "Yeni Urun"
        , ProductSubcategoryID = 1, Color = "Red"
        , Class = "M", ListPrice = 100
        , ProductNumber = "PRD-1204", ReorderPoint = 10
        , StandardCost = 90, ProductModelID = 123
        , SafetyStockLevel = 45,SellStartDate=new DateTime(2007,1,1)
        , SellEndDate=new DateTime(2008,1,1), DiscontinuedDate=new DateTime(2006,6,6)
        , ModifiedDate=DateTime.Now
    };

adwContext.Products.InsertOnSubmit(newProduct);

adwContext.SubmitChanges();
```

Burada SubmitChanges metodu güncelleme ve ekleme işlemlerinden sonra birer kez ayrı ayrı çağırılmaktadır. Bu durumda SQL Profiler aşağıdaki sonuçları üretecektir.

![mk235_19.gif](/assets/images/2007/mk235_19.gif)

Görüldüğü gibi SubmitChanges her çağırıldığında o ana kadar gerçekleştirilen ne kadar ekleme, güncelleme veya silme işlemi varsa ayrı bir Transaction kapsamı içerisinde çalışmaktadır. Elbetteki istenirse son kod parçasındaki tüm sorgu işlemlerin aynı transaction kapsamı (Scope) içerisine dahil edilmeside sağlanabilir. Bunun için TransactionScope nesnesinden yararlanılabilir. Aşağıdaki örnek bu durum basit olarak ele alınmaktadır.(TransactionScope sınıfının kullanılabilmesi için.Net Framework 2.0 ile birlikte gelen System.Transactions.dll assembly'ının projeye referans edilmesi gerekmektedir.)

```csharp
using (TransactionScope tScope = new TransactionScope())
{
    var guncellenecekler = from p in adwContext.Products
                                        where p.Class == "M" && p.ProductSubcategoryID==1
                                            select p;

    foreach (Product prd in guncellenecekler)
        prd.ListPrice += 10;

    adwContext.SubmitChanges();

    Product newProduct = new Product() 
        { 
            Name = "Yeni Urun"
            , ProductSubcategoryID = 1, Color = "Red"
            , Class = "M", ListPrice = 100
            , ProductNumber = "PRD-1204", ReorderPoint = 10
            , StandardCost = 90, ProductModelID = 123
            , SafetyStockLevel = 45, SellStartDate=new DateTime(2007,1,1)
            , SellEndDate=new DateTime(2008,1,1), DiscontinuedDate=new DateTime(2006,6,6)
            , ModifiedDate=DateTime.Now
        };

    adwContext.Products.InsertOnSubmit(newProduct);
    adwContext.SubmitChanges();

    tScope.Complete();
}
```

Dikkat edileceği üzere tüm kod parçası TransactionScope nesne örneğine ait bir using bloğu içerisine alınmış ve en sonra Complete metodu çağırılmıştır. Bu durumda SubmitChanges çağrıları ayrı ayrı yapılmış olsada tüm işlemler aynı transaction kapsamına (Transaction Scope) dahil edilecektir. TransactionScope kullanılması çok doğal olarak programatik taraftan Transaction ile ilgili daha fazla yönetsel işlemin yapılabilmesi anlamınada gelmektedir. Söz gelimi izolasyon seviyeleri (Isolation Level) daha kontrollü bir şekilde ele alınabilir. Hatta dağıtık transaction (Distributed Transaction) geçişleri daha kolay programlanabilir.

TransactionScope kullanımı dışında yerel transaction kullanılarakta ilgili işlemlerin aynı Transaction içerisinde gerçekeştirilmesi sağlanabilir. Aşağıdaki örnek kod parçasında bu durum ele alınmaya çalışılmaktadır.

```csharp
try
{
    var guncellenecekler = from p in adwContext.Products
                                        where p.Class == "M" && p.ProductSubcategoryID == 1
                                            select p;

    foreach (Product prd in guncellenecekler)
        prd.ListPrice += 10;

    // Transaction başlatılması için bağlantının açık olması gerekir.
    adwContext.Connection.Open();

    // Transaction başlatılır ve AdventureContext tipine bildirilir.
    adwContext.Transaction = adwContext.Connection.BeginTransaction();

    adwContext.SubmitChanges();

    Product newProduct = new Product() 
        { 
            Name = "Yeni Urun"
            , ProductSubcategoryID = 1, Color = "Red"
            , Class = "M", ListPrice = 100
            , ProductNumber = "PRD-1204", ReorderPoint = 10
            , StandardCost = 90, ProductModelID = 123
            , SafetyStockLevel = 45, SellStartDate=new DateTime(2007,1,1)
            , SellEndDate=new DateTime(2008,1,1), DiscontinuedDate=new DateTime(2006,6,6)
            , ModifiedDate=DateTime.Now
        };

    adwContext.Products.InsertOnSubmit(newProduct);

    adwContext.SubmitChanges();

    // Herşey yolunda ise Transaction onaylanır
    adwContext.Transaction.Commit();
}
catch
{
    // Bir aksilik olduysa Transaction geri alınır
    adwContext.Transaction.Rollback();
}
finally
{
    if (adwContext.Connection.State == ConnectionState.Open)
        adwContext.Connection.Close();
}
```

Dikkat edileceği üzere DataContext tipinin Transaction özelliğine değer atanırken Connection üzerinden gidilmiş ve BeginTransaction metodu kullanılmıştır. BeginTransaction metodunun parametresinden yararlanılarak, Transaction'ın izolasyon seviyeside (Isolation Level) değiştirilebilir. Burada Transaction'ın oluşturulabilmesi için bağlantının (Connection) açık olması gerekmektedir. Bu sebeptende finally bloğu içerisinde açık kalan bağlantının kontrollü bir şekilde kapatılması sağlanmaktadır. Transaction'a dahil olan işlemlerin onaylanması için Commit metodu kullanılırken bir sorun ile karşılaşılması halinde o ana kadar yapılan işlemlerin geri alınması içinde Rollback metoduna başvurulmaktadır. Sonuç olarak SQL Profiler aracı izlendiğinde ekleme (insert) ve güncelleme (update) işlemlerinin yine tek bir Transaction kapsamı içerisinde ele alındığı görülmektedir.

LINQ To SQL mimarisin şu ana kadar işlenen temel CRUD işlemlerinde ele alınması gereken başka hususlarda vardır. Söz gelimi eş zamanlı çalışan programların aynı veriler üzerinde işlemler yaptığı durumlarda oluşan çakışmaların (Conflict) ele alınması gibi. Bu ve benzeri konuları ilerleyen makalelerimizde ve görsel derslerimizde incelemeye çalışıyor olacağız. Bu makalemizde çok basit ve temel seviyede ekleme (Insert), Silme (Delete) ve Güncelleme (Update) işlemlerini nasıl yapabileceğimizi incelemeye çalıştık. Ayrıca bu işlemleri yaparken Transaction'ların nasıl kullanılabildiğinide gördük. Varsayılan olarak bilinçsiz şekilde başlatılan Transaction'ları TransactionScope ile veya Local Transaction teknikleri nasıl kontrol edebileceğimiz gördük. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](/assets/files/2007/LinqToSqlCRUD.rar)