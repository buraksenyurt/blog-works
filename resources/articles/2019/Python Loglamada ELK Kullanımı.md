---
title: "Python Loglamada ELK Kullanımı"
pubDate: 2019-04-27 19:32:00
categories:
  - python
tags:
  - python
  - Bash
  - XML
  - JSON
  - HTTP
  - Docker
  - Performance
  - GitHub
---

# Python Loglamada ELK Kullanımı
![elk.png](images/elk.png)

Laptop ekranına kitlenmiş error seviyesindeki logları inceliyordum. HTTP 400 en sevdiğim (yazar burada kendisiyle dalga geçiyor) ama çözmekte en çok zorlandıklarımdan birisiydi. Neyse ki monitör ettiğimiz araç bize güzel detaylar veriyordu. Pek tabii iş yoğunluğundan olsa gerek, üzerinde geliştirme yaptığımız ürünlerin bazı kurgularını inceleme fırsatı bulamıyordum. Lakin zaman zaman takım arkadaşlarımla veya mimari ekiptekilerle yaptığım konuşmalarda havada uçuşan, daha önceden duyduğum ama derinlemesine bilgi sahibi olmadığım kelimelere rastlıyordum.

ELK kısaltmasını ilk telafüz ettiklerinde zihnimde hiçbir şey canlanmamıştı. Bilmediğim ve öğrenmem gereken bir konu daha ortaya çıkmıştı işte. 2019 yılına girerken aldığım karar doğrultusunda öncelikle sağdan soldan adını duyduğum enstrümanları anlamaya çalışacak ve bunları [saturday-night-works](https://github.com/buraksenyurt/saturday-night-works) altında kabataslak notlarla deneyimleyecektim. Sanırım bu yıl için aldığım en doğru karardı diyebilirim. Şimdi bunun nimetlerini toplamak üzere bazı çalışmaları bloğumda kendime not olarak düşüyorum. Sırada ELK kelimesinin derin anlamını öğrenmek var. Tabii onu Kanada'nın simgelerinden biri olan 800 kiloluk koca bir geyik türü olarak düşünmüyoruz. En azından şimdilik...

ELK...Yani Elasticsearch, Logstash ve Kibana üçlüsü. Mikroservislerde log stratejisi olarak sıklıkla kullanılıyorlar. Tabii bazı ürünlerde çeşitli performans sıkıntıları nedeniyle tercih edilmediklerine dair yazılara da rastlamadım değil. Ancak şirketimizde de kullanılan bir düzenek olduğu için onu anlamamın en iyi yolu denemekten geçiyordu. Aslında düzenek son derece basit. İzlemek istediğimiz uygulamalara ait log bilgileri logstash tarafından dinlenip JSON formatına dönüştürülüyor ve Elasticsearch'e basılıyor. Elasticsearch'e alınan log kayıtları da Kibana arayüzü ile izleniyor.

Benim amacım ELK üçlüsünü WestWorld'de (Ubuntu 18.04, 64bit) deneyimlemek ve loglama işini yapan uygulama tarafında basit bir Python kodunu kullanmak. WestWorld'ün uzun denemeler sonrası bozulan ekosistemini daha da dağıtmak istemediğimden Elasticsearch ve Kibana tarafı için Docker Container'larını kullanacağım. Kabaca aşağıdaki gibi bir senaryonun söz konusu olduğunu ifade edebilirim.

![Cover_1.jpg](images/Cover_1.jpg)

## Örnek Uygulama Kodları

Aşağıdaki içeriğe sahip ve izlemek için log üreten çok basit bir Python kod dosyamız var. Dosyanın başındaki import bildirimlerinden de görüleceği üzere logging, time ve random isimli modüller kullanılıyor. Built-in logging modülünden yararlanılarak appLogs isimli dosyaya appende modda log atılacağı belirtiliyor. Logun mesaj ve tarih formatları da basicConfig metodunun son parametreleri ile tanımlanıyor.

```bash
# Python tarafında logging sistemi built-in olarak gelmektedir.
import logging
import time
import random

logging.basicConfig(filename="appLogs.txt",
                    filemode='a',
                    format='%(asctime)s %(levelname)s-%(message)s',
                    datefmt='%Y-%m-%d %H:%M:%S')
logging.warning('Sistem açılışında tutarsızlık')

for i in range(0, 10):  # 10 tane log atıyoruz
    # zamanı 0 ile 4 arası rastgele sürelerde duraksatıp log attırıyoruz
    d = random.randint(0, 4)
    time.sleep(d)
    if d == 3:
        logging.exception('Fatal error oluştu')
    else:
        logging.warning('Sistemde yavaşlık var...')

logging.critical('Sistem kapatılamıyor')
```

## Kurulumlar

Şimdi bu kodun logları için gerekli ELK düzeneğini kuracağız. Gerekenleri aşağıdaki gibi maddeleştirmeye çalıştım.

### Elasticsearch ve Kibana Tarafı

Elasticsearch'ün Docker kurulumu ve başlatılması için,

```bash
docker pull docker.elastic.co/elasticsearch/elasticsearch:6.6.0
docker run -p 9200:9200 -p 9300:9300 -e "discovery.type=single-node" docker.elastic.co/elasticsearch/elasticsearch:6.6.0
```

Kibana'ya ait Docker imajı için,

```bash
docker pull docker.elastic.co/kibana/kibana:6.6.0
sudo docker run --net=host -e "ELASTICSEARCH_URL=http://localhost:9200" docker.elastic.co/kibana/kibana:6.6.0
```

terminal komutlarını çalıştırmak yeterli. Ben örneği denediğim dönemde stabil olan versiyonları kullandım ancak siz kendi denemelerinizi yaparken konuları google'layıp doğru sürümleri kullanmaya çalışın. Bu arada Elasticsearch ve Kibana container'ları çalıştıktan sonra aşağıdaki adreslere gidip aktif hale gelip gelmediklerini kontrol etmekte yarar var.

```text
http://localhost:9200/ -> Elasticsearch
http://localhost:5601/status -> Kibana
```

Elastichsearch çalışır durumda.

![04_20_Cover_2.png](images/04_20_Cover_2.png)

Monitoring aracımız olan Kibana'da öyle.

![04_20_Cover_3.png](images/04_20_Cover_3.png)

### Logstash Tarafı

Logstash tarafı için öncelikle [şu adresten](https://www.elastic.co/downloads/logstash) ilgili içeriği indirip kurmak gerekiyor (Docker imajı yerine neden bu yolu tercih ettim şu anda hatırlamıyorum) Bundan sonra python uygulamamızın ürettiği logları takip etmesi için aşağıdaki içeriğe sahip bir konfigurasyon dosyasına ihtiyacımız var. Dosyayı etc/logstash/conf.d altına oluşturuyoruz. Bu klasör içerisindeki conf uzantılı dosyalar logstash servisi tarafından takip edilmekte. Böylece logstash hangi logları takip edeceğini bilecek ve onları Kibana tarafına gönderecek.

> Bu ve benzeri konfigurasyon dosyalarının logstash servisi tarafından otomatik olarak ele alınabilmesi için etc/logstash/conf.d klasörü altında konuşlandırılmaları önemli. Tabii Ubuntu için geçerli bir durum olduğunu ifade edelim.

logstash-python.conf

```xml
input{
 file{
 path => "/home/burakselyum/Development/saturday-night-works/No 20 - Python Logging with ELK/appLogs.txt"
 start_position => "beginning"
 }
}
filter
{
 grok{
 match => {"message" => "%{TIMESTAMP_ISO8601:timestamp} %{LOGLEVEL:log-level}-%{GREEDYDATA:message}"}
 }
    date {
    match => ["timestamp", "ISO8601"]
  }
}
output{
 elasticsearch{
 hosts => ["localhost:9200"]
 index => "index_name"}
stdout{codec => rubydebug}
}
```

Dosyanın üç ana kısımdan oluştuğunu söyleyebiliriz. Log dosya kaynağına ait bilgilerin olduğu input, dosya içinden bilginin nasıl alınacağına dair filter ve dönüşüm sonrası içeriğin nereye basılacağının belirtildiği output. path özelliğinin değeri logstash'in izleyeceği dosyayı ifade etmektedir. grok elementinin içeriği de önemlidir. Nitekim text dosyasına atılan standart log mesajlarını nasıl yakalayacağına dair bir desen tanımlamaktadır. Kısacası Grok filtreleme aracı ile text dosyaları gibi hedeflere atılan unstructured log bilgilerini parse etmenin oldukça kolaylaştığını ifade edebiliriz. Sistem logları, Apache logları, SQL logları vb bir çok enstrümanın loglama yapısı buradaki desenlere uygundur zaten. output kısmında dikkat edileceği üzere Elasticsearch'ün host bilgisi yer alıyor.

## Çalışma Zamanı

Tabii düzeneğin işlerliğini görebilmek adına en az bir kereliğine de olsa main.py dosyasını çalıştırmamız lazım. Bunu aşağıdaki terminal komutu ile yapabiliriz.

```bash
python3 main.py
```

Bu arada logstash servisinin aktif olduğundan emin olmak gerekiyor ki yazılan log'lar takip edilsin. Eğer çalışmıyorsa Logstash servisini başlatmak için terminalden

```bash
service logstash service
```

komutunu yürütmek yeterli.

## Kibana Monitoring

logstash etkinleştirildikten sonra Kibana'ya gidip yeni bir index oluşturabiliriz. index_name* ve @timestamp field'ını seçerek ilerlediğimizde python uygulaması tarafından üretilen logların yakalandığını görürüz.

![04_20_Cover_4.png](images/04_20_Cover_4.png)

> Visualize kısmını kurcalayarak çeşitli tipte grafikler hazırlayıp Dashboard'u etkili bir monitoring aracı haline dönüştürmemiz de mümkün.

## Docker Tarafı

Testler sonrası Docker tarafında ihtiyaç duyabileceğimiz komutlar da olabilir. Söz gelimi Container'ların listesini görmek ve durdurmak için aşağıdaki komutlardan yararlanabiliriz ki çalışma sırasında benim çok işime yaradılar (Container ID'ler farklılık gösterecektir)

```bash
sudo docker ps -a
sudo docker stop 3402e6aaced3
```

## Ben Neler Öğrendim

Cumartesi gecesi çalışmalarının [20nci bölümünü de bu şekilde tamamlamıştım](https://github.com/buraksenyurt/saturday-night-works). Bu macerada da öğrendiklerim oldu.

- ELK üçlemesinin nasıl bir çözüm sunduğunu
- Mikro servis dünyasında nasıl kurgulanabileceklerini
- Ubuntu platformunda Docker imajlarından nasıl yararlanılabileceğini
- Python kodundan logging paketini kullanarak nasıl log atılabileceğini
- Elastichsearch ve Kibana'nın docker imajları ile çalışmayı
- logstash config dosyasında ne gibi tanımlamalara yer verildiğini

Böylece geldik bir maceramızın daha sonuna. Sizde merak ettiklerinizi öğrenmek için kendinize vakit ayırın. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
