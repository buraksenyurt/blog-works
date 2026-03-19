---
layout: post
title: "FizzBuzz ile Basit Bir TDD Pratiği"
date: 2018-08-09 07:52:00 +0300
description: "DevOps felsefesinin içerdiği önemli pratiklerden birisi de test süreçleridir ve bu noktada TDD(Test Driven Development) büyük önem taşımaktadır. TDD, temel olarak Unit Tests, Integration Tests, User Acceptance Tests gibi pratikleri içerir ve en azından bunların DevOps süreçlerine dahil edilmesi beklenir. Ancak TDD ve DevOps söz konusu olunca daha bir çok test tekniği vardır. Smoke Testing, Penetration Testing, Stress Testing, A/B testing, Fuzz Testing ve Boundary Testing gibi."
categories:
  - codekata
tags:
  - codekata
  - testing
---
# FizzBuzz ile Basit Bir TDD Pratiği
Merhaba Arkadaşlar,

[DevOps felsefesinin](https://medium.com/@burakselyum/devops-e%C4%9Fitiminden-akl%C4%B1mda-kalanlar-6853070d89d6) içerdiği önemli pratiklerden birisi de test süreçleridir ve bu noktada TDD (Test Driven Development) büyük önem taşımaktadır. TDD, temel olarak Unit Tests, Integration Tests, User Acceptance Tests gibi pratikleri içerir ve en azından bunların DevOps süreçlerine dahil edilmesi beklenir. Ancak TDD ve DevOps söz konusu olunca daha bir çok test tekniği vardır. Smoke Testing, Penetration Testing, Stress Testing, A/B testing, Fuzz Testing ve Boundary Testing gibi.

Geliştiriciler olarak bizlerin TDD'ye yatkın olması bu açıdan önemlidir. Yazılım geliştirmeye yeni başlayanlar için TDD pratiklerini öğrenmenin güzel yollarından birisi de Code Kata'sı yapmaktır. Ben daha önceden verdiğim eğitimlerde de basit Code Kata'ları ile TDD pratiklerini anlatmaya çalışmıştım. Bu pratikler bize önemli bir test odaklı bakış açısı disiplini kazandırmak için idealdir. Geçtiğimiz günlerde şirkette yakaladığım yaklaşık 20 dakikalık bir boşluk olunca hemen FizzBuzz katasını bir hatırlayayım istedim.

Açtım Spotify'ı başladım Toto'dan Africa ile kodlamaya. OBS ile ekran kaydı da almayı ihmal ettim. FizzBuzz katası temel olarak aşağıdaki sayı dizilimini içeren kod parçasının geliştirmek için kullanılıyor.

1,2,Fizz,4,Buzz,Fizz,7,8,Fizz,Buzz,11,Fizz,13,14,FizzBuzz,16,17,Fizz,19,Buzz,...

Algoritma oldukça basit. 3 ile tam bölünebilen sayılar için Fizz, 5 ile tam bölünebilen sayılar için Buzz, hem 3 hem 5 ile tam bölünebilen sayılar için FizzBuzz yazılması isteniyor. Dizideki diğer sayılar içinse sayının kendisi yazılmalı. Pek tabii bu pratiği TDD felsefesi ile geliştirmemiz bekleniyor. Önce hata aldıracak şekilde testlere başlayıp, sonrasında bunları çalışır hale getirip en sonunda kodu refactor etmeyi öğreniyoruz. Temel olarak Red Green Blue ilkelerini kullanmaya çalışıyoruz. Hatta Fake değer ve çok basit anlamda Triangulation'e yer veriyoruz.

[Youtube Link](https://www.youtube.com/watch?v=eyeSAFc4N3I)

Bu katayı siz de yapmaya çalışın. Daha önceden yapmadığınız bir kata ise adım adım bakarak ilerlemeli sonrasında ise bakmadan yapmaya çalışmalısınız. Umarım yazılım tarafında TDD ile ilk kez tanışan ve Code Kata'sı yapmak isteyen arkadaşlarımız için yararlı olur. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.