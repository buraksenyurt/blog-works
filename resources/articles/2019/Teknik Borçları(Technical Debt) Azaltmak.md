---
title: "Teknik Borçları(Technical Debt) Azaltmak"
pubDate: 2019-01-04 05:03:00
categories:
  - DevOps
tags:
  - DevOps
  - HTTP
  - Docker
  - Visual Studio
---

# Teknik Borçları(Technical Debt) Azaltmak
![sonar_7.gif](images/sonar_7.gif)

Merhaba Arkadaşlar,

Bir yazılım ürünü geliştirilirken dikkat edilmesi gereken konuların başında kod kalitesi geliyor. Kaliteli kod, bilinen kodlama standartlarına uyan, okunabilirliği yüksek, karmaşıklığı az, dokümante edilmiş ve bakımı yapılabilir özellikleri barındırmak durumunda. Bu kurallara uymaya çalışmak geliştirme sürelerini uzatsa da uzun vadede kalitenin korunması için gerekli. Üstelik endüstüriyel normlara uygun, derecelendirilebilir uygulamalar geliştirmek istiyorsak kuvvetle üzerinde durulması gereken bir konu. Eğer kaliteyi bozacak ihlaller yaparsak uygulama arkasında ödenmesi zor büyük borçlar bırakabiliriz. Nam-ı diğer Teknik Borç (Technical Debt)

Teknik borç yeni bir kavram değil. İlk olarak Wiki’nin kurucusu, Extreme Programming ve Design Patterns kavramlarının öncülerinden olan Ward Cunningham tarafından ortaya konulmuştur. Oluşmasına etken olan sebepler vardır. Bazen geliştirilen ürünün proje bitiş süresi sebebiyle mecburen stratejik olarak kabul edilir ya da ekip kendi insiyatifinde bunu taktiksel olarak kabul edebilir. Bazı durumlarda ekibin yetkinliğinin az olması sebebiyle ortaya çıkar. Ancak belki de en kötüsü teknik borç üstüne yeni yapılan geliştirmelerin getirdiği ek borçlardır.

Teknik borçların bir kısmı pek çoğumuzun bilmeden de olsa gelecek programcılara bıraktığı ve hatta istemeden de olsa üzerine aldığı sorunlardır aslında. Bu sorunlar sebebiyle zamanla kalitesi bozulan, problemleri çoğalan, güvenilirliği ve daha da kötüsü itibarı azalan ürünler ortaya çıkar. Teknik borçların temizlenmesi mi, müşterinin yeni isteklerinin karşılanması mı derken ürün, üzerinde çalışan programcıları eskitmeye de devam eder. Sonunda legacy olarak tanımlanan, işini yapan ama kimsenin de ellemek istemediği yaşamını devam ettiren korkutucu projeler oluşur.

Teknik borçlanmanın önüne geçmek için alınabilecek belli başlı tedbirler var. Her şeyden önce yazılım geliştirme şeklimizi değiştirmemiz gerekiyor. DevOps gibi kültürlerde yaşamalı, şeffaf olmalı, çevik teknikleri işin içerisine katmalı, pair programming uygulamaktan korkmamalıyız.

Test odaklı geliştirme de kalitenin artması için önemli. Hatta DevOps’un olmazsa olmazlarından birisi. TDD (Test Driven Development) en azından Red, Green, Blue ilkesine göre kod geliştirmemizi öneriyor. Blue (Refactoring) olarak nitelendirilen ve kodun yeniden gözden geçirildiği kısım bile sonradan oluşacak borçların önüne geçmek için önemli. Hatta Code Review süreçlerinin işletilmesi de gerekiyor. Pull Request kavramı da buna hizmet eder nitelikte (Farkındaysanız bunların tamamı DevOps kültüründe bahsi geçen konulardan)

Aşağıdaki tabloda kodu yeniden gözden geçirmenin ve iyileştirmenin teknik borçlanma ile olan ilişkisi ifade ediliyor. Zaman ilerledikçe refactor edilmeyen kodlar teknik borcun artmasına ve değişim maliyetlerinin yükselmesine neden olmakta. Hayatlarımızın belli dönemlerinde rastladığımız (rastlayacağımız) o büyük projeleri düşünün. Hani bakım maliyetleri yüksek olan. Temel sebep, zaman içerisinde çoğalarak artan teknik borç miktarıdır.

![sonar_6.gif](images/sonar_6.gif)

Yazılımcıya düşen bir çok görev var. Test güdümlü yazılım geliştirmeye yatkın olması, kod kalite standartlarından haberdar olması, kokan koddan rahatasızlık duyması, kodlama standartlarının farkında olması, temiz kodun ne anlama geldiğini bilmesi çok önemli. Ancak birey olarak bilinçlenmek de yeterli değil. Kullandığımız programlama dillerinden ortamlara kadar hemen her şey zamanla yarışırcasına yenileniyor, değişiyor. Dolayısıyla temel bilgileri bilsek bile yardımcı araçlardan faydalanmak gerekiyor.

Örneğin kodun statik ve dinamik olarak analiz edilmesi, raporlar çıkartılması ve buna göre gerekli düzeltmelerin yapılması için belli başlı araçlardan yararlanılabilir. SonarQube da bu araçlardan birisi ve beni teknik borçlanma ile ilgili olarak bildiklerimi özetlememin sebepleri arasında.

> (Sürekli denetim)

Biz de DevOps odaklı kültür değişimimiz süresince kod kalitesini arttırmak ve teknik borçları azaltmak için bazı araçlardan yararlanıyor ve hatta danışmanlık alıyoruz (Burada [Saha Bilgi Teknolojileri](https://sahabt.com/) ve [Emre Dundar](https://medium.com/@emredundar) için ayrı bir parantez açmam lazım. Özellikle kod kalitesinin arttırılmasına yönelik farkındalığın oluşturulmasında çok değerli katkıları var)

Tabii test odaklı geliştirilmeyen, üzerinden çokça yazılımcının geçtiği yaşlı projelerde bu araçların sonuçları pek de beklediğimiz (aslında beklediğimiz) gibi değil. Yeni nesil ürünlerde ise inanılmaz derecede yardımcı oluyor ve işin başında tedbirler almamızı sağıyor. Nitekim belirli ihlaller ürünün çıkmaması için yeterli.

Bu amaçla SonarQube ve SonarLint araçlarından yararlanıyoruz. SonarQube, VSTS Continuous Integration hattı üzerinde devreye girmekte. Konulan kurallara bağlı olaraktan check-in’lenmiş kodun Continuous Delivery noktasına geçirilmesine veya geçirilmemesine karar verebiliyor. Eğer sorunlar varsa bunlar için geri bildirimlerde bulunuyor (Bug’ın sahibine mail ile bildirilmesi gibi) Aşağıdaki örnek ekran görüntüsünde aracın var olan ürünlerimizden birisi için olan rapor ekranını görebilirsiniz (Şeffaf olmaktan zarar gelmez)

![sonar_1.gif](images/sonar_1.gif)

Burada çok önemli bilgiler yer alıyor. Örneğin projede toplam 48 bug varmış ve son yapılan geliştirmeler sonrası 13 bug daha dahil olmuş. A, E, D gibi ifadelerle sınav notumuzu görebiliyoruz. Vulnerabilities kısmı kritik. Hatta ilk ele alınan kısım olduğunu ifade edebilirim. Burada özellikle kod bazlı injection’a sebep olabilecek güvenlik açıkları gibi bilgiler yer alıyor. [OWASP](https://www.owasp.org/index.php/Main_Page) benzeri standartlar göz önüne alınarak çalıştırılan kurallar da söz konusu.

Bir de tabii [kokan kod (Code Smells)](http://wiki.c2.com/?CodeSmellMetrics) durumu vardır. Kod standartlarına pek uyulmadığı durumlarda bir süre sonra kodda unutulan iyileştirmelerin sayısı artar. Bu da teknik borç oluşmasına sebebiyet veren etkenlerdendir. Nitekim kodun okunurluğunu zorlaştıran, boat anchor gibi anti-pattern’lerin oluşmasına neden olan durumlar vardır (Daha fazlası için [sizi şöyle alalım](https://medium.com/@burakselyum/y%C4%B1llar-ge%C3%A7sede-de%C4%9Fi%C5%9Fmeyen-%C5%9Feyler-var-121ddf9c0476)) Koddan koku geldiğini anlamanın belli semptomları vardır. Bunları kabaca aşağıdaki gibi sıralayabiliriz.

- Tekrar eden kod parçaları (Duplicated Code)
- Anlaşılması güç uzun metod gövdeleri
- Birden çok şeyi yapmaya çalışan büyük sınıflar (God Object)
- Çok sayıda parametre
- Birden fazla yerde tekrar eden Switch ifadeleri
- Attığı taş kurbayı ürkütmez felsefesindeki tembel sınıflar (Lazy Class)
- Kodun ne yaptığını anlatan yorum satırları (Bu kısım her zaman tartışmaya açık sanırım)
- …

Tekrar grafiğe dönecek olursak kokan kodların temizlenmesi için öngörülen sürelerin de yer aldığını görebiliriz. Toplamda 13 günlük (tahmini bir süre ve aracın bunu hangi tip developer’a göre verdiğini henüz anlayamadım) bir çalışma yapılması gerektiği ifade edilmekte. Diğer yandan bu yaşlı uygulama için Code Coverage değeri yüzde sıfır. Yani kodun hiç bir kısmı için test yazılmamış durumda. Bu pek de iyi bir durum değil. Kodun test edilebilir olması çok önemli. Raporun bir kısmında da tekrar eden kod bloklarına yer veriliyor. Bu proje özelinde kodun %1.8lik kısmı kod tekrarı içermekte. 188 kod bloğunun tekrar edildiği ifade ediliyor. Issues kısmına geldiğimizde durumla ilgili olarak daha fazla detay görebiliriz.

![Sonar_2.gif](images/Sonar_2.gif)

Severity bölümünde yer alan kısımda derecelerine göre seviyelendirilmiş bulgular yer alıyor. Tahmin edileceği üzere Blocker, Critical ve Major kategorisine giren maddeler öncelikli olarak değerlendirilmeliler. Aşağıda bu kısımlara ait örnekler yer alıyor.

Blocker örneği (1o dakikalık bir efor öngörülüyor ve kokan kod kategorisinde olup bizi bloklayacak bir problemden bahsediliyor)

![sonar_3.gif](images/sonar_3.gif)

Critical örneği.

![sonar_4.gif](images/sonar_4.gif)

Major örneği.

![sonar_5.gif](images/sonar_5.gif)

Bu bulgulara bakıldığında SonarQube’ün detaylı çözüm yollarını ve sorunun olduğu kod dosyalarını görebiliyoruz. Tüm bu kriterlere göre bir Quality Gate puanı hesaplanıyor. Şu anki tabloya göre kodun güvenilirliğinin zayıf olduğunu ve bu sebepten Failed statüsünde kaldığını söyleyebiliriz. Yani CI Server bu paketi hiç bir şekilde taşıma kapısına göndermeyecek.

![gandalf_2.gif](images/gandalf_2.gif)

> SonarQube CI server üzerinde kurgulanan bir ürün ancak [bulut tabanlı bir sürümü](https://sonarcloud.io/about) de bulunuyor. Hatta [Docker imajını](https://hub.docker.com/_/sonarqube/) kullanmak da mümkün.

Geliştirici olarak SonarQube sunucusuna gelmeden önce de bir takım tedbirler alabiliriz. Bu noktada SonarLint aracından yararlanabileceğimizi ifade edebilirim. SonarQube sunucusu ile de entegre olabildiği için şirket bünyesinde konulan kural setlerine bağlı kalarak geliştirme yapma şansımız var. Ama yoksa bile local geliştirmeler için varsayılan kural setlerinden yararlanmak mümkün. Hem Visual Studio hem de Code ile eklenti olarak kullanılabilen bir ürün SonarLint.

Pek tabii static kod analizi için geliştirme ortamı ile birlikte gelen ürünler de kullanılabilir. SonarQube, CI hattı ile entegre olabilen merkezi bir statik kod analiz aracı olduğu için local araçlara göre daha fazla tercih edilmekte. Aslında muadil olan ürünler de mutlaka var. Çok fazla araç bağımlısı olmamak da gerekiyor belki ama kod kalitesini arttırmak ve teknik borç yükünü azaltmak için farkındalık yaratacak araçlar bulunduğunu söyleyebiliriz. Biz yazılım geliştiricilerin de bu çerçevede düşünmesi gerekiyor. Eğer yeni bir ürün geliştirmeye başlıyorsanız boyutuna göre mutlaka statik kod analizine tabii olun derim.

Farkında olalım, farkında kalalım.
