---
layout: post
title: "Effective Engine — Bir Uzay Macerası"
date: 2021-05-14 18:22:00 +0300
categories:
  - aspnet-web-api
tags:
  - aspnet-web-api
  - bash
  - csharp
  - javascript
  - json
  - dotnet
  - entity-framework
  - ef-core
  - linq
  - sql-server
  - postgresql
  - nosql
  - xml
  - soap
  - rest
  - web-api
  - web-service
  - http
  - grpc
  - authentication
  - docker
  - blazor
  - async-await
  - transactions
  - generics
  - dependency-injection
  - github
  - dependency-management
---
Altunizade’nin bahar aylarında insanı epey dinlendiren yeşil yapraklı ağaçları ile çevrelenmiş caddesinin hemen sonunda, köprüye bağlanmadan iki yüz metre kadar öncesinde dört katlı bir bina vardır. Araba ile geçerken eğer kırmızı ışığa denk gelmediyseniz pek fark edilmez ama yürürken önündeki geniş kaldırımları ile dikkat çeker. Ana cadde tarafındaki yeşillikler binanın ilk katlarını gizemli bir şekilde saklar. Binanın bulunduğu adanın etrafı cadde ve ara sokaklarla çevrilidir. Bir sokağın karşısında yeni yapılmış hastane ile metro çıkışı, ana cadde tarafında ev yapımı tatlılarının kokusu ile insanı baştan çıkaran pastane, eczane, kuaför, camii ve minik bir park bulunur. Dört yola bakan diğer cadde tarafında ise eskiden karakol olan ama çok uzun zamandır kreş olarak işletilen bir komşu yer alır.

![asset_cover.png](/assets/images/2021/asset_cover.png)

Bu binanın bende özel bir yeri vardır. Bir zamanlar Netron adıyla da bilinen yazılım eğitim kurumlarının genel merkeziydi. Şirket eğitimlerindeki başarısı ve iyi eğitmenleri ile adından söz ettirmiş bir kurumdu. 2005 yılının sonlarına doğru adım attığım eğitmenlik kariyeriminin başlangıç noktasıydı. Geniş, yemyeşil bir bahçesi ve Proxy isimli bir köpeği vardı. Her sabah o güzel bahçeye bakan camekanlı kafesinde taze simit ve poğaçalar olur, ücretsiz dağıtılırdı. İsteyen istediği kadar alabilirdi. Camekanlı kafeye giriş kapsının önündeki basamaklardan indiğinizde solunuzda kalan birde bilgisayar vardı. Şöyle CRT tüplü monitörü olan bir bilgisayar. Otururak kullanabildiğiniz değil de bir bar sandalyesi üstünde hafif rahatsız biçimde kullanabildiğiniz yavaş bir bilgisayar. Uzunca bir süre sınıflardaki birkaç eğitmen bilgisayarı dışında o binada internete bağlanabilinen tek bilgisayar O olmuştu. Şifresiz ve herkesin kullanımına açıktı. O zamanlar öğrencilerin ders sırasında veya arasında sınıftaki bilgisayarları kullanarak internete çıkmalarını pek istemezdik. Nitekim derste öğretilenler ile bir şeyleri çözebileceklerini umut eder, onları buna yönlendirmeye çalışırdık — Lakin bir defasında Ogame filosuna saldırdıkları için oldukça endişeli görünen bir öğrencim sebebiyle ders arasını birkaç dakika erken vermiştim.

O yıllarda eğitimler genellikle Microsoft’un sertifika sınavlarına göre şekillendirdiği müfredata uygun olarak verilirdi. Microsoft’un eğitimciler için hazırladığı eğitim dokümanları epey kallavi olurdu ve ön hazırlıkları bile zaman alırdı. Bazen bir ders saati için tüm haftasonumu heba ettiğim olurdu. özellikle kurumsal bir eğitim söz konusu ise şirketlerin deneyimli personelinden gelen acımasız soruları cevaplayabilmek adına ekstra efor sarf etmek gerekiyordu. Bu durum zamanla deneyim kazanan eğitmenler için sorun olmasa da yeni başlayan bir eğitmen ve onun ilk öğrencileri için bazı hazin sonuçlara sebebiyet verebilirdi. Derken sektörün ihtiyaçlarına bakıp kendi içeriklerimizi planlamaya başladık. Hatta çoğu zaman kendi eğitim materyallerimizi hazırladık. Keyifli ama bir o kadar da külfetli bir işti. Nitekim sahip olunan insan gücü düşünüldüğünde sürekli değişen yeniliklere adapte olmak ve materyalleri güncellemek başlı başına zor oluyordu. Sanki sadece yenilikleri takip edip bu materyalleri hazırlayacak ayrı bir ekip gerekliydi. Gerçi bu endişeler çok geride kaldı.

Profesyonel anlamda eğitmenliği bırakalı oldukça uzun bir zaman oldu. En azından on yıldan fazladır bir eğitim kurumunda eğitmen olarak görev almıyorum. Sadece son dönemlerde çalıştığım firmalar iç eğitmenlik programları kapsamında benden destek istediler. Elimden geldiğince yardımcı olmaya çalıştım/çalışıyorum. İşte 2021'in şeker bayramında eve kapandığımız vakitlerde böyle sıfırdan uzun soluklu bir eğitim vermem gerekse nasıl hazırlanırdım diye düşünmeye başladım.

öyle ya, artık eğitimlerin veriliş şekilleri ve eğitmenlerden beklentiler çok değişti. Artık sınıf eğitimlerinden ziyade çevrimiçi ulaştığımız ve önceden hazırlanıp kaydedilmiş eğitimler daha popüler görünüyor. Ekran görüntüsü kaydetmenin, akış olarak internet ortamına yüklemenin çok daha kolay olduğu bu zaman diliminde hafif hazırlıklar ile bir eğitimi sunmak daha kolay görünüyor — sizi sıkıştıran anlık soruların olmadığı bir ortam olması sebebiyle bireysel anlamda yeni nesil eğitmenleri ne kadar zorluyor bu da tarışılır tabii. Bununla birlikte uzun metrajlı içerikler algı ve odaklanma sürelerimize göre yerini mikro anlatımlara bırakıyor. Fiziki sınıf eğitimlerinde karşımızdakilerle beden dili kullanarak kurduğumuz sıcak ilişkiyi çevrimiçi ortamda sağlamak zor olduğundan, Icebreaker denen oyunlaştırılmış araçlar kullanılıyor — ki bana göre hiçbir şey gerçek anlamda görsel ve işitsel temas ile kurulan bağın ötesine geçemez.

Ancak her ne olursa olsun yazılım alanındaki bir eğitimin bazı temel prensipleri ve araçları değişmemelidir. Bu anlamda inandığım bazı ilkeler var.

- Halen daha eğitmenin konuya olan hakimiyeti çevrimdışı bir eğitim bile olsa çok önemli. Bu hakimiyet öğrenciye sorulacak sorular için de zemin hazırlıyor. Yeri geldiğinde karşıda video kaydedici bile olsa düşündürücü bir soru yöneltip es vermek gerekiyor.
- Kendi eğitmenlik zamanlarımda da önem arz eden bir diğer konu ise saha tecrübesi. Teorik bilgi birikimi ne kadar yüksek olursa olsun sahada karşılaşılan problemlerin verdiği tecrübe aktarımı bir başka oluyor. Nitekim yazılım eğitimlerinin en zor kısımlarından birisi soyutlaşan kavramların gerçek hayatla örtüştüğü noktaları karşı tarafa aktarabilmek — ki benim üstadlarım bana “gerekirse konuyu çöp adam kullanarak tahtaya çizip anlatmaya çalış” demişti. Tahtanın yerini şimdi Whiteboard ve dokunmatik ekranlar aldı belki ama prensip aynı; Basitleştirerek anlatmak.
- Bir eğitim her şeyi eğitmenin yaptığı değil aksine öğrencinin de bir şeyler yaptığı şekilde olmalı. çünkü araştırma ve sorgulayarak cevap bulma kasları bilişim sektörü personeli için çok önemli. Bu nedenle eğitmenin bilhassa açık bıraktığı bazı noktaları keşfetmesi için öğrencilerine ödevler vermesi gerekiyor.
- Görev addetmek mühim bir mesele olsa da onu takip etmek ve karşılıklı müzakere yollarını keşfederek önerilerde bulunmak çok daha önemli. öneride bulunup öğrencinin yerine yapmaya çalışmak ise iyi bir pratik değil.
- Senaryolaştırmak, bazen konunun ne olduğuna bağlı olarak öncesinde bitmiş eseri gösterip sonra adım adım ilerletmek de kıymetli bir yaklaşım. Bazen yalın bir Hello World hiçbir şey ifade etmez ama senaryosu olan bir Hello World çok şey ifade edebilir.
- Eğitime konu olan örneklerin bütünlüğü de kritik bir mesele. Pek çok uygulamalı kitabın ilk noktasından son noktasına gelindiğinde, anlatılan her şeyin kullanıldığı bir ürün ortaya çıkmış oluyor. Bu pratiği eğitimin kendisine yaymak kolay değil ve daha da önemlisi oldukça titiz bir hazırlık süreci gerektiriyor.

Düşünceme göre bir eğitime hazırlanmak gerçekten de kolay değil. Büyük sorumluluk, büyük mesuliyet, iyi hazırlık, iyi meziyet gerektirmekte. Bu vesile ile bende bir deneme yapmak istedim. öncesinde geçmiş yıllarımdan bir tecrübe birkaç anı kırıntısı bulmaya çalıştım. Havaların erken karardığı ve eğitimin akşam 19:00da başladığı bir Netron gününde anlattığım Xml Web Service konusu geldi aklıma. Ne ilginçtir ki o zamandan beri Microsoft’un birçok materyalindeki konsept değişmedi. O bir saatlik eğitime hazırlandığım resmi Microsoft eğitim dökümanının ilgili bölümünde bir hava durumu servisinin geliştirilmesi öğretilmekteydi.

önce bitmiş Web servis çalıştırılıp elde edilmesi beklenen sonuç gösteriliyor, sonrasında Request nesnesine ait XML’in hazırlanması öğretiliyor ve nihayetinde bu işin geliştirme ortamında nasıl yapılacağına değiniliyordu. Felsefesi gayet doğruydu. Bir Web Servis temel olarak ne işe yarar baştan görebiliyordunuz. Eğitim seviyesi sebebiyle iç dinamiklerinde XML ne anlama geliyor, bir SOAP talebi hangi parçalardan oluşuyor öğreniyordunuz. Ne elde edeceğim, nasıl çalışıyor terapisinden sonra pratiğe geçiliyor ve uygulama geliştiriliyordu. Bugünkü.Net 5 dünyasına — veya.Net Core tarafına baktığımızda da bir Web API servisi söz konusu ise benzer bir senaryo koşulduğunu biliyor olmalısınız. Artık bilinç altımıza işlemiş olan, her şablonda karşımıza çıkan WeatherForecast senaryosu.

Pek çok saygın kitapta veya çevrimiçi eğitimde olduğu gibi uzun vadedeki çözümler veri odaklı bir dünya üzerine inşa ediliyor. Ulaşılmak istenen nokta ister Blazor ister MVC olsun, ister Progressive Web App ister Mobil çözüm veya bir başkası olsun o büyük kitabın veya sekiz saatlik eğitimin başlarında bir yerlerde bir REST veya gRPC servisi söz konusu oluyor. üstelik bu servis pratik olması açısından genellikle In-Memory tabanlı, Docker ile kurgulanmış veya local formasyonda çalışan bir veritabanı kullanıyor. İşte eğitimcinin yaratıcılığı bu noktada başlıyor. Renkli bir senaryo kurgulamak, hikayenin bazı noktalarında öğrencinin isteklerini kabul ederek yeni şeyleri sürece katmak (Farklı entity nesneleri veya fonksiyonellikler gibi), In-Memory veritabanı ile başlatıp Docker ile diğer türlere geçişlerin yapılacağı ödevler vermek vs

Peki ya bunu nasıl yapacak? İşte bir eğitmenin bence sahip olması gereken en önemli özelliklerden birisi. Yazarak adım adım planlamak. Ben bu tip bir işe kalkışsam sanırım en büyük yardımcılarım Markdown formatındaki bir Readme dosyası ile [kodları planlayarak tutabileceğim github benzeri bir kaynak deposu](https://github.com/buraksenyurt/effective-engine) olurdu. Hatta o depoyu da belki branch’ler ile kurgulayarak önce şöyle, şimdi böyle, sonra da öyle gibi ifade etmek gerekirdi. [örneğin…](https://github.com/buraksenyurt/hands-on-aspnetcore-di)

Sonuç olarak aşağıdaki gibi amatör bir kurgu oluşturdum. Umarım şirket içi eğitmenlere yol gösterici olur.

## Senaryo

Haftasonu sıkılan.Net geliştiricisi için eğlencelik bir Web API kodlaması düşündüm. Şirket içi eğitimlerde bir Web API’ye ihtiyaç duyduğumuz durumlar için güzel olabilir. Hani kobay bir Web API servisi olur ya hep, görsellik katılınca sükseli duran. İşte onun için güzel bir senaryo olabileceğini düşünüyorum. Senaryoyu aşağıdaki gibi çizmeye çalıştım.

![assets_01.png](/assets/images/2021/assets_01.png)

Gelecekte geçen bir zaman diliminde galaksinin uzak diyarlarını keşfetmek üzere Uzay Yolu’nu izlemiş mürettabattan oluşan gemiler vardır. Güneşin ve ayın konumuzla bir alakası yok ama kompozisyonu tamamlarlar diye düşünüp resme dahil ettim. Bir uzay gemisi (Spaceship) içinde en az 2 en fazla 7 mürettebat (Voyager) olabilir. Mürettebat görev kontrolün (MissionControl) uygun gördüğü gemiyle bir göreve (Mission) çıkar. Her görev tek bir gemiyle ilişkilendirilir ama itirazınız varsa bunu çoklayabiliriz de. Görevin başlatılması için bir adının olması, kendilerine has takma isimleri olan mürettebatın bulunması, görev süresi verilmesi (En az 12 en fazla 24 ay), bir gemiyle görevin ilişkilendirilmesi yeterlidir. Senaryoyu birlikte genişletebiliriz ama varsayılan hali aşağıdaki gibidir.— Burası öğrencilere senaryonun anlatıldığı kısım. Eğlenceli olmalı, ilgi çekmeli, hatta eğitmen bunu canlı olarak çizerek anlatmalıdır.

## 0 — Başlangıç

Solution ve projenin ilk aşamasıdır. Giriş kısmı olduğu için sarf edilen sözler önemlidir. Neden bir Class Library açarak başladık ve ona neden EntityFrameworkCore diye bir paketi ekledik anlatmamız gerekir.

```bash
# Bir Solution oluşturdum
dotnet new sln -o GalaxyExplorer

# Sonra Voyager, Spaceship ve Mission olarak adlandırdığım nesneler için Entity ile DbContext'in duracağı bir class library oluşturup solution'a ekledim.
cd GalaxyExplorer
dotnet new classlib -o GalaxyExplorer.Entity
dotnet sln add .\GalaxyExplorer.Entity\GalaxyExplorer.Entity.csproj

# EntityFrameworkCore kullanacağım için birde gerekli paketi ekledim
cd GalaxyExplorer.Entity
dotnet add package Microsoft.EntityFrameworkCore -v 5.0.6
```

## 1 — Entity Sınıflarının İnşası

Uzay gemilerini Spaceship sınıfı ile işaret edeceğiz. Adı ve ışık yılı olarak gidebileceği mesafeyi taşıması yeterli.

```csharp
namespace GalaxyExplorer.Entity
{
    public class Spaceship
    {
        public int SpaceshipId { get; set; }
        public string Name { get; set; }
        public double Range { get; set; }
        public bool OnMission { get; set; }
        public int MaxCrewCount { get; set; }
    }
}
```

Mürettebatı ise Voyager olarak tanımlayabiliriz. Şimdilik aşağıdaki gibi kullanacağız. Kaşifin adı, rütbesi, ilk görev tarihi, aktif olup olmadığı bilgileri olsun yeterli.

```csharp
using System;

namespace GalaxyExplorer.Entity
{
    public class Voyager
    {
        public int VoyagerId { get; set; }
        public string Name { get; set; }
        public string Grade { get; set; }
        public DateTime FirstMissionDate { get; set; }
        public int MissionId { get; set; }
        public bool OnMission { get; set; }
    }
}
```

Bir görev söz konusu. Bunu Mission sınıfı ile temsil edebiliriz. Bir görev bir gemiyle ilişkili olmalıdır diye ifade etmiştik. Ayrıca bir göreve birden fazla mürettebat da dahil olabilmelidir. Bu düşünceleri resmeden bir sınıfı aşağıdaki gibi yazabiliriz.

```csharp
using System;
using System.Collections.Generic;

namespace GalaxyExplorer.Entity
{
    public class Mission
    {
        public int MissionId { get; set; }
        public int SpaceshipId { get; set; }
        public string Name { get; set; }
        public int PlannedDuration { get; set; }
        public DateTime StartDate { get; set; }
        public IEnumerable<Voyager> Voyagers { get; set; }
    }
}
```

“Neden bu entity sınıflarını inşa ediyoruz?” diye sormalı karşılıklı görüş almalıyız.

2 — DbContext Sınıfının Yazılması

Senaryomuzda hangi veritabanını kullanacağımıza henüz karar vermedik lakin Entity Framework Core’dan yararlanmaktayız. Code First modeli ile ilerliyoruz ama Model First ve Database First şeklinde farklı versiyonlar olduğunu da hatırlayalım. Şu anda Domain’e ait tipleri tasarlayıp sonrasında veritabanına geçeceğiz. İlerleyen derslerde isteyen istediği veritabanı ile çalışabilir olacak (Uygun olan veritabanı tabii) Bu amaçla GalaxyExplorerDbContext sınıfını aşağıdaki gibi yazarak devam edelim. İçinde kullanıma hazır uzay gemileri de var — Burada öğrencilerden de uzay gemisi adları alabiliriz. Hayal güçlerini kullanmaları her zaman etkileşimi yükseltir

```csharp
using Microsoft.EntityFrameworkCore;

namespace GalaxyExplorer.Entity
{
    public class GalaxyExplorerDbContext
        : DbContext
    {
        public GalaxyExplorerDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Spaceship> Spaceships { get; set; }
        public DbSet<Voyager> Voyagers { get; set; }
        public DbSet<Mission> Missions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Mission>().HasMany(m => m.Voyagers).WithOne();

            modelBuilder.Entity<Spaceship>().HasData(
                new Spaceship
                {
                    SpaceshipId=1,
                    Name = "Saturn IV Rocket",
                    OnMission = false,
                    Range = 1.2,
                    MaxCrewCount=2
                },
                new Spaceship
                {
                    SpaceshipId = 2,
                    Name = "Pathfinder",
                    OnMission = true,
                    Range = 2.6,
                    MaxCrewCount = 5
                },
                new Spaceship
                {
                    SpaceshipId = 3,
                    Name = "Event Horizon",
                    OnMission = false,
                    Range = 9.9,
                    MaxCrewCount = 3
                },
                new Spaceship
                {
                    SpaceshipId = 4,
                    Name = "Captain Marvel",
                    OnMission = false,
                    Range = 3.14,
                    MaxCrewCount = 7
                },
                new Spaceship
                {
                    SpaceshipId = 5,
                    Name = "Lucky Tortiinn",
                    OnMission = false,
                    Range = 7.7,
                    MaxCrewCount = 7
                },
                new Spaceship
                {
                    SpaceshipId = 6,
                    Name = "Battle Master",
                    OnMission = false,
                    Range = 10,
                    MaxCrewCount = 5
                },
                new Spaceship
                {
                    SpaceshipId = 7,
                    Name = "Zerash Guidah",
                    OnMission = true,
                    Range = 3.35,
                    MaxCrewCount = 3
                },
                new Spaceship
                {
                    SpaceshipId = 8,
                    Name = "Ayran Hayd",
                    OnMission = false,
                    Range = 5.1,
                    MaxCrewCount = 4
                },
                new Spaceship
                {
                    SpaceshipId = 9,
                    Name = "Nebukadnezar",
                    OnMission = false,
                    Range = 9,
                    MaxCrewCount = 7
                },
                new Spaceship
                {
                    SpaceshipId = 10,
                    Name = "Sifiyus Alpha Siera",
                    OnMission = false,
                    Range = 7.7,
                    MaxCrewCount = 7
                }
            );
        }
    }
}
```

3 — DTO Tipleri için Bir Kütüphane Oluşturulması

Görev kontrol tarafına ilk etapta sadece bir başlatma emri gelsin istiyoruz. Görevin adı, katılacak mürettebatın isimleri gibi az sayıda bilgi yeterli olabilir. Entity türlerini doğrudan API üzerinden açmak yerine bir ViewModel vasıtasıyla sadece aksiyona özgü değişkenlerle sunmak niyetindeyiz. O yüzden Data Transfer Object olarak düşünülebilecek sınıfları kullanacağız— “DTO’lar yazılım dünyasının hangi noktasında karşımıza çıkarlar? Bu senaryoda ki kullanım amaçları dışında bir rolleri olabilir mi?” şeklinde sorular sorup müzakere etmek gerekiyor.

```bash
# DTO Projesini açtım
dotnet new classlib -o GalaxyExplorer.DTO

# ve Solution'a ekledim
dotnet sln add .\GalaxyExplorer.DTO\GalaxyExplorer.DTO.csproj
```

Sonrasında yeni bir görev başlatmak için kullanacağımız aşağıdaki DTO sınıflarını ekleyerek devam edelim.

Göreve katılacak mürettebat için VoyageRequest sınıfı.

```csharp
using System.ComponentModel.DataAnnotations;

namespace GalaxyExplorer.DTO
{
    public class VoyagerRequest
    {
        [Required]
        [MinLength(3)]
        [MaxLength(25)]
        public string Name { get; set; }
        [Required]
        public string Grade { get; set; }
    }
}
```

Görevin kendisi içinse MissionStartRequest sınıfı. En az iki en fazla yedi mürettebat katılabilen görevlerden bahsetmiştik. Gemi ataması ise havuzdaki müsait olanlardan yapılmalı. Bu yüzden görev gemisi ile ilgili bir bilgi eklemedik. Bu noktada da fark edeceğiniz üzere bir görevi başlatmak için ihtiyaç duyulan veri modeli ile Entity tam olarak örtüşmüyor. İşte Data Transfer Object için bir başka bahane. — Bu noktada gerçekten doğru bir şeyler söylüyor muyum diye sorgulatmak lazım. öğrencilerle tartışılması gereken bir konu daha.

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GalaxyExplorer.DTO
{
    public class MissionStartRequest
    {
        [Required]
        [MinLength(10)]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [Range(12,24)] // En az 12 en fazla 24 aylık görev olabilir
        public int PlannedDuration { get; set; }
        [Required]
        [MinLength(2)]
        [MaxLength(7)] //Minimum 2 maksimum 7 mürettebat olsun diye
        public List<VoyagerRequest> Voyagers { get; set; }
    }
}
```

Görevi başlatma sırasında oluşacak hatalar ile ilgili ayrı bir dönüş tipi kullanmak yararlı olabilir. Bunu sağlamak için MissionStartResponse sınıfını ekleyebiliriz. — Bu noktada “Servis portlarının girdi ve çıktı mesajlarında bir standart kullnamak gerekir mi?” sorusunu sorup tartışabiliriz.

```csharp
namespace GalaxyExplorer.DTO
{
    public class MissionStartResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
```

Başka ne tür validasyon nitelikleri kullanılabilir, araştırmalarını söyleyebiliriz. Tabi söylemek yetmez takibini de yapmamız gerekir. Bir sonraki ders kısa bir tekrar sonrası sorulan sorular üstünde tartışmak verimli bir öğrenim süreci sağlar.

4 — Servis Bileşenleri için Kütüphane Eklenmesi

Web API haricinde buradaki kurguyu farklı bir ortamda da kullanmak isteyebiliriz. Controller tipinin kullanacağı Entity Framework işlerini başka bir kütüphanede toplayacak şekilde proje bazında soyutlasak güzel olabilir. Hatta servisleştirirsek çok daha iyi olur. Böylece Dependency Injection çatısını kullanarak asıl ürüne eklememiz de kolay olur. önce bir kütüphane oluşturalım ve gerekli projeleri referans edelim— Dependency Injection. Hassas, çok hassas bir konu. Burada gerekirse uzun süreli es verip karşılıklı konuşmak, [önceki derslerde anlatılan kısımlara refernas](https://medium.com/dogustech/asp-net-corea-nas%C4%B1l-merhaba-deriz-4469c0e06ff5) ederek yönlendirmek gerekebilir.

```bash
# Projeyi oluştur
dotnet new classlib -o GalaxyExplorer.Service
# Solution'a ekle
dotnet sln add .\GalaxyExplorer.Service\GalaxyExplorer.Service.csproj
# Proje içine gir
cd .\GalaxyExplorer.Service
# DTO projesini referans et
dotnet add reference ..\GalaxyExplorer.DTO\GalaxyExplorer.DTO.csproj

# DbContext'e ihtiyacım olacak.
dotnet add reference ..\GalaxyExplorer.Entity\GalaxyExplorer.Entity.csproj
```

önce soyutlamayı sağlayacak arayüz tipini ekleyelim.

```csharp
using GalaxyExplorer.DTO;
using System.Threading.Tasks;

namespace GalaxyExplorer.Service
{
    public interface IMissionService
    {
        Task<MissionStartResponse> StartMissionAsync(MissionStartRequest request);
    }
}
```

Sonra asıl işi yapan sınıfı (Concrete Class) yazalım.

```csharp
using GalaxyExplorer.DTO;
using GalaxyExplorer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GalaxyExplorer.Service
{
    public class MissionService
        : IMissionService
    {
        private readonly GalaxyExplorerDbContext _dbContext;
        // Servisi kullanan uygulamanın DI Container Service Registery'si üzerinden gelecektir.
        // O anki opsiyonları ile birlikte gelir. SQL olur, Postgresql olur, Mongo olur bilemiyorum.
        // Entity modelin uygun düşen bir DbContext gelecektir.
        public MissionService(GalaxyExplorerDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<MissionStartResponse> StartMissionAsync(MissionStartRequest request)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(); // Transaction başlatalım
            try
            {
                // Mürettebat sayısı uygun olup aktif görevde olmayan bir gemi bulmalıyız. Aday havuzunu çekelim.
                var crewCount = request.Voyagers.Count;
                var candidates = _dbContext.Spaceships.Where(s => s.MaxCrewCount >= crewCount && s.OnMission == false).ToList();
                if (candidates.Count > 0)
                {
                    Random rnd = new();
                    var candidateId = rnd.Next(0, candidates.Count);
                    var ship = candidates[candidateId]; // Index değerine göre rastgele bir tanesini alalım

                    ship.OnMission = true;
                    await _dbContext.SaveChangesAsync(); // Gemiyi görevde durumuna alalım

                    // Görev nesnesini oluşturalım
                    Mission mission = new Mission
                    {
                        Name = request.Name,
                        PlannedDuration = request.PlannedDuration,
                        SpaceshipId = ship.SpaceshipId, // Gemi ile ilişkilendirdik
                        StartDate = DateTime.Now
                    };
                    await _dbContext.Missions.AddAsync(mission);
                    await _dbContext.SaveChangesAsync(); // Görev nesnesini db'ye yollayalım

                    // Gelen gezginlerin listesini dolaşıp
                    var voyagers = new List<Voyager>();
                    foreach (var v in request.Voyagers)
                    {
                        Voyager voyager = new Voyager // Her biri için bir Voyager nesnesi örnekleyelim
                        {
                            Name = v.Name,
                            Grade = v.Grade,
                            OnMission = true,
                            MissionId = mission.MissionId // Görevle ilişkilendirdik
                        };
                        voyagers.Add(voyager);
                    }
                    await _dbContext.Voyagers.AddRangeAsync(voyagers); // Bunları topluca Voyagers listesine ekleyelim
                    await _dbContext.SaveChangesAsync(); // Değişiklikleri kaydedelim.
                    await transaction.CommitAsync(); // Transaction'ı commit edelim

                    return new MissionStartResponse
                    {
                        Success = true,
                        Message = "Görev başlatıldı."
                    };
                }
                else // Müsait veya uygun gemi yoksa burda durmamızın anlamı yok
                {
                    await transaction.RollbackAsync();

                    return new MissionStartResponse
                    {
                        Success = false,
                        Message = "Şu anda görev için müsait gemi yok"
                    };
                }                
            }
            catch (Exception exp)
            {
                await transaction.RollbackAsync();
                return new MissionStartResponse
                {
                    Success = false,
                    Message = $"Sistem Hatası:{exp.Message}"
                };
            }
        }
    }
}
```

Yazılan servis kodundan çeşitli sorular sorulabilir. örneğin hangi tür injection tekniği kullanılmaktadır, başka ne türleri vardır, veritabanı belli midir, belli ise bağlantı bilgisi nerededir, transaction açılmasının sebebi nedir, temel transaction ilkeleri nelerdir vb. Buradan yola çıkarak “BASE’i duymuş muydunuz?” diye bir soru sorulabilir ve NoSQL ilkelerine geçilip dağıtık sistemler için önem arz eden CAP teoremine atıfta bulunulabilinir. Detaylar ders harici zamanlarda merak edenlerle konuşulur veya araştırma ödevi olarak atanır.

5 — Sırada Controller var. Yani Web API’nin İnşası

önce projeyi oluşturup gerekli paketleri ve proje referanslarını aşağıdaki gibi ekleyelim.

```bash
# Web API projesini oluştur
dotnet new webapi -o GalaxyExplorer.API
# Solution'a ekle
dotnet sln add .\GalaxyExplorer.API\GalaxyExplorer.API.csproj
# Proje klasörüne geç
cd .\GalaxyExplorer.API
# EntityFrameworkCore paketini ekle
dotnet add package Microsoft.EntityFrameworkCore -v 5.0.6
# Local SQL kullanmak istedim. Onun paketini ekle
dotnet add package Microsoft.EntityFrameworkCore.SqlServer -v 5.0.6
# Migration için gerekli olacak paket
dotnet add package Microsoft.EntityFrameworkCore.Design -v 5.0.6

# WeatherForecast* tiplerini sildim

# Service ve DTO projelerini referasn ettim
dotnet add reference ..\GalaxyExplorer.Service\GalaxyExplorer.Service.csproj
dotnet add reference ..\GalaxyExplorer.DTO\GalaxyExplorer.DTO.csproj
dotnet add reference ..\GalaxyExplorer.Entity\GalaxyExplorer.Entity.csproj
```

Startup.cs içerisindeki ConfigureServices metodunu da takip eden kod parçasında olduğu gibi düzenleyelim.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // DI serivslerine DbContext türevini ekliyoruz. 
    services.AddDbContext<GalaxyExplorerDbContext>(options =>
    {
        // SQL Server baz alınacak ve appsettings.json'dan GalaxyDbConnStr ile belirtilen bağlantı bilgisi kullanılacak.
        options.UseSqlServer(Configuration.GetConnectionString("GalaxyDbConnStr"), b => b.MigrationsAssembly("GalaxyExplorer.API"));
    });
    services.AddControllers();
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "GalaxyExplorer.API", Version = "v1" });
    });
}
```

Bu senaryo özelinde makinelerimizde de hazır olması sebebiyle Local SQL Server’ı kullanmayı tercih edebiliriz. Gerekli ConnectionString bilgisini AppSettings.json dosyasına aşağıdaki gibi eklemek gerekir — Sınıfı katılımcı sayısına göre gruplara bölüp farklı veritabanı ile çalışmalarını da sağlatabiliriz. Postgresql’in Docker Container kullanan bir versiyonu ideal çözüm olabilir.

```javascript
"ConnectionStrings": {
      "GalaxyDbConnStr": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=GalaxyExplorer;Integrated Security=True"
    }
```

Ardından projeye MissionController isimli bir Controller sınıfını ekleyelim.

```csharp
using GalaxyExplorer.DTO;
using GalaxyExplorer.Service;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GalaxyExplorer.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class MissionController : ControllerBase
    {
        // DI Container'a kayıtlı IMissionService uyarlaması kimse o gelecek
        private readonly IMissionService _missionService;
        public MissionController(IMissionService missionService)
        {
            _missionService = missionService;
        }
        [HttpPost]
        public async Task<IActionResult> StartAsync([FromBody] MissionStartRequest request) // JSON Body'den request nesnesini alsın
        {
            if (!ModelState.IsValid)
                return BadRequest(); // Model validasyon kurallarında ihlal olursa

            // Servis metodunu çağıralım
            var startResult = await _missionService.StartMissionAsync(request);
            if (startResult.Success) // Sonuç başarılı ise HTTP OK
                return Ok(startResult.Message);
            else
                return BadRequest(startResult.Message); // Değilse HTTP Bad Request
        }
    }
}
```

Controller sınıfının IMissionService implementasyonunu kullanabilmesi için Startup dosyasında yer alan DI servislerine gerekli bildirimi yapmayı da ihmal etmemek lazım. — Kullanılan Constructor Injection tekniğine göre bu Controller’a talep geldiğinde hazır edilecek bileşenin nerede bildirilmesi gerektiğini önce sınıfa soralım, sonrasında biz gösterelim.

```csharp
services.AddTransient<IMissionService, MissionService>();
```

Artık bir şeyleri elle tutulur şekilde gösterebilmek de gerekiyor. Bunun için veri tabanının oluşması lazım. Dolayısıyla mevzu Migration. Migration işlemleri için dotnet ef aracını kullanabiliriz ancak öğrencilerin sisteminde bu kurulu olmayabilir. Bu gibi durumlarda sorun yaşamamak ve öğrenciyi kaybetmemek adında “Eğitime Gelmeden önce Makinenizde Yapmanız Gerekenler” tadında basit bir kılavuz hazırlayıp paylaşmak iyi olabilir. Biz aşağıdaki gibi ilerleyerek devam edelim.

```bash
# Tool kurulumu için
dotnet tool install --global dotnet-ef
# tool'u güncellemek için
dotnet tool update --global dotnet-ef
# tool'u projede kullanmak için
dotnet add package Microsoft.EntityFrameworkCore.Design
# kurulduğunu görmek için
dotnet ef

# Aşağıdaki komutları Web API projesi içinde çalıştırdım.
dotnet ef migrations add Initial -o Db/Migrations
dotnet ef database update
```

Tam bu noktada SQL tarafına geçip bir veri tabanı oluştuğundan ve hatta Spaceship tablosuna örnek verilerin dolduğundan emin olmak lazım. Diğer yandan Local SQL yerine Docker’dan yararlanarak popüler bir başka veritabanını basitçe kullanabileceğimizi de belirtmemiz önemli. [Şuradaki gibi diyerek referans da gösterebiliriz](https://github.com/buraksenyurt/studious-adventure).

6 — öncü Testler

Artık testlere başlanabilir. Şükür ki Swagger gibi yapılar artık proje şablonlarına entegre edilmiş şekilde geliyorlar. Dolayısıyla örneğimizde Web API’yi doğrudan çalıştırınca aşağıdaki şık arayüzle karşılaşmamız gerekir. Dolayısıyla ilk testleri yapmak oldukça kolay olur. Eskiden buralar dutluktu.

![assets_02.png](/assets/images/2021/assets_02.png)

örnek bir JSON içeriğini aşağıdaki gibi uygulayabiliriz.

```json
{
  "name": "Ufuk ötesi Macerası",
  "plannedDuration": 18,
  "voyagers": [
    {
      "name": "Kaptan Tupolev",
      "grade": "Yüzbaşı"
    },
    {
      "name": "Melani Garbo",
      "grade": "Bilim Subayı"
    },
    {
      "name": "Dursun Durmaz",
      "grade": "Seyrüseferci"
    }
  ]
}
```

Gerekirse diye bir Curl komutu da verebiliriz — Her platformu düşünmemiz lazım.

```bash
curl -X POST "https://localhost:44306/api/Mission" -H  "accept: */*" -H  "Content-Type: application/json" -d "{\"name\":\"Ufuk ötesi Macerası\",\"plannedDuration\":18,\"voyagers\":[{\"name\":\"Kaptan Tupolev\",\"grade\":\"Yüzbaşı\"},{\"name\":\"Melani Garbo\",\"grade\":\"Bilim Subayı\"},{\"name\":\"Dursun Durmaz\",\"grade\":\"Seyrüseferci\"}]}"
```

Bu örnek JSON talebi sonrası elde edilen sonuçlar da istediğimiz gibi olmalıdır— öğrencilerin elde ettiği sonuçları da gözlemlemek gerekir.

![assets_03.png](/assets/images/2021/assets_03.png)

Doğrulama ifadelerinin işe yarayıp yaramadığını görmek içinse aşağıdaki gibi bir JSON talebi kullandırabiliriz. Mümkün mertebe her tür testi göstermemiz yararlı olabilir.

```json
{
  "name": " ",
  "plannedDuration": 10,
  "voyagers": [
    {
      "name": "The Choosen One",
      "grade": "Hacker"
    }
  ]
}
```

Buna göre şöyle bir çıktı elde etmemiz gerekir. İşler yolunda gitmekte.

![assets_04.png](/assets/images/2021/assets_04.png)

Bu andan itibaren başka ne gibi fonksiyonelliklere ihtiyacımız olabilir diye tartışmaya açmak lazım. Düşünülen yeni fonksiyonellikleri öğrencilerin uygulaması istenebilir. öncü olması açısından da “Ek Geliştirmeler” başlığı altındaki adımlar paylaştırılabilir.

7 — Ek Geliştirmeler

Temel senaryo aslında tamam ancak…

Gezginler zaman içerisinde sayıca artacaktır. Genelde bu tip senaryolarda HTTP Get ile çağırılan fonksiyonlar tüm listeyi döndürür. En azından basite kaçtığımız senaryolarda böyledir. Ancak satır sayısı fazla ise servisten her şeyi döndürmek iyi bir pratik olmayabilir. Bunun yerine kriter bazlı veri döndürmek daha iyi olur. örneğin aktif görevde olan veya olmayanların listesini çekmek. Bu bile fazla sayıda satır dönmesine sebebiyet verebilir. Ağ trafiği ve servislerin cevap verebilirlik süreleri her zaman kritiktir. Şimdi olmasa bile kullanıcı sayısı arttığında önem arz edecektir. Dolayısıyla sayfalama kriteri eklemek iyi bir çözüm olabilir. Bu sebeple Response ve Request için bazı DTO tiplerini aşağıdaki gibi tasarlayabiliriz. — Gerçek hayat senaryolarından dem vurarak bazı öğütlerde bulunmamız oldukça elzem.

Controller tipinin ilgili metoduna gelecek talep için aşağıdaki sınıfı tasarlayarak devam edelim. Kaçıncı sayfadan itibaren kaç satır alınacağını belirttiğimiz basit bir kurgu var. Ek olarak görevde olup olmama durumunu taşıdığımız boolean bir özellik bulunuyor.

```csharp
using System.ComponentModel.DataAnnotations;

namespace GalaxyExplorer.DTO
{
    public class GetVoyagersRequest
    {
        [Required]
        public int PageNumber { get; set; }
        [Required]
        [Range(5,20)] // Sayfa başına minimum 5 maksimum 20 satır kabul edelim
        public int PageSize { get; set; }
        public bool OnMission { get; set; }
    }
}
```

API metodunun dönüşünü ise aşağıdaki gibi geliştirelim. Toplam gezgin sayısı, aktif görevdeki gezgin sayısı, istenen sayfa listesi ve sonraki sayfaya geçiş için yardımcı bağlantı bilgisini döndürmeyi düşünebiliriz. Sayfalama yapılan servislerde önceki ve sonraki bölümlere geçişi kolaylaştıran referans linkleri paylaşmak standart bir pratiktir. — önceki sayfa linkinin eklenmesi, gidilecek sayfa kalmaması halinde alınması gereken önlem veya yapılması gereken işin ne olduğu öğrencilere görev olarak verilebilir.

```csharp
using System.Collections.Generic;

namespace GalaxyExplorer.DTO
{
    public class GetVoyagersResponse
    {
        public int TotalVoyagers { get; set; }
        public int TotalActiveVoyagers { get; set; }
        public List<VoyagerResponse> Voyagers { get; set; }
        public string NextPage { get; set; }
    }
}
```

Bu response tipinde kullanılan liste elemanını ise aşağıdaki gibi ekleyelim. Gezginin adı ve rütbesi dışında hakkında detaylı bilgi almak için Detail isimli bir özellik de bulunmakta. — Detay kısmında büyük ihtimalle ID kullanılması gerekecektir. Bunu söylemeden Detail kısmını nasıl oluşturmamız gerektiği öğrenciler ile karşılıklı olarak tartışılabilinir.

```csharp
namespace GalaxyExplorer.DTO
{
    public class VoyagerResponse
    {
        public string Name { get; set; }
        public string Grade { get; set; }
        public string Detail { get; set; }
    }
}
```

Sonrasında Servis arayüzüne yeni fonksiyon bildirimini eklememiz gerekir.

```csharp
Task<GetVoyagersResponse> GetVoyagers(GetVoyagersRequest request);
```

Pek tabii eklenen yeni operasyonun MissionService üzerinde uygulanması gerekir. Bunu aşağıdaki şekilde yapabiliriz.

```csharp
public async Task<GetVoyagersResponse> GetVoyagers(GetVoyagersRequest request)
{
    var currentStartRow = (request.PageNumber - 1) * request.PageSize;
    var response = new GetVoyagersResponse
    {
        // Kolaylık olsun diye sonraki sayfa için de bir link bıraktım
        // Lakin başka kayıt yoksa birinci sayfaya da döndürebiliriz
        NextPage = $"api/voyager?PageNumber={request.PageNumber + 1}&PageSize={request.PageSize}&OnMission={request.OnMission}", 
        TotalVoyagers = await _dbContext.Voyagers.CountAsync(),
        TotalActiveVoyagers = await _dbContext.Voyagers.CountAsync(v => v.OnMission == true)
    };

    var voyagers = await _dbContext.Voyagers
        .Where(v => v.OnMission == request.OnMission)
        .Skip(currentStartRow)
        .Take(request.PageSize)
        .Select(v => new VoyagerResponse
        {
            Name = v.Name,
            Grade = v.Grade,
            Detail = $"api/voyager/{v.VoyagerId}" // Bu Voyager'ın detaylarını görmek için bir sayfaya gitmek isterse diye
        })
        .ToListAsync();
    response.Voyagers = voyagers;

    return response;
}
```

Bu yeni fonksiyonu kullanabilmek için Controller tarafına da müdahale etmek gerekir. Voyager ile ilgili bir işlem söz konusu olduğundan VoyagerController isimli yeni bir Controller tipi eklemek çok daha doğrudur.

```csharp
using GalaxyExplorer.DTO;
using GalaxyExplorer.Service;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GalaxyExplorer.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoyagerController : ControllerBase
    {
        // DI Container'a kayıtlı IMissionService uyarlaması kimse o gelecek
        private readonly IMissionService _missionService;
        public VoyagerController(IMissionService missionService)
        {
            _missionService = missionService;
        }
        [HttpGet]
        public async Task<IActionResult> GetVoyagers([FromQuery] GetVoyagersRequest request) // Parametreleri QueryString üzerinden almayı tercih ettim
        {
            var voyagers = await _missionService.GetVoyagers(request);
            return Ok(voyagers);
        }
    }
}
```

Burada biraz durup tartışma başlatmak da gerekiyor. İdeal bir Controller dağılımı söz konusu gibi. Voyager ile ilgili operasyonları VoyagerController, Mission ile ilgili operasyonları MissionController üstleniyor. Açıkta bıraktığımız nokta her ikisinin IMissionService türevli bileşenleri kullanması. İdeal bir tasarımda IVoyagerService de söz konusu olmalıdır. Lakin bunun bir soru olarak gelmesini beklemeliyiz. Gelmezse “Sizce ideal bir tasarım oldu mu?” şeklinde sorup öğrencileri bu noktaya çekmeliyiz.

Uygulamayı tekrar çalıştırıp başka görevler de başlattıktan sonra Get metodunu yine Swagger arabirimi üzerinden test etmemiz gerekir.

![assets_05.png](/assets/images/2021/assets_05.png)

```bash
# curl ile test etmek isterseniz
curl -X GET "https://localhost:44306/api/Voyager?PageNumber=1&PageSize=5&OnMission=true" -H  "accept: */*
```

Aşağıdakine benzer bir çıktı alabilmeliyiz.

```json
{
  "totalVoyagers": 14,
  "totalActiveVoyagers": 11,
  "voyagers": [
    {
      "name": "Kaptan Tupolev",
      "grade": "Yüzbaşı",
      "detail": "api/voyager/1"
    },
    {
      "name": "Melani Garbo",
      "grade": "Bilim Subayı",
      "detail": "api/voyager/2"
    },
    {
      "name": "Di Ays Men",
      "grade": "İkinci Pilot",
      "detail": "api/voyager/4"
    },
    {
      "name": "Healseying",
      "grade": "Sağlık Subayı",
      "detail": "api/voyager/6"
    },
    {
      "name": "Kaptan Fasma",
      "grade": "Tugay Komutanı",
      "detail": "api/voyager/7"
    }
  ],
  "nextPage": "api/voyager?PageNumber=2&PageSize=5&OnMission=True"
}
```

Tabi sonraki sayfayı da nextPage ile gelen url bilgisini kullanarak denememiz lazım ki işe yarayıp yaramadığını görelim.

![assets_06.png](/assets/images/2021/assets_06.png)

Buraya kadar öğrenciler başarılı bir şekilde gelebiliyse harika! Eğlenceli sayılabilecek ama açık noktaları da olan bir senaryo üstünden iki temel fonksiyon kullanmış olduk. Biraz Dependency Injection, biraz Entity Framework, biraz LINQ, biraz asenkron operasyon kullanımı, biraz migration işleri, biraz Swagger farkındalığı vs… Bu kazanımları “Aklınızda neler kaldı?” diyerek öğrencilere anlattırmak gerekiyor. Sorular da alındıktan sonra onlara bazı ödevler vermek şart.

öğrenciye Neler Yaptırılabilir?

- Voyager listesinden herbir gezginin şu ana kadar katıldığı toplam görev sayısını döndürebiliriz.
- Voyager listesinden dönen Detail özelliğinin karşılığı olan Controller metodunu tamamlayabiliriz.
- Aktif görevler ve bu görevlerdeki gezginlerin listesini döndürecek bir fonksiyon ekletebiliriz.
- VoyagerController için MissionService yerine başka bir soyutlama yaptırabiliriz (IVoyagerService ve VoyagerService gibi)
- Tamamlanan görevle ilgili güncellemeri yapacak bir PUT fonksiyonu dahil ettirilebiliriz. Bu, ilgili görevin durumunu tamamlandıya çekip, göreve katılan mürettebatı yeni görev almaya uygun olarak işaretleyen bir fonksiyon olabilir. Eksik Entity alanları varsa onların fark edilmesi ve yeni bir Migration planı hazırlanıp çalıştırılmasını isteyebiliriz.
- ve öğrencilerin aklına gelen diğer ekler.

Görüldüğü üzere çok sık yazılan, anlatılan, öğretilen bir konu için hazırlık yapmak önemli bir efor ve çaba gerektiriyor. üstelik varılan sonuçların tutarlı olması ve ortak stadartlar üzerinde durması da önemli.

Faydalı olması ve ilham vermesi dileğiyle…
