---
title: "Ruby Kod Parçacıkları 34 - Struct ve OpenStruct"
pubDate: 2017-04-10 15:00:00
categories:
  - Ruby
tags:
  - Ruby
  - MySQL
  - HTTP
---

# Ruby Kod Parçacıkları 34 - Struct ve OpenStruct
![image.axd](images/image.axd)

Merhaba Arkadaşlar,

Bir süredir şirket içinde vereceğim Ruby eğitimine hazırlanmaktayım. İşlerden çok vakit kalmasa da önceki Ruby notlarımı ve farklı kaynakları takip ederek 101 seviyesinde bir içerik oluşturmaya çalışıyorum. Gün içinde C# evde geç vakitlerde ise Ruby. Biraz yorucu olsa da oldukça keyifli aslında. Hem yeni bir şeyler öğreniyorum hem de iç eğitim gibi bir gerçek olduğundan ciddi anlamda not çıkartıyorum. Bugün konular üzerinden geçerken struct ve openstruct kavramlarını atladığımı fark ettim (Ov yooo) Tabii hemen öğrenmeye başladım. Neymiş ne için kullanılırmış biraz fikir sahibi oldum. İşte notlarım.

Struct aslında Ruby'nin built-in sınıflarından birisi. Temel olarak bir sınıf hazırlamadan nitelik ve değer barındıran tip tanımlanmasına olanak sağlıyor. Konuyu basit bir şekilde incelemeye başlamak için aşağıdaki kod parçasını göz önüne alarak ilerleyebiliriz.

```text
player=Struct.new :firstName,:lastName,:level
dam=player.new()
dam.firstName="jan kulod van"
dam.lastName="dam"
dam.level=900
puts "#{dam.lastName}, #{dam.firstName}-[#{dam.level}]"

obiWan=player.new("kenobi","obi wan",850)
puts "#{obiWan.lastName}, #{obiWan.firstName}-[#{obiWan.level}]"
```

![image.axd](images/image.axd)

Örnek kodda player isimli bir yapı bildirimi yer alıyor. İlk tanımlama sırasında new operatörünü takiben bu veri yapısına dahil olan niteliklerin bildirimi yapılmakta. Kodumuzda aynı veri modeline sahip iki Struct değişkenine yer veriliyor. dam ve obiWan:) İlk kullanımda nitelik değerleri new operatöründen sonra atanmıştır. obiWan isimli değişken örneklenirken de ilgili nitelik değerleri new fonksiyonunda belirtilmiştir.

Yapıları tanımlarken do...end bloklarını da işin içerisinde dahil edebilir ve bu sayede çeşitli fonksiyonlar içermesini de sağlayabiliriz. Aşağıdaki kod parçasında bu durum örneklenmektedir.

```text
book=Struct.new :title,:price,:category,:author do
  def getInfo
    "#{title} from #{author}. #{price},#{category}"
  end
end
tehlikeliOyunlar=book.new
tehlikeliOyunlar.title="Tehlikeli Oyunlar"
tehlikeliOyunlar.price=50
tehlikeliOyunlar.author="Oguz Atay"
tehlikeliOyunlar.category="Turk Edebiyati"
puts tehlikeliOyunlar.getInfo
```

![image.axd](images/image.axd)

book isimli yapı yazılırken do end blokları arasında getInfo isimli bir metod tanımına da yer verilmiştir. getInfo metodu sadece yapının niteliklerini string formunda geriye döndürmektedir. Burada sınıflardan farklı olarak niteliklere erişirken @ işaretinin kullanılmadığı gözden kaçırılmamalıdır.

Aslında bu ve bir önceki örnekleri göz önüne alırsak benzer veri yapılarını sınıf olarak tanımlamak istediğimizde aşağıdakine benzer bir yol izlememiz gerektiği ortadadır.

```text
class Player
  attr_accessor :firstName,:lastName,:price

  def initialize(firstName,lastName,price)
    @firstName,@lastName,@price=firstName,lastName,price
  end  
  
  def getInfo
    "#{@firstName},#{@lastName},#{@price}"
  end
end
```

Dikkat edileceği üzere attribute tanımlamaları ve new operatörü için initialize metodunun yazımı zorunludur. Yapılar bu noktada daha pratik bir veri modeli tanımlama yolu sunmaktadır. Nitekim bir yapı initialize metodu içermemesine rağmen new operatöründe içerdiği niteliklerine değer ataması yapılabilir.

Yapılar ile ilgili dikkat çekici bir diğer nokta da OpenStruct tipinin kullanımıdır. Bu tip kullanılırken niteliklerinin baştan belirtilmesine gerek yoktur. Yani yapı istenildiği kadar nitelik barındırabilir.Nasıl mı? Aynen aşağıdaki kod parçasında görüldüğü gibi.

```text
require "ostruct"

parameters=OpenStruct.new()
parameters.connection="provider=mysql..."
parameters.username="bsenyurt"
parameters.password="****"
parameters.timeout=6000
puts parameters.timeout
parameters.ftpAddress="ftp://localhost/images/"
puts parameters.ftpAddress
```

OpenStruct için ostruct bildirimi gereklidir. Sonrasında yine new operatöründen yararlanılarak bir yapı tanımlanmıştır. parameters yapısına istediğimiz kadar nitelik atayabiliriz. Örnekte programlarımızda sıklıkla başvurduğumuz ve sayısı genellikle belli olmayan parametre modeli sembolize edilmeye çalışılmıştır. Tahmin edileceği üzere OpenStruct kendi içerisinde bir hash kullanır. Bunu new ile oluşturulduğu sırada örnekler. Hash, nitelik:değer eklendikçe genişlemeye olanak sağlar.

Peki bir yapının bu örneklerde olduğu gibi n sayıda niteliğinin tamamına kolayca erişmenin bir yolu yok mudur? Tabii ki vardır. Meşhur each metodumuz ne güne duruyor. İşte örnek bir kaç kullanım.

```text
require "ostruct"

parameters=OpenStruct.new()
parameters.connection="provider=mysql..."
parameters.username="bsenyurt"
parameters.password="****"
parameters.timeout=6000
parameters.ftpAddress="ftp://localhost/fileServer"
parameters.each_pair{|key,value| puts "#{key}->#{value}"}

player=Struct.new :firstName,:lastName,:level
obiWan=player.new("kenobi","obi wan",850)
obiWan.each{|o|puts o}
obiWan.each_pair{|key,value| puts "#{key}->#{value}"}
puts obiWan[:firstName]
```

![image.axd](images/image.axd)

Bu örnekte each, each_pair ve [] operatörü kullanımları örneklenmiştir. each ile bir yapının tüm nitelik değerlerine erişmemiz mümkündür. each_pair tahmin edileceği üzere key:value benzeri yapının nitelik adı ve değerlerine erişmekte kullanılır. İstersek bir yapının elemanlarına indeksleyici ile de ulaşabiliriz. Aslında bir yapının diziye veya hash nesnesine atanması da oldukça kolaydır. Hatta select fonksiyonundan yararlanarak bir yapının taşıdığı değerler üzerinde koşullu seçimler yapılması da sağlanabilir (Detaylar için [bu](https://ruby-doc.org/core-2.2.0/Struct.html) ve [şu](http://ruby-doc.org/stdlib-2.0.0/libdoc/ostruct/rdoc/OpenStruct.html) adreslerdeki Ruby dokümanlarına bakmanızı öneririm)

Peki yapıların bu pratik kullanımları nedeniyle sınıflar yerine tercih edilmeleri gerekir mi? Aslında yapıların kullanım sebepleri biraz daha farklıdır. Çoğunlukla geçici bir veri yapısına (Temporary Data Structure) ihtiyaç duyduğumuzda yapılardan yararlanabiliriz. Ya da test ortamında stub nesne ihtiyacı olduğunda kullanabiliriz. Bir diğer kullanım şeklide sınıf içerisinde dahili veri modeline ihtiyaç duyduğumuz durumlardır. Aşağıdaki kod parçasında bu durum örneklenmeye çalışılmıştır.

```text
class Employee
  attr_accessor :firstName,:lastName, :address
  Address = Struct.new(:street, :city, :country, :postal_code)

  def initialize(firstName,lastName, addressInfo)
    @firstName,@lastName=firstName,lastName
    @address = Address.new(addressInfo[:street], addressInfo[:city], addressInfo[:country], addressInfo[:postal_code])
  end

end

sitiv = Employee.new("Sitiv","Jobs", {
  street: "hevan street",
  city: "Second Parallel New York",
  country: "Beauty Country",
  postal_code: "HVN-1001"
})

puts sitiv.address.inspect
```

![image.axd](images/image.axd)

Bu örnekte Employee sınıfı içerisinde Address isimli bir yapı tanımlanmıştır. Employee sınıfına ait bir nesne örneklenirken initialize metoduna gelen son parametre bu yapıya ait bir değişken içeriğidir. Indeksleyici operatörü ile değerler alınıp Address yapısına ait değişken nitelikleri doldurulmuştur.

Görüldüğü gibi yapılar oldukça pratik kullanıma sahip bir veri tipi olarak karşımıza çıkmaktadır. Built-In olarak sınıf kökenli olan bu tipin kullanışlı fonksiyonları bulunmaktadır. Böylece geldik bir Ruby Kod Parçacığının daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
