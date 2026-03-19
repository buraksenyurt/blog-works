---
layout: post
title: "C# 2.0 Covariance ve Contravariance Delegates"
date: 2005-06-30 12:00:00 +0300
description: "Bildiğiniz gibi temsilciler (delegates) çalışma zamanında metodların başlangıç adreslerini işaret eden tiplerdir. Bu tipleri uygulamalarımızda tanımlarken çalışma zamanında işaret edebilecekleri metodların geri dönüş tipi ve parametrik yapılarını bildirecek şekilde oluştururuz. Ancak özellikle , C# 1.1 ortamında temsilcilerin kullanımında parametre ve dönüş tipileri açısından iki önemli sıkıntımız vardır. Bu sıkıntıların kaynağında birbirlerinden türeyen yani aralarında kalıtımsal (inheritance) ilişkiler olan tipler yer alır."
categories:
  - csharp
tags:
  - csharp
  - dotnet
  - delegates
---
# C# 2.0 Covariance ve Contravariance Delegates
Değerli Okurlarım Merhabalar,

Bildiğiniz gibi temsilciler (delegates) çalışma zamanında metodların başlangıç adreslerini işaret eden tiplerdir. Bu tipleri uygulamalarımızda tanımlarken çalışma zamanında işaret edebilecekleri metodların geri dönüş tipi ve parametrik yapılarını bildirecek şekilde oluştururuz. Ancak özellikle, C# 1.1 ortamında temsilcilerin kullanımında parametre ve dönüş tipileri açısından iki önemli sıkıntımız vardır. Bu sıkıntıların kaynağında birbirlerinden türeyen yani aralarında kalıtımsal (inheritance) ilişkiler olan tipler yer alır.

Nitekim temsilciler C# 1.1 versiyonunda oluşturulacakları zaman, işaret edecekleri metodlar için kesin dönüş ve parametre tipi uyumluluğunu ararlar. Dolayısıla bu tiplerde kalıtımsal ilişkiler söz konusu olduğunda ortaya çıkan iki temel problem vardır. Bu iki temel sorunumuza bakmadan önce, konunun odağında aralarında kalıtım ilişkisi olan iki sınıf olduğunu göz önüne almalıyız. Örneğin Sekil ve Dortgen isimli iki sınıfımız olduğunu ve Dortgen sınıfının Sekil sınıfından türediğini varsayalım. Bu sınıfların sahip olduğu içeriğin bizim için şu aşamada çok fazla önemi yoktur. İlk olarak covariance'lığa neden olan sorunu ele alalım. Aşağıdaki uygulamamızı dikkatle inceleyelim.

```csharp
using System;

namespace UsingCovariance
{
    public class Sekil
    {
        public Sekil()
        {
        }
    }
    public class Dortgen:Sekil
    {
        public Dortgen()
        {
        }
    }

    public delegate Sekil Temsilci(); 

    class Class1
    {
        public static Sekil Metod_1()
        {
            return null;
        }

        public static Dortgen Metod_2()
        {
            return null;
        }

        [STAThread]
        static void Main(string[] args)
        {
            Temsilci temsilci=new Temsilci(Metod_1);
            temsilci=new Temsilci(Metod_2); // Derleme zamanı hatası
        }
    }
}
```

Bu uygulamayı derlediğimizde,

```csharp
temsilci=new Temsilci(Metod_2);
```

satırı için Method 'UsingCovariance.Class1.Metod2 ()' does not match delegate 'UsingCovariance.Sekil UsingCovariance.Temsilci ()' hatasını alırız. Peki burada sorun nedir? Temsilci isimli delegate tipimiz, Sekil sınıfından nesne örneklerini geriye döndüren ve parametre almayan metodları işaret edebilecek şekilde tanımlanmıştır. Bu durumda,

```csharp
Temsilci temsilci=new Temsilci(Metod_1);
```

satırı düzgün olarak çalışacaktır. Nitekim Metod_1 delegate tipimizin tanımlamalarına uyan tarzda bir metoddur. Oysaki Metod_2 metodumuzun geriye döndürdüğü değer Dortgen sınıfı tipindendir. Dolayısıyla temsilci nesnemizin tanımladığı bildirimin dışında bir dönüş tipininin dönüşü söz konusudur. Hatırlayın, temsilciler işaret edecekleri metodlar için kesin dönüş tipi ve parametre tipi uyumluluğu ararlar. Oysaki Dortgen ve Sekil sınıfı arasında kalıtımsal bir ilişki söz konusudur ve bu sebeple bu tarz bir kullanımın sorunsuz olarak çalışacağı düşünülmektedir. Çünkü kalıtımın doğası gereği bu Dortgen sınıfına ait nesne örnekleri Sekil sınıfına ait nesne örneklerine dönüştürülebilir. Ancak temsilcimiz açısından bu kural ne yazık ki geçerli değildir. Yani temsilcimiz belirtilen tipler için çalışma zamanında çok biçimliliği destekleyememiştir.

Peki bu sorunu nasıl çözebiliriz? Tek yapabileceğimiz Dortgen sınıfına ait nesne örneklerini döndüren Metod_2 isimli metodumuzu işaret edecek yeni bir delegate nesnesini aşağıdaki örnek kod parçasında olduğu gibi tanımlamak ve kullanmak olacaktır. Bu tahmin edeceğiniz gibi hantal bir çözümdür. Bir birleri arasında tür dönüşümü yapılabilecek iki sınıf için gereksiz yere iki ayrı temsilci tipi tanımlamak zorunda kalışımız dahi istenmeyen bir durumdur.

```csharp
public delegate Sekil Temsilci();
public delegate Dortgen Temsilci2();

class Class1
{
    public static Sekil Metod_1()
    {
        return null;
    }

    public static Dortgen Metod_2()
    {
        return null;
    }

    [STAThread]
    static void Main(string[] args)
    {
        Temsilci temsilci=new Temsilci(Metod_1);
          Temsilci2 temsilci2i=new Temsilci2(Metod_2);
    }
}
```

İşte C# 2.0' da, delegate tipi için söz konusu olan bu sorun ortadan kaldırılmıştır. Yukarıdaki örneğimizin aynısını C# 2.0' da yazdığımızı düşünürsek her hangi bir derleme zamanı hatası almayız. Covariance'ın desteklenebilmesi için C# 2.0' a getirilen ekstra bir kodlama tekniği yoktur.

![mk127_1.gif](/assets/images/2005/mk127_1.gif)

Bu tamamen.net çekirdeğinde delegate tipi üzerinde yapılan bir düzenlemenin sonucudur.

![dikkat.gif](/assets/images/2005/dikkat.gif)
Covariance, temsilcilerin çalışma zamanında işaret etmek istediği metodların, aralarında kalıtımsal ilişki olan dönüş tipleri arasındaki poliformik uyum sorununu ortadan kaldıran bir özellik olarak nitelendirilebilir.

Temsilci nesnelerimizin işaret edeceği metodların dönüş tiplerinin kalıtımsal ilişkilere izin verecek şekilde düzenlenmiş olması elbetteki çalışma zamanında bize büyük bir esneklik getirmektedir. Nitekim bu sayede birbirlerinden türemiş n sayıda sınıfa ait nesne örneklerinin dönüş tipi olarak kullanıldığı metodları tek bir delegate nesnesi vasıtasıyla çalışma zamanında işaret edebiliriz.

Gelelim contravariance durumuna. Bu kez kalıtım ilişkisine sahip olan sınıf örnekleri, temsilcilerin işaret edeceği metodların parametrik yapıları içerisinde kullanılmaktadır. Aşağıdaki örnek kod parçası C# 1.1 için bu durumu örneklemektedir.

```csharp
using System;

namespace UsingContravariance
{
    public class Sekil
    {
        public Sekil()
        {
        }
    }
    public class Dortgen:Sekil
    {
        public Dortgen()
        {
        }
    }

    public delegate int Temsilci(Dortgen dortgen);

    class Class1
    {
        public static int Metod_1(Dortgen dortgen)
        {    
            return 0;
        }
        public static int Metod_2(Sekil sekil)
        {
            return 0;
        }

        [STAThread]
        static void Main(string[] args)
        {
            Temsilci temsilci=new Temsilci(Metod_1);
            temsilci=new Temsilci(Metod_2); // Derleme zamanı hatası
        }
    }
}
```

Bu kez delegate tipimiz geriye int tipinden değer döndüren ve parametre olarak Dortgen sınıfı tipinden nesne örneklerini alan metodları işaret edebilecek şekilde tanımlanmıştır. Kodu derlediğimizde Method 'UsingContravariance.Class1. Metod2 (UsingContravariance.Sekil)' does not match delegate 'int UsingContravariance.Temsilci (UsingContravariance.Dortgen)' hatasını alırız. Yine delegate tipimiz burada parametrik imza uyuşmazlığından bahsetmektedir. Sorun,

```csharp
temsilci=new Temsilci(Metod_2);
```

satırında oluşur. Çünkü Metod_2 Dortgen sınıfı tipinden bir nesne örneğini parametre olarak almaktansa Dortgen sınıfının üst sınıfı olan Sekil sınıfından bir nesne örneğini parametre olarak almaktadır. Buradaki problem tersi durum içinde söz konusudur. Yani temsilcimizi tanımlarken metodun alacağı parametrenin, türeyen sınıf yerine temel sınıftan (base class) bir nesne örneğini kullanacak şekilde aşağıdaki kod parçasında görüldüğü gibi tanımlandığını düşünürsek;

```csharp
public delegate int Temsilci(Sekil sekil);

class Class1
{
    public static int Metod_1(Dortgen dortgen)
    {
        return 0;
    }
    public static int Metod_2(Sekil sekil)
    {
        return 0;
    }

    [STAThread]
    static void Main(string[] args)
    {
        Temsilci temsilci=new Temsilci(Metod_1); // Derleme zamanı hatası
        temsilci=new Temsilci(Metod_2);
    }
}
```

bu sefer Metod_1 için oluşturulan temsilci isimli nesnemiz üzerinden aynı hata mesajını alırız. Doğal olarak, metod parametlerinin üst sınıftan veya türeyen sınıftan olması durumu değiştirmeyecektir.

![dikkat.gif](/assets/images/2005/dikkat.gif)
Contravariance, temsilcilerin çalışma zamanında işaret etmek istediği metodların parametreleri arasında kalıtımsal ilişkiye sahip tiplerin neden olduğu polimorfik uyum sorununu ortadan kaldıran bir özellik olarak nitelendirilebilir.

Parametrelerin uyumsuzluğunun neden olduğu bu sorunu C# 1.1 ile çözmek için, covariance tekniğinde olduğu gibi her bir metod için ayrı temsilci nesnelerini aşağıdaki kod parçasında olduğu gibi tanımlamamız gerekecektir.

```csharp
public delegate int Temsilci(Sekil sekil);
public delegate int Temsilci2(Dortgen dortgen);

class Class1
{
    public static int Metod_1(Sekil sekil)
    {
        return 0;
    }
    public static int Metod_2(Dortgen Dortgen)
    {
        return 0;
    }

    [STAThread]
    static void Main(string[] args)
    {
        Temsilci temsilci=new Temsilci(Metod_1);
          Temsilci2 temsilci2=new Temsilci2(Metod_2);
    }
}
```

Yukarıdaki örneğimizi C# 2.0 ile derlediğimizde ise her hangi bir sorun olmadığını görürüz.

![mk127_2.gif](/assets/images/2005/mk127_2.gif)

Parametrelerin arasındaki kalıtımsal ilişki, çalışma zamanında temsilciler oluşturulurken de değerlendirilecek ve nesneler arasındaki tür dönüşümü başarılı bir şekilde parametrelerede yansıyacaktır. Bu covarince probleminde olduğu gibi bize yine büyük bir esneklik sağlar. Aralarında kalıtımsal ilişki olan n sayıda sınıf nesne örneğini kullanan metodları tek bir temsilci nesnesi ile çalışma zamanında işaret edebilme yeteneği. Böylece geldik bir makalemizin daha sonuna, bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.