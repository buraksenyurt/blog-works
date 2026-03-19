---
title: "Tek Fotoluk ipucu - 58 Derived Tipler için XElement Converter"
pubDate: 2012-07-11 21:15:00
categories:
  - Tek Fotoluk Ipucu
tags:
  - Tek Fotoluk Ipucu
  - .NET
  - Oracle
---

# Tek Fotoluk ipucu - 58 Derived Tipler için XElement Converter
Merhaba Arkadaşlar,

Farz edelim ki elimizden tonlarca POCO (Plain Old CLR Object) tip var. Hatta laf aramızda tonlarca otomatik olarak üretilmiş SQL User Defined Type karşılığı sınıf var. İstiyorsunuz ki, bu tiplerin çalışma zamanındaki canlı örnekleri, XElement tipine dönüştürülebilsin. Hatta XElement içerisinde özelliklerin adları ile birlikte.Net tarafında ki type bilgileri de detaylı olarak bulunsun. Bulunsun ki başka bir yerden tekrar ayağa kaldırabilelim. Her tip için birer Extension method'mu yazarsınız? Ya da tüm tipler için Convert ile ilişkili bir Interface implementasyonu mu? Belki de bu tipler size Oracle'dan gelmiştir de türedikleri bir base type'da vardır

![Wink](images/smiley-wink.gif)

İşte size fikir verecek bir ipucu daha.

![TPI_59_1.png](images/TPI_59_1.png)

Debug time görüntüsü ise,

![TPI_59_2.png](images/TPI_59_2.png)

Başka bir ipucundan görüşmek dileğiyle

![Smile](images/smiley-smile.gif)
