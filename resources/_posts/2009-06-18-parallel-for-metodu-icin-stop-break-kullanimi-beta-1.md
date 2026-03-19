---
layout: post
title: "Parallel.For Metodu için Stop, Break Kullanımı [Beta 1]"
date: 2009-06-18 06:32:00 +0300
description: "Parallel.For metodu bildiğiniz gibi döngüsel işlemleri birden fazla göreve bölerek kısa sürede yapılmasına olanak sağlamaktadır. Bu yazımda, kelimeler ile ifade etmeyi bir türlü beceremediğim ancak bir örnek üzerinden sizlere aktarabileceğim Stop ve Break metodları üzerinde durmaya çalışacağım."
categories:
  - parallel-programming
tags:
  - parallel-programming
  - csharp
  - linq
  - threading
  - generics
  - visual-studio
---
# Parallel.For Metodu için Stop, Break Kullanımı [Beta 1]
Merhaba Arkadaşlar,

Parallel.For metodu bildiğiniz gibi döngüsel işlemleri birden fazla göreve bölerek kısa sürede yapılmasına olanak sağlamaktadır. Bu yazımda, kelimeler ile ifade etmeyi bir türlü beceremediğim ancak bir örnek üzerinden sizlere aktarabileceğim Stop ve Break metodları üzerinde durmaya çalışacağım. Aslında amaç tahmin edeceğiniz üzere paralel çalışan döngü içerisinden çıkmak. Bu ardışıl çalışan bir for döngüsü göz önüne alındığında problem değil. Ya da önemsenmesi gereken sorunlara yol açabilecek bir konu değil. Nitekim tek bir Thread söz konusu. Ancak Parallel.For metodu işlemleri gerçekleştirirken birden fazla Task'in başlatılmasına neden olmaktadır. Bu durumdada Stop veya Break gibi iki farklı metodun nasıl davranış göstereceğini bilmekte yarar vardır. İşte konuyu anlayabilmek için Visual Studio 2010 Beta 1 sürümünde geliştirdiğim örnek Console uygulaması kodları.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;

namespace ParallelForStopBreak
{
    class Program
    {
        static void Main(string[] args)
        {
            ConcurrentDictionary<int, DateTime> values = new ConcurrentDictionary<int, DateTime>();

            // ls ParallelLoopState tipinden olup derleyici tarafından üretilmektedir.
            Random rnd = new Random();

            Parallel.For(0, 1000, (i,ls) =>
                {
                    // Güncel ThreadId değerini alalım.
                    string threadId=Thread.CurrentThread.ManagedThreadId.ToString();

                    values.TryAdd(i, DateTime.Now);
                    Thread.Sleep(500);
                    Console.Write("({0}) {1} ",threadId,i.ToString());

                    #region Stop Durumu

                    if (rnd.Next(1, 100) == 3) // Eğer rastgele üretilen sayı 3 ise Stop metodu çağırılır.
                    {
                        ls.Stop();
                        Console.WriteLine("\t\n {0} için Stop çağrısı yapıldı", threadId);
                    }
                    if (ls.IsStopped) // Eğer çalışan paralel Thread durdurulmuşsa
                        Console.WriteLine("\n{0} durduruldu", threadId);

                    #endregion
                }
            );

            Console.WriteLine("{0} eleman eklendi.\nÇıkmak için bir tuşa basınız.",values.Count.ToString());
            Console.ReadLine();
        }
    }
}
```

Program kodumuzda 0' dan 1000'e kadar zaman değerlerinin üretilip bir ConcurrentDictionary koleksiyonuna eklenmesi söz konusudur. Döngü içerisinde Random sınıfından yararlanılarak 3 değeri kontrol edilmektedir. Eğer 3 değerine denk gelinirse Stop metodu çağırılır. Program çalışması sonucu her seferinde farklı sonuçlar üretilmesi olasıdır. Bunu peşinen söyliyim. Nitekim her defasında farklı sayıda ve sırada görevler çalışmaktadır. Örnek sonuçlardan birisi aşağıdaki ekran görüntüsünde olduğu gibidir.

![blg33_1.gif](/assets/images/2009/blg33_1.gif)

Evettt. Şimdi bu çalışma şeklini bir değerlendirelim. Parallel.For metodu, 12,6,10,11,21,20,19,18,13,16,17,15,14 numaralı Thread'leri oluşturmuştur. Çalışma sırasında, 12 nolu Thread görevini yürütürkende Stop çağrısı gelmiştir. Bu durumda çalışmakta olan tüm paralel görevlere durdurulma emri gitmektedir. Ancak 12 nolu Thread sırasında Stop emri gelmesine rağmen diğer Thread'ler kısa bir sürede olsa (örneğe göre birer eleman ekleme süresi kadar) geç durmuştur. Thread'lerin durup durmadıkları, dikkat edeceğini üzere ParallelLoopState referansının IsStopped özelliği ile anlaşılmaktadır.

Peki ya Break metodu nasıl bir etkide bulunmaktadır. Bu amaçla Parallel.For metodu içerisine aşağıdaki kodları ekledim.

```csharp
if (rnd.Next(1, 100) == 3) // Eğer rastgele üretilen sayı 3 ise Break metodu çağırılır.
{
 ls.Break();
 Console.WriteLine("\t\n {0} için Break çağrısı yapıldı.", threadId);
}
```

Aslında bu kez Stop metodu yerine sadece Break metodunu kullandığımızı görebiliriz. Peki ya çalışma zamanı? Her zamanki her çalışma sonrası farklı sonuçların üretildiği ortadadır. Aşağıdaki ekran görüntüsünde bu çalışmalardan birisi ele alınmaktadır.

![blg33_2.gif](/assets/images/2009/blg33_2.gif)

Durumu değerlendirmeye çalışalım. Herşeyden önce birden fazla Thread'in çalıştığı kolayca gözlemlenebilir. Örnekte 10, 11, 6, 13, 12, 14, 15, 19, 21, 17 numaralı Thread'ler çalıştırılmaktadır. Derken çalışma zamanının bir anından, 14ncü Thread için Break çağrısı gelmiştir. Bunun üzerine 14 numaralı Thread durdurulmuştur. Diğer yandan, Break metodu ile karşılaşıncaya kadar başlatılan diğer Thread'ler çalışmalarına devam etmektedir. İşte Stop metodu ile aradaki önemli bir farklılık. Yinede ilerleyen kısımlarda diğer Thread'lerden bazılarının yürütülmesi esnasında Break çağrısı ile karşılaşılması olasıdır ki 13ncü Thread için bu gerçekleşmiştir. Tabiki bu çağrı sonrasında 13ncü Thread'de sonlandırılmış ama daha önceden başlatılmış diğer Thread'ler kendilerine ayrılan üst sınır değerine kadar yürümeye devam etmiştir. Nitekim ilerleyen kısımlarda diğer Thread'ler için Break komutu ile karşılaşılmamıştır.

Sanıyorumki Stop ve Break metodları arasındaki farkı biraz biraz kendini göstermeye başladı.

Yinede şu ana kadar yaptığım analizde havada kalan noktalar var gibi hissediyorum.

![Undecided](/assets/images/2009/smiley-undecided.gif)

Farklılığı tam olarak göremediğimi itirifat etemliyim. Bu nedenle Break tekniği ile ilişkili kod parçasında if kontrolünü aşağıdaki gibi değiştirdim ve Thread.Sleep süresini biraz daha kısalttım. Amaç çalışan Thread'lerden 10 numaralı Id'ye sahip olana denk gelindiğinde Break komutu kullanmak ve diğer Thread'lere ne olacağını anlamaktı.

if (threadId=="10")

Volaaaa...

![Laughing](/assets/images/2009/smiley-laughing.gif)

Bu durumda oluşan farklı çalışma zamanı sonuçlarından birisi aşağıdaki ekran görüntüsündeki gibi oldu.

![blg33_3.gif](/assets/images/2009/blg33_3.gif)

Ve diğer bir denemenin sonucu;

![blg33_4.gif](/assets/images/2009/blg33_4.gif)

Bu sonuçlara ve diğerlerine baktığımda 1000 adımlık iterasyonun, Thread'lere farklı sayılarda bölündüğünü farkettim. Diğer yandan 10 numaralı Thread çalışmaya başlayıp bir eleman eklendikten sonra gelen Break metodu çağrısı nedeniyle durdurulmuştu. Diğer Thread'ler ise çalışmalarına devam ederek kendilerine ayrılan limitler dahilinde eleman eklemeyi sürdürmüşlerdi. O zaman aynı vakada Stop metodu ne yapar diye insan ister istemez merak ediyor. Bunun üzerine Stop metodunun kullanıldığı senaryodaki if koşulunda 10 numaralı Thread'i kontrol etmeye karar verdim. Ve işte çalışma zamanı sonuçlarından birisi;

![blg33_5.gif](/assets/images/2009/blg33_5.gif)

Görüldüğü gibi 10ncu Thread çalışmaya başlayıp 1 eleman ekledikten sonra gelen Stop metodu nedeniyle hem kendisi hemde diğer Thread'ler mümkün olan en kısa sürede durdurulmuştur.

Sanıyorumki artık Stop ve Break arasındaki farkı daha iyi görebiliyoruz.

![Laughing](/assets/images/2009/smiley-laughing.gif)

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[ParallelForStopBreak.rar (21,27 kb)](/assets/files/2009/ParallelForStopBreak.rar)