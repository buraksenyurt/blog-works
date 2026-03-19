---
title: "Tek Fotoluk İpucu 129 - Override Edemediğimiz ToString Metodunu Genişletmek"
pubDate: 2016-04-03 06:00:00
categories:
  - Tek Fotoluk Ipucu
tags:
  - Tek Fotoluk Ipucu
  - Reflection
  - Generics
---

# Tek Fotoluk İpucu 129 - Override Edemediğimiz ToString Metodunu Genişletmek
Merhaba Arkadaşlar,

Geçtiğimiz günlerde çalışma arkadaşımın oldukça enteresan bir sorusu ile karşılaştım. Projesinde referans ettiği bir kütüphane içerisinde yer alan POCO (Plain Old CLR Objects) tiplerine ait ToString metodlarını ezmesi (Override) gerekiyordu. Ne yazık ki ilgili kütüphane dll olarak referans edildiğinden, içerisindeki sınıflara girip ToString metodunu ezmek mümkün değildi. Bir şekilde ToString metodunu genişletebilir miyiz diye düşünmeye başladık. Bu mümkün gibi görünmüştü. Ancak varsayılan ToString metodu genişletilse dahi kod her zaman Object tipinin ToString metoduna gitmekteydi. Dolayısıyla istediğimizi bir türlü gerçekleştiremiyorduk. Sonrasında var olan projede kullanılan ToString çağrılarında ufak parametre değişiklikleri yaparak ilerleyelim dedik ve aşağıdaki gibi bir extension method yazmaya karar verdik.

![image.axd](images/image.axd)

ToString metodu generic bir versiyon olarak geliştirildi. İlk parametre ToString metodunun uygulanacağı nesne örneğini ifade etmekte. İkinci parametre ise daha çok ToString metodunu aşırı yüklemek (overload) ve var olan ToString metodundan ayırmak için eklenmiş durumda. Aksi durumda object tipinin ToString operasyonuna gidebiliriz. Elbette ikinci parametreyi de işe yarar bir şekilde ele alıyoruz. T tipinin string dönüşüm içerisine katmak istediğimiz özellik adlarını bu dizi yardımıyla kod içerisinde kullanmaktayız. Hafif bir reflection tekniği yardımıyla istenen özellik değerleri StringBuilder üzerinden birleştiriliyor. Hepsi bu.

> Burada siz değerli okurlarıma da bir görev düşüyor. Tüm özellik değerlerinin string içeriklerinin döndürülmesini istersek bunu nasıl yapabiliriz? Haydi biraz düşünelim ve yapmaya çalışalım.

Böylece geldik bir tek fotoluk ipucumuzun daha sonuna. Başka bir ipucunda görüşünceye dek hepinize mutlu günler dilerim.
