---
layout: post
title: "Bir Web API Hizmetinde Talepler ile CQ Arasında Mediator Kullanmak"
date: 2020-05-29 17:23:00 +0300
description: "CQRS, nam-ı diğer Command Query Responsibility Segregation mimari deseni, veritabanına doğru yapılan yazma, güncelleme, silme gibi aksiyonlar ile veri çekme işlemlerini ayrıştırmayı önermektedir. Command tarafı ile ilişkilendirilen aksiyonlar sadece veri üzerinde işlem yapar ve geriye bir şey döndürmezler. Sorgulama(Query) tarafına ayrılan aksiyonlar da tam tersine sadece veri döndürmekten sorumludurlar(Listeleme veya tek bir öğe detayının döndürülmesi gibi) Bir Web API ve CQRS söz konusu olduğunda karşımıza birde Mediator tasarım kalıbının uygulanışı çıkar."
categories:
  - dotnet-core
tags:
  - dotnet-core
  - bash
  - csharp
  - dotnet
  - entity-framework
  - linq
  - web-api
  - http
  - async-await
  - generics
  - github
---
# Bir Web API Hizmetinde Talepler ile CQ Arasında Mediator Kullanmak
![cqrs.png](/assets/images/2020/cqrs.png)

CQRS, nam-ı diğer Command Query Responsibility Segregation mimari deseni, veritabanına doğru yapılan yazma, güncelleme, silme gibi aksiyonlar ile veri çekme işlemlerini ayrıştırmayı önermektedir. Command tarafı ile ilişkilendirilen aksiyonlar sadece veri üzerinde işlem yapar ve geriye bir şey döndürmezler. Sorgulama (Query) tarafına ayrılan aksiyonlar da tam tersine sadece veri döndürmekten sorumludurlar (Listeleme veya tek bir öğe detayının döndürülmesi gibi) Bir Web API ve CQRS söz konusu olduğunda karşımıza birde Mediator tasarım kalıbının uygulanışı çıkar.

Şöyle düşünebiliriz; Veritabanındaki kahramanların listesini çekmek Controller tarafına gelen bir HTTP Get talebidir ve davranışsal olarak listelemeyi ifade eder. Listelemeyi ele alacak bir Handler tanımlanabilir. Listeleme ihtiyacı oluştuğunda bunun doğru Query nesnesi ile ilişkilendirilmesi sağlanmalıdır. İşte bu noktada devreye girecek Mediator, Controller üzerinden doğru Handler<->(Command/Query) ilişkisini tesis eder. Benzer şekilde yeni bir kahramanın veritabanına eklenmesi veya silinmesi CQRS'in Command kısmını ilgilendiren bir mevzudur. Yeni kahraman eklenmesini Create isimli bir tip olarak ifade edersek bu tasarım içerisinde bir Handler ve Command ilişkisini kurabiliriz.

Bu teoriyi daha kolay anlayabilmek için temel CRUD (Create Read Update Delete) operasyonlarını içeren bir.Net Core Web API hizmeti üstünde CQRS ve Mediator kütüphanesini uygulamalı olarak çalışmakta yarar var. Örnekte Handler tiplerinin tasarlanması ve Controller tarafında Command/Query ile Handler ilişkilerinin tesis edilmesi için MediatR isimli Nuget paketinden yararlanılması ele alınıyor.

## Projenin İnşası

Örnekte kullanılan veri kaynağı çok önemli değil. Basit olması için SQLite ve Entity Framework kullanmayı tercih edebiliriz. İskelet ve gerekli kurulumları aşağıdaki terminal komutlarında olduğu gibi yapılabiliriz. Web API projesinin ve gerekli sınıfların oluşturulmasını takiben SQLite tarafı için de bir migration işlemi uygulamaktayız.

```bash
dotnet new webapi -o Marvil
cd Marvil
mkdir Model

# DbContext, basit model sınıfı ve Controller
touch Model/Hero.cs Model/MarvilDbContext.cs Controllers/HeroesController.cs

# Entity Framework, SQLite ilişkisi ve migration desteği için gerekli nuget paketlerinin yüklenmesi
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite

# Hero.cs, MarvilDbContext.cs, appSettings, startup.cs içerisinde gerekli hazırlıkları yaptıktan sonra migration işlemini uygulayabiliriz
dotnet ef migrations add initial
dotnet ef database update

# Mediator rolünü üstlenecek MediatR paketinin eklenmesi
dotnet add package MediatR.Extensions.Microsoft.DependencyInjection

# Handler klasörünün açılması ve ilgili sınıf dosyalarının açılması
mkdir Handler
touch Handler/List.cs Handler/Single.cs Handler/Create.cs Handler/Delete.cs Handler/Update.cs Handler/GreaterThan.cs
```

Sırasıyla uygulamamızdaki kodlarımızı yazarak devam edelim. Bir kahramanı temsil eden model nesnemiz ve DbContext türevi ile işe başlanabilir.

Hero.cs;

```csharp
namespace Marvil.Model
{
    public class Hero
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AlterEgo { get; set; }
        public int ForceLevel { get; set; }
    }
}
```

MarvilDbContext.cs;

```csharp
using Microsoft.EntityFrameworkCore;

namespace Marvil.Model
{
    public class MarvilDbContext : DbContext
    {
        public MarvilDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Hero> Heroes { get; set; }
    }
}
```

Controller görevi üstlenen HereosController sınıfının inşa edilmesinden önce Command ve Query sınıflarının yazılması gerekir. Create, List, Update, Delete...

List.cs;

```csharp
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Marvil.Model;

namespace Marvil.Handler
{
    /* 
        Davranışımız listeleme. List isimli sınıfla ifade edeceğiz.
        Listelemeye karşılık gelen Query ve Handler sınıflar da bu sınıfın içerisinde tanımlanıyorlar
        IRequest sınıfı MediatR paketiyle gelmekte
    */
    public class List
    {
        // CQRS'in Query nesnesi gibi düşünelim.
        // Hero tipinden bir liste dönmesi gerektiği ifade ediyor
        public class Query : IRequest<List<Hero>> { }

        /*
            Listeleme işini üstlenen Handler sınıfı
            Uyguladığı arayüze göre ilk parametre ile hangi Query nesnesini kullanacağı 
            ikinci parametre ile de Handler'ın geriye ne döndüreceği belirtiliyor
            HeroesController sınıfında bu Handler sınıfının nasıl kullanıldığına dikkat edelim
        */
        public class Handler : IRequestHandler<Query, List<Hero>>
        {
            // Entity Context nesnesi Constructor üstünde enjekte ediliyor
            private MarvilDbContext _context { get; }
            public Handler(MarvilDbContext context)
            {
                _context = context;
            }
            // IRequestHandler arayüzünden gelen aşağıdaki metot Entity Tarafı ile konuşan
            // ve listeyi döndüren operasyonu üstlenmekte
            public async Task<List<Hero>> Handle(Query request, CancellationToken cancellationToken)
            {
                var heroes = await _context.Heroes.ToListAsync();
                return heroes;
            }
        }
    }
}
```

Single.cs;

```csharp
using MediatR;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Marvil.Model;

namespace Marvil.Handler
{
    /* 
        Genel pratik olarak ID bazlı arama yaptırıp tek bir kahraman bilgisini geri döndüren davranışı programlıyoruz.
    */
    public class Single
    {
        /*
            CQRS'in Query nesnesi gibi düşünelim.
            Hero tipinden bir nesne döndüreceği belirtiliyor
            İsme göre arama yapacağımız için Name isimli bir özellik de var.
        */
        public class Query : IRequest<Hero>
        {
            public string Name { get; set; }
        }

        public class Handler : IRequestHandler<Query, Hero>
        {
            private MarvilDbContext _context { get; }
            public Handler(MarvilDbContext context)
            {
                _context = context;
            }
            public async Task<Hero> Handle(Query request, CancellationToken cancellationToken)
            {
                var hero = await _context.Heroes.FirstOrDefaultAsync(h => h.Name == request.Name);
                if (hero == null)
                    throw new Exception("Aranan kahraman bulunamadı");
                return hero;
            }
        }
    }
}
```

Create.cs;

```csharp
using System;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Marvil.Model;

namespace Marvil.Handler
{
    /*
    Yeni veri ekleme bizim için Create isimli bir davranış
    */
    public class Create
    {
        /*
            Command sınıfı MediatR'deki IRequest arayüzünden türer.
            CQRS'in Command nesneleri bilindiği üzere geriye bir şey döndürmeyen aksiyonlarda ele alınır. Veri ekleme gibi.
            Bu nedenle List ve Single sınıflarındaki Query tiplerinde olduğu gibi türlü bir IRequest söz konusu değildir.
            Hero sınıfı ile aynı özelliklere sahiptir.
            Handler ile ilişkilendiriecek Command nesnesidir.
        */
        public class Command : IRequest
        {
            public string Name { get; set; }
            public string AlterEgo { get; set; }
            public int ForceLevel { get; set; }
        }

        /*
            Handler sınıfı IRequestHandler<Command> arayüzünü uygulamakta
        */
        public class Handler : IRequestHandler<Command>
        {
            // DbContext'in enjekte edilmesi
            private readonly MarvilDbContext _context;
            public Handler(MarvilDbContext context)
            {
                _context = context;
            }

            /*
                Yeni kahraman ekleme işini ele alan metodumuz.
                İlk parametre aynı zamanda gelen talepteki bilgileri alıp yeni Hero nesnesinin örneklenmesin kullanılıyor
            */
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                //TODO: Kahraman daha önceden eklenmişse tekrar eklenmesin

                var hero = new Hero
                {
                    Name = request.Name,
                    AlterEgo = request.AlterEgo,
                    ForceLevel = request.ForceLevel
                };
                // DbContext üstündeki Heroes koleksiyonuna ekleniyor
                _context.Heroes.Add(hero);
                // Kayıt işlemi başarılıysa 
                var success = await _context.SaveChangesAsync() > 0;
                if (success)
                {
                    return Unit.Value;
                }
                else // Değilse
                {
                    throw new Exception("Kahraman listeye eklenemedi");
                }
            }
        }
    }
}
```

Delete.cs;

```csharp
using System;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Marvil.Model;

namespace Marvil.Handler
{
    /*
        Operasyon silme.
        Create operasyonu gibi geriye veri döndürmez.
        Command ve Handler buna göre tasarlanır.
    */
    public class Delete
    {

        // Silinmek istenen verinin Name bilgisi yeterli. Command sınıfını buna göre tasarlıyoruz.
        // Geriye veri döndürmediğinden generic olmayan IRequest'i kullandık
        public class Command : IRequest
        {
            public string Name { get; set; }
        }

        // Silme operasyonunu ele alan Handler sınıfı
        public class Handler : IRequestHandler<Command>
        {
            private readonly MarvilDbContext _context;

            public Handler(MarvilDbContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                // Önce kahramanı bulalaım
                var hero = await _context.Heroes.FirstOrDefaultAsync(h => h.Name == request.Name);
                if (hero == null) //Yoksa exception fırlatıyoruz
                {
                    throw new Exception("Bu isme sahip bir kahraman listede yok");
                }
                _context.Remove(hero); // Bulduysak siliyoruz
                var success = await _context.SaveChangesAsync() > 0; //Unit tipinden bir şey döndürmemiz lazım
                if (success)
                {
                    return Unit.Value;
                }
                throw new Exception("Silme işlemi sırasında bilinmeyen hata.");
            }
        }
    }
}
```

Update.cs;

```csharp
using System;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Marvil.Model;

namespace Marvil.Handler
{
    /*
        Güncelleme tasarlandığı kısım.
    */
    public class Update
    {
        /*
            Kahraman verisi güncellenirken kuvvetle muhtemel tüm özelliklerinin son hallerini almak lazım.
            Güncelleme CQRS'in Command kısmına ait bir konu olduğundan geriye bir şey döndürmeyeceğiz.
            Bu nedenle sadece IRequest türetmesi söz konusu.
        */
        public class Command : IRequest
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string AlterEgo{get;set;}
            public int ForceLevel { get; set; }
        }

        // Güncelleme işini üstlenen Handler
        public class Handler : IRequestHandler<Command>
        {
            private readonly MarvilDbContext _context;

            public Handler(MarvilDbContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                // Önce kahramanı bulalaım
                var hero = await _context.Heroes.FirstOrDefaultAsync(h => h.Id == request.Id);
                if (hero == null) //Yoksa exception fırlatıyoruz
                {
                    throw new Exception("Bu isme sahip bir kahraman listede yok");
                }
                // varsa güncelleme yapıp kaydediyoruz.
                hero.Name=request.Name;
                hero.AlterEgo=request.AlterEgo;
                hero.ForceLevel=request.ForceLevel;

                var success = await _context.SaveChangesAsync() > 0; //Unit tipinden bir şey döndürmemiz lazım
                if (success)
                {
                    return Unit.Value;
                }
                throw new Exception("Silme işlemi sırasında bilinmeyen hata.");
            }
        }
    }
}
```

GreaterThan.cs;

```csharp
using MediatR;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Marvil.Model;

namespace Marvil.Handler
{
    /* 
        Davranışımız yine listeleme ama biraz daha farklı.
        Bu sefer gücü belli bir değerin üstünde olan kahramanları döndürüyoruz
    */
    public class GreaterThan
    {
        /*
            CQRS'in Query nesnesi gibi düşünelim.
            Hero tipinden bir liste dönmesi gerektiği ifade ediyor.
            Listenin arama kriterini de property olarak belirliyoruz.
        */
        public class Query : IRequest<List<Hero>>
        {
            public int LevelValue { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<Hero>>
        {
            private MarvilDbContext _context { get; }
            public Handler(MarvilDbContext context)
            {
                _context = context;
            }
            // IRequestHandler arayüzünden gelen aşağıdaki metot Entity Tarafı ile konuşan
            // ve listeyi döndüren operasyonu üstlenmekte
            public async Task<List<Hero>> Handle(Query request, CancellationToken cancellationToken)
            {
                var heroes = await _context.Heroes.Where(h => h.ForceLevel >= request.LevelValue).ToListAsync();
                return heroes;
            }
        }
    }
}
```

Sonrasında bu nesneleri MediatoR yardımıyla ilişkilendiren Controller'ı yazabiliriz.

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using Marvil.Model;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Marvil.Handler;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeroesController : ControllerBase
    {
        // Mediator nesnesini constructor üzerinden enjekte ediyoruz
        private readonly IMediator _mediator;
        public HeroesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /*
            Yeni kahraman ekleme işinin ele alındığı metot. 
            Tipik HTTP Post.
            Parametre olarak Create tipi içindeki Command sınıfı (Handler görevini üstlenen) kullanılıyor
        */
        [HttpPost]
        public async Task<ActionResult<Unit>> Create(Create.Command command)
        {
            // Mediator gelen talebi uygun Handler'a yönlendirmekte
            return await _mediator.Send(command);
        }

        [HttpDelete("{name}")]
        public async Task<ActionResult<Unit>> Delete(string name)
        {
            return await _mediator.Send(new Delete.Command { Name = name });
        }

        // Veri güncelleme işini üstlenen operasyon
        [HttpPut]
        public async Task<ActionResult<Unit>> Update(Update.Command command)
        {
            return await _mediator.Send(command);
        }

        // Listeleme için gelen Http Get talebine karşılık çalışacak
        // Geriye Hero listesi döndürür
        [HttpGet]
        public async Task<ActionResult<List<Hero>>> List()
        {
            // İşte en güzel kısım :)
            // Listeleme davranışı için devreye giren Mediator nesnesi
            // Bu isteği List Handler içerisindeki Query sınıfına yönlendiriyor.
            // Diğer metotlarda da sadece Send fonksiyonunu çağırdığımıza ve gerekli Query ya da Command nesnesini parametre olarak verdiğimize dikkat edelim.
            return await _mediator.Send(new List.Query());
        }

        // İsimden kahraman detaylarını döndüren HTTP Get operasyonu
        [HttpGet("{name}")]
        public async Task<ActionResult<Hero>> Single(string name)
        {
            /*
                Single sınıfı içindeki Query nesnesini örneklerken,
                gerekli isim parametresini de besliyoruz
            */
            return await _mediator.Send(new Single.Query() { Name = name });
        }

        [HttpGet("gt/{value}")]
        public async Task<ActionResult<List<Hero>>> GreaterThan(int value)
        {
            return await _mediator.Send(new GreaterThan.Query() { LevelValue = value });
        }
    }
}
```

Startup.cs tarafında Entity Framework ve MediatR için gerekli Middelware bildirimlerini de ConfigureServices metodu içerisinde aşağıdaki gibi yapabiliriz.

```csharp
public void ConfigureServices(IServiceCollection services)
{
	// DbContext nesnesini middelware'e ekliyoruz ve appsettings dosyasındaki bağlantı bilgisi doğrultusunda Sqlite kullanacak şekilde ayağa kaldırıyoruz.
	services.AddDbContext<MarvilDbContext>(opt =>
	{
		opt.UseSqlite(Configuration.GetConnectionString("MarvilDbConnection"));
	});

	// Mediator nesnesini servis olarak çalışma zamanına ekliyoruz            
	services.AddMediatR(typeof(List.Handler).Assembly);

	services.AddControllers();
}
```

## Çalışma Zamanı

Piuvvv!!! Epey zahmetli bir yoldan geçerek buraya kadar gelmiş olmalısınız. Öyleyse çalışma zamanına geçebiliriz. Web API'yi çalıştırdıktan sonra Postman veya muadili bir araçla gerekli testler yapılabilir.

```bash
dotnet watch run
```

Örnek sorgularımızı ve beklenen çıktıları aşağıda bulabilirsiniz.

```bash
#Yeni Kahraman Ekleme
HTTP Post
http://localhost:5000/api/heroes

{
"Name": "Batman",
"AlterEgo": "Bruce Wayne",
"ForceLevel": 76
}
```

![skynet_20_Screenshot_1.png](/assets/images/2020/skynet_20_Screenshot_1.png)

```bash
#Tek bir kahraman detayını çekme
HTTP Get
http://localhost:5000/api/heroes/Wonder Woman
```

![skynet_20_Screenshot_3.png](/assets/images/2020/skynet_20_Screenshot_3.png)

```bash
#Tüm kahramanların listesini çekme
HTTP Get
http://localhost:5000/api/heroes
```

![skynet_20_Screenshot_2.png](/assets/images/2020/skynet_20_Screenshot_2.png)

```bash
#Gücü 90nın üstünde olan karakterlerin çekilmesi
HTTP Get
http://localhost:5000/api/heroes/gt/90
```

![skynet_20_Screenshot_4.png](/assets/images/2020/skynet_20_Screenshot_4.png)

```bash
#Bir kahramanı veritabanından silme
HTTP Delete
http://localhost:5000/api/heroes/Black Canary
```

ve

```bash
#Bir kahramanın verisini güncelleme
HTTP Put
http://localhost:5000/api/heroes
{
"Id":1,
"Name": "Batman",
"AlterEgo": "Bruce Wayne",
"ForceLevel": 82
}
```

![skynet_20_Screenshot_5.png](/assets/images/2020/skynet_20_Screenshot_5.png)

Tavsiyem Controller, Handler ve MediatR ilişkisini daha iyi kavrayabilmek için uygulamayı debug ederek analiz etmeniz. Peki sizce CQRS desenini Mediator ile bir arada kullanmanın avantajları neler olabilir? Bu sorunun cevabını düşünürken uygulamaya birkaç ilave daha yapabilirsiniz. Örneğin kahramanların katıldığı görevleri tutan Mission isimli bir sınıf tasarlayıp Hero ile Mission arasında çoğa çok (karmaşık gelirse bire çok da olur) ilişki tesis edip bir kahraman ve katıldığı görevler listesini kontrol edecek Handler-Query tiplerini entegre etmeye çalışabilirsiniz. Buna ek olarak kahramanın gücünün belli bir değerden küçük olması için ayrı bir Handler (LessThan isimli) yazmak yerine GreaterThan ve Equal gibi operasyonları da içerisine alacak ortak bir Handler tasarımı yapmayı düşünebilirsiniz.

Böylece geldik bir [SkyNet](https://github.com/buraksenyurt/skynet) derlemesinin daha sonuna. Örneğin tüm kodlarına [github reposundan](https://github.com/buraksenyurt/skynet/tree/master/No%2020%20-%20CQRS%20with%20Mediator) erişebilirsiniz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.