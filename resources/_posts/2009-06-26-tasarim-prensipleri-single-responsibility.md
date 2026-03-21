---
layout: post
title: "Tasarım Prensipleri - Single Responsibility"
date: 2009-06-26 16:04:00 +0300
categories:
  - tasarim-prensipleri-design-principles
tags:
  - design-principles
---
Sanıyorum benim gibi eskiler, yandaki resimde yer alan değerli ressamı hatırlayacaklardır. Bob Ross. Küçüklüğümde (ve halen) Bob Ross'un TRT televizyonunda yayınlanan Resim Sevinci programlarını zaman zaman izler ve yarım saatlik sürede çizdiği doğa manzaralarına bakakalırdım. Rahmetli Bob bu günkü Tasarım Prensipleri uyarlanması sırasında Einstein ile birlikte küçük bir rol üstleniyor olacak. Öyleyse sözü fazla uzatmadan konumuza geçelim.

![bob-ross.jpg](/assets/images/2009/bob-ross.jpg)

Hatırlayacağınız gibi son iki blog yazımda, nesneye dayalı tasarım prensipleri içerisinde uyarlanan ilkelere değinmeye çalışmıştım. Bu günkü konumuz ise Single Responsibility prensibi. Bu prensip anlaşılması kolay ancak çoğu zaman tespit edilmesi veya gerekliliğinin ortaya çıkartılması zor bir ilke olarak karşımıza çıkmaktadır. İlkenin savunduğu tez şudur; Bir sınıf sadece tek bir sorumluluk içermelidir. Bir başka deyişle bir sınıfın birden fazla sorumluluğa sahip olmasına karşı bir ilkedir. Bunun en büyük nedeni olarak, sıklıkla yapılan ya da beklenen değişikliklerin, sorumluluk sayısı fazla olan sınıflar için yeniden kullanılabilirliği (Reusable), test edilebilirliği, genişletilebilirliği vb... zorlaştırıyor olmasıdır. Bu ilke aslında şu şekildede açıklanabilir; bir sınıfın değişikliğe uğraması için birden fazla neden olmamalıdır.

Her zamanki gibi konuyu daha kolay kavrayabilmek adına basit bir örnek üzerinden ilerlemekte yarar olduğu kanısındayım.

Bu konu ile ilişkili olaraktan çeşitli kaynaklarda oldukça farklı ve güzel örnekler bulunmakta. Özellikle şu sıralar takip ettiğim, Robert C. Martin'in [Agile Principles, Patterns, and Practices in C#](http://www.amazon.com/Principles-Patterns-Practices-Robert-Martin/dp/0131857258/ref=sr_1_1?ie=UTF8&s=books&qid=1246047268&sr=8-1)kitabındaki örneklerden esinlendiğimi baştan belirtmek isterim.

Öncelikli olarak problemi içeren Solution içeriğimizi ele alalım. Örneğimizde Star isimli bir sınıf bulunmaktadır. Bu sınıf basit anlamda 3 boyutlu bir yıldız şeklinin bazı değerlerini tutmaktadır. Ancak dahada önemlisi içerisinde yıldızın hacmini hesaplayan ve yıldız şeklinde bir Windows Formu çizen fonksiyonlar yer almaktadır.(Bu son görevleri kafamızın bir köşesinde şimdiden tutalım ![Wink](/assets/images/2009/smiley-wink.gif)) Sınıfımız bir Class Library içerisindedir. Bu sınıf içerisinde bir Windows Form'unun çizilmesi sağlandığından System.Windows.Forms assembly'ını referans etmektedir. Diğer yandan Star isimli sınıfı kullanan iki farklı uygulama söz konusudur. Bunlardan birisi bir Windows uygulaması olup Star sınıfı içerisinde Form çizen operasyonu ele almaktadır. Diğer uygulama ise basit bir Console projesidir ve sadece generic List tabanlı bir Star nesne koleksiyonundaki hacim değerlerini kullanarak bilimsel bir hesaplama gerçekleştirmektedir. Solution içeriği aşağıda görüldüğü gibidir.

![blg37_1.gif](/assets/images/2009/blg37_1.gif)

Gelelim kod içeriğimize. GraphicLib isimli sınıf kütüphanemizde yer alan Star sınıfına ait kodlar aşağıdaki gibidir.

```csharp
using System;
using System.Windows.Forms;

namespace GraphicLib
{
    public class Star
    {
        public int CornerCount { get; set; }
        public int LineWidth { get; set; }
        public int zValue { get; set; }

        public Form Paint(string text)
        {
            Form frm = new Form();
            
            // Aslında yıldız şeklinde bir form çizdirildiği varsayılabilir
            frm.Text = text;
            frm.Width = LineWidth*2;
            frm.Height = Convert.ToInt32((CornerCount * LineWidth) / 3.14);

            return frm;
        }

        public double Volume()
        {
            // Tamamen hayali bir hacim hesaplaması. Normalde oluşturulan yıldızın hacminin hesaplandığı düşünülmektedir.
            return CornerCount * LineWidth;
        }
    }
}
```

Star sınıfı içerisinde yapılan anlamsız işlemlere takılmayın. Amacımız tamamen işe yarar bir sınıfı kullanmak değil şu aşamada

![Wink](/assets/images/2009/smiley-wink.gif)

Ancak Paint ve Volume metodları bizim için oldukça önemlidir. Computer isimli Console uygulamamız içerisinde Geometric isimli yardımcı bir sınıf bulunmaktadır.

```csharp
using System.Collections.Generic;
using GraphicLib;

namespace Computer
{
    class Geometric
    {
        // Sembolik bir matematiksel hesaplama yaptığı varsayılır
        public double Compute(List<Star> stars)
        {
            double total = 0;

            for (int i = 0; i < stars.Count; i++)
            {
                total += stars[i].Volume();
            }

            return total;
        }
    }
}
```

Bu sınıf içerisinde yer alan Compute metodu, parametre olarak gelen List koleksiyonunundaki her bir eleman için Volume metodunu ele almaktadır. Computer isimli programa ait test kodları ise aşağıdaki gibidir.

```csharp
using GraphicLib;
using System;

namespace Computer
{
    class Program
    {
        static void Main(string[] args)
        {
            Geometric einstein = new Geometric();

            double result=einstein.Compute(
                new System.Collections.Generic.List<GraphicLib.Star>{
                    new Star{CornerCount=10,LineWidth=12},
                    new Star{CornerCount=8,LineWidth=24},
                    new Star{CornerCount=5,LineWidth=35}
                }
                );
            System.Console.WriteLine("Bilimsel hesaplama yapılmıştır");
            
            System.Console.WriteLine(result.ToString());

            Console.ReadLine();
        }
    }
}
```

Peki ya WinForms uygulamamız. Bu uygulama içerisinde de Painter isimli bir sınıf bulunmaktadır.

```csharp
using System.Windows.Forms;
using GraphicLib;

namespace ProblemWinForm
{
    class Painter
    {
        public void DrawScreen(Star aStar)
        {
            Form form = aStar.Paint("Yeni Form");
            
            form.Show();            
        }
    }
}
```

Bu sınıf içerisinde yer alan DrawScreen metodu, parametre olarak Star tipinden bir referans almakta ve Paint metodunu çağırmaktadır. Ve sıra Bob'tadır. İşte Windows formumuzdaki Button kontrolüne basılınca olmasını istediklerimiz.

```csharp
using System;
using System.Windows.Forms;
using GraphicLib;

namespace ProblemWinForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnDraw_Click(object sender, EventArgs e)
        {
            Painter bobRoss = new Painter();
            bobRoss.DrawScreen(new Star() { LineWidth = 100, CornerCount = 12 });
        }
    }
}
```

Peki şimdi ne oldu? Görüldüğü gibi tozu dumana kattık ve ortalığı iyice karştırdık. Karıştırdım

![Sealed](/assets/images/2009/smiley-sealed.gif)

Aslında aşağıdaki şekil zihnimizi biraz daha kolay aydınlatabilir.

![blg37_2.gif](/assets/images/2009/blg37_2.gif)

Sanıyorumki durum şimdi biraz daha netleşmektedir. Yinede açıklamaya çalışayım. Windows uygulamasında yer alan Painter sınıfı için, Star tipi içerisinde kullanılan tek bir fonksiyonellik vardır. Paint isimli metod. Painter kesinlikle Volume isimli fonskiyonla ilgilenmemektedir ki işinede yaramamaktadır zaten. Aksine Volume metodu, Geometric tipinin yer aldığı Console uygulaması için önemlidir. Diğer yandan Geometric sınıfı içinde, Star tipindeki Paint metodunun bir önemi yoktur. Buda uygulamaların taşınmaları veya dağıtılmaları esnasında gereksiz olan assembly'larında taşınması anlamına gelmektedir. Bahsettiklerimizden yola çıkarak şu sonuca varabiliriz; Star sınıfı iki farklı sorumluluğu üstlenmektedir ve bu, Single Responsibility ilkesine ters düşmektedir.

Öyleyse Single Responsibility prensibine uygun olacak şekilde nasıl bir çözüm üretebiliriz. Tek yapılması gereken sorumlulukları farklı sınıflara dağıtmak olacaktır. Yani iki farklı Star tipi tasarlanacak, bunlardan birisi Form çizme işlemini üstlenirken diğeri ise sadece hesaplama işlemlerini üzerine alacaktır. Söz gelimi GeometricStar ve GraphicStar isimli iki farklı sınıf bu amaçla tasarlanabilir.

![blg37_3.gif](/assets/images/2009/blg37_3.gif)

Grafiksel işlemlere ait sorumluluklar farklı bir sınıfa, bilimsel hesaplamalar ile ilişkili sorumluluklarda diğer bir sınıfa verilmiştir. Her iki sınıf için değişmez olan ortak özellikler ise bir üst sınıfta toplanmıştır.

Görüldüğü gibi ilke son derece basit ama bazen tespit edilmesi, görülmesi ve hatta uygulanması kolay olmayabilir.

![Undecided](/assets/images/2009/smiley-undecided.gif)

Söz gelimi bu yazıdaki örneklerde farklı Assembly'lar yer almaktadır. Star sınıfı üzerindeki sorumlulukları farklı sınıflara dağıtmış olsak bile, Console ve Windows uygulamalarının her ikiside aynı kütüphaneyi referans etmekte ve her iki Star tipinede (dolayısıyla her iki sorumluluğada) erişebilmektedir. Belkide sadece sorumlulukları dahilindeki tiplere erişmeleri için bir takım önlemler alınması gerekebilir. Nitekim bu durumda, gereksiz tiplerinde taşınması söz konusudur ki buda ürünün çevikliğini negatif etkileyebilir. İşte hepimize kafa karıştırıcı olduğu kadar gerekli olan bir tartışma konusu. Yorumlarınız, tüm okurlarımız için değerli olacaktır.

Böylece geldik bir yazımızın daha sonuna. Bu yazımızda basit bir şekilde Single Responsibility prensibini incelemeye çalıştık. Örnek tam olarak faydalı olmasada, bir sınıfın tek bir sorumluluğa sahip olması gerekliliğinin Single Responsibility prensibinin kendisi olduğunu anlamış bulunuyoruz.

![Laughing](/assets/images/2009/smiley-laughing.gif)

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[SRP.rar (109,33 kb)](/assets/files/2009/SRP.rar)
