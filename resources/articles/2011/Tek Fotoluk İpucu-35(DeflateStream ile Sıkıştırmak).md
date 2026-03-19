---
title: "Tek Fotoluk İpucu-35(DeflateStream ile Sıkıştırmak)"
pubDate: 2011-10-21 08:13:00
categories:
  - C#
  - Tek Fotoluk Ipucu
tags:
  - C#
  - Tek Fotoluk Ipucu
---

# Tek Fotoluk İpucu-35(DeflateStream ile Sıkıştırmak)
Merhaba Arkadaşlar,

Diyelim ki uygulama içerisinde kullandığınız büyük boyutlu bir byte dizisi var. Aslında bu diziyi bellek üzerinde sıkıştırarak daha az yer tutacak şekilde de kullanma şansınız olabilir. DelfateStream tipi bu anlmada işinize yarayacak Compress ve Decompress metodlarını içermektedir. İşte size örnek bir kullanım. Lorem Ipsum'u byte seviyesinde sıkıştırıyoruz. E decompress kısmı da size kaldı.

![Wink](images/smiley-wink.gif)

[![PhotoTrick35](images/PhotoTrick35_thumb.png)](images/PhotoTrick35.png)

[MemoryCompressing.rar (25,22 kb)](assets/MemoryCompressing.rar)
