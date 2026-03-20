---
layout: post
title: "Rust, WASM, Web Api ve Can-Ban Board !"
date: 2024-05-24 08:49:00 +0300
categories:
  - rust
tags:
  - rust
  - dotnet
  - rest
  - web-api
  - http
  - javascript
  - blazor
  - github
---
Rust çoğunlukla bir sistem programlama dili olarak öne çıkıyor. Ancak [geniş kütüphane](https://crates.io) desteği sayesinde bildiğimiz iş modellerinin uygulanabildiği türden birçok program da geliştirebiliyoruz. Bende hem rust kodlama pratiklerimi artırmak hem de basit gerçek hayat senaryolarını uçtan uca ele alabilmek adına bu tip bir uygulama geliştirmeye çalıştım. Sonrasında kamera arkasına geçtim.

Bu [youtube videosunda](https://youtu.be/a6KVjYGon1c) Rust programlama dilini kullanarak geliştirdiğim basit Kanban Board uygulamasının arka planında neler olup bittiğini anlatmaya çalışıyorum. Uygulamanın önyüz tarafında biraz HTML, Javascript, Bootstrap ve Rust ile derlenmiş WASM paketi kullanılırken, arka planda yine Rust ile yazılmış REST tabanlı bir Web Api yer alıyor. Günümüzdeki birçok uygulama senaryosunda benzer yaklaşımların söz konusu olduğunu ifade edebilirim. Web tabanlı önyüzler, asıl iş fonksiyonellikleri için backend taraftaki servislere HTTP protokolünün Post, Get, Put, Delete, Patch gibi metotları ile ulaşarak kendi gereksinimlerini karşılıyorlar. Hatta benzer senaryo.Net cephesinde Blazor ile de icra edilmekte.

[Youtube Link](https://www.youtube.com/watch?v=a6KVjYGon1c)

Uygulama kullanıcısının aynı anda en fazla beş görev ile çalışılmasına izin veriyor. Bu durumda yeni bir tane eklemek için ya tamamlananları arşive göndermek ya da diğerlerinden feragat etmek lazım. Elbette birçok yeni özellik eklenebilir, kod tekrardan gözden geçirilip iyileştirilebilir. Anlatımda yer alan uygulamanın program kodlarına [github reposundan](https://github.com/buraksenyurt/can-ban-board) bakabilirsiniz. Readme dosyasında WASM derleme işlemleri ile programın nasıl çalıştığına dair detaylı bilgiler de yer alıyor. Bir başka çalışmada görüşmek dileğiyle, hepinize mutlu günler.