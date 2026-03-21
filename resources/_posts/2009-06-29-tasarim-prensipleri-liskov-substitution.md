---
layout: post
title: "Tasarım Prensipleri - Liskov Substitution"
date: 2009-06-29 12:57:00 +0300
categories:
  - tasarim-prensipleri-design-principles
tags:
  - design-principles
---
Bu günkü blog yazımızın kahramanı Barbara Liskov ([http://en.wikipedia.org/wiki/Barbara_Liskov](http://en.wikipedia.org/wiki/Barbara_Liskov)). Ve tahmin edeceğiniz üzere konumuz tasarım prensipleri içerisinde uygulanan disiplinlerden birisi olan Liskov Substitution (LSP) ilkesi. Bu ilke üst sınıf (Base Class) ve alt sınıf (Sub Class) arasındaki ilişkinin rol aldığı bir prensip olarak göz önüne alınabilir aslında. İlkenin özet cümlesini söylemeden önce basit bir örnek üzerinden ilerlemenin daha iyi olacağı kanısındayım. Nitekim özet cümleyi okuduğunuzda kafanızın karışmamasını garanti edemeyeceğim.

![BarbaraLiskov.jpg](/assets/images/2009/BarbaraLiskov.jpg)

![Undecided](/assets/images/2009/smiley-undecided.gif)

Örnek Console uygulamamızda Document isimli bir abstract sınıfımız mevcuttur. Bu sınıf Pdf, Xps gibi dökümanların ortak özellikleri ile uygulaması gereken kuralları tanımlamaktadır. Örneğin dökümanın network üzerinde bir yere gönderilmesi veya printer üzerinden yazdırılması bu anlamda zorunlu fonksiyonellikler olarak düşünülebilir. Diğer yandan, DocumentManager isimli bir başka sınıfta, Document sınıfına bağımlı olan bir tip olarak karşımıza çıkmaktadır. Bu tip kendi içerisinde, Document tipinden türeyen nesne örnekleri üzerinde ortak işlemlerin yapılmasını sağlamaktadır. Söz gelimi birden fazla Document tipinin yazdırılması veya gönderilmesi bu ortak operasyonlar olarak düşünülebilir. Burada işin önemli kısımlarından birisi, Xps, Pdf gibi sınıfların ve sonradan sisteme eklenecek Document türevli tiplerin, DocumentManager tarafında ortaklaşa kullanılabilecek olmasıdır. Dilerseniz örneğimize bakalım.

![blg38_1.gif](/assets/images/2009/blg38_1.gif)

```csharp
using System;

namespace Problem
{
    // Base Class
    abstract class Document
    {
        public int PageCount { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }

        public abstract void Send();
    }

    // Sub Class
    class Pdf
        : Document
    {
        public override void Send()
        {
            Console.WriteLine("PDF gönderme işlemi\n\tDöküman {0}\n\tSayfa Sayısı -> {1}\n\tDöküman sahibi -> {2}", Name, PageCount.ToString(), Owner);
        }
    }

    // Sub Class
    class Xps
        : Document
    {
        public override void Send()
        {
            Console.WriteLine("XPS gönderme işlemi\n\tDöküman {0}\n\tSayfa Sayısı -> {1}\n\tDöküman sahibi -> {2}", Name, PageCount.ToString(), Owner);
        }
    }

    // Client
    static class DocumentManager
    {
        public static void SendAll(Document[] dcmnts)
        {
            foreach (Document dcmnt in dcmnts)
            {
                dcmnt.Send();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Document[] dcmnts ={
                                     new Pdf{ Name="eXtreme Programming.pdf", PageCount=800, Owner="The Good"},
                                     new Xps{Name="Programming with C#.xps", PageCount=350, Owner="The Bad"},
                                     new Xps{Name="Design Patterns.xps",PageCount=890,Owner="The Ugly"}
                                };

            DocumentManager.SendAll(dcmnts);            
        }
    }
}
```

Console uygulaması Pdf ve Xps sınıflarına ait nesne örneklerinin toplandığı Document tipinden bir diziyi oluşturmakta ve bunu DocumentManager sınıfındaki SendAll metoduna göndermektedir. Xps ve Pdf tipleri, Document sınıfından türemiştir ve Send metodunun abstract olarak tanımlanması nedeniylede, söz konusu fonksiyonu kendi içlerinde ezmişlerdir (Override). Bu nedenle Send metodu içerisindeki döngüde, Document tipinden nesne örnekleri ele alınmasına rağmen, Send metodu çalışma zamanında yeri geldiğinde Xps için, yeri geldiğinde de Pdf için çalıştırılacaktır. İşte nesne yönelimli olmanın güzel noktalarından birisi.

![Wink](/assets/images/2009/smiley-wink.gif)

Uygulamayı çalıştırdığımızda aşağıdaki sonuçlar alırız.

![blg38_3.gif](/assets/images/2009/blg38_3.gif)

Ancak Liskov Substitution ilkesinin savunduğu da bir kural vardır. Bu kuralı göstermek için senaryomuzu şu andan itibaren biraz değiştiriyor olacağız. İlk olarak uygulamaya Document sınıfından türeyen CSharp isimli yeni bir tip eklediğimizi düşünelim. Bu tip cs uzantılı kod dosyalarını ifade etmektedir.

![blg38_2.gif](/assets/images/2009/blg38_2.gif)

Yeni senaryomuza göre CSharp isimli tiplerin temsil ettiği dökümanların (cs uzantılı kod dosyaları olarak düşünebiliriz) herhangibir sebeple Send işlemine tabi tutulmaması gerekmektedir. Oysaki Send metodu Document tipi içerisinde abstract olarak tanımlandığından, CSharp tipi içerisinde de ezilmesi gerekmektedir. Peki ya sistemde Send işleminin yaptırtılması istenmiyorsa...

İki seçeneğimiz olabilir. Bunlardan birincisi CSharp tipi içerisindeki Send metodundan üst katmana doğru bir istisna (Exception) fırlatmaktadır. Belkide geliştirici tarafından yazılmış bir istisna tipide söz konusu olabilir. Diğer bir alternatif ise Client tipi içerisindeki (DocumentManager sınıfı), SendAll metodunda tip kontrolü yapmaktır. Buna göre gelen tip CSharp ise Send işleminin icra edilmemesi sağlanabilir. Bir başka deyişle kodlarımızı aşağıdak gibi güncelleştirebiliriz.

```csharp
using System;

namespace Problem
{
    // Base Class
    abstract class Document
    {
        public int PageCount { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }

        public abstract void Send();
    }

    // Sub Class
    class Pdf
        : Document
    {
        public override void Send()
        {
            Console.WriteLine("PDF gönderme işlemi\n\tDöküman {0}\n\tSayfa Sayısı -> {1}\n\tDöküman sahibi -> {2}", Name, PageCount.ToString(), Owner);
        }
    }

    // Sub Class
    class Xps
        : Document
    {
        public override void Send()
        {
            Console.WriteLine("XPS gönderme işlemi\n\tDöküman {0}\n\tSayfa Sayısı -> {1}\n\tDöküman sahibi -> {2}", Name, PageCount.ToString(), Owner);
        }
    }

     // Sub Class
    class CSharp
        : Document
    {
        public override void Send()
        {
            // Seçeneklerden birisi exception fırlatılması olabilir. Bu durumda exception' ın üst katmanlarda ele alınması(catch) gerekir.
            throw new Exception("Bu döküman için gönderme işlemi yapılamaz");
        }
    }

    // Client
    static class DocumentManager
    {
        public static void SendAll(Document[] dcmnts)
        {
            foreach (Document dcmnt in dcmnts)
            {
                // Bir diğer seçenek tip kontrolü olabilir. Tipe göre söz konusu alt sınıf operasyonunun gerçekleştirilmemesi sağlanabilir.
                if(dcmnt is CSharp)
                    continue;
                else
                    dcmnt.Send();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Document[] dcmnts ={
                                     new Pdf{ Name="eXtreme Programming.pdf", PageCount=800, Owner="The Good"},
                                     new Xps{Name="Programming with C#.xps", PageCount=350, Owner="The Bad"},
                                     new CSharp{Name="ProductEntity.cs", PageCount=15, Owner="SourceSafe"},
                                     new Xps{Name="Design Patterns.xps",PageCount=890,Owner="The Ugly"}
                                };

            DocumentManager.SendAll(dcmnts);            
        }
    }
}
```

Örnekte SendAll metodu içerisinde tip kontrolü yapılarak söz konusu senaryonun gerçeklenmemesi sağlanmıştır. Hatta uygulama çalıştırıldığında bir önceki ile aynı sonuçlar alınacaktır. Ancak burada ilk başka Open Closed ilkesine ters düşen bir durum yaşanmaktadır. Öyleki, sisteme yeni bir tip eklendiğinde, DocumentManager içerisindede değişiklik yapılması zorunludur. Zaten, Open Closed prensibine uymayan durumlar Liskov Substitution ilkesininde bozulduğu anlamına gelmektedir. Liskov Substitution ilkesi bize şunu söylemektedir; üst sınıf ile alt sınıf nesne örnekleri yer değiştirdiklerinde, üst sınıf kullanıcısı alt sınıfın operasyonlarına erişmeye devam edebilmelidir. Oysaki son senaryoda, CSharp alt sınıfındaki operasyona erişilirken özel bir işlem yapılması gerekmiştir (tip kontrolü veya istisna yakalama işlemi). Bu örnekte LSP ilkesine aykırı olan durumu ortadan kaldırmak için yapılması gereken tek şey vardır oda CSharp sınıfını Document sınıfına ait Domain yapısından çıkartmak.

![Undecided](/assets/images/2009/smiley-undecided.gif)

Internet ve basılı kaynaklarda LSP ilkesini araştırdığınızda Rectangle ve Square örneği ilede karşılaşabilirsiniz. Bu senaryoda, Liskov Substitution ilkesini son derece basit ve yalın bir dille anlatmaktadır. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[LSP.rar (23,83 kb)](/assets/files/2009/LSP.rar)
