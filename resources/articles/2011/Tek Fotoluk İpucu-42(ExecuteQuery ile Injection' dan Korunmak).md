---
title: "Tek Fotoluk İpucu-42(ExecuteQuery ile Injection' dan Korunmak)"
pubDate: 2011-11-26 03:15:00
categories:
  - C#
  - LINQ to SQL
  - Tek Fotoluk Ipucu
tags:
  - C#
  - LINQ to SQL
  - Tek Fotoluk Ipucu
  - LINQ
---

# Tek Fotoluk İpucu-42(ExecuteQuery ile Injection' dan Korunmak)
Merhaba Arkadaşlar,

LINQ to SQL kullandığımız durumlarda bildiğiniz gibi dışarıdan SQL sorgularını da icra ettirebilmekteyiz. Bu amaçla DataContext tipinin ExecuteQuery metodu kullanılmakta. Ancak özellikle SQL Injection saldırılarına karşı dikkatli olmamız gerekiyor. Bu nedenle söz konusu metodun placeholder kullanımına izin veren versiyonunu ele almamızda yarar olduğu kanısındayım. Nasıl mı?

![Winking smile](images/wlEmoticon-winkingsmile_59.png)

[![PhotoTrick42](images/PhotoTrick42_thumb.png)](images/PhotoTrick42.png)

[ExecuteQueryAndInjection.rar (52,04 kb)](assets/ExecuteQueryAndInjection.rar)
