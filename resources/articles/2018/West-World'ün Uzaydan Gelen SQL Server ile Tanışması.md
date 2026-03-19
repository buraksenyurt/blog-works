---
title: "West-World'ün Uzaydan Gelen SQL Server ile Tanışması"
pubDate: 2018-06-26 03:00:00
categories:
  - .Net Core
tags:
  - .Net Core
  - Bash
  - C#
  - .NET
  - Entity Framework
  - LINQ
  - SQL Server
  - Oracle
  - JSON
  - Web API
  - HTTP
  - Docker
  - Java
  - Performance
  - Generics
  - Microservices
  - Visual Studio
---

# West-World'ün Uzaydan Gelen SQL Server ile Tanışması
![image.axd](images/image.axd)

Merhaba Arkadaşlar,

Renkler ve zevkler tartışılmaz. Hatta dünya öylesine renkli bir yerdir ki insanlar bazen neyi seçeceklerine karar veremeyebilir. Tabii ki işin içerisinde yazılım olunca bu renkler siyah ve beyaz gibi sadece iki seçeneğe de indirgenebilmişdir.

Hatta bu sadece yazılım için değil donanım için de söz konusu olmuştur. Hep bir kıyaslama vardır. PC'mi Mac'mi, RISC tabanlı mı CISC olanı mı, Intel'mi AMD'mi... Diğer yandan hepinizin bildiği üzere ben yaşlanmakta olan bir yazılımcıyım. Eski kuşak bir programcı olarak (bir başka deyişle 70li yılların ürünü bir birey olarak) benim neslin uzun yıllar içinde yer aldığı çatışmaların canlı şahitlerindenim. Hala süregelir ya bu sonu gelmez tartışmalar. Sizde içinde yer almışsınızdır mutlaka. Java'mı,.Net mi, SQL'mi Oracle'mı, OOP'mi Functional'mı, SOA'mı Microservices'mı vb diye sürer gider.

Bu ve benzeri tartışmalar daha da sürecek mi diye düşünürken şartlar uzun süre önce değişmeye başladı. Bugün bir Mac alıp üzerindeki Intel tabanlı işlemcide Windows koşturabiliyoruz. Hatta Microsoft açık kaynak dünyası ile çoktan el sıkıştı; ortaya.Net Core'u çıkardı. Yanılıyor muyum? Hatta Java Developers Day'a altın ortak bile oluverdi (Yaşıyorsa evet [şu adrese girip Gold Sponsor kısmına](https://javaday.istanbul/) bir bakın derim)

Ne alıp veremedikleri vardı ki bu ayrı dünyalarda yaşadıklarını zanneden standart koyucuların, büyük oyucuların. Bu ayrı bir hikaye ama sonuçta bugün Linux üzerinde SQL Server kullanabilir ve hatta C# üzerinden onunla konuşabilir hale geldik. Barış güzel bir şey. İşte günün konusu. Linux üzerine SQL Server kurmak ve bir Web API servisi ile CRUD operasyonlarını deneyimlemek.

SQL Server Kurulumu

Tabii ki ilk yapılması gereken Linux üzerine SQL server'ın kurulması. Her zaman ki gibi bu kurulum işleminde de terminalden yararlanacağız. Aşağıdaki komutları arka arkaya çalıştırarak işe başlayabiliriz.

```bash
wget -qO- https://packages.microsoft.com/keys/microsoft.asc | sudo apt-key add -
sudo add-apt-repository "$(wget -qO- https://packages.microsoft.com/config/ubuntu/16.04/mssql-server-2017.list)"
sudo apt-get update
sudo apt-get install -y mssql-server
```

Bu adımlardan sonra SQL Server'ın Linux için konfigüre edilmesi gerekiyor.

```bash
sudo /opt/mssql/bin/mssql-conf setup
```

Yukarıdaki komut sonrasında karşıma çıkan sorulara baktım ve Developer sürümü ile ilerleyeceğimi belirttim (Kuvvetle muhtemel ölene kadar developer olarak kalacağım:D) Bana sorulan SA (SQL'in meşhur System Admin kullanıcısı) için güzelde bir şifre belirledim.

![image.axd](images/image.axd)

İşlemlerin sorunsuz bir şekilde tamamlandığından emin olmanın yolu tahmin edileceği üzere SQL Server hizmetinin Linux ortamında çalışıp çalışmadığından emin olmak. Bunun için West-World'de aşağıdaki terminal komutunu vermem yeterliydi. Kısaca sistem kontrole mssql-server hizmetinin durumunu soruyoruz. Active durumunda olduğunu görmek yeterli.

```bash
systemctl status mssql-server
```

![image.axd](images/image.axd)

Sırada Command-Line Tool Kurulumu Var

Tabii ki SQL Server'ın West-World'e kurulması yeterli değil. Eğer Windows topraklarında olsaydım büyük ihtimalle Management Studio gibi bir şey de arayacaktım. Linux tarafında da bu tip bir araç kullanmak mümkün ama komut satırından da pekala bilinçli bir şekilde aynı işlemler halledilebilir. CLI aracının kurulumu için aşağıdaki komutların çalıştırılması yeterli.

```bash
wget -qO- https://packages.microsoft.com/keys/microsoft.asc | sudo apt-key add -
sudo add-apt-repository "$(wget -qO- https://packages.microsoft.com/config/ubuntu/16.04/prod.list)"
sudo apt-get update
sudo apt-get install -y mssql-tools unixodbc-dev
echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> ~/.bash_profile
echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> ~/.bashrc
source ~/.bashrc
```

Artık West-World üzerinde biraz SQLce (eskilerde [sqlCe](https://www.dotnetperls.com/sqlce) diye bir sürüm de vardı) konuşmaya başlayabilirim. Örneğin bir veritabanı ve tablolarını üretebilir bu tabloya veri ekleyebilirim. İşte başlamak için terminalden aşağıdaki komutu vermek yeterli.

```bash
sqlcmd -S localhost -U SA -P '...'
```

-S anahtarı sonrası bağlanacağımız sunucuyu, -U sonrası bağlanacak kullanıcıyı ve -P sonrası da bu kullanıcının şifresini belirtiyoruz. West-World güzel havadan olsa gerek oldukça ılımlı. Sorunsuz bir şekilde bağlantı isteklerini kabul edip komut satırını kullanımıma açtı. Ben de bu teklifi memnuniyetle karşıladım ve veritabanı oluşturmak için aşağıdaki komutu kullandım.

```bash
CREATE DATABASE azon
Go
```

var olan veritabanlarını görmek içinse aşağıdaki komutu. Amacım tabii ki de az önce oluşturduğum Azon isimli veritabanını sistem içerisinde görebilmekti.

```bash
SELECT Name from sys.Databases
go
```

![image.axd](images/image.axd)

Derken o an üzerinde çalışılacak veritabanını değiştirmeye ve bir tablo oluşturup ona birkaç örnek veri satırı eklemeye karar verdim.

```bash
use azon
go

create table Category(id int,name nvarchar(50))
go

insert into Category values(1,'book');
insert into Category values(2,'movie');
insert into Category values(3,'music');
go
select * from Category
go
```

Örnekte azon isimli bir veritabanı oluşturuluyor. Sonrasında bu veritabanının kullanılacağı belirtiliyor. Use ifadesinin çalıştırılması sonrası komut satırından gerçekleştirilecek tüm işlemler azon veritabanı için geçerli olacaktır. Category tablosu tamamen deneysel amaçlı. id ve name alanlarından oluşuyor. İçine 3 satır veri ekleniyor ve son olarak tüm içeriğe bir Select sorgusu atılıyor. Sonuçlar West-World için aşağıdaki gibiydi.

![image.axd](images/image.axd)

Doğruyu söylemek gerekirse West-World, SQL Server ile terminal üzerinden gayet güzel bir biçimde anlaşıyordu. Database oluşturulabiliyor, içerisine tablo konulup veriler eklenebiliyordu. Pekiiiii ya bu işin içerisine.Net Core ile yazılmış bir Web API servisini de katabilir miydim? Elebette bu mümkündü. Beni heyecanladıran kısım bunun Linux tabanlı bir sistemde olması.

Web API Servisinin Yazılması

İşe bir.Net Core Web API servisini oluşturmakla başlamak lazım. Visual Studio Code arabirimini açıp terminalden aşağıdaki komutları vererek ilerleyebiliriz.

```bash
dotnet new webapi -o Players
cd Players
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 2.0.2
```

Players isimli bir Web API uygulamamız var. Entity Framework paketinin SQL Server için yazılmış sürümünü kullanıyoruz. Servisin standart Values Controller tipini değiştirmeden önce Models isimli bir klasör oluşturup içerisine gerekli tipleri ekleyebiliriz. Bir oyuncunun temel özelliklerini barındıracak olan Player sınıfı,

```csharp
using System.ComponentModel.DataAnnotations;

namespace Players.Models  
{
    public class Player
    {
        public int PlayerId { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Team { get; set; }
        [Required]
        public int Level{get;set;}
    }
}
```

ve DbContext tipi.

```csharp
using Microsoft.EntityFrameworkCore;

namespace Players.Models  
{
    public class PlayersDbContext 
    : DbContext
    {
        public PlayersDbContext(DbContextOptions<PlayersDbContext> options)
            : base(options)
        {
            this.Database.EnsureCreated();
        }

        public DbSet<Player> Players { get; set; }
    }
}
```

Tipik bir Entity Context sınıfı söz konusu. Örneğin basit olması açısından sadece Player sınıfına ait bir veri seti sunuyor ancak siz kendi denemelerinizi yaparken Master-Child ilişkide bir yapı kurgulayabilirseniz daha iyi olabilir. Bu işlemlerin ardından ValuesController sınıfını aşağıdaki gibi değiştirerek ilerleyebiliriz.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Players.Models;

namespace Players.Controllers
{
    [Route("fabrica/api/[controller]")]
    public class PlayersController 
        : Controller
    {
        private readonly PlayersDbContext _context;

        public PlayersController(PlayersDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var playerList = _context.Players.ToList();
            return Ok(new { Players = playerList });
        }

        [HttpPost]
        public IActionResult Create([FromBody]Player player)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            _context.Players.Add(player);
            _context.SaveChanges();
            return Ok(player);
        }

        [HttpPut("{playerId}")]
        public IActionResult Update(int playerId, [FromBody]Player player)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var findResult = _context.Players.Find(playerId);
            if (findResult == null)
            {
                return NotFound();
            }
            findResult.FullName = player.FullName;
            findResult.Team = player.Team;
            findResult.Level=player.Level;
            _context.SaveChanges();
            return Ok(findResult);
        }

        [HttpDelete("{playerId}")]
        public IActionResult Delete(int playerId)
        {
            var findResult = _context.Players.Find(playerId);
            if (findResult == null)
            {
                return NotFound();
            }
            _context.Remove(findResult);
            _context.SaveChanges();
            return Ok(findResult);
        }
    }
}
```

Temel CRUD (Create Read Update Delete) operasyonlarını içeren bir Controller tipi olduğunu ifade edebiliriz. HTTP'nin POST, PUT, DELETE ve GET operasyonlarına cevap verecek metodlar içermekte. Bir oyuncu bilgisini veritabanına eklemek için Create metodu çalışıyor. Veri, HTTP talebinin Body kısmından alınmakta ([FromBody] kullanımına dikkat) Benzer durum Update metodu için de söz konusu. HTTP adresinden gelen playerID bilgisi ve paketin Body'sinden çekilen JSON içeriğine göre bir güncelleme yapılmakta. Delete metodu sadece adresten gelen playerId bilgisini kullanarak bir silme operasyonunu icra ediyor. Get metodu tahmin edileceği üzere tüm oyuncu listesini döndürmekte. Elbette tüm listeyi döndürmek çok da tercih edeceğimiz bir yöntem değil ama burada amacımız West-World'de SQL Server'ı kullanan bir API servisini.Net Core ile yazmak;)

Model, DbContext ve Controller tipleri hazır ama iş henüz bitmedi. Entity Framework hizmetinin orta katmana bildirilmesi gerekiyor. Bunun için Startup.cs sınıfındaki ConfigureServices fonksiyonunu değiştirmeliyiz. Aynen aşağıdaki gibi.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();
    var conStr = "Data Source=localhost;Initial Catalog=PlayersDatabase;User ID=sa;Password=P@ssw0rd;";
    services.AddDbContext<PlayersDbContext>(options => options.UseSqlServer(conStr));
}
```

Dikkat edileceği üzere AddDbContext fonksiyonunu kullanırken bir connection string tanımı mevcut. Bu West-World'e kurduğum SQL Server'a ulaşabilmek için gerekli. Diğer yandan generic tip olarak Models klasörüne eklediğimiz DbContext tipini belirtmekteyiz. Daha iyi olması açısından kullanıcı bilgilerini konfigurasyondan veya farklı bir ortamdan daha güvenli bir şekilde almayı deneyebilirsiniz. Konfigurasyonda okuyacaksanız kullanıcı adı ve şifre bilgilerinin kripto edilmiş hallerinin tutulmasında yarar var.

Testler

Artık testlere başlanabilir. İlk olarak,

```bash
dotnet run
```

komutu ile servisi yayına almak gerekiyor. Bundan sonrasında ise GET, POST, PUT, DELETE komutlarını ayrı ayrı deneyimlemek lazım. Ben Postman'den yararlanarak ilgili testleri gerçekleştirdim (Bu arada Postman kullanmamız şart değil. Curl terminal komutunu kullanarak da aynı işlemleri gerçekleştirebilirsiniz ki araştırıp deneyimlemenizi öneririm) İlk olarak POST ile birkaç oyuncu bilgisi ekledim.

```text
Http metodumuz : POST
Adresimiz : http://localhost:5555/fabrica/api/players
Content-Type değeri application/json
Body'de gönderdiğimiz raw tipindeki içerikse örneğin
{"FullName":"Red Skins Mayk","Team":"Orlando Pelicans","Level":90}
```

Sonuç başarılıydı.

![image.axd](images/image.axd)

Dikkati çekici noktaysa ilk isteğe cevap gelmesi için geçen yaklaşık 16 saniyelik kocaman sürey. Bunun veritabanının henüz ortada olmayışından kaynaklanan bir durum olduğu aşikar. Nitekim sonraki çağrılarda süre 200 milisaniyeler altına indi. Bir kaç oyuncu bilgisi daha ekledikten sonra Http GET operasyonu da denedim.

```text
Http Metodumuz : Get
Adresimiz : http://localhost:5555/fabrica/api/players
```

![image.axd](images/image.axd)

Sonuçların başarılı bir şekilde geldiğini görmek oldukça hoş. Hemen bir güncelleme işlemi denemenin tam sırası.

```text
Http metodumuz : PUT
Adresimiz : http://localhost:5555/fabrica/api/players/1
Content-Type değeri application/json
Body'de gönderdiğimiz raw tipindeki içerikse örneğin
{"FullName":"Red Skin Mayk","Team":"Houston Motors","Level":55}
```

İşlem başarılıydı. HTTP 200 OK mesajı dönmüştü.

![image.axd](images/image.axd)

Gerçekten de tekrar Http Get talebi gönderdiğimde aşağıdaki sonuçla karşılaştım. 1 numaralı playerId içeriği istediğim gibi güncellenmişti.

![image.axd](images/image.axd)

Gerçi ben Red Skin Mayk'a takmıştım. Onu silmeye de karar verdim.

```text
Http metodumuz : DELETE
Adresimiz : http://localhost:5555/fabrica/api/players/1
```

delete çağrısının sonucu

![image.axd](images/image.axd)

ve güncel oyuncu listesinin durumu.

![image.axd](images/image.axd)

Sql Sunucusunda Durum Ne?

Tüm bu işlemler kurulu olan SQL sunucusuna da yansımaktadır. Hemen aşağıdaki terminal komutlarını deneyerek gerçekten de orada da bir şeyler olduğunu kendi gözlerimizle görebiliriz. West-World'de durum aşağıdaki gibi gerçekleşti.

```text
sqlcmd -S localhost -U SA -P 'P@ssw0rd'
select name from sys.databases
go
use PlayersDatabase
go
select FullName,Team from Players
go
```

![image.axd](images/image.axd)

Bu makalede konu West-World açısından heyecan vericiydi. Uzun yıllar birbirlerini görmeyen iki taraf buluşmuştu. Uygulamaya sizde bir şeyler katabilirsiniz. Söz gelimi SQL Server'ı Linux ortamınıza kurmak istemediğimizi düşünelim. Ne yapabiliriz? Acaba Docker bir çözüm olabilir mi?;) Belki de Docker üzerinde konuşladıracağımız bir SQL Server hizmetini kullanabiliriz. API servisine de eklenecek bir çok şey olabilir. Bir Data Query servisi haline getirilebilir pekala. Hatta çalıştığınız yerde eğer SQL Server kullanıyorsanız ve deneysel amaçlı çalışmalar yapabileceğiniz bir ortamınız varsa SQL'in Linux üzerinde.Net Core ile yazılımış servislerle çalışması halindeki performans durumlarını da gözlemleyebilirsiniz. Her şey sizin elinizde. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

## Kaynaklar

[Microsoft'un Resmi Dokümanı](https://docs.microsoft.com/en-us/sql/linux/quickstart-install-connect-ubuntu)
