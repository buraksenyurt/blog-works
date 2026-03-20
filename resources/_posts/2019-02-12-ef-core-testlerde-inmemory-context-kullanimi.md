---
layout: post
title: "EF Core : Testlerde InMemory Context Kullanımı"
date: 2019-02-12 09:00:00 +0300
categories:
  - dotnet-core
tags:
  - dotnet-core
  - bash
  - csharp
  - dotnet
  - entity-framework
  - ef-core
  - linq
  - sql-server
  - transactions
  - generics
  - testing
  - github
---
Ablamla rahmetli babamız bir önceki sabah olduğu gibi o günde terastaki ahşap yemek masasının üzerine kurdukları fileyi karşılıklı sabitlemekle meşgullerdi. Normal ebatlarına göre çok daha dar ve kısa olan yemek masası, benim gibi orta okul çağlarındaki birisi için ideal bir ping pong sahasıydı esasında. Son bir kaç yazdır en büyük eğlencelerimizden birisi haline gelmişti. Kuzenlerle dolup taşan kalabalık yaz akşamlarında bir çok aile ferdini çevresine sığdıran Alman ahşapından yapılma o sağlam masa, prüzsüz yüzeyiyle sabahları çekişmeli ping pong maçlarına ev sahipliği yapıyordu. Güzel anıları ile birlikte rahmetli babamı zaman zaman kızdıran vakitlere de tanıklık etmişti. Bir keresinde raketi tutan kolumu tavana doğru öyle bir açmıştım ki florasan lambayı tuzla buz etmiştim. O günden sonra tavandan sarkan değil zeminine sabit bir lamba tercih etmiştik. Lakin bir diğer sefer daha büyük bir sorun yaşamıştık.

![ttplayer_n.jpg](/assets/images/2019/ttplayer_n.jpg)

Evin zemin katındaki terasta hemen bahçe giriş kapısının önünde topraktan elli santimetre kadar yüksekte olan taş zemin üzerinde duran yemek masası, sokağa bakan tarafı boydan boya cam olan mutfağın da yanı başındaydı. Her ne kadar masa ile mutfak camı arasında bir metrelik mesafe olsa da büyüyen ben yıllar içerisinde aradaki kol mesafesini de azaltmıştım. Ve bir gün lise çağına geldiğimde olan olmuştu. Kolumu sağa doğru koşarken öyle geniş ve sert açmıştım ki, kırmızı yüzeyi ile göz göze geldiğim raket elimden fırlayıvermişti. Bahçe yerine mutfak camına doğru. Koca cam ortadan büyük bir yarıkla kırıldı. Eh tabii o zamanlar bugünkü gibi minicik parçalara ayrılıp kimseye zarar vermeyen camlara sahip değildik. Annemin "Aman oğlum iyi ki size bir şey olmadı" deyişinin yanında rahmetlinin o en meşhur bakışı saplanmıştı gözlerimden içeriye. Telepatik olarak mesaj alınmıştı. Sonraki gün ve yaz tatillerinin ilerleyen yıllarında, panayır yerindeki ping pong masasını kiralamanın çok daha ucuz olacağını anlamıştık.

Ortaokul çağlarında başlayan masa tenisi sevdam üniversite yıllarında da devam etti. Pek tabii bir alanda çok iyi olmak için gerçekten de çok çalışmak gerekiyor. İyi masa tenisi oynamak, müsabakalara katılıp derece yapabilmek her gün saatlerce masa tenisi oynamayı gerektiriyor. Ben hep amatör altı seviyede kalsam da dönem dönem derece almış ya da bu oyunu çok sevmiş arkadaşlara da sahip oldum. Yazlıktaki Sinan, üniversitedeki Emre, ellibeş yaşında üst kattaki teraslarına hakiki masa tenisi kurup benden ders alan hevesli Erdal Amca ve diğerleri. Gel zaman git zaman kırklı yaşlarıma geldim. Derken son girdiğim iş yerinde yemekhaneye çıktığım o ilk gün...Uzaktaki bir dinlenme alanında masa tenisi oynayan insanlar...Ve tekrar oynamaya başladım. Ah bu arada masa tenisi demişken, dünyanın en iyi oyuncularının listesini [uluslararası masa tenisi federasyonunun şu sayfasında](https://www.ittf.com/rankings/) bulabilirsiniz. Ben onlardan birisinin ismini bir Entity nesnesini örnekleyip InMemory çalışan veritabanına yazmak için kullanacağım.

Entity Framework ile çalışırken test süreçlerini zorlaştırabilecek bağımlılıklardan birisi de uzak veritabanı bağlantısıdır. Genellikle bir SQL sunucusu ile çalışıldığından connectionString bilgisinde belirtilen adrese birim testlerin çalıştırılması sırasında da gidiliyor olması beklenir. Ancak bu şu anki durumda şart değil. EF context'ini bellekte çalışacak şekilde o anki process içerisinde de kullanabiliriz. Bunun için [şu adreste yayınlanan Nuget paketinden](https://www.buraksenyurt.com/admin/app/editor/Entity Framework ile çalışırken test süreçlerini zorlaştırabilecek bağımlılıklardan birisi de uzak veritabanı bağlantısıdır. Genellikle bir SQL sunucusu ile çalışıldığından connectionString bilgisinde belirtilen adrese test ortamında da gidilebiliyor olması beklenir. Ancak bu şu anki durumda şart değil. EF context'ini bellekte çalışacak şekilde o anki process içerisinde de kullanabiliriz. Bunun için...) yararlanıyoruz. Ancak bellekte çalışan bu veritabanı modelini ilişkisel olan versiyonları ile karıştırmamak lazım. Nitekim InMemory veritabanı bir SQL Server veritabanını taklit edemiyor (O amaçla geliştirilmemiş) Bu sebepten genel amaçlı veritabanı operasyonları için kullanılması daha doğru diyebiliriz. MSDN dokümanlarına göre ilişkisel veritabanı modelinin yerine kullanılacak test amaçlı bir araç gerekiyorsa, SQLite'ın InMemory çalışan verisyonunu göz önüne alabiliriz. Şimdilik amacımız basit veritabanı operasyonları sunan bir servise ait birim testlerde hakiki SQL sunucusuna gitmeden fonksiyonellikleri deneyimleyebilmek.

Gelin adım adım ilerleyerek söz konusu testleri nasıl yazabileceğimize bir bakalım. Öncelikle üzerinde çalışacağımız Solution'ı hazırlayalım. Bunun için terminalden aşağıdaki komutlarla ilerleyebilir ve bir proje ağacı oluşturabiliriz.

```bash
mkdir Testing
cd Testing
dotnet new sln
mkdir CustomerService
cd CustomerService
dotnet new classlib
cd ..
dotnet sln add CustomerService/CustomerService.csproj
mkdir CustomerService.Tests
cd CustomerService.Tests
dotnet new mstest
dotnet add reference ../CustomerService/CustomerService.csproj
cd ..
dotnet sln add CustomerService.Tests/CustomerService.Tests.csproj
cd CustomerService.Tests
```

Testing isimli solution'ımız içerisinde iki tip proje yer alıyor. CustomerService isimli sınıf kütüphanesinde (class library) Entity Framework tabanlı çalışan içeriklere yer vereceğiz. Test fonksiyonlarını ise CustomerService.Tests isimli mstest şablonundaki projede yazacağız. Kabaca aşağıdaki şekilde görülen ağacı oluşturmamız başlangıç için yeterli.

![efinm_1.gif](/assets/images/2019/efinm_1.gif)

Pek tabii ihtiyacımız olan paketleri de kurmamız lazım. EntityFrameworkCore, SqlServer ve InMemory paketlerini CustomerService projesine eklemek için aşağıdaki terminal komutları ile çalışmamıza devam edelim.

```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.InMemory
```

Artık DbContext türevli CustomerContext sınıfını ve diğerlerini kodlayabiliriz. Örneği basit bir şekilde almak için Customer isimli tek bir Entity sınıfı kullanacağız.

```csharp
namespace CustomerService
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Title { get; set; }
    }
}
```

Sadece isim, soyisim ve ünvana yer verdiğimiz Customer tipinden sonra CustomerContext sınıfını yazarak devam edelim.

```csharp
using Microsoft.EntityFrameworkCore;

namespace CustomerService
{
    public class CustomerContext
    : DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        public CustomerContext()
        { }

        public CustomerContext(DbContextOptions<CustomerContext> options)
            : base(options)
        { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=PDOSVIST01;Database=ATPMasters.InMemory;Trusted_Connection=True;ConnectRetryCount=0");
            }
        }
    }
}
```

CustomerContext, Customer tipinden bir DbSet ile çalışıyor. DbContext türevli olan bu sınıfın içerisinde iki yapıcı metoda (Constructor) yer veriyoruz. Varsayılan yapıcı değil ama DbContextOptions türünden parametre alan ikinci verisyon önemli. Nitekim bu parametreye vereceğimiz bilgilerle test projesinde CustomerContext nesnesini oluştururken InMemory veritabanı kullanılacağını belirteceğiz. Override edilen OnConfiguring metodunda kullandığımız hakiki bir SQL Server bağlantı bilgisi olduğu dikkatinizden kaçmamış olsa gerek. Yani testler sırasında InMemory ilerlenirken, Context'in orjinal kullanımında aksi belirtilmediği sürece ilişkisel veritabanı ile konuşulacağını garanti etmiş oluyoruz.

Temel işlemleri içeren AddingService sınıfını da aşağıdaki gibi geliştirebiliriz.

```csharp
using System.Collections.Generic;
using System.Linq;

namespace CustomerService
{
    public class AddingService
    {
        private CustomerContext _context;

        public AddingService(CustomerContext context)
        {
            _context = context;
        }
        public Customer CreateCustomer(Customer customer)
        {
            var newCustomer=_context.Customers.Add(customer);
            _context.SaveChanges();
            return newCustomer.Entity;
        }
        public void UpdateCustomer(Customer customer)
        {
            var cust = _context.Customers.FirstOrDefault(c => c.CustomerID == customer.CustomerID);
            if (cust != null)
            {
                cust.Firstname = customer.Firstname;
                cust.Lastname = customer.Lastname;
                cust.Title = customer.Title;
                _context.SaveChanges();
            }
        }
        public IEnumerable<Customer> FindByLastname(string lastName)
        {
            return _context.Customers
                .Where(c => c.Lastname.Contains(lastName))
                .ToList();
        }

        public Customer FindById(int customerID)
        {
            return _context.Customers.FirstOrDefault(c=>c.CustomerID==customerID);
        }
    }
}
```

CreateCustomer ile yeni bir müşteri oluşturma, UpdateCustomer ile bilgilerini güncelleme, FindById ile belli bir CustomerID'ye göre kişi bulma ve FindByLastName ile de soyadına göre listeleme operasyonlarını üstlenen fonksiyonlarımız var. Tipik LINQ işlemlerine yer verdiğimizi düşünebiliriz. Tüm metodlarda CustomerContext örneğini kullanıyoruz. Bu nesneyi servis sınıfımıza yine yapıcı metod üzerinden geçirmekteyiz. Dolayısıyla hangi veri sağlayıcısını kullanacaksak buradaki fonksiyonlar ona göre işlem yapacaklar.

Servis tarafındaki ihtiyaçlarımızı tamamladığımıza göre artık test metodlarını geliştirmeye başlayabiliriz. Bunun için Unit Test projesine geçelim ve AddingTests sınıfını aşağıdaki gibi geliştirelim.

```csharp
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CustomerService.Tests
{
    [TestClass]
    public class AddingTests
    {
        [TestMethod]
        public void Create_Single_Customer_In_Memory()
        {
            var options = new DbContextOptionsBuilder<CustomerContext>()
                .UseInMemoryDatabase(databaseName: "TT100")
                .Options;

            using (var context = new CustomerContext(options))
            {
                var service = new AddingService(context);
                var nadal = new Customer
                {
                    Firstname = "Dimitrij",
                    Lastname = "OVTCHAROV",
                    Title = "Mr"
                };
                service.CreateCustomer(nadal);
            }

            using (var context = new CustomerContext(options))
            {
                Assert.AreEqual(1, context.Customers.Count());
                var added = context.Customers.Single();
                Assert.AreEqual("Dimitrij", added.Firstname);
                Assert.AreEqual("OVTCHAROV", added.Lastname);
                Assert.AreEqual("Mr", added.Title);
            }
        }

        [TestMethod]
        public void Find_Customers_By_Lastname()
        {
            var options = new DbContextOptionsBuilder<CustomerContext>()
                .UseInMemoryDatabase(databaseName: "TT50")
                .Options;

            using (var context = new CustomerContext(options))
            {
                context.Customers.Add(new Customer { Firstname = "Kim Hing", Lastname = "Yong", Title = "Mr" });
                context.Customers.Add(new Customer { Firstname = "Burak Selim", Lastname = "Yong", Title = "Mr" });
                context.Customers.Add(new Customer { Firstname = "Su Han", Lastname = "Yong", Title = "Ms" });
                context.Customers.Add(new Customer { Firstname = "Kim Hing", Lastname = "Yang", Title = "Mr" });
                context.Customers.Add(new Customer { Firstname = "Koki", Lastname = "Niwa", Title = "Ms" });
                context.Customers.Add(new Customer { Firstname = "Fun Sun", Lastname = "Kim", Title = "Ms" });
                context.SaveChanges();
            }

            using (var context = new CustomerContext(options))
            {
                var service = new AddingService(context);
                var result = service.FindByLastname("Yong");
                Assert.AreEqual(3, result.Count());
            }
        }

        [TestMethod]
        public void Update_Single_Customer()
        {
            var options = new DbContextOptionsBuilder<CustomerContext>()
                .UseInMemoryDatabase(databaseName: "TT50")
                .Options;
            var id = 0;
            using (var context = new CustomerContext(options))
            {
                var service = new AddingService(context);

                var kimHing = service.CreateCustomer(new Customer { Firstname = "Kim Hing", Lastname = "Yong", Title = "Mr" });
                context.SaveChanges();
                id = kimHing.CustomerID;

                service.UpdateCustomer(new Customer
                {
                    CustomerID = id,
                    Firstname = "Kim Kim",
                    Lastname = "Yong",
                    Title = "Mr"
                });
            }

            using (var context = new CustomerContext(options))
            {
                var service = new AddingService(context);
                var founded = service.FindById(id);
                Assert.AreEqual("Kim Kim", founded.Firstname);
            }
        }
    }
}
```

Üç test metodumuz var. Tek bir müşterinin oluşturulması, n sayıda müşteriden aynı soyada sahip olanlarının çekilmesi ve belli bir müşterinin verisinin değiştirilmesi işlerini deneyimliyoruz. Buna uygun olacak bir kaç Assert kullanımımız var. Tüm test metodlarının yazımız açısından en önemli ortak noktası ise DbContextOptionsBuilder nesnesi örneklenirken UseInMemoryDatabase fonksiyonunun kullanılmış olması. Bu sayede sonraki satırlarda oluşturulan CustomerContext örneklerinin hangi tür veritabanı ile çalışacağını belirtmiş oluyoruz.

Test metodlarına ait çalışma zamanı sonuçlarını görmek için

```bash
dotnet test
```

terminal komutunu vermemiz yeterli olacaktır. Ben denemelerimde aşağıdaki ekran görüntüsünde yer alan sonuçlara ulaştım. Tüm testler başarılı bir şekilde ilerletildi. Dolayısıyla operasyonun InMemory veritabanı kullanılarak icra edildiğini söyleyebiliriz.

![efimd_2.gif](/assets/images/2019/efimd_2.gif)

InMemory veritabanı kullanımı görüldüğü gibi oldukça basit ancak başlarda da belirttiğimiz üzere her veritabanı özelliği desteklenmiyor. Örneğin transaction desteği yok ve bu veritabanı üzerinden SQL sorgularını çalıştıramıyoruz. Bu tip bir durumda SQLite veritabanının bellekte çalışacak şekilde kullanılması öneriliyor. Amaç yine SQL Server'a ihtiyaç duymadan genel Entity Framework işlevlerini test edebilmek. Ufak bir kaç kod değişikliği ile testlerimizi SQLite'ın InMemory modda çalışan versiyonuna çekebiliriz. İlk etapta SQLite paketinin projeye dahil edilmesi gerekiyor.

```bash
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

Örnek olması açısından bir test metodunda aşağıdaki değişiklikleri yaparak ilerleyebiliriz.

```csharp
[TestMethod]
public void Create_Single_Customer_In_Memory()
{
    SqliteConnection connection = new SqliteConnection("DataSource=:memory:");
    connection.Open();
    var options = new DbContextOptionsBuilder<CustomerContext>()
        //.UseInMemoryDatabase(databaseName: "TT100")
        .UseSqlite(connection)
        .Options;

    using (var context = new CustomerContext(options))
    {
        context.Database.EnsureCreated();
        var service = new AddingService(context);
	// Diğer kod satırları aynen devam ediyor
```

SqliteConnection tipinden bir nesne oluşturuyor ve parametre olarak verdiğimiz değerle bellekte çalışacağını belirtmiş oluyoruz. UseSqlite fonksiyonuna yapılan çağrıya bu connection bilgisini verdiğimiz için CustomerContext değişkeni artık Sqlite tipinden bir veritabanını kullanacak (Hemde bellekte çalışan sürümünü) Bir ihtimal ilgili veritabanının oluşmaması ihtimaline karşılık context üzerinden EnsureCreated metodunu çağırmamız da gerekebilir. Testleri bu şekilde çalıştırdığımızda bir öncekiler ile aynı sonuçları elde edeceğimizi görebilirsiniz.

Kuvvetle muhtemel ilerleyen dönemlerde özellikle kolay test yapabilmek için farklı opsiyonlarda karşımıza çıkabilir. Şu an için genel amaçlı kullanılan ve belli başlı CRUD operasyonlarını içeren Entity Framework tabanlı servislere ait test senaryolarında değerlendirebileceğimiz iki önemli seçenek var. InMemory veya ilişkisel modele biraz daha yakın durabilen SQLite'ın InMemory versiyonu. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Örnek kodlara github üzerinden erişebilirsiniz](https://github.com/buraksenyurt/dotnetcore/tree/master/EF/Testing)
