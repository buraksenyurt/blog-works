---
layout: post
title: "Entity Framework - Generic Repository ve Unit of Work Uyarlaması"
date: 2014-12-14 03:00:00 +0300
description: "DDD denilince aklımıza daha çok veri odaklı uygulamalar gelir ve bu tip ürünlerde RDBMS(Relational Database Management System) lerin yeri hatırı sayılır ölçüde fazladır(Her ne kadar son yıllarda NoSQL cephesinde önemli gelişmeler ve kullanımda ciddi artışlar olsa da…) Hal böyle olunca O/RM(Object Relational Mapping) araçlarının kullanımı da önem kazanmaktadır. Yıllardır hayatımızda olan bu araçlar modellerin nesnel olarak inşasında da önemli bir yere sahiptirler. Lakin Object Oriented dünyasının kuralları içerisinde yaşarlar ve bu yüzden bazı kurumsal prensipleri uygulamaları gerekmektedir."
categories:
  - entity-framework
  - tasarim-prensipleri-design-principles
tags:
  - entity-framework
  - tasarim-prensipleri-design-principles
  - csharp
  - dotnet
  - linq
  - nosql
  - http
  - concurrency
  - transactions
  - generics
  - testing
  - dependency-injection
  - dependency-management
---
# Entity Framework - Generic Repository ve Unit of Work Uyarlaması
[![thinking](/assets/images/2014/thinking_thumb.jpg)](/assets/images/2014/thinking.jpg) Merhaba Arkadaşlar,

Yazılım dünyasında var olan mimari prensipler veya tasarım kalıpları tek başlarına belirli sorunları çözseler de, bazı kurumsal projelerde mutlak suretle bir arada düşünülmeleri gerekir. Söz gelimi Repository ve Unit of Work kalıpları, özellikle Domain Driven Design odaklı yapılarda bir arada değerlendirilmesi gerekenlerdendir.

DDD denilince aklımıza daha çok veri odaklı uygulamalar gelir ve bu tip ürünlerde RDBMS (Relational Database Management System) lerin yeri hatırı sayılır ölçüde fazladır (Her ne kadar son yıllarda NoSQL cephesinde önemli gelişmeler ve kullanımda ciddi artışlar olsa da…)

Hal böyle olunca O/RM (Object Relational Mapping) araçlarının kullanımı da önem kazanmaktadır. Yıllardır hayatımızda olan bu araçlar modellerin nesnel olarak inşasında da önemli bir yere sahiptirler. Lakin Object Oriented dünyasının kuralları içerisinde yaşarlar ve bu yüzden bazı kurumsal prensipleri uygulamaları gerekmektedir.

Benim gibi.Net üzerinde geliştirme yapanlar için O/RM araçları da az çok bellidir. Entity Framework bunlardan birisidir. Ancak Entity Framework’ ün uygulamalardaki kullanımında genellikle hatalar yapılır. Enterprise bir çözüm söz konusu olduğunda varsayılan olarak Data Access ve Business Logic katmanlarının olması izolasyon açısından önemlidir. İşte bu noktada DAL ve BLL arasındaki kullanımlarda EF’in çoğu zaman bir O/RM aracı olarak soyutlanamadığı görülür. Hal böyle olunca sistemin farklı bir kaynağı kullanarak yaşamaya devam etmesi de zorlaşır. [Repository](http://martinfowler.com/eaaCatalog/repository.html) ve [Unit of Work](http://martinfowler.com/eaaCatalog/unitOfWork.html) özellikle bu vakalara çözüm niteliğindeki iki değerli desendir.

> Hiç kimse bu deseneleri Martin Fowler kadar iyi açıklayamaz. Bu yüzden makalenin amacı ilgili desenelerin Entity Framework için örnek bir kullanımının anlatımından ibarettir.

İşin Gerçeği

Gerçek hayatta Entity Framework veya başka bir O/RM aracının kullanıldığı hallerde aşağıdaki grafikteki iki durumdan birisi söz konusu olur (Genellikle de en soldaki). Klasik olarak DbContext doğrudan iş katmanında değerlendirilir. Ancak Test Driven Development veya Domain Driven Design gibi yaklaşımların kullanıldığı geliştirme süreçlerinde, Repository ve Unit of Work desenelerinin icra edilmesi önemlidir. Nitekim bu sayede uygulamanın iş mantığının tutulduğu katman ile veri erişim katmanının izole edilmesi kolaylaşır. t anında farklı bir Repository ile çalışılabilmesi veya yenilerinin yazılarak sisteme dahil edilmesinin yolu açılır. Aynı kolaylık Unit of Work yapıları için de geçerlidir.

[![ruof_1](/assets/images/2014/ruof_1_thumb.png)](/assets/images/2014/ruof_1.png)

İlk senaryoya göre iş mantığı, veri erişimi ve EF arasında kuvvetli bağlar oluşur. Bu sebepten, üründe kullanılan veri tabanını değiştirmek (farklı bir Repository’ yi tercih etmek) ve özellikle Unit Test gibi yapılarda Mock nesneleri değerledirmek zorlaşır. Bir Unit Test metodu içerisindeki işlemler bütününde her zaman CRUD (CreateReadUpdateDelete) operasyonları icra edilmek istenmeyebilir. Nitekim iş bütününün Repository odaklı olmayan kısımlarının test edilmesi de söz konusudur.

Unit Test’ lerin çalıştığı geliştirme ortamının hiç bir şekilde bir veri kaynağına gidemediği hallerde geri kalan kısmın test edilme ihtiyacı bu tip bir gereksinimdir. Ayrıca aynı veri kaynağı ile çalışılacak diye bir kural yoktur. Domain içerisindeki Entity modelleri sabit kalabilir ve iş kuralları çok az değişiklik gösterebilir. Ama verilerin yazıldığı ortamlar duruma göre farklılık gösterebilir, açılıp kapatılmak istenebilir. Bu sebeple soyutlama (abstraction) yapmak ve uygun sözleşme tanımlamalarını (Interface bildirimleri diyebiliriz) işin içerisine katmak önemlidir.

Gelin konuyu basit ve pek de işe yaramayacak örnek bir senaryo üzerinden ele alalım. Amacımız içinde iki Entity barındıran bir DbContext türevini, Repository ve Unit of Work desenleri çerçevesinde nasıl ele alabileceğimizi incelemektir.

Code First ile Entity Modelin İnşası

Örnek uygulama her zaman ki gibi gösterişsiz bir Console projesidir. Amaç ilgili desenlerin sade bir uyarlamasını görebilmektir. Ama öncesinde NuGet üzerinden güncel Entity Framework’ ün son sürümü projeye indirilerek işe başlanabilir.

[![rpuow_1](/assets/images/2014/rpuow_1_thumb.png)](/assets/images/2014/rpuow_1.png)

Ardından kobay olarak aşağıdaki Entity sınıfları ve DbContext türevini yazabiliriz.

[![ruof_2](/assets/images/2014/ruof_2_thumb.png)](/assets/images/2014/ruof_2.png)

```csharp
using System.Collections.Generic; 
using System.Data.Entity;

namespace RPandUOW.EntityModel 
{ 
    public class ShopContext 
        : DbContext 
    { 
        public DbSet<Category> Categories { get; set; } 
        public DbSet<Product> Products { get; set; } 
    } 
    public class Category 
    { 
        public int CategoryID { get; set; } 
        public string Title { get; set; } 
        public virtual ICollection<Product> Products { get; set; } 
    } 
    public class Product 
    { 
        public int ProductID { get; set; } 
        public string Title { get; set; } 
        public decimal UnitPrice { get; set; } 
        public int Quantity { get; set; } 
        public int CategoryID { get; set; } 
        public virtual Category Category { get; set; } 
    } 
}
```

Tipik olarak one-to-many ilişki içerisinde sayabileceğimiz iki POCO tipi bulunmaktadır. Bir kategori ve bu kategoriye bağlı olan ürünler. Gelelim Repository deseninin uygulanış biçimine.

Repository Yapısının İnşası

Öyle bir yapı kurgulamalıyız ki, hem bir Repository için gerekli minimum fonksiyonelliklerin bir sözleşmesi hem de Context içerisinde yer alan her T tipi için çalışabilecek generic bir sınıf olsun. Ve pek tabi varsayılan kuralları istediği gibi işleyecek yeni Repository’ leri yazmanın da yolu açılabilsin. İlk olarak aşağıdaki sınıf diagramında görülen tiplerin tasarlanmasıyla işe başlanabilir.

[![rpuow_2](/assets/images/2014/rpuow_2_thumb.png)](/assets/images/2014/rpuow_2.png)

ve kodlar;

```csharp
using RPandUOW.EntityModel; 
using System; 
using System.Collections.Generic; 
using System.Data.Entity; 
using System.Linq; 
using System.Linq.Expressions;

namespace RPandUOW.Repositories 
{ 
    public interface IGenericRepository<T> 
        where T:class 
    { 
        T FindById(object EntityId); 
        IEnumerable<T> Select(Expression<Func<T, bool>> Filter = null); 
        void Insert(T Entity); 
        void Update(T Entity); 
        void Delete(object EntityId); 
        void Delete(T Entity); 
    }

    public class ShopRepository<T> 
        :IGenericRepository<T> 
        where T:class 
    { 
        private ShopContext _context; 
        private DbSet<T> _dbSet; 
        public ShopRepository(ShopContext Context) 
        { 
            _context = Context; 
           _dbSet = _context.Set<T>(); 
        } 
        public virtual T FindById(object EntityId) 
        { 
            return _dbSet.Find(EntityId); 
        } 
        public virtual IEnumerable<T> Select(Expression<Func<T, bool>> Filter = null) 
        { 
            if (Filter != null) 
            { 
                return _dbSet.Where(Filter); 
            } 
            return _dbSet; 
        } 
        public virtual void Insert(T entity) 
        { 
            _dbSet.Add(entity); 
        } 
        public virtual void Update(T entityToUpdate) 
        { 
            _dbSet.Attach(entityToUpdate); 
            _context.Entry(entityToUpdate).State = EntityState.Modified; 
        } 
        public virtual void Delete(object EntityId) 
        { 
            T entityToDelete = _dbSet.Find(EntityId); 
           Delete(entityToDelete); 
        } 
        public virtual void Delete(T Entity) 
        { 
            if (_context.Entry(Entity).State == EntityState.Detached) //Concurrency için 
            { 
                _dbSet.Attach(Entity); 
            } 
            _dbSet.Remove(Entity); 
        } 
    } 
}
```

Burada neler yaptık, ortalığı nasıl karıştırdık incelemeye çalışalım. IRepository arayüzü içerisinde bir Repository için söz konusu olabilecek temel fonksiyonların tanımlandığını görmekteyiz. CRUD (CreateReadUpdateDelete) operasyonları olarak adlandırabileceğimiz metodlar ile bir Repository’ nin minimumda sahip olması gereken sözleşmeyi de tanımlamış oluyoruz.

ShopRepository sınıfı dikkat edileceği üzere IRepository arayüzünü uygulamakta ve kendi içerisinde DbContext sınıfından türetilmiş bir ShopContext örneğini kullanmaktadır. Yani ShopRepository generic sınıfı, ShopContext içinde tanımlı herhangi bir T tipini kullanarak CRUD operasyonlarını gerçekleştirebilir. Bunun bir diğer anlamıda, farklı kaynakları kullanan veya Mock nesne olabilen Repository tiplerinin istenildiği zaman sisteme dahil edilebilmesidir. Tek yapılması gereken ilgili IRepository sözleşmesinin yeni Repository için uygulanmasından başka bir şey değildir.

Repository’ nin kullandığı Context nesnesinin oluşturulması aslında yapıcı metod içerisinde icra edilmektedir. Burada da generic bir kullanım yolu düşünülebilir. Dikkat çekici noktalardan bir tanese bir Context için söz konusu olan Save işleminin bu tiplerde her angi bir biçimde ele alınmamış olmasıdır. Aslında bu, Unit of Work yapısının inşasında ele alınması gereken bir fonksiyonelliktir. Öyleyse Unit of Work yapısını tesis etmeye başlayabiliriz.

Unit of Work Yapısının İnşası

Entity Framework açısından bir birimlik işi; içerisinde konuya dahil olması gereken Repository örneklerinin oluşturulması ve Save işleminin icra edilmesi olarak düşünebiliriz (Hatta bu yapı içerisine Transaction açılıp kapatılması da dahil edilebilir) Pek tabi Unit of Work yapısınında bir sözleşme üzerinden değerlendirilmesi, farklı Unit of Work’ lerin de değerlendirilebilmesi açısından önemlidir. Bu düşünceler ışığında aşağıdaki yapıyı kurgulayabiliriz.

[![rpuow_3](/assets/images/2014/rpuow_3_thumb.png)](/assets/images/2014/rpuow_3.png)

ve kodlar;

```csharp
using RPandUOW.EntityModel; 
using RPandUOW.Repositories; 
using System; 
using System.Transactions;

namespace RPandUOW.UnitOfWorks 
{ 
    public interface IUnitOfWork 
        :IDisposable 
    {   
        void Save(); 
        // Başka operasyonlar da tanımlanabilir. 
        // void OpenTransaction(); 
        // void CloseTransaction(); 
        // gibi 
    }

    public class ShopUnitOfWork 
        :IUnitOfWork 
    { 
        private ShopContext _context = new ShopContext(); 
        private ShopRepository<Category> _categoryRepository; 
        private ShopRepository<Product> _productRepository; 
        private bool _disposed = false; 
        public ShopRepository<Category> CategoryRepository 
        { 
            get 
            { 
                if (_categoryRepository == null) 
                   _categoryRepository = new ShopRepository<Category>(_context); 
                return _categoryRepository; 
            } 
        } 
        public ShopRepository<Product> ProductRepository 
        { 
            get 
            { 
                if (_productRepository == null) 
                    _productRepository = new ShopRepository<Product>(_context); 
                return _productRepository; 
            } 
        } 
        public void Save() 
        { 
            using (TransactionScope tScope = new TransactionScope()) 
           { 
                _context.SaveChanges(); 
                tScope.Complete(); 
            } 
        } 
        protected virtual void Dispose(bool disposing) 
        { 
            if (!this._disposed) 
            { 
                if (disposing) 
                { 
                    _context.Dispose(); 
                } 
            } 
            this._disposed = true; 
        } 
        public void Dispose() 
        { 
            Dispose(true); 
            GC.SuppressFinalize(this); 
        } 
    } 
}
```

Pek tabi soyutlama amacıyla IUnitOfWork isimli bir arayüz tanımlanarak işe başlanmıştır. Arayüz şu an için Save metodunun uygulanması gerektiğini belirtir. Tabi bir de IDisposable arayüzü nedeniyle Dispose metodunun ezilmesi zorunlıdır. Bir birimlik iş için ihtiyaca göre başka genel fonksiyonellikler de sözleşme içerisine dahil edilebilir. Örneğin bir Transaction açılması ve kapatılması için gerekli metodlar sözleşme ile zorunlu tutulabilir. Tabi bunu çok da spesifik düşünmemek gerekir. Nitekim kimi Repository’ lerin, kullandığı veri kaynakları bir Transaction ile çalışmak zorunda olmayabilir. Hatta ortada bir veri kaynağı da bulunmayabilir (Burada Mock nesnelere atıfta bulunmaktayım)

ShopContext için kullanılacak Unif of Work kurgusunda ise, işe dahil olacak Repository’ ler birer Property olarak tanımlanmış ve sadece okunabilir şekilde son kullanıcıya sunulmuşlardır. Üretim işlemleri sırasında yapılan null kontrolü, Unit of Work nesnesinin yaşamı boyunca, tüm Repository’ lerin aynı Context tipini (ki örnekte context isimli ShopContext örneğidir) kullanması açısında önemlidir. (Bu durumu daha iyi anlamak için debug modda çalışmanızı öneririm)

Bir başka deyişle örneğin Save işlemi sırasında tüm Repository nesnelerinin aynı DbContext örneği üzerinden işlemlerini gerçekleştirmesi ve tek bir Transaction bütünlüğü içerisinde çalışması sağlanmış olmaktadır. Zaten Unit of Work desenin temel amaçlarından birisi de bu işlem bütünlüğünü kurgulamaktr.

Basit Bir Kullanım

Yazılan Unit of Work uyarlamasının uygulanış biçimi oldukça kolaydır. Normal şartlarda bir BLL fonksiyonelliği içerisinde de değerlendirilebilir. Konunun basitçe ele alınması açısından Main metodu aşağıdaki kodları içerecek şekilde geliştirilmiştir.

```csharp
using RPandUOW.EntityModel; 
using RPandUOW.UnitOfWorks; 
using System; 
using System.Collections.Generic;

namespace RPandUOW 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            using (ShopUnitOfWork worker = new ShopUnitOfWork()) 
            { 
                Category computerBook = new Category { Title = "Computer Books" }; 
                worker.CategoryRepository.Insert(computerBook); 
                computerBook.Products = new List<Product> { 
                    new Product { Title = "Advanced NoSQL", Quantity = 1, UnitPrice = 34.59M }, 
                    new Product { Title = "NHibernate in Action", Quantity = 5, UnitPrice = 29.99M }, 
                    new Product { Title = "Unleashed Delphi 2.0", Quantity = 3, UnitPrice = 9.99M } 
                }; 
                Category cookBook = new Category { Title = "Cook Books" }; 
                worker.CategoryRepository.Insert(cookBook); 
                cookBook.Products = new List<Product> { 
                new Product() 
                    { 
                        Title = "Italian Kitchen", Quantity = 20, UnitPrice = 12 } 
                    }; 
                worker.CategoryRepository.Insert(cookBook); 
                worker.Save(); 
                var books = worker.ProductRepository.Select(p => p.CategoryID == computerBook.CategoryID); 
                foreach (var book in books) 
                { 
                    Console.WriteLine("{0} {1} {2}", book.Title, book.UnitPrice, book.Quantity); 
                } 
            } 
        } 
    } 
}
```

[![rpuow_4](/assets/images/2014/rpuow_4_thumb.png)](/assets/images/2014/rpuow_4.png)

IDisposable arayüzü implementasyonu nedeniyle ShopUnitOfWork sınıfı using bloğun içerisinde kullanılabilir. Zaten dipose işlemi sınıfın içerisinde override edilmiştir. Blok içerisinde bir dizi örnek işlem icra edilmektedir. Buna göre bir kaç kategorinin ve bu kategorilere bağlı ürünlerin eklenmesi işlemi ele alınmaktadır. Save işlemi, Unit of Work uyarlamasının bir fonksiyonu olduğundan, dahil edilen tüm Repository örnekleri için ortak bir kullanım noktasıdır. Öyle ki, örnekte asıl Context nesnesi üzerinden yapılan kaydetme işleminin bir TransactionScope içerisinde gerçekleştirilmesi sağlanmaktadır.

Görüldüğü üzere Repository ve Unit of Work desenelerini Entity Framework tarafında uygulamak oldukça kolaydır. Kaynaklarda bu desenlerin daha etkili uygulanış biçimlerini de görebilirsiniz. Örneğin [Codeplex’ in şu adresindeki](http://genericunitofworkandrepositories.codeplex.com/) uygulanış tarzı beni etkileyenler arasındadır. Hatta Unit of Work uyarlamasının daha generic ve Context’ lere gevşek bağlı (Loosely Coupled) olan bir versiyonu da yazılabilir (İşin içine Dependency Injection da katılıp olay daha bir renkli hale getirilebilir) Bunlara biraz kafa yormakta fayda vardır.

Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.