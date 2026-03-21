---
layout: post
title: "WCF Rest Modelinde UriTemplate Kullanımı"
date: 2009-04-20 19:19:00 +0300
categories:
  - wcf
tags:
  - windows-communication-foundation
  - rest-api
---
SOAP (Simple Object Access Protocol) bazlı WCF servislerininin REST (REpresentational State Transfer) modeline taşınmasını ele aldığımız bir önceki [yazımızda](https://www.buraksenyurt.com/post/Soap-Bazl%C4%B1-WCF-Servislerini-REST-Modeline-Tas%C4%B1mak)varsayılan URL şablonu kullanılmıştır. Varsayılan URL şablonu, WebGet niteliğinde herhangibir başka desen belirtilmediğinde devreye girmektedir. Kabaca aşağıdaki dizime benzer bir yapıdadır.

```text
http://servisAdresi/servisAdi.svc/OperasyonAdi?parametre1=parametreDegeri¶metre2=parametreDegeri
```

Bu standart şablona göre, WebGet niteliği (Attribute) ile imzalanmış olan metod, operasyonAdi kısmına gelmekte, sonrasında ise eğer varsa metod parametreleri ve değerleri yer almaktadır. Parametre adları metodda kullanılanlar ile aynı olmalıdır. Söz gelimi ürünlerin belirli bir kategori altında olanlarını getirmek istediğimizde, REST bazlı bir servise gönderilecek talepler için aşağıdakine benzer bir adresleme kullanılır.

```text
http://servisAdresi/servisAdi.svc/GetProducts?CategoryID=1
```

Oysaki bunun yerine,

```xml
http://servisAdresi/servisAdi.svc/Products/1
```

veya

```text
http://servisAdresi/servisAdi.svc/Products(1)
```

ve hatta

```text
http://servisAdresi/Products/Kitap
```

gibi adreslemelerde bulunmak daha anlaşılırdır.

Nitekim günümüz web teknolojilerinde, URL üzerinde daha anlaşılır bilgilerin yazılması tercih edilmektedir (Örneğin arama motorları bu kriterlere çok fazla dikkat eder) ASP.Net tarafında URL Rewriting teknikleri ile ele alınan, MVC (ModelViewController) deseninde önemli bir yere sahip olan, ADO.Net Data Service'lerde varsayılan olarak kullanılan URL şablonları, istenirse WebGet niteliğinin UriTemplate özelliği ile REST modeline taşınmış WCF servislerinde de uygulanabilir. Bu yazımızda bu konsepti ele almaya çalışıyor olacağız. Ne kadar basit olduğunu biraz sonra sizlerde göreceksiniz.

Her zamanki gibi elimizde REST modelin taşınmış hazır bir WCF Servis uygulaması olduğunu düşünelim. Uygulamada yer alan basit bir operasyon, Northwind veritabanına bağlanarak Products tablosundan, kullanıcıdan gelen categoryId değerine sahip olanları, Product sınıfı tipinden örnekleri taşıyan generic bir List koleksiyonunda geriye döndürmektedir. Özellikle veri çekilmesi için basit bir Stored Procedure kullanılmakta ve kod tarafında bu işlem [Enterprise Library 4.1](http://msdn.microsoft.com/en-us/library/dd203099.aspx) sürümü ile gerçekleştirilmektedir.

Sp içeriği

```text
CREATE PROCEDURE dbo.GetProductsByCategory
(
 @CategoryID int
)
 
AS
 Select 
  ProductID
  ,ProductName
  ,SupplierID
  ,CategoryID
  ,QuantityPerUnit
  ,UnitPrice
  ,UnitsInStock
  ,UnitsOnOrder
  ,ReorderLevel
  ,Discontinued
  From Products Where CategoryID=@CategoryID
 
 RETURN
```

Söz konusu uygulamanın servis sözleşmesi (Service Contract) ve uygulayıcı içerikleri ise aşağıdaki gibidir.

Product isimli veri tipimiz

```csharp
using System;

namespace Northwind
{
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

Servis sözleşmesi

```csharp
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Collections.Generic;

namespace Northwind
{    
    [ServiceContract]
    public interface IProducts
    {
        [OperationContract]        
           [WebGet]
        List<Product> GetProducts(string categoryId);
    }
}
```

Uygulayıcı tip

```csharp
using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Collections.Generic;

namespace Northwind
{    
    public class Products 
        : IProducts
    {
        #region IProducts Members

        public List<Product> GetProducts(string categoryId)
        {
            List<Product> products = new List<Product>();
            Database db = DatabaseFactory.CreateDatabase("NorthConStr");
            IDataReader reader=db.ExecuteReader("GetProductsByCategory",Convert.ToInt32(categoryId));
            while (reader.Read())
            {
                products.Add(
                    new Product
                    {
                         CategoryID=Convert.ToInt32(reader["CategoryID"]),
                          ProductID=Convert.ToInt32(reader["ProductID"]),
                           ProductName=reader["ProductName"].ToString(),
                            QuantityPerUnit=reader["QuantityPerUnit"].ToString(),
                             ReorderLevel=Convert.ToInt16(reader["ReorderLevel"]),
                             SupplierID=Convert.ToInt32(reader["SupplierID"]),
                              UnitPrice=Convert.ToDecimal(reader["UnitPrice"]),
                               UnitsInStock=Convert.ToInt16(reader["UnitsInStock"]),
                                UnitsOnOrder=Convert.ToInt16(reader["UnitsOnOrder"]),
                                 Discontinued=Convert.ToBoolean(reader["Discontinued"])
                    }
                    );
            }
            return products;
        }

        #endregion
    }
}
```

Bu arada unutmamamız gereken noktalardan biriside, Enterprise Library için Microsoft.Practices.EnterpriseLibrary.Common, Microsoft.Practices.EnterpriseLibrary.Data ile WebServiceHostFactory tipi için (Servisin çalışma zamanında REST modeline göre ele alınması için gereken ve Markup kısmındaki ServiceHost direktifi içerisinde Factory niteliği ile belirtilen tiptir) System.ServiceModel.Web assembly'larının referans edilmesidir.

![blg4_1.gif](/assets/images/2009/blg4_1.gif)

Şimdiii.....Bu haliyle Product.svc talep edildiğinde her hangibir kategoriye ait ürünleri elde edebilmek için aşağıdaki ekran görüntüsünde yer alan URL diziminin kullanılması gerekmektedir.

![blg4_5.gif](/assets/images/2009/blg4_5.gif)

Burada URL için varsayılan davranışı görmekteyiz. Sırada ilk hamlemiz var. Bu hamlemizde WebGet niteliğini aşağıdaki gibi değiştiriyoruz.

```csharp
[WebGet(UriTemplate = "Products/{categoryId=1}")]
List<Product> GetProducts(string categoryId);
```

Buna göre categoryId için varsayılan olarak 1 değerini belirtmiş oluyoruz. Yani, kullanıcı talebinde eğer categoryId değeri girilmesse, varsayılan olarak 1 olanları getirecektir.

![blg4_3.gif](/assets/images/2009/blg4_3.gif)

Elbette artık Products/2 veya Products/3 gibi kullanımlarda mümkün olacaktır. Hatta WebGet niteliğindeki UriTemplate kısmını,

```csharp
[WebGet(UriTemplate = "Products/({categoryId=1})")]
List<Product> GetProducts(string categoryId);
```

şeklinde değiştirirsek, aşağıdaki ekran görüntüsünde yer alan sonuçları elde edebiliriz.

![blg4_4.gif](/assets/images/2009/blg4_4.gif)

Ne kadar basit öyle değil mi?

![Laughing](/assets/images/2009/smiley-laughing.gif)

Hatta istersek IIS 7.0 üzerindeki ayarları kullanarak (bir sonrak yazımda ele almaya çalışacağım) svc uzantılı kısımlardan kurtulabilir ve URL kısmını dahada özelleştirebiliriz.

Böylece geldik bir blog yazımın daha sonuna. Bu yazımızda WebGet niteliğinin UriTemplate özelliğini kullanarak, istemciden servis tarafına gelecek olan URL bilgilerinin nasıl özelleştirilebileceğini, WCF Resf modeli için değerlendirmeye çalıştık. Tekrardan görüşünceye dek hepinize mutlu günler dilerim...

[Northwind.rar (234,38 kb)](/assets/files/2009/Northwind.rar)