---
layout: post
title: "Composite Cancellations"
date: 2011-04-26 12:15:00 +0300
categories:
  - parallel-programming
  - tpl
tags:
  - csharp
  - parallel-programming
  - cancellation-token
  - visual-studio
  - task-parallel-library
  - monitoring-cancellation
---
Hatırlayacağınız üzere bir önceki yazımızda ([Task İptal İşlemlerinin İzlenmesi (Monitoring Cancellation)](https://www.buraksenyurt.com/post/Task-Iptal-Islemlerinin-Izlenmesi(Monitoring-Cancellation))) Task Cancellation işlemlerinin izlenmesi ile ilişkili teknikleri ve konuları irdelemeye başlamıştık. Bu yazımızda da iptal işlemleri ile ilgili farklı bir konuya değinmeye çalışıyor olacağız. Bu gün kü konumuz Composite Cancellation vakası.

[![blg205_Giris](/assets/images/2011/blg205_Giris_thumb.jpg)](/assets/images/2011/blg205_Giris.jpg)


Bildiğiniz üzere Task iptal taleplerinde, CancellationTokenSource örneğine ait Cancel metodunun çağırılması gerekmektedir. CancellationTokenSource örneği üzerinden yapılan iptal taleplerinin hangi Task işleyişini keseceğinin belirlenmesinde ise CancellationToken örneklerinden yararlanılmaktadır. CancellationToken örnekleri hatırlayacağını üzere CancellationTokenSource örnekleri tarafından üretilmekte ve Task’ ler ile ilişkilendirilmektedir. Bu sebepten Cancel metodunun hangi Task ile ilişkili olduğu bellidir. Bir önceki yazımızda geliştirdiğimiz son örnekte aynı Token örneğini kullanan birden fazla Task’ in tek bir iptal çağrısı ile nasıl kesilebileceğini incelemiştik. Bir diğer durum da şudur;

> Birden fazla CancellationTokenSource birbirlerine bağlanarak bir zincir oluşturulabilir ve bunlardan herhangibiri üzerinden Cancel işleminin yapılması, zincirdeki tüm source’ lar için de aynı talebin gerçekleştirilmesi anlamına gelmektedir.

Konuyu daha net kavrayabilmek için aşağıdaki kod parçasını göz önüne alabiliriz.

```csharp
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading; 
using System.Threading.Tasks;

namespace CancellationScenarios2 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            IEnumerable<int> numbers = Enumerable.Range(0, 100000000);

            CancellationTokenSource tokenSource1 = new CancellationTokenSource(); 
            CancellationTokenSource tokenSource2 = new CancellationTokenSource(); 
            CancellationTokenSource tokenSource3 = new CancellationTokenSource();

            CancellationTokenSource compositeSource = CancellationTokenSource.CreateLinkedTokenSource(tokenSource1.Token, tokenSource2.Token,tokenSource3.Token);

            Task startedTask = Task.Factory.StartNew(() => 
            { 
                for (int i = 0; i < numbers.Count(); i++) 
                { 
                    compositeSource.Token.ThrowIfCancellationRequested(); 
                    i++; 
                    i--; 
                    i *= 2; 
                    Console.Write("."); 
                } 
            } 
            , compositeSource.Token);

            Task startedTask2 = Task.Factory.StartNew(() => 
            { 
                for (int i = 0; i < numbers.Count(); i++) 
                { 
                    compositeSource.Token.ThrowIfCancellationRequested(); 
                    i++; 
                    i--; 
                    i++; 
                    i *= 2; 
                    Console.Write("+"); 
                } 
            } 
            , compositeSource.Token);

            Task startedTask3 = Task.Factory.StartNew(() => 
            { 
                for (int i = 0; i < numbers.Count(); i++) 
                { 
                   compositeSource.Token.ThrowIfCancellationRequested(); 
                    i++; 
                    i *= 2; 
                    Console.Write("/"); 
                } 
            } 
            , compositeSource.Token);

            compositeSource.Token.Register(() => 
            { 
                Console.WriteLine("İşlem iptali"); 
            } 
            );

            Console.WriteLine("İşlemler devam ediyor. İptal etmek için bir tuşa basınız"); 
            Console.ReadLine();

            tokenSource3.Cancel();

            Console.ReadLine(); 
            Console.WriteLine("Task 1 Status = {0}\nTask 2 Status = {1}\nTask 3 Status = {2}", startedTask.Status, startedTask2.Status,startedTask3.Status); 
        } 
    } 
}
```

Örnek uygulamamızda 3 adet CancellationTokenSource nesnesi örneklendiği görülmektedir. Sonrasında ise bu örnekler CancellationTokenSource sınıfının static CreateLinkedTokenSource metodu yardımıyla birbirlerine bağlanmıştır. 3 Task örneği oluşturulduğu sırada yapılan Token bildirimlerinde, CreateLinkedTokenSource metodu ile üretilen CancellationTokenSource nesne örneğine ait Token özelliğinden yararlanılmıştır.

Kodun ilerleyen kısımlarında Task fonksiyonellikleri icra ettikleri işleri tamamlanmadan önce kullanıcı bir tuşa basarsa tokenSource3 isimli değişken üzerinden yapılan Cancel çağırısı devreye girmektedir. Buna göre zincir içerisinde yer alan CancellationTokenSource örneklerinin her biri için bir iptal talebi söz konusu olacaktır. Dolayısıyla uygulamanın çalışması sonrasında aşağıdaki ekran görüntüsüne benzer sonuçlar üretilecektir.

[![blg205_CompositeTest1](/assets/images/2011/blg205_CompositeTest1_thumb.gif)](/assets/images/2011/blg205_CompositeTest1.gif)

Dikkat edileceği üzere 3 Task içinde Status değeri Canceled olmuştur. CancellationTokenSource örneklerinin birden fazla olduğu ve herhangibiri üzerinde yapılan iptal işleminin diğerleri içinde söz konusu olması gerektiği durumlarda bu teknikten yararlanılabilir. İlk örneğimizdeki durumu kafamızda daha kolay canlandırmak için aşağıdaki tasviri göz önüne alabiliriz.

[![blg205_Schema](/assets/images/2011/blg205_Schema_thumb.gif)](/assets/images/2011/blg205_Schema.gif)

Şimdi konuyu farklı bir bakış açısından değerlendirmek için kodu biraz değiştirip aşağıdaki hale getirelim.

```csharp
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading; 
using System.Threading.Tasks;

namespace CancellationScenarios2 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            IEnumerable<int> numbers = Enumerable.Range(0, 100000000);

            CancellationTokenSource tokenSource1 = new CancellationTokenSource(); 
            CancellationTokenSource tokenSource2 = new CancellationTokenSource(); 
            CancellationTokenSource tokenSource3 = new CancellationTokenSource();

            CancellationTokenSource compositeSource = CancellationTokenSource.CreateLinkedTokenSource(tokenSource1.Token, tokenSource2.Token);

            Task startedTask = Task.Factory.StartNew(() => 
            { 
                for (int i = 0; i < numbers.Count(); i++) 
                { 
                    compositeSource.Token.ThrowIfCancellationRequested(); 
                    i++; 
                    i--; 
                    i *= 2; 
                    Console.Write("."); 
                } 
            } 
            , compositeSource.Token);

            Task startedTask2 = Task.Factory.StartNew(() => 
            { 
                for (int i = 0; i < numbers.Count(); i++) 
                { 
                    compositeSource.Token.ThrowIfCancellationRequested(); 
                    i++; 
                    i--; 
                    i++; 
                    i *= 2; 
                    Console.Write("+"); 
                } 
            } 
            , compositeSource.Token);

            Task startedTask3 = Task.Factory.StartNew(() => 
            { 
                for (int i = 0; i < numbers.Count(); i++) 
                { 
                    tokenSource3.Token.ThrowIfCancellationRequested(); 
                    i++; 
                    i *= 2; 
                    Console.Write("/"); 
                } 
            } 
            , tokenSource3.Token);

            compositeSource.Token.Register(() => 
            { 
                Console.WriteLine("İşlem iptali"); 
            } 
            );

            Console.WriteLine("İşlemler devam ediyor. İptal etmek için bir tuşa basınız"); 
            Console.ReadLine();

            tokenSource1.Cancel();

            Console.ReadLine(); 
            Console.WriteLine("Task 1 Status = {0}\nTask 2 Status = {1}\nTask 3 Status = {2}", startedTask.Status, startedTask2.Status,startedTask3.Status);

        } 
    } 
}
```

Bu örnekte sadece tokenSource1 ve tokenSource2 örnekleri birbirlerine bağlanmış ancak tokenSource3 bu zincirin dışında tutulmuştur. Buna göre tokenSource1.Cancel çağrısının gerçekleştirilmesi sonucunda sadece zincir içerisinde dahil edilmiş Task’ lerin iptal işlemi gerçekleşecektir. Çalışma zamanı çıktısında bu durum net bir şekilde görülebilir.

[![blg205_CompositeTest2](/assets/images/2011/blg205_CompositeTest2_thumb.gif)](/assets/images/2011/blg205_CompositeTest2.gif)

Görüldüğü gibi Task 1 ve 2 için Status değerleri Canceled olarak set edilmişken, Task 3 çalışmaya devam ettiğinden Running değerini almıştır.

Böylece geldik bir yazımızın daha sonuna. Paralel programlama ile ilişkili konuları incelemeye devam ediyor olacağız. Bundan sonraki yazımızda Task'lerin belirli süreler boyunca bekletilmesi amacıyla kullanabileceğimiz teknikleri araştırıyor ve örnekliyor olacağız. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[CancellationScenarios2.rar (21,79 kb)](/assets/files/2011/CancellationScenarios2.rar) [Örnek Visual Studio 2010 Ultimate sürümünde geliştirilmiş ve test edilmiştir]