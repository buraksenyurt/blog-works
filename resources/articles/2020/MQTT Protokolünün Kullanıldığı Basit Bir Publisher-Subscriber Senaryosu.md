---
title: "MQTT Protokolünün Kullanıldığı Basit Bir Publisher/Subscriber Senaryosu"
pubDate: 2020-05-05 13:48:00
categories:
  - Node.js
tags:
  - Node.js
  - Bash
  - JavaScript
  - JSON
  - REST
  - HTTP
  - Docker
  - GitHub
---

# MQTT Protokolünün Kullanıldığı Basit Bir Publisher/Subscriber Senaryosu
Yine bir yerlerde bir şeyleri araştırırken özellikle IoT ve M2M konseptinde yaygın olarak kullanılan MQTT (Message Queuing Telemetry Transport) isimli bir mesajlaşma protokolüne denk geldim. Düşük bant genişliklerinde, yüksek gecikme sürelerinin olduğu senaryolarda hafif bir mesajlaşma protokolü olarak karşımıza çıkıyor. En sık verilen senaryo bir IoT cihazının ısı sensöründen yayınlanan mesajın abone olan cep telefonu veya bilgisayarlar tarafından görülebilmesi. Elimde bir Raspberry PI vardı ama ısı sensörü yoktu. Dahası sensör alıp kurcalamaya üşendim diyelim. Hızlı bir antrenman için hayali bir senaryo düşündüm aşağıdaki karalamayı yaptım.

![Screenshot_1.jpg](images/Screenshot_1.jpg)

Bir basketbol sahasının seyirci giriş çıkıp kapılarını düşünelim. Bilet okutulur, kapıdaki cihaz bunla alakalı bir konuda (topic) mesaj yayınlamak ister. Cihaz akıllıdır ve salonun WiFi ağına bağlıdır. Kapı giriş/çıkış taleplerini toplayan bir REST servisine HTTP Post ile bilgi gönderir. Servis bunu MQTT protokolü üzerinden bir Broker'a aktarır (ki bizim senaryoda O açık kaynak Eclipse Moqsquitto'nun docker container örneğidir) Broker MQTT mesajlarını dinleyip abonelere dağıtan bir aracı görevini üstlenmektedir. Abone olan cep telefonu, bilgisayar veya farklı IoT cihazları bu mesajları yakayabilir. Senaryom çok anlamlı değil belki ama ben ille de MQTT'yi kullanacağım ya:D O yüzden antrenman için ideal. Bu arada örneğimizi gerçekleştirmek için iki önemli malzeme gerekiyor; Mosquitto Docker Image ve NodeJs:) Dilerseniz vakit kaybetmeden Broker servisi ayağa kaldırıp sunucu tarafını yazarak işe başlayalım.

## Broker ve Sunucu Tarafı

Broker için gerekli docker imajını yükleyip sonrasında proje klasör yapısını oluşturmamız lazım. İki NodeJs uygulamamız var. Birisi 4444 nolu porttan yayın yapan (publisher) ve gelen mesajları Mosquitto Broker'ına gönderen, diğeri 4445 nolu porttan ayağa kalkan ve abone olduğu konuları broker'dan dinleyen (subscriber).

```bash
# Gerekli docker imajını yükleyip başlatıyoruz. MQTT Broker hazır ve nazır
sudo docker run -d --name jerry-maguire -p 1883:1883 eclipse-mosquitto

# Verileri toplayan REST Servisin oluşturulması
mkdir collector
cd collector
touch server.js
npm init --y
# REST Api için express paketini kullanabiliriz. JSON içerikleri içinde body-parser biçilmiş kaftan
# MQTT Broker ile iletişim kurabilmek içinse mqtt paketini yüklüyoruz
npm install --save express body-parser mqtt
cd ..
```

Verileri toplayan server.js içeriğini aşağıdaki gibi kodlayabiliriz.

```javascript
var express = require('express');
var bodyParser = require('body-parser');
const mqtt = require('mqtt');
var app = express();
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));

// Önce bir bağlanalım. Docker Container'ın olduğu adrese doğru.
var qt = mqtt.connect("http://localhost:1883");

// MQTT Client paketi olay bazlı çalışır. Kullanımı kolaydır.

// Broker ile bağlantı sağlandığında
qt.on('connect', () => {
    console.log('Eclipse Mosquitto ile bağlantı sağlandı');
});

// Bir hata oluştuğunda
qt.on('error', (err) => {
    console.log(`Bir hata oluştu sanırım. ${err}`);
    qt.end();
});

// İstemci HTTP Post üstünden Broker'a iletilmek üzere bir istek aldığında
app.post("/input", function (req, res) {
    // HTTP Body ile gelen JSON içeriği alıyoruz
    var payload = req.body;
    // Bu içerikteki gate niteliğinin değerini topic olarak
    // identity niteliğini değerini de mesaj olarak kullanıyoruz
    // ve Broker'a gönderiyoruz
    qt.publish(payload.gate, payload.identity);
    console.log(`Broker'a ${payload.gate} konusunda ${payload.identity} mesajı gönderildi`);
    res.status(200).send('Mesaj Brokera gönderildi'); //İstemciye de HTTP 200 Ok gönderelim
});

app.listen(4444, function () {
    console.log("Uygulama 4444 nolu porttan hizmette");
});
```

## İstemci Tarafı

İstemci tarafını da basit bir NodeJs uygulaması olarak tasarlayacağız.

```bash
mkdir subscriber
cd subscriber
touch index.js
npm init --y
# istemci tarafında da mqtt paketini kullanmamız gerekiyor tabii
npm install --save mqtt express
```

ve index.js içeriğimiz.

```javascript
const mqtt = require('mqtt');
var express = require('express');
var app = express();

// Hemen bağlantımızı sağlayalım
var qt = mqtt.connect("http://localhost:1883");

// westSide isimli topic için abonelik başlatıyoruz
qt.subscribe('west side', { qos: 0 });

// Broker ile bağlantı sağlandığında
qt.on('connect', () => {
    console.log('Eclipse Mosquitto ile bağlantı sağlandı');
});

// Broker ile olan bağlantı kapatıldığında
qt.on('close', () => {
    console.log('Mosquitto ile bağlantı kesildi');
});

// Broker'a belli bir konuda bir mesaj geldiğinde
qt.on('message', function (topic, message) {
    console.log(`${topic} konusunda ${message} şeklinde bir mesaj geldi.`);

});

// Bir hata oluştuğunda
qt.on('error', (err) => {
    console.log(`Bir hata oluştu sanırım. ${err}`);
    qt.end();
});

app.listen(4445, function () {
    console.log("Uygulama 4445 nolu porttan ayakta ve dinlemede");
});
```

## Çalışma Zamanı

Artık sonuçları görmek için kolları sıvayabiliriz. Her iki node uygulamasını npm run start terminal komutu ile kendi klasörlerinde çalıştırdıktan sonra http://localhost:4444/input adresine

```json
{
"gate": "west side",
"identity": "1132"
}
```

benzeri farklı türde ve sayıda talepler göndererek abone olan diğer istemcide mesajların çıkıp çıkmadığını görebiliriz. Aşağıdaki ekran görüntüsünde olduğu gibi;)

![skynet_17_Screenshot_3.png](images/skynet_17_Screenshot_3.png)

Hepsi bu kadar sevgili dostlar:) Elbette bu örnek çalışma üstünden yapabileceğiniz birçok şey olduğunu da ifade etmek isterim. Örneğin subscriber uygulamasından birden fazla örnek çalıştırıp her bir dinleyiciye aynı mesajlar ulaşıyor mu kontrol edebilirsiniz. Diğer yandan subscriber olarak NodeJs'ten farklı dillerde program geliştirip broker ile çalışmayı deneyebilirsiniz. Ben üşendim ama siz üşenmezseniz eğer bir Raspberry PI'ye ısı sensörü bağlayıp sensörden okuduğunuz bilgiyi REST servisine veya doğrudan Mosquitto broker'ına göndermeyi düşünebilirsiniz. Böylece geldik bir SkyNet derlemesinin daha sonuna. Örneğe ait tüm kodlara [github reposu](https://github.com/buraksenyurt/skynet/tree/master/No%2017%20-%20MQTT%20on%20Simple%20Scenario) üzerinden erişebilirsiniz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
