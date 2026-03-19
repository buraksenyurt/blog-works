---
layout: post
title: "Tek Fotoluk İpucu 135 - Ruby ile Tek Satırda Dosya içi Arama"
date: 2016-10-18 21:30:00 +0300
description: "Geçenlerde notpead++ ile oluşturduğum bir text dosya üzerinde düşünüyordum da...Dosya içerisinde House dizisinde çalan 75 adet şarkının bilgisi vardı. Söyleyenler ve şarkı adları. Sonra aklıma bu dosya içerisinde belli bir metnin geçtiği satırları nasıl bulabilirim sorusu geldi. Örneğin \"House dizisinde çalınan şarkılardan hangileri The Rolling Stones grubuna aittir?\" Mutlaka komut satırından bazı araçlar ile bu işlem kolayca gerçekleştirilebilir."
categories:
  - ruby
tags:
  - ruby
  - dotnet
  - linq
  - http
  - performance
  - visual-studio
---
# Tek Fotoluk İpucu 135 - Ruby ile Tek Satırda Dosya içi Arama
Merhaba Arkadaşlar,

Geçenlerde notpead++ ile oluşturduğum bir text dosya üzerinde düşünüyordum da...Dosya içerisinde House dizisinde çalan 75 adet şarkının bilgisi vardı. Söyleyenler ve şarkı adları.

![tfi135_1.gif](/assets/images/2016/tfi135_1.gif)

Sonra aklıma bu dosya içerisinde belli bir metnin geçtiği satırları nasıl bulabilirim sorusu geldi. Örneğin "House dizisinde çalınan şarkılardan hangileri The Rolling Stones grubuna aittir?" Mutlaka komut satırından bazı araçlar ile bu işlem kolayca gerçekleştirilebilir. Hatta hemen Visual Studio'yu açıp basit bir Console uygulaması da yazabilirim. Ancak ben bunu Interactive Ruby aracı üzerinde kodla nasıl yapabilirim peşindeydim. Çünkü hemen o anda ihtiyacım vardı bu bilgiye. Derlemeli değilde komut satırından çalışan yorumlamalı bir dil işime geliyordu. Aynı sonuca ulaşmanın birden fazla yolu olmakla birlikte istediğim bilgiyi tek satırlık bir kod parçası ile alabildiğimi görünce çok mutlu oldum. Epey hoşuma gitti o yüzden paylaşayım istedim.

```text
puts File.readlines('SomeAlbums.txt').select{|line| line['The Rolling Stones']}
```

ve sonuç

![tfi135_2.gif](/assets/images/2016/tfi135_2.gif)

Olayın kahramanları File sınıfı, parametre olarak gelen dosyayı satır bazında okumamızı sağlayan readlines metodu ve bu metodun döndürdüğü veri listesi üzerinde metin bazlı arama yapmamızı sağlayan select fonksiyonu. Sanki LINQ sorgusu yazar gibi. line['The Rolling Stones'] ifadesi ilgili satırı karakter dizisi bazında ele almamızı sağlıyor.

Bu teknik içerisinde Regex ifadesi de kullanabiliriz. Tabii çok büyük boyutlu dosyalarda bir şeyler aramak istediğimizde performans açısından daha farklı tekniklere yönelmemiz gerekebilir (Örneğin bir kaç gigabyte boyutundaki log dosyasında OutOfMemoryException geçen satırları aradığınızı düşünün) Büyük boyutlu dosyalar ile çalışmak başlı başına bir konu. [Şuradaki yazı size fikir verecektir kanaatindeyim](http://smyck.net/2011/02/12/parsing-large-logfiles-with-ruby/).

Bir başka ipucunda görüşmek dileğiyle hepinize mutlu günler dilerim.