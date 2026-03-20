---
layout: post
title: ".NET Core 2.0 ile Basit Bir Web API Geliştirmek"
date: 2017-10-04 09:58:00 +0300
categories:
  - aspnet-web-api
tags:
  - aspnet-web-api
  - bash
  - xml
  - csharp
  - dotnet
  - entity-framework
  - ef-core
  - linq
  - rest
  - json
  - web-api
  - http
  - generics
  - visual-studio
---
Günler yoğun geçiyor. Bir süredir sosyal medyadan da uzaktayım. Kendimce sebeplerim var. Ağırlık görev değişikliği sonrası kritik geliştirmeler barındıran işimdeki yoğunluk. Bunun dışında daha çok kitap okuduğumu, telefona neredeyse hiç bakmadığımı (Türkiye ortalamasına göre bir kişi günde 70 kez telefona bakıyormuş-kahrolsun Instagram çağı), Serdar Kuzuloğlu'ndan [dünya hallerini](https://www.dunyahalleri.com/) daha çok okuduğumu, Gündem Özel'i daha çok izlediğimi (Yazıyı yazdığım günlerdeki [şu yayınlarını](https://www.youtube.com/watch?v=GYwyBC5XfFQ) tavsiye ederim. [Hasan Söylemez'i](http://www.hasansoylemez.com/) de takip edin kitabını alın derim), okuyup dinlediklerimden kendime küçük küçük notlar çıkarttığımı, daha çok basketbol oynadığımı, işe gittiğim her gün gerek otobüs gerek metorbüs gerek minibüs daha çok sıkıştığımı (tutunmadan seyahat edebilmek dahil) ama Beşiktaş-Üsküdar arası motor hattında nefes alarak huzur bulabildiğim günler geçirdiğimi ifade edebilirim. Kalan zamanlarda eskisi kadar çok olmasa da bir şeyler öğrenmeye gayret ediyorum. Bir süredir de.Net Core tarafında servis geliştirme noktasında neler yapılabileceğini incelemek istiyordum. İşlerden boşluk bulduğum bir sırada Web API nasıl yazılır araştırayım ve yaptığım örneği bloğuma ekleyeyim dedim.

![bogazmini.gif](/assets/images/2017/bogazmini.gif)

Ortam Hazırlıkları

İlk olarak [Microsoft'un ilgili adresinden](https://www.microsoft.com/net/download/core).Net Core'un son sürümünü indirdim. Çalışmaya başladığım tarih itibariyle 2.0 versiyonu bulunuyordu. Kurulumu Windows 7 işletim sistemi olan bir makinede gerçekleştirdim (Şirket bilgisayarı) Saha Hizmetleri ekibimizin de desteği ile makineye 2.0 sürümünü sorunsuz şekilde yükledim (Malum makinede Local Admin'lik olmayınca) Ardından komut satırını ve Notepad++ uygulamasını açtım. Amacım Visual Studio ailesinin (Code dahil) ürünlerini kullanmadan Web API geliştirmenin temellerini anlamaktı. Hem bu sayede hazır şablonların içeridiği kod parçalarını daha iyi anlayabilirdim. Sonrasında terminal penceresine geçtim ve incelediğim kaynaklardan derlediğim notlara da bakarak ilk komutumu verdim.

```bash
dotnet --help
```

ile dotnet komut satırı aracının nasıl kullanılabileceğini incelemeye çalıştım..Net Core'un komut satırında proje şablonlarını otomatik olarak hazırlayan new komutunun nasıl kullanılabileceğini görmek için de şu komutu kullandım. Bu sayede dotnet'in popüler ve gerekli build, restore, run gibi komutlarını nasıl kullanabileceğimizi detaylı bir şekilde görebiliriz.

```bash
dotnet new --help
```

![corewebapi_1.gif](/assets/images/2017/corewebapi_1.gif)

Sonrasında.Net Core çalışmaları için açtığım klasörde aşağıdaki komutu vererek Fabrika isimli bir Web API projesi oluşturdum.

```bash
dotnet new webapi -o Fabrika
```

![corewebapi_2.gif](/assets/images/2017/corewebapi_2.gif)

Peki şimdi ne oldu? -o parametresi ile verdiğimiz Fabrika ismi nedeniyle Fabrika adında bir klasör oluştu ve içerisine gerekli tüm proje dosyaları hazır olarak eklendi.

![corewebapi_3.gif](/assets/images/2017/corewebapi_3.gif)

Dikkat edileceği üzere Controllers isimli bir klasör de bulunuyor. Temel olarak Model View Controller desenini kullanmaya hazır bir şablon oluşturulduğunu ifade edebiliriz. Bir başka deyişle kullanıcılardan gelecek REST taleplerini kontrol eden (Controllers) geriye dönecek varlıkları (Models) varsayılan olarak JSON tipinde basacak (View) bir desen söz konusu. Varsayılan olarak Models klasörü yoktu. Bunu kendim ekledim.

Oluşturulan proje yapısından sonra ilk yaptığım şey kaynaklarda da belirtildiği üzere ortamı çalıştırmaktı.

```bash
dotnet run
```

![corewebapi_4.gif](/assets/images/2017/corewebapi_4.gif)

Dikkat edileceği üzere http://localhost:5000 adresinden ayağa kalkan ve istemci taleplerini dinlemeye hazır bir sunucu söz konusu. Tabii direkt bu adrese gidersek bir sonuç alamayız. Çünkü varsayılan olarak gelen bir yönlendirme (Router) sistemi var. Bu adres Controllers klasöründeki Controller tipinden türeyen sınıfa göre şekilleniyor. Hazır şablonla gelen ValuesControllers sınıfının kodlarına baktığımızda Route niteliğinin (attribute) kullanıldığını görürüz. Bu nitelikte ifade edilen api/[Controller] bildirimi talep edebileceğimiz HTTP adresinin şeklini belirler ki bu durumda aşağıdaki gibi olmalıdır.

http://localhost:5000/api/values

Sonuçta örnek olarak konulmuş string dizi içeriği elde edilir.

![corewebapi_5.gif](/assets/images/2017/corewebapi_5.gif)

Elbette varsayılan bir Controller sınıfı söz konusu. ValuesController sınıfının içerisinde yer alan metodlar incelendiğinde HTTP Get, Post, Put ve Delete operasyonları için gerekli hazır fonksiyonların konulduğu görülür. Hangi metodun hangi HTTP talebine cevap vereceğini belirtmek için HttpGet, HttpPost, HttpPut ve HttpDelete niteliklerinden yararlanılmaktadır.

EntityFrameworkCore Paketinin Yüklenmesi

Ben bunun üzerine işin içerisine EntityFrameworkCore'u da katmaya ve klasik ürün listelemesi yapan REST servis örneğini inşa etmeye karar verdim. Tabii ilk bulmam gereken Entity Framework Core sürümünün bu projeye nasıl ekleneceğiydi. Söz konusu kütüphane bir NuGet paketi olarak ele alınabildiğinden projenin kullandığı paketler listesinde tanımlanması yeterli olacaktı. Bu yüzden Fabrika.csproj isimli proje dosyasını açtım ve EntityFrameworkCore paketi için ItemGroup elementi altına bir PackageReference bildirimi ekledim.

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>netcoreapp2.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <Folder Include="wwwroot\" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.All" Version="2.0.0" />
	<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="2.0.0"/>
  </ItemGroup>

  <ItemGroup>
    <DotNetCliToolReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Tools" Version="2.0.0" />
  </ItemGroup>

</Project>
```

Bu işlemin ardından aşağıdaki komutu kullanarak Microsoft.EntityFrameworkCore.InMemory paketinin 2.0.0 versiyonunun indirilmesini sağlanır. restore ile bildirimi yapılan tüm paketler çözümlenir ve projenin kullanımı için gerekli indirme işlemleri yapılır.

```bash
dotnet restore
```

![corewebapi_6.gif](/assets/images/2017/corewebapi_6.gif)

Model Sınıflarının Yazılması

EntityFrameworkCore paketi eklendiğine göre gerekli Model içeriklerini yazarak ilerleyebilirdim. Product ve FabrikaContext isimli iki sınıfı Models klasörü içerisine aşağıdaki içeriklerle ekledim.

Product sınıfı

```csharp
namespace Fabrika.Models
{
	public class Product
	{
		public long Id {get;set;}
		public string Name {get;set;}
		public double UnitPrice {get;set;}
	}
}
```

FabrikaContext sınıfı

```csharp
using Microsoft.EntityFrameworkCore;

namespace Fabrika.Models
{
	public class FabrikaContext
		:DbContext
	{
		public DbSet<Product> Products{get;set;}
		
		public FabrikaContext(DbContextOptions<FabrikaContext> options)
			:base(options)
			{			
			}
	}
}
```

Product tipik bir POCO (Plain Old C# Object) olarak tasarlanmıştır. FabrikaContext ise DbContext türevli basit bir sınıftır ve içerisinde Product tipini kullanan Products isimli bir DbSet barındırmaktadır. base kullanımı nedeniyle varsayılan bir nesne oluşumu söz konusudur.

Controller Sınıfının Yazılması

Model içerikleri de hazır olduğuna göre, istemciden gelecek HTTP talebine göre devreye girecek kontrolcüyü (Controller) yazarak ilerleyebilirim. Bu amaçla Controllers klasörüne ProductsController isimli aşağıdaki içeriğe sahip sınıfı ekledim. Kontrolcünün görevi istemciden gelecek talebi ele alıp modelden yararlanarak bir çıktı üretmekten ibaret.

ProductsController sınıfı

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Fabrika.Models;

namespace Fabrika.Controllers
{
    [Route("Fabrika/restapi/[controller]")]
    public class ProductsController : Controller
    {
		private readonly FabrikaContext _context;
		
		public ProductsController(FabrikaContext context)
		{
			_context=context;
			
			if(_context.Products.Count()==0)
			{
				_context.Products.Add(new Product{Id=19201,Name="Lego Nexo Knights King I",UnitPrice=45});
				_context.Products.Add(new Product{Id=23942,Name="Lego Starwars Minifigure Jedi",UnitPrice=55});
				_context.Products.Add(new Product{Id=30021,Name="Star Wars çay takımı ",UnitPrice=35.50});
				_context.Products.Add(new Product{Id=30492,Name="Star Wars kahve takımı",UnitPrice=24.40});
				
				_context.SaveChanges();
			}
		}
        
        [HttpGet]
        public IEnumerable<Product> Get()
        {
			return _context.Products.ToList();
        }

        
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
			var product=_context.Products.FirstOrDefault(t=>t.Id==id);
			if(product==null)
			{
				return NotFound();
			}
			return new ObjectResult(product);
        }

        [HttpPost]
        public void Post([FromBody]string value)
        {
			//TODO:Yazılacak
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
			//TODO:Yazılacak
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
			//TODO:Yazılacak
        }
    }
}
```

ProductsController sınıfında route adresinin değiştirildiğinde, DbContext türevli FabrikaContext tipinin kullanıldığında dikkat edelim. Get taleplerini karşılayan iki metodumuz bulunuyor. Birisi tüm ürün listesini döndürmekte. Bu nedenle generi IEnumerable tipini döndürmekte. Diğer Get metodu ise belli bir Id'ye ait ürünü döndürüyor. Bu dönüş için IActionResult arayüzünün taşıyabileceği bir nesne örneği kullanılmakta (ObjectResult) Yapıcı metod içerisinde ürün olmama ihtimaline karşın bir kaç tane örnek ürün eklenmekte. Eklenen ürünler SaveChanges ile veritabanına kayıt altına da alınmakta (Henüz Post, Put ve Delete metodlarını tamamlamadım. Bu fonksiyonlar sonraki boşluk için kendime atadığım görevler)

İlk Deneme

Hemen

```bash
dotnet build
```

komutu ile kodu derledim. Hatasız olduğunu görünce de sevindim ve

```bash
dotnet run
```

ile sunucuyu başlatıp ürünler için tarayıcıdan bir talep girdim.

http://localhost:5000/Fabrika/restapi/products

Ancak çalışma zamanı hataları ile karşılaştım.

![corewebapi_7.gif](/assets/images/2017/corewebapi_7.gif)

FabrikaContext tipi için gerekli servis çözümlemesi bir şekilde yapılamıyordu. Sonrasında DbContext tipini servis olarak eklemeyi unuttuğumu fark ettim. Startup.cs dosyasını açarak ConfigureServices metoduna aşağıdaki satırı ilave etmek sorunun çözümü için yeterliydi (Fabrika.Models ve Microsoft.EntityFrameworkCore namespace bildirimlerini de aldığım hatalar sonrası eklemem gerektiğini itiraf etmek isterim. Biraz daha dikkatli ol Burak!)

```csharp
public void ConfigureServices(IServiceCollection services)
{
	services.AddDbContext<FabrikaContext>(opt=>opt.UseInMemoryDatabase("FabrikaDb"));
	services.AddMvc();
}
```

Burada bellekte çalışacak şekilde FabrikaDb isimli bir veritabanını, uygulamanın kullanacağı servisler listesine eklemiş oluyoruz. Örneği tekrar çalıştırdığımda sorun yaşamadım ve tarayıcıdan yaptığım bazı taleplere karşılık aşağıdaki ekran görüntülerinde yer alan sonuçları elde ettiğimi gördüm.

http://localhost:5000/Fabrika/restApi/products talebi sonrası

![corewebapi_8.gif](/assets/images/2017/corewebapi_8.gif)

http://localhost:5000/fabrika/restapi/products/30021 talebi sonrası

![corewebapi_9.gif](/assets/images/2017/corewebapi_9.gif)

http://localhost:5000/fabrika/restapi/products/999 talebi sonrası

![corewebapi_10.gif](/assets/images/2017/corewebapi_10.gif)

--Derken--

Derken Post işlemini de en iyi kaynaklardan birisi olan [şuradan](https://docs.microsoft.com/en-us/aspnet/core/tutorials/web-api-vsc#implement-the-other-crud-operations) öğrenip araya sıkıştırayım dedim ve bu paragraf açılmış oldu.

```csharp
[HttpPost]
public IActionResult Post([FromBody]Product newProduct)
{
	if(newProduct==null)
		return BadRequest();

	_context.Products.Add(newProduct);
	_context.SaveChanges();
	return CreatedAtRoute("GetProduct",new {id=newProduct.Id},newProduct);				
}
```

İlk olarak metodun IActionResult döndürecek şekilde değiştirildiğini belirtelim. FromBody niteliği ürün bilgisinin HTTP talebinin Body kısmından okunacağını belirtmekte. Eğer bir ürün bilgisi gelmezse BadRequest mesajı basılıyor. Ürünün veritabanına eklenmesi işi standart bir Entity Framework işi. CreatedAtRoute fonksiyonu HTTP 201 mesajının basılmasını sağlarken aynı zamanda GetProduct isimli bir metoda talepte bulunuyor. Tahmin edeceğiniz üzere yeni eklenen ürünün id bilgisini kullanarak bir HTTP Get talebi yapmakta. Önemli olan kısım ilk parametredeki adın nerede tanımlandığı. Bunu anlayana kadar bir kaç hata aldım da...

```csharp
[HttpGet("{id}",Name="GetProduct")]
public IActionResult Get(int id)
```

Name niteliğine atanan değer CreateAtRoute'un kullandığı fonksiyon adı. Böylece istemciye hem işlemin başarılı olduğunu söylüyor hem de yeni oluşan ürün içeriğini gönderiyoruz. Tabii senaryoyu test etmenin en pratik yolu Postman gibi bir araçtan yararlanarak JSON tipinden bir talep göndermek. Aynen aşağıdaki ekran görüntülerinde olduğu gibi.

![corewebapi_13.gif](/assets/images/2017/corewebapi_13.gif)

--Derken--

Varsayılan Port Bilgisinin Değiştirilmesi

Merak ettiğim konulardan birisi de 5000 nolu port bilgisini nasıl değiştirebileceğimdi. Bunun için Program.cs dosyasına uğramak gerekiyor. BuildWebHost fonksiyonunda ortamla ilişkili bir takım ayarlamalar yapılabilir. Örneğin 5555 nolu portun kullanılacağı bilgisi ifade edilebilir. UseUrls fonksiyonuna dikkat edelim.

```csharp
public static IWebHost BuildWebHost(string[] args) =>
	WebHost.CreateDefaultBuilder(args)
		.UseStartup<Startup>()
		.UseUrls("http://localhost:5555/") 
		.Build();
```

Bu arada Fluent bir metod zinciri söz konusu olduğunu ifade edelim (Bilmeyenler Fluent API nasıl yazılır, Fluent Interface nedir gibi sorularla bir araştırma yapsınlar derim. Buradaki pek çok projemizde bu tip Fluent yapılar kullanıyoruz)

![corewebapi_11.gif](/assets/images/2017/corewebapi_11.gif)

Statik İçeriklere İzin Verilmesi

Merak ettiğim bir diğer konu da hazır olarak gelen wwwroot klasörünü hangi amaçlarla kullanabileceğimizdi. Araştırmalarım sonucunda burada static sayfalara yer verebileceğimizi öğrendim ve şöyle bir HTML sayfası ekledim.

```text
<html>
<body>
	Fabrika'da üretilen <b><a href="http://localhost:5555/fabrika/restapi/products">ürünler</a></b>.
</body>
</html>
```

Ne varki sayfaya bir türlü erişemedim. Sonrasında statik dosyaları kullanacağımı çalışma zamanına bildirmem gerektiğini öğrendim. Bunun için startup.cs içerisindeki Configure metodunda UseStaticFiles fonksiyon bildirimini yapmak yeterli.

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
	if (env.IsDevelopment())
	{
		app.UseDeveloperExceptionPage();
	}

	app.UseStaticFiles();
	app.UseMvc();
}
```

Sonrasında index.html sayfasının geldiğini de gördüm.

![corewebapi_12.gif](/assets/images/2017/corewebapi_12.gif)

Bu arada varsayılan olarak wwwroot olarak tanımlanan klasör bilgisini UseWebRoot metodunu kullanarak farklı bir konuma da yönlendirebiliriz (Static sayfaların kullanımı ile ilgili daha fazla detay da var. [Şu adrese](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/static-files) bakmanızı öneririm)

Swagger Tabanlı Yardım Sayfasının Eklenmesi

[Swagger](https://swagger.io/) altyapısını baz alan Swashbuckle isimli NuGet paketini kullanarak etkileyici görünüme sahip yardım sayfaları oluşturabiliriz. Böylece API'nin versiyonu, ne tür operasyonlar içerdiği, nasıl kullanıldığı hakkında bilgiler verebilir hatta o an örnek test verileri ile denemeler yaptırtabiliriz. Bunu denemek için ilk olarak komut satırından Fabrika isimli projeye ilgili paketi aşağıdaki ifadeyle ekledim.

```bash
dotnet add Fabrika.csproj package Swashbuckle.AspNetCore
```

Komutu çalıştırdıktan sonra proje dosyasına yeni bir PackageReference bildirimi eklendiğini görebiliriz (Bu arada bir paketi manuel olarak proje dosyasına ekleyip dotnet restore komutu ile ilerlemek yerine bu şekilde işlem yapılabileceğini de öğrenmiş bulundum. Mutluyum)

```xml
<ItemGroup>
	<PackageReference Include="Microsoft.AspNetCore.All" Version="2.0.0" />
	<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="2.0.0" />
	<PackageReference Include="Swashbuckle.AspNetCore" Version="1.0.0" />
</ItemGroup>
```

İlgili servisi kullanıma sunmak içinse Startup.cs sınıfındaki ConfigureServices ve Configure metodlarında bazı değişiklikler yapılması gerekiyor.

```csharp
//Diğer isim alanları
using Swashbuckle.AspNetCore.Swagger;

namespace Fabrika
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
			//Diğer kodlar
			
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Info { Title = "Fabrika API", Version = "v1" });
			});
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
			//Diğer kodlar
			
			app.UseSwagger();
			app.UseSwaggerUI(c=>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json","Fabrika API v1.0");
			});
        }
    }
}
```

İki önemli ek söz konusu. İlk olarak ConfigureServices metodu içerisinde ilgili Swagger servisinin orta katmana eklenmesi sağlanıyor. Configure fonksiyonunda ise kullanıcı arayüzü için gerekli json içeriğinin adresi (Endpoint bilgisi) belirtilmekte ve Swagger çatısının kullanılacağı ifade edilmekte. Bu ekleri yaptıktan sonra aşağıdaki adrese talep gönderdim ve otomatik olarak üretilen bir JSON çıktısı ile karşılaştım.

http://localhost:5555/swagger/v1/swagger.json

![corewebapi_14.gif](/assets/images/2017/corewebapi_14.gif)

Sonrasında ise takip ettiğim MSDN dokümanının söylediği gibi doğrudan swagger adresine gittim.

http://localhost:5555/swagger/

Sonuç inanılmaz güzeldi benim için (Otursam böyle bir tasarım yapamayacağım için olsa gerek) Kendimi çok fazla yormadan hazır bir swagger paketini kullanarak söz konusu API operasyonlarını görebileceğim, test edebileceğim bir içeriğe ulaştım.

![corewebapi_15.gif](/assets/images/2017/corewebapi_15.gif)

Artık Fabrika API'sinin yardım sayfası en temel haliyle hazır diyebiliriz. Pek tabi bunu özelleştirmek de gerekiyor ki gayet güzel bir şekilde özelleştirebiliyoruz. Açıklamaları genişletebiliyor, XML Comment'leri kullanarak operasyonlar hakkında daha detaylı bilgiler verebiliyoruz vs... [Şu adreste](https://docs.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger?tabs=netcore-cli) bu konu ile ilgili detaylı bilgiye ulaşabilirsiniz.

> Mesela AddSwaggerGen fonksiyonunu aşağıdaki gibi zengileştirebiliriz.
> ```csharp
> services.AddSwaggerGen(c =>
> {
> 	c.SwaggerDoc("v1", new Info { 
> 		Title = "Fabrika API"
> 		, Version = "v1" 
> 		, Description ="Fabrika'da üretilen ürünler hakkında bilgiler"
> 		, Contact=new Contact{
> 			Name="Burak Selim Şenyurt"
> 			, Email="", Url="http://www.buraksenyurt.com"},
> 		License=new License{
> 			Name="Under GNU"
> 			, Url="http://www.buraksenyurt.com"}
> 		});
> });
> ```
> ![corewebapi_16.gif](/assets/images/2017/corewebapi_16.gif)

Son Sözler

Yazıyı bitirmekte olduğum şu anda içim biraz olsun huzurla doldu beynimdeki dopamin salınımı da arttı diyebilirim. En azından.Net Core 2.0'ı kullanarak, Visual Studio ailesine de el atmadan Notepad++ ile REST tabanlı basit bir Web API servisi yazabildim. Şimdi bunu evdeki Ubuntu üzerinde yapmaya çalışacağım.

Tabii gerçek hayat senaryolarında durum biraz daha farklı. Bir firmanın dışarıya açacağı servis bazlı API'leri düşünelim. Manuel olarak tek tek servis yazmak istenmeyecektir. Şirket içindeki hazır veri üretimi yapan birimlerin dinamik kodlar yardımıyla ayağa kalkacak servisler şeklinde sunulmasına çalışılacaktır. Kısacası bu tip Web API'leri bir Factory yardımıyla dinamik olarak nasıl üretebiliriz sorusu da gündeme geliyor. Örneğin şirketinizde n sayıda kütüphanenin belirli fonksiyonlarının Web API'ler ile açılacağını düşünün. Her bir kütüphane için Web API servisi yazmaya çalışmak yerine otomatik olarak bunları ayağa kaldıracak, yetkilendirmelere tabii tutacak bir mekanizma yazmak çok daha avantajlı olacaktır. Bu açılardan konuyu düşünmemizde ve öğrenmeye devam etmemizde yarar olduğu kanısındayım. Böylece geldim bir makalemin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

## Yazmaya Üşenenler İçin

[ProductsController.cs (2,38 kb)](https://www.buraksenyurt.com/file.axd?file=/2017/10/ProductsController.cs)

[FabrikaContext.cs (271,00 bytes)](https://www.buraksenyurt.com/file.axd?file=/2017/10/FabrikaContext.cs)

[Product.cs (161,00 bytes)](https://www.buraksenyurt.com/file.axd?file=/2017/10/Product.cs)

[Program.cs (665,00 bytes)](https://www.buraksenyurt.com/file.axd?file=/2017/10/Program.cs)

[Startup.cs (1,31 kb)](https://www.buraksenyurt.com/file.axd?file=/2017/10/Startup.cs)
