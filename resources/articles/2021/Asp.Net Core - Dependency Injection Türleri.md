---
title: "Asp.Net Core - Dependency Injection Türleri"
pubDate: 2021-04-29 11:03:00
categories:
  - Asp.Net Core
tags:
  - Asp.Net Core
  - C#
  - .NET
  - ASP.NET
  - ASP.NET MVC
  - RabbitMQ
  - Async/Await
  - Dependency Injection
  - GitHub
  - Dependency Management
---

# Asp.Net Core - Dependency Injection Türleri
![image.axd](images/image.axd)

Ayakta durmuş odanın camından dışarıyı izlerken yazıya nasıl bir giriş yapsam diye düşünüyordum. Baharın etkisi ile yapraklarını açmış meşenin yavaş yavaş gölgelediği caddeden İtalyan bayrağı kasklı bir motosikletli geçti aniden. Sadece birkaç metre gerisinden de onu neredeyse aynı süratle takip eden martıya binmiş bir genç. Kaldırımda bir elinde alışveriş poşeti ötekinde onu yola doğru çekiştiren haylazla birlikte yürümeye çalışan orta yaşlarında bir kadın. Hemen binanın önündeki basket sahasında da yaşları beş ile on beş arasında değişen on çocuk. Futbol oynuyorlar. Bağrışlar, çağrışlar. Çekişmeli de gidiyor ama herkesin yüzünde bir maske. Eve kapanmak zorunda kalmadan önce çocukların son bir bahar ziyafetini izliyorum diye iç geçiriyorum.

On yedi günlük evden çıkma yasaklarının bir gün öncesi çünkü bugün, 29 Nisan 2021 Perşembe. Pek tabii hayat evde de olsa devam ediyor. Bende bu dönemi iyi değerlendirmek adına yaz aylarında vereceğim şirket eğitimleri için Amazon'dan getirttiğim kitapları çalışmaya ağırlık vereyim diyorum. Malum.Net 5 güldür güldür geleli çok oldu ve orada öğrenmem gereken birçok konu birikti. En önemli şey ise öğrendiğim bir konuyu olabildiğince basit şekilde anlatabilmek. Bakalım bu yazıda bunu başarabilecek miyim?

Hatırlayacağınız üzere [bir önceki yazımızda](Asp.Net Core'a Nasıl Merhaba Deriz-.md) Asp.Net 5 tarafında nasıl Hello World diyebileceğimizi incelemeye çalışmıştık (Henüz okumadıysanız bir göz atmanızda yarar var) O çalışmada ana odak noktamız dahili Dependency Injection mekanizmasının nasıl kullanıldığını görmekti. Kobay senaryomuzdaki en önemli noktalardan birisi de GameController sınıfı içerisinde IGameRepository yardımıyla low-level bir bileşenin kullanımıydı. Burada Constructor Injection tekniğinden yararlandığımızı ifade etmiştik. Bu teknik dışında kullanabileceğimiz versiyonlar da var. Bağımlı nesne çözümlemesini metot üzerinden, Property yardımıyla ve Asp.Net MVC 6 ile gelen @inject direktifi yoluyla da gerçekleştirebiliriz. İşte bu yazımızdaki amacımız aynı senaryoyu devam ettirerek söz konusu tekniklerin nasıl uygulanabileceğini öğrenmek. Yazıdaki kod parçaları [şuradaki github hesabımda](https://github.com/buraksenyurt/hands-on-aspnetcore-di) yer alıyor. Initial, constructor-injection, method-injection, property-injection ve view-injection şeklinde farklı branch'ler içeriyor. Bu branch'lerde ilgili tekniklerin proje üstünden ayrı ayrı uygulanış şekillerini takip edebilirsiniz. Yazı boyunca ise odak noktamızı kaybetmemek adına sadece gerekli kod parçalarını kullanacağım. Paralel hareket etmeniz gerekebilir. Hazırsanız başlayalım;

## Method Injection

Aslında yapıcı metot (Constructor) bir metottur. Dolayısıyla neden bu şekilde farklı bir uygulama tekniği olduğunu düşünebilirsiniz. Ne var ki, Constructor tekniğinde bağımlı nesnenin çözümlenmesi (DI'ın Resolution aşaması) ona ihtiyaç duyan nesne örneklenirken gerçekleşir. Bazı durumlarda sadece belli bir metot içerisinden kullanılan bağımlı nesneler de olabilir. Sanırım örnek bir senaryo üzerinden gidersek konu daha anlaşılır olacak.

Oyun portaline yenilerini eklemek için bir fonksiyon yazacağımızı düşünelim. Ayrıca her oyun eklendiğinde bir dış sisteme mesajla bildirim yapamak istiyoruz. RabbitMQ gibi bir kuyruk sistemi, veritabanı ya da doğrudan e-posta sunucusu bile olabilir. En nihayetinde GameRepository'nin Create metodu içerisinde bu gönderim işlemini yapmaya karar veriyoruz. Bununla birlikte mesaj yayınlama işini üstlenen asıl sınıfı kullanmak yerine, sadece gönder demenin daha doğru olacağını da biliyoruz. Çünkü bu sıkı bağlı (tightly-coupled) yapıda söz konusu olan xyz sistemi için gerekli gönderim adımlarını, bununla ilgisi olmayan GameRepository sınıfının anlamasına gerek yok. Hatta tightly-coupled durumlarda ısrarla kaçınmaya da çalışıyoruz. Haydi gelin kodlamaya başlayalım. İlk olarak IPublisher isimli bir arayüz geliştirelim.

```csharp
public interface IPublisher
{
	void Send(string message);
}
```

IPublisher arayüzü senaryomuz için oldukça ilkel bir sözleşme sunuyor. Geriye değer döndürmeyen ve string tipte parametre alan Send isimli bir metot bildirimi taşıyor. Bu arayüzü kullanan örnek bir sınıfı da aşağıdaki gibi geliştirdiğimizi düşünelim.

```csharp
public class RabbitPublisher
	: IPublisher
{
	public void Send(string message)
	{
	   //todo something
	}
}
```

Biliyorum, fonksiyon içerisinde bir şey yapmıyoruz ama unutmayın; amacımız Method Injection'ı uygulamak. Bu durumda GameRepository sınıfı için düşündüğümüz Create metodunu nasıl yazarız, bir düşünün. İdeal olanı aşağıdaki kod parçasında olduğu gibidir.(Bu arada IGameRepository arayüzünde de Create bildiriminin yapılması gerektiğini hatırlatayım. Nitekim IGameRepository üstünden kullanacağımız bir fonksiyon olmalı)

```csharp
public Game Create(Game game, IPublisher publisher)
{
	publisher.Send("A new game has been added to inventory");
	return game;
}
```

Metodun belki de en önemli kısmı ikinci parametresidir ve IPublisher arayüz referansı kullanılmaktadır. Dolayısıyla onu uygulayan bir nesneyi metot içerisine alabilir ve Send fonksiyonunu çağırabiliriz. Bir başka deyişle Create metodunun ihtiyacı olan asıl nesne arayüz üzerinden kullanılır. Bu, Method Injection tekniği ile bağımlılığın çözümlenmesidir. Ancak ortada halen daha bir soru var. Create metodunun hangi IPublisher türevi ile çalışacağını nerede söyleyeceğiz? Yani bağımlı nesne için gerekli kayıt işlemini (Dependency Injection Service Registration) nerede yapacağız? Tahmin edileceği üzere bu sorunun cevabı Create metodunun çağırıldığı yerdir ve GameController sınıfındaki Create metodu bunun için uygundur.

```csharp
[HttpPost]
public IActionResult Create(Game game)
{
	_gameRepository.Create(game, new RabbitPublisher());
	return RedirectToAction("Index");
}
```

Dikkat edileceği üzere ikinci parametrede bir RabbitPublisher nesne örneği kullanılıyor. Yani Create metodunun ihtiyaç duyduğu asıl nesneyi metot üzerinden göndermiş oluyoruz.

## Property Injection

Yukarıdaki senaryoyu düşündüğümüzde aklımıza şöyle bir soru da gelebilir; ortada bir IPublisher referansı yoksa Send metodunun çalışmamasını nasıl sağlarız? Yani Create metodunun IPublisher ile çalışmasını opsiyonel olarak sunmak istersek nasıl bir yol izleriz? Bu senaryo için IGameRepository arayüzünü aşağıdaki gibi değiştirelim.

```csharp
public interface IGameRepository
{
	IEnumerable<Game> GetAllGames();
	IPublisher Publisher { get; set; }
	Game Create(Game game);
}
```

Dikkat edileceği üzere Create metodunun parametresi olarak kullandığımız IPublisher arayüzünü, özellik olarak tip seviyesine aldık. Çok doğal olarak GameRepository sınıfının içeriği de buna uygun şekilde değiştirilmeli.

```csharp
public class GameRepository
	: IGameRepository
{
	public IPublisher Publisher { get; set; }
	public IEnumerable<Game> GetAllGames()
	{
		return new List<Game>
		{
			new Game{ Id=1, Title="Commandos II",ListPrice=10.5M },
			new Game{ Id=2, Title="Prince of Persia",ListPrice=9.45M },
			new Game{ Id=3, Title="Prince of Persia",ListPrice=9.45M }
		};
	}

	public Game Create(Game game)
	{
		if (Publisher != default)
			Publisher.Send("A new game has been added to inventory");
		return game;
	}
}
```

Lütfen Create metodunun içerisindeki if kullanımına dikkat edelim. Eğer IPublisher türünden olan Publisher isimli özellik (property) gerçekten bir nesne referansı taşıyorsa Send metodu çağırılacaktır. Böylece Create metodunun bağımlılığını property seviyesine çekerek tercihe bırakmış olduk. Tabii bağımlı nesnenin kayıt işlemini de yapacağımız bir yer olmalı öyle değil mi? Yine GameController sınıfındaki Create metodunda bunu yapabiliriz.

```csharp
[HttpPost]
public IActionResult Create(Game game)
{
	_gameRepository.Publisher = new RabbitPublisher();
	_gameRepository.Create(game);
	return RedirectToAction("Index");
}
```

Görüldüğü üzere _gameRepository nesnesinin (ki o da GameController sınıfına Constructor üzerinden enjekte edilmektedir) Publisher özelliğine yeni bir RabbitPublisher referansı atadık. Dolayısıyla Create metodu çağrıldığında RabbitMQ'ya mesaj gönderen asıl fonksiyon işleyecektir. Lakin Publisher özelliğine bir atama yapılmazsa herhangi bir gönderim işlemi de olmayacaktır. Seçime bağlı bu nesne çözümlemesi için Property Injection tekniğini nasıl kullanacağımızı da görmüş olduk.

## View Injection

Aslında Asp.Net tarafına MVC 6 ile birlikte gelen ve genel Dependency Injection teknikleri içerisinde olmadığını düşündüğüm bir yöntem daha var. @inject direktifinin kullanımı. MVC/MVVM desenlerinde bir View'u, Controller'dan veya View-Model'den ayrıştırmak istediğimiz durumlarda kullanabileceğimiz bir yöntem olarak karşımıza çıkıyor. Yöntem sayesinde DI Container üstünden kayıt edilen bir nesne veya servis metodunun View üstünden doğrudan çağrılması sağlanabiliyor. Senaryomuzda portaldeki hareketliliklerle ilgili veri toplayan ve örneğin aktif kullanıcıların sayısını veren aşağıdaki gibi bir sınıf olduğunu düşünelim.

```csharp
public class DataCollectorService
{
	public async Task<int> GetActiveUserCount()
	{
		return await Task.FromResult(new Random().Next(10,50));
	}
}
```

DataCollectorService içerisindeki GetActiveUserCount metodunun ne iş yaptığının çok önemi yok ama bu metodu bir View bileşeninde aşağıdaki gibi doğrudan kullanmamız mümkün.

```text
@inject C64Portal.Agent.DataCollectorService dataCollectorService

<div>
    <h2>Envanterdeki C64 Oyunları</h2>
    <hr />
    <h3>Current active user count is @await dataCollectorService.GetActiveUserCount() </h3>

</div>
```

Tabii örneği bu haliyle çalıştırıp Inventory sayfasına gitmek istersek kaçınılmaz olarak aşağıdaki hata mesajı ile karşılaşırız.

![image.axd](images/image.axd)

View nesnesi bir nesne çözümlemek istemektedir ancak bu nesne dahili DI Container'ın servis koleksiyonunda yer almamaktadır. Dolayısıyla Startup sınıfındaki ConfigureServices metodunda DataCollectorService isimli servis için bir kayıt işlemi yapılmalıdır.

```csharp
public void ConfigureServices(IServiceCollection services)
{
	services.AddControllersWithViews();

	services.AddTransient<IGameRepository, GameRepository>();
	services.AddTransient<DataCollectorService>();
}
```

Sonrasında uygulamanın sorunsuz çalıştığı gözlemlenebilir.

![image.axd](images/image.axd)

Bu ve önceki yazıyla birlikte Asp.Net 5'in temel Dependency Injection uygulama tekniklerini görmüş olduk. Tabii Dependency Injection konusu bunlarla bitmiyor. ConfigureServices metodunda servisleri kayıt altına alırken hep AddTransient metodunu kullandığımızı fark etmiş olmalısınız. Oysa ki AddScope ve AddSingleton metotları da var. Yani kayıt altına alınan bir DI servisinin hangi anda örnekleneceğini ve yaşam ömrünün ne olacağını da belirleyebiliyoruz. Bu konu ile ilgili fırsatım olursa bir şeyler karalamaya çalışacağım. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize sağlıklı günler dilerim.
