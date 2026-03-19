---
layout: post
title: "Tek Fotoluk İpucu 80–Bir Assembly’ ın Public Key Token Değerini Bulmak"
date: 2013-02-18 02:00:00 +0300
description: "Merhaba Arkadaşlar,"
categories:
  - tek-fotoluk-ipucu
tags:
  - tek-fotoluk-ipucu
  - dotnet
---
# Tek Fotoluk İpucu 80–Bir Assembly’ ın Public Key Token Değerini Bulmak
Merhaba Arkadaşlar,

Diyelim ki elinizde projeye referans ettiğiniz bir.Net assembly dosyası bulunmakta. Örneğin Log4Net

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_153.png)

ve bununla birlikte konfigurasyon dosyası içerisinde de ilgili assembly’ ın versiyon numarasını ve daha da önemlisi Public Key Token değerini girmeniz gereken bir bölüm yer almakta. Söz konusu Assembly’ ın Public Key Token değerini öğrenmek için pratik olarak nasıl bir yol izlersiniz acaba?

Kodla elde etmek veya ILDASM aracı ile Metadata kısmında yer alan hexadecimal içeriğe bakmak, bir çözüm olabilir mi? Belki de SN (Strong Name Utility) isimli komut satırı aracı sizin işinizi görecektir. Nasıl mı? Buyrun

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_153.png)

[![tfi_80](/assets/images/2013/tfi_80_thumb.png)](/assets/images/2013/tfi_80.png)

Strong Name Utility’ yi hep assembly’ ları işaretlemek için (özellikle GAC’ a atacaklarımızı) kullanırdık değil mi? Bakın başka pratik kullanımları da varmış. Bir başka ip ucundan görüşmek dileğiyle

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_153.png)