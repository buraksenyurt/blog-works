---
layout: post
title: "Tasarım Desenleri - Memento"
date: 2009-07-05 20:45:00 +0300
categories:
  - tasarim-kaliplari-design-patterns
tags:
  - tasarim-kaliplari-design-patterns
  - csharp
---
Sanıyorum yandaki resmi görenler Guy Pearce ve Carrie Anne Moss'un başrolde yer aldıkları bu filmi hatırlayacaklardır. Benimde favori klasiklerim arasında yer alan bu film, tersten ilerlemesi bir yana herşeyi unutan ve bazı kritik kuralları hatırlamak için dövemeler yaptırmak zorunda kalan bir adamın hikayesi ile ilgiliydi. Çok şükürki nesne yönelimli (Object Oriented) dillerde, önceki hallerinin hatırlanılması istenilen varlıklar için geliştiricilerin vücutlarına dövme yaptırmasına gerek yoktur.

![memento-1-1024.jpg](/assets/images/2009/memento-1-1024.jpg)

İşte bu günkü konumuz Memento tasarım kalıbı...

Çok sık kullanılmamakla birlikte (en azından dofactory.com istatistiklerine göre %20' ler seviyesinde) oluşturulması ve kullanılması kolay olan bu desen, davranışsal (Behavioral) kalıplar arasında yer almaktadır. Esas itibariyle bir nesnenin daha önceki halinin (hallerinin) saklanması ve istenildiğinde tekrardan elde edilmesi üzerine tasarlanmış bir kalıptır. Nesnelere, dahili durumları için (Initial State) geri alma işlemi (Undo) yeteneğininin kazandırılması olarak da düşünebiliriz. Bu kalıpta durumu korunmak istenen nesnenin birebir veya en azından saklanmak istenen alanlarını (özelliklerini) tutan kopyası yer alır (Memento).

Diğer taraftan memento nesnesini oluşturan, bir başka deyişle kaydeden yada var olduğu son durumu yükleyerek geri getiren fonksiyonelliklere sahip olan asıl tipimiz yer almaktadır (Originator). Bu temel tiplerin yanında, saklanan memento nesnesinin güvenli bir şekilde korunmasını sağlayacak bakıcı bir tipde yer almaktadır (Caretaker). Şimdi tabi olaya bu şekilde bakınca (şekilsiz olarak) anlamak zor olabiliyor. Yaşasın UML şemaları diyerek yola devam etmek lazım.

![Wink](/assets/images/2009/smiley-wink.gif)

Bu amaçla örnek Console uygulamamızın sınıf diagramına bir bakalım.

![blg42_uml.gif](/assets/images/2009/blg42_uml.gif)

Uygulamanın çok basit bir amacı var. Product tipinden bir nesne örneğinin istenildiği durumda bir önceki haline döndürülmesi. Dikkat edileceği üzere Product tipinin özelliklerinin birebir aynısını içeren Memento isimli bir sınıf bulunmaktadır. Bu sınıf, Product nesne örneğinin herhangibir anda saklanmak istenen dahili içeriğini tutmak amacıyla oluşturulmuştur. Product isimli sınıfımız içerisinde de Memento nesne örneğini oluşturan Save ve tekrar geri alıp dolduran Restore metodları yer almaktadır. Memory tipi ise, Memento nesne örneğinin güvenli bir şekilde saklamasından sorumludur. Buna ek olarak Memento nesnesi üzerinde hiç bir operasyon gerçekleştirilmesine izin vermemektedir. Gelelim uygulama kodlarımıza.

```csharp
using System;

namespace MementoPattern
{
    // Originator Class
    // Yaratıcı sınıf
    class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal ListPrice { get; set; }

        // O anki Product nesne örneğinin içeriğini yeni bir Memento nesne örneğinde toplar ve bunu dış ortama verir.
        public Memento Save()
        {
            return new Memento { 
                ProductId = this.ProductId
                , Name = this.Name
                , ListPrice = this.ListPrice 
            };
        }

        // Saklanan Memento nesne örneğini alarak o anki Product nesne örneğinin dahili içeriğinin doldurulmasında kullanılır.
        public void Restore(Memento memento)
        {
            this.ListPrice = memento.ListPrice;
            this.Name = memento.Name;
            this.ProductId = memento.ProductId;
        }

        public override string ToString()
        {
            return String.Format("{0} : {1} ( {2} )", ProductId, Name, ListPrice.ToString("C2"));
        }
    }

    // Memento Class
    // Akıl defteri sınıfı
    // Product tipi içerisinden saklanmak amacıyla kullanılacak tüm özellikleri tanımlar
    class Memento
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal ListPrice { get; set; }
    }

    // Caretaker class
    // Bakıcı sınıf
    class Memory
    {
        public Memento ProductMemento { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Örnek bir Product nesnesi oluşturulur
            
            Product prd = new Product
            {
                ProductId = 1000,
                Name = "Starbucks Kahve Fincanı 330 mililitre",
                ListPrice = 12
            };
            Console.WriteLine(prd.ToString());
            
            // Caretaker nesnesi oluşturulur.            
            Memory memory = new Memory();
            // Memento nesnesi içeriği o anki Product örneğinden elde edilir.
            memory.ProductMemento = prd.Save();
            Console.WriteLine("Product nesnesi kaydedildi.Değişiklik yapılacak.");

            prd.ProductId = 9999;
            prd.Name = "STARBUCKS KAHVE KABI";
            prd.ListPrice = 24;
            Console.WriteLine("Yeni hali : \n\t{0}", prd.ToString());

            // Restore işlemi gerçekleştirilir
            prd.Restore(memory.ProductMemento);
            Console.WriteLine("Undo : \n\t{0}",prd.ToString());
        }
    }
}
```

Ve uygulamamızın çalışma zamanındaki durumu.

![blg42_2.gif](/assets/images/2009/blg42_2.gif)

Görüldüğü gibi Product nesne örneği oluşturulup içeriği üzerinde değişiklik yapıldıktan sonra bir önceki konumuna döndürülebilmiştir. Oldukça basit ama etkileyici olan bu deseni, nesne örnekleri üzerinde Undo operasyonunun kullanılması istenen pek çok senaryoda ele alabiliriz. Tabi burada dikkat edilmesi gereken bir hususda vardır. Sadece tek bir Undo işlemi yapılabilmektedir. Oysaki istenirse birden fazla adım geriye gidilmesi de sağlanabilir. Bunu bir düşünmenizi ve yapmaya çalışmanızı öneririm

![Wink](/assets/images/2009/smiley-wink.gif)

Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Youtube Link](https://www.youtube.com/watch?v=yio36Q5g5vU)

[MementoPattern.rar (23,01 kb)](/assets/files/2009/MementoPattern.rar)
