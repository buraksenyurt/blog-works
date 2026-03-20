---
layout: post
title: "React Üzerinde Socket.IO Kullanımı"
date: 2019-08-30 13:00:00 +0300
categories:
  - nodejs
tags:
  - nodejs
  - bash
  - csharp
  - javascript
  - http
  - react
  - github
---
Bir zamanlar sıkı bir Formula 1 izleyicisiydim. O dönemde dünyanın bir numaralı pilotu üç kez F1 dünya şampiyonu olan [Ayrton Senna](https://youtu.be/-pItzcHBHt0)'ydı. Yağmurlu havalardaki ustalığı nedeniyle Rainman lakabını almış bir yarışçı olmakla birlikte virajları hız kesmeden dönmeyi becerirdi. Monaco yarışından bir görüntüsü geldi şimdi gözümün önüne. Sağ eliyle kokpitin sağındaki vitesi sol eliylede direksiyonu tutuyordu.

![senna.png](/assets/images/2019/senna.png)

Kullandığı McLaren Honda MP4/4 marka aracın 1987'de 950 beygir güç üreten bir canavar olduğunu düşününce o hızlarda o yağmurlu havalardaki sürüş tekniği ile gelmiş geçmiş en iyi yarışçı olduğunu adeta ispat ediyordu. İlk başarılarını Lotus ile eden Senna'nın en büyük rakiplerinden birisi Williams takımından Alain Prost'tu (ki bir dönem McLaren'de takım arkadaşı da oldular) Lise yıllarıma denk gelen bu iki kahramanın özellikle kullandıkları canavarların dev posterleri oda duvarlarımı süslerdi. Yarışçı olmak gibi bir hayalim yoktu ama onların meydan okuyuşları, takımlarının otomobil dünyasındaki öncülükleri ilgimi çekiyordu.

> I'll be honest with you; I was never a Senna fan. I always thought Gilles Villeneuve was the greatest racing driver of them all. But, to make this film, I've watched hours and hours and hours of footage. And the thing is, Villeneuve was spectacular on a number of occasions. Senna...He was spectacular every single time he got in a car.
> Size karşı dürüst olacağım; Asla bir Senna hayranı olmadım. Her zaman Gilles Villeneuve'un hepsinin en iyi yarış pilotu olduğunu düşündüm. Ancak, bu filmi yapmak için saatlerce, saatlerce ve saatlerce çekimleri izledim. Mesele şu ki, Villeneuve birkaç kez muhteşemdi. Senna...Arabasında geçerdiği her anda muhteşemdi.
> Jeremy Clarkson, 2010, Top Gear, Series 15, Episode 5

Senna, ne yazık ki 1 mayıs 1994 günü henüz 34 yaşındayken San Marino grand prixinde İmola pistinde geçirdiği kaza sonucu hayatını kaybetmişti. Sonraki yıllarda Formula 1 yarışlarına olan ilgim epeyce azaldı. Şimdilerde televizyondaki canlı yayınlarını bile izlemiyorum desem yeridir. Ama arada bir baktığımda en çok dikkatimi çeken şey bilgisayar oyunlarındakine benzer ekranlar oluyor. Aracın iç kamerasından gelen sürüş görüntüleri üzerine eklenen anlık hız, ivme, vites vb bilgilerin sunulduğu grafikler gerçekten müthiş. Üstelik bu verilerin neredeyse hiç gecikme yaşanmadan ekrana ulaşması da bende hayranlık uyandıran başka bir konu. Bu grafikler tekrar nasıl mı gündeme geldi? İzin verin anlatayım.

Bir süre önce eski bir meslektaşım OBD2 portlarından nasıl bilgi okunabileceğini sormuştu. Bu konuyu araştırırken kendimi çok farklı bir yerde buldum. OBD2 portu ile bir arabadan veri almak mümkün. Peki bir yarış sırasında tüm araçların hız, motor sıcaklığı, anlık devir vb bilgilerini bu şekilde bir yerlere aktarabildiğimizi düşünsek. Bu verileri yarışı mobil uygulamalarından takip edenlere anlık gönderimi için nasıl bir yol izleyebiliriz? İşte araştırma sırasında geldiğim nokta buydu. Donanımsal gereksinimleri bir kenara bırakırsak bunun minik bir POC (Proof of Concept) çalışmasını yapmak istedim.

En ideal senaryolardan birisi Web Socket kullanmaktı. Socket.IO kütüphanesi bu amaçla değerlendirilebilirdi. Bir yarış aracının WebSocket haberleşmesi ile veri yayınlayacağını düşünelim. Haberleşme ağı üzerinde olan başka bir sunucu uygulama ile araç verileri abone olan istemcilere gönderilecek. Veri yayıncısı ve broadcast yönetimini üstlenecek sunucu için Node.js, görsel arayüzle yarış araçlarının gönderilen bilgilerine bakacak istemci tarafı içinse bir React uygulaması geliştirmeye karar verdim. E ne duruyoruz öyleyse. Kodlamaya başlayalım.

![credit_1.jpg](/assets/images/2019/credit_1.jpg)

> Bizim senaryomuzda tek bir yarış aracının bilgi yayınladığını varsayıyoruz. Şekildeki gibi n sayıda aracın ve dinleyicinin olduğu bir senaryoda, yayın yapan araçların verilerini diğerleri ile karışamayacak şekilde konsolide ederek göndermemiz gerekir ki istemciler n sayıda aracın verisini ya da istedikleri belli bir aracın verisini kullanabilsin.

## Ön Hazırlıklar

Örneği her zaman olduğu gibi WestWorld (Ubuntu 18.04, 64bit) üzerinde geliştirmekteyim. Sistemde node.js, npm ve react projesi oluşturmak için gerekli ekipmanlarım mevcut. Dolayısıyla hızlı bir şekilde proje iskeletini aşağıdaki terminal komutlarını kullanarak oluşturabilirim/oluşturabiliriz.

```bash
mkdir Zion
mkdir VehicleDataPublisher
cd Zion
npm init
npm i --save express socket.io
touch server.js
cd ..
cd VehicleDataPublisher
npm init
npm i --save socket.io-client
touch index.js
cd ..
sudo npx create-react-app dashboard
sudo npm i --save react-d3-speedometer save socket.io-client
```

Klasör yapısından ve içindeki uygulamalardan bahsetmek yarar var. Zion isimli klasörde sunucu uygulama kodlarımız olacak (Aslında yayıncı ve aboneler arasında bir veri aktarım organı) Veri yayını yapan uygulamamız burayı kullanacak. Sunucu, kendisini dinleyenlere ilgili verileri yayınlayacak. Web Sockets tabanlı bir iletişim söz konusu. Bu nedenle express ve socket.io paketleri kullanılıyor. VehicleDataPublisher uygulaması sembolik olarak veri yayını yapan program kodlarını içeriyor (Yani yarış aracımızdan veri gönderen parçayı taklit ediyor) Socket sunucusu ile haberleşmesi gerektiğinden socket.io-client paketini kullanıyor. Son olarak dashboard isimli bir react uygulamamız var. Bunu yarış aracından yayınlanan veriyi grafik formatında göstermek için kullanacağız. Bu nedenle [react-d3-speedometer](https://www.npmjs.com/package/react-d3-speedometer) (gerekirse benzerleri) paketini içeriyor. Pek tabii bu uygulama soket dinleyicisi olarak sunucu ile konuşmak durumunda. Bu nedenle socket.io-client paketini de referans ediyor. Son olarak react uygulamasını oluşturmak için [npx paket çalıştırıcısı](https://www.npmjs.com/package/npx)ndan yararlandığımızı da belirteyim.

## Gelelim Kodlarımıza

İskeletimiz hazır. Artık gerekli kodlamaları yapabiliriz. İşe Zion projesindeki server.js dosyasını yazarak başlayalım. Daha önceki derlemelerde olduğu gibi kodları aralardaki yorum satırları ile mümkün mertebe anlatmaya çalıştım.

```csharp
/*
Önce sunucu için gerekli modülleri ekleyelim.
Socket.Io kullanımı için socketIo modülü kullanılıyor.
Web server ve http özellikleri içinse epxress ve http modülleri.
*/

const http = require("http");
const express = require("express");
const socketIo = require("socket.io");

const app = express(); // express nesnesini örnekle
const appServer = http.createServer(app); // express'i kullanan http server oluşturuluyor
const channel = socketIo(appServer); // Socket.io middleware'e ekleniyor.

// ya çevre değişkenlerinden gelen port bilgisini ya da 5555 portunu kullanıyoruz
const port = process.env.PORT || 5555;

// Yeni soketler için connection isimli bir olay dinleyici açılıyor.
// İstemci connection namespace'ini kullanarak bağlanıyor
channel.on("connection", socket => {
    console.log(`${Date(Date.now()).toLocaleString()}: yeni bir istemci bağlandı`);
    // TODO: İstemci hakkında daha fazla bilgiyi nasıl alabilirim? IP adresi gibi.

    // gelen veriyi dinleyeceğimiz bir olay metodu olarak düşünebiliriz.
    // bir publisher sokete veri yolladığında devreye giriyor
    // Yayıncı, "input road" isimli namespace'den yararlanarak veri gönderebiliyor
    socket.on("input road", (data) => {

        console.log(`${Date(Date.now()).toLocaleString()}:Gelen veriler\n\tHız:${data.speed}\n\tDevir:${data.rpm}\n\tMotor sıcaklığı:${data.heat}`);
        // gelen veriyi, göndericiyi hariç tutaraktan bağlı olan ne kadar dinleyici varsa onlara yolluyoruz.
        // aslında bir broadcast yayın yapıyoruz diyebiliriz.
        // istemcilere yayın "output road" isimli namespace üzerinden yapılıyor.
        // emit metodunun ikinci parametresinde, yayıncının yolladığı verinin serileştirilerek kullanıldığını görebilirsiniz.
        socket.broadcast.emit("output road", { engineData: data });  // burası callback metodumuz olarak düşünülebilir
    });

    // istemcilerin bağlantı kesmelerini ele aldığımız olay
    // Bu kez "disconnect" isimli bir namespace söz konusu
    // disconnect, socket.io için rezerve edilmiş anahtar kelimelerden.
    socket.on("disconnect", () => {
        /* 
        Burada çeşitli temizleme operasyonları yapılabilir.
        Mesela istemcinin geliş gidiş hareketlerini takip ediyorsak,
        burada state değişikliği yaptırtabiliriz.
        */
        console.log(`${Date(Date.now()).toLocaleString()}istemci bağlantıyı kapattı`);
    });
});

// Sunucuyu ayağa kaldırıyor ve dinlemeye başlıyoruz
appServer.listen(port, () => {
    console.log(`${Date(Date.now()).toLocaleString()}: Sunucu ${port} nolu port üzerinden aktif konumda.`);
});
```

Araçla ilgili veri yayını yapan VehicleDataPublisher projesindeki index.js içeriğini de aşağıdaki gibi yazalım.

```javascript
/*
Bu kodun aslında bir araç üzerinde olduğunu varsayalım.
*/

// soket sunucusuna bağlantı oluşturuyoruz
// socket.io-client modülünü kullanıyoruz
let socket = require('socket.io-client')('http://localhost:5555');

// örnek simülasyon verimiz. Hız, devir ve motor sıcaklığı gibi
let engineData = {
    "speed": 0,
    "rpm": 0,
    "heat": 0
};

// Her 5 saniyede bir çalışacak bir fonksiyon.
setInterval(function () {
    // Rastgele veriler üretiyoruz.
    engineData.speed = getRandomValue(70, 180);
    engineData.rpm = getRandomValue(1000, 10000);
    engineData.heat = getRandomValue(100, 500);

    console.log(`Üretilen veri\nHız:${engineData.speed}\nDevir:${engineData.rpm}\nMotor sıcaklığı:${engineData.heat}`);
    /* 
        Veriyi emit metodu ile "input road" namespace'ini kullanarak sunucuya yolluyoruz
        oradaki callback'de devreye girip bu veriyi bağlı olan diğer istemcilere 
        (output road, namespace'ini kullanan) yayınlayacak.
    */
    socket.emit("input road", engineData);
}, 5000);

/* 
    Rastgele veri üertmek için kullandığımız basit fonksiyon.
    İki değer aralığında veri üretiyor.
*/
function getRandomValue(min, max) {
    return Math.floor(Math.random() * (max - min + 1) + min);
}
```

Verileri grafiksel ortamda gösterecek olan dashboard isimli react uygulamasının app.js dosyası da şu şekilde tasarlanabilir.

{% raw %}
```javascript
import React, { Component } from 'react';
import socketIOClient from "socket.io-client";
import ReactSpeedometer from "react-d3-speedometer";
/*
React uygulaması broadcast dinleyicisi rolünde.
Socket.io-client modülünü bu nedenle referans ediyor.
Ayrıca görsel metrikler için react-d3-speedometer paketini kullanıyor.
*/

class App extends Component {
  constructor() {
    super();

    // state değişkenlerimizde hızı, sıcaklığı, devri ve endpoint adresini tutuyoruz
    this.state = {
      speed: 0,
      rpm: 0,
      heat: 0,
      endpoint: "http://localhost:5555"
    };
  }

  /*
  componentDidMount yaşam döngüsü düşünüldüğünde
  component Document Object Model'e eklendiğinde devreye giren metodumuz.
  soket bağlantısını gerçekleştirip, "output data" yayınına abone oluyoruz.
  */
  componentDidMount() {
    const { endpoint } = this.state;
    const socket = socketIOClient(endpoint);
    //console.log(`${endpoint} adresine bağlantı yapılıyor...`);
    // output road'dan veri geldikçe bunları state değişkenlerine atıyoruz
    socket.on("output road", data => {
      this.setState({
        speed: data.engineData.speed,
        heat: data.engineData.heat,
        rpm: data.engineData.rpm
      });

      //console.log(`Gelen bilgi : ${data.engineData.speed}`);
    });
  }

  /*
  Bileşenin render edildiği metod.
  state değişkenlerini alıp, div elementindeki ReactSpeedometer kontrollerinde gösteriyoruz.
  */
  render() {
    const { heat } = this.state;
    const { rpm } = this.state;
    const { speed } = this.state;

    return (
      <div style={{ textAlign: "center" }}>
        <h2>Hız</h2>
        <ReactSpeedometer
          maxValue={200}
          minValue={70}
          value={speed}
          needleColor="gray"
          startColor="orange"
          segments={10}
          endColor="red"
          needleTransition={"easeElastic"}
          ringWidth={20}
          textColor={"black"}
        />

        <h2>RPM</h2>
        <ReactSpeedometer
          maxValue={10000}
          minValue={1000}
          value={rpm}
          needleColor="gray"
          startColor="orange"
          segments={100}
          maxSegmentLabels={10}
          endColor="red"
          needleTransition={"easeElastic"}
          ringWidth={20}
          textColor={"black"}
        />

        <h2>Motor Isısı</h2>
        <ReactSpeedometer
          maxValue={500}
          minValue={100}
          value={heat}
          needleColor="gray"
          startColor="orange"
          segments={5}
          endColor="red"
          needleTransition={"easeElastic"}
          ringWidth={20}
          textColor={"black"}
        />
      </div>
    )
  }
}

export default App;
```
{% endraw %}

## Çalışma Zamanı

Program kodlarımız hazır. Artık uygulamaları çalıştırıp sonuçlarına bakabiliriz. En az 3 terminal penceresi ile ilerlemek lazım. Birisinde sunucu, diğerinde publisher ve sonuncusunda da react tabanlı dinleyici çalıştırılmalı. Aşağıdaki terminal komutu ile sunucu ve veri yayıncılarını başlatabiliriz (Ayrı terminal pencrelerinde tabii ki)

```bash
npm run serve
```

React uygulaması içinse şu komutu kullanabiliriz.

```bash
npm run start
```

> React uygulaması başlatıldığında http://localhost:3000 adresi tarayıcıda açılır ve app.js'den render edilen html içeriği buraya basılır.

WestWorld üzerinde yakaladığım çalışma zamanına ait iki ekran görüntüsü aşağıda bulabilirsiniz. Aslında göstergeler canlı ortamda hareket ettiklerinden çok daha hoş ve etkileyici bir sonuç ortaya çıkıyor. Veri her 5 saniyede bir yenilenmekte.

![credit_2.jpg](/assets/images/2019/credit_2.jpg)

Bir başka t anında;

![credit_3.jpg](/assets/images/2019/credit_3.jpg)

Hepsi bu kadar:) Tabii örneği zenginleştirmek lazım. Benim ki epey aceleye geldi. Mesela senaryonun n sayıda araç (yayıncı) için n sayıda istemcide tekil veya toplu halde çalışabileceği farklı bir versiyonunu yazılabilir. Bu size güzel bir ev ödevi olsun.

## Ben Neler Öğrendim?

Bu yoğun çalışmada deneyimlediğim bir çok yeni şey oldu. WebSocket kavramına aşina olsam da onu bu örnekteki gibi daha görünür bir şekilde uygulamak değerliydi. Öğrendiklerimi şu şekilde özetleyebilirim.

- socket.io ile websocket bazlı iletişim trafiğinin node.js'de nasıl tesis edilebileceğini
- socket.on olay dinleyicilerinin ne amaçla ele alındığını
- broadcasting'in nasıl yapıldığını
- disconnect ve connection namespace'lerinin ayrılmış kelimelerden (reserved words) olduğunu (bunları doğru yazmassak istemciler bağlanamaz veya çevrim dışı olamazlar)
- node.js tarafında rastgele sayı üretimini
- belirli periyotlarda sürekli olarak çalışan bir fonksiyonun nasıl yazılacağını
- yayıncıların abonelere olan mesajları gönderdiğimiz fonksiyonun bir callback metodu olduğunu
- React bileşeninde state nesne kullanımını
- React üzerinden web socket haberleşmesinin nasıl yapılabileceğini
- component DOM'a bağlandığında hangi olay metodunun tetiklendiğini
- ReactSpeedometer'ın temel kullanımını

Böylece geldik [40 numaralı cumartesi gecesi derlemesi](https://github.com/buraksenyurt/saturday-night-works/tree/master/No%2040%20-%20SocketIO%20with%20React)nin sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
