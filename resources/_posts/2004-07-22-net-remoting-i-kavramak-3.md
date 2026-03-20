---
layout: post
title: "NET Remoting' i Kavramak - 3"
date: 2004-07-22 12:00:00 +0300
categories:
  - dotnet-remoting
tags:
  - dotnet-remoting
  - csharp
  - threading
  - concurrency
  - delegates
---
Bu makalemizde, uzak nesneler üzerindeki metodlara asenkron olarak nasıl erişebileceğimizi kısaca incelemeye çalışacağız. Remoting ile ilgili bir önceki makalemizde, çok basit haliyle uzak nesnelerin, istemciler tarafından nasıl kullanılabildiğini incelemiştik. Geliştirmiş olduğumuz örnekte, uzak nesne üzerindeki metoda senkron olarak erişmekteydik. Yani, uzak nesnedeki metodun işleyişi bitene kadar, istemci uygulama kısa sürelide olsa duraksıyordu. Ancak bazı zamanlarda, uzak nesneler üzerinde işleyecek olan metodlar, belirli bir süre zarfında gerçekleşebilecek uzunlukta işlemlere sahip olabilirler. Böyle bir durumda istemci uygulamalar, metodların geri dönüş değerlerini beklemek zorunda kalabilirler. Oysaki, uzak nesneye ait metodlar bir yandan çalışırken, diğer yandanda istemci uygulamadaki izleyen kod satırlarının eş zamanlı olarak çalışması istenebilir. Bunu sağlamak için, uzak nesne metodlarına asenkron olarak erişilir.

Uzak nesne metodlarına asenkron olarak erişim, istemci uygulamalar için oldukça kullanışlıdır. Elbette bazı durumlarda senkron erişim tercih edilir. Öyleki, uzak nesneye ait metodun sonucu veya sonuçları, istemci uygulamada izleyen kod satırlarında kullanılıyor olabilir veya uzak nesne metodunun sonucu, istemci uygulama içindeki başka metodlara parametre olarak gönderiliyor olabilir vb... Elbette böyle bir durumda, uzak nesne üzerindeki metoda asenkron olarak erişmek çok mantıklı değildir. Nitekim, uzak nesne metodunun sonucunun veya sonuçlarının etkilediği başka işlemler söz konusudur.

Bu kısa açıklamalardan sonra, asenkron erişim için gerekli olan temel unsurlardanda kısaca bahsedelim. Uzak nesne üzerindeki metodların asenkron olarak çağırılması, normal bir uygulamadaki metodların asenkron olarak çağırılmasından çok da farklı değildir. Örneğin aşağıdaki console uygulamasını ele alalım. Bu uygulamada basit olarak Hesapla isimli metoda asenkron erişim gerçekleştirilmiştir.

```csharp
using System;
using System.Threading;

namespace istemciUygulama
{
    public class Sinif
    {
        public double Hesapla(double a,double b)
        {
            Thread.Sleep(3500);
            return(a*a+b*b);
        }
    }

    public class istemci
    {
        private delegate double Temsilci(double d1,double d2);

        public static void Main(string[] args)
        {
            Sinif nesne=new Sinif();
            Temsilci t=new Temsilci(nesne.Hesapla); 
            IAsyncResult res=t.BeginInvoke(4,5,null,null);
            Console.WriteLine("Uygulama çalışıyor..."); 
            res.AsyncWaitHandle.WaitOne();
            if(res.IsCompleted)
            {
                double sonuc=t.EndInvoke(res);
                Console.WriteLine(sonuc);
            }
            Console.ReadLine();
        } 
    }
}
```

Bu uygulamayı derleyip çalıştırdığımızda acaba tam olarak neler olmaktadır? Hesapla isimli metodumuz double tipinden iki parametre alan ve yine double tipinden değer döndüren bir yapıya sahiptir. Bu metod içerisinde, Thread sınıfının sleep metodu kullanılmış ve uygulama yaklaşık olarak 3.5 saniye süre ile duraksatılmıştır. Burada amaç uzun süren bir metod işleyişi gerçekleştirmektir. Metoda asenkron erişimin sağlanabilmesi için, bir delegate nesnesi kullanılmaktadır. Öncelikle delegate nesnemiz tanımlanır.

```csharp
private delegate double Temsilci(double d1,double d2);
```

Delegate nesnesinin metod imzasının, Hesapla metodu ile aynı olduğuna ve tipininde, Hesapla metodunun geri dönüş değeri tipi ile aynı olduğuna dikkat edelim. Gelelim Main metodu içerisindeki kodlara. Öncelikle,

```csharp
Temsilci t=new Temsilci(nesne.Hesapla);
```

satırları ile delegate nesnemiz oluşturulmaktadır. Artık t isimli temsilcimiz, nesne sınıfına ait Hesapla metodunun bellekteki başlangıç adresini temsil etmektedir. İşte bu adımdan sonraki işlemler önemlidir ve asenkeron erişim tekniğinin uygulanışını içermektedir.

```csharp
IAsyncResult res=t.BeginInvoke(4,5,null,null);
```

Satırı ile, delegate nesnesi için BeginInvoke metodu çağırılmaktadır. Bu metod görüldüğü gibi 4 parametre almıştır. İlk iki parametre, temsilcinin işaret ettiği metodun kullanacağı iki parametrenin değerini belirtmektedir. Sonraki parametreler ise null olarak bırakılmıştır. Burada olan olay şudur. t isimli temsilcinin işaret ettiği metod 4 ile 5 değerlerini parametre alarak çalışmaya başlamıştır. Lakin, bir metod çağırımından sonra uygulamanın izleyen kod satırlarını devam ettirebilmesi için, metodun işleyişini tamamlamış olması gerekir. Ancak burada, BeginInvoke ile Hesapla metodu çalıştırılmış ve anında ortama IAsyncResult arayüzü türünden bir nesne döndürülmüştür. Nitekim BeginInvoke metodunun geri dönüş değeri IAsyncResult arayüzü tipinden bir nesne örneğidir. Dolayısıyla izleyen satırlardaki kodlar işletilebilecektir. Bunun sonucu olarak ekrana "Uygulama çalışıyor..." ifadesi yazılır. Bu noktadan sonra uygulamada istenilen işlemler yapılabilir.

Tabiki temsilcimizin çalıştırdığı metodun sonucunun bir şekilde alınması gerekir. İlk yapılacak işlem, IAsyncResult arayüzünden nesne örneğinin IsCompleted özelliğinin değerine bakmak olacaktır. Bu değer true ise, asenkron metodun işleyişi tamamlanmış demektir. Bu durumda, asenkron olarak çalışan metodun geri dönüş değerini alabilmek için, t temsilcisinin EndInvoke metodu, uygulama ortamında o anda var olan IAsyncResult arayüzü nesne örneği res parametresi ile çağırılır. Sonuç olarak, asenkron metodun çalışmasının sonucu ürettiği değer elde edilir. Tüm bu işlemler için aşağıdaki kod satırları işletilmiştir.

```csharp
res.AsyncWaitHandle.WaitOne();
if(res.IsCompleted)
{
      double sonuc=t.EndInvoke(res);
      Console.WriteLine(sonuc);
}
Console.ReadLine();
```

Burada ilk satır ile, IAsyncResult arayüzü nesnesinin, çalışan asenkron metodun işleyişini tamamlamasını beklemesi söylenmiştir. Bu kod satırının yazılmasının amacı şudur. Asenkron metodun çalıştırılmaya başlamasından sonra, uygulamada izleyen kod satırları bu örnekte olduğu gibi çoktan tamamlanmış ancak halen daha asenkron metodun işleyişi bitmemiş olabilir. Bu durumda if döngüsü gerçekleşmeyeceği için metodun geri dönüş değeride alınamıyacaktır. Bu satır ile, asnekron metodun işleyişinin tamamlanması garanti altına alınmış olur. Uygulamanın bu kısmını aşağıdaki gibi daha kısa bir şekildede yazabiliriz.

```csharp
double sonuc=t.EndInvoke(res);
Console.WriteLine(sonuc);
Console.ReadLine();
```

Burada direkt olarak EndInvoke metodu çağırılmış ve asenkron metodun dönüş değeri alınmıştır. Eğer bu noktada, asenkron metod halen daha tamamlanmamış ise, tamamlanıncaya kadar beklenir ve uygulama bu cevap gelinceye kadar duraksar.

Burada kullandığımız basit örnekteki teknik, uzak nesnelerin kullanıldığı Remoting uygulamaları içinde geçerlidir. Yine temsilciler, IAsyncResult arayüzü, BeginInvoke ve EndInvoke metodları bizim anahtar üyelerimiz olacaklardır. Remoting uygulamalarında, uzak nesneye ait metodların asenkron olarak nasıl çağırıldığını incelemeden önce, geçtiğimiz Remoting Makalesindeki UzakNesne.cs sınıfımıza aşağıdaki gibi yeni bir metod ekleyelim.

```csharp
public double Alan(double yaricap)
{
    Thread.Sleep(1500);
    return (yaricap*3.14)/2;
}
```

Şimdi UzakNesne.cs dosyamızı yine aşağıdaki komut satırı ile derleyerek dll dosyamızı oluşturalım.

![mk78_1.gif](/assets/images/2004/mk78_1.gif)

Geliştirdiğimiz istemci uygulamanın bulunduğu klasöre UzakNesne.dll dosyamızı kopyalayalım ve istemci sınıfımızın kodlarınıda aşağıdaki gibi yazalım. Bu örneğimizde, metodumuza senkron tekniği ile erişmekteyiz. Amacımız uzak nesne metoduna senkron olarak yani varsayılan yapısı ile eriştiğimizde, uygulamanın çalışma şeklini inceleyebilmek.

```csharp
using System;
using System.Runtime.Remoting;

namespace istemciUygulama
{
    public class istemci
    {
        public static void Main(string[] args)
        {
            RemotingConfiguration.Configure("istemci.config");
            UzakNesne.Musteriler m=new UzakNesne.Musteriler();
            double alani=m.Alan(3);
            for(int i=1;i<=200;i++)
            {
                Console.Write(i.ToString()+" ");
            }
            Console.WriteLine("----");
            Console.WriteLine(alani);
            Console.WriteLine("Metodlarin Isleyisi Bitti");
            Console.ReadLine();
        } 
    }
}
```

Remoting uygulamamızı bu haliyle denemek için önce sunucu uygulamamızı çalıştıralım ve sunucuya gelecek olan istemci taleplerini dinlemeye başlayalım. Daha sonra ise, istemci uygulamamızı çalıştıralım. Bu adımlardan sonra aşağıdakine benzer ekran görüntüsü ile karşılaşırız.

![mk78_2.gif](/assets/images/2004/mk78_2.gif)

Bu ekran görüntüsünü açıklamadan anlamaya çalışmak bizim için yanıltıcı olacaktır. İstemci uygulama çalıştıktan yaklaşık 2 saniye kadar sonra bu ekran görüntüsü komple oluşacaktır. Yani for döngüsündeki kod satırları, uzak nesne üzerindeki metodumuzun işleyişi bitmeden ekrana yazılmayacaktır. Doğal olarak, uzak nesnedeki Alan metodunun istemci uygulamaya değer döndürmesiyle birlikte, for döngüsündeki işleyiş de ekrana yansıyacak ve tüm bu işlemler konsol penceresinde aynı zamanda gerçekleşecektir.

Oysaki uzak nesnemiz üzerindeki Alan metodunu asenkron olarak çağırsaydık, öncelikle döngü içindeki kod satırları çalışarak ekrana 1 den 500' e kadar olan sayılar yazılacak ve bu işlemlerin ardından uzak nesnedeki metodun sonucu ekrana yazılacaktı. Dolayısıyla uzak nesnedeki Alan metodu çalıştırıldıktan sonra, istemcideki izleyen kod satırları eş zamanlı olarak yürütülebilecekti. İşte bunu gerçekleştirebilmek için, istemci uygulamamızı aşağıdaki haliyle düzenlememiz ve uzak nesne metoduna asenkron olarak erişmemiz gerekmektedir.

```csharp
using System;
using System.Runtime.Remoting;

namespace istemciUygulama
{
    public class istemci
    {
        private delegate double Temsilci(double d);

        public static void Main(string[] args)
        {
            RemotingConfiguration.Configure("istemci.config");
            UzakNesne.Musteriler m=new UzakNesne.Musteriler();
            Temsilci t=new Temsilci(m.Alan);
            IAsyncResult res=t.BeginInvoke(3,null,null); 
            for(int i=1;i<=500;i++)
            {
                Console.Write(i.ToString()+" ");
            }
            Console.WriteLine("----");
            double alani=t.EndInvoke(res);
            Console.WriteLine(alani); 
            Console.WriteLine("Metodlarin Isleyisi Bitti");
            Console.ReadLine();
        } 
    }
}
```

Şimdi sunucuyu çalıştırıp, sonrada istemciyi çalıştırdığımızda sonuç olarak yine aynı ekran görüntüsünü elde ettiğimizi görürüz. Ancak bu kez döngü, asenkron metodun işleyişi ile birlikte çalışmış ve 1' den 500' e kadar olan sayıların konsol penceresine yazılmasının ardından kısa bir süre sonrada asenkron metodun sonucu ekrana yazılmıştır. İşleyiş tekniği ilk başta incelediğimiz örnekteki ile aynıdır. Bu kez sadece, temsilcimizi oluştururken, işaret edeceği metod olarak, istemci uygulamanın çalıştığı klasördeki dll içinde bulunan uzak nesne sınıf örneğine ait metod belirtilmiştir. Tabiki bu metodun uzak nesnedeki örneğinin çalıştırılması, konfigurasyon dosyasındaki kanal ayarlamaları sonucu ilgili kanal nesneleri üzerinden gerçekleşmektedir. Böylece, bir Remoting uygulamasında, uzak nesneler üzerindeki metodların asenkron olarak nasıl çalıştırılabileceğini en temel hatları ile ve yüzeysel olarak incelemiş olduk. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek kodlar için tıklayın.](/assets/files/2004/Remoting3.zip)