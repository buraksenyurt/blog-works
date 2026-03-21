---
layout: post
title: "Matematik ve Oyun Programlama - Missile Command - Bölüm 1"
date: 2022-11-18 17:00:00 +0300
categories:
  - rust
tags:
  - rust
  - rust-lang
  - matematik
  - oyun-matematiği
  - oyun-programlama
  - vektörler
  - açılar
  - macroquad
---
Uzun bir süredir Rust programlama dili ile hobi amaçlı uğraşıyorum. Son birkaç aydırda Rust tarafında kullanılan oyun motorlarını kurcalamaktayım. Ancak birkaç haftadır amacım oyun programlamada kullanılan temel matematik enstrümanları öğrenmek. Bana göre bu alanda ilerleyebilmemin en iyi yolu bilinen oyunların birer klonunu yazmaya çalışmak. Onca vektör, açı, nokta çarpım problemini işledikten sonra ise ilk gözüme kestirdiğim zamanın efsane Atari oyunlarından olan[Missile Command](https://en.wikipedia.org/wiki/Missile_Command).

Kaynaklara göre seksenli yılların en kült oyunlarından birisi olarak ifade ediliyor. Oyunda ekranın üst kısmından rastgele açılarda şehre doğru inen füzeleri görüyoruz. Oyuncu, üssün tam orta yerinde duran füze rampasından ateş ederek şehre inen füzeleri önlemeye çalışıyor. Kaçan füzeler şehre düşünce de hasar veriyor. Füzelerden kurtuldukça farklı seviyelere geçiş yapıyorsunuz. Görüntü tamamen piksel hareketlerinden oluşmakta ve benim asıl ilgilendiğim oyunun arkasındaki matematik hesaplamalar.

İlk etapta ekranın üst tarafındaki rastgele konumlardan, belli gecikmelerle (bazen de aynı anda) farklı ya da sabit açılarda hareket eden füzelerin nasıl olup da bu şekilde ilerlediğini keşfetmek istiyorum. Ekranın genişliğini düşünerek x ekseni üstünde rastgele konumlar üretmek oldukça kolay esasında. Peki açıyı nasıl ayarlayacağız? Hatta açıyı öyle bir belirlemeliyiz ki füzeler ekranın solundan veya sağından dışarıya çıkmasınlar. Füzeleri hallettikten sonra pek tabii oyuncunun da ekran üzerinde mouse ile tıkladığı noktalara yine belli bir açıda ve hızda ateş etmemiz gerekiyor. Konuyu araştırırken vektörler arasındaki açı hesaplamasından yararlanabileceğimi anladım. Tabii aynı sonuca ulaşmak için farklı matematik yöntemler de kullanılabilir pekala. Bunlar yoruma açık ve sizlerin desteği ile hep birlikte daha da iyi öğrenebiliriz.

Konu ile ilgili örneği Rust programlama dilini kullanarak geliştiriyorum. [Github hesabımdan ilgili kaynak kodlara ulaşabilirsiniz](https://github.com/buraksenyurt/game-dev-with-rust/tree/main/missile-commander). Oyun motoru olarak hafifsiklet olanlardan [Macroquad](https://macroquad.rs/examples/)'ı tercih ettim. Örneği iki bölüm halinde incelemeyi planlıyorum ve işte ilki. Keyifli seyirler.

[Youtube Link](https://www.youtube.com/watch?v=I5AonLlBizo)

İkinci bölümde savunma sistemini ve mouse ile tıklanan yere ateş etmeyi ele almaya çalışacağım. Tabii buna patlama efektini de eklemem gerekiyor. İçerden dışa doğru çapı genişleyen ve sonra ortadan kaybolan bir çember pekala işimi görebilir. Bakalım yol boyunca nelerle karşılaşacağım (karşılaşacağız). Tekrardan görüşünceye dek hepinize mutlu günler dilerim.