---
layout: post
title: "Tek Fotoluk İpucu 84–WCF içerisinde Property Kullanımı"
date: 2013-03-25 20:40:00 +0300
description: "Merhaba Arkadaşlar,"
categories:
  - tek-fotoluk-ipucu
tags:
  - tek-fotoluk-ipucu
  - wcf
---
# Tek Fotoluk İpucu 84–WCF içerisinde Property Kullanımı
Merhaba Arkadaşlar,

Malum bildiğiniz üzere get ve set bloklarından oluşan özellikler (Properties) aslına bakarsanız arka planda (IL-Intermediate Language) birer metod olarak ifade edilirler. Bu teoriden yola çıkarsak bir servis içerisine özellik (Property) yazıp get,set metoldarını operasyon olarak dış dünyaya sunabiliriz

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_156.png)

Nasıl mı? Aynen aşağıdaki fotoğrafta görüldüğü gibi.

[![tfi_84](/assets/images/2013/tfi_84_thumb.png)](/assets/images/2013/tfi_84.png)

Gördüğünüz gibi ReadOnly olarak tanımlanmış bir Property, OperationContract niteliği ile işaretlenen get metodunu dışarıya operasyon olarak sunabilmekte. Bir başka ipucundan görüşmek dileğiyle

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_156.png)