---
layout: post
title: ".Net Core Web Api Tarafında SqlKata ile Sevimli SQL İşlemleri"
date: 2020-10-28 14:14:00 +0300
categories:
  - dotnet-core
tags:
  - dotnet-core
  - yaml
  - bash
  - csharp
  - dotnet
  - ado-net
  - entity-framework
  - linq
  - postgresql
  - mysql
  - rest
  - json
  - web-api
  - http
  - docker
  - performance
  - caching
  - transactions
  - generics
  - visual-studio
  - github
---
Veri odaklı uygulamalarda sorgu komutlarını çalıştırmak için kullandığımız birçok hazır altyapı var. Örneğin.Net dünyasına baktığımızda en temel seviyede Ado.Net ve Object Relational Mapping tarafında Entity Framework sıklıkla karşılaştıklarımız arasında. [SqlKata](https://sqlkata.com/)'da bunlardan birisi olarak düşünülebilir. Bir süredir de sağda solda okuduğum makale ve github çalışmalarından dolayı merak edip kurcalamak istediğim bir kütüphane. Öncelikle ismi çok hoş (Code Kata'yı çağrıştırıyor bana)

![kata.png](/assets/images/2020/kata.png)

C# ile geliştirimiş paketin temel amacı SqlServer, PostgreSql, Firebird, MySql gibi veritabanları için kod tarafında ortak bir sorgu oluşturma/derleme arabirimi sunmak ama bunu LINQ sorgu metotları üzerinden SQL dilinin anlaşılır rahatlığında, injection yemeden (Parameter Binding tekniğini kullandığı için) ve ön bellekleme (caching) gibi performans artırıcıları kullanarak sağlamak. Tabii konuşmayı bir kenara bırakıp kod yazarak onu tanımaya çalışmak en doğrusu. Ben örneği Heimdall (Ubuntu-20.04) üzerinde ve Visual Studio Code arabirimini kullanarak geliştirmekteyim (Uygulama kodlarının tamamına [SkyNet github reposu](https://github.com/buraksenyurt/skynet/tree/master/No%2035%20-%20Hiyaaa%20This%20is%20SqlKata) üzerinden erişebilirsiniz)

Örneğimizde SqlKata paketini bir PostgreSql veritabanı üzerinden kullanacağız. Daha önceden de sıklıkla yaptığımız üzere Docker imajından yararlanabiliriz. Ancak elimde içeriği ile dolu dolu hazır bir veritabanı olsa güzel olabilir. Microsoft'un Adventure Works ve Northwind gibi, yazılım eğitimlerinde sıklıkla kullanılan efsane veritabanları olduğunu bilirsiniz. PostgreSql için hazırlanmış olan hatta Docker Container olarak çalıştırılabilecek bir versiyonunu da [şu github adresinden](https://github.com/pthom/northwind_psql) tedarik edebiliyoruz. Üstelik güzel bir ilişkisel diagram da mevcut (repodaki wind-of-change klasöründe kurulumu için gerekli SQL dosyasını bulabilirsiniz) Docker için kullanacağımız kompozisyonun içeriği ise aşağıdaki gibidir.

```yml
version: '3'

services:
  db:
    image: postgres:12
    environment:
      POSTGRES_DB: northwind
      POSTGRES_USER: scoth
      POSTGRES_PASSWORD: tiger
    ports:
        - "5433:5433"
    volumes:
      - ./dbdata:/var/lib/postgresql/data
      - ./init.sql:/docker-entrypoint-initdb.d/init.sql
    command: -p 5433
```

İlgili terminal komutu ile docker container ayağa kaldırıldıktan sonra yüklenen Northwind veritabanı içerisine şöyle bir bakmakta da yarar var. En azından verilerin geldiğini görelim.

```bash
# container'ı ayağa kaldırmak için wind-of-change klasöründe aşağıdaki terminal komutunu kullanabiliriz
docker-compose up

# ikinci bir terminalden veya pgAdmin gibi bir araçla Northwind içeriğine bakabiliriz
# Ben scoth kullanıcı adını tanımlamıştım.
docker-compose exec db psql -U scoth -d northwind

# Yukarıdaki komut sonrası açılan psql cli'de birkaç SQL ifadesi deneyebiliriz
# Örneğin En pahalı 3 ürünü listeleylim
Select product_name,unit_price from products order by unit_price desc limit 3;
# veya kategorilerin adlarını
Select category_name from categories;
```

![skynet_35_Screenshot_01.png](/assets/images/2020/skynet_35_Screenshot_01.png)

Pek tabii amacımız SqlKata ile bu PostgreSql veritabanına bağlanıp bir takım işlemler yapabilmek. Rapor çekmek, satır ekleyip silmek ve benzer işlemleri icra edebiliriz. Deneme kodları için bir.Net Core Web Api projesi pekala uygun bir çözüm. REST servis çağrıları ile uçtan uca çalışan bir örneği denemiş de oluruz. Öyleyse aşağıdaki terminal komutlarından yararlanarak projemizi oluşturalım ve gerekli Nuget paketlerini de ekleyerek yolumuza devam edelim.

# Önce src klasöründe bir api projesi açayım
dotnet new webapi -o northwind-api

# Gerekli paketleri yükleyelim
# Postgresql için npsql ve SqlKata için SqlKata:)
dotnet add package Npgsql
dotnet add package SqlKata
dotnet add package SqlKata.Execution

SQLKata ile ilgili örnek kullanımlar Controller sınıflarımızda yer alıyor. Basit olması açısından Category, Product ve Customer tabloları için birer Controller tipi mevcut. İlk olarak bu sınıfları yazarak işe başlayabiliriz. CustomerController içerisinde hangi şehirde kaç müşteri olduğu bilgisini raporlayan basit bir operasyon bulunuyor.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

/*
    SqlKata kullanımı için eklenen namespace bildirimleri
*/
using SqlKata;
using SqlKata.Execution;

namespace NorthwindApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly QueryFactory _queryFactory;

        public CustomerController(ILogger<CustomerController> logger, QueryFactory queryFactory)
        {
            _logger = logger;
            _queryFactory = queryFactory;
        }

        /*
            Hangi şehirde kaç müşterimiz olduğunu döndüren action.
            customers tablosunda city bilgisine göre gruplama yapıp, count alıyoruz yani.
            Örnek sorgu -> https://localhost:5001/api/customer/cityreport
        */
        [HttpGet("cityreport")]
        public IActionResult GetCustomerCountsByCity()
        {
            var report = _queryFactory
                .Query("customers")
                .Select("city")
                .SelectRaw("count(customer_id) as count") // aggregation yaptığımız yer
                .GroupBy("city") // city alanına göre grupluyoruz
                .HavingRaw("count(customer_id)>1") // toplam müşteri sayısı 1in üstünde olanlar için (having e bir bakayım demiştim)
                .Get();

            return Ok(report);
        }
    }
}
```

ProductController ise iki operasyon sunmakta. Birisinde satışta olmayan/üretilmeyen ürünlerin listesini döndürüyoruz (Siz var olanları çekmeyi de deneyin) Diğeri ise kategoriye göre ürün listesini sayfa bazlı döndürüyor. REST tipindeki servislerde liste bazlı operasyonların büyük boyutta veri döndürmesi çok tercih edilen bir durum değil. Bunun yerine veriyi sayfalama tekniği ile döndürmek hem performans hem de veri güvenliği açısından daha doğru. Sayfalamanın SQLKatacasını aşağıdaki kod parçasında görebilirsiniz:)

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

/*
    SqlKata kullanımı için eklenen namespace bildirimleri
*/
using SqlKata;
using SqlKata.Execution;

namespace NorthwindApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        /*
            Startup tarafında bildirimini yaptığımız QueryFactory nesnesini
            constructor ile buraya enjekte ettik. Böylece controller içindeki
            tüm action metotlarında SqlKata yı kullanabileceğiz.
        */
        private readonly QueryFactory _queryFactory;

        public ProductController(ILogger<ProductController> logger, QueryFactory queryFactory)
        {
            _logger = logger;
            _queryFactory = queryFactory;
        }

        /*
            İlk SqlKata denemem.
            products tablosunda discontinued olanların listesini çekmeye çalışıyoruz
            Geriye JSON içerik dönecektir
        */
        [HttpGet("Discontinued/")]
        public IActionResult GetDiscontinuedProducts()
        {
            var products = _queryFactory
                .Query("products") // products tablosu için sorgu hazırlanacak
                .Select("product_id", "product_name", "unit_price") // sadece bu alanlar getirilecek
                .Where("discontinued", 1) // discontinued değeri 1 olanlar çekilecek
                .Get();

            //_logger.LogInformation($"{DateTime.UtcNow.ToLongTimeString()} - ProductController - GET");

            return Ok(products);
        }

        /*
            Parametre olarak gelen kategori altındaki ürünleri sayfalayarak getiren action.
            Sayfalama için Limit ve Offset fonksiyonlarını kullanıyoruz.
            Route üstünden gelen page değerine göre bir konuma gidip o konumdan itibaren 5 kayıt gösteriyoruz.
            Örnek sorgu -> https://localhost:5001/api/product/Beverages/3
        */
        [HttpGet("{categoryName}/{page}")]
        public IActionResult GetProductsByCategory(string categoryName, int page)
        {
            var products = _queryFactory
                .Query("products as p")
                .Join("categories as c", "p.category_id", "c.category_id")
                .Select(
                    "c.category_name",
                    "p.{product_id,product_name,unit_price,units_in_stock}")
                .Limit(5)
                .Offset((page - 1) * 5)
                .Get();

            return Ok(products);
        }

    }
}
```

CategoryController tarafında kullandığımız Category sınıfı;

```csharp
namespace NorthwindApi.Model
{
    public class Category{
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] Picture { get; set; }
    }
}
```

CategoryController, yeni kategori eklenmesi, silinmesi ve kategori listesinin döndürülmesi ile ilgili metotlar barındırıyor.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NorthwindApi.Model;

/*
    SqlKata kullanımı için eklenen namespace bildirimleri
*/
using SqlKata;
using SqlKata.Execution;

namespace NorthwindApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly QueryFactory _queryFactory;

        public CategoryController(ILogger<CategoryController> logger, QueryFactory queryFactory)
        {
            _logger = logger;
            _queryFactory = queryFactory;
        }

        /*
            Yeni bir kategori eklemek için kullanacağımız post action.
            Parametre olarak gelen JSON içeriğindeki alanları kullanıyor.
            Insert işlemi sonucuna göre de Ok veya 500 dönüyoruz.
            Adres : https://localhost:5001/api/category
            Metot : HTTP Post
            Body : 
            {
                "CategoryId":10,
                "Name": "Kitap",
                "Description": "Kitap konulu ürünler"
            }
        */
        [HttpPost]
        public IActionResult AddCategory(Category category)
        {
            try
            {
                var inserted_id = _queryFactory
                                .Query("categories")
                                .Insert(new
                                {
                                    category_id = category.CategoryId,
                                    category_name = category.Name,
                                    description = category.Description
                                });
                return Ok(category);
            }
            catch (Exception exp)
            {
                _logger.LogError(exp.Message);
                return StatusCode(500, "Kategori ekleme işlemi başarısız!");
            }
        }

        /*
            Denemeler sırasında categories tablosunu kirletecek yeni satırlar ekledim tabii :)
            Silme operasyonu da lazım.
            Örnek sorgu https://localhost:5001/api/category/10
            Metot HTTP Delete
        */
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            int deleted = _queryFactory.Query("categories").Where("category_id", id).Delete();
            return Ok(deleted);
        }

        /*
            Kategorileri listeleyen action
            https://localhost:5001/api/category
        */
        [HttpGet]
        public IActionResult GetCategories()
        {
            var categories = _queryFactory
                .Query("categories")
                .OrderBy("category_name")
                .Get();

            return Ok(categories);
        }
    }
}
```

ve tabii SQLKata için gerekli Middleware tanımlarını da içeren startup dosyamız.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
/*
    Postgresql ve SqlKata için gerekli namespace bildirimleri
*/
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;
using Npgsql;

namespace NorthwindApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            /*
                QueryFactory sınıfını burada kayıt edip controller'lara constructor üzerinde enjekte ederek kullandırtabiliriz.
                Oluştururken Postgresql bağlantı bilgisini veriyoruz.
                Ayrıca sorgular için gerekli derleyici nesnesi de üretiliyor
            */
            services.AddScoped(factory =>
            {
                return new QueryFactory
                {
                    Compiler = new PostgresCompiler(),
                    // Varsayılan olarak Postgresql 5432 portunu kullanıyor. 
                    // Ben docker-compose'da dışarıya 5433 portundan açtığım için farklı. 
                    Connection = new NpgsqlConnection("Server=127.0.0.1;Port=5433;Username=scoth;Password=tiger;Database=northwind")
                };
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
```

Artık uygulamamız test sürüşüne hazır;) Web API hizmeti çalışmaya başlamadan önce tahmin edeceğiniz gibi docker-compose ile PostgreSql Container örneğinin ayağa kalkmış olması gerekiyor. Sonrasında aşağıdaki terminal komutu ile ilerleyebiliriz.

```bash
dotnet run watch
```

İlk olarak artık satışta olmayan ürünleri çekmeye çalışalım. Bunun için servise

```text
https://localhost:5001/api/product/discontinued
```

şeklinde bir talep göndermemiz yeterli. Aşağıdakine benzer bir sonuç almamız gerekiyor.

![skynet_35_Screenshot_02.png](/assets/images/2020/skynet_35_Screenshot_02.png)

Bir kategoriye ait ürünlerin birkaç bilgisini getiren ama bunu yaparken sayfalama tekniğini kullanan operasyon içinse aşağıdaki gibi bir talep yollanabilir.

```text
https://localhost:5001/api/product/Beverages/3
```

![skynet_35_Screenshot_04.png](/assets/images/2020/skynet_35_Screenshot_04.png)

Dikkat edileceği üzere 3ncü sayfadan itibaren ilk 5 kayıt getirilmiştir. En basit operasyonlardan birisi de tüm kategorilerin çekilmesidir:)

```text
https://localhost:5001/api/category
```

![skynet_35_Screenshot_05.png](/assets/images/2020/skynet_35_Screenshot_05.png)

Müşterilerin bulundukları şehre göre guruplandığı raporu ise aşağıdaki taleple çekebiliriz.

```text
https://localhost:5001/api/customer/cityreport
```

![skynet_35_Screenshot_06.png](/assets/images/2020/skynet_35_Screenshot_06.png)

Hemen yeni bir kategori eklemeyi deneyelim. Ekleme işlemini Postman ile denemek için aşağıdaki örnek bilgileri kullanabilirsiniz.

```text
Adres: https://localhost:5001/api/category
Metod: HTTP Post
Body: json
Örnek İçerik:
{
	"CategoryId":10,
	"Name": "Kitap",
	"Description": "Kitap konulu ürünler"
}
```

![skynet_35_Screenshot_07.png](/assets/images/2020/skynet_35_Screenshot_07.png)

Tabii aynı Id ile bir kategori eklemek istersek exception yönetimimize göre aşağıdaki gibi bir çıktı almamız gerekir (Bu hata mesajını biraz daha anlamlı hale getirmek yerinde olabilir. Nitekim işlem başarısız ama neden başarısız olduğu istemci tarafında tam olarak anlaşılmıyor)

![skynet_35_Screenshot_08.png](/assets/images/2020/skynet_35_Screenshot_08.png)

Eklediğimiz kategoriyi silmek içinse bir HTTP Delete çağrısı yapmalıyız.

```text
Adres: https://localhost:5001/api/category/10
Metot: HTTP Delete
```

Bu arada örneği çalışırken karşılaştığım enteresan bir durum vardı. Bu durumu açıklamak için localhost:5001/api/product/Beverages talebini göz önüne alalım. Beverages kategorisindeki ürünlerin listesini almayı bekliyoruz. Ancak kod tarafında yapacağımız minik bir değişiklik yüzünden aşağıdaki ekran görüntüsünde olduğu gibi connection string bilgisini görme ihtimalimiz var. Öncelikle bu nasıl mümkün olabilir, ikinci olarak bunun önüne nasıl geçeriz? Lütfen yorumlarınızı esirgemeyin, tüm okurlarımız faydalansın.

![skynet_35_Screenshot_03.png](/assets/images/2020/skynet_35_Screenshot_03.png)

SQL sorgulama komutlarına hakim olanlar için LINQ (Language INtegrated Query) ile birleştirilen bu kullanım şekli oldukça pratik görünüyor. Üretim ortamında da değerlendirilebilir mi henüz emin değilim ancak pilot olarak denenebilir. Yüksek transaction içeren servis operasyonlarında nasıl bir tepki vereceğine bakacak şekilde yük testlerine tabi tutmak karar verme noktasında yardımcı olabilir.

SQLKata'yı tanımaya çalıştığımız bu örnek üzerinde yapabileceğiniz daha birçok şey var. Örneğin OData altyapısını kullanmadan servisi OData standartlarına uyumlu hale getirmeyi deneyebilirsiniz ya da PostgreSql yerine MySql gibi bir veritabanını kullanabilirsiniz. SQL sorgularını daha yakından tanımak için farklı operasyonları devreye alabilirsiniz. Son bir haftada (ya da belirtilen aylarda) verilen siparişlerin listesini çekmek, talepte bulunan kullanıcının yaşadığı bölgeye göre o yörede en çok satılan ürünleri raporlamak vb. Temel CRUD operasyonlarını saymıyorum bile;) Böylece geldik bir [SkyNet](https://github.com/buraksenyurt/skynet) derlememizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
