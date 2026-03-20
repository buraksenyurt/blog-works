---
layout: post
title: "Entity Framework 6 – Code First için Convention Nedir?"
date: 2012-12-12 05:15:00 +0300
categories:
  - entity-framework
tags:
  - entity-framework
  - csharp
  - xml
  - http
  - reflection
  - generics
---
Entity Framework takımı aldı başını gidiyor. Kim durduracak onları. Onlarda The Mask filmindeki karakter gibi “Somebody stop me!” demiyor ki... Aslında bakarsanız olaylar bana göre, Microsoft geliştirici takımlarının, diğer geliştiricilerin seslerini duymaya ve dikkate almaya başlamasından sonra epeyce gelişti.

[![themask](/assets/images/2012/themask_thumb.jpg)](/assets/images/2012/themask.jpg)

Microsoft’ un çeşitli takımlarının açtığı anketler sayesinde, geliştiricilerin talepleri dinleniyor, değerlendiriliyor ve kayda değer olanlar planlanıp peyder pey yeni sürümlere ilave ediliyor. Hatta takımların ortaya koyduğu “şu da olsa nasıl olur?” ruh halindeki öğeler de geliştiriciler tarafından oylanıyor ve aynı sürece dahil edilebiliyor.(Bloğumdaki takip ettiklerim listesinde bir kaç survey adresini bulabilirsiniz)

Hal böyle olunca çok doğal olarak bir sürü sürüm çıkıyor ve var olanlar çabucak eskiyor. Takip edilmesi zor olan ve özellikle Enterprise seviyede ki projelerde “acaba bu teknolojiyi kullanabilir miyiz?” gibi soruların doğmasına ve ne yazık ki negatif olarak yanıtlanmasına neden olabilecek bir durum bu. Fakat biz yine de üstümüze düşen görevi yapalım ve gerekli anlatımımızı icra ederek öğrendiklerimizi sizlerle paylaşalım. Öyleyse başlayalım

![Smile](/assets/images/2012/wlEmoticon-smile_74.png)

[Makalede yazılanlar Entity Framework 6 Alpha 2 sürümünü baz almaktadır]

Entity Framework alt yapısının sunduğu önemli yaklaşımlardan birisi de Code-First modelidir. Bu modele göre geliştiriciler, önce sınıfları basit POCO (Plain Old CLR Objects) tipler şeklinde tasarlar. Böylece Conceptual (Domain) Model oluşturulur. POCO tiplerinin tek başına tasarlanması elbette yeterli değildir. DbContext türevli bir sınıfında, model de kullanılması düşünülen POCO tiplerine ait koleksiyon bazlı özellikleri içeriyor olması gerekmektedir. Bu noktada DbSet tipinden yararlanılır. Ayrıca tipler arasın ilişkileri betimleyen Navigation Property’ ler de tasarlanır.

Sonrasında ise Entity Framework ilgili Context tipinden yararlanarak çalışma zamanında gerekli veritabanı üretimini icra eder. Peki hiç şu soru aklınıza geldi mi;

Bu üretim işlemi sırasında tablolar hangi kuralar göre oluşur, hangi alan Primary Key kabul edilir, tablolar arasındaki ilişkiler (Relations) nasıl belirlenir vb.

İşte bu yazımızda bu soruya biraz daha açıklık getirmeye çalışıyor olacağız.

Code First yaklaşımında, veritabanı tarafının üretilmesi aşamasında devreye girmekte olan bir takım kurallar bütünü bulunmaktadır. Convention olarak adlandırılan bu kurallar bütünü aslında System.Data.Entity.ModelConfiguration.Conventions isim alanı altında yer alan bazı tipler yardımıyla ifade edilmektedir. (Entity Framework 5.0 sürümü için ilgili isim alanında (namespace) yer alan tipleri [bu adresten inceleyebilirsiniz](http://msdn.microsoft.com/en-us/library/system.data.entity.modelconfiguration.conventions(v=vs.103).aspx))

Burada pek çok Convention tipi yer almaktadır. İlk olarak bu basit Convention tiplerinden bazılarını kavramsal olarak incelemeye çalışalım. Bu amaçla aşağıdaki basit içeriğe sahip olduğumuz bir örnek üzerinden ilerleyebiliriz.

[![efcon_2](/assets/images/2012/efcon_2_thumb.png)](/assets/images/2012/efcon_2.png)

```csharp
using System; 
using System.Collections.Generic; 
using System.Data.Entity;

namespace HowTo_EFCodeFirstConvetions 
{ 
    public class AzonBookShop 
       :DbContext 
    { 
        public DbSet<Book> Books { get; set; } 
        public DbSet<Category> Categories { get; set; }

        static AzonBookShop() 
        { 
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<AzonBookShop>()); 
        } 
    }

    public class Category 
    { 
        public Guid ID { get; set; } 
        public string Name { get; set; } 
        public virtual ICollection<Book> Books { get; set; } 
    }

    public class Book 
    { 
        public int BookID { get; set; } 
        public string Title { get; set; } 
        public decimal ListPrice { get; set; } 
        public virtual Category Category { get; set; } 
        public int CategoryId { get; set; } 
    } 
}
```

> Örnek üzerinde ilerlerken modeli sıkça değiştireceğimizden, static yapıcı metod (Constructor) içerisinde bir strateji seçerek, Initialize sırasında eğer model de bir değişiklik olmuşsa Drop işlemlerinin uygulanması gerektiği belirtilmiştir.

Şimdi bu modele göre üretilen veritabanı şemasını şöyle kısaca bir inceleyelim dilerseniz.

[![efcon_1](/assets/images/2012/efcon_1_thumb.png)](/assets/images/2012/efcon_1.png)

Dikkat edileceği üzere Categories ve Books isimli iki tablo üretilmiştir. Her iki tabloda birer Primary Key alan bulunmaktadır. İşte burada Primary Key Convention kural kümesi devreye girmektedir. Bu kural setine göre, Guid veya int tipinden olup adı ID veya [SınıfAdı][Id] notasyonunda olan özellikler, veritabanı şemasında birer Identity alan olarak üretilecek ve hatta Primary Key şeklinde işaretleneceklerdir.

Senaryomuza Class seviyesinde baktığımızda, bir kategorinin altında birden fazla kitabın yer alabileceği görülmektedir. Nesneler arası kurulan bu ilişkiyi (association) tanımlamak adına Category sınıfı içerisinde ICollection tipinden bir özellik kullanılmıştır. Bunun karşılığı olarak veritabanı şemasında görüldüğü üzere iki tablo arasında bir relation kurulmuştur. Bu ilişki, Categories tablosundan Books tablosuna doğru one-to-many olacak şekildedir. Burada ise Relation Convention kuralları devreye girmektedir.

Örnekte kasıtlı olarak Book sınıfı içerisidne CategoryId isimli ayrı bir özellik daha tanımlanmıştır. Mantıksal olarak bu özellik bir kitabın bağlı olduğu Category tipini işaret etmek üzere planlanmıştır. Ancak Relation Convention kurallarına göre isimlendirme de bir sorun vardır. Category tablosunun Identity şeklindeki Primary Key alanı ID olarak belirlenmiştir. Bu sebepten tablo şemasına bakıldığında CategoryID isimli bir alanın daha eklendiği ve iki tablo arasındaki bire çok ilişkinin, bu alan üzerinden sağlandığı görülmektedir.

[![efcon_3](/assets/images/2012/efcon_3_thumb.png)](/assets/images/2012/efcon_3.png)

```text
USE [AzonBookShop] 
GO

ALTER TABLE [dbo].[Books]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Books_dbo.Categories_Category_ID] FOREIGN KEY([Category_ID]) 
REFERENCES [dbo].[Categories] ([ID]) 
GO

ALTER TABLE [dbo].[Books] CHECK CONSTRAINT [FK_dbo.Books_dbo.Categories_Category_ID] 
GO
```

Relation ile ilişkili SQL script’ i içinde de görüldüğü üzere Books tablosunda yer alan CategoryID ForeignKey olarak belirlenmiş ve Books tablosundaki ID alanına bağlanmıştır.

Şimdi dilerseniz örneğimizi biraz daha genişletelim ve Context için aşağıdaki sınıf çizelgesinde yer alan POCO tipini eklediğimizi düşünelim.

[![efcon_5](/assets/images/2012/efcon_5_thumb.png)](/assets/images/2012/efcon_5.png)

```csharp
public class Book 
{ 
    public int BookID { get; set; } 
    public string Title { get; set; } 
    public decimal ListPrice { get; set; } 
    public virtual Category Category { get; set; } 
    //public int CategoryId { get; set; } 
    public Detail BookDetail { get; set; } 
}

public class Detail 
{ 
    public int PageSize { get; set; } 
    public double Weight { get; set; } 
    public bool HardCover { get; set; } 
    public string Language { get; set; } 
}
```

Book sınıfına Detail tipinden BookDetail isimli bir özellik eklenmiştir. Detail sınıfının dikkat çekici özelliği ise Primary Key Convention kuralına uygun bir özellik içermiyor olmasıdır. Bu durumuda Complex Type Convention kural seti devreye girecektir ve veritabanı tarafında aşağıdaki sonuçların oluşmasına neden olacaktır.

[![efcon_4](/assets/images/2012/efcon_4_thumb.png)](/assets/images/2012/efcon_4.png)

Görüldüğü üzere Detail sınıfının özellikleri, Book tablosu içerisinde birer alan (Field) haline getirilmiştir.

Örnekte, makinede yüklü olan SQL 2008 sunucusu kullanılmıştır. Bu nedenle app.config dosyası içerisinde aşağıdakine benzer bir ConnectionString bilgisi yer almaktadır. name niteliğinin değerinin Context sınıf adı ile aynı olması önemlidir.

```xml
<connectionStrings> 
    <add name="AzonBookShop" connectionString="data source=.;database=AzonBookShop;integrated security=SSPI" providerName="System.Data.SqlClient"/> 
</connectionStrings>
```

Bu noktada aslında Connection String Convention kural seti devreye girmektedir. Bu kural setinin veritabanı şemasını oluştururken baktığı yerlerden birisi, config dosyasındaki connectionStrings elementi içeriğidir. Eğer name özelliğinin değeri ile DbContext türevli Context tipinin adı eşlenirse, o elemente ait Connection bilgisi kullanılaraktan bir şema üretimi gerçekleştirilecektir.

## Ötesi

Buraya kadar anlatmaya çalıştığımız Convention kural setlerinin daha pek çok özelliği bulunmaktadır. Data Annotations ve Fluent API kullanımı gibi durumlarda ilgili Convention kural kümelerinin ezilmesi vb mümkündür. İstenirse bir Convention kural seti devre dışı bırakılabilir de. Bunun için DbContext üzerinden gelen OnModelCreating metodunun ezilmesi ve içerisinde aşağıdakine benzer bir kodun kullanılması yeterlidir.

```csharp
using System; 
using System.Collections.Generic; 
using System.Data.Entity; 
using System.Data.Entity.ModelConfiguration.Conventions;

namespace HowTo_EFCodeFirstConvetions 
{ 
    public class AzonBookShop 
        :DbContext 
    { 
        public DbSet<Book> Books { get; set; } 
        public DbSet<Category> Categories { get; set; }

        static AzonBookShop() 
        { 
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<AzonBookShop>()); 
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) 
        { 
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>(); 
        } 
    } 
    ...
```

modelBuilder tipi üzerinden Conventions özelliği ile Remove metoduna erişilmekte ve PluralizingTableNameConvention sınıfı generic parametre olarak belirtilmektedir. Örneğimizin önceki kısımlarında veritabanı tarafında üretilen tablo adları mutlaka dikkatinizi çekmiştir. Çoğul isimlendirme kuralına göre üretilmişlerdir. Ancak OnModelCreating içerisinden bu kural setini kaldırmamız, tablo adlarının sınıf adları olarak tanımlanmasını sağlamaktadır.

[![efcon_6](/assets/images/2012/efcon_6_thumb.png)](/assets/images/2012/efcon_6.png)

Peki, Convention kuralları ile ilişkili olarak daha başka neler yapabiliriz? Özellikle bunları manuel olarak ele abilir miyiz? Var olan Convention kurallarını geçersiz kılarak kendi istediğimiz ayarların devreye girmesini nasıl sağlarız?

Convetion kurallarını manuel olarak ele almanın bir kaç yolu bulunmaktadır. Bunlardan birisi Lightweight modelidir. Bu modelde önce bir filtreleme işlemi yapılır ve işlem sonucuna göre konfigurasyonun değiştirilerek uygulanması sağlanır. Örneğin modelimizde yer alan Category sınıfının içeriğini aşağıdaki gibi değiştirdiğimiz düşünelim.

```csharp
public class Category 
{ 
	public Guid Signature { get; set; } 
	public string Name { get; set; } 
	public virtual ICollection<Book> Books { get; set; } 
}
```

Burada ID isimli özelliğin Signature olarak değiştirildiği görülmektedir. Bu değişiklik nedeniyle pek tabi Primary Key Convention kural seti uygulanamayacaktır. Daha da kötüsü, senaryomuz gereği Category ile Book arasında bir relation tesis edilebilmesi için gerekli Foreign Key bulunamayacak ve çalışma zamanında aşağıdaki Exception ile karşılaşılacaktır.

[![efcon_7](/assets/images/2012/efcon_7_thumb.png)](/assets/images/2012/efcon_7.png)

İşte bu noktada Lightweight Convention tekniği ile durum çözümlenebilir. Bunun için yine OnModelCreating içerisinde bazı işlemler yapılması gerekmektedir. Aynen aşağıda görüldüğü gibi.

> Söz konusu kodda yer alan Properties özelliği EF 6.0 Alpha 2 sürümünde duyurulmuştur. Dolayısıyla bundan sonraki kodlar için güncel PreRelease sürümünü kurarak devam etmelisiniz. Kurulum için [şuradaki adresten](http://msdn.microsoft.com/en-us/data/ee712906) yararlanabilirsiniz

```csharp
protected override void OnModelCreating(DbModelBuilder modelBuilder) 
{ 
    modelBuilder.Conventions.Remove<PluralizingTableNameConvention>(); 
    modelBuilder 
        .Properties() 
        .Where(p => p.Name.Contains("Signature")) 
        .Configure(p => p.IsKey()); 
}
```

modelBuilder üzerinden Properties metodu kullanılarak, Entity'deki özellikler arasında Signature kelimesini içeren bir tane olup olmadığına bakılmakta ve eğer var ise IsKey metodu çağrısı ile bunun bir Identity alan olması gerektiği (bir başka deyişle Primary Key Convention kurallarının uygulanması gerektiği) vurgulanmaktadır. Buna göre çalışma zamanı sonucunda veritabanı tarafında aşağıdaki şemanın üretildiği gözlemlenecektir.

[![efcon_8](/assets/images/2012/efcon_8_thumb.png)](/assets/images/2012/efcon_8.png)

Convention çeşitlerinden bir diğeri de Model-Based olan versiyondur. Bu teknikte doğrudan model ile çalışma ve ayarlama şansına sahip oluruz. İlgili tekniği uygulayabilmek için IEdmConvention, IDbConvention ve IDbMappingConvention arayüzlerini (interface) implemente eden sınıflardan yararlanırız. Aşağıdaki basit örneği göz önüne alalım.

[![efcon_10](/assets/images/2012/efcon_10_thumb.png)](/assets/images/2012/efcon_10.png)

```csharp
using System.Data.Entity.Core.Metadata.Edm; 
using System.Data.Entity.ModelConfiguration.Conventions;

namespace HowTo_EFCodeFirstConvetions 
{ 
    public class StringLengthConversion 
        : IEdmConvention<EdmProperty> 
    { 
        public void Apply(EdmProperty edmDataModelItem, EdmModel model) 
        { 
            if (edmDataModelItem.PrimitiveType.PrimitiveTypeKind == PrimitiveTypeKind.String) 
                edmDataModelItem.MaxLength = 200; 
        } 
    } 
}
```

IEdmConvention interface'ini implemente eden StringLengthConversion sınıfı Apply metodunu uygulamaktadır. Bu metodun içerisinde, edmDataModelItem isimli değişkenin String tipi olup olmadığına bakılmakta ve eğer öyleyse Max Length değeri 200 karakter ile sınırlandırılmaktadır.

Bu işlem pek tabi Model içerisinde yer alan ne kadar String tipte öğe var ise geçerli olacktır. Tabi söz konusu sınıfın devreye girebilmesi için yine OnModelCreating içerisine müdahale edilmelidir. Aşağıdaki kod parçasında görüldüğü gibi.

```csharp
protected override void OnModelCreating(DbModelBuilder modelBuilder) 
{ 
    modelBuilder.Conventions.Remove<PluralizingTableNameConvention>(); 
    modelBuilder 
        .Properties() 
        .Where(p => p.Name.Contains("Signature")) 
        .Configure(p => p.IsKey()); 
    modelBuilder 
    .Conventions 
    .AddBefore<StringLengthAttributeConvention>(new StringLengthConversion()); 
}
```

Bu işlem sonucunda veritabanı şemasında aşağıdaki sonuçların oluştuğu gözlemlenecektir.

[![efcon_9](/assets/images/2012/efcon_9_thumb.png)](/assets/images/2012/efcon_9.png)

Bir başka Convention modeli ise Configuration tabanlı olanıdır. Bu teknikte IConfigurationConvention arayüzünün (intercace) implenente edildiği bir sınıfın devreye girerek Convention kurallarına müdahale etmesi söz konusudur. Aşağıdaki örnek sınıfı göz önüne alalım.

[![efcon_11](/assets/images/2012/efcon_11_thumb.png)](/assets/images/2012/efcon_11.png)

```csharp
using System; 
using System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive; 
using System.Data.Entity.ModelConfiguration.Conventions; 
using System.Reflection;

namespace HowTo_EFCodeFirstConvetions 
{ 
    public class StringColumnTypeConvention 
        :IConfigurationConvention<PropertyInfo,StringPropertyConfiguration> 
    { 
        public void Apply(PropertyInfo memberInfo 
           , Func<StringPropertyConfiguration> configuration) 
        { 
           if (configuration().ColumnType == null) 
            { 
                configuration().ColumnType = "nvarchar"; 
                configuration().IsNullable = false; 
                configuration().MaxLength = 50; 
            } 
        } 
    } 
}
```

Öncelikli olarak ColumnType özelliğinin değerinin null olup olmadığın bakılmaktadır. Bu işlem, ilgili alanın daha önceden oluşturulup oluşturulmadığını da işaret etmektedir. Pek tabi Convention kurallarının devreye girebilmesi için yine OnModelCreating metoduna bir müdahale de bulunmak gerekmektedir. Aşağıdaki kod parçasında bu durumu görebilirsiniz.

```csharp
protected override void OnModelCreating(DbModelBuilder modelBuilder) 
{ 
    modelBuilder.Conventions.Remove<PluralizingTableNameConvention>(); 
    modelBuilder 
        .Properties() 
        .Where(p => p.Name.Contains("Signature")) 
        .Configure(p => p.IsKey());

    modelBuilder.Conventions.Add<StringColumnTypeConvention>(); 
}
```

Bu işlem sonrasında veri tabanı şemasının aşağıdaki gibi üretildiği görülecektir.

[![efcon_12](/assets/images/2012/efcon_12_thumb.png)](/assets/images/2012/efcon_12.png)

Dikkat edileceği üzere String özelliklerin karşılığı olarak nvarchar tipinde olan, null değer içeremeyen ve maksimum 50 karakter uzunluğunda içerik tutabilen alanlar üretilmiştir.

Code First yaklaşımında Convention kullanımı ile ilişkili olarak daha ileri seviye uygulamalar da mevcuttur. Söz gelimi Custom Attribute’ lar le yeni Convention kural setleri tanımlanabilir. Özellikle LightWeight modelinde kullanılabilecek epey fazla fonksiyonllik bulunmaktadır. Bu konuda [şu adresteki](http://msdn.microsoft.com/en-us/data/jj819164) yazının son kısımlarını da değerlendirebilir ve kendi denemelerinizi yaparak konuyu irdelemeye çalışabilirsiniz. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_162.png)

[Makalede yazılanlar Entity Framework 6 Alpha 2 sürümünü baz almaktadır]

[HowTo_EFCodeFirstConvetions.zip (2,14 mb)](/assets/files/2012/HowTo_EFCodeFirstConvetions.zip) (Dosya boyutunun büyümemesi için Packages klasörü çıkartılmıştır. EF'in 6ncı sürümünü projeye indirmeniz gerekebilir)