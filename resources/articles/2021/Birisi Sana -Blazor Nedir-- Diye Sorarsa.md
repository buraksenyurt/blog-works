---
title: "Birisi Sana \"Blazor Nedir?\" Diye Sorarsa"
pubDate: 2021-05-07 22:04:00
categories:
  - Blazor
tags:
  - Blazor
  - .NET
  - ASP.NET
  - ASP.NET Core
  - ADO.NET
  - Entity Framework
  - NoSQL
  - WCF
  - Workflow Foundation
  - WPF
  - Windows Forms
  - Silverlight
  - XML
  - SOAP
  - REST
  - Web API
  - Web Service
  - HTTP
  - gRPC
  - Docker
  - Rust
  - JavaScript
  - TypeScript
  - Node.js
  - React
  - Vue
  - Concurrency
  - Performance
  - Microservices
  - Windows Phone
  - Visual Studio
  - GitHub
---

# Birisi Sana "Blazor Nedir?" Diye Sorarsa
![image.axd](images/image.axd)

Yeni bir on yılın arifesini çoktan geçtik ve bu on yıla girmeden önce Microsoft, milenyumun başında da yaptığı üzere önemli ürünlerin altına imzasını attı. Açık kaynak dünyasına hızlı bir girişten sonra yıllardır süregelen Mono projesi daha da anlam kazandı. Artık Silverlight, Windows Phone, Web Forms,.Net Remoting gibi kavramlardan neredeyse hiç söz etmiyoruz. Üstelik bazıları yıllar önce rafa kalktı. Rafa kalkanların, eskiyenlerin bıraktığı tecrübe yeni nesil ürünlerin başarısını artırdı. Unity ile platform bağımsız oyunlar, Xamarin ile macOS ve linux ayırt etmeksizin çalışan kodlar vs derken.Net Core hayatımıza girerek büyük sükse yaptı.

Dahası da var. 2017'de başlatılan ve standart haline gelen [WASM (Web Assembly)](https://webassembly.org/) Microsoft cephesinin gözünden kaçmadı. 2018 yılında deneysel bir çalışma olarak başlayan Blazor kısa sürede evrimleşti ve şu anda yatırım yapılması gereken bir konu haline geldi (Örneğin Asp.Net Web Forms tabanlı ürünlerinizi modernize etmek istiyorsanız) Ancak ortada önemli bir sorun var. Onu bir arkadaşına nasıl anlatırsın? (Photo by [Museums Victoria](https://unsplash.com/@museumsvictoria?utm_source=unsplash&utm_medium=referral&utm_content=creditCopyText) on [Unsplash](https://unsplash.com/s/photos/history?utm_source=unsplash&utm_medium=referral&utm_content=creditCopyText))

Ben 1995 yılından beri kodlama yapıyorum. Microsoft'un tüm.Net sürümlerinde geliştirme yapma fırsatı buldum. C# dilinin duyrulduğu zamanlardaki.Net Framework çatısının günümüzdeki halini alıncaya kadar geçirdiği evrimi gözlemleme fırsatı bulan şanslı programcılardanım. Şöyle bir durup geriye baktığımda beni en çok zorlayan konunun dağıtık sistemlerde kodlama yaparken kullandığım.Net Remoting olduğunu düşünüyorum (90ların [dll hell sendromu](https://ieeexplore.ieee.org/document/8509170)nu düşünmezsek) Sanıyorum günümüz.Net programcılarının bir çoğu onu duymamıştır.

Remoting tarafındaki zorlukları sırtını SOAP (Simple Object Access Protocol) standardına dayayan XML Web Service'ler büyük ölçüde kapatıyordu. Lakin yüksek performans, TCP bazlı çalışma imkanları, küçük paket boyutları gibi bazı önemli detaylar ortaya çıkınca.Net Remoting'e başvurmadan da olmuyordu. Sonrasında Windows Communication Foundation duyuruldu. Servis tabanlı geliştirme tek bir çalışma modeline indirgendi ve bu bana göre devrimsel bir dönüşümdü (Microsoft bu tip yapısal birleşimleri sıklıkla yapar. Bakınız Unified.Net) İşi gücü bırakıp onu öğrenmeye, ürünlerde kullanmaya başladık. Neredeyse her tür servis iletişimini destekliyor ve bunu oldukça kolaylaştırıyordu..Net Remoting'e göre öğrenilmesi de çok daha kolaydı, her türlü WS-I profilini destekliyordu. SOA'cıların gözdesi haline gelmekteydi. Üstelik Workflow Foundation ile birlikte ele alındığında hafif siklet iş akış yönetim mekanizmalarını da kurgulayabiliyordunuz. Bizzat çalıştığım bir kurumun iş akış şemalarını Workflow Foundaction, WCF ve Visual Studio genişletmeleri yardımıyla ürünleştirdiğine şahidim.

Fakat web ve mobil dünyasındaki gelişmeler daha zengin kullanıcı deneyimine sahip istemci uygulamalar istiyordu. Öyle bütün bir sayfayı sürekli kullanıcı ve ana makine arasında dolaştırmak yerine istemciye inip orada çalışan ve sunucu ile kısmi haberleşen zengin arabirimli uygulamalar bekleniyordu. Uygulamaların mobil sistemlerde çevrimdışı çalışabilir olması elbette büyük bir avantajdı, lakin senkronizasyon yeni bir problem olarak karşımızdaydı. Ado.Net'in bağlantısız (Disconnected) çalışan versiyonunun boy ölçüşebileceği bir şey değildi. Web ve mobil dünyasının dağıtım stratejisindeki cazibesi masaüstü uygulamaları arka plana atıyordu. Microsoft, ClickOnce ile uygulama dağıtımını kolaylaştıran merkezileşmiş bir strateji sunmuş olsa da bu Windows odaklıydı. Bu nedenle platform bağımsızlık adına tarayıcı tarafında plug-in destekli çalışma modelini göz önüne almış ve Flash'e rakip olacağını düşündüğü bir politika izlemeyi ihmal etmemişti. Doksanların sonları ve milenyumun başındaki ActiveX objelerinin kullanımı artık demode olduğundan farklı bir ürüne ihtiyacı vardı. Karşımıza Silverlight çıktı. Heyecan veriyordu. Oturduk bu kez Silverlight'ı da haldır haldır öğrenmeye çalıştık. İstemci tarafında Silverlight için gerekli çalışma zamanı olduğu sürece işler yolunda gidiyordu. Bu arada masaüstü tarafı Windows Presentation Foundation çatısı ile oldukça zenginleşmiş ve Windows işletim sisteminin sonraki sürümlerinde kullanıcı deneyimi yüksek ürünler geliştirilebilir olmuştu. Yine de tarayıcılar uygulamaların kalesi gibiydi.

Nitekim Javascript kodlarını her tür platform tarayıcısında çalıştıran Virtual Machine'in gücü ve avantajları bir türlü yakalanamıyordu. Sunucu tarafı ile haberleşmenin zengin bir yolu iyi bir çözüm olabilir miydi? Portföyümüze Rich Internet Application Services (RIA Services) eklendi. Onu HTTP metodları ile sorgulanabilir verileri baz alan Data Service'ler izledi. Silverlight gibi istemciler bu servisler yardımıyla.Net Framework'ün zengin imkanlarına erişebilirdi. Yine de ters giden şeyler vardı ve dikiş bir türlü tutmuyordu. Web dünyası servis iletişiminde yeni yeni şeyler denerken Silverlight unutulmaya başlandı. İstemci ve sunucu arasındaki iletişimin parça parça yapılması için kullanılan Ajax gibi çatılar bile daha az kullanılır oldu.

Derken REST (Representational State Transfer) stil servisler girdi hayatımıza. WCF ilk etapta bunu genişletme kütüphaneleri ile birlikte desteklemeye çalıştı, sonrasında dahili olarak bünyesine kattı. Artık kimse Web Servislerden ya da.Net Remoting ile tasarlanmış dağıtık bileşenlerden, Silverlight ile geliştirilmiş plug-in destekli istemcilerden, Windows Presentation Foundation ya da Workflow Foundation'dan pek bahsetmiyordu. REST hızla evrimleşirken Web API çözümleri yaygınlaşmaya başladı. İstemci ve sunucu iletişiminde anlık gelişmeleri yakalamak için Ajax'tan ziyade soket haberleşme öne çıktı ve SignalR devreye girdi.

Dünyada o kadar çok servis yazılmaya başlanmıştı ki performans, yüksek cevap verme süreleri, kolay ölçeklenebilirlik, hatalar sonrası çabucak toparlanabilmek doğal olarak öncelikli aranan kriterler haline gelmişti. Nitekim basit bir sayfanın içinden bile parça parça onlarca servis çağrısı eş zamanlı olarak yürütülüyordu. Video izleme ve müzik dinleme platformları bununla nasıl başa çıkacaktı? Dağıtık mimarinin çakıl taşlı patikalarında çıplak ayakla yürümeyi hangi mimar isterdi. Paket boyutlarını küçültmek lazımdı belki de.

Çözümler pek çok sefer olduğu gibi açık kaynak dünyasından yayılıyordu. Sektörün öncüleri yeni yeni ürünler ile önümüze birçok şey bırakmaya devam etti. gRPC onlardan biriydi. Çok daha yüksek performanslı bir servis haberleşmesinin yolunu açmıştı. Onu ilk denediğim zamanlarda proto nesnesinin bir örneğini istemci tarafına da koyunca,.Net Remoting zamanlarında yaptığımıza benzer Marshall by Reference/Value yaklaşımları gelmişti aklıma.

Servis dünyasındaki gelişmeler hızla devam ederken, mobil dünyasının inanılmaz yükselişi ile karşımıza farklı farklı geliştirme modelleri çıktı. Progressive Web App ve Single Page Application yaklaşımları gümbür gümbür yayılıyordu. Elimizden düşürmediğimiz telefonlardaki sosyal medya hareketlerimiz öylesine arttı ki, yazılım firması olmayan ürün sahipleri yeni diller ve platformlar çıkarmaya başladı. React, Vue, Go, Docker vs bu şekilde dünyamıza girdi. Microsoft bu alanda da söz sahibiydi elbette ve Angular ile SPA dünyasını desteklediğini açıkça göstermişti. Fakat Microsoft'un yapması gereken çok daha önemli bir şey vardı. Her şeyden önce bu değerli.Net platformunun ve o güzel C# dilinin Linux, macOS ayırt etmeksizin prüzsüzce çalışabilir olması gerekiyordu. Ücretsiz, herkesin seveceği türden IDE'ler ile, kolayca entegre edilebilen paketlerle. Bunun için şüphesiz ki açık kaynak dünyasının gücünü arkasına almalıydı. Xamarin daha da ciddiye alınmalıydı. İşte bu şekilde yeni düzene geçildi.

Velhasıl herkesin ortak problemleri hep aynı konular üzerinde yoğunlaşıyordu. Kullanıcı sayıları fazla, istekleri sınırsızdı. Mobil uygulamalar geliştirme platformlarını zorluyordu. Performans için bulut çözümler ucuz görünse de pahalıya patlıyordu. Ağ trafiği her zaman sıkıntılı bir konuydu ve parçala yönet şeklindeki dağıtık yaklaşım mimarilerini uygulama ve öğrenme maliyetleri de az değildi. Metodolojiler değişmekte çevik olunmaktaydı. Javascript, Node ile sunucu tarafını etkili kullanarak öne geçiyor ama yine de eş zamanlı hareketlerin çoğalması, Go ve Rust gibi öğrenilmesi zorlu dillerle bir takım çözümler geliştirmeyi mecbur kılıyordu. Sunucular yetersiz gelmekte dünyanın birçok köşesine yeni veri merkezleri açılmaktaydı.

Bu uzun ve kronolojik sıralamaya yer yer uymayan kurguda sizlere zihnimi açmaya çalıştım. Hikayede değinmediğim, unuttuğum birçok kavram da oldu (Mesela Javascript'te zorlanıyoruz diye bize verilen Typescript) Ve sanıyorum ki hiç Windows Forms demedim. Halbuki çok önemli bir detaydı benim gibi yıllanmış programcıların hayatında. Her şeyden önce dünyanın belki de en çok kullanılan işletim sistemi üzerinde pürüzsüz çalışırdı. Kullanıcı dostu formlar geliştirmek için Delphi doğasından kopup gelen Anders'in ekibinin zengin bileşenlerini sürükleyip bırakmak yeterliydi. Çevrimdışı çalışabilmek doğasında vardı. Performansı, yüklendiği makinenin gücüne bağlıydı ve dağıtımı zordu belki ama pekala sunucular ile de konuşabiliyordu. Bazen kapalı bir ağ sisteminde bazen internete açık bir odadaki PC'de gül gibi yaşıyordu.

Ne var ki HTML ve Web standartları o kadar başarılıydı ki daha doksanlı yılların sonlarından itibaren Windows Forms'un yüzüne pek de bakmayışımızı haklı çıkartıyorlardı. Düşünün bir; çoğumuz ofis uygulamalarını web tabanlı tarayıcılardan kullanıyoruz. Bulut servis sağlayıcılarının yine tarayıcılarında çalışan IDE'lerinde kodlama yapıyoruz. Bir PDF dosyasını bile tarayıcıda açıp yazıcı ile bastırabiliyoruz. Kocaman ekranlarda açtığımız web sayfalarını, minik mobil ekranlarda yine aynı tasarım deneyimini kaybetmeden gezebiliyoruz. Web'in nimetlerini düşündüğümüzde bu muazzam bir şey elbette.

Peki ya Blazor! Tarayıcıda çevrimdışı çalışabildiği de düşünülecek olursa o günlere dönüş için geliştirilmiş olabilir mi? Yoksa Silverlight'ın yeni bir versiyonu mu duruyor karşımızda? Kısa sürecek bir hayal mi yoksa? Aslında hiçbiri değil. Fikir olarak benzerlikler olsa da bu kez Microsoft hedefi tam on ikiden vurdu diye düşünüyorum (Umarım on yıl sonra bu yazıya uğrayıp, "Vuramadı" diye not almam)

Benim de mutlak suretle öğrenmem gereken bir uygulama çatısı Blazor. Sahiplenmek değil ama en azından farkına varmam gereken bir çözüm. Onu öğrenmek için birkaç kitaba, bir Pluralsight eğitimine ve bazı dergi yazılarına bakıyorum. Kendimce notlar da alıyorum. Aşağıdaki mektubu yazarken kullandığım kaynakları yazının sonunda bulabilirsiniz. Bilgi vermesi dileğiyle.

## Kimsin Sen Blazor?

Sevgili yazılım sevdalısı merhaba, Ben Blazor.

2018 yılında Microsoft'un uzak diyarlardaki bir ofisinde dünyaya geldim. Amacım Angular, React ve Vue gibi Single Page Application tabanlı programlar geliştirmeni kolaylaştırmak. Bunu yaparken Asp.Net Core Web çatısının nimetlerini de sana sunuyorum. Üstelik bunu yıllardır kullandığın C# ve Razor bileşenleri ile gerçekleştirebilirsin. Yanına azcık HTML ve CSS de koyabilirsin. İster Windowsçu ol ister Xamarinci, ister Web Formscu ol ister MVCci fark etmez;) Beni kolayca öğrenebilirsin. Daha da güzel bir şey söyleyeyim mi? Benimle birlikte geliştirdiğin uygulamaları tarayıcı üstünde çevrimdışı çalışacak şekilde modelleyebilirsin (Blazor WebAssembly)

Ama sana sunucu üstünden çalışan klasik bir model de sunuyorum (Blazor Server) Hangisini tercih edersen artık. Ayrıca arkadaşım Xamarin ile hibrid çözümler için de yardımcı olurum. Mobil dünyasını unutmuş değilim. Tarayıcı üstünde çalışan parçalarım için Javascript kullanmana gerek yok. Gerçekten yok! Sadece C# dilini kullanarak istediğin çözümü geliştirebilirsin. Yine de olur ya Javascript paketleri ile konuşman gerekir, o zaman Javascript Interoperability (JS Interop) isimli bir araç da sunuyorum..Net içinden Unmanaged bir kod parçasını (mesela bir Win32 sistem fonksiyonunu) çağırmak gibi bir şey aslında.

> Benimle pek çok türde program geliştirebilirsin. Tetris, Astreoid, Diablo, Flappy Bird gibi oyunlar, içerik yönetim sistemleri (CMS-Content Management System), IoT (Internet of Things) sürücüleri, Electron ile hibrid çözümler, mobil uygulamalar, elektronik ticaret siteleri, ofis programları, kod yazma araçları ve daha neler neler. Daha fazla detay ve örnek kod için [şuradaki repoya uğra](https://github.com/AdrienTorris/awesome-blazor)yabilirsin. Hatta istersen o repodaki uygulamalara kolayca ulaşabileceğin ve yine Blazor ile yazılmış bir tarayıcı da kullanbilirsin ki [ona da şu adresten ulaşabilirsin](https://jsakamoto.github.io/awesome-blazor-browser/).
> ![image.axd](images/image.axd)

Benim en önemli yapı taşlarımdan birisi de Razor bileşenleri (component). HTML, C# ve CSS üçlemesini kullanabileceğin Razor bileşenleri sayfandaki bir parça, sayfanın kendisi, bir dialog penceresi veya bir form olabilir. MVC ve Razor Pages için tasarladığın bileşenler varsa onları benimle de kullanabilirsin. Şimdilik hazır bileşen setim çok güçlü olmayabilir ama çevreden birçok firmanın bu alanda sunduğu paketler mevcut. Telerik ve DevExpress onlardan sadece ikisi. Bu firmaların Asp.Net ve Windows Form kontrollerini onlarca yıl kullandın. Tecrübelerini biliyorsun. Şimdi sana sunduğum geliştirme modellerimi anlatmak istiyorum.

## Blazor WebAssembly

Benim en çok kıskandıkları özelliğim bu. Ona Client-Side Blazor dedikleri de oluyor. Amacı Javascript yazmana gerek kalmadan C# kodlarının tarayıcıda doğrudan çalıştırılmasını sağlamak. Bu Typescript kodlarının istemci için Javascript'e dönüştürülmesinden farklı bir şey anlıyorsun değil mi? Aslında yazdığın uygulamanın çalışması için gerekli ne kadar DLL ve WASM-based kütüphane varsa, çalışma zamanı ile birlikte istemciye indiriyorum. Çalışma zamanı derken demek istediğim WASM tabanlı.Net Runtime'ı yollamak. Sonrasında uygulaman istemci üzerinde rahatça koşuyor. Sunduğum WASM-based.NET çalışma zamanı WASM standartları üzerine tasarlandı. Bu yüzden WASM'ı destekleyen Edge, Safari, Firefox, Chrome gibi bilinen tarayıcıların tamamında çalışıyor. Ortak standart sonuçta. Büyük babam Silverlight'tan farklı olarak küresel bir standart üzerinde koştuğumdan istemciye bana özel bir plug-in kurmana da gerek yok. Küresel standartlara bağlı kalmak her zaman iyidir.

### Kötü Huylarım

Her ne kadar seni mutlu eden şeyler söylesem de bazı kötü huylarım olduğunu da belirtmek isterim.

- Mesela benim bu modelimi Debug etmek o kadar kolay olmuyor. En azından abim Blazor Server'a göre.
- Plug-In gerekmiyor belki ama WASM destekleyen bir tarayıcı olması da şart. Belki bildiğin bütün tarayıcılar WASM'ı destekliyor ama Javascript çalışma zamanına göre daha büyük bir motor bloğuna ihtiyacım var. Bu da hafifsiklet tarayıcılarda benim çalışmamı çok zorlaştırıyor. IoT cihazlar belki sana küsebilirler.
- Yazdığın kodların bulunduğu DLL'ler istemci tarafına iniyor ya. Heh işte ona dikkat et. Şeytanın işi olmaz kod içerisinde unuttuğun bazı hassas verileri açığa çıkabilir. O yüzden paketlerde ne kodladığına dikkat et. Gerekirse onları sunucu tarafında tut.
- Çok doğal olarak istemcide çalışabilmenin de bir maliyeti var. Hamama giren terler:) İhtiyacın olan kütüphaneler uygulamanın ilk talebinde yükleme süresini uzatabilir. Boyutları itibariyle bu yükleme zaman da alabilir. O yüzden önbelleğe alma stratejilerini göz önünde bulundurmanı öneririm.

Bunların haricinde sana iyi haberlerim de var;)

### İyi Huylarım

En güzel haberi baştan söyleyeyim.

- Uygulamanın istemcide çalışması için Javascript yazmana gerek yok. Çok iyi biliyorsun ki Node.js çıkınca Javascript ile sadece istemci taraflı değil sunucu taraflı kodlamayı da aynı dille yapabilme şansı elde etmiştin..Net tarafında ise sunucu bazlı kodlama öne çıkıyor ve istemci için yine Javascript veya Typescript'e başvurman gerekiyor. Artık bu modelimi kullanarak yazdığın C# kodlarının derlenmiş hallerini istemcide doğrudan çalıştırabilirsin.
- Performans olarak da iyi sonuçlar veren bir model bu. Sunucu tarafında yaşanabilecek ölçeklendirme veya geç cevaplama gibi durumlar pek de söz konusu değil. Sunucu tarafı bağımlılığı ise aslında yok gibi bir şey. Yine de çok istersen elin kolun bağlı değil. Pekala Web API'lerle veya farklı dış dünya servisleri ile iletişim kurabilirsin.
- Performans demişken bu modelde yazdıkların tarayıcının tam da istediği ara dile önceden derleniyor. Tarayıcının doğrudan derlenmiş kodlarla çalışma şansı oluyor. Basit uygulamalarda değil ama örneğin oyun geliştirirken, etkileşimi yüksek eğitim programları hazırlarken bu çok işine yarayabilir.
- Modern uygulama dünyasının bilinen yıldızı Progressive Web App için de tam destek sunuyorum. Yani Native uygulama deneyimini bu modelle sunabilirsin.
- Üstelik bu modelde geliştirdiğin bir uygulamayı istemci tarafına indirdikten sonra internete ihtiyaç yok. Tamamen çevrimdışı çalışabiliyorum. Mariana çukurunda bile çalışırım.
- Bu modelde yazdığın ve çeşitli iş kurallarını içeren bir kütüphaneyi alıp abim Blazor Server modelinde de kullanabilirsin. Her ikisi de aynı bitlere derlenmiş versiyonlar ile çalışacaklardır. Dolayısıyla Blazor WebAssembly olarak geliştirdiğin bir uygulamayı basit hamlelerle Blazor Server modeline dönüştürmen mümkün.

Bu modelime ait çalışma şeklini aşağıdaki renkli içerikte bulabilirsin (Fark ettim de Document Object Model'in L harfini unutmuşum. Pardon)

![image.axd](images/image.axd)

Şimdi sana sunduğum diğer geliştirme modelini anlatayım.

## Blazor Server

Bu modelime Server-Side Blazor dedikleri de oluyor. Aslında Client-Based'den önce.Net Core döneminde ortaya çıktı. Tahmin edeceğin üzere uygulamanın ana kodları sunucu tarafında işletiliyor. Dolayısıyla sana Asp.Net Core ortamının nimetlerinden yararlanma fırsatı sunuyorum. İstersen.Net Standart kütüphanelerinden de faydalanabilirsin. İstemci tarafına yolladığım şey ise DOM (Document Object Model)'in kendisi. Bunun içinse Razor bileşenlerinin sunucu tarafında render edilmiş hallerini kullanıyorum. Tahmin ediyorum ki aklına takılan bir soru da var. İstemciye giden bileşenler ile sunucu kodlarını nasıl haberleştireceksin, değil mi? Bunun için sana DOM ve uygulama tarafındaki farkları takip etme stratejisi üzerine kurgulanmış SignalR enstrümanını sunuyorum.

### İyi Huylarım

Şimdi bu modelin sana sunduğu güzelliklerden bahsedeyim.

- Tekrarı gibi olacak ama, bu modelde de tek satır Javascript yazmana gerek yok. Üstelik küçük kardeşim Blazor WebAssembly ile aynı sözdizimini kullanan bileşenlerim var (Razor Components) Teşekkürler Razor!
- Uygulama kodun sunucu üstünde güvenli bir alanda duruyor olacak. Zengin hosting seçeneklerin var. Web API servisleri ile konuşma, bulut servis nitemlerinden yararlanma, veritabanı gibi diğer bileşen bağımlılıklarını kullanma gibi imkanlardan bahsediyorum. Büyük ihtimalle de bulut bilişim servislerini tercih edebilirsin ki Azure senin için ideal bir konumlandırma olacaktır.
- İstemciye sadece Render edilmiş bir şeyler gönderiyorum ve tüm kod sunucuda çalıştığı için uzun indirme süreleri, büyük dosyalar veya geç çalışma zamanı açılışları yok.
- Bir yazılımcının vazgeçemediği enstrümanlardan olan debug etme kabiliyetlerinde oldukça cömertim.
- Son olarak ne iş olsa yaparım ve her tarayıcıda çalışırım demek istiyorum.

### Kötü Huylarım

Sana bu modelin birçok iyi özelliğini saydım ama kötü yanları da yok değil.

- SignalR ile yakın ilişki içerisinde olduğumu ifade edeyim. Zaman zaman çok fazla konuşuruz. Mesela her sayfa örneklendiğinde ayrı bir SignalR bağlantısı tesis ederim. Takibi, bakımı ve ölçeklenebilirliği zordur. Azure tarafındaki SignalR yapısını kullanarak bu sorunu aşabilirsin ama bulut tabanlı dünyaya geçiş yapman gerekir ki orası da esasında ayrı bir uzmanlıktır.
- Arabirimle etkileşim o kadar fazladırki ağ trafiği bazen İstanbul iş çıkış saati trafiğini aratmaz. Belki uygulamanın dağıtıldığı yere göre uygun sunucu konumlandırmaları yapabilirsin (Yani Avrupa kıtasından gelenler Almanya'daki bir sunucuya yönlensin, Amerika kıtasındakiler Kanada'daki bir sunucuya yönlensin tadında) ama bu da ne demek biliyorsun, bolca kiralama bedeli.
- Bir diğer sorunumsa.Net çalışma zamanına olan bağımlılığım. Kardeşim Blazor WebAssembly gibi çalışacağım tarayıcıya kendi runtime motorumu indirip işe başlayamıyorum. Sunucuda benim için bir.Net çalışma zamanı kurulu olmak durumunda.
- Ne yazık ki çevrimdışı çalışma desteğim yok denecek kadar az. Hatta sürekli çevrim içi olmayı beklerim ki SignalR alt yapım sorunsuz çalışabilsin.
- Sunucu çökerse ne mi olur? Düşünmek bile istemiyorum.

Çevremdeki dostlarım bu modeli genellikle aşağıdaki çizimle hatırlarlar. Pek çok kaynakta tıpkısını göreceksin (Document Object Model'i doğru yazmışım. Enteresan!)

![image.axd](images/image.axd)

> Blazor WebAssembly ve Server modelleri genellikle performans açısından sıklıkla kıyaslanırlar. Bu konuda [Telerik'ten David Grace'in güncel araştırma yazısı](https://www.telerik.com/blogs/how-blazor-performs-against-other-frameworks)nı okumanı tavsiye ederim.

Gördüğün gibi desteklediğim iki modelin birbirlerine göre avantaj ve dezavantajları var. Duruma göre uygun olan modeli tercih etmek gerekiyor. Ancak bu işe ilk kez başlıyorsan ve Blazor'a merhaba demek istiyorsan kuracağın kulübe katılacak arkadaşlarının herhangi bir zaman diliminde gördükleri bir kitabın fotoğrafını koyup yorumlayabildikleri, puan verebildiği bir sistemi aşağıdaki topolojiye göre oluşturmayı deneyebilirsin.

![image.axd](images/image.axd)

Büyük resme baktığında şunları anlaman önemli. Çözümde her iki Blazor modeli de kullanılıyor. İstemci tarafında çevrimdışı olarak çalışabilen bir uygulaman var. Okumakta olduğun kitabın fotoğrafını çekebilir, hakkında bir şeyler yazabilir ve çevrimiçi olduğunda da Web API üstünden HTTP Post komutu ile bu bilgileri Backend uygulamasına gönderebilirsin. Backend tarafı bunu alıp istediğin Repository ortamında kalıcı olarak saklayacaktır. Entity Framework kullanırsan Repository bağımsız hareket etme şansın da var ama mecburi değilsin. İlişkisel bir veritabanı modeli seçebileceğin gibi NoSQL nimetlerinden yararlanabilirsin. Yorgun argın eve geldikten sonra da istersen bilgisayarındaki tarayıcıdan Web uygulamasını açar, arkadaşlarının eklediği kitapları Web API'den HTTP Get ile çekersin. Yapacağın veri odaklı güncellemeleri de HTTP Put ile yollayabilirsin. Hatta sen sayfada gezindiğin sırada bir arkadaşın Web Assembly uygulaması üstünden yeni bir kitap bilgisi eklerse SignalR mekanizması bu değişikliği Web üstünden bağlı tüm istemcilere de göndereceği için sen de değişiklikten anında haberdar olabileceksin. Ya da tam tersi sen sayfada gezindiğin sırada bir kitabın bilgisini değişitirirsen bu değişiklikten de diğer bağlı istemciler anında haberdar olacak. Gördüğün üzere SignalR sadece chat, anlık borsa veya stok hareketlerini takip etmek için kullanılan bir yapı değil.

## Benimle Geliştirme Yaparken

Bu arada aklıma gelen birkaç noktayı daha ifade etmek istiyorum. Ben MVC (Model View Controller), MVP (Model View Presenter) ve MVVM (Model View ViewModel) kalıplarından farklı olarak genelde Vertical Slices Architecture yaklaşımını kullanmanı öneriyorum. Yani kodunu fonksiyonlara göre gruplamaktan ziyade, özellik (Feature) bazlı gruplamanı öneriyorum. Yanlış anlama, onları kullanamazsın demiyorum ancak bildiğin üzere ben bileşenleri (Components) etkin kullanan SPA modelini öncelikli olarak benimsiyorum. Bileşen odaklı bu sade yaklaşımım nedeniyle Vertical Slice Architecture'ın aşağıdaki kurgusu kullanmak için çok ideal. Her kutunun bir bileşen olduğuna dikkat et lütfen.

![image.axd](images/image.axd)

## Sonuç Olarak

Esasında sonda anlattığım örnek senaryo farklı uygulama geliştirme çatıları veya programlama dilleri ile de yapılabiliyor. Buradaki avantaj çoğu zaman Javascript tarafından kaçan ve yıllardır.Net üzerinde geliştirme yapan birisinin C#'ın gücünü kullanmaya devam ettiği Blazor WebAssembly tarafı. Diğer yandan SignalR odaklı Server modelinin performansı da istemci-sunucu etkileşimi açısından bakıldığında diğer modellere göre daha iyi olabilir. Yine de kesin bir şey söylemek zor. Ancak şu bir gerçek ki büyük ihtimalle şirketinde kullanılan Asp.Net Web Forms kökenli bir uygulaman varsa onu modernize etmenin iyi yollarından birisi Blazor tarafına geçirmek olabilir. Bu arada Blazor WebAssembly alternatifi olan çalışmalar da var. Mesela [Lara](https://github.com/integrativesoft/lara) bunlardan birisi. Onu da kurcalaman da yarar var. Bir Nuget paketi uzağında.

Sözlerime burada son verirken önümüzdeki yılların sana ve tüm sevdiklerine sağlık getirmesini diliyorum.

## Kaynaklar

[A New Era of Productivity with Blazor](https://codemag.com/Article/1911052/A-New-Era-of-Productivity-with-Blazor), Ed Charbeneau,Code Magazine

[Asp.Net Core 5 for Beginners](https://www.amazon.com.tr/dp/1800567189/ref=cm_sw_em_r_mt_dp_6CMD17ZA5K1MDVQBWMRA?_encoding=UTF8&psc=1), Andreas Helland, Vincent Maverick Durano, Jeffrey Chilberto, Ed Price, Packt Publishing

[Software Architecture with C# 9.0 and.NET 5](https://www.amazon.com.tr/Software-Architecture-NET-Architecting-microservices/dp/1800566042/ref=sr_1_1?__mk_tr_TR=%C3%85M%C3%85%C5%BD%C3%95%C3%91&dchild=1&keywords=Software+Architecture+with+C%23+9.0+and+.NET+5&qid=1620331734&sr=8-1), Gabriel Baptista, Fancesco Abbruzzese, Packt Publishing

[An Atypical ASP.NET Core 5 Design Patterns Guide](https://www.amazon.com.tr/Atypical-ASP-NET-Design-Patterns-Guide/dp/1789346096/ref=sr_1_2?__mk_tr_TR=%C3%85M%C3%85%C5%BD%C3%95%C3%91&dchild=1&keywords=Software+Architecture+with+C%23+9.0+and+.NET+5&qid=1620331754&sr=8-2), Carl-Hugo Marcotte, Packt Publishing
