---
title: "NoSQL Maceraları–Redis ile Hello World"
pubDate: 2014-02-11 13:04:00
categories:
  - NoSQL
tags:
  - NoSQL
  - C#
  - .NET
  - LINQ
  - Redis
  - JSON
  - HTTP
  - Ruby
  - JavaScript
  - Node.js
  - Caching
  - Generics
  - Visual Studio
  - GitHub
---

# NoSQL Maceraları–Redis ile Hello World
[![Blizzard_Entertainment_Logo.svg](images/Blizzard_Entertainment_Logo.svg_thumb.png)](images/Blizzard_Entertainment_Logo.svg.png) Merhaba Arkadaşlar,

Üniversite yıllarında en çok oynadığım oyunlar sanıyorum ki Warcraft II ve Starcraft idi. Sonrasında bunlara Diablo’ da eklendi. Bu üç güzide oyunun ortak özelliği ise Blizzard firması tarafından geliştirilmiş olmalarıydı. 1991 yılının bir Şubat ayında (soğuk muydu hava bilemiyorum) UCLA üniversitesi mezunu Michale Morhaime, Allen Adham ve Frank Pearce tarafından Kaliforniya’ da kurulan şirket, 2012 itibariyle tam olarak 4700 çalışana sahip. (Hani çalıştığım uluslararası bankanın yaptığı işleri ve IT departmanını düşünce gerçekten durup 8 kere düşünüyorum) Peki firmanın kendi ortamında kullandığı ürünlerden birisinin Redis isimli NoSQL sistemi olduğunu biliyor muydunuz?

![Smile](images/wlEmoticon-smile_90.png)

Buyrun öyleyse yeni yazımızda Redis’ i tanımaya başlayalım.

Bildiğiniz üzere bir süredir NoSQL veritabanı sistemlerinin,.Net tarafındaki kullanımlarını basit Hello World uygulamaları ile incelemeye çalışmaktayız. Bu günkü yazımıza konu olan ürün ise, popüler NoSQL sistemler arasında yer alan Redis. Oldukça popüler sayılabilecek olan bu ürünün kullanıcıları arasında hemen hemen her gün ziyaret ettiğimiz pek çok site bulunmakta. Stackoverflow, instagram, flickr, blizzard, github, disqus ve hatta guardian. Tabi dikkat çekici noktalardan birisi tüm bu örneklerin web tabanlı birer uygulama olması.

Genel Özellikleri

Detaylı bilgisine [bu adresten](http://redis.io/topics/introduction) ulaşabileceğiniz Redis, C ile yazılmış olan, Client/Server modelinde çalışan, Key/Value Store tipinden bir NoSQL (Not Only SQL) veritabanıdır. Veriyi bellek üzerinde (in-memory) tutmak üzerine inşa edilmiştir. Bu nedenle özellikle key/value store’ lara ulaşılması noktasında önemli hız avantajlarına sahiptir. Diğer yandan Persistence modeli temelde ikiye ayrılır. Snapshot ve Semi-Persistence Durability mod. Durability moda ihtiyaç duyulmadığı hallerde tamamen bellek üzerinden çalışıyor olmak, key/value store’ ların okunması ve yazılmasında maksimum performansı vermektedir.

Redis, ağırlıklı olarak string tipinin kullanıldığı key/value çiftlerinden oluşan Dictionary’ ler ile çalışır. Ancak özellike Value tarafında çeşitli liste tiplerine de destek verir. Bir başka deyişle, Value tipleri sadece string olmak zorunda değildir. List, Sets, Sorted Sets ve Hashed Sets gibi string listeler de desteklenmektedir.

Orjinal olarak yazıldığı C programlama dilinden C#’ a, iPhone uygulamarı geliştirilmesinde kullanılan Objective-C’ den ActinScript’ e, popüler JavaScript kütüphanesi Node.js’ den Ruby’ ye, PHP’ den Haskell’ e kadar geniş bir dil yelpazesi tarafından kullanılabilmektedir.

BSD (Berkley Software Distribution) lisansı altındadır ve bu sebepten açık kaynak olarak kullanılmasında/geliştirilmesinde neredeyse hiç bir sınır yoktur. Ürün normalde production ortamı olarak Linux sistemler üzerinde koşmaktadır. Ancak [şu adresten](http://redis.io/download) de görebileceğiniz üzere Windows ortamında servis olarak çalışabilecek bir versiyonuda Microsoft Open Tech grubu tarafından geliştirilmiştir. (Win32/64bit sürümlerine göre geliştirilmiş olan bu versiyon Official değildir. En azından yazıyı hazırladığım tarih itibariyle durum buydu)

Bir key/value depolama modelini kullandığından, sistem içerisine dahil olacak veriler mutlak suretle bir key ile ilişkilendirilir.

Biz geliştireceğimiz Hello World örneğinde, Microsoft Open Tech grubunun yazdığı sunucu versiyonunu ve [git üzerinden indirilebilecek](https://github.com/ServiceStack/ServiceStack.Redis) ServiceStack.Redis isimli istemci kütüphanesini (Client Library) kullanıyor olacağız.

İlk Adımlar

Öncelikli olarak sunucu tarafını ayağa kaldırmamız gerekiyor. Bu amaçla, yine git üzerinden indirebileceğimiz MSOpenTech.Redis dosyasını açarak işe başlayabiliriz. Bu içerikte kaynak kodları barındıran bir Solution ve hemen sunucuyu ayağa kaldırabileceğimiz programlar yer almaktadır.

Makalenin yazıldığı ve yayınlandığı tarihlere göre kullanılan araçlara ait sürüm farklılıkları olabilir. Güncel sürümler ile test etmeyi unutmayınız.

Solution içeriğine msvs klaösöründen ulaşılabilinir. Bu içerik Visual Studio 2010 ile yazılmış C++ projelerinden oluşmaktadır. İstenirse Visual Studio 2012 ortamında açılabilir fakat dönüşüm işlemi sırasında C++ çalışma zamanının güncellenmemesi gerekir. Aksi durumda derleme zamanı hataları alınacaktır.

[![redis_1](images/redis_1_thumb.png)](images/redis_1.png)

bin/release klasöründe, bir kaç zip arşivi yer almakta olup bunların içerisinde hemen çalıştırılabilir dosyalar yer almaktadır (32/64 bit sürümler dahildir) Örneğin redistbin arşivinde, redis-benchmark, redis-check-aof, redis-check-dump, redis-cli ve redis-server isimli çalıştırılabilir komut satırı programları yer almaktadır. Bu programlardan redis-server tahmin edileceği üzere sunucudur. Diğer yandan redis-cli, çalışmakta olan sunucuya bağlanıp hemen komut gönderebileceğimiz (Veri eklemek, silmek, okumak vb) bir programcıktır.

> Buradaki çalışma senaryosu daha çok Redis’ i öğrenmek ve basit geliştirmeler yapmak noktasında anlamlıdır. Production ortamlarında başta da belirttiğimiz üzere Linux tabanlı sunucular ele alınmaktadır.

Dilerseniz hemen sunucuyu çalıştıralım ve hatta client uygulamasını kullanarak bir komutun nasıl çalıştırılabileceğine bakalım. Redis-server uygulaması varsayılan olarak 6379 numaralı portu dinleyen bir servisi başlatacaktır. (Aslında Redis sunucu ayarları dışarıdan konfigure edilebilir. Port değiştirilebilir ve hatta master bir sunucunun altında yer alacak slave Redis sunucuları çalıştırılabilir ki bu Replication özelliğini desteklemesinden ötürüdür. Konfigurasyon tarafı ile ilişkili kullanım bilgileri için [şu adresteki dokümanı](http://redis.io/topics/config) incelemenizi öneririm)

> Redis istemcisinde kullanılabilecek olan komutlara [bu adresten ulaşabilir](http://redis.io/commands) ve hatta canlı canlı test edebilirsiniz. Özellikle [şu adreste yayınlanan online tutorial](http://try.redis.io/)’ ını mutlaka denemenizi öneririm.

Aşağıdaki örnek ekran çıktısında Redis üzerinden person.Name ve person.Salary isimli iki key (anahtar) üretilmiş ve örnek değerleri (Value) verilerek okunmuştur. Bunun için basit olarak set ve get metodlarından yararlanılmaktadır.

[![redis_3](images/redis_3_thumb.png)](images/redis_3.png)

Elbette komut satırı üzerinden hareket etmek her zaman için mantıklı değildir. Görsel arayüze sahip uygulamaların veya diğer katmanların, bu fonksiyonellikleri daha basit bir şekilde ele alabiliyor olmaları gerekir. İşte bu noktada yine [Git üzerinden erişebileceğimiz ServiceStack.Redis](https://github.com/ServiceStack/ServiceStack.Redis) kütüphanesini kullanabiliriz.

Örnek İstemci Uygulama

Aslında ServiceStack, Mono projesi lideri olan Miguel De Icaza’ nın geliştirdiği ve yine [Git üzerinden elde edebilecek olan](https://github.com/migueldeicaza/redis-sharp) Redis-Sharp isimli ürünün bir açılımıdır. Kaynak kodları ile birlikte gelen ServiceStack arşivi içerisinde ServiceStack.Common, ServiceStack.Interfaces, ServiceStack.Redis ve ServiceStack.Text isimli Assembly’ lar bulunmaktadır. Bu assembly’ ların, Redis ile iletişimde olacak istemci uygulamaya referans edilmeleri suretiyle ilerlenebileceği gibi, ilgili ürün NuGet Paket Yönetim aracı ile de projeye alınabilir.

[![redis_4](images/redis_4_thumb.png)](images/redis_4.png)

Ben örnek uygulamada NuGet aracını kullanmayı tercih ettim. Paket ekleme işlemi sonrasında Solution içeriği de aşağıdaki ekran görüntüsündeki gibi olacaktır.

[![redis_5](images/redis_5_thumb.png)](images/redis_5.png)

İlgili referanslar da eklendiğine göre artık bir parça kod yazarak Redis ile konuşmaya başlayabiliriz. Console olarak geliştireceğimiz örnek uygulamamızda aşağıdaki sınıf diagramında yer alan ve Object Oriented dünyada asla vazgeçemediğimiz POCO (Plain Old CLR Objects) tiplerini ele alıyor olacağız.

[![redis_6](images/redis_6_thumb.png)](images/redis_6.png)

ve örnek kodlarımız;

```csharp
using ServiceStack.Redis; 
using ServiceStack.Redis.Generic; 
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Text; 
using System.Threading.Tasks;

namespace HowTo_Redis 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            // ilk önce Sunucu ile aramızda bir kanal açalım 
            using(RedisClient client=new RedisClient("127.0.0.1",6379)) 
            { 
                // T ile belirtilen tip ile çalışabileceğimiz bir Redis arayüzünü tedarik etmemizi sağlar 
                IRedisTypedClient<Person> personStore=client.As<Person>();

                #region Örnek bir veri kümesinin eklenmesi

                // Temiz bir başlangıç için istenirse var olan Person kümesi silinebilir de 
               if (personStore.GetAll().Count > 0) 
                    personStore.DeleteAll();

                // Bir kaç örnek departman ve personel verisi oluşturuyoruz 
                Department itDepartment = new Department 
                        { 
                            DepartmentId = 1000, 
                            Name = "IT", 
                            Description = "Information Technologies" 
                        }; 
                Department financeDepartment = new Department 
                        { 
                            DepartmentId = 1000, 
                            Name = "Finance", 
                            Description = "Finance Unit" 
                        };

                List<Person> persons = new List<Person> 
                { 
                    new Person 
                    { 
                        PersonId=personStore.GetNextSequence() 
                        , Name="Burak" 
                        , Level=100 
                        , Department=itDepartment 
                    }, 
                    new Person 
                    { 
                        PersonId=personStore.GetNextSequence() 
                        , Name="Bill" 
                        , Level=200 
                        , Department=itDepartment 
                    }, 
                    new Person 
                    { 
                        PersonId=personStore.GetNextSequence() 
                        , Name="Adriana" 
                        , Level=250 
                        , Department=itDepartment 
                    }, 
                    new Person 
                    { 
                        PersonId=personStore.GetNextSequence() 
                        , Name="Sakira" 
                        , Level=300 
                        , Department=financeDepartment 
                    }, 
                    new Person 
                    { 
                        PersonId=personStore.GetNextSequence() 
                        , Name="Bob" 
                        , Level=550 
                        , Department=financeDepartment 
                    } 
                };

                // Elemanları StoreAll metodu yardımıyla Redis' e alıyoruz. 
                personStore.StoreAll(persons);

                #endregion Örnek bir veri kümesinin eklenmesi

                #region Verileri elde etmek, sorgulamak

                Console.WriteLine("Tüm Personel"); 
                // Kaydettiğimiz elemanların tamamını GetAll metodu yardımıyla çekebiliriz. 
                foreach (var person in personStore.GetAll()) 
                { 
                    Console.WriteLine(person.ToString()); 
                }

                // Dilersek içeride tutulan Key/Value çiftlerinden Key değerlerine ulaşabiliriz 
                List<string> personKeys=personStore.GetAllKeys();

                Console.WriteLine("\nKey Bilgileri"); 
                foreach (var personKey in personKeys) 
                { 
                    Console.WriteLine(personKey); 
                }

                // İstersek bir LINQ sorgusunu GetAll metodu üstünden dönen liste üzerinden çalıştırabiliriz 
                IOrderedEnumerable<Person> itPersons = personStore 
                    .GetAll() 
                   .Where<Person>(p => p.Department.Name == "IT") 
                    .OrderByDescending(p => p.Level);

                Console.WriteLine("\nSadece IT personeli"); 
                foreach (var itPerson in itPersons) 
                { 
                    Console.WriteLine(itPerson.ToString()); 
                }

                // Random bir Key değeri alabilir ve bunun karşılığı olan value' yu çekebiliriz 
                string randomKey = personStore.GetRandomKey(); 
                Console.WriteLine("\nBulunan Key {0}",randomKey); 
                // seq:Person ve ids:Person key değerleri için hata oluşacağından try...catch' den kaçıp başka bir kontrol yapmaya çalışıyoruz. 
                if(randomKey!=personStore.SequenceKey 
                    && randomKey!="ids:Person") 
                { 
                    var personByKey = personStore[randomKey]; 
                    Console.WriteLine("{0}", personByKey.ToString()); 
                }

                personStore.SaveAsync(); // Kalıcı olarak veriyi persist edebiliriz. Asenkron olarak yapılabilen bir işlemdir.

                #endregion Verileri elde etmek, sorgulamak

                Console.WriteLine("\nÇıkmak için bir tuşa basınız"); 
                Console.ReadLine();                
            } 
        } 
    }

    class Person 
    { 
        public long PersonId { get; set; } 
        public string Name { get; set; } 
        public int Level { get; set; } 
        public Department Department { get; set; }

        public override string ToString() 
        { 
            return string.Format("{0}-{1} {2} {3}", PersonId, Name, Level, Department.Name); 
        } 
    }

    class Department 
    { 
        public int DepartmentId { get; set; } 
        public string Name { get; set; } 
        public string Description { get; set; } 
    } 
}
```

Uygulamamızda ilk olarak bir kaç Deparment ve bunlara bağlı Person nesne örneği oluşturmaktayız. Elde edilen nesnelerin Redis sunucusunun çalıştığı bellek bölgesinde konuşlandırılması için öncelikli olarak RedisClient tipinden bir istemci oluşturulmaktadır. Dikkat edileceği üzere bu örnekleme sırasında bir IP/makine adı bilgisi ve port numarası verilmiştir. Tahmin edileceği üzere bu bilgiler, Redis-server uygulamasının kullandığı sunucu adı ve port adresidir. Bir başka deyişle servise bağlanacak bir kanal açtığımızı ifade edebiliriz. Kanal üzerinde bir Person listesi ile çalışmak istediğimizden IRedisTypedClient arayüzü (interface) referansı ile taşınabilecek bir nesne de örneklenmiştir. Bundan sonraki tüm işlemler için bu nesne referansı kullanılmaktadır.

Örneğin bir Person listesini depolamak için StoreAll fonksiyonu kullanılmaktadır. Person tipinden tutulan tüm listeyi çekmek için GetAll, var olan listeyi silmek için DeleteAll, tüm Key değerlerini okumak içinse GetAllKeys isimli metodlardan yararlanılmıştır. GetAll gibi metodlar aslında geriye sorgulanabilir referanslar döndürmektedir. Bu sebepten ilgili listeler üzerinde LINQ sorguları da çalıştırılabilir. GetRandomKey fonksiyonu sayesinde var olan liste içerisinden rastgele bir Key değerinin alınması sağlanabilir ve bu Key ile ilişkili Value’ ya da gidilebilir. Uygulamayı çalıştırdığımızda aşağıdakine benzer bir ekran görüntüsü ile karşılaşırız.

[![redis_7](images/redis_7_thumb.png)](images/redis_7.png)

Örnek kodlarda yaptığımız önemli işlemlerden birisi de SaveAsync metoduna yapılan çağrıdır. Aslında bu çağrının yapılması şart değildir. Yapılmadığı durumda bildiğiniz üzere sunucu açık kaldığı veya nesneler için tanımlanabilen Expire süreleri dolmadığı müddetçe, verilere bellek üzerinden erişilebilinir. Ancak, Save ve SaveAsync gibi metodlar söz konusu Store nesnelerinin kalıcı olarak fiziki disk indirilmesine neden olur. Bu durumda sunucu kapansa da kayıt edilen bilgiler fiziki disk üzerinde yaşamaya devam ederler. Örneğimizin çalışması sonrası dosya sisteminde aşağıdakine benzer bir içerik oluşacaktır.

[![redis_8](images/redis_8_thumb.png)](images/redis_8.png)

Dikkat edileceği üzere dump.rdb isimli fiziki dosya içerisinde, personStore örneğinin tuttuğu ne kadar nesne örneği var ise, JSON (JavaScript Object Notation) formatında serileştirilmiştir.

Redis ürününü ServiceStack ile kullanmamız oldukça kolaydır. Nevar ki, ürünün kullanımı çok daha geniştir ve öğrenilmesi gereken epeyce özelliği de bulunmaktadır. Söz gelimi fiziki olarak kayıt edilen içeriği nasıl okuyabileceğimizi araştırarak işe başlayalabilirsiniz. Diğer yandan Amazon.com’ dan da görebileceğiniz aşağıdaki kitapları ve bu adresteki [The Redis Cookbook isimli online içeriği](http://rediscookbook.org/index.html) de incelemenizi öneririm.

[![redisbook_1](images/redisbook_1.gif)](http://www.amazon.com/Redis-Cookbook-Tiago-Macedo/dp/1449305040/ref=sr_1_1?s=books&ie=UTF8&qid=1361338144&sr=1-1&keywords=redis) [![redisbook_2](images/redisbook_2.gif)](http://www.amazon.com/Redis-Definitive-modeling-caching-messaging/dp/1449396097/ref=sr_1_2?s=books&ie=UTF8&qid=1361338144&sr=1-2&keywords=redis) [![redisbook_3](images/redisbook_3.gif)](http://www.amazon.com/Redis-Action-Josiah-L-Carlson/dp/1617290858/ref=sr_1_4?s=books&ie=UTF8&qid=1361338144&sr=1-4&keywords=redis)

Örneği zenginleştirmek ve daha da ileriye götürmek tabiki sizin elinizde. Bu yazımızda.Net Framework tarafından basitçe Redis’ e Merhaba demeye çalıştık. Böylece geldik bir yazımızın daha sonuna. Bir başka makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.

[HowTo_Redis.zip (1,83 mb)](assets/HowTo_Redis.zip)
