---
layout: post
title: "C# 3.0: Derinlemesine Extension Method Kavramı"
date: 2008-03-21 04:00:00 +0300
categories:
  - csharp-3-0
tags:
  - csharp-3-0
  - csharp
  - dotnet
  - linq
  - generics
  - visual-studio
  - datatable
---
Bilindiği üzere Language INtegrated Query (LINQ) mimarisinin uygulanışında C# 3.0 (Visual Basic 9.0) ile birlikte gelen yenilikler oldukça önemli bir yere sahiptir. Bu yeniliklerin çoğu var olan.Net Framework 2.0 yapısını bozmadan genişletebilmek amacıyla tasarlanmıştır. Genişletme Metodları (Extension Methods) bu yeniliklerden sadece bir tanesidir.(Object Initializers, Anonymous Types, Partial Methods, var anahtar kelimesi, auto-implemented property, => operatörü diğer C# 3.0 yenilikleri arasında sayılabilir) Söz konusu yeniliğin çıkış amacı genişletilemeyen tiplere yeni fonksiyonelliklerin eklenebilmesinin sağlanmasıdır. Öyleki bu sayede koleksiyonlar (Collections), DataTable, dizi (Array) gibi var olan CLR tipleri (Common Lanugage Runtime) üzerinde LINQ tarzı sorgu ifadelerinin yazılabilmesi olanaklı hale gelmiştir.

> Örneğin IEnumerable arayüzüne (Interface) uygulanan genişletme metodları (Extension Methods) sayesinde T türünden koleksiyonlar üzerinde Sum, Count, Select, Average, OrderBy,Distinct gibi fonksiyonellikler uygulanabilmektedir. Bunun için System.Linq isim alanı (Namespace) altında Enumerable isimli static bir sınıf geliştirilmiş ve içerisine aşağıdaki sınıf diagramda (Class Diagram) bir kısmı görünen pek çok genişletme metodu ilave edilmiştir.
> ![mk246_2.gif](/assets/images/2008/mk246_2.gif)

Bilindiği üzere SQL sorgularına benzeyen LINQ ifadeleri aslında arka planda metodlar yardımıyla işaret edilebilirler. Nitekim programatik ortamlar bu tarz bir yaklaşımı gerektirmektedir. Üstelik bu işlemler yapılırken var olan tiplerin içeriklerine müdahale edilmemekte, sadece ek fonksiyonellikler katılmaktadır. Bu nedenle genişletme metodları LINQ ifadelerinin kullanılabilmesinde önemli bir role sahiptir. Bu makalemizde genişletme metodlarını derinlemesine incelemeye çalışacak ve ayrıntılara bakıyor olacağız.

> Genişletme metodları var olan tiplere ek fonksiyonellikler kazandırılmasını sağlarken bunların orjinal yapısını asla bozmazlar. Tanımlandıkları programda, uygulandıkları tipin bir parçası olarak yaşar ama o tipin orjinalliğine etki etmeden ek işlevselliklerin kullanılabilmesini olanaklı kılarlar.

Herşeyden önce nesne tabanlı programlama dillerinde (Object Oriented Programming Language), kalıtım (Inheritance) sayesinde var olan tiplerin (Type) genişletilmesi mümkündür. Ancak türetilmesine izin verilmeyen tiplerde mevcuttur. Söz gelimi sealed anahtar kelimesi ile imzalanmış olan tipler türetme tekniği yardımıyla genişletilemez. Üstelik.Net içerisinde bu şekilde tanımlanmış olan sayısız sınıf vardır. Örneğin String sınıfı sealed olarak imzalanmış bir sınıf olduğundan kendisinden türetme yapılmasına izin verilmemektedir. (Üstelik String parçalı bir sınıf (Partial Class)' da değildir.)

![mk246_1.gif](/assets/images/2008/mk246_1.gif)

Bu nedenle bu sınıfa ek fonksiyonellikler ilave edilmesi mümkün değildir. Oysaki var olan.Net tiplerinin (yada kendi geliştirdiğimiz ama türetme yapılmasına izin verilmeyen tiplerin) yapısını bozmadan yeni fonksiyonelliklerin katılarak uygulamalar içerisinde ela alınması istendiği vakalar söz konusudur. Ki nesnelerin SQL tarzında sorgulanabilmeside buna bir örnek olarak verilebilir. Bu sebepten genişletme metodlarının önemi oldukça fazladır.

> Genişletme metodları (Extension Methods), değer (value) ve referans (reference) türleri ile arayüzlere (Interface) uygulanabilir. Değer türü olarak yapılar (struct) göz önüne alınabilir. Nitekim yapılar açıkça belirtilmesede kendilerinden türetilme yapılmasına izin vermemektedir.

Genişletme metodları ele alınırken gözden kaçırılmaması gereken bir nokta daha vardır. Genişletme metodları nesne yönelimli programlama jargonundaki kurallardan birisi değildir. Sadece.Net Framework mimarisine özgü bir kavramdır. Genişletme metodları bu anlamda bir tipin paylaşımlı fonksiyonları olarakda düşünülebilir. Hatta bu metodlar.Net Framework 3.5 öncesi sürümlerdeki tipler içinde uygulanabilirdir. Tabi öncelikli olarak genişletme metodlarının (Extension Methods) C# 3.0 içerisinde nasıl yazıldığına bakmata yarar vardır. Genişletme metodlarının yazılmasında üç basit kural vardır. Bu metodlar static bir sınıf içerisinde static olarak tanımlanmalı ve uygulanacakları tipi ilk parametrelerinde this anahtar kelimesi ile birlikte almalıdır.(Ki bunların bir takım sebepleri vardır) Örnek olarak ayrı bir sınıf kütüphanesi içerisinde geliştirilmiş olan aşağıdaki sınıfı göz önüne alabiliriz.

![mk246_3.gif](/assets/images/2008/mk246_3.gif)

Merkez isimli static sınıfın içeriği aşağıdaki gibidir.

```csharp
using System;
using System.Drawing;

namespace Genisletmeler
{
    /// <summary>
    /// Genişletme fonksiyonelliklerini içeriri
    /// </summary>
    public static class Merkez
    {
        /// <summary>
        /// Bir string içerisindeki tüm karakterlerin Ascii değerlerini ele alıp byte dizisi şeklinde geriye döndürür. Eğer string null veya empty ise exception döndürür.
        /// </summary>
        /// <param name="s">Byte değerleri döndürülecek string parametre</param>
        /// <returns>Ascii değerleri</returns>
        public static byte[] GetAscii(this string s)
        {
            if (String.IsNullOrEmpty(s))
                throw new Exception("String veri olmalıdır.");
            byte[] result = new byte[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                result[i] = (byte)s[i];
            }
            return result;
        }

        /// <summary>
        /// Int32 tipinden bir sayının faktöryelinin bulunmasını sağlar
        /// </summary>
        /// <param name="sayi">Faktöryel değeri hesap edilecek değişken</param>
        /// <returns>Sayının faktöryeli</returns>
        public static double Faktoryel(this Int32 sayi)
        {
            if (sayi == 0
                || sayi == 1)
                return 1;
            else
                return  sayi*Faktoryel(sayi-1);
        }

        /// <summary>
        /// İki Point arasındaki uzaklığın pisagor teoremine göre hesap edilmesini sağlar.
        /// </summary>
        /// <param name="nokta1">Birinci nokta</param>
        /// <param name="nokta2">İkinci nokta</param>
        /// <returns>Mesafe</returns>
        public static double Uzaklik(this Point nokta1,Point nokta2)
        {
            int xFarki = nokta1.X - nokta2.X;
            int yFarki = nokta2.X - nokta2.Y;
            return Math.Sqrt((xFarki * xFarki) + (yFarki * yFarki));
        } 
    }
}
```

Merkez isimli static sınıf içerisinde 3 farklı metod yer almaktadır. Bu metodlardan GetAscii String sınıfına, Faktoryel Int32, Uzaklik ise Point yapılarına (Struct) uygulanmaktadır. GetAscii metodu yardımıyla string bir değişkenin karakterlerinin byte tipinden bir dizi olarak elde edilmesi sağlanmaktadır. Faktroyel metodu Int32 tipinden değişkenlere uygulanabilmekte olup, basit olarak sayının faktöryelini hesaplamaktadır. (Üstelik yinelemeli-Recursive bir metod olarak tasarlanmıştır. Buna göre genişletme metodlarının recursive formasyonda kullanılabileceği söylenebilir.)

Uzaklik isimli metod ise diğerlerinden farklı olarak birde ek parametre almaktadır. Buna göre Point tipinden bir değişkenin başka bir Point ile arasındaki uzaklığın bulunabilmesi sağlanmaktadır. (Yani kabaca iki nokta arasındaki mesafenin pisagor teoremi çerçevesinde hesaplanması gerçekleştirilmeltedir.) Bir başka deyişle genişletme metodları (Extension Methods) uygulanacakları tipi belirten ilk parametreden sonra ek parametrelerde alabilmektedir. Söz konusu sınıfın özellikle CIL (Common Intermediate Language) tarafına nasıl aktarıldığını incelemeden önce kullanımına bakılabilir. Bu amaçla Merkez isimli static sınıfı içeren kütüphaneyi (Class Library) referans eden basit bir Console uygulaması geliştirilip aşağıdaki kodlar test amacıyla kullanılabilir.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genisletmeler;
using System.Drawing;

namespace DerinlemesinExtensionMethods
{
    class Program
    {
        static void Main(string[] args)
        {
            // String sınıfı sealed olarak imzalanmıştır bu nedenle kendisinden türetme yapılıp ek fonksiyonellikler katılamaz.
            string ad = "Burak Selim";
            byte[] asciiDegerleri=ad.GetAscii();
            foreach(byte b in asciiDegerleri)
                Console.Write(b.ToString()+" ");
    
            // Int32 bir struct' tır. Struct' lar açıkça belirtilmesede sealed' dır. Yani kendilerinden türetme yapılamaz. Ancak genişletme metodları yardımıyla bunlar ek fonksiyonellikler     katılabilir.
            int sayi = 3;
            Console.WriteLine(sayi.Faktoryel());

            // Point .Net Framework içerisinde System.Drawing isim alanında tanımlanmış olan struct tipidir. Kendisinden türetme yapılamaz. Ancak extension method sayesinde Uzaklik isimli bir metoda sahip olabilir
            Point pn = new Point(10, 20);
            Console.WriteLine("İki Nokta Arası Uzaklık {0}",pn.Uzaklik(new Point(20, 30)).ToString());
        }
    }
}
```

Uygulama test edildiğinde aşağıdakine benzer sonuçlar ile karşılaşılır.

![mk246_4.gif](/assets/images/2008/mk246_4.gif)

Görüldüğü gibi string, int ve Point tipinden değişkenler üzerinden yeni fonksiyonellikler kullanılabilmektedir. Peki bu sistem alt tarafta nasıl yürümektedir? Sonuç itibariyle var olan bir CLR tipinin (Common Language Type) bozulmadan genişletilebilmesinin ancak çalışma zamanı motoru (Runtime Engine) tarafından önemli olduğu ortadadır. Hatta dikkat edileceği üzere Visual Studio geliştirme ortamı, eklenen genişletme metodlarını intellisense özelliğinde aynen aşağıdaki ekran görüntüsünde olduğu gibi gösterebilmektedir.

![mk246_5.gif](/assets/images/2008/mk246_5.gif)

Çalışma zamanında bazı bilgilere bakılması söz konusu ise eğer, niteliklerden (Attribute) yararlanılmaktadır. Bilindiği üzere nitelikler yardımıyla çalışma zamanına ekstra metadata bilgileri aktarılabilmektedir. Bu sebepten genişletme metodlarında da başrol oyuncusu olarak System.Core.dll assembly'ında System.Runtime.CompilerServices isim alanı (Namespace) altında bulunan Extension isimli bir nitelik (Attribute) görev almaktadır. Bu her ne kadar kod tarafında sadece Visual Basic 9.0 ile görülsede, IL tarafında rahat bir şekilde tespit edilebilmektedir.

![mk246_6.gif](/assets/images/2008/mk246_6.gif)

Yukarıdaki IL görüntüsündende dikkat edileceği üzere GetAscii isimli metod ExtensionAttribute niteliği ile imzalanmıştır. Bu son derece anlamlıdır nitekim hem compiler hemde çalışma zamanı (Runtime) için, takip eden metodun, ilk parametre ile belirtilen tip için bir genişletme olduğu belirtilmektedir. Bir başka deyişle söz konusu nitelik derleyiciye veya çalışma zamanına, ilk parametredeki tip için bazı ek bilgiler gönderir ve yeni fonksiyonelliği kazanmasını sağlar. Buraya kadar genişletme metodlarının ne olduğundan ve nasıl uygulandığından bahsetmeye çalıştık. Genişletme metodları ile ilişkili dikkat edilmesi gereken bazı noktalar da vardır. Dilerseniz yazımızın ilerleyen kısımlarında bu konulara değinelim.

1 - Genişletme metodları aşırı yüklenebilirler (Overloading)

Metodlar aynı isim altında birden fazla kez yazılabilirler (Buna şu an için verilebilecek en güzel örneklerden birisi WriteLine metodudur. Dikkat edileceği üzere bu metodun 19 farklı versiyonu bulunmaktadır.) Bu kısaca metodun aşırı yüklenmesi (Method Overloading) olarak adlandırılmaktadır. Metodun aşırı yüklenmesi sırasındaki önemli kriter, parametre tipleri ve sayılarının belirlediği imzalardır (Method Signature). Çok doğal olarak genişletme metodlarıda aşırı yüklenebilirler. Söz gelimi Merkez sınıfı içerisinde iki nokta arasındaki uzaklığı bulmak için tasarlanmış olan Uzaklik metodunun farklı bir versiyonu aşağıdaki gibi yazılabilir.

```csharp
/// <summary>
/// İki Point arasındaki uzaklığın pisagor teoremine göre hesap edilmesini sağlar
/// </summary>
/// <param name="nokta1">Metodun uygulanacağı Point tipinden değişken</param>
/// <param name="x2">X2 Değeri</param>
/// <param name="y2">Y2 Değeri</param>
/// <returns>Mesafe</returns>
public static double Uzaklik(this Point nokta1, int x2, int y2)
{
    int xFarki = nokta1.X - x2;
    int yFarki = nokta1.Y - y2;
    return Math.Sqrt((xFarki * xFarki) + (yFarki * yFarki));
}
```

Bu versiyon diğerinden farklı olarak Point tipinden ikinci bir parametre almak yerine int tipinden iki ayrı parametre kullanmaktadır. Burada metodların hem parametre sayıları hemde tipleri farklılaşmayı sağlamaktadır. Merkez sınıfı bu haliyle örnek uygulamada kullanıldığında aşağıdaki ekran görüntüsünden de izlenebileceği gibi iki farklı Uzaklik metodunun çağırılabileceği görülür.

![mk246_7.gif](/assets/images/2008/mk246_7.gif)

Burada akla hemen şu soru gelebilir. Merkez sınıfının haricinde başka bir static sınıf içerisinde, Point tipi için Uzaklik metodunun aşırı yüklenmiş başka bir versiyonu yazılabilir mi? Eğer yazılırsa söz konusu uygulamada bu versiyon kullanılabilir mi? Bu sorulara cevap verebilmek için Console uygulaması içerisinde aşağıdaki gibi bir static sınıf tanımlaması yapıldığını varsayalım.

```csharp
static class Genisletme
{
    public static double Uzaklik(this Point nokta1, double x2, double y2)
    {
        double xFarki = nokta1.X - x2;
        double yFarki = nokta1.Y - y2;
        return Math.Sqrt((xFarki * xFarki) + (yFarki * yFarki));
    }
}
```

Merkez sınıfı Genisletmeler isim alanı altında ve üstelik farklı assembly içerisinde yer almaktadır. Genisletme isimli sınıf ise DerinlemesineExtensionMethods isimli Console uygulaması içerisinde tanımlanmıştır. Bu durumda Main metodu içerisinde Point tipinden bir değişken kullanılmak istendiğinde Uzaklik isimli fonksiyonun 3 farklı versiyonuna ulaşılabildiği görülecektir.

![mk246_8.gif](/assets/images/2008/mk246_8.gif)

Bir başka deyişle farklı static sınıflar içerisindede olsalar genişletme metodları aşırı yüklenebilirler.

2 - CLR Tipi (Common Language Runtime Type) içerisinde tanımlı olan bir fonksiyonun aynısı extension method olarak yazılıp, örnek (Instance) tipe ait metod ezilebilir (Override) mi?

Bu bir anlamda orjinal CLR tiplerinin güvenliği ile ilişkilide bir konudur. Nitekim türetilmesine izin verilmeyen tiplerin asıl tasarım amaçlarından biriside içeriklerinin değiştirilmesinin engellenmesidir. Bu anlamda sealed olarak işaretlenmiş tiplerin aslında genişletme metodları yardımıyla ek fonksiyonelliklere sahip olabilmesi ve hatta aşırı yükleme yapılabilmesi orjinal tipte tanımlı metodların ezilip ezilemeyeceği vakasını ortaya çıkarmaktadır. Bu durumu analiz etmek için basit olarak String tipinde tanımlı olan bir metodun aynısını extension method olacak şekilde tanımlamaya çalışabiliriz.

```csharp
public static string Insert(this string s,int siraNo, string metin)
{ 
    Console.WriteLine("Extension Method");
    return metin;
}
```

Burada String sınıfının Insert metodunun aynısı extension metod olarak yazılmaya çalışılmaktadır. Uygulama derlendiğinde herhangibir hata mesajı alınmaz. Ancak string bir değişken üzerinden Insert metodu çağırıldığında aşağıdaki ekran görüntüsünde olduğu gibi orjinal versiyonun kullanılabileceği görülür.

![mk246_9.gif](/assets/images/2008/mk246_9.gif)

Buna göre derleyici açısından nesne örneği (Object Instance) metodunun daha öncelikli olduğu ortadadır. Bir başka deyişle genişletme metodları yardımıyla orjinal nesne örneğine ait metodlar ezilemezler.

3 - Bir tip içerisinde tanımlı özellik yada alan ile aynı isimde bir extension metod tanımlanırsa.

Bu durumu analiz edebilmek için aşağıdaki kod parçası göz önüne alınabilir.

```csharp
sealed class Materyal
{
    public int Katsayi;
}
static class Genisletme
{
    public static void Katsayi(this Materyal mtr)
    {
        Console.WriteLine("Genişletme metodu");
    }
}
```

Burada tanımlanan Materyal isim sınıf sealed olarak imzalanmıştır ve içerisinde int tipinden Katsayi isimli bir alan (Field) içermektedir. Bu tip bir sınıfın başka bir nesne kullanıcısı (Object User) tarafından genişletilmek istendiği bir durumda, bilinçsiz olarak aynı isme sahip genişletme metodları eklenebilir. Bunu sembolize eden Genisletme sınıfı kendi içerisinde, Materyal sınıfındaki alanla aynı adda olan Katsayi isimli bir metod içermektedir. Ne varki kod tarafında Materyal sınıfına ait bir örnek oluşturulduğunda, Katsayi genişletme metoduna erişilemediği açık bir şekilde görülmektedir.

![mk246_13.gif](/assets/images/2008/mk246_13.gif)

Dikkat edilecek olursa sadece Katsayi isimli nesne alanı (Field) görünmektedir. Fakat burada oldukça enteresan bir durumda söz konusudur. Eğer kodda ısrar edilir ve Katsayi genişletme metodu kullanılmak istenirse derleme zamanı hatası alınmadığı görülür. Hatta kod yürütüldüğünde, genişletme metodunun çalıştığı görülecektir. Bu durum aslında genişletme metodlarının isimlendirilmesinin önemli olduğunu göstermektedir.

4 - Extension metodlar dilden bağımsızdır.

Genişletme metodları daha öncedende bahsedildiği gibi CIL (Common Intermediate Language) tarafında Extension niteliği ile imzalanırlar. Bu sebepten dolayıda.Net destekli diller tarafından kullanılabilirler. Söz gelimi C# kodlaması ile geliştirilmiş genişletme metodları, Visual Basic ile yazılmakta olan bir proje içerisinde kullanılabilir. Elbette tam tersi durumda geçerlidir. Konuyu daha kolay analiz etmek için Merkez isimli sınıfı içeren C# tabanlı kütüphaneyi basit bir Visual Basic Console uygulamasında aşağıdaki gibi deneyebiliriz.

```text
Imports Genisletmeler
Imports System.Drawing

Module Module1

    Sub Main()

        Dim str As String = "Burak Selim Şenyurt"
        Dim dizi As Byte() = str.GetAscii()
        For i As Int32 = 0 To dizi.Length - 1
            Console.Write(dizi(i).ToString() + " ")
        Next

        Console.WriteLine()
    
        Dim sayi As Integer = 4
        Console.WriteLine(sayi.Faktoryel().ToString())
    
        Dim nokta1 As New Point(3, 4)
        Console.WriteLine(nokta1.Uzaklik(6, 8).ToString())

    End Sub

End Module
```

Visual Basic tarafında kod yazıyor olsakta, Visual Studio arabiriminin intellisense özelliği C# ile yazılmış genişletme metodlarını gösterecektir. Sonuç itibariyle burada yapılan farklı bir assembly içerisinde tip ve üyelerine erişmektir.

![mk246_11.gif](/assets/images/2008/mk246_11.gif)

Elbetteki kodun çalışabilmesi için C# kütüpanesinin Visual Basic tabanlı projeye referans edilmesi gerekmektedir.

![mk246_10.gif](/assets/images/2008/mk246_10.gif)

Bu işlemin ardından uygulama çalıştırılırsa genişletme metodlarının başarılı bir şekilde çalıştığı görülür.

![mk246_12.gif](/assets/images/2008/mk246_12.gif)

5 - Object tipinin genişletilmesi.

Object tipide genişletme metodlarına sahip olabilir..Net Framework içerisinde yer alan tipler object türevli olduklarından çok doğal olarak tanımlanan genişletme metodlarını kullanabilirler. Bu durumu test edebilmek için sembolik olarak aşağıdaki genişletme metodunu eklediğimizi düşünelim.

```csharp
public static string GetTypeName(this object obj)
{
    return obj.GetType().Name;
}
```

Metod basitçe herhangibir nesnenin tip adını döndürmektedir. Metodun uygulanışına bakıldığında ise herhangibir tipteki değişkenden sonra çağırılabildiği görülecektir.

![mk246_14.gif](/assets/images/2008/mk246_14.gif)

Aşağıda, örnek bir kod parçası kullanımı ve çalışma zamanı çıktısı yer almaktadır.

```csharp
int puan = 51;
Console.WriteLine(puan.GetTypeName());

Point nokta3 = new Point(3, 4);
Console.WriteLine(nokta3.GetTypeName());

string firmaAdi = "FreeLancer";
Console.WriteLine(firmaAdi.GetTypeName());
```

![mk246_15.gif](/assets/images/2008/mk246_15.gif)

> Visual Basic 9.0' da özellikle Object tipinden bir değişkene atama yapıldığında, genişletme metodlarını çağırmak çalışma zamanı istisnasına (Run Time Exception) neden olmaktadır.
> Dim obj As Object = 3.14F
> Console.WriteLine (obj.GetTypeName ())
> Bu kullanım çalışma zamanında MissingMemberException istisnasının (Exception) fırlatılmasına neden olmaktadır. Sorun Object tipinin Late-Bound olmasından kaynaklanmaktadır. Bunun çözmek için type inference kavramından (C# karşılığı var anahtar kelimesi) yararlanılabilir. (Dim obj=3.14F)
> Ne varki bu durum C# tarafında geçerli değildir. Bu nedenle C# tarafında aşağıdaki kod parçası sorunsuz olarak çalışmaktadır.
> Object obj = 3.14f;
> Console.WriteLine (obj.GetTypeName ());

Object tipi için genişletme metodları var anahtar kelimesi ile birliktede kullanılabilirler. Aşağıdaki kod parçası bu durumu göstermektedir. Bu kod içerisinde var anahtar kelimesi ile tanımlanan nesne isimli değişken eşitliğin sol tarafı göz önüne alındığında float (Single yapısı-struct) tipindendir. Bu sebepten nesne üzerinden çağırılan GetTypeName isimli genişletme metodu geriye Single değerini döndürecektir.

```csharp
var nesne = 3.14f;
Console.WriteLine(nesne.GetTypeName());
```

6 -.Net Framework 2.0 hedefli bir uygulama içerisinde extension metodlar kullanılabilir mi?

Extension niteliği System.Core.dll assembly'içerisinde tanımlanmıştır ve System.Runtime.CompilerServices isim alanında bulunmaktadır. System.Core.dll'i.Net Framework 3.5 ile gelmekte olsada.Net Framework 2.0 motorunu kullanarak çalışmaktadır. Bu noktada.Net 2.0 ile geliştirilmiş bir uygulamada extension metod kullanımı söz konusu olabilir mi? Akla ilk gelen yöntem System.Core.dll assembly'ının ilgili projeye referans edilmesidir. Ancak Visual Studio 2008 içerisinde bu denendiğinde.Net 2.0 tabanlı projeye söz konusu referansların eklenemediği görülecektir.

![mk246_16.gif](/assets/images/2008/mk246_16.gif)

Browse seçeneği ile ekleme yapılmaya çalışılsada durum değişmeyecektir. Ancak izlenecek basit bir yol ile.Net 2.0 tabanlı projede extension metod kullanımı sağlanabilir. Bunun için uyulamada System.Runtime.CompilerServices isimli bir namespace tanımlanır ve içerisine Extension isimli bir attribute sınıfı eklenir. Bu işlemin ardından extension metod yazılabildiği hatta kullanılabildiği görülecektir. Durumu daha iyi analiz etmek amacıyla.Net 2.0 tabanlı bir Console uygulamasına ait aşağıdaki kod parçası göz önüne alınabilir.

```csharp
using System;
using System.Runtime.CompilerServices;

// 1nci : İlk olarak System.Runtime.CompilerServices adlı isim alanı içerisinde ExtensionAttribute isimli bir nitelik tanımlanır
namespace System.Runtime.CompilerServices
{
    // 2nci: Nitelik assembly, sınıf ve metod seviyesinde uygulanabilir. Bir kere kullanılabilir.
    [AttributeUsage(AttributeTargets.Assembly| AttributeTargets.Class| AttributeTargets.Method,AllowMultiple=false,Inherited=false)]
    public class ExtensionAttribute :
        Attribute
    {
    }
}
namespace DerinlemesineExtensionMethods2
{
    static class ExtensionMethods
    { 
        // Eğer ExtensionAttribute tanımlanmazsa this keyword kullanımı için derleme zamanı hatası alınacaktır.
        public static string GetTypeName(this object obj)
        {
            return obj.GetType().Name;
        }    
        public static double Faktoryel(this Int32 sayi)
        {
            if (sayi == 0
                || sayi == 1)
                return 1;
            else
                return sayi * Faktoryel(sayi - 1);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            int puan = 12;
            Console.WriteLine(puan.GetTypeName()); // Extension metod kullanımı
        
            int sayi = 4;
            Console.WriteLine(sayi.Faktoryel().ToString());
        }
    }
}
```

Uygulama çalıştırıldığında genişletme metodlarının işe yaradığı görülebilir.

> Tabi bu vaka Visual Studio 2008 üzerinde.Net 2.0 tabanlı bir proje şablonu için gerçeklenmektedir. Nitekim derleme aşamasında sadece C# 3.0 derleyicisi genişletme metodunu değerlendirebilmektedir. Bir başka deyişle Visual Studio 2005 ortamında aynı örnek çalıştırılamayacaktır.

7 - Arayüzlere genişletme metodları eklenebilir.

LINQ (LanguageINtegratedQuery) mimarisinin temelinde yatan genişletme metodlarının çoğu arayüzlere (Interface) uygulanmaktadır. Böylece, genişletme metodlarının uygulandığı arayüz tiplerinden türeyen türlerin tamamı, söz konusu ek fonksiyonellikleri kullanabilir duruma gelmektedir. Bu gerçektende önemli bir yetenektir. Çok doğal olarak geliştirici tarafından yazılmış olan yada Framework içerisinde yer alan arayüz tiplerine genişletme metodları eklenebilir. Aşağıdaki örnek kod parçasında bu duruma örnek olacak bir metod içeriği yer almaktadır.

```csharp
public static IEnumerable<string> HaricindeKalanlar(this IEnumerable<string> koleksiyon,string aranan) 
{
    foreach (string s in koleksiyon)
    {
        if (s != aranan)
            yield return s;
    }
}
```

HaricindeKalanlar isimli genişletme metodu, IEnumerable tipinden türeyen generic koleksiyonlara uygulanabilmektedir. Görevi parametre olarak verilen string değer dışında kalan elemanları tespit ederek yeni bir IEnumerable tipi içerisinde geriye döndürmektedir.(İşlerin kolaylaştırılmasında.Net 2.0 ile birlikte gelen yield anahtar kelimesinin önemli bir rolü vardır.) Buna göre IEnumerable arayüzünden türeyen her tip, HaricindeKalanlar isimli genişletme metodunu kullanabilmektedir. Söz gelimi aşağıdaki kod parçasında List, Stack, Queue tiplerine uygulanmaktadır.

```csharp
List<string> isimler = new List<string> { "Burak", "Ahmet", "Mehmet",  "Mehmet", "Ahmet", "Özgür", "Emrah", "Bülent" };
Stack<string> isimler2 = new Stack<string>(isimler);
Queue<string> isimler3 = new Queue<string>(isimler);

var sonuc1=isimler.HaricindeKalanlar("Ahmet");
var sonuc2 = isimler3.HaricindeKalanlar("Ahmet");
var sonuc3 = isimler.HaricindeKalanlar("Mehmet");
```

Çalışma zamanında örneğin sonuc3 değişkeninin içeriği aşağıdaki ekran görüntüsündeki gibi olacaktır. Dikkat edileceği üzere Mehmet ismi dışında kalanlar elde edilmektedir.

![mk246_17.gif](/assets/images/2008/mk246_17.gif)

Buraya kadar bahsedilenler kısaca değerlendirilirse, genişletme metodlarının aşağıdaki avantajları sağladığından bahsedilebilir.

- Var olan tiplere (Types) yeni fonksiyonellikerin eklenebilmesi sağlanır. Öyleki yazılmış olan uygulamaların çalışma sistemini bozmadan yeni fonksiyonellikler katarak genişlemelerine yardımcı olur.
- Tiplere yeni fonksiyonellikler eklenirken orjinal içeriklerine müdahale edilmesine gerek kalmaz.
- Özellikle kaynak koda (Source Code) erişilemediği durumlarda ek işlevselliklerin katılabilmesinde önemli rol oynar.
- Tipleri türeterek genişletmek mümkündür, ancak türetilmelerine izin verilmeyen (Sealed Types) tipler söz konusu olduğunda çözüm genişletme metodlarıdır.

Yinede genişletme metodlarının nesne yönelimli programlama modeli nosyonunun bir parçası olmadığını düşünmekte yarar vardır. Öyleki nesne yönelimli programlama nosyonu göz önüne alındığında, tip genişletmesi aslında türetme ile gerçeklenmektedir. Böylece geldik bir makalemizin daha sonuna. Bu makalemizde kısaca genişletme metodlarını (Extension Methods) derinlemesine incelemeye çalıştık. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](/assets/files/2008/DerinlemesinExtensionMethods.rar)