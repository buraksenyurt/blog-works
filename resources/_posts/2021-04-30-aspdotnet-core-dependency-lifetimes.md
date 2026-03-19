---
layout: post
title: "Asp.Net Core - Dependency Lifetimes"
date: 2021-04-30 23:54:00 +0300
description: "Asp.Net 5 tarafında Dependency Injection ile ilişkili kafa karıştıran ve saha çözümlerinde dikkat gerektiren konulardan birisi de servis yaşam süreleridir(lifetimes) Bu kısa yazıda söz konusu meseleyi basit bir şekilde anlamaya çalışacağız. Örneğimiz bir önceki yazımızda da kullandığımız .Net çözümü üzerinde geliştirilecek. Dolayısıyla kodun detaylarına github adresinden bakabilirsiniz. Ben odaklanmamız gereken kısımları ve sonuçları paylaşmaya çalışarak bakmamız gereken alanı daraltmaya çalışacağım. Dilerseniz neler yaptığımız bir bakalım."
categories:
  - asp-dotnet-core
tags:
  - asp-dotnet-core
  - csharp
  - dotnet
  - aspnet
  - entity-framework
  - wcf
  - performance
  - generics
  - dependency-injection
  - github
  - dependency-management
---
# Asp.Net Core - Dependency Lifetimes
![hellomvc_11.png](/assets/images/2021/hellomvc_11.png)

Çalışmakta olduğum şirketin çok büyük bir ERP (Enterprise Resource Planning) uygulaması var. Microsoft.Net Framework 1.0 sürümünde düşünce olarak hayat geçirilip geliştirilmeye başlanmış. Milyonlarca satır koddan ve sayısız sınıftan oluşan, katmanlı monolitik mimari üstünde yürüyen, sahada on binden fazla personelin kullandığı çok etkili bir ürün. Geçtiğimiz yıl bu uygulamanın modernizasyonu kapsamında başlatılan IT4IT çalışmaları bünyesinde nesne bağımlılıklarının yönetimi için Dependency Injection mekanizmasının nimetlerinden de epeyce yararlanıldı. Doğruyu söylemek gerekirse koda yaptıkları dokunuşları hayranlıkla izledim.

Elbette başa dert olan ve sahada fark edilmesi güç bazı konular da gündeme gelmedi değil. Bunlarda birisi de bağımlı nesnelerin yaşam ömürleri ile alakalıydı. Gerçekten böylesine büyük bir sistemde AddTransient ile mi gitmeli yoksa AddScoped olarak mı bırakmalı gibi sorulara cevap vermek kolay değil. Öncelikle şu nesne yaşam ömrü meselesini anlamak gerekiyor. Bende hazır evden çıkmamız yasak kitaplarıma gömülmüşken bu meseleyi iyice bir öğreneyim istiyorum. Kapak fotoğrafı mı? Her zaman ki gibi konumuzla bir alakası yok. Sadece yazıyı yazarken dinlemekte olduğum Bon Jovi'nin 1984 çıkışlı stüdyo albümüne ait:D

Aslında Asp.Net 5 açısından bakıldığında da Dependency Injection ile ilişkili kafa karıştıran ve saha çözümlerinde dikkat gerektiren konulardan birisi servis yaşam süreleri (Hoş,.Net Remoting ve WCF tarafındaki nesne yaşam döngülerini düşününce nispeten çok daha kolay bir konu) Bu kısa yazıda söz konusu meseleyi öğrendiğim kadarıyla sizlere anlatmaya çalışacağım. Örneğimiz [bir önceki yazıda](Asp.Net Core - Dependency Injection Türleri.md) da değindiğimiz.Net çözümü (hands-on-aspnetcore-di) üzerinde koşuyor olacak. Ayrıca kodun detaylarına [github adresinden](https://github.com/buraksenyurt/hands-on-aspnetcore-di/tree/lifetimes) bakabilir ve eksik kısımları tamamlayabilirsiniz. Ben odaklanmamız gereken yerleri ve sonuçları paylaşmaya çalışarak bakmamız gereken alanı daraltmak niyetindeyim. Her şeyden önce senaryomuza bir göz atalım (Taslak çizimin kusurlarını lütfen mazur görün)

![hellomvc_10.png](/assets/images/2021/hellomvc_10.png)

Anlamsız bir model ancak nesne yaşamlarını öğrenmek için hem kitaplarda hem de internet kaynaklarında kullanılan yaygın bir yöntemi değerlendireceğiz; Guid tipi yardımıyla hayattaki nesnelerin takibi. Senaryomuzda GameController tipinin bağımlı olduğu dört farklı bileşen var. Bu bağımlılıklar IGameRepository, IPartRepository, IShopRepository ve arayüzleri üstünden gelen sınıflar ile PerformanceCounter tipi. İşin ilginç yanı PerformanceCounter sınıfının da IGameRepository, IPartRepository ve IShopRepository referansları üzerinden gelen bileşenlere bağımlılığı var. Bu kurguda amaç, çalışma zamanında DI Container servislerine kayıt edilen IGameRepository, IPartRepository ve IShopRepository türevlerinin, PerformanceCounter içerisine alınırken farklı yaşam süresi seçimlerine göre nasıl tepki geliştirdiklerini öğrenmek.

Dependency Injection servis koleksiyonuna kayıt edilen bileşenler için normal şartlarda üç tip yaşam ömrü seçeneği bulunuyor. Transient, Scoped ve Singleton. Genellikle konuya yabancı olan ben gibiler kolaya kaçıp Transient seçeneğini tercih ediyor. Fakat duruma göre uygun olan modeli belirlemek lazım. Örneğin Entity Framework tarafına ait DbContext servisi kayıt edilirken neden Scoped olarak dahil ediliyor? Peki ya ILogger'ın varsayılan ömrü neden Singleton? Dolayısıyla aradaki farkları anlamamız önemli.

DI Container'a Scoped türünde kayıt edilen bir servis her web talebi için yeniden örnekleniyor. Singleton modelinde ise servis bir kere örnekleniyor ve uygulama (Web App) ayakta kaldığı sürece yaşamaya devam ediyor. Dolayısıyla onu çözümleyen (Resolve) bileşenler hep aynı nesne örneğini kullanıyorlar. Son olarak Transient seçeneğinde, bağımlı bileşen her nerede çözümlenirse çözümlensin hep yeni bir örneği oluşturularak kullanılıyor.

İyi güzel hoş ama bunu canlı bir örnekle nasıl analiz ederiz? Yukarıdaki şekle göre gerekli kodlarımızı yazmaya başlayım. IGameRepository, IShopRepository ve IPartRepository arayüzleri Guid tipinden birer özellik sunuyorlar. Bu Guid'leri onları uygulayan asıl bileşenlerin (Concrete Instance) çalışma zamanındaki takibini yapmak için kullanacağız. IShopRepository ve ShopRepository tiplerinin içeriğini aşağıda bulabilirsiniz. Diğerleri de benzer bir düzeneğe sahipler.

```csharp
using C64Portal.Models;
using System;

namespace C64Portal.Data
{
    public interface IShopRepository
    {
        public Guid InstanceID { get; set; }
        void Sell(Game game,decimal offer);
    }
}
```

ve onu uygulayan asıl sınıf (Concrete Class).

```csharp
using C64Portal.Models;
using C64Portal.Queue;
using System;
using System.Collections.Generic;

namespace C64Portal.Data
{
    public class ShopRepository
        : IShopRepository
    {
        public Guid InstanceID { get; set; }
        public ShopRepository() :this(Guid.NewGuid())
        {
        }
        public ShopRepository(Guid instanceID)
        {
            InstanceID = instanceID;
        }

        public void Sell(Game game, decimal offer)
        {
            // Do Something
        }
    }
}
```

İşe yarayan bir fonksiyon yok ancak yapıcı metodun (constructor) nasıl kullanıldığı bizim için önemli. ShopRepository sınıfına ait bir nesne örneklenirken yeni bir Guid oluşturuyoruz. Varsayılan yapıcı metot, DI kayıt işlemi (Register) sırasında gerekli olduğu için çağrıldığında parametre ile donatılan diğer yapıcı metodu tetikliyor. Doğal olarak seçilen lifetime kriterine göre takip edeceğimiz benzersiz bir değere sahip olmuş olacağız. Diğer arayüz ve uyarlamalarını yazdıktan sonra PerformanceCounter sınıfını da aşağıdaki gibi geliştirebiliriz.

```csharp
using System;

namespace C64Portal.Data
{
    public class PerformanceCounter
    {
        public Guid ShopRepositoryID { get; set; }
        public Guid GameRepositoryID { get; set; }
        public Guid PartRepositoryID { get; set; }
        private readonly IGameRepository _gameRepository;
        private readonly IShopRepository _shopRepository;
        private readonly IPartRepository _partRepository;
        public PerformanceCounter(IGameRepository gameRepository, IShopRepository shopRepository, IPartRepository partRepository)
        {
            _gameRepository = gameRepository;
            _shopRepository = shopRepository;
            _partRepository = partRepository;
            GameRepositoryID = _gameRepository.InstanceID;
            ShopRepositoryID = _shopRepository.InstanceID;
            PartRepositoryID = _partRepository.InstanceID;
        }
        public void CalculateMemoryUsage()
        {
            //Do Something
        }
    }
}
```

İğrenç bir sınıf değil mi?:D Ancak yapıcı metoda yine dikkat edelim. Sınıfın bağımlı olduğu bileşenler, tasarladığımız arayüzler üzerinden çözümlenerek içeri alınıyor ve gelen nesne örneklerinin Guid tipli özelliklerinin herbiri için ayrılmış alanlara atanıyorlar. Şimdi de GameController içeriğini aşağıdaki gibi değiştirelim.

```csharp
using C64Portal.Data;
using C64Portal.Models;
using C64Portal.Queue;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace C64Portal.Controllers
{
    public class GameController : Controller
    {
        private readonly IGameRepository _gameRepository;
        private readonly IShopRepository _shopRepository;
        private readonly IPartRepository _partRepository;
        private readonly PerformanceCounter _performanceCounter;
        private readonly ILogger<GameController> _logger;
        public GameController(
            IGameRepository gameRepository
            ,IShopRepository shopRepository
            ,IPartRepository partRepository
            , PerformanceCounter performanceCounter
            , ILogger<GameController> logger)
        {
            _logger = logger;
            _performanceCounter = performanceCounter;
            _gameRepository = gameRepository;
            _shopRepository = shopRepository;
            _partRepository = partRepository;
        }
        public IActionResult Index()
        {
            _logger.LogInformation($"\n[SINGLETON]\tShopRepo ID:{_shopRepository.InstanceID},In Perf Counter:{_performanceCounter.ShopRepositoryID}");
            _logger.LogInformation($"\n[TRANSIENT]\tGameRepo ID:{_gameRepository.InstanceID},In Perf Counter:{_performanceCounter.GameRepositoryID}");
            _logger.LogInformation($"\n[SCOPED   ]\tPartRepo ID:{_partRepository.InstanceID},In Perf Counter:{_performanceCounter.PartRepositoryID}");

            var games = _gameRepository.GetAllGames();
            _performanceCounter.CalculateMemoryUsage();
            return View(games);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Game game)
        {
            _gameRepository.Publisher = new RabbitPublisher();
            _gameRepository.Create(game);
            return RedirectToAction("Index");
        }
    }
}
```

GameRepository'dekine benzer bir durum burada da söz konusu. Sadece fazladan PerformanceCounter ve ILogger bağımlılıkları da var. Lakin fazladan dediğimiz PerformanceCounter kullanımı önemli. Web uygulaması çalıştığında GameController tipi her ne zaman çağırılırsa yapıcı metodu sebebiyle DI Container'dan IGameRepository, IShopRepository, IPartRepository ve PerformanceCounter referansları isteyecek. Bu da asıl sınıfların örneklendiği (Constructor'ların tetiklenmesi) ya da örneklenmeyip örneklenmiş olanların verildiği bir operasyon anlamına geliyor. Diğer yandan PerformanceCounter'ın çağırılması halinde onun da istediği IGameRepository, IPartRepository ve IShopRepository referansları var. PerformanceCounter sınıfı bunları da DI Container'dan isteyecek (Hatta onu bilerek AddTransient olarak kayıt edeceğiz ki her örneklendiğinde DI'dan diğer arayüz referanslarını istesin) İşte bu ikinci isteklerde söz konusu servislerin hangi yaşam döngüsü seçeneğine göre kaydedildiği önem kazanıyor. Diğer yandan ufak bir detay ama Index isimli Action içerisinde bir Log yayınladığımızı da fark etmiş olmalısınız. Loglamayı, Controller'a gelindiğinde ve Index fonksiyonu çağırıldığında oluşan bağımlı bileşenlerin güncel Guid değerlerini kaydetmek için kullanıyoruz. Bu arada tüm bileşenlerin Constructor Injection tekniği ile çözümlendiğine dikkat edin ve başka hangi tekniklerden bahsetmiştik hatırlayın.

> Bu arada loglamayı dilerseniz fiziki olarak bir Text dosyasına da yapabilirsiniz. Ben bunun için Serilog.Extensions.Logging.File isimli Nuget paketini projeye ekledim ve Startup sınıfındaki Configure metodunu da aşağıdaki gibi değiştirdim.
> ```csharp
> public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
>  ILoggerFactory loggerFactory)
> {
> 	var path = Directory.GetCurrentDirectory();
> 	loggerFactory.AddFile($"{path}\\Logs\\Log.txt");
> ```

Gelelim bileşenlerin DI Servis kataloğuna kayıt edilmesine ki burası yazımızın dönüm noktası. Bunun için Startup sınıfındaki ConfigureServices metodunu aşağıdaki gibi kullanabiliriz.

```csharp
public void ConfigureServices(IServiceCollection services)
{
	services.AddControllersWithViews();

	services.AddTransient<IGameRepository, GameRepository>();
	services.AddScoped<IPartRepository, PartRepository>();
	services.AddSingleton<IShopRepository, ShopRepository>();
	services.AddTransient<PerformanceCounter>();

	services.AddTransient<DataCollectorService>();
}
```

IGameRepository üstünden bağlanan GameRepository, AddTransient fonksiyonu ile eklenmiş durumda. Buna göre kendisine her ihtiyaç duyulduğunda tekrardan örneklenecek. Yani onun adına hep yeni bir Guid değeri görmemiz gerekiyor. PartRepository sınıfı ise AddScoped metodu ile dahil edilmiş durumda. Buna göre aynı Scope içerisinde kalındığı sürece hem Controller hem de PerformanceCounter'da tekil bir PartRepository nesnesinin kullanılmasını bekliyoruz. Ta ki farklı bir scope'a geçip tekrar buraya dönene kadar (Bunu diğer bir Controller'a geçip geri gelerek kontrol edebiliriz) Son olarak sıra AddSingleton ile eklenen ShopRepository nesnesinde. Buna göre web uygulaması çalıştığı sürece, sayfa yenilense (Örneğin F5 ile) ya da farklı Controller ve Action metotları çalışsa bile, uygulama yeniden başlatılıncaya kadar tek bir ShopRepository örneğinin kullanılıyor olması lazım.

Bu aşamaya geldiyseniz uygulamayı çalıştırıp logları takip etmeniz yeterli. Ben örneğin çalışma zamanına ait iki ekran görüntüsü bırakmak istiyorum. İlki komut satırından yürütülen çalışma zamanına ait. Console penceresine düşen logları görebilirsiniz (O değilde kopyala yapıştırın acı bir sonucu var burada. İki tane Prince of Persia eklenmiş yahu)

![hellomvc_8.png](/assets/images/2021/hellomvc_8.png)

Paylaşmak istediğim diğer görüntü ise Guid bilgilerini topladığım Excel'e ait.

![hellomvc_9.png](/assets/images/2021/hellomvc_9.png)

Guid değerlerinin hangi durumda nasıl farklılaştığını görebiliyor musunuz? Bir nesnenin hangi aksiyonda nasıl davranış sergilediğini anlamak oldukça kolay. ShopRepository sisteme Singleton modelde alındığı için hangi aksiyon olursa olsun üretilen Guid hep aynı kalmakta. Sayfa yenilense de scope değişse de fark etmiyor. Yani GameController için de, onun içinden çağırılan PerformanceCounter için de aynı nesne kullanılıyor ve sayfa yenilense bile bu nesne yaşamaya devam ediyor. Lakin PartRepository nesnesine ait Guid bilgisi gerçekleşen aksiyon bazında değişmiş görünüyor. Fakat bir fark var. Aynı Scope'a dahil olan PerformanceCounter'da aynı PartRepository nesne örneğini kullanıyor. Bu nedenle Guid aksiyon bazında aynı kalmış halde. Bu noktada Scoped tekniğinin, Singleton ile sürekli olarak karıştırıldığını ifade edebilirim. Biri uygulama ayakta kaldığı müddetçe aynı kalırken diğeri sadece ortak Scope'a dahil olan farklı aksiyonlar boyunca aynı kalıyor. O nedenle yapılan her aksiyonda yeni bir PartRepository örnekleniyor ve hem GameController hem PerformanceCounter bu aynı nesneyi kullanıyor. GameRepository ise oldukça şımarık:) Aksiyon ne olursa olsun hep yeni bir Guid oluşmuş görünüyor; GameController tarafında da PerformanceCounter tarafında da.

İşte bu kadar:)

Bu örnekle bağımlı bileşenlerin nesne ömürlerinin nasıl şekillendiği kafamda biraz daha netleşmiş oldu. Elbette gerçek hayat senaryolarında bu seçimler oldukça kritik öneme sahip. Tüm uygulama yaşamı boyunca yaşayacak bir nesne örneği her ne kadar cazip görünse de bellek tüketiminin bir anda artmasına sebebiyet verebilir. Ya da web talebi için bir nesne örneklenmesi, ilk oluşturma maliyeti yüksek olan bileşenler düşünüldüğünde performans kaybına neden olabilir. Her nesne gerektiğinde yeni bir örnek oluşturulması basit bir seçim gibi dursa da network trafiğinin aşırı derecede artmasına sebebiyet verebilir. Kaynaklar en kötü karar bile kararsızlıktan iyidir felsefesini benimseyerek AddTransient olarak ilerleyin diyor. Bense vakaya göre seçim yapmamız gerekiğini düşünüyorum (It depends hali). Notlarıma burada son vermeden önce araştırmanız için iki konu bırakıyorum.

- Sizce bir arayüz üstünden n sayıda bağımlı bileşeni DI Container servisine kayıt edebilir miyiz?
- Çalışma zamanının herhangi bir noktasında DI Container'a kayıt edilmiş servisleri tek tek veya toplu olarak silebilir miyiz?

Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize sağlıklı, huzur dolu günler dilerim.