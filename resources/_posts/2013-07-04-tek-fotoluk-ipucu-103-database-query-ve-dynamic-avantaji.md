---
layout: post
title: "Tek Fotoluk İpucu 103–Database.Query ve dynamic Avantajı"
date: 2013-07-04 21:31:00 +0300
categories:
  - tek-fotoluk-ipucu
tags:
  - web-matrix
  - webmatrix.data
  - database
  - query
  - dynamic
  - var
  - runtime
---
SQL gibi bir veri kaynağına erişmek için pek çok yol olduğunu gayet iyi biliyoruz. Hatta bu işi öğrenmeye başladığımız ilk zamanları hatırlayın. Connection'ın açılması, Command hazırlanması, bir DataAdapter'dan yararlanılarak DataTable/DataSet doldurulması ve DataReader ile veri setinin ileri yönlü dolaşılması ve benzeri tiplerle uğraşırız. Hatta Entity Framework gibi alt yapılar da kendi içlerinde bu temel türlerin ata tiplerinden fazlasıyla yararlanmaktadır.

Son zamanlarda popüler olan (en azından benim dikkatimi yeni çeken) kütüphanelerden birisi de WebMatrix.Data dır (Oldukça eğlenceli olduğunu da ifade etmek isterim) Bu kütüphane içerisinde yer alan Database tipi ise tam bir sihirbaz. Özellikle dynamic desteği sağlayan metodları.

Söz gelimi Northwind veritabanındaki ürünlerin kategori bazlı sayılarını öğrenmek istediniz. Aşağıdaki gibi bir kod parçası pekala işinize yarar.

[![tfi_103](/assets/images/2013/tfi_103_thumb.png)](/assets/images/2013/tfi_103.png)

dynamic türü nedeniyle sorgu sonucu elde edilen liste elemanları üzerinden doğrudan CategoryName ve ProductCount alanlarına gidilmesi mümkün olmuştur

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_211.png)

Bir başka ip ucunda görüşmek dileğiyle.