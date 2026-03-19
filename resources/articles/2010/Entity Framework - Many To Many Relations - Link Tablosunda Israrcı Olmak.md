---
title: "Entity Framework - Many To Many Relations - Link Tablosunda Israrcı Olmak"
pubDate: 2010-03-23 22:35:00
categories:
  - Entity Framework
tags:
  - Entity Framework
  - C#
  - .NET
  - ADO.NET
  - LINQ
  - Generics
  - Visual Studio
  - Rc
---

# Entity Framework - Many To Many Relations - Link Tablosunda Israrcı Olmak
![blg141_Giris.jpg](images/blg141_Giris.jpg)

Merhaba Arkadaşlar,

Bazen bebek adımları ile ilerlememiz gerekir. Özellikle yazılım alanında bazı konuları öğrenirken işin teorisinden önce pratik bir örneği adım adım geliştirmek son derece faydalıdır. Ado.Net Entity Framework ile ilişkili inclemelerimize devam edeceğimiz bu yazımızda bebek adımları ile ilerleyeceğiz.

Hatırlayacağınız üzere son iki yazımızda Many-To-Many ilişkileri nasıl ele alabileceğimizi incelemiştik. Many-To-Many ilişkilerin Entity Model'e olan yansımasında belkide en önemli nokta, ara bağlantı tablosunun taşınmıyor olmasıydı. Bu genellikle ara bağlantı tablosu üzerinde diğer tablolara ait Primary Key alanlarının bulunduğu durumlar düşünülerek meydana gelen bir sonuçtur. Nitekim ara tablonun Entity tarafına taşınmayışının herhangibir olumsuz maliyeti bulunmamaktadır. Ancak bazı durumlarda söz konusu ara bağlantı tablosunun ilerleyen zamanlarda ek alanlar ile genişlemesi söz konusu olabilir. Bu durumda ara bağlantı tablosunun Entity olaraktan Model Diagram içerisinde yer almasının avantajı olacaktır. Nitekim ileride eklenecen kolonlar için Entity Model tarafında sadece Conceptual Schema'yı güncellemek yeterlidir.

Dolayısıyla bu yazımızdaki amacımız, varsayılan olarak Entity Model Diagram üretilirken ortadan kaldırılan ara bağlantı tablosunun manuel olarak oluşturulmasını sağlama olacaktır. Başlangıçta Visual Studio 2010 Ultimate RC sürümü üzerinde oluşturduğumuz Console uygulaması içerisinde yer alan Entity Model diagramının aşağıdaki gibi olduğunu varsayıyoruz.

![blg141_FirstState.gif](images/blg141_FirstState.gif)

Tahmin edeceğiniz üzere Chinook veritabanı içerisinde yer alan Playlist-Track ilişkilerini değerlendirmekteyiz. Şekil üzerinde yer alan çarpı işareti mutlaka dikkatinizi çekmiştir. Aslında bu yapacağımız ilk hamle olacak. Aradaki Association nesnesini silmek. Bu durumda çok doğal olarak Playlist ve Track tablolarında yer alan ve birbirlerine olan geçişleri sağlayan Navigation Property'lerin de ortadan kaldırıldığını göreceğiz.

![blg141_DeleteRelation.gif](images/blg141_DeleteRelation.gif)

Ancak burada dikkat etmemiz gereken bir nokta bulunmaktadır. Silme işlemi sırasında Designer'ın aşağıdaki sorusu ile karşılaşacağız. PlaylistTrack tablosu her ne kadar Entity olarak ifade edilmese de, oluşturacağımız Entity'nin ileride veritabanında karşılık geldiği tablo ile ilişkilendirilmesi sırasında ele alınacaktır. Bu nedenle No seçimi yaparaktan silme işlemini icra etmemiz daha doğrudur.

![blg141_Delation2.gif](images/blg141_Delation2.gif)

Sıradaki adımımız ise veritabanında yer alan ve Playlist ile Track tabloları arasındaki Many-To-Many ilişkiyi gerçekleştiren tabloya karşılık gelen bir Entity tipinin oluşturulmasıdır. Yani veritabanında yer alan PlaylistTrack isimli tablonun karşılığı olan Entity tipinin üretilmesi. Bunun için Model diagramı üzerinden Add->Entity seçimini yapmamız yeterli olacaktır. Sonrasında ise Entity özelliklerini aşağıdaki gibi ayarlamamız gerekecektir.

![blg141_AddEntity.gif](images/blg141_AddEntity.gif)

Burada dikkat edilmesi gereken noktalardan birisi, Key Property özelliğinin kullanılmayışıdır. Nitekim bu alana senaryomuza göre gerek yoktur. Ara tablonun karşılığı olan PlaylistTrack Entity tipinin görevi, Playlist ve Track Entity tipleri arasındaki Many-To-Many ilişkiyi sağlamak olduğundan, bu Entity'lere ait Key özelliklerini barındırması yeterlidir. Buna göre yeni oluşturulan Entity içerisinde aşağıdaki şekilde görülen Scalar Property'lerin eklenmesi gerekir.

![blg141_AddColumnLast.gif](images/blg141_AddColumnLast.gif)

Yine dikkat edilmesi gereken önemli bir nokta vardır. PlaylistTrack Entity tipi içerisinde yer alan PlaylistId ve TrackId isimli alanlarının Entity Key özelliklerine true değer atanmıştır. Buna göre bu alanların Playlist ve Track tablosundaki karşılıkları ile ilişki kurabilmesi mümkün olacaktır. Dolayısıyla sıradaki adımımız tablolar arasındaki ilişkileri kurmaktır.

> Aslında PlaylistId ve TrackId alanlarının oluşturulması şart değildir. PlaylistTrack tablosuna ait bilgileri silmediğimizden Association'ları oluşturduğumuz sırada söz konusu alanların otomatik olarak üretileceğini görebiliriz. Bu tekniğide tercih edebilirsiniz. Şu andaki ilerleyişimize göre Association'ların oluşturulması sırasında Add foreign key properties to the... seçeneğini kaldırmamız gerekmektedir. Ancak kolonların oluşturulmasını otomatikleştirmek istiyorsak, bu seçeneği işaretli bırakmamız gerekmektedir. Böyle yaptığımız takdirde kolon adlarının pekte istediğimiz gibi olmayacağını, yeniden isimlendirmemiz gerektiğinide belirtmek isterim. Tercih size kalmış.
>
> ![Wink](images/smiley-wink.gif)

Şimdi bir düşünelim. Bir Playlist'e bağlı birden fazla Track olabilir. Benzer şekilde bir Track birden fazla Playlist içerisinde yer alabilir. Buna göre Playlist ve Track varlıkları arasında Many-To-Many ilişki söz konusudur. Bunu zaten biliyoruz

![Smile](images/smiley-smile.gif)

Ancak bunun Entity Model diagram tarafında, şu andaki yapıya göre ifadesi nasıl olacaktır? Cevap; Playlist Entity'sinden PlaylistTrack Entity'sine ve benzer şekilde Track Entity'sinden yine PlaylistTrack Entity'sine doğru One-To-Many ilişki sağlayarak. Bunun için diagrama bir Association nesnesi eklenmesi yeterlidir (Add->Association).

Playlist -> PlaylistTrack

![blg141_1.gif](images/blg141_1.gif)

Dikkat edileceği üzere End özelliklerinden sol tarafta yer alanda Playlist bulunmaktadır. Playlist Entity tipi için Multiplicity özelliği 1 (One) iken, sağ tarafta yer alan PlaylistTrack için (Many) dir. Buna göre Playlist Entity tipinden PlaylistTrack Entity tipine doğru One-To-Many ilişki kurulmuştur. Üretilen Navigation Property'lerden PlaylistTrack olarak adlandırılanlar, çoğulluk nedeni ile PlaylistTracks olarak yeniden isimlendirilmiştir. Bu işlemin sonrasında diagramın yeni hali aşağıdaki gibi olacaktır.

![blg141_2.gif](images/blg141_2.gif)

Track -> PlaylistTrack

![blg141_3.gif](images/blg141_3.gif)

Bu kez Track Entity tipi üzerinden az önceki gibi PlaylistTrack Entity tipine doğru One-To-Many ilişki kurulmuştur. Sonuç olarak diagramın yeni hali aşağıdaki gibi olacaktır.

![blg141_4.gif](images/blg141_4.gif)

Sıradaki adımda oluşturulan yeni PlaylistTrack Entitiy tipinin, veritabanında yer alan Tablo karşılığı ile eşleştirilmesi gerekir. Bu sebepten PlaylistTrack Entity tipinin Mapping Details özelliklerinden aşağıdaki gibi gerekli eşleştirmenin yapılması şarttır. Hatırlayacağınız üzere ilk adımda Association nesnesini silerken tablonun kaldırılmamasını belirtmiştik. Bu sayede Table eşleştirmesini kolayca gerçekleştirdik.

![blg141_5.gif](images/blg141_5.gif)

Ancak işlemlerimiz bir türlü bitmek bilmemektedir. Sabredin, çok az kaldı. Takip eden adımda Referentail Constraint'lerin tanımlanması gerekmektedir. Normal şartlarda eğer Association'lar oluşturulurken Add foreign key properties to the... işaretli olsaydı bunlarda ilgili alanlar ile birlikte otomatik oluşturulacaklardı. Bu nedenle izleyen şekillerde görüldüğü üzere ilgili eklemelerin yapılması gerekmektedir.

![blg141_6.gif](images/blg141_6.gif)

ve

![blg141_7.gif](images/blg141_7.gif)

Buraya kadar yaptıklarımızı özetleyecek olursak aşağıdaki adımları gerçekleştirdiğimizi ifade edebiliriz.

1- Entity'ler arasındaki Many-To-Many tipinden Relation silinir.

2- Veritabanında yer alan ama Entity Model tarafına aktarılmayan ara tablonun karşılığı olan tipin üretilmesi sağlanır.

3- Eklenen Entity tablosuna Entity Key özellikleri true olan ve diğer Entity'ler üzerinde karşılık düşen alanlar ile aynı tipten olan (Örneğimizde hepsi Int32) Scalar Property'ler ilave edilir.

4- Entity tipleri arasındaki One-To-Many ilişkiler (Association) tesis edilir.

5- Eklenen Entity nesnesi, veritabanındaki tablo ile ilişkilendirilir (Mapping).

6- Association'lar ile ilişkili olaraktan gerekli Referencial Constraints'ler tanınmlanır.

Artık konuyu basit bir örnek sonlandırabiliriz. Bu amaçla aşağıdaki program kodunu yazdığımızı düşünelim.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RelationEntityCreation
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ChinookEntities entities = new ChinookEntities())
            {
                var result = (from p in entities.Playlists.Include("PlaylistTracks.Track")
                              where p.PlaylistId == 3
                              select p).First();

                foreach (var r in result.PlaylistTracks)
                {
                    Console.WriteLine(r.Track.Name);
                }
            }
        }
    }
}
```

Örnek kod parçasında PlaylistId değeri 3 olan satıra bağlı olan Track'lerin çekilmesi sağlanmaktadır. Include metodu içerisinde belirtilen tanımlama önemlidir. Burada PlaylistTracks üzerinden Track Entity'sine ulaşılmaktadır. Bu sayede Playlist'e bağlı Track'lerinde yüklenmesi sağlanmış olur. Uygulama kodunun çalışmasının sonucu aşağıdaki gibidir.

![blg141_8.gif](images/blg141_8.gif)

Tabi bu kodun çalışması sonrasında SQL tarafında oldukça yoğun bir sorgunun üretilmesi söz konusudur.

```text
SELECT 
[Project1].[PlaylistId] AS [PlaylistId], 
[Project1].[Name] AS [Name], 
[Project1].[C1] AS [C1], 
[Project1].[PlaylistId1] AS [PlaylistId1], 
[Project1].[TrackId] AS [TrackId], 
[Project1].[TrackId1] AS [TrackId1], 
[Project1].[Name1] AS [Name1], 
[Project1].[AlbumId] AS [AlbumId], 
[Project1].[MediaTypeId] AS [MediaTypeId], 
[Project1].[GenreId] AS [GenreId], 
[Project1].[Composer] AS [Composer], 
[Project1].[Milliseconds] AS [Milliseconds], 
[Project1].[Bytes] AS [Bytes], 
[Project1].[UnitPrice] AS [UnitPrice]
FROM ( SELECT 
 [Limit1].[PlaylistId] AS [PlaylistId], 
 [Limit1].[Name] AS [Name], 
 [Join1].[PlaylistId] AS [PlaylistId1], 
 [Join1].[TrackId1] AS [TrackId], 
 [Join1].[TrackId2] AS [TrackId1], 
 [Join1].[Name] AS [Name1], 
 [Join1].[AlbumId] AS [AlbumId], 
 [Join1].[MediaTypeId] AS [MediaTypeId], 
 [Join1].[GenreId] AS [GenreId], 
 [Join1].[Composer] AS [Composer], 
 [Join1].[Milliseconds] AS [Milliseconds], 
 [Join1].[Bytes] AS [Bytes], 
 [Join1].[UnitPrice] AS [UnitPrice], 
 CASE WHEN ([Join1].[PlaylistId] IS NULL) THEN CAST(NULL AS int) ELSE 1 END AS [C1]
 FROM   (SELECT TOP (1) [Extent1].[PlaylistId] AS [PlaylistId], [Extent1].[Name] AS [Name]
  FROM [dbo].[Playlist] AS [Extent1]
  WHERE 3 = [Extent1].[PlaylistId] ) AS [Limit1]
 LEFT OUTER JOIN  (SELECT [Extent2].[PlaylistId] AS [PlaylistId], [Extent2].[TrackId] AS [TrackId1], [Extent3].[TrackId] AS [TrackId2], [Extent3].[Name] AS [Name], [Extent3].[AlbumId] AS [AlbumId], [Extent3].[MediaTypeId] AS [MediaTypeId], [Extent3].[GenreId] AS [GenreId], [Extent3].[Composer] AS [Composer], [Extent3].[Milliseconds] AS [Milliseconds], [Extent3].[Bytes] AS [Bytes], [Extent3].[UnitPrice] AS [UnitPrice]
  FROM  [dbo].[PlaylistTrack] AS [Extent2]
  INNER JOIN [dbo].[Track] AS [Extent3] ON [Extent2].[TrackId] = [Extent3].[TrackId] ) AS [Join1] ON [Limit1].[PlaylistId] = [Join1].[PlaylistId]
)  AS [Project1]
ORDER BY [Project1].[PlaylistId] ASC, [Project1].[C1] ASC
```

Buuuwvvvvv!!!

![Sealed](images/smiley-sealed.gif)

Açıkçası ben bu tip bir tablonun eğer maliyet kaybı yoksa ısrarla eklenmesinden yana değilim. Yani Ado.Net Entity Framework gibi düşünüyorum. Fakat yazımızın başında da belirttiğimiz üzere ara tablonun, One-To-Many ilişkiler için gerekli olanlar dışında ek alanlar da içerebileceği durumlar söz konusu olabilir. Bu alanların zaman içerisinde tabloya eklenmesi ve Entity tarafına da taşınması gerektiği hallerde, Entity Model üzerinde söz konusu ara tablonun karşılığının olması, sadece Conceptual Model üzerinde müdahaleler yaparak işi kurtarmamızı sağlayabilir. Teorik olarak. Yine de anlattıklarımızın RC sürümü üzerinde yazıldığını belirtmek isterim. Yani ilerleyen sürümde farklılıklar söz konusu olabilir.

![Wink](images/smiley-wink.gif)

Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[RelationEntityCreation_RC.rar (52,15 kb)](assets/RelationEntityCreation_RC.rar) [Örnek Visual Studio 2010 Ultimate RC sürümü üzerinde geliştirilmiş ve test edilmiştir]
