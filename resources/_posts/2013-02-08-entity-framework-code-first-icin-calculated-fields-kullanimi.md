---
layout: post
title: "Entity Framework Code-First için Calculated Fields Kullanımı"
date: 2013-02-08 05:15:00 +0300
categories:
  - entity-framework
tags:
  - entity-framework
  - csharp
  - linq
  - t-sql
---
Genellikle göç etmek gibi anlamlarda kullanılan Migrate kelimesinin yazılım dünyasındaki karşılığını düşündüğümüzde, elbetteki yandaki fotoğrafta yer alan ve bir birlerinin akvaryumuna atlayan balıklar gelmeyecektir/gelmemelidir.

[![hot-water-migration](/assets/images/2013/hot-water-migration_thumb.jpg)](/assets/images/2013/hot-water-migration.jpg)


Ancak Entity Framework Code-First yaklaşımı ve Calculated Fields kavramını göz önüne getirdiğimizde, Migration kelimesini ciddi manada düşünmemiz gerekebilir. Nasıl mı? Haydi okumaya devam

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_134.png)

Hesaplanmış alanlar (Calculated Fields/Columns) veritabanı programcılığında sık kullanılan özelliklerden birisidir. Bu alanların içeriği genellikle tablonun diğer alanları kullanılarak bir hesaplama sonucu üretilir. Söz gelimi personel verilerinin tutulduğu bir tablodaki FirstName ve LastName alanlarının değerleri birleştirilerek, bir Calculated Field oluşturulması mümkündür. Peki bu desteği Entity Framework Code-First yaklaşımında nasıl değerlendirebiliriz?

Bildiğiniz üzere Entity Framework Code-First yaklaşımında, veritabanı nesnelerinin tasarımları POCO (Plain Old CRL Object) tipleri üzerinden gerçekleştirilmektedir. Dolayısıyla Calculated Field şeklinde düşünülmesi gereken bir özelliğin veritabanı tarafına nasıl yansıtılacağı kafalarda bir soru işareti oluşturmaktadır. Pek tabi bunun için de bir nitelik (attribute) desteği sunulmuş olabilir ki öyledir. DatabaseGenerated niteliğinde DatabaseGeneratedOption.Computed enum sabiti değerini kullanarak, istenilen hesaplanabilir alan bildirimlerini yaptırabiliriz. Acaba durum gerçekten böyle midir?

![Who me?](/assets/images/2013/wlEmoticon-whome.png)

Dilerseniz basit bir örnek üzerinden hareket ederek konuyu incelemeye çalışalım. İlk etapta aşağıdaki sınıf çizelgesinde (Class Diagram) yer alan tipleri geliştirdiğimizi düşünelim. Senaryomuzdaki başrol oyuncuları, Shop isimli Context tipi ve Product sınıfının TotalPrice özelliğidir.

[![efcf_3](/assets/images/2013/efcf_3_thumb.png)](/assets/images/2013/efcf_3.png)

Product isimli örnek POCO (Plain Old CLR Object) tipi;

```csharp
using System.ComponentModel.DataAnnotations.Schema;

namespace HowTo_CalculatedFields 
{ 
    public class Product 
    { 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int ProductId { get; set; } 
        public string Name { get; set; } 
        public int ListPrice { get; set; } 
        public int Quantity { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)] 
        public int TotalPrice { 
            get; 
            private set; 
        } 
    } 
}
```

DbContext türevli Context tipi;

```csharp
using System.Data.Entity;

namespace HowTo_CalculatedFields 
{ 
    public class Shop 
        :DbContext 
    { 
        public DbSet<Product> Products{ get; set; } 
    } 
}
```

Product tipi içerisinde yer alan TotalPrice özelliğine dikkat edelim. Bu özellik içerisinde ürünün fiyatı ve miktarından yararlanılarak gerçekleştirilen bir hesaplama işlemi söz konusudur. Bunun veritabanı tarafına da yansıtılması için DatabaseGenerated niteliğinden yararlanılmaktadır. Peki çalışma zamanı bu durumu anlayabilecek midir?

> Örneğimizde Code-First yaklaşımına istinaden config dosyasında aşağıdaki bağlantı bilgisini kullanmayı tercih ettim. Herhangibir bilgi ifade etmediğimizde SQL Express sürümü üzerinde bir veritabanı oluşturulmaya çalışıldığını hatırlatmak isterim. Diğer önemli bir nokta da DbContext türevli sınıf adı ile ConnectionString elementinin name niteliğinin değerlerinin aynı olmasıdır. Bu sayede çalışma zamanı Shop veritabanı için gerekli bağlantı bilgisini bulabilir.
> name="Shop"
> connectionString="data source=localhost;database=Shop;integrated security=true"
> providerName="System.Data.SqlClient"/>

Program.cs içeriğini aşağıdaki şekilde kodlayarak senaryomuza devam edelim.

```csharp
using System; 
using System.Linq;

namespace HowTo_CalculatedFields 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            using (Shop context = new Shop()) 
            { 
                Product hpKeyboard = new Product 
                { 
                     Name="HP 102 Tuş Kablosuz Klavye", 
                     ListPrice=35, 
                     Quantity=125 
                };

                context.Products.Add(hpKeyboard); 
                context.SaveChanges();

                var finded = (from k in context.Products 
                     where k.Name == "HP 102 Tuş Kablosuz Klavye" 
                     select k) 
                    .FirstOrDefault(); 
                Console.WriteLine(finded.TotalPrice); 
            } 
        } 
    } 
}
```

Shop context tipinin örneklenmesinin ardından bir Product nesnesi üretilmektedir. Dikkat edileceği üzere identity alan olarak set edilen ProductId ve Calculated Field olması planlanan TotalPrice için bir atama işlemi söz konusu değildir. Beklentimiz yeni Product, context üzerine eklendiğinde TotalPrice alanınında otomatik olarak hesaplanmış olmasıdır. Lakin bu aşamaya kadar ilerleyemeyiz bile. [![efcf_1](/assets/images/2013/efcf_1_thumb.png)](/assets/images/2013/efcf_1.png)

Dikkat edileceğiz üzere SaveChanges metoduna yapılan çağrı sonrasında bir çalışma zamanı istisnası (Runtime Exception) oluşmuştur. Söylenene göre TotalPrice alanı null değer içeremez. Aslında bu mesajı doğrudan Calculated Field ile ilişkili değildir. Yine de veritabanı tarafına baktığımızda şöyle bir durum oluştuğunu gözlemleyebiliriz; Shop isimli veritabanı üretilmiş, Products isimli tablo oluşturulmuş ve hatta içerisine TotalPrice isimli alan da dahil edilmiştir. Hımmm...Ne var ki TotalPrice kolonu Calculate Field haline gelmemiştir.

[![efcf_4](/assets/images/2013/efcf_4_thumb.png)](/assets/images/2013/efcf_4.png)

Peki ya çözüm?

![I don't know smile](/assets/images/2013/wlEmoticon-idontknowsmile.png)

Neyseki elimizin altında migration diye bir kabiliyet bulunmakta. Şu anda var olan veritabanı yapısını biraz değiştirip, TotalPrice alanı için de bir müdahalede bulunmamız gerekecek. (Hatta Name alanının boyutuna bir dokunuş yaparsak hiç de fena olmaz ![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_134.png))

Şimdi Migration özelliğini etkinleştirip yeni bir Migration setini projeye dahil ediyor olacağız. Bunun için Package Manager Console penceresinden sırasıyla Enable-Migrations ve Add-Migration komutlarını çağıralım. Aşağıdaki gibi.

[![efcf_2](/assets/images/2013/efcf_2_thumb.png)](/assets/images/2013/efcf_2.png)

AddTotalPriceCalculateFields olarak adlandırdığımız Migration sınıfının içeriğinde yer alan Up ve Down metodlarını ise şu şekilde düzenleyebiliriz.

```csharp
namespace HowTo_CalculatedFields.Migrations 
{ 
    using System; 
    using System.Data.Entity.Migrations; 
    
    public partial class AddTotalPriceCalculatedFields 
        : DbMigration 
    { 
        public override void Up() 
        { 
            DropTable("dbo.Products");

            CreateTable( 
                "dbo.Products", 
                c => new 
                { 
                    ProductId = c.Int(nullable: false, identity: true), 
                    Name = c.String(maxLength:50), 
                    ListPrice = c.Int(nullable: false), 
                    Quantity = c.Int(nullable: false) 
                }) 
                .PrimaryKey(t => t.ProductId);

            Sql("ALTER TABLE dbo.Products ADD [TotalPrice] as ([ListPrice] * [Quantity])"); 
        }

        public override void Down() 
        { 
            DropTable("dbo.Products"); 
        } 
    } 
}
```

Aslında iki noktaya dokunduk. İlk olarak TotalPrice alaının eklenmesi için herhangibir işlem yapmadığımızı görüyoruz. İkinci olarak da bir T-SQL ifadesinin çalıştırılması için gerekli metod çağrısında bulunduk. Sql Metod çağrısına dikkat edilecek olursa Calculated Field için gerekli olan T-SQL ifadesini içerdiğini görebiliriz. Kısacası tablo Create edildikten sonra bir Alter işlemini bilinçli olarak uygulatıyor ve hesaplanabilir alanın bildirilmesini sağlıyoruz.

Artık veritabanını manuel olarak güncelletebiliriz. Bu güncelleme işlemi için Package Manager Console üzerinden Update-Database komutunu göndermemiz yeterli olacaktır

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_134.png)

> Verbose anahtarını kullanmamızın tek sebebi, veritabanına doğru giden T-SQL ifadelerini görmektir.

[![efcf_5](/assets/images/2013/efcf_5_thumb.png)](/assets/images/2013/efcf_5.png)

Bu adımdan sonra veritabanına gidip Products tablosuna baktığımızda, gerçektende TotalPrice için bir Calculated T-SQL ifadesinin yazılmış olduğunu görebiliriz.

[![efcf_6](/assets/images/2013/efcf_6_thumb.png)](/assets/images/2013/efcf_6.png)

Üstelik çalışma zamanında bir ürünü çektiğimizde, miktar ve birim fiyata göre TotalPrice özelliğinin de veritabanından hesaplanarak getirildiğini görebiliriz.

[![efcf_7](/assets/images/2013/efcf_7_thumb.png)](/assets/images/2013/efcf_7.png)

Herşey buraya kadar iyi gitti diyebiliriz. Lakin ufak bir sorunumuz daha var.

![Laughing out loud](/assets/images/2013/wlEmoticon-laughingoutloud_1.png)

Eğer Quantity veya ListPrice değerlerinde, nesne örneği üzerinden değişiklik yaparsak, bu durumda Calculated Field beklediğimiz gibi bir davranış göstermeyecektir. Aşağıdaki ekran görüntüsünde yer alan kod parçasını dikkate alalım.

[![efcf_8](/assets/images/2013/efcf_8_thumb.png)](/assets/images/2013/efcf_8.png)

Senaryoda ürün eklendikten sonraki durumda Calculated Field alanının hesaplanarak geldiği görülmektedir. Yani ilk eklemeden sonra gerçekleştirilen LINQ ifadesine göre TotalPrice için SQL tarafındaki hesaplama devreye girmiştir. Ancak bellek üzerinde kalan Product nesne örneğinin Quantity veya ListPrice alanlarında bir değişiklik yapıldığında, bu çok doğal olarak TotalPrice'a yansımayacaktır.

Bu durum çok doğal olarak veritabanına gidilmeden yapılan nesne örneği bazlı özellik güncellemelerinde doğru verinin gösterilemeyeceği anlamına gelir ki bu da pek istemediğimiz bir durumdur. Soruna Product tipi içerisindeki TotalPrice özelliği üzerinden müdahalede bulunarak çözüm getirebiliriz. Aynen aşağıdaki kod parçasında görüldüğü gibi;

```csharp
using System.ComponentModel.DataAnnotations.Schema;

namespace HowTo_CalculatedFields 
{ 
    public class Product 
    { 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int ProductId { get; set; } 
        public string Name { get; set; } 
        public int ListPrice { get; set; } 
        public int Quantity { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)] 
        public int TotalPrice 
        { 
            get 
            { 
                return ListPrice * Quantity; 
            } 
            private set // get bloğunu açtığımız için aşağıdaki bloğu boş olsa bile açmak mecburiyetindeyiz. 
            { 
            } 
        } 
    } 
}
```

İşte şimdi oldu

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_134.png)

[![efcf_9](/assets/images/2013/efcf_9_thumb.png)](/assets/images/2013/efcf_9.png)

Böylece geldik bir yazımızın daha sonuna. Bu makalemizde Code-First yaklaşımının kullanıldığı senaryolarda, biraz da veritabanı tarafına özgü olan Calculated Field’ ların nasıl etkin hale getirilebileceğini bir kaç küçük hile ile incelemeye çalıştık. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[HowTo_CalculatedFields.zip (2,60 mb)](/assets/files/2013/HowTo_CalculatedFields.zip)