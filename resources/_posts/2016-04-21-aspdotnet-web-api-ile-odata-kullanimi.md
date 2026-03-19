---
layout: post
title: "Asp.Net Web API ile OData Kullanımı"
date: 2016-04-21 09:00:00 +0300
description: "İşlerin epeyce hafiflediği bir haftaydı diyebilirim. Dolayısıyla kırda parkta bayırda oturup dinlenmek için epeyce vaktim vardı. Ya da bir şeyler araştırmayı da tercih edebilirdim ki ben de öyle yaptım. Uzun zamandır Asp.Net Web API tarafında bir şeyler yapmıyordum. Araştırmalarım sırasında OData'nın Web API tarafındaki kullanımına denk geldim. Her zaman ki gibi konuyu olabildiğince basit bir halde öğrenmenin iyi olacağını düşündüm. Sonunda konuyu kaleme almayı başardım. Haydi başlayalım."
categories:
  - aspnet-web-api
tags:
  - aspnet-web-api
  - csharp
  - dotnet
  - aspnet
  - entity-framework
  - linq
  - rest
  - json
  - web-api
  - http
  - generics
  - visual-studio
---
# Asp.Net Web API ile OData Kullanımı
![ODataWebAPI_G.gif](/assets/images/2016/ODataWebAPI_G.gif)

Merhaba Arkadaşlar,

İşlerin epeyce hafiflediği bir haftaydı diyebilirim. Dolayısıyla kırda parkta bayırda oturup dinlenmek için epeyce vaktim vardı. Ya da bir şeyler araştırmayı da tercih edebilirdim ki ben de öyle yaptım. Uzun zamandır Asp.Net Web API tarafında bir şeyler yapmıyordum. Araştırmalarım sırasında OData'nın Web API tarafındaki kullanımına denk geldim. Her zaman ki gibi konuyu olabildiğince basit bir halde öğrenmenin iyi olacağını düşündüm. Sonunda konuyu kaleme almayı başardım. Haydi başlayalım.

OData (Open Data Protocol) ile veri kaynaklarına HTTP üzerinden sorgu atabilmek ve bunu REST stilinde gerçekleştirmek mümkündür. OData standartları sayesinde veri üzerinde filtreleme, belirli alanlarını çekme (Projection), ana içeriklerden detay içerikleri genişletme (Expand), gruplamalı hesaplamalar yapma (Aggregations) ve benzeri SQL'den aşina olduğumuz belli başlı fonksiyonellikleri servis odaklı kullanabiliriz.

Bu standardı dilersek Asp.Net Web API tabanlı servisler ile de ele alabiliriz. Tek yapmamız gereken Microsoft.AspNet.OData isimli NuGet paketinden yararlanmaktır. İşte bu yazımızda çok basit bir örnek ile söz konusu senaryoyu incelemeye çalışacağız. Senaryomuzda kategori ve bu kategoriler bağlı ürünlerimizin olduğu In-Memory bir veri depomuz olacak. Bu veri kümesine Web API servisleri üzerinden OData sorgusu atmaya çalışacağız. Gelin adım adım ilerleyerek örneğimizi geliştirelim.

## Projenin Oluşturulması ve Gerekli NuGet Paketlerinin Yüklenmesi

Visual Studio ortamında (ki ben örneği 2013 sürümünde geliştirdim) bir Asp.Net projesi oluşturarak işe başlayalım. Empty şablonunu tercih edip Web API seçeneğini işaretleyerek devam edelim.

![ODataWebAPI_1.gif](/assets/images/2016/ODataWebAPI_1.gif)

Proje oluşturulduktan sonra OData kullanımını kolaylaştıracak olan NuGet paketinin yüklenmesi gerekmektedir. NuGet Package Manager Console üzerinden ilgili paket aşağıdaki komut ile projeye yüklenir.

install-package Microsoft.AspNet.OData

> Örneği Entity Framework tabanlı olacak şekilde geliştirebilirsiniz de. Bu durumda install-package EntityFramework komutu ile gerekli NuGet paketini yüklemeniz yeterlidir.

## Örnek Model Sınıflarının Yüklenmesi

İşlemlerimizi çok basit bir şekilde ele alacağız. Aşağıdaki sınıf çizelgesinde görülen tipleri Models klasörüne ekleyerek ilerleyebiliriz. Teorik olarak kategoriler ve bunlara bağlı ürünlerin olduğu miniminacık bir dünyamız var. Her iki Entity arasında bir ilişki kurduğumuzu görebilirsiniz. Yani bir kategoriye bağlı n sayıda ürün söz konusudur.

Burada önemli olan kısım Contained niteliğinin (Attribute) kullanımıdır. Bu nitelik ile OData sorgularının Entity'ler arası ilişkileri (Relations) çalışma zamanında tanıyabilmesini sağlamaktayız. AzonDataSources sınıfı temel olarak veriyi doldurduğumuz yerdir. Örnek çalışmamızda Entity Framework yerine In-Memory çalışan bir çözüm kullandığımızı bir kere daha hatırlatmak isterim.

![ODataWebAPI_2.gif](/assets/images/2016/ODataWebAPI_2.gif)

Data Source Sınıfımız

```csharp
using System.Collections.Generic;
using System.Data.Entity;

namespace UsingOData.Models
{
    public class AzonDataSources
    {
        private static AzonDataSources ds = null;
        public List<Category> Categories { get; set; }
        public List<Product> Products { get; set; }

        private AzonDataSources()
        {
            Categories = new List<Category>();
            Products = new List<Product>();
            this.PrepareData();
        }

        public static AzonDataSources Instance
        {
            get
            {
                if (ds == null)
                {
                    ds = new AzonDataSources();
                }
                return ds;
            }
        }

        public void PrepareData()
        {
            Category kitapCategory = new Category
            {
                 CategoryID=1,
                 Name="Kitap"
            };
            Category elektronikCategory = new Category
            {
                CategoryID=2,
                 Name="Elektronik"
            };

            kitapCategory.Products = new List<Product>
            {
                new Product{ 
                    Category=kitapCategory,
                    ListPrice=20,
                    ProductID=1,
                    Title="C# All in One"
                },
                new Product{ 
                    Category=kitapCategory,
                    ListPrice=8.95M,
                    ProductID=91,
                    Title="Asp.Net Web API Introduction"
                },
                new Product{ 
                    Category=kitapCategory,
                    ListPrice=12.50M,
                    ProductID=8,
                    Title="Pragmatic Programmer"
                },
                new Product{ 
                    Category=kitapCategory,
                    ListPrice=5,
                    ProductID=14,
                    Title="The Last Lecture"
                }
            };

            elektronikCategory.Products = new List<Product>
            {
                new Product{ 
                    Category=elektronikCategory,
                    ListPrice=200,
                    ProductID=28,
                    Title="LG Tablet x10"
                },
                new Product{ 
                    Category=elektronikCategory,
                    ListPrice=280.95M,
                    ProductID=92,
                    Title="Apple Smart Watch"
                },
                new Product{ 
                    Category=elektronikCategory,
                    ListPrice=1200M,
                    ProductID=55,
                    Title="Diesel Watch Blue"
                }
            };

            this.Categories.Add(kitapCategory);
            this.Categories.Add(elektronikCategory);
            this.Products.AddRange(kitapCategory.Products);
            this.Products.AddRange(elektronikCategory.Products);
        }
    }
}
```

Category isimli sınıfımız,

```csharp
using System.Collections.Generic;
using System.Web.OData.Builder; 

namespace UsingOData.Models
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string Name { get; set; }
        [Contained]
        public IList<Product> Products { get; set; }
    }
}
```

Product isimli sınıfımız

```csharp
using System.Web.OData.Builder;

namespace UsingOData.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string Title { get; set; }
        public decimal ListPrice { get; set; }
        [Contained]
        public Category Category { get; set; }
    }
}
```

## OData Endpoint'inin Ayarlanması

Şimdi OData için gerekli Route ayarlarını yapalım. Bunun için WebApiConfig.cs içeriğini aşağıdaki gibi düzenlemeliyiz.

```csharp
using Microsoft.OData.Edm;
using System.Web.Http;
using System.Web.OData.Batch;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using UsingOData.Models;  

namespace UsingOData
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapODataServiceRoute("odata", null, GetEdmModel(), 
                new DefaultODataBatchHandler(GlobalConfiguration.DefaultServer));
            config.EnsureInitialized();
        }
        private static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.Namespace = "ODataSample";
            builder.ContainerName = "DefaultContainer";
            builder.EntitySet<Category>("Categories");
            builder.EntitySet<Product>("Products");
            return builder.GetEdmModel();
        }
    }
}
```

Kritik nokta MapODataServiceRoute metodunda kullanılan GetEdmModel fonksiyonudur. ODataConventionModelBuilder tarafından çağırılan GetEdmModel fonksiyonunun sonucunu döndürmektedir. Burada builder için gerekli Namespace, Container ve kullanılacak EntitySet bildirimleri yapılır. EntitySet bildirimlerinde kullanılan isimler biraz sonra yazacağımız Controller sınıflarının da adı olacaktır. (Örneğin Categories için CategoriesController gibi)

## Controller Sınıflarının Yazılması

Pek tabi Web API servisimizin önemli aktörlerinden birisi de Controller sınıflarıdır. Senaryomuzda iki Entity söz konusu olduğu için yine iki adet Controller ekleyeceğiz. Controller içeriklerimiz son derece basit.

CategoriesController sınıfı

```csharp
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using UsingOData.Models;

namespace UsingOData.Controllers
{
    public class CategoriesController 
        : ODataController
    {
        [EnableQuery]
        public IHttpActionResult Get()
        {
            return Ok(AzonDataSources.Instance.Categories.AsQueryable());
        }
    }
}
```

ve ProductsController sınıfı

```csharp
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using UsingOData.Models;

namespace UsingOData.Controllers
{
    public class ProductsController 
        : ODataController
    {
        [EnableQuery]
        public IHttpActionResult Get()
        {
            return Ok(AzonDataSources.Instance.Products.AsQueryable());
        }      
    }
}
```

Her iki sınıf birer Get metoduna sahip. Örneğimizi çok basit bir şekilde ele alacağımızdan Post, Put, Delete gibi operasyonları hariç tuttuk. IHttpActionResult arayüzünün (Interface) taşıyabileceği nesne örnekleri cinsinden değer döndüren Get metodlarımız EnableQuery niteliği (Attribute) ile işaretlenmiş durumdalar. Bu nitelik sayesinde çalışma zamanında OData sorgu komutlarını kullanabileceğimizi belirtmiş oluyoruz. Ok isimli fonksiyon OkNegotiatedContentResult tipinden değer döndürmekte. Örneğimizde HTTP 200 dönmesini bekliyoruz.

## Test Etmeye Hazırız

Herhangibir tarayıcıdan aşağıdaki komutları deneyerek örneğimizi test edebiliriz. Sonuçlar tahmin edileceği gibi JSON formatında görünecektir.

http://localhost:61708/$metadata çağrısı ile aslında servisin metadata içeriğine ulaşabiliriz. Böylece servisin hangi entity'leri sunduğunu da görebiliriz.

![ODataWebAPI_5.gif](/assets/images/2016/ODataWebAPI_5.gif)

http://localhost:61708/Categories ile tüm kategorileri elde ederiz.

![ODataWebAPI_7.gif](/assets/images/2016/ODataWebAPI_7.gif)

http://localhost:61708/Categories?$expand=Products ile kategorileri ve bunlara bağlı ürünlerin listesini komple elde ederiz.

![ODataWebAPI_6.gif](/assets/images/2016/ODataWebAPI_6.gif)

http://localhost:61708/Products?$select=Title,ListPrice ile tüm ürünlerin sadece Title ve ListPrice değerlerini elde ederiz.

![ODataWebAPI_8.gif](/assets/images/2016/ODataWebAPI_8.gif)

http://localhost:61708/Products?$filter=startswith (Title,'A') ile A harfiyle başlayan ürün listesini elde ederiz.

![WebAPI_9.gif](/assets/images/2016/WebAPI_9.gif)

Pek tabi OData sorgularında kullanabileceğimiz pek çok anahtar kelime var. Bu kabiliyetlere [http://docs.oasis-open.org/odata/odata/v4.0/odata-v4.0-part2-url-conventions.html](http://docs.oasis-open.org/odata/odata/v4.0/odata-v4.0-part2-url-conventions.html) adresinden detaylı bir şekilde bakabilirsiniz.

Bu makalemizde bir Web API hizmetini OData sorgularını destekleyecek hale nasıl getirebileceğimizi incelemeye çalıştık. Bir diğer makalemizde görüşünceye dek hepinize mutlu günler dilerim.