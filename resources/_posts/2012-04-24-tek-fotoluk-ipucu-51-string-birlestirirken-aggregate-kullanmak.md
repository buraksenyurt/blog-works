---
layout: post
title: "Tek Fotoluk İpucu 51 - String Birleştirirken Aggregate Kullanmak"
date: 2012-04-24 09:40:00 +0300
categories:
  - tek-fotoluk-ipucu
tags:
  - tek-fotoluk-ipucu
  - linq
  - generics
---
Diyelim ki elinizde n sayıda e-mail adresi var ve bunları kod içerisinde string tipinden generic bir List koleksiyonunda saklıyorsunuz. Bu mail adreslerinin tamamına toplu olarak mail göndermek isterseniz genellikle aralarına virgül veya noktalı virgül işareti koyarak birleştirmeniz gerekir. Aslında bu amaçla basit bir for döngüsü/foreach döngüsü işinize yarayacaktır. Ya da aşağıdaki gibi LINQ'in getirdiği bazı extension method nimetlerinden de yararlanabilirsiniz

![Wink](/assets/images/2012/smiley-wink.gif)

![tfi_51N.PNG](/assets/images/2012/tfi_51N.PNG)

Görüşmek üzere

![Smile](/assets/images/2012/smiley-smile.gif)
