---
layout: post
title: "NoSQL Maceraları - RavenDB ile Hello World"
date: 2013-01-12 12:05:00 +0300
categories:
  - nosql
tags:
  - ravendb
  - .net
  - json
  - open-source
  - nosql
  - schema-less
  - store
  - document
  - delete
  - load
  - query
  - generic
---
Kuzgun’ lar Kargagiller ailesinden gelen bir kuş çeşididir. Diğer karga cinslerine göre daha iridirler ve özellikle çok daha zeki oldukları söylenir. Yapılan araştırma ve deneyler sonrasında bu cins kargaların, sorunları çözmek için çevresel materyalleri kullanabilme (kullanmak için de öğrenebilme) becerisine sahip oldukları öne sürülmüştür. Hatta parlak, beyaz ve mavi renkli metallere karşı özel bir ilgileri olduğundan, hırsız olarak da ifade edilmektedirler.

[![173110_P001_Raven](/assets/images/2013/173110_P001_Raven_thumb.png)](/assets/images/2013/173110_P001_Raven.png)


Kanat açıklığı 1.5 metreyi bulan bu kuşlar, aynı zamanda deli cesaretine sahiptir

![Surprised smile](/assets/images/2013/wlEmoticon-surprisedsmile_3.png)

Niye mi? Çünkü, kendilerinden daha yırtıcı olan kuşlara hiç düşünmeden saldırabilirler. Tabi buradaki avantajları hep bir filo halinde hareket etmeleridir. Yani ekip olgusuna inanırlar. Bir diğer önemli özellikleri de ingilizce de Raven olarak adlandırılmalarıdır. (Daha fazla detay istiyorsanız [wikipedia bağlantısına](http://tr.wikipedia.org/wiki/Baya%C4%9F%C4%B1_kuzgun) bakabilirsiniz)

Gelelim Raven ile ne işimiz olduğuna

![Smile](/assets/images/2013/wlEmoticon-smile_77.png)

Açık kaynaklı NoSQL veritabanlarını incelemeye çalıştığımız ilk yazımızda, hatırlayacağınız üzere [Apache Cassandra](https://www.buraksenyurt.com/post/Apache-Cassandra-ve-Net)’ ya kısaca bir göz atmış ve basit bir Hello World uygulaması geliştirmiştik. Tabi NoSQL veritabanı sistemleri denilince pek çok ürün olduğunu görmekteyiz. İşte bu yazımızda bu ürünlerden dikkate değer bir tanesini daha inceleyeceğiz; RavenDB.

RavenDb, açık kaynak NoSQL veritabanlarındandır..Net ile yazılmıştır ve şemasız (Schema-less) JSON (Java Script Object Notation) veri tipini kullanmaktadır. Doküman tabanlı (Document Based) çalışmaktadır. JSON formatınının kullanılması ve.Net ile yazılmış olması, erişilebilirlik ve ölçeklenebilirlik anlamında da bazı avantajlar sunmaktadır. Bunların arasında LINQ (Language INtegrated Query) ile sorgulanabilme ve RESTful API ile ulaşılabilme sayılabilir. Bu veritabanı ayrıca Transactional’ dır. Bir başka deyişle ACID (Atomicity, Consistency, Isolation, Durability) prensiplerine destek vermektedir.

RavenDB’ nin daha çok Windows tabanlı.Net uygulamaları için geliştirildiği düşüncesi hakimdir. Ancak RESTful desteği olması sebebiyle farklı platformlara da açılabilir. Hatta sunduğu API sayesinde.Net, Silverlight, Javascript ve HTTP tabanlı REST istemcileri ile çalışabilir. JSON formatını kullanması verinin az yer tutması için de bir avantajdır.

> Document tipindeki NoSQL yapılarında genel olarak veri, bir dosya içerisinde saklanmakta ve bir anahtar (key) ile ilişkilendirilmektedir. Saklanacak veri için herhangibir şemaya (Schema) ihtiyaç yoktur.
> Bu nedenle SQL, Oracle gibi ilişkisel veritabanı sistemlerinde (Relational Database Management System) yapılması gereken, tablo tanımlama ve benzeri işlemler mevcut değildir.
> Çünkü buna gerekte yoktur
>
> ![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_167.png)
> Düşünce son derece basit olmaktan yanadır. “Veriyi doğrudan diske yaz, okumak istediğinde anahtarı ile ulaş”

RavenDB ile ilişkili bir ürün tanıtımı ve kuzgun’ ların kanat açıklığının 1.5 metre olması bilgilerinden sonra dilerseniz biraz da kod yazalım

![Laughing out loud](/assets/images/2013/wlEmoticon-laughingoutloud_3.png)

Öncelikli olarak RavenDB’ yi kurmamız gerekiyor. Yazıyı yazdığım günlerde [bunun için şu adrese bir uğramanız](http://ravendb.net/) gerekmekteydi. Bilgilerden de anlaşılacağı üzere ürünü, NuGet paket yönetim aracı yardımıyla tedarik edebilirsiniz de.

> NoSQL tipindeki veritabanı ürünlerinin bir diğer avantajıda kurulumlarının son derece kolay olmasıdır. Özellikle SQL, Oracle gibi ürünlerin kurulumları düşünüldüğünde
>
> ![Disappointed smile](/assets/images/2013/wlEmoticon-disappointedsmile_2.png)
>
> Pek çok NoSQL ürünü açık kaynak olarak indirilebilir ve doğrudan çalıştırılıp kullanılabilir. Bir install işleminden sürecinden geçilmesine çoğu zaman gerek duyulmamaktadır.

Başlatma

RavenDB içeriğini indirdikten sonra, Server klasörü altında yer alan Raven.Server.exe isimli uygulamanın çalıştırılması yeterlidir. Komut satırından yürütülen uygulama, sunucunun çalışmasını sağlayacaktır. Tahmin edileceği üzere ürün, client/server modeline göre çalışmaktadır. Sunucu açık olduğu sürece, istemcilerin RavenDB sistemini kullanması mümkündür.

[![rvndb_1](/assets/images/2013/rvndb_1_thumb.png)](/assets/images/2013/rvndb_1.png)

İşin güzel yanı, ürünün bir de web arayüzünün bulunmasıdır. Eğer makinenizde 8080 port’ u üzerinden yayın yapan bir başka uygulama var ise (ki benim sistemimde vardı) RavenDB, 8081 numaralı port üzerinden hizmet vermeye çalışacaktır (Eğer 8081 de doluysa tahminlerime göre bir sonraki boş portu bulana kadar deneyecektir)

Buna göre http://localhost:8081/ adresine gidildiğinde http://localhost:8081/raven/studio.html adresine yönlendirilip, web arayüzüne ulaşıldığı gözlemlenecektir.

[![rvndb_2](/assets/images/2013/rvndb_2_thumb.png)](/assets/images/2013/rvndb_2.png)

Bu arayüzden yararlanılarak verilerin eklenmesi, silinmesi, değiştirilmesi veya sorgulanması sağlanabilir. Elbette biz bunu kod üzerinden nasıl yapabileceğimizi incelemeye çalışacağız. Bu amaçla basit bir Console uygulaması oluşturarak işe başlayabiliriz.

İstemci için Hazırlık

RavenDB’ yi istemci tarafında ele alırken yardımcı kütüphane olan RavenDB.Client assembly’ ından yararlanılmaktadır. RavenDB’ yi indirdiğimiz zaman Client klasörü içerisinden bu kütüphanenin farklı versiyonlarına da erişilebilinir. (Hatta burada yakından tanıdığımız bir dost da vardır. Newtonsoft.json.dll ![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_167.png) Kendisi ile [buradaki](https://www.buraksenyurt.com/post/Tek-Fotoluk-Ipucu-69-Newtonsoft-JSONNet-ve-dynamic-Keyword.aspx) ve [şuradaki](https://www.buraksenyurt.com/post/Tek-Fotoluk-Ipucu-70-Newtonsoft-Jsonnet-and-dynamic-and-parsing.aspx) tek fotoluk ipuçlarında haşırneşir olmuştuk)

RavenDB istemci kütüphanesi, NuGet paket yönetim aracı ile de uygulamaya eklenebilir. Hatta bu şekilde ilerlenmesi, en güncel sürümün alınması ve yardımcı kütüphanelerin de indirilmesi açısından kolaylık sağlayan bir fonksiyonellik olarak görülmelidir.

[![rvndb_3](/assets/images/2013/rvndb_3_thumb.png)](/assets/images/2013/rvndb_3.png)

Ben örnekte NuGet aracından yararlanarak ilgili kurulum işlemini gerçekleştirdim. Bunun sonucunda uygulamaya aşağıdaki.Net kütüphanelerinin eklendiğine şahit oldum.

[![rvndb_4](/assets/images/2013/rvndb_4_thumb.png)](/assets/images/2013/rvndb_4.png)

Dikkat edileceği üzere 3ncü parti kütüphanelerden NLog ve Newtonsoft.Json assembly’ ları da, referans edilmiş durumdadır.

Referans işlemlerinin ardından, istemcinin hangi sunucuya bağlanacağını da belirtmemiz gerekiyor. Aslında bu, RavenDB ile olan bağlantıda kullanılacak ConnectionString bilgisidir. Dolayısıyla söz konusu bağlantı bilgisi app.config dosyası içerisinde aşağıdaki gibi tanımlanabilir.

```xml
<?xml version="1.0" encoding="utf-8" ?> 
<configuration> 
  <connectionStrings> 
    <add name="RavenDBConnection" connectionString="url=http://localhost:8081"/> 
  </connectionStrings> 
</configuration>
```

Çok doğal olarak ilk kuruluma göre RavenDB, localhost’ daki 8081 (benim makinem de böyle varsayılan olarak 8080 portu) portu üzerinden yayın yapmaktadır.

İlk Kodlar

Aşağıdaki örnek kodları geliştirdiğimizi düşünelim.

```csharp
using Raven.Client.Document; 
using System; 
using System.Linq;

namespace HelloWorldRavenDB 
{ 
    class Program 
    { 
        static DocumentStore docStore = null;

        static void Main(string[] args) 
        {            
            docStore=new DocumentStore { ConnectionStringName = "RavenDBConnection" }; 
            docStore.Initialize();

            #region Örnek ürünlerin eklenmesi

            AddProduct(new Product 
            { 
                Id="1", 
                ProductNumber = "E1-1001", 
                Title = "LG-Optical Mouse", 
                ListPrice = 34.50, 
                StockLevel = 100 
            } 
            );

            AddProduct(new Product 
            { 
                Id="2", 
                ProductNumber = "E1-1002", 
                Title = "LG-Optical Keyboard", 
                ListPrice = 44.25, 
                StockLevel = 85 
            } 
            );

            AddProduct(new Product 
            { 
                Id="3", 
                ProductNumber = "B1-1005", 
                Title = "SOA Design Patterns", 
                ListPrice = 49.95, 
                StockLevel = 12 
            } 
            );

            #endregion

            ListAllProducts();

            ChangeStockLevel("1",45);           

            ListAllProducts();

            DeleteProductByNumber("B1-1005"); 

            Product product=FindProductByNumber("E1-1002"); 
            Console.WriteLine(product.Title); 
        }

        private static void AddProduct(Product newProduct) 
        { 
            using (var session = docStore.OpenSession()) 
           {                
                session.Store(newProduct); 
                session.SaveChanges(); 
            } 
        }

        private static void ChangeStockLevel(string id,int newStockLevel) 
        { 
            using (var session = docStore.OpenSession()) 
            { 
                Product product = session.Load<Product>(id); 
                if(product!=null) 
                {                   
                    product.StockLevel = newStockLevel; 
                    session.SaveChanges(); 
                } 
            } 
        }

        private static void DeleteProductByNumber(string productNumber) 
        { 
            using (var session = docStore.OpenSession()) 
            { 
                Product product = session 
                    .Query<Product>() 
                    .Where(p => p.ProductNumber == productNumber) 
                   .SingleOrDefault(); 
                if (product != null) 
               { 
                    session.Delete(product); 
                    session.SaveChanges(); 
                } 
            } 
        }

        private static void ListAllProducts() 
        { 
            using (var session = docStore.OpenSession()) 
            { 
                var products = session.Query<Product>() 
                    .OrderBy(p => p.Title) 
                   .ToList();

                foreach (var product in products) 
                { 
                    Console.WriteLine("{0}-{1},{2},{3}" 
                        ,product.ProductNumber 
                        ,product.Title 
                        ,product.ListPrice.ToString("C2") 
                        ,product.StockLevel.ToString()); 
                } 
                Console.WriteLine(""); 
            } 
        }

        private static Product FindProductByNumber(string productNumber) 
        { 
            using (var session = docStore.OpenSession()) 
            { 
                return session 
                    .Query<Product>() 
                    .Where(p => p.ProductNumber == productNumber) 
                    .SingleOrDefault(); 
            } 
        } 
    }

    class Product 
    { 
        public string Id { get; set; } 
        public string ProductNumber { get; set; } 
        public string Title { get; set; } 
        public double ListPrice { get; set; } 
        public int StockLevel { get; set; } 
    } 
}
```

Daha önceden Entity Framework veya LINQ to SQL ile çalıştıysanız eğer, kodlardaki yaklaşım oldukçta tanıdık gelecektir. İlk olarak genel bir Context tipine ihtiyacımız bulunmakta. Bunun için DocumentStore tipinden yararlanılmaktadır.

DocumentStore örneklenirken ConnectionStringName özelliğine app.config dosyasında yer alan key değeri verilmiştir. Sonrasında gerçekleştirilen veri çekme, ekleme, silme ve güncelleştirme işlemlerinin tamamında ise OpenSession metoduna yapılan çağrı ile elde edilen IDocumentSession arayüzü türevli referans tipi kullanılmaktadır.

Veri çekme işlemlerinden de dikkat edileceği üzere LINQ metodlarından yararlanılmaktadır. Sorgulamalar için başlangıç noktası Query metodudur. Bunun dışında bir veriyi Key değeri üzerinden elde etmek istersek (ki Product sınıfındaki string türünden Id özelliği bunun için eklenmiştir) Load metodundan yararlanılabilinir. Veri ekleme için Store, silme işlemi içinse Delete fonksiyonları kullanılmıştır. Elbette yapılan tüm veri ekleme, silme ve güncelleştirme işlemlerinin, döküman içerisine yazılması SaveChanges metoduna yapılacak çağrı ile mümkün olmaktadır.

Uygulamayı çalıştırdığımzda, 3 adet Product örneğinin eklendiğini, bir tanesinin güncelleştirildiğini ve bir diğerinin de silindiğini analiz edebiliriz. Ayrıca tüm bu işlemler Web arayüzü üzerinden de anlık olarak takip edilebilirler. Örneği çalıştırdıktan sonra http://localhost:8081/raven/studio.html adresine gidersek aşağıdaki ekran görüntüsü ile karşılaşırız.

[![rvndb_5](/assets/images/2013/rvndb_5_thumb.png)](/assets/images/2013/rvndb_5.png)

1 numaralı ürün üzerinde durulduğunda ise verinin JSON formatındaki karşılığı da rahat bir şekilde gözlemlenebilir. Ayrıca herhangibir ürün açıldığında aşağıdaki ekran görüntüsü ile karşılaşılacaktır.

[![rvndb_6](/assets/images/2013/rvndb_6_thumb.png)](/assets/images/2013/rvndb_6.png)

Mutlaka sağ tarafta yer alan ETag değeri de dikkatinizi çekmiştir. Dilerseniz verilerinizi etag ile ilişkilendirebilirsiniz. Store metodunun aşırı yüklenmiş versiyonlarında GUID tipinden etag değerlerinin girilebilmesine de izin verilmektedir.

[![rvndb_7](/assets/images/2013/rvndb_7_thumb.png)](/assets/images/2013/rvndb_7.png)

veya

[![rvndb_8](/assets/images/2013/rvndb_8_thumb.png)](/assets/images/2013/rvndb_8.png)

Eklenen bütün ürünler Product isimlidir ve RavenDb tarafından isim çoğullama yapılarak Products adındaki koleksiyon içerisine dahil edilmişlerdir. Bu ve varsa diğer koleksiyonlara, Web arayüzündeki Collections kısmından ulaşılabilir.

[![rvndb_11](/assets/images/2013/rvndb_11_thumb.png)](/assets/images/2013/rvndb_11.png)

> Çok doğal olarak server etkin değilse istemci tarafı, çalışma zamanına bir WebException istisnası fırlatıyor olacaktır.[![rvndb_10](/assets/images/2013/rvndb_10_thumb.png)](/assets/images/2013/rvndb_10.png)

Diğer yandan uygulama çalıştırılmadan önce, çalıştığı süre zarfı içinde ve sonrasında, RavenDb.Server.exe programının komut satırına bazı loglar attığına şahit oluruz. Aynen aşağıdaki ekran görüntüsünde yer aldığı gibi.

[![rvndb_9](/assets/images/2013/rvndb_9_thumb.png)](/assets/images/2013/rvndb_9.png)

Dikkat edileceği üzere çeşitli HTTP metodları söz konusu olmuştur. Veri çekme işlemlerinde GET, ekleme işlemlerinde POST ve silme işlemlerinde de DELETE metodlarına ilişkin talepler (Request) oluşmuştur.

Bu arada 960 numaralı talebi dilerseniz URL den manuel olarak girmeyi deneyebilirsiniz. http://localhost:8081/docs/1 şeklinde bir talep gönderdiğimizde, indeks değeri 1 olan içeriğin JSON formatlı çıktısına ulaşırız.

{"ProductNumber":"E1-1001","Title":"LG-Optical Mouse","ListPrice":34.5,"StockLevel":45}

Bu çok doğal olarak RavenDB’ nin RESTful servis desteği sunmasından kaynaklanmaktadır.

Görüldüğü üzere RavenDB’ yi kullanmak oldukça basittir. Özellikle.Net geliştiricilerin aşina olduğu teknikler söz konusudur (LINQ metodlarının kullanılması gibi). Üstelik doğrudan POCO (Plain Old Clr Object) tipleri ile çalışılabilinir. Dolayısıyla kullanışlı bir NoSQL ürünü olduğunu ifade edebiliriz

![Open-mouthed smile](/assets/images/2013/wlEmoticon-openmouthedsmile_39.png)

Size tavsiyem söz konusu örnekte yer alan kodlardan yola çıkarak, örneği Web/Windows platformuna taşımanız olacaktır. Hatta hali hazırda kullanmakta olduğunuz minik ve veritabanı odaklı çalışan bir uygulamanız var ise, bunu RavenDB ile çalışacak şekilde yeniden kurgulamayı deneyebilirsiniz. Farklı NoSQL veritabanlarını inceledikçe kullanımlarını sizlerle paylaşmaya çalışıyor olacağım. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[HelloWorldRavenDB.zip (1,08 mb)](/assets/files/2013/HelloWorldRavenDB.zip)