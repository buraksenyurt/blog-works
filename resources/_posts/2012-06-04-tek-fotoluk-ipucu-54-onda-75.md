---
layout: post
title: "Tek Fotoluk İpucu–54 Onda 75"
date: 2012-06-04 15:16:00 +0300
categories:
  - tek-fotoluk-ipucu
tags:
  - collections
  - comparer
  - .net-framework
---
Kendi tiplerimize ait koleksiyon nesnelerini kullanırken, Sort metodunu ele aldığımız durumlarda mutlaka neye göre karşılaştırma yapacağımızı belirtmemiz gerekmektedir. Bu amaçla IComparer veya IComparable gibi arayüzleri (Interface) ve bunların generic versiyonlarını kullanırız.

.Net Framework 4.5 ile birlikte ise, karşılaştırma işlemini tek satırda belirtebileceğimiz bir metod gelmektedir (Tabi RC sürümü için konuştuğumuzu hatırlatalım)

Comparer tipinin Create isimli metodu, Sort fonksiyonu için gerekli olan karşılaştırma tipini kolayca üretebilmemizi sağlamaktadır. Parametre olarak aldığı temsilci (Delegate) metodunun kullanımı sırasında, primitive type seviyesine inip Compare operasyonunu çağırmaız yeterlidir. İşte size basit bir örnek

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_96.png)

[![TPI_54Nokta75_thumb2](/assets/images/2012/TPI_54Nokta75_thumb2_thumb.png)](/assets/images/2012/TPI_54Nokta75_thumb2.png)

Başka bir ip ucunda görüşmek dileğiyle

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_96.png)