---
layout: post
title: "NoSQL Maceraları - MemCached ile Hello World"
date: 2013-12-18 23:45:00 +0300
categories:
  - nosql
tags:
  - nosql
  - xml
  - csharp
  - dotnet
  - wcf
  - web-api
  - http
  - authentication
  - python
  - java
  - ruby
  - performance
  - caching
  - serialization
---
Belki de açık kaynak olarak geliştirilen projelerin bu kadar popüler olma nedenlerinden bir tanesi de, logolarındaki orjinalliktir. Küçük bir ihtimal olsa da böyle bir algı olduğuna inanıyorum. Söz gelimi Linux’ un pengueni, Mono projesinin maymunu (Biliyorsunuz Mono İspanyolca’ da maymun demek), Android’ in yeşil robotu, Joomla’ nın birbirlerine kenetlenmiş insanları vb.

[![memcached_hero_thumb4](/assets/images/2013/memcached_hero_thumb4_thumb.jpg)](/assets/images/2013/memcached_hero_thumb4.jpg)


Bu gün inceleyeceğimiz açık kaynak NoSQL ürünlerinden birisi olan MemCached için de benzer bir durum söz konusu bana kalırsa. Logo’ da yer alan bu sevimli yaratıklar (belki de soğuk bir kış günü yürüyüş yapan atkı takmış kediler) ilk dikkatimi çeken yanı olmuştur.

Peki Memory ve Cached kelimelerinin kombinasyonundan oluşturulan MemCached, hangi amaçlarla ve nasıl kullanılabilir? BirHello World demeye var mısınız?

Genel Özellikleri

Web uygulamalarında ele alınan performans arttırma kriterlerinden birisi de Caching tekniklerinden yararlanılmasıdır. Özellike veri odaklı çalışan web uygulamaları göz önüne alındığında, nesnelerin bellekte tutulması/getirilmesi, sık görülen geliştirme tekniklerinden birisidir. Pek tabi bu senaryo biraz daha geniş çaplı düşünüldüğünde, bir web sunucusunda yer alan n sayıda uygulamanın tamamı için kullanılabilecek bellek tabanlı bir depolama alanı da söz konusu olacaktır/olmuştur.

Aslında bu tip bellek tabanlı çalışan nesne tutma sistemleri, çok uzun zamandır hayatımızdadır. Avantajlarından birisi veri sorgulama gibi ihtiyaçlarda, asıl fiziki veri kaynağı yerine RAM üzerinde duran canlı nesnelere gidilmesidir. Üstelik bu tip bir çalışmanın dağıtık sunucularda (Distributed Servers) mümkün olması da ayrı bir avantaj sunmaktadır. (Burada biraz durup Microsoft'un Velocity kod adlı Distributed Caching sistemini göz önüne getirebilirsiniz)

> Bilgisayar sistemlerinde her zaman için RAM üzerinde çalışmak, Fiziki Disk üzerinde çalışmaktan hızlı ve performanslıdır. Hatta bir ağ ortamında farklı bir sunucunun RAM’ ini kullanmak, yerel diski kullanmaktan daha performanslıdır.
> Ancak; RAM’ ın geçici bir alan olması ve makinenin bir şekilde restart olması sonrasında boşaltılması, fiziki depolama alanlarının kalıcılığı istenen veriler için biçilmiş kaftan olmasına neden olmaktadır.
> Yine de uygulamalar fiziki olarak tuttukları ve sık kullandıkları verileri zaman zaman ön belleğe alarak kullanmayı tercih ederler. Bu bağlamda melez depolama çözümleri dahi mevcuttur. Yani; RAM tabanlı önbellekleme (Caching) ile fiziki olarak kalıcı ortamların bir arada kullanılması.

Veriyi bellek üzerinde tutma konusunda NoSQL (Not-only SQL) tabanlı sistemlerin bulunduğunu da ifade etmemiz gerekir. Bunlardan birisi de, 2003 yılında Cprogramlama dili ile geliştirilmiş olan MemCached isimli üründür. MemCached aslında Key-Value Object Store tipinden olan ama bu nesne çiftlerini bellek üzerinde tutan bir NoSQL ürünüdür. Dağıtık yapıda çalışabilmektedir. Yani n sayıda sunucu üzerinde n sayıda servis formasyonunda bir ortam (Environment) hazırlanması mümkündür. Bu açıdan bakıldığında, Client/Server mimari üzerinde oturduğunu ifade edebiliriz. Aşağıda bu durumu izah eden basit bir vaka çizimi vardır.

[![memcached_0_thumb4](/assets/images/2013/memcached_0_thumb4_thumb.jpg)](/assets/images/2013/memcached_0_thumb4.jpg)

Bu çizimde MemCached için söz konusu olan tipik senaryo ifade edilmektedir. n sayıda, MemCached servisi barındıran sunucu ve n sayıda istemci. İstemciler, TCPgibi bir protokol üzerinden bir porta mesaj göndermek suretiyle MemCached servislerine bağlı kalır ve bellek üzerine nesne bırakıp, okuyabilirler.

Dikkat edilmesi gereken notkalardan birisi de, client-server arası iletişimin güvenliğidir. Normal şartlarda bir istemcinin, zaten kullanmak istediği sunucu ile aynı kapalı ağ içerisinde olduğu ve çeşitli Firewall kurallarının uygulandığı hallerde önemli bir güvenlik riski doğmamaktadır. Yine de ürün Simple Authentication and Security Layer (SASL) desteği sunmaktadır (Binary iletişim olmasını gerektiren bu framework ile ilişkili olarak [şu adresten bilgi alabilirsiniz](http://en.wikipedia.org/wiki/Simple_Authentication_and_Security_Layer))

> İncelediğimiz ürün basit Client-Server temellerine dayanan bir servis uygulaması ve istemcilerin bu servisi kullanmasına yardımcı olabilecek API’ ler bütününden oluşmaktadır. Teorik olarak istemciler servis tarafına erişip, bulundukları programlama dilinin çeşitli kriterlerine göre genellikle object tipinden örnekler göndermekte ve çekmektedir.
> Buraya kadar ki kısım tahmin edileceği üzere orta düzey bir.Net geliştiricisinin tasarlayıp, yazabileceği bir senaryodur. Ne varki işin zorlayıcı kısmı servisin işletim sisteminden tamamen bağımsız olması ve API’ lerin pek çok dile destek verecek şekilde yazılmış bulunmasıdır.
> Peki.Net geliştiricisi böyle bir ürün yazmak isterse ne yapar? Nasıl bir yol izler? WCF servislerinden yararlanır mı? Web API kullanır mı yoksa eskilere gidip.Net Remoting’ e başvurur mu? Bu konuyu bir düşünün. Vardığınız sonuçta, ürünü geliştirenin programlama dilinden bağımsız düşünmesi gerektiğini fark edecek ve belki de C’ ye kadar gideceksiniz.

MemCached sadece bellekteki nesneleri key-value yapısına göre devasa Hashtable'lar da tutan bir çalışma modeline sahiptir. Bu açıdan bakıldığında MemcacheDB, Couchbase Server ve Tarantool gibi benzeri ürünlerden farklılaşır. Nitekim bu ürünler bellek tabanlı nesne hizmeti haricinde, kalıcı bir depolama (Persistence Store) alanı da sunabilmektedir.

Bunun avantajı özellikle bellekteki tablolar dolduğunda kendini göstermektedir. Çünkü bir sebepten belleğin boşalması ve kaybolan key-value çiflerinin talep edilmeleri halinde kalıcı bir depolama alanı oldukça işe yarayacaktır. Örneğin MemCached, belleğin aşırı dolması veya Hashtable’ ın şişmesi durumunda, listenin alt sıralarında kalan eski key-value çiflerini temizlemek gibi bir davranış sergilemektedir. Bu durumda silinen key-value çiftleri bellekte yer almadığından istemci tarafından erişilebilir olmayacaktır. İstemcinin bunu kontrol etmesi gerekir.

> Yani bir nesne bellekten okunmak istendiğinde var olup olmadığına bakılmalı, var ise kullanılmalı, yok ise ve belleğe atılması gerekiyorsa da atılmalıdır. Hatta yok ise ve üretilmesi gereken bir nesne ise tekrardan üretilmeli sonrasında belleğe atılmalıdır.

Dolayısıyla MemCached servisini kullanırken istemcilerin, ilgili belleğin geçici olduğunu kabul ederek hareket etmeleri gerekmektedir.

Gelelim MemCached'in diğer belirgin özelliklerine;

- Platform bağımsızdır. Linux, Unix, MacOS X ve Windows Server gibi sistemlere yüklenebilir.
- Açık kaynaktır (Open Source)
- İstemciler için geniş bir API desteği de bulunmaktadır. [Bu adresten de](http://code.google.com/p/memcached/wiki/Clients) görülebileceği gibi C,C++,PHP,Java,Python,Ruby,Perl,.Net vb istemcilere açıktır.
- Yüksek performans sunmaktadır.
- Özellikle veritabanı kullanan dinamik web uygulamalarında performans arttırıcı olarak değerlendirilir (Daha çok web uygulamalarında tercih edildiğini ifade edebiliriz ama bu tabiki kanun hükmünde kararname değildir)
- Key içerikleri 250byte, Value içerikleri ise 1 Megabyte civarında olabilir ki bu değerler oldukça makuldur.
- İstemciler, servis ile iletişim kurarken bir hash değerini kullanırlar (Ne varki istemcilerin aynı hash değerlerini kullanmaları gibi bir durum söz konusu olursa, birbirlerinin cache'lenmiş verilerine de erişebilirler. Bu durumun oluşmamasını sağlamak gerekir)

Kayda değer noktalardan birisi de MemCached ürününü kullananlardır. İşte liste de yer alan dünya çapındaki örneklerden bir kaçı,

- Wikipedia
- Flickr
- Twitter
- Youtube
- Digg
- WordPress.com

Dolayısıyla endüstüriyel anlamda geçerliliğini kabul ettirmiş bir ürün üzerinde konuştuğumuzu ifade edebiliriz.

Install

Bu kadar hikayeden sonra dilerseniz basit bir örnek geliştirerek ilerlemeye çalışalım. İlk olarak MemCached'in install edilmesi gerekmektedir. Windows için gerekli versiyonlarını [bu adresten tedarik edebiliriz](http://blog.elijaa.org/index.php?post/2010/10/15/Memcached-for-Windows&similar). Ben örnek uygulamada 32 bitlik 1.4.5 sürümünü kullanmayı tercih ettim.

MemCached hizmetini çalıştırmak için exe uzantılı dosyanın yürütülmesi yeterlidir. Sunucudaki komut satırından bile bu işlem yapılabilir. Tabi tercih edilmesi gereken yol Windows Server tarafı için Windows Service'lerinden yararlanmak olmalıdır. Windows Service otomatik başlatılabilir moda ayarlanırsa, makine her ayağa kalktığında (ki sunucular genelde çok nadir resetlense de bu tedbirler alınmalıdır ve yazılımcının sorumluluğundadır) ilgili servis çalıştırılıp hizmet vermeye başlayacaktır.

[![memcached_1_thumb2](/assets/images/2013/memcached_1_thumb2_thumb.png)](/assets/images/2013/memcached_1_thumb2.png)

Uygulamayı -help komutu ile çalıştırdığımızda pek çok ek parametresi daha olduğunu görebiliriz. Aşağıdaki komut satırı ekranında bu durum özetlenmektedir.

[![memcached_2_thumb2](/assets/images/2013/memcached_2_thumb2_thumb.png)](/assets/images/2013/memcached_2_thumb2.png)

Çok doğal olarak bu bir servis uygulaması olduğundan, istemcilerin kullanabilmesi için çalışır durumda olması şarttır. Peki ya istemci tarafı?

İstemci Tarafı için NuGet Paketinin İndirilmesi

Yazımızın başlarında da belirttiğimiz gibi bunun için kullanılabilecek, ortama göre değişen pek çok API mevcuttur. Ancak NuGet üzerine yüklenmiş olanlar da var. Dolayısıyla NuGet Package Manager aracını kullanarak, istemci için gerekli Library'nin indirilmesini kolayca sağlayabiliriz. Ben örnek proje de Emyim’ in bir ürününü kullanmayı tercih ettim. Yazının hazırlandığı tarih itibariyle 2012 yılından güncel bir sürümü de mevcuttu.

[![memcached_3_thumb4](/assets/images/2013/memcached_3_thumb4_thumb.png)](/assets/images/2013/memcached_3_thumb4.png)

Install işlemi sonrası Enyim.Caching.dll assembly'ının projeye referans edildiği görülebilir.

[![memcached_4_thumb2](/assets/images/2013/memcached_4_thumb2_thumb.png)](/assets/images/2013/memcached_4_thumb2.png)

Örnek Uygulama

Gelelim örnek kodlarımıza (Bu aslında işin en basit kısmı olacak) Normal şartlarda bir web uygulamasının kullanılmasını tercih ederiz ancak burada amacımız sadece Hello World demek olduğundan test amaçlı basit bir Console uygulamasını tercih edebiliriz (Her zamanki gibi) İstemci tarafının MemCached servisini kullanması için ya kod tarafında ya da konfigurasyon dosyasında bir takım bildirimlerin yapılması gerekmektedir. Bunun için app.config dosyasını aşağıdaki gibi şekillendirmemiz yeterli olacaktır.

```xml
<?xml version="1.0" encoding="utf-8" ?> 
<configuration> 
  <configSections> 
    <sectionGroup name="enyim.com"> 
      <section name="memcached" 
               type="Enyim.Caching.Configuration.MemcachedClientSection, Enyim.Caching" /> 
    </sectionGroup> 
  </configSections> 
  <enyim.com> 
    <memcached protocol="Binary"> <!--Binary yerine Text' de kullanılabilir--> 
      <servers> 
        <add address="127.0.0.1" port="11211"/> 
      </servers> 
   </memcached> 
  </enyim.com> 
</configuration>
```

Burada kullanılan section adları varsayılan değerlerdir. Kodlarımızı ise aşağıda görüldüğü gibi yazabiliriz.

```csharp
using Enyim.Caching; 
using Enyim.Caching.Memcached; 
using System;

namespace HowTo_Memcached 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            Player burki = new Player 
            { 
                 PlayerId=1, 
                 Nickname="burki", 
                 TotalScore=10 
            };

            using(MemcachedClient client=new MemcachedClient()) // Default Constructor varsayılan olarak config dosyasında enyim.com/memcached sekmesini arar. 
           { 
                client.Store(StoreMode.Set, "burki", burki); // ikinci parametre key değeri iken son parametre value' dur(object tipindendir)

                object value=null; 
                if(client.TryGet("burki", out value)) 
                { 
                    Player cachedPlayer = value as Player; 
                   if(cachedPlayer!=null) 
                    { 
                        Console.WriteLine(cachedPlayer.ToString()); 
                    } 
                } 
            } 
        } 
    }

    [Serializable] // Veriyi binary modda transfer edeceğimizi belirttiğimiziden POCO' nun Binary formatta serileştirilebilir olması gerekmektedir. 
    public class Player 
    { 
        public int PlayerId { get; set; } 
        public string Nickname { get; set; } 
        public int TotalScore { get; set; }

        public override string ToString() 
        { 
            return string.Format("[{0}]-{1} {2}" 
                , PlayerId.ToString() 
                , Nickname 
                , TotalScore.ToString()); 
        } 
   } 
}
```

Örnekte Player tipinden bir nesne örneğinin belleğe burki isimli key değeri ile atılması ve sonrasında ise okunması senaryosu icra edilmektedir. Önemli nesnelerden birisi MemcachedClient tipi örneğidir. Varsayılan olarak app.config/web.config dosyasındaki emyim.com/memcached sektörünü baz alarak, belirtilen IP adresli makinenin 11211 portuna erişmeye çalışır. Tabi ilgili tipin yapıcılarına (Constructor) ait versiyonlarından da görüleceği üzere IMemcachedClientConfiguration arayüzü (interface) implemantasyonu yapan bir tip üzerinden de bu bilgiler alınabilir.

> Örnekte yerel makine üzerinde çalıştırılan Memcached.exe servisi söz konusu olduğundan, 127.0.0.1 adresine gidilmektedir. Çok doğal olarak gerçek hayat senaryolarında bu IP adresi farklı olabilir.
> Tabi yine gerçek hayat senaryolarına bakıldığında, istemci olarak düşünebileceğimiz web sunucusu ile MemCached servisini host eden sunucu arasındaki bağlantının Firewall engellerine takılmadan tesis edilmesi gerekebilir. Nitekim sunucularda hemen hemen pek çok port güvenlik nedeniyle bilinçli olarak kapatılır. Bu anlamda 11211 numaralı portun açık olması önemlidir.

Store fonksiyonu ile belleğe nesne atabilir veya güncelleyebiliriz. Get, Get, TryGet gibi metodlar yardımıyla da bir Key değerini okuyabiliriz. Örnekte TryGet ile nesneyi varsa almaya çalıştık. Tabi başka yararlı fonksiyonlarda vardır. Örneğin bellekteki tüm nesneleri FlushAll metodu ile atabilirsiniz veya n sayıda nesneyi elde etmek için ExecuteGet metodundan yararlanabilirsiniz. Dikkat edilmesi gereken önemli noktalardan birisi ise, kullandığımız Player sınıfının Binary formatta serileştirilebilir (Binary Serialization) olmasıdır. Bu, protokolü Binary olarak seçtiğimiz için şarttır. Örneği çalıştırdığımızda aşağıdaki ekran çıktısını elde ederiz.

[![memcached_5_thumb2](/assets/images/2013/memcached_5_thumb2_thumb.png)](/assets/images/2013/memcached_5_thumb2.png)

> Eğer MemCached servisi çalışmıyorsa, istemci tarafına bir Exception mesajı düşmemektedir. Doğal olarak belleğe atılmış nesneler varsa da erişilemez. Bu durumu istemci tarafının kontrol altına alması gerekir. Nitekim servis tarafı, kendisine bağlı olan istemcilere bir Notification’ da bulunmamaktadır.(Güncel sürümde ki durumu kontrol ediniz)

Bu yazımızda MemCached ürününe,.Net dünyasından Hello World demeye çalıştık. In-Memory modelinde performansı yüksek bir Distributed-Caching mekanizmasına ihtiyacımız olduğunda değerlendirebileceğimiz NoSQL ürünlerinden birisini inceleme fırsatı bulduk. Size tavsiyem söz konusu örneği web tabanlı bir veya daha fazla projede denemeniz yönünde olacaktır. Eğer lab ortamınız varsa, birden fazla MemCached servisinin bir arada çalışmasını da (farklı sunucularda olacak şekilde) test edebilirsiniz. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[HowTo_Memcached.zip (573,00 kb)](/assets/files/2013/HowTo_Memcached.zip)