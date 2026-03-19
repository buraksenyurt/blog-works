---
layout: post
title: "Tek Fotoluk İpucu 130 - Distinct Fonksiyonunu IEqualityComparer<T> ile Özelleştirmek"
date: 2016-04-12 12:15:00 +0300
description: "Koleksiyon bazlı veri kaynaklarında LINQ(Language INtegrated Query) sorgularını yoğun şekilde kullanmaktayız. LINQ bildiğiniz üzere pek çok genişletme metodundan oluşan ve ifadesel olarak koleksiyonlar üzerinde SQL benzeri sorgular yapabilmemizi sağlayan bir alt yapı sunmaktadır. Sıklıkla Select, Where, Count, Sort, Max, Min, Reverse, GroupBy, OrderBy ve daha pek çok metodu kullanırız. Bunlar zaman zaman ifadeler şeklinde ele aldığımız gibi zaman zamanda metod zincirleri biçiminde değerlendiririz."
categories:
  - tek-fotoluk-ipucu
tags:
  - tek-fotoluk-ipucu
  - linq
---
# Tek Fotoluk İpucu 130 - Distinct Fonksiyonunu IEqualityComparer<T> ile Özelleştirmek
Merhaba Arkadaşlar,

Koleksiyon bazlı veri kaynaklarında LINQ (Language INtegrated Query) sorgularını yoğun şekilde kullanmaktayız. LINQ bildiğiniz üzere pek çok genişletme metodundan oluşan ve ifadesel olarak koleksiyonlar üzerinde SQL benzeri sorgular yapabilmemizi sağlayan bir alt yapı sunmaktadır. Sıklıkla Select, Where, Count, Sort, Max, Min, Reverse, GroupBy, OrderBy ve daha pek çok metodu kullanırız. Bunlar zaman zaman ifadeler şeklinde ele aldığımız gibi zaman zamanda metod zincirleri biçiminde değerlendiririz.

Bu metodlardan bazıları aldıkları parametrelere göre farklı davranışları da öğrenebilirler. Örneğin bir tip koleksiyonunun, tipe ait belli bir özelliğe göre Distinct listeye dönüştürülmesini istediğimizi varsayalım. Bu durumda Distinct metodunun IEqualityComparer arayüzünü uygulayan tipleri parametre olarak alabilen aşırı yüklenmiş versiyonunu kullanabiliriz. Böylece çalışma zamanına, Distinct metodunu nasıl icra etmesi gerektiğini öğretebiliriz.

Örneğin elimizde bir ürün listesi olduğunu ve bu listedeki ürünlerin kategori adlarını distinct ile çekmek istediğimizi düşünelim. IEqualityComparer arayüzünden (Interface) yararlanarak bu işlemi basitçe gerçekleştirebiliriz. Nasıl mı? Aynen aşağıdaki fotoğrafta görüldüğü gibi.

![tfi_130.gif](/assets/images/2016/tfi_130.gif)

Dikkat edileceği üzere IEqualityComparer uyarlaması sonucu Equals ve GetHashCode isimli metodların ezilmesi gerekmiştir. Distinct işlemini örneğe göre ürünlerin kategori adları için yapmaktayız. Bu nedenle string tipinden olan CategoryName özelliklerinin Equals ve GetHashCode metodlarına başvuruyoruz. Bu arada dilerseniz Distinct metodunu parametresiz de kullanabilirsiniz. Çalışma zamanı bir hata fırlatmaz ancak Distinct işlemini de düzgün şekilde uygulamaz. Bir deneyin.

Böylece geldik bir tek fotoluk ipucunun daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.