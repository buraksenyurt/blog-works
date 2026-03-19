---
title: "Azure Üzerinde Redis Cache Kullanımı"
pubDate: 2018-09-01 07:02:00
categories:
  - Azure
tags:
  - Azure
  - Bash
  - C#
  - .NET
  - NoSQL
  - Redis
  - JSON
  - Web API
  - Python
  - Java
  - Node.js
  - Performance
  - Caching
  - Serialization
---

# Azure Üzerinde Redis Cache Kullanımı
![image.axd](images/image.axd)

Merhaba Arkadaşlar,

Bir süredir kişisel becerilerimizle alakalı olarak karşımıza çıkan T-Shaped Person isimli bir konu var (Aslında yıllardır var) Daha yakın zamanda katıldığım Scrum eğitiminde tekrardan karşıma çıkan ve [hatta şuradaki yazıyla](https://medium.com/@jchyip/why-t-shaped-people-e8706198e437) kısaca bilgilenebileceğiniz, özetle bir alanda gerçekten uzman ama bu alanla alakalı yan dallarda da bir şeyler yapabilen insan modelinden bahsediyorum.

Tek kişi için düşündüğümüzde bile çok yönlü bir birey geliyor aklımıza ama özellikle bir takımı bu tip insanlardan oluşturduğumuzda birbirlerinin açıklarını kapatabilen başarılı ekiplerin ortaya çıktığını görüyoruz. I-Shaped yerine T-Shaped olmak daha mühim bu nedenle. Özellike çevik ekiplerin başarısında önemli bir yere sahip.

> İşin aslı bir çok şekil var hayatımıza girmiş olan. I, T-shaped dışında M-Shaped, Comb-Shaped, Pi-Shaped, E-Shaped diye gidiyorlar araştırabildiğim kadarıyla. Üşenmeyin siz de araştırın. Bakın [burada da eneteresan bir yazı](https://peoplecentre.wordpress.com/2017/06/19/the-m-shaped-employee/) var konu ile ilgili.

Ben T-Shaped birisi olmaya çalışıyorum ama oldukça yavaş kaldığımı söyleyebilirim. Aslında kendi mesleğimle ilgilli pek çok konuda epey geriden geliyorum. Belki de sadece öğrensem ve yazmak için uğraşmasam şimdilerde daha ileri bir noktada olabilir T-Shaped forma biraz daha uyabilirdim. Ancak işin doğrusu öğrendiklerimi paylaşmayı seviyorum. En azından deneyimlerimi, konu ile ilgili başıma gelenleri aktarma fırsatı bulduğumu düşünüyorum.

Belki de bilmem kaç yüzünce kez karşılaştığınız bir konu ile karşınızdayım bu sebeple:) Özellikle NoSQL dünyasını tanıyanların yakından bildiği Redis ve Microsoft Azure platformuna konuk olacağız bu yazımıda. Amacımız bir [Redis](https://redis.io/topics/introduction) Cache hizmetini devreye almak ve basit bir.Net Core istemcisinden yararlanarak kendisiyle konuşmak. Redis, bellek tabanlı çalışan en popüler veri deoplama sistemlerinden birisi. In-Memory Data Structure Store olarak ifade ediliyor hatta. String, Hash, List, Set, Sorted Set, Bitmap, HyporLogLog gibi çeşitli veri türlerinin tutulmasına olanak sağlıyor. Ağrılıklı olarak uygulamaların performans kazanımı gerektiren vakalarında değerlendiriliyor. Bu, özellikle back-end tarafı için önemli. Veriyi bellek üzerinden yapısal olarak anlamlı şekilde tutmak ve hatta dağıtılabilir olarak sunmak büyük ölçekte istenen bir kabiliyet.

Bu açılardan düşünüldüğünde bulut bilişim hizmetlerinin de olmazsa olmaz kalemlerinden birisi olarak karşımıza çıkıyor. Öyle ki, bulut üzerine aldığımız uygulamaların veriye hızlı erişmesi gerektiği durumlarda ciddi anlamda kullanılıyor. Sık sık ihtiyaç duyulan, sürekli olarak değişime uğramayan verilerin fiziki diskten çekilmesi yerine bellekten alınması elbette performans açısından daha iyi. Lakin bunu dağıtık sistemler, ölçeklenebilirlik, veri çeşitliliği gibi noktalardan düşündüğümüzde Redis gibi çözlümlere yönelmemiz gerekiyor.

Artık kurulumundan, duruma göre ölçeklendirme planlarının oluşturulmasına kadar pek çok yönetsel işlevin bulut sistemleri tarafından sağlanıyor olduğunu da görüyoruz. Microsoft Azure platformu bu anlamda bizlere önemli bir imkan sunuyor. Azure üzerindeki Redis Cache hizmetini kullanarak bu tip bir tesisatı oluşturmak son derece kolay. Nasıl mı? Haydi gelin bir "Nasıl Yapılır?" macerasına daha başlayalım.

Redis Cache Kaynağını Oluşturmak

İşe azure portal üzerinden redis cache araması yaparak başlayabiliriz (Bu aşamada Microsoft Azure aboneliğinizin olduğunu varsayıyorum)

![image.axd](images/image.axd)

Redis Cache öğesini bulduktan sonra tek yapmamız gereken yeni bir tane oluşturmak. Diğer pek çok hizmette olduğu gibi isim, kaynak grubu, lokasyon ve benzeri bilgileri girmemiz gerekiyor. Ben DNS adı olarak Gondor'u kullandım ve "Kullandıkça Öde" tipindeki aboneliği seçtim. Bu vakaya özel olmasını istediğim için gondor-redis-rg isimli yeni bir Resource Group belirttim. Lokasyon olarak da West Europe tarafındaki sunucu merkezini işaret ettim.

![image.axd](images/image.axd)

Burada da bir fiyatlandırma söz konusu elbette:) Ücretsiz bir kullanımını bulamadım ancak geliştirme amacıyla C0 Basic isimli modeli değerlendirmemiz mümkün. Tabii başka modellerde var. "View Full Pricing Details" bağlantısına basarsak diğer seçenekleri görebiliriz.

![image.axd](images/image.axd)

İhtiyaca yönelik olarak doğru modeli seçerek ilerlemek önemli. Gerekli bilgiler sonrası oluşturma işlemi başlatılabilir. Redis Cache bir kaç dakika içinde kullanıma hazır hale gelecektir. Başlangıç için tek Node'dan oluşan 250 MB kapasiteli, SSL desteği veren ve 256 bağlantıya kadar çıkabileceğimiz bir Redis Cache hizmeti söz konusu.

![image.axd](images/image.axd)

Burası bir veri kaynağı olduğu için doğal olarak Connection String bilgisine de ihtiyacımız var. Show Access Keys kısmından bu bilgilere ulaşabiliriz.

![image.axd](images/image.axd)

Primary Connection string içerisindeki bilgi.Net Core tarafnda StackExchange.Redis paketi için ele alınabilir formattadır.

İstemci Uygulamanın Geliştirilmesi

Aslında portal tarafındaki hazırlıklarımız tamamlanmış durumda. Şimdi basit bir istemci geliştirerek Redis Cache ile konuşmaya çalışalım. Her zaman ki gibi bir Console uygulaması üzerinden ilerleyeceğiz. Öncesinde terminal üzerinden yapmamız gereken bir kaç hazırlık var. Console projesinin oluşturulması, Redis ile konuşmamızı sağlayacak StackExchange.Redis ve JSON serileştirme işlemlerini kolaylaştıracak Newtonsoft.json paketlerinin eklenmesi. Bunun için terminalden aşağıdaki komutları işletebiliriz.

```bash
dotnet new -o console TalkWithGondor
dotnet add package StackExchange.Redis
dotnet add package Newtonsoft.json
dotnet restore
```

TalkWithGondor isimli bir Console uygulaması oluşturduk. Kodlara gelince,

```csharp
using System;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace TalkWithGondor
{
    class Program
    {
        static void Main(string[] args)
        {
            string conStr = "gondor.redis.cache.windows.net,password=YWG04toTMb45VUTv7hcIcjbIQymuL7IaRp2Z7/5cNNU=,ssl=True,abortConnect=False";
            var connection = ConnectionMultiplexer.Connect(conStr);
            var db = connection.GetDatabase();
            var pingResponse = db.Execute("ECHO","Nabersin?");
            Console.WriteLine(pingResponse);

            db.StringSet("Motto", "Yağmurlu bir Nisan akşamıydı...");
            var mottoValue = db.StringGet("Motto");
            Console.WriteLine(mottoValue);

            Product box = new Product
            {
                Id = 10001,
                Title = "Lego head box",
                UnitPrice = 50
            };
            db.StringSet("LegoBox", JsonConvert.SerializeObject(box));
            Product productFromCache = JsonConvert.DeserializeObject<Product>(db.StringGet("LegoBox"));
            Console.WriteLine($"\t{productFromCache.Id}\t{productFromCache.Title}\t{productFromCache.UnitPrice}");
        }
    }

    class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public double UnitPrice { get; set; }
    }
}
```

Neler yaptık kısaca inceleyelim. Redis ile kolayca haberleşebilmek için StackExchange.Redis isim alanındaki tiplerden yararlanıyoruz. ConnectionMultiplexer sınıfını kullanarak bir bağlantı açıyoruz. Connect metoduna parametre olarak Azure portalından aldığımız bağlantı bilgisini koyduğumuza dikkat edelim. Sonrasında GetDatabase metodu ile db isimli bir değişken örneklenmekte. Bu değişken üzerinden çeşitli Execute denemeleri icra etmekteyiz. Redis'in klasik PING ve ECHO gibi selamlaşma fonksiyonları var. İlk Execute işleminde ECHO komutunu kullanarak bir mesaj gönderiyoruz. Redis gönderdiğimiz mesajı bize aynen geri yollamalı. Bir nevi bağlantımızı test ettiğimizi ifade edebiliriz.

İzleyen satırda StringSet ve StringGet kullanımlarına ait örnekler var. StringSet ile tahmin edeceğiniz üzere Redis üzerinde bir key:value çifti oluşturulmasını sağlıyoruz. Veri tipi metinsel içerikten oluşmakta. StringGet ile de Motto anahtar adıyla yolladığımız içeriğin değerini getiriyoruz. Kodun son kısmında ise işimize daha çok yarayacak bir örnek yer alıyor. Bir sınıfa ait nesne örneğini Redis üzerinde nasıl tutabileceğimizi görüyoruz.

Aslında anahtar nokta içeriği JSON formatında saklamaktan ibaret. Sonuçta hangi platform olursa olsun JSON genel bir veri formatı standardı sunuyor. Örnekte yer alan Product nesne örneğinin verisini de bu şekilde tutmamız mümkün. Tabii serileştirme ve ters serileştirme noktasında JsonConvert sınıfının SerializeObject ve DeserializeObject metodlarından yararlanmaktayız. Kodlarımız görüldüğü üzere son derece basit. Zaten basit olması da gerekiyor. Neden işleri karmaşıklaştıralım ki?(Yazar burada OverEngineering'cilere atıfta bulunuyor:P)

Uygulamayı çalıştırdığımızda aşağıdaki ekran görüntüsündekine benzer sonuçlar almamız lazım.

![image.axd](images/image.axd)

Görüldüğü gibi ECHO ile gönderdiğimiz mesaj bize aynen geri gönderildi. Ayrıca Motto mesajının da başarılı bir şekilde aktarıldığını görmekteyiz. Benzer durum LegoBox anahtar değeri ile tutulan Product nesne örneği için de geçerli. Neredeyse her türden veriyi Redis Cache üzerine almamız mümkün.

Çalışmalar devam ettikçe Redis Cache üzerindeki harketlilikler de artacaktır. Portal üzerindeki Monitoring sekmesini kullanarak çeşitli metrikleri inceleyebiliriz (Tahminimce Amazon Web Services'ler de olduğu gibi belli eşik değerlerine ulaşıldığında devreye girecek alarm mekanizmaları da kurulabiliyordur. Araştırmam lazım) Ben yaptığımız ilk bir kaç deneme sonrası aşağıdaki sonuçlarla karşılaştım.

![image.axd](images/image.axd)

Bağlantı sayıları, get ve set operasyon çağrıları, cache nesnelerinin durumları vs. Diğer kaynaklarda olduğu gibi oldukça geniş bir ölçümleme seti var. Biraz kurcalamak lazım. Bu adımları başarılı bir şekilde tamamladıysanız ve Redis Cache ile ilgili başka bir şey yapmayacaksanız size tavsiyem ilgili kaynak grubunu silmeniz olacaktır. Neme lazım arka planda unutulup da ilerleyen zamanlarda fiyatlandırma konusunda bize problem çıkartmasın değil mi?

![image.axd](images/image.axd)

Bu yazımızda Azure tarafından sunulan Redis Cache hizmetini nasıl kullanabileceğimize dair basit bir örnek yapmaya çalıştık. İstemci tarafında sadece.Net Core değil, Python, Node.Js, Java gibi diğer platformları da kullanabiliriz. Detaylı bilgi ve diğer öğretiler için [Microsoft'un resmi dokümanlarına](https://docs.microsoft.com/en-us/azure/redis-cache/cache-overview) bir bakmanızı öneririm. İşi daha da ileri götürmek için istemci uygulamanızı da Azure üzerinde host etmeyi deneyibilirsiniz. Pekala bu bir Web uygulaması ya da Web API hizmeti olabilir. Bu uygulamayı App Service olarak host edip Redis Cache'den yararlanmaya çalışabilirsiniz. Hatta dağıtık bir önbellekleme stratejisinin mimari seviyede nasıl ele alınması gerektiğine dair [şu adresteki pratiğe](https://docs.microsoft.com/en-us/azure/architecture/best-practices/caching?toc=%2Fazure%2Fredis-cache%2Ftoc.json) de bakabilirsiniz. Daha gerçekçi bir vaka çalışması olacağı kesin. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
