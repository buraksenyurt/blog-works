---
title: "REST Starter Kit Nedir?"
pubDate: 2009-05-05 12:41:00
categories:
  - WCF
tags:
  - WCF
  - .NET
  - ASP.NET
  - XML
  - REST
  - JSON
  - Web Service
  - XML Web Services
  - HTTP
  - Authentication
  - Authorization
  - JavaScript
  - Async/Await
  - Caching
  - Transactions
  - Visual Studio
---

# REST Starter Kit Nedir?
![WCF-Rest-Starter-Kit.png](images/WCF-Rest-Starter-Kit.png)

Merhaba Arkadaşlar,

Bildiğiniz üzere bir süredir [WCF REST Starter Kit](http://aspnet.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=24644)ile ilişkili yazılarımı ve görsel derslerimi sizlerle paylaşmaktayım. Ancak önemli bir noktayı kaçırdığımı düşünüyorum.

![Embarassed](images/smiley-embarassed.gif)

Nedir bu WCF REST Starter Kit? Bizlere ne gibi avantajlar getirmektedir?

WCF REST Starter Kit temel olarak, WCF servislerinin REST (REpresentational State Transfer) bazlı olaraktan geliştirilmesi için gerekli özellik ve şablonları (Visual Studio Templates) içermekte olan bir yardımcı araç kitidir. Bu kit.Net Framework 3.5 ve SP1 ile birlikte, WCF tarafına kazandırılan Web programlama modelini kullanır ve geliştiricinin, REST bazlı WCF servislerinin yazılması ve tüketilmesi sırasında gerekli olan bir çok kıstasın (aspect olarakta düşünebiliriz) kolayca ele alabilmesini hedefler. WCF Rest Starter Kit, konsept olarak WCF servislerini ve REST modelini hedef almakla birlikte özellikle açık kaynak kodlu olması açısındanda önemli bir eklenti olarak düşünülmelidir.

NOT: Aslında Xml Web Servisleri içinde zamanında Web Service Enhancements adıyla yayımlanmış bir yardımcı kit bulunmaktadır. WSE'nin en büyük amacı, Xml Web Servislerinde çok fazla kodlamayı gerektiren bazı standart kıstasların (Transaction Flow, Security, MTOM bazlı dosya aktarımı vb...) nitelik veya konfigurasyon bazında daha kolay bir şekilde uygulanabilmesidir.

Kit aslında iki ana bölümden oluşmaktadır.

1- Sunucu Tarafı Yetenekleri; REST tabanlı WCF servislerinin host edildiği sunucu tarafı ile ilgili özelliklerdir. Örneğin,

- dekleratif olarak ön bellekleme özelliklerini tanımlayabilmek (Declarative Caching),
- hata yönetimi (Error Handling),
- güvenlik (Security),
- yardım desteği (Help Page Support),
- büyük boyutlu dosyaların sunucu kaynaklarını fazla yormada taşınması (push style streaming)...

Bunlara ek olarak Visual Studio 2008 ortamına eklenen pek çok şablonda yer almaktadır.

- Koleksiyon bazlı servisler (Rest Collection Wcf Service),
- tek kaynaklı servisler (Rest Singleton Wcf Service),
- Atom protokolü bazlı servisler (Atom Publishing Protocol WCF Service),
- Atom bazlı içerik yayınlama servisi (Atom Feed WCF Service),
- ve basit olarak sadece HTTP bazlı taleplere cevap verecek olan WCF Servisleri (HTTP Plain XML WCF Service)

2- İstemci Tarafı Yetenekleri; İstemcilerin, REST tabanlı WCF servislerini kolayca kullanabilmeleri için gerekli özelliklerdir. REST Starter Kit’in ikinci versiyonunda, istemci tarafından REST mesajlarının gönderilmesi ve cevapların işlenmesini kolaylaştıracak şekilde HttpClient isimli yeni bir sınıf geliştirilmiştir. Ayrıca Visual Studio IDE’ sine Paste Xml As Type isimli bir diğer özellik katılarak (add-in), XML içeriklerinin (XSD şemasıda kullanılabilir) serileştirilebilir tip haline dönüştürülmesi de son derece kolaylaştırılmaktadır. Bu sayede servis tarafından yayımlanan bir XML içeriğin copy-paste tekniğini kullanarak ve Paste Xml As Type seçeneğinden yararlanarak, managed tarafta ele alınabilecek bir tip haline getirebiliriz.

Bu iki ayrım haricinde, Starter Kit ile birlikte gelip göze çarpan bir kaç noktayıda, aşağıdaki gibi özetleyebiliriz.

Çıktı formatları için destek (Representation Format): Günümüz web uygulamalarında, istemci tarafının ele aldığı en popular içerik formatları, XML (Xtensible Markup Language) ve JSON (JavaScript Object Notation) dır. REST bazlı WCF servisleri zaten bu tipte yayınlama yapma yeteneğine sahiptir. Starter Kit’ in burada kattığı diğer bir yetenek ise, istemciden gelen HTTP-GET talebine bakarak geriye XML veya JSON formatında içerik gönderilmesini kolaylaştırmaktır.

Dekleratif Ön bellekleme (Declerative Caching): Deklerafit kelimesinin burada kattığı anlam aslında, söz konusu özelliğin bir aspect olarak ele alınabilmesidir. Buna gore nitelik (attribute) ve konfigurasyon (web.config örneğin) bazında önbellekleme ile ilişkili bildirimler kolayca yapılabilir. Starter Kit burada işlemleri kolaylaştırmak için WebCache niteliğini sunmaktadır.

Yardım Desteği (Help Support): REST bazlı WCF servislerinin istemci tarafından nasıl kullanılabileceğinin bilinmesi önemlidir. Burada servise giden taleplerin URI formatında olduğu düşünüldüğünde, servisin sunduğu operasyonların URI taleplerinin ne olacağı, operasyonun özet olarak ne yaptığı gibi bilgileri istemciye sunmak için REST starter kit /help sayfa desteğini getirmektedir. Burada, operasyonlar açısından önem arz eden konulardan biriside WebHelp niteliği yardımıyla özet bilgilerin belirtilmesi ve yardım sayfasında gösterilmelerinin sağlanmasıdır.

Hata Desteği (Error Handling): Servis tarafında bir hata oluştuğunda bunun istemci tarafında string veya geliştirici tarafından yazılmış bir.Net tipi olarak XML veya JSON formatında döndürülebilmesi, starter kit ile dahada kolaylaştırılmaktadır. Burada WebProtocolException tipinden yararlanılmaktadır. Tabi JSON veya XML tipinden dönüş olacağına WebGet veya WebInvoke niteliklerindeki, ResponseFormat özelliğine atanan değer ile karar verilir.

Service Host: REST Starter kit, daha az konfigurasyon ile çalışma zamanının pek çok değerinin ayarlanabilmesini sağlamaktadır. Bu nedenle REST Starter Kit, WebServiceHost2 isimli bir tip içermektedir. Bu tipi, Visual Studio 2008 altındaki REST şablonlarını oluşturduğumuzda, svc öğelerine ait markup dosyaları içerisinde görebiliriz.

Güvenlik (Security): REST bazlı WCF servislerini, starter kit ile geliştirirken, Asp.Net güvenlik özelliklerden yararlanılabilir. Böylece, örneğin form tabanlı doğrulama (Form Based Authentication) veya rol bazlı yetkilendirme (Role Based Authorization) yapılabilir. Bu işlemler için Asp.Net'in standart Membership API'sinden yararlanılabilir yada özel provider'lar kullanılabilir.

HttpClient: İstemci tarafına getirilen bu tip ile, REST bazlı WCF servislerini HTTP Put, Get, Delete, Post metodlarına göre kullanmak dahada kolaylaşmaktadır. Ayrıca asenkron programlama (async programming) modeline de destek verilmektedir. Üstelik, olay bazlı asenkron programlama (Event Based Asnyc Programming) modelide kullanılabilmektedir. Ayrıca Atom protokolüne gore yayın yapan bir servisin istemci tarafından ele alınması için AtomPubClient sınıfıda HttpClient tipini kullanmak üzere, REST Starter Kit’ e getirilmiş genişletmelerden birisidir.

Tabiki daha pek çok detay bulunmaktadır. Ancak sanıyorumki bu kısa bilgiler sizlerde WCF Rest Starter Kit hakkında bir fikir oluşturmuştur.

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
