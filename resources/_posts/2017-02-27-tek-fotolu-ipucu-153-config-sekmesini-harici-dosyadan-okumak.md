---
layout: post
title: "Tek Fotolu İpucu 153 - Config Sekmesini Harici Dosyadan Okumak"
date: 2017-02-27 02:00:00 +0300
categories:
  - dotnet-temelleri
tags:
  - .net
  - csharp
  - configuration-management
  - configuration-manager
  - configSource
---
Uygulamalarımıza ait pek çok parametrik bilgiyi zaman zaman config uzantılı dosyalar içerisinde tuttuğumuz çok olmuştur. Web tabanlı uygulamalar ve servislerde web.config, exe tipi uygulamalarda ise app.config dosyaları söz konusudur. Bu dosyalarda standard olarak kullanılan içerikler mevcuttur. appSettings, connectionStrings sanıyorum ki en popüler olanlarındandır. Peki bu tip konfigurasyon segmentlerinin harici dosyalardan da alınabileceğini biliyor muydunuz? Örneğin uygulamanın appSettings içeriğinin farklı bir dosyadan gelmesini sağlayabiliriz (appSettings içerisine alınacak olan key:value çiftlerinin çok kalabalık olduğu senaryolarda bu teknik oldukça işe yarayabilir) Nasıl mı? Aynen aşağıdaki ekran görüntüsünde olduğu gibi.

![tfi153.gif](/assets/images/2017/tfi153.gif)

Burada önemli olan app.config dosyası içerisindeki appSettings elementinde kullanılan configSource niteliğidir. Bu nitelikte ilgili segmentin hangi dosyadan okunacağı belirtilir. Örnekte kullanılan ApplicationParameters.config için dikkat edilmesi gereken bir kaç nokta vardır. Bunlardan birisi sadece yerine geçecek segment içeriğini taşımasıdır. Yani sadece elementini bulundurmalıdır (Console.WriteLine'daki {0} yerine bir içerik geldiğini hayal edelim) Diğer yandan dosyanın mutlaka asıl konfigurasyon dosyası ile aynı yerde olması beklenir (Bu örnekte app.config ile) Dolayısıyla "Copy to Output Directory" özelliğinin "Copy if Newer" veya "Copy Always" olması gerekmektedir. Bir başka ipucunda görüşmek dileğiyle hepinize mutlu günler dilerim.
