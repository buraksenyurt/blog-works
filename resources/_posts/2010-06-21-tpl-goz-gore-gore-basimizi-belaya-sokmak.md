---
layout: post
title: "TPL – Göz Göre Göre Başımızı Belaya Sokmak"
date: 2010-06-21 06:08:00 +0300
categories:
  - parallel-programming
  - tpl
tags:
  - parallel-programming
  - tpl
  - csharp
  - task-parallel-library
  - threading
  - visual-studio
---
Bazen göz göre göre başımıza bi ton dert açarız. Kimi zaman başlayacağımız iş bize çok eğlenceli gelebilir (Yandaki resimde yüzü görünmeyen şahsın da bu heyacanla Hamburgere bindiğinden eminiz) Ama işin sonuçlarını biliyorsak eğer, bunu yapmamızın nedeni büyük olasılıkla adrenalindir.

[![blg209_Giris](/assets/images/2010/blg209_Giris_thumb.jpg)](/assets/images/2010/blg209_Giris.jpg)


Tabi ki bir yazılımcı için adrenalin genellikle üst yöneticisi tarafından salgılanan bir hormondur. Nitekim yazılımcıların, ilerideki felaketleri kestirerek hareket etmesi ve geliştirmeleri buna göre yapması her zaman kolay olmayabilir. Bir başka deyişle bazı vakalara hazırlıklı olmak için önceden bunları çalışmak gerekmektedir.

İşte bu yazımızda biz de Task Parallel Library için söz konusu olan ve geliştiricilerin başını derde sokacak 2 vaka üzerinde duruyor olacağız. Haydi o zaman parmakları sıvayalım ve işe koyulalım.

Deadlock Durumu

Bu kelime her zaman korkutucu olmuştur. Yazılım Geliştirme serüvenime ilk başladığım yıllarda çoğunlukla veritabanı tarafındaki kilitlenmelerden söz edildiğini çok net hatırlıyorum. Ancak birden fazla iş parçasının da deadlock’ a düşmesi, bir başka deyişle birbirlerini beklemeleri nedeniyle, içinde çalıştıkları Thread’ i (çoğunlukla ana uygulama iş parçası-Main Thread) kitlemeleri söz konusudur. Durumu daha net anlayabilmek için aşağıdaki kod parçasını göz önüne alalım.

```csharp
using System; 
using System.Threading.Tasks;

namespace Disasters 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            Task<double> task1 = null; 
            Task<int[]> task2 = null;

            task2 = Task.Factory.StartNew<int[]>(() => 
            { 
               double task1Result=task1.Result; 
                Random rnd = new Random(); 
                int[] numbers = new int[5]; 
                for (int i = 0; i < 5; i++) 
                { 
                    numbers[i] = rnd.Next(1, 250); 
                } 
                return numbers; 
            } 
            );

            task1 = Task.Factory.StartNew<double>(()=> 
            { 
                double totalValue = 0; 
                foreach (int number in task2.Result) 
                { 
                    totalValue += number; 
                } 
                return totalValue; 
            } 
            );

            Task.WaitAll(task1, task2);

            Console.WriteLine("İşlemlerin sonu.Programdan çıkmak için bir tuşa basınız"); 
            Console.ReadLine();   
        } 
    } 
}
```

Aslında kod çok fazla değerlendirilebilir veya anlamlı değildir. Ancak Deadlock oluşumunu görmemiz açısından yeterlidir. Örnekte task1 ve task2 isimli Task nesne örneklerinin, birbirlerinin dönüş değerlerini kullanmaya çalıştığı ifade edilmektedir. İşte Task örneklerinin çalışma zamanında birbirlerini beklemeleri, kendi durumlarının Deadlock olarak set edilmesine neden olacaktır. Bu durum Debug modda aşağıdaki ekran görüntüsünde olduğu gibi görülebilir.

[![blg209_Debug1](/assets/images/2010/blg209_Debug1_thumb.gif)](/assets/images/2010/blg209_Debug1.gif)

Görüldüğü üzere her iki Task birbirini bekler şekilde kalmıştır. Çok doğal olarak çalışma zamanı çıktısı kapkara bir ekran olacaktır.

[![blg209_FirstRuntime](/assets/images/2010/blg209_FirstRuntime_thumb.gif)](/assets/images/2010/blg209_FirstRuntime.gif)

Gelelim diğer bir senaryoya.

Döngü Değişkenlerine Dikkat

Bu aslında oldukça eğlenceli ve bir o kadarda beklenmedik sonuçları üreten vakalardandır. Olayı hızlı bir şekilde değerlendirmek adına aşağıdaki kod parçasını göz önüne alabiliriz.

```csharp
using System; 
using System.Threading.Tasks;

namespace Disasters 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            for (int i = 0; i < 10; i++) 
            { 
                Task.Factory.StartNew(() => 
                    { 
                        for (int j = 0; j < 125000000; j++) 
                        { 
                            j++; 
                            j--; 
                            j*=j; 
                        } 
                        Console.WriteLine("Güncel Task Id : {0}\tGüncel i : {1}",Task.CurrentId,i.ToString()); 
                    } 
                ); 
            }

            Console.WriteLine("Kapatmak için bir tuşa basınız"); 
            Console.ReadLine(); 
        } 
    } 
}
```

Örnek kod ile çalışma zamanında 10 Task örneği başlatılmakta ve bunların lambda ifadeleri (=> Expressions) içerisinden o anki i değerleri ekrana yazdırılmaktadır. Normal şartlarda i değerlerinin her bir Task örneği için farklı olması beklenir. Ancak çalışma zamanına baktığımızda aşağıdaki enteresan sonuçlar ile karşılaştığımızı görebiliriz.

[![blg209_Runtime2](/assets/images/2010/blg209_Runtime2_thumb.gif)](/assets/images/2010/blg209_Runtime2.gif)

Görüldüğü üzere bütün Task örnekleri for döngüsü sayacının son değerini ekrana yazdırmaktadır. Çok kolay bir şekilde gözden kaçabilecek bu vaka nedeniyle uzun süre ekrana baka kalabilir ve arkadaşlarımızın bize “Kal Gelmiş” demelerine neden olabiliriz. Oysa ki çözüm son derece basittir.

```csharp
using System; 
using System.Threading.Tasks;

namespace Disasters 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            for (int i = 0; i < 10; i++) 
            { 
                Task.Factory.StartNew((s) => 
                { 
                    for (int j = 0; j < 125000000; j++) 
                    { 
                        j++; 
                        j--; 
                        j *= j; 
                    } 
                    int currentI = (int)s;

                    Console.WriteLine("Güncel Task Id : {0}\tGüncel i : {1}", Task.CurrentId, currentI.ToString()); 
                },i 
                ); 
            }

            Console.WriteLine("Kapatmak için bir tuşa basınız"); 
            Console.ReadLine(); 
        } 
    } 
}
```

Bu sefer StartNew metodunun farklı bir versiyonu kullanılmıştır. Dikkat edileceği üzere metodun ikinci parametresi olarak i değişkeni kullanılmıştır. Bu aslında State Object olarak düşünülebilir. Dolayısıyla başlatılan her Task örneğine parametre olarak o anki döngü değeri (i değişkeninin değeri) geçirilmektedir. s isimli değişken, for döngüsünden gelen i değişkenini object tipinden temsil ettiği için de, basit bir Cast işlemi yapılması yeterlidir. İşte çalışma zamanı sonuçları.

[![blg209_Runtime3](/assets/images/2010/blg209_Runtime3_thumb.gif)](/assets/images/2010/blg209_Runtime3.gif)

Görüldüğü gibi Task örnekleri kendilerine atanan i değerlerini ekrana basmıştır.

Elbette farklı vakalar ve felaket senaryoları da söz konusudur. Bu gibi durumları ilerleyen yazılarımızda ele almaya çalışıyor olacağız. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Disasters.rar (22,06 kb)](/assets/files/2010/Disasters.rar) [Örnek Visual Studio 2010 Ultimate Sürümünde Geliştirilmiş ve Test Edilmiştir]