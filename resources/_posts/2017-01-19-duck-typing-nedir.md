---
layout: post
title: "Duck Typing Nedir?"
date: 2017-01-19 21:30:00 +0300
categories:
  - ruby
tags:
  - ruby-lang
  - type-systems
  - duck-typing
  - static-languages
  - dynamic-programming
---
Son bir yıldır Ruby ve Python gibi script diller üzerinde araştırmalar yapıyorum. Daha çok bu dilleri öğrenme, anlama gayretindeyim. En azından orta seviye bilgi sahibi olmak benim için yeterli. Bu dilleri incelerken uzun yıllardır çalıştığım dinamik tip sistemli ve derleyici odaklı yaklaşımları da sorgulamak durumunda kalıyoum. Nitekim farklı programlama yaklaşımlarına sahipler.

![duffy.gif](/assets/images/2017/duffy.gif)

Ruby, Python vb script diller çoğunlukla dinamik tip sistemini (dynamic type system) kullanıyorlar. Yani C#, Java, C++ benzeri dillerde kullanılan statik tip sisteminden (static type system) farklı bir yaklaşım söz konusu. Ayrıca betik diller kodun çalışma zamanında yorumlanarak (Interpret) yürütülmesine odaklandıklarından, derleme (compiling) yaklaşımını benimseyen dillerden epeyce farklılaşıyorlar. Hal böyle olunca diller arası bazı farklı teknikler ve yazım stilleri ortaya çıkıyor. Bunlardan birisi de Duck Typing.

## Tanım

> If it walks like a duck, and quakcs like a duck, then it must be a duck.

Enteresan bir tanımlama şekli olduğu aşikar. Kısaca bir şey ördek gibi yürüyor ve ördek gibi konuşuyorsa O bir ördek olarak kabul edilebilir. Bu felsefeyi anlamak beni biraz uğraştırdı doğrusu. Yıllarca karşılaşmadığım Ruby ile uğraşmasam haberdar olamayacağım bir teknik. Öncelikle düşünce yapımı değiştirdim. Programcı olarak bir nesnenin (object) ne olduğundan ziyade ne yapabileceğine odaklanmam gerektiğini benimsemem gerekiyordu.

Duck Typing aslında çalışma zamanı ile ilgili de bir konu. Compile-Time ile Run-Time arasındaki davranış farklılıkları bu teknikte öne çıkıyor. Nitekim duck typing stilinde, sistem çalışmadan önce çeşitli şartların sağlanmasını beklemiyor. Bunun yerine çalışma zamanında herhangibir şeyi (örneğin parametre olarak gelen bir nesne üzerinden bir fonksiyonun çalıştırılması) yürütmeyi deniyor. "Deniyor" diyoruz çünkü icra edilmesi istenen fonksiyonelliğin önceden tanımlanmış bir kuralı veya şablonu olmak zorunda değil.

## Ducy Typing Yokken

Kafalar biraz karıştı değil mi? Aslında benim de karışık. Haydi gelin bir örnek ile konuyu anlamaya çalışalım. İlk olarak Ducy Typing olmadan bir şeyleri görmemiz gerekiyor. Bu anlamda aşağıdaki C# kodunu ele alacağız.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicTyping
{
    class Program
    {
        static void Main(string[] args)
        {
            Car frrari = new Car();
            Plane cesna = new Plane();
            Ship fery = new Ship();
            DriveVehicle(frrari);
            DriveVehicle(cesna);
            DriveVehicle(fery);
        }

        static void DriveVehicle(IVehicleOps vehicle)
        {
            vehicle.Drive();
        }
    }

    interface IVehicleOps
    {
        bool Drive();
    }

    class Car
        : IVehicleOps
    {
        public bool Drive()
        {
            Console.WriteLine("Driver is driving");
            return true;
        }
    }

    class Plane
        :IVehicleOps
    {

        public bool Drive()
        {
            Console.WriteLine("Pilot is driving");
            return true;
        }
    }

    class Ship
    {

    }
}
```

Kodu dikkatlice inceleyelim. Sizce bu kod derlenir mi? Aslında böyle sorduğuma göre derlenmiyor olması lazım değil mi? Uygulamada bir interface ve üç sınıf bulunmakta. Car ve Plane sınıfları IVehicleOps arayüzünü uyguluyorlar. Bu nedenle Drive isimli bir işlevselliğe sahipler. Ship isimli sınıfın ise bu arayüzü uygulamadığını görüyoruz. Dolayısıyla Drive isimli bir kabiliyete sahip değil. Program sınıfı içerisinde yer alan DriveVehicle metodu parametre olarak IVehicleOps tipinden değişkenler alıyor. Dolayısıyla bu arayüzü uygulayan nesne örneklerini kendisine aktarıp Drive fonksiyonunu çağırabiliriz. Lakin Ship sınıfına ait tasarımda böyle bir yetenek mevcut değil. Nitekim Ship, IVehicleOps tarafından taşınabilecek bir nesne modeli değil. Bu sebepten kod derlenmeyecektir.

![ducktype_1.gif](/assets/images/2017/ducktype_1.gif)

Ne zararı var peki? Programcı zaten tipleri önceden tanımlayarak geliştirme yapıyor. Kuralları biliyor. Interface kullanımı sayesinde metoda çok biçimli bir yapı kazandırıp OOP (Object Oriented Programming) ilkelerinden birisini de uyguluyor. Ship nesne örneği üzerinden Drive operasyonunun gerçekleşmemesi normal bir sonuç. Çünkü arayüz tanımına uygun yapıda değil. Burada bir çok tanımlama söz konusu. Kullanıcı tanımlı tipin önceden tasarlanması, uyacağı kuralların bildirilmesi gerekti.

## Ducy Typing Yaklaşımı

Dinamik tip sistemli dillere gelindiğinde farklı bir bakış açısı daha var. Şimdi gelin bu farklılığı ortaya koyan Duck Typing ile aynı senaryonun bir benzerinin nasıl ele alınacağına bakalım. Ruby'de aşağıdaki örnek kod parçasını yazarak işe başlayabiliriz.

```text
class Car
  def drive
    puts "driver is driving"
  end
end
class Plane
  def drive
    puts "pilot is driving"
  end
end
def drive(vehicles)
  vehicles.each{|v|v.drive}
end

frrari=Car.new()
bat=Plane.new()
vehicle_array=[frrari,bat]
drive(vehicle_array)
```

![ducktype_2.gif](/assets/images/2017/ducktype_2.gif)

Ne olduğuna kısaca bakalım. Car, Plane sınıfları içerisinde yine drive isimli birer metodumuz var. Örnekte vehicles isimli değişken alan drive metodunun işleyişi önemli. Dikkat edileceği üzere vehicles dizisinin içerisindeki her v değişkeni üzerinden drive fonksiyonu çağırılıyor (Aslında vehicles'ın Car, Plane gibi araçlar taşıyacağı da kesin değil. Ne demiştik? Yaklaşımımızı farklılaştırmamız gerekiyor. Derleyici gibi değil yorumlayıcı gözüyle bakıp kodu yazmalıyız) Buna göre vehicles değişkenini Car ve Plane gibi sınıflara ait nesne örnekleri ile doldurup topluca drive operasyonunu uygulatmamız mümkün. Ortada bir arayüz bildirimi veya türetme gibi bir şey yok. Çalışma zamanı felsefesi basit. Ruby yorumlayıcısı vehicles elamanlarını gezerken eğer sürüş yeteneği varsa sürerim diyor. Çok doğal olarak Ship isimli ve drive metodu olmayan bir sınıf örneğini bu dizi içerisine alıp kullanmak istesek çalışma zamanında hata alırız (Dikkat edin derleme zamanı değil çalışma zamanı dedim)

> Eğer drive metoduna gelen değişken içindeki nesne drive metodunu destekliyorsa sıkıntı yok. Çalıştırmayı dene! Ama drive metodu o anki nesne için söz konusu değilse çalışma zamanına hata fırlat.

![ducktype_3.gif](/assets/images/2017/ducktype_3.gif)

Sanıyorum siz de benim gibi Duck Typing yaklaşımını biraz daha iyi anladınız. Bakalım Ruby, Python gibi dilleri inceledikçe daha farklı nelerle karşılacağız!? Bir başka yazımızda görüşünceye dek hepinize mutlu günler dilerim.
