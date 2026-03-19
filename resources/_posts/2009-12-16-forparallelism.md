---
layout: post
title: "FORParallelism"
date: 2009-12-16 04:55:00 +0300
description: "Günümüz yazılım teknolojilerinin belkide en popüler olan konularından biriside paralel programlamadır(Parallel Programming). Özellikle kullanıcı bilgisayarlarının artık birden fazla çekirdeğe sahip işlemcilerle donatılmış olduğu düşünüldüğünde geliştirme ortamlarının da(.Net Framework 4.0' da olduğu üzere) paralel programlamaya daha fazla destek vermeye başladığını görmekteyiz."
categories:
  - tpl
tags:
  - tpl
  - csharp
  - parallel-programming
  - task-parallel-library
  - threading
  - concurrency
  - performance
  - generics
  - visual-studio
---
# FORParallelism
![blg100_Giris.jpg](/assets/images/2009/blg100_Giris.jpg)

Merhaba Arkadaşlar,

Günümüz yazılım teknolojilerinin belkide en popüler olan konularından biriside paralel programlamadır (Parallel Programming). Özellikle kullanıcı bilgisayarlarının artık birden fazla çekirdeğe sahip işlemcilerle donatılmış olduğu düşünüldüğünde geliştirme ortamlarının da (.Net Framework 4.0' da olduğu üzere

![Wink](/assets/images/2009/smiley-wink.gif)

) paralel programlamaya daha fazla destek vermeye başladığını görmekteyiz.

Aslında zaten var olan araçlar ile paralel programlama tekniklerini uygulayabilmekteyiz. Ne varki kodlanmasının karmaşık olması bir yana, birden fazla tekniğin kullanılabiliyor olması, hangisinin daha performanslı olduğunun anlaşılması için test aşamalarının da önemini ortaya çıkarmakta. Microsoft cephesi bir süredir, paralel programlama kütüphanesi ile söz konusu tekniklere ait tasarımları aza indirgeyip kolay geliştirilebilir ve performanslı sonuçlar üreten tiplerin tasarlanması ve geliştirilmesini gereçekleştirmekte..Net Framework 4.0 içerisinde doğrudan gelen Task Parallel Library kütüphanesi bu anlamda önemli kabiliyetler içermekte.

Peki elimizde bu kütüphane olmasaydı? ![Sealed](/assets/images/2009/smiley-sealed.gif) O zaman n sayıda tekrar edecek olan bir işlemi paralel hale getirmek için nasıl bir kodlama yapmamız gerekirdi?

Söz gelimi başlangıç ve bitiş değerleri parametrik olan bir döngünün içerisinden çağırılan bir fonksiyonun, birden fazla Thread'e bölünerek çalıştırılmasını istediğimizi düşünelim. Aslında teorik olarak makinede kaç işlemci yada kaç çekirdek var ise o sayıda Thread açılması tercih edilir. Buna göre tekrar edecek olan işlemler belirli aralıklara bölünerek bu aralıkların açılan Thread'ler tarafından ele alınması sağlanır. Ne demek istediğimi aşağıdaki örnek kod parçası ile aktarmaya çalışayım.

```csharp
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Parallelism
{
    class Program
    {
        static void Main(string[] args)
        {
            ParallelFor(48, 98,
                (i) =>
                {
                    double r = 0;
                    for (int j = 0; j < i * 99999; j++)
                        r = Math.Sqrt((i * Math.PI) / Math.E);
                    Console.Write("{0} ", i.ToString());
                }
            );

            Console.ReadLine();
        }

        static void ParallelFor(int lowerBound, int upperBound, Action<int> body)
        {
            int processorCount = Environment.ProcessorCount; // İşlemci/çekirdek sayısı bulundu
            int range = (upperBound - lowerBound) / processorCount; // Yaklaşık iterasyon sayısı hesaplanır. 
            Console.WriteLine("İşlemci/Çekirdek Sayısı : {0} , Iterasyon Boyutu {1}\n", processorCount.ToString(), range.ToString());

            #region Birinci Yöntem (List<Thread> Kullanımı)

            List<Thread> threads = new List<Thread>(processorCount); // İşlemci/çekirdek sayısı kadar Thread taşıyacak koleksiyon tanımlanır.

            // İşlemci/çekirdek sayısı kadar çalışacak bir döngü
            for (int processor = 0; processor < processorCount; processor++)
            {
                // Thread tarafından ele alınacak değer aralığı hesaplanır
                int startPoint = (processor * range) + lowerBound;
                int endPoint = (processor == processor - 1) ? upperBound : startPoint + range;
                Console.WriteLine("Start : {0} End : {1}", startPoint.ToString(), endPoint.ToString());
                // Her bir çekirdek için bir Thread oluşturulur ve içerisinde iterasyon aralığı uzunluğunda bir döngü oluşturularak parametre olarak gelen fonksiyon çalıştırılır
                threads.Add(new Thread(() =>
                    {
                        for (int i = startPoint; i < endPoint; i++)
                        {
                            body(i);
                        }
                    }
                )
                );
            }

            // Thread' ler çalıştırılır
            foreach (Thread t in threads)
            {
                t.Start();
            }
            // Thread' lerin tamamlanması beklenir
            foreach (Thread t in threads)
            {
                t.Join();
            } 
            #endregion
        }
    }
}
```

Örneğimizde tamamen anlamsız olan bir döngü çalıştırıldığını görmektesiniz. İçerdiği bazı hesaplamalar sayesinde zaman alan bir işlemler bütünü söz konusu. Burada söz konusu olan operasyonun birden fazla Thread'e bölünerek çalıştırılması içinde ParallelFor isimli yardımcı metoddan yararlanılmaktadır. Bu modelde ParallelFor isimli metod döngünün başlangıç ve bitiş değerlerini almakta, ayrıca çalıştıracağı fonksiyonu işaret eden Action tipinden bir parametre almaktadır.

Metoda göre Thread'lerin değerlendireceği aralıklar hesaplanır. Thread'ler basit bir List koleksiyonunda tutulmakta olup çalıştırılmaları ve tamamlanmalarının beklenmeleri için iki foreach döngüsünden yararlanılır. İlk döngü oluşturulan Thread'leri başlatırken diğeride oluşturulan Thread'leri Main Thread'e katıp uygulamanın sonlanması için söz konusu Thread'lerin işlerinin bitirilmesinin beklenmesini garanti etmektedir. Dikkat edilmesi gereken nokta işlemci/çekirdek sayısı kadar Thread oluşturulması ve oluşturulan her bir Thread'in yaklaşık olarak hesap edilen iterasyon alanı kadar değeri hesaba katarak Action ile gelen operasyonu çalıştırmasıdır.

Peki çalışma zamanındaki durum nedir?

Bu konuda çok şanslıyız nitekim Visual Studio 2010 ile birlikte son derece etkili performans analiz araçları gelmekte. Geliştirmeyi yapmakta olduğumuz Visual Studio 2010 Ultimate Beta 2 sürümünde yer alan Concurrency Profiler raporuna bakıldığında, yukarıdaki örnek için aşağıdaki sonuçların elde edildiği görülür.

İlk çalışmanın sonucu oluşan ekran görüntüsü;

![blg100_FirstRun.gif](/assets/images/2009/blg100_FirstRun.gif)

İlk çalışma sonucu elde edilen Concurrency Profiler çıktısı;

![blg100_FirstReport.gif](/assets/images/2009/blg100_FirstReport.gif)

Sarı alanlar bir Thread'in çalışmakta olduğunu ama diğer bir Thread tarafından o süre boyunca etkisizleştirildiğini göstermektedir. Yeşil renkli alanlar Thread'in işini yaptığı zaman aralıklarıdır. Sarı bölgelerin fazla olması performansı olumsuz yönde etkileyen bir faktördür. Nitekim aşırı talebin (Oversubscription) oluştuğunu göstermektedir. Peki iyileştirmenin bir yolu olabilir mi? Aslında ThreadPool tipinden yararlanarak Therad yönetiminin sisteme bırakılması sağlanabilir. İşte buna göre yazılan yeni modelimiz;

```csharp
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Parallelism
{
    class Program
    {
        static void Main(string[] args)
        {
            ParallelFor(48, 98,
                (i) =>
                {
                    double r = 0;
                    for (int j = 0; j < i * 99999; j++)
                        r = Math.Sqrt((i * Math.PI) / Math.E);
                    Console.Write("{0} ", i.ToString());
                }
            );

            Console.ReadLine();
        }

        static void ParallelFor(int lowerBound, int upperBound, Action<int> body)
        {
            int processorCount = Environment.ProcessorCount; // İşlemci/çekirdek sayısı bulundu
            int range = (upperBound - lowerBound) / processorCount; // Yaklaşık iterasyon sayısı hesaplanır. 
            Console.WriteLine("İşlemci/Çekirdek Sayısı : {0} , Iterasyon Boyutu {1}\n", processorCount.ToString(), range.ToString());

            #region İkinci Yöntem (ThreadPool)

            int remainingProcessor = processorCount;
            // ManualResetEvent bir olay meydana geldiğinde beklemekte olan bir veya daha çok Thread' e bilgilendirmede bulunur.
            ManualResetEvent manuelResetEvent = new ManualResetEvent(false);
            for (int processor = 0; processor < processorCount; processor++)
            {
                int startPoint = (processor * range) + lowerBound;
                int endPoint = (processor == processorCount - 1) ? upperBound : startPoint + range;

                // ThreadPool, Thread' ler için bir havuz sağlar ve asenkron işleyişin yönetimini sağlar
                // QueueUserWorkItem yürütülmek üzere bir metodu kuyruğa atar.ThreadPool içerisindeki ilgili thread kullanılabilir olduğunda da metod icra edilir.
                ThreadPool.QueueUserWorkItem((o) =>
                    {
                        for (int i = startPoint; i < endPoint; i++)
                        {
                            body(i);
                        }
                        // Birden fazla Thread tarafından kullanılan remainingProcessor değeri azaltılır ve 0 olup olmadığı kontrol edilir. Eğer 0 ise bekleyen tüm Thread' ler için sinyal verilir
                        if (Interlocked.Decrement(ref remainingProcessor) == 0)
                            manuelResetEvent.Set();
                    }
                );

            }
            // Diğer Thread' ler tamamlanıncaya kadar(ki Set dolayısıyla sinyal geldiğinde anlaşılır) ana thread' i bekletecektir.
            manuelResetEvent.WaitOne();
            manuelResetEvent.Close(); // Güncel WaitHandle ile alakalı tüm kaynaklar serbest bırakılır(Release).

            #endregion
        }
    }
}
```

Bu sefer ThreadPool tipinin QueueUserWorkItem static metodu kullanılmıştır. ThreadPool bir önceki modele göre Thread yönetimini daha iyi yapmaktadır. Öyleyse yeni modele göre oluşan çalışma zamanı Thread analizine bir bakalım.

İkinci modele göre çalışma sonucu;

![blg100_SecondRun.gif](/assets/images/2009/blg100_SecondRun.gif)

İkinci modele göre Concurrency Profiler çıktısı;

![blg100_SecondReport.gif](/assets/images/2009/blg100_SecondReport.gif)

Bu rapora göre Thread'lerin toplam çalışma sürelerinin bir önceki modele göre azaldığı görülmektedir. Sarı alanların süresi daha az görünsede sayıları yinede çok azalmış değildir. Dolayısıyla aşırı talep (Oversubscription) durumu devam ediyor görünmektedir. Ancak ana Thread'in sadece Thread'lerin çalışması tamamlanıncaya kadar bloklandığı gözlemlenmektedir. Peki yeni bir yöntem tercih edilebilir mi? Evet edilebilir. Aslında ThreadPool kullanımının iyileştirilmesi yoluna gidilebilir ki biz daha fazla ilerlemeyeceğiz...

Gördüğünüz üzere çoğu geliştirici açısından ileri seviyede kalan bir kodlama gerekmektedir. Özellikle geliştiricinin Thread konusuna son derece iyi hakim olması şarttır. Her ne kadar söz konusu karmaşık teknikler birer tasarım kalıbı olarak şekillenmiş olsalarda geliştiricinin kafa ayarını da fazla çizdirmemek gerekir. Buda yazımızın neden kafayı çizmiş bir bilgisayarcı resmi ile başladığının ispatıdır

![Laughing](/assets/images/2009/smiley-laughing.gif)

İşte Task Parallel Library ile birlikte gelen tipler bu anlamda işleri kolaylaştırmaktadır. Ama tabiki Concurrency Profiler ile üretilen rapor sonuçlarını değerlendirmek gerekir.(Bu tip karmaşık teknikleri tercih ederken kişisel görüşüme göre programcının performans mı? kolay ve hızlı kodlama mı? sorusuna verdiği cevap büyük önem kazanmaktadır) İşte aynı süreç için Parallel.For kullanımı ve rapor sonuçları;

```csharp
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Parallelism
{
    class Program
    {
        static void Main(string[] args)
        {
            Parallel.For(48, 98, (i) =>
                {
                    double r = 0;
                    for (int j = 0; j < i * 99999; j++)
                        r = Math.Sqrt((i * Math.PI) / Math.E);
                    Console.Write("{0} ", i.ToString());
                }
            );

            Console.ReadLine();
        }
    }
}
```

Parallel.For kullanımının sonucu;

![blg100_ThirdRun.gif](/assets/images/2009/blg100_ThirdRun.gif)

Parallel.For kullanımına göre Concurrency Profiler çıktısı;

![blg100_ThirdReportAgain.gif](/assets/images/2009/blg100_ThirdReportAgain.gif)

Bu rapora göre Thread işlemlerinin tamamlanma süresinin çok daha azaldığı görülmektedir. Ayrıca Sarı alanların sayısında belirgin ölçüde azalma gözlemlenmektedir. İlginç olan noktalardan biriside Main Thread'de bloklanmanın (Kırmızı alanlar) diğer modellere göre çok daha az sayıda olmasıdır. Bir başka deyişle aşırı talep (Oversubscription) durumu biraz daha azalmıştır.

> Raporlar ile ilişkili not: Döngülerin çalışma zamanında açtıkları Thread'lerin çalışma şeklini raporlamak amacıyla Visual Studio 2010 ile birlikte gelen Debug menüsündeki Start Peformance Analysis öğesi kullanılmaktadır. Bu öğe ile açılan sihirbazda bizim örneğimiz için Concurrency seçeneği işaretlenmelidir. Ayrıca bu seçeneğin alt seçimi olan Visualize the behavior of multithreaded application kutucuğunun işaretlenmiş olması da gerekmektedir. Ancak bu son seçenek Windows 7, Server 2008 işletim sistemleri üzerinde kullanılabilmektedir.
> Raporların oluşturulması programın çalıştırılması ile birlikte başlamaktadır. Bu nedenle söz konusu analiz raporlarının üretilmesi zaman alabilir. Raporlarda n sayıda Thread'in görülmesi mümkündür. Örneklerimizdeki analizlerimizi kolayca incelemek için sadece ilgili Thread'lerin çalışma zamanı durumları göz önüne alınmıştır. Diğerleri ise gizlenmiştir. Üretilen analiz raporundaki Threads kısmında yer alan renklerin belirli anlamları vardır. Sarı renkler Preemption olarak adlandırılmakta olup genellikle aşırı talep (Oversubscription) ile ilişkili süreleri belirtmektedir. Yeşil alanlar Thread'in iş yaptığını gösteren zaman aralıkları iken kırmızı alanlar bloklama yapılan zaman aralıklarını ifade etmektedir.

Tabiki bu test sonuçları, uygulamanın çalıştığı sistemin donanımsal özelliklerine göre değişiklik gösterecektir. Ancak sonuç olarak Parallel.For döngüsünün paralel işlemleri daha efektif olarak yürüttüğünü düşünebiliriz. Bunlara ek olarak aslında Parallel.For döngüsünün sağladığı başka avantajlarda vardır. Bunlar aşağıda listelenmiştir.

- Etkili Yük Dengelemesi (Load Balancing): Parallel.For Thread'ler arasındaki yük dağılımını organize eder.
- Dinamik Thread Sayısı: Parallel.For akılldır ve zaman aşımları durumunda döngü içerisindeki Thread sayılarını dinamik olarak ayarlayabilir.
- Yüksek Değer Aralıkları: Parallel.For metodu Int32 dışında Int64 tipini de kullanılabilir.
- Konfigurasyon Seçenekleri: Örneğin Parallel.For içerisinde açılacak olan Thread sayısı için limit belirlenebilir.
- İstisna Yönetimi (Exception Handling): Döngü içerisinde bir istisna oluştuğunda, dahil olan tüm Thread'ler mümkün olduğunca kısa sürede işlemlerini durdururlar. Aslında varsayılan olarak yeni iterasyonların başlaması durdurulur. Bu nedenle exception sonrası Thread'lerin yürüttüğü bazı iterasyon adımları devam edebilir ancak yenilerinin başlatılması exception nedeniyle engellenir.
- İç içe paralellik (Nested Parallelism):İç içe çalıştırılan Parallel.For döngüleri birbirlerinin Thread kaynaklarını koordineli olarak paylaşarak çalışırlar.
- vb...

Şimdi bu avantajları kendi yazdığımız ParallelFor metodu içinde gerçellemeye çalıştığımızı düşünelim. Hatta deneyin

![Wink](/assets/images/2009/smiley-wink.gif)

Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[TaskParallelLibrary.rar (89,38 kb)](/assets/files/2009/TaskParallelLibrary.rar)