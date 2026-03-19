---
layout: post
title: "TFS OData Desteği"
date: 2013-08-25 10:35:00 +0300
description: "Özellikle ALM(Application Lifecycle Management) gibi geniş konuların uygulandığı ürünlerin değerlendirildiği firmalar ve kalabalık ekipleri düşünüldüğünde, bu heterojenlik kendini iyiden iyiye hissettir. Dolayısıyla ürünün geliştiriciler açısından ne kadar ve nasıl genişletilebileceği önem kazanır..."
categories:
  - team-foundation-server
tags:
  - team-foundation-server
  - xml
  - json
  - http
  - javascript
  - visual-studio
---
# TFS OData Desteği
[![almworld](/assets/images/2013/almworld_thumb.jpg)](/assets/images/2013/almworld.jpg)[Orjinal Yazım Tarihi 3/20/2013]

Merhaba Arkadaşlar,

Çoğu zaman geliştirilen yazılım ürünleri ile farklı profilden insanları ortak bir payda da buluşturmayı hedefleriz. Farklı özelliklere sahip insanları, ürüne nasıl katabileceğimizi keşfetmeye çalışırız. Tabi geliştirilen ürünün hedef kitlesi de burada önemli bir rol oynar.

Bazı ürünlerin arayüzlerinin son derece basit tasarlanması yeterli iken bazılarında ise tam tersi bir durum söz konusudur.

Hangisi olursa olsun kullanıcı bir insan olarak düşünüldüğünde çok da fazla zorlanmamalı veya kolayca adapte olabilmelidir. Ne kadar kolay kullanılırsa, hedef kitle içerisinde o kadar fazla sayıda farklı profile de ulaşılabilinir. Ancak bazı hallerde ürünün hedef kitlesi o kadar dağınıktır ki, hepsini çekebilmek ya da bir başka deyişle kazanabilmek için yapılan genişletmeler yeterli gelmeyebilir. Böyle bir durumda çevreye şu mesajı vermeniz gerekebilir;

> Ey ahali…Bu gördüğünüz, ürünümüzün dışarıya açılmış olan servisi/servisleri/sdk’sı/api’si. Buyrun istediğiniz gibi uyarlayın, kullanın. Sonuçta ürünümüzün yaşamının bir parçası olabileceksiniz
>
> ![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_195.png)

Özellikle ALM (Application Lifecycle Management) gibi geniş konuların uygulandığı ürünlerin değerlendirildiği firmalar ve kalabalık ekipleri düşünüldüğünde, bu heterojenlik kendini iyiden iyiye hissettir. Dolayısıyla ürünün geliştiriciler açısından ne kadar ve nasıl genişletilebileceği önem kazanır.

> Ekipleriniz içindeki profilleri düşünün! Yazılımcılar IDE’ leri, iş analistleri Word dokümanlarını, Müdür’ ler web browser üzerinden erişilebilen raporları, Release Manager’ lar Team Explorer’ ı, Proje Yöneticileri Ms Project’ i, CIO’ lar ise ürünlerinin hangi sprint’ ler de olup ne kadarlık işlerinin kaldığını okuyabildikleri e-postaları, sever. Listeyi uzatmak mümkün
>
> ![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_195.png)

Bu felsefeden baktığımızda bence Microsoft’ un Team Foundation Server ürünü epey önemli bir noktada yer alıyor. Hatta Gartner’ ın Application Lifecycle Management konusundaki bi raprunda yer alan Magic Quadrant grafiği de, bunu doğrular nitelikte. Kabiliyet ve sunulan vizyon açısından Microsoft liderler arasında en iyi noktada yer alıyor diyebiliriz (Rapor hakkında detaylı bilgiye [bu adresten](http://www.gartner.com/technology/reprints.do?id=1-1ASCXON&ct=120606&st=sb) ulaşabilirsiniz)

[![odatatfs_15](/assets/images/2013/odatatfs_15_thumb.png)](/assets/images/2013/odatatfs_15.png)

Peki TFS takımı bunu nasıl başarıyor?

Bildiğiniz üzere Team Foundation Server ailesi içerisinde,.Net tabanlı kullanılabilen Object Modeller (Client Object Model, Server Object Model, Build Process Object Model), yabancı ürünlerin entegre olabilmesini sağlayan Provider’ lar (MSSCCI Provider ve Team Explorer Everywhere), servis bazlı entegrasyon için XML Web hizmetleri var. Ayrıca bilindiği üzere TFS in Cloud tabanlı çalışan bir başka verisyonu daha bulunmakta.

Cloud Tabanlı TFS

Team Foundation Server bilindiği üzere bir süredir Cloud Service olarak da hizmet vermekte. 5 Windows Live ID hesabına kadar ücretsiz kullanılabilen hizmet, sunucu modelli kurulum sonucu yapılabilen hemen herşeyi karşılamakta. Scrum, MSF, CMMI gibi şablonları doğrudan destekleyen servis, TFS Web Access arayüzü ile de oldukça kolay bir kullanıma sahip. Visual Studio ailesine, Excel gibi ofis ürünlerine kolayca bağlanabilmekte. Hatta son zamanlarda Git ile olan entegrasyonu sayesinde, Git fanatiği geliştiricilerin de dikkatini çekmeyi başardı.

Bir bulut servisi olduğu için, TFS ortamının kurulumunu düşünmemize gerek yok. Sadece abone oluyor veya lisanslı kullanıcı iseniz kira bedelini ödüyorsunuz. Kurulumu düşünmüyor olmanız beraberinde ölçeklenebilirlik (Scalability), sunucu performansı, donanım alımı, personel istihdamı gibi mevzuları da düşünmemize gerek olmadığı anlamına gelmekte.

> Pek tabi bazı kurumlar halen daha bulut servislerine temkinli yaklaşmakta ve hatta doğrudan geri çevirmekte. Örneğin ülkemizdeki BDDK gibi kurumlar, bankaların bu tip bulut tabanlı yapılar ile çalışmalarını ve bilgi alışverişinde bulunmalarını epeyce sorgulamakta ve kolay kolay izin vermemekte.
> Açıkçası askeri ve stratejik açıdan düşünürsek ülke ekonomisine ait değerli bilgilerin 3ncü parti sunucularda, ülke dışında tutmak çok da doğru değil bana kalırsa.

[http://tfs.visualstudio.com](http://tfs.visualstudio.com) adresinden ulaşılabilen hizmete son aylarda eklenen ve halen geliştirilme aşamasında olan önemli bir yenilikte OData (Open Data Protocol) servis desteği. XML (eXtensible Markup Language) üzerine oturan ve URL bazlı sorgulama yetenekleri tanıyan bu protokol, bir dünya standardı. Standardın hedefi ise veri odaklı yayınlayıcılar (Publishers).

Hal böyle olunca herhangibir veri kaynağının, özellikle internet ortamı üzerinden OData protokolüne göre sorgulanabilmesi mümkün hale geliyor. Bunun tam karşılığı ise, platform bağımsızlıktan başka bir şey değil. Ama bu son derece önemli bir yetenek. Nitekim bulut üzerinde koşan TFS hizmetini kullanabilecek istemcileri her hangi bir platform için geliştirebileceğimiz anlamına gelmekte.

[OData](http://www.odata.org/) protokolünün ön gördüğü veri odaklı sorgular bilindiği üzere tamamen URL bazlı olarak çalışmakta. Bu nedenle bulut TFS servisi üzerinde duran bilgileri basit bir tarayıcı uygulamayı kullanarak sorgulayabilirsiniz de. Bunun sonucu olarak dilerseniz TFS projenizin bazı raporlarını veya yönetsel arabirimi özelliklerini, örneğin mobil cihazınıza kadar indirebilirsiniz. (Bu konu ile ilişkili olarak [Nisha Singh’ in Windows 8 uygulamasına](http://blogs.msdn.com/b/nishasingh/archive/2013/01/08/tfs-dashboard-a-sample-windows-8-store-app-for-team-foundation-server.aspx) bir göz atmanızı öneririm)

Gelelim bu yazımızdaki konumuza. Hiç kod yazmayacak ve geliştirme yapmayacağız aslında

![Disappointed smile](/assets/images/2013/wlEmoticon-disappointedsmile_5.png)

Bir tarayıcı uygulama, tfs.visualstudio.com üzerindeki hesabımız ve OData servislerinden yararlanarak sorgulamalar gerçekleştireceğiz.

Seddulbahir

Bu amaçla ben seddulbahir.visualstudio.com adresinde konuşlandırılmış olan ve sahibi olduğum Team Project Collection alanını kullanıyor olacağım. OData örnekleri için çok basit olarak SoniK isimli uydurmasyon bir Team Project oluşturdum. Söz konusu proje Scrum 2.2 şablonuna göre kullanılmakta. Şimdilik tek üyesi benim ve tüm Task’ lar üzerimde

![Confused smile](/assets/images/2013/wlEmoticon-confusedsmile_30.png)

> tfs.visualstudio.com servisinin en güzel yanlarından birisi de, son güncellemeleri haberiniz olmasa dahi hızla ve ilk elden implemente ediyor oluşu. Söz gelimi Scrum’ ın yeni bir versiyonunun çıktığını fark etmemiş olabilirsiniz. Ama bulut üzerinde bu güncelleme çıktığı gibi entegre edilmiştir de.

Tabi OData servisleri ile bir Team Project’ in sorgulanması denince dikkatler hemen Work Item içeriklerine çevrilecektir. Yani Product Backlog Item, Task, Test Case, Bug, Impediment gibi öğelere (Scrum için söz konusu olan bu Work Item çeşitleri, seçilen süreç şablonuna göre elbetteki değişiklik gösterebilir) Bu nedenle SoniK isimli proje içerisine aşağıdaki ekran görüntüsünde yer alan bazı Work Item’ ları ekledim ve bunları şimdilik, 2 haftalık süreye sahip olan Sprint 1 içerisinde değerlendirmeye aldım. Görüldüğü gibi TO DO’ dan IN PROGRESS’e aldığım iki Task’ ım var

![Smile](/assets/images/2013/wlEmoticon-smile_95.png)

[![odatatfs_2](/assets/images/2013/odatatfs_2_thumb.png)](/assets/images/2013/odatatfs_2.png)

[![odatatfs_4](/assets/images/2013/odatatfs_4_thumb.png)](/assets/images/2013/odatatfs_4.png)

Sorgulama işlemine başlamadan önce yapılması gereken küçük bir hazırlık daha var. OData servislerini etkinleştirmek için TFS hesabımızın profil özelliklerinden Enable Aternate Credentials seçeneğini aktif hale getirmemiz ve bir kullanıcı adı ile şifre belirlememiz gerekiyor.

[![odatatfs_1](/assets/images/2013/odatatfs_1_thumb.png)](/assets/images/2013/odatatfs_1.png)

Bu işlemin ardından [https://tfsodata.visualstudio.com/defaultcollection](https://tfsodata.visualstudio.com/defaultcollection) adresine girerek başlama vuruşunu yapabiliriz

![Smile](/assets/images/2013/wlEmoticon-smile_95.png)

domainAdı\kullanıcıAdı ve şifre ile giriş yapabiliriz. Örneğin benim TFS projem seddulbahir.visualstudio.com olduğundan, seddulbahir\AlternatifKullanıcıAdı ve şifre ile giriş yapmam gerekiyor. defaultcollection adresini sorguladığımızda standart bir OData servisinden beklediğimiz sonuçlar ile karşılaşırız. Bize TFS hizmeti için sorgulanabilir olan Entity adlarının adreslerini içeren bir sayfa üretilecektir. Aşağıdaki ekran görüntüsündeki gibi.

[![odatatfs_3](/assets/images/2013/odatatfs_3_thumb.png)](/assets/images/2013/odatatfs_3.png)

Dikkat edileceği üzere kullanıcılardan Area’ lara, Work Item’ lardan, Team Project Collection içerisindeki Team Project'lere, Build tanımlamalarından, Iteration’ lara kadar sorgulanabilecek oldukça geniş bir yelpaze söz konusudur. Şimdi dilerseniz örnek projemiz için bir kaç OData sorgusu icra edelim ve sonuçları görmeye çalışalım.

Örnek Sorgular

Aşağıdaki tabloda sorguya ait URL ifadeleri, bazı kısa açıklamalar ve SoniK için üretilen sonuçlara ait ekran görüntüleri yer almaktadır.

Team Project Collection içerisinde yer alan Team Project öğelerinin çekilmesi

https://tfsodata.visualstudio.com/DefaultCollection/Projects

Yukarıdaki sorgu hesabımıza ait Team Project Collection içerisinde ne kadar Team project var ise döndürür. Dikkat çekici entry özelliklerinden birisi, belirli bir projeye erişebilmek için id elementi ile gelen URL değeridir.

[![odatatfs_5](/assets/images/2013/odatatfs_5_thumb.png)](/assets/images/2013/odatatfs_5.png)

Belirli bir Team Project içerisinde tanımlanmış olan Area bilgilerinin çekilmesi

https://tfsodata.visualstudio.com/DefaultCollection/Projects ('Sonik')/AreaPaths

Bir proje içerisinde pek çok Area tanımlanmış olabilir. Genellikle aynı projede birden fazla takımın kendilerine ait Area’ lar üzerinde yetkilendirilerek çalışması gibi ihtiyaçlarda ideal bir çözümdür. Sorguda önce Projects üzerinden SoniK’ e gidilmiş ve AreaPaths alt entity içeriği talep edilmiştir.

[![odatatfs_6](/assets/images/2013/odatatfs_6_thumb.png)](/assets/images/2013/odatatfs_6.png)

Bir Area’ nın altında yer alan alt Area’ ların çekilmesi

https://tfsodata.visualstudio.com/DefaultCollection/AreaPaths ('SoniK%3CDevelopment')/SubAreas

Bazı durumlarda bir Area altında birden fazla Child Area açılmış olabilir. Hatta bu, bir kaç seviyelendirme şeklinde yapılmış da olabilir. Örnek projede aşağıdaki gibi bir Area yapısı söz konusudur ve yukardaki OData sorgusu ile bu içerik XML formatında elde edilebilir.
[![odatatfs_7](/assets/images/2013/odatatfs_7_thumb.png)](/assets/images/2013/odatatfs_7.png)

[![odatatfs_8](/assets/images/2013/odatatfs_8_thumb.png)](/assets/images/2013/odatatfs_8.png)

Bir Team Project için söz konusu olan Work Item Öğelerinin toplam sayısını bulmak

https://tfsodata.visualstudio.com/DefaultCollection/Projects ('Sonik')/WorkItems/$count

Örneğin Sonik isimli Team Project içerisinde yer alan toplam work item sayısını count OData fonksiyonundan yararlanarak bulabiliriz.

[![odatatfs_9](/assets/images/2013/odatatfs_9_thumb.png)](/assets/images/2013/odatatfs_9.png)

Belirli bir Id değerine sahip Work Item’ ın elde edilmesi

https://tfsodata.visualstudio.com/DefaultCollection/Projects ('Sonik')/WorkItems (3)

Bu sorgu ile 3 numaralı ID değerine sahip Work Item bilgisi elde edilmektedir. Tabi çok fazla özellik olduğundan element sayısı da epeyce fazladır. Azaltmak için bir sonraki örnekte olduğu gibi projection type kullanımı tercih edilebilir.

[![odatatfs_10](/assets/images/2013/odatatfs_10_thumb.png)](/assets/images/2013/odatatfs_10.png)

Work Item’ lardan ilk 2sinin sadece Title ve Type bilgilerini çekmek

https://tfsodata.visualstudio.com/DefaultCollection/Projects ('Sonik')/WorkItems?$select=Title,Type&$top=2

Burada aslında bir projection kullanımı söz konusudur. select anahtar kelimesini takiben, çekilmek istenen özelliklerin adları verilmiştir. top anahtar kelimesi ile de kaç tane element çekileceği ifade edilir.

[![odatatfs_11](/assets/images/2013/odatatfs_11_thumb.png)](/assets/images/2013/odatatfs_11.png)

Sorgu çıktısını JSON (JavaScript Object Notation) formatında elde etmek

https://tfsodata.visualstudio.com/DefaultCollection/Projects ('Sonik')/WorkItems?$select=Title,Type&$top=3&$format=json

Çıktının sadece XML formatında elde edilmesi şart değildir. Örneğin daha az yer tutan JSON tipinde bir çıktı üretilmesi için format anahtar kelimesinden yararlanılabilir. Yukarıdaki sorguda Sonik projesindeki ilk 3 Work Item’ ın Title ve Type değerlerini içeren JSON çıktısı talep edilmektedir. Sonuç aşağıdaki gibidir.

[![odatatfs_12](/assets/images/2013/odatatfs_12_thumb.png)](/assets/images/2013/odatatfs_12.png)

Bir projedeki belirli bir Work Item tipine ait öğeleri çekmek

https://tfsodata.visualstudio.com/DefaultCollection/WorkItems?$filter=Project eq 'Sonik'and Type eq 'Test Case'&$select=Project,Type,Title,AreaPath

Örnek sorguda filter ve select anahtar kelimelerinden yararlanılmış olup, Sonik isimli projede yer alan Test Case tipindeki Work Item’ ların Project, Type, Title ve AreaPath bilgileri talep edilmiştir.

[![odatatfs_13](/assets/images/2013/odatatfs_13_thumb.png)](/assets/images/2013/odatatfs_13.png)

Belirli bir Sprint içindeki Work Item bilgilerinin çekilmesi

https://tfsodata.visualstudio.com/DefaultCollection/WorkItems?$filter=Project eq 'Sonik'and Type eq 'Product Backlog Item'and IterationPath eq 'SoniK\Release 1\Sprint 1'&$select=Project,Type,Title,AreaPath,IterationPath

Örnekte Sprint 1 içerisine alınmış olan Product Backlog Item öğelerinin Project, Type, Title, AreaPath, IterationPath değerleri sorgulanmaktadır. filter anahtar kelimesi ile birden fazla kriterin hesaba katılması noktasında and ve or operatörlerinden yararlanılır.

[![odatatfs_14](/assets/images/2013/odatatfs_14_thumb.png)](/assets/images/2013/odatatfs_14.png)

Şu an için TFS Odata servis sorgularında filter, count, select, orderby, top, skip, format ve callback anahtar kelimeleri kullanılabilmektedir. Ancak bu anahtar kelime seti artabilir. Sorgular sırasında? ve $ harflerine de dikkat edilmelidir. Tüm OData komutlarının önünde dikkat edileceği üzere $ harfi yer almaktadır. Her ne kadar örneklerde ağırlıklı olarak Work Item’ lar üzerinde durulmuş olsa da DefaultCollection altında sunulan Entity’ lerin çoğu üzerinde sorgulamalar yapılabilir. Bunu denemenizi öneririm

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_195.png)

Görüldüğü üzere söz konusu OData sorgularını kullanarak farklı platformlar üzerinde çalışacak istemci uygulamaların geliştirilmesinin önü son derece açıktır. Şu anda halen geliştirilmekte olan [TFS OData servislerinin kullanımına ilişkin detaylı bilgileri bu adresten takip edebilirsiniz](https://tfsodata.visualstudio.com/). Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Orjinal Yazım Tarihi 3/20/2013]