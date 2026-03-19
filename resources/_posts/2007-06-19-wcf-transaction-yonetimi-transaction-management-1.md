---
layout: post
title: "WCF - Transaction Yonetimi (Transaction Management) - 1"
date: 2007-06-19 09:00:00 +0300
description: "Transaction (İşlem) yönetimi özellikle veritabanı kaynakları söz konusu olduğunda her sistemde büyük bir önem sahiptir. Basit olarak transaction bir veya daha çok işlem bütününü temsil eder. Bütünü oluşturan söz konusu işlem parçaları çoğunlukla birbirleriyle ilişkilidir ve hepsinin başarılı bir şekilde tamamlanabilmesi sonrasında transaction' ın başarılı olduğu söylenebilir. Bu doğal olarak işlem parçalarından herhangibirinin başarısız olması sonucunda transaction' ınında başarısız olması anlamına gelmektedir. Aşağıdaki şekilde bir transaction' ın süreç içerisinde sistemin belirli bir konumdan başka bir konuma geçişi sırasında üstlendiği rol ifade edilmeye çalışılmaktadır."
categories:
  - wcf
tags:
  - wcf
  - dotnet
  - ado-net
  - sql-server
  - oracle
  - web-service
  - http
  - performance
  - transactions
  - atomic-operations
---
# WCF - Transaction Yonetimi (Transaction Management) - 1
Değerli Okurlarım Merhabalar,

Transaction (İşlem) yönetimi özellikle veritabanı kaynakları söz konusu olduğunda her sistemde büyük bir önem sahiptir. Basit olarak transaction bir veya daha çok işlem bütününü temsil eder. Bütünü oluşturan söz konusu işlem parçaları çoğunlukla birbirleriyle ilişkilidir ve hepsinin başarılı bir şekilde tamamlanabilmesi sonrasında transaction'ın başarılı olduğu söylenebilir. Bu doğal olarak işlem parçalarından herhangibirinin başarısız olması sonucunda transaction'ınında başarısız olması anlamına gelmektedir. Aşağıdaki şekilde bir transaction'ın süreç içerisinde sistemin belirli bir konumdan başka bir konuma geçişi sırasında üstlendiği rol ifade edilmeye çalışılmaktadır.

![mk209_1.gif](/assets/images/2007/mk209_1.gif)

Buna göre sistemin belirli bir konumdan başka bir konuma geçmesi sırasında yapılacak işlemlerin tamamı bir bütün olarak ele alınmalıdır. Söz konusu bütün içerisinde meydana gelebilecek aksamalarda sistem ilk konumuna (Konum A) dönebilmelidir. Tam tersine tüm işlemler başarılı olduğunda sistem yeni konumuna (Konum B) geçebilir. Bu şekilde düşünüldüğüne transaction ACID (AtomicityConsistencyIsolationDurability) adı verilen standartları sağlamak durumundadır. Makalemizin konusu Windows Communication Foundation sistemi üzerinde transaction yönetiminin nasıl sağlanabileceğinin temellerini incelemektir. Yinede ACID ilkelerinden kısaca bahsetmek gerektiği en azından hatırlanmasında fayda olacağı kanısındayım.

Atomicity ilkesine göre bir transaction'ın başarılı olması, içerisinde yer alan tüm işlem parçalarının ayrı ayrı başarılı olması anlamına gelmektedir. Consistency ilkesi, yapılan işlemler sonucunda oluşan çıktıların tutarlılığı anlamına gelir. Her zaman verilen bir örnek tutarlılık ilkesini son derece güzel anlatmaktadır. Bankalar arası yapılan bir havale işlemi sırasında bir hesaptan çıkan miktar ne kadar ise diğer hesaba aktarılanında o kadar olması gerekir. Ne bir fazla ne de bir eksik olmamalıdır. İşte bu verinin tutarlılığını korumak olarak düşünülebilir. Isolation ilkesine göre bir transaction içerisindeki tüm işlem parçaları diğer transactionlardan izole olacak şekilde çalışmalıdır. Bu olmadığı takdirde tutarsızlıklar oluşabilir.

Elbette transaction'larda söz konusu olabilecek Dirty-Read, Phantom, Non-Repeatable Read gibi durumların hangisinin tercih edileceğine göre çeşitli izolasyon seviyeleri (Isolation Levels) pek çok veri tabanı sisteminde ele alınmaktadır. Bunlar.Net tarafındaki Transaction sınıflarıncada kullanılmakta ve IsolationLevel enum sabitleri ile işaret edilmektedir. Söz konusu seviyeler WCF içerisindede bir servis davranışı (Service Behavior) olarak belirtilebilmektedir. Söz konusu bildirim ServiceBehavior niteliğinin TransactionIsolationLevel özelliğinin değeri ile sabitlenmektedir.

> Temel Olarak Transaction'larda Düşünelecek Senaryolar;
> Phantom; başlatılan iki transaction olduğunu düşünelim. Bu transactionların birisi açık iken yeni bir veri girişi yapıp işlemi onaylıyor (Commit) olsun. Bu durumda halen daha açık olan diğer transaction aynı veri kümesini tekrardan talep ettiğinde daha önce kendisinde var olmayan yeni eklenmiş bir satır ile karşılaşacaktır. Bu satırlar hayalet (Phantom) olarak adlandırılır.
> Non-Repeatable Read; yine başlatılmış iki transaction olduğunu düşünelim. Bu sefer açık olan transaction'lardan birisi var olan veri kümesinde belirli bir satırda (satırlarda) güncelleme işlemi yapmış olsun. Sonrasında ise yapılan bu değişiklikleri onaylasın (Commit). Açık kalan diğer transaction'ın tekrardan aynı veri kümesini çektiğini düşünelim. Bu durumda açık olan transaction daha önce baktığı verilerin değiştiğini görecektir. Bu Non-Repeatable Read olarak adlandırılan bir durumdur.
> Dirty-Read; yine başlatılmış iki transaction olduğunu göz önüne alalım. Bunlardan birisi veri kümesine herhangibir satır eklemiş veya güncellemiş olsun. Ancak bu sırada diğer transaction'ın tekrardan veri çektiğini düşünelim. Burada dikkat edilmesi gereken durum şudur; iki transaction açık iken, herhangibiri commit edilmeden önce diğeri veri çekebilmektedirler. Dolayısıyla diğer transaction var olan ekleme ve değişiklikleri o an için görecektir. Ne varki bu senaryoda değişiklikleri yapan transaction işlemleri Commit etmez. Aksine geri alır (Rollback). Bu durumda daha önce tekrardan veri çeken açık transaction'da aslında geri alınmış güncellemeler ve eklemeler görülmektedir. Bu Dirty-Read durumu olarak adlandırılmaktadır.

Durability ilkesine göre transaction içerisinde yer alan iş parçalarında bir aksaklık olması halinde sistemin ilk haline dönebilmesi gerekir. Yani taşlar ilk konumlarındaki yerlerinde kalabilmeldir.

Gelelim WCF içerisindeki duruma. Transaction'lar servis yönelimli mimari (Service Oriented Architecture) üzerinde ele alındığında işin içerisine giren faktörler transaction yönetimini zorlaştırmaktadır. Nitekim işlem parçalarının farklı makinelerdeki, farklı uygulama alanları (AppDomain) içerisinde (Dolayısıyla farklı süreç-process'lerde) olmaları söz konusudur. Bu işlem parçaları çeşitli kaynakları (Resources) kullanacaktır. Söz konusu kaynaklar farklı veritabanı sistemleride olabilir. Örneğin bir servis Microsoft SQL Server üzerinde bir işlem gerçekleştiriyorken, buna bağlı olan diğer bir işlem parçasıda Oracle veritabanı kullanan başka bir servisin parçası olabilir. Dahası farklı uygulama alanları (Application Domain) içerisindeki taraflar basit birer istemci dışında başka servislerde olabilir ve bunlar farklı platformlar üzerinde yer alabilir. En iyimser haliyle düşünüldüğünde tek bir AppDomain içerisinde konuşlandırılmış bir WCF Servis uygulaması, kendisine bağlı bir veya daha çok kaynak (resource) üzerinde çalışacak transaction'ları kolay bir şekilde yönetebilir. Aşağıdaki şekilde bu durum analiz edilmeye çalışılmaktadır.

![mk209_2.gif](/assets/images/2007/mk209_2.gif)

Burada bilinen Ado.Net Transaction tiplerinin kullanılması bile yeterlidir. Ne varki az önce bahsettiğimiz gibi servis yönelimli mimari aslında aşağıdaki şekildekine benzer bir durumuda içermektedir.

![mk209_3.gif](/assets/images/2007/mk209_3.gif)

Sanıyorumki şimdi durum biraz daha karıştı. Bu şekle göre istemcinin talepte bulunduğu işlemler sonrası Servis A, Servis B ve Servis C üzerindeki kaynaklardaki işlem parçalarının başarılı bir şekilde tamamlanmasını istemektedir. Bir başka deyişle Servis B, Servis C ve Servis A üzerindeki işlem parçalarının hepsinin ayrı ayrı başarılı olmaları gerekmektedir. Burada ortaya bir koordinasyon problemi çıktığıda son derece açıktır. Örneğin aşağı maddeler halinde sıralanmış olan soruların söz konusu sistemde nasıl karşılanabileceği düşünüldüğünde, sadece bu işler için neden güçlü koordinasyon uygulamaları yazıldığı daha kolay bir şekilde anlaşılmaktadır.

- Acaba bu sistemde Transaction'ı kim başlatacak?
- Diyelimki Transaction birisi tarafından başlatıldı. Diğer işlem parçalarını açılan bu Transaction'a kim dahil edecek (auto enlist)? Öyleki dahil olmadan transaction'ın farklı sistemlere yayıldığı nasıl anlaşılacak?
- Geri alma (Rollback) veya onaylama (Commit) işlemlerini kim üstlenecek?
- Birden fazla servis söz konusu olduğundan bunlar üzerinde toplu bir rollback veya commit kararı nasıl ve neye göre verilecek?
- Eğer Transaction'ı üstlenen, bir başka deyişle süreci başlatıp bitirecek olan servis, diğer servislerdeki işlem parçalarının sonuçlarından nasıl haberdar olacak?
- Bahsedilen bu işlem parçalarını içeren ve yürüten servisler farklı platformlarda hatta farklı iletişim seviyelerinde (Http, Tcp gibi) olduğu zaman nasıl kontrol altına alınacak?

Görüldüğü üzere sorular ve sorunlar giderek artmaktadır. Çözümün adı dağıtık transaction (Distributed Transactions) dır. Gerçektende servis yönelimli mimarilerde (SOA-Service Oriented Architecture) yer alan işlem parçalarını kontrol altına almanın tek yolü dağıtık transaction'ları kullanmaktadır. Dağıtık transaction'lar iki önemli parçaya sahiptir. İlk olarak bu modelde çift yönlü geçerli kılma (two phase commit) adı verilen bir protokol söz konusudur. İkinciside transaction yönetimini üstlenen ve genelde üçüncü parti olarak tedarik edilen bir yönetici uygulamadır (Dedicated Transaction Manager).

Çok basit olarak two phase commit protokolünü açıklamaya çalışarak devam edelim. Bu protokol iki aşamadan oluşur. İlk aşamada transaction'a katılan işlem parçalarından sorumlu servisler kendi işlemlerinin başarılı olup olmadıklarına dair yöneticiye bilgi gönderirler (Pek çok kaynakta bu işlem oylama-voting olarakta geçmektedir). Buradaki yönetici root görevini üstlenen bir servise adanmış da olabilir. İkinci aşamada yönetici, açmış olduğu transaction içerisine katılmış işlem parçalarının oylamalarını değerlendirir. Eğer hepsi geçerli ise tüm bağlı olan işlem parçaları için ilgili servislere onaylayın bilgisini gönderir (Commit aşaması). Elbetteki tam tersi durumda bağlı servislere işlemleri geri almalarına dair başka bir bilgi gönderilecektir (Rollback aşaması).

Söz konusu çift yönlü geçerli kılma protokolünün yanı sıra, Windows Communication Foundation servislerin bulundukları farklı platformlara, aradaki iletişim protokollerinin çeşitliliğine göre üç ayrı transaction protokolü içerir. Bunlar Lightweight, OleTx ve WSAT (WS-Atomic) transaction protokolleridir. Aslında OleTx ve WS-Atomic kendi içlerinde two phase commit protokolünü zaten ele almaktadırlar.

Lightweight transaction protokolüne göre işlem parçaları yerel olarak servis içerisinde ele alınır. Bir başka deyişle söz konusu transaction servis sınırları dışına çıkartılamaz. Buda ilgili iş parçalarının başka servislerde başlatılacak hizmetlere katılamayacağı anlamına gelir. Tipik olarak bir istemci ve servisin söz konusu olduğu durumlarda ele alınabilir. Servis içerisinde yerel olarak kullanıldıklarından performans yönünden avantajlıdır diğer modellerine göre daha avantajlıdır.

OleTx tipinde, transaction başlatıldığı servis, uygulama alanı ve makine sınırlarını aşabilir. Dolayısıyla dağıtık transaction'ların yönetimi sırasında kolayca ele alınabilir. Ancak bu protokol intranet tabanlı Windows sistemleri üzerine tasarlanmıştır. Bu sebepten firewall gibi sistemlere takılma riski vardır. Ayrıca Windows tabanlı geliştirildiğinden farklı tipteki platformlar tarafından desteklenmeyecektir. Bu nedenle OleTx tipi transaction protokolü Windows tabanlı intranet sistemlerinde tercih edilmektedir.

WSAT (Web Service Atomic) tipinden transactionlar aynen OleTx'de oluduğu gibi servis, uygulama alanı ve makine sınırlarını aşabilir. Diğer taraftan WS-Atomic global bir standarttır. Özellikle web servislerinde yaygın olarak kullanılmaktadır. Bu nedele açık metin tabanlı olarak çalıştığında farklı platformlar tarafından kullanılabilir ve OleTx tipinde olduğu gibi firewall engeline takılmaz. Bu sebeten internet tabanlı sistemlerdeki dağıtık transaction'ların yönetiminde tercih edilen bir protokoldür. Elbetteki intranet tabanlı sistemlerede istenirse uygulanabilir.

Bildiğiniz gibi Windows Communication Foundation içerisinde servisler ile istemciler arasındaki iletişim sağlanırken pek çok ayarlamayı bünyesinde barındıran bağlayıcı tipler (Binding Type) kullanılmaktadır. Bağlayıcı tipler iletişim protokolü, güvenlik ayarları dışında, transaction protokollerininde kolay bir şekilde belirlenmesinde önemli görevler üstlenir. Bunların bir kısmının OleTx yada WSAT transaction protokolleri için desteği bulunmamaktadır. Bir kısmınında hiç bir transaction protokolüne desteği bulunmamaktadır. Aşağıdaki tabloda var olan Windows Communication Foundation bağlayıcı tiplerinin desteklediği varsayılan transaction protokolleri yer almaktadır.

Binding Tipi
Transaction Protokolü Desteği

BasicHttpBinding
Destek yok

WSHttpBinding
WSAT

WSDualHttpBinding
WSAT

WSFederationHttpBinding
WSAT

NetTcpBinding
OleTx

NetPeerTcpBinding
Destek yok

NetNamedPipesBinding
OleTx

NetMsmqBinding
Destek yok

MsmqIntegrationBinding
Destek yok

Burada dikkat edilmesi gereken konulardan biriside NetTcpBinding, NetNamedPipesBinding için varsayılan olan OleTx transaction desteğinin WSAT olarak da ayarlanabileceğidir. Ancak intranet üzerinde koşan Windows tabanlı bir sistemde bu performası olumsuz etkileyecek bir değişiklik olabilir. Yinede istenirse, intranet üzerinde farklı platformlar söz konusu olduğunda ilgili bağlayıcı tipler için WSAT desteği seçilebilir.

> Bilindiği gibi Windows Communication Foundation uygulamalarında kendi bağlayıcı tiplerimizi (user defined binding type) yazabiliriz. Böyle bir durumda hangi transaction protokolünün kullanılacağı tam olarak geliştiriciye bağlıdır.

Artık bağlayıcı tipin ele alabileceği transaction protokolleri bilindiğine göre geriye bir tek transaction yönetimini üstlenecek bir uygulama bulmak kalmaktadır. Bu tip bir uygulamayı geliştirmek oldukça zor olduğundan çoğunlukla üçüncü parti programlar göz önüne alınmaktadır. Windows Communication Foundation düşünüldüğünde kullanılabilecek söz konusu yöneticiler, LTM (LightWeight Transaction Manager), KTM (Kernel Transaction Manager) ve DTC (Distributed Transaction Cooridantor) tipleridir. Bu tiplerden KTM, Windows Vista ile birlikte gelmektedir.

Söz konusu yöneticiler arasında bizi en çok ilgilendirenlerden ve aşina olduklarımızdan birisi DTC'dir. Özellikle Ado.Net 2.0 ile birlikte gelen TransactionScope nesnesinin otomatik olarak DTC özelliğini aktifleştirebildiği ve dağıtık transaction'ların daha kolay yönetilebilirliğini sağladığı göz önüne alındığında, servis yönelimli mimari model için ne kadar anlamlı olduğu açık bir şekilde ortadadır. Dağıtık transaction koordinatörü, hem OleTx hemde WSAT transaction protokollerini destekler. Dolayısıyla internet veya intranet tabanlı sistemlerde yer alabilecek servis uygulamalarında transaction'ların servis sınırları dışarısına yayınlanabilmesine olanak sağlar.

DTC (Distributed Transaction Coordinator) transactionların oluşturulup başlatılmasından, süreç içerisindeki diğer uygulama alanlarında (AppDomain) yer alan servislere ait işlem parçalarının var olan transaction'a katılmasından (enlist), kaynak yöneticilerinden (Resoruce Managers) gelen onay mesajlarının (vote) toplanmasından ve elbetteki tüm işlemlerin onaylanması (Commit) veya geri alınmasından (Rollback) sorumlu genel bir sistem servisi olarak göz önüne alınabilir. Bu açıdan bakıldığında Windows Communication Foundation ile arasında oldukça sıkı bir ilişki olması kaçınılmazdır. Konuyu daha iyi kavrayabilmek için aşağıdaki şekil göz önüne alınabilir.

![mk209_4.gif](/assets/images/2007/mk209_4.gif)

İstemci uygulama, Makine A üzerinde yer alan servisten bir işlem için talepte bulunmaktadır. Makine A üzerinde yer alan servis bu senaryoda root olarak görev yapmaktadır. Buna göre transaction'ın başlatılmasından hatta sonlandırılmasından (ister commit ister rollback) kendisi sorumludur. Makine A'da yer alan servis işlemlerin geri kalanı için Makine B ve C üzerinde yer alan servisleride kullanmaktadır. Burada root servis kendi proxy nesnesi yardımıyla B ve C servislerine bir transaction ID değeri gönderir. Bu transaction ID sistem tarafından üretilen benzersiz ve tekrar etmeyen bir değerdir. Bunu programlama ortamında da kullanabildiğimiz GUID (Global Unique IDentifier) değeri olarak düşünebiliriz. Söz konusu ID, B ve C servisleri tarafından kendi makinelerindeki DTC hizmetlerinede bildirilir. Bir başka deyişle B ve C makinelerinde yapılmak üzere olan işlemlerin, transaction ID'si belirtilen sürece ait oldukları bildirilir. Dolayısıyla B ve C servislerinin yürüttükleri işlemler otomatik olarak, root servis tarafından açılan transaction'a dahil edilmiş olurlar.

Bu işlemlerin ardından tahmin edileceği üzere çift yönlü geçerli kılma protokolü (two phase commit protocol) başlar. Yani root servis, transaction'a dahil olmuş diğer servislerden oylarını ister. Burada diğer makinelerdeki DTC servisleri devreye girerek oylama (vote) sonuçlarını root DTC servisine iletirler. Eğer herkes kabul ediyorsa, bir başka deyişle transaction'a dahil olan tüm servisler yaptıkları işlemlere onay vermişse, root servis ikinci aşamaya geçer. Bu aşamada da transaction'a dahil olan servislere işlemleri onaylamalarına dair bilgi gider. Sonuç olarak işlemler onaylanır (Commit). Elbetteki diğer makinelerdeki DTC'lerden gelecek tek bir iptal isteği, tüm transaction'ın iptal edileceği ve işlemlerin geri alınması için root DTC'den, bağlı olan servislere bilgi gönderileceği anlamına da gelmektedir (Rollback). Burada dikkat edilmesi gereken noktalardan biriside, yönetsel işlemleri üstlenen Dağıtık Transaction Koordinasyon (DTC) servisinin, root makinede olmasıdır. Bir başka deyişle Servis Yönelimli Mimari (SOA) modelindeki tüm DTC'lerin oylarını kontrol edip, kabul edilmeleri veya iptal işlemlerini gerçekleştirebilecek tek bir DTC var olabilir.

Windows Communication Foundation, transaction'lara otomatik olarak bir transaction yöneticisi (transaction manager) atar. Varsayılan olarak tek başına çalışan ve farklı makineler yada süreçlerdeki (process) servislere transaction yayınlamayan sistemler söz konusu olduğunda otomatik olarak ilgili transaction'la LTM (LightWeight Transaction Manager) ilişkilendirilir. Ancak servis tarafından başlatılan transaction'ın kendi uygulama alanı sınırları dışına çıkması bir başka deyişle farklı makine veya süreçlere yayınlanması halinde transaction yöneticisinin seviyesi otomatik olarak DTC (Distributed Transaction Coordinator) servisine devredilir. KTM göz önüne alındığında çok değişken kaynaklar (Volatile Resource) vardır. Yinede servis sınırları dışına çıkılması halinde DTC'ye terfi edilme söz konusudur.

Burada elbetteki kaynak yöneticisinin (resource manager) bahsedilen yöneticilerden hangisine destek verdiğide çok önemlidir. Kaynak yöneticisi olarak SQL Server 2005, Oracle, MSMQ gibi pek çok sistem göz önüne alınabilir. SQL Server 2005 veritabanı servisine ait kaynak yöneticisinin şu an için KTM (Kernel Transaction Manager) dışında LTM ve DTC'yi desteklediği bilinmektedir. Aslında DTC değişken olabilecek her türde kaynak yöneticisi tarafından desteklenmektedir.

Böylece geldik bir makalemizin daha sonuna. Bu makalemizde teorik olarak Windows Communication Foundation ortamında transaction yönetiminin temellerini incelemeye çalıştık. Görüldüğü gibi yönetimin arka tarafında dikkate alınması gereken pek çok etken bulunmaktadır. Dahasıda vardır. WCF ile ilgili bir sonraki makalemizde transaction yönetimini örnekler ile incelemeye ve diğer temelleride görmeye çalışacağız. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.