---
title: "Ruby Kod Parçacıkları 32 - Singleton"
pubDate: 2017-02-03 21:23:00
categories:
  - Ruby
tags:
  - Ruby
---

# Ruby Kod Parçacıkları 32 - Singleton
![rfun.gif](images/rfun.gif)

Merhaba Arkadaşlar,

Ruby'nin oldukça geniş bir program kütüphanesi bulunuyor. Fırsat buldukça bazılarını incelemeye çalışıyorum. Geçtiğimiz günlerde Singleton isimli bir modüle rastladım. Kısaca tasarım kalıplarından (Design Patterns) olan Singleton deseninin kolayca uygulanmasını sağlıyor.

Ancak dikkat çekici başka bir özelliği daha var. Nesne durumunun saklanması ve istenen anda saklanan duruma döndürülmesi vakasında da Marshal modülü ile birlikte güzel bir işbirliği içerisinde. Marshalling ile Singleton olan bir nesne örneğinin anlık durumunu (State) sonradan geri yükleme imkanıyla saklayabiliyoruz.

## Singleton Tasarım Kalıbının Uygulanması

Öncesinde Singleton tasarım kalıbının amacını hatırlamakta yarar var: Bir sınıfa ait nesne örneğinin çalışma zamanında tek olmasını garantilemek. Örneğin yazdığımız uygulama ayağa kalkarken çeşitli konfigurasyon ayarlarını yükleyen farklı modüldeki bir sınıfı kullanıyor olsun. Sınıfın çalışma zamanında tek bir örneğinin olmasını ve herhangi bir şekilde çoğaltılmamasını (hatta klonlanmamasını) istersek, Singleton deseni bizim için ideal çözüm olacaktır.

Ruby tarafında bir sınıfın Singleton kalıbını uygular hale getirilmesi ise oldukça kolay. Singleton modülünü sınıfa dahil etmemiz yeterli. Aşağıdaki örnek kod parçasını ele alalım.

```text
require "Singleton"

class ConfigurationManager
  include Singleton
  
  attr_accessor :default_host,:default_port,:entry_point
  
  def get_default_host
    "tcpip://#{@default_host}:#{@default_port}/#{@entry_point}"
  end
end

mngr1=ConfigurationManager.instance
mngr1.default_host="localhost"
mngr1.default_port="4500"
mngr1.entry_point="mainProcess"

puts "mngr1 object id =#{mngr1.object_id}"
puts mngr1.get_default_host

mngr2=ConfigurationManager.instance
puts "mngr2 object id =#{mngr2.object_id}"
puts mngr2.get_default_host

puts mngr1==mngr2
```

reuqire bildirimi ile Singleton modülünü script'e dahil ediyoruz. ConfigurationManager sınıfı içerisindeki include ifadesi ile de Singleton modülünü uygulayacağını belirtiyoruz. Sınıfın üç niteliği bulunuyor. Bunların değerlerini düzgün bir formatta get_default_host metodunu kullanarak geriye döndürüyoruz.

Kodun ilerleyişinde ilk olarak mngr1 isimli nesne örneğini oluşuyor. Burada dikkat edilmesi gereken nokta instance özelliğinin kullanılması. Normalde bir sınıf örneğini new metodundan yararlanarak üretiriz. Ancak Singleton bir nesne söz konusu ise new metoduna yapılan çağrı aşağıdaki hata mesajının üretilmesine neden olur.

```text
in `<main>': private method `new' called for ConfigurationManager:Class (NoMethodError)
```

Kodda üretilen her iki ConfigurationManager nesne örneği de aynı object_id değerlerine sahiptir. Hatta bu iki nesne aynıdır ki eşitlik sonrası kod true değeri döndürmüştür. Bir başka dikkat çekici nokta ise, mngr1 üretimi sırasında atanan nitelik değerlerinin mngr2 için de geçerli olmasıdır. Kodun çalışma zamanı çıktısı aşağıdaki gibidir.

![rsingleton_1.gif](images/rsingleton_1.gif)

Çok doğal olarak ConfigurationManager tipinden n sayıda nesne örneği üretilebilir. Ancak gerçek anlamda çalışma zamanında tek bir ConfigurationManager nesne örneği söz konusu olacaktır. Bu n sayıda nesne örneğinin herhangibirinde yapılan değişiklikler pek tabii diğer değişkenleri de etkiler. Söz gelimi yukarıdaki koda aşağıdaki parçayı eklediğimizi düşünelim.

```text
mngr2.default_host="127.0.0.1"
puts mngr1.get_default_host
puts mngr2.get_default_host
```

Bu durumda mngr2 ile yapılan değişiklik mngr1 tarafından da görülecektir.

![rsingleton_2.gif](images/rsingleton_2.gif)

## Marshalling ile Nesne Durumunu Korumak

Gelelim Marshaling ile Singleton karakteristiğindeki bir nesne örneğinin anlık durumunun (State) saklanmasına. Bunun için Singleton modülünden gelen _load ve _dump metodlarının çalışma zamanında durumu korumak istenen sınıf için ezilmesi gerekiyor. Aşağıdaki örnek kod parçasında bu durum ele alınıyor.

```text
require "Singleton"

class ConfigurationManager
  include Singleton
  
  attr_accessor :default_host,:default_port,:entry_point
  
  def get_default_host
    "tcpip://#{@default_host}:#{@default_port}/#{@entry_point}"
  end
  
  def _dump(obj)
    state="#{@default_host}|#{@default_port}|#{@entry_point}"
    Marshal.dump(state,obj)
  end
  
  def self._load(name)
    state=Marshal.load(name)
    values=state.split("|")
    instance.default_host=values[0]
    instance.default_port=values[1]
    instance.entry_point=values[2]
    instance
  end
end

mngr=ConfigurationManager.instance
mngr.default_host="azonfactory"
mngr.default_port="5555"
mngr.entry_point="prod"
puts "BeforeMarshalling"
puts mngr.get_default_host
last_state=Marshal.dump(mngr)
puts last_state
mngr.default_host="localhost"
mngr.default_port="8080"
mngr.entry_point="test"
puts "AfterChange"
puts mngr.get_default_host
mngr_backup=Marshal.load(last_state)
puts "AfterLoad"
puts mngr_backup.get_default_host
```

_dump metodu içerisinde sınıf örneğinin o anki durumu ile ilişkili olarak tutmak istediğimiz ne kadar nitelik varsa ardışıl olarak aralarına pipe işareti koyarar dizdik. Bunu okuduğumuz yerde çözümlüyor ve instance ile eriştiğimiz nesne örneğinin ilgili niteliklerine atıyoruz. Okuma işlemini kolaylaştırmak için | işaretinden faydalandık. Aslında ConfigurationManager içerisindeki _dump ve _load metodları işleyişlerini gerçekleştirirken Marshal modülünün ilgili fonskiyonları kullanılmaktalar. Marshal modülü nesne içeriğinin byte stream olarak serileştirlmesi ve ters okunması noktasında görev alıyor.

Kodu denediğimiz akışta mngr isimli ConfigurationManager sınıf örneğinin anlık durumunu kayıt altına alıyoruz (Bellekte) Ardından niteliklerinde bir takım değişiklikler yapıyoruz. Bu sadece test amaçlı bir işlem. Sonrasında nesne örneğini ilk kayıt ettiğimiz durumuna döndürüyoruz. Farklı bir değişken ile dönmüş olsa da Singleton kalıbı gereği mngr ve mngr_backup aynı nesnelerdir. İşte çalışma zamanı çıktısı.

![rsingleton_3.gif](images/rsingleton_3.gif)

Böylece geldik bir Ruby kod parçacığımızın daha sonuna. Bu yazıda Ruby dilinde Singleton tasarımın kalıbının ne kadar kolay uygulanabildiğini ve nesne durumlarının saklanması için Marshal modülünden nasıl faydalanabileceğimizi gördük. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
