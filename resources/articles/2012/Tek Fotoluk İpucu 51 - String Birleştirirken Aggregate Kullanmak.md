---
title: "Tek Fotoluk İpucu 51 - String Birleştirirken Aggregate Kullanmak"
pubDate: 2012-04-24 09:40:00
categories:
  - Tek Fotoluk Ipucu
tags:
  - Tek Fotoluk Ipucu
  - LINQ
  - Generics
---

# Tek Fotoluk İpucu 51 - String Birleştirirken Aggregate Kullanmak
Merhaba Arkadaşlar,

Diyelim ki elinizde n sayıda e-mail adresi var ve bunları kod içerisinde string tipinden generic bir List koleksiyonunda saklıyorsunuz. Bu mail adreslerinin tamamına toplu olarak mail göndermek isterseniz genellikle aralarına virgül veya noktalı virgül işareti koyarak birleştirmeniz gerekir. Aslında bu amaçla basit bir for döngüsü/foreach döngüsü işinize yarayacaktır. Ya da aşağıdaki gibi LINQ'in getirdiği bazı extension method nimetlerinden de yararlanabilirsiniz

![Wink](images/smiley-wink.gif)

![tfi_51N.PNG](images/tfi_51N.PNG)

Görüşmek üzere

![Smile](images/smiley-smile.gif)
