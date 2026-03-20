---
layout: post
title: "Tek Fotoluk İpucu-44 (Mail Adresi Doğru mu?)"
date: 2012-01-02 10:40:00 +0300
categories:
  - csharp
  - tek-fotoluk-ipucu
tags:
  - csharp
  - tek-fotoluk-ipucu
---
Aslında bu soruya cevap vermek özellikle web developer'lar için son derece kolay. RegularExpressionValidator kontrolünde uygun deseni seçip kontole hatalı mail adresi girilmesi engellenebilir. Ama yine de bazen tedbiri elden bırakmamakta yarar vardır. Söz gelimi bir mail adres listesine toplu mail atacağımız bir senaryoyu göz önüne alalım. Geliştirdiğimiz kodlarda mail adreslerinin doğru olup olmadığını çok basit bir hile ile kontrol edebiliriz. Nasıl mı?

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_61.png)

(Tabi bir yol da RegEx kullanmaktır bildiğiniz üzere. O yolun uygulanış biçimini de size bırakıyorum)

[![PhotoIpucu44](/assets/images/2012/PhotoIpucu44_thumb.png)](/assets/images/2012/PhotoIpucu44.png)

[PerfectEmailing.rar (21,55 kb)](/assets/files/2012/PerfectEmailing.rar)

ve isteyenler için VS Schema Settings dosyası:) [BurakSenyurtVsColorSchema.vssettings (280,59 kb)](/assets/files/2012/BurakSenyurtVsColorSchema.vssettings)
