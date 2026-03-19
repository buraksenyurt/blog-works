---
title: "for mu, foreach mi? Yoksa Parallel.For mu, Parallel.ForEach mi? [Beta 1]"
pubDate: 2009-06-09 16:51:00
categories:
  - TPL
tags:
  - TPL
  - C#
  - .NET
  - Parallel Programming
  - Concurrency
  - Visual Studio
---

# for mu, foreach mi? Yoksa Parallel.For mu, Parallel.ForEach mi? [Beta 1]
Merhaba Arkadaşlar,

Gecenin bu saatinde uyuyamayıp blog'uma bir şeyler yazmak isteyişimin sebebi, bu gün bir okurumdan gelen şu sorudur; "Madem Parallel.For veya Parallel.ForEach ile herşey daha hızlı oluyor, niye normal for ve foreach döngülerini bu formasyona sokmuyorlarda ek bir şeyler ilave ediyorlar". Dolayısıyla klavyemi elime aldım ve hemen bir test programı yazmaya koyuldum. Bu kez amaç vaat edilenin tersini göstermekti. Yani performansa ve hıza ulaşmaya çalışmayacak, tam aksi yöne gitmeye gayret edecektim. Aslında bu işlemler için gayet profesyonel test araçları mevcuttur. Ancak bir araca gerek duymadanda analizimizi yapabiliriz. İşe Visual Studio 2010 Beta 1 üzerinde, basit bir Console örneği geliştirerek başladım. İşte kodlarımız;

```csharp
using System;
using System.Diagnostics;
using System.Threading;

namespace ForForEachPerformance
{
    class Program
    {
        static void Main(string[] args)
        {
            int arraySize = 1000;
            double[] array1 = new double[arraySize];
            
            Random rnd=new Random();
            Stopwatch watch1 = Stopwatch.StartNew();

            for (int i = 0; i < array1.Length; i++)
            {
                array1[i] = (rnd.NextDouble()/Math.Cos(rnd.NextDouble()))*Math.Sqrt(rnd.NextDouble());
            }

            Console.WriteLine("For döngüsü eleman ekleme süresi {0} milisaniyedir.",watch1.Elapsed.TotalMilliseconds.ToString());

            double[] array2 = new double[arraySize];

            Stopwatch watch2 = Stopwatch.StartNew();
            Parallel.For(0, array2.Length, i =>
                {
                    array1[i] = (rnd.NextDouble() / Math.Cos(rnd.NextDouble())) * Math.Sqrt(rnd.NextDouble());
                }
            );

            Console.WriteLine("Parallel.For döngüsü eleman ekleme süresi {0} milisaniyedir.", watch2.Elapsed.TotalMilliseconds.ToString());

            Stopwatch watch3 = Stopwatch.StartNew();

            for (int i = 0; i < array1.Length; i++)
            {
                double d = array1[i];
            }

            Console.WriteLine("For döngüsü eleman okuma süresi {0} milisaniyedir.", watch3.Elapsed.TotalMilliseconds.ToString());

            Stopwatch watch4 = Stopwatch.StartNew();

            Parallel.For(0, array1.Length, i =>
                {
                    double d = array1[i];
                }
            );

            Console.WriteLine("Parallel.For döngüsü eleman okuma süresi {0} milisaniyedir.", watch4.Elapsed.TotalMilliseconds.ToString());

            Stopwatch watch5 = Stopwatch.StartNew();

            Parallel.ForEach(array1, i =>
                {
                    double d = i;
                }
            );

            Console.WriteLine("Parallel.ForEach döngüsü eleman okuma süresi {0} milisaniyedir.", watch5.Elapsed.TotalMilliseconds.ToString());

            Stopwatch watch6 = Stopwatch.StartNew();

            foreach (double i in array1)
            {
                double d = i;
            }

            Console.WriteLine("ForEach döngüsü eleman okuma süresi {0} milisaniyedir.", watch6.Elapsed.TotalMilliseconds.ToString());
            Console.ReadLine();
        }
    }
}
```

Aslında kodumuz son derece basit. Eleman sayısını 1000 olarak set ettiğimiz double tipinden dizilere eleman eklemek (ki eklerken işin uzun sürmesini sağlamak adına tamamen anlamsız bir matematik formülü içermektedir) ve okumak için for, foreach, Parallel.For ile Parallel.ForEach metodlarını kullanmaktayız. Burada eleman sayısının bilhakis düşük tutulması son derece önemlidir aslında. Program çalıştırıldığında, for ve Parallel.For ile yapılan ekleme işlemleri ile, yine for, foreach ve Parallel.ForEach ile yapılan okuma işlemlerine ait toplam süre değerlerini bildirmektedir. Ben arka arkaya 10 deneme yaptıktan sonra ekleme işlemleri için aşağıdaki sonuçları elde ettim.

Eleman Ekleme

Deneme
For ile
Parallel.For ile

1
0,2592
4,6796

2
0,2718
3,8415

3
0,2799
3,913

4
0,2732
5,7423

5
0,2584
4,3159

6
0,291
3,8208

7
0,2782
3,9725

8
0,2612
4,041

9
0,2626
3,9412

10
0,2598
8,0166

Grafiksel olarak bakarsak çok daha acı bir gerçekle karşılabiliriz.

![blg30_1.gif](images/blg30_1.gif)

Görüldüğü üzere for ile gerçekleştirilen ekleme işlemi, eş zamanlı ve dolayısıyla paralel çalışabilen Parallel.For kullanımına göre çok daha hızlı yapılmıştır. Peki diziden veri okuma işlemi sırasındaki durum nedir? İşte sonuçlar;

Eleman Okuma

Deneme
For ile
Parallel.For
For Each
Parallel.ForEach

1
0,0128
0,6894
0,0145
23,1096

2
0,0125
0,8129
0,0134
26,3994

3
0,0128
0,7707
0,0139
25,152

4
0,0131
0,6062
0,0136
25,661

5
0,0128
0,8048
0,0139
25,6406

6
0,0131
0,842
0,0153
35,9978

7
0,0125
0,5439
0,0131
24,9442

8
0,0134
1,3035
0,0134
23,9636

9
0,0125
0,7635
0,0136
24,2947

10
0,0128
14,211
0,0142
37,5547

Okuma işlemlerinin grafiksel sonucu ise aşağıdaki şekilde görüldüğü gibidir.

![blg30_2.gif](images/blg30_2.gif)

Her ne kadar for süreleri grafik üzerinde görünmesede (sıfıra çok yakın olduğu için) en hızlı okuma süresi rekoru kendisine aittir. Sonrasında foreach gelmektedir. Parallel.For nispeten belirli süre foreach'e yakın değerlerde seyretmesine rağmen, 10ncu denemede açılan paralel task'lerin canından bezmesi nedeni ile çok kötü bir süre üretmiştir. Ancak Parallel.ForEach bu teste göre sondan birinci olmuştur.

Bu testleri Dual Core işlemcili e 4 Gb Ram'i olan, Vista yüklü bir sistem üzerinde denediğimde elde ettim. Elbetteki paralel tekniklerin burada kötü sonuçlar vermesinin en büyük nedeni işlemlerin zaten normal for veya foreach ' ler ile çok kısa sürede tamamlanabilmesidir. Öyleki, bu süreler içerisinde işleyişi paralel iş parçalarına ayırmak için yapılacak tüm hazırlıklar, sürenin dahada uzamasına neden olmaktadır. Sonuç olarak şu noktayı vurgulamak gerekiyor,

Parallel.For ve Parallel.ForEach metodları, döngü içerisindeki işlemlerin gerçekten uzun sürelerde yapılabilldiği durumlarda kullanılmalıdır. Bu noktada kodun çalışacağı sistemin kapasitesi veya döngüler içerisinde yer alan işlemlerin maliyeti gibi pek çok etken, tamamlanma sürelerinde belirleyici rol oynamaktadır. Söz gelimi grafik tabanlı matematiksel hesaplamaların çok sayıda nesne örneği için yapılması gereken durumlarda (DirectX, OpenGL kullanan grafik uygulamaları veya oyunlar) paralel tekniklerden yararlanılması düşünülebilir.

Bir diğer önemli noktada aslında, Parallel.For, Parallel.ForEach, Task, Parallel.Invoke gibi kavramların paralel programlama genişletmesi (Parallel Extensions) olaraktan henüz beta aşamasında yer alan bir ürüne dahil edilmiş olmalarıdır ki.Net Framework 3.5 ilede ek bir paket yüklenerek ele alınabilmektedir. Bu açıdan bakıldığında Relase sürümde farklılıklar olması veya arzu edilen iyileştirmelerin yapılmasıda muhtemeldir.

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[ForForEachPerformance.rar (21,64 kb)](assets/ForForEachPerformance.rar)
