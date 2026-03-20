---
layout: post
title: "TFİ 110 - Control Tipini Extend Edip Basit Bind İşlemleri Gerçekleştirmek"
date: 2014-10-08 21:07:00 +0300
categories:
  - tek-fotoluk-ipucu
tags:
  - tek-fotoluk-ipucu
  - windows-forms
  - reflection
  - generics
---
Diyelim ki, geliştirdiğiniz Windows Forms tabanlı ekranlarınızdaki kontrollerin içerikleri farklı bir veri kaynağından (Strongly Typed özellikte) geliyor ve siz bunları kontrollerin ilgili özelliklerine bağlayacak generic özellikte bir metod geliştirmek istiyorsunuz. Doğrudan özelliklere değer set etmek ile uğraşabilirsiniz de ama, veri kaynağından okuma yapan kod parçasının içerisinde bu işlemi merkezileştirmeyi de düşünüyorsunuz. Ancak ilk adım olarak Control türevli tipler için bir Extension metod üzerinden özelliklere değer bağlama işlemlerini yapmak istiyorsunuz. Nasıl bir yol izlersiniz? Aşağıdaki ip ucu işinize yarayabilir mi?

Peki aynı felsefeyi Web uygulamalarınız için tasarlayabilir misiniz? Hatta reflection hamleleri kokan bu kod parçasında dynmaic kullanabilir miyiz?

![tfi_110.png](/assets/images/2014/tfi_110.png)

Bir başka ip ucunda görüşmek dileğiyle.