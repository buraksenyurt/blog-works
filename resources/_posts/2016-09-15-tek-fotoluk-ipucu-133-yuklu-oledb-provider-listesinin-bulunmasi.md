---
layout: post
title: "Tek Fotoluk İpucu 133 - Yüklü OLEDB Provider Listesinin Bulunması"
date: 2016-09-15 21:01:00 +0300
description: "Malumunuz büyük çaplı sistemler kolay kolay yenilenmiyorlar. Ancak teknolojik gereklilikler ve değişen ihtiyaçlar ister istemez bu yaşayan organizmaların yeni sunucular üzerinde hayata devam etmelerini gerektirebiliyor. Yeniden yazma maliyetlerinin yüksek olduğu durumlarda var olan sistemin kullandığı pek çok bileşenin de bu sunucular ile uyumlu olması gerekiyor(bekleniyor). Uyumlu olmayanların yerine geçici çözümler uygulanıyor. Tabii mümkün mertebede. Bazen yeni sunuculara taşınan sistem üzerinde yıllardır yaşamını sürdüren C,C++ gibi derlendikten sonra pek de geri çevrilip içeriği görülemeyeccek kodlar da söz konusu oluyor. Böyle bir durumla karşı karşıya kalırsanız vay halinize. Ben ve değerli ekip arkadaşım bu durumdam çok çektik."
categories:
  - csharp
tags:
  - csharp
---
# Tek Fotoluk İpucu 133 - Yüklü OLEDB Provider Listesinin Bulunması
Merhaba Arkadaşlar,

Malumunuz büyük çaplı sistemler kolay kolay yenilenmiyorlar. Ancak teknolojik gereklilikler ve değişen ihtiyaçlar ister istemez bu yaşayan organizmaların yeni sunucular üzerinde hayata devam etmelerini gerektirebiliyor. Yeniden yazma maliyetlerinin yüksek olduğu durumlarda var olan sistemin kullandığı pek çok bileşenin de bu sunucular ile uyumlu olması gerekiyor (bekleniyor). Uyumlu olmayanların yerine geçici çözümler uygulanıyor. Tabii mümkün mertebede. Bazen yeni sunuculara taşınan sistem üzerinde yıllardır yaşamını sürdüren C,C++ gibi derlendikten sonra pek de geri çevrilip içeriği görülemeyeccek kodlar da söz konusu oluyor. Böyle bir durumla karşı karşıya kalırsanız vay halinize. Ben ve değerli ekip arkadaşım bu durumdam çok çektik.

Gelelim ipucumuzun konusuna. Yukarıdaki gibi bir senaryo ile karşılaştığımızı düşünelim. Bu kez mesele yeni Windows Server 2012 sunucusunda MSDAORA isimli OLEDB provider'ını kullanan ASP kod parçalarının çalışmaması (Aslında çalışmama sebebi büyük ihtimalle ilgili nesnenin Windows Server 2012 de zaten desteklenmemesi) Öncelikle bu sunucuda hangi OLEDB provider'larının yüklü olduğunu öğrenmeye çalışarak işe başlamaya karar verdik. Siz olsanız bunun için ne yapardınız. Aşağıdaki gibi bir kod parçası olabilir mi?

![tfi133n.gif](/assets/images/2016/tfi133n.gif)

Aslında tek yapılan şey OleDbEnumerator sınıfının static GetRootEnumerator metodu ile dönen listede hareket etmek ve o anki öğenin tüm bilgilerini ekrana yazdırmak. Böylece geldik bir ipucunun daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.