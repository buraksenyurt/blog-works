---
layout: post
title: "Şu Docker-Compose Dedikleri"
date: 2020-02-28 20:23:00 +0300
description: "Aslında birden fazla Container söz konusu olduğunda bunların yönetimi için kullanılan araçlardan birisi Docker-Compose. Senaryoda farklı dillerde yazılmış üç servisi kullanmak istiyorum. Birisi .Net Core ile yazılmış Web Api, diğeri Flask kullanan Python ve sonuncusu da Express bazlı NodeJs uygulaması. Servislerin ne iş yaptığı çok önemli değil. Odaklanmamız gereken nokta Dockerfile içerikleri ile docker-compose.yml üstünde Container kompozisyonunun nasıl tasarlandığı olacak. Buradaki servislere birde veritabanı bağımlılığı eklemeyi denemek gerçek hayat senaryolarını tatbik etmek adına iyi olabilir. Örneğin Nodejs uygulamasının Postgresql kullanacak şekilde dockerize edilmesi ve Postgresql için docker-compose'da bir imajın kullanılacağının ifade edilmesi iyi bir pratik olacaktır. Üstelik bu tip bir kurgu eğer eğitmenseniz de çok işinize yarayacaktır. Biz şimdi işimize geri dönelim."
categories:
  - dotnet-core
tags:
  - dotnet-core
  - bash
  - javascript
  - csharp
  - yaml
  - dotnet
  - aspnet
  - linq
  - postgresql
  - json
  - web-api
  - http
  - docker
  - python
  - nodejs
  - caching
  - generics
  - github
---
# Şu Docker-Compose Dedikleri
![docker-compose.png](/assets/images/2020/docker-compose.png)

Önceki çalışmalardan [birisinde](Yine Yeni Yeniden ELK(Bu sefer E ve K için docker-compose işin içinde).md) ELK kurgusunda ElasticSearch ve Kibana tarafı için docker compose'dan yararlanmıştım. Orada iş nispeten daha kolaydı. Var olan docker imajlarını bir kompozisyon çerçevesinde düşünerek ele almıştım. Ama kendi servislerimizden oluşan bir kompozisyon gerekirse nasıl bir yol izlenebilir merak da ediyordum. İşte yılın o vakitleri bunu öğrenmeye çalışmışım.

Aslında birden fazla Container söz konusu olduğunda bunların yönetimi için kullanılan araçlardan birisi Docker-Compose. Senaryoda farklı dillerde yazılmış üç servisi kullanmak istiyorum. Birisi.Net Core, diğeri Python ve sonuncusu da NodeJs ile yazılabilir. Python tarafında Flask, NodeJs tarafında da Express paketlerini kullanabiliriz. Servislerin ne iş yaptığı çok önemli değil. Odaklanmamız gereken nokta Dockerfile içerikleri ile docker-compose.yml üstünde Container kompozisyonunun nasıl tasarlanacağı.

Buradaki servislere birde veritabanı bağımlılığı eklemeyi denemek gerçek hayat senaryolarını tatbik etmek adına da iyi olabilir. Örneğin Nodejs uygulamasının Postgresql kullanacak şekilde dockerize edilmesi ve Postgresql için docker-compose'da bir imajın kullanılacağının ifade edilmesi iyi bir pratik olacaktır. Üstelik bu tip bir kurgu eğer eğitmenseniz de çok işinize yarayacaktır. Lakin ben her zaman ki gibi kolaya kaçıyorum ve sabit içerikler döndüren servisler inşa ederek ilerlemeyi planlıyorum. İşe klasör ağacı ve kodlar yazarak başlayalım.

## Hazırlıklar, Kodlama ve Dockerfile...

Klasör ağacı ve gerekli dosyaları aşağıdaki gibi oluşturabiliriz. Bu arada ben Heimdall (Ubuntu-20.04) üzerinde çalışmaktayım. Siz farklı bir platformda olabilirsiniz ama ilkeler temel olarak aynı.

```text
src
---docker-compose.yml
---BookApi
------Dockerfile
---SportApi
------Dockerfile
---WordApi
------Dockerfile
```

Gerekli komutlarımız.

```bash
dotnet new webapi -o SportApi
touch SportApi/Dockerfile
touch docker-compose.yml

mkdir BookApi
touch BookApi/main.py
touch BookApi/Dockerfile
touch BookApi/requirements.txt

mkdir WordApi
cd WordApi
npm init
touch index.js
touch Dockerfile
npm install --save express cors body-parser
Kompozisyonun Hazırlanması ve Çalıştırılması
sudo docker-compose build
sudo docker-composse up
```

## Kod Tarafı

İlk olarak Nodejs tarafında yazdığımız WordApi'ye bakalım. index.js içeriğini aşağıdaki gibi geliştirebiliriz.

```javascript
const express = require('express');
const bodyParser = require('body-parser');
const cors = require('cors');

const config = {
    port: 3000,
    host: '0.0.0.0',
};

const app = express();

app.use(bodyParser.json());
app.use(cors());

app.get('/lucky', (req, res) => {
    res.status(200).send('Sturdy: (of a person or their body) strongly and solidly built');
});

app.listen(config.port, config.host, (e)=> {
    if(e) {
        throw new Error('Internal Server Error');
    }
    console.log('Word Api is running and listening on 0.0.0.0:3000.')
});
```

Dockerfile içeriği ise aşağıdaki gibi kurgulanabilir.

```text
FROM node:14-alpine
RUN mkdir -p /usr/src/app
WORKDIR /usr/src/app
COPY . .
RUN npm install
EXPOSE 3000
CMD ["npm", "run", "start"]
```

Flask kullandığımız BookApi'ye ait kodlarımız ve Dockerfile ise aşağıdaki gibi yazılabilir.

```text
from flask import Flask, jsonify
from flask_restful import Resource, Api 
  
app = Flask(__name__) 

@app.route('/book')
def todaysbook():
    return jsonify(
        {'title': 'Learning Docker Compose'
        ,'description':'A Simple learning book about docker-compose tool'
        ,'publishdate':'2020'
        })
```

ve Dockerfile.(Çalışma zamanının ihtiyaç duyacağı paketlerin Requirements.txt içerisinden bildirildiğine dikkat edelim)

```text
FROM python:3.7-alpine
WORKDIR /code
ENV FLASK_APP main.py
ENV FLASK_RUN_HOST 0.0.0.0
RUN apk add --no-cache gcc musl-dev linux-headers
COPY requirements.txt requirements.txt
RUN pip install -r requirements.txt
COPY . .
CMD ["flask", "run"]
```

Son olarak.Net Core ile yazdığımız SportsApi tarafına bakalım.

Event.cs isimli model sınıfımız,

```csharp
using System;

namespace SportApi
{
    public class Event
    {
        public DateTime Date { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
    }
}
```

Controller içeriği,

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SportApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventController : ControllerBase
    {
        private readonly ILogger<EventController> _logger;

        public EventController(ILogger<EventController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public Event GetTodaysEvent()
        {
            return new Event{
                Date=DateTime.Now,
                Title="Günün sportif başarısı...",
                Description="Bugün olan sportif olayın başarısına ait bilgiler"
            };
        }
    }
}
```

ve tabii en önemlisi de Dockerfile.

```text
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY /. ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "SportApi.dll"]
```

Her üç uygulama kendine has çalışma zamanlarına sahip. Bu nedenle Dockerfile içerikleri birbirlerinden farklı. Örneğin herbiri kendisi için gerekli olan docker imajlarını baz almakta.

## Docker-Compose

Çalışmanın en kritik noktası tahmin edeceğiniz üzere Docker-compose.yml içeriği. Bu dosyayı aşağıdaki şekilde kurgulayabiliriz.

```yml
version: "3"
services:
    sportapi:
        container_name: sport_container
        build: ./SportApi
        ports:
            - "5555:80"
    bookapi:
        container_name: book_container
        build: ./BookApi
        ports:
            - "5000:5000"
    wordapi:
        container_name: word_container
        build: ./WordApi
        ports:
            - "4000:3000"
```

Services kısmında üç container tanımı yer alıyor. Herbirisi için container_name ile bir isim belirtilmekte. build bildirimi ile dockerfile'ların yolunu da belirtmiş oluyoruz. Container'lar ayağa kalktığında dışarıya açılacak olan port bilgisi ile bu portun nereye yönlendirileceği de ports kısmında tanımlanmakta. Örneğin 5555 portuna gelecek SportApi talepleri içerideki 80 portuna yönlenecekler ki, Asp.Net Web API servisi Dockerfile bilgilerine göre bu porttan hizmet sunuyor. Artık tek yapmamız gereken aşağıdaki terminal komutunu kullanarak servislerin ayağa kalkmasını sağlamak ve onları deneme amaçlı olarak tüketmek.

```bash
sudo docker-compose up
```

![skynet_13_Screenshot_1.png](/assets/images/2020/skynet_13_Screenshot_1.png)

```bash
curl http://localhost:5555/Event
curl http://localhost:4000/lucky
curl http://localhost:5000/book 
```

Ekran görüntüsünden de görüleceği üzere up komutu ile servisler ayağa kalktıktan sonra curl aracı ile birkaç talep gönderiyoruz. Sizin de benzer şekilde servis çağrılarınızdan cevaplar alabilmeniz gerekiyor. Antrenman bitince kompozisyona dahil olan container'ları kaldırmak için sudo docker-compose down komutunu kullanabiliriz. Size tavsiyem örnekteki servisleri başta da belirttiğim üzere kendi veritabanı sistemleri ile çalışır hale getirmeye çalışmanız. Hatta bu veritabanlarını da docker-compose içerisinde planlayabilirsiniz diye düşünüyorum. Örnek kodlara [skynet github reposu](https://github.com/buraksenyurt/skynet/tree/master/No%2013%20-%20What%20is%20Docker%20Compose)ndan ulaşabilirsiniz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.