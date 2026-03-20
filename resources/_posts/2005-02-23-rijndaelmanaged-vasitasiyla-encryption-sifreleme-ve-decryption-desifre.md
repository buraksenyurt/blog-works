---
layout: post
title: "RijndaelManaged Vasıtasıyla Encryption(Şifreleme) ve Decryption(Deşifre)"
date: 2005-02-23 12:00:00 +0300
categories:
  - csharp
tags:
  - csharp
  - dotnet
---
Bu makalemizde, Rijndael Algoritmasını kullanan Managed tiplerden RijndaelManaged sınıfı ile şifreleme (encryption) ve deşifre etme (decryption) işlemelerinin nasıl gerçekleştirilebileceğini incelemeye çalışacağız. Konu ile ilgili örneklerimize geçmeden önce.Net Framework içerisinde yer alan Cryptography mimarisinde kısaca bahsetmekte yarar olduğunu düşünüyorum. Aşağıdaki şekil,.Net Framework’ te System.Securtiy.Cryptograhpy isim alanında yer alan şifreleme hiyerarşisini göstermektedir. Framework mimarisinde şifreleme sistemi ilk olarak üç ana katmandan oluşur. İlk katmanda taban sınıflar (base classes) yer alır. Bunlar SymmetricAlgorithm, AsymmetricAlgorithm ve HashAlgorithm sınıflarıdır. Bu sınıflar kendisinden türeyen ikinci katman sınıfları için temel ve ortak şifreleme özelliklerini içerirler.

![mk115_1.gif](/assets/images/2005/mk115_1.gif)

SymmetricAlgorithm sınıfını kullanan şifreleme mekanizmalarında herhangi bir anahtar ile şifrelenen veriler, deşifre edilmek istendiklerinde yine aynı anahtarı kullanırlar. Bu özellikle internet gibi herkesin kullanımına açık olan ortamlarda güvenlik açısından tehlike yaratabilir. Nitekim anahtarın herhangi bir şekilde ele geçirilmesi, şifrelenen verinin çözülmesi için yeterli olacaktır. Diğer yandan bu tekniğe göre geliştirilen algoritmalar hızlı ve performanslı çalışırlar.

![dikkat.gif](/assets/images/2005/dikkat.gif)
SymmetricAlgorithm katmanından türeyen Encryption sınıfları ile uygulanan şifreleme algoritmalarında veriyi şifrelerken kullandığımız key (anahtar) ve IV (vektör) değerleri, aynı veriyi deşifre ederken de gereklidir.

SymmetricAlgorithm yapısını kullanan şifreleme mimarilerinin neden olduğu güvenlik sorununun çözümü için AsymmetrciAlgorithm taban sınıfı (base class) geliştirilmiştir. Bu mekanizmada veri şifreleneceği zaman public bir anahtar kullanılır. Bu anahtarın herhangi bir şekilde ele geçirilmesi, verinin deşifre edilebilmesi için yeterli değildir. Nitekim verinin deşifre (decryption) edilebilmesi için karşı tarafın private bir anahtara gereksinimi vardır. Bu avantajının yanında AsymmetricAlgroithm mekanizması SymmetricAlgorithm mekanizmasına göre daha yavaş çalışmaktadır.

![dikkat.gif](/assets/images/2005/dikkat.gif)
AsymmetricAlgorithm katmanından türeyen Encryption sınıfları ile uygulanan şifreleme (encryption) algoritmalarında veriyi şifrelerken public bir key kullanırken, deşifre (decryption) işlemi sırasında farklı olan private key kullanılır.

Birinci katmanda yer alan taban sınıflar, abstract niteliktedir. Dolayısıyla kendisinden türeyen şifreleme sınıflarının içermesi ve uygulaması zorunlu olan üyeler içerirler. Bildiğiniz gibi abstract sınıflardan nesne örnekleri üretilemez. Ancak taban sınıfların static Create metotları yardımıyla bu sınıfları da şifreleme mekanizmalarında kullanabiliriz. İkinci katmanda yer alan sınıflar ise, özellikle belirli şifrelme algoritmalarını işaret ederler. Örneğin bu gün işleyeceğimiz Rijndael algoritması 256 bitlik bir anahtar (key) ile şifreleme (deşifre etme) sağlar. Buradaki sınıflar, taban sınıflardan (base classes) türemiştir ve abstract sınıflardır.

Asıl şifreleme metotlarını ve üyelerini bizim için kullanışlı hale getiren sınıflar üçüncü katmanda yer alırlar. Burada dikkat edecek olursanız bazı sınıflar ServiceProvider kelimesi ile biterler. Bu sınıflar Windows’ un CryptoApi kütüphanesini kullanan sınıflardır. Diğer yandan Managed kelimesi içeren sınıflar (örneğin RijndaelManaged) özellikle.net için geliştirilmiş yönetimsel uyarlamalardır (Managed Implementations). Buradaki sınıflar sealed olarak tanımlanmıştır. Yani kendilerinden türetme yapılamaz. Buna rağmen eğer istersek ikinci veya birinci katman sınıflarını kullanarak kendi şifreleme algoritma sınıflarımızı veya uyarlamalarımızı geliştirebiliriz.

Peki bir veri kümesini şifrelemek için yukarıdaki katmanları ve içeriklerini nasıl kullanabiliriz. Bu makalemizde biz örnek olarak Rijndael algoritmasını kullanan iki örnek geliştireceğiz..Net içerisinde verileri şifrelemek için izleyeceğimiz yolda anahtar nokta CryptoStream sınıfıdır. Bu sınıfa ait nesne örnekleri yardımıyla verileri stream bazlı olarak, istenen algoritmaya göre şifreleyebilir ya da deşifre edebiliriz. CryptoStream sınıfından bir nesne örneğini aşağıdaki yapıcı metot (constructor) yardımıyla oluşturabiliriz.

```text
public CryptoStream(Stream stream, ICryptoTransform transform, CryptoStreamMode mode);
```

Bu metodun ilk parametresine dikkat edecek olursanız bir Stream nesnesidir. CryptoStream sınıfı şifrelenecek veya deşifre edilecek verilerin işlenmesi sırasında Stream nesnelerini kullanılır. Dolayısıyla bellek üzerinde tutulan verileri, fiziki dosyalarda tutulan verileri veya network üzerinden akan verileri ilgili stream nesneleri yardımıyla (MemoryStream, FileStream, NetworkStream vb...) CrpytoStream sınıfına ait bir nesne örneğine aktarabiliriz. İkinci parametre ise Stream üzerindeki verinin hangi algoritma ile şifreleneceğini (deşifre edileceğini) belirlemek üzere kullanılır. Bu parametre ICryptoTransform ara yüzü tipinden bir nesne örneğidir. Üçüncü parametre ise Stream’ e yazma veya stream’ den okuma yapılacağını belirtir.

Stream üzerindeki veriyi şifreleyeceğimiz zaman üçüncü parametre Write değerini alırken, deşifre işlemlerinde Read değerini alır. Karışık gibi görünmesine rağmen örneklerimizden de göreceğiniz gibi, şifreleme (deşifre etme) işlemleri sanıldığı kadar zor değildir. Dilerseniz vakit kaybetmeden örneklerimize geçelim. İlk örneğimizde metin tabanlı bir dosya içeriğini FileStream nesnesinden faydalanarak okuyor ve CyrptoStream sınıfına ait nesne örneği yardımıyla şifreleyerek bir dosyaya yazıyoruz. Daha sona ise şifrelenen bu dosyayı tekrardan çözüyoruz.

```csharp
using System;
using System.IO;
using System.Security.Cryptography;

namespace CryptoStreamS1
{
    class Class1
    {
        [STAThread]
        static void Main(string[] args)
        {
            #region Dosya Şifrelemesi (Rijndael algoritması ile)

            // İlk olarak şifrelemek istediğimiz dosya için bir stream oluşturuyoruz.
            FileStream fs=new FileStream(@"SifreliDosya.txt",FileMode.OpenOrCreate,FileAccess.Write);

            /* Kullanacağımız şifreleme algoritmasını uygulatabileceğimiz managed Rijndael sınıfına ait nesne örneğimizi tanımlıyoruz. Şifreleme algoritması olarak Rijndael tekniğini kullanıyoruz. */
            RijndaelManaged rm=new RijndaelManaged();
            rm.GenerateKey(); // Aalgoritma için gerekli Key üretiliyor.

            /* Şimdi algoritma için gerekli key ve vektör değerlerini üretiyoruz. Burada kullanılan şifreleme algoritması simetrik yapıda olduğundan şifrelenen verinin açılabilmesi için (decrypting) aynı key ve vektör değerine ihtiyacımız var. Bu nedenle bunları bir byte dizisinde tutuyoruz.*/ 

            // Elde ettiğimiz key değerini bir byte dizisine aktarıyoruz.
            byte[] k=new byte[rm.Key.Length];
            for(int i=0;i<rm.Key.Length;i++)
            {
                Console.Write(rm.Key[i]);
                k[i]=rm.Key[i];
            } 

            Console.WriteLine();

            rm.GenerateIV(); // Algoritma için gerekli IV vektör değeri üretiliyor.
            byte[] v=new byte[rm.IV.Length];

            // Elde ettiğimiz Vektör değerini bir byte dizisine aktarıyoruz.
            for(int i=0;i<rm.IV.Length;i++)
            {
                Console.Write(rm.IV[i]);
                v[i]=rm.IV[i];
            }
    
            Console.WriteLine();

            /* Belirlediğimiz şifreleme algoritmasını kullanarak, stream üzerinde şifrelemeyi yapacak CryptoStream nesnemizi oluşturuyoruz. Şifrelemeyi oluşturmak için RijndaelManaged sınıfından örneklendirdiğimiz nesnemizin CreateEncyrptor metodunu kullanıyoruz. Oluşturulan şifreli dökümanı ilgili stream’ e yazmak istediğimizdenCryptoStreamMode olarak Write değerini seçiyoruz.*/
            CryptoStream cs=new CryptoStream(fs,rm.CreateEncryptor(),CryptoStreamMode.Write);

            /*Şimdi şifrelenecek olan byte dizisini almak üzere dosyamız için bir akım oluşturuyoruz. Nitekim CryptoStream’ in aşağıda kullanılan aşırı yüklenmiş versiyonu ilk parametre olarak şifrelenecek veri yapısını bir byte dizisi halinde alıyor.*/ 
            FileStream fs2=new FileStream(@"Dosya.txt",FileMode.Open);
            // dosyanın içeriğini byte dizisine aktarıyoruz.
            byte[] veriler=new byte[fs2.Length];
            fs2.Read(veriler,0,(int)fs2.Length);

            // CryptoStream sınıfının write metodu ile dosya.txt’ yi okuduğumuz byte dizisinin içeriğini fs2 ile belirttiğimiz stream’ e yazıyoruz.
            cs.Write(veriler,0,veriler.Length);
            fs2.Close();
            cs.Close();
            #endregion

            #region Şifrelenmiş dosyanın örnek olarak ilk satırının decrypt edilerek okunması.

            /* Bu kez işlemleri tersten yapıyoruz. İlk olarak şifrelenmiş ve decrypt edilmek istenen stream nesnesinin oluşturuyoruz. Ardından bu stream’     deki veriye Rijndael alogirtmasını uygulayarak Decrypting yapıyoruz. */
            FileStream fsSifreliDosya=new FileStream(@"SifreliDosya.txt",FileMode.Open,FileAccess.Read);
            RijndaelManaged rm2=new RijndaelManaged();
            //simetrik algoritma kullandığımız için decrypting içinde aynı key ve vektör değerlerini kullanmamız gerekiyor.
            rm2.Key=k;
            rm2.IV=v;
            CryptoStream cs2=new CryptoStream(fsSifreliDosya,rm2.CreateDecryptor(),CryptoStreamMode.Read); 
            StreamReader sr=new StreamReader(cs2);
            string satir=sr.ReadLine();
            Console.WriteLine(satir);
            #endregion
        }
    }
}
```

Uygulamayı çalıştırdığımızda orijinal içerikli dosyanın aşağıdaki gibi şifrelendiğini görürüz.

![mk115_5.gif](/assets/images/2005/mk115_5.gif)

İkinci örneğimizde ise, MemoryStream nesnesinden yararlanacağız. Bu kez, bir veri tablosundan çektiğimiz belli bir alanı bellek üzerinden şifreliyor ve daha sonra şifrelenen verinin orijinal içeriğini elde edecek şekilde deşifre işlemini uyguluyoruz.

```csharp
using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace CryptoStreamS2
{
    class Kriptografi2
    {
        [STAThread]
        static void Main(string[] args)
        {     
            #region verinin şifrelenmesi
            /* Öncelikle şifrelemek istediğimiz veriyi elde ediyoruz. Örnek olarak SQL Sunucusundaki Ogrenciler tablosundan belirli bir alanı aldık. */
            SqlConnection con=new SqlConnection("data source=BURKI;database=Work;integrated security=SSPI");
            SqlCommand cmd=new SqlCommand("SELECT AD FROM Ogrenciler WHERE OGRENCINO=1",con); 
            con.Open();
            string sifrelenecekVeri=cmd.ExecuteScalar().ToString(); 
            con.Close();

            /* Şifrelenecek verinin herşeyden önce bir byte dizisi olarak ele alınması gerekiyor.*/
            byte[] sv=new byte[sifrelenecekVeri.Length];
            for(int i=0;i<sv.Length;i++)
            {
                sv[i]=(byte)sifrelenecekVeri[i];
            }

            /* Şifrelenecek veriyi belleğe yazacağız. Bu nedenle MemoryStream sınıfı tipinden bir nesne örneği oluşturduk*/
            MemoryStream ms=new MemoryStream();
            /* Şifreleme algoritması olarak Rijnadel tekniğini sağlayan Managed nesne örneğimizi oluşturuyoruz.*/
            System.Security.Cryptography.RijndaelManaged rm=new RijndaelManaged();

            /* Şifreleme için gerekli anahtar ve vektör değerlerini elde ediyoruz.*/
            rm.GenerateKey();
            rm.GenerateIV();

            /* RijndaelManaged nesnesi tarafından üretilen anahtar ve vektör değerlerini byte dizilerine alıyoruz. Nitekim karşı tarafın şifrelenen veriyi çözebilmesi için bu anahtar ve vektör değerlerinin aynılarına ihtiyaçları olacaktır.*/
            byte[] anahtar=rm.Key;
            byte[] vektor=rm.IV;

            /* Veriyi belirttiğimiz algoritmaya göre şifreleyerek parametre olarak verilen stream’ e ki burada MemoryStream’ e yazmak için CryptoStream sınıfımızdan nesne örneğimizi oluşturuyoruz.*/
            CryptoStream cs=new CryptoStream(ms,rm.CreateEncryptor(anahtar,vektor),CryptoStreamMode.Write);
            /* Veriyi şifreleyerek belleğe yazıyoruz. Başından sonuna kadar.*/
            cs.Write(sv,0,sv.Length); 
            cs.FlushFinalBlock();

            Console.Write("Verinin şifrelenen hali ");
            byte[] icerik=ms.ToArray(); /* Belleğe yazdığımız şifrelenmiş veriyi bir byte dizisine alarak okuyor ve ekrana yazdırıyoruz.*/
            for(int i=0;i<icerik.Length;i++)
            {
                Console.Write((char)icerik[i]);
            }
            Console.WriteLine();
            #endregion

            #region şifrelenen verinin çözümlenmesi
            /* Bellekte tutulan icerik değerini yani şifrelenmiş olan veriyi parametre alan stream nesnemizi oluşuturuyoruz.*/
            MemoryStream msCoz=new MemoryStream(icerik);
            RijndaelManaged rmCoz=new RijndaelManaged(); // Rijndael algoritmasını kullanarak şifrelenen veriyi çözecek olan provider nesnemizi     tanımlıyoruz.*/
            /* SymmetricAlgorithm söz konusu olduğundan RijndaelManaged sınıfına ait nesne örneğinin decryption işlemi için encrypt’ te kullanılan key ve IV değerlerine ihtiyacımı var.*/
            rmCoz.Key=anahtar;
            rmCoz.IV=vektor;
            /* Bu kez CryptoStream nesnemiz stream’ den okuduğu veri üzerinde Decypting işlemini gerçekleştirecek. Bu nedenle Rijndael nesne     örneğimizin CreateDecryptor metodunu çağırıyoruz.*/
            CryptoStream csCoz=new CryptoStream(msCoz,rmCoz.CreateDecryptor(anahtar,vektor),CryptoStreamMode.Read);
            byte[] cozulen=new byte[ms.Length]; // Çözülen veriyi tutacak bir byte dizisi oluşturuyoruz.
            csCoz.Read(cozulen,0,icerik.Length); // Şifrelenen veriyi çözümleyerek okuyoruz.
            Console.Write("Şifrelenen verinin çözülmüş hali "); 
            /* Çözümlenmiş veriyi son olarak ekrana yazdırıyoruz.*/
            for(int i=0;i<cozulen.Length;i++)
            {
                Console.Write((char)cozulen[i]);
            }
            Console.ReadLine();
            #endregion
        }
    }
}
```

Uygulamamızı arka arkaya çalıştırdığımızda aşağıdakine benzer sonuçlar alırız. Dikkat ederseniz deşifre edilen veri her seferinde aynı olmasına rağmen, şifrelenen veri içeriği bir birlerinden farklıdır.

![mk115_2.gif](/assets/images/2005/mk115_2.gif)

![mk115_3.gif](/assets/images/2005/mk115_3.gif)

![mk115_4.gif](/assets/images/2005/mk115_4.gif)

Bu makalemizde kısaca Rijndael algoritmasını kullanan RijndaelManaged sınıfı ile şifreleme ve deşifre işlemlerini incelemeye çalıştık. İlerleyen makalelerimizde, AsymmetricAlgorithm tekniğininin nasıl uygulanabileceğini incelemeye çalışacağız. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek kodlar için tıklayın.](/assets/files/2005/Cryptography.rar)