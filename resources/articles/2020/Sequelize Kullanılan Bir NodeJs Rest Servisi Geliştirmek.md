---
title: "Sequelize Kullanılan Bir NodeJs Rest Servisi Geliştirmek"
pubDate: 2020-05-27 20:23:00
categories:
  - Node.js
tags:
  - Node.js
  - Bash
  - JSON
  - JavaScript
  - PostgreSQL
  - MySQL
  - REST
  - HTTP
  - Docker
  - Async/Await
  - GitHub
---

# Sequelize Kullanılan Bir NodeJs Rest Servisi Geliştirmek
![hearthstone_2.jpg](images/hearthstone_2.jpg)

Bilgisayar ile ilk tanıştığım günden beri oyun oynamayı seven birisiyim. Tabii ilerleyen yıllarda buna vakit ayırmak benim için çok zorlaştı. Bu nedenle hep kendi devrimin efsane sayabileceğim oyunlarında takılı kaldım. Söz gelimi paraya kıyarak aldığım oyun bilgisayarıma (Hani şu acayip ekran kartları olan, bir sürü fan barındıran, renkli ışıklarıyla gece disko topuna dönüşen masaüstü canavarından bahsediyorum) taaa ikibinlerin başında ve öncesinde oynadığım Red Alert II ve Command & Conquer Generals oyunlarını yükleyip vakit geçirdim. Hani en en en yeni oynadığım oyun sanıyorum ki Hearthstone ve onda da herkes beni ezip duruyor diyebilirim:D Bende onu nerede kullanabilirim diye düşünürken bari kart ve kahramanlarını bir NodeJs servisine malzeme yapayım dedim.

Lakin epey zamandır Nodejs ile kod yazmamıştım. İşte bu öğretideki amacım Postgresql veritabanını kullanan bir REST servisini NodeJs ile geliştirmek. Kod tarafındaki Entity nesneleri ile Postgresql arasındaki ORM (Object Relational Mapping) katmanında Sequelize paketini kullanmayı öğrenmeye çalışıyorum.

Postgresql tarafı için sistemi kirletmemek adına Docker imajından yararlanabiliriz. Önce onun ayağa kaldırarak işe başlayalım derim. Aşağıdaki terminal komutlarında hem Postgresql docker container ayağa kalkıyor hem de gamedb isimli veritabanı oluşturuluyor.

```bash
sudo docker run --name London -e POSTGRES_PASSWORD=P@ssw0rd -p 5433:5432 -d postgres
docker exec -it London bash
psql -U postgres
Create Database gamedb;
```

## Şablonun Oluşturulması

Örneği Heimdall (Ubuntu 20.04) üzerinde geliştiriyorum. Senaryomuzda oyun kartı ve kahramanlarına ait bilgileri ekleyip listelememize izin veren bir servis geliştirmeye çalışacağız. İlk olarak aşağıdaki terminal komutlarını kullanıp nodejs ortamını hazırlayalım ve gerekli modülleri yükletelim.

```bash
mkdir hartstone
cd hartstone
npm init
touch index.js
npm install express body-parser sequelize sequelize-cli pg pg-hstore
```

Express modülü REST servis alt yapısını yazmak, body-parser HTTP taleplerini kolayca parse etmek, pg postgresql iletişimini kurmak, pg-hstore JSON verilerini hstore formatında serileştirebilmek (hstore Postgresql'e özgü olan key-value türünden bir kolon tipidir) için kullanılıyor. Dahil edilen sequelize aracı ise standart bir proje şablonu oluşturmak için ele alınmakta. Diğer yandan migration işlemlerinde de bu aracı kullanabiliyoruz. İşimizi kolaylaştıracak şablon için aşağıdaki init komutunu kullanmamız yeterli.

```bash
node_modules/.bin/sequelize init
```

Bu işlemle üç klasör oluşacaktır. Veritabanı ayarları config klasöründeki config.json dosyasında tanımlanır. Migration işlemlerinin bulunduğu kod dosyaları içinse migrations klasörü kullanılır. Entity türleri ise models klasöründe tutulmaktadır. Biz tabii ki kendi geliştirmelerimizi yapacağız.

## Peki Biz Bu Şablonda Neler Yapacağız?

İlk olarak config/config.json içeriğini postgresql kullanılacak şekilde ortam bazlı olarak (dev,test,prod için ayrı ayrı) düzenleyelim.

```json
{
  "development": {
    "username": "postgres",
    "password": "P@ssw0rd",
    "database": "gamedb",
    "host": "localhost",
    "port": 5433,
    "dialect": "postgres"
  },
  "test": {
    "username": "root",
    "password": null,
    "database": "database_test",
    "host": "127.0.0.1",
    "dialect": "mysql",
    "operatorsAliases": false
  },
  "production": {
    "username": "root",
    "password": null,
    "database": "database_production",
    "host": "127.0.0.1",
    "dialect": "mysql",
    "operatorsAliases": false
  }
}
```

Oyuna ait kahraman ve kart bilgilerini barındıracağımız tipleri models klasörü içerisinde inşa edebiliriz. Burada sequelize nesnesini nasıl kullandığımıza dikkat edin. Aslında Postgresql tarafındaki veri modeli ve ilişkileri tanımlıyoruz.

card.js

```javascript
module.exports = (sequelize, DataTypes) => {

    let Card = sequelize.define('Card', {
        name: DataTypes.STRING,
        description: DataTypes.STRING,
        attack: DataTypes.INTEGER,
        health: DataTypes.INTEGER,
        spell: DataTypes.INTEGER
    });

    Card.associate = function (models) {
        Card.belongsTo(models.Hero, {
            onDelete: "CASCADE",
            foreignKey: 'heroId'
        });
    };

    return Card;
}
```

hero.js

```javascript
module.exports = (sequelize, DataTypes) => {

    let Hero = sequelize.define('Hero', {
        name: DataTypes.STRING,
        info: DataTypes.STRING
    });

    Hero.associate = function (models) {
        Hero.hasMany(models.Card, {
            foreignKey: 'id',
            as: 'cards'
        });
    };

    return Hero;
}
```

Migrations klasöründe ahero-migration ve card-migration isimli javascript dosyalarını oluşturarak devam edelim. Burası tipik olarak migration işlemleri sırasında Up ve Down operasyonlarında çalışacak kodları içeriyor. Her iki model için ayrı up ve down operasyonları söz konusu olabilir. Bu nedenle ayrı dosyalarda konuşlandırılıyorlar.

ahero-migration.js

```javascript
module.exports = {

    up: (queryInterface, Sequelize) =>

        queryInterface.createTable('Heros', {
            id: {
                allowNull: false,
                autoIncrement: true,
                primaryKey: true,
                type: Sequelize.INTEGER,
            },
            name: {
                type: Sequelize.STRING,
                allowNull: false,
            },
            info: {
                type: Sequelize.STRING,
                allowNull: false,
            },
            createdAt: {
                allowNull: false,
                type: Sequelize.DATE,
            },
            updatedAt: {
                allowNull: false,
                type: Sequelize.DATE,
            },
        }),

    down: (queryInterface) =>
        queryInterface.dropTable('Heros'),
};
```

card-migration.js

```javascript
module.exports = {

    up: (queryInterface, Sequelize) =>

        queryInterface.createTable('Cards', {
            id: {
                allowNull: false,
                autoIncrement: true,
                primaryKey: true,
                type: Sequelize.INTEGER,
            },
            name: {
                type: Sequelize.STRING,
                allowNull: false,
            },
            description: {
                type: Sequelize.STRING,
                allowNull: false,
            },
            attack: {
                type: Sequelize.INTEGER,
                allowNull: false,
                defaultValue: 1,
            },
            health: {
                type: Sequelize.INTEGER,
                allowNull: false,
                defaultValue: 3,
            },
            spell: {
                type: Sequelize.INTEGER,
                allowNull: false,
                defaultValue: 1,
            },
            heroId: {
                type: Sequelize.INTEGER,
                onDelete: 'CASCADE',
                references: {
                    model: 'Heros',
                    key: 'id',
                    as: 'heroId'
                },
            },
            createdAt: {
                allowNull: false,
                type: Sequelize.DATE,
            },
            updatedAt: {
                allowNull: false,
                type: Sequelize.DATE,
            },
        }),

    down: (queryInterface) =>
        queryInterface.dropTable('Cards'),
};
```

Şimdi Controller isimli bir klasör oluşturup içerisine hero, card ve index dosyalarını ekleyelim. Burada ana klasördeki main.js içerisinde express vasıtasıyla yakalanan yönlendirmelerin karşılığı olan fonksiyonlara yer vermekteyiz. Her modelimiz için ayrı bir controller söz konusu.

hero.js

```javascript
const Hero = require('../models').Hero;

module.exports = {
    async getAll(req, res) {
        try {
            const heros = await Hero.findAll({});
            res.status(201).send(heros);
        }
        catch (e) {
            console.log(e);
            res.status(500).send(e);
        }
    },

    async create(req, res) {
        try {
            const hero = await Hero.create({
                name: req.body.name,
                info: req.body.info
            });
            res.status(201).send(hero);
        }
        catch (e) {
            console.log(e);
            res.status(400).send(e);
        }
    }

    // Update ve delete işlevleri eklenmeli
}
```

card.js

```javascript
const Hero = require('../models').Hero;
const Card = require('../models').Card;

module.exports = {
    async getAllByHero(req, res) {
        try {
            const hero = await Hero.findOne({
                where: {
                    id: req.params.heroId
                }
            });
            console.log(hero.name);

            if (hero) {
                const cards = await Card.findAll({
                    where: {
                        heroId: req.params.heroId
                    }
                })

                res.status(201).send(cards);
            }
            else {

                res.send(404).send("Hero and it's cards not found")
            }
        }
        catch (e) {
            console.log(e);
            res.status(500).send(e);
        }
    },

    async create(req, res) {
        try {
            const card = await Card.create({
                name: req.body.name,
                description: req.body.description,
                attack: req.body.attack,
                health: req.body.health,
                spell: req.body.spell,
                heroId: req.body.heroId
            });
            res.status(201).send(card);
        }
        catch (e) {
            console.log(e);
            res.status(400).send(e);
        }
    }

    // Update ve delete işlevleri eklenmeli
}
```

index.js

```javascript
const hero = require('./hero');
const card = require('./card');

module.exports = {
    hero,
    card
}
```

Şimdi minik bir kahve arası verebiliriz. Kahvemizi içip geldikten sonra ise routes isimli klasörü oluşturup içerisindeki index.js dosyasını aşağıdaki gibi kodlayabiliriz.

```javascript
const heroController = require('../controller').hero;
const cardController = require('../controller').card;

module.exports = (app) => {

    app.get('/game/api', (req, res) => {
        res.status(200).send({
            data: "Hartstone Oyun API servisi sürüm 1.0"
        })
    })

    app.get('/game/api/hero', heroController.getAll);
    app.post('/game/api/hero', heroController.create);

    app.get('/game/api/hero/:heroId/cards', cardController.getAllByHero);
    app.post('/game/api/card', cardController.create);
}
```

Ardından belki bir de çay molası verip dönüşte ana klasöre geçer ve index.js içeriğini aşağıdaki gibi değiştiririz. Main içerisine express paketi devreye giriyor. Express, yönlendirmeler için Routes klasöründeki index.js'i kullanmakta. O da doğru controller tiplerini...Dikkat edileceği üzere main içeriği oldukça sade ve anlaşılır. Kodun tamamını okurken Main'den aşağıya doğru inmeye çalışırsanız çok daha anlaşılır olur ve tüm taşlar yerine oturur.

```javascript
const express = require('express');
const bodyParser = require('body-parser');
const app = express();

app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: false }));

require('./routes')(app);

const PORT = 5555;
app.listen(PORT, () => {
    console.log(`Hartstone Game API servisi ${PORT} üstünden hizmettedir ;)`);
})
```

Bu biraz da uzun sürecek kodlamaların ardından db migration sürecini başlatılabilir ve tabloların oluşup oluşmadığı kontrol edebiliriz. Tek yapmamız gereken aşağıdaki terminal komutunu kullanmak.

```bash
node_modules/.bin/sequelize db:migrate
```

> Hero ve Card arasında one-to-many ilişki var. Yani bir kahramana ait birden fazla kart olabilir. Bu nedenle migration sırasında önce Hero planının çalıştırılması lazım ki postgresql tarafında iki tablo arasındaki bire çok ilişki başarılı şekilde kurgulanabilsin. Bu nedenle hero-migration.js dosyasının başında bir a harfi bulunuyor. Çünkü db:migrate komutu klasördeki javascript içeriklerini alfabetik sırada çalıştırıyor. En azından ben denerken böyle bir şey fark ettim. Fark edene kadar da epey bir debelendim:)

![skynet_12_Screenshot_1.png](images/skynet_12_Screenshot_1.png)

## Çalışma Zamanı

Sonuçları görmek için sabırsızlandığınızı tahmin edebiliyorum. Öyleyse ana klasördeki index.js dosyasını çalıştıralım ve sonrasında Postman ile 5555 portundan hizmet veren servise HTTP Get, Post talepleri gönderelim.

```bash
node index.js
```

Örnek bir kahramanın oluşturulması için aşağıdaki Response içeriğini kullanabiliriz.

```text
HTTP Post
http://localhost:5555/game/api/hero

JSON

{
"name": "Paladin",
"info": "The. Paladin is one of the ten classes in Hearthstone, represented by Uther Lightbringer, Lightforged Uther, Lady Liadrin, Prince Arthas, and Sir Annoy-O."
}
```

![skynet_12_Screenshot_2.png](images/skynet_12_Screenshot_2.png)

Tüm kahramanların listesinin çekilmesi içinde şu komut işe yarar. (http://localhost:5555/game/api/heros daha iyi durabilir)

```bash
HTTP Get
http://localhost:5555/game/api/hero
```

![skynet_12_Screenshot_3.png](images/skynet_12_Screenshot_3.png)

Yeni bir Card oluşturmak içinse malum bir HTTP Post talebi göndermek icap eder. Body, bir JSON içeriği olmalıdır.

```text
HTTP Post
http://localhost:5555/game/api/cad

JSON

{
"name": "Aviana",
"description": "Aviana is a Druid-only minion. This card was introduced with The Grand Tournament and can now only be obtained through crafting. Below the card images, you will find explanations to help you use the card optimally in every game mode of Hearthstone.",
"attack": 5,
"health": 5,
"spell": 10,
"heroId": 2
}
```

Belli bir kahramana ait kartları çekmek içinse şöyle bir talep yeterli olur.

```bash
HTTP Get
http://localhost:5555/game/api/hero/2/cards
```

![skynet_12_Screenshot_4.png](images/skynet_12_Screenshot_4.png)

Yazması biraz zahmetli ama sonuçları açısından anlaşılır bir öğreti olduğunu düşünüyorum. Umarım sizler için de faydalı olur. Kodların tamamına [skynet github reposu](https://github.com/buraksenyurt/skynet/tree/master/No%2012%20-%20REST%20with%20Sequelize)ndan ulaşabilirsiniz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
