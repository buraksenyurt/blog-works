---
layout: post
title: "Tek Fotoluk İpucu 50 - Pivot Taklidi Yapan LINQ"
date: 2012-04-05 01:55:00 +0300
categories:
  - tek-fotoluk-ipucu
tags:
  - language-integrated-query
  - pivot
---
Elimizde ülke bazlı bir toplam satış rakamlarını içeren bir veri listesi olduğunu düşünelim. Normal şartlarda bu tip bir çıktıyı sorguladığımızda veri içeriği ülke bazlı olacak şekilde dikine akacaktır. Ancak istediğimiz çıktı, ükle bazlı satışların toplam tutarlarını yatay eksene taşıyabilmek. Bir nevi SQL tarafındaki PIVOT hareketini gerçekleştirmek istiyoruz. Bunu bir LINQ sorgusu ile yapmaya ne dersiniz? Burdan buyrun

![Wink](/assets/images/2012/smiley-wink.gif)

![tfi_50.PNG](/assets/images/2012/tfi_50.PNG)