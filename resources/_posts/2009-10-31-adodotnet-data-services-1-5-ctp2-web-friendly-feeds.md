---
layout: post
title: "Ado.Net Data Services 1.5 CTP2 - Web Friendly Feeds"
date: 2009-10-31 11:25:00 +0300
categories:
  - ado-net-data-services
tags:
  - ado.net-data-services
  - web-firendly-feeds
  - language-integrated-query
  - entity-framework
  - odata
---
Ado.Net Data Services v1.5 CTP1 ile gelen Web Friendly Feeds özelliği, CTP2 sürümünde eklenen iki yeni eşleştirme seçeneği ile genişletilmiştir. Durun bir dakika...Web Friendly Feeds nedir?

![blg80_Giris.jpg](/assets/images/2009/blg80_Giris.jpg)

![Undecided](/assets/images/2009/smiley-undecided.gif)

Arkadaşlıktan farklı bir şey olsa gerek

![Wink](/assets/images/2009/smiley-wink.gif)

Öncelikle bu konuya açıklık getirmek gerekiyor.

Web Friendly Feeds özelliği, bir Entity'nin herhangibir özelliğini (Property), Ado.Net Data Service'inden çıktı olarak üretilen Atom içeriğindeki bir elemente eşleştirmekte kullanılmaktadır. Nitekim servisin ürettiği varsayılan Atom içeriğinde yer alan author name, url, title vs... gibi bilgiler zaten standart olarak kabul edilmiştir ve bu nedenle söz konusu elementleri değerlendiren yorumlayıcılara, var olan Entity içeriğindeki bazı özellik değerlerinin aktarılması istenebilir. Bir başka deyişle, servisin ürettiği içeriğin kaynağındaki özelliklerin çıktıda map edileceği yerler, Atom içeriğindeki belirli noktalar olarak belirlenebilir.

Çok doğal olarak eşleştirmenin çalışma zamanında değerlendirilmesi gerekmektedir. Ado.Net Data Servislerin, arka planda Entity Framework veya Custom LINQ Provider kullandığı düşünüldüğünde, eşleştirme işleminin nerede bildirileceği şu anda bizim için öğrenilmesi gereken yegane noktadır. Bir geliştirici olarak söz konusu eşleştirme bildirimlerinin nitelik (attribute) veya konfigurasyon bazlı olarak yapılacağının düşünülmesi son derece doğaldır ki bunların çalışma zamanında ele alınması gerekmektedir. Biz bu yazımızda önce eksikliği anlamaya çalışacak, sonrasında ise Entity Framework kullanılması halinde eşleştirmeyi nasıl gerçekleştirebileceğimizi inceleyeceğiz.

Öncelikle basit bir Asp.Net Web uygulaması oluşturalım ve aşağıdaki EDM grafiğinde görülen Ado.Net Entity Data Model öğesini söz konusu projeye ekleyelim. Her zamanki gibi kobay olarak AdventureWorks veritabanını hedef alıyor olacağız.

![blg80_Edm.gif](/assets/images/2009/blg80_Edm.gif)

Contact tablosunu değerlendirmek amacıyla basit bir Ado.Net Data Services v1.5 CTP2 öğesini projeye ekleyerek devam edelim. Öğemizin kod içeriğini aşağıdaki gibi geliştirmemiz konumuz için yeterlidir.

```csharp
using System.Data.Services;

namespace WebFriendlyFeed
{
    public class AdventureServices 
        : DataService<AdventureWorksEntities>
    {
        public static void InitializeService(DataServiceConfiguration config)
        {           
            config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);        
            config.DataServiceBehavior.MaxProtocolVersion = System.Data.Services.Common.DataServiceProtocolVersion.V2;
     }
    }
}
```

Söz konusu servis üzerinden Contact tablosundaki örneğin ilk 3 satırı talep ettiğimizde aşağıdakine benzer bir çıktı elde ederiz.

![blg80_FirstRun.gif](/assets/images/2009/blg80_FirstRun.gif)

Eksiklik şudur; standart atom feed içeriğinde yer alan author/name ve title elementlerinin içeriği boştur. Bu bilgilerin eksik olması şu aşamada önemli değilmiş gibi görünebilir. Ama bu atom feed çıktısını değerlendiren bir uygulamada söz konusu alanlar belirli amaçlar ile kullanılıyor olabilir ve bu nedenden boş olmaları yararlı olmayabilir. Örneğin Internet Explorer'ın feed içeriklerini görsel olarak yorumlama özelliğini (Turn on feed reading view) açtığımızda aynı sorgu aşağıdaki sonucu üretecektir.

![blg80_FirstRunViewer.gif](/assets/images/2009/blg80_FirstRunViewer.gif)

Görüldüğü gibi ilk etapta feed entry'leri ile ilişkili title veya author/name gibi elementler doldurulmadığı için okunabilir bir içeriğin oluşmadığı görülmektedir. Üstelik Title gibi alanlara göre sıralama işlemi yapılmasıda mümkün değildir. İşte Ado.Net Data Services 1.5 sürümünde getirilen Web Friendly Feed özelliği, söz konusu standart entry elementlerinden bazılarının, entity içeriğinden beslenmesini sağlayabilmektedir. Peki ama nasıl?

![Undecided](/assets/images/2009/smiley-undecided.gif)

Örneğimizde Entity Framework modeli kullanıldığından, Conceptual Schema Definition Language (CSDL) içeriğinde bazı ayarlamalar yapılması gerekmektedir. (Tabi Entity modelinin Update edilmesi halinde bu değişikliklerin uçabileceğini hatırlatmam gerekir.) Aşağıdaki şekilde görüldüğü gibi.

![blg80_EdmChange.gif](/assets/images/2009/blg80_EdmChange.gif)

İlk olarak bir namespace ilave edildiğini görüyoruz. Bu namespace ilave edilmez ise, FCKeepInContent veya FCTargetPath gibi nitelikleri kullanamayız. Yeri gelmişken bu niteliklerin ne iş yaptıklarını açıklayalım. CSDL içeriğinde yer alan FirstName özelliğinin değerinin Atom Feed içerisindeki author/name elementinde çıkması için FCTargetPath niteliğine SyndicationAuthorName değeri atanmıştır. Benzer şekilde Atom Feed içeriğinin bir Contact için üretilen Title elementinde LastName özelliğinin değerinin çıkması için, FC_TargetPath niteliğine SyndicationTitle değeri atanmıştır.

Aynı işlem Email adresi içinde gerçekleştirilmiş ve özellik değerinin author/email elementinde çıkması için FCTargetPath niteliğine SyndicationAuthorEmail değeri atanmıştır. Buna göre FCTargetPath niteliğinin değerinin, entity özelliğinin hangi atom alanında çıkacağını belirttiğini ifade edebiliriz. FCKeepInContent niteliğine atanan false değeri ilede, atom feed'in element noktlarında çıkan değerlerin, üretilen Contact elementine ait content içeriğinde gösterilmemesi sağlanmaktadır. Buna göre biraz önce yaptığımız talebi tekrarlarsak aşağıdaki çıktıları alırız.

![blg80_SecondRunPreview.gif](/assets/images/2009/blg80_SecondRunPreview.gif)

Ve atom çıktısını içeriği;

![blg80_SecondRunSource.gif](/assets/images/2009/blg80_SecondRunSource.gif)

Görüldüğü üzere FirstName ve EmailAddress bilgileri, author elementi altındaki name ve email alt elementlerine yazılmıştır. Ayrıca entry'nin title elementinin içeriğinede LastName özelliğinin değerinin yazıldığı görülmektedir. Bununla birlikte buradaki özellikler, entry elementi altındaki content elementi içeriğindende çıkartılmıştır. Süper

![Smile](/assets/images/2009/smiley-smile.gif)

Peki Atom Feed Entry Elementlerinden hangilerine atamalar yapabiliriz? İşte Ado.Net Data Services v1.5 CTP2 eklentilerinin de yer aldığı son liste.

Entity tip özelliklerini hangi Atom Entry elementlerine eşleştirebiliriz.

author/email -> SyndicationItemProperty.AuthorEmail
author/name -> SyndicationItemProperty.AuthorName
author/uri -> SyndicationItemProperty.AuthorUri
published -> SyndicationItemProperty.Published
rights -> SyndicationItemProperty.Rights
summary -> SyndicationItemProperty.Summary (CTP2 ile Gelmiştir)
title -> SyndicationItemProperty.Title
Updated -> SyndicationItemProperty.Updated (CTP2 ile Gelmiştir)
contributor/name -> SyndicationItemProperty.ContributorName
contributor/email -> SyndicationItemProperty.ContributorEmail
contributor/uri -> SyndicationItemProperty.ContributorUri

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[WebFriendlyFeed.rar (35,28 kb)](/assets/files/2009/WebFriendlyFeed.rar)
