---
title: "Ruby Kod Parçacıkları 29 - Kod Yazan Kodlar"
pubDate: 2016-12-16 16:51:00
categories:
  - Ruby
tags:
  - Ruby
  - Reflection
---

# Ruby Kod Parçacıkları 29 - Kod Yazan Kodlar
Merhaba Arkadaşlar,

Ruby programlama dilinin öne çıkan özellikleri arasında dinamiklik ve meta programlama yetenekleri de yer almaktadır. Aslında işin özeti kod yazan kodlardır diyebiliriz. Ruby yorumlamalı bir dildir ve doğal olarak her şey çalışma zamanında icra edilmektedir. Ancak bazı senaryolarda metinsel olarak gelen bir içeriğin kod parçası şeklinde değerlendirilmesi ve çalışma zamanında yürütülmesi istenebilir. Hatta çalışan kod üzerinde değişiklikler yapılabilmesi de belli ölçülerde mümkündür. Konu pek tabii benim burada anlatacağım kadar basit değil. Örneğin [Amazon'da bu konu üzerine yazılmış kitap](https://www.amazon.com/Metaprogramming-Ruby-Program-Like-Facets/dp/1941222129/ref=sr_1_1?s=books&ie=UTF8&qid=1481543025&sr=1-1&keywords=ruby+metaprogramming)lar bulabilirsiniz. Bu yazımızda kodu çalışma zamanında üretmek veya çalışan kodu manipule etmek için yapılabileceklerden bir kaçına değinmeye çalışacağız.

> Bazı durumlarda Meta-programlama ile Reflection birbirleri ile karışıtırılır. Normal de Reflection Meta-programlama'nın bir alt dalı olarak karşımıza çıkar ve özellikle yorumlamalı betik dillerde program kodu hakkında çalışma zamanına bilgi sağlamak amacıyla kullanılır. Bir sınıfın özellik adlarını yakalamak gibi. Oysa ki meta-programlama ile çalışan kodun çalışma zamanında değiştirilmesi veya çalışabilecek metinsel kod parçalarının yine çalışma zamanında ayağa kaldırılıp yürütülmesi gibi işlemler kastedilir. Bu açıdan bakıldığında meta-programlama'nın yer bulduğu en önemli alan Domain Specific Language yazılmasıdır.

## Eval ile Çalışma Zamanında Kod İşletmek

İlk örneğimizde eval fonksiyonelliğini ele alarak metinsel bir kod parçasını nasıl çalıştırabileceğimizi anlamaya çalışacağız.

```text
puts 'insert a simple code'
code=gets
eval code

someCodes="puts 'whats your name?'
        name=gets
        puts name.upcase!()
"
eval someCodes
```

İlk olarak çalışma zamanı çıktısına bir bakalım dilerseniz.

![image.axd](images/image.axd)

Sizi bilemem ama ben bu kod parçasına baktığımda epeyce etkileniyorum. İlk olarak kullanıcıdan ekrana bir kod parçası girmesini istiyoruz. Tabii örnekte işletilebilir bir kod parçası eklediğimizi itiraf etmeliyim (Hatalı bir kod parçasının nasıl tepki vereceğini incelemek ise size bir ödev olsun) Sonrasında ise birden fazla satıra yayılan bir kod parçası söz konusu. Önemli olan nokta code ve someCodes değişkenlerinin eval ile birlikte kullanılması. Yani çalışan bir kodun içerisinde başka bir ruby kodunun çalıştırılmasını sağlamış olduk. eval, parametre olarak gelen ifadenin değerlendirilmesini yapıp yorumlanacak şekilde ruby ortamına aktarmakla görevli. Bu arada eval, RubyKernel API'si içerisinde yer alan instance metodlarından birisidir.

## Binding

Bir diğer Kernel metodu ise binding'dir. eval fonksiyonu ile birlikte kullanılır. Temel olarak bir metod ve değişkenlerini program ortamı içerisinde başka bir noktaya bağlayabilmemize olanak sunar. Konuyu açıklamak için aşağıdaki kod parçasını ele alalım.

```text
def doSomething
  puts "in doSomething"
  return binding
end
bind=doSomething{3.times{puts "arigatou"}}
eval "yield",bind
```

ve çalışma zamanı görüntüsü.

![image.axd](images/image.axd)

doSomething metodu içerisinde binding kullanılmıştır. Buna göre çalışma zamanında doSomething metodu ve parametrik yapısı yaklanarak eval ifadesi üzerinden kendisine kod parçası gönderilebilir. Şimdi konuyu dikkatlice inceleyelim. Çalışma zamanında ekrana ilk olarak "in doSomething" yazar. Sonrasında ise 3 kez "arigatou". Bunu 3.times{puts "arigatou"} ifadesinin gerçekleştirdiği aşikardır. Bu ifadenin doSomething metodu içerisine gönderilmesi içinse eval fonksiyonuna iki parametre geçilmiştir. İlki yield anahtar kelimesi, ikincisi ise metodun bağlandığı değişken olan bind (Açıkçası binding konusu bana biraz karışık geldi. Daha iyi bir şekilde öğrenmek için uğraşıyorum)

## Bir Sabitin Çalışma Zamanında Yakalanması

Pek tabii dinamiklik ve meta-programlama söz konusu ise ortada reflection gibi konularda yer alacaktır. Örneğin çalışma zamanında bir değişmezi (Constant) yakalamak için const_get metodundan yararlanılabilir (eval'e göre daha performanslı olduğu ifade edilmektedir) Aşağıdaki kod parçasını ele alalım.

```text
eValue="E"
piValue="PI"
result1=Math.const_get(eValue)*10
result2=Math.const_get(piValue)*10*10
puts result1,result2

pi=eval "Math::PI"
puts pir
```

Bu kod parçasında Math sınıfında yer alan PI ve E değişmezlerinin çalışma zamanında bir string üzerinden yorumlanması örneklenmektedir. eValue ve piValue değişkenleri dikkat edileceği üzere string veri türündedir. Math sınıfı üzerinden const_get metodu kullanılarak bu iki değişmezin değeri yakalanabilir. Dolayısıyla kendi tanımladığımız bir sabitin değerini de bu şekilde çalışma zamanında yakalamamız mümkün. İkinci kod parçasında sabit değerinin eval ile yorumlanması örneklenmiştir. Çalışma zamanına ait çıktımız aşağıdaki gibi olacaktır.

![image.axd](images/image.axd)

## Bir Sınıfı Adından Örneklemek

Dilersek bir sınıfı yine metin olarak gelen adından örnekleyebiliriz..Net tarafında da reflection teknikleri ile yapabildiğimiz bir operasyon olduğunu biliyoruz. Aslında bir sınıfın çalışma zamanında örneklenmesi olarak ifade edebileceğimiz bir durum (Tabii Ruby'de her şey çalışma zamanında gerçekleşiyor bunu da unutmamak lazım) Konuyu aşağıdaki örnekle anlamaya çalışalım.

```text
module Game
  class Player
    attr_accessor :name,:point
    def initialize(name,point)
      @name,@point=name,point
    end
    def to_s
      "#{@name}-(#{@point})"
    end
  end
end

# sinif module icinde oldugundan :: notasyonu kullaniliyor
obj=Object.const_get("Game::Player") #sinif adini alip
o=obj.new("burk",100) #ornekliyoruz
puts o.to_s #ve icindeki bir metodu kullaniyoruz
```

Game modülü içerisinde örnek olarak ele aldığımız Player isimli bir sınıf bulunuyor. Sınıfa sembolik olarak bir kaç nitelik ve ezilmiş to_s metodunu ekledik. Bizi ilgilendiren kısım ise Object sınıfı üzerinden çağırdığımız const_get fonksiyonu. Bu, parametre olarak "Game::Player" şeklinde bir metin almakta. Player sınıfı bir modül içerisine yer aldığından:: notasyonuna başvuruyoruz. Bu satır ile Player tipinden bir örnek üretiliyor ve obj isimli değişkene aktarılıyor. Player.new gibi bir oluşumdan farklı bir şey yaptığımızı fark ediyorsunuz değil mi? Nitekim obj.new ile Player nesnesi oluşturmaktayız evet ama nesne adı string olarak gelmekte. Hatta yapıcı metoda parametrelerini gönderip to_s metodunu da kullanıyoruz. İşte çalışma zamanı çıktıları.

![image.axd](images/image.axd)

## define_method ile Çalışma Zamanında Metod Oluşturmak

Ruby dilini öğrenmeye çalışırken özellikle dinamik olması ve meta-programlama yetenekleri içermesi nedeniyle anlamakta zorlandığım pek çok kısım oluyor. define_method'da bunlardan birisi. Temel olarak çalışma zamanında metod üretebilmemize izin veren bir fonksiyonellik olarak düşünebiliriz. Konuyu öğrenirken ki tek sıkıntım işe yaramayacak olsa da nasıl uygulandığını gösteren bir örnek bulmak oldu. İlk olarak aşağıdaki gibi bir sınıfımız olduğunu düşünelim.

```text
class GameZone
  def title=(zone_name)
    @title=zone_name
  end
  def title
    @title
  end
  def capacity=(player_count)
    @capacity=player_count
  end
  def capacity
    @capacity
  end
  def color=(color)
    @color=color
  end
  def color
    @color
  end
end
rogue_one=GameZone.new
rogue_one.title="Rogue One"
rogue_one.capacity=48
rogue_one.color="black"
puts rogue_one.title,rogue_one.capacity,rogue_one.color
```

Bu kod parçasında GameZone isimli bir sınıf ve bir kaç nitelik metodu tanımı görüyoruz. Aslında title,capacity ve color nitelikleri için attr_accessor da kullanılabilir ki genelde öyle yapılıyor ancak amacımız bu metod ihtiyaçlarının çalışma zamanında nasıl üretilebileceğini görmek. Aşağıdaki kod parçasını inceleyince ne demek istediğimi daha iyi anlayacaksınız.

```text
class GameZoneV2
   PROPERTIES=["title","capacity","color"]
   
   PROPERTIES.each{|p|
     define_method("#{p}="){|i|
       instance_variable_set("@#{p}",i)
     }
     
     define_method("#{p}"){
       instance_variable_get("@#{p}")
     }
   }
end
zone_gold=GameZoneV2.new
zone_gold.title="Gold zone"
zone_gold.capacity=34
zone_gold.color="Gold"
puts zone_gold.title,zone_gold.capacity,zone_gold.color
```

GameZoneV2 içerisinde yine sihirli bir şeyler var. PROPERTIES isimli dizi içerisindeki her bir eleman için getter ve setter metodları define_method yardımıyla çalışma zamanında üretilmekte. each bloğu içerisinde her bir p değişkeni (PROPERTIES elemanı) için iki define_metod çağrısı söz konusu. İlkinde = ile biten setter metod oluşturuluyor ki burada instance_variable_set ile atama bildirimi de yapılmakta. İkinci define_method ile de getter fonksiyonu tanımlamakta. Kodun ilerleyen kısımlarında zone_gold isimli nesne örneği üzerinden title, capacity ve color niteliklerinin kullanılabildiğine şahit oluyoruz. İşte çalışma zamanı sonuçları.

![image.axd](images/image.axd)

Demek ki çalışma zamanında gelecek bir takım parametrelere göre sınıflara farklı operasyonları eklememiz mümkün. Dikkat edin çalışma zamanında diyorum.

## self.class.send Kullanarak metod işletmek

Olayı biraz daha ilginç hale getirmeye ne dersiniz? Mesela sınıfa dahil etmek istediğimiz getter ve setter metodlarının sahibi olacak nitelikleri de dışarıdan verebiliyor olalım. Aşağıdaki kor parçasını bu anlamda ele alabiliriz.

```text
class Person
  def createGetterSetter(*args)
    Array(args).each{|attr|
      self.class.send(:define_method,"#{attr}="){|v|
        instance_variable_set("@#{attr}", v)
      }
      self.class.send(:define_method,"#{attr}") {
        instance_variable_get("@#{attr}")
      }
    }
  end
end
logan=Person.new
logan.createGetterSetter("name","salary")
logan.name="burki"
logan.salary=1250
puts "#{logan.name}-(#{logan.salary})"
```

Person sınıfı içerisinde yer alan createGetterSetter metodu değişken sayıda parametre alabilmektedir. İçinde yer alan each bloğunda her bir argüman için birer getter ve setter metodu tanımlanması sağlanmaktadır. Burada dikkat edilmesi gereken nokta define_method'un kullanım şeklidir. define_method private tanımlanmış bir sınıf fonksiyonudur. Bunu Player örneği üzerinden çağırmak için self.class.send şeklinde bir yol izlenmelidir. Kodun ilerleyen kısımlarında createGetterSetter metoduna örnek iki eleman yollanmış ve kullanılmıştır.

![image.axd](images/image.axd)

Aynı örnekte işi biraz daha ileriye götürebiliriz. Zaten attr_accessor bir nitelik için gerekli getter ve setter operasyonlarını hazır olarak sunmaktadır. Peki aynı örnekteki nitelikleri attr_accessor ile tanımlayabilir miyiz? Tabii çalışma zamanında. Aşağıdaki kod parçası işimizi görecektir.

```text
class Person
  def createAttr(*args)
    Array(args).each{|attr|
      self.class.send(:attr_accessor,"#{attr}")
    }
  end
end
logan=Person.new
logan.createAttr(:name,:salary)
logan.name="burki"
logan.salary=1250
puts "#{logan.name}-(#{logan.salary})"
```

Yine self.class.send metodunun kullanıldığına dikkat edelim. Bunun dışında createAttr metoduna:name ve:salary isimli iki symbol göndermekteyiz.

Görüldüğü gibi metinsel olarak gelen bir kod parçasını çalışma zamanında işletmemiz mümkün. Bir sınıfa çalışma zamanında gelecek bilgiler ve yönergeler doğrultusunda yeni fonksiyonellikler katmamız da söz konusu. Bir sınıfı adından yola çıkarak örneklememiz veya içerisindeki değişmez değeri yakalamamız da mümkün. Daha yapılabilecek pek çok şey olduğu da ortada. Bunları ilerleyen bölümlerimizde incelemeye gayret edeceğim. Böylece geldik bir kod parçasının daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
