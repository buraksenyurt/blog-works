---
layout: post
title: "Concurrent Collections (Eş Zamanlı Koleksiyonlar) [Beta 1]"
date: 2009-06-12 10:20:00 +0300
categories:
  - parallel-programming
tags:
  - parallel-programming
---
.Net Framework 4.0 ve içerdiği paralel genişletmeler (Parallel Extensions) ile birlikte gelmekte olan yenilikler arasında, eş zamanlı (Concurrent) çalışabilen ve Thread Safe olan koleksiyonlarda bulunmaktadır. Bu koleksiyonlar aslında veri yapıları (Data Structures) ile birlikte gelen yeni tipler arasında yer almaktadır.

![blg31_1.gif](/assets/images/2009/blg31_1.gif)

Geçtiğimiz günlerde çok şanslı bir insan olarak hafta sonumu bir tatil beldesinde geçirirken, bu kez gecenin derin sessizliğinde araştırmaya başladığım konulardan biriside işte bu yeni koleksiyonlar oldu. Bu koleksiyon tipleri elbetteki relase sürümünde değişikliğe uğrayabilir.

![Wink](/assets/images/2009/smiley-wink.gif)

Söz konusu koleksiyon tipleri esasında System.Collections.Concurrent isim alanı altındadır. Ancak bu isim alanı System ve Mscorlib olmak üzere iki assembly içerisine aşağıdaki şekilde görüldüğü gibi dağılmıştır.

![blg31_2.gif](/assets/images/2009/blg31_2.gif)

Visual Studio 2010 Beta 1 üzerindeki object browser yardımıyla söz konusu tiplere baktığımda bana tanıdık gelebilecek olanlar sadece ConcurrentDictionary, ConcurrentQueue ve ConcurrentStack koleksiyonlarıydı. Nitekim bu tipler daha önceki.Net sürümlerinden bildiğimiz Dictionary, Queue ve Stack koleksiyonlarının eş zamanlı çalışabilen versiyonlarıydı. Ancak kafamda iki önemli soru bulunmaktaydı. Bir; diğer koleksiyon tipleri nasıl ve hangi amaçlar ile kullanılmaktaydı ve iki; koleksiyonların eş zamanlı olmasının ne anlamı vardı

![Smile](/assets/images/2009/smiley-smile.gif)

Paralel genişletme ile gelen koleksiyonların ataları çoğunlukla Thread Safe yapıda değildir. Bu nedenle geliştiricinin Thread Safe yapısını sağlaması gerektiği durumlarda kolları sıvaması ve kilitleme mekanizmalarını bilinçli olarak kullanması gerekmektedir. Bir başka deyişle, koleksiyon içerisine dahil edilen elemanlar üzerinde bir iterasyon yapıldığında, başka Thread'ler üzerinden aynı koleksiyonun elemanlarına ulaşmak güvenli değildir. Bu nedenle örneğin bir koleksiyonun elemanları dolaşılırken belirli kriterlere göre aynı koleksiyondan eleman çıkartılmasıda mümkün değildir.(Ki bu durumda geliştiricilerin multi-thread yapıları içerisinde ele alınan koleksiyonlar için senkronizasyon tekniklerini kullanarak sorunu çözmesi gerekmektedir) Hatta aşağıdaki kod parçasında olduğu gibi bir koleksiyonun üyelerinin dolaşılması sırasında,

```csharp
static void Main(string[] args)
{
 Dictionary<int, string> numbers = new Dictionary<int, string>
 {
  {1,"Bir"},
  {2,"İki"},
  {3,"Üç"},
  {4,"Dört"},
  {5,"Beş"},
  {6,"Altı"}
 };
 
 foreach (KeyValuePair<int,string> number in numbers)
 {
  numbers.Remove(number.Key);
  Console.WriteLine("{0} çıkartıldı.",number.Key);
 }
}
```

eleman çıkartma işlemi gerçekleştirildiğinde çalışma zamanında aşağıdaki ekran görüntüsünde yer alan InvalidOperationException istisnasını almamız kaçınılmazdır.

![blg31_3.gif](/assets/images/2009/blg31_3.gif)

Görüldüğü gibi ilk eleman çıkartıldıktan sonra koleksiyonun boyutu değiştiğinden InvalidOperationException istisnasının fırlatılması söz konusu olmuştur. Oysaki Dictionary koleksiyonu yerine Concurrent versiyonu kullanılsaydı Thread Safe kuralları çerçevesinde herhangibir sorun ile karşılaşılmazdı. Aşağıdaki kod parçasında bu duruma ait bir kod parçası görülmektedir.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace BlockingCollection
{
    class Program
    {
        static void Main(string[] args)
        {           
            #region Concurrent versiyonu

            ConcurrentDictionary<int, string> numbers = new ConcurrentDictionary<int, string>();
            numbers.TryAdd(1, "Bir");
            numbers.TryAdd(2, "İki");
            numbers.TryAdd(3, "Üç");
            numbers.TryAdd(4, "Dört");
            numbers.TryAdd(5, "Beş");
            numbers.TryAdd(6, "Altı");

            foreach (KeyValuePair<int,string> number in numbers)
            {
                string value;
                bool result=numbers.TryRemove(number.Key, out value);
                if(result)
                    Console.WriteLine("{0} çıkartıldı.",value);
            }

            #endregion
        }
    }
}
```

ve sonuç;

![blg31_4.gif](/assets/images/2009/blg31_4.gif)

Görüldüğü gibi koleksiyon elemanları foreach döngüsü ile gezilirken teker teker çıkartılma işlemi yapılabilmiştir. Buna göre öyle vakalar olmalıdır ki, koleksiyonları ele alan paralel süreçlerin aynı örnek üzerindeki elemanlarda Thread Safe kuralları çerçevesinde ekleme, silmve ve güncelleme gibi işlemler yapılabilmelidir. Dolayısıyla paralel genişletmelere ait veri yapılarında yer alan Concurrent koleksiyonların temel kullanım amacı belkide bu şekilde ifade edilebilir. Ben tabiki hemen diğer koleksiyonları ve kullanım amaçlarını merak etmeye başladım ve incelemeye karar verdim. Ne varki içimden bir dürtü, "bak Burakcığım, Thread Safe kolayca bertaraf edilmiş, eş zamanlı olarak aynı koleksiyon üzerinde birden fazla sürecin işlem yapabilmesi sağlanmış. Peki ya performanstan ne haber?" ![Laughing](/assets/images/2009/smiley-laughing.gif) Bu nedenle.Net 4.0 öncesi Dictionary koleksiyonu ile ConcurrentDictionary koleksiyonu arasındaki performans farklılıklarını analiz etmeye karar verdim. Aslında ilk tahminlerimin doğru çıktığını ifade edebilirim şimdiden

![Sealed](/assets/images/2009/smiley-sealed.gif)

Thread Safe + aynı anda ilerleme,ekleme, çıkartma, düzenleme yapabilme yeteneği = pahalı maliyet

İşte test programı kodları;

```csharp
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace BlockingCollection
{
    class Program
    {
        static void Main(string[] args)
        {
            //Dictionary ve ConcurrentDictionary koleksiyonları için arka arkaya 10 test yapılır
            for (int i = 0; i < 10; i++)
            {
                DictionaryTest();
                ConcurrentDictionaryTest();
                ParallelConcurrentTest();
            }
        }

        static int length = 9000000;

        // Dictionary<int,int> koleksiyonuna eleman ekleme ve okuma işlemlerini ele alır.
        static void DictionaryTest()
        {
            Stopwatch watch = Stopwatch.StartNew();

            Dictionary<int, int> collection = new Dictionary<int, int>();

            // Eleman ekleme işlemi
            Random rnd = new Random();
            for (int i = 0; i < length; i++)
            {
                collection.Add(i, rnd.Next(1, 1000000));
            }
            watch.Stop();
            Console.WriteLine("{0}",watch.Elapsed.TotalSeconds.ToString());

            // Zamanlayıcı sıfırla ve yeniden başlat.
            watch.Reset();
            watch.Start();

            // Eleman okuma işlemi
            foreach (KeyValuePair<int,int> item in collection)
            {
                int value = item.Value;
            }
            watch.Stop();
            Console.WriteLine("{0}", watch.Elapsed.TotalSeconds.ToString());
        }

        // ConcurrentDictionary<int,int> koleksiyonuna eleman ekleme ve okuma işlemlerini ele alır
        static void ConcurrentDictionaryTest()
        {
            Stopwatch watch = Stopwatch.StartNew();

            ConcurrentDictionary<int, int> collection = new ConcurrentDictionary<int, int>();

            // Eleman ekleme işlemleri
            Random rnd = new Random();
            for (int i = 0; i < length; i++)
            {
                collection.TryAdd(i, rnd.Next(1, 1000000));
            }
            watch.Stop();
            Console.WriteLine("\t{0}", watch.Elapsed.TotalSeconds.ToString());

            // Zamanlayıcıyı sıfırla ve yeniden başlat
            watch.Reset();
            watch.Start();
            // Eleman okuma işlemleri
            foreach (KeyValuePair<int, int> item in collection)
            {
                int value = item.Value;
            }
            watch.Stop();
            Console.WriteLine("\t{0}", watch.Elapsed.TotalSeconds.ToString());
        }

        // Parallel.For ve Parallel.ForEach kullanıldığında Concurrent koleksiyonun eleman ekleme ve okuma işlemlerini test eder.
        static void ParallelConcurrentTest()
        {
            Stopwatch watch = Stopwatch.StartNew();

            ConcurrentDictionary<int, int> collection = new ConcurrentDictionary<int, int>();

            // Eleman ekleme işlemleri
            Random rnd = new Random();

            // Paralel çalışan For döngüsü
            Parallel.For(0, length, i =>
            {
                collection.TryAdd(i, rnd.Next(1, 1000000));
            }
            );
            watch.Stop();
            Console.WriteLine("\t{0}", watch.Elapsed.TotalSeconds.ToString());

            // Zamanlayıcıyı sıfırla ve yeniden başlat
            watch.Reset();
            watch.Start();
            // Eleman okuma işlemleri
            // Paralel çalışan ForEach döngüsü
            Parallel.ForEach<KeyValuePair<int, int>>(collection, item =>
            {
                int value = item.Value;
            }
            );
            watch.Stop();
            Console.WriteLine("\t{0}", watch.Elapsed.TotalSeconds.ToString());
        }
    }
}
```

Uygulamamızda Dictionary ve ConcurrentDictionary tipinden iki koleksiyon 3 farklı test metodu yardımıyla ele alınmaktadır. Testler sırasında her iki koleksiyonada rastgele sayılardan oluşan 9000000 tam sayı ilave edilmektedir. Sonrasında ise doldurulan koleksiyonlar ileri yönlü bir iterasyon ile okunmaktadır. Program kodunun temel amacı, eleman ekleme ile okuma işlemlerinde, Dictionary ve ConcurrentDictionary koleksiyonlarının söz konusu işlemleri ortalama olarak ne kadar sürelerde tamamladıklarının testini yapmaktır. ParallelConcurentTest isimli metod dikkat edileceği üzere TPL (Task Parallel Library) kütüphanesinde yer alan Parallel.For ve Parallel.ForEach metodlarını kullanarak ConcurrentDictionary koleksiyonunu ele almaktadır. Ben bu programı intel tabanlı çift çekirdek işlemcili, 4 Gb Ram belleğe sahip ve Vista Enterprise işletim sistemi üzerinde koşturduğumda anlık koşullara göre aşağıdaki ekleme sürelerini tespit ettim.

Eleman Ekleme Süreleri

Deneme
Dictionary
ConcurrentDictionary
Parallel

1
1,5965157
8,6457496
9,7127165

2
1,7207327
8,6280703
8,8890291

3
1,7718992
8,6033512
9,246576

4
1,9256235
8,7227608
9,4900385

5
1,9287144
8,4039116
9,539486

6
2,0223963
8,6328307
9,6052221

7
1,9426832
10,3117767
11,4428462

8
2,0062376
10,2670853
11,3882937

9
1,9487786
9,7330822
10,8873102

10
1,8028344
10,4151047
11,3630567

Grafik olarak baktığımızda,

![blg31_5.gif](/assets/images/2009/blg31_5.gif)

ConcurrentDictionary koleksiyonu için eleman ekleme sürelerinin gerçekten çok kötü olduğu gözlemlenebilir. Hatta durumu kurtarmak adına Parallel.For ve Parallel.ForEach metodlarının kullanıldığı durumdaki zaman değerleride son derece kötüdür. Diğer yandan, oluşturulan bu koleksiyonların tüm elemanlarını ileri yönlü bir iterasyon ile dolaştığımızda aşağıdaki zaman değerlerini elde ettiğimi gördüm.

Eleman Okuma Süreleri

Deneme
Dictionary
ConcurrentDictionary
Parallel

1
0,2707316
0,5216791
0,7073974

2
0,2715216
0,4951542
0,7149783

3
0,3506021
0,5100271
0,7525682

4
0,3380284
0,4933783
0,7305076

5
0,338477
1,3850732
0,7164944

6
0,322663
0,4776662
0,7548498

7
0,2821501
0,5871846
0,8353176

8
0,3824846
0,8149798
0,8492322

9
0,305484
0,577625
0,9152573

10
0,3560983
0,5122665
0,8599752

Duruma grafiksel olarak baktığımızda,

![blg31_6.gif](/assets/images/2009/blg31_6.gif)

ConcurrentDictionary ve Dictionary arasındaki sürelerin birbirlerine yaklaştıklarını görebiliriz. Ancak ConcurrentDictionary koleksiyonu için okuma sürelerinin (işlemler paralel halde ele alınsalara dahi) yinede Dictonary koleksiyonuna göre belirgin ölçüde yavaş olduğu açıktır.

Elbetteki bu testler, henüz relase edilmemiş olan beta 1 sürümü üzerinden yapılmaktadır. Dolayısıyla zaman içerisinde iyileştirmelerin olması muhtemeldir. Hatta söz konusu uygulamanın çekirdeği yeniden yazılmış olan Windows 7 işletim sisteminde test edilmeside mutlaka gereklidir. Ancak, Concurrent koleksiyonların kullanılma sebeplerinin başında hız veya performans olmadığı gayet net bir biçimde ortadadır. Tabiki bunun dışında kalan senaryolardada gerçekten performans kaybını göze almamızı gerektirecek durumlar olmalıdır. Şu anda sesli düşünüyorum; "Bir uygulama içerisindeki birden fazla tipin ortaklaşa kullandığı bir koleksiyon üzerinde, eş zamanlı olarak ekleme, silme ve düzenleme işlemeleri yapılabiliyor olsun..." Bilmiyorum siz ne düşünüyorsunuz. Aslında fikirlerinizi yorum olarak paylaşabilirsiniz.

Concurrent koleksiyonlar ile ilişkili araştırmalarım devam etmekte. Örneğin şu sıralar göz kestirdiklerimden birisi olan ve aslında bu yazıda incelemek isteyipte, performans ve hız kriterine takıldığım için araştıramadığım BlockingCollection. Bunuda bir sonraki yazımda ele almaya gayret ediyor olacağım. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[ConcurrentCollectionTest.rar (23,87 kb)](/assets/files/2009/ConcurrentCollectionTest.rar)
