---
layout: post
title: "Entity Framework 6 –Yeni Esintiler"
date: 2014-04-08 11:09:00 +0300
categories:
  - entity-framework
tags:
  - entity-framework
  - xml
  - csharp
  - dotnet
  - linq
  - http
  - visual-studio
---
Tam da bu gün İstanbul’ da hafif rüzgarlı, güneşli bir bahar havası var. Binaların kapalı mekanlarında çalışan bizler için iki dakikalığına da olsa dışarıya çıkmanın, rüzgarın hafif esintisini ve güneşin ılık sıcaklığını hissetmenin değeri paha biçilemez.

[![ef61_6](/assets/images/2014/ef61_6_thumb.png)](/assets/images/2014/ef61_6.png)


Bahara olan özlemimizin tavan yaptığı bu günlerde, başka diyarlarda da değişik esintiler söz konusu elbette. [Örneğin C# 6.0 da](https://www.buraksenyurt.com/post/C-60e28093Yeni-Esintiler), örneğin Entity Framework’ de. Bakalım bu günkü esintiler bizi nerelere götürecek?

Entity Framework geliştirilmeye ve bünyesine yeni özellikler dahil edilmeye devam etmekte. Ancak son gelişmelerden bir tanesi oldukça önemli sanırım. O da artık Entity Framework’ ün tamamen harici bir NuGet paketi olarak kullanılacağı. Bir başka deyişle.Net Framework’ ün bir parçası olmaktan çıkartılmış ve Codeplex üzerinden yürür duruma gelmiş. Son bilgileri göre EF 6x verisyonları.Net 4.0 ve üstü için kullanılabiliyor. Ayrıca Visual Studio 2010 ve sonrası IDE’ ler de ele alındığını da belirtelim. (Standalone bir kütüphane olarak değerlendirebileceğimiz Entity Framework ile ilişkili son gelişmeleri [Codeplex üzerindeki adreslerinden](http://entityframework.codeplex.com/) takip etmekte yarar var. Hatta yanılmıyorsam projeye Contributor olarak katılmanız bile mümkün olabilir)

CUD Operasyonlarında Stored Procedure Kullanımı ve Interceptors

Tabi bu önemli değişiklik dışında dikkatimiz çeken başka yenilikler de var. Örneğin artık context üzerinden çalıştırılan sorgu komutlarının yakalanması mümkün. Aslında basit bir kesme/araya girme mekanizmasından bahsediyoruz. Ya da veri ekleme, güncelleme ve silme gibi CUD (CreateUpdateDelete) operasyonlarına ait komutlar çalıştırılırken (Executing) ve çalıştırıldıktan sonra (Executed) araya girebilme yeteneğinden. Bu noktada çok doğal olarak devreye bir arayüz (Interface) tipinin girdiğini ve çalışma zamanı Context’ ine bu arayüz sayesinde yeni bir davranış biçimi kazandırılabildiğini ifade edebiliriz.

> Yine de “Bu sistem sıfırdan tasarlansaydı, geliştiricilerin belirli kurallar çerçevesinde genişletmeler yapabilmeleri nasıl sağlanırdı?” sorusuna cevap bulmaya çalışarak kendinizi geliştirebilirsiniz.

Interceptor’ lar dışında kayda değer bir diğer özellikle de CUD operasyonlarının veritabanı şemasının oluşturulması noktasında Stored Procedure olarak da belirlenebilmesi. Buna göre bir Entity için söz konusu olan tipik Insert, Update ve Delete operasyonlarının bire Stored Procedure şeklinde ele alınabilmesi de mümkün hale gelmekte. (Burada iki yaklaşım olduğunu ifade edebiliriz. CUD operasyonunu var olan bir Stored Procedure ile eşleştirme veya sıfırdan üretilmesini sağlama)

Biz bu yazımızda söz konusu iki önemli özelliği basit bir Console uygulaması üzerinden yüzeysel olarak incelemeye çalışacağız.

Senaryo

Elbette basit bir senaryo üzerinden ilerlenmesinde yarar var. İçinde sadece Product isimli bir tip barındıran DbContext türevinde, CUD işlemlerinin Stored Procedure olarak ele alınmasını sağlayacağız. Bunun haricinde çalışma zamanında her hangibir Stored Procedure çağrımı söz konusu olursa, NLog aracından faydalanarak, yordama ait o anki parametre değerlerinin Console ekranına yazdırılmasını sağlayacağız.

Ön Hazırlıklar (Entity Framework ve NLog)

Senaryomuzda iki önemli NuGet paketine yer veriyor olacağız. Entity Framework ve NLog. Makalenin yazıldığı tarih itibariyle örnekte EF’ in Stable sürümlerinden 6.1 kullanılmıştır. NLog paketini ise Log yazma mekanizması için kullanacağız.

EF 6.1 Paketinin eklenmesi;

[![ef61_1](/assets/images/2014/ef61_1_thumb_1.png)](/assets/images/2014/ef61_1_1.png)

NLog Paketlerinin eklenmesi;

[![ef61_2](/assets/images/2014/ef61_2_thumb_1.png)](/assets/images/2014/ef61_2_1.png)

NLog paketlerinden NLog Configuration’ ı eklemeyi unutmayalım. Bu sayede NLog.config dosyası içerisinde intellisense özelliğinden yararlanabileceğiz.

Konfigurasyon İçerikleri

Normal şartlarda Entity Framework varsayılan olaral local veritabanını, Context adını baz alarak oluşturmakta ve kullanmaktadır. Ancak bilindiği üzere connectionStrings kımsında belirtilen bağlantı cümleciğinden yararlanılması da sağlanabilir. Örnekte makinede yer alan yerel SQL sunucusu kullanılmıştır. Buna göre ShopContext isimli DbContext türevi için,. ile belirtilen SQL sunucusu üzerinde YourShop isimli bir veritabanı üretilecektir. İşte App.config içeriği;

```xml
<?xml version="1.0" encoding="utf-8"?> 
<configuration> 
  <configSections> 
    <section name="entityFramework" type="System.Data.Entity.Internal.ConfigFile.EntityFrameworkSection, EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false" /> 
  </configSections> 
  <connectionStrings> 
    <add 
      name="ShopContext" 
      connectionString="Data Source=.;Initial Catalog=YourShop;Integrated Security=True;MultipleActiveResultSets=True" 
      providerName="System.Data.SqlClient" 
      /> 
  </connectionStrings> 
  <startup> 
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5.1" /> 
  </startup> 
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

NLog.config dosya içeriği ise aşağıdaki gibi oluşturulabilir. Burada target elementinde belirtilen nitelik değerlerine göre, log’ ların renkli formatta Console ekranına yazdırılması sağlanmaktadır. Diğer yandan layout niteliğinde belirtilen değere göre log mesajının başına uzun formatta bir tarih bilgisi eklenecektir ($ ile başlayan kelimelerin NLog çalışma zamanı için anlamlaştırılabilen birer komut olduğunu ifade edebiliriz) rules elementi içerisinde ise minimum Trace seviyesinde olmak üzere her tür bilginin log olarak yazılacağı belirtilmektedir.

```xml
<?xml version="1.0" encoding="utf-8" ?> 
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd" 
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"> 
  <targets> 
      <target xsi:type="ColoredConsole" name="c" 
            layout="${longdate} : ${message}" /> 
  </targets> 
  <rules> 
    <logger name="*" minlevel="Trace" writeTo="c" /> 
  </rules> 
</nlog>
```

Kod

Artık kod tarafının inşasına başlanabilir. Temel olarak aşağıdaki sınıf çizelgesinde (Class Diagram) yer alan şema kodlanmıştır.

[![ef61_5](/assets/images/2014/ef61_5_thumb.png)](/assets/images/2014/ef61_5.png)

Tipik olark Code-First yaklaşımına uygun olacak şekilde bir POCO tipi (Product) ve buna ait DbSet içeriğinin konuşlandırıldığı (Products özelliği) bir DbContext türevi söz konusudur. Dikkate değer kısımlar ise, EF için konfigurasyon ayarlarının kod tarafında da ele alınmasını sağlayan, DbConfiguration türevli ShopContextConfig ve IDbCommandInterceptor arayüzünü uygulamış olan StoredProcedureInterceptor sınıflarıdır.

StoredProcedureInterceptor sınıfı, IDbInterceptor arayüzünden gelen NonQueryExecuted, NonQueryExecuting, ReaderExecuted, ReaderExecuting, ScalarExecuted, ScalarExecuting metodlarını uygulamak durumundadır. Bu metodların tamamı o anki işleyişe kaynak olan DbCommand nesne örneği ile güncel Context içeriğini parametre olarak alır. Dolayısıyla, CUD operasyonlarının çalıştırıldığı veya tamamlandığı noktalarda ilgili nesne örneklerinin durumları ele alınabilir. Özetle söz konusu operasyonlar sırasında bu metodları kullanarak araya girilebileceğini ifade edebiliriz.

DbConfiguration türevli olan ShopConfig sınıfının bu senaryodaki temel görevi, Interceptor tiplerinin çalışma zamanına bildirimini yapmaktır. Çok doğal olarak IDbInterceptor arayüzünü uygulayan tiplerin bir şekilde çalışma zamanına bildirilmesi gerekmektedir.

Gelelim kodun detaylarına.

```csharp
using System.Data; 
using System.Data.Common; 
using System.Data.Entity; 
using System.Data.Entity.Infrastructure.Interception; 
using System.Linq; 
using NLog;

namespace EF6_NewFeatures 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            using (ShopContext bilbosShop = new ShopContext()) 
            { 
                bilbosShop.Products.Add(new Product { Title = "Platsik Tabak", Quantity = 10, UnitPrice = 1.05M }); 
                bilbosShop.Products.Add(new Product { Title = "Metal Kaşık", Quantity = 5, UnitPrice = 12 }); 
                bilbosShop.Products.Add(new Product { Title = "Tahta Bıçak", Quantity = 15, UnitPrice = 13.49M });

                bilbosShop.SaveChanges();

                var products = from p in bilbosShop.Products 
                                      where p.UnitPrice < 2 
                                      select p;

                foreach (var p in products) 
                { 
                    p.UnitPrice+= 0.5M; 
                }

                bilbosShop.SaveChanges();                
            }            
        } 
    }

    class Product 
    { 
        public int ProductID { get; set; } 
        public string Title { get; set; } 
        public decimal UnitPrice { get; set; } 
        public int Quantity { get; set; } 
    }

    class ShopContext 
       :DbContext 
    { 
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) 
        { 
            modelBuilder.Entity<Product>().MapToStoredProcedures(p => 
           { 
               p.Insert(s => s.HasName("sp_InsertProduct")); 
                p.Update(s => s.HasName("sp_UpdateProduct")); 
            });

            base.OnModelCreating(modelBuilder); 
        } 
    }

    class StoredProcedureInterceptor 
        : IDbCommandInterceptor 
    { 
        Logger logger = LogManager.GetCurrentClassLogger(); 
        public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext) 
        { 
            LogCurrentProcedureState(command, "Non Query Executed"); 
        }

        public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext) 
        { 
            LogCurrentProcedureState(command, "Non Query Executing"); 
        }

        public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext) 
        { 
            LogCurrentProcedureState(command, "Reader Executed"); 
        }

        public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext) 
        { 
            LogCurrentProcedureState(command,"Reader Executing"); 
        }

        public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext) 
        { 
        }

        public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext) 
        { 
        } 
        private void LogCurrentProcedureState(DbCommand command, string Location) 
        { 
            if (command.CommandType == CommandType.StoredProcedure) 
            { 
                logger.Trace(string.Format("{0}->{1}", command.CommandText, Location)); 
                foreach (DbParameter parameter in command.Parameters) 
                { 
                    logger.Info(string.Format("{0}{1}", parameter.ParameterName.PadLeft(20), parameter.Value.ToString().PadLeft(30))); 
               } 
            } 
        } 
    }

    class ShopContextConfig 
        :DbConfiguration 
    { 
        public ShopContextConfig() 
        { 
            AddInterceptor(new StoredProcedureInterceptor()); 
        } 
    } 
}
```

Detaylar

ShopContext sınıfı içerisinde ezilen OnModelCreating metodunda MapToStoredProcedures isimli fonksiyonun kullanıldığı görülmektedir. Product isimli Entity tipi için çalıştırılan metodun lambda operatörünün kullanıldığı içeriğinde ise Insert ve Update fonksiyonları için bire Stored Procedure adı verildiği görülebilir. Buna göre Product tipinin insert operasyonu için spInsertProduct, Update operasyonu için spUpdateProduct isimli yordamlar oluşturulacak ve kullanılacaktır. Dikkat edileceği üzere Delete operasyonu için bir bildirim de bulunulmamıştır. Ancak MapToStroredProcedures metodu otomatik olarak delete operasyonu için ProductDelete şeklinde bir yordam üretecektir.

Senaryoya göre, StoredProcedureInterceptor sınıfının NonQueryExecuting, NonQueryExecuted, ReaderExecuting ve ReaderExecuted isimli metodları değerlendirilmiştir. Metodlar LogCurrentProcedureState isimli bir iç fonksiyondan yararlanmaktadır. Söz konusu metod DbCommand örneğinin tipine göre hareket etmektedir. Eğer söz konusu komut bir Stored Procedure ise o anki parametre değerleri ekrana yazdırılacaktır.

ShopContextConfig sınıfı, DbConfiguration sınıfından türetilmiştir. Bu türetme otomatik olarak çalışma zamanınca algınır ve yapıcı metod içerisinde belirtilen bazı ayarların yürütülmesi sağlanır. Burada AddInterceptor isimli metoddan yararlanılmaktadır. İlgili fonksiyon tahmin edileceği üzere parametre olarak yeni bir StoredProcedureInterceptor örneğini alır. Yapıcı metod içerisinde ele alınabilecek farklı konfigurasyon ayarlamaları da söz konusudur. (Bunların neler olduğunu keşfetmek için this anahtar kelimesini yazıp noktaya basmanızı önerebilirim)

> DbConfiguration türevli tipler sayesinde, DbContext örneklerinin konfigurasyon bazındaki ayarları kod üzerinden yapılabilir.

Çalışma Zamanı Sonuçları

Uygulama çalıştırıldığında aşağıdaki ekran görüntüsünde yer alan sonuçların elde edildiği görülebilir. Aslında 3 adet Product örneği eklenmekte ve birim fiyatı 2 birimin altında olanlar için bir güncelleme yapılmaktadır. Tabi örnekteki amaç bu CUD işlemlerinin oluştuğu anlarda parametre değerlerinin yakalanmasıdır.

[![ef61_3](/assets/images/2014/ef61_3_thumb.png)](/assets/images/2014/ef61_3.png)

Görüldüğü üzere Main metodu içerisinde gerçekleştirilen yeni ürün ekleme ve güncelleme işlemlerine karşılık yürütülen Stored Procedure çağrıları yakalanmış ve o andaki parametre değerleri Console ekranına yazdırılmıştır.

Peki ya Veritabanı Durumu?

Veritabanına bakılırsa Products isimli bir tablonun ve CUD operasyonlarına karşılık ilgili stored procedure’ lerin oluşturulduğu gözlemlenir.

[![ef61_4](/assets/images/2014/ef61_4_thumb.png)](/assets/images/2014/ef61_4.png)

Yordamlara ait Script’ ler ise aşağıdaki gibidir.

Product_Delete isimli yordam için;

```text
USE [YourShop] 
GO 
/****** Object:  StoredProcedure [dbo].[Product_Delete]    Script Date: 04/09/2014 11:09:01 ******/ 
SET ANSI_NULLS ON 
GO 
SET QUOTED_IDENTIFIER ON 
GO 
ALTER PROCEDURE [dbo].[Product_Delete] 
    @ProductID [int] 
AS 
BEGIN 
    DELETE [dbo].[Products] 
    WHERE ([ProductID] = @ProductID) 
END
```

Delete yordamında beklendiği gibi ProductID’ nin parametre olarak değerlendirildiği bir script söz konusudur. Nitekim Product tipinin ProductID özelliği tablo tarafında Primary Key alandır.

sp_InsertProduct isimli yordam için;

```text
USE [YourShop] 
GO 
/****** Object:  StoredProcedure [dbo].[sp_InsertProduct]    Script Date: 04/09/2014 11:08:43 ******/ 
SET ANSI_NULLS ON 
GO 
SET QUOTED_IDENTIFIER ON 
GO 
ALTER PROCEDURE [dbo].[sp_InsertProduct] 
    @Title [nvarchar](max), 
    @UnitPrice [decimal](18, 2), 
    @Quantity [int] 
AS 
BEGIN 
    INSERT [dbo].[Products]([Title], [UnitPrice], [Quantity]) 
    VALUES (@Title, @UnitPrice, @Quantity) 
    
    DECLARE @ProductID int 
   SELECT @ProductID = [ProductID] 
    FROM [dbo].[Products] 
    WHERE @@ROWCOUNT > 0 AND [ProductID] = scope_identity() 
    
    SELECT t0.[ProductID] 
    FROM [dbo].[Products] AS t0 
    WHERE @@ROWCOUNT > 0 AND t0.[ProductID] = @ProductID 
END
```

Insert operasyonunda yeni ürünün eklenmesini takiben, ProductID için oluşturulan otomatik Identity değerinin geri döndürüldüğü gözlemlenmektedir.

sp_UpdateProduct için;

```text
USE [YourShop] 
GO 
/****** Object:  StoredProcedure [dbo].[sp_UpdateProduct]    Script Date: 04/09/2014 11:08:23 ******/ 
SET ANSI_NULLS ON 
GO 
SET QUOTED_IDENTIFIER ON 
GO 
ALTER PROCEDURE [dbo].[sp_UpdateProduct] 
   @ProductID [int], 
    @Title [nvarchar](max), 
   @UnitPrice [decimal](18, 2), 
    @Quantity [int] 
AS 
BEGIN 
    UPDATE [dbo].[Products] 
    SET [Title] = @Title, [UnitPrice] = @UnitPrice, [Quantity] = @Quantity 
    WHERE ([ProductID] = @ProductID) 
END
```

Update ifadesinde aynen Delete ifadesinde olduğu gibi ProductID isimli Primary Key alandan yararlanılmaktadır.

> Title isimli alanın kullanıldığı parametrelerin nvarchar (max) tipinden tanımlandığı gözden kaçmamalıdır. Bu normaldir nitekim Title alanı Products tablosu içinde bu şekilde oluşturulmuştur. Halbuki bu alanın örneğin 50 karakter uzunluğunda olması daha uygun olabilir. Code-First yaklaşımını kullandığımız bu örnek senaryoda bu kriteri nasıl sağlarsınız? İşte size güzel bir antrenman sorusu.

Sonuç

Dikkat edileceği üzere CUD operasyonlarının icrası noktasında araya girerek bir takım iş kurallarının işletilmesi mümkündür. Hatta örnek senaryoda görüldüğü gibi Log’ lamanın daha kural bazlı işlenmesi için bu kesmeler ideal olabilir. Diğer yandan CUD operasyonlarının Stored Procedure olarak inşa edilebilmesi, ilgili fonksiyonelliklerin veritabanı tarafında birer yordam nesnesi olarak değerlendirilebilmesi anlamına gelmektedir. Ayrıca parametre yapıları doğru ise DB tarafında var olan SP’ lerin map edilmesi de mümkündür. Bu durumda Db tarafındaki SP’ ler içerisinde yer alan ve DB’ ye özgü bir takım ifadelerin bu basit CUD operasyonları sırasında değerlendirilebilmesi de söz konusudur.

Entity Framework tarafında geliştirmeler devam etmektedir. Codeplex üzerinden izlemeye devam ediyor olacağız. Bakalım baharın geldiği şu günlerde daha ne gibi esintilerle karşılacaşacağız. Böylece geldik bir makalemizin daha sonunda. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.