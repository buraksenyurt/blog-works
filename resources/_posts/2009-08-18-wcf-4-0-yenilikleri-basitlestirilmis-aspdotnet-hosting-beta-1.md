---
layout: post
title: "WCF 4.0 Yenilikleri - Basitleştirilmiş Asp.Net Hosting [Beta 1]"
date: 2009-08-18 03:10:00 +0300
categories:
  - wcf-4-0-beta-1
tags:
  - wcf-4-0-beta-1
  - csharp
  - xml
  - dotnet
  - aspnet
  - wcf
  - web-service
  - xml-web-services
  - http
  - serialization
  - visual-studio
---
WCF 4.0 ile birlikte gelen yenilikler bitmek bilmiyor.

![Smile](/assets/images/2009/smiley-smile.gif)

Aslında irili ufaklı bu değişikliklerin ilk bölümünde daha çok basitleştirilmiş konfigurasyon (Simplified Configuration) özellikleri üzerinde durmaya çalışıyoruz. Bu değişiklikler irili ufaklı olsalarda WCF çalışma zamanında (WCF Runtime) ciddi geliştirmelerin yapıldığını göstermektedir. Gelen değişikliklerden biriside Asp.Net Hosting tarafındadır. Aslında konunun sonuna geldiğimizde "Ben bunu bir yerlerden hatırlıyorum" diyebilirsiniz.

![Wink](/assets/images/2009/smiley-wink.gif)

İlk olarak.Net Framework 4.0 öncesinde Asp.Net Hosting tabanlı olaraktan bir WCF servisini sunmak için neler yaptığımıza bir bakalım;

Svc uzantılı bir dosya oluşturulur.
Svc uzantılı dosyanın code-behind parçasında gerekli geliştirmeler yapılır. Burada servis sözleşmesi (Service Contract) için bir interface tanımlaması ve servisi uygulayan tipin kendisinin yazılması söz konusudur. Elbette arayüzün ServiceContract, operasyonlarının OperationContract, eğer operasyon dönüş tipi olarak kullanılan serileştirilebilir tipler var ise DataContract ve özelliklerininde DataMember nitelikleri ile işaretlendiğini hatırlayalım.
Web.config dosyası içerisinde system.serviceModel elementlerinde gerekli bildirimler yazpılır. (Endpoint, behavior tanımlamaları vb...)

WCF 4.0 tarafında ise sadece aşağıdaki gibi bir svc dosyası oluşturulması yeterlidir.

Solution içerisindeki örnek görüntü;

![blg65_SingleSvc.gif](/assets/images/2009/blg65_SingleSvc.gif)

AdventureService.svc isimli dosya içeriği;

```csharp
<%@ ServiceHost Language="C#" Service="AdventureService"  %>

[System.ServiceModel.ServiceContract]
public class AdventureService
{
    [System.ServiceModel.OperationContract]
    public double MostExpensiveProduct(int categoryId)
    {
        return 1000.00;
    }

    [System.ServiceModel.OperationContract]
    public Product GetMostExpensiveProduct(int categoryId)
    {
        return new Product { ProductId = 1001, Name = "Ferrari" };
    }
}

[System.Runtime.Serialization.DataContract]
public class Product
{
    [System.Runtime.Serialization.DataMember]
    public int ProductId { get; set; }
    [System.Runtime.Serialization.DataMember]
    public string Name { get; set; }
}
```

Konfigurasyon (web.config) dosyası olmasına ve içinde system.serviceModel elementlerinin bildirimine gerek yoktur. Servis sözleşmesi (Service Contract) için herhangibir Interface tanımlaması yapılmamıştır. Örneği bu şekilde doğrudan çalıştırabiliriz.(Örneği.Net Framework 4.0 Beta 1 üzerinde Visual Studio 2010 Beta 1 ile geliştirdiğmizi hatırlatmak isterim.)

![blg65_Runtime.gif](/assets/images/2009/blg65_Runtime.gif)

Gördüğünüz gibi servis başarılı bir şekilde çalışmaktadır. Elbette istenirse bazı özel ayarlar için web.config dosyasında system.serviceModel üzerinde geliştirmelerin yapılması gerekebilir. Söz gelimi şu anda çalışmakta olan servisimiz HTTP bazlı metadata publishing yapmamaktadır. Bir başka deyişle istemcilerin proxy üretimi için servisin metadata bilgilerine ulaşmaları engellenmiştir. Bu durumda yine WCF 4.0 ile gelen kolaylıklardan yararlanabiliriz (Name gibi niteliklerin kullanılma zorunluluğunun ortadan kaldırıldığını hatırlayalım) Buna göre uygulamaya bir web.config dosyası ilave edip, system.serviceModel elementini aşağıdaki gibi geliştirmemiz yeterlidir.

```xml
<system.serviceModel>
    <behaviors>
      <serviceBehaviors>
        <behavior>
          <serviceMetadata httpGetEnabled="true"/>
        </behavior>
      </serviceBehaviors>
    </behaviors>
  </system.serviceModel>
```

Bu durumda uygulamayı tekrar çalıştırdığımızda, proxy üretimi için metadata yayınlamasının etkineştirildiğini görebiliriz.

![blg65_RuntimeMetadata.gif](/assets/images/2009/blg65_RuntimeMetadata.gif)

Hatta WSDL içeriğine bakacak olursak, varsayılan Endpoint bilgisininde aşağıdaki şekilde olduğu gibi eklendiğini görebiliriz.

![blg65_DefaultEndpoint2.gif](/assets/images/2009/blg65_DefaultEndpoint2.gif)

Peki bu yenilikler size neyi çağrıştırıyor?

![Wink](/assets/images/2009/smiley-wink.gif)

Xml Web Servislerini hatırlayın. Sadece WebService ve WebMethod nitelikleri (attribute) ile imzalanan tip ve üyeleri söz konusudur. Web.config içerisinde herhangibir bildirim yapmaya gerek yoktur. Dolayısıyla WCF için Asp.Net Hosting tarafına, Xml Web Servislerindeki çalışma zamanı kolaylığının getirildiğini düşünebiliriz.

[SimpleAspNetHosting.rar (12,40 kb)](/assets/files/2009/SimpleAspNetHosting.rar)
