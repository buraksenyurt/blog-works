---
layout: post
title: "C# 2.0 ve Static Sınıflar"
date: 2005-06-20 12:00:00 +0300
description: "Çoğu zaman uygulamalarımızda, nesne örneğinin oluşturulmasına gerek duymayacağımız üyeleri kullanmak isteriz. Bu amaçla static üyeleri kullanırız. Şimdi bir de sadece static üyelerden oluşacak bir sınıf tasarlamak istediğimizi düşünelim. C# programlama dilinin ilk versiyonunda bu tip bir sınıfı yazmak için dikkat etmemiz gereken bir takım noktalar vardır. Static üyeler kullanılabilmeleri için tanımlı oldukları sınıfın nesne örneğine ihtiyaç duymazlar. Bu sebepten sadece static üyeler içerecek olan bir sınıfın örneklendirilememesi tercih edilecektir. Örneğin aşağıdaki kod parçasını dikkate alalım. TemelAritmetik isimli sınıfımız Toplam isimli static bir metod içermektedir."
categories:
  - csharp
tags:
  - csharp
  - generics
  - visual-studio
---
# C# 2.0 ve Static Sınıflar
Değerli Okurlarım Merhabalar,

Çoğu zaman uygulamalarımızda, nesne örneğinin oluşturulmasına gerek duymayacağımız üyeleri kullanmak isteriz. Bu amaçla static üyeleri kullanırız. Şimdi bir de sadece static üyelerden oluşacak bir sınıf tasarlamak istediğimizi düşünelim. C# programlama dilinin ilk versiyonunda bu tip bir sınıfı yazmak için dikkat etmemiz gereken bir takım noktalar vardır. Static üyeler kullanılabilmeleri için tanımlı oldukları sınıfın nesne örneğine ihtiyaç duymazlar. Bu sebepten sadece static üyeler içerecek olan bir sınıfın örneklendirilememesi tercih edilecektir. Örneğin aşağıdaki kod parçasını dikkate alalım. TemelAritmetik isimli sınıfımız Toplam isimli static bir metod içermektedir.

```csharp
using System;

namespace UsingStaticClasses
{
    public class TemelAritmetik
    { 
        public static double Toplam(double deger1,double deger2)
        {    
            return deger1+deger2;
        }
}

class Class1
{
        static void Main(string[] args)
        {
            double toplamSonuc=TemelAritmetik.Toplam(10,15);
            Console.WriteLine(toplamSonuc);
        }
    }
}
```

Bu kod parçasında aşağıda olduğu gibi TemelAritmetik sınıfına ait bir nesne örneği tanımlayıp Toplam metodunu bu nesne örneği üzerinden çağırmaya çalışmak derleme zamanında hataya neden olacaktır. Az öncede belirttiğimiz gibi, static üyelere tanımlandıkları sınıfa ait nesne örnekleri üzerinden erişilemezler.

```csharp
TemelAritmetik temelAritmetik=new TemelAritmetik();
double toplamSonuc=temelAritmetik.Toplam(4,5);
```

Ancak yinede dikkat ederseniz TemelAritmetik sınıfına ait nesne örneğini oluşturabilmekteyiz. Bu durumda,ilk olarak static üyelerle dolu bir sınıfın kesin olarak örneklendirilmesini önlemek isteyeceğizdir. Bu nedenle varsayılan yapıcı metodu (default constructor) private olarak tanımlayarak bu durumun önüne geçebiliriz.

```csharp
public class TemelAritmetik
{ 
    // Varsayılan yapıcı private olduğundan sınıfa ait nesne örneği oluşturulamayacaktır.
    private TemelAritmetik()
    {
    }

    public static double Toplam(double deger1,double deger2)
    {
        return deger1+deger2;
    }
}
```

Görüldüğü gibi artık TemelAritmetik sınıfına ait nesne örneklerinin üretilmesinin önüne geçtik. Aynı şekilde varsayılan yapıcı metodun private olarak tanımlanması bu sınıfta türetilme yapılmasınıda engellemektedir. Ancak bazen static üyeler içeren sınıfımızın yapıcı metodu public veya protected olarak tanımlanmış olabilir. Bu durumda türetme işlemi gerçekleşebilecektir. Örneğin aşağıdaki kod parçasını ele alalım.

```csharp
using System;

namespace UsingStaticClasses
{
    public class TemelAritmetik
    { 
        public TemelAritmetik()
        {
        }

        public static double Toplam(double deger1,double deger2)
        {
            return deger1+deger2;
        }
    }

    public class AltAritmetik:TemelAritmetik
    {
        public void AltIslemler()
        {
        }
    }

    class Class1
    {
        [STAThread]
        static void Main(string[] args)
        {
            AltAritmetik altAritmetik=new AltAritmetik();
            altAritmetik.AltIslemler();
            double sonuc=AltAritmetik.Toplam(1,2);
        }
    }
}  
```

Dolayısıyla sadece static üyeler içerecek bir sınıfın türetme işlemi için kullanılmasınıda bir şekilde önlemek isteyebiliriz. Bu durumda sealed anahtar sözcüğü yardımıyla, ilgili sınıfın türetme amacıyla kullanılamayacağını belirtiriz.

```csharp
public sealed class TemelAritmetik
```

Bu sadece static üyeler içeren, türetilmeye izin vermeyen ve nesne örneğini kesin olarak oluşturtmak istemediğimiz bir sınıf için acaba yeterli midir? sealed olarak tanımlanmış sınıflar her ne kadar türetme amacıyla kullanılamasada, public erişim belirleyicisine sahip yapıcı metodları nedeni ile örneklendirilebilirler. Bize biraz daha zorlayıcı, kesin kurallar sunacak yeni bir yapı C# 2.0 ile birlikte gelmektedir. C# 2.0 sınıfların static olarak tanımlanabilmesine izin vermektedir. Static sınıflar beraberinde bir takım kurallarıda getirir. Aşağıdaki kod parçası C# 2.0 için örnek bir static sınıf tanımlamasını içermektedir.

```csharp
using System;
using System.Collections.Generic;
using System.Text;

namespace UsingStaticClasses
{
    public static class TemelAritmetik
    {
        public static double Toplam(double deger1,double deger2)
        {
            return deger1+deger2;
        }
        public static double pi = 3.14;
        public static double PI
        {
            get
            {
                return pi;
            }
            set
            {
                pi = value;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            double sonuc=TemelAritmetik.Toplam(3, 5);
            Console.WriteLine(sonuc);
            Console.WriteLine(TemelAritmetik.pi);
            TemelAritmetik.PI = 3;
            Console.WriteLine(TemelAritmetik.PI);
        }
    }
}
```

Örneğimizde içeriğinide static olarak tanımlanmış metod, field ve özellik bulunan bir sınıf görmektesiniz. Bu sınıfın bir önceki versiyona göre en önemli özelliği static olarak tanımlanabiliyor oluşudur. Peki bu sınıfın static olarak tanımlanmasını getirdiği kısıtlamalar nelerdir? İlk olarak bu sınıfa ait bir nesne örneği oluşturmaya çalışalım.

```csharp
TemelAritmetik ta;
TemelAritmetik tAritmetik = new TemelAritmetik();
```

Bu tip bir kod yazımına Visual Studio.2005 zaten intellisense özelliği yardımıyla izin vermeyecektir. Lakin tAritmetik isimli nesne örneğini new operatörü ile oluşturmaya çalıştığımızda TemelAritmetik sınıfının adının listeye gelmediğini görürüz.

![mk124_1.gif](/assets/images/2005/mk124_1.gif)

Diğer yandan kodu derlediğimizde aşağıdaki hataları alırız. Toplam 3 hatamız vardır. İlk satır için verilen hatadan static olarak tanımlanmış bir sınıfa ait nesne tanımlaması yapamadığımızı anlayabiliriz. Diğer yandan, ikinci satırda static sınıfa ait nesne örneğinin oluşturulamayacağı ve aynı zamanda tanımlanamayacağına dair hata mesajlarını alırız.

![mk124_2.gif](/assets/images/2005/mk124_2.gif)

Buradan şu sonuçlara varabiliriz;

![dikkat.gif](/assets/images/2005/dikkat.gif)
Static olarak tanımlanmış sınıflara ait nesne örneklerini üretemeyiz. Ayrıca static sınıflara ait nesne tanımlamalarını da yapamayız.

Static sınıfların sınırlamaları sadece bunlarla sınırlı değildir. Şimdi yukarıdaki static sınıfımıza aşağıda olduğu gibi static olmayan bir kaç yeni üye daha ekleyelim.

```csharp
public static class TemelAritmetik
{
    public TemelAritmetik()
    {
    }
    private int e;
    protected void BilgiVer()
    {
        // Bir takım kodlar.
    }

    public static double Toplam(double deger1,double deger2)
    {
        return deger1+deger2;
    }
    public static double pi = 3.14;
    public static double PI
    {
        get
        {
            return pi;
        }
        set
        {
            pi = value;
        }
    }
}
```

Kaynak kodumuzu derlediğimizde 4 adet derleme zamanı hatası alırız.

![mk124_3.gif](/assets/images/2005/mk124_3.gif)

İlk hatamız static sınıf içerisinde varsayılan yapıcı (default construcor) metod tanımlamaya çalışmaktır. Dilerseniz varsayılan yapıcı metodu aşırı yükleyerek başka versiyonları ile de aynı uygulamayı derlemeyi deneyebilirsiniz. Sonuç hep aynı olacaktır. Elbette doğal olarak yapıcı metodları static olarak tanımlamaya çalışabiliriz ki bu zaten izin verilmeyen bir durumdur.

![dikkat.gif](/assets/images/2005/dikkat.gif)
Static olarak tanımlanmış sınıflar yapıcı metodları (varsayılan yapıcı ve aşırı yüklenmiş versiyonları) içeremez.

Diğer hatalarımız ise, static sınıf içerisinde static olmayan üyeler tanımlamaya çalışmamızdır. Buradan da şu sonuca varabiliriz.

![dikkat.gif](/assets/images/2005/dikkat.gif)
Static olarak tanımlanmış sınıflar sadece static üyeler içerebilir.

İncelememiz gereken bir diğer durum static sınıfların türetilip türetilemiyeceğidir. Aşağıdaki kod parçasını göz önüne alalım.

```csharp
public static class TemelAritmetik
{
    // Static sınıf kodlarımız
}

public class AltAritmetik : TemelAritmetik
{
}
```

Visual Studio.2005 her zamanki gibi bu tarz bir yazıma zaten izin vermeyecektir. Ancak elimizin altında visual studio gibi bir geliştirme ortamı olmadığını ve notepad gibi bir editor yardımıyla bu kodu yazdığımızı düşünecek olursak aşağıdaki hata mesajını alırız.

![mk124_4.gif](/assets/images/2005/mk124_4.gif)

Buradan varacağımız sonuç ise,

![dikkat.gif](/assets/images/2005/dikkat.gif)
Static olarak tanımlanmış sınıflardan başka sınıflar türetilemez.

Burada dikkate değer başka bir durum daha vardır. Static bir sınıftan başka bir sınıfı türetemeyiz. Peki static sınıfı başka bir sınıftan türetebilir miyiz? Cevap basit;

![mk124_5.gif](/assets/images/2005/mk124_5.gif)

Ancak bu durum static sınıflarında mutlaka object sınıfından türediği gerçeğini değiştirmez. Her ne kadar static bir sınıfı başka bir sınıftan türetemessekte onun object sınıfından türediği kesindir. Hata mesajımızda zaten bu durumu bize ispatlamaktadır. Bu noktada aklımıza kurnazca bir teknik gelebilir. Static bir sınıfı başka bir static sınıftan türetmeyi deneyebiliriz. Bu tarz bir kodu denediğimizde aşağıdaki hata mesajlarını alırız.

![mk124_6.gif](/assets/images/2005/mk124_6.gif)

Buna göre şu sonuca varabiliriz;

![dikkat.gif](/assets/images/2005/dikkat.gif)
Static olarak tanımlanmış sınıflardan başka static sınıflar da türetilemez.

Static olarak tanımlanmış sınıfların sadece static üyeler içereceği kesin olmasına rağmen, üyelerinin static anahtar sözcüğü ile tanımlanıyor olması biraz tuhaf bir durum olarak görülebilir. Öyleki izlediğim bir iki blog sitesinde bu durum biraz alaycı ifadeler ile komik olarak ele alınmış. Tabi şu anda test ettiğimiz static class'ların beta 2 sürümüne ait olduğu düşünülecek olursa piyasaya çıkacak olan sürümde bu durum üzerinde iyileştirmeler yapılabilir.

Durumu özetleyecek olursak, sadece static üyeler içermesini düşündüğümüz, örneklenmesini ve türetilmesini kesinlikle istemediğimiz sınıfların tanımlanabilmesi için static anahtar sözcüğünü sınıflar için kullanabiliriz. Bu düşündüğümüz tasarım modelinin uygulamamız için yeterli olacaktır. Geldik bir makalemizin daha sonuna. Bu makalemizde C# 2.0' a gelen yeni özelliklerden brisi olan static sınıfları incelemeye çalıştık. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.