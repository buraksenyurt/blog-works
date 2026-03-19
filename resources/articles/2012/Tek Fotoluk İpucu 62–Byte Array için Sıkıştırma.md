---
title: "Tek Fotoluk İpucu 62–Byte Array için Sıkıştırma"
pubDate: 2012-07-26 09:00:00
categories:
  - Tek Fotoluk Ipucu
tags:
  - Tek Fotoluk Ipucu
---

# Tek Fotoluk İpucu 62–Byte Array için Sıkıştırma
Merhaba Arkadaşlar,

Kod içerisinde bir yerlerde öyle ya da böyle elde ettiğiniz ama boyutu azcık da olsa küçülebilse dediğiniz byte tipinden array’ ler olduğunu düşünün. Kimi zaman bir dosyanın içeriği olabileceği gibi, sistem içerisinde üretilmiş bir byte dizisi bile olabilir bu. Peki söz konusu içeriği var olan GZip veya Deflate algoritmalarına göre sıkıştırmak isterseniz

![Winking smile](images/wlEmoticon-winkingsmile_101.png)

Aşağıdaki gibi bir Extension Method eminim ki işinize yarayacaktır.

[![spt_62](images/spt_62_thumb.png)](images/spt_62.png)

Bir başka ipucunda görüşmek dileğiyle.
