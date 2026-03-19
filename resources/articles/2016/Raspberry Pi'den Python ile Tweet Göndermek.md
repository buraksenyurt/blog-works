---
title: "Raspberry Pi'den Python ile Tweet Göndermek"
pubDate: 2016-06-14 06:00:00
categories:
  - python
  - Raspberry Pi
tags:
  - python
  - Raspberry Pi
  - REST
  - HTTP
  - Java
  - Ruby
---

# Raspberry Pi'den Python ile Tweet Göndermek
![raspi_0.gif](images/raspi_0.gif)

Merhaba Arkadaşlar,

Geçtiğimiz sene yaz ayı boyunca Ruby programlama dilini öğrenmeye çalışmıştım. Ruby dilinde orta seviyelere kadar bilgimi ilerletebildim ancak bu yaz dönemini paslanmamak adına farklı bir uğraş ile geçirmeye karar verdim. Ruby'yi öğrenmeye başlama amaçlarımın başında çocuklara programlama dilini öğretmek geliyordu. Benzer şekilde Raspberry Pi'nin de bu amaçla kullanılabildiğini öğrendim. Özellikle Scratch dilini kullanarak çocukların robotlara bir şeyler yaptırabilmesi ve bu şekilde programlamayı öğrenmeleri harika. Pek tabi Raspberry Pi'yi özel yapan bir diğer neden de IoT (Internet of Things).

Bunun üzerine aldım Bruce'u yanıma her zamanki gibi büyük bir hevesle koyuldum yola. Önce bir Raspberry Pi 3 Model B aldım (Sonrasında 7inch dokunmatik ekran, sıcaklık sensörü, HDMI bağlantı kablosu, 40 GPIO kablosu, bir adet servo motor kontrolcüsü gibi parçaları da dahil ederek kredi kartlarımdan birini epeyce şişirdim) Ancak sonradan öğrendim ki [Robotistan](http://www.robotistan.com) gibi sitelerde Raspberry Pi ile ilgili komple setler satılıyor. Yeni başlayacaklara tavsiyem bir set alarak ilerlemeleri.

## Raspi Hakkında Kısa Bilgi

Raspberry Pi yaklaşık olarak kredi kartından biraz daha büyük olan bir kart. Maliyeti oldukça düşük olan Raspberry Pi aslında microprocessor içeren (ki bu noktada MicroController içeren Arduino'dan ayrılıyor) bir bilgisayar olarak düşünülebilir. Dolayısıyla onunla her şeyi yapabilirsiniz. Bir Web Server haline de getirebilir (ki denedim ve apache2 üzerinde php kodlaması yaptım) sıcaklık sensörüne sahip bir kahve fincanını kumanda da edebilirsiniz. Yapabilecekleriniz hayal gücünüz ile sınırlı diyebilirim.

Çevre ünitelerle (monitör, kamera, GPIO-General Purpose Input Output ile bağlanabilen her türlü elektronik alet, klavye, mouse vb) olan bağlantıları sağlayacak arabirimler üzerinde yer alan kart uygun işletim sistemi yüklendiğinde mini bir bilgisayar haline gelmekte. Bu anlamda Windows 10 IoT, Ubuntu Mate ve kendi orjinal işletim sistemi Raspbian'ı kullanabiliriz.

> Kart ile ilişkili çok fazla detaya girmeyeceğim nitekim yeni yeni öğreniyorum ama bu konuda epey bilgili olan [Recep Duman hocam ile yaptığımız youtube söyleşisini](https://youtu.be/LfTrBhXo8vk) mutlaka izlemenizi öneririm.

Kullandığım Raspberry Pi üzerinde Ruby standart olarak yüklü. Buna ek olarak python diline ait ortam da hazır. Java,Mathematica, Scratch de diğer diller arasında sayılabilirler. Her biri için güzel birer IDE'de gelmekte.

Aslında amacım bir motoru Raspberry Pi ile programlayıp evdeki Lego Technic'ler den birisini önceden tayin ettiğim rota doğrultusunda, engellere çarpmayacak şekilde dolaştırabilmek. Ama tabii bu mertebelere gelmeden önce bir programlama diline de hakim olmak gerekiyor. Tercihimi python'dan yana kullanıyorum. Böylece yeni bir programlama dilini araştırıp deneme şansım da olacak. Internetteki kaynaklardan ve aldığım bir kaç kitaptan kısa bir giriş turu ile değişkenleri, döngüleri, range, tuple, dictionary gibi nesne koleksiyonlarını, modülleri, metod tanımlamalarını, kodlardaki girintilerin önemi vb kavramları hızlıca denedikten sonra kendimi bir anda Raspberry Pi organizasyonuna ait sitedeki tutorial'ları çalışırken buldum.

İlk gözüme kestirdiğim ise Twitter API sini kullanarak Tweet atmak oldu. Daha önceden C# ve Ruby dillerini kullanarak denediğim bir çalışmaydı. Ancak şimdi ki deneyim daha eğlenceliydi diyebilirim. Bloğuma da dilim döndüğünce vakayı yazayım hangi yollardan geçtiğimi sonradan bakınca hatırlayayım istedim. Şimdi maceramıza başlayabiliriz.

## Genel Prensipler Aynı

Twitte API sini kullanmanın belli başlı prensipleri var. Bunlar aslında platform bağımsız kabul edebileceğimiz prensipler. İlk önce hangi platformda olursak olalım Twitter gibi sosyal ağları kolay bir şekilde kullanabilmek için servis bazlı çalışan kullanımı kolay API'lere ihtiyacımız var. Genelde REST tabanlı tasarlanan bu servisleri python tarafında Twython isimli modülü kullanarak ele almak son derece kolay. Bir diğer önemli kuralda bu API'yi kullanabilecek yetkilere sahip olmamızın gerektiği. Twitter tarafından baktığımızda bu kullanım için consumerkey, consumersecret, accesstoken ve accesstokensecret değerlerine ihtiyacımız bulunmakta. [Şu adresten kendinize bir application oluşturup gerekli bilgileri alabilirsiniz](https://apps.twitter.com/).

> Şunu belirtelim ki bir kütüphane kullanmak zorunda değilsiniz. Pekala Twitter REST servisleri için gerekli GET,POST paketlerini kendiniz de kod içinde hazırlayıp gönderebilirsiniz.

## İlk adım twython modülünü yüklemek

.Net dünyasında nuget paketleri, ruby tarafında gem'ler...Python tarafında ise module olarak adlandırılan kütüphaneler söz konusu. Tweet atmamızı kolaylaştıracak olan twython (ki adı twitter ve python un birleşimi ile elde edilmiş. Çok hoş bir isim değil mi?) kütüphanesini sistemimize yüklemek için terminalden aşağıdaki komutları sırasıyla çalıştırmamız gerekiyor.

Burada ki sudo komutunu pi isimli varsayılan kullanıcının ilgili operasyonlar sırasında Permission Denied almaması için kullanılmakta. apt-get ile tahmin edeceğiniz üzere sistem güncellemesi ve yeni program yüklemeleri gibi işlemleri gerçekleştirmekteyiz. python tarafına ilgili kütüphaneyi yükleyecek asıl komut ise pip install. Son satır ile twython kütüphanesini sisteme yüklemiş bulunuyoruz.

sudo apt-get update
sudo apt-get upgrade
sudo apt-get install python-setuptools
sudo easy_install pip
sudo pip install twython

## Azcık Kod

İlgili kütüphane yüklendiğine göre artık gerekli kodlarımızı yazmaya başlayabiliriz. Uygulama iki kod dosyasından oluşmakta. py uzantılı olarak kaydedilecek dosyalardan ilkinde consumer ve access bilgilerine ait değişkenleri tutacağız. Bunu bir nevi konfigurasyon içeriği gibi de düşünebiliriz (Bu arada kodlar için Python 2 IDLE yi kullandığımı belirtmek isterim. Nitekim biraz sonra değineceğimiz twython kütüphanesini python 3.4.2de kullanamadım. Ama işin peşindeyim)

auth.py içeriği

```text
consumer_key="..."
consumer_secret="..."
access_token="..."
access_token_secret="..."
```

Tabii ki siz... yazan kısımlara Twitter'ın sizin için ürettiği değerleri girmelisiniz.

İkinci dosyamızda (twitter.py olarak kaydedebiliriz) ise aşağıdaki satırlara yer vereceğiz.

```text
from twython import Twython
from auth import(
consumer_key,
consumer_secret,
access_token,
access_token_secret
)

def send_tweet(message):
	twitter=Twython(
	consumer_key,
	consumer_secret,
	access_token,
	access_token_secret
	)
	twitter.update_status(status=my_message)
	print("\'%s\' seklinde mesaj gonderildi" % message)

my_message="Bu mesaj Raspi'den python kodu ile gonderilmistir"
send_tweet(my_message)
```

İlk iki satırda koda enjekte ettiğimiz tip ve değişkenler olduğunu düşünebiliriz. Twython nesnesini oluşturabilmek için ilgili tipi az önce yüklediğimiz modül içinden import etmekteyiz. Buna ek olarak aynı klasörde yer alan auth.py dosyasından da consumerkey,consumersecret,accesstoken ve accesstokensecret isimli string değişkenleri koda alıyoruz.

sendtweet bir metod tahmin edeceğiniz üzere. Ruby'deki gibi def anahtar kelimesi ile tanımlanan fonksiyon: işareti ile sonlanıyor (aslında for, if gibi kod bloğuna sahip olabilecek ifadeler hep: kullanıyor) Sonrasında önemli olan konu ise metod içerisindeki kod satırlarının girintili yazılması gerekliliği. Eğer böyle yapmazsak kodu kontrol ettirdiğimizde (Alt+X) Invalid Syntax hatası alırız. Bu arada yorumlayıcı metodun bittiğini nasıl anlıyor derseniz;orada boş bir satır var ya...

Koda import ettiğimiz değişkenler twitter isimli nesnenin oluşturulması sırasında (şimdilik benim yapıcı metod-constructor olarak düşündüğüm yerde) kullanılıyor. Metoda parametre olarak gelen message isimli değişken içeriği de updatestatus isimli fonksiyon kullanılarak twitter'a gönderiliyor. Son olarak print fonksiyonunu ile komut satırına bir bilgi basıyoruz. Sonuçlar mı?

![twython_1.gif](images/twython_1.gif)

Burada % kullanımını bir türlü beceremediğimi fark etmişsinizdir:D O yüzden arka arkaya bir kaç tweet gitmiş bulundu. Ama sonuçlar ilk gün için oldukça tatmin ediciydi. Kredi kartı büyüklüğündeki o bilgisayardan, python kodları ile harici bir kütüphane kullanarak tweet atmayı başarabildim. Demek ki bir robotu Raspi ile kullanmaya başladığımda, robotun tweet atmasını artık sağlayabilirim.

![twython_2.gif](images/twython_2.gif)

Pek tabii modül içerisinde bir çok fonksiyonellik bulunmakta. Örneğin timeline'ı, kendi tweet'lerimizi görebilir, resim gönderebilir ve daha pek çok işlemi gerçekleştirebiliriz. Sonuçta Twitter API'sini kullanabileceğiniz kütüphane elinizin altında. Bunları nasıl yapabileceğinizi görmek için [şu adresteki tutorial](https://www.raspberrypi.org/learning/getting-started-with-the-twitter-api/worksheet/)'ı aynen benim gibi adım adım yapmanızı öneririm. Ben biraz farklı olarak metod kullandım.

Böylece geldik bir makalemizin daha sonuna. Umarım eğlenceli bir yazı olmuştur. Raspi maceralarımıza devam edeceğiz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
