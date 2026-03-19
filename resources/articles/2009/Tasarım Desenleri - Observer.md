---
title: "Tasarım Desenleri - Observer"
pubDate: 2009-07-09 07:35:00
categories:
  - Tasarım Kalıpları(Design Patterns)
tags:
  - Tasarım Kalıpları(Design Patterns)
  - C#
  - .NET
  - Java
  - Generics
  - Visual Studio
---

# Tasarım Desenleri - Observer
![blg44_1.jpg](images/blg44_1.jpg)

Merhaba Arkadaşlar,

Yaklaşık olarak 10 yılı aşkın bir süredir yazılım teknolojileri ile ilgilenmekteyim. Bu süre zarfı içerisinde Delphi 1.0' dan tutunda Java'ya, C++'tan C#' a, Visual Basic'ten Cobol'a kadar pek çok programlama dili ile uğraşma fırsatım oldu. Her ne kadar dünyada kaç nesne yönelimli programlama dili olursa olsun, hayatın ve dolayısıyla yazılım teknolojilerinin değişmez kuralları arasında tasarım kalıpları yer almakta...

Ve birde platform, dil, cihaz gözetmeksizin var olan oyunlar.

![Laughing](images/smiley-laughing.gif)

Bende her ne kadar yazılımı bir hayat biçimi olarak benimsemiş olsamda, zaman zaman eski günlerdeki gibi strateji oyunlarını oynamıyor da değilim. Warcraft 2 ve "Yeş mi Lord" ile başlayan bu serüvende, Starcraft'taki Protosları, Red Alert'taki Yuri'nin Mind Control makinelerini, Generals'taki Stealth Fighter'ları ve daha nicelerini kullanma fırsatım oldu. Geçen akşamda kafamda bir oyun canlandırmaya çalışırken, korumakta olduğum topraklara yapılan bir saldırıyı, filomdaki birimlere nasıl ileteceğimi düşünmeye başladım (Çok doğal olarak telsizle bildir olsun bitsin diyebilirsiniz)

Bir saldırı anında tank, helikopter, yaya piyade veya deniz gücündeki operatörlere bu durumun iletilmesi gerekiyordu. Aslında birilerininde sürekli olarak dinlemede olması şarttı. Aslında bu birimlerin hepsi oyun sahası içerisinde yer alan birer nesne idi ve OOP tarafında tip olarak modellenebilirlerdi. Dolayısıyla aralarında bire-çok (one to many) ilişki olabilecek nesnelerin olması ve birisinde meydana gelecek değişikliklerin diğerlerine bildirilmesi gibi senaryo ortaya çıkmaktaydı.(Bir filonun komuta merkezinden, ona bağlı tüm birimlere haber gitmesi durumu)

Duruma farklı senaryolardan da bakabiliriz aslında. Örneğin,

- bir stok takip sisteminde, stok hareketlerinde olan değişimlerin bayilere bildirimesi,
- haber ajanslarının, kendilerine bağlı olan bölümlere yeni başlıklar geldikçe bilgilendirmede bulunmaları,

gibi gerçek hayat senaryoları söz konusu olabilir.

Tahmin edeceğini üzere varmakta olduğumuz nokta bu 3 basit senaryoda benzer vakanın ve ihtiyaçların olmasıdır. Buda bizi bir tasarım kalıbına götürmektedir. Observer tasarım deseni.

![Wink](images/smiley-wink.gif)

Bu desen o kadar yaygındır ki onu;

- Model View Controller (MVC) içerisinde
- veya.Net ile Java tarafındaki olay güdümlü programlamada (Event Based Programming)

bulabiliriz.

Aslında bu desen servis yönelimli mimarilerde dahi karşımıza çıkmaktadır. Özetle çok sık kullanılan bir kalıp olduğunu ifade edebiliriz. Desenin ilkesi az öncede belirttiğimiz üzere birbirlerine bire-çok ilişki ile bağlı nesnelerde olabilecek değişiklikleri diğerlerine iletmektir. Bu nedenle izlenmeye değer bir konu ve bununla ilişkili nesneler (Subject, ConcreteSubject) söz konusudur. Ayrıca konuyu takip edecek olan tiplerde (Observer, ConcreteObserver) bulunmaktadır.

![blg44_2.jpg](images/blg44_2.jpg)

Dilerseniz konumuza basit bir örnek ile devam edelim. Yazımıza giriş kısmında bahsettiğimiz askeri strateji bazlı senaryoyu bu bağlamda göz önüne alabiliriz. Senaryomuza göre konumuz, bir saldırı veya benzer durumlarda, istediğimiz bazı birliklerin operatörlerinin bilgilendirilmesidir.

Burada bilgilendirmeyi yapacak olan yayımcılar (Publishers) ve bunu dinleyecek olan aboneler (Subscribers) olduğunuda düşünebiliriz aslında. Aslında Observer deseni bir anlamda publisher/subscriber modelinin bir uygulanış biçimi olarakta düşünülebilir. İşte sınıf diagramımız ve kodlarımız.

![blg44_5.gif](images/blg44_5.gif)

(Bu arada sınıf diagramı biraz farklı görünebilir. Nitekim şu anda bu yazıyı kayınvalidemin bilgisayarında yazmaktayım. Makine Visual Studio yok belki ama SharpDevelop sağolsun her ihtiyacımı görüyor. ![Wink](images/smiley-wink.gif))

Ve kodlarımız;

```csharp
using System;
using System.Collections.Generic;

namespace Observer
{
    /// <summary>
    /// Subject class
    /// </summary>
    internal abstract class HeadQuarters
    {
        private string _information;
        private List<IOperator> _operators=null;

        protected HeadQuarters(string information)
        {
            _operators = new List<IOperator>();
            Information = information;
        }

        public string Information
        {
            get { return _information; }
            set { 
                _information = value; 
                NotifyOperators();
            }
        }

        public void AddOperator(IOperator opt)
        {
            _operators.Add(opt);
        }
        public void RemoveOperator(IOperator opt)
        {
            _operators.Remove(opt);
        }
        public void NotifyOperators()
        {
            foreach (IOperator opt in _operators)
            {
                opt.Update(this);
            }
        }
    }

    /// <summary>
    /// Concrete Subject class
    /// </summary>
    internal class RedFleetBase
        :HeadQuarters
    {
        public RedFleetBase(string information)
           :base(information)
        {
            
        }

        public RedFleetBase()
            :base("...")
        {
            
        }
    }

    /// <summary>
    /// Observer class
    /// </summary>
    internal interface IOperator
    {
        void Update(HeadQuarters headQuarters);
    }

    /// <summary>
    /// Concrete Observer class
    /// </summary>
    internal class PlatoonOperator
        :IOperator
    {
        public string OperatorName { get; set; }

        #region IOperator Members

        public void Update(HeadQuarters headQuarters)
        {
            Console.WriteLine("[{0}] : {1}",OperatorName,headQuarters.Information);
        }

        #endregion
    }

    /// <summary>
    /// Concrete Observer Class
    /// </summary>
    internal class TankOperator
        : IOperator
    {
        public int TankId  { get; set; }
        #region IOperator Members

        public void Update(HeadQuarters headQuarters)
        {
            Console.WriteLine("[{0}] : {1}", TankId, headQuarters.Information);
        }

        #endregion
    }

    /// <summary>
    /// Client App
    /// </summary>
    class Program
    {
        static void Main()
        {
            RedFleetBase redFleetBase = new RedFleetBase {Information = "Süper işlemciler piyasada"};
            redFleetBase.Information = "İşlemciler gelişiyor";

            redFleetBase.AddOperator(new PlatoonOperator { OperatorName="Azman"} );
            redFleetBase.AddOperator(new PlatoonOperator { OperatorName = "Kara Şahin"});
            redFleetBase.AddOperator(new PlatoonOperator { OperatorName="Kartal Kondu"});

            redFleetBase.Information = "Tüm birlikler Sarı Alarma! Sarı Alarma!";

            Console.WriteLine("");

            redFleetBase.Information = "Emir iptal! Emir iptal!";

            Console.WriteLine("");

            redFleetBase.AddOperator(new TankOperator{TankId=701});
            redFleetBase.AddOperator(new TankOperator{TankId=801});
            redFleetBase.Information = "Sınır ihlali.";
        }
    }
}
```

Biraz kodları incelemekte fayda olduğu kanısındayım...

Subject tipimiz olan HeadQuarters, Information isimli bir özelliğe sahiptir. Bu özellik, Observer tiplerinin değerlendireceği bir bilgidir. Bir başka deyişle, HeadQuarters'tan, IOperator türevlerine aktarılan bildirimdir. HeadQuarters abstract tipi içerisinde bilgilendirilme yapılacak IOperator türevleride bir koleksiyon içerisinde saklanmaktadır. Buna göre, HeadQuarters içerisindeki Information özelliğinde yapılacak bir değişilik sonrası devreye giren Set bloğundan, NotifyOperators isimli metod çağırılmaktadır. Dikkat edileceği üzere söz konusu metod, IOperator tipinden olan List koleksiyonundaki tüm türevlerin Update metodunu çağırmakta ve çalışma zamanındaki HeadQuarters nesne referansını alt sınıflara göndermektedir. Dolayısıyla, bilgilendirme yaplılacak IOperator türevlerinin söz konusu Subject tipi içerisinde saklanması için ekleme ve çıkarma operasyonları da HeadQuarters sınıfına dahil edilmiştir.

Tabiki istemcinin (Console uygulamamız) asıl kullanacağı tip Subject'ten türeyen ConcreteSubject sınıfıdır (RedFleetBase). İstemci, bu tip üzerinde abonelerini bildirmektedir. Yani IOperator (Observer) arayüzünü implemente eden PlatoonOperator ve TankOperator nesne örneklerini...Kodun içerisinde RedFleetBase nesne örneği üzerinden yapılan her bir Information değişikliği sonrası, ne kadar tank ve piyade operatörü varsa bilgilendirilmektedir. Aynen aşağıdaki örnek çalışma zamanı çıktısında olduğu gibi.

![blg44_6.gif](images/blg44_6.gif)

Modelde dikkat edilmesi gereken noktalardan biriside, Subject ve Observer tiplerinden istenildiği kadar türetme yapılabilmesi ve bunların aralarında kuvvetli bir bağ ilişkisi olmamasıdır. Burada Subject tipinin kendi içerisinde soyut bir Observer tip koleksiyonu kullanmasınında büyük bir anlamı vardır. Böylece geldik bir tasarım kalıbımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Youtube Link](https://www.youtube.com/watch?v=X7NYBDeB2eM)

[Observer.rar (29,47 kb)](assets/Observer.rar)
