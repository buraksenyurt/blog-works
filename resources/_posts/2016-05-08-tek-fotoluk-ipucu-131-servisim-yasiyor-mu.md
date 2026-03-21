---
layout: post
title: "Tek Fotoluk İpucu 131 - Servisim Yaşıyor mu?"
date: 2016-05-08 02:00:00 +0300
categories:
  - tek-fotoluk-ipucu
tags:
  - web-request
  - web-response
  - http-status-codes
  - extension-methods
  - csharp
  - httpwebrequest
  - httpwebresponse
---
Böyle yeni bir şeyler öğrenmek için enerji bulamadığım vakitler oluyor zaman zaman. Ya çevremdekilerin ya da işlerin etkisi ile azalır enerjim. Bir şeyler öğrenmeyince de kendimi kötü hissettiğimden en azından genişletme metodu (Extension Method) yazmaya çalışayım da pas tutmayım derim. Tabii önce konu seçmek gerekir. Bu kez aklıma "bir servisin yaşayıp yaşamadığını nasıl anlarım?" sorusu takıldı. Mesela bir Uri için Http durum kodu bilgisinin 200 olmasını kontrol eden bir genişletme metodu yazabilirdim. Örneğin aşağıdaki fotoğrafta görüldüğü gibi.

![tfi_131.gif](/assets/images/2016/tfi_131.gif)

Check isimli genişletme metodunu Uri tipine uygulayabiliyoruz. Metod geriye Http durum kodunu, servisin yaşayıp yaşamadığını ve olası bir exception varsa içeriğini döndürüyor. Tek yaptığımız ise söz konusu adrese bir talep göndermek. Sanki bir tarayıcıdan buraya HTTP Get çağrısı yapıyormuşuz gibi. Dönen bilgiyi HttpWebResponse örneği üzerinden değerlendiriyoruz.

Pek tabii fonksiyonun bağzı eksikleri var. Örneğin sadece servislerin yaşayıp yaşamadığını kontrol etmek istiyorsak asmx, html gibi içerikleri bu işin dışında tutmalıyız. Ya da servisin geçerli bir adres olup olmadığını denetlemeliyiz. Bizim metodumuz biraz genel bir metod oldu diyebiliriz. Yani herhangibir Uri'nin yaşayıp yaşamadığını öğreniyoruz. Ancak amaca özel hale getirmek daha mantıklı olabilir. Sadece WCF Servisleri için kontrol yapsın, sadece ASMX ler için yapsın, sadece REST tabanlı olanlar için yapsın ve benzerleri. Bu kararı da metoda biz söyleyebiliriz belki de. Bu kutsal görevleri siz değerli okurlarıma bırakıyorum. Canınız mı sıkılıyor? Buyrun düzeltilecek, uğraşılacak bir şeyler. Bir başka ipucunda görüşmek dileğiyle hepinize mutlu günler dilerim.
