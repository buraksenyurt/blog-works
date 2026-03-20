---
layout: post
title: "Tasarım Prensipleri - Open Closed"
date: 2009-06-25 04:22:00 +0300
categories:
  - tasarim-prensipleri-design-principles
tags:
  - tasarim-prensipleri-design-principles
  - csharp
---
Bir önceki yazımda, yazılm tasrımında benimsenen ilkelerinden birisi olan Loose Coupling prensibine değinmiştik. Bu yazımızda ise, Open Closed (Açık Kapalı) prensibine değiniyor olacağız.(Bu prensibini pek çok yazılım disiplininde görebilirsiniz. Örneğin eXtreme Programming veya Aspect Oriented Programming-AOP içerisinde.)

Açık kapalı prensibi aslında son derece basit bir ilkedir. Bu ilke bir sistemin sürekli olarak değişimlere maruz kalabileceğini göz önüne alaraktan (ki örneğin çevik süreçlerde hızlı değişimler asıl odaklanılan noktadır), genişletilmeye açık ama modifiye edilmeye kapalı varlıkların (Sınıf, Method vb...) kullanılmasını önerir. Gerçekten de günümüz Enterprise çözümlerin çoğunda,müşteri ihtiyaçlarına göre yazılımın sürekli güncelleniyor olması gerekmektedir. Bu noktada güncelleştirme işlemleri sırasında koda dokunmadan ilerlemeye çalışmak neredeyse imkansızdır. Ancak bu risk en aza indirgenmeye çalışılabilir. OCP (Open Closed Principle) bu noktada devreye giren prensiplerden sadece birisidir. Tabikide bu teorik anlatım bir örnekle süslenmediği takdirde çok anlaşılır değildir

![Wink](/assets/images/2009/smiley-wink.gif)

Gelin önce problemli bir tasarım ile yola çıkalım ve sonrasında ise OCP'i nasıl uygulayabileceğimize bakalım (ki bu noktada bir önceki blog yazısına göre bir dejavu yaşayabilirsiniz benden söylemesi ![Surprised](/assets/images/2009/smiley-surprised.gif))

![blg36_1.gif](/assets/images/2009/blg36_1.gif)

Yukarıdaki basit UML şemasında farklı formatlarda resimler üretmek için kullanılan bir yaratıcı sınıf (ImageCreator) görülmektedir. İlişkidende anlaşılacağı üzere ImageCreator sınıfı ile diğer resim sınıfları arasında kuvvetli bir bağ vardır. Bu acemi tasarımı biraz toparlamaya çalışmak istediğimizi düşünelim. Belkide aşağıdaki UML şemasında görülen kurguyu tasarlamış olabiliriz.

![blg36_2.gif](/assets/images/2009/blg36_2.gif)

Bu kez ImageBase isimli bir ata sınıfı işin içerisine katmışız gibi görünüyor. Hatta koduda aşağıda şekilde tasarladığımızı düşünelim (Tabi bu şekilde kod yazmamızın amacı tamam şakacıktan. Amaç bizi Open Close prensibine götüren sebepleri ortaya koyabilmek. ![Wink](/assets/images/2009/smiley-wink.gif))

![blg36_4.gif](/assets/images/2009/blg36_4.gif)

```csharp
using System;

namespace Problem
{
    class ImageCreator
    {
        ImageBase _image = null;
        public ImageCreator(ImageBase obj)
        {
            _image = obj;
        }
        public void Randomize()
        {
            if (_image is Bmp)
                ((Bmp)_image).Randomize();
            else if (_image is Jpg)
                ((Jpg)_image).Randomize();
            else if (_image is Tif)
                ((Tif)_image).Randomize();
            else
                Console.WriteLine("Geçersiz format");
        }
        public void Draw()
        {
            if (_image is Bmp)
                ((Bmp)_image).Draw();
            else if (_image is Jpg)
                ((Jpg)_image).Draw();
            else if (_image is Tif)
                ((Tif)_image).Draw();
            else
                Console.WriteLine("Geçersiz format");
        }    
    }

    class ImageBase
    {
    }

    class Bmp
        :ImageBase
    {
        public void Randomize()
        {
            Console.WriteLine("Random bitmap");
        }
        public void Draw()
        {
            Console.WriteLine("Draw bitmap");
        }
    }

    class Jpg
        :ImageBase
    {
        public void Randomize()
        {
            Console.WriteLine("Random Jpg");
        }
        public void Draw()
        {
            Console.WriteLine("Draw Jpg");
        }
    }

    class Tif
        :ImageBase
    {
        public void Randomize()
        {
            Console.WriteLine("Random Tif");
        }
        public void Draw()
        {
            Console.WriteLine("Draw Tif");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ImageCreator creator = new ImageCreator(new Jpg());
            creator.Randomize();
            creator.Draw();

            creator = new ImageCreator(new Tif());
            creator.Randomize();
            creator.Draw();
        }
    }
}
```

Kodun amacına göre farklı formatta Image tiplerini oluşturan bir sınıf söz konusudur. Bu sınıf içerisinde yer alan Randomize ve Draw isimli metodlar parametre olarak ImageBase tipinden referanslar almaktadır. Her iki metodda kendi içerisinde, gelen tipin Jpg, Bmp veya Tif olup olmadığına bakarak işlemler yapmaktadır (Kapalılık ilkesi zaten if kısmında bozulmaktadır ![Sealed](/assets/images/2009/smiley-sealed.gif)) Hatta tip tespitinden sonra doğru Randomize yada Draw metodunu çağırabilmek için bir Cast işlemininde uygulandığını görebiliriz. Tabiki normal şartlarda bu tip bir tasarımı tercih etmeyiz, etmemeliyiz. Nitekim söz konusu tasarımın şu sorunları doğuracağı ortadadır.

- Yeni bir resim formatı sisteme eklenmek istendiğinde ImageCreator sınıfı içerisinde yer alan Randomize ve Draw metodlarında yer alan if koşullarına ilaveler yapılması gerekmektedir. Buda üretici sınıf koduna müdahele edilmesi anlamına gelmektedir.
- Her yeni imaj eklenişinde unit testlerininde (eğer hazırlanmışlarda) tekrardan tasarlanması veya oluşturulması gerekir. Özellikle UnitTest'i yapılmış olan bir kod parçasında tekrardan değişikliğe gidilmek zorunda kalınması, testin yeniden kurgulanmasınıda gerektirecektir. En azından eski teste olan güveni sorgulatacaktır.

Bu sonuçlara göre ImageCreator sınıfı için Closed bir yapı sağlanamadığını ifade edebiliriz. Bir başka deyişle modifiyeye açık (ama olmaması gereken) bir tip söz konusudur. Peki öyleyse Open Closed prensibine uygun olarak kod nasıl tasarlanabilir. Önce UML şemasındaki düzenlememizi yapalım.

![blg36_3.gif](/assets/images/2009/blg36_3.gif)

Görüldüğü gibi ImageCreator ile Jpg, Bmp, Gif ve benzeri resim sınıflar arasındaki kuvvetli bağ ortadan kaldırılmış, kurallar interface tipine yıkılmıştır. Peki ya bu şemayı C# tarafında nasıl uygulayabiliriz. İşte örnek Console uygulaması kodlarımız.

![blg36_5.gif](/assets/images/2009/blg36_5.gif)

```csharp
using System;

namespace Solution
{
    public class ImageCreator<T>
        where T:IImage
    {
        private T _image;

        public ImageCreator(T img)
        {
            _image = img;
        }

        public void RandomizeImage()
        {
            _image.Randomize();
        }
        public void DrawImage()
        {
            _image.Draw();
        }
    }

    public interface IImage
    {
        void Randomize();
        void Draw();
    }

    class Bmp
        :IImage
    {
        public void Randomize()
        {
            Console.WriteLine("Random bitmap");
        }
        public void Draw()
        {
            Console.WriteLine("Draw bitmap");
        }
    }

    class Jpg
        :IImage
    {
        public void Randomize()
        {
            Console.WriteLine("Random Jpg");
        }
        public void Draw()
        {
            Console.WriteLine("Draw Jpg");
        }
    }

    class Tif
        :IImage
    {
        public void Randomize()
        {
            Console.WriteLine("Random Tif");
        }
        public void Draw()
        {
            Console.WriteLine("Draw Tif");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ImageCreator<Bmp> creator = new ImageCreator<Bmp>(new Bmp());
            creator.RandomizeImage();
            creator.DrawImage();

            ImageCreator<Tif> creator2 = new ImageCreator<Tif>(new Tif());
            creator2.RandomizeImage();
            creator2.DrawImage();            
        }
    }
}
```

Dikkat edileceğiz üzere Jpg, Gif ve Bmp isimli sınıflar IImage arayüzünü uygulamaktadır. Diğer taraftan ImageCreator sınıfı kendi içerisinde IImage arayüzünü ele alarak Randomize ve Draw operasyonlarını icra etmektedir (Neden dejavu yaşayacağınızı anladınız sanırım ![Laughing](/assets/images/2009/smiley-laughing.gif)) Buna göre ImageCreator tipinin yapısını bozmadan sisteme yeni resim formatları eklenmesi sağlanabilir. Dolayısıyla ImageCreator sınıfı OCP uyumlu hale getirilmiştir.

Ve bu blog girişinin Özlü Cümlesi: OCP ilkesi, sınıf, metod gibi OOP varlıklarının genişletilmeye açık (Open) ancak düzenlenmeye kapalı (Closed) olması gerektiğini savunur. Özellikle müşteriden gelen istekler nedeniyle sık sık genişletilmesi gereken varlıklarda, genişletmenin kod içerisinde mümkün olduğunca az meydana gelmesine çalışmak gerekir. İlkenin amacını, yeni fonksiyonelliklerin kazandırılması için minimum kod değişikliğinin yapılması olarak düşünebiliriz.

Bu arada kod ve resimlerin bazı yerlerinde Tif bazı yerlerinde Gif formatlarını ele aldığımı farkettim. Ama son yaptıpımız OCP tasarımına göre hiç sorun değil

![Wink](/assets/images/2009/smiley-wink.gif)

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[OCP.rar (42,53 kb)](/assets/files/2009/OCP.rar)