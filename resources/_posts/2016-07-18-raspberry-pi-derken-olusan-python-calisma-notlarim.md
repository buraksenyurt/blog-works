---
layout: post
title: "Raspberry PI Derken Oluşan Python Çalışma Notlarım"
date: 2016-07-18 21:30:00 +0300
description: "Bloğumu takip edenler bu yaz için Raspberry PI ve Python programlama diline merak saldığımı biliyordur. Bu merakımı boşa çıkarmamak için de vakit buldukça her iki konuya da çalışıyorum. İş yoğunluğu ve araya giren bayram tatili beni az da olsa geriye atmış durumda. Ama neyseki defterime aldığım renkli renkli notlarım var. Bu yüzden notlarımın üstünden geçmem öğrendiklerimi hatırlamamda epey yardımcı oldu. Notları renklendirmiş olmam da onları okurken sıkılmamamı ve hatta yer yer kendi kendime tebessüm etmemi sağladı. Yazdıklarımın üzerinden geçerken bunları bloğuma da koyayım ve benim gibi temel seviyede bu işe başlayan, elektronik'ten, Linux' den ve Python' dan bihaber olanlarla bir şeyler paylaşayım istedim."
categories:
  - python
  - raspberry-pi
tags:
  - python
  - raspberry-pi
  - bash
  - dotnet
  - rest
  - json
  - web-api
  - http
  - java
  - ruby
  - javascript
  - serialization
---
# Raspberry PI Derken Oluşan Python Çalışma Notlarım
![RaspiCover.gif](/assets/images/2016/RaspiCover.gif)

Merhaba Arkadaşlar,

Bloğumu takip edenler bu yaz için Raspberry PI ve Python programlama diline merak saldığımı biliyordur. Bu merakımı boşa çıkarmamak için de vakit buldukça her iki konuya da çalışıyorum. İş yoğunluğu ve araya giren bayram tatili beni az da olsa geriye atmış durumda. Ama neyseki defterime aldığım renkli renkli notlarım var.

Bu yüzden notlarımın üstünden geçmem öğrendiklerimi hatırlamamda epey yardımcı oldu. Notları renklendirmiş olmam da onları okurken sıkılmamamı ve hatta yer yer kendi kendime tebessüm etmemi sağladı. Yazdıklarımın üzerinden geçerken bunları bloğuma da koyayım ve benim gibi temel seviyede bu işe başlayan, elektronik'ten, Linux'den ve Python'dan bihaber olanlarla bir şeyler paylaşayım istedim.

Raspberry PI ve Python maceramda sevgili [Recep Duman](http://www.recepduman.net/) hocamın da yardımları var. Onun yol göstericiliğinde ilerlemeye çalışıyorum. Örneğin birlikte Karaköy'deki elektronikçilere gidip gerekli malzemeleri aldık. Ayrıca takip ettiğim iki kitap var.

[![book1a.gif](/assets/images/2016/book1a.gif)](http://www.idefix.com/Kitap/Raspberry-Pi/Guray-Yildirim/Egitim-Basvuru/Bilgisayar/urunno=0000000681261) [![book2a.gif](/assets/images/2016/book2a.gif)](http://www.idefix.com/Kitap/Yeni-Baslayanlar-Icin-Python/Ahmet-Aksoy/Egitim-Basvuru/Ders-Kitaplari/Teknik-Ders-Kitaplari/urunno=0000000694609)

Bu kitapları da şiddetle tavsiye ederim. Orta seviyeye kadar Raspberry PI ve Python ile donanmamızı sağlayacak değerli kaynaklar. Öyleyse vakit kaybetmeden notlarımızın üzerinden bir geçelim.

## Temel Terminal Komutları [8 Haziran 2016]

İlk olarak kullanmakta olduğum Raspberry PI kartı üzerinde Raspbian işletim sisteminin yüklü olduğunu belirteyim. Kurulum sonrası varsayılan kullanıcı adının pi ve şifrenin de raspberry olduğunu öğrendim. İlk yaptığım işlerden birisi kullanıcı şifresini değiştirmek oldu. Bugünü aşağıda kısa açıklamaları yer alan temel terminal komutlarını inceleyerek geçirmişim.

- ls - Dizin ve dosyaları listelemek içindir
- cd.. - Bir üst klasöre çıkarız
- mkdir - Klasör oluşturmak için kullanılır
- pwd - Print Working Directory'nin kısaltmasıdır. Bulunduğumuz konum bilgisini verir.
- clear - Kalabalıklaştıktan sonra terminal ekranının temizlemek için kullanabiliriz. Hayatımızda yeni bir sayfa açar gibi.
- whoami - O an sistemde çalışmakta olan kullanıcının adını öğrenmemizi sağlar.
- rmdir - Klasör veya dosya silmek içindir.
- touch - İçeriği boş bir dosya oluşturmamızı sağlar.
- rm - Sadece dosya silmek için kullanılır.
- nano - Terminal üzerinden kullanılan bir metin editörüdür. Beni MS-DOS zamanlarına götürmüştür. Merak edip örnek kullanımını da not almışım. -sudo nano HelloWorld.rb ile Ruby kod dosyası açmışım.
- cat, more, less - Dosya içeriklerini görmek için kullanılan komutlardır.
- df - Sistemdeki diskleri ve boş alan boyutlarını gösterir. Alan boyutlarının daha rahat okunabilmesi için genellikle -h parametresi ile kullanılır.
- file - Dosya veya dizinlerin türleri hakkında bilgi edinmemizi sağlar.
- history - Terminalde son kullandığımız komutların neler olduğuna bakmak istersek kullanabiliriz.
- --help - Bir komutun kullanımı hakkında bilgi verir.
- man [Komut Adı] - Bu ise ilgili komut hakkındaki yardım dokümanına ulaşmamızı sağlar.
- apt-get - Bu komutu sistemi güncellemek ve uygulama yüklemek için kullanabiliriz. Örnek olarak VLC isimli bir media player yüklemişim ama bunu yapmadan önce sistemi de güncellemişim (sudo komutunu var olan kullanıcı bir takım operasyonları yerine getirirken Permision Denied hatası almasın diye kullanmaktayız)
![firstDay.gif](/assets/images/2016/firstDay.gif)

Bu komutları terminal ekranında biraz denemişim. Hatta Raspberry PI'ye uzaktan bağlanmak için de bir şeyler yapmışım. Siz de bunu yapmak isterseniz öncelikle Raspberry PI'yi Internete bağlayıp yerel ağ için verilen IP adresini öğrenmeniz gerekir. Bunun için terminalden

```bash
hostname -I
```

komutunu çalıştırmanız yeterli (Bana 192.168.1.107 gibi bir adres dönmüş o gün) Sonrasında uzak bir bilgisayardan Secure Shell bağlantısı kurabiliriz. Bu bağlantı için yardımcı programlar kullanabileceğimiz gibi uzak bilgisayar terminalinden ssh komutu vererek de bağlanmayı deneyebiliriz.

```bash
ssh pi@192.168.1.107 
```

gibi. Burada pi raspberry pi bilgisayarına hangi kullanıcı ile bağlanmak istediğimizi berlitmektedir.

Rasbian'ın konfigurasyon ayarlarını değiştirmek için terminal ekranından sudo raspi-config komutunu kullanabiliriz.

> Raspberry PI üzerinde Ruby ile geliştirme yapan [Ray Hightower](http://rayhightower.com/), 2012 yılında Raspberry PI'sini 37 Ekran bir TV'ye bağlamış. Hatta halen bu analog TV'yi kullandığını ifade etmekte. Bu bana Commodore 64 oyun bilgisayarımı ve onu bağladığım 37 ekran televizyonumu hatırlattı. İlk gün aldığım ilginç notlardan birisi.

## Temel Python ile Geçen Bir Perşembe [9 Haziran 2016]

Bir sonraki gün temel Python çalışmaları ile geçmiş (Sanırım elektronikden dil kısmına doğru kaymaya karar vermişim) İşe terminalden python komutunu vererek başlayabiliriz. Bu durumda python kodlarını yazabileceğimiz ortama girmiş oluruz. Ortamdan çıkmak için yine komut satırından exit () ifadesini girebilir veya Ctrl+D kısa yol kombinasyonunu kullanabiliriz. Gelelim ilk gün yaptığım basit örneklere (Bunları makaleye geçirirken yine [Repl.it](https://repl.it/languages/python3) üzerinden denedim ve doğru çalıştıklarından emin oldum)

Tabii giriş kısmında bir Merhaba demek gerekiyordu.

```text
print("Hello Python and Raspbbery PI")

myAge=39
myName="Burak"

print("My name is {} and I'am {} years old.\nAnd Pi*(R^2)={}".format(myName,myAge,3.14*10*10))
```

İlk olarak print komutu ile ekrana bir metin içeriği yazdırıyoruz. Devam eden satırlarda ise myAge ve myName isimli iki değişken tanımı söz konusu. Son print ifadesinde süslü parantezler içerisinde bu değişkenleri yazdırıyor, \n ile bir alt satıra geçiş işlemini gerçekleştiriyoruz (\ ile escape karakter kullanımının söz konusu olduğunu ifade edebiliriz) Süslü parantezler içerisine gelecek ifadeler için format metodundan faydalanmaktayız. İlk süslü paranteze myName, ikinci süslü paranteze myAge son süslü paranteze ise matematiksel bir işlemin sonucu gelmekte.

![notes_1.gif](/assets/images/2016/notes_1.gif)

Bu girişten sonra Python dilinin temel veri türlerinden olan listelere göz atmışım.

```text
players=["burki","tubi","sharpi","reco","tusubasa"]

print(players)
print(players[2])

players.append("anduin")
print(players)

players.remove("burki")
print(players)

motto="Today is a good day to die"
print("good" in motto)

points=[30,21.50,90,15,18.5,45,3,3.14,0,-4,-2,(90*-3)]
print(points)

points.sort()
print(points)

points.sort(reverse=True)
print(points)

someObjects=[1,-1,"burk",True,90.345,900000055555555555000000000000000015]
print(someObjects)
```

![notes_3.gif](/assets/images/2016/notes_3.gif)

Belli tipte veya farklı türlerede elemanları tutmak için listelerden yararlanabiliriz. Listenin elemanlarına ulaşmak için [] operatörü kullanılır. Pop metodu ile son eklenen eleman listeden çekilir ve ayrıca silinir (ki burada denemeyi unutmuşum ama siz deneyebilirsiniz) Listeden eleman çıkartmak içinse remove fonksiyonu kullanılır. Faydalı fonksiyonlardan birisi de sort. Karışık bir listenin ascending veya descending sıralanmasını sağlar. Descending sırlama için reverse parametresine True değeri verilmesi yeterlidir. O gün hoşuma giden kullanımlardan birisi de, bir öğenin listede olup olmadığının bulunması olmuş. "good" in motto ifadesine göre good kelimesi motto değişkeni ile işaret edilen metinde geçiyorsa True geçmiyorsa False sonucu elde edilir.

> Defterde reverse=true şeklinde yazdıktan sonra Python'un case-sensitive bir dil olduğunu not almışım.
> ![notes_2.gif](/assets/images/2016/notes_2.gif)

Notlarım döngüler ve range kullanımı ile devam etmiş. Epey verimli bir haziran günü geçirmişim.

```text
players=["burki","tubi","sharpi","reco","tusubasa"]

for player in players:
	print(player,"is online now")
	
numbers1=range(11) #0dan 10a kadar bir sayı aralığıdır
total=0
for number in numbers1:
	total+=number
	
print(total)

numbers2=range(-25,0,5)
for number in numbers2:
	print(number)
```

![notes_4.gif](/assets/images/2016/notes_4.gif)

range veri türü ile belirli aralıklardaki sayı dizilerini kolayca tanımlayabiliriz. Hatta bu tanımlamalarda adım değeri de verebiliriz. Örneğin -25'den -5e kadar olan sayıları 5er adım aralıkla elde edebiliriz. for döngüleri ile range ve list veri türleri üzerinde dolaşarak belli işlemler gerçekleştirmemiz de mümkün. Belli bir değer aralığındaki sayıların toplamını bulmak, oyuncu listesini ekrana yazdırmak örnek kod parçasında yer verilen işlemlerdir.

> Python girinti (indent) kuralları barındıran bir dildir. Bir döngünün (bir metodun, if ifadesinin vb) alt satırlarına inildiğinde girinti verilmesi zorunludur diye de not almışım.
> ![notes_5.gif](/assets/images/2016/notes_5.gif)

Bu arada o günkü kod örneklerini dosyalarda bulundurmaya başlamışım. Python kod dosyaları için py uzantısı kullanılmakta. Eğer bir python dosyasını terminalden çalıştırmak istersek aynen Ruby dilinden olduğu gibi bir komut kullanmamız yeterli. Örneğin

python HelloWorld.py

gibi.

> Bugüne ait önemli notlardan birisi de sistemde yüklü iki farklı Python sürümü olması. Python 2.7.9 ve Python 3.4.2. Bunlar arasında belirgin farklılıklar var. Bu farklılıkları araştırırken [şu blog adresinden](http://sebastianraschka.com/Articles/2014_python_2_3_key_diff.html) yararlanmışım.

Kod çalışmasına metod tanımlamaları ile devam etmişim. E onlar olmadan olmaz tabii.

```text
players=["burki","tubi","sharpi","reco","tusubasa"]

def writeAllPlayers():
	for player in players:
		print(player,"is online now")
		

def rangeSum(x,y):
	numbers=range(x,y)
	total=0
	for n in numbers:
		total+=n
	return total

writeAllPlayers()
result=rangeSum(0,11)
print(result)
```

![notes_6.gif](/assets/images/2016/notes_6.gif)

Örnek kod parçasında iki metod yer alıyor. writeAllPlayers isimli fonksiyonumuz players listesindeki elemanları ekrana yazdırmakta. rangeSum metodu ise x,y parametrelerini kullanarak bir range oluşturmakta ve bu değer aralığındaki sayıların toplamını hesap ederek geri döndürmekte. Bu basit metod kullanımlarını inceledikten sonra o haziran günü yaptığım çalışmaları tamamlamışım.

## Modules ve pip [10 Haziran 2016]

O Cuma günü module kavramını incelmeye çalışmışım. Aslında aldığım notlara göre sadece random modülünü kod içerisinde kullanmışım. Module'leri kod parçaları içeren ayrık dosyalar olarak düşünebiliriz. Module'ler genellikle aynı domain'e özgü fonksiyonellikleri barındırır. Söz gelimi rastgele sayı üretimi ile ilişkili tüm operasyonlar random modülünde toplanmıştır. Modüller içerisinde çeşitli veri türleri ve fonksiyonellikler bulunur. Bir modül içerisinde hangi üyelerin olduğunu görmek için dir metodundan yararlanabiliriz.

```text
import random
print("random içeriği",dir(random),"\n")
print("shuffle içeriği",dir(random.shuffle),"\n")
```

![notes_7.gif](/assets/images/2016/notes_7.gif)

Örnek kod parçasında random modülünün ve bu modüldeki shuffle operasyonunun üyeleri listlenmektedir. Eğer istersek bu üyelerin yardım dokümanlarına da ulaşabiliriz. Bunun için help (random) gibi bir ifade kullanmamız yeterlidir.

> Kodda dikkat edilmesi gereken noktalardan birisi de import kullanımıdır. import ile kodun ilerleyen kısımlarında random modülündeki operasyonlar kullanılabilir hale gelir. Eğer bunu yapmazsak
> Traceback (most recent call last):
> File "python", line 2, in
> NameError: name 'random'is not defined
> şeklinde bir hata alırız.

İşte random modülüne ait bazı kullanımlar.

```text
import random

def createTenRandoms(x=0,y=11,isInt=False):
	numbers=[]
	for i in range(11):
		rnd=random.random() if isInt else random.randint(x,y)
		numbers.append(rnd)
	return numbers

numbers1=createTenRandoms(False)
print(numbers1,"\n")

numbers2=createTenRandoms(0,11,True)
print(numbers2,"\n")

print("a random number is",random.choice(numbers1),"\n")
print("and next random number is",random.choice(numbers1))
```

![notes_8.gif](/assets/images/2016/notes_8.gif)

createTenRandoms isimli fonkisyon üç parametre almaktadır ve bu parametrelerin varsayılan ilk değerleri de verilmektedir. Yani metod çağrısı sırasında x,y ve isInt değişkenlerine bir değer atanması zorunlu değildir. Fonksiyon içerisine gelen isInt değerine göre ternary operatörüne benzer bir kullanım söz konusudur. Eğer isInt değeri True gelmişse random () fonksiyonu, False gelmişse randint (x,y) çağrısı söz konusudur. random () rastgele kayan noktalı sayılar üretirken random (x,y) x ve y aralığında yer alacak rastgele tamsayılar üretir. Üretilen rastgele sayılar numbers isimli listeye eklenir ve bu liste fonksiyondan geri döner.

O gün izlediğim kitaptan yararlanarak python çalışma zamanından Raspberry Pi üzerindeki uygulamaların nasıl çağırılabileceğine de bakmışım. Bu işlem için subprocess isimli bir modülden yararlanılmakta.

```text
import subprocess

print(subprocess.check_output("pwd"))
print(subprocess.check_output(["ls", "l"]))
subprocess.run(["ping", "8.8.8.8"])
```

Modüllerin kullanımlarını bu şekilde inceledikten sonra Raspberry PI'ye internetten bir kaç paket yüklemeyi denemişim. Ruby'de ki gem veya.Net'teki Nuget paketleri gibi indirilip python tarafında kullanılabilen hazır kütüphaneler söz konusu. Epey faydalı kütüphane olduğunu ifade edebilirim. Örneğin web uygulamaları geliştirmek için kullanılabilecek flask kütüphanesini terminalden aşağıdaki gibi yüklememiz mümkün.

```bash
sudo pip install flask
```

veya belli bir sürümünü yüklemek için

```bash
sudo pip install flask==0.10.2
```

Kütüphanenin başarılı bir şekilde yüklenip yüklenmediğini anlamak da oldukça kolay aslında. Bunun için python komut satırından

```text
import flask
```

şeklinde komut çalıştırılması yeterli. Bir hata mesajı almıyorsak paket başarılı bir şekilde yüklenmiştir.

Paketleri kaldırmak için uninstall parametresinden yararlanılıyor.

```bash
sudo pip uninstall flask
```

Eğer sistemde yüklü olan paketleri görmek ve hatta bunları bir metin dosyasına yazdırmak istersek terminalden aşağıdaki komutu girmemiz yeterli.

```bash
sudo pip freeze>modules.txt
```

> Bu arada uzun modül adları söz konusu olursa import ve as ile bu modüle takma ad (alias) vererek kod içerisinde daha kolay kullanılmalarını sağlayabiliriz.
> ![notes_9.gif](/assets/images/2016/notes_9.gif)

Aslında kendi modüllerimizi geliştirmemiz oldukça kolay. Tek yapılması gereken aynı alana ait operasyonları içeren py uzantılı bir kod dosyası oluşturmak. Bu kod dosyasının adı aynı zamanda modülün adı olacaktır.

### Paketler

Bir de Paket kavramı var tabii. Notlarıma baktığımda ilerleyen günlerde paketler (Packages) ile ilişkili bir şeyler karaladığımı gördüm.

![notes_18.gif](/assets/images/2016/notes_18.gif)

Aslında aynı amaca hizmet eden n sayıda modülü hiyerarşik bir klasör yapısı ile ifade edip paket haline getirebiliriz. Kritik nokta her paket içinde init.py isimli bir dosyanın olmasıdır. Bu dosyayı içeren klasör aslında otomatik olarak bir paket haline gelir. Örneğin aşağıdaki gibi bir klasör yapımız olduğunu düşünelim.

serialization/
__init__.py
schema.py
serializers.py
common.py

init.py nin bırda (yanlışlıkla bırda yazmışım ve burada diye düzeltmek içimden gelmemiş) bir içeriğie sahip olması zorunlu değildir. Ancak kod parçaları da içerebilir. Varlığı serialization klasörünün bir paket haline gelmesi için yeterlidir. Eğer bu paketten örneğin schema modülünü kullanmak istersek

from serialization import common

gibi bir ifadeyi ele almamız gerekir. O gün konu ile ilişkili olarak internetten araştırma yaptığımı not almışım. [Stackoverflow'da konu ile ilgili güzel bir içerik de bulmuşum.](http://stackoverflow.com/questions/448271/what-is-init-py-for)

## Web Sitesi Yayımlamışım [12 Haziran 2012]

O gün kitap üzerinden çalışmaya devam etmişim ve Raspberry Pi'yi bir web sunucusu haline getirmeye çalışmışım. Raspberry Pi oldukça ucuz bir kart ama aynı zamanda bir bilgisayar. Bu nedenle onu bir web sunucusu haline getirebilir, üzerinde web uygulamaları barındırabiliriz. Hatta micro-service'ler konuşlandırarak etkileşime geçtiği cihazlardan (örneğin ortam sıcaklığı,mesafe, odadaki ışık şiddeti gibi ölçümler yapabilen çeşitli tipteki sensörlerden) dış dünyaya servis bazlı veri sağlayabiliriz (REST tabanlı servisler ile sensör verilerini JSON formatında yayımlamak gibi) Tabii ilk adım Raspi'yi bir web sunucusu haline getirebilmek. İşte bunun için aşağıdaki adımları izlemişim.

### Apache Kurulumu

İlk olarak bir web sunucusuna ihtiyacımız var. En uygun seçim Apache. Apache kurulumu için terminalden aşağıdaki komutu çalıştırmamız yeterli.

```bash
sudo apt-get install apache2
```

Kurulum öncesi sistemi update etmekte yarar olabilir. Kurulum tamamlandığında tarayıcıdan makine IP adresine bağlanılarak apache web sunucusunun kurulup kurulmadığı kontrol edilmelidir. Hatırlanacağı üzere Raspi'nin o anki IP adresini öğrenmek için terminalden hostname -I komutunu vermek yeterliydi. O gün benim Raspim bana 192.168.1.105 nolu IP adresini vermiş.

### Index.html İçeriğinin Değiştirilmesi

Apache kurulduktan sonra /var/www/html içerisine varsayılan bir index sayfası koyar. Bu HTML içeriği istenildiği gibi değiştirilebilir ve kendi karşılama sayfamız sunucuya yüklenebilir. Ben index.html içeriğini aşağıdaki gibi değiştirmişim.

```text
<html>
	<head>
		<title>Burkinin Ahududu Bahcesi</title>
	</head>
	<body>
		<h1>Burkinin ahududu bahcesine hosgeldiniz!</h1>
		<p><a href="http://www.buraksenyurt.com">Blog</a></p>
	</body>
</html>
```

Sonrasında kendi bilgisayarımdan bu adrese giderek Raspim üzerinde değiştirdiğim index.html içeriğine ulaşabilmişim.

![notes_10.gif](/assets/images/2016/notes_10.gif)

### Php Kullanımı

Derken işi ilerletip web sunucusunda php ile nasıl kod yazılacağına dair notlar almışım. Tabii ilk önce php alt yapısının yüklenmesi gerekiyor. Bunun için yine terminalden ilerlememiz lazım.

```bash
sudo apt-get install php5 libapache2 -mod -php5
```

install'dan sonra gelen ikinci parametre dizisi php'nin apache için gerekli eklentisinin yüklenmesini sağlamakta.

### Bir php dosyasının oluşturulması

Tabii sisteme php yükledikten sonra bir de Hello World demek gereki değil mi?(Ama php tarafına bulaşacağımı hiç sanmıyorum. Şimdiden not edeyim) Bunun için kitapta öngörüldüğü üzere bilgi.php uzantılı bir dosyayı oluşturmuş ve içerisine aşağıdaki kod parçasını yazmışım.

```text
<?php
    phpinfo();
?>
```

phpinfo () metodu ile sistemde yüklü php versiyonunu ve içerisinde yer alan modül listesini elde etmemiz mümkün. Çekinmeyin deneyin.

## Söyleşi Öncesi Hazırlık [15 Haziran 2016]

![notes_12.gif](/assets/images/2016/notes_12.gif)

Bugün Recep Duman hocamla yapacağım söyleşi öncesi bazı notlar almışım. Özellikle python diline ait genel özelliklere çalışmışım.

- Yorumlamalı (Interpreted) dillerdendir. Yani kaynak kodun tamamının derlenip makine diline çevrilmesi yerine sırası gelen satırın derlenip o anda çalışması söz konusudur.
- Dinamik tip sistemine sahiptir.
- Platform bağımsızdır. linux, macos, windows, amiga, symbian gibi ortamlarda çalışabilir.
- Modüler bir yapısı vardır.
- Nesne yönelimli (Object Oriented) bir dildir.
- Girintilere (Indent) dayalı, katı kuralları olan, büyük küçük harf duyarlı bir söz dizimine sahiptir.
- Python yazılım vakfınca desteklenmektedir.
- Tasarımcısı [Guido Van Rossum](https://en.wikipedia.org/wiki/Guido_van_Rossum)'dur ve ilk sürümü 1991 yılında çıkmıştır. Kendisine python topluluğu tarafından "Yaşam Boyu Hayırsever Diktatör (Benevolent Dictator For Life)" ünvanı verilmiştir.
- Büyük çaplı yazılımların hızlı geliştirilmesi ve prototip üretilmesinde C,C++ gibi dillere tercih edilir. Hatta Java diline göre 3 ila 5, C++ diline göre ise 5 ila 10 kez daha kısa kod blokları ile aynı işlerin yapılabilmesine olanak vardır.
- Groovy, ruby ve javascript gibi dilleri etkilemiştir. Kendisi ise C, Haskell, Java, Lisp ve Perl'den etkilenmiştir.
- Adını yılandan değil Monty Python grubunun Monty Python's Flying Circus isimli gösterisinden almıştır.
- Betik (script) dili olarak openoffice.org, GIMP, Inkspace, Blender, Scribus, Paintshop Pro vb ürünlerde kullanılmıştır.
- Django (Python ile yazılan programların internet üzerinden çalışmasına olanak sağlar ki bu bağlamda The New York Times, The Guardian, Bit Bucket ve Instagram gibi siteler doğmuştur), Youtube, BitTorrent, Pardus, Google, NASA ve CERN python kullanan firma ve ürünlerden sadece bir kaçıdır.

## REST Servis Kullanımı [18-Haziran-2016]

O gün [Weather Data Service](https://home.openweathermap.org/) isimli bir Web API'yi Raspi üzerinden python ile nasıl kullanabileceğime çalışmışım. Pek çok Web API hizmetinde olduğu gibi bu servisten yararlanabilmek için bir uygulama anahtarına (Application Key) ihtiyacımız var. Bunun için öncelikle hizmete üye olmalı ve bir API Key ürettirmeliyiz. Üzerinde çalıştığım örnek temel olarak ilgili API'ye bir REST talebi gönderip sonuçlarını almakta. Örneğin bir şehrin 3 günlük hava durumu bilgisine ulaşmanız mümkün. O günkü çalışmada benim için kritik olan nokta, bu tip hizmetler için HTTP çağrılarının python üzerinden nasıl gerçekleştirildiğini görmekti. Python tarafındaki kodda bu talepleri zahmetsizce gönderebilmek için requests modülünden yararlanabiliriz. Ancak bu modülü aldığım notlara göre sisteme yüklenmesi gerekiyor.

```bash
sudo pip install requests
```

Bu işlemin ardından çok basit bir şekilde aşağıdaki kod parçasında olduğu gibi talep gönderilmesi ve JSON formatında içerik alınması mümkün hale gelmekte.

```text
import requests
import json

def getWeatherData(city_id,ApiKey):
	data=get("http://api.openweathermap.org/data/2.5/forecast?id={}&APPID={}".format(city_id,KEY))
	return data.json()
	
print(getWeatherData("buraya sehir idsi gelmeli","buraya api key gelmeli"))
```

get metodu dikkat edileceği üzere basit bir HTTP Get talebi göndermekte. Bunu yaparken de Open Weather Map'in gösterdiği kriterlere göre parametre alıyor. id ile şehir numarasını appid ile de servis kullanıcısına açılan uygulama anahtarını kullanıyor. Dönen içeriği JSON formatında ele almak için json () metodunun çağırılması yeterli. Sanıyorum o gün oldukça yorgun olmalıyım ki sadece örneği yapıp bilgisayarda kullanmışım. Deftere de fazla not almamışım. (Dolayısıyla python ile bir REST servis yazıp bunu kullandıracağım bir makaleyi yazılacaklar listesine almam lazım)

## Kriptografi ve One-Time Pad Algoritması [19-Haziran-2016]

Eğlenceli bir gece geçirmişim. One-Time Pad algoritmasını öğrenmiş bunun python dili ile yazımını gösteren tutorial'ı adım adım izlemişim. Aslında One-Time Pad algoritmasını anlamakta epey zorlandığımı hatırlıyorum. Kağıt kalem ile ders çalışır gibi yazıp çizerek işi kurtarmıştım.

![notes_11.gif](/assets/images/2016/notes_11.gif)

Örneğin burak kelimesini şifrelemek istediğimizi düşünelim. b alfadeki 1nci indise denk gelir. Diğer yandan örnek otp dosyasının birinci değeri 10dur. Buna göre 1+10 toplamının mod 26 değerine bakılır ki bu değer 11dir. 11 ise alfabede yer alan l harfine karşılık gelmektedir. Bu şekilde ilerlendiğinde burak kelimesi için lzaee şeklinde şifrelenmiş bir içerik üretilecektir. Şifrelenen içeriğin çözümlenmesi sırasında ise tam tersi durum söz konusudur. l harfinin alfabedeki indisi 11dir. Şifreleme sırasında b harfi için 10 ekleme yapıldığından tam tersi olacak şekilde 11-10 işleminin mod 26 değerine bakılır. Bu değer de 1dir ve sıfır indisli alfabedeki b harfine denk gelmektedir. Bu şekilde şifreleme ve ters şifrleme işlemleri one-time pad algoritmasına göre yapılabilir.

Kodlara gelirsek. Öncelikle one-time pad için gerekli sayı dizilerini içeren rastgele dosyalara ihtiyacımız var. Bu dosya üretimi ve üretilen dosya içeriklerinin görülmesi için aşağıdaki kod parçasından yararlanılabilir.

```text
import random

alfa="abcdefghijklmnopqrstuvwxyz"

def generateOtpFiles(fileCount,length):
	for sheet in range(fileCount):
		with open("otpFile"+str(sheet)+".txt","w") as f:
			for i in range(length):
				f.write(str(random.randint(0,26))+"\n")

def loadOtpFile(filename):
	with open(filename,"r") as f:
		content=f.read().splitlines()
		
	return content

generateOtpFiles(3,100)
print(loadOtpFile("otpFile1.txt"))
```

![notes_13.gif](/assets/images/2016/notes_13.gif)

generateOtpFiles metodu iki parametre alır. İlk parametre one-time pad için üretilecek dosya adedini, ikinci parametre ise bu dosyalar içerisine atılacak karışık sayı miktarını ifade eder. Açılan ilk for döngüsü dosya sayısı kadar, ikinci for döngüsü ise her bir dosyanın içereceği eleman sayısı kadar döner. with open ile başlayan kısımda verilen w değeri, dosyanın yazma amaçlı olarak açılacağını belirtir. Dosya içerisine 0 ile 26 arasında rastgele tamsayılar atamak için randint (0,26) metodundan yararlanılır. Elde edilen sayısal değer f ile ifade edilen dosyaya write metodu ile yazılır.

Üretilen bir otp dosya içeriğini görmek içinse loadOtpFile metodundan yararlanılır (Bu bir nevi test metodu olarak düşünülebilir aslında) Yine with open ile başlayan satırda dosyanın bu kez okuma amaçlı olarak açıldığı r anahtarı ile belirtilmektedir. Dosya içeriğini okumak için başvurulan read metodunu takiben splitlines fonkisyonuna da çağrı yapılır. Bu sayede ilgili dosya içeriğinin satır bazında elde edilmesi ve bir listeye alınması sağlanmış olur. Sıra geldi şifreleme ve çözümleme metodlarını yazmaya. Program kodunun son hali aşağıdaki gibidir.

```text
import random

letters="abcdefghijklmnopqrstuvwxyz"

def generateOtpFiles(fileCount,length):
	for sheet in range(fileCount):
		with open("otpFile"+str(sheet)+".txt","w") as f:
			for i in range(length):
				f.write(str(random.randint(0,26))+"\n")

def loadOtpContent(filename):
	with open(filename,"r") as f:
		content=f.read().splitlines()
		
	return content

def encryptMessage(message,otp):
	text=""
	for position,character in enumerate(message):
		if character not in letters:
			text+=character
		else:
			idx=(letters.index(character)+int(otp[position])) % 26
			text+=letters[idx]
	
	return text
			

def decryptMessage(message,otp):
	text=""
	for position,character in enumerate(message):
		if character not in letters:
			text+=character
		else:
			idx=(letters.index(character)-int(otp[position])) % 26
			text+=letters[idx]
	
	return text

generateOtpFiles(3,100)
otpContent=loadOtpContent("otpFile1.txt")

yourMessage=input("Please type your secret message").lower()
print("Your message is '{0}'".format(yourMessage))
em=encryptMessage(yourMessage,otpContent)
print("Encrypted message is '{0}'".format(em))
dc=decryptMessage(em,otpContent)
print("Decrypted message is '{0}'".format(dc))
```

![notes_14.gif](/assets/images/2016/notes_14.gif)

Görüldüğü gibi kullanıcının girdiği mesaj önce şifrelenmiş ve sonra şifrelenen içerikten tekrar elde edilmiştir. Kodda o gün için öğrenilecek pek çok yeni kavram da çıkmış bana. Örneğin ekrandan bilgi almak için input fonksiyonundan yaralanıyoruz. Fonksiyon arkasından yapılan lower çağrısı ise girilen içeriğin küçük harfe dönüştürülmesini sağlıyor. Nitekim alfabemiz küçük harflerden oluşmakta. encryptMessage ve decryptMessage metodları şifrlenecek ve çözümlenecek içerik ile kullanılacak one-time pad dosya içeriğini parametre olarak alan fonksiyonlar. Her iki metod içerisinde algoritmanın gereklilikleri yerine getiriliyor.

Üzerinde çalışılacak mesajın içeridiği karakterlerin kendisi ve yerlerini dolaşabilmek için for döngülerinin nasıl kullanıldığına dikkat edin. enumerate metodu ile mesajlardaki karakterleri ve indis değerlerini key-value çiftleri şeklinde ele alabiliyoruz. letters isimli metin katarında yer alan bir lokasyona gitmek için index ([indis değeri]) notasyonundan yaralanmaktayız. Benzer şekilde yüklenen otp içeriğindeki bir sayıya gitmek için de otp[position] notasyonunu kullanmaktayız. O gün izlediğim bu tutorial sayesinde iyi bir pratik yapmış olduğumu görüyorum.

## İkinci Kitap ile Python'a Doğru Kaymış Gönlüm [21 Haziran 2016]

Her ne kadar Raspberry Pi için bir çok sensör ve malzeme almış olsam da sanıyorum ki python dili epeyce hoşuma gitmiş. Bu nedenle o gün python dilini anlatan ikinci kitaba başlamışım (Gerçi diğer kitapta kaldığım kameranın Raspberry Pi'den kontrolü ile ilişkili kısmı kamerayı alamadığım için beklemeye almak zorunda kalmamın da bunda etkisi olmuş olabilir) İlk bölümlerde aşina olduğum kısımları hızlıca geçtikten sonra python içerisindeki temel veri türlerini kaleme almışım. Ne yazık ki gece yaptığım çalışmam S (h) arp Efe tarafında sabote edilmiş ve o da kendi bakış açısından çalışma notlarına ekler ilave etmiş.

![notes_15.gif](/assets/images/2016/notes_15.gif)

Sadede gelecek olursak temel veri türlerini şu şekilde özetleyebiliriz.

![notes_16.gif](/assets/images/2016/notes_16.gif)

Tabii en çok dikkatimi çeken nokta kompleks sayıları doğrudan ifade eden bir türün olması ki şu şekilde sanal ve gerçel köklerini ele alıp kullanabiliriz.

```text
kompleksX=3.14+4j
print(kompleksX.real)
print(kompleksX.imag)
kompleksY=-3-8.125j
kompleksZ=kompleksX+kompleksY
print(kompleksZ)
```

![notes_17.gif](/assets/images/2016/notes_17.gif)

Gelelim temel veri türleri ile ilgili diğer notlara.

- Metinsel ifadeler için kullanılan String veri türü immutable bir tiptir. Bir başka deyişle string olarak ifade edilen bir karakter dizisinin elemanlarını değiştiremeyiz.
- Benzer şekilde Tuple veri türü de Immutable'dır. Yani oluşturulduktan sonra doğrudan elemanlarını değiştiremeyiz. Ancak istisnai durumlar da vardır. Söz gelimi Tuple'ın bir elemanı List tipinden olabilir. Listeler Mutable dır. Yani elemanları değiştirilebilir. Dolayısıyla bir Tuple içerisinde tanımlı listenin elemanlarında değişiklik yapabiliriz.
- Listeler çoğunlukla farklı tipteki elemanları ardışıl olarak saklamak için kullanılan veri türüdür. Mutable'dır. Yani elemanlarını değiştirebiliriz. Elamanları diğer python veri türlerinden her hangibirisi olabilir.
- Sets olarak ifade edilen kümeler tekrarlı elemanlar içermeyen dizilerdir. Her ne kadar dizi gibi olduklarını not almış olsam da elemanlarına indis operatörü ile erişmek mümkün değildir.
- Dictionaries Mutable tiplerden bir diğeridir. Elemanlar key-value çiftleri şeklinde saklanmaktadır. Anahtar değerleri benzersiz (Unique) olmak zorundadır.

O gün örnek kod parçalarını deneyerek çalışmalarıma devam etmişim.

```bash
#En basit haliyle bir liste tanımı. Sondan bir önceki eleman küme son eleman bir Tuple'dır.
someList=[100,-90,"burk",True,False,3.1415,
			{"one","two","five","six"},
			("burki",1195,"Math.Eng")]
print(someList)

#En basit haliyle bir Tuple tanımı
person=(90001,"burak selim","şenyurt",1.78,"White")
print(person)

#En basit haliyle küme tanımlaması. Çıktıya dikkat edilecek olursa tekrar eden elemanların teke indirildiği görülebilir.
someSet={3,4,5,1,2,18,16,3,3,2,1,-1,91}
print(someSet)
print("3 küme içinde mi? {}".format("Evet" if 3 in someSet else "Hayır"))
#Bir eleman listesinin tekilleştirilmesi istenirse en basit haliyle kümelerden yararlanılabilir. Örneğin aşağıdaki metinsel ifade içerisinde geçen harfleri tekil olarak elde etmek istersek
motto="This is my last world"
mottoSet=set(motto)
print(mottoSet)
#Kümelerle aynı matematikteki gibi küme işlemleri yapılabilir
set1={1,2,3,4,6,8,10,12}
set2={4,5,6,7,8,9,10}
print("Çıkartma-> set1-set2 = ",set1-set2)
print("Birleşim-> set1|set2 = ",set1|set2)
print("Kesişim-> set1&set2 = ",set1&set2)
print("Kesişim Dışında Kalanlar-> set1^set2 = ",set1^set2)

#En basit haliyle dictionary tanımlaması
studentNotes={"klara":90,"norman":100,"burk":38,"tubi":85}
print("Klara'nın notu",studentNotes["klara"])
```

![notes_24.gif](/assets/images/2016/notes_24.gif)

## Metodlara Bir Bakıp Çıkmışım [23 Haziran 2016]

O gün metodlar ile ilgili bir şeyler yapmışım. Aslında 17 Hazirandaki ["Bu Yaz Macerasının Adı Python"](https://www.buraksenyurt.com/post/bu-yaz-macerasinin-adi-python.aspx) isimli yazımda metodlara biraz da olsa değinmişim. Ancak kitabı takip etmeye devam ettiğim için tekrar mahiyetinde de olsa bazı notlar da almışım. Hatta yeni bilgiler de edinmişim. Örneğin değişken uzunluk parametre alan bir metod üzerinde çalışmışım (Yani C# tarafındaki params kullanımını işlemişim) pass kullanımı ile NotImplementedException deneyimi yaşamış, Default Method Arguments ile metod parametrelerinde varsayılan değer kullanımına bakmışım. Kısaca aşağıdaki kodları çalışmışım.

```text
'''
Aşağıdaki kod satırlarında metodlara ait şu kavramlara yer verilmektedir.
pass
varsayılan parametre değerleri
değişken uzunluklu metod parametreleri
lambda kullanımı
'''

#bir metodun içeriği sonradan tamamlanacaksa kullanılabilir. Biraz NotImplementedException vari bir kullanım
def getPlayerStatistic(playerName):
	pass

#metod argümanları isim verilerek kullanılabilir
def doConnect(port,serverName):
	#do something
	print("Connection for {}:{} is ok".format(serverName,port))
	return False
	
#metod parametrelerine varsayılan ilk değerler verebiliriz.
def findLocation(playerName,city="Paris"):
	#do something
	print(city,"is scanning for",playerName)

# C# taki params kullanımı burada da var ki
def createPlayer(name,city,*properties):
	print("{} from {}\n".format(name,city))
	prop=[]
	for p in properties:
		prop.append(p)
		
	print(prop)
	print("*"*10)

# lambda kullanımı. Bunlar tek satırlık metodlar olarak düşünülebilirler. C# taki => ve anonymous metodları hemen aklınıza gelmiştir.
sum=lambda x,y:x+y
print(sum(4,5))

getPlayerStatistic("burk")
result=doConnect(port="8080",serverName="192.168.1.107")
print("Connection is {} success".format("" if result else "not"))
findLocation("burk")
findLocation("tubi","new york")
createPlayer("burki","istanbul","black","master",True,1685)
```

ve çalışma zamanı sonuçları.

![notes_21.gif](/assets/images/2016/notes_21.gif)

## String Veri Yapısı ile Eğlenceli Dakikalar [25 Haziran Cumartesi]

Bugün string veri yapısını kullanarak eğlenceli kod parçalarını ele almışım. Hatta string tipinin pek çok metodunu sonradan denemem için de not düşmüşüm (Niye göbekli bir karakter çizdiğimi ben de bilemiyorum sayın seyirciler)

![notes_19.gif](/assets/images/2016/notes_19.gif)

Peki ya kodlar?

```text
motto="this is gonna be the BesT Day of my liFe"

#Büyük harfe çevirme ve metin uzunluğunu bulma
print("{} (length={})".format(motto.upper(),len(motto)))

#İlk dört karakteri yazdırma
print(motto[:4])

#Metni tersten yazdırma
print(motto[::-1])

#Son 4 karakter hariç tüm metin
print(motto[:-4])

#this ile başlayıp başlamadığını bulma
print(motto.startswith("this"))

#Cümlenin ilk harfini büyük harfe çevirme
print(motto.capitalize())

#Boşluk karakterine göre metni kelimelere ayırma
words=motto.split(" ")
for w in words:
	print(w)

#4üncüden 7nciye kadar
print(motto[4:7])

#Büyük harfleri küçük, küçük harfleri büyük harfe çevirme
print(motto.swapcase())

#İçerik sayısal mı?
print(motto.isalnum())
print("450048".isalnum())

#Metin içinden karakter değiştirme
newMotto=motto.replace(" ","_")
print(newMotto)

# in operatörü ile bir kelimeyi metin içinde arama
print("is there a 'gonna'? {}".format("Yes" if "gonna" in motto else "No"))
```

![notes_23.gif](/assets/images/2016/notes_23.gif)

Bu arada string türü ile ilişkili olarak [şu adresten de](http://www.tutorialspoint.com/python/python_strings.htm) yararlanmaya ve bilgi almaya çalışmışım.

## Büyülü Kodlar [27 Haziran 2016]

O gün okuduğum kitapta çok enteresan kod parçalarına rastlamışım ve bunları teker teker denemişim. Kod parçaları bana sihirli gelmiş olacak ki bir de sihirbaz şapkası çizmeye çalışmışım.

![notes_20.gif](/assets/images/2016/notes_20.gif)

```text
import random

#rastgele sayı üretmenin uzun bir yolu
numbers=[]
for i in range(10):
	numbers.append(random.randint(10,100))
print(numbers)

#ve rastgele sayı üretmenin tek satırlık kısa yolu
numbers=[random.randint(10,100) for i in range(10)]
print(numbers)

#0 ile 100 arasında 7 ile bölünebilen sayıları listeye almanın uzun yolu
points=[]
for i in range(101):
	if  i%7==0:
		points.append(i)
print(points)

#0 ile 100 arasındaki sayıları listeye almanın kısa yolu
points=[i for i in range(101) if i%7==0]
print(points)

#iki listenin iç içe iki döngü ile ele alınmasının uzun yolu
colorList=["green","gray","red","blue"]
players=["burki","tubi","tom"]
combination=[]
for c in colorList:
	for p in players:
		combination.append(c+" "+p)
print(combination)

#iki listenin iç içe iki döngü ile ele alınmasının kısa yolu
combination=[c+" "+p for c in colorList for p in players]
print(combination)
```

Örnekte 3 farklı operasyon söz konusu. İlk olarak 10 ile 100 arasında rastgele tamsayılar üretip bunları bir listeye alıyoruz. İkinci örnekte 0 ile 100 arasındaki sayılardan 7 ile bölünebilenlerini çekiyoruz. Üçüncü ve son örnekte ise iki farklı listenin tüm ikili kombinasyonlarını çıkartmaktayız. Örneklerin uzun ve kısa versiyonlar var. Kısa versiyonlarda köşeli parantezler içerisinde yazılan ifadeler beni oldukça etkiledi. Köşeli parantezler içerisindeki ifadeleri 3 ana parça halinde düşünebiliriz. İlk parçada eşitliğin sol tarafına atanacak değer ifade edilir. Sonrasında ise bir for döngüsü ve bunu takiben de eğer gerekliyse koşullu bir ifadeye yer alır. Bu sayede tek satırda bir sayı dizisini dolaşıp belirli kriterlere uyanlar için işlemleri kısaca yaptırabiliriz.

![notes_22.gif](/assets/images/2016/notes_22.gif)

Çalışmalarım bu tarih itibariyle sonlanmış. Nitekim izin dönemi gelmiş. Şimdi kaldığımız yerden devam edebilirim. Bu uzun yazıda Python notlarımı elektronik ortama almaya çalıştım. Umarım python severler için yararlı bir yazı olmuştur. Bir başka makalemizde görüşünceye dek hepinize mutlu günler dilerim.