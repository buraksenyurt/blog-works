---
layout: post
title: ".NET Remoting'i Kavramak"
date: 2004-05-22 09:00:00 +0300
description: "Bu makalemizde, .net remoting sistemini anlamaya çalışacak ve .net remoting sistemini oluşturan temel yapılar üzerinde duracağız. Öncelikle .net remoting' in ne olduğunu ve ne işe yaradığını tanımlamakla işe başlılayım. Remoting sistemi, kısaca, farklı platformlarda çalışan uygulamalar arasında veri alışverişine imkan sağlayan bir sistemdir. Bu tanımda söz konusu olan platformlar farklı işletim sistemlerinin yer aldığı farklı ve birbirlerinden habersiz proseslerde çalışan uygulamaları içerebilir. Olayın en kilit noktasıda, farklı sistemlerin veri alışverişinde bulunabilmelerinin sağlanmasıdır."
categories:
  - dotnet-remoting
tags:
  - dotnet-remoting
  - dotnet
  - xml
  - soap
  - http
  - performance
  - serialization
---
# .NET Remoting'i Kavramak
Değerli Okurlarım, Merhabalar.

Bu makalemizde,.net remoting sistemini anlamaya çalışacak ve.net remoting sistemini oluşturan temel yapılar üzerinde duracağız. Öncelikle.net remoting'in ne olduğunu ve ne işe yaradığını tanımlamakla işe başlılayım. Remoting sistemi, kısaca, farklı platformlarda çalışan uygulamalar arasında veri alışverişine imkan sağlayan bir sistemdir. Bu tanımda söz konusu olan platformlar farklı işletim sistemlerinin yer aldığı farklı ve birbirlerinden habersiz proseslerde çalışan uygulamaları içerebilir. Olayın en kilit noktasıda, farklı sistemlerin veri alışverişinde bulunabilmelerinin sağlanmasıdır.

Konuyu daha iyi irdeleyebilmek amacıyla, Microsoft tarafından belirtilen şu örneği göz önüne alabiliriz. Bir pocket pc aygıtında, Windows CE işletim sistem üzerinde, C# dili ile yazılmış bir uygulamamız olduğunu düşünelim. Bu uygulama herhangibir parçası olmadığı bir ağda yer alan bir uygulamanın ilgili metodlarını çağırıyor olsun. Uzak network üzerinde kurulu olan bu uygulama, Windows 2000 işletim sisteminde çalışan VB ile yazılmış ve başka bir sql sunucusunda yer alan bir takım bilgileri tedarik eden bir yapıya sahip olabilir. İşte.net remoting ile bu iki farklı uzak nesne arasında kolayca iletişim kurabilir ve veri alışverişini sağlayabiliriz.

Remoting sisteminin bize vadettiği aslında, herhangibir zamanda, yerde ve aygıt üzerinden bilgi erişimine izin verilebilmesidir. Bu aşağıdaki şekilde daha anlaşılır bir biçimde ifade edilmeye çalışılmıştır.

![mk71_1.gif](/assets/images/2004/mk71_1.gif)

Şekil 1. NET Remoting Sisteminde Temel Amaç

Gelelim.net remoting sisteminin temel olarak nasıl çalıştığına ve nelere ihtiyaç duyduğuna. Öncelikli olarak sistemde uzak sınırlarda yer alan nesnelerin olduğunu söyleyebiliriz. Bu nesneler arasında meydana getirilecek iletişim,.net remoting alt yapısında yer alan bir takım üyeler ile sağlanacak. Herşeyden önce, uzak nesneler arasında hareket edicek mesajların taşınması işlemi ile, iletişim kanallarının (Communication Channels) ilgilendiğini söyleyebiliriz. Uzak nesneler arasındaki tüm mesajlar bu kanal nesneleri yardımıyla taşınacak, kodlanacak (encoding) ve çözülecektir (decoding). Kodlamaktan kastımız, uzak nesneler arasında taşınacak mesajların, kullanılan iletişim protokolünün tipine göre belli bir formatta serileştirilmesi (serialization) dir. Diğer yandan kodlanan bu mesajın aynı kurallar çevrçesinde diğer uzak nesne tarafından çözümlenmesi (decoding) gerekecektir. Zaten aradaki mesajların yaygın iletişim protokolleri üzerinden gerçekleştirilmesi, remoting'in en temel özelliklerinden birisidir.

Bir iletişim kanalı nesnesi yardımıyla taşınan mesajlar, doğal.net serleştirici formatları (binary, soap) kullanılarak, kodlanır ve çözülürler. Bir uzak nesne, başka bir uzak nesneye mesaj gönderdiğinde, bu mesaj, kullanılan kanal nesnelerinin esas aldığı protokoller çerçevesinde serileştirilerek binary yada xml formatına dönüştürülür. Mesajı alan uzak nesne ise, serileştirilmiş bu mesajı uygun protokol tabanlı.net serileştirme formatlarını kullanarak açar ve kullanır. Serileştirme amacıyla kullanılan iki kodlama çeşidi vardır.

![mk71_2.gif](/assets/images/2004/mk71_2.gif)

Şekil 2. Serileştirme Seçenekleri

Gelelim diğer önemli bir konuya. Uzak nesneler nasıl kullanılacak. Özellikle server (Sunucu) nitelikli merkezi nesnelere, client (istemci) uygulamalardan nasıl erişilebilecek. Bunun için uzak nesnelere ait referanslara ihtiyacımız olucaktır. Ancak burada karşımıza adresleme sorunu çıkacaktır. Nitekim, herhangibir proses içinde çalışan bir nesne için, uzak nesnelerin çalıştığı prosesler anlamlı değildir. Bu sorunun aşılmasında kullanılabilecek ilk yol sunucu nesnesinin, istemcilere kopyalanmasıdır. Bu durumda istemci uygulama, sunucu nesnesinin bir örneğine ihtiyaç duyduğunda bunu local olarak kullanabilecektir.

Ancak kopylama yönteminin bir takım dezavantajları vardır. Herşeyden önce çok sayıda metod içeren büyük boyutlu nesnelerin kopylanması verimlilik ve performans açısından negatif bir etki yaratacaktır. Bununla birlikte, istemci uygulamalar, server nesnelerinin sadece belirli metodlarını kullanıyor olabilirler. Buda nesnenin tamamımın kopyalanmasının anlamsız olmasına neden olan bir diğer durumdur. Bir diğer dezavantaj ise, server nesnesinin bulunduğu sistemdeki fiziki adreslere referanslar içerebiliecek olmasıdır. Örneğin dosya sistemine başvuruda bulunan nesneleri barındıran server nesneleri söz konusu olabilir. Böyle bir durumda elbetteki kopyalama sonucu bu referanslar yitirilecek ve istemci tarafından kullanılamıyacaktır. Son olarak, server nesnesinin kopyalanmasının, istemci kaynaklarını harcadığını söyleyebiliriz.

Bu dezavantajlar göz önüne alındığında bize başka bir yöntem gerekmektedir. Bu teknikte, server nesnelerini kullanmak için, bu nesnelere referansta bulunan proxy nesneleri kullanılır. İstemci uygulama, bir uzak nesneye ihtiyaç duyduğunda, bu talebini proxy nesnesine iletecektir. Proxy nesnesi ise,.net remoting'in sağlamış olduğu imkanlar dahilinde, ilgili uzak nesnesin ilgili üyesinin referansına başvuracak, bu metodu çalıştıracak ve dönen sonuçları yine proxy nesnesi aracılığıyla, istemci uygulamaya ulaştıracaktır. Bu konuyu aşağıdaki şekil ile daha net bir biçimde zihnimizde canlandırabiliriz.

![mk71_3.gif](/assets/images/2004/mk71_3.gif)

Şekil 3. Proxy Nesnesinin Kullanılması

Remoting sisteminde kullanılan nesnelerin kopyalanabilmesi veya refarans olarak değerlendirilmesi, remoting nesnelerinin iki temel kategoriye ayrılmalarına neden olmuştur. Remotable nesneler ve NonRemotable nesneler. Remotable nesneler, Başka uygulamalarca kopyalanma veya referans tekniği ile erişilebilen nesnelerdir. NonRemotable nesneler ise, diğer uygulamalar tarafından erişilemiyen nesnelerdir. Çoğunlukla büyük boyutlu, çok sayıda metod içeren veya local olarak fiziki adres referanslarına ihtiyaç duyan uzak nesnelerin, nonRemotable olarak belirtilmesi sık görülen bir tekniktir. Peki bu durumda bu nesneleri kullanmak isteyen uzak uygulamalar nasıl bir yol izleyebilir? İşte bu noktada, nonRemotable nesnelerin, istemcilere açılabilecek olan bölümleri için remotable nesneler kullanılır.

Diğer yandan remotable nesneler, kopylama veya referans taşımaya imkan veren uzak nesnelerdir. Burada ise karşımıza iki çeşit remotable nesne oluşumu çıkar. Marshall By Value tipi remotable nesneler ve Marshall by Reference tipi remotable nesneler. Bu kategorilendirme şekilsel olarak aşağıdaki gibidir.

![mk71_4.gif](/assets/images/2004/mk71_4.gif)

Şekil 4. Uzak Nesnelerin Temel Kategorilendirilmesi.

Özellikle remotable nesneler için yapılan ayrım kullanım açısından çok önemlidir. Marshall By Value olarak belirlenmiş remotable nesneler, bir istemci tarafından talep edildiklerinde, özellikle bu nesnenin herhangibir metodu istemci tarafından çağırıldığında,.net remoting sistemi bu nesnenin bir kopyasını oluşturur ve bu kopyayı iletşim kanal nesneleri yardımıyla serileştirerek, çağrıyı yapan istemciye gönderir. İstemci tarafında yer alan,.net remoting sistemi ise bu kopyayı çözümler ve bir nesnenin bir kopyasını istemci makinede oluşturur. Nesnenin, istemciye kopyalanabilmesi için serileştirme işlemi uygulanır. Burada önemli olan nokta, serileştirmenin otomatik olarak gerçekleştirilebilmesi için, nesneye ISerializable arayüzünün uygulanmasıdır. Nesnelerin, istemcilere bu yolla taşınması özellikle istemciler açısından işlem zamanını kısaltıcı bir etken olarak karşımıza çıkmaktadır. Nitekim istemcinin talep ettiği metodlara, artık istemcideki uzak nesnenin kopyası üzerinden erişilmektedir.

Marshall By Reference nesneleri ise, istemci uygulamalar için bu sistemlerde birer proxy nesnesi şeklinde oluşturulur. Bu proxy nesnesi, asıl uzak nesneye ilişkin referansları içeren metadata bilgilerine sahiptir. Uzak nesnenin herhangibir metodu çağırıldığında, proxy nesnesi ilgili metodun referansı ile hareket eder ve bir takım bilgileri sunucuya gönderir. Sunucu gelen bilgi paketini çözümledikten sonra ilgili parametreleri değerlendirerek talep edilen metodu çalıştırır ve bunun sonucu olarak geri dönüş değerlerini istemci makinedeki proxy nesnesine gönderir. Sonuçlar istemciye bu proxy nesnesi yardımıyla gönderilecektir.

Bir uzak nesnenin herhangibir metodunun çağırılmasında veya uzak nesneye ait bir örneğin istemcide new anahtar sözcüğü ile oluşturulmaya çalışılmasında, uzak nesnenin davranış biçimi ve aktifleştirilmesi önem kazanır. Nitekim uzak nesnenin aktif hale gelmesi iki teknik ile gerçekleştirilebilmektedir.

![mk71_5.gif](/assets/images/2004/mk71_5.gif)

Şekil 5. Nesne Etkinleştirme.

Öncelikle sunucu taraflı etkinleştirmeden bahsedelim. Bu etkinleştirme tekniğinde, istemci tarafından sunucu nesnesinin bir metodu çağrıldığında, sunucu tarafından bu nesneye ait bir örnek oluşturulur. Daha sonrada bu nesne örneğinin proxy nesnesi, istemci tarafında oluşturulur. Burada dikkat çekici nokta nesne örneğinin, new anahtar sözcüğü ile değil, sunucu nesneye ait bir metod çağırıldığında oluşturuluyor olmasıdır.

Bu etkinleştirme tekniğine örnek olarak şunu gösterebiliriz. Devlet dairelerindeki sunuculara bağlı bürolar olduğunu düşünelim. Sunucu nesnemiz, bu bürolara, vergi numarasına göre kontrol ve kimlik bilgisi değerlerini gönderiyor olsun. Bu örnekte büro istemcilerinin, sunucuya sürekli bağlı olduklarını düşünelim. İstemci, bir vergi numarasını kontrol etmek istediğinde, sunucu nesnesindeki ilgili metodu çağıracaktır. İşte bu noktada sunucu taraflı etkinleştirme devereye girerek, metod çağırımına karşılık sunucu nesnesinin örneğini oluşturur.

Diğer yandan sunucu taraflı etkinleştirmede, Singleton ve SingleCall teknikleri kullanımaktadır. Singleton tekniğine göre etkinleştirmede, sunucu nesneyi kullanan kaç istemci olursa olsun, her bir istemcinin metod çağırımları sunucu tarafındaki tek bir nesne örneği tarafından yönetilmektedir. SingleCall tekniğinde ise, istemci tarafından yapılan her metod çağırısına karşılık birer sunucu nesnesi örneği oluşturulacaktır.

İstemci taraflı etkinleştirmeye gelince. Bu kez sunucu taraflı etkinleştirmenin aksine, new anahtar sözcüğü ile bir sunucu nesne örneği istemci uygulamada oluşturulduğunda, sunucu üzerinde sunucu nesnesinin örneği oluşturulacaktır. Bu tip kullanıma örnek olarak chat uygulamalarını gösterebiliriz.

Remoting sisteminde uzak nesneler dışında en önemli unsur mesajları taşıyan kanal nesneleridir. (Channels) Kanal nesneleri uzak bilgisayarlarda yer alan uzak nesneler arasındaki haberleşmede rol oynayan kilit nesnelerdir. Aradaki iletişimi sağlamak için kullanılan kanal nesneleri, bu iletişim üzerinden mesajların gönderilmesi, taşınması ve alınması gibi işlemlerden sorumludurlar. Bildiğiniz gibi kanal nesnelerinin taşıdığı mesajlar HTTP veya TCP protokolleri çerçevesinde hareket ederler. Bir kanal nesnesi yardımıyla, uzak kanallara mesaj gönderebilir yada uzak kanallardan gelen çağrıları dinleyebiliriz.

Bir uzak nesne, başka bir uzak nesneye mesaj gönderdiğinde, kanal nesneleri devreye girerek bu mesajı binary veya xml formatında serileştirir. Mesajı yani çağrıyı alan uzak nesne ise, bu mesajın içeriğini, buradaki kanalın mesajı binary veya xml formattan çözümlemesi ile okuyabilir. Remoting sisteminde kullanılan kanallara ilişkin sınıflar ve arayüzler System.Runtime.Remoting.Channels isim alanında yer almaktadır.

Temelde bütün kanal nesneleri IChannels arayüzünü uygulamaktadır. Kanal nesneleri iletişimde oynayacakları role göre kategorilendirilirler. Eğer kanal nesnesi çağrıları dinlemek amacıyla kullanılacaksa, alıcı (receiver) veya server (sunucu) olarak adlandırılırlar. Bununla birlikte, mesaj göndericek olan kanal nesneleri sender (gönderici) veya client (istemci) olarak adlandırılırlar.

![mk71_6.gif](/assets/images/2004/mk71_6.gif)

Şekil 6. Kanallar.

Receiver kanallar, IChannelSender arayüzünü uygulayan kanallardır. Kullandıkları protokollere göre HttpClientChannel ve TcpClientChannel sınıflarından oluşturulurlar. Diğer yandan Sender kanallar, IChannelReceiver arayüzünü uygulayan TcpServerChannel ve HttpServerChannel nesneleridir. Diğer önemli iki kanal nesnesi ise HttpChannel ve TcpChannel nesneleridir.

Şimdi dilerseniz bu kanalların.net remoting sisteminde oynadıkları rolleri kısaca incelemeye çalışalım. Burada önemli olan, mesajlaşmanın gerçekleştirileceği protokoldür. Nitekim HTTP protokolünü kullanacaksak buna uygun kanal nesnelerini kullanmalı, TCP protokolünü kullanacaksakta buna uygun kanal nesnelerini kullanmalıyız. Eğer HTTP protokolü kullanılacaksa, System.Runtime.Remoting.Channels.Http isim alanında yer alan kanal sınıflarını kullanırız.

![mk71_7.gif](/assets/images/2004/mk71_7.gif)

Şekil 7. Http Kanalları.

İstemciden, uzak nesnelere mesaj göndermek için HttpClientChannel sınıfına ait nesne örnekleri kullanılır. Diğer taraftan, istemcilerden gelicek çağrıları dinleyecek kanal nesneleri ise, HttpServerChannel sınıfından örneklenir. HttpClient sınıfına ait nesne örnekleri ise mesaj almak ve mesaj göndermek için kullanılırlar. HttpClientChannel nesne örnekleri oluşturulurken, bu kanalın kullanacağı bir port numarasını özellikle belirtmemiz gerekmez. Remoting sistemi o an için kullanılabilen serbest portlardan birisini bu kanal nesnesi için tahsis edecektir. Diğer yandan çağrıları dinlemek amacıyla tanımlanan bir HttpServerChannel kanal nesnesinin belirli bir port numarası üzerinden oluşturulması gerekmektedir. Şayet o an için kullanımda olan bir port belirlenirse çalışma zamanında istisna alırız. Bununla birlikte HttpServerChannel nesnesinin yapıcı metoduna 0 değerini göndererek, port seçme insiyatifini otomatik olarak.net remoting sistemine bırakabiliriz.

Http kanalları SoapFormatter sınıfını kullanarak, mesajları xml formatında serileştirirler. Bununka birlikte, TCP protokolünü kullanan kanal nesneleri ise, serileştirme işlemini binary olarak yapar ve bunun içinde BinaryFormatter sınıfını kullanırlar. TCP protokolünü taban alan kanal nesneleri, System.Runtime.Remoting.Channels.Tcp isim alanında yer alan sınıflardan örneklenirler.

![mk71_8.gif](/assets/images/2004/mk71_8.gif)

Şekil 8. Tcp Kanalları.

Tcp isim alanında yer alan sınıflar, Http'dekiler ile aynı işlevlere sahiptirler. Tek fark, kullanılan serileştirme işleminin farklı oluşudur. Her iki isim alanı içinde, oluşturulan kanal nesne örneklerinin ChannelServices sınıfında yer alan static RegisterChannel metodu ile sisteme kayıt edilmeleri gerekmektedir.(Registiration)

Kanallaların kullanımında dikkat edilmesi gereken en önemli nokta, uzak nesnelerin aynı protokolü destekleyen kanalları kullanmalarının gerekli olduğudur. Örneğin, istemcilerden gelen çağrıları dinlemek amacıyla Http protokolünü taban alan kanal nesneleri kullanılıyorsa, istemcilerdede mesaj göndermek için Http protokolünü taban alan kanal nesneleri kullanılmalıdır. Nitekim farklı protokol tabanlı kanalların kullanılmasında istisnalar alırız. Bunun sebebi, farklı protokol kullanımının sonucu olarak kodlama ve çözümleme işlemlerinin farklı serileştirme teknikleri içerisinde yapılıyor olmasıdır. Http ve Tcp kanalları arasındaki farkları şu şekilde özetleyebiliriz.

1
Http kanal nesneleri Http protokolünü, Tcp kanal nesneleri ise Tcp protokolünü kullanır.

2
Serileştirme işleminde, Http kanal nesneleri SoapFormatter sınıfını kullanırken, Tcp kanal nesneleri BinaryFormatter sınıfını kullanır.

3
Http kanal nesneleri için serileştirme xml tabanlı yapılırken, Tcp kanal nesneleri için serileştirme binary formatta yapılır.

Bu makalemiz ile.net remoting sisteminin temel yapıtaşlarını tanımaya çalıştık. Bir sonraki makalemizde, en basit anlamda bir remoting uygulamasının nasıl yapıldığını incelemeye çalışacak ve teoride anlattıklarımızın programatik olarak nasıl yazılacağını göreceğiz. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.