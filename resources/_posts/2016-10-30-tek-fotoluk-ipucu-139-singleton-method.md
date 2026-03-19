---
layout: post
title: "Tek Fotoluk İpucu 139 - Singleton Method"
date: 2016-10-30 21:30:00 +0300
description: "Gün geçmiyor ki Ruby dilinde beni şaşırtan bir kabiliyet ile daha karşılaşmayayım. Dün gece Ruby kitapları arasında dolanırken öğrendiğim yeni bir kavram var; Singleton Method. Instance metodlarından farklı olarak sadece belli bir nesne örneği için çalışabilecek fonksiyonellikler tanımlayabilmemizi sağlayan önemli bir yetenek. Yani sınıf tanımı içerisinde yer almayıp nesne örneği üzerinden tanımlanabilen ve sadece o örnek için kullanılabilen fonksiyonlar geliştirebildiğimizi düşünün. Konuyu aşağıdaki fotoğrafta yer alan kod parçasında daha net bir şekilde görebiliriz."
categories:
  - ruby
tags:
  - ruby
  - testing
---
# Tek Fotoluk İpucu 139 - Singleton Method
Merhaba Arkadaşlar,

Gün geçmiyor ki Ruby dilinde beni şaşırtan bir kabiliyet ile daha karşılaşmayayım. Dün gece Ruby kitapları arasında dolanırken öğrendiğim yeni bir kavram var; Singleton Method. Instance metodlarından farklı olarak sadece belli bir nesne örneği için çalışabilecek fonksiyonellikler tanımlayabilmemizi sağlayan önemli bir yetenek. Yani sınıf tanımı içerisinde yer almayıp nesne örneği üzerinden tanımlanabilen ve sadece o örnek için kullanılabilen fonksiyonlar geliştirebildiğimizi düşünün. Konuyu aşağıdaki fotoğrafta yer alan kod parçasında daha net bir şekilde görebiliriz.

![tfip_139.gif](/assets/images/2016/tfip_139.gif)

Player isimli bir sınıfımız var. Her bir oyuncunun adını ve oyundaki pozisyonunu tutuyoruz. Bununla birlikte oyuncuların hareket etmelerini sağlayan Move isimli bir metodumuz da var. Normal şartlarda burki isimli nesne örneği üzerinden Move isimli fonksiyonu çağırabiliriz. Hatta başka Player nesne örnekleri için de bu fonksiyonellik geçerli. Ancak kodun ilerleyen kısımlarında sadece burki nesnesi için geçerli olacak Jump isimli bir fonksiyon (Singleton Method) tanımı ve kullanımı söz konusu. Bu fonksiyonu sharp isimli diğer bir Player nesne örneği ile kullanamayız, nitekim kodun çalışması sırasında "undefined method..." hatası aldığımızı fark etmişsinizdir.

Bir diğer enteresan durumda fonksiyon ezilmesi (overriding). Player sınıfı içerisinde tanımlı olan Move fonksiyonunu sadece sharp nesne örneği için ezdik ve çalışma şeklini değiştirdik. Singleton metodların özellikle Unit Test'ler de önemli bir yeri olduğu söyleniyor. Bir sınfın her Unit Test sırasında tekrarlayarak çalışan ama o anki vaka için gerekli olmayan fonksiyonlarını Override etme yeteneğini ile devre dışı bırakma şansımız var örneğin. Ayrıca çalışma zamanında belli bir nesne örneği için ek davranışların tasarlanmasında da Singleton metodlardan yararlanabiliriz.

Böylece geldik bir tek fotoluk ipucunun daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.