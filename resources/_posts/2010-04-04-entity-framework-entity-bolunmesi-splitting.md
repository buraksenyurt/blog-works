---
layout: post
title: "Entity Framework - Entity Bölünmesi (Splitting)"
date: 2010-04-04 20:15:00 +0300
categories:
  - entity-framework
tags:
  - entity-framework
  - csharp
  - dotnet
  - ado-net
  - linq
  - performance
  - generics
  - visual-studio
  - rc
---
Hız kesmeden Ado.Net Entity Framework ile ilişkili araştırmalarımızı paylaşmaya devam ediyoruz. Bu günkü yazımızda bir Entity tipinin, veritabanı tarafında birden fazla Tablo'yu işaret edebilmesi kabiliyetini incelemeye çalışacağız. Söz konusu duruma çok sık rastlanılmasa dahi, zaman zaman gereksinim duyulmaktadır. Konuyu daha kolay bir şekilde kavrayabilmek adına, yazıyı yazdığım tarih itibariyle sistemimde kurulu olan Adventure Works 2008 veritabanında yer alan ve aşağıdaki diagramda görülen tablolarımızın olduğunu varsayalım (Tabiki kendi sisteminizde yer alan örnek veritabanında benzer ilişikilere sahip başka tabloları da aynı teori içerisinde değerlendirebilirsiniz)

![blg142_SqlDiagram.gif](/assets/images/2010/blg142_SqlDiagram.gif)

Person, Password ve BusinessEntity tabloları arasındaki ilişkileri değerlendirdiğimizde, 1 Person'a ait 1 Password satırı ve yine 1 Person'a ait 1 BusinessEntity satırı olabileceğini görmekteyiz. Aslında bu tabloların tamamı tek bir Person verisini ifade etmekte. Tabi veritabanı tarafında kavramsal olarak bu veri birden fazla tabloya ayrıştırılmış durumda. Çok doğal olarak kod tarafına geçtiğimizde bu tabloların her biri için ayrı birer Entity tipi üretileceğine şahit olacağız. Aynen aşağıdaki Entity Model Diagram'da görüldüğü gibi.

![bkg142_EntityDiagram.gif](/assets/images/2010/bkg142_EntityDiagram.gif)

Tabi kod tarafında Person içeriğini değerlendirirken Password veya BusinessEntity içeriklerine de kolay bir şekilde erişebilmek gibi bir amacımız varsa eğer, 3 farklı Entity yerine 1 Entity kullanmak daha avantajlı olabilir. Özellikle kodlama zamanında yazılan LINQ sorgularında veya CRUD işlemlerinde. Peki bu 3 Entity'nin tek bir Entity olarak ele alınması için ne yapmak gerekmektedir? Son derece basit. Cut-Paste özelliklerinden faydalanılmalıdır

![Wink](/assets/images/2010/smiley-wink.gif)

Bu anlamda, BusinessEntity ve Password Entity içeriklerine baktığımızda bizim için gerekli olan alanları kesip Person Entity tipi içerisine yapıştırmamız yeterlidir. Ne yazık ki bu örnekte yer alan BusinessEntity tablosunun içeriğinde çok faydalı bilgiler bulunmamaktadır. Ama yinede konunun kavranabilmesi açısından değerlendirilmiştir. Bu işlemleri gerçekleştirirken dikkat edilmesi gereken noktalaradan biriside, aynı isimli Property'lerin kopyalanması sonrasında, son ek olarak 1 gibi sayısal bir isimlendirmenin söz konusu olmasıdır. Tabiki bu tip özellikleri tekrardan isimlendirmekte yarar vardır. Gerekli kesme ve yapıştırma işlemleri tamamlandıktan sonra Password ve BusinessEntity tipleri diagramdan silinebilir. Yine bu işlem sırasında dikkat edilmesi gereken bir noktada bize sorulan soruya No cevabını vermektir.(Sorunun ne olduğunu söylemeyeceğim, örneği yaparken görmenizi istiyorum. ![Wink](/assets/images/2010/smiley-wink.gif)) Sonuç olarak diagramın yeni hali aşağıdaki gibi olacaktır.

![blg142_NewType.gif](/assets/images/2010/blg142_NewType.gif)

Yeterli mi? Elbette değil. Designer'da yapmış olduğumuz taşıma işlemlerine rağmen, Mapping Details içeriğinde halen daha eksiklikler bulunmaktadır. Aşağıdaki şekilde bu durum net bir şekilde görülebilir.

![blg142_FirstCase.gif](/assets/images/2010/blg142_FirstCase.gif)

Eksik olan kısım şudur; kopyalanan özelliklerin hangi tablolardaki hangi alanları işaret ettiği belli değildir. Dolayısıyla Mapping Details kısmında gerekli düzenlemeleri yaparak aşağıdaki şekilde görülen eşleştirmeleri tamamlamamız gerekir.

![blg142_SecondCase.gif](/assets/images/2010/blg142_SecondCase.gif)

Buna göre Person Entity tipinin Person tablosu dışında BusinessEntity ve Password tablolarını da işaret ettiği belirtilmiş olur. Gerekli Field-Property eşleştirmeleri yapıldıktan sonra (örneğin rowguid'in BusinessEntityRowGuid ile eşleşmesi gibi) uygulama derlenip örnek bir kodun denenmesi için gerekli işlemlere başlanabilir. Bu amaçla Main metodu içerisinde aşağıdaki test kodunu yazdığımızı düşünelim.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitting
{
    class Program
    {
        static void Main(string[] args)
        {
            using (AdventureWorks2008Entities entities = new AdventureWorks2008Entities())
            {
                var result = (from p in entities.People
                             select new
                             {
                                 p.PersonType,
                                 p.FirstName,
                                 p.MiddleName,
                                 p.LastName,
                                 p.PasswordHash,
                                 p.PasswordSalt,
                                 p.BusinessEntity_rowguid,
                                 p.Password_ModifiedDate
                             }).Take(3);

                foreach (var r in result)
                {
                    Console.WriteLine(r.ToString()+"\n");
                }
            }
        }
    }
}
```

Örneğimizde ilk 3 satıra ait alanlardan bazılarının çekildiği isimsiz tip (Anonymous Type) kullanımı söz konusudur. Dikkat edilmesi gereken nokta ise tek bir Entity nesnesi üzerinden, Person, Password ve BusinessEntity tablolarının ilgili alanlarına ulaşılabiliyor olmasıdır ki bu Splitting özelliğinin bir sonucudur. Buna göre örneğimizin çalışma zamanı çıktısı aşağıadaki gibi olacaktır.

![blg142_Runtime.gif](/assets/images/2010/blg142_Runtime.gif)

Bu kabiliyetin uygulanışı sırasında en çok dikkat edilmesi gereken nokta arka planda çalıştırılan SQL sorgularıdır. Nitekim birden fazla Tablonun tek bir Entity içerisinde birleştirilmesi arka planda Inner Join sorgularının atılmasına neden olacaktır. Yukarıdaki örneğin çalışma zamanı sırasında SQL tarafında icra edilen sorgudan bu durum net bir şekilde görülebilir.

```text
SELECT TOP (3) 
[Extent1].[BusinessEntityID] AS [BusinessEntityID], 
[Extent3].[PersonType] AS [PersonType], 
[Extent3].[FirstName] AS [FirstName], 
[Extent3].[MiddleName] AS [MiddleName], 
[Extent3].[LastName] AS [LastName], 
[Extent2].[PasswordHash] AS [PasswordHash], 
[Extent2].[PasswordSalt] AS [PasswordSalt], 
[Extent1].[rowguid] AS [rowguid], 
[Extent2].[ModifiedDate] AS [ModifiedDate]
FROM   [Person].[BusinessEntity] AS [Extent1]
INNER JOIN [Person].[Password] AS [Extent2] ON [Extent1].[BusinessEntityID] = [Extent2].[BusinessEntityID]
INNER JOIN [Person].[Person] AS [Extent3] ON [Extent1].[BusinessEntityID] = [Extent3].[BusinessEntityID]
```

Dolayısıyla bu çalışma şekli dikkate alınarak gerçekten gereksinim var ise Entity birleştirmesi yolu tercih edilmelidir. Öyleki Entity tiplerinin ayrık olarak kalmaları tercih edilebilir. Söz gelimi geliştirdiğimiz örnekte BusinessEntity tablosundan alınan alanların program kodu içerisinde hiç bir yerde kullanılmadığı bir durumda, bu tablo alanlarının Person Entity tipi içerisine alınması anlamsızdır. Nitekim, performans açısından gereksiz Inner Join sorgusu atılmasına neden olmaktadır.

Sonuç olarak Splitting özelliği yardımıyla bir Entity içerisinde yer alan özelliklerin (Property), veritabanı tarafında birden fazla Tablo'nun alanlarına (Fields) ait olması sağlanabilmektedir. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Splitting_RC.rar (49,90 kb)](/assets/files/2010/Splitting_RC.rar) [Örnek Visual Studio 2010 Ultimate RC sürümü üzerinde geliştirilmiş ve test edilmiştir]
