---
layout: post
title: "Tek Fotoluk İpucu 158 - GoLang'de Constructor var mı?"
date: 2017-05-16 18:17:00 +0300
categories:
  - golang
tags:
  - golang
  - go
---
GO tam anlamıyla nesne yönelimli (Object Oriented) bir dil değildir. Hatta object terimi yerine Type kavramının daha çok öne çıktığı bir programlama dilidir. Geliştirici tanımlı tipler için struct'lardan yararlanılır ve onların örneklenmesinde kullanılabilecekk doğal yapıcı metodlar (built-in constructor) vardır. Yine de istersek kendi yapıcı metodlarımızı yazabiliriz. Nasıl mı? Aynen aşağıdaki fotoğrafta olduğu gibi.

![tfi158.gif](/assets/images/2017/tfi158.gif)

Az da olsa birazcık hile var gibi değil mi? Product isimli struct tipinin built-in constructor ile nasıl üretildiğini 10ncu satırda görebiliriz. car isimli değişken dinamik olarak türlendirilmiş ve:= operatörü sonrasında gelen ifade içerisinde tip niteliklerine ilk değerleri verilmiştir. Bu zaten GO'nun sunduğu varsayılan yapıcıdır. Biz hafiften abstract factory design pattern benzeri bir çözüm uyguladık. NewProduct metodu parametre olarak aldığı bilgilere göre GO'nun built-in yapıcı metod özelliğini kullanarak yeni bir ürün tipini geriye döndürmektedir. Bir nevi kendi yapıcı metodumuzu yazmış olduğumuzu ifade edebiliriz. Basit ama bir Object Oriented programcısı için tuhaf. Bir başka ipucunda görüşmek üzere hepinize mutlu günler dilerim.
