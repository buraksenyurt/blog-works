---
title: "Correlation Nedir? Yenir mi? İçilir mi?"
pubDate: 2010-02-01 00:25:00
categories:
  - WCF 4.0 Beta 2
  - WF 4.0 Beta 2
tags:
  - WCF 4.0 Beta 2
  - WF 4.0 Beta 2
  - WCF
  - Workflow Foundation
  - XML
---

# Correlation Nedir? Yenir mi? İçilir mi?
![blg126_Giris.jpg](images/blg126_Giris.jpg)

Merhaba Arkadaşlar,

Bazen bir kavramı yada konuyu anlamakta inanılmaz zorlandığınızı hatırlayın. Ne yaparsınız? Kimisi kendisini yemeğe verir. Kimisi hayat küsermişçesine bir köşeye çekilir. Kimisi kendiyle baş başa kalır ve çığlık çığlık haykırır. Kimisi de daha akıllı davranıp bir süre tatile çıkar veya anlayamadığı kavramla ilişkili herhangibir dökümanı bir süreliğine araştırmamaya, okumamaya karar verir. Neredeyse unuturcasına bir zaman koyar araya. Sonrasında ise aynı konuyu tekrar araştırmaya karar verir. İnanın başarılı olma şansı bir önceki denemeye göre çok daha yüksek olacaktır. Önemli olan noktalardan birisi, yılmadan bu iterasyona devam edebilmektir. Okudunuz, hala anlamadınız...Kısa bir ara daha...Sonra tekrar aynı konu ama mümkünse farklı kaynaklarla...

![Wink](images/smiley-wink.gif)

Bende bir süredir Workflow Service'lerde oldukça önemli olan konulardan birisi üzerinde araştırmalarımı tamamen durdurmuştum. Correlation. Çünkü; Matematikte "bağlılaşım/korelasyon", ekonomide "bağlanım", nükleer bilimlerde "bağlantı/eş ilişki", denizbilimde "kaçınım", tıpta "Aferent uyarıların gerekli cevabı oluşturmak üzere beyinin ilgili merkezinde birleşmesi" olarak çevirileri yer alan bu kavramın, Workflow Services içerisinde ne anlama geldiğini anlamak için epey bir süre tepinmem gerekmişti. Geçtiğimiz günlerde aynı konu üzerinde yeniden durmaya ve araştırmaya ve edindiğim bilgileri sizlere paylaşmaya karar verdim. İşte elde ettiğim sonuçlar;

Correlation kavramını mesajları bir arada gruplamanın bir yolu olarak düşünülebilir ilk etapta. Örneğin bir talep (Request) ve bu talebe karşılık gönderilen cevap (Reply) arasındaki ilişki Correlation olarak ifade edilmektedir. Özellikle WCF (Windows Communication Foundation) tarafında Session bazlı haberleşmelerde mesajlar arasında bir Correlation oluştuğu söylenebilir. Ancak Correlation'ın farklı bir yönü daha vardır. Bir servis örneği (Instance) ile bir oturum (Session) arasında da bağıntı kurulabilir. Yani bir SessionId değerine ait olaraktan hareket eden mesajların içeriğinde yer alan bazı veri parçalarının, Session ile alakalı bir servis örneği ile eşleştirilmesi mümkün olabilir. Bir başka deyişle aynı SessionId değeri altındaki mesajların her zaman için aynı servis örneğine ait olduğunun anlaşılmasında, SessionID=Service Instance ID eşitliğinin sağlanması da Correlation olarak ifade edilebilir. Bu ilişki çoğunlukla bilinçsiz (Implicit) olarak sağlanır. Yani, geliştiricinin çoğu zaman bir aksiyonda bulunmasına gerek yoktur. Aslında bu durumu aşağıdaki şekilde olduğu gibi canlandırabiliriz.

![blg126_Correlation.gif](images/blg126_Correlation.gif)

Ancak Workflow örneklerinde uzun zaman süren süreçlerin ele alınması da söz konusudur (Long Running Process). İstemcilerin, Workflow Service'ler ile olan haberleşmelerinde aradaki oturumu kapatıp ayrılmaları bu tip süreçlerde son derece yaygındır. Buna göre istemci ile servis arasındaki oturumun her an sonlanabilir olması önemli bir sorunu ortaya çıkarmaktadır; Correlation nasıl sağlanacak?

![Undecided](images/smiley-undecided.gif)

Bu amaçla Workflow Service'lerde Correlation'ın sağlanması için kullanılan çeşitli teknikler mevcuttur. Aslında burada da tam bir kavram kargaşası vardır. En güvenilir kaynaklardan birisi olarak ele alacağımız MSDN, Correlation'ı Content-Based ve Protocol-Based olmak üzere iki çeşide ayırmıştır. Protocol-Based Correlation'da kendi içerisinde Context ve Request-Reply isimli iki farklı Correlation tekniğini daha barındırmaktadır.

Content-Based Correlation'da service örneği (Instance) ile ilişkili olan veri (Map edilmiş veri olaraktan da düşünebiliriz) mesajın içeriğinde yer alır. Örneğin mesajın Header veya Body kısımlarında bulunabilir. Dolayısıyla Correlation'ı sağlayan arabirimlerin XML tabanlı olan bu veri içeriğini kontrol edebilmesi gerekmektedir. İşte bu noktada XPath gibi sorgu teknikleri devreye girmektedir. Protocol-Based Correlation ise iletişim mekanizmasını baz alır ve buna göre mesajlar arasında yada mesajlar ile doğru servis örneği arasında gerekli eşleştirmeyi sağlar.

Bu giriş yazımızda özellikle Content-Based Correlation üzerinde durabiliriz. En çok örneklenen model genellikle budur. Workflow Service'lerde servise gelen ve servisten giden mesajların içerdiği veri parçaları ile çalışma zamanındaki servis örnekleri arasında eşleştirme yapmak (yani Correlation'ı sağlamak) son derece kolaydır. Tüm mesajlaşma aktivitelerinin CorrelationInitializers isimli bir koleksiyonu bulunur. Bu koleksiyon içerisinde Key-Query çiftleri yer almaktadır. Tahmin edileceği üzere Key değerleri ile Query'lerin birbirlerinden ayrıştırılması sağlanır. En önemli nokta ise Query'dir. Query içerisinde yer alan XPath sorgusu ile Content içerisindeki veri işaret edilir. Tabi bir mesajlaşma aktivitesi, diğer bir mesajlaşma aktivitesinin başlattığı Correlation'u takip edebilmelidir. Bunun içinde CorrelationWith isimli özellikten yararlanılır. Bu bilgilere göre Correlation'ın bir şekilde başlatılması (Initialize) ve takip edilmesi (Follow) gerektiği anlaşılmaktadır. Başlatılan bir Correlation'ın takip edilmemesi halinde mesajlar ve servis örneği arasında bir eşleştirmenin yapılması söz konusu olmayacaktır. Elbette Workflow Foundation 4.0 içerisinde bir Correlation'ın başlatılmasını kolaylaştıran bir aktivitede gelmektedir. InitializeCorrelation bileşeni.

Sanırım şu ana kadar anlattıklarımız ile kafamızda Correlation'ın ne olduğuna dair bir fikir oluşmuştur. Tabi konuyu kavramak tek başına yeterli değildir. Pratiğe dökmemizde yarar vardır. Ancak şu an için bu konudaki araştırmaya ara verip tatile çıkmak niyetindeyim. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
