---
title: "Koleksiyon Bazlı WCF Rest Servisleri"
pubDate: 2009-04-22 08:07:00
categories:
  - WCF
tags:
  - WCF
  - C#
  - .NET
  - ASP.NET
  - XML
  - REST
  - JSON
  - HTTP
  - JavaScript
  - Caching
  - Serialization
  - Generics
  - Visual Studio
---

# Koleksiyon Bazlı WCF Rest Servisleri
Merhaba Arkadaşlar,

Bildiğiniz üzere bir süredir WCF servislerinin REST modeline göre geliştirilmesi ile ilgili bilgilerimi ve öğrendiklerimi sizlerle paylaşmaktayım. Bu nedenle dün gece yaşadığım bir macerayıda aktararak başıma gelenleri sizlerle paylaşmak istiyorum. Bir süre önce [WCF Rest Starter Kit'](http://aspnet.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=24644)i incelemeye başlamış ve bu konuyla ilgili olaraktan bir görsel video yayınlamıştım.([NedirTv? bağlantısı](http://www.nedirtv.com/video/WCF-REST-Bolum-2---Readonly-Collection-Service.aspx))

Bu görsel derste, veri kaynağı olarak içerdikleri koleksiyonları yanlız okuma (Read-only) amaçlı ele alan REST bazlı WCF servislerinin, WCF Rest Start Kit Preview 1 sürümü ile nasıl geliştirilebileceğini incelemeye çalışmıştım. Tabi aradan uzun zaman geçti ve WCF Rest Start Kit Preview 2 sürümü yayınlandı. Ayrıca görsel derste insert, update ve delete işlemlerini ele almamıştım. Bende hazır fırsat varken, bu tip WCF servislerinde Insert, Update, Delete işlemlerini nasıl yapabiliriz konusunu araştırmaya başladım. Starter Kit ile birlikte gelen Lab'lar içerisinde (3ncü alıştırmada) bu konu oldukça kolay anlaşılır bir şekilde ele alınmaktadır. Benim size aktaracaklarım daha çok başıma nelerin geldiği.

![Laughing](images/smiley-laughing.gif)

İlk olarak REST Collection Service kavramını biraz açmamızda yarar var. Starter Kit ile birlikte Visual Studio 2008 ortamına bir proje şablonu olarak gelen bu yapı, REST modeline göre veri kümelerinin, servis tarafında koleksiyon bazlı olaraktan ele alınmasını otomatikleştirmektedir. Veriler istemci tarafına XML formatı dışında [JSON (JavaScript Object Notation)](http://www.json.org/json-tr.html) standartlarına görede yayımlanabilir. Ayrıca daha önceki yazılarımızda değindiğimiz WebGet niteliği ile UriTemplate'ler oluşturulmasına gerek yoktur. Çünkü buda hazır olarak gelmektedir. Bunlara ek olarak, HTTP POST, GET, DELETE ve PUT metodlarına cevap verecek şekilde bir çalışma zamanı alt yapısına sahiptir ki bu sayede Select dışında Insert,Update,Delete gibi işlemleride yapabiliriz. Tabiki request'leri doğru bir şekilde gönderebildiğimiz takdirde. Şablonu Visual Studio 2008 ortamında kullanmak son derece kolaydır.

![blg5_1.gif](images/blg5_1.gif)

Aslında Lab içerisindeki adımlarda ilerlerken ilk dikkat çeken noktalardan birisi, Service tipinin CollectionServiceBase abstract sınıfından türemesi ve ICollectionService arayüzünü (Interface) uygulamasıydı. TItem tipi koleksiyon içerisinde kullanılacak veri tipini işaret etmektedir. CollectionServiceBase abstract sınıfı içerisinde, Servis tipi tarafından uygulanması gereken bazı abstract metodlar yer almaktadır (OnGetItems, OnGetItem, OnAddItem, OnDeleteItem, OnUpdateItem). Diğer taraftan arayüzün içerisindede az önce belirttiğimiz metodların Json ve Xml formatları için olan tanımlamaları yer almaktadır. Aslında bir servis geliştirilirken bilindiği üzere Servis Sözleşmesi (Service Contract) ve onu uygulayan asıl servis sınıfı ele alınmaktadır. REST Collection Service şablonunda, sözleşme görevini üstlenen arayüz ICollectionService dır. Burada tanımlanan operasyonların sahip olduğu WebHelp, WebGet, WebInvoke gibi niteliklerin içeriklerinde istenirse oynamalar yapılabilir. Ancak şablon, bu niteliklere varsayılan değerlerini koyarak, hazır bir uygulanış biçiminide sunmaktadır.

Antrenmanı Lab üzerinden adım adım yaparken, asıl veri kaynağı olarak neyi kullanacağımı düşünüyordum. Her zamanki gibi kolaya kaçıp tembellik yaptığımdan, Northwind veritabanında yer alan Products tablosunu ve CUD işlemleri içinde bir kaç SP ile Enterprise Library kullanmaya karar vermiştim. Genellikle profesyonel çaptaki projelerde basit CRUD işlemlerini Stored Procedure'ler içerisine almak çok sık yapılan bir şey değildir. Ancak amacım sadece bir veri kümesi kullanmak ve CUD (CreateUpdateDelete) işlemlerini REST modeli üzerinden test etmek olduğu için bu durumu şimdilik görmezden geldim.

> Tabiki istenirse farklı veri kaynaklarıda koleksiyon bazlı olacak şekilde REST modeline göre servisleştirilebilir. Örneğin XML tabanlı bir veri kaynağı ele alınabilir yada program alanında tutulan bir koleksiyon. Nitekim servisin kullandığı Dictionary koleksiyonu içerisinde tutulan nesneler aslında veriyi sembolize eden birer entity olarak düşünülmelidir.

Neyse çok fazla dağıtmadan konuyu devam edeyim. Stored Procedure'leri temel CUD işlemlerini gerçekleştirmek üzere aşağıdaki gibi tasarladım.

Ekleme işlemi

```text
CREATE PROCEDURE InsertProduct
            @ProductName nvarchar(40)
           ,@SupplierID int
           ,@CategoryID int
           ,@QuantityPerUnit nvarchar(20)
           ,@UnitPrice money
           ,@UnitsInStock smallint
           ,@UnitsOnOrder smallint
           ,@ReorderLevel smallint
           ,@Discontinued bit
AS
INSERT INTO [Northwind].[dbo].[Products]
           (ProductName
           ,SupplierID
           ,CategoryID
           ,QuantityPerUnit
           ,UnitPrice
           ,UnitsInStock
           ,UnitsOnOrder
           ,ReorderLevel
           ,Discontinued
           )
     VALUES
           (
           @ProductName
           ,@SupplierID
           ,@CategoryID
           ,@QuantityPerUnit
           ,@UnitPrice
           ,@UnitsInStock
           ,@UnitsOnOrder
           ,@ReorderLevel
           ,@Discontinued
          ) 
Select SCOPE_IDENTITY()
```

Güncelleme işlemi

```text
CREATE PROCEDURE UpdateProduct
    @ProductName nvarchar(40)
           ,@SupplierID int
           ,@CategoryID int
           ,@QuantityPerUnit nvarchar(20)
           ,@UnitPrice money
           ,@UnitsInStock smallint
           ,@UnitsOnOrder smallint
           ,@ReorderLevel smallint
           ,@Discontinued bit
           ,@ProductID int
AS

Update [Northwind].[dbo].[Products]
Set
           ProductName=@ProductName
           ,SupplierID=@SupplierID
           ,CategoryID=@CategoryID
           ,QuantityPerUnit=@QuantityPerUnit
           ,UnitPrice=@UnitPrice
           ,UnitsInStock=@UnitsInStock
           ,UnitsOnOrder=@UnitsOnOrder
           ,ReorderLevel=@ReorderLevel
           ,Discontinued=@Discontinued
Where
    ProductID=@ProductID
```

Silme işlemi

```text
CREATE PROCEDURE DeleteProduct
(
 @ProductID int 
)
AS
Delete From Products Where ProductID=@ProductID
RETURN
```

Daha sonrada Servis sınıfını aşağıdaki gibi yeniledim. Yeniledim diyorum, nitekim proje şablonu zaten içerisinde hazır olarak bir uyarlama gerçekleştirmekte ve SampleItem isimli bir tipi koleksiyon içerisinde kullanmaktadır. Ayrıca OnGetItems, OnGetItem, OnAddItem, OnUpdateItem ve OnDeleteItem metodları içinde hazır kodlamalar yer almaktadır. (Bu tip şablonların Visual Studio 2010 içerisinde dahada otomatikleştirilmesi söz konusu olabilir.)

```csharp
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using Microsoft.ServiceModel.Web;
using System.ServiceModel.Activation;
using System.Net;
using Microsoft.ServiceModel.Web.SpecializedServices;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;

[assembly: ContractNamespace("", ClrNamespace = "NorthwindV2")]

namespace NorthwindV2
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Service 
        : CollectionServiceBase<Product>, ICollectionService<Product>
    {
        // products isimli Dictionary koleksiyonunda Product nesne örnekleri value, ProductID değerleri ise key olarak tutulmakta.
        Dictionary<string, Product> products = new Dictionary<string, Product>();
        // Enterprise Library' den yararlanarak Database nesnesi üretiliyor.
        Database db = DatabaseFactory.CreateDatabase("NorthConStr");

        // Bu metod ile istemci tarafından gelecek talep sonrasında tüm ürünlerin istenilen formatta gösterilmesi sağlanmaktadır.
        // CategoryID değeri null olmayan satırlar çekilir ve IDataReader üzerinden Product nesnesi şeklinde oluşturulan örnekler, products koleksiyonuna eklenerek geriye döndürülür.
        // http://localhost:1000/Service.svc talebi sonrası bu metod devreye girer.
        protected override IEnumerable<KeyValuePair<string, Product>> OnGetItems()
        {            
            IDataReader reader = db.ExecuteReader(CommandType.Text, "Select * From Products where CategoryID is not null");

            while (reader.Read())
            {
                products.Add(reader["ProductID"].ToString(),
                    new Product
                    {
                        ProductID=Convert.ToInt32(reader["ProductID"]), 
                        CategoryID=Convert.ToInt32(reader["CategoryID"]),
                          Discontinued=Convert.ToBoolean(reader["Discontinued"]),
                           ProductName=reader["ProductName"].ToString(),
                            QuantityPerUnit=reader["QuantityPerUnit"].ToString(),
                             ReorderLevel=Convert.ToInt16(reader["ReorderLevel"]),
                              SupplierID=Convert.ToInt32(reader["SupplierID"]),
                               UnitPrice=Convert.ToDecimal(reader["UnitPrice"]),
                                UnitsInStock=Convert.ToInt16(reader["UnitsInStock"]),
                                 UnitsOnOrder=Convert.ToInt16(reader["UnitsOnOrder"])                           
                    }
                    );
            }
            return this.products;
        }

        // Id değeri üzerinden bir Product' ın elde edilip geriye döndürülmesi için kullanılır. Özellikle Update metodunda dahili olaraktan kullanılmaktadır.
        // Yani http://localhost:1000/Service.svc/4 gibi bir talep sonrası bu metod devreye girmektedir.
        protected override Product OnGetItem(string id)
        {
            int productId;
            // int tipinden olmayan bir ProductId değeri ise
            if (!Int32.TryParse(id, out productId))
            {
                // Exception fırlatılır
                throw new WebProtocolException(HttpStatusCode.BadRequest);
            }
            return this.products[id];
        }

        // Yeni bir Product tipinin eklenmesi için kullanılan bu metoddan geriye, yeni oluşturulan satırın ProductID değeri döndürülür(eğer işlemler başarılı ise)
        protected override Product OnAddItem(Product initialValue, out string id)
        {
            try
            {
                id = db.ExecuteScalar("InsertProduct", initialValue.ProductName, initialValue.SupplierID, initialValue.CategoryID, initialValue.QuantityPerUnit, initialValue.UnitPrice, initialValue.UnitsInStock, initialValue.UnitsOnOrder, initialValue.ReorderLevel, initialValue.Discontinued).ToString();
                initialValue.ProductID = Convert.ToInt32(id);
                this.products.Add(id, initialValue);
            }
            catch(Exception excp)
            {
                throw new WebException(excp.Message.ToUpper(), WebExceptionStatus.RequestCanceled);
            }            
            return initialValue;
        }

        // Bir Product' ın güncelleştirilmesi işlemi sırasında kullanılan metoddur. Metod başarılı bir şekilde güncelleştirme işlemini yaparsa geriye Product tipinin son hali döndürülür.
       protected override Product OnUpdateItem(string id, Product newValue)
       {       
            int result = 0;
            try
            {
                result = db.ExecuteNonQuery("UpdateProduct", newValue.ProductName, newValue.SupplierID, newValue.CategoryID, newValue.QuantityPerUnit, newValue.UnitPrice, newValue.UnitsInStock, newValue.UnitsOnOrder, newValue.ReorderLevel, newValue.Discontinued, Convert.ToInt32(id));
            }
            catch (Exception excp)
            {
                throw new WebException(excp.Message, WebExceptionStatus.RequestCanceled);
            }

            if (oldValue == null) // Eğer veri kaynağında güncelleştirilecek bir Product nesnesi yoksa istisna mesajı verilir.
            {
                throw new WebProtocolException(HttpStatusCode.NotFound);
            }
           
            int result=db.ExecuteNonQuery("UpdateProduct", newValue.ProductName, newValue.SupplierID, newValue.CategoryID, newValue.QuantityPerUnit, newValue.UnitPrice, newValue.UnitsInStock, newValue.UnitsOnOrder, newValue.ReorderLevel, newValue.Discontinued, newValue.ProductID);

            if (result == 1) // ProductID değerleri Auto Identity tipinden olduklarında güncelleştirilen kayıt sayısı 1 ise
            {                
                // products koleksiyonundaki değer güncellenir
                this.products[id] = newValue;
                // güncellenen değerlere sahip Product tipi geriye döndürülür.
                return newValue;
            }
            else
                return null; // Aksi durumda null döndürülür
        }

        // Bir Product' ın silinmesi için kullanılan metoddur. Silme işleminin başarılı olması halinde true değeri döndürülür.
        protected override bool OnDeleteItem(string id)
        {
            // İlk olarak silinmek istenen id değerine sahip Product tipi çekilir
            Product item = OnGetItem(id);
            // Eğer ilgili Product null ise false değer döndürülür
            if (item == null) 
                return false;
            // Eğer var ise Products tablosundan silme işlemi yapılır
            int result=db.ExecuteNonQuery("DeleteProduct", id);
            // Eğer 1 satır silinebildiyse,
            if (result == 1)
            {
                // products koleksiyonundan da çıkartma işlemi yapılır.
                this.products.Remove(id);
                return true;
            }
            else
                return false;
        }
    }

    // Koleksiyon içerisinde kullanılan Product sınıfı.
    public class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int SupplierID { get; set; }
        public int CategoryID { get; set; }
        public string QuantityPerUnit { get; set; }
        public decimal UnitPrice { get; set; }
        public short UnitsInStock { get; set; }
        public short UnitsOnOrder { get; set; }
        public short ReorderLevel { get; set; }
        public bool Discontinued { get; set; }
    }
}
```

Artık herşey test için hazırdı. İlk etapta 1000 numaralı port üzerinden hizmet verecek olan servisi F5 ile çalıştırdım. Lab içerisinde söz konusu REST servisinin testi sırasında Post, Put, Delete talepleri için bir istemci uygulama yerine Fiddler aracı kullanılmaktaydı. Yani yeni bir ürün eklemek, silmek veya güncellemek istediğimde istemci tarafından gönderilecek olan HTTP paketini Fiddler aracı yardımıyla hazırlayıp gönderilmesi öneriliyordu. Bu sadece test ve içerik analizi için bir öneriydi. Ancak daha o adımlara geçmeden önce ilk hata mesajım ile karşılaştım. İlk etapta sorun yok gibi görünüyordu. Ancak aynı servisi ikinci bir tarayıcı penceresinden talep ettiğimde aşağıdaki görüntü ile karşılaştım.

![blg5_2.gif](images/blg5_2.gif)

Hata nerededir diye araştırırken aslında her talepte OnGetItems metodunun çağırıldığını ve bu sebeple koleksiyona veri ekleme işleminden önce aslında temizlenmesi gerektiğini farkettim. Dolayısıyla kodu aşağıdaki halde yenilemek sorunu çözdü.

```csharp
protected override IEnumerable<KeyValuePair<string, Product>> OnGetItems()
{            
  products.Clear(); // Temizlemediğimizde ikinci bir request için hata mesajı alınır.            
  IDataReader reader = db.ExecuteReader(CommandType.Text, "Select * From Products where CategoryID is not null");
```

> Tabii burada servisin çok sık değişmeyen bir koleksiyonu yayınlaması durumunda, her talep için veritabanından bir Select sorgusu ile veri çekmesi yerine, performansı arttırmak için belki önbellekleme (Caching) sistemi kullanılabilir. Nitekim servis, sonuç itibariyle Asp.Net Host ortamında sunulmaktadır. Bu nedenle Cache mimarisini ele alabilir. Hatta SqlCacheDependency kullanılarak Cache içeriğinin gerçekten tabloda değişiklik olduğu durumlarda ele alınmasıda sağlanabilir. Bu durumu ilerleyen yazılarımızda ele almayı planlıyorum.

Son sorunu çözdükten sonra hemen yeni bir satır Product eklemeye karar verdim. Aynen Lab'da belirtildiği gibi, paketi manuel olarak Fiddler aracı ile hazırlayıp servise gönderim.

![blg5_3.gif](images/blg5_3.gif)

Burada talep metodunun POST olarak seçildiğine, Content tipinin text/xml olarak belirtildiğine dikkat etmek lazım. Diğer tarafında RequestBody kısmında manuel olarak yazdığımız XML içeriğinde ProductID değeri yazmadığımı da belirtelim. Nitekim, ProductID otomatik artan ve insert sorgusuna dahil edilmeyen bir alandır. Ancak ne varki Execute işleminden sonra servis tarafından 307 kodlu bir cevap gelmiştir (Temporary Redirect).

![Frown](images/smiley-frown.gif)

Oysaki 201 cevabının gelmesi gerekirdi.

![blg5_4.gif](images/blg5_4.gif)

Bu hatayla uzun bir süre cebelleştikten sonra, sorunun adres kısmını yanlış yazmamdan kaynaklandığını tespit ettim. Yani adresin http://buraksenyurt:1000/Service.svc adresinin http://buraksenyurt:1000/Service.svc/ olarak yazılması gerekiyormuş. Tamamen benim hatam...Adresi bu şekilde düzelttikten sonra insert işleminin gerçekleştirildiğini ve hem koleksiyonda hemde Products tablosunda yeni Product tipi için gerekli eklemelerin yapıldığını görebildim.

Fiddler görüntüsü

![blg5_5.gif](images/blg5_5.gif)

Tarayıcı görüntüsü

![blg5_6.gif](images/blg5_6.gif)

SQL Tarafı

![blg5_7.gif](images/blg5_7.gif)

Artık güncelleme ve silme işlemlerini tespit edebilirdim. Güncelleştirme işlemi sırasında dikkat etmem gereken noktalardan ilki, HTTP protokolünün Put metodunu kullanmam gerektiğiydi. Ancak ilk denemede yine patlayınca aslında güncelleştirmek istediğim satırın ProductID değerini Request Body içerisindeki XML kısmında değil, URL kısmında belirtmem gerektiğini farkettim.

![Embarassed](images/smiley-embarassed.gif)

Buna göre Fiddler aracı yardımıyla aşağıdaki talebi gönderdiğimde,

![blg5_8.gif](images/blg5_8.gif)

servis tarafından 200-Ok cevabını alabildiğimi gördüm.

![blg5_9.gif](images/blg5_9.gif)

![Laughing](images/smiley-laughing.gif)

İşte mutluluğun resmi.

Resmi tamamlamak için son olarak delete işlemini test etmem gerekiyordu. Bu sefer HTTP protokolünü kullanarak göndereceğim talepte, Delete metodunu seçmem gerektiğini biliyordum. Ayrıca silmek istediğim Product satırının ProductID değerinide bir önceki Update işlemine göre URL satırından göndermem gerektiğinin farkındaydım. Hatta Request'in body kısmında herhangibir bilgi olmaması gerektiğinide tahmin edebilmiştim. Dolayısıyla tek seferde çalıştırabileceğim düşüncesindeydim.

![blg5_10.gif](images/blg5_10.gif)

İşte bu kadar. Bir maceramızın daha sonuna geldik. REST bazlı WCF servislerinin kullanımı ile ilişkili çalışmalarıma ve araştırmalarıma devam ederken bunları sizlerlede paylaşıyor olacağım. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

Örnek Uygulama; [NorthwindV2.rar (322,50 kb)](assets/NorthwindV2.rar)

Sp ler için script dosyası: [script.sql (4,65 kb)](assets/script.sql)
