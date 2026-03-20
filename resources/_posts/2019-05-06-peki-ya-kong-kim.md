---
layout: post
title: "Peki ya Kong Kim?"
date: 2019-05-06 07:00:00 +0300
categories:
  - devops
tags:
  - devops
  - bash
  - csharp
  - javascript
  - dotnet
  - linq
  - postgresql
  - cassandra
  - rest
  - json
  - web-api
  - http
  - authentication
  - docker
  - nodejs
  - caching
  - generics
  - microservices
  - github
---
Kurumsal mimari ekibinin önerdiği çatılardan birisi üzerine kurulmuş yeni ürünümüzü test ortamına almaya çalıştığımız bir gündü. Local makinelerimizde çok az sorunla ayağa kaldırdığımız proje, test ortamında ne yazık ki daha fazla problem üretmişti. Ağırlıklı olarak web önyüzünden iş kurallarının yürütüldüğü Web API servislerine gidişlerde sorunlar yaşıyorduk.

![kingkong.jpg](/assets/images/2019/kingkong.jpg)

CI/CD hattındaki parametreleri, veri tabanı nesnelerini, SSO ayarlarını kontrol edip Kibana loglarını incelemeye başladık. Tüm bu işler devam ederken DevOps ekibinden bize destek veren sevigili Yavuz, servisler üzerindeki trafiği monitör etmekteydi. Konuşmalarımız sırasında Docker Container'larının önünde yer alan KONG isimli bir arabirimden bahsetti. O an içimde bir merak uyanmış olsa da aslında sorunların bir an önce çözülmesini istiyordum. Bu yüzden merakımı birkaç hafta sonrasına bıraktım.

Derken artık cumartesi geceleri dışına da taşan [saturday-night-works](https://github.com/buraksenyurt/saturday-night-works) çalışmalarımda ona yer verme fırsatı yakaladım. [Kimdi bu Kong?](https://konghq.com/kong/) Müzik grubu olan Kong'muydu yoksa Skull adasındaki iri olan mıydı? Belki de API Gateway'di. Onu Westworld üzerinde çalıştırabilir miydim? Öğrenmin yolu basitti. Sonunda macera başladı. Github çalışmaları tamamlandıktan uzun süre sonra da bloğuma not olarak düşmeye karar verdim.

Hali hazırda çalışmakta olduğum firmada, microservice'lerin orkestrasyonu için KONG kullanılıyor. Kabaca bir API Gateway rolünü üstlenen KONG mikro servislere gelen taleplerle ilgili olarak Load Balancing, Authentication, Rate Limiting, Caching, Logging gibi cross-cutting olarak tabir edebileceğimiz yapıları hazır olarak sunuyor (muş) Web, Mobil ve IoT gibi uygulamalar geliştirirken back-end servisleri çoğunlukla mikro servis formunda yaşamaktalar. Bunların orkestrasyonunda görev alan KONG, Lua dili ile geliştirilmiş, performansı ile ön plana çıkan NGINX üzerinde koşan açık kaynaklı bir proje olmasıyla da dikkat çekiyor.

Benim amacım ilk etapta KONG'u WestWorld (Ubuntu 18.04, 64bit) üzerine kurmak ve en az bir servis geliştirip ona gelen talepleri KONG üzerinden geçirmeye çalışmak (Kabaca proxy rolünü üstlenecek diyebiliriz) Normal şartlarda KONG'u sisteme tüm gereksinimleri ile kurabiliriz ancak denemeler için docker imajlarını kullanmak da yeterli olacaktır ki ben bu yolu tercih ediyorum.

## Kobay REST servisleri

Çalışmada en azından bir Web API servisinin olması lazım. Bir tane.net core bir tane de node.js tabanlı servis geliştirmeye karar verdim. Projeler için WestWorld'de uyguladığım terminal komutları şöyle.

```bash
mkdir services
cd services
dotnet new webapi -o FabrikamApi
touch Dockerfile
touch .dockerignore
mkdir GameCenterApi
cd GameCenterApi
npm init
sudo npm i --save express body-parser
touch index.js
touch Dockerfile
```

.Net Core ile geliştirilmiş FabrikamApi servisindeki hazır kod dosyalarında bir kaç değişiklik yapıp, Node.js tabanlı GameCenterApi klasöründeki index.js'i sıfırdan geliştirmem gerekti (Servislerin normal kullanım örneklerine ait [Postman dosyasını burada](https://github.com/buraksenyurt/saturday-night-works/blob/master/No%2033%20-%20Who%20is%20Kong/assets/postman_samples.json) bulabilirsiniz)

PlayerController içeriği;

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FabrikamApi.Models;

namespace FabrikamApi.Controllers
{
    /*
    PlayersController isimli Controller sınıfı Player türünden bir listeyle çalışıyor.
    Konumuz KONG'u tanımak olduğu için çok detalı bir servis değil.
    Temel Get, Post, Put ve Delete operasyonlarını içermekte.
    Listeyi static bir değişkende tutuyoruz. Dolayısıyla servis sonlandırıldığında bilgiler uçacaktır.
    Ancak isterseniz kalıcı bir repository ekleyebilirsiniz.
     */
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private static List<Player> playerList = new List<Player>{
            new Player{Id=1000,Nickname="Hatuta Matata",Level=100}
        };
        [HttpGet]
        public ActionResult<IEnumerable<Player>> Get()
        {
            return playerList;
        }

        [HttpGet("{id}")]
        public ActionResult<Player> Get(int id)
        {
            var p = playerList.Where(item => item.Id == id).FirstOrDefault();
            if (p != null)
            {
                return p;
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public void Post([FromBody] Player player)
        {
            playerList.Add(player);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Player player)
        {
            var p = playerList.Where(item => item.Id == id).FirstOrDefault();
            if (p != null)
            {
                p.Nickname = player.Nickname;
                p.Level = player.Level;
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var p = playerList.Where(item => item.Id == id).FirstOrDefault();
            if (p != null)
            {
                playerList.Remove(p);
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
    }
}
```

PlayerController tarafından kullanılan Player sınıfı içeriği;

```csharp
namespace FabrikamApi.Models
{
    public class Player{
        public int Id { get; set; } 
        public string Nickname { get; set; }
        public int Level { get; set; }
    }
}
```

FabrikamAPI ye ait Docker ve.dockerignore içerikleri;

```text
FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/out .

ENV ASPNETCORE_URLS=http://+:65001

ENTRYPOINT ["dotnet", "FabrikamApi.dll"]
```

Başlangıçta dotnet:sdk imajından yararlanılacağı bildiriliyor. Çalışma klasörü bildirildikten sonra proje dosyasının kopyalanıp paketlerin yüklenmesi için restore işlemi başlatılıyor. Diğer her şeyin kopylanamasını bir build işlemi takip ediyor ki burada release versiyonu da çıkılıyor. Çalışma zamanı imajı alındıktan sonra 65001 numaralı port yayın noktası olarak belirtiliyor. Son adımsa dll'i çalıştıran dotnet komutunu içermekte. Birde bin ve obj klasörlerinin docker ortamında yer almaması için.dockerignore isimli dosyamız var. İçeriği oldukça basit.

```text
bin\
obj\
```

games isimli json veri dizisi ile ilgili basit get operasyonları içeren GameCenterApi uygulamasındaki kod içeriklerimiz ise şöyle.

index.js

```javascript
/*
GameCenterApi'den yayına alınan bu dummy servis games isimli diziyi döndüren iki basit fonksiyonelliğe sahip.
*/
const express = require('express');
const bodyParser = require('body-parser');
const app = express();

const games = [
    {
        id: 1,
        title: 'Red Dragons',
        maxPlayerCount: 10
    },
    {
        id: 2,
        title: 'Green Barrets',
        maxPlayerCount: 24
    },
    {
        id: 3,
        title: 'River Raid',
        maxPlayerCount: 4
    },
    {
        id: 4,
        title: 'A-Team',
        maxPlayerCount: 9
    },
];

app.use(bodyParser.json());

app.get('/api/v1/games', (req, res) => {
    res.json(games);
});

app.get('/api/v1/games/:id', (req, res) => {
    res.json(games[req.params.id]);
});

app.listen(65002, () => {
    console.log(`Oyun servisi aktif! http://localhost:65002/api/v1/games`);
});
```

DockerFile dosyası

```text
FROM node:carbon

# create work directory
WORKDIR /usr/src/app

# copy package.json
COPY package.json ./
RUN npm install

# copy source code
COPY . .

EXPOSE 65002

CMD ["npm", "start"]
```

Dosya node.js ortamlarından birisini ifade eden carbon bildirimi ile başlıyor. İmaj buradan alınacak. Çalışma klasörünün oluşturulması, package.json dosyasının burayı alınıp proje bağımlılıklarının install edilmesi, uygulamanın 65002 numaralı porttan ayağa kaldırılması diğer bildirimler olarak karşımıza çıkıyor.

Geliştirme noktasında servislerin çalıştığını kontrol etmemiz gerekiyor. FabrikamAPI isimli.Net Core tabanlı servisi çalıştırmak için,

```bash
dotnet run
```

terminal komutunu verip http://localhost:65001/api/v1/players adresine gidebiliriz. GameCenterApi isimli Node.js tabanlı servisi çalıştırmak içinse package.json içerisine aldığımız start kod adlı script'i işlettirebiliriz.

```bash
npm run start
```

Sonrasında http://localhost:65002/api/v1/games adresi üzerinden bu servisi de test edebiliriz.

> localhost bilgisi ilerleyen kısımlarda görüleceği gibi Docker'a geçildikten sonra değişmektedir.

## Servislerin Dockerize Edilmesi

Dikkat edilmesi gereken noktalardan birisi de, her iki örneğin Dockerize edilebilecek şekilde Dockerfile dosyaları ile donatılmış olmalarıdır. İlaveten.Net Core uygulamasında.dockerignore dosyası vardır. Bunu build context'ini ufalamak için kullanıyoruz. Docker imajları KONG tarafından kullanılacakları için önemli.

FabrikamApi uygulaması için Dockerize işlemleri aşağıdaki terminal komutuyla yapılabilir.

```bash
docker build -t fma_docker .
```

GameCenterApi isimli Node.js uygulaması içinse aşağıdaki gibi.

```bash
docker build -t gca_docker .
```

Dockerize işlemleri tamamlandıktan sonra container'ları çalıştırıp kontrol etmemizde yarar var. İlk iki komutla ayağa kaldırıp son komutla listede olup olmadıklarına bakıyoruz.

```bash
docker run -d --name=game_center_api gca_docker
docker run -d --name=fabrikam_api fma_docker
docker ps -a
```

WestWord'de durum aşağıdaki gibi.

![04_33_credit_1.png](/assets/images/2019/04_33_credit_1.png)

> Docker imajları çalışmaya başladıktan sonra servislere hangi IP adresi üzerinden gitmemiz gerektiğine bakmak için 'docker inspect game_center_api've 'docker inspect fabrikam_api'komutlarından yararlanabiliriz. Bize uzun bir Json içeriği dönecektir ancak son kısımda IPAddress bilgisini yakalayabiliriz. WestWorld için docker tabanlı adresler http://172.17.0.3:65001/api/v1/players ve http://172.17.0.2:65002/api/v1/games şeklinde oluştu. Sizin sisteminizde bu IP adresleri farklı olabilir.

![04_33_credit_2.png](/assets/images/2019/04_33_credit_2.png)

![04_33_credit_3.png](/assets/images/2019/04_33_credit_3.png)

## Kong Kurulumları ve Docker Servislerinin Dahil Edilmesi

Tüm işlemleri Docker Container'lar üzerinde yapacağız. Bu nedenle kendimize yeni bir ağ oluşturarak işe başlamakta yarar var. Aşağıdaki terminal komutları ile devam edelim.

```bash
docker network create sphere-net

docker run -d --name kong-db --network=sphere-net -p 5555:5432 -e "POSTGRES_USER=kong" -e "POSTGRES_DB=kong" postgres:9.6

docker run --rm --network=sphere-net -e "KONG_DATABAE=postgres" -e "KONG_PG_HOST=kong-db" kong:latest kong migrations bootstrap

docker run -d --name kong --network=sphere-net -e "KONG_LOG_LEVEL=debug" -e "KONG_DATABASE=postgres" -e "KONG_PG_HOST=kong-db" -e "KONG_PROXY_ACCESS_LOG=/dev/stdout" -e "KONG_ADMIN_ACCESS_LOG=/dev/stdout" -e "KONG_PROXY_ERROR_LOG=/dev/stderr" -e "KONG_ADMIN_ERROR_LOG=/dev/stderr" -e "KONG_ADMIN_LISTEN=0.0.0.0:8001, 0.0.0.0:8444 ssl" -p 9000:8000 -p 9443:8443 -p 9001:8001 -p 9444:8444 kong:latest
```

- İlk komutla sphere-net isimli bir docker network'ü oluşturuyoruz.
- İkinci uzun komutla Postgres veri tabanı için bir Container başlatıyoruz. sphere-net ağında çalışacak olan veri tabanını KONG kullanacak. KONG, veri tabanı olarak Postgres veya Cassandra sistemlerini destekliyor. Eğer yerel makinede Postgres imajı yoksa (ki örneği denediğim dönemde WestWorld'de yoktu) pull işlemi biraz zaman alabilir.
- Üçüncü komutla Postgres veri tabanının KONG için hazırlanması söz konusu.
- Dördüncü ve uzuuuuuun bir parametre listesine sahip komutla da KONG Container'ını çalıştırıyoruz (üşenmedim, kopyalamadan yazdım. Siz de öyle yapın)

Bu adımlardan sonra kong ve postgres ile ilgili Container'ların çalıştığını teyit etmeliyiz.

![04_33_credit_4.png](/assets/images/2019/04_33_credit_4.png)

Hatta http://localhost:9001 adresine bir HTTP GET talebi attığımızda konfigurasyon ayarlarını da görebiliriz. 9001 portu (Normal kurulumda 8001 de olabilir) yönetsel işlemlerin bulunduğu servis katmanıdır. Service ekleme, silme, görüntüleme ve güncelleme gibi işlemler 9001 portundan ulaşılan servisçe yapılır (Route yönetimi içinde aynı şey söz konusudur)

![04_33_credit_5.png](/assets/images/2019/04_33_credit_5.png)

Komutlar biter mi? Şimdi servislere ait Container'ları sphere-net üzerinde çalışacak şekilde ayağa kaldırmalıyız.

```bash
docker run -d --name=game_center_api --network=sphere-net gca_docker
docker run -d --name=fabrikam_api --network=sphere-net fma_docker
docker ps -a
```

![04_33_credit_6.png](/assets/images/2019/04_33_credit_6.png)

> KONG için bir Docker Network oluşturduk. Bu ağa dahil olan ne kadar Container varsa IP adresleri farklılık gösterecektir. sphere-net'e dahil olan Container'ların host edildiği IP adreslerini öğrenmek için terminalden 'docker inspect sphere-net'komutunu çalıştırabiliriz.

![04_33_credit_7.png](/assets/images/2019/04_33_credit_7.png)

## Çalışma Zamanı (Bir başka deyişle KONG üzerinde servislerin ayarlanması)

KONG, veri tabanı olarak kullanılan Postgres ve geliştirdiğimiz iki REST Servisine ait Docker Container'ları ayakta. WestWorld'deki güncel duruma göre

- http://172.19.0.4:65002/api/v1/games adresinde Node.js tabanlı servisimiz yaşıyor.
- http://172.19.0.5:65001/api/v1/players adresinde ise.Net Core Web API servisimiz bulunuyor.

Amacımız şu anda localhost:9000 adresli KONG servisine gelecek olan games ve players odaklı talepleri aslı servislere iletmek. Yani KONG ilk etapta bir Proxy servis şeklinde davranış gösterecek. Bunun için öncelikle servislerimizi KONG'a eklemeliyiz. KONG'a eklenen servisler http://localhost:9001/services adresinden izlenebilir ve hatta yönetilebilirler. Şimdi bu adrese aşağıdaki içeriğe sahip POST komutunu gönderelim (Postman ile yapabilir veya curl komutu ile terminalden icra edebiliriz)

```text
URL : http://localhost:9001/services
Method : HTTP Post
Content-Type : application/json
Body :
{
"name":"api-v1-games",
"url":"http://172.19.0.4:65002/api/v1/games"
}
```

![04_33_credit_8.png](/assets/images/2019/04_33_credit_8.png)

Bu işlemi FabrikamAPI içinde yaptıktan sonra http://localhost:9001/services adresine gidersek servis bilgilerini görebiliriz.

![04_33_credit_9.png](/assets/images/2019/04_33_credit_9.png)

Servisleri eklemek yeterli değil. Route tanımlamalarını da yapmak gerekiyor (KONG tarafındaki entrypoint tanımlamaları için gerekli bir aksiyon olarak düşünebiliriz) KONG services'e aşağıdaki içeriğe sahip talepleri göndererek gerekli route tanımlamaları yapılabilir.

```text
URL: http://localhost:9001/services/api-v1-players/routes
Method : HTTP Post
Content-Type : application/json
Body :
{
"hosts":["api.ct.id"],
"paths":["/api/v1/players"]
}
```

```text
URL: http://localhost:9001/services/api-v1-games/routes
Method : HTTP Post
Content-Type : application/json
Body :
{
"hosts":["api.ct.id"],
"paths":["/api/v1/games"]
}
```

Oluşan route bilgilerini http://localhost:9001/routes adresinden görebiliriz. Her iki servis için gerekli route tanımlamaları başarılı bir şekilde yapıldıktan sonra KONG üzerinden GameCenterAPI ve FabrikamAPI servislerine erişebiliyor olmamız gerekir.

![04_33_credit_10.png](/assets/images/2019/04_33_credit_10.png)

## Yararlandığım Diğer Docker Komutları

Örneği geliştirirken yararlandığım bazı Docker komutları da oldu. Mesela çalışan Container'ları stop komutu sonrası durduramayınca,

```bash
sudo killall docker-containerd-shim
```

Container'larımı görmek için,

```bash
docker ps -a
```

Container'ları sık sık remove etmem gerektiğinden,

```bash
docker rm {ContainerID}
```

Container'ın tüm bilgilerini görmem gerektiğinde de (özellikle IP adresini)

```bash
docker inspect {container adı}
docker inspect {ağ adı}
```

## Ben Neler Öğrendim

Doğruyu söylemek gerekirse [Saturday-Night-Works](https://github.com/buraksenyurt/saturday-night-works) çalışmalarının herbirisi bana tahmin ettiğimden de çok şey öğretiyor. 33 numaralı örnekten yanıma kar olarak kalanları şöyle sıralayabilirim.

- KONG'un temel olarak ne işe yaradığını
- .Net Core ve Node.js tabanlı servis uygulamaları için Dockerfile dosyalarının nasıl hazırlanacağını
- KONG a bir servis ve route bilgisinin nasıl eklenebileceğini
- Bolca Docker terminal komutunu
- Docker Container içine açılan uygulamaların asıl IP adreslerini nasıl görebileceğimi

Bu macerada API Gateway olarak kullanılabilen KONG isimli ürünü bir Linux platformunda docker imajları üzerinde deneyimlemeye çalıştık. Böylece geldik bir maceramızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
