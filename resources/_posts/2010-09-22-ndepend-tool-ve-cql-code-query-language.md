---
layout: post
title: "NDepend Tool ve CQL(Code Query Language)"
date: 2010-09-22 20:16:00 +0300
categories:
  - teknik-disi-konular
tags:
  - visual-studio
  - ndepend
  - cql
  - code-query-language
---
Aşağıdaki resimde Visual Studio 2010 Ultimate ortamına ait bir ara pencere görmektesiniz. Dikkat edeceğiniz üzere Select Methods Where NbLinesOfCode>=5 şeklinde bir sorgu cümlesi var. Oppsss!!! Bu nasıl bir sorgu cümlesi? Tahmin etmeye çalışalım. Sanki uygulamadaki kod satır sayısı 5’ ten fazla olan metodların adlarını döndüren bir sorgu cümlesi gibi.

[![blg227_CQL_Sample_1](/assets/images/2010/blg227_CQL_Sample_1_thumb.gif)](/assets/images/2010/blg227_CQL_Sample_1.gif)

Şu anda bir Code Query Language örneğinin Visual Studio 2010 IDE’ si içerisindeki raporlama sonucunu görmektesiniz.

Peki ben bu sorguya nasıl mı kavuştum?

Sevgili Patrick Smacchia (C# MVP), [NDepend](http://www.ndepend.com) isimli bir.Net analiz ürünü geliştirdiklerini bir süre önce bana bildirmişti. Sonrasında ürünün tam sürümünü gönderdi ve bende incelememi, eğer varsa fikirlerimi iletmemi rica etti.

Bildiğiniz üzere Visual Studio 2010 IDE’ si içerisinde Architecture sekmesinde erişip kullanabileceğimiz bazı analiz operasyonları mevcut. Söz gelimi Assembly’ lar, tipler vb… arası bağımlılıkları çıkartabiliyoruz veya bir metodun Sequence Diagram’ ına ulaşabiliyoruz. Bazı var olan mimari kalıpları uygulatıp doğrulatabiliyoruz. Ancak yine de büyük çaplı projelerin yazılması sırasında, bazı metric’ lere uyulup uyulmadığını denetlemek, projenin genel görünümünü rapolarmak, daha detaylı görsel şemalar görmek ve derin bilgi edinmek isteyebiliyoruz. Bu tip ihtiyaçları karşılamak için başka araçlar da var ancak NDepend özellikle CQL (Code Query Language) ve Raporlama kısımlarındaki başarısı ile bence biraz daha ön plana çıkıyor.

NDepend aracı hem kod kalitesini arttırmak için gerekli metric’ lerin kontrolünü sağlıyor hem de az önceki erkan görüntüsünde olduğu gibi CQL ile bazı kriterleri sorgulayabilmemize olanak tanıyor. Tabi bu sadece şu ana kadar öğrenebildiğim iki güzel özelliği. Daha da fazlası var. Ancak basit bir test sürüşü ile yolumuza devam edebiliriz. Bu amaçla örnek olarak aşağıdaki ekran görüntüsünde yer alan Solution içeriğini göz önüne alabiliriz.

[![blg227_SolutionTree](/assets/images/2010/blg227_SolutionTree_thumb.gif)](/assets/images/2010/blg227_SolutionTree.gif)

Sağ alt köşede sarı renkli olan daire mutlaka dikkatinizi çekmiştir. Aslında bu NDepend aracı ile gelen ve Popup Menu açan sihirli bir dairedir

![Wink](/assets/images/2010/smiley-wink.gif)

Ki bastığınızda aşağıdaki ekran görüntüsüne benzer sonuçlar ile karşılaşabilirsiniz.

[![blg227_YellowCircle](/assets/images/2010/blg227_YellowCircle_thumb.gif)](/assets/images/2010/blg227_YellowCircle.gif)

Dikkat edileceği üzere 18 kural ihlali (18 Rules Violated) olduğu hemen göze çarpmaktadır

![Undecided](/assets/images/2010/smiley-undecided.gif)

Rules Violated kısmına tıkladığımızda ise aşağıdaki ara birim ile karşılaştığımızı görürüz.

[![blg227_SampleViolation](/assets/images/2010/blg227_SampleViolation_thumb.gif)](/assets/images/2010/blg227_SampleViolation.gif)

Bu ekran görüntüsünden de anlaşılacağı üzere bazı kalite kriterlerine ait analiz sonuçları üretilmiştir. Örneğin kodun kalitesi açısından Code Quality grubu ve altındaki Type Metrics kısmına bir bakalım. Type Metrics sonuçları içerisinde örneğin çok fazla sayıda metod içeren 10 öğe olduğu belirtilmektedir. Eğer bu satıra çift tıkarsak aşağıdaki CQL sorgusunu ve sonuçlarını elde ederiz.

[![blg227_TooManyMethods](/assets/images/2010/blg227_TooManyMethods_thumb.gif)](/assets/images/2010/blg227_TooManyMethods.gif)

Volaaa!!!

![Laughing](/assets/images/2010/smiley-laughing.gif)

Süper. CQL sorgusunu okuduğumuzda 20 den fazla metod içeren 10 tip olduğunu görmekteyiz (Çok doğru. Bunların hepsi Entity Framework tarafndan üretilen Entity tipleri ve Context sınıfıdır

![Sealed](/assets/images/2010/smiley-sealed.gif)

) Burada tanımlı olan kurala göre bir tipin 20den fazla metod içeriyor olması kodun yönetimi açısından zorlayıcıdır. Tabi bu bir Warning olarak karşımıza çıkmaktadır. İsterseniz bu kuralı hiçe sayabilir veya uymak için bir şeyler yapabilirsiniz. Tabi söz konusu kuralları esnetmeniz de mümkündür. Nitekim buradaki metric’ ler atlına yenilerini ekleyebilir, yeni gruplar oluşturabilir ve var olan bazı metric’ lerin sorgularını değiştirebiliriz.

Halen söz konusu aracı detaylı bir şekilde incelemekteyim. Kurulum sonrasın zaten Visual Studio 2010, 2008 ve 2005 için de AddIn olarak kullanabileceğinizi görebilirsiniz. Şu sıralar özellikle kodun kaliteli olması noktasında son derece hassas bir dönemimdeyim

![Smile](/assets/images/2010/smiley-smile.gif)

NDepend aracının bu yazıya sığmayacak kadar fazla sayıda özelliği var. Söz gelimi projenin analiz raporu basit bir HTML çıktısı olarak bizlere sunulmakta ama içerisinde inanılmaz derece de derin bilgiler yer almakta. Örneğin aşağıdaki gibi görsel bağımlılık bilgisi içeren görüntüler. Resmen uygulamaların röntgeninin çekildiğini ifade edebiliriz

![Laughing](/assets/images/2010/smiley-laughing.gif)

[![blg227_NDependView](/assets/images/2010/blg227_NDependView_thumb.png)](/assets/images/2010/blg227_NDependView.png)

Hani projeyi analiz etmeye, dökümantasyon çıkartmaya üşenenlerin yardımına koşan araçlardan birisi olduğunu açıkça ifade etmek isterim. Mesajım umuyorum ki gerekli yerlere gider

![Wink](/assets/images/2010/smiley-wink.gif)

NDepend uygulaması ile ilişkili [detaylı bilgiye buradan](http://www.ndepend.com/Features.aspx) ulaşabilirsiniz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.