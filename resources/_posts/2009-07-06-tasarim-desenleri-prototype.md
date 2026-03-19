---
layout: post
title: "Tasarım Desenleri - Prototype"
date: 2009-07-06 20:35:00 +0300
description: "Yandaki resimde 118 WallyPower isimli tekneyi(yada suda giden uzay mekiği) görmektesiniz. Bu tekneyi hangi filmde gördüğünüzü bir hatırlamaya çalışın. Seyredenler The Island filmi olduğunu hemen bulacaktır..."
categories:
  - tasarim-kaliplari-design-patterns
tags:
  - tasarim-kaliplari-design-patterns
  - csharp
  - dotnet
---
# Tasarım Desenleri - Prototype
![blg43_1.jpg](/assets/images/2009/blg43_1.jpg)

Merhaba Arkadaşlar,

Yandaki resimde 118 WallyPower isimli tekneyi (yada suda giden uzay mekiği) görmektesiniz. Bu tekneyi hangi filmde gördüğünüzü bir hatırlamaya çalışın. Seyredenler The Island filmi olduğunu hemen bulacaktır. The Island filminde, Ewan McGregor'un gece rüyasında görüpte ustaca çizdiği (sanki çöpten adam çiziyormuşçasına) bu teknenin senaryodaki adı ise Renovatio (Yeniden doğmak-latince) idi.

Bu arada filmin ana fikri, ileride yarın öbür gün organ ihtiyacı olabilecek olan insanların birer klonunun üretildiği ve tutulduğu gizli bir yer altı şehri üstüne kuruluydu. Klon demişken var olan insanların birebir kopyasının üretildiği bir üs olduğunu belirteyim.

![Wink](/assets/images/2009/smiley-wink.gif)

E haliyle yeni klonların üretimi bir insanın doğum sürecine göre (filme gereği) çok daha kısa sürede olabilmektedir. Peki nerden geldik bu konuya...Aslında nesne yönelimli tarafta da, üretimi pahalı olan nesneler söz konusu olduğunda ve new operatörü ile oluşan maaliyetten kaçınmak istendiğinde, klon nesnelerin üretilmesi yolu tercih edilebilir.

Bu durum zaman içerisinde pek çok nesne yönelimli projede ortaya çıkınca haliyle kalıplaşmış ve bir desen haline gelmiştir. Creational tasarım kalıplarından olan Prototype deseni. Bu kalıp gerçek hayat uygulamalarının pek çok noktasında karşımıza çıkabilir. Söz gelimi bir oyun sahnesinin tekrar eden nesne üretimlerinde, oluşturulma maaliyetlerinin azaltılmasına etki edebilir (Öyleki oyun sahnesi içerisindeki sabit olan pek çok yapının nesnel olarak ifadesi sırasında bu maliyetler oldukça yükselmektedir.) Yada finansal veriler üzerine analiz gerçekleştiren bir sistemde, aynı veri kümesinden hareket edeceğimiz durumlarda, veriyi içeren nesne üretimlerinin maaliyetleri en aza indirgenebilir. Senaryolar çoğaltılabilir ancak özünde, nesne oluşturulma maaliyetleri vardır.

Dilerseniz durumu basit bir örnek üzerinde incelemeye gayret edelim. Az önce bahsettiğimiz senaryolardan oyun sahası problemini çok basit bir seviyede ele alıyor olacağız. Buna göre bir oyun sahasında yer alan kahraman ve mayınların birden fazla sayıda üretiminde aynı olan bazılarından yararlanıldığı düşünülmektedir. Sınıf diagramımızı aşağıdaki gibi tasarlayabiliriz.

![blg43_3.gif](/assets/images/2009/blg43_3.gif)

Görüldüğü gibi klonlama operasyonu bir abstract tip (veya interface) içerisinde bildirilmektedir. Bu abstract tip prototipimizdir. Prototype tipten türeyen Hero ve Mine sınıfları ise kullanılacak asıl nesne örneklerini modellemektedirler (Concrete Prototype). Buna göre oyun sahasını yöneten GameSceneManager sınıfı kendi içerisinde, Prototype tipten oluşan bir koleksiyonu kullanmaktadır. (Bu sayede farklı prototip tiplerinin sisteme kolayca eklenebilmesinin yoluda açılmıştır.![Wink](/assets/images/2009/smiley-wink.gif)) Peki Clone operasyonu nasıl gerçekleştirilecektir. Burada pek çok nesne yönelimli dilde yer alan bazı yardımcı metodlardan faydalanılabilir. Söz gelimi.Net tarafında MemberwiseClone fonksiyonu kullanılabilir. Buna göre uygulama kodlarımız aşağıdaki gibidir.

```csharp
using System;

namespace Prototype
{
    // Prototype Class
    abstract class GameScenePrototype
    {
        public abstract GameScenePrototype Clone();
    }

    // Concrete Prototype Class A
    class Hero
        :GameScenePrototype
    {
        public int Width { get; set; }
        public int Heigth { get; set; }
        public string Name { get; set; }
        public HeroType Type { get; set; }

        public Hero(int width,int heigth,string name,HeroType heroType)
        {
            Width = width;
            Heigth = heigth;
            Name = name;
            Type = heroType;
        }

        public override GameScenePrototype Clone()
        {
            return this.MemberwiseClone() as GameScenePrototype;
        }
    }

    // Concrete Prototype class B
    class Mine
        :GameScenePrototype
    {
        public double Gravity{ get; set; }
        public MineType Type { get; set; }

        public Mine(double gravity,MineType mineType)
        {
            Gravity = gravity;
            Type = mineType;
        }

        public override GameScenePrototype Clone()
        {
            return this.MemberwiseClone() as GameScenePrototype;
        }
    }

 // Prototype Manager class
    class GameSceneManager
    {
        public List<GameScenePrototype> GameObjects { get; set; }
        public GameSceneManager()
        {
            GameObjects = new List<GameScenePrototype>();
        }
    }

    #region Yardımcılar

    enum HeroType
    {
        Warrior,
        Employee,
        Archer
    }

    enum MineType
    {
        Gold,
        Silver,
        Bronze
    }

    #endregion

    class Program
    {
        static void Main()
        {
            GameSceneManager manager = new GameSceneManager();

            Hero hero1 = new Hero(10,20,"Bıkanyus", HeroType.Archer);
            manager.GameObjects.Add(hero1);
            Hero hero2 = new Hero(15, 35, "Wah!tupus", HeroType.Employee);
            manager.GameObjects.Add(hero2);

            Mine mine1 = new Mine(3, MineType.Gold);
            manager.GameObjects.Add(mine1);
            Mine mine2 = new Mine(5, MineType.Silver);
            manager.GameObjects.Add(mine2);

            // Var olan Mine ve Hero nesne örneklerinden klonlama yapılır
            manager.GameObjects.Add(mine2.Clone() as Mine);
            manager.GameObjects.Add(hero1.Clone() as Hero);
        }
    }
}
```

Prototype tip görevini üstlenen GameScenePrototype abstract sınıfı içerisinde tanımlanmış olan Clone metodu, kendi tipinden bir nesne referansını geriye döndürmek üzere planlanmıştır. Buna göre GameScenePrototype sınıfından türeyen Mine ve Hero isimli sınıflarında kendi içerisinde yer alan Clone metodlarında benzer davranışı göstermeleri gerekir. Burada MemberwiseClone metodundan yararlanılarak deep copy işleminin yapılması ve var olan nesnenin tüm içeriği ile birlikte klonlanması sağlanmaktadır. Tabiki MemberwiseClone metodu object tipinden bir referans döndürdüğü için açık bir şekilde (Explicit) tip dönüşümü yapılması gerekmektedir. Çalışma zamanında örnek olaran mine2 ve hero1 isimli nesne örneklerinin klonlanması işlemi ele alınmıştır. Böylece geldik bir bölümün daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Youtube Link](https://www.youtube.com/watch?v=s6-d2HrXhyg)

[Prototype.rar (22,60 kb)](/assets/files/2009/Prototype.rar)