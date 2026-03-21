---
layout: post
title: "Karmaşık Değil Son Derece Basit"
date: 2011-09-29 06:50:00 +0300
categories:
  - dotnet-framework-4-0
  - csharp-4-0
tags:
  - generic
  - generics
  - interface
  - domain
  - binary-serialization
---
Kurumsal eğitim vermenin en güzel yanlarından birisi de, gelenlerin istekleri ve talepleri doğrultusunda gerçek hayat örneklerini daha kolay bir şekilde kodlayabilmeniz ve gösterebilmenizdir.

[![experience](/assets/images/2011/experience_thumb.jpg)](/assets/images/2011/experience.jpg)


Söz gelimi geçtiğimiz hafta içerisinde vermeye başladığım ve makaleyi yazdığım tarih itibariyle devam etmekte olan bir eğitim sırasında, Binary ve XML Serileştirme konularını anlatırken, sahip olduğumuz dil ve framework materyallerinden bazılarını iç içe ve ne kadar etkili kullanabildiğimizi gördük

![Göz kırpan gülümseme](/assets/images/2011/wlEmoticon-winkingsmile_66.png)

Bu durumdan esinlenerek sizlere de bir gerçek hayat örneği aktarmaya çalışmak isterim.

İlk önce ne yapacağımızı belirtmem gerekiyor ama bunu en sona bırakmak ve ne yapmış olduğumuzu o zaman göstermek (aslında sizin anladığınızı görmek) arzusundayım. Öncelikli olarak aşağıdaki şekilde görülen Solution yapısını oluşturarak işe başlayabiliriz. Tabi ki bu yapı bizim test çözümümüz olarak tasarlanmıştır.

[![bei_3](/assets/images/2011/bei_3_thumb.gif)](/assets/images/2011/bei_3.gif)

Common ve DomainLibrary isimli projelerimiz birer Class Library iken TestApp tahmin edileceği üzere bir Console uygulamasıdır. Şimdi de DomainLibrary içeriğini aşağıdaki Class Diagram’ da olduğu gibi yapılandıralım.

[![bei_4](/assets/images/2011/bei_4_thumb.gif)](/assets/images/2011/bei_4.gif)

ISerializationRule.cs;

```csharp
namespace DomainLibrary 
{ 
    public interface ISerializationRule 
    { 
    } 
}
```

Product.cs;

```csharp
using System;

namespace DomainLibrary 
{ 
    [Serializable] 
    public class Product 
        :ISerializationRule 
    { 
        public int Id { get; set; } 
        public string Name { get; set; } 
        public int CategoryId { get; set; } 
        public decimal ListPrice { get; set; } 
    } 
}
```

Category.cs;

```csharp
using System; 
namespace DomainLibrary 
{ 
    [Serializable] 
    public class Category 
       :ISerializationRule 
    { 
        public int Id { get; set; } 
        public string Name { get; set; }       
    } 
}
```

ISerializationRule interface tipini uygulamakta olan Category ve Product isimli iki sınıfımız bulunduğunu görmekteyiz. ISerializationRule arayüzü herhangibir kural bildirimi yapmasa bile ilerleyen bölümlerde çok kritik bir görevi üstlenecektir

![Göz kırpan gülümseme](/assets/images/2011/wlEmoticon-winkingsmile_66.png)

Şimdi de Common isimli sınıf kütüphanemiz içerisine aşağıdaki Operations sınıfını ve içeriğini eklediğimizi düşünelim.

[![bei_5](/assets/images/2011/bei_5_thumb.gif)](/assets/images/2011/bei_5.gif)

```csharp
using System; 
using System.Collections.Generic; 
using System.IO; 
using System.Runtime.Serialization.Formatters.Binary; 
using DomainLibrary;

namespace Common 
{ 
    public class Operations 
    { 
        public bool BinarySerialize<T>(List<T> Source, string FilePath) 
            where T : ISerializationRule        
        { 
            bool result = false;

            try 
            { 
                using (FileStream fs = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.Write)) 
                { 
                    BinaryFormatter formatter = new BinaryFormatter(); 
                    formatter.Serialize(fs, Source); 
                    result = true; 
                } 
            } 
            catch (Exception excp) 
            { 
                throw excp; 
            } 
            return result; 
        } 
    } 
}
```

![Göz kırpan gülümseme](/assets/images/2011/wlEmoticon-winkingsmile_66.png)

Operations sınıfı içerisinde Binary serileştirme işlemi olan bir metod olduğunu görmektesiniz. Söz konusu metod generic olarak tasarlanmıştır. Generic olmakla kalmayıp bir de kısıtlama (Constraints) getirmiştir. Bu kısıtlamaya göre T tipinin ISerializationRule isimli arayüz tarafından taşınabilen bir referans olması şartı konulmaktadır

![Gülümseme](/assets/images/2011/wlEmoticon-smile_15.png)

Bir başka deyişle az önce tasarlamış olduğumuz Domain yapısı içerisinde yer alan ve ISerializationRule arayüzünü uygulayan tipler için bu metodun kullanılabilmesi mümkündür. Dolayısıyla bu koşulun dışında kalan tipler için söz konusu metod kullanılamayacaktır. Sanırım bu gerçek hayat örneğinin en can alıcı noktası da burasıdır. Bizim sahip olduğumu bir Domain ile çalışabilecek generic bir serileştirme metodu geliştirmiş bulunmaktayız.

Metodun içeriği son derece basittir. Exception yönetimi metodu kullanan bir üst katmana bırakılmıştır (catch bloğu içerisinde yaptığımız throw hareketine dikkat edin![Göz kırpan gülümseme](/assets/images/2011/wlEmoticon-winkingsmile_66.png)) Diğer yandan BinaryFormatter tipinden yararlanılmış ve metoda parametre olarak gelen FilePath ile işaret edilen ve FileStream ile yazmak üzere açılan dosyaya doğru bir serileştirme işlemi gerçekleştirilmektedir.

Şimdi söz konusu operasyonu test edeceğimiz kod içeriğini de Console uygulamamızda aşağıdaki gibi geliştirelim.

```csharp
using System; 
using System.Collections.Generic; 
using System.IO; 
using Common; 
using DomainLibrary;

namespace TestApp 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            List<Category> categories = new List<Category>() 
            { 
                new Category{ Id=1,Name="Book"}, 
                new Category{ Id=2,Name="Music"} 
            };

            List<Product> products = new List<Product>() 
            { 
                new Product{ Id=1,CategoryId=1, Name="Kitap 1", ListPrice=10}, 
                new Product{ Id=2,CategoryId=1, Name="Kitap 2", ListPrice=5}, 
                new Product{ Id=3,CategoryId=2, Name="Muzik 1", ListPrice=15}, 
                new Product{ Id=4,CategoryId=1, Name="Kitap 3", ListPrice=3}, 
                new Product{ Id=5,CategoryId=2, Name="Muzik 2", ListPrice=9}, 
                new Product{ Id=6,CategoryId=1, Name="Kitap 4", ListPrice=8}, 
            };

            Operations opt = new Operations(); 
            bool result1=opt.BinarySerialize<Product>(products, Path.Combine(Environment.CurrentDirectory, "Products.bin")); 
            bool result2 = opt.BinarySerialize<Category>(categories, Path.Combine(Environment.CurrentDirectory, "Categories.bin"));

            if(result1) 
                Console.WriteLine("Products listesi binary serileştirildi"); 
            if(result2) 
                Console.WriteLine("Categories listesi binary serileştirildi"); 
        } 
    } 
}
```

Örneğimizde sembolik olarak Product ve Category tipinden birer koleksiyon üretilmekte ve bunlar için Binary serileştirme işlemi icra edilmektedir. Uygulamayı çalıştırdığımızda aşağıdakine benzer bir ekran çıktısı ile karşılaşırız

![Göz kırpan gülümseme](/assets/images/2011/wlEmoticon-winkingsmile_66.png)

[![bei_6](/assets/images/2011/bei_6_thumb.gif)](/assets/images/2011/bei_6.gif)

Tabi çıktı olarak üretilen Binary dosyaların içeriği de aşağıdaki gibi oluşturulacaktır.

Categories.bin içeriği;

ÿÿÿÿ DDomainLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null System.Collections.Generic.List`1[[DomainLibrary.Category, DomainLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]] _items_size_version DomainLibrary.Category[] DomainLibrary.Category
DomainLibrary.Category k__BackingFieldk__BackingField Book Music

Products.bin içeriği;

ÿÿÿÿ DDomainLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null €System.Collections.Generic.List`1[[DomainLibrary.Product, DomainLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]] _items_size_version DomainLibrary.Product[] DomainLibrary.Product
DomainLibrary.Product k__BackingFieldk__BackingFieldk__BackingFieldk__BackingField
Kitap 1 10 Kitap 2 5 Muzik 1 15
Kitap 3 3 Muzik 2 9 Kitap 4 8

Aslında bu geliştirdiğimiz örnek ile kazandığımız bir takım avantajlar olduğunu vurgulamalıyız. Öncelikli olarak development safhasındayken Binary serileştirme işini üstlenen metoda atayabileceğimiz tipler için Business anlamda bir Domain kuralı getirmiş bulunmaktayız. Bunu metodu çağırdığımız sırada da zaten net bir şekilde görebiliriz.

[![bei_7](/assets/images/2011/bei_7_thumb.gif)](/assets/images/2011/bei_7.gif)

Dikkat edileceği üzere kırmızı kutucuk içerisinde almış olduğumuz kısım ile T tipinin ISerializationRule arayüzü tarafından taşınabilecek bir tip olması zorunluluğu geliştiriciye bildirilmiş oluyor

![Göz kırpan gülümseme](/assets/images/2011/wlEmoticon-winkingsmile_66.png)

Sanırım şu anda ne demek istediğimi daha net anlatabilmişimdir.

Geliştirdiğimiz örnek Solution içerisinde yer alan Assembly’ lar arası bağları ele alarak makalemizi yavaş yavaş sonlandırmaya başlayalım. Tam olarak Assembly’ larımız arası ilişki aşağıdaki şekilde görüldüğü gibidir.

[![bei_8](/assets/images/2011/bei_8_thumb.gif)](/assets/images/2011/bei_8.gif)

Generate Dependency Graph’ ı seviyorummm

![Açık ağızlı gülümseme](/assets/images/2011/wlEmoticon-openmouthedsmile_15.png)

Peki bu çözümde neleri kullandık?

- Generic bir metod geliştirdik.
- Generic metodda kullandığımız T tipi için generic kısıtlama (Constraint) kullandık.
- Kendi Domain yapımızı düşündük ve bir arayüz (ISerializationRule) ile generic kısıtlama için imkan sağladık.
- BinaryFormatter tipi ile serileştirme işlemini icra ettik.
- Serileştirme hedefi olarak fiziki bir dosya bağlantısını kullandık (FileStream).
- Class Diagram’ ları kullanarak Domain yapımızı daha net görebildik.
- Assembly bazında Dependency Graph üreterek Assembly’ larımız arası referans bağımlılıklarını da daha net görebildik.

Görüldüğü üzere sahip olduğumuz.Net bilgi ve materyallerini bazı durumlarda bir araya getirip gerçek hayat senaryoları için icra ettirebiliyoruz. Tabi söz konusu senaryoya eklenebilecek daha pek çok fonksiyonellik söz konusu olabilir. İlerleyen zamanlarda başka gerçek hayat örneklerini de sizlerle paylaşmaya çalışıyor olacağım. Özellikle bu yazıda katılımcı arkadaşlarımızın oldukça büyük emeği var. Kendilerine de çok teşekkür ediyorum. Tekrardan görüşünceye dek hepinize mutlu günler dilerim

![Gülümseme](/assets/images/2011/wlEmoticon-smile_15.png)

[GercekHayatOrnekleri.rar (58,66 kb)](/assets/files/2011/GercekHayatOrnekleri.rar)