---
layout: post
title: "WCF - Internet Üzerinden Güvenliği Sağlamak - 1"
date: 2007-07-03 12:00:00 +0300
description: "Windows Communication Foundation ile geliştirilen dağıtık mimari uygulamalarında istemci(client) ve servis(service) arasındaki güvenliği temel olarak mesaj seviyesinde(Message Level) ve iletişim seviyesinde(Transport Level) sağlayabileceğimizden daha önceki yazılarımızda bahsetmiştik."
categories:
  - wcf
tags:
  - wcf
  - bash
  - dotnet
  - aspnet
  - http
  - iis
  - authentication
  - authorization
  - visual-studio
---
# WCF - Internet Üzerinden Güvenliği Sağlamak - 1
Değerli Okurlarım Merhabalar,

Windows Communication Foundation ile geliştirilen dağıtık mimari uygulamalarında istemci (client) ve servis (service) arasındaki güvenliği temel olarak mesaj seviyesinde (Message Level) ve iletişim seviyesinde (Transport Level) sağlayabileceğimizden daha önceki yazılarımızda bahsetmiştik. Söz konusu seviyelerden hangisi tercih edilirse edilsin, istemcilerin servisi kullanırken doğrulanmaları (authenticate) ve gerekli işlemleri yapabilmeleri için yetkilerine bakılmaları (authorization) gerekir. Windows Communication Foundation, istemcileri doğrulamak (authenticate) adına altı farklı yol kullanılmasına olanak tanımaktadır. Bunlar aşağıdaki tabloda görüldüğü gibidir.

Windows Communication Foundation Doğrulama (Authenticate) Yolları

Yol
Açıklama

Windows
İstemcilerin doğrulanması için tipik olarak servis tarafında yer alan windows hesaplarından (Windows Accounts) faydalanılır. Daha çok Kerberos veya NTLM gibi sistemler ele alınır. Bu teknik intranet tabanlı dağıtık mimari uygulamalarında oldukça işe yaramakta ve tercih edilmektedir.

Kullanıcı Adı/ Şifre
(Username/Password)
İstemciler servis tarafına kullanıcı adı (Username) ve şifre (Password) bilgisi gönderir. Kullanıcı hesap bilgileri servis tarafında çoğunlukla bir veritabanı sistemi üzerinde tutulur. WCF servislerinin IIS üzerinde tutulabildiği göz önüne alındığında Asp.Net ile gelen Membership veritabanlarını kullanmak yaygın olarak tercih edilebilir. WCF servisinin HTTP üzerinden yayınlandığı internet tabanlı senaryolarda sıklıkla kullanılabilir.

X509
İstemciler servis tarafına kendilerini geçerli bir sertifika yardımıyla tanıtırlar.

Özel (Custom)
Doğrulama (Authenticate) işlemleri için özelleştirilmiş yapılar kullanılır. Biometric buna örnek olarak verilebilir. Söz gelimi istemcilerin parmak izlerine veya göz retinalarına göre doğrulanması gibi mekanizmalar var olan yapıların özelleştirilmesi ile mümkün olabilir.

Issued Token
Bu doğrulama tekniğine verilebilecek en güzel örnek.Net Framework 3.0 ile gelmiş olan CardSpace mimarisidir.

Yok (None)
İstemcilerin tamamı doğrulanır. Bir anlamda da servise herkesin erişebilmesi sağlanmış olunur.

İstemcilerin kendilerini servis tarafına doğrulatmaları esnasında kullanıcı bilgilerinin saklandığı bazı ortamlar söz konusudur. Windows hesaplarının (account) tutulduğu sistemler bellidir. Ancak bunun dışında özellikle internet tabanlı senaryolarda ele alınabilecek şekilde veritabanı (database) kullanımıda mümkündür. WCF mimarisinde servis tarafı IIS üzerinde barındırılabilmektedir. Bu sebepten dolayı kullanıcılara ait hesap bilgileri için Asp.Net 2.0 ile birlikte gelen üyelik yönetim sisteminden (Membership Management API) faydalanılabilir. Elbetteki windows veya veritabanı dışında özel depolama sistemleride söz konusu olabilir.

Doğrulanan kullanıcıların yetkilerine bakılmadan işlem yapılması tam olarak güvenliğin sağlanamadığı anlamınada gelir. Dolayısıyla servis tarafında yer alan operasyonlarda doğrulanan kullanıcıların rollerine, başka bir deyişle yetkilerine bakılarak ilerlenilmesinde fayda vardır. WCF mimarisinde güvenlik denince aklan gelenler sadece authentication ve authorization olmamalıdır. Aslında Windows Communication Foundation, maksimum güvenliğin sağlanabilmesi için üç farklı ilkenin var olmasını gerektirmektedir. Bunlar, mesaj bütünlüğü (Message Integrity), mesaj mahremiyeti (Message Privacy) ve müşterek doğrulama (Mutual Authentication) ilkeleridir.

Maksimum Güvenlik için Sağlanması Gereken İlkeler

İlke
Açıklama

Mesaj Bütünlüğü
(Message Integrity)
Mesaj bütünlüğü ilkesine göre istemciden servise doğru gidecek olan mesajın başkaları tarafından kurcalanıp bozulamaması gerekmektedir. Bir başka deyişle bu ilke, kötü niyetli kullanıcıların (malicious users) arada hareket eden mesajların bütünlüğünü bozacak şekilde hamlelerde bulunamamasının sağlanmasını gerektirir.

Mesaj Mahremiyeti
(Message Privacy)
Bu ilke istemci ve servis arasında hareket eden mesajların gizliliğinin sağlanmasını gerektirir. Bir başka deyişle kötü niyetli kullanıcılar çeşitli 3ncü parti yazılımları kullanarak mesaj içeriklerini okuyamamalıdır. Bu ilke aynı zamanda Message Integrity ilkesinin tamamlayıcısı olarak da düşünülebilir.

Müşterek Doğrulama
(Mutual Authentication)
İstemcilerin doğru servis ile haberleşmesini, istemciden gelen ehliyet (Crendential) bilgilerinin servis tarafında doğrulanmasını ve bunlara ek olarak tekrarlı atakların (replay attacks) bertaraf edilebilmesinin sağlanmasını hedefleyen ilkedir.

WCF için söz konusu olan iletişim güvenlik sistemleri yukarıdaki ilkeleri göz önüne alır ve buna göre maksimum güvenliğin sağlanabilmesini kolaylaştırır. Buna göre kendi özel güvenlik sistemlerimizi geliştirmek istediğimizde burada bahsedilen ilkelere uygun olacak şekilde hareket edilmesi gerekir.

WCF mimarisinde iletişim güvenliğini sağlayabilmek adına kullanılabilecek 5 farklı iletişim güvenlik tekniği bulunmaktadır. Bunlar None, Transport, Message, Mixed ve Both teknikleridir. Message seviyesinde iletişim güvenliği tekniğini daha önceki [yazımızda](http://www.bsenyurt.com/MakaleGoster.aspx?ID=204) ele almıştık. Transport tekniğini ise bu yazımız ile birlikte ele almaya çalışacağız. Gelelim diğer modlara. Mixed modda Message Integrity ve Privacy ilkelerini sağlamak için iletişim (Transport) seviyesinde güvenlik tekniği kullanılır. İstemci ehliyetlerini (Client Credential) korumak içinse mesaj (Message) seviyesinde güvenlik tekniği ele alınır. Both iletişim güvenlik tekniğinde mesajlar mesaj seviyesinde güvenlik tekniğine göre şifrelenirken, istemciden servis tarafında gönderilirken Transport tekniğine göre aktarılır.

WCF mimarisinin pek çok konusunda olduğu gibi bazı işlemleri gerçekleştirmek için ele alınması gereken oldukça fazla faktör vardır. Güvenlik teknikleri ile var olan bağlayıcı tipler (binding types) arasındaki durumda aynıdır. Bu sebepten dolayı aşağıdaki tablonun bilinmesinde ve ele alınmasında fayda vardır.

Bağlayıcı Tipler ve İletişim Güvenlik Teknikleri Arasındaki İlişki

Bağlayıcı Tip (Binding Type)
Transport
Message
Mixed
Both
None

NetTcpBinding
Evet (Varsayılan)
Evet
Evet
Hayır
Evet

NetPeerTcpBinding
Evet (Varsayılan)
Evet
Evet
Hayır
Evet

NetNamedPipeBinding
Evet (Varsayılan)
Hayır
Hayır
Hayır
Evet

NetMsmqBinding
Evet (Varsayılan)
Evet
Hayır
Evet
Evet

WSHttpBinding
Evet
Evet (Varsayılan)
Evet
Hayır
Evet

WSFederationHttpBinding
Hayır
Evet (Varsayılan)
Evet
Hayır
Evet

WSDualHttpBinding
Hayır
Evet (Varsayılan)
Hayır
Hayır
Evet

BasicHttpBinding
Evet
Evet
Evet
Hayır
Evet (Varsayılan)

Bu tabloda hangi bağlayıcı tipin hangi iletişim güvenlik tekniklerini desteklediği belirtilmektedir. Örneğin Both tekniğini sadece NetMsmqBinding bağlayıcı tipi desteklerken diğerleri desteklemez. Dolayısıyla istemci ve sunucu arasındaki güvenliğin nasıl sağlanacağına karar verildikten sonra uygun bağlayıcı tiplerin göz önüne alınması için yukarıdaki tabloda yer alan bilgilerden faydalanılabilir.

Bu bölümde geliştirilmeye başlanacak olan örnekte iletişim seviyesinde güvenlik (transport level security) tekniği kullanılacak olup yazının ikinci bölümünde istemcilere ait ehliyet bilgilerini servis tarafında kontrol ederken Sql Membership Provider ve Sql Role Provider API'leri ele alınacaktır. Örneğe geçmeden önce iletişim seviyesi güvenlik tekniğinde seçilen bağlayıcı tipe göre hangi ehliyet modellerinin kullanılabileceiğinin bilinmesinde fayda vardır. Dolayısıyla göz önünde bulundurulması gereken bir tablo daha karşımıza çıkmaktadır.

Bağlayıcı Tipler, İletişim Seviyesinde Güvenlik Tekniği ve Doğrulama Modelleri Arasındaki İlişki

Bağlayıcı Tip (Binding Type)
Windows
Username/Password
X509
None

NetTcpBinding
Evet (Varsayılan)
Hayır
Evet
Evet

NetPeerTcpBinding
Hayır
Evet
Evet
Hayır

NetNamedPipeBinding
Evet (Varsayılan)
Hayır
Hayır
Hayır

NetMsmqBinding
Evet (Varsayılan)
Evet
Hayır
Evet

WSHttpBinding
Evet (Varsayılan)
Evet
Evet
Evet

WSFederationHttpBinding

X

WSDualHttpBinding

BasicHttpBinding
Evet
Evet
Evet
Evet (Varsayılan)

Dikkat edilmesi gereken noktalardan birisi WSFederationHttpBinding ve WSDualHttpBinding bağlayıcı tiplerinin iletişim seviyesinde güvenlik tekniği söz konusu olduğunda hiç bir doğrulama modelini desteklemediğidir. Bu noktaları açıklığa kavuşturduktan sonra nihayetinde bir örnek geliştirmeye başlayarak internet tabanlı WCF uygulamalarında iletişim seviyesinde güvenliği nasıl sağlayabileceğimizi incelemeye başlayabiliriz.

İletişim seviyesinde güvenlik söz konusu olduğundan WCF servisinin IIS üzerinde barındırılması ve HTTPS protokolünü baz alarak hizmet verebilmesinin sağlanması gerekmektedir. Ancak öncesinde güvenli iletişim kanalı kullanımı için (bir başka deyişle https üzerinden hizmet vermek için) hayali bir sertifika oluşturulmalıdır. Hayali sertifikaları oluşturmak için Makecert.exe aracı kullanılabilir. Bu araç tamamen test amaçlı X509 sertifikalarının üretilmesini sağlamaktadır.

> MakeCert.exe aracı ile ilgili detaylı bilgi için [http://msdn2.microsoft.com/en-us/library/bfsktky3 (VS.80).aspx](http://msdn2.microsoft.com/en-us/library/bfsktky3(VS.80).aspx) adresinden bilgi alınabilir.

Test sertifikası oluşturmak için Visual Studio 2005 command prompt üzerinden aşağıdaki komutun yazılması yeterlidir.

```bash
C:\>makecert -sr LocalMachine -ss My -n CN=TestSertifika-HTTPS-Server -sky exchange -sk TestSertifika-HTTPS-Key
```

![mk211_1.gif](/assets/images/2007/mk211_1.gif)

Succeeded mesajı görüldüğü takdirde sertifika başarılı bir şekilde oluşturulmuş demektir. (Makalemizin amacı Makecert aracını tanımak olmadığından aracın parametre detayları üzerinde durulmayacaktır.) Oluşturulan sertifikayı görmek için Microsoft Management Console'dan faydalanılabilir.

Diğer taraftan oluşturulan sertifikanın iletişim kanalı ile (ki burada söz konusu olan yerel makinedeki 8000 numaralı Http portudur) ilişkilendirilmesi gerekir. Bu işlem için HttpCfg.exe aracından yararlanılabilinir.

> Windows XP'de HttpCfg.exe aracını kullanabilmek için [Windows XP Service Pack 2 Support Tools](http://thesource.ofallevil.com/downloads/details.aspx?FamilyId=49AE8576-9BB9-4126-9761-BA8011FABF38&displaylang=en)'u indirmek gerekebilir.

Httpcfg.exe aracı parametre olarak oluşturulan sertifikaya ait parmak damgasını (thumbprint) kullanır. Parmak damgasını elde edebilmek için öncelikli olarak Microsoft Management Console'da aşağıdaki ekran görüntüsünde yer aldığı gibi Add/Remove Snap In seçeneğinden Certificates işaretlenir.

![mk211_2.gif](/assets/images/2007/mk211_2.gif)

Certificates seçildikten sonra sıradaki adımda Computer Account seçilir.

![mk211_3.gif](/assets/images/2007/mk211_3.gif)

Sonraki adımda snap-in yönetimini üstlenecek olan bilgisayar seçilir. Bu varsayılan olarak yerel bilgisayarı (Local Computer) işaret etmektedir.

![mk211_4.gif](/assets/images/2007/mk211_4.gif)

Bunun sonucunda oluşturulan test sertifikası aşağıdaki ekran görüntüsünde yer aldığı gibi Personal->Certificates klasörü altında görülecektir.

![mk211_5.gif](/assets/images/2007/mk211_5.gif)

Buradanda sertifikanın detaylarına geçilerek Thumbprint alanının değeri öğrenilebilir.

![mk211_6.gif](/assets/images/2007/mk211_6.gif)

Artık HttpCfg aracı yardımıyla sertifikanın port ile ilişkilendirilmesi sağlanabilir. Httpcfg.exe aracı yardımıyla sertifikaya ait parmak damgasının (thumbprint), port ile ilişkilendirilmesini sağlamak için aşağıdaki komut, Xp Support Tools Command Prompt üzerinden çalıştırılmalıdır.

C:\>httpcfg set ssl -i 0.0.0.0:8000 h7eae8740bda1985efceaa1b91d1ce266bfe5788c

![mk211_7.gif](/assets/images/2007/mk211_7.gif)

HttpSetServiceConfiguration Completed with 0 mesajı görüldüğü takdirde operasyonun başarılı bir şekilde tamamlandığı anlaşılabilir.

> HttpSetServiceConfiguration completed with 183 gibi bir mesaj alınması hata oluştuğu anlamına gelir. Hata mesajı zaten var olan dosyaya tekrardan yazılmak istenmesinden kaynaklanmaktadır. (ERRORALREADYEXISTS 183 Cannot create a file when that file already exists.) Bu nedenle sertifika unload edilebilir. Sertifikanın Unload edilmesi için komut satırından
> C:\>Httpcfg delete ssl /i 0.0.0.0:8000
> yazılması yeterlidir. Buna göre yerel makineye ait 8000 port numarası için tanımlanmış olan ssl sertifikalarına ait bildirimler silinecektir. İstenirse, Httpcfg.exe aracı yardımıyla IIS üzerinde yüklenmiş olan sertifikaları görmek için komut satırından
> C:\>Httpcfg query ssl
> yazılması yeterlidir.

Oluşturulan sertifikanın IIS üzerinde yer alacak WCF uygulaması tarafından kullanılabilmesi için öncelikli olarak bildirilmesi gerekir. İzleyen adımlarda Windows XP işletim sistemi üzerinde yer alan IIS 5.1 için söz konusu bildirim işleminin nasıl yapılacağı ele alınmaktadır. Öncelikli olarak Internet Information Services üzerinden Default Web Site sağ tıklanıp özelliklerden (Properties) Directory Security kısmına geçilir ve buradan Server Sertificate düğmesi tıklanır.

![mk211_9.gif](/assets/images/2007/mk211_9.gif)

Burada da Next ile ilerlenip Assign an existing certificate seçeneği işaretlenir ve devam edilir. Böylece daha önceki adımlarda yüklenmiş olan sertifikanın kullanılabilmesi sağlanmış olmaktadır.

![mk211_10.gif](/assets/images/2007/mk211_10.gif)

İzleyen adımda az önce yüklenmiş olan sertifika görülebilir. Yüklü olan başka sertifikalarda söz konusu olabilir. Uygun olan sertifika seçilerek ilerlemeye devam edilir.

![mk211_11.gif](/assets/images/2007/mk211_11.gif)

Eğer sertifika yükleme işlemi başarılı bir şekilde gerçekleştirildiyse aşağıdaki ekran görüntüsü elde edilmelidir.

![mk211_12.gif](/assets/images/2007/mk211_12.gif)

Bundan sonraki tüm adımlar onaylanarak işlemler tamamlanır.

Artık WCF Service uygulamasının yazılmasına başlanabilir. Bu amaçla Visual Studio 2005 üzerinden New Web Site seçeneği ile yeni bir web sitesi açılır ve WCF Service şablonu seçilir. Burada önemli olan nokta IIS üzerinde açılacak olan sanal klasör için Secure Socket Layer kullanılacağını belirtmektir. Bunun için aşağıdaki ekran görüntüsünde olduğu gibi lokasyon seçilirken Use Secure Sockets Layer seçeneğinin işaretlenmesi yeterlidir.

![mk211_13.gif](/assets/images/2007/mk211_13.gif)

Örnek servis CebirServisi olarak isimlendirilmiştir. Bu işlemin ardından aşağıdaki ekran görüntüsüden yer aldığı gibi ilgili web adresinin başında HTTP yerine HTTPS yazıldığı görülecektir.

![mk211_14.gif](/assets/images/2007/mk211_14.gif)

Söz konusu adımlar tamalandıktan sonra oluşturulan CebirServisi isimli servisin iletişim sırasında HTTPS protokolüne göre çalışabilmesi için, IIS üzerinden gerekli hazırlıkların yapılması gerekmektedir. Bunun için ilk olarak IIS altında yeni açılmış olan CebirServisi sanal klasörünün özelliklerinden Directory Security kısmında gidilir. Yazının başlarında Default Web Site'a hazırlanmış olan hayali sertifika tanıtıldığından, View Certificate kısmı kullanılabilir haldedir.

![mk211_15.gif](/assets/images/2007/mk211_15.gif)

View Certificate düğmesi tıklandıktan sonra açılan Certificate penceresinde, yüklenmiş olan test sertifikasına ait bilgiler aşağıdaki ekran görüntüsünde olduğu gibi görülebilir.

![mk211_17.gif](/assets/images/2007/mk211_17.gif)

CebirServisi için yapılması gereken bir diğer işlem ise, Secure Communications kısmında yer alan Edit düğmesine tıkladıktan sonra çıkan ekrandan, Require Secure Channel (SSL) seçeneğini işaretlemektir. Böylece servis ile güvenli iletişimin sağlanması için SSL gerekliliği bildirilmiş olunur.

![mk211_16.gif](/assets/images/2007/mk211_16.gif)

Son olarak yine Directory Security kısmından Anonymous Access and Authentication Control içerisinde yer alan Edit düğmesine tıklanıp açılan pencerede, Integrated Windows Authentication seçeneği kaldırılmalı ve Basic Authentication işaretlenmelidir.

Yazı dizisinin bu ilk bölümünde WCF Servisi için IIS üzerinde gerekli sertifika bildirimlerinin nasıl yapılabileceğini incelemeye çalışırken hayali bir sertifika için Makecert ve Httpcfg araçlarını nasıl kullanabileceğimizide görmeye çalıştık. Bunların dışında iletişim seviyesinde güvenlik söz konusu olduğunda bağlayıcı tiplerin ve doğrulama modlarının nasıl ele alınması gerektiği üzerinde durduk. Böylece geldik bir makalemizin daha sonuna. Sonraki makalemizde geliştirilen örneği tamamlamaya çalışacağız. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.