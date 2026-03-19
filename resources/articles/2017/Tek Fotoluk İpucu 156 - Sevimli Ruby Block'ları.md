---
title: "Tek Fotoluk İpucu 156 - Sevimli Ruby Block'ları"
pubDate: 2017-04-16 15:38:00
categories:
  - Ruby
tags:
  - Ruby
---

# Tek Fotoluk İpucu 156 - Sevimli Ruby Block'ları
Merhaba Arkadaşlar,

Farklı programlama dillerini öğrenmeye çalışırken alışkın olduğum programlama dillerindeki ortamlardan çok daha farklı kabiliyetleri görme şansı buluyorum. Bazı dillerin kabiliyetleri çok dikkat çekici oluyor. Örneğin Ruby dilindeki block kavramı. İlk tanımaya çalıştığım [şu kod parçasında](../2015/Ruby Kod Parçacıkları - 10 (Yield ve Block Kullanımı).md) mevzuyu az çok anlamış olsam da asıl gücünü görmek için Ruby'nin kendi built-in yapılarındaki block kullanımlarını incelemem gerekti. Sonunda asıl faydasını anladım diyebilirim. Konuyu anlamak için dizilere sıklıkla uygulanan each, find_all, reject gibi metodları göz önüne aldım. Örneğin bir dizide belirli kurallara uyan elemanları elde etmek için kullanılan find_all metodunu kendim yazmak istesem ve bir block kullanmam gerekse bunu nasıl yapabileceğimi bulmaya çalıştım. Siz, Ruby'yi geliştiren kişi olsanız ve metodlara parametre olarak kod bloklarını geçirme yeteneğini ilave etseniz veri yapılarında bu özelliği nasıl kullandırtınız? Aşağıdaki örnek ekran çıktısında yer alan findSomething metodu gibi olabilir mi?

![image.axd](images/image.axd)

findSomething metodu içerisinde bir çok ders var. Örneğin bu metoda bir kod bloğu geçilip geçilmediğini anlamak için block_given? fonksiyonunu kullanıyoruz. findSomething public erişim belirleyicisine sahip. Kullanmadığımız takirde private kabul ediliyor ve çalışma zamanında NoMethodError hatası alıyoruz. Bir başka deyişle herhangibir nesneye uygulayamıyoruz. İçeride yer alan self anahtar kelimesi ile findSomething metodunun uygulandığı nesneyi yakalamaktayız. Şayet bu, dizi türevli bir nesne ise each ile elemanlarında dolaşabiliriz. yield ile tahmin edileceği üzere findSomething metoduna gelen kod bloğunu çağırıyoruz. Ayrıca yield ile findSomething metoduna gelen kod bloğuna işlenmesi için iterasyonun o anki nesnesini de (item oluyor) göndermekteyiz.

> yield anahtar kelimesinin kullanıldığı yerde findSomething metodundan dışarıya çıkıp kendisine parametre olarak gelen kod bloğuna (örnekte {|n|n%3==} kısmı) geçici olarak uğradığımızı ve item değerini oraya bıraktığımızı, bloktaki iş bitince de tekrar findSomething metoduna döndüğümüzü ifade edebiliriz. Bu block'ların temel çalışma felsefesidir.

<< operatörü ile result isimli diziye eleman ataması gerçekleştirmekteyiz. Kodun findSomething operasyonunu test ettiğimiz satırında 10dan 28e kadar olan değer aralığındaki sayıların 3 ile tam bölünebilenlerini çekip r isimli bir değişkende topluyoruz. findSomething metodu, parametre olarak gelen kod bloğuna göre elde ettiği sonuçları geriye döndüren bir fonksiyon görevi üstleniyor burada. find_all metodunun tıpkısını aynısı gibi değil mi?

Aynı felsefeyi kullanarak örneğin reject isimli built-in fonksiyonu da siz yazmaya çalışabilirsiniz. Aslında Ruby ile birlikte gelen ve kod bloğu alarak çalışan fonksiyonları kendiniz yazmayı deneyerek block kavramına olan aşinalığınızı arttırabilir konuyu pek güzelce pekiştirebilirsiniz. Ruby, sevimli yetenekleri ile beni kendisine hayran bırakmaya devam ediyor. Bir başka ipucunda görüşmek dileğiyle.
