---
layout: post
title: "Tasarım Desenleri - Builder"
date: 2009-07-17 10:44:00 +0300
categories:
  - tasarim-kaliplari-design-patterns
tags:
  - tasarim-kaliplari-design-patterns
  - csharp
  - dotnet
---
Zaman içerisinde geliştirdiğimiz uygulamalar son derece karmaşık bir hal alırlar. Uygulamanın çapının ve ihtiyaçlarının artması bir kenara içerisinde yer alan en küçük parçaların bile kullanımları kompleksleşebilir. Bu küçük birimlerin karmaşıklaşmasına etken olarak gösterilebilecek konulardan biriside, nesne üretimleri için kullanılan sınıfların sayılarının artması ve inşa işlemlerinin kompleks olması olarak düşünülebilir. Hal böyle olunca bazı vakalar için standartlaşmış kalıpları kullanmanın genişleyebilirlik ve ölçeklenebilirlik açısından büyük yararı vardır.

Nesne üretimi söz konusu olduğunda, Creational isimli kategoride yer alan tasarım kalıpları göz önüne alınmaktadır. Bunlardan birisi olan Builder deseni, karmaşık yapıdaki nesnelerin oluşturulmasında, istemcinin sadece nesne tipini belirterek üretimi gerçekleştirebilmesini sağlamak için kullanılmaktadır.

Bu desende istemcinin kullanmak istediği gerçek ürünün birden fazla sunumunun olabileceği göz önüne alınır. Bu farklı sunumların üretimi ise Builder adı verilen nesnelerin sorumluluğu altındadır. Dolayısıyla Builder kalıbından yararlanılarak aslı ürünün farklı sunumlarının elde edilebilmesi için gerekli olan karmaşık üretim süreçleri, istemciden tamamen soyutlanabilir. Desenin önemli olan özelliklerinden biriside Abstract Factory tasarım kalıbı ile çok benzer yapıda olmasıdır. Ancak arada bazı farklılıklarda vardır. Herşeyden önce Abstract Factory kalıbına göre, fabrikanın metodları kendi nesnelerinin üretiminden doğrudan sorumludur. Builder deseninin başlıca kahramanları aşağıda sıralandığı gibidir.

- Builder: Product nesnesinin oluşturulması için gerekli soyut arayüzü sunar.
- ConcreteBuilder: Product nesnesini oluşturur. Product ile ilişkili temel özellikleride tesis eder ve Product'ın elde edilebilmesi için (istemci tarafından) gerekli arayüzü sunar.
- Director: Builder arayüzünü kullanarak nesne örneklemesini yapar.
- Product: Üretim sonucu ortaya çıkan nesneyi temsil eder. Dahili yapısı (örneğin temel özellikleri) ConcreteBuilder tarafından inşa edilir.

Bu kahramanlarımızın aralarındaki ilişkileri aşağıdaki diagram üzerindeki dizilimi ise aşağıdaki gibidir.

![blg47_uml.gif](/assets/images/2009/blg47_uml.gif)

Builder tasarım kalıbının kullanım oranı doFactory.com sitesinin istatistiklerine göre %40' lar civarındadır. Bunun en büyük nedenlerinden biriside uygun senaryoların tespit edilmesinin zor olmasıdır. Yinede desenin kullanılabileceği bir kaç senaryo üzerinde konuşarak daha kolay anlaşılmasını sağlayabiliriz.

![blg47_motorcycle.jpg](/assets/images/2009/blg47_motorcycle.jpg)

Söz gelimi dofactory.com tarafından verilen örneği göz önüne alalım. Bu örnekte motorsiklet, otomobil, scooter gibi ürünler söz konusudur. Tüm bu araçların istemci açısından kullanılabilir olması için üretim işleminde motorun (her ne kadar motorsiklette ve scooter'da kapı olmasada

![Embarassed](/assets/images/2009/smiley-embarassed.gif)

0 veya null değeri kullanılabilir), kapıların, viteslerin vb parçalarında üretimi gerekmektedir.

Aslında bu ortak fonksiyonellikler bu ürünlerin hepsi için geçerlidir. Yani bu araçların kendisi bir Product olarak temsil edilebilirler. İstemci, sadece kullanmak istediği ürünün farklı bir sunumunu elde etmek isteyecektir. Bu tip bir senaryoda istemcinin asıl ürüne ulaşmak için ele alması gereken üretim aşamalarından uzaklaştırılarak sadece üretmek istediği ürüne ait tipi bildirmesi yeterli olmalıdır. Bu senaryoda araç (Vehicle) aslında üründür (Product). Motorsiklet veya araba ise araç tipleridir ve üretim işlemleri sonucu ortaya bir Vehicle çıkartırlar. Yani desendeki ConcreteBuilder tipleridir. Bu senaryo pekala bir oyun programı içerisindeki araçların üretimi aşamasında göz önüne alınabilir.

![blg47_promotion.jpg](/assets/images/2009/blg47_promotion.jpg)

Diğer bir senaryoda bir firmanın çalışanlarına yılın belirli dönemlerinde farklı tipte promosyon ürünleri gönderdiğini düşünebiliriz. Söz gelimi farklı çalışan profilleri için sunumu farklı olan promosyon ürünlerinin geliştirilmesi safhasındaki üretim karmaşıklığı, Builder deseni ile istemciden uzaklaştırılabilir. Bu senaryoda promosyonun kendisi ürün iken, promosyon ürününü kullanacak olan profil sahipleri ConcreteBuilder tipleri olarak düşünülebilir.

Peki daha gerçekçi bir örnek olamaz mı? Her zaman dediğim gibi, aslında tasarım kalıplarının çoğunun.Net Framework içerisinde kullanıldığını kolaylıkla görebiliriz. Builder tasarım kalıbı için düşünülebilecek en güzel örnek Connection String Builder operasyonudur.

![blg47_2.gif](/assets/images/2009/blg47_2.gif)

Şekildende görüldüğü gibi bir istemcinin, kullanmak istediği Connection tipi için uygun olan bağlantı bilgisine ihtiyaç vardır. Burada bağlantının string şeklindeki içeriği önemlidir. Bu içeriğin sunum şekli ise OleDb, SQL, ODBC için farklıdır. Dolayısıyla söz konusu farklı string üretimleri için bazı ConcreteBuilder tiplerinden (OleDbConnectionStringBuilder vb...) yararlanılır. Her ne kadar DbConnectionStringBuilder abstract bir sınıf olmasada, Builder tipinin görevini üstlenmektedir.

![blg47_pizza.jpg](/assets/images/2009/blg47_pizza.jpg)

Ancak benim popüler senaryom şu anda midemden beynime doğru gelen sinyallerinde söylediği üzere Pizzacı örneğidir. Nitekim şu aşamada heleki hafta sonuna girdiğimiz şu güzel Cuma gecesinde, bu deseni eğlenceli bir şekilde ele almamamız için hiç bir sebep bulunmamaktadır.

![Wink](/assets/images/2009/smiley-wink.gif)

İşte deseni ele aldığımız kod parçaları.

```csharp
using System;

namespace Builder
{
    // Product class
    public class Pizza
    {
        public string PizzaTipi { get; set; }
        public string Hamur { get; set; }
        public string Sos { get; set; }

        public override string ToString()
        {
            return String.Format("{0} {1} {2}", PizzaTipi, Hamur, Sos);
        }
    }

    // Builder class
    public abstract class PizzaBuilder
    {
        protected Pizza _pizza;

        public Pizza Pizza
        {
            get { return _pizza; }
        }

        public abstract void SosuHazirla();
        public abstract void HamuruHazirla();
    }

    // ConcreteBuilder class
    public class BaharatliPizzaBuilder
        : PizzaBuilder
    {
        public BaharatliPizzaBuilder()
        {
            _pizza = new Pizza { PizzaTipi = "Baharatlı Baharatlı" };
        }
        public override void SosuHazirla()
        {
            _pizza.Sos = "Acı sos, pepperoni, atom biber";
        }

        public override void HamuruHazirla()
        {
            _pizza.Hamur = "İnce Kenar, Kaşarlı";
        }
    }

    // ConcreteBuilder Class
    public class DortMevsimPizzaBuilder
        : PizzaBuilder
    {
        public DortMevsimPizzaBuilder()
        {
            _pizza = new Pizza { PizzaTipi = "4 Mevsim" };
        }
        public override void SosuHazirla()
        {
            _pizza.Sos = "Biber, Domates, Peynir, Salam, Sosis";
        }

        public override void HamuruHazirla()
        {
            _pizza.Hamur = "Kalın, fesleğenli";
        }
    }

    // Director Class
    public class VedenikliKamil
    {
        public void Olustur(PizzaBuilder vBuilder)
        {
            vBuilder.SosuHazirla();
            vBuilder.HamuruHazirla();
        }
    }

    // Client class
    class Program
    {
        static void Main(string[] args)
        {
            PizzaBuilder vBuilder;

            VedenikliKamil kamil= new VedenikliKamil();
            vBuilder = new BaharatliPizzaBuilder();
            
            kamil.Olustur(vBuilder);
            Console.WriteLine(vBuilder.Pizza.ToString());

            vBuilder = new DortMevsimPizzaBuilder();
            kamil.Olustur(vBuilder);
            Console.WriteLine(vBuilder.Pizza.ToString());
        }
    }
}
```

İstemcinin tek derdi istediği tipte bir pizza almaktır. Örneğin 4 mevsim veya Baharatlı pizza. Bu pizzaların içinde ise soslarının ve hamurlarının belirlenerek üretim işlemine dahil edilmesi gerekmektedir. Bu VenedikliKamil açısından kolay olmakla birlikte istemciyi ilgilendiren bir durum değildir. Bu nedenle istemcinin sadece pizza üretimini gerçekleştiren asıl ConcreteBuilder nesne örneğini seçmesi yeterlidir. Bu seçim işlemi Director sınıfı içerisindeki Olustur metoduna parametre olarak gönderilir. Sonrasında ise istemcinin istediği pizza üretilerek elde edilir. Örneği çalıştırdığımızda aşağıdaki sonuçları elde ederiz.

![blg47_4.gif](/assets/images/2009/blg47_4.gif)

Umarım sizler içinde faydalı bir anlatım olmuştur. Her zamanki gibi bu desenin görsel dersinide en kısa sürede eklemeye çalışacağım. Bu yazının üstünede şöyle güzel bir espresso içilir kanımca

![Wink](/assets/images/2009/smiley-wink.gif)

![blg47_espresso.jpg](/assets/images/2009/blg47_espresso.jpg)

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Builder.rar (23,15 kb)](/assets/files/2009/Builder.rar)