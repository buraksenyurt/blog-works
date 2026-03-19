---
title: "Tasarım Prensipleri - Interface Segregation"
pubDate: 2009-07-02 01:16:00
categories:
  - Tasarım Prensipleri(Design Principles)
tags:
  - Tasarım Prensipleri(Design Principles)
  - C#
  - Dependency Management
---

# Tasarım Prensipleri - Interface Segregation
Merhaba Arkadaşlar,

Bir süredir pek çok nesne yönelimli yazılım disiplininde önem arz eden ve kullanılan Tasarım Prensiplerini (Design Principles) incelemeye ve öğrendiklerimi sizlere aktarmaya çalışıyorum. Şu ana kadar pek çok prensibi inceledik ve kısaltmalarına tanık olduk.

- LCP (Loose Coupling Principle)
- OCP (Open Closed Principle)
- SRP (Single Responsibility Principle)
- LSP (Liskov Substituation Principle)
- DIP (Dependency Inversion Principle)

Elbetteki önemli olan, kısaltmalarının karşılıklarını bilmek değil

![Wink](images/smiley-wink.gif)

, söz konusu prensiplerin farkına vararak yazılım geliştirmek yada geliştirilen yazılım içerisinde bu prensiplerin uygulanabileceği, uygulanması gereken yerleri tespit edebilmektir. Bu hususları dikkate alarak ara sıra tasarım prensiplerini tekrar etmeye özen göstererekten, yeni tasarım prensibini incelemeye başlayabiliriz. Interface Segregation Principle (ISP)

Çok hızlı bir giriş olacak ama konuya aşağıdaki sınıf diagramında görülen tipleri göz önüne alarak başlayalım.(Her zamanki gibi ilkenin özlü sözünü kavrayabilmek için örnekle başlamakta yarar olduğu kanısındayım)

![blg40_1.gif](images/blg40_1.gif)

Kod tarafından baktığımızda ise;

```csharp
 interface IComponent
    {
        Guid ComponentId { get; }

        void Initialize();
        void Draw();
        void Render();
    }

    public class WinButton
    : IComponent
    {
        #region IComponent Members

        public Guid ComponentId
        {
            get { return Guid.NewGuid(); }
        }

        public void Initialize()
        {
            Console.WriteLine("Windows Button başlangıç işlemleri");
        }

        public void Draw()
        {
            Console.WriteLine("Ekrana çizdirme işlemleri");
        }

        public void Render()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    class WebButton
        :IComponent
    {
        #region IComponent Members

        public Guid ComponentId
        {
            get { return Guid.NewGuid(); }
        }

        public void Initialize()
        {
            Console.WriteLine("Web Button başlangıç işlemleri");
        }

        public void Draw()
        {
            throw new NotImplementedException();
        }

        public void Render()
        {
            Console.WriteLine("HTML Render işlemleri");
        }

        #endregion
    }
```

Aslında böylesine kötü bir tasarım ile konuya başlamak istemezdim ancak ISP ilkesinin neyi öğütlediğini bilmek adına bu yeterli bir yaklaşımdır. Hedefte bir sistemin parçası olan görsel bileşenlerin uygulaması gereken kuralları bildiren IComponent isimli arayüz tipi bulunmaktadır. Görsel bileşenler olarak örneğimizde, Windows ve Web tabanlı uygulamalardaki Button kontrolleri göz önüne alınmaktadır. Ancak bir windows kontrolünün temel olarak ekrana çizdirilmesi ile, bir Web kontrolünün istemci tarafına HTML içeriği olarak Render edilmesi iki farklı ve ap ayrı fonksiyonelliktir.

Dolayısıyla ISP ilkesine bu tespitten itibaren ters düşülmeye başlanmaktadır. Nitekim, IComponent arayüzünü uygulayan WinButton sınıfı içerisinde yer alan Render metodunun kesin olarak implemente edilmemesi gerekir. Hatta implemente edilmesi anlamsızdır. Bu nedenle çözüm olarak içerisinden NotImplementedException istisnasının fırlatıldığını görmekteyiz. Diğer taraftan benzer durum WebButton bileşeni içinde geçerlidir. Öyleki Web arayüzü için Render işlemi önemli iken, Draw isimli fonksiyonelliğin gerçekleştirilmemesi gerekir. Ayrıca IComponent arayüzüne yeni eklentiler yapılmak istendiğinde, Liskov Substitution ilkesine ters düşebilecek durumlarında oluşması söz konusudur. (Bknz: [Tasarım Prensipleri: Liskov Substitution](https://www.buraksenyurt.com/post/Tasarim-Prensipleri-Liskov-Substitution.aspx)

![Wink](images/smiley-wink.gif)

) Peki bu sorunlar bize neyi göstermektedir?

IComponent arayüzünü uygulayan tipler, aslında kendi içlerinde kullanmayacakları fonksiyonellikleri uyarlamak zorunda kalmışlardır. NotImplementedException ile istisna fırlatılmış olsa bile... İşte Interface Segregation ilkesi, bir istemcinin kullanmayacağı arayüz fonksiyonelliklerini hiç bir şekilde uygulamaması gerektiğini belirtmektedir. Buda tahmin edileceği üzere söz konusu fonksiyonellikleri farklı arayüzlere bölerek mümkün olabilir. Yani, yukarıda tasarlamış olduğumuz sistemi aşağıdaki hale getirerek ISP ilkesine sadık kalmayı başarabiliriz.

![blg40_2.gif](images/blg40_2.gif)

Kod içeriğimizi ise şu şekilde güncellemeliyiz;

```csharp
interface IComponent
    {
        Guid ComponentId { get; }

        void Initialize();
    }
    interface IWebComponent
    {
        void Render();
    }
    interface IWinComponent
    {
        void Draw();
    }

    public class WinButton
    : IComponent,IWinComponent
    {
        #region IComponent Members

        public Guid ComponentId
        {
            get { return Guid.NewGuid(); }
        }

        public void Initialize()
        {
            Console.WriteLine("Windows Button başlangıç işlemleri");
        }

        #endregion

        #region IWinComponent Members

        public void Draw()
        {
            Console.WriteLine("Ekrana çizdirme işlemleri");
        }

        #endregion
    }

    class WebButton
        : IComponent,IWebComponent
    {
        #region IComponent Members

        public Guid ComponentId
        {
            get { return Guid.NewGuid(); }
        }

        public void Initialize()
        {
            Console.WriteLine("Web Button başlangıç işlemleri");
        }

        #endregion

        #region IWebComponent Members

        public void Render()
        {
            Console.WriteLine("HTML Render işlemleri");
        }

        #endregion
    }
```

Görüldüğü gibi Web tarafını ilgilendiren Render fonksiyonu IWebComponent isimli arayüzde, Windows tarafını ilgilendiren Draw metodu ise IWinComponent arayüzü içerisinde ele alınmıştır. Artık, ortak olan üyelerin IComponent, Web tarafını ilgilendirenlerin IWebComponent ve Windows tarafını ilgilendirenlerinde IWinComponent içerisinde toplanması sağlanarak ISP ilkesinin korunması sağlanabilir. Hatta söz konusu senaryoda IWebComponent ve IWinComponent arayüzlerinin IComponent arayüzünden türemesi dahi düşünülebilir.

![blg40_3.jpg](images/blg40_3.jpg)

Bu ilke ile ilişkili olaraktan internet ve basılı kaynaklarda çok çok güzel örnekler yer almaktadır. Örneğin Object Oriented Design isimli sitede bir şirketin çalışanları göz önüne alınmıştır. Çalışanlar yemek yiyen insanlar ve yemek yemeyip sürekli çalışan Robotlardan oluşmaktadır.

![Laughing](images/smiley-laughing.gif)

Ancak ilk etapta tasarlanan herşeyi içinde barındıran şişman arayüz (Fat Interface), yemek yeme fonksiyonunu barındırdığı için, Robot'larında gerekmediği halde söz konusu işlevselliği uygulaması zorunlu olmuştur ki bu andan itibaren ISP ilkesine ters bir durum oluşmaktadır. Bu yazıyıda fikir vermesi açısından incelemenizi tavsiye ederim.

Böylece geldik bir ilkenin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[ISP.rar (31,41 kb)](assets/ISP.rar)
