---
layout: post
title: "Generic Lazy Tipi Olmasaydı"
date: 2010-10-18 02:05:00 +0300
categories:
  - csharp-4-0
tags:
  - csharp
  - base-class-library
  - lazy-initialization
  - generics
  - singleton-design-pattern
  - lock
  - thread-safe
---
Aklım üniversite kampüsünün çimlerinde hala...Pek çok gün İstinye Park'a giden öğle arası servisimiz, dönüş yolunda İTÜ kütüphanesi önünden geçmekte (Geçmekte idi...Sonrasında o yol trafiğe kapatıldı) Öğrencilerin çimlere yayılarak mavi gök yüzünü seyre dalmasına her zaman imreniyorum. Çoğu zaman bu psikolojideki öğrencinin kafasında oluşan sorunlar bellidir. Kız arkadaş veya erkek arkadaş sorunu, maddi sorunlar, dersler, vize ve finaller...Tabi iş hayatına giren ben gibi insanlar aynı çimlere yatmaya kalksa, kafada dönen sorunların sayısı azalacağına büyük ihtimalle artacaktır. Hele ki yazılımla uğraşıyorsanız mavi göğü seyre dalarken geçen bulutların çoğu birer Component haline dönüşecek ve üstünüze üstünüze gelecektir. Neyse...

![blg195_Giris.jpg](/assets/images/2010/blg195_Giris.jpg)

Hatırlayacağınız üzere [Tembellik Etmek İstiyorum (Generic Lazy Tipi ile Et)](https://www.buraksenyurt.com/post/Tembellik-Etmek-Istiyorum-(Generic-Lazy-Tipi-ile-Et).aspx) başlıklı yazımızda.Net Framework 4.0 ile birlikte Base Class Library'ye eklenen yeni tiplerden birisi olan Lazy sınıfını irdelemeye çalışmıştık. Bu sınıfın T tipi için gerçek anlamda Lazy Initialization yaptığını örnekler üzerinden öğrendik ve yine hatırlayacağınız üzere Lazy nesne örnekleri üzerinden Value özelliği çağırılmadığı sürece T tipinden değer döndüren bir operasyonun çağırılması söz konusu değildir. Üstelik Value özelliğine sonradan yapılan çağrılarda T dönüşü yapan operasyonların tekrardan çağırılmadığı da bilinmektedir. Şimdi Lazy tipini kullanmadan bir sınıf özelliğinin Lazy Initializate işleminde değerlendirilip değerlendirilemeyeceğini düşünerek yola koyulalım. Bu amaçla aşağıdaki gibi bir kod örneğini göz önüne alabiliriz.

```csharp
using System.Collections.Generic;

namespace LazyPart2
{
    class Program
    {
        static void Main(string[] args)
        {
            var productList=ProductCreater.Products;
            var productListAgain=ProductCreater.Products; // Bu çağrı sonrasında ProductCreator içerisindeki products alanının yeniden örneklenmediği görülecektir
        }
    }

    class ProductCreater
    {
        private static readonly List<Product> products = new List<Product>
        {
            new Product{ProductId=1,Name="Product X",ListPrice=1},
            new Product{ProductId=2,Name="Product Y",ListPrice=3},
        };

        private ProductCreater()
        {
        }

        public static List<Product> Products
        {
            get
            {
                return products;
            }
        }
    }

    class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public double ListPrice { get; set; }
    }
}
```

ProductCreator isimli sınıf new operatörü ile örneklenemeyecek şekilde tasarlanmıştır. products isimli field değişkeni static olarak tanımlanmıştır ve ilk değerleri atanmıştır. ProductCreator tipi üzerinden kullanılmak istenen ürün listesini alabilmek için static Products özelliğinin kullanılması yeterlidir. Bu kod debug edildiğinde Products özelliğine yapılan çağrılardan ilkinde, products isimli field'ın oluşturulduğu görülecektir. Yani ilk değerler yüklenecektir. İkinci çağrıda ise yeniden bir örnekleme söz konusu olmayacaktır. Bu da aslında tek bir products alanının olmasının garantilendiğini gösterir. Hatta şu anki tasarıma göre ProductCreator üzerinden çağırılabilen tek public üye Products özelliği olduğundan ürün listesinin kullanılmak istendiği yerde initialize olacağı sonucuna varabiliriz. Ki bu büyük bir aldatmaca olacaktır. Ne demek istediğimizi daha iyi anlamak için kod içeriğini aşağıdaki gibi değiştirdiğimizi düşünelim.

```csharp
using System.Collections.Generic;

namespace LazyPart2
{
    class Program
    {
        static void Main(string[] args)
        {
            ProductCreater.DoSomething();
            var productList=ProductCreater.Products;
            var productListAgain=ProductCreater.Products; // Bu çağrı sonrasında ProductCreator içerisindeki products alanının yeniden örneklenmediği görülecektir
        }
    }

    class ProductCreater
    {
        private static readonly List<Product> products = new List<Product>
        {
            new Product{ProductId=1,Name="Product X",ListPrice=1},
            new Product{ProductId=2,Name="Product Y",ListPrice=3},
        };

        private ProductCreater()
        {
        }

        public static List<Product> Products
        {
            get
            {
                return products;
            }
        }

        public static void DoSomething()
        {
            System.Console.WriteLine("Do Something");
        }
    }

    class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public double ListPrice { get; set; }
    }
}
```

Dikkat edileceği üzere ProductCreator sınıfı içerisine, geriye bir şey döndürmeyen ve aslında anlamlı bir işte yapmayan static DoSomething metodu eklenmiştir. Bu metod modelin çökmesi için yeterlidir. Çünkü ProductCreator.Products için yapılan ilk çağrıdan önce gerçekleştirilen ProductCreator.DoSomething operasyonu, products alanının initialize edilmesine neden olmaktadır. Bu da products alanının kullanılmak istendiği zaman initialize olacağı teorisini ilk tasarladığımız sınıf yapısı düşünüldüğünde çökertmektedir. Örnek debug edildiğinde bu durumda daha net bir şekilde görülmektedir.

![blg195_Debug1.gif](/assets/images/2010/blg195_Debug1.gif)

İşleyiş sırasına baktığımızda DoSomething çağrısını takiben 17nci satıra gelindiği görülmektedir ki burada da product isimli alanının doldurulması işlemleri söz konusudur. Aslında MSDN araştırması yapıldığında IL tarafında ProductCreator sınıfı için beforefieldinit isimli tanımlamanın yapılmasının bu initialize işlemine neden olduğu ifade edilmektedir. Gerçekten öyle midir acaba? Bu tanımlamayı bir şekilde kaldırmayı başarırsak static products alanı sadece çağırıldığı zaman mı doldurulacaktır. Önce IL tarafındaki tanımlamaya bir bakalım.

```bash
.class private auto ansi beforefieldinit ProductCreater
    extends [mscorlib]System.Object
{
    .method private hidebysig specialname rtspecialname static void .cctor() cil managed
    {
    }
    .method private hidebysig specialname rtspecialname instance void .ctor() cil managed
    {
    }
    .method public hidebysig static void DoSomething() cil managed
    {
    }
    .property class class [mscorlib]System.Collections.Generic.List`1<class LazyPart2.Product> Products
    {
        .get class [mscorlib]System.Collections.Generic.List`1<class LazyPart2.Product> LazyPart2.ProductCreater::get_Products()
    }
    .field private static initonly class [mscorlib]System.Collections.Generic.List`1<class LazyPart2.Product> products
}
```

Bu noktada bir de static constructor kullanımı düşünülebilir. Nitekim static yapıcı metod kullanımı beforefieldinit tanımlamasını kaldıracaktır. Bu durumu ele almak için öncelikle ProructCreator sınıfını aşağıdaki hale getirelim. Hatta sınıfıda static olarak tasarladığımızdan, private erişim belirleyicisine sahip Constructor'unda kaldırılması gerekmektedir.

```csharp
static class ProductCreater
{
	private static readonly List<Product> products = new List<Product>
	{
		new Product{ProductId=1,Name="Product X",ListPrice=1},
		new Product{ProductId=2,Name="Product Y",ListPrice=3},
	};

	static ProductCreater()
	{
	}

	public static List<Product> Products
	{
		get
		{
			return products;
		}
	}

	public static void DoSomething()
	{
		System.Console.WriteLine("Do Something");
	}
}
```

Bu durumda IL çıktısı aşağıdaki gibi olacaktır.

```bash
.class private abstract auto ansi sealed ProductCreater
    extends [mscorlib]System.Object
{
    .method private hidebysig specialname rtspecialname static void .cctor() cil managed
    {
    }
    .method public hidebysig static void DoSomething() cil managed
    {
    }
    .property class class [mscorlib]System.Collections.Generic.List`1<class LazyPart2.Product> Products
    {
        .get class [mscorlib]System.Collections.Generic.List`1<class LazyPart2.Product> LazyPart2.ProductCreater::get_Products()
    }
    .field private static initonly class [mscorlib]System.Collections.Generic.List`1<class LazyPart2.Product> products
}
```

Dikkat edileceği üzere beforefieldinit takısı görülmemektedir. İşte şimdi oldu diyerek rahat rahat koltuklarımıza yaslanabileceğimizi düşünebiliriz. Ama ne yazık ki bir anda irkilmemiz sadece an meselesidir. İşte Debug zamanındaki çalışma sırası.

![blg195_Debug2.gif](/assets/images/2010/blg195_Debug2.gif)

O da ne? DoSomething sonrası yine product alanının initialize edildiği görülmektedir.

![Cry](/assets/images/2010/smiley-cry.gif)

Tam bir hüsran diyebilir miyiz? Aslında olayı iyi tarafından ele alabiliriz. Şöyleki; sınıf içerisinde yer alan static alanlar, sınıfın başka bir static üyesi (metod, özellik) çağırıldığı takdirde otomatik olarak zaten initialize edilmektedir. Demek ki kalıbımızın uygulanış biçimi çok doğru değildir. Üstelik Thread Safe bir yaklaşım içermediği de gün gibi ortadadır. O halde ProductCreator tipimizi aşağıdaki gibi değiştirerek ilerlemeye devam edelim.

```csharp
using System.Collections.Generic;

namespace LazyPart2
{
    class Program
    {
        static void Main(string[] args)
        {
            ProductCreater.DoSomething();
            var productList = ProductCreater.Products;
            var productListAgain = ProductCreater.Products; // Bu çağrı sonrasında ProductCreator içerisindeki products alanının yeniden örneklenmediği görülecektir
        }
    }

    static class ProductCreater
    {
        private static List<Product> products = null;
        private static readonly object lockObject = new object();

        static ProductCreater()
        {
        }

        public static List<Product> Products
        {
            get
            {
                if (products == null)
                    lock (lockObject)
                    {
                        if (products == null)
                        {
                            products = new List<Product>
                            {
                                new Product{ProductId=1,Name="Product X",ListPrice=1},
                                new Product{ProductId=2,Name="Product Y",ListPrice=3},
                            };
                        }
                    }

                return products;
            }
        }

        public static void DoSomething()
        {
            System.Console.WriteLine("Do Something");
        }
    }

    class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public double ListPrice { get; set; }
    }
}
```

Bu sefer ThreadSafe olan (Double Check yapıldığına dikkat edelim) ve gerçekten Products özelliği çağırıldığında initialize işlemini gerçekleştiren bir tip elde etmiş olduk. Her ne kadar DoSomething metodunu çağrısı sonrası çalışma zamanı products ve lockObject alanlarını oluşturacak olsa da, bizim için önemli olan ürün listesinin Lazy olarak başlatılmasıdır. Sanıyorum ki biraz daha elle tutulur bir sonuca vardık. Hatta bu kod parçasına göre en basit tasarım kalıbının (Design Pattern) uygulandığı ip ucunu verebiliriz. Ama daha iyisi olabilir mi? Söz gelimi içinde hiç bir iş yapılmayan bir static metodumuz var. Bu olmasa. Sınıfımızda static tanımlanmak zorunda değil aslında. Aslında işi yine.Net tarafına yıkabiliriz. Aslında ben Lazy tipini kullanmak istiyorum diye haykırabiliriz

![Yell](/assets/images/2010/smiley-yell.gif)

İşte haykırıyorum. Aşağıdaki kodu göz önüne alalım.

```csharp
using System.Collections.Generic;
using System;

namespace LazyPart2
{
    class Program
    {
        static void Main(string[] args)
        {
            ProductCreater.DoSomething();
            var productList = ProductCreater.Products;
            var productListAgain = ProductCreater.Products; // Bu çağrı sonrasında ProductCreator içerisindeki products alanının yeniden örneklenmediği görülecektir
        }
    }

    class ProductCreater
    {
        private static readonly Lazy<List<Product>> products = new Lazy<List<Product>>(
            () =>
            {
                return new List<Product>
                            {
                                new Product{ProductId=1,Name="Product X",ListPrice=1},
                                new Product{ProductId=2,Name="Product Y",ListPrice=3},
                            };
            }
        );

        public static List<Product> Products
        {
            get
            {
                return products.Value;
            }
        }

        public static void DoSomething()
        {
            System.Console.WriteLine("Do Something");
        }
    }

    class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public double ListPrice { get; set; }
    }
}
```

Bu sefer products isimli değişken Lazy> tipinden tanımlanmıştır. Diğer yandan Products özelliği products isimli alanın Value özelliğini döndürmektedir. Buna göre çalışma zamanındaki kod icra sırasına bakıldığında DoSomething çağrısı sonucu Product listesinin oluşturulduğu kod bloğuna girilmediği görülecektir. Hatta Lazy tipinin Lazy Initialize işlemini gerçekleştirdiği safha da Thread Safe bir ortam sağladığını da biliyoruz. Bu nedenle bir önceki yapı da uyguladığımız lock kilit mekanizmalı double check içeren kod parçasına da gerek kalmamıştır.

Vuuvvv!!! Baya bir karışık oldu yauv. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[LazyPart2.rar (21,80 kb)](/assets/files/2010/LazyPart2.rar) [Örnek Visual Studio 2010 Ultimate üzerinde geliştirilmiş ve test edilmiştir]
