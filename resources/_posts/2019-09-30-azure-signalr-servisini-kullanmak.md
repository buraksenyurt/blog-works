---
layout: post
title: "Azure SignalR Servisini Kullanmak"
date: 2019-09-30 13:00:00 +0300
categories:
  - azure
tags:
  - azure
  - bash
  - csharp
  - javascript
  - json
  - dotnet
  - http
  - nodejs
  - async-await
  - concurrency
  - visual-studio
  - github
---
Basketbolu neden bu kadar çok seviyorum diye düşündüm geçenlerde. Oturduğumuz sitenin basket sahasını futbol oynamak için kullanan onca çocuk ve genç gibi bir gerçek varken ben neden bu spora böylesine sevdalıydım. İnanılmaz enerjisi ve sürekli değiştirdiği NBA şapkaları ile rahmetli İsmet Badem mi sevdirmişti? Yoksa final serisi maçları sabahın kaçında olursa olsun uyanamayıp okula geç gitmeme neden olan majestelerinin maçları mı? Basketbolun tüm efsanelerini kendi kardeşiymiş gibi tanıyan ve maçları kendine has heyecanı ile anlatan Murat Murathanoğlu muydu yoksa?

![emcey.png](/assets/images/2019/emcey.png)

Belki de Koraç kupasını alarak Avrupa'da bir ilke imza atan Efes'in Abdi İpekçi salonundaki Stefanel Milano maçına girmek için kuyrukta beklerken arabasından bizi seyreden yaşıtım Mirsad Türkcan'ın onca seyirciyi coşkuyla selamlamasıydı. Kim bilir belki de hücum süresi henüz otuz saniyeyken Peter Naumovski'nin eliyle tshirt'ünün sağ yakasını ağzına götürerek verdiği setin adıydı. Belki de zamanında her gün büyük bir iç motivasyonla gittiğim turuncu bankanın CBL[(corporate basketball league)](http://www.cbl.com.tr/) seçmelerinde koçun bana gelip "abi kusura bakma" dedikten sonra yaşımı öğrenip "sen ciddi misin abi? Ben bu kadar büyük olduğunu bilmiyordum. Çok daha genç duruyorsun. Basketbol sevgine hayran kaldım" söylemine rağmen takıma almayışı ve Bill Murray'ın Space Jam'de Larry Bird ile olan konuşmasında ona "You can't play" demesini hatırlayışım mıydı? İnanın hiç bilmiyorum. Ama çok sevip de hiç bir zaman beceremediğim bu oyunu mesleki çalışmalarımda kullanmaya bayılıyorum. İşte öyle bir çalışmanın girizgahındasın şu anda sevgili okur:)

O [cumartesi gecesi çalışması](https://github.com/buraksenyurt/saturday-night-works)ndaki amacım Azure platformundaki SignalR hizmetini kullanarak abone programlara çeşitli tipte bildirimlerde bulunabilmekti. Normal SignalR senaryosundan farklı olarak istemciler ve tetikleyici arasındaki eş zamanlı iletişimi (Real Time Communications) Azure platformundaki bir SignalR servisi ile gerçekleştirmek istemiştim. Senaryoda bildirimleri gören en az bir istemci (ki n tane olması daha anlamlı), local ortamda çalışan ve bildirim yayan bir Azure Function uygulaması ve Azure platformunda konuşlandırılan bir SignalR servisi olmasını planlamıştım. Ayrıca Azure üzerinde koşan bu SignalR servisini Serverless modda çalışacak şekilde ayarlamayı planlıyordum. Bir takım sonuçlara ulaşmayı başardım. Şimdi çalışmaya ait notları derleme zamanı. Öyleyse ne duruyoruz. Haydi başlayalım.

SignalR servisi tüm Azure fonskiyonları ile kullanılabilir. Örneğin Azure Cosmos DB'deki değişiklikleri SignalR servisi ile istemcilere yollayabiliriz. Benzer şeyi kuyruk mesajlarını veya HTTP taleplerini işleyen Azure fonksiyonları için de sağlayabiliriz. Kısacası Azure fonksiyonlarından yapılan tetiklemeler sonrasında SignalR servislerinden yararlanarak bağlı olan aboneleri bilgilendirebiliriz. Şimdi WestWorld'ün gereksinimlerini tamamlayaraktan örneğimizi geliştirmeye başlayalım.

## Ön Gereksinimler

Azure platformunda SignalR servisini oluşturmadan önce WestWorld (Ubuntu 18.04, 64bit) tarafında Azure Function geliştirebilmek için gerekli kurulumları yapmam gerekiyordu. İlk olarak [Azure Functions Core Tools](https://github.com/Azure/azure-functions-core-tools)'un yüklenmesi lazım. Aşağıdaki terminal komutları ile bunu gerçekleştirmek mümkün. Önce Microsoft ürün anahtarını Ubuntu ortamına kaydediyor ve sonrasında bir güncelleme yapıp devamında azure-functions-core-tools paketini yüklüyoruz.

```bash
curl https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.gpg
sudo mv microsoft.gpg /etc/apt/trusted.gpg.d/microsoft.gpg

sudo apt-get update

sudo apt-get install azure-functions-core-tools
```

Kurulumdan sonra terminalden Azure Function projeleri oluşturmaya başlanabilir. Lakin bu işin Visual Studio Code tarafında daha kolay bir yolu var. O da [Azure Functions isimli aracı](https://code.visualstudio.com/docs/azure/extensions) kullanmak.

![09_34_credit_1.png](/assets/images/2019/09_34_credit_1.png)

Visual Studio Code'a gelen bu araçla kolayca Azure Function projeleri oluşturabiliriz.

## Azure SignalR Servisinin Hazırlanması

Adım adım ilerlemeye çalışalım. Öncelikle Azure platformunda bir SignalR servisi oluşturmamız gerekiyor. Ben [Azure Portal adresinden](https://portal.azure.com) SignalR Service öğesini aratarak işe başladım. Sonrasında aşağıdaki ekran görüntüsünde yer alan bilgiler ile servisi oluşturdum.

![09_34_credit_2.png](/assets/images/2019/09_34_credit_2.png)

Free Tier planında, learning-rg Resource Group altında, basketcini.service.signalr.net isimli bir SignalR servisimiz var. Bu servisinin oluşması biraz zaman alabilir ki ben bir süre beklediğimi hatırlıyorum. Servis etkinleştikten sonra özelliklerine giderek Serverless modda çalışacak şekilde ayarlayabiliriz. Bunun için Service Mode özelliğini Serverless'a çekmek yeterli. Tabii ekran görüntüsünden de fark edeceğiniz üzere PREVIEW modunda. Kuvvetle muhtemel sizin denemelerinizi yapacağınız durumda son halini almış olabilir.

![09_34_credit_3.png](/assets/images/2019/09_34_credit_3.png)

Bu SignalR servisi ile local makinede çalışacak ve tetikleyici görevini üstlenecek Azure Function uygulamasının haberleşebilmesi için, Key değerlerine ihtiyacımız olacak. Bu değerleri Azure Function uygulamasının local.settings.json dosyasında kullanmamız gerekiyor. O nedenle aşağıdaki ekran görüntüsündeki gibi ilgili değerleri kopyalayıp güvenli bir yerlerde saklayın.

![09_34_credit_4.png](/assets/images/2019/09_34_credit_4.png)

## Azure Functions Projesinin Oluşturulması

Yüklenen Azure Functions aracından Create New Project seçimini yaparak ilerleyebiliriz. Proje için bir klasör belirleyip (Ben NotifierApp isimli klasörü kullandım) dil olarak C#'ı tercih ederek devam edelim. Sonrasında Create Function seçeneği ile projeye Scorer isimli bir fonksiyon ekleyelim. Ben bu işlem sırasında sorulan sorulara aşağıdaki cevapları verdim. Siz kendi projenize özgün hareket ederseniz daha iyi olabilir. Özetle HTTP metodları ile tetiklenen bir fonksiyon söz konusu diyebiliriz.

```bash
Fonksiyon Adı : Scorer
Klasör : NotifierApp
Tipi : Http Trigger
Namespace : Basketcini.Function
Erişim Yetkisi : Anonymous
```

> Örnekte Table Storage seçeneği değerlendirilmiştir. Bunun için öncelikle Azure Portal üzerinde learningsignalrstorage isimli bir Storage Account oluşturdum ve Access Keys kısmında verilen Connection Strings bilgisini kullandım. Yani bildirimlerin depolanacağı Storage alanını sevgili Azure'a devrettim. Çünkü WestWorld'ün disk kapasitesi epeyce azalmış durumdaydı:P

### Azure Functions Projesinde Yapılanlar

Azure fonksiyonu oluşturulduktan sonra elbette biraz kodlama yapmamız gerekecek. Ama öncesinde bizim için gerekli nuget paketlerini yüklemeliyiz. Aşağıdaki terminal komutlarını NotifierApp klasöründe çalıştırarak devam edelim.

```bash
dotnet add package Microsoft.Azure.WebJobs.Extensions.EventGrid 
dotnet add package Microsoft.Azure.WebJobs.Extensions.SignalRService 
dotnet add package Microsoft.Azure.WebJobs.Extensions.Storage
```

Önemli değişikliklerden birisi local.settings.json dosyasında yer alıyor. Burada Azure SignalR servisine ait Connection String bilgisi ve CORS tanımı (Senaryoya göre isimsiz tüm istemciler Azure Function Api'sini kullanabilecek) eklemek lazım. Nasıl yapıldığını söylemek isterdim ama gitignore dosyasında bu json içeriğini dışarıda bırakmışım. Yani hatırlamıyorum:) Yani sizin keşfetmeniz gerekecek;)

Bunun haricinde skor durumunu ve anlık olarak meydana gelen olay bilgisini tutan Timeline ve Action isimli sınıfları da aşağıdaki gibi kodlayabiliriz. Biliyorum henüz senaryo tam olarak şekillenmiş değil. Ama çalışma zamanına geldiğimizde ne olduğunu gayet iyi anlayacaksınız. Action sınıfı ile başlayalım.

```csharp
namespace Basketcini.Function
{
    /*
        Table Storage'e yazılacak veri içeriğini temsil eden sınıftır.
        Azure Table Storage'a aşağıdaki özellikler birer alan olarak açılacaktır.
     */
    public class Action
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Player { get; set; }
        public string Summary { get; set; }
    }
}
```

ve Timeline sınıfımız;

```csharp
namespace Basketcini.Function
{
    /*
        Abonelere döndürülecek veri içeriğini taşıyacan temsili sınıftır.
        Kim, hangi olayı gerçekleştirdi bilgisini tutar.
    */
    public class Timeline
    {
        public string Who { get; set; }
        public string WhatHappend { get; set; }
    }
}
```

Scorer isimli Function sınıfında da üç metod bulunuyor. Birisi tetikleyici olarak yeni bir olay gerçekleştirmek için, birisi istemcinin kendisini SignalR Hub'ına bağlaması için (negotiation aşaması), birisi de servisin istemciye olay bildirimlerini basması için (push message aşaması) Her zaman ki gibi kod içerisindeki yorum satırlarında anladıklarımı basitçe anlatmaya çalıştım.

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Basketcini.Function
{
    public static class Scorer
    {
        /*
        Scorer fonskiyonu HTTP Post tipinden tetiklemeleri karşılar.
        Oluşan aksiyonları saklamak için Table Storage kullanılır. Actions isimli tablo Table niteliği ile bildirilmiştir.
        Ayrıca gerçekleşen olaylar bir kuyruğa atılır(Queue niteliğinin olduğu kısım)
        Console'a log yazdırmak için ILogger türevli log değişkeni kullanılır.
        */
        [FunctionName("Scorer")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] Timeline timelineEvent,
            [Table("Actions")]IAsyncCollector<Action> actions,
            [Queue("new-action-notification")]IAsyncCollector<Timeline> actionNotifications,
            ILogger log)
        {
            log.LogInformation("HTTP tetikleme gerçekleşti");
            log.LogInformation($"{timelineEvent.Who} için {timelineEvent.WhatHappend} olayı");

            /* HTTP Post metodu ile gelen timeline bilgilerini de kullanarak bir Action nesnesi 
            oluşturuyor ve bunu Table Storage'e atıyoruz.
            Amaç, meydana gelen olaylarla ilgili gelen bilgileri bir tabloda kalıcı olarak saklamak.
            Pek tabii bunun yerine farklı repository'ler de tercih edilebilir. Cosmos Db gibi örneğin.
            */
            await actions.AddAsync(new Action
            {
                PartitionKey = "US",
                RowKey = Guid.NewGuid().ToString(),
                Player = timelineEvent.Who,
                Summary = timelineEvent.WhatHappend
            });

            /* 
                new-action-notification ile ilintili olan kuyruğa gerçekleşen olay bilgilerini atıyoruz.
                İstemci tarafını bu kuyruk içeriği ile besleyebiliriz.
            */
            await actionNotifications.AddAsync(timelineEvent);

            return new OkResult();
        }

        /*
        Azure SignalR servisine bağlanmak için kullanılan metodumuz. 
        HTTP Post ile tetiklenir.
        Fonksiyon bir SignalRConnectionInfo nesnesini döndürür.
        Bu nesne Azure SignalR'a bağlanırken gerekli benzersiz id ve access token bilgisini içerir.
        SignalR Hub-Name olarak notifications ismi kullanılır.
         */
        [FunctionName("negotiate")]
        public static SignalRConnectionInfo GetNotificationSignal(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequest request,
            [SignalRConnectionInfo(HubName = "notifications")]SignalRConnectionInfo connection,
            ILogger log
        )
        {
            log.LogInformation("Negotiating...");
            return connection;
        }

        /*
        Abone olan tarafa veri göndermek (push) için kullanılan fonksiyondur.
        QueueTrigger niteliğindeki isimlendirme ve tipin Scorer fonksiyonundaki ile aynı olduğuna dikkat edelim.
        İstemciye mesaj taşıyan nesne bir SignalRMessage örneğidir. 
        Bu nesnenin Arguments özelliğinde timeline içeriği (yani gerçekleşen maç olayları) taşınır.
        Peki aboneler buradaki olayları nasıl dinleyecek dersiniz? Bunun içinde Target özelliğine atanan içerik önem kazanır. 
        Örneğimizide aboneler 'actionHappend' isimli olayı dinleyerek mesajları yakalayacaktır.
         */
        [FunctionName("PushTimelineNotification")]
        public static async Task PushNofitication(
            [QueueTrigger("new-action-notification")]Timeline timeline,
            [SignalR(HubName = "notifications")]IAsyncCollector<SignalRMessage> message,
            ILogger log
        )
        {
            log.LogInformation($"{timeline.Who} için gerçekleşen olay bildirimi");

            await message.AddAsync(
                new SignalRMessage
                {
                    Target = "actionHappend",
                    Arguments = new[] { timeline }
                }
            );
        }
    }
}
```

## İstemci Uygulama Tarafı

İstemci tarafı Node.js tabanlı basit bir Console uygulaması. Aslında web tabanlı bir arayüzü takip etmem gerekiyordu ancak amacım kısa yoldan SignalR servisinden akan verileri görmek olduğundan Node.js kullanmayı tercih ettim. Siz istemci tarafında tamamen özgünsünüz. SignalR tarafı ile rahat konuşabilmek için @aspnet/signalr isimli npm paketini kullanabiliriz. Terminalden aşağıdaki komutları kullanarak kobay istemcimizi oluşturalım.

```bash
mkdir FollowerApp
cd FollowerApp
npm init
touch index.js
npm install @aspnet/signalr
```

İstemci tarafında index.js ve package.json dosyalarını kodlayacağız. Aşağıda index sınıfına ait kod içeriğini bulabilirsiniz. Uygulama Hub'a bağlandıktan sonra bildirimleri dinler modda yaşamını sürdürecek diyebiliriz.

```javascript
const signalR = require("@aspnet/signalr"); // signalR istemci modülünü bildirdik

/* 
    Hub bağlantı bilgisini inşa ediyoruz.
    withUrl parametresi Azure Function uygulamasının yayın yaptığı adrestir
*/
const connection = new signalR.HubConnectionBuilder()
    .withUrl('http://localhost:4503/api')
    .build();

console.log('Bağlantı sağlanıyor...');

/*
    Bağlantıyı başlatıyoruz. Başarılı ise then metodunun içeriği,
    bir hata oluşursa da catch metodunun içeriği çalışır.
*/
connection.start()
    .then(() => console.log('Bağlantı sağlandı...'))
    .catch(console.error);

/*
    actionHappend olayını dinlemeye başladık.
    Eğer SignalR servisi üzerinden bir push mesajı söz konusu olursa
    bu olay üzerinden geçeceği için istemci tarafından yakalanıp
    doSomething metodu çağırılacaktır.
    doSomething'e gelen parametre Azure Function'daki
    PushTimelineNotification fonksiyonundan dönen mesajın Arguments içeriğini taşır.

*/
connection.on("actionHappend", doSomething);

function doSomething(action) {
    console.log(action);
}

connection.onclose(() => console.log('Bağlantı koparılıyor...'));
```

ve package.json

```json
{
  "name": "followerapp",
  "version": "1.0.0",
  "description": "",
  "main": "index.js",
  "scripts": {
    "test": "echo \"Error: no test specified\" && exit 1",
    "dev": "node index.js"
  },
  "author": "",
  "license": "ISC",
  "dependencies": {
    "@aspnet/signalr": "^1.1.2"
  }
}
```

## Çalışma Zamanı (NotifierApp Uygulaması)

Bu adam neler anlattı, neler yazdı diyor gibisiniz biliyorum. O nedenle çalışma zamanına geçmeden önce senaryodan bahsetmem çok doğru olacaktır. WestWorld üzerinde NotifierApp isimli Azure Function uygulaması ayağa kalkar. Bu, Azure SignalR servisi ile haberleşen programımız. Postman ile hayali olarak o anda oynanan bir basketbol maçından çeşitli bilgiler göndereceğiz. Sayı oldu, blok yapıldı vs gibi. Bu bilgiler Azure tarafındaki SignalR servisimiz tarafından karşılanacak ve Table Storage üstünde kuyruğa yazılacak. Yine WestWorld üzerinde çalışan bir başka uygulama (Etkili bir görsellik için bir web sayfası ya da konuyu anlamak için bir console uygulaması olabilir) Local ortamda çalışan Azure Function servisine bağlanıp actionHappend olaylarını dinleyecek. Postman üzerinden maça ait bir basketbol olayı gönderildikçe bu bilgilerin tamamının yer aldığı kuyruk içeriği abone olan istemcilere otomatik olarak dağıtılacak. Sonuçta canlı bir maçın gerçekleşen anlık olayları bu haber kanalını dinleyen istemcilerine eş zamanlı olarak basılmış olacak (en azından senaryonun bu şekilde çalışmasını bekliyoruz)

Yazılan Azure Function uygulamasını çalıştırmak için terminalden aşağıdaki komutu vermek yeterli. Tabii bu komutu Azure Function projesinin olduğu klasörde icra etmeliyiz;)

```bash
func host start
```

![09_34_credit_5.png](/assets/images/2019/09_34_credit_5.png)

Function uygulamamız şu anda local ortamda çalışır durumda olmalı ve Azure SignalR ile haberleşmesi gerekli. En azından WestWorld üzerinde bu şekilde işledi. Şimdi Postman aracını kullanarak api/Scorer adresine bir HTTP Post talebi gönderebiliriz. Örneğin aşağıdaki gibi.

```text
Url : http://localhost:4503/api/Scorer
Method : HTTP Post
Body : {
"Who":"Mitsiç",
"WhatHappend":"3 sayılık basket. Skor 33-21 Anadolu Efes önde"
}
```

![09_34_credit_6.png](/assets/images/2019/09_34_credit_6.png)

Bir şeyleri doğru yazmış olmalıyım ki log mesajlarında istediğim hareketliliği gördüm. Hatta Azure Storage tarafında bir tablonun oluşturulduğunu ve gönderdiğim bilginin içerisine yazıldığını da fark ettim (Tekrar eden bilgileri nasıl normalize etmek gerekir bunun yolunu bulmak lazım) Şu aşamaya gelen okurlarım, umarım sizler de benzer sonuçları görmüşsünüzdür.

![09_34_credit_7.png](/assets/images/2019/09_34_credit_7.png)

## Çalışma Zamanı (İstemci/Abone olan taraf)

Bildirim yapmayı başardık. Bildirimlerin kuyruğa gittiğini de gördük. Peki ya abonelerden ne haber? Senaryonun tam işlerliğini görmek için her iki uygulamayı da birlikte çalıştırmak lazım elbette. Node.js tabanlı FollowerApp için terminalden aşağıdaki komutu vermek yeterli.

```bash
npm run dev
```

İlk ekran görüntüsü istemci ile Azure SignalR servisinin, Azure Function uygulaması aracılığıyla el sıkışmasını gösteriyor.

![09_34_credit_8.png](/assets/images/2019/09_34_credit_8.png)

Alt ekran görüntüsünde dikkat edileceği üzere Negotiation başarıyla sağlandıktan sonra bir id ve token bilgisinin üretildiği görülmekte. Buradaki çıktı, Azure Function uygulamasındaki negotiate sonrası döndürdüğümüz connection bilgisine ait. Dikkat çekici noktalardan birisi de Web Socket adresi. Görebildiniz mi?

İkinci ekran görüntüsünde http://localhost:4503/api/Scorer adresine HTTP Post talebi ile örnek bir olay bilgisi gönderilmekte. Bu talep sonrası uygulamalardaki log hareketliliklerine dikkat etmek lazım. Oluşan içerik bağlı olan istemciye yansımış olmalıdır. Bu yılın flaş takımı Anadolu Efes'ten 4 ve 5 numara pozisyonlarında oynayabilen ve üçlük yüzdesi de fena olmayan Moaerman epey ribaund toplamış sanki.

![09_34_credit_9.png](/assets/images/2019/09_34_credit_9.png)

Üçüncü çalışma zamanı görüntüsünde ekrana ikinci bir istemci dahil etmekteyiz. Bu durumda push edilen bilgiler bağlı olan tüm abonelere gönderilecektir ki istediğimiz senaryolardan birisi de bu (Bırayn Danstın mı? Yok artık Babi diksın mı?:D)

![09_34_credit_10.png](/assets/images/2019/09_34_credit_10.png)

> Eğer bu senaryoda yaptığımız gibi bir maçın canlı anlatımını çevrimiçi tüm abonelere göndermek istiyorsak, sonradan dahil olanların maçın başından itibaren kaçırdıkları olayları da görmesini isteyebiliriz. Burada Table Storage veya benzeri bir depoda maç bazlı tutulacak verileri, istemci ilk bağlandığında ona nasıl yollayabiliriz doğrusu çok merak ediyorum. İşte size güzel bir TODO;)

## Ben Neler Öğrendim?

Aslında hepsi bu. Temel bir kurgu ile Azure tarafındaki SignalR servisimizi kullanarak bir push notification sürecini deneyimledik diyebilirim. Her cumartesi gecesi çalışmasında olduğu gibi bu uygulamadan da bir şeyler öğrendim elbette. Bunları aşağıdaki gibi sıralayabilirim. Unutana kadar bendeler:)

- Azure tarafında bir SignalR Servisinin nasıl oluşturulacağını
- Geliştirme ortamında bir Azure Function projesinin nasıl inşa edilebileceğini
- SignalR üzerinden Hub dinleyicisi istemcilerde @aspnet/signalr npm paketinin nasıl kullanılabileceğini
- Azure Storage oluşturmadan Function projesindeki Table Storage'ın kullanılamayacağını
- SignalR servisini kullanan Azure Function projesinin herhangi bir istemci tarafından kullanılabilmesi için CORS tarafında '*' kullanılması gerektiğini (Bunu makalede bulamayacaksınız sizin keşfetmeniz gerekebilir:()
- Azure Function tarafında abonelerin SignalR ile el sıkıştığı fonksiyon adının 'negotiate'olması gerektiğini (Farklı bir isim kullanınca istemci tarafında HTTP 404 NotFound hatası aldım)
- Benzer şekilde SignalR Hubname olarak notifications kullanılması gerektiğini (Farklı bir isimlendirme kullanınca oluşan bilgilerin SignalR servisi tarafından yorumlandığını ama abonelere akmadığına şahit oldum)

Böylece geldik doğduğum, yaşadığım ve asla kopamayacağım [İstanbul plakalı cumartesi gecesi derlemesinin sonuna](https://github.com/buraksenyurt/saturday-night-works/tree/master/No%2034%20-%20Using%20Azure%20SignalR). Umarım sizler için de yararlı bir çalışma olmuştur. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
