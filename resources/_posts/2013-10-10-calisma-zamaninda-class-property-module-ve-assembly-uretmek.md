---
layout: post
title: "Çalışma Zamanında Class,Property, Module ve Assembly Üretmek"
date: 2013-10-10 07:50:00 +0300
description: "Merhaba Arkadaşlar,"
categories:
  - csharp
tags:
  - csharp
---
# Çalışma Zamanında Class,Property, Module ve Assembly Üretmek
Merhaba Arkadaşlar,

Şöyle bir senaryo düşünelim; Bir Excel dosyasında yer alan sayfa ve kolon bilgilerini programatik ortamda ifade etmek istiyoruz. Ancak Excel dosyası oldukça büyük. Sheet ve kolon sayıları çok fazla. Bir şekilde dosyayı okumayı, kolon adlarını, içeriklerini ve veri tiplerini öğrenmeyi başarıyoruz. Her bir Sheet'in bir sınıfa karşılık gelmesi gerektiğini fark ediyoruz. Ama işin zor olan kısmı şemaya uygun şekilde sınıf ve özelliklerinin programatik ortamda üretilmesi. Aklımıza gelen bir kaç yol var fakat biz en şık olanlarından birisini tercih ediyoruz. Sınıfları (Class), özelliklerini (Property) ve bu tiplerin bulunduğu Assembly’ ı içeren sınıf kütüphanesini (Class Library) kod ile üretiyoruz. Üretme işini gerçekleştirirken Intermediate Language’ e kadar da uzanıyoruz. Merak ediyor musunuz?

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_213.png)

[DynamicAssembly.rar (31,37 kb)](/assets/files/2013/DynamicAssembly.rar)