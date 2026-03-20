---
layout: post
title: "Meraklısına NuGet ve NLog ile 5 Dakikada Loglama"
date: 2014-06-20 20:12:00 +0300
categories:
  - dotnet-framework-4-5
tags:
  - dotnet-framework-4-5
---
Loglama kodlamanın vazgeçilmez unsurlarından birisidir. Ayrıca Enterprise seviyedeki çözümlerde kullanılan önemli CrossCutting'ler arasında yer almaktadır. Uygulamaların çeşitli yerlerinde çeşitli seviyelerde log atma işlemleri sıklıkla icra edilir. Bu işlemler, olası Exception’ ların tespit edilmesi, işleyen süreçlerde hareket eden verilerin tarihsel anlamda izlenmesi, uygulamaların sağlık durumları hakkında bilgi edinip tedbirler alınması, buna bağlı olarak gerekli sistemsel birimlerin uyarılması gibi durumlarda oldukça işe yaramaktadır. Her ne kadar kayıt altına alınacak verilerin ne olacağına karar vermek zor olsa da Loglama çoğu zaman hayat kurtarır.

Günümüzde loglama özelliği taşımayan bir Enterprise çözüm görmemiz neredeyse imkansızdır. Ar-Ge ve taşıdığı yazılım prensiplerinin uygulanma şekillerini öğrenmek gibi amaçlar dışında kimse sıfırdan bir Loglama mekanizması geliştirmemektedir. Bunun yerine hazır olarak sunulan açık kaynak kütüphanaler kullanılmaktadır. Hatta elinizin altında NuGet gibi bir paket yönetim aracı varsa, üzerinde çalıştığınız projeye birkaç adımda loglama kabiliyeti kazandırmak oldukça kolaydır. Nasıl mı? Haydi gelin 5 dakikada loglama yapalım.

Not: Güncel NLog sürümünü kontrol edin. Metodların kullanım şekillerinde farklılıklar olabilir ama teori aynıdır.