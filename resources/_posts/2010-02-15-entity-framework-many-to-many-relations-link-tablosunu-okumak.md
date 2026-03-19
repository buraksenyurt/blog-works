---
layout: post
title: "Entity Framework - Many To Many Relations - Link Tablosunu Okumak"
date: 2010-02-15 22:30:00 +0300
description: "Hatırlayacağınız üzere bir önceki yazımızda veritabanı tarafındaki Many-To-Many Relation' ların, Entity Framework tarafında nasıl değerlendirilebileceğine değinmeye çalışmıştık. Dikkat edilmesi gereken önemli noktalardan birisi, veritabanı tarafında yer alan ara bağlantı tablosunun Entity Framework üzerindeki modele alınmayışıydı."
categories:
  - entity-framework
tags:
  - entity-framework
  - csharp
  - linq
---
# Entity Framework - Many To Many Relations - Link Tablosunu Okumak
Merhaba Arkadaşlar,

Hatırlayacağınız üzere bir önceki yazımızda veritabanı tarafındaki Many-To-Many Relation'ların, Entity Framework tarafında nasıl değerlendirilebileceğine değinmeye çalışmıştık. Dikkat edilmesi gereken önemli noktalardan birisi, veritabanı tarafında yer alan ara bağlantı tablosunun Entity Framework üzerindeki modele alınmayışıydı. Ancak tam bu noktada ortaya bir soru işareti çıkmakta. Ya çalışma zamanında sadece ilişkili primary key alanlarının değerlerini elde etmek istersek. Bir önceki senaryomuza göre bu, hangi Track satırının hangi Playlist'ler ile ve hangi Playlist satırının hangi Track'ler ile ilişkili olduğunun görülmesi anlamına gelmektedir. Kısacası sadece TrackId ve PlaylistId değerlerinin eşleşmesine bakmak istediğimizi düşünebiliriz. Bu durumda aşağıdaki gibi bir kod parçası işimizi görecektir. Şükürler olsun isimsiz tiplere (Anonymous Type)

![Wink](/assets/images/2010/smiley-wink.gif)

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
                #region Only Related Columns

                var relations = from t in entities.Tracks
                                from p in entities.Playlists
                                where t.Name.StartsWith("Ab")
                                select new
                                {
                                    t.TrackId,p.PlaylistId
                                };

                foreach (var relation in relations)
                {
                    Console.WriteLine(relation.ToString());
                }

                #endregion
            }
        }
    }
}
```

Bu LINQ sorgusunda çoklu Select işlemi yapılmaktadır. Sorguya göre Track.Name değeri Ab ile başlayan parçaların TrackId değeleri ile Playlist Entity nesnesi içeriğinde yer alan PlaylistId değerleri arasındaki ilişkiler, yeni bir isimsiz tip (Anonymous Type) içerisine dahil edilmektedir. Kodun çalışması sonucu aşağıdakine benzer bir çalışma zamanı çıktısı elde edilecektir.

![blg132_Runtime.gif](/assets/images/2010/blg132_Runtime.gif)

Arka planda çalışan SQL Sorgusu ise aşağıdaki gibidir.

```text
SELECT 
[Extent1].[TrackId] AS [TrackId], 
[Extent2].[PlaylistId] AS [PlaylistId]
FROM  [dbo].[Track] AS [Extent1]
CROSS JOIN [dbo].[Playlist] AS [Extent2]
WHERE [Extent1].[Name] LIKE N'Ab%'
```

Görüldüğü üzere Anonymous Type kullanımı nedeniyle sadece istediğimiz TrackId ve PlaylistId alanları Select sorgusuna dahil edilmiştir. Buda arka planda çalışan SQL sorgusunda, tüm tablo alanlarının hesaba katılmaması anlamına gelmektedir ki bizim için bir avantajdır ve istediğimizde zaten budur

![Wink](/assets/images/2010/smiley-wink.gif)

Bir nevi veritabanında yer alan ama Entity olarak kod tarafına aktarılmayan ara tablo içeriğini, LINQ tarafında elde etmiş olduk. Entity Framework tarafında ipucu tadındaki başka konularlar devam ediyor olacağız. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.