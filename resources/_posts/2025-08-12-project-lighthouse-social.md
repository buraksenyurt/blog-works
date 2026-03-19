---
layout: post
title: "Project Lighthouse Social"
date: 2025-08-12 08:00:00 +0300
description: "Project Lighthouse Social, C# ile uçtan uca bir Web projesi geliştirilme serüvenidir. Konu, dünya üzerindeki deniz fenerlerine ait fotoğrafların paylaşıldığı, yorumlandığı ve puanlandığı bir sosyal platform hazırlamaktır. Projede mümkün mertebe yazılım dünyasının efsane konularına olan ihtiyaçları ortaya koymaya çalışmak ilk amaçlarımdan birisidir. Örneğin, hiçbir mimari kalıba uymadan sadece belli prensipleri (soyutlamalar, bağımlılıkları tersine çevirme, sorumlulukları dağıtma vs gibi) baz alarak bir proje iskeleti oluşturup, sonrasında sorularla yaklaşımın doğruluğunu değerlendirmek, açık noktaları tespit etmek ve standartlaşmış bir mimari kalıba çevirmek gibi."
categories:
  - csharp
tags:
  - csharp
  - dotnet
  - postgresql
  - redis
  - rabbitmq
  - web-api
  - docker
  - performance
  - github
---
# Project Lighthouse Social
Project Lighthouse Social, C# ile uçtan uca bir web projesinin geliştirilme serüvenidir. Konu, dünya üzerindeki deniz fenerlerine ait fotoğrafların paylaşıldığı, yorumlandığı ve puanlandığı bir sosyal platform yazmaktır. Projede mümkün mertebe yazılım dünyasının efsane konularına olan ihtiyaçları ortaya koymaya çalışmak ilk amaçlarımdan birisidir. Örneğin, hiçbir mimari kalıba uymadan sadece belli prensipleri (türetmeler, bağımlılıkları tersine çevirme, sorumlulukları dağıtma vs gibi) baz alarak bir proje iskeleti oluşturup sonrasında sorularla yaklaşımın doğruluğunu değerlendirmek, açık noktaları tespit etmek ve standartlaşmış bir mimari kalıba çevirmek gibi.

Seriye ait anlatımlara [youtube üzerinden](https://youtube.com/playlist?list=PLY-17mI_rla6Kt-Ri6nP1pE62ZyE-6wjS&si=YGd-iBIwZGLIAZBM) erişebilirsiniz. Seri boyunca işlenen kodlar [github reposuna](https://github.com/buraksenyurt/project-lighthouse-social) da düzenli olarak aktarılmaktadır.

[Youtube Link](https://www.youtube.com/playlist?list=PLY-17mI_rla6Kt-Ri6nP1pE62ZyE-6wjS)

Günümüzde yapay zeka kodlama araçlarının gelişimi ortada olan bir gerçek. Bir gün Claude Sonnet muazzam işler yaparken bir bakmışsınız ertesi gün GPT bir adım öne çıkmış, derken aralarından Gemini sıyrılmaya kalkmış. Artık bir solution'ı kolayca yorumluyor, defolarını çıkartıyor, iyileştirme noktaları öneriyor ve hatta uygulayabiliyorlar. Yakın zamanda belki de en iyi yazılım mimarlarından dahi daha iyi çözüm iskeletleri oluşturacak ve hatta var olan mimarileri evrimleştirip yenilerini karşımıza çıkaracaklar. Giderek daha az saçmaladıkları da aşikar. Oysa ki programcıyı programcı yapan şey yaptığı hatalardan edindiği tecrübelerdir. İşte bu çalışmada kasıtlı veya kasıtsız hatalar yapıp, insanların neden yazılım mimarilerine ihtiyaç duyduklarını neden birtakım prensipler geliştirdiklerini ortaya koymaya çalışacağız. Teknik borçtan basedeceğiz, farklı sistemleri (Redis, MinIO, Vault, RabbitMQ, KeyCloack, Postgresql vb) entegre edip, birim testler yazacağız. AI tabanlı web api'lerden hizmet alıp, performans odaklı kütüphaneler geliştireceğiz. En başından beri beni heyecanlandıran bu projenin biraz da konusu ve genel özelliklerinden bahsedelim.

## Proje Konusu

Deniz Feneri ve fotoğraf meraklıları için bir sosyal paylaşım platformu geliştirmek.

## Projenin Genel Özellikleri

- Platform üyeleri çektikleri deniz feneri fotoğraflarını paylaşabilirler.
- Üyeler dünyanın dört bir yanında yer alan deniz fenerleri hakkında kapsamlı ve detaylı bilgiler öğrenebilirler.
- Platform üyeleri deniz feneri fotoğraflarına yorum bırakabilir ve puanlayabilirler.

## Amaçlar

Bu projeyi geliştirmenin temel amaçları aşağıda listelenmiştir.

- C# ve.Net platformunu örnek bir proje geliştirerek tanımak.
- Düzenli olarak refactoring uygulayıp kodu iyileştirmeye çalışmak.
- Temel yazılım prensiplerini keşfetmek, uygulamak ve sorgulamak.
- AI Asistanlarından yararlanmak (minimum ölçüde)
- Yazılım mimarilerinin ihtiyaçlarını fark etmeye çalışıp, tartışmak, uygulamak.
- Bol bol düşündürmek, sorgulatmak, sabretmek.

## Zorluklar

Projede aşmaya çalışacağımız ve kendime misyon edindiğim birçok zorluk var. Özellikle dağıtık mimari sahasına çıkmaya çalıştığımız noktada bu zorluklar daha da eğitici olabilirler. İşte bunlardan bazıları.

- Kullanıcıların paylaştığı fotoğrafları nasıl ve nerede tutacağız? (Boyut, depolama yeri, yazma/okuma hızları, dağıtık topoloji kullanımları)
- Kullanıcı yorumlarının denetlenmesi ve istenmeyen ifadelerin engellenmesi nasıl sağlanır?
- Bir fotoğrafın doğru deniz fenerine ait olduğu nasıl tespit edilir?
- Fotoğraflardaki özgünlüğü anlamak için kategorilendirme veya tag belirleme aşamasında AI araçlarından nasıl yararlanılır?
- Çok sayıda kullanıcının farklı lokasyonlardan fotoğraf yüklemesi halinde fotoğrafın analizi, doğrulanması, sınıflandırılması gibi hizmetlerin sistemin genelini etkilemeden en hızlı şekilde yapılması nasıl sağlanır?
- Çözüme dahil olan harici servislerin oluşturacağı dağıtık sistemde kaotik durumların önüne nasıl geçilir, sistemin dayanıklılığı nasıl sağlanır?

## Bu Çalışma Kimlere Göre

Sık gelen sorulardan birisi de bu seriden yararlanmak isteyenlerin neleri bilmesi gerektiği yönünde.

- Minimum Profil;
  - Temel seviyede C# ile programlama bilgisine sahiptir.
  - Temel seviyede Nesne Yönelimli Programlama (Object Oriented Programming) bilgisi vardır.
  - Temiz kod kavramı ve standartları hakkında farkındalık sahibidir.
  - Doğrudan uygulamak yerine, sorgular, araştırır, ikna olur ve sonra uygular.
- İdeal Profil;
  - SOLID prensiplerini sorgular.
  - Teknik Borç ile mücadele yöntemleri hakkında fikir sahibidir.
  - Yazılım mimarilerine meraklıdır.
  - Kendi sistemlerinde docker kullanır.
  - Web Api dışında farklı servis geliştirme standartları olduğunu bilir.
  - Dağıtık sistemlerin zorluklarına aşinadır.
  - Doğrudan uygulamak yerine, sorgular, araştırır, ikna olur ve sonra uygular.

Umarım izleyenler ve repoyu takip edenler için hem motivasyon artırıcı hem de ilham verici bir çalışma olur. Başka bir çalışma tekrardan görüşünceye dek hepinize mutlu günler dilerim.