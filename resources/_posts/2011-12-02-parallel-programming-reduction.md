---
layout: post
title: "Parallel Programming–Reduction"
date: 2011-12-02 11:51:00 +0300
categories:
  - parallel-programming
tags:
  - task-parallel-library
  - parallel-programming
  - reduction
  - map-reduce
---
Neredeyse son bir kaç saattir yoğun bir şekilde Wolfenstein isimli bilgisayar oyununu oynamaktaydım. Aslında çok fazla bilgisayar oyunu oynayan birisi değilimdir. Hatta bu oyunun ilk versiyonunu çok çok uzun zaman önce oynadığımı ve arada çok az oyunla haşır neşir olduğumu itiraf edebilirim

[![wolfenstein](/assets/images/2011/wolfenstein_thumb.jpg)](/assets/images/2011/wolfenstein.jpg)


![Confused smile](/assets/images/2011/wlEmoticon-confusedsmile_12.png)

Lakin bazen oyun perisi gelip beni bir dürtmekte ve saatlerce bilgisayar başından kalkmadan oyun oynamamı istemekte. Bu gece kendisini kıramadım işte

![Smile](/assets/images/2011/wlEmoticon-smile_21.png)

Aslında gece boyunca Wolfenstein her ne kadar beni aşırı derece de sürüklemiş olsa da aklımın bir köşesinde beni kemiren paralel programlama konulu düşüncelerimin önüne de geçemedim. Doğruyu söylemek gerekirse şu anda çok geç bir saat de olsa konuyu açıklığa kavuşturmanın ve bununla ilişkili bir blog girdisi üretmenin tam zamanıdır diye düşündüm ve işte karşınızdayım. Oyun perisinin ensesinde biten ilham perisinin isteğiydi bu sanırım. Lafı fazla uzatmadan konuya giriş yapalım dilerseniz.(Bunu belirtmek istedim çünkü bu kısa girişleri sevmeyen bir sürü geliştirici de tanıyorum![Smile](/assets/images/2011/wlEmoticon-smile_21.png))

Olay paralel programlamada veriyi paralelize etmek ile alakalı. Aslında çok basit ama gözden kaçtığı takdirde önemli hatalara neden olabilecek bir durum söz konusu. Bunu izah etmenin en iyi yolu belkide basit bir örnek üzerinden ilerlemekle olacaktır. Şimdi şöyle düşünelim; elimizde yüsek boyutlu sayısal bir dizi veya koleksiyon olsun ve biz bu sayı kümesi üzerinde örneğin 7 ile tam bölünebilenlerin sayısını bulmak istediğimizi var sayalım. Standart bir for döngüsü ile bu işlemi yapabileceğimiz gibi, çok yüksek boyutlu bir sayı olması halinde Parallel.For veya Parallel.ForEach metodlarını da söz konusu hesaplama için kullanabiliriz pekala

![Winking smile](/assets/images/2011/wlEmoticon-winkingsmile_72.png)

Elimizde çok çekirdekli veya çok işlemcili bir sistem var ise, paralel döngüleri kullanmak pek çok açıdan avantajlı olabilir nitekim. Şimdi az önce bahsetmiş olduğumuz senaryoyu aşağıdaki Console uygulamasına ait kod satırlarında simüle ettiğimizi var sayalım.

```csharp
using System; 
using System.Collections.Generic; 
using System.Threading.Tasks;

namespace ReductionSample 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            List<int> numbers = Helper.GetRandomNumberList(9000000);

            #region Klasik Yol

            int count=0; 
            for (int i = 0; i < numbers.Count; i++) 
            { 
                if (numbers[i] %7==0) 
                    count++;

            }

            Console.WriteLine("[Klasik For] 7 ile bölünebilen {0} sayı vardır",count.ToString());

            #endregion

           #region Parallel For

            int parallelCount=0;

            Parallel.For(0, numbers.Count, (i) => 
            { 
                if (numbers[i] % 7 == 0) 
                    parallelCount++; 
           } 
            );

            Console.WriteLine("[Parallel.For] 7 ile bölünebilen {0} sayı vardır",parallelCount.ToString());

            #endregion

        } 
    }

    static class Helper 
    { 
        public static List<int> GetRandomNumberList(int NumberCount) 
        { 
            List<int> result = new List<int>();
            Random rnd = new Random(); 
            for (int i = 0; i < NumberCount; i++) 
            { 
                result.Add(rnd.Next(1, 100)); 
            }
            return result; 
        }
    } 
}
```

static olarak tanımlanmış olan Helper sınıfı belirli sayıda rastgele tam sayı üretmek üzere yazılmış GetRandomNumberList isimli bir metod içermektedir. Uygulamaya ait Main metodu içerisinde ise önce standart bir for döngüsü ile ardından da bunun paralel versiyonu ile birer iterasyon gerçekleştirilmekte ve 7 ile bölünebilen sayıların toplam sayısı hesap edilerek ekrana yazdırılmaktadır.

Aslında sayı aralığı ne kadar yüksek olursa standart for döngüsünün hesaplama için daha fazla zaman harcayacağı ve Parallel.For döngüsünün aynı işi daha kısa sürede bitireceği aşikardır. Ne varki burada öngöremediğimiz ve belki de tahmin etmediğimiz bir durum daha söz konusudur. Bunu anlamak için kodun çalışma zamanındaki bir kaç çıktısına bakabiliriz

![Winking smile](/assets/images/2011/wlEmoticon-winkingsmile_72.png)

Birinci çalıştırmanın sonuçları;

[![artcl_1_1](/assets/images/2011/artcl_1_1_thumb.gif)](/assets/images/2011/artcl_1_1.gif)

O oooo!!!

![Surprised smile](/assets/images/2011/wlEmoticon-surprisedsmile_1.png)

İkinci çalıştırmanın sonuçları;

[![artcl_1_2](/assets/images/2011/artcl_1_2_thumb.gif)](/assets/images/2011/artcl_1_2.gif)

O ooooo too!!!

![Surprised smile](/assets/images/2011/wlEmoticon-surprisedsmile_1.png)

Üçüncü çalıştırmanın sonuçları;

[![artcl_1_3](/assets/images/2011/artcl_1_3_thumb.gif)](/assets/images/2011/artcl_1_3.gif)

Şaka mı bu?

![Surprised smile](/assets/images/2011/wlEmoticon-surprisedsmile_1.png)

Dikkat edilecek olursa Parallel.For döngüsü 7 ile bölünebilen sayıların miktarını her defasında standart for döngüsüne göre farklı hesaplamış ve aslında hiç birisinde de doğru sonucu tutturamamıştır. Bu aslına bakılacak olursa son derece doğal bir sonuçtur. Nitekim Parallel.For döngüsü çalışmaya başladıktan sonra birden fazla Task oluşturmakta ve bunları bir veya daha fazla Thread’ in yönetimine sunmaktadır. Gözden kaçan nokta, bu Task’ ların her birinin aslında aynı değişken üzerinde hesaplama yapmaya çalışıyor olmalarıdır. Yani, örneğimizde açılan Task bloklarının her biri aslında aynı parallelCount değişkeni üzerinde bir artım gerçekleştirmeye çalışmaktadır. Bu da çok doğal olarak doğru sonucun hesaplanamamasına neden olmaktadır. Peki ya öyleyse çözüm nedir?

Standart bir for döngüsünün kullanılması tercih edilebilir

![Open-mouthed smile](/assets/images/2011/wlEmoticon-openmouthedsmile_19.png)

Lakin bu durumda Parallel olmanın avantajları kaybedilecektir. Buradaki gibi bir sayı aralığında bu çok önemli gözükmese de, bilimsel veya finansal hesaplama yapan bir uygulamada bu ayrım, performans açısında çok kritik bir fark doğurabilir. Bu nedenle Parallel döngüler ile devam edecek isek Reduction olarak tanımlanan ve aslında ilerleyen zamanlarda işleyeceğimiz MapReduce deseninin temelini oluşturan bir konuyu göz önüne alarak ilerlememiz gerekmektedir. Aslında teorik olarak çok fazla sıkmak istemiyorum sizi, ancak özet olarak işleyişi ifade etmek isterim. Parallel.For döngüsünü öyle bir çalıştırmalıyız ki, açılacak olan Thread’ ler ve bunların içerisinde yer alacak olan Task’ lar, parallelCount sayısının aslında aralarında paylaştıkları bir veri olduğunu bilmelidirdler

![Winking smile](/assets/images/2011/wlEmoticon-winkingsmile_72.png)

Üstelik her bir Thread kendi içerisinde bir toplam sayı hesabı yapmalı ve işleyişini bitirdiğinde de herkes için ortak olan bir değişkene bunu eklemelidir. Bu ekleme işinin ise senkronize edilerek yapılması şarttır.

Neyseki Parallel.For ve Parallel.ForEach döngüleri bu ortak paylaşımlı veri değişkenlerine izin veren aşırı yüklenmiş (Overload) versiyonlara sahiptirler. Çözüm olarak kodumuzu aşağıdaki gibi değiştirmemiz yeterli olacaktır.

```bash
#region Reduction

int reductionCount = 0;

Parallel.For(0, numbers.Count, 
    () => 0, 
    (i, state, currentTotal) => 
    { 
        if (numbers[i] % 7 == 0) 
            currentTotal++;

        return currentTotal; 
    }, 
    (currentTotal) => 
    { 
        Console.WriteLine("{0} Current Total {1}",Thread.CurrentThread.ManagedThreadId,currentTotal.ToString()); 
        Interlocked.Add(ref reductionCount, currentTotal); 
    } 
    );

Console.WriteLine("[Reduction] 7 ile bölünebilen {0} sayı vardır",reductionCount.ToString());

#endregion
```

Aslında Parallel.For için biraz karmaşık bir yazım söz konusu. İlk iki parametre tanıdık. Üçüncü parametrede herhangibir işlem yapmadan geçiyoruz. Önemli olan ise metodun aldığı son iki parametre. Bunlardan ilki Func tipinden. Diğeri ise Action türündendir. Teorik olarak yapılmak istenen her bir Thread’ in kendi içerisinde değerlendireceği özel bir yerel değişken oluşturmak (Private Thread-Local Variable) ve tüm paralel çalışma tamamlandığında bu özel yerel değişkenlerin bir toplamını hesaplayarak sonuca ulaşmaktır. Bir başka deyişle açılan Thread’ ler kendi özel toplam değişkenlerini arttıracaklardır. Bu işlemi,

```csharp
(i, state, currentTotal) => 
{ 
     if (numbers[i] % 7 == 0) 
          currentTotal++;

     return currentTotal; 
}
```

kısmı gerçekleştirmektedir.

Paralel çalışmaya dahil olan Thread’ ler işlemlerini bitirdikten sonra da,

```csharp
(currentTotal) => 
{ 
        Console.WriteLine("{0} Current Total {1}",Thread.CurrentThread.ManagedThreadId,currentTotal.ToString()); 
        Interlocked.Add(ref reductionCount, currentTotal); 
}
```

kodu devreye girmekte ve Thread’ ler için hesaplanan currentTotal değerlerini ref ile reductionCount değerine eklemektedir. Böylece tüm Thread’ lerin kendi yerel alanlarındaki veriler üzerinde yaptığı 7 ile bölünebilen sayıların miktarı, birleştirilmektedir. Interlocked burada devreye giren önemli bir fonksiyonellik sunmakta ve senkronize bir şekilde currentTotal değerlerinin reductionTotal değişkenine eklenmesine olanak sağlamaktadır. İşte bir koleksiyon içeriğinin bu şekilde tekil bir değere indirgenmesine Reduction adı verilmektedir.

Örneğimizi az önceki testte olduğu gibi yine arka arkaya 3 defa çalıştırırsak aşağıdaki ekran görüntüsünde yer alanlara benzer sonuçları elde ettiğimizi görebiliriz.

İlk çalışma sonucu;

[![artcl_1_4](/assets/images/2011/artcl_1_4_thumb.gif)](/assets/images/2011/artcl_1_4.gif)

ikinci çalışma sonucu;

[![artcl_1_5](/assets/images/2011/artcl_1_5_thumb.gif)](/assets/images/2011/artcl_1_5.gif)

üçüncü çalışma sonucu

[![artcl_1_6](/assets/images/2011/artcl_1_6_thumb.gif)](/assets/images/2011/artcl_1_6.gif)

![Winking smile](/assets/images/2011/wlEmoticon-winkingsmile_72.png)

Görüldüğü gibi Reduction tekniğine göre yapılan hesaplama sonuçları ile klasik for döngüsü ile yapılan hesaplama sonuçları bire bir örtüşmektedir. Elbetteki her çalışma sonrasında Parallel.For döngüsünün başlatacağı Thread’ ler farklı olacaktır. Bu yüzden son Action temsilcisinin icrası sonucu üretilen currentTotal değerleri farklılıklar gösterecektir. Bu yüzden Current Total değerleri hep farklı sonuçlar vermiştir.

Özetle Reduction tekniğini kullandığımızda, paralel olarak işletilen Thread’ lerin kendi yerel değişkenleri ile çalışmaları sağlanabilmekte ve bunlar sonuç olarak tek bir değişkene indirgenerek bu tip senaryolarda göz önüne alınabilmektedir. Yazımızın başında belirttiğimiz üzere Reduction tekniği aslında MapReduce deseninin temelini oluşturan önemli bir kavramdır. Bu deseni ilerleyen yazılarımızda sizlere örnek bir senaryo üzerinden aktarmaya çalışıyor olacağım. Eğer bu ve bunun gibi diğer paralel programlama desenlerini merak ediyorsanız Microsoft’ un ücretsiz olarak indirebileceğiniz [Patterns for Parallel Programming: Understanding and Applying Parallel Patterns with.Net Framework 4.0](http://www.microsoft.com/download/en/details.aspx?id=19222) dökümanını okuyabilirsiniz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[ReductionSample.rar (24,88 kb)](/assets/files/2011/ReductionSample.rar)