---
layout: post
title: "Task Süreçlerinde Bilinçli Olarak Duraksatma"
date: 2010-12-31 13:20:00 +0300
description: "Bu günkü konumuz Task nesne örneklerinin işlettikleri süreçleri bilinçli olarak nasıl bekletebileceğimiz ile ilgilidir. Pek çok sebepten dolayı Task örneklerinin çalıştırdıkları iş parçalarının belirli süreler boyunca veya süre bağımsız olarak bekletilmeleri istenebilir. Burada zaman bağımlı ya da koşul bağımlı olarak bekletmelerin/duraksatmaların yapılabilmesi söz konusudur. Genel olarak 3 farklı bekletme tekniğinden söz edebiliriz."
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
# Task Süreçlerinde Bilinçli Olarak Duraksatma
[![blg206_Giris](/assets/images/2010/blg206_Giris_thumb.jpg)](/assets/images/2010/blg206_Giris.jpg) Merhaba Arkadaşlar,

Pek çoğumuz hayatımızın çeşitli dönemlerinde ayakta veya oturarak bir şeyler için beklemek zorunda kalırız. Kimi zaman seyahat edeceğimiz yere gidecek aracı bekleriz. Özellikle uçak seyahatleri gibi gecikmelerin sıklıkla yaşanabileceği durumlardan tutunda, İstanbul trafiğinin akşam saatlerindeki yoğunluğu yüzünden yine belirsiz süre dolmuş beklenmesi gibi hallerle sıklıkla karşılaşılır.

Fakat bazı zamanlarda belirli süreli bekleyişler de söz konusu olabilir. Örneğin bir iş görüşmesinin saati bellidir ve gecikme durumları pek söz konusu olmamaktadır. Ya da bir metronun kalkış saati, hatta gideceği yere varış zamanı dahi sabittir. Dolayısıyla metroaya binmek için beklenen ve metro içindeyken gidilecek yere kadar geçen süreler genellikle standarttır. Ancak hangi durum olursa olsun, hayatımızda her zaman için beklemeler söz konusudur ve bunlar zaman zaman tekrar ederek bir yaşam döngüsünün oluşmasına neden olmaktadır.

Beklemeye neden bu kadar taktığımızı düşünebilirsiniz. Aslında beklemeyi seven bir insan değilimdir. Metro’ ların yaşam stili bana biraz daha uygundur diyebilirim. Çünkü bekleme süreleri sabittir ve buna göre planlama yapmak kolaydır. Ancak hayat her zaman bu kadar kolay planlama yapmayı olanaklı kılmayacak sürprizlerle doludur.

Bu günkü konumuz Task nesne örneklerinin işlettikleri süreçleri bilinçli olarak nasıl bekletebileceğimiz ile ilgilidir. Pek çok sebepten dolayı Task örneklerinin çalıştırdıkları iş parçalarının belirli süreler boyunca veya süre bağımsız olarak bekletilmeleri istenebilir. Burada zaman bağımlı ya da koşul bağımlı olarak bekletmelerin/duraksatmaların yapılabilmesi söz konusudur. Genel olarak 3 farklı bekletme tekniğinden söz edebiliriz.

- CancellationToken nesne örneği üzerinden ulaşılan WaitOne metodu ile
- Thread sınıfının static Sleep metodu ile
- Thread sınıfının static SpinWait metodu ile

Şimdi bu farklı teknikleri örnekler yardımıyla incelemeye başlayalım.

CancellationToken.WaitHandle.WaitOne Tekniği

WaitOne metodu yardımıyla belirsiz süreli veya belirli süreli duraksatma işlemleri gerçekleştirebiliriz. Konuyu daha net kavramak adına aşağıdaki örnek kod parçasını göz önüne alalım. Oldukça tanıdık gelecek.

```csharp
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading; 
using System.Threading.Tasks;

namespace WaitingScenarios 
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
                    bool waitStatus=token.WaitHandle.WaitOne(); 
                    if (waitStatus == true) 
                        throw new OperationCanceledException(token); 
                    else 
                        Console.WriteLine("CancellationToken WaitOne Status = {0}",waitStatus); 
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

            Console.WriteLine("İşlemler devam ediyor. İptal etmek için bir tuşa basınız."); 
            Console.ReadLine();

            tokenSource.Cancel();

            Console.ReadLine(); 
            Console.WriteLine("Task 1 Status = {0}", startedTask.Status);

        } 
    } 
}
```

Örnekte Task bloğu içerisinde yer alan for döngüsünün ilk satırında belirsiz süreli bir duraksatma yapıldığı görülmektedir. Nitekim WaitOne metodu herhangibir süre parametresi almamıştır. Dolayısıyla şartlar uygun olduğunda Task’ in sonsuza kadar beklemesi de olasıdır.

Diğer yandan WaitOne metodu bool bir değer döndürmektedir. Peki nasıl olucaktırda bu değer dönecektir? Az önce belirsiz süreli bir bekletme yaptığımızdan bahsetmiştik. Cevap, iptal talebinin gelmesidir. CancellationTokenSource nesne örneği üzerinden gelen Cancel çağrısı, WaitOne metodunun işleyişini kesmesi, bir başka deyişle geriye anında bool bir değer döndürmesi ve kodun yürümeye devam etmesi anlamına gelmektedir. Böyle bir durumda WaitOne metodu true değer döndürecektir.

Örneği çalıştırdığımızda ve özellikle bir tuşa basarak işlemi iptal etmediğimizde, Debug modda Task örneğinin anlık durumu aşağıdaki gibi görülecektir.

[![blg206_WaitOneTest1Debug](/assets/images/2010/blg206_WaitOneTest1Debug_thumb.gif)](/assets/images/2010/blg206_WaitOneTest1Debug.gif)

Dikkat edileceği üzere Task nesne örneğinin Status değeri Waiting olarak set edilmiştir. Çalışma zamanında tuşa basılarak işlemin iptal edilmesinin sonucunda ise, aşağıdaki ekran görüntüsünde yer alan durum ile karşılaşılacaktır.

[![blg206_WaitOneTest1](/assets/images/2010/blg206_WaitOneTest1_thumb.gif)](/assets/images/2010/blg206_WaitOneTest1.gif)

Görüldüğü gibi Task örneğinin Status değeri Canceled olarak değişmiştir.

Tabiki bu örnekte belirsiz süreli bir duraksatma işlemi gerçekleştirilmiş ve içinden çıkılması için Cancel çağrısının yapılması şart olmuştur. Ancak WaitOne metoduna istenildiğinde bekleme süresi, milisaniye cinsinden de verilebilir. Örnek kodumuzda 3000 milisaniyelik (yani 3 saniyelik) bir duraksatma yaptırmak istediğimizi düşünelim. Bu durumda WaitOne metodunu aşağıdaki gibi kullanmamız yeterlidir.

bool waitStatus=token.WaitHandle.WaitOne (3000);

Bu durumda örneğin çalışma zamanı çıktısı aşağıdakine benzer olacaktır.

[![blg206_WaitOneTest3](/assets/images/2010/blg206_WaitOneTest3_thumb.gif)](/assets/images/2010/blg206_WaitOneTest3.gif)

[![Exclamation](/assets/images/2010/Exclamation_thumb.gif)](/assets/images/2010/Exclamation.gif) Farketmişsinizdir…3 saniyelik bir duraksatma yapmamıza rağmen ekrana yazılan bilgiler +2 saniye daha geç gelmektedir. Burada şüpheli şahıs değer aralığıdır. Nitekim Enumberable ile üretilen aralığın küçültülmesi (örneğin 10000’ e çekilmesi) halinde süreler beklendiği gibi çıkmaktadır. Yani +2 lik kayıp olmamaktadır.

Sonuç itibariyle belirli süreliğine Task örneğinin işleyişinin duraksatılması gerçekleştirilmiştir. Bu süreli çalışmalarda milisaniye olarak belirtilen periyodun her tamamlanışında, kod bir sonraki satırdan devam etmekte ve CancellationTokenSource nesne örneğinin WaitOne metodu false değeri anında ortama döndürmektedir. Tahmin edileceği üzere bu değerin true olmasının Task işleyişinin iptal edilmesi gerekmektedir.

Thread.Sleep Metodunun Kullanılması

Task Parallel Library var olmadan önce, Çok Kanallı (Multi Thread) uygulamaların geliştirilmesinde en çok haşır neşir olduğumuz tip sanıyorum ki Thread sınıfıdır. Özellikle beliri durumlarda, çalışmakta olan bir Thread örneğini duraksatmak istediğimizde, static Sleep metodundan yararlanabiliriz. Sleep metodu Task örneklerinin duraksatılması içinde kullanılabilir. Bu son derece doğaldır nitekim TPL zaten var olan Thread sistemi üzerine kurulumuş bir alt yapıdır. İşte örnek kod parçamız.

```csharp
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading; 
using System.Threading.Tasks;

namespace WaitingScenarios 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            IEnumerable<int> numbers = Enumerable.Range(0, 10000);

            CancellationTokenSource tokenSource = new CancellationTokenSource(); 
            CancellationToken token = tokenSource.Token;

            Task startedTask = Task.Factory.StartNew(() => 
            { 
                for (int i = 0; i < numbers.Count(); i++) 
                { 
                   Thread.Sleep(8000);                    
                    i++; 
                    i--; 
                    i *= 2; 
                    Console.WriteLine("Time : {0}...",DateTime.Now.ToLongTimeString()); 
                    token.ThrowIfCancellationRequested(); 
                } 
            } 
            , token);

            token.Register(() => 
            { 
                Console.WriteLine("Time : {0} , İşlem iptali",DateTime.Now.ToLongTimeString()); 
            } 
            );

            Console.WriteLine("İşlemler devam ediyor. İptal etmek için bir tuşa basınız."); 
            Console.ReadLine();

            tokenSource.Cancel();

            Console.ReadLine(); 
            Console.WriteLine("Time : {0} , Task 1 Status = {1}", DateTime.Now.ToLongTimeString(),startedTask.Status); 
        } 
    } 
}
```

Örnekte 8 saniyelik bir duraksatma işlemi söz konusudur. Burada anlaşılması güç bir durum yoktur ancak çalışma zamanında dikkat edilmesi gereken bir nokta vardır. İlk testimizin sonuçları aşağıdaki gibidir.

[![blg206_ThreadSleepTest](/assets/images/2010/blg206_ThreadSleepTest_thumb.gif)](/assets/images/2010/blg206_ThreadSleepTest.gif)

Şimdi ikinci teste ait ekran görüntüsünü de yazımıza ekleyelim ve iki şekil arasındaki 9 farkı bulmaya çalışalım.

[![blg206_ThreadSleepTest2](/assets/images/2010/blg206_ThreadSleepTest2_thumb.gif)](/assets/images/2010/blg206_ThreadSleepTest2.gif)

İlk çalışma da işleyişi sonlandırmak için kullanıcı tuşa bastığında bir Cancel talebi üretilmektedir. Cancel talebi 7nci saniye de gelmiş ve kullanıcı tekrardan tuşa 10ncu saniye de basarak uygulamayı sonlandırmıştır. Ne varki Task örneğinin Status değeri Running olarak set edilmiştir. Yani halen çalışıyor olduğu bildirilmektedir. Bunun sebebi, Task için Cancel çağrısı yapılmış olsa bile, Thread tipinin static Sleep metodunun belirttiği bekleme süresinin dolmamış olmasıdır. Ancak bu süre dolduktan sonra Task örneğinin Status değerinin Canceled olduğu görülebilir. Bu da CancellationToken üzerinden ele alınan WaitOne metodu ile Thread.Sleep arasındaki en önemli farktır.

Thread.SpinWait Metodunun Kullanımı

Bu teknikte CPU’ nun çalıştıracağı ve duraksatma işlemi için gerekli döngü sayısı belirtilir. İşlemci aslında kendi içerisinde çok hafif bir döngüyü kullanarak duraksatma işlemini gerçekleştirmektedir. Bu nedenle işlemcinin durumu, anlık yoğunluğu gibi kriterler bekleme sürelerinin belirlenmesinde önemli rol oynamaktadır. Gelin örnek kod parçamızı değerlendirerek konuyu anlamaya çalışalım.

```csharp
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading; 
using System.Threading.Tasks;

namespace WaitingScenarios 
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
                    Thread.SpinWait(50000000); 
                    i++; 
                    i--; 
                    i *= 2; 
                    Console.WriteLine("Time : {0}...", DateTime.Now.ToLongTimeString()); 
                    token.ThrowIfCancellationRequested(); 
                } 
            } 
            , token);

            token.Register(() => 
            { 
                Console.WriteLine("Time : {0} , İşlem iptali", DateTime.Now.ToLongTimeString()); 
            } 
            );

            Console.WriteLine("İşlemler devam ediyor. İptal etmek için bir tuşa basınız."); 
            Console.ReadLine();

            tokenSource.Cancel();

            Console.ReadLine(); 
            Console.WriteLine("Time : {0} , Task 1 Status = {1}", DateTime.Now.ToLongTimeString(), startedTask.Status); 
        } 
    } 
}
```

SpinWait metoduna verilen 50milyon değeri tahmin edeceğiniz üzere süreyi değil iterasyon sayısını belirtmektedir. Buna göre işlemcinin 50 milyon kere, hafif bir döngüyü çalıştırarak o anki Task örneğinin işleyişini duraksatacağı belirtilmektedir. Kendi sistemimde söz konusu örnek kodun çalışma zamanı sonuçları aşağıdaki gibi gerçekleşmiştir. Ancak bu durum farklı sistemlerde farklı sonuçlar verebilir.

[![blg206_SpinWaitTest1](/assets/images/2010/blg206_SpinWaitTest1_thumb.gif)](/assets/images/2010/blg206_SpinWaitTest1.gif)

Görüldüğü gibi sabit bir bekleme süresi söz konusu olmamıştır. Bazen 3 saniye bazen 2 saniye ve bazende 4 saniyelik bir duraksama olduğu görülmektedir. Yine doğal olarak bu bekleme sürecinden çıkılması için iptal talebinin yapılması veya tüm işleyişin sona ermesinin beklenmesi yeterlidir.

Bu yazımızda Task işleyişlerinin duraksatılması için kullanabileceğimiz teknikleri incelemeye çalıştık. Bir sonraki yazımızda duraksatma işlemleri için kullanılabilecek diğer teknikleri de öğrenmeye çalışıyor olacağız. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[WaitingScenarios.rar (23,76 kb)](/assets/files/2010/WaitingScenarios.rar) [Örnek Visual Studio 2010 Ultimate sürümü üzerinde geliştirilmiş ve test edilmiştir]