---
layout: post
title: "Tek Fotoluk İpucu 138 - Bir Web Sayfa İçeriğini İndirmek"
date: 2016-10-24 21:35:00 +0300
categories:
  - ruby
tags:
  - ruby-lang
  - http-request
  - nethttp
---
Vakti zamanında (90lı yılların ortaları diyelim) internette dolanırken bir şeyler öğrenmeye çalıştığım geliştirici siteleri olurdu. Internet o zamanlar kıymetli olduğundan site içeriklerini offline olarak da dolaşabilmek için bazı yardımcı uygulamaları kullanırdık. O yıllarda Delphi konulu makaleler içeren bir sitenin tüm içeriğini indirdiğimi hatırlıyorum. Geçenlerde bu tip bir uygulamayı nasıl yazabilirim diye düşünerken işe web içeriklerini nasıl indirebileceğimi araştırarak başlayayım dedim..Net tarafında bununla ilgili daha önceden uğraşmıştım ama bu sefer Ruby'yi seçtim. Aşağıdaki ekran görüntüsünde bu işi gerçekleştiren basit bir kod parçasını görebilirsiniz (Fotoğraftaki Zero Hour Maps tab'ına aldırmayın:D)

![tfi138.gif](/assets/images/2016/tfi138.gif)

Kod oldukça basit. NetHttp kütüphanesini kullanarak bir web sayfası içeriğini makineye indiriyoruz. İlk olarak bir URI değişkeni jhazırladık. Sonrasında bu değişkeni get_response metoduna parametre olarak veriyoruz. Bu fonksiyon ile elde edilen cevap üzerinden HTTP Get çağrısına ait pek çok bilgiye ulaşabiliriz. Bu bilgiler için her bir key değerini dolaşan döngümüz de mevcut. Örneğin Header bilgilerini okumamız mümkün. İçeriğin tipini, uzunluğunu, sunucudaki işletim sistemi versiyonunu etag bilgisi ve benzerlerini elde edebiliriz. HTTP cevabının 200 OK olması halinde (HTTPOK) ilgili değerleri ekrana basıyor ve sayfa içeriğini content.hmlt isimli dosyaya yazdırıyoruz. (body özelliğinde 0dan içeriğin boyutu kadar karakter okuması yaptığımıza dikkat edin)

Pek tabii HTTP üzerinden içerik okumanın farklı yolları da mevcut. nethttp kütüphanesi oldukça geniş imkanlara sahip ve sizin bu kod üzerine yapabileceğiniz pek çok şey var. Büyük boyutlu içerikleri parça parça indirmek, HTTP Post talepleri göndermek, proxy üzerinden geçiş yapıyorsanız proxy ayarları ile talep yollayabilmek, https için sertifika etkinleştirmek vb...Bu değerli araştırma konularını da size bırakıyorum. Bir başka ipucunda görüşmek dileğiyle hepinize mutlu günler dilerim.
