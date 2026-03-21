---
layout: post
title: "Ruby Kod Parçacıkları 25 - Unit Test Yazmak"
date: 2016-11-07 21:28:00 +0300
categories:
  - ruby
tags:
  - ruby-lang
  - unit-test
  - testing
  - assertion
  - test-driven-development
  - assert
---
İyi yazılım geliştirmenin olmazsa olmaz parçalarından birisi de birim testleridir (Unit Tests) Tahmin edileceği üzere Test Driven Development denildiğinde (aklımızda hemen Red Green Blue - Fail, Fix, Refactoring oluşmalı) birim testleri ön plana çıkmaktadır. Pek çok programlama ortamında birim testler için hazır kütüphaneler yer alır. Ruby dilinde ise standart kütüpanede yer alan Test::Unit modülü kullanılmaktadır. Testlere dahil edilecek savlar (assert diyelim) oldukça geniştir. assert, assert_not_equal, assert_nil, assert_respond_to, assert_raise ve benzeri fonksiyonellikler söz konusudur ([Kullanılabilecek savların listesine şuradan bakabilirsiniz](https://ruby-doc.org/stdlib-2.1.1/libdoc/test/unit/rdoc/Test/Unit/Assertions.html)) Aşağıdaki kod parçasında basit bir test düzeneğine yer verilmiştir.

```text
require 'test/unit'

class Product

	attr_accessor :title, :listPrice, :category
	
	def initialize(title,listPrice,category)
	
	       raise ArgumentError,"#{listPrice} must be greater than 10" unless listPrice>10
	       @title=title
	       @listPrice=listPrice
	       @category=category
		       
	end
end
class ProductTest < Test::Unit::TestCase             
                
	TITLE,LIST_PRICE,CATEGORY="Head First Design Patterns",39.99,"Book"            
	
	def setup
	       @product=Product.new(TITLE,LIST_PRICE,CATEGORY)
	end        
	
	def testForTitle
	       assert_not_nil @product.title
	       assert_equal TITLE,@product.title
	end
	
	def testForListPrice
	       assert_equal 39.99,@product.listPrice
	       assert_raise(ArgumentError){Product.new(TITLE,9.99,CATEGORY)}
	end
	
	def testForCategory
	       assert_send([["Book","Computer","Camera"], :include?, @product.category]) 
	       assert_send([["Book","Computer","Camera"], :include?, Product.new(TITLE,LIST_PRICE,"Unknown").category]) 
	end
                
end
```

Program kodu test/unit modülünün kullanılacağını belirten require bildirimi ile başlar. Örneğimizde Product isimli bir kobay sınıf yer almaktadır. Sınıfı belirleyen üç nitelik var. Ürün adı, liste fiyatı ve yer aldığı kategori. Amacımız Unit Test'lerin basitçe nasıl yazıldığını görmek. Bu yüzden çok karmaşık test vakalarına girmedik. Tüm savlar ProductTest sınıfı içerisinde yer alan testForFile,testForListPrice ve testForCategory fonksiyonları ile icra edilmekte.

ProductTest sınıfının test metodları içerdiğini ve çalışma zamanında test sürecine dahil olacağını belirtmek içinse Test::Unit::TestCase sınıfından bir türetme söz konusudur. setup fonksiyonu var olan test senaryolarından önce çalışan bir metoddur. Tahmin edileceği gibi bu sınıf içerisinde test metodları boyunca kullanılacak ortak nesnelerin üretimi veya ön hazırlık işlemleri yapılabilir. Örnekte bir Product nesnesini üretip test vakalarının kullanımına sunuyoruz.

testForTitle metodunda Title niteliği için iki sav deneniyor. Bunlardan birisi nil kontrolü diğeri ise nitelik değerinin "Head First Design Pattern" metnine eşit olup olmadığı. testForListPrice metodunda da benzer bir eşitlik kontrolü var. Ayrıca liste fiyatının 10 birimin altında olması durumunda almayı beklediğimiz ArgumentError istisnasının kontrolü de bulunuyor. Yani 10 birimin altında bir değer verildiğinde ortama ArgumentError fırlatılmasını bekliyoruz. Nitekim Product sınıfının initalize metodu içerisinde bu tip bir exception kontrolü söz konusu. Eğer liste fiyatı beklenen değerin altında ise ortama fırlatılacak bir hata var. assert_raise bu hatanın fırlatılacağı vakayı test ediyor. Savımızın kabul kriteri 10un altında bir değer verilmesi halinde ortama hata yollanması. testForCategory metodunda ise assert_send isimli bir çağrım yer alıyor. Burada kategorinin ilk parametre ile gelen dizi içerisindeki bir eleman olması durumu test edilmekte. Metodun ilk parametresinde kullanılabilecek kategorilere ait dizi yer alıyor. İkinci parametre array sınıfı üzerinden kullanılabilecek ve include? metodu ve son parametrede include?'a verilebilecek olan değer. İkinci satırdaki kodda sav'ın geçersiz olmasına ait bir kriter kontrol ediliyor. Hepsi bu. Dilerseniz çalışma zamanı sonuçlarına da bir bakalım.

![utest_1.gif](/assets/images/2016/utest_1.gif)

Product içerisinde yer alan tüm test metodları sırasıyla çalıştırılmıştır. 3 test metodu ve içerisinde yer alan 6 assert icra edilmiştir. Bu varsayımlardan beklediğimiz gibi hatalı kateogori vakasının kodun hangi satırında oluştuğu terminal penceresine dökülmüştür. Dilersek bir veya n sayıda test metodunu da çalıştırabiliriz. Nitekim bazı test sınıfları içerisinde fazla sayıda test metodu olabilir ve o an için sadece bellir birini (veya bir kaçını) test etmemiz gerekebilir. Sonuçta test operasyonları da zaman zaman süre ve kaynak kullanımı adına maliyetli olabilir. Böyle bir durumda verbose --name testMetodu dizilimini kullanabiliriz. Aşağıdaki ekran görüntüsünde örnek bir kullanım yer almaktadır.

![utest_2.gif](/assets/images/2016/utest_2.gif)

Bu kez sadece testForTitle isimli test vakası çalıştırılmıştır. Görüldüğü gibi Unit Test'ler oluşturmak oldukça pratik ve kolay. İşin önemli olan kısmı test vakalarını oluşturabilmek ve Test Driven Development yaklaşımının benimsediği şekilde kodları geliştirebilmek. Böylece geldik bir kod parçamızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.