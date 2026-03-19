---
title: "Tek Fotoluk İpucu 147 - Port Dinlemek"
pubDate: 2016-12-07 21:51:00
categories:
  - Ruby
tags:
  - Ruby
  - SQL Server
  - Oracle
  - MySQL
  - HTTP
---

# Tek Fotoluk İpucu 147 - Port Dinlemek
Merhaba Arkadaşlar,

Diyelim ki bir sunucu üzerinde tanımlı portların anlık durumları hakkında bilgi sahibi olmak istiyorsunuz (Örneğin sunucuda SQL Server yüklü ise varsayılan 1433 portu açık mı veya ftp portu cevap verir konumda mı vb) Ruby dilinde bu tip bir işlevselliği gerçekleştirmek son derece basit. Tek yapmamız gereken makine adı ve port bilgisini kullanmak. Nasıl mı? Aynen aşağıdaki fotoğrafta olduğu gibi.

![tfi_147.gif](images/tfi_147.gif)

TCPSocket nesnesini kullanabilmek için sockets kütüphanesine ihtiyacımız var. Built-In gelen kütüphaneyi require anahtar kelimesi ile bildirdikten sonra, belli başlı portlar için bir Hash listesi oluşturuyoruz. Anahtar (key) olarak servis/port adını, değer (value) olarakta port numarasını kullanıyoruz (Örnekte bilinen belli başlı port numaralarına yer verdik. Sistemde yüklü olan servislere göre genişletebilirsiniz)

get_port_status metodu iki parametre almakta. port'un olduğu makine adı ve port numarası. Eğer ilgili soket kapalı ise ve bize cevap dönmüyorsa ortama bir hata fırlatılması muhtemel. Eğer bağlantı tanımlı bir firewall kural seti nedeniyle geri çevrilirse ECONNREFUSED, zaman aşımı sonucu ulaşılamazsa ETIMEDOUT hatası oluşmakta. Bu nedenle bir istisna bloğundan yararlandık. TCPSocket nesnesi başarılı bir şekilde örneklenmişse portun açık olduğu sonucuna varabiliriz (Dilerseniz hata mesajını da geriye döndürebilir ve portun kapalı olma sebebini e bilgi vermek için kullanabilirsiniz)

Kodun ilerleyen kısmında tüm hash listesini dönüyor ve listede yer alan portların anlık durumlarını ekrana yazdırıyoruz (Tabii benim iş bilgisayarımda sadece Http servisi etkin olduğundan diğer tüm portlar kapalı durumda)

## Port Demişken

Aslında bir makinede tanımlı portlar 1 ile 65535 aralığında değer alırlar. 1 ile 1023 aralğında bilinen portlar bulunur. ftp, smtp, http, https vb...1024 ile 49151 aralığında ise IANA ([Internet Assigned Numbers Authority](http://www.iana.org/assignments/service-names-port-numbers/service-names-port-numbers.xhtml)) tarafından kayıt edilmiş servisler yer alır. SQL Server, Oracle, MySQL gibi uygulama servislerine ait portlar burada bulunur. 49152 ile 65535 aralığında ise geçici olarak kullanılan portlar vardır. Yani buradaki portları kullanan uygulamalar için sabit numaralar söz konusu değildir. Biz kayıtlı olan port numaralarına bakarak açık veya kapalı olup olmadıklarını anlamaya çalışıyoruz. Aslında porttan cevap alabiliyorsak port durumunu açık olarak kabul ediyoruz.

## Porttan Cevap Almak Demişken

Bir porttan cevap almamız onun açık olduğu bir başka deyişle portun arkasındaki bir uygulamanın gelecek paketleri dinlemekte olduğunu ifade edecektir. Aslında ilk olarak ilgili porta SYN isimli bir paket gider. Yani yeni bir bağlantı talebi göndermiş oluruz. Bunun sonucu eğer portun arkasında duran uygulama bağlantı için hazırsa, istemci tarafa SYN/ACK şeklinde bir paketle cevap döner. Yani sunucu tarafı istemciye "seninle konuşmaya hazırım" der. Tabii ilgil portun canı konuşmak istemezse talebi geri çevirecektir ki bu durumda istemciye RST paketi gönderilir. Ancak, istemci SYN/ACK mesajını almışsa artık sunucu ile konuşabilmenin verdiği memnuniyetle iade-i itibar ederek ACK paketi ile "harikasın" şeklinde dönüş yapar. Her ne kadar Network programlama eğitimi almamış olsam da three-way handshake adlı bu iletişim şeklinden anladığım budur.

Siz örneği farklı amaçlarla da kullanabilirsiniz. Örneğin iki makine arasında bu şekilde port temelli bir haberleşme olacaksa ve kaynak makinenin hedef makinedeki ilgili porta ulaşıp ulaşamadığının kontrolü gerekiyorsa bu tip bir betik kullanılabilir. Bir nevi ping işlemi yapıyoruz o yüzden aynı porta farklı sayılarda talep gönderip iletişim sürelerini ölçümleyebiliriz de. Kod parçası bir servis listesi için de uygulanabilir. Yeni kurulan bir sunucu üzerinde konuşlandırılan ve farklı port numaraları üzerinden hizmet eden servis noktalarımız olduğunu varsayarsak, istemciler için istisnai Firewall talebi yapılması gerekiyor mu sorusuna hemen cevap bulabiliriz.

Böylece geldik bir ipucunun daha sonuna. Bir başka ipucunda görüşmek dileğiyle hepinize mutlu günler dilerim.
