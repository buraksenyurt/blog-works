---
layout: post
title: "TPL(Task Parallel Library) Nedir? [Beta 1]"
date: 2009-06-03 02:50:00 +0300
categories:
  - tpl
tags:
  - tpl
  - csharp
  - dotnet
  - linq
  - task-parallel-library
  - threading
  - concurrency
  - delegates
  - generics
---
Uzun uzun zaman önceydi. İlk bilgisayarımı daha dün gibi hatırlıyorum. Efsane Commodore 64.

![blg26_8.jpg](/assets/images/2009/blg26_8.jpg)

Açıkçası onunla yaptığım tek şey oyun oynamaktı itiraf ediyorum. En çok sevdiğim oyunlar arasında Grean Beret, Barbarian, Karate Kid 2, 1942, Airwolf vardı. Gel zaman git zaman, Üniversite yıllarına girince, bilgisayar işini daha ciddi düşünmeye başlamıştım. Yanlış hatırlamıyorsam yaklaşık olarak 2400 dolar değerinde (![Money mouth](/assets/images/2009/smiley-money-mouth.gif)) 486DX işlemcili bir bilgisayarım daha olmuştu.

![blg26_3.jpg](/assets/images/2009/blg26_3.jpg)

Sonrada olayların ardı arkası kesilmedi ve Pentium MMX, Celeron derken, çift çekirdekli ve hatta şu sıralarda moda olan 4 çekirdekli işlemciler ile karşılaştık.

![blg26_5.jpg](/assets/images/2009/blg26_5.jpg)

Özellikle işlemcilerin bu şekilde ilerlemesine paralel olarak, yazılım geliştirme ortamlarında da pekala pek çok değişiklik ve yenilikçi fikir ortaya çıktı. Son zamanların özellikle Microsoft.Net cephesindeki en popüler konularından biriside paralel genişletmeler (Parallel Extensions). Bir başka deyişle, sistemin sahip olduğu işlemci gücünün tümünü kullanarak (Arabanın hakkını ver hakkını

![Smile](/assets/images/2009/smiley-smile.gif)

), paralel işlemler veya eş zamanlı yürütmelerin gerçekleştirilmesi. Bildiğiniz gibi paralel genişletmelerin önemli kısımlarından birisi olan PLINQ (Parallel Language INtegrated Query) alt yapısı üzerine yaptığım araştırmalarımı ve edindiğim bilgileri bir süredir sizlerle paylaşmaktayım. İşte bu yazımızda diğer önemli parça olan (belkide ile etapta incelenmesi gereken) TPL (Task Parallel Library) alt yapısını incelemeye başlıyor olacağız.

TPL'in en büyük amacı, eş zamanlı veya paralel olarak yürütülmek istenen işlemlerin, daha kolay ve basit bir şekilde ele alınmasını sağlamaktır. Bu anlamda günümüz işlmecilerinin çekirdek sayısı veya sistemlerdeki işlemci sayısının birden fazla olması durumunda, TPL verimli sonuçlar elde etmemizi sağlamaktadır. Bu açıdan bakıldığında TPL alt yapısına tüm sistem çekirdek gücünü verme imkanına sahip olduğumuzu belirtebiliriz. Ancak elbetteki bu güç yanlış anlaşılmamalı ve kullanılmamalıdır. Bildiğiniz gibi "kontrolsüz güç, güç değildir" derler

![Wink](/assets/images/2009/smiley-wink.gif)

Elbetekki TPL kullanımı ile ilişkili olarak unutulmaması gereken bir noktada, işlemlerin Multi-Threading mantığına göre yapılıyor olmasıdır. Dolayısıyla, programın çalışma zamanı yükünü arttırıcı bir etkendir. Bir başka deyişle her işlemin, elimizde TPL var diye paralel olarak yürütülmeye çalışılması doğru değildir. Bazı süreçlerin gerçekten ve bilinçli olaraktan ardışık (Sequential) yürümesi gerekebilir.

NOT: Aslında, PLINQ (Parallel Language INtegrated Query) kendi alt yapısında TPL tipleri ve üyelerinden destek almaktadır.

Artık olaya biraz daha teknik açıdan bakabileceğimizi düşünüyorum. TPL esas itibariyle.Net Framework 4.0 ile birlikte gelen ve paralel işlem yapma yeteneklerini ele alan kütüphanedir. System.Threading ve System.Threading.Tasks isim alanları bu kütüphaneye ait çeşitli tipleri ve üyelerini içermektedir. TPL, içerisinde birde Task Scheduler içerir. Bu planlayıcı ThreadPool ile TPL tipleri arasındaki entegrasyonu sağlamaktadır. Ancak istenirse kendi özel görev planlayıcılarımızı yazabilir ve kullanabiliriz. Gerçi benim buna niyetim yok

![Embarassed](/assets/images/2009/smiley-embarassed.gif)

Aşağıdaki şekilde özellikle System.Threading.Tasks isim alanı altında yer alan tipler görülmektedir.

![blg26_1.gif](/assets/images/2009/blg26_1.gif)

Geliştirme sürecinde özellikle System.Threading.Tasks isim alanı altında yer alan tipler kullanılmaktadır. TPL kütüphanesinin belkide en önemli tipi Task sınıfıdır. Task sınıfına ait üyeler kullanılarak aşağıdaki işlemleri gerçekleştirebiliriz;

- Yeni görev (ler) başlatılabilir, iptal edilebilir yada bekletilebilir.
- Bir görevin tamamlanması halinde, tamamlandığı yerden başka bir görev veya görevlere çağrıda bulunulabilir.
- Başlatılan görevlerden geriye değer döndürülebilir.
- Bir görev kendi içinde alt görevler başlatabilir. Bu görevler aynı Thread içerisinde veya farklı bir Thread üzerinde çalışıyor olabilir.

Tüm teorik bilgiler bir yana, konuyu ilk etapta kavramının en kolay yolu basit bir örnek üzerinden ilerlemektir. Bu anlamda, TPL'e Hello World demenin belkide en kolay yolu, System.Threading isim alanı altında yer alan Parallel static sınıfı ve üyelerini kullanmaktır.

![blg26_4.gif](/assets/images/2009/blg26_4.gif)

Görüldüğü gibi For, ForEach ve Invoke isimli bizlere çok tanıdık gelen metodlar yer almaktadır. Bu fonskiyonellikleri kullanarak işlemlerin paralel olarak yürütülmesi sağlanabilmektedir. For ve ForEach metodları adlarındanda anlaşılacağı üzere, koleksiyon veya dizi yapıları üzerinde döngüsel işlemlerin paralel olarak yürütülmesini sağlamaktadır. Invoke metodu ise sunduğu Action temsilcisi (delegate) yardımıyla, birden fazla metodun aynı anda paralel olarak çalıştırılabilmesine olanak sağlamaktadır.

> For veya ForEach gibi Parallel sınıfına ait üyeleri kullandığımız hallerdede, arka planda Task sınıfı ve üyeleri gizlice devreye girerler.

Aşağıdaki Console uygulamasında bu metodalara ait örnek kullanımlar yer almaktadır.

```csharp
using System;
using System.Linq;
using System.Threading;

namespace HelloTPL
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] numbers = Enumerable.Range(1, 100000).ToArray();

            #region Parallel.For Örneği

            // Action temsilcisinin söylediği kurallara uygun olaraktan, lambda operatöründen yararlanılır.
            Console.WriteLine("For\n");
            Parallel.For(1, numbers.Length, 
                i =>
                {
                    if(i%1500==0)
                        Console.Write("{0} ",i.ToString());
                }
            );
            
            Console.WriteLine("\n\nFor(İçeriden başka metod çağırarak)\n");            
            Parallel.For(1,numbers.Length,
                (i)=>{
                    if (i % 1500 == 0)
                        Task1(i);
                }
            );

            #endregion

            #region Parallel.ForEach Örneği

            /* ForEach metodunun 19 aşırı yüklenmiş versiyonu vardır. İlk dikkati çeken nokta, IEnumerable<T> generic arayüzünü(interface) implemente eden referanslarıda parametre olarak almasıdır. Dolayısıyla, her koleksiyon veya diziye uygulanabilir. */
            Console.WriteLine("\n\nForEach Örneği\n");
            Parallel.ForEach(numbers, number =>
            {
                if (number % 1500 == 0)
                    Console.Write("{0} ", number.ToString());
            }
            );

            #endregion

            #region Parallel.Invoke Örneği

            Console.WriteLine("\n\n");

            // Parallel.Invoke metodu Action temsilcisi tipinden referanslar alan bir diziyi parametre olarak kullanır.
            // Bu şekilde istenildiği kadar metodun paralel olarak başlatılması sağlanabilir            
            Parallel.Invoke(
                () =>
                {
                    Console.WriteLine("Toplam Tek sayı hesabı başladı\n");
                    Console.WriteLine("Managed Thread ID {0} ",Thread.CurrentThread.ManagedThreadId.ToString());
                    OddCount(numbers);
                    Console.WriteLine("Toplam Tek sayı bulma işi tamamlandı\n");
                },
                () =>
                {
                    Console.WriteLine("Toplam Çift sayı hesabı başladı\n");
                    EvenCount(numbers);
                    Console.WriteLine("Managed Thread ID {0} ", Thread.CurrentThread.ManagedThreadId.ToString());
                    Console.WriteLine("Toplam Çift sayı bulma işi tamamlandı\n");
                }
                ,
                () =>
                {
                    Console.WriteLine("9 ile bölünenlerin toplamını bulma işi başladı\n");
                    NineCount(numbers);
                    Console.WriteLine("Managed Thread ID {0} ", Thread.CurrentThread.ManagedThreadId.ToString());
                    Console.WriteLine("9 ile bölünenlerin toplamını bulma işi tamamlandı\n");
                }
        );

            #endregion
        }
        
        static void Task1(int number)
        {
            // Değişiklik işlemler
            Console.Write("{0} ", number.ToString());                
        }

        static void EvenCount(int[] numbers)
        {
            int result = (from number in numbers
                          where number % 2 == 0
                          select number).Count();
            Console.WriteLine("\tDizi içerisinde {0} adet ÇİFT sayı vardır\n",result.ToString());
        }
        static void OddCount(int[] numbers)
        {
            int result = (from number in numbers
                          where number % 2 != 0
                          select number).Count();
            Console.WriteLine("\tDizi içerisinde {0} adet TEK sayı vardır\n", result.ToString());
        }

        static void NineCount(int[] numbers)
        {
            int result = (from number in numbers
                          where number % 9 == 0
                          select number).Count();
            Console.WriteLine("\tDizi içerisinde {0} adet 9 ile bölünebilen sayı vardır\n", result.ToString());
        }
    }
}
```

Aslında kod son derece açıktır ancak dikkat edilmesi gereken noktalarda vardır. For, ForEach ve Invoke metodları, Action temsilcisini sıklıkla kullanmaktadır. Bunun en büyük nedeni, paralel işleme tabi tutulacak kod parçalarını taşıyan herhangibir metod veya bloğun kullanılabilmesini sağlamaktır. Bildiğiniz gibi Action temsilcisi, parametre almayan ve geriye döndürmeyen metodları işaret etmektedir. Diğer taraftan Func temsilcisinin kullanıldığı versiyonlarda bulunmaktadır. Yani geriye değer döndüren ve parametre alan metodlarında işin içerisinde katılması sağlanabilir. Elbette kullanımı kolaylaştırmak adına, lambda operatörüde (=>) ciddi şekilde ele alınmaktadır. Uygulamayı çalıştırdığımızda aşağıdaki ekran görüntüsünde yer alan sonuçlara benzer bir çıktı elde ederiz.

![blg26_9.gif](/assets/images/2009/blg26_9.gif)

Tabiki kodu test ettiğimiz sistemin çekirdek veya işlemci sayısına göre, yada o anda çalışmakta olan programlara göre farklı sıralarda sonuçlar elde edilebilir. Ancak çalışan kod parçasında işlemlerin paralel yapıldığına dair pek çok iz vardır. Dikkatlice bakıldığında For, ForEach döngülerinin dizileri gerçekten paralel bir sırada değerlendirdiği ve işlemeri yaptığı ortadadır. Invoke metoduda benzer şekilde, çağırdığı 3 metodu mümkün mertebede paralel olarak başlatmıştır.

NOT: TPL tarafında geliştiricinin alt-seviye (Low-Level) işlemlerle uğraşmasına gerek yoktur. Ancak bu durum görsel programlama tarafında, Illegal Cross Thread Exception gibi istisnaların olmayacağı anlamına gelmemelidir ![Undecided](/assets/images/2009/smiley-undecided.gif)

Böylece geldik bir yazımızın daha sonuna. Bu yazımızda TPL alt yapısını en basit ve yalın haliyle tanımaya çalıştık. Elbetteki işimiz bitmedi. Bi dünya işimi var aslında

![Cool](/assets/images/2009/smiley-cool.gif)

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[HelloTPL.rar (25,96 kb)](/assets/files/2009/HelloTPL.rar)
