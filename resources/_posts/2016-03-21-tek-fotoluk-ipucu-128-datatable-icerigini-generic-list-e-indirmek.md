---
layout: post
title: "Tek Fotoluk İpucu 128 - DataTable içeriğini Generic List'e İndirmek"
date: 2016-03-21 11:00:00 +0300
categories:
  - tek-fotoluk-ipucu
tags:
  - tek-fotoluk-ipucu
  - dotnet
  - oracle
  - generics
  - datatable
---
Diyelim ki kullandığınız harici bir metod size DataTable tipinden bir nesne örneği döndürmekte. Ne var ki kendi sisteminizde bu nesne içeriğini List tipinden koleksiyon örnekleri ile dolaştırmanız gerekiyor. Hatta DataTable içeriğinin doldurulduğu tablonun kolon adları da sizin programınızdaki standartlar ile uyumlu değil. Bu durumda karşımıza şöyle bir soru çıkıyor. Herhangi bir DataTable içeriğini bir List tipine nasıl dönüştürebiliriz? Dahası dönüşüm işlemi sırasında T tipinin özellik adlarının tablodaki asıl kolon adları ile eşleştirilmesini nasıl sağlarız? İşin sırrı aslına bakarsanız kendi tarafımızdaki Entity tiplerinde birer Attribute kullanmaktan ve bunu çalışma zamanında ele almaktan geçiyor. Aşağıdaki ekran görüntüsü size bir fikir verecektir.

![tfi_129.gif](/assets/images/2016/tfi_129.gif)

Örnekte Oracle üzerinde duran Product isimli bir tablo kullanılmıştır. Tabloda Number tipinden ID, NVARCHAR2 (50) tipinden URUNADI ve yine Number tipinden BIRIMFIYAY isimli alanlar yer almaktadır (Tablonun içeriklerini DataTable olarak geriye döndüren, müdahale edemeyeceğimiz harici bir metod olduğunu düşünerek hareket ettiğimizi unutmayalım) Amaç DataTable içeriğini Product sınıfına ait bir listeye dönüştürmek ama bunu yaparken Türkçe kolon adlarını özellik adları ile eşleştirebilmektir. İşte bu noktada devreye Attribute tipinden türettiğimiz ColumnAttribute girer. Bu niteliği ise DataExtensions sınıfı içerisindeki generic ToList metodu kullanır. Genişletme metodumuz, uygulandığı DataTable içerisindeki tüm satırlar için T tipinden birer nesne örneği oluşturmakta, oluşturulan nesne örneğine ait özellikleri doldurduktan sonra da sonuç kümesini referans eden listeye eklemektedir. Hepsi bu (Bu arada örnekte Oracle bağlantıları için ODP.Net'in 4.0 sürümünü kullanıyoruz) Böylece geldik bir ipucunun daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
