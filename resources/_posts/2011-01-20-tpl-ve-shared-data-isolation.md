---
layout: post
title: "TPL ve Shared Data Isolation"
date: 2011-01-20 13:35:00 +0300
categories:
  - parallel-programming
tags:
  - parallel-programming
  - csharp
  - dotnet
  - task-parallel-library
  - threading
  - delegates
  - visual-studio
  - shared-state
---
Sanıyorum ki, “Bir elin nesi var, iki elin sesi var” deyimini bilmeyen yoktur. Bir matematikçi olarak tüme varım yaparsam, n tane elin çok daha sesli olduğunu ispat etmek isteyebilirim. Ne varki dünya kupasındaki n tane elin tuttuğu Vuvuzela’ ların çıkarttığı sesi düşününce, bu teoremden hemen vazgeçebilirim de. Neyseki konumuz bu değil. Konumuz paralel kütüphanede, paylaşımlı verileri nasıl ele alabileceğimiz.

![blg210_Giris](/assets/images/2011/blg210_Giris_thumb.jpg)

Task Parallel Library alt yapısını kullanarak geliştirdiğimiz paralel kodlarda önem arz eden konulardan birisi de, paylaşılan verilerin hesaplamalara katıldığı durumlardaki sonuç tutarlılıklarının nasıl sağlanacağıdır. Bunun bilinen bir kaç yolu vardır. Aslında bir tanesi ve en basiti kodu tamamen senkron olarak geliştirmektir. Yani paralel çalıştırmak gibi bir maceraya hiç girmemektir. Diğer bir yol ise Task örnekleri içerisinde ele alınan paylaşılmış verilerin izole edilerek kullanılmasıdır. Aslında durumu daha kolay bir şekilde analiz edebilmek için önce sorunu masaya yatıralım. Bu amaçla aşağıdaki kod parçasını göz önüne alarak ilerleyebiliriz.

```csharp
using System; 
using System.Threading.Tasks;

namespace SharedDataScenarios 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            for (int i = 0; i < 10; i++) 
            { 
                TestMethod(); 
            }

            Console.WriteLine("İşlemler tamamlandı.\nProgramı kapatmak için bir tuşa basınız"); 
            Console.ReadLine(); 
        }

        private static void TestMethod() 
        { 
           Plane f16 = new Plane(); 
            Task[] tasks = new Task[5];

            for (int i = 0; i < 5; i++) 
            { 
                tasks[i] = new Task(() => 
               { 
                    for (int j = 0; j < 1000; j++) 
                    { 
                        f16.Altitude += j - 5; 
                    } 
                } 
                ); 
                tasks[i].Start(); 
            }

            Task.WaitAll(tasks);

            Console.WriteLine("[ {0} ]", f16.Altitude); 
        } 
    }

    class Plane 
    { 
        public int Altitude { get; set; } 
    } 
}
```

Örnek kod parçasında Altitude isimli int tipinden özelliği (Property) olan Plane isimli bir sınıf yer almaktadır. TestMethod içerisinde ise bu sınıfa ait f16 isimli bir nesne örneği kullanılmaktadır. TestMethod içerisinde 5 adet Task nesnesi örneklenmektedir. Bu örneklere ait lambda ifadelerinde ise, f16 örneği üzerinden ulaşılan Altitude özelliğinin değeri 1000 kere 5 birim arttırılmaktadır. Bu işlemi 5 farklı Task örneğinin yaptığı unutulmamalıdır. Main metodu içerisinde ise TestMethod isimli fonksiyonun arka arkaya 10 defa çalıştırıldığı görülmektedir. Buradaki amaç, 10 farklı denemenin sonuçlarını irdelemektir. Nitekim her çalıştırılışta farklı sonuçlar alma ihtimalimiz çok yüksektir. Aşağıdaki ekran görüntüsünde olduğu gibi.

[![blg210_Test1](/assets/images/2011/blg210_Test1_thumb.gif)](/assets/images/2011/blg210_Test1.gif)

Aslında aynı süreç 10 kere çalıştırılmış ve hemen her seferinde Plane nesne örneğinin Altitude değeri farklı hesap edilmiştir. Oysaki başlangıçta 0 olan bu değer bir Task örneği içerisinde 1000 defa 5 birim arttırılmaktadır. Bununda 5 farklı Task örneği ile yapıldığı düşünüldüğünde toplamda elde edilen sonuçların aslında her deneme için aynı olması beklenmektedir. Örnek çıktıda ise sadece 2472500 için iki hesaplamanın aynı olduğu görülmektedir. Sorun ne olabilir?

Paralel olarak başlatılan Task örneklerine ait kod blokları, işlemcinin durumuna göre farklı zamanlarda devreye girer ve yürütülürler. Buradaki zaman farkları çok küçük birimlere denk gelse de, örnekteki gibi ortak olarak paylaşılan veriler söz konusu olduğunda beklenmeyen sonuçların üretilmesine neden olabilirler. Geliştirilen örnek düşünüldüğünde; çalışma zamanında herhangibir t anındaki j değerleri, her bir Task için farklı olabilir. Yani Task 1, 500 nolu j değerini işlemekteyken, Task 2 halen 345nci değerinde olabilir. Dolayısıyla ortak olarak kullanılan Altitude değeri, her denemede farklı artımlarla değiştirilebilmektedir. Peki nasıl bir çözüm uygulanabilir?

Aslında her bir Task örneğinin kendi kapsamında ele alacağı bir değişken ile bu sorun çözülebilir. Ancak ek olarak, en sonda kümülatif bir hesaplama yapılması ve işlerin birleştirilmesi de gerekmektedir. Bunu aşağıdaki örnek kod parçasında daha net bir şekilde görebiliriz.

```csharp
using System; 
using System.Threading.Tasks;

namespace SharedDataScenarios 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            for (int i = 0; i < 10; i++) 
            { 
                TestMethod(); 
            }

            Console.WriteLine("İşlemler tamamlandı.\nProgramı kapatmak için bir tuşa basınız"); 
            Console.ReadLine(); 
        }

        private static void TestMethod() 
        { 
            Plane f16 = new Plane(); 
            Task<int>[] tasks = new Task<int>[5];

            for (int i = 0; i < 5; i++) 
            { 
                tasks[i] = new Task<int>((os) => 
                { 
                    int currentAltitude = (int)os;

                    for (int j = 0; j < 1000; j++) 
                    { 
                        currentAltitude += j - 5; 
                    }

                    return currentAltitude; 
                },f16.Altitude 
                ); 
                tasks[i].Start(); 
            }

            Task.WaitAll(tasks);

            for (int i = 0; i < tasks.Length; i++) 
            { 
                f16.Altitude += tasks[i].Result; 
            }

            Console.WriteLine("[ {0} ]", f16.Altitude); 
        } 
    }

    class Plane 
    { 
        public int Altitude { get; set; } 
    } 
}
```

Kodda, Task örnekleri oluşturulurken, her birinin kendi Altitude değişkeni ile çalışması için gerekli düzenlemeler yapılmıştır. Dikkat edileceği üzere Task sınıfına ait yapıcı metod (Constructor) ikinci bir parametre daha almaktadır. Bu parametre, lambda ifadesi içerisine taşınacak olan bir Object State değişkenini ifade etmektedir. Dolayısıyla bir Task örneklenirken, kendi kapsamında bir Altitude değişkeni ile çalışacaktır.

Tabiki yerel olarak ele alına bu değişkenlerin kodun dışarısında tekrardan bütünleştirilmesi gerekeceğinden Task örneklerinden birer sonuç döndürülmesi gerekmiştir. Söz konusu dönüş değerleri daha sonradan toplanmış ve aşağıdaki çalışma zamanı çıktısının oluşması sağlanmıştır.

[![blg210_Test2](/assets/images/2011/blg210_Test2_thumb.gif)](/assets/images/2011/blg210_Test2.gif)

Görüldüğü üzere 10 denemenin her birisinde aynı sonuç elde edilmiştir.

## TLS (Thread Local Storage)

Aslında yukarıda geliştirilen kod parçasında.Net çalışma zamanının herhangibir zorlayıcılığı olmadığını ifade edebiliriz. Bu zorlayıcılık içinse, Thread Local Storage isimli özel alanlardan yararlanabiliriz. Bu depoları birden fazla iş parçası için ayrıştırılmış özel veri alanları olarak düşünülebiliriz. TLS vakasına göre, her bir Thread kendisine ait yerel depolama alanına sahiptir. Bu durumda herhangibir iş parçası, bir diğerinin TLS bölgesine müdahalede bulunamaz. Bir başka deyişle diğer bir iş parçasının izole edilmiş veri bölgesini okuyamaz veya yazamaz. Söz konusu alanlar, yönetimli kod tarafında (Managed Code) ThreadLocal sınıfı yardımıyla ele alınabilirler. Biraz önce geliştirdiğimiz örnek kod parçasında TLS kullanımını aşağıdaki gibi gerçekleştirebiliriz.

```csharp
using System; 
using System.Threading.Tasks; 
using System.Threading;

namespace SharedDataScenarios 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            for (int i = 0; i < 10; i++) 
            { 
                TestMethod(); 
            }

            Console.WriteLine("İşlemler tamamlandı.\nProgramı kapatmak için bir tuşa basınız"); 
            Console.ReadLine(); 
        }

        private static void TestMethod() 
        { 
            Plane f16 = new Plane(); 
            Task<int>[] tasks = new Task<int>[5]; 
            ThreadLocal<int> local = new ThreadLocal<int>();

            for (int i = 0; i < 5; i++) 
            { 
                tasks[i] = new Task<int>((os) => 
                { 
                    local.Value = (int)os;

                    for (int j = 0; j < 1000; j++) 
                    { 
                        local.Value += j - 5; 
                    } 
                    return local.Value; 
                },f16.Altitude 
                ); 
                tasks[i].Start(); 
            }

            Task.WaitAll(tasks);

            for (int i = 0; i < tasks.Length; i++) 
            { 
                f16.Altitude += tasks[i].Result; 
            }

            Console.WriteLine("[ {0} ]", f16.Altitude); 
        } 
    }

    class Plane 
    { 
        public int Altitude { get; set; } 
    } 
}
```

Bu kez System.Threading isim alanı (Namespace) altında yer alan ThreadLocal tipinden bir örnek oluşturulmuş ve lambda ifadesi içerisindeki Object State atamasında kullanılmıştır. İşlemlerin tamamı bu örneğe ait Value özelliği üzerinden yapılmaktadır. Sonuçlar bir önceki ile aynı olacaktır.

[![blg210_Test3](/assets/images/2011/blg210_Test3_thumb.gif)](/assets/images/2011/blg210_Test3.gif)

## ThreadLocal Lazy Initialization Kullanımı ve Tuzak

ThreadLocal tipinin kullanımında değerlendirilebilecek bir versiyon daha bulunmaktadır. Aşırı yüklenmiş olan yapıcı metod, Func tipinden bir tesmilci (Delegate) almaktadır.

[![blg210_OverloadVersion](/assets/images/2011/blg210_OverloadVersion_thumb.gif)](/assets/images/2011/blg210_OverloadVersion.gif)

Bu versiyonda Lazy Initialization söz konusudur. Bu sebepten çok dikkatli olunması gerekmektedir. Func temsilcisi, izole edilmiş veri değişkeninin oluşturulması aşamasında devreye girecek bloğu işaret etmektedir. Lakin bu blok, ThreadLocal örneğinin Value özelliği çağırılıncaya kadar devreye girmeyecektir. İşte bu sebepten yazımızda ele aldığımız senaryo için yine farklı sonuçların elde edilmesi söz konusu olabilir. Kodumuzu buna göre aşağıdaki gibi geliştirdiğimizi düşünelim.

```csharp
using System; 
using System.Threading.Tasks; 
using System.Threading;

namespace SharedDataScenarios 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            for (int i = 0; i < 10; i++) 
            { 
                TestMethod(); 
            }

            Console.WriteLine("İşlemler tamamlandı.\nProgramı kapatmak için bir tuşa basınız"); 
            Console.ReadLine(); 
        }

        private static void TestMethod() 
        { 
            Plane f16 = new Plane(); 
            Task<int>[] tasks = new Task<int>[5]; 
            ThreadLocal<int> local = new ThreadLocal<int>( 
                () => 
                { 
                    Console.WriteLine("{0}",f16.Altitude); 
                    return f16.Altitude; 
                } 
            );

            for (int i = 0; i < 5; i++) 
            { 
                tasks[i] = new Task<int>(() => 
                { 
                    for (int j = 0; j < 1000; j++) 
                    { 
                        local.Value += j - 5; 
                    } 
                    return local.Value; 
                } 
                ); 
                tasks[i].Start(); 
            }

            Task.WaitAll(tasks);

            for (int i = 0; i < tasks.Length; i++) 
            { 
                f16.Altitude += tasks[i].Result; 
            }

            Console.WriteLine("[ {0} ]", f16.Altitude); 
        } 
    }

    class Plane 
    { 
        public int Altitude { get; set; } 
    } 
}
```

Dikkat edileceği üzere Task örneklerinin kullanacağı Altitude değerinin, izole edilmiş yerel değişkene set edilmesi işlemi, ThreadLocal örneklemesi yapılırken Func temsilcisinin işaret ettiği blok içerisinde yapılmaktadır. Bu nedenle Object State kullanımına gerek kalınmamıştır. Ne varki çalışma zamanı sonuçları beklediğimiz gibi olmayacaktır.

[![blg210_Test4](/assets/images/2011/blg210_Test4_thumb.gif)](/assets/images/2011/blg210_Test4.gif)

Herşeyden önce 10 denemenin çoğu kendi aralarında farklı sonuçlar vermiş ve hatta bir önceki örnekteki ile alakası olmayan çıktılar üretilmiştir. Diğer yandan ThreadLocal örneklemelerinin hemen her bir deneme için (nitekim sonlarda bire düşmüştür ve her çalışmada bu değişebilir) 2 kez çağırıldığı görülmektedir. Bunun sebebi ise, örneğin geliştirildiği makinenin çift çekirdekli olmasıdır. Öyleki TLS tekniğinde Task örnekleri değil Thread’ ler söz konusudur ve makine çift çekirdekli olduğundan aslında çalışma zamanında iki Thread yürümektedir ki bunlarda her 5 Task örneğini paylaşır. Piuvvvvvv!!!

Bu yazımızda biraz karmaşık ve anlaşılması zor olan bir konuyu gündeme getirmeye çalıştık. Aslında bu noktada ele alacağımız daha bir sürü vaka var. Fakat paralel programlamanın, Yönetimli Kodda (Managed Code) bile olsa çok dikkatli uygulanması gerektiğini bir kere daha gördük. Özellikle son örneğimize göre ThreadLocal kullanımı sırasında çok dikkatli olunması ve testlerin mutlaka iyi bir şekilde yapılması gerektiğini ortaya koyduk. Her an biz tuzağa düşebiliriz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[SharedDataScenarios_.rar (25,91 kb)](/assets/files/2011/SharedDataScenarios_.rar)[Örnek Visual Studio 2010 Ultimate Sürümünde Geliştirilmiş ve Test Edilmiştir]
