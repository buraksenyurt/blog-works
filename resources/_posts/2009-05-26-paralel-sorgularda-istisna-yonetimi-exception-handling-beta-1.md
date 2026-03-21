---
layout: post
title: "Paralel Sorgularda İstisna Yönetimi(Exception Handling) [Beta 1]"
date: 2009-05-26 05:30:00 +0300
categories:
  - linq
  - plinq
tags:
  - plinq
  - language-integrated-query
---
Yönetimli kod (Managed Code) tarafında istisna yönetimi oldukça önemli konulardan birisidir. Uygulamaların veya kod süreçlerinin istem dışı sonlanmasının önüne geçilmek istendiği durumlarda, basit try...catch...finally bloklarından yararlanabilir yada Enterprise Library gibi kütüphanelerin sunduğu bloklardan faydalanarak istisna yönetimini üst seviyede sağlayabiliriz.

Bu yazımda çok geniş kapsamda düşünmeyip, PLINQ (Parallel Language INtegrated Query) ifadelerinde oluşabilecek istisnai durumların nasıl ele alınması gerektiği üzerinde durmaya çalışacağız. Olaya hızlı bir giriş yapıp aşağıdaki örnek kod parçasına sahip olduğumuzu düşünelim.

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
            productList[497].SafetyStockLevel = 0;
            productList[503].SafetyStockLevel = 0;

            var result1 = from product in productList.AsParallel()                          
                          orderby product.ProductId
                          where product.ListPrice>=500
                          select new
                          {
                              product.ProductId,
                              product.Name,
                              product.ListPrice,
                              product.Color,
                              SellPrice=FindSellPrice(product.ListPrice,product.SafetyStockLevel)    // İlk durum                          
                          };

            try
            {
                foreach (var r in result1)
                {
                    Console.WriteLine(r.ProductId + " " + r.Name + " (" + r.SellPrice.ToString("C2") + ")");
                }
            }
            catch (AggregateException excp) // Hata oluştuğunda PLINQ içerisinde başlatılan tüm alt işlemler(Threads) iptal edilir
            {
                // PLINQ ifadeleri çalıştırıldığı sırada oluşan istisnalar AggreateException nesne örneği içerisindeki InnerExceptions özelliğinin refere ettiği koleksiyonda toplanırlar.
                foreach (Exception error in excp.InnerExceptions)
                {
                    Console.WriteLine(error.Message);
                }
            }
        }
           static decimal FindSellPrice(decimal listPrice,int stockLevel)
        {
            decimal result=-1;
                if (DateTime.Now.Day >= 25
                    && DateTime.Now.Day <= 28)
                    result=listPrice - (listPrice * (1 / stockLevel)); // SafetyStockLevel' ın 0 gelmesi halinde exception oluşacak olan yer.
                else
                    result= listPrice * 1.18M;
            return result;
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

Visual Studio 2010 Professional Beta 1 ile geliştirilen bu kod parçasında, Product isimli bir sınıftan yararlanılmaktadır. Product tipine ait veriler, SQL sunucusu üzerindeki Product tablosundan alındıktan sonra, generic List koleksiyonu içerisinde tutulmakta ve sonrasında ise paralel sorgulamaya tabi tutulmaktadır. Burada ayrıca dikkat edilmesi gereken bir noktada, sorgulama sırasında FindSellPrice isimli metodun çağırılması ve parametre olarak, sorgunun t anındaki Product nesnesine ait ListPrice ile StockLevel değerlerinin gönderilmesidir. Dikkat edileceği üzere FindSellPrice metodu içerisinde güne göre ürünlerde indirim uygulanmasını hedef alan bir formül yer almaktadır. Formülü tamamen kafadan uydurduğumu ifirat etmek isterim. Zaten sizde bunu anlamışsınızdır.

![Wink](/assets/images/2009/smiley-wink.gif)

Asıl varmak istediğim nokta, StockLevel değerlerinin 497 ve 503 nolu ürünler için bilinçli olarak sıfıra set edilmiş olmasıdır. Bu nedenle bölme işlemi sırasında bir istisna oluşması kaçınılmazdır.

(Yani kendi kendimize kaşınıp kod içerisine bir bubi tuzağı koymuş durumdayız. ![blg22_3.jpg](/assets/images/2009/blg22_3.jpg))

Önemli olan nokta, paralel sorgu motorunun bu tip bir durum ile karşılaştığında ne yapacağıdır. Nitekim söz konusu sorgulama tekniğine göre, operasyon bir kaç parça Thread'e bölünmete ve bu nedenle oluşacak bir istisnada (veya istisnalarda) çalışan iş parçalarına ne olacağı sorusu akla gelmektedir. İşte kodun yukarıdaki halinin çalışması sonrası ekran görüntümüz.

![blg22_1.gif](/assets/images/2009/blg22_1.gif)

Görüldüğü gibi hiç bir Product bilgisi ekrana çıktı olarak gelmemiştir. Bu son derece doğaldır. Nitekim paralel sorgulama motoru herhangibir istisna ile karşılaştığında, çalışmakta olan tüm Thread'lerin iptal edilmesini sağlamaktadır. Diğer yandan istisna nesnesinin tipine dikkat edilmelidir. PLINQ ifadeleri içerisinde meydana gelebilecek istisnalar, AggregateException istisna sınıfı tarafından sarmalanmaktadır. Birden fazla istisna olabileceğinden, AggregateException sınıfı InnerExceptions isimli birde özelliğe sahiptir. Dolayısıyla catch bloğu içerisinde, sıfıra bölem hatasının yakalanması için, InnerExceptions koleksiyonunda dolaşılması gerekemtekdir. (InnerExceptions özelliği ReadOnlyCollection tipinden bir koleksiyon döndürmektedir.)

Peki ya, istisna olan parçaların atlanması (bu örneğe göre) istenirse. Bir başka deyişle istisna almayan parçaların yinede paralel sorgulama sonucu ele alınması istenirse ne yapabiliriz?

Bu sorunun cevabı son derece basittir aslında. Exception yönetimi, FindSellPrice isimli metod içerisinde gerçekleştirilir.

```csharp
static decimal FindSellPrice(decimal listPrice,int stockLevel)
{
    decimal result=-1;
    try 
    {
    if (DateTime.Now.Day >= 25 && DateTime.Now.Day <= 28)
       result=listPrice - (listPrice * (1 / stockLevel)); // SafetyStockLevel' ın 0 gelmesi halinde exception oluşacak olan yer.
    else
       result= listPrice * 1.18M;
    }
    catch(DivideByZeroException excp)
    {
      Console.WriteLine("\tStok miktarı 0 olduğundan satış fiyatı hesaplanamadı");
    }
    return result;
}
```

Bu durumda uygulama başarılı bir şekilde çalışacak ve aşağıdaki ekran görüntüsüne benzer sonuçlar alınacaktır.

![blg22_2.gif](/assets/images/2009/blg22_2.gif)

Görüldüğü gibi istisnaya neden olan ürünler kontrollü bir şekilde elenmiş ve sorgunun paralel olarak yürütülmesine devam edilebilmiştir. Böylece geldik bir yazımızın daha sonuna. Bu yazımızda PLINQ sorgularında, istisna yönetiminde nelere dikkat etmemiz gerektiğine değinmeye çalıştık. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[ExceptionHandling.rar (27,33 kb)](/assets/files/2009/ExceptionHandling.rar)