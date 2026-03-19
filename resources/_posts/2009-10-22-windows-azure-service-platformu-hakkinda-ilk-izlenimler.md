---
layout: post
title: "Windows Azure Service Platformu Hakkında İlk İzlenimler"
date: 2009-10-22 07:15:00 +0300
description: "Microsoft' un son yıllarda Cloud Computing mimarisi için getirdiği geliştirmelerden biriside Windows Azure Services platformudur. Bu platformu servis bazlı bir işletim sistemi olarak düşünebiliriz. Ama bu çok basit bir yaklaşım olur. Bu konuda aslında pek çok kaynakta yazılmakta ve çizilmektedir. Ancak ürün henüz nihai halini almadığından sürekli olarak değişimlere uğramaktadır. Söz gelimi daha önce yayınlanan SDK içerisine Microsoft.Net Services bloku içerisinde yer alan Workflow Services modeli, Temmuz 2009' da yayınlanan CTP sürümünde kaldırılmışdır. Aslında konunun detaylarına girdiğimde çok kısa bir sürede kaybolduğumu ifade edebilirim. Bu nedenle yazımızın bundan sonraki kısımlarında mimarinin ne getirdiğini sizlere araştırma sonuçlarımdan aktarmaya çalışacağım."
categories:
  - windows-azure
tags:
  - windows-azure
  - dotnet
  - wcf
  - soap
  - rest
  - http
  - java
  - ruby
  - visual-studio
---
# Windows Azure Service Platformu Hakkında İlk İzlenimler
![blg91_Giris.jpg](/assets/images/2009/blg91_Giris.jpg)

Merhaba Arkadaşlar,

Güneşli açık bir hava ve sessiz bir gün yada gece...Deniz kenarında veya caddenin herhangibir köşesinde...Mutlaka zaman zaman gökyüzüne bakıp bulutları bazı nesnelere benzettiğimiz olmuştur. Yandaki resimde görüldüğü gibi gülen bir bulutla karşılaşma ihtimalimiz az olsada, arabaya, ördeğe veya başka bir şekle benzettiğimiz bulut sayısı oldukça fazladır. Bugünkü yazımızda bulutlar ile pek bir haşır neşir olacağımızı düşünebilirsiniz. Ancak tabiki yazılımsal anlamda

![Wink](/assets/images/2009/smiley-wink.gif)

Microsoft'un son yıllarda Cloud Computing mimarisi için getirdiği geliştirmelerden biriside Windows Azure Services platformudur. Bu platformu servis bazlı bir işletim sistemi olarak düşünebiliriz. Ama bu çok basit bir yaklaşım olur. Bu konuda aslında pek çok kaynakta yazılmakta ve çizilmektedir. Ancak ürün henüz nihai halini almadığından sürekli olarak değişimlere uğramaktadır. Söz gelimi daha önce yayınlanan SDK içerisine Microsoft.Net Services bloku içerisinde yer alan Workflow Services modeli, Temmuz 2009' da yayınlanan CTP sürümünde kaldırılmışdır. Aslında konunun detaylarına girdiğimde çok kısa bir sürede kaybolduğumu ifade edebilirim. Bu nedenle yazımızın bundan sonraki kısımlarında mimarinin ne getirdiğini sizlere araştırma sonuçlarımdan aktarmaya çalışacağım.

Windows Azure Service Platformu iki ana parçayı içermektedir. İlk parça olan Windows Azure, bulut tabanlı işletim sistemidir. Bu işletim sistemi üzerinde ise çeşitli servis inşa blokları (Building Blocks) yer alır (Microsoft SQL Services, Microsoft.Net Services, Live Services, Microsoft Sharepoint Services, Microsoft Dynamics CRM Services) gibi. Aşağıdaki şekilde söz konusu mimari model daha net görülebilmektedir.

![blg91_Architecture.gif](/assets/images/2009/blg91_Architecture.gif)

Windows Azure platformunu aslında şu cümle ile tanımlayabiliriz; Azure, internet tabanlı bulut bilişim ortamını (Internet Based Cloud Computing) sağlayarak, uygulamaların çalıştırılması veya verinin saklanması için Microsoft veri merkezlerinin (Data Centers) kullanılabilmesini sağlayan bir platformdur. Bu tanımlamaya göre Windows Azure Service platformunu bir Cloud Computing Fabrikası olarak düşünülebilir. Öyleki bir veri merkezi olarak, uygulama veya servislerin Internet üzerinde geliştirilmesi (Development), dağıtılması (Deployement, yönetilmesi (Management) mümkündür. Bu açıdan bakıldığında Windows Azure Service platformunun iki temel fonksiyonelliği olduğu söylenebilir. Uygulama çalıştırmak ve veri saklamak.

Peki Internet üzerinde yer alan böyle bir bulut kümesinin ne gibi bir faydası olabilir? Herşeyden önce uygulamaların çalıştırılacağı verinin fiziki olarak saklanacağı ortama ait IT karmaşıklığını düşünememize gerek kalmamaktadır. Bir başka deyişle host ortamının fiziki sunucu özelliklerinin ayarlanması, oluşturulması, bunlar için gerekli yazılımların tesis edilmesi gibi maliyetlerin düşünülmesine gerek kalmamaktadır. Üstelik Cloud içerisine dahil edilen uygulama veya veri kümelerinin istenen anda istenilen yerden kullanılabilmesi de mümkündür.

Windows Azure tarafında önem arz eden kavramlardan olan veri depolama (Data Storage) bloku, BLOB (Binary Large OBjects), kuyruk (Queue) ve basit tablo (Simple Table) verilerinin saklanması için gerekli servisleri sunmaktadır. Ancak aksine ilişkisel veritabanları tarafından sunulan, sorgulama (query), arama (Search),raporlama (Reporting) vb yetenekleri sağlamamaktadır. Bu yeteneklerin, Cloud üzerinde saklanan veri kümelerinde kullanılabilmesi için Microsoft SQL Services'lerden yararlanılması gerekmektedir.

Microsoft.Net Services şu anda 2 temel fonksiyonelliği karşılamaktadır. Uygulamalar arası bağlanabilirlik (Application Connectivity) ve Erişim kontrolü (Access Control). Uygulamalar arası bağlanılabilirlik için Microsoft.NET Services Bus bloku kullanılmaktadır. Bu blok, çeşitli mesajlaşma desenlerine göre Internet üzerindeki uygulamalar arasında bir network altyapısının oluşturulabilmesini sağlamaktadır. Erişim kontrolü için Microsoft.NET Access Control Service bloku ele alınır. Bu blok ile aslında Claims tabanlı erişim kontrolünün Cloud üzerinde gerçeklenmesi sağlanır.

Aslında Microsoft.Net Service'leri, bütünüyle servis bazlı geliştirme fabrikası olarak düşünülebilir. Microsoft.Net Service'lerini geliştirirken kullanılan SDK, WCF ile entegrasyon da sağlamaktadır. Bu sayede var olan.Net geliştirme teknikleri ile tasarlanan WCF uygulamalarının Cloud servisi haline getirilmesi mümkündür. Önemli olan noktalardan bir diğeride, Microsoft.Net Service'in sadece.Net geliştiriciler için tasarlanmış olmayışıdır. Desteklenen REST, SOAP, WS gibi protokoller sayesinde Java ve Ruby gibi diller için geliştirilmiş olan SDK'lardan yararlanarak da Cloud servisleri geliştirilebilir.

[![blg91_Book2.jpg](/assets/images/2009/blg91_Book2.jpg)](http://www.amazon.com/gp/product/0470506385/ref=ox_ya_oh_product)

Windows Azure platformu ile ilişkili olarak başlangıç noktamız [http://www.microsoft.com/windowsazure/](http://www.microsoft.com/windowsazure/) adresi olmalıdır. Bu adresten gerekli SDK ve Visual Studio 2008/2010 Beta X araçlarının indirilierek kurulması geliştirilmeye başlanması için yeterlidir. Ek olarak örneğin.Net Services geliştirilmesi yapılacaksa bu konu ile ilişkili SDK'nın [http://www.microsoft.com/windowsazure/developers/dotnetservices/](http://www.microsoft.com/windowsazure/developers/dotnetservices/) adresinden tedarik edilmesi gerekmektedir.

Ben bu konuya oldukça meraklıyım aslında. Nitekim işin içerisinde Service kavramı var

![Cool](/assets/images/2009/smiley-cool.gif)

Bu nedenle bende boş durmadım ve önümüzdeki sene Cloud Computing ve Windows Azure ile ilişkili olan araştırmalarımı daha sağlıklı devam ettirebilmek adına Amazon'dan [Cloud Computing with the Azure Platform](http://www.amazon.com/Cloud-Computing-Windows-Platform-Programmer/dp/0470506385/ref=sr_1_1?ie=UTF8&s=books&qid=1256228080&sr=8-1)adlı kitabı sipariş ettim. Bakalım önümüzdeki sene Azure ile ilişkili olarak ne gibi bir macera beni (ve doğal olarak siz değerli okurlarımı) bekliyor olacak. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.