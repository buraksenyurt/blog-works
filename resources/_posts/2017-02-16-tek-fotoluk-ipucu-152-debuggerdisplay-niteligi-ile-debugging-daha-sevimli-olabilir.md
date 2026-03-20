---
layout: post
title: "Tek Fotoluk İpucu 152 - DebuggerDisplay Niteliği ile Debugging Daha Sevimli Olabilir"
date: 2017-02-16 21:29:00 +0300
categories:
  - tek-fotoluk-ipucu
tags:
  - tek-fotoluk-ipucu
  - debugging
  - visual-studio
---
Nitelikler bildiğiniz üzere çalışma zamanına bilgi taşımak amacıyla kullanılan önemli tiplerdendir. Visual Studio tarafında da işimize yarayan bir çok nitelik (Attribute) yer alır. Bunlardan birisi DebuggerDisplay'dir. Önce aşağıdaki ekran görüntüsündeki kod parçasını göz önüne alalım.

![tfi152_1.gif](/assets/images/2017/tfi152_1.gif)

Product isimli sınıfımız ve kendisinden örneklenen nesnelere sahip bir listemiz var. Debug modda olduğumuz kesin. basket değişkeni üzerinde durduğumuzda ise UseDebuggerDisplay.Product şeklinde Namespace.typeName notasyonuna uygun çıktılar görüyoruz. Aslında bir şeyleri debug ederken bu tip listeler üzerinde işe yarar bilgilerin görünmesi daha iyi olabilir (Nitekim + ile açıp ulaşmaya çalışmak yerine o anda hemen görebilmek benim tercih ettiğim bir görüntülenme şekli) Visual Studio bir çalışma zamanı ortamı olduğuna göre ona bunu öğretmemiz gerekiyor. İşte DebuggerDisplay niteliği bu aşamada devreye giriyor. Nasıl mı? Aynen aşağıdaki fotoğrafta görüldüğü gibi.

![tfi152_2.gif](/assets/images/2017/tfi152_2.gif)

Tek yaptığımız Product sınıfının başına System.Diagnostics alanında bulunan DebuggerDisplay niteliğini uygulamak oldu. {} içerisinde verilen DebugMessage ifadesinin Product sınıfının private bir özelliği olduğuna ve yanlızca okunabilir tanımlandığına (mecburi değil) dikkat edelim. Read-Only olması mantıklı çünkü bu özelliği object user değil Visual Studio ortamı kullanıyor. Aslında DebugMessage özelliğinin döndürdüğü string içeriğin formatını DebuggerDisplay niteliğinde doğrudan kullanabiliriz de ancak bu şekilde gerekli ifadeyi (expression), nitelikten alıp bir fonskiyonda sarmalamış oluyoruz. get bir metod bloğu olduğundan içerisinde çok daha farklı işlemler yapabiliriz. Söz gelimi bu bir kategori tipi olsa ve içerisinde ürün listesi barındırsa, kategori adının yanında kaç tane ürün olduğunu ve toplam liste fiyatını göstermek için gerekli hesaplamaları bu blok içerisinde yazabiliriz (Bence bunu bir deneyin)

> Aynı işi aslında ToString metodunu override ederek yapma şansına da sahibiz. Diğer yandan DebuggerDisplay niteliği üzerinden çalışma zamanında bazı özel işvelsellikle gönderme şansına da sahibiz. Söz gelimi nq=No Quatos şeklinde bir bilgi gönderebiliriz. Bir diğer tercih sebebi de az önce bahsettiğimiz gibi ToString'in private bir metod olmayışıdır. Tipin ToString metodunu dışarıya açmak istemediğimiz durumlarda DebuggerDisplay kullanımı ve bu sorumluluğun private bir metoda devredilmesi mantıklıdır.

Bir başka ipucunda görüşmek dileğiyle hepinize mutlu günler dilerim.
