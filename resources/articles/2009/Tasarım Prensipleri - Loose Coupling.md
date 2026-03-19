---
title: "Tasarım Prensipleri - Loose Coupling"
pubDate: 2009-06-23 22:46:00
categories:
  - Tasarım Prensipleri(Design Principles)
tags:
  - Tasarım Prensipleri(Design Principles)
  - C#
  - WPF
---

# Tasarım Prensipleri - Loose Coupling
Merhaba Arkadaşlar

Yazılım teknolojilerinde uygulanan tekniklerin çoğunda temel tasarım prensipleri sıklıkla ele alınmaktadır/Alınmalıdı. Örneğin eXtreme Programming, Aspect Oriented Programming vb... yazılım geliştirme tekniklerinde bu prensiplerin çoğuna rastlayabiliriz. Bu yazı ile birlikte Temel Tasarım Prensiplerinin incelenmesine başlıyor olacağız ki özellikle büyük çaplı projelerde bu tip disiplinler büyük bir öneme sahiptir.

Enterprise yazılım süreçlerinde en çok zorlanılan noktalardan biriside müşteri ihtiyaçlarının sürekli olarak en hızlı şekilde karşılanması gerekliliğidir. Bu durumda yazılımın bir süre sonra kendinden geçerek

![Sealed](images/smiley-sealed.gif)

dağılmasını engellemek gerekmektedir. Bu anlamda uygulanan süreçlerin içerisinde önem arz eden noktalardan biriside, kullanılacak yazılım disiplinleridir. Bu nedenle Hatta bu presipler içerisinde tasarım desenlerininde (Design Patterns) önemli bir yeri vardır. Serimizin bu ilk yazısına en kolay prensip ile başlıyor olacağız; Zayıf Bağlılık (Loose Coupling) prensibi.

Konuyu kavramamın en güzel yolu tahmin edeceğiniz üzere basit bir örnek üzerinden ilerlemek olacaktır. Bu amaçla aşağıdaki Console uygulamasını geliştirdiğimizi düşünelim.

![blg35_1.gif](images/blg35_1.gif)

```csharp
using System;

namespace Problem
{
    // WinScreen tipine ait nesne örneklerinin yaratıcısı olan sınıf
    class ScreenCreator
    {
        private WinScreen winScreen = null;

        public ScreenCreator()
        {

        }
        public ScreenCreator(WinScreen winScr)
        {
            winScreen = winScr;
        }

        public void InitializeScreen()
        {
            winScreen.Initialize();
        }
        public void DrawScreen()
        {
            winScreen.Draw();
        }
    }

    // Windows tabanlı sistemleri için düşünülen bir ekran
    class WinScreen
    {
        public void Initialize()
        {
            Console.WriteLine("WinScreen initialize işlemi");
        }
        public void Draw()
        {
            Console.WriteLine("WinScreen Draw işlemi");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Ekran üretme işlemi başladı");

            ScreenCreator creator = new ScreenCreator(new WinScreen());
            creator.InitializeScreen();
            creator.DrawScreen();
        }
    }
}
```

Bu örnek uygulama ScreenCreator ve WinScreen isimli iki sınıf ve arasındaki ilişki ele alınmaktadır. ScreenCreator sınıfına ait nesne örnekleri, aşırı yüklenmiş yapıcı metoda gelen WinScreen referansları üzerinden Initialize ve Draw isimli fonksiyonelliklerin uygulanabilmesine imkan tanımaktadır. Böylece bir Windows Formunun başlatılması ve ekrana çizilmesi işlemlerinin gerçekleştirileceği düşünülmektedir.

Uygulamayı çalıştırdığımızda aşağıdaki sonuçları alırız.

![blg35_2.gif](images/blg35_2.gif)

Aslında kodu dikkatlice incelediğimizde ve yazılım tasarımına baktığımızda bazı sıkıntılar olduğunu görebiliriz. Örneğin;

- ScreenCreator nesne örnekleri tek başlarına anlamsızdır. Kullanışlı olmaları için WinScreen sınıfı ile birlikte ele alınmaları gerekir.
- WinScreen sınıfı içerisinde yapılacak değişiklikler ScreenCreator tipinide etikeyecektir.
- ScreenCreator sadece WinScreen tipini ele almaktadır. Oysaki web ekranları, WPF ekranları, mobile ekranlar veya x ekranlar için tasarlanmış sınıfların ScreenCreator tarafından ele alınması mümkün değildir. Kuvvetli bağ buna müsade etmemektedir.

Bu sonuçlara göre ScreenCreator ve WinScreen nesne örnekleri arasında aşağıdaki UML diagramında görülen ilişkinin olduğunu düşünebiliriz.

![blg35_3.gif](images/blg35_3.gif)

İşte Loose Coupling ilkesi söz konusu sorunlara çözüm getirmeyi kolaylaştırmaktadır. Aslında nesneler arasındaki bu kuvvetli bağların kaldırılması işi kökten çözebilir. Ne varki OOP (Object Oriented Programming) dillerinde bu mümkün değildir. Dolayısıyla bağı zayıflaştırmaya çalışmak üzere bir takım işlemler yapılabilir..Net tarafından olaya baktığımızda interface veya abstract tipleri kullanarak zayıf bağlı bir ortam oluşturabiliriz. İşte Loose Coupling prensibini uygulayan yeni kod içeriğimiz.

![blg35_4.gif](images/blg35_4.gif)

```csharp
using System;

namespace Solution
{
    public interface IScreen
    {
        void Initialize();
        void Draw();
    }

   public class WinScreen
       : IScreen
   {
       #region IScreen Members

       public void Initialize()
       {
           Console.WriteLine("WinScreen Initialize işlemi");
       }

       public void Draw()
       {
           Console.WriteLine("WinScreen draw işlemi");
       }

       #endregion
   }

   public class WebScreen
       :IScreen
   {
       #region IScreen Members

       public void Initialize()
       {
           Console.WriteLine("WebScreen initialize işlemi");
       }

       public void Draw()
       {
           Console.WriteLine("WebScreen draw işlemi");
       }

       #endregion
   }

    public class MobileScreen
        : IScreen
    {
        #region IScreen Members

        public void Initialize()
        {
            Console.WriteLine("MobileScreen initialize işlemi");
        }

        public void Draw()
        {
            Console.WriteLine("MobileScreen draw işlemi");
        }

        #endregion
    }

    public class ScreenCreator
    {
        private IScreen _screen;

        public ScreenCreator(IScreen scr)
        {
            _screen = scr;
        }
        public void InitializeScreen()
        {
            _screen.Initialize();
        }
        public void DrawScreen()
        {
            _screen.Draw();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ScreenCreator creator = new ScreenCreator(new WebScreen());
            creator.InitializeScreen();
            creator.DrawScreen();

            creator = new ScreenCreator(new WinScreen());
            creator.InitializeScreen();
            creator.DrawScreen();

            creator=new ScreenCreator(new MobileScreen());
            creator.InitializeScreen();
            creator.DrawScreen();
        }
    }
}
```

Yeni tasarımızda Initialize ve Draw isimli operasyonlar IScreen isimli bir arayüz içerisinde bildirilmiş ve bu arayüzden türeyen tiplerde ayrı ayrı uygulanmışlardır. ScreenCreator sınıfı ise kendi içerisinde IScreen interface referansını ele almaktadır. Buna göre IScreen arayüzünü implemente eden her sınıf, ScreenCreator tarafından kullanılabilir. Bir başka deyişle ScreenCreator sınıfının, üreteceği ekran ile ilişkili herhangibir bilgiye sahip olmasınada gerek yoktur. Sadece Interface referansının Initialize ve Draw operasyonlarını çağırmaktadır. Diğer yandan IScreen arayüzünü implemente eden tipler içerisinde yapılacak değişimler, ScreenCreator sınıfını doğrudan etkilemeyecektir. Hatta bu etki en aza indirgenmiş olacaktır. Olaya basit bir UML şeması ile baktığımızdaysa tipler arasındaki ilişkileri daha net görebiliriz.

![blg35_5.gif](images/blg35_5.gif)

Programı çalıştırdığımızda ise aşağıdaki sonuçları elde ederiz.

![blg35_6.gif](images/blg35_6.gif)

Böylece Temel Tasarım Prensiplerinden birisi ve bana kalırsa belkide en basiti olan Loose Coupling ilkesini incelemiş olduk. İlerleyen yazılarımızda diğer prensiplerede bakıyor olacağız. Örneğin Open Closed, Single Responsibility, Liskov Substitution vb...

![Wink](images/smiley-wink.gif)

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[LooseCoupling.rar (39,67 kb)](assets/LooseCoupling.rar)
