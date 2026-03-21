---
layout: post
title: "Tek Fotoluk İpucu 137 - Temporary File ile Çalışmak"
date: 2016-10-20 21:30:00 +0300
categories:
  - ruby
tags:
  - ruby-lang
  - io
  - temp-file
  - tek-fotoluk-ipucu
---
Geliştirdiğimiz uygulamanın sadece çalışma zamanında oluşturup kullanacağı ve herhangi bir anda ortadan kaldıracağı geçici bilgilere ihtiyacı olduğunu düşünelim. İlk akla gelen bu tip bilgileri bir yerlerde işi bitene kadar saklamak olacaktır. Bunun için veritabanında geçici bir tabloyu veya işletim sisteminin Registry gibi alanlarını kullanabiliriz. Aslında nesne yönelimli dünyada söz konusu içerikleri birer sınıf örneği olarak tutmakta mümkün.

Ancak bazı hallerde söz konusu bilgileri geçici bir dosyada (Temp File) tutmakta gerekebilir. Bu dosya benzersiz olmalıdır ve sadece içinde oluştuğu uygulama Process'inde veya oluşturulduğu Thread'de kullanılmalıdır. Yani dışarıdan başka Thread'lerin bu dosyaya erişimi olmamalıdır. Hatta uygulamanın dosya ile işi bitince otomatik olarak sistemden kaldırılmalıdır. İşte Ruby dilinde bu ihtiyaç için kullanılabilen hazır bir gem var. İşte basit bir örnek.

![tfi137.gif](/assets/images/2016/tfi137.gif)

Geçici bir dosya oluşturmak için Tempfile sınıfına ait bir nesne örneği oluşturmamız yeterli. Parametre olarak gelen dosya adının sonuna otomatik olarak sistem zamanı ve benzersiz bir ID değeri eklenir. Bu şekilde geçici dosyanın benzersiz olması da sağlanmaktadır. Nitekim aynı uygulamanın birden fazla örneğin sistemde n sayıda geçici dosyanın eş zamanlı olarak oluşmasına neden olacaktır.

Dosya içerisine bilgi atmak ise oldukça kolay. write metodunu veya << operatörünü bu iş için kullanabiliriz. Dosyanın başına gitmek için aynen kasetçalarlarda olduğu gibi rewind operasyonunu kullanıyoruz. Çalışma zamanında oluşan içeriği read metodu ile okuyabilir ve oluşturulduğu yeri görmek için path niteliğine başvurabiliriz. close metoduna yapılan çağrı sonrası ilgili dosya oluşturulduğu yerden otomatik olarak kaldırılacaktır. Gayet basit. Benzersiz, sadece o Thread içinde kullanılabilien geçici bir bilgi havuzu.

Bir başka ipucunda görüşünceye dek hepinize mutlu günler dilerim.