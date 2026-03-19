---
layout: post
title: "Tek Fotoluk İpucu 62–Byte Array için Sıkıştırma"
date: 2012-07-26 09:00:00 +0300
description: "Merhaba Arkadaşlar,"
categories:
  - tek-fotoluk-ipucu
tags:
  - tek-fotoluk-ipucu
---
# Tek Fotoluk İpucu 62–Byte Array için Sıkıştırma
Merhaba Arkadaşlar,

Kod içerisinde bir yerlerde öyle ya da böyle elde ettiğiniz ama boyutu azcık da olsa küçülebilse dediğiniz byte tipinden array’ ler olduğunu düşünün. Kimi zaman bir dosyanın içeriği olabileceği gibi, sistem içerisinde üretilmiş bir byte dizisi bile olabilir bu. Peki söz konusu içeriği var olan GZip veya Deflate algoritmalarına göre sıkıştırmak isterseniz

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_101.png)

Aşağıdaki gibi bir Extension Method eminim ki işinize yarayacaktır.

[![spt_62](/assets/images/2012/spt_62_thumb.png)](/assets/images/2012/spt_62.png)

Bir başka ipucunda görüşmek dileğiyle.