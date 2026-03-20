---
layout: post
title: "Mountebank ile Mock Servis Desteği Sunmak"
date: 2020-12-07 14:00:00 +0300
categories:
  - nodejs
tags:
  - nodejs
  - bash
  - javascript
  - dotnet
  - rest
  - json
  - http
  - github
---
Mountebank, ne zamandır merak ettiğim ve denemek istediğim araçlardan birisiydi. Test senaryolarında kullanmak isteyeceğimiz mock servislerini kolayca inşa edebilmemize olanak sağlayan bir araç olarak tanımlayabilirim. Örneğin test kodumuz arka tarafta belki bir veritabanına bağlanan belki başka bir servis zincirini çağıran ya da farklı bağımlıkları olan bir servisi kullanmak zorunda olabilir. Normal şartlarda bu servisin ayakta olması zorunludur ki testimiz yürüsün. Ancak o anki test vakasının ilerleyen adımlarının çalışması için illaki bu servisin vereceği çıktıya ihtiyacımız yoktur. Test vakası adımlarının devamı için o servisin vereceği çıktının sanki verilmiş gibi yapılarak ilerlenilmesi tercih edilen yöntemlerdendir.

![mb.jpg](/assets/images/2020/mb.jpg)

Üstelik kullandığı servisin hep aynı veri setini kullanarak çalışan bir testin, veri değişikliklerinden etkilenmemesi de istenebilir. Böyle durumlarda asıl servismiş gibi hareket eden (Sahtekar/Taklitçi gibi isimlendirebiliriz bunları) ama testin ihtiyacı olup asıl vakayı bozmayacak şekilde kullanılabilen servisleri test senaryosu içerisine monte edebiliriz. Yani bir mock servis ile teste devam edelim diyebiliriz.

İşte Mountebank, mock servislerin host edilmesi noktasında oldukça kullanışlı bir araç olarak karşımıza çıkıyor. Mountebank kendisi ile iletişim için REST API arayüzü sunuyor. Bu API'yi kullanarak Mountebank'a mock servisler eklenebiliyor. Yani bir mock servis ihtiyacımız varsa bunu Mountebank'a yüklemek için HTTP Post çağrısı ile bir şeyler göndermemiz (Stub'lardan oluşan Imposter aktörleri) yeterli oluyor. Mountebank'ın CI/CD hatlarına da entegre edilebildiği ifade ediliyor (ki henüz gözümle görme şansım olmadı) Bu çalışmamın amacı Heimdall (Ubuntu-20.04) üstünde onu deneyimlemek ve nasıl çalıştığını, ne gibi bir çözüm sunduğunu anlayabilmek.

## Senaryo

Mountebank sunucu uygulamasını ayağa kaldırırken kendisine otomatik olarak en az iki mock servisi kayıt edeceğiz. Bu servisleri imposter olarak görebilmeli ve curl, postman veya herhangibir tarayıcıdan tüketebilmeliyiz. Ayrıca Mountebank sunucusu ayakta iken yine Postman gibi bir aracı kullanıp yeni bir mock servis bildirimini gönderebilmemiz gerekiyor. Mountebank uygulaması ve ilgili servisler ayakta iken elbette bir birim test üzerinden de bu servislerin tüketimini ele almalıyız.

## Ön Hazırlıklar

Öncelikle Mountebank'ı sistem yükleyerek işe başlamamız gerekiyor. Bunu bir NodeJs uygulaması üzerinden icra edeceğiz. Aşağıdaki adımları izleyerek devam edelim.

```bash
mkdir asgard
cd asgard
npm init --yes
# Mountebank paketini npm aracı ile yüklüyoruz
# Birde yazacağımız mock servisleri Mountebank sunucusuna bildirmek için
# node-fetch paketinden yararlanacağız. Dolayısıyla onu da ekliyoruz.
npm i --save mountebank node-fetch

mkdir src
cd src
# port bilgilerini tutacağımız bir konfigurasyon dosyası ile 
# Bir Mountebank sunucusunu ayağa kaldırmaktan sorumlu index dosyasını oluşturuyoruz
# Bunlar src dizini altında konuşlanabilirler
touch ports.js index.js

# ve ilk Mock Service'imiz için aşağıdaki dosyayı kullanabiliriz
# yine src altında olabilir
touch ping-service.js

# İkinci servisimizde herhangi bir şehir bilgisini getirmek için kullanacağımız bir mock servis
# Şehir bilgileri normalde bir veritabanında tutuluyor ve primary key değerine göre çekiliyor.
# Ancak test senaryomuzda zaten belli şehirleri alıp ilerlememiz mümkün. Yine de bunu bir servis üstünde
# yapmamız lazım. İşte mock servis bu noktada devreye giriyor (şehir bilgilerini cities.csv dosyasında tutuyoruz)
touch city-service.js
```

Kod içeriklerini sırasıyla yazarak ilerleyelim.

ports.js;

```javascript
module.exports = {
    server: 5500, // Mountebank uygulamasının ana servis adresidir
    ping_service: 5501, // ping-pong servisinin kullanacağı adrestir
    city_service: 5502, // bu çalışacağımız şehir bilgilerini getiren servise ait bir adres
};
```

index.js;

```javascript
// mountebank ve kendi yazdığımız ports modülünü ekledik
const mb = require('mountebank');
const ports = require('./ports');

/* 
    Mountebank uygulamasını ayağa kaldırdığımızda, yazdığımız mock servislerin de 
    etkinleştirilmesini sağlayabiliriz.
    then fonksiyonuna odaklanın.
*/
const pingService = require('./ping-service');
const cityService = require('./city-service');

// Yeni bir mountebank örneği oluşturuyoruz
mb.create({
    port: ports.server,
    pidfile: '../mb.pid',
    logfile: '../mb.log', // Bir üst klasörde tutacağımız log dosyası bildirimi
    protofile: '../protofile.json',
    ipWhitelist: ['*']
}).then(function () {
    pingService.register(); // pingService'i 
    cityService.register(); // ve cityService'i register ediyoruz
});
```

ping-service.js;

```javascript
/*
    Bu bir Hello World mock servisi.
    Mountebank'a register ediliyor.
    Mountebank tarafına register edilen bir mock servis imposter olarak tanımlanır.
    Imposter içerisinde stub tanımlaları yer alır. Birden fazla stub tanımı olabilir.
    Stub'larda ne tür talepler için ne tür cevaplar verileceğinin tanımlandığı yer olarak düşünülebilir.
    
    Örneğin aşağıdaki stub tanımında, JSON formatında bir sözleşme(contract) mevcuttur.
    Predicates ile hangi route ve metod için talep alınacağı ifade edilir.
    Response kısmında da bu talep için nasıl bir cevap dönüleceği. Örnekte HTTP 200 OK durum bilgisi ile birlikte basit bir JSON cevap verilmektedir.
    Yani bu sayede mock servisin talebe karşılık ne döndüreceğini tanımlamış oluruz.
    imposter kısmında ise mock servis ile nasıl bir protokol üstünden iletişim kurulacağı,
    hangi porttan yayın yapacağı ve stub sözleşmesinde nelerin yer alacağın dair bilgilere toplanır.
    Örnekte HTTP protokolünün kullanılacağı ifade edilmektedir.
*/
const fetch = require('node-fetch'); // Mountebank servisine Post işlemini kolaylaştıracak
const ports = require('./ports');

function register() {

    const stub = [
        {
            predicates: [{
                equals: {
                    method: "GET",
                    "path": "/ping"
                }
            }],
            responses: [
                {
                    is: {
                        statusCode: 200,
                        headers: {
                            "Content-Type": "application/json"
                        },
                        body: JSON.stringify({ message: "Pong!" })
                    }
                }
            ]
        }
    ];

    const imposter = {
        port: ports.ping_service,
        protocol: 'http',
        stubs: stub
    };

    /*
        Aşağıdaki kod parçasında Mountebank'ın imposters API'sine HTTP Post ile bir talep gönderme işlemi yer alıyor.
        body parametresine yukarırdaki imposter'ın JSON formatına serileştirilen halini gönderdiğimize dikkat edelim.
        Böylece bu mock servisini Mountebank sunucusuna kayıt etmiş ve kullanıma açmıl olacağız.
    */
    const url = `http://127.0.0.1:${ports.server}/imposters`;

    //console.log(JSON.stringify(imposter));

    return fetch(url, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(imposter)
    });

}

module.exports = { register };
```

city-service.js;

```javascript
/*
    Mountebank ile ilgili yaygın imposter senaryolarından birisi de CSV gibi içeriklerden okunan veriyi döndürmek.
    Örneğin bir veritabanından gelen id değerine göre şehir bilgisi döndüren bir servisimiz olduğunu düşünelim.
    Test senaryomuzda asıl servis yerine onu taklit eden bir servis kullanmak istiyoruz.
    Aşağıdaki gibi bir stub yapısı kullanılabilir.
    Cities/1 gibi bir HTTP talebi olursa,
    fromDataSource kısmında belirtilen CSV dosysını regex ile sorguluyoruz.
    Desenimiz city_id alanını index kabul ederek içeriden bu alana ait satırı buluyor.
    Bulunan satır row değişkenine alınıyor ve body kısmındaki map tekniği ile bir JSON sonuç üretiliyor.
    Test senaryosu böylece gerçekte veritabanına gitmeyen ama ihtiyacımız olan şehir bilgisi döndürecek taklitçi ile akışını devam ettirebilir.
*/
const ports = require('./ports');
const fetch = require('node-fetch');

function register() {
    const stub = [
        {
            predicates: [{
                and: [
                    { equals: { method: "GET" } },
                    { startsWith: { "path": "/cities/" } }
                ]
            }],
            responses: [
                {
                    is: {
                        statusCode: 200,
                        headers: {
                            "Content-Type": "application/json"
                        },
                        body: '{ "cityName": "${row}[name]", "cityCode": "${row}[code]" }'
                    },
                    _behaviors: {
                        lookup: [
                            {
                                "key": {
                                    "from": "path",
                                    "using": { "method": "regex", "selector": "/cities/(.*)$" },
                                    "index": 1
                                },
                                "fromDataSource": {
                                    "csv": {
                                        "path": "src/data/cities.csv",
                                        "keyColumn": "city_id"
                                    }
                                },
                                "into": "${row}"
                            }
                        ]
                    }
                }
            ]
        }
    ];

    const imposter = {
        port: ports.city_service,
        protocol: 'http',
        stubs: stub
    };

    const url = `http://127.0.0.1:${ports.server}/imposters`;

    //console.log(JSON.stringify(imposter));

    return fetch(url, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(imposter)
    });
}

module.exports = { register };
```

## Çalışma Zamanı

Uygulamanın çalışma zamanı için aşağıdaki adımları takip etmemiz yeterli. Mountebank server'ını ayağa kaldırmak için asgard klasörü altında aşağıdaki komutu vermek yeterli. Bunun işletilmesi içinse package.json'a start komutunu ekledik. Normal olarak src klasörü altındaki index.js dosyasını çalıştırıyor. Çalıştırılan komut sonrası sunucunun ayakta olup olmadığını anlamak için pekala http://localhost:5500 adresine gidebiliriz (Bir JSON içeriği görmemiz lazım) Hatta gelen json'da belirtilen adreslere giderek yüklenen imposter'ları, servis hareketlerine ait log içeriklerini ve konfigurasyonu da görebiliriz.

```bash
npm start
```

Mountebank server'ı npm start ile ayağa kaldırdığımızda 5500 portundan gelecek olan json içeriği aşağıdaki ekran görüntüsündeki gibi olacaktır.

![skynet_34_Screenshot_01.png](/assets/images/2020/skynet_34_Screenshot_01.png)

Mock Servis örneklerini ekledikçe, imposter sözleşmelerinde belirtilen route tanımlarına gidilerek kayıt edilen servislerin çalışıp çalışmadığı kontrol edilmelidir. Örneğin ping-service'i Mountebank'a ekledikten sonra http://localhost:5501/ping adresine talete bulunup, stub->response kısmında belirtiğimiz pong cevabını almamız gerekir.

![skynet_34_Screenshot_02.png](/assets/images/2020/skynet_34_Screenshot_02.png)

Ayrıca birden fazla servisi Mountebank'a ekledikten sonra (ister kod yoluyla ister Postman gibi araçlarla Post ederek olsun) http://localhost:5500/imposters gibi adresten bunları izleyebilir ve gelen talep sayılarına bakabiliriz. Ben ikinci servisi de ekledikten sonra aşağıdaki ekran görüntüsünde olduğu gibi bu durumu gözlemleyebildim.

![skynet_34_Screenshot_03.png](/assets/images/2020/skynet_34_Screenshot_03.png)

Mountebank uygulaması ayakta iken Postman veya muadili bir araçla aşağıdaki çıktıyı gönderdiğimizde de söz konusu servisin imposter olarak eklendiğini görürüz. Yani ille de uygulama içerisinde kod yoluyla servis yüklenmesi mecburi değildir. Mountebank, REST Api şeklinde bir arabirim sunduğundan ekleme, silme vb işlemleri doğru içerikten oluşan talepler ile sağlayabiliriz. Tabii mountebank sunucusu kapandığında bu mock servisler de ömürlerini tamamlayacaktır.

```javascript
Kullandığım adres : http://localhost:5500/imposters
Http metodu : POST
Body tipi : raw/json
Body içeriği :

{
  "port": 5503,
  "protocol": "http",
  "stubs": [
    {
      "predicates": [
        {
          "equals": {
            "method": "POST",
            "path": "/creditrisk/check/12345678"
          }
        }
      ],
      "responses": [
        {
          "is": {
            "statusCode": 200,
            "headers": {
              "Content-Type": "application/json"
            },
            "body": {
              "available": "no"
            }
          }
        }
      ]
    }
  ]
}
```

![skynet_34_Screenshot_04.png](/assets/images/2020/skynet_34_Screenshot_04.png)

## Testler

Mock servisleri yazdık. İyi güzel de bunları Nodejs tarafındaki testlerde nasıl kullanacağız? İşin içerisine Mocha ve Chai paketlerini katsak pek bir güzel olur sanki;) Hatta Mock servis çağrılarını gerçekleştirmek için axios paketi en ideali. Asgard ile paralel yeni bir proje açıp devam örneğimize edelim.

```bash
# Asgard klasöründe önce gerekli test ve servis haberleşme paketlerimizi yükleyelim
# Mocha : Belki en popüler test framework'lerinden birisi
# Chai : Behavioral Driven Design'ın TDD üstünde başarılı bir uyarlaması
# Axios : Mountebank servis çağrıları için kullanacağımız modül
npm i --save axios mocha chai

# Sonra yine asgard klasörü içerisindeyken test isimli bir klasör açalım.
# ve içerisine test dosyamızı koyalım
# Ayrıca asgard'a ait package.json içerisinde de gerekli test komutunu vermemiz gerekiyor
mkdir test
touch ./test/index.test.js
```

index.test.js;

```javascript
const expect = require('chai').expect;
const axios = require('axios');

describe('Asgard Mock Servis testleri', () => {

    it('Herhangi bir şehirden en az bir kullanıcı bilgisi gelmeli', () => {
        var city;

        return axios
            .get(`http://localhost:5502/cities/3`)
            .then(res => res.data)
            .catch(error => console.log(error))
            .then(response => {
                /*
                    Beklentilerimizi yazıyoruz.
                    Mock servisinin dönüşü bir object olmalı,
                    cityName özelliği bulunmalı ve değeri Istanbul olmalı
                    ayrıca cityCode özelliğinin değeri de 340 gelmeli
                */
                expect(typeof response).to.equal('object');
                expect(response.cityName).to.equal('Istanbul')
                expect(response.cityCode).to.equal("340")

                city = response.cityName; // sonraki işlemler için değişkeni sakladım sadece
            }).then(() => {
                // Burada başka bir test operasyonu icra edilebilir
                // console.log(city, "ile ilgili başka testler");
            });
    });

    it('Ping mesajıma karşılık Pong denmeli ve oyun başlamalı', () => {
        return axios
            .get(`http://localhost:5501/ping`) // servis adresini bozup Fail durumunu da test edebiliriz
            .then(res => res.data)
            .catch(error => console.log(error))
            .then(response => {
                expect(typeof response).to.equal('object');
                expect(response.message).to.equal('Pong!')
            });
    });
});
```

Test kodlarını tamamladıktan sonra yine iki terminal üzerinden örnekleri denemek lazım. İlk terminalde Mountebank sunucusunu ayağa kaldırıp mock servisleri devreye sokmamız gerekiyor. İkinci terminalde ise yine asgard klasörü altında aşağıdaki komutu işletmeliyiz.

```bash
npm test
```

Yazılan iki testin de başarılı olma haline ait bir görüntüyü aşağıda görebilirsiniz.

![skynet_34_Screenshot_05.png](/assets/images/2020/skynet_34_Screenshot_05.png)

Mountebank servisleri ayakta değilken ki durum ise aşağıdaki gibi olacaktır.

![skynet_34_Screenshot_06.png](/assets/images/2020/skynet_34_Screenshot_06.png)

Temel olarak Mountebank'ın nasıl kullanıldığını az çok anladık diye düşünüyorum. Şimdi bunu kendi projelerinizde kullanmayı deneyebilirsiniz. Konu ile ilgili not olarak aldığım birkaç soruyu buradaya da bırakayım.

- Mountebank uygulamasına bir mock servis sözleşmesini (imposter) NodeJs harici bir uygulamadan da (Örneğin bir.Net Core uygulaması) yollayabilir miyiz?
- Bir imposter dosyasına birden fazla stub yüklenebilir mi?
- Peki bir stub içerisinde n sayıda prediction ve response çifti bulunabilir mi?
- Eklenen bir imposter'ı nasıl silebiliriz?

Böylece geldik bir skynet derlememizin daha sonuna. Örneğin tamamına [github reposu](https://github.com/buraksenyurt/skynet/tree/master/No%2034%20-%20Mountebank) üzerinden erişebilirsiniz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
