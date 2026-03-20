---
layout: post
title: "WCF 4.0 Yenilikleri - Artık Svc Uzantısına Gerek Yok [Beta 1]"
date: 2009-08-18 03:45:00 +0300
categories:
  - wcf-4-0-beta-1
tags:
  - wcf-4-0-beta-1
  - csharp
  - dotnet
  - aspnet
  - wcf
  - soap
  - http
  - iis
---
Nihayet WCF 4.0 için basitleştirilmiş konfigurasyon (Simplified Configuration) yeniliklerinden sonuncusuna değineceğimiz blog girişimize ulaştık. Tabiki WCF 4.0 tarafındaki diğer yenilikleride zaman içerisinde inceliyoruz olacağız. Örneğin Discovery, Routing, RESTful geliştirmeleri vb...Ancak diğer köklü değişikliklere başlamadan önce konfigurasyon tarafına son noktayı koyalım artık.

![Wink](/assets/images/2009/smiley-wink.gif)

Bir önceki blog yazımızdan hatırlayacağınız üzere Asp.Net tabanlı olarak host edilen WCF servislerinde, tek bir svc dosyası ilede yayınlama yapabileceğimizi görmüştük. Asp.Net hosting tarafını ilgilendiren bir diğer yenilik ise Url Rewriting konusunun Svc dosyaları için de uyarlanabilir olmasıdır. Bildiğiniz gibi özellikle RESTful servislerde, URL satırında yer alan bilginin daha okunaklı ve anlaşılır olması söz konusudur ve önemlidir. Genellikle IIS tarafında veya kod yardımıyla gerçekleştirilebilecek bu işlemler için WCF tarafı konfigurasyon bazında bir kolaylık getirmektedir. Yine bir önceki blog yazımızda yer alan örneği göz önüne alırsak son hali ile aşağıdaki gibi çalıştırıldığını hatırlayabiliriz.

![blg65_RuntimeMetadata.gif](/assets/images/2009/blg65_RuntimeMetadata.gif)

Burada görüldüğü gibi servisin adresi http://localhost:53513/AdventureService.svc şeklindedir. Bir başka deyişle tipik bir dosya uzantısı talebi (svc file request) ifade edilmektedir. Ancak istenirse servise olan talebin,

http://localhost:53513/Companies/Adventure/ProductInformations

şeklindeki bir URL tanımlaması ile olmasıda sağlanabilir. Bu yazım, okunaklığı ve herşeyden önemlisi servis amacını çok daha net ifade edebilecek bir model sunmaktadır. Peki bu tanımlamanın WCF tarafındaki geçerliliği özel kod yazmadan veya IIS'e bulaşmadan nasıl sağlanabilir?

WCF 4.0 açısından olaya bakıldığında yapılması gereken tek şey web.config dosyası içerisinde aşağıdaki eklentilerin yapılmasıdır.

```csharp
<system.serviceModel>
    <behaviors>
      <serviceBehaviors>
        <behavior>
          <serviceMetadata httpGetEnabled="true"/>
        </behavior>
      </serviceBehaviors>
    </behaviors>
    <serviceHostingEnvironment>
      <serviceActivations>
        <add relativeAddress="/Companies/Adventure/ProductInformations" service="AdventureService"/>
      </serviceActivations>
    </serviceHostingEnvironment>
  </system.serviceModel>
```

Görüldüğü gibi serviceHostingEnvironment elementi içerisinde yer alan serviceActivations boğumuna yeni bir Relative Address bilgisi eklenmiştir. Buna göre /Companies/Adventure/ProductInformations bilgisi aslında AdventureService isimli servisi işaret etmektedir. Dolayısıyla WCF çalışma zamanı (Runtime) gelen talepteki URL bilgisini değerlendirip hangi servisi ayağa kaldırması/çalıştırması gerektiğini söz konusu konfigurasyon içeriğinden anlayabilmektedir. Geliştirdiğimiz örneğin bu şekilde çalıştırılması halinde aşağıdaki ekran görüntüsünde yer alan sonuçlar ile karşılaşırız.

![blg66_1.gif](/assets/images/2009/blg66_1.gif)

Hatta WSDL içeriğide aynı adresleme formatı üzerinden elde edilebilmektedir. Çok doğal olarak WSDL içeriğinde de SOAP adres tanımlamaları ile ilişkili bölümlerde yeni adresleme bilgisi kullanılmaktadır.

![blg66_2.gif](/assets/images/2009/blg66_2.gif)

Sırada Discovery, RESTFul ve Routing gibi yeniliklerin incelenmesi var. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[SimpleAspNetHosting2.rar (13,94 kb)](/assets/files/2009/SimpleAspNetHosting2.rar)
