---
layout: post
title: "Tasarım Desenleri - Strategy"
date: 2009-07-02 22:34:00 +0300
categories:
  - tasarim-kaliplari-design-patterns
tags:
  - tasarim-kaliplari-design-patterns
  - csharp
  - delegates
  - dependency-management
---
Bir süredir tasarım prensiplerini (Design Principles) incelemeye çalışıyoruz. Tabiki prensipler iyi bir yazılım modeli ve geliştirilmesi için yeterli değildir. Çoğu prensip kendi içerisinde çeşitli tasarım desenlerini (Design Patterns) uygulamaktadır. Bu nedenle ara sıra tasarım deseneleride bakmakta, öğrenmekte, daha önceden bakmışsak bile sık sık tekrar etmekte yarar var. Ben bu günkü yazımda sizlere davranışsal (Behavioral) kalıplardan olan Strategy tasarım desenini aktarmaya çalışacağım.

![blg41_1.jpg](/assets/images/2009/blg41_1.jpg)

[Youtube Link](https://www.youtube.com/watch?v=t8SZ2MFplLA)

Strategy deseni temel olarak, bir nesnenin her hangibir operasyonu gerçekleştirmek için kullanabileceği farklı algoritmaları içeren farklı tipleri kendi içerisinde ele alarak kullanması yerine, kullanmak istediği politikayı nasıl uygulandığını bilmesine gerek kalmaksızın sadece seçerek çalışma zamanında yürütmesine olanak tanımaktadır...Ughhh!!!

![Sealed](/assets/images/2009/smiley-sealed.gif)

Dediğimde sanıyorumki kafamızda pek bir şey oluşmamıştır. Hiç dert etmeyin bende bu deseni ilk öğrendiğim yıllarda bu tip cümleleri okurken etrafıma, havaya, doğaya şöyle bir bakıp kavramak için çaba harcardım. Bu nedenle gelin olayı önce senaryolaştıralım ve sonrasında bu cümleyi anlamaya çalışalım.

Elimizde string tipte verileri çeşitli algoritmalara göre şifreleyen ve tekrar eski haline getiren bir içerik tipi (Context Type) olduğunu varsaylım. Çok doğal olarak burada iki ana operasyon söz konusudur. Bunlardan ilki veriyi şifrelemek (Encryption) diğeride çözümlemek (Decryption) olarak düşünülebilir. Ne var bu string içeriklerin şifrelenmesi ve çözümlenmesi sırasında farklı tipte algoritmalar kullanılmak istenebilir. Örneğin Rijndael, Triple Des, SHA vb... Bu durumda ilk akla gelen içerik tipi içerisinde söz konusu şifreleme seçeneklerini ele almaktır. Bu da bir sürü if veya switch ile olayı kontrol altına almak anlamına gelebilir.

Ancak kaybettiğimiz önemli değerler vardır. Esneklik, genişletilebilirlik, test edilebilirlik vb...Örneğin, veriyi yeni bir algoritmaya göre (SHA 512 mesela) şifrelemek istediğimizde, içerik tipinin üzerinde kod değişikliği yapmamız gerekecektir. Halbuki içerik tipinin çalışma zamanında kullanıldığı yerde, sadece şifreleme algoritmasını seçmesinin sağlanması bunun önüne geçebilir. Dikkat etmemiz gereken noktalardan biriside, içerik tipinin şifreleme algoritmasının nasıl yapıldığının bilmesine gerek olmayışıdır. Diğer yandan bunu bilmeyecek ise nasıl çağıracaktır. Daha da önemlisi, istediğimizde yeni bir algoritmayı içerik tipinde değişiklik yapmadan sisteme nasıl ekleyebiliriz.

Tüm bu soruların cevabı aşağıdaki sınıf diagramında ve kod parçasında yer almaktadır.

![blg41_uml.gif](/assets/images/2009/blg41_uml.gif)

Kod içeriğimize gelince;

```csharp
using System;

namespace StrategyPattern
{
    // Strategy type
    interface IEncrypter
    {
        string Encrypt(string obj);
        string Decyrpt(string obj);
    }

    // ConcreteStrategy type 1
    class RijndaelEncrypter
        : IEncrypter
    {
        #region IEncrypter Members

        public string Encrypt(string obj)
        {
            Console.WriteLine("obj için Rijndael şifreleme");
            return obj;
        }

        public string Decyrpt(string obj)
        {
            Console.WriteLine("obj için Rijndael ters şifreleme");
             return obj;
        }

        #endregion
    }

    // ConcreteStrategy type 1
    class TripleDesEncrypter
        : IEncrypter
    {
        #region IEncrypter Members

        public string Encrypt(string obj)
        {
            Console.WriteLine("obj için TripleDES şifreleme");
            return obj;
        }

        public string Decyrpt(string obj)
        {
            Console.WriteLine("obj için TripleDES ters şifreleme");
            return obj;
        }

        #endregion
    }

    // Context Type
    class Encrypter
    {
        IEncrypter _enc=null;

        public Encrypter(IEncrypter enc)
        {
            _enc = enc;
        }

        public string Encrypt(string obj)
        {
             return _enc.Encrypt(obj);
        }
        public string Decyrpt(string obj)
        {
            return _enc.Decyrpt(obj);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string str = "<app><config><sqlConnection>data....</sqlConnection></config></app>";

            Encrypter enc1 = new Encrypter(new TripleDesEncrypter());            
            string encryptedStr=enc1.Encrypt(str);
            string decryptedStr = enc1.Decyrpt(str);

            enc1 = new Encrypter(new RijndaelEncrypter());
            encryptedStr = enc1.Encrypt(str);
            decryptedStr = enc1.Decyrpt(str);
        }
    }
}
```

Kodu incelediğinize göre biraz üzerinde konuşalım. Context tipimiz (Encrypter sınıfı) kendi içerisinde IEncrypter isimli bir interface kullanmakta ve bu arayüz üzerinde Encrypt ile Decrypt metodlarını çağırmaktadır. Bu bize şu esnekliği sağlamaktadır. IEncrypter arayüzünü uygulayan herhangibir tipi kendi içerisinde istediği şifreleme algoritmasını uygulayabilir. Context sınıfının bunu düşünmesine gerek yoktur. Bir başka deyişle, Context sınıfı ile asıl işi yapan algoritma tipleri arasında bir bağımlılık oluşmasına engel olunmaktadır. Buna göre herhanbiri zamanda, context tipinin kullanabileceği yeni bir şifreleme algoritması sisteme eklenebilir.

Tek yapılması gereken IEncrpyter arayüzünü implemente eden bir sınıfın yazılması ve çalışma zamanında bu tipin kullanılacağının söylenmesidir. Dikkat edileceği üzere Context tipi kendi içerisinde stratejik nesneye ait refaransı ele almaktadır. Dolayısıyla, arayüzlerin polimorfik özellikte olmaları nedeniyle, Context tipi içerisinde yer alan Encrpyt ve Decrypt metodları, yapıcı metod (Consturctor) ile çalışma zamanında gelen tip ne ise ona göre şifreleme ve çözümleme işlemlerini uygulayacaktır. Sanıyorumki şu anda ilk başta söylediğim o karışık cümle biraz daha anlaşılır hale gelmiştir.

![Wink](/assets/images/2009/smiley-wink.gif)

Tabi tam bu noktada insanın aklına C# 3.0 ve bazı şeytanlıklarda gelmiyor değil.

![Cool](/assets/images/2009/smiley-cool.gif)

Öyleki C# 3.0' da lambda operatatörümüz, Func<> gibi temsilcilerimi bulunmakta. Bu durumda yukarıdaki desenin C# 3.0 daki yetenekler ile yazmaya çalıştığımızda belki aşağıdaki gibi bir uygulanış şeklininde söz konusu olabileceğini söyleyebiliriz. İşte Encrypter tipimizin ikinci versiyonu.

```csharp
class EncrypterV2
    {
        public string Encrypt(Func<string,string> function,string obj)
        {
            return function(obj);
        }
        public string Decyrpt(Func<string, string> function, string obj)
        {
            return function(obj);
        }
    }
```

Görüldüğü gibi Encrypt ve Decrypt fonksiyonlarımız Func tipinden bir temsilciyi (delegate) parametre olarak almakta ve içeride uygulamaktadır.Buna göre EncyrpterV2 sınıfımızı kullanacağımız yerde, şifreleme ve ters şifreleme fonksiyonlarının kendimiz yazıp vermeliyiz. (Gerçi bu durumda Context tipinin, algoritmaların nasıl çalıştığı ve yapıldığını bilmesine gerek olmayışı ilkesi ile çelişilmektedir. Bunada dikkat edelim) Yani Context tipini çalışma zamanı için aşağıdaki gibi kullanabiliriz.

```csharp
string str = "<app><config><sqlConnection>data....</sqlConnection></config></app>";

            EncrypterV2 v2 = new EncrypterV2();

            v2.Encrypt(s =>
            {
                Console.WriteLine("{0}\n için TripleDes şifreleme yapılıyor\n", s);
                return s;
            },str);

            v2.Decyrpt(s =>
            {
                Console.WriteLine("{0}\n için TripleDes çözümleme yapılıyor\n", s);
                return s;
            }, str);
```

Ne diyebilirim ki. C# 3.0 sürpriz yeteneklerle dolu ve bazı temel esaslara bakış açımızı oldukça değiştiriyor.

![Wink](/assets/images/2009/smiley-wink.gif)

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[StrategyPattern.rar (22,61 kb)](/assets/files/2009/StrategyPattern.rar)
