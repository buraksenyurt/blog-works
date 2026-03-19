---
title: "Tek Fotoluk İpucu 141 - Dosyadan Rastgele Satır Çekmek"
pubDate: 2016-11-01 21:36:00
categories:
  - Ruby
tags:
  - Ruby
  - Threading
  - Performance
---

# Tek Fotoluk İpucu 141 - Dosyadan Rastgele Satır Çekmek
Merhaba Arkadaşlar,

Elinizde N sayıda şarkı adının kayıtlı olduğu fiziki bir dosya olduğunu düşünün. Amacımız ise bu dosya içerisinden rastgele şarkılar çekmek. Ancak bunu yaparken ilgili dosyanın tamamını belleğe açmak istemiyorsunuz. Nitekim dosyayı okuma modunda açıp readlines gibi bir metod ile tüm içeriğ okuduktan sonra içinden rastgele bir satırı seçme yolunu tercih edebilirsiniz. Ama bu büyük boyutlu bir dosyanın tamamen belleğe yüklenmesine de neden olacaktır. Performans ve hız açısından farklı bir şey yapılabilmelidir. Örneğin belleğe sadece o anki satırı okuyup ileri yönlü hareket edecek bir iterasyon kodu geliştirilebilir. Peki Ruby'de bunun için nasıl bir yol izlerdiniz? Aşağıdaki fotoğraftaki gibi olabilir mi?

![tfi_141.gif](images/tfi_141.gif)

Enumerable modülü içerisine getRandomLine isimli bir metod yerleştirdik. Metod aslında numaralandırıcı olarak gelen listeyi dolaşmakta. Bunun için each_with_index fonksiyonundan yararlanıyoruz. each_with_index fonksiyonu iki parametre alıyor. İlki numaralandırıcının o an üzerinde olduğu veri (ki örneğimizde bu dosya satırı olacak), ikincisi ise index numaras (ki bu da hangi satırda olduğumuzu gösterecek) Blok içerisinde rastgele sayı üretip bir kontrol gerçekleştiriyor ve o anki satırın bu koşula uyması halinde line değişkenine atanmasını ve son olarak nil değilse geri döndürülmesini sağlıyoruz.

Örnek metin dosyasını açtıktan sonra getRandomLine metodunu üzerinde uygulayabiliriz çünkü satır bazında ileri yönlü itere edilebilir bir nesne örneği vermektedir. Dikkat edilmesi gereken nokta ise rewind çağrısıdır. Nitekim satır bulunurken mutlak olarak dosya sonuna gelinmektedir. Yani her satır okunur. Bu yüzden tekrar başa dönmezsek sadece ilk seferde şarkı seçimi gerçekleşir ve sonraki iterasyonlarda nil değerler alınır (Şarkıları rahatça görebilmek için program Thread'ini 2 saniye süreyle uyutuyoruz ki konumuzla çok alakalı değil)

Bu tekniği içeriğinde farklı veriler tutan pek çok dosya için uygulayabilirsiniz. Örneğin kullanıcılara rastgele promosyon uygulanacağı durumlarda verinin fiziki dosyada saklanması söz konusu ise bu tip bir metod oldukça işe yarayacaktır. Böylece geldik bir ipucunun daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
