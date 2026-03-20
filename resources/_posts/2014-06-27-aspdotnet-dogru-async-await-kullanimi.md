---
layout: post
title: "Asp.Net–Doğru async, await Kullanımı"
date: 2014-06-27 04:00:00 +0300
categories:
  - aspnet-4-5
tags:
  - aspnet-4-5
  - dotnet
  - aspnet
  - http
  - async-await
  - threading
  - concurrency
---
Bazen web sayfalarının yüklenmesi sırasında senkron olarak çalışan ve uzun süren işlemler gerçekleştiririz (ki aslında Web uygulamalarında bu tip yaklaşımları pek tercih etmeyiz) Sayfada ki kontrollerde gösterilmek üzere çeşitli kaynaklardan veri çekilmesi buna örnek olarak verilebilir. Bu tip veri yükleme işlemleri ağırlıklı olarak PageLoad olay metodu içerisinde gerçekleştirilir. Uzun süren işlemlerin kısa sürede tamamlanabilmesi için farklı teknikler mevcuttur. Bir tanesi de asenkron olarak çalışabilmelerini sağlamaktır (Örneğin zaman kaybettiren servis çağrılarının, veri çekme işlemlerinin eş zamanlı hale getirilmesi)

.Net dünyasında bu tip asenkron işleri kolaylaştıran async, await keyword’ leri artık yaygın olarak kullanılmakta. Lakin Web dünyasında biraz daha dikkatli olmak gerekir. Nitekim bir web sayfasının yaşam döngüsü (Page Life-Cycle), async geri bildirimlerini sorunsuz şekilde ele alan Windows UI Thread’ lerinden biraz daha farklı çalışmaktadır. Güvenilir ve stabil bir ortam söz konusu değildir. Dahası HTTP 503 hatasının alınmasına neden olabilecek vakalar vardır. İşte bu görsel dersimizde bir sayfanın yüklenmesi esnasında asenkron hale getirilmesi istenen işlemlerde uygulanabilecek doğru ve tavsiye edilen bir pratiği incelemeye çalışacağız.

Bir başka görsel dersimizde görüşmek dileğiyle.