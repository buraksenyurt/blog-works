---
layout: post
title: "Entity Framework 6 Alpha 1 ve async, await Özellikleri"
date: 2012-10-31 22:41:00 +0300
categories:
  - entity-framework
tags:
  - entity-framework
  - csharp
  - dotnet
  - http
  - async-await
  - parallel-programming
  - threading
  - visual-studio
---
Doğruyu söylemek gerekirse yazılım hayatım boyunca en çok kurduğum cümlelerden birisi de sanıyorum ki şu olmuştur: “Microsoft’ un hızına yetişemiyoruz” Bazı açılardan bakıldığında bu özellikle nihayi ürün ile geliştirme yapanlar için bir handikap olarak görülebilir. Çünkü yeni sürümler genellikle geliştiricilerin ve ürün yöneticilerinin arzu ettikleri, görmek istedikleri kabiliyetleri içermektedir.

[![speed_limit](/assets/images/2012/speed_limit_thumb.jpg)](/assets/images/2012/speed_limit.jpg)

Diğer taraftan kişisel görüşüme göre, Microsoft yazılım ekiplerinin bu çalışkanlığı da takdir edilmesi gereken bir durumdur. Bu ekiplerin başında da Entity Framework geliştirilmesinden [sorumlu ekip](http://blogs.msdn.com/b/adonet/) gelmektedir.

Sözü fazla uzatmıyayım ama daha bu ayki Entity Framework 5.0 tabanlı [Nedirtv?com](HTTP://www.nedirtv.com) Webinerime hazırlanırken bir kaç gün önce EF 6.0’ nın alpha sürümünün yayınlandığını ve NuGet paket yönetim aracı ile indirilebileceğini öğrendim.

> Paketi arayıp bulabilmek için, Manage Nuget Packages dialog penceresindeki Include Prerelease seçeneğini işaretlemeyi unutmayın. Aksi durumda Release sürümleri öncesindeki ürünler listelenmeyeceklerdir.
> [![ef6async_1](/assets/images/2012/ef6async_1_thumb.png)](/assets/images/2012/ef6async_1.png)

Entity Framework geliştirilmesinden sorumlu takım bildiğiniz gibi [codeplex](http://entityframework.codeplex.com/) üzerinden kaynak kodları da açmış durumdadır. Dolayısıyla alpha sürümüne ait kodları açık kaynak olarak inceleyebilir ve hatta Microsoft’ un beklediği gibi, ürünle ilişkili geri bildirimlerinizi (Feedbacks) ekibe iletebilirsiniz. Bizi dinliyorlar ve gerçekten bazı gerekli özellikleri yeni sürümlere dahil ediyor veya en azından yol haritasına (Roadmap) alıyorlar. (Bu arada [Ef tarafındaki Roadmap ile ilişkili olarak bu adresi takip edebilirsiniz](http://entityframework.codeplex.com/wikipage?title=Roadmap))

6.0 versyionu ile birlikte Entity Framework’ ün son sürümüne dahil edilmesi planlanan bir çok özellik de bulunmakta. Bunlardan birisi de artık.Net Framework’ ün olmassa olmaz parçası haline gelen paralel programlama desteği ve pek tabi Task tabanlı kabiliyetleri dil seviyesinde kolaylaştıran async, await anahtar kelimelerinin kullanılabilmesi. Bu destek kendisini diğer alt yapılarda da göstermekte. Entity Framework tarafında da kayıt ve sorgulama operasyonları için asenkron çalışma desteği getirilmiş durumda (alpha sürümü için). İşte bu yazımızda async, await kullanımını incelemeye çalışıyor olacağız.

Başlangıç

İlk etapta Visual Studio 2012 üzerinde basit bir Console uygulaması oluşturup gerekli Entity Framework kütüphanesini Nuget yardımıyla dahil ederek işe başlayabiliriz. Konuyu basit ve kolay bir şekilde anlayabilmek adına Code-First yaklaşımını tercih ediyor olacağız. Başlangıçta ki tip hiyerarşisini aşağıki gibi oluşturabiliriz.

[![ef6async_2](/assets/images/2012/ef6async_2_thumb.png)](/assets/images/2012/ef6async_2.png)

```csharp
class Category 
{ 
    public int CategoryId { get; set; } 
    public string Name { get; set; } 
}

class Shop 
    : DbContext 
{ 
    public DbSet<Category> Categories { get; set; } 
}
```

Klasik olarak kobay tiplerimizden kategori sınıfını kullandığımız bir yapı söz konusudur

![Smile](/assets/images/2012/wlEmoticon-smile_66.png)

Kategoriler ve isterseniz bunlara bağlı ürünleri de dahil edebileceğini veri modelini Shop isimli DbContext türevi içerisinden sunmaktayız.

> Eğer kendi belirleyeceğiniz SQL bağlantısı üzerinde Shop context tipi için gerekli veritabanını üretmeyi planlıyorsanız, app.config dosyasında aşağıdakine benzer bir bildirimde bulunmanız gerekecektir.

## Klasik Veri Ekleme, Sorgulama

Şimdi bilinen yöntemi ile örnek kategoriler ilave edip, sonrasında sorgulamak istediğimizi düşünelim. Aşağıdakine benzer bir kod içeriği pekala işimizi görecektir.

```csharp
class Program 
{ 
    static void Main(string[] args) 
    { 
        InsertCategory("Kitap"); 
        InsertCategory("Kalem"); 
        InsertCategory("Defter"); 
        InsertCategory("Oyuncak");

        WriteCategories(); 
    }

    static void InsertCategory(string categoryName) 
    { 
        using (Shop context = new Shop()) 
        { 
            Category newCategory = new Category { Name = categoryName }; 
            context.Categories.Add(newCategory);               
            context.SaveChanges(); 
        } 
    } 
    static void WriteCategories() 
    { 
        using (Shop context = new Shop()) 
        { 
            var categories = from c in context.Categories 
                             orderby c.Name 
                             select c; 
            foreach (var category in categories) 
            { 
                Console.WriteLine("{0} {1}",category.CategoryId,category.Name); 
            } 
        } 
    } 
}
```

Örneği basit seviyede ele almak adına sadece kategori tipinden örneklerin eklenmesi ve sorgulanması simüle edilmiştir.

## async ve await ile Beslemek

Yeni eklenen Entity nesne örneklerini veritabanına gönderirken ve sorgularken kullanabileceğimiz asenkron metodlar SaveChangesAsync ve ForEachAsync isimli fonksiyonlardır. (Bir başka deyişle Async son eki ile biten operasyonları kullanmamız gerekmektedir) Bu operasyonlar await edilebilir niteliktedirler. Şimdi dilerseniz örneğimizde yer alan insert ve select operasyonların asenkron modda çalışabilir hale getirelim. Bunun için aşağıdaki kod parçasını değerlendirebiliriz.

```csharp
class Program 
{ 
    static void Main(string[] args) 
    { 
        InsertAndSelectAsync().Wait(); 
    }

    static async Task InsertAndSelectAsync() 
    { 
        await InsertCategory("Kitap"); 
        await InsertCategory("Kalem"); 
        await InsertCategory("Defter"); 
        await InsertCategory("Oyuncak");

        await WriteCategories(); 
    }

    static async Task InsertCategory(string categoryName) 
    { 
        using (Shop context = new Shop()) 
        { 
            Category newCategory = new Category 
            { 
                Name = categoryName 
            }; 
            context.Categories.Add(newCategory);               
            await context.SaveChangesAsync(); 
        } 
    }

    static async Task WriteCategories() 
    { 
        using (Shop context = new Shop()) 
        { 
            await context 
                .Categories 
                .ForEachAsync(c=> 
            { 
                Console.WriteLine("{0} {1}",c.CategoryId,c.Name); 
            }); 
        } 
    } 
}
```

Dikkat edileceği üzere InsertCategory metodu async anahtar kelimesi ile imzalanmış ve içerisinde yapılan SaveChangesAsync fonksiyon çağırımında await kullanılmıştır. Benzer durum WriteCategories metodu için de söz konusudur. Bu metodda sorgu işlemini asenkron modda gerçekleştirmek için, ForEachAsync metoduna yapılan çağrıda da await kullanılmıştır. WriteCategories fonksiyonu da InsertCategory gibi async çalışacak şekilde işaretlenmiştir.

InsertCategory ve WriteCategories fonksiyonları async desenine uygun şekilde geliştirildiklerinden await edilebilirler. Bu sebepten tüm işlemleri kapsülleyen InsertAndSelectAsync metodu içerisinde await kullanımlarına yer verilerek ilerlenilmiştir. (Bu metodu yazmak mecburi değildir) Main metodu içerisinde asenkron çalışan metodların işlmeleri tamamlanıncaya kadar uygulamanın beklemesini sağlamak içinse Wait fonksiyonundan yararlanıldığında dikkat edilmelidir.

Pek tabi örnekteki operasyonlar çok basit ve aslında hızlı olduklarından asenkron bir modelin kullanılmasının performansa önemli bir katkısı bulunmamaktadır. Hatta Thread seviyesinde yapılan hazırlıklar nedeni ile negatif bir etki de söz konusu olabilir.

Yine de bazı senaryolarda ve özellikle sunucu tarafında çok fazla sayıda CRUD (Create Retrieve Update Delete) işleminin gönderilebileceği vakalarda bu tip bir yaklaşım ele alınabilir. İstenirse söz konusu asenkron işleyiş için Task tiplerinden aşağıdaki kod parçasında olduğu gibi yararlanılabilir de. Bir başka deyişle InsertAndSelectAsync fonksiyonu bir zorunluluk değildir.

```csharp
static void Main(string[] args) 
{ 
    Task[] tasks = new Task[5]; 
    tasks[0] = InsertCategory("Kitap"); 
    tasks[1] = InsertCategory("Kalem"); 
    tasks[2] = InsertCategory("Defter"); 
    tasks[3] = InsertCategory("Oyuncak"); 
    tasks[4] = WriteCategories(); 
    Task.WaitAll(tasks); 
}
```

async olarak işaretlenmiş InsertCategory ve WriteCategories metodları Task dönüş tipine sahip olduklarından, Task türevli bir dizinin elemanı olarak kullanılabilirler. Buna göre senaryomuzda yer alan asenkron fonksiyon çağrılarına ait bir Task dizisinin, WaitAll tekniğine göre ele alınması ve uygulama sonlanmadan önce bu işlemleri içeren tüm Task örneklerinin işlerinin bitirilmesinin beklenmesi sağlanabilir.

Görüldüğü üzere Entity Framework tarafında asenkron programlama desteği de artık adım adım gelmektedir. Alpha sürümüne ait yapmış olduğumu bu örnek uygulamanın beta ve release sürümlerinde çok fazla değişikliğe uğramayacağını ama async takılı ek metodların da gelebileceğini düşünmekteyim. Entity Framework’ ün yeni özelliklerine dair bilgileri ilerleyen zamanlarda paylaşmaya devam ediyor olacağım. Örneğin bu yeniliklerden birisi Code-First tarafında Stored Procedure ve Function tanımlanıp kullanılabilmesidir. Tekrardan görüşmek dileğiyle hepinize mutlu günler dilerim

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_146.png)

[EF6_Alpha_AsyncAwait.zip (2,25 mb)](/assets/files/2012/EF6_Alpha_AsyncAwait.zip)

[Örnekte Entity Framework 6.0 Alpha 1 sürümü kullanılmıştır zip boyutunu küçültmek için içerideki nuget bazlı package klasörü çıkartılmıştır.]