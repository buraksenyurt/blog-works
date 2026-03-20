---
layout: post
title: "WCF 4.0 Yenilikleri - Routing Service Geliştirmek - Giriş [Beta 1]"
date: 2009-08-24 06:15:00 +0300
categories:
  - wcf-4-0-beta-1
tags:
  - wcf-4-0-beta-1
  - dotnet
  - wcf
  - xml
  - soap
  - http
  - iis
---
Servis Yönelimli Mimari (Service Oriented Architecture) çözümlerinde zaman zaman yönlendirme amaçlı servislerin yazılması gerekmektedir (Router Service). Bu servislerin genel kullanım amacı çoğunlukla, istemcilerden gelecek olan talepleri değerlendirip asıl işi yapacak olan servislere devretmek ile ilişkilidir.

![blg71_Giris.jpg](/assets/images/2009/blg71_Giris.jpg)

Ancak, gelen taleplere ait içeriğinin (Message Content, Header vb...) filtrelenerek ele alınması gibi ileri seviye teknikleride içerebilir. Yönlendirme işlemleri için kullanılan pek çok donanımsal cihaz ve hatta yazılım zaten mevcuttur. Bu nedenle öncelikli olarak yönlendirme servislerine neden ihtiyaç duyulabileceğini kavramakta yarar vardır.

WCF tarafında Routing Service geliştirilmesi hangi durumlarda tercih edilir?

- Özel bir Load Balancing yapısı için (genellikle donanımsal veya yazılımsal yük dengeleyici sistemlerin yetersiz kaldığı yada özelleştirilmek istendiğin durumlarda).
- İstemciden gelen mesajın içeriğine göre servis yönlendirilmesi yapılmak istendiğinde (Content Based).
- Önceliğe göre servis yönlendirmesi yapılmak istendiğinde (Priority Based)
- Versiyonlama senaryolarında.
- İstemciler ve yönlendirilen servisler arasında bir güvenlik geçidi kurulmak istendiğinde (ki bu geçit genellikle DMZ-demilitarized zone arkasında asıl servislere olan akışı güvenlik kontrolüne alır).

WCF 3.X tarafında yönlendirme servisi geliştirebilmek için belirli kodlama eforu sarfetmek gerekirken (ki bu konuda daha önceden yayınladığım bir [yazımı](https://www.buraksenyurt.com/post/WCF-Front-End-Service-Gelistirmek.aspx)incleyebilirsiniz), WCF 4.0 tarafında bu işlemler belirli tipler ve konfigurasyon özellikleri ile one-way,two-way ve duplex iletişim seviyesinde olduçka kolaylaştırılmıştır. WCF 4.0 tarafında System.ServiceModel.Routing assembly'ı içerisinde yine aynı adlı isim alanında yer alan RoutingService isimli sınıf, söz konusu yönlendirme servisi için gerekli çalışma zamanı ortamının hazırlanmasını sağlamakta ve ayrıca istemci taleplerinin filtrelenerek uygun alt servislere aktarılmasında önemli bir rol oynamaktadır.

> Not:.Net Framework Beta 1 sürümünde RoutingService olarak geçen yönlendirme sınıfı çeşitli internet kaynaklarında (örneğin [Michele Leroux Bustamante'](http://www.aspnetpro.com/articles/2009/05/asp200905mb_f/asp200905mb_f.asp)nin bloğunda) RouterService olarak geçmektedir. Dolayısıyla final sürümde servisin adında farklılıklar olabilir. Örneklerimizi.Net Framework Beta 1 üzerinde geliştirdiğimizi hatırlatmak isterim.

![blg71_RoutingService.gif](/assets/images/2009/blg71_RoutingService.gif)

Object Browser yardımıyla elde edilen yukarıdaki görüntünden farkedeceğiniz gibi, RoutingService sınıfı 4 farklı servis sözleşmesini (Service Contract) uygulamaktadır. Bu anlamda IDuplexSessionRouter, IRequestReplyRouter, ISimplexDatagramRouter ve ISimplexSessionRouter gibi önemli arayüzleri (Interfaces) implemente etmektedir. Dolayısıyla gerekli MEP (Message Exchange Patterns) modellerinin tümü desteklenmektedir. Buna göre servisin one-way, two-way veya duplex temelli isteklere göre çalışabilmesi sağlanmaktadır. RoutingService, ServiceHost nesne örneklemesi sırasında parametre olarak kullanıldığından, normal WCF host kurallarına tabidir. Yani IIS veya Self modellerde host edilebilir. Genellikle bir Windows Service, IIS yada duruma göre WAS üzerinden host edilmesi tercih edilmektedir. Genel olarak yönlendirme modelini aşağıdaki şekilde görüldüğü gibi özetleyebiliriz.

![blg71_ArchitectureV2.gif](/assets/images/2009/blg71_ArchitectureV2.gif)

İstemciden (Client) gelen talepler yönlendirme servisine (Router Service) ulaştığında belirli filtrelerden geçmekte ve bu filtrelere göre belirlenmiş alt servis noktalarına aktarılmaktadırlar. Görüldüğü üzere önemli olan noktalardan biriside filtrelemedir. Filtreleme tablosu (Filter Table) ve içerdiği filtreler (Filters) config dosyası içerisinde depolanır. Bu filtrelerde;

- XPath gibi sorgular kullanılabilir. Sonuç itibariyle mesaj içeriğinin XML tabanlı olduğu düşünüldüğünde bu son derece doğaldır.
- Sadece mesajın Header veya Soap Action kısımlarına bakılabilir.
- Birden fazla filtrenin mantıksal ve (And) işlemine tabi tutulması sağlanabilir. Nitekim, daha önceden tanımlanmış iki farklı filtreye olan uygunluğun bir arada sağlanması istenebilir.
- İstenirse programlanmış bileşeneler ile özel filtrelemeler yapılabilir.

Filtrelemeler konfigurasyon içerikli olarak tutulduğundan, geliştiricilerin (Developers) söz konusu filtreleme davranışlarını koda bulaşmadan değiştirebilmesi, güncellemesi veya yenilerini eklemesi mümkündür.

Yönlendirme servisi çok doğal olarak istemciden gelen talepleri belirli kurallara göre işletmektedir. Yönlendirme hizmetinin istemcilere sunacağı bir Endpoint olması kaçınılmazdır. Tabi istenirse birden fazla endpoint sunabilir (Örneğin built-in gelen ISimplexDatagramRouter, IRequestReplyRouter gibi servis sözleşmelerinden yararlanarak...) Diğer yandan istemcilerden gelen mesaj filtrelerde yer alan koşullardan birisine uyduğunda, ilişkili olan alt servise yönlendirilmelidir.

Buna göre yönlendirme servisi, istemciden gelen ve filtreden geçen mesajları uygun alt servislere iletmesi gerektiğinden aynı zamanda bir istemci olarak düşünülmelidir. Dolayısıyla şekildende görüleceği gibi üzerinde her alt servis için en az bir Endpoint bulunmaktadır. Alt servisler istemcinin asıl işini yapmakla yükümlü olmakla birlikte, aynı sunucuda veya farklı sunucular üzerinde konuşlandırılmış olabilirler. Bu nedenle, yönlendirme servisi arkasında Web Farm gibi yapılara sıklıkla rastlandığını söyleyebiliriz.

Peki yönlendirme servisinin içerisinde yer aldığı basit bir sistemi nasıl tasarlayabiliriz? Burada belkide en kritik konu filtrelemelerdir. Özellikle filtrelerde gelen mesaj içeriği üzerinde XPath ile sorgular atılması önemli olan ve zor noktalardandır. Bu gibi konuları bir sonraki yazımızda ele almaya çalışıyor olacağız. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
