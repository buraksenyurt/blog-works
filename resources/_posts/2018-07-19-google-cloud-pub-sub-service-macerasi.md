---
layout: post
title: "Google Cloud Pub/Sub Service Macerası"
date: 2018-07-19 21:10:00 +0300
categories:
  - dotnet-core
  - gcp
tags:
  - google
  - google-cloud-platform
  - publisher-subscriber-model
  - machine-learning
  - image-processing
  - http
  - .net-core
  - golang
---
Yeni yuvam ile evimin arası 40 km. Uzaklık nedeniyle mesailerimiz erken başlıyor. Sabah 05:50de çalan alarmla güne başlıyorum. Üst baş, kişisel bakım, seyahat boyu bana eşlik edecek filtre kahveyi hazırlama vs derken 06:35 sıralarında sevgili servis şoförümüz İhsan ağabey ile buluşup yola devam ediyorum. Yaklaşık 40-45 dakikalık bir seyahatten sonra iş yerine ulaşıyorum. Yol boyunca "o saatte kim ayakta olur?" sorusunu cevaplarcasına her sabah onlarca kez ezen insanla karşılaşıyorum. Mevsime göre evlerin sarı beyaz oda ışıkları, seyir halindeki arabalar, çalışanları işe götüren servisler, otobüsler, minibüsler, duraklarda bekleyen öğrenciler... O vakitlerde empati yapmak farklı bir deneyim.

![surfing.gif](/assets/images/2018/surfing.gif)

Ama bazı sabahlarda Feedly sayfama düşen yazıları okuyorum. Şirkete ulaştığımda mesainin başladığı 07:45e kadar da neredeyse yarım saatlik serbest zamanım oluyor. Genelde beğendiğim ve Feedly listemde favorilere eklediğim bir yazının devamını o zaman diliminde getiriyorum. Bazı yazıları da tekrar tekrar okuyorum. İşte tam da böyle bir sabahtı Google'ın iş ortaklarından olan [Incentro](https://www.incentro.com/en/) firmasının (Internet sayfaları çok hoş) direktörü Kees van Bemmel'ın kaleminde çıkan yazıyı okuduğumda. Makale, Cloud Platform temelli olarak geliştirilen bir çözüm hakkındaydı.

Şirkete varır varmaz ilk işim yazının üstünden bir kere daha geçmek olmuştu. Sadece başlığında Publisher/Subscriber, Cloud Function, Machine Learning, Serverless kelimeleri geçiyordu. Bunlar ilgi çekici olması için fazlasıyla yeterliydi. Google'un bloğuna konu olan vakada, video, resim ve ses gibi içerikleri yönetmeye çalışan bir firmanın Google Cloud Platform ile uyguladığı Serverless çözüm anlatılıyordu. Sorun bu içeriklerin takı bazında kayıt altına alınması ve aramalarda doğru konumlandırılamamasıyla alakalıydı.

Şöyle düşünebiliriz; müşteri olarak elimizde tonlarca video, resim ve ses içeriği bulunuyor. Bunları tag sistemi ile teker teker kategorize etmeye çalışırken ne kadar doğru sonuçlar üretebiliyoruz? Kaçını yanlış takılarla, hatta takıları olmadan sisteme dahil ediyoruz. İşte söz konusu çözümde hem takı belirleme hem de arama işleri için gerekli içeriklerin Elasticsearch üzerinde indekslenmesinde Google Cloud Platform'un çeşitli hizmetlerinden nasıl yararlanıldığı anlatılıyordu. Biliyorum "ne bu? ne bu?" diyorsunuz. [Buradaki yazıyı](http://cloudplatform.googleblog.com/2018/01/how-we-built-a-serverless-digital-archive-with-machine-learning-APIs-Cloud-Pub-Sub-and-Cloud-Functions.html) okumanızı şiddetle tavsiye ederim.

> Şunu hayal edin...Diyelim ki aksiyon videoları/fotoğrafları çekenlerin olduğu bir internet arşivinin sahibisiniz. Müşterileriniz videolarını yükledikçe içeriklerindeki bir takım imgelere göre otomatik olarak takılarla işaretlenmelerini (mesela rüzgar sörfünü otomatik olarak algılayıp windsurfing takısı ile işaretlenmesi) ve hatta konuşmalarındaki metinsel ifadelere göre de ("alaçatı'nın rüzgarları sörf yapmak için idealmiş ahbap...") hangi sporlarla uğraştıklarını ve belki de konuşanın hangi ünlü olabileceğini kayıt altına almak istiyorsunuz. Hatta üyelerinizin çekip yükledikleri fotoğraflarda sizin tanımladığınız imgelerin otomatik olarak algılanıp takı bazında değerlendirilmesini istiyorsunuz vs

Ben yazıyı okuduktan sonra masamın başına geçtiğimde yaptığım ilk iş, mimari resmin bir benzerini çizmeye çalışmak oldu. Her zaman okuduğumu bakarak da olsa (az bakarak yapılanı kabul, hiç bakmadan tek seferde yapılanı makbuldur) çizmeye çalışırdım. Kendi notlarımı ekleyerek öğrenmeyi pekiştirmeye gayret ederdim. Sonuçta kurşun kalemle de olsa aşağıdaki gibi bir şeylere ulaştım.

![gcpps_1.gif](/assets/images/2018/gcpps_1.gif)

Kabaca olayı anlamış gibiydim. Eğer işlenmesini istediğim bir değer varsa (asset diyelim), bunu Google Cloud Storage'a atmam yeterliydi. Sonrasında Google'ın Handler fonksiyonları devreye girip bu değeri çeşidine göre işleyişin yürütüldüğü hattaki uygun enstrümana (yazının konusu olan Pub/Sub hizmetine) yönlendirecekti. Bu yönlendirme sırasında resim, video ve ses ile ilgili işleme fonksiyonları (Google Cloud Functions) devreye girecekti. Sonrası Elasticsearch'e atılan bilgilerden ibaretti. Benim ilgimi çeken Machine Learning, Speech to Text gibi akıllı hizmetlerin sunulduğu Google Cloud Functions alanıydı. Lakin daha önceden bir şekilde incelemiş olmama rağmen aradaki bir katmanı öğrenmeden ilerleyemeyeceğimi anlamıştım. Google Pub/Sub hizmeti.

Esasında bir resmin içerisindeki nesnelere göre Tensorflow'un bile araya girebildiği anlamlaştırma ya da bir video içerisindeki sesin Speech API ile metne dönüştürülüp konunun ne olduğunun çıkartılması ve tüm bunların EleasticSearch üzerine yazılması gibi fonksiyonellikler bir şekilde tetikleniyordu. Genellikle bir HTTP talebi bunun için yeterli ancak Publisher/Subscriber modeli de bu tetikleyicilerden birisiydi. İşte benim öncelikli olarak Google Cloud Platform üzerindeki Publisher/Subscriber hizmetini anlamam gerekiyordu.

Temel olarak bu hizmeti şu şekilde ifade edebiliriz; Uygulamalar arasında güvenilir şekilde hızlı ve asenkron olarak mesaj değiş tokuşuna izin veren bir Google hizmeti olarak tanımlasak yanlış olmaz. Sistem klasik Publisher/Subscriber modelini baz alır. Publisher rolünü üstlenen taraf belli bir konu (topic) için mesaj yayımlar. Subscriber rolünü üstlenen taraf eğer ilgili konuya (topic) abone olmuşsa yayıncının mesajını istediği zaman çekebilir. Google'ın bu hizmetindeki mesajlar alıcıya ulaşana kadar belli bir süre boyunca (ben araştırırken 7 gündü) korunmaktadır.

Bu teorik bilgiyi pekiştirmek ve özellikle bunu West-World gibi Ubuntu tabanlı bir dünyada,.Net Core,Go,Ruby, Python, Node.js gibi dilleri kullanarak deneyimlemek benim için önemliydi. Ana amacım Google Cloud Platform'da Pub/Sub servisini belli bir proje için etkinleştirmek ve sonrasında.Net Core tarafında belli bir topic için mesaj yayınlayıp bu mesajı okuyabilmek. Zaten Google Cloud Platform açısından amaç da bu. Uygulamalar arasında güvenilir bir hat üzerinden mesaj akışına izin veren yüksek performanslı tamamen asenkron çalışan bir boru hattı (pipeline) Öyleyse gelin adım adım ilerleyerek konuyu anlamaya çalışalım.

gCloud ile İlk Deneyim

Google bu konu ile ilişkili oldukça zengin ve basit öğreti dökümanları sunmakta (Diğer bulut bilişim sistemlerinde olduğu gibi) Bende ilgili dokümanları takip ettim ve ilk olarak gCloud aracını kullanarak var olan bir Google projemde Publisher/Subscriber modelini deneyimlemeye çalıştım. my-starwars-game-project isimli projemi seçtikten sonra Google Console-> API sekmesinden Enable APIS and Services linkine tıklayarak ilerledim. Big Data kısmında yer alan Google Cloud Pub/Sub API hizmetini seçip

![gcpsp_2n.gif](/assets/images/2018/gcpsp_2n.gif)

Enable yazan düğmeye bastım. Böylece Google Cloud Platform üzerinde yer alan bir projem için Pub/Sub API hizmetini etkinleştirmiş oldum.

> GCP üzerinde, doğal dil işleme hizmetinden (Google Cloud Natural Language API) çeviri servisine (Google Cloud Translation API), tahminlemeden (Prediction API) makine öğrenimine (Google Machine Learning Engine) kadar farklı farklı kategorilerde bir çok API olduğunu ve proje bazlı kullanılabildiğini ifade edebilirim. Tabii bunları deneyimlemeden önce mutlaka ücret politikasını incelemekte yarar var.

Bundan sonra iş West-World terminalindeydi. gCloud komutunu kullanarak (daha önceden West-World'e kurmuştum) bir topic oluşturmayı, bu topic'e abone olup mesaj göndermeyi ve diğer bir abone ile de bu mesajı okumayı denedim. İşte West-World'ün bu denemeler sonrası görünümü.

![gcpps_6.gif](/assets/images/2018/gcpps_6.gif)

Öncelikle şunu belirtmem lazım; işe gcloud init komutu ile başlamakta yarar olabilir. Nitekim projeniz için makinedeki ayarların tekrardan yapılması gerekebilir. Ekran görüntüsünden de görüleceği üzere pubsub uygulamasına ait komutları kullanarak string bir mesajı codeTopic isimli bir konu başlığı altında yayınlıyor ve tekrardan okuyoruz. Kullanılan komutlara kısaca bakacak olursak şunları söyleyebiliriz;

codeTopic isimli bir konu başlığı oluşturmak için şu komutu kullanıyoruz,

```bash
gcloud pubsub topics create codeTopic
```

Ben oluşturulan topikleri nasıl görebileceğimizi de merak ettiğimden şöyle bir komut buldum.

```bash
gcloud pubsub topics list
```

Tabii bu konu başlığına mesaj atmak veya okumak için öncelikle abone olunması gerekiyor. westworldSubscription isimli bir aboneliği aşağıdaki komutla oluşturmak mümkün.

```bash
gcloud pubsub subscriptions create --topic codeTopic westworldSubscription
```

Eğer abonelerin bir listesini görmek istersek de şu komut işimize yarayacaktır.

```bash
gcloud pubsub subscriptions list
```

Bir topic ve bir abone var. Bu durumda uzaya doğru bir mesaj fırlatılabilir. Söz gelimi codeTopic başlığı altında "convert this message to C#" şeklinde bir mesaj gönderebiliriz.

```bash
gcloud pubsub topics publish codeTopic --message "convert this message to c#"
```

Peki gönderdiğimiz bu mesajı nasıl çekeceğiz? Bunun aslında iki yolu var. Birisi Push diğeri ise Pull olarak geçiyor. Push modelinde google cloud platform tarafının abone olan tarafa mesaj göndermesi gibi bir durum söz konusu. Pull modelinde ise abonenin kendisi gidip mesajı alıyor. Aşağıdaki komut pull modeline göre çalışmakta ([Şu adreste](https://cloud.google.com/pubsub/docs/subscriber#push_pull) Push ve Pull metodlarının çalışması ve hangi durumda hangisinin tercih edilmesi gerektiğine dair bir takım bilgiler bulunmakta)

```bash
gcloud pubsub subscriptions pull --auto-ack westworldSubscription
```

Ben mesajı gönderip almayı başardıktan sonra ilgili konu başlığı ve aboneliği nasıl sileceğimi de öğrendim. Şu komutlarla codeTopic ve buna abone olan westworldSubscription'ı silebiliyoruz. Elbette bu tip oluşturma ve silme işlemlerini Google Cloud Platform arabirimi üzerinden de yapabiliriz.

```bash
gcloud pubsub topics delete codeTopic
gcloud pubsub subscriptions delete westworldSubscription
```

.Net Core Tarafı

Gelelim kod tarafına. Komut satırından çalışırken West-World üzerinden gCloud aracılığıyla topic oluşturabileceğimi, bu topic için bir abonelik kullanabileceğimi ve mesaj gönderip okuyabileceğimi öğrenmiştim. Pek tabii bunu bir program kodu ile nasıl yapabileceğimi de keşfetmem gerekiyordu. İlk olarak West-World'ün en sevilen sakinlerinden olan Visual Studio Code'un kapısını çaldım. Ondan bana basit bir Console projesi açmasını istedim.

```bash
dotnet new console -o gcppubsubhello
```

Sonrasındaysa işimi kolaylaştıracak olan Google.Cloud.PubSub kütüphanesinin yazıyı hazırladığım tarihte önerilen sürümünü projeye ekledim.

```bash
dotnet add package Google.Cloud.PubSub.V1 --version 1.0.0-beta16
```

Artık paketi kullanarak Pub/Sub API ile konuşmaya başlayabilirdim. İlk olarak bir topic oluşturmayı ve bu topic için mesajlar yayınlamayı denedim.

```csharp
using System;
using System.Collections.Generic;
using Google.Cloud.PubSub.V1;

namespace gcppubsubhello
{
    class Program
    {
        static void Main(string[] args)
        {
            var projectId = "subtle-seer-193315";
            var topicId = "codeTopic";

            PublisherServiceApiClient psClient = PublisherServiceApiClient.Create();

            TopicName topicName = new TopicName(projectId, topicId);
            psClient.CreateTopic(topicName);
            IEnumerable<Topic> topics = psClient.ListTopics(new ProjectName(projectId));
            foreach (Topic t in topics)
                Console.WriteLine($"{t.Name}");

            PublisherClient publisher = PublisherClient.Create(topicName, new[] { psClient });
            var result = publisher.PublishAsync("Convert.ToQBit function");
            Console.WriteLine(result.Result);

            result = publisher.PublishAsync("GetFactorial function");
            Console.WriteLine(result.Result);           
        }
    }
}
```

İşin başında PublisherServiceApiClient türünden bir nesne oluşturmak gerekiyor. Bunu Create metodu ile sağlıyoruz. Sonrasında TopicName türünden bir örnek oluşturuluyor. İlk parametre GCPdeki projenin ID değeri, diğeri ise topic için verilecek string bir bilgi (Topic ID) CreateTopic fonksiyonu kullanılarak ilgili Topic'in Google tarafında oluşturulması sağlanıyor. Ki örneği ilk çalıştırdığımda bunu görebildim.

![gcpps_7.gif](/assets/images/2018/gcpps_7.gif)

ListTopics metodu ile var olan tüm topic bilgilerini elde edebiliriz. Bende bunu denemek istedim. Mesaj yayınlamak içinse bir PublisherClient örneğine ihtiyaç var. Bunu oluştururken ilk parametre ile topic nesnesini, ikinci parametre ile de PublisherServiceApiClient örneğini veriyoruz. Böylece hangi Google projesinin hangi konusuna abone olacağımızı bildirmiş oluyoruz. Sonrası oldukça kolay. PublishAsync fonksiyonunu kullanarak bir konu başlığına mesaj bırakılıyor. Ben örnek olarak iki tane string içerik gönderdim. Sonuç olarak elde edilen bilgiler ise bu mesajlar için üretilen AcknowledgeID değerleridir. Topic altına bırakılan her mesajın (sonradan aynı içeriğe sahip mesajlar tekrar geldiğinde farklı olacak şekilde) birer ackID değeri bulunur. Kodu arka arkaya çalıştırdığımda aşağıdaki sonuçları elde ettim.

![gcpps_8.gif](/assets/images/2018/gcpps_8.gif)

İlk çalıştırma normal sonuçlansa da ikinci çalıştırmada bir exception almıştım. Aslında hata oldukça basitti.

```text
Unhandled Exception: Grpc.Core.RpcException: Status(StatusCode=AlreadyExists, Detail="Resource already exists in the project (resource=codeTopic).")
```

Zaten codeTopic isimli bir Topic vardı. Tekrar yaratmaya çalışınca bir çalışma zamanı istisnası oluştu. Bu gibi durumları engellemek için topic nesnesinin var olup olmadığını kontrol etmekte ve yoksa oluşturmaya çalışmakta yarar var. Silme işlemi için DeleteTopic fonksiyonu kullanılabilir. Oluşturulma adımıysa try...catch...when yapısı ile daha güvenli hale getirilebilir. Ben bu kadarlık ipucu vereyim. Gerisini siz deneyin;)

Topic oluşturulması ve mesaj yayınlandığını görmek benim için yeterliydi. Sıradaki adım codeTopic konusuna atılan mesajları okumaktı. İlk olarak bir abone oluşturmak gerekiyor.

```csharp
var projectId = "subtle-seer-193315";
var topicId = "codeTopic";

TopicName topicName = new TopicName(projectId, topicId);
SubscriberServiceApiClient subsClient = SubscriberServiceApiClient.Create();
SubscriptionName subsName = new SubscriptionName(projectId, "einstein");
subsClient.CreateSubscription(subsName, topicName, pushConfig: null, ackDeadlineSeconds: 120);
```

Abone üretme işi bu kez SubscriberServiceApiClient nesnesinde. Create metodu ile bu nesne örneklendikten sonra CreateSubscription fonksiyonu ile de aboneyi oluşturmaktayız. Abonemiz einstein isimli bir ID değerine sahip, Push değil de Pull modelini kullanan bir abone. Kodu ilk çalıştırdığımda abonenin my-starwars-game-project için başarılı bir şekilde oluşturulduğunu gördüm.

![gcpps_9.gif](/assets/images/2018/gcpps_9.gif)

Pek tabii kodu ikince kez denediğimde zaten var olan bir aboneyi tekrar oluşturmaya çalıştığım için exception almam gayet normaldi.

```text
Grpc.Core.RpcException: Status(StatusCode=AlreadyExists, Detail="Resource already exists in the project (resource=einstein).")
```

Çözüm olarak abonenin zaten var olup olmadığı kontrol edilebilir. Aynen Topic oluşturma vakasında olduğu gibi (Bunu benim için denersiniz değil mi?:))

Bir abonem olduğuna göre onu kullanarak codeTopic üzerine bırakılan mesajları okumayı deneyebilirdim. İşte kodlar.

```csharp
using System;
using System.Linq;
using System.Text;
using System.Threading;
using Google.Cloud.PubSub.V1;

namespace gcppubsubhello
{
    class Program
    {
        static void Main(string[] args)
        {
            var projectId = "subtle-seer-193315";

            SubscriberServiceApiClient subsClient = SubscriberServiceApiClient.Create();
            SubscriptionName subsName = new SubscriptionName(projectId, "einstein");
            SubscriberClient einstein = SubscriberClient.Create(subsName, new[] { subsClient });
            bool acknowledge = false;
            einstein.StartAsync(
                async (PubsubMessage pubSubMessage, CancellationToken cancel) =>
                {
                    string msg = Encoding.UTF8.GetString(pubSubMessage.Data.ToArray());
                    await Console.Out.WriteLineAsync($"{pubSubMessage.MessageId}: {msg}");
                    return acknowledge ? SubscriberClient.Reply.Ack : SubscriberClient.Reply.Nack;
                });
            Thread.Sleep(5000);
            einstein.StopAsync(CancellationToken.None).Wait();
        }
    }
}
```

Bütün iş einstein isimli nesnede bitiyor. StartAync metodu içerisinde, abonenin daha önceki kod parçasında oluşturulurken abone olduğu Topic üstüne atılan mesajlar alınıyor. Ne kadar mesaj varsa gelecektir. Eğer mesaj başarılı bir şekilde alınabilmişse bu Reply.Ack ile ifade edilir (Message handled successfully) Aksi durumda Reply.Nack olur (Message not handled successfully)

![gcpps_10.gif](/assets/images/2018/gcpps_10.gif)

Görüldüğü gibi.Net Core tarafında uygun kütüphaneleri kullanarak Pub/Sub API ile konuşmak oldukça basit. Elbette yapılabilecek bir çok şey var. Söz gelimi bu örnekte.Net Core uygulaması Google hizmetini kullanırken hiçbir credential bilgisi kullanmadık. Nitekim West-World'e çok önceden

```bash
export GOOGLE_APPLICATION_CREDENTIALS="my-starwars-game-project-d977a50a19f5.json"
```

şeklinde bir terminal komutu ile gerekli credential bilgilerini işlemiştim. Eğer yazdığımız ürünü bir sunucuya atacak ve oradan Google Pub/Sub hizmetini kullandırmak isteyeceksek bu tip Credential bilgilerini de kod tarafında yüklememiz gerekebilir. Bunun nasıl yapılabileceği ile ilgili olarak Google'ın [şu adresindeki yazıya](https://cloud.google.com/docs/authentication/production) bakabilirsiniz.

Benim için sıradaki aşama bir Google fonksiyonunu Pub/Sub API üzerinden tetikletmek. Yani yazının başında bahsettiğim vakadaki çalışmanın minik bir hattını canlandırmaya çalışmak. Bakalım yolda karşıma öğrenmem gereken daha neler neler çıkacak. Siz buradaki kullanım şekillerini geliştirerek ilerlemeye devam edebilirsiniz. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
