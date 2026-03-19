---
layout: post
title: "Servis Çıktılarını Plotly.js ile Grafikleştirmek"
date: 2019-03-18 21:51:00 +0300
description: "West-World'de eğlence tüm hızı ile devam ediyor. Bu geceki \"Easy Graphics of new Era\" isimli partinin onur konuğu ise açık kaynak javascript grafik kütüphanesi Plotly. Oldukça renkli bir kişiliğe sahip olan plotly github şehrinin de en sevilen karakterlerinden birisi. Kendisini West-World'e getirense en yakın arkadaşları D3.js ve WebGL. Ona West-World sakinleri adına bir soru yönelttik ve izleyicilerini nasıl büyülediğini sorduk. Enerjik ve içten uslübuyla \"dans figürlerimi çalışırken çoğunlukla JSON ve CSV melodilerinden ilham alıyorum. Kareografide uzun zamandarı Mr. jQuery ile ilerliyorum. Ayrıca Node'un bana sağladığı içsel motivasyondan bahsetmeden geçemem. Hepsi içimde harika bir karmanın oluşmasına neden oluyor. Sonuç gülümseyen ve ritmime uymaya çalışan insanların ortaya çıkarttığı müthiş enerji\" diyor..."
categories:
  - nodejs
tags:
  - nodejs
  - bash
  - json
  - javascript
  - rest
  - http
  - python
  - github
---
# Servis Çıktılarını Plotly.js ile Grafikleştirmek
![plotly_02.jpg](/assets/images/2019/plotly_02.jpg)

West-World'de eğlence tüm hızı ile devam ediyor. Geçen ay gerçekleştirdiğimiz "C64 Retro" partisinden sonra sıra bu geceki "Easy Graphics of new Era" adlı eğlenceye geldi. Onur konuğumuz açık kaynak Javascript dünyasının son zamanlardaki yükselek yıldızı olarak görülen grafik kütüphanesi Plotly. Oldukça renkli bir kişiliğe sahip olan Plotly, GitHub şehrinin de en sevilen karakterlerinden birisi haline gelmiş durumda. Şehrin devasa enerji santrallerinin ürettiği verilerle çalışan çılgın istatistikçileri arasında da çok popüler bir karakter. Kendisini West-World'e getiren en yakın destekçileri D3.js ve WebGL'de partiye renk katanlar arasındalar.

Ona West-World sakinleri adına bir soru yönelttik ve izleyicilerini nasıl böylesine inanılaz şekilde büyülediğini sorduk. Her zaman ki enerjisi ve içten uslübuyla "dans figürlerimi çalışırken çoğunlukla JSON ve CSV melodilerinden ilham alıyorum. Kareografide uzun zamandır Mr. jQuery ile ilerliyorum. Ayrıca Node'un bana sağladığı içsel motivasyondan bahsetmeden geçemem. Her isteğimi bekletmeden ve hızla karşılıyor. Hepsi içimde harika bir karmanın oluşmasına neden olmakta. Sonuç, gülümseyen ve ritmime uymaya çalışan insanların ortaya çıkarttığı müthiş bir dans gösterisi..." diyor.

Pek hayalimdeki gibi bir West-World partisi olmasa da sonuçta aşağıdaki çıktıya ulaşmak istediğimi ifade edebilirim esasında. Plotly.js kütüphanesi ile çok fazla çalışmışlığım yok. Projemizdeki belirli ihtiyaçlar nedeniyle javascript tabanlı bir grafik kütüphanesi araştırırken onunla karşılaştım. Özellikle basitliği, geniş grafik yelpazesi ve WebGL desteği dikkat çekici geldi. Ayrıca R, Python ve Matlab dilleriyle de kullanılabiliyor. Data Scientest rolündekilerin grafiksel raporlama ihtiyaçlarında bu oldukça kıymetli diye düşünüyorum. Nitekim veriyi tarayıcıda grafikselleştirmek birazdan göreceğiniz üzere gayet kolay.

![plotly_01.gif](/assets/images/2019/plotly_01.gif)

Tabii öncesinde onu en yalın haliyle kullanabilmem gerekiyordu. Kafamda basit bir kurgu hazırladım. Node.js ve express'i kullanarak, üç sunucunun son yedi günlük talep karşılama değerlerini JSON formatında döndüren bir servis yazacaktım. HTTP Rest modelinde çalışmasını planladığım bu servisi tamamladıktan sonra ona talepte bulunup gelen çıktıyı plotly sayesinde ekrana çizen bir HTML sayfası geliştirecektim. İşe FunnyGraphics isimli bir klasör açıp gerekli ön hazırlıkları yaparak başladım.

```bash
npm init
npm install express --save
```

npm başlangıcını yapıp, express modülünü yükledikten sonra package.json dosyasının son hali aşağıdaki gibiydi.

```json
{
  "name": "funnygraphics",
  "version": "1.0.0",
  "description": "simple plotly.js sample",
  "main": "app.js",
  "scripts": {
    "test": "echo \"Error: no test specified\" && exit 1",
    "start": "node app.js"
  },
  "author": "buraks",
  "license": "ISC",
  "dependencies": {
    "express": "^4.16.4"
  }
}
```

Sonrasında node.js sunucusu olarak görev yapıp talepleri karşılayacak app.js dosyasını yazmaya başladım.

```javascript
var express = require("express");
var app = express();
var path = require('path');

app.get("/", (req, res, next) => {
    res.sendFile(path.join(__dirname + '/index.html'));
});

app.get("/report", (req, res, next) => {

    var days = ['Day01', 'Day02', 'Day03', 'Day04', 'Day05', 'Day06', 'Day07'];
    var seri01 = {
        x: days,
        y: [5, 7, 9, 14, 12, 10, 9],
        name: 'dcist01',
        mode: 'lines+markers',
        type: 'scatter'        
    };
    var seri02 = {
        x: days,
        y: [5, 3, 8, 10, 12, 6, 3],
        mode: 'lines+markers',
        name: 'dcizm03',
        type: 'scatter'
    };
    var seri03 = {
        x: days,
        y: [0, 3, 5, 8, 8, 8, 7],
        mode: 'lines+markers',
        name: 'dclnd07',
        type: 'scatter'
    };
    var data = [seri01, seri02, seri03];
    res.json(data);
});

app.listen(6701, () => {
    console.log("Raporlama sunucusu aktif!");
});
```

express modülüne ait değişkenimiz 6701 nolu yerel porttan dinleme yapacak şekilde kullanılıyor. /report adresine gelen talepler için devreye giren fonksiyonumuzda aslında index.html içerisindeki plotly için önem arz edem bir JSON içeriği döndürülmekte. Burada gün bazlı olacak şekilde bir takım rastsal sayılar bulunduran üç farklı seri söz konusu. Her biri sembolik olarak bir sunucuyu belirtmekte. Ayrıca grafiklerin tipine ilişkin bir takım bilgiler de yolluyoruz. Burada bir kararsızlık yaşadığımı ifade edebilirim. Acaba servisten, plotly ile alakalı özellik bilgilerini göndermekle servisi ve görsel kütüphaneyi çok mu bağımlı hale getirdik? Peki sadece veriyi göndersek de bunun ayrıştırma ve gösterme kısmını HTML içerisine bıraksak nasıl olur du? Doğruyu söylemek gerekirse grafiğin ihtiyaç duyduğu veriyi aynı web site içerisinde çalıştığım için bu şekilde göndermek daha kolayıma geldi:| Servisi yazdıktan sonra küçük bir test yaptım. Önce

```bash
npm start
```

ile sunucuyu başlattım ve ardından http://localhost:6701/report adresine Postman'den HTTP Get talebi gönderdim. Sonuçlar başarılıydı.

![plotly_03.gif](/assets/images/2019/plotly_03.gif)

Sunucuya göre kök adrese gelen talepler doğrudan index.html sayfasının istemciye gönderilmesi ile sonuçlanmakta. Grafiğin çizildiği asıl yer index.html dosyasındaki script bloğu. Onu da aşağıdaki gibi tasarladım.

```text
<head>
    <script src="https://cdn.plot.ly/plotly-latest.min.js"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
</head>

<body>
    <div id="divStatistics">
    </div>
    <script>

        $(document).ready(function () {
            $.ajax({
                url: '/report',
                type: "GET",
                success: function (result) {
                    var layout = {
                        xaxis: { autorange: true },
                        yaxis: { autorange: true },
                        legend: {
                            x: 0,
                            y:-0.5,
                            yref: 'paper',
                            font: {
                                family: 'Tahoma',
                                size: 16
                            }
                        },
                        title: 'Haftalık sunucu istatistikleri (Akfit Servis/Gün)'
                    };

                    Plotly.newPlot('divStatistics', result, layout);
                },
                error: function (error) {
                    console.log('error ${error}')
                }
            })
        });

    </script>
</body>
```

Plotly ve jQuery kütüphanelerini dokümanın başındaki script bloklarında referans olarak bildiriyoruz (Local dosya olarak da kullanabiliriz tabii) /report adresine HTTP Get talebi yapmak için tek yöntem jQuery değil. ES6'nın fetch fonksiyonunu, XmlHttpRequest nesnesini veya bir başka çözümü de değerlendirebiliriz. Lakin benim kolayıma gelen jQuery oldu diyebilirim. Ajax çağrısı ile gerçekleştirilen işlem başarılı ise success bloğundaki kod parçası devreye giriyor. Grafiği çizdiren fonksiyon Plotly.newPlot isimli metod. İlk parametre ile grafiğin çizileceği div elementini belirtiyoruz. İkinci parametre ile grafiğin kaynak veri serileri gönderiliyor. Son parametre ile de grafiğe ait bir takım özellikler yollanıyor (legend'ın yeri, grafiğin başlığı, x ve y eksenlerinin otomatik olarak boyutlandırılacağı vs) Aslında hepsi bu kadar;)

Kütüphanenin kullanım alanı tabii ki çok daha geniş. Söz gelimi CSV içerikleri ile de kolayca çalışılabiliyor. Sadece veri serilerini doğru şekilde eşleştirmek gerekiyor. Aslında bu konuda [şu adresteki](https://raw.githubusercontent.com/plotly/datasets/master/stockdata.csv) veri kümesini kullanarak grafik örnekleri deneyebilirsiniz. [Resmi kaynakta](https://plot.ly/javascript/) konu ile ilgili detaylı bilgiler mevcut. Bir bakmakta yarar var ancak başlangıç basit senaryolarda okuduğunuz örnek yeterli olacaktır. Böylece geldik West-World'deki çılgın bir partinin daha sonuna. Sabah ışıklarını çok şükür bugün de gördük. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

Örneğe [GitHub adresinden](https://github.com/buraksenyurt/nodejs-tutorials/tree/master/Day12) ulaşabilirsiniz.