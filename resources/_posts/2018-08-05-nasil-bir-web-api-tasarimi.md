---
layout: post
title: "Nasıl bir Web API Tasarımı?"
date: 2018-08-05 21:30:00 +0300
description: "Web API'ler dahası RESTful API türünden servisler çok uzun zamandır hayatımızda. Benim de gerek blog yazılarımdaki örnekler olsun gerek iş yerinde kullandıklarımız olsun sürekli haşırneşir olduğum enstrümanlar. Ancak son zamanlarda okuduğum kaynaklardan sonra bir çok isimlendirme hatası yaptığımı fark ettim. Nedir bu işin doğru yolu diyerek ortak standartları araştırmaya başladım. Elde ettiğim bir takım sonuçlar oldu. Bu sonuçlardan basit bir çizelge de hazırladım. Öncesinde bir kaç kısa bilgi verelim."
categories:
  - rest
tags:
  - rest
  - json
  - web-api
  - http
  - serialization
---
# Nasıl bir Web API Tasarımı?
![restful_guid_1.gif](/assets/images/2018/restful_guid_1.gif)

Merhaba Arkadaşlar,

Web API'ler ya da RESTful API türünden servisler çok uzun zamandır hayatımızda. Benim de gerek blog yazılarımdaki örnekler olsun gerek iş yerinde kullandıklarımız olsun sürekli haşırneşir olduğum enstrümanlardan birisi. Ancak son zamanlarda okuduğum kaynaklardan sonra bir çok tasarım hatası yaptığımı ve uygulamadığım şeyler olduğunu fark ettim. Nedir bu işin doğru yolu diyerek ortak standartları araştırmaya başladım. Elde ettiğim bir takım sonuçlar oldu. Bu sonuçlardan basit bir çizelge de hazırladım. Aşağıda görebilirsiniz. Ama öncesinde bir kaç kısa bilgi verelim.

Bu tip servisler çoğunlukla kaynaklarla (resources) ilgili temel işlemleri gerçekleştirmek için kullanılmakta. Kaynakların listelenmesi, eklenmesi, silinmesi veya güncellenmesi söz konusu temel operasyonlar olarak düşünülebilir. Servisler, kaynaklarla ilgili operasyonları belli adresler (endpoint diyebiliriz) üzerinden sunar. Servislerin, sundukları kaynakların ve endpoint'lerin sayısı arttıkça baştan düşünülmesi gereken standartlar olduğunu fark ederiz. Özellikle bu servisler farklı takımlarca geliştiriliyorsa. Bu noktada kaynakların isimlendirilmesinden servis adresinin nasıl olacağına, versiyon bilgisinden serileştirme formatının bildirimine, operasyon adlarından döndürülecek HTTP cevaplarından istemciden hangi metodlarla gelineceğine kadar bir çok farklı kavram karşımıza çıkıyor. Bu konulardan bir kısmı ile ilgili olarak şunları söyleyebiliriz.

İsimlendirme standardı olarak aşağıdaki gibi kullanımlar öneriliyor.

```text
https://blog.fabrikamai.com/sample/api/v1/categories
https://api.fabrikamai.com/v1/categories
```

İki kullanımdan birisi tercih edilebilir ama benim önerdiğim ikincisi. Dikkat edileceği üzere her iki tanımlamada versiyonlama bilgisi içermekte. Bazı kaynaklar versiyon bilgisinin Header içerisinde gönderilebileceğini de dile getiriyor ancak yukarıdaki yazımda olduğu gibi açık olması daha doğru.

HTTP Header kısmında daha çok istemci ve sunucu arasındaki mesajlaşmanın formatı belirtiliyor. Content-type ile request, Accept ile de response formatlarını belirtmemiz mümkün.

```text
Content-Type: application/json
Accept: application/json
```

Diğer yandan sadece HTTP-Post ve Get desteği olan istemciler için mutlak suretle X-HTTP-Method-Override kullanılması bekleniyor. Bu konu ile ilgili [şu yazıma](Post Görünümlü Put.md) bakabilirsiniz.

Resource isimlendirmelerinde de çoğul ifadelerin kullanılması öneriliyor. Yani category yerine categories şeklinde tanımlama yapmak lazım. Diğer kullanım önerilerini de şu şekilde özetleyebiliriz ([Excel dosyasını indirmek için tıklayın](https://www.buraksenyurt.com/file.axd?file=/2018/07/RESTfulAPI.xlsx))

![rest_table.png](/assets/images/2018/rest_table.png)

Artık elimde/elinizde RESTful API tasarlamak için güzel bir kılavuz var. Bir başka yazıda görüşmek dileğiyle hepinize mutlu günler dilerim.