---
layout: post
title: "Rust Programlama Dili için \\\"Hello World\\\""
date: 2022-03-27 09:00:00 +0300
categories:
  - rust
tags:
  - rust
  - ownership
---
Yakın zamanda yazılımcılardan oluşan bir ekibe Rust programlama dili ile ilgili bildiklerimi anlattım. Bunu yaparken örnek bir program kodu üzerinden ilerledim. İlk etapta neyi nasıl anlatacağım konusunda hiçbir fikrim yoktu. Sonrasında doğaçlama hareket etmeye ve yolda karşımıza çıkacak sorunlar üzerinden dilin birkaç özelliğini anlatmaya karar verdim. Derken anlattıklarımı bir video haline getirsem iyi olabilir diye düşündüm. Pek tabii Rust dilini yeni öğrenen birisi olarak bunu bir saatlik zaman diliminde yapmak pek mümkün değil. Yine de ilerisi için iyi bir hazırlık oldu. Belki ilgi duyan arkadaşlar için yol gösterici de olur. Deneysel amaçla gerçekleştirdiğim bu görsel derste aşağıdaki konulara değindim.

- cargo.toml
- log, env-logger crate'leri
- Terminalden girdi almak (stdin)
- mutable olmak
- cargo clippy ile ideomatic yaklaşım
- match ifadesi
- Result tipi
- struct, enum veri yapıları
- &str ve lifetimes (Azıcık)
- Debug, Copy, Clone ve Display trait'leri
- Ownership, Borrowing (Azıcık)

[Youtube Link](https://www.youtube.com/watch?v=qbiAfQFNrUk)