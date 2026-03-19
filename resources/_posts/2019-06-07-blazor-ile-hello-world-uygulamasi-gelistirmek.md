---
layout: post
title: "Blazor ile Hello World Uygulaması Geliştirmek"
date: 2019-06-07 11:00:00 +0300
description: "Amacım Microsoft'un deneysel olarak geliştirdiği Blazor çatısı (Web Framework) ile C#/Razor (Razor HTML markup ve C#'ın bir arada kullanılabildiği syntax olara düşünülebilir. Bu sayede C# ve HTML kodlamasını aynı dosyada intellisense desteği ile ele alabiliriz) ,HTML ve WebAssembly tabanlı web uygulamalarının nasıl geliştirilebileceğini Hello World diyerek deneyimlemek."
categories:
  - dotnet-core
tags:
  - dotnet-core
  - bash
  - csharp
  - xml
  - dotnet
  - json
  - web-api
  - http
  - rust
  - javascript
  - blazor
  - async-await
  - performance
  - dependency-injection
  - visual-studio
  - github
  - dependency-management
---
# Blazor ile Hello World Uygulaması Geliştirmek
![gulelim.png](/assets/images/2019/gulelim.png)

Oturduğunuz yerden göründüğü gibi çok karikatür okuyan biri değilimdir. Ama bazen kendimi sevgili Yiğit Özgür'ün kaleminden çıkan bir Huni Kafa karakteri gibi hissettiğim olur. Bir sebepten ne olduğunu tam olarak anlayamadığım konular üzerinde debelenir dururum. O kaynaktan bu kaynağa geçerken de kaybolurum. Lakin her zaman elle tutulur bir şeylere ulaşma şansı da bulurum.

Blazor'da bu standart anlayamama sürecime takılan konulardan birisiydi. Ona olan merakım çevremde konuşulanlarla başlamıştı. Çok yakın dostum [Bora Kaşmer](http://www.borakasmer.com/)'in konu ile ilgili yazıları ve şirketteki deneyimli yazılımcıların tariflemelerine rağmen zihnimde onu tanımlayacak iyi bir cümleyi bir türlü kuramıyordum. Neden kullanacaktım ki onu? Hangi problemi çözüyordu? Ne gibi kolaylıklar getiriyordu? Bunları tam olarak niteleyemediğimi görünce 19ncu bölüm ortaya çıktı. Öyleyse notlarımı derlemeye başlayalım.

[Saturday-Night-Works'ün 19ncu bölümü](https://github.com/buraksenyurt/saturday-night-works/tree/master/No%2019%20-%20Hello%20Blazor)ndeki amacım Microsoft'un deneysel olarak geliştirdiği [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/client) çatısı (Web Framework) ile C#/Razor (Razor HTML markup ve C#'ın bir arada kullanılabildiği syntax olarak düşünülebilir. Bu sayede C# ve HTML kodlamasını aynı dosyada intellisense desteği ile ele alabiliriz), HTML ve WebAssembly tabanlı uygulamaların nasıl geliştirilebileceğini Hello World diyerek deneyimlemekti.

Aslında uzun süredir hayatımızda olan ve Windows, macOS, Linux gibi platformlarda C# tabanlı Client Web uyulamalarının geliştirilmesine odaklanan Blazor, bu idealini gerçekleştirirken WebAssembly desteğinden yararlanıyor. [WebAssembly](https://webassembly.org/), yüksek performanslı web uygulamalarının geliştirilmesinde kullanılan öncü akımlardan. Felsefe olarak C, C++, Rust gibi düşük seviyeli dillerle yazılmış kodların derlenerek browser (tüm tarayıcılar destekliyor) üzerinde çalıştırılabilmesi ilkesini benimsiyor. İşte bu noktada yorumlamalı dillerden olan ve web tarafında çok kullanılan Javascript'in önüne geçiyor. Bunun en büyük sebebi derlemenin getirdiği performans ve hız kazanımı. Blazor işte bu avantajı C# tarafında kullanabilmemize olanak sağlayan bir çatı. Konu kafamda hala muallakta olmakla birlikte en azından.Net Core cephesinde bir Blazor uygulaması nasıl geliştirilir bilmem gerekiyor. İlk hedef basit bir uygulamayı inşa edip ayağa kaldırmak ve temel bileşenleri anlamaya çalışmak.

> Blazor,.Net ile geliştirilmiş Single Page Application'ların WebAssembly desteği yardımıyla tarayıcı üzerinde çalışmalarına olanak sağlayan bir Web Framework olarak düşünülebilir.

Blazor cephesinde Client Side ve Server Side Hosting modelleri söz konusu. Client-Side modelinde C#/Razor ile geliştirilip derlenen.Net Assembly'ları,.Net Runtime ile birlikte tarayıcıya indiriliyor. Sunucu bazlı modele bakıldığındaysa, Razor bileşenlerinin sunucu tarafında konuşlandığını UI, Javascript ve olay (event) çağrıları içinse SignalR odaklı iletişimin devreye girdiğini görüyoruz. Esasında uygulamalar Component bazlı geliştirilmekte. Bir component bir C# sınıfıdır ve Blazor açısından bakıldığında genellikle bir cshtml dosyasıdır (Elbette bir C# dosyası da olabilir)

> WebAssembly koduna derlenen uygulamalar herhangi bir tarayıcıda yüksek performansla çalışabilirler.

## Nelere İhtiyacımız Var?

Pek çok kaynak konuyu Visual Studio üzerinde incelemekte. Bu profesyonel IDE üzerinde bir Web projesi açarken şablon kısmından Blazor'u seçmek yeterli. Ancak ben yabancı topraklardayım ve WestWorld'de Linux ile en yakın arkadaşı Visual Studio Code yaşamakta. Bu nedenle işe aşağıdaki terminal komutları ile başlamak gerekiyor.

```bash
dotnet new --install "Microsoft.AspNetCore.Blazor.Templates"
dotnet new blazor -o HelloWorld
```

Öncelikle blazor için gerekli proje şablonunu indiriyoruz. Ardından blazor tipinden hazır bir proje iskeletini oluşturuyoruz. Hemen ilgili klasöre girip dotnet run komutu ile programı çalıştırıp deneyebiliriz. Uyguluma, localhost:5000 numaralı porttan hizmet verecektir.

Oluşturulan ilk örneği didiklemekte fayda var. Index, Counter ve FetchData (Dependency Injection kullanılan örnek) yönlendirmeleri sonrası çalışan aynı isimli cshtml içeriklerine odaklanmak gerekiyor. Söz gelimi Counter sayfasında düğmeye bastıkça sayaç değeri artmakta. Ancak bu gerçekleşirken sayfa yeniden yüklenmiyor ki bunun için normalde Client-Side Javascript kodunun yazılması gerekir. Olaya Blazor açısından baktığımızda, kodlamanın Javascript değil de C# ile yapıldığını fark etmemiz lazım. İlgili sayfada oynayarak farklı sonuçlar elde etmeye çalışabiliriz. Ben Counter sayfasını biraz kurcalayıp kod tarafını aşağıdaki gibi ele almaya çalışmıştım

```text
@page "/counter"

<h1>Rastgele Toplamlar</h1>
<p>Blazor'a geçişten önce bu tarafı anlamaya çalışıyorum...</p>
<p>Güncel rastgele toplam: @currentCount</p>
<p>Arttırım miktarı: @incraseValue </p>

<button class="btn btn-primary" onclick="@IncrementValue">Arttırmak için bas!</button>

@functions {
    // Değişken değerlerini HTML tarafında @ operatörü ile kolayca kullanabiliriz
    int currentCount = 0;
    int incraseValue=0;
    Random random=new Random();

    void IncrementValue() // button'un onclick metodunda @ operatörü ile erişiyoruz
    {        
        incraseValue=random.Next(1,100); //1 ile 100 arasında rastgele değer ürettirdik
        currentCount+=incraseValue; 
    }
}
```

ki çalışma zamanı çıktısı aşağıdakine benzerdi.

![05_19_cover_1.png](/assets/images/2019/05_19_cover_1.png)

Arayüz mutlaka dikkatinizi çekmiştir. Hoş bir tasarımı var. En azından benim için öyle. Blazor proje şablonuna göre CSS tarafı için bootstrap hazır olarak geliyor. Sol taraftaki navigation menu'yü kurcalamak istersek, Shared klasöründeki NavMenu.cshtml ile oynamak yeterli ki örneğin son kısmında burayı değiştirmiş olacağız. Her şeyin giriş noktası olan index.html sayfasında blazor.webassembly.js isimli javascript dosyası için bir referans bulunuyor.

## Dependency Injection Kullanımı

Blazor dahili bir DI mekanizmasını destekliyor ve built-in olanlar haricinde kendi servislerimizin de içeriye bu mekanizma yardımıyla alınmasına olanak sağlıyor (hatta buna zorluyor) Söz gelimi HttpClient gibi bir built-in servisi client-side Razor tarafına enjekte edip kullanabiliriz. IJSRuntime, IUriHelper gibi bir çok yararlı built-in servis bulunmakta. Kendi servislerimizi de (söz gelimi bir data repository için kullanılabilecek tipleri) DI ile sisteme dahil etmemiz mümkün. Aynen.Net Core'da olduğu gibi ConfigureServices metoduna gelen IServicesCollection arayüzünden yararlanarak bunu sağlayabiliriz (WorldPopulation sayfasında built-in servis kullanımına dair bir örnek bulunuyor)

```csharp
services.AddSingleton<IMessenger, SMSMessenger>();
```

## Kod Tarafının Geliştirilmesi

Şimdi Blazor tarafındaki kodlamayı anlayabilmek için iki basit bileşen tasarımı yapalım. Bunlardan ilkinde kobay olarak kitaplarımızı konu alacağız. Bir listeye kitap eklenmesi ve bu listenin gösterilmesi işlerini yapmaya çalışacağız. Bir kitabı kod tarafında temsil emtek için book isimli aşağıdaki sınıftan yararlanabiliriz. I know, I know... Bir kitabı birden fazla yazar yazmış olabilir ve bir yazarın birden fazla kitabı da olabilir. Hani nerede nesneler arası many-to-many ilişki? Motivasyonum Blazor tarafında Hello World demek olduğu için bu kısmı tamamen örtpas etmiş durumdayım.

```csharp
public class Book
{
    public string Title { get; set; }
    public string Summary { get; set; }
    public int PageCount { get; set; }
    public string Authors { get; set; }
}
```

Kitaplar ile ilgili işlemler için Pages klasörüne Book.cshtml isimli bir dosya ekleyip aşağıdaki şekilde kodlayabiliriz. Çok basit olarak kitap listesinin gösterilmesi ve yeni bir kitabın eklenebilmesi için gerekli fonksiyonelliklerin sunulduğu bir arayüzümüz var. HTML tarafı ile kod bir arada kullanılmakta.

```text
@page "/bookList"

<h1>Okuduğum Kitaplar (Toplam @books.Count() kitabım var) </h1> 
<!--Toplam kitap sayısını da başlığa ekledik -->

<blockquote class="blockquote">
    Burada okumaktan keyif aldığım kitaplar yer alıyor.
</blockquote>

<ul>
    <!-- Tüm kitapları dolaşıp örnek olarak başlıklarını listeliyor ve hemen alt kısmına özet bilgilerini yerleştiriyoruz-->
    @foreach(var book in books){
        <li aria-describedby="bookTitle">@book.Title</li>
         <small id="bookTitle" class="form-text text-muted">@book.Summary</small> 
    }
</ul>
<!-- Yeni bir kitap bilgisinin girişi için Bootstrap ile zenginleştirilmiş basit bir formumuz var -->
<div class="form-group">
    <input class="form-control" id="txtTitle" placeholder="Kitabın adı" bind="@newBook.Title"/><br/> <!--bind attribute'una atanan değer ile Title özelliğine bağladık -->
    <input class="form-control" id="txtAuthors" placeholder="Yazarlar" bind="@newBook.Authors" /><br/>
    <input class="form-control" id="txtPageCount" placeholder="Sayfa sayısı" bind="@newBook.PageCount" /><br/>
    <input class="form-control" id="txtSummary" aria-describedby="summaryHelp" placeholder="Özet" bind="@newBook.Summary" />
    <small id="summaryHelp" class="form-text text-muted">Lütfen bir cümleyle kitabın neyle ilgili olduğunu anlat</small> <!-- yardımcı bilgi veren metin için koyduk -->
</div>
<button onclick="@AddNewBook" class="btn btn-primary">Listeye ekleyelim</button> <!-- onclick attribute'unda AddNewBook metoduna bağladık -->

<!-- Fonksiyonlarımız -->
@functions{
    // Tüm kitap listemizi ifade eden koleksiyonumuz
    IList<Book> books=new List<Book>();
    Book newBook=new Book();
    // Yeni bir kitap eklemek için kullanıyoruz.
    void AddNewBook(){
        books.Add(newBook); // Kitabı listeye ekledik
        newBook=new Book(); // Eğer newBook nesnesini sıfırlamassak büyük ihtimalle koleksiyona hep aynı nesne örneği eklenecektir.
    }
}
```

Ekleyeceğimiz bir diğer örnek Dependency Injection kullanımı ile ilgili. Built-in olarak gelen HttpClient servisini cshtml tarafında nasıl kullanabileceğimizi görebilmek için WorldPopulation.cshtml isimli bir dosya geliştiriyoruz. Yine Pages klasörüne konuşlandıracağımız dosya içeriği aşağıdaki gibi yazılabilir. @page direktifine göre /population adresine gelen taleplere karşılık bu sayfa işletilecektir. @inject kısmında httpClient servisinin koda enjekte edilmesi söz konusudur.

```text
@page "/population"
@inject HttpClient httpClient
<!-- built-in servislerden olan HttpClient servisini buraya enjekte ettik. 
httpClient değişken adıyla kullanabiliriz -->

<h2>Güncel 3 Günlük Dünya Nüfusu Bilgileri</h2>

<blockquote class="blockquote">
    Bilgiler api.population.io sitesinden alınmıştır.
</blockquote>

@if (values == null) // Henüz veriler gelmemiş olabilir.
{
    <p><em>Bilgiler alınıyor...</em></p>
}
else
{
    <div class="card" style="width: 18rem;">
        <ul class="list-group list-group-flush">
            @foreach (var currentData in @values) // Tüm değerleri dolaşıp güncel nüfus verisini ekrana basıyoruz            
            {
                <li class="list-group-item">@string.Format("{0:#,0}",@currentData.Value) - @currentData.Date.ToShortDateString() </li>
            }
        </ul>
    </div>
}

@functions{
    Population[] values; // istatistik bilgilerin dizisi

    // Sayfamızın başlangıç aşamasında çalışan asenkron olay metodumuz
    protected override async Task OnInitAsync()
    {
        // GetJsonAsync metodunu kullanarak bir talep gönderiyoruz ve sunucu tarafından json dosyasını alıyoruz
        // Burada harici bir servis adresine de çıkılabilir
        //TODO: world.json içeriğini veren bir .net web api dahil edelim
        values = await httpClient.GetJsonAsync<Population[]>("db/world.json");
    }

    // Nüfus bilgilerini tutan sınıfımız
    class Population
    {
        public DateTime Date { get; set; }
        public Int64 Value { get; set; }
    }
}
```

Bu sayfa sembolik olarak üç günlük dünya nüfusu bilgilerini paylaşıyor. Tamamen kafadan uydurma bir örnek. Normal şartlarda nüfus bilgileri bir servis aracılığıyla çekilmekte. Bu noktada HttpClient hizmetinden yararlanmalıyız. Biz veri kaynağı olarak gerçek bir servisi kullanmak yerine sahte bir json içeriğini ele alıyoruz. wwwroot altında oluşturacağımız db klasöründe yer alan world.json dosyası bu noktada devreye giriyor. Ancak TODO kısmında belirttiğimiz üzere siz örneği geliştirirken bir Web API servisini kullanmayı deneyebilirsiniz.

```xml
[
    {
        "date": "2019-02-01",
        "value": 7644991666
    },
    {
        "date": "2019-02-02",
        "value": 7645213391
    },
    {
        "date": "2019-02-03",
        "value": 7645435108
    }
]
```

Pek tabii boilerplate etkisi ile üretilen projenin menüsü hazır şablona göre tesis edilmiş durumda. Burayı yeni eklediğimiz kendi sayfalarımıza göre düzenleyebiliriz. Tek yapmamız gereken NavMenu.cshtml dosyasını kurcalayarak aşağıdaki kıvama getirmektir. NavLink elementlerinde yeni eklediğimiz bileşenlerdeki @page direktiflerinde belirtilen URL adresleri kullanılmaktadır.

```text
<div class="top-row pl-4 navbar navbar-dark">
    <a class="navbar-brand" href="">HelloWorld</a>
    <button class="navbar-toggler" onclick=@ToggleNavMenu>
        <span class="navbar-toggler-icon"></span>
    </button>
</div>

<div class=@(collapseNavMenu ? "collapse" : null) onclick=@ToggleNavMenu>
    <ul class="nav flex-column">
        <li class="nav-item px-3">
            <NavLink class="nav-link" href="" Match=NavLinkMatch.All>
                <span class="oi oi-home" aria-hidden="true"></span> Başlangıç :P
            </NavLink>
        </li>
        <li class="nav-item px-3">
            <NavLink class="nav-link" href="population">
                <span class="oi oi-home" aria-hidden="true"></span> Dünya Nüfusu
            </NavLink>
        </li>
       <!-- 
        <li class="nav-item px-3">
            <NavLink class="nav-link" href="counter">
                <span class="oi oi-plus" aria-hidden="true"></span> Sayaç
            </NavLink>
        </li>
        -->
        <!-- Yeni eklediğimiz book sayfası için link. href değerine göre bookList.cshtml sayfasına yönlendirileceğiz -->
        <li class="nav-item px-3">
            <NavLink class="nav-link" href="bookList">
                <span class="oi oi-list-rich" aria-hidden="true"></span> Kitaplar
            </NavLink>
        </li>
    </ul>
</div>

@functions {
    bool collapseNavMenu = true;

    void ToggleNavMenu()
    {
        collapseNavMenu = !collapseNavMenu;
    }
}
```

## Çalışma Zamanı

Artık bir deneme sürüşüne çıkabiliriz. Uygulamayı terminalden aşağıdaki komutu vererek çalıştırmamız mümkün.

```bash
dotnet run
```

Örnek olarak bir iki kitap girip sonuçları inceleyebiliriz. Ben aşağıdakine benzer bir ekran görüntüsü yakalamışım.

![05_19_cover_2.png](/assets/images/2019/05_19_cover_2.png)

Çalışma zamanını incelerken F12 ile debug moda geçmekte yarar var. Söz gelimi booklist üzerinde çalışırken kitap ekleme ve listeleme gibi operasyonların gerçekleştirilmesine karşılık oluşan HTML kaynağı aşağıdaki gibidir. Standart üretilen HTML çıktılarından biraz farklı değil mi? MVC'de, eski nesil Server Side Web Forms'larda veya saf HTML ile yazdıklarımızda üretilen içerikleri düşünelim. Bir takım elementleri source üzerinde göremiyoruz gibi. Yine de sayfamız kanlı canlı bir şeyler yürütüyor. Derlenmiş bir uygulamanın tarayıcıda koştuğunu ifade edebiliriz.

![05_19_cover_3.png](/assets/images/2019/05_19_cover_3.png)

Built-In HttpClient servisini enjekte ettiğimiz dünya nüfus verileri sayfası ise şöyle görünecektir (Ekranı daraltmamıza rağmen UX deneyiminin bozulmadığını görmüşsünüzdür)

![cover_4.png](/assets/images/2019/cover_4.png)

## Ürünün Paketlenmesi

Bir Blazor uygulamasının dağıtımı için publish işlemine ihtiyacımız var. Visual Studio tarafında bu iş oldukça kolay. Microsoft Azure platformuna servis olarak da alabiliriz. WestWorld gibi Ubuntu tabanlı ortamlardaysa dağıtım işlemini dotnet komut satırı aracını kullanarak aşağıdaki terminal komutuyla gerçekleştirebiliriz.

```bash
dotnet publish -c Release
```

> Oluşan dosya içeriklerini incelemekte yarar var. publish operasyonu sırasında gereksiz kütüphaneler çıkartılıp paket boyutu mümkün mertebe küçültülüyor. Dikkat çekici nokta C# kodunun çalışması için gerekli ne kadar runtime bileşeni (mscorlib, mono runtime, c libraries vb) varsa mono.wasm içine konulması. WestWorld'teki örnek için bu 2.1 mb'lık dosya anlamına geldi.

Bunun sonucu olarak bin/Release/netstandard2.0/publish/ klasörü altına gerekli tüm proje dosyaları atılır. Bu dosyaları web sunucusuna veya bir host service'e alarak (manuel veya otomatik araçlar yardımıyla) uygulamayı canlı (production) ortama taşıyabiliriz.

## Ben Neler Öğrendim?

Blazor benim yeni yeni keşfetmeye, öğrenmeye ve anlamaya çalıştığım konulardan birisi. Yer yer huni takmama sebep olan iç mimarisi sebebiyle üstüne daha çok kafa patlatmam gerektiğiyse aşikar. Buna rağmen bu basit Hello World denemesi sırasında bile öğrendiğim bir kaç şey oldu. Bunları şöyle maddeleştirebilirim.

- Bir Blazor proje şablonunun temel bileşenlerinin ne olduğunu
- Blazor tarafında Bootstrap kullanarak daha şık tasarımlar yapılmasını
- Razor'da sayfa bileşenleri ile fonksiyonların nasıl etkileşebileceğini
- Blazor'daki Dependency Injection mekanizmasının nasıl ele alınabileceğini
- Bileşen odaklı bir geliştirme ortamı olduğunu
- Kabaca WASM terimini
- Blazor uygulamasının canlı ortamlar için publish edilmesini

Ve böylece geldik bir [Saturday-Night-Works](https://github.com/buraksenyurt/saturday-night-works) derlemesinin daha sonuna. Bir başka macerada görüşmek üzere hepinize mutlu günler dilerim.