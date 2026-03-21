---
layout: post
title: "Entity Framework Üzerinde TransactionScope Kullanımı"
date: 2011-04-01 08:52:00 +0300
categories:
  - entity-framework
tags:
  - entity-framework
  - transaction
---
Günlerden Salı, sonbahar. Neredeyse hiç uyumadan geçen bir gecenin ardından sabaha karşı yorgunluktan sızan Netspecter, CAD’ in baş ucunda dakikalarca miyavlaması sonucu ancak kendine gelebilir. Eksi ve köhne divanı, o doğrulurken olabildiğince haykırarak gıcırdamaktadır. Önce terliklerini arar. Oda darma dağınıktır. Tüm gece kütüphanedeki sayısız kitabı indirmiş ve bir sonuç bulmak için saatlerce araştırma yapmıştır. Sonuçta terliğin tekinin dahi bulunamadığı bir kalabalık kitap yığınıdır.

[![blg231_Giris](/assets/images/2011/blg231_Giris_thumb.gif)](/assets/images/2011/blg231_Giris.gif)


Kafada korkunç bir baş ağrısı, dışarıdan gelen metronun raylarda bıraktığı ses ve CAD’ in cılız miyavlamaları…Netspecter divandan kalkarken şöyle bir belinden geriye doğru esner. Derken tavan lambasının biraz üstüne vuran dikdörtgen biçimli gölgeyi fark eder. Nasıl olur? Bu kitap gözünden nasıl kaçmıştır. Gece karanlığında fark edemediği kaynağı gün ışığı açığa çıkarmıştır. İşte oradadır. Gölgenin kaynağına doğru gider. Ado.Net and System.Xml v2.0 The Beta Version

![Open-mouthed smile](/assets/images/2011/wlEmoticon-openmouthedsmile_1.png)

Yaşasın diyerek haykırır.

Yıllar yıllar önce.Net Framework 2.0 ile gelen yenilikleri takip etmeye çalıştığım dönemlerde, Amazon üzerinden getirttiğim kitaplardan birisi de yandaki resimde görülen Ado.Net and System.Xml v2.0 kitabı idi. Şu an halen kitaplığımda durmakta. O sıralar CSharpNedir? bünyesinde Ado.Net bölüm editörlüğü yaptığımdan, bu kitabı tedarik etmiş ve çalışmıştım..Net Framework 2.0 ile gelen yeni Xml alt yapısının beta hali ile alması bir yana Ado.Net’ in yeni 2.0 sürümü için planlanan bazı kabiliyetlerde anlatılmaktaydı ki bunlardan bekli de en önemlisi System.Transaction.dll assembly içerisinde yer alan ve özellikle Distributed Transaction yönetimini daha etkili ve kolay bir şekilde ele almamızı sağlayan TransactionScope tipiydi.

Bilindiği üzere bu tip sayesinde özellikle blok içerisine alınan birden fazla ve farklı bağlantı (Connection) üzerinden gerçekleştirilecek veritabanı işlemlerinin, aynı Transaction alanı içerisinde ele alınması ve toplu olarak Commit/Rollback edilmeleri mümkün olmaktadır. Çok doğal olarak ardışıl olarak gerçekleştirilen ve belirli iş kuralları içerisinde sıralanan bazı veritabanı işlemlerinin, tamamen başarılı oldukları takdirde onaylanmasının istendiği durumlar son derece yaygındır.(Lütfen Transaction kavramı ve ACID ilkelerini hatırlayınız)

Bu durum Entity Framework içinde geçerlidir. Çok doğal olarak basit bir Entity nesne örneğinin veri içeriğinin veritabanına eklenmesi işleminden tutunda da, farklı Context örnekleri içerisinde gerçekleştirilen veri işlemlerinin de bir Transaction alanı içerisinde gerçekeştirilmesi istenebilir

![Sarcastic smile](/assets/images/2011/wlEmoticon-sarcasticsmile_1.png)

İşte bu yazımızda Transaction yapısının Entity Framework tarafındaki kullanımını incelemeye çalışıyor olacağız. İşe ilk olarak aşağıdaki basit kod parçası ile başlayabiliriz.

```csharp
namespace EFTransactionManagement 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            using (ChinookEntities entities = new ChinookEntities()) 
            {                
                Artist newArtist = new Artist 
                { 
                     Name="Sertap Erener" 
                }; 
                entities.AddToArtists(newArtist);

                Album newAlbum = new Album() 
                { 
                     Title="Rengarenk"                     
                }; 
                newArtist.Albums.Add(newAlbum); 
                entities.AddToAlbums(newAlbum);

                entities.SaveChanges();                
            } 
        } 
    } 
}
```

Chinook model veritabanını kullanan bu örnekte Context nesnesine ait SaveChanges metodunun çağırılmasından önce iki Insert işlemi gerçekleştirildiği görülmektedir. Öncelikli olarak Artist tipinden bir örnek oluşturulmuş ve Context tipine ait koleksiyona eklenmiştir. Hemen arından da bir Album örneği oluşturulmuş ve yine ilgili koleksiyona ilave edilmiştir. Tabi bu örnekte Album ile Artist örnekleri arasında bir ilişki de söz konusudur. Yani üretilen yeni Album nesne örneği, üretilen yeni Artist örneğinin Albums özelliği ile işaret edilen koleksiyonuna da eklenmiştir. Eğer SaveChanges metodunun çağırılması sonrasındaki SQL Server Profiler görüntüsüne bakarsak aşağıdaki çıktı ile karşılaştığımızı görebiliriz.

[![blg231_ImplicitlyTransaction](/assets/images/2011/blg231_ImplicitlyTransaction_thumb.gif)](/assets/images/2011/blg231_ImplicitlyTransaction.gif)

Dikkat edileceği üzere iki insert işleminden önce bir Transaction başlatılmış ve sonrasında ise Commit işlemi ile bu girişler onaylanmıştır. Bir başka deyişle Entity Framework tarafında, SaveChanges metodunun çağırılması sonrasında bilinçsiz olarak (Implicitly) bir Transaction oluşturulduğu görülmektedir. Elbette bu davranış şekli değiştirilebilir. Bir başka deyişle developer bazlı bir Transaction kullanımı da söz konusu olabilir. Bunun için ilk akla gelen bilinçli olarak TransactionScope örneğinin kullanılması ve SaveChanges çağrısının burada kullanılmasıdır. Şimdi örnek uygulamamıza System.Transaction.dll assembly’ ını referans ederek vakamızı incelemeye devam edelim.

[![blg231_TransactionReference](/assets/images/2011/blg231_TransactionReference_thumb.gif)](/assets/images/2011/blg231_TransactionReference.gif)

Örnek kod içeriğini ise aşağıdaki gibi geliştirebiliriz.

```csharp
using System; 
using System.Transactions;

namespace EFTransactionManagement 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            bool acceptStatus = false;

            using (ChinookEntities entities = new ChinookEntities()) 
            {                 
                    Artist newArtist = new Artist 
                    { 
                        Name = "Sertap Erener" 
                    }; 
                    entities.AddToArtists(newArtist);

                    Album newAlbum = new Album() 
                    { 
                        Title = "Rengarenk" 
                    }; 
                    newArtist.Albums.Add(newAlbum); 
                    entities.AddToAlbums(newAlbum);

                using (TransactionScope scope = new TransactionScope()) 
                { 
                    try 
                    { 
                        entities.SaveChanges(); 
                        scope.Complete(); 
                        acceptStatus = true; 
                    } 
                    catch //Exception kontrolü yapmamızda yarar var 
                    { 
                        Console.WriteLine("Sorun var!"); 
                    }

                    if (acceptStatus) 
                        entities.AcceptAllChanges(); 
                } 
            } 
        } 
    } 
}
```

Dikkat edileceği üzere Album ve Artist nesne örneklerinin giriş işlemlerinin onaylandığı SaveChanges metod çağrısı bir try…catch bloğu içerisinde kontrol altına alınmıştır. Dahası söz konusu try…catch bloğu da TransactionScope bloğu içerisinde konuşlandırılmıştır. Eğer kod try bloğunun son satırına kadar başarılı bir şekilde gelebilirse TransactionScope nesne örneğine ait Complete metodunun çağırılması ile, yapılan tüm işlemlerin onaylanması sağlanmaktadır. (Elbette catch bloğuna girilmesi halinde ilgili Transaction içeriğinin Rollback edilmesi süreci de otomatik olarak işletilecektir) Burada bilinçli olarak bir transaction bloğu açılması söz konusudur. Uygulama kodunu çalıştırdığımızda ve SQL Server Profiler aracının çıktısına baktığımızda aşağıdaki ekran görüntüsünde yer alan sonuçların üretildiği görülebilir.

[![blg231_ExplicitlyTransaction](/assets/images/2011/blg231_ExplicitlyTransaction_thumb.gif)](/assets/images/2011/blg231_ExplicitlyTransaction.gif)

Dikkat edileceği üzere ilk örneğimizde olduğu gibi bir transaction (Begin Transaction) başlatılmış ve insert işlemleri sonrasında commit (Commint Transaction) edilmiştir.

Peki ya İki farklı Context nesnesi üzeriden bilinçli TransactionScope kullanımı söz konusu olabilir mi?

![Winking smile](/assets/images/2011/wlEmoticon-winkingsmile_1.png)

Çok doğal olarak uygulama içerisinde farklı veri kaynaklarını temsil eden farklı Context nesneleri kullanılıyor olabilir. Buna göre ilgili Context nesneleri arasında akan bazı iş fonksiyonelliklerinin transaction kullanımını gerektirmesi de düşünülebilir. Dilerseniz bu durumu incelemek için senaryomuza aşağıdaki Product isimli tablo içeriğine sahip AzonStore isimli bir veritabanını daha ekleyelim. AzonStore tahmin edeceğiniz üzere buradaki TransactionScope kullanımını ele almamız için düşünülmüş kobay veritabanıdır ve her hangibir özel tarafı yoktur

![Winking smile](/assets/images/2011/wlEmoticon-winkingsmile_1.png)

[![blg231_AzonStoreProductTable](/assets/images/2011/blg231_AzonStoreProductTable_thumb.gif)](/assets/images/2011/blg231_AzonStoreProductTable.gif)

Tabi bu durumda uygulamamızda iki adet Entity Data Model olması gerektiğini de ifade etmeliyiz.

[![blg231_DoubleEDM](/assets/images/2011/blg231_DoubleEDM_thumb.gif)](/assets/images/2011/blg231_DoubleEDM.gif)

Örnek kod parçamızı da aşağıdaki gibi geliştirdiğimizi düşünelim.

```csharp
using System; 
using System.Transactions;

namespace EFTransactionManagement 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            bool acceptStatus = false; 
            ChinookEntities chinookEntities = null; 
            AzonStoreEntities azonEntities = null;

            using (chinookEntities = new ChinookEntities()) 
            { 
                Artist newArtist = new Artist 
                { 
                    Name = "Sertap Erener" 
                }; 
                chinookEntities.AddToArtists(newArtist);

                Album newAlbum = new Album() 
                { 
                    Title = "Rengarenk" 
                }; 
                newArtist.Albums.Add(newAlbum); 
                chinookEntities.AddToAlbums(newAlbum);

                azonEntities = new AzonStoreEntities(); 
                Product newProduct = new Product 
                { 
                    OrgId = newAlbum.AlbumId, 
                    Name = newAlbum.Title, 
                    ListPrice = 10 
                }; 
                azonEntities.AddToProducts(newProduct);

                using (TransactionScope scope = new TransactionScope()) 
                { 
                    try 
                    { 
                        chinookEntities.SaveChanges(); 
                        azonEntities.SaveChanges();

                        scope.Complete(); 
                        acceptStatus = true; 
                    } 
                    catch (Exception excp) 
                    { 
                        Console.WriteLine(excp.Message); 
                    } 
                } 
                if (acceptStatus) 
                { 
                    chinookEntities.AcceptAllChanges(); 
                    azonEntities.AcceptAllChanges(); 
                } 
            } 
        } 
    } 
}
```

Bu kez işin içerisine ikinci bir Context örneği daha girmektedir. Chinook üzerinde yapılan Album ekleme işleminden sonra bu albüme ait bilgilerden bazıları AzonStore Context’ I içerisindeki Product nesnesine de eklenmektedir. Dolayısıyla iki farklı veritabanı üzerinde gerçekleşecek bir işlem söz konusudur. Burada söz konusu Context’ lerin işaret ettiği/kullandığı veritabanları farklı sunucular üzerinde de olabilir ki bu durumda TransactionScope, Distirbuted Transaction Coordinator (DTC) aracını otomatik olarak devreye alarak bir dağıtık transaction alanı başlatacaktır. Dilerseniz bir de SQL Server Profiler aracının ürettiği çıktıya bakalım.

[![blg231_MultiContextTrx](/assets/images/2011/blg231_MultiContextTrx_thumb.gif)](/assets/images/2011/blg231_MultiContextTrx.gif)

Görüldüğü üzere Chinook ve AzonStore veritabanları üzerindeki tüm Insert işlemleri aynı Transaction alanı içerisine dahil edilmişlerdir (Begin Transaction <–> Commit Transaction)

Peki son senaryodaki Context nesne örnekleri üzerinden gerçekleştirilen işlemleri bir TransactionScope bloğu içerisine ele almasaydık? ![Sarcastic smile](/assets/images/2011/wlEmoticon-sarcasticsmile_1.png)

Nitekim bu son derece doğal bir davranış biçimi olacaktır. Özellike Entity Framework’ ü ilk kez kullanmaya başlayan bir geliştirici için. Bu durumu analiz etmek için kodu aşağıdaki gibi yenilediğimizi düşünebiliriz.

```csharp
using System;

namespace EFTransactionManagement 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        {            
            ChinookEntities chinookEntities = null; 
            AzonStoreEntities azonEntities = null;

            using (chinookEntities = new ChinookEntities()) 
            { 
                Artist newArtist = new Artist 
                { 
                    Name = "Sertap Erener" 
                }; 
                chinookEntities.AddToArtists(newArtist);

                Album newAlbum = new Album() 
                { 
                    Title = "Rengarenk" 
                }; 
                newArtist.Albums.Add(newAlbum); 
                chinookEntities.AddToAlbums(newAlbum);

                azonEntities = new AzonStoreEntities(); 
                Product newProduct = new Product 
                { 
                    OrgId = newAlbum.AlbumId, 
                    Name = newAlbum.Title, 
                    ListPrice = 10 
                }; 
                azonEntities.AddToProducts(newProduct);

                chinookEntities.SaveChanges(); 
                azonEntities.SaveChanges(); 
            } 
        } 
    } 
}
```

Bu kez sadece ilgili Context nesne örneklerine ait SaveChanges metodlarının ardışıl olarak çağırılması söz konusudur ki bunun SQL Server Profiler aracına baktığımızda üreteceği sonuç aşağıdaki ekran görüntüsünde olduğu gibidir.

[![blg231_NoneTransactionScope](/assets/images/2011/blg231_NoneTransactionScope_thumb.gif)](/assets/images/2011/blg231_NoneTransactionScope.gif)

Görüldüğü üzere her SaveChanges çağrısı için ayrı bir Transaction işlemi başlatılmıştır. Burada AzonStore için kullanılan Context örneğinin, Chinook için üretilen Context nesnesine ait using bloğu içerisinde olmasının dahi bir önemi yoktur. Olay sadece SaveChanges metod çağrılarına aittir.

Entity Framework tarafında bu yazıda ele aldığımız gibi basit veritabanı modellerinin söz konusu olduğu durumlarda Transaction kullanımına dikkat edilmemesi söz konusu olabilir. Hatta gerekmeyebilir. Nitekim Transaction oluşturmanın da veritabanı kaynağı üzerinde bir maliyeti vardır. Ancak Enterprise seviyedeki uygulamalar ve kullandıkları veritabanı sistemleri ile karmaşık ve bütünlüğü önem arz eden iş kuralları söz konusu olduğunda, TransactionScope kullanımına ciddi anlamda dikkat edilmesi gerekmektedir. Tabi bilindiği üzere TransactionScope nesne örneği üzerinden yapılabilecek farklı ayarlamalar da söz konusudur. Şu anki örneklerimizde bu tipin son derece yalın ve sade kullanımı değerlendirilmiştir. Size tavsiyem söz konusu örneği farklı sunucular veya SQL Instance’ ları üzerindeki n sayıda veritabanında ele alacak şekilde değiştirmeniz ve gözlemlemenizdir.

![Be right back](/assets/images/2011/wlEmoticon-berightback.png)

Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.