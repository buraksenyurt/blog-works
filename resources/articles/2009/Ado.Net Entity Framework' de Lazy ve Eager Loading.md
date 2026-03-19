---
title: "Ado.Net Entity Framework' de Lazy ve Eager Loading"
pubDate: 2009-04-16 03:41:00
categories:
  - Entity Framework
tags:
  - Entity Framework
  - C#
  - .NET
  - ADO.NET
  - LINQ
  - Performance
  - Generics
  - Visual Studio
---

# Ado.Net Entity Framework' de Lazy ve Eager Loading
Merhaba Arkadaşlar,

Bildiğiniz üzere uzun bir süre önce Microsoft, LINQ to SQL yerine Ado.Net Entity Framework ile ilerleme kararı aldı. Bu konu ile ilişkili olaraktan okuduğum hemem hemen bütün kitaplarda Ado.Net'in geleceğinde önemli bir yere sahip olan Ado.Net Entity Framework alt yapısının geliştiriciler tarafından asla ihmal edilmemesi gerektiğide sıkça vurgulanmakta. Peki günlüğüme konu olan mesele nedir?

Aslında günlüğe yazmadan önce odaklandığım nokta, aralarında master-detail ilişki bulunan tablo verilerinin, LINQ to SQL tarafında nasıl yüklendiğinin incelenmesiydi. Dolayısıyla önce LINQ to SQL tarafındaki duruma bir göz atmakya yarar var. İşe öncelikle basit bir Console uygulamasında kobay nesnelerimizden Northwind veritabanını kullanarak başlayabiliriz. Burada Visual Studio 2008 üzerinde ve.Net Framework 3.5 tabanlı bir geliştirme yaptığımızı belirtelim. Söz konusu uygulamamızda kullanacağımız LINQ to SQL diagramının içeriği aşağıdaki gibi tasarlanabilir.

![blog1_1.gif](images/blog1_1.gif)

Örnekte Category ve Product tablolarına ait tipleri göz önüne almaktayız. Buna göre "Kategoriler ve bu kategorilerdeki toplam ürün sayılarını öğrenmek" gibi basit bir sonuç kümesi elde etmek istediğimizde aşağıdakine benzer program kodlarını ele alacağımız muhtemeldir.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlogSample
{
    class Program
    {
        static void Main(string[] args)
        {
            using (NorthwindDataContext northContext = new NorthwindDataContext())
            {
                // northContext.Log = Console.Out; // İsterseniz SQL sorgularını Console çıktısından da takip edebilirsiniz.
                
                foreach (Category category in northContext.Categories)
                {
                    Console.WriteLine("Category Name :{0} ({1})", category.CategoryName, category.Products.Count);
                }
            }
        }
    }
}
```

Programın bu haliyele çıktısı aşağıdaki gibidir.

![blog1_2.gif](images/blog1_2.gif)

Aslında istediğimizi elde etmiş görünüyoruz.

![Cool](images/smiley-cool.gif)

Gayet doğal olarak foreach döngüsü içerisinde bir kategoriye bağlı ürün sayıları bulunurken Count özelliğinden yararlanılmaktadır. Ancak bu durumda Lazy Loading adı verilen durum oluşmakta ve SQL tarafına bakıldığında oldukça fazla sorgunun çalıştığı görülmektedir. Öyleki yukarıdaki kod çıktısı SQL Profiler yardımıyla incelendiğinde ilk sırada aşağıdaki sorgunun çalıştığı görülür.

![blog1_3.gif](images/blog1_3.gif)

Görüldüğü üzere ilk olarak tüm kategoriler Select sorgusu ile çekilmektedir. Ancak iş bundan sonra biraz daha dikkate değer bir hal alır. Nitekim her kategoriye ait ürün sayıları Products özelliği üzerinden elde edilmek istenmektedir. Bu durumda SQL tarafında her bir kategori satırı için birer sorgu cümlesi daha çalıştırılır.

![blog1_4.gif](images/blog1_4.gif)

Bu bir kaç satırlık veri için önemli gözükmese de, büyük boyutlu veriler ile çalışıldığı durumlarda önemli performans kayıplarına neden olabilir. Bu nedenle istenirse Eager Loading isimli bir teknikten de yararlanılabilir. Tek yapılması gereken kod tarafına aşağıdaki değişiklikleri eklemektir.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace BlogSample
{
    class Program
    {
        static void Main(string[] args)
        {
            using (NorthwindDataContext northContext = new NorthwindDataContext())
            {
                // northContext.Log = Console.Out; // İsterseniz SQL sorgularını Console çıktısından da takip edebilirsiniz.
                DataLoadOptions loadOption = new DataLoadOptions();
                loadOption.LoadWith<Category>(c => c.Products);
                northContext.LoadOptions = loadOption;

                foreach (Category category in northContext.Categories)
                {
                    Console.WriteLine("Category Name :{0} ({1})", category.CategoryName, category.Products.Count);
                }
            }
        }
    }
}
```

Bu durumda da kod aynı sonuçları verir ve SQL tarafındaki çıktıya bakıldığına tek bir sorgunun çalıştırıldığı gözlemlenebilir.

![blog1_5.gif](images/blog1_5.gif)

Ancak en iyi performans için bu teknikler yerine Anonymous Type kullanılması çok daha doğrudur. Öyleki sorguda istenen sadece kategori adları ve o kategoriye bağlı ürünlerin toplam sayılarıdır. Oysa ki sorgu cümlelerine bakıldığında o anda gerekli olmayan tablo alanlarının da hesaba katıldığı görülmektedir. Dolayısıyla sorgunun sadece, CategoryName ve buna bağlı Product satırlarının toplam sayısını bulacak şekilde iyileştirilebilmesi gerekmektedir. Aslında burada LINQ sorgularının defered execution adı verilen "gerektiği yerde sorguyu gönder" sistemide önemlidir. Kısaca LINQ sorgusunun yazıldığı satırda değilde, ilk kullanıldığı yerde SQL cümlesinin gönderilmesinden bahsediyoruz. Lafı fazla uzatmadan kod tarafındaki değişikliklerimize bakalım.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace BlogSample
{
    class Program
    {
        static void Main(string[] args)
        {
            using (NorthwindDataContext northContext = new NorthwindDataContext())
            {
                var resultSet = from c in northContext.Categories
                                select new
                                {
                                    c.CategoryName,
                                    c.Products.Count
                                };
                foreach (var result in resultSet)
                {
                    Console.WriteLine("Category Name :{0} ({1})", result.CategoryName,result.Count.ToString());
                }
            }
        }
    }
}
```

Bu durumda SQL tarafına giden sorgunun aşağıdaki gibi olduğu görülebilir.

![blog1_6.gif](images/blog1_6.gif)

Dikkat edileceği üzere sadece istediğimiz alanlar sorguya dahil edilmiştir.

Derken rüyadan uyanırım ve birden aklıma artık Ado.Net Entity Framework konulu bir yazı yazacağım gelir.

![Embarassed](images/smiley-embarassed.gif)

Bakalım o tararfa Lazy Loading, Eager Loading durumları nasıl ele alınabilir. Bu kez projede kullanacağımız tiplerin EDM (Entity Data Model) diagramındaki görüntüsü aşağıdaki gibidir. Aynen yukarıda örneklerde olduğu gibi Category ve Product tablolarını ele alıyor olacağız.

![blog1_7.gif](images/blog1_7.gif)

Şimdi ilk kod parçamızı geliştirelim.

```csharp
using (NorthwindEntities entity = new NorthwindEntities())
{
	foreach (var category in entity.Categories)
	{
		Console.WriteLine("Category Name :{0}({1})",category.CategoryName,category.Products.Count);
	}
}
```

SQL Profiler'a geçmeden önce uygulama çalıştırıldığında aşağıdaki sonuç ile karşılaşılır.

![blog1_8.gif](images/blog1_8.gif)

Dikkatli gözlerden, ürün sayılarının 0 olarak geldiği kaçmayacaktır. SQL Profiler aracı ile gönderilen sorguya bakıldığındaysa sadece kategorilerin çekildiği ancak ürün sayılarının bulunması ile ilişkili bir şey yapılmadığı gözlemlenebilir.

![blog1_9.gif](images/blog1_9.gif)

Aslında bu son derece doğaldır nitekim Ado.Net Entity Framework modelinde Lazy Loading'in bilinçli ve açık bir şekilde yapılması istenmektedir. Dolayısıyla geliştiricinin gerçekten Lazy Loading yapmak istediğini kod tarafında belirtmesi gerekir. Peki bu nasıl yapılır? Aşağıdaki basit kod parçasında olduğu gibi...

```csharp
using (NorthwindEntities entity = new NorthwindEntities())
{
	foreach (var category in entity.Categories)
	{
		if(!category.Products.IsLoaded)
			category.Products.Load();
		Console.WriteLine("Category Name :{0}({1})",category.CategoryName,category.Products.Count);
	}
}
```

SQL Profiler'a bakıldığında aşağıdaki çıktı ile karşılaşılır.

İlk olarak kateogoriler çekilmiş sonrasında ilk 5 kategori için sırasıyla ürünlere ait sorgular çalıştırılmıştır. Ardından ise Category tablosu için yine bir Select çalıştırılmakta ve kalan 3 kategorinin her biri için tekrardan ürün sorguları yürütülmektedir.(Bu yazıda anlattıklarımı bire bir uygulamanızı, SQL Profiler aracaı yardımıylada irdelemeye çalışmanızı şiddetle öneririm.)

![blog1_11.gif](images/blog1_11.gif)

Buradaki kod parçasında her bir kategori için buna bağlı ürünlerinde Products özelliği ile işaret edilen referansa yüklenmesi istendiği Load metodu yardımıyla belirtilmektedir. Bu kod ile Lazy Loading gerçekleştirilmiş olmaktadır. Peki ya Eager Loading? Eager Loading için aşağıdaki kod parçasını kullanmak yeterli olacaktır.

```csharp
using (NorthwindEntities entity = new NorthwindEntities())
	{
		foreach (var category in entity.Categories.Include("Products"))
		{                    
			Console.WriteLine("Category Name :{0}({1})",category.CategoryName,category.Products.Count);
		}
	}}               
}
```

Aslında tek yaptığımız Categories özelliği üzerinden Include metodunu çağırmak ve Products değerini vermek olmuştur. Buna göre SQL tarafında aşağıdaki sorgunun çalıştırıldığı görülür.

![blog1_12.gif](images/blog1_12.gif)

Elbette yine istediğimizi alamadık. Nitekim sadece kategori adı ve ürün sayılarını elde etmek gibi bir amacımız vardı. LINQ to SQL tarafında yapmış olduğumuz iyileştirmeyi Ado.Net Entity Framework tarafında da Include metodunu hesaba katarak ve yine Anonymous Type kullanarak gerçekleştirebiliriz.

```csharp
using (NorthwindEntities entity = new NorthwindEntities())
{
	var resultSet = from c in entity.Categories.Include("Products")
					select new
					{
						c.CategoryName,
						c.Products.Count
					};
	foreach (var result in resultSet)
	{
		Console.WriteLine("Category Name :{0}({1})", result.CategoryName, result.Count);
	
```

Sonuç olarak SQL tarafına aşağıdaki sorgu cümlesi gönderilecektir.

![blog1_13.gif](images/blog1_13.gif)

Görüdüğü üzere sadece CategoryName ve sub query ile birlikte ürün sayıları hesaba katılmaktadır.

Böylece geldik bir günlük yazımızın daha sonuna. Bu yazımızda LINQ to SQL ve Ado.Net Entity Framework tarafında Lazy ve Eager Loading kavramlarını değerlendirmeye çalıştık. Umarım yararlı olmuştur.

Görüşmek dileğiyle...
