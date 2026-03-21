---
layout: post
title: "Tasarım Desenleri - Decorator"
date: 2009-07-22 08:44:00 +0300
categories:
  - tasarim-kaliplari-design-patterns
tags:
  - design-patterns
  - oop
  - csharp
---
Görsel tasarım işinden pek anladığımı söyleyemem. Hatta ne zaman büyük bir hevesle Win Forms yada Asp.Net ön yüzü tasarlamaya kalksam renkleri bir türlü tutturamayak başladığım süreci hep yarım bırakmak zorunda kalırım. Bu sebepten genellikle arka plandaki iş mantıkları ile uğraşmayı tercih ederim. Sanırım WCF tarafında geliştirme yapmayı sevmemin en büyük nedenide bu olsa gerek. Anlaşılacağı üzere sanatsal yeteneğim pek yok. Hatta evimizin tüm dekorasyonu sevgili eşime aittir. Ama Decoration tasarım deseni deyince sanıyorumki anlatabilecek, paylaşabilecek bir kaç bilgim olabilir. İşte bu günkü konumuz Structural desenlerden olan Decorator Tasarım kalıbı.

Bu tasarım kalıbı bir nesneye dinamik olarak yeni sorumlulukların eklenmesi ve hatta var olanların çıkartılması amacıyla kullanılır. Bir açıdan bakıldığında nesneyi kendisinden türeyen alt sınıflar ile genişletmek yerine kullanılabilen alternatif bir yaklaşım olarak düşünülebilir. Desenin başlıca kahramanları ve UML şeması ise aşağıda görüldüğü gibidir.

![blg48_uml.gif](/assets/images/2009/blg48_uml.gif)

Gelelim UML şemasında görülen kahramanlarımıza;

- Component: Dinamik olarak sorumluklar eklenebilecek olan asıl nesne için sunulan arayüzdür. Interface veya abstract sınıf olarak tasarlanabilir.
- ConcreteComponent: Sorumlulukların dinamik olarak eklenebilecekleri asıl bileşen sınıflarıdır. Component arayüzünü uyarlarlar ve abstract sınıf olarak tasarlanırlar.
- Decorator: Decorator tipi hem Component arayüzünü uygular hemde kendi içerisinde Component tipinden bir nesne örneği referansını barındırır. Bu sebepten UML şemasındanda görüldüğü gibi Decorator ve Component arasında bir Aggregation ilişkisi mevcuttur.
- ConcreteDecorator: Bileşenlere yeni sorumlulukları eklemekle görevli tiptir. Ek işlevler bu tip içerisinde tanımlanan üyelerdir.

Peki bu tasarım kalıbını hangi koşullarda kullanabilir yada tercih edebiliriz?

Aslında tanımı son derece açık olmasına rağmen zihnimizde daha iyi canlanabilmesi için bir kaç senaryoyu göz önüne almamızda yarar olduğu kanısındayım. Örneğin grafiksel bir arayüzün genişletilmesini göz önüne alalım. Normal bir Windows Form'unun kendisini, Scroll ile kullanılabilir şekilde genişletmek istediğimiz bir durumda Decorator desenini ele alabiliriz. Özellikle GUI (GraphicalUserInferface) Tookit'lerinin çoğu, Decorator desenini ele alarak genişletilmeye imkan sağlarlar. (Hatta O'Reilly yayınevinden çıkan C# 3.0 Design Patterns kitabında, Windows Form'larının genişletilmesine ilişkin bir Decorator örneği verilmektedir.)

Yakın dostumuz.Net Framework içerisinde yer alan Streaming alt yapısındada (Stream, FileStream, MemoryStream...) Decorator deseninin kullanıldığını görebiliriz. Kaynak olarak MSDN Magazine dergisinde (uzun bir süre abone olup posta hizmetinin eve sürekli geç getirmesi ve sonundada getirmemesi nedeniyle, online takip ettiğim ama her yazılımcının mutlaka takip etmesi gerektiğini düşündüğüm dergi) [yayınlanan](http://msdn.microsoft.com/en-us/magazine/cc188707.aspx)makaleyi okumanızı şiddetle öneririm. Yine.Net Framework içerisine baktığımızda, Asp.Net tarafında yer alan IHttpModule türevli sınıf zincirinde de Decorator kalıbının kullanıldığını görebiliriz. Örnekleri çoğaltmak mümkündür. Ancak gelin kendi örneğimizi yaparak desenin nasıl uygulandığını öğrenmeye çalışalım.

![blg48_1.jpg](/assets/images/2009/blg48_1.jpg)

Uzun bir süredir oyunlar içerisinde kullanılan aktörlerden kendimi kurtarabilmiş değilim. Hal böyle olunca, örnek olarak bir ordunun sahip olduğu silahlar ve bu silahlara dinamik olarak yeni sorumluluklar ekleyebilmek için Decorator tasarım kalıbını ele alacağımız bir örnek geliştirmeye çalışıyor olacağız. Yandaki resimdende görüldüğü üzere senaryomuzun kahramanı bir Topçu bataryası. Ama tabiki bir Tank veya Uçak'ta olabilir. Bunların her birini Arms isimli bir bileşen olarak düşüneceğiz.

Örnek Console uygulamamızın kod içeriği aşağıdaki gibidir.

```csharp
using System;
using System.Collections.Generic;

namespace Decorator
{
    // Component
    abstract class Arms
    {
        public string Name;
        public abstract void Fire();
    }

    // ConcreteComponent
    class Artillery
        : Arms
    {
        protected double _barrel;
        protected double _range;

        public Artillery(double barrel, double range, string name)
        {
            _barrel = barrel;
            _range = range;
            Name = name;
        }

        public override void Fire()
        {
            Console.WriteLine("{0} sınıfından olan topçu, {1} mm namlusundan {2} mesafeye ateşleme yaptı", Name, _barrel.ToString(), _range.ToString());
        }
    }

    // Decorator
    abstract class ArmsDecorator
        : Arms
    {
        protected Arms _arms;
        public ArmsDecorator(Arms arms)
        {
            _arms = arms;
        }
        public override void Fire()
        {
            if (_arms != null)
                _arms.Fire();
        }
    }

    // ConcreteDecorator
    class ArtilleryDecorator
        : ArmsDecorator
    {
        public ArtilleryDecorator(Arms arms)
            : base(arms)
        {
        }

        public void Defense()
        {
            Console.WriteLine("\t{0} Savunma Modu!", base._arms.Name);
        }
        public void Easy()
        {
            Console.WriteLine("\t{0} Atış serbest modu!", _arms.Name);
        }
        public override void Fire()
        {
            base.Fire();
        }
    }

    // Client
    class Program
    {
        static void Main()
        {
            // Bileşen örneklenir
            Artillery azman = new Artillery(125, 40, "Fırtına A1");
            azman.Fire();

            // Decorator nesnesi örneklenir
            ArtilleryDecorator  azmanDekorator= new ArtilleryDecorator(azman);
            // Decorator nesnesi üzerinden o anki asıl Component için(Artillery sınıfı) ek fonksiyonellikler çağırılır.
            azmanDekorator.Defense();
            azmanDekorator.Fire();
            azmanDekorator.Easy();
            azmanDekorator.Fire();
        }
    }
}
```

![blg48_2.gif](/assets/images/2009/blg48_2.gif)

Örneğimizde Artillery isimli bir topçu bileşenine Defense ve Easy isimli yeni fonksiyonelliklerin eklenebilmesi için Decorator tasarım kalıbından yararlanılmıştır. Arms (Component) isimli bileşenden türeyen her gerçek tip için sisteme yeni bir Decorator eklenebilir. Bir başka deyişle, Tank isimli bir asıl bileşen sisteme dahil olduğunda pekala bunun içinde bir ConcreteDecorator tip (örneğin TankDecorator) söz konusu olabilir ki buda Tank için ek sorumlulukların dinamik olarak yüklenmesini sağlayabilir.

Tabiki bu desende kritik olan noktalardan biriside Decorator tipinin tanımlanış şeklidir. Bu tip kendi içerisinde, asıl bileşenleri kullanabilmek için Component tipinden bir üyeye sahiptir. Aynı zamanda bu üyenin initialize (başlatılma) işlemindende sorumludur. Diğer taraftan Component tipinden türediği için aslında Component içinde tanımlı olup, ConcreteComponent tipleri tarafından ezilmesi gereken kuralları kendiside uygulamak zorundadır. İşte bu noktada, kendi içerisinde sakladığı Component bileşeninin abstract üyelerini çağırarak çalışma zamanında taşıdığı asıl bileşenin ezdiği üyeleri devreye sokabilir (ArmsDecorator içinde overrride edilmiş Fire metoduna dikkat edelim). Ama puzzle'ın eksik kalan kısmını Decorator (ArmsDecorator) tipinden türeyen bileşenler üstlenmektedir.

Örnekte yer alan ArtilleryDecorator (ConcreteDecorator), ArmsDecorator (Decorator)' den türemekte ve kendisine çalışma zamanında verilen asıl bileşeni üst sınıfın yapıcısına iletmektedir. Bu sebepten üst sınıf üzerinden çağırılan ve asıl bileşenler tarafından ezilen tüm üyelere ulaşabilir (ArtilleryDecorator içerisinde ezilmiş olan Fire metoduna dikkat edelim). Ama aynı zamanda kendisi içerisindede ek fonksiyonellikleri tanımlayabilir. Nitekim çalışma zamanında ConcreteDecorator (ArtilleryDecorator) tipinin çalıştığı nesne örneği, ek sorumluluklar üstlenmesi istenen asıl bileşen ConcreteComponent (Artillery)' den başkası değildir. Uygulamanın çalışma zamanı çıktısına baktığımızda aşağıdaki sonuçlar elde ettiğimizi görebiliriz.

![blg48_3.gif](/assets/images/2009/blg48_3.gif)

Ancak bu desendede bazı eksik noktalar olabilir. Özellikle, nesnelere yeni fonksiyonelliklerin çalışma zamanında eklenmesi nedeni ile sistemin fonksiyonelliğine ait hataları ayıklamak (debug) daha zordur. Decorator deseni, Adapter ve Composite kalıpları ile zaman zaman karıştırılabilir. Adapter deseni bir nesnenin arayüzünü değiştirirken, Decorator kalıbı sorumlulukları (Responsibilities) değiştirmektedir. Bununla birlikte decorator deseni sadece bir component ile ilgilendiğinden, Composite kalıbının çakma bir hali olarakta düşünülebilir

![Wink](/assets/images/2009/smiley-wink.gif)

Ancak bununla birlikte Decorator kalıbının bileşene ek sorumlulukla yüklediğide bir gerçektir. Böylece geldik bir yazımızın daha sonuna. Umarım sizler için yararlı olmuştur. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Decorator.rar (22,98 kb)](/assets/files/2009/Decorator.rar)
