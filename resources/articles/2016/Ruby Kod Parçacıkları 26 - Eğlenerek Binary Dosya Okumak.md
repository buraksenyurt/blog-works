---
title: "Ruby Kod Parçacıkları 26 - Eğlenerek Binary Dosya Okumak"
pubDate: 2016-11-14 21:30:00
categories:
  - Ruby
tags:
  - Ruby
  - NoSQL
---

# Ruby Kod Parçacıkları 26 - Eğlenerek Binary Dosya Okumak
Merhaba Arkadaşlar,

Vakit buldukça Ruby programlama dili ile ilgili bir şeyler yapmaya çalışıyorum. Halen daha dilin kabiliyetlerini tanıma aşamasındayım. Geçtiğimiz günlerde de Ruby Cookbook kitabından binary dosyalar üzerinde yapılan işlemlere ait örnekleri inceliyordum. Hoşuma giden uygulamalardan birisi de MP3 dosyalarına ait Tag bilgilerinin elde edilmesiydi. Gerçek hayat örneği olduğundan benim için daha öğretici idi. Örnekleri inceledim ve kendime göre farklılaştırarak basit bir kod parçası oluşturmaya çalıştım.

Amaç bir MP3 dosyası üzerinde eğer varsa kayıtlı Tag bilgilerini elde etmek. Tag bilgilerine bakarak şarkının adını, söyleyen sanatçı veya grubu, yayınlandığı yılı ve içinde bulunduğu albüm bilgilerini elde etmek mümkün. Hatta albümdeki sıra numarası, türünü, başlangıç ve bitiş sürelerini dahi elde edebiliyoruz. Tabii MP3 dosya formatlarına ve ID3 standartlarına göre farklılıklar söz konusu olabiliyor. Uzatılmış tag yapısı gibi bir kavram ve ID3 ün farklı versiyonları var. Buna göre farklı bilgileri de elde etmek mümkün. Lakin ben 128 byte'lık segment yapısına sahip dosyaları ele almaya çalıştım ([ID3 formatına ait detaylı bilgi almak için wikipedia adresine bakmanızı öneririm](https://en.wikipedia.org/wiki/ID3))

Önce kod içeriğine bir bakalım ve sonrasında neler öğrendiğimi sizlere aktarayım.

```text
class TagInfo
	attr_accessor :track_name,:artist_name,:album_name,:year
	
	def initialize(track_name,artist_name,album_name,year)
		@track_name,@artist_name,@album_name,@year=track_name,artist_name,album_name,year
	end
	
	def to_s
		"#{@artist_name}-#{@album_name}-#{@track_name} (#{@year})"
	end
end

def get_tag_info(mp3)
	info=nil
	open(mp3) do |f|
		f.seek(-128,File::SEEK_END)
		if f.read(3)=="TAG"
			info=TagInfo.new(
				f.read(30),
				f.read(30),
				f.read(30),
				f.read(4)
			)
		end
	end
	return info
end

songs=Dir.glob("*.mp3")
songs.each{
	|s|
	puts get_tag_info(s).to_s
	}
```

Kodumuz TagInfo isimli bir sınıf tasarımı ile başlıyor. Bana göre MP3 şarkısının tag yapısı sistem içerisinde bir varlık olarak ifade edilmeli (Hatta daha geniş kapsamlı düşünüsek MP3 dosya içeriğini ve ilgili Tag bilgilerini barındıran bir sınıf tasarımı da mümkün olabilir. Bu sınıfa ait nesne örnekleri NoSQL tabanlı bir yapıda saklanabilir. Alın size db tabanlı bir müzik seti) Ben çok daha yüzeysel basit bir sınıf tanımladım. İçerisinde dört nitelik yer alıyor. track, artist, album ve year bilgilerini tutmayı planlıyorum. Örnek çıktılarını ekrana bastırmak gibi bir amacım da olduğundan to_s metodunu eziyorum (Bu arada initialize metodu içerisinde çoklu atama işlemi gerçekleştirdiğimize dikkat edelim)

Kodun önemli operasyonu tabii ki get_tag_info isimli metod. MP3 dosyaları normalde binary olarak okunabiliyorlar. Standartlara göre 128nci byte'tan sonra gelen ilk 3 byte içerisinde TAG yazıyorsa bahsettiğimiz bilgilere ulaşma şansımız var. Öncelikle dosyayı okuma modunda açmamız gerekiyor. open fonksiyonunu bu amaçla kullanıyoruz. Fonksiyona geçecek blokta dosyanın kendisini f ile işaret etmekteyiz. seek metodu 128nci byte'tan itibaren dosya sonuna kadarlık kısmı almamızı sağlayacak. Yani dosya üzerindeki işaretçiyi 128nci byte'a aldığımızı düşünebiliriz. read metoduna verilen parametre ile ne kadarlık bilgi okuyacağımızı belirtiyoruz. Buna göre read (3) ile aradığımız TAG bilgisinin olduğu bölümü okumaktayız. Okuma işlemi sonrası dosya üzerindeki konumumuz 3 byte ilerlemiş olacak. Eğer buradaki içerik TAG ise sonrasında ID3 standartına göre dosya üzerindeki konumumuzu ileri doğru hareket ettirerek bir okuma gerçekleştiriyoruz.

İlk 30 byte ile track_name, sonrasında gelen 30 byte ile artist_name, sonrasında gelen 30 byte ile album_name ve sonrasında gelen 4 byte ile de year bilgisini okumaktayız. Okuduğumuz bilgileri kullanarak bir TagInfo nesne örneği oluşturuyor ve metodumuzdan geriye döndürüyoruz.

Tabii kodu uygulamak için MP3 şarkılarının olduğu bir konumda çalıştırmak gerekiyor (Siz kendi örneğinizi yaparken klasör bilgisini programa parametre olarak almayı denemelisiniz) İşte o anda bir klasör ve içerisindeki MP3 uzantılı dosyaları nasıl çekebileceğimi öğrendim. Dir tipine ait glob metoduna geçirilecek desen ile bu işlemi gerçekleştirmemiz mümkün. sonrasında each metodunu çağırıyor ve blok içerisinde get_tag_info fonksiyonunu kullanarak MP3 dosyalarına ait bilgileri ekrana basıyoruz. Örnek şarkılarıma ait klasör içeriğini tarattığımda aşağıdaki gibi bir çıktı elde ettim.

![image.axd](images/image.axd)

Görüldüğü gibi Binary dosya içeriklerini iler yönlü okumak ve içlerinden bilgi almak son derece kolay. open fonksiyonu haricinde seek ve read metodlarının kullanımı da önemli. Örneğimizde sınıf oluşturulması ve Dir tipine ait globe metodunun kullanılmasını da öğrenmiş bulunuyoruz. Bilgilerimiz çoğaldıkça Ruby ile kodlama daha da eğlenceli hale gelmeye başlıyor. Bir başka kod parçasında görüşünceye dek hepinize mutlu günler dilerim.
