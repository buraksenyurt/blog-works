---
layout: post
title: "Virtual(Sanal) Metotlar"
date: 2003-12-25 10:00:00 +0300
categories:
  - csharp
tags:
  - csharp
---
Bugünkü makalemizde sanal metotların kalıtım içerisindeki rolüne bakacağız. Sanal metotlar, temel sınıflarda tanımlanan ve türeyen sınıflarda geçersiz kılınabilen metotlardır. Bu tanım bize pek bir şey ifade etmez aslında. O halde gelin sanal metodların neden kullanırız, once buna bakalım. Bu amaçla minik bir örnek ile işe başlıyoruz.

```csharp
using System;
namespace

ConsoleApplication1
{
    public class Temel
    {
        public Temel()
        {

        }

        public void Yazdir()
        {
            Console.WriteLine("Ben TEMEL(BASE) sinifim");
        }

    }
    public class Tureyen : Temel
    {
        public Tureyen()
        {

        }
        
        public void Yazdir()
        {
            Console.WriteLine("Ben TUREYEN(DERIVED) sinifim");
            }
    }
    
    class Class1
    {
        static void Main(string[] args)
        {
            Temel bs;
            Tureyen drv =new Tureyen();
            bs = drv;
            bs.Yazdir();
        }
    }
}
```

Bu örneği çalıştırmadan once satırlarımızı bir inceleyelim. Kodumuz Temel isimli bir base class ve Tureyen isimli bir Derived Class vardır. Her iki sınıf içinde Yazdir isimli iki metod tanımlanmıştır. Main metodu içinde Temel sınıftan türettiğimiz bir nesneye (bs nesnesi) Tureyen sınıf tipinden bir nesneyi (drv nesnesi) aktarıyoruz. Ardından bs nesnemizin Yazdir isimli metodunu çağırıyoruz. Sizce derleyici hangi sınıfın yazdır metodunu çağıracaktır.

Drv nesnemiz Tureyen sınıf nesnesi olduğundan ve Temel sınıftan kalıtımsal olarak türetildiğinden bs isimli nesnemize aktarılabilmiştir. Şu durumda bs isimli Temel sınıf nesnemiz drv isimli Tureyen sınıf nesnemizi taşımaktadır. Bu tip bir atamam böyle base-derived ilişkide sınıflar için geçerli bir atamadır. Sorun bs isimli nesne için Yazdir metodunun çağırılmasındadır. Biz burada Tureyen sınıf nesnesini aktardığımız için bu sınıfa ait Yazdir metodunun çalıştırılmasını bekleriz. Oysaki sonuç aşağıdaki gibi olucaktır.

![mk27_1.gif](/assets/images/2003/mk27_1.gif)

Şekil 1. Temel Sınıfın Yazdir metodu çağırıldı.

Görüldüğü gibi Temel sınıfa ait Yazdir metodu çalıştırılmıştır. Bir çözüm olarak daha önceki kalıtım kavramını anlattığımız makalemizde inceledeğimiz new anahtar kelimesini Tureyen isimli derived class içinde kullanmayı düşünebilirsiniz. Birde böyle deneyelim, bakalım neler olucak.

```csharp
using System;

namespace ConsoleApplication1
{
    public class Temel
    {
        public Temel()
        {
        }

        public void Yazdir()     
        {
            Console.WriteLine("Ben TEMEL(BASE) sinifim");
        }
    }
    public class Tureyen : Temel
    {
        public Tureyen()
        {

        }
        public new void Yazdir()
        {
            Console.WriteLine("Ben TUREYEN(DERIVED) sinifim");
        }
    }
    class Class1
    {
        static void Main(string[] args)
        {
            Temel bs;
            Tureyen drv =new Tureyen();
            bs = drv;
            bs.Yazdir();
        }
    }
}
```

Ancak new anahtar kelimesini kullanmış olsakta sonuç yine aynı olucaktır ve aşağıdaki görüntüyü alacağızdır.

![mk27_1.gif](/assets/images/2003/mk27_1.gif)

Şekil 2. Yine style='mso-spacerun:yes'> Temel Sınıfın Yazdir metodu çağırıldı.

İşte bu noktada çözüm Temel sınıftaki metodumuzu Virtual (sanal) tanımlamak ve aynı metodu, Tureyen sınıf içersinde Override (Geçersiz Kılmak) etmektir. Sanal metodların kullanım amacı budur; Base sınıfta yer alan metod yerine base sınıfa aktarılan nesnenin üretildiği derived class’taki metodu çağırmaktır.

Şimdi örneğimizi buna gore değiştirelim.

```csharp
using System;

namespace ConsoleApplication1
{
    public class Temel
    {
        public Temel()
        {
        }
        public virtual void Yazdir()
        {
            Console.WriteLine("Ben TEMEL(BASE) sinifim");
        }
    }

    public class Tureyen : Temel
    {
        public Tureyen()
        {
        }        

        public override void Yazdir()
        {
            Console.WriteLine("Ben TUREYEN(DERIVED) sinifim");
        }
    }

    class Class1
    {
        static void Main(string[] args)
        {
            Temel bs;
            Tureyen drv =new Tureyen();
            bs = drv;
            bs.Yazdir();
        }
    }
}
```

![mk27_2.gif](/assets/images/2003/mk27_2.gif)

Şekil 3. Tureyen sınıftaki Yazdir metodu çağırılmıştır.

Burada önemli olan nokta, Temel sınıfaki metodun virtual anahtar kelimesi ile tanımlanması, Tureyen sınıftaki metodun ise override anahtar kelimesi ile tanımlanmış olmasıdır. Sanal metodları kullanırken dikkat etmemiz gereken bir takım noktalar vardır. Bu noktalar;

1
İki metodda aynı isime sahip olmalıdır.

2
İki metodda aynı tür ve sayıda parametre almalıdır.

3
İki metodunda geri dönüş değerleri aynı olmalıdır.

4
Metodların erişim haklarını aynı olmalıdır. Biri public tanımlanmış ise diğeride public olmalıdır.

5
Temel sınıftaki metodu türeyen sınıfta override (geçersiz) hale getimez isek metod geçersiz kılınamaz.

6
Sadece virtual olarak tanımlanmış metodları override edebiliriz. Herhangibir base class yöntemini tureyen sınıfta override edemeyiz.

Bu noktalara dikkat etmemiz gerekmektedir. Değerli Okurlarım, geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.