---
title: "AttachedToParent Hakkında Detaylar"
pubDate: 2010-09-02 08:30:00
categories:
  - Parallel Programming
  - TPL
tags:
  - Parallel Programming
  - TPL
  - C#
  - .NET
  - Threading
  - Visual Studio
  - DataTable
---

# AttachedToParent Hakkında Detaylar
![blg178_Giris.jpg](images/blg178_Giris.jpg)

Merhaba Arkadaşlar,

Malum "her yiğidin farklı bir yoğurt yiğiş tarzı vardır" derler. Genellikle programlama dilleri veya.Net Framework gibi yapılarda da bir sonuca ulaşmak için birden fazla ve farklı yol söz konusu olabilir. Böyle bir durumun oluşmasına neden olan etkenlerin başında, çevresel ortam parametrelerinin farklılaşmasının geldiğini ifade edebiliriz.

Çok basit bir kaç örnek vererek olayı kafamızda daha net bir şekilde canlandırmaya çalışalım. Bir koleksiyon içerisindeki öğeleri for veya foreach döngüleri ile dolaşabiliriz. Ya da örneğin bir veri tablosundan veriyi çekmek için, DataTable bazlı bir tekniği veya DataReader bazlı bir yöntemi ele alabiliriz. Ancak öyle vakalar söz konusudur ki, aynı amaç için ele alınabilecek veya değerlendirilebilecek yolların sayısı çok fazladır. Bu fazlalık bir süre sonra karar vermeyi zorlaştırır ve işin içinden çıkılmaz bir duruma düşülebilir.

Söz gelimi paralel programlama konusu ile ilgili olarak bir süredir incelediğimiz Parent-Child Task ilişkilerini göz önüne alalım. Daha önceki iki yazımızda ([Parent-Child Tasks Kavramı](Parent-Child Tasks Kavramı.md), [Parent-Child Task Exception Durumlar](Parent-Child Task Exception Durumları.md)) sürekli olarak AttachedToParent metodunun belirli bir kullanımını ele aldık. Oysa ki, Child Task örneklerinin Parent Task örneklerinin yaşam döngülerine eklenmelerinde izlenebilecek birden fazla yol bulunmaktadır. Buna göre Parent Task örneğine dahil olmak için aşağıdaki tekniklerden herhangibirisinden yararlanılabilir.

- Task nesnesine ait yapıcı metod (Constructor) içerisinde TaskCreationOptions.AttachedToParent enum sabiti bildirimi yapılarak
- Task sınıfının static StartNew metodunda TaskCreationOptions.AttachedToParent enum sabiti bildirimi yapılarak
- Task nesne örneği üzerinden çağırılabilen ContinueWith metoduna TaskContinuationOptions.AttachedToParent enum sabiti parametresini geçirerek
- Task.Factory.ContinueWhenAll static metoduna TaskContinuationOptions.AttachedToParent enum sabiti parametresini geçirerek
- Task.Factory.FromAsync metodu içerisinde TaskCreationOptions.AttachedToParent enum sabiti bildirimi yapılarak
- TaskCompletionSource nesne örneğini oluştururken, TaskCreationOptions.AttachedToParent enum sabitini parametre olarak geçirerek

Yazımızın bundan sonraki bölümlerinde söz konusu çalıştırma seçeneklerinden bazılarını, örnek kodlar yardımıyla incelemeye çalışalım. İlk olarak aşağıdaki kod parçası ile işe başlayabiliriz.

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AttachedToParentCases
{
    class Program
    {
        static void Main(string[] args)
        {
            Task parentTask = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Parent Task...");
                
                #region 1 - Constructor Kullanımı

                Task childTask1 = new Task(() =>
                    {
                        Console.WriteLine("Child Task 1");
                    }, TaskCreationOptions.AttachedToParent
                );
                childTask1.Start();

                #endregion

                Thread.Sleep(30000); //Debug modda Parallel Task' leri izlemek için konulmuştur
            });
            parentTask.Wait();
        }
    }
}
```

Bu kod parçasında yer alan işleyişi kavrayabilmek childTask1 içerisindeki Console.Writeline satırına BreakPoint koyarak ilerleyecek ve çalışma zamanında Parallel Tasks penceresindeki durumu analiz etmeye çalışacağız. Bu işlemleri diğer kod örnekleri için de tekrar edeceğiz. İşte ilk kod parçamızın debug moddaki durumu.

![blg178_Region1Runtime.gif](images/blg178_Region1Runtime.gif)

Bu kod örneğinde, parentTask nesne örneği StartNew metodu ile başlatılırken içerisinde de bir Child Task örneği önce new operatörü ile oluşturulmakta, sonrasında ise Start metodu ile çalıştırılmaktadır. Çalışma zamanında BreakPoint koyduğumuz noktadan Parallel Tasks penceresine baktığımızda iki adet Task örneğinin var olduğunu görebiliriz. parentTask nesne örneği Thread.Sleep metodu nedeniyle Waiting modundadır. Diğer yandan başlatılan Child Task örneği Running modundadır. Ancak burada önemli olan nokta ID değeri 2 olan Task örneğinin Parent değeridir. Dikkat edileceği üzere 2 numaralı ID değerine sahip Task örneğinin bağlı olduğu Task, 1 numaralı ID değerine sahip Task örneğidir.

Gelelim ikinci kod örneğimize. Bu sefer Child Task örneğini Task.Factory.StartNew metodu yardımıyla oluşturmaktayız.

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AttachedToParentCases
{
    class Program
    {
        static void Main(string[] args)
        {
            Task parentTask = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Parent Task...");                

                #region 2 - StartNew Kullanımı

                Task childTask2 = Task.Factory.StartNew(() =>
                    {
                        Console.WriteLine("Child Task 2");
                    }, TaskCreationOptions.AttachedToParent
                    );

                #endregion

                 Thread.Sleep(30000); //Debug modda Parallel Task' leri izlemek için konulmuştur
            });
            parentTask.Wait();
        }
    }
}
```

Gelelim çalışma zamanı çıktısına.

![blg178_Region2Runtime.gif](images/blg178_Region2Runtime.gif)

Bir önceki örnektekine benzer olaraktan, Child Task nesne örneğinin icra ettiği kod satırında durulduğunda, çalışmakta olan 2 numaralı ID değerine sahip Task nesne örneğinin dahil olduğu Parent Task'in, 1 numaralı ID değerine sahip Task olduğu görülebilmektedir ki bu kodumuzda yer alan parentTask değişkeninin işaret ettiği Task'dir.

İlk iki kod örneğimizde olaylar oldukça nettir ve beklediğimiz şekildedir. Dilerseniz diğer örnekler ile devam edelim ve işleri biraz daha karıştıralım

![Wink](images/smiley-wink.gif)

İşte yeni kod parçamız.

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AttachedToParentCases
{
    class Program
    {
        static void Main(string[] args)
        {
            Task parentTask = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Parent Task...");

                #region 3 - Task<Task>.Factory.StartNew() Kullanımı

                 Task<Task> childTask3 = Task<Task>.Factory.StartNew(() =>
                    {
                        Console.WriteLine("Child Task 3");
                        return Task.Factory.StartNew(() =>
                        {
                            Console.WriteLine("Child Task 4");
                        });                        
                    }
                    , TaskCreationOptions.AttachedToParent
                    );

                #endregion
               

                Thread.Sleep(30000); //Debug modda Parallel Task' leri izlemek için konulmuştur
            });
            parentTask.Wait();
        }
    }
}
```

Bu sefer biraz daha dikkatli davranmamız gerekiyor. childTask3 isimli nesne örneği oluşturulurken, içerisinde iş yapan diğer bir Child Task başlatılmaktadır. Dikkat edileceği üzere childTask3 için AttachedToParent değeri kullanılmış ancak içerideki childTask4 için böyle bir bildirimde bulunulmamıştır. Söz konusu yeni kod parçası çalışma zamanında debug edilirken iki noktada durup düşünmek gerekmektedir.

![blg178_Region3_1Runtime.gif](images/blg178_Region3_1Runtime.gif)

Yukarıdaki duruma göre childTask3, parentTask'in alt Task örneğidir. Parent sütünundaki 1 değeri bunu ispat etmektedir. İlginç olan ise childTask3 içerisinde başlatılan yeni bir Task'in içerisindeki BreakPoint noktasında durulduğunda ortaya çıkmaktadır.

![blg178_Region3_2Runtime.gif](images/blg178_Region3_2Runtime.gif)

Volaaa!!!

![Wink](images/smiley-wink.gif)

Dikkat edilecek olursa en içteki Task, parentTask nesne örneğinin çalışma zamanındaki yaşam döngüsüne ilave edilmemiştir. Kendi başına çalışan bir Task olarak ele alınmaktadır. İşte bu, dikkat edilmesi gereken vakalardan birisidir. Nitekim parent Task örneğine Attach edilen bir Task içerisindeki Task'lerin enum sabitinin ilgili değeri belirtilmeden Attach olmaları gerektiği sanılabilir. Oysaki şu durumda böyle olmadığı görülmektedir.

Gelelim 4ncü kod parçamıza.

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AttachedToParentCases
{
    class Program
    {
        static void Main(string[] args)
        {
            Task parentTask = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Parent Task...");
                
                #region 4 - ContinueWith Kullanımı

                Task detachedTask = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("Detached Task");
                }
                );
                Task childTask5 = detachedTask.ContinueWith((t) =>
                {
                        Console.WriteLine("Child Task 5");
                }
                , TaskContinuationOptions.AttachedToParent);

                #endregion

                 Thread.Sleep(30000); //Debug modda Parallel Task' leri izlemek için konulmuştur
            });
            parentTask.Wait();
        }
    }
}
```

Bu kez ContinueWith kullanımı söz konusudur. Dikkat edileceği üzere Parent Task örneğine Attach edilmeyen detachedTask isimli bir Task örneği mevcuttur. Lakin childTask5 isimli nesne örneği oluşturulurken ContinueWith metodu kullanılmış ve ayrıca TaskContinuationOptions.AttachedToParent enum sabiti ile parent Task örneğine Attach edileceği belirlenmiştir. Bakalım gerçekten böyle midir? Yine iki noktada BreakPoint kullanarak söz konusu durumu analiz etmeye çalışacağız. İlk olarak detachedTask içerisinde duralım.

![blg178_Region4_1Runtime.gif](images/blg178_Region4_1Runtime.gif)

Görüldüğü gibi detachedTask örneği açıkça belirtilmediği için Parent Task örneğinin yaşam döngüsüne dahil edilmemiştir ki normali de buduru. Ancak ikinci BreakPoint noktasına geldiğimizde aşağıdaki ekran görüntüsünde yer alan sonuçlar ile karşılaşırız.

![blg178_Region4_2Runtime.gif](images/blg178_Region4_2Runtime.gif)

Beklediğimiz gibi childTask5 nesne örneği 1 numaralı ID değerine sahip parentTask nesne örneğinin başlattığı yaşam döngüsüne dahil edilmiştir. Dikkat edilmesi gereken nokta, Attach edilmeyen bir Task örneği ile devam eden başka bir Task örneğinin, Parent Task yaşam döngüsüne dahil edilebiliyor olmasıdır.

5nci durum ile devam edelim.

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AttachedToParentCases
{
    class Program
    {
        static void Main(string[] args)
        {
            Task parentTask = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Parent Task...");

                #region 5 - ContinueWhenAll Kullanımı

                Task detachedTask2 = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("Detached Task 2");
                }
                );

                Task childTask6 = Task.Factory.ContinueWhenAll(new Task[] { detachedTask2 }, (t) =>
                    {
                        Console.WriteLine("Child Task 6");
                    }, TaskContinuationOptions.AttachedToParent);

                #endregion

                 Thread.Sleep(30000); //Debug modda Parallel Task' leri izlemek için konulmuştur
            });
            parentTask.Wait();
        }
    }
}
```

Bu kez yine Detached olarak tesis edilmiş bir Task örneği söz konusudur. Parent Task içerisine ilave etme işlemi için ise, ContinueWhenAll metodundan yararlanılmaktadır. Bir önceki örneğimizde olduğu gibi iki BerakPoint ile ilerlememizde yarar vardır. İşte sonuçlar.

![blg178_Region5_1Runtime.gif](images/blg178_Region5_1Runtime.gif)

Beklendiği üzere detachedTask2 kesinlikle Parent Task nesne örneğinin başlattığı yaşam döngüsüne dahil edilmemiştir. Ancak bu durum childTask6 nesne örneği için geçerli değildir.

![blg178_Region5_2Runtime.gif](images/blg178_Region5_2Runtime.gif)

Gelelim bir diğer kod parçamıza. Bu biraz ilginç bir deneyim olacak aslında

![Wink](images/smiley-wink.gif)

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AttachedToParentCases
{
    class Program
    {
        static void Main(string[] args)
        {
            Task parentTask = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Parent Task...");

                #region 6 - FromAsync

                Task detachedTask3 = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("detached task 3");
                }
                );

                Task childTask7 = Task.Factory.FromAsync(detachedTask3, (iar) =>
                {
                    Console.WriteLine("Child Task 7");
                }, TaskCreationOptions.AttachedToParent);

                
                #endregion

                Thread.Sleep(30000); //Debug modda Parallel Task' leri izlemek için konulmuştur
            });
            parentTask.Wait();
        }
    }
}
```

Söz konusu örnekte takip ettiğim kaynakların belirttiğine göre, childTask7 örneğinin Parent Task örneğine Attach olması beklenmektedir. Bakalım gerçekten böyle midir? Yine iki noktada BreakPoint koyarak çalışma zamanındaki durumu incelememizde yarar vardır. İşte ilk durum;

![blg178_Region6_1Runtime.gif](images/blg178_Region6_1Runtime.gif)

Beklediğimiz gibi detachedTask3 nesne örneği, Parent Task örneğinin yaşam döngüsüne ilave edilmemiştir. Kodun ilerleyen kısımlarında childTask7 nesne örneği üretilmeye çalışılmış ve bu amaçla FromAsync metodundan yararlanılmıştır. Metodun ilk parametresi detachedTask3 isimli nesne örneğidir. İkinci parametre olarak IAsyncResult arayüzü tipinden bir referansı parametre olarak alan isimsiz metod (AnonymousMethod) söz konusudur ve son parametre ile üretilen Task örneğinin Parent Task örneğine Attach edilmesi istenmektedir. Oysaki ikinci BreakPoint noktasında durum aşağıdaki gibidir.

![blg178_Region6_2Runtime.gif](images/blg178_Region6_2Runtime.gif)

3 numaralı ID değerine sahip olan Task, childTask7 nesne örneğini işaret etmektedir ve Parent değeri yoktur. Bir başka deyişle bu Task örneği, herhangibir Task örneğinin (Özellikle parentTask değişkeninin başlattığı) yaşam döngüsüne katılmamıştır. Bu durumu bende biraz garipsemiş durumdayım ve araştırmalarıma devam etmekteyim. Mutlaka gözden kaçırdığım bir yer olmalı diye düşünüyorum. Belki de siz bana bu konuda yardımcı olabilirsiniz. Nitekim şu anda benim de bir BreakPoint anında belirli bir süre beklemem gerekiyor.

![Smile](images/smiley-smile.gif)

Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[AttachedToParentCases.rar (26,72 kb)](assets/AttachedToParentCases.rar) [Örnek Visual Studio 2010 Ultimate sürümü üzerinde geliştirilmiş ve test edilmiştir]
