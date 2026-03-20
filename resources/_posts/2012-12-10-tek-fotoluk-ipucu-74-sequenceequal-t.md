---
layout: post
title: "Tek Fotoluk İpucu–74–SequenceEqual<T>"
date: 2012-12-10 20:40:00 +0300
categories:
  - tek-fotoluk-ipucu
tags:
  - tek-fotoluk-ipucu
---
Diyelim ki uygulamanızda zaman zaman da olsa farklı referanslar da duran ve aynı tipte elemanlardan oluşan koleksiyonlarınız oluşuyor ve bunları yeri geldiğinde birbirleri ile kıyaslamak istiyorsunuz. Ne yaparsınız?

Mantıksal olarak her iki koleksiyonu dolaşacak ortak bir döngü ile bire bir kıyaslama yolunu tercih edersiniz

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_122.png)

Ama daha pratik yollar da var. Örneğin aşağıdaki gibi

![Smile](/assets/images/2012/wlEmoticon-smile_54.png)

[![tfi_74](/assets/images/2012/tfi_74_thumb.png)](/assets/images/2012/tfi_74.png)

Bu kadar basit. Bir başka ip ucunda görüşmek dileğiyle.

Not: SequenceEqual metodu kaynak ve hedef koleksiyonların sıralı olduğunu varsayar. Yani yukarıdaki örnekte koleksiyonların içeriklerini farklı sırada tutarsak sonuç False dönecektir.