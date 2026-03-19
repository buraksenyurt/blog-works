---
title: "Sadece Tarayıcı Kullanarak Web API Servisini Google Cloud Platform Üzerinde Yayınlamak"
pubDate: 2018-01-14 00:44:00
categories:
  - .Net Core
tags:
  - .Net Core
  - .NET
  - Web API
  - Concurrency
---

# Sadece Tarayıcı Kullanarak Web API Servisini Google Cloud Platform Üzerinde Yayınlamak
Merhaba Arkadaşlar,

Sizi Cumartesi gecesi çalışma odama davet etsem...Olmaz mı? Pekiiii...Sadece 15 dakika içerisinde standart bir.Net Core Web API hizmetini Google Cloud Platform üzerine taşıyabileceğinizi söylesem. İlginizi çekmedi mi hala...Pekiiii...Tüm bunları sadece tarayıcı (Chrome, IE, Firefox, Opera, elde ne varsa) ile yapabileceğinizi söylesem:) Sanırım şimdi dikkatinizi çekmiş olmalı. Bu gece farklı bir çalışma denedim. Her Cumartesi olduğu gibi bu Cumartesi gecesi de konsere gitmek yerine West-World üzerinde dolaşmaya karar verdim. Hafta içinde yaptığım denemelerde Google Cloud Platform üzerinde çeşitli.net core uygulamalarını nasıl yayınlayabileceğimi incelemeye çalışmıştım. Google tarafında da işler inanılmaz derecede güzeldi. Derken yaptıklarımı yazmak yerine şöyle eş zamanlı bir video kaydı halinde tutsam daha iyi olmaz mı diye düşündüm. Windows'taki Camtasia Studio'yu aramadım desem yeridir ama Ubuntu tarafındaki OBS'de işimi gördü sayılır. Sonuç olarak hataları ve özellikle de görünmeyen fare imleci ile birlikte Youtube kanalıma yükleyebileceğim keyifli bir çalışma ortaya çıktı. Konuşma içermeyen, piyano tınıları eşliğinde süregelen sakin bir çalışma. Umarım sizler için faydalı olur.

[Youtube Link](https://www.youtube.com/watch?v=u8cXFh0g_AE)

Not: Talep sayısının artma ve Free Trial için verilen paranın bitmesi ihtimaline karşın servis şu anda hizmet dışıdır:) Bilginiz olsun.

![gconsole.gif](images/gconsole.gif)

Peki ne yaptık?

- Chrome ortamını terk etmeden Google Cloud Platform üzerinde proje oluşturduk.
- Bu proje üzerinde Cloud Shell'i kullanarak basit bir.Net Core Web API uygulması açtık.
- Uygulama kodlarını Code Editor (Beta sürümünde) ile düzenledik.
- Bir publish işlemi yapıp app.yaml dosyası ile projeyi dağıtıma hazırlandık.
- gCloud aracını kullanaraktan deploy işlemini
- Son aşamada yayınlana servis adresinne giderek sonuçları gördük.

Komutlar

Çalışma sırasında kullandığım komutları ise şu şekilde özetleyebilirim.

- dotnet --version (dotnet sürümünü öğrendik ki 2.1.3 olmuştu. Restore işleminde biraz sorun oluşturdu)
- dotnet new webapi -o PlanetAPI (webapi şablon projesini oluşturduk)
- dotnet publish -c Release (uygulamayı publish etmek için kullandık)
- gcloud app deploy --version v1 (uygulamamızın App Engine'taşınmasını sağladık. Bunu publish edilen paketin olduğu yerde çalıştırdık.)

Google Cloud Platform tarafı epey hoş. AWS'den sonra burada da farklı bir deneyim yaşadığımı ifade edebilirim. İlerleyen zamanlarda GCP tarafı ile ilgili bir şeyler de yazmaya çalışacağım. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
