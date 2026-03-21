---
layout: post
title: "NoSQL Maceraları - STSdb ile Hello World"
date: 2013-07-24 12:24:00 +0300
categories:
  - nosql
tags:
  - nosql
  - sts-db
  - key-value-storage
  - not-only-sql
---
[Matrix Reloaded'](http://en.wikipedia.org/wiki/The_Matrix_Reloaded) ı seyrettiğim zamanları düşündüğümde, anımsadıklarım arasında heyecanlı aksiyon sahnelerinde yer alan ve eski Amerikan stilini de yansıtan kocaman otomobiller vardı. (Hatta bildiğim kadarı ile ikinci dünya savaşı sonrası çelik stoklarının fazlalığı nedeniyle Amerikan otoları hep kocaman olmuşlardı)

[![Cadillac_STS](/assets/images/2013/Cadillac_STS_thumb_2.jpg)](/assets/images/2013/Cadillac_STS_2.jpg)

General Motors firmasına ait olan otomobillerden birisi de, Cadillac STS'in farklı bir versiyonu olan CTS idi. Tabi ben konuyu bir şekilde bu günkü yazının konusu olan STSdb'ye getirmek istediğimden [Cadillac STS](http://en.wikipedia.org/wiki/Cadillac_STS)'e ait bir fotoğrafa yer vermek istedim

![Smile](/assets/images/2013/wlEmoticon-smile_80.png)

Öyleyse vakit kaybetmeden konumuza geçelim.

Bilindiği üzere bir süredir NoSQL (Not only SQL) veritabanlarını incelemeye (öğrenmeye) çalışıyoruz. Daha önceki yazılarımızda Apache Cassandra, RavenDB ve DEX ürünlerine bir göz atmış ve.Net uygulamalarında nasıl kullanılabileceklerini görmüştük. Klasik olarak yaptığımız Hello World uygulamalarının bir benzerini de bu gün inceleyeceğimiz STSdb için gerçekleştiriyor olacağız.

Bu veritabanı için bazı kaynaklarda Revolutionary (Devrimci, devrimsel, devrim niteliğinde) sıfatı kullanılmış. Gerçekten böyle bir sistem olduğunu ispat etmemiz oldukça zor tabi. Yine de bu konu ile ilişkili yapılmış bir kaç veritabanı testine bakmak az da olsa fikir verebilir. Söz gelimi kaynağının ne kadar güvenilir olduğunu tam olarak bilmediğim [bu adreste](http://www.techmixer.com/revolutionary-database-paradigm-stsdb/), karşılaştırıldığı veritabanlarına göre en hızlısı olduğu iddia edilmiş.

Aslında bizim temel amacımız key-value modeline göre çalışan NoSQL veritabanlarından bir diğerini incelemektir

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_174.png)

Dilerseniz STSdb ürününü kısaca mercek atlına almaya başlayalım.

> STSdb ile ilişkili olarak karşılaştığım ilk sıkıntı dokümantasyonun yeteri kadar doyurucu olmamasıydı. MSDN benzeri API dokümantasyonu ve bir kaç örnek kod parçası haricinde, [forumlarda](http://stsdb.com/forum/forum.html) da yeteri kadar doyurucu içeriğe rastlayamadım. Amazon.com'da konu ile ilişkili bir kitabı da ne yazık ki bulamadım. Pek tabi zaman ilerledikçe bu durum değişebilir.
> Hal böyle olunca biraz dene-keşfet modeline göre öğrenilmeye çalışılan bir ürün oldu benim için. Kaldı ki API dokümantasyonuna baktığımızda gerçekten çok önemli görünen pek çok fonksiyonellik var. Örneğin Snapshot almak için XTable sınıfına ait bir metod var ama örnek bir kod parçası yok. Dolayısıyla biraz kurcalanması gereken bir ürün olarak karşımıza çıkmakta.

STSdb, Key-Value modeline göre çalışan açık kaynak (GPL 2 ve GPL 3. Ancak Community lisanslaması da yapılabiliyor) NoSql depolama API'lerinden birisidir..Net Framework üzerinde C# programlama dili kullanılarak geliştirilmiş gömülü (Embedded) bir sistem olarak karşımıza çıkıyor. Her platformu destekleyebilir ilkesini ilgili platformlarda Mono yüklü olması halinde karşılamakta (Tam bir plaform bağımsızlık olduğunu ifade edemeyiz ama Mono ile Linux, MacOS X ve Unix gibi sistemlere de entegre edilebilir)

Genel Özellikler

Genel özelliklerini ise aşağıdaki maddeler halinde ifade edebiliriz.

- .Net Framework’ e gömülü olarak geliştirildiğinden doğrudan uygulama ile ilişkilendirilip çalıştırılabiliyor. Yani ek bir konfigurasyon hazırlığına ihtiyaç yok.
- Veriyi disk üzerinde, bellekte (In-Memory) veya her ikisinin kombinasyonu olacak şekilde hibrid bir yapıda tutabilir.
- Kullanılabilirlik alanları oldukça geniştir. İnsan Makine Arayüzleri (Human Machine Interface), Süreç kontrol sistemleri (Process Control System), Otomotiv Endüstürisi, çeşitli tipte finansal uygulamalar vb...
- Mantıksal modeli iki katmandan oluşmaktadır. Birinci kat dahili dosya sistemidir (File System). Bu katman 2üzeri64 dosyaya (Her biri ile de 2üzeri64 byte’ a çıkılabilir) kadar destek vermektedir.
İkinci katman ise tabloların tutulduğu veritabanıdır (Database Layer). Burada tablo yönetimine ilişkin işlemler yapılır. Tabloların oluşturulması, silinmesi, tablolar üzerinde ileri/geri hareket edilmesi, veya düzenlenmesi bu operasyonlar arasında sayılabilir.
- Transaction desteği vardır. Bu nedenle Atomicity, Consistency, Isolation ve Durability olarak bilinen ACID prensiplerini karşılamaktadır.
- Karmaşık bir.Net tipi, primary key olarak kullanılabilir.
- Snapshot özelliği sayesinde gerçek zamanlı yedekleme (Real-time Backup) imkanı da sunar.
- Çok doğal olarak.Net Framework üzerinde geliştirilmiş olduğundan LINQ (Language INtegrated Query) desteğine sahiptir.
- Veriyi bir algoritma yardımıyla sıkıştırmaktadır. Sıkıştırma önemli ölçüde yer kazanımına da imkan sağlamaktadır.
- Belki de en önemli özelliklerinden birisi tutabileceği tablo veya kayıt için bir üst limit değerinin olmayışıdır. Bu nedenle çok büyük boyutlu veri kümeleri için tercih edilebilir.

İlk olarak ürünü tedarik etmemiz gerekiyor tabi ki

![Laughing out loud](/assets/images/2013/wlEmoticon-laughingoutloud_5.png)

Bunun için [şu adresi](http://stsdb.com/products/stsdb-w4.0/downloads/embedded-server/) kullanabiliriz. İndirilen içerik aşağıdaki gibidir. (Yazının yayınlandığı tarihi itibariyle 4.0 RC sürümüde mevcut)

[![sts_2](/assets/images/2013/sts_2_thumb.png)](/assets/images/2013/sts_2.png)

Tahmin edileceği üzere STSdb.dll assembly’ ının, projeye referans edilmesi kullanılması için yeterlidir.

[![sts_1](/assets/images/2013/sts_1_thumb.png)](/assets/images/2013/sts_1.png)

Console uygulaması olarak geliştireceğimiz programımızda Hello World demek maksadıyla aşağıdaki kod içeriğini yazdığımızı düşünelim. Table Record olarak AutoMobile isimli bir sınıftan yararlanıyor olacağız.

[![sts_3](/assets/images/2013/sts_3_thumb.png)](/assets/images/2013/sts_3.png)

Veri Ekleme Operasyonu

ilk olarak veri ekleme işini sembolize edelim.

```csharp
using STSdb.Data; 
using System; 
using System.Linq;

namespace HelloSTSdb 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            Add(); 
        }

        private static void Add() 
        { 
            var index1 = new XKey<byte[], string>(Guid.NewGuid().ToByteArray(), "Kadillak STS"); 
            var index2 = new XKey<byte[], string>(Guid.NewGuid().ToByteArray(), "Lamborjini Diyablo"); 
            var index3 = new XKey<byte[], string>(Guid.NewGuid().ToByteArray(), "Ferrarriim"); 
            var index4 = new XKey<byte[], string>(Guid.NewGuid().ToByteArray(), "Doğanım");

            #region Örnek veri kümesi oluşturulması

            using (StorageEngine sEngine = StorageEngine.FromFile("gm.stsdb")) 
           { 
               var table = sEngine.Scheme 
                    .CreateOrOpenXTable<XKey<byte[], string>, AutoMobile>(new Locator("Automobile")); 
                table.KeyMap = new XKeyMap<byte[], string>(null, 16, null, 512);

                sEngine.Scheme.Commit();

                table[index1] = new AutoMobile 
               { 
                    Manufacturer = "Ceneral Motor Kampani", 
                    Model = "STS Turbo CRDI AK/S", 
                    Price = 90000, 
                    ProductionDate = "2005" 
                }; 
                table[index2] = new AutoMobile 
                { 
                    Manufacturer = "İtalyan cob", 
                    Model = "Diablo III", 
                    Price = 250000, 
                    ProductionDate = "2008" 
                }; 
                table[index3] = new AutoMobile 
                { 
                    Manufacturer = "Ferrari", 
                    Model = "799", 
                    Price = 350000, 
                    ProductionDate = "2013" 
                }; 
                table[index4] = new AutoMobile 
                { 
                    Manufacturer = "Türk işi", 
                    Model = "Doğan SLX/AK 8 ileri", 
                    Price = 450, 
                    ProductionDate = "2014" 
                };

                table.Commit(); 
                table.Close(); 
            }

            #endregion 
        }

    }

    public class AutoMobile 
    { 
        public string Model { get; set; } 
        public string Manufacturer { get; set; } 
        public string ProductionDate { get; set; } 
        public decimal Price { get; set; }

        public override string ToString() 
        { 
            return string.Format("{0} {1}({2}) by {3}", ProductionDate, Model, Price, Manufacturer); 
        } 
    } 
}
```

Dikkat edileceği üzere StorageEngine ana nesnedir. using bloğu ile kullanılabilen, bir başka deyişle Dispose edilebilir olan bu nesne örneğinin FromFile metodundan yararlanarak depolama işlemini yapan dosya belirtilmektedir. Key değerleri için XKey tipinden nesneler örneklenmiştir. Veriyi tutacak olan ise XTable tipine ait nesne örnekleridir. Bu tanımlama için StorageEngine nesne örneği üzerinden hareket edilir ve CreateOrOpenXTable metoduna başvurulur. Bir başka deyişle bir şema tanımı yaptığımızı ifade edebiliriz.

Dikkat edilmesi gereken noktalardan birisi de Key olarak Primitive tip kullanmadığımız için KeyMap özelliğine bir değer atama zorunluluğumuz olmasıdır. Bu atama sırasında, XKey içerisindeki tip yapısına göre maksimum boyutlar belirtilmek zorundadır. Aksi durumda çalışma zamanında null referans nedeniyle bir istisna (Exception) alınacaktır.

Veri ekleme işlemi aslında son derece basittir. XTable tipinin indeksleyici operatörüne tanımlanan Key örnekleri atanır. Eşitlikten sonra ise yine bildirimi yapılmış olan tipten (ki örneğimizde AutoMobile sınıfına ait nesnelerdir) örnekler atanır. Tüm işlemlerin tamamlanmasının ardından bir Close ve Commit çağrısının yapılması, verilerin kalıcı olarak yazılması açısından kritiktir. Console uygulamasını bu şekilde çalıştırdığımızda dosya sistemi üzerinde gm.stsdb isimli bir dosya oluşturulduğu gözlemlenir.

[![sts_4](/assets/images/2013/sts_4_thumb.png)](/assets/images/2013/sts_4.png)

Veri Okuma

Gelelim veri okuma işine. Örneğin az önce eklemiş olduğumuz XTable içeriğini ekrana yazdırmayı deneyelim. Bu amaçla aşağıdaki Read metodunu ele alabiliriz.

```csharp
private static void Read() 
{ 
    #region Veri okunması

    using (StorageEngine sEngine = StorageEngine.FromFile("gm.stsdb")) 
   { 
        var table = sEngine.Scheme 
           .OpenXTable<XKey<byte[], string>, AutoMobile>(new Locator("Automobile"));

        foreach (var row in table.Where(r => r.Record.Price > 100000)) 
        { 
            Console.WriteLine("{0}\n{1}\n" 
                , row.Key.ToString() 
                , row.Record.ToString() 
                ); 
        }

        Console.WriteLine("\nTüm Kayıtlar\n"); 
        foreach (var row in table) 
        { 
            Console.WriteLine("{0}-{1}\n{2}\n" 
                , (new Guid(row.Key.SubKey0)).ToString() 
                , row.Key.SubKey1 
                , row.Record.ToString()                 
                ); 
        }

        table.Close(); 
    }

    #endregion Veri okunması 
}
```

Okuma işleminde başrol oyuncusu olarak yine, StorageEngine sınıfı devreye girmektedir. FromFile metodu ile sts veritabanı dosyası yüklendikten sonra ise, veri okuması yapılmak istenen XTable’ ın açılması gerekmektedir. Bu işlem için OpenXTable metodundan yararlanılmaktadır. Generic parametreye (, AutoMobile>) dikkat edileceği üzere, oluşturulan XTable’ ın şema yapısına uygun olacak şekilde verildiği görülmektedir.

table nesne örneği elde edildikten sonra basit bir foreach iterasyonu ile kayıtlar arasından dolaşılabilinir. Hatta örnekte görüldüğü gibi Where genişletme metodundan yararlanılarak bir LINQ sorgusunun icra edilmesi de sağlanabilir. lambda (=>) operatörü etrafında kullanılan r değişkeni, AutoMobile sınıfından bir nesne örneğidir. Dolayısıyla özellikleri sorgulamada filtre kriteri olarak kullanılabilir. Uygulamanın çalışma zamanı çıktısı aşağıdaki gibi olacaktır.

[![sts_5](/assets/images/2013/sts_5_thumb_1.png)](/assets/images/2013/sts_5_1.png)

Temel olarak key-value teorisini baz alarak çalışan STSdb sisteminde, tablo anahtar (Table Key) ve kayıtlarının (Table Record) hangi türlerden oluşabileceği belirlidir. Burada oldukça geniş bir nesne yelpazesinin olduğunu ifade edebiliriz.

Tablo anahtarlarına (Table Keys) baktığımızda aşağıdaki tiplerin desteklendiğini görmekteyiz.

- .Net primitive types (Boolean, Byte, Char, DateTime, Decimal, Double, Int16, Int32, Int64, TimeSpan, SByte, Single, String, UInt16, UInt32, Uınt64)
- byte[] dizisi (Örneğin bir resmin byte içeriğini Key olarak kullanabilirsiniz ![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_174.png))
- STSdb’ ye özgün Locator tipi
- Tn primitive olmak suretiyle XKey,
- IKeyMap arayüzü (Interface) türetmeleri

Tablo kayıtlarına (Table Records) baktığımızda ise benzer tiplerin desteklendiğini görürüz.

- .Net primitive types (Boolean, Byte, Char, DateTime, Decimal, Double, Int16, Int32, Int64, TimeSpan, SByte, Single, String, UInt16, UInt32, Uınt64)
- byte[] dizisi, enum, Type ![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_174.png), Image, Icon, MemoryStream, Guid, XElement gibi diğer tipler
- STSdb’ ye özgü Locator ve BlobStream (Çok büyük boyutlu binary içerikleri alabilirsiniz demek)
- Serileştirilebilir nesneler (Serializable Objects)
- IBinaryPersist> arayüzünün türevleri (Özellikle kendi Persistence mekanizmamızı yazmak istediğimiz durumlarda yine bu arayüzden türetilmiş tipler söz konusu olacaktır. Bir başka deyişle Binary olarak yazma ve okuma işlemlerine müdahale edip Persistence şeklini değiştirebiliriz)

Görüldüğü üzere özellikle Key değerlerini karmaşık tipler şeklinde tutmak mümkün. Burada nesne yönelimli dünyanın (Object Oriented World) faydalarını da görüyoruz. Interface türetmeli tiplerin Key veya Record olarak kullanılabilmesi bunun en güzel örneği belki de.

STSdb’ yi göz önüne alırken.Net Framework platformu için garanti, embedded bir NoSQL çözümü olduğunu ve özellikle Cross-Platform alanında Mono’ ya bağımlı olduğunu unutmamalıyız. Bu bir kısıtlama gibi görünse de, ürünlerini.Net ortamında geliştirenler için çok da büyük bir sorun değil. Ayrıca sunduğu üst limitsiz depolama alanı, Transaction, Snapshot, sıkıştırma kabiliyetleri vb diğer imkanlar nedeniyle, özellikle büyük boyutlu veri kümeleri için ideal görünüyor. STS firmasının bir ürünü olan bu veritabanı, şirketin uzun yıllardır FOREX pazarı üzerine yaptığı uygulama geliştirmelerine ait tecrübelerinin bir çıktısı aslında.

Bu yazımızda kısaca STSdb ürününü incelemeye çalıştık. Bir Hello World uygulaması geliştirdik ve veri ekleme ile okuma işlemlerinin nasıl yapılabildiğine baktık. Elbette örnekleri zenginleştirmek ve gerçek saha tecrübesini yapmak sizin elinizde. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[HelloSTSdb.zip (249,52 kb)](/assets/files/2013/HelloSTSdb.zip)

[İlk Yazım Tarihi 2013-01-15]