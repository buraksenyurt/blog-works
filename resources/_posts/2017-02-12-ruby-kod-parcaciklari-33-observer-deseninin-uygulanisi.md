---
layout: post
title: "Ruby Kod Parçacıkları 33 - Observer Deseninin Uygulanışı"
date: 2017-02-12 21:47:00 +0300
description: "Bir önceki kod parçasında Singleton kalıbının Ruby tarafında nasıl uygulandığını incelemeye çalışmıştık. Hatırlayacağınız gibi hazır Singleton modülünü kullanarak bu işi gerçekleştirmek son derece kolaydı. Benzer durum Observer tasarım kalıbı için de geçerli..."
categories:
  - ruby
tags:
  - ruby
---
# Ruby Kod Parçacıkları 33 - Observer Deseninin Uygulanışı
![notifyg.gif](/assets/images/2017/notifyg.gif)

Merhaba Arkadaşlar,

Bir önceki kod parçasında [Singleton kalıbının Ruby tarafında nasıl uygulandığını](Ruby Kod Parçacıkları 32 - Singleton.md) incelemeye çalışmıştık. Hatırlayacağınız gibi hazır singleton modülünü kullanarak bu işi gerçekleştirmek oldukça kolaydı. Benzer durum Observer tasarım kalıbı için de geçerli. Bu kalıp bir nesnenin durumunda meydana gelen değişiklikler sonrası ilgili diğer nesnelerin uyarılması amacıyla kullanılan popüler yazılım desenlerinden birisi. (.Net tarafındaki uygulanış şekli ile ilgili olarak [şu eski yazımdan](https://www.buraksenyurt.com/post/Tasarc4b1m-Desenleri-Observer) yararlanabilirsiniz)

Dilerseniz örnek bir senaryo üzerinden hareket ederek bu deseni nasıl uygulayabileceğimize kısaca bakalım. Bir oyun kodunda oyuncuların belirli puan noktalarını aşmaları sonrası oyuncu nesnesi ile ilişkili başka nesnelerin bilgilendirilmesini istediğimizi düşünelim. Söz gelimi bu bilgilendirmeler sırasında oyuncuların seviyelerini bir diğer nesne üzerinden değiştirelim. Tasarım kalıbının uygulanış biçimine göre gözlemlenebilir (observable) bir oyuncu nesnesi ve bu nesnedeki durum değişikliklerini ele alacak bir gözlemci (observer) örneğine ihtiyaç bulunuyor. Aşağıdaki kod parçasını örnek olarak ele alabiliriz.

```text
require 'observer'

class PlayerObserver
  def notify(player,point)
    puts "Player's current point is #{point}"
    player.level="Looser" if player.point<0
    player.level="Pro gamer" if player.point >1000
    player.level="Blackhat" if player.point>2000
  end
end

class Player
  include Observable
  
  attr_accessor :level,:title
  attr_reader :point
  
  def initialize(title)
    @title,@point,@level=title,100,"Standard gamer"
    add_observer(PlayerObserver.new,"notify")
  end
  
  def set_point(v)
      @point=v
      changed
      notify_observers(self,v)
  end  
end

rudi=Player.new("rudii the ram")
puts rudi.level
rudi.set_point(1240)
puts rudi.level
rudi.set_point(2300)
puts rudi.level
rudi.set_point(-300)
puts rudi.level
```

Neler yaptığımıza kısaca bakalım.

observer kalıbını kullanmak için ilgili modül bildirimini yaparak kodlamaya başlıyoruz. PlayerObserver sınıfı gözlemci rolünü üstleniyor. Bu sınıfta yer alan notify metodu, Player nesne örneklerinin puanlarında olan değişiklilere göre tetiklenecek. notify metodu içerisinde o anki Player nesnesinin point değerine bakıp level niteliğini değiştiriyoruz.

Player sınıfı ise observable modülünü kullanacağını belirterek başlıyor. Standart olarak bir kaç niteliği var. Oyununcun lakabını title niteliğinde tutarken, seviyesini level ve güncel puanını da point özelliklerinde tutuyoruz. Dikkat edilmesi gereken ilk nokta initialize metodundaki add_observer fonksiyonunun kullanımı. Bu metod iki parametre alıyor. İlki gözlemci nesne örneği, ikincisi ise bildirim için tetiklenecek olan fonksiyon adı (Eğer herhangibir metod adı belirtilmezse varsayılan olarak update metodu aranacaktır. Hafiften bir duck typing durumu da söz konusudur) Oyuncunun güncel puanını düzenleyen set_point metodunda ise notify_observers çağrısı yapılıyor. Bu çağrıya göre PlayerObserver sınıfındaki notify metoduna o anki Player nesne örneğinin kendisi (self kullanımı ile) ve puanı taşınıyor. Tahmin edeceğiniz gibi birden fazla observer nesnesinin tanımlanması ve tamamına ait olay yayınlanması mümkün.

Uygulamanın çalışma zamanı sonuçları aşağıdaki gibi olacaktır.

![rubyobsrv_1.gif](/assets/images/2017/rubyobsrv_1.gif)

Görüldüğü gibi oyuncunun puanları değiştikçe bunu gözlemleyen sınıftaki notify metodu tetiklenmiş ve bir takım aksiyonlar oluşmuştur. Bu örnekte sadece ekrana bildirim yapılmış olsa da gerçek hayat örneklerinde ilgili bildirim operasyonlarında çok daha farklı işlemler gerçekleştirebilir. Söz gelimi oyuncu için puanına göre bir promosyon kodunun üretilip kendisine alternatif kanallar ile bildirilmesi ve benzeri kuyruklanabilecek asenkron operasyonlar düşünülebilir.

> Pek tabii set_point metodu içerisinde o anki Player nesne örneğinin level değeri belirlenebilir. Buradaki amaç nesnenin çalışma zamanındaki varlığında olacak değişiklikler sonucu bir diğer nesne (veya nesnelerin) uyarılmasıdır. Farklı nesne örnekleri üzerinde olacak değişiklilerin çeşitli gözlemleyiciler tarafından veya bir nesne örneğinde olacak değişiklilerin n sayıda gözlemci tarafından yakalanması gibi vakalarda değerlendirilebilecek bir tasarım kalıbı olarak düşünülmesinde yarar vardır.

Böylece geldik bir kod parçacığının daha sonuna. Bu kısa yazımızda Observer tasarım kalıbının Ruby tarafında nasıl uygulanabileceğini basitçe incedik. Size tavsiyem [Observable modülünü](https://ruby-doc.org/stdlib-1.9.3/libdoc/observer/rdoc/Observable.html#method-i-changed) detaylı bir şekilde incelemeniz. Nitekim eklenen bir gözlemcinin nasıl kaldırılabileceği, ilgili gözlemcinin bir bildirim operasyonunun olup olmadığını kontrolü, ortamda kaç gözlemcinin olduğu ve benzeri işlevsellikleri de kullanmanız gerekebilir. Bir başka kod parçasında görüşmek üzere hepinize mutlu günler dilerim.