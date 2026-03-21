---
layout: post
title: "NoSQL Maceraları - Apache Cassandra ve .Net"
date: 2012-12-16 19:35:00 +0300
categories:
  - nosql
tags:
  - nosql
  - apache-cassandra
  - .net
  - nuget
  - not-only-sql
---
Size bu günkü makale konumuzun yanda fotoğrafı görülen model (youtube modeli de diyebiliriz) Cassandra Bankson ile alakalı olduğunu söylemek isterdim ama yakınlarından bile geçmeyeceğiz. (Zaten araştırırsanız aslında makyaj bidonu ile bu hale geldiğini keşfedeceksiniz) Başlıktan da anlayacağınız üzere bu günkü yazımızın konusu.Net plaftormunda Apache Cassandra’ yı kullanmak.

[![cassandra](/assets/images/2012/cassandra_thumb.jpg)](/assets/images/2012/cassandra.jpg)


Uzun zamandır gündemimde olan konulardan birisi de NoSQL veritabanı sistemleri. Internet şirketlerinin pek çoğu (Facebook, Twitter, Youtube, Netflix vb) NotOnly SQL veritabanlarını kullanmakta ve hatta bir kısmının kendi geliştirdikleri NoSQL sistemleri bile var. Amazon, Google bu noktada öncüler diyebiliriz.

NoSQL veritabanlarının popüler olmalarının elbette bazı sebepleri var. Özellikle RDMS'lerin tipik özelliklerine ters gelen kabiliyetleri nedeni ile büyük veriler ile çalışılmasında, ölçeklemelerde, performans da öne çıkabilmektedirler. Tabi bu avantajlar, NoSQL yapısına uygun veri kümeleri için söz konusudur. Her veri yapısı veya modeli için NoSQL sistemleri uygun olmayabilir. (Yani RDMS'i terk eden bir dünyadan bahsedemeyiz ![Smile](/assets/images/2012/wlEmoticon-smile_61.png))

Ancak NoSQL sistemler çeşitlilikleri açısından da RDMS’ lere göre farklılaşmakta ve bu yüzden daha fazla tercih edilir olmaktadır. Bu çeşitler kısaca şunlardır;

- Wide Column Store / Column Families (Cassandra bu kategoride gösterilmiştir)
- Document Store
- Key Value / Tuple Store
- Graph Databases
- Multimodel Databases
- Object Databases
- Grid & Cloud Databases
- XML Databases
- Multidimensional Databases
- Multivalue Database

Aslında ortalıkta pek çok NoSQL sistemi var. Hatta çoooook uzun zaman önce Berkley üniversitesi tarafından üretilen BerkleyDB bu işin atasıdır diyebiliriz. Günümüzde ise MongoDb, Neo4j, db4o, bigtable, hadoop, Redis, MemcachedDB vb ürünler bulunmakta. [(Tam ve güncel bir listeye bu adresten ulaşabilirsiniz)](http://nosql-database.org/) NoSQL sistemlerinin popüler olanlarından biriside ismini Yunan Mitolojisinden aldığını düşündüğüm Apache'nin Cassandra isimli ürünüdür. Bu yazımızda Cassandra’ yı.Net platformunda nasıl kullanabileceğimizi incelemeye çalışıyor olacağız.

> Apache Cassandra'yı [http://cassandra.apache.org/download/](http://cassandra.apache.org/download/) adresinden indirebilirsiniz. Cassandra yı indirdiğinizde büyük ihtimalle tar.gz uzantılı bir dosya gelecektir. Dolayısıyla Windows tabanlı bir sistemde bunu açacak uygulamaya ihtiyacınız var. 7Zip bu konuda bana yardımcı oldu diyebilirim.
> Cassandra'yı bir klasöre açtıktan sonra sisteme bazı çevresel değişkenlerin de ilave edilmesi gerekmektedir (Environment Variables). Bunlardan birisi JAVA_HOME dur ve sistem de kurulmuş olan Java klasörünü göstermektedir. Diğeri ise CASSANDRA_HOME olarak adlandırılmaktadır ve Cassandra'nın açıldığı klasörü işaret etmektedir. Kendi sistemimde bu değerleri şu şekilde ayarladım.
> [![uc_1](/assets/images/2012/uc_1_thumb.png)](/assets/images/2012/uc_1.png)
> [![uc_2](/assets/images/2012/uc_2_thumb.png)](/assets/images/2012/uc_2.png)

Kurulum işlemi için gerekli ayarları yaptıktan sonra bin klasörü altında yer alan cassandra.bat dosyasını çalıştırabiliriz. Bu batch dosyası sunucuyu etkinleştirecektir. Cassandra verilerin loglarını tutmak için root klasördeki var isimli alt klasörü kullanır (Bu kuruluma göre d:\var altındadır ama istendiği takdirde conf klasörü içindeki cassandra.yaml'den konum değiştirilebilir)

## Genel Özellikler

Cassandra'nın genel özelliklerine baktığımızda aşağıdakileri ifade edebiliriz.

- Java ile geliştirilmiş bir veritabanı sistemidir ve bu nedenle kurulduğu makinede Java ortamının var olması gerekmektedir.
- Bir NoSQL (Not Only SQL) sistemidir. Dolayısıyla SQL, Oracle gibi ilişkisel bir veritabanı modeli değildir.
- Açık kaynaktır (Open Source)
- Bir Oracle veya SQL Server olmasa da Youtube, Netflix gibi pek çok dünya markası tarafından çeşitli ürünlerde kullanılmaktadır.
- Kolayca ölçeklenebilir (Scalable)
- Hata tolerans yönetimi mevcuttur.
- Sütun odaklı (Wide Column Store/Column Families) çalışan bir NoSQL tipidir.
- Distribution tasarım modeli Amazon'un Dynamo ürünü esaslıdır.
- Dağıtık modele destek verdiğinden veriyi n sayıda makine üzerinde genişletmek mümkündür. Bu anlamda RDMS (Relational Database Managament Systems) lerin tam aksine Ring düzenine göre çalışır. Bir başka deyişle dikey (Vertical) değil yatay (Horizontal) olarak ölçeklenir.
- Terabyte'larca veriyi tutabilir ![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_138.png)

Veri modeline üstten bakıldığında yapısı RDMS şemalarına benzetilebilir. Sütunlar ve isimlendirilmiş değerler (Named Values) söz konusudur. Ancak pratikte hiç de böyle değildir. Cassandra bir şema yapısı kullanmamaktadır. Ya bizim aşina olduğumuz tipte bir şema kullanmamaktadır diyebiliriz. Veriyi sütunlar topluluğu halinde tutar. Aslında bu konuda çok fazla detaya girmeyeceğiz.

## İlk Çalışma

Cassandra'yı kurup çalıştırdıktan sonra (cassandra.bat ile yapıyoruz) komut satırından hemen veri girişi işlemleri yaptırılabilir. Bunun için yine bin dizinindeki cassandracli.bat dosyasının çalıştırılması yeterlidir. Bu bir komut satırı istemcisidir ve cassandra sunucusuna bağlanacaktır. Aşağıdaki ekran görüntüsünde örnek bir kullanıma yer verilmiştir.

[![uc_3](/assets/images/2012/uc_3_thumb.png)](/assets/images/2012/uc_3.png)

Buradaki komutlardan da anlaşılacağı üzere veritabanı aslında bir keyspace'dir. Bu keyspace içerisinde bir tablo oluşturmak aslında bir Column Family yaratmak anlamına gelmektedir. Column Family içerisine set edilen Row Key’ ler aslında bildiğimiz tablo satırlarına benzetilebilir. Row Key’ ler içerisinde ise key-value şeklinde kolonlar ve verileri bulunmaktadır. Her key-value column aslında bir Row Key ile ilintilidir.

Dikkat edileceği gibi kolonlarda key-value şeklinde bir tutuluş söz konusudur. Ortam tamamen case sensitive'dir ve ifadelerin çalışması için; ile bitirilmesi gerekmektedir. Örnekte kullandığımız komutlar ise aşağıdaki gibidir.

## İlk Komutlar

- create keyspace ile BigFootMotorCompany isimli bir key space üretilmiştir.
- create column family ile Car isimli bir Row Key üretilmiştir.
- set ile Car içerisinde sütun (Column) ve key-value eklenmiştir.
- get ile bir ModelX isimli column verisi çekilmiştir.

Yapılan bu üretim sonrasında klasör yapısıda aşağıdaki şekilde görüldüğü gibi oluşacaktır.

[![uc_4](/assets/images/2012/uc_4_thumb.png)](/assets/images/2012/uc_4.png)

Veri binary formatta tutulmaktadır.

## Gelelim.Net Framework tarafına

.Net tarafında Cassandra ile çalışabilmek için NuGet paketlerinden birisi olan FluentCassandra ile çalışabiliriz. İlk olarak basit bir Console uygulaması açalım ve NuGet paket yöneticisini kullanarak internetten Fluent Cassandra paketini indirelim (Dilerseniz komut satırından da install edebilirsiniz. Install-Package FluentCassandra ifadesini çalıştırmanız yeterli olacaktır)

[![uc_5](/assets/images/2012/uc_5_thumb.png)](/assets/images/2012/uc_5.png)

## İlk Kodlar

İşte ilk deneme kodlarımız.

```csharp
using FluentCassandra; 
using FluentCassandra.Connections; 
using System;

namespace HowTo_HelloApacheCassandra 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            // Sunucu ile bağlant kuralım. Varsayılan olarak localhost ismiyle bağlanabiliriz. 
            Server cassandraServer = new Server("localhost");

            // Komut satırından ürettiğimiz BigFootMotorCompany isimli key space ile çalışacağız 
           using (var database = new CassandraContext(keyspace: "BiGFootMotorCompany", server: cassandraServer)) 
            { 
                // sunucuya ve veritabanına bağlanıp bağlanamadığımız test etmek için bir kaç bilgi talep edelim. 
                Console.WriteLine("Server port : {0}, database verison : {1}" 
                    ,cassandraServer.Port 
                    , database.DescribeVersion() 
                    );

                #region Yeni bir Column Family oluşturmak

                CassandraColumnFamily newFamily = database.GetColumnFamily("ModelDesigner");

                // Cassandra Query Language kullanarak bir Column Family oluşturuyor ve içerisine bir kaç column ilave ediyoruz 
                // Sonraki denemelerde bu satırda hata vermemesi için bir null kontrolü yapmak da işe yarayabilir 
                if(newFamily==null) 
                    database.ExecuteQuery(@" 
                    create columnfamily 
                    ModelDesigner( 
                        ModelDesignerId ascii Primary Key 
                        ,Title text 
                       ,Nickname text 
                        ,Level int 
                        ,Outsource boolean);" 
                    );

                // Yeni bir satır oluşturalım 
                // dynamic tipin çalışma zamanında çözümlenmesine neden olacaktır. 
                dynamic burkinyus=newFamily.CreateRecord("burkinyus"); 
                // Key' lerin değerlerini atayalım 
                burkinyus.Title = "Mr."; 
                burkinyus.Nickname = "burkinyus"; 
                burkinyus.Level = 100; 
                burkinyus.Outsource = false;

                // Yeni oluşturlan satırları Context' e ekleyelim 
                database.Attach(burkinyus);

                // bir satır daha oluşturalım ve alanlarını set edelim 
                dynamic oktavyus = newFamily.CreateRecord("oktavyus"); 
                oktavyus.Title = "Mr."; 
                oktavyus.Nickname = "oktavyus"; 
                oktavyus.Level = 400; 
                oktavyus.Outsource = true; 
                
                database.Attach(oktavyus);

                // Değişiklikleri kayıt edelim 
                database.SaveChanges();

                // şimdi de verileri çekip gösterelim 
                var designers = database.ExecuteQuery("select * from ModelDesigner");

                foreach (dynamic designer in designers) 
                { 
                    Console.WriteLine("{0} {1}({2}) {3}" 
                        ,designer.Title 
                        ,designer.Nickname 
                        ,designer.Level 
                        ,designer.Outsource==true?"Outsource":"_" 
                        ); 
                } 
                #endregion 
            }            
        } 
    } 
}
```

ve çalışma zamanı çıktısı.

[![uc_6](/assets/images/2012/uc_6_thumb.png)](/assets/images/2012/uc_6.png)

> Uygulamanın başarılı bir şekilde çalışabilmesi için tahmin edeceğiniz üzere Cassandra sunucusunun da açık olması gerekir. Aksi durumda çalışma zamanında aşağıdakine benzer bir istisna (Exception) fırlatılacaktır.
> [![uc_7](/assets/images/2012/uc_7_thumb.png)](/assets/images/2012/uc_7.png)

Bu ilk örnekte BiGFootCompany key space’ ine bağlanıp ModelDesigner isimli yeni bir Column Family oluşturmaktayız. Bu Column Family içerisinde de, Title, Nickname, Level ve Outsource gibi kolonlar bulunmakta. Örnek olarak iki adet satır oluşturulmakta ve veritabanına ilave edilerek bir sonuç listesinin ekrana bastırılması sağlanmaktadır. Dikkat edileceği üzere SQL ifadelerine çok benzeyen bazı sorgular da kullandık. Bunlar CQL (Cassandra Query Language) olarak adlandırılmaktadır. Ayrıca işlerimizi biraz daha kolaylaştırmak adına dynamic anahtar kelimesinden faydalanmaya çalıştık. Bu sayede Row Key’ lerin kolonlarına, birer özellikmiş gibi erişebilmemiz mümkün oldu. Böylece geldik bir makalemizin daha sonuna. Bir sonraki yazımızda görüşünceye dek hepinize mutlu günler dilerim

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_138.png)

[HowTo_HelloApacheCassandra.zip (457,46 kb)](/assets/files/2012/HowTo_HelloApacheCassandra.zip)