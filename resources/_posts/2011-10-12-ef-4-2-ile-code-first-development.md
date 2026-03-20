---
layout: post
title: "EF 4.2 ile Code–First Development"
date: 2011-10-12 20:45:00 +0300
categories:
  - entity-framework
tags:
  - entity-framework
  - csharp
  - xml
  - dotnet
  - ado-net
  - sql-server
  - http
  - generics
  - rc
---
Teknolojinin hızına inanın ki yetişilemiyor. Her gün yeni bir bilim ve teknoloji haberi var dünyada. Ancak bir geçek daha var ki o da Microsoft, Google, Apple gibi devlerin de hızına yetişilememsi. Örneğin daha henüz 2011 Mix’ te RTM olarak duyurulan Entity Framework 4.1 sürümü üzerine geçtiğimiz günlerde 4.2 RC (Release Candidate) duyuruldu. Ben de bunun üzerine yeni sürümde Code-First Development’ ın nasıl uygulandığını anlamak ve görmek istedim. Haydi gelin keşfetmeye başlayalım.

[![speed](/assets/images/2011/speed_thumb.jpg)](/assets/images/2011/speed.jpg)


İlk olarak Entity Framework 4.2 sürümü ile ilişkili detaylı bilgilere [Ado.Net Team Blog’](http://blogs.msdn.com/b/adonet/) un ilgili adresi üzerinden ulaşabileceğinizi belirtmek isterim. Peki herşey güzel de nedir bu Code-First Development? Ado.Net Entity Framework tarafında en başından beri var olduğunu bildiğimiz Model-First ve Database-First Development yaklaşımları mevcut. Ancak Code-First development ne ola ki

![Gülümseme](/assets/images/2011/wlEmoticon-smile_17.png)

Doğruyu söylemek gerekirse bu tanımı en sonda yapmanın ve aradan geçen zaman dilimi içerisinde basit bir örnek üzerinden adım adım ilerlemenin daha doğru olacağı kanısındayım.

Bu amaçla ilk olarak basit bir Console Application projesi açtığımızı ve içerisine aşağıdaki sınıf diagramında görülen tipleri eklediğimizi var sayalım.

[![bei_19](/assets/images/2011/bei_19_thumb.gif)](/assets/images/2011/bei_19.gif)

Department.cs

```csharp
using System.Collections.Generic;

namespace CodeFirstDevelopment 
{ 
    public class Department 
    { 
        public int DepartmentId { get; set; } 
        public string Name { get; set; } 
        public virtual ICollection<Employee> Employees { get; set; } 
    } 
}
```

Employee.sc

```csharp
namespace CodeFirstDevelopment 
{ 
    public class Employee 
    { 
        public int EmployeeId { get; set; } 
        public string Name { get; set; } 
        public string AccountNumber { get; set; } 
        public virtual Department Department { get; set; } 
    } 
}
```

Aslında bir departman ve bu departmana bağlı işçileri temsil etmeye çalıştığımız basit iki POCO (Plain Old CLR Object) tipi yazdığımızı görmekteyiz. Bir başka deyişle çok basit tipleri kullanarak aralarında ilişkiler tanımladığımız basit bir Model’ i kurgulamaktayız. Peki bu iki tipe ait nesne koleksiyonlarını nasıl kullanacağız? Normal şartlarda Ado.Net Entity Framework içerisinde, basit tipleri sarmallayan ve onlara ait koleksiyonları birer özellik gibi sunan, ve ayrıca yükleme, ekleme, silme gibi temel operasyonları üstlenen bir tipin daha olduğunu biliyoruz.

Code-First Development senaryosunda bu tipin yazılması da geliştiriciye ait. Ancak bu tip sanıldığı kadar karmaşık değil. Tek dikkat edilmesi gereken nokta var olan EF kabiliyetlerini sunabilmesi için.Net’ in aşina olduğu bir tipten türemesi gerekliliği. İşte bu noktada NuGet ile uygulamamıza install edebileceğimiz bir kütüphane meydana çıkıyor. EntityFramework.Preview

![Göz kırpan gülümseme](/assets/images/2011/wlEmoticon-winkingsmile_68.png)

[![bei_18](/assets/images/2011/bei_18_thumb.gif)](/assets/images/2011/bei_18.gif)

Teşekkürler NuGet

![Gülümseme](/assets/images/2011/wlEmoticon-smile_17.png)

Güncel proje üzerinden kısa sürede gerçekleştirilen install işlemi sonrasında bazı ek library dosyalarının referans edildiğini görürüz. Aşağıdaki şekilde bu kütüphaneler işaretlenmiştir.

[![bei_20](/assets/images/2011/bei_20_thumb.gif)](/assets/images/2011/bei_20.gif)

Şimdi projemize yeni bir sınıf daha ekliyor olacağız. Aslında Context diye tabir edilen bu sınıfın görevi temel olarak Model içerisindeki POCO tiplerine ait koleksiyon bazlı nesneleri sunmak olacaktır. Tabi EF kabiliyetlerinin kendisine kazandırılması gerektiğinden indirilen paket içerisinde yer alan bir tipten de türetilmesi (Inheritance) söz konusudur. İşte Context tipimiz;

[![bei_21](/assets/images/2011/bei_21_thumb.gif)](/assets/images/2011/bei_21.gif)

ve kod içeriğimiz;

```csharp
using System.Data.Entity;

namespace CodeFirstDevelopment 
{ 
    public class CompanyContext 
        :DbContext 
    { 
        public DbSet<Department> Departments { get; set; } 
        public DbSet<Employee> Employees { get; set; } 
    } 
}
```

Dikkat edileceği üzere CompanyContext tipi Department ve Employee tiplerine ait nesnel koleksiyonları sunmak için DbSet tipinden olan özellikler sunmaktadır. Buna ek olarak DbContext tipinin de IObjectContextAdapter ve IDisposable arayüzlerini (Interface) uyguladığını görmekteyiz. Bu interface yardımıyla aslında Object Context’ e erişilmektedir. Diğer yandan ilgili nesnesinin Dispose edilebilir olması için IDisposable arayüzünün de uygulandığı görülmektedir. Eğer DbContext tipinin üyelerine bakılacak olursa SaveChanges gibi bir metod olduğunu fark edebiliriz ki bunun EF tarafında ne kadar önemli bir metod olduğunu tahmin edebilirsiniz

![Göz kırpan gülümseme](/assets/images/2011/wlEmoticon-winkingsmile_68.png)

Aslında DbContext, Entity verileri ile birer nesne gibi çalışılabilmesini ve sorgulama işlemlerinin yapılabilmesini sağlamaktadır. Bu yüzden Entity nesneleri üzerinde yapılan veri değişimlerinin bir kaynağa doğru gönderilebilmesi için SaveChanges gibi metod sunmaktadır.

Artık Console uygulamamızda söz konusu tipleri kullanarak bir test gerçekleştirebiliriz. Bu amaçla aşağıdaki örnek kod parçasını göz önüne alalım.

```csharp
using System.Collections.Generic;

namespace CodeFirstDevelopment 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            using (CompanyContext context = new CompanyContext()) 
            { 
                Department IT = new Department { DepartmentId = 1, Name = "IT" };

                List<Employee> employees = new List<Employee> 
                { 
                    new Employee{ EmployeeId=1000, Name="Burak Senyurt", Department=IT, AccountNumber="IT10235"}, 
                    new Employee{ EmployeeId=1001, Name="Steve Jobs", Department=IT, AccountNumber="RIP1234"}, 
                    new Employee{ EmployeeId=1250, Name="Bill Gates", Department=IT, AccountNumber="BIL1222"} 
                }; 
                IT.Employees = employees;

                context.Departments.Add(IT);

                context.SaveChanges(); 
            } 
            
        } 
    } 
}
```

Dikkat edileceği üzere Context nesnesi örneklendikten sonra önce bir Department üretilmiş ardından bu departmana bağlı bir kaç örnek Employee eklenmiştir. Yani bir departman ve bu departmana bağlı veri içerikleri bellek üzerinde gerçekleştirilmiştir. Son olarakta Context nesne örneği üzerinden SaveChanges metodu çağırılmıştır

![Şaşırmış gülümseme](/assets/images/2011/wlEmoticon-surprisedsmile.png)

SaveChanges? Hımmm…Şimdi burada bir saniye durup düşünelim. SaveChanges metodu bellek üzerinde gerçekleşen ekleme, silme, güncelleme vb işlemler sonucu ilgili varlık içeriklerini nereye kayıt edecektir. Sanki bir yerlerde bir şeyleri söylememiz gerekiyordu. Aslında söylemesekte olur

![Göz kırpan gülümseme](/assets/images/2011/wlEmoticon-winkingsmile_68.png)

Nitekim DbContext nedeniyle ilgili şema yapıları için standart bir SQL bağlantısı söz konusudur. SQLExpress sürümü kullanılaraktan bir veritabanı otomatik olarak oluşturulacak ve söz konusu tablolar üretilerek ilgili veri içerikleri buraya yüklenecektir. Ancak biz bu davranışı değiştirmek istediğimizi düşünelim. Söz gelimi SQL Server üzerinde bir veritabanına doğru yazılsın. Bu amaçla tek yapmamız gereken uygulamanın App.Config dosyasına aşağıdaki Connection String bilgisini eklemektir.

```xml
<?xml version="1.0" encoding="utf-8" ?> 
<configuration> 
  <connectionStrings> 
    <add name="CompanyContext" connectionString="data source=.;database=Company;integrated security=SSPI" providerName="System.Data.SqlClient"/> 
  </connectionStrings> 
</configuration>
```

Buraya dikkat! Biz şu ana kadar Company isimli bir veritabanı (database) oluşturmadık

![Göz kırpan gülümseme](/assets/images/2011/wlEmoticon-winkingsmile_68.png)

Dolayısıyla ilk çalışma esnasında söz konusu veritabanı ve içeriği üretilse çok iyi olur değil mi?

![Gülümseme](/assets/images/2011/wlEmoticon-smile_17.png)

Haydi gelin uygulamamızı debug modda çalıştıralım. Eğer çalışma zamanında Context nesne örneği üzerinde biraz dolaşırsak aşağıdaki gibi bir Connection String bilgisinin uygulanmaya çalışıldığını görürüz.

[![bei_22](/assets/images/2011/bei_22_thumb.gif)](/assets/images/2011/bei_22.gif)

Burada,

Data Source=.\SQLEXPRESS;Initial Catalog=CodeFirstDevelopment.CompanyContext;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFrameworkMUE

şeklinde bir bağlantı bilgisi üretilmiştir. SaveChanges metodunu atladığımızda ise ilk yapmamız gereken aslında yerel sunucuya bakmak olmalıdır. Ve sonuç;

[![bei_23](/assets/images/2011/bei_23_thumb.gif)](/assets/images/2011/bei_23.gif)

Volaaa!!! Sizce de süper değil mi?

![Gülümseme](/assets/images/2011/wlEmoticon-smile_17.png)

Çok basit tipler tanımladık ve code modelimize bakılarak ilgili tablolar ve aralarındaki ilişkiler otomatik olarak oluşturuldu. Hatta verilerin yüklendiğini de SQL sorgularımızı atarak görebiliriz.

[![bei_24](/assets/images/2011/bei_24_thumb.gif)](/assets/images/2011/bei_24.gif)

Bu durumda geldiğimiz aşamaya baktığımızda, basit POCO nesnelerini ve DbContext tipini kullanaraktan veritabanı modelimizi, önce kodu düşünerek geliştirmiş olduğumuzu ifade edebiliriz. Elbette yapılabilecek daha pek çok işlem var. Code-First Development ile ilişkili diğer özellikleri ben de zaman içerisinde incelemeyi düşünüyorum. Şimdilik bu giriş niteliğindeki Hello World yazısı ile idare etmeye çalışacağım. Eğer Entity Framework konusunda geçerli ve güncel bir kaynaktan yararlanmak isterseniz Julie Lerman’ ın [Programming Entity Framework: Building Data Centric Apps with the ADO.NET Entity Framework](http://www.amazon.com/Programming-Entity-Framework-Building-Centric/dp/0596807260/ref=sr_1_1?s=books&ie=UTF8&qid=1318495364&sr=1-1) adlı kitabını tavsiye edebilirim. Her ne kadar 2010 Ağustos baskısı olsada kalan kısımları Offical Ado.Net Team Blog ile eksik kısımları ve yenilikleri tamamlayabilirsiniz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[CodeFirstDevelopment.rar (1,01 mb)](/assets/files/2011/CodeFirstDevelopment.rar)