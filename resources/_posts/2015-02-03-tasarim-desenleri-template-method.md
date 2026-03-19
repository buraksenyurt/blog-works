---
layout: post
title: "Tasarım Desenleri – Template Method"
date: 2015-02-03 19:00:00 +0300
description: "Düzenli olarak teknik paylaşımlarda bulunan internet yazarlarının karşılaştığı en büyük sorunlardan birisi, hızla gelişen teknoloji nedeniyle ele alınan konuların kolayca eskimesidir. Hangi firma olursa olsun bu kural geçerlidir. Bu eskitme işinde elbette başı çeken bir kaç firma var. Zaman zaman yazarların serzenişte bulunup kızdığı Microsoft, Oracle, Google ve diğerleri. Pek tabi teknik yazıların bir yaşam ömrüne sahip olması, kalıcı olan içerik sayısının daha kıymetli olmasına neden olmaktadır. Söz gelimi bir programlama dilinin temel özellikleri, dilden bağımsız düşünülebilen matematik algoritmaları gibi mevzular kolay kolay eskimezler. Hatta eskimeyen konuların başında tasarım kalıpları(Design Patterns) gelir."
categories:
  - tasarim-kaliplari-design-patterns
tags:
  - tasarim-kaliplari-design-patterns
  - csharp
  - oracle
  - xml
  - http
  - visual-studio
  - dependency-management
---
# Tasarım Desenleri – Template Method
[![eskiici](/assets/images/2015/eskiici_thumb.jpg)](/assets/images/2015/eskiici.jpg) Merhaba Arkadaşlar,

Düzenli olarak teknik paylaşımlarda bulunan internet yazarlarının karşılaştığı en büyük sorunlardan birisi, hızla gelişen teknoloji nedeniyle ele alınan konuların kolayca eskimesidir. Hangi firma olursa olsun bu kural geçerlidir. Bu eskitme işinde elbette başı çeken bir kaç firma var. Zaman zaman yazarların serzenişte bulunup kızdığı Microsoft, Oracle, Google ve diğerleri.

Dolayısıyla yazdığımız yazılar bir süre sonra eskiyen gaz lambaları misali duvarın bir köşesine asılıp yavaş yavaş çürüyorlar. Elbette istisnai durumlar da söz konusu. Nitekim pek çok firma (örneğin finans kurumları) teknolojiyi bazen geriden takip etmekte. O nedenle yazılan içeriğin hala bir yerlerde birilerinin işine yarayacağını ümit edebiliriz.

Pek tabi teknik yazıların bir yaşam ömrüne sahip olması, kalıcı olan içerik sayısının daha kıymetli olmasına neden olmaktadır. Söz gelimi bir programlama dilinin temel özellikleri, dilden bağımsız düşünülebilen matematik algoritmaları gibi mevzular kolay kolay eskimezler. Hatta eskimeyen konuların başında tasarım kalıpları (Design Patterns) gelir.

Uzun bir süre önce [tasarım kalıpları konusundaki çalışmalara](https://www.buraksenyurt.com/?tag=/design+patterns) hem yazılı hem de görsel materyaller ile başlamıştım. Ancak geçtiğimiz gün yaşadığım bir olay nedeniyle bu köşeyi eksik bıraktığımı fark ettim. İşte bu yazımızda davranışsal tasarım kalıplarından (Behavioral Design Patterns) olan Template Method desenini incelemeye çalışacağız.

Aslında deseni ele almaya karar vermem de etkili olan faktor, bir süredir incelediğim Anti-Pattern konusudur. [Wikipedia](http://en.wikipedia.org/wiki/Anti-pattern) daki kaynaktan Object Oriented Programming başlığı altındaki Sequential Coupling konulu anti-pattern maddesini incelerken, çözüm yolu olarak Template Method deseninin önerildiğini fark etmiştim.

Tanımlama

Template Method tasarım kalıbı daha çok sıralı operasyonları içeren fonksiyonellikleri ilgilendirmektedir. Öyleki bu fonksiyonellikler içeisine dahil olan operasyonların bazıları, duruma göre farklı şekillerde uygulanmak istenebilir (Diğer fonksiyonlarda aslında standart olarak hep aynı işi yaparlar) Dolayısıyla fonksiyonelliğin sahiplendiği ve çalışma biçimleri değişkenlik gösterebilecek olan operasyonların kolayca genişletilebilmesi, sahip oldukları kod parçalarının yeniden kullanılabilirliğinin arttırılması (Code Reusability) noktasında bir çözüm gerekliliği ortaya çıkmaktadır. Template Method tasarım kalıbı burada çözüm olarak kullanılabilir. Buna göre kalıbı şu şekilde tanımlayabiliriz,

> Bir algoritmanın sıralı parçalarını oluşturan operasyonlardan değişime açık olanlarının alt sınıflarda (Sub Classes) implemente edilmek suretiyle ele alınmasını öngeren bir desendir.

Sınıf Çizelgesi

Tasarım kalıbının sınıf çizelgesi aşağıdaki gibi düşünülebilir.

[![tmdp_1](/assets/images/2015/tmdp_1_thumb.png)](/assets/images/2015/tmdp_1.png)

Dikkat edilmesi gereken operasyon TemplateMethod isimli fonksiyondur. Bu fonksiyon içerisinde sırasıyla çalıştırılan başka alt fonksiyonlar bulunmaktadır. Bunlar sırasıyla OperationZ, OperationA, OperationY, OperationF ve OperationB dir. OperationA ve OperationB isimli fonksiyonlar ise abstract olarak tanımlanmışlardır ve uygulanışları Concrete sınıflar da gerçekleştirilmektedir. Bir başka deyişle OperationA ve OperationB için davranışsal bir genişletme imkanı söz konusudur.

N sayıda Concrete sınıf söz konusu olabilir. Çalışma zamanında, tercih edilen Concrete sınıf hangisi ise, TemplateMethod içerisindeki dizilim ilgili Concrete sınıfın OperationA ve OperationB metodlarını kullanacaktır. OperationA ve OperationB dışında kalan metodlar aslında sabit olan, bir başka deyişle çalışma şekillerinde her hangi bir değişiklik bulunmayacak fonksiyonellikleri temsil etmektedir.

Örnek Senaryo

Çok doğal olarak konuyu anlamanın en kolay yolu basit bir örnek ile mümkün olabilir. Bu amaçla şu senaryoyu göz önüne alabiliriz;

Bir oyun programında kullanıcı istatistiklerinin özet olarak raporlandığını düşünelim. Bu raporlama işini üstlenen bir sınıf söz konusu olsun. Sınıfın Template Method olarak düşünülecek ilgili fonksiyonelliğinin ise çalıştırdığı bir metod zinciri bulunsun. Bu zincirde yer alan fonksiyonelliklerden oyuncu bilgisinin toplanması ve ayrıştırılması değişmez parçalar olmak üzere iki ayır metod şeklinde tasarlansın. Ancak en son olarak çağırılan ve toplanan içeriğinin bastırılacağı yeri ele alan fonksiyonellik genişletilebilir olsun.

Sınıf Çizelgesi

Çok basit olarak aşağıdaki sınıf çizelgesinde yer alan örneği tasarlamamız yeterli olacaktır.

[![tmdp_2](/assets/images/2015/tmdp_2_thumb.png)](/assets/images/2015/tmdp_2.png)

GameReporter abstract sınıfı içerisinde yer alan WriteSummary metodunu Template Method olarak düşünebiliriz. Bu metod kendi içinde sırasıyla GetResults, ParseResults ve WriteResults isimli fonksiyonellikleri çağıracaktır. Dikkat edileceği üzere WriteResults bir abstract metod olarak tanımlanmıştır ve GameReporter tipinden türeyen TextReporter, ConsoleReporter ve XmlReporter sınıflarınca ezilmektedir. Bu tasarıma göre GameReporter sınıfının tüketicisine özet bilgiyi yazdırabileceği 3 farklı alternatif sunulmaktadır. Kullanıcı bilgileri isterse Text ya da XML dosyasına yazabilir veya doğrudan Console penceresine bastırabilir. Eğer yeni bir seçenek eklenmesi gerekirse (örneğin sonuçları PDF olarak bastırmak gibi) bu durumda yeni bir GameReporter türetmesi yapılması yeterli olacaktır.

> Pek tabi buradaki abstract sınıfın kendisi, uygulayıcıları ve tüketici olan program farklı katmanlarda (projelerde) duruyor olabilir.

Kod ve Çalışma Zamanı

Sınıf çizelgesinde yer alan tipleri aşağıdaki kod parçasında görüldüğü gibi yazabiliriz.

```csharp
using System;

namespace ConsoleApplication9 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            GameReporter reporter = null;

            reporter = new XmlReporter(); 
            reporter.WriteSummary(); 
            Console.WriteLine();

            reporter = new TextReporter(); 
            reporter.WriteSummary(); 
            Console.WriteLine();

            reporter = new ConsoleReporter(); 
            reporter.WriteSummary(); 
        } 
    }

    abstract class GameReporter 
    {        
        public void GetResults() 
        { 
            Console.WriteLine("Oyuncuların istatistikleri toplanıyor"); 
        }

        public void ParseResults() 
        { 
            Console.WriteLine("İstatistikler ayrıştırılıyor"); 
        }

        public abstract void WriteResults();

        public void WriteSummary() 
        { 
            GetResults(); 
            ParseResults(); 
            WriteResults(); 
        } 
    }

    class XmlReporter 
        : GameReporter 
    { 
        public override void WriteResults() 
        { 
            Console.WriteLine("İstatistikler XML dosyasına yazılıyor."); 
        } 
    }

    class TextReporter 
        : GameReporter 
    { 
        public override void WriteResults() 
        { 
            Console.WriteLine("İstatistikler TEXT dosyasına yazdırılıyor."); 
        } 
    }

    class ConsoleReporter 
        : GameReporter 
    { 
        public override void WriteResults() 
        { 
            Console.WriteLine("İstatistikler CONSOLE ekranına basılıyor."); 
        } 
    } 
}
```

Main metodu içerisinde GameReporter tipinden bir değişken tanımlandığı görülmektedir. Değişken örneklenirken ise türeyen sınıflar (Derived Class) kullanılmaktadır. Bu son derece doğaldır, nitekim abstract tipler çok biçimli olduklarından, kendisinden türeyen sınıfların nesne örneklerini taşıyabilmektedirler (Nesne yönelimli programlama temellerinden Polymorphsym’ i hatırlayalım)

Dolayısıyla reporter değişkenine bir GameReporter nesne örneği atandığında, reporter değişkeni GameReporter gibi hareket ediyor olacaktır. Benzer şekilde reporter değişkenine bir XmlReporter nesne örneği atandığında, reporter değişkeni bu kez XmlReporter gibi hareket edecektir.

O yüzden çalışma zamanında, türeyen tip içerisinde ezilmiş (override) olan WriteResults metodu devreye girecektir. Nitekim çok biçimlilik, üst tipin kendisinden türeyen alt tip içerisindeki bir fonksiyonelliği yürütebilmesine olanak tanımaktadır.

Uygulamanın çalışma zamanı çıktısı ise aşağıdaki gibidir.

[![tmdp_3](/assets/images/2015/tmdp_3_thumb.png)](/assets/images/2015/tmdp_3.png)

Uygulamasaydık

Peki bu şekilde bir yola başvurmasaydık ve söz konusu GameReporter sınıfını örneğin aşağıdaki gibi tasarlasaydık!?(Muhtemelen diyelim)

```csharp
using System;

namespace ConsoleApplication9 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            GameReporter reporter = new GameReporter(); 
            reporter.WriteSummary(Target.Console); 
            reporter.WriteSummary(Target.TextFile); 
            reporter.WriteSummary(Target.XmlFile); 
        } 
    }

    enum Target 
    { 
        Console, 
        XmlFile, 
        TextFile 
    } 

    class GameReporter 
    {        
        void GetResults() 
        { 
            Console.WriteLine("Oyuncuların istatistikleri toplanıyor"); 
        }

        void ParseResults() 
        { 
            Console.WriteLine("İstatistikler ayrıştırılıyor"); 
        }

       void WriteResults(Target target) 
        { 
           switch (target) 
            { 
                case Target.Console: 
                    Console.WriteLine("Console a yaz"); 
                    break; 
                case Target.XmlFile: 
                    Console.WriteLine("Xml dosyasına yaz"); 
                    break; 
                case Target.TextFile: 
                    Console.WriteLine("Text dosyasına yaz"); 
                    break; 
                default: 
                    break; 
            } 
        }

        public void WriteSummary(Target target) 
        { 
            GetResults(); 
            ParseResults(); 
            WriteResults(target); 
        } 
    } 
}
```

Neler yaptığımıza bir bakalım dilerseniz.

GameReporter sınıfı içerisinde yer alan WriteSummary metoduna bilgileri yazdırmak istediğimiz hedefi seçebileceğimiz bir Enum sabitini parametre olarak geçirmekteyiz. WriteResults metodu da bu Enum sabitini ele almakta ve çıktıyı seçilen hedefe doğru yönlendirmekte. Temelde bir sorun yok gibi görünüyor. Ancak yeni bir hedefin eklenmesi istendiği noktada (örneğin PDF dosyasına export etmek gibi) mecburen Enum sabitini ama daha da önemlisi WriteResults metodunun içeriğini değiştirmeliyiz. Zaten bu noktada bir bağımlılık oluştuğunu düşünebiliriz.

Ancak Template Method desenini uyguladığımız örnekte GameReporter ve diğer alt sınıfların farklı kütüphanelerde durduğunu düşünecek olursak, GameReporter tipine bir müdahale de bulunmadan ve daha da önemlisi WriteSummary içerisindeki akışı bozmadan, yeni hedefleri ilave etme şansına sahip olduğumuzu fark edebiliriz. Aşağıdaki şekildeki uygulanış biçimi iyi bir çözüm olacaktır.

[![tmdp_4](/assets/images/2015/tmdp_4_thumb.png)](/assets/images/2015/tmdp_4.png)

Buna göre PDFReporter şeklinde yeni bir seçenek dahil edilmek istendiğinde LibraryC isimli kütüphaneye ilgili sınıfın eklenmesi yeterli olacaktır. LibraryA kütüphanesinde yer alan WriteResults içerisine müdahale edilmesine gerek yoktur. Consumer sınıf ise, sadece yeni eklenen sınıfı örnekleyip kullanmakla yükümlüdür.

Ödev

Örnek bir ödev ile yazımızı sonlandıralım. Söz gelimi bir text içeriğinin analiz edilme sürecini göz önüne alalım. Dosyanın okunması, içeriğinin analiz edilerek bazı sonuçlar çıkartılması (Aggregation'lar olabilir) ve son olarak bir yerlere kayıt edilmesini süreç olarak değerlendirelim. Dosyayı okuma ve yazma işlemleri değişmez iken, analiz ve sonuç çıkartma kısımları farklılık gösterebilir. Okuma ve yazma işlemlerini birer operasyon olarak düşündüğümüzde, değişkenlik gösterebilecek fonksiyonellikler analiz ve sonuç üretme kısımlarında ortaya çıkar. Bu senaryoda Template tasarım kalıbını uygulamaya çalışınız. Eğer zorlanırsanız Visual Studio Magazine'de Eric Vogel tarafından yazılan [şu makaleden yardım alabilirsiniz](http://visualstudiomagazine.com/articles/2013/12/06/template--method-pattern-in-dot-net.aspx).

Böylece geldik bir makalemizin daha sonuna. Bu makalemizde davranışsal tasarım kalıplarından birisi olan Template Method desenini incelemeye çalıştık. Uygulanışı oldukça basit olan desenin özellikle Sequential Coupling isimli anti-pattern’ in refactor edilmesi noktasında önemli bir yere sahip olduğunu öğrendik. Bir diğer makalemizden görüşünceye dek hepinize mutlu günler dilerim.

[ConsoleApplication9.zip (73,18 kb)](/assets/files/2015/ConsoleApplication9.zip)