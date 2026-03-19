---
title: "AWS Elastic Beanstalk Macerası"
pubDate: 2018-02-16 05:00:00
categories:
  - AWS
  - python
tags:
  - AWS
  - python
  - Bash
  - .NET
  - HTTP
  - Docker
  - Java
  - Ruby
  - Node.js
---

# AWS Elastic Beanstalk Macerası
![image.axd](images/image.axd)

Merhaba Arkadaşlar,

Geçenlerde sıkıldığım bir ara kendimi Google'da "How To Draw..." araması yaparken buldum. [Bir internet sitesinde](http://www.drawinghowtodraw.com/stepbystepdrawinglessons/2016/10/draw-cute-kawaii-chibi-robin-dc-comics-batman-robin-easy-steps-drawing-lesson-kids/) DC Comics'in Robin karakterini nasıl çizebileceğimizi anlatan içerik ilgimi çekmişti. Geometri bilgisini iyi kullandığı için anlaşılırdı. Tabii önemli bir eksiğim vardı...Yetenek. Sonuçları sizlerle paylaşmayı çok tercih etmiyorum ama yandaki Robin'in kafasının pek yakınlarından geçemediğimi gönül rahatlığıyla itiraf edebilirim. Dolayısıyla google aramasını ve internet sayfasını kapatıp tekrardan az buçuk anlamaya çalıştığım yazılım dünyasına döndüm.

Aslında bazen öğrenmek istediğimiz konuyu adım adım ve her adımında da tane tane anlatan bir dokümanı takip ederiz. Ama çalıştığımız ortamlar her zaman için bir yerlerde sorunlarla karşılaşmamıza neden olabilirler. Geçtiğimiz cumartesi günü de benzer sorunlarla karşılaştım. Amacım Amazon'un [şu adreste](http://docs.aws.amazon.com/elasticbeanstalk/latest/dg/create-deploy-python-django.html?refid=em_68105) yayınladığı dokümanı takip ederek Elastic Beanstalk üzerine Django ile oluşturulmuş bir web uygulamasını taşıyıp Doğu Amerika kıtasındaki herhangibir Elastic Compute Cloud (Amazon EC2) sistemi üzerinden canlı yayına almaktı. İlk başlarda kolay giden adımlar özellikle sonlara doğru çeşitli sürprizlerle karşılaşmama neden oldu.

Her şey o Cuma günü AWS'de açmış olduğum hesapla neler yapabileceğime bakarken başladı. Bir süre öncesinde [Amazon Lambda hizmetini incelemiş](AWS Lambda Üzerinde .Net Core Koşturmak.md) ve.Net Core ile kullanabildiğimi gördükten sonra epey keyif almıştım. Şimdiki hedefim Elastic Beanstalk ürünüydü. Kısaca Platform as a Service gibi konumlanan bu ürün sayesinde, çeşitli platformları Amazon EC2 örnekleri ile çalışacak şekilde hazırlayabiliyoruz. Platform anlamında oldukça geniş bir ürün yelpazesi de söz konusu. [Buradaki adresten](http://docs.aws.amazon.com/elasticbeanstalk/latest/dg/concepts.platforms.html) detaylarını öğrenebileceğinize gibi.Net Core'dan Go'ya, Python'dan Java'ya, Ruby'den Php'ye, Node.js'ten Docker'a kadar pek çok uygulama için hazır ortamlar söz konusu.

Buradaki hazırlıklarda Infrastructure as a Service gibi konumlanan Amazon EC2 tarafını da pek düşünmemize gerek kalmıyor esasında. Bu noktada EB'nin başarılı bir Deployment aracı olduğunu da ifade edebiliriz. EB'yi kullanırken Amazon Web Console üzerinden bir kaç tıklama ile bir platformu ayağa kaldırıp yayına almamız mümkün. İşte o Cuma akşamı bunu denemiştim. Şekilden de görüleceği gibi üzerinde Python 3.6 ortamı kurulu olan 64bitlik bir Linux makine emrime amadeydi.

![image.axd](images/image.axd)

Hatta hazır şablon olarak gelen bir giriş sayfası da bulunuyordu (What's Next? kısmı da oldukça dikkat çekiciydi. Django ve Flesk hemen dikkatimi çekmişlerdi)

![image.axd](images/image.axd)

Adres satırından da göreceğiniz gibi her şey Amazon'un doğu Amerika bölgesinde bir yerlerde gerçekleşmekteydi (Sanırım) Kuvvetle muhtemel domain'in arkasında o bölgeye ait bir Cloud Server deposu ve EC2 makine örnekleri yer almaktaydı. Yönetim panelini kullanarak bu platform üzerine dosya bırakma yoluyla da taşımalar yapılabiliyordu. Benim merak ettiğim konu ise kendi geliştirme ortamımda yazdığım bir uygulamayı (veya hazır şablondan üretilmiş bir tanesini) komut satırından Elastic Beanstalk ortamına nasıl aktarabileceğimdi.

O zaman maceramıza başlayalım. Yapacaklarımız özetle oldukça basit. West-world üzerinde (ki artık 64bit çalışan Ubuntu 16.04 sistemi olduğunu biliyorsunuz) sanal bir çalışma ortamı hazırlayacağız. Ardından bu ortamda Django çatısını kullanarak hazır bir web şablonu üreteceğiz. Elastic Beanstalk için gerekli olan çevre değişkenlerini ayarladıktan sonra bir Local Repository üretip taşıma adımlarını işleteceğiz.

VirtualEnv Gerekli

VirtualEnv ile linux üzerinde sanal bir ortam hazırlamamız mümkün. Bunu daha çok sistemde var olan paketlerden daha eski veya yeni sürümlerini kullanmak istediğimiz uygulamalarda ele alabiliriz.

```bash
sudo apt-get install virtualenv
```

![image.axd](images/image.axd)

Sanal Ortamın Kurulumu

VirtualEnv aracı hazır olduğuna göre artık sanal ortamın kurulmasına başlanabilir.

```bash
virtualenv ~/beanstalk-virt
```

ile python odaklı sanal ortam kurulur. Oluşan klasör içeriğine bakıldığında Python ile hazırlanmış kod dosyaları ve ortam enstrümanları olduğu görülür. Sürüm olarak Python 2.7 söz konusudur ki Amazon Elastic Beanstalk'da bu Python sürümünü tavsiye etmektedir. Sanal ortamı etkinleştirmek içinse aşağıdaki komutu kullanabiliriz.

```bash
source ~/beanstalk-virt/bin/activate
```

![image.axd](images/image.axd)

Artık West-World'ün üzerinde tamamen izole bir çalışma sahası var.

Sahne Django'nun

Python tarafında web uygulaması geliştirmek için kullanılan en popüler çatılardan birisi Django. Sanal ortama pip paket yöneticisi yardımıyla ilgili çatıyı kurmak gerekiyor.

```bash
pip install django==1.9.12
```

Amazon dokümantasyonuna göre desteklenen Django versiyonu önemli. Ben araştırmalarımı yaptığım sırada Amazon 1.9.12 sürümünün kullanılması tavsiye ediliyordu.

![image.axd](images/image.axd)

Kurulum başarılı bir şekilde tamamlandıktan sonra hemen hazır web şablonu kullanılarak bir proje oluşturabiliriz. Aşağıdaki komut bunun için yeterli.

```bash
django-admin startproject amznWebStore
```

Projenin adı amznWebStore. Klasör içeriğine bakıldığında Python kod dosyalarından oluşan basit bir içerik oluşturulduğu görülebilir. Projeyi bu haliyle local makine üzerinden çalıştırmak istersek manage.py kod dosyasının aşağıdaki gibi çağırılması yeterlidir.

```bash
python manage.py runserver
```

Eğer işler yolunda gittiyse varsayılan olarak localhost'un 8000 numaralı portu üzerinden web içeriğine ulaşabiliriz.

![image.axd](images/image.axd)

Uygulamanın Elastic Beanstalk'a Taşınması

Buraya kadarki işlemlerimizi şöyle bir hatırlayalım. İlk olarak West-World üzerinde sanal bir ortam açtık. Sanal ortam üzerinde Python yüklü olarak geliyordu. Oluşturulan ortamı etkinleştirdikten sonra basit bir Web uygulamasını ayağa kaldırmak için Django Framework'ü kurduk. Hiçbir kod değişikliği yapmadan manage.py dosyasından yararlanarak bu standart şablonun 127.0.0.1:8000 adresinden ayağa kaldırıldığını gördük. Sırada bu uygulamanın Elastic Beanstalk'a taşınması var. Önce bir kaç hazırlık yapmamız gerekiyor. İlk olarak ortam gereksinimlerin text dosyaya alıyoruz.

```bash
pip freeze > requriements.txt
```

![image.axd](images/image.axd)

Dosya içeriğinde aslında tek bir gereksinim var o da taşımanın yapılacağı ortamda Django'nun 1.9.12 sürümünün olmasını istiyor. Takip eden adımda amznWebStore kök dizini altında.ebextensions isimli yeni bir klasör oluşturup içerisine django.config isimli bir dosya atmamız gerekiyor. Bu dosyanın içeriği aşağıdaki gibi.

```text
option_settings:
  aws:elasticbeanstalk:container:python:
    WSGIPath: amznWebStore/wsgi.py
```

Aslında Elastic Beanstalk ortamına söz konusu uygulamayı nereden başlatacağını söylemekteyiz ki bu senaryoda wsgi.py dosyası oluyor. Bu son iki adım bize şunu öğretmekte. Sanal ortamda geliştireceğimiz web uygulaması içerisinde neleri kullanırsak (söz gelimi SQLite gibi bir veritabanı, Flesk çatısı vb) bunların Elastic Beanstalk'a söylenmesi gerekmekte.

Sırada Command Languagen Interface'i kullanarak Local Repository'nin oluşturulması adımı var. Tabii bu aşamada benim gibi aşağıda görülen sorunlarla karşılaşabilirsiniz.

![image.axd](images/image.axd)

İlk olarak eb isimli CLI aracı sisteminizde yüklü olmayabilir. Python ile yazılmış olan bu aracı kullanabilmek için pip yöneticisi ile kurmak gerekiyor. Lakin bu kurulum sanal ortamda gerçekleştirilebilen bir kurulum da değil. Benim düştüğüm bir diğer hata da bu oldu. deactivate komutu ile beanstalk-virt isimli sanal ortamdan çıktıktan sonra gerekli kurulum işlemini yaptım. Sonrasında sanal ortamı etkinleştirip eb init operasyonunu bir kez daha denedim.

Bu sefer daha farklı bir durumla karşılaştım. İlk olarak hangi bölge üzerine taşıma yapmak istediğimi sorduğunu düşündüğüm bir liste ile karşılaştım. Amazon web sitesi üzerinden yaptığım örneği düşünerekten us-east-2 (13ncü bölge) seçimini yaptım. Tabii sonrasında gelen hata mesajı gayet açıktı. Credential bilgilerimi bildirmediğim için yetki hatası almıştım. Hemen [Amazon Web Console](https://console.aws.amazon.com/iam/home#/home)'a geçerek ve AdministratorAccess Policy'sini kullanan westworld-buraksenyurt isimli bir kullanıcı oluşturdum. Access Key ID ve Secret Access Key değerlerini credential bilgilendirmesi için kullanarak adımı tamamlamayı başardım.

![image.axd](images/image.axd)

Sonunda uygulama oluşturuldu. Artık tek yapılması gereken ortamın EB üzerine taşınmasıydı. Aşağıdaki komut ile bunu denedim.

```bash
eb create my-eb-sample-env
```

Evdeki hesap çarşıya uymamış gibiydi.

![image.axd](images/image.axd)

Üstüne üstelik bir de şu vardı.

![image.axd](images/image.axd)

Ne yazık ki sonuç pek beklediğim gibi olmadı. Komut satırı hareketliliklerini izlerken Elastic Beanstalk üzerinde my-eb-sample-env isimli uygulamanın oluşturulduğunu gördüm ancak site üzerinden yaptığım uygulama gibi yeşil değil kırmızı renkteydi. Ayrıca komut satırına requriements.txt dosyasının geçersiz olduğuna dair hata mesajı düşmüştü. Requirements.txt dosyasının oluşturulduğu adıma geri dönerek sorunu anlamaya çalıştım. İlk iş içeriğini aşağıdaki hale getirdim.

```text
Django==1.9.12
```

Başka bir şeye ihtiyacım yoktu çünkü. Tekrardan

```bash
eb deploy
```

ile taşıma işlemini yaptım. Şaşılacak şekilde yeşil oku gördüm. Uygulama Elastic Beanstalk üzerinde sağlıklı bir şekilde ayağa kalkmış gibi duruyordu. Komut satırından

```bash
eb open
```

ile siteyi açtırmak istediğimdeyse yine hüsranla karşılaştım. HTTP_HOST header bilgisinin geçersiz olduğu söyleniyordu.

![image.axd](images/image.axd)

Bunun üzerine amznWebStore klasöründeki settings.py dosyasını açıp ALLOWED_HOSTS değerinin olduğu satırı bulup aşağıdaki hale getirdim.

```text
ALLOWED_HOSTS = ['my-eb-sample-env.prii9kimtp.us-east-2.elasticbeanstalk.com']
```

Sonrasında tekrar deploy ve tekrar open.

```bash
eb deploy
eb open
```

Nihayet! Uygulama Elastic Beanstalk üzerine başarılı bir şekilde taşınmış ve ayağa kaldırılmıştı.

![image.axd](images/image.axd)

Biraz yorucu bir çalışma olduğunu ifade edebilirim ancak West-World üzerinde kurgulanan senaryonun Amazon Elastic Beanstalk üzerinde konuşlanması tatmin ediciydi de diyebilirim. Bu çalışma bana bir çok şey kattı. Bir PaaS'in tam olarak ne işe yarayabileceğini görme fırsatı buldum. Amazon Console penceresini kullanırken her deployment denemesi sonrası web panelindeki anlık hareketleri inceleyip başarılı bir log sisteminin nasıl olabileceğini ve monitoring'in ne kadar önemli olduğunu anladım. Belki Django ile oluşturulan web uygulaması için hiç kod yazmadım ama taşıma öncesinde platformun hangi gereksinimlere ihtiyacı olduğunun nasıl söylenebileceğini gördüm. Her anlamda yararlı bir çalışma olduğunu ifade edebilirim. Umarım sizler için de bilgilendirici olmuştur. AWS tarafını incelemeye devam ediyorum. Yeni bir şeyler öğrendikçe paylaşmaya çalışacağım. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
