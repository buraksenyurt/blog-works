---
layout: post
title: "Tek Fotoluk İpucu 151 - C#, Reflection ve About Info"
date: 2017-01-12 22:00:00 +0300
categories:
  - tek-fotoluk-ipucu
tags:
  - tek-fotoluk-ipucu
  - dotnet
  - wpf
  - windows-forms
  - reflection
  - generics
---
Programlardaki About Info kısımlarını bilirsiniz. Yazdığımız uygulama hakkında bir takım bilgiler verir. Genellikle ürünün adı, bir iki cümle ile ne yaptığı, üreticisi ve hatta versyion numarası ve benzeri bilgiler yer alır. Micorosoft.Net tarafında aslında bu tip bilgileri Assembly'a ait niteliklerde (attribute) belirtiriz. Aynen aşağıdaki ekran görüntüsünde olduğu gibi.

![tfi_151_1.gif](/assets/images/2017/tfi_151_1.gif)

Peki bu bilgileri (en azından son kullanıcı için işe yarar olanları) nasıl elde edebiliriz? Visual Basic tarafında olukça kolayken C# tarafında işin içerisine Reflection'ı katmamız gerekir. Aşağıdaki sınıfa bir bakalım.

![tfi151_2.gif](/assets/images/2017/tfi151_2.gif)

Aslında kritik nokta Reflection ile Assembly özelliklerinin tutulduğu niteliklere (Attribute) bir şekilde ulaşmak. O an çalışmakta olan Assembly örneğini yakalamakla işe başlıyoruz. GetName ile ulaştığımız değişken üzerinden Name ve Version gibi bilgilere ulaşmak mümkün. Lakin Title, Description, Product, Copyright, Trademark ve Company bilgileri Assembly için birer nitelik olarak tutulmakta. Dolayısıyla çalışma zamanında ilgili nitelikleri çalışmakta olan Assembly için okumamız gerekiyor. Bu işi biraz olsun kolaylaştırmak adına GetValue isimli generic bir metod kullandık. T ile gelen niteliğin property ile belirtilen özellik değerini, current ile işaret ettiğimiz güncel Assembly nesne örneği üzerinden yakalamaya çalışıyoruz. Generic bilgilerinizi tazelemenin tam zamanı. Çalışma zamanı görüntüsü ise aşağıdaki gibidir.

![tfi151_3.gif](/assets/images/2017/tfi151_3.gif)

Görüldüğü üzere projeyi derlemeden önce girdiğimiz temel bilgileri çalışma zamanında yakaladık. Elbette bir Console uygulamasında assembly bilgilerini almak çok mantıklı değil. Windows Forms, WPF, Mobile ve benzeri platformlarda bu fonskiyonellik daha çok işinize yarayabilir. Daha da önemlisi envanterde yer alan ne kadar.Net uygulaması varsa onlar hakkında bilgiler alabilir ve belki de bir portal geliştirebiliriz. Hatta geliştirilen uygulamalar için Assembly seviyesinde eksik nitelik bilgilerini de yakalayabilirsiniz. Gerisi sizde. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
