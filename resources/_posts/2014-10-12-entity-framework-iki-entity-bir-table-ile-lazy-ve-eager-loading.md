---
layout: post
title: "Entity Framework–İki Entity Bir Table ile Lazy ve Eager Loading"
date: 2014-10-12 02:00:00 +0300
categories:
  - entity-framework
tags:
  - entity-framework
  - lazy-loading
  - eager-loading
  - code-first-development
---
Yandaki görüntü 1988 yılında Mevlüt Dinç (nam-ı diğer Mev Dinc) tarafından kurulan [Vivid Image](http://en.wikipedia.org/wiki/Vivid_Image) firmasının geliştirdiği oyunlardan birisine ait. [The First Samurai](http://en.wikipedia.org/wiki/First_Samurai_(video_game)). Mev Dinc ülkemizin yetiştirdiği en önemli değerlerden birisidir. Kendisi ile NedirTv topluluğunda yapılmış güzel bir röportaj da bulunmaktadır. Pek çoğumuz onu, [SOBEE](http://www.sobee.com.tr) firması ile de tanımıştır. Ben ise uzun zaman önce Microsoft’ un Darphane’ deki binasında katıldığım bir söyleşiden…

[![eftsplit_0](/assets/images/2014/eftsplit_0_thumb.png)](/assets/images/2014/eftsplit_0.png)


MVP olduğum o dönemlerde Microsoft Türkiye düzenlediği bir etkinlik ile onu karşımıza çıkartmıştı. Kendisini büyük bir keyifle dinlemiştik. Nasıl bu günlere geldiğinden, geliştirdiği oyunlardan, kurduğu SOBEE firması'nın projelerinden bahsetmişti. Hatta akılda kalan önemli ifadelerden birisi de, yeni geliştirmekte oldukları oyunlarda C++ yerine C# programlama dilini tercih etmeleriydi. (Sene 2007 olabilir) Ancak benim daha çok aklımda kalan tam olarak hatırlayamasam da aşağıda yazan ifadeleriydi.

> Bir savaş oyununda binanın tamamını düşünmeye gerek yoktur. O an için sadece aktörlerin bulunduğu odayı düşünmek yeterlidir.

Şimdi nereden geldik bu sözlere. Geçtiğimiz günlerde Entity Framework üzerine bir takım araştırmalar yaparken Lazy ve Eager Loading işlemlerinin hangi noktalarda kullanılabileceğine dair bazı yararlı bilgiler edindim. Bununla birlikte bir senaryo gerçekten dikkatimi çekti.

Temel Gereksinim

Özellikle içerisinde CLOB veya BLOB benzeri alanlar barındıran tabloların Entity Framework tarafındaki kullanımlarında network yükünü hafifletmek adına bir tablonun iki Entity ile ifade edilebilmesi düşünülebilir. Böyle bir durumda Lazy Loading’ i tablo içerisindeki alanlar bazında uygulama şansına sahip oluruz. Bu, özellikle LINQ (Language INtegrated Query) sorgularını işlettiğimiz yerlerde performans açısından faydalı bir iyileştirmedir. Kısacası bir tablonun kendi alanları içerisinde ilişki kurup bunu Entity seviyesinde ifade etmemiz gerekmektedir. Gelin basit bir örnek üzerinden ilerleyerek konuyu incelemeye çalışalım.

> Console Application formundaki örnekte yazının yazıldığı tarih itibariyle Entity Framework 6.1.1 sürümü kullanılmıştır. Entity Framework, NuGet paket yönetim aracı ile projeye dahil edilebilir.

Model

Code First yaklaşımını baz alarak geliştireceğimiz örnekte ilk olarak aşağıdaki sınıf çizelgesinde (Class Diagram) yer alan tipleri inşa ederek işe başlayabiliriz. Temel olarak PDF,WMV gibi yüksek boyutlarda ve binary formatta ifade edilebilen veri içeriklerini taşıyacak bir SQL tablosunun iki farklı Entity ile ifade edildiğini söyleyebiliriz. (Ki bu sebepten DocumentContent isimli sınıf içerisinde yer alan Content ve FrontCover özellikleri byte[] tipinden tanımlanmışlardır)

![eftsplit_2](/assets/images/2014/eftsplit_2_thumb.png)

```csharp
using System.ComponentModel.DataAnnotations; 
using System.Data.Entity;

namespace HowTo_EFTable 
{ 
    public class CompanyBookContext 
        :DbContext 
    { 
        public DbSet<Document> Document { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) 
        { 
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Document>().ToTable("Documents"); 
            modelBuilder.Entity<DocumentContent>().ToTable("Documents");

            modelBuilder.Entity<Document>() 
                .HasRequired(c => c.Content) 
                .WithRequiredPrincipal(); 
        } 
    }

    public class Document 
    {   
        [Key] 
        public int DocumentID { get; set; } 
        public string Title { get; set; } 
        public int PageCount { get; set; } 
        public string Language { get; set; } 
        public string Genre { get; internal set; } 
        public string Publisher { get; set; } 
        public virtual DocumentContent Content { get; set; } 
        public string ISBN { get; internal set; } 
    }

    public class DocumentContent 
    { 
        [Key] 
        public int DocumentID { get; set; } 
        public byte[] Content { get; set; } 
        public byte[] FrontCover { get;set; } 
    } 
}
```

Pek tabi dikkat edilmesi gereken önemli noktalar bulunmaktadır. Söz gelimi Document ve aslında büyük boyutlu içerikleri barındıran DocumentContent sınıfları arasında bir ilişki vardır. Öyleki her ikisi de aslında aynı tabloyu işaret etmelidir. Bunun için her iki sınıfın DocumentID isimli özellikleri Key nitelikleri (attribute) ile işaretlenmiştir.

> Entity tiplerinde Key niteliği kullanılmazsa kuvvetle muhtemel aşağıdaki gibi bir çalışma zamanı hatası alınacaktır.[![eftsplit_1](/assets/images/2014/eftsplit_1_thumb.png)](/assets/images/2014/eftsplit_1.png)

Document tipi içerisinde yer alan Content isimli özellik aslında DocumentContent tipindedir ve bir Navigation Property şeklinde düşünülebilinir.(Sanki iki farklı tablo arasında one-to-one Relation kuruyoruz gibi düşünebiliriz)

Çok doğal olarak Code First yaklaşımının kullanıldığı bu örnekte modelin inşası sırasında da bazı özel işlemler yapılması gerekmektedir. Nitekim veri tabanı üzerinde tek bir tablo olması planlanmaktadır ve model’in içinde yer alan iki Entity’ nin aslında aynı tabloyu işaret edeceği veri tabanı nesnelerinin üretimi sırasında söylenebilmelidir.

Bu işlem ezilen (Override) OnModelCreating metodu içerisinde yapılmaktadır. Dikkat edileceği üzere Document ve DocumentContent isimli Entity tiplerinin aynı tablo’ yu (ki örnekte Documents) işaret ettikleri belirtilmektedir.

Bu arada Config

Uygulamada SQL Server kullanılmaktadır. Bu nedenle config dosyası içerisindeki connectionStrings elementi içeriği önemlidir. Hatırlanacağı üzere DbContext türevli sınıf adı ile aynı isimde bir connectionString elementinin bulunması gerekmektedir. Örnekte aşağıdaki bağlantı bilgisi kullanılmıştır.

```xml
<?xml version="1.0" encoding="utf-8"?> 
<configuration> 
  <configSections> 
    <section name="entityFramework" type="System.Data.Entity.Internal.ConfigFile.EntityFrameworkSection, EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false" /> 
  </configSections> 
  <startup> 
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5" /> 
  </startup> 
  <connectionStrings> 
    <add name="CompanyBookContext" connectionString="data source=.;database=Azon;integrated security=SSPI;" providerName="System.Data.SqlClient"/> 
  </connectionStrings> 
  <entityFramework> 
    <defaultConnectionFactory type="System.Data.Entity.Infrastructure.LocalDbConnectionFactory, EntityFramework"> 
      <parameters> 
        <parameter value="v11.0" /> 
      </parameters> 
    </defaultConnectionFactory> 
    <providers> 
      <provider invariantName="System.Data.SqlClient" type="System.Data.Entity.SqlServer.SqlProviderServices, EntityFramework.SqlServer" /> 
    </providers> 
  </entityFramework> 
</configuration>
```

Ana Uygulama Kodları

Örnekte temel olarak Lazy ve Eager Loading operasyonlarının özellikle SQL Script'ler bazında nasıl olduğu üzerinde durulmaktadır. Bu nedenle aşağıdaki anlamsız kod içeriği ele alınabilir. Bizim için önemli olan arka planda yürütülen SQL betikleridir.

Program.cs

```csharp
using System; 
using System.IO; 
using System.Linq;

namespace HowTo_EFTable 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            byte[] samplePDF = File.ReadAllBytes(@"c:\DomainDrivenDesignQuicklyOnline.pdf"); 
            byte[] sampleCover= File.ReadAllBytes(@"c:\SampleCover.png");

            using (CompanyBookContext context=new CompanyBookContext()) 
            { 
                context.Database.Log = Console.Write; // SQL Script' lerini izlemek için Log çıktısını Console olarak set ettik.

                Document someBook = new Document 
                { 
                    Language = "TR", 
                    ISBN="1234-3456-BOOK-1202", 
                    Title = "Domain Driven Design Quickly - Online Edition", 
                    Genre="Computer Books", 
                    PageCount = 348, 
                    Publisher="Your Best Publisher", 
                    Content = new DocumentContent 
                    { 
                        FrontCover =sampleCover, 
                        Content =samplePDF 
                    } 
                };

                context.Document.Add(someBook); 
                context.SaveChanges();

                #region Lazy Loading

                var dcmnt = (from d in context.Document 
                             where d.ISBN == "1234-3456-BOOK-1202" 
                             select d).FirstOrDefault(); 
                if (dcmnt != null) 
                { 
                    byte[] bookContent = dcmnt.Content.Content; 
                    Console.WriteLine(bookContent.Length.ToString()); // bookContent' i her hangibir şekilde kullanmassak ikinci Select işlemi gerçekleşmez. 
                }

                #endregion

                #region Eager Loading

                dcmnt = (from d in context.Document.Include("Content") 
                         where d.ISBN == "1234-3456-BOOK-1202" 
                         select d).FirstOrDefault(); 
                if (dcmnt != null) 
                { 
                    byte[] bookContent = dcmnt.Content.Content; 
                }

                #endregion 
            } 
        } 
    }    
}
```

Örnekte Document tipinden bir nesne örneği üretilmektedir. Dikkat edilmesi gereken nokta Content özelliğine de DocumentContent tipinden bir örneğin atanmış olmasıdır. Document nesne örneğinin DbSet’ e eklenmesi işlemi veri tabanı tarafından Documents isimli tabloya bir insert işlemi olarak algılanmaktadır.

Kodun sonraki kısmında ise oluşturulan Document içeriğinin tablodan iki farklı şekilde çekilme işlemi söz konusudur. İlkinde Lazy Loading tekniğinin uygulaması söz konusudur. Document içeriği çekilirken DocumentContent tipinin taşıyacağı alanlar ilk etapta alınmazlar. Ta ki Content özelliği kodda kullanılana dek (byte[] array'in boyutunun Console penceresine yazıldığı yer)

Eager Loading tekniğinin uygulandığı durumda ise DocumentContent tipinin işaret ettiği Content ve FrontCover alanlarının, LINQ ifadesindeki Include terimi nedeniyle Select işlemi sırasında çekilmesi söz konusudur. Yani tüm tablo içeriği, Lazy Loading’ in aksine Select ifadesi ile birlikte gelmektedir.

Çalışma Zamanı Analizi

Aslında durumu daha iyi analiz etmek adına çalışma zamanı çıktılarına bakabiliriz. Log içeriğini Console penceresine yansıttığımızdan, arka planda çalıştırılan SQL Script'lerini kolayca görebiliriz.

[![eftsplit_5](/assets/images/2014/eftsplit_5_thumb.png)](/assets/images/2014/eftsplit_5.png)

Şimdi ilk LINQ ifadesini ele alalım (Lazy Loading region’ lı kısım)

Arka planda ilk olarak Content ve FrontCover alanlarını içermeyen bir Select cümleciği çalıştırıldığı görülmektedir.

```text
SELECT TOP(1)
[Extent1].[DocumentID] as [DocumentID],
[Extent1].[Title] as [Title],
[Extent1].[PageCount] as [PageCount],
[Extent1].[Language] as [Language],
[Extent1].[Genre] as [Genre],
[Extent1].[Publisher] as [Publisher],
[Extent1].[ISBN] as [ISBN]
FROM [dbo].[Documents] as [Extent1]
WHERE N'1234-3456-BOOK-1202'=[Extent1].[ISBN]
```

Bu son derece doğaldır nitekim gelmeyen alanlar kodun o anki satırına kadar talep edilmemiştir. Ancak elde edilen Document nesne örneğinin Content özelliği üzerinden hareketle uzunluk bilgisi ekrana yazılmak istendiğinde, SQL tarafında ikinci bir Select ifadesinin çalıştırıldığı görülmektedir.

```text
SELECT TOP(1)
[Extent1].[DocumentID] as [DocumentID],
[Extent1].[Content] as [Content],
[Extent1].[FrontCover] as [FrontCover]
FROM [dbo].[Documents] as [Extent1]
WHERE [Extent1].[DocumentID]=@EntityKeyValue1
```

Aynı Where koşulu için çalıştırılan bu ifade de Documents tablosundan sadece FrontCover ve Content alanlarının çekildiğine dikkat edilmelidir. İşte bu, “ihtiyaç duyduğum yerde verileri yükle” felsefesi olarak yorumlanabilir. Kısaca Lazy Loading…

Gelelim ikinci ifadeye; Bu kez LINQ sorgusunda Include metodunun çağırıldığı ve parametre olarak Content isimli Navigation Property değerinin verildiği görülmektedir. Buna göre Documents tablosundaki tüm alanlar (Document ve DocumentContent Entity tiplerine ait özelliklerin işaret ettikleri) Select cümleciğine dahil edilmiştir.

```text
SELECT TOP(1)
[Extent1].[DocumentID] as [DocumentID],
[Extent1].[Title] as [Title],
[Extent1].[PageCount] as [PageCount],
[Extent1].[Language] as [Language],
[Extent1].[Genre] as [Genre],
[Extent1].[Publisher] as [Publisher],
[Extent1].[ISBN] as [ISBN],
[Extent1].[Content] as [Content],
[Extent1].[FrontCover] as [FrontCover]
FROM [dbo].[Documents] as [Extent1]
WHERE N'1234-3456-BOOK-1202'=[Extent1].[ISBN]
```

İşte bu da, “o an kullanmayacak olsam da tüm içeriği şu anda bana ver.” felsefesidir. Yani Eager Loading…

Tabi program çalıştırıldığında modelin inşasının sonuçlarına da bakılmalıdır. SQL Management Studio ile ilgili veritabanına gidilirse Documents isimli tek bir tablonun aşağıdaki şekildeki gibi oluşturulduğu görülebilir.

![eftsplit_3](/assets/images/2014/eftsplit_3_thumb.png)

Ayrıca kod tarafında gerçekleştirilen Insert işlemi sonrasında, iki farklı Entity örneğindeki özellik değerlerinin tek bir satır içerisine yerleştirildiği fark edilebilir.

[![eftsplit_4](/assets/images/2014/eftsplit_4_thumb.png)](/assets/images/2014/eftsplit_4.png)

Sonuç

Aslında yazımızın başında da belirttiğimiz ve Mev Dinc’ in ifade ettiği üzere, bazen olay anında ve yerinde tüm detayın olmasına gerek yoktur. O anda sadece bulunması gereken verilere ihtiyaç vardır. Bu felsefe görüldüğü üzere sadece oyun programlama tekniklerinde değil farklı konularda da karşımıza çıkmaktadır.Tabi bu felsefe kurgunun çok iyi yapılmasını gerektirir. Lazy ve Eager Loading teknikleri, diğer ORM araçlarında olduğu gibi Entity Framework’ ün de olmassa olmazlarıdır. Makalemizde ele aldığımız konu ince bir performans ayarını işaret etmektedir. Bir tabloyu kod tarafında parçalı şekilde ifade edebilmek, gerekli parçalarının Lazy Loading ile yüklenmesinin yolunu açmaktadır. Konu hakkında daha detaylı bilgiye [Peter Vogel'in MSDN Magazine'de yayınlanan makalesinden](http://visualstudiomagazine.com/articles/2014/09/01/splitting-tables.aspx) ulaşabilirsiniz.

Böylece geldik bir makalemizin daha sonuna. Bir bakşa makalemizde görüşünceye dek hepinize mutlu günler dilerim.