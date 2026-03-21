---
layout: post
title: "Asp.Net–Farklı Ortam, Farklı Config, Farklı Deploy"
date: 2014-07-07 12:04:00 +0300
categories:
  - aspnet-4-5
tags: []
---
Bazen geliştirdiğimiz web uygulamaları farklı ortamlar için farklı parametrik değerler kullanır. Bu parametre değerleri çoğunlukla web.config dosyası içerisinde yer alır. Böyle bir durumda ortamlara göre Deployment yapmak zahmetli bir hal alabilir. Nitekim yaygın olarak kullanılan dört farklı ortam söz konusudur. Development, Test, PreProd ve Prod. Her bir ortam için parametreler farklı değerlere sahip olabilir/olması gerekir. Bu yüzden Publish adımlarında, ortamlara göre Profile hazırlanması tercih edilir. Peki bu farklı profiller, config dosyaları içerisindeki çeşitli değerleri (veritabanı bağlantıları, proxy veya servis adresleri, fiziki path bildirimleri vb) ortamlara göre nasıl değiştirebilir? İşte bu görsel dersimizde bu soruya cevap bulmaya çalışıyoruz.

Bir başka görsel dersimizde görüşünceye dek hepinize mutlu günler dilerim.