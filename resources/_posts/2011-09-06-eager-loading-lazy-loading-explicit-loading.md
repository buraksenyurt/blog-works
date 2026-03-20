---
layout: post
title: "Eager Loading, Lazy Loading, Explicit Loading"
date: 2011-09-06 20:30:00 +0300
categories:
  - entity-framework
tags:
  - entity-framework
  - csharp
  - linq
  - http
---
Şöyle basit tek bir Main metodu içerisinde, Entity Framework'teki Loading çeşitlerini görmek ister miydiniz?

![Winking smile](/assets/images/2011/wlEmoticon-winkingsmile_64.png)

Öyleyse aşağıdaki kod bloğu oldukça işinize yarayacaktır diye düşünüyorum. Senaryolar oldukça basit. Meşhur Chinook veritabanında yer alan Artist ve buna bağlı Album tablolarını ele alıyoruz.

Eager Loading Senaryosunda Artistler ve bunlara bağlı olan tüm Albumlerin Sub Select içeren bir Select sorgusunda tek seferde yüklendiğine şahit olmaktayız.

Varsayılan olarak açık olan Lazy Loading senaryosunda ise 1 numaralı Artist ve buna bağlı Albümler çekiliyor. Dikkat edilmesi gereken artiste bağlı albümlerin sorgulandığı anda arka planda bir SQL sorgusu ile bağlı veri kümesinin çekiliyor olması. Yani lazım olduğunda.

![Winking smile](/assets/images/2011/wlEmoticon-winkingsmile_64.png)

Son olarak Explicit Loading senaryosunda ise Lazy Loading'deki davranış biçiminin kodlamacı tarafında açık bir şekilde yapılması durumuna şahit oluyoruz. Biz Load metodunu çağırmadığımız sürece bağlı veri kümesi yüklenmiyor.

İşte kod parçamız.

```csharp
using System; 
using System.Linq;

namespace ConsoleApplication5 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            #region Eager Loading

            using (ChinookEntities context = new ChinookEntities()) 
            { 
                // Eager Loading için LazyLoading özelliği kapatılmış olmalıdır 
                context.ContextOptions.LazyLoadingEnabled = false;

                // Tek bir Select sorgusu içerisinde Sub Select kullanılarak Hem Artist hem de bağlı Album kümeleri yüklenir 
                var resultSet = from a in context.Artists.Include("Albums") 
                                select a;

                //foreach (var r in resultSet) 
                //{ 
                //    Console.WriteLine(r.Name); 
                //    foreach (var album in r.Albums) 
                //    { 
                //        Console.WriteLine("\t{0}",album.Title); 
                //    } 
                //} 
            }

            #region SQL Sorgusu 
            
            // Eager Loading için arka planda çalışan SQL Sorgusu şu şekildedir 
            //            SELECT 
            //[Project1].[ArtistId] AS [ArtistId], 
            //[Project1].[Name] AS [Name], 
            //[Project1].[C1] AS [C1], 
            //[Project1].[AlbumId] AS [AlbumId], 
            //[Project1].[Title] AS [Title], 
            //[Project1].[ArtistId1] AS [ArtistId1] 
            //FROM ( SELECT 
            //    [Extent1].[ArtistId] AS [ArtistId], 
            //    [Extent1].[Name] AS [Name], 
            //    [Extent2].[AlbumId] AS [AlbumId], 
            //    [Extent2].[Title] AS [Title], 
            //    [Extent2].[ArtistId] AS [ArtistId1], 
            //    CASE WHEN ([Extent2].[AlbumId] IS NULL) THEN CAST(NULL AS int) ELSE 1 END AS [C1] 
            //    FROM  [dbo].[Artist] AS [Extent1] 
            //    LEFT OUTER JOIN [dbo].[Album] AS [Extent2] ON [Extent1].[ArtistId] = [Extent2].[ArtistId] 
            //)  AS [Project1] 
            //ORDER BY [Project1].[ArtistId] ASC, [Project1].[C1] ASC

            #endregion

            #endregion

            #region Lazy Loading

            // Varsayılan olarak Lazy Loading özelliği açıktır

            using (ChinookEntities context = new ChinookEntities()) 
            { 
                var resultSet = from a in context.Artists 
                                where a.ArtistId==1 
                                select a;

                //foreach (var r in resultSet) 
                //{ 
                //    Console.WriteLine(r.Name); 
                //    foreach (var album in r.Albums) // Bağlı Album kümesi talep edildiğinde ikinci Select sorgusu otomatik olarak çalışır 
                //    { 
                //        Console.WriteLine("\t{0}", album.Title); 
                //    } 
                //}

                #region SQL Sorguları

                //                SELECT 
                //[Extent1].[ArtistId] AS [ArtistId], 
                //[Extent1].[Name] AS [Name] 
                //FROM [dbo].[Artist] AS [Extent1] 
                //WHERE 1 = [Extent1].[ArtistId]

                //                exec sp_executesql N'SELECT 
                //[Extent1].[AlbumId] AS [AlbumId], 
                //[Extent1].[Title] AS [Title], 
                //[Extent1].[ArtistId] AS [ArtistId] 
                //FROM [dbo].[Album] AS [Extent1] 
                //WHERE [Extent1].[ArtistId] = @EntityKeyValue1',N'@EntityKeyValue1 int',@EntityKeyValue1=1

                #endregion

            }

            #endregion

            #region Explicit Loading

            // Bağlı veri kümesi bilinçli olarak Load metodu ile yüklenir

            using (ChinookEntities context = new ChinookEntities()) 
            { 
                // Explicit Loading için LazyLoading özelliği kapatılmış olmalıdır 
                context.ContextOptions.LazyLoadingEnabled = false;

                var resultSet = from a in context.Artists 
                                where a.ArtistId==1 
                                select a;

                foreach (var r in resultSet) 
                { 
                    Console.WriteLine(r.Name);

                    if (!r.Albums.IsLoaded) //Albums seti yüklenmediyse 
                        r.Albums.Load();  //yükle (Burada ikinci Select sorgusu çalışır)

                    foreach (var a in r.Albums) 
                    { 
                        Console.WriteLine("\t{0}",a.Title); 
                    } 
                } 
            }

            #region SQL Sorguları

            //            SELECT 
            //[Extent1].[ArtistId] AS [ArtistId], 
            //[Extent1].[Name] AS [Name] 
            //FROM [dbo].[Artist] AS [Extent1] 
            //WHERE 1 = [Extent1].[ArtistId]

            //    exec sp_executesql N'SELECT 
            //[Extent1].[AlbumId] AS [AlbumId], 
            //[Extent1].[Title] AS [Title], 
            //[Extent1].[ArtistId] AS [ArtistId] 
            //FROM [dbo].[Album] AS [Extent1] 
            //WHERE [Extent1].[ArtistId] = @EntityKeyValue1',N'@EntityKeyValue1 int',@EntityKeyValue1=1

            #endregion

            #endregion 
        } 
    } 
}
```

[ConsoleApplication5.rar (98,75 kb)](/assets/files/2011/ConsoleApplication5.rar)

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

Chinook veritabanını [bu adresten](http://chinookdatabase.codeplex.com/) indirebilirsiniz.
