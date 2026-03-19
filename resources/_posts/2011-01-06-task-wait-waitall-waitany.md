---
layout: post
title: "Task Wait,WaitAll,WaitAny"
date: 2011-01-06 13:25:00 +0300
description: "Task Süreçlerinde Bilinçli Olarak Duraksatma başlıklı bir önceki yazımızda CancellationToken.WaitHandle.WaitOne, Thread.Sleep ve Thread.SpinWait metodlarından yararlanarak bir Task çalışmasının bekletme işlemlerinin nasıl yapılabileceğini incelemeye çalışmıştık."
categories:
  - parallel-programming
tags:
  - parallel-programming
  - csharp
  - linq
  - task-parallel-library
  - threading
  - generics
  - visual-studio
---
# Task Wait,WaitAll,WaitAny
[![blg207_Giris](/assets/images/2011/blg207_Giris_thumb.jpg)](/assets/images/2011/blg207_Giris.jpg) Merhaba Arkadaşlar,

[Task Süreçlerinde Bilinçli Olarak Duraksatma](../2010/Task Süreçlerinde Bilinçli Olarak Duraksatma.md) başlıklı bir önceki yazımızda CancellationToken.WaitHandle.WaitOne, Thread.Sleep ve Thread.SpinWait metodlarından yararlanarak bir Task çalışmasının bekletme işlemlerinin nasıl yapılabileceğini incelemeye çalışmıştık. Özellikle WaitOne metodunun, CancellationToken.WaitHandle özeliği üzerinden çalıştırıldığını unutmayalım. Diğer yandan tüm bu teknikleri Task gövdesi içerisinde gerçekleştirmiştik. Bunun doğal sonucu olarakta yürütülmekte olan Task işlevlerinin duraksatılmasını sağlamıştık.

Ancak incelediğimiz bu teknikler dışında, Task nesne örnekleri veya Task sınıfı üzerinden kullanılabilecek farklı bekletme teknikleri de söz konusudur.

Aslında Task örnekleri üzerinden Result değerlerinin okunmaya çalışılması, söz konusu Task’ in işleyişini tamamlayıncaya kadar, çağıran uygulamanın bekletilmesi anlamına gelmektedir. Fakat bunun dışında kullanılabilecek Wait, WaitAll ve WaitAny gibi metodlar söz konusudur. Tüm bu metodların kullanımında amaç, Task örneğini/örneklerini başlatan uygulamanın, belirtilen şartlar doğrultusunda bekletilmesini sağlamaktır. Bir önceki yazımızda incelediğimiz konular düşünüldüğünde bu önemli bir farktır. Bu seferki hedefimiz çağıran uygulamanın/metodun duraksatılmasıdır. Dilerseniz söz konusu modellere ait örnek uygulamalarımızı geliştirerek ilerleyelim.

## Wait Tekniği

Bu modelde, Task nesne örneği üzerinden çağırılan Wait metodu ile, Task örneğini başlatan fonksiyon veya uygulamanın duraksatılması söz konusudur. Aslında Task örneğini başlatan en azından uygulamanın ana Thread’ idir. Dolayısıyla Wait, Main Thread’ in duraksatılmasına neden olmaktadır. Durumu daha açık bir şekilde kavrayabilmek adına aşağıdaki kod parçasını göz önüne alalım.

```csharp
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading; 
using System.Threading.Tasks;

namespace WaitingScenarios2 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            IEnumerable<int> numbers = Enumerable.Range(0, 100000000);

            CancellationTokenSource tokenSource = new CancellationTokenSource(); 
            CancellationToken token = tokenSource.Token;

            Task startedTask = Task.Factory.StartNew(() => 
            { 
                for (int i = 0; i < numbers.Count(); i++) 
                { 
                    token.ThrowIfCancellationRequested(); 
                    i++; 
                    i--; 
                    i *= 2; 
                    Console.Write("."); 
                } 
            } 
            , token);

            token.Register(() => 
            { 
                Console.WriteLine("İşlem iptali"); 
            } 
            ); 
            Console.WriteLine("İşlemler devam ediyor. İlk duraksatma için bir tuşa basın."); 
            Console.ReadLine();

            Console.WriteLine("TimeSpan.FromSeconds(5) duraksatması. Time : {0}",DateTime.Now.ToLongTimeString()); 
            bool waitStatus=startedTask.Wait(TimeSpan.FromSeconds(5)); 
            Console.WriteLine("Timespan.FromSeconds(5) duraksatması bitti Time : {0}\nTask tamamlanmış mı ? {1}\nİkinci duraksatma için bir tuşa basın.",DateTime.Now.ToLongTimeString(),waitStatus); 
            Console.ReadLine();

            Console.WriteLine("10 saniyelik duraksatma. Time : {0}", DateTime.Now.ToLongTimeString()); 
            waitStatus=startedTask.Wait(10000); 
            Console.WriteLine("10 saniyelik duraksatma bitti. Time : {0}\nTask tamamlanmış mı? {1}\nİşlemler devam ediyor. İptal etmek için bir tuşa basınız.",DateTime.Now.ToLongTimeString(), waitStatus);        
            Console.ReadLine();

            tokenSource.Cancel();

            Console.ReadLine(); 
            Console.WriteLine("Task 1 Status = {0}", startedTask.Status);

        } 
    } 
}
```

Tedbiri elden bırakmadık ve yine bir iptal işlemini işin içerisine kattık. Ancak odaklanacağımız nokta tabiki bu değil. Kodun iki farklı satırında Wait metodunun kullanıldığını görmekteyiz. İlk kullanımda o anki andan itibaren 5 saniyelik bir duraksatma gerçekleştiriyoruz. Diğer metod kullanımından ise 10000 mili saniyelik süre bildirimi yaparak 10 saniyelik bir duraksatma icra ettirmekteyiz. Wait metodu bool tipinden değer döndürmektedir. False değer dönmesi, ilgili Task bloğunun herhangibir sebepten işyelişini tamamlamamış olması veya iptal edilmesi anlamına gelmektedir. Tahmin edileceği üzere Task’ in başarılı bir şekilde tamamlanması sonucu bu değer True olacaktır. Örneğimizi çalıştırdığımızda, aşağıdaki ekran görüntüsündekine benzer sonuçlar ile karşılaşabiliriz.

[![blg207_WaitOneTest1](/assets/images/2011/blg207_WaitOneTest1_thumb.gif)](/assets/images/2011/blg207_WaitOneTest1.gif)

Dikkat edileceği üzere duraksatmalar da bekleyen aslında Console uygulamasının kendisidir. Yani Main Thread’ dir. Ana uygulama, belirtilen süreler boyunca duraksamıştır. Lakin, Task gövdesi içerisinden başlatılan for döngüsü, bu duraksatmalar sırasında işleyişine devam etmiştir.

Wait metodunun aşırı yüklenmiş versiyonlarına bakıldığında parametre almayan bir versiyonunun daha olduğu görülmektedir. Sıklıkla kullanılan bu versiyona göre ana uygulama, söz konusu Task örneği işleyişini tamamlayıncaya kadar duraksatılacaktır. Wait () kullanımına ilişkin örnek kod parçası aşağıdaki gibidir.

```csharp
IEnumerable<int> numbers = Enumerable.Range(0, 10000000);

Task startedTask = Task.Factory.StartNew(() => 
{ 
	for (int i = 0; i < numbers.Count(); i++) 
	{ 
		i++; 
		i--; 
		i *= 2; 
		Console.Write("."); 
	} 
} 
);

Console.WriteLine("Wait()"); 
startedTask.Wait();                  
Console.WriteLine("\nTask 1 Status = {0}", startedTask.Status);
```

Değer aralığı bilinçli olarak küçültülmüştür. Çok fazla beklememek için. Burada dikkat edilmesi gereken noktalardan birisi de, parametre almayan Wait metodunun geriye bool bir değer döndürmeyişidir. Task örneğinin işlettiği kod parçasında herhangibir istisna oluşmadığı varsayıldığında, ana uygulmanın, işleyiş tamamlanıncaya kadar beklemesi söz konusudur.

[![blg207_WaitOneTest2](/assets/images/2011/blg207_WaitOneTest2_thumb.gif)](/assets/images/2011/blg207_WaitOneTest2.gif)

WaitAll Kullanımı

Önceki örneklerimizde tek bir Task nesne örneği üzerinden, çağıran uygulamanın duraksatılması işlemi gerçekleştirilmiştir. Bir başka deyişle çağıran uygulama, tek bir Task nesne örneği için belirli/belirsiz süre bekletilmiştir. Ancak doğal olarak birden fazla Task örneğinin yer aldığı bir senaryoda, tüm Task’ ler için ana uygulamanın bekletilmesi de istenebilir. Hatta bu teknikte temel amaç, birden fazla Task örneğinin işleyişi tamamlanıncaya kodun ilerlememesidir. Bu durumda Task sınıfının static WaitAll metodundan yararlanılabilir. İşte örnek kod parçamız.

```csharp
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading; 
using System.Threading.Tasks;

namespace WaitingScenarios2 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            IEnumerable<int> numbers = Enumerable.Range(0, 100000000); 
            IEnumerable<int> numbers2 = Enumerable.Range(0, 150000000);

            Task startedTask = Task.Factory.StartNew(() => 
            { 
                for (int i = 0; i < numbers.Count(); i++) 
                { 
                    i++; 
                    i--; 
                    i *= 2; 
                    Console.Write("."); 
                } 
            } 
            );

            Task startedTask2 = Task.Factory.StartNew(() => 
            { 
                for (int i = 0; i < numbers2.Count(); i++) 
                { 
                    i++; 
                    i--; 
                    i *= 2; 
                    Console.Write("+"); 
                } 
            } 
            );

            Console.WriteLine("WaitAll Çağrısı. Time : {0}",DateTime.Now.ToLongTimeString()); 
            Task.WaitAll(startedTask, startedTask2); 
            Console.WriteLine("\nWaitAllÇağrısı sonrasındaki satır geçiş zamanı : {0}",DateTime.Now.ToLongTimeString()); 
            Console.WriteLine("\nTask 1 Status = {0}\nTask 2 Status = {0}", startedTask.Status,startedTask2.Status); 
        } 
    } 
}
```

Örnekte iki Task örneği üretilmiş ve başlatılmıştır. Sonrasında ise Task sınıfı üzerinden static WaitAll metoduna parametre olarak aktarılmışlardır. Bunun sonucu olarak ana uyglamaya ait iş parçası (Main Thread), ilgili Task işleyişleri tamamlanıncaya kadar duraksatılmaktadır. Örnek kod parçasında Cancel işlemi ele alınmamıştır. Ancak görsel bir uygulama söz konusu olduğunda, kullanıcının Task örneklerinin işleyişleri devam ederken iptal isteğinin gönderilebileceği de göz önüne alınmalıdır. Örnek kod parçasının çalışma zamanı çıktısı aşağıdaki gibi olacaktır.

[![blg207_WaitAllTest1](/assets/images/2011/blg207_WaitAllTest1_thumb.gif)](/assets/images/2011/blg207_WaitAllTest1.gif)

WaitAll metodunda istenirse sürede belirtilebilir. Bir başka deyişle parametre olarak gelen bir Task kümesinin çalışması sonucu, yürütücü uygulamanın belirli süre duraksatılması sağlanabilir. Bu durumu analiz etmek için aşağıdaki kod parçasını göz önüne alabiliriz.

```csharp
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading; 
using System.Threading.Tasks;

namespace WaitingScenarios2 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            CancellationTokenSource tokenSource = new CancellationTokenSource(); 
            CancellationToken token = tokenSource.Token;

            IEnumerable<int> numbers = Enumerable.Range(0, 100000000); 
            IEnumerable<int> numbers2 = Enumerable.Range(0, 150000000);

            Task startedTask = Task.Factory.StartNew(() => 
            { 
                for (int i = 0; i < numbers.Count(); i++) 
                { 
                    token.ThrowIfCancellationRequested(); 
                    i++; 
                    i--; 
                    i *= 2; 
                    Console.Write("."); 
                } 
            } 
            ,token);

            Task startedTask2 = Task.Factory.StartNew(() => 
            { 
                for (int i = 0; i < numbers2.Count(); i++) 
                { 
                    token.ThrowIfCancellationRequested(); 
                    i++; 
                    i--; 
                    i *= 2; 
                    Console.Write("+"); 
                } 
            } 
            ,token 
            );

            token.Register(() => 
                { 
                    Console.WriteLine("İptal"); 
                } 
            );

            Console.WriteLine("WaitAll Çağrısı. Time : {0}", DateTime.Now.ToLongTimeString()); 
            Task[] tasks = { startedTask, startedTask2 }; 
            Task.WaitAll(tasks,5000); 
            Console.WriteLine("\nWaitAllÇağrısı sonrasındaki satır geçiş zamanı : {0}", DateTime.Now.ToLongTimeString()); 
            Console.WriteLine("\nTask 1 Status = {0}\nTask 2 Status = {0}\nİptal etmek için bir tuşa basınız.", startedTask.Status, startedTask2.Status); 
            Console.ReadLine(); 
            tokenSource.Cancel(); 
            Console.ReadLine();

            Console.WriteLine("\nTask 1 Status = {0}\nTask 2 Status = {0}", startedTask.Status, startedTask2.Status); 
        } 
    } 
}
```

Örnekte WaitAll metoduna tasks isimli Task[] dizisi aktarılmış ve 5000 milisaniyelik duraksatma değeri verilmiştir. Ayrıca iptal işlemi de yapılabilmektedir. Örnek bir çalışma zamanı çıktısı aşağıdaki gibidir.

[![blg207_WaitAllTest2](/assets/images/2011/blg207_WaitAllTest2_thumb.gif)](/assets/images/2011/blg207_WaitAllTest2.gif)

Dikkat edileceği üzere WaitAll çağrısı sonrasındaki satıra geçiş süresi, 5 saniyelik bir duraksamaya neden olmuştur. Tabi ki bu aralıkta Task örneklerinin gövdeleri işleyişlerini sürdürmektedir. WaitAll çağrısı aşıldıktan sonraysa, Task örnekleri henüz çalışmalarını tamamlamadıklarından Running durumunda kalmışlardır. Ki işleyişleri de devam etmektedir. Burada kullanıcı isterse iptal işlemini gerçekleştirebilir ki örneğimizde bu senaryo icra edilmiştir. Dolayısıyla Task örneklerinin her ikisi içinde Status değerleri Canceled olmuştur.

Gelelim diğer bir duraksatma tekniğine.

## WaitAny Kullanımı

WaitAny metodu ile bir Task topluluğundan herhangibiri için, çağırıcının bekletilmesi sağlanabilir. Task sınıfı üzerinden çağırılabilen static WaitAny metodu, parametre olarak gelen Task örneklerinden hangisi tamamlanmışsa, bu örneğin dizi içerisindeki indis değerini döndürür. –1 değer döndürmesi, zaman aşımına uğrandığını veya CancellationToken üzerinden bir iptal talebi gerçekleştirildiğini ifade etmektedir. Ancak bu değerin alınabilmesi içinde, WaitAny metodunun uygun olan aşırı yüklenmiş (Overload) versiyonunun kullanılması gerekmektedir. Dilerseniz konuyu daha net kavrayabilmek adına aşağıdaki örnek kod parçasını göz önüne alalım.

```csharp
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading; 
using System.Threading.Tasks;

namespace WaitingScenarios2 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        {            
            IEnumerable<int> numbers = Enumerable.Range(0, 60000000); 
            IEnumerable<int> numbers2 = Enumerable.Range(0, 15000000); 
            IEnumerable<int> numbers3 = Enumerable.Range(0, 45000000);

            Task startedTask = Task.Factory.StartNew(() => 
            { 
                for (int i = 0; i < numbers.Count(); i++) 
                { 
                    i++; 
                    i--; 
                    i *= 2; 
                    Console.Write("."); 
                } 
            });

            Task startedTask2 = Task.Factory.StartNew(() => 
            { 
                for (int i = 0; i < numbers2.Count(); i++) 
                { 
                    i++; 
                    i--; 
                    i *= 2; 
                    Console.Write("+"); 
                } 
            } 
            );

            Task startedTask3 = Task.Factory.StartNew(() => 
            { 
                for (int i = 0; i < numbers3.Count(); i++) 
                { 
                    i++; 
                    i--; 
                    i *= 2; 
                    Console.Write("/"); 
                } 
            } 
            );

            Task[] tasks = { startedTask, startedTask2, startedTask3 }; 
            Console.WriteLine("WaitAny Çağrısı. Time : {0}", DateTime.Now.ToLongTimeString());          
            int completedTaskIndex = Task.WaitAny(tasks); 
            Console.Write("\nİlk tamamlanan Task : {0}. Durumu {1}\n",completedTaskIndex,tasks[completedTaskIndex].Status); 
            Console.ReadLine(); 
        } 
    } 
}
```

Kod içerisinde 3 farklı Task örneğinin çalıştırıldığı görülmektedir. Bu işlemler sonrasında WaitAny metodu çağırılmış ve ana uygulama Thread’ inin duraksatılması sağlanmıştır. Örneği çalıştırdığımızda aşağıdaki ekran çıktısındakine benzer bir sonuç ile karşılaşmamız olasıdır.

[![blg207_WaitAnyTest1](/assets/images/2011/blg207_WaitAnyTest1_thumb.gif)](/assets/images/2011/blg207_WaitAnyTest1.gif)

Burada dikkat edilmesi gereken nokta, üç Task nesne örneğinden ilk olarak hangisi bitmişse, WaitAny metodunun ona ait indis değerini döndürmesidir. Örneğimizde dizi içerisinde ikinci sırada yer alan, bir başka deyişle 1 numaralı index değerine sahip olan Task gövdesi ilk tamamlanan içeriktir. Dolayısıyla WaitAny metodu geriye 1 değerini döndürmektedir. Geri dönüşten sonra fark edileceği üzere henüz tamamlanmayan Task örnekleri çalışmalarına devam edecektir.

[![Exclamation](/assets/images/2011/Exclamation_thumb_1.gif)](/assets/images/2011/Exclamation_1.gif) Örnekleri daha iyi kavrayabilmek adına mutlaka çalıştırıp test etmenizi, debug zamanında durarak anlık durumları incelemenizi öneririm.

Böylece geldik bir yazımızın daha sonuna. Bu yazımızda Task örneklerinin sahibi olan çalıştırıcıların (Ana uygulama Thread’ i gibi) nasıl bekletilebileceklerini inclemeye çalıştık. Tabi bu örneklerde istisna fırlatılmasına yönelik vakaları değerlendirmedik. Ancak bu tip durumlarında incelenmesi gerekmektedir. İşte size güzel bir araştırma konusu. Task Parallel Library ile ilişkili kavramları incelemeye devam ediyor olacağız. Bir sonraki yazımızda görüşünceye dek hepinize mutlu günler dilerim.

[WaitingScenarios2.rar (23,05 kb)](/assets/files/2011/WaitingScenarios2.rar) [Örnek Visual Studio 2010 Ultimate sürümü üzerinde geliştirilmiş ve test edilmiştir]