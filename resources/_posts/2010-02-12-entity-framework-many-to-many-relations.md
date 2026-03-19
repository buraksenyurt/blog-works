---
layout: post
title: "Entity Framework - Many To Many Relations"
date: 2010-02-12 01:05:00 +0300
description: "Bundan yıllar önce üniversiteden arkadaşım Orkun Şentürk ile birlikte Altunizade Capitol alışveriş merkezinde Dreamcatcher isimli bir bilim kurgu gerilim filmine gittiğimizi hatırlıyorum. (Aslında bilim kurgu filmlerin tam bir hayranıyımdır. Ancak seyrettiklerimin hiç biri Starwars veya Terminator gibilerinin yerini tutmamakta) Film Stephen King' in bir romanından uyarlanmıştı. Sevgili arkadaşım Orkun kitabını okuyarak geldiğinden, film üzerinde daha iyi yorumlar yapmıştı ancak ortak kanımız çok başarılı olmadığı yönündeydi. Açıkçası fazla etkilenmemiştik."
categories:
  - entity-framework
tags:
  - entity-framework
  - csharp
  - dotnet
  - ado-net
  - linq
  - concurrency
  - performance
  - visual-studio
  - rc
---
# Entity Framework - Many To Many Relations
![blg131_GirisNew.jpg](/assets/images/2010/blg131_GirisNew.jpg)

Merhaba Arkadaşlar,

Bundan yıllar önce üniversiteden arkadaşım Orkun Şentürk ile birlikte Altunizade Capitol alışveriş merkezinde Dreamcatcher isimli bir bilim kurgu gerilim filmine gittiğimizi hatırlıyorum. (Aslında bilim kurgu filmlerin tam bir hayranıyımdır. Ancak seyrettiklerimin hiç biri Starwars veya Terminator gibilerinin yerini tutmamakta) Film Stephen King'in bir romanından uyarlanmıştı. Sevgili arkadaşım Orkun kitabını okuyarak geldiğinden, film üzerinde daha iyi yorumlar yapmıştı ancak ortak kanımız çok başarılı olmadığı yönündeydi. Açıkçası fazla etkilenmemiştik.

Özellikle filmden çıktıktan sonra yaptığımız muhabbet bunu doğrular nitelikteydi..Net Framework 1.1 ile yazmakta olduğumuz programda kullandığımız SQL 2000 veritabanındaki many-to-many ilişkileri anlamaya çalışıyorduk. O zamanlar bizim için SQL veritabanına bağlanabilmek bile bir lüks iken bu tip alengirli konular korkutucu geliyordu. Filmden daha çok gerildiğimiz bir konuydu.

![Tongue out](/assets/images/2010/smiley-tongue-out.gif)

Sonuçta bende sevgili Orkun'da evlerimizin yolunu tuttuğumuzda, odamızda bizi bekleyen masa üstü bilgisayarlar ile ne yapacağımızı gayet iyi biliyorduk. Many-To-Many olayını kavramak.

Gerçektende yazılımın ilk yıllarında hepimiz benzer durumları tecrübe etmekteyiz. Pek çok konuyu anlamakta, öğrenmekte güçlük çekiyoruz. Tabi içimizdeki öğrenme arzusunun gücüne bağlı olaraktan, ya çok çalışarak ya da önemli ve dikkat edilmesi gereken noktaları anında yakalayarak ilerleyişimizi sürdürüyoruz. Bir yazılımcı, yıllar sonra geldiği noktadan geriye doğru dönüp baktığındaysa, ilerleyişinin ne kadar hızlı olduğunu net bir şekilde görebilir aslında. Ancak bu yeterli değildir. Dilerseniz sözü fazla uzatmadan bu günkü konumuza geçelim. Bu gün yine Many-To-Many ilişkileri inceliyor olacağız. Ancak bu kez olayı Entity Framework üzerinden değerlendireceğiz. Başlamadan önce veritabanı üzerindeki tablolar arası ilişkilerden (Relations) kısaca bahsetmekte yarar olduğu kanısındayım.

Genel olarak tablolar arasında bire bir (one-to-one), bire çok (one-to-many), çoğa çok (many-to-many) ve self referencing ilişkilerden bahsedilmektedir. Bire-bir ilişkilerde tabloda yer alan bir satırın diğer tabloda yine tek bir satır ile ilişkilendirilmesi söz konusudur. Yaygın olarak kullanılan bire-çok ilişkilerde ise, bir tablodaki satıra diğer bir tablodan n adet satırın bağlanabilmesi mümkündür. Söz gelimi ürün kategorilerinin tutulduğu tablodaki bir satırın, ürünlerin tutulduğu tablodaki n satırı referans etmesi gibi. Self-Referencing ilişkilerde ise bir tablonun herhangibir satırının/satırlarının, kendi içerisindeki bir satırı referans etmesi durumu söz konusudur. Örneğin bir şirketin organizasyon ağacında yer alan bir personelin/personel topluluğunun kime bağlı olduğunun tutulduğu tablolarda, bu tip ilişkiler kullanılabilir.

Gelelim çoğa-çok ilişkilere. Örneğin filmler ve oyunculara ait tablolar olduğunu düşünelim. Burada bir oyuncunun birden fazla filmde rol alması veya bir film içerisinde birden fazla oyuncunun bulunması çok normal ve olasıdır. Bu sebepten her iki tablo birbirleri üzerinde n sayıda satırı referans edebilmektedir. Bu tip bir durumda tablolar arasındaki ilişkiyi ifade etmek için ek bir tablonun kullanılması söz konusudur. Bu tablo üzerinden sağlanan ilişkiler sayesinde çoğa-çok ilişkinin gerçeklenmesi mümkün olabilir. Peki SQL tarafında ek bir tablo yardımıyla ele alınan bu ilişkilerin Entity Framework tarafındaki yansıması nasıldır? Gelin bu durumu basit bir örnek yardımıyla incelemeye çalışalım.

İlk olarak Chinook veritabanında yer alan ve SQL tarafındaki diagramda aşağıdaki şekilde görülen ilişkilere sahip olan tabloları kullanacağımızı belirtelim.

![blg131_SqlDiagram.gif](/assets/images/2010/blg131_SqlDiagram.gif)

Buradaki senaryoda Playlist ve Track tabloları arasında çoğa-çok ilişki olduğu görülmektedir. Bir başka deyişle bir Playlist birden fazla Track satırına referans edebileceği gibi, bir Track satırı da birden fazla Playlist satırına referans edebilir. Şimdi bu ilişkilerin Entity Framework tarafındaki oluşumuna bir bakalım. (Bu amaçla Visual Studio 2010 Ultimate RC ve Ado.Net Entity Framework 4.0 sürümlerini kullanıyor olacağız. Ancak bu konunun Entity Framework'ün önceki sürümünde de aynen geçerli olduğunu hatırlatalım.) Özellikle 3 tabloyu da seçtiğimizi düşünelim.

![blg131_Selection.gif](/assets/images/2010/blg131_Selection.gif)

Dikkat edileceği üzere Playlist, Track ve PlaylistTrack tablolarının tamamı seçilmiştir. Sihirbaz adımlarını tamamladığımızda ise, aşağıdaki Model diagramının oluştuğunu görürüz.

![blg131_EntityDiagram.gif](/assets/images/2010/blg131_EntityDiagram.gif)

Hımmm...

![Wink](/assets/images/2010/smiley-wink.gif)

Dikkat edileceği üzere PlaylistTrack tablosu model diagramına dahil edilmemiştir. Neden?

SQL tarafına bakıldığında PlaylistTrack tablosu üzerinde sadece Playlist ve Track tablolarına ait Primary Key alanları bulunmaktadır. Bunlar dışında ek bir alan yoktur. Bir başka deyişle veritabanı tarafında Playlist ve Track tablolarının many-to-many ilişkilerinin sağlandığı tablodur. Ancak Entity Framework tarafında tabloların sınıflar yardımıyla ve bu sınıfların referans ettikleri diğer sınıfların ise özellikler yardımıyla belirtildiği bilinmektedir. Dolayısıyla Entity Framework tarafında PlaylistTrack isimli bir sınıfın oluşturulmasının bir anlamı yoktur. Dahası olmamasının bir kaybı da yoktur. Bu yüzden Playlist ve Track sınıfları birbirlerine EntityCollection tipinden olan navigasyon özellikleri yardımıyla (Tracks ve Playlists) doğrudan bağlanmışlardır. Bu durum ilişkiyi sağlayan bileşenin (Association nesnesi) özelliklerine bakıldığında da net bir şekilde görülebilir.

![blg131_Relations.gif](/assets/images/2010/blg131_Relations.gif)

Şimdi kod tarafında Many-To-Many ilişkileri nasıl ele alacağımıza bakmaya çalışalım. Örneğin TV-Show listesine ait Track bilgilerini getirmek istediğimizi düşünelim. Bunun için aşağıdaki gibi bir kodlama yapabiliriz.

```csharp
using System;
using System.Linq;

namespace ManyToMany
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ChinookEntities entities = new ChinookEntities())
            {
                Playlist result = (from pList in entities.Playlists
                                   where pList.PlaylistId == 10
                                   select pList).First();

                Console.WriteLine("{0}-{1}", result.PlaylistId, result.Name);
                foreach (var t in result.Tracks)
                {
                    Console.WriteLine("\t {0}[{1}]", t.Name, t.Milliseconds.ToString());
                }
            }
        }
    }
}
```

Buna göre aşağıdaki sonuçları elde ederiz.

![blg131_Runtime.gif](/assets/images/2010/blg131_Runtime.gif)

Bu kod parçasının çalışması sonucunda SQL tarafında, aşağıdaki sorgunun oluşturulduğu gözlemlenecektir.

İlk önce Playlist bilgisinin çekilmesi,

```text
SELECT TOP (1) 
[Extent1].[PlaylistId] AS [PlaylistId], 
[Extent1].[Name] AS [Name]
FROM [dbo].[Playlist] AS [Extent1]
WHERE 10 = [Extent1].[PlaylistId]
```

sonrasında ise ilgili Playlist'e bağlı Track'lerin Inner Join sorgusu ile elde edilmesi gerçekleşecektir.

```text
exec sp_executesql N'SELECT 
[Extent2].[TrackId] AS [TrackId], 
[Extent2].[Name] AS [Name], 
[Extent2].[AlbumId] AS [AlbumId], 
[Extent2].[MediaTypeId] AS [MediaTypeId], 
[Extent2].[GenreId] AS [GenreId], 
[Extent2].[Composer] AS [Composer], 
[Extent2].[Milliseconds] AS [Milliseconds], 
[Extent2].[Bytes] AS [Bytes], 
[Extent2].[UnitPrice] AS [UnitPrice]
FROM  [dbo].[PlaylistTrack] AS [Extent1]
INNER JOIN [dbo].[Track] AS [Extent2] ON [Extent1].[TrackId] = [Extent2].[TrackId]
WHERE [Extent1].[PlaylistId] = @EntityKeyValue1',N'@EntityKeyValue1 int',@EntityKeyValue1=10
```

Birde ters tarafan gitmeye çalışalım. Örneğin bir Track'ın geçtiği Playlist'leri bulmak istediğimizi düşünelim. Bu durumda aşağıdaki gibi bir kod parçasını göz önüne alabiliriz.

```csharp
using System;
using System.Linq;

namespace ManyToMany
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ChinookEntities entities = new ChinookEntities())
            {
                Track track = (from t in entities.Tracks
                               where t.TrackId == 1
                               select t).First();

                Console.WriteLine("{0}-{1}-[{2}]",track.TrackId.ToString(),track.Name,track.Milliseconds.ToString());

                foreach (var p in track.Playlists)
                {
                    Console.WriteLine("\t{0}-{1}",p.PlaylistId.ToString(),p.Name);
                }
            }
        }
    }
}
```

Bu kod parasına göre, çalışma zamanında TrackId değeri 1 olan parçanın geçtiği Playlist'lerin listesini elde edebildiğimizi görürüz.

![blg131_Runtime2.gif](/assets/images/2010/blg131_Runtime2.gif)

Bu kod parçası ise bir öncekine benzer olaraktan aşağıdaki SQL sorgularının çalıştırılmalarına neden olacaktır.

Önce Track bilgileri çekilecek,

```csharp
SELECT TOP (1) 
[Extent1].[TrackId] AS [TrackId], 
[Extent1].[Name] AS [Name], 
[Extent1].[AlbumId] AS [AlbumId], 
[Extent1].[MediaTypeId] AS [MediaTypeId], 
[Extent1].[GenreId] AS [GenreId], 
[Extent1].[Composer] AS [Composer], 
[Extent1].[Milliseconds] AS [Milliseconds], 
[Extent1].[Bytes] AS [Bytes], 
[Extent1].[UnitPrice] AS [UnitPrice]
FROM [dbo].[Track] AS [Extent1]
WHERE 1 = [Extent1].[TrackId]
```

sonrasında ise ilgili Track satırının geçtiği Playlist satırlarının bulunması için gerekli Inner Join sorgusu çalıştırılacaktır.

```text
exec sp_executesql N'SELECT 
[Extent2].[PlaylistId] AS [PlaylistId], 
[Extent2].[Name] AS [Name]
FROM  [dbo].[PlaylistTrack] AS [Extent1]
INNER JOIN [dbo].[Playlist] AS [Extent2] ON [Extent1].[PlaylistId] = [Extent2].[PlaylistId]
WHERE [Extent1].[TrackId] = @EntityKeyValue1',N'@EntityKeyValue1 int',@EntityKeyValue1=1
```

Pek tabi olarak Many-to-Many ilişki söz konusu olan tablolar üzerinden Update, Delete veya Insert işlemleri de yapılabilir. Burada, özellikle Delete operasyonlarında tarafların nasıl tepki göstereceği önemlidir. Normal şartlarda Association nesnesi üzerinde yer alan End1 OnDelete ve End2 OnDelete isimli özelliklerin değerleri none olarak belirlenmiştir. Ancak istenirse bunlar Cascade değerine çekilebilir. Pek tabi ilişkinin nasıl bir tepki vereceğine bağlı olaraktan SQL tarafından Foreign Key'ler ile alakalı istisnaların alınması da muhtemeldir. Şimdi bu durumu incelemeye çalışalım.

```csharp
using System;
using System.Linq;

namespace ManyToMany
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ChinookEntities entities = new ChinookEntities())
            {
                #region Delete and Foreign Key Violation

                var trackOnDelete = (from t in entities.Tracks
                                     where t.TrackId == 3502
                                     select t).First();

                entities.DeleteObject(trackOnDelete);

                entities.SaveChanges();

                #endregion
            }
        }
    }
}
```

Bu kod parçasına göre TrackId değeri 3502 olan Track satırı silinmeye çalışılmaktadır. Ancak Track'lar çok doğal olarak Playlist'lere bağlıdır. Bu nedenle silme işlemi sırasında aşağıdaki çalışma zamanı istisnası (Runtime Exception) alınacaktır.

![blg131_Exception.gif](/assets/images/2010/blg131_Exception.gif)

Burada sebep, söz konusu alan için bir Relation'ın var olmasıdır. Dolayısıyla öncelikle silinmek istenen Track satırı ile bağlı olduğu Playlist satırları arasındaki ilişkileri kaldırmak gerekmektedir. Silme operasyonunun gerçeklenmesi için, silinmek istenen Track'ın dahil olduğu Playlist nesnelerini bulup, herbirinin Tracks özelliği üzerinden Remove metodunun çalıştırılması yeterlidir. Mi acaba?

![Undecided](/assets/images/2010/smiley-undecided.gif)

Bu tip bir kod yazılmak istendiğinde elde edilen Playlist.Tracks özellikleri üzerinden yapılan silme hareketleri, koleksiyonunun değişmesine neden olacağından, çalışma zamanında Concurrency hatası alınacaktır. Çok şükür ki artık elimizin altında.Net Framework 4.0 içerisine gömülü olarak gelen Concurrent koleksiyonlar bulunmaktadır.([Concurrent Collections (Eş Zamanlı Koleksiyonlar) [Beta 1]](https://www.buraksenyurt.com/post/Concurrent-Collections-(Es-Zamanli-Koleksiyonlar).aspx), [Concurrent Collections: Macera BlockingCollection ile Devam Ediyor [Beta 1]](https://www.buraksenyurt.com/post/Concurrent-Collections-(Es-Zamanlc4b1-Koleksiyonlar)-Macera-Devam-Ediyor.aspx)) Buna göre kodu aşağıdaki gibi geliştirebiliriz.

```csharp
using System;
using System.Linq;
using System.Collections.Concurrent;

namespace ManyToMany
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ChinookEntities entities = new ChinookEntities())
            {
                #region Delete and Foreign Key Violation

                var trackOnDelete = (from t in entities.Tracks
                                     where t.TrackId == 3503
                                     select t).First();

                ConcurrentBag<Playlist> playList = new ConcurrentBag<Playlist>(trackOnDelete.Playlists);
                foreach (var pl in playList)
                {
                    pl.Tracks.Remove(trackOnDelete);
                }

                entities.DeleteObject(trackOnDelete);

                entities.SaveChanges();

                #endregion
            }
        }
    }
}
```

Biraz performans kaybı söz konusu olabilir ancak silme işlemi başarılı bir şekilde gerçekleştirilebilecektir. Özellikle SQL tarafında çalıştırılan sorgulara bakıldığında, aşağıdaki ifadelerin icra edildiği gözlemlenir.

Önce Track ve Playlist tabloları arasındaki çoğa-çok ilişkiyi sağlayan PlaylistTrack tablosundaki ilgili satırlar silinir.

```text
exec sp_executesql N'delete [dbo].[PlaylistTrack]
where (([PlaylistId] = @0) and ([TrackId] = @1))',N'@0 int,@1 int',@0=1,@1=3503

exec sp_executesql N'delete [dbo].[PlaylistTrack]
where (([PlaylistId] = @0) and ([TrackId] = @1))',N'@0 int,@1 int',@0=5,@1=3503

exec sp_executesql N'delete [dbo].[PlaylistTrack]
where (([PlaylistId] = @0) and ([TrackId] = @1))',N'@0 int,@1 int',@0=8,@1=3503

exec sp_executesql N'delete [dbo].[PlaylistTrack]
where (([PlaylistId] = @0) and ([TrackId] = @1))',N'@0 int,@1 int',@0=12,@1=3503

exec sp_executesql N'delete [dbo].[PlaylistTrack]
where (([PlaylistId] = @0) and ([TrackId] = @1))',N'@0 int,@1 int',@0=13,@1=3503
```

Artık sorun yoktur, nitekim Track tablosu ile Playlist arasındaki ilişkiler ortadan kalkmıştır. Buna göre son olarak, Track tablosundan ilgili satırın silinmesi işlemi gerçekleştirilir.

```text
exec sp_executesql N'delete [dbo].[Track]
where ([TrackId] = @0)',N'@0 int',@0=3503
```

Insert ve Update işlemlerinin incelenmesini de siz değerli okurlarıma bırakıyorum. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[ManyToMany_RC.rar (47,16 kb)](/assets/files/2010/ManyToMany_RC.rar) [Örnek Visual Studio 2010 Ultimate RC sürümü üzerinde geliştirilmiş ve test edilmiştir]