---
title: "WCF Eco System"
pubDate: 2010-01-05 01:30:00
categories:
  - WCF
tags:
  - WCF
  - .NET
  - ADO.NET
  - Entity Framework
  - LINQ
  - Workflow Foundation
  - WPF
  - Windows Forms
  - Silverlight
  - SOAP
  - HTTP
  - IIS
  - Java
  - JavaScript
  - Transactions
---

# WCF Eco System
Merhaba Arkadaşlar,

Özellikle son bir iki yıllık zaman dilimi içerisinde.Net tarafında pek çok servis modeli ve ismiyle karşılaştık. Örneğin Astoria kod adıyla başlayan Ado.Net Data Services, Silverlight gibi Rich Internet Application'ları hedef alan.Net RIA Services vb... (Eğer Microsoft'un ürünleri için kullandığı kod adlarını merak ediyorsanız [Wikipedia](http://en.wikipedia.org/wiki/List_of_Microsoft_codenames)'daki ilgili listeye bakmanızı öneririm) Hal böyle olunca ortada bir sürü kod adı ve isim oluşmaya başladı. Buda çok doğal olarak bizim gibi geliştricilerin kafasında pek çok soru işaretine neden oldu. Acaba hangi servis modelini hangi amaçlar ile kullanmalıyız? Bunların nihai sürümler yaklaştıkça isimlendirmeleri neler olacak? Ne gibi avantaj veya dez avantajları var?

Soruları arttırmak mümkün. Aslında kabul edilmesi gereken önemli bir nokta var; Tüm bu servis modelleri.Net Framework 3.0' dan beri var olan ve her sürümde önemli yetenekler kazanan Windows Communication Foundation (WCF) alt yapısı (Infrastructure diyebiliriz) üzerinde konuşlandırılmış durumda. Şu an içinde bulunduğumuz servis modellerinin çeşitliliğini ve sayısını düşündüğümüzde ise bir Eco System'in oluştuğunu net bir şekilde ifade edebiliriz. Aşağıdaki tabloda WCF Eco System'in parçaları yer almakta olup kısaca amaçları özetlenmeye çalışılmaktadır.

Model

Özet Bilgi

SOAP Services Modeli

Interoperability standartlarına uygun olan böylece örneğin Java gibi platformlar ile konuşabilen, mesaj tabanlı güvenliği (Message Based Security) baz alabilen, transaction akışına (Transaction Flow) izin veren, IIS üzerinden HTTP tabanlı veya IIS dışından host edilebilen (örneğin bir Windows Service, Windows Forms veya WPF uygulaması yada basit bir Console programı olarak), pek çok WS- standardını destekleyen tipteki servisler olarak düşünülebilir. Bu servis modeli.Net Framework 3.0 versiyonundan bu yana mevcuttur. Geliştiricilere çok daha fazla kontrol imkanı sunan bir model olarak düşünülebilir. Diğer servis modellerindeki gibi belirli bir konuya odaklanmaktan ziyade ihtiyaçlara göre düşünülen çözümlerde değerlendirilir.

WebHttp Services Modeli

URI (Uniform Resource Identifier) bazlı olaraktan servis operasyonlarının RESTful yaklaşımına göre sunulduğu fonksiyonellikleri barındıran servis modelidir. WCF tarafındaki bu yetenekler.Net Framework 3.5 ile birlikte gelen Web programlama modeli (Web Programming Model) sayesinde ortaya çıkartılmış olup.Net Framework 4.0' da ek özellikler ile arttırılmıştır. Bu modelde verinin Get,Post,Put ve Delete gibi HTTP protokol metodlarına uygun olaraktan sunulması mümkündür. Geliştiriciler verinin sunulması sırasındaki URI bilgisine, çıktı formatına (örneğin JavaScript Object Notation tipinden olması) müdahalede bulunabilir.

Data Services Modeli

Veri modelimizi (Data Model) ve bu modelin içerdiği iş mantığın bir RESTful arayüzü üzerinden sunmak istediğimiz durumlarda kullandığımız servis modeli yaklaşımıdır..Net için Open Data Protocol desteğini de içermektedir. Aslında ilk olarak.Net Framework 3.5 Service Pack 1 ile ve Ado.Net Data Services adıyla ortaya çıkmıştır. Bu yaklaşımda servisin sunacağı veri kaynağına ulaşırken Ado.Net Entity Framework gibi gelişmiş ORM birimlerinden yararlanılabilir. Ancak istenildiğinde Custom LINQ Provider'lardanda faydalanılıp farklı veri kaynaklarının kullanılması mümkün olabilir.
RIA Services ile Data Services zaman zaman bir birlerine karıştırılmaktadır. Aslında Data Services, veri modelinin ve bu model ile ilişkili iş mantığının sunulması ile ilgilenmekte iken RIA Service'leri Silverlight uygulamalarının end-to-end modelinde geliştirilmesine odaklanmaktadır. Bu nedenle benzer olmalarına rağmen ilgilendikleri vakalar tamamen farklıdır.
Yine Data Service'lerin zaman zaman WebHttp Service'leri ile karıştırılmasıda söz konusudur. Ancak yine her iki servis modelinin ilgilendiği vakalar farklıdır. WebHttp Service'leri URI, format ve protocol bilgileri üzerinde tam yönetime izin verecek şekilde RESTful servisler geliştirebilmemize olanak sağlar. Ancak Data Service'lerde RESTful arayüzü zaten hazırdır ve sadece veri modeli ile ilişkilendirilmesi yeterlidir.

Workflow Services Modeli

Uzun süreli (Long Running) ve kalıcılık (Persistence) desteği olması gereken Workflow uygulamalarının servis bazlı olarak kullanılabilmesini sağlayan modeldir. Bu modele göre Workflow nesnelerinin WCF servisi olarak sunulması, tüketilmesi ve ayrıca Workflow nesnelerinin içerisinde WCF servislerinin çağrılıp değerlendirilmesi mümkündür.
*.Net Framework 3.0' da birbirlerine uzaktan bakan WF ve WCF alt yapıları,.Net Framework 3.5 ile flört etmeye başlamış ve.Net Framework 4.0' da evlenmiştir. ![Wink](images/smiley-wink.gif)

RIA Services Modeli

Sliverlight gibi Rich Internet Application (RIA) uygulamalarında orta katmandaki iş modelinin servis bazlı olarak hem istemci hemde sunucu tarafında yönetilmesini, oluşturulmasını ve kullanılmasnı kolaylaştıran end-to-end servis modelidir. Önceki adı.Net RIA Services olmasına rağmen değişen adıyla birlikte tam olarak Silverlight 4.0 sürümünde nihai versiyonuna ulaşacağı tahmin edilmektedir.

Tabi buradaki bilgiler dışında aşağıdaki çizelgeyide göz önüne almamızda yarar vardır. Bu çizelgede WCF Eco System'in mimari modeli gösterilmektedir.

![blgWCFEcoSystem.jpg](images/blgWCFEcoSystem.jpg)

Aslında WCF alt yapısı üzerinde duran servis programlama modelleri, geliştiricilerin vakaya göre WCF alt yapı detaylarından uzaklaşmasını sağlamaktadır. Bu, özellikle Data Services ve RIA Services modellerinde ön plana çıkmaktadır. Yine de istenildiğinde bu modelleri esnetebileceğimizi bilmeliyiz. Hatta bana kalırsa WCF alt yapısı üzerine oturan kendi programlama modellerimizi de geliştirebiliriz. Ancak buna gerek olup olmadığını tartışmalıyız.

Umarım WCF Eco System hakkında biraz fikir sahibi olabilmişizdir. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
