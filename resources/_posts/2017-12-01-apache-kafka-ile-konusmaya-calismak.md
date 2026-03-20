---
layout: post
title: "Apache Kafka ile Konuşmaya Çalışmak"
date: 2017-12-01 06:01:00 +0300
categories:
  - dotnet-core
tags:
  - dotnet-core
  - bash
  - csharp
  - dotnet
  - oracle
  - rabbitmq
  - http
  - docker
  - java
  - threading
  - serialization
  - generics
  - visual-studio
---
Bilgisayarla yeni tanıştığım dönemlerde aynı zamane çocuklarımızın tabletlerde yaptığı gibi oyun oynamaya bayılırdım. Dersler, sınavlar bir yana oyunlar bir yana. Kasetlerin olduğu Commodore 64 zamanlarından, disketler ile 486DX işlemcili makinelerdekilere, CD ile yüklenenlerden, internetten indirilip oynananlara kadar... Pek çok efsane vardı tabii oynananlar arasında. Bazen onların başında saatlerce nasıl vakit geçiriyormuşum diye düşününce hayret ediyorum kendime. Hoşuma giden oyunların en önemli özellikleri arasında ses efektleri ve karakter konuşmaları gelirdi.

![kafka_core_giris.gif](/assets/images/2017/kafka_core_giris.gif)

Duke Nukem'da onlardan birisidir. Geçenlerde internetten sözlerini araştırırken 3D isimli sürümünün listesini bulup text dosya içine attım. Neme lazım belki bir kod parçasında rastgele söz çıkartmak için kullanırız (Hatta Docker'ın bir tutorial'ında kullanılmıştı diye biliyorum) Peki bugünkü konumuzda nerede ele alacağız? Önce West-World'de neler oluyor bir bakalım.

West-World oldukça hareketli günler geçiriyor. En son Docker ile ilgili denemelerim olmuştu. Doğruyu söylemek gerekirse Ubuntu işletim sisteminden ve üzerinde çalışmaktan oldukça memnunum. Visual Studio Code inanılmaz esnek bir ortam sağlıyor. Dosyaları uzantısından tanıyıp önerilerde bulunabiliyor. Geniş bir extension deposu var. Diğer yandan.Net Core'u keşfetmeye de devam ediyorum. Docker tarafına da zaman zaman dönüp bakmaktayım. Resmi sitesindeki öğretiler gerçekten keyifli ve ne işe yaradığını anlıyorsunuz. Dipnot olarak terminal üzerinde çalışmak da keyif veriyor. Konuyu özümsemek adına adımları daha net görebiliyorum. Ancak yapmam gereken çok şey var. Şimdilik merak ettiğim konuları denemeye gayret ediyorum. Bunlardan birisi de bir kaç aydır çalışmakta olduğum ekipte sözü geçen ve üzerinde denemeler yapılan Apache Kafka. Onu elasticsearch ile entegre edip büyüyen loglarımızı dizginlemek amacıyla kullanmayı planlıyorlar.

Aktif olarak kullanıldığından nasıl bir şeydir öğrenmem gerekiyordu. Önce bankanım tahsis ettiği Ubuntu dizüstü üzerinde denemeler yaptım. Sonrasında West-World ile konuyu pekiştirmeye çalıştım. Tabii her şeyden önce Kafka'nın felsefesini kavramam lazımdı. Internette konu ile ilgili derya deniz kaynak var. Benim için en akılda kalıcı ve anlamlı şeyse temize çektiğim aşağıdaki not oldu.

![kafka_core_2.gif](/assets/images/2017/kafka_core_2.gif)

Şekildeki adımlar kısaca şöyle; Yayımcı/yayımcılar (Producer) belli bir konu başlığına (topic) ait mesaj/mesajlar yayınlar. Broker üzerinden akan mesaj başka bir tüketici/tüketiciler (Consumer) tarafından okunabilir. Karşımızda bir mesaj dağıtım sistemi var görüldüğü gibi. Kafka'nın yaptığı ise hasara uğramadan gerçek zamana oldukça yakın sürelerde veri akışkanlığını sağlamak. Sistem biraz daha netleşecek. Önemli sorulardan birisi neden böyle bir ürüne ihtiyaç duyulduğu? Verinin inanılmaz derecede büyüdüğünde hem fikiriz. Üstelik yıllardır var olan bir mevzu bu. Big Data kavramının ortaya koyduğu pek çok sorunsal da var. Verinin bu denli büyüyor olması bilgi akışlarının gerçek zamanlı olmasını negatif etkileyen bir durum. Kolayca ölçeklenebilir ve dağıtık modelde çalışabilecek sistemler gerekli.

İşte Kafka, vakti zamanında Linkedin'e çare olmuş bu sorunsalda. Sonrasında açık kaynak olarak sunulmuş (Tabii Linkedin ile sınırlı kalmamış. Paypal'den Twitter'a, Spotify'dan Netflix'e kadar pek çok tanıdık oyuncu var kullanıcıları arasında) Onun için başarılı bir dağıtık mesajlaşma servisi olduğunu ifade edebiliriz. Publisher/Subscriber (mesaj n sayıda tüketiciye gidebilir) ve Message Queue (Point to Point olarak da ifade ediliyor sanırım-mesaj sadece bir alıcı tarafından alınabilir) gibi iki modeli destekleyen, TCP/IP tabanlı, platform bağımsız, kolay ölçeklenebilir ve mesajları log kayıtlarına benzer yapıda tutan tam asenkron bir sistem var önümüzde.

> Kafka özellikle büyük boyutta loglama yapılan sistemlerde çok değerli. Log hareketlilikleri ve mesaj içerikleri nedeniyle başa bela olabilen bankacılık sistemlerinde ilk kabul gören çözümler arasında yer alıyor. Bunların dışında Streaming, Event Sourcing, Message Queing, Web Activity Tracing gibi konu başlıklarında da değerlendirilmekte.

Burada dikkat edilmesi gereken önemli noktalardan birisi de Kafka'nın kullanım amacı. Büyük veriyi tutmak için değil bunları toplayıp ilgili sistemelere hatasız ve hızlı biçimde aktarmak için kullanılan bir mesajlaşma hizmeti olarak değerlendirmek daha doğru gibi. Bu sebeple çoğunlukla tek başına ele alınmamakta. Kafka'yı kullanarak verinin ElasticSearch, Hadoop, Spark gibi sistemlere akıtılması söz konusu. Bunun belli başlı motivasyon kaynakları var. Her şeyden önce ilgili verinin aktarılacağı sistemler kapalı olsa bile bir süre Kafka'da tutma imkanı bulunmakta. Bu yetenek uç sistemlerden birinin çökmesi durumunda mesaj kaybını da engellemekte. Diğer bir motivasyon sebebi de verinin büyüklüğü. Büyük veriyi diğer sistemlere taşırken paralel çalışabilen ölçeklenebilir bir dağıtık sistemin arada olması önemlidir.

> "Aslında Kafka'yı anlamak için Arthur Schopenhauer'i anlamak lazım" demek isterdim ama esasında diğer Message Oriented Middleware'lere bakmamız gerekmekte. Microsoft Message Queue, IBM MQ, Apache Qpid, RabbitMQ ve varsa diğerleri. Bunlar Publisher/Subscriber modelini başarılı şekilde uygulayan MOM ürünleridir ancak akan veri büyüdüğünde ve Publisher sayısı onbinler seviyesine çıktığında otoriterlerin de belirttiği üzere çuvallarlar.
> Kafka'yı onlardan ayıran unsulardan birisi Subscriber'ların mesajları nasıl aldığı ile ilgili. Kafka'da Consumer'ın mesajı alabilmesi için belli bir konu başlığına abone olması gerekir. Diğer yandan Kafka düşük veri kaybını garanti etmektedir. Hem real-time veri akışını sağlayabilir hem de offline çalışma yeteneği sayesinde veriyi saklayabilir.
> Tabii işin özünde O da diğer MOM ürünlerinde olduğu gibi mesajları farklı bileşenler arasında taşımayı esas amaç edinmiştir.

Kafka ile konuşmaya başlarken bazı temel anahtar kelimelerine aşina olmak gerekiyor. Yukarıdaki şekilde de bahsettiğimiz gibi temel terimlerimiz Producer, Topic, Consumer ve Broker. Makaleye hazırlanırken işin mimari boyutlarına az da olsa girmeye çalıştım ancak kısa sürede odak noktamı kaybettim. West-World'teki amacım Kafka'yı kurup.Net Core kullanarak kendisiyle haberleşebilmekti en nihayetinde. Belli bir Topic için mesaj gönderip okuyabilirsem benim için yeterli olacaktı.

Önce Kurulum

Tabii işe ilk olarak Kafka'yı kurmakla başlamak lazım. Ben Kafka'yu doğrudan West-World üzerinde kuracağım ama dilersek Docker üzerinde de konuşlandırabiliriz (Bu aklımın bir köşesinde dursun) Sırasıyla aşağıdaki adımları takip ederek kurulum işlemini gerçekleştirebiliriz.

Alışılageldiği üzere adettendir bir update işlemi yapmakta yarar var.

```bash
sudo apt-get update
```

Java ortamına ihtiyacımız bulunuyor. O nedenle onu da kuruyoruz.

```bash
sudo add-apt-repository -y ppa:webupd8team/java
sudo apt-get update
sudo apt-get install oracle-java8-installer
```

Ardından ZooKeeper'ı yüklememiz gerekiyor. Kendisi Cluster'ları yönetecek olan koordinatör servisi oluyor. Kafka hizmete başlamadan önce onun başlatılması gerekiyor.

```bash
sudo apt-get install zookeeperd
```

> ZooKeeper aslında dağıtık sistem koordinatörü olarak düşünülebilir. Söz gelimi Cluster'a bir Broker eklendiğinde ya da Broker çöktüğünde bağlı olan Producer ve Consumer nesnelerini bilgilendirmek gibi kritik görevleri üstlenir.

Artık Kafka kurulumuna başlanabilir. Kafka'nın sıkıştırılmış paketini sistemde açacağımız bir klasöre indirebiliriz. Ben Home dizininde Kafka isimli bir klasör içerisine indirdim. Sürüm farklılık gösterebilir. En güncel versiyonu öğrenmek için [şu adrese](https://www.apache.org/dyn/closer.cgi?path=/kafka/1.0.0/kafka_2.11-1.0.0.tgz) bakmakta yarar var.

```bash
mkdir Kafka
cd Kafka
wget http://ftp.jaist.ac.jp/pub/apache/kafka/0.10.0.0/kafka_2.11-0.10.0.0.tgz
tar -xvf kafka_2.11-0.10.0.0.tgz -C .
```

Kafka'yı hemen deneyebiliriz. İlk olarak ZooKeeper hizmeti başlatılmalı.

```bash
sudo bin/zookeeper-server-start.sh config/zookeeper.properties
```

Bu arada başlatma sırasında bazı hatalar alınabilir. Örneğin ben Java Runtime'dan 2181 nolu portun kullanıldığına dair "Address already in use" hatası aldım ki bu port ZooKeeper tarafından kullanılmaktadır. O nedenle bir kill operasyonu gerçekleştirmem gerekti. 2181 nolu portun boşta olup olmadığını anlamak için

```bash
sudo netstat -lnap | grep 2181
```

komutunu kullanabilirsiniz. Eğer sorun çıkmazsa ZooKeeper çalışmaya başlayacaktır.

![kafka_core_3.gif](/assets/images/2017/kafka_core_3.gif)

Yaptığımız işlem bin klasörü altındaki zookeper-sever-start.sh betiğini config klasöründeki zookeper.properties dosyasında belirtilen varsayılan ayarlarla başlatmak. Şimdi kafka servisi etkinleştirilebilir. Bunun için yeni bir terminal penceresi açıp kafka paketini açtığımız konuma giderek aşağıdaki komutu çalıştırabiliriz.

```bash
sudo bin/kafka-server-start.sh config/server.properties
```

![kafka_core_4.gif](/assets/images/2017/kafka_core_4.gif)

Terminal Penceresinde Eğlence Zamanı

Ne durumdayız? Koordinatör servisimiz etkin ve bir tane Kafka Broker örneği de çalışır vaziyette. Kafka sunucusu çalıştırırken config altındaki server.properites dosyasından yararlanılmakta. İstersek bunları çoğaltabilir içeriklerindeki port numaralarını farklılaştırarak aynı anda birden fazla Broker'ın çalışmasını da sağlayabiliriz. Bu tam anlamıyla bir gerçek hayat senaryosu olur. Benim amacım bu kadar uzun boylu değil. Tek broker nesnesi üzerinden birer Producer (Publisher oluyor) ve Consumer (Subscriber oluyor) arasında mesaj dolaştırsam yeterli. Bunu yapmak içinse öncelikle bir konu başlığı (topic) açmamız gerekiyor. Yeni bir terminal penceresi açalım ve Producer rolünde bir topic açıp içerisine bir kaç bilgi atalım.

```bash
sudo bin/kafka-topics.sh --create --zookeeper localhost:2181 --replication-factor 1 --partitions 1 --topic ToDoList

sudo bin/kafka-topics.sh --list --zookeeper localhost:2181
```

![kafka_core_5.gif](/assets/images/2017/kafka_core_5.gif)

kafka-topics.sh betiğinden yararlanarak ilk komutla ToDoList isimli bir topic oluşturuyor, sonraki komutla da var olanları listeliyoruz. create ve list komutlarını kullanırken zookeeper adresini belirttiğimize dikkat edelim. Sanırım birden fazla Cluster olduğu senaryolarda birden fazla ZooKeeper hizmetinden yararlanabiliriz. Bu örnekte tek Broker kullanıldığı için replication faktörü 1 olarak belirlendi. Şimdi bir Producer oluşturup ToDoList isimli Topic altına birkaç veri ekleyelim.

```bash
sudo bin/kafka-console-producer.sh --broker-list localhost:9092 --topic ToDoList
```

Bu komutu çalıştırdığımızda terminal penceresinde satır bazlı veri girebilir hale geliriz. Aşağıdaki ekran görüntüsünde birkaç ToDo maddesi eklendiğini görebilirsiniz. Test için iki Consumer penceresi var. Consumer'ların bir Topic içeriğini görmesi için kafka-console-consumer.sh betiğinden yararlanmamız yeterli.

```bash
sudo bin/kafka-console-consumer.sh --zookeeper localhost:2181 --topic ToDoList --from-beginning
```

![kafka_core_6.gif](/assets/images/2017/kafka_core_6.gif)

Örnekleri denerken ilgimi çeken noktalardan birisi de Producer penceresi açıkken girilen bilgilerin bir alt satıra geçildiğinde otomatik olarak abone olan tüm Consumer pencerelerine yansımasıydı. Bunu canlı izlemek çok keyifli bir deneyim. Mutlaka deneyin derim:)

Denemeler yaparken düştüğüm bir hata da vardı. Producer betiğini çalıştırdıktan sonra terminalde bir şeyler olmasını beklerken rastgele tuşlara basmış, boş satırlar eklemiştim. Bunlar mesaj olarak gönderilmiyor diye düşünüyordum, çünkü ekranda bir tepki oluşmamıştı. Ancak sonradan Consumer üzerinden ne eklediysem gördüm. Kirli bir ToDoList konusu ile çalışmak istemiyordum. Bu nedenle ilgili Topic nesnesini aşağıdaki komutla silmek istedim.

```bash
sudo bin/kafka-topics.sh --delete --zookeeper localhost:2181 --topic ToDoList
```

ToDoList isimli topic nesnesinin silinmek üzere işaretlendiğine dair bir mesaj aldım. Ancak topic listesini çektiğimde olduğu yerde duruyordu. Aynı mesajda silme operasyonunun etkili olması için konfigurasyonda delete.topic.enable özelliğinin değerinin true olması gerektiği de belirtilmişti. Yapılması gereken şey config klasöründeki server.properties dosyasının sonuna bu bildirimi eklemek ve Kafka sunucusunu tekrardan çalıştırmaktan ibaretti. Bunu yaptıktan sonra delete betiğini tekrar çalıştırdım ve ToDoList'in kaldırıldığını gördüm. Pek tabii o sırada bu konu başlığına bağlı olan aboneler varsa onlara ilgili Topic nesnesinin olmadığına dair bir hata mesajı gönderildi.

![kafka_core_7.gif](/assets/images/2017/kafka_core_7.gif)

Kafka'yı terminalden az çok nasıl kullanacağımı öğrendim. Ancak terminalden uzun uzun o betikleri yazmaya çalışmak zevkli olsa da zorlayıcıydı. Her şeyden öte B12siz bir insanım. Çabuk unutuyorum. Şöyle işleri kolaylaştıracak kendi bildiğim dillerle kullanabileceğim bir API olsa hiç fena olmazdı. Hazır.Net Core dünyasında bir şeyler yapmaya çalışıyorken onunla ilerleyeyim dedim.

.Net Core Uygulamalarının Yazılması

Senaryomuzda iki Console uygulaması olacak. Birisinden Producer yardımıyla Broker'a mesaj bırakacağız. Diğer Console uygulamasını ise Consumer olarak tasarlayacağız. Consumer uygulaması, Producer tarafından belli konu başlıklarında bırakılan mesajları okumak için kullanılacak. İlk olarak FabrikamProducer'ı tasarlayalım.

```bash
dotnet new console -o FabrikamProducer
```

Bu işlemin ardından Confulent.Kafka paketini projemize eklememiz gerekiyor. Bu paket aracılığıyla Kafka ile konuşabileceğiz. Restore işlemini yapmayı ihmal etmessek iyi olur. Nitekim indirilen paket.Net Core 2.0 için yeniden ayarlanacaktır.

```bash
dotnet add package Confluent.Kafka
dotnet restore
```

Şimdi Program.cs içeriğini aşağıdaki gibi güncelleyelim.

```csharp
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;

namespace FabrikamProducer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Producer Tarafı\n");
            string brokerEndpoint="localhost:9092";
            var quotes=File.ReadAllLines("Quotes.txt");
            Random random=new Random();
            Console.WriteLine("Bir topic adı girer misin?");
            string topicName=Console.ReadLine();

            var config=new Dictionary<string,object>{
                {"bootstrap.servers",brokerEndpoint}
            };

            using(var producer= new Producer<Null,string>(config,null,new StringSerializer(Encoding.UTF8)))
            {
                for(;;)
                {                       
                    string message=quotes[random.Next(1,quotes.Length)-1];
                    var result=producer.ProduceAsync(topicName,null,message).GetAwaiter().GetResult();
                    Console.WriteLine($"Partition : {result.Partition} Offset : {result.Offset}\n{message}");                        
                    System.Threading.Thread.Sleep(10000);
                }
            }
        }
    }
}
```

Teori değişmiyor. Producer nesnesinin işini yapabilmesi için localhost:9092 adresindeki Broker ile konuşabilmesi gerekiyor. Bunun için config isimli değişkende bootstrap bilgisini ve endPoint adresini veriyoruz. Broker'a mesaj gönderme işini Producer tipinden bir nesne örneği gerçekleştirmekte. Mesaj string tipte ve UTF8 kodlamasına göre gönderilecek. Gönderme işini ProduceAsync isimli fonksiyon gerçekleştiriyor. İlk parametre ile kullanıcıdan aldığımı Topic adını belirtiyoruz. Son parametresinde ise Quotes.txt dosyasından çektiğimiz rastgele bir Duke Nukem sözü:) Eğer mesaj gönderimi başarılı ise bu mesajın Broker üzerindeki Partition ve Offset bilgilerini de ekrana basıyoruz. FabrikamProducer, 10 saniyede bir Duke Nukem mesajı yayınlayacak şekilde tasarlanmış durumda.

Abone olacak Console uygulamamızı ise FabrikamConsumer ismiyle oluşturabiliriz. Yukarıdakine benzer şekilde dotnet komutlarını kullanarak terminalden gerekli oluşturma işlemlerini yapıp, Confluent.Kafka paketini projeye dahil etmemiz gerekiyor.

```bash
dotnet new console -o FabrikamConsumer

dotnet add package Confluent.Kafka
dotnet restore
```

Sonrasında Program.cs içeriğini aşağıdaki gibi güncelleyebiliriz.

```csharp
using System;
using System.Text;
using System.Collections.Generic;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;

namespace FabrikamConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Consumer Tarafı\n");
            string brokerEndpoint="localhost:9092";
            Console.WriteLine("Hangi konu başlığına abone olacaksın?");
            string topicName=Console.ReadLine();

            var config=new Dictionary<string,object>{
                {"group.id","FabrikamConsumer"},
                {"bootstrap.servers",brokerEndpoint},
            };

            using(var consumer=new Consumer<Null,string>(config,null,new StringDeserializer(Encoding.UTF8)))
            {
                consumer.OnMessage+=(o,m)=>{
                    Console.WriteLine($"Duke Nukem diyor ki: {m.Value}");
                };

                consumer.Subscribe(new List<string>(){topicName});
                var isCancelled=false;
                Console.CancelKeyPress+=(_,e)=>{
                    e.Cancel=true;
                    isCancelled=true;
                };
                Console.WriteLine("Ctrl-C ile çıkabilirsin");
                while(!isCancelled)
                {
                    consumer.Poll(100);
                }
            }
        }
    }
}
```

Consumer nesnesi örneklenirken diğer örnekte olduğu gibi konuşacağımız Kafka Broker'ını belirtmemiz lazım. Bunun için yine localhost:9092 adresine bootstrap.servers ayarları ile gitmeye çalışıyoruz. Consumer nesne örneğinin OnMessage isimli olay metodu, abone olunan konu başlığına bir mesaj geldiğinde otomatik olarak tetiklenmekte. Bu nedenle ilgili olay metodu üzerinden m.Value ile ilgili Topic altına gelen metinsel bilgiyi (yani Duke Nukem sözünü) yakalıyoruz. Aboneyi ayakta tutmak için kullanıcı Ctrl+C tuşuna basana kadar uygulamayı bekletiyoruz. Poll operasyonunda yeni mesaj almaya hazır olduğumuzu belirtiyoruz. Normalde burada bir timeout süresi veriliyor ancak -1 ile bunu sonsuz olarak belirlemiş bulunmaktayız.

Uygulamaları test etmek için Visual Studio Code ortamındaki Integrated Terminal penceresini kullanarak dotnet run dememiz yeterli. Elbette Kafka sunucusunun ve onun öncesinde de ZooKeeper kontrol servisinin çalışır olduğundan emin olmalıyız. Sonuçlar benim için çok heyecan verici oldu. Açıkçası denemeniz ve kendi gözünüzle görmeniz lazım. 10 saniyede bir DukeNukem konu başlığına atılan mesajları, kaç tane abone varsa anında alabildi. İlginç, değişik, güzel, hoş.

Producer çalışmasından bir görüntü

![kafka_core_8.gif](/assets/images/2017/kafka_core_8.gif)

ve Consumer çalışmasından bir görüntü

![kafka_core_9.gif](/assets/images/2017/kafka_core_9.gif)

Böylece amacıma ulaşmış oluyorum. West-World artık Kafka'nın felsefesine biraz daha aşina gibi. Tabii merak edilesi bir konu daha var. Acaba buraya elasticsearch nasıl bağlanıyor. Bakalım buna vakit ayırabilecek miyim. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
