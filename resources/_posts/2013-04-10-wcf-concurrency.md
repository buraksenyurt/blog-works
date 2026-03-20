---
layout: post
title: "WCF–Concurrency"
date: 2013-04-10 10:41:00 +0300
categories:
  - wcf
tags:
  - wcf
  - concurrency
---
Windows Communication Foundation içerisinde önem arz eden konuların başında, istemciden gelen taleplerin eş zamanlı olarak değerlendirilme stratejilerine karar verilmesi de gelir. Aslında bir servis davranış biçimi olan ve Single, Multiple, Reentrant olmak üzere 3 farklı modda uygulanabilen Concurrency, tek başına değil, Instance Context Mode ile birlikte düşünülmelidir. Servislerin PerCall, PerSession ve Single gibi modlarda örneklenebildikleri düşünüldüğünde ortaya, 9 farklı kombinasyon çıkmaktadır. İşte bu görsel dersimizde WCF Concurrency konusuna değiniyor ve örnek bir uygulama üzerinden Instance Context Mode ile olan kullanımını kavramaya çalışıyoruz.

Gribal sorunlar nedeniyle zaman zaman konuşmakta zorlandığım ve sık sık öksürmek zorunda kaldığım görsel dersimizi Nedirtv Youtube kanalından da izleyebilirsiniz.

[Youtube Link](https://www.youtube.com/watch?v=1epxQJqc6Ao)

Bir başka görsel dersimizde görüşmek dileğiyle

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_200.png)