---
layout: post
title: "Kalıtım (Inheritance) Kavramına Kısa Bir Bakış"
date: 2003-12-25 12:00:00 +0300
categories:
  - csharp
tags:
  - csharp
  - oop
  - .net
---
Bir önceki makalemizde C# dilinde sınıf kavramına bir giriş yapmış ve OOP (Objcet Oriented Programming-Nesneye Dayalı Programlama) tekniğinin en önemli kavramlarından biri olan kalıtımdan bahsedeceğimizi söylemiştik. Bugünkü makalemizde bu kavramı incelemeye çalışacağız.

Kalıtım kavramı için verilebilecek en güzel örnekler doğamızda yer almaktadır. Örneğin Bitkiler, türlerine gore sınıflandırılırlar. Ancak hepsinin birer bitki olması ortak bir takım özelliklere sahip oldukları anlamınada gelmektedir. Bitki isimli bir sınıfı göz önüne alıp Ağaçlar,Çiçekler,Deniz Bitkileri vb… gibi alt sınıflara ayırabiliriz. Tüm bu alt sınıflar Bitki sınıfından gelmekte, yani türemektedirler. Aynı şekilde Bitki türü olan bir sınıf kendi içinde başka alt sınıflara ayrılabilir. Örneğin, Çiçek sınıfı Güller, Orkideler, Papatyalar vb… Tüm bu nesneler hiyerarşik bir yapıda ele alındıklarında temel bir sınıftan türedikleri görülebilmektedir.

Temel bir nesne ve bu nesneden bir takım özellikleri almış ve başka ek özelliklere sahip olan nesneler bir araya getirildiklerinde aralarındaki ilişkinin kalıtımsal olduğundan söz ederiz. İşte nesneye dayalı programlama dillerininde en önemli kavramlarından birisi kalıtımdır. Ortak özelliklere sahip olan tüm nesneleriniz için bir sınıf yazarsınız. Ancak elinizde bu ortak özelliklerin yanında başka ek özelliklere sahip olacak veya ortak özelliklerden bir kaçını farklı şekillerde değerlendirecek nesnelerinizde olabilir. Bunlar için yazacağınız sınıf, ilk sınıfı temel sınıf olarak baz alıcak ve bu temel sınıftan türetilecek ve kendi özellikleri ile işlevselliklerine sahip olucaktır. Konuyu daha iyi pekiştirmek amacı ile aşağıdaki şekili göz önüne alarak durumu zihnimizde canlandırmaya çalışalım.

![mk26_1.gif](/assets/images/2003/mk26_1.gif)

Şekil 1. Inheritance Kavramı

C# ile Temel bir sınıftan birden fazla sınıf türetebilirsiniz. Ancak bir sınıfı birden fazla sınıftan türetmeniz mümkün değildir. Bunu yapmak için arayüzler (interface) kullanılır. Türeyen bir sınıf türediği sınıfın tüm özelliklerine ve işlevlerine sahiptir ve türediği sınıftaki bu elemanların yeniden tanımlanmasına gerek yoktur. Ayrıca siz yeni özellikler ve işlevsellikler katabilirsiniz. Bu makalemizdeki örnek uygulamamızda kalıtımda önemli role sahip base ve new anahtar kelimelerinin kullanımında göreceksiniz. Base ile temel sınıfa nasıl paramtere gönderebileceğimizi, temel sınıftaki metodları nasıl çağırabileceğimizi ve new anahtar sözcüğü sayesinde türeyen sınıf içinde, temel sınıftaki ile aynı isme sahip metodların nasıl işlevsellik kazanacağını da inceleme fırsatı bulacaksınız. Dilerseniz bu kısa açıklamalardan sonra hemen örnek uygulamamızın kodlarına geçelim. Konu ile ilgili detaylı açıklamaları örnek içerisindeki yorum satırlarında bulabilirisiniz.

```csharp
using System;
namespace Inheritance1
{
    class TemelSinif /* Öncelikle temel sınıfımızı yani Base Class'ımızı yazalım. */
    {
        private string SekilTipi; /* Sadece bu class içinde tanımlı bir alan tanımladık. Bu alana Türeyen sınıfımız (derived class) içerisinden de erişemeyiz. Eğer temel sınıfta yer alan bir alana sadece türeyen sınıftan erişebilmek ve başka sınıflardan erişilmesini engellemek istiyorsak, bu durumda bu alanı protected olarak tanımlarız.*/
        /* Bir özellik tanımlıyoruz. Sadece get bloğu olduğu için yanlızca okunabilir bir özellik. */
        public string sekilTipi
        {
            get
            {
                return SekilTipi;
            }
        }

        public TemelSinif() /* Temel sınıfımızn varsayılan yapıcı metodu. */
        {
            SekilTipi = "Kare";
        }

        public TemelSinif(string tip) /* Overload ettiğimiz yapıcı metodumuz. */
        {
            SekilTipi = tip;
        }

        public string SekilTipiYaz() /* String sonuç döndüren bir metod.*/
        {
            return "Bu Nesnemiz, " + SekilTipi.ToString() + " tipindedir";
        }
    }

    /* İşte türeyen sınıfımız. :TemelSinif yazımı ile, bu sınıfın TemelSinif'tan türetildiğini belirtiyoruz. Böylece, TureyenSinif class'ımız TemelSınıf class'ının özelliklerinide bünyesinde barındırmış oluyor.*/
    class TureyenSinif : TemelSinif
    {
        private bool Alan;
        private bool Cevre;
        private int Taban;
        private int Yukseklik;
        private int UcuncuKenar;
        private bool Kare;
        private bool Dikdortgen;
        private bool Ucgen;
        public bool alan
        {
            get
            {
                return Alan;
            }
        }

        public bool cevre
        {
            get
            {
                return Cevre;
            }
        }

        public int taban
        {
            get
            {
                return Taban;
            }
        }

        public int yukseklik
        {
            get
            {
                return Yukseklik;
            }
        }

        public int ucuncuKenar
        {
            get
            {
                return UcuncuKenar;
            }
        }

        public bool kare
        {
            get
            {
                return Kare;
            }
            set
            {
                Kare = value;
            }
        }
        public bool dikdortgen
        {
            get
            {
                return Dikdortgen;
            }
            set
            {
                Dikdortgen = value;
            }
        }

        public bool ucgen
        {
            get
            {
                return Ucgen;
            }
            set
            {
                Ucgen = value;
            }
        }

        public TureyenSinif()
            : base() /* Burada base anahtar kelimesine dikkatinizi çekerim. Eğer bir TureyenSinif nesnesini bu yapıcı metod ile türetirsek, TemelSinif'taki yapıcı metod çalıştırılacaktır. */
        {
        }

        public TureyenSinif(string tip, bool alan, bool cevre, int taban, int yukseklik, int ucuncuKenar)
            : base(tip) /* Buradada base anahtar kelimesi kullanılmıştır. Ancak sadece tip isimli string değişkenimiz, TemelSinif'taki ilgili yapıcı metoda gönderilmiştir. Yani bu yapıcı metod kullanılarak bir TureyenSinif nesnesi oluşturduğumuzda, TemelSinif'taki bir tek string parametre alan yapıcı metod çağırılır ve daha sonra buraya dönülerek aşağıdaki kodlar çalıştırılır.*/
        {
            Alan = alan;
            Cevre = cevre;
            Taban = taban;
            Yukseklik = yukseklik;
            UcuncuKenar = ucuncuKenar;
        }

        public double AlanBul() /* Tureyen sınıfımızda bir metod tanımlıyoruz. */
        {
            if (Kare == true)
            {
                return Taban * Taban;
            }
            if (Dikdortgen == true)
            {
                return Taban * Yukseklik;
            }
            if (Ucgen == true)
            {
                return (Taban * Yukseklik) / 2;
            }
            return 0;
        }
        public double CevreBul() /* Başka bir metod. */
        {
            if (Kare == true)
            {
                return 4 * Taban;
            }
            if (Dikdortgen == true)
            {
                return (2 * Taban) + (2 * Yukseklik);
            }
            if (Ucgen == true)
            {
                return Taban + Yukseklik + UcuncuKenar;
            }
            return 0;
        }
        public new string SekilTipiYaz() /* Buradaki new anahtar kelimesine dikkat edelim. SekilTipiYaz metodunun aynısı TemelSinif class'ımızda yer almaktadır. Ancak bir new anahtar kelimesi ile aynı isimde bir metodu TureyenSinif class'ımız içinde tanımlamış oluyoruz. Bu durumda, TureyenSinif class'ından oluşturulan bir nesneye ait SekilTipiYaz metodu çağırılırsa, buradaki kodlar çalıştırılır. Oysaki new anahtar kelimesini kullanmasaydık, TemelSinif içindeki SekilTipiYaz metodunun çalıştırılacağını görecektik. */
        {
            if (Kare == true)
            {
                return "kare";
            }
            if (Dikdortgen == true)
            {
                return "dikdortgen";
            }
            if (Ucgen == true)
            {
                return "üçgen";
            }
            return "Belirsiz";
        }
    }
    class Class1
    {
        static void Main(string[] args)
        {
            /* Önce TemelSinif tipinde bir nesne oluşturup SekilTipiYaz metodunu çağırıyoruz.*/
            TemelSinif ts1 = new TemelSinif();
            Console.WriteLine(ts1.SekilTipiYaz());
            TemelSinif ts2 = new TemelSinif("Dikdörtgen"); /* Bu kes TemelSinif tipinden bir nesneyi diğer yapıcı metodu ile çağırıyor ve SekilTipiYaz metodunu      çalıştırıyoruz. */
            Console.WriteLine(ts2.SekilTipiYaz());

            /* Şimdi ise TureyenSinif'tan bir nesne oluşturduk ve sekilTipi isimli özelliğin değerini aldık. Kodlara bakıcak olursanız sekilTipi özelliğinin TemelSinif içinde tanımlandığını görürsünüz. Yani TureyenSinif nesnemizden, TemelSinif class'ındaki izin verilen alanlara,metodlara vb.. ulaşabilmekteyiz.*/
            TureyenSinif tur1 = new TureyenSinif();
            Console.WriteLine(tur1.sekilTipi);

            /* Şimdi ise başka bir TureyenSinif nesnesi tanımladık. Yapıcı metodumuz'un ilk parametresinin değeri olan "Benim Şeklim" ifadesi base anahtar kelimesi nedeni ile, TemelSinif class'ındaki ilgili yapıcı metoda gönderilir. Diğer parametreler ise TureyenSinif class'ı içinde işlenirler. Bu durumda tur2 isimli TureyenSinif nesnemizden TemelSinifa ait sekilTipi özelliğini çağırdığımızda, ekrana Benim Şeklim yazdığını görürüz. Çünkü bu değer TemelSinif class'ımızda işlenmiş ve bu class'taki özelliğe atanmıştır. Bunu sağlayan base anahtar kelimesidir.*/
            TureyenSinif tur2 = new TureyenSinif("Benim Şeklim", true, true, 2, 4, 0);
            Console.WriteLine(tur2.sekilTipi);

            tur2.dikdortgen = true;
            Console.WriteLine(tur2.SekilTipiYaz());

            /* Tureyen sinif içinde tanımladığımız SekilTipiYaz metodu çalıştırılır. Eğer, TureyenSinif içinde bu metodu new ile tanımlamasaydık TemelSinif class'i içinde yer alan aynı isimdeki metod çalışıtırılırdır.*/
            if (tur2.alan == true)
            {
                Console.WriteLine(tur2.sekilTipi + " Alanı:" + tur2.AlanBul());
            }
            if (tur2.cevre == true)
            {
                Console.WriteLine(tur2.sekilTipi + " Çevresi:" + tur2.CevreBul());
            }
            TureyenSinif tur3 = new TureyenSinif("Benim üçgenim", true, false, 10, 10, 10);
            Console.WriteLine(tur3.sekilTipi);

            tur3.ucgen = true;
            Console.WriteLine(tur3.SekilTipiYaz());

            Console.WriteLine(tur3.sekilTipi + " Alanı:" + tur3.AlanBul());
            if (tur2.cevre == true)
            {
                Console.WriteLine(tur3.sekilTipi + " Çevresi:" + tur3.CevreBul());
            }
        }
    }
}
```

Uygulamamızı çalıştırdığımızda ekran görüntümüz aşağıdaki gibi olucaktır. Umuyorum ki kalıtım ile ilgili olarak yazmış olduğumuz bu örnek sizlere yeni fikirler verebilecektir. Örneğin çok işlevsel olması şu an için önemli değil. Ancak bir sınıfın başka bir sınıftan nasıl türetildiğine, özellikle base ve new anahtar kelimelerinin bize sağladığı avantajlara dikkat etmenizi isterim.

![mk26_2.gif](/assets/images/2003/mk26_2.gif)

Şekil 2. Uygulamanın çalışmasının sonucu.

Geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.