---
title: "Docker Yerine Podman (Pod Manager) Kullanmak"
pubDate: 2020-12-27 09:14:00
categories:
  - DevOps
tags:
  - DevOps
  - Bash
  - JavaScript
  - .NET
  - MongoDB
  - NoSQL
  - RabbitMQ
  - JSON
  - Web API
  - HTTP
  - Docker
  - Kubernetes
  - Node.js
  - GitHub
---

# Docker Yerine Podman (Pod Manager) Kullanmak
![anakinspod.png](images/anakinspod.png)

Heimdall üstünden birşeyler kurcalamak istediğimde yolum genellikle bir Docker imajı ile kesişiyor. Bakmak istediğim bir NoSQL veritabanı mı var, ELK üçlüsü mü gerekli, bir NGinx server ortamımı lazım ya da yeni bir servis için çalışma zamanımı hazırlamam gerekiyor... Hemen Docker kardeşimizin kapısını çalıyorum. Aslında bakarsanız Container teknolojileri denince çoğumuzun aklına Docker'dan başka bir şey gelmiyordur belki de. "Gerçekten de böyle mi?" diye düşündüğüm bir ara Docker'ın güçlü bir alternatifi olan Podman isimli ürünle karşılaştım ve onu biraz tanımaya karar verdim.

Esasen Docker tek ve vazgeçilmez bir container aracı olarak düşünülmemeli. Sonuçta Open Container Initiative tarafından belirlenmiş standartlara uyan araçlar mevcut. [Open Container Initiative](https://opencontainers.org/), bu tip araçlarda üç temel özelliğin olmasını vurguluyor. Container çalışma zamanı (runtime), dağıtım stratejisi (distribution) ve baş aktör olarak da image. Podman bu standartlara uyan araçlardan birisi. Buna göre Podman ile hazırlanan imajlar Docker ile veya XYZ isimli başka bir Container ile de uyumlu oluyor (Zaten stadartların amacı da bu değil mi? Farklı ürünlerin birbirleri yerine tercih edilip kullanılabilmesi için ortak bir yöntemler kılavuzu sunmak)

Red Hat tarafından açık kaynak olarak geliştirilen Podman özünde Pod (Kubernetes'in en küçük işlem birimi olarak bahsi geçen lakin benim aklıma hep Anakin Skywalker'ın yarış aracını getiren) sistemine dayanıyor. Dolayısıyla Podman ortamından Kubernetes üzerine göç etmek (migration) oldukça kolay. Pod içerisinde birden fazla Container kullanılabiliyor ve ayrıca Docker da olduğu gibi daemon ihtiyacı bulunmuyor. Zaten temel fark her ikisinin farklı mimari kullanmaları. Docker, client-server temelli bir mimariyi baz alıyor. Client görevi üstlenen CLI arabirimi (ki biz onun komutlarını kullanıyoruz) arka planda (Server side) image nesnesi inşa etmek ve container çalıştırmak gibi işleri üstlenen daemon ile iletişim halinde. CLI'ın Daemon ile olan bu iletişimi Root kullanıcı yetkilerine ihtiyaç duymakta.

Podman ise bunun aksine Root kullanıcı şart koşmuyor çünkü Daemonless bir mimari kullanmakta. Standart bir kullanıcı söz konusu ise onun için açılan özel çalışma sahası (namespace) kullanılıyor. Buna göre her kullanıcı sadece kendi Container örnekleri ile çalışıyor. Root kullanıcı yetkisi gerekmemesinin başka bir avantajı daha olabilir. Container başka bir kullanıcı tarafında ele geçirilse bile Root kullanıcı yetkilerine sahip olmadığından sadece o workspace üzerindeki yetkilerle sınırlı kalacaktır.

Podman, Image oluşturmak için Buildah ve uzak repolara image kaydetmek (Register) için skopeo gibi araçları kullanır. Diğer yandan öğrendiğim kadarıyla sadece Linux sistemlerde çalışıyor (Windows Subsystem for Linux bir istisna olabilir mi bakmak lazım) Bununla birlikte Docker Compose'un Podman tarafında henüz bir karşılığı yok (Yazıyı hazırladığım tarih itibariyle doğrulanmamış bir bilgidir. Lütfen araştırınız) Haydi gelin bu Podman nasıl kurulur, temel terminal komutları nelerdir ve onunla bir Poderize işlemi nasıl gerçekleştirilir bir bakalım.

## Önce Kurulum

Bu seneki pek çok SkyNet çalışmasında olduğu gibi ben denemelerimi Heimdall (Ubuntu-20.04) üzerinde yapmaktayım. Ancak diğer platformlar içinde aynı terminal komutları ve prensipler söz konusu olacaktır. Linux tarafı için şağıdaki şekilde konuya giriş yapabiliriz (Güncel kurulum bilgilerine [resmi adresinden](https://podman.io/getting-started/installation) bakılabilir)

```bash
# Podman resmi dokümantasyonundaki adımları takip ederek kurulumu yaptım
. /etc/os-release
echo "deb https://download.opensuse.org/repositories/devel:/kubic:/libcontainers:/stable/xUbuntu_${VERSION_ID}/ /" | sudo tee /etc/apt/sources.list.d/devel:kubic:libcontainers:stable.list
curl -L https://download.opensuse.org/repositories/devel:/kubic:/libcontainers:/stable/xUbuntu_${VERSION_ID}/Release.key | sudo apt-key add -
sudo apt-get update
sudo apt-get -y upgrade 
sudo apt-get -y install podman

# Sonrasında bir versiyon kontrolü de yaptım
podman -v
```

Kurulum sonrasında birkaç komutla Podman'i inceleyebiliriz. İlgi çekici kısım bir pod oluşturmak ve bu pod içerisine n sayıda container yerleştirmek olacak tabii ki ama önce temeller.

```bash
# Önce bir pod oluşturalım (adı pod_race olsun)
podman pod create --name pod_race

# Şimdi sistemdeki pod listesine bakalım
podman pod list

# Bu pod içinde bir tane alpine imajından container başlatalım
podman run -d --pod pod_race alpine

# Hadi bir tane de nginx imajından http server container'ı çalıştıralım (aynı pod içinde)
podman run -d --pod pod_race nginx

# Hatta bir tane de rabbitmq container'ı başlatalım. O da aynı pod içinde olsun.
podman run -d --pod pod_race rabbitmq

# Şimdi pod_race isimli pod'un içindeki container'lara bakabiliriz
podman ps -a --pod

# Aşağıdaki komutla yüklü olan image örneklerini de görebiliriz
podman images

# Bir podu aşağıdaki komutla durdurabiliriz. Bu, içindeki Container'ları da durduracaktır.
podman pod stop pod_race

# Pek tabi oluşturulan bir podu, içindeki tüm container örnekleri ile birlikte silebiliriz de
podman pod rm pod_race
```

Yukarıdaki temel çalışmaların kısa bir özeti aşağıdaki ekran çıktısındaki gibidir.

![skynet_40_Screenshot_01.png](images/skynet_40_Screenshot_01.png)

Podman ile uzak depolardaki imajları kolayca arayabiliriz de. Mesela sevgili MariaDB imajlarını aradığımızı ve 20 yıldız üstünde olup automated özellikli olanları bulmak istediğimizi düşünelim...

```bash
podman search mariadb --filter=stars=20 --filter=is-automated

# ya da resmi bir imaj arayıp açıklamaların da tamamını(--no-trunc) istersek şunu kullanabiliriz
podman search mariadb --no-trunc --filter=is-official

# Hatta çıktı tablosundaki kolonlardan sadece istediklerimizi de mustache stilindeki parametrelerle değiştirebiliriz
podman search --format "table {{.Name}} {{.Stars}}" mariadb --filter=stars=20

# Uzak repolardaki kendi imajlarımızı da aratmak isteyebiliriz.
# Mesela Quay.io'da ki imajlarımızı aratmak istediğimizi düşünelim.
# Aşağıdaki komutla bunu yapabiliriz?
# Lakin kuvvetle muhtemel öncesinde Quay.io için Login olmamız gerekebilir. Podman bunu da sağlar.
podman login quay.io
podman search quay.io/
```

Uzak diyarlardaki imajları terminalden nasıl arayacağımızı gördük. Bazen belli bir imajın özelliklerini onu sisteme indirmeden detayda da öğrenmek isteyebiliriz. Bunun için Skopeo aracından yararlanıyoruz. Örneğin alpine imajının son sürümü ile ilgili bir takım temel özellikleri şu komutla öğrenebiliriz.

```bash
skopeo inspect docker://docker.io/alpine:latest
```

Yukarıdaki terminal komutlarına ait Heimdall çıktıları ise aşağıdaki gibi oluşmuştur.

![skynet_40_Screenshot_02.png](images/skynet_40_Screenshot_02.png)

> Podman varsayılan kurulumunda image registery adresleri olarak docker ve quay geldi. Başka adresler eklemek istersek (mesela private repo'lar) /etc/containers/registries.conf dosyasını düzenlemek gerekir.

Şimdi podman ile bir imaj hazırlayıp onu build etmeyi deneyelim. Tahmin edileceği üzere bize kobay bir uygulama lazım ve bunun en basit yolu aptal bir NodeJs servisi. Express web çatısını kullanan bu servisi aşağıdaki terminal komutları ile hazırlayıp kodlayarak devam edelim.

```bash
mkdir pingapi
cd pingapi
npm init -y
touch index.js
npm i express
touch Dockerfile
```

index.js içeriğini aşağıdaki gibi yazabiliriz. Sadece masa tenisinin güzel bir spor olduğunu ve bazen her şeye ara verip biraz masa tenisi oynamanın iyi olacağını söyleyeyen bir servis;)

```javascript
const express = require('express')
const app = express()

app.get('/ping', function (request, result) {
    result.send('Biraz ara verip Ping Pong oynayalım mı?')
})

app.listen(5555, "0.0.0.0", function () {
    console.log('Servisimiz http://localhost:5555/ping adresinden denenebilir.')
})
```

Tabii inşanın olmazsa olmazı Dockerfile dosyamız. Node11 sürümünü baz alarak uygulamamızı olduğu gibi kopyalayıp 5555 nolu port üstünden açacak bir çalışma zamanı ortamını tanımlıyor.

```text
FROM node:11
WORKDIR /app
COPY package*.json  ./
RUN npm install
COPY . .
EXPOSE 5555
CMD ["node", "index.js"]
```

Kobay servisimiz Dockerize (Poderize) edilmeye hazır;) İzleyen terminal komutları ile onu inşa edelim, sorasında çalıştırıp kullanmayı deneyelim.

```bash
# Önce Build işlemini yapalım
podman build -t ping-api .

# İmajın oluşup oluşmadığı kontrol ettikten sonra onu
# çalıştırıp api'den değer alıp alamadığımıza bakmak iyi olabilir ;)
podman images
podman run -p 5555:5555 ping-api
podman ps -a
curl http://localhost:5555/ping

# Çalışmakta olan container'ı durdurmak içinse aşağıdaki komutu kullanabiliriz
# (337f id değeri tabii ki siz denerken farklı olacaktır)
podman stop 337f
```

İşte çalışma zamanı çıktısı. Podman ile Container ayağa kalktıktan sonra servisi masa tenisi oynamaya götürebiliriz.

![skynet_40_Screenshot_03.png](images/skynet_40_Screenshot_03.png)

Tabii stop komutu ile ilgili container durdurulduğunda servise gönderilen talepler cevapsız kalır. Bu bir nevi Container tatile çıktığında servisin çalışmaması gerektiğinin de bir ispatıdır.

![skynet_40_Screenshot_04.png](images/skynet_40_Screenshot_04.png)

## Skopeo

Podman ile ilgili bilgileri araştırırken yanında yardımcı başka araçlar da görebiliriz. OCI ilkelerine göre imaj oluşturmayı kolaylaştıran [Buildah](https://buildah.io/) (Kanımca Build Yeaaa diye telafuz ediliyor) veya yukarıda bir image nesnesinin detay özelliklerini öğrenmek ve aynı zamanda depolar arası container transferlerini (kendi deponuzla docker.io veya quay.io gibi public registery noktaları arasında taşımak) kolaylaştıran [skopeo](https://github.com/containers/skopeo) gibi. Skopeo için Linux tarafında aşağıdaki adımları takip ederek kurulum yapabilirsiniz.

```bash
. /etc/os-release
sudo sh -c "echo 'deb http://download.opensuse.org/repositories/devel:/kubic:/libcontainers:/stable/x${NAME}_${VERSION_ID}/ /' > /etc/apt/sources.list.d/devel:kubic:libcontainers:stable.list"
curl -L https://download.opensuse.org/repositories/devel:/kubic:/libcontainers:/stable/x${NAME}_${VERSION_ID}/Release.key | sudo apt-key add -
sudo apt-get -y update
sudo apt-get -y install skopeo
```

Şimdilik bu kadar. Gelelim bu çalışma haricinde daha neler yapabileceğinize. Örneğin pod_a ve pod_b şeklinde iki ayrı pod oluşturup içlerindeki container'ların birbirlerini kullanmasını deneyebilirsiniz. Yani pod_a'da ki bir.net web api, pod_b'deki MongoDb container'ını kullanmaya çalışabilir mi sorusunun cevabını arayabilirsiniz. Diğer yandan Podman benzeri OCI standartlarına uyan başka container teknolojileri var mı araştırmakta ve hatta aralarındaki kıyaslamalara bakmakta yarar var.

Böylece geldik bir SkyNet derlememizin daha sonuna. Bu çalışmamızda kobay bir NodeJs servisini Poderize (Dockerize yerine bunu kullanayım dedim) etmeyi ve Podman'in genel kullanımını öğrendik. Örnek kodlara [github reposu](https://github.com/buraksenyurt/skynet/tree/master/No%2040%20-%20This%20is%20Podman) üzerinden erişebilirsiniz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
