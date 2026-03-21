---
layout: post
title: "Node.js - Asenkron Talep Karşılama"
date: 2019-01-10 17:00:00 +0300
categories:
  - nodejs
tags:
  - node.js
  - async
  - asynchronous-programming
  - rest-api
  - web-service
  - mongodb
  - javascript
  - vs-code
  - npm
  - node
  - express
---
Adrenali oldukça yüksek (özellikle benim için) ve zorlayıcı bir Cumartesi gününü geride bıraktım. Yo yo sandığınız gibi Cape Town'da büyük beyazlar ile dalış yapmadım ya da Helikopter'den Bungee Jumping... Hatta deployment sırasında canlı ortam datalarını da silmedim. Tek yaptığım Vialand'e gitmek oldu. Daha ilk turda bindiğim Vikings beni yeterince heyecanlndırırken, "Nefes Kesen" neredeyse ses hızına yaklaştığımı hissettirdi:P

![async_viking.jpg](/assets/images/2019/async_viking.jpg)

Ehh, yanınızda bu adrenaline doymayan bir arkadaşınız veya çocuğunuz varsa o aletten diğerine koşturmayı bırakın her bir aleti defalarca deneyimlemek zorunda da kalabilirsiniz. Hoş bunu sevenler ve etkilenmeyenler için inanılmaz derecede eğlenceli bir ortam söz konusu. Lakin benim gibi yaşlı bünyeler için aslında bu kadar adrenalin biraz (belki birazdan da fazla) ürkütücü diyebilirim. Bu yoğun heyecan üzerine beni dengeleyen tek yer çalışma odam oldu. Viking'deki sulu inişleri, Rollar Coaster'daki 38 saniyelik öldürücü heyecanı, Adelet Kulesinden yapılan 50 metrelik sert düşüşü bir kenara bıraktım ve West World'e doğru yol aldım. Elimde incelenmeyi bekleyen ve hafta boyu gerek Pluralsight eğitimleri gerek dokümanlar olsun çalıştığım güzel bir konu vardı.

Bu yazımızda Node.js ile geliştirilmiş sunucu uygulamalarında async kullanımını inceleyeceğiz. Amacımız istemci talebi sonrası arka planda paralel servis çağrıları gerçekleştirmek ve ayrıca bu süreç sırasında sunucuya gelecek diğer isteklerinde değerlendirilebileceğini görmek. Bunlara ilaveten ön tarafta konuşlandıracağımız ana servisin bir yönlendirici (router) gibi kullanılabileceğini öğreneceğiz. Haydi gelin hiç vakit kaybetmeden serüvenimize başlayalım. Konuyu basit bir şekilde anlayabilmek adına örnek bir senaryo üzerinden gitmekte yarar var. Başlangıç için aşağıdaki şemayı göz önüne alabiliriz.

![async_node_0.gif](/assets/images/2019/async_node_0.gif)

İki farklı MongoDb (farklı türlerden de olabilir) veri depomuz olduğunu düşünelim. Bunların sayısı daha da artabilir. Her iki mongodb ile ayrı ayrı çalışan servislerimiz var. JSON tabanlı basit Rest servisleri olarak ele alabiliriz. Bu iki oluşumun farklı sunucular üzerinde tesis edildiğini varsayalım. Önde duran ve belirli talepler için arka taraftaki ilgili servislere yönlendirme (routing) işini üstlenen bir başka servis var. Bu servise vereceğimiz temel görev, player ve team servislerine paralel talep gönderek çıktıların istemciye yollanması. Yani öndeki servisimiz takım ve oyuncu listelerini veren servis metodlarını paralel olarak işletip tamamı elde edilince istemciye cevap dönecek.

Testlerimiz sırasında arkadaki servislerin standart get operasyonlarında duraksatma yapacağız (Örneğin 7şer saniye kadar) Bu durumda ervisleri arka arkaya çalıştıracak olsak çıktıların toplamda 14 saniye civarında elde edilmesi beklenir. Ancak paralel çalıştırıp her iki çıktıyı da 7 saniye civarlarında elde etmemiz mümkün. İşte bu noktada async modülü ve paralel fonksiyon çalıştırma özellikleri işimize yarayacak. Hatta bu veri elde etme işlemi yapıldığı süre boyunca öndeki servisimiz farklı talepleri de karşılayabilir durumda olacak ki bu da Node.js'in doğal çalışma prensipleri ile mümkün. Gerçek hayat senaryolarında sıklıkla ihtiyaç duyacağımız bir senaryo. Bakalım Node.js tarafında bu iş nasıl yapılabiliyor basitçe inceleyelim.

Öncelikle West-World (Ubuntu 16.04 - 64bit) üzerinde çalıştığımı ve MongoDB'nin Compass Community Edition'ının yüklü olduğunu ifade edeyim. Bunlara ek olarak tabii ki node.js'de sistemde yüklü durumda. Biz üç servisimizi de aynı makine üzerinde ama farklı portlardan sunacağız. Bu şekilde grafikteki senaryoyu taklit etmeye çalışacağız. Çözümümüze ait temel klasör yapısını aşağıdaki gibi kurgulayarak işe başlayalım.

models
---player.js
---team.js
server
---MainServer.js
---PlayerServer.js
---TeamServer.js

Örneklerde kullanacağımız bir takım npm paketleri var. İşlerimizi kolaylaştırması açısından. MongoDb ORM eşlemesi için mongoose, REST servis tarafı için express, asenkron işlemleri kolaylaştırmak için async ve son olarak JSON parsing için body-parser... Terminalden aşağıdaki komutları kullanarak gerekli kurulum işlemlerini yapabiliriz.

```bash
sudo npm install mongoose
sudo npm install express
sudo npm install request
sudo npm install async
sudo npm install body-parser
```

MongoDb tarafında kullanılacak iki temsili veritabanı modelimiz var. Player ve Team. Bunlara ait entity nesnelerini aşağıdaki gibi tanımlayabiliriz.

Player.js

```javascript
var mongoose = require('mongoose');

var playerSchema = mongoose.Schema({
    fullName: String,
    size: String,
    position: String
});

module.exports = mongoose.model('Player', playerSchema);
```

Team.js

```javascript
var mongoose = require('mongoose');

var teamSchema = mongoose.Schema({
    name: String,
    city: String
});

module.exports = mongoose.model('Team', teamSchema);
```

Her iki kod parçasında mongoose paketinden yararlanılıyor. Schema metodunda Team ve Player nesnelerinin özelliklerini tanımlıyoruz. Bu özellikler aynen MongoDb tarafında da kullanılacaklar. Team ve Player nesnelerini modül üzerinden dışarıya açarken de model fonksiyonundan yararlanılmakta. Burada model adlarını ve eşleştikleri şemaları belirtmekteyiz. Şimdi de Player ve Team nesneleri ve dolayısıyla MongoDb veritabanı ile çalışacak olan REST servislerine ait kodlarımızı yazalım. PlayerServer sınıfının kod içeriğini aşağıdaki gibi geliştirebiliriz.

```javascript
var express = require('express');
var app = express();
var bodyParser = require('body-parser');
var Player = require('../models/player.js');

var mongoose = require('mongoose');
mongoose.connect('mongodb://localhost/player', { useMongoClient: true });

app.use(bodyParser.json());
app.use(bodyParser.urlencoded({
    extended: true
}));

app.post('/players', function (req, res) {
    var newPlayer = new Player(req.body);
    newPlayer.save(function (err) {
        if (err) {
            res.json({ error: err });
        };
        res.json({ info: 'oyuncu bilgisi oluşturuldu' });
    });
});

app.get('/players', function (req, res) {
    Player.find(function (err, players) {
        if (err) {
            res.json({ error: err });
        };
        setTimeout(function () {
            res.json({ data: players });
        }, 7000);
    });
});

app.get('/players/:id', function (req, res) {
    Player.findById(req.params.id, function (err, player) {
        if (err) {
            res.json({ error: err });
        };
        if (player) {
            res.json({ data: player });
        } else {
            res.json({ info: 'oyuncu bulunamadı' });
        }
    });
});

var server = app.listen(7001, function () {
    console.log('PlayerServer is online http://localhost:7001/');
});
```

Yerele makinenin 7001 nolu portu üzerinden hizmet veren PlayerServer, express ve mongoose modüllerini etkin bir şekilde kullanmakta. Model olarak Player.js dosyasından yararlanılıyor. İşlemleri basitleştirmek adına sadece üç operasyon sunmaktayız. Tüm oyuncu listesini çekebiliyoruz ya da mongoDb'de oluşturulan objectId bilgisini kullanarak tek bir tanesini talep edebiliyoruz. Birde oyuncu ekleme işini kolaylaştırmak için yazdığımız Post tabanlı çalışan metodumuz var. MongoDb bağlantısı connect metodu üzerinden sağlanmakta. Eğer MongoDb örneğinde Player veya Team gibi veritabanları yoksa ilk bağlantı sırasında oluşturulacaklardır. Kodda dikkat edilmesi gereken noktalardan birisi de setTimeout metodunu kullanmış olmamız. Bunu testimizin bir parçası olarak düşünebilirsiniz. Senaryomuza göre tüm oyuncu listesinin çekilmesi yaklaşık olarak yedi saniyede gerçekleşiyor. PlayerServer kendi başına çalışabilen bir servis olduğundan belli bir port üzerinden yayın yapacak şekilde ayarlanmış durumda. Örneğimize göre yerel makinedeki 7001 nolu port üzerinden hizmet verecek. TeamServer dosyasındaki kodlarda PlayerServer tarafındakine oldukça benzer. Sadece Team modeli ile çalıştığını söyleyebiliriz.

```javascript
var express = require('express');
var app = express();
var bodyParser = require('body-parser');
var Team = require('../models/team.js');

var mongoose = require('mongoose');
mongoose.connect('mongodb://localhost/team', { useMongoClient: true });

app.use(bodyParser.json());
app.use(bodyParser.urlencoded({
    extended: true
}));

app.post('/teams', function (req, res) {
    var newteam = new Team(req.body);
    newteam.save(function (err) {
        if (err) {
            res.json({ error: err });
        };
        res.json({ info: 'takım bilgisi oluşturuldu' });
    });
});

app.get('/teams', function (req, res) {
    Team.find(function (err, teams) {
        if (err) {
            res.json({ error: err });
        };
        setTimeout(function () {
            res.json({ data: teams });
        }, 7000);
    });
});

app.get('/teams/:id', function (req, res) {
    Team.findById(req.params.id, function (err, team) {
        if (err) {
            res.json({ error: err });
        };
        if (team) {
            res.json({ data: team });
        } else {
            res.json({ info: 'takım bulunamadı' });
        }
    });
});

var server = app.listen(7002, function () {
    console.log('TeamServer is online http://localhost:7002/');
});
```

PlayerServer içerisindeki çalışma prensiplerinin Team nesnesi için değiştirilmiş olduğunu görebilirsiniz. Birde tabii farklı bir port üzerinden yayın yapmaktayız. Bu iki servisi kullanan MainServer isimli yönlendirme servisinin kodları biraz daha farklı olacak. Aynen aşağıda olduğu gibi.

```javascript
var express = require('express');
var app = express();
var bodyParser = require('body-parser');
var async = require('async');
var request = require('request').defaults({
    json: true
});

app.use(bodyParser.json());
app.use(bodyParser.urlencoded({
    extended: true
}));

app.get('/sports/api', function (req, res) {
    async.parallel({
        player: function (callback) {
            request({ uri: 'http://localhost:7001/players' }, function (error, response, body) {
                if (error) {
                    callback({ service: 'player', error: error });
                    return;
                };
                if (!error && response.statusCode === 200) {
                    callback(null, body.data);
                } else {
                    callback(response.statusCode);
                }
            });
        },
        team: function (callback) {
            request({ uri: 'http://localhost:7002/teams' }, function (error, response, body) {
                if (error) {
                    callback({ service: 'team', error: error });
                    return;
                };
                if (!error && response.statusCode === 200) {
                    callback(null, body.data);
                } else {
                    callback(response.statusCode);
                }
            });
        }
    }, function (error, results) {
        res.json({
            error: error,
            results: results
        });
    });
});

app.get('/aloha', function (req, res) {
    res.json({ yuhuuu: Date.now() });
});

var server = app.listen(7000, function () {
    console.log('MainServer is online http://localhost:7000/');
});
```

MainServer içerisinde async ve request modüllerinden yararlanarak paralel çalışma disiplinlerini uyguluyoruz. sports/api şeklinde gelecek olan bir talep ele alınırken async modülünün parallel metodu çağrılıyor. Burada player ve team isimli iki task oluşturulduğunu görebilirsiniz. Herbiri request nesnesini kullanarak arka taraftaki servislere HTTP Get talebinde bulunuyor. Eğer üç servisinde ayrı makinelerde barındırıldıklarını düşünecek olursak, MainServer Router Service görevini de icra ediyor diyebiliriz. parallel fonksiyonu içerisindeki görevler tamamlandığında (yani arka servislere yapılan çağrıların sonuçları elde edildiğinde) ikinci parametredeki fonksiyon devreye giriyor ve istemci tarafına sonuçların JSON formatında döndürülmesi sağlanıyor. Paralel talep işlenen get fonksiyonu dışında aloha şeklinde gelecek taleplerin ele alındığı bir metodumuz daha var. Yani MainServer birisi parelel task barındırmak suretiyle iki HTTP Get operasyonu sunmakta. Bu fonksiyonu neden koyduğumuzu yazının sonlarında daha net anlayacağımızı düşünüyorum.

PlayerServer ve TeamServer önceden de belirttiğimiz üzere tek başlarına da hizmet verebilirler. 7001 ve 7002 gibi iki farklı porttan aynı anda yayınlanabilirler. Her ikisi için deneysel olması adına POST, GET, GET (ID ile) olmak üzere üç operasyon sunuluyor. İlerlemeden önce bu operasyonların işlerliğini basitçe test etmekte yarar var. Örneğin Postman kullanılarak aşağıdaki komut ile yeni bir oyuncu bilgisini eklememiz mümkün.

```text
Request : HTTP Post
Address : http://localhost:7001/players
Body : {"fullName":"toni kukoç","size":"2.06cm","position":"power forward"}
```

Sonuç aşağıdaki gibi olacaktır.

![async_node_1.gif](/assets/images/2019/async_node_1.gif)

Eğer eklenen oyuncuların tamamını çekmek istersek aşağıdaki gibi bir talepten yararlanabiliriz.

```text
Request : HTTP Get
Address : http://localhost:7001/players
```

![async_node_2.gif](/assets/images/2019/async_node_2.gif)

Tabii içeriye koyduğumuz 7 saniyelik şaşırtmaca sebebiyle sonuçlar anında ekrana yansımayacaktır. Belli bir IDye bağlı oyuncuyu görmek istersek de aşağıdakine benzer bir talep yapmamız yeterli olur.

```javascript
Request : HTTP Get
Address : http://localhost:7001/players/5b9e5423c826230460cc0310
```

![async_node_3.gif](/assets/images/2019/async_node_3.gif)

Benzer çalışmalar TeamServer servisi çalıştırılarak da deneyimlenebilir. İlerlemden önce yazdığını TeamServer hizmetinin operasyonlarını da test etmenizi öneririm.

Gelelim asıl senaryomuza. Şimdi üç servisi de terminalden ayağa kaldırmamız lazım (ayrı terminal pencereleri kullanarak bu işi yapabiliriz ya da forever gibi bir npm paketinden faydalanabiliriz)

```bash
node PlayerServer.js
node TeamServer.js
node MainServer.js
```

![async_node_4.gif](/assets/images/2019/async_node_4.gif)

Yine Postman'den yararlanarak aşağıdaki talebi gönderelim.

```text
Request : HTTP Get
Address : http://localhost:7000/sports/api
```

![async_node_5.gif](/assets/images/2019/async_node_5.gif)

Hem oyuncu hem de takım listeleri aynı JSON içeriğinde çıktı olarak döndürüldüler. Ancak dikkat edilmesi gereken nokta sadece arka planda yapılan adres yönlendirmesinin başarılı bir şekilde çalışmış olması değil. Her iki servisin get operasyonu 7 saniyelik duraksatma içeriyor ve servisin toplam cevap süresi de 7 saniye civarında. Bu MainServer'a gelen talep sonrası PlayerServer ve TeamServer'a eş zamanlı olarak taleplerin gönderilmiş olduğu anlamına da geliyor. Bu noktada servislerden birisinin duraksatma süresini kaldırıp tekrardan test etmenizi tavsiye ederim. Hatta MainServer'a yapılan talep sonrası oluşan 7 saniyelik bekleme süresince şu talebi göndermenizi öneririm.

```text
Request : HTTP Get
Address : http://localhost:7000/aloha
```

![async_node_6.gif](/assets/images/2019/async_node_6.gif)

Yani sorumuz şu; Yedi saniyelik talep cevaplama süresi boyunca yapılacak olan yukarıdaki istek anında cevaplandırılır mı?;) Tahmin edeceğiniz üzere node.js doğal çalışma dinamikleri gereği ilgili talebi duraksatmayacaktır. Dolayısıyla paralel olarak n sayıda talebin servis tarafında ele alınması mümkündür.

Bu yazıdaki örneğimizde bir yönlendirici servisin nasıl yazılabileceğini ve herhangibir talebin asenkron çalışma prensipleri doğrultusunda paralel görevleri nasıl başlatabileceğini görmüş olduk. Ayrıca eş zamanlı başlatılan görevlerin yer aldığı taleplerin çalışması uzun sürse bile, diğer isteklerin bloke olmadan cevaplanabileceğini öğrenmiş olduk. Bu bilgiler çerçevesinde yüksek performanslı, eş zamanlı talep karşılama yeteneklerine sahip ve talep için paralel görevler icra edebilen servislerin Node.js ile kolayca geliştirilebileceğini ifade edebiliriz.

Ben aynı durumu.Net Core tarafında da deneyimlemeye çalışacağım. Nitekim o tarafta da bu tip geliştirmeler yapmak mümkün. Ayrıca melez çözümler de uygulanabilir. Örnek senaryomuzdaki Player ve Team servisleri pekala farklı teknolojiler ile geliştirilmiş REST API servisleri olabilirler. Hatta MongoDb dışında veri depolama aygıtlarını da kullanabiliriz. Player servisinin MySQL ile konuşan Scala ile yazılmış bir uygulama olduğunu, Team servisinin de MongoDB ile yürüyen.Net Core ile yazılmış başka bir servis olduğunu düşünün (Hatta düşünmeyin kendi denemelerinizde bu kurguyu çalışın) Servis sayıları arttırılabilir ve çeşitlendirilebilir. Bir açıdan n sayıda microservice önüne Node.js ile yazılmış bir MainServer'ı koyduğumuzu da düşünebiliriz. Konuyu derinlemesine araştırmakta yarar var. Ancak gözlerim iyiden iyiye kapanmak üzere. Dolayısıyla müsadenizi istemek durumundayım. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

Örneği [github üzerinden](https://github.com/buraksenyurt/nodejs-tutorials/tree/master/Day07) indirebilirsiniz.
