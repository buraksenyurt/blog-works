---
title: "Ruby Kod Parçacıkları 28 - Multithreading ve Mutex Kullanımı"
pubDate: 2016-12-09 21:30:00
categories:
  - Ruby
tags:
  - Ruby
  - Threading
  - Concurrency
  - Thread Safety
  - Shared State
  - Mutex
---

# Ruby Kod Parçacıkları 28 - Multithreading ve Mutex Kullanımı
Merhaba Arkadaşlar,

Ruby Kod Parçacıkları serimizin bir önceki bölümünde çoklu iş parçacıklarının (Multithreading) nasıl yazılabileceğini incelemeye başlamıştık. Başrol oyuncumuz olan Thread sınıfının kullanımını gördük. Lakin birden fazla iş parçacığının ele alındığı senaryolarda dikkat edilmesi gereken önemli konulardan birisi de ortak veriler üzerinde işlemler yapıldığında ortaya çıkmaktadır. Eş zamanlı olarak çalışan iş parçacıkları bazı durumlarda verinin tutarlılığının bozulmasına neden olabilir. Nitekim n sayıda iş parçacığının farklı anlarda aynı veri üzerinde işlem yapması söz konusudur. Birbirlerinin işlerini kesebilirler.

Eğer ortak verinin tutarlılığı/kararlılığı önemli ise ilgili iş parçacıklarının senkronize edilmesi gereklidir. Bu pek çok programlama dilinde benzer teknikler ile çözülen bir problemdir. Ruby tarafında Mutex sınıfını kullanarak bu sorunun kontrollü bir şekilde ele alınması sağlanabilir. Mutex ile atomik olmayan metodların atomikleştirilmesi, memory barier konulması vb işler yapılabilir. Kısacası thread-safe denilen güvenli paralelliği sağlayabiliriz.

Tabii öncelikle meseleyi anlamamız gerekiyor. Ortak veri nasıl olur da bozulabilir? İşe aşağıdaki basit kod parçası ile başlayabiliriz.

## Concurrency'nin Temel İlkesi Atomicity'nin Bozulması

```text
require 'thread'

for i in 1..5 do
  points = [0, 0, 0, 0, 0]
  
  threads = 100.times.map do
    Thread.new do
      5000.times do
        points.map! { |i| i + 1 }
      end
    end
  end
  threads.each{|t|t.join}  
  puts points.to_s
end
```

Kodu dikkatli bir şekilde incelemeliyiz. Anlamlı bir iş yapmıyor ancak bize Concurrency konusunda önemli dersler veriyor. 5 sefer çalışan bir test sürecimiz söz konusu. Her seferinde 100 farklı iş parçacığı açıyoruz (Thread nesnesi örnekleyerek) Her bir iş parçacığı içinde 5000 kez yapılan bir işlem söz konusu. Bu işlem sırasında points isimli dizinin eleman değerleri arttırılıyor. Uygulama tamamlandığında dizideki her bir elemanın 500000 olmasını bekleriz değil mi? Oysaki çalışma zamanı sonuçları oldukça farklıdır.

![image.axd](images/image.axd)

Ekran görüntüsü Eclipse IDE'sine aittir. Eclipse'e [şu adresteki plug-in'i yükleyerek](https://marketplace.eclipse.org/content/ruby-dltk) Ruby için geliştirme yapabilirsiniz.

Dikkat edileceği üzere sadece bir sefer 500000 değeri tutturulabilmiş (siz yapacağınız denemelerde daha fazla tutturabilir veya hiç tutturamayabilirsiniz de) ve kalan diğer denemelerde farklı sayılar elde edilmiştir. Buradaki sıkıntı points dizisi içerisindeki elemanlara erişen iş parçacıkları içerisinde atomiklik ilkesini bozan map! metodunun kullanılmasıdır. Bu durumu biraz daha açmaya çalışalım.

Çalışan iş parçacıklarından birisi points dizisindeki bir elemanı okumuş olsun. Okuma işlemi sonrası bunun değerini bir arttıracaktır. Lakin arttırma işlemi öncesi diğer bir iş parçacığının ilk iş parçacığının işleyişini kestiğini düşünelim. Hatta bu ikinci iş parçacığı dizideki ilgili elemanı okuyup arttırmış ve başarılı bir şekilde kayıt etmiş olsun (O değerle ilgilenen ilk iş parçacığı henüz işini bitirememişken) İşte problem.

Bu yüzden 500000 değerinden küçük ve birbirlerinde farklı sayısal değerler elde ettik. Problem iş parçacıklarının birbirini karşılıklı kesmesi dışında Conccurency'nin temel gerekliliklerinden olan Atomicity ilkesini bozan map! metodunun kullanılmasıdır. Öyleyse sorunu çözelim (Bu arada Conccurency denince üç temel ilke söz konusu. Atomicity, Visibility-bir thread tarafından oluşturulan etkinin diğer thread'ler tarafından da görülmesi ve Ordering. Visibility ve Ordering konularını ilerleyen zamanlarda ele almaya çalışacağım)

## map! Kullanımını Atomikleştirmek

Mutex sınıfını kullanarak map operasyonunu atomik hale getirebiliriz. Aslında yapacağımız şey bir iş parçacıklarını ilgili veri ile uğraşırken kitlemek ve diğer iş parçacıklarını bu iş sırasında bekletmekten ibaret. Kodu aşağıdaki gibi değiştirerek ilerleyelim.

```text
require 'thread'

mtx=Mutex.new()

for i in 1..5 do
  points = [0, 0, 0, 0, 0]
  
  threads = 100.times.map do
    Thread.new do
      5000.times do
        mtx.synchronize(){
          points.map! { |i| i + 1 }
        }
      end
    end
  end
  threads.each{|t|t.join}  
  puts points.to_s
end
```

Yaptığımız ilk şey mtx isimli bir Mutex nesnesi örneklemek. Sonrasında atomikleştirmek istediğimiz kod parçasını synchronize metoduna ait kod bloğu içerisine alıyoruz. Bu şekilde map operasyonunu senkronize etmiş oluyoruz. Senkronizasyonu sağlayarak çalışmakta olan iş parçacıklarının aynı veri üzerinde işlem yaparken birbirlerini kesmemesi ve veri bütünlüğünün bozulmaması mümkün hale geliyor. İşte çalışma zamanı sonuçları.

![image.axd](images/image.axd)

Görüldüğü gibi tüm denemelerde points dizisi elemanları beklediğimiz gibi aynı toplam değerini üretmiştir. İş parçacıklarını senkronize olarak çalıştırmak bazı durumlarda işlemlerin daha yavaş tamamlanmasına da neden olabilir ancak verinin tutarlılığı oldukça önemli bir konudur. Böylece geldik bir kod parçacığının daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
