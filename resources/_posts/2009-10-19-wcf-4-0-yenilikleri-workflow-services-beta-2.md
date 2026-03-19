---
layout: post
title: "WCF 4.0 Yenilikleri - Workflow Services [Beta 2]"
date: 2009-10-19 13:22:00 +0300
description: "Merhaba Arkadaşlar,"
categories:
  - wcf-4-0-beta-2
tags:
  - wcf-4-0-beta-2
  - wcf
  - workflow-foundation
  - xml
  - xaml
  - http
  - visual-studio
---
# WCF 4.0 Yenilikleri - Workflow Services [Beta 2]
![blg78_Giris.jpg](/assets/images/2009/blg78_Giris.jpg)

Merhaba Arkadaşlar,

WCF ve WF arasında ilişkiyi anlatan güzel bir cümle vardır..Net Framework 3.0' da arkadaş olan WCF ve WF,.Net Framework 3.5 sürümünde nişanlanmış,.Net Framework 4.0 sürümünde ise evlenmişlerdir.

![Wink](/assets/images/2009/smiley-wink.gif)

Bu ikilinin bir arada ele alınması sonucu Workflow Services adı verilen bir ufaklıkta ortaya çıkmıştır. Aslında bir akışın servis bazlı olması son derece önemlidir. Nitekim WF tarafında, uzun süreli işlemlerin ele alınması (Long Running Process), akışın çeşitli noktalarından farklı anlar için kalıcı olarak saklanabilmesi (Persistence), asenkron çalışma zamanı motorunun zaten hazır olarak bulunması söz konusudur. Bu özelliklerin WCF Servis Noktaları (Endpoints) ile desteklenmesi, bir akışın ağ üzerindeki istemciler tarafından başlatılabilmesine olanak tanımaktadır. Üstelik bu akışlar sonuçlar üreterek bunları geriye de döndürülebilir. Bu durum aslında WCF 3.5 sürümünde de gerçeklenebilmektedir. Ne varki, WCF 4.0 tarafında tamamen declerative olarak tanımlanabilen ve saf XAML (eXtensible Application Markup Language) içeriğinden oluşan bir Workflow Servisinin geliştirilebilmesi mümkündür. Üstelik Visual Studio 2010 WF Designer sayesinde bu XAML içeriğinin üretilmesi için IDE desteği de sunulmaktadır.

Kişisel Not: Windows XP, Vista ve 7 sürümlerinde kullandığım Visual Studio 2010 Beta 1 ürününde, WF Designer sürekli olarak hata verip IDE'yi Restart işlemine zorlamaktaydı.![Undecided](/assets/images/2009/smiley-undecided.gif) Neyseki bu sorun Visual Studio 2010 Beta 2 sürümünde ortadan kaldırılmış durumdadır.![Wink](/assets/images/2009/smiley-wink.gif)

Dolayısıyla bu yazımızın konusu şimdiden belli oldu. Çok basit ve büyük ihtimalle pek işe yaramayan bir Workflow Service'in nasıl geliştirilebileceğini incelemeye çalışıyor olacağız. İşe ilk olarak Declerative Sequential Service Library tipinden bir WCF projesi oluşturarak başlayacağız. Aşağıdaki ekran görüntüsünde olduğu gibi.

![blg78_NewProject.gif](/assets/images/2009/blg78_NewProject.gif)

Örnek olarak AdventureWFServices ismiyle oluşturduğumuz projede Service1.Xamlx dosyası hemen dikkati çekmektedir. Bu örnek olarak oluşturulan Workflow Service örneğidir. Yani Microsoft tarafından sunulan hazır Hello World örneği

![Wink](/assets/images/2009/smiley-wink.gif)

Tamamen XAML içeriğinden oluşmaktadır ve istemcilere servis olarak nasıl sunulacağına ilişkin çalışma zamanı bilgileri (WCF Servis konfigurasyon bilgileri) Web.config dosyasında tutulmaktadır. (Bu bir WCF kütüphanesi projesi olduğundan doğrudan çalıştırılabilir ve Web.config içerisindeki ayarlar sayesinde HTTP bazlı olarak host edilir. Diğer taraftan test etmek için WcfTestClient aracından kolayca yararlanılabilir.)

![blg78_FirstScreen.gif](/assets/images/2009/blg78_FirstScreen.gif)

Biz tabiki bu içerik yerine kendi Workflow Service örneğimizi kullanacağız. Ama öncesinde ReceiveRequest ve SendResponse isimli bileşenlerin ne işe yaradıklarını kısaca anlamaya çalışmakta yarar vardır. ReceiveRequest isimli bileşen aslında Receive tipinden bir aktivitedir. Benzer şekilde SendResponse isimli bileşende, SendReply tipinden bir aktivitedir. Tahmin edileceği üzere Receive aktivitesi ile, istemci tarafından gelen talep (Request) alınmakta ve SendReply aktivitesi yardımıylada bir cevap (Response) döndürülmektedir.(Bu iki bileşen arasında ise istenen bir akışın yürütüldüğü düşünülebilir) Bir başka deyişle WCF bazlı mesaj alınması ve cevap döndürülmesi amacıyla kullanılan iki aktivite tipi söz konusudur. ReceiveRequest isimli bileşen aynı zamanda istemci tarafından çağırabilecek bir operasyon sunduğundan OperationName isimli bir özelliği de sahiptir. Diğer taraftan istemci tarafından gelecek olan talep içerisindeki değişkenler Content özelliği tarafından taşınabilirler ki bu özellik aslında bir koleksiyon olarak düşünülebilir. Çünkü operasyonun birden fazla parametre alması gerekebilir. Receive aktivitesi için önem arz eden noktalardan biriside, CanCreateInstance özelliğinin değeridir. Bu değer örneğimizde true olarak set edilmiştir ve söz konusu aktiviteye bir talep gelmesi ile birlikte Workflow örneğinin oluşturulup oluşturulmayacağını işaret etmektedir. Önemli olan noktalardan biriside, SendResponse isimli bileşenin Request özelliğinin değerinin, Receive tipinden olan ReceiveRequest isimli bileşen olmasıdır. Dolayısıyla bu iki mesajlaşma aktivitesinin birbirleriyle haberleştiklerini söyleyebiliriz. (İlerki yazılarımızda burada sözü edilen korelasyon konusuna değiniyor olacağız ki XAML içeriğinde bu konunun izlerini görmekteyiz.)

Bu arada Sequential Service tipinin üzerinde tanımlanmış bazı değişkenler (Variables) olduğu görülebilir.

![blg78_Variables.gif](/assets/images/2009/blg78_Variables.gif)

data isimli Int32 tipinden olan değişken, Receive aktivitesi için Content değerini de temsil etmektedir. Diğer taraftan, SendReply aktivitesine ait Content özelliğinde kullanılmış ve string karşılığı üretilerek geriye döndürülmüştür. Buna göre akışın senaryosunu şu şekilde düşünebiliriz. İstemci talep olarak Int32 tipinden bir değer gönderir. Bu değer ReceiveRequest isimli aktivite bileşeni tarafından alınır ve üst aktivitesi olan Sequential Service'in data değişkenine atanır. Bu değişken SendResponse isimli aktivite bileşeninin de ulaşabileceği kapsamda (Scope) olduğundan, istemciye geriye gönderilecek cevabın içeriğinde kullanılabilir. Şimdi bu bilgilerden yola çıkarak kendi Workflow Servisimizi oluşturalım.

Örnek olarak istemciden gelen iki sayının toplamını bulup geriye bu sonucu döndürecek bir Workflow Servis geliştirmeyi planlayabiliriz. Böylece XAML içeriğini anlamamız daha kolay olacaktır. Bu amaçla projemize yeni bir Declerative Sequential Service öğesi ekleyerek devam edebiliriz.

![blg78_NewService.gif](/assets/images/2009/blg78_NewService.gif)

MathService.xamlx servisinin tasarım görüntüsü aşağıdaki gibidir.

![blg78_MathServiceDsgn.gif](/assets/images/2009/blg78_MathServiceDsgn.gif)

Aslında global seviyede tanımladığımız GlobalX ve GlobalY isimli iki değişken haricinde çok fazla detay görülmemektedir. Bu nedenle konuyu anlamanın en iyi yolu XAML içeriğine bakmamız olacaktır (Zaten ilerleyen zamanlarda bu blog girişinin destekleyici görsel videosunuda yayınlıyor olacağım) İşte MathService.xamlx içeriği;

sad:XamlDebuggerXmlReader.FileName="C:\My Dot Net World\Projects\2010\WCF\AdventureWFServices\AdventureWFServices\MathService.xamlx" sad1:VirtualizedContainerService.HintSize="303,348.553333333333" mva:VisualBasic.Settings="Assembly references and imported namespaces serialized as XML namespaces">

True

sad1:VirtualizedContainerService.HintSize="257,85.2766666666667" OperationName="Sum" ServiceContractName="p1:IMathService">

[GlobalX]
[GlobalY]

sad1:VirtualizedContainerService.HintSize="257,85.2766666666667">

[GlobalX + GlobalY]

İlk etapta biraz korkutucu görünebilir.

![Sealed](/assets/images/2009/smiley-sealed.gif)

Ancak şu an için üzerinde duracağımız önemli noktalar Bold olarak işaretlenmiştir. Sequence.Variables elementi içerisinde 3 değişken tanımı görülmektedir. Bizim eklediklerimiz GlobalX ve GlobalY isimli int tipinden olanlardır. Bu değişkenler Sequence elementi içerisinde tanımlandıklarından, takip eden Receive ve SendReply elementleri veya gelecek başka aktiviteler tarafından ortaklaşa kullanılabilirler. Receive elementinde operasyon ile ilişkili bilgiler dışında, servisin sözleşme ismide belirtilmektedir. Receive aktivitesi için en önemli parça ReceiveParametersContent kısmıdır. Burada X ve Y isimli OutArgument tipinden argümanlar tanımlamıştır. Bu argümanlarda önemli olan kısım GlobalX ve GlobalY atamalarıdır. X ve Y aslında istemcinin çağıracağı servis operasyonunun parametreleridir. İstemciden gelen bu değerler, aktivitenin GlobalX ve GlobalY değerlerine set edilmektedir. SendReply aktivitesi içerisinde yer alan SendParametersContent kısmında ise InArgument tipinden bir argüman tanımlanmıştır. Result ismi ile tanımlanan bu argümanın içeriği, GlobalX ve GlobalY değerlerinin toplamından oluşmaktadır. Bir başka deyişle istemci tarafına döndürelecek cevap içeriği belirlenmiş olur. İçeriğin tipi ise TypeArguments niteliğine atanan Int32 yapısıdır. Görüldüğü üzere herhangibir kod dosyası kullanılmamıştır. Tüm işlemler XAML bazlı olaraktan tanımlanabilmektedir. Buda çalışma zamanı için önemli bir esnekliktir.

Oluşturduğumuz sınıf kütüphanesinin Web.config dosyasının içeriği ise son derece sadedir.

Görüldüğü gibi belirli bir Endpoint tanımlaması yoktur. Ancak exception detayının gönderilmesi ve Metadata bilgisinin elde edilebilmesi için gerekli davranışlar (Behaviors) tanımlanmıştır. Buna göre Workflow Servisimiz, HTTP bazlı olaraktan host edilecektir. Uygulamayı F5 ile başlattığımızda ve MathService.xamlx içeriğini talep ettiğimizde aşağıdaki ekran görüntüsü ile karşılaşırız.

![blg78_Runtime.gif](/assets/images/2009/blg78_Runtime.gif)

Görüldüğü üzere WSDL içeriğide doğrudan talep edilebilmektedir. Buda istemciler için gerekli Proxy üretiminin kolayca yapılabileceği anlamına gelmektedir. Burada WSDL içeriğinide inceleyebiliriz.

![Sealed](/assets/images/2009/smiley-sealed.gif)

Ancak benim özellikle göstermek istediğim nokta http://localhost:65193/MathService.xaml?xsd=xsd1 talebinin sonucudur. Nitekim WSDL dökümanına baktığımızda, Sum isimli operasyonun kullandığı X ve Y parametrelerini göremeyiz. Ancak bu parametrelerin tanımlandığı XSD içeriğine bir referans bildirildiğini fark edebiliriz. Bu XSD içeriğinde aşağıdaki ekran görüntüsünde olduğu gibi, X,Y ile istemci tarafına döndürelecek Result değişkenlerine ait tanımlamalar yer almaktadır.

![blg78_Xsd1.gif](/assets/images/2009/blg78_Xsd1.gif)

Dilerseniz artık servisimizi test edelim. Bu amaçla daha önceden de belirttiğimiz gibi WcfTestClient aracını kullanabiliriz. İşte örnek bir toplama işlemi;

![blg78_ClientRuntime.gif](/assets/images/2009/blg78_ClientRuntime.gif)

İlk olarak Request için X ve Y değerleri girilir sonrasında Invoke düğmesine tıklanır. Bir süre beklemenin ardından servisten geri dönüş elde edilir. BasicHttpBinding tabanlı olarak host edilen Workflow Servisimiz başarılı bir şekilde çalıştırılmıştır. Yapılan Invoke işlemi sonrası üretilen XML içeriğine bakıldığında ise aşağıdaki çıktının oluştuğu görülebilir.

![blg78_ClientXml.gif](/assets/images/2009/blg78_ClientXml.gif)

İşte bu kadar.

![Smile](/assets/images/2009/smiley-smile.gif)

Workflow Servisleri sayesinde bir iş akışının servis bazlı olarak başlatılabilmesi ve WF Çalışma zamanının nimetlerinden yararlanabilmesi mümkün olmaktadır. Üstelik,.Net 4.0 tarafında gelen yeni modele göre söz konusu Workflow Servislerinin tamamen XAML içeriğinden oluşacak şekilde dekleratif olarak tanımlanabilmesi söz konusudur. Bu da çalışma zamanında koda müdahale etmeye gerek duymadan akış ile ilişkili bazı değişikliklerin yapılabileceğinin bir göstergesidir. Bakalım WCF&WF kardeşliğinde başka ne gibi yenilikler bizleri bekliyor. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[AdventureWFServices.rar (442,66 kb)](/assets/files/2009/AdventureWFServices.rar)