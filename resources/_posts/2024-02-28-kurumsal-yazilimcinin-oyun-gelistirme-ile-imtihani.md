---
layout: post
title: "Kurumsal Yazılımcının Oyun Geliştirme ile İmtihanı"
date: 2024-02-28 16:03:00 +0300
description: "Aslında benim dünyam iş modellerinin nesne yönelimli dillerle buluştuğu birçok yazılım prensibinin uygulanıp teknik borçların da yer aldığı devasa bir evren. Hal böyle olunca insan ister istemez arada bir düzen dışına çıkıp bambaşka maceralara dalmak istiyorum. Ben bunun için ağırlıklı olarak farklı programlama dillerini öğrenmeye çalışıyorum. Java, Ruby, Python ve Go bunlardan bazıları. Ancak en ciddi seviyede ilgilendiğim bir diğer dil Rust. Onunla yaklaşık 3 yıldır hararetli bir şekilde ilgilenmekteyim ve bana kalırsa bir programlama dilini öğrenmenin en eğlenceli yanı onunla oyunlar yazmaya çalışmak."
categories:
  - rust
tags:
  - rust
  - dotnet
  - python
  - java
  - ruby
  - pointers
  - github
  - ownership
  - borrow-checker
---
# Kurumsal Yazılımcının Oyun Geliştirme ile İmtihanı
Bu yazıyı yazdığım tarih itibariyle SGK dökümüm 20 yıl 1 aydır çalışmakta olduğumu ifade ediyor. Sektöre sigortalı bir çalışan olarak adım attığım 1999 yılında Delphi dili ile geliştirme yapan yeni yetme bir bilgisayar programcısıydım. 20 yıllık iş hayatımın %90'ında ise.Net teknolojileri ve C# programlama dilini kullandım, kullanmaya da devam ediyorum. Bu zaman diliminde telekominikasyondan finansa, eğitimden otomotive farklı sektörlerde çalışma fırsatı buldum. Aslında benim dünyam iş modellerinin nesne yönelimli dillerle buluştuğu, birçok yazılım prensibinin uygulanıp teknik borçların hortladığı, büyük ölçekli teknoloji değişimlerinin yapılıp çevik metodolojilerle ilerlendiği içinde Enterprise kelimesinin sıklıkla zikredildiği devasa bir evren. Bu evrende bir çok Neo ve Trinity var. Gündüzleri kurumsal dünyanın rutin çarklarına adapte olup geceleri farklı dünyaların kapılarını açmaya çalışan. Hal böyle olunca insan ister istemez arada bir düzen dışına çıkıp kendi konfor alanını terk ederek bambaşka maceralara dalmak istiyor. Ben bunun için ağırlıklı olarak farklı programlama dillerini öğrenmeye çalışıyorum. Java, Ruby, Python ve Go bunlardan sadece bazıları. Ciddi anlamda ilgilendiğim bir diğer programlama dili ise Rust.

Esasında benim gibi yıllarını kurumsal projelerde geçiren ve managed ortamlarda koşturan bir geliştirici için oldukça ters köşe yapan bir dil olduğunu ifade edebilirim. Sadece Ownership, Borrow Checker, Resource Acquisition is Initialization (RAII) gibi unsurları anlamak, Garbage Collection, Null Reference ve Exception Handling gibi yönetimli kod dünyasının mekanizmaları olmadan çalışmak bile ilk etapta sizi fazlasıyla zorlayabiliyor. Bu konseptleri anladıktan sonra ise yüksek performanslı ve güvenilir kod yapılarını inşa etmek için C, C++ gibi dillerden çok daha ciddi bir alternatif olduğu da ortaya çıkıyor (Özellikle C, C++ tarafından aşina olduğumuz double free, use after free, dangling pointer gibi kritik hataların henüz derleme zamanında önüne geçtiğini düşünürsek...)

Rust ile yaklaşık üç yıldır hararetli bir şekilde uğraşmaktayım ve bana kalırsa bir programlama dilini öğrenmenin en eğlenceli yollarından birisi de onunla oyunlar yazmaya çalışmaktan geçiyor (Söz gelimi Python ile uğraşıyorsanız onu öğrenmenin eğlenceli yollarından birisi IoT cihazlarda kullanmak bir diğeri de onunla eğlenceli terminal oyunları yazmaktır diyebilirim. Tecrübeyle sabittir) Çok basit bir terminal oyunu bile dile hakim olma noktasında önemli yetkinlikler kazandırabilir. Rust tarafındaki oyun motorları oldukça yetenekli. Her ne kadar henüz Unity veya Unreal Engine gibi gelişmiş IDE'ler söz konusu olmasa da bir oyun motorunun nasıl çalıştığı veya oyun dinamiklerinin ne olduğunu öğrenmek için uğraşmaya değer. Rust oyun programlama tarafı ile ilgili ilk durak noktanız ise [Are We Game Yet](https://arewegameyet.rs/) olmalı.

İşin başında temel bazı matematik enstrümanları da hatırlamak gerekiyor, benden söylemesi:) İşte bu düşünceler ışığında geçtiğimiz günlerde[Özgür Yazılım Topluluğunun](https://kommunity.com/ozgur-yazilim-toplulugu/events/past) davetlisi olarak bir sunum gerçekleştirdim. Bir saatlik sunumu iki saat zannedip bana ayrılan zamanı fazlasıyla aştım ve anlatmak istediklerimin tamamını aktaramadım. Bunun için ikinci bir program yapmaya karar verdik ancak şimdilik bu oturumu paylaşabilirim. Lütfen yazmaya çalıştığım çoğu yarım kalan oyunlarımdan yüksek bir beklentiniz olmasın. Aslında hepsi çöp ancak ben yazmaya çalışırken acayip keyif aldım:) İyi seyirler dilerim.

[Youtube Link](https://www.youtube.com/watch?v=i8vu3XbW3rw)

Sunumda üzerinde durduğum oyunlara ait kodlara aşağıdaki github adreslerinden ulaşabilirsiniz.

- [Lunar Landing](https://github.com/buraksenyurt/game-dev-with-rust/tree/main/lunar_landing): SDL2 (Simple DirectMedia Library) kütüphanesi kullanılarak, 1969 yapımı Lunar Landing oyunundan esinlenilerek geliştirilmiştir.
- [Slam Dunk Manager](https://github.com/buraksenyurt/game-dev-with-rust/tree/main/slam-dunk-manager): Herhangi bir oyun motoru kullanmayan, sadece terminalden çalışmak üzere planlanmış bir basketbol menejerlik oyunudur. İlk kez kurcaladığım Game Design Document örneğini de içermektedir.
- [Tetra Pong](https://github.com/buraksenyurt/game-dev-with-rust/tree/main/tetra-pong): Tetra framework kullanılarak geliştirilmiş bir oyundur.
- [Wing Pilot 2024](https://github.com/buraksenyurt/game-dev-with-rust/tree/main/wing-pilot-2042): Macroquad motoru kullanılarak Commodor 64 için yazılmış 1942 oyunundan esinlenilerek geliştirilmiştir.
- [On My Way](https://github.com/buraksenyurt/game-dev-with-rust/tree/main/on-my-way): Bevy oyun motoru ile geliştirilmiş ECS (Entity Component System) altyapısı kullanılan bir oyundur.
- [Packyman](https://github.com/buraksenyurt/rust-farm/tree/main/Practices/packman): Hands-on Rust: Effective Learning through 2D Game Development and Play isimli kitabın yazarı Herbert Wolverson tarafından geliştirilmiş Bracket-Lib kütüphanesi kullanılarak yazılmış bir oyundur.
- [Missile Command](https://github.com/buraksenyurt/missile-command): Macroquad ile geliştirilmiş efsanevi Atari Missile Command oyununun ilkel bir klonudur.
- [Unity Learning](https://github.com/buraksenyurt/learning_unity): Linux üzerinde Unity ile ilgili öğrendiklerime yer verdiğim repodur.

Ayrıca oyun geliştirme macerasında bana yardımcı olan, referans olarak kullandığım kitapları da şöyle sıralayabilirim.

- [3D Math Primer for Graphics and Game Development](https://www.amazon.com/Math-Primer-Graphics-Game-Development/dp/1568817231)
- [Game Engine Architecture](https://www.amazon.com/Engine-Architecture-Third-Jason-Gregory/dp/1138035459)
- [Game Development with Rust and WebAssembly: Learn how to run Rust on the web while building a game](https://www.amazon.com/Game-Development-Rust-WebAssembly-building/dp/1801070970)
- [Exploring Game Mechanics: Principles and Techniques to Make Fun, Engaging Games](https://www.amazon.com/Exploring-Game-Mechanics-Principles-Techniques-ebook/dp/B0BQZS568Q/)
- [Mazes for Programmers: Code Your Own Twisty Little Passages](https://www.amazon.com/Mazes-Programmers-Twisty-Little-Passages-ebook/dp/B013HA1UY4/)
- [Hands-on Rust](https://www.amazon.com/Hands-Rust-Herbert-Wolverson-ebook/dp/B09BK8Q6GY/)
- [The Big Book of Small Python Projects: 81 Easy Practice Programs](https://www.amazon.com/Big-Book-Small-Python-Programming-ebook/dp/B08FH9FV7M/)
- [Essential Mathematics for Games and Interactive Applications: A Programmer's Guide](https://www.amazon.com/Essential-Mathematics-Games-Interactive-Applications/dp/B01K0U7RKU/)
- [Beginning.NET Game Programming in C#](https://www.amazon.com/Beginning-NET-Game-Programming-C/dp/1590593197/)