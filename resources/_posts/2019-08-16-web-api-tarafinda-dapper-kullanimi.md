---
layout: post
title: "Web API Tarafında Dapper Kullanımı"
date: 2019-08-16 10:30:00 +0300
categories:
  - dotnet-core
tags:
  - dotnet-core
  - bash
  - json
  - csharp
  - dotnet
  - entity-framework
  - linq
  - sql-server
  - mysql
  - nosql
  - rest
  - web-api
  - http
  - performance
  - generics
  - visual-studio
  - github
---
O [cumartesi gecesi çalışması](https://github.com/buraksenyurt/saturday-night-works)nı bitirdiğimde, yaptığım örneğin beni çok da tatmin etmediği gerçeğiyle karşı karşıyaydım. Bazen böyle hissediyordum. Boşa kürek çektiğim hissine kapılıyor ve neden tüm bunlarla uğraştığımı sorguluyordum. Belki de sonraki yıllar boyunca kullanmayacağım bir şeyler üzerinde çalışmıştım. Ne aldığım notları ne de senaryo için kullandığım Westwind adını beğenmiştim. Örnek çok sığdı. Zengin değildi. Bir şekilde beni rahatsız ediyordu.

![westwind.png](/assets/images/2019/westwind.png)

Gürültülü soğutma sistemi ile zamanın pancar motorlu tekneleriyle karıştırdığım emektar WestWorld'ün başından kalkıp evin basketbol sahasına bakan çalışma odasının penceresine doğru yürüdüm. Gün henüz batmak üzereyken havadaki az sayıda buluta, karşı okulun damına tünemiş bir kaç martıya, sahada şuuruzca oradan oraya koşuşturup duran çocuğa, her zaman ki gibi yanındaki bakkalla dükkanının önünde tavla oynayan Ekrem amcaya baktım. Gelip giden düşünceler eşliğinde dengeye gelmeyi ve bir kaç saattir beni terk eden iç motivasyonumu bulmaya çalışıyordum. Benimle tekrar iletişim kurması biraz zaman alsa da sonunda bir şeyler fısıldamaya başlamıştı.

İncelediğim konuyu belki şirket projelerinde veya farklı bir yerde kullanmayacaktım ama farkında olacaktım. Basit dahi olsa denediğim örnek bana konuşma hakkı verecekti. Onu en azından bir kişiyle bile paylaşabilir, karşılıklı fikir alışverişi yapıp göremediğim noktaları fark edebilirdim. Şunu unutmamak lazım ki kişisel gelişim adına yaptığımız çalışmaların hiçbiri boşa değil. Hele ki rutine bağlayıp düzenli olarak yaptıklarımızın. Mutlaka size ve öğrendiklerinizi paylaştığınız çevrenize faydası var. Öyleyse notlarımızı derlemeye başlayalım mı?

Veri odaklı uygulamalar düşünüldüğünde kalıcı depolama enstrümanları ilişkisel veya dağıtık sistemler olarak karşımıza çıkar (RDBMS veya NoSQL araçları ifade etmeye çalışıyorum) Bu sistemlerin temel görevi veriyi tutmak ve yönetmektir. Lakin son kullanıcıya hitap eden etkileşim noktalarına gelindiğinde farklı program arayüzlerinin kullanımı söz konusudur (Müşteri son yaptığı rezarvosyonla ilgili bilgileri kontrol etmek için veri tabanına bağlanıp bir sorgu cümlesi çalıştırmak istemez öyle değil mi?) Özellikle nesne yönelimli dünya söz konusuysa verinin programatik ortamda ifade ediliş biçimi de önem arz eden bir konu olarak karşımıza çıkar. Tablo, kolon gibi şematik yapıların programlama dünyasında ifade ediliş biçimleri domain odaklı kodlama yaparken işleri kolaylaştırmalıdır. İlk zamanlardaki çözümler sonrasında Object Relational Mapping adı verilen ara katman hayatımıza girmiştir. Yani veri tabanındaki nesnel varlıklar ile programatik dünyadaki örneklerin eşleştirilmesi konusundan bahsediyoruz.

Bugün bir çok ORM (Object Relational Mapping) aracı mevcut. Ben daha önceden Entity Framework, Hibernate, LLBLGen gibi araçlarla çalışma fırsatı buldum. Genel olarak veriyi tuttuğumuz taraf ile nesne yönelimli dünya arasındaki iletişimde devreye giren bu araçlarda felsefe az çok aynı. Bir noktadan sonra kullanım kolaylıkları, entegre olabildikleri sistemler, performans ve açık kaynak olma halleri ön plana çıkıyor. Ben o geceki çalışmada Stackoverflow ekibi tarafından açık kaynak olarak geliştirilen ve iyi bir [Micro ORM olarak nitelendirilen Dapper aracını](https://github.com/StackExchange/Dapper) incelemiştim. SQLite, MySQL, SQLCE, SQL Server, Firebird ve daha bir çok veri tabanı platformu ile çalışabilen Dapper'ın performans olarak da iyi sonuçlar verdiği ifade edilmekte. Genel olarak uygulamadaki amacımsa, Dapper'ı bir Web API uygulamasında SQLite ile birlikte kullanabilmek.

## SQLite Tarafındaki Hazırlıklar

Örneği her zaman olduğu gibi WestWorld (Ubuntu 18.04, 64bit) üzerinde geliştirmeye çalıştım. Sistemde o gün itibariyle SQLite yüklüydü ve hatta Visual Studio Code tarafında veri tabanı nesnelerini görebilmek için [şu adresteki](https://marketplace.visualstudio.com/items?itemName=alexcvzz.vscode-sqlite) eklentiyi kullanıyordum. Senaryoya göre dünya çapındaki üretici firmaların temel bilgilerini tutacağım bir tablo kullanacaktım. Westwind veri tabanının adıydı (Northwind çakması:P) Firm isimli bir tablo kullanmaya ve içerisinde şirket adı, merkez şehri, güncel bütçe bilgisi gibi alanlara yer vermeye karar verdim. Bu hazırlıkları sqlite3 komut satırı aracını kullanarak pekala yapabiliriz (ki artık bu andan itibaren siz benimle birlikte yazmaya başlıyor olmalısınız)

```bash
sqlite3 Westwind.db
.databases
CREATE TABLE FIRM(
ID INT PRIMARY KEY NOT NULL,
NAME TEXT NOT NULL,
CITY CHAR(50) NOT NULL,
SALARY REAL
);

INSERT INTO FIRM (ID,NAME,CITY,SALARY) VALUES (1,'Pedal Inc','Los Angles',10000);

INSERT INTO FIRM (ID,NAME,CITY,SALARY) VALUES (2,'Cycling Do','London',9000000);

SELECT * FROM FIRM;
```

İlk komutla dosya sisteminde Westwind.db isimli SQLite veri tabanı nesnesi oluşturulur..databases komutu sayesinde var olan veri tabanını görebiliriz. Takip eden CREATE, INSERT, SELECT komutları çoğunuzun aşina olduğu standart SQL cümleleridir. Tablonun oluşturulmasını takiben örnek olarak iki firma bilgisi girilmiş ve tablonun tüm içeriği terminal penceresine istenmiştir.

![07_22_Cover_1.png](/assets/images/2019/07_22_Cover_1.png)

![07_22_Cover_2.png](/assets/images/2019/07_22_Cover_2.png)

## Web API Projesinin Oluşturulması

SQLite tarafındaki başlangıç hazırlıklarımız tamamlandığına göre.Net Core Web API projesinin oluşturulmasıyla örneğe devam edebiliriz. WestwindAPI isimli projemiz iki nuget paketi kullanacak. Bunlardan birisi SQLite'ın.Net Core için yazılmış sürümü. Diğeri ise ORM aracımız olan Dapper ile ilgili. Aslında var olan SQLite tiplerini genişleteceğimiz bir paket olduğunu ifade edebiliriz (Bu arada siz örneği yazarken bulunduğunuz uzay zaman dilimine göre güncel sürümleri kontrol ederek ilerleyin. Core 3.0 ve sonrası için paketler değişikliğe uğraşmış olabilir)

```bash
dotnet new webapi -o WestwindAPI
cd WestwindAPI
dotnet add package System.Data.SQLite.Core
dotnet add package Dapper
```

Artık kod tarafına geçebiliriz. SQlite tarafı için gerekli bağlantı bilgisini appsettings.json dosyasına alarak devam edelim (Westwind dosyasını db isimli bir klasör altında tutuyoruz)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "ConnectionStrings": {
    "WestWindConStr": "Data Source=db/Westwind.db"
  },
  "AllowedHosts": "*"
}
```

Şirket bilgilerinin tutulduğu SQL tablosunu kod tarafında Firm isimli entity ile karşılayabiliriz. Entity sınıfını models isimli klasör altında aşağıdaki gibi oluşturalım. Tipik bir POCO (Plain Old CLR Object) sınıfı...

```csharp
using System;

namespace WestwindAPI.Models
{
    public class Firm
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public float Salary { get; set; }
    }
}
```

Bilindiği üzere başlangıç şablonuyla ValuesController isimli bir sınıf geliyor. İçerisinde temel CRUD operasyonlarına karşılık gelecek HTTP metodlarını barındırdığı için onu kullanabiliriz. Bu sınıfı FirmsController olarak yeniden isimlendirip Dapper kullanılacak hale getirmeye çalışalım. Mümkün mertebe koda yorum satırları katarak açıklamaya çalıştım.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WestwindAPI.Models;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Data.SQLite;

namespace WestwindAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FirmsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private string conStr;

        // appsettings içerisindeki ConnectionStrings bilgisine ihtiyacımız olacak
        // Bu nedenle .net core'un built-in configuration yöneticisini içeri alıyoruz.
        public FirmsController(IConfiguration configuration)
        {
            _configuration = configuration;
            conStr = _configuration.GetConnectionString("WestwindConStr");
        }

        [HttpGet]
        public ActionResult<IEnumerable<Firm>> Get()
        {
            // Standart get talebi sonrası Firm listesini döndürüyoruz
            IEnumerable<Firm> firms = new List<Firm>();
            // SQLite connection nesnesini oluştur
            using (var conn = new SQLiteConnection(conStr))
            {
                conn.Open(); // bağlantıyı aç
                // standart bir SQL sorgusu çalıştırıyoruz
                // isme göre sıralayarak firma bilgilerini alıyoruz
                firms = conn.Query<Firm>("SELECT * FROM FIRM ORDER BY NAME");

            }
            return new ActionResult<IEnumerable<Firm>>(firms);
        }

        // Belli bir şehirdeki firmaların bilgilerini döndüren metodumuz
        [HttpGet("{city}")]
        public ActionResult<IEnumerable<Firm>> GetByCity(string city)
        {
            IEnumerable<Firm> firms = new List<Firm>();
            // SQLite connection nesnesini oluştur
            using (var conn = new SQLiteConnection(conStr))
            {
                conn.Open(); // bağlantıyı aç
                // Bu kez işin içerisinde bir where koşulu var
                firms = conn.Query<Firm>("SELECT * FROM FIRM WHERE CITY = @FirmCity ORDER BY NAME", new { FirmCity = city });

            }
            return new ActionResult<IEnumerable<Firm>>(firms);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Firm payload)
        {
            try
            {
                using (var conn = new SQLiteConnection(conStr))
                {
                    conn.Open(); // bağlantıyı aç
                    // INSERT cümleciğini çalıştır
                    // ikinci parametreye dikkat. Burada API'ye talebin body'si ile gelen JSON içeriğini kullanıyoruz.
                    conn.Execute(@"INSERT INTO FIRM (ID,NAME,CITY,SALARY) VALUES (@ID,@NAME,@CITY,@SALARY)", payload);
                    return Ok(payload);
                }
            }
            catch (SQLiteException excp) // Olası bir SQLite exception durumunda HTTP 400 Bad Request hatası verip içerisine exception mesajını gömüyoruz
            {
                return BadRequest(excp.Message); //Bunu production ortamlarında yapmayın. Loglama yapın başka bir mesaj verin. Exception içerisinde koda ve sorguya dair ipuçları olabilir.
            }
        }

        // Güncelleme işlemleri için kullanacağımız metot
        [HttpPut()]
        public IActionResult Put([FromBody] Firm payload)
        {
            try
            {
                using (var conn = new SQLiteConnection(conStr))
                {
                    conn.Open(); // bağlantıyı aç
                    // UPDATE cümleciğini çalıştır
                    // Parametreler diğer metodlarda olduğu gibi @ sembolü ile başlayan kelimelerden oluşuyor
                    // Bu parametrelere değer atarken anonymous type de kullanabiliyoruz.

                    //TODO Aslında gelen JSON içeriğinde hangi alanlar varsa sadece onları güncellemeye çalışalım
                    var result = conn.Execute(@"UPDATE FIRM SET NAME=@firmName,CITY=@firmCity,SALARY=@firmSalary WHERE ID=@firmId",
                        new
                        {
                            firmName = payload.Name,
                            firmCity = payload.City,
                            firmSalary = payload.Salary,
                            firmId = payload.ID
                        });
                    if (result == 1)
                        return Ok(payload); // Eğer güncelleme sonucu 1 ise (ki ID bazlı güncelleme olduğundan 1 dönmesini bekliyoruz) HTTP 200
                    else
                        return NotFound(); // ID değerinde bir firma yoksa HTTP 404
                }
            }
            catch (SQLiteException excp) // Olası bir SQLite exception durumunda HTTP 400 Bad Request hatası verip içerisine exception mesajını gömüyoruz
            {
                return BadRequest(); // HTTP 400 
            }
        }

        // Silme operasyonları için çalışan metot
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            using (var conn = new SQLiteConnection(conStr))
            {
                conn.Open(); // bağlantıyı aç
                var result = conn.Execute(@"DELETE FROM FIRM WHERE ID=@firmId",new { firmId = id });
                if (result == 1)
                    return Ok(); // Eğer silme operasyonu başarılı ise etkilenen kayıt sayısı (ki bu senaryoda 1 bekliyoruz) 1 döner HTTP 200
                else
                    return NotFound(); // Aksi durumda bu ID de bir kayıt yoktur diyebiliriz. HTTP 404
            }
        }
    }
}
```

HTTP'nin Post, Put, Delete ve Get metodlarının tamamına yer verdik. Tüm operasyonlarımız IActionResult ve türevlerini döndürmekte. Pek çok noktada REST servis standartlarını yakalamaya çalışıp HTTP 200, HTTP 404 gibi mesajların karşılıkları olan Ok, NotFound benzeri metod çağrılarına yer vermekteyiz.

> Kodu incelerken Dapper'ın nerede devreye girdiği sorusunu aklınızı kurcalayabilir. SQLiteConnection nesne örneği üzerinden uygulanan Query ve Execute metodları, Dapper'a ait genişletme fonksiyonlarıdır. Yani sorguların SQLite'a iletilmesi noktasında Firm entity örnekleri ile temas edilen yerlerde çalışmaktadır.
> Esasında [açık kaynak olan bu mikro çatıyı derinlemesine incelemekte fayda var](https://github.com/StackExchange/Dapper). Nitekim dinamik parametre seçeneği ile SQL injection saldırılarından koruma gibi imkanlar da sunuyor. Üstelik bir şekilde performansı oldukça iyi. Kendim test edip sonuçları görmediğim için kesin bir şey diyemiyorum ama neyi nasıl yapıyor ya da diğerlerinden hangi noktada farklılaşıyor araştırıp öğrenmek lazım. Github kodlarını incelemek bu açıdan önemli.

## Çalışma Zamanı

Kod tarafı tamamlandığına göre servisi test etmeye başlayabiliriz. Uygulamayı terminalden

```bash
dotnet run
```

komutu ile çalıştırdıktan sonra Postman veya muadili bir aracı kullanarak API fonksiyonellikleri denenebilir. Örnek bir veri girişiyle başlayalım. Adres, HTTP metodu ve gövdede göndereceğimiz JSON içeriği şöyle olsun;

```text
http://localhost:5404/api/Firms 
POST
{"Id":21,"Name":"Dust&Dones Guitars","City":"Detroit","Salary":5250000}
```

Sonuç aşağıdaki gibi olmalıdır.

![07_22_Cover_3.png](/assets/images/2019/07_22_Cover_3.png)

Aynı ID ile tekrar giriş yapmak istersek Primary Key alanı nedeniyle bir çalışma zamanı hatası alınması gerekir. Nitekim tekillik ihlal edilmektedir.

![07_22_Cover_4.png](/assets/images/2019/07_22_Cover_4.png)

Şimdi de belli bir şehirdeki şirketleri listeleyelim.

```text
http://localhost:5404/api/Firms/Detroit
GET
```

![07_22_Cover_5.png](/assets/images/2019/07_22_Cover_5.png)

Tavsiye edilen bir servis çağrımı olmamakla birlikte tüm firmalar (1000 satırlık veriyi birden vermek çok da anlamlı olmaz gerçek hayat senaryolarında) için aşağıdaki talebi kullanalım.

```text
http://localhost:5404/api/Firms
GET
```

![07_22_Cover_6.png](/assets/images/2019/07_22_Cover_6.png)

ID bazlı bir güncelleme ile testlere devam edebiliriz.

```text
http://localhost:5404/api/Firms
PUT
{"Id":55,"Name":"Queen Marry Music LTD","City":"London","Salary":4350000}
```

![07_22_Cover_7.png](/assets/images/2019/07_22_Cover_7.png)

Son olarak ID bilgisiyle bir firmayı silerek testlerimizi tamamlayalım.

```text
http://localhost:5404/api/Firms/103
DELETE
```

Tabi o ID için bir kayıt yoksa HTTP 404 NotFound döndürdüğümüzü de görmemiz lazım.

![07_22_Cover_8.png](/assets/images/2019/07_22_Cover_8.png)

## Ben Neler Öğrendim?

Testler tamamlandı. Eğer buraya kadar geldiyseniz neler öğrendiğinizi düşünmeye çalışabilirsiniz. Ben Web API, ORM ve SQLite tarafına çok da yabancı olmadığımdan bazı şeyleri tekrar etmiş gibi oldum. Lakin SQLite ve Dapper araçlarını deneyimleme fırsatı bulduğum için bir kaç şey de öğrendim.

- Dapper Micro ORM aracının.net core tarafında nasıl kullanıldığını
- CRUD (Create Read Update Delete) operasyonlarını SQLite ile çalıştırmayı
- IActionResult/ActionResult ile Web API metodlarından nasıl sonuçlar dönebileceğimizi ve bu sayede REST standartlarına nasıl uyacağımızı

[22 numaralı örnek](https://github.com/buraksenyurt/saturday-night-works/tree/master/No%2022%20-%20Dapper%20with%20Web%20API)te mikro ORM araçlarının gözlerinden olan Dapper'ı, SQLite tabanlı olacak şekilde.Net Core 2 ile yazılmış bir Web API projesinde deneyimlemeye çalıştık. Üstüne fazlasını katmak sizin elinizde. Hoş bir arabirimin bu servisi kullanması sağlanabilir. Örneğin progressive tarzda bir web uygulaması denenebilir. SQLite tarafı bir bulut hizmetine devredilebilir. Belki de servis dockerize edilerek aynı bulut sistemi üzerinde konuşlandırılabilir. Senaryoları çeşitlendirmek mümkün. Günümüz mini mikro servis uyarlamaların çoğunda bu tip ORM araçlarının kullanıldığı da ortada diyerek son mesajımı vereyim. Böylece geldik pratik kazandıran bir cumartesi gecesi derlemesinin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
