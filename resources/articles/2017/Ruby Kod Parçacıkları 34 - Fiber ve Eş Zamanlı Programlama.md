---
title: "Ruby Kod Parçacıkları 34 - Fiber ve Eş Zamanlı Programlama"
pubDate: 2017-05-05 12:17:00
categories:
  - Ruby
tags:
  - Ruby
  - Task Parallel Library
  - Threading
  - Concurrency
---

# Ruby Kod Parçacıkları 34 - Fiber ve Eş Zamanlı Programlama
![rubyfiber_1.gif](images/rubyfiber_1.gif)

Merhaba Arkadaşlar,

Eş zamanlı programlamanın (Concurrent Programming) dile veya çatıya göre farklı uygulanma şekilleri olabiliyor. Esas itibariyle genel amaç eş zamanlı olarak birden fazla işin gerçekleştirilmesini sağlayabilmek. Bu noktada en zorlayıcı noktalardan birisi işlemcinin ve işletim sisteminin bu çalışma taleplerine olan anlık tepkilerinin yönetilmesi. Neredeyse pek çok programlama ortamında Thread'ler ile karşılaşıyoruz (Bu arada yandaki fotoğrafın Ruby Fiber ile bir alakası yok. Fiber konulu imaj ararkan doğal olarak lifli yiyecekler ve sevimsiz diyet konusunu karşıma çıkmıştı)

Ruby tarafında da böyle bir yapı mevcut ama bunun dışında Fiber adı verilen farklı bir kontrol yapısı daha var. Bu nesneler daha hafif iş parçacıkları için kullanılıyor ve çoğunlukla otomatik hesaplamalar yapan yardımcı rutinler için tercih ediliyor. Tercih edilmelerindeki önemli etkenlerden birisi planlamanın geliştirici tarafından yapılması gerekliliği. Yani sanal makinenin veya önceden tanımlı bir Thread yönetim mekanizmasının kontrolünde değiller. Bu kontrol yapısı resume ve yield isimli fonksiyonları kullanıyor. resume ile bir Fiber kod bloğunun çalıştırılması sağlanıyor ve ilgili iş parçacığına abone olan diğer iş parçacığına dönebilmek için yield işlevinden yararlanılıyor.

Çoğunlukla parçalı hesaplamaların yapıldığı, yapılan hesaplama sonuçlarının çağıran ortama geri döndürüldüğü ve tekrardan hesaplamayı yapan kod bloğuna yollanarak yeni sonuçların elde edildiği senaryolarda tercih ediliyorlar. Aslında Fiber kavramı benim yeni karşılaştığım bir konu..Net dünyasında çoğunlukla Task Parallel Library gibi işleri oldukça kolaylaştıran yapılarla çalışıyorum. Bakalım Fiber mevzusunu kıvırabilecek miyim? Dilerseniz vakit kaybetmeden Fiber kullanımına ait basit bir kod parçası ile yola devam edelim.

Merhaba Fiber

```text
=begin
Fiber konusuna bir bakalım
=end

myFiber=Fiber.new do
  puts "Fiber metoduna girdik"
  Fiber.yield
  puts "Fiber metodunun sonuna geldik"
end

puts "Burası çağıran kod kısmı"
myFiber.resume
puts "Tekrar çağıran koddayım"
myFiber.resume
```

Kodu çalıştırdığımızda aşağıdaki sonuçlar elde ederiz.

![rubyfiber_2.gif](images/rubyfiber_2.gif)

Peki ne oldu burada? Aşağıdaki grafik konuyu daha güzel özetleyebilir.

![rubyfiber_3.gif](images/rubyfiber_3.gif)

Aslında ana uygulama kodu ve Fiber nesnesi ile açılan kod bloğu arasında resume ve yield fonksiyonlarından yararlanarak geçişler yapıldığını görüyoruz. Yardımcı rutin olarak çalışmasını istediğimiz kod bloğuna geçiş yapmak veya o blokta kaldığımız yerden işlemlere devam etmek için resume, bu kod bloğunu çağıran uygulama parçacığına geri dönmek içinse yield fonksiyonundan yararlanıyoruz. Bu bir çeşit rutinler arası geçisin planlanmasıdır (Scheduling)

Çağıran ve Blok Arası Veri Alışverişi

Pek tabi Fiber blokları ve çağıran uygulama arasında veri alışverişi yapılması da gerekebilir. Yani Fiber içerisine argüman gönderilmesi ve bir takım işlemler sonucu elde edilen değerlerin döndürülmesi istenebilir. Aşağıdaki kod parçasında bu veri alışverişinin nasıl yapılabileceği basitçe örneklenmeye çalışılmıştır.

```text
fiberX=Fiber.new do |input|
  puts "#{input} bilgisi geldi.Şimdi bunla bir şeyler yapayım"
  Fiber.yield(rand)
  puts "{input} şeklinde yeni bir bilgi geldi. Bunu da hesaplayayım."
  Fiber.yield(rand)
  "Her şey tamamlandı"
end

puts "Çağıran kod"
output1=fiberX.resume(198)
puts "Fiber içinden #{output1} cevabı döndü"
output2=fiberX.resume(200)
puts "Fiber içinden bu kez #{output2} cevabı döndü"
puts(fiberX.resume)
```

Bu kez Fiber bloğuna input isimli tek bir değişken taşınıyor. Bu değişkene göre üretilen rastgele sonuçlar da çağıran tarafa iletiliyor. Kullanım oldukça basit. yield ile Fiber bloğu içinden çağıran tarafa sonuçlar dönülebiliyorken, resume ile de Fiber içerisine parametre aktarılabiliyor. Tabii planlama sırasına göre çağıran kod parçası ile Fiber kod bloğu içerisinde karşılıklı geçişler sağlanmakta. Çalışma zamanı sonuçları aşağıdaki gibidir.

![rubyfiber_4.gif](images/rubyfiber_4.gif)

Fiber Blokları Arası Veri Transferi

Şu ana kadar ki örneklerimizde Fiber ile çağıran ana uygulama arasında geçişler yaptık. Çok doğal olarak n sayıdaki Fiber bloğu arasında da veri transferi gerçekleştirmek isteyebiliriz. Nitekim bir Fiber kod bloğu tarafında yapılan hesaplamalar sırasında farklı bir Fiber kod bloğunun bu sonuçlar ile başka işlemler yapıp diğer bloğa cevap vermesi gerekebilir. Tamamen senaryoya bağlı bir durum. Biz şimdilik bu geçişlerin nasıl yapılabileceğine basit bir örnekle bakalım. İşte kod parçamız.

```text
require 'fiber'

fiber1=fiber2=nil

fiber1=Fiber.new do |input|
  puts "Fiber 1 Başlangıç Input : #{input}"
  newInput=fiber2.transfer(input*rand)
  puts "Fiber 1e gelen yeni Input : #{newInput}"
  fiber2.transfer("işlemleri bitir")
end

fiber2=Fiber.new do |input|
  puts "Fiber 2ye gelen Input : #{input}"
  newInput=fiber1.transfer(input*rand)
  puts "Fiber1 diyor ki '#{newInput}'"
end

puts "işlemler başlıyor"
fiber1.transfer(100)
puts "işlemler bitti"
```

İşlemler biraz karışık gelebilir ancak ilk iki örnekteki mantığı düşünmemiz örneği anlamamız için yeterli. Fiber blokları arasındaki geçişler için yield fonksiyonu yerine fiber modülünde yer alan transfer operasyonundan yararlanıyoruz. Hatta ana kod parçasından fiber1 isimli ilk kod bloğunu başlatırken de transfer fonksiyonunu kullanmaktayız. Kodun çalışma zamanı çıktısı biraz daha iyi fikir verecektir.

![rubyfiber_5.gif](images/rubyfiber_5.gif)

Yaygın Örnek

Ruby, Fiber blokları görüldüğü üzere eş zamanlı programlamada adına basit ve hafif bir kullanım sunmakta. Eş zamanlı çalışma planlamasının programcıya bırakıldığı bu hafif yapı yardımcı rutinlerin ele alındığı senaryolarda oldukça işe yarar görünüyor. En çok verilen başlangıç örneği ise Fibonacci sayılarının hesaplanması için Fiber bloğundan yararlanılması (Hep Recursive metodları kullanırdık ama yardımcı rutin olarak bu sayıları üretecek bir iş parçacığını geliştirmeyi pek düşünmemiştik)

> Fibonacci sayı dizisi 0,1 ile başlayıp her sayının kendisinden önce gelen iki sayının toplamı olarak hesaplandığı bir seridir. 0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144...

Buna ait kod parçasını geliştirmeye çalışmanız, bir süre uğraşıp yapamadığınız takdirde Internetten bakmanız kişisel gelişiminiz açısından yararlı olacaktır. Aşağıdaki kod parçasına da lütfen uğraştıktan sonra bakınız.

```text
def fibogen
  Fiber.new do
    x,y=0,1
    loop do 
      Fiber.yield(y)
      x,y=y,x+y
    end
  end
end

generator=fibogen
20.times do 
  print generator.resume,","
end
```

![rubyfiber_6.gif](images/rubyfiber_6.gif)

fibogen isimli fonksiyon içerisinde yeni bir Fiber bloğu oluşturulmakta. Bu blok içerisinde de sonsuz bir döngümüz var. Başlangıçta Fibonacci sayı dizisinin ilk iki değeri varsayılan olarak verilmekte (ki sayı üreticisinin bu başlangıç değerlerini parametre olarak alması daha güzel olabilir. Nitekim bu sayede istediğimiz iki sayıdan sonrasını elde etme şansını da bulabiliriz) Döngü içerisinde yield operasyonu ile çağıran koda geri dönülüyor ve o anki y değeri yollanıyor. Bu arada generator şeklinde bir değişken tanımlamassak hep 1 sayısını elde edebiliriz. Bunu bir deneyin. Ayrıca ilk 20 fibonacci sayısını bastıktan sonra örneğin ilk 5 fibonacci sayısını yakalamak istersek yeni bir fibogen değişkenine ihtiyacımız olacak (Başlangıç değerlerini parametre olarak alınnnnn) Nitekim bunu yapmassak sonraki 5 fibonacci elemanı ilk 20 elemanın arkasındakiler olarak hesaplanır.

Örneği çok daha iyi hale getirmek gerekiyor. Sınıflaştırılabilir, her yeni sayı dizisi hesaplaması için yeni bir generator üretilmemesi sağlanabilir. Bu kutsal görevleri siz değerli okurlarıma bırakıyorum. Böylece geldik bir Ruby kod parçacığımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
