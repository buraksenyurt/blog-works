---
layout: post
title: "Tek Fotoluk İpucu-35(DeflateStream ile Sıkıştırmak)"
date: 2011-10-21 08:13:00 +0300
description: "Merhaba Arkadaşlar,"
categories:
  - csharp
  - tek-fotoluk-ipucu
tags:
  - csharp
  - tek-fotoluk-ipucu
---
# Tek Fotoluk İpucu-35(DeflateStream ile Sıkıştırmak)
Merhaba Arkadaşlar,

Diyelim ki uygulama içerisinde kullandığınız büyük boyutlu bir byte dizisi var. Aslında bu diziyi bellek üzerinde sıkıştırarak daha az yer tutacak şekilde de kullanma şansınız olabilir. DelfateStream tipi bu anlmada işinize yarayacak Compress ve Decompress metodlarını içermektedir. İşte size örnek bir kullanım. Lorem Ipsum'u byte seviyesinde sıkıştırıyoruz. E decompress kısmı da size kaldı.

![Wink](/assets/images/2011/smiley-wink.gif)

[![PhotoTrick35](/assets/images/2011/PhotoTrick35_thumb.png)](/assets/images/2011/PhotoTrick35.png)

[MemoryCompressing.rar (25,22 kb)](/assets/files/2011/MemoryCompressing.rar)