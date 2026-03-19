---
layout: post
title: ".Net 4.0 Öncesi ThreadPool Kullanımı"
date: 2009-12-23 03:15:00 +0300
description: "ThreadPool; arka planda belli bir işi yapmak üzere planlanmış görevlerin Thread' lere bölünmesi ve bu Thread' lerin bir koleksiyon şeklinde tutularak asenkron işleyişlerinin yönetilmesi amacıyla kullanılan sarmalayıcı(Wrapper) bir tip olarak düşünülebilir. Genellikle sunucu tabanlı uygulamalarda değerlendirildiği gözlemlenmektedir..."
categories:
  - csharp
tags:
  - csharp
  - dotnet
  - workflow-foundation
  - threading
  - delegates
  - visual-studio
  - dependency-management
---
# .Net 4.0 Öncesi ThreadPool Kullanımı
![blg122_Giris.jpg](/assets/images/2009/blg122_Giris.jpg)

Merhaba Arkadaşlar,

İlk okulda eminimki pek çok arkadaşımız havuz problemlerinden müzdarip olmuştur. Genellikle bu havuzlarda ikiden fazla musluk olması neredeyse garantidir ve genellikle bu musluklardan bazıları havuzu belirli sürelerde doldururken, bazılarıda belirli sürelerde boşaltır. Hatta zamanla bu muslukların ne kadar hızla su doldurduğu veya boşalttığıda işin içerisine girer ve aslında sadece yüzmek için kullanabileceğimiz güzelim havuz koca bir problem haline dönüşür.

Ben açıkçası bu problemleri çözmekte hep yetersiz kalmışımdır. Hatta çoğunlukla atmasyon cevaplar ürettiğimi itiraf edebilirim. Nitekim havuz deyince aklıma genellikle yandaki resimde görülen manzara gelir. Peki havuz problemlerinden kurtulabildik mi? Ihhh

![Laughing](/assets/images/2009/smiley-laughing.gif)

Çünkü artık Multi-thread uygulamalar ile uğraşmaktayız ve elimizde yönetebileceğimiz n sayıda Thread olabiliyor. Bu Thread'ler bir havuz içerisinde toplanabilir mi peki? Evet toplanır...İşte bu günkü konumuz. ThreadPool kullanımı.

ThreadPool; arka planda belli bir işi yapmak üzere planlanmış görevlerin Thread'lere bölünmesi ve bu Thread'lerin bir koleksiyon şeklinde tutularak asenkron işleyişlerinin yönetilmesi amacıyla kullanılan sarmalayıcı (Wrapper) bir tip olarak düşünülebilir. Genellikle sunucu tabanlı uygulamalarda değerlendirildiği gözlemlenmektedir. Örneğin Windows Service'leri içerisinde ThreadPool kullanımı mantıklıdır. Bunun dışında dosya giriş çıkış (IO), yapay zeka, veritabanı, Karmaşık Matematik problemlerin çözüm algoritmaları gibi Multi-Threading gerektiren işlemlerde değerlendirilebilir.

Burada önemli olan noktalardan birisi çalışma modelidir. Aslında yapılmak istenen işe ait havuza gelen her talep, havuz içerisinde bir Thread'e atanır ve asenkron olarak yürütülür. Burada ana uygulama Thread'ine bir bağımlılık söz konusu değildir (ki ana uygulamanın ThreadPool tarafından değerlendirilen işlemlerin tamamlanmasını beklemesi yönünde uyarılması gerekebilir). Hatta alt taleplerin bekletilmeside söz konusu değildir. Bununla birlikte önemli olan noktalardan biriside, işi biten bir görevin sahibi olan Thread'in tekrardan kullanılıncaya dek havuzda yer alan kuyruğa atılmasıdır. Burada kuyrukta yer alan Thread'in, ana uygulama tarafından tekrardan kullanılması halinde, Thread oluşturma maliyetlerinin önüne geçilmesi mümkün hale gelmektedir ki bu bir avantajdır. Tabiki havuzunda belirli bir kapasitesi vardır (Varsayılan olarak 25 Thread). Bu kapasitenin dolu olması halinde ek olarak gelen görevler kuyrukta kalır ve ancak işleyen Thread'ler çalıştırılmaya müsait olduklarında icra edilebilir.

Buraya kadar anlattıklarımızı değerlendirecek olursak, Thread yönetiminin daha kolay bir şekilde ele alınabildiğini görebiliriz. Hatta ThreadPool'un temel olarak iki fonksiyonu olduğunu da düşünebiliriz. Bunlardan birincisi havuzda yer alan Thread'lerin üstlendiği işlerin tamamlanma durumlarını takip etmek, koordinasyonu sağlamak ve ikinci olarakta Thread koleksiyonunu bir kuyruk düzeninde yönetmektir. Tabi bu durumda ThreadPool tipinin yönettiği koleksiyona thread ekleme (enqueue) ve çıkarma (dequeue) işlemleri ile ilişkili bir algoritma içeridiğini ifade edebiliriz. Hatta kaç Thread'e ihtiyaç duyulacağını hesaplamak gibi önemli yeteneklere sahip olduğunu da söylemeliyiz.

Açıkçası bu iki fonksiyonelliğin içerdiği algoritmaları geliştirmekle uğraşmak yerine işi ThreadPool'a bırakmak daha optimal bir çözüm olarak görülmelidir. Yine de istendiğinde kendi ThreadPool tiplerimizi de geliştirebiliriz. Tabi bilindiği üzere.Net Framework 4.0 ile birlikte ThreadPool üzerinde de bazı geliştirmeler yapılmıştır. Bu geliştirmelere göre, havuzun çalışma mantığı değişmiştir. Ancak şu anda bu konuya girmeyeceğiz. Önce var olan modeli bir öğrenelim. (.Net 4.0 tarafındaki ThreadPool kabiliyetlerini biraz daha cesaret toplayıp ileride incelemeyi ve sizlere aktarmayı planlıyorum ![Wink](/assets/images/2009/smiley-wink.gif)) Dilerseniz bu kadar laf kalabalığından sonra basit bir örnek ile devam edelim. Visual Studio 2008 ortamında ve.Net Framework 3.5 tabanlı olaraktan bir Console uygulaması geliştireceğiz. İşte örnek kodlarımız.

```csharp
using System;
using System.Diagnostics;
using System.Threading;

namespace WhatIsThreadPool
{
    class Program
    {
        static ManualResetEvent[] mrEvents;
        static int[] testNumbers; // Faktöryel değerleri hesap edilecek sayıların tutulacağı dizi.
        static long[] results; // Faktöryel sonuçlarının tutulacağı dizi
        static int testCount = 5; // Denemesayısı

        static void Main(string[] args)
        {
            Console.WriteLine("Başlamak için bir tuşa basınız. Ana Thread Id : {0}",Thread.CurrentThread.ManagedThreadId.ToString());
            Console.ReadLine();

            // Ana Thread' i pool içinde çalışan Thread' lerin bittiği konusunda bilgilendirecek ManualResetEvent nesne dizisi oluşturulur
            mrEvents = new ManualResetEvent[testCount];
            results = new long[testCount];
            testNumbers = new int[testCount];
            Random rnd = new Random();

            Stopwatch watcher = new Stopwatch();
            watcher.Start();

            for (int i = 0; i < testCount; i++)
            {
                // Başlangıçta ManualResetEvent nesnesi false değer ile üretilir.
                mrEvents[i] = new ManualResetEvent(false);
                testNumbers[i] = rnd.Next(1, 20); // örnek bir test sayısı üretimi
                ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadWork), i);
            }

            // Tüm Thread' lerin işi bitinceye kadar ana uygulamayı duraksat
            WaitHandle.WaitAll(mrEvents);

            watcher.Stop();
            Console.WriteLine("\nİşlemler tamamlandı...Toplam Süre {0} \n",watcher.Elapsed.TotalMilliseconds.ToString());

            for (int i=0;i<results.Length;i++)
            {
                Console.WriteLine("\t\t{0} için sonuç {1} ",testNumbers[i].ToString(), results[i].ToString());
            }
            Console.ReadLine();
        }

        // ThreadPool içerisindeki Thread' lerin işaret ettiği metod. WaitCallback temsilcisinin bildirimine uygun olaraktan object tipinden parametre almakta ve değer döndürmemektedir
        static void ThreadWork(object obj)
        {
            int currentNumber = (int)obj;

            Console.WriteLine("{0} sayısı için hesaplama. Current Thread Id : {1}",testNumbers[currentNumber].ToString(),Thread.CurrentThread.ManagedThreadId.ToString());
            
            // Faktöryel hesaplamasını gerçekleştiren metod çağrısı
            results[currentNumber]=Factorial(testNumbers[currentNumber]);
            // Ana Thread' in bilgilendirilmesi sağlanır.
            mrEvents[currentNumber].Set();
        }

        // Faktöryel hesabını yapan recursive metod
        static long Factorial(int number)
        {
            long result;
            if (number == 0
                || number == 1)
                result = 1;
            else
                result = Factorial(number - 1) * number;

             return result;
        }
    }
}
```

Uygulamamızda 1 ile 20 arasındaki rastgele 5 sayının Faktöryel değerlerinin hesaplanmasında ThreadPool'dan yararlanılmıştır. Örneği çalıştırdığımızda her seferinde farklı sonuçlar elde etmemiz söz konusudur. İşte benim yakaladığım sonuçlardan birisi.

![blg122_FirstRun.gif](/assets/images/2009/blg122_FirstRun.gif)

Dikkat edileceği üzere ThreadPool tarafında iki Thread üretilmiş ve 5 sayısal değer için gerçekleştirilen faktöryel hesaplamaları bu Thread'ler tarafından ele alınmıştır.

> Kişisel Not: Tabi işin kolayına kaçtığımızı ifade etmek isterim. Özellikle 21 sayısı dahil sonraki faktöryel hesaplarında eksi değerlere geçtiğimizden sayı aralığımız çok sınırlı. Bir diğer yanıltıcı nokta ise toplam hesaplama süresi. Buradaki faktöryel işlemleri aslında pek yorucu işlemler değildir. Bu nedenle aynı örneği Thread mekanizması olmadan çalıştırdığınızda hesaplama süresinin çok çok daha kısa sürdüğünü görebilirsiniz.
> Elbette bizim odaklandığımız nokta bu değil. Aslında ThreadWork metodunun çalıştırdığı Factorial fonksiyonunun gerçekten uzun süren yoğun işlemler gerçekleştirdiği düşünülebilir. Bu durumda ThreadPool mekanizması bize zaman yönünden avantaj getirecektir. Hatta işlemlerin uzunluğuna göre Thread sayısını arttırmasıda mümkün olabilir. Buna göre çıkartmamız gerken bir derste gerçekten ihtiyaç olunduğunda ThreadPool modelinin kullanılmasının uygun olduğudur.

Tabi dikkat edilmesi gereken bir kaç nokta olduğunu söyleyebiliriz. Öncelikli olarak Workflow Foundation mimarisinde de sık sık karşımıza çıkan ManualResetEvent tipinden bahsedelim. Bu tipten yararlanarak bir Thread'den başka bir Thread'e işin bittiğine dair sinyal gönderilmesi mümkündür. Bu noktada ThreadPool içerisinde çalışmakta olan Thread'lerin işlemlerini bitirmesini takiben, ThreadPool'un sahibi olan ana Thread'in (ki burada Main metodunun yer aldığı Program tipine ait Thread'den bahsediyoruz) işlemlerin tamamlandığı yönünde uyarılması gerekmektedir. Bu sebepten ManualResetEvent nesne örnekleri, Set metodu yardımıyla diğer Thread'i işlemlerin bittiği yönünde uyarmaktadır.

Ana uygulama için önem arz eden noktalardan biriside, ThreadPool içerisinde çalıştırılmakta olan Thread'lerin tamamının görevleri sonuçlanıncaya kadar beklemesi gerekebileceğidir. Bu sebepten ManualResetEvent tipinden olan dizinin tüm elemanlarının Set metodunun çalıştırılıp ana uygulama Thread'ini uyarması gerekmektedir. Örnek kod parçasından da görüleceği üzere söz konusu duraksatma işlemi için WaitHandle tipine ait static WaitAll metodunun çağırılması yeterlidir.

Thread'ler ile ilişkili görevlerin ThreadPool tarafından yönetilen kuyruğa atılmaları için QueueUserWorkItem metodundan yararlanılmaktadır. Bu metodun ilk parametresi dikkat edileceği üzere WaitCallback temsilcisi (delegate) tipindendir. Bu temsilci object tipinden parametre alan ve geriye değer döndürmeyen (void) metodları işaret edebilir. Buna göre Thread'lerin eşleştiği görevleri üstlenen fonksiyonların söz konusu metod modeline uygun olması gerekmektedir. İlgili metod içerisinde dikkat edileceği üzere ManualResetEvent nesne örneği üzerinden Set metodu çağrısı gerçekleştirilmektedir. Tabi C# 3.0 tarafında gelen lambda operatörü (=>) sayesinde aynı kodun aşağıdaki şekilde yazılmasıda mümkündür.

```csharp
for (int i = 0; i < testCount; i++)
            {
                // Başlangıçta ManualResetEvent nesnesi false değer ile üretilir.
                mrEvents[i] = new ManualResetEvent(false);
                testNumbers[i] = rnd.Next(1, 20); // örnek bir test sayısı üretimi
                // ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadWork), i);

                ThreadPool.QueueUserWorkItem((obj) =>
                {
                    int currentNumber = (int)obj;

                    Console.WriteLine("{0} sayısı için hesaplama. Current Thread Id : {1}", testNumbers[currentNumber].ToString(), Thread.CurrentThread.ManagedThreadId.ToString());

                    // Faktöryel hesaplamasını gerçekleştiren metod çağrısı
                    results[currentNumber] = Factorial(testNumbers[currentNumber]);
                    // Ana Thread' in bilgilendirilmesi sağlanır.
                    mrEvents[currentNumber].Set();
                }
                , i);                
            }
```

Böylece geldik bir yazımızın daha sonuna. Umarım ThreadPool konusunda biraz fikir sahibi olabilmişizdir. Tekraradan görüşünceye dek hepinize mutlu günler dilerim.

[WhatIsThreadPool.rar (23,73 kb)](/assets/files/2009/WhatIsThreadPool.rar)