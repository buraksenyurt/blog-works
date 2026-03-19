---
title: "Biyütıful Kod - Ninject ile Bir Dependency Injection Macerası"
pubDate: 2015-01-04 20:00:00
categories:
  - C#
  - Tasarım Prensipleri(Design Principles)
tags:
  - C#
  - Tasarım Prensipleri(Design Principles)
  - .NET
  - HTTP
  - Reflection
  - Testing
  - Dependency Injection
  - GitHub
  - Dependency Management
---

# Biyütıful Kod - Ninject ile Bir Dependency Injection Macerası
[![OLYMPUS DIGITAL CAMERA](images/legoninjas_thumb.jpg)](images/legoninjas.jpg) Merhaba Arkadaşlar,

Her yazılım geliştirici özellikle büyük bir projeye girdiğinde kodlarının kaliteli olması için uğraşır. Bu yönde adımlar atar. Çoğu zaman bu bir sanata dahi dönüşebilir. Okunabilir kodlar oluşturmanın dışında, mimari açıdan büyüleyici olan, yeniden kullanılabilirliğin üst seviyede olduğu, fazla uğraşılmadan genişleyebilen ürünler ortaya çıkartmak en büyük gayelerden birisi haline gelir. Martin Fowler'ın ilkeleri sıkı sıkıya takip edilir. Kurumsal çözüm içerisinde Fluent API'ler kullanılmaya, "Dependency Injection Container" gibi kavramlar konuşulmaya başlanır. Ne kadar başarılabilir bilinmez ama amaçlardan birisi de Biyütıful Kodu ortaya çıkartmaktır.

Dependency Injection Container Hakkında

Nesne yönelimli programlama (Object Oriented Programming) dünyasından bakıldığında Dependency Injection, yazılımların (Software) harici bileşenlere (External Components) olan bağımlılıklarının kontrol altın alınmasında önem arz eden bir prensiptir. Amaç, yazılımların kullandıkları bileşenler ile (veya sınıfların birbirleri ile) gevşek bağlar (loose coupling) kurabilmesini sağlamaktır. Söz konusu prensibin kolayca uygulanabilmesi de önemlidir. Öyle ki, gevşek bağlanan bileşenler arasında kolayca ve zahmetsizce geçişler yapılabilmeli ya da sorumluluk zincirine yeni bağımlılıklar zahmetsizce eklenebilmelidir. Üstelik bu değişiklikler koda minimum seviyede dokunarak ve sadece gerekli olanlar yeniden Build edilerek yapılabilmelidir.

> Kısaca Dependency Injection, bileşenler arasındaki hard-coded bağımlılıkların, tasarım zamanı (design time) yerine çalışma zamanında (run time) enjekte edilmesidir.

Bu esenkliğin pek çok noktada faydası vardır. Örneğin;

- Test güdümlü geliştirilen (Test Driven Development) uygulamalarda, o an için ihtiyaç duyulmayan bileşenlerin sahteleri ile kolayca değiştirilerek birim testlerin (Unit Test) çalıştırılmasında,
- Miras olarak kalmış kod parçalarının (Legacy Codes) yazılım tarafında fazla kod davranışı değiştirmeden kullanılabilmesinde,
- Asıl uygulamaların yeniden derlenmeye gerek kalmadan kolayca bileşen değiştirebilmesinde,
- Mikro servis mimarisinde,
- Log'lama gibi Cross Cutting bileşenlerinin dış bağımlılıklarının esnek bir şekilde değiştirilebilmesinde,
- İzole edilmiş 3ncü parti bileşenler arasında geçişler yapılabilmesinde,
- Belli bir bağımlılığın n sayıda bileşene enjekte edilmesi gerektiği durumlarda (Özellikle DAO-Data Access Object'ler de sıklıkla görebiliriz),
- Bir bileşenin farklı örneklerinin diğer bileşenlere farklı konfigurasyonlar ile bağlanması gerektiği durumlarda vb...

> Depedency Injection ile ilişkili olarak akla gelen en büyük soru aslında ne zaman kullanılması gerektiğidir. Bu noktada [şu adresteki](http://blog.ploeh.dk/2012/11/06/WhentouseaDIContainer/) makaleyi takip etmenizi şiddetle öneririm.

Yukarıda bahsettiğimiz vakalar göz önüne alındığında Dependency Injetcion'ın basitçe uygulanmasının da önemli olduğunu ifade edebiliriz. Bunun için geliştirilmiş pek çok Container kütüphanesi mevcuttur. En popülerleri Castle Windsor, Spring.Net, Unity ve Ninject'tir. Elbette kendimiz de bir Dependency Injection Container bileşeni yazabiliriz. Ancak kurumsal çaptaki uygulamalarda çoğunlukla hazır kütüphanelerden faydalanıldığını da unutmamalıyız (Reinventing the Square Wheel gibi bir anti-pattern oluşmaması için. Tabi sıfırdan öğrenmek istiyorsak istisnai bir durum mümkündür)

Başlangıç

İşte bu yazımızdaki amacımız temel anlamda bu tip Container'ların nasıl kullanılabildiğini anlamaktır. Neredeyse tüm Conatiner araçları aynı temeller üzerine oturmaktadır. Ninject kütüphanesini göz önüne alarak ilerlemeye çalışalım. Konuyu basitçe değerlendirmek için ele alacağımız Console uygulamasına Ninject kütüphanesini NuGet paket yönetim aracı ile yükleyerek devam edebiliriz.

[![htn_1](images/htn_1_thumb.png)](images/htn_1.png)

> Ninject açık kaynak kodlu bir projedir. [GitHub](https://github.com/ninject/ninject) üzerinden bakılabilir.

İlk Kodlar

Ninject ile bağımlılıkları enjekte etmeden önce aşağıdaki gibi bir kod içeriğine sahip olduğumuzu düşünelim.

[![htn_2](images/htn_2_thumb.png)](images/htn_2.png)

```csharp
namespace HowTo_UsingNinject 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            IEncryptor dv = new DaVinciEncryptor(); 
           MessageProvider provider = new MessageProvider(dv); 
            string encryptedMessage=provider.EncryptMessage("Bir not"); 
            string decryptedMessage = provider.DecryptMessage(encryptedMessage); 
        } 
    }

    class MessageProvider 
    { 
        private IEncryptor _encryptor; 
        public MessageProvider(IEncryptor Encryptor) 
        { 
            _encryptor = Encryptor; 
        } 
        public string EncryptMessage(string Message) 
        { 
            return _encryptor.Encrypt(Message); 
        } 
        public string DecryptMessage(string Message) 
        { 
            return _encryptor.Decrypt(Message); 
        } 
    }

    interface IEncryptor 
    { 
        string Encrypt(string Message); 
        string Decrypt(string Message); 
    }

    class MichalengeloEncryptor 
        : IEncryptor 
    { 
        public string Decrypt(string Message) 
        { 
            // Bir takım işlemler yapıldığını düşünelim 
            return Message; 
        }

        public string Encrypt(string Message) 
        { 
            // Bir takım işlemler yapıldığını düşünelim 
            return Message; 
        } 
    }

    class DaVinciEncryptor 
        : IEncryptor 
    { 
        public string Decrypt(string Message) 
        { 
            // Bir takım işlemler yapıldığını düşünelim 
            return Message; 
        }

        public string Encrypt(string Message) 
        { 
            // Bir takım işlemler yapıldığını düşünelim 
            return Message; 
        } 
    } 
}
```

İlk Kod Parçasında Ne Yaptık?

Örnek senaryoda bir mesajlaşma sisteminde hareket eden içeriklerin şifrelenme ile ilgili işlemlerin ele alındığını ifade edebiliriz. IEncryptor isimli arayüz (Interface) bilginin şifrelenmesi veya şifrelenmiş bilginin çözülmesi için gerekli iki temel fonkisyonellik sunmaktadır. Asıl şifreleme işini ise DaVinciEncryptor ve MichalengeloEncryptor isimli sınıflar üstlenmektedir. Elbette yeni şifreleme teknikleri bu arayüz sözleşmesinden yararlanılarak sisteme kolayca entegre edilebilir ve MessageProvider tarafında ele alınabilir. Bir nevi şifreleme sözleşmesi (Contract) tanımladığımızı ve iki basit uyarlamasını hazırladığımızı düşünebiliriz.

MessageProvider sınıfı ise aslında manuel olarak bir Dependency Injection uygulamaktadır. Dikkat edileceği üzere yapıcı metod (constructor) IEncryptor arayüzü tipinden bir parametre almakta ve private tanımlanmış encrpytor değişkeninin set edilmesinde kullanılmaktadır. Bu değişkenin alabileceği çalışma zamanı nesne örnekleri, IEncryptor sözleşmesini uygulayan sınıflardan olabilir. İçerdiği EncryptMessage ve DecryptMessage metodları ise, çalışma zamanında atanacak şifreleme tipini kullanmaktadır.

Önemli olan nokta ise; IEncryptor arayüzünün taşıyacağı gerçek nesne örneğinin ne olacağına MessageEncryptor sınıfı içinde değil, MessageEncryptor'ı çağıran yerde (senaryoda Console uygulamasının kendisidir) karar verilmesidir.

Ninject ile Bağımlılıkların Enjekte Edilmesi

Peki bu manuel olarak bağımlılıkları enjekte etme yöntemi yerine Ninject aracını kullanmak istesek nasıl bir yol izleriz? Dahası Ninject bize bu bağımlılıkların enjekte edilmesi noktasında nasıl bir avantaj sağlamaktadır? Kod içeriğini aşağıdaki şekilde değiştirerek devam edelim bu soruların cevabını bulmaya çalışalım.

[![htn_3](images/htn_3_thumb.png)](images/htn_3.png)

```csharp
using System; 
using System.Reflection; 
using Ninject; 
using Ninject.Modules;

namespace HowTo_UsingNinject 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            #region Ninject Kullanarak

            IKernel kernel = new StandardKernel(); 
            kernel.Load(new MessageBindingModule()); 
            IEncryptor dv = kernel.Get<IEncryptor>();

            MessageProvider provider = new MessageProvider(dv); 
            string encryptedMessage = provider.EncryptMessage("Bir not"); 
            string decryptedMessage = provider.DecryptMessage(encryptedMessage);

            #endregion 
        } 
    }

    class MessageBindingModule 
        :NinjectModule 
    { 
        public override void Load() 
        { 
            Bind<IEncryptor>().To<DaVinciEncryptor>(); 
        } 
    }

    // Diğer kodlar 
}
```

İlk dikkat edilmesi gereken nokta var olan sınıflarda ve arayüz de herhangibir kod değişikliği yapılmamış olmasıdır. Bağımlılıkların tanımlanması tamamen yeni bir sınıf içerisinde gerçekleştirilmektedir. MessageBindingModule sınıfının ve Main metodundaki enjekte etme adımlarının farklı bir assembly'da olabileceğini düşünürsek özellikle var olan bileşen yapılarını da bozmadan ilerlenebileceğini ifade edebiliriz.

MessageBindingModule, NinjectModule sınıfından türemiştir ve Load metodu ezilmiştir. Bu modül tahmin edileceği üzere bağımlılıkların tanımlandığı yerdir. Koda göre IEncryptor arayüzünün DaVinciEncryptor'a bağlanması söz konusudur ki bu çalışma zamanında icra edilecek bir operasyondur.

> Modül kullanımı bu tip bağımlılıkların tanımlanması için bir zorunluluk değildir. Farklı Injection Pattern teknikleri de bulunmaktadır. Diğer yandan çoklu bağımlılık tanımlamalarında (Multi Injection) modül yaklaşımı tercih edilmelidir.

Main metodu içerisinde ise dikkat çekici işlemler vardır. IKernel referansı bir StandartKernel nesne örneği olarak alındıktan sonra bağımlılıkların Load metodu içerisine verilen MessageBindingModule üzerinden yapılması gerektiği ifade edilmektedir. kernel.Get metoduna verilen arayüz adı, modül içerisindeki Load metodunca otomatik olarak bulunacak ve geriye DaVinciEncryptor örneği döndürülecektir. Kodun geri kalan kısmında yapılanlar ise aynıdır.

Nasıl Bir Avantaj?

Aslında Ninject ile bağımlılıkları enjekte ettiğimiz yukarıdaki örnekte çok da fazla avantaj yok gibi görünmektedir. Hatta manuel yazdığımız örnektekine göre daha fazla kod satırı oluştuğunu düşünebiliriz. Ninject'in veya benzer bir Container'ın hangi noktada avatantaj sağladığını görmek için, bileşenler arası bağımlılıkların sayısının arttığını düşünmemiz yerinde olacaktır. Nitekim bir gerçek hayat projesinde bileşenler arasındaki bağımlılıkların proje büyüdükçe arttığı gözlemlenir. Bu artış sonrası bağımlılık zincirlerinin tesit edilmesi, component’ ler in değiştirilmesi giderek zorlaşır. İşte böyle bir durumda bağımlılıkları otomatik olarak algılayabilecek ve kod eforunu aza indirgeyecek Container'ların kullanılması önemlidir.

Bu anlamda senaryoya şöyle bir ek yaptığımızı düşünelim.

```csharp
interface IAlgorithmProcessor 
{ 
    string Calculate(string Info); 
}

class IntelligenceProcessor 
    : IAlgorithmProcessor 
{ 
    public string Calculate(string Info) 
    { 
        // Bir algoritma kullanılıyor 
        return Info; 
    } 
}

class MichalengeloEncryptor 
    : IEncryptor 
{ 
   private IAlgorithmProcessor _processor;

    public MichalengeloEncryptor(IAlgorithmProcessor Processor) 
    { 
        _processor = Processor; 
    } 
    public string Decrypt(string Message) 
    {   
        // Bir takım işlemler yapıldığını düşünelim 
        return _processor.Calculate(Message); 
    }

    public string Encrypt(string Message) 
    { 
        // Bir takım işlemler yapıldığını düşünelim 
        return _processor.Calculate(Message); 
    } 
}
```

MichalengeloEncryptor içerisinde IAlgorithmProcessor arayüzünü uygulayan sınıflar için bir bağımlılık daha söz konusudur. Çok doğal olarak MessageProvider sınıfı da bu bağımlılık üzerinden IAlgorithmProcessor'a bağlanmıştır. Bu yeni bağımlılığın sisteme enjekte edilmesi için Ninject modülü içerisinde aşağıdaki kodlamayı yapmak yeterli olacaktır.

```csharp
class MessageBindingModule 
    :NinjectModule 
{ 
    public override void Load() 
    { 
        Bind<IEncryptor>().To<MichalengeloEncryptor>(); 
        Bind<IAlgorithmProcessor>().To<IntelligenceProcessor>(); 
    } 
}
```

Dolayısıyla bir Dependency Injection Container bileşeni, uygulamada kullanılan sınıflar arası bağımlılıklar arttıkça etkisini gösterecektir. Ninject Container aracını kullanmak görüldüğü üzere son derece kolaydır. Diğer yandan aracın çok daha farklı yetenekleri bulunmaktadır. Ninject popüler olanlarından sadece birisidir. Diğerleri de benzer ilkeler ile çalışmakta ve temel olarak IoC (Inversion of Control) prensibini baz almaktadır.

Bu yazımızda Ninject aracının çok basit seviyede bağımlılıkları enjekte etme notkasında nasıl kullanılabileceği ele alınmıştır. [Detaylı bilgi için Dojo'ya](http://www.ninject.org/learn.html) uğramanız gerekmektedir. İlerleyen günlerde Ninject ile bağımlılıkların farklı seviyelerde nasıl oluşturulabileceğini de incelemeye çalışacağız. Yani yapıcı metod haricindeki metodlarda veya özellik (Property) seviyesinde bu bağımlılıkları nasıl tanımlayabileceğimize bakacağız. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
