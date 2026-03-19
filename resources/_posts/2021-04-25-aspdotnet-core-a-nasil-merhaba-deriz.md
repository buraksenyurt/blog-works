---
layout: post
title: "Asp.Net Core'a Nasıl Merhaba Deriz?"
date: 2021-04-25 09:00:00 +0300
description: "Yazılım geliştirme işine ciddi anlamda başladığım yeni milenyumun başlarında .Net Framework sahanın yükselen yıldızıydı. C# programlama dilinin gücü ve .Net Framework çatısının vadettikleri düşünülünce bu son derece doğaldı. Aradan geçen neredeyse 20 yıllık süre zarfında .Net Framework'te evrimleşti. Microsoft'un açık kaynak dünyasına girişi, cross-platform stratejileri, Linux gibi bir zamanların ciddi rakipleri ile el sıkışarak hamle yapması sonrasında da son beş yıllık zaman diliminde .Net Core hayatımıza girdi. Bu beraberinde Microsoft'un sıklıkla yaptığı üzere bazı kavram karmaşalarını da beraberinde getirdi. En nihayetinde tek ve birleşik bir .Net 5 ortamından bahsedilmeye başlandı..."
categories:
  - dotnet-core
tags:
  - dotnet-core
  - bash
  - csharp
  - dotnet
  - aspnet
  - aspnet-mvc
  - asp-dotnet-core
  - entity-framework
  - ef-core
  - linq
  - wcf
  - workflow-foundation
  - web-api
  - blazor
  - generics
  - dependency-injection
  - visual-studio
  - github
  - dependency-management
---
# Asp.Net Core'a Nasıl Merhaba Deriz?
![hellomvc_cover.png](/assets/images/2021/hellomvc_cover.png)

Yazılım geliştirme işine ciddi anlamda başladığım yeni milenyumun başlarında.Net Framework sahanın yükselen yıldızıydı. Delphi’den kopup gelen Anders’in yarattığı C# programlama dilinin gücü ve.Net Framework çatısının vadettikleri düşünülünce bu son derece doğaldı. Aradan geçen neredeyse 20 yıllık süre zarfında.Net Framework’te evrimleşti ve sürekli güncellendi. Versiyon 2.0 ile gelen generic tipler, 3.0'la birlikte SQL yazar gibi sorgulanabilir nesneler (LINQ-Language INtegrated Query), sonrasında karşımıza çıkan WCF (Windows Communication Foundation), WF (Workflow Foundation), Entity Framework vs derken Microsoft’un açık kaynak dünyasına girişi, benimsediği platform bağımsız stratejiler (Miguel De Icaza’nın Mono’suna da saygı duyalım), Linux, MacOS gibi bir zamanların ciddi rakipleri ile el sıkışarak hamle yapması sonrasında da son birkaç yıllık zaman diliminde karşımıza çıkan.Net Core. Yeni gelişmeler Microsoft’un sıklıkla yaptığı üzere bazı kavram karmaşalarını da beraberinde getirdi elbette. En nihayetinde tek ve birleşik bir.Net 5 ortamından bahsedilmeye başlandı. (Photo by [Element5 Digital](https://unsplash.com/@element5digital?utm_source=unsplash&utm_medium=referral&utm_content=creditCopyText) on [Unsplash](https://unsplash.com/s/photos/education?utm_source=unsplash&utm_medium=referral&utm_content=creditCopyText))

Gelişmeleri zaten sizler de benim gibi takip ediyorsunuzdur. Bu durum benim de kişisel olarak kendimi yenilemem gereken bir dönemi tetikledi. Bir süredir özellikle Amazon’dan getirttiğim kitaplardan.Net 5 dünyasını tanımaya,.Net 4.7.2 gibi versiyonlarda yazılmış uygulamarı yeni sürüme göç ettirmenin (migration) yollarını öğrenmeye çalışıyorum. Bu kişisel çabayı da çalışmakta olduğum şirketin iç eğitim programından gelen talepleri karşılamak için kullanıyorum.

Sabahsız gecelerimin birisinde zen merkezim olan çalışma odamdaki kanepeya uzanmış boş boş tavana bakıyordum. Hoş aklımda cevap arayan güzel bir soru da vardı. [Geleceğe Giriş](https://www.gelecegegiris.com) programı kapsamındaki bir Asp.Net Core eğitimine nasıl başlamalıydım? Nasıl bir Hello World olmalıydı? Doğrudan üretilecek uygulamanın kendisini en başından gösterip; “İşte bu uygulamayı nasıl yazacağımızı adım adım öğreneceğiz” şeklinde mi yol almalıydım. Yoksa Hello World deme şekli.Net Core sonrası daha mı farklıydı?

Bu işe başladığım yıllarda beni eğitenler veya okuduklarım Nesne Yönelimli Dil (Object Oriented Programming) konusunun ne denli önemli olduğundan bahseder, kalıtım (Inheritance), çok biçimlilik (Polymorphism) ve soyutlama (Abstraction) gibi kavramların önemine vurgu yapardı. İş, düşük maliyeti nedeniyle çok sık tercih edilen monolitik mimarinin en yaygın kullanılan örneklerinden olan çok katmanlı (n-tier) çözümlere geldiğinde ise mahşerin beş atlısı SOLID ilkeleri, sayısız yazılım prensibi ve tasarım kalıbı ile karşılaşırdık. Gerçekten yazılım mühendisliğinden bahsettiğimiz noktaya gelindiğinde ise Autofac, Ninject, Unity, Castle Windsor gibi bileşenler arası bağımlılıkları yöneten çatıları kullanmaya başlardık. O günleri düşünürken aklıma.Net Core'u (esasında.Net 5'i) bu bağlamda ele almak geldi. Çok üst düzey yetenekleri olmasa da zaten dahili bir DI (Dependency Injection) mekanizmasına sahipti.

Belki sadece DI deyip geçtiğimiz ve bazen şuursuzca IServiceCollection üzerinden bağımlıkları kayıt etmemize olanak sağlayan bu kavram esas itibariyle Single Responsibility, Dependency Inversion Principle ve Inversion of Control esasları üzerine oturuyor. Bu sebepten basit bir Asp.Net Core eğitimine başlarken bile sadece Model nesnesi oluşturup bir liste döndüren Controller ile View kullanmak kafi olmayabilir. Öncesinde ve mutlak suretle eğitimdeki değerli zihinlere.Net Core'un DI mekanizmasının nasıl çalıştığını, neden önemli olduğunu göstermek gerekir...

Diye notlar alarak geçmişim bu yazının başına. Amacım, eğitim için basit ve hızlı okunabilir bir ön doküman hazırlamaktı. Bu dokümanı eğitim katılımcılarına gönderip, "şuna bir göz atın, anlamaya çalışın, sondaki sorulara cevaplar bulun ve derse öyle gelin" demekti belki de. Sonunda aşağıdaki içeriğe sahip basit bir rehber ortaya çıktı (Level 101 diyebiliriz)

Hello World'ler artık bildiğim Hello World'ler gibi değiller.

## Sıfır Noktası

Şu bir gerçek ki, Asp.Net Core tarafında kullanılan MVC, Razor, Blazor, Web API vb uygulama tipleri ile bunların sıklıkla kullandığı Hosting, Routing, Logging, Configuration, ApplicationLifetime gibi servisler doğrudan Microsoft.Extensions.DependencyInjection yapısı üzerine oturuyorlar (Bu arada Microsoft.Extensions.DependencyInjection kütüphanesinin harici olarak da kullanılabilen bir NuGet paketi olduğunu ve bu sepele bir Console uygulamasında dahi DI mekanizmasını kullanabilmemize olanak sağladığını da hatırlatalım) Onlar için ekstra bir çaba sarf etmeden daha çalışma zamanı ayağa kalkarken sisteme dahil ediliyorlar. Aslında yine kavramlar arasında kayboluyor gibiyiz. Belki de DI kullanmadığımız bir örnekteki basit kusuru görmeye çalışırsak daha iyi olur. DI demişken bu kısaltmanın adını duymuş olmalısın; Dependency Injection! Bu terime alışsan iyi olur, nitekim şirketin temel ilkelerinden birisi de onunla iyi geçinmek. Ancak öncesinde sana problemi göstermem lazım. Yazılımcıların pek de sevmediği bir durum. Tightly-Coupled (birbirine sıkı sıkıya bağlı) olma hali. Haydi gel, bir örnekle durumu açıklayalım.

Sisteminde.Net 5 yüklüğü olduğunu varsayıyorum. Hangi platformda olduğunun çok da önemi yok. Bir Terminal penceresi aç ve aşağıdaki komutu işleterek basit bir MVC projesi oluştur.

```bash
dotnet new mvc -o FunnyHello
```

## Masum Kodlar Basamağı

Sonrasında Visual Studio Code, Visual Studio 2019 Community Edition veya muadili bir IDE ile projeni aç. Model klasöründe aşağıdaki içeriğe sahip Game isimli bir sınıf oluştur ve ilk kodlarını yazmış ol. Sen ve arkadaşlarının sevdiği oyunların isimlerini ve liste fiyatlarını tutacağımız basit bir nesne bu aslında.

```csharp
namespace FunnyHello.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal ListPrice { get; set; }
    }
}
```

Başka bir zaman diliminde onu Entity Framework Core üstünden SQL ile ya da Azure Cosmos Db ile veya istediğin başka bir Repository ile ilişkilendirebilirsin. Şimdilik Web uygulaması alanında dolaşımda olacak ve kullanıcının göreceği sayfayı kurgulayan View nesnesi için anlam ifade eden bir model şablonu olduğunu söylesek yeterli. Sıradaki adımımız Data isimli bir klasör oluşturmak ve içerisine aşağıdaki içeriğe sahip GameRepository sınıfını yerleştirmek.

```csharp
using FunnyHello.Models;
using System.Collections.Generic;

namespace FunnyHello.Data
{
    public class GameRepository
    {
        public List<Game> GetAllGames()
        {
            return new List<Game>
            {
                new Game{ Id=1, Title="Commandos II",ListPrice=10.5M },
                new Game{ Id=2, Title="Prince of Persia",ListPrice=9.45M },
                new Game{ Id=3, Title="Prince of Persia",ListPrice=9.45M }
            };
        }
    }
}
```

Bizi sonuca götürecek, görsel ortamda birkaç veriyi kullanmamızı sağlayan aptalca bir sınıftan başka bir şey değil ama senaryo için yeterli. Şu ana kadar seni zorlayan pek bir şey olmadığı düşüncesindeyim. Haydi o zaman devam edelim. Madem Model View Controller türevli bir Web uygulaması geliştiriyoruz, View ile Model arasındaki iletişim Controller sınıfının görevi olmalı. Öyleyse hali hazırda var olan Controllers klasörüne GameController isimli yeni bir sınıf ekle ve kodlamasını aşağıdaki gibi yaparak devam et. MVC ve detayları içinse üzülme. Eğitim sırasından ondan da bolca bahsedeceğiz.

```csharp
using FunnyHello.Data;
using Microsoft.AspNetCore.Mvc;

namespace FunnyHello.Controllers
{
    public class GameController : Controller
    {
        public IActionResult Index()
        {
            GameRepository gameRepository = new GameRepository();
            var games = gameRepository.GetAllGames();
            return View(games);
        }
    }
}
```

Gayet prüzsüz bir sınıf. Controller türevli olması bir yana dursun Index isimli fonksiyon (ki uygulama için çağırılabilir bir Action anlamına geliyor) GameRepository sınıfını kullanarak oyun listesini alıp kendisi ile ilişkili olan View'a gönderiyor. Hangi View'a gideceğini nereden mi biliyor? Hımmm...Bunu bir düşünelim. GameController'ın Controller kelimesini çıkarırsak geriye Game kalıyor. View tarafında da Game isimli bir klasör olur ve içinde Index isimli bir sayfa olursa sanırım otomatik bir yönlendirme düzeneği tesis edilmiş olur. Aynı varsayılan şablonla gelen HomeController ve View/Home alıntdaki Index.cshtml düzeneğinde olduğu gibi. O halde sıradaki görevin belli. View klasörüne geçip Game isimli yeni bir klasör oluştur ve altına aşağıdaki içeriğe sahip Index.cshtml dosyasını ekle.

```text
@model IEnumerable<Game>

<div>
    <h1>Tüm Oyunlar</h1>
    <hr />
    <table>
        <thead>
            <tr>
                <th>Id</th>
                <th>Title</th>
                <th>List Price</th>
            </tr>
        </thead>
        @foreach (var g in Model)
        {
            <tr>
                <td>@g.Id</td>
                <td>@g.Title</td>
                <td>@g.ListPrice</td>
            </tr>
        }
    </table>
</div>
```

Belki kafana takılan bazı sorular olabilir. Neden başlangıça @model diye bir direktif var? Bu sayfa ile arka plandaki nesneler arasında gerekli olan bağlantı nasıl gerçekleşiyor? for döngüsünü biliyorum lakin buradaki kullanım tüm oyun listesini dolaşmak için mi acaba? ve benzerleri. Lütfen sabırlı ol. Amacımız şimdilik bu detaylarla ilgili değil. Minik bir parça daha ekleyelim. Shared klasöründeki _Layout.cshtml sayfasını bul ve içerisine aşağıdaki kod parçasını ekle.

```text
<li class="nav-item">
   <a class="nav-link text-dark" asp-area="" asp-controller="Game" asp-action="Index">Games</a>
</li>
```

Nereye koyman gerektiğini söylemiyorum ancak basitçe bulacağından eminim;) Tahmin edeceğin üzere yeni bir menü öğesi yerleştirdik ve ona basılınca hangi Controller nesnesinin hangi Action üyesinin tetiklenmesi gerektiği ifade ettik. Eğer hazırsan terminalden dotnet run komutunu vererek ya da Visual Studio ortamındaysan F5 tuşuna basarak örneği çalıştırabilirsin. Aşağıdaki ekran görüntüsündekine benzer bir sonuç elde etmeni bekliyorum.

![hellomvc_1.png](/assets/images/2021/hellomvc_1.png)

Nasıl? Hiç yoktan iyidir değil mi? Mesela oyun bilgilerinin veritabanından geldiğini düşün. Hatta yeni oyun ekleme, fiyat değiştirme, oyunlara kapak fotoğrafları ekleme, yorum alma ve puan verme gibi kullanıcı etkileşimi yüksek fonksiyonellikler dahil ettiğini düşün. Hatta önyüz tarafında hazır Bootstrap çatısını kullanarak makyaj yaptığını ve albenisi yüksek, moda tabirle UX (User Experience) açısından zengin bir uygulama inşa ettiğini. Etkileyici bir Web uygulaması ortaya koymamız işten bile değil:) Ama ortada bir sorun var gibi.

## Problem Ne?

Şu anda bir MVC uygulamasına Hello World demiş olduğumuzu sanabilirsin. Biraz üstünde durup düşününce, Controller sınıfının ne yaptığını, View bileşeninin bir Action ile nasıl ilişkilendiğini ve kendisi ile alakalı model nesnelerini nasıl kullandığını anlamış olabilirsin. Ne var ki uygulama şirketimizde çalışan yazılımcıların rahatsız olacağı bir kod parçası içeriyor. Biraz düşünüp neresi olduğunu bulmak ister misin? Arzu edersen bunu bir kahve molası eşliğinde daha da derinlemesine düşünebilirsin.

![matt-hoffman-ZUUsGnG5zwc-unsplash.jpg](/assets/images/2021/matt-hoffman-ZUUsGnG5zwc-unsplash.jpg)

Photo by [Matt Hoffman](https://unsplash.com/@__matthoffman__?utm_source=unsplash&utm_medium=referral&utm_content=creditCopyText) on [Unsplash](https://unsplash.com/s/photos/coffee-break?utm_source=unsplash&utm_medium=referral&utm_content=creditCopyText)

Tekrar hoşgeldin;) GameController içerisindeki aşağıdaki kullanıma odaklanmalısın. Bu kullanım yazılımcıların hoşuna gitmez. Gelecek ile ilgili endişeler duymalarına sebebiyet verir.

```csharp
GameRepository gameRepository = new GameRepository();
```

Bunda ne sorun olabilir ki dediğini duyar gibiyim. Aynen bana da öğretildiği üzere oyun nesnesi üstünde temel CRUD (Create Read Update Delete) operasyonlarını üstelenen ve dolayısıyla sadece bu sorumluluğu üstüne alan bir sınıfın nesne örneğini alıp güzelce kullandın. Bir şekillde ifade etmek istersek kabaca aşağıdaki gibi bir durumun söz konusu olduğunu ifade edebilirim (Ve lütfen çizimimin kötü olmasına aldırma)

![hellomvc_2.png](/assets/images/2021/hellomvc_2.png)

İşte o terim; Tightly-Coupled! Yine karşımıza çıktı:D Sorun, GameController nesnesinin GameRepository sınıfını doğrudan kullanması. Bu sıkı bir arkadaşlığın göstergesi gibi. Ancak uygulama kodları arttıkça ve proje ister istemez büyüdükçe GameRepository nesnesinin farklı yerlerde kullanımı da söz konusu olacak. Ya Low-level bileşen olarak ifade edilen GameRepository'nin (yapması gereken işle ilgili kaynaklara doğrudan erişip karmaşık bir şeyler yapan nesne) işleyişi farklılaşır veya adı değişirse? Ya onu kullanan bir test metodunda gerçekten veritabanına gitmeden sırf test senaryosunun kalanını işletmek için hayali bir Game listesi döndürmesi istenen bir fonksiyon gerekirse? Mesela GameRepository, GameController'a küser ve fonksiyonunu kaldırırsa:P İşin şakası bir yana GameController'ın çalışması ve oyuncu listesini View'a vermesi, GameRepository'nin ellerindedir. Bu sıkı bağımlı bileşkeler GameRepository'yi başka bir şeyle değiştirmeyi zorlaştırır.

## Nasıl Çözeriz?

Sanırım sorun kısmen de olsa anlaşıldı. Bu ikili arasındaki sıkı dostluğa lafımız yok ama ilişkilerine bir mesafe koymalarında yarar var. Peki ya bunu nasıl sağlarız? Aşağıdaki şekle bakmadan biraz düşün derdim ama şu anda onu gördüğünü biliyorum:)

![hellomvc_3.png](/assets/images/2021/hellomvc_3.png)

Yapılması gereken GameController'ı GameRepository sınıfından koparmak ve aradaki ipleri gevşetmek (Loosely-Coupled ilkesini sağlanması) Bir başka deyişle, High-Level Component olan GameController ile asıl işi yapan Low-Level Component GameRepository arasına soyut bir katman (abstraction layer) koymak. Asıl işi yapan sınıfın detaylarını umursamayan ve asıl işi yapan sınıfın yaptığı işe ihtiyaç duyan GameController sınıfının isteklerine elçi olan. Ayrıca oyunun kurallarını bir sözleşme ile belirleme ve gerçekten de Controller'ın ihtiyacı olan fonksiyonları verecek asıl nesneyi kullandırma imkanına sahip olacağız. Nesne yönelimli diller açısından baktığımızda bunun en pratik yolu Interface tipini kullanmak. Şimdi üstünlüğü ele geçirelim. Yine Data klasörü altına geç ve IGameRepository isimli aşağıdaki arayüzü ekleyerek çalışmana devam et.

```csharp
using FunnyHello.Models;
using System.Collections.Generic;

namespace FunnyHello.Data
{
    public interface IGameRepository
    {
        IEnumerable<Game> GetAllGames();
    }
}
```

GameRepository sınıfını bu arayüzden türet (Belki bir dönüş tipi düzeltmesi de yapman gerekebilir) Aynen aşağıdaki kod parçasında olduğu gibi.

```csharp
using FunnyHello.Models;
using System.Collections.Generic;

namespace FunnyHello.Data
{
    public class GameRepository
        :IGameRepository
    {
        public IEnumerable<Game> GetAllGames()
        {
            return new List<Game>
            {
                new Game{ Id=1, Title="Commandos II",ListPrice=10.5M },
                new Game{ Id=2, Title="Prince of Persia",ListPrice=9.45M },
                new Game{ Id=3, Title="Prince of Persia",ListPrice=9.45M }
            };
        }
    }
}
```

Güzellll! Gayet iyi gidiyorsun. Artık GameController sınıfına geçebilir ve GameRepository yerine eklediğimiz soyutlamayı kullanmasını sağlayabilirsin. Bunun için GamesController sınıfının ilgili interface tipi ile çalışmasını sağlaman lazım. Bildiğin gibi bir interface aslında soyutlama için kullanılan bir sözleşmedir (Contract) ve sadece çağırılacak asıl nesnenin içindeki fonksiyonların neler olduğunu GamesController'a söylemekle yükümlüdür. Şunu da biliyorsun ki Interface gibi arabulucu sözleşmeler sınıflar gibi örneklenip kullanılamazlar (new operatörü ile onları örnekleyemezsin) ama nesne referansı taşıyabilirler;) Belki de onu Controller sınıfına Constructor metot üstünden alıp kullanabiliriz;)

```csharp
using FunnyHello.Data;
using Microsoft.AspNetCore.Mvc;

namespace FunnyHello.Controllers
{
    public class GameController : Controller
    {
        private readonly IGameRepository _gameRepository;
        public GameController(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }
        public IActionResult Index()
        {
            //GameRepository gameRepository = new GameRepository();
            var games = _gameRepository.GetAllGames();
            return View(games);
        }
    }
}
```

Harika! Sonuca çok yaklaştın. Haydi uygulamayı tekrar çalıştırda, her şey yolunda mı görelim;)

## Aaa...Houston. We have a problem!

Galiba sende benim gibi hiç beklenmedik bir hata ile karşılaştın.

![hellomvc_4.png](/assets/images/2021/hellomvc_4.png)

Bu çalışma zamanı hatası da nereden çıktı şimdi!? Doğruyu söylemek gerekirse pek de sevimli bir ekran görüntüsü değil. Oysaki uygulama derlenebiliyor. Senden ricam StackTrace içeriği ile birlikte hata mesajını dikkatlice okuman.

Sorunu görebildin mi?

GameController sınıfına tekrar dön. Yapıcı metot parametre olarak IGameRepository şeklinde bir interface referansı alıyor. Bir başka deyişle, IGameRepository arayüzünü uygulayan herhangi bir sınıf bu yapıcı metoda referans olarak taşınıyor. Lakin.Net çalışma zamanı bunu henüz bilmiyor. Bir yerlerde bir şekilde IGameRepository görüldüğü anda "Acaba bana ihtiyacım olan bir GameRepository nesnesi verebilir misin?" diyebilmeliyiz. İşte Dependency Inversion Principle'ın süreç yöneticisi Inversion of Control'un elçisi Dependency Injection Container'ların dile geldiği yerdeyiz.

## Ne Gerektiğini Söylemek

.Net Core içerisindeki built-in DI mekanizması çalışma zamanında yukarıdaki senaryoda görülen bağımlıkların kolayca tanımlanmasına izin verir. Asp.Net tarafı söz konusuysa burası Startup sınıfı içerisindeki IServiceCollection arayüzünün kullanıldığı ConfigureServices metodudur. Oraya aşağıdaki kod parçasını eklemeni rica ediyorum (AddTransient metoduna şimdilik takılma. Raf ömrüne göre farklı kullanım senaryolarımız da var)

```csharp
public void ConfigureServices(IServiceCollection services)
{
	services.AddControllersWithViews(); //Burası zaten var
	services.AddTransient<IGameRepository, GameRepository>();
}
```

Artık çalışma zamanında GameController nesnesi IGameRepository üstünden bir fonksiyon işletmek istediğinde gerçekten o işi yapacak asıl nesne (ki senaryomuza göre GameController) elinde hazır olacak. IGameRepository'nin belirlediği sözleşme kurallarının dışına çıkmadığın sürece GameController, GameRepository'deki değişimlerden zerre kadar etkilenmeyecek;)

Tebrik ediyorum. Eğitimden önce yapman gereken hazırlığı bitirdin ve gerçek anlamda Asp.Net Core için Hello World dedin. Üstelik bunu Constructor Injection tekniği ile yaptın ki bunun dışında metot ve özellik (property) seviyesinde bile Injection tekniklerini kullanacaksın. Lakin her şey daha yeni başlıyor. Neredeyse tüm.Net 5 projelerinde bu DI mekanizmasını kullanacağız. Hatta yarın katmanlar artacak, servisler çoğalacak, repository'ler yerlerini belki de CQRS (Command and Query Responsibility Segregation) desenine bırakacak, nesne arası bağımlılıklar uzak servislere de sıçrayacak vs. Tüm bu serüven sırasında DI Container'lar hep seninle olacak.

Senden istediğim birkaç şey daha var. Bu bir sonraki adımın için iyi bir hazırlık olabilir. Şu senaryoyu düşün;

Sisteme yeni oyun ekleme özelliği sunan bir fonksiyonun olsun. Bir oyun eklendiğinde, üyelere mail ile bildirim yapacak bir sistem de kurgulamak istiyorsun. SQL'deki trigger veya Button'a basılınca çalışan Click olayı gibi. Şu an bunu GameRepository sınıfına ekleyeceğin bir Add metodu içinden yaparsın diye tahmin ediyorum. Gönderim işini ise MailSender isimli bir sınıfla gerçekleştirmeyi düşünebilirsin. Ancak GameRepository ile MailSender birbirlerine sıkı sıkıya bağlı olmamalılar. Bu bağımlılığı çöz;)

Tamam tamam. Seni rahat bırakacağım artık. Lütfen son olarak aşağıdaki maddelere de bir göz at ve cevaplarını dokümante etmeye çalış.

- Single Responsibility, Dependency Inversion prensiplerini, onları bilmeyen birisine nasıl anlatırsın?
- Inversion of Control, Dependency Inversion Principle ile aynı şey midir? Farklarını nasıl tanımlarsın?
- High-Level Component ve Low-Level Component ne demektir? Araştırıp birer cümle ile tarifler misin?
- Projedeki Data içeriğini harici bir kütüphaneye alıp kullanabilir misin?
- Sence DI Container kullanımının artıları nelerdir?
- Constructor dışında bir nesne bağımlılığını bildirmenin farklı yolları olabilir mi? Varsa bunları araştırıp örnekler misin?
- Örnekte kullanıdığımız Transient fonksiyonu tam olarak ne anlama geliyor? Onun yerini alacak farklı versiyonlar varsa bir bakar mısın?
- Örnekte Built-In mekanizma yerine örneğin Unity veya Ninject'i kullanmayı dener misin?

## Son Dakika Gelişmesi

Eğer Constructor Injection dışındaki method, property ve view (Asp.Net MVC 6 sonrası geldi) türevli tekniklerin basit uygulamasına bakmak istersen [github'a eklediğim hands-on-aspnetcore-di reposu](https://github.com/buraksenyurt/hands-on-aspnetcore-di)na uğramanı tavsiye edebilirim. Bu repoda varsayılan main haricinde initial, constructor-injection, method-injection, property-injection ve view-injection isimli ayrı branch'ler var. İşe yarayan bir örnek değil ama temiz bir biçimde bu farklı teknikleri nasıl uygulayabileceğini gösteriyor;)

Eğitimde görüşmek üzere;) Sağlıklı günler.