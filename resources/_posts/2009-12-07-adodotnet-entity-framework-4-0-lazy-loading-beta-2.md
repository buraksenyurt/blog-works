---
layout: post
title: "Ado.Net Entity Framework 4.0 - Lazy Loading [Beta 2]"
date: 2009-12-07 02:27:00 +0300
categories:
  - entity-framework
tags:
  - entity-framework
  - csharp
  - dotnet
  - ado-net
  - linq
  - sql-server
  - visual-studio
---
Ado.Net Entity Framework'ün en çok eleştirilen yönlerinden birisi, ORM (Object Relational Mapping) ihtiyacını karşılayacak bir alt yapı olarak tasarlanmasına rağmen ORM'in karakteristik özelliklerinden birisi olan Lazy Loading'in (Bir nesnenin ihtiyaç duyulduğu noktada veri kaynağından yüklenmesini-Load hedefleyen bir tasarım kalıbı olarak düşünülebilir) tam manada desteklemiyor olmasıdır. Zaman içerisinde Deferred Loading veya Explicity Lazy Loading gibi isimler ile Ado.Net Entity Framework içerisine bir takım özellikler eklenerek bu eksiklik ortadan kaldırılmaya çalışılsada gerçek manada 4.0 versiyonunun Beta 2 sürümünde, beklenildiği gibi bir iyileştirme olduğunu görmekteyiz.

Bu yazımızdaki temel amacımız ise, Ado.Net Entity Framework'ün bir önceki sürümünde durumun ne olduğunu anlamaya çalışıp, 4.0 Beta 2 sürümünde Lazy Loading adına hangi özelliğin getirildiğini (belkide getirilmediğini) ve nasıl kullanıldığını görebilmektir. Örneklerimizde Codeplex üzerinden açık kaynak olarak yayınlanan Chinook veritabanını kullanıyor olacağız. Haydi parmakları sıvayalım

![Wink](/assets/images/2009/smiley-wink.gif)

Örneklerimizden ilkini Visual Studio 2008 diğerini ise Visual Studio 2010 Ultimate Beta 2 ortamında geliştiriyor olacağız. Ancak her iki örnekte aşağıdaki şekilde görülen tabloları kullanıyor olacak.

![blg112_Model.gif](/assets/images/2009/blg112_Model.gif)

Visual Studio 2008 ve Ado.Net Entity Framework V1.0

İlk olarak Console uygulamamızda aşağıdaki gibi bir kod parçası geliştirdiğimizi düşüelim.

```csharp
using System;
using System.Linq;

namespace Before
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ChinookEntities context = new ChinookEntities())
            {
                var albums = from a in context.Album
                             select a;

                foreach (Album albm in albums)
                    Console.WriteLine("{0} [{1}]",albm.Title,albm.Track.Count.ToString());

            }
        }
    }
}
```

Bu kod parçasında Album listesi elde edildikten sonra Title bilgileri ile birlikte, o anki albüme ait Track sayıları ekrana yazdırılmaktadır. Yani albümler ve bu albümler altında kaç şarkı olduğu bilgisine ulaşmak istediğimizi düşünebilir. Buna göre çalışma zamanı görüntüsü aşağıdaki gibidir.

![blg112_Runtime1.gif](/assets/images/2009/blg112_Runtime1.gif)

Hatta SQL Server Profiler aracına bakıldığında söz konusu kod parçası için aşağıdaki sorgunun çalıştırıldığı görülür.

```text
SELECT 
[Extent1].[AlbumId] AS [AlbumId], 
[Extent1].[Title] AS [Title], 
[Extent1].[ArtistId] AS [ArtistId]
FROM [dbo].[Album] AS [Extent1]
```

Bir sorun var mı?

![Undecided](/assets/images/2009/smiley-undecided.gif)

Aslında var. Dikkat edileceği üzere güncel Album Entity nesnesi ile ilişkili olan Track nesneslerinin toplam sayıları (Count) her zaman 0 olarak elde edilmiştir. Bir başka deyişle X Entity nesnesi ile ilişkili olan Y Entity nesnesine dair herhangibir sorgunun işletilmesi söz konusu olmamıştır. Oysaki Lazy Loading kalıbına göre çalışma zamanındaki Album nesne örneklerine ait Track özellikleri üzerinden o anki Track nesne örneğinin herhangibir üyesine erişilmek istendiğinde (Örnekteki Count gibi), SQL tarafındada gerekli sorguların çalıştırılacağı düşünülür. Peki ilk versiyonda yaşanan bu durum üzerine ne yapılmaktaydı? Temel olarak iki basit yöntem ile bu durumu çözüme kavuşturabiliriz. Bunlardan birisi Include metodunun LINQ ifadesinde aşağıdaki gibi kullanılmasıdır.

```csharp
var albums = from a in context.Album.Include("Track")
                             select a;
```

Bu durumda çalışma zamanı görüntüsü aşağıdaki gibi olacaktır.

![blg112_Runtime2.gif](/assets/images/2009/blg112_Runtime2.gif)

Görüldüğü üzere albümler içerisinde yer alan parçaların toplam sayıları elde edilebilmiştir. Bunun için SQL tarafında da aşağıdaki sorgunun çalıştırıldığı gözlemlenmektedir.

```text
SELECT 
[Project1].[AlbumId] AS [AlbumId], 
[Project1].[Title] AS [Title], 
[Project1].[ArtistId] AS [ArtistId], 
[Project1].[C1] AS [C1], 
[Project1].[C3] AS [C2], 
[Project1].[C2] AS [C3], 
[Project1].[TrackId] AS [TrackId], 
[Project1].[Name] AS [Name], 
[Project1].[MediaTypeId] AS [MediaTypeId], 
[Project1].[GenreId] AS [GenreId], 
[Project1].[Composer] AS [Composer], 
[Project1].[Milliseconds] AS [Milliseconds], 
[Project1].[Bytes] AS [Bytes], 
[Project1].[UnitPrice] AS [UnitPrice], 
[Project1].[AlbumId1] AS [AlbumId1]
FROM ( SELECT 
 [Extent1].[AlbumId] AS [AlbumId], 
 [Extent1].[Title] AS [Title], 
 [Extent1].[ArtistId] AS [ArtistId], 
 1 AS [C1], 
 [Extent2].[TrackId] AS [TrackId], 
 [Extent2].[Name] AS [Name], 
 [Extent2].[AlbumId] AS [AlbumId1], 
 [Extent2].[MediaTypeId] AS [MediaTypeId], 
 [Extent2].[GenreId] AS [GenreId], 
 [Extent2].[Composer] AS [Composer], 
 [Extent2].[Milliseconds] AS [Milliseconds], 
 [Extent2].[Bytes] AS [Bytes], 
 [Extent2].[UnitPrice] AS [UnitPrice], 
 CASE WHEN ([Extent2].[TrackId] IS NULL) THEN CAST(NULL AS int) ELSE 1 END AS [C2], 
 CASE WHEN ([Extent2].[TrackId] IS NULL) THEN CAST(NULL AS int) ELSE 1 END AS [C3]
 FROM  [dbo].[Album] AS [Extent1]
 LEFT OUTER JOIN [dbo].[Track] AS [Extent2] ON [Extent1].[AlbumId] = [Extent2].[AlbumId]
)  AS [Project1]
ORDER BY [Project1].[AlbumId] ASC, [Project1].[C3] ASC
```

Dikkat edileceği üzere Album ve Track tablolarının Left Outer Join ile birleştirilmesi söz konusudur. Dahası, sonucun elde edilmesi için tek bir SQL ifadesinin çalıştırılması yeterli olmuştur. Ancak dikkat çeken noktalardan biriside sadece Count değeri ile ilgilenmemize rağmen Track tablosundaki tüm alanların ifadeye dahil edilmesidir. Zaten Count hesabı için SQL tarafında çalıştırılmış bir ifade de bulunmamaktadır. Bunun yerine kod tarafına çekilen Track listesinin toplam değerinin hesaplanması söz konusudur.

Diğer bir teknik ise Load metodunun kullanılmasıdır. Bu amaçla foreach döngüsünde aşağıdaki düzenlemeyi yaptığımızı düşünelim.

```csharp
foreach (Album albm in albums)
{
   albm.Track.Load();
   Console.WriteLine("{0} [{1}]", albm.Title, albm.Track.Count.ToString());
}
```

Bu durumda çalışma zamanı görüntüsü Include kullanımındaki ile aynı olacaktır.

![blg112_Runtime3.gif](/assets/images/2009/blg112_Runtime3.gif)

SQL Server Profiler aracına bakıldığında ise aşağıdakine benzer bir ekran görüntüsü ile karşılaşılacaktır.

![blg112_Profiler1.gif](/assets/images/2009/blg112_Profiler1.gif)

Dikkat edileceği üzere foreach döngüsü içerisinde Load metodunun çağırıldığı her an arka planda bir SQL sorgusunun çalıştırıldığı, yine sadece Count ile ilgilenmemize rağmen tüm Track alanlarının işe katıldığı gözlemlenebilir. Hatta Count ile ilişkili olarak SQL tarafında çalıştırılan bir ifade bulunmamaktadır. Bu değer, kod tarafındaki Track listesinin ilgili Album için elde edilmesinden sonra hesaplanmaktadır.

Aslında tam bu noktada, ele alınan senaryo için Include metodunun kullanımının, Load metoduna göre daha efektif olduğunu düşünebiliriz. Nitekim istemci ve SQL sunucusu arasında sadece tek bir sorgusunun çalıştırılması söz konusudur. Diğer yandan foreach döngüsü içerisinde senaryoya göre bazı koşullar sağlandığı takdirde Count özelliğinin kullanılması istendiği durumlarda, Load metodunun tercih edilmesi yoluna gidilebilir. Her neyse...Bakalım Ado.Net Entity Framework 4.0 Beta 2 içerisinde gerçek anlamda Lazy Loading için ne yapılmıştır.

Visual Studio 2010 Beta 2 ve Ado.Net Entity Framework 4.0 Beta 2

Yeni sürümde ObjectContext türevli tip üzerinden uygulanabilen LazyLoadingEnabled isimli bir özellik (Property) bulunmaktadır. Aslında varsayılan olarak Lazy Laoding özelliğinin açık olduğunu söyleyebiliriz. Aynı örneği Visual Studio 2010 Beta 2 üzerinde denediğimizi düşünelim.(Entity adlarının oluşturulması sırasında çoğullaştırma özelliği etkinleştirilmiştir. Bu nedenle Albums ve Tracks isimlendirmeleri söz konusudur)

```csharp
using System;
using System.Linq;

namespace ReallyLazy
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ChinookEntities context = new ChinookEntities())
            {
                var albumsWithTracks = from a in context.Albums
                                       select a;

                foreach (Album albm in albumsWithTracks)
                {
                    Console.WriteLine("{0} ({1})",albm.Title,albm.Tracks.Count.ToString());
                }
            }
        }
    }
}
```

Çalışma zamanında aşağıdaki sonuçları elde ederiz.

![blg112_Runtime4.gif](/assets/images/2009/blg112_Runtime4.gif)

Dikkat edileceği üzere kod içerisinde herhangibir şey belirtmememize rağmen Album nesneleri ile ilişkili olan Track nesnelerinin Count özelliklerinin değerleri elde edilebilmiştir. Peki arka planda çalışan SQL sorgusu (sorguları)?

![Wink](/assets/images/2009/smiley-wink.gif)

İşte SQL Server Profiler aracından elde edilen sonuçlar;

![blg112_Profiler2.gif](/assets/images/2009/blg112_Profiler2.gif)

Görüldüğü gibi foreach döngüsü içerisinde Track.Count özelliğine gidildiği her noktada bir SQL sorgusu çalıştırılmış ve o anki albüme bağlı olan parça bilgileri çekilmiştir. Dikkat edileceği üzere Count için SQL tarafında yapılmış özel bir sorgulama yoktur. Bu değer kod tarafında elde edilmektedir. Aslında bu çalışma şeklinin Load metodunun kullanımındaki ile benzer olduğunu söyleyebiliriz. Benzer diyorum çünkü Load kullanımında çalıştırılan SQL sorgusu ile buradaki arasında fark vardır. (Bakalım iki şekil arasındaki 9 farkı bulabilecek misiniz? ![Smile](/assets/images/2009/smiley-smile.gif))

> Kişisel Not: Aslında bazı blog yazılarında LazyLoadingEnabled özelliğinin true olması halinde bu şekilde çalıştığı gösterilmiştir. İşte sürüm farklılıklarının bir sonucu daha. Bu nedenle Beta 2 üzerinde yazdığımız bu konunun gelecek sürümlerde değişikliğe uğraması söz konusu olabilir.

Şimdi kodumuzda aşağıdaki değişikliği yaptığımızı düşünelim.

```csharp
using (ChinookEntities context = new ChinookEntities())
{
   context.ContextOptions.LazyLoadingEnabled = false;
```

Burada LazyLoadingEnabled özelliğine false değer atanması sonucu aşağıdaki sonuçlar ile karşılaşılacaktır.

![blg112_Runtime5.gif](/assets/images/2009/blg112_Runtime5.gif)

Beklediğimiz gibi Lazy Loading özelliğini kapattık. Peki ya SQL tarafı? İşte SQL Server Profiler'dan yakalanan SQL sorgusu.

```text
SELECT 
[Extent1].[AlbumId] AS [AlbumId], 
[Extent1].[Title] AS [Title], 
[Extent1].[ArtistId] AS [ArtistId]
FROM [dbo].[Album] AS [Extent1]
```

Sanırım yazımızın başladığı noktadaki sonuçlara döndük ne dersiniz?

![Wink](/assets/images/2009/smiley-wink.gif)

Özetle Ado.Net Entity Framework 4.0 Beta 2 sürümünde Lazy Loading kabiliyetinin varsayılan olarak açık geldiğini ve istendiğinde Context nesnesinin LazyLoadingEnabled özelliğine atanacak false değeri ile kapatılabileceğini görmüş olduk. Bakalım sürümün ileriki versiyonlarında ne gibi yenilikler olacak. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Before.rar (41,07 kb)](/assets/files/2009/Before.rar)

[ReallyLazy.rar (98,20 kb)](/assets/files/2009/ReallyLazy.rar)