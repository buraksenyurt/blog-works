---
layout: post
title: "Tek Fotoluk İpucu 92–WMI ile RAM Bilgilerini Almak"
date: 2013-03-25 21:17:00 +0300
categories:
  - tek-fotoluk-ipucu
tags:
  - tek-fotoluk-ipucu
  - wmi
  - windows-management-instrumentation
  - wql
  - wmi-query-language
  - ansi-sql
---
Diyelim ki uygulamanız içerisinden, çalışmakta olduğu Windows işletim sistemi tabanlı makinenize ait fiziki RAM bilgilerini almak istiyorsunuz. Örneğin markasını, hangi slota takılı olduğunu, boyutunu, tipini vs…

Bu amaçla kullanabileceğiniz etkili yöntemlerden birisi de bildiğiniz üzere WMI (Windows Management Instrumentation) alt yapısından yararlanmaktır. Aslında tek yapmanız gereken ANSI-SQL standartlarının bir alt kümesi olan basit bir WQL (WMI Query Language) ifadesi kullanmaktır. Nasıl mı? Aynen aşağıdaki fotoğrafta görüldüğü gibi

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_180.png)

[![tfi_92](/assets/images/2013/tfi_92_thumb.png)](/assets/images/2013/tfi_92.png)

Örneğin benim sistemimde 2 adet Samsung marka 2Gb RAM varmış. Tipleri Unknown gelse de biraz fikir sahibi oldum diyebilirim. Bir başka ipucundan görüşmek dileğiyle

![Smile](/assets/images/2013/wlEmoticon-smile_85.png)