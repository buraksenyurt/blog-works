---
layout: post
title: "Ruby Tarafından Redis(Docker Bazlı) Veritabanı ile Konuşmak"
date: 2019-12-01 09:45:00 +0300
categories:
  - ruby
tags:
  - redis
  - nosql
  - ruby-lang
  - docker
  - gem
---
Geçtiğimiz yıl kendi kendimi eğitmek üzere 41 bölümden oluşan [Saturday-Night-Works](https://github.com/buraksenyurt/saturday-night-works) isimli bir çalışma yapmıştım. Github üzerine aldığım notlar ve kod örneklerini blog üzerinde derleyip toparlamak onları pekiştirmek için önemli ve gerekliydi. Ne var ki yetişemediğimiz yazılım teknolojilerindeki gelişmeler için sürekli antrenman yapmam gerekiyor. Bu sebepten araya uzun bir tatil koyup tekrardan enerjimi depolamaya çalıştım. Nadas süresi boyunca neler yapabileceğimi düşündüm. Yeni maceramın felsefesi bir öncekisi ile aynı olmalıydı. Rastgele ve pek deneyimli olmadığım konularla ilgili araştırma yapıp bunları basit örneklerle çalışmalı ve dokümanlaştırmalıydım. Ha geldi gelecek derken nihayet ilham perim beni buldu ve artık tatilimi sonlandırmam gerektiğine karar verdim. Yeni serüvenime SkyNet evrenindeki Ahch-To (MacOS Mojave - Intel Core i5 1.4Ghz, 4 Gb 1600 Mhz DDR3) gezegeninde başlıyorum. Orada geçirdiğim bir saat dünya zamanında birkaç güne denk geliyor.

![terminatormotorcyle.jpg](/assets/images/2019/terminatormotorcyle.jpg)

Öyleyse gelin size ilk gün başımdan geçenleri anlatayım. Bu ilk macerada epeydir eğilmediğim Ruby kodları ile tekrar bir aradayım. Amacım Ruby ile docker container üzerinde host edilmiş bir Redis veritabanında basit işlemler yapabilmek. Daha önce birkaç NoSQL sistemini deneyimle fırsatım olmuştu. Ancak MacOS üzerinde docker kullanımı konusunda tecrübeli değildim. Ruby kodlamayı ve Redis'in temel veri tipleriniyse çoktan unuttum. Dolayısıyla benim için bilgi pekiştirmek açısından iyi bir gün olacağını düşünüyorum.

Redis, In-Memory NoSQL veritabanı sistemlerinden birisi olarak karşımıza çıkıyor. Tüm veriyi bellekte saklayıp sorguladığından epeyce hızlı (Metrik değerlere bakmak lazım) Key-Value (Tuple Store) tipinden bir veritabanı olup hash, list, set, sorted-set ve [string tiplerinden](https://redis.io/topics/data-types)oluşan zengin bir veri yapısına sahip. Dağıtılabilir caching stratejilerinde, otomatik tamamlama özelliklerine ait önerilerin hızla getirilmesinde, aktif kullanıcı oturumlarının takibinde, iş kuyruklarının modellenmesinde (publisher/subscriber türevli) etkili bir NoSQL çözümü olarak tercih ediliyor.

Bu temel bilgilere ek olarak CAP (Consistency, Availability, Partition Tolerance) üçgeninin CP kenarında yer aldığını söylemeliyiz. CAP teoremindeki harflerin anlamlarını kısaca hatırlayalım mı? Dağıtık bir sistemde Consistency ilkesine göre tüm istemciler verinin her zaman aynı görünümüne ulaşır. Pi'nin değeri bir istemci tarafından 3.14 olarak belirlenmişse diğer istemciler Pi'ye baktıklarında bu sayıyı görür. Availability ilkesi tüm istemcilerin dağıtık sistem üzerinde her an okuma ve yazma yapabiliyor olmasını öngörür. Partition ilkesine göre node'larda fiziki olarak kopma meydana gelse bile sistem kullanılabilir durumda kalmalıdır. Tabii en önemli nokta şudur ki CAP teoremine göre dağıtık bir sistem bu üç unsuru aynı anda karşılayamaz. Sürprizzzz:) Çok sık gördüğümüz CAP üçgenini aklımızda kaldığı kadarıyla çizmeye çalışırsak bilgileri biraz daha pekiştirmiş oluruz. Örneğin aşağıdaki gibi.

![11_screenshot_5.png](/assets/images/2019/11_screenshot_5.png)

Bu kısa bilgilerden sonra örneklerimizi geliştirmeye başlamaya ne dersiniz!?

## İlk Adımlar

Öncelikle Redis Docker imajını ayağa kaldırıp ping atmamız gerekiyor. Sistemde docker yüklüyse aşağıdaki komutlardan yararlanılabilir.

```bash
docker pull redis
docker run --name london --network host -d redis redis-server --appendonly yes
docker run -it --network host --rm redis redis-cli -h localhost
ping

docker stop london
docker container rm 0793a
```

İlk komut ile redis imajının son sürümünü indiriyoruz. Buradaki appendonly anahtarına verilen değer sebebiyle dataset üzerinde yapılan her değişiklik fiziki diskte kalıcı hale getirilecektir ([Persistance detayları için şuraya bakabiliriz](https://redis.io/topics/persistence)) İkinci komutla container'ı başlatıp ardından gelen ile redis-cli terminal aracını devreye alarak redis ortamına bağlanıyoruz. ping karşılığında PONG mesajını görmemiz redis'in çalıştığının işaretidir. Dilerseniz buradayken de Redis ortamını deneyimleyebilirsiniz. Son komutlar opsiyonel olmakla birlikte Docker container'ını durdurmak ve kaldırmak için kullanılır.

![11_screenshot_1.png](/assets/images/2019/11_screenshot_1.png)

Docker container'ı varsayılan olarak localhost:6379 adresinden hizmet vermektedir. Proje iskeletini oluşturmak için ben kendi sistemimde aşağıdaki terminal komutlarını kullandım. Diğer yandan örnek kodlarımızı ruby ile geliştireceğiz ancak Redis ile konuşabilmemizi kolaylaştıracak bir pakete de (gem diyoruz) ihtiyacımız olacak.

```bash
mkdir src
cd src
touch main.rb publisher.rb subscriber.rb dessert.rb

sudo gem install redis
```

![11_screenshot_2.png](/assets/images/2019/11_screenshot_2.png)

Son satırda yer alan install komutu ile redis isimli gem paketini sisteme dahil etmiş oluyoruz.

## Kod Tarafı

Öğretide üç farklı uygulama söz konusu. İlk örnek ruby kodlarından Redis'e bağlanıp çok basit bir kaç işlemin nasıl icra edildiğini göstermekte. Ağırlıklı olarak Redis veri tiplerinin genel kullanımları söz konusu. Kodlarda yer alan yorum satırlarında mümkün mertebe ne yaptığımızı açıklamaya çalıştım.

main.rb içeriği

```text
require 'redis' # redis gem'ini kullanacağımızı belirttik

redis=Redis.new(host:"localhost")
#redis.ping()

redis.set("aloha",1001) # örnek key:value ekledik
word=redis.get("aloha") # eklediğimizi alıp

puts word # ekrana bastırdık

# List kullanımına örnekler

redis.del('user_actions') # önce user_actions ve tutorial_list listelerini temizleyelim
redis.del('tutorial_list')

# Right Push
redis.rpush('user_actions','Naycıl login olmayı deniyor')
redis.rpush('user_actions','şifre hatalı girildi')
redis.rpush('user_actions','Naycıl giriş yaptı')
redis.rpush('user_actions','Naycıl alışveriş sepetine bakıyor')
redis.rpush('user_actions','Naycıl sepetten 19235123A kodlu ürünü çıkarttı')

p redis.lrange('user_actions',0,-1) # tüm listeyi ilk girişten itibaren getirir

redis.ltrim('user_actions',-1,-1) # son elemana kadar olan liste elemanlarını çıkarttık
puts ''
p redis.lrange('user_actions',0,-1)

puts ''

#Left Push
redis.lpush('tutorial_list','redis')
redis.lpush('tutorial_list','mongodb')
redis.lpush('tutorial_list','ruby on rails')
redis.lpush('tutorial_list','golang')

p redis.lrange('tutorial_list',0,-1)

# Set Kullanımına Örnekler

redis.del('cenifer-friends')
redis.del('melinda-friends')

redis.sadd('cenifer-friends','semuel')
redis.sadd('cenifer-friends','nora')
redis.sadd('cenifer-friends','mayki')
redis.sadd('cenifer-friends','lorel')
redis.sadd('cenifer-friends','bill')

redis.sadd('melinda-friends','mayki')
redis.sadd('melinda-friends','ozi')
redis.sadd('melinda-friends','bill')
redis.sadd('melinda-friends','törnır')
redis.sadd('melinda-friends','sementa')
redis.sadd('melinda-friends','kıris')

puts ''
p redis.smembers('cenifer-friends')
puts ''
p redis.smembers('melinda-friends')
puts ''
p redis.sinter('cenifer-friends','melinda-friends') # Yukarıdaki iki kümenin kesiştiği elemanları verir
puts ''
p redis.srandmember('melinda-friends') # her srandmember çağrısı kümeden rastgele bir elemanı döndürür
p redis.srandmember('melinda-friends')

# Sorted Set örnekleri

redis.del('best-players-of-the-week')
# haftanın oyuncularını ağırlık puanlarına göre ekledik
redis.zadd('best-players-of-the-week',32,'maykıl cordın')
redis.zadd('best-players-of-the-week',24,'skati pipın')
redis.zadd('best-players-of-the-week',32,'leri börd')
redis.zadd('best-players-of-the-week',21,'con staktın')

puts ''
puts redis.zrevrange('best-players-of-the-week',0,-1) # en yüksek skor puanından en küçüne doğru getirir (rev hecesine dikkat)
puts '' 
puts redis.zrevrange('best-players-of-the-week',0,0) # en iyi skora sahip olanı getirir (rev hecesine dikkat)
puts ''
puts redis.zrangebyscore('best-players-of-the-week',20,30) # skorları 20 ile 30 arasında olanlar
```

İkinci örnek biraz daha kapsamlı olup publish/subscribe modelinin uyarlamasını ele alıyor. Publisher görevini üstlenen uygulama sembolik olarak belirli aralıklarla broadcast yayını yapar ve game-info-101 ve game-info-102 kodlu kannalar üzerinden mesajlar yollar. Bu kanalları dinleyen aboneler yayınlanan mesajları görebilir.

publisher.rb içeriği

```text
require 'redis'

puts 'Maç bilgileri gönderiliyor...'
redis=Redis.new(host:"localhost") #Redis sunucusuna bağlan

redis.publish 'game-info-101','Harden üç sayılık basket. Skor 92-92' # bir mesaj fırlat
redis.publish 'game-info-102','Furkan top çalma, hızlı hücum ve basket. Skor 2-0'
sleep 16
redis.publish 'game-info-102','Joel Embit blog. Skor 2-0'
sleep 14 # 14 saniye bekle
redis.publish 'game-info-101','Donçiç harika bir assist yapıyor ve Maksi Kleber smacı vuruyor. Skor 92-94'
sleep 22 # 22 saniye bekle
redis.publish 'game-info-101','Dallas son bir molaya gidiyor'
sleep 60 # 1 dakika bekle
redis.publish 'game-info-101','exit' # aboneler, bu kanal için aboneliklerini sonlandırabilir
redis.publish 'game-info-102','exit'

puts 'Program sonu'
```

subscriber.rb içeriği

```text
require 'redis'

channelName=ARGV[0] # komut satırında kanal bilgisini al

redis=Redis.new(host:"localhost") #Redis sunucusuna bağlan

begin
    # game-info-101 isimli olaya abone ol
    redis.subscribe channelName do |on|

    on.subscribe do |channel,msg| # abonelik gerçekleşince çalışır
        puts "#{channel} kanalına abone olundu"
    end
    
    on.message do |channel, msg| # mesaj gelince çalışır
      puts "#{channel} -> #{msg}" # mesajı ekrana bastır
      redis.unsubscribe if msg=="exit" # eğer mesaj bilgisi exit olarak geldiyse aboneliği sonlandır
    end

    on.unsubscribe do |channel,msg| # abonelik sonlandığında çalışır
        puts "#{channel} kanalına abonelik sonlandırıldı"
    end
end
rescue redis::BaseConnectionError => err # bir bağlantı hatası sorunu olursa 3 saniye içinde tekra bağlanmaya çalışılır
    puts "#{err}, 3 saniye içinde tekrar deneyeceğim"
    sleep 3
    retry
end
```

Üçüncü ve son örnek ise bir SQL tablosunun Redis dünyasında kabaca nasıl tarif edilebileceği ile alakalıdır. Olayı karakterize etmek için aşağıdaki görseli ele alabiliriz.

![11_screenshot_6.png](/assets/images/2019/11_screenshot_6.png)

Player isimli bir tablomuz olduğunu düşünelim. Bu tip bir veri yığınını Redis'in deneysel dokümanlarına göre sağ taraftaki gibi tariflemek mümkün. Hash ve Sorted Set'ler tablo ve Select sorgusunu karşılayabilir niteliktedir. Örneğin oyuncuların skorlarına göre sıralanacağı bir listeyi Sorted Set nesnesi gibi düşünebilir ve belli bir puan aralığındakilerin listesinin sıralı olarak çekilmesini sağlayabiliriz. Bu deneyselliği ele aldığımız Dessert.rb dosyasının içeriği aşağıdaki gibidir.

```text
require "redis"

redis=Redis.new(host:"localhost")

# Hash içerisine birkaç player örneği ekliyoruz
redis.hmset("player:1","fullname","Baz Layt Yiır","country","moon","score",339)
redis.hmset("player:2","fullname","Mega maynd","country","mars","score",317)
redis.hmset("player:5","fullname","Payn payn laki lu","country","wild west","score",405)
redis.hmset("player:3","fullname","Aeyrın Vaykovski","country","poland","score",322)
redis.hmset("player:4","fullname","Bileyd Rut","country","saturn","score",185)

# hgetall metoduna verdiğimiz parametre ile player:3 veri setini çekip ekrana bastırıyoruz
# Select'in where koşulu gibi düşünelim
player3=redis.hgetall("player:3")
puts "#{player3}\n\n"

# hmget ile player:2 key içeriğinden sadece country ve fullname değerlerini yazdırıyoruz
puts "#{redis.hmget('player:2','country','fullname')}\n\n"

# bir sorted list oluşturuyoruz
# listeyi yukarudaki hash üzerinden oluşturmak için player:* desenine uygun olan key içeriklerini çekmemiz lazım
# scan_each metodunun match parametresi bunu sağlıyor
redis.scan_each(:match=>"player:*"){|key|
    currentScore=redis.hmget(key,"score") #hmget metoduna iterasyondaki key değerini verip score bilgisini yakalıyoruz
    redis.zadd("score_list",currentScore,key) # score bilgisini o anki key ile ilişkilendirerek sorted list nesnesine ekliyoruz
}

# sorted list için skoru 330 puanın altına olanların listesini çektik (Select'in where koşulu gibi düşünelim)
scores_under_330=redis.zrangebyscore("score_list",0,330)

puts "Skoru 0 ile 330 arasında olanlar\n\n"
# bulduğumuz listede dolaşıp sadece fullname bilgilerini ekrana bastırdık
scores_under_330.each do |plyr|
    info=redis.hgetall(plyr)
    puts "#{info['fullname']} için güncel skor değeri #{info['score']}"
end
```

## Çalışma Zamanı

Kod tarafını tamamladıysak çalışma zamanına geçebiliriz. Main.rb ile birlikte diğer örnekleri de incelemeye başlayalım.

### Başlangıç

Öncelikle Redis container'ını başlatmalıyız. Bunun için aşağıdaki terminal komutları ile ilerleyebiliriz. İlk komut docker container'ını çalıştırıyor. İkinci komut kontrol amaçlı olarak container listesine bakmak için. Son terminal komutumuz tahmin edileceği üzere main.rb isimli ruby dosyasını yürütüyor.

```bash
docker run -d -p 6379:6379 redis
docker container ps -a
ruby main.rb
```

![11_screenshot_3.png](/assets/images/2019/11_screenshot_3.png)

### Ana Yemek

Yemek olarak publisher soslu subscriber'ımız var. Üstelik en ünlü İtalyan şeflerinden Rizotta Galliani tarafından özenle hazırlandı:P Sizi ciddiyete davet ediyorum Burak Bey. En az 3 terminal ekranı açıp birisinde publisher.rb diğer ikisinde de subscriber.rb dosyalarını dinleyecekleri kanalları parametre olarak verip çalıştırmak sonuçları irdelememiz için yeterlidir. Aynen aşağıdaki gibi.

```text
ruby subscriber.rb 'game-info-1'
ruby subscriber.rb 'game-info-2'
ruby publisher.rb
```

![11_screenshot_4.png](/assets/images/2019/11_screenshot_4.png)

Dikkat edileceği üzere aboneler program başlatılırken bağlanacakları kanalın adını belirtiyor. Bu nedenle her ikisi de farklı maç akışlarını dinlemekteler. Pek tabii bir abonenin birden fazla kanalı dinlemesi de mümkün. Kurgumuz burada oldukça basit ve senkron kaldı. N sayıda maça ait haber akışını paylaşabileceğiniz bir publisher uygulamasını nasıl yazarsınız bir düşünün.

### Tatlı

Tatlı olarak bir SQL tablosunun Redisce yorumlanması var. Aşağıdaki terminal komutu ile örneğimizi çalıştıralım ve sonuçları sınıfça irdeleyelim.

```bash
ruby dessert.rb
```

![11_screenshot_7.png](/assets/images/2019/11_screenshot_7.png)

Pek tabii SQL'in Domain'e özgü yazılmış lisanında SELECT, WHERE gibi ifadeleri kullanarak istenilen sorguları çalıştırmak çok daha kolay. Lakin yine de Redis dünyasında bu tip bir yaklaşımı nasıl ele alabiliriz sonuçlardan görebiliyoruz. Bunun farklı varyasyonlarını LINQ (LanguageINtegratedQuery) gibi yapılarda da görmemiz mümkün. Elimizde bir liste ve içerisinde veri tutan nesnelerimiz varken filtrelemeler için koddakine benzer yaklaşımları kullanıyoruz.

### Tamamlarken

Eğer kullandığımız Redis Container'ı ile işimiz bittiyse önce durdurup sonrasında sistemden kaldırmak isteyebiliriz. Bunun için aşağıdaki terminal komutlarını çalıştırmak yeterli olacaktır (Muhtemelen sizdeki Names değeri farklı olur. Ben çalışırken rastgele interestingnobel ismi atanmıştı)

```bash
docker container ps -a
docker stop interesting_nobel
docker container rm interesting_nobel
docker container ps -a
```

![screenshot_8.png](/assets/images/2019/screenshot_8.png)

## Neler Öğrendim?

[Saturday-Night-Works](https://github.com/buraksenyurt/saturday-night-works) çalışmalarında olduğu gibi yeni başladığım SkyNet serüveni de bana birçok şey öğreteceğe/hatırlatacağa benziyor. Bu ilk macerada aklımda kalanları şöyle özetleyebilirim.

- Redis docker imajını MacOS üzerinde kullanmayı
- Redis'in CAP teoreminde hangi ikiliye yakın olduğunu
- Redis gem'ini kullanarak yapılabilecek temel işlemleri
- Temel redis veri tiplerini
- Basit bir pub/sub kurgusunu işletmeyi
- SQL stilinde bir tablonun Redisce oluşturulmasını ve verinin filtrelenmesini
- Bir hash içindeki tüm key değerlerini nasıl dolaşabileceğimi

## Eksikliği Hissedilen Konular

SkyNet'in ilk gününden kalan ve merak ettiğim şeyler de var. Bunlar siz değerli okurlarımın araştırabileceği konular arasında yer alıyor. Bölüm sonu soruları gibi düşünebilirsiniz:)

- Publisher tarafında farklı kanalların birbirinden bağımsız ve asenkron olarak yayın yapmasını nasıl sağlayabiliriz?
- Subscriber olarak birden fazla kanala abone olabilir miyim?
- Olmayan bir kanala abone olmak istediğimde Ruby çalışma zamanı nasıl tepki verir?
- Peki pub/sub senaryosunu Ruby on Rails tabanlı bir web projesinde nasıl kullanırım?

Böylece geldik bir SkyNet gününün sonuna. Zaman farkından dolayı tekrar Dünya'ya dönmek zorundayım. Bir sonraki gidişimde bakalım bizleri ne tür maceralar karşılayacak. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
