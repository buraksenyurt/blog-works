---
layout: post
title: "Tek Fotoluk İpucu 157 - GO ile Bir Queue Yazalım mı?"
date: 2017-05-04 10:10:00 +0300
categories:
  - golang
tags:
  - queue
  - golang
  - struct
  - pointer
  - data-structures
  - algoritma
  - type
---
Go programlama dilinde C#,Java veya benzer dillerden gelenler için söz konusu olan pek çok kolaylık bulunmayabilir. Söz gelimi Stack veya Queue gibi bir koleksiyon kullanmak istersek baştan tasarlamamız gerekebilir. Nitekim [bu adresteki](https://golang.org/pkg/) standart kütüphanelerde Queue ile ilgili bir şey bulamadığımı ifade edebilirim (aslında github üzerinde açık kaynak ek kütüphaneler var) Olsa da olmasa da bir queue veri yapısı tasarlayabiliriz. Hem basit ve temel bir antrenman yapmış oluruz. Aynen aşağıdaki fotoğrafta olduğu gibi (Sisteminizde benim şirket bilgisayarımda olduğu gibi GO ortamı var olmayabilir. [https://play.golang.org/](https://play.golang.org/) adresindeki online derleyiciyi bu anlamda kullanabilirsiniz)

![tfi157.gif](/assets/images/2017/tfi157.gif)

Queue veri yapısı bilindiği üzere FIFO (First In First Out) ilkesine göre çalışır. Yani eklediğimiz ilk eleman ilk olarak elde edilir. Burada standartlaşmış iki fonksiyon söz konusudur. Enqueue ve Dequeue. Enqueue ile eleman eklenmesi, Dequeue ile de ilk eklenen elemanın elde edilmesi ve aynı zamanda veri yapısından çıkartılması işlemleri gerçekleştirilir.

Örnek kod parçasında işleri kolaylaştırmak adına slice tipinden yararlanılmıştır. Çünkü bu tip sahip olduğu fonksiyonellikler sayesinden otomatik olarak genişleyebilir. En azından bu kolaylığı kullanalım değil mi?

Kodda ilk olarak bir struct tanımı olduğu görülmektedir. MyQueue isimli struct içerisinde ise string tipinden bir Slice tanımlanmıştır. Tahmin edileceği üzere kuyruk string veri tipi ile çalışacak şekilde tasarlanmıştır ancak siz istediğiniz bir tipi kullanabilirsiniz. Enqueue operasyonunda yaptığımız tek şey append fonksiyonundan yararlanarak parametre olarak gelen öğeyi items listesine eklemektir. Tabii son eleman olarak eklenecektir. Enqueue, MyQueue nesnesinin adresini geriye döndürmektedir. Yani ilgili fonksiyonlar her zaman oluşturulan MyQueue üzerinde çalıştırılacaktır.

Dequeue fonksiyonunda slice içerisindeki ilk eleman (0 indisli olan) yakalanır. Sonrasında slice'ın 1nci indisinden itibaren kalan kısmı kendi üzerine tekrardan atanır. Dolayısıyla Dequeue fonksiyonu hem ilk giren elemanın döndürülmesini hem de ilk eleman sonrasında kalanların var olan slice'a atanmasını sağlar. MyQueue isimli struct içerisindeki öğelerin ekrana düzgün basılması için Println'in içinde kullandığı String fonksiyonunun yeniden yazılması yeterlidir. Bu sayede ekrana daha düzgün bir formatta içerik basılabilir.

Pek çoğunuzun MyQueue gibi generic bir struct tipi olsa elimizde de kullansak dediğinizi duyar gibiyim. Bunu gerçekleştirmemiz şu anda mümkün değil gibi görünse de interface'leri kullanarak en azından tip güvenli bir veri yapısı oluşturabiliriz. Bunu denemenizi ve hatta Stack şeklinde (LIFO-Last In First Out) bir koleksiyon tipi oluşturmaya çalışmanızı önerebilirim. Kodda bazı tuzaklarda yer alıyor. Örneğin tüm elemanları çektikten sonra index out of range gibi bir hataya düşme olasılığınız yüksek.

![tfi157_2.gif](/assets/images/2017/tfi157_2.gif)

Bunu çözmeyi deneyin.

Bir başka ipucunda görüşünceye dek hepinize mutlu günler dilerim.
