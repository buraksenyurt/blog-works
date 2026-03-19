---
title: "Bir Ruby Uygulamasını Google Cloud Platform Üzerine Taşımak"
pubDate: 2018-01-27 17:00:00
categories:
  - Ruby
tags:
  - Ruby
  - Bash
  - REST
  - JSON
  - HTTP
  - Python
  - Java
  - Performance
  - Serialization
  - Visual Studio
---

# Bir Ruby Uygulamasını Google Cloud Platform Üzerine Taşımak
![image.axd](images/image.axd)

Merhaba Arkadaşlar,

"Futbol basit bir oyundur. 22 kişi 90 dakika boyunca bir topun peşinde koşar ve sonunda her zaman Almanlar kazanır." demiş bizim de ne yazık ki yakından tanıdığımız Gary Lineker. Konumuzla ne alakası var derseniz. Az sonra onun bu sözünü buluta alacağız.

Ben bu bulut platformlarını çok tuttum. Gerek Microsoft Azure, gerek Amazon Web Services, gerek Google Cloud Platform...Hepsi çok çekici duruyor. Kurcaladığım ve üzerinde çalıştığım örneklerle, özellikle West-World dünyasında yazılmış bir programın ilgili platform üzerinde konuşlandırılması ve yürütülmesini öğrenmeye gayret ediyorum. Ağırlıklı olarak REST (Representational State Transfer) tipinden servis uygulamalarını taşımaya çalışıyorum ki bu sayede bir mikroservis bulut alanında nasıl kullanılıyor öğreneyim.

Bu seferki hedefimse rastgele özlü söz veren Ruby programlama dili ile yazılmış bir REST servisinin Google Cloud Platform'a taşınarak App Engine Flexible ortamında ayağa kaldırılması. İşlemlerime başlamadan önce Google Cloud Platform üzerinde geçerli bir ödeme seçeneğinin olması gerekiyor (AWS'de de benzer şekilde servislerden yararlanabilmek için geçerli bir kredi kartı bilgisi isteniyordu) Bir ablam olduğu için çok şanslıyım. Sağolsun bu proje için sanal kredi kartını kullanmama izin verdi. Gelin adım adım ilerleyerek ruby servisimizi App Engine Flexible ortamından çalıştıralım.

Google Cloud Tools Olmazsa Olmaz

West-World bildiğiniz üzere Ubuntu 16-04 64bit sürümünü kullanıyor. Bu nedenle sıradaki Google Cloud Tools kurulum işlemi Debian/Ubuntu sistemleri için geçerli. Elbette Windows, MacOSX, RedHat/Centos gibi sistemler için de bu araçtan yararlanabiliriz ([Şu adresten](https://cloud.google.com/sdk/docs/) kendi sisteminiz için uygun olan sürümü indirebilirsiniz) Google Cloud Tools taşıma işlemi sırasında kullanacağımız araç olduğu için yüklememiz gerekiyor. Aşağıdaki terminal komutlarını kullanarak kurulumu yapıyorum.

```bash
export CLOUD_SDK_REPO="cloud-sdk-$(lsb_release -c -s)"
echo "deb http://packages.cloud.google.com/apt $CLOUD_SDK_REPO main" | sudo tee -a /etc/apt/sources.list.d/google-cloud-sdk.list
curl https://packages.cloud.google.com/apt/doc/apt-key.gpg | sudo apt-key add -
sudo apt-get update && sudo apt-get install google-cloud-sdk
sudo apt-get install google-cloud-sdk-app-engine-java
gcloud init
```

Son adımda Google login sayfasına yönlendirildim ve hangi projeyi hangi Google Compute Engine bölgesinde kullanmak istediğime dair iki soruyla karşılaştım. Ben YourFunnyQuote isimli projemi ve uğurlu rakamım 13 ile australia-southeast1-b bölgesini tercih ettim.

![image.axd](images/image.axd)

ve login sonrasında da aşağıdaki ekranla karşılaştım.

![image.axd](images/image.axd)

Bir başka deyişle West-World üzerinden gcloud'u kullanabilmek için kimliğim doğrulanmıştı.

> Diğer bölgeler hakkında görsel bilgiye ihtiyaç duyarsanız [şu adrese](https://cloud.google.com/about/locations/#regions-tab) uğramanızı tavsiye ederim.
> ![image.axd](images/image.axd)

Kurulumun sorunsuz tamamlanıp tamamlanmadığından emin olmak için de şu komutu çalıştırdım.

```bash
gcloud --help
```

![image.axd](images/image.axd)

Bu işlemler sırasında seçtiğimiz proje önemli. Nitekim taşıma işlemi sonrasında kodlarımız bu proje ile ilişkiliendirimiş Instance'a taşınacak.

Ruby Uygulamasının Geliştirilmesi

gcloud aracını yükledikten sonra basit bir Ruby projesi geliştirmeye karar verdim. Ne var ki bu ana kadar West-World üzerinde hiçbir ruby projesi geliştirmemiştim. West-World bu değerli Ruby mücehverinin ne olduğunu bilmiyordu. O yüzden ona kurulması lazımdı.

```bash
sudo apt-get install ruby-full
```

ile tam sürümü yükledim. Kurulum sonrası

```bash
ruby -v
```

terminal komutu ile de yüklenen versiyondan emin oldum.

![image.axd](images/image.axd)

Örnek olarak REST tipinden bir servis geliştirmeyi planlıyordum. İşimi kolaylaştıracak paket ise Sinatra'ydı (Ruby on Rails çatısı da tercih edilebilir tabii ki) Ancak Ruby ile yeni tanışmış olan West-World büyük üstad Sinatra'dan da bihaberdi. Ona "I did it my way" sözlerini fısıldayarak usta sanatçıyı anımsatmaya çalıştım dersem deli olduğumu düşüneceksiniz. Bende bu nedenle

```bash
sudo gem install sinatra
```

ile sisteme Sinatra'yı yüklemeyi uygun gördüm.

Ayrıca serüvenim sırasında Ruby bağımlılıklarının yüklenmesini sağlamak için ruby-bundle aracına da ihtiyacım vardı. Nitekim taşıma işlemi sırasında Google Cloud Platform, Gemfile.lock dosyasının içeriğine bakarak ilgili bağımlılıkların ortama kurulumunu gerçekleştirecekti. Onun standartları buydu.

```bash
sudo apt-get install ruby-bundler
```

Nihayet ruby kodlarını yazmaya başlayabilirdim. Paslanmış olan ruby bilgime aldırmadan Visual Studio Code'u açtım ve uygun bir klasör içerisinde önce app.rb dosyasını ardından özellikle Google için önemli olan app.yaml ve Gemfile içeriklerini oluşturdum.

```text
require "sinatra"
require "json"

set :port, 8080

get "/quotes/random" do
    quotes=Array.new
    quotes<<Quote.new(122548,"Michael Jordan","I have missed more than 9000 shots in my career. I have lost almost 300 games. 26 times, I have been trusted to take the game winning shot and missed. I have failed over and over and over again in my life. And that is why I succeed.")
    quotes<<Quote.new(325440,"Vince Lombardi","We didn't lose the game; we just ran out of time")
    quotes<<Quote.new(150094,"Randy Pausch","We cannot change the cards we are dealt, just how we play the game")
    quotes<<Quote.new(167008,"Johan Cruyff","Football is a game of mistakes. Whoever makes the fewest mistakes wins.")
    quotes<<Quote.new(650922,"Gary Lineker","Football is a simple game. Twenty-two men chase a ball for 90 minutes and at the end, the Germans always win.")
    quotes<<Quote.new(682356,"Paul Pierce","The game isn't over till the clock says zero.")
    quotes<<Quote.new(156480,"Jose Mourinho","Football is a game about feelings and intelligence.")
    quotes<<Quote.new(777592,"LeBron James","You know, when I have a bad game, it continues to humble me and know that, you know, you still have work to do and you still have a lot of people to impress.")
    quotes<<Quote.new(283941,"Roman Abramovich","I'm getting excited before every single game. The trophy at the end is less important than the process itself.")
    quotes<<Quote.new(185674,"Shaquille O'Neal","I'm tired of hearing about money, money, money, money, money. I just want to play the game, drink Pepsi, wear Reebok.")

    content_type 'application/json'
    index=rand(quotes.length-1)
    quote=quotes[index]
    quote.to_json
end

class Quote
    attr_accessor:id
    attr_accessor:owner
    attr_accessor:text
     
    def initialize(id,owner,text)
        @id=id
        @owner=owner
        @text=text
    end
    def to_json(*a)
        {
            "json_class" => self.class.name,
            "data"       => {"id" => @id, "owner" => @owner, "text" => @text}
        }.to_json(*a)
    end
    def self.json_create(object) 
        new(object["data"]["id"], object["data"]["owner"],object["data"]["text"])
    end
end
```

Kod oldukça basit. Quote sınıfına ait nesne örneklerinden oluşan bir dizi var. Quote sınıfı JSON serileştirme için gerekli fonkisyonellikleri de barındırıyor. sinatra çatısını kullanan uygulamanın HTTP Get ile erişilebilen bir metodu var. Buna göre localhost:8080 adresine gelip quotes/random yoluna gittiğimizde, üretilen rastgele sayıya göre diziden bir özlü sözün döndürülmesi söz konusu. Bildiğiniz üzere sinatra şarkılarını varsayılan olarak 4567 numaralı porttan söylemekte. Bu senaryoda 8080 portunu kullanmak için küçük bir atama söz konusu. Talep sonrası içerik tipinin application/json formatında olacağı da content_type ataması ile belirleniyor. Gelelim özellikle Google Cloud Platform tarafındaki sistem için önem arz eden diğer iki içeriğe.

app.yaml isimli bir dosya oluşturup içeriğini aşağıdaki gibi doldurmam gerekti (Google dokümantasyonu sağolsun)

```text
runtime: ruby
env: flex
entrypoint: bundle exec ruby app.rb

manual_scaling:
  instances: 1
resources:
  cpu: 1
  memory_gb: 0.5
  disk_size_gb: 10
```

Burada Google Cloud tarafındaki çalışma ortamı için gerekli bir takım ayarların bildirimi yapılıyor. Çalışma zamanının ruby motoruna göre tasarlanması gerektiği, uygulamanın başlatılacağı giriş noktası, ölçekleme seçenekleri ve donanımsal kaynak gereksinimleri (işlemci adedi, bellek, disk boyutu) gibi bilgilere yer veriliyor. Gemfile isimli uzantısız bir dosya daha eklemek lazım. Bu dosyada projenin ihtiyaç duyduğu gem'ler varsa isimleri bildiriliyor. source ataması ile de, ilgili gem içeriklerinin hangi kaynaktan çeklileceği söylenmekte. Paketler için depo bildirimi yapıldığını düşünebiliriz. Benim örneğimde sadece sinatra gerektiğinden içeriği aşağıdaki gibi yazmam yeterliydi.

```text
source "https://rubygems.org"

gem "sinatra"
```

Bu işlemlerin arındandan ilk olarak kodun kendi sistemimde çalıştığından emin olmalıydım. Terminalden

```bash
ruby app.rb
```

ifadesi ile programı çalıştırdım ve tarayıcıyı her güncelleyişimde rastgele bir söz ile karşılaştığımı gördüm. Dikkat edeceğiniz üzere localhost:8080/quotes/random adresine talepte bulunuluyor. Şu an için yeterli gibi görünse de amaç bunu Google Cloud üzerinden sunabilmekti.

![image.axd](images/image.axd)

gcloud ile Taşıma

Sırada taşıma öncesi yapmam gereken bir hazırlık daha var (mış). Ruby kodunu tamamladıktan sonra özellikle

```bash
bundler install
```

terminal komutu ile proje bağımlılıkların yükleneceği Gemfile.lock'un oluşması gerekiyor. Ancak bu işlem sonrası taşıma adımlarına geçilebilir. Aksi halde taşıma işlemi sırasında hata alınmakta (Ne olduğunu söylesem mi? Yok, söylemeyeceğim...bundler install yapmadan gcloud deploy komutunu çalıştırıp kendiniz görün)

İçerik proje için aşağıdaki gibi oluştu.

```text
GEM
  remote: https://rubygems.org/
  specs:
    mustermann (1.0.1)
    rack (2.0.3)
    rack-protection (2.0.0)
      rack
    sinatra (2.0.0)
      mustermann (~> 1.0)
      rack (~> 2.0)
      rack-protection (= 2.0.0)
      tilt (~> 2.0)
    tilt (2.0.8)

PLATFORMS
  ruby

DEPENDENCIES
  sinatra

BUNDLED WITH
   1.11.2
```

Artık taşıma işlemini deneyebilirdim.

```bash
gcloud deploy
```

terminal komutunu çalıştırarak süreci başlattım. Bir kaç dakika sonrasında taşıma işleminin başarılı bir şekilde yapıldığını gördüm (Belki de Avusturalya'yı seçmemeliydim. Uzak diye mi taşıma böyle uzun sürdü dersiniz?:P) Bundan sonra tek yapmam gereken https://yourfunnyquote.appspot.com/quotes/random adresine gitmek oldu. Tabii dilerseniz komut satırından

```bash
gcloud app browse
```

diyerekten de tarayıcının açılmasını ve ilgili projeye gidilmesini sağlayabilirsiniz. Benim elde ettiğim sonuç şöyleydi.

![image.axd](images/image.axd)

Adres çubuğundaki adrese dikkat edin lütfen. Bu arada yazıyı tamamladıktan sonra servisi kapattım:)) Yani çalıştığının tek ispatı bu ekran görüntüsü. Nitekim Google Cloud Platform bana 364 günlük bedava kullanma süresi ve 300 dolarlık kredi bahşetmişti ama neme lazım. Her an her şey olabilirdi. Bu nedenle ilgili adrese ulaşamazsanız lütfen beni affedin. Kendi projeniz ile denediğinizde ekstra bir durum oluşmazsa taşımanızı sorunsuz gerçekleştiriyor olmalısınız.

Kapatırken

Pek tabii Google Cloud Platform bu kadar basit değil. Computation, Big Data, Storage, StackDriver, Networking vb pek çok ana başlık altında yer alan hizmetler içeriyor. Söz gelimi API Endpoint dikkatimi çekelerinden birisi. Özellikle ölçekleme ve yüksek performans gerektiren APIlerimiz için biçilmiş kaftan gibi duruyor. Bunun en büyük sebeplerinden birisi Proxy Container tarafında NGinx sunucularını kullanması. Aşağıdaki çizimde söz konusu yapının kabataslak hali var (Çözünürlük için lütfen kusura bakmayın. Telefon kamerasından ancak bu kadar oluyor) İstemci talepleri öndeki Load Balancer'dan sonra Nginx tabanlı Proxy servisine geliyor ve buradan API'nin (söz gelimi Python veya bir Java uygulamasının) konuşlandırıldığı Container Instance'ına geçiyor. gcloud alt taraftan da görüleceği üzere dağıtım adımından sorumlu. Servis yönetimi (Service Management) gcloud ile konuşarak taşıma operayonlarını ve konfigurasyon içeriğini yönetiyor. Çalışma zamanı kontrolleri ve raporlamalarda servis kontrolün (Service Control) işi. Çalışma zamanını web tabanlı Cloud Console üzerinden de izleme şansımız var.

![image.axd](images/image.axd)

Bu mimarileri anlamaya çalışmamız önemli. Ama biraz dinlendikten sonra. Çünkü, West-World'de güneş çoktan battı. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
