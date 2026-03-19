---
layout: post
title: "Sekiz Saatlik Sonsuz Döngü"
date: 2019-12-27 07:00:00 +0300
description: "Uygulama geliştirme yaşam döngümüzün otuzuncu sprintinin ilk günlerinden birisiydi. İki haftalık koşu görevlerini Sprint Planning toplantısında zaten belirlemiştik. Takım olarak 13 Story Point’e sahip Production Support Buffer mecburen her sprint içerisine dahil ettiğimiz bir maliyet. 17 yaşındaki Microsoft .Net tabanlı devasa ERP(Enterprise Resource Planning) ürünümüz ek geliştirmeler veya önceki yıllardan kalan teknik borçlar sebebiyle bazen üretim ortamı sorunları ile karşımıza gelmekte. Büyüklüğüne nazaran Code Coverage oranının düşük olması yeni ilavelerin var olan yapılara olan etkisini anlamamızı zorlaştırıyor. Ben, Mali İşler ve Ortak Modüller(Kimsenin bilmediği bir modül varsa böyle gelin) ekibindeyim. Lakin ERP’nin diğer modüllerinde de benzer sorunlar olabiliyor."
categories:
  - deneyimler
tags:
  - deneyimler
  - dotnet
  - aspnet
  - sql-server
  - postgresql
  - wcf
  - soap
  - rest
  - web-api
  - javascript
  - typescript
  - nodejs
  - vue
  - blazor
  - performance
  - transactions
  - microservices
---
# Sekiz Saatlik Sonsuz Döngü
![shellby_mini.png](/assets/images/2019/shellby_mini.png)

Uygulama geliştirme yaşam döngümüzün henüz otuzuncu Sprint başındaydı. İki haftalık koşu görevlerini Sprint Planning toplantısında zaten belirlemiştik. Takım olarak 13 Story Point’e sahip Production Support Buffer mecburen her sprint içerisine dahil ettiğimiz bir maliyet. 17 yaşındaki Microsoft.Net tabanlı devasa ERP (Enterprise Resource Planning) ürünümüz ek geliştirmeler veya önceki yıllardan kalan teknik borçlar sebebiyle bazen üretim ortamı sorunları ile karşımıza gelmekte. Büyüklüğüne nazaran Code Coverage oranının düşük olması yeni ilavelerin var olan yapılara olan etkisini anlamamızı zorlaştırıyor. Ben, Mali İşler ve Ortak Modüller (Kimsenin bilmediği bir modül varsa böyle gelin) ekibindeyim. Lakin ERP’nin diğer modüllerinde de benzer sorunlar olabiliyor.

> (farkına varmayız)

O gün her zaman ki gibi Madelein yanıma oturdu ve acil çözüm bekleyen müşteri sorunlarının en önemli olanlarından birisinden bahsetmeye başladı. Tüm öncelikleri bir kenara bırakıp bu üretim problemine yoğunlaşmam ve en kısa sürede çözüme ulaştırmam gerekiyordu. Bu, bazı stand-up meeting toplantıları sonrası yaşadığımız standart bir durum. Gün geçtikçe azalan ama bir süre daha hayatımızın parçası olacak bir gerilim hatta. Her ne kadar müşterinin de dahil olduğu kurumsal çaptaki Dijital Board ile işler önceliklendirilse de, bayilerden ve ana yüklenici firmadan gelen üretim ortamı sorunları göz ardı edilemiyor.

Bir bayinin kısa süreliğine de olsa fatura kesememesi ya da stok sahasındaki mobil cihaz süreçlerinde meydana gelen yavaşlık nedeniyle durma noktasına gelen sevkiyat, elbette ortaya çıkan zararlar düşünülünce kriz ortamının bir mantar bulutu gibi geliştiricinin masasında patlamasına neden olmakta (Ve kimse radyoaktif maddeyle kaplanmış bir geliştiricinin yakınlarında olmayı tercih etmez)

Bu nedenle müşteri isteklerinin doğru şekilde parçalanması, ortak paydaşlarda el sıkışılması, önceliklendirmelerin ve iş değerlerinin üst kademeden itibaren şeffaf bir şekilde görülebilmesi elzem derecede önemli. Bunu aşmak için modüllerdeki iş sahipleri ve scrum master’lar belirli aralıklarda müşteri ve üst yönetim ile bir araya gelerek kabul kriterleri üzerinde müzakerelerde bulunup vaziyet hakkında bilgi paylaşımında bulunuyorlar. Bu sayede her şeyin şeffaklıkla tüm birimlere akması sağlanıyor.

### Shelby’nin Monolitik Dünyası

Tamamiyle monolitik karakteristikte bir yazılım olarak niteleyebileceğimiz ERP ürünü gerçek bir Legacy sistem. 2002 yılında.Net Framework’ün ilk sürümüyle birlikte geliştirilmeye başlanmış, N-Tier mimariye göre yazılmış, zaman içerisinde SOA (Service Oriented Architecture) evrimini gerçekleştirmiş, tek Microsoft SQL Server örneği ile çalışan, 8500 ekran, 4500 tablo, 27binden fazla Stored Procedure’den oluşan ve sahada 10bin personel tarafından kullanılan bir Asp.Net Web Forms uygulaması (Şimdi pek çoğunuz “ne kadar da demode bir teknoloji kullanıyorlar” diyebilirsiniz ama unutmayın; Bu monolitik mimari yıllardır ve halen müşteri için olan görevini başarıyla yerine getiriyor)

Shelby, çevresel olarak entegre olduğu çeşitli tipte sayısız servise de sahip. Eski nesil SSIS (Sql Server Integration Services) paketlerinden yeni nesil REST servislerine, kurum dışı WCF uç noktalarından SOAP bazlı Web hizmetlerine kadar oldukça geniş bir dal budak yığını söz konusu. Sektörün bir gereği olarak yurt dışı firmalarla yapılan bir çok entegrasyon noktası bulunuyor. Bazılarıyla TCP gibi protokollerle iletişim kuruluyor. Regülasyonlar sebebiyle devlet kurumları ile entegre olunan noktalar da mevcut. Kocaman bir ekosistem işte:)

Güncel.Net Framework sürümüne evrilmiş olsa da SonarQube ile başlanan teknik borç azaltma çalışmaları öncesi yaklaşık 2 milyon satır koda sahip olan bir üründen söz ediyoruz. Big Bang öncesi %17 kadarı tekrarlı koddan oluşuyordu (Doğruyu söylemek gerekirse turuncu bankada çalışırken gördüğüm C ile yazılmış devasa muhasebe paketinden sonra Tanrı Nesneler-God Object içeren bir uygulama daha görmek şaşırtıcı değil) SonarQube, çeşitli seviyelerden gelen toplam teknik borcun temizlenmesi için 786 adam günlük bir tahminlemede bulundu.

Teknik borçların azaltılması şarttı çünkü bir sene öncesinde başlanmış olan dönüşüm süreci kapsamında yeni araç ve kültürlere adapte olunurken Shelby’nin modernize edilmesi gerekiyordu. Terk edilemeyecek kadar önemli bir role sahip olan ürünün baştan yazılması maliyeti ciddi anlamda yüksekti. Bu nedenle hedeflerden birisi sonraki yıl onun teknik borcunun sıfırlanması olarak belirlenmişti. Bunu yapmazsak CI/CD hattındaki Quality Gate noktasına takılacağımız ve Gandalf’ın o meşhur ‘you shall not pass’ sözüyle karşılaşacağımız aşikar.

![last_1.png](/assets/images/2019/last_1.png)

İşin başında ve sonrasında,

![last_2.png](/assets/images/2019/last_2.png)

> ThoughtWorks
> Anti-Pattern

SonarQube’nin pek çok çıktısı benzer kural ihlallerini verdiğinden genç arkadaşlarımızdan oluşan bir ekip (deneyimli yazılımcıların yanına eklenen stajyerlerden oluşan bir grup parlak beyin) Roslyn ile kokan kısımları hızlıca bertaraf etti. İşin başında teknik borcun farkında olup ürünleşme yolundaki engelleri de bilen ve CEO’ya doğrudan raporlama yapan yazılım grup müdürü dahi vardı. Bizzat kodlamaya katıldığını ve teknik borcun temizlenmesi için elinden geleni yaptığını gözlerimle gördüm. Kısa sürede teknik borcun akıllı kod parçaları ile 133 güne kadar indiği görülmüştü. Asıl sorun Complexity değerleri yüksek olan God Object damgalı sınıfların nasıl ayıklanacağıydı. İşte bu noktada daha önceden öğrenip de ne zaman kullanırız dediğimiz bazı kavramlar değer kazanıyor (Çok fazla if bloğu veya switch koşulu barındıran ve bu nedenle Complexity değeri yüksek çıkan bir fonksiyonda hangi tasarım kalıbını kullanarak çözüm üretirsiniz?)

### Dönüşmeye Çalışmak ve Sıkıntılar (Dijital Dönüşüm)

Çeşitli ürün gruplarından farklı teknolojilerle geliştirilmiş bir çok sistemin bir arada yer aldığı kaotikleşmiş, bakım maliyetlerinin yüksek olduğu, sirkülasyon nedeniyle bilgi kaybının yaşandığı, basit bir kod düzeltmesinin (hotfix gibi) çıkılması için dahi rutin geçiş gününün beklendiği bir ortam söz konusuydu. ERP bir yana yeni nesil ürünler ile de haşır neşir olunuyor, kaçınılmaz modernizasyon çalışmaları pek çok ekipçe planlamalara dahil ediliyordu.

Kabaca bir yanda yeni nesil savaş uçağının geliştirilmesi, diğer yanda kırk yıllık tankların modernize edilmesi öteki tarafta amfibik yetenekleri ile diğer unsurları taşıyacak uçak gemisinin yapımı noktasında çalışan bir çok insan olduğunu hayal edin. Akıllı ve dikkatlice hareket edilmediği takdirde 135mm tank topu taşıyan ve uçacağına kesin gözüyle bakılan bir mühendislik harikası icat etmeniz içten bile değil.

Bu bağlamda çevik metodolojilere adapte olunup Scrum ile yürünmesine, DevOps kültürünün benimsenmesine, ürün taşıma sisteminin yani CI/CD (Continuous Integration/Continuous Delivery-Deployment) hattının kurgulanıp TFS yerine git bazlı Azure DevOps platformuna geçilmesine karar verildi. Feature bazlı geliştirmeyi desteklemek adına Git Flow stratejisi tercih edildi. Artık develop isimli branch tabanlı olarak açılan Feature setleri üzerinde yapılan geliştirmelerin test sonuçlarından gelen bilgilere göre tekrar develop hattına, oradan master’a merge edilmesi söz konusu. Hatta kod yamaları için de Git Flow stratejisi tercih ediliyor. Üretim ortamında aldığımız bir hata için rutin geçiş gününü beklememiz şart değil. Acil geçiş mi? Hiç sorun değil!

![last_3.png](/assets/images/2019/last_3.png)

git extension tarafından bir görüntü

Her feature, Scrum Board üzerinde açılmış bir User Story veya Work Item ile ilişkili. Derlenmesi neredeyse bir saati bulan (şimdilik) ERP her öğle vakti teste, iki haftada bir her pazartesi dondurularak (freeze) pre-prod ortamına ve o haftanın çarşamba gecesi üretim ortamına çıkıyor (Şu vakitler haftada bir üretim ortamına çıkılması da gündemde) Benim için heyecan uyandıran tüm bu otomatikleştirilmiş etkileşim Azure DevOps üzerinden yönetiliyor. Hedef bu periyotların dışına çıkıp istenildiği zaman üretim ortamına özellik eklenebilmesi. Lakin bunun için biraz daha yolumuz var.

![last_4.png](/assets/images/2019/last_4.png)

Araya renk katacak bir ekran görüntüsü de koyalım. Fotoğrafta buradaki belki de en renkli takımlardan olan [lTunes klanı](https://medium.com/ltunes)nın board’u yer alıyor. Bu renkli oluşumda onları motive eden yöneticileri [Ismail KIRTILLI](https://medium.com/u/89dabd262e97) nın rolü çok büyük (Şekerpınar’daki bu board hiç bozulmadan Maslak’taki yeni ofise de taşındı)

Her ne kadar süreç otomatik hale getirilmiş olsa da üretim geçişleri çoğunlukla CAB (change advisory board) süreci üzerinden ilerletiliyor. Acil geçiş ihtiyaçları mutlaka direktör onayından geçiriliyor. Geçişin etkisi, kritikliği ve geri alma planları sürece dahil edilmeye çalışılıp sorun olduğunda bir önceki doğrulanmış versiyona dönebilmek için ne gerekirse yapılıyor.

Tüm bu çetrefilli işlerin yanında şirketin kültür dönüşümünün sancıları da olmadı değil. Bizi en çok zor anlayan her zaman olduğu gibi müşteri tarafıydı. ERP, onların göbekten bağlı olduğu bir sistemdi ve şimdi isteklerini sprint’ler başlamadan önce ürün sahipliği rolünü üstlenerek ilk kez temas ettikleri yazılım ekipleri ile birlikte belirlemeye çalışmak başlarda onlara da zor gelmişti. Binalar arası bir otoban olmasıydı belki de bizi birbirimizden uzaklaştıran ama sonunda alıştılar…

> SAFE
> (Scaled Agile Framework)

### Yeni Nesil Ürünler ve Gelecek Vizyonu

İlk başladığım zamanlarda bu yeni nesil ürünlere biraz temkinli yaklaştığımı ifade etmek isterim. Henüz deneysel sürümlerde olan platformların ürünleştirilip müşteriye sunulmasına her zaman karşıyım. Deneyimlemek daima tavsiye ettiğim heyecanlı bir çalışma ama müşterinin bakış açısı çok daha farklı oluyor. Onlar sorunsuz ürünlere yeni isteklerini ekletmek istiyorlar.

> Technology Radar

![last_5.png](/assets/images/2019/last_5.png)

Yeni nesil ürünler çok daha şanslı. Microsoft’un açık kaynak ve platform bağımsız.Net Core tabanlı Web API servisleri ile konuşan Vue/Angular tabanlı önyüz sistemlerinden oluşan bu çözümlerde deneyselliğe de izin veriliyor.

Javascript yerine daha çok Typescript tercih ediliyor, [webpack](https://webpack.js.org/) gibi modern paketleyiciler, Node.js gibi sunucular kullanılıyor. Tamamen Dockerize edilerek ölçeklenebilirliği kolaylaşan uygulamalar, Azure üzerindeki CI/CD (Continuous Integration/Delivery-Deployment) hattında Test standartlarına uyulmaya çalışılarak yaşıyor. Yaygınlaştırılmaya başlanan Selenium entegrasyonları ile fonksiyonelliklerin davranış odaklı test senaryoları üzerinden kontrolü yapılıyor. Günün herhangi bir anında kolayca sürüm çıkılması ve container çoklaması ile performans iyileştirilmesi mümkün.

[KONG](Peki ya Kong Kim-.md) arkasında yönetilen bu ürünler çoğunlukla kendi veritabanları ile çalışırken pek çoğunda PostgreSQL gibi farklı alternatifler de değerlendiriliyor (Hatta Shelby’nin veritabanının domain bazlı olarak parçalanmasından önce PostgreSQL’e geçişi ile ilgili deneysel çalışmalar yapılmakta. Üzgünüm Microsoft ama topyekün bakıldığında lisanslama maliyetlerin çok pahalıya gelebiliyor) Yeni nesil ürünler kendi Bounded Context’leri içinde konumlandırıldıklarından mikro servis yapısına daha uygunlar. Henüz mezun olmuş ya da birkaç yıllık iş tecrübesi bulunan geliştiricilerin aşina olduğu ve kolayca adapte olabileceği ortamlar.

### Her Şeyin Farkında Olmalıyız (İzleme, Erken Tedbir ve Otomatikleştirme)

İster onyedi yaşındaki ERP ürünü olsun ister yeni doğmuş, emekleyen, yürümeye henüz başlamış yeni nesil ürünler, izleme (monitoring) her şeydir.

Her ne kadar erken uyarı sistemlerini tam olarak robotlaştıramasak da çalışma zamanında oluşan sıkışmaları görmek, hataları koda girmeden yakalayabilmek adına çeşitli araçlardan yararlanıyoruz. Shelby’nin dünyasını genellikle Riverbed üzerinden izliyoruz (Open APM ve HP Diagnostics kullanan ekipler de var) Yavaşlayan bir ekran varsa, ön yüz sayfa taleplerinden Data Access Layer metotlarına ve arka plandaki SQL çağrılarına giderek sorunu tespit etmemiz ve çözmemiz kolaylaşıyor. Yavaşlamaya başlayanlar genellikle yoğun iş kuralı içeren stored procedure’ler veya indeksleri bozulan tablolar oluyor (Veritabanı ekibiyle yapılan koordineli çalışma ile sorunlar bertaraf edilebiliyor ama kalıcı çözümler için bazı bazı kaynak sıkıntısı da yaşanıyor)

![last_6.png](/assets/images/2019/last_6.png)

RiverBed ürününden bir ekran görüntüsü. Temsili.

Vakit ve belki de yeterli kaynak olsa bu ve benzer sorunları problem olarak sınıflayıp [ITIL](../2018/ITIL'ın Farkına Vardım.md) (Information Technology Infrastructure Library) felsefesinde geçen ilkelere göre kalıcı olarak çözmek çok da güzel olur.

Yeni nesil ürünlerin Elasticsearch tarafına atılan sayısız log verisini Kibana üstünden takip ediyoruz. Şu meşhur ELK üçlemesini ele aldığımızı ifade edebilirim. Bu log gerçekten çok büyük önem arz ediyor. Üretim ortamına ait sıkıntılı durumlarda kayıtlardaki izlere bakarak yazılıma müdahale bile etmeden çözümler üretebiliyoruz.

![last_7.png](/assets/images/2019/last_7.png)

ElasticSearch içeriği daha güzel bir şekilde izlediğimiz Kibana'dan bir görüntü

Yeni şeyler ekledikçe ürünlerin kalitesini korumamız da lazım. Bunun için [SonarQube](https://www.sonarqube.org/)’ye, güvenlik noktasındaki açıkların tespiti için [Fortify](https://www.microfocus.com/en-us/products/static-code-analysis-sast/overview)’a başvuruyor, [OWASP](https://www.owasp.org/index.php/Main_Page) (Open Web Application Security Project) gibi standartlara bağlı kalmaya çalışıyoruz.

Yeni nesil ürünlerdeki en yakın dostumuz ise Chrome’un F12 tuşu. Ön yüzün arka taraftaki REST servislerine hangi tip çağrı ve payload bilgisi ile eriştiğini görmek, problem yaşanan durumlar için ilgili senaryoları local geliştirme ortamında kolayca tatbik edip çözüm bulmamızı kolaylaştırıyor. Senaryoları gerçekleştirirken ağırlıklı olarak Postman ve SOAP UI gibi araçlardan yararlanıyoruz.

Bazı işleri otomatik hale getirebiliyoruz. Kullanıcıların sıklıkla tekrarladığı aynı şeyleri RPA (Robotic Process Automation) süreçleri ile kontrol altına alıyoruz. Bu da şirket bünyesinde yaygınlaştırılmaya çalışan bir alan. Sonuçta önemli bir zaman ve maliyet kazancı olacağı ortada.

### Vizyon

Kurumsal boyuttaki uygulamaların yenilenmek üzere tekrardan masaya yatırılması sıklıkla gündeme gelir. Sadece yeni isteklerin karşılanması değil baş ağrılarının giderilmesi ve modern çağa uymak için bu gereklidir. Bu nedenle somut ve cesaret isteyen adımlar atılmalıdır. Bu kararlılığı bir veya iki kişinin ya da bir takımın göstermesi yetmez. Konunun müşteri ile tartışılabilecek ve sayısal veriler ile desteklenebilecek şekilde en üst yönetim kadrosu tarafından da benimsenmiş olması beklenir. İnsiyatif alma isteği uyandıracak iç motivasyon unsurları önemlidir. Ancak hepsi bir yana bir teknoloji vizyonunun olması şarttır. Hangi durumdayız, nereye gitmek istiyoruz, bu yolda ilerlemek için hangi teknolojileri mercek altına almalıyız, kısa vadede hangi kararları uygulamalı uzun vadeye ne şekilde yaymalıyız…

Geçtiğimiz yaz ayında yapılan bir toplantıda Shelby’nin modernizasyonu kapsamında aşağıdaki maddelerdekine benzer işlerin planlandığını ifade edebilirim.

- Web Site’ların ilk etapta Web Application’a çevrilmesi ve ilerleyen dönemlerde Blazor çatısına taşınması (Sıcak bir noktada)
- En kısa sürede DevOps ekibinin hazırız dediği Blue/Green dağıtım stratejisine geçilmesi.
- MSSQL veritabanının PostgreSQL dönüşümüne başlanması. Kısa vadede yoğun iş kuralları içeren SP’lerin EF tarafında karşılıklarının oluşturulma maliyetlerinin çıkartılması.
- Orta vadede bir ORM (Object Relational Mapping) kullanımına geçilmesi.
- Teknik borç hedeflerinin yerine getirilmesi hususunda devam eden Fortify, Sonarqube, Testinium çalışmalarının tamamlanması (Testinium üzerinde modül bazlı olarak given-when-then senaryoları yazılmaya başlandı)
- Platformda.Net Framework 4.8 versiyonuna geçilmesi ve kod içerisinde C# 8.0 destekli yeni kalite standartlarının adaptasyonu (Yazıyı tamamladığım tarih itibariyle bitmişti)
- N-Tier yapının 3-Tier formuna indirgenmesi.
- Performans ölçümlemede OpenAPM ürününe geçilmesi için gerekli çalışmalarının yapılması (ki geçildi)
- Servis bağımlılıkları sebebiyle duran WS-* transaction yapılarının terk edilerek farklı bir stratejinin kullanılmasına başlanması.

Buradaki bazı kararları değil almak düşünmenin bile çok zor olduğu kurumsal dünyalar olduğu aşikar.

### Sonuç

Şüphesiz ki pek çok büyük oyuncunun dönüşmeye çalıştığı/dönüştürdüğü platformlar ile haşır neşir olduğumuz bir zaman dilimindeyiz. Yirmi yıl önceki dönüşümler tekrarlanıyor ve ileride de tekrarlanacak. Hatta daha kısa periyotlarla değişime adapte kalmamız gerekecek. Eğer şirketiniz aşağıdakileri yapıyorsa ciddi anlamda değişimi düşünmek gerekebilir.

- Programa eklenen ve müşteri testi yapılmış yeni bir özellik için üretime çıkış gününü bekliyoruz.
- Geliştirdiğimiz ve üretime aldığımız özelliklerin müşteri için oluşturduğu katma değeri bilmiyor/ölçümleyemiyoruz.
- Az zamanımız olduğu için bazı testleri atlamak zorunda kalıyoruz.
- Üretim ortamında oluşan bir sorunu çözmek için kullandığımız tekniğe onu düzeltmek için tekrar dönme fırsatı bulamıyoruz.
- Temel kod metriklerini aşan devasa sınıflar ve veritabanı nesneleri üzerinde hata ayıklama işlemleri yapıyoruz.
- Sahip olduğumuz ürünün ne kadar büyük bir teknik borcu olduğunu bilmiyoruz.

Bu dünyada birbirinden farklı eski-yeni bir çok teknolojiyi bir arada görüyoruz. Yeni nesil araçlar sayesinde önceden önlemlerimizi almak, teknik borçlanma gibi konuları bertaraf etmek, her noktasını test ettiğimizden emin olduğumuz ürünleri sürümlemek artık çok daha kolay. Hazırlıklarımızı buna göre yapmalı ve kendimizi sürekli geliştirmeliyiz.

Yazıda bahsettiğim ne kadar şey varsa sadece bulunduğum ERP uygulaması ve çevresindeki gelişmelerden ibaret. Robotik süreç otomasyonu (RPA-Robotic Process Automation), yapay zeka, makine öğrenimi, IoT (Eşyanın Interneti), finansman, sigorta, filo, raporlama vb işlerle uğraşan daha pek çok ekip var ve hepsi bu büyük ekosistemin bir parçası olarak gelişimini sürdürüyor.

### Bahsedilen Terimler

Yazı boyunca aşağıdaki listede yer alan terimlerden bahsettim. Eminim ki bir çoğuna aşinasınız ama çoğunu da bilmiyorsunuz. Bildiklerinizi de iyi bilip bilmediğiniz konusunda tereddütleriniz var. Bu yüzden bilmedikleriniz dahil var olanları da araştırıp pekiştirmenizi, üzerlerine eklenecek yeni daha neler olduğunu sürekli araştırmaya devam ederek canlı kalmanızı tavsiye ederim. Benim sekiz saatlik sonsuz döngüm neredeyse her gün bu şekilde işliyor. Zamansal paradox yaratarak oturduğum yerde kara delik açan bu cümle ile yazıma son veriyorum. Hepinize mutlu günler dilerim.

#agile #scrum #sprint #storyPoint #safe #workItem #dotNet #dotNetCore #nodeJs #typescript #vue #angular #erp #monolithic #legacySystem #nTier #SOA #SOAP #REST #WCF #sonarQube #codeCoverage #technichalDepth #qualityAssurance #mssqlServer #postgreSql #gitFlow #vsts #tfs #azureDevOps #branch #fortify #owasp #roslyn #CI/CD #continuousIntegration #continuousDelivery #continuousDeployment #dockerize #container #microService #kong #riverBed #ITIL #CAB #antiPattern #godObject #boundedContext #dataAccessLayer #chromeDevTools #thoughtWorks #microService #hypeTech #techRadar #SSIS #IoT #yapayZeka #robotik #postman #soapUI #rpa