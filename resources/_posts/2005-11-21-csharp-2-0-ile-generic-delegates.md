---
layout: post
title: "C# 2.0 ile Generic Delegates"
date: 2005-11-21 10:00:00 +0300
description: "Bu makalemizde generic temsilcilerin (generic delegates) ne olduğunu ve nasıl kullanılabildiğini incelemeye çalışacağız. .Net 2.0 ile gelen en önemli yenelik generic mimarisidir. Generic mimarisi, tür bağımsız algoritmalar kurmamıza imkan sağlayan gelişmiş bir yapıdır. .Net 2.0' da sınıfları (class), yapıları (struct), arayüzleri (interface), metodları (method), koleksiyonları (collection) ve temsilcileri (delegate) generic olarak oluşturabilir ve kullanabiliriz. Bildiğiniz gibi generic mimarisinin sağlamış olduğu iki önemli özellik vardır. Bunlar tip güvenliği (type-safety) ve performans artışıdır. Özellikle performans ile kastedilen konu gereksiz boxing ve unboxing işlemlerinin ortadan kaldırılabilmesidir. Generic mimarinin getirdiği bu avantajları delegate (temsilci) tipi içinde kullanabilmekteyiz."
categories:
  - csharp
tags:
  - csharp
  - dotnet
  - performance
  - delegates
  - generics
---
# C# 2.0 ile Generic Delegates
Değerli Okurlarım Merhabalar,

Bu makalemizde generic temsilcilerin (generic delegates) ne olduğunu ve nasıl kullanılabildiğini incelemeye çalışacağız..Net 2.0 ile gelen en önemli yenelik generic mimarisidir. Generic mimarisi, tür bağımsız algoritmalar kurmamıza imkan sağlayan gelişmiş bir yapıdır..Net 2.0' da sınıfları (class), yapıları (struct), arayüzleri (interface), metodları (method), koleksiyonları (collection) ve temsilcileri (delegate) generic olarak oluşturabilir ve kullanabiliriz. Bildiğiniz gibi generic mimarisinin sağlamış olduğu iki önemli özellik vardır. Bunlar tip güvenliği (type-safety) ve performans artışıdır. Özellikle performans ile kastedilen konu gereksiz boxing ve unboxing işlemlerinin ortadan kaldırılabilmesidir. Generic mimarinin getirdiği bu avantajları delegate (temsilci) tipi içinde kullanabilmekteyiz.

İlk olarak generic temsilcilere neden ihtiyacımız olabileceğini 1.1 versiyonundaki kullanımını göz önüne alarak irdelemeye çalışalım. Aşağıdaki örnek uygulamada overload edilmiş Toplam isimli iki metod yer almaktadır. Bu iki metod farklı tipte parametreler almaktadır. Bunun yanında dönüş tipleride farklıdır. Bu metodları temsilci nesneleri ile işaret etmek istediğimizde iki ayrı temsilci tipi tanımlamamız gerekecektir. Nitekim temsilci tipi, tanımlamış olduğu desen ile (dönüş tipi ve metod imzası) birebir uyumlu metodları işaret edebilmektedir.

```csharp
using System;

namespace UsingGenericDelegates
{
    #region Temsilci tipleri tanımlanır

    public delegate float TemsilciFloat(float a,float b);
     public delegate int TemsilciInt(int a,int b);

    #endregion

    public class TemelAritmetik
    {
        public TemelAritmetik() {}
    
        public float Toplam(float x,float y)
        {
            return x+y;
        }

        public int Toplam(int x,int y)
        {
            return x+y;
        }
    }
}
```

TemelAritmetik sınıfımıza ait Toplam metodlarını çalışma zamanında belirlediğimiz temsilci nesneleri ile aşağıdaki kod parçasında olduğu gibi ilişkilendirebiliriz.

```csharp
using System;

namespace UsingGenericDelegates
{
    class TestClass
    {
        [STAThread]
        static void Main(string[] args)
        {
            TemelAritmetik ta=new TemelAritmetik();
        
            #region Temsilciler Oluşturulur
        
            TemsilciInt tint=new TemsilciInt(ta.Toplam);
            TemsilciFloat tFloat=new TemsilciFloat(ta.Toplam);

            #endregion

            int toplamInt=tint(1,2);
            float toplamFloat=tFloat(1.2f,1.2f);
    
            Console.WriteLine(toplamInt.ToString());
            Console.WriteLine(toplamFloat.ToString());

        }
    }
}
```

Bu uygulama sorunsuz şekilde çalışmaktadır. Ancak benzer işlevlere sahip olan yani int ve float tipinden verileri toplayıp bu türde geri döndüren metodları çalışma zamanında ilgili temsilci nesne örnekleri ile işaret edebilmek için iki ayrı delegate tipi tanımlamamız gerekmiştir. İşte.Net 2.0 ile birlikte gelen generic mimarisi sayesinde tek bir temsilci tipi tanımını kullanarak birden fazla metodu istenilen veri tipi için kullanabiliriz. Başka bir deyişle, yukarıdaki örnekte olduğu gibi iki farklı delegate tipi tanımlamak yerine generic yapıda tek bir temsilci tipi (delegate type) tanımlayıp, bu tip üzerinden çalışma zamanında uygun metodları çağırabiliriz.

.Net 2.0 için generic temsilci (generic delegate) tanımlamasında da anahtar nokta, açısal ayraçlar (<>) arasına yazılan tür belirtme operatörüdür. Örneğin, void dönüş tipine sahip ama parametre tipleri belli olmayan (başka bir deyişle çalışma zamanında belli olacak olan) bir temsilciyi aşağıdaki gibi tanımlayabiliriz.

```csharp
public delegate void TemsilciX<T>(T deger);
```

Bu ifadede, TemsilciX isimli delegate tipinin çalışma zamanında geri dönüş tipi olmayan (void) ve her hangibir tipte tek bir parametre alan metodları işaret edebileceğini söylemiş oluyoruz. Örneğimizde bu metod desenine uyan aşağıdaki Yaricap fonksiyonumuzun olduğunu düşünelim.

```csharp
public void Yaricap(double r)
{
    Console.WriteLine("yarıçap ={0}", r);
}
```

Buna göre TemsilciX delegate tipini uygulamamızda aşağıdaki gibi kullanabiliriz.

```csharp
TemsilciX<double> tX = new TemsilciX<double>(ta.Yaricap); // Temsilci nesne örneği oluşturulur.
tX(1.3); // Temsilcimizin işaret ettiği metod çağırılır.
```

Bu kod parçasında tX örneğini oluştururken, çalışma zamanında double tipinden parametre alacak metodları işaret edebileceğini belirtmiş oluyoruz. Diğer yandan örneğin, int tipinden bir parametre alıp void dönüş tipine sahip başka bir metodu işaret etmek istediğimizde, TemsilciX deleagate tipini buna göre tekrardan örneklendirebiliriz.

![mk141_1.gif](/assets/images/2005/mk141_1.gif)

Böylece tek bir temsilci nesnesini kullanarak çalışma zamanında kendi belirlediğimiz tipleri kullanan metodları işaret edebiliriz. Generic temsilcileri kullanarak, işaret edecekleri metodların dönüş tiplerinin çalışma zamanında ne olacağını da belirleyebiliriz. Makalemizin başındaki örneği dikkate aldığımızda Toplam metodunun her iki versiyonununda farklı tipten geri dönüş değerlerine sahip olduğunu görmekteyiz. Buna göre, generic temsilci tipimizi aşağıdaki gibi tanımlayabiliriz. Bu sefer, her metod için ayrı birer temsilci tipi tanımlamaktansa, tek bir generic temsilci tipi tanımı işimizi görecektir.

```csharp
public delegate R Temsilci<T,R>(T deger1,T deger2);
```

Burada R harfi ile temsilcimizin çalışma zamanında işaret edeceği metodun dönüş tipini belirtmiş oluruz. T harfi ilede, metodun alacağı parametrelerin tipini belirliyoruz. Bu değişiklikeri göz önüne aldığımızda, makalemizin başındaki örneğimizi aşağıdaki haliyle güncelleyebiliriz.

```csharp
using System;

namespace UsingGenericDelegates
{
    // Temsilci tipimiz tanımlanır.
    public delegate R Temsilci<T, R>(T deger1, T deger2);

    class TestClass
    {
        [STAThread]
        static void Main(string[] args)
        {
            TemelAritmetik ta=new TemelAritmetik();

            #region temsilcimize ait nesne örnekleri oluşturuluyor
    
            Temsilci<float, float> t1 = new Temsilci<float, float>(ta.Toplam); 
            Temsilci<int,int> t2=new Temsilci<int,int>(ta.Toplam);

            #endregion
    
            Console.WriteLine(t1(1.2f, 1.3f));
            Console.WriteLine(t2(1,2));
        }
    }
}
```

Her ne kadar t1 ve t2 olmak üzere TemelAritmetik sınıfı içerisindeki Toplam metodlarının her biri için ayrı ayrı iki delegate nesnesi örneklemiş olsakta; bu nesne örneklerinin ait olduğu delegate tipi tektir. Uygulamayı çalıştırdığımızda aşağıdaki ekran görüntüsünü elde ederiz.

![mk141_2.gif](/assets/images/2005/mk141_2.gif)

![dikkat.gif](/assets/images/2005/dikkat.gif)
Tek bir generic temsilci tipi ile, çalışma zamanında değişik dönüş tipi ve parametrelere sahip metodları işaret edebiliriz.

Temsilcilerin sağlamış olduğu tip güvenliği (type-safety) ve performans artışı gibi faydalar dışında özellikle kendi tiplerimizi kullandığımız takdirde devreye giren kısıtlamalar (constraints), delegate tipleri içinde geçerlidir. Bildiğiniz gibi generic yapılarda Where anahtar sözcüğü yardımıyla, parametre tiplerine ilişkin kesin bazı kurallar koyabiliyoruz. Örneğin TemelAritmetik isimli sınıfımızda aşağıdaki gibi iki string tipte parametre alan bir metodumuz olduğunu düşünelim.

```csharp
public string Toplam(string x, string y)
{
    return x + y;
}
```

Böyle bir durumda, Temsilci delegate tipimiz ile çalışma zamanında bu metodu aşağıdaki kod parçasında olduğu gibi işaret edebiliriz.

```csharp
Temsilci<string, string> t3 = new Temsilci<string, string>(ta.Toplam);
Console.WriteLine(t3("Burak", "Selim"));
```

Burada önemli olan nokta Toplam metodunun bu versiyonunun string tipinde, (bir başka deyişle referans tipinde) parametreler alıyor oluşudur. Oysaki biz temsilcimizin sadece değer türünde (value types) parametreler alan metodlar işaret etmesini de isteyebiliriz. İşte böyle bir sorunu, where anahtar sözcüğü ile parametre tiplerine yönelik (veya dönüş tipine yönelik) kısıtlamalar girerek aşabiliriz. Dolayısıyla Temsilci isimli delegate tipimizi aşağıdaki gibi tanımlamamız yeterli olacaktır.

```csharp
public delegate R Temsilci<T, R>(T deger1, T deger2) where T:struct where R:struct;
```

Burada where anahtar sözcüklerini kullanarak T ve R türlerinin mutlaka struct tipinde olmaları gerektiğini, bir başka deyişle değer türü olmaları gerektiğini belirtmiş oluyoruz. Dolayısıyla uygulamamızı bu haliyle derlemek istediğimizde aşağıdaki şekilde görüldüğü gibi, derleme zamanı hata mesajlarını alırız.

![mk141_3.gif](/assets/images/2005/mk141_3.gif)

Yaptıklarımızı kısaca gözden geçirecek olursak generic temsilcilerin sağlamış olduğu avantajları şu şekilde sıralayabiliriz.

Temsilci tiplerini tür bağımsız olacak şekilde tanımlayabiliriz.

Where anahtar sözcüğünün imkanlarından faydalanarak temsilci tiplerine ait parametre ve dönüş değerleri üzerinde tip güvenliğini (type-safety) daha üst düzeyde sağlayabiliriz.

Çalışma zamanında farklı parametre tiplerini kullanan uygun metodlar için ayrı ayrı temsilci tipleri tanımlamak zorunda kalmayız.

Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.