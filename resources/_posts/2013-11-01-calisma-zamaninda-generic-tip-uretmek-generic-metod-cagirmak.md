---
layout: post
title: "Çalışma Zamanında Generic Tip Üretmek, Generic Metod Çağırmak"
date: 2013-11-01 03:45:00 +0300
description: "Merhaba Arkadaşlar,"
categories:
  - csharp
tags:
  - csharp
  - xml
  - generics
---
# Çalışma Zamanında Generic Tip Üretmek, Generic Metod Çağırmak
Merhaba Arkadaşlar,

Bu görsel dersimizde çalışma zamanında kullanılabilecek örnek senaryolardan birisine daha değinmekteyiz. İlk amacımız Generic olarak tanımlanmış bir tipin çalışma zamanında üretilmesini sağlamak (Örneğin List koleksiyonunun) Diğer amacımız ise generic bir metodun yine çalışma zamanında üretilip, yürütülmesi.

Her iki senaryo için de geçerli olan önemli nokta ise generic tip bilgisinin çalışma zamanında string bir değişken olarak gelmesi. Bir başka deyişle T, K gibi isimlendirilen generic bilginin, aslında çalışma zamanında her hangi bir kaynaktan (örneğin XML tabanlı bir map dosyasından) metinsel olarak gelmesi durumu söz konusu. Bu nedenle senaryo, geliştirme zamanında generic tip kullanımından farklılaşmakta. Haydi gelin nasıl yaptığımızı birlikte inceleyelim.