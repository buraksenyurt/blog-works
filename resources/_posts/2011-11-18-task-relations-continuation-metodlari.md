---
layout: post
title: "Task Relations–Continuation Metodları"
date: 2011-11-18 12:00:00 +0300
categories:
  - parallel-programming
tags:
  - task-parallel-library
  - parallel-programming
---
Böylesine yağmurlu ve sabah trafiğinin tavan yaptığı bir günde size ne Radyo Eksen’ deki güzel melodiler, ne de okuduğunuz mizah dergisindeki karikatürler iyi gelmiyorsa, başka bir şeyle uğraşmanın yeridir diyebilirim. Ben bu sıkıntıyı aşmak ve kendimi daha iyi hissetmek adına bir makale daha yazmaya karar verdim ve hemen Windows Live Writer programını açtım

[![1342533_gray_day_over_water](/assets/images/2011/1342533_gray_day_over_water_thumb.jpg)](/assets/images/2011/1342533_gray_day_over_water.jpg)


![Gülümseme](/assets/images/2011/wlEmoticon-smile_20.png)

Yanında da Paint.Net’ i. Bakalım bu gün menümüzde neler var?(Gerçi şöyle sahil kenarına gidip yürüyüşte yapabilirdim ama bu seferlik böyle olsun)

Microsoft, paralel programlama ile ilişkili olarak olabildiğince çok alternatifi düşünmüş ve kullanıma sunmuştur. Özellikle senaryo bazında düşündüğümüzde paralel çalışma algoritmalarından senkronizasyona kadar pek çok noktada bu durumu görmekteyiz. Söz gelimi Task’ ler arası ilişkiler (Relations) ve veri transferleri konusunda olabilecek tüm kombinasyonlar değerlendirilmeye çalışılmıştır. Task’ ler arası ilişkiler oldukça ilginç ve enteresan bir konudur. Gerçek hayat senaryolarında doğru ve uygun karşılıklarını bulmak her ne kadar zor olsa da en azından teorik olarak paralel programlamacıların konuyu bilmesi önemlidir. İşte bu yazımızda Task örnekleri arası ilişkiler konusuna değinmeye başlayacak ve ilk olarak Continuations seçeneklerini irdelemeye çalışacağız.

Continuations tekniklerinde, bir Task’ ın çalıştırılması veya icra edilmesi, atası olan ya da öncesinden tanımlanıp kendisine bağlanan Task örneklerine bağlıdır. Normal şartlar altında size tek bir Task örneği ve bu Task çalışmasını bitirdikten sonra devreye girmesi gereken bir Task örneğinin ele alındığı senaryoyu aktarmam gerekiyor. Ancak bana göre konunun daha iyi anlaşılabilmesi için aşağıdaki kod parçasında yer alan örneği göz önüne alarak başlamamız daha doğru olacaktır

![Göz kırpan gülümseme](/assets/images/2011/wlEmoticon-winkingsmile_71.png)

```csharp
using System; 
using System.Threading; 
using System.Threading.Tasks;

namespace TaskContinuation 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            #region Task Örnekleri

            Task<string> TaskA = new Task<string>(() => 
                { 
                    Thread.SpinWait(Int32.MaxValue-4000000); 
                    Console.WriteLine("Task A"); 
                    return "Dennis Ritchie"; 
               } 
            );

            Task<int> TaskB = new Task<int>(() => 
                { 
                    Thread.SpinWait(Int32.MaxValue - 5000000); 
                    Console.WriteLine("Task B"); 
                    return 10; 
                } 
            ); 
            Task<bool> TaskC = new Task<bool>(() => 
                { 
                    Thread.SpinWait(Int32.MaxValue - 10000000); 
                    Console.WriteLine("Task C"); 
                    return true; 
                } 
            );

            #endregion

            Task[] tasks={TaskA,TaskB,TaskC}; 
            // Succesor Task, TaskA,TaskB ve TaskC tamamlanıncaya kadar bekleyecektir 
            Task succesorTask = Task.Factory.ContinueWhenAll(tasks, (antecedentTasks) => 
            { 
                Console.WriteLine("Succesor Task"); 
                Console.WriteLine("{0}\n{1}\n{2}",TaskA.Result,TaskB.Result,TaskC.Result); 
            } 
            );

            TaskA.Start(); 
            TaskB.Start(); 
            TaskC.Start();

            Task.WaitAll(tasks); // TaskA, B ve C' nin tamamlanmasını bekle 
            succesorTask.Wait(); // succesorTask' ın tamamlanmasını bekle

            Console.WriteLine("Program Sonu"); 
            Console.ReadLine();        
        } 
    } 
}
```

Öncelikli olarak kod parçamızı kısaca inceleyelim. Senaryomuzda 4 adet Task örneği bulunmaktadır. TaskA, TaskB ve TaskC isimli Task nesne örnekleri geriye farklı tiplerde değerler döndürmektedir. Dikkat edilmesi gereken nokta succesorTask değişkeni ile tanımlanmış olan Task örneğinin oluşturulma şeklidir. Dikkat edileceği üzere Task.Factory.ContinueWhenAll metodu kullanılmıştır. Buna göre, ContinueWhenAll metodunun ilk parametresine verilen Task dizisine ait Task örnekleri tamamlanmadığı sürece, ikinci parametre ardından gelen Anonymous Metod içeriği çalıştırılmayacaktır

![Göz kırpan gülümseme](/assets/images/2011/wlEmoticon-winkingsmile_71.png)

Bir başka deyişle Succesor Task devreye girmeyecektir. Bu durumu uygulamayı çalışma zamanında Debug ederken daha net bir şekilde görebiliriz.

[![bei_33](/assets/images/2011/bei_33_thumb.gif)](/assets/images/2011/bei_33.gif)

Şekilden de görüleceği üzere Task örneklerinin her üçü de Start edilmiş ancak succesorTask’ ın o anki Status durumu WaitingForActivation olarak kalmıştır. Bunun sebebi, önceki Task örneklerinin tamamının işleyişini henüz bitirmemiş olmasıdır. Örnek uygulamamızın çalışma zamanındaki görüntüsü ise aşağıdaki gibi olacaktır.

[![bei_34](/assets/images/2011/bei_34_thumb.gif)](/assets/images/2011/bei_34.gif)

Görüldüğü üzere bir Task örneğinin, kendisinden önceki başka Task örneklerinin tamamının işleyişini bitirmesinden sonra devreye girmesi bekleniyorsa, ContinueWhenAll metodu kullanılabilir. Aslında bakarsanız daha gerçekçi senaryolara gitmek için Continue… metodlarının aldığı TaskContinuationOptions enum sabitinin değerlerine bakmakta yarar vardır. Çünkü bu Enum sabitinin değerleri, Succesor Task örneğinin hangi durumlarda devreye girmesi konusunda daha farklı bakış açılarının değerlendirilebilmesini sağlamaktadır. Enum sabitinin alabileceği değerler ise şunlardır.

- None
- AttachedToParent
- ExecuteSynchronously
- LongRunning
- NotOnCanceled
- NotOnFaulted
- NotOnRanToCompletion
- OnlyOnCanceled
- OnlyOnFaulted
- OnlyOnRanToCompletion
- PreferFairness

Konuyu daha net kavramak adına örnek uygulamamızdaki kodlarımızı biraz değiştirelim.

```csharp
using System; 
using System.Threading; 
using System.Threading.Tasks; 
using System.IO;

namespace TaskContinuation 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            #region Senaryo #2

            Task TaskA = new Task(() => 
                { 
                    Console.WriteLine("Adventure Servisi üzerinden işlemler"); 
                    throw new FileNotFoundException(); 
                } 
            );

            Task succesorTask = TaskA.ContinueWith( 
                (antecedentTasks) => 
                { 
                    Console.WriteLine("Bir hata oluştu. Rollback operasyonu yapılacak"); 
                } 
                , TaskContinuationOptions.OnlyOnFaulted 
                );

            TaskA.Start(); 
            try 
            { 
                TaskA.Wait(); 
            } 
            catch(AggregateException excp) 
            { 
                succesorTask.Wait(); 
            }

            #endregion

            Console.WriteLine("Program Sonu"); 
            Console.ReadLine();        
        } 
    } 
}
```

Şimdi bu senaryoda daha farklı bir durum söz konusudur. TaskA içerisinde bilinçli olarak bir Exception üretildiği görülmektedir. Tabiki gerçek hayat senaryosunda böyle bir olasık olma ihtimali olduğu göz önüne alınmalıdır. Diğer yandan TaskA üzerinden ContinueWith metodunu kullanarak succesorTask örneği oluşturulmakta ve OnlyOnFaulted enum sabiti değeri verilmektedir. Buna göre, succesorTask nesne örneğinin devreye girme durumu, bir önceki Antecedent Task örneği içerisinde bir Exception oluşması ve Faulted durumuna düşmesi halidir. Dolayısıyla örneğimizi çalıştırdığımızda aşağıdaki ekran görüntüsüne benzer bir sonuç ile karşılaşmamız son derece doğaldır.

[![bei_35](/assets/images/2011/bei_35_thumb.gif)](/assets/images/2011/bei_35.gif)

Diğer yandan throw Exception satırı yorum haline getirilir veya kaldırılırsa bu kez çalışma zamanı görüntüsü aşağıdaki gibi olacaktır.

[![bei_36](/assets/images/2011/bei_36_thumb.gif)](/assets/images/2011/bei_36.gif)

Görüldüğü üzere bir önceki Task örneğinde herhangibir Exception durumu söz konusu olmadığından, succesorTask örneğine ait metod icra edilmemiştir. Tabi burada akıllı bir geliştirici hemen şunu soracaktır; Birden fazla Task örneğinden herhangibirinde bir hata meydana geldiğinde ilgili Succesor Task devreye girse olmaz mı?

![Göz kırpan gülümseme](/assets/images/2011/wlEmoticon-winkingsmile_71.png)

Güzel soru…Bu vakayı test etmek için aşağıdaki kod parçasını deneyebiliriz.

```csharp
using System; 
using System.IO; 
using System.Threading.Tasks;

namespace TaskContinuation 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            #region Senaryo #2

            Task TaskA = new Task(() => 
                { 
                    Console.WriteLine("Adventure Servisi üzerinden işlemler"); 
                    //throw new FileNotFoundException(); 
                } 
            );

            Task TaskB = new Task(() => 
            { 
                Console.WriteLine("Northwind Servisi üzerinden işlemler"); 
                //throw new FileNotFoundException(); 
            } 
); 
            Task[] tasks = { TaskA, TaskB }; 
            Task succesorTask = Task.Factory.ContinueWhenAny(tasks, 
                (antecedentTasks) => 
                { 
                    Console.WriteLine("Bir hata oluştu. Rollback operasyonu yapılacak"); 
               } 
                , TaskContinuationOptions.OnlyOnFaulted  
                );

            TaskA.Start(); 
            TaskB.Start(); 
            try 
            { 
                Task.WaitAll(tasks); 
            } 
            catch(AggregateException excp) 
            { 
                succesorTask.Wait(); 
            }

            #endregion 
            
            Console.WriteLine("Program Sonu"); 
            Console.ReadLine();        
        } 
    } 
}
```

Ne yazık ki uygulamayı çalıştırdığımızda aşağıdaki ekran görüntüsünde yer alan Exception mesajını alırız

![Üzgün gülümseme](/assets/images/2011/wlEmoticon-sadsmile_8.png)

[![bei_37](/assets/images/2011/bei_37_thumb.gif)](/assets/images/2011/bei_37.gif)

Bu aslında TaskContinuationOptions enum sabitine verdiğimiz OnlyOnFaulted değeri için söz konusu bir durumdur. (Aslına bakarsanız ben bu senaryonun çalışmasını beklerdim ![Kafası karışmış gülümseme](/assets/images/2011/wlEmoticon-confusedsmile_11.png)) Diğer enum sabiti değerlerinde bu tip bir sorun ile karşılaşmasanız da OnlyOnFaulted hakkatten bir Fault vermektedir

![Açık ağızlı gülümseme](/assets/images/2011/wlEmoticon-openmouthedsmile_18.png)

Yazımızın buraya kadarki kısmında kısaca Task örnekleri arasındaki ilişkileri sağlamak adına Continous tekniklerini ve özellikle ContinueWhenAll ve ContinueWith metodlarını irdeledik. Bu iki metoda ek olarak ContinueWhenAny isimli bir metodun daha olduğunu belirtmek isterim. Bu metod aslında bir Task dizisi içerisindeki Task’ lerden herhangibiri tamamlandıktan sonra Succesor Task örneğinin devreye girmesi amacıyla tasarlanmıştır. Bu durumu analiz etmek için aşağıdaki kod parçasını göz önüne alabiliriz.

```csharp
using System; 
using System.Threading; 
using System.Threading.Tasks;

namespace TaskContinuation 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            #region Senaryo #3

            Task TaskA = new Task(() => 
           { 
                Thread.Sleep(4000); 
                Console.WriteLine("Adventure Servisi üzerinden işlemler"); 
                //throw new FileNotFoundException(); 
           } 
           );

            Task TaskB = new Task(() => 
            { 
                Thread.Sleep(2000); 
                Console.WriteLine("Northwind Servisi üzerinden işlemler"); 
                //throw new FileNotFoundException(); 
            } 
); 
            Task[] tasks = { TaskA, TaskB }; 
            Task succesorTask = Task.Factory.ContinueWhenAny(tasks, 
               (antecedentTasks) => 
                { 
                   Console.WriteLine("Succesor Task"); 
                } 
                );

            TaskA.Start(); 
            TaskB.Start(); 
            try 
            { 
                Task.WaitAll(tasks); 
            } 
            catch (AggregateException excp) 
            { 
                 
            } 
            succesorTask.Wait();

            #endregion

            Console.WriteLine("Program Sonu"); 
            Console.ReadLine();        
        } 
    } 
}
```

Uygulama kodunda yer alan TaskA ve TaskB nesne örneklerine ait kod bloklarından ilk olarak TaskB tamamlanacaktır (Verilen Thread durdurma süreleri gereği) Buna göre de çalışma zamanında TaskB tamamlanır tamamlanmaz Succesor Task bloğu yürütülecektir. Aşağıda görüldüğü gibi.

[![bei_38](/assets/images/2011/bei_38_thumb.gif)](/assets/images/2011/bei_38.gif)

Böylece geldik bir yazımızın daha sonra

![Gülümseme](/assets/images/2011/wlEmoticon-smile_20.png)

Bir sonraki yazıda büyük bir olasılıkla Task’ ler arası ilişkiler konusuna devam ediyor olacağım. Ama araya başka bir konu da girebilir. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[TaskContinuation.rar (22,47 kb)](/assets/files/2011/TaskContinuation.rar)
