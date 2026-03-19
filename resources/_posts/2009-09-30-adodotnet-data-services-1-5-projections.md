---
layout: post
title: "Ado.Net Data Services 1.5 - Projections"
date: 2009-09-30 00:00:00 +0300
description: "Çok zaman değil daha bir sene öncesine kadar Astoria kod adlı Ado.Net Data Services konusunu incelemeye başlamıştım. Entity Framework veya Custom LINQ Provider' ları ile sunulan veri kümelerine, REST bazlı olarak URL sorgular atılabilmesini sağlayan ve özellikle Silverlight gibi RIA içeriklerinde son derece kıymetli olan bir servis uygulaması olarak değerlendirebileceğimiz bu konu ile ilişkili ilk paylaşımlarımı yaptıktan sonra araya WCF 4.0, WF 4.0, Design Patterns, Design Principles, .Net RIA Services gibi konular girdi."
categories:
  - ado-net-data-services
  - wcf
tags:
  - ado-net-data-services
  - wcf
  - csharp
  - dotnet
  - aspnet
  - ado-net
  - entity-framework
  - linq
  - sql-server
  - workflow-foundation
  - silverlight
  - rest
  - http
  - performance
  - visual-studio
---
# Ado.Net Data Services 1.5 - Projections
![blg79_Giris.jpg](/assets/images/2009/blg79_Giris.jpg)

Merhaba Arkadaşlar,

Gün geçmiyorki yazılım teknolojilerinde bir yenilik, bir güncelleme, bir genişletme çıkmasın...Özellikle dünyanın dev yazılım şirketlerinin en büyüğü olarak görebileceğimiz Microsoft tarafında bu gelişme ve güncelleme hızı oldukça yüksek. Gerçektende heyecan verici yenilikler, özellikler ile karşılaşmıyor değiliz. Bu konuya nereden mi geldim?

Çok zaman değil daha bir sene öncesine kadar Astoria kod adlı Ado.Net Data Services konusunu incelemeye başlamıştım. Entity Framework veya Custom LINQ Provider'ları ile sunulan veri kümelerine, REST bazlı olarak URL sorgular atılabilmesini sağlayan ve özellikle Silverlight gibi RIA içeriklerinde son derece kıymetli olan bir servis uygulaması olarak değerlendirebileceğimiz bu konu ile ilişkili ilk paylaşımlarımı yaptıktan sonra araya WCF 4.0, WF 4.0, Design Patterns, Design Principles,.Net RIA Services gibi konular girdi. Bu konulardaki incelemelerimi ve paylaşımlarımı devam ettirirken bir baktım ki Ado.Net Data Services konusuna çok uzun zaman ara vermişim. Ara vermeklede iyi yapmamışım

![Undecided](/assets/images/2009/smiley-undecided.gif)

Nitekim program yöneticisi olan Mike Flasko boş durmamış ve [Ado.Net Data Services v1.5](http://www.microsoft.com/downloads/details.aspx?FamilyID=a71060eb-454e-4475-81a6-e9552b1034fc&displaylang=en) versiyonu için CTP2 sürümünü duyurmuş (.Net Framework 3.5 Service Pack 1 ve Silverlight 3.0' ı hedefleyen ama.Net Framework 4.0 içerisinede dahil edilecek olan özellikleri içeren bir sürüm olarak düşünülebilir). Duyurulması ile birlikte hem [blog](http://blogs.msdn.com/astoriateam/default.aspx) sitesinde hemde çeşitli kaynaklarda konu ile ilişkili yazılar yayınlanmaya da başlanmış.

Bu versiyonda bazı yenilikler ve daha önceki sürüme ait çeşitli düzeltmeler (bug-fix) yer almakta. Gelen yeni özelliklerden birisi de Projections kullanımı. Bu yeniliğe göre servis üzerinde gerçekleştirilen URL bazlı sorguların sonuçları kırpılabiliyor ve sadece ilgilenilmek istenenlerin istemci tarafına çekilmesi sağlanabiliyor.

![Wink](/assets/images/2009/smiley-wink.gif)

Bir başka deyişle, istemcinin yapmış olduğu bir talebin (Request) sonuçlarında sadece ilgilendiği özelliklerin getirilmesi sağlanabilmekte. Bunu tam olmasada, LINQ sorguları sırasında anonymous type kullanımına benzetebiliriz. Söz konusu özellik içerisinde primitive/complex tipleri veya navigation özelliklerini de kullanabilmekteyiz. Özelliğin getirisi, istemcinin talebi sonrası tüm Entity kümesinin işlenmesi ve ağ üzerinde hareket etmesi yerine, sadece istediği özellikleri içeren kümenin/kümelerin değerlendirilebilmesi olarak görülebilir.

Bu çok doğal olarak istemci ile sunucu arasındaki trafiği boyutsal olarak azaltmaktadır. Projections kullanımı son derece basittir. Bunun için $select operatöründen yararlanılmaktadır. Tabiki konuyu anlamamızın en iyi yolu basit bir örneği adım adım geliştirmek ve üzerinde ilerlemekle olacaktır. Bu nedenle kolları sıvayıp işe koyulalım. İlk olarak Visual Studio 2008 ortamında (Service Pack 1 yüklü olan) basit bir Asp.Net Web Uygulaması oluşturarak işe başlayabiliriz. Sonrasında servisimiz için gerekli Entity kaynağını oluşturmamız gerekiyor. Bu amaçla Ado.Net Entity Framework'ten yararlanabilir ve yine kobay veritabanımız olan AdventureWorks'ü değerlendirebiliriz. Örneğimizde aşağıdaki EDM şemasını kullanıyor olacağız.

![blg79_Edm.gif](/assets/images/2009/blg79_Edm.gif)

AdventureWorks veritabanındaki Production şemasında yer alan ProductCategory, ProductSubCategory ve Product tablolarını kullanmaya çalışıyoruz. Entity modelimizi oluşturduktan sonra, projemize yeni bir Ado.Net Data Services öğesi ekleyerek devam edebiliriz. Tabi bu seferki örneğimizde v1.5 CTP2 sürümüne ait öğeyi kullanmamız gerekiyor.

![blg79_NewItem.gif](/assets/images/2009/blg79_NewItem.gif)

Ado.Net Data Services öğemizin kod içeriğini ise aşağıdaki gibi değiştirmemiz yeterli olacaktır.

```csharp
using System.Data.Services;

namespace Projections
{
    public class AdventureServices 
        : DataService<AdventureWorksEntities>
    {
        public static void InitializeService(DataServiceConfiguration config)
        {
            // Tüm Entity' leri sadece okuma amaçlı açıyoruz
            config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
            // İstemciden gelecek olan Projection taleplerinin değerlendirileceğini belirtiyoruz
            config.DataServiceBehavior.AcceptProjectionRequests = true;
            // Versiyon 2 için geliştirme yapacağımızı belirtiyoruz. Bu versiyon belirtilmediği takdirde select operatörü ve projection fonksiyonelliği çalışmayacaktır.
            config.DataServiceBehavior.MaxProtocolVersion = System.Data.Services.Common.DataServiceProtocolVersion.V2;
     }
    }
}
```

Dikkat edilmesi gereken noktalardan birisi AcceptProjectionRequest ise MaxProtocolVersion özelliklerine atanan değerlerdir. Bu değerlere göre servisimiz, istemcilere Projection fonksiyonelliğini sunabilecektir. AdventureServices.svc dosyasını bir tarayıcı yardımıyla talep ettiğimizde, başlangıç için aşağıdakine benzer bir ekran görüntüsü ile karşılaşırız.

![blg79_FirstRun.gif](/assets/images/2009/blg79_FirstRun.gif)

Görüldüğü üzere Product, ProductCategory ve ProductSubcategory Entity'leri kullanılmaya hazırdır. Evetttt...Gelelim yazımızın önemli olan kısmına. Tarayıcı üzerinden aşağıdaki sorguyu talep ettiğimizi düşünelim.

http://localhost:1714/AdventureServices.svc/Product?$select=ProductID,Name,ListPrice

Dikkat edileceği üzere Product Entity'si üzerinden select sorgusu atılmış ve sadece ProductID,Name,ListPrice alanları talep edilmiştir. Bu sorgunun çalışma zamanı çıktısı aşağıdaki gibi olacaktır.

![blg79_SecondRun.gif](/assets/images/2009/blg79_SecondRun.gif)

Dikkat edileceği üzere Product tablosundaki tüm ürünlerin sadece ProductID,Name ve ListPrice alanları çekilmiştir. İşin güzel yanı, bu URL talebi için arka planda çalıştırılan SQL sorgusuda sadece istenen alanları değerlendirmektedir. İşte URL'imize ait SQL sorgusunun SQL Server Profiler'dan yakalanan içeriği.

```text
SELECT 
1 AS [C1], 
CASE WHEN ([Extent1].[ProductID] IS NULL) THEN N'' ELSE N'AdventureWorksModel.Product' END AS [C2], 
N'ProductID,Name,ListPrice' AS [C3], 
[Extent1].[ProductID] AS [ProductID], 
[Extent1].[Name] AS [Name], 
[Extent1].[ListPrice] AS [ListPrice]
FROM [Production].[Product] AS [Extent1]
```

Dolayısıyla Projection kullanılaraktan, bir Entity üzerinden sadece istenen alanları içeren çıktıların alınması sağlanabilir. Bu kullanım aynen SQL tarafı içinde geçerli olduğundan, performans adına da bazı kazanımların elde edildiği ortadadır.

select operatörünü dilersek navigasyon özellikleri (Navigation Properties) ilede bir aradada kullanabiliriz. Örneğin aşağıdaki gibi bir URL talebinde bulunduğumuzu düşünelim.

http://localhost:1714/AdventureServices.svc/ProductSubcategory?$select=Name,Product

Buna göre ProductSubcategory Entity'sinden sadece Name alanlarının değerlerini isterken, her alt kategoriye bağlı ürünleri tutan Product Entity örneklerini de talep etmekteyiz. Bu URL talebinin çıktısı aşağıdaki gibi olacaktır.

![blg79_ThirdRun.gif](/assets/images/2009/blg79_ThirdRun.gif)

Dikkat edileceği üzere alt kategoriye bağlı olan Product kümeleri için sadece bağlantı bildirimi yapılmaktadır. Söz konusu URL'ın çalıştırılması sonucunda SQL tarafında da aşağıdaki sorgunun yürütüldüğü görülecektir.

```text
SELECT 
1 AS [C1], 
CASE WHEN ([Extent1].[ProductSubcategoryID] IS NULL) THEN N'' ELSE N'AdventureWorksModel.ProductSubcategory' END AS [C2], 
N'Name,ProductSubcategoryID' AS [C3], 
[Extent1].[Name] AS [Name], 
[Extent1].[ProductSubcategoryID] AS [ProductSubcategoryID]
FROM [Production].[ProductSubcategory] AS [Extent1]
```

Fark edilebileceği gibi, Product tablosu ile ilişkili bir sorgu ifadesi yer almamaktadır. Diğer yandan sadece Name alanını talep etmemize rağmen, PrimaryKey olan ProductSubcategoryID alanı da getirilmektedir. Bu son derece doğaldır nitekim, belirli bir ProductSubcategory'nin çekilmesinde primary key alanı ayırt edici özelliklerdendir üstelik entry/id elementleri içerisinde gereklidir. Diğer yandan, URL satırını aşağıdaki gibi değiştirirsek,

http://localhost:1714/AdventureServices.svc/ProductSubcategory?$select=Name,Product&$expand=Product&$top=2

Hımmm...

![Wink](/assets/images/2009/smiley-wink.gif)

Bu sorguya göre ProductSubcategory içeriğinden sadece Name alanını almakla kalmıyor, aynı zamanda alt kategoriye bağlı olan ürünleride çekiyoruz. Üstelik sadece ilk 2 ProductSubcategory tipini ele alıyoruz (Sondaki top=2 sorgusu nedeniyle). İşte çalışma zamanı çıktımız.

![blg79_ForthRun.gif](/assets/images/2009/blg79_ForthRun.gif)

Peki bu URL talebi sonrası arka planda nasıl bir SQL sorgusu çalışıyor?

```text
SELECT 
[Project2].[ProductSubcategoryID] AS [ProductSubcategoryID],[Project2].[Name] AS [Name], [Project2].[rowguid] AS [rowguid], [Project2].[ModifiedDate] AS [ModifiedDate], [Project2].[C1] AS [C1], [Project2].[C2] AS [C2], [Project2].[C3] AS [C3], [Project2].[C4] AS [C4], [Project2].[C5] AS [C5], [Project2].[C6] AS [C6], [Project2].[C7] AS [C7], [Project2].[ProductID] AS [ProductID], [Project2].[Name1] AS [Name1], [Project2].[ProductNumber] AS [ProductNumber], [Project2].[MakeFlag] AS [MakeFlag], [Project2].[FinishedGoodsFlag] AS [FinishedGoodsFlag], [Project2].[Color] AS [Color], [Project2].[SafetyStockLevel] AS [SafetyStockLevel], [Project2].[ReorderPoint] AS [ReorderPoint], [Project2].[StandardCost] AS [StandardCost], [Project2].[ListPrice] AS [ListPrice], [Project2].[Size] AS [Size], [Project2].[SizeUnitMeasureCode] AS [SizeUnitMeasureCode], [Project2].[WeightUnitMeasureCode] AS [WeightUnitMeasureCode], [Project2].[Weight] AS [Weight], [Project2].[DaysToManufacture] AS [DaysToManufacture], [Project2].[ProductLine] AS [ProductLine], [Project2].[Class] AS [Class], [Project2].[Style] AS [Style], [Project2].[ProductModelID] AS [ProductModelID], [Project2].[SellStartDate] AS [SellStartDate], [Project2].[SellEndDate] AS [SellEndDate], [Project2].[DiscontinuedDate] AS [DiscontinuedDate], [Project2].[rowguid1] AS [rowguid1], [Project2].[ModifiedDate1] AS [ModifiedDate1]
FROM ( SELECT 
 [Limit1].[ProductSubcategoryID] AS [ProductSubcategoryID],  [Limit1].[Name] AS [Name],  [Limit1].[rowguid] AS [rowguid],  [Limit1].[ModifiedDate] AS [ModifiedDate],  [Limit1].[C1] AS [C1],  [Limit1].[C2] AS [C2],  [Limit1].[C3] AS [C3],  [Limit1].[C4] AS [C4],  [Limit1].[C5] AS [C5],  [Limit1].[C6] AS [C6],  [Extent2].[ProductID] AS [ProductID],  [Extent2].[Name] AS [Name1],  [Extent2].[ProductNumber] AS [ProductNumber],  [Extent2].[MakeFlag] AS [MakeFlag],  [Extent2].[FinishedGoodsFlag] AS [FinishedGoodsFlag],  [Extent2].[Color] AS [Color],  [Extent2].[SafetyStockLevel] AS [SafetyStockLevel],  [Extent2].[ReorderPoint] AS [ReorderPoint],  [Extent2].[StandardCost] AS [StandardCost],  [Extent2].[ListPrice] AS [ListPrice],  [Extent2].[Size] AS [Size],  [Extent2].[SizeUnitMeasureCode] AS [SizeUnitMeasureCode],  [Extent2].[WeightUnitMeasureCode] AS [WeightUnitMeasureCode],  [Extent2].[Weight] AS [Weight],  [Extent2].[DaysToManufacture] AS [DaysToManufacture],  [Extent2].[ProductLine] AS [ProductLine],  [Extent2].[Class] AS [Class],  [Extent2].[Style] AS [Style],  [Extent2].[ProductModelID] AS [ProductModelID],  [Extent2].[SellStartDate] AS [SellStartDate],  [Extent2].[SellEndDate] AS [SellEndDate],  [Extent2].[DiscontinuedDate] AS [DiscontinuedDate],  [Extent2].[rowguid] AS [rowguid1],  [Extent2].[ModifiedDate] AS [ModifiedDate1], 
 CASE WHEN ([Extent2].[ProductID] IS NULL) THEN CAST(NULL AS int) ELSE 1 END AS [C7]
 FROM   (SELECT TOP (2) [Project1].[ProductSubcategoryID] AS [ProductSubcategoryID], [Project1].[Name] AS [Name], [Project1].[rowguid] AS [rowguid], [Project1].[ModifiedDate] AS [ModifiedDate], [Project1].[C1] AS [C1], [Project1].[C2] AS [C2], [Project1].[C3] AS [C3], [Project1].[C4] AS [C4], [Project1].[C5] AS [C5], [Project1].[C6] AS [C6]
  FROM ( SELECT 
   [Extent1].[ProductSubcategoryID] AS [ProductSubcategoryID], 
   [Extent1].[Name] AS [Name], 
   [Extent1].[rowguid] AS [rowguid], 
   [Extent1].[ModifiedDate] AS [ModifiedDate], 
   1 AS [C1], 
   1 AS [C2], 
   CASE WHEN ([Extent1].[ProductSubcategoryID] IS NULL) THEN N'' ELSE N'AdventureWorksModel.ProductSubcategory' END AS [C3], 
   N'Name,ProductSubcategoryID' AS [C4], 
   N'Product' AS [C5], 
   1 AS [C6]
   FROM [Production].[ProductSubcategory] AS [Extent1]
  )  AS [Project1]
  ORDER BY [Project1].[ProductSubcategoryID] ASC ) AS [Limit1]
 LEFT OUTER JOIN [Production].[Product] AS [Extent2] ON [Limit1].[ProductSubcategoryID] = [Extent2].[ProductSubcategoryID]
)  AS [Project2]
ORDER BY [Project2].[ProductSubcategoryID] ASC, [Project2].[C7] ASC
```

Amanınnnn!!!

![Sealed](/assets/images/2009/smiley-sealed.gif)

Aslında biraz can sıkıcı ama doğal olarak tüm Product alanlarının değerlendirildiğini görüyoruz. Nitekim aksini belirtmedik. Peki belirtebilir miyiz? Yani ProductSubcategory kümesinden ve genişletilebilen Product kümesinden bir kaç alanı almayı başarabilir miydik? İşte örnek bir cevabı

![Cool](/assets/images/2009/smiley-cool.gif)

http://localhost:1714/AdventureServices.svc/ProductSubcategory?$select=Name,Product/Name,Product/ListPrice&$expand=Product&$top=5

Görüldüğü gibi EntityAdı/AlanAdı (örneğin Product/Name) stilinde yapılan bildirimlerle, üretilecek olan çıktıda birden fazla Entity'den gelebilecek alanları ayrı ayrı belirtebiliyoruz. (Buna göre sizlerde ProductCategory,ProductSubcategory ve Product kümelerinin tamamının bir arada bulunduğu örnek URL üzerinde çalışabilirsiniz. Çalışmanızı öneririm.)

![blg79_FifthRun.gif](/assets/images/2009/blg79_FifthRun.gif)

Görüldüğü üzere alt kategori ile ilişkili Feed girişlerinde Name alanı yer almaktayken, o alt kategoriye bağlı Product tipleri için sadece Name ve ListPrice değerleri getirilmektedir. Dolayısıyla SQL sorgusuda buna göre aşağıda görüldüğü gibi oluşacaktır.

```text
SELECT 
[Project2].[ProductSubcategoryID] AS [ProductSubcategoryID], [Project2].[Name] AS [Name], [Project2].[C1] AS [C1], [Project2].[C2] AS [C2], [Project2].[C3] AS [C3], [Project2].[C4] AS [C4], [Project2].[C5] AS [C5], 
[Project2].[C9] AS [C6], [Project2].[C6] AS [C7], [Project2].[C7] AS [C8], [Project2].[C8] AS [C9], [Project2].[Name1] AS [Name1], [Project2].[ListPrice] AS [ListPrice], [Project2].[ProductID] AS [ProductID]
FROM ( SELECT 
 [Limit1].[ProductSubcategoryID] AS [ProductSubcategoryID],  [Limit1].[Name] AS [Name],  [Limit1].[C1] AS [C1],  [Limit1].[C2] AS [C2],  [Limit1].[C3] AS [C3],  [Limit1].[C4] AS [C4],  [Limit1].[C5] AS [C5],  [Extent2].[ProductID] AS [ProductID],  [Extent2].[Name] AS [Name1],  [Extent2].[ListPrice] AS [ListPrice],  CASE WHEN ([Extent2].[ProductID] IS NULL) THEN CAST(NULL AS int) ELSE 1 END AS [C6], 
 CASE WHEN ([Extent2].[ProductID] IS NULL) THEN CAST(NULL AS varchar(1)) ELSE CASE WHEN ([Extent2].[ProductID] IS NULL) THEN N'' ELSE N'AdventureWorksModel.Product' END END AS [C7], 
 CASE WHEN ([Extent2].[ProductID] IS NULL) THEN CAST(NULL AS varchar(1)) ELSE N'Name,ListPrice,ProductID' END AS [C8], 
 CASE WHEN ([Extent2].[ProductID] IS NULL) THEN CAST(NULL AS int) ELSE 1 END AS [C9]
 FROM   (SELECT TOP (5) [Project1].[ProductSubcategoryID] AS [ProductSubcategoryID], [Project1].[Name] AS [Name], [Project1].[C1] AS [C1], [Project1].[C2] AS [C2], [Project1].[C3] AS [C3], [Project1].[C4] AS [C4], [Project1].[C5] AS [C5]
  FROM ( SELECT 
   [Extent1].[ProductSubcategoryID] AS [ProductSubcategoryID], [Extent1].[Name] AS [Name], 1 AS [C1], 1 AS [C2], 
   CASE WHEN ([Extent1].[ProductSubcategoryID] IS NULL) THEN N'' ELSE N'AdventureWorksModel.ProductSubcategory' END AS [C3], 
   N'Name,ProductSubcategoryID' AS [C4], 
   N'Product' AS [C5]
   FROM [Production].[ProductSubcategory] AS [Extent1]
  )  AS [Project1]
  ORDER BY [Project1].[ProductSubcategoryID] ASC ) AS [Limit1]
 LEFT OUTER JOIN [Production].[Product] AS [Extent2] ON [Limit1].[ProductSubcategoryID] = [Extent2].[ProductSubcategoryID]
)  AS [Project2]
ORDER BY [Project2].[ProductSubcategoryID] ASC, [Project2].[C9] ASC
```

Görüldüğü üzere Ado.Net Data Services v1.5 CTP2 ile gelen Projection özelliği performans kazanımı elde etmemizi sağlayacak derecede önemli bir özellik olarak karşımıza çıkmaktadır. Bu yazımızda kullandığımız sorgular aşağıdaki gibidir.

- http://localhost:1714/AdventureServices.svc/Product?$select=ProductID,Name,ListPrice ->(Product kümesinden ProductID, Name ve ListPrice alanları alınır)
- http://localhost:1714/AdventureServices.svc/ProductSubcategory?$select=Name,Product -> (ProductSubcategory kümesinden Name alınır, her bir alt kategoriye bağlı Product kümelerinin sadece linkleri getirilir.)
- http://localhost:1714/AdventureServices.svc/ProductSubcategory?$select=Name,Product&$expand=Product&$top=2 -> (Bir önceki sorgu değerlendirilir ama Product kümesinin tüm üyeleri ve sadece ilk iki alt kategori tipi çekilir)
- http://localhost:1714/AdventureServices.svc/ProductSubcategory?$select=Name,Product/Name,Product/ListPrice&$expand=Product&$top=5 -> (Bir önceki sorgu çalışır ancak Product kümesinden sadece Name ve ListPrice alanları hesaba katılır. Alt kategorilerinde ilk 10 adedi getirilir.)

Bakalım Ado.Net Data Services 1.5 CTP2 tarafında bizleri başka ne gibi sürprizler beklemekte. Bu konularıda ilerleyen yazılarımızda değerlendirmeye çalışıyor olacağız. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Projections.rar (53,71 kb)](/assets/files/2009/Projections.rar)