---
layout: post
title: "Tek Fotoluk İpucu 55 - Distinct ve IEqualityComparer"
date: 2012-07-03 21:00:00 +0300
categories:
  - tek-fotoluk-ipucu
tags:
  - distinct
  - extension-methods
  - language-integrated-query
  - iequalitycomparer
  - csharp
---
Diyelim ki elinizde kendi tipinize ait generic bir liste ve bu liste içerisinde veri bazında tekrarlı nesne örnekleri var. Örneğin bir ürün listesi ve bu liste içerisinde aynı üretici adına ait pek çok kayıt olduğunu düşünelim. Normal şartlar altında SQL tarafında yazacağınız basit bir sorgu ile üreticilerin adlarını tekrarsız olarak elde edebilirsiniz. Peki LINQ tarafındaki Distinct fonksiyonunu kullanarak aynı işi yapabilir misiniz? Ufak bir interface'den yararlanarak bu sorunu aşmanız mümkün. Sorun diyoruz, çünkü interface implementasyonunu yapmassanız, Distinct genişletme metodu (Extension Method) yine çalışır ama beklemediğiniz şekilde

![Wink](/assets/images/2012/smiley-wink.gif)

![tfi56.png](/assets/images/2012/tfi56.png)

Bir diğer ipucunda görüşmek dileğiyle

![Wink](/assets/images/2012/smiley-wink.gif)
