---
layout: post
title: "Tek Fotoluk İpucu 140 - Distributed Ruby(dRuby)"
date: 2016-10-31 21:16:00 +0300
categories:
  - ruby
tags:
  - ruby
  - dotnet
  - wcf
  - web-service
  - http
  - performance
---
Eğitmenlik yaptığım dönemlerde.Net'in en zor alanlarından birisi de Remoting idi. Konu aslında Client-Server mantığına uygun olacak şekilde farklı process'ler arasında veri alışverişinin yapılmasından ibaretti ancak uygulaması web servis geliştirmek kadar kolay olmuyordu. TCP/IP bazlı ağlarda.Net Remoting epeyce iyi performans sergilemekteydi. Özellikle ağ üzerinde hareket eden nesnelerin Binary olarak serileşmesi ve TCP/IP protokolünün devrede olması hız avantajı sağlıyordu. Zaman içerisinde servis odaklı yaklaşımlarda kullanılan WCF gibi alt yapılar.Net Remoting yerine tercih edilmeye başlandı. Dağıtık Uygulamalar (Distributed Applications) olarak geçen bu konu Ruby dilinde de kullanılabiliyor. Üstelik çok çok daha basit. Standart Ruby kurulumu ile birlikte gelen drb modülü bu işler için ele alınmakta ([dRuby, Masatoshi Seki tarafından geliştirilmiş bir üründür ve standart Ruby kütüphanesi içerisinde yer almaktadır](http://ruby-doc.org/stdlib-2.3.1/libdoc/drb/rdoc/))

> Aslında dağıtık uygulamaların çıkış amacı şöyle özetlenebilir; Bir bilgisayar problemini tek seferde N sayıda process üzerinden çözümlemeye çalışmak. Zamanında SETI projesi vardı. Sanıyorum ki bu vakaya verilebilecek en güzel örneklerden birisidir. 2011de kaynak yetersizliğinden duran proje 2012de tekrar hayata geçirilmişti. Son durumu nedir bilemiyorum ama [katılımcı olmak için bu adrese](http://setiathome.ssl.berkeley.edu/) bakılabilir.

Ruby dünyasında bu iş için drb kütüphanesini kullanıyoruz. Tahmin edileceği üzere bir sunucu ve bu suncuya çalıştığı süre boyunca bağlanacak istemciler söz konusu. Bu ikili dağıtık bir Ruby uygulamasının iki ana parçasını oluşturmakta. Sunucu ve istemcilerin temel özelliklerini aşağıdaki şekilde olduğu gibi sıralayabiliriz.

![tfi_140_2.gif](/assets/images/2016/tfi_140_2.gif)

Peki basit bir sunucu ve bu sunucu ile iletişim kuracak istemci (ler) nasıl geliştirilebilirler? Yoksa aşağıdaki ekran görüntülerindeki gibi olabilir mi?

Önce sunucu uygulama;

![tfi_140_3.gif](/assets/images/2016/tfi_140_3.gif)

ardından istemci uygulama;

![tfi_140_4.gif](/assets/images/2016/tfi_140_4.gif)

ve çalışma zamanında olanlar

![tfi_140_1.gif](/assets/images/2016/tfi_140_1.gif)

Aslında örnek son derece basit. Sunucu uygulama üzerinde bir dizi tanımlıyor ve 8890 nolu port üzerinden yayına çıkıyoruz. Uygulamayı açık tutmak için sonsuz bir döngümüz var. Döngü içerisinde 5er saniyelik duraksamalarımız ve sonrasında anlık olarak objects isimli dizi içeriğini ekrana basan kod parçamız bulunuyor. Terminal ekranına bir çeşit log bıraktığımızı düşünebiliriz.

İstemci tarafındaki kod parçasında ise 8890 nolu porta bağlanıyor ve new_with_uri metodu ile uzak nesneyi yakalıyoruz. Uzak nesne serileştirilebilir formatta olan bir dizi aslında. Dolayısıyla istemci tarafında << gibi operatörleri kullanarak sunucu tarafındaki sürece ait olan diziye yeni elemanlar ekleyebilir veya dizi bazlı çeşitli işlemler gerçekleştirebiliriz. Dahası bu dizi içeriğindeki nesneleri 8890 nolu porttan hizmet eden sunucuya bağlanan tüm istemciler için kullanabiliriz. İşte en yalın haliyle Distributed Ruby'ye merhaba demiş olduk (Bu arada dRuby konusu ile ilgili olarak [Amazon'daki şu kitabı](https://www.amazon.com/dRuby-Book-Distributed-Parallel-Computing/dp/193435693X/ref=sr_1_1?ie=UTF8&qid=1478002259&sr=8-1&keywords=distributed+ruby) tavsiye ederim)

Bir başka ipucunda görüşmek dileğiyle hepinize mutlu günler dilerim.