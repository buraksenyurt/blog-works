---
layout: post
title: "Tek Fotoluk İpucu–54Buçuk"
date: 2012-06-04 11:12:00 +0300
categories:
  - tek-fotoluk-ipucu
tags:
  - .net-framework
  - zipfile
  - compression
  - system.io.compression
  - system.io.compression.filesystem
  - zip
---
Malum Visual Studio 2012 sürümünün RC sürümü geçtiğimiz hafta içerisinde yayınlandı ve internet üzerinden bu konu ile ilişkili yazılarda yayılmaya başlandı. Sadece Visual Studio 2012 değil ama.Net Framework 4.5 tarafında da epey önemli yenilikler geliyor. Ağırlık noktası her ne kadar paralel programlama tarafı ve doğal olarak async ile await anahtar kelimeleri olsa da, temel bazı yenilikler de var. Örneğin artık Zip formatında sıkıştırma desteği var. Söz gelimi bir klasör içeriğini ZIP formatında sıkıştıracak bir Extension metod yazmak istediniz

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_94.png)

İşte buyrun.

[![TPI_54Nokta5](/assets/images/2012/TPI_54Nokta5_thumb.png)](/assets/images/2012/TPI_54Nokta5.png)

[![TPI_54Nokta5_2](/assets/images/2012/TPI_54Nokta5_2_thumb.png)](/assets/images/2012/TPI_54Nokta5_2.png)

> Not: System.IO.Compression.dll ile System.IO.Compression.FileSystem.dll referanslarını eklemek gerekiyor. Ayrıca örnek RC (Release Candidate) sürümüdür. Yani Release sürümde değişiklikler olabilir unutmayın
>
> ![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_94.png)
