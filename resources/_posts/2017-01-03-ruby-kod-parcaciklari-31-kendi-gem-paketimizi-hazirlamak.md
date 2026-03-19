---
layout: post
title: "Ruby Kod Parçacıkları 31 - Kendi gem Paketimizi Hazırlamak"
date: 2017-01-03 22:30:00 +0300
description: "Bildiğiniz gibi günümüz popüler programlama dillerinin çoğunun internet üzerinden ulaşılabilen paket destekleri mevcut. Özellikle açık kaynak tabanlı ürünlerde önem verilen bir konu olduğu ortada. Yazılımcıların birbirlerinin kullanımına açtığı bu paketler Ruby tarafında da mevcut ve gem olarak adlandırılmakta. Hatta şu adresten bir çok değerli mücehvere ulaşabilir kendi paketlerinizi de yükleyebilirsiniz. Peki kendi gem paketlerimizi en basit haliyle nasıl yazabiliriz? Hatta yazdığımız bir gem'i herkesin kullanımına nasıl açabiliriz? Gelin bu konuyu kısaca incelemeye çalışalım."
categories:
  - ruby
tags:
  - ruby
  - bash
  - http
  - testing
---
# Ruby Kod Parçacıkları 31 - Kendi gem Paketimizi Hazırlamak
![rubygems_8.gif](/assets/images/2017/rubygems_8.gif)

Merhaba Arkadaşlar,

Bildiğiniz gibi günümüz popüler programlama dillerinin çoğunun internet üzerinden ulaşılabilen paket destekleri mevcut. Özellikle açık kaynak tabanlı ürünlerde önem verilen bir konu olduğu ortada. Yazılımcıların birbirlerinin kullanımına açtığı bu paketler Ruby tarafında da mevcut ve gem olarak adlandırılmakta. Hatta [şu adresten](https://rubygems.org/) bir çok değerli mücehvere ulaşabilir kendi paketlerinizi de yükleyebilirsiniz. Peki kendi gem paketlerimizi en basit haliyle nasıl yazabiliriz? Hatta yazdığımız bir gem'i herkesin kullanımına nasıl açabiliriz? Gelin bu konuyu kısaca incelemeye çalışalım.

> Bazı firmalarda katı güvenlik kuralları olabilir. Örneğin bir bankanın geliştirme ortamlarından, NuGet, RubyGems gibi küresel paket sağlayıcılarına erişim izni verilmeyebilir. Bu gibi hallerde güvenlik denetimlerinden geçmiş olup geliştiricilerin kullanabileceği paketlerin sunulması için şirket ağı üzerinde paket depoları oluşturulabilir.

## Paket için Klasör Yapısı

Bir gem üretmeden önce belli bir ağaç yapısını sistem üzerinde oluşturmamız gerekir. Aşağıdaki ekran görüntüsünde bu işin en yalın halini görmektesiniz. Ancak pakete ilişkin teknik dokümantasyon, paketle birilikte kullanılacak yürütülebilir dosyalar gibi diğer kaynaklara da ihtiyaç varsa farklı klasörleri konuşlandırmak gerekir. İcra edilebilir (Executable) dosyalar için bin, Unit Test'ler için test isimli klasörler vb.

![rubygems_4.gif](/assets/images/2017/rubygems_4.gif)

## Paket İçeriğini Hazırlayalım

Paketimizin adı Einstein. İçerisinde lib isimli bir klasör ve gemspec uzantılı bir dosya var. Kodlarımızı lib klasörü içerisinde tutmamız gerekiyor. Einstein.rb dosyasına ait kod içeriği aşağıdaki gibi oluşturulabilir.

```bash
#Einstein Gem
module Algebra
	class Common
		def self.factorial(number)
			if(number==0)
				return 1
			else
				return number*factorial(number-1)
			end
		end
		
		def self.sum(*numbers)
			total=0
			numbers.each{|n|total+=n}
			total
		end
	end
end
```

Sadece pratik olması açısından iki fonksiyonellik sunduğumuz basit bir sınıf söz konusu. Bir sayının faktöryelini ve n sayıda rakamın toplamını hesap eden operasyonlar Algebra modülündeki Common sınıfında yer alıyor (self kullanımı nedeniyle sınıfı örneklemeden ilgili fonksiyonellikleri çağırabileceğimizi hatırlatalım) Önemli olan kısım gemspec uzantılı dosya içeriği. Paket ile aynı isimde olan dosya da aslına bakarsanız ruby kodları içermekte. Dikkat edilecek olursa Specification tipinden yeni bir nesne oluşturuyor ve bazı niteliklerine değerler atılıyor.

```text
Gem::Specification.new do |s|
  s.name        = 'BuraksEinstein'
  s.version     = '0.0.2'
  s.date        = '2016-12-20'
  s.summary     = "Simple and funny math operations for kids"
  s.description = "A Simple and Funny Math"
  s.authors     = ["Burak Selim Senyurt"]
  s.email       = 'selim@buraksenyurt.com'
  s.files       = ["lib/einstein.rb"]
  s.homepage    = 'http://rubygems.org/gems/einstein'
  s.license     = 'MIT'
end
```

Bu dosyayı paket ile ilgili temel bilgileri içeren bir manifesto olarak düşünebilirsiniz. Paket adı, versiyon numarası, ne yaptığının özeti, kimin yazdığı, irtibat için elektronik posta adresi, lisanslama modeli ve içerisinde kullanılan kod dosyası vb pek çok bilgi bu dosya içerisinde bildirilmekte.

## Paketin Üretilmesi

Kod ve manifesto içerikleri hazırlandıktan sonra gem aracını kullanarak paketin oluşturulması ve test adımlarına geçilebilir. Komut satırından yapacaklarımız aşağıdaki ekran görüntüsünde olduğu gibidir.

![rubygems_3.gif](/assets/images/2017/rubygems_3.gif)

Dilerseniz neler olduğuna sırasıyla bakalım. İlk olarak bir build işlemi gerçekleştirmekteyiz. Bu işlem sonrasında gem uzantılı bir dosya oluşur. manifesto içerisinde belirtilen versiyon numarasına göre farklı gem içerikleri üretilebilir. Bu sayede kütüphanenin kullanılacağı platform için farklı sürümlerin değerlendirilmesi de mümkün olabilir (Özellikle güncellenen ürünlerin eski sürümleri ile bir süre daha paralel çalışması gerektiği hallerde önem kazanan bir uygulamadır) gem için build komutu sonrası bunu kendi sistemimizde denemek adına install işlemi gerçekleştiriyoruz (3 numara) Dikkat edilecek olursa paket adını yazarken versiyon numarasını da belirttik (Bir başka deyişle sistemimize farklı versiyonları da yükleyebiliriz) Paketin oluşturulması yeterli değil. Bunu kolay bir şekilde deneyip çalıştığından emin olmakta yarar var. irb üzerinden factorial metodunu kullanarak 8 sayısının faktoryelini hesaplıyoruz (İşte test klasörünün oluşturulması ve test metodlarının icra edilmesi bu noktada anlam kazanan bir durum. Yazımızda yer vermedik ancak kaynak bağlantıdan bakarak konu hakkında detay bilgi alabilirsiniz.)

## Paketi RubGems.org Sitesine Aktarmak

Buraya kadar her şey yolunda gitti. Kodlarımızı kendi sistemimizde gem paketi haline getirip başarılı bir şekilde denedik. Peki bunu herkesin ortaklaşa kullanabileceği paket sunucusuna nasıl atabiliriz? Öncelikle rubygems.org'a üye olmamız ve bizim için üretilecek uygulama anahtarına (rubygemsapikey olarak geçiyor) ihtiyacımız var. Üyelik işlemi sonrası ilgili anahtarı üretmek için komut satırından curl aracından yararlanabilir veya paketin ilgili sunuculara gitmesi için gerekli credentials dosyasının içeriğini manuel olarak çekebiliriz. Tek yapmamız gereken üyelik işlemi sonrası [https://rubygems.org/api/v1/api_key.yaml](https://rubygems.org/api/v1/api_key.yaml) adresine gidip kullanıcı adı ve şifre bilgisini girdikten sonra indirilen yaml içeriğini credentials dosyasına koymak. Eğer her şey yolunda giderse aşağıdaki ekran görüntüsünde olduğu gibi paketin sunucuya atıldığını görebiliriz. Bunun için gem aracını push komutu ile birlikte kullanmamız yeterlidir.

![rubygems_5.gif](/assets/images/2017/rubygems_5.gif)

> Artık BuraksEinstein isimli paketimiz rubygems sitesinde kendisine bir yer buldu. Aslında örneği Einstein isimli bir paketi atacak şelilde geliştirmiştim ancak bu isimde zaten bir çok paket olduğundan hata aldım. Paket adının BuraksEinstein olmasının sebebi bu.

![rubygems_6.gif](/assets/images/2017/rubygems_6.gif)

Pek tabii bu gem'in de kullanılıp kullanılmadığını denememiz lazım. Bu yüzden ilgili paketi sistemimize yüklemeyi deneyerek ilerleyelim. Sonrasında da irb'den bir test yapalım.

![rubygems_7.gif](/assets/images/2017/rubygems_7.gif)

Görüldüğü gibi yazdığımız bir gem paketini RubyGems.org sitesine yükleyebildik. Ruby ile geliştirdiğimiz paketleri bu şekilde ekleyebilir ve başka geliştiricilerin kullanımına açarak diğer Rubyist'leri sevindirebiliriz. Elbette işe yarar paketler yazmamı önemli. Buradaki tek eksi sanıyorum ki gem içeriklerinin kontrol edilemeyişi. Yani kötü amaçlı gem paketleri de bu sunuculara atılabilir mi bilemiyorum. Böylece geldik bir makalemizin daha sonuna. Bir başka kod parçacığında görüşünceye dek hepinize mutlu günler dilerim.

Kaynak: [http://guides.rubygems.org/make-your-own-gem/](http://guides.rubygems.org/make-your-own-gem/)