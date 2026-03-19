---
title: "Microsoft ML.Net Kullanarak Basit Bir Öneri Tahminleme Uygulaması Geliştirmek"
pubDate: 2020-08-08 06:13:00
categories:
  - .Net Core
tags:
  - .Net Core
  - Bash
  - C#
  - .NET
  - ASP.NET
  - ASP.NET Core
  - Entity Framework
  - EF Core
  - LINQ
  - Generics
  - Visual Studio
  - GitHub
  - DataSet
---

# Microsoft ML.Net Kullanarak Basit Bir Öneri Tahminleme Uygulaması Geliştirmek
![image.axd](images/image.axd)

Yaz başından beri Mark J. Price'ın, C# 8.0 and.NET Core 3.0 – Modern Cross-Platform Development: Build applications with C#,.NET Core, Entity Framework Core, ASP.NET Core, and ML.NET using Visual Studio Code, 4th Edition (Evet biraz uzun bir ismi var:D) [kitabıyla](https://www.amazon.com/8-0-NET-Core-3-0-Cross-Platform-dp-1788478126/dp/1788478126/ref=mt_other?_encoding=UTF8&me=&qid=) uğraştım. Hoş ben daha kitabı tamamlayamadan.Net 5.0 son sürümü çıktı ve hatta Mark bu sürüm için de yeni bir kitap yayınladı ya neyse:D Kitabın ilgimi çeken bölümlerinden birisi (19ncu Kısım) makine öğrenmesi ile alakalı olandı. Makine Öğrenmesi benim çok ama gerçekten çok uzak olduğum bir konu.

Yine de "Algoritması benden, modeli senin için eğitirim ve basitçe kullanırsın" diyen Microsoft'un ML.Net çatısını bir kod parçasında nasıl uygulayacağımı da merak etmekteydim. En iyisi kitabın dediklerini uygulamalı olarak yapmaktı. O zaman gelin bir parça kod bir parça ML.Net bir parça MVC yazalım.

ML.Net, Microsoft'un platform bağımsız ve açık kaynak olarak sunduğu makine öğrenmesi (Machine Learning) çatısı olarak tanımlanıyor. Bu kütüphane topluluğundan yararlanarak aşağıdakilere benzer senaryoları kolayca işletilebiliriz.

- Classification: Müşterilerin geri bildirimlerinin duyarlılığını analiz ederek gelen yorumun pozitif veya negatif olup olmadığını tahminlemek.
- Image Classification: En bilinen senaryolardan birisidir. Bir fotoğrafın (imgenin) hangi kategoriye ait olduğunu tahminlemek.
- Regression (Value prediction): Değer bazlı tahminleme yapmak. Örneğin bir yere giderken kullandığımız taksi ücretini ya da bir seyahatin fiyatını tahminlemek gibi.
- Recommendation: Kullanıcın geçmiş hareketliliklerine bakarak ona önerilerde bulunmak.

ML.Net ile veri setlerinin çeşitli tip algoritmalarla eğitilmesi ve kullanılması nispeten kolay görünüyor. Veri setindeki Feature adı verilen girdiler ve Label olarak isimlendirilen çıktı değerleri kullanılarak eğitilen model, yeni girdilere göre bir tahminlemede bulunabiliyor. Geliştireceğimiz kodda bir tahminlemede bulunacağız. Ancak bunu yazınca ML'i öğrenmiş olmuyoruz. ML'i kavramak için örneğin burada kullanılan Matrix Factorization öneri algoritmasını anlamak gerekiyor.

Örnek, Microsoft'un emektar Northwind veritabanını kullanan bir MVC uygulaması olacak. Müşterilerin satın aldığı ürüne göre onlara öneride bulunacak bir kurgu söz konusu (Bunu aldıysanız bunlara da bir bakabilirsiniz der gibisinden) Product tablosundaki ID değerleri ülke bazında bir matrise oturtuluyor (İki kolonlu bir tuple liste. ProductRelation model sınıfına dikkat edelim) İki kolonlu bu matris veri setimizi oluşturmakta. Bu veri setini baz alan Matrix Factorization algoritmasını kullanarak bir model eğitiyoruz. Çalışma zamanında kullanıcılar sepete ürün eklediklerinde daha önceden eğitilmiş olan model gizemli güçlerini kullanarak diğer müşterilerin de aldığı ve bizim alabileceğimiz en potansiyel 3 ürünü listeliyor. Örnek kodların tamamına [skynet github reposu](https://github.com/buraksenyurt/skynet/tree/master/No%2026%20-%20Easy%20ML.Net%20Sample) üzerinden erişebilirsiniz. Şimdi terminalden gerekli hazırlıkları yaparak kodlamaya başlayabiliriz.

```bash
# MVC Uygulamasının Oluşturulması
dotnet new mvc -o SmartWind
cd SmartWind
mkdir Data
touch Data/Northwind.cs
touch Models/Category.cs Models/Product.cs Models/Order.cs Models/OrderDetail.cs Models/Customer.cs Models/CartItem.cs Models/Cart.cs Models/ProductRelation.cs Models/Recommendation.cs Models/EnrichedRecommendation.cs Models/HomeCartViewModel.cs Models/HomeIndexViewModel.cs /Views/Home/Cart.cshtml

# Veriseti dosyalarını tutacağımız klasör
mkdir wwwroot/DataSets

# Gerekli NuGet Paketleri (EF, ML.Net)
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.ML
dotnet add package Microsoft.ML.Recommender
```

Konumuz ML.Net tarafını kavramak olduğundan Model ve DbContext sınıflarına detaylıca girmemiz gerek yok ancak bizim için kritik olan sınıfları aşağıdaki gibi geliştirerek devam edebiliriz. Gerekli modellerle başlayalım. İlk olarak tahminlemede devreye girecek algoritmanın girdisi olan veri modelini tanımlıyoruz.

```csharp
using Microsoft.ML.Data;

namespace SmartWind.Models
{
    /*
        Satın alınan bir ürünle ilintili diğer ürünlerin ilişkilendiği entity modeli.
        Bu aslında Matrix Factorization algoritmasının girdisi olan veriyi tutacak nesne.
    */
    public class ProductRelation
    {
        // 200 ile olası maksimum ID değerini belirttik. Küçük bir veri setinden çalışalım diye
        [KeyType(200)] // Column
        public uint ProductID { get; set; }
        [KeyType(200)] // Row
        public uint RelatedProductID { get; set; }
    }
}
```

Veri seti eğitildikten sonra algoritmanın çıktısını aşağıdaki sınıf ile ifade edebiliriz. Burada ilişkili ürün için birde skor bilgisine yer veriliyor.

```csharp
namespace SmartWind.Models
{
    /*
        Makine öğrenme algoritmasının çıktısı olan entity modeli.
        Önerilen ürün numarası ile skor puanını tutmakta.
    */
    public class Recommendation
    {
        public uint RelatedProductID { get; set; }
        public float Score { get; set; }
    }
}
```

EnrichedRecommendation sınıfı algoritma çıktısı olan tipten türemekle birlikte ek olarak ürün adını taşımaktadır. ML modeli Recommendation uyumlu bir çıktı verebildiğinden modeli genişletmek için türetme yoluna gidilmiştir.

```csharp
namespace SmartWind.Models
{
    public class EnrichedRecommendation
    : Recommendation
    {
        public string ProductName { get; set; }
    }
}
```

Controller sınıfı oldukça kritik görevler içerir. Ürün kartı ile ilgili temel işler dışında esas itibariyle veri setinin hazırlanması ve modelin eğitilmesiyle ilgili fonksiyonları ihtiva eder. Yorum satırlarını dikkatlice okumayı ihmal etmeyin;)

```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
// ML kullanımı için gerekli kütüphaneler
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.Data;
using Microsoft.ML.Trainers;

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text;

using SmartWind.Data;
using SmartWind.Models;

namespace SmartWind.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Northwind _db;
        private readonly IWebHostEnvironment _hostEnv;
        private string[] countries = { "Germany", "USA", "UK" };
        /*
            Constructor'dan EF Context ve WebHostEnvironment'i(Data folder'ını bulmak için) enjekte ettik
        */
        public HomeController(ILogger<HomeController> logger, Northwind db, IWebHostEnvironment hostEnv)
        {
            _logger = logger;
            _db = db;
            _hostEnv = hostEnv;
        }

        /*
            Seçilen verisetinin fiziki adresini döndürmeye yardımcı olan metot.
        */
        private string GetDataSetPath(string file)
        {
            return Path.Combine(_hostEnv.ContentRootPath, "wwwroot", "DataSets", file);
        }

        private HomeIndexViewModel CreateHomeIndexViewModel()
        {
            return new HomeIndexViewModel
            {
                Categories = _db.Categories.Include(c => c.Products),
                GermanyDatasetExists = System.IO.File.Exists(GetDataSetPath("germany-dataset.txt")),
                UKDatasetExists = System.IO.File.Exists(GetDataSetPath("uk-dataset.txt")),
                USADatasetExists = System.IO.File.Exists(GetDataSetPath("usa-dataset.txt"))
            };
        }

        public IActionResult Index()
        {
            /*
                Kategorileri, ürünleri ile birlikte döndüren 
                ve hatta DataSets klasörü içerisinde ülke bazında verisetleri
                olup olmadığı bilgilerini de içeren HomeIndexViewModel 
                nesnesini örnekleyip View tarafına gönderiyoruz.
            */
            var model = CreateHomeIndexViewModel();
            return View(model);
        }

        /*
            ML için veri setlerini örnekleyen ve sonrasında
            HomeIndexViewModel nesnesini oluşturup döndüren metot.
            Metot LING sorgusu yardımıyla 
            ProductID - RelatedProductID
            ikililerinden oluşan bir liste hazırlar. 
            Ülke bazında hazırlanan bu liste germany-dataset.txt,
            usa-dataset.txt, uk-dataset.txt adları ile
            wwwroot altındaki datasets klasörüne yazılır.
        */
        public IActionResult GenerateDataSets()
        {
            foreach (string country in countries) // Tanımlanan üç ülke için
            {
                // Bu ülkedeki siparişleri al
                var orders = _db.Orders
                            .Where(o => o.Customer.Country == country)
                            .Include(o => o.OrderDetails)
                            .AsEnumerable();

                // Ülke siparişlerindeki ürünler arası ilişkileri bul
                var productRelations = orders
                .SelectMany(
                    o =>
                    from item1 in o.OrderDetails
                    from item2 in o.OrderDetails
                    select new ProductRelation
                    {
                        ProductID = (uint)item1.ProductID,
                        RelatedProductID = (uint)item2.ProductID
                    }
                ).Where(p => p.ProductID != p.RelatedProductID)
                .GroupBy(p => new { p.ProductID, p.RelatedProductID })
                .Select(p => p.FirstOrDefault())
                .OrderBy(p => p.ProductID)
                .ThenBy(p => p.RelatedProductID);

                // Oluşturulan veriyi text dosyaya yaz
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("ProductID\tRelatedProuductID");
                foreach (var p in productRelations)
                {
                    builder.AppendLine($"{p.ProductID}\t{p.RelatedProductID}");
                }
                System.IO.File.WriteAllText(GetDataSetPath($"{country}-dataset.txt"), builder.ToString());
            }

            // Modeli oluşturup View'a döndür
            // Yukarıdaki döngü çalışınca ülke bazlı veri setleri de hazır olacaktır
            var model = CreateHomeIndexViewModel();
            return View("Index", model);
        }

        /*
            Modeli eğitilmesi için kullanılan Action metodu.
            Matrix Factorization (Collaborative Filtering olarak da geçiyor) algoritması kullanılır.
        */
        public IActionResult TrainModels()
        {
            foreach (string country in countries)
            {
                var mlContext = new MLContext();

                // Algoritma için girdi verisini taşıyan IDataView örneği hazırlanır

                var dataView = mlContext.Data.LoadFromTextFile( // Dosyadan yükleyecek
                  path: GetDataSetPath($"{country}-dataset.txt"), // veriseti dosyasını belirtiyoruz
                  columns: new[] // column ve row bilgilerini tanımlıyoruz
                  {
                    new TextLoader.Column(
                    name:     "Label",
                    dataKind: DataKind.Double,
                    index:    0),

                    new TextLoader.Column(
                    name:     "ProductID",
                    dataKind: DataKind.UInt32,
                    source:   new [] { new TextLoader.Range(0) },
                    keyCount: new KeyCount(200)),

                    new TextLoader.Column(
                    name:     "RelatedProductID",
                    dataKind: DataKind.UInt32,
                    source:   new [] { new TextLoader.Range(1) },
                    keyCount: new KeyCount(200))
                    },
                    hasHeader: true,
                    separatorChar: '\t'); // Kolonları Tab ile ayırmıştık hatırlarsanız

                /*
                    Algoritmaya has ayarlar. Buraları anlamak için algoritmanın detaylarını öğrenmem lazım.
                    Alphe, Lambda ve C değerleri ne anlama geliyor. Neden bu değerler verilmiş araştıralım.
                */
                var options = new MatrixFactorizationTrainer.Options
                {
                    MatrixColumnIndexColumnName = "ProductID",
                    MatrixRowIndexColumnName = "RelatedProductID",
                    LabelColumnName = "Label",
                    LossFunction = MatrixFactorizationTrainer.LossFunctionType.SquareLossOneClass,
                    Alpha = 0.01,
                    Lambda = 0.025,
                    C = 0.00001
                };

                MatrixFactorizationTrainer coachCarter = mlContext.Recommendation()
                  .Trainers.MatrixFactorization(options);

                ITransformer kokoskov = coachCarter.Fit(dataView); // Model eğitilir

                /* 
                    Üretilen model zip uzantılı kaydedilir.
                    Bu zip'i alıp başka bir uygulamada da kullanabiliriz.
                    Tabii veri setinin değişmesi halinde modeli yeniden eğitmek gerekecektir.
                */

                mlContext.Model.Save(kokoskov,
                  inputSchema: dataView.Schema,
                  filePath: GetDataSetPath($"{country}-model.zip"));
            }

            // Modelin ne kadar sürede eğitildiğini bulmak için buraya bir Stopwatch kullanımı getirilebilir ;)
            var model = CreateHomeIndexViewModel();
            return View("Index", model);
        }

        /*
            Ürün kartı gösteren Action Metodu.
            Burayı yazarken yer yer beynim yandı.
        */
        public IActionResult Cart(int? id)
        {
            // O anki Cart bilgisini cookie'de saklıyor
            string cartCookie = Request.Cookies["basket_items"] ?? string.Empty;

            /*
                Sepete eklenen ürünler bu örnek özelinde bir cookie'de duruyorlar.
                Cart action metoduna gelen id değeri boş değilse 
            */
            if (id.HasValue)
            {
                if (string.IsNullOrWhiteSpace(cartCookie))
                {
                    cartCookie = id.ToString();
                }
                else // ve ürün sepeti çerezinin içerisinde veriler varsa
                {
                    string[] ids = cartCookie.Split('|'); // pipe karakterine göre içeriği split ediyoruz 

                    if (!ids.Contains(id.ToString())) // gelen id bu çerez içerisinde yoksa(yani ürün sepette değilse)
                    {
                        cartCookie = string.Join('|', cartCookie, id.ToString()); // çerezin sonuna ürün numarasını (ProductID) ekliyoruz
                    }
                }

                // Çeresin güncel halinide basket_items anahtar değeri ile Response.Cookies koleksiyonuna ekliyoruz
                Response.Cookies.Append("basket_items", cartCookie);
            }

            // Önerileri ve güncel sepet içeriğini tutan model nesnesini örnekliyoruz
            // İlerleyen aşamalarda Recommendations ile belirtilen öneriler kısmı da doldurulacak
            var model = new HomeCartViewModel
            {
                Cart = new Cart
                {
                    Items = Enumerable.Empty<CartItem>()
                },
                Recommendations = new List<EnrichedRecommendation>()
            };

            // Çerez içeriğini ele aldığımız kısım
            if (cartCookie.Length > 0)
            {
                /*
                    Çerez listesini pipe işaretine göre böldükten sonra
                    Her bir ID'yi ve bundan yararlanarak bulacağımız ürün adını
                    CartItem nesnelerini örneklemek için kullanıyoruz
                    dolayısıyla Cart modelindeki Items koleksiyonunu çerezdeki ürün bilgileri ile doldurmuş olduk
                */
                model.Cart.Items = cartCookie.Split('|').Select(item =>
                  new CartItem
                  {
                      ProductID = int.Parse(item),
                      ProductName = _db.Products.Find(int.Parse(item)).ProductName
                  });
            }

            /*
                Şimdi eğitilmiş modelimizi devreye almaktayız.
                uk-model.zip'i kullanıyoruz. TrainModels bizim için gerekli model eğitimlerini
                tamamlayıp ülkelere göre ayrı zip dosyalarının oluşturulmasını sağlamıştı.
            */
            if (System.IO.File.Exists(GetDataSetPath("uk-model.zip"))) // UK Model eğitilmişse
            {
                var mlContext = new MLContext(); // MLContext nesnesi

                ITransformer modelUK;

                // uk-model.zip dosyasını kullanarak tahminleme motoru için gerekli model nesnesini yüklüyoruz
                using (var stream = new FileStream(
                  path: GetDataSetPath("uk-model.zip"),
                  mode: FileMode.Open,
                  access: FileAccess.Read,
                  share: FileShare.Read))
                {
                    modelUK = mlContext.Model.Load(stream, out DataViewSchema schema);
                }

                // Burası önemli! Tahminleme motorunu aktifleştiriyoruz
                var predictionEngine = mlContext.Model.CreatePredictionEngine<ProductRelation, Recommendation>(modelUK);

                // Şimdi var olan ürün listesini ele alalım
                var products = _db.Products.ToArray();

                /*
                    Sepete eklenen her ürün için tahmin motorunu kullanarak öneriler alınacak.
                    Bu öneriler Modelimizdeki Recommendations isimli liste üzerinde değerlendiriliyor.
                    Ekleme sırasında yapılan skorlamaya göre en olası 3 ürün Recommendations listesinde bırakılıyor.
                */
                foreach (var item in model.Cart.Items) // Çerezlerden yüklenen ürün listesindeki herbir öğeyi al
                {
                    /*
                        Ürünlerdeki ProductID değerini RelatedProductID olarak alıp çerezden gelen listedeki ProductID ile
                        ilişkilendirip tahminleme motorundan bir tahminleme yapmasını istiyoruz.
                        Bu ilişki skorlara göre tersten sıralanıyor ve ilk üçü alınıyor. Yani en olası üçlü.
                    */
                    var topThree = products
                      .Select(product =>
                        predictionEngine.Predict(
                          new ProductRelation
                          {
                              ProductID = (uint)item.ProductID,
                              RelatedProductID = (uint)product.ProductID
                          })
                        )
                      .OrderByDescending(x => x.Score)
                      .Take(3)
                      .ToArray();

                    /*
                        Öneriler id ve skor duran standart output nesnesine düşer.
                        Ürün bilgisini de buraya katmak istediğimizden 
                        Recommendation sınıfından türeyen EnrichedRecommendation isimli bir sınıf daha var.
                        Herbir ürün için bu öneriler oluşur ama...
                    */
                    model.Recommendations.AddRange(topThree
                      .Select(rec => new EnrichedRecommendation
                      {
                          RelatedProductID = rec.RelatedProductID,
                          Score = rec.Score,
                          ProductName = _db.Products.Find((int)rec.RelatedProductID).ProductName
                      }));
                }

                // ...ama tüm önerilerden en iyi üçü gereklidir. O nedenle son listeden tekrar top 3 yapılmış durumda
                model.Recommendations = model.Recommendations
                  .OrderByDescending(rec => rec.Score)
                  .Take(3)
                  .ToList();
            }

            return View(model);
        }
    }
}
```

Ürün kartının gösterildiği View aşağıdaki gibi programlanabilir.

```text
@model HomeCartViewModel
@{
    ViewData["Title"] = "Alışveriş Sepeti";
}
<h1>@ViewData["Title"]</h1>
<table class="table table-bordered">
    @foreach (CartItem item in Model.Cart.Items)
    {
        <tr>
            <td>@item.ProductID</td>
            <td>@item.ProductName</td>
        </tr>
    }
</table>
<h3>Sepetinizdeki ürünleri alan müşteriler ayrıca şu ürünleri de aldı!</h3>
@if (Model.Recommendations.Count() == 0)
{
    <div><p>Üzgünüm :( Şimdilik önerim yok. Belki modelinizi eğitmeniz gerekebilir.</p></div>
}
else
{
	<table class="table table-bordered">
		<tr>
			<th></th>
			<th>İlişkili Ürün</th>
			<th>Puanı</th>
		</tr>
		@foreach (EnrichedRecommendation rec in Model.Recommendations)
		{
			<tr>
			<td>
				<a asp-controller="Home" asp-action="Cart" asp-route-id="@rec.RelatedProductID" class="btn btn-primary">Sepete At</a>
			</td>
			<td>
				@rec.ProductName
			</td>
			<td>
				@rec.Score
			</td>
			</tr>
		}
	</table>
}
```

Giriş (Index) sayfasında aslında veri setinin oluşturulması, modelin eğitilmesi ve ürün kartına ulaşmak için gerekli bağlantılar yer alır. Tabii bu bir öğreti olduğu için bu işleri kullanıcının yapması istenmektedir. Gerçek hayat senaryosunda modelin harici bir ortamda eğitilip web uygulamasına servis edilmesi daha doğru bir yaklaşım olacaktır.

```text
@model HomeIndexViewModel
@{
  ViewData["Title"] = "Ürün Listesi";
}
<h1 class="display-8">@ViewData["Title"]</h1>
<p class="lead">
  <ol>
    <li>İlk adım, <a asp-controller="Home" asp-action="GenerateDataSets">Veri Setlerini Oluştur</a>.</li>
    <li>İkinci olarak, <a asp-controller="Home" asp-action="TrainModels">modelleri eğit</a>.</li>
    <li>Şimdi sepete birkaç ürün ekle.<a asp-controller="Home" asp-action="Cart">Ürün Kartı</a>.</li>
  </ol>
</p>
<hr />
@foreach (Category category in Model.Categories)
{
  <h3>@category.CategoryName <small>@category.Description</small></h3>
  <table>
  @foreach (Product product in category.Products)
  {
      <tr>
        <td>
          <a asp-controller="Home" asp-action="Cart" 
            asp-route-id="@product.ProductID"
            class="btn btn-success">Sepete At</a>
        </td>
        <td>
          @product.ProductName <i>($ @product.UnitPrice)</i>
        </td>
      </tr>
  }
  </table>
}
```

Index sayfasının kullandığı ViewModel tipini de aşağıdaki gibi yazarak çalışmamıza devam edebiliriz.

```csharp
using System.Collections.Generic;

/*
    Verisetlerinin eğitilip eğitilmediği bilgisini tutan Index modelimiz
*/
namespace SmartWind.Models
{
    public class HomeIndexViewModel{
        public IEnumerable<Category> Categories { get; set; }
        public bool UKDatasetExists { get; set; }
        public bool GermanyDatasetExists { get; set; }
        public bool USADatasetExists { get; set; }
        public long Milliseconds { get; set; }
    }
}
```

Buraya kadar geldiysek artık çalışma zamanı sonuçlarına bakabiliriz.

```bash
dotnet run
```

terminal komutu ile web uygulamasını çalıştırdıktan sonra [https://localhost:5001] adresine gidebiliriz. İlk olarak veri setleri oluşturulur, sonrasında model eğitilir ve sepete ürünler eklenip önerilerin ne olduğuna bakılır. Kahvelerimizi yudumlarken:D

Ana sayfamız aşağıdaki gibi görünecektir.

![image.axd](images/image.axd)

Örneğin sepete aşağıdaki gibi birkaç ürün eklediğimizde eğitilen model bize birkaç öneride bulunacaktır.

![image.axd](images/image.axd)

Tabii elimizde eğitilmiş bir model yoksa aşağıdaki gibi bir hata mesajı ile karşılaşırız.

![image.axd](images/image.axd)

Ben örnekleri icra ederken dikkatimi çeken unsurlardan birisi sepete ürün ekledikçe bazen aynı ürünün yine öneriler kısmında görünüyor olmasıydı. Bunu engellemek için ne yapılabilir yorumlarda paylaşabilirsiniz. Diğer yandan uygulamada eksik olan birçok kısım da mevcut. Örneğin sepete ürün ekleme ve görüntüleme işini aynı noktada üstlenen Cart action metodunu ayrıştırmaya çalışabilirsiniz ve ürün silme fonksiyonelliğini de sisteme katabilirsiniz. Başta da belirttiğim gibi ML konusu benim çok çok uzağımda kalan bir alan. Yine de ML.Net ile bazı fikirlerin kendi ürünlerimiz için hayata geçirilmesi oldukça kolay görünüyor. Eğer MVP ürünleri üstünde çalışıyorsanız ve bu tip ML fonksiyonelliklerine ihtiyacınız varsa bence göz önüne alınabilir. Bu arada Microsoft'un ML.Net'in uygulanması ile ilgili resmi öğreti dokümanları da oldukça başarılı. Ben iki öğretiyi [Skynet reposunda deneme fırsatı buldum](https://github.com/buraksenyurt/skynet/tree/master/No%2006%20-%20DT-Training/Chapter05). Böylece geldik bir derlemenin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
