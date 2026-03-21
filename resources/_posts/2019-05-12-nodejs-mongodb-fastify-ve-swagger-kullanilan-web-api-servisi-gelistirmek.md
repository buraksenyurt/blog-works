---
layout: post
title: "Node.js, MongoDB, Fastify ve Swagger Kullanılan Web API Servisi Geliştirmek"
date: 2019-05-12 21:45:00 +0300
categories:
  - nodejs
tags:
  - node.js
  - mongodb
  - fastify
  - swagger
  - web-api
  - rest-api
  - web-service
  - postman
  - nodemon
  - async
  - routers
---
Yazılım tarafında yeni bir şeyler öğrenmeye çalışmak hayatımın standart ritüelleri arasında. Bu döngü içerisinde yaşamak en büyük keyiflerimden birisi. Tabii bu döngünün en önemli parçalarından birisi masabaşında yapılan kodlama çalışmaları. WestWorld ve son zamanlardaki gözdem Ahch-To başlıca yardımcılarım. Çalışmalar değişik diyarlardan geliyor. Bazen konular arasında keskin geçişler yapıyorum. Bir gün Node.js dünyasında debelenirken bir başka gün daha aşina olduğum.Net Core kıyılarında yürüyüşe çıkıyorum.

![minions2.png](/assets/images/2019/minions2.png)

Ancak konular ne kadar değişirse değişsin bazı şeyler hep aynı kalıyor. Bu sebepten kullandığım örneklerdeki veri odaklı varlıklar zamanla tekrar önüme geliyor. Star Wars gezegenleri, ünlü düşünürlerin özlü sözleri, yapılacaklar listesindeki maddeler, emektar Northwind ve AdventureWorks veri tabanları, müzik gruplarının sevilen albümleri, Marvel karakterleri, basketbol yıldızları ve Minion'lar:) İşte yine onlarla karşı karşıyayım. Bu sefer eski örneklerden birisini masaya yatırmaya karar verdiğimde rastladım onlara.

Cumartesi geceleri çalışmaları kapsamında ele aldığım [07 numaralı örnekteki amacım](https://github.com/buraksenyurt/saturday-night-works/tree/master/No%2007%20-%20NoMoFaS) MongoDB kullanan Node.Js tabanlı basit bir Web API servisi geliştirmekti. Ancak bunu yaparken web framework olarak sıklıkla kullandığım express yerine fastify paketini tercih etmiştim. Ayrcıa web api tarafından sunulan operayonların geliştirici dostu bir arayüzle sunulması için Swagger'dan yararlandım (Web API geliştiricilerinin artık olmazsa olmazlarından diyebiliriz) Örneği Visual Studio Code yardımıyla geliştirdiğim WestWorld'de (Ubuntu 18.04 64bit) Node.js, npm (Node paket yönetimi aracı) ve MongoDB (NoSQL veri tabanımız) yüklüydü. Bu örneğe ait notların üstünden bir kez daha geçerek bilgilerimi yeniden hatırlama fırsatı bulmuş oldum.

> MongoDB'yi Ubuntu sistemine kurmak için [şu adresteki](https://docs.mongodb.com/manual/tutorial/install-mongodb-on-ubuntu/) bilgilerden yararlanabiliriz. Ama isterseniz MongoDB'nin konu ile ilgili [docker imajını da](https://hub.docker.com/_/mongo) ele alabilirsiniz.

## Klasör Ağacı ve Paketler

Uygulamanın klasör yapısını ilk etapta aşağıdaki gibi kurguladım. Çok basit anlamda bir MVC (Model View Controller) deseni olduğunu varsayabiliriz. Her ne kadar ortada view isimli bir klasör olmasa da, yönlendirme işlemlerinin ele alındığı routes bu anlamda düşünülebilir. Son satırda yer alan npm init komutu ile node operasyonu başlatılmış oluyor.

```bash
mkdir Minion-API
cd Minion-API
mkdir src
cd src
mkdir models
mkdir controllers
mkdir routes
mkdir config
touch index.js
npm init
```

Uygulamanın pek tabii ihtiyaç duyduğu belli başlı paketler var. Bunları npm aracı ile aşağıdaki terminal komutu yardımıyla yükleyebiliriz.

```bash
npm i nodemon mongoose fastify fastify-swagger boom
```

nodemon'u kod dosyalarından birisinde değişiklik olduğunda node sunucusunu otomatik olarak yeniden başlatmak için kullanıyoruz. Özellikle geliştirme safhasında çok işe yarayan bir monitoring fonksiyonelliği olduğunu ifade edebilirim. Sürekli uygulamayı sonlandırıp yeniden başlatmaya gerek bırakmayan bir özellik. Bu arada kullanımı için package.json dosyasındaki start komutunu aşağıdaki gibi değiştirmemiz gerekiyor.

```javascript
"start": "./node_modules/nodemon/bin/nodemon.js ./src/index.js"
```

mongoose, mongodb ile konuşabilmek için gereken paketimiz. Fastify, Hapi ve Express'ten ilham alınarak yazılmış oldukça hızlı bir web framework olarak ifade edilmekte. İlk kez bu örnek çalışma kapsamında tanıştığımı itiraf edeyim. API dokümantasyonu için Fastify'a Swagger desteği veren Fastify-swagger modülü kullanılıyor. Fastify route tanımlamaları Swagger ile otomatik olarak ilişkilendirilecekler (Koddaki izleri takip edin) HTTP hata mesajlarını göstermek için boom isimli utility paketinden yararlanılıyor (Bu arada [ilgili paket](https://www.npmjs.com/package/boom) bir süre önce devre dışı bırakılmış. [Şu adresten güncel sürümüne](https://github.com/hapijs/boom) ulaşabiliriz)

## Kod Tarafı

Uygulama veri odaklı bir REST servis olarak özetlenebilir. Verinin tutulduğu taraf MongoDB. Popüler bir doküman bazlı NoSQL sistemi olduğunu biliyoruz. Verinin kod tarafında şemalar yardımıyla modellenmesi mümkün. Örneğe göre mongodb dokümanlarına ait şemaları models klasöründe tutuyoruz (minion.js) Veri ile ilgili ekleme, güncelleme, silme veya okuma gibi CRUD operasyonlarını controllers içerisinde karşılıyoruz. minioncontroller.js minion modeli ile ilgili Controller tipimiz. HTTP taleplerini ele aldığımız yer ise routes klasöründeki index.js dosyası. Bu dosya, HTTP taleplerini aldığında (örneğin yeni bir satır eklenmesi veya tüm listenin çekilmesi gibi) bunları Controller sınıfına iletmekte. Controller sınıfı da esasen MongoDb ve model sınıfı ile işbirliği içerisinde ilgili talepleri karşılamakta.

minion.js;

```javascript
const mongoose = require('mongoose')

// mini isimli şemayı tanımladık. 
// Minion filmindeki bir karakteri temsil ediyor
const minionSchema = new mongoose.Schema({
    nickname: String,
    age: Number,
    gender: String
})

module.exports = mongoose.model('Minion', minionSchema)
```

minioncontroller.js;

```javascript
const boom = require('boom') //bomba gibi bir hata mesajı yöneticisi
const Minion = require('../models/minion')

// yeni bir Minion karakteri eklemek için
exports.add = async (req, res) => {
    try {
        // Minion bilgilerini request'in body'sinden aldık
        const mini = new Minion(req.body)
        return mini.save() //kaydedip sonucu geriye döndürdük
    } catch (err) {
        throw boom.boomify(err)
    }
}

// bir Minion karakterini güncellemek için
exports.update = async (req, res) => {
    try {
        // güncelleme işlemini gerçekleştir
        const result = await Minion.findByIdAndUpdate(req.params.id, req.body, { new: true })
        return result
    } catch (err) {
        throw boom.boomify(err)
    }
}

// bir Minion karakterini silmek için
exports.delete = async (req, res) => {
    try {
        // query parametresi olarak gelen id'den ilgili Minion bul ve kaldır
        const result = await Minion.findByIdAndRemove(req.params.id)
        return result
    } catch (err) {
        throw boom.boomify(err)
    }
}

// id bilgisinden Minion bul
exports.getSingle = async (req, res) => {
    try {
        const result = await Minion.findById(req.params.id)
        return result
    } catch (err) {
        throw boom.boomify(err)
    }
}

// ne kadar Minion varsa geriye döndür
exports.getAll = async (req, res) => {
    try {
        const result = await Minion.find()
        console.log(result)
        return result
    } catch (err) {
        throw boom.boomify(err)
    }
}
```

routes/index.js;

```javascript
// controller tipini içeriye tanımladık
const minionController = require('../controllers/minionController')
const help = require('./swagger-help/minionApi') // swagger yardım dokümanının yeri söylendi

// HTTP Get, Post, Put, Delete tanımlamalarını yapıyoruz
const handlers = [
    {
        method: 'GET', // alt satırdaki adrese HTTP Get talebi gelirse
        url: '/api/minions',
        handler: minionController.getAll, //controller'daki getAll metoduna yönlendir
        schema: help.getAllMinionSchema
    },
    {
        method: 'GET', //alt satırdaki adrese HTTP Get talebi gelirse
        url: '/api/minions/:id',
        handler: minionController.getSingle //controller'daki getSingle metoduna yönlendir
    },
    {
        method: 'POST', //alttaki adres için POST talebi gelirse
        url: '/api/minions',
        handler: minionController.add, // yeni bir mini ekleme isteği nedeniyle controller'daki add metoduna yönlendir
        schema: help.addMinionSchema
    },
    {
        method: 'PUT', //aşağıdaki adres için PUT talebi gelirse
        url: '/api/minions/:id',
        handler: minionController.update //güncelleme sebebiyle update metoduna yönlendir
    },
    {
        method: 'DELETE', //aşağıdaki adres için HTTP Delete talebi gelirse
        url: '/api/minions/:id',
        handler: minionController.delete //miniyi silmek için controller'daki delete metodunu çağır
    }
]

module.exports = handlers // handlers isimli array'deki metodları modül dışına aç
```

Web API fonksiyonelliklerini hoş bir şekilde göstermek ve daha kullanışlı testler yaptırabilmek için Swagger ile ilgili ayarlamalar yapmak yerinde olur. Bunun için config klasöründeki swagger.js dosyasını kullanabiliriz.

```javascript
exports.options = {
    routePrefix: '/help',
    exposeRoute: true,
    swagger: {
      info: {
        title: 'Minions API',
        description: 'Minion ailesi ile ilgili yönetsel işlemler...',
        version: '1.0.0'
      },
      externalDocs: {
        url: 'https://swagger.io',
        description: 'Daha fazla bilgi için buraya gidin'
      },
      host: 'localhost',
      schemes: ['http'],
      consumes: ['application/json'],
      produces: ['application/json']
    }
  }
```

Dikkat edileceği üzere help bir adres öneki olarak belirtilmiş durumda (ama değiştirebilirsiniz) Yardım sayfasına ait başlık, açıklama ve servisin versiyon bilgileri info elementinde belirtiliyor. İstenirse servisle ilgili harici dokümantasyonlara yönlendirmelerde de bulunulabilinir. Bu, externalDocs isimli kısımda tanımlanmakta. Takip eden bölümlerde host, schema ve content type bilgileri belirtilmekte. Servisin ilgili operayonlarına yapılacak GET ve POST gibi çağrılara ait yardımcı bilgilerse routes/swagger-help klasöründeki js dosyası içerisinde yazıyor. Aşağıdaki örnek kod parçasına göre POST ve GET kullanımları için bazı tanımlamalar yapılmış durumda. Bu tanımlamalar yardım sayfasının önyüzüne yansıtılmakta.

```javascript
exports.addMinionSchema = {
    description: 'Yeni minionlar ekle',
    tags: ['minions'],
    summary: 'Minionlar ailesine yeni bir mini eklemek için',
    body: {
        type: 'object',
        properties: {
            nickname: { type: 'string' },
            age: { type: 'number' },
            gender: { type: 'string' }
        }
    },
    response: {
        200: {
            description: 'Eklendi',
            type: 'object',
            properties: {
                _id: { type: 'string' },
                nickname: { type: 'string' },
                age: { type: 'number' },
                gender: { type: 'string' },
                __v: { type: 'number' }
            }
        }
    }
}

exports.getAllMinionSchema = {
    description: 'Tüm minionlar',
    tags: ['minions'],
    summary: 'Tüm minionları getirmek için kullanılır',
    response: {
        200: {
            description: 'Liste başarılı bir şekilde çekilir',
            type: 'object',
            properties: {
                _id: { type: 'string' },
                nickname: { type: 'string' },
                age: { type: 'number' },
                gender: { type: 'string' },
                __v: { type: 'number' }
            }
        }
    }
}
```

Dosya bilgilerine göre localhost:4005/help adresine talepte bulunduğumuzda ekran görüntüsünde yer alan yardım sayfası ile karşılaşırız. Tam bir geliştirici dostu öyle değil mi?

![04_07_credit_2.png](/assets/images/2019/04_07_credit_2.png)

Pek tabii node.js uygulamasını ayağa kaldıran ana modüle ait kodlarımız da oldukça önemli. Proje iskeletine göre routes klasörü altındaki modülleri Fastify ile ilişkilendirmek gerekiyor. Bunun için bir forEach döngüsü kullanılmakta (Fastify'ın Swagger ile ilişkilendirildiği yeri görebildiniz mi?)

```javascript
//gerekli modüller yüklenir
const fastify = require('fastify')({ logger: true })
const routes = require('./routes') //route modüllerinin yeri söylendi
const swagger = require('./config/swagger') //swager konfigurasyonunun yeri söylendi
fastify.register(require('fastify-swagger'), swagger.options) // swagger, fastify için kayıt edildi
const mongoose = require('mongoose')

// routes klasöründeki tüm modülleri fastify ile ilişkilendiriyoruz
routes.forEach((route, index) => {
    fastify.route(route)
})

// mongodb'ye bağlanılıyor. minions isimli veritabanı yoksa oluşturulacaktır
mongoose.connect('mongodb://localhost/animation', { useNewUrlParser: true })
    .then(() => console.log('MongoDB ile iletişim kuruldu'))
    .catch(err => console.log(err))

// sunucu 4005 nolu porttan yayın yapacak.
// asenkron çalışır
const online = async () => {
    try {
        await fastify.listen(4005)
        fastify.swagger()
        fastify.log.info(`Sunucu ${fastify.server.address().port} adresi üzerinden dinlemede`)
    } catch (err) {
        fastify.log.error(err)
        process.exit(1)
    }
}
online()
```

Kodun devam eden kısmında mongodb ile bağlantı sağlanıyor. Sonrasındaysa online isimli bir fonksiyonun asenkron olarak çağırıldığını görüyoruz. listen metoduna yapılan isteğe göre uygulamamız sonlandırılıncaya kadar 4005 numaralı port üzerinden dinlemede kalacak. Herhangibir hata olması ihtimaline karşın bir try...catch bloğu kullanılıyor. Gelelim çalışma zamanına.

## Çalışma Zamanı Testleri

Elbette ilk olarak mongodb servisini çalıştırmak lazım. Ardından node uygulaması ayağa kaldırılabilir. İki ayrı terminal penceresi açılarak ilerlenebilir ki ben örneği bu şekilde denemiştim.

```bash
mongod
npm start
```

![04_07_credit_1.png](/assets/images/2019/04_07_credit_1.png)

Dikkat edileceği üzere ekrana gayet hoş log'lar da düşüyor. Testler için curl veya popüler araçlardan olan Postman kullanılabilir. Ben bu tip çalışmalarda servis çalışabilirliğini hızlı ve kolay bir şekilde test etmek için Postman veya SoapUI gibi araçlardan yararlanıyorum.

Örneğimize yeniden odaklanırsak;

Yeni bir minion eklemek için http://localhost:4005/api/minions adresine gövdesinde JSON formatında içeriğe sahip bir talep göndermek yeterli.

```json
{
"nickname":"Agnes Gru",
"age":5,
"gender":"Female"
}
```

Eklenen kayıtlara ait benzersiz ID değerleri tahmin edileceği üzere MongoDB tarafından otomatik olarak üretilmekte. ID değerleri veri silme ve güncelleme operasyonları için önemli arama kriterlerinden. Aşağıdaki ekran görüntüsünde üstteki çağrı sonuçlarını görebiliriz. Agnes başarılı bir şekilde eklenmiş durumda.

![04_07_credit_3.png](/assets/images/2019/04_07_credit_3.png)

Bir kaç minion daha ekledikten sonra bunların güncel listesini elde etmek için http://localhost:4005/api/minions adresine HTTP Get talebini yollamak yeterli. Belli bir minion'u elde etmek içinse MongoDb'nin verdiği ID bilgisini kullanabiliriz. Örneğin, http://localhost:4005/api/minions/5c1581e579140d6969b5951f talebi için şöyle bir sonuç dönebilir.

![04_07_credit_4.png](/assets/images/2019/04_07_credit_4.png)

Benzer şekilde aynı adresi PUT metodu ile kullanıp BODY kısmında yeni minion bilgilerini JSON formatında göndererek güncelleme işlemini de gerçekleştirebiliriz. Bu ve silme operasyonlarını örneği tamamlayıp denemenizi öneririm.

## Ben Neler Öğrendim?

Bu çalışmaya tekrardan dönmek benim için faydalı oldu. Sonuçta sürekli gelişen yazılım dünyasında bir şeylerin ucundan tutabilmek için geriye dönük çalışmaları arada bir hatırlamak gerekiyor. Ben bu yazı için aşağıdaki kazanımları elde ettiğimi not almışım.

- Web çatısı için express yerine Fastify'ı nasıl kullanabileceğimi
- nodemon'un çalışma zamanına getirdiği rahatlığı
- mongodb'de temel veri işlemlerinin node.js tarafında mongoose ile nasıl kodlanacağını
- Swagger ile API arayüzünün geliştirici dostu hale getirilmesini
- Postman ile basit REST testlerinin yapılmasını

Böylece geldik bir [Saturday Night Works](https://github.com/buraksenyurt/saturday-night-works) macerasının daha sonuna. Bu sefer [eski maceralardan birisini](https://github.com/buraksenyurt/saturday-night-works/tree/master/No%2007%20-%20NoMoFaS) bloguma not olarak düşmeye çalıştım. Birkaç ay öncesinden kalma bir çalışma olsa da örneğin üstünden bir kere daha geçmek, kodları yeniden çalıştırmayı denemek ve yazılanları incelemek unuttuklarımı hatırlamama yardımcı oldu. Sonuç olarak bu çalışma kapsamında node.js ile MongoDB bazlı bir CRUD API servisi geliştirmeye çalıştığımızı özetleyebiliriz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
