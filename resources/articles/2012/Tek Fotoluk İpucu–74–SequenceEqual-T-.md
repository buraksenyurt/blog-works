---
title: "Tek Fotoluk İpucu–74–SequenceEqual<T>"
pubDate: 2012-12-10 20:40:00
categories:
  - Tek Fotoluk Ipucu
tags:
  - Tek Fotoluk Ipucu
---

# Tek Fotoluk İpucu–74–SequenceEqual<T>
Merhaba Arkadaşlar,

Diyelim ki uygulamanızda zaman zaman da olsa farklı referanslar da duran ve aynı tipte elemanlardan oluşan koleksiyonlarınız oluşuyor ve bunları yeri geldiğinde birbirleri ile kıyaslamak istiyorsunuz. Ne yaparsınız?

Mantıksal olarak her iki koleksiyonu dolaşacak ortak bir döngü ile bire bir kıyaslama yolunu tercih edersiniz

![Winking smile](images/wlEmoticon-winkingsmile_122.png)

Ama daha pratik yollar da var. Örneğin aşağıdaki gibi

![Smile](images/wlEmoticon-smile_54.png)

[![tfi_74](images/tfi_74_thumb.png)](images/tfi_74.png)

Bu kadar basit. Bir başka ip ucunda görüşmek dileğiyle.

Not: SequenceEqual metodu kaynak ve hedef koleksiyonların sıralı olduğunu varsayar. Yani yukarıdaki örnekte koleksiyonların içeriklerini farklı sırada tutarsak sonuç False dönecektir.
