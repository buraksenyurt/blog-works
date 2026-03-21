---
layout: post
title: "Simetrik Şifreleme(Symmetric Cryptography) Hızları"
date: 2014-09-28 04:00:00 +0300
categories:
  - dotnet-framework-4-5
tags:
  - cryptography
  - encryption
  - decryption
  - şifreleme
  - algoritma
  - symmetric-encryption
---
Crptograpy… Hayır hayır şöyleydi.Crpytograyph… Yok yok böyle değil. Belki de… Cyrptograhy… Offf! Bir türlü beceremiyorum şunu yazmayı. Hah sanırım buydu. Cryptography. Nihayet! Şifreleme teknikleri sizlerin de bildiği üzere karmaşık matematiksel algoritmaları baz alacak şekilde tesis edilmeye çalışılırlar. Algoritma ne kadar karmaşık veya basit olursa olsun önemli olan çıkan sonuçların zor kırılacak cinsten olmalarıdır. Bu noktada kullanılan anahtarların ve bit değerlerinin de büyük önemi vardır. E tabi doğal olarak zaman içerisinde pek çok şifreleme algoritması ortaya çıkmıştır. Bunları temel de simetrik ve asimetrik olanlar gibi iki ana kategoriye ayırabiliriz. Ama diğer yandan hash algoritmaları veya veri bütünlüğünü korumaya yönelik algoritmalar da mevcuttur.(Ve belki de bizim bilmediğimiz ama üst düzey veri güvenliğinde kullanılan çok gizli olanları)

[![62374](/assets/images/2014/62374_thumb.gif)](/assets/images/2014/62374.gif)

.Net Framework gibi pek çok programlama geliştirme ortamı, bu tip şifreleme algoritmalarının kod içerisinde kolayca ele alınmasını sağlayacak cinsten tipler/sınıflar içermektedirler. Genellikle hangisinin kullanılacağını tercih ederken simetrik veya asimetrik şifreleme yapılıp yapılmayacağına ya da ne kadar zor kırılacağına bakılır (AES’ in bu konuda bazı yarışmaları var. İnceleyin derim). Bunlardan herhangibiri seçilirse bu durumda farklı faktörleri de göz önüna almamız gerekebilir. Örneğin algoritmanın şifreleme ve çözümleme işlemlerini ne kadar sürede yaptığı da önemli bir kriter olabilir.

Küçük içerikler söz konusu olduğunda bu çok da önem arz eden bir konu değildir, lakin elimizde n sayıda büyük boyutlu, şifrelenmesi gereken ve yeri geldiğinde de çözülecek olan veri kümeleri varsa bu durumda hız önemli bir faktör olabilir.

Biz bu yazımızda, simetrik şifreleme algoritmalarından olan AES (Advenced Encryption Standard’ in Cryptographic Application Programming Interfaces uyarlaması), TripleDES, DES, RC2 ve Rijndael tekniklerini ele alıp, büyük boyutlu bir veri içeriği için şifreleme ve çözümleme zamanlarını ölçümlemeye çalışacağız. (Hem bu vesile ile söz konusu sağlayıcıları pratik olarak nasıl kullanabileceğimizi de göreceğiz)

Temel Cryptography Tipleri

Başlamadan önce Visual Studio – Object Browser yardımıyla söz konusu şifreleme algoritmalarının tip hiyerarşisine bakmamızda yarar olduğu kanısındayım. Temel olarak simetrik şifreleme işlemlerini üstlenen sağlayıcılar (Providers) SymmetricAlgorithm tipinden türeyen alt tiplerden üretilirler.

[![scs_4](/assets/images/2014/scs_4_thumb.png)](/assets/images/2014/scs_4.png)

Aslında şifreleme için kullanılacak tipler Provider veya Managed son eki ile biten sınıflardır. Tüm bu sınıflar tepede yer alan SymmetricAlgorithm isimli abstract sınıftan türemektedir.

> Bu açıdan bakıldığında, Dependency Injection kullanılarak şifreleme işlemleri için bir üst provider yazılması ve konfigurasyon bazlı olarak ele alınması sağlanabilir. Bu sayede hangi şifreleme sağlayıcısını kullanmak istiyorsak, uygulama ortamına kolayca enjekte edebiliriz.

Tüm bu şifreleme algoritmalarının ortak özelliklerinden birisi de paylaşımlı anahtar (Shared Key) kullanıyor olmalarıdır. Yani, şifreleme için kullanılan anahtar (Key) ve ilklendirme vektör değeri (IV-Initialization vector) ortaktır. Dolayısıyla şifreleme yapılırken kullanılan bu değerler, çözümleme sırasında da devreye girmektedir. Bu zaten, simetrik ve asimetrik şifrelemeler arasındaki en büyük farklardan birisidir.

Test Senaryosu

Artık söz fazla uzatmayalım ve kodlarımızı yazarak testimizi gerçekleştirmeye çalışalım. Test senaryomuzdaki amacımız, 5 şifreleme algoritmasının aynı veri üzerindeki şifreleme ve çözümleme sürelerini ölçmek olacaktır. Bunun için yaklaşık olarak 50 mb büyüklüğünde bir text dosyasını kullandığımı ifade etmek isterim. Text dosyası içeriği ise bildiğimiz Lorem Ipsum paragraflarından oluşmaktadır. [(Lorem Ipsum üretimleri için bu adresten yararlanabilirsiniz)](http://tr.lipsum.com/)

[![scs_6](/assets/images/2014/scs_6_thumb.png)](/assets/images/2014/scs_6.png)

Uygulama Kodları

Gelelim kod tarafına. Console uygulaması olarak geliştireceğimiz projemizde aşağıdaki sınıf diagramında yer alan kod içeriğini kullanıyor olacağız.

[![scs_3](/assets/images/2014/scs_3_thumb.png)](/assets/images/2014/scs_3.png)

```csharp
using System; 
using System.Diagnostics; 
using System.IO; 
using System.Security.Cryptography;

namespace HowTo_Cryptography 
{ 
    public static class Utility 
    { 
        #region Fields(Alanlar)

        private static string _targetContent = String.Empty; 
        private static Stopwatch _watcher = null;

        private static AesCryptoServiceProvider _aesProvider = null; 
        private static byte[] _aes_Key = null; 
        private static byte[] _aes_IV = null;

        private static TripleDESCryptoServiceProvider _tdesProvider = null; 
        private static byte[] _tdes_Key = null; 
        private static byte[] _tdes_IV = null;

        private static RijndaelManaged _rijndaelProvider = null; 
        private static byte[] _rijndael_Key = null; 
        private static byte[] _rijndael_IV = null;

        private static RC2CryptoServiceProvider _rc2Provider = null; 
        private static byte[] _rc2_Key = null; 
        private static byte[] _rc2_IV = null;

        private static DESCryptoServiceProvider _desProvider = null; 
        private static byte[] _des_Key = null; 
        private static byte[] _des_IV = null;

        #endregion

        static Utility() 
        { 
            _aesProvider = new AesCryptoServiceProvider(); 
            _aes_Key = _aesProvider.Key; 
            _aes_IV = _aesProvider.IV;

            _tdesProvider = new TripleDESCryptoServiceProvider(); 
            _tdes_Key = _tdesProvider.Key; 
            _tdes_IV = _tdesProvider.IV;

            _rijndaelProvider = new RijndaelManaged(); 
            _rijndael_Key = _rijndaelProvider.Key; 
            _rijndael_IV = _rijndaelProvider.IV;

            _rc2Provider = new RC2CryptoServiceProvider(); 
            _rc2_Key = _rc2Provider.Key; 
            _rc2_IV = _rc2Provider.IV;

            _desProvider = new DESCryptoServiceProvider(); 
            _des_Key = _desProvider.Key; 
            _des_IV = _desProvider.IV;

            _targetContent = ReadContent(); 
            _watcher = new Stopwatch(); 
        }

        #region AES(Advanced Encyption Standard)

        // Şifreleme metodu 
        public static byte[] AES_Encrypt() 
        { 
            return Encypt<AesCryptoServiceProvider>(_aesProvider, _aes_Key, _aes_IV); 
        }

        // Çözümleme metodu 
        public static string AES_Decrypt(byte[] source) 
        { 
            return Decrypt<AesCryptoServiceProvider>(_aesProvider, source, _aes_Key, _aes_IV); 
        }

        #endregion

        #region TripleDES

        // Şifreleme metodu 
        public static byte[] TripleDES_Encrypt() 
        { 
            return Encypt<TripleDESCryptoServiceProvider>(_tdesProvider, _tdes_Key, _tdes_IV); 
        }

        // Çözümleme metodu 
        public static string TripleDES_Decrypt(byte[] source) 
        { 
            return Decrypt<TripleDESCryptoServiceProvider>(_tdesProvider, source, _tdes_Key, _tdes_IV); 
        }

        #endregion

        #region Rijndael

        // Şifreleme metodu 
        public static byte[] Rijndael_Encrypt() 
        { 
            return Encypt<RijndaelManaged>(_rijndaelProvider, _rijndael_Key, _rijndael_IV); 
        }

        // Çözümleme metodu 
        public static string Rijndael_Decrypt(byte[] source) 
        { 
            return Decrypt<RijndaelManaged>(_rijndaelProvider, source, _rijndael_Key, _rijndael_IV); 
        }

        #endregion

        #region RC2

        // Şifreleme metodu 
        public static byte[] RC2_Encrypt() 
        { 
            return Encypt<RC2CryptoServiceProvider>(_rc2Provider, _rc2_Key, _rc2_IV); 
        }

        // Çözümleme metodu 
        public static string RC2_Decrypt(byte[] source) 
        { 
            return Decrypt<RC2CryptoServiceProvider>(_rc2Provider, source, _rc2_Key, _rc2_IV); 
        }

        #endregion

        #region DES

        // Şifreleme metodu 
        public static byte[] DES_Encrypt() 
        { 
            return Encypt<DESCryptoServiceProvider>(_desProvider, _des_Key, _des_IV); 
        }

        // Çözümleme metodu 
        public static string DES_Decrypt(byte[] source) 
        { 
            return Decrypt<DESCryptoServiceProvider>(_desProvider, source, _des_Key, _des_IV); 
        }

        #endregion

        #region Generic şifreleme ve çözümleme metodları

        static byte[] Encypt<T>(T provider,byte[] key,byte[] iv) 
            where T:SymmetricAlgorithm 
        { 
            byte[] result = null; 
            ICryptoTransform encryptor = provider.CreateEncryptor(key, iv);

            _watcher.Restart();

            using (MemoryStream ms = new MemoryStream()) 
           { 
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write)) 
               { 
                    using (StreamWriter sWriter = new StreamWriter(cs)) 
                    { 
                        sWriter.Write(_targetContent); 
                    } 
                    result=ms.ToArray(); 
               } 
           }

            _watcher.Stop(); 
            Console.WriteLine("Encrypt\t{0}\n{1}",provider.ToString(),_watcher.ElapsedMilliseconds.ToString());

            return result; 
       }

        static string Decrypt<T>(T provider,byte[] source, byte[] key, byte[] iv) 
            where T : SymmetricAlgorithm 
        { 
            string result = String.Empty;

            ICryptoTransform decryptor = provider.CreateDecryptor(key, iv);

            _watcher.Restart();

            using (MemoryStream ms = new MemoryStream(source)) 
            { 
               using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read)) 
                { 
                    using (StreamReader sReader = new StreamReader(cs)) 
                   { 
                        result=sReader.ReadToEnd(); 
                    } 
                } 
           }

            _watcher.Stop(); 
            Console.WriteLine("Encrypt\t{0}\n{1}\n", provider.ToString(), _watcher.ElapsedMilliseconds.ToString());

            return result; 
        }

        #endregion

        #region Yardımcı metodlar

        static string ReadContent() 
        { 
            return File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "SampleDocument.txt")); 
        }

        #endregion 
    } 
}
```

Aslında kod içeriği her ne kadar karmaşık görünse de region’ ları kapatıp büyük resme baktığımızda iskelet daha rahat anlaşılabilir. (Hafiften bir BLOB veya GOD Object AntiPattern’ ine kaymış gibiyiz ama bunu önemsemesekte olur)

[![scs_5](/assets/images/2014/scs_5_thumb.png)](/assets/images/2014/scs_5.png)

Kodun Çalışma Şekli

Şimdi neler yaptığımızı kısaca özetleyelim.

Öncelikli olarak static Utility sınıfı içerisinde bazı temel alanlar (Fields) yer aldığı görülmektedir. Her bir şifreleme sağlayıcısı (Provider) ve bunlara ait Key, IV değerleri için birer alan tanımlanmıştır. Söz konusu alanlar static yapıcı metod (Constructor) içerisinde initialize edilirler. Ayrıca yapıcı metodumuz, 50 mb’ lık text dosyasını da içerdeki string değişkene alacak şekilde tesis edilmiştir.

5 farklı simetrik şifreleme algoritması için ayrı ayrı metodlar yazıldığı görülmektedir ancak her biri generic olarak tasarlanmış Encypt (ki an itibariyle yanlış yazdığımı fark ettim ![Open-mouthed smile](/assets/images/2014/wlEmoticon-openmouthedsmile_36.png) Sanırım enkıyipt olarak telafüz edebiliriz) ve Decrypt fonksiyonlarını kullanmaktadır. Bu fonksiyonlarda dikkat edilmesi gereken en önemli nokta ise generic T tipi için bir kısıtlama (Constraint) belirtilmiş olmasıdır ki bu kısıtlamaya göre T tipi mutlaka SymmetricAlgorithm türevli olmak zorundadır.

> Yazıya konu olan kod parçasının en önemli kısımı, generic metod kullanılması ve T tipi için constraint uygulanarak, sadece simetrik algoritma sağlayacılarına destek verecek ortak fonksiyonelliklerin üretilmiş olmasıdır. Bu kodun yeniden kullanılabilirliği (re-usable), okunabilirliği (readable) ve bakımı (maintable) açısından önemlidir.

Aslında Provider’ ların kullanımları da aynıdır. Şifreleme işlemleri için ICryptoTransform arayüzü (Interface) tipi üzerinde taşınabilecek şekilde bir tip üretilmektedir. Bunun için kullanılan CreateEncryptor metodu parametre olarak Key ve IV değerlerini alır. Bu değerler tahmin edeceğiniz üzere kullanılan şifreleme algoritmasına özeldir. Benzer şekilde çözümleyici için de CreateDecryptor metodlarından yararlanılmaktadır ki bu fonksiyon da parametre olarak Key ve IV değerlerini alır.

Gerek şifreleme gerek çözümleme operasyonları olsun, her ikisinde de CryptoStream tipinden yararlanılmaktadır. Eğer bir şifreleme işlemi söz konusu ise doğal olarak CryptoStreamMode.Write modu, tersine bir çözümleme operasyonu yapılacaksa CryptoStreamMode.Read modu kullanılır.

Şifreleme işlemleri sonucunda provider’ lar içerikleri bir byte[] dizisine yazarlar. Bu noktada StreamWriter sınıfından yararlanılmaktadır. Çözümleme işlemlerinde ise byte[] tipinden olan içeriğin genellikle bir string katarına alınması söz konusudur. Bu noktada da, StreamReader tipinden yararlanılmaktadır.

Aslında yazma/şifreleme operasyonundaki yapı şu şekilde özetlenebilir.

```csharp
using (MemoryStream ms = new MemoryStream()) 
            { 
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write)) 
                { 
                    using (StreamWriter sWriter = new StreamWriter(cs)) 
                    { 
                        sWriter.Write(_targetContent); 
                    } 
                    result=ms.ToArray(); 
                } 
            }
```

StreamWriter, CryptoStream’ e yazar. CryptoStream ise MemoryStream’ e. Son olarak MemoryStream örneği üzerine yazılan içerik bir byte[] array’ e atanır.

Okuma/çözümleme işleminde ise,

```csharp
using (MemoryStream ms = new MemoryStream(source)) 
{ 
    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read)) 
    { 
        using (StreamReader sReader = new StreamReader(cs)) 
        { 
            result=sReader.ReadToEnd(); 
        } 
    } 
}
```

bu kez StreamReader, CryptoStream’ e yazar. CryptoStream’ de yine MemoryStream’e. Son olarak StreamReader nesne örneği üzerinden ReadToEnd ile çözümlenen içeriğin string karşılığı alınır.

Örnekte süre ölçümlemesi için Stopwatch tipinden yararlanılmıştır. Bu tipin Restart ve Stop edildiği aralıktaki işlemlerin süresi hesaplanmaktadır. Bu hesaplamalar şifreleme ve çözümleme işlemlerinde devreye giren generic metodlarda yapılmaktadır.

Main metodunun bulunduğu Program sınıfının içeriğini ise aşağıdaki gibi kodlayabiliriz.

```csharp
using System;

namespace HowTo_Cryptography 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            for (int i = 0; i < 5; i++) 
            { 
                Console.WriteLine("***Test Case Start***"); 
                TestMethod(); 
                Console.WriteLine("***Test Case End***"); 
            } 
        }

        private static void TestMethod() 
        { 
            string content = String.Empty;

            byte[] aesEncrypted = Utility.AES_Encrypt(); 
           content = Utility.AES_Decrypt(aesEncrypted);

            byte[] tdesEncrypted = Utility.TripleDES_Encrypt(); 
            content = Utility.TripleDES_Decrypt(tdesEncrypted);

            byte[] rijndaelEncrypted = Utility.Rijndael_Encrypt(); 
            content = Utility.Rijndael_Decrypt(rijndaelEncrypted);

            byte[] rc2Encrypted = Utility.RC2_Encrypt(); 
            content = Utility.RC2_Decrypt(rc2Encrypted);

            byte[] desEncrypted = Utility.DES_Encrypt(); 
            content = Utility.DES_Decrypt(desEncrypted); 
        } 
    } 
}
```

Test Sonuçları

Test olması açısından 5 simetrik algoritmanın şifreleme ve çözümleme metodlarını arka arkaya 5 kez çağıran bir yapı kurgulanmıştır. Örneği bu şekilde çalıştırdığımızda aşağıdakine benzer sonuçlar ile karşılaşırız. Tabi buradaki ölçüm değerleri donanımsal faktörlere de bağlıdır.

> Testler, 4 çekirdekli ve 4Gb Ram kullanan intel İ5 chipset’ li bir desktop PC üzerinden, Windows 7 işletim sistemi ortamında Visual Studio 2012 ve.Net Framework 4.5 kullanılarak geliştirilmiş bir uygulama tarafından yapılmıştır.

[![scs_1](/assets/images/2014/scs_1_thumb.png)](/assets/images/2014/scs_1.png)

Sonuçları daha iyi irdelemek adına süreleri bir Excel grafiğinde birleştirebiliriz. Aynen aşağıda görüldüğü gibi

![Winking smile](/assets/images/2014/wlEmoticon-winkingsmile_143.png)

[![scs_2](/assets/images/2014/scs_2_thumb.png)](/assets/images/2014/scs_2.png)

Elde edilen sonuçlara göre en hızlı şifreleme ve çözümleme AES sağlayıcısı tarafından gerçekleştirilmiştir. En yavaş olan algoritma ise Triple DES’ tir. Genel olarak tüm algoritmaların şifreleme süreleri çözümleme sürelerine oranla daha düşüktür. Ancak RC2 algoritması tabanlı sağlaycının verdiği sonuçlar ilginçtir. Yapılan 5 test göz önüne alındığında, RC2 için çözümleme hızı, şifreleme hızına göre daha yüksek çıkmıştır.

Bu test örneğinde simetrik şifreleme algoritmalarının Encryption ve Decryption operasyonlarına ait süre ölçümlemelerine bakılmıştır. Hızın öne çıktığı bir durum söz konusu ise AES’ in tercih edilmesi elbette olasıdır ama yine de iyi düşünmek gerekir. Nitekim hangi algoritmanın kaç bitlik şifreleme yaptığına göre de karar verilmesi önemlidir. Bu açıdan bakıldığında, en sağlam algoritmalarından birisi olarak TripleDes ve Rijndael daha fazla öne çıkmaktadır.

> Tabi key boyutları dışında farklı etkenler de söz konusudur. Bilindiği üzere 2000li yılların başında şifreleme anahtarlarının en az 128bit destekli olması standart olarak kabul edilmiştir. Çok hassas verilerin korunmasında ise 192bit ile 256bit şifrelemeler önerilmektedir.
> National Institute of Standards and Technology (NIST) in ifade ettiğine göre, 80bit anahtar kullanan algoritmalar 2015 yılından itibaren geçerliliğini yitireceklerdir. Dolayısıyla bu ve altında (örneğin 56bit gibi) anahtarlar ile hizmet veren şifreleme algoritmalarının kullanılmaması doğru bir hareket olacaktır.

Bu yazımızda en bilinen ve popüler olan simetrik şifreleme algoritmalarının.Net Framework tarafındaki süre bazlı performanslarını incelemeye çalışıp, nasıl kullanıldıklarını gördük. Konu veri güvenliği olunca en sağlam sistemi de kursanız %1 ihtimalle kırılma olasılığını her zaman için göz önünde bulundurmak gerekir. Bu sebepten yazdıklarıma güvenmeyip daha derin bir araştırma ile hangi simetrik şifreleme algoritmasını seçeceğinize karar vermeniz yerinde bir hareket olacaktır. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[HowTo_Cryptography.zip (45,00 kb)](/assets/files/2014/HowTo_Cryptography.zip)

[Text dosyasının içeriği bilinçli olarak boşaltılıp 1Kb seviyesine indirilmiştir. Test etmeden önce dosya içeriğini copy-paste ile çoğaltarak boyut arttırımı yapmanızı ve 50mb seviyelerine getirdikten sonra kodu çalıştırmanızı öneririm]