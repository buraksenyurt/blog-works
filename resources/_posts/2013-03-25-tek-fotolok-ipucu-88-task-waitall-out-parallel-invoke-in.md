---
layout: post
title: "Tek Fotolok İpucu 88–Task.WaitAll out, Parallel.Invoke in"
date: 2013-03-25 21:00:00 +0300
categories:
  - tek-fotoluk-ipucu
tags:
  - tek-fotoluk-ipucu
  - task-parallel-library
---
Bildiğiniz üzere paralel çalışmasını istediğimiz görevler olduğunda genellikle bunları birer Task halinde üretir ve bir dizi içerisinde toplarız (En azından TPL-Task Parallel Library geldikten sonra böyle yapmakta olduğumuzu ifade edebiliriz) Görevleri Task tipinden bir dizi içerisinde toplamamızın sebebi ise genellikle WaitAll gibi bir çağrıya ihtiyaç duyabilecek olmamızdır.

Ancak bunun daha pratik olan bir yolu da vardır. O da Parallel sınıfı üzerinden erişilebilen Invoke metodudur. Bu metod birden fazla Action örneğini parametre olarak alabilir, çalıştırabilir ve hatta tamamı sonlanıncaya kadar kod satırını duraksatabilir. Nasıl mı? Aynen aşağıda görüldüğü gibi

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_168.png)

[![tfii_88](/assets/images/2013/tfii_88_thumb.png)](/assets/images/2013/tfii_88.png)

Bir başka ipucunda görüşmek dileğiyle

![Smile](/assets/images/2013/wlEmoticon-smile_78.png)