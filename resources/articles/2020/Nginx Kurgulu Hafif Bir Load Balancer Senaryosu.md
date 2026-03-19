---
title: "Nginx Kurgulu Hafif Bir Load Balancer Senaryosu"
pubDate: 2020-06-17 09:20:00
categories:
  - Node.js
tags:
  - Node.js
  - Bash
  - JavaScript
  - .NET
  - MongoDB
  - REST
  - JSON
  - HTTP
  - Docker
  - GitHub
---

# Nginx Kurgulu Hafif Bir Load Balancer Senaryosu
Dünyanın en popüler ve hızlı proxy serverlarıdan birisi sanıyorum ki Nginx. Daha önce birçok kez onun üzerinde.Net Core tabanlı servisleri konuşlandırmıştım. Ancak bu defa Nginx'in talepleri dengeli bir şekilde dağıtacak şekilde (Load Balancer olarak) nasıl kullanacağımızı öğrenmeye çalışacağız. Senaryo gereği NodeJS ile yazılmış basit bir dummy servisin birkaç kopyasını çalıştıracağız. Aynı makinede farklı port adresleri üzerinden hizmet verecek bu servislere gelen taleplerin dağıtımını, Load Balancer görevini üstlenecek olan Nginx sunucusu üstlenecek.

Ben bu kurguyu her zaman yaptığım gibi Heimdall (Ubuntu-20.04) üzerinde icra ediyorum. Ancak teorik olarak farklı platformlarda da benzer şekilde ilerleyebilirsiniz. Sisteminizde docker ve NodeJs'in yüklü olması şu an için yeterli. Nginx sunucusunu makineye kurmaktansa Docker imajı ile çalışmak çok daha mantıklı olacaktır. Hatta bu kurgu özelinde kendi docker imajımızı kullanıp Nginx konfigurasyonlarını bu Container içerisinde yapacağız. Öylese işe Dummy servisimizi yazarak başlayabiliriz.

## Dummy Service ve Diğer NPM Kurulumları

Express paketini kullanan dummy NodeJs servisini aşağıdaki terminal komutları ile oluşturarak senaryomuza başlayalım. Söz konusu servis sevdiğim birkaç özlü sözün listesini istemciye göndermekte. Kobay bir servis olduğundan mümkün mertebe basit olmasında yarar var.

```bash
mkdir DailyQuoteApi
cd DailQuoteApi

# Node proje açılışı
npm init --y
# REST Servis özellikleri için express, HTTP Request loglama için morgan paketlerinin yüklenmesi
npm i --save express morgan
touch index.js

# Global paket olarak Process Manager'ın eklenmesi (servisin birden fazla örneğini çalıştırmak için işimizi kolaylaştıracak)
sudo npm i --g pm2
# versiyon kontrolü
pm2 -v 
```

index.js içeriğini de aşağıdaki gibi yazabiliriz.

```javascript
var express = require('express');
var morgan = require('morgan');

var app = express();
app.use(morgan('combined')); // HTTP Loglama middleware bildirimi

// Tipik bir HTTP Get talebi karşılıyoruz
// quotes olarak gelen yönlendirmelerde çalışacak
app.get('/quotes', function (req, res) {
    res.send(quotes);
})

// Varsayılan HTTP Get Talepleri
app.get('/', function (req, res) {
    res.send('pong!');
})

// listen çağrısı ile servis uygulaması ayağa kalkıyor
// Birden fazla port seçeneği olabilir. 
// Process Manager olan PM2 aracı argv[2] üstünden gelen port ile uygulamayı ayağa kaldırabilir
app.listen(process.argv[2] || process.env.PORT || 4500, () => {
    console.log(`Uygulama ayakta ve ${process.argv[2] || process.env.PORT || 4500} nolu porttan dinlemede.`);
});

// Sembolik bir JSON içeriği
// Birkaç özlü söz yer alıyor
var quotes = [
    { "id": 1, "owner": "Nelson Mandela", "content": "The greatest glory in living lies not in never falling, but in rising every time we fall." },
    { "id": 2, "owner": "John Lennon", "content": "Life is what happens when you're busy making other plans." },
    { "id": 3, "owner": "Aristotle", "content": "It is during our darkest moments that we must focus to see the light." },
    { "id": 4, "owner": "Marilyn Monroe", "content": "Keep smiling, because life is a beautiful thing and there's so much to smile about." },
    { "id": 5, "owner": "Oprah Winfrey", "content": "You know you are on the road to success if you would do your job and not be paid for it." },
];
```

Dikkatinizi çekmiştir sisteme pm2 isimli bir araç yükledik. Process Manager isimli bu aracı kobay servisimizin birkaç farklı örneğini çalıştırmak için kullanıyoruz. Yeri gelmişken pm2 ile ilgili faydalı birkaç komuta da bakabiliriz. Nitekim Node servisleri arka planda çalıştırırken PM2 (Process Manager) aracı epey işe yarıyor. Örneğin,

```bash
# --name ile process'i isimlendirelim ki tanımamız kolay olsun
# -f ile Obi Van Kenobi gücü kullanıyoruz (force yahu)
# -- 4501 sıralamasına dikkat. 4501 programa argv[2] ile gelen komut satırı parametre indeksi
# --watch uygulama değişiklikleri otomatik algılansın diye
pm2 start index.js --watch --name app1 -f -- 4501
pm2 start index.js --watch --name app2 -f -- 4502
pm2 start index.js --watch --name app3 -f -- 4503
pm2 start index.js --watch --name app4 -f -- 4504

# Process'leri görmek için kullanılır
pm2 status

# id bilgisi ile process'leri silmek için
pm2 delete 0 1 2 3
```

pm2 kullanımına ait çalışma zamanı çıktılarını şöyle resmedebiliriz.

![skynet_16_Screenshot_1.png](images/skynet_16_Screenshot_1.png)

![skynet_16_Screenshot_2.png](images/skynet_16_Screenshot_2.png)

## Nginx Tarafı

Şimdi proxy server'ımızı Load Balancer olarak ayarlayalım. Bu amaçla kendi Nginx docker imajımızı kullanacağımızdan bahsetmiştik. Oldukça basit bir imaj. Önemli olan bu imaj içerisinde çalışacak Nginx'in Load Balancer özelliklerini tutacak olan konfigurasyon. Bunu nginx.conf dosyasındaki tanımlamalar ile sağlayacağız.

```bash
mkdir judgedredd
cd judgedredd

touch nginx.conf
touch dockerfile

# Senaryoya özel nginx imajının hazırlanması
# Dockerfile içeriğine göre tap taze bir nginx imajı oluşturuyoruz
sudo docker build -t freshnginx .

# Container çalıştırılır ve 8080 portu dışarıya açılır
sudo docker run -d --name judge-dredd -p 8080:80 freshnginx
# Container çalışıyor mu bir bakmak usüldendir
sudo docker ps
```

dockerfile ve conf içeriklerini ise aşağıdaki gibi tasarlamamız gerekiyor.

Dockerfile;

```text
FROM nginx

RUN rm /etc/nginx/conf.d/default.conf
COPY nginx.conf /etc/nginx/conf.d
EXPOSE 80

CMD ["nginx","-g","daemon off;"]
```

nginx.conf;

```javascript
upstream backend {
    server 172.17.0.1:4501;
    server 172.17.0.1:4502;
    server 172.17.0.1:4503;
    server 172.17.0.1:4504;
}

server {
 listen 80;
 location / {
   proxy_pass http://backend;
 }
}
```

Konfigurasyon tanımlamalarına göre NGinx sunumuz 80 portuna gelen talepleri [http://backend](http://backend) adresine yönlendirecek. Bu adres bir upstream bloğu olarak tanımlanmış durumda ve 4 farklı porta sahip alt adresleri barındırmakta. Bu arada kurguyu hazırlarken bulmakta zorlandığım şeylerden birisi de nginx container'ından makinedeki (Heimdall) nodejs servislerine hangi IP ile çıkıldığını öğrenmek oldu (Benim senaryoda 172.17.0.1) Benzer bir ihtiyaç sizin için de geçerli olabilir. Bu adresi bulmak içinse şöyle bir yol izlenebilir.

```bash
sudo docker container attach judge-dredd
```

Ben yukarıdaki terminal komutu ile çalışan container'a log açıp http://localhost:8080/ adresine talep gönderdim. Bu sayede nginx.conf içerisinde kullanılan dış IP adresinin ne olduğunu görmeyi başardım. Servislere docker container'ı içinden gidilip gidilmediğinden emin olmak içinse, container içerisindeki terminale girip curl ile talep göndermeyi ihmal etmedim.

```bash
sudo docker exec -it judge-dredd /bin/bash
```

![skynet_16_Screenshot_3.png](images/skynet_16_Screenshot_3.png)

Şu anki kurgumuzda varsayılan olarak kabul edilen [Round-Robin](https://www.nginx.com/resources/glossary/round-robin-load-balancing/) isimli Load Balancer algoritması kullanılmakta. Ancak bu algoritma dışında hash, ip_hash, least_conn gibi farklı modeller de mevcut. Benim pek araştırma fırsatım olmadı ama siz kendi çalışma sahanızda bu modeller arasındaki farklılıkları analiz etmeyi deneyebilirsiniz.

## Çalışma Zamanı

Gelelim yaptıklarımızın ne işe yaradığını görmeye. Nginx Container'ını çalıştırdıktan sonra curl ile farklı sayıda talebi servisimize doğru gönderebiliriz.

```bash
curl http://localhost:8080/
curl http://localhost:8080/quotes
```

Buna göre aşağıdaki ekran görüntülerinde yer alan sonuçların benzerlerini elde edebilmeniz gerekiyor.

![skynet_16_Screenshot_5.png](images/skynet_16_Screenshot_5.png)

Özellikle alttaki görüntüde 80 için gelen taleplerin 450* portlarında dağıldığını görüyoruz.

![skynet_16_Screenshot_4.png](images/skynet_16_Screenshot_4.png)

Görüldüğü üzere bir şekilde Docker Container ayağa kalktı, reverse proxy olan Nginx görevini yerine getirdi ve gelen talepleri ilgili servisin çalışma zamanı örneklerine iletti. Senaryomuza göre aynı servisin makine üstünde farklı portlardan çalıştırılan dört örneği bulunduğunu fark etmiş olmalısınız. Nginx sunucusundaki upstream ayarlarına göre 8080 üstünden gelen taleplerin bu process'lere dağıtılıyor olması lazım. Soru şu; Gerçekten dağıldıklarını nasıl ispatlarsınız?:) PM2 bizim için 4 farklı process açıp servisin birer örneğini buraya atıyor ama gerçekten taleplerin ayrık işlemci süreçlerine gittiğini nasıl anlarız?

Bunun haricinde senaryonun daha da anlamlı hale getirilmesini sağlayabilirsiniz. Söz gelimi özlü sözlerin yine docker imajı üstünden çalışan bir MongoDb deposundan alınması sağlanabilir. Arka plan servisimizde bir Docker Container olarak kullanılabilir. Hatta senaryoyu biraz daha ilerletip arka plan servisi, veritabanı reposu ve nginx'in docker compose ile hazırlanıp tek seferde ayağa kaldırılıp kullanılması da söz konusu olabilir. Ben bu düşünce tohumlarını ortaya bırakmış oldum. Kalanı sizde:) Böylece geldik bir [SkyNet](https://github.com/buraksenyurt/skynet) derlemesinin daha sonuna. Kodların tamamına [github reposu](https://github.com/buraksenyurt/skynet/tree/master/No%2016%20-%20LB%20With%20Nginx)ndan ulaşabilirsiniz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
