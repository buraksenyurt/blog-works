---
layout: post
title: ".Net RIA Servisleri Nedir?"
date: 2009-05-08 13:41:00 +0300
categories:
  - dotnet-ria-services
tags:
  - dotnet-ria-services
  - dotnet
  - aspnet
  - ado-net
  - entity-framework
  - linq
  - wcf
  - workflow-foundation
  - silverlight
  - rest
  - http
  - authentication
  - authorization
  - javascript
  - visual-studio
---
Son yıllarda bildiğiniz üzere Servis Tabanlı Uygulamalar (Service Oriented Applications) hayatımızda oldukça fazla yer kaplamaya başladı. Microsoft cephesinden olaya baktığımızda, en büyük sıçramanın Windows Communication Foundation ile.Net Framework 3.0' da yaşandığını söyleyebiliriz. WCF'in getirdiği servis bazlı uygulama geliştirme yaklaşımı,.Net Framework 3.5 ile dahada zenginleşti. Eklenen Web programlama modeli (Web Programming Model) özellikleri sayesinde, REST (Representational State Transfer) bazlı servislerin geliştirilebilmesinin yolu açıldı. Sonrasında Workflow Foundation ile iç içe geçen WCF özellikleri sayesinde, iş akışlarının farklı domainler ile haberleşebilmesi veya servis gibi sunulabilmesi olanaklı hale geldi. Derken.Net Framework 3.5 Service Pack 1 ile hayatımıza başka bir kavram daha girdi. Ado.Net Data Services.

Bu model ile, Ado.Net Entity Framework veya LINQ (Language INtegrated Query) bazlı sağlayıcılar üzerinden verinin REST tabanlı olarak sunulabilmesi mümkün hale geldi.Tabi bu geçişler sırasında Client Application Services ve Azure gibi kavramlar ile geliştiricinin hayatını kolaylaştıran REST Starter Kit gibi pek çok yeni fikir ve vizyon ile karşılaştık. Ama Microsoft cephesindeki yenilikler tüm hızıyla sürmeye devam etti, ediyor, edecek...

![Laughing](/assets/images/2009/smiley-laughing.gif)

Bir süredir.Net Framework 4.0 ve bu etapta WF 4.0&WCF 4.0 yeniliklerini incelemekteyim. Ancak arada kaçırdığım önemli bir konu var..Net RIA (Rich Internet Application) Services ve Silverlight

![Embarassed](/assets/images/2009/smiley-embarassed.gif)

Dolayısıyla bu yazımda sizlere,.Net RIA Servisleri ile ilişkili öğrendiklerimi ve bilgilerimi aktarmaya çalışıyor olacağım.

En nihayetinde, Silverlight sayesinde istemci tarafında çok zengin içeriklere sahip olabilecek ve tarayıcı tabanlı (ve hatta Silverlight 3.0 sonrası masaüstü...) uygulamaların geliştirilmesi mümkün. Ancak Silverlight gibi bir uygulama geliştirme modelinde, istemcinin sunucu üzerinde yer alan bazı veri kaynaklarına erişmesi için, servislerin kullanılmasıda kaçınılmaz bir gerçek. (Nedenini biraz sonra daha iyi anlatabileceğim.)

Özellikle Silverlight 3.0 ve.Net RIA Service çıkana kadar, geliştiricilerin sunucu verilerine erişmesi için biraz daha fazla kodlama yapması gerekmektedir. Aslında olaya sadece Silverlight değil, Asp.Net Ajax gibi istemciler açısından bakıldığında da, benzer kodlama süreçleri söz konusudur. Bu tip RIA uygulamalarını, n-tier tarzı mimariler ile geliştirmek istediğimizden, aslında sunum (Presentation) katmanının standart Asp.Net modelinden farklı olarak, tamamen istemci tarafına yıkıldığı oldukça önemli bir noktadır. Sanıyorum burada biraz kafaları karıştırdım.

![Undecided](/assets/images/2009/smiley-undecided.gif)

Gelin olayı standart n-tier modelin Asp.Net uygulamalarındaki genel kullanımı ile analiz etmeye başlayalım. Aşağıdaki şekilde bu model vurgulanmaya çalışılmaktadır.

![blg14_1.gif](/assets/images/2009/blg14_1.gif)

Klasik olarak bir Asp.Net Web uygulamasında (çoğunlukla Asp.Net Ajax içinde benzer durum söz konusudur), katmanların tamamı sunucu üzerinde yer alır. Uygulama mantığı (Application Logic-Business Layer), veriye erişim katmanı (Data Access Layer) ve istemcinin göreceği HTML çıktının üretileceği sunum katmanı (Presentation Layer). Bunlara ek olarak web uygulaması içerisinde, veri erişim katmanından dış servisler yardımıyla farklı kaynaklara gidilebilir veya uygulamanın kendisinin farklı alanlardaki programlara sunacağı bir takım hizmetler/servisler olabilir. Oldukça basit ve kullanışlı.

Ancak, günümüz uygulamalarında ve özellikle son yıllarda kullanıcı deneyimini (User Experience) zenginleştirecek şekilde yapılan bir çok atılım vardır. (Bu etkileşim özellikle web tabanlı mimarilerde kendini daha da ön plana çıkarmaktayken, geliştirme süreçlerinin standart masaüstü uygulamalara nazaran daha karmaşık ve zor olduğuda söylenebilir.) Bu nedenle tarayıcı uygulamalar üzerindeki kullanıcı deneyimini zenginleştirecek Silverlight gibi geliştirme ortamları söz konusudur. Hal böyle olunca yukarıdaki şekilde çizdiğimiz katmanlı model biraz daha değişim göstermektedir. Aşağıdaki şekilde olduğu gibi.

![blg14_2.gif](/assets/images/2009/blg14_2.gif)

Zengin internet uygulamalarında, sunum katmanı/mantığı istemci tarafına yıkılmaktadır (Hatırlayalım, Silverlight uygulamalarının çalıştırılması için istemci tarafında minik bir framework, add-in tarzında yüklenmiş olmalıdır). Bu da kullanıcı etkileşimini dahada üst seviyeye çıkartmak anlamına gelmektedir. Ama doğal olarak n-tier modelde sunum katmanı ile uygulama mantığı arasına internet ağının girmesi gerekmektedir. Buna göre RIA'ları basit bir istemci uygulamadan ziyade, sunucu bileşenlerinide içeren birer Internet uygulaması olarak düşünmek gerekmektedir. Hal böyle olunca, sunucu tarafındaki veri kaynaklarının sunum tarafında kullanılabilmesinde servisler önemli bir rol üstlenmektedir.

.Net RIA Servislerine kadarki zaman diliminde, geliştiricilerin bu anlamda düşünmesi gereken pek çok kıstas vardır. Herşeyden önce veriyi istemci tarafına taşıyacak servisin ve metodlarının yazılması gerekir. Ayrıca istemci tarafında, bu servisin kullanılabilmesi için gerekli proxy üretiminin yapılması şarttır. Silverlight tarafında kolay olan proxy üretimi, Asp.Net Ajax tarafı düşünüldüğünde ek javascript kütüphaneleri anlamına gelmektedir. Yinede, sunucu tarafında Ado.Net Entity Framework veya LINQ to SQL gibi modelleri kullanabileceğimizden bu zahmete girmeye değmektedir. Microsoft'un söz konusu servislerin, n-tier içerisindeki uyarlanışını daha da kolaylaştırmak adına.Net RIA Servislerini geliştirdiğini söyleyebiliriz..Net RIA Servisleri kavramsal olarak iki ana parçadan oluşur.

![blg14_3.gif](/assets/images/2009/blg14_3.gif)

DataService sınıfı aslında, temel CRUD (CreateRetrieveUpdateDelete) işlemlerini ve özel bir takım operasyonları içerebilir. Bunlara ek olarak doğrulama (Validation), yetkilendirme (Authorization) gibi kısıtlarıda ele alabilir. Bu nedenle DataService sınıfının, veri için ele alınacak iş mantığını içerdiğini söyleyebiliriz. DataService sınıfı genel olarak arka planda, hazır olan (built-in) veri modellerini kullanır. Yani Ado.Net Entity Framework veya LINQ to SQL burada göz önüne alınabilir. Elbetteki diğer veri kaynaklarıda gerek servisler, gerek özel kodlamalar yardımıyla kullanılabilir.

İkinci bölümde yer alan DataContext sınıfı ise, servislerin istemciye sunduğu verilerin, tip bazındaki karşılıklarını içermektedir. Bu nedenle istemci tarafında, verilerin yüklenmesi, üzerinde yapılan değişikliklerin tekrardan sunucu tarafına gönderilmesi için gerekli kodlamaları ve metodlarıda hazır olarak içermektedir. Tahmin edeceğiniz üzere,.Net RIA Servislerinin Visual Studio 2008 ortamında geliştirilmesi son derece kolay ve basittir.

![Laughing](/assets/images/2009/smiley-laughing.gif)

Son olarak.Net RIA Servisleri ile ilişkili olaraktan merak edilen sorulara cevap bulabileceğiniz ve gerekli yüklemeleri edinebileceğini bir [internet adresini](http://silverlight.net/forums/t/80529.aspx) paylaşmak isterim.

Böylece geldik bir yazımızın daha sonuna. Bu yazımda sizlere.Net RIA Servislerini, anladığım kadarıyla anlatmaya çalıştım. Bir sonraki yazımızda basit bir örnek geliştirerek Merhaba.Net RIA Servisi diyeceğiz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.