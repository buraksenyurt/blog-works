---
layout: post
title: "Generic Mimaride Kısıtlamaların(Constraints) Kullanımı"
date: 2005-12-31 08:00:00 +0300
description: "Bu makalemizde .Net 2.0 ile birlikte gelen Generic mimarisinin uygulanışında, kısıtlamaların rolünü basit örnekler ile incelemeye çalışacağız. Generic mimari her ne kadar tür bağımsız algoritmaların geliştirilmesine izin versede, bazı durumlarda çeşitli zorlamaların uygulanmasınıda gerektirir. Örneğin generic olması planlanan tiplerin sadece referans tipleri ile çalışmasını isteyebiliriz. Generic bir tipe her hangibir zorunluluk kuralını uygulayabilmek için where anahtar sözcüğünü içeren bir ek ifade kullanılır. Bu ifadeler 5 adettir ve aşağıdaki tabloda gösterilmektedir."
categories:
  - csharp
tags:
  - csharp
  - dotnet
  - generics
---
# Generic Mimaride Kısıtlamaların(Constraints) Kullanımı
Değerli Okurlarım Merhabalar,

Bu makalemizde.Net 2.0 ile birlikte gelen Generic mimarisinin uygulanışında, kısıtlamaların rolünü basit örnekler ile incelemeye çalışacağız. Generic mimari her ne kadar tür bağımsız algoritmaların geliştirilmesine izin versede, bazı durumlarda çeşitli zorlamaların uygulanmasınıda gerektirir. Örneğin generic olması planlanan tiplerin sadece referans tipleri ile çalışmasını isteyebiliriz. Generic bir tipe her hangibir zorunluluk kuralını uygulayabilmek için where anahtar sözcüğünü içeren bir ek ifade kullanılır. Bu ifadeler 5 adettir ve aşağıdaki tabloda gösterilmektedir.

Koşul
Syntax

Değer tipi olma zorunluluğu
where Tip: struct

Referans tipi olma zorunluluğu
where Tip: class

Constructor zorunluluğu
where Tip: new ()

Türeme zorunluluğu
where Tip:

Interface zorunluluğu
where Tip:

İlk olarak struct ve class zorunluluklarını incelemeye çalışacağız. Bu kısıtlamaları, Generic bir tip içerisinde yer alan tiplerin değer türü veya referans türlerinden mutlaka ve sadece birisi olmasını istediğimiz durumlarda kullanırız. Generic'lik, uygulandığı tip için tür bağımsızlığını ortaya koyan bir yapıdır. Generic mimari sayesinde bir tipin çalışma zamanında kullanacağı üyelerin türünü belirleyebiliriz. Ancak, hangi tür olursa olsun, ya değer türü ya da referans türü söz konusu olacaktır. İşte kısıtlamaları kullanarak, generic mimari üzerinde referans türümü yoksa değer türümü olacağını belirleyebiliriz.

Konuyu daha iyi anlayabilmek için şu örneği göz önüne alalım. Bildiğiniz gibi C# 2.0 beraberinde, Framework 1.1 ile gelen koleksiyonların generic karşılıklarını da getirmiştir. Generic koleksiyonlarlar çalışma zamanında sadece belirtilen türden nesneleri kullanır. Doğal olarka ya değer türlerini ya da referans türlerini kullanılar. Peki ya generic bir koleksiyonun sadece değer türlerini taşımasını istersek ne yapabiliriz. Bir şekilde T tipinin sadece değer türü olması zorunluluğunu bildirmemiz gerekecektir. Aşağıdaki şemada BenimKoleksiyonum isimli özel bir koleksiyon tanımı yer almaktadır. BenimKoleksiyonum isimli generic sınıfımız içeride List türünden bir generic koleksiyonu kullanmaktadır.

![mk142_4.gif](/assets/images/2005/mk142_4.gif)

```csharp
public class BenimKoleksiyonum<T> :IEnumerable<T>
{
    private List<T> icListe = new List<T>();
    public void Ekle(T urun)
    {
        icListe.Add(urun);
    }
    public T Oku(int indis)
    {
        return icListe[indis];
    }
    #region IEnumerable<T> Members

    public IEnumerator<T> GetEnumerator()
    {
        return icListe.GetEnumerator() ;
    }
    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return icListe.GetEnumerator();
    }

    #endregion
}
```

Burada sınıfımız içerisinde yer alan icListe isimli List tipinden generic koleksiyonumuz, T türünden elemanlar ile iş yapacak şekilde tasarlanmıştır. Aynı şekilde, Ekle, Oku ve GetEnumerator metodunun bir versiyonuda sadece T tipinden elemanlar üzerinde iş yapmaktadır. Şimdi BenimKoleksiyonum sınıfına ek olarak aşağıda bilgileri verilen iki yapımız (struct) olduğunu düşünelim.

![mk142_5.gif](/assets/images/2005/mk142_5.gif)

Dvd.cs ve Kitap.cs;

```csharp
struct Kitap
{
    private string mBaslik;
    private double mFiyat;

    public Kitap(string baslik, double fiyat)
    {
        mBaslik = baslik;
        mFiyat = fiyat;
    }
    public override string ToString()
    {
        return mBaslik + " " + mFiyat;
    }
}
struct Dvd
{
    private string mBaslik;
    private double mFiyat;
    private int mSure;

    public Dvd(string baslik, double fiyat,int sure)
    {
        mBaslik = baslik;
        mFiyat = fiyat;
        mSure = sure;
    }
    public override string ToString()
    {
        return mBaslik + " " + mFiyat+" "+mSure;
    }
}
```

Diyelimki BenimKoleksiyonum isimli sınıfın uygulama içerisinde sadec yukarıda bilgileri verilen Kitap, Dvd ve ileride bunlar gibi geliştirilebilecek başka struct'lar ile ilgili işlemler yapmasını istiyoruz. Yani asla ve asla referans tiplerinin kullanmasını istemediğimizi düşünelim. Bu durumda tek yapmamız gereken şey BenimKoleksiyonum isimli sınıfa bir generic Constraint uygulamaktır.

```csharp
public class BenimKoleksiyonum<T> :IEnumerable<T> where T:struct
```

Nasıl ki generic tipi değer türünden olmaya yukarıdaki söz dizimde olduğu gibi zorlayabiliyorsak, aynı işi referans türlerine zorlamak içinde yapabiliriz. Tek yapmamız gereken struct yerine class anahtar sözcüğünü kullanmak olacaktır.

```csharp
public class BenimKoleksiyonum<T> :IEnumerable<T> where T:class
```

Struct zorlamasını kullandığımız takdirde eğer ki, BenimKoleksiyonum sınıfını kod içerisinde herhangibir referans türü ile kullanmaya çalışırsak (örneğin string referans türü ile) derleme zamanında aşağıdaki hata mesajlarını alırız.

![mk142_6.gif](/assets/images/2005/mk142_6.gif)

Örneğimizde kullandığımız değer türü kısıtlaması her ne kadar koleksiyonun sadece struct'ları kullanmasını sağlıyorsada, eğer sadece Kitap ve Dvd gibi kendi tanımladığımız struct'ların kullanılmasına bir zorunluluk getirmemektedir. Nitekim int, double gibi struct'larıda BenimKoleksiyonum ile birlikte kullanabilirsiniz. Bu noktada daha güçlü bir kısıtlama kullanmakta fayda vardır. Tam olarak soy bağımlılığı kısıtlaması bu talebimizi karşılar. Bu kısıtlamaya göre generic olarak kullanılan türün belli bir tipten veya bu tipten türeyen başka tip (tipler) den olması zorunluluğu vardır. Dolayısıyla, ister değer türü ister refarans türü olsun, belli bir tür veya bu türden kalıtımsal olarak türeyen tiplerin kullanılmasını zorunlu hale getirebiliriz. Bu kısıtlamayı uygulamak için aşağıdaki söz dizimi kullanılır.

```csharp
where Tip : <Temel Sınıf>
```

Bu kısıtlamayı anlamak için şu örneği göz önüne alalım. Otomobillere ait çeşitli ürünleri nesneye dayalı mimari altında tasarlamaya çalıştığımızı düşünelim. Her ürünü ayrı bir sınıf olarak tasarlayabiliriz. Lakin pek çok ürünün ortak olan bir takım özellikleri ve işlevleride vardır. Bu tip üyeleri temel bir sınıfta toplayabiliriz. Çok basit olarak aşağıdaki sınıf diagramında görülen bir örnek geliştirelim. Burada Lastik ve Silecek isimli sınıflarımız, UrunTemel sınıfından türemiştir. UrunTemel isimli sınıfımız tüm ürünler için ortak olan ürün kodu, fiyat ve kısa açıklama bilgileri için gerekli özellikleri barındırmaktadır.

![mk142_1.gif](/assets/images/2005/mk142_1.gif)

Yukarıdaki şemada görülen UrunTemel, Lastik ve Silecek isimli sınıflarımıza ait kod satırlarımız ise aşağıdaki gibidir.

UrunTemel.cs

```csharp
using System;
using System.Collections.Generic;
using System.Text;

namespace UsingGenericConstrainst
{
    public class UrunTemel
    {
        private int urunKodu;
        private double urunFiyati;
        private string urunBilgisi;

        public UrunTemel(int kod,double fiyat,string bilgi)
        {
            urunKodu = kod;
            urunFiyati = fiyat;
            urunBilgisi = bilgi;
        }

        public int UrunKodu
        {
            get{return urunKodu;}
        }
        public double BirimFiyat
        {
            get{return urunFiyati;}
        }

        public string UrunTanimi
        {
            get{return urunBilgisi;}
        }
    }
}
```

Lastik.cs

```csharp
using System;
using System.Collections.Generic;
using System.Text;

namespace UsingGenericConstrainst
{
    public class Lastik : UrunTemel
    {
        private int capi;
        private int genislik;
        private string tipi;

        public Lastik(int kodu, double fiyat, string bilgi)
            : base(kodu, fiyat, bilgi)
        {
        }

        public int Cap
        {
            get{return capi;}
            set{capi = value;}
        }

        public int Genislik
        {
            get{return genislik;}
            set{genislik = value;}
        }

        public string Tip
        {
            get{return tipi;}
            set{tipi = value;}
        }

        public override string ToString()
        {
            return UrunKodu.ToString() + " " + BirimFiyat.ToString() + " " + UrunTanimi + " " + Cap.ToString() + " " + Genislik.ToString() + Tip;
        }
    }
}
```

Silecek.cs

```csharp
using System;
using System.Collections.Generic;
using System.Text;

namespace UsingGenericConstrainst
{
    public class Silecek : UrunTemel
    {
        private int uzunluk;

        public Silecek(int kodu, double fiyat, string bilgi)
            : base(kodu, fiyat, bilgi)
        {
        }
        public int Uzunluk
        {
            get{return uzunluk;}
            set{uzunluk = value;}
        }

        public override string ToString()
        {
            return UrunKodu.ToString() + " " + BirimFiyat.ToString() + " " + UrunTanimi + " " + Uzunluk.ToString();
        }
    }
}
```

Şimdi buradaki soy ilişkisini kullanacak tipte bir yönetici sınıf geliştirdiğimizi düşünelim. Urunler isimli bu sınıfımızı değişik tipleri barındırabilecek bir generic koleksiyon ile birlikte kullanacağız. Doğal olarak, Urunler isimli sınıfımızıda generic bir mimaride geliştireceğiz. Urunler isimli sınıfımıza ait şema bilgisini ve kod satırlarını aşağıdaki grafikte görebilirsiniz.

![mk142_2.gif](/assets/images/2005/mk142_2.gif)

Urunler.cs

```csharp
using System;
using System.Collections.Generic;
using System.Text;

namespace UsingGenericConstrainst
{
    public class Urunler<T> // where T : UrunTemel
    {
        private List<T> urunListe;

        public Urunler()
        {
            urunListe = new List<T>();
        }

        public void Ekle(T uye)
        {
            urunListe.Add(uye);
        }
        public void Sil(T uye)
        {
            urunListe.Remove(uye);
        }
        public void Listele()
        {
            foreach (T uye in urunListe)
            {
                Console.WriteLine(uye.ToString());
            }
        }
    }
}
```

Şimdi bu sınıflarımızı Main metodumuzda aşağıdaki kod satırları ile kullanmaya çalışalım.

```csharp
Urunler<Int16> urunler = new Urunler<short>();
urunler.Ekle(13);
urunler.Ekle(15);
urunler.Ekle(24);
urunler.Listele();
```

Uygulamamızı bu haliyle çalıştırdığımızda her hangibir sorun ile karşılaşmayız. Urunler isimli sınıfımız Generic bir yapıda olduğundan Short veri türünden değişkenleri işleyecek şekilde tasarlayabiliriz. Fakat bu istediğimiz bir kullanım şekli değildir. Nitekim biz Urunler isimli sınıfımızın generic olmasını ama sadece Urun grubu ile ilgili türleri işlemesini istemekteyiz. Bu sebepten yorumsal olarak yazdığımız kısıtlama satırını kaldırmamız ve bu sayede Urunler sınıfını sadece UrunTemel soyundan gelecek tiplerin kullanımına açmamız gerekiyor. Uygulamamızı yukarıdaki kodları ile bırakıp, kısıtlamamızı devreye soktuğumuzda, build işleminden sonra aşağıdaki hata mesajları alırız.

![mk142_3.gif](/assets/images/2005/mk142_3.gif)

Urunler isimli işlevsel sınıfımız, generic tip olarak sadece TemelUrun ve soyundan gelen sınıf nesne örnekleri ile çalışabilecek şekilde kısıtlandırıldığı için bu hata mesajları alınmıştır. Ancak uygulama kodlarımızı aşağıdaki gibi değiştirdiğimizde herhangibir problem ile karşılaşmayız.

```csharp
Urunler<UrunTemel> urunler = new Urunler<UrunTemel>();

Lastik lst = new Lastik(1000, 10, "Otomobil Lastiği");
lst.Tip = "Kış Lastiği";
lst.Cap = 185;
lst.Genislik = 75;

Silecek slc = new Silecek(1001, 5, "On silecek takimi");
slc.Uzunluk = 60;

urunler.Ekle(lst);
urunler.Ekle(slc);

urunler.Listele();
```

Generic kısıtlamalar ile ilgili olarak göreceğimiz bir diğer modelde interface uygulama zorunluluğudur. Bu kurala göre, generic mimari içinde kullanılacak olan tür, koşul olarak belirtilen interface'i veya ondan türeyenlerini mutlaka implemente etmiş bir tip olmak zorundadır. Örneğin aşağıdaki mimariyi göz önüne alalım. Bu örnekte bir veritabanı sisteminde yer alan varlıklar çeşitli sınıflar ile temsil edilmeye çalışılmıştır. Bu varlıkların ortak özelliği mutlaka ve mutlaka IGenelVeriIslem isimli arayüzü uyguluyor olmalarıdır.

![mk142_7.gif](/assets/images/2005/mk142_7.gif)

Bizim bu varlıkları yönetecek bir sınıfımız var ise ve bu sınıfı generic bir mimari içerisinde kullanmak istiyorsak sadece IGenelVeriIslem arayüzünü ugulayan tiplerin kullanılmasını da garanti edebiliriz. Tek yapmamız gereken ilgili yönetici sınıfımıza arayüz kısıtlamasını aşağıdaki kod satırlarında görüldüğü gibi eklemek olacaktır.

```csharp
class entYonetici<T> where T:IGenelVeriIslem
{
    // örnek kod satırları
}
```

Burada T tipinin mutlaka IGenelVeriIslem arayüzünü implemente etmiş bir tür olması zorunluluğu getirilmektedir. Böylece yönetici sınıfımız çeşitli tipleri kullanabiliyor olmakla birlikte şu an için sadece ilgili arayüzü uygulayan varlık sınıflarına destek vermektedir.

Generic kısıtlamalar ile ilgili bir diğer özellikte, varsayılan yapıcı metod olması zorunluluğudur. Buna göre generic mimari içerisinde kullanılan bağımsız türün mutlaka varsayılan yapıcı (default constructor-parameterless constructor) metoda sahip olması amaçlanmaktadır. Bunu daha iyi anlayabilmek için, varsayılan yapıcısı olmayan bir tipi, generic olarak kullanmaya çalışmalıyız. Aşağıdaki örneği göz önüne alalım.

![mk142_8.gif](/assets/images/2005/mk142_8.gif)

Bu örnekte, CDKoleksiyon basit olarak tasarlanmış generic tipte bir koleksiyondur. Cd isimli referans tipimizi pekala bu generic koleksiyon içerisinde kullanabiliriz. Lakin Cd isimli sınıfımızın default constructor metodu mevcut değildir. Bunun yerine parametre alan overload edilmiş bir versiyonu kullanılmıştır. CDKoleksiyon sınıfının, taşıyacağı generic tiplerin mutlaka ve mutlaka varsayılan yapıcı metodları içermesini isteyeceğimiz durumlar söz konusu olabilir. Bu zorlamayı gerçekleştirmek için tek yapmamız gereken new kısıtlamasını kullanmak olacaktır.

```csharp
public class CDKoleksiyon<T> where T:new()
{
    private List<T> icListe = new List<T>();

    public void Ekle(T cd)
    {
        icListe.Add(cd);
    }

    public T Oku(int indis)
    {
        return icListe[indis];
    }
}
public class Cd
{
    private int BarkodNo;
    public Cd(int id)
    {
        BarkodNo = id;
    }
}
```

Uygulamada CDKoleksiyon sınıfına ait bir nesne örneğini Cd tipini kullanacak şekilde oluşturmaya çalıştığımızda aşağıdaki ekran görüntüsünde verilen hata mesajlarını alırız. Buna göre, generic tipin mutlaka parametresiz bir constructor kullanması gerektiği derleme zamanında hata mesajı olarak bildirilmektedir.

![mk142_9.gif](/assets/images/2005/mk142_9.gif)

![dikkat.gif](/assets/images/2005/dikkat.gif)
Dilersek generic kısıtlamaların bir kaçını bir arada kullanabiliriz. Örneğin bir generic türün hem belli bir tipten gelmesini hemde struct olmasını sağlayabiliriz. Bu kombinasyonları arttırmamız mümkündür. Buradaki tek şart, eğer varsayılan yapıcı kısıtlamasıda var ise new anahtarının her zaman için en sonda belirtilmesi gerekliliğidir.

Bu sayede, zorlamaları kullanarak tip güvenliğini daha belirleyici şekilde sağlamış oluyoruz. Bu makalemizde generic mimarinin önemli özelliklerinden birisi olan kısıtlamaları incelemeye çalıştık. Kısıtlamalar yardımıyla tür bağımsızlığını kullanırken belirli şartların sağlanmasını zorunlu hale getirebileceğimizi gördük. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.