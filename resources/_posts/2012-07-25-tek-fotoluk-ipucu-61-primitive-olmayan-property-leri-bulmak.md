---
layout: post
title: "Tek Fotoluk İpucu 61–Primitive Olmayan Property’ leri Bulmak"
date: 2012-07-25 22:51:00 +0300
categories:
  - tek-fotoluk-ipucu
tags:
  - reflection
  - language-integrated-query
  - isprimitive
  - cts
  - common-type-system
  - .net-types
  - assembly
  - extension-methods
---
Diyelim ki bir değişkenin tipinin içerisinde yer aldığı Assembly’ daki diğer tiplerin Primitive olmayan (int,double,char vb) özelliklerini bulmak gibi bir ihtiyacınız var. Nasıl bir yol izlersiniz? Kuvvetle muhtemel Reflection’ dan yararlanırsınız. Hatta belki biraz da LINQ katarsınız işin içine. Ya da aklınızdan geçen tam olarak aşağıdaki gibi bir Extension Method’ dur

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_100.png)

[![spt_61New2](/assets/images/2012/spt_61New2_thumb.png)](/assets/images/2012/spt_61New2.png)

[http://www.buraksenyurt.com/pics/spt_61New2.png](/assets/images/2012/spt_61New2.png)Hazır primitive tip demişken. String ve Decimal’ in primitive olmadıklarını biliyor muydunuz?
