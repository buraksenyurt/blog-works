---
title: "Visual Studio 2012 için Entity Framework Yenilikleri"
pubDate: 2013-02-13 01:18:00
categories:
  - .Net Framework 4.5
tags:
  - .Net Framework 4.5
  - .NET
  - ADO.NET
  - Entity Framework
  - LINQ
  - XML
  - HTTP
  - Visual Studio
---

# Visual Studio 2012 için Entity Framework Yenilikleri
[![Delphi5_RTM](images/Delphi5_RTM_thumb.jpg)](images/Delphi5_RTM.jpg) Merhaba Arkadaşlar,

Çok eskiden kullanılan programlama dilleri ve platformları düşünüldüğünde çok ilkel IDE’ ler ile çalışmış olduğumuzu görmekteyiz. Hatta bazı programlama dilleri ile yapılan geliştirmelerde değil IDE, komut satırına mahkum olmuşuzdur (Gerçi komut satırında script yazarak geliştirme yapmak özellikle fonksiyonel programlama dilleri göz önüne alınırsa oldukça popüler ve isabetlidir)

Gelişen sistemler, kullanıcı deneyimleri ve görselliklerin artması ile de yazılımcıların daha profesyonel olan IDE’ ler üzerinde çalışması şart olmuştur.

Bu anlamda tarihin belki en başarılı geliştirme arayüzlerinden birisi, herkesin bildiği üzere Delphi ortamıdır. Ne varki o zaman ki Borland firmasının sahip olduğu bu özellik, Anders Heijslberg’ in Microsoft takımına geçmesi ile birlikte yerini Visual Studio ailesine bırakmıştır. Anders sadece.Net Framework plaformu ve C# diline babalık etmemiş önemli bir User Experience tecrübesini de Microsoft şirketine taşımıştır. Visual Studio özellikle 2008 sürümünden itibaren inanılmaz derecede gelişti ve gelişmeye de devam ediyor.

Bu IDE ile çalışmak hem çok keyifli, hem de tek bir ortam üzerinden çok geniş bir yelpazeye ulaşılabilmekte. Açıkçası IDE dışına çıkmadan herşeyin elinizin altında olduğu bir geliştirme ortamında çalışmanın değeri paha biçilemez…Gerisi için Master Card

![Smile with tongue out](images/wlEmoticon-smilewithtongueout_7.png)

Entity Framework bilindiği üzere son sürümlerinden itibaren çok daha fazla etkili olmaya başladı. Bu noktada Ado.Net geliştirici takımının müşteri ihtiyaçlarını da çeşitli anketler yardımıyla dinliyor olmasının önemi büyüktür. Örneğin uzun bir zaman anketin en üst sıralarında yer alan Enum desteğinin getirilmesi gibi. (Bu amaçla açılan [Entity Framework Feature Suggestions](http://data.uservoice.com/forums/72025-entity-framework-feature-suggestions/filters/new) forumunu takip etmenizi öneririm)

Biz bu yazımızda Entity Framework’ ün Visual Studio 2012 tarafındaki bazı yeniliklerine değinmeye çalışıyor olacağız (Bazı diyorum çünkü yazıyı yazdığım sırada yeni bir update çıkmış da olabilir. Lütfen kontrol ediniz ve takipte kalınız) Haydi gelin hiç vakit kaybetmeden işe başlayalım.

## Enum Desteği

Enum tipi için Entity Framework tarafındaki desteği epeyce bekledik aslında. Bu desteğin gelmesi ile birlikte elbetteki Visual Studio 2012 IDE’ si de gerekli kolaylığı göstermekte. Model içerisine yeni bir Enum tipi eklenmek istendiğinde tek yapılması gereken, diagram üzerinden Add New –> Enum Type seçeneğini işaretlemek.

[![vsef_6](images/vsef_6_thumb.png)](images/vsef_6.png)

Bu işlemin ardından çıkan iletişim penceresinden, Enum tipine ait sabit değerleri girilir. Örneğin aşağıdaki şekilde görüldüğü üzere ülkeleri kullanabiliriz.

[![vsef_7](images/vsef_7_thumb.png)](images/vsef_7.png)

İstenirse harici bir assembly/isim alanı içerisinde yer alan Enum tiplerinin kullanılması da mümkündür. Bunun için Reference external type seçeneğinin etkinleştirilmesi ve [Namespace].[EnumTypeName] formatında ilgili tip adının girilmesi yeterlidir. Enum tipi oluşturulduktan sonra Model Browser penceresinde yer alan Enum Types kısmında görülecektir.

[![vsef_8](images/vsef_8_thumb.png)](images/vsef_8.png)

Enum tipleri için Designer tarafında bir destek yoktur (Şimdilik) ama bu pek de gerekli olmayabilir. Enum tipleri sonuç itibariyle sabitlerini sayısal olarak ifade etmektedir. Bu açıdan bakıldığında, bir Entity tip özelliğinin sayısal değerinin karşılığı olarak ilgili Enum sabitlerinin kullanılabilmesi de mümkündür. Örneğin Employee isimli bir Entity’ nin EmployeeCountry isimli int tipinden bir özelliğinin (Veritabanı tablosunda da bunun karşılığı alanın olduğunu varsayıyoruz) bulunduğunu düşünelim. Bu özelliği Country isimli Enum sabiti ile aşağıdaki şekilde görüldüğü üzere eşleştirebilir ve artık kod içerisinde yer LINQ sorgularında sayısal değer yerine Enum sabitlerini kullanabiliriz.

[![vsef_9](images/vsef_9_thumb.png)](images/vsef_9.png)

## Kalabalık Diagramlardan Kurtulduk

Entity sayısının model üzerinde artması sonucu oluşan görsel kullanım güçlüğünü engellemenin yollarından birisi de, diagramları bölümlemektir. Bunun şu an için kullanışlı olan iki farklı tekniği mevcuttur.

> Model Diagramın bölümlenmesinin sebeplerininin başında, kalabalık ve takibi zor olan, çok sayıda Entity içeren modeller yer almaktadır. Ancak tek sebep bu değildir. N sayıda Entity içeren bir model’ de sadece bir kaç Entity ile çalışıldığı durumlarda, bu senaryoya konu olan objeleri kolayca takip edebilme ihtiyacı da nedenler arasında gösterilebilir.

### Diagram Bölümleme (Move Metodu ile)

Bu teknikte Model üzerinden başka bir diagrama alınmak istenen Entity’ ler seçilir ve Move to new diagram ile taşınması sağlanır. Örneğin Northwind veritabanı için üretilen model’ de yer alan bir kaç Entity objesini seçip, sağ tıklama sonrası açılan menüden Move to new diagram seçeneğini işaretleyerek söz konusu bölümlemeyi gerçekleştirebiliriz.

[![vsef_3](images/vsef_3_thumb.png)](images/vsef_3.png)

Bu işlem sonucunda seçilen Entity objeleri yeni bir diagrama alınacak ve bu değişiklik Model Browser penceresinden de izlenebilecektir. Aşağıdaki çıktıda görüldüğü gibi.

[![vsef_4](images/vsef_4_thumb.png)](images/vsef_4.png)

Bu teknikte dikkat edilmesi gereken en önemli nokta gerçek anlamda bir Move işlemi yapılmasıdır. Yani, taşınan Entity objeleri aslında kaynak diagram üzerinden silinirler. Eğer bu istenmiyorsa takip eden teknikten yararlanılabilir.

> Farklı sayıda diagram kullanıldığı durumlarda bunları anlamlı şekilde isimlendirmekte önemlidir. AdventureWorks gibi veritabanlarını düşündüğümüzde, tabloların şema (schema) bazlı olarak tutulduklarını ve domain olarak ayrıştırıldıklarını görürüz.
> Buna göre her domain için ayrı bir diagram oluşturmak, görsellik ve takip edilebilirlik açısından oldukça yararlı olabilir. Böyle bir vakada, parçalanan ana modele ait diagramları yine veritabanında olduğu gibi şema adları ile tutmak mantıklı olacaktır.

### Diagram Bölümleme (Model Browser üzerinden)

Model Browser, Entity diagramlarının yönetilebildiği etkili dialog pencerelerinden birisidir. İstenirse bir modelin bölümlenerek farklı diagramlara taşınması/kopyalanması işlemi buradan da gerçekleştirilebilir. Bunun için öncelikli olarak Model Browser’ a yeni bir diagram ilave edilir.

[![vsef_5](images/vsef_5_thumb.png)](images/vsef_5.png)

Yeni bir diagram oluşturulduktan sonra yine Model Browser içerisinde yer alan Entity’ leri boş alana sürükleyerek söz konusu işlemi gerçekleştirebiliriz. Burada ne yazık ki bir diagramın içindeki öğeyi seçip başka bir diagrama sürükleme işlemi söz konusu değildir (En azından kullandığım Visual Studio 2012 sürümü için bu geçerli değildi) Bunun yerine örneğin NorthwindModel ismiyle duran öğe altından sürükleme işlemlerinin yapılması gerekmektedir.

> Diagram bölümleme aslında görsel bir ayrıştırma anlamına gelmektedir. Bir başka deyişle diagram bazlı yapılan taşıma, kopyalama gibi işlemler modelin tip yapısını bozmaz.

### Daha Renkli Bir Diagram

Malum pek çok veritabanı sistemi (özellikle Relational olanlar ve Enterprise çözümlerde kullanılanlar) oldukça fazla sayıda tablo, view ve Stored Procedure içermekte. Hal böyle olunca Entity modellerini içeren diagramların, Visual Studio IDE ortamında takip edilmesi de çok zor olabilmekte.

Diagramların bölünebilmesi özelliği haricinde Visual Studio 2012 ile gelen önemli özelliklerden birisi de, Entity objelerinin renklendirilebiliyor olması. Bu özellik sayesinde örneğin aynı isim alanına ait (veya aynı domain içerisinde yer alması gereken) Entity’ leri farklı şekillerde renklendirerek, diagramın hem göze daha hoş görünmesi sağlanabilmekte hem de algının daha da güçlendirilmesi mümkün olmakta. Bunun için bir kaç Entity objesini aynı anda seçip, özellikler penceresinden Fill Color’ ı set etmemiz yeterli olacaktır. Aşağıdaki ekran çıktısında bu kullanıma örnek bir görüntü yer almaktadır

![Winking smile](images/wlEmoticon-winkingsmile_147.png)

Renkli TV Versiyonu

[![vsef_1](images/vsef_1_thumb.png)](images/vsef_1.png)

Sıkıcı Olan Siyah Beyaz TV Versiyonu

[![vsef_2](images/vsef_2_thumb.png)](images/vsef_2.png)

## Birden Fazla Stored Procedure Seçerek Eklemek (Batch Insert)

Entity Framework bilindiği üzere veritabanı tarafındaki Stored Procedure’ leri birer fonksiyon olarak model tarafına almaktadır. Pek tabi Database First modelin kullanıldığı senaryolarda, Model üretilirken Stored Procedure’ lerden istenilenlerinin seçilerek de ilave edilmesi istenebilir. Visual Studio 2012’ de bunun için ek bir seçenek getirilmiştir.

[![vsef_10](images/vsef_10_thumb.png)](images/vsef_10.png)

Entity Data Model Wizard üzerinde ilerlerken çıkan Import selected stored procedures and functions into the entity model seçeneği işaretlendiği takdirde, Stored Procedures and Functions kısmında seçilen yordamlar için gerekli fonksiyonların topluca üretildiği ve modele dahil edildiği görülecektir. Bu, Visual Studio 2010 da yapılan t anında tek bir Stored Procedure’ ü fonksiyonelleştirme aksiyonuna göre çok daha iyidir (Add->Function Import)

## Varsayılan Olarak DbContext Türevli Entity Model

Code First Development yaklaşımının en önemli noktalarından birisi de POCO (Plain Old CLR Objects) dışından DbContext türevli bir Context tipinin kullanılmasıdır. Visual Studio 2012 arabirimi Entity Model’ lerin üretilmesinde artık varsayılan olarak Entity Framework 5 DbContext T4 Template’ ini kullanmakta ve buna göre de üretilen entity context sınıfı DbContext türevli oluşturulmakta. Hatta, Entity sınıflarının da birer POCO tipi olarak üretildiğini görmekteyiz.

[![vsef_12](images/vsef_12_thumb.png)](images/vsef_12.png)

Bilindiği üzere Visual Studio 2010 bu konuda ObjectContext türevli bir yaklaşımı benimsemektedir. Ancak Entity Framework takımın uygulama geliştiricilere önerisi DbContext türevli içerik tipini kullanmaları ve POCO sınıfları ile ilerlemeleri yönündedir.

## Property Sıralaması

Model diagramda yer alan Entity özellikleri varsayılan olarak veritabanındaki kolon sıralamasına göre gelmektedir. Ama istenirse bu özelliklerin sırası değiştirilebilir. Bunun için Alt+Yön Tuşu kombinasyonu kullanılabilir (Örneğin Alt+Up ile özellik bir üste, Alt+Home ile en başa, Alt+End ile en sona geçer) veya özellikler penceresinden aşağıdaki şekilde görüldüğü gibi ilerlenebilir. (Visual Studio 2010 IDE’ sin de bu işlem için XML tarafına geçmek gerekirdi)

[![vsef_11](images/vsef_11_thumb.png)](images/vsef_11.png)

Böylece geldik bir yazımızın daha sonuna. Bu yazımızda özellikle Visual Studio 2012 ile Entity Framework tarafına gelen yenilikleri incelemeye çalıştık. Çok doğal olarak çeşitli Extension’ lar yardımıyla Visual Studio 2012 ortamındaki Entity Framework niteliklerini arttırmak mümkündür. Bu yazımızda, varsayılan olarak gelen kabiliyetlere bakmaya çalıştık. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
