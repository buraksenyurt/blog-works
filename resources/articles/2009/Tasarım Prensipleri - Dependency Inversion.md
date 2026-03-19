---
title: "Tasarım Prensipleri - Dependency Inversion"
pubDate: 2009-06-30 10:45:00
categories:
  - Tasarım Prensipleri(Design Principles)
tags:
  - Tasarım Prensipleri(Design Principles)
  - C#
  - .NET
  - XML
  - JSON
  - Testing
  - Dependency Management
---

# Tasarım Prensipleri - Dependency Inversion
Merhaba Arkadaşlar,

Bu yazımızda Dependency Inversion isimli tasarım prensibinden bahsediyor olacağız. Bu prensip kabaca, alt sınıflar ve üst sınıflar arasında kuvvetli bir bağ olmamasını önermektedir. Bunun en büyük gerekçesi, alt sınıflarda olabilecek sık değişiklerin, üst sınıfında değişmesine neden olabilecek olmasıdır ki bu hızla değişen yazılım ihtiyaçlarında sorunlara neden olmaktadır. Buna birde yeni alt tipler ile genişletilebilme olasılıklarınıda eklersek, üst ve alt sınıflar arasındaki bağımlılıkların ortadan kaldırılmasının (bağımsızlık olarak düşünmek istesemde, bağımlılığın tersine çevrilmesi olarak bilmek zorundayız ![Wink](images/smiley-wink.gif)) aslında ne kadar önemli olduğu anlaşılabilir. Durumu daha iyi kavrayabilmek adına basit bir örnek üzerinden ilerlemek çok daha doğrudur. Öncelikli olarak aşağıdaki sınıf diagramı ve kod içeriğinde görülen örnek Console uygulamasını göz önüne alalım.

![blg39_1.gif](images/blg39_1.gif)

```csharp
using System;

namespace Problem
{
    // Low Level Class
    class XmlContent
    {
        public string Content { get; set; }

        public void Parse()
        {
            Console.WriteLine("parsing işlemi");
        }
    }

    // High Level Class
    class Parser
    {
        XmlContent xContent { get; set; }

        public Parser(XmlContent xmlContent)
        {
            xContent = xmlContent;
        }

        public void DoWork()
        {
            // Kompleks işlemler yapıldığını varsayabiliriz.
            xContent.Parse();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Parser prsr = new Parser(new XmlContent { Content = "<Kitaplar><Kitap><Ad>C#</Ad></Kitap><Kitap><Ad>VB.Net</Ad></Kitap></Kitaplar>" });
            prsr.DoWork();
        }
    }
}
```

Bu örnekte yer alan Parser sınıfı, kendi içerisinde tanımlı olan XmlContent tipine bağımlıdır. XmlContent tipi temel olarak bir Xml içeriğini ifade etmek üzere tasarlanmış olup, ayrıştırma işlemi için özel bir metoda sahiptir. Diğer yandan Parser sınıfı içerisinde yer alan DoWork metodu, XmlContent tipinin Parse metodunu çağırmaktadır. DoWork metodu içerisinde aslında, XmlContent tipi ile ilişkili olaraktan farklı ve hatta karmaşık bazı iş kurallarının uygulandığı farz edilebilir. Ancak alt sınıf (Low Level Class) olarak kabul edebileceğimiz XmlContent tipinin yapısında olabilecek değişikliker tahmin edileceği üzere üst sınıfı (High Level Class) doğrudan etkileyecektir.

Özellike üst sınıfta kod değişikliğine gidilmesi gerekecektir. Diğer yandan senaryoya yeni bir tip eklenmek istendiğinde de Parser tipinin bozulması ve içeriğinin değiştirilmesi gerekecektir. Yani genişletilebilirlik söz konusu olduğunda, üst sınıfta kod meydana gelecek kod değişimi kaçınılmaz olacaktır. Şimdi senaryomuzu genişletip sorunu ortaya koymaya çalışalım. Bu amaçla var olan sisteme, yeni bir alt sınıfın eklendiğini farzedelim. Örneğin Parser sınıfının, Json formatındaki dökümanları ifade eden bir sınıf ilede çalışması istenebilir. Bu durumda 3 temel sorundan bahsedebiliriz;

- Yeni eklenen tip nedeniyle Parser sınıfının kendisine müdahele edilmesi ve DoWork metodunun kod içeriğinin değiştirilmesi gerekecektir.
- DoWork metodu veya Parser sınıfı içerisindeki bazı fonksiyonelliklerin işleyişi olumsuz yönde etkilenebilir.
- Her ne olursa olsun, Unit Test işlemlerinin tekrardan oluşturulması ve yapılması gerekecektir.

Şimdi biraz durup bu sorunların üstesinden nasıl gelebileceğinizi düşünmenizini öneririm. Hatta düşünürken bir kahve arası verebilir ve çevrenizde bu konu ile ilgili kişiler varsa onlarla durumu tarışabilirsiniz.

![Wink](images/smiley-wink.gif)

Dependency Inversion prensibi, yukarıda bahsediğimiz tipte bir senaryonun oluşmasını engellemek için, üst sınıf ile alt sınıf arasına bir soyutlayıcının (abstract class veya interface) konulmasını belirtir. Yani üst sınıf ve alt sınıf arasında bir interface veya abstract tipin kullanılması önerilmektedir. İşte örnek senaryomuzun Dependency Inversion prensibine uygun olarak geliştirilen son hali.

![blg39_2.gif](images/blg39_2.gif)

```csharp
using System;

namespace Solution
{
    // Abstraction Layer
    interface IContent
    {
        string Content { get; set; }

        void Parse();
    }

    // Low Level Class
    class XmlContent
        :IContent
    {
        #region IContent Members

        public string Content { get; set; }

        public void Parse()
        {
            Console.WriteLine("Xml parsing işlemi");
        }

        #endregion
    }

    // Low Level Class
    class JsonContent
        : IContent
    {
        #region IContent Members

        public string Content { get; set; }

        public void Parse()
        {
            Console.WriteLine("Json parsing işlemi");
        }

        #endregion
    }

    // High Level Class
    class Parser
    {
        IContent content { get; set; }

        public Parser(IContent cntnt)
        {
            content = cntnt;
        }

        public void DoWork()
        {
            // Kompleks işlemler yapıldığını varsayabiliriz.
            content.Parse();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Parser prsr = new Parser(new XmlContent { Content = "" });
            prsr.DoWork();

            prsr = new Parser(new JsonContent { Content = "" });
            prsr.DoWork();
        }
    }
}
```

Görüldüğü gibi XmlContent ve JsonContent isimli tiplerimiz IContent arayüzünden türetilmiştir. Buna bağlı olarakta Parser tipi ile IContent arayüzü arasında bir bağlantı oluşturulmuştır. Bir başka deyişle, Parser tipinden XmlContent veya JsonContent tiplerine doğrudan bir bağ mevcut değildir. Artık alt sınıflar olan XmlContent ve JsonContent içerisinde istenilen değişiklikler yapılabilir. Söz gelimi Parse metodlarının iş mantığında değişimler olabilir yada tipler içerisine yeni üyeler eklenebilir. Sonuç olarak; arayüzün (veya soyut tipin) yapısının değiştirilmediği düşünüldüğünde, üst sınıfın, alt sınıflardaki olası değişiklier ve sistemdeki genişletmelere karşı bağımsız olması garanti altına alınmış olur.

Bir sonraki yazımızda görüşünceye dek hepinize mutlu günler dilerim.

[DIP.rar (40,27 kb)](assets/DIP.rar)
