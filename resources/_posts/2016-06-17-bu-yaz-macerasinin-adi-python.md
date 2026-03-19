---
layout: post
title: "Bu Yaz Macerasının Adı Python"
date: 2016-06-17 00:00:00 +0300
description: "Kısa bir süre önce bu yaz dönemi için kendime yeni bir uğraşı buldum. Raspberry Pi. Çalıştığım turuncu bankadaki ekip arkadaşım sevgili Recep Duman'ın, Raspberry Pi ile epey zamandır uğraştığını da öğrenince planlar yapılmaya başlandı.Lego, IoT ve Raspberry Pi konularını bir araya getireceğimiz bir fikir üzerinde ilerlemeye karar verdik. Bunun için gerekli çalışmalarımıza devam ediyoruz. Hatta Recep Hocam ile gerçekleştirdiğimiz iki söyleşimize buradan ve şuradan ulaşabilirsiniz."
categories:
  - python
tags:
  - python
  - bash
  - dotnet
  - http
  - java
  - ruby
  - javascript
---
# Bu Yaz Macerasının Adı Python
![Guido_van_Rossum.gif](/assets/images/2016/Guido_van_Rossum.gif)

Merhaba Arkadaşlar,

Kısa bir süre önce bu yaz dönemi için kendime yeni bir uğraşı buldum. Raspberry Pi. Çalıştığım turuncu bankadaki ekip arkadaşım sevgili [Recep Duman](http://www.recepduman.net/)'ın, Raspberry Pi ile epey zamandır uğraştığını da öğrenince planlar yapılmaya başlandı.Lego, IoT ve Raspberry Pi konularını bir araya getireceğimiz bir fikir üzerinde ilerlemeye karar verdik. Bunun için gerekli çalışmalarımıza devam ediyoruz. Hatta Recep Hocam ile gerçekleştirdiğimiz iki söyleşimize [buradan](https://youtu.be/LfTrBhXo8vk) ve [şuradan](https://youtu.be/h3xfi1-SmQc) ulaşabilirsiniz.

Raspberry Pi kredi kartı büyüklüğünde de olsa sonuçta bir bilgisayar olduğundan, onu yönetebilmek için de programlama dillerinden yararlanmak gerekiyor. Raspi üzerinde Java, Scratch (Scratch'i MIT'den bilirsiniz), Ruby ve Python gibi dilleri kullanarak geliştirme yapmak ve donanıma bağlanan cihazları yönetebilmek mümkün. Pek tabii Windows IoT açısından olaya baktığımızda C# dilini de kullanabiliriz. Ben Ruby ile ilerlemeyi düşünüyordum ancak hazır yeri gelmişken Python programlama dilini odağıma almaya karar verdim.

> Raspberry Pi üzerine varsayılan olarak yüklenen Raspbian işletim sistemi Java, Python, Ruby, Scratch gibi diller için gerekli platform desteği ile birlikte geliyor. Dolayısıyla IDE'leri de hazır. Hatta Python için 2 ve 3 sürümlerinin ayrı ayrı kullanılabildiğini söyleyelim.

Python'u pek tabii Raspberry Pi üzerinde çalışmaya başladım. Bu anlamda özellikle [Raspberry Pi Foundation](https://www.raspberrypi.org/resources/learn/)'ın ilgili sitesi sunduğu örnekler açısından bana büyük kolaylık sağladı. Hatta ilk hevesle Twithyon API'sini kullnarak Tweet atmayı denedim. Bu kaynağa ek olarak [http://pythonprogramminglanguage.com/](http://pythonprogramminglanguage.com/) adresinden de yararlanmaya başladığımı ifade edebilirim. Dilin temel özelliklerini hızlı bir şekilde öğrenebileceğiniz güzel bir kaynak.

Evde çalışmak oldukça kolay nitekim Raspberry Pi'mi çalışma masam üzerinde kurduğum yapıda rahatça kullanabiliyorum. Ne varki Raspim henüz derli toplu bir bilgisayar değil. Tüm çevre üniteleri dağınık durumda. Üstelik masanın üzerinde bir sürü kablo var. HDMI kablosu, her yeri çıplak olan 7" dokunmatik ekran, ısı sensörü, 40 Pin GPIO tesisatı, klavye, mouse vs.. Bu donanımları sürekli şirkete taşımak ve müsait vakitlerde çalışmak çok da kolay değil. Bu yüzden ben de online olarak kodları deneyebileceğim bir platform aradım. En azından dilin belli karakteristik özelliklerini inceleyebileceğim bir ortam yeterli olacaktı. Sonunda [repl.it isimli siteyi](https://repl.it) buldum. Dolayısıyla öğle araları veya mesai saatleri dışındaki vakitlerimi (bazen de çaktırmadan mesai saatleri içindeki) değerlendirebileceğim ortamlar üç aşağı beş yukarı hazırdı.

## Python'un Karakteristik Özellikleri

Gelelim yeni gözdem Python dilinin temel özelliklerine. Hollandalı [Guido Vann Rossum](https://tr.wikipedia.org/wiki/Guido_van_Rossum) tarafından ilk sürümü 1991 yılında yayınlanan Python, yorumlamalı (Interpretter destekli), nesne yönelimli (Object Oriented), dinamik ve güçlü tip sistemine sahip, fonksiyonel bir dil olarak düşünülebilir. Ruby ile pek çok benzerliği bulunmakta. Yorumlamalı bir dil olduğu için terminal ekranı üzerinden scriptleri yazılabilir ve sonuçlarını hemen o anda görebiliriz. Diğer yandan nesne yönelimli bir dil olduğundan Domain Driven Design gibi konseptlere hizmet edecek şekilde büyük çaplı projelerde de kullanabiliriz. Aslında Raspbi ve IoT söz konusu olduğunda Python'u kullanarak yapabileceklerimizin hayal gücümüz ile sınırlı olduğunu söyleyebiliriz.

Python Yazılım Vakfınca desteklenen açık kaynak kodlu olan bu dil Google, Youtube, Facebook, Bittorrent, NASA, CERN, OpenOffice gibi kulvarlarda yaygın şekilde kullanılmakta. Pardus'un çekirdeği değil ama üzerine konuşlandırılan uygulamalar Python ile yazılmış örneğin. Hatta Python'dan yararlanılarak geliştirilen çeşitli betik (script) diller de söz konusu. Python esasen ABC, ALGOL 68, C, Haskell, Icon, Java, Lisp, Modula-3, Perl gibi dillerden esinlenerek geliştirlmiş ve Boo, Cobra, D, Falcon, Groovy, Ruby, JavaScript, Comfy gibi dilleri etkilemiş. Dolayısıyla oldukça popüler bir programlama dili. Dilin benim için en ilginç özelliği ise blok ifadelerinde indentation (girintileme) kurallarına sahip olması. Yani bir metoda ait kod bloğunda tab veya space'lerin bir anlamı var. Hatta while, for döngülerinde, if koşullarında da durum böyle. Ne demek istediğimi kodları incelerken göreceğiz.

## İlk Kodlar

İnsan daha önceden bir kaç farklı platform ve programlama dili ile uğraşınca yeni bir dil öğrenmesi de kolay olabiliyor. Ben de bu nedenle ilk etapta biraz dağınık ve karmaşık bir giriş yapmış bulunuyorum. Birazdan denediğim derme çatma kodları sizlerle paylaşacağım. Tam bir düzen olmadığı için lütfen beni affedin.

Kodları yazmaya başladığımda karşılaştığım ilk sorun aşağıya doğru uzayıp giden uzun satırlardı. Ne kadar çok şey denemek istiyorsam main.py isimli dosyayı şişirmekteydi. Bu nedenle main.py dışında diğer öğrendiğim kavramları ayrı bir dosyada metodlaştırarak tutmaya karar verdim. Bu vesile ile pek çok şey de öğrenme fırsatım oldu.

> Python programlama dilinde dosya uzantıları py şeklindedir. Ayrıca main.py aynen.Net tarafındaki main metodu gibi uygulamanın başladığı kod parçası olarak düşünülebilir.

İlk olarak SomeFunctions.py isimli bir dosya oluşturdum. Bu dosyanın kod içeriği ise aşağıdaki gibi.

```text
import math

def get_players():
	players=[
		("burki",1000,True,"1234-PLY"),
		("turp",9003,False,-1,"gold player","4445-PLY"),
		("joiy",1004,True,-10,"silver player")
		]
	return players
	
def get_language(key):
	languages={
		"EN":"English",
			"FR":"France",
			"GE":"German",
			"TR":"Turkish",
			"US":"Unitad State English"
		}
	return languages[key]

def pythagoras(a,b):
	value=math.sqrt(a*a+b*b)
	return value

#lucky number game
def lucky_number_game():
	lucky_num=0
	while lucky_num!=7:
		lucky_num=int(input("Guess a number:"))
	
		if lucky_num!=7:
			print("Too bad,Sorry")
		
	print("Vuhuu")

def greetings():
	print("hello python\'s world")
	print("My name is\nBurak Selim Senyurt\n")
	name=input("so...What\'s your name?")
	print("Wellcome %s" %name)
	age=int(input("How old are you?"))
	print("Your age is %s" %age)

def a_little_bit_for_loops():
	minValue=int(input("Please insert a minimum value"))
	maxValue=int(input("Please insert a maximum value"))

	for n in range(minValue,maxValue+1):
		print(n)

	for i in range(0,5):
		for j in range(0,5):
			print(i,' ',j)
			
def give_me_a_nickname():
	yourNickName="none"
	while len(yourNickName)<5 or len(yourNickName)>10:
		yourNickName=str(input("Please select a nickname"))
		if len(yourNickName)<5:
			print("Your nickname too short")
		elif len(yourNickName)>10:
			print("Your nickname too long")

	print("Your nick name '%s' is cool now" %yourNickName)
	
def using_list():
	some_numbers=[1,3,4,2,6,7,2,2,9,12,14,10,5]
	return some_numbers
```

Biraz karışık duruyorlar değil mi? Korkmayın. Hepsi üzerinden dikkatlice geçeceğiz.

## Metodlar

Öncelikle bu dosya içerisinde sadece metod tanımlamaları olduğunu belirtelim. metodlarda void, int,double,bool gibi geri dönüş tipini belirten hiç bir ifade olmadığı dikkatinizi çekmiştir. Ancak bu, metodların geriye bir şey döndürmediği anlamına da gelmemlidir. Nitekim kod bloğunda return ifadesi ile dönüş yapılan fonksiyonlar da mevcuttur.

Python dinamik bir dil olduğundan herhangibir şekilde tür bildirimi yapılmasına da gerek yoktur. Ne metod parametrelerinde ne de kod bloklarındaki değişken tanımlamalarında bir tür bildirimi yapılmamıştır. Sadece tipin kullanılacağı yerlerde gerekiyorsa dönüşümler söz konusu olabilir. Örneğin kullanıcıdan alınan bir içeriğin sayısal olarak değerlendirilmesi gerekiyorsa.

Metod tanımlamalarının en dikkat çekici özelliği ise: işareti ve bitiş noktalarıdır. Metodlar def anahtar kelimesi ile tanımlanmaya başlanıp girintili yazılması zorunlu olan ifadeler ile devam eder. Metod sonuna gelindiğinde ise son ifadeyi takiben bir girinti verilmesi veya alt satıra geçilmesi yeterlidir. Girintilere dikkat edilmediği takdirde Invalid Syntax hatası alınacaktır (ama hala bu kısmı öğrenmeye çalışıyorum onu ifade edeyim)

Kod dosyasının en başında import anahtar kelimesi ile yapılan bir bildirim mutlaka dikkatinizi çekmiştir. math bir modüldür. İçerisinde pyhton ile built-in gelen bir takım matematiksel fonksiyonellikler bulunmaktadır. Örneğin pisagor üçgeni için hesaplama yapan metod içerisindeki math.sqrt kullanımı için gereklidir. Dolayısıyla kendi geliştirdiğimiz, internet üzerinden indirdiğimiz (.Net'teki Nuget paketleri veya Ruby'deki gem'ler gibi) veya python ile built-in gelen bu tip modülleri kullanabilmek için import ifadesinden yararlanırız.

Gelelim metodlarımızda neler yaptığımıza. Bunları aşağıdaki tabloda kısaca açıklamaya çalıştım.

Metod
İçinde Neler Oluyor?

getplayers
Bu metod içerisinde Tuple kullanımı söz konusudur. players isimli Tuple 3 elemandan oluşmaktadır. Dikkat edilmesi gereken nokta her bir elemanın farklı sayıda ve tipte niteliğe sahip olabileceğidir. Metod geriye oluşturduğu bu Tuple içeriğini döndürür. Tuple tipinin elemanlarına [] operatörü ile erişilebilir. main.py içerisinde örnek bir kullanım söz konusudur. Ancak siz daha fazlasını da keşfetmeye çalışın derim.

getlanguage

Bu fonksiyonda ise Dictionary kullanımı örneklenmiştir. Basit anlamda key:value çiftlerinden oluşan Dictionary'ler sıklıkla kullanılabilecek veri yapılarındandır.
Metod key isimli bir parametre almış ve bunu Dictionary içerisinde [] operatörü ile arama yapılmasında kullanmıştır. Yani [] operatörü key değerini alıp buna karşılık gelen değeri elde etmemizi sağlamaktadır. Metod eğer key içeriği bulunursa değerini döndürü. Bulamazsa none şeklinde bir dönüş yapacaktır.

pythagoras
Pisagor hesabı yapan bu metod a ve b isimli iki parametre almakta olup math.sqrt fonksiyonu ile yaptığı hesaplama sonucunu geriye döndürmektedir. Fonksiyon sadece import kullanımını örneklemek amacıyla ele alınmıştır.

luckynumbergame

Bu metod diğerlerine göre nispeten biraz daha eğlencelidir. Kullanıcının bir sayıyı tahmin etmesi istenir. Bizim için dikkat edilmesi gereken nokta ise kullanılan while döngüsü, if ifadesi ve kullanıcıdan bilgi almak için çağırılan input fonksiyonudur.
Gerek while döngüsü gerek if ifadesi: işareti ile tanımlandıktan sonra yine girintili şekilde koda devam edilmelidir. Aksi durumda bildiğiniz üzere Invalid Syntax hatası alırız.
While döngüsü kullanıcı 7 sayısını girene kadar sürekli olarak bir sayı isteyecektir. Kullanıcının girdiği sayı input fonksiyonu ile alınmakta olup luckynum isimli değişkende saklanmaktadır.

greetings
Aslında ilk yazdığım metod buydu ama karışık sırada eklediğimden garip bir yere gelmiş. Console/Terminal ekranına bilgi basmak için print ve kullanıcıdan giriş almak için input fonksiyonlarının kullanımına yer verilmektedir. Bir de %s ile place holder kullanımı söz konusudur. Tahmin edileceği üzere %name ve %age değerleri, string ifadelerdeki %s yazan yerlere gelmektedir.

alittlebitforloops

Bu metod içerisinde for döngüsüne ait örnekler ve range kullanımı yer almaktadır. range'ler oldukça hoşuma giden bir enstrüman. Verilen parametrelere göre bir değer aralığının otomatik olarak oluşturulmasını sağlmaktadır. Örnekteki ilk döngüde de minValue ve maxValue değerlerinden yararlanılarak range türü ile bir sayı dizisi oluşturulmuştur.
range'e üçüncü bir parametre de verilebilir. Söz konusu parametre ile adım sayısı belirtilir. Yani ikişer ikişer artan bir sayı aralığı oluşturulması da söz konusudur. Deneyin;)
İkinci for döngüsü tahmin edileceği üzere iç içe geçen bir döngü kullanımına aittir.
Bu arada kullanıcıdan aldığımız min ve max değerlerinin sayısal işlemlerde kullanmak için int metodu ile tür dönüşümüne dahil edildiği de gözden kaçmamalıdır.

givemeanickname
Bu fonksiyonda da bir while kullanımı söz konusudur. Ancak koşul da or operatörüne yer verilmiştir. Kullanıcının gireceği nickName'in 5 ile 10 karakter arasında olması sağlanana kadar bilgi istenen sonsuz bir döngü kurgusudur diyebiliriz. Karaketer uzunluklarının tespiti içinse len fonksiyonundan yararlanılmıştır.

usinglist
Son metodumuzda ise liste türüne ait bir örnek verilmiştir. Burada her ne kadar konudan sıkılıp sadece sayısal değerlerden oluşan bir liste söz konusu olsa da farklı türlerden oluşacak listeler tanımlanabileceğini de belirtmek isterim. Deneyin;)

Tuple ile listeler arasında önemli bir fark vardır. Listelere yeni elemanlar eklenebilir ve çıkartılabilir ancak Tuple için bu tip işlemler söz konusu değildir. Tuple'ı sadece okunabilir bir tip listesi gibi de düşünebiliriz. Dictionary kullanımı için de verebileceğimiz bir iki tüyo var. Örneğin Dictionary'ler de değerler sayısal bile olsa çift tırnaklar arasında yazılır. Yani bir tür dönüşümü yapılması söz konusu olabilir. Ayrıca bir Dictionary'ye yeni eleman eklemek için aşağıdaki gibi bir ifade kullanılabilir.

```text
languages["ES"]="Spanish"
```

Diğer yandan var olan bir elemanı değiştirmek istersek de aynı tekniği kullanıyoruz.

```text
languages["EN"]="British English"
```

Dolayısıyla Dictionary'ler için var olan bir anahtar değerini değiştirmenin, silme ve yeniden ekleme operasyonu olduğunu düşünebiliriz.

Listelerin, Tuple ve Dictionary'lerin kullanımı hakkında daha fazla detay için [şu adrese bakabilirsiniz](http://belgeler.istihza.com/py2/liste_demet_sozluk.html). Nitekim ben şu anda bu sayfayı okuyorum.

## Metod Testleri için main.py kullanımı

Peki ilgili metodları nasıl test edeceğiz? Hemen main.py dosyamıza geliyor ve aşağıdaki kod satırlarını oluşturuyoruz.

```bash
#Some practices

import SomeFunctions as func

#interesting code
print(5*"Burki","\n")

# standart input output
func.greetings()

# simple tuples
players=func.get_players()
#print(players)
for player in players:
	print(player)

# simple dictionary
language_code=input("Please give me a country code")
print("\nYour choice is %s\n" %func.get_language(language_code))

# simple list
numbers=func.using_list()
print("\n...and the number list are\n")
for n in numbers:
	print(n)
print("Second element of numbers is %s" %numbers[1])
print("The element count of numbers is %s" %len(numbers))
	
# while, if elif else
func.give_me_a_nickname()

# simple method usage
result=func.pythagoras(int(input("x value:")),int(input("y value:")))
print("pythagoras result is %s" %result)

# while if elif
func.lucky_number_game()

# simple loops
func.a_little_bit_for_loops()
```

Yorumlamalı bir dilden bahsediyoruz. Dolayısıyla kod yukarıdan aşağıya doğru aktıkça çalışma zamanı tarafından yürütülecek. İlk olarak yine bir import tanımı söz konusu. Nitekim metodlarımızı biriktirdiğimiz SomeFunctions'ı bu kod dosyasında bir şekilde ele almalıyız. Dikkat edileceği üzere as anahtar kelimesi ile de bu dosya için bir takma ad (alias) oluşturduk. Bu sayede SomeFunctions içindeki metodlara erişebiliriz.

Kod aslında entersan bir ifade ile başlıyor. Burada [Bora Kaşmer](http://www.borakasmer.com/) hocamın aşağıdaki Tweet'inin etkili olduğunu ifade etmek isterim.

> [@burakselyum](https://twitter.com/burakselyum) 5*"Burak" enteresan bir code:) Ben ilk başladığımda çok şaşırmıştım😉
> 16 Haziran 2016

Sonrasında ise diğer metodların sırasıyla çağırıldığını görüyoruz. Eğer kodlarda bir hata yapmadıysanız (özellikle girintilerde) aşağıdakine benzer sonuçlar almanız gerekiyor.

![pythonhw_1.gif](/assets/images/2016/pythonhw_1.gif)

Tam çıktı ise aşağıdaki gibi.

Python 3.5.1 (default, Dec 2015, 13:05:11)
[GCC 4.8.2] on linux

BurkiBurkiBurkiBurkiBurki

hello python's world
My name is
Burak Selim Senyurt

so...What's your name? Charles
Wellcome Charles
How old are you? 39
Your age is 39
('burki', 1000, True, '1234-PLY')
('turp', 9003, False, -1, 'gold player', '4445-PLY')
('joiy', 1004, True, -10, 'silver player')
Please give me a country code EN

Your choice is English

...and the number list are

1
3
4
2
6
7
2
2
9
12
14
10
5
Second element of numbers is 3
The element count of numbers is 13
Please select a nickname Burk
Your nickname too short
Please select a nickname burkkkkkkkkkkkkkkkkkk
Your nickname too long
Please select a nickname buolurmu
Your nick name 'buolurmu'is cool now
x value: 4
y value: -9
pythagoras result is 9.848857801796104
Guess a number: 4
Too bad,Sorry
Guess a number: 3
Too bad,Sorry
Guess a number: 7
Vuhuu
Please insert a minimum value 1
Please insert a maximum value 5
1
2
3
4
5
0 0
0 1
0 2
0 3
0 4
1 0
1 1
1 2
1 3
1 4
2 0
2 1
2 2
2 3
2 4
3 0
3 1
3 2
3 3
3 4
4 0
4 1
4 2
4 3
4 4

Başka bir eksik kaldı mı diye düşünüyorum şu anda...Ah evet. # ile başlayan satırlar tahmin edeceğiniz üzere yorum satırlarıdır.

Bu yazıda merhaba dediğimiz pek çok kavram oldu aslına bakarsanız. if, while, for, Tuple, Dictionary, List, Range,def, metod ve değişken tanımlamaları, print, input, int, len, import, as ve unuttuğum diğerleri. Görüldüğü gibi Python dili oldukça esnek, söz dizimi yalın ve basit. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.