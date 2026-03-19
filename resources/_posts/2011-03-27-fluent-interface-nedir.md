---
layout: post
title: "Fluent Interface Nedir?"
date: 2011-03-27 05:15:00 +0300
description: "Bu günkü yazımızda ilk olarak Martin Fowler ve Eric Evans tarafından tanımlanan Fluent Interface konusunu irdelemeye çalışıyor olacağız. Aslında kelime anlamlarından yola çıkarsak okunabilir, açık, net arayüz tiplerinden bahsettiğimizi düşünebiliriz. Ancak bu şekilde söz konusu kavrama biraz haksızlık etmiş oluruz. Fluent Interface esas itibariyle daha okunabilir kodlama açısından önem arz eden ve uygulanması sırasında metod zincirlerinden yararlananan bir yaklaşım sunmaktadır. Şimdi ne demek istediğimizi ben de ifade edemedim aslında Confused smile Gelin basit bir örnek ile konuyu didiklemeye başlayalım.Bu amaçla aşağıdaki kod içeriğini göz önüne alabiliriz."
categories:
  - csharp
tags:
  - csharp
  - http
  - generics
---
# Fluent Interface Nedir?
[![blg236_Giris](/assets/images/2011/blg236_Giris_thumb.jpg)](/assets/images/2011/blg236_Giris.jpg) Merhaba Arkadaşlar,

Yazılımcı olarak bizlerin zaman içerisindeki gelişimimiz/ilerleyişimiz açısından takip etmemiz gereken önemli kişiler olduğu aşikardır. Söz gelimi çevik süreç prensiplerine ait manifestoyu hazırlayanlar arasında yer alan Martin Fowler gibi. [Martin Fowler](http://martinfowler.com/) bana göre yazılım mühendisliğinin uç noktalarında yaşayan bir bilim insanıdır. Bilim insanı diyorum nitekim çalıştığı şirkette Chief Scientist pozisyonunda görev almaktadır

![Smile](/assets/images/2011/wlEmoticon-smile.png)

Bu güne kadar yayımlamış olduğu çok değerli kitaplar bulunmaktadır. Hatta son çıkarttığı ve merakla bekleyip okumaya başladığım [Domain-Specific Languages (Addison-Wesley Signature Series)](http://www.amazon.com/Domain-Specific-Languages-Addison-Wesley-Signature-Martin/dp/0321712943/ref=sr_1_1?ie=UTF8&qid=1288249840&sr=8-1) isimli kitap en büyük favorilerim arasında yer almakta.

> Bundan önceki favori kitaplarım ise [Clean Code: A Handbook of Agile Software Craftsmanship](http://www.amazon.com/Clean-Code-Handbook-Software-Craftsmanship/dp/0132350882/ref=sr_1_10?ie=UTF8&qid=1288249840&sr=8-10) ve [Agile Principles, Patterns, and Practices in C#](http://www.amazon.com/Agile-Principles-Patterns-Practices-C/dp/0131857258/ref=sr_1_14?ie=UTF8&qid=1288256328&sr=8-14) dir

Pek Martin Fowler’ ın kulaklarını niye bu kadar çok çınlatıyoruz

![Thinking smile](/assets/images/2011/wlEmoticon-thinkingsmile_1.png)

Bu günkü yazımızda ilk olarak Martin Fowler ve Eric Evans tarafından tanımlanan Fluent Interface konusunu irdelemeye çalışıyor olacağız. Aslında kelime anlamlarından yola çıkarsak okunabilir, açık, net arayüz tiplerinden bahsettiğimizi düşünebiliriz. Ancak bu şekilde söz konusu kavrama biraz haksızlık etmiş oluruz. Fluent Interface esas itibariyle daha okunabilir kodlama açısından önem arz eden ve uygulanması sırasında metod zincirlerinden yararlananan bir yaklaşım sunmaktadır. Şimdi ne demek istediğimizi ben de ifade edemedim aslında

![Confused smile](/assets/images/2011/wlEmoticon-confusedsmile_2.png)

Gelin basit bir örnek ile konuyu didiklemeye başlayalım.Bu amaçla aşağıdaki kod içeriğini göz önüne alabiliriz.

[![blg236_ClassDiagram1](/assets/images/2011/blg236_ClassDiagram1_thumb.gif)](/assets/images/2011/blg236_ClassDiagram1.gif)

```csharp
namespace UsingFluentInterface 
{ 
    struct Location 
    { 
        public int X { get; set; } 
        public int Y { get; set; } 
    }

    enum Level 
    { 
        Low, 
        High, 
        Advanced, 
        Expert 
    }

    interface IPlayerSpec 
    { 
        string NickName{  set; } 
        Level Level { set; } 
        double TotalPoint { set; } 
        string Picture { set; } 
        Location Location { set; } 
    }

    class Player 
        : IPlayerSpec 
    { 
        string _nickName; 
        Level _level; 
        double _totalPoint; 
        string _picture; 
        Location _location;

        #region IPlayerSpec Members

        public string NickName 
        { 
            set { _nickName = value; } 
        }

        public Level Level 
        { 
            set { _level = value; } 
        }

        public double TotalPoint 
        { 
            set { _totalPoint = value; } 
        }

        public string Picture 
        { 
            set { _picture = value; } 
        }

        public Location Location 
        { 
            set { _location = value; } 
        }

        #endregion 
    }

    class Program 
    { 
        static void Main(string[] args) 
        { 
            IPlayerSpec marti = new Player();

            marti.NickName = "Makflay"; 
            marti.Level = Level.Advanced; 
            marti.Location = new Location { X = 12, Y = 19 }; 
            marti.Picture = "Monster.jpg"; 
            marti.TotalPoint = 198.45;

        } 
    } 
}
```

Örnek kod parçasında IPlayerSpec isimli bir arayüz (Interface) tanımının kullanıldığı görülmektedir. Bu tanımlaya göre söz konusu arayüzü implemente eden herhangibir sınıfın uygulaması gereken bazı özellikler (Properties) vardır. Player isimli sınıf, IPlayerSpec isimli arayüzü implemente etmektedir ve buna göre NickName, Level, Location, Picture ve TotalPoint isimli özelliklerinin her birini ele almalıdır.

Aslında buraya kadar ki kod parçasında olağan dışı bir durum yer almadığını ifade edebiliriz. Standart olarak interface tanımlaması ve bunu uygulayan bir sınıf tasarımı söz konusudur. Main metodu içerisinde ise dikkat edileceği üzere interface tipinin çok biçimli yapısı göz önüne alınarak marti isimli değişkene, Player tipinden bir nesne örneğinin oluşturulup atandığı görülmektedir. Güzel…Şimdi örneğimizi biraz daha değiştiriyor olacağız. İşte yeni kod içeriğimiz.

[![blg236_ClassDiagram2](/assets/images/2011/blg236_ClassDiagram2_thumb.gif)](/assets/images/2011/blg236_ClassDiagram2.gif)

```csharp
namespace UsingFluentInterface 
{ 
    struct Location 
    { 
        public int X { get; set; } 
        public int Y { get; set; } 
    }

    enum Level 
    { 
        Low, 
        High, 
        Advanced, 
        Expert 
    }

    #region Fluent Sample

    interface IPlayerSpecV2 
    { 
        IPlayerSpecV2 SetNickName(string nickName); 
        IPlayerSpecV2 SetLevel(Level level); 
        IPlayerSpecV2 SetTotalPoint(double totalPoint); 
        IPlayerSpecV2 SetPicture(string picture); 
        IPlayerSpecV2 SetLocation(Location location); 
    }

    class PlayerV2 
        :IPlayerSpecV2 
    { 
        string _nickName; 
        Level _level; 
        double _totalPoint; 
        string _picture; 
        Location _location;

        #region IPlayerSpecV2 Members

        public IPlayerSpecV2 SetNickName(string nickName) 
        { 
            this._nickName = nickName; 
            return this; 
        }

        public IPlayerSpecV2 SetLevel(Level level) 
        { 
            this._level = level; 
            return this; 
        }

        public IPlayerSpecV2 SetTotalPoint(double totalPoint) 
        { 
            this._totalPoint = totalPoint; 
            return this; 
        }

        public IPlayerSpecV2 SetPicture(string picture) 
        { 
            this._picture = picture; 
            return this; 
        }

        public IPlayerSpecV2 SetLocation(Location location) 
        { 
            this._location = location; 
            return this; 
        }

        #endregion 
    }

    #endregion

    class Program 
    { 
        static void Main(string[] args) 
        { 
            IPlayerSpecV2 kanon = new PlayerV2() 
                .SetNickName("Konan") 
                .SetLevel(Level.Expert) 
                .SetLocation(new Location { X = 15, Y = 45 }) 
                .SetPicture("Snoopy.gif") 
                .SetTotalPoint(45); 
        } 
    } 
}
```

Vay…Vay…Vay…Vayyyy!!!

![Disappointed smile](/assets/images/2011/wlEmoticon-disappointedsmile.png)

Bu sefer çok daha ilginç bir kod parçası ile karşı karşıyayız. İlk olarak IPlayerSpecV2 isimli arayüz tipi içerisine bakmamızda yarar olacağı kanısındayım. Görüldüğü üzere burada tanımlı olan özellikler yine IPlayerSpecV2 arayüz tipinin taşıyabileceği referansları döndürmektedir. Bir başka deyişle, IPlayerSpecV2 arayüzünü uygulayan sınıfa ait nesne örneklerinin döndürüldüğünü ifade edebiliriz. Bu durumda PlayerV2 sınıfının içeriği de önem kazanmaktadır.

Nitekim Interface implementasyonu sonucu dikkat edileceği üzere Set ön eki ile başlayan her metod, iç değer atamalarında this anahtar kelimesini kullanmaktadır. Buna göre çalışma zamanında o anki PlayerV2 nesne örneğinin kullanılması söz konusudur. Ayrıca her Set… metodunun sonunda return this; ifadesinin kullanıldığına da dikkat edilmelidir. Peki tüm bunlar ne anlama geliyor? Aslında tüm bunların ne anlama geldiğini anlamak için Main metodu içerisinde yer alan kod parçasını göz önüne almamız yeterli olacaktır.

[![blg236_CodeView](/assets/images/2011/blg236_CodeView_thumb.gif)](/assets/images/2011/blg236_CodeView.gif)

Mutlaka dikkatinizi çekmiştir. Set… ön eki ile başlayan metodlardan hangisini kullanırsak kullanalım arkasından yine IPlayerSpecV2 üzerinden tanımlanmış olan metodlardan birisine erişilebilmektedir

![Open-mouthed smile](/assets/images/2011/wlEmoticon-openmouthedsmile_6.png)

İşte size bir metod zinciri. Bu tip bir kullanım son derece doğaldır, nitekim Set… metodları geriye IPlayerSpecV2 arayüzünün taşıyabileceği referansları döndürmek üzere kodlanmıştır. Bu sayede her hangibir Set… metod çağrısı ile başlatılan çalışma zamanı Context’ inin alt metod çağrılarına da taşınabilmesi kolaylaşmaktadır. Ayrıca ilk başlangıçta üretilen ile son kullanılan Context içeriğinin aynı ve tek olması da garanti altına alınmaktadır

![Winking smile](/assets/images/2011/wlEmoticon-winkingsmile_6.png)

Şimdi dilerseniz başka bir örnek daha ele alalım. İşte kodlarımız.

[![blg236_RealExample](/assets/images/2011/blg236_RealExample_thumb.gif)](/assets/images/2011/blg236_RealExample.gif)

```csharp
using System.Collections.Generic; 
using System; 

namespace UsingFluentInterface 
{ 
    #region Real Sample

    class Ability 
    { 
        public string Name { get; set; } 
        public void Apply() 
        { 
            Console.WriteLine("{0}",Name); 
        } 
    }

    interface IPlayerSpecV3 
    { 
        IPlayerSpecV3 SetName(string nickName); 
        IPlayerSpecV3 SetAbility(Ability ability); 
        IPlayerSpecV3 ApplyAbility(Ability ability); 
    }

    class PlayerV3 
        : IPlayerSpecV3 
    { 
        string _nickName; 
        List<Ability> _abilities = new List<Ability>();

        #region IPlayerSpecV3 Members

        public IPlayerSpecV3 SetName(string nickName) 
        { 
            this._nickName = nickName; 
            return this; 
        }

        public IPlayerSpecV3 SetAbility(Ability ability) 
        { 
            if (!this._abilities.Contains(ability)) 
                this._abilities.Add(ability); 
            return this; 
        }

        public IPlayerSpecV3 ApplyAbility(Ability abl) 
        { 
            var ability=this._abilities.Find(t => t == abl); 
            ability.Apply(); 
            return this;                           
        }

        #endregion 
    }

    #endregion

    class Program 
    { 
        static void Main(string[] args) 
        { 
            Ability shoot=new Ability{Name="Shoot...Shoot...Shoot"}; 
            Ability moveLeft=new Ability{Name="Move to the Left"}; 
            Ability moveBack = new Ability { Name = "Move to the Back" }; 
            Ability lookBack=new Ability{Name="I sense danger may be near!"};

            IPlayerSpecV3 tank = new PlayerV3() 
                .SetAbility(shoot) 
                .ApplyAbility(shoot) 
                .SetAbility(moveLeft) 
                .SetAbility(lookBack) 
                .ApplyAbility(moveLeft) 
                .ApplyAbility(lookBack) 
                .SetAbility(moveBack) 
                .ApplyAbility(moveBack) 
                .ApplyAbility(shoot);    
        } 
    } 
}
```

Bu sefer IPlayerSpecV3 arayüzü içerisinde SetAbility ve ApplyAbility isimli iki metod bildirimi yapıldığı görülmektedir. Fluent Interface modeline göre bu metodlar geriye yine IPlayerSpecV3 referansı döndürmektedir. Örnekte ilginç olan nokta ise şudur; Esas istibariyle PlayerV3, bir oyuncu için eklenebilecek n sayıda farklı kabiliyeti ele alabilir ve uygulayabilir yapıda düşünülmüş ve tasarlanmıştır. Söz gelimi çalışma zamanında örneklenen tank değişkenine, ateş etmek, sola dönmek, geriye bakmak, geriye hareket etmek gibi yetenekler eklenmektedir. Ayrıca bu yeteneklerin istenildiği zaman uygulanması da sağlanmaktadır.

Bu noktada PlayerV3 nesne örneğini üretirken yapılan metod çağrılarına dikkat etmenizi öneririm. SetAbility ve ApplyAbility metodlarının aynı ifade içerisinde, karmaşık sırada çağırılabildiği görülmektedir. Bu tipik olarak bir metod zinciridir ve aynı çalışma zamanı Context’ ini kullanarak tankımza farklı kabiliyetlerin eklenmesi ve yürütülmesi işlemlerinin gerçekleştirilebilmesini sağlamaktadır. Örneğimize ait çalışma zamanı görüntüsü aşağıdakine benzer olacaktır.

[![blg236_Runtime](/assets/images/2011/blg236_Runtime_thumb.gif)](/assets/images/2011/blg236_Runtime.gif)

Sanıyorum ki bu son örnek Fluent Interface kullanımı konusunda bizlere daha fazla fikir verebilmiştir. Ancak son olarak işin içerisine bir de Extension metodları katarak işin kaymağını da hazırlayabiliriz

![Winking smile](/assets/images/2011/wlEmoticon-winkingsmile_6.png)

Son örneğimizde PlayerV3 tipinden tek bir nesne örneği için metod zincirini oluşturduğumuzu görüyoruz. Oysaki n sayıda tank için de birlikte hareket etmelerini sağlayacak biçimde metod zincirleri kurmak ve kullanmak isteyebiliriz. Bu noktada elimizde bir koleksiyon yapısı olduğu düşünülebilir (IEnumerable gibi) Söz konusu koleksiyon yapısı için eklenecek olan bir Extension ile de istediğimiz fonksiyonelliği kazanma şansına sahip olabiliriz. Gelin bu amaçla aşağıdaki gibi bir kod parçası geliştirelim.

[![blg236_ClassDiagram4](/assets/images/2011/blg236_ClassDiagram4_thumb.gif)](/assets/images/2011/blg236_ClassDiagram4.gif)

```csharp
using System; 
using System.Collections.Generic;

namespace UsingFluentInterface 
{ 
    #region Real Sample

    public class Ability 
    { 
        public string Name { get; set; } 
        public void Apply() 
        { 
            Console.WriteLine("{0}",Name); 
        } 
    }

    public interface IPlayerSpecV3 
    { 
        IPlayerSpecV3 SetName(string nickName); 
        IPlayerSpecV3 SetAbility(Ability ability); 
        IPlayerSpecV3 ApplyAbility(Ability ability); 
    }

    public class PlayerV3 
        : IPlayerSpecV3 
    { 
        public string PlayerName { get; set; } 
        string _nickName; 
        List<Ability> _abilities = new List<Ability>();

        #region IPlayerSpecV3 Members

        public IPlayerSpecV3 SetName(string nickName) 
        { 
            this._nickName = nickName; 
            return this; 
        }

        public IPlayerSpecV3 SetAbility(Ability ability) 
        { 
            if (!this._abilities.Contains(ability)) 
                this._abilities.Add(ability); 
            return this; 
        }

        public IPlayerSpecV3 ApplyAbility(Ability abl) 
        { 
            var ability=this._abilities.Find(t => t == abl); 
            ability.Apply(); 
            return this;                           
        }

        #endregion 
    }

    #endregion

    #region Extensions for Fluent Interface

    public static class EnumerableExtensions 
    { 
        public static IEnumerable<IPlayerSpecV3> SetAbility(this IEnumerable<IPlayerSpecV3> players,Ability ability) 
        { 
            foreach (IPlayerSpecV3 player in players) 
            { 
                player.SetAbility(ability); 
            } 
            return players; 
        }

        public static IEnumerable<IPlayerSpecV3> ApplyAbility(this IEnumerable<IPlayerSpecV3> players,Ability ability) 
        { 
            foreach (IPlayerSpecV3 player in players) 
            { 
                player.ApplyAbility(ability); 
            } 
            return players; 
        } 
    }

    #endregion

    class Program 
    { 
        static void Main(string[] args) 
        { 
            Ability shoot=new Ability{Name="Shoot...Shoot...Shoot"}; 
            Ability moveLeft=new Ability{Name="Move to the Left"}; 
            Ability moveBack = new Ability { Name = "Move to the Back" }; 
            Ability lookBack=new Ability{Name="I sense danger may be near!"}; 

            List<IPlayerSpecV3> aTeam = new List<IPlayerSpecV3> 
            { 
                new PlayerV3{ PlayerName="Red Leader"}, 
                new PlayerV3{PlayerName="Orange 1"}, 
                new PlayerV3{PlayerName="Victor Echo 2"} 
            };

            aTeam 
                .SetAbility(shoot) 
                .SetAbility(moveLeft) 
                .SetAbility(moveBack) 
                .SetAbility(shoot) 
                .ApplyAbility(moveBack) 
                .ApplyAbility(moveLeft) 
                .ApplyAbility(shoot); 
        } 
    } 
}
```

Bu örnekte IEnumerable arayüzü (Interface) için geliştirilmiş iki Extension metod kullanılmaktadır. Bu genişletme metodları tahmin edeceğiniz üzere IPlayerSpecV3 arayüzü tarafından taşınabilen (ki örneğimizde bu PlayerV3 sınıfına ait nesne örnekleridir) referanslar üzerinde SetAbility ve ApplyAbility fonksiyonelliklerinin uygulanabilmesini sağlamaktadır. Bu sayede PlayerV3 tipinden nesne örneklerinden oluşan bir koleksiyon üzerinde toplu olarak Fluent Interface etkisi gerçekleştirilebilir.

Söz gelimi Main metodu içerisindeki kod parçalarına dikkat edildiğinde aTeam isimli nesne örneği üzerinden SetAbility ve ApplyAbility çağrıları gerçekleştirilmiş ve sonuç olarak Red Leader, Orange 1 ve Victor Echo 2 isimli tanklar için ortak yürütme işlemleri icra ettirilmiştir. Aşağıdaki örnek ekran çıktısında bu durum daha net bir şekilde görülebilmektedir.

[![blg236_Runtime2](/assets/images/2011/blg236_Runtime2_thumb.gif)](/assets/images/2011/blg236_Runtime2.gif)

Sanıyorum ki Fluent Interface kullanımı şimdi daha da bir anlam kazanmış oldu

![Winking smile](/assets/images/2011/wlEmoticon-winkingsmile_6.png)

İlerleyen zamanlarda bu tip uç prensipleri incelemeye devam etmeye çalışacağım. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[UsingFluentInterface.rar (30,94 kb)](/assets/files/2011/UsingFluentInterface.rar)