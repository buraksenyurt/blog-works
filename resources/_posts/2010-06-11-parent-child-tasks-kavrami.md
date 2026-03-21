---
layout: post
title: "Parent-Child Tasks Kavramı"
date: 2010-06-11 02:00:00 +0300
categories:
  - parallel-programming
  - tpl
tags:
  - task-parallel-library
  - parallel-programming
  - parallel-computing
---
Planlama gerçek hayatta her zaman karşımıza çıkan ve yaşamımızın, işlerimizin düzenli devam edebilmesi için gereken olmazsa olmazlar arasında yer alan bir kavramdır. Toplantıların planlanmasından tutun da, işlerin hangi sırada yapılacağına karar verilmesine kadar pek çok yerde planlamanın önemini görürüz. Aslında başarılı sistemlerin tasarlanması, çalışması ve istenen sonuçları üretmesi iyi planlamayla ilişkilidir. Tasarımın planlanması, kaynakların planlanması, sistemin önceliklerinin planlanması, müşteri toplantılarının planlanması vs...Bazen kafamızda hayatımızın ilerleyişini planlarız ve bazende yazdığımız kodun fonksiyonelliklerini.

![blg171_Giris.jpg](/assets/images/2010/blg171_Giris.jpg)

Dolayısıyla planlamanın yazılımın pek çok noktasında da son derece önemli bir rol üstlendiğine şahit oluruz. Aslında planlı olmak, neyin ne zaman nasıl ve ne şekilde yapılacağının bilinmesi, ortaya çıkacak sonuçlarda nasıl hareket edileceğinin tespit edilmesi noktasında son derece hayatidir.

Planlama, bu yazımızda ele alacağımız gibi Task Parallel Library içerisinde de ele alınan bir konudur. Özellikle Parallel.ForEach, Parallel.For, Parallel.Invoke gibi metodlar zaten paralel çalışma için gerekli planlamaları (yapılandırmaları ki neden yapılandırma dediğimi biraz sonra anlayacağız) kendi iç yapılarında gerçekleştiren fonksiyonellikler sunmaktadır.

Ancak Task nesne örneklerinin devreye girdiği noktada, Parent-Child ilişkiler kurularak Planlanmış/Yapılandırılmış görevlerin (Structured Tasks) oluşturulması da mümkündür.

> Gerçi Structured kelimesini Yapılandırılmış, Denetim Altında Olan anlamlarında da kullanabiliriz ancak burada mevzu bahis olan konu bana göre Task nesne örneklerinin planlı bir şekilde Parent-Child ilişki içerisine alınması ve çalıştırılmasıdır. Tabi ki evdeki hesap her zaman için çarşıya uymaz, uymayabilir. Bu nedenle Parent Task nesne örneklerinin n sayıda Child Task örneğini içeriyor olması da, bir yapının-Structure kurulması olarak düşünülebilir.

İşte bu yazımızda Task nesne örnekleri arasında Parent-Child ilişkiyi incelemeye çalışıyor olacağız. İşe ortam gereklilikleri ile başlamakta yarar olduğu kanısındayım.

Task nesne örnekleri arasında Parent-Child ilişki oluşturulabilmesi için gerekli iki şart vardır. İlk olarak Child olacak Task örneğinin, Parent Task örneğinin çalıştığı yaşam döngüsü (Life Cycle) içerisindeyken oluşturulması (Create) gerekmektedir. İkincil olarak Child Task örneklerinin oluşturulma işlemleri sırasında, TaskCreationOptions enum sabitlerinden AttachedToParent değeri ile üretilmesi gerekmektedir. Bir başka deyişle oluşturulan Task nesne örneğinin, içerisinde çalıştığı Task nesne örneğinin Child'ı olacağının bilinçli bir şekilde bildirilmesi gerekir. Nitekim normal şartlarda varsayılan olarak tüm Task nesne örnekleri Detached pozisyondadır. Çok doğal olarak Parent-Child Task ilişkisi için en az iki Task nesne örneğinin olması gerektiği ortadadır.

![Wink](/assets/images/2010/smiley-wink.gif)

Child Task örnekleri, Parent Task örneklerine dönüş durumlarının (Return States) değerlerine göre etkide bulunabilirler. TaskStatus enum sabiti için olası değerler bu noktada önem taşımaktadır. Aşağıdaki şemada olası durumların hangi zaman aralıklarında anlam kazandığı gösterilmeye çalışılmaktadır.

![blg171_SchemaNew.gif](/assets/images/2010/blg171_SchemaNew.gif)

Bir başka deyişe, Child Task nesne örnekleri dönüş değerlerine göre Parent örneklerin yaşam döngülerine (Life Cycle) etkilde bulunurlar diyebiliriz. Peki en basit haliyle Parent-Child Task ilişkisini canlandırmak istersek nasıl bir kod deseni oluşturmamız gerekir? Aşağıda Visual Studio 2010 Ultimate RC ortamında buna istinaden oluşturulmuş bir kod örneği görülmektedir.

```csharp
using System;
using System.Threading.Tasks;

namespace StructuredTasking
{
    class Program
    {
        static void Main(string[] args)
        {
            Task parentTask = Task.Factory.StartNew(
                () =>
                {
                    Console.WriteLine("Parent Task...");
                    Task childTaskOne = Task.Factory.StartNew(() => { Console.WriteLine("\tChild Task 1"); }, TaskCreationOptions.AttachedToParent);
                    Task childTaskTwo = Task.Factory.StartNew(() => { Console.WriteLine("\tChild Task 2"); }, TaskCreationOptions.AttachedToParent);
                    Task childTaskThree = Task.Factory.StartNew(() => { Console.WriteLine("\tChild Task 3"); }, TaskCreationOptions.AttachedToParent);
                }
            );
            parentTask.Wait();
            Console.WriteLine("İşlemlerin sonu");
        }
    }
}
```

Kod parçasında dikkat edileceği üzere childTaskOne, childTaskTwo ve childTaskThree isimli Task nesne örnekleri parentTask nesne örneğinin oluşturulduğu metod içerisinde üretilmiş ve başlatışmıştır. Buna göre çalışma zamanında (Runtime) aşağıdakine benzer bir sonuç ile karşılaşılabilir.

![blg171_Runtime1.gif](/assets/images/2010/blg171_Runtime1.gif)

Karşılaşılabilir diyoruz, nitekim Parent Task nesne örneğine dahil olan Child Task nesne örnekleri farklı sıralarda çalıştırılabilirler. Örneğimizde Task sırası 1,3,2 şeklindedir ama bu sıra sabit ve kesin değildir. Yani 3,2,1 veya 3,1,2 vb sonuçlar elde edilebilir.(Toplamda 6 farklı sonuç olabileceğini de ifade edebiliriz![Smile](/assets/images/2010/smiley-smile.gif))

Biraz önce Child Task nesne örneklerinin Parent Task örneğine dahil olmaları için AttachedToParent sabit değerini belirtmeleri gerektiğini söylemiştik. Aksi durumda varsayılan olarak Detached olduklarını ifade etmiştik. Yukarıdaki kod parçasında AttachedToParent değerlerini kullanmassak aşağıdaki ekran görüntülerinde yer alanlara benzer sonuçlar ile karşılaşırız.

İlk deneme;

![blg171_Test1.gif](/assets/images/2010/blg171_Test1.gif)

İkinci deneme;

![blg171_Test2.gif](/assets/images/2010/blg171_Test2.gif)

Üçüncü deneme;

![blg171_Test3.gif](/assets/images/2010/blg171_Test3.gif)

Dördüncü deneme;

![blg171_Test4.gif](/assets/images/2010/blg171_Test4.gif)

Görüldüğü gibi bilinçli olarak bir planlama belirtilmediğinden Task örneklerinin yapılandırılmasında sorunlar oluşmuş, bazı çalışmalarda bazı Task örneklerinin sonuçları alınamamıştır.(3 ve 4 teki durumlar)

Parent-Child Task senaryolarında dikkat edilmesi gereken hususlardan birisi de, Parent task nesne örneğinin Final State zaman dilimine girebilmesi için, içerdiği Child Task nesne örneklerinin çalışmalarını tamamlamış olmaları gerektiğidir. Bu durumu analiz etmek için aşağıdaki kod parçasını göz önüne alabiliriz.

```csharp
using System;
using System.Threading.Tasks;
using System.Threading;

namespace StructuredTasking
{
    class Program
    {
        static void Main(string[] args)
        {
            Task parentTask = Task.Factory.StartNew(
                () =>
                {
                    Console.WriteLine("Parent Task...");

                    #region IsCompleted

                    Task childTaskOne = Task.Factory.StartNew(() => { 
                        Console.WriteLine("\tChild Task 1 Başladı");
                        Thread.Sleep(2000);
                        Console.WriteLine("\tChild Task 1 Bitti");
                    }, TaskCreationOptions.AttachedToParent);
                    Task childTaskTwo = Task.Factory.StartNew(() => {
                        Console.WriteLine("\tChild Task 2 Başladı");                        
                        Thread.Sleep(3000);
                        Console.WriteLine("\tChild Task 2 Bitti");
                    }, TaskCreationOptions.AttachedToParent);
                    Task childTaskThree = Task.Factory.StartNew(() => { 
                        Console.WriteLine("\tChild Task 3 Başladı");
                        Thread.Sleep(5000);
                        Console.WriteLine("\tChild Task 3 Bitti");
                    }, TaskCreationOptions.AttachedToParent);

                    #endregion
                }
            );

            while (!parentTask.IsCompleted)
            {
                Console.WriteLine("{0} Parent Task Durumu : {1}",DateTime.Now.ToLongTimeString(),parentTask.Status);
                Thread.Sleep(500);
            }
            Console.WriteLine("{0} Parent Task Durumu : {1}", DateTime.Now.ToLongTimeString(), parentTask.Status);
            Console.WriteLine("İşlemlerin sonu");
        }
    }
}
```

Kodu çalıştırdığımızda aşağıdakine benzer sonuçlar ile karşılaşırız. Child Task'ler içerisinde bilinçli olarak Thread.Sleep metodu kullanılmış ve uygulamanın belirli süreler boyunca duraksatılması sağlanmıştır. while döngüsünde ise Parent Task nesne örneğinin tamamlanıp tamamlanmadığı sürekli olarak denetlenmektedir.

![blg171_WaitFor.gif](/assets/images/2010/blg171_WaitFor.gif)

Dikkat edileceği üzere parentTask nesne örneğinin RanToCompletion moduna, bir başka deyişle Final State'e geçmesi için Child Task nesne örneklerinin tamamının bitmesi beklenmiştir. Üstelik Child Task nesne örnekleri çalışmalarını sürdürürken Parent Task nesne örneğine kendisini beklemesini bildirmektedir ki bu durumda Parent Task ara zaman diliminde durmaktadır ve bu nedenle WaitingForChildrenToComplete modundadır.

Parent Task nesne örneğinin Final State zaman dilimine girmesi anında olası 3 Status değeri bulunmaktadır. Şemamızdan hatırlayacağınız üzere bunlar RanToCompletion, Canceled ve Faulted olarak belirlenmiştir. Child Task örneklerinin başarılı bir şekilde tamamlanmış olmaları, Parent Task örneğinin RanToCompletion değeri üretmesi için yeterlidir. Peki ya Child Task nesne örneklerinden herhangibirinin başlattığı kod içerisinden çalışma zamanına bir Exception fırlatılırsa?

![Wink](/assets/images/2010/smiley-wink.gif)

Söz gelimi bir önceki örnek kodumuzda yer alan Child Task örneklerinden birisinden bir Exception fırlattığımızı ve bunu yakalamak istediğimizi düşünelim.

```csharp
Task childTaskOne = Task.Factory.StartNew(() => { 
                        Console.WriteLine("\tChild Task 1 Başladı");
                        Thread.Sleep(2000);
                        throw new Exception("Muahahahaha!");
                    }, TaskCreationOptions.AttachedToParent);

...

while (!parentTask.IsCompleted)
            {
                Console.WriteLine("{0} Parent Task Durumu : {1} Exception : {2}", DateTime.Now.ToLongTimeString(), parentTask.Status, parentTask.Exception);
                Thread.Sleep(500);
            }

            Console.WriteLine("{0} Parent Task Durumu : {1} Exception : {2}", DateTime.Now.ToLongTimeString(), parentTask.Status,parentTask.Exception);
            Console.WriteLine("İşlemlerin sonu");
```

Bu durumda uygulama aşağıdaki örnek çıktıyı verecektir.

![blg171_ExceptionNew.gif](/assets/images/2010/blg171_ExceptionNew.gif)

Dikkat edilmesi gereken 3 önemli nokta vardır. Parent Task nesne örneği Faulted Status değerini üreterek sonlanmıştır. Diğer yandan ilk Child Task, bir Exception ile sonlandırılsa bile diğer Task'lerin başlattığı işler yürütülmeyi sürdürmüş ve tamamlanmıştır. Bu dikkat edilmesi gereken bir durumdur. Nitekim Child Task'ler ortak bir takım verileri etkiliyor olabilirler ki bu durumda herhangibir istisna, diğer Task'lerin yanlış veriler üzerinden işlem yapmaya devam etmesine neden olabilir. Üçüncü nokta ise Parent Task nesne örneğinin Exception özelliğinin değeridir. Yukarıdaki senaryoya göre bu özellik, Parent Task nesne örneği WaitingForChildrenToComplete durumundayken null değer döndürmektedir. Bir başka deyişle Child Task içerisinden fırlatılan Exception nesnesi aslında bu zaman dilimi içerisinde Parent Task örneğine bildirilmemektedir. Ancak Faulted durumuna düşüldükten sonra söz konusu Exception nesnesi yakalanmaktadır.

Son olarak Parent-Child ilişki ile ilgili olarak şu notu düşebiliriz; Parent Task nesne örnekleri, kaç adet Child Task örneği içerdiğini bilmektedir. Bir başka deyişle kendisine eklenen Child Task'lerin sayısını tutar. Dolayısıyla Child Task nesne örneklerinin, dahil oldukları Parent Task örneğine bir referans bildiriminde bulunduğunu ve hatta tamamlanma durumlarını ilettiklerini ifade edebiliriz. Parent-Child Task'ler arasındaki ilişkiyi fırsat buldukça incelemeye devam ediyor olacağız. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[StructuredTasking_RTM.rar (23,32 kb)](/assets/files/2010/StructuredTasking_RTM.rar) [Örnek Visual Studio 2010 Ultimate RTM Sürümü üzerinde geliştirilmiş ve test edilmiştir]
