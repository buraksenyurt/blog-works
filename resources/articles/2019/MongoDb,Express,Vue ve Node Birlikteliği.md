---
title: "MongoDb,Express,Vue ve Node Birlikteliği"
pubDate: 2019-07-19 05:47:00
categories:
  - Vue.js
tags:
  - Vue.js
  - Bash
  - JavaScript
  - MongoDB
  - JSON
  - Web API
  - HTTP
  - Node.js
  - Vue
  - GitHub
---

# MongoDb,Express,Vue ve Node Birlikteliği
![cobolll.png](images/cobolll.png)

Aranızdan kaç kişi akıllı telefonundaki herhangi bir arkadaşının numarasını ezbere söyleyebilir? Eminim bazılarımız bizi dokuz ay karnında taşıyan annesinin telefonunu dahi hatırlamıyordur. Peki ya birlikte sıklıkla vakit geçirdiğiniz ama çok da yakın çevrenizden olmayan kankanızın doğum günü ne zaman? Teknolojik cihazlarınızdaki hatırlatıcılar olmadığında kaç arkadaşınızın doğum gününü unutacaksınız hiç düşündünüz mü?

İletişim bilgisi ve doğum günleri bizi yakın çevremize bağlayan veya uzağı yakın eden unsurlar arasında yer alıyor. Hatırlanmak güzel olduğu kadar hatırlamak da gerekiyor. Ortaokul sıralarında kullandığım bir fihrist defterim vardı. İçinde yakın arkadaşlarımın ev telefonları ve doğum günü bilgileri yazardı. Pek tabii çok sık iletişimde olduğum sıra arkadaşım sevgili Burak Gürkan gibi dostlarımı aramak için o deftere ihtiyacım yoktu. Neredeyse her gün telefonla konuştuğumuz için numarayı ezberlemiştim.

Aradan yıllar geçti ve Yıldız Teknik Üniversitesi Matematik Mühendisliği bölümünü kazandım. Okulun ikinci yılındaki bilgisayar programlama dersinde Cobol görüyorduk ve dönem sonuna doğru hocamızla birlikte yaptığımız o uzun metrajlı çalışmanın konusu fihrist defteriydi (O yıl Cobol ile ilk ve son karşılaşmam olur diye düşünsem de hayatımın ilerleyen yıllarında iki kez karşıma çıkarak bende hoş anıların oluşmasına neden olacaktı) Bu kez arkadaşlarımızın iletişim bilgilerini ve doğum günlerini tutmak için 3.5 inçlik floppy diskten, 1ler ve 0lardan yararlanacaktık. Her ne kadar o zamanlar için heyecan verici bugün içinse çok sıradan bir örnek olsa da benim için geçmişe yapılan manevi bir yolculuk. Dolayısıyla zaman zaman bu örnek konsepti bir şeyleri öğrenmeye çalışırken kullanıyorum. İşte sıradaki [saturday-night-works birinci faz](https://github.com/buraksenyurt/saturday-night-works) derlememizin konusu da bir fihrist. Gelin hiç vakit kaybetmeden notlarımızı toparlamaya başlayalım.

Amacım başlıkta geçen enstrümanları kullanarak Web API tabanlı basit bir web uygulaması geliştirmekti. Veriyi tutmak için MongoDB'yi, sunucu tarafı için Node.js'i, Web Framework amacıyla express'i ve önyüz geliştirmesinde de Vue'yu kullanmak istemiştim. Kobay olarakta doksanlı yıllardan aklıma gelen ve Cobol öğretirlerken gösterdikleri Fihrist örneğini seçtim (O vakitler sanırım hepimiz öğrendiğimiz dillerle arkadaşlarımızın telefon numaralarına yer verdiğimiz bir fihrist uygulaması yazmışızdır) Özellikle Vue tarafında bileşen (component) geliştirmenin nasıl yapılabileceğini, bunlar arasındaki haberleşmenin nasıl tesis edileceğini merak ediyordum. İşin içerisine WebPack de girince güzel bir çalışma alanı oluştu diyebilirim.

## Projenin İskeleti

Projenin genel iskelet yapısı ve kullanacağımız dosyaları aşağıda görülen hiyerarşide oluşturabiliriz.

```text
Fihrist/
|----- app/
|----- config.js (Mongo bağlantısı gibi ortam parametrelerini tuttuğumuz konfigurasyon modülü)
|----- Routers.js (HTTP Get,Post,Put,Delete operasyonlarını üstlenen WebAPI tarafı)
|----- Contact.js (mongodb tarafı için kullanılan entity modeli)
|----- public/
|----- src/
|------------ bus.js (vue component'leri arasındaki iletişimi sağlayan event bus dosyası)
|------------ main.js (vue tarafının giriş noktası)
|------------ vue.js (vue npm paketi yüklendikten sonra dist klasöründen alınıp buraya kopyalanmıştır)
|------------ components/ (vue componentlerini tuttuğumuz yer)
|----------------- createContact.vue (yeni bir bağlantı eklemek için kullanılan bileşen)
|----------------- contacts.vue (tüm kontak listesini gösteren bileşen)
|----------------- app.vue (ana vue bileşeni)
|----- index.html (kullanıcının etkileşimde olacağı ana sayfa)
|----- server.js (sunucu tarafı)
|----- webpack.config.js (webpack build işleminin kullandığı konfigurasyon dosyası)
```

app klasöründe model sınıf, yönlendirme paketi ve bir konfigurasyon dosyası yer alıyor. public klasöründe HTML ve gerekirse CSS, image gibi öğlere ve vue uygulamasının kendisine yer veriliyor. server.js tahmin edileceği üzere node server rolünü üstleniyor.

## Gerekli Kurulumlar

Sistemde node, npm ve mongodb'nin yüklü olduğunu varsayıyoruz. Buna göre kök klasörde,

```bash
npm init
```

ile başlangıcı yapıp gerekli paket kurulumlarını tamamlayabiliriz.

```bash
npm install body-parser express mongoose morgan
```

JSON bazlı servise ait mesaj gövdelerini kolayca parse etmek için body-parser, HTTP sunucusu ve servis taleplerinin karşılanması için express, mongodb veri tabanı ile konuşmak için mongoose, mongo loglarını console tarafından izleyebilmek için morgan paketlerini kullanıyoruz. Buna göre server.js dosyasının içeriğini aşağıdaki gibi kodlayabiliriz.

```javascript
var express = require('express')
var morgan = require('morgan')
var path = require('path')
var app = express()
var mongoose = require('mongoose')
var bodyParser = require('body-parser')
var config = require('./app/config')
var router = require('./app/router')

mongoose.connect(config.conn, { useNewUrlParser: true }) // konfigurasyon dosyasındaki bilgi kullanılarak mongoDb bağlantısı tesis edilir

// static dosyaların public klasöründen karşılanacağı belirtilir
app.use(express.static(path.join(__dirname, '/public')))

// Middleware katmanına morgan'ı enjekte ederek loglamayı etkinleştirdik
app.use(morgan('dev'))

// İstemci taleplerinden gelecek Body içeriklerini JSON formatından kolayca ele almak için 
// Middleware katmanına body-parser modülünü ekledik
app.use(bodyParser.json())
app.use(bodyParser.urlencoded({ extended: true }))

var port = config.default_port || 8080 // port bilgisi belirlenir. config'de varsa 5003 yoksa 8080

app.listen(port)
app.use('/api', router) // /api adresine gelecek taleplerin router modülü tarafından karşılanacağı belirtilir.

app.get('/', function (req, res, next) {
    res.sendFile('./public/index.html') // eğer / adresine talep gelirse (yani http://localhost:5003/ şeklinde) index.html sayfasına yönlendiriyoruz
})

console.log('Sunucu hazır ve dinlemede')
```

Sunucu tarafında kullandığımız yardımcı modüllerimiz olacak. Web API tarafı için router.js'i ve MongoDb veri tabanı bağlantısını konfigurasyon dosyasından beslememizi sağlayan config.js ki içeriğini aşağıdaki gibi oluşturabiliriz.

```text
// genel konfigurasyon ayarlarımız
// veritabanı bağlantısı, sunucu için varsayılan port bilgisi vs
module.exports={
    conn:'mongodb://localhost:27017/fihristim', 
    default_port:5003 
}
```

Web API görevini üstlenen router.js'in temel görevi CRUD operasyonları için HTTP desteği sunmak (Read için Get, Create için Post vb) Her ne kadar ilişkisel bir veri tabanı kullanmıyor olsak da, Mongo tarafındaki koleksiyon için bir şema bilgisi tanımlamamız gerekiyor. Yani bir model oluşturmalıyız. Bunun için contact isimli modülü oluşturabiliriz. Arkadaşımızın tam adını, telefon numarasını, yaşadığı yeri ve doğum tarihi bilgilerini tutan bu modelde sadece String ve Date veri türlerine yer vermiş olsak da siz veri farklı tiplerle yapıyı pekala zenginleştirebilirsiniz.

```javascript
// MongoDb'de koleksiyonunun karşılığı olan model tanımı
var mongoose = require('mongoose')

// contact isimli bir şemamız var
// örnek olması açısından bir kaç özellik içeriyor
var contact = new mongoose.Schema({
    fullname: { type: String },
    phoneNumber: { type: String },
    location: { type: String },
    birtdate: { type: Date }
},
    {
        collection: 'contacts' // kontaklarımızı tuttuğumuz koleksiyon
    }
)

module.exports = mongoose.model('Contact', contact)
```

Bu şemayı kullanan ve esas itibariyle CRUD operasyonlarının karşılığı olan servis taleplerini ele alan router dosyasının içeriğini de aşağıdaki gibi geliştirerek devam edebiliriz.

```javascript
// Web API Router sınıfımız
// gerekli modülleri tanımlıyoruz
var express = require('express')
var operator = express.Router()
var contact = require('./contact')

// HTTP Post ile yeni bir contact eklenmesini sağlıyoruz
operator.route('/').post(function (req, res) {
    // request body'den gelen değerlere göre contact oluşturuluyor
    contact.create({
        fullname: req.body.fullname,
        phoneNumber: req.body.phoneNumber,
        location: req.body.location,
        birtdate: req.body.birtdate
    }, function (e, c) { //callback fonksiyonu
        if (e) { //hata oluşmuşsa HTTP 400 döndük
            res.status(400).send('kayıt işlemi başarısız')
        }
        res.status(200).json(c) // Hata yoksa HTTP 200 Ok dönüyor ve cevabın içine oluşturulan contact nesnesini gömüyoruz
    }
    )
})

// HTTP Get talebi için tüm kontakların listesini dönüyoruz
operator.route('/').get(function (req, res, next) {
    contact.find(function (e, contacts) {
        if (e) { //hata varsa sonraki fonksiyona bunu yollar
            return next(new Error(e))
        }
        res.json(contacts) // hata yoksa tüm kontaları json serileştirip döner
    })
})

// Belli bir ID'ye ait kontak bilgisini döndürür
// HTTP Get ile çalışır
// Querystring'teki id kullanılır
operator.route('/:id').get(function (req, res, next) {
    var id = req.params.id //id parametresinin değeri alınır
    contact.findById(id, function (e, c) {
        if (e) //hata varsa kayıt bulunanamış diyebiliriz
        {
            return next(new Error('Bu ID için bir kontak bilgisi mevcut değil'))
        }
        res.json(c)
    })
})

// ID bazlı kontak silmek için çalışan fonksiyon
// HTTP Delete kullanılır
operator.route('/:id').delete(function (req, res, next) {
    var id = req.params.id
    contact.findByIdAndRemove(id, function (e, c) {
        if (e) {
            return next(new Error('Bu ID için bir kontak bulunamadığından silme işlemi yapılamadı'))
        }
        res.json('Başarılı bir şekilde silindi')
    })
})

// Güncelleme işlemi
// HTTP Put kullanılır
operator.route('/').put(function (req, res, next) {
    var id = req.body.id
    // önce id'den contact bulunur
    contact.findById(id, function (e, c) {
        if (e) {
            return next(new Error('Güncellenme için bir kayıt bulunamadı'))
        } else { //bulunduysa özellikler body'den gelenler ile değiştirilir

            c.fullname = req.body.fullname ? req.body.fullname : c.fullname
            c.phoneNumber = req.body.phoneNumber ? req.body.phoneNumber : c.phoneNumber
            c.location = req.body.location ? req.body.location : c.location
            c.birtdate = req.body.birtdate ? req.body.birtdate : c.birtdate

            // contact yeni haliyle kayıt edilir
            c.save()
            res.status(200).json(c)
        }
    })
})

module.exports = operator
```

server.js ve App klasörü içindeki dosyalarımız hazır. An itibariyle sunucu tarafını çalıştırabilir ve çeşitli servis çağrıları gereçekleştirerek Mongo üzerinde CRUD operasyonlarını deneyimleyebiliriz. Sunucu tarafını başlatmak için terminalden

```bash
npm start
```

komutunu vermek yeterli. Tabii bu aşamada MongoDb'nin de çalışır olduğundan emin olmak lazım. mongod ile mongodb servisini başlatabiliriz. Sonrasında veri tabanı üzerindeki operasyonlar için mongo komutunu kullanarak arabirim haberleşmesini de açabiliriz. Eğer mongod ile servis başarılı bir şekilde çalışırsa 27017 port veri tabanı haberleşmesi için aktif hale gelecektir. Sonrasında örneğin db komutunu kullanılabilir ve örneğin veri tabanlarını listeyebiliriz.

```bash
mongod
mongo
db
```

![06_13_credit_1.png](images/06_13_credit_1.png)

## Servis Testleri

Servis tarafının işlerliğini kontrol etmek için curl aracıyla aşağıdaki denemeler yapılabilir. Ben denemeler sırasında en yakın arkadaşlarımdan dördünü ekledim. Sonrasında listeleme, belli bir id'ye bağlı kişi çekme, bilgi güncelleme ve silme operasyonlarını icra ettim.

```bash
curl -H "Content-Type: application/json" -X POST -d '{"fullname":"M.J.","phoneNumber":"555 55 23","location":"chicago","birtdate":"1963-05-18T16:00:00Z"}' http://localhost:5003/api

curl -H "Content-Type: application/json" -X POST -d '{"fullname":"Çarls Barkli","phoneNumber":"555 55 34","location":"phoneix","birtdate":"1963-05-18T16:00:00Z"}' http://localhost:5003/api

curl -H "Content-Type: application/json" -X POST -d '{"fullname":"meycik cansın","phoneNumber":"555 55 32","location":"los angles","birtdate":"1959-05-18T16:00:00Z"}' http://localhost:5003/api

curl -H "Content-Type: application/json" -X POST -d '{"fullname":"leri börd","phoneNumber":"555 55 33","location":"boston","birtdate":"1956-05-18T16:00:00Z"}' http://localhost:5003/api

curl http://localhost:5003/api

curl http://localhost:5003/api/5c29222522433f0234e71e1b

curl -H "Content-Type: application/json" -X PUT -d '{"id":"5c29222522433f0234e71e1b","fullname":"maykıl cordın"}' http://localhost:5003/api

curl -X DELETE http://localhost:5003/api/5c29222522433f0234e71e1b
```

Belli dokümanlar için kullanılan ve MongoDb tarafından otomatik olarak üretilen ID değerleri elbette siz kendi denemelerinizi yaparken farklılıklar gösterecektir.

## Front-End Tarafı

Servis tarafı hazır. Elimizde veri kaynağı ile haberleşen ve temel işlemleri gerçekleştiren bir sunucu mevcut. Şimdi bu servisle konuşan arayüz uygulamasını tasarlamaya başlayalım. Tüm front-end enstrümanları public klasörü altında konuşlanmış durumdadır. Javascript ve css öğelerini tek bir paket haline getirmek için WebPack'ten faydalanacağız. Vue tarafındaki HTTP çağrıları için istemci olarak axios paketini kullanacağız. Gereken paket kurulumları için terminalden şöyle ilerleyebiliriz.

```bash
npm install babel-core babel-loader@7 babel-preset-env babel-preset-stage-3 css-loader vue-loader vue-template-compiler webpack webpack-dev-server bootstrap
```

Vue tarafındaki ana bileşenimiz app.vue dosyasında bulunuyor. Kendi içinde iki alt bileşene sahip. Bir kontak eklemek için kullanacağımız newContact.vue ve listeleme için ele alacağımız contacts.vue. Bu bileşenleri aşağıdaki gibi kodlayabiliriz.

newContact.vue

```text
<template>
  <div>
    <h1>Yeni Bağlantı</h1>
    <form>
      <div class="form-group">
        <label>Fullname</label>
        <input
          type="text"
          class="form-control"
          aria-describedby="inputGroup-sizing-default"
          placeholder="nasıl isimlendirirsin?"
          v-model="contact.fullname"
        >
      </div>
      <div class="form-group">
        <label>Phone</label>
        <input
          type="text"
          class="form-control"
          aria-describedby="inputGroup-sizing-default"
          placeholder="nereden ulaşırsın?"
          v-model="contact.phoneNumber"
        >
      </div>
      <div class="form-group">
        <label>Location</label>
        <input
          type="text"
          class="form-control"
          aria-describedby="inputGroup-sizing-default"
          placeholder="nerede yaşıyor?"
          v-model="contact.location"
        >
      </div>
      <div class="form-group">
        <label>Birthdate</label>
        <input
          type="text"
          class="form-control"
          aria-describedby="inputGroup-sizing-default"
          placeholder="1976-04-12T11:35:00Z"
          v-model="contact.birtDate"
        >
      </div>
      <div class="form-group">
        <button type="button" class="btn btn-primary" @click="createContact($event)">Kaydet</button>
      </div>
    </form>
  </div>
</template>

<script>
import axios from "axios"; // API servis haberleşmesi için
import bus from "./../bus.js"; // bileşenler arası haberleşme için
// HTML elementlerindeki input kontrollerinde dikkat edileceği üzere v-model attribute'ları kullanıldı. Bunlar modelimizin özellikleri.

export default {
  data() {
    return {
      contact: {
        fullname: "",
        phoneNumber: "",
        location: "",
        birtDate: ""
      }
    };
  },
  methods: {
    createContact(event) {
      //Button'un @click niteliğinde yüklenen olay metodu
      if (event) event.preventDefault();
      let url = "http://localhost:5003/api";
      let param = {
        //parametre değerleri input kontrollerinden geliyor
        fullname: this.contact.fullname,
        phoneNumber: this.contact.phoneNumber,
        location: this.contact.location,
        birtDate: this.contact.birtDate
      };
      axios
        .post(url, param) //HTTP Post çağrısını gönderdik
        .then(response => {
          console.log(response); // tarayıcının developer tool kısmından log takibi için. Canlı ortamda kullanmaya gerek yok.
          this.clear();
          this.refresh(); // yeni bir bağlantı oluşturlduğunda refresh metodu çağırılır
        })
        .catch(error => {
          console.log(error);
        });
    },
    clear() {
      this.contact.fullname = "";
      this.contact.phoneNumber = "";
      this.contact.location = "";
      this.contact.birtDate = "";
    },
    refresh() {
      bus.$emit("refresh"); // metod evenbus'a bir olay fırlatır. refresh isminde. bunu diğer bileşende yakalayarak bağlantılar listesini anında günelleyebiliriz
    }
  }
};
</script>
```

contacts.vue bileşenimiz

```text
<template>
  <div>
    <div class="col-md-12" v-show="contactList.length>0">
      <h3>Tüm bağlantılarım</h3>
      <div class="row mrb-10" v-for="contact in contactList" :key="contact.id">
        <div class="card bg-light border-dark mb-3" style="width: 18rem;">
          <div class="card-body">
            <h5 class="card-title">{{ contact.fullname }}</h5>
            <p class="card-text">{{ contact.phoneNumber }}</p>
            <p class="card-text">{{ contact.location }}</p>
            <p class="cart-text">{{ contact.birtdate }}</p>
            <span v-on:click="deleteContact(contact._id)" class="btn btn-primary">Sil</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import axios from "axios";
import bus from "./../bus.js";

export default {
  data() {
    return {
      contactList: [] //modelimizin verisini içerecek array elemanı
    };
  },
  created: function() {
    // başlangıçta çalışacak fonksiyonumuzda iki işlem yapılıyor
    this.getAllContacts(); // tüm bağlantıları al
    this.listenToBus(); // ve diğer bileşenden yeni eklenecek bağlantıları alabilmek için eventBus'ı dinlemeye başla
  },
  methods: {
    getAllContacts() {
      let uri = "http://localhost:5003/api"; // klasik axios ile web api'mize HTTP Get talebi gönderdik
      axios.get(uri).then(response => {
        this.contactList = response.data; //dönen veriyi contactList dizisine aldık
        console.log(this.contactList);
      });
    },
    deleteContact(id) {
      // bir arkadaşımızı silmek istediğimizde
      let uri = "http://localhost:5003/api/" + id;
      axios.delete(uri); // HTTP Delete talebini gönderiyoruz. id parametresi Mongo'nun ürettiği Guid
      this.getAllContacts(); // listemizi tazeleyelim
    },
    listenToBus() {
      bus.$on("refresh", $event => {
        this.getAllContacts(); // Diğer bileşen tarafından yeni bir bağlantı eklenirse dinlediğimiz refresh isimli hattan bunu yakalayabileceğiz. Bu uyarı sonrası bağlantı listesini tekrar çekiyoruz
      });
    }
  }
};
</script>
```

> Özellikle yukarıdaki iki bileşenin kullandığı bus isimli modüle dikkat etmek lazım. Vue tarafında bileşenler arasında olay bazlı çalışan bir eventbus modeli ile iletişim sağlanabilir. Bu sayede bir bileşende yapılan değişiklikleri başka birisine göndermek mümkündür. Örneğimizdeki newContact bileşeninde refresh isimli bir olay tetiklenir (Yeni bir kontak eklendiğinde çalıştırıyoruz) Contacts bileşeni de refresh isimli olayı dinlemektedir. Dolayısıyla yeni bir kontak bilgisi eklediğimizde diğer bileşen otomatik olarak güncel kontak listesini çekecektir.

ve son olarak app.vue isimli ana bileşenimiz.

```text
<template>
  <div id="app">
    <div class="container">
      <div class="row col-md-6 offset-md-3">
        <create-contact></create-contact>
        <!-- createContact bileşeni buraya yerleşecek diğeri de aşağıya -->
        <contacts></contacts>
      </div>
    </div>
  </div>
</template>

<script>
import createContact from "./newContact.vue"; // yeni bağlantı eklediğimiz bileşeni aldık
import contacts from "./contacts.vue"; // contact bileşenini aldık
export default {
  name: "app",
  data() {
    return {};
  },
  components: { createContact, contacts } // bileşenlerimizi tanıttık
};
</script>
```

Frontend tarafı geliştmeleri bittikten sonra bir build işlemi ile mypackage.js dosyasını oluşturacağız. Bu içerik Vue bileşenlerinin de sunulacağı index.html dosyasında referanslanacak. Çok sade bir HTML içeriği elde edeceğimizi ifade edebilirim:) Tabii burada build süreci için gerekli bir webpack.config.js dosyasına ihtiyaç var. İlgili dosyayı aşağıdaki gibi kodlayabiliriz.

```javascript
const VueLoaderPlugin = require('vue-loader/lib/plugin');

module.exports = {

    entry: './public/src/main.js',
    output: {
        filename: './public/build/mypackage.js'
    },
    resolve: {

        alias: {
            vue: './vue.js'
        }
    },
    module: {
        rules: [
            {
                test: /\.css$/,
                use: [
                    'vue-style-loader',
                    'css-loader'
                ]
            }, {
                test: /\.vue$/,
                loader: 'vue-loader',
                exclude: /node_modules/,
                options: {
                    loaders: {
                    }
                }
            },
            {
                test: /\.js$/,
                loader: 'babel-loader',
                exclude: /node_modules/
            }
        ]
    },
    plugins: [
        new VueLoaderPlugin()
    ],
    devServer: {
        port: 3000
    }
}
```

Build sonrası oluşan paket için bir takım bildirimlere yer verilmektedir. Giriş noktası olarak src klasöründeki main.js tanımlanmıştır. Build sonrası çıktı output sekmesinde belirtilen ortama yapılacaktır. Devam eden kısımlarda css, vue vs js formatlı dosyalar için henüz ne anlama geldiklerini öğrenemediğim kural setleri vardır. Dosya tamamlandıktan sonra build işlemine geçilebilir. Aşağıdaki terminal komutunu bu amaçla kullanabiliriz.

```bash
npm run build
```

Eğer sorunsuz bir şekilde (sorunsuz diyorum çünkü webpack.config.js dosyasını ayarlarken epey problem yaşadım) build işlemi gerçekleşirse dist/public/build/mypackage.js şeklinde bir dosya oluşur. Bu bohçayı index.html public klasörüne alıp aşağıdaki gibi kullanıma açabiliriz.

```text
<!DOCTYPE html>
<html>

<head>
    <meta charset="utf-8">
    <title>Benim en iyi arkadaşlarım</title>
</head>

<body>
    <app></app>
    <script src="./build/mypackage.js"></script>
</body>

</html>
```

Hepsi bu kadar;) Yazdığımız uygulamayı test etmek için node ve mongodb sunucularını çalıştırmamız ve http://localhost:5003/ adresine gitmemiz yeterli. Malum burası varsayılan olarak index sayfasına yönlendirilmekte (Nereden yapıldığını hatırlıyor musunuz?) index.html içinde Vue kodlarımızı paketlediğimiz mypackage.js dosyası referans edildiğinden ilgili bileşenler de buraya render edilecek.

> Build sonrası bileşenlerde bir değişiklik yapılırsa tekrardan build işleminin çalıştırılması ve bundle paketinin kullanıma alınması gerekir.

Ekleme işlemi ile ilgili ilk test sonucu aşağıdaki gibidir.

![06_13_credit_2.png](images/06_13_credit_2.png)

Listeleme ve listenen bağlantıları silme işini contacts.vue isimli bileşen üstlenmekte. Buda app.vue içerisinde tanımlanıp sayfaya yerleştiriliyor. contacts.vue içerisinde bootstrap card stillerini kullanmayı tercih ettim. Çok berbat bir tasarım olmadı ama çok iyi de olmadı. Fonksiyonel olarak bağlantıları listeletebiliyor, silme ve yenilerini ekleme işlemlerini gerçekleştirebiliyoruz.

> Update işlemi içinde buraya bir şeyler eklemek lazım. Mesela bir link'le farklı bir adrese yönlendirilme ve onun üstünden güncelleme işlemlerinin yapılması sağlanabilir. Bu kutsal görevi......:D

İşte örnek bir ekran görüntüsü daha...

![06_13_credit_3.png](images/06_13_credit_3.png)

## Ben Neler Öğrendim?

Olayın başlangıç noktası olan index.html son derece sadedir. İçinde bootstrap ve vue bileşenleri gibi gerekli diğer kütüphaneleri barındırmaktadır. Bu sadeliği webpack sağlıyor ama bu ifadem mutlaka webpack'i kullanın anlamına gelmemeli. Farklı alternatif bundler araçları da mevcut. Benim bu çalışma sonrası öğrendiklerim ise şöyle;

- Vue'da component nasıl geliştirilir
- template üzerinde model kullanımı nasıldır
- axios ile HTTP Post,Get,Put gibi metodlar nasıl çağrılır
- webpack.config dosyası nasıl hazırlanır
- bir bileşenden eventbus'a bildirim nasıl yapılır ve diğer bileşenlerden bu değişiklik nasıl yakalanır
- Bootstrap Card'ları nasıl kullanılır

Böylece geldik bir [cumartesi gecesi çalışması](https://github.com/buraksenyurt/saturday-night-works)na ait derlemenin daha sonuna. [13 numaralı örnek](https://github.com/buraksenyurt/saturday-night-works/tree/master/No%2013%20-%20MEVN%20Sample)le geçmişe bir yolculuk daha yaptım. Tekrardan okuyunca unuttuğum bir çok noktayı yeniden hatırladığım bir yazı olduğu için kendi adıma kardayım. Umarım sizler için de faydalı olmuştur. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
