---
title: "PLINQ - Sıralamayı(Ordering) Korumak [Beta 1]"
pubDate: 2009-05-23 20:30:00
categories:
  - PLINQ
tags:
  - PLINQ
  - C#
  - LINQ
  - Concurrency
  - Generics
  - Visual Studio
---

# PLINQ - Sıralamayı(Ordering) Korumak [Beta 1]
Merhaba Arkadaşlar,

Hatırlayacağınız gibi, PLINQ (Parallel LINQ) ile ilişkili ilk [yazımda](https://www.buraksenyurt.com/post/PLINQ-(Parallel-LINQ)-Hello-World), LINQ sorgularının eş zamanlı olarak nasıl çalıştırılabileceğini incelemeye çalışmıştık. Hello World örneğimizde ağırlıklı olarak aşağıdaki sorgu üzerinde durmuştuk.

```csharp
var result2 = from p in products.AsParallel()
                          where p.ListPrice >= 10 && p.InStock==true
                          orderby p.Name descending
                          select p;
```

Bu sorguda yer alan orderby kelimesi aslında çok büyük bir öneme sahiptir. Gelin ne demek istediğimi size anlatmaya çalışayım. Yine Visual Studio 2010 Professional Beta 1 ortamında geliştirilen aşağıdaki kod parçasına sahip bir Console uygulamamız olduğunu göz önüne alacağız.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Product> products = FillProducts();
            Console.WriteLine("Liste dolduruldu. İşlemlere devam etmek için tıklayın");
            Console.ReadLine();

            Console.WriteLine("Listenin ilk hali");
            foreach (Product prd in products)
            {
                if(prd.InStock==true)
                    Console.WriteLine(prd.Name);
            }

            var result = from p in products.AsParallel()
                         where p.InStock == true
                         select p;

            Console.WriteLine("AsParalell sonrası listenin hali");
            foreach (Product prd in result)
            {
                Console.WriteLine(prd.Name);
            }
        }

        static List<Product> FillProducts()
        {
            List<Product> products = new List<Product>();

            for (long i = 1; i < 21; i++)
            {
                Product prd = new Product
                {
                    Id = i
                    ,Name = "Product" + i.ToString()
                    ,ListPrice = i * 0.1M
                    ,InStock = i % 2 == 0 ? true : false
                };
                products.Add(prd);
            }

            return products;
        }
    }

    class Product
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal ListPrice { get; set; }
        public bool InStock { get; set; }
    }
}
```

Bu seferki örneğimizde temel amacımız hız veya işlemlerin daha kısa sürede tamamlanması değildir. products isimli generic List koleksiyonu doldurulduktan sonra bir foreach döngüsü yardımıyla dolaşılmakta ve stokta olanlar (InStock==true) ekrana yazdırılmaktadır. Sonrasında ise PLINQ sorgumuz gelmekte ve aynı sonuçları paralel çalışan task'ler üzerinde elde etmemizi sağlamaktadır. Hemen küçük bir dip not belirtelim; LINQ sorguları aslında kullanıldıkları anda çalıştırılmaktadır (Deferred Execution). Yani çalışma zamanında ikinci foreach döngüsüne gelindiğinde, söz konusu PLINQ sorgusu yürütülmekte ve dolayısıyla paralel görevler devreye girmektedir. Peki herşey güzel hoş... Hoş ama niye bunun üzerinde duruyoruz? Aslında çalışma zamanındaki ekran görüntüsü herşeyi biraz olsun açıklıyor.

![blg20_1.gif](images/blg20_1.gif)

Dikkat edileceği üzere, listenin ilk halinde stokta olan ürünler, koleksiyonda yer aldıkları sıraya göre ekrana getirilmektedir. Ancak AsParallel genişletme metodunun kullanılması sonrasında elde edilen listedeki ürünler sıralı bir şekilde gelmemektedir. Bu çok doğal olarak paralel çalışmanın bir sonucudur. Ancak bazı hallerde AsParallel kullanımı sonrasında, kaynak listenin sıralı olarak elde edilmesi istenebilir. Bu durumda orderby kullanımı sorunu çözecektir. Söz gelimi yukarıdaki örnekte yer alan LINQ ifadesinde orderby aşağıdaki gibi kullanılabilir.

```csharp
var result = from p in products.AsParallel()
                         where p.InStock == true
                         orderby p.Name
                         select p;
```

Bu durumda örneğin çalışması sonucu aşağıdaki çıktı elde edilir.

![blg20_2.gif](images/blg20_2.gif)

Ancak PLINQ tipinden sorgunun çalıştırılması sırasında orjinal nesne sırasının korunması da istenebilir ki bu orderby kullanımından daha farklı anlamdadır. (orderby kullanımında listenin, koleksiyondaki orjinal sırası yerine, sıralama kriteri belirtilir.) İşte bu noktada devreye ParallelEnumerable static sınıfı içerisinde tanımlanmış olan AsOrdered isimli genişletme metodu (Extension Method) girer. Yani sorguyu aşağıdaki hale getirirsek, liste elemanlarının koleksiyon içerisindeki orjinal sırasını koruyarak sonuç elde edilmesi sağlanabilir.

```csharp
var result = from p in products.AsParallel().AsOrdered()
                         where p.InStock == true
                         select p;
```

Ve sonuç...

![blg20_3.gif](images/blg20_3.gif)

Tabi burada önemli bir nokta daha vardır. AsOrdered çok doğal olarak PLINQ sorgunun çalışma zamanında yavaş işlemesine neden olacaktır. Çünkü, sorgu sonucu elde edilen liste eşitliğin sol tarafına aktarılmadan önce, AsOrdered nedeni ile orjinal sıra konumlarına yerleştirilir. Bu ek işlem, sonucun elde edilmesini yavaşlatacaktır. Bu nedenle MSND kaynaklarında, AsOrdered genişletme metodunun gerekmedikçe kullanılmaması öğütlenmektedir. Hatta, orderby kullanımının tercih edilmesi önerilmektedir. Yazımızın başında belirttiğimiz Orderby kullanımının neden önemli olduğunu sanırım anlamış bulunuyoruz. Tabiki zorunlu hallerde AsOrdered kullanılmasıda gerekebilir.

Bu yazımda sizlere önemsiz gibi görünen fakat dikkat edilmesi gereken bir konuyu aktarmaya çalıştım. Bir sonraki yazımızda görüşünceye dek mutlu günler dilerim.

[Ordering.rar (22,74 kb)](assets/Ordering.rar)
