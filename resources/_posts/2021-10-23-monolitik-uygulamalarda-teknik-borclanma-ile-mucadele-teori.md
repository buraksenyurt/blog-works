---
layout: post
title: "Monolitik Uygulamalarda Teknik Borçlanma ile Mücadele (Teori)"
date: 2021-10-23 07:00:00 +0300
categories:
  - deneyimler
tags:
  - tecnichal-debt
  - monolitik-mimari
  - yazılım-mimarileri
  - modernizasyon
  - sonarqube
  - statik-kod-analizi
  - dijital-dönüşüm
  - çevik-olmak
  - yazılım-geliştirme-yaşamdöngüsü
  - strateji
---
İş hayatına adım attığımda tarihler 1999 yılını gösteriyordu. Bilgi İşlem Sorumlusu unvanı ile yarı zamanlı başladığım şirkette bir çağrı merkezi uygulamasının geliştirilmesinden, bilgisayarların kurulumlarından ve kullanıcı destek işlemlerinden sorumluydum. O zamanlar sahip olduğum bilgiler çok kıt ve tamamen programlama üzerindeydi. Ne katmanlı mimarilerden ne de tasarım kalıplarından bihaberdim. Hal böyle olunca yazdığım uygulama buton arkası kodlamanın ötesine geçemiyordu. Üstelik web tabanlı değil Windows tabanlı bir programdı ve dağıtımı çağrı merkezi bilgisayarlarına kopyala yapıştır usulüne göre yapılıyordu (Neyse ki şirkette sadece on iki çağrı personeli vardı) ancak bir sonraki işimde dengeler tamamen değişti. Bu sefer yazılım dünyasının milenyum başındaki yükselen yıldızlarından olan.Net platformu üstünde yeni yetme bir yazılımcı olarak işe başlamıştım. Bana Junior Software Developer unvanı vermişlerdi ve bu kez web tabanlı bir uygulama ile bol miktarda katman söz konusuydu. Tipik olarak katmanlı mimariye göre geliştirilmiş ve müşterisi olan bir ürün üstünde çalışan birkaç yazılımcıdan birisiydim.

Aradan yıllar geçti ve ben yazıyı yazdığım tarih itibariyle yedinci şirkette olduğumu fark ettim. Halen daha görev aldığım Doğuş Teknoloji’de dördüncü yılımı doldurmak üzereyim. İlginç olan şu ki hem burası hem de öncesinde altı yıla yakın çalıştığım ING Bank, ciddi anlamda dönüşüm geçiren iki kurum. Her ikisi de var olan legacy sistemlerini yenilemek veya baştan yazmak için hem kültürel hem de teknolojik altyapı değişimi geçirdiler, geçiriyorlar. Bende geliştirici olarak bu dönüşüm çalışmalarında ipin bir ucundan da olsa tutma fırsatı buluyorum. Şüphesiz ki değişim yazılım dünyasının olmazsa olmaz bir parçası. Diğer yandan son on yılda içinde yer aldığım işlerden gözlemlediğim kadarıyla monolitik yaklaşıma uygun olarak geliştirilmiş katmanlı sistemlerde var olanı yeniden yazmak da modernize etmek de hiç kolay değil. Çok planlı olunması ve iyi bir strateji ile hareket edilmesi gerektiği ortada. Her şeyi en ince ayrıntısına kadar düşünmek gerekiyor. Seçilecek mimari, bu konuda çalışacak insan gücünün sahip olması gereken yetkinlikler, revize edilecek süreçler, yük olan lisanslamalar ve alternatifleri, kullanılacak açık kaynak ürünler, nelerin SaaS (Software as a Services) haline geleceği vb.

Tüm bu gelişmelere ek olarak bir süredir şirketin iç girişim programına dahil edilen bir ürün fikri için kıymetli bir meslektaşıma destek olmaya çalışıyorum. Özellikle fizibilite safhasında yirmiden fazla firmanın önemli pozisyonlardaki çalışanları ile karşılıklı görüşme fırsatımız oldu. Konumuz ürünle alakalı olsa da benim dikkatimi çeken nokta özellikle beş yaş üstü şirketlerin neredeyse tamamında birçok uygulamanın yenilenmesinin söz konusu oluşu. İster kırk yıllık mainframe sistemler etrafında koşan uygulamalar olsun ister iki yaşında bir ürün mutlak suretle bir değişimden (yenilemeden, modernizasyondan…Artık nasıl düşünürseniz) bahsediliyor. Son on yıldır görev aldığım iki kurumun uygulama modernizasyonlarına tanıklık etmiş birisi olarak sektörü dinlediğimde ortaya çıkan sonuçların örtüşmesi dikkatimi monolitik sistemin katmanlı uygulamalarına çekti. Onlardan her yerde bolca bulmak mümkün.

Yazılımcı olmanın kaçınılmaz bir gerçeği üretim ortamından gelen problemler ile uğraşmaktır belki de. Çalışmakta olduğumuz sistemlerin giderek büyümesi, iş kurallarının zamanla karmaşıklaşması, nasıl yapılır nereye bakılır bilgisinin ayrılan iş gücü nedeniyle eksilmesi, entegrasyon noktalarının çoğalması ve daha birçok sebepten ötürü bu kaçınılmazdır. Her birimiz yazılım yaşam süresi boyunca farklı tipte mimariler üzerinde çalışırız. Örneğin 2021 yılının ilk çeyreğinde hazırladığım ve yedi yüzden fazla [kişinin katıldığı “Teknik Borç Farkındalık Anketi” isimli çalışmanın sonuçlarına göre](https://docs.google.com/forms/d/1O_EwxGI22cADNVa2PIW5yOxGKHRECGvIp7JRWSMLw4w/edit#responses) beşimizden dördünün katmanlı mimari olarak adlandırdığımız monolitik sistemlerde görev aldığını söyleyebiliriz. Hele ki sektörde yirmi yılı deviren bir yazılım geliştirici iseniz (ki yine anket sonuçlarına göre neredeyse %40ımız 10 yaşından büyük ürünlerle çalışmış) böyle bir sistemle yolunuzun kesişmemiş olması pek mümkün değildir.

![techdebt_01.png](/assets/images/2021/techdebt_01.png)

Sonuçların bilimselliği bir kenara dursun katmanlı sistemlerin en büyük dertlerinden birisi kolayca modernize edilemeyişleridir. Bunun en büyük sebeplerinden birisi de kontrolsüz büyümenin ve birçok [Anti Pattern](https://www.buraksenyurt.com/post/AntiPatterns-Ders-Notlarc4b1m) ihlalinin teknik borç yükünü artırmış olmasıdır. Ward Cunningham’ın bu terimi programcı olmayan insanlara neden kodu refactor etmeleri gerektiğini anlatmak için metafor olarak kullanmasının üstünden çok süre geçmiş olsa da pek çok sistemin baş etmek zorunda kaldığı bir gerçektir teknik borç. Bu baş belası için bazı kaynaklarda ürkütücü The Silent Company Killer terimi kullanılır ve bence bu son derece isabetli bir ifade.

> Neden legacy kabul edilen bir sistemi modernize etmeye çalışıyoruz da onu sıfırdan yazmıyoruz sorusu aklınıza gelebilir. Ancak ölçekçe büyük, müşteri nezdinde fonksiyonel olarak yüksek memnuniyet sağlayan sistemlerde veya mainframe gibi kolayca kopartılamayacak bağımlılığı bulunan yapılarda Big Bang olarak da ifade edilen sistemi komple değiştirmek kurumun hareket etme kabiliyetini olumsuz yönde etkileyebilir ve hatta bir noktada işleyen operasyonun yürütülmesini engelleyebilir.
> Bir üst seviyeye geçmeden önce (örneğin yeni bir mimari modele) domain olarak var olan süreçleri hem veri hem işleyiş bazında ayrıştırmak daha iyi bir yaklaşım olacaktır. Nitekim büyük resmi görmek ve sonrasında detaylara inebilmek böylesine büyük yapılarda çok zordur. Bu da bir stratejidir ve bunu gerçekleştirme noktasında var olan yüklerden kurtulmak yani teknik borcu hafifletmek önemlidir. Hatta var olanın yenisini paralelde yazmayı seçtiğimiz durumda da sistemi anlamak ve iyileştirmek için önemlidir.

Teknik borç yazılım tarafındakiler için anlamlı bir terim (bazen) olmasına karşın bazen IT personeli ve daha da önemlisi iş birimi ile paydaşlar için pek önemli bir kavrammış gibi görünmez. Sonuçta ölçülebilir değerler sunmadığımız ve bunların maliyet tablosundaki etkilerini göstermediğimiz sürece paydaşlar için anlamlı olmaz. Oysaki Gartner, McKinsey, Price Waterhouse Coopers, CAST ve CISQ (Consortium for Information & Software Quality) gibi kurumların zaman içerisinde yaptığı çeşitli çalışma sonuçları ve raporlar durumun ne kadar ciddi olduğunu gözler önüne sermektedir. İşte konu ile ilgili dikkat çekici bazı istatistikler;

- CISQ’ in 2020 yılı bazlı [The Cost of Poor Software Quality in the US](https://www.it-cisq.org/cisq-files/pdf/CPSQ-2020-report.pdf) raporuna göre Birleşik Devletler’ de ciddi sorunlara yol açan teknik borç düzeltme maliyeti 1.13 Trilyon dolar civarındadır.
- Gartner’ a göre ciddi sorunlara yol açabilecek teknik borç düzeltme maliyeti sadece 2020 için 2.84 Trilyon dolar civarındadır ki aynı danışmanlık firmasının 2011’de yayınladığı raporda 2015 için bu borcun 1 Trilyon dolar civarında olacağı ön görülmüştür.
- NYSE borsasında tahtası olan ve devlet tarafından belirlenen bir regülasyonu hatalı uygulayan bir firma, uygulamanın eksik dağıtımı sebebiyle 45 dakika içinde 462 milyon dolar zarar etmiştir.
- McKinsey’ nin değeri 1 milyar doların üstünde olan şirketlerin CIO’ ları ile yapığı çalışma sonuçlarına göre CIO’ ların %60’ı son üç yıllık dönemde teknik borçların gözle görülür şekilde arttığını belirtmektedir.
- Yine aynı rapora göre yeni ürünlere ayrılan bütçenin %10 ile %20 kadarı teknik borçların giderilmesi için harcanmaktadır. Üstelik teknik borçların tüm teknoloji mülkiyeti içerisindeki payının %20 ile %40 arasında değiştiği belirtilmiştir.
- [CAST](https://www.castsoftware.com/research-labs/technical-debt-estimation)’in 160 farklı organizasyondan 550 milyon satır koda sahip 1400 uygulamayı inceleyen raporuna göre kod satırı başına ortalama teknik borç maliyeti tahmini 3.61 dolar seviyesindedir.
- Tricentis Software Fail Watch 2017 yılında 606 ölümcül yazılım hatası raporlamış ve bunların 304 firmaya 1.7 trilyon dolar civarında zarar yol açtığı sonucuna varmıştır.

Bazı rakamlar ütopik değerler gibi görünse de çalıştığım şirketlerdeki gözlemlerim bu bulguların oldukça gerçekçi olduğu hissiyatını uyandırmakta. Diğer yandan dünya çapında bilinen kod tabanının evrenimiz gibi genişlediği de söylenebilir. Yine CISQ raporlarına göre 2005’te yıllık ortalama 35 milyar satır kod üretildiği öne sürülmüştür. Bu değer 2020'de ortalama 100 milyar satır kod olarak revize edilmiştir. Rapordaki tahmin projeksiyonuna göre 2020 yılında dünya üstünde yaklaşık 1.655 trilyon satır kod olduğu öngörülmüştür. Yakın zamanda karşılaştığım Tanrı Parçacığından bozma kod dosyasındaki bir sınıfın 600 kitap sayfasına eş değer satırı olduğunu hesaplayınca bu rakamlarda gerçeklik payı olduğunu ve kayıt dışı olanlarla birlikte çok daha büyük bir havuzda yaşadığımızı söyleyebilirim.

Her ne kadar belirtilen rakamlar oldukça karamsar bir tablo çizse de teknik borcu iyi yöneten firmaların çeşitli kazanımlar elde ettiği de ortadadır. Sadece teknik borç ödeme yönetiminin bilinçli olarak kabulü ve buna göre hareket edilmesi bile fark yaratır. İsmini vermek istemeyen bulut tabanlı bir bilişim firmasının CIO’ suna göre teknik borçla mücadele yöntemlerinde yapılan değişiklik sonrası yazılımcıların bu işler için normal mesailerinden ayırdıkları zaman %75’ten %25’e kadar düşmüştür — McKinsey. Hatta aynı rapora göre teknik borcu aktif olarak yönetebilen firmalarda mühendisler zamanlarının %50 kadar fazlasını olması gerektiği gibi iş hedeflerine harcayabilmektedir.

Teknik borçlanma yazılımın kalitesine de doğrudan etki eder. Hangi metrik olursa olsun belli standartların üzerinde koşan kaliteli ürünlerde bile kusurlara rastlanabilir. Bu kusurlara çevresel faktörler de eklenince ortaya korkutucu sonuçlar çıkar. 2019–2020 aralığı büyük yazılım hatalarının tarihe geçtiği yıllar olarak hafızlara kazınmıştır. Fidye programları, siber saldırılar, beklenmedik IT kesintileri ve veri sızıntıları gibi nedenler havacılıktan bankacılığa, devlet kurumlarından savunma sanayine kadar birçok sektörde zarara sebep olmuştur. Örneğin NASA’nın Boeing Startliner mekiği iki büyük yazılım sorunu yüzünden insansız görevi sırasında Uluslararası Uzay İstasyonuna kenetlenememiş ve kargosunu bırakamadığı gibi dünyaya geri dönmek zorunda kalmıştır. Bu hatalar sebebiyle yeni bir uçuş planlanmış ve bunun maliyeti dört yüz milyon doları geçmiştir. İngilizce ders kitaplarına da konu olan Britanya’nın ünlü Heathrow havalimanı, Check-In sistemindeki kusurlar yüzünden aynı gün içinde yüzden fazla uçuş planının bozulmasına şahit olmuş, yaşanan sorun ancak sonraki gün düzeltilebilmiştir. Yoğun geçen bir yaz dönemi sonrasında belki de yorgun düşen yazılım sisteminin çökmesi sonrasında British Airways’in yüzden fazla uçuşu iptal edilmiş, üç yüzden fazlası da gecikmeli olarak sefer yapmıştır.

Şirketler bu ve benzeri olaylar neticesinde sadece para değil itibar da kaybederler. Ne yazık ki hepsinden kötüsü yazılım sorunlarının ölümcül sonuçlara sebep verdiği gerçeğidir. Dünyanın ünlü şirketlerinden birisinin göz bebeği olup Titanik misali övülen hava taşıtı yazılımındaki hata sebebiyle başka bir uçakla çarpışmış ve bu hazin olay sonucu 346 insan yaşamını yitirmiştir. Sonuç olarak program sonlandırılmış, şirket sonradan yine kazansa da hatırı sayılır derece hisse değeri kaybetmiştir. Peki ölenler geri gelebilmiş midir?

### Tanım

Raporlar ve rakamlar bu işin ciddiyetini ortaya koymaktadır ancak her şeyden önce bir tanım yapılması gerekir. Üstelik bu tanım bilişim personeli, iş sahipleri ve tüm paydaşlarca anlaşılır olmalıdır. Nitekim farkında olunması gereken bir mevzu söz konusudur. Big Commerce firmasından Shawn McCormick’e göre bir proje olgunlaştıkça çevikliği azaltan her kod teknik borçtur ve gerçek teknik borç tesadüfi değil kasıtlı olarak ortaya çıkmaktadır. Forbes’tan Brad Sousa’ya göre bir işletmenin doğru çözüm için gereken zaman ve parayı harcamak yerine mevcut kodu yeni kodlarla yükseltmeyi seçtiğinde maruz kaldığı gerçek maliyet teknik borcun ta kendisidir. Git Connected’ tan Trey Huffine’ e göre teknik borç, hızlı kazançlar elde etmek için yazılıma eklenen ve ileride düzeltilmesi için daha fazla çaba gerektiren herhangi bir koddur. Hackernoon’ dan fpgaminer kod adlı kişi ise onu şöyle yorumlamıştır; hedefe daha hızlı ulaşmak için koda kestirme yollar eklediğinizde bugün normalden daha fazlasını yapabilirsiniz ama sonra daha yüksek bir maliyet ödersiniz. Konu ile ilgili daha akademik bir tanım da mevcuttur. ScienceDirect teknik borcu şöyle ifade eder; Teknik borç kasti veya değil müşteri isteklerine veya uygulama kısıtlarına (deadline gibi) öncelik veren yazılım geliştirme eylemlerinin bir sonucudur.

### Teknik Borç Türleri

Teknik borcun bu bahsi geçen türlü tanımları onun farklı kategorilerde ele alınmasını da gerektirmiş ve buna göre bazı farklar ortaya konmuştur.

![techdebt_02.png](/assets/images/2021/techdebt_02.png)

Örneğin Steve McConnel 2007 yılında teknik borçları kasıtlı olarak yapılan ve yapılmayanlar şeklinde iki ana kategoriye ayrıştırmıştır. Gerçekten de bazı durumlarda oluşacak teknik borçlanma stratejik bir karar sebebiyle sineye çekilebilir. Buna karşın ilerde borcun ödenmesi gerekir. Software Engineering Institute tarafından 2014 yılında yapılan bir çalışmaya göre teknik borç birçok kategoride değerlendirilir. Mimari yaklaşımlardaki anomaliler de teknik borca sebebiyet verebilir, personelin sahip olduğu yeteneklerdeki eksiklik de teknik borca sebebiyet verebilir gibi. Bana göre bu sınıflandırmalar teknik borcun domain bazlı ayrılabilmesine ve buna göre ekiplerin kontrolüne verilmesine de olanak sağlamaktadır.

Çok doğal olarak hepimizin seveceği ve dikkat kesileceği çeşitlendirme Martin Fowler tarafından yapılmıştır. [Martin’in Quadrant’ına göre](https://martinfowler.com/bliki/TechnicalDebtQuadrant.html) durum öncelikle kasıtlı veya kazara mı meydana geliyor ona bakılmalıdır. Buna göre umursamaz ve pervasız bir şekilde hareket etmekle tedbirli ve ihtiyatlı bir şekilde hareket etmek arasında farklılıklar oluşacaktır. “Tasarım için zamanımız yoktu” sözü genellikle kasten ve umursamaz olduğumuz bir durumu ifade etmek için idealdir. Bazen bilerek teknik borca girmemiz kaçınılmaz olur. Hemen çıkmamız gereken bir özellik için bu sonucun oluşacağının bilincindeysek durumu tedbirli bir borçlanma gibi yorumlayabilir ve riski göze alabiliriz. Tabii alınan riskin etkin bir şekilde yönetilmesi de gerekir.

### Sebepler

Neden teknik borç oluşur sorusunun farklı cevapları vardır. Örneğin McKinsey bunları dört ana başlık altında toplamış ve aşağıdaki gibi kategorize etmiştir.

Stratejik Sorunlar

- IT girişimlerinin şirket stratejisi üzerindeki etkilerini ölçememek.
- Portföy yönetimindeki sorunlar sebebiyle senkronize olmayan kaynak tahsisi ve toplam maliyet tahmini yapılamaması yüzünden finansman ile yaşanan uyum sorunları.
- Birleşme ve satın almalardan sonra teknoloji entegrasyonunun yetersiz kalması — Birkaç şirketi satın alan bir firmanın kendi ürünleri ile gelen eski ürünler arasındaki iletişimde yaşayacağı her türlü teknik, kültürel zorluk olarak düşünebiliriz.
- Ürünlerdeki aşırı karmaşıklık.

Süreçsel Sorunlar

- Proje Backlog maddelerine doğru öncelik verilememesi veya bu maddeler listesinin etkin bir şekilde kullanılmaması.
- Geliştirme ve bakım sürecinin zayıf yönetimi.
- Nadiren yapılan kod kalite ölçümleri.
- Zayıf olağanüstü durum (Disaster Recovery diyelim) yöntemlerinin kullanılması sebebiyle IT operasyonlarındaki düzensizlik ve tutarsızlık.

Yetenek Bazlı Sorunlar

- Ürünlerin kullanıcılara teslimini geciktiren, kaynak riski oluşturan beceri eksiklikleri.
- Ekip kapasitesinin nadiren teknik borcu azaltmak için tahsis edilmesi.
- Karar vermede teknoloji borcunun göz ardı edilmesi.
- Ekiplerin sadece kısa vadede özellik (feature) sağlamaya odaklanması.

Mimari Bazlı Sorunlar

- Uygulama sunucuları, veri tabanları, alt yapı platformlarındaki güncelleme hataları.
- Legacy sistemlerden kalan düzeltilmemiş sorunlar.
- Yeniden kullanılabilirliği sınırlayan zayıf arabirime sahip monolitik bloklar.
- Özelleştirilmiş paketlere sahip esnek olmayan yazılımlar.
- PoC (Proof of Concept) formatında başlanıp ürün haline getirilmiş yazılımlar ki en sevdiğimiz günahlardan birisidir.
- Yazılım içerisine sıkıştırılmış, değiştirilmesi zor yerleşik iş kuralları. Hatta bazen yazılım kodundan çıkıp veri tabanı içinde hayat bulan iş kuralları.
- Tutarlı bir veri modeli üzerinde anlaşamama ve düşük veri kalitesi.
- Standard sistem entegrasyonu yaklaşımlarının doğru kullanılmaması sebebiyle çoğalan uygulamadan uygulamaya entegrasyon noktaları (Point-to-Point entegrasyon noktalarının çoğalması olarak düşünülebilir ki bazıları zamanla atıl hale gelip unutulur ve hayalet bağımlılıklar oluşur)

### Teknik Borç Nasıl Yönetilir?

Büyük resme bakıldığında teknik borç sadece kodun kötü parçalarından ayıklanması ya da iyileştirilmesiyle alakalı değildir. Şirketin genelini ilgilendiren bir sorundur ve herkes tarafından kabul edilmelidir. Ayrıca teknik borcun rakamsal olarak ifadesi ve denk düştüğü eksi maliyetler şeffaf bir şekilde sunulmalıdır. Teknik borç, yönetim kademesinden yazılımcısına kadar herkesin bilinçli şekilde baş etmesi gereken bir sorundur. Buna göre bazı çözüm yöntemleri önerilir. Yine sıkılıkla referans olarak kullandığım McKinsey raporlarına göre bu mücadele aşağıdaki sıralama ile yapılabilir.

1. Ortak Tanım Yap: İş birimleri ve bilişim personeli liderleri teknik borcun ne anlama geldiğinin ortak tanımını yapmalıdır.

2. İşle İlgili Bir Sorun Olduğunu Kabul Et: Teknik borcun sadece teknolojik değil aynı zamanda işle ilgili olduğunu kabul edin.

3. Şeffaf Ol: Teknik borçları rakamsal olarak açıkça ifade edin.

4. Karar Verme Sürecini Resmileştir: Belli kurallar üzerinde anlaşılmış bir portföy yönetimini takip edin.

5. Kaynak Ayır: Sadece teknik borca adanmış bir görev gücü oluşturabilirsiniz.

6. Büyük Patlamaya Dikkat Et: Mega projelerde teknik borcu tutarlı, tahmin edilebilir ve stratejik yol haritasına bağlı olarak ayırın ki rekabet etme yeteneğinizi kaybetmeyin. Yani topyekün değiştirmek yerine parçalayarak mücadele edin. Nitekim bir teknik borcu düzelteceğiz derken çalışan ve sahanın göz bebeği olan ürünü işlemez ve geliştirilemez hale de getirebilirsiniz.

7. İflas Noktalarını Belirle: Teknolojik varlıklar değerinin %50’ sini aşan borçlarda yeni bir Stack oluşturmayı düşünün. Maraton projeler için “IT Platform in a Box” şeklindeki yaklaşımları da tercih edebilirsiniz — ya da ürünler için bir raf ömrü belirleyebilirsiniz. Beş yılı geçen ürünleri yeni nesil teknolojilere dönüştürüyoruz gibi bir politika pekala belirlenebilir.

Teknik borcu yönetmek için çeşitli yöntemler olduğu aşikardır. Diğer yandan bu işin önemli bir parçası olan yazılım mühendislerinin işi pek de kolay değil. Yazılımların giderek büyümesi ve daha da karmaşıklaşması, artan ve ciddi boyutlarda zarar veren siber saldırılar, kirli bilgi dezenformasyonu sebebiyle yanlış öğrenilenler, sonu gelmeyen kullanıcı ihtiyaçları, teknoloji geliştirme hatlarının (tech stack diyelim) sürekli evrimleşmesi, gerekli teknik becerilerin çabucak eskimesi, hangi çevik metodoloji olursa olsun BT üzerinden kalkmayan zaman baskısı, işletme modellerinin değişen müşteri ihtiyaçları ve teknolojilere ayak uydurmak için devamlı değişmesi, veri güvenliği ve siber güvenlik ile ilgili regülasyonlar, yazılan her kod satırının potansiyel bir hata noktası olma ihtimali ve daha bir çok nedenden ötürü teknik borçla mücadele noktasında yazılım geliştirenleri bekleyen pek çok zorluk bulunmaktadır.

Bu arada teknik borcu yönetme noktasın teşhis koymak, acı noktalarını belirlemek, devam kararı alıp almamak için skor kartlarına başvurulabilir. Bu konu ile ilişkili olarak The Art of Service tarafından yayınlanan Technical Debt — A Complete Guide (Practical Tools for Self-Assessment) kitabını tavsiye edebilirim. Kitabın genel amacı aşağıdaki skor kartlarının çeşitli kategorideki sorulara verilen cevaplara göre puanlanarak doldurulmasıdır.

![techdebt_03.png](/assets/images/2021/techdebt_03.png)

### Mimari

Ben ve benim gibi yazılımcılar açısından bakıldığında mimari seçim ve uygulanış biçiminin teknik borçlanma üzerinde etkisi olduğu söylemek kaçınılmazdır. Yazılım mimarileri oldukça geniş ve kapsamlı bir konudur ancak çok yüksek irtifadan meseleye bakıldığında işimize yarayacak bir özet üzerinde durabiliriz.

![techdebt_04.png](/assets/images/2021/techdebt_04.png)

Yazılım mimarileri temel olarak ikiye ayrılır. Monolitik sistemler ve dağıtık olanlar. Pek çoğumuzun yakinen tanıdığı katmanlı tarz monolitik tarafta yer alırken pek çoğumuzun da çalışmak istediği hatta bazen kurtarıcı olarak gördüğü mikro servis yaklaşımı dağıtık mimari kategorisinde bulunur.

Tipik olarak katmanlı çözümler aşağıdakine benzer bir kurguya sahiptir ve pek çok kitapta bu şekilde resmedilir.

![techdebt_05.png](/assets/images/2021/techdebt_05.png)

Hatta bu yapının aktörleri zaman zaman dağıtılabilir farklı parçalara da bölünebilir. Bu daha çok ölçeklenebilirliği mümkün mertebe etkin kullanmak amacıyla yapılır. Aynen aşağıdaki şekilde görüldüğü gibi.

![techdebt_06.png](/assets/images/2021/techdebt_06.png)

Veri tabanı katmanı ve diğer kısımlar ayrı bir şekilde dağıtılabilirken uygulamanın tamamı tek parça halinde de üretim ortamına uğurlanabilir arkasından bir bardak su dökülerekten. Bizim üzerinde durduğumuz katmanlı yapı ile diğerlerini kıyasladığımızda ele alınması gereken birçok kriter de vardır. Pek çok kriter zaman içerisinde ortaya çıkan ihtiyaçlar doğrultusunda önem kazanmıştır. Netflix, Amazon, Google, Spotify ve benzeri öncülerin sürüklediği sistemler mimarilerin değişmesine neden olmakla kalmaz hata toleransından sistemin ayakta kalabilir olmasına kolay dağıtımdan esnekçe genişleyebilmeye kadar birçok faktörün de dikkate alınmasına sebep olur. Aşağıdaki tabloda söz konusu mimari yaklaşımlar arasındaki avantaj ve dezavantajları görebilirsiniz. Bir yıldız çok zayıf, beş yıldız çok güçlü anlamındadır.

![techdebt_07.png](/assets/images/2021/techdebt_07.png)

Katmanlı mimari küçük çaplı, basit uygulamalar ile web siteleri gibi çözümler için son derece idealdir. Ayrıca başlangıç bütçesi düşüktür. Bu nedenle kurumsal çapta düşünüldüğünde fikirleri hayata geçirme noktasında sıklıkla tercih edilir. Nitekim çabuk sonuç verir. Dağıtık sistemlere geçildikçe düşünülmesi, yönetilmesi gereken ayrık bileşenler çoğalır ve daha iyi bir teknik yönetim gerekir. Hata yönetimi bile dağıtık sistemler düşünüldüğünde katmanlı yapılara göre çok daha zordur. Anlamlı loglar atmanız, bunları yorumlamanız, yorumlarınıza uygun alarm sistemleri kurmanız, çakılan servisler kendine geldiğinde ne yapmalıyı planlamanız, versiyonlamaları nasıl yöneteceğinizi düşünmeniz, ağ trafiğini gözlemleyip iyileştirmeniz, kara cumalara aylar öncesinden hazır olmanız vs gerekir.

> (sonarqube, cast vb)

### Semptomlar

Monolitik yapılar iyidir hoştur ama zaman her şeyin ilacı olduğu kadar bazen de çaresi olmayan bir virüstür. Kontrolü kaybettiğimiz noktadan itibaren teknik borç sistemin tüm damarlarına sirayet etmeye başlar. Esasında bazı semptomlar teknik borcun artmış olduğunun iyi birer göstergesidir. Bunları aşağıdaki gibi özetleyebiliriz.

- Loglar sorun tespitinde yetersiz kalır ve üretim ortamında sıklıkla debug yapılır.
- Anti Pattern pratikleri çoğalır.
- Yerleşik iş kurallarını dışarıya almak sadece zor değil neredeyse imkansız hale gelmiştir.
- Test yazmak zordur ve hatta test yazılmaz.
- Yazılım tek paket olarak üretime çıkar ve bu sırada kesintiler olup diğer paydaşlar bundan etkilenir.
- Bir katmandaki hata diğerlerini de etkiler.
- Basit bir sınıf değişikliği yeniden dağıtım gerektirir.
- Mean Time to Recovery süreleri dakikalar mertebesinde artar.
- Benzer hatalar sürekli olarak karşımıza gelir.

Monolitik yapılar büyüyüp karmaşıklaştıkça dağıtık mimarilere nazaran sahip oldukları kolay anlaşılırlık, düşük inşa ve bakım maliyeti gibi avantajlarını kaybederler. Yukarıdaki semptomlar çok doğal olarak bir şeyler yapılmasını gerektirir. Bu noktada bazen ilk akla gelen monolitik mimariyi terk edip dağıtık sistemlerin göz bebeği olan mikro servis yaklaşımına geçmektir.

Büyük resme Legacy sistem terimi üstünden de bakmak gerekiyor. Günümüzde monolitik yapıları görünce genellikle Legacy sistemler olarak anıyoruz. Onlar eski çağdan kalmış, yeni neslin çalışmak istemediği, korkunç teknolojiler içeren ürünler! Değil mi? Adı her ne olursa olsun bu tip yazılımların kalitesini artırmanın ve daha da önemlisi ömrünü uzatmanın bilinen belli başlı yolları da mevcut (Çalışan sisteme dokunma derler ya. Az biraz dokununca güzel sonuçlar da çıkabiliyor aslında.)

- Tüm sistem detaylarını izole edecek şekilde API’lere ve Container’lara geçmek— Encapsulate,
- Sadece bug’lar ile uğraşıp bakım yapmak — Repair,
- Yeni donanımlara geçerek sistemi hızlandırmak — Replatform,
- İnce ayar çekmek — Rebuild, yeni bir teknolojik platforma adapte etmeye çalışmak — Rearchitect,
- Mainframe gibi parçaları bulut servis sağlayıcılarına taşımak — Rehost,
- Çözümü Software as a Service ile değiştirmek — Replace,
- Teknik borçları azaltıp olası hataların önüne geçmek — Refactor
- ve en nihayetinde üstteki maddelerin birçoğunu bir arada yapmaya çalışmak.

Fakat bu iyi bir organizasyonu ve çözüm metodolojisini gerektirir.

### TDML (Technical Debt Management Lifecycle)

Yaklaşık olarak dört yıldır çalıştığım Doğuş Teknoloji ve öncesinde görev aldığım bankada teknik borçlanma ile mücadele cephesinde savaştım, savaşıyorum. En azından üstüme düşen görevleri yapmaya çalıştığımı ifade edebilirim. Statik ve dinamik kod analiz araçları bu mücadelenin önemli birer parçasıdır ancak teknik borcun sadece kodla ilgili olmadığını düşündüğümüzde yeterli değildir. Dolayısıyla diğer konular için yönetsel seviyede destek almak bu mücadele açısından hayatidir.

Edindiğim tecrübelere göre monolitik bir sistemde teknik borçlanma ile mücadele için aşağıdaki gibi bir yaşam döngüsünün kullanılması gerekir. Buna TDML (Technical Debt Management Life Cycle) adını verebiliriz ve farklı türden sistemlere de uyarlanabilir.

![techdebt_08.png](/assets/images/2021/techdebt_08.png)

Döngüye girebilmek için ön gereksinimlerin tamamlanması gerekir. Her şeyden önce teknik borcu kabul etmek, ortak bir tanımını yapmak, yarattığı yükü hesaplayıp genel maliyetini ölçmek ve bunu şeffaf bir şekilde paylaşmak gerekir. Ön gereksinimler için aşağıdaki gibi genel bir kontrol formu kullanılabilir ve Dashboard benzeri bir arabirim ile sistem içerisinde her an görünür olması sağlanabilir. Görünürlük de bu mücadelenin olmazsa olmazlarındandır.

![techdebt_09_new.png](/assets/images/2021/techdebt_09_new.png)

Teknik borç yönetimi yaşam döngüsü belli periyotlarda tekrar eden bir düzenektir. Gereksinimlerin karşılanmasını takiben sırasıyla aşağıdaki adımlara göre işletilir.

- Keşfet: Keşif aşamasında var olan kodun çarpık yanları ortaya konur. Bir başka deyişle teknik borç keşfi yapılır. Bu amaçla çeşitli araçlardan yararlanılabilir. Statik Kod analiz araçlarından birisi olan SonarQube bunlardan birisidir — ancak tek değildir. Aşağıdaki tabloda diğer araçları ve genel özelliklerini görebilirsiniz. Buna ilaveten IT4IT anlamında yapılması düşünülen yenilikler de keşif aşamasında değerlendirilebilir. Bu, bizim de şirket bünyesinde sıklıkla uyguladığımız bir pratiktir. Örneğin Business Layer içerisindeki fonksiyonların servis olarak dışarı çıkartılması, Session kullanımından vazgeçilip Redis’e dönülmesi, konfigürasyon değerlerinin Secret Vault üstüne alınması, karmaşıklık değeri yüksek fonksiyonların (cognitive complexity) hafifletilmesi, tekrarlı kod bloklarının tekilleştirilmesi, Transaction kullanımından vaz geçilmesi, bağımlılıkların Dependency Injection mekanizmaları ile dışarıya alınması, veri tabanı tarafına yayılmış iş kurallarının otomatik araçlarla kod tarafına çekilmesi vb. Bu noktada yazılımcıları dinlemek oldukça önemlidir. Onlardan toplanan fikirlerin TDML sürecine sokulması noktasında yine bir komite desteğini almakta yarar vardır.
- Öncelik Ver: Araçlardan elde edilen bulgular ya da hissedilen düzensizlikler sonrası teknik borcun azaltılması için toplanan ve yapılması düşünülen fikirler çoğalacaktır. Bu nedenle bir komite eşliğinde hangilerinin öncelikli olarak yapılacağını belirlemek önemlidir. Nispeten SonarQube gibi bir aracın bulgularını takım bazında parçalamak kolay olsa da genel mimariyi etkileyen önemli değişikliklerin planlanması için bir komite desteği ve görüşü almak gerekir. Burada önceliklerin belirlenmesi, kayıt altına alınması, planlanması ve takibi gibi konularda çevik metodolojiler uygulanmalı ve bir komite eşliğinde ilerlenmelidir. Bazı firmalarda (örneğin bizde) bu amaçla açılmış özerk mangalar (chapter) görebilirsiniz.
- Dağıt: Öncelik durumlarına göre sıralanan bulgular bu işle uğraşacak bireylere veya takımlara dağıtılır. Bu dağıtım sırasında TDLM Kimlik Kartında belirtilen kişi başına ne kadarlık bir eforun bu işe ayrılacağı mutlak suretle göz önüne alınmalıdır.
- Refactor Yap: Dağıtımı yapılan işlerin oluşturduğu teknik borçlanma gözden geçirilir ve gerekli müdahaleler yapılarak ortadan kaldırılması için çalışılır.
- Raporla: Yapılan değişikliklere ait sonuçlar raporlanır ve güncel durum analiz edilir. Bu aşama görev panosunun da güncellenmesi gereken dilimdir. Yapılan son çalışmalara istinaden borçlanmanın genel durumu şeffaf bir şekilde yenilenmeli ve tüm paydaşlara sunulmalıdır. Üstte belirttiğimiz skor kartının bu aşamada yeniden hesaplanması yerinde bir ölçüm olacaktır.
- Kontrol Et: Burası geri dönülmez kontrol noktası olarak düşünülebilir. Devam etmekte olan geliştirmeler ve önceden var olan kodların yarattığı teknik borçlar törpülense de terk ediş noktası olarak belirlenen hedefin uzağında kalmış olabiliriz. Dolayısıyla döngünün bu safhasında var olan uygulamanın artık yenisi ile değiştirilmesi gerekliliği kararı verilebilir. Diğer yandan göstergeler pozitif anlamda belirlenen bir noktanın üzerindeyse köklü mimari dönüşümler için hazırlıklara başlanması düşünülebilir. Örneğin mikro servis mimari için domain bazlı parçalamalar için gerekli alt yapı hazırlıkları IT4IT işleri kapsamında bitmiş olabilir…

![techdebt_10.png](/assets/images/2021/techdebt_10.png)

Burada bahsi geçen sistem genel konsept olarak teknik borç yükü altındaki birçok uygulamaya uyarlanabilir. Hatta siz bile kendi TDLM sürecinizi tasarlayıp çeşitli araçlarla donatabilirsiniz. Önemli olan teknik borçla mücadelenin iş birimi, bilişim personeli ve paydaşlar tarafından anlaşılmış, kabul edilmiş olması ve bu çerçevede karar verilen bir strateji ile belli bir metodoloji altında icra edilmesidir. TDLM gibi bir yaşam döngüsü sürekli tekrar eden bir kültür sağlar. Bu kültürün devamlılığının sağlanması da başlı başına bir konudur.

#### EK — İleriyi Görerek Modernizasyonu Yapmak

Daha önceden de belirttiğim üzere bazı legacy sistemleri bir sonraki seviyeye geçirmeden önce modernize etmek ya da anlamaya çalışmak önemlidir. Bu amaçla yapılan IT4IT işlerinde genellikle neler yapılacağı maddeler halinde ortaya konur ve bir komite eşliğinde veya farklı bir strateji ile ilgililere dağıtılarak icra edilmeye çalışılır. Ancak gözden kaçan bir nokta vardır; Bir IT4IT işinin ne işe yaradığı çoğu zaman açık ve anlaşılır olsa da ürünün sonraki kademesinde hangi alanın ihtiyacını karşılayacağı ya da devam eden dönemlerdeki hangi IT4IT işini tetikleyeceği bilinmez. Bu yüzden modernizasyon için de bir yol haritası oluşturmak yararlı olabilir. Kabaca aşağıdaki haritanın çok daha büyük ve kapsamlı bir versiyonunu kullanabiliriz.

![techdebt_11.png](/assets/images/2021/techdebt_11.png)

Bu sayede yapılan işin neyi tetikleyeceği ve hedef seviyede hangi alana işaret edeceği net bir şekilde modernizasyona dahil olan tüm paydaşlar tarafından anlaşılır hale gelecektir.

[Youtube Link](https://www.youtube.com/watch?v=vmTVVl5rOU4)

Sunuma [bu adresten](https://www.slideshare.net/BurakSelimSenyurt1/monolitik-uygulamalarda-teknik-borlanma-ile-mcadele-teori) erişebilirsiniz.

### Kaynaklar

- [Technical debt and agile software development practices and processes: An industry practitioner survey](https://www.sciencedirect.com/science/article/pii/S0950584917305098#:~:text=Technical%20debt%20describes%20the%20consequences,technical%20implementation%20and%20design%20considerations.),
- [The Cost of Poor Software Quality in the US: A 2020 Report](https://www.it-cisq.org/cisq-files/pdf/CPSQ-2020-report.pdf),
- [What is Technical Debt](https://www.productplan.com/glossary/technical-debt/),
- [CAST Technical Debt Estimation](https://www.castsoftware.com/research-labs/technical-debt-estimation),
- [Identification and Management of Technical Debt: A Systematic Mapping Study](https://pdf.sciencedirectassets.com/271539/1-s2.0-S0950584915X00115/1-s2.0-S0950584915001743/Nicolli_S_R_Alves_Technical_Debt_2015.pdf?X-Amz-Security-Token=IQoJb3JpZ2luX2VjEKj%2F%2F%2F%2F%2F%2F%2F%2F%2F%2FwEaCXVzLWVhc3QtMSJHMEUCIQCP37P1LrIXgSHRN%2FjW2tUniDkeMKXvPcgPdDC8A9DOpgIgWCoIxS4aWl4C63BcrlhF5d%2BTpHGXWt3J%2BVRdd2HyCekqgwQIwf%2F%2F%2F%2F%2F%2F%2F%2F%2F%2FARAEGgwwNTkwMDM1NDY4NjUiDIrPuHMmb5VbUE4f%2FirXA3YpbV4M0ZU79eov3dUWwRzZ4qF1BG%2FJQk%2BBa1nSP1ywyNVqUPk%2BUyuNsb80mqRt7PVMMAulItYWOc9E3caZ6ql%2B12aFzMx6Xcf566bv1HW59IbHprXUI4NT%2BNE4HgZURIalOa7ak%2FK1XoY0BXjMERfgtyPLTRstJDGJXtOj5%2FVhv8bJMXSexpx7mrapqP5YmOkRq7unz9eqT4UMiiF4lq535lYMhb20N0aZosfwtZNE05vNh%2B8YOmB2Lv5%2FlXddKQqvcLGNZsWOKzH58z5TM2lBlFPA1EjQ1zy32BMruGowcMSqYd6XPH7yudKjKkuAgBsbpysBiM5J3GIsRwq%2FLUxKngsY4m7uSaEBNKV3Y0cne214tVcQTyOkzk54kVo4q11RYCLanrLUM2ZDC0yA01YByDpUyUSeSPQe99QJ%2BOzgLMO7nmYFed699sjHoJEy4duLAzKXi0gsVlDuuX3PrPdxuWm1RNujlnESHIWyvBFUHqPsQyOOk5u085wB3wlIVqWQnMs2QtqMvkf%2F7jkAzt0Ky6Dxyd3%2FWMljH489nfqP%2FfflFAx69aaHXyfTLpW3osnXbWLY0m3cvYCZhxHCElSjHJQ4UabGCcgKKcjOC6AYGAOfEjOjuzCqvMqIBjqlAYrfaRr6pmDrWMsBOD2c7iYLqjid%2FzmczBzC%2FsOxQxcMm5YgwFzHga5r9DlqIDeZFnnc%2BNTxOQ9LQti6WP%2BoMHrIAGlf3dK43sPgdTmYWLiEbCYHfdYAX%2Bn6W6JDeJAcsF3Pnw3vXM1MPzM5xBD6LQDixDZh0MzhHHQ4ACQqGw3mxZpiWMVQ2iMsJGl2n20fbiVK4r37Z1H8S%2Bu2mOy0vqk6cAnlpQ%3D%3D&X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Date=20210810T160818Z&X-Amz-SignedHeaders=host&X-Amz-Expires=300&X-Amz-Credential=ASIAQ3PHCVTYQHS5BDBD%2F20210810%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Signature=73c39a99e3a29eff3a4955674161afe67e53c09f5beca39c08437515839ebf9c&hash=af1a40704bfc93396cb420d58c61c9dd3214787d4a0ecd55e0e0d0e005d5c452&host=68042c943591013ac2b2430a89b270f6af2c76d8dfd086a07176afe7c76c2c61&pii=S0950584915001743&tid=pdf-1b40ad4e-73cb-462b-a66b-9741505b548e&sid=950738e73dd014440359d5f259870579643bgxrqb&type=client),
- [Tech debt: Reclaiming tech equity](https://www.mckinsey.com/business-functions/mckinsey-digital/our-insights/tech-debt-reclaiming-tech-equity),
- [Managing Technical Debt](https://apps.dtic.mil/sti/pdfs/AD1123234.pdf),
- [A Field Study of Technical Debt](https://insights.sei.cmu.edu/blog/a-field-study-of-technical-debt/),
- [A systematic literature review on Technical Debt prioritization: Strategies, processes, factors, and tools](https://www.sciencedirect.com/science/article/pii/S016412122030220X),
- [TechnicalDebt, Martin Fowler](https://martinfowler.com/bliki/TechnicalDebt.html),
- [Fundamentals of Software Architecture](https://www.amazon.com/Fundamentals-Software-Architecture-Comprehensive-Characteristics/dp/1492043451), Richards & Ford, O’Reilly
- [Technical Debt A Complete Guide — 2021 Edition](https://www.amazon.com.tr/gp/product/1867433923/ref=ppx_yo_dt_b_asin_title_o04_s00?ie=UTF8&psc=1), The Art of Service
