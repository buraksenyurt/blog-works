---
title: "Eski Dost ve Entity Framework 4.0"
pubDate: 2010-02-09 07:28:00
categories:
  - Entity Framework
tags:
  - Entity Framework
  - C#
  - .NET
  - ADO.NET
  - LINQ
  - SQL Server
  - HTTP
  - Visual Studio
  - GitHub
  - Rc
---

# Eski Dost ve Entity Framework 4.0
Merhaba Arkadaşlar,

Bildiğiniz üzere.Net Framework 4.0 ve Visual Studio 2010 sürümlerinin çıkmasına az bir zaman kaldı (Hatta yarın RC sürümü herkese açık olacak). Blog yazısını hazırladığım zaman itibariyle Microsoft'taki güvenilir kaynaklar ve MSDN yazarlarından edinilen bilgilere göre, sürümün Nisan 2010 içerisinde çıkması bekleniyor. Tabi yeni bir sürüm denilince eski sürüm ile aradaki farklılıkları bilmek, nereden nereye gelindiği ve nelerin düzeltildiğini öğrenmek, beklentilerin karşılanıp karşılanmadığına bakmak oldukça önemli. Bende buna istinaden yazımızın ilerleyen kısımlarında, Ado.Net Entity Framework 4.0 ile bir önceki versiyonu arasında, sorgu teknikleri açısından oluşan farkları aktarmaya çalışıyor olacağım.

Ado.Net takımının 4.0 sürümündeki hedeflerinden biriside SQL sorgularının daha anlaşılır olmasını sağlamak. Buna ek olarak gereksiz SQL ifadelerinin kullanılmasından uzaklaşılmaya çalışılmış ve hatta önceki Entity Framework sürümünde SQL sorgusuna dönüştürülemeyen LINQ ifadeleri kullanılabilir hale getirilmiş. Tabi daha da fazla fark bulunabilir. Ancak MSDN ve Ado.Net takımının gerek blog yazılarından gerek Webcast'lerinden takip ettiğim kadarı ile aşağıda yer alan 4 önemli farklılık göze çarpmaktadır. Çok doğal olarak söz konusu iyileştirmeler LINQ ifadelerinin SQL sorgularına dönüştürülmesinde devreye giren motor üzerinde yer almaktadır.

Not: Örneklerimizde yer alan LINQ sorguları.Net Framework 3.5 odaklı olan Visual Studio 2008 ve.Net Framework 4.0 odaklı olan Visual Studio 2010 Ultimate Beta 2 ürünleri üzerinde yazılmıştır. Özellikle Ado.Net Entity Framework 4.0 üzerinden daha nihai bir sürüme ulaşılmadığı için ilereyen zamanlarda değişiklikler gözlemlenebilir. LINQ sorgularının icrası sırasında, SQL Server Profiler aracı kullanılarak arka planda yürütülen SQL sorgularının elde edilmesi sağlanmıştır. Ayrıca örnekte [Codeplex üzerinden sunulan Chinook veritabanı (Giihub'a taşındı)](https://github.com/lerocha/chinook-database)kullanılmıştır.

## Vaka 1: Sorgularda In Desteği Açıklama

Ado.Net Entity Framework 4.0 öncesindeki sürümde SQL tarafında In anahtar kelimesine dönüşebilecek sorgu desteği bulunmamaktadır. 4.0 versiyonunda In'e dönüştürülebilme desteği belirli ölçüde bulunmaktadır. Aşağıdaki LINQ sorgusuna göre, Berlin ve Paris şehirlerinde yer alan müşteriler elde edilmeye çalışılmaktadır. 4.0 öncesi sürümde bu tip bir LINQ sorgusunda yer alan Contains metodunun SQL tarafına dönüştürülemediği görülür. Ancak 4.0 versiyonunda SQL tarafına In olarak aktarılması sağlanmaktadır.

### Önceki Versiyon - LINQ Sorgusu

```csharp
string[] cityNames = { "Berlin", "Paris" };
                var result = from customer in entites.Customer
                             where cityNames.Contains(customer.City)
                             select customer;

                foreach (var r in result)
                {
                    Console.WriteLine("{0} {1} {2}", r.Email, r.FirstName, r.LastName);
                }
```

### Önceki Versiyon - SQL Sorgusu

Bir SQL sorgusu yürütülememektedir nitekim çalışma zamanında aşağıdaki Exception mesajı alınacaktır.

![blg130_Exception.gif](images/blg130_Exception.gif)

### 4.0 - LINQ Sorgusu

```csharp
string[] cityNames = { "Berlin", "Paris" };
                var result = from customer in entites.Customers
                             where cityNames.Contains(customer.City)
                             select customer;

                foreach (var r in result)
                {
                    Console.WriteLine("{0} {1} {2}", r.Email, r.FirstName, r.LastName);
                }
```

### 4.0 - SQL Sorgusu

```text
SELECT 
[Extent1].[CustomerId] AS [CustomerId], 
[Extent1].[FirstName] AS [FirstName], 
[Extent1].[LastName] AS [LastName], 
[Extent1].[Company] AS [Company], 
[Extent1].[Address] AS [Address], 
[Extent1].[City] AS [City], 
[Extent1].[State] AS [State], 
[Extent1].[Country] AS [Country], 
[Extent1].[PostalCode] AS [PostalCode], 
[Extent1].[Phone] AS [Phone], 
[Extent1].[Fax] AS [Fax], 
[Extent1].[Email] AS [Email], 
[Extent1].[SupportRepId] AS [SupportRepId]
FROM [dbo].[Customer] AS [Extent1]
WHERE [Extent1].[City] IN (N'Berlin',N'Paris')
```

## Vaka 2 - Gruplamada Cast Sorunu Açıklama

Gruplama uygulanmış olan bir LINQ sorgusunda Count genişletme metodunun (Extension Method) kullanılması halinde bir önceki versiyonda Cast operatörünün SQL sorgusuna dahil edildiği görülür. Ancak 4.0 versiyonunda bu gereksiz durum düzeltilmiştir.

### Önceki Versiyon - LINQ Sorgusu

```csharp
var result = from track in entites.Track
                             group track by track.Composer into trackGrp
                             select new
                             {
                                 trackGrp.Key,
                                 Count = trackGrp.Count(),
                                 Sum = trackGrp.Sum<Track>(t => t.UnitPrice),
                                 Max = trackGrp.Max<Track>(t => t.UnitPrice),
                                 Min = trackGrp.Min<Track>(t => t.UnitPrice)
                             };

                foreach (var r in result)
                {
                    Console.WriteLine(r.ToString());
                }
```

### Önceki Versiyon - SQL Sorgusu

```text
SELECT 
1 AS [C1], 
[GroupBy1].[K1] AS [Composer], 
[GroupBy1].[A1] AS [C2], 
[GroupBy1].[A2] AS [C3], 
[GroupBy1].[A3] AS [C4], 
[GroupBy1].[A4] AS [C5]
FROM ( SELECT 
 [Extent1].[Composer] AS [K1], 
 COUNT( CAST( 1 AS bit)) AS [A1], 
 SUM([Extent1].[UnitPrice]) AS [A2], 
 MAX([Extent1].[UnitPrice]) AS [A3], 
 MIN([Extent1].[UnitPrice]) AS [A4]
 FROM [dbo].[Track] AS [Extent1]
 GROUP BY [Extent1].[Composer]
)  AS [GroupBy1]
```

### 4.0 - LINQ Sorgusu

```csharp
var result = from track in entites.Tracks
                             group track by track.Composer into trackGrp
                             select new
                             {
                                 trackGrp.Key,
                                 Count = trackGrp.Count(),
                                 Sum = trackGrp.Sum<Track>(t => t.UnitPrice),
                                 Max = trackGrp.Max<Track>(t => t.UnitPrice),
                                 Min = trackGrp.Min<Track>(t => t.UnitPrice)
                             };

                foreach (var r in result)
                {
                    Console.WriteLine(r.ToString());
                }
```

### 4.0 - SQL Sorgusu

```text
SELECT 
1 AS [C1], 
[GroupBy1].[K1] AS [Composer], 
[GroupBy1].[A1] AS [C2], 
[GroupBy1].[A2] AS [C3], 
[GroupBy1].[A3] AS [C4], 
[GroupBy1].[A4] AS [C5]
FROM ( SELECT 
 [Extent1].[Composer] AS [K1], 
 COUNT(1) AS [A1], 
 SUM([Extent1].[UnitPrice]) AS [A2], 
 MAX([Extent1].[UnitPrice]) AS [A3], 
 MIN([Extent1].[UnitPrice]) AS [A4]
 FROM [dbo].[Track] AS [Extent1]
 GROUP BY [Extent1].[Composer]
)  AS [GroupBy1]
```

## Vaka 3 - Join sorgularında Is Null Kontrolü

LINQ tarafında icra edilen aşağıdaki gibi bir Join sorgusunda, bir önceki versiyonda üretilen SQL ifadesinde anahtar alan için IS NULL kontrolü yapıldığı görülmektedir. 4.0 sürümünde ise gereksiz olan bu kontrol SQL tarafına aktarılmamaktadır.

### Önceki Versiyon - LINQ Sorgusu

```csharp
var result = from artist in entites.Artist
                             join album in entites.Album
                             on artist.ArtistId equals album.Artist.ArtistId
                             select new
                             {
                                 ArtistName = artist.Name,
                                 AlbumTitle = album.Title
                             };
                foreach (var r in result)
                {
                    Console.WriteLine(r.ToString());
                }
```

### Önceki Versiyon - SQL Sorgusu

```text
SELECT 
1 AS [C1], 
[Extent1].[Name] AS [Name], 
[Extent2].[Title] AS [Title]
FROM  [dbo].[Artist] AS [Extent1]
INNER JOIN [dbo].[Album] AS [Extent2] ON ([Extent1].[ArtistId] = [Extent2].[ArtistId]) OR (([Extent1].[ArtistId] IS NULL) AND ([Extent2].[ArtistId] IS NULL))
```

### 4.0 - LINQ Sorgusu

```csharp
var result = from artist in entites.Artists
                             join album in entites.Albums
                             //on artist.ArtistId equals album.Artist.ArtistId
                             on artist.ArtistId equals album.ArtistId //(Zaten yeni sürümde yandaki gibi yazabiliyoruz artık)
                             select new
                             {
                                 ArtistName = artist.Name,
                                 AlbumTitle = album.Title
                             };

                foreach (var r in result)
                {
                    Console.WriteLine(r.ToString());
                }
```

### 4.0 - SQL Sorgusu

```text
SELECT 
[Extent1].[ArtistId] AS [ArtistId], 
[Extent1].[Name] AS [Name], 
[Extent2].[Title] AS [Title]
FROM  [dbo].[Artist] AS [Extent1]
INNER JOIN [dbo].[Album] AS [Extent2] ON [Extent1].[ArtistId] = [Extent2].[ArtistId]
```

## Vaka 4 - Skip, Take gibi Metod Kullanımlarında Tüm Alanların Sorguya Dahil Edilmesi

Aşağıdaki LINQ sorgusuna göre artistlerin tersten sıralanan adlar listesi ilk 10 satır atlanarak elde edilmektedir. 4.0 öncesi versiyonda bu tip bir LINQ sorgusunun çalıştırılması halinde SQL tarafında oluşturulan Sub SELECT sorgusunda aslında LINQ sorgusuna dahil edilmeyen alanlarında hesaba katıldığı gözlemlenir. 4.0 versiyonunda ise gereksiz alanların SELECT sorgusuna alınmasının önüne geçilmiştir.

### Önceki Versiyon - LINQ Sorgusu

```csharp
var result = (from artist in entites.Artist
                              orderby artist.Name descending
                              select artist.Name).Skip(10);
                foreach (var r in result)
                {
                    Console.WriteLine(r);
                }
```

### Önceki Versiyon - SQL Sorgusu

```text
SELECT 
[Extent1].[Name] AS [Name]
FROM ( SELECT [Extent1].[ArtistId] AS [ArtistId], [Extent1].[Name] AS [Name], row_number() OVER (ORDER BY [Extent1].[Name] DESC) AS [row_number]
 FROM [dbo].[Artist] AS [Extent1]
)  AS [Extent1]
WHERE [Extent1].[row_number] > 10
ORDER BY [Extent1].[Name] DESC
```

### 4.0 - LINQ Sorgusu

```csharp
var result = (from artist in entites.Artists
                              orderby artist.Name descending
                              select artist.Name).Skip(10);

                foreach (var r in result)
                {
                    Console.WriteLine(r);
                }
```

### 4.0 - SQL Sorgusu

```text
SELECT 
[Extent1].[Name] AS [Name]
FROM ( SELECT [Extent1].[Name] AS [Name], row_number() OVER (ORDER BY [Extent1].[Name] DESC) AS [row_number]
 FROM [dbo].[Artist] AS [Extent1]
)  AS [Extent1]
WHERE [Extent1].[row_number] > 10
ORDER BY [Extent1].[Name] DESC
```

Görüldüğü üzere Ado.Net Entity Framework 4.0 sürümünde özellikle SQL sorgularına dönüştürme işlemlerinde, motor üzerinde bazı yenilemelerin yapıldığı ve gereksiz işlemlerin önüne geçilmeye çalışıldığı gözlemlenebilmektedir. Açıkçası bu konu ile ilişkili olaraktan çıkacak kitapları merakla bekliyorum. Özellikle Julia Lerman'ın daha önceki [Programming Entity Framework](http://www.amazon.co.uk/Programming-Entity-Framework-Julia-Lerman/dp/059652028X/ref=sr_1_2?ie=UTF8&s=books&qid=1263557453&sr=8-2)kitabını okuyanlar eminim 4.0 için olan versiyonuda sabırsızlıkla ve merakla bekliyordur. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Differences.rar (183,69 kb)](assets/Differences.rar) [Örnek Visual Studio 2010 Ultimate Beta 2 üzerinde geliştirilmiştir. Yarın public olarak yayınlanacak RC sürümü için test edilmemiştir]
