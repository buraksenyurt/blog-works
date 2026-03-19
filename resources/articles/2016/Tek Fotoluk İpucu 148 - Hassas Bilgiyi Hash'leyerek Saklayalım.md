---
title: "Tek Fotoluk İpucu 148 - Hassas Bilgiyi Hash'leyerek Saklayalım"
pubDate: 2016-12-15 21:54:00
categories:
  - Tek Fotoluk Ipucu
tags:
  - Tek Fotoluk Ipucu
---

# Tek Fotoluk İpucu 148 - Hassas Bilgiyi Hash'leyerek Saklayalım
Merhaba Arkadaşlar,

Yazdığımız bir uygulamada kullanıcının anne kızlık soyadı, kimlik numarası, şifre ve benzeri bilgilerini aldığımızı düşünelim. Bir arayüz üzerinden giriliyor olabilirler. Bu bilgileri herhangibir amaçla veritabanında sakladığımızı varsayalım. Şirket güvenlik politikları gereği ilgili bilgiler açık bir şekilde tutulmamalı. Yani anne kızlık soyadı, şifre veya kimlik numarası gözle okunabilir halde tutulmamalı. Buna göre ilgili bilgileri veritabanı üzerinde maskeleyerek saklamak doğru bir çözüm olabilir. Bunu yapabilmek için akla gelen yollardan birisi de tahmin edileceği üzere Hash algoritmalarına başvurmaktır. Peki güçlü bir Hash algoritması ile bu maskeleme işlemini yapmak ister misiniz? Aşağıdaki fotoğraf size yol gösterebilir.

![tfi148.gif](images/tfi148.gif)

RNGCryptoServiceProvider sınıfı bir salt içeriği oluşturulmasında görev alıyor. Bu içerik sonraki aşamada verinin maskelenmesi sırasında kullanılıyor. Salt hash çıktısının benzersiz ve tahmin edilemez olması noktasında önem arz etmekte. Örnekte 48 byte'lık bir salt verisi oluşturuldu ama bu şart değil. Farklı boyutlarda salt içerikleri üretilebilir. Oluşturulan salt Rfc2898DerivedBytes sınıfının yapıcı metoduna parametre olarak geçiliyor. İlk parametrede ise maskelenecek olan içeriğimiz var ki bu örnekte Anne Kızlık Soyadı bilgisini ele alıyoruz. İkinci parametre ise salt değeri. IterationCount özelliğine atanan değer karmaşıklığı arttırmak için veriliyor. İterasyon sayısı fazlalaştıkça algoritma biraz daha karmaşık çalışıyor ve tahmin edilebilirlik ihtimalini düşürüyor diyebiliriz. Maskelenen içeriği yine GetBytes metodu yardımıyla elde ediyoruz. Sonrasında bunu ToBase64String metodu ile string formatta ekrana yazdırmaktayız. Elbette gerçek dünya senaryosunda maskelenen bu string içeriği veritabanına kayıt etmeniz gerekiyor.

Peki ya sonrasında...Müşteri uygulamaya tekrardan giriş yapıp Anne Kızlık Soyadı bilgisini girdiğinde aynı süreç işleyip üretilen maskelenmiş verinin veritabanındaki versiyonu ile karşılaştırması yeterli. Bu iki içerik aynı olduğu takdirde doğru bilgi girmiş olduğunu düşünerek sürecin ilerletilmesini sağlayabiliriz. Ayrıca anne kızlık soyadı sistemin hiç bir noktasında gözle okunabilir formatta dolaşmamış da olur.

> Tabii ekranda bu bilgilerin girildiği kutucukların ve ağ üzerinde bu bilgilerin aktığı hattın güvenliği bambaşka bir konu. Burada servis mesaj içeriklerinin şifrelenerek kullanılması ya da https gibi protokollerin tercih edilmesi doğru bir yaklaşım olabilir. Nitekim hattın güvenli olması ve dinleyen yabancı uygulamaların bu içerikleri görememesi çok önemlidir.

Tabii örnek sadece nasıl kullanılırı gösteriyor. İlgili fonksiyonelliğin kütüphaneleştirilmesi çok daha doğru olacaktır. Bu görevi siz değerli okurlarıma bırakıyorum. Bir başka tek fotoluk ipucunda görüşmek dileğiyle hepinize mutlu günler dilerim.
