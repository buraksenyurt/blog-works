---
layout: post
title: ".Net Memory Management’ i Kavramak"
date: 2012-03-02 06:54:00 +0300
description: "Sanki C++ ile geliştirilmiş bir değişken tipiydi de, Release edilmesi unutulmuş ve bellek üzerinde bir şekilde ayakta kalmış bir programcıktı Smile Şimdi nereden çıktı bu Matrix, Merovingian diyeceksiniz. Konumuz .Net bellek yönetimi. Ama bu kez biraz daha farklı ve detaylı..."
categories:
  - dotnet-framework-4-0
tags:
  - dotnet-framework-4-0
  - xml
  - dotnet
  - http
  - threading
  - memory-management
  - performance
  - pointers
---
# .Net Memory Management’ i Kavramak
[![Merovingian200px](/assets/images/2012/Merovingian200px_thumb.png)](/assets/images/2012/Merovingian200px.png) Merhaba Arkadaşlar,

Matix! Ne filmdi ama değil mi? Özellikle yazılım tarafına hakim olan bizler için, filmin içerisindeki pek çok gönderi anlamlı birer mesaj haline gelmişti. İlk bölüm zaten efsanenin başlangıcı olma niteliğindeydi. İkinci bölümde işler daha da bir farklılaştı tabi. Örneğin, silinmeyen ve Matrix içerisinde kendini geliştirip küçük bir krallık yaratan Merovingian karakteri vardı. Bu sistem içerisinde yer alan ve süresi dolduktan sonra silinmesi gereken bir program iken, kaynağa (Source) geri dönmemişti.

Sanki C++ ile geliştirilmiş bir değişken tipiydi de, Release edilmesi unutulmuş ve bellek üzerinde bir şekilde ayakta kalmış bir programcıktı

![Smile](/assets/images/2012/smiley-smile.gif)

Şimdi nereden çıktı bu Matrix, Merovingian diyeceksiniz. Konumuz.Net bellek yönetimi. Ama bu kez biraz daha farklı ve detaylı.

.Net Framework’ ün bilindiği üzere en önemli özelliklerinden birisi de Managed Code adını verdiğimiz yaklaşımı destekliyor olmasıdır. Hatta özellikle bu yaklaşım üzerine kurulmuş bir alt yapı mimarisi sunduğunu ifade edebiliriz. Managed Code denildiğinde, üretilen kodun çalışma zamanında bir ortam tarafından kontrol atlında tutulduğu sonucuna varmamız yeterlidir..Net Framework’ ün içsel yapılarını göz önüne aldığımızda, çalışma zamanı yani Common Language Runtime, yürütülmekte olan Assembly’ lar ve ilişkili Module’ ler ile ilgili olarak bir çok yönetsel kontrol mekanizmasını devreye sokar. Örneğin Exception Handling, Code Access Security, Type Safety ve en önemlilerinden birisi olan Memory Management.

Özellikle C++ gibi programlama dilleri, sistemler üzerinde özgürce kod geliştirebilmemize olanak sağlamaktadır. Örneğin bellek üzerinde pointer gibi temel tipler yardımıyla her noktaya erişebilir ve nesnelerin yaşam döngülerini çok daha esnek bir biçimde ele alabiliriz. Tabi bu özgürlük, development’ ı biraz daha zorlaştırır. Pointer aritmetiği ile uğraşılmak zorunda kalınır ve bellek yönetimi güçleşir. Sonuç pek tabi, kaynağa geri dönmesi unutulan bir [Merovingian](http://en.wikipedia.org/wiki/Merovingian_%28The_Matrix%29) olup çıkar ki bunun doğal yansıması da genellikle Memory Leak ve kötü performans olur. Bu ve buna benzer bazı nedenlerden dolayı,.Net Framework daha ilk sürümünden itibaren, kodu kontrol altında tutmuş ve belleği yönetimini ağırlıklı olarak üzerine almıştır. Bize de belirli ölçülerde esnetmeler sunmuştur.

.Net ile geliştirme yapmaya veya onu öğrenmeye başlayan hemen her programcı aşağıdakine benzer bir şekil ile de mutlaka karşılaşır.

[![memmng_1](/assets/images/2012/memmng_1_thumb.png)](/assets/images/2012/memmng_1.png)

Bize öğretilen, bizim öğrendiğimiz ve hatta öğrettiğimiz haliyle,.Net Framework içerisinde veri türleri iki ana dala ayrılır. Belleğin Stack bölgesinde tutulan değer türleri (Value Types) ve belleğin Heap bölgesinde tutulan referans türleri (Reference Types). int, double, Point, DateTime gibi aslında Common Type System içerisinde birer struct ile ifade edilebilen tüm tipler değer türü iken, class gibi tipler de referans türleridir. Özellikle bunların kendi aralarındaki atamalarında bellek üzerindeki işleyiş şekilleri de çouğunlukla farklıdır. Aksi belirtilmediği ve müdahale edilmediği sürece referans türleri arası yapılan atamalar, aslında stack bölgesindeki adres işaretçilerinin çoğullanması ama heap üzerindeki aynı adres bölgesinin ifade edilmesidir. Değer türlerinde ise bu durum tam tersidir. Değerler stack bölgesinde atamalar sonrası kopyalanırlar.

Aslında bu bilgiler bizim için temel niteliği taşımaktadır. Dedik ya, CLR aslında çalışma zamanındaki bellek yönetimini de üstlenmektedir. O yüzden çoğumuz, “nasıl olsa belleği birileri yönetiyor, nesneleri de zamanı gelince temizliyor, ortalığı toparlıyor” diyerek temel olan başka bir konuyu da atlarız. Gerçekten de.Net Memory Management acaba nasıl çalışmaktadır? Eğer bunu merak ediyorsanız, yaptığım araştırmalar ve kendimce edindiğim fikirler ile konuyu sizlere aktarmaya çalışıyor olacağım. Dolayısıyla bundan sonrasını merak ediyorsanız okumaya devam edin

![Wink](/assets/images/2012/smiley-wink.gif)

Uygulamalarımızın çalışma zamanında ürettiği referans tiplerinin Garbage Collector tarafından ele alındığını biliyoruz aslında. Hatta GC, GCSettings gibi tipler yardımıyla ona bir ölçüde müdahale etme şansımız da bulunmakta. Teorik olarak Heap bellek bölgesindeki nesne örneklerinin yaşam döngüsünden, onların bellek üzerindeki fragmantasyonlarından ve elbetteki serbest bırakılmalarından sorumlu olduğunu özetleyebiliriz. Garbage Collector ilke olarak iki tip nesne ile ilgilenir.

[![memmng_2](/assets/images/2012/memmng_2_thumb.png)](/assets/images/2012/memmng_2.png)

Aslında bir.Net uygulaması process olarak belleğe açıldığında, Managed Heap üzerinde o process’e ait olacak şekilde iki farklı alan göz önüna alınır. Bunlardan birisi uygulamanın 83Kilobyte ve daha az büyüklükteki nesneleri içindir ki Small Object Heap (SOH) olarak adlandırılır. Boyutu 83Kb üzerinde olan nesneler içinse Large Object Heap (LOH) olarak adlandırılan başka bir heap bloğu göz önüne alınır.

> Tekil bir.net Process’ i Win32 platformunda çalıştırıldığında bellek üzerinden maksimum 2Gb’ a kadar yer kullanabilir.

Çok doğal olarak Garbage Collector söz konusu bu nesnelerin bellek üzerinde yerleştirilmeleri (allocate), fragmante edilmeleri (re-allocate) ve geri çağırılmaları (reclaim) vb işlemler sırasında kritik bir rol üstlenir. Şimdi ilk olarak SOH tarafına bir bakalım. SOH temel olarak zaman içerisinde nesne ömürlerini 3 farklı jenerasyonda oluşacak şekilde ele almaktadır (Generational yaklaşım).

> Generation 0, Generation 1 ve Generation 2.

SOH içerisinde yer alması gereken nesne örnekleri oluşturuldukça Generation 0 adı verilen süreçte sırasyıla yerleştirilmeye başlarlar (Burada C++ taki Linked List tarzındaki bellek açılımından farklı bir durum söz konusu). Çok doğal olarak zaman içerisinde bu bölgede yer alan bazı nesneler Dispose edilme aşamasına gelir. GC varsayılan davranış stratejisine göre Generation 0 dolana kadar bir aksiyonda bulunmaz. Generation 0 dolduğunda, Dispose sürecine girmesi gereken atıl nesneler toplanmaya ve halen yaşamakta olan canlı nesnelerde Generation 1 bölgesine kopyalanarak taşınmaya başlanırlar.

Generation 1 bölgesinin de çok doğal olarak bir kapasitesi vardır ve zaman içerisinde buradaki canlı nesnelerden bazıları yine Dispose sürecine girecektir. Dolayısıyla Generation 1 bölgesinin de dolması sonrası yine canlı nesnelerin bu kez Generation 2 bölgesine kopyalanması ve atıl nesnelerin Generation 1 den atılması söz konusu olacaktır.

Durumu kabaca bu şekilde düşünecek olursak aşağıdaki gibi bir zaman diagramını göz önüne almamız mümkün olabilir. Kabaca tabi

![Wink](/assets/images/2012/smiley-wink.gif)

[![memmng_3](/assets/images/2012/memmng_3_thumb.png)](/assets/images/2012/memmng_3.png)

Ancak, olay bu kadar da basit değildir. Aslında GC mekanizması ana uygulama Thread’ inden bağımsız olarak çalışan farklı bir Thread olarak düşünüldüğünde, söz konusu işlemleri concurrent olarak gerçekleştirmektedir. Özellikle Generation 0,1 ve 2 bölgeleri üzerinde her zaman şekilde olduğu gibi sıralı ve düzgün bir dizilim söz konusu olmayacaktır. Dolayısıyla kopyalama metoduna göre yapılan taşıma işlemleri sırasında, nesneler boş bulunan bellek bölgelerine atılırlar.

Diğer yandan kopyalama işlemleri sırasında oluşabilecek bir sorun da vardır. GC ayrı bir Thread üzerinden, bir alt Generation’ daki canlı nesneleri tespit ettikten sonra, bunları bir üst generation’ a kopyalar. Lakin alt Generation’ daki nesneler bu taşıma sırasında veya öncesinde halen daha ana veya farklı bir Thread tarafından kullanılıyor olabilirler. Hımmm

![Wink](/assets/images/2012/smiley-wink.gif)

İşte bu noktada GC şöyle bir yol izler. Thread’ ler arası güvenli bir nokta oluşturur (Safe Point) ve taşıma sırasında ilgili uygulama Thread’ lerinin tamamı durdurulur. Sonrasında ise kopyalanan tüm içeriği orjinal referansları ile eşleştirerek düzeni korur. Güzel bir trick öyle değil mi?

![Laughing](/assets/images/2012/smiley-laughing.gif)

Bir o kadar da karışık aslına bakarsanız. (Ben hala konu ile ilişkili kaynakları ve CLR via C#’ ın ilgili bölümlerini okuyarak pekiştirmeye çalışıyorum)

Generation 2 bölgesi aslında performans ölçümlerinde de ip ucu veren bir alan olarak düşünülmektedir. Bu bölgenin çok sık ve fazla şişerek dolması ileride programın bellek ile ilişkili sıkıntılar üretebileceğinin de bir işaretedir.

> [ANTS Memory Profiler](http://www.red-gate.com/products/dotnet-development/ants-memory-profiler/) gibi araçlar yardımıyla, uygulamalarımızın bellek üzerindeki ölçümlerini detaylamasına yapabiliriz. Tabi daha ucuz çözümler de var. CLR Performance Counter’ lar
>
> ![Wink](/assets/images/2012/smiley-wink.gif)

Gelelim LOH (Large Object Heap) bölgesine. 83KB üzeri olarak belirtilen Large Object’ lerin taşıma/kopylama maliyetleri tahmin edileceği üzere yüksektir. Bu sebepten dolayı SOH için uygulanan Generations tekniği yerine farklı bir yaklaşım kullanılır. Generation 2 parçasında Large Object nesnelerinin, ölen nesnelerden boşalan yerlere iliştirilmesi söz konusudur. Aslında aşağıdaki şekil ile durumu biraz olsun ifade edebiliriz.

[![memmng_4](/assets/images/2012/memmng_4_thumb.png)](/assets/images/2012/memmng_4.png)

Bir LO eklenmek istendiğinde Generation 2 kısmındaki ilk boş bölgeye açılması söz konusudur. Sonrasında sisteme dahil olacak diğer LO’ ler de Generation 2’ de boş olan yerlere serpiştirilirler. Tabi Generation 1 den gelen nesne örnekleri 83Kb’ den küçük olduklarından, yeni gelen 83Kb’ den büyük nesnelerin sığabilecekleri uygun yerlerinde Generation 2 üzerinde var olması gerekir.

Peki yoksa?

![Sealed](/assets/images/2012/smiley-sealed.gif)

Bu durumda uygulama daha fazla bellek alanının allocate edilmesi için işletim sisteminde bir talepte bulunacaktır. Hatta Heap alanının yetmediği durumlarda, fiziki disk bölgelerinden sanal olarak bu alanların karşılanması istenecektir. Eğer işletim sisteminden olumlu bir cevap alamazsa bu durumda GC’ nin Generation 2 içerisinde yapacağı de-allocate işlemlerinin yeteri kadar yer ayırması beklenecektir.

LO’ ler için uygulanan strateji, generations sistematiğine göre daha performanslıdır. Nitekim bellek üzerinde copy/move/re-refereance işlemleri yoktur. Ancak belleğin fragmente edilme noktalarında da bir dezavantaj oluşturacaktır. Tabi LOH ile SOH’ un bir arada çalıştığı da unutulmamalıdır. GC her iki tür için gerekli yönetsel işlemleri üstlenmektedir.

> İşte tüm bunlar göz önüne alındığında karmaşık olan ve bize aslına bakarsanız çok fazla sorun çıkartıp baş ağrısı yapmayan en basit uygulamalarımızın bile, şöyle bir bellek testinden geçirilerek ne yaptıklarının incelenmesinde yarar olduğu söylenebilir.

.Net Framework içerisinde sisteme gelen ve CLR’ ın bellek yönetimi odaklı olarak kullanılan pek çok performans ölçüm kriteri bulunmaktadır. Bytes in all heaps, time spent in GC, allocated bytes per sec vb…Bu tip kriterlere bakılarak geliştirdiğimiz.Net uygulamalarının bellek yönetimi açısından daha performanslı hale getirilmesi, istatistiki bilgilerinin çıkartılması, farklı ürün stratejilerinin belirlenmesi de söz konusu olabilir. Diğer yandan Garbage Collector’ un kod tarafından da ele alınması ve bazı kurallarının değiştirilmesi söz konusu olabilir. Özellikle performans ve heap’ in etkin kullanımı arasında fedakarlık yapılması gerektiği durumlarda (kısaca performance ve heap etkinliği arası trade-off diyelim) uygun modun seçilmesi sağlanabilir. Bu anlamda GC iki temel modu desteklemektedir. Workstation ve Server.

Workstation modu, kullanıcıya maksimum cevap verilebilirlik için tercih edilmekte olup Concurrent ve non-Concurrent çalışacak şekilde ele alınabilir. Varsayılan olarak Concurrent çalışma prensibi uygulanır. Buna göre GC mekanizması uygulama ile birlikte ayrı bir Thread üzerinden işlemlerini gerçekleştirir.

Server mode ağırlıklı olarak performans (Performance), ölçeklenebilirlik (Scalability) ve verimliliğin (throughput) ön plana çıktığı sunucu ortamlarında (Server Environment) göz önüne alınmaktadır. Bu modda, generation eşik değerleri ile bellekteki segment boyutları, Workstation Mode’ a göre çok daha yüksektir. Bu son derece doğaldır nitekim sunucuların bellek kapasiteleri, workstation’ lara göre daha fazldır

![Smile](/assets/images/2012/smiley-smile.gif)

Server Mode ile çalışmanın en önemli artısı ise paralel veya multi-thread olarak çalışabilmesidir. Buna göre SOH ve LOH bölgeleri n sayıda fiziki işlemci tarafından ele alınabilir (Tabi birbirlerini kesmeyecek şekilde)

> Bu söylenenlere göre Workstation’ ların da Server Mode’ da çalıştırılması düşünülebilir. Ancak bu modda kullanıcı cevap verilebilirliği ikinci plandadır. Çünkü tüm uygulama Thread’ leri, GC’ nin çalıştığı sürelerde Suspend moda geçecektir ki bu kullanıcının direk uygulama ile olan etkileşiminde eksi puandır.

Bir de GC’ nin tekrardan toplamasına gerek duyulmayacak şekilde kullanılabilen Weak Referance tipleri bulunmaktadır. WeakReference sınıfı bu noktada devreye girmektedir. Bu konuyu ilerleyen zamanlarda incelemeyi ve sizlerle paylaşmayı planlıyorum.

Yukarıda bahsettiklerimiz anlamında GC’ i kod tarafında ve config dosyası üzerinde ayarlayabilir ve hangi modda ne şekilde çalışacağına karar verebiliriz. Örneğin uygulamaya ait config dosyasına yapılacak aşağıdaki bildirim ile Server Mod üzerinde çalışılacağı ifade edilir.

```xml
<configuration> 
  <runtime> 
    <gcServer enabled="true" /> 
  </runtime> 
</configuration>
```

veya örneğin

```text
<configuration> 
  <runtime> 
    <gcServer enabled="false"/> 
    <gcConcurrent enabled="false"/> 
  </runtime> 
</configuration>
```

bildirimi ile Workstation modda ve non-concurrent olarak çalışılacağı ifade edilebilir.

Bunlara ek olarak GCLatencyMode adında önemli bir ayar daha sunulmaktadır. Bu ayara göre daha fazla nesnenin bellekten toplanması bir başka deyişle çok daha fazla alanın açılması sağlanabilir. Bilindiği üzere GC, çöpleri topladığı sırada çalışmakta olan diğer tüm Thread’ leri geçici olarak durdurur. Bu nedenle Latency (gecikme) ‘ nin kontrol altına alınması gereken uygulamalar söz konusu olabilir.

GCLatencyMode özelliği Batch, Interactive ve LowLatency olmak üzere 3 sabit değerden birisini alabilir. Batch mode genellikle bir arayüzü veya sunucu taraflı operasyonu olmayan uygulamalarda tercih edilmektedir. Arayüzü (UI) bulunan uygulamalarda Interactive mode seçilebilir. Bunun dışında bazı uygulamalar bilindiği üzere bellek üzerinde çok daha fazla harekette bulunurlar. Özellikle çok kısa sürelerde işlemlerin yapılması gerekmektedir. Örneğin animasyon işlemleri ile uğraşan uygulamalar buna örnek olarak verilebilir. Bu tip uygulamalarda zaman oldukça önemlidir. O nedenle LowLatency modda çalıştırılmaları sağlanabilir. Latency Mode ile ilişkili daha detaylı bilgiyi [MSDN adresinden](http://msdn.microsoft.com/en-us/library/bb384202.aspx) bulabilirsiniz.

Devam eden yazımızda LOH ve SOH kullanımları sırasında uygulamalarımıza ait bellek değerlerini nasıl ölçümlendirebileceğimizi aktarmaya çalışıyor olacağım. Şimdilik teoriyi pekiştirmemizde ve neyin ne olduğunu ayrıştırmamızda yarar var. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[![memmng_5](/assets/images/2012/memmng_5.png)](http://www.amazon.com/CLR-via-C-Jeffrey-Richter/dp/0735627045/ref=sr_1_1?ie=UTF8&qid=1330694365&sr=8-1) Bu arada CLR’ ın çalışma şeklini daha iyi ve derinlemesine öğrenmek isterseniz sizlere tavsiyem MS Press’ in Jeffrey Ritcher imzalı CLR via C# isimli kitabı olacaktır.

896 sayfalık bu kitap içerisinde elbetteki bulacağınız tek şek bellek yönetimi (Memory Managemet) değil. Ama yazdığımız temel C# kod parçalarının CLR (Common Language Runtime) tarafından nasıl ele alındığını görmek, CIL (Common Intermediate Language) seviyesine kadar inebilmek mümkün. Fiyatı biraz yüksek görünebilir ama bence elinizin altında olması gereken bir kaynaktır diye düşünüyor ve hatta bu konuda ısrar ediyorum.