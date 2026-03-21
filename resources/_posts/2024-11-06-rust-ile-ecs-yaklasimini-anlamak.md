---
layout: post
title: "Rust ile ECS Yaklaşımını Anlamak"
date: 2024-11-06 11:05:00 +0300
categories:
  - rust
tags:
  - rust
  - entity-component-system
  - bevy
  - game-programming
  - game-engine
  - composition
  - oop
  - compositionOverInheritance
  - programlama
---
ECS, Entity Component System olarak adlandırılan ve birçok oyun motorunda kullanılan bir yaklaşımı ifade eder. Composition over Inheritance prensibini benimseyen Data Oriented bir geliştirme ortamı sağlar. Rust tarafında Bevy gibi bazı oyun motorları built-in olarak bu yapıyı kullanır. Oyun kodlarının daha okunabilir, yönetilebilir ve bakımı kolay şekilde tesis edilmesinde önemli imkanlar sağlar. Plug-In ve Bundle yaklaşımlarının uygulanmasını da kolaylaştırır.

Bu bölümde önce klasik, sonrasında Composition over Inheritance yaklaşımlarını basitçe ele alıp Bevy ECS'in ne olduğunu keşfetmeye çalışacağız. Üstelik kodlarımızı yazarken ["Birlikte Rust Öğrenelim"](https://www.youtube.com/playlist?list=PLY-17mI_rla6GsC51G5qz0FTBz_uCrY6S) serisindeki bazı konuları da tekrar etme fırsatımız olacak.

[Youtube Link](https://www.youtube.com/watch?v=KaxEnExhNrk)

Bir başka bölümde görüşmek dileğiyle hepinize mutlu günler dilerim.