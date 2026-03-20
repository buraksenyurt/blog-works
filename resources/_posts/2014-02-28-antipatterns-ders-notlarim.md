---
layout: post
title: "AntiPatterns Ders Notlarım"
date: 2014-02-28 13:10:00 +0300
categories:
  - anti-patterns
tags:
  - anti-patterns
  - dotnet
  - oracle
  - nosql
  - http
  - authentication
  - java
  - javascript
  - performance
  - generics
---
Yazıyı yayınladığım şu andan sadece bir kaç saat sonra sekizinci [NedirTv](http://www.nedirtv.com) kuruluş yıl dönümü etkinliğinde konuşma fırsatı bulacağım. Konularım AntiPatterns ve NoSQL. AntiPatterns konusu ile ilişkili olarak daha önceden Y.T.Ü. tarafından düzenlenen [Finans ve Yazılım Günleri](https://www.buraksenyurt.com/post/YTU-Finans-ve-Yazc4b1lc4b1m-Gunleri)’ nde konuşma fırsatım olmuştu. Her iki etkinliğe de hazırlanırken, sektörde yer aldığım süre içerisinde gözlemlediğim bilgileri özellikle dikkate almaya çalıştım. Pek tabi konuyu doğru bir şekilde aktarabilmek için teknik destek ve referans kaynaklar da gerekiyordu. Şüphesiz ki böylesine önemli bir konu, teoride olduğu kadar pratikte de tecrübe edilmişse izah edilebilirdi.

[![anti_0](/assets/images/2014/anti_0_thumb.png)](/assets/images/2014/anti_0.png)


Yazının bundan sonraki bölümlerinde, AntiPatterns konusuna hazırlanırken aldığım çalışma notlarının derlenmiş halini bulabilirsiniz. Çok fazla düzenlenmemiş halleri ile paylaşıyorum. Aralara kendi yorumlarımı da katmaya çalıştım tabi. Faydalı olmasını dilerim

![Winking smile](/assets/images/2014/wlEmoticon-winkingsmile_218.png)

Küçük Bir Hikaye

[![storytime1](/assets/images/2014/storytime1_thumb.jpg)](/assets/images/2014/storytime1.jpg) Vaktiyle zamanında Z sektöründe var olan kürsel ölçekli X isimli bir firma varmış. Firmanın yazılım geliştirme alanında ikiyüzden fazla elamanı bulunmaktaymış. Yazılım departmanı içerisinde bir kaç grup bulunuyormuş. Bu guruplardan bazıları vaktiyle zamanında tüm kurumu ilgilendiren bir takım süreçlerin önemli bir parçası üzerinde çalışmaya başlamışlar. Ne yazık ki farklı dönemlerde.

İstenen iş bir dönem bir ekipte, diğer bir dönem başka bir ekipte ele alınmış. Ürünün akademik destek gerektirebilecek önemli bir parçası içinse V isimli Vendor’ la anlaşılmış. V, standart ürününü ilk başından itibaren ekiplerin kullanımına sunmuş. Ancak, X’ nin iş biriminden gelen özel istekler nedeniyle standart ürünlerini bazı noktalarda özelleştirmek zorunda kalmış. Örneğin sırf X firması istediği için uzmanlık alanına girmediği halde arayüzler tasarlamış.

Bir süre sonra V firması, ürününe ait bazı kütüphanelerin kaynak kodlarını da anlaşma gereği X firmasına vermiş. Ekip içinde tek başına çalışan bazı geliştiriciler, zaman içerisinde üzerine aldıkları bu ürünü alıp kendi ihtiyaçları doğrultusunda değiştirmişler. Ürünün zaman içinde devredildiği her ekip bazen aynı amaçlara hizmet eden kod parçalarını, karmaşıklıklarından dolayı tekrardan yazmak zorunda kalmış. Hatta kodlar tekrardan incelendiğinde hiç kullanılmayan, bir köşede unutulmuş, atıl olmuş parçalara rastlanmış.

Bazı noktalarda ise mecburen V’ ye gidilmesi gerekmiş. Bu istekler basit de olsa V, X firmasına toplantı talepleri göndermiş. Analizler yapılması gerektiğini vurgulamış. Analizler ve uzun süren toplantılar nedeniyle yeni istekler çoğu zaman gecikmiş. Ayrıca V, bu istekleri faturalamakta ve uzun sürede gerçekleştirmekteymiş.

Sonunda X, söz konusu ürünü In-House olarak geliştirme kararı almış. Hem de uzun yıllar sonunda. Ancak ürünün kritik noktalarında yer alan bazı Vendor çözümleri şirketin ağı içerisinde öyle bir yayılmış ve kullanılmış ki, var olan sistemi çöpe atmak gibi bir alternatif söz konusu değilmiş. Yeni ekibin eski ürünün bazı özelliklerini alıp yeni çözümde kullanması ve bunu yaparken yeni çözümün mükemmel alt yapısını da tasarlaması gerekiyormuş.

Eski ürünün özellikle veri katmanı tarafındaki oluşumunda da sıkıntılar bulunuyormuş. İsimlendirme standartları çoğu yerde ihlal edilmiş, düzgün bir domain yapısı kurgulanamamış, kurgulanmışsa da zaman içerisinde bozulmuş, veriler normalize edilmek zorunda kalacak şekilde saklanmış, veriye erişim için pek çok noktada tekrar eden işlevsellikler türemiş vb…

İşin kötü tarafı ekibe verilen süre ve kaynak sayısı oldukça yetersizmiş. Daha da kötüsü ekipten çevik (Agile) olmaları ve son bir sene içerisinde diğer birimlerin yaptığı gibi Scrum metodolojisinde yürümeleri isteniyormuş. Ancak ekip daha önce çevik bir proje geliştirme tecrübesi yaşamamış.

Bu kısa hikaye, içerisinde bir kaç AntiPattern tanımı bulundurmaktadır. Bu AntiPattern’ ler nedeniyle ürün, çeşitli zaman dilimlerinde tıkanma notkasına gelmiş, bu yüzden yeniden yazılması söz konusu olmuştur.

> İlerleyen bölümlerde yer alan AntiPattern tanımlarını okuduktan sonra, hikayede geçenleri bulabilirsiniz. Diğer yandan sizin içerisinde bulunduğunuz projelerde de benzer vakaların oluşup oluşmadığını tespit etmeye çalışarak pratik yapabilirsiniz.

Başlamadan Önce

AntiPatterns konusunu daha iyi kavrayabilmek adına kendinize şu soruları sorabilirsiniz.

- Hiç bir tasarım kalıbını veya kod parçasını nasıl çalıştığını anlamadan kullandınız mı?
- Ne kadar iş kuralı varsa hepsini arayüz'deki kontroller arkasına gömdünüz mü?
- Kürsel olarak sunulmuş bir çözümü kullanmak yerine onun özel vakaları olduğunu düşünüp tekrardan yazma yoluna gittiniz mi?
- Mükemmel olması için en ince detayına kadar incelenen bir ihtiyaca ait ürünü geliştirmek için analizin çıkmasını beklediniz mi?
- Bir zamanlar deneme amaçlı olarak geliştirdiğiniz kütüphaneleri bir ürünün geliştirilmesinde doğrudan kullandınız mı?
- Kernel gibi isimlendirdiğiniz ve önemli olduğunu düşündüğünüz tüm fonksiyonellikleri içerisine kattığınız devasa bir sınıf geliştirdiniz mi?
- Daha önce başarılı bir şekilde kullandığınız çözüm mimarisini sonraki ürünlerde terk etmeyi hiç düşündünüz mü?
- Yıllar sonra geliştirdiğiniz kütüphane içerisinde var olan ama artık atıl olmuş bir sınıfa/metoda rastladınız mı?
- Ürün içerisinde bir başkasının uyguladığı kod parçasının aynen yapıştırıp içeriğinde biraz değişiklik yaparak kullandınız mı?
- Kaynak kodları açık olan ve şirket içerisindeki ürünlerde kullanılan 3ncü parti bir bileşende özelleştirme yaptınız mı?
- Bir servis den aldığınız hata mesajını son kullanıcıdan gizlediğiniz oldu mu?

Vereceğiniz cevaplar ne olursa olsun oluşan hal genellikle bir AntiPattern oluşumunu işaret edecektir.

Ön Bilgiler ve Tanımlama (Tanımlamaya Çalışma)

[![andrewkoenig](/assets/images/2014/andrewkoenig_thumb.png)](/assets/images/2014/andrewkoenig.png) AntiPatterns terimi 1995’ de Andrew [Koenig](http://en.wikipedia.org/wiki/Andrew_Koenig_(programmer))’ in Journal of Object Oriented Programming[(ki doğrulatamadığım bilgilere göre bunun yerini Journal of Object Technology almıştır)](http://www.jot.fm)’ de yayınlanan C++ köşesindeki Patterns and AntiPatterns makalesinde şu şekilde tanımlanmıştır;

> AntiPattern is just like pattern, except that instead of solution it gives something that looks superficially like a solution, but isn't one. (Koenig, 1995)
> Şöyle Yorumlayabiliriz: AntiPattern görünüşte (yüzeysel anlamda) çözüm zannedilen bir Pattern gibidir, ama aslında değildir.

Koenig’ in bu tanımı daha sonra Cambridge Üniversitesi’ nin derlediği [The Patterns Handbook: Techniques, Strategies, and Applications (SIGS Reference Library)](http://www.amazon.com/Patterns-Handbook-Techniques-Strategies-Applications/dp/0521648181/ref=sr_1_1?s=books&ie=UTF8&qid=1390554526&sr=1-1&keywords=The+Patterns+Handbook%3A+Techniques%2C+Strategies%2C+and+Applications) isimli kitaba da dahil edilmiştir.

AntiPattern’ ler yazılım geliştirme de kötü çözüm yaklaşımları ve pratikleri olarak da bilinirler. Söz gelimi tasarım desenleri, belli başlı problemlerin çözümünde standart pratikleri önerirken, AntiPattern’ ler tam tersine arzu edilmeyen ve sonrasında daha büyük problemlerin kapısını açan pratiklerin uygulanması olarak düşünülebilir. Aslında AntiPattern’ lerin tehlikeli tarafı ürün geliştirme süreçlerinde ve vakalarda en uygun çözüm yolu olarak düşünülmeleridir.

> Dünün en popüler çözümü bugünün AntiPattern’ i olabilir.

AntiPattern’ ler aslında Design Pattern’ lerin doğal bir uzantısı olarak yazılım geliştirme sürecinde tekrar eden bazı yanlışları ifade etmektedir ve özünde bu yanlışları engellemek, anlamak ve onlardan korunmak için gerekli bir takım tanımlamaları da içermektedir.

> AntiPatterns kavramlarını bilmek, yazılım geliştirme sürecinde karşılaşılabilecek ciddi problemleri önceden tahmin edebilmeyi ve tedbir almayı kolaylaştırır.

Diğer yandan mimari kavramlar ile gerçek dünya uyarlamaları arasındaki boşluğu dolduran bir köprü olarak da vurgulanmışlardır. Nitekim mimari kavramların doğru uygulanamadığı veya seçilemediği noktalarda, gerçek dünya ihtiyaçlarını karşılamak için ele alınan ve ideal gibi görünen çözümler, iki dünya arasındaki boşluğu dolduran AntiPattern’ ler haline gelebilirler.

> Bir Pattern çözdüğünden daha fazla problem oluşturuyorsa AntiPattern’ dir.

Çok sık verilen ve bilinen AntiPattern örnekleri vardır. Spaghetti Code, Copy-Paste Programming, God Object (BLOB AntiPattern olarak da bilinir) vb. Yazılım dünyasının genişlemesi, platformlar arası iletişimin artması ve Enterprise olarak kabul edilen çözümlerin yaygınlaşması ile birlikte, AntiPattern konusu daha da önem kazanmıştır. Sayısız AntiPattern vardır ve bunlar ana ve çeşitli alt kategoriler halinde sınıflandırılmaktadır.

> Gün içerisinde vakit ayırıp en azından bir AntiPattern vakası okumak, bununla ilişkili semptomları kavramak, çözüm yollarını incelemek ve olası istisnai durumları tanımak her yazılımcı için önemlidir. Hatta aşağıdakine benzer post-it’ ler hazırlanması oldukça faydalıdır.
> [![ap_receipe](/assets/images/2014/ap_receipe_thumb.png)](/assets/images/2014/ap_receipe.png)

AntiPattern’ lar belirli karakteristikler sergilerler. Her birinin tipik oluşma nedenleri (Typical Causes), oluştuklarına dair belirtileri (Symptoms) ve oluşmaları sonrası ortaya çıkan neticeleri (Consequences) vardır. Her birinin düzeltilmesine yönelik çözümler bulunur (Refactored Solutions) Oluşmalarına müsade edilebilecek İstisnai durumlar da söz konusudur (Exceptions).Bu nedenle AntiPattern’ lar okunurken bu maddeler çerçevesinde değerlendirilmelidirler.

Şablon (Template)

Bir AntiPattern tanımlanırken standart olarak aşağıdaki gibi bir şablon (Template) kullanılır. Bu şablona göre var olan bir takım koşullar ve sebepler sonucu oluşan AntiPattern’ in ürettiği sonuçlar ve uygulanan çözümün AntiPattern olduğunu işaret eden belirtiler tanımlanır. Sonrasında Refactoring çözümü ve bu çözümün çıktısı olan sonuçlar ve kazançlar belirtilir. Pek tabi bir AntiPattern başka AntiPattern veya Pattern’ ler ile ilişkilendirilebilir.

[![ap_template](/assets/images/2014/ap_template_thumb.png)](/assets/images/2014/ap_template.png)

Kategorilendirme

Hays W.”Skip” McCormick’ in yer aldığı [AntiPatterns:Refactoring Software,Architectures, and Projects in Crisis](http://www.amazon.com/AntiPatterns-Refactoring-Software-Architectures-Projects/dp/0471197130/ref=sr_1_1?s=books&ie=UTF8&qid=1390555386&sr=1-1&keywords=AntiPatterns) isimli kitaba göre 3 ana kategori söz konusudur.

Software Development
Software Architecture
Software Project Management

AntiPatterns.com’ a göre kategorilendirme yine yukarıdaki 3 ana başlık altında detaylanmakta ancak her ana kategori için birer Mini AntiPattern sınıflandırılmasına da gidilmektedir.

> Kategorilendirme de esas nokta Uygulama Geliştirme Yaşam (Application LifeCycle Management) döngüsüne dahil olan farklı bakış açılarıdır. Bu bakış açıları temelde üç pozisyon ile ilgilidir. Mimar (Architect), Geliştirici (Developer) ve Yönetici (Manager).

Development AntiPatterns
The Blob (God Class)
Cut and Paste Programming
Functional Decomposition
Golden Hammer
Lava Flow
Plotergeists
Spaghetti Code
Development Mini AntiPatterns
Ambigous Viewpoint
Boat Anchor
Continous Obsolescence
Dead End
Input Kludge
Mushroom Management
Architecture AntiPatterns
Architecture by Implication
Design By Committee
Reinvent the Wheel
Stovepipe Enterprise
Stovepipe System
Vendor Lock-In
Architecture Mini AntiPatterns
Auto Generated Stovepipe
Cover Your Assets
The Grand Old Duke of York
Intellectual Violence
Jumble
Swiss Army Knife
Wolf Ticket
Management AntiPatterns
Analysis Paralysis
Corncob
Death By Planning
Irrational Management
Project Mis-Management
Managemenet Mini AntiPatterns
Blowhard Jamboree
Email is Dangerous
Fear of Success
The Feud
Smoke and Mirrors
Throw it Over The Wall
Viewgraph Engineering
Warm Bodies

Ben sunum için Wikipedia tarafında yayınlanan AntiPatterns maddelerini ele almaya çalıştım ve onları aşağıdaki gibi daha anlaşılır bir grafik haline getirdim. (Sarı punto ile işaretlenen başlıklar, sunumlarda vakit yettiği ölçüde ele almak istediğim konulardı)

[![anti_18](/assets/images/2014/anti_18_thumb.png)](/assets/images/2014/anti_18.png)

[![apktgri](/assets/images/2014/apktgri_thumb.png)](/assets/images/2014/apktgri.png)

Kaynaklar

Konuya hazırlanırken kavramın ilk çıkış noktasına denk gelen kaynakları özellikle takip etmeye çalıştım. Tabi gelişen teknoloji düşünüldüğünde güncel bir kaç araştırmaya ve ağırlıklı olarak Wikipedia’ ya bakmam gerekti. İşte okuduğunuz dokümanın oluşmasında kullandığım değerli kaynaklar.

[![anti_19](/assets/images/2014/anti_19_thumb.png)](/assets/images/2014/anti_19.png)
[AntiPatterns Internet Sitesi](http://www.antipatterns.com)

[AntiPatterns: Refactoring Software, Architectures, and Projects in Crisis by William J. Brown, Raphael C. Malveau, Hays W. "Skip" McCormick and Thomas J. Mowbray (Apr 3, 1998)](http://www.amazon.com/AntiPatterns-Refactoring-Software-Architectures-Projects/dp/0471197130/ref=sr_1_1?s=books&ie=UTF8&qid=1390555386&sr=1-1&keywords=AntiPatterns)

[The Patterns Handbook: Techniques, Strategies, and Applications (SIGS Reference Library)](http://www.amazon.com/Patterns-Handbook-Techniques-Strategies-Applications/dp/0521648181/ref=sr_1_1?s=books&ie=UTF8&qid=1390554526&sr=1-1&keywords=The+Patterns+Handbook%3A+Techniques%2C+Strategies%2C+and+Applications)

[Design Patterns Past and Future](http://proceedings.informingscience.org/InSITE2011/InSITE11p109-138Bulajic276.pdf)

[Wikipedia](http://en.wikipedia.org/wiki/Anti-pattern)

[Sourcemaking](http://sourcemaking.com/antipatterns)

Bazı AntiPatterns Vakaları

Pek çok AntiPatterns bulunmakta. Hepsine çalışacak vakit olsa da tamamının özetini çıkartmaya yetecek kadar yoktu. Yine de en çok dikkatimi çeken ve anlamakta zorlanmadıklarımın bir özetini çıkartmayı başardım. Programlama kategorisinden Lava Flow ile başlayalım.

## Lava Flow – Programming

> Lüzumsuz veya düşük kaliteli kodları, kaldırma maliyetlerinin yüksek olması veya ön görülemeyen sebepler nedeniyle barındırmaya devam etmek.

[![anti_5](/assets/images/2014/anti_5_thumb.png)](/assets/images/2014/anti_5.png) Bazen araştırma amaçlı olarak başlayan yazılımlar ürüne dönüşürler. Ürüne dönüşme noktasından geriye doğru bakıldığında ne amaçla yazıldıkları belli olmayan (hatta yazan kişinin de şirketten ayrılmış olması sebebiyle kimsenin bilemediği) kod parçalarından oluşan bir tarihi antika oluşabilir. Öyle ki, projede görev alan bazı geliştiricilere sorulduğunda ilgili kod parçasının hangi amaçla yazıldığını hatırlamayabilir (İstisnai bir durum ise gri veya ak saçlı geliştiricilerin bunu hatırlayabilmesidir)

Bu durumun oluşmasının pek çok nedeni vardır. Örneğin projede görevlendirilip tek başına kodlama yapan bir geliştirici sebepler arasında sayılabilir (Lone Wolf Developer veya Single Developer Written Code) ya da deneyimli olmayan veya yeteri kadar bilgisi bulunmayan bir yönetici/mimar bu duruma sebebiyet verebilir.

> Günümüzde vuku bulan bir geliştirici tanımı da Monkey Developer’ dır. Professional Developer ve Monkey Developer arasındaki ayrımı değerlendiren [isabetli bir yazıyı bu adresten okuyabilirsiniz](http://blog.binarymist.net/2014/01/25/essentials-for-creating-and-maintaining-a-high-performance-development-team/).
> Her ne kadar ürünlerin piyasadaki rekabet nedeniyle hızla çıkması gerekse de bu, hızlı development’ ın ortaya koyacağı istenmeyen sonuçların oluşmasını kabul etmek olarak da algılanmamalıdır.

Sonuç olarak kod içeriğine ve projenin genel çatısına bakıldığında müdahale edilmesi zor, modası geçmiş ve kullanılmayan kod parçaları içeren, üstüne sürekli yeni özellikler eklenirken hata ayıklaması da oldukça zorlaşan bir ürünün ortaya çıktığı görülür. İçinde başka AntiPattern’ leri de barındırır diyebiliriz. Söz gelimi Boat Anchor, Spaghetti Code, Error Hiding vb…Kaynak kodun takibi de zorlaşır. Çözüm yollarından birisi Configuration Management’ tır.

İstisnai Durum:

Tabi oluşmasına müsaade edilebilecek durumlar da vardır ki bu durum çalışmanın bir araştırma çalışması olmasıdır. POC (Proof of Concept) tipindeki uygulamalarda bu durumun oluşmasına müsaade edilebilir.

## Analysis Paralysis – Organizational

> Bir projenin analizine orantısız ölçüde yüksek efor harcamak.

[![anti_11](/assets/images/2014/anti_11_thumb.png)](/assets/images/2014/anti_11.png) Analiz Felci olarak bilinir. Bazı projelerin analiz safhası hem uzun sürer hem de kullanılan kaynakların (eleman sayısı gibi) maliyeti yüksek ve fazla olur. Çoğunlukla Waterfall adı verilen yazılım geliştirme metodolojisinde karşımıza çıkar. Analizin uzamasının belli nedenleri vardır. Bunlardan birisi geresiz olabilecek detaylara çok fazla girilmesidir.

Mükemmel bir analiz olmadan tasarım yapılamayacağı ve ilerlenemeyeceği varsayılır. Bu AntiPattern ayrıca günümüzün popüler çevik (Scrum gibi) süreçlerinin daha çok tercih edilmesinde de rol almış olabilir. Nitekim çevik süreçler periyodik iterasyonlar sonucunda işe yarar bir ürünün veya paketin müşteriye sunulmasına odaklanırlar.

İstisnai Durum:

Kitaba göre bu deseni haklı çıkartacak hiçbir istisnai durum yoktur.

## Reinventing the Square Wheel – Methodological

> Var olan bir çözüm yerine ondan daha kötü olan özel bir çözüm üretme hatasına düşmek.

Bazı yazılım problemlerinin çözümünde kullanılacak olan yollar zaten standart ve bellidir. Üstelik bu çözümler için standart hale gelmiş mimari yaklaşımlar, ürünler ve alt yapılar (Frameworks) mevcuttur. (Hatta tasarım kalıpları) Problemin bu tip yardımcılar ile çözülemeyeceğini düşünüp sıfırdan bir çözüm üretilmeye başlandığı hal tekerleğin yeniden keşfi olarak düşünülür ve bu AntiPattern’ in oluşmasına neden olur. Nitekim ekip söz konusu problemin çok özel olduğuna ve var olan pratikler ile ele alınamayacağına inanır.

[![anti_3](/assets/images/2014/anti_3_thumb.png)](/assets/images/2014/anti_3.png) Yazılım geliştirme projeleri arasında teknoloji transferi ve yeterli iletişimin (özellikle bilgi akışının) olmaması bu durumun oluşmasının sebeplerindendir. Diğer yandan sürecin gerektirdiği sistemin en baştan inşa edilmesi gerektiğine inanılması da oluşma sebepleri arasında sayılabilir.

İstisnai Durum:

Bazı istisnai hallerde kabul edilebilir. Özellikle araştırma (Research) nitelikli projelerde, yeni tekniklerin öğrenilmesi veya nasıl çalıştığının anlaşılması gibi süreçlerde ve bazı bilimsel çalışmalarda göz ardı edilebilir.

## Cargo Cult Programming – Programming

> Desen ve metodları ne/nasıl/niçin olduğunu anlamadan kullanmak.

[![anti_7](/assets/images/2014/anti_7_thumb.png)](/assets/images/2014/anti_7.png) Bu aslında bir inanışı sorgusuz sualsiz kabul etmekten dolayı isimlendirilmiş bir desendir. Çoğu zaman geliştirici bir çözüm için kullandığı bileşenleri, prensip ile desenleri, kod parçalarının nasıl çalıştığını/niye kullanıldığını bilmeden uygular. Bu sıkı sıkıya bağlılık aynı felsefeleri başka çözümlerde de kullanmaya çalışmasına yol açar.

Bunun doğal sonucu olarak geliştirici bir süre sonra problemi doğru şekilde algılayıp uygun teşhisi koyma noktasından da uzaklaşabilir.

Bir nevi Copy-Paste Programming AntiPattern yaklaşımının bir sonucu olarak ortaya çıktığını da düşünebiliriz. Nitekim geliştirici çözüm için bir kod parçasının gerekli olup olmadığını ve nasıl çalıştığını anlamadan bir yerden bir yere taşıyarak uygulamaya çalışır.

## Vendor Lock-In – Organizational

> Bir sistemin dışarıdan sağlanan bir bileşene aşırı bağımlı tasarlanması/geliştirilmesi/yürütülmesi.

Müşteri olarak bir üreticinin ürünü veya hizmetine sıkı sıkıya bağlı olunan hallerde ortaya çıkar. Öyleki farklı bir üreticinin ürününe geçiş yapmak için ekstra maliyet altına girmek gerekebilir. Sıklıkla verilen örneklerden birisi müzik sistemi içeriye gömülü olan otomobillerdir. Bunlarda müzik sistemini değiştirmek epeyce külfetli bir iştir.

[![anti_12](/assets/images/2014/anti_12_thumb.png)](/assets/images/2014/anti_12.png) Ya da bir servis sağlayıcısı tarafından sunulan hizmet veya satın alınan bir yazılım bileşeninin herhangi bir ara katman geliştirilmeksizin doğrudan kullanılması bu halin oluşmasına neden olabilir.

Elbette bu duruma düşülmesinin de belli başlı sebepleri vardır. Örneğin sadece pazar ve satış bilgilerine bakılıp, ürünün teknik detayları atlandığında kolayca ortaya çıkabilir. Diğer yandan uygulamanın sıkı sıkıya bağlı olduğu yazılımdan kopartılmasına uygun teknik bir çözüm veya maliyet modeli olmadığında da bu durum oluşabilir.

Bazen büyük firmaların kullandığı sayısız ürün arasına bir şekilde entegre olan servis sağlayıcı ürünler kullanılır. Aslında bu ürünler ilk etapta firmanın pek çok yerinde kullanılacak diye alınmaz. Fakat şartlara ve günün ihtiyaçlarına göre bu tip bir oluşum söz konusu olabilir. Uzun bir müddet sonra ilgili ürünlerde ve uzantılarında yapmak istenilen ekler ve değişikliker Vendor’ a ait ürünlerde de bir takım geliştirmeleri gerekir. İşte bu nokta da Vendor ile sürekli bir fiyat pazarlığı haline kalınabilir.

Durumun oluşmasını engellemek için uygulama bazında katmanları iyi bir şekilde izole etmek ve vendor ürünü yerine yeri geldiğinde bir başkasını koyabilme kabiliyetini oluşturabilmek gerekir. Bu da ürün ile onu kullanan asıl uygulama arasında izole edilmiş bir katmanın olması ile sağlanabilir.

İstisnai Durum:

Bir uygulamanın gerektirdiği harici çözümleri büyük oranda karşılayan tek bir Vendor ile çalışılıyorsa göz yumulabilir.

## OverEngineering – Project Management

> Bir projeyi gereğinden daha karmaşık ve güçlü hale getirmek için kaynak harcamak.

[![anti_16](/assets/images/2014/anti_16_thumb.png)](/assets/images/2014/anti_16.png) Bir uygulamanın gereğinden ve ihtiyaç duyulandan daha fazla oranda kompleks geliştirilmesidir. İstenen şeyler çok basit seviyede olabilecekken, gereksiz yere karmaşıklaştırılır. Bu bir nevi sedan tipindeki aile arabasının saatte 300 KM süratle gidebilmesini sağlayacak teknolojiyi kullanarak bir üretim bandı hazırlanmasına ve yapılmasına benzer.

Sonuçta üretim ve bakım maliyeti yükselir, kullanılan ustaları kumaşı daha da pahalı hale gelir. Kısacası bir problemi olduğundan daha karmaşıkmış gibi algılayıp çözmeye çalışmak olarak düşünülebilir.

## Golden Hammer – Methodological

> Favori bir çözümün evrensel anlamda kabul gördüğünü varsaymak.

[![anti_2](/assets/images/2014/anti_2_thumb.png)](/assets/images/2014/anti_2.png) Daha önceden başarılı bir şekilde uygulanmış bir çözümün, sonraki problemlerde de kullanılmaya çalışılması olarak düşünülebilir. Oysaki bazı problemler aynı yöntemler ve yaklaşımlar ile çözümlenemeyebilir. Bu, biraz da çözümü arayan kişilerin daha önce başarılı bir şekilde uyguladıkları yaklaşımları sahiplenmesinden kaynaklanır.

Örneğin tüm yazılımların SOA (Service Oriented Architecture) mimari bütünü içerisinde ele alınması gerektiğini düşünmek bu duruma örnek olarak verilebilir. En sık görülen AntiPattern’ ler arasında yer alır.

Söz konusu durumun oluşmasının nedenlerinden birisi, teknolojik gelişmelerden ekiplerin haberdar olmamasıdır. Bazı firmalar gerekli eğitimlerin getirdiği ek maliyetlerden kaçınır ve ekibi donatmaz. Hatta bazı ekiplerin bireysel anlamda gelişmenin önünü açacak aktivite ve çalışmalara müsaade etmemesi de bu sebepler arasında sayılabilir.

İstisnai Durum:

Kullanımını geçerli kılan istisnai durumlar da vardır. Söz gelimi uzun soluklu ürünlerde Oracle veri tabanına ait veri nesnelerinin kullanılması ve diğer çözümler için de ele alınması kabul edilebilir bir yaklaşımdır. Bankalarda bu tip veri tabanı seçimleri genellikle bir kez yapılır ve kolay kolay değiştirilmez.

Tabi burada da belli başlı sıkıntılar doğmaktadır. Özellikle tüm iş kurallarının Oracle içerisine gömülmesi, Java kodlarının Stored Procedure’ ler içerisine alınması, SQL ile PL-SQL geçişlerinin sıklaştırılması vb bir süre sonra satır sayısı artan, kontrol edilmesi, güncelleştirilmesi, yönetilmesi zorlaşan, performans sıkıntıları doğuran ürünlerin oluşmasına sebebiyet verebilir.

## Boat Anchor – Programming

> Her hangi bir amaçla kullanılmayan bir sistem parçasını tutmak/unutmak.

[![anti_6](/assets/images/2014/anti_6_thumb.png)](/assets/images/2014/anti_6.png)“Bu metoda/kütüphaneye ilerleyen zamanlarda ihtiyacımız olabilir” denilerek yazılan fakat yazıldığı yerde unutulan kod parçalarının ortaya çıkarttığı durumdur.

Unutulan kod parçaları yıllar sonra bırakıldıkları halleri ile kafalarda tam bir soru işareti oluşturabilirler. Neden yazıldıkları, ne amaçla kullanıldıkları bilinmeyebilir.

Yazılımsal görünümü haricinde donanımsal tarafta da karşımıza çıkar. Örneğin artık demode olmuş, teknolojisi oldukça eskimiş bir bilgisayarın hasarlı bile olsa bir köşede durmaya devam etmesi/unutulması olarak da düşünülebilir.

## Copy-Paste Programming – Methodological

> Daha generic bir çözüm üretmek yerine var olan kodları koplayarak geliştirme yolunu tercih etmek.

[![anti_4](/assets/images/2014/anti_4_thumb.png)](/assets/images/2014/anti_4.png) Kaynaklarda Cut-Paste Programming olarak da geçer. Çoğunlukla bir çözüm için yazılımın her hangi bir yerinde uygulanan bir kod parçasının, ihtiyaç olunan başka bir yerde aynen kopyalanarak kullanılmaya devam etmesi olarak tanımlanır.

Bunun doğal sonucu olarak bir değişiklik olması halinde kodun çoğaltıldığı yerlere gidilmesi de gerekecektir. Güncellemeler için fazla maliyetli eforlar sarf edilebilir. Hatalar gözden kaçabilir ve uygulamanın yanlış çalışma riski giderek artabilir.

Söz konusu parçaları soyutlayıp nesne yönelimli dil temellerine uygun olacak şekilde ayrıştırmak önemlidir. Günümüzün gelişmiş yazılım mimari yaklaşımlarında bu tip bir durumun oluşması nispeten daha azdır ama yine de günü kurtarma stratjisi burada da etkisini gösterebilir. Başka adları da vardır. Clipboard Coding, Software Cloning, Software Propogation gibi.

İstisnai Durum:

İlginçtir ama bu AntiPattern, kodun bir an önce dışarıya çıkartılması gerektiği durumlarda kabul edilebilir. Tabi bakım maliyetleri de bu durumda kabul edilmiş olunur.

## Spaghetti Code – Programming

> Özellikle kod yapılarının kötü kullanılması nedeniyle güç anlaşılır programların oluşması.

Genellikle mantıksal bir tasarım bütünü içerisinde düşünülmeden hareket edildiğinde ortaya çıkar. Nesne yönelimli dil yetenekleri göz ardı edilir ve neredeyse her iş süreci için ayrı birer fonksiyonun yazılması söz konusudur.

[![anti_8](/assets/images/2014/anti_8_thumb.png)](/assets/images/2014/anti_8.png) Bunun doğal sonucu olarak okunabilirlikten uzak, takibi oldukça zor bir kod bütünü ortaya çıkar. Yaşam döngüleri içerisinde sürekli olarak güncellenen programlar ve deneyimsiz geliştiriciler bu tip kodların oluşmasına sebebiyet verebilir.

- Nesne yönelimli olmayan dillerde daha sık rastlanır.
- Metotlar daha çok süreç odaklı yazılır hatta süreç adları olarak isimlendirilir.
- Nesneler arasında neredeyse hiç ilişki yoktur.
- Çoğu metod parametre almaz ve global seviyedeki sınıf değişkenlerini oluşturmakta kullanılır.
- Kodun yeniden kullanılabilirliği zordur.
- OOP temel özellikleri (kalıtım, çok biçimlilik, soyutlama) kullanılmaz.

> Bu tip kodları genellikle kodun sahibinden başkası anlamaz ve çoğu zaman yeniden yazılması gündeme gelir.

[![anti_9](/assets/images/2014/anti_9_thumb.png)](/assets/images/2014/anti_9.png) İstisnai Durum:

Bazı ara yüz parçalarında implementasyonun iç içe olduğu hallerde göz ardı edilebilir. Özellikle bileşenin yaşam ömrünün kısa olduğu ve sistemin geri kalanından tamamen ayrıldığı/izole edildiği durumlarda göz yumulabilir.

Örneğin bir web uygulamasına ait ara yüzlerde yoğun javascript kullanıldığı hallerde (ki günümüzde popüler olarak kullanılan pek çok JavaScript tabanlı Client Library vardır) sayfa kodunun bu şekilde karmaşıklaşması istisnai bir durum olarak görülebilir.

## Error Hiding – Programming

> Bir hata mesajının kullanıcıya gösterilmeden önce yakalanması ama kullanıcıya ya hiçbir şey gösterilmemesi ya da anlamlı bir mesaj gösterilmemesi. Ayrıca Stack izlerinin Exception’ ın ele alındığı sürede silinmesi ve hata ayıklamaya engel olunması.

[![anti_10](/assets/images/2014/anti_10_thumb.png)](/assets/images/2014/anti_10.png) Bazen geliştiriciler, asıl hata mesajını son kullanıcıdan saklama veya anlamlı bir gerekçe göstermeme yolunu tercih ederler. Çoğunlukla nesne yönelimli dillerin kullanıldığı senaryolarda Exception Handling noktasında kendisini gösterir.

Kullanıcının oluşan hata ile ilişkili olarak anlamlı bir mesajla uyarılmayışı çok doğal olarak sorunun anlaşılamaması demektir. Bu AntiPattern oluştuğunda geliştirici genellikle exception handling’ i ezer ve uç noktalara anlamlı mesajlar vermeyi ihmal eder.

Oysaki uygulamalarda oluşan hataları seviyelendirmek, seviyesine göre doğru pozisyonlar için kritik olan detay kadar tutmaya çalışmak ürünlerin gelişimi açısından önemlidir. Son kullanıcının tipine göre verilmesi gereken bir hata mesajı aslında ters noktadaki fonksiyon da, onu ele alacak geliştirici açısından detaylandırılabilmelidir.

## Dead End – Software Development

> Ticari bir yazılımın modifiye edilmesinin oluşturduğu bakım yükü.

[![anti_13](/assets/images/2014/anti_13_thumb.png)](/assets/images/2014/anti_13.png) Genellikle çözümlerde kullanılan 3ncü parti bileşenlerde değişiklikler yapıldığında ortaya çıkar. Bileşenin destekçisinin yapacağı bir güncelleme sonrasında, hali hazırda yapılmış olan değişiklikler çok doğal olarak ortadan kalkar. Kalkması istenmiyorsa bu durumda entegre edilmesi için epeyce efor sarf edilmesi gerekebilir.

Aslında bu tip bileşenlerde değişiklik yapmaktan kaçınmak daha doğrudur. Pek tabi bileşen sahibi bir süre sonra ürüne olan desteğini de bırakabilir.

Bu durumun önüne geçmek için aynen Vendor Lock-In durumunda olduğu gibi bileşenleri izole edilmiş bir katman ile asıl uygulamadan ayrıştırma yolu tercih edilmelidir.

İstisnai Durum:

## God Object – Object Oriented Design

> Tasarımın tek bir parçasının-ki burada kastedilen bir sınıftır- çok fazla sayıda fonksiyona konsantre olmasıdır.

[![anti_14](/assets/images/2014/anti_14_thumb.png)](/assets/images/2014/anti_14.png) Bir sınıf içerisine çok sayıda fonksiyonelliğin gömüldüğü (60dan fazla metot) ve sınıf ile ilişkili verilerin ayrı veri sınıflarında tutularak bu sınıfla ilişkilendirildiği hal olarak düşünülebilir. Process odaklı procedural yaklaşım da diyebiliriz.

- Bu durumda tüm iş yükünü üstlenen sınıfların bakımı, genişletilmesi, iş mantıklarının kolayca okunur olarak içerisinde yer alması oldukça zorlaşır.
- Karmaşıklığın yanında, belleğe yüklenmesi zaman alan nesneler ortaya çıkar.
- Sınıf daha da kalabalıklaştığında yeni istekler için güncellemelerin yapılması zorlaşacaktır.
- Üstelik iş mantıklarının da yeteri kadar soyutlanamaması nedeniyle tekrarlı kodların önüne geçilmesi mümkün olmayacak ortak iş mantıkları farklı kanallara sunulamayacaktır.

> BLOB AntiPattern olarak da anılır. AntiPatterns kitabının konuya ilişkin örneğinde PowerBuilder ile tasarlanan bir GUI ele alınmıştır.

İstisnai Durum:

Bazı istisnai durumlarda oluşmasına müsaade edilebilir. Örneğin miras olarak alınan çok eski sistemlerin sarmalanarak (Bir C++ kütüpanesinin.Net tarafında Wrap edilmesi olarak düşünülebilir) daha kolay erişilebilmesi ve yeni nesil ortamlara basit bir katman üzerinden sunulması gerektiği durumlarda göz önüne alınabilir.

Örneğin bir donamımın (Fax makinesi gibi) driver yazılımını.Net veya Java tabanlı bir uygulamada rahat kullanabilmek için onu sarmallayan kütüphaneler God Object olsalar dahi izolasyonu sağladıklarından tercih edilebilirler.

## Magic PushButton – Software Design

> Soyutlama kullanmadan arayüz üzerinde doğrudan uygulama mantığı kodlamak.

[![anti_15](/assets/images/2014/anti_15_thumb.png)](/assets/images/2014/anti_15.png) Bu desen GUI (Graphical User Interface) tipindeki uygulamalarda daha fazla ortaya çıkar. Arayüz tarafı ile iş mantıkları genellikle buton gibi bir kontroller arkasına gömülür.

Arayüz kontrollerine ait doğrulama işlemleri gibi operasyonlar butona basılmadan önce, çalıştırılması gereken iş mantıkları ise butona basıldıktan sonra devreye giren olay metodlarında ele alınır.

Doğal olarak iş mantıkları soyutlanmamış olacağından arayüz ile sıkı sıkıya bağlı hale gelir ve ilgili iş birimleri farklı yerlerde kullanılmak istendiğinde işler zorlaşır. Kodun okunabilirliği de ortadan kalkar.

İstisnai Durum:

Sonuç

Yazılım sektöründe 10 yıl ve üzerinde yer alan bireylerin yukarıdaki maddeleri okuduğunda benzer senaryolar ile karşılaşmış ve hatta içerisinde aktif olarak rol oynamış olabileceklerini düşünüyorum. Yazılım geliştirme süreçleri, teknikleri, mimarileri ve daha pek çok enstürman inanılmaz bir hızla geliştiğinden AntiPattern vakalarına da kolayca düşülebilmekte. Bu yüzden bilinçlenmekte yarar var. Daha pek çok AntiPattern olduğunu da belirtmek isterim. Bunları mutlaka ilgili kaynaklardan okumalı ve örnek vakalarına göz atarak kaçınmaya çalışmalısınız. Aslında AntiPatterns konusunun Üniversiteler de müfredata dahil edilmesi gerektiğine inananlardanım. Özellikle de Yüksek Lisans programlarında.

Bir başka etkinlikte görüşmek üzere hepinize mutlu günler dilerim.