---
layout: post
title: "Docker SQL Server Kullanımı"
date: 2018-11-09 06:25:00 +0300
categories:
  - dotnet-core
tags:
  - docker
  - sql-server
  - ubuntu
  - linux
  - .net-core
  - csharp
  - entity-framework
  - web-api
  - http-post
  - http-put
  - http-delete
  - http-get
  - postman
  - crud
  - sql
  - select
---
Geçenlerde Guinness Dünya Rekorları kitabının 2018 baskısını aldım. Bu zamana kadar kayıt altına alınmış bir çok rekorun yer aldığı resimli ansiklopedi de ilgimi çeken değişik bölümler oldu. "Derinler" başlıklı kısmı okurken aklım dünyanın ilginç ve bir o kadar da zorlu mesleklerine gitti. Mesela derin sualtı dalgıcı olduğunuzu düşünsenize. İkinci dünya savaşında Norveç'in soğuk kuzey deniz sularının 245 metre derinine gömülmüş bir savaş gemisine batık dalışı yapan 12 dalgıçtan birisi olabilirdiniz. Her ne kadar 1981'de 31 gün içinde 431 külçe altın çıkartmış olsanız da kolay iş değil. Bunun gibi bir çok zorlu meslek var düşününce. Astrontlar, madencilikle uğraşanlar, yüksek gökdelen camlarını temizleyenler, İstanbul boğazındaki köprülerin bağlantı kablolarına tadilat yapanlar, hayvanat bahçelerindeki timsahlara bakanlar, bomba imhası ile uğraşan uzmanlar ve diğerleri.

![sqldocker_8.jpeg](/assets/images/2018/sqldocker_8.jpeg)

Bu meslek gruplarını düşünürkene dedim bir kaç tane atmasyon iş ilanı çıkayım. Astronot, dalgıç arayayım. Bunları West-World'de SQL Server veritabanında bir Web API ile yöneteyim dedim. Sonra gün gelir kullanıcı dostu arayüzü olan bir web uygulaması yazarım dedim. West-World'e daha önceden Microsoft SQL Server sürümünü kurmuş ve basit denemeler yapmıştım. Bu yeni çalışma için de onu kullanabilirdim ama şart değildi. SQL Server'ın Linux platformuna özel bir Docker imajını da kullanabilirdim. Hem benim için de bir değişiklik olurdu. Docker bu. Alışınca vazgeçemiyor insan.

İşte bu yazıda docker imajını baz alarak, Entity Framework çatısını kullanan basit bir Web API servisini nasıl geliştirebileceğimizi öğrenmeye çalışacağız. Vakit kaybetmeden başlayalım mı? Adımlarımız çok çok çok basit. İşe Sql Server'ın linux platformu için kullanılabilir [docker imajını](https://hub.docker.com/r/microsoft/mssql-server-linux/) yükleyerek başlamak lazım. Bunun için West-World (Ubuntu 16.04 - 64 bit) üzerinde aşağıdaki terminal komutunu kullandım (Yazı yayınlandığında kullanılan linux sql server versiyonu farklı olacaktır)

```bash
sudo docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=P@ssw0rd' -p 1433:1433 -d microsoft/mssql-server-linux:2017-CU8
```

Şu ana kadar West-World'e böyle bir docker imajı yüklemediğimden hub'dan ilgili dosyaların indirilmesi gerekti. İmaj dosyası büyük olsa da kısa bir beklemeden sonra yükleme işlemleri tamamlanmıştı.

![sqldocker_1.gif](/assets/images/2018/sqldocker_1.gif)

İmajın başarılı bir şekilde yüklendiğinden emin olmak için "docker images -all" terminal komutundan yararlandım.

![sqldocker_2.gif](/assets/images/2018/sqldocker_2.gif)

Artık bu imajı kullanacak bir Web API uygulaması geliştirmeye başlayabilirdim. Kodları yazmak için Visual Studio Code'un başına geçtim. SQL server tarafıyla konuşmak için Entity Framework'ün.Net Core sürümünü kullanmaya karar verdim. İşte kullandığım terminal komutları. Web Api projesini oluştur, EF paketini ekle, bir kere build et.

```bash
dotnet new webapi -o AdventureJobs
dotnet add package EntityFramework --version 6.2.0
dotnet build
```

Bir sonraki adımda model sınıflarını yazmaya başladım. Tipik olarak bir entity sınıfı ve DbContext türevi yazmayı planlamıştım. Model klasöründe yer alan Job isimli sınıf çeşitli iş ilanlarına ait özet bilgileri tutmakla yükümlü.

```csharp
using System.ComponentModel.DataAnnotations;

namespace AdventureJobs.Models
{
    public class Job
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Salary { get; set; }
        [Required]
        public string City { get; set; }
    }
}
```

Job sınıfı identity tipindeki Id özelliği haricinde mutlaka değer içermesi gereken özelliklere de sahip (Required niteliğini-attribute bu nedenle kullandık) ManagementContext isimli DbContext türevli sınıfı da aşağıdaki gibi yazdım.

```csharp
using Microsoft.EntityFrameworkCore;

namespace AdventureJobs.Models
{
    public class ManagementContext
    : DbContext
    {
        public ManagementContext(DbContextOptions<ManagementContext> options)
            : base(options)
        {
            this.Database.EnsureCreated();
        }

        public DbSet<Job> Jobs { get; set; }
    }
}
```

Tipik bir Context sınıfı söz konusu. Yapıcı metod (Constructor) içerisinde veritabanının var olduğundan emin olunuyor. Eğer veritabanı mevcut değilse oluşturulacak. Pek tabii bir de Controller sınıfına ihtiyacım vardı. Controllers klasörü içerisindeki JobController sınıfını da aşağıdaki gibi geliştirdim.

```csharp
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using AdventureJobs.Models;

namespace AdventureJobs.Controllers
{
    [Route("api/[controller]")]
    public class JobsController : Controller
    {
        private readonly ManagementContext _context;

        public JobsController(ManagementContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var model = _context.Jobs.ToList();
            return Ok(new { jobs = model });
        }

        [HttpPost]
        public IActionResult Create([FromBody]Job entity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Jobs.Add(entity);
            _context.SaveChanges();

            return Ok(entity);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]Job entity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
                
            var job = _context.Jobs.Find(id);

            if (job == null)
                return NotFound();

            job.Title = entity.Title;
            job.Description = entity.Description;
            job.City=entity.City;
            job.Salary=entity.Salary;

            _context.SaveChanges();

            return Ok(job);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var job = _context.Jobs.Find(id);

            if (job == null)
                return NotFound();

            _context.Remove(job);
            _context.SaveChanges();

            return Ok(job);
        }
    }
}
```

JobsController sınıfı temel CRUD operasyonlarını barındırmakta. Tüm iş listesinin çekilmesi, veritabanına yeni bir işin eklenmesi, var olan bir işin güncellenmesi veya silinmesi operasyonlarını yürütmekten sorumlu. Artık tek yapılması gereken Startup.cs içerisinde docker üzerinden servis veren SQL server için gerekli bağlantı (Connection) bilgisini bildirmek. Bunun için aşağıdaki yolu izledim (SQL Sunucusu, kullanıcı adı ve şifre bilgilerini koddan değil de bir çevre değişkeninden almak daha doğru olacaktır. Siz öyle yapın.)

```text
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc().
SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
    var conStr = $"Data Source=localhost;Initial Catalog=AdventureJobsDb;User ID=sa;Password=P@ssw0rd;";
    services.AddDbContext<ManagementContext>(options => options.UseSqlServer(conStr));
}
```

Testlere başlama zamanı gelmişti. Uygulamayı "dotnet run" terminal komutu ile başlattıktan sonra Postman'den bir kaç talep göndererek denemeler yaptım. İlk olarak http://localhost:5001/api/jobs adresine bir HTTP Get talebinde bulundum. Bu ilk çalıştırılma ve AdventureJobsDb veritabanı ortada yok. Terminal satırına düşen log bilgilerine baktığımda veritabanı ve ilgili tabloların oluşturulması için gerekli komutların başarılı bir şekilde işletildiğini gördüm.

![sqldocker_3.gif](/assets/images/2018/sqldocker_3.gif)

Hatta HTTP Get talebine uygun olarak bir SELECT sorgusu da gönderilmişti. Tabii şu anda hiçbir iş bilgisi olmadığından boş bir veri seti ile karşılaşmam son derece normaldi.

```text
Talep : HTTP Get
Adres : http://localhost/api/jobs
```

![sqldocker_4.gif](/assets/images/2018/sqldocker_4.gif)

Bunun üzerine bir kaç iş ekleyeyim dedim. Yetiştirilmek üzere bir astronot, karayiplerdeki İspanyol kalyonlarına derin dalış yapacak deneyimli bir balık adam ve Top Gear programındaki Stig'in yerini alacak usta bir şoför arayışım vardı. HTTP Post ile ve iş bilgilerini Body kısmında JSON formatında göndermek yeterliydi.

```text
Talep : HTTP Post
Address : http://localhost:5001/api/jobs
Body Content Type : application/json
Content : {"id": 1,"title": "Dalgıç","description": "Batıklara dalacak deneyimli dalgıç","salary": 10000,"city": "Karayipler"}
```

![sqldocker_5.gif](/assets/images/2018/sqldocker_5.gif)

Her POST talebi sonrası terminale düşen loglara bakmayı da ihmal etmedim. Buradan SQL Server'a gönderilen INSERT komutlarını görebiliyordum. Sonunda HTTP Get ile elime bir kaç anlamlı iş ilanı geçmişti.

![sqldocker_6.gif](/assets/images/2018/sqldocker_6.gif)

Bunun üzerine Update ve Delete operasyonlarını da denedim. Aşağıda örnek kullanımıları bulabilirsiniz.

İş güncelleme örneği;

```text
Talep : HTTP Put
Address : http://localhost:5001/api/jobs/2
Body Content Type : application/json
Body : {"title": "Astronot","description": "Yetiştirilmek üzere astronot adayları aranıyor.","salary": 20000,"city": "Houston"}
```

İş silme örneği;

```text
Talep : HTTP Delete
Address : http://localhost:5001/api/jobs/2
```

Bunlar da başarılı şekilde çalışıyordu.

Yazdığım Web API, Linux üzerinde çalışabilir bir SQL Server örneğinin docker versiyonunu kullanmakta. Vakti zamanında West-World'e bir SQL Server örneği kurmama bile gerek yokmuş aslında:) Buna ek olarak SQL Server veritabanı ile çalışmak için uygulama yazmak zorunda da değildim. SQL server'a terimalden de bağlanılabilir. Aşağıdaki ilk komut bunun için yeterli. Sonrasında temel SQL komutlarından yararlanılıyor. Ben aktif veritabanını değiştirip basit bir SELECT sorgusu çalıştırdım.

```bash
sudo docker exec -it 40af8fedc80b /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P P@ssw0rd
use AdventureJobsDb
go
select * from Jobs
go
```

(-it komutundan sonra SQL Server container ID bilgisi geliyor ancak buraya ismini de yazabiliriz.:setvar ile girilen değerler terminaldeki select çıktılarını daha kolay okumak için kullanılıyor)

![sqldocker_7.gif](/assets/images/2018/sqldocker_7.gif)

Bir Cumartesi gecesi çalışmasının daha sonuna gelmiş bulunuyorum. Docker ile zahmetsizce ulaştığım SQL imajında güzel işler yapılabileceğini görmüş oldum. Bundan sonra West-World'e bir şey kurarken iki kere düşüneceğim. İlk olarak Docker imajı var mı ona bakacağım. Böylece geldik kısa bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
