---
layout: post
title: "Python - PEP8 Uyumlu Kod Geliştirmek"
date: 2017-06-27 17:10:00 +0300
description: "Geçtiğimiz günlerde senelik kişisel gelişim döngümün ikinci yarısının ilk konusu olan Python'a başladım. Yine bir önceki yılın aynı dönemlerinde Raspberry Pi ile ilgili olarak Python üzerine bir şeyler yapmaya çalışmıştım. Döngünün bir sonraki adımında yine Python ile başbaşayım. Head First Python, A Brain Friendly Guilde isimli kitabı takip ediyorum. Her örneği tek tek yapmaya çalışıyorum. Bayram tatilinde ilgimi çeken bir konu ile karşılaştım. Yazdığımız python kodlarının PEP8(Python Enhancement Proposals) adı verilen standartlara uygun olup olmadığının tespiti."
categories:
  - python
tags:
  - python
  - bash
  - http
  - go
  - ruby
---
# Python - PEP8 Uyumlu Kod Geliştirmek
![pythonpep_2.gif](/assets/images/2017/pythonpep_2.gif)

Merhaba Arkadaşlar,

Geçtiğimiz günlerde senelik kişisel gelişim döngümün ikinci yarısının ilk konusu olan Python'a tekrardan başladım. Bir önceki yılın aynı dönemlerinde Raspberry Pi ile ilgili olarak Python üzerine bir şeyler yapmaya çalışmıştım. Ruby ve Go ile devam eden iterasyonun sıradaki adımında Python'u bir başucu kitabını kullanarak tekrar etmekteyim. Bu amaçla [Head First Python, A Brain Friendly Guilde](http://shop.oreilly.com/product/0636920036777.do) isimli kitaptan yararlanıyorum. Her örneği tek tek yapmaya çalışıyorum. Bugün ilgimi çeken bir konu ile karşılaştım. Yazdığımız python kodlarının PEP8 (Python Enhancement Proposals) adı verilen standartlara uygun olup olmadığının tespiti. PEP konusu ile ilgili detaylı bilgilere [şu adresten](https://www.python.org/dev/peps/) ulaşabilirsiniz. Hatta bu kısa yazıya konu olan PEP8 içeriğine de [bu adresten](https://www.python.org/dev/peps/pep-0008/) bakabilirsiniz.

Tahminlerime göre bloğumu okuyan hemen herkes yazılım geliştiriyor ve bu işte kodlama standartlarının ne kadar önemli olduğunu da biliyor. Bu standartlarda değişken isimlendirmelerinden yorum satırlarının nasıl olması gerektiğine kadar pek çok yazım stili önerisi de bulunuyor. Bu öneriler kodun daha okunabilir ve diğer meslektaşlarımız ile aynı stilde içerik üretilmesi açısından mühim. Python dili içinde kodun standard kütüphanenin yazım stiline uygun olup olmadığının kontrolünü yapabileceğimiz bir kılavuz mevcut. Gerçi içerik olarak bakıldığında daha çok yazım stilinin ön plana çıktığı ve bu noktada bir tutarlılığın sağlanmaya çalışıldığı anlaşılabiliyor. Gelin Python ile yazdığımız kodların (örneğin basit bir modül içeriğinin) PEP8 standartlarına uygunuluğunu kontrol edelim.

Gerekli Araçlar

Öncelikle bu kontrol işlerini üstlenecek yardımcı araçları çalışmakta olduğumuz sistemimize kurmamız gerekiyor. pytest isimli araçla pep8 standartlarını içeren paketleri sistemimize almalıyız. Örnekleri Windows tarafında geliştirdiğimden pip yükleyicisini py komutu ile birlikte kullanmalıyım. -3 ile Python'un 3 ve sonrası sürümü için bir yükleme işlemi yapılmasını belirtiyoruz. Sırasıyla önce pytest ardından da pytest-pep8 araçlarını kuruyoruz.

```bash
py -3 -m pip install pytest

py -3 -m pip install pytest-pep8
```

![pythonpep_1.gif](/assets/images/2017/pythonpep_1.gif)

> Windows tarafındaki yükleme sonrası py.test aracının bulunamadığına dair bir hata mesajı ile karşılaşabilirsiniz. Bu, Python'un script klasörüne ait Path bildiriminin olmayışından kaynaklanmaktadır. Script klasörünü Environment Variables-Path tanımına eklediğiniz takdirde sorun çözümlenecektir.

Bir Test Kodu

Pek tabii test için örnek bir kod parçasına ihtiyacımız var. İçeriğinin çok büyük bir önemi yok. Bizim için kobay olacak diyebiliriz.

```text
import math,os

# Bu fonksiyon iki sayısal değerin toplamını hesaplamak için kullanılır. İki int gibi.
def sum(x,y):
    return x+y

# Variadic function sample
def sum(*numbers):
    if not len(numbers)>0:
        return 0
    total=0
    for n in numbers:
        total+=n
    return total

# Calculate a Circle space
def circleSpace(r):
    return math.pi*r*r

class player:
   def __init__(self, name, level):
      self.name = name
      self.level = level
   
   def display(self):
      print("Name : ", self.name,  ", Level: ", self.level)

# test scripts
print(sum(2,3))
print(sum(1,3,5,7,9,11))
print(circleSpace(10))
throll=player("Throll",22)
throll.display()
```

Aslında üç basit fonksiyonumuz ve bir de sınıfımız var. sum isimli fonksiyonun aşırı yüklenmiş iki versiyonu bulunuyor. Birisi iki değerin toplamını almakta iken diğeri Variadic özelliğe sahip. Bir başka deyişle n sayının toplamını hesaplatabiliriz. Son fonksiyonumuz ise math modülündeki pi sabitini kullanarak daire alanının hesaplanmasında kullanılıyor. Her ne kadar kullanmıyor olsak da iki modül bildirimi var. math ve os (Eğer GoLang tarafında olsaydık derleme hatası alırdık biliyorsunuz değil mi? "Kullanmadığın paketi niye koyuyorsun" oraya derdi) player isimli sınıfımız bir oyuncuyu temsilen yazılmış durumda. Kodların IDLE üzerinden F5 ile test edersek çalıştığını görebiliriz.

![pythonpep_4.gif](/assets/images/2017/pythonpep_4.gif)

Peki ya PEP8 Kontrolü

Kodlar güzel bir şekilde çalışıyor peki yazım tarzı Guido van Rossum, Barry Warsaw, Nick Coghlan abilerimizin istediği gibi mi? Nitekim PEP8 dokümantasyonunda onların imzası var. Haydi bir bakalım. Komut satırından py.test aracını kullanarak testi çalıştırdığımızda beklenmedik sonuçlarla karşılaşabiliriz.

```bash
py.test --pep8 algebra.py
```

![pythonpep_5.gif](/assets/images/2017/pythonpep_5.gif)

Liste aşağıya doğru uzayıp gitmekte. Temel olarak yazım stili ile ilgili kızılan şeyler var. Örneğin math ve os paket bildirimlerinin aynı satırda olmasına kızılıyor. Fonksiyon bildirimlerinden önce iki boş satır bekleniyor. Operatorlerden önce ve sonra birer boşluk isteniyor. Bir satırdaki karakter sayısının çok fazla olduğu ifade ediliyor (72 harfi geçen bir yorum satırımız var) Sınıfın içeriğini diğer bir dosyadan kopyalarken girintilerde kaymalar olduğu için 4 boşluklu tab kuralının bozulduğu dile getiriliyor (Aslında PEP8 dokümanına bakıldığında sınıf adları veya if kullanımları ile ilgili öneriler de var. Lakin test aracından bunları çıkarttıramadım) Şimdi kodun yazım stilini belirtilen uyarı mesajlarına göre yeniden düzenleyelim.

```text
import math
import os

# Bu fonksiyon iki sayısal değerin toplamını hesaplamak için kullanılır.
# İki int gibi.
def sum(x, y):
    return x+y

# Variadic function sample
def sum(*numbers):
    if not len(numbers) > 0:
        return 0
    total = 0
    for n in numbers:
        total += n
    return total

# Calculate a Circle space
def circleSpace(r):
    return math.pi*r*r

class player:
    def __init__(self, name, level):
        self.name = name
        self.level = level

    def display(self):
        print("Name : ", self.name,  ", Level: ", self.level)

# test scripts
print(sum(2, 3))
print(sum(1, 3, 5, 7, 9, 11))
print(circleSpace(10))
throll = player("Throll", 22)
throll.display()
```

import bildirimlerini iki ayrı satıra koydum. 72 karakteri geçen yorum satırını da iki satıra böldüm. Fonksiyoların öncesinde ikişer satır boşluk bıraktım. Eşitlik, büyüktür gibi operatörlerin önüne ve arkasına birer boşluk dahil ettim. Fonksiyon parametrelerinde ve çağırımlarındaki virgüllerden sonrasına da birer boşluk koydum. Sınıfın içerisindeki bozulan girinti yapısını tekrar elden geçirdim. Sonuç şöyle oldu.

![pythonpep_6.gif](/assets/images/2017/pythonpep_6.gif)

Görüldüğü gibi test başarılı. Elbette yapılan test sadece kodun yazım stilini denetleyen ve standart python kütüphanesindeki ile tutarlı hale gelinmesini sağlayan türde. Sonuçta PEP8 ile tutarlılık sağlandı diyebiliriz. Lakin test aracı bir metodun parametre sayısının belli bir değerin üstünde olması haline sınıf kullanın gibisinden bir öneri sunmadı. İnsan Juval Lowy'nin eşsiz C# kodlama standartlarındaki gibi bir şeyler bekliyor ancak konu daha çok göze hoş gelen yazım stili gibi duruyor. Dokümantasyona bakıldığında yine de güzel öneriler var. Şahsen GoLang tarafındaki bir takım stil kurallarının build mekanizmasına dahil edilmesinin çok daha iyi olduğu kanaatindeyim. Nitekim bazı stillere uyulmadığı takdirde (örneğin fonksiyon süslü açma parantezinin alt satırda olması hali) derleme hatası fırlatabilen, pek çoğumuza katı görünen ama standartlığı en baştan sağlayan bir derleme mekanizması mevcut. Bu öz eleştirimi de yaptıktan sonra huzurlarınızdan ayrılıyorum. Python kitabıma devam etmem lazım. Gitmem gereken yaklaşık 700 sayfa daha var. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.