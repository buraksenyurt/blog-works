---
title: "PLINQ (Parallel LINQ) - Hello World [Beta 1]"
pubDate: 2009-05-21 19:11:00
categories:
  - LINQ
  - PLINQ
tags:
  - LINQ
  - PLINQ
  - C#
  - .NET
  - XML
  - HTTP
  - Task Parallel Library
  - Threading
  - Concurrency
  - Generics
  - Visual Studio
---

# PLINQ (Parallel LINQ) - Hello World [Beta 1]
Merhaba Arkadaşlar,

Bildiğiniz gibi son yazımı deniz kenarında bir kafede tatildeyken yazmıştım

![Wink](images/smiley-wink.gif)

Ama tatil bitti malesef ve tekrardan Morpheus'un sözleri kulaklarımda çınladı "Wellcome to the real world".

![Cry](images/smiley-cry.gif)

Yinede 1 haftalığınada olsa tatil yapabildiğime şükrediyorum. Gerçek dünyaya döndükten sonra tabiki bir süre adaptasyon sorunları ile karşılaşıyor insan doğal olaraktan. Bu adaptasyon sorunları içerisinde boğuşurken, neleri araştırabilirim diye düşünürken buluverdim kendimi.

Herşeyden önce.Net Framework 4.0 ve [Visual Studio 2010 Beta 1](http://www.microsoft.com/downloads/details.aspx?FamilyID=255fc5f1-15af-4fe7-be4d-263a2621144b&displaylang=en) sürümlerinin yayınlandığını hepimiz biliyoruz. Dolayısıyla odaklanılacak konu zaten ap açık ortadaydı..Net Framework 4.0 içerisinde entegre olarak gelen bir yenilik hemen ilgi odağım oldu. PLINQ (Parallel Language INtegrated Query). Aslında PLINQ yeni çıkmış bir eklenti değil. Zaten uzun süredir.Net Framework 3.5 ve Visual Studio 2008 üzerinde CTP sürümü ile testlerimizi yapabiliyorduk. Ne varki,.Net Framework 4.0 göz önüne alındığında PLINQ ile ilişkili tiplerin System.Core.dll assembly'ının 4.0 versiyonu içerisine doğrudan ilave edildiğini görüyoruz. Aşağıdaki Visual Studio 2010 Object Browser'dan alınan görüntüde bu durum açık bir şekilde gözlemlenebiliyor.

[![blg18_1mini.gif](images/blg18_1mini.gif)](images/blg18_1big.gif)

Tabiki öncelikli olarak PLINQ kavramından biraz bahsetmemizde yarar var. PLINQ aslında, Microsfot Research ve CLR (Common Language Runtime) takımları tarafından ortaklaşa geliştirilen Parallel Extensions isimli genişletmelerin sadece bir paçasıdır. Diğer parça ise TBL (Task Parallel Library) dir.(Bunu ilerleyen yazılarımda ele almaya çalışacağım) Her iki yapının kullanım amacı, Yönetimli Kod (Managed Code) tarafındaki eş zamanlı işleyişlerin kolay bir şekilde sağlanmasıdır. Söz konusu yapı PLINQ olunca haliyle, LINQ sorgularının kendi içerisine parçalanarak farklı thread'lerde çalışması ve bu parçaların paralel yürüyerek sonuçların elde edilmesi akla gelmektedir. Gerçektende PLINQ yapısının temel amacı bu şekilde özetlenebilir. Hatta PLINQ için Eş Zamanlı Sorgu Yürütme Motorudur (Concurrency Query Execution Engine) diyebiliriz. PLINQ temel olarak LINQ to XML ve LINQ to Objects gibi uygulama alanları üzerinde etkin bir şekilde kullanılabilmektedir.

> Unutulmaması gereken noktalardan biriside, PLINQ ifadelerinin aslında çift çekirdek ve üstü işlemcilerin yada birden fazla işlemcinin olduğu sistemlerde anlamlı olmasıdır. Nitekim, PLINQ motoru, çalışmakta olan sorgu sürecini, makinenin sahip olduğu çekirdek sayısına göre parçalara ayırır ve yürütür. Bu özellikle büyük çaplı projeler göz önüne alındığında, şirketin sahip olduğu kaç bilgisayar var ise hepsini en azından çift çekirdekli olacak şekilde yenilemek gibi bir maliyet anlamına da gelmemelidir.
> Nitekim bazı istemci-sunucu mimarilerinde, sunucu tarafında çalışmakta olan pek çok LINQ sorgusu, PLINQ motoru kullanılaraktan daha efektif hale getirilebilir. Bir başka deyişle, istemciler birden fazla çekirdekli işlemcilere sahip olmasalarda, mümkün mertebe LINQ ifadelerini içeren iş mantıklarının, sunucu tarafında olduğu senaryolarda PLINQ büyük avantajlar sağlayabilir (Çok kısa bir süre önce çalışmakta olduğum bir projede yer alan test makinesinin, 8 işlemcili olduğunu hatırlıyorum
>
> ![Laughing](images/smiley-laughing.gif)
>
> )

Tabi burada var olan nesneler üzerindeki LINQ sorgularının paralel olarak çalıştırılması için, Select, Where gibi genişletme metodlarının (Extension Methods) çalışma sırasında işi farklı parçalara bölebilecek versiyonlarının olması gerektiği düşünülebilir. İşte bu noktada devreye, System.Core assembly'ının 4.0 versiyonu içerisinde yer alan ve System.Linq isim alanında bulunan ParallelEnumerable adlı static sınıf girmektedir.

![blg18_2.gif](images/blg18_2.gif)

Bu sınıftaki en önemli genişletme AsParallel isimli fonksiyondur. Bu metodun görevi, IEnumerable türevli bir koleksiyonun paralel olarak sorgulanabilir hale getirilmesi veya hazırlanmasıdır. Öyleki, metod geriye ParallelQuery isimli sınıfa ait bir nesne örneği döndürmektedir. ParallelQuery sınıfı IEnumerable arayüzünü uygulamaktadır ama herşeyden önemlisi paralel sorgulanabilme için gerekli ön hazırlıkları içeren operasyonlarada sahiptir.

Bu teknik detaylar eminimki bir Hello World yazısında sizede sıkıcı gelmiştir. Hiç vakit kaybetmeden basit bir örnek ile ilerlemekte yarar olduğunu düşünmekteyim. Aşağıdaki kod parçası Visual Studio 2010 Beta 1 sürümünde yazılmış basit bir Console uygulamasına aittir.

```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            Stopwatch watch = Stopwatch.StartNew();

            var result1 = from p in products
                          where p.ListPrice >= 10 && p.InStock == true
                          orderby p.Name descending
                          select p;
            Console.WriteLine("Toplam {0} adet ürün bulundu",result1.ToList().Count.ToString());

            Console.WriteLine("Toplam süre {0}",watch.ElapsedMilliseconds.ToString());
            Console.WriteLine("Parallel Olduğunda");

            Stopwatch watch2 = Stopwatch.StartNew();

            var result2 = from p in products.AsParallel()
                          where p.ListPrice >= 10 && p.InStock==true
                          orderby p.Name descending
                          select p;
            Console.WriteLine("Toplam {0} adet ürün bulundu", result2.ToList().Count.ToString());

            Console.WriteLine("Toplam süre {0}", watch2.ElapsedMilliseconds.ToString());
        }

        static List<Product> FillProducts()
        {
            List<Product> products = new List<Product>();

            for (long i = 1; i < 1750000; i++)
            {
                Product prd = new Product { 
                    Id = i
                    , Name = "Product" + i.ToString()
                    , ListPrice = i * 0.1M
                    , InStock=i%2==0?true:false
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

Uygulama içerisinde Products isimli bir sınıf ve bu tipe ait nesne örneklerinden oluşan bir koleksiyon veri kaynağı olarak kullanılmaktadır. Dikkat edileceği üzere, iki adet LINQ sorgusu bulunmaktadır. Product tipinden olan generic List koleksiyonu, FillProducts metodu yardımıyla tamamen hayali veriler ile doldurulmuştur. Her iki sorguda ListPrice değeri 10' un üzerinde olan ve stokta bulunan ürünleri, adlarına göre ters sırada döndürmektedir. Ancak önemli olan nokta ikinci LINQ ifadesinde AsParallel metodunun kullanılmasıdır. Bu örnek kod parçasını çalıştırdığımda, aşağıdaki ekran görüntüsünde yer alan sonuçları aldım.

![blg18_3.gif](images/blg18_3.gif)

Hemen şunu belirteyim. Programı yazdığım makinede çift çekirdekli Intel işlemci ve 4 Gb Ram bulunmakta. İşletim sistemi olarakta Windows Vista Enterprise yer alıyor. Tabi bu örnek için Intel tabanlı işlemcinin daha büyük önem taşıdığını hemen söyleyebiliriz. Çalışma zamanındanda görüldüğü gibi, paralel olarak yürütülen LINQ ifadesi neredeyse %50 daha az zamanda tamamlanmıştır. (Aslında bu kod parçasını 4 çekirdekli bir işlemcide test etmeyi çok istiyorum. Bu konuda siz değerli okurlarımın yorumlarını ve test sonuçlarını bekliyor olacağım

![Wink](images/smiley-wink.gif)

)

Uygulama çalışırken Task Manager aracı ile CPU kullanım durumuna baktığımda ise aşağıdaki sonuçlar ile karşılaştım.

![blg18_4.gif](images/blg18_4.gif)

Bu ekran görüntüsünde yer alan sonuçlar tam anlamıyla durmun net analizi olmasada bir parça olsun fikir vermektedir. Yuvarlak içerisine aldığımda kısımlar, sorgunun PLINQ motoru tarafından ele alınmaya başladığı yerlerdeki ölçüm değerleridir. Dikkat edileceği üzere CPU çekirdeklerinin kullanım değerleri %100' e vurmuş durumdadır ki buda aslında, sorgunun çalıştırılması sırasında tüm işlemci gücünün kullanıldığı anlamınada gelmektedir. (Aslında Einstein'ın kuramına göre bu durum göreceli olarak iyi sayılabilir. Ama sayılmayadabilir

![Undecided](images/smiley-undecided.gif)

)

Yazdığım örnek kod parçasında işlemleri gerçekten yavaşlatmak adına bir sıralama işlemide kullandım. Anacak bunun yapılması zorunlu değildir. Özellikle sıralama kullanılmadığında sorgu çalıştırma sürelerinin birbirlerine çok yakın olduğunu gördüm. Açıkçası, PLINQ'in avantajı gerçekten çok uzun sürebilecek sorgular söz konusu olduğunda ortaya çıkmata. Bu nedenle her LINQ sorgusunun PLINQ formatına dönüştürülmesininde anlamlı olmadığını (olmayacağını) söyleyebiliriz. Nitekim, bazı durumlarda herşey tersine dönebilir. Örneğin aşağıdaki kod parçasını göz önüne alalım.

```csharp
int[] values = new int[100];Random rnd = new Random();
for (int i = 1; i < values.Length; i++)
{
 values[i] = rnd.Next(1, 1000);
}

Stopwatch watch3 = Stopwatch.StartNew();
var result3 = from value in values
              where value % 2 == 0
              select value;
Console.WriteLine("Toplam {0} çift sayı var",result3.ToList().Count.ToString());
Console.WriteLine(watch3.ElapsedMilliseconds.ToString());

Stopwatch watch4 = Stopwatch.StartNew();
var result4 = from value in values.AsParallel()
              where value % 2 == 0
              select value;
Console.WriteLine("Toplam {0} çift sayı var", result4.ToList().Count.ToString());
Console.WriteLine(watch4.ElapsedMilliseconds.ToString());
```

Bu kod parçasındaki LINQ sorgularında, 100 tane raslantısal ve 1 ile 1000 arasında olan tamsayı değerlerinden oluşan bir dizi içerisinde kaç çift sayı olduğu tespit edilmektedir. İkinci sorgu, PLINQ motoru tarafından ele alınmaktadır. PLINQ'in, sorguyu paralel olan iş parçalarına bölerek çalıştırdığı düşünüldüğünde, ikinci ifadenin birincisine göre çok daha hızlı çalışması gerektiği tahmin edilebilir. Ama gerçekten böylemi olacaktır. İşte sonuçlar...

![blg18_5.gif](images/blg18_5.gif)

Aslında bu sonuç son derece doğaldır. PLINQ motoru çalışma zamanında, çekirdeklere bölünecek işler için hazırlıklar yapmalıdır, thread'leri ayarlamalıdır vb... Bu ön hazırlıklar nedeni ile zaten sorgunun sürece girmesi başlı başına bir zaman kaybı anlamına gelmektedir. Bu örnek en basit anlamda, PLINQ'in her LINQ ifadesi için ele alınmaması gerektiğinide göstermektedir.

Böylece geldik bir yazımızın daha sonuna. Tatil dönüşü sonrası üzerimdeki adaptasyon bıkkınlığını hafifleten bu yazımda sizlere,.Net Framework 4.0 içerisinde artık standart olarak yer alan ve Parallel Extension mimarisinin bir parçası olan PLINQ konusunu anlatmaya çalıştım. Elbetteki PLINQ içerisindede çok daha fazlası var. Bunlarıda ilerleyen yazılarımda aktarmaya çalışıyor olacağım. Sizlerde [bu](http://blogs.msdn.com/pfxteam/default.aspx)adresten Parallel Extension ile ilişkili son bilgileri alabilirsiniz. Hatta şu saatlerde VS 2010 ile gelen yeniliklerde anlatılmakta. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[HelloWorld.rar (21,81 kb)](assets/HelloWorld.rar)
