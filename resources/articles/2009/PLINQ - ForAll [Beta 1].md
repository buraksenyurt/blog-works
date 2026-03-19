---
title: "PLINQ - ForAll [Beta 1]"
pubDate: 2009-05-28 08:43:00
categories:
  - LINQ
  - PLINQ
tags:
  - LINQ
  - PLINQ
  - C#
  - Threading
  - Delegates
  - Generics
---

# PLINQ - ForAll [Beta 1]
Merhaba Arkadaşlar,

Bildiğiniz gibi bir süredir LINQ sorgularının paralel çalıştırılması ile ilişkili çalışmalarıma ve araştırmalarıma devam etmekteyim. Bu yazımdaki konumuz ise System.Linq.ParallelEnumerable static sınıfı içerisinde tanımlanmış olan ForAll genişletme metodudur (extension methods).

public static void ForAll (this ParallelQuery source, Action action);

ForAll metodu yukarıdaki prototipinden de görüldüğü gibi ParallelQuery referanslarına uygulanabilmektedir. Bununla birlikte metod ikinci parametre olarak, Action tipinden generic bir temsilci almaktadır.

public delegate void Action (T obj);

Yukarıdaki prototipe göreyse, Action temsilcisi (delegate), generic tip olarak ForAll metoduna gelen tipi (TSource) kullanmaktadır. Bu generic tip tahmin edeceğiniz üzere ParalleQuery referansınında kaynak tipidir. Ayrıca temsilci geriye herhangibir değer döndürmeyen (void) metodları işaret edebilmektedir.

Sonuç olarak ForAll metodu aslında, AsParallel metodunun kullanılması sonucu üretilen referans üzerinden gelen her bir nesne örneği için yapılması istenen işlemleri ele almaktadır. Bu açıdan bakıldığında akla gelen soru şu olacaktır.

Paralel sorguların çalışması sonucu üretilen çıktılar üzerinde foreach döngüleri yardımıyla da dolaşabiliyorken, ForAll metodunu neden kullanırız?

Aşağıdaki kod parçasını göz önüne alalım.

```csharp
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;

namespace UsingForAll
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Product> productList = GetProductList();

            var result = from p in productList.AsParallel()//.WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                         where p.ListPrice>=400 && p.Color=="Black"
                         select p;

            result.ForAll(p=>Console.WriteLine("("+Thread.CurrentThread.ManagedThreadId.ToString()+")\t"+p.Name));
        }

        static List<Product> GetProductList()
        {
            List<Product> productList = new List<Product>();

            SqlConnection conn = new SqlConnection("data source=Manchester;database=AdventureWorks2008;integrated security=true");
            SqlCommand cmd = new SqlCommand("Select ProductId,Name,ListPrice,ProductNumber,Color,SafetyStockLevel From Production.Product", conn);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (reader.Read())
            {
                productList.Add(new Product
                {
                    ProductId = Convert.ToInt32(reader[0]),
                    Name = reader[1].ToString(),
                    ListPrice = Convert.ToDecimal(reader[2]),
                    ProductNumber = reader[3].ToString(),
                    Color = reader[4].ToString(),
                    SafetyStockLevel = Convert.ToInt32(reader[5])
                }
                );
            }
            reader.Close();
            return productList;
        }
    }

    class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal ListPrice { get; set; }
        public string ProductNumber { get; set; }
        public string Color { get; set; }
        public int SafetyStockLevel { get; set; }
    }
}
```

Bu kod parçasında odaklanmamız gereken nokta result referansı üzerinden ForAll metodunun çağırılışıdır. Bu çağrı sırasında labmda operatöründen (=>) yaralanılmaktadır ve bulunan ürünlerin adları ile o an çalışmakta olan Thread'in numarası (ManagedThreadId) Console ekranına yazdırılmaktadır. Sonuç aşağıdakine benzer olacaktır.

![blg23_1.gif](images/blg23_1.gif)

Her ne kadar Thread sayıları eşit olmasada 4 ve 1 nolu iki ayrı iş parçasının çalıştırıldığı görülmektedir. Şimdi aynı sorgu sonuçlarını foreach döngüsü yardımıyla elde etmeye çalıştığımızı düşünelim.

```csharp
List<Product> productList = GetProductList();

var result = from p in productList.AsParallel()//.WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                 where p.ListPrice>=400 && p.Color=="Black"
                 select p;

foreach (Product p in result)
{
   Console.WriteLine("(" + Thread.CurrentThread.ManagedThreadId.ToString() + ")\t" + p.Name);
}
```

Bu kez uygulama çalıştırıldığında aşağıdaki sonuçları alırız.

![blg23_2.gif](images/blg23_2.gif)

Volaaa!!!

![Cool](images/smiley-cool.gif)

1 numaralı sadece tek bir thread görünüyor.

Bu nasıl oldu? Acaba foreach döngüsü kullanıldığında sorgu AsParallel metodu olmasına rağmen paralel çalıştırılmadı mı? Yoksa çalışma zamanı (runtime) sorgunun paralel çalıştırılmaya değer olmadığına mı kanaat getirdi (ki böyle bir meselede var)?

Aslında farklı çalışmanın sebebi şu. LINQ sorguları bilindiği gibi kullanıldıkları yerde çalıştırılırlar (deferred execution ilkesi). Bu nedenle sorgunun çalıştırılması foreach döngüsünde ilk eleman elde edilmeye çalışıldığı sırada olur. Lakin sorgu AsParallel metodu nedeniyle paralel çalışmasına rağmen, foreach metodu okuma işlemine başlamadan önce tüm yönetimli thread'leri tekrardan tek bir thread içerisinde birleştirir. Yani foreach döngüsünün kendisi paralel çalışma özelliğne sahip değildir. Bu nedenle paralel çalıştırılan sorgu sonuçlarını, o an üzerinde çalıştığı thread'de birleştirmeden ilerleyemez. ForAll metodu ise tam aksine çalışmakta olup, okuma işlemlerininde paralel yürütülmesini sağlamaktadır. Aslında durumu basit iki resim ile canlandırmaya çalışalım. Aşağıdaki şekilde foreach çalışması sırasındaki işleyiş sembolize edilmektedir.

![blg23_4.gif](images/blg23_4.gif)

Buna göre sorgu paralel olarak çalışan görevlere ayrılmakta ve herbir görev içerisinde where gibi koşullar ele alınmaktadır. Ancak tüm PLINQ ifadesi tamamlandığında sonuçlar tek bir Task altında birleştirilmektedir. (Kahverengi çerçeveli kutucuklar bulunan nesne örnekleri üzerinden yapılan işlemleri sembolize etmektedir. Örneğin Console.WriteLine gibi) Sonrasında ise herbir öğe için foreach döngüsü içerisinde yazılan kodlar işletilmektedir.

Aşağıdaki şekilde ise ForAll kullanımı sırasındaki senaryo ifade edilmeye çalışılmaktadır.

![blg23_3.gif](images/blg23_3.gif)

Yine PLINQ ifadesinin çalışması sırasında n adet Task paralel olarak başlatılır. Ancak foreach'ten farklı olarak her task'in içerisinde hem Where gibi koşulların kontrolü ele alınmakta hemde örneğimizde ki her bir sonuç için ayrı ayrı işlemler (Console.WriteLine gibi) gerçekleştirilmektedir. Yani task'ler paralel olarak işledikten sonra tek bir Task altında birleştirilmezler. Sanıyorum şekil yardımıyla sizde benim gibi, gerçekleşen iki farklı işleyişi daha net canlandırabildiniz. (Tabi işlemcinin içerisine girip olan biteni canlı canlı görmemiz mümkün değil. Ama kim bilir, belki gelecek nesil sistemlerde çalışma zamanını, tıpkı bir doktorun sanal bir hastanın organları içerisinde ilerleyişi gibi, bilgisayar donanımı üzerindem gözlemleyebiliriz. ![Wink](images/smiley-wink.gif))

Herşey güzel ama, hangisini ne zaman kullanmak gerekir öyleyse?

Aslında foreach döngüsünü daha çok sorgu sonuçlarının sırasını (order) korumak istediğimiz durumlarda değerlendirebiliriz. Bununla birlikte, sonuç listesi üzerinde ardışıl olarak işlemler yapmak istiyorsakta tercih edebiliriz.

Böylece geldik kısa bir yazımızın daha sonuna. Bir sonraki yazımızda görüşünceye dek hepinize mutlu günler dilerim.

[UsingForAll.rar (22,17 kb)](assets/UsingForAll.rar)
