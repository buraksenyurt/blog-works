---
layout: post
title: "Bir Python Uygulamasını git Tekniği ile Azure Platformuna Taşımak"
date: 2019-09-09 12:30:00 +0300
categories:
  - python
tags:
  - python
  - bash
  - dotnet
  - docker
  - ruby
  - nodejs
  - github
---
Rey evrenin taaa bir ucundan kalkıp ahch-to gezegenine gelmiş ve Jedi ustasının onu eğitmesini istemişti. Galaksinin bir kez daha Luke Skywalker'a ihtiyacı vardı. Uzun zamandır inzivada olan Luke ise Kylo Ren'den sonra buna pek gönüllü değildi. İzleyenler bilir. Luke, neredeyse sadece sudan ibaret ahch-to gezegenindeki bir adada, eski Jedi tapınağında yaşamını sürdürmektedir (Sevgili [ekşi sözlük yazarı John Harrison](https://eksisozluk.com/entry/78992085) bu girişi beğenmeyecektir ama olsun:D)

![ahchto.png](/assets/images/2019/ahchto.png)

Peki bu gezegeninin gerçekte nerede olduğunu biliyor musunuz?

Ahch-to, İrlanda'nın Kerry ilinin yaklaşık 11.6 km batısında yer alan Skellig Michael isimli bir ada aslında. Atlantik okyanusunda yer alan bu küçük ada üstünde 6ncı yüzyılda kurulan bir de manastır bulunuyor. Zaten filmde de Luke'un inzivaya çekildiği yer benzer dini ve mistik karakteristikliklere sahip. Tarihi yapısını önemli derecede korumuş ve UNESCO tarafından 1996 yılında Dünya Mirasları arasına alınmış bu adanın benim için anlamı ise yeni bir macera.

Nitekim o cumartesi gecesi Westworld'ü (Ubuntu 18.04, 64bit) kenara koymuş çok sık kullanılan Azure öğretilerinden birisini ahch-to üstünde deniyordum. Öğretinin temel amacı git yardımıyla Azure üzerinden deployment işlemi başlatabilmekti. Ben bunu bir python uygulaması için denemek istiyordum. Bu sefer işim biraz daha zorluydu. Çünkü [cumartesi gecesi çalışmaları](https://github.com/buraksenyurt/saturday-night-works)nın ikinci fazını yapmayı planladığım ahch-to (Mac Mini, High Sierra) adasındaydım. Derken düğmeye bastım ve öğretiyi uygulamaya koyuldum. Pek tabii ahch-to gezegeninde eksikler vardı...

## Ön Gereksinimler ve Kurulumlar

Geliştireceğimiz örnek python flask paketini kullanan basit bir web uygulaması olacak. ahch-to sisteminde python'un 2.7 sürümü mevcut (ki bu sierra sürümü ile yüklü olarak geldi) lakin ben 3 üzeri bir versiyon kullanmak istiyorum. Bu nedenle homebrew paket yönetim aracından yararlanarak yeni bir kurulum gerçekleştirdim (Tabii homebrew de sistemde yoktu. Nasıl kurulacağının keşfini size bırakıyorum)

```bash
brew update
brew install python
```

### Azure CLI Kurulumu

Paket yöneticisini kurduktan sonra Azure tarafındaki işlemler için yararlanılan CLI (command-line interface) aracını da kurmak gerekiyor. Onu kurmak için ilk terminal komutunu kullanabiliriz. Pek tabi kurulum yeterli değil. Azure tarafı ile konuşabilmek için login işlemini de yapmamız lazım. İkinci terminal komutu bunun için (Bu aşamada Azure'da bir hesabınız olduğunu varsayıyorum)

```bash
brew install azure-cli
az login
```

![08_41_credit_1.png](/assets/images/2019/08_41_credit_1.png)

### Azure Deployment Hazırlıkları

Uygulamayı Azure tarafına deploy edebilmek için de yapılması gerekenler var. Sırasıyla deployment user, resource group, service plan ve son olaraktan da bir web app oluşturmalıyız. Bunlar için aşağıdaki terminal komutlarını kullanarak ilerleyebiliz.

```bash
as webapp deployment user set --user-name dpyl-usr-buraks --password <azure kurallarına uyan bir şifre>
az group create --name rg-todoshero --location westeurope
az appservice plan create --name plan-todoshero --resource-group rg-todoshero --sku B1 --is-linux
az webapp create --resource-group rg-todoshero --plan plan-todoshero --name todosherowebapp --runtime "PYTHON|3.7" --deployment-local-git
```

İlk satırda dply-usr-buraks isimli bir kullanıcı tanımlıyoruz. Deployment işlemlerini bu kullanıcı yapacak. İkinci satırda veri tabanı, servis planlaması, kurulumu yapılacak uygulamalar gibi bu işimizle ilgili kaynakları grupladığımız bir tanımlama bulunuyor (Örneğin Resource Group'u platformdan kaldırdığımızda bu grup altında hazırladığımız ne kadar Azure enstrümanı varsa silinecektir) İşimizle ilgili kaynakları tek noktadan yönetmek için açtığımızı düşünebilirsiniz. Üçüncü komutta bir servis planı oluşturmaktayız. Burada basic ödeme şartlarına göre oluşturulan ve linux tabanlı docker container kullanılan bir plan söz konusu. Son terminal komutu ile todosherowebapp isimli bir web uygulaması oluşturuyoruz. Bu servis uygulaması biraz önce oluşturlan servis planına göre hazırlanacak ve Python 3.7 sürümü ile çalışacak. Sonda yer alan --deployment-local-git parametresi ile dağıtım planımızı (git ile yapacağımızı) belirtiyoruz.

Terminalden çalıştırdığım komutlar başarılı olunca aşağıdaki sonuçla karşılaştım.

![08_41_credit_3.png](/assets/images/2019/08_41_credit_3.png)

Uygulamanın web adresi todosherowebapp.azurewebsites.net olarak belirlenirken, github repository adresi de https://dply-usr-buraks@todosherowebapp.scm.azurewebsites.net/todosherowebapp.git şeklinde oluştu. Görüldüğü üzere varsayılan bir hoş geldin sayfamız bile var. Hatta doğrudan dokümantasyonlarına ulaşıp ilk geliştirmelerimizi yapabiliriz de (Şu an aktif değil. Malum kullanılmayacak bir servis olacağından sildim)

![08_41_credit_4.png](/assets/images/2019/08_41_credit_4.png)

## Uygulamada Yapılanlar

Öncelikle local ortamımızda neler yaptığımız bir bakalım. Python tarafındaki örneğimiz son derece basit. Klasör ve dosya ağacı aşağıdaki gibi oluşturulabilir. Kritik noktalardan birisi requirements.txt dosya içeriği.

```bash
cd src
mkdir todayshero
touch todayshero/app.py
touch todayshero/requirements.txt
touch todayshero/.gitignore
```

Python kodumuzu içeren app.py aşağıdaki gibi yazılabilir. Uygulama, web tarafından route adrese gelen taleplere karşılık içindeki heros isimli listeden rastgele isim döndürmek üzere tasarlanmış durumda.

```text
from flask import Flask # basit web özelliklerini kazandırmak için
from random import seed
from random import randint  # Rastgele sayı üretmek için

app = Flask(__name__)

# bir kahraman listemiz var
heros = ["thor", "wolverine", "iron man", "hulk", "doctor strane"
         "kira", "superman", "batman", "wonder woman"]

# kök adrese talep geldiğinde devreye giren metodumuz
@app.route("/")
def getRandomHero():
    randomIndex = randint(0, len(heros)-1) #0 ile listedeki eleman sayısı aralığında rastgele bir tam sayı üretiyoruz
    return '<h2>'+heros[randomIndex]+'</h2>' # sonucu html olarak dönüyoruz
```

Requirements.txt, Azure platformunun Python ortamlı deploy işlemi için kullanacağı bir doküman. Bu dosya içerisine yazılan paketler, azure deploy işlemi sırasında pip ile uzak sunucu ortamına yüklenmeye çalışılır. Örneğe göre son derece sade bir içeriğimiz var (:

```text
Flask==1.0.2
```

> Bu arada ahch-to üzerindeki denemeler için flask paketini geliştirme ortamına da yüklememiz gerekiyor.
> ```bash
> pip3 install flask
> ```

## Çalışma Zamanı (Local ortamda)

Kod tarafı hazır olduğuna göre uygulamayı çalıştırıp sonuçlarını değerlendirebiliriz. İlk olarak local makine üzerinden aşağıdaki terminal komutu ile ilerleyelim.

```bash
FLASK_APP=app.py flask run
```

![08_41_credit_2.png](/assets/images/2019/08_41_credit_2.png)

Sayfaya yeni talepler gönderdikçe farklı kahramanlar ile karşılaşmamız gerekiyor. Bu basit uygulama kodu list içeriğinden seçtiği rastgele bir karakterin adını ekrana yazdırmakla görevli (git ve azure ikilisinin bir arada kullanılması üzerine yoğunlaştığımızdan mümkün mertebe basit bir örnek kullanıyoruz)

## Çalışma Zamanı (Git Deploy)

Artık programın çalıştığından eminiz. Dolayısıyla asıl dağıtım operasyonuna başlayabiliriz. Bunu git aracılığıyla yapmak için aşağıdaki terminal komutlarını çalıştırmamız yeterli (todoshero klasörü altında)

```bash
git init
git remote add azure https://dply-usr-buraks@todosherowebapp.scm.azurewebsites.net/todosherowebapp.git
git add .
git commit -m "Application has been added"
git push azure master
```

Standart git komutları ile uygulamayı azure reposuna deploy ettik. İlk olarak initialize işlemi var. Sonra uzak repo adresini ekliyoruz. Tüm kod dosyalarını. ile alıp commit ettikten sonra push çağrısı ile değişikliklerimizi yolluyoruz.

push işlemini takiben azure sitesine tekrar gittiğimde python uygulamasının başarılı bir şekilde etkinleştiğini görmemiz lazım. Aşağıdakine benzer bir durum olmalı.

![08_41_credit_5.png](/assets/images/2019/08_41_credit_5.png)

Hatta Azure portaline baktığımızda hem oluşturulan resource group içeriğini hem de yaptığımız son push işlemlerini de görebilmeliyiz.

![08_41_credit_6.png](/assets/images/2019/08_41_credit_6.png)

![08_41_credit_7.png](/assets/images/2019/08_41_credit_7.png)

Hepsi bu!:) Siz de farklı uygulama geliştirme ortamları için (ruby,.net core, node.js vb) aynı kurguyu gerçekleştirmeyi deneyebilirsiniz.

## Ben Neler Öğrendim?

Yazılım ürünlerinin resmi sitelerinde yer alan bu tip adım adım serileri konuyu artık çok iyi öğretiyor. Mesleğe ilk başladığım zamanlarda neredeyse yok denecek kadar azdı. Hatta MSDN'in bile ilk yıllarındaki içerik kalitesi oldukça karışıktı. Ancak yeni nesil bu konuda epey şanslı. Peki [saturday-night-works](https://github.com/buraksenyurt/saturday-night-works) çalışmalarının [birinci fazının bu son örneği](https://github.com/buraksenyurt/saturday-night-works/tree/master/No%2041%20-%20Python%20to%20Azure%20With%20Git)nde ben neler öğrendim dersiniz.

- brew ile macOS platformuna paket yüklemeyi
- azure CLI ile deployment user, resource group, service plan ve web app oluşturmayı
- git komutları ile kodu azure'a atmayı
- requirements.txt dosya içeriğinin ne işe yaradığını

Böylece geldik bir maceramızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
