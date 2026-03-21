---
layout: post
title: "Temeller Kolay Unutulur (C# – Implicitly Name Hiding Sorunsalı)"
date: 2011-02-06 15:50:00 +0300
categories:
  - csharp
tags:
  - csharp
  - object-oriented-programming
  - oop
  - polymorphism
---
Sizde benim gibi basketol tutkunu musunuz? Aslında ülkemizde hemen herkesin birincil olarak futbol merakı olması beklenir. Oysaki bende diğer pek çok arkadaşım gibi birincil olarak basketbol’ a meraklıyımdır. Aslında lise yıllarında sevgili Michael Jordan ve Chicago Bulls ile başlayan bu merakım sonrasında Efes Pilsen, Ülker, Tofaş gibi takımlarla daha da artmıştır. Üniversite yıllarından beri bu takımların pek çok maçına gitmişimdir ve halen daha gitmeye çalışmaktayım (Tabi bir dönem Tofaş profesyonel basketbol şubesini kapatmıştı…) Tabi şu anda 11 aylık S (h) arp Efe buna pek müsade etmiyor. Ama yine de takım ayırt etmeksizin özellikle avrupa arenasındaki pek çok maça gitmeye çalışıyorum.

[![blg214_Giris](/assets/images/2011/blg214_Giris_thumb.jpg)](/assets/images/2011/blg214_Giris.jpg)


Basketbol denilince aklıma gelen en önemli şahsiyetler arasında ise değerli spiker Murat Murathanoğlu ve değerli yorumcu İsmet Badem ikilisi gelmektedir. Her ne kadar uzun bir süre önce yollarını ayırmış olsalar da, özellikle İsmet Badem’ in hemen her maçta gençlere verdiği basketbol ip uçları halen kullaklarımdadır. Özellikle basketbolun temellerinin (Fundamentals) çok önemli olduğunu genç basketbolcu adaylara sürekli ifade etmiştir, etmektedir.

Aslında bakarsanız temeller bir programlama dili için de son derece önemlidir. Profesyonel geliştiriciler, yazdıkları uygulamalarının çeşitliliği ve kullanılan araçlar düşünüldüğünde zaman içerisinde programlamanın temel kavramlarını kolayca unutabilir. Temellerin yer yer tekrar edilmemesi veya zaman içerisinde geriye dönüp bakılmaması bunun en büyük nedenlerindedir.

Özellikle C#, Java gibi nesne yönelimli (Object Oriented) programlama dillerinin temelleri son derece önemlidir ve bu temeller bir süre sonra profesyonel bir geliştirici için artık bisiklet sürmek gibi unutlmayacak unsurlara dönüşmelidir. İşte bu yazımızda kolayca unutulabilen bilinçsiz üye gizleme (Implicitly Name Hiding) ile alakalı bir vakayı ele almaya çalışıyor olacağız.

Dilerseniz hiç vakit kaybetmeden sorunu örnek uygulama üzerinden masaya yatırarak ilereleyelim. Bu amaçla aşağıdaki sınıf çizelgesindeki (Class Diagram) tiplerin kullanıldığı bir Console uygulamasını geliştirdiğimizi düşünelim.

[![blg214_Case1ClassDiagram](/assets/images/2011/blg214_Case1ClassDiagram_thumb.gif)](/assets/images/2011/blg214_Case1ClassDiagram.gif)

```csharp
using System;

namespace ImplicitHiding 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            #region Case 1

            // Bu vakaya göre her Dipose çağrısı için FileManager tipine ait Dispose metodu içeriğinin devreye gireceği düşünülmektedir.

            FileManager fm = new FileManager(); 
            StreamManager sm = fm; 
            IDisposable dm = fm;

            // FileManager örneğine ait Dispose metodunu çağırmak için 3 yol olduğunu düşünebiliriz. 
            // kendisi üzerinden 
            fm.Dispose(); 
            // türediği base class üzerinden 
            sm.Dispose(); 
            // türediği base class' ın implement ettiği Interface tipi üzerinden 
            dm.Dispose();

            #endregion 
        } 
    }

    abstract class StreamManager 
        :IDisposable 
    { 
        #region IDisposable Members

        public void Dispose() 
        { 
            Console.WriteLine("Stream Manager için Dispose çağrısı\n"); 
        }

        #endregion 
    }

    sealed class FileManager 
        : StreamManager 
    { 
        // Buradaki Warning mesajı geliştirici tarafından göz ardı edilmiş olabilir. Bazen sadece Build Succeeded mesajı yeterlidir. 
        public void Dispose() 
        { 
            Console.WriteLine("FileManager için Dispose çağrısı"); 
            base.Dispose(); 
        } 
    } 
}
```

Uygulamada StreamManager isimli abstract tipten olan sınıf (Class), IDisposable arayüzünü implemente etmektedir. Biraz daha ilerlemeden önce buradaki bazı temelleri de hatırlamamızda yarar olacağı kanısındayım;

- abstract tipler örneklenemezler ve çok biçimli (Polymorhic) bir yapıya sahiptirler. Diğer yandan kendilerinden türeyen tipler (Derived Types) için mutlaka uygulanması gereken üyeleri içerebilirler ki bunlarda abstract olarak tanımlanırlar.
- IDisposable bir arayüz tipidir (Interface Type) ve CLR tarafında bellek yönetimi ile ilişkili bildirim içerir. Arayüzler sınıflar gibi işlevsel bloklara sahip üyeler içeremezler. Sadece kendisini uygulayan tiplerin yapması zorunlu olan bildirimleri barındırırlar. Dispose metodu IDisposable arayüzünün belirttiği zorunlu üyelerden birisidir. Arayüzlerde çok biçimli yapıya sahiptirler.
- Çok biçimlilik (Polymorphsym) aslında türeyen tiplere ait nesne örneklerinin, türedikleri tiplere ait değişkenler tarafından taşınmaları halinde ortaya çıkan bir özelliktir. Burada üst tipte (base type) tanımlanıp, alt tipte (sub type) ezilen üyelerin büyük önemi vardır. Nitekim üst tipe atanan bir alt tip nesne örneğinin ezilen üyesi, üst tip üzerinden çağırılabilmektedir. Bir başka deyişle üst tipler yeri geldiklerinde alt tipler gibi davranış gösterebilmektedir. Çok biçimlilik adı da zaten buradan gelmektedir.
- Bir tipin çok biçimli yapıda olması özellikle Plug-In tabanlı programlama da önemlidir. Nitekim üst tipleri ve ezilebilen (ezilmesi zorunlu olan) üyeleri tanıyan bir sistemin genişletilmesinde, bu kurallara uyan tiplerin entegrasyonu söz konusudur.
- sealed olarak tanımlanmış olan FileManager tipi aslında kendisinden türetme yapılamayan kısır bir sınıftır.
- Dispose metodları çoğunlukla tipe ait nesne örneklerinin Garbage Collector (GC) tarafından toplandığı noktalarda devreye girmektedir. Dolayısıyla IDisposable ile GC’ ye bir takım ek talimatlar yollanabilir ve özellikle unmanaged tarafla ilişkili kaynak temizleme işlemleri bu metod üzerinden icra edilir.

Bu temelleri hatırladıktan sonra vakamıza dönebiliriz. FileManager sınıfı StreamManager tipinden türetilmiştir. StreamManager ise IDisposable arayüzünü uygulamaktadır. Dolayısıyla StreamManager tipinin ezmesi gereken bir Dispose metodu söz konusudur. Lakin kritik nokta FileManager tipi içerisinde de bir Dispose metodunun yazılmış olmasıdır.

Main metodu içerisindeki kod bloğuna bakıldığında FileManager tipinin örneklenip (fm isimli değişken) StreamManager tipinden bir değişkene atandığı görülmektedir. Diğer yandan IDisposable arayüzüne ait bir değişkene de aynı örnek atanmıştır. Her ne kadar StreamManager ve IDisposable, örneklenemeyen tipler olsalar da bu, değişken olarak tanımlanamayacakları anlamına gelmemelidir.

Bunlara ek olarak IDisposable ve StreamManager’ ın çok biçimli tipler oldukları da ortadadır. Dolayısıyla sm ve dm değişkenleri üzerinden çağırılan Dispose metodlarının, polimorfizim (Çok biçimlilik) nedeni ile aslında taşınmakta olan fm nesne örneğinin Dispose fonksiyonuna doğru olması düşünülebilir. Öyleyse uygulamanın çalışma zamanındaki çıktısına bir bakalım.

[![blg214_Case1Runtime](/assets/images/2011/blg214_Case1Runtime_thumb.gif)](/assets/images/2011/blg214_Case1Runtime.gif)

Dikkat edileceği üzere fm nesne örneği üzerinden yapılan çağrıda FileManager tipine ait Dispose metodu yürütülmüştür. Ancak base class ve uygulanan IDisposable arayüzleri üzerinden yapılan Dispose çağrılarında bu böyle olmamıştır. Her iki çağrıda da sub class olan FileManager tipine ait Dispose metodu yerine StreamManager tipinin Dispose metodunun icra edildiği görülmektedir. Acaba gerçekten böyle midir? Debug noktalarını koyarak ilerlediğimizde böyle olduğu ispat edilebilir.

StreamManager üzerinden Dispose çağrısı yapıldığında (sm.Dispose (); satırı) kod, StreamManager sm=fm atamasına rağmen FileManager tipinin Dispose metoduna uğramamaktadır.

IDisposable üzerinden Dispose çağrısında (dm.Dispose ();) kod IDisposable dm=fm; atamasına rağmen FileManager tipinin Dispose metoduna uğramamış ve yine StreamManager tipinin Dispose metodu çağırılmıştır ki sanırım en ilginç olanı da budur.

Peki bu durumların oluşmasının sebebi nedir? Aslında sorun bilinçsiz olarak yapılan üye gizleme (Implicitly Name Hiding) operasyonundan kaynaklanmaktadır. Nitekim şu anda FileManager içerisinde, üst tipteki ile aynı isimde olan bir metod tanımı söz konusudur ve çalışma zamanı bu kullanımı gördüğünde varsayılan olarak üst tipe ait üyeleri çağırmaktadır. Bir başka deyişle üst tip üyesi alt tipi gizlemektedir. Gerçi bilinçsiz bir şekilde üye gizleme yapıldığı pek doğru değildir. Nitekim Visual Studio IDE’ si geliştiriciyi bu noktada aşağıdaki gibi uyarmaktadır.

[![blg214_IDE](/assets/images/2011/blg214_IDE_thumb.gif)](/assets/images/2011/blg214_IDE.gif)

Ancak dikkatsiz bir geliştirici çok kalabalık bir projede bu tip bir warning mesajını kolayca gözden kaçırabilir. Aslında çoğu zaman geliştirici için projenin başarılı bir şekilde derlenmesi yeterli olmaktadır. Tabi ReSharper gibi araçları kullanmıyorsak ya da TFS (Team Foundation Server) altında geliştirme yapıp Policy’ ler uygulayarak Warning’ ler aşılmadan kodun derlenmesini engellemiyorsak bu tip gözden kaçırmaların sayısı artacaktır.

Peki bu temeli bilen bir geliştirici sorun haline gelen vakaya düşmemek için ne yapmalıdır?

İlk akla gelen yöntem üst sınıf (base class) içerisinde tanımlı olan ve IDisposable arayüzü üzerinden gelen Dispose metodunun virtual olarak tanımlanmasıdır.

```csharp
abstract class StreamManager 
        :IDisposable 
{ 
	#region IDisposable Members

	public virtual void Dispose() 
	{ 
		Console.WriteLine("Stream Manager için Dispose çağrısı\n"); 
	}

	#endregion 
}
```

Ancak virtual olarak yapılan tanımlama da yeterli değildir. Halen daha base class'ta yer alan aynı isimli Dispose metodunun alt tiptekini gizlemesi durumu devam etmektedir. Dolayısıyla çalışma zamanındaki durum değişmeyecek ve aşağıdaki çıktı alınmaya devam edecektir.

[![blg214_VirtualRuntime](/assets/images/2011/blg214_VirtualRuntime_thumb.gif)](/assets/images/2011/blg214_VirtualRuntime.gif)

Abstract bir üye olarak Dispose metodunun tanımlanması da düşünülebilir ancak üzerinde çalıştığımız senaryo da geçerli bir kullanım değildir.

[![Exclamation](/assets/images/2011/Exclamation_thumb_4.gif)](/assets/images/2011/Exclamation_4.gif) Bilindiği üzere abstract üyeler herhangibir şekilde kod bloğu içermezler ve türeyen tipler içerisinde mutlaka ezilmek (override) zorundadırlar.

Aslında üst sınıfın üye gizleme işlemini yapması istenmiyorsa yapılması gereken yol alt tip içerisindeki ezme (override) işleminin açık bir şekilde gerçekleştirilmesidir. Söz gelimi virtual kullanımı söz konusu ise kodun aşağıdaki şekilde düzenlenmesi yeterli olacaktır.

```csharp
abstract class StreamManager 
	:IDisposable 
{ 
	#region IDisposable Members

	public virtual void Dispose() 
	{ 
		Console.WriteLine("Stream Manager için Dispose çağrısı\n"); 
	}

	#endregion 
}

sealed class FileManager 
	: StreamManager 
{ 
	public override void Dispose() 
	{ 
		Console.WriteLine("FileManager için Dispose çağrısı"); 
		base.Dispose(); 
	} 
}
```

Dikkat edileceği üzere FileManager tipi içerisinde yer alan Dispose metodu override bildirimi ile tanımlanmıştır. Yani açık bir şekilde üst tipten gelen Dispose metodunun ezilmesi ve yerine bu fonksiyon gövdesinin çağırılması gerektiği belirtilmiştir. Bu durumda çalışma zamanı çıktısı tam da istediğimiz gibi olacaktır.

[![blg214_OverrideRuntime](/assets/images/2011/blg214_OverrideRuntime_thumb.gif)](/assets/images/2011/blg214_OverrideRuntime.gif)

Tabi şu durumda unutulmamalıdır. Gerçekten bilinçli bir şekilde üye gizleme (Name Hiding) işleminin yapılması gerekiyorsa, new operatörünün kullanılması gerekmektedir. Aşağıdaki kod parçasında olduğu gibi.

```csharp
sealed class FileManager 
        : StreamManager 
{ 
	public new void Dispose() 
	{ 
		Console.WriteLine("FileManager için Dispose çağrısı"); 
		base.Dispose(); 
	} 
}
```

Elbette bu durumda üst tipte yer alan Dispose metodunun virtual olmasına da gerek yoktur. new operatörünün kullanılması, bir anlamda geliştiricinin de ne yaptığının farkında olması demektir. Yazımızda değerlendirdiğimiz bu örnek vakanın aslında kulağımıza bir cümleyi küpe yaptığı da gerçektir: “Siz siz olun Warning mesajlarını dikkate alın” Böylece geldik bir yazımızın daha sonuna. Tekrardan örüşünceye dek hepinize mutlu günler dilerim.

[ImplicitHiding.rar (22,03 kb)](/assets/files/2011/ImplicitHiding.rar)