---
title: "Razor Dünyasındaki İlk Adımlarım"
pubDate: 2019-05-17 12:53:00
categories:
  - Asp.Net Core
tags:
  - Asp.Net Core
  - Bash
  - C#
  - JSON
  - .NET
  - ASP.NET
  - Entity Framework
  - EF Core
  - LINQ
  - HTTP
  - Authentication
  - Async/Await
  - Generics
  - Visual Studio
  - GitHub
---

# Razor Dünyasındaki İlk Adımlarım
![image.axd](images/image.axd)

Bizim servisin dönüş yolculuğu bir başkadır. Her gün yaklaşık git gel neredeyse seksen kilometrelik yol teperiz (Daha ne kadar teperim bilemiyorum tabii) Dönüş yolculuğumuz trafiğin durumuna göre bazen çok uzun sürer. İşte böyle akşamların çok özel bir anı vardır.

Şekerpınardan yola çıkan yüzler Ümraniye sapağına girmek üzere otobandan ayrıldığımızda gülümser. Sadece evlerimize yaklaştığımız ve günün yorgunluğunu atmak üzere ayakkabılarımızı fırlatacağımız için değil, sevgili İhsan Bey radyosunu açıp Zeki Müren'den Müzeyyen Senar'dan Safiye Ayla'dan Muazzez Ersoy'dan ve daha nice değerli sanatçımızdan oluşan koleksiyonunu dinletmeye başladığı için de tebessüm ederiz.

Şirkete ilk başladığım günlerde servisteki pek çok kişi bana bakıp rapçi olduğumu düşünmüş ve İhsan Bey'in çaldığı şarkıları pek sevemeyeceğime kanaat getirmişti. Aslında lise yıllarında sıkı bir Heavy Metal'ci olan ben büyüdükçe farklı tınıları, farklı kültürlerin tonlamalarını da dinler olmuştum. Müziğin dili, dini, ırkı olmaz diyenlerdenim. Zaman geçtikçe ve özellikle plak merakım da başlayınca Aşık Veysel'den Joe Satriani'ye, Coşkun Sabah'tan Pink Floyd'a, Barış Manço'dan Metallica'ya, Sezen Aksu'dan Mozart'a kadar çok geniş bir müzik keyfine ulaştığımı fark ettim. Bu konuya nereden mi geldik? Microsoft'un Razor'unu kurcalarken kaleme aldığım derlemeye nasıl bir giriş yaparım diye düşünürken aklıma gelen ACDC'nin The Razors Edge albümünden. Haydi başlayalım;)

[Saturday-Night-Works çalışmalarımdaki 21 numaralı örnek](https://github.com/buraksenyurt/saturday-night-works/tree/master/No%2021%20-%20Introducing%20Razor)teki amacım, Microsoft'un Asp.Net Core MVC tarafında özellikle sayfa odaklı senaryolar için geliştirdiği Razor çatısını tanımaktı. Bu çatıda sayfalar doğrudan istemci taleplerini karşılayıp arada bir Controller'a uğramadan sayfa modeli (PageModel) ile konuşabilmekte. Razor sayfaları SayfaAdı.cshtml benzeri olup kullandıkları sayfa modelleri SayfaAdi.cshtml.cs şeklinde oluşturuluyor. Genel hatları ile URL yönlendirmeleri aşağıdaki tablodakine benzer şekilde olmakta. Örneğin /Book adresine göre pages klasöründeki Book.cshtml isimli sayfa talep edilmiş oluyor. Sayfanın arka plan kodları da aynı klasördeki cs dosyasında yer alıyor. Web standartları gereği /Index ve / talepleri aynı route adres olarak değerlendiriliyor. Tabii adreslere farklı şekillerde adresleme yapmakta mümkün. Tablodaki /Category önekli adres yönlendirmeleri bu anlamda düşünülebilir. Elbette konuyu anlamanın en iyi yolu bir örneği çalışmaktan geçiyor.

Örnek URL Adresi
Karşılayan Razor Sayfası
Model Nesnesi

/Book
pages/Book.cshtml
pages/book.cshtml.cs

/Category/Product
pages/Category/Product.cshtml
pages/Category/Product.cshtml.cs

/Category
pages/Category/Index.cshtml
pages/Category/Index.cshtml.cs

/Category/Index
pages/Category/Index.cshtml
pages/Category/Index.cshtml.cs

/Index
pages/Index.cshtml
pages/Index.cshtml.cs

/
pages/Index.cshtml
pages/Index.cshtml.cs

> Çalışmada veri girişi yapılabilen basit bir form tasarlayıp, Razor'un kod dinamiklerini anlamak istedim. İlk aşamada bilgileri InMemory veri tabanında tutmayı planladım. Son aşamada ise SQLite veri tabanını devreye aldım.

## Başlangıç

Hazırsanız ilk adımlarımızla işe başlayalım. Ben diğer pek çok örnekte olduğu gibi kodlamayı WestWorld (Ubuntu 18.04, 64bit) üzerinde Visual Studio Code aracıyla gerçekleştirmekteyim. Linux tarafında Razor uygulamalarını oluşturmak için en azından.Net Core 2.2'ye ihtiyacımız var. Projeyi aşağıdaki terminal komutunu kullanarak oluşturabiliriz.

```bash
dotnet new webapp -o MyBookStore
```

Açılan uygulama iskeletini biraz inceleyecek olursak Razor sayfaları ve ilişkili model sınıflarının Pages klasöründe konuşlandırıldığını görebiliriz. Static HTML dosyaları, Javacript kütüphaneleri ve CSS içerikleri de wwwroot altında bulunmaktadır. Resim, video vb varlıkları da bu klasör altında toplayabiliriz. Şu haliyle bile uygulamayı ayağa kaldırıp varsayılan olarak gelen içerikle çalışmamız mümkün. Ancak bizim amacımız okuduğumuz kitapları yöneteceğimiz basit bir Web arayüzü geliştirmek.

## Geliştirme Safhası

Gelelim kod tarafına. Burada kitap ekleme, listeleme ve düzenleme işlemleri için bir takım sayfalarımız mevcut. Ancak öncelikle Data isimli bir klasör oluşturup StoreDataContext.cs ve Book.cs isimli Entity sınıflarını ekleyerek işe başlayalım. Tahmin edeceğiniz üzere Entity Framework Core ile entegre ettiğimiz bir ürünümüz var.

StoreDataContext.cs

```csharp
using Microsoft.EntityFrameworkCore;

namespace MyBookStore.Data
{
    public class StoreDataContext
        : DbContext
    {
        public StoreDataContext(DbContextOptions<StoreDataContext> options)
            : base(options)
        {
            // InMemory db kullanacağımız bilgisi startup'cs deki
            // Constructor metoddan alınıp base ile DbContext sınıfına gönderilir
        }

        public DbSet<MyBookStore.Data.Book> Books { get; set; } // Kitapları tutacağımız DbSet 
    }
}
```

Book.cs

```csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace MyBookStore.Data
{
    /*
    Book entity sınıfının özelliklerini DataAnnotations'dan gelen çeşitli
    attribute'lar ile kontrol altına alıyoruz.
    Zorunlu alan olma hali, sayısallar ve string'ler için aralık kontrolü yapmaktayız.
    Buradaki ErrorMessage değerleri, Razor Page tarafında Validation işlemi sırasında 
    değer kazanır ve gerektiğinde uyarı olarak sayfada gösterilirler.
     */
    public class Book
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Kitabın adını yazar mısın lütfen")] 
        [StringLength(60, MinimumLength = 2, ErrorMessage = "En az 2 en fazla 60 karakter")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Kaç sayfalık bir kitap bu")]
        [Range(100, 1500, ErrorMessage = "En az 100 en çok 1500 sayfalık bir kitap olmalı")]
        public int PageCount { get; set; }
        [Required(ErrorMessage = "Liste fiyatı girilmeli")]
        [Range(1, 100, ErrorMessage = "En az 1 en çok 100 liralık kitap olmalı")]
        public double ListPrice { get; set; }
        [Required(ErrorMessage = "Kısa da olsa özet gerekli")]
        [StringLength(250, MinimumLength = 50, ErrorMessage = "Özet en az 50 en fazla 250 karakter olmalı")]
        public string Summary { get; set; }
        [Required(ErrorMessage = "Yazar veya yazarlar olmalı")]
        [StringLength(60, MinimumLength = 3, ErrorMessage = "Yazarlar için en az 3 en fazla 60 karakter")]
        public string Authors { get; set; } //TODO Author isimli bir Entity modeli kullanalım
    }
}
```

> Örnek ilk başta InMemory veri tabanını kullanacak şekilde tasarlanmıştır. Bu nedenle Startup.cs dosyasındaki ConfigureServices metodunda aşağıdaki gibi bir enjekte söz konusudur.
> ```bash
> // InMemory veritabanı kullanacağımız DbContext'imizi DI ile ekledik
> services.AddDbContext<StoreDataContext>(options=>options.UseInMemoryDatabase("StoreLook"));
> ```
> SQLite kullanımına geçildiğindeyse buradaki servis entegrasyonu şöyle olmalıdır.
> ```csharp
> // appsettings'den SQLite için gerekli connection string bilgisini aldık
> var conStr=Configuration.GetConnectionString("StoreDataContext");
> // ardından SQLite için gerekli DB Context'i servislere ekledik
> // Artık modellerimiz SQLite veritabanı ile çalışacak
> services.AddDbContext<StoreDataContext>(options=>options.UseSqlite(conStr));
> ```

Kitap ekleme fonksiyonelliği için Pages klasörüne ekleyeceğimiz AddBook.cshtml ve AddBook.cshtml.cs tipleri kullanılmaktadır. Bunlar Razor Page ve Model nesnelerimiz.

AddBook.cshtml

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyBookStore.Data;

namespace MyBookStore.Pages
{
    // İsimlendirme standardı gereği Razor sayfa modelleri 'Model' kelimesi ile biter
    public class AddBookModel : PageModel // PageModel türetmesi ile bir model olduğunu belirttik
    {
        private readonly StoreDataContext _context;
        //BindProperty özelliği ile Book tipinden olan BookData özelliğini Razor sayfasına bağlamış olduk.
        [BindProperty]
        public Book BookData { get; set; }

        public AddBookModel(StoreDataContext context)
        {
            _context = context; // Db Context'i kullanabilmek için içeriye aldık
        }

        // Asenkron olarak çalışabilen ve sayfadaki Submit işlemi sonrası tetiklenen Post metodumuz
        // Tipik olarak Razor sayfasındaki model verisini alıp DbSet'e ekliyor ve kayıt ediyoruz.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var addedBook=_context.Books.Add(BookData).Entity;
            Console.WriteLine($"{addedBook.Title} eklendi");
            await _context.SaveChangesAsync();
            return RedirectToPage("/Index"); // Kitap eklendikten sonra ana sayfaya yönlendirme yapıyoruz
        }
    }
}
```

AddBook.cshtml.cs

```text
@page // sayfanın bir razor page olduğunu belirttik
@model MyBookStore.Pages.AddBookModel  // sayfanın konuşacağı model sınıfını işaret ettik.

<html>
    <body>
        <h2>Yeni bir kitap eklemek ister misin?</h2>
        <form method="POST">
            <!--BookData sayfaya bağladığımız entity tipinden nesne örneği. 
            Bunu bağlamak için AddBookModel sınıfında BindProperty niteliği ile işaretlenmiş 
            bir özellik tanımladık. Her input kontrolünde dikkat edileceği üzere asp-for
            niteliği ile bir özelliğe bağlantı yapılmakta -->

            <div class="input-group mb-3">
                <input type="text" asp-for="BookData.Title" placeholder="Başlığı" class="form-control" aria-label="Default" aria-describedby="inputGroup-sizing-default">
            </div>
            <div class="input-group mb-3">
                <input type="text" asp-for="BookData.Authors" placeholder="Yazarları" class="form-control" aria-label="Default" aria-describedby="inputGroup-sizing-default">
            </div>
            <div class="input-group mb-3">
                <input type="text" asp-for="BookData.PageCount" placeholder="Sayfa sayısı" class="form-control" aria-label="Default" aria-describedby="inputGroup-sizing-default">
            </div>
            <div class="input-group mb-3">
                <input type="text" asp-for="BookData.ListPrice" placeholder="Liste fiyatı" class="form-control" aria-label="Default" aria-describedby="inputGroup-sizing-default">
            </div>
            <div class="input-group mb-3">
                <textarea asp-for="BookData.Summary" placeholder="Kısa bir özeti" class="form-control" aria-label="Özet"></textarea>
            </div>
            <button type="submit" class="btn btn-primary btn-lg btn-block">Kaydet</button>
        </form>
        <!--asp-for kullanılan tüm elementler için çalışacak olan
        validation işleminin sonuçları buraya yansıtılıyor-->
        <div asp-validation-summary="All"></div>
    </body>
</html>
```

Kitap bilgilerini düzenlemek içinse EditBook.cshtml ve EditBook.cshtml.cs isimli tipleri kullanmaktayız.

EditBook.cshtml.cs

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyBookStore.Data;
namespace MyBookStore.Pages
{
    public class EditBookModel
        : PageModel
    {
        // EditBook.cshtml sayfasına BookData özelliğini bağlamak için bu nitelik ile işaretledik
        [BindProperty]
        public Book BookData { get; set; }
        private StoreDataContext _context;
        public EditBookModel(StoreDataContext context)
        {
            _context = context;
        }

        // Güncelleme sayfasına id bilgisi parametre olarak gelecektir
        // Bunu kullanarak ilgili kitabı bulmaya ve bulursak BindProperty özelliği taşıyan
        // BookData isimli özelliğe bağlıyoruz.
        public async Task<IActionResult> OnGetAsync(int id)
        {
            BookData = await _context.Books.FindAsync(id);
            if (BookData == null) // Eğer bulunamassa ana sayfaya geri dön
            {
                return RedirectToPage("/index");
            }
            return Page(); //Bulunduysa sayfada kal
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Eksik veya hatalı bilgiler nedeniyle Model örneği doğrulanamadıysa
            // sayfada kalalım
            if (!ModelState.IsValid)
            {
                return Page();
            }
            // Güncellenen kitap bilgilerini Context'e ilave edip durumunu Modified'e çektik
            _context.Attach(BookData).State = EntityState.Modified;

            try
            {
                // Değişiklikleri kaydetmeyi deniyoruz
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Exception($"{BookData.Id} numaralı kitabı bulamadık!");
            }

            // İşlemler başarılı ise tekrardan index'e(Anasayfa oluyor tabii) dönüyoruz
            return RedirectToPage("/index");
        }
    }
}
```

EditBook.cshtml

```text
@page "{id:int}" // Sayfa direktifinde parametre bilidirmi söz konusu. Nitekim buraya güncellenmek istenen sayfanın id bilgisini almamız gerekiyor
@model MyBookStore.Pages.EditBookModel
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers

@{
    ViewData["Title"] = "Kitap Bilgisi Güncelleme"; //Sayfa başlığını değiştirdik
}

<!--
Yeni bir kitap ekleme sayfasındakine benzer olacak şekilde bir formumuz var.
Form verisini Page Model sınıfındaki BindProperty'nin verisi ile dolduruyoruz.
Bunun için HTML kontrollerinin asp-for niteliklerini kullanmaktayız.
Submit özellikli Button'a basılması Sayfa model sınıfındaki OnPostAsync fonksiyonunun
tetiklenmesine neden olacaktır. Bu sayfa yüklenirken devreye giren OnGetAsync metodunun parametresi
Page direktifinde belirtilmiştir. Yani sayfa Id parametresi ile gelen talepleri karşıladığında
bunu ilgili metoda iletir. Tahmin edileceği üzere integer tipinden olmayan geçersiz bir Id değeri ile 
sayfaya gelinmesi HTTP 404 etkisi yaratacaktır.
Bir sayfaya gelen router parametrelerinin opsiyonel olmasını istersek ? takısını kullanmak yeterlidir.
"{id:int?}" gibi
-->
<h3>@Model.BookData.Id numaralı kitabın bilgilerini günelleyebilirsiniz</h3>
<form method="post">
    <input asp-for="BookData.Id" type="hidden" />
    <div class="input-group mb-3">
    <input type="text" asp-for="BookData.Title" placeholder="Başlığı" class="form-control" aria-label="Default" aria-describedby="inputGroup-sizing-default">
    </div>
    <div class="input-group mb-3">
        <input type="text" asp-for="BookData.Authors" placeholder="Yazarları" class="form-control" aria-label="Default" aria-describedby="inputGroup-sizing-default">
    </div>
    <div class="input-group mb-3">
        <input type="text" asp-for="BookData.PageCount" placeholder="Sayfa sayısı" class="form-control" aria-label="Default" aria-describedby="inputGroup-sizing-default">
    </div>
    <div class="input-group mb-3">
        <input type="text" asp-for="BookData.ListPrice" placeholder="Liste fiyatı" class="form-control" aria-label="Default" aria-describedby="inputGroup-sizing-default">
    </div>
    <div class="input-group mb-3">
        <textarea asp-for="BookData.Summary" placeholder="Kısa bir özeti" class="form-control" aria-label="Özet"></textarea>
    </div>
    <button type="submit" class="btn btn-primary btn-lg btn-block">Kaydet</button>
    <div asp-validation-summary="All"></div>
</form>
```

Varsayılan olarak gelen Index.cshtml ve Index.cshtml.cs içeriklerinide aşağıdaki gibi değiştirelim.

Index.cshtml.cs

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyBookStore.Data;

namespace MyBookStore.Pages
{
    public class IndexModel
            : PageModel
    {
        private readonly StoreDataContext _context;

        public IndexModel(StoreDataContext context)
        {
            // DbContext'i içeriye aldık
            _context = context;
        }
        public IList<Book> Books { get; private set; }
        // Kitap listesini çektiğimiz asenkron metodumuz
        public async Task OnGetAsync()
        {
            Books = await _context.Books
                            .AsNoTracking()
                            .ToListAsync();
        }
        // Silme operasyonunu icra eden metodumuz
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            // Silme operasyonu için Identity alanından önce
            // kitabı bul
            var book=await _context.Books.FindAsync(id);
            if(book!=null) //Kitabı bulduysan
            {
                _context.Books.Remove(book); 
                //Kitabı çıkart ve Context'i son haliyle kaydet
                await _context.SaveChangesAsync();
            }
            return RedirectToPage(); // Scotty bizi o anki sayfaya döndür
        }
    }
}
```

Index.cshtml

```text
@page
@model IndexModel
@{
    ViewData["Title"] = "Kitaplarım";
}
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers

<h2>Güncel Liste</h2>
<form method="post">
    <!-- Modeldeki Books özelliğinin işaret ettiği nesnelerin her biri için dönüyoruz -->
    @foreach (var book in Model.Books)
    {
      <div class="card">
        <div class="card-body">
            <!--O anki book nesne örneğinin özelliklerine ulaşıp değerlerini basıyoruz -->
            <h5 class="card-title">@book.Title (@book.PageCount sayfa)</h5>
            <h6 class="card-subtitle mb-2 text-muted">@book.Authors</h6>
            <p class="card-text">@book.Summary</p>
            <p class="card-text">@book.ListPrice</p>
            <!--Güncelleme başka bir Razor Page tarafından yapılacak -->
            <a asp-page="./EditBook" asp-route-id="@book.Id" class="card-link">Düzenle</a>
            <!--Silme işlemi ise bu sayfadan Post edilerek gerçekleşecek
            asp-route-id ile silme ve güncelleme operasyonlarında gerekli identity
            alanının nereden bağlanacağını belirtiyoruz
            -->
            <button type="submit" asp-page-handler="delete" asp-route-id="@book.Id" class="card-link">Sil</button>
        </div>
      </div>  
    }
    <!--Yeni bir kitap eklemek için AddBook sayfasına yönlendiriyoruz-->
    <a asp-page="./AddBook">Yeni Kitap</a>
</form>
```

Ayrıca shared klasöründe yer alan _Layout.cshtml dosyasınıda kurcalayıp navigasyon sekmesindeki linklerin bizim istediğimiz şekilde çıkmasını sağlayabiliriz.

```text
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - MyBookStore</title>

    <environment include="Development">
        <link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.css" />
    </environment>
    <environment exclude="Development">
        <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/4.1.3/css/bootstrap.min.css"
              asp-fallback-href="~/lib/bootstrap/dist/css/bootstrap.min.css"
              asp-fallback-test-class="sr-only" asp-fallback-test-property="position" asp-fallback-test-value="absolute"
              crossorigin="anonymous"
              integrity="sha256-eSi1q2PG6J7g7ib17yAaWMcrr5GrtohYChqibrV7PBE="/>
    </environment>
    <link rel="stylesheet" href="~/css/site.css" />
</head>
<body>
    <header>
        <nav class="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3">
            <div class="container">
                <a class="navbar-brand" asp-area="" asp-page="/Index">Sevdiğim Kitaplar</a>
                <button class="navbar-toggler" type="button" data-toggle="collapse" data-target=".navbar-collapse" aria-controls="navbarSupportedContent"
                        aria-expanded="false" aria-label="Toggle navigation">
                    <span class="navbar-toggler-icon"></span>
                </button>
                <div class="navbar-collapse collapse d-sm-inline-flex flex-sm-row-reverse">
                    <ul class="navbar-nav flex-grow-1">
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-area="" asp-page="/Index">Lobi</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-area="" asp-page="/AddBook">Yeni Kitap</a>
                        </li>
                        <!-- <li class="nav-item">
                            <a class="nav-link text-dark" asp-area="" asp-page="/Privacy">Privacy</a>
                        </li> -->
                    </ul>
                </div>
            </div>
        </nav>
    </header>
    <div class="container">
        <partial name="_CookieConsentPartial" />
        <main role="main" class="pb-3">
            @RenderBody()
        </main>
    </div>

    <footer class="border-top footer text-muted">
        <div class="container">
            © 2019 - MyBookStore - <a asp-area="" asp-page="/Privacy">Privacy</a>
        </div>
    </footer>

    <environment include="Development">
        <script src="~/lib/jquery/dist/jquery.js"></script>
        <script src="~/lib/bootstrap/dist/js/bootstrap.bundle.js"></script>
    </environment>
    <environment exclude="Development">
        <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.3.1/jquery.min.js"
                asp-fallback-src="~/lib/jquery/dist/jquery.min.js"
                asp-fallback-test="window.jQuery"
                crossorigin="anonymous"
                integrity="sha256-FgpCb/KJQlLNfOu91ta32o/NMZxltwRo8QtmkMRdAu8=">
        </script>
        <script src="https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/4.1.3/js/bootstrap.bundle.min.js"
                asp-fallback-src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"
                asp-fallback-test="window.jQuery && window.jQuery.fn && window.jQuery.fn.modal"
                crossorigin="anonymous"
                integrity="sha256-E/V4cWE4qvAeO5MOhjtGtqDzPndRO1LBk8lJ/PR7CA4=">
        </script>
    </environment>
    <script src="~/js/site.js" asp-append-version="true"></script>

    @RenderSection("Scripts", required: false)
</body>
</html>
```

## Çalışma Zamanı

Kodlama tarafını tamamladıktan sonra uygulamayı aşağıdaki terminal komutu ile çalıştırıp deneme sürüşüne çıkabiliriz.

```bash
dotnet run
```

Eğer uygulama sorunsuz çalıştıysa http://localhost:5401/ adresi üzerinden hareket edebiliriz. İster üst bara eklediğimiz linkten ister http://localhost:5401/AddBook adresine giderek yeni kitap ekleme sayfasına ulaşabiliriz (Razor için belirlenen varsayılan adres WestWorld sisteminde kullanıldığı için UseUrls metodu ile onu 5401e çektim. Program.cs'e bakınız)

> In Memory veritabanı kullandığımız versiyonda uygulama sonlandığında tüm kayıtlar uçacaktır. Kalıcı bir depolama için SQL, SQLite ve benzeri sistemleri içeriye enjekte edebiliriz. İlerleyen kısımda SQLite denememiz olacak.

Uncle Bob temalı örnek bir kitap verisini ilk denemede kullanmak isterseniz diye aşağıya bilgilerini bırakıyorum;)

```text
Clean Architecture
Robert C. Martin (Uncle Bob)
393
34.99
"This is essential reading for every current of aspiring software architect..."
```

![image.axd](images/image.axd)

Console logundan kitabın eklendiğini izleyebiliriz.

![image.axd](images/image.axd)

İşlemler sırasında veri doğrulama kontrolüne takılırsak aşağıdaki gibi bir görüntü ile karşılaşırız (Bu kısmı daha şık bir hale getirmek gerekiyor. Belki popup'lar ile uyarı vermek daha güzel olabilir. Bunu yapmayı bir deneyin)

![image.axd](images/image.axd)

Başarılı girişler sonrası gelinen Index sayfasının çıktısı ise aşağıdaki ekran görüntüsündekine benzer olacaktır.

![image.axd](images/image.axd)

Bir kitabı düzenlemek için Düzenle başlıklı linke tıkladığımızda EditBook/{Id} şeklindeki bir yönlendirme çalışır. Bu tahmin edeceğiniz üzere EditBook.cshtml sayfasının işletilmesini sağlayacaktır.

![image.axd](images/image.axd)

Düzenleme sonrası örnek sonuçlar da şöyle olabilir.

![image.axd](images/image.axd)

## InMemory Veritabanını SQLite ile Değiştirme

Örnekte kullandığımız veri merkezini SQLite tarafına dönüştürmek için EntityFramework Core'un ilgili NuGet paketini projeye eklemek lazım. Bunun için aşağıdaki terminal komutu kullanılabilir.

```bash
dotnet add package Microsoft.EntityFrameworkCore.SQLite
```

Ardından appsettings.json dosyasına bir Connection String bildirimi dahil edip, Startup sınıfındaki ConfigureServices metodunda minik bir ayarlama yapmak gerekiyor ki bunu yazının önceki kısımlarında not olarak belirtmiştik.

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "ConnectionStrings": {
    "StoreDataContext": "Data Source=MyBookStore.db"
  },
  "AllowedHosts": "*"
}
```

Bunlar başlangıç aşamasında yeterli değil. Çünkü ortada fiziki veri tabanı yok. Dolayısıyla SQLite veri tabanının da oluşturulması gerekiyor.

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Yukarıdaki terminal komutları sayesinde DataContext türevli sınıf baz alınarak migration planları çıkartılır. Planlar hazırlandıktan sonra ikinci komut ile update işlemi icra edilir.

![image.axd](images/image.axd)

Eğer veri tabanını baştan hazırlamaz ve update planını çalıştırmazsak aşağıdakine benzer bir hata ile karşılaşabiliriz.

![image.axd](images/image.axd)

Artık verilerimiz SQLite ile fiziki olarak da kayıt altında. Hatta Visual Studio Code'a [SQLite Explorer Extension](https://marketplace.visualstudio.com/items?itemName=alexcvzz.vscode-sqlite) isimli aracı eklersek oluşan DB dosyasının içeriğini de görebiliriz.

![image.axd](images/image.axd)

## Ben Neler Öğrendim?

Bu çalışmanın da bana kattığı bir sürü şey oldu elbette. Üstünden tekrar geçmenin faydalarını gördüm ilk başta. Özetle öğrendiklerimi aşağıdaki gibi sıralayabilirim.

- Razor Page ve Page Model kavramlarının ne olduğunu
- Razor'un temel çalışma prensiplerini
- Yönlendirmelerin (Routing) nasıl işlediğini
- Razor içinden model nesnelerine nasıl bağlanılabileceğini (property binding)
- Entity Framework Core'da InMemory veri tabanı kullanımını
- DI ile ilgili servislerin nasıl enjekte edildiğini
- Çeşitli DataAnnotations niteliklerini (attributes)
- InMemory veri tabanında SQLite kullanımına geçince yapılması gereken değişiklikleri ve Migration'ın ne işe yaradığını

Böylece [Saturday-Night-Works](https://github.com/buraksenyurt/saturday-night-works) çalışmalarının 21 numaralı örneğine ait derlemenin sonuna gelmiş olduk. Diğer çalışmalardan da gözüme kestirdiklerimi ele alıp bloğuma not olarak düşeceğim. Fark ettim ki Saturday-Night-Works çalışmaları kendimi kişisel olarak geliştirmek adına yeterli ama tamamlayıcılık açısından eksik. Yapılan her uygulamanın üstünden bir kere daha geçmek, kodları okumak ve notları daha derli toplu olarak bloguma koymak tamamlayıcı bir motivasyon olarak karşıma çıkıyor. Bir başka macera derlemesinde görüşmek ümidiyle hepinize mutlu günler dilerim.
