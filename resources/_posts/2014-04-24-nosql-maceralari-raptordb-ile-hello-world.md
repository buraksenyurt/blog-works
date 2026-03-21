---
layout: post
title: "NoSQL Maceraları - RaptorDB ile Hello World"
date: 2014-04-24 14:55:00 +0300
categories:
  - nosql
tags:
  - nosql
---
Yıllardır vaz geçemediğim bilgisayar oyunlarından birisidir [Command and Conquer Generals](http://tr.wikipedia.org/wiki/Command_%26_Conquer:_Generals). Özellikle Zero Hour setini çok ama çok severim. Bu set içerisinde en çok kullandığım GLA güçleridir ama zaman zaman China Tank General veya USA Air Force’ u da tercih ederim.

[![070401-F-6701P-005](/assets/images/2014/F-22_Raptor_-_070401-F-6701P-046_thumb.jpg)](/assets/images/2014/F-22_Raptor_-_070401-F-6701P-046.jpg)


USA Air Force’ un en belirgin özelliği King Raptor olarak adlandırılan ve aslında günümüzde de [F22 Raptor olarak bilinen savaş uçaklarını](http://tr.wikipedia.org/wiki/F-22_Raptor) içermesidir. Bu uçaklar kendilerine fırlatılan füzeleri lazer ile vurabilir, ayrıca normal Raptor’ lara göre daha fazla güdümlü füze taşıyabilir.

Araştırmasam da, Raptor isminin Dinazor çağındaki [Velociraptor](http://tr.wikipedia.org/wiki/Velociraptor) isimli yırtıcıdan geldiğini düşünmekteyim. Hızlı Hırsız anlamına gelen Velociraptor, 15Kg ağırlığında olan bir etoburdur. Yaklaşık 75 ila 71 milyon yıl önce yaşadığı bilinmektedir. Saatte 40km hızla koşabilen 2metre boyundaki canlı ölümcül bir yırtıcı olarak ifade edilmiştir. O kadar sivri ve güçlü pençelere sahipmiş ki, kurbanının damarlarına denk gelmesi halinde onu 3 ile 5 saniye içerisinde öldürürmüş. Günümüzde bir Hindi boyunda olan bu canlının en korkutucu yanı ise sürü halinde dolaşıyor olmalarıymış. Karada yürüyen Pirhanalara benzetebiliriz sanırım

![Disappointed smile](/assets/images/2014/wlEmoticon-disappointedsmile_4.png)

İsim içerisinde yer alan Raptor pek çok ürüne de esin kaynağı olmuştur. Bir savaş uçağına isim babası olmuştur, Western Digital tarafından bir Hard Disk’ e verilmiştir, Yamaha’ nın bir ATV modelinde kullanılmıştır, NBA takımlarından Toronto’ nun adı olmuştur, hatta Battlestar Galactica’ da bile kullanılmıştır. Popüler bir isim anlayacağınız…

Peki bu ismin bir NoSQL veritabanı sisteminde de ele alındığını biliyor muydunuz?

![Smile](/assets/images/2014/wlEmoticon-smile_89.png)

Tanım

Document Store NoSQL veritabanı tiplerinden birisi olan RaptorDB kuvvetle muhtemel isimlendirilirken tarih öncesi Velociraptor’ dan esinlenilmiştir. Pek çok NoSQL sisteminde olduğu gibi geniş bir kullanım yelpazesi olduğunu ifade edebiliriz. Her ne kadar diğer NoSQL ürünlerinde olduğu gibi geniş bir referans kitlesi göze çarpmasa da, Forumlar, Bloglar, Wiki tarzı siteler, İçerik Yönetim Sistemleri (Content Management Systems) ve Sharepoint benzeri uygulamaların yazılabileceği belirtilmektedir.

Ama tabiki diğer NoSQL sistemlerinde olduğu gibi, Microsoft SQL Server, Oracle vb maliyetli veritabanlarının kullanılmak istenmediği durumlarda da, ideal çözümler arasında yer almaktadır.

> RaptorDB ile ilişkili olarak takip ettiğim referans doküman [Mehdi Gholam’ ın CodeProject’ deki yazısı](http://www.codeproject.com/Articles/375413/RaptorDB-the-Document-Store) oldu. Tabi ürünün [Codeplex üzerinde de yayınlanan](http://raptordb.codeplex.com/) bir içeriği de bulunmakta. Yazı epeyce uzun o yüzden kendime şöyle güzel notlar çıkartıp sizlerle paylaşayım istedim.

RaptorDB projesinin sahibi ve geliştiricisi olan Mehdi Gholam,.Net tabanlı olarak geliştirdiği Library içerisinde pek çok yardımcı açık kaynak koda da başvurmuş. Hatta bunların neredeyse tamamı ile ilgili CodeProject üzerinde kaynağa rastlamak mümkün. Esas itibariyle RaptorDB’ nin yaşam alanına giren kütüphaneleri aşağıdaki grafik ile özetleyebiliriz.

[![raptor_5](/assets/images/2014/raptor_5_thumb_1.png)](/assets/images/2014/raptor_5_1.png)

İçeriğin JSON (JavaScriptObjectNotation) formatında ele alındığını görmekteyiz ama bunun dışında Binary serileştirme noktasında devreye giren yardımcı bir kütüphanede söz konusu. Özellikle verinin sıkıştırılması noktasında b-tree algoritmasını baz alan WAHBitArray’ den yararlanılmış. Full Text Search işlemlerinde h00t isimli ürün devreye girerken Log tutmada ise Enterprise çözümlerde dahi sıklıkla kullandığımız Log4Net’ in daha sade bir sürümü değerlendirilmiş. Pek tabi başrol oyuncusu RaptorDB kütüphanesi.

Genel Özellikleri

RaptorDB’ yi bir ürün olarak göz önüne aldığımızda ise aşağıdaki maddelerde yer alan temel özelliklere sahip olduğunu görmekteyiz.

- Tamamıyla.Net ile geliştirilmiş bir üründür. Hatta Source Code ile birlikte indirildiğinde referans edilen projeler debug edilebilir. Bu sayede iç çalışma yapısını da gayet güzel anlayabiliyorsunuz.
- Dokümanlar ASCII JSON veya Binary serileştirilmiş JSON formatında saklanmaktadır.
- Özellikle Network üzerinde hareket edeceği düşünülen veri için sıkıştırma işlemi söz konusudur (Test etmedim)
- Kritik fonksiyonelliklerden birisi olan ve pek çok NoSQL sisteminde görmeye alıştığımız Transaction desteği ise 1.6 sürümünden itibaren gelmektedir. (Tabi Transaction kullanımında belirtilen handikaplardan birisi de, bir Query’ nin sadece dahil olduğu Transaction içerisine konu olan veri kümelerindeki değişiklikleri fark edebilmesidir)
- String içerikler UTF-8 veya Unicode formatta ele alınmaktadır. Çok doğal olarak string tipteki kolonlar için bir veri boyutu ifade edilmesine gerek yoktur ama veritabanı motoru da bu içerikler için otomatik olarak bir boyut ayarlamasına gitmektedir.
- Ayrıca String alanlar için Full Text Search desteği bulunmaktadır.
- Biraz sonra değineceğimiz üzere View adı verilen yapılar veriyi JSON yerine Binary formatta saklamaktadırlar. Ki Query’ lerin bu View’ lar üzerinden çalıştırıldığı düşünülürse, b-tree’ ye göre arama yapan motorun hızlı sonuç döndüreceği ifade edilebilir.
- View adı verilen ve veritabanındaki tablolara benzetilen (ki ben bilinçaltımın bir yansıması olarak SQL’ daki View’ lar ile özdeşleştiriyorum) içeriklerden Primary olarak kullanılan eğer bir Transaction ile ilişkilendirilirse, bu View ve ilişkili olan View’ lar üzerindeki tüm operasyonlar, söz konusu Transaction’ a dahil edilir ve Rollback mekanizmasından yararlanılabilinir.
- Kayıt edilmek istenen her tip için mutlaka bir Primary View oluşturulması gerekmekedir.
- Pek tabi sorgulamalarda anahtar noktalardan birisi bir index yapısının kullanılmasıdır.
- LINQ (Language INtegrated Query) desteği mevcuttur.
- Sistem de saklanması düşünülen resim,müzik gibi byte[] array’ ler ile ifade edilebilecek yapılar için ayrı fiziki dosyalarda verinin saklanabilmesi mümkündür (Test etmedim)
- Performansı arttırmaya yönelik olarak Task Parallel Library’ den yararlanıldığı ve bu nedenle minimum.Net Framework 4.0 ortamına ihtiyaç duyulduğu ifade edilmektedir.
- İncelediğim versiyona göre 4 milyar adet doküman depolanabilmektedir.
- Apache License 2.0 ile lisanslanmıştır.

Bu genel özellikler dışında, RaptorDB’ yi kullanarak veri oluşturma noktasında izlenen yolu da aşağıdaki şekilde görüldüğü gibi ifade edebiliriz.

[![raptor_6](/assets/images/2014/raptor_6_thumb_1.png)](/assets/images/2014/raptor_6_1.png)

Çok doğal olarak bir.Net geliştiricisinin veriyi tanımlamadaki en güçlü kozu POCO (Plaint Old CLR Objects) tipleridir. Burada da Entity’ ler aslında birer POCO tipidir. Herhangibir Attribute içermezler ve genellikle sadece basit özellikler (Property) lerden oluşurlar. Bir Entity tanımlandıktan sonra genellikle tepede yer alanlar için (örneğin Category ve Product senaryosunda Category tipi için) Primary View’ lar oluşturulur. Bu View’ lar aynı zamanda verinin sorgulanması sırasında da rol oynamakta olduğundan iyi tasarlanmalıdır. View’ lar kendi içlerinde bir index yapısını da belirlemektedirler.

Entity’ ler için bir veya daha fazla View tanımlanabilir ama mutlaka bir Primary Key oluşturulmalıdır. Oluşturulan View’ ların etkinliği Register edilmelerine bağlıdır. Bu sepeten RaptorDB’ ye bir bildirim yapılır.

Bu hazırlıklar sonrasında örnek Entity’ ler kayıt edilebilir veya var olan View’ lardan yararlanılarak bazı sorgulamalar yürütülebilir. İstenirse bu aşamadan sonra yeni View’ lar da tanımlanabilir ve farklı indeksleme seçeneklerine uygun olacak şekilde yeni sorgulama alanları kullanılabilir.

Aslında View’ lar üretilirken var olan doküman verileri de, Map adı verilen fonksiyonlar yardımıyla View içerisine alınırlar. Doküman (Document) esas itibariyle bir Entity veya bir Entity örneği (Object) olarak düşünülmelidir. Her doküman, sistem içerisinde bir Guid ile benzersiz olarak işaretlenir. Sorgulamanın en hassas kısmı iyi indeksler kullanılmasıdır. Bu sebepten View’ lar içerisinde indeks’ lere ilişkin basit şemalar kullanılır. Bu şemalar aslında birer inner type’ dır ve View içerisinde bir sınıf olarak ifade edilir.

> View, belli bir amaç için oluşturulmuş çok boyutlu bir doküman (Multi Dimensional Document) nesnesinin, 2 boyutlu resmidir.
> Mehdi Gholam

Örnek

Dilerseniz bu kadar laf kalabalığı ve Key Note’ dan sonra basit bir örnek üzerinden ilerlemeye çalışalım. Her zamanki gibi bir Console uygulamasına odaklanıyor olacağız. Ürünü CodeProject veya CodePlex üzerinden tedarik ettikten sonra Source Code’ u Visual Studio ortamında açıp Build etmeniz yeterli olacaktır. Elde edilen üretim sonrasında ortaya çıkan Assembly’ ı ise, RaptorDB’ yi kullanmak istediğimiz uygulamaya referans etmemiz gerekmektedir.

Bu arada makaleyi yazdığım zaman diliminde ilgili kodlar Visual Studio 2010 tabanlıydılar. Ancak Visual Studio 2012 ile sorunsuz açılıp,.Net Framework 4.5’e taşınabildiler ve problemsiz derlendiler.

[![raptor_1](/assets/images/2014/raptor_1_thumb_1.png)](/assets/images/2014/raptor_1_1.png)

Gelelim uygulamada inşa ettiğimiz tip yapısına ve kodlara.

[![raptor_10](/assets/images/2014/raptor_10_thumb_1.png)](/assets/images/2014/raptor_10_1.png)

```csharp
using RaptorDB; 
using RaptorDB.Views; 
using System; 
using System.Collections.Generic; 
using System.Diagnostics;

namespace HowTo_RaptorDB 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            string[] categoryNames = {"Kitap","Bilgisayar","Elektronik","Çiçek","Oyuncak","Kırtasiye" };

            RaptorDB.RaptorDB db = RaptorDB.RaptorDB.Open("Data"); 
            
            if (db.RegisterView(new CategoryView()).OK == false) 
            { 
                Console.WriteLine("Registration işlemi sırasında bir hata oluştu. Programdan çıkılacak! Çık"); 
                return; 
            }

            if (db.RegisterView(new CategoryWithProductsView()).OK == false) 
            { 
                Console.WriteLine("Registration işlemi sırasında bir hata oluştu. Programdan çıkılacak! Çık"); 
                return; 
            }

            #region Veri Oluşturmak

            Stopwatch watcher = new Stopwatch(); 
            watcher.Start();

            foreach (string categoryName in categoryNames) 
            { 
                var newCategory = new Category 
                           { 
                               CategoryId = Guid.NewGuid(), 
                               CategoryName = categoryName, 
                               Description = categoryName + " için açıklama" 
                           }; 
                newCategory.Products = new List<Product>();

                for (int j = 0; j < 100000; j++) 
                { 
                    newCategory.Products.Add( 
                        new Product 
                        { 
                            ProductId = Guid.NewGuid(), 
                            Title = "Product " + j.ToString(), 
                            UnitPrice = 10.45M, 
                            StockSize = 100 
                        }); 
                }

                db.Save<Category>(newCategory.CategoryId, newCategory); 
            }

            watcher.Stop(); 
            Console.WriteLine("Toplam kaydetme süresi {0}" 
                , watcher.ElapsedMilliseconds.ToString() 
                ); 
            Console.ReadLine();

            #endregion Veri Oluşturmak

            #region Veri Sorgulamak

            var result = db 
                .Query( 
                    typeof(Category) 
                    , (Category c) => (c.CategoryName == "Kitap") 
                    );

            Console.WriteLine(result.TotalCount);

            var result2 = db 
                .Query("CategoryWithProductsView" 
                , (Category c) => (c.CategoryName == "Kitap") 
                );

            Console.WriteLine(result2.TotalCount); 
                
            #endregion Veri Sorgulamak 
        } 
    }

    public class Category 
    { 
        public Guid CategoryId { get; set; } 
        public string CategoryName { get; set; } 
        public string Description { get; set; } 
        public List<Product> Products { get; set; } 
    }

    public class Product 
    { 
        public Guid ProductId { get; set; } 
        public string Title { get; set; } 
        public decimal UnitPrice { get; set; } 
        public int StockSize { get; set; } 
    }

    public class CategoryView 
        : View<Category> 
    { 
        public class RowSchema 
        { 
            public string CategoryName { get; set; } 
            public string Description { get; set; } 
        }

        public CategoryView() 
        { 
            this.Name = "PrimaryCategoryView"; 
            this.Description = "Primary View of Category"; 
            this.isPrimaryList = true; 
            this.isActive = true; 
            this.BackgroundIndexing = true;

            this.Schema = typeof(CategoryView.RowSchema);

            this.AddFireOnTypes(typeof(Category));

            this.Mapper = (api, docId, doc) => 
            { 
                    api.Emit(docId 
                        , doc.CategoryName 
                        , doc.Description 
                        );                                
            }; 
        } 
    }

    public class CategoryWithProductsView 
    : View<Category> 
    { 
        public class RowSchemaV2 
        { 
            public NormalString ProductTitle { get; set; } 
            public int ProductStockSize { get; set; } 
            public string CategoryName { get; set; } 
            public string Description { get; set; } 
        }

        public CategoryWithProductsView() 
        { 
            this.Name = "CategoryWithProductsView"; 
            this.Description = "Category with Products View"; 
            this.isPrimaryList = false; 
            this.isActive = true; 
            this.BackgroundIndexing = true;

            this.Schema = typeof(CategoryWithProductsView.RowSchemaV2);

            this.AddFireOnTypes(typeof(Category));

            this.Mapper = (api, docId, doc) => 
            { 
                foreach (var product in doc.Products) 
                    api.Emit(docId 
                        , product.Title 
                        ,product.StockSize 
                        , doc.CategoryName 
                        , doc.Description 
                        ); 
            }; 
        } 
    } 
}
```

Kodun İşleyişi

İki adet POCO tipi söz konusudur. Category ve Product. Tahmin edileceği üzere bir kategori altında n sayıda ürün olabilmelidir. Bu ilişki için Category sınıfı içerisinde Product tipinden generic bir List kullanılmaktadır.

İki adet View tipi söz konusudur. Bunlardan birisi Primary View olarak set edilmiştir. Her iki View için söylenebilecek en önemli özellik ise içerdikleri dahili tip tanımlarıdır. RowSchema ve RowSchemaV2 içerisinde bir index yapısının da belirtildiğini ifade edebiliriz. Öyleki burada tanımlı olan alanlar için sorgulamalar yapılabilir. Bu nedenle View’ ların yapıcı metodlarında (Constructors) kullanılan Mapper fonksiyonelliklerinde bir Emit işlemi gerçekleştirilmektedir. Dikkat edileceği üzere CategoryView için kategori adı ve açıklamasından oluşan bir veri girişi söz konusudur. Ancak CategoryWithProductsView için kategori adı ve açıklaması haricinde o kategoriye dahil olan ürün başlığı ve stok miktarı da söz konusudur.

Yani bir View tanımlanırken, içerisinde sorgulama için kritik olan index yapısının şeması yine bir sınıf olarak tanımlanır ve verilerin oluşturulması aşamasında bu içeriğin üretilmesi Mapper’ lar yardımıyla sağlanır. Tabi gözden kaçırılmaması gereken bir nokta da View’ ların aslında generic View sınıfında türemiş olmalarıdır. Bu üst sınıfdan gelen isPrimaryViewList, isActive, BackgroundIndexing, Schema, Mapper gibi özellikler ve AddFireOnTypes gibi fonksiyonlar yapıcı metod içerisinde kullanılmaktadır.

View’ ların etkinliği veritabanına kayıt edilmelerine bağlıdır. Bu nedenle üretilen RaptorDB nesne örneği üzerinden RegisterView metodu kullanılmış ve bunlara yazılan View’ ların nesne örnekleri bildirilmiştir.

Aslında bundan sonraki aşamalar oldukça kolaydır. Bir dokümanı kaydetmek için RaptorDB örneğinin Save metodu kullanılırken ilk parametre olarak bir Guid bilgisi verilir. Diğer yandan sorgulamalar için Query fonksiyonu ele alınmaktadır. Query fonksiyonunun farklı versiyonları vardır. Eğer parametre olarak bir Entity nesne örneği verilirse bu durumda ilgili Entity için belirtilen Primary View kim ise, sorgulama için o View örneği ele alınır. Ancak farklı bir View kullanılması isteniyorsa, bu durumda ilgili View’ un tipi veya string olarak adı kullanılabilir.

Debug Anında

RaptorDB nesnesi örneklenirken parametre olarak bir klasör belirtilir. Eğer bu klasör yoksa uygulamanın çalışması sırasında oluşturulur. Ben örneği test ederken RaptorDB’ nin örneklendiği noktada durdum ve aşağıdaki klasör yapısının otomatik olarak üretildiğine şahit oldum.

[![raptor_2](/assets/images/2014/raptor_2_thumb.png)](/assets/images/2014/raptor_2.png)

Hatta Data klasörü içerisine baktığımda farklı uzantılarda pek çok dosyanın üretildiğini ancak içerisine bir veri atılmadığını fark ettim.

[![raptor_3](/assets/images/2014/raptor_3_thumb.png)](/assets/images/2014/raptor_3.png)

Her ne zaman Entity örnekleri birer Doküman olarak kayıt edilmeye başladılar işte o zaman mgdat ve mgidx uzantılı dosyaların içerikleri değişmeye başladı. Örnek olarak her bir kategori için 100bin Dummy Product nesne örneği ürettiğimde yaklaşık olarak 188 megabyte’ lık veri kümesinin 4 saniyeye yakın bir sürede üretildiğine tanık oldum. Diğer ürünler ile kıyaslanmasına bakmadım tabi ama bu süre oldukça etkileyici gibi geldi bana

![Sarcastic smile](/assets/images/2014/wlEmoticon-sarcasticsmile_15.png)

[![raptor_4](/assets/images/2014/raptor_4_thumb.png)](/assets/images/2014/raptor_4.png)

ve tabi data.mgdat dosyasının içerisine bakıldığında verinin buraya serileştiğine şahit oldum.

[![raptor_7](/assets/images/2014/raptor_7_thumb.png)](/assets/images/2014/raptor_7.png)

Örneğin Primary View’ un kullanıldığı sorgulama senaryosunda, CategoryView içerisinde belirtilen şema yapısına uygun bir sonuç kümesinin çekildiği görülmektedir. Aşağıdaki Breakpoint görselinde bu durum ifade edilmektedir.

[![raptor_8](/assets/images/2014/raptor_8_thumb.png)](/assets/images/2014/raptor_8.png)

Dikkat edileceği üzere sadece kategori adı ve açıklaması ile benzersiz bir Guid söz konusudur. Hiç bir Product yoktur. Zaten buradan o kategoriye bağlı ürünlere de gidilemeyecektir. Nitekim View’ un index yapısından kategorinin ürünleri belirtilmemiştir. Lakin ikinci View’ da farklı bir şema kullanılmıştır. Bu nedenle Kategori ve buna bağlı ürünler elde edilebilir. Aşağıdaki Breakpoint görseline dikkat edelim (100binlik örnek senaryo yerine 20 adet Dummy Product tipi ele alınmıştır)

[![raptor_9](/assets/images/2014/raptor_9_thumb.png)](/assets/images/2014/raptor_9.png)

Görüldüğü üzere 20 satırlık bir sonuç kümesi söz konusudur ve her bir satır içerisinde CategoryWithProductsView’ da belirtilen şemaya uygun alanlar yer almaktadır. Ürün başlığı (Title), stok miktarı (StockSize), kategori adı (CategoryName) ve kategori açıklaması (Description). Bu son derece doğaldır çünkü ilgili View içerisindeki şema yapısı buna uygun olacak şekildedir. Pek tabi bu şema yapısı indekside ifade ettiğinden, LINQ sorgusunda belirtilen alanlardan biri veya bir kaçının kullanılabilirliği söz konusudur.

Görüldüğü üzere RaptorDB, kullanımı ve felsefesi ile dikkat çekicidir. Hızlı bir Document Store NoSQL ürünü olduğunu ifade edebiliriz. Açık kaynak olması da işin cabası

![Smile](/assets/images/2014/wlEmoticon-smile_89.png)

Böylece geldik bir makalemizin daha sonuna. Bir sonraki yazımızda görüşünceye dek hepinize mutlu günler dilerim.

[HowTo_RaptorDB.zip (162,25 kb)](/assets/files/2014/HowTo_RaptorDB.zip)