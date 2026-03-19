---
title: "Matematik ve Oyun Programlama - Missile Command - Bölüm 2"
pubDate: 2022-11-25 12:15:00
categories:
  - Rust
tags:
  - Rust
  - GitHub
---

# Matematik ve Oyun Programlama - Missile Command - Bölüm 2
Matematiği bana sevdiren lise birinci sınıftaki öğretmenimdi. Fizik, Kimya ve Türkçe derslerinde o kadar iyi değildim ama konu matematik olunca pür dikkat kesilirdim. Ancak üniversite yıllarına geldiğimde Matematik Mühendisliğinin o yoğun teorik programı içerisinde kaybolurken nefes alacak yer bulmakta zorlanıyordum. İmdadıma yine bölüm müfredatında yer alan programlama dersleri yetişmişti (ve halen Fizik dersinde başarısızdım:D) Programlama dillerine olan tutkum beni bir yazılımcı olmaya ve bugünlere kadar gelmeye ikna etti. Ancak bazen durup bir düşünüyorum. Yeterince farkında olsaydım acaba oyun geliştiricisi olmak ister miydim? Çünkü hem matematiği hem de programlamayı bir arada bulabileceğim en güzel alandı. Tabii evdeki hesap çarşıya uymamıştı. O zamanlarda ne programlamada ne de matematiğin oyun geliştirme dünyasındeki yeri anlamında yeterli değildim. Üstelik oyun geliştiriciliği alanına hitap eden bir bölümde de okumuyordum. Yine de "çok geç değil, nasıl olsa hobi amaçlı ilgilenebilirim" dedim ve işte bu seri böylece başlamış oldu. Konuşmak için çok erken ve daha yolun başındayım ama öğrendikçe ve fırsat buldukça meraklılarına aktarmaya çalışacağım.

Bir önceki programımızda Atari'nin efsane oyunlardan Missile Command'in arkasındaki matematik enstrümanları [incelemeye başlamıştık](Matematik ve Oyun Programlama - Missile Command - Bölüm 1.md). İkinci bölümde ise zeminin orta noktasına ve şehrin biraz yukarsına füze bataryamızı yerleştiriyoruz. Bu füze bataryası, oyuncu mouse imlecini ekranda hareket ettirdikçe oraya doğru dönebilen bir çizgiden ibaret esasında. Oyuncu mouse imlecinin olduğu yerde sol tuşa tıklarsa da bataryanın namlu ucundan bu noktaya doğru sevimli ve mavi renkte minik bir dörtgen fırlamakta. Herhangi bir zamanda sahnede sadece üç tane mermi bulunabilir. Hatta füze bataryası 30 derecenin altına veya 150 derecenin üstüne hareket edemediği gibi ateş de edemez. Diğer yandan şehre inmekte olan füzelerden birazcık daha hızlı hareket eden mermiler mouse ile tıklanan noktalara vardıklarında sahneden kaldırılmaktalar.

İşte tüm bu bahsettiklerimizin arkasında matematiğin ve özellikle lineer cebirin temel enstrümanları bulunuyor. Önceki programda olduğu gibi bu yayında da vektörlerden, birim vektörden, vektörler arası açı hesaplamasından, kosinüs ve sinüs fonksiyonları ile pisagor teoreminden yararlanıyoruz. Kabaca aşağıdaki şekilde görülen bir karmaşayı dilim döndüğünce anlatmaya çalışıyorum diyebilirim:)

![image.axd](images/image.axd)

Konu ile ilgili örneği Rust programlama dilini kullanarak geliştiriyorum. [Github hesabımdan ilgili kaynak kodlara ulaşabilirsiniz](https://github.com/buraksenyurt/game-dev-with-rust/tree/main/missile-commander). Oyun motoru olarak hafifsiklet olanlardan [Macroquad](https://macroquad.rs/examples/)'ı tercih ettim. Bu arada ders çekimi sırasında birkaç kez dilim sürçtü ama kusurlarıyla birlikte yararlı bir anlatım oldu diye düşünüyorum.

[Youtube Link](https://www.youtube.com/watch?v=Mu8xDsfI2Po)

Sonraki bölümde amacımız, merminin kendi varış noktasına geldiğinde belli bir yarıçap değerine kadar büyüyen bir çember çizdirmek ve bu çember ile temas eden roketlerin yok edilmesini sağlamak. Tabii bunun arkasındaki matematik enstrümanlara odaklanacağız. Üçüncü bölümde görüşünceye dek hepinize mutlu günler dilerim.
