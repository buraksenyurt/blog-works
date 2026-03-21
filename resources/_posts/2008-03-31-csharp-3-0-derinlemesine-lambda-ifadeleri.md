---
layout: post
title: "C# 3.0 : Derinlemesine Lambda İfadeleri"
date: 2008-03-31 06:00:00 +0300
categories:
  - csharp-3-0
tags:
  - csharp
  - lambda-operator
---
C# programlama dilinin 3ncü versiyonu ile birlikte gelen önemli yeniliklerden biriside lambda (=>) operatörüdür. Bu operatörün kullanıldığı ifadeler yardımıyla temsilci (delegate) oluşturulması, kod bloğunun yazılması, sonuçların alınması ve tip tahmini (Type Inference) gibi işlemlerin tek seferde gerçekleştirilmesi mümkündür. Bu sebepten dolayı LINQ (Language INtegrated Query) sorgularında yer alan genişletme metodlarında (Extension Methods) büyük öneme sahiptir. Ne varki lambda operatörünü kavramak için ona olan ihtiyacın nereden doğdunu bilmek ve nasıl bu operatöre ulaşıldığını anlamak gerekmektedir. En iyi başlangıç noktası elbetteki C# dilinin ilk versiyonudur. Bu yazımızda lambda operatörünün getirdiği avantajları görmeye çalışırken derinlemesinede inceliyor olacağız.

Herşeyden önce C# 1.0 versiyonunda kullanıcı tanımlı bir tipe ait koleksiyonlar üzerinde bazı sorgulamalar yapmak istediğimizi düşünelim. C# 1.0 versiyonunda generic mimari kavramı yoktur. Bu sebepten generic olarak türden bağımsız ve.Net Framework içerisinde önceden tanımlanmış olan koleksiyonlar bulunmamaktadır. Bunun yerine elemanları her zaman object türünden olan ArrayList, Stack, Queue gibi Collection bazlı koleksiyonlar ile Hashtable ve SortedList gibi Dictionary bazlı koleksiyonlar mevcuttur. Eğer sadece bizim istediğimiz tip ile çalışacak kuvvetle türlendirilmiş bir koleksiyon (Strongly Typed Collection) kullanmak istersek CollectionBase veya DictionaryBase abstract sınıflarından türetme yolunu tercih edebiliriz. Böylece sadece istenilen tipler ile çalışacak bir koleksiyonumuz olur.(Lakin bu koleksiyon tip güvenli-type safety olmasına rağmen gereksiz boxing ve unboxing işlemlerini engellemez.)

Şu an için asıl amaç bu koleksiyon içerisinde yer alan tipler üzerinde farklı sorgular çalıştırabilmektir. Söz gelimi bir personelin bilgirini tanımlayan sınıfa ait bir koleksiyon içerisinden, çalışanın maaşına, adına, giriş tarihine göre sorgular yaparak alt koleksiyonların çekilmesini sağlayacak fonksiyonelliklerin olması istenebilir. Hatta bu metodarın sayısının arttırılmasınada olanak verecek şekilde esnek bir yapının geliştirilmesi istenebilir. Dolayısıyla alt koleksiyonları elde edebilmek için geliştirilen ortak bir metodun kullanacağı koşulsal fonksiyonelliklerin işaret edilebilmesi son derece yararlı olur. İşte bu noktada temsilciler (Delegates) devreye girmektedir. Bu cümleler ile tam olarak neye sebebiyet verdiğimizi görmek üzere C# 1.0 dilinin yeteneklerinin kullanıldığı aşağıdaki program kodu göz önüne alınabilir.

![mk247_1.gif](/assets/images/2008/mk247_1.gif)

Kod içeriği;

```csharp
using System;
using System.Collections;

namespace DotNet1Deyken
{
    enum Departman
    {
        BilgiIslem
        ,Yazilim
        ,Muhasebe
        ,InsanKaynaklari
        ,GenelMudurluk
    }

    class Personel
    {
        int _id;
        string _ad;
        string _soyad;
        Departman _bolumu;
        double _maas;
        DateTime _girisTarihi;

        public DateTime GirisTarihi
        {
            get { return _girisTarihi; }
            set { _girisTarihi = value; }
        }

        public string Soyad
        {
            get { return _soyad; }
            set { _soyad = value; }
        }

        public double Maas
        {
            get { return _maas; }
            set { _maas = value; }
        }

        internal Departman Bolumu
        {
            get { return _bolumu; }
            set { _bolumu = value; }
        }

        public string Ad
        {
            get { return _ad; }
            set { _ad = value; }
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public Personel(int id, string ad, string soyad, Departman bolumu, double maas,DateTime girisTarihi)
        {
            Id = id;
            Ad = ad;
            Soyad = soyad;
            Bolumu = bolumu;
            Maas = maas;
            GirisTarihi = girisTarihi;
        }
        public override string ToString()
        {    
            return String.Format("{0} {1} {2} {3} {4} {5}", Id.ToString(), Ad, Soyad.ToUpper(), Bolumu.ToString(), Maas.ToString("C2"), GirisTarihi.ToShortDateString());
        }
    }

    class PersonelList
        : CollectionBase
    {
        public void Ekle(Personel prs)
        {
            List.Add(prs);
        }
        public void Cikart(Personel prs)
        {
            List.Remove(prs);
        }
    }

    delegate bool KontrolHandler(Personel p);

    class Program
    {
        static bool DepartmaniIKmi(Personel p)
        {
            return p.Bolumu == Departman.GenelMudurluk;
        }
        static bool AdininBasHarfiBmi(Personel prs)
        {
            return prs.Ad[0] == 'B';
        }
        static bool Maas1000Uzerindemi(Personel prs)
        {
            return prs.Maas > 1000;
        } 

        static PersonelList Bul(PersonelList liste,KontrolHandler handler)
        {
            PersonelList sonucListesi = new PersonelList();
            foreach (Personel prs in liste)
            {
                if (handler(prs))
                    sonucListesi.Ekle(prs);
            }
            return sonucListesi;
        }

        static void Listele(PersonelList liste)
        {
            foreach (Personel prs in liste)
                Console.WriteLine(prs.ToString());
            Console.WriteLine("");
        }

        static void Main(string[] args)
        {
            PersonelList calisanlar = new PersonelList();
    
            calisanlar.Ekle(new Personel(1000,"Mayk","Hemır", Departman.BilgiIslem,1050,new DateTime(1979,10,1)));
            calisanlar.Ekle(new Personel(1001,"Büyük","Başkan", Departman.GenelMudurluk,53000,new DateTime(1989,2,3)));
            calisanlar.Ekle(new Personel(1002,"EmSi","Hemmır", Departman.GenelMudurluk,13500,new DateTime(1990,2,4)));
            calisanlar.Ekle(new Personel(1003,"Tombul","Raydır", Departman.InsanKaynaklari,2250,new DateTime(1994,8,5)));
            calisanlar.Ekle(new Personel(1008,"Şirine","Şirin", Departman.BilgiIslem,900,new DateTime(1991,3,6)));
            calisanlar.Ekle(new Personel(1006,"Burak","Selim", Departman.InsanKaynaklari,2250,new DateTime(1976,7,3)));
            calisanlar.Ekle(new Personel(1004,"Osvaldo","Nartayyo", Departman.Muhasebe,3500,new DateTime(1975,6,3)));
            calisanlar.Ekle(new Personel(1005,"Higuin","Kim", Departman.Yazilim,1250,new DateTime(1974,4,2)));
            calisanlar.Ekle(new Personel(1007,"Karim","Cabbar", Departman.Yazilim,750,new DateTime(1975,2,7)));
            calisanlar.Ekle(new Personel(1011, "Billl", "Geytis", Departman.Yazilim, 650, new DateTime(1976, 3, 8)));

            // Departmanı Insan Kaynakları olanların bulunması
            PersonelList sonuclar1=Bul(calisanlar, new KontrolHandler(DepartmaniIKmi));
        
            // İsminin baş harfi B olanların bulunması
            PersonelList sonuclar2 = Bul(calisanlar, new KontrolHandler(AdininBasHarfiBmi));
        
            // Maaşı 1000 YTL üzerinde olanların bulunması
            PersonelList sonuclar3 = Bul(calisanlar, new KontrolHandler(Maas1000Uzerindemi));

            Listele(sonuclar1);
            Listele(sonuclar2);
            Listele(sonuclar3);
        }
    }
}
```

Öncelikli olarak bu uzun Console uygulaması kodlarında neler olduğuna bir bakalım. Personel isimli sınıf bir çalışanın Id'sini, adını, soyadını, işe giriş tarihini, maaşını, departmanını ve maaşını tutacak şekilde tanımlanmıştır. Bu sınıfa ait örneklerin içeriklerinin kolay bir şekilde string olarak alınabilmesi içinde ToString metodu Personel tipi içerisinde ezilmiştir (override). Personelin bölümü, Departman isimli bir enum sabiti ile belirtilmektedir. PersonelList isimli sınıf CollectionBase abstract sınıfından türetilmektedir. Bu nedenle Collection tabanlı bir koleksiyondur. Konunun kolay anlaşılabilmesi için sadece Ekle ve Cikart isimli iki fonksiyonelliğe sahiptir. Bu metodlar sadece Personel tipinden parametreler almaktadır. Buda zaten PersonelList isimli koleksiyonun tip güvenli (Type Safety) olmasını sağlamaktadır. Dikkatimiz çeken tiplerden biriside KontrolHandler isimli temsilcidir (delegate).

> Temsilcileri (delegates) metodları işaret edebilecek şekilde kullanılabilen.Net tipidir. Bir temsilci işaret edebileceği metodun parametrik yapısı ile dönüş tipinide belirtmektedir.

Söz konusu temsilci, Personel tipinden bir parametre alan ve geriye bool değer döndüren metodları işaret edecek şekilde tanımlanmıştır. Bu temsilcinin tek bir tasarım amacı vardır. Buna göre, bir Personel nesne örneğinin herhangibir şartı sağlayıp sağlamadığına dair true veya false değer döndürecek bir metodun işaret edilmesini sağlamaktadır. Peki neden böyle bir temsilciye ihtiyacımız vardır? Bu sorunun cevabını Bul isimli fonksiyon vermektedir.

```csharp
static PersonelList Bul(PersonelList liste,KontrolHandler handler)
{
    PersonelList sonucListesi = new PersonelList();
    foreach (Personel prs in liste)
    {
        if (handler(prs))
            sonucListesi.Ekle(prs);
    }
    return sonucListesi;
}
```

Dikkat edilecek olursa Bul metodu geriye PersonelList tipinden bir nesne örneği döndürmektedir. Bu nesne örneği metod içerisinde oluşturulmaktadır. Oluşturulma işlemi sırasında ise belirli bir şarta bakılmaktadır. Nitekim bu şartın ne olduğu belli değildir. Ancak şartın sonucunun alınmasını sağlayan metodu işaret edebilecek KontrolHandler tipinden bir temsilci, fonksiyona parametre olarak gelmektedir. Temsilci nesne örneği çalışma zamanında (runtime) ilgili fonksiyonu işaret edeceğinden, if ifadesi içerisindeki çağrı aslında o andaki Personel nesne örneği için koşul metoduna doğru yapılan bir yürütmeden başka bir şey değildir.

Artık tek yapılması gereken şartları sağlayacak metodların yazılması ve sonrasında ise Bul fonksiyonelliğinin kullanılarak ilgili sonuç kümesinin alınmasıdır. Örneğin IK departmanında çalışan personelin bulunabilmesi için DepartmanIKmi isimli bir metod geliştirilmiştir.

```csharp
static bool DepartmaniIKmi(Personel p)
{
    return p.Bolumu == Departman.GenelMudurluk;
}
```

Bu metod basitçe gelen Personel nesne örneğinin Bolumu özelliğine bakmakta ve geriye true yada false değerini döndürmektedir. Başka bir örnek olarak maaşı 1000 YTL üzerinde olanların elde edilmesi istenebilir. Bunun içinde Maas1000Uzerindemi isimli bir metod geliştirilmiştir.

```csharp
static bool Maas1000Uzerindemi(Personel prs)
{
    return prs.Maas > 1000;
}
```

Tahmin edileceği üzere bu fonksiyonda, KontrolHandler temsilcisinin belirttiği yapıya uygun bir şekilde tasarlanmıştır. Bu yaklaşımlar göz önüne alındığında koleksiyon içerisinde istenildiği gibi filtreleme yapılabileceği görülmektedir. Tek şart temsilciye uygun tipte bir karşılaştırma fonksiyonelliğinin var olmasını sağlamaktır. Uygulamanın çalışma zamanındaki çıktısı aşağıdaki gibi olacaktır.

![mk247_2.gif](/assets/images/2008/mk247_2.gif)

Görüldüğü gibi maaşı 1000 YTL üzerinde olanlar, IK departmanında çalışanlar ve isminin baş harfi B olanlar kolay bir şekilde elde edilmektedir.

Bu yaklaşım her ne kadar kolay ve anlaşılır olsada bazı sıkıntılar olduğu ortadadır. Herşeyden önce Bul fonksiyonunun parametresi olan temsilci tipinin işaret edeceği metod bloklarının ayrı ayrı yazılıyor olma şartı vardır. Diğer taraftan söz konusu mimari şuanda sadece PersonelList isimli koleksiyona uygulanabilecek şekilde tasarlanmıştır. Hatta KontrolHandler isimli temsilci dahi sadece Personel tipleri ile çalışabilecek şekilde ele alınabilmektedir. Oysaki bu yapının herhangibir koleksiyon tipi içerisinde ele alınabilmesi sağlanabilmelidir.

Bu, ilgili yapının sadece uygulama bazlı değil Framework bazlı olacak şekilde genişletilebilmesini sağlayacaktır ki bu oldukça önemlidir. Elbette bu iş sanıldığı kadar kolay değildir. Nitekim türden bağımsız olacak şekilde fonksiyonel yapıların olması şarttır. Peki öyleyse olaya C# 2.0 açısından bakmaya çalışalım. Bu sefer elimizde isimsiz metodlar (Anonymous Methods) ve generic mimari gibi oldukça güçlü kozlar yer almaktadır. Dolayısıyla yukarıdaki örnek mimari modeli C# 2.0 içerisinde aşağıdaki şekilde ele alınabilir.

```csharp
using System;
using System.Collections.Generic;

namespace DotNet2Deyken
{
    enum Departman
    {
        BilgiIslem
        ,Yazilim
        ,Muhasebe
        ,InsanKaynaklari
        ,GenelMudurluk
    }

    class Personel
    {
        int _id;
        string _ad;
        string _soyad;
        Departman _bolumu;
        double _maas;
        DateTime _girisTarihi;

        public DateTime GirisTarihi
        {
            get { return _girisTarihi; }
            set { _girisTarihi = value; }
        }

        public string Soyad
        {
            get { return _soyad; }
            set { _soyad = value; }
        }

        public double Maas
        {
            get { return _maas; }
            set { _maas = value; }
        }

        internal Departman Bolumu
        {
            get { return _bolumu; }
            set { _bolumu = value; }
        }

        public string Ad
        {
            get { return _ad; }
            set { _ad = value; }
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public Personel(int id, string ad, string soyad, Departman bolumu, double maas, DateTime girisTarihi)
        {
            Id = id;
            Ad = ad;
            Soyad = soyad;
            Bolumu = bolumu;
            Maas = maas;
            GirisTarihi = girisTarihi;
        }
        public override string ToString()
        {
            return String.Format("{0} {1} {2} {3} {4} {5}", Id.ToString(), Ad, Soyad.ToUpper(), Bolumu.ToString(), Maas.ToString("C2"), GirisTarihi.ToShortDateString());
        }
    }

    // Koşul kontrolünü yapabilecek metodları içeren tür bağımsız temsilci(Generic Delegate) tanımı
    delegate bool KontrolHandler<T>(T parametre);

    class Program
    {
        // generic tipten oluşan koleksiyon üzerinden alt küme çekme işlemini üstlenen metod
        static List<T> Bul<T>(List<T> liste,KontrolHandler<T> handler)
        {
            List<T> sonuclar = new List<T>();
            foreach (T eleman in liste)
                if (handler(eleman)) // Generic temsilcinin işaret edeceği karşılaştırma metodu çağırılır.
                    sonuclar.Add(eleman);
            return sonuclar;
        }    

        // Generic Listeleme fonksiyonu
        static void Listele<T>(List<T> liste)
        {
            foreach (T t in liste)
                Console.WriteLine(t.ToString());
            Console.WriteLine("");
        }

        static void Main(string[] args)
        {    
            List<Personel> calisanlar = new List<Personel>();
    
            calisanlar.Add(new Personel(1000, "Mayk", "Hemır", Departman.BilgiIslem, 1050, new DateTime(1979, 10, 1)));
            calisanlar.Add(new Personel(1001, "Büyük", "Başkan", Departman.GenelMudurluk, 53000, new DateTime(1989, 2, 3)));
            calisanlar.Add(new Personel(1002, "EmSi", "Hemmır", Departman.GenelMudurluk, 13500, new DateTime(1990, 2, 4)));
            calisanlar.Add(new Personel(1003, "Tombul", "Raydır", Departman.InsanKaynaklari, 2250, new DateTime(1994, 8, 5)));
            calisanlar.Add(new Personel(1008, "Şirine", "Şirin", Departman.BilgiIslem, 900, new DateTime(1991, 3, 6)));
            calisanlar.Add(new Personel(1006, "Burak", "Selim", Departman.InsanKaynaklari, 2250, new DateTime(1976, 7, 3)));
            calisanlar.Add(new Personel(1004, "Osvaldo", "Nartayyo", Departman.Muhasebe, 3500, new DateTime(1975, 6, 3)));
            calisanlar.Add(new Personel(1005, "Higuin", "Kim", Departman.Yazilim, 1250, new DateTime(1974, 4, 2)));
            calisanlar.Add(new Personel(1007, "Karim", "Cabbar", Departman.Yazilim, 750, new DateTime(1975, 2, 7)));
            calisanlar.Add(new Personel(1011, "Billl", "Geytis", Departman.Yazilim, 650, new DateTime(1976, 3, 8)));

            // Anonymous Method yardımıyla arama işlemleri yapılır
            // Bul metodunun ikinci parametrelerinin nasıl verildiğine dikkat edelim

            // Insan Kaynakları departmanında çalışanların bulunması
            List<Personel> IKCalisanlari=Bul<Personel>(calisanlar,delegate(Personel p)
                                                                                    {
                                                                                        return p.Bolumu == Departman.Yazilim;
                                                                                    }
                                                                                );

            // Şubat ayında işe girenlerin bulunması
            List<Personel> SubatAyindaBaslayanlar = Bul<Personel>(calisanlar, delegate(Personel p)
                                                                                    {
                                                                                        return p.GirisTarihi.Month == 2;
                                                                                    }
                                                                                );

            //Departmanı Yazilim olanlardan Maaşı 1000 YTL üzerinde olanların bulunması
            List<Personel> MaasiVeDepartmaninaGore = Bul<Personel>(calisanlar, delegate(Personel p)
                                                                                    {
                                                                                        return (p.Maas >= 1000 && p.Bolumu == Departman.Yazilim);
                                                                                    }
                                                                                );
            Listele<Personel>(IKCalisanlari);
            Listele<Personel>(SubatAyindaBaslayanlar);
            Listele<Personel>(MaasiVeDepartmaninaGore);
        }
    }
}
```

Bu uzun kod parçasında bir önceki versiyona göre en büyük farklılıklar generic koleksiyon ile generic ve isimsiz metod (Anonymous Method) kullanımlarıdır. Dikkat edilecek olursa herhangibir tipteki List koleksiyonu üzerinde arama işlemi yapılabilmesini sağlayacak şekilde generic bir Bul metodu yer almaktadır. Dahada önemlisi, koleksiyon içerisindeki elemanların kıyaslama işlemlerinin yapılacağı metodları işaret edebilecek olan temsilcide generic olarak tanımlanmıştır. Bu sayede T tipindeki bir List koleksiyonu içerisinde Bul metodunun kullanılabilmesi ve o tip için bir koşullandırma yapılabilmesi sağlanmaktadır.

Fakat bütün bunlara rağmen en çok dikkate değer kısımlardan biriside, isimsiz metodların kullanımıdır. Bu sebepten dolayı bir önceki örnekte olduğu gibi, ayrı ayrı karşılaştırma metodlarının yazılmasına gerek kalmamaktadır. Tam aksine Bul metodunun kullanıldığı yerlerde ikinci parametrelerde isimsiz metod kullanılarak koşul deyimlerinin aynı ifade içerisinde tanımlanabilmeside sağlanmıştır. Örneğin Şubat ayında işe giren personelin bulunabilme sürecini göz önüne alalım. Burada Bul metodu, calisanlar isimli generic koleksiyondaki Personel nesne örneklerini tek tek dolaşmalı, GirisTarihi özellikleri üzerinden Month değerlerinin 2 olup olmadığına bakmalı ve eğer öyleyse bunları yeni bir koleksiyonda birleştirerek geriye döndürmelidir. İsimsiz metodlar yardımıyla bu iş aşağıda görüldüğü gibi tek bir ifadede sağlanabilir.

```csharp
List<Personel> SubatAyindaBaslayanlar = Bul<Personel>(calisanlar, delegate(Personel p)
{
   return p.GirisTarihi.Month == 2;
}
);
```

Dikkat edilecek olursa delegate anahtar kelimesi burada KontrolHandler tipini işaret etmektedir. Dahası kod yazılırken generic mimarinin, Visual Studio IDE'si içerisinde aşağıdaki şekilde ele alındığı görülmektedir.

![mk247_4.gif](/assets/images/2008/mk247_4.gif)

Görüldüğü gibi Bul metoduna generic parametre olarak Personel tipi verildiğinde, liste isimli List koleksiyonu ve handler isimli KontrolHandler temsilcisi otomatik olarak bu tiple çalışacak hale gelmektedir. Buda Bul metodunun generic yapısından kaynaklanmaktadır.

Sonuç olarak program çıktısı aşağıdaki gibi olacaktır.

![mk247_3.gif](/assets/images/2008/mk247_3.gif)

Elbette Bul fonksiyonu geliştirici tarafından yazılmış bir metoddur. Oysaki.Net Framework 2.0 özellikle List koleksiyonları üzerinde bu tip filtreleme ve arama işlemlerinin gerçekleştirilmesi amacıyla hazır Predicate temsilcisini kullanan Find, FindAll, Exists, FindIndex, FindLast, FindLastIndex, RemoveAll gibi metodlar içermektedir. (Burada hazır bir temsilcinin olması geliştiricinin uygulamadan bağımsız olacak şekilde, Framework'ün kullanıldığı her yerde söz konusu koşullandırma metodlarını işaret ederek, başka hazır CLR tipi metodlarına parametre olarak verebileceği anlamına da gelmektedir.) Dolayısıyla yukarıda geliştirilen örnek,.Net Framework 2.0' ın tipleri sayesinde aşağıdaki hale getirilebilir.

```csharp
class Program
{
    static void Listele<T>(List<T> liste)
    {
        foreach (T t in liste)
            Console.WriteLine(t.ToString());
        Console.WriteLine("");
    }

    static void Main(string[] args)
    {
        List<Personel> calisanlar = new List<Personel>();
        
        #region Test Verileri

        // Test verilerinin girildiği kodlar

        #endregion
        
        List<Personel> BHarfliler = 
                                calisanlar.FindAll(delegate(Personel p)
                                                            {
                                                                return p.Ad[0] == 'B';
                                                            }
                                                        );
        List<Personel> SubattaBaslayanlar=
                                calisanlar.FindAll(delegate(Personel p)
                                                            {
                                                                return p.GirisTarihi.Month == 2;
                                                            }
                                                        );
        List<Personel> GenelMudurlukCalisanlari = 
                                calisanlar.FindAll(delegate(Personel p)
                                                            {
                                                                return p.Bolumu == Departman.GenelMudurluk;
                                                            }
                                                        );
    
        Listele<Personel>(BHarfliler);
        Listele<Personel>(SubattaBaslayanlar);
        Listele<Personel>(GenelMudurlukCalisanlari);
    }
}
```

Bu kez delegate anahtar kelimesi FindAll metodunun istediği Predicate temsilcisini işaret etmektedir. Kod yazımı sırasında intellisense ile bu açık bir şekilde görülmektedir.

![mk247_5.gif](/assets/images/2008/mk247_5.gif)

Mimaride halen daha eksiklikler vardır. Özellikle veri tabanı uygulamalarında yer alan sorgulama tekniklerinin, programatik tarafta ifade edilme zorlukları bilinmektedir. Çok basit olarak düşünüldüğünde, bir veritabanı tablosunun program tarafında Entity olarak sınıf bazlı ifade edilmesi sonrasında geliştiricilerin beklentisi, veri sorgulama dili esnekliğinin nesnel olarakta sağlanabilmesidir. Bir başka deyişle bilinen select, where, group by, distinct, sum vb... sorgulama kelimelerinin, program tarafındaki nesnel varlıklar üzerinde de uygulanamabilmesi istenmektedir. İşte bu LINQ mimarisinin geliştirilmesinin en büyük nedenlerinden de birisidir. Peki elde bulunan imkanlar ile bu nasıl sağlanabilir? Yoksa dile yeni bir takım kolaylaştırıcı özelliklerin entegre edilmesimi gerekmektedir?

Herşeyden önce son örnekte yer alan FindAll metodu ile, bir koleksiyon üzerinde filtreleme yapılabildiği görülmektedir. Bu bir anlamda Where ve Select gibi ifadelerin bir karşılığı olarak göz önüne alınabilir. Ancak bu yeterli değildir. Yeni tipler geliştirmeden, var olan.Net Framework tiplerine FindAll metoduna benzer fonksiyonel yenilikleri ilave edebilmek gerekmektedir. İşte bu noktada Extension metodlar devreye girerek özellikle IEnumerable uyarlamalı tiplerin genişletme metodları ile LINQ mimarisine destek verebilmesi sağlanmaktadır. İşin içerisinde yine temsilci (delegate) tipleri rol almaktadır. LINQ mimarisine destek verebilmek için pek çok temsilci tipi geliştirilmiş ve Framework içerisine dahil edilmiştir. Bu kadar ilerlemeden önce, yazıya konu olan örneğin C# 3.0 içerisindeki geliştirilme şekline bakmakta yarar vardır. Nitekim ilk hedef Lambda ifadelerinin rolünü kavramaktır. (Bu seferki örnek Visual Studio 2008 üzerinde.Net Framework 3.5 seçilerek yapılmıştır.)

![mk247_8.gif](/assets/images/2008/mk247_8.gif)

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNet3Nokta5Deyken
{
    enum Departman
    {
        BilgiIslem
        ,Yazilim
        ,Muhasebe
        ,InsanKaynaklari
        ,GenelMudurluk
    }
    class Personel // Bu sınıfta otomatik özellikler(Automatic Property) kullanılmıştır.
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public Departman Bolumu { get; set; }
        public double Maas { get; set; }
        public DateTime GirisTarihi { get; set; }

        public override string ToString()
        {
            return String.Format("{0} {1} {2} {3} {4} {5}", Id.ToString(), Ad, Soyad.ToUpper(), Bolumu.ToString(), Maas.ToString("C2"), GirisTarihi.ToShortDateString());
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            // Object Initializers' dan faydalanılmıştır.
            List<Personel> calisanlar = new List<Personel>()
            {
                new Personel(){Id=1000, Ad="Mayk", Soyad="Hemır", Bolumu=Departman.BilgiIslem, Maas=1050, GirisTarihi=new DateTime(1979, 10, 1)},
                new Personel(){Id=1001, Ad="Büyük", Soyad="Başkan",Bolumu= Departman.GenelMudurluk,Maas= 53000,GirisTarihi= new DateTime(1989, 2, 3)},
                new Personel(){Id=1002, Ad="EmSi", Soyad="Hemmır", Bolumu=Departman.GenelMudurluk, Maas=13500, GirisTarihi=new DateTime(1990, 2, 4)},
                new Personel(){Id=1003, Ad="Tombul", Soyad="Raydır", Bolumu=Departman.InsanKaynaklari, Maas=2250, GirisTarihi=new DateTime(1994, 8, 5)},
                new Personel(){Id=1008, Ad="Şirine", Soyad="Şirin",Bolumu= Departman.BilgiIslem, Maas=900, GirisTarihi=new DateTime(1991, 3, 6)},
                new Personel(){Id=1006, Ad="Burak", Soyad="Selim", Bolumu=Departman.InsanKaynaklari,Maas= 2250, GirisTarihi=new DateTime(1976, 7, 3)},
                new Personel(){Id=1004, Ad="Osvaldo", Soyad="Nartayyo", Bolumu=Departman.Muhasebe, Maas=3500,GirisTarihi= new DateTime(1975, 6, 3)},
                new Personel(){Id=1005, Ad="Higuin", Soyad="Kim",Bolumu= Departman.Yazilim, Maas=1250,GirisTarihi= new DateTime(1974, 4, 2)},
                new Personel(){Id=1007, Ad="Karim", Soyad="Cabbar", Bolumu=Departman.Yazilim, Maas=750, GirisTarihi=new DateTime(1975, 2, 7)},
                new Personel(){Id=1011, Ad="Billl", Soyad="Geytis", Bolumu=Departman.Yazilim, Maas=650,GirisTarihi= new DateTime(1976, 3, 8)}
            };

            // B ile başlayanlar
            var AdiBIleBaslayanlar = calisanlar.FindAll((Personel p) => (p.Ad[0] == 'B'));

            // Departmanı Yazilim olanlar (Burada type inference söz konusu)
            var YazilimDepartmaniCalisanlari = calisanlar.FindAll(p => p.Bolumu == Departman.Yazilim);

            //Giris yılı 1976 öncesi olanlar çekilirken başka bir metod çağırılıyor.
            var GirisYili1976OncesiOlanlar = calisanlar.FindAll(
                                                                            p =>{
                                                                                if (p.GirisTarihi.Year < 1976)
                                                                                {
                                                                                    PrimArttir(p);
                                                                                    return true;
                                                                                }
                                                                                else
                                                                                    return false;
                                                                            }
                                                                    );

            Listele<Personel>(AdiBIleBaslayanlar);        
            Listele<Personel>(YazilimDepartmaniCalisanlari);
            Listele<Personel>(GirisYili1976OncesiOlanlar);
        }

        private static void PrimArttir(Personel p)
        {
            Console.WriteLine("\t"+p.Ad+" "+p.Soyad.ToUpper()+" için prim arttırım talebi");
        }
    
        static void Listele<T>(IEnumerable<T> liste)
        {
            foreach (T t in liste)
                Console.WriteLine(t.ToString());
            Console.WriteLine("");
        }
    }
}
```

Örnekte C# 3.0 ile birlikte gelen pek çok yenilik kullanılmaya çalışılmıştır. var anahtar kelimesi, koleksiyon ve Personel nesnelerininin başlatılması (object initialization), otomatik özellikler (automatic properties) gibi. Ancak yazımızda odaklanacağımız nokta => operatörü ve içerisinde yer aldığı ifadelerdir. Dikkat edilecek olursa FindAll metodlarının içerisinde kullanılan parametrelerde => operatörleri yer almaktadır. İlk metod çağrısı aşağıdaki gibidir.

```csharp
var AdiBIleBaslayanlar = calisanlar.FindAll((Personel p) => (p.Ad[0] == 'B'));
```

Burada => operatörünün sol tarafında Personel tipinden bir değişken tanmı yer almaktadır. Operatörün sağ tarafında ise yine parantezler içerisinde p değişkeninin Ad özelliğinin ilk karakterine bakılmaktadır. Daha önceki örneklerden hatırlanacağı gibi FindAll metodu Predicate temsilcisini parametre olarak almaktadır. Bu temsilci geriye bool değer döndüren ve generic tipte parametre alan metodları taşıyabilemektedir. Bu sebepten Lambda operatörünün sağ tarafında yer alan kod parçasının bool tipinden bir değer döndürüyor olması şarttır.

Predicate temsilcisinin işaret edeceği metodun alacağı parametre ise operatörün sol tarafında belirtilmektedir. Peki burada Lambda operatörü neyi sağlamaktadır? Nitekim aynı amaç için isimsiz metod (anonymous method) kullanımıda mümkündür. Hatta isimsiz metod kullanmadanda yapılabildiği görülmektedir. Ne farki fonksiyonel programlama ortamlarına bakıldığında bu tip ifadelerin yaygın bir şekilde ele alındığı görülmüştür. Bununla beraber => operatörü burada, temsilcinin örneklenmesi, işaret edeceği metoda parametre aktarılması, uygun tipte sonuç üreten bir kod bloğunun yazılması operasyonlarının tek bir ifade içerisinde gerçekleştirilmesini sağlamaktadır.

İkinci kullanım şekli ilkinden biraz daha farklıdır.

```csharp
var YazilimDepartmaniCalisanlari = calisanlar.FindAll(p => p.Bolumu == Departman.Yazilim);
```

Bu sefer dikkat çekici nokta => operatörünün sol ve sağ tarafındaki deyimlerde parantez kullanılmayışı değildir. Dikkat edilmesi gereken nokta operatörün sol tarafında sadece p yazılmasıdır. Oysaki bir önceki kullanım şeklinde temsilcinin işaret ettiği metoda aktarılacak olan parametrenin tipi açık bir şekilde belirtilmiştir. Burada tip tahmini (type inference) kavramı devreye girmektedir. Öyleki FindAll metodunun, List koleksiyonunun generic yapısına göre kullanacağı tipin Personel olma olasılığı muhtemeldir. Bu son derece doğaldır nitekim calisanlar değişkeni List tipinden bir koleksiyonu taşımaktadır. Buna göre compiler, p değişkeninin Personel tipinden olacağını tahmin eder. Bu tahminin ne kadar tutarlı olduğu Visual Studio IDE'si içerisinde intellisense özelliği ile açık bir şekilde görülebilmektedir.

![mk247_6.gif](/assets/images/2008/mk247_6.gif)

Görüldüğü gibi lambda operatörünün sağ tarafında p değişkeni kullanılmak istendikten sonra, tahmin edilen tipin üyeleri ekrana gelmektedir.

Üçüncü kullanım şekli ise aşağıdaki gibidir.

```csharp
var GirisYili1976OncesiOlanlar = calisanlar.FindAll(
p =>{
	if (p.GirisTarihi.Year < 1976)
	{
		PrimArttir(p);
		return true;
	}
	else
		return false;
	}
);
```

Burada ise tek fark lambda operatörünün sağ tarafında yer alan deyimlerde normal kod bloklarınında geliştirilebiliyor olmasıdır. Bir başka deyişle Predicate temsilcisinin istediği şekilde bool değer döndürmek dışında, metod çağrısı gibi farklı deyimlerde yapılabilmektedir.

Geliştirilen son örnek çalıştırıldığında aşağıdaki ekran görüntüsünde yer alan sonuçların alındığı görülmektedir.

![mk247_7.gif](/assets/images/2008/mk247_7.gif)

Lambda operatörleri özellikle LINQ içerisinde yer alan genişletme metodlarında sıklıkla kullanılmaktadır. Bilindiği üzere LINQ sorgularının desteklenmesi için Enumerable (System.Core.dll assembly'ı içerisinde yer alan System.Linq isim alanında yer almaktadır) isimli static sınıf içerisine çok sayıda genişletme metodu (Extension Methods) dahil edilmiştir. Bu sınıfın en büyük özelliklerinden biriside içerisinde yer alan metodlarının IEnumerable türevli tipleri genişletmesidir. Diğer taraftan söz konusu sınıf içerisinde çoğunlukla Func isimli generic temsilci kullanılmaktadır. Bu temsilcisinin farklı versiyonları aşağıdaki gibidir.

```csharp
delegate TResult Func<TResult>(T arg)
delegate TResult Func<T, TResult>(T arg)
delegate TResult Func<T1,T2, TResult>(T1 arg1, T2 arg2)
delegate TResult Func<T1,T2,T3 TResult>(T1 arg1, T2 arg2, T3 arg3)
delegate TResult Func<T1,T2,T3,T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
```

Func bir temsilci olduğu için, kullanılacağı her yerde lambda operatörleri ele alınabilir. Buna çok basit olarak aşağıdaki kod parçasını örnek gösterebiliriz.

```csharp
double sonuc = calisanlar
                            .Where<Personel>(p => p.Bolumu == Departman.Yazilim)
                                .Sum<Personel>(p => p.Maas);
Console.WriteLine(sonuc.ToString("C2"));

int sonuc2 = calisanlar.Aggregate(0,(toplam,p) => p.Maas>2000?toplam+=1:toplam);
Console.WriteLine(sonuc2.ToString());
```

İlk kullanımda departmanı yazılım olan personelin maaşlarının toplamı bulunmuştur. İkinci kullanımda ise lambda operatörünün iki parametreyi birden temsilciye gönderdiği görülmektedir. Dikkat edilecek olursa operatörün sol tarafında toplam ve p isimli değişkenler tanımlanmaktadır.(Bu iki parametre bildirimi eğer parantezler içersinde yazılmassa derleme zamanı hatası alınır. Dolayısıyla lambda operatörünün sol tarafında birden fazla parametre olacaksa bunların parantezler içerisine alınması gerekmektedir.)

Buradaki toplam değişkeni yine tahmin edilerek int tipinden belirlenmiştir. Bu tarz bir kullanım normaldir nitekim Func temsilcisinin bu şekilde iki parametre ile çalışan versiyonu mevcuttur. Buna göre Aggregate metodu, personelin maaşı 2000 YTL üzerinde olanlar var ise, toplam değişkeninin değerini 1 arttırarak geriye döndürmektedir. (Bu işlem için Sum metoduda kullanılabilir. Aggregate genişletme metodunun yazılmasının amacı Sum, Count, Max, Min, Avg gibi standart gruplama fonksiyonları dışındaki gereksinimlerin karşılanmasıdır.)

Artık Lambda operatörünün kullanımı hakkında fikir sahibi olduğumuzu sanıyorum. Şimdi diğer noktalara değinmeye çalışalım. Söz gelimi lambda operatörünün yer aldığı ifadeler IL (Intermediate Language) tarafında nasıl yorumlanmaktadır? Bu noktada lambda operatörünün, isimsiz metod kullanımı ile aynı IL çıktısını verdiğini söyleyebiliriz. Örnek olarak aşağıdaki kod parçasını göz önüne alalım.

```csharp
using System;

namespace LambdaVeCIL
{
    delegate T IslemHandler<T>(T T1,T T2);

    class Program
    {
        static void Main(string[] args)
        {
            IslemHandler<double> hnd = (x, y) => x + y;

            IslemHandler<int> hnd2=
                delegate(int a,int b){
                    return a + b; 
                };
        }
    }
}
```

Yukarıdaki kod parçasında yer alan IslemHandler isimli generic temsilci tipi, T türünden iki parametre alan ve yine T türünden sonuç üreten metodları işaret edebilecek şekilde tasarlanmıştır. hnd değişkeni oluşturulurken lambda ifadesinden, hnd2 oluşturulurkende isimsiz metoddan (anonymous method) yararlanılmıştır. Bu kodun IL çıktısına ildasm aracı ile bakıldığında ağaç yapısının aşağıdaki gibi olduğu görülür.(Ağaç yapısının kolay bir şekilde elde edilmesi için Dump TreeView seçeneğinden yararlanılmıştır.)

![mk247_9.gif](/assets/images/2008/mk247_9.gif)

Program içerisinde b0 ve b1 adları ile tanımlanmış iki adet metod olduğu görülmektedir. Tahmin edileceği üzere bu iki üye, lambda ifadesi ve isimsiz metod kullanımı sonrası oluşturulmuş metodlardır. Bir başka deyişle lambda operatörü kullanıldığında aynen isimsiz metodlarda olduğu gibi IL tarafında iş yapan metod oluşturulmaktadır. Bu metodlar derleyici tarafından oluşturulan gizli metodlardır. Compiler tarafından oluşturuldukları için CompilerGenerated niteliği (Attribute) ile imzalanmışlardır. Eğer Main metodunun IL çıktısına bakılırsa aşağıdaki kod parçalarının üretildiği görülür.

```text
.method private hidebysig static void Main(string[] args) cil managed
{
    .entrypoint
    // Code size 66 (0x42)
    .maxstack 3
    .locals init ([0] class LambdaVeCIL.IslemHandler`1<float64> hnd,[1] class LambdaVeCIL.IslemHandler`1<int32> hnd2)
    IL_0000: nop
    IL_0001: ldsfld class LambdaVeCIL.IslemHandler`1<float64> LambdaVeCIL.Program::'CS$<>9__CachedAnonymousMethodDelegate2'
    IL_0006: brtrue.s IL_001b
    IL_0008: ldnull
    IL_0009: ldftn float64 LambdaVeCIL.Program::'<Main>b__0'(float64,float64)
    IL_000f: newobj instance void class LambdaVeCIL.IslemHandler`1<float64>::.ctor(object, native int)
    IL_0014: stsfld class LambdaVeCIL.IslemHandler`1<float64> LambdaVeCIL.Program::'CS$<>9__CachedAnonymousMethodDelegate2'
    IL_0019: br.s IL_001b
    IL_001b: ldsfld class LambdaVeCIL.IslemHandler`1<float64> LambdaVeCIL.Program::'CS$<>9__CachedAnonymousMethodDelegate2'
    IL_0020: stloc.0
    IL_0021: ldsfld class LambdaVeCIL.IslemHandler`1<int32> LambdaVeCIL.Program::'CS$<>9__CachedAnonymousMethodDelegate3'
    IL_0026: brtrue.s IL_003b
    IL_0028: ldnull
    IL_0029: ldftn int32 LambdaVeCIL.Program::'<Main>b__1'(int32,int32)
    IL_002f: newobj instance void class LambdaVeCIL.IslemHandler`1<int32>::.ctor(object,native int)
    IL_0034: stsfld class LambdaVeCIL.IslemHandler`1<int32> LambdaVeCIL.Program::'CS$<>9__CachedAnonymousMethodDelegate3'
    IL_0039: br.s IL_003b
    IL_003b: ldsfld class LambdaVeCIL.IslemHandler`1<int32> LambdaVeCIL.Program::'CS$<>9__CachedAnonymousMethodDelegate3'
    IL_0040: stloc.1
    IL_0041: ret
} // end of method Program::Main
```

Her ne kadar IL (Intermediate Language) tarafı karışık görünsede dikkat edilmesi gereken noktalar IL0001 - IL0020 aralığındaki yapının IL0020 - IL0040 arasındaki ile aynı olmasıdır. Söz edilen ilk aralıkta lambda ifadesinin kullanıldığı satıra ait üretimler yer almaktadır. İkinci parçada ise isimsiz metod kullanımına ait üretimler bulunmaktadır. Yazımızın asıl amacı IL tarafındaki üretimleri kavramak değildir ancak sonuç itibariyle lambda ifadeleri, isimsiz metodlar ile aynı IL çıktılarının üretilmesini sağlamaktadır.

Son olarak lambda ifadeleri kullanılırken dikkat edilmesi gereken bazı durumları göz önüne alalım.

1 - Lambda ifadelerinde tanımlanan değişkenler diğer metodlar tarafından kullanılamazlar. Başka bir deyişle, değişkenlerin kapsamı lambda ifadesinin sınırlarıdır. Aşağıdaki ekran görüntüsünde bu durum ifade edilmektedir. Görüldüğü gibi ifade içerisinde tanımlanan d değişkenine kapsam dışından erişilememekte ve derleme zamanı hatası (Compile-Time Error) alınmaktadır.

![mk247_10.gif](/assets/images/2008/mk247_10.gif)

2 - Elbette lambda ifadesi dışında tanımlanmış olan bir değişkene, ifade içerisinden erişilebilmektedir. Söz gelimi aşağıdaki ekran çıktısındanda görüleceği gibi, d değişkeni lambda ifadesi dışında 10 olarak tanımlanmış ve metod çağrısından sonra 11 olarak değiştirilmiştir.

![mk247_11.gif](/assets/images/2008/mk247_11.gif)

3 - Lambda ifadelerinin sol tarafında yer alan parametrelerde ref ve out anahtar kelimeleri kullanılamaz.

Bu yazımızda lambda ifadelerinin genel kullanımı üzerinde durulmaya çalışmıştır. Lambda ifadelerinin getirdiği kolaylığı görmek amacıyla C# 1.0 tarafından C# 3.0 tarafına doğru ilerlenmeye çalışılmıştır. Lambda ifadeleri ile ilişkili bir diğer önemli konuda ifade ağaçlarıdır (Expression Trees). Bu konuyu ilerleyen yazılarımızda incelemeye çalışacağız. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](/assets/files/2008/LambdayiAnlamak.rar)