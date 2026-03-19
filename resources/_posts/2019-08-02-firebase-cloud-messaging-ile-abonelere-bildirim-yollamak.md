---
layout: post
title: "Firebase Cloud Messaging ile Abonelere Bildirim Yollamak"
date: 2019-08-02 17:48:00 +0300
description: "Tarayıcı üzerinde yaşayan ve çevrim dışı ya da çok düşük internet hızlarında da çalışabilme özelliğine sahip olan PWA(Progressive Web Applications) uygulamalarının en önemli kabiliyetlerinden birisi de Push Notification'dır. Bu, mobil platformlardan yapılan erişimler düşünüldüğünde oldukça önemli bir nimettir. Arka planda içerik güncelleme (updating) özelliği de bir diğer önemli kabiliyettir. Bu yetenekler uygulama için tekrardan submit operasyonuna gerek kalmadan güncel kalabilmek anlamına da gelir."
categories:
  - gcp
tags:
  - gcp
  - rest
  - json
  - http
  - javascript
  - nodejs
  - github
  - dependency-management
---
# Firebase Cloud Messaging ile Abonelere Bildirim Yollamak
![friendship.png](/assets/images/2019/friendship.png)

Servis kapısı açıldığında gözlerini herkesten kaçırıp araca binerken heyecanlı ses tonuyla "Günaydın" diyerek en arka koltuğa geçen kadının ruh hali her yönüyle tanıdık geliyordu. Bir buçuk yıl kadar önce yine bu servise bindiğim ilk gün bende benzer kaygıları hissetmiştim. Oysa hayatımda ilk kez servis binmiyordum.

Ama işte o ilk biniş sırasında söylenen "Günaydın" kelimesi ardından ben ve şoförümüz İhsan Bey dışında kimsenin karşılık vermediği ve onun gözlerini aradığım sırada geçen kısa zaman diliminde aklından geçenleri tahmin ettiğim anlar, en arka koltuğa oturduğunu gördükten sonra toplum psikolojisine ayak uydurup önüme doğru bakmamla son bulmuştu.

Servis şirkete vardıktan sonra her birimiz fabrikanın farklı noktalarına doğru yürümeye başladık. Bizim binamız yolun karşı tarafında kalıyordu ve can güvenliği nedeniyle bir üst geçitten geçilerek ulaşılabiliyordu. Merdivenleri çıkarken onun ne durumda olduğunu bile unutmuşum. Tesadüfen arkamı dönüp baktığımda tek başına ve biraz da şaşkın bir şekilde ne yöne gideceğini anlamaya çalıştığını fark ettim. Yanlış yöne gittiği apaçık ortadaydı. Çelimsizliğinden, gencecik yüzünden ve taşıdığı not defterinden üniversite talebesi olan bir stajyer olduğu biraz da olsa anlaşılıyordu. Geçen yılın aynı vakitlerinde de benzer manzaralar fabrikanın çeşitli sabahlarında yaşanmıştı.

Merdivenleri tekrar indim ve arkasından ona yetişerek "Merhaba...Yeni başladınız sanırım. Nereye gidecektiniz?" dedim. Aynı günün akşamında koltuğuma oturmuş hareket saatini beklerken kapıda yine o tedirgin duruşuyla beliriverdi. İlk adımını attığında yüzündeki gerginlik az da olsa okunabiliyordu. Aklından geçen "iyi akşamlar diyeceğim ve sanırım kimse sallamayacak" düşüncesi bir bulut olup kafasının üzerinden belirmişti. Ama o sabah ki yardımın etkisinde olsa gerek bu kez yüzümü aradı ve görünce hafif bir tebessümle "iyi akşamlar" dedi. "İyi akşamlar. Eee ilk günün nasıl geçti bakalım..." diye karşılık verince bir yanımdaki koltuğa oturdu. Artık daha iyi hissediyordu.

Biraz sonraki derlemeye nasıl giriş yapacağımı bilemediğim günlerden birindeyiz anlayacağınız üzere. O yüzden yakın zamanda başıma gelen bir olayı sizinle paylaşarak başlamak istedim. Kıssadan hisse herkesin kendisine pay çıkaracağını düşünüyorum. Hepimiz stayjer olduk. Onların daha çok farkına varmamız gerektiği konusunda ufak bir hatırlatmam olsun burada.

...

Cumartesi gecesi çalışmalarının [31nci örneğinde](https://github.com/buraksenyurt/saturday-night-works/tree/master/No%2031%20-%20Push%20Notifications%20on%20PWA) Firebase Cloud Messaging sistemini kullanarak uygulamalara (örnek özelinde bir PWA programına) nasıl bildirimde bulunulabileceğini anlamaya çalışmışım. Her zaman olduğu gibi örneği WestWorld (Ubuntu 18.04, 64bit) üzerinde geliştirmişim. Şimdi notların üstünden geçme, unutulanları hatırlama ve eksikleri giderme zamanı. Öyleyse gelin notlarımızı derlemeye başlayalım.

Tarayıcı üzerinde yaşayan ve çevrim dışı ya da çok düşük internet hızlarında da çalışabilme özelliğine sahip olan PWA (Progressive Web Applications) uygulamalarının en önemli kabiliyetlerinden birisi de Push Notification'dır. Bu, mobil platformlardan yapılan erişimler düşünüldüğünde oldukça önemli bir nimettir. Uygulamaya otomatik bildirim düşmesi veya arka plan veri güncellemeleri kullanıcı deneyimi açısından bakıldığında değerli bir işlevselliktir. Bu yetenekler uygulama için tekrardan submit operasyonuna gerek kalmadan güncel kalabilmeleri anlamına gelir.

Geliştireceğimiz örnek bir basketbol maçı için güncel haber bilgisinin abone olan uygulamalara gönderilmesi üzerine kurgulanmakta. Burada mesajlaşma servisi olarak Firebase Cloud Messaging altyapısını kullanacağımız için ilgi çekici bir örnek olduğunu ifade edebilirim. Şimdi hazırlıklarımıza başlayabiliriz.

Ön Hazırlıklar
İki uygulama geliştireceğiz. Birisi çok sade HTML içeriğine sahip olan PWA uygulaması. İkinci uygulama ise bir servis. Kullanıcıların bildirim servisine abone olma ve çıkma işlemlerinin yönetimi ile bağlı olanlara bildirim yapma görevini (işte bu noktada Firebase Cloud Messaging sisteminden yararlanacak) üstlenecek. İlk olarak basketbol haberlerini takip edeceğimiz basit önyüz uygulamasını geliştirmeye başlayım. Klasör yapısını aşağıdaki terminal komutları ile oluşturabiliriz.

Ayrıca uygulama testlerini HTTP üzerinden kolayca yapabilmek için serve isimli bir npm paketinden yararlanacağız. Kurulumu için aşağıdaki terminal komutunu kullanmak yeterli.

Kodları yazmaya başlamadan önce Google tarafıyla haberleşme noktasında gerekli olan bir manifesto dosyasının oluşturulması gerekiyor.

Manifesto Dosyası

Manifesto dosyası PWA'nın Firebase tarafında etkinleştirilecek Push Notification özelliği için gereklidir. İçerisinde Sender ID değerini barındırır (ilerde karşınıza çıkacak) ve önyüzün kullandığı main modülü, abonelik başlatılırken bu değeri karşı tarafa iletmekle yükümlüdür. Peki bu dosyayı nasıl üreteceğiz?

Öncelikle [Firebase](https://app-manifest.firebaseapp.com/) kontrol paneline gidilir ve PWA için metadata bilgilerini tutacak bir Web App Manifest dosyası üretilir. Ben örnekte aşağıdaki bilgileri kullandım.

![07_31_credit_1.png](/assets/images/2019/07_31_credit_1.png)

Sonrasında Zip dosyasını bilgisayara indirip proje klasörüne açmamız gerekiyor. Manifest.json dosyası ile birlikte images isimli bir klasör de gelecektir. Images klasöründe kendi eklediğimiz active.png dosyasının farklı cihazlar için standartlaştırılmış boyutları yer alır. Bu bilgiler manifest.json dosyasına da konur.

İşimiz henüz bitmedi! Uygulama için Push Notification özelliğini de etkinleştirmek gerekiyor. Bunun için [Firebase Console](https://console.firebase.google.com/) arabirimine gidip yeni bir proje oluşturmalı ve ardından proje ayarlarına (Project Overview -> Project Settings) ulaşıp Cloud Messaging sekmesine gelinmeli (Ben "basketin-cepte-project'" isimli bir proje oluşturdum:P Hayaller başka tabii ama eldeki malzeme şimdilik bu)

Bu bölümde proje için oluşturulan Server Key ve Sender ID değerleri yer alır. Az önce bahsedildiği gibi Sender ID değerinin manifest.json dosyasına eklenmesi gerekiyor (gcmsenderid yazan kısma bakınız)

PWA Kodları
Artık basketkolik klasöründeki dosyalarımızı kodlamaya başlayabiliriz. index.html sayfası aslında haber kaynağına aboneliği başlatıp durdurabileceğimiz bir test sahası gibi. Tek dikkat edilmesi gereken nokta manifest dosyası ile bağ kurulmuş olması. Tasarım son derece aptalca yapıldı ama esas amacımız elbette şukela bir görsellik sunmanın dışında push notification kabiliyletlerini deneyimlemek. Index sayfası ile işe başlayalım.

efes_barca.html isim dosya da şimdilik bildirim alındığında gösterilecek içeriği barındırıyor. Bunun dinamik olduğunu bir düşünsenize. Abone olduktan sonra yeni bir haber geldiğinde efes_barcha benzeri dinamik HTML içerikleri kullanılacak. Tüm uygulamayı tamamladıktan sonra bu tip bir özellikle örneğinizi daha da zenginleştirebilirsiniz.

Sworker modülü aslında Service Worker görevini üstlenmekte. Kodlar arasına katmaya çalıştığım yorumlarla mümkün mertebe neler olduğunu anlamaya ve anlatmaya çalıştım.

Bu taraf için son olarak main içeriğini de aşağıdaki gibi geliştirebiliriz.

İlk Test
Kodları tamamladıktan sonra kısa bir test ile push notification hizmetinin çalışıp çalışmadığı hemen kontrol edilebilir. Bunun için terminalden

komutunu verip uygulamayı ayağa kaldırmamız yeterli. Eğer aşağıdaki ekran görüntülerindekine benzer sonuçlar elde edebiliyorsak REST API uygulamasını yazmaya başlayabiliriz.

![07_31_credit_2.png](/assets/images/2019/07_31_credit_2.png)

![07_31_credit_3.png](/assets/images/2019/07_31_credit_3.png)

Uygulama, Push Notification hizmeti için abone olunurken benzersiz bir ID değeri alır. Firebase Cloud Messaging sistem bu değeri kullanarak kime bildirim yapılacağını bilebilir.

REST API Uygulamasının Yazılması

Abone olan uygulamaların ID bilgilerini yönetmek için Node.js tabanlı bir REST servisi yazabiliriz. Servis temel olarak PWA'nın FCM ile olan iletişiminde devreye girmektedir. Hem abonelik yönetimi hem de istemcilere bildirim yapılması ki bunu tek başına yapmayacaktır. Service Worker dolaylı olarak FCM üzerinden bu servisle yakın ilişki içerisindedir.

Bu servisi ayrı bir klasörde projelendirmek iyi olur. Pek tabii node.js tarafında REST Servisi yazımını kolaylaştırmak için bazı paketlerden destek alınabilir. express dışında HTTP mesajlarındaki gövdeleri kolayca ele almak için body-parser, servisin Firebase Cloud Messaging ile konuşabilmesini sağlamak amacıyla da fcm-node paketi kullanılablir. morgan modülünü ise sunucu tarafındaki HTTP trafiğini loglamak için değerlendirebiliriz. Aşağıdaki terminal komutları ile klasör ağacını oluşturup server dosyasını kodlayarak derlememize devam edelim.

server modülü

Çalışma Dinamikleri

Uygulamanın çalışma dinamiklerini anlamak oldukça önemli. Index.html olarak düşündüğümüz web uygulamamız çalıştırıldığında iki aksiyonumuz var. Basketbol topuna basıp bir abonelik başlatmak veya tekrar basarak aboneliği durdurmak.

Abonelik başlatıldığında FCM benzersiz bir ID değeri üretir ve bunu PusherAPI servisi kendisine gelen çağrı ile kayıt altına alır (diziye eklediğimiz yer) Sonraki herhangi bir t zamanında PusherAPI servisinin abonelere bildirim gönderen HTTP metodu tetiklenirse, Firebase Cloud Messaging devreye girer ve dizideki ID bilgilerini kullanarak abonelerine bildirimde bulunur. Bildirimler web uygulaması tarafındaki Service Worker (sworker.js) tarafından push olayıyla yakalanır. Push olayı şimdilik sadece statik bir sayfa gösterimi yapmakta ki aslında asıl içeriği yine servis üstünden veya web aracılığıyla başka bir adresten aldırabiliriz.
Çalışma Zamanı (Development Ortamı)
Testler için PWA ve servis tarafını ayrı ayrı çalıştırmalıyız.

terminal komutu ile web uygulamasını

ile de REST servisini başlatabiliriz. Aboneliği başlattıktan sonra http://localhost:8080/news/push adresine talepte bulunursak bir bildirim mesajı ile karşılaşırız (sworker daki push olayı tetiklenir) Aynen aşağıdaki ekran görüntüsünde olduğu gibi.

![07_31_credit_4.png](/assets/images/2019/07_31_credit_4.png)

Bildirim kutusuna tıklarsak statik olarak belirlediğimiz sayfa açılacaktır (yani notificationclick olayı tetiklenir)

![07_31_credit_5.png](/assets/images/2019/07_31_credit_5.png)
PWA ve Service Uygulamalarının Firebase Hosting'e Alınması
Her iki uygulamada local geliştirme ortamında gayet güzel çalışıyor. Ancak bunu anlamlı hale getirmek için her iki ürünü de Firebase üzerine alıp genel kullanıma açmamız lazım. Web uygulamasını Firebase Hosting ile REST servisini de Firebase Function ile yayınlamalıyız. Bu işlemler için firebase-tools aracına ihtiyacımız olacak. Terminalden aşağıdaki komutu kullanarak ilgili aracı sisteme yükleyebiliriz.

Basketkolik'in Dağıtımı
Yeni bir dağıtım klasörü oluşturmalı, initializion işlemini gerçekleştirip basketkolik uygulama kodlarını oluşan public klasörü içerisine atmalıyız. Ardından üzerinde çalışacağımız projeyi seçip deploy işlemini yapabiliriz. Bu işlemler için aşağıdaki terminal komutlarından yararlanılabilir.

firebase init işleminde bize bazı seçenekler sunulacaktır. Burada aşağıdaki görüntüde yer alan seçimlerle ilerleyebiliriz. En azından ben öyle yaptım.

![07_31_credit_6.png](/assets/images/2019/07_31_credit_6.png)

Eğer dağıtım işlemi başarılı olursa aşağıdaki ekran görüntüsündekine benzer sonuçlar elde edilmelidir.

![07_31_credit_7.png](/assets/images/2019/07_31_credit_7.png)
PusherAPI servisinin Dağıtımı
Hatırlanacağı üzere web uygulaması bir REST Servisi yardımıyla FCM sistemini kullanıyordu. PusherAPI isimli uygulama, Fireabase tarafı için bir Function anlamına gelmektedir (Serverless App olarak düşünelim) Ölçeklenebilirliği, HTTPS güvenliği, otomatik olarak ayağa kalkması gibi bir çok iş Google Cloud ortamı tarafından ele alınır. Şimdi aşağıdaki terminal komutu ile fonksiyon klasörünü oluşturalım (dist klasörü içerisinde çalıştığımıza dikkat edelim)

Yine bazı seçenekler karşımıza gelecektir. Burada gelen sorulara şöyle cevaplar verebiliriz;

Dil olarak Javascript seçelim.
ESLint kullanımına Yes diyelim.
npm dependency'lerin kurulmasına da Yes diyelim ki uygulamanın gereksinim duyduğu node paketleri de yüklensin.

Devam eden adımda functions klasöründeki index dosyasının içeriğini PusherAPI'deki server içeriği ile değiştirmeliyiz. Ancak bu kez express'in firebase-functions ile kullanılması gerekiyor. İhtiyacımız olan express, body-parser ve fcm-node paketlerini üzerinde çalıştığımız functions klasörü içinede de yüklemeliyiz. Son olarak dist klasöründeki firebase.json dosyasına rewrites isimli yeni bir bölüm ekleyip fonksiyonumuzu deploy edebiliriz.

Yapmamız gereken bir şey daha var. Web uygulamasının kullandığı main dosyasının içeriğini, yeni eklediğimiz google functions ile uyumlu hale getirmek. Tahmin edileceği üzere gidilen servis adreslerini, oluşturulan firebase proje adresleri ile değiştirmemiz lazım (dist/public/main.js içeriğini kontrol edin) Web uygulamasındaki bu değişikliği Cloud ortamına taşımak içinse public klasöründeyken yeniden bir deploy işlemi başlatmamız yeterli olacaktır.

Çalışma Zamanı (Production Ortamı)
Uygulama artık https://basketin-cepte-project.firebaseapp.com/ adresinden yayında (En azından bir süre için yayındaydı ki aşağıdaki ekran görüntüsü de bunun kanıtıdır)

![07_31_credit_8.png](/assets/images/2019/07_31_credit_8.png)
Ben Neler Öğrendim?
Bu çalışmanın da bana kattığı bir çok şey oldu elbette. Özellikle bir uygulamaya uzak sunuculardan bildirim yollanması ve bunun abonelik temelli yapılması merak edip öğrenmek istediğim konulardan birisiydi. İşin içerisine basit bir PWA modeli de ekleyince çalışma ilgi çekici bir hal almıştı. Yapılan hazırlıklar düşünüldüğünde aslında bizi çok fazla yormayacak bir geliştirme süreci olduğunu ifade edebiliriz. Derlemeyi sonlandırmacan önce yanıma kar kalanların neler olduğunu aşağıdaki maddeler ile özetleyebilirim.

Firebase Cloud Messaging (FCM) sisteminin kabaca ne işe yaradığını
PWA uygulamasının FCM ile nasıl haberleşebileceğini
Abone olan istemciye bildirimlerin nasıl gönderilebileceğini
Service Worker üzerindeki push ve notificationclick olaylarının ne anlama geldiğini
serve paketinin kullanımını
firebase terminal aracı ile deployment işlemlerinin nasıl yapıldığını
Web uygulaması ve Functions'ın Google Cloud tarafından bakıldığında farklılıklarını

Bu arada proje büyük ihtimalle Google platformundan kaldırılmıştır. Malum istenmeyen bir yüklenme sonrası yüklü bir fatura ile karşılaşmamak adına küçük bir tedbir olduğunu ifade edebilirim. O nedenle kendiniz başarmaya çalışırsanız daha kıymetli olacaktır. Konseptin basketbol olması önemli değil. Abonelerinize bildirimlerde bulunacağınız herhangi bir senaryo olması yeterli olur. Bildirim yapan servisi de planlanmış bir düzeneğe bağlayabiliriz. Söz gelimi günün belirli anlarında bir konu ile ilgili bildirimlerin yönlendirilmesi işini üstlenebilir. Böylece geldik bir [saturday-night-works](https://github.com/buraksenyurt/saturday-night-works) macerasının daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.