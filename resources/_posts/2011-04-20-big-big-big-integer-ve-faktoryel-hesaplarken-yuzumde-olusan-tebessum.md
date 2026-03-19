---
layout: post
title: "Big Big Big Integer ve Faktöryel Hesaplarken Yüzümde Oluşan Tebessüm"
date: 2011-04-20 15:05:00 +0300
description: "Bu hüzünlü girişten sonra bu kocaman araçların konumuzla ne alakası olduğunu düşünebilirsiniz. Aslında bu gün sizlere yine .Net Framework 4.0 ile birlikte gelen yeniliklerden birisinde bahsediyor olacağım. Aslında kocaman, iri, büyük bir yenilik. BigInteger ;) .Net Framework 4.0 ile birlikte System.Numerics.dll isimli bir assembly daha gelmektedir. Bu yeni assembly içerisinde ise aşağıdaki şekilde görülen iki Değer Türü(Value Type) yer almaktadır."
categories:
  - dotnet-framework-4-0
  - csharp-4-0
tags:
  - dotnet-framework-4-0
  - csharp-4-0
  - csharp
  - dotnet
  - http
  - visual-studio
---
# Big Big Big Integer ve Faktöryel Hesaplarken Yüzümde Oluşan Tebessüm
[![blg203_Giris](/assets/images/2011/blg203_Giris_thumb.jpg)](/assets/images/2011/blg203_Giris.jpg) Merhaba Arkadaşlar,

[Monster Truck](http://en.wikipedia.org/wiki/Monster_truck) yarışlarını izleyen var mıdır bilemiyorum. Bir zamanlar Eurosport kanalında sık sık izler ve bu devasa, kocaman araçların, önlerinde ufacık kalan (ki o araçların çoğu avrupada kullanılan binek otoların çoğundan en ve boyca büyüktür) araçların üstünden atlarken onları nasıl ezdiklerine ağzım açık bakardım.

Amerikalıların gerçekten garip müsabaka anlayışları ve sportif aktiviteleri var. Monster Truck araçlarının kullanıldığı bu tip yarışmalarda bile binlerce seyirciyi toplayabiliyorlar. Üstelik bu seyirciler çılgınlar gibi bağırıp duruyor ve keyif alıyorlar. (Biz daha basketbol maçlarına seyirci toplayamazken üstelik:()

Bu hüzünlü girişten sonra bu kocaman araçların konumuzla ne alakası olduğunu düşünebilirsiniz. Aslında bu gün sizlere yine.Net Framework 4.0 ile birlikte gelen yeniliklerden birisinde bahsediyor olacağım. Aslında kocaman, iri, büyük bir yenilik. BigInteger;).Net Framework 4.0 ile birlikte System.Numerics.dll isimli bir assembly daha gelmektedir. Bu yeni assembly içerisinde ise aşağıdaki şekilde görülen iki Değer Türü (Value Type) yer almaktadır.

[![blg203_ObjectBrowser](/assets/images/2011/blg203_ObjectBrowser_thumb.gif)](/assets/images/2011/blg203_ObjectBrowser.gif)

Hey gidi günler:D Bir zamanlar [C#Nedir?](http://www.csharpnedir.com) adına düzenlenen C# Akademi eğitimlerinde, operatörlerin aşırı yüklenmesi (Operator Overloading) konusunu anlatırken genellikle kompleks sayılardan (-3i+2j gibi) yararlanırdık. Öncelikle kompleks sayıları ifade edebileceğimiz bir tip tanımlar ve bu tipe toplama, çıkarma gibi matematiksel işlemleri öğreterek Operator Overloading konusunu irdelerdik. Nihayet.Net Framework 4.0 sürümü ile birlikte Complex isimli yeni bir değer türüne daha sahip olduk. Tahmin edeceğiniz üzere bu tip ile kompleks sayıları ifade edebilmekteyiz.

System.Numerics içerisine dahil edilen ve bu yazımıza konu olan diğer bir tip ise BigInteger isimli tam sayı türüdür. Bu tip ile gerçekten çok büyük sayıları ifade edebilmemiz mümkündür. Bu önemli bir gelişmedir. Nitekim BigInteger dışında değerlendirebileceğimiz büyük sayısal değerleri düşündüğümüzde, değer aralıklarının aşağıdaki tabloda ifade edildiği gibi olduklarını görebiliriz.

Tip
Minimum Değer
Maksimum Değer

Int64 (long)
-9223372036854775808
9223372036854775807

Unsigned Int64
0
18446744073709551615

Decimal
-79228162514264337593543950335
79228162514264337593543950335

Double
-1,79769313486232E+308
1,79769313486232E+308

Her ne kadar değer aralıkları büyük görünse de bazı durumlarda asla yeterli gelmeyeceklerdir. Bu durumu daha net bir şekilde anlayabilmek için aşağıdaki Console uygulaması kodlarını göz önüne alalım. Bu arada BigInteger tipini kullanabilmemiz için System.Numerics.dll assembly’ ının projeye referans edilmesi gerektiğini de unutmayalım.

[![blg203_Reference](/assets/images/2011/blg203_Reference_thumb.gif)](/assets/images/2011/blg203_Reference.gif)

```csharp
using System; 
using System.Numerics;

namespace HugeInteger 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            Console.WriteLine("Lütfen Faktöryel değeri hesap edilecek sayıyı giriniz"); 
            int number = 0; 
            if (Int32.TryParse(Console.ReadLine(), out number)) 
            { 
                Console.WriteLine("BigInteger üzerinden hesaplama sonucu {0} ",Factorial(number)); 
                Console.WriteLine("long üzerinden hesaplama sonucu {0} ",FactorialOld(number)); 
            } 
            else 
                Console.WriteLine("Geçersiz sayısal değer"); 
        } 
        static BigInteger Factorial(int value) 
        { 
            if (value == 0 || value == 1) 
                return 1; 
            else 
                return value*Factorial(value-1); 
        } 
        static long FactorialOld(int value) 
        { 
            if (value == 0 || value == 1) 
                return 1; 
            else 
                return value * FactorialOld(value - 1); 
        } 
    } 
}
```

Yine hey gidi günler diyeceğim:) Özellikle C# programlama dilinin temellerinden Recursive metodların anlatılmasında en çok kullandığımız fonksiyonellikler arasında, Faktöryel ve Fibonacci sayı dizisi hesaplamaları gelmekteydi.

Yukarıdaki kod parçasında da kullanıcının girdiği sayısal değerin faktöryel hesaplamalarının yapıldığı iki yinelemeli (Recursive) metod görülmektedir. Bu metodlar arasındaki tek fark ise Factorial metodunun BigInteger tipinden bir değer döndürmesi diğerinin ise long tipini kullanmasıdır. İyi de ne olmuş ki? Gelin bir kaç sayısal değer için deneme yapalım.

10! (10 Faktöryel)

[![blg203_Test1](/assets/images/2011/blg203_Test1_thumb.gif)](/assets/images/2011/blg203_Test1.gif)

10 sayısının faktöryeli için bulunun sonuçlar güzel.

20!

[![blg203_Test2](/assets/images/2011/blg203_Test2_thumb.gif)](/assets/images/2011/blg203_Test2.gif)

Herşey yolunda görünüyor.

21!

[![blg203_Test3](/assets/images/2011/blg203_Test3_thumb.gif)](/assets/images/2011/blg203_Test3.gif)

Uppsss!!!:S Bir terslik var sanki. 21 sayısının faktöryel değeri için Int64 tipinden olan hesaplama negatif değer döndürdü. Oysaki BigInteger ile çalışan metodumuz olması gereken değeri döndürdü. Sanıyorum ki ne demek istediğimi gayet iyi anladınız;) Peki olayı biraz daha büyütelim mi? Örneğin 100 sayısının faktöryel değerini hesap etmek istediğimizi düşünelim.

100!

[![blg203_Test4](/assets/images/2011/blg203_Test4_thumb.gif)](/assets/images/2011/blg203_Test4.gif)

Volaaaaaa!!!!:D Oldukça büyük, kocaman bir rakam ile karşı karşıyayız. Ama doğru sonuç olduğunu ifade edebiliriz. Faktöryel hesaplamalarını kontrol etmek için [http://www.cs.uml.edu/~ytran/factorial.html](http://www.cs.uml.edu/~ytran/factorial.html) adresindeki web tabanlı hesap makinesinden de yararlanabilirsiniz. Hatta zamanında elimizde BigInteger gibi bir kavram olmadığından, bu web sayfasına kod içinden sayısal değerleri request olarak gönderip sonuçlarını program ortamına aktarmayı bile denemiştim. Artık bu kadar kolaya kaçmanın gereği yok;)

BigInteger tipi yukarıdaki kullanımı dışında sahip olduğu static metodlar sayesinde çok yüksek haneli sayılar ile kolayca çalışılabilmesine olanak sağlamaktadır. Aşağıdaki kod parçasında bir kaç örnek kullanıma yer verilmektedir.

```csharp
using System; 
using System.Numerics;

namespace HugeInteger 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        {          
            #region BigInteger diğer kullanım çeşitleri

            // 3ün 1000 üssü hesap edilmektedir. 
            // Ayrca sonucun çift sayı olup olmadığı IsEven özelliği ile kontrol edilir. 
            BigInteger number1=BigInteger.Pow(3, 1000); 
            Console.WriteLine("3^1000 = \n{0}. Sonuç çift sayı mı {1}\n",number1.ToString(),number1.IsEven); 
            // String olarak girilen büyük bir sayısal değerin Parse edilmesi işlemi gerçekleştirilir. 
            // Ayrıca girilen sayının 2nin katı olup olmadığına IsPowerOfTwo özelliği ile bakılmaktadır. 
            BigInteger number2=BigInteger.Parse("2901391039103910239120488574562098472357569235820394039473285647365349586302394723042368646"); 
            Console.WriteLine("2901391039103910239120488574562098472357569235820394039473285647365349586302394723042368646, 2nin katı mı? {0}\n", number2.IsPowerOfTwo);

            // İki BigInteger değerinden büyük olanı Max metodu yardımıyla bulunabilir. 
            BigInteger number3 = BigInteger.Max(Factorial(34), Factorial(33)); 
            Console.WriteLine("{0}\n",number3);

            // İki BigInteger sayının çarpılması için * operatörü haricinde Multiply metodundan da yararlanılabilir. 
            BigInteger number4 = BigInteger.Multiply(Factorial(10), Factorial(29)); 
            Console.WriteLine("10! * 29! = {0}\n",number4.ToString());

            // Çok büyük iki sayının bölümünde kalan değerin hesaplanması için Remainder fonksiyonundan yararlanılabilir 
            BigInteger number5=BigInteger.Remainder(Factorial(30),((BigInteger)(long.MaxValue))+1); 
            Console.WriteLine("30! ile long.MaxValue+1 in bölümünden kalan ={0}\n",number5.ToString());

            // İki büyük sayının bölümünün sonucu ve bölümden kalan değerin elde edilmesi için DivRem metodundan da yararlanılabilir 
            BigInteger number6,remainderNumber; 
            number6=BigInteger.DivRem(Factorial(22), 199, out remainderNumber); 
            Console.WriteLine("22! / 199 = {0}, Kalan {1}\n",number6.ToString(),remainderNumber.ToString());

            #endregion 
        } 
        static BigInteger Factorial(int value) 
        { 
            if (value == 0 || value == 1) 
                return 1; 
            else 
                return value*Factorial(value-1); 
        } 
    } 
}
```

Kod parçasında BigInterger türü ile ilişkili çeşitli örnek kullanımlar yer almaktadır. Uygulamanın çalışmasının sonucu aşağıdaki gibi olacaktır.

[![blg203_Test5](/assets/images/2011/blg203_Test5_thumb.gif)](/assets/images/2011/blg203_Test5.gif)

3ün 1000nci üssünün sonucunun dahi ele alınabildiği görülmektedir. Üstelik string tabanlı olarak gelen çok yüksek haneli bir değer, kolay bir şekilde sayısal olarak ele alınabilmiştir. Diğer yandan çok büyük sayılar üzerinden bölme, bölmeden kalan sonucun hesap edilmesi veya primitive bir sayısalın maksimum değerinin 1 fazlasının ele alınması mümkün hale getirilmiştir.

BigInteger türü özellikle yüksek değerli sayıların ele alındığı matematiksel hesaplamaların olduğu senaryolarda, görüntü işleme programlarında, finansal analiz yapan uygulamalarda vb… bizlere önemli kolaylıklar ve avantajlar sunmaktadır. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[HugeInteger.rar (22,79 kb)](/assets/files/2011/HugeInteger.rar) [Örnek Visual Studio 2010 Ultimate sürümü üzerinde geliştirilmiş ve test edilmiştir]