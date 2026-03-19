---
layout: post
title: "Tek Fotoluk İpucu 132 - Bir Tipin Özelliklerine Varsayılan Değerlerini Set Etmek"
date: 2016-06-05 18:00:00 +0300
description: "Nesneler kodlarımızın olmazsa olmaz enstrümanları. Özellikle Domain odaklı çözümlerimizde POCO(Plain Old CLR Objects) tiplerini sıklıkla kullanıyoruz. Bu tipler(genellikle class olarak tasarlanıyorlar) içlerinde sayısız özellik(Property) de barındırabiliyorlar. Nesneler örneklendiklerinde ve sonrasındaki yaşam süreleri boyunca çeşitli değerler ile nitelendiriliyorlar."
categories:
  - tek-fotoluk-ipucu
tags:
  - tek-fotoluk-ipucu
  - generics
---
# Tek Fotoluk İpucu 132 - Bir Tipin Özelliklerine Varsayılan Değerlerini Set Etmek
Merhaba Arkadaşlar,

Nesneler kodlarımızın olmazsa olmaz enstrümanları. Özellikle Domain odaklı çözümlerimizde POCO (Plain Old CLR Objects) tiplerini sıklıkla kullanıyoruz. Bu tipler (genellikle class olarak tasarlanıyorlar) içlerinde sayısız özellik (Property) de barındırabiliyorlar. Nesneler örneklendiklerinde ve sonrasındaki yaşam süreleri boyunca çeşitli değerler ile nitelendiriliyorlar. Peki t anında bir nesne örneğinin tüm özelliklerine varsayılan değerlerini atamanız gerekseydi ne yapardınız? Peki ya bunu herhangibir T tipi için uygulanabilir hale getirmek isteseniz ne yapardınız? Yoksa aşağıdaki gibi generic bir genişletme metodu (Generic Extension Method) ile mi çözüm arardınız?

![tfi132new.gif](/assets/images/2016/tfi132new.gif)

Örnekte yapılan şey aslında çok basit. SetToDefault metodu T tipinden herhangi bir nesne üzerinden çağırılabilen bir fonksiyon. Bu metod içerisinde T tipinin tüm özellikleri dolaşılıyor ve türüne göre varsayılan değer atamaları gerçekleştiriliyor. Özellikle referans (Reference) ve değer (Value) türü ayrımına göre bir işlem uygulanmakta. Eğer özellik tipi String ise boş bir değer atıyoruz (String.Empty). Değer türü ise özellik tipinden bir örnek oluşturulup atanıyor (Activator.CreateInstance ile) Hiç biri değilse bir referans türü olarak null değer ataması yapılıyor. (Burada Domain'e göre bazı kuralları işin içerisine katıp farklı varsayılan değerlerin verilmesi de söz konusu olabilir. Bunu bir düşünün) Bir başka ipucunda görüşmek dileğiyle hepinize mutlu günler dilerim.