---
layout: post
title: "EF Core ile MariaDb Kullanımı"
date: 2018-02-19 17:18:00 +0300
description: "Bu yazımızda MySQL’in yaratıcısı olan Monty Widenius‘un MySQL’in kodunu çatallayıp oluşturduğu ve kızının adını verdiği MariaDB'nin Entity Framework Core ile nasıl kullanılabileceğini incelemeye çalışıyoruz."
categories:
  - dotnet-core
tags:
  - dotnet-core
  - bash
  - csharp
  - dotnet
  - aspnet
  - asp-dotnet-core
  - entity-framework
  - ef-core
  - linq
  - sql-server
  - oracle
  - mysql
  - web-api
  - iis
  - go
  - vue
  - generics
  - visual-studio
  - github
---
# EF Core ile MariaDb Kullanımı
![mariacore_5.gif](/assets/images/2018/mariacore_5.gif)

Merhaba Arkadaşlar,

Son bir kaç aydır Cumartesi gecelerimi bir şeyler yazmak veya araştırmak için değerlendirmekteyim. Bu tip çalışma disiplinlerini daha önceden de denemiş ve epeyce faydasını görmüştüm. Sonuçta üzerinde çalıştığımız yazılım platformları ve ürünler sürekli ve düzenli olarak değişim içerisindeler. Dolayısıyla yeniliklerin ucundan da olsa tutabilmek lazım. Bir anlamda şu meşhur Pomodoro çalışma tekniğini haftalık periyotlara böldüğümü ifade edebilirim.

Geçtiğimiz hafta içerisinde.Net Core tarafında nelere bakabilirim diye internette sörf yaparken eski dostumuz Entity Framework'e rastladım. Tabii oyun sahası benim için artık değişmişti. West-World, Microsoft Sql Server nedir pek bilmiyordu. Hatta IIS'e bile burun kıvırıp Nginx ya da Apache ile konuşuyordu. Elimde pek çok makalede kullanılan Visual Studio'da yoktu. Buna rağmen yetenekleri daha kısıtlı olan Visual Studio Code ile hayat oldukça güzeldi. Sonunda bir süredir merak ettiğim MariaDb'yi, Entity Framework Core ile basitçe nasıl konuşturabilleceğimi görmeye karar verdim. Öncelikle MariaDb'yi tanımlayalım...

[MariaDB](https://mariadb.org/) GNU lisansı ile sunulan, MySQL’in yaratıcısı Monty Widenius‘un kodlarını çatallayarak (fork işlemi) aynı komutları, arayüzleri ve API’leri destekleyecek şekilde geliştirmeye başlanmış bir ilişkisel veritabanı yönetim sistemi (Relational Database Management System) Her ne kadar sevimsiz bir tanımlama gibi dursa da logosu en az MySQL'in sevimli yunusu kadar dikkat çekici. Nihayetinde neden böyle bir oluşma gidildiğine dair internette pek çok tartışma var. Onları araştırmanızı öneririm. Benim dikkatimi çeken işin içerisine Oracle girdikten sonra inanılmaz derecede artan destek ücretleri (Sun'ın 2010 da satın alınması ve MySQL'in Oracle'e geçmesi sonrası destek ücretinin %600 arttığı söyleniyor) Bu düşünceler ışığında geçtim evdeki Ubuntu'nun başına.

MariaDb kurulumu

Tabii ilk olarak West-World'e MariaDb'yi kurmam lazım. Son zamanlarda bu dünya üzerinde çok fazla şey yüklenip kaldırıldı ama bana mısın demedi. Sanırım o da bu yeni deneyimlerden keyif alıyor. Kendisine sırasıyla aşağıdaki komutları gönderek MariaDb'yi yüklemesini ve gerekli servisleri çalıştırmasını söyledim.

```bash
sudo apt-get update
sudo apt-get install mariadb-server mariadb-client
sudo mysql_secure_installation
```

Son komut sonrası bazı sorularla karşılaştım. İlk etapta root user şifresinin değiştirilmesinin yerinde olacağını belirteyim. Kurulum sorasında root kullanıcının şifresi boştur. Dolayısıyla Enter tuşuna basarak geçilebilir. Ardından bir şifre vermek gerekecektir. Şifre adımından sonra isimsiz kullanıcılara (anonymous user) izin verilip verilmeyeceği, root kullanıcı için uzaktan erişim sağlanıp sağlanmayacağı, test veritabanının kaldırılıp kaldırılmayacağı ve benzeri sorulara verilecek cevaplarla kurulum işlemi sonlandırılır. Kurulum sonrası aşağıdaki komutu kullanarak MariaDb ortamına giriş yapabiliyor olmamız gerekir.

sudo mysql -u root -p

![mariacore_1.gif](/assets/images/2018/mariacore_1.gif)

Yukarıdaki ekran görüntüsünde show databases; komutu kullanılarak yüklü olan veritabanlarının listesinin elde edilmesi sağlanmışıtır. Burada basit SQL sorgu ifadeleri kullanarak bir takım işlemler yapabiliriz. Bir deneyin derim.

Bu arada eğer sudo kullanmadan giriş yapmak istersek yada ERROR 1698 (28000): Access denied for user 'burakselyum'@'localhost'benzeri bir hata alırsak, root user'ın mysql_native_password plug-in'inini kullanabilmesini söyleyerek çözüm üretebiliriz.

```bash
use mysql;
update user set plugin='mysql_native_password' where user='root';
flush privileges; 
exit;
```

sonrasında

```bash
service mysql restart 
```

ile mysql hizmetini yeniden başlatmak gerekir.

Console Uygulamasının Yazılması

Gelelim MariaDb ile konuşacak uygulama kodlarının yazımına. Konuya basit bir girizgah yapmak istediğim için her zaman ki gibi tercihim Console uygulaması geliştirmek. Ancak siz örneği denerken bir Web API servisi arkasınada da kullanmayı tercih edebilirsiniz.

```bash
dotnet new console -o MariaDbSample
```

Şimdi gerekli paketleri de eklemek lazım. MariaDb'yi Entity Framework Core ile kolayca kullanabilmek için [Pomelo](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql) tarafından geliştirilmiş bir paket bulunmakta. Bu ve diğer EF paketlerini aşağıdaki komutları kullanarak projeye entegre edebiliriz.

```bash
dotnet add package Microsoft.EntityFrameworkCore.Tools -v:'2.0.0'
dotnet add package Microsoft.EntityFrameworkCore.Tools.DotNet -v:'2.0.0'
dotnet add package Pomelo.EntityFrameworkCore.MySql -v:'2.0.0.1'
```

Paketler yüklendikten sonra bir restore işlemi uygulamakta yarar var.

```bash
dotnet restore 
```

Program.cs içeriği ise aşağıdaki gibi. Kalabalık göründüğünü bakmayın. Büyük bir kısmı kitap ve kategori ekleme kodlarından oluşuyor.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MariaDbSample
{
    [Table("Book")]
    public class Book
    {
        public long BookID { get; set; }
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }
        public bool InStock { get; set; }
        public double Price { get; set; }
        public virtual BookCategory Category { get; set; }
    }

    [Table("Category")]
    public class BookCategory
    {
        [Key]
        public long CategoryID { get; set; }
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }
        public virtual ICollection<Book> Books { get; set; }
    }

    public partial class BeautyBooksContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<BookCategory> BookCategories { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(@"uid=root;pwd=rootşifre;Host=localhost;Database=BeautyBooks;");
        }
    }

    public class Program
    {
        public static void Main()
        {
            using (var context = new BeautyBooksContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var scienceCategory = new BookCategory()
                {
                    Name = "Science"
                };
                var programmingCategory = new BookCategory()
                {
                    Name = "Programming"
                };
                var mathCategory = new BookCategory()
                {
                    Name = "Math"
                };
                context.BookCategories.Add(scienceCategory);
                context.BookCategories.Add(programmingCategory);

                context.Books.Add(new Book()
                {
                    Title = "2025 : Go to the MARS",
                    Price = 8.99,
                    InStock = true,
                    Category = scienceCategory
                });
                context.Books.Add(new Book()
                {
                    Title = "The 9nth Element",
                    Price = 6.99,
                    InStock = false,
                    Category = scienceCategory
                });
                context.Books.Add(new Book()
                {
                    Title = "Calculus - I",
                    Price = 19.99,
                    InStock = false,
                    Category = mathCategory
                });
                context.Books.Add(new Book()
                {
                    Title = "Advanced Asp.Net Core 2.0",
                    Price = 38.10,
                    InStock = true,
                    Category = programmingCategory
                });
                context.Books.Add(new Book()
                {
                    Title = "C# 7.0 Introduction",
                    Price = 15.33,
                    InStock = false,
                    Category = programmingCategory
                });
                context.Books.Add(new Book()
                {
                    Title = "Vue.js for Dummies",
                    Price = 28.49,
                    InStock = false,
                    Category = programmingCategory
                });
                context.Books.Add(new Book()
                {
                    Title = "GoLang - The New Era",
                    Price = 55.55,
                    InStock = false,
                    Category = programmingCategory
                });

                context.SaveChanges();

                Console.WriteLine("Book List\n");

                var query = context.Books.Include(p => p.Category)
                    .Where(p => p.Price < 30.0)
                    .ToList();

                Console.WriteLine("{0,-8} | {1,-50} | {2,-8} | {3}\n\n", "BookID", "Title", "Price", "Category");
                foreach (var book in query)
                    Console.WriteLine("{0,-8} | {1,-50} | {2,-8} | {3}", book.BookID, book.Title, book.Price, book.Category.Name);

                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }
        }
    }
}
```

Aslında Entity Framework ile kod geliştirenlerin gayet aşina olduğu işlemler söz konusu. Senaryoda kitaplar ve kategorilerini tuttuğumuz tipler söz konusu. Book ve BookCategory tipleri için örnek olması açısından Table attribute'ları ile MariaDb tarafındaki tablo adları belirtiliyor. Normal şartlarda Primary Key olan alanın [TipAdı]ID olarak ifade edilmesi yeterli. Ancak BookCategory sınıfı için anahtar alanı işaret etmek üzere Key niteliğinden yararlanmaktayız. Bazı alanlar için maksimum alan uzunluğu ve gereklilik gibi kriterleri de yine nitelikler yardımıyla belirliyoruz (Required, MaxLength nitelikleri) İki sınıf arasında birer ilişki de söz konusu. Tek yönlü kurduğumuz ilişkide bir kategori altında birden fazla kitap olabileceğini ifade ediyoruz. Bu ilişki sağlanırken ICollection tipinden de yararlanıyoruz.

DbContext türevli BeautyBooksContext sınıfı içerisinde sunduğumuz DbSet'ler var (Books ve BookCategories isimli özellikler) OnConfiguring metodu eziliyor ve içerisinde UseMySql ile provider'ın değiştirilmesi sağlanıyor. Parametre, MariaDb için kullanılacak Connection String bilgisini içermekte.

Main fonksiyonu içerisinde peş peşe iki Ensure operasyonunun çağırıldığı görülebilir. İlk olarak eğer BeautyBooks veritabanı varsa siliniyor ki bu şart değil. Sadece örnekte deneme olarak kullandım. EnsureCreated çağrısı ile de veritabanı ve ilgili nesnelerin (tabloların) olmaması halinde yaratılmaları sağlanıyor. Örnek ilk çalıştırıldığında aslında bu veritabanı ve tablolar sistemde yok. Bu yüzden önce oluşturulacaklar.

İlerleyen kod satırlarında Context'e bir kaç veri girişi yapılıyor. SaveChanges ile de yapılan eklemelerin veritabanına yazılması sağlanıyor. Ekleme işlemleri sonrası bir LINQ sorgusu var. Fiyatı 30 birim altında olan ürünleri kategorileri ile birlikte çekip ve ekrana düzgün bir formatta basıyor.

Artık uygulama çalışabilir. Console penceresine yansıyan çıktı aşağıdaki gibi olacaktır.

![mariacore_2.gif](/assets/images/2018/mariacore_2.gif)

Diğer yandan MariaDb üzerindeki duruma da kendi arabiriminden bakılabilir. Ben aşağıdaki komutları denedim.

```text
show databases;
use BeautyBooks;
show tables;
select * from Book;
select * from Category;
```

İlk komut ile veritabanlarını gösteriyoruz. Kodun çalışması sonrası oluşan BeautyBooks üzerinde işlemler yapmak istediğimiz için use komutunu kullanıyoruz. Veritabanı seçiminin ardından show tables ile tabloları gösteriyoruz ki Book ve Category nesnelerinin oluşturulduğunu görebiliriz. İki standart Select sorgusu ile de tablo içeriklerini göstermekteyiz (Bu arada MariaDb'nin terminal çıktıları çok hoşuma gitti. Bana üniversite yıllarımda DOS ekranında Pascal ile tablo çizip içini verilerle doldurmaya çalıştığım günleri anımsattı. Çok basit bir görünüm ama sade ve anlaşılır)

![mariacore_3.gif](/assets/images/2018/mariacore_3.gif)

Log Eklemeyi Unutunca Ben

Aslında arka planda MariaDb üzerinde ne gibi SQL ifadelerinin çalıştırıldığını görebilsem hiç de fena olmazdı. Ben tabii bunu unuttum ve makaleyi tekrardan düzenlediğim bir ara fark ettim. Hemen Microsoft dokümanlarını kurcalayarak en azından Console'a log'ları nasıl atabilirimi araştırdım. Sonrasında BeautyBookContext içeriğini aşağıdaki hale getirdim.

```csharp
public partial class BeautyBooksContext : DbContext
    {
        public static readonly LoggerFactory EFLoggerFactory
            = new LoggerFactory(new[] {new ConsoleLoggerProvider((cat, level) =>
            cat== DbLoggerCategory.Database.Command.Name && level==LogLevel.Information,true)});

        public DbSet<Book> Books { get; set; }
        public DbSet<BookCategory> BookCategories { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(EFLoggerFactory);
            optionsBuilder.UseMySql(@"uid=root;pwd=rootşifre;Host=localhost;Database=BeautyBooks;");
        }
    }
```

Aslında bu kod parçasında çok güzel bir ders var. Bir Log mekanizmasını EF çalışma motoruna enjekte ediyoruz. EFLoggerFactory, LoggerFactory türünden üretilirken ilk parametrede ConsoleLoggerProvider örneği verilmekte. Farklı LoggerProvider'lar da var ve hatta biz de kendi LoggerProvider tipimizi buraya ekleyebiliriz. Yapıcı metodda verilen parametrelerle bir filtreleme yaparak hangi bilgilerin hangi seviyede kayıt altına alınacağını belirtiyoruz. İlgili LoggerFactory'nin kullanılabilmesi içinse UseLoggerFactory operasyonunu devreye alıyoruz. Sonuçta kodu çalıştırdığımızda arka planda hareket eden SQL ifadelerinin Console penceresine basıldığını görebiliriz.

![mariacore_4.gif](/assets/images/2018/mariacore_4.gif)

Görüldüğü üzere Entity Framework ile MySQL türevli MariaDb'yi kullanmak oldukça basit. Elbette önemli olan konulardan birisi EF çalışma zamanına MariaDb provider'ının enjekte edilmesi. Yani DbContext türevli sınıfın ezilen OnConfiguring fonksiyonu içerisinde yapılan UseMySql çağrısı. Bu sebepten github'taki kod içeriğini incelemekte de oldukça büyük yarar olduğu kanısındayım. Nitekim kendi veritabanı sistemimizin Entity Framework Core tarafında kullanılmasını istediğimiz durumlarda benzer kod çalışmasını yapmamız gerekecektir. Böylece bir cumartesi gecesini daha eğlenceli şekilde bitiriyorum. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.