---
layout: post
title: "Tasarım Desenleri - Mediator"
date: 2009-07-28 09:04:00 +0300
categories:
  - tasarim-kaliplari-design-patterns
tags:
  - design-patterns
  - oop
  - csharp
---
Yandaki resimde Zurich hava alanına ait bir görüntü yer almaktadır. Hava alanının ne kadar karmaşık olduğu aşikardır. Aslında yazımıza konu olarak Londra'daki Heathrow hava alanını dahil edecektim. Nitekim uzun zaman önce Discovery Channel'da izlediğim bir belgeselde, bir iniş ve birde kalkış pistiyle bu kadar işlek bir havalimanının ne kadar ustalıkla yönetildiği anlatılıyordu. Ancak yaptığım araştırmalar sonrası dünyadaki en iyi hava alanları arasında olmadığını gördüm.([http://www.worldairportawards.com/](http://www.worldairportawards.com/)). Her neyse.

![blg52_2.jpg](/assets/images/2009/blg52_2.jpg)

Konumuz aslında kimin daha iyi olduğu değil ama tüm hava alanları için ortak olan bir sorun. İnip kalkan ve hatta aynı havasahasına giren uçakların koordine edilmesi. Hiç çok işlek hava alanlarında kontrol kulesi olmadığını hayal ettiniz mi?

![Sealed](/assets/images/2009/smiley-sealed.gif)

Sanıyorumki aşağıdaki konuşmalar ile karşılaşabilirdik.

- AzonAir - 110: Ben sağdaki piste inmek üzere alçalıyorum arkadaşlar.

- CargoL TL 101: Hayır hayır oraya ben inecektim.

- AzonAir - 110: Eeee...Önce gelen kapar.

- Öz Hawai - 444: Savulunnnn!!! Ben o pistten kalkış yapıyorum.

- CargoL TL 101: Hangi pist, hangi pist?...

- Cazırt cuzurt, kraşş bummm...

Abartamaya gerek yok tabiki ama bu anektodunda bir manası var. Bir kontrol kulesi temel olarak tüm iniş kalkışları düzenler ve bu işi yaparken yukarıdaki gibi, uçakların birbirleri ile konuşmasına gerek kalmaz. Bir başka deyişle birbirleriyle etkileşimde olan uçakların tüm iletişimi, kontrol kulesi içerisinde hesaplanır ve işletilir. Dahada açık bir ifade ile kontrol kulesi aslında Mediator nesnesinin kendisidir. Mediator??? Hımmm..

Pekala konuyu biraz daha örneklemeye çalışalım.

![Wink](/assets/images/2009/smiley-wink.gif)

Bu kez bir network ağındaki kullanıcıları ve grupları göz önüne alalım. Kullanıcıların (Users) birden fazla gruba dahil olması muhtemeldir. Benzer şekilde bir grupta kendi içerisinde birden fazla kullanıcı barındırabilir. Yani kullanıcı ve gruplar arasında çoğa çok (Many to many) ilişki söz konusudur. Bu aktörler aslında birer nesne (Object) olarak düşünüldüklerinde, birbirlerine sıkı sıkıya bağlı olmaları (Tghtly Coupling), yönetimlerini zorlaştırmakla kalmaz, ileride yapılacak olan genişletmelerin çok fazla nesneyi etkilemesinede neden olur. Dolayısıyla aralarındaki bağı zayıflaştırmak (Loose Coupling) gerekir. Bu noktada veritabanı tasarımı ile uğraşanlar için sorunu çözmek son derece kolaydır. Nitekim bir ara tablo yardımıyla çoğa çok ilişkinin tesisi kolayca sağlanabilir. Diğer yandan Nesne Yönelimli (Object Oriented) tarafta, kullanıcı ve gruplar arasındaki iletişimi, onlardan soyutlayarak kendi içerisinde yönetecek olan bir ara nesneye ihtiyaç vardır. Kim...Mediator.

Anlaşılacağı üzere konumuz Behavioral tasarım kalıplarından olan Mediator desenidir. Bu desenin kullanım amacındaki odak noktası, nesne kümelerinin birbirleriyle nasıl haberleşebileceğini soyutlayan bir ara nesnenin kullanılmasıdır. Yazılım dünyasında bu konuya ilişkin verilebilecek en güzel örneklerden biriside Chat uygulamalarıdır. Bir Chat uygulamasına dahil olan katılımcıların her birinin birbirleriyle iletişim kurarak konuşması, zaman içerisinde ağ yükünü arttıracak ve yönetilemez hale getirecektir.

Üstelik performans açısından da son derece kötü bir yaklaşımdır. Bunun yerine katılımcılar arasındaki iletişimin yönetimini sağlayacak bir merkezin olması önerilir. Böylece, katılımcılar hiç bir şekilde birbirlerinin nesnelerine istemeden müdahele edemez veya karmaşık hesaplamalar, karar mekanizmaları ile karşı karşıya kalamazlar. Katılımcılar isteklerini Mediator nesneye iletirler ve sonuçlardan haberdar edilirler. Chat uygulaması göz önüne alındığında Mediator aslında mesajlaşma işlemini üstlenen sunucu uygulama olarak düşünülebilir.

Gelin UML şemamıza bakalım ve arkasından geliştireceğimiz basit bir örnek yardımıyla konuyu irdelemeye çalışalım.

![blg52_uml.gif](/assets/images/2009/blg52_uml.gif)

Şemadanda görüldüğü üzere Colleague'den Mediator'a doğru ve Concrete Mediator'dan, Concrete Colleague nesnelerine doğru tek yönlü ilişkiler (Asscoiation) söz konusdur. Yani okun solunda yer alan nesneler, okun ucundaki nesneleri ve izin verilen üyelerini kullanmaktadır. UML şemamızda yer alan nesnelerin ne işe yaradıklarını daha kolay kavrayabilmek amacıyla bir örnek üzerinden ilerleyebiliz. İşte sınıf diagramı ve kodlarımız.

![blg52_3.gif](/assets/images/2009/blg52_3.gif)

```csharp
using System;
using System.Collections.Generic;
using System.Threading;

namespace MediatorPattern
{
    // Mediator
    interface IAirportControl
    {
        void Register(Airline airLine);
        void SuggestWay(string fligthNumber, string way);
    }

    // Concrete Mediator
    class IstanbulControl
        :IAirportControl
    {
        // Concrete Colleague nesne örnekleri bu koleksiyonda depolanmaktadır.
        private Dictionary<string, Airline> _planes;

        public IstanbulControl()
        {
            _planes = new Dictionary<string, Airline>();
        }

        #region IAirportControl Members

        // Kontrol kulesine çevredeki uçakların kayıt olması için Register metodu kullanılır. Bu metod parametre olarak Colleague' den türeyen her hangibir Concrete Colleague nesne örnepğini alabilir.
        public void Register(Airline airLine)
        {
            if (!_planes.ContainsValue(airLine))
                _planes[airLine.FlightNumber] = airLine;

            // Hava yolu şirketine ait uçağın, kuleden yeni rota talep edebilmesi için, Concrete Colleague nesne örneğinin, Mediator referansının bildirilmesi gerekir.
            airLine.Airport = this;
        }

        // Concrete Colleague nesne örneklerinin yeni rota talep ederken kullandıkları metod. Bu metod o anki koşullar gereği sakladığı diğer uçakların konum bilgilerinden yararlanıp bir takım sonuçlara varmaktadır. Bu sayede n tane kombinasyonun, her bir uçak tarafından ele alınması yerine, tüm bu kombinasyonlar daha az sayıya indirgenerek Mediator içerisinde değerlendirilebilmektedir.
        public void SuggestWay(string fligthNumber, string way)
        {
            // TODO: Diğer uçakların konumlarına bakılarak flightNumber için yeni bir rota önerilir. Gerekirse diğer uçaklarada farklı rotalar önerilebilir.
            
            // Sembolik olarak yeni bir rota belirleniyor. Bilgilendirme rotayı talep eden Concrete Colleague nesne örneğinin GetWay metoduna yapılan çağrı ile gerçekleştiriliyor.
            Thread.Sleep(250);
            Random rnd = new Random();
            _planes[fligthNumber].GetWay(String.Format("{0}:{1}E;{2}:{3}W", rnd.Next(1, 100).ToString(), rnd.Next(1, 100).ToString(), rnd.Next(1, 100).ToString(), rnd.Next(1, 100).ToString()));
        }

        #endregion
    }

    // Colleague
    abstract class Airline
    {
        public IAirportControl Airport { get; set; }
        public string FlightNumber { get; set; }
        public string From { get; set; }

        // Mediator' den yani kuleden yeni bir rota talep ederken kullanılan metod.
        public void RequestNewWay(string myWay)
        {
            // Çağrı dikkat edileceği üzere Mediator tipine ait nesne referansına doğru yapılmaktadır. Peki bu referansı nerede verdik. Bknz Register metodu. :)
            Airport.SuggestWay(FlightNumber, myWay);
        }

        // Mediator tipinin, çağırıda bulunacağı GetWay metodu. Bu metodun parametre içeriği, kuleden(Concrete Mediator) üzerinden gelmektedir.
        public virtual void GetWay(string messageFromAirport)
        {
            Console.WriteLine("{0} rotasına yönelmemiz gerekmektedir.", messageFromAirport);
        }
    }

    // Concrete Colleague
    class OzHawaii
        :Airline
    {
        public override void GetWay(string messageFromAirport)
        {
            Console.WriteLine("Oz Hawaii, Uçuş {0} : ",FlightNumber);
            base.GetWay(messageFromAirport);
        }
    }

    // Concrete Colleague
    class ZorluYol
        : Airline
    {
        public override void GetWay(string messageFromAirport)
        {
            Console.WriteLine("ZorluYol, Uçuş {0} : ", FlightNumber);
            base.GetWay(messageFromAirport);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Kule nesnesi örneklenir(Concrete Mediator)
            IstanbulControl istanbulKule = new IstanbulControl();

            // Kuleden hizmet alacak tüm uçakların kendisini kuleye bildirmesi gerekmektedir. Bu nedenle uçaklar örneklendikten sonra Concrete Mediator tipine Register metedo yardımıyla kayıt olurlar.
            OzHawaii oh101 = new OzHawaii { Airport = istanbulKule, FlightNumber = "oh101", From="Hawai" };
            istanbulKule.Register(oh101);
            OzHawaii oh132 = new OzHawaii { Airport = istanbulKule, FlightNumber = "oh132", From="Roma" };
            istanbulKule.Register(oh132);
            ZorluYol zy99 = new ZorluYol { Airport = istanbulKule, FlightNumber = "zy99", From = "Antarktika" };
            istanbulKule.Register(zy99);

            // Uçaklar yeni rotalarını talep ederler.
            zy99.RequestNewWay("34:43E;41:41W");

            oh101.RequestNewWay("34:43E;41:41W");
        }
    }
}
```

Her ne kadar bir chat uygulaması yapmış olmasakta (ki dofactory.com'da Chat uygulaması örneğini bulabilirsiniz.) örneğimizdeki temel amacımız konumuza giriş yaptığımız kontrol kulesi senaryosunu simule edebilmektir. İlk etapta şunu rahatlıkla itiraf edebiliriz ki, Mediator deseni aslında uygulanması zor kalıplardan birisidir.

![Undecided](/assets/images/2009/smiley-undecided.gif)

Örneğimizde meslektaş (Colleague) nesnelerimiz belirli hava yollarını işaret etmektedir. Örnek olarak OzHawaii ve ZorluYol isimli sınıflar, Concrete Colleague sınıflarımızdır. Bu havayolu şirketlerine ait uçakların inişleri ve kalkışları için yeni rotaları bulmak amacıyla birbirleri ile konuşmaları yerine bu işi Mediator nesnesi içerisinde yer alan basit bir fonksiyon üstlenmektedir. Örneğe göre hava yolu şirketleri kuleden, yaklaşma halindeyeken veya kalkıştan önce, yeni rota talebinde bulunabilirler. Bunun için Colleague tipi olan AirLine içerisindeki RequestNewWay metodu kullanılır. Bu metod ise aslında, Concrete Mediator tipi içerisinde yer alan SuggestWay isimli bir foksiyonu çağırmaktadır. Dikkat edileceği üzere bu metod içerisinde, Mediator nesne örneğine abone olan tüm hava yolu şirketi uçaklarının o anki rota, yükseklik ve diğer bilgilerinden yararlanılarak, parametre olarak gelen Concrete Colleague nesne örneğine bilgilendirme yapılmaktadır.

Bu bilgilendirmenin yapılabilmesi için tahmin edileceği üzere, Concrete Mediator tipinin, Concrete Colleague tiplerine erişebiliyor olması gerekmektedir. Bu sayede, Concrete Colleague tipleri içerisindeki GetWay metodları, Mediator nesne örneği içerisinden çağrılabilir. Bu da zaten UML şemasında yer alan, Concrete Mediator'den, Concrete Colleague nesnelerine olan tek yönlü ilişkiyi (Association) açıklamaktadır. Çok doğal olarak Concrete Mediator tipinin, hangi nesne kümelerini değerlendireceğini bilmesi gerekmektedir. Bu amaçla örneğimizde generic bir Dictionary koleksiyonundan yararlanılmaktadır. Peki, Concrete Colleague nesne örnekleri, Concrete Mediator nesne örneklerine nasıl bildirilecektir? İşte bu noktada Register isimli metod devreye girmektedir. Buda UML şemamızda, Colleague'den Medaitor'e doğru olan tek yönlü ilişkiyi (Association) açıklamaktadır. Nitekim Register metodu parametre olarak AirPort tipinden (Concrete Colleague) nesne örnekleri almaktadır. Örneğimizi çalıştırdığımızda aşağıdaki ekran görüntüsündekine benzer sonuçları elde ederiz.

![blg52_4.gif](/assets/images/2009/blg52_4.gif)

Özet olarak herhangibir havayoluna ait bir uçak, İstanbul kulesine yaklaştığında kendisine yeni bir rota talep ederken diğer uçaklar ile haberleşmek ve onların konumlarına göre hesaplamalar yaparak bir yön tayin etmek zorunda değildir. Tüm uçaklar bir birlerinden ayrıştırılmış ve yönlerini belirlemek üzere kullanılması gereken algoritmalar Mediator tipi içerisine kapsüllenmiştir. Biraz karışık bir desen implemantasyonu olmasına rağmen faydalı olduğunu umuyorum. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[MediatorPattern.rar (24,85 kb)](/assets/files/2009/MediatorPattern.rar)
