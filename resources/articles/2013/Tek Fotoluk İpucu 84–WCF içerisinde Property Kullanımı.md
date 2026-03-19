---
title: "Tek Fotoluk İpucu 84–WCF içerisinde Property Kullanımı"
pubDate: 2013-03-25 20:40:00
categories:
  - Tek Fotoluk Ipucu
tags:
  - Tek Fotoluk Ipucu
  - WCF
---

# Tek Fotoluk İpucu 84–WCF içerisinde Property Kullanımı
Merhaba Arkadaşlar,

Malum bildiğiniz üzere get ve set bloklarından oluşan özellikler (Properties) aslına bakarsanız arka planda (IL-Intermediate Language) birer metod olarak ifade edilirler. Bu teoriden yola çıkarsak bir servis içerisine özellik (Property) yazıp get,set metoldarını operasyon olarak dış dünyaya sunabiliriz

![Winking smile](images/wlEmoticon-winkingsmile_156.png)

Nasıl mı? Aynen aşağıdaki fotoğrafta görüldüğü gibi.

[![tfi_84](images/tfi_84_thumb.png)](images/tfi_84.png)

Gördüğünüz gibi ReadOnly olarak tanımlanmış bir Property, OperationContract niteliği ile işaretlenen get metodunu dışarıya operasyon olarak sunabilmekte. Bir başka ipucundan görüşmek dileğiyle

![Winking smile](images/wlEmoticon-winkingsmile_156.png)
