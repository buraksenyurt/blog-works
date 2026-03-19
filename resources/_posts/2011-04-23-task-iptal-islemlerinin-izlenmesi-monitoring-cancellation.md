---
layout: post
title: "Task İptal İşlemlerinin İzlenmesi(Monitoring Cancellation)"
date: 2011-04-23 12:10:00 +0300
description: "Bu yazımızda daha önceden .Net Framework Beta 1 ve Beta 2 sürümlerinde incelediğimiz Task iptal işlemlerini son sürümde ele alıp toparlamaya çalışıyor olacağız."
categories:
  - parallel-programming
tags:
  - parallel-programming
  - csharp
  - dotnet
  - linq
  - threading
  - delegates
  - generics
  - visual-studio
---
# Task İptal İşlemlerinin İzlenmesi(Monitoring Cancellation)
[![blg204_Giris](/assets/images/2011/blg204_Giris_thumb.jpg)](/assets/images/2011/blg204_Giris.jpg) Merhaba Arkadaşlar,

Bu yazımızda daha önceden.Net Framework Beta 1 ve Beta 2 sürümlerinde incelediğimiz Task iptal işlemlerini son sürümde ele alıp toparlamaya çalışıyor olacağız. Task iptal işlemleri oldukça önemli ve üzerinde titizlikle durulması gereken bir konudur. Nitekim bazı hallerde çalıştırılmakta olan Task işlevlerinin iptal edilmesi gerekebilir. Bu iptal işlemi, sistem tarafından her hangibir koşulun gerçeklenmesi sonucu talep edilebileceği gibi, kullanıcı tarafından da uygulatılmak istenebilir.

Task örnekleri işaret ettikleri ve planladıkları fonksiyonellikleri çalıştırdıklarında ve herhangibir sebeple iptal işlemi gerçekleştirilmek istendiğinde CancellationTokenSource ve CancellationToken tiplerinden yararlanılması gerekmektedir.

Bu amaçla İptal işlemlerinde CancellationTokenSource nesne örnekleri üzerinden Cancel metodunun çağırılması yeterlidir. Ancak burada da dikkat edilmesi gereken bir husus vardır. Çalışma zamanı Cancel çağrısı sonucu ilgili Task örneklerinin işleyişlerini otomatik olarak sonlandırmaz. Bir başka deyişle Task gövdeleri veya paralel yürütülen metod blokları içerisinde Cancel işleminin talep edilip edilmediğinin takibi gerekmektedir (Monitoring Cancellation). Peki bu takip işlemleri nasıl gerçekleştirilebilir. Yazımızın ilerleyen kısımlarında bu takip işlemlerinde hangi konuların ele alındığını irdelemeye çalışıyor olacağız.

## Polling Tekniğini ile İptal Taleplerinin İzlenmesi

Bu teknik adından da anlaşılacağı üzere bir iptal talebinin yapılıp yapılmadığını sürekli olarak denetlemeyi gerektirir. Bu durumu analiz etmek için Visual Studio 2010 Ultimate ortamında geliştirilmiş aşağıdaki kod parçasını göz önüne alabiliriz.

```csharp
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading; 
using System.Threading.Tasks;

namespace CancellationScenarios 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            CancellationTokenSource tokenSource = new CancellationTokenSource(); 
            CancellationToken token = tokenSource.Token; 
            IEnumerable<int> numbers =Enumerable.Range(0, 100000000);

            Task startedTask=Task.Factory.StartNew(() => 
                { 
                   for(int i=0;i<numbers.Count();i++) 
                    { 
                        if (token.IsCancellationRequested) 
                        { 
                            Console.WriteLine("İşlemler {0}. elemandan sonra iptal edildi",i.ToString()); 
                           throw new OperationCanceledException(token); 
                        } 
                        else 
                        { 
                            i++; 
                            i--; 
                            i *= 2; 
                            Console.Write("."); 
                        } 
                    } 
                } 
            , token);

            Console.WriteLine("İşlemler devam ediyor. İptal etmek için bir tuşa basınız"); 
            Console.ReadLine();          

            tokenSource.Cancel();

            Console.ReadLine(); 
            Console.WriteLine("Task Status = {0}",startedTask.Status); 
        } 
    } 
}
```

İlk olarak CancellationTokenSource nesnesi örneklenmiş ve sonrasında bu örneğin Token özelliğinden yararlanılarak bir CancellationToken üretilmiştir. CancellationToken nesne örneği Task tipinin örneklenmesi sırasında da parametre olarak aktarılmaktadır.

Dikkat edileceği üzere for döngüsü içerisinde parametre olarak gelen token değişkeni üzerinden IsCancellationRequested özelliği kontrol edilmektedir. Kontrol işlemi döngünün her bir iterasyonunda söz konusudur. IsCancellationRequested özelliğinin true değerini alması için CancellationToken ile ilişkilendirilmiş olan CancellationTokenSource nesne örneği üzerinden Cancel metodunun çağırılması gerekmektedir. Örneği bu haliyle çalıştırdığımızda ve işlemler devam ederken herhangibir noktada herhangibir tuşa bastığımızda ise aşağıdaki ekran görüntüsüne benzer sonuçlar ile karşılaşırız.

[![blg204_PollingTest](/assets/images/2011/blg204_PollingTest_thumb.gif)](/assets/images/2011/blg204_PollingTest.gif)

Önemli olan noktalardan birisi de başlatılan Task’ in Status değeridir. Dikkat edileceği üzere bu değer Canceled olarak set edilmiştir. Ancak bunun böyle olmasının sebebi iptal işleminin kontrol edildiği noktadan OperationCanceledException tipinden istisna nesnesinin fırlatılmasıdır. Bu istisnanın üretilmemesi sonucu ise aşağıdaki gibi olacaktır ki bu da Task durumunu değerlendiren kod parçaları için tam anlamıyla bir handikaptır.

[![blg204_PollingTest2](/assets/images/2011/blg204_PollingTest2_thumb.gif)](/assets/images/2011/blg204_PollingTest2.gif)

Görüldüğü gibi Exception fırlatılmadığı için Task durumu Running olarak kalmıştır. Oysaki Task iptal edilmiştir.

Polling modelinde uygulanan genel kod deseni yukarıdaki gibi olmasına rağmen ikinci bir teknik daha söz konusudur. Bu teknikte kontrol etme ve Exception fırlatma işlemleri zaten var olan bir metoda yüklenmiştir. Buna göre for döngüsü içeriğini aşağıdaki gibi değiştirdiğimizi düşünelim.

```csharp
for(int i=0;i<numbers.Count();i++) 
{ 
	token.ThrowIfCancellationRequested(); 
	i++; 
	i--; 
	i *= 2; 
	Console.Write("."); 
}
```

Bu durumda da çalışma zamanında aşağıdaki sonuçları alırız.

[![blg204_PollingTest3](/assets/images/2011/blg204_PollingTest3_thumb.gif)](/assets/images/2011/blg204_PollingTest3.gif)

Polling tekniği özellikle Task gövdelerinin döngüsel işlemler yaptığı senaryolar için uygundur. Nitekim döngünün her iterasyonu sırasında, bir iptal isteği olup olmadığı kontrol edilmekte ve eğer varsa, döngü dışına çıkılması, ortama uygun İstisna (Exception) nesnesi fırlatılması, gerekiyorsa ilgili kaynakların (Resources) serbest bırakılması işlemleri gerçekleştirilmektedir.

## Delegate Kullanımı

Polling modeli kullanışlı olmasına rağmen özellikle CancellationToken üzerinden ThrowIfCancellationRequested metodu kullanıldığında, sanki eksik olan bir şey var hissi uyandırmaktadır. 6ncı hissimiz bize bu noktada şunu söylemektedir; User Interface’ e sahip uygulamalarda kullanıcıların işlemin iptal edildiğine dair bilgilendirilmesi nasıl sağlanacaktır? Bu his biraz daha genişletilebilir ama sonuç itibariyle iptal işlemi sonrası ana uygulamanın bilgilendirilmesi önemlidir. Özellikle işlemlerin loglandığı, IO süreçlerinin söz konusu olduğu durumlarda. Bu tip bir vaka da delegasyon kullanımı ile iptal işleminin ele alınması tercih edilebilir. Aşağıdaki örnek kod parçasında bu durum irdelenmektedir.

```csharp
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading; 
using System.Threading.Tasks;

namespace CancellationScenarios 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            CancellationTokenSource tokenSource = new CancellationTokenSource(); 
            CancellationToken token = tokenSource.Token; 
            IEnumerable<int> numbers = Enumerable.Range(0, 100000000);

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
                Console.WriteLine("İptal işlem gerçekleştirildi. Tam bu sırada Task Status =  {0}",startedTask.Status); 
            } 
            ); 
            Console.WriteLine("İşlemler devam ediyor. İptal etmek için bir tuşa basınız"); 
            Console.ReadLine();

            tokenSource.Cancel();

            Console.ReadLine(); 
            Console.WriteLine("Task Status = {0}", startedTask.Status); 
        } 
    } 
}
```

Dikkat edileceği üzere token nesnesi üzerinden Register metodu kullanılarak bir delegasyon yapılmaktadır. Buna göre tokenSource.Cancel çağrısı gerçekleştirildiğinde, Register metodu ile işaret edilen blok otomatik olarak devreye girecektir. Örneğin çalışma zamanı çıktısı aşağıdaki ekran görüntüsündekine benzer olacaktır.

[![blg204_DelegationTest1](/assets/images/2011/blg204_DelegationTest1_thumb.gif)](/assets/images/2011/blg204_DelegationTest1.gif)

Dikkat edilmesi gereken notkalardan birisi de, delegasyon metodu içerisine girildiğinde iptal edilen Task örneğinin durumunun halen Running olarak görünmesidir. Ancak işlem tamamlandıktan sonra Status değeri Canceled olmaktadır.

[![Question](/assets/images/2011/Question_thumb.gif)](/assets/images/2011/Question.gif) Bloğun sorusu; Register ile işaret edilen delegasyon metodu içerisinden, for döngüsünün hangi iterasyonunda iptal işleminin gerçekleştirildiği çalışma ortamına nasıl bildirilebilir?

## Wait Handle Kullanımı

Task İptal işlemlerinde delegasyon kullanımına alternatif bir yol olarak Wait Handle tekniğinden de yararlanılabilir. Bu kullanım şeklini daha net bir kavrayabilmek adına aşağıdaki kod parçasını göz önüne alalım.

```csharp
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading; 
using System.Threading.Tasks;

namespace CancellationScenarios 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        {      
            CancellationTokenSource tokenSource = new CancellationTokenSource(); 
            CancellationToken token = tokenSource.Token;

            IEnumerable<int> numbers = Enumerable.Range(0, 100000000);

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
            Task handleTask = Task.Factory.StartNew(() => 
                { 
                    token.WaitHandle.WaitOne(); 
                    Console.WriteLine("İşlem iptali."); 
                } 
            ); 
            Console.WriteLine("İşlemler devam ediyor. İptal etmek için bir tuşa basınız"); 
            Console.ReadLine();

            tokenSource.Cancel();

            Console.ReadLine(); 
            Console.WriteLine("Task Status = {0}", startedTask.Status); 
        } 
    } 
}
```

[![blg204_WaitHandleTest](/assets/images/2011/blg204_WaitHandleTest_thumb.gif)](/assets/images/2011/blg204_WaitHandleTest.gif)

Bu kullanım şeklinde ikinci bir Task bloğunun rol alması gözden kaçmamalıdır;) handleTask isimli Task’ e ait kod bloğu içerisinde, CancellationToken nesnesi üzerinden WaitHandle.WaitOne çağrısının gerçekleştirildiği görülmektedir. Aslında çalışma zamanında her hangibir anda Task durumlarına bakıldığında aşağıdaki ekran görüntüsündekine benzer sonuçlarla karşılaşıldığı görülecektir.

[![blg204_Breakpoint2](/assets/images/2011/blg204_Breakpoint2_thumb.gif)](/assets/images/2011/blg204_Breakpoint2.gif)

Dikkat edileceği üzere handleTask örneğinin durumu Waiting olarak görülmektedir. Bu son derece doğaldır nitekim Task örneğinin başlatılması sonrasında devreye giren fonksiyon bloğunun daha ilk satırında WaitOne ile Thread ‘ in bekletilmesi söz konusudur.

CancellationTokenSource nesne örneği üzerinden Cancel metodunun çağırılması sonrasında ise WaitOne metod çağrısını takip eden kod satırına girildiği ve startedTask örneğinin icra etmekte olduğu işlemlerinde iptal edildiği görülebilir.

Buraya kadar geliştirdiğimiz örnek kod parçaları ve iptal deseneleri göz önüne alındığında her zaman için CancellationTokenSource üzerinden üretilen bir CancellationToken nesneleri olduğu görülmektedir. Ayrıca iptal işlemi de her zaman için CancellationTokenSource örneği üzerinden yapılmaktadır. CancellationToken örnekleri ise Task üretimi sırasında ve içeride kullanılmaktadır. Peki bu bizim ne işimize yarayabilir?

Birden Fazla Task İşleminin Tek Noktadan İptal Edilmesi

İşte az önceki sorunun cevabı. Aynı CancellationToken nesne örneğinin birden fazla Task ile ilişkilendirilmesi, bu token örnekleri ile ilişkili olan CancellationTokenSource örneği üzerinden yapılan iptal talebinin, tüm ilişkili Task’ lere iletilmesi anlamına gelmektedir. Gelin bu cümleyi aşağıdaki kod parçası ile anlamaya çalışalım.

```csharp
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading; 
using System.Threading.Tasks;

namespace CancellationScenarios 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            CancellationTokenSource tokenSource = new CancellationTokenSource(); 
            CancellationToken token = tokenSource.Token;

            IEnumerable<int> numbers = Enumerable.Range(0, 100000000); 
            IEnumerable<int> numbers2 = Enumerable.Range(0, 100000000);

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

            Task startedTask2 = Task.Factory.StartNew(() => 
            { 
                for (int i = 0; i < numbers2.Count(); i++) 
                { 
                    token.ThrowIfCancellationRequested(); 
                    i++; 
                    i--; 
                    i *= 2; 
                    Console.Write("/"); 
                } 
            } 
            , token);

            token.Register(() => 
                { 
                    Console.WriteLine("İşlem iptali"); 
                } 
            );

            Console.WriteLine("İşlemler devam ediyor. İptal etmek için bir tuşa basınız"); 
            Console.ReadLine();

            tokenSource.Cancel();

            Console.ReadLine(); 
            Console.WriteLine("Task 1 Status = {0}\nTask 2 Status = {1}", startedTask.Status,startedTask2.Status); 
        } 
    } 
}
```

Kod parçasında iki farklı Task içeriği söz konusudur. Ancak her iki Task örneklenirken aynı CancellationToken ile ilişkilendirilmiştir. Buna göre CancellationTokenSource üzerinden yapılacak olan iptal talebi, her iki task işlemi içinde istenmiş olmaktadır. Örnek çalışma zamanı çıktısı aşağıdaki şekilde görüldüğü gibidir.

[![blg204_MultipleCancel](/assets/images/2011/blg204_MultipleCancel_thumb.gif)](/assets/images/2011/blg204_MultipleCancel.gif)

Görüldüğü gibi her iki Task örneğinin durumu Canceled olarak set edilmiştir. Task iptal işlemleri ile ilişkili olarak ele almamız gereken farklı konular da mevcuttur. Bunları bir sonraki yazımızda ele almaya çalışıyor olacağız. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[CancellationScenarios.rar (23,87 kb)](/assets/files/2011/CancellationScenarios.rar) [Örnekler Visual Studio 2010 Ultimate sürümü üzerinde geliştirişmiş ve test edilmiştir]