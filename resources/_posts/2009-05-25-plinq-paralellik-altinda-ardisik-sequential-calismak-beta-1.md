---
layout: post
title: "PLINQ - Paralellik Altında Ardışık(Sequential) Çalışmak [Beta 1]"
date: 2009-05-25 11:34:00 +0300
categories:
  - linq
  - plinq
tags:
  - plinq
  - language-integrated-query
---
Bir önceki blog yazımızda PLINQ ifadelerinde sıralama konusuna değinmeye çalışmıştık. Bu yazımızda ise, paralel olarak çalıştırılan LINQ sorguları içerisinde, ardışık (Sequential) olarak nasıl işlem yapılabileceğini incelemeye çalışacağız.

PLINQ ifadeleri, sorgu içerisindeki işlemleri paralel çalışan görevlere ayırmakta son derece başarılıdır. Ancak öyle senaryolar olabilirki, sorgunun belirli bir noktasından (noktalarından) sonra ardışık olarak işlemlerin devam etmesi istenebilir.(Hatta sonra tekrardan paralel olarak devam edilmeside sağlanabilir) Tabi bu şekilde anlatmaya çalışınca inanın benim kafamda karışıyor.

![Undecided](/assets/images/2009/smiley-undecided.gif)

Gelin olayı basit bir örnek ile ele almaya çalışalım. İşte Visual Studio 2010 Professional Beta 1 üzerinde geliştirdiğim Console uygulamasına ait kodlar.

```csharp
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SequentialPLINQ
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Product> productList = GetProductList();

            int tid= 0;
            var result1 = from product in productList.AsParallel()
                          where product.Color.StartsWith("B")
                          orderby product.ProductId
                          select new
                          {
                              Id = tid++,
                              product.ProductId,
                              product.Name,
                              product.ListPrice,
                              product.Color
                          };
                         

            foreach (var r in result1)
            {
                Console.WriteLine(r.Id+" \t"+r.ProductId+" "+r.Name+" "+r.Color);
            }
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
                    ProductId=Convert.ToInt32(reader[0]),
                    Name=reader[1].ToString(),
                    ListPrice=Convert.ToDecimal(reader[2]),
                    ProductNumber=reader[3].ToString(),
                    Color=reader[4].ToString(),
                    SafetyStockLevel=Convert.ToInt32(reader[5])
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
        public string ProductNumber{ get; set; }
        public string Color { get; set; }
        public int SafetyStockLevel { get; set; }
    }
}
```

Örnekte SQL Server 2008 üzerinde kurulu olan, AdventureWorks2008 veritabanındaki Production şemasında yer alan Product tablosuna ait veriler kullanılmaktadır. Product tablosunun kod içerisindeki temsili için, Product isimli bir sınıf tasarlanmıştır. Sınıfa ait nesne örneklerinden oluşan generic List koleksiyonunun doldurulması için GetProductList metodundan yararlanılmaktadır. Söz konusu generic liste PLINQ ifadesi yardımıylada sorgulanmaktadır. Buraya kadarki kısımda zaten ilginç bir şey yok. Dikkat edeceğimiz nokta, isimsiz tip (Anonymous Type) içerisinde tid isimli sayacın arttırılmasıdır.

![Wink](/assets/images/2009/smiley-wink.gif)

Öyleki uygulamayı çalıştırdığımızda aşağıdaki sonuçları elde ederiz.

![blg21_1.gif](/assets/images/2009/blg21_1.gif)

Burada dikkat edilmesi gereken nokta tid değerlerinin ardışık mantığa göre değil, paralel çalışmanın bir sonucu olarak farklı sıralarda üretilmesidir. Bu nedenle 1, 3, 5, 8, 74... gibi bir dizi oluşmuştur. Bu dizi kodun her çalıştırılmasında farklı şekillerde üretilebilir. Örneği ikinci kez çalıştırdığımda bu kez aşağıdaki sonuçları aldım.

![blg21_2.gif](/assets/images/2009/blg21_2.gif)

Görüldüğü üzere tid değerlerinin arttırımının, çıktıya yansıyışı farklı olmaktadır. Hatta MSDN kaynaklarında, arttırımı yapılan değerlerin tekrar etmesininde mümkün olabileceği berlitilmektedir. İşte yazmış olduğum bu anlamsız örnekteki gibi (örneğin arttırım işlemlerinin ardışık bir şekilde gerçekleştirilmesinin gerektiği vb...), paralel olarak çalışan LINQ sorguları içerisinde, ardışık çalışması gereken bölümler var ise, AsSequential genişletme metodunun (Extension Method) kullanılması gerekmektedir. Buna göre yukarıdaki örnekte yer alan LINQ sorgusunu,

```csharp
var result1 = productList
                .AsParallel()
                .Where(p => p.Color.StartsWith("B"))
                .OrderBy(p => p.Name)
                .AsSequential()
                .Select(
                p => new
            {
                Id = tid++,
                p.ProductId,
                p.Name,
                p.ListPrice,
                p.Color
            });
```

şeklinde değiştirir ve örneği tekrar çalıştırırsak aşağıdaki sonuçları elde ederiz.

![blg21_3.gif](/assets/images/2009/blg21_3.gif)

Görüldüğü gibi tid arttırımı düzenli bir şekilde çıktıya yansıtılmıştır. (Yinede dikkatlice baktığınızda Name özelliğine göre yapılan sıralamalarda, her iki LINQ sorgusu arasında küçük farklar olabileceğini görebilirsiniz)

Sonuç olarak paralel olarak çalıştırılan LINQ sorguları içerisinde, ardışık yürütülmesi gereken operasyonlar var ise bu durumda AsSequential genişletme metodnun kullanılması gerekmektedir. Diğer taraftan elbetteki bu kullanım, söz konusu paralel çalışmanın hızını düşürecek bir etkiye neden olacaktır. Bu da gözden kaçırılmaması gereken diğer bir noktadır.

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[SequentialPLINQ.rar (25,80 kb)](/assets/files/2009/SequentialPLINQ.rar)