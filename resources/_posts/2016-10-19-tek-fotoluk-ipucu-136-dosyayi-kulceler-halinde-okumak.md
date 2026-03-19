---
layout: post
title: "Tek Fotoluk İpucu 136 - Dosyayı Külçeler Halinde Okumak"
date: 2016-10-19 21:30:00 +0300
description: "Büyük boyutlu dosyalar neredeyse her programlama ortamının en büyük sorunlarındadır. Nitekim bu tip içeriklerin bir yerden bir yere taşınması, içeriklerinde arama yapılması ve benzeri senaryolarda oluşan sistemsel yükler söz konusudur. Tek işlemli süreçlerde sıkıntı olmasa da eş zamanlı olarak n sayıda dosya üzerinde toplu işlemler söz konusu olduğunda farklı tekniklerin uygulanması önerilmektedir."
categories:
  - ruby
tags:
  - ruby
  - concurrency
---
# Tek Fotoluk İpucu 136 - Dosyayı Külçeler Halinde Okumak
Merhaba Arkadaşlar,

Büyük boyutlu dosyalar neredeyse her programlama ortamının en büyük sorunlarındadır. Nitekim bu tip içeriklerin bir yerden bir yere taşınması, içeriklerinde arama yapılması ve benzeri senaryolarda oluşan sistemsel yükler söz konusudur. Tek işlemli süreçlerde sıkıntı olmasa da eş zamanlı olarak n sayıda dosya üzerinde toplu işlemler söz konusu olduğunda farklı tekniklerin uygulanması önerilmektedir.

Doğal olarak çok büyük boyutlu bir dosyayı tamamıyla belleğe açmaya çalışmak pek anlamlı değildir. Böyle hallerde genellikle dosyanın belli boyutlu parçalar (chunk diyelim) halinde okunması tercih edilir (Örneğin 4Kb lık boyutlarda parça parça okumak gibi) Ruby tarafında da bu iş aslında son derece basit. Örneğin binary içerikli aşağıdaki resim dosyasının belirli boyutlarda okunmasını istediğimizi düşünelim.

![einstien_3.jpg](/assets/images/2016/einstien_3.jpg)

Tek yapmamız gereken File sınıfına işleri kolaylaştıracak bir metod eklemekten ibaret. Nasıl mı? Aynen aşağıdaki ekran görüntüsünde olduğu gibi.

![tfi136.gif](/assets/images/2016/tfi136.gif)

readChunk metodunu Monkey Patching tekniğini kullanarak eklemiş bulunuyoruz. Aslında File sınıfını bu fonksiyon ile genişlettiğimizi ifade edebiliriz. readChunk metodu varsayılan olarak 4096 byte'lık bloklar halinde okuma yapacak şekilde tanımlanmış durumda. İçerisinde yield operatörünü kullanarak File sınıfının çalışma zamanında sahibi olduğu dosya üzerinde ileri yönlü okuma işlemini gerçekleştirmekteyiz. Bu okuma işlemi dosya sonuna kadar yapılmakta ki bunun için until eof? ifadesini kullanıyoruz. Kodun ilerleyen kısmında ise söz konusu operasyonu test etmekteyiz. open metoduna yapılan çağrı sonucu f ile ifade edilen nesne üzerinden readChunk metodu çağırılabilir. Örnekte 1024 byte'lık okumalar gerçekleştirmekteyiz.

Pek tabii bu teknik bir servis metodunun parçası olabilir. Söz konusu servis büyük boyutlu binary içerikleri karşı tarafa parçalar halinde verebilir. Büyük boyutlu binary içeriklerde yapmak istediğimiz işlemlerde ele alabileceğimiz basit bir kod. Tekrar görüşünceye dek hepinize mutlu günler dilerim.