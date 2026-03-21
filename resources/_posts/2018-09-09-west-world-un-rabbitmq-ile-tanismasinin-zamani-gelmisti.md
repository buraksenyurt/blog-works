---
layout: post
title: "West-World'ün RabbitMQ ile Tanışmasının Zamanı Gelmişti"
date: 2018-09-09 21:49:00 +0300
categories:
  - dotnet-core
tags:
  - rabbitmq
  - message-queue
  - linux
  - ubuntu
  - erlang
  - csharp
  - .net-core
  - vs-code
  - producer
  - RabbitMQ.Client
  - consumer
  - queue
  - fifo
  - ConnectionFactory
---
Küçük bir çocukken, soğuk bir kış gününde ablamın elini tutmuş Kadıköy'deki büyük postaneye doğru yol aldığımı hatırlarım. O yaz mektuplaşmak için adresini aldığım arkadaşıma yazdıklarımı göndermek istiyordum. Hayal mayal hatırladığım anlar. İlkokul zamanlarından kalma. Üzerinden onca yıl geçmiş durumda. Gönderen belli, alıcı belli, mektup ortada, pulları üstünde, yazılanlar içinde, taşıyıcı PTT her zaman ki gibi hizmetimizde. Tabii postaneye gelen bir çok mektup daha var. Hepsinin göndericisi ve gideceği adresler de belli.

![rabonww_g.jpg](/assets/images/2018/rabonww_g.jpg)

Bir postane uzun zamandır Message Broker sistemlerine benzetiliyor aslında. Mükemmel çalışan asenkron kuyruk sistemlerinin bir kısmının sırf bu işle uğraştığı söyleniyor. Apache Kafka, MSMQ ve başkaları. Ama tabii bir de pek çok platform tarafından desteklenen meşhur RabbitMQ var. RabbitMQ aslında bir posta ofisi olarak göz önüne alınıyor. Mektuplar yerine binary large objects (BLOB) gibi nesneleri taşıyabiliyor. Bu nesnelerin mutlaka göndericisi ve alıcısı (alıcıları) oluyor. Üstelik bu gönderim FIFO (First In First Out) mantığına göre yapılıyor. İlk giren mesaj ilk çıkar mantalitesinde.

Anladığım Kadarıyla

İşin teknik boyutuna girerek konuyu biraz daha açmaya çalışalım. Belli amaçlar için üretilen mesajlar olduğunu ve bu mesajların alıcılarının bulunduğunu düşünelim. Günümüzde kullandığımız herhangi bir sistemi düşünebiliriz. Özellikle mikroservislerin olduğu sistemler göz önüne alınabilir. Onlarca mikroservisin mesaj ürettiğini ve bu mesajların alıcılarının olduğunu düşünelim. Birisinin bu mesaj trafiğini yönetebilmesi gerekiyor. Hatta bir kuyruk sistemi ile ele alınması hiç fena olmaz. İşte bu tip ihtiyaçlarda genellikle Messaging Queue sistemlerinden yararlanılır ki RabbitMQ, Avanced Message Queueing Protocol'ünü baz alan gelişmiş versiyonlardan birisidir. Anladığım haliyle RabbitMQ aşağıdaki gibi bir mesajlaşma sistemini baz alır.

![rabonww_a.gif](/assets/images/2018/rabonww_a.gif)

Producer rolünü üstlenen taraf kuyruğa (belki de kuyruklara) atılmak üzere bir mesaj göderir. Mesaj, Exchange arabirimi tarafından karşılanır ve çeşitli kurallara göre bir veya daha fazla kuyruğa yönlendirmede bulunur. Exchange modellerinin herbirisi için ortak olan özellikler vardır. Name, Durability, Auto-Delete ve Arguments. Exchange'ler genellikle adlandırılırlar. Durability özelliğinin değerine göre mesajların disk üzerinde kalıcı olarak tutulup tutulmayacağı belirlenir. Varsayılan olarak bellek kullanılır. Auto-Delete ile işi biten mesajın kuyruktan otomatik olarak düşürülüp düşülmeyeceği ayarlanır. Arguments kısmında ise mesaja ait ek niteliklere yer verilir. Aslında Exchange türlerini aşağıdaki grafiklerle daha iyi anladığımı itiraf edebilirim (Teşekkürler Pluralsight)

Direct Exchanges. Genelde tek bir kuyruk kullanımı söz konusu ile ele alınıyor.

![rabonww_b.gif](/assets/images/2018/rabonww_b.gif)

Fanout Exchanges modelinde, mesaj birden fazla kuyruğa kopyalanmakta. Broadcasting sistemlerinde anlamlı. Örneğin güncel oyun sonuçlarının tüm oyunculara bildirilmesi veya hava durumunun haber kanallarına yayınlanması.

![rabonww_c.gif](/assets/images/2018/rabonww_c.gif)

Topic Exchanges'de mesajlar konularına göre farklı kuyruklara dağılabilmekte. Buna göre tüketiciler ilgili oldukları konuya ait kuyruğa düşen mesajları okuyorlar. Bir nevi sınıflandırma yapılığını ifade edebiliriz.

![rabonww_d.gif](/assets/images/2018/rabonww_d.gif)

Header Exchanges modelinde mesaj ile ilgili birden fazla niteliğin kullanılması ve buna göre uygun kuyruğa atılması söz konusu Burada önceki modellerde kullanılan Routing-Key ele alınmamakta. Bunun yerine mesajın başlığına eklenen nitelikler öne çıkmakta. Routing-Key'lerin sadece string olabileceği düşünülürse bu model kullanılarak farklı türlere göre sınıflandırma yapılmasına da mümkün hale geliyor.

![rabonww_f.gif](/assets/images/2018/rabonww_f.gif)

Kuyruğunda kendine göre bi takım özellikleri vardır. Name, Durable, Exclusive ve Auto-Delete. İsmi dışında kuyruğun hafızada mı yoksa kalıcı olarak disk üzerinde mi tutulacağını belirtebiliriz. Nitekim kuyruğun makinenin restart olması halinde kaybolmasını istemiyorsak Durable özelliğine true değerini atamamız gerekir. Kuyruk için açılan bağlantının durumuna göre silinip silinmeyeceği de Exclusive özelliğiyle belirlenir. Consumer'un abonelikten çıkması halinde de kuyruğun silinmesi istenebilir. Auto-Delete bu durumun ayarlanması için kullanılır. Malum kuyruğun da sonuç itibariyle bir maliyeti var. O nedenle kalıcı olması veya işi bitince silinmesi gibi kriterler büyük ölçeklere çıktığımızda ince performans ayarlarını gerektirebilir.

Consumer rolünü üstlenen taraf tahmin edileceği üzere kuyruktan ilgilendiği mesajı okur. Birden fazla mesaj tüketicisi olabilir. Hepsi aynı kuyruktan ya da birbirlerinden tamamen farklı konulardaki kuyruklardan beslenebilirler. Bu yoğurt yiğiş biraz da seçilen Exchanges stratejisine göre değişiklik gösterir. Consumer ile kuyruk arasında bir haberleşme söz konusudur. Consumer çoğu zaman bir mesajı aldıktan sonra bunu anladığına dair (acknowledgment) kuyruğa bildirimde bulunur ya da timeout gibi vakalar oluştuğunda mesajı geri çevirmek (Discard) veya yeniden kuyruğa aldırmak (Re-Queue) için dönüşler yapar. Kabaca bu durumu aşağıdaki gibi ifade resimleyebiliriz.

![rabonww_g.gif](/assets/images/2018/rabonww_g.gif)

Benim bu Cumartesi gecesindeki tek amacım ise RabbitMQ'yu West-World üzerinde konuşlandırmak ve.Net Core ile geliştirilmiş örneklerden yararlanarak basit mesaj alış verişi için kullanmak. Sevgili dostum Bora Kaşmer konuyu uzun zaman önce [şuradaki yazısında](http://www.borakasmer.com/rabbitmq-nedir) zaten ele almıştı. Ben hem bu yazıdan hem de internetteki diğer kaynaklardan yararlanarak RabbitMQ'yu Linux platformu üzerinde deneyimlemek istedim. Haydi gelin West-World'de bunun için neler yaptım sizlere kısaca anlatayım.

Başlıyorum

İlk önce West-World'e RabbitMQ'yu kurmam gerekiyordu. Uzun zamandır güncellemediğimi fark ettiğimden çalıştırılacak ilk iki terminal komut belliydi.

```bash
sudo apt-get update
sudo apt-get upgrade
```

Bu arada RabbitMQ'nun bir bağımlılığı bulunuyormuş. Erlang programlama diline ait platforma ihtiyaç duymaktaymış. Bu yüzden onu yükleyerek işe başladım.

```bash
wget http://packages.erlang-solutions.com/site/esl/esl-erlang/FLAVOUR_1_general/esl-erlang_20.1-1~ubuntu~xenial_amd64.deb
sudo dpkg -i esl-erlang_20.1-1\~ubuntu\~xenial_amd64.deb
```

Sonuçlar iç açıcıydı:P

![rabonww_0.gif](/assets/images/2018/rabonww_0.gif)

Erlang dilinin sisteme yüklendiğini teyit etmek için komut satırından erl yazıp çalıştırmam gerektiğini öğrendim. Bu şu an için çok yabancı olduğum bir fonksiyonel programlama dili.

```bash
erl
```

![rabonww_1.gif](/assets/images/2018/rabonww_1.gif)

Artık RabbitMQ kurulumuna başlayabilirdim. Linux terminalinde komutlarımı arka arkaya yazmaya başladım.

```bash
echo "deb https://dl.bintray.com/rabbitmq/debian xenial main" | sudo tee /etc/apt/sources.list.d/bintray.rabbitmq.list
wget -O- https://www.rabbitmq.com/rabbitmq-release-signing-key.asc | sudo apt-key add -
sudo apt-get update
sudo apt-get install rabbitmq-server
```

Kurulum başarılı bir şekilde tamamlandıktan sonra servisi etkinleştirmem gerekti.

```bash
sudo systemctl start rabbitmq-server.service
sudo systemctl enable rabbitmq-server.service
```

Sistem kontrolcüsünden yararlanarak RabbitMQ hizmeti yönetilebilir. Örneğin servisi durdurmak için,

```bash
sudo systemctl stop rabbitmq-server.service
```

komutu kullanabilir. Güncel durumunu kontrol etmek içinse,

```bash
sudo rabbitmqctl status
```

komutundan yararlanılabilir.

Bu işlemlerden sonra RabbitMQ'yu web browser'dan izleyebilmek için de bir şeyler yapmak gerekiyormuş. Nitekim bir şeyler yapmadan localhost üzerinde 15672 adresine gitmek istediğimde hatayla karşılaştım. İlk iş rabbitmq için yönetim arabirimini etkinleştirmekti.

```bash
sudo rabbitmq-plugins enable rabbitmq_management
sudo chown -R rabbitmq:rabbitmq /var/lib/rabbitmq/
```

Ardından Jerry isimli bir kullanıcıyı sisteme ekledim. Aslında bu kullanıcı ile web arayüzüne erişmeyi planlıyorum. Üstelik kendisini administrator rolü ile de ödüllendireceğim.

```bash
sudo rabbitmqctl add_user Jerry tom1234!
sudo rabbitmqctl set_user_tags Jerry administrator
sudo rabbitmqctl set_permissions -p / Jerry ".*" ".*" ".*"
```

![rabonww_2.gif](/assets/images/2018/rabonww_2.gif)

Artık tarayıcıdan http://localhost:15672 adresine gidip az önce oluşturduğum Jerry kullanıcısı ile giriş yapabilirim. İlk kez karşılaştığım bir arabirim. Ama RabbitMQ'nun artık West-World üzerinde olduğundan eminim.

![rabonww_3.gif](/assets/images/2018/rabonww_3.gif)

Producer Tarafını Yazmak

Ortada bir kuyruk sistemi artık var. Sırada mesaj yayınlayacak tarafı yazmak vardı. Klasik olarak Visual Studio Code'u açtım ve terminal penceresini kullanarak bir Console uygulaması oluşturdum. TurboNecati isimli uygulama mesaj gönderen program rolünü üstlenecek. RabbitMQ ile el sıkıştıktan sonrada AfacanMurat için kuyruğa bir kaç mesaj bırakacak. RabbitMQ ile kolayca konuşabilmek için RabbitMQ.Client isimli NuGet paketinden yararlanılabilir (Ben örneği.Net Core tabanlı olarak geliştirdiğim için bu paketi kullanıyorum. Ancak RabbitMQ için dil desteği çok geniş. Python, PHP, Java, Ruby, Go, Objective-C, Swift, Javascript gibi dillerle de rahatlıkla kullanabiliyor)

```bash
dotnet new console -o TurboNecati
dotnet add package RabbitMQ.Client
dotnet restore
```

Amacım kuyruğa mesaj atıp okuyabilmek olduğu için program kodları oldukça mütevazi.

```csharp
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using RabbitMQ.Client;

namespace TurboNecati
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionFactory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = connectionFactory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "commands",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var messages = new List<string>{
                                "Matematik çalış",
                                "Haftada bir kitap bitir",
                                "Felsefeyi tanı",
                                "Havalar sıcak sokak kapısına bir kap su bırak"
                            };

                    foreach (var message in messages)
                    {
                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(exchange: "",
                                         routingKey: "commands",
                                         basicProperties: null,
                                         body: body);
                        Console.WriteLine($"'{message}' gönderildi");
                        Thread.Sleep(300);
                    }
                }
            }

            Console.WriteLine(" Şimdilik bu kadar. Görüşürüz.");
            Console.ReadLine();
        }
    }
}
```

Öncelikle localhost adresini baz alan bir ConnectionFactory nesnesi örnekleniyor. Bundan faydalanarak bir IConnection arayüzü tarafından taşınabilecek bir bağlantı üretiliyor. connection nesnesinden yararlanılaraktan da asıl mesaj gönderme işlerini üstlenecek olan IModel arayüzünün taşıyabileceği bir değişken oluşturuluyor. QueueDeclare metodu ile command isimli bir kuyruk tanımlanmakta (Tabii metod parametrelerinin değerlerinin çeşitli anlamları var. Örnekğe göre mesajlar bellekte saklanacak ve sadece bu bağlantı için geçerli olacak) Eğer RabbitMQ üzerinde bu kuyruk yoksa oluşturulacak. Sonrasında gönderilecek mesajların herbirisi için bir byte dönüştürme işi uygulanıyor. Dolayısıyla çeşitli tipte nesneleri kuyruğa atabiliriz. Mesajların yollanması için BasicPublish metodu kullanılıyor. Temel görevi body ile gelen içeriği ilgili kuyruğa basmak. Programı çalıştırdıktan sonra elde ettiğim sonuçlar oldukça hoş. RabbitMQ web arayüzüne gittiğimde kuyruğa gelmiş ve okunmak için hazır bekleyen toplam 4 mesaj olduğunu gördüm.

![robonww_4.gif](/assets/images/2018/robonww_4.gif)

Hatta Queues kısmına girdiğimde commands isimli kuyruğun oluşturulduğunu da gördüm.

![robonww_5.gif](/assets/images/2018/robonww_5.gif)

TurboNecati sevgili yeğeni AfacanMurat için anlamlı yaz tatili mesajlarını bırakmıştı bile. Peki AfacanMurat bu mesajları nasıl okuyacak?

Consumer Tarafını Yazmak

Birileri mesajları yazıyorsa atılan bu mesajları okumak isteyen tarafları da vardır mutlaka. Şimdi de onu yazmam gerekiyordu. AfacanMurat uygulamasını benzer şekilde oluşturdum.

```bash
dotnet new console -o AfacanMurat
dotnet add package RabbitMQ.Client
dotnet restore
```

Sonrasında kodları yazmaya başladım.

```csharp
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

class Program
{
    public static void Main()
    {
        var connectionFactory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = connectionFactory.CreateConnection())
        {
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "commands",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var reader = new EventingBasicConsumer(channel);
                reader.Received += (m, e) =>
                {
                    var body = e.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"{message}");
                    Thread.Sleep(500);
                };
                channel.BasicConsume(queue: "commands",
                                     autoAck: true,
                                     consumer: reader);

                Console.WriteLine("Teşekkürler Necati Amca :)");
                Console.ReadLine();
            }
        }
    }
}
```

Aslında mesaj gönderen program kodlarına oldukça benzer bir kurgu söz konusu. İlk önce bir ConnectionFactory, ardından IConnection ve oradan da kanal modeli oluşturuluyor. AfacanMurat'da commands isimli kuyruğu dinleyecek. Mesaj okuma işlemi Received olay metodu ile sağlanmakta. BasicDeliverEventArgs tipinden olan e parametresinden yararlanılarak kuyruktaki mesaj alınıp string formata dönüştürülüyor. Örnekte 4 adet mesaj söz konusuydu. RabbitMQ'nun FIFO ilkesine göre de ilk giren mesaj ilk olarak elde edilecektir. Kodu çalıştırdım ve aşağıdaki ekran görüntüsünü elde ettim.

![rabonww_6.gif](/assets/images/2018/rabonww_6.gif)

Mesajlar AfacanMurat tarafından okunduğu için kuyruktan silinmişlerdi. RabbitMQ web arayüzünden bunu açıkça görebiliyordum.

![rabonww_7.gif](/assets/images/2018/rabonww_7.gif)

Bir şeyleri deneyimleyebildiğim güzel bir Cumartesi gecesi daha sonlanmak üzere. Sevgili CoderBora'nın güzel yazısı West-World üzerinde RabbitMQ'yu deneyimlemek için çok destekleyici oldu. Sonuçta kuyruk modelli bir mesajlaşma sistemini kurup üzerine bilgi yazıp okuyabildim. RabbitMQ bu kadarla sınırlı kalabilecek bir konu değil elbette. Ben kendim için kendi deneyimimi yazıp aktardım sadece. Gerisi sizin elinizde. Okuyun, araştırın, deneyin, ilerleyin. Örneğin ben Work Queues konusunu incelemeyi düşünüyorum. Web uygulamaları düşünüldüğünde bu kritiktir. RabbitMQ'da bu yapı nasıl uygulanır, öğrenir öğrenmez yazıya dökmek istiyorum. Böylece geldik bir maceranın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

### Kaynaklar:

RabbitMQ'nun güzide [Tutorial serisinin başlangıç noktası](https://www.rabbitmq.com/tutorials/tutorial-one-dotnet.html).
