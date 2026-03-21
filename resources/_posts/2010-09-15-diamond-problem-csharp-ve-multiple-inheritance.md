---
layout: post
title: "Diamond Problem, C# ve Multiple Inheritance"
date: 2010-09-15 06:28:00 +0300
categories:
  - csharp
tags:
  - csharp
  - c++
  - inheritance
  - multiple-inheritance
---
Yandaki resimde mükemmel ölçülerde bir elmas taşını görmektesiniz. Sanıyorum ki içerisinde altın orana rastlamak bile mümkündür. Elmas karatına göre derece değerli bir taş olduğu kadar, elde edilmesi de zahmetli ve zordur. Hatta özellikle Afrika’ nın elmas yönünden zengin olan bazı ülkelerinde bu taş yüzünden pek çok kişi hayatını kaybetmiştir/kaybetmektedir. Ki bu konu günümüz sinemasına da malzeme olmayı başarmıştır.

[![blg226_Giris](/assets/images/2010/blg226_Giris_thumb.jpg)](/assets/images/2010/blg226_Giris.jpg)


Elbette bizim bu günkü konumuzun elmaslar ve üzerinde dönen dolaplar ile pek bir alakası bulunmamaktadır. Aslında bu günkü yazımızda C# programlama dilinde yasaklanmış olan ama örneğin C++ ile ele alabileceğiniz çoklu kalıtım (Multiple Inheritance) konusuna farklı bir açıdan bakmaya çalışıyor olacağız.

Esasında C# programlama dili ile ilişkili kaynaklara baktığımızda, kalıtım ile ilişkili temel kurallardan birisi n sayıda sınıf kullanılarak çoklu kalıtımın yasaklanmış olduğudur. Ancak bunun sebebi veya nedenleri hakkında elle tutulur çok fazla bilgi bulunmaz. Bulunue ama pek anlaşılmaz

![Undecided](/assets/images/2010/smiley-undecided.gif)

Bu nedenle durumu buna izin veren bir dil ile değerlendirmekte yarar vardır. Olaya C++ tarafından baktığımızda, Diamond Problem adı verilen bir sorunsalın, böyle bir yasağa neden olduğunu da ifade edebiliriz. Peki Diamond Problemi nedir? Dilerseniz öncelikle bu vakayı ele almaya çalışarak işe başlayalım. Bu amaçla basit bir Win32 Console Application projesi geliştiriyor olacağız. Söz konusu proje C++ ile geliştirilmiş olup aşağıdaki kod içeriğine sahiptir.

[![blg226_CPlusPlusDiagram](/assets/images/2010/blg226_CPlusPlusDiagram_thumb.gif)](/assets/images/2010/blg226_CPlusPlusDiagram.gif)

```bash
#include "stdafx.h" 
#include <string> 
#include <stdio.h>

class BaseObject 
{ 
public: 
    BaseObject(void) 
    { 
        printf("BaseObject Default Constructor\n"); 
    } 
public: 
    virtual void WriteInfo() 
    {        
        printf("\tBaseObject WriteInfo Method\n"); 
    } 
    virtual void CheckValidation() 
    { 
        printf("\tBaseObject CheckValidation Method\n"); 
    } 
};

class Drawable 
    :public BaseObject 
    //: virtual public BaseObject 
{ 
public: 
    Drawable(void) 
    { 
        printf("\t\tDrawable Default Constructor\n"); 
    } 
    void WriteInfo() 
    { 
        printf("\t\tDrawable WriteInfo Method\n"); 
    } 
};

class Rectangle 
    :public BaseObject  
    //:virtual public BaseObject 
{ 
public: 
    Rectangle(void) 
    { 
        printf("\t\tRectangle Default Constructor\n"); 
    } 
    void CheckValidation() 
    { 
        printf("\t\tRectangle CheckValidation Method\n"); 
    } 
};

class Label 
    :public Rectangle, 
    public Drawable 
{ 
public: 
    Label(void) 
    { 
        printf("Label Default Constructor\n"); 
    } 
    void CheckValidation() 
    { 
        printf("\t\t\tLabel CheckValidation Method\n"); 
    } 
};

int main() 
{ 
    Label lblHello; 
    lblHello.CheckValidation(); 
    // lblHello.WriteInfo(); //Diamond Problem

    return 0; 
}
```

Şekilden de görüleceği üzere Label isimli sınıf iki sınıftan birden türetilmektedir. Bunlar Rectangle ve Drawable isimli sınıflardır. Söz konusu iki sınıfta tekil bir kalıtım ile BaseObject sınıfından türemektedir. BaseObject sınıfı içerisinde virtual olarak tanımlanmış iki fonksiyon yer almaktadır. Bunlardan CheckValidation metodu, Rectangle tipi içerisinde ezilmiştir (override). Diğer WriteInfo metodu da Drawable sınıfı içerisinde ezilmiştir. Benzer şekilde Label isimli sınıfta CheckValidation metodunu ezmektedir. Kodu bu şekilde çalıştırdığımızda aşağıdaki ekran görüntüsünde yer alan sonuçları alırız.

[![blg226_Runtim1](/assets/images/2010/blg226_Runtim1_thumb.gif)](/assets/images/2010/blg226_Runtim1.gif)

Sonuçlar gayet normaldir. Label tipine ait bir nesnenin kullanılması, türetildiği Drawable ve Rectangle sınıfılarının örneklenmesine neden olmaktadır. Bu sınıflarda hiyerarşiye göre BaseObject tipinden türedikleri için her birisi adına birer BaseObject örneği üretilmektedir. Label örneği üzerinden çağırılan CheckValidation metodu, override edilmiş olması nedeniyle üst tip yerine Label içerisinden yürütülmüştür.

Sıkıntı ise main metodu içerisinde bold olarak işaretlemiş olduğumuz satırı etkinleştirdiğimizde meydana gelmektedir. Burada Label tipinden lblHello değişkeni kullanılarak WriteInfo metodunun çağırıldığı görülmektedir. Ne varki Label sınıfı söz konusu metodu ezmemiştir. WriteInfo metodu ezilmemekle birlikte türediği iki üst sınıf içerisinde de ezilmiştir. Bu durumda kod hangi üst WriteInfo metodunu çağıracağına karar verememektedir. Dolayısıyla derleme zamanının kafası karışmıştır

![Sealed](/assets/images/2010/smiley-sealed.gif)

Buna göre yorum satırı açıldığında aşağıdaki hata mesajının alındığı görülecektir.

[![blg226_BuildError](/assets/images/2010/blg226_BuildError_thumb.gif)](/assets/images/2010/blg226_BuildError.gif)

Tabi bu durumun C++ tarafında çözümü vardır. Virtual Inheritance kullanılarak bu sorun çözülebilir. Yani kodda virtual public ile başlayan yorum satırlarını türetme için kullandığımızda Label nesne örneği üzerinde çağırılan WriteInfo metodunun, Drawable tipi içerisinde ezilmiş olan versiyonuna gittiği görülecektir ki buna göre çıktı aşağıdaki gibi olacaktır.

[![blg226_Runtime2](/assets/images/2010/blg226_Runtime2_thumb.gif)](/assets/images/2010/blg226_Runtime2.gif)

Yasağın bir nedenini öğrenmiş olduk. Tabi bu örnekteki gibi 4 sınıfın işin içerisine girdiği senaryolarda bu pek bir sorun olarak görülmeyebilir. Ancak sınıf ve türetme dallarının arttığı, üye sayısının fazlalaştığı hallerde Diamond Problem önemli bir sıkıntı haline gelecektir. Şimdi tekrar Managed ortama dönelim ve C# tarafında aşağıdaki gibi geliştirme yapmış olduğumuzu varsayalım.

[![blg226_CSharpDiagram1](/assets/images/2010/blg226_CSharpDiagram1_thumb.gif)](/assets/images/2010/blg226_CSharpDiagram1.gif)

```csharp
using System;

namespace MultipleInheritanceTrick 
{ 
    class WebControl 
    { 
        public int ControlId { get; set; }

        public void Render(string objectName) 
        { 
            Console.WriteLine("WriteInfo {0}", objectName); 
        } 
    } 
    class EventControl 
    { 
        public string EventName { get; set; }

        public void FireEvent(string objectInformation) 
        { 
            Console.WriteLine("ReadInfo {0}", objectInformation); 
        } 
    }

    class Button 
       :WebControl        
    { 
        public void Validate() 
        { 
            Console.WriteLine("Derived.Validate"); 
        } 
    }

    class Program 
    { 
        static void Main(string[] args) 
        { 
            Button helloBtn=new Button(); 
            //helloBtn.FireEvent("Common Informations"); 
            helloBtn.Render("Person"); 
            helloBtn.Validate(); 
        } 
    } 
}
```

Burada WebControl tipinden türeyen Button isimli bir sınıf olduğunu görmekteyiz. Aslında bu sınıfın EventControl sınıfından da türemesi iyi olabilirdi. Nitekim bu durumda Button sınıfı üzerinden veya içerisinden WebControl sınıfındaki Render ve EventControl sınıfındaki FireEvent metodlarına çağrıda bulunabilirdik. Ancak bildiğimiz üzere Button sınıfını hem WebControl hem de EventControl tipinden türetmeye çalışmamız aşağıdaki gibi sonuçlanacaktır.

[![blg226_BuildError2](/assets/images/2010/blg226_BuildError2_thumb.gif)](/assets/images/2010/blg226_BuildError2.gif)

Peki kurallar biraz olsun esnetilebilir mi?

![Wink](/assets/images/2010/smiley-wink.gif)

Yani bir şekilde EventControl ve WebControl tipi üzerinden izin verilen fonksiyonların Button tipine ait bir nesne örneği üzerinden çağırılması sağlanabilir mi? Bu konuda Internet üzerinden araştırma yaptığınız takdirde genellikle LINQ tarafında çok önemli bir yere de sahip olan Extension Method’ ların göz önüne alındığını fark edebilirsiniz. Örneğin Miguel A. Castro bir blog yazısında bu durumu ele almıştır.

Bilindiği üzere Extension Method’ lar sayesinde tiplerin fonksiyonel olarak genişletilmesi mümkündür. Hatta burada sealed gibi işaretlenmiş veya kendi kontrolümüzde olmayan tiplerin ya da türetilemeyen tiplerin genişletilebilirliği de söz konusudur. Diğer yandan C# tarafında her ne kadar sınıf seviyesinde çoklu kalıtıma izin verilmese de, bu bir sınıfı birden fazla Interface tipinden türetemeyeceğimiz anlamına da gelmemelidir.

Zaten C# tarafında çoklu kalıtım için interface mantığının kullanılması önerilmektedir. Aslında senaryomuzda yer alan sınıfları kendi geliştirdiğimiz tipler olarak düşünerek hareket ettiğimizde, içlerindeki bazı metodları extension method olarak dışarıya alıp belirli interface tiplerine uygulatmamız sorunun anahtar çözümü olmaktadır. Durumu daha iyi anlayabilmek için gelin uygulama kodumuzu aşağıdaki hale getirelim.

[![blg226_CSharpDiagram2](/assets/images/2010/blg226_CSharpDiagram2_thumb.gif)](/assets/images/2010/blg226_CSharpDiagram2.gif)

```csharp
using System;

namespace MultipleInheritanceTrick 
{ 
    interface IWebControl 
    { 
    } 
    interface IEventControl 
    { 
    } 
    class WebControl 
    { 
        public int ControlId { get; set; } 
    } 
    class EventControl 
    { 
        public string EventName { get; set; } 
    } 
    static class WebControlExtensions 
    { 
        public static void Render(this IWebControl wCtrl, string objectName) 
        { 
            Console.WriteLine("WriteInfo {0}", objectName); 
        } 
    } 
    static class EventControlExtensions 
    { 
        public static void FireEvent(this IEventControl eCtrl, string objectInformation) 
        { 
            Console.WriteLine("ReadInfo {0}", objectInformation); 
        } 
    }

    class Button 
        :IWebControl,IEventControl 
    { 
        public void Validate() 
        { 
            Console.WriteLine("Derived.Validate"); 
        } 
    }

    class Program 
    { 
        static void Main(string[] args) 
        { 
            Button helloBtn=new Button(); 
            helloBtn.FireEvent("Common Informations"); 
            helloBtn.Render("Person"); 
            helloBtn.Validate(); 
        } 
    } 
}
```

Dikkat edileceği üzere Render ve FireEvent isimli metodlar bulundukları sınıflardan çıkartılarak Extension Method haline getirilmiştir. Render metodu IWebControl arayüzüne uygulanırken, FireEvent metodu da IEventControl arayüzüne uygulanmaktadır. Tesadüfe bakın ki, Button tipimizde hem IWebControl hem de IEventControl arayüzlerinden türemektedir

![Laughing](/assets/images/2010/smiley-laughing.gif)

Bu sebepten dolayı her iki arayüz için de geçerli olan genişletme metodlarını çağırabilecek şekilde tasarlanmış hale geldiğini söyleyebiliriz. İşte size çakma çoklu kalıtım

![Wink](/assets/images/2010/smiley-wink.gif)

Bu durumda uygulama kodunun çalışma zamanı çıktısı da aşağıdaki gibi olacaktır.

[![blg226_Runtime3](/assets/images/2010/blg226_Runtime3_thumb.gif)](/assets/images/2010/blg226_Runtime3.gif)

Tabi bu tekniğin de bazı eksik yönleri ve handikapları vardır. Herşeyden önce extension method yazılması gerekmiştir ve söz konusu teknik bu sebepten sadece metodlara uygulanabilmektedir. Yani özellikler (Properties) için geçerli değildir. Tabi ilerleyen C# sürümlerinde belki de Extension Property kavramı ile karışaşabiliriz, bilemiyorum

![Wink](/assets/images/2010/smiley-wink.gif)

Okuyan arkadaşlarımız kadar benim içinde enteresan bir yazı olduğunu ifade etmek isterim. Ancak bu yazımızda en azından C++ tarafında bilinen Diamond sorunsalına bakarak C# tarafına getirilen çoklu kalıtım yasağı için bir sebep bulduğumuzu düşünüyorum. Diğer yandan Extension Method ve Interface’ leri kullanarak çoklu kalıtımın farklı bir şekilde nasıl gerçekleştirilebileceğini de görmüş olduk. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[DiamondProblem.rar (2,00 mb)](/assets/files/2010/DiamondProblem.rar)