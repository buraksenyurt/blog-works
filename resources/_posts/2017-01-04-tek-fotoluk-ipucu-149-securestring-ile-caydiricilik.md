---
layout: post
title: "Tek Fotoluk İpucu 149 - SecureString ile Caydırıcılık"
date: 2017-01-04 21:23:00 +0300
categories:
  - dotnet-temelleri
tags:
  - security
  - tek-fotoluk-ipucu
  - .net
  - csharp
  - securestring
---
Elimizde veritabanı bağlantı bilgisi, kullanıcı şifresi, uygulamamıza özel port numaraları, finansal oranlar gibi hassas olabilecek içerikleri tutan bir sınıf olduğunu düşünelim. Bu sınıfı kullanmak için doğal olarak bir şekilde örneklenmesi gerekir. Nesnenin kullanılabilir olması içeriği ile birlikte belleğe açılması anlamına da gelir. Uygulama,.Net'in çalışma zamanı ortamında kendisi için oluşturulan Application Domain içerisinde yaşar.

Peki bu metinsel içeriklerin bellekte güvenli bir şekilde durduklarını söyleyebilir miyiz? Belleğe açılan uygulamaların izlerini takip etmek aslında mümkün. RedGate ve benzeri araçlar yada uygulamanın çalışma zamanındaki bellek hareketliliklerini indiren Memory Dump programları ile bu mümkün olabilir. Dolayısıyla belleğe alınmış bir değişken içeriğinin güvenli şekilde saklanmasını sağlamamız önemlidir. SecureString sınıfı bu ihtiyacı karşılamak için kullanılan.Net sınıflarındandır. Nasıl mı? Aynen aşağıdaki fotoğrafta görüldüğü gibi.

![tfi149_1.gif](/assets/images/2017/tfi149_1.gif)

SecureString sınıfını örnekledikten sonra string içeriğin önce karakter katarına dönüştürülmesi sonra her bir karakterin AppendChar sınıfı ile ilave edilmesi söz konusu. MakeReadOnly çağrısı ile ilgili nesne örneğinin Immutable olması sağlanmakta. Protect fonksiyonunu extension metod olarak yazdığımıza dikkat edelim. Tabii buradaki beklenti myPassword değişkeninin şifrelenmiş olarak ekrana basılması değil. Konu, belleğe yerleşen değişken içeriğinin okunma ihtimaline karşı şifrelenerek korunması.

Elbette SecureString ile koruma altına aldığımız içerikleri çalışma zamanı içerisinde elde etmemiz mümkün. Aşağıdaki kod parçası bunun sağlaması gibi düşünülebilir.

![tfi149_2.gif](/assets/images/2017/tfi149_2.gif)

Hafiften StringExtensions sınıfımızı ve Protect metodumuzu bozduk. Marshal tipine ait fonksiyonellikleri kullanarak da Unmanaged Code tarafına geçerek CLR dışı alanlara bulaştık. Sadece işin sağlaması için yaptığımızı tekrar belirtelim. Böylece geldik bir ipucunun daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.