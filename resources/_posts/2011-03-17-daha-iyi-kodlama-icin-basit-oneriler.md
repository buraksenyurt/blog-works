---
layout: post
title: "Daha iyi Kodlama için Basit Öneriler"
date: 2011-03-17 07:01:00 +0300
categories:
  - csharp
tags:
  - csharp
  - .net-framework
---
Bilenler bilir, uzun süredir.Net Framework 2.0 üzerinde yazılmış ve Visual Studio 2005 ortamında geliştirilmeye devam edilen bir bankacılık uygulamasında görevliyim. Buradaki işim müşterinin yeni isteklerini sisteme katmak/katmaya çalışmak olarak düşünülebilir

[![blg225_Giris](/assets/images/2011/blg225_Giris_thumb.jpg)](/assets/images/2011/blg225_Giris.jpg)


![Sealed](/assets/images/2011/smiley-sealed.gif)

Aslında bu yeni görevime atanırken Visual Studio 2010 ile uğraştığımı hatırlıyorum da…Kendimi Ice Age filmindeki buz çağı devrine dönmüş canlı bir yaşam formu gibi zannetmiştim

![Laughing](/assets/images/2011/smiley-laughing.gif)

Geliştirilmiş olan ürüne ait proje kodları biraz eski tarihli olduğundan ilk zamanalarda içinde kaybolduğumu belirtebilirim. Günlerce Debug yapıp süreçlerin nasıl işlediğini anlamaya çalıştım. (E haliyle koda dökümantasyon yazmassanız, XML Comment’ ler koymassanız, proje geliştirilmekteyken sürekli eleman sirkülasyonuna izin verir ve herkesin günü kurtarmak adına kodlama yapmasına davetiye çıkartırsanız, analiz dökümanlarını saklamaz veya güncellemesseniz olacağı da budur

![Undecided](/assets/images/2011/smiley-undecided.gif)

)

Aslında daha okunaklı, daha efektif ve zaman zaman daha verimli kod üretmek için bir kaç küçük noktaya dikkat etmekte yarar olabilir. Bu sayede sanıyorum ki kodlarımız en azından daha şık duracaktır

![Wink](/assets/images/2011/smiley-wink.gif)

Gelin bu bir kaç küçük püf noktadan bir kaçına hep birlikte bakalım.

## 1 – Auto Property’ ler ve Private Block’ lar

İlk olarak özellikle POCO (Plain Old CLR Objects) gibi tip tanımlamalarında yaygın olarak kullandığımız ve özellikle LINQ (Language Integrated Query) tarafında da çok işimize yarayan Auto Property kavramına bir bakalım. Bazı durumlarda özelliklerin sadece okunabilir (Read Only) ya da yazılabilir (Write Only) olmalarını isteyebiliriz. Auto Property kullanmadığımız durumlarda get veya set bloklarını yazmayarak bu kolayca sağlanabilir. Ancak Auto Property’ lerde get veya set bloklarının her ikisinin de yazılması zorunludur. Buna rağmen normal Property yazımındaki felsefe ile hareket ettiğimizde derleme zamanında aşağıdaki ekran görüntüsünde yer alan hata mesajı ile karşılaşırız.

[![blg225_AutoPropertyError](/assets/images/2011/blg225_AutoPropertyError_thumb.gif)](/assets/images/2011/blg225_AutoPropertyError.gif)

Görüldüğü üzere sadece get veya sadece set bloğu yazarak readonly veya writeonly özellik tanımlanamaz. Ancak private gibi bir erişim belirleyicisini (Access Modifier) devreye alırsak amacımıza ulaşabiliriz. İşte örnek bir kod parçası.

```csharp
using System;

namespace OldTimes 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        {

            #region Auto Property ve Private get,set

            Person prd = new Person 
            { 
                 PersonId=10, 
                  Name="Burak", 
                   Birth=new DateTime(1976,12,4) 
            }; 
            Console.WriteLine(prd.Birth);

            #endregion

        }

    }

    #region Auto Property

    class Person 
    { 
        public int PersonId { get; private set; } 
        public string Name { get; set; } 
        public DateTime Birth { private get; set; } 
    }

    #endregion    
}
```

Dikkat edileceği üzere PersonId ve Birth özelliklerinde get ve set bloklarının başında private bildirimi yapılmıştır. Bu durumda PersonId sadece okunabilir, Birth ise sadece yazılabilir özellikler olarak değerlendirilmektedir. Hatta kodun bu halini derlediğimizde aşağıdaki hata mesajlarını aldığımızı görürüz.

[![blg225_AutoProperty](/assets/images/2011/blg225_AutoProperty_thumb.gif)](/assets/images/2011/blg225_AutoProperty.gif)

Peki üretilen IL (Intermediate Language) çıktısına baktığımızda

![Wink](/assets/images/2011/smiley-wink.gif)

Bu durumda aşağıdaki sonuçlar ile karşılaşırız.

[![blg225_AutoPropertyIL](/assets/images/2011/blg225_AutoPropertyIL_thumb.gif)](/assets/images/2011/blg225_AutoPropertyIL.gif)

Dikkat edilmesi gereken nokta private olarak işaretlenmiş get ve set metodlarının varlığıdır. Üstelik bu metodların içerisinde iş yapan kod parçaları da mevcuttur. Ancak bu ip ucunda dikkat edilmesi gereken bir husus daha vardır. Bu farkı görmek için eski stilde ReadOnly bir özelliğin nasıl tanımlandığına bakalım.

```csharp
namespace OldTimes 
{ 
    class Product 
    { 
        private int _productId=0;

        public int ProductId 
        { 
            get { return _productId; } 
        } 
    } 
...
```

Product tipi içerisinde yer alan ProductId özelliği Readonly olarak tanımlanmıştır. Nitekim bir set bloğu yoktur. Bunun doğal olarak IL çıktısı ise aşağıdaki gibi olacaktır.

[![blg225_Readonly](/assets/images/2011/blg225_Readonly_thumb.gif)](/assets/images/2011/blg225_Readonly.gif)

Farkı görebildiniz mi?

![Wink](/assets/images/2011/smiley-wink.gif)

Dikkat edileceği üzere setProductId isimli bir metod IL üretimi içerisine dahil edilmemiştir. Belki de auto property’ lerin en sevmediğim yanı budur. Keşke sadece get veya sadece set tanımlamasına izin verilseymiş.

## 2 -?? Operatörü

Zannedersem, C# dili içerisinde atalarından gelen ternary operatörünü (?:) bilmeyen, hatta kullanmayan yoktur. Tamam tamam kabul ediyorum. Çoğu durumda if…else… bloklarını kullanmayı tercih etmekteyiz. Ancak?: operatörü kadar ilginç olan ve genellikle pek kullanmadığımız bir operatörümüz daha vardır. Null kontrol operatörü (??). İlk olarak aşağıdaki kod parçasını göz önüne alarak işe başlayalım.

```csharp
using System;

namespace OldTimes 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            #region ?? Operator

            IPv4 incomingIp=null; 
            SendPacket(incomingIp); 
            #endregion 
        }

        #region ?? Operator

        static void SendPacket(IPv4 targetAddress) 
        { 
            IPv4 ip=null; 
            if (targetAddress == null) 
            { 
                ip = new IPv4 { Block1 = 127, Block2 = 0, Block3 = 0, Block4 = 1 }; 
            } 
            Console.WriteLine("{0} adresine gönderim yapılıyor",ip.ToString()); 
        }

        #endregion 
    }

    #region ?? Operator 
    class IPv4 
    { 
        public byte Block1 { get; set; } 
        public byte Block2 { get; set; } 
        public byte Block3 { get; set; } 
        public byte Block4 { get; set; }

        public override string ToString() 
        { 
            return String.Format("{0}.{1}.{2}.{3}", Block1, Block2, Block3, Block4); 
        } 
    } 
    #endregion 
}
```

Çoğu zaman bazı referans örneklerinin null değere sahip olup olmadıklarını denetlememiz gerekir. Yukarıdaki örnek kod parçasında bu durum analiz edilmeye çalışılmaktadır. SendPacket isimli static metod IPv4 isimli sınıfa ait bir nesne örneğini parametre olarak alır. Metod içerisinde öncelikli olarak bir null kontrolü yapılır. Bunun için de standart ve yaygın yol if…else blokları kullanılmıştır. Aslında metod içerisinde ternary operatörü kullanılaraktan da aynı işlem yapılabilir.

ip = targetAddress == null? new IP { Line1 = 127, Line2 = 0, Line3 = 0, Line4 = 1 }: targetAddress;

? işaretinden önce targetAddress değişkeninin null olup olmadığına bakılmaktadır.? ile: işaretleri arasındaki kısım koşulun true olması halini ele alırken,: işaretinden sonraki kısım false olma durumunu değerlendirmektedir. Aslında burada null kontrolü yapıldığından?? operatörü de ele alınabilir. Aşağıdaki gibi

![Wink](/assets/images/2011/smiley-wink.gif)

ip = targetAddress?? new IP { Line1 = 127, Line2 = 0, Line3 = 0, Line4 = 1 };

Daha yalın daha okunaklı olduğunu ifade edebilir miyiz? Bu sorunun cevabı duruma göre değişir. Ancak en azından şirketinizin bir kod standardı var ise, bu tip null kontrollerinde nasıl bir yol izlenilmesi gerektiği ve hangi operatörlerin kullanılması gerektiği açıktır. Söz gelimi daha önceden çalıştığım şirketlerin birisinde?: operatörünün kullanımı yasaklanmıştır. (Sebebini hiç sormayın ama sonuç itibariyle bir standart vardı en azından

![Wink](/assets/images/2011/smiley-wink.gif)

)

## 3 – As operatörü

Hazır?? operatörüne değinmişken as operatörünü de es geçmemek taraftarıyım. Yine projelerimizde pek çok nokta da bir referans örneğinin null olmaması halinde dönüştürme işlemi yapılarak ilerlenilmesi söz konusudur. Aşağıdaki kod parçasını göz önüne alarak 3ncü maddemizi kavramaya çalışalım.

```csharp
using System;

namespace OldTimes 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            #region AS Operator

            DrawShape(new Rectangle(), 10, 5); 
            DrawShape(new Circle(), 3, 6);

            #endregion 
        }

        #region AS Operator 

        static void DrawShape(Shape shape,int x,int y) 
        { 
            Rectangle rect = null; 
            Circle crcl = null;

            if (shape is Rectangle) 
            { 
                rect = (Rectangle)shape; 
                rect.Draw(x, y); 
            } 
            else if (shape is Circle) 
            { 
                crcl = (Circle)shape; 
                crcl.Draw(x, y); 
            }  
        } 

        #endregion        
    }

    #region AS Operator

    public class Shape 
    { 
    }

    public class Circle 
        : Shape 
    { 
        public void Draw(int x, int y) 
        { 
            Console.WriteLine("{0}:{1} koordinatına Daire",x,y); 
        } 
    }

    public class Rectangle 
        : Shape 
    { 
        public void Draw(int x, int y) 
        { 
            Console.WriteLine("{0}:{1} koordinatına dörtgen", x, y); 
        } 
    }

    #endregion  
}
```

Örnek kod içerisinde yer alan DrawShape metodu parametre olarak Shape sınıfında bir örnek almakadır. İçeride ise gelen örneğin Rectangle veya Circle olup olmadığına bakılarak bir dönüştürme işlemi gerçekleştirilmekte ve sembolik bir çizim işlemi yürütülmektedir. Ancak dikkat edilmesi gereken nokta önce is operatörü ile bir tip kontrolü yapılması sonrasında ise eğer tip beklenen tip ise bilinçli bir tip dönüştürme işlemi yapılmasıdır. Bir başka deyişle is operatörü de kontrol için bir dönüştürme işlemi ele almakta ve bu da gereksiz yere iki tip dönüşümüne neden olmaktadır. Aslında bu kod yerine aşağıdaki teknikte uygulanabilir.

```csharp
Rectangle rect = shape as Rectangle; 
Circle crcl = shape as Circle;

if (rect != null) 
    rect.Draw(x,y); 
else if (crcl != null) 
    crcl.Draw(x,y);
```

as operatörü sağ tarafındaki değişkeni sol tarafındaki tipe dönüştürmeye çalışır ve bunda başarılı olamassa geriye null değer döndürür. Dolayısıyla tek bir kontrol ve koşul doğru ise dönüştürerek atama işlemi gerçekleştirilmektedir. Sonrasında ise tek yapılması gereken null kontrolünü gerçekleştirmek ve buna göre ilgili operasyonu devreye almaktır.

## 4 – Timespan Yardımcı Metodları

Aslında.Net Framework içerisinde yer alan tipleri iyi bir şekilde öğrenmek ve bilmekte her zaman için yarar vardır. Hiç ummadığımız yerlerde işimize yarayacak fonksiyonellikler bulabiliriz. Örneğin bunlardan birisi zaman farkı belirtilmesi gereken yerlerde kullanılması önerilen TimeSpan tipidir. Ne demek istediğimi biraz daha net anlayabilmek için gelin aşağıdaki örnek kod parçasını göz önüne alalım.

```csharp
using System; 
using System.Threading;

namespace OldTimes 
{ 
    class Program 
    {        
        static void Main(string[] args) 
        { 
            #region Timespan

            double r1=Calculate(1, 100, 10); 
            Console.WriteLine(r1);

            #endregion 
        }

        #region

        static double Calculate(int x,int y,int duration) 
        { 
            double result=0;

            for (int i = x; i < y; i++) 
            { 
                result+=i; 
                Thread.Sleep(duration); 
            } 
            return result; 
        }

        #endregion 
    }  
}
```

Calculate metodu sembolik olarak belirli aralıkta sayıların toplamını hesap etmek üzere çalışmakta olan bir metoddur. Metodun for döngüsü içerisinde ise belirli süreliğine çalışmakta olan Thread’ in Sleep metodu yardımıyla uyutulması sağlanmaktadır. Aslında bu sürenin milisaniye cinsinden olduğunu hepimiz biliyoruz. Yine de zaman zaman unuttuğumuz anlar olabiliyor ve milisaniye miydi yoksa saniye miydi diyebiliyoruz? Dolayısıyla geliştiricinin var ise ve eğer yazılmışsa XML Comment’ ine bakmasın gerekiyor. Gelin bu işi Developer’ ın insiyatifinden çıkartalım. İşte TimeSpan kullanımı…

```csharp
using System; 
using System.Threading;

namespace OldTimes 
{ 
    class Program 
    {        
        static void Main(string[] args) 
        { 
            #region Timespan

            double r1=Calculate(1, 5, TimeSpan.FromSeconds(1)); // o andan itibaren 1er saniye aralıklarla 
            Console.WriteLine(r1); 
            double r2=Calculate(1, 5, new TimeSpan(0, 0, 2)); // 0 saat 0 dakika 2 saniye aralıkla 
            Console.WriteLine(r2); 
            double r3 = Calculate(1, 5, TimeSpan.FromMilliseconds(50)); // o andan itibaren 50şer milisaniye aralıklarla 
            Console.WriteLine(r3); 
            double r4 = Calculate(1, 5, new TimeSpan(0, 0, 0, 0, 250)); // 0 gün 0 saat 0 dakika 0 saniye 250 milisaniye aralıkla

            #endregion 
        }

        #region

        static double Calculate(int x,int y,TimeSpan duration) 
        { 
            double result=0;

            for (int i = x; i < y; i++) 
            { 
                result+=i; 
                Thread.Sleep(duration); 
            } 
            return result; 
        }

        #endregion 
    }  
}
```

Dikkat edileceği üzere Calculate metodunun son parametresi TimeSpan tipi ile değiştirilmiştir. Buna göre Thread.Sleep metoduna parametre olarak nekadarlık bir süre geçirileceğini belirlemek çok daha kolaydır. Örnek kullanımlarda bu amaçla TimeSpan tipinin static ve aşırı yüklenmiş yapıcı metodlarından (Overloaded Constructors) yararlanılmaktadır. Buna göre 1 saniye, 2 saniye, 50 milisaniye ve 250 milisaniye aralıklarla işlemler yaptırılmaktadır. Hatta dilerseniz aralığı gün bazında bile belirtebilirsiniz

![Laughing](/assets/images/2011/smiley-laughing.gif)

## 5 – Stopwatch Sınıfı

Hazır yukarıdaki kod parçasına değinmişken…Gerçektende belirtilen sürelere göre işlemlerin yapıldığını nasıl ölçebiliriz hiç düşündünüz mü?

![Wink](/assets/images/2011/smiley-wink.gif)

Yani 1’ den 5’ e kadar olan sayıların toplamında her bir iterasyonda 2 saniye duraksama olursa tahminlerimize göre 8 saniye kadar bir toplam süre söz konusu olmalıdır öyle değil mi? Hatta bu 8 saniye de 8000 milisaniye olarak düşünülebilir. Bunu ölçmek için Calculate metodunun içerisinde aşağıdaki düzenlemeleri yaptığımızı düşünebiliriz.

```csharp
static double Calculate(int x,int y,TimeSpan duration) 
{ 
    DateTime startTime = DateTime.Now; 
    double result=0;

    for (int i = x; i < y; i++) 
    { 
        result+=i; 
        Thread.Sleep(duration); 
    } 
    DateTime endTime = DateTime.Now; 
    TimeSpan distance = endTime - startTime; 
    Console.WriteLine("İşlem süresi {0} milisaniyedir",distance.TotalMilliseconds); 
    return result; 
}
```

İşte sonuçlar;

[![blg225_Runtime1](/assets/images/2011/blg225_Runtime1_thumb.gif)](/assets/images/2011/blg225_Runtime1.gif)

Görüldüğü gibi DateTime tipine ait iki nesne örneğinden yararlanılmış ve aradaki farka bakılarak işlemin ne kadar zaman aldığı hesap edilmiştir. Peki daha şık bir kodlama söz konusu olabilir mi? Evet olur

![Laughing](/assets/images/2011/smiley-laughing.gif)

```csharp
static double Calculate(int x,int y,TimeSpan duration) 
{ 
    Stopwatch watcher = Stopwatch.StartNew(); 
    double result=0;

    for (int i = x; i < y; i++) 
    { 
        result+=i; 
        Thread.Sleep(duration); 
    } 
    watcher.Stop(); 
    Console.WriteLine("İşlem süresi {0} milisaniyedir",watcher.ElapsedMilliseconds); 
    return result; 
}
```

Bu kez başlangıç noktasında static StartNew metodu ile bir StopWatch nesnesi örneklenmiştir. Bu nesne örneği devam eden kod satırları boyunca bir kronometre görevini üstlenmektedir. İşlemler tamamlandığında ortaya çıkan süre farkını hesap etmek için Stop metodu kullanılmaktadır. Stop metoduna yapılan çağrı sonrasında ise watcher nesne örneğinin Elapsed ön ekli özelliklerinden yararlanılarak ilgili süre farklarına ulaşılmaktadır.

Görüldüğü üzere başlangıç ve bitiş zamanlarını tespit etmek için iki farklı DateTime değişkeni tanımlamak yerine, zaten bu tip kronometre işlemleri için tasarlanmış StopWatch tipini kullanmak kodun anlamsal bütünlüğü açısından daha verimli görülmektedir. Hatta çalışma zamanı sonuçlarına baktığımızda StopWatch tipinin aslında daha tutarlı değerler döndürdüğünü de görebiliriz.

[![blg225_Runtime2](/assets/images/2011/blg225_Runtime2_thumb.gif)](/assets/images/2011/blg225_Runtime2.gif)

Anlattıklarımızdan yola çıkacak olursak eğer şu tip sonuçlara da varabiliriz;

- Kodlama yaparken şirketimizin belirlediği standartlar var ise bunlara harfiyen uymakta,
- Kodlama yaparken ilerleyen yılllarda başka bir geliştiricinin bakabileceğini düşünerek yazmakta,
- Bir önceki maddede belirttiğimiz ve ilerleyen yıllarda proje başına gelecek geliştiricinin, Dummy User olduğunu varsayarak, olayı iyi anlayabilmesi için ne kadar yorum satırı, XML Comment gerekiyorsa (tabi abartmadan) yazmakta,
- Özellikle.Net Framework içerisinde ki tipleri boş vakitlerimizde araştırıp, var olan bir projenin hangi noktasında avantaj sağlayabileceğini incelemekte

yarar vardır

![Wink](/assets/images/2011/smiley-wink.gif)

Peki başka ip uçları olabilir mi? Elbette olabilir. Bunları da ilerleyen yazılarımızda incelemeye devam edeceğiz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[OldTimes.rar (25,69 kb)](/assets/files/2011/OldTimes.rar) [Örnek Visual Studio 2010 Ultimate ile geliştirilmiş ve test edilmiştir]