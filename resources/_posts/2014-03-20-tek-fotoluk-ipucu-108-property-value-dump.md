---
layout: post
title: "Tek Fotoluk İpucu 108–Property Value Dump"
date: 2014-03-20 21:30:00 +0300
description: "Merhaba Arkadaşlar,"
categories:
  - tek-fotoluk-ipucu
tags:
  - tek-fotoluk-ipucu
---
# Tek Fotoluk İpucu 108–Property Value Dump
Merhaba Arkadaşlar,

Projelerinizde, temel tiplerden (Primitive Types) özellikler içeren yalın nesne örnekleri döndüren servis metodlarını tüketir misiniz? Mutlaka bir yerlerde kullanıyorsunuzdur. Bu tipler bazen kurum dışı servis üreticileri tarafından hazırlanmış olabilirler. Hatta bazıları içlerinde 50ye yakın özellik (Property) de barındırabilir. Ve bazen projenizin özellikle log atan kısımlarında bu nesnelerin belirli tiplerden oluşan özelliklerine ait değerleri olduğu gibi yazdırmak istersiniz.

Acaba her hangibir tipin çalışma zamanı nesne örneğinin, istediğimiz tiplerden oluşan özelliklerine ait değerlerini toplu olarak nasıl elde edebiliriz? Aşağıdaki gibi bir yol tercih edilebilir mi?

[![tfi_108](/assets/images/2014/tfi_108_thumb.png)](/assets/images/2014/tfi_108.png)

Tabi bu ip ucunda da geliştirilebilecek noktalar var.

- Ya tip içindeki özelliklerden bazıları yine kullanıcı tanımlı tiplerse.(ServiceOutput’ un içinde User diye bir sınıf örneği kullanıldığını düşünün) Hafiften bir Recursive’ lik kokusu mu var yoksa?
- Peki ya bazı özellikler IEnumerable türevli koleksiyonlardan veya dizilerden oluşuyorsa. Peki onların çalışma zamanı içeriklerini çıktıya nasıl katabiliriz?

Bu iki konuyu da çözüme kavuşturmaya çalışarak kendinizi daha da geliştirebilirsiniz. Bir başka ip ucunda görüşmek dileğiyle.