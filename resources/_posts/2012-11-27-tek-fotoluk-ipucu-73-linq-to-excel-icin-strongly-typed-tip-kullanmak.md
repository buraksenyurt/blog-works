---
layout: post
title: "Tek Fotoluk İpucu–73–LINQ to Excel için Strongly Typed Tip Kullanmak"
date: 2012-11-27 04:10:00 +0300
description: "Merhaba Arkadaşlar,"
categories:
  - tek-fotoluk-ipucu
tags:
  - tek-fotoluk-ipucu
  - linq
  - http
---
# Tek Fotoluk İpucu–73–LINQ to Excel için Strongly Typed Tip Kullanmak
Merhaba Arkadaşlar,

[Bir önceki ip ucunda](http://www.buraksenyurt.com/post/Tek-Fotoluk-Ipucu-LINQ-to-Excel-ile-Basit-Sorgulama) LINQ to Excel Provider’ dan yararlanmış ve bir Excel dosyasını kolayca nasıl sorgulayabileceğimizi görmüştük. Peki ya Excel tablosunda yer alan satırları, kod tarafında oluşturacağımız Strongly Typed sınıflar içerisindeki özelliklere karşılık gelecek şekilde ifade edebiliyor olsaydık

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_120.png)

Daha şık olmaz mıydı?

İşte size basit bir örnek. Tek dikkat etmemiz gereken, Excel tablosundaki kolonların başlıkları ile aynı isimde olan ve veri türü olarak da dönüştürülebilir tipteki özellikleri (Property) içeren basit bir POCO sınıfı tasarlamamız ve bunu Worksheet çağrısında T yerine kullanmamız. Bir başka deyişle Worksheet ile doğru şekilde eşleştirilebilecek bir POCO (Plain Old CLR Object) sınıfına ihtiyacımız var; o kadar. Hepsi bu

![Smile](/assets/images/2012/wlEmoticon-smile_52.png)

[![tfi_73](/assets/images/2012/tfi_73_thumb.png)](/assets/images/2012/tfi_73.png)

Bir başka ip ucunda görüşünceye dek hepinize mutlu günler dilerim.

[Not:Provider'da değişiklik yapılmış olabilir ve yukarıdaki kod parçasının son sürümde daha farklı bir şekilde ele alınması gerekebilir. Buna dikkat edelim]