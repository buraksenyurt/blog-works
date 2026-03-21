---
layout: post
title: "WCF 4.0 Yenilikleri - HTTP Cache Desteği [Beta 1]"
date: 2009-09-22 13:07:00 +0300
categories:
  - wcf-4-0-beta-1
tags:
  - windows-communication-foundation
---
Performans pek çok uygulama geliştirme ortamında önem arz eden konuların başında gelmektedir. Özellikle Web tabanlı uygulamalarda performans arttırmak adına göz önüne alınan kriterlerden biriside farklı tipteki önbellekleme (Caching) işlemleridir.

![blg77_Performance.jpg](/assets/images/2009/blg77_Performance.jpg)

En basit ve popülerlerinden birisi olan Output Caching,REST tabanlı WCF servisleri içinde kullanılabilmektedir. WCF'in önceki sürümünde WebOperationContext tipinden yararlanılarak ekstra kod eforu ile ele alınabilen Output Cache özelliği, 4.0 sürümünde tamamen dekleratif olarak değerlendirilebilmektedir. Aslında bu yenilik bilindiği üzere WCF Rest Starter Kit Preview 2 ile birlikte.Net Framework 3.5 üzerinde de uygulanabilmektedir. Output Cache özelliği performans için önemli bir kriter olduğundan, WCF 4.0 versiyonunda doğrudan ele alınmaktadır.

Bu yazımızda ön bellekleme işleminin WCF 4.0 içerisinde, REST tabanlı servisleri için nasıl geliştirilebileceğini ele almaya çalışacağız. İşe ilk olarak bir WCF Service Application projesi oluşturarak ve içerisine aşağıdaki servis sözleşmesi ve uygulayıcı tipi ekleyerek başlayabiliriz.

Servis Sözleşmesi (Service Contract)

```csharp
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Web.Caching;

namespace Calculus
{
    [ServiceContract(Namespace="http://calculus/BasicMathService")]
    public interface IBasicMathService
    {
        [AspNetCacheProfile("ShortCache")] // Config dosyasındaki outputCacheProfile girdilerinden parametre olarak verilen isimdekini işaret eder
        [OperationContract]
        [WebGet]        
        string Sum(double x, double y);
    }
}
```

Uygulayıcı tip;

```csharp
using System;
using System.ServiceModel.Activation;

namespace Calculus
{
    [AspNetCompatibilityRequirements(RequirementsMode=AspNetCompatibilityRequirementsMode.Allowed)]
    public class BasicMathService 
        : IBasicMathService
    {
        public string Sum(double x, double y)
        {
            return string.Format("{0} zamanlı hesaplama {1}+{2}={3}", DateTime.Now.ToLongTimeString(), x, y, x + y);
        }
    }
}
```

Servisimize ait svc içeriği;

```text
<%@ ServiceHost Language="C#" Debug="true" Service="Calculus.BasicMathService" CodeBehind="BasicMathService.svc.cs" Factory="System.ServiceModel.Activation.WebServiceHostFactory" %>
```

IBasicMathService isimli servis sözleşmesi içerisinde yer alan Sum metoduna WebGet ve OperationContract dışında AspNetCacheProfile isimli bir niteliğin (attribute) daha uygulandığı görülmektedir. Bu nitelik parametre olarak string bir bilgi alır. Bu bilgi ise biraz sonra yazacağımız Web.config dosyası içerisindeki bir Cache profilini işaret etmektedir. Dolayısıyla bir operasyonun çıktısının ön belleklenmesi için gerekli özellikler, konfigurasyon dosyasında tanımlanır. Servis kodunda dikkat çekici noktalardan biriside, BasicMathService tipinin, AspNetCompatibilityRequirements isimli niteliği uygulamış olmasıdır. Bu durumu biraz sonra değerlendiriyor olacağız nitekim uygulanmadığı hallerde başımıza iş açacaktır

![Undecided](/assets/images/2009/smiley-undecided.gif)

Tabikide üzerinde durmamız gereken en önemli kısım config dosyası içeriğidir.

```xml
<?xml version="1.0"?>
<configuration>
 <system.web>
  <compilation targetFrameworkMoniker=".NETFramework,Version=v4.0" debug="false"/>
 </system.web>
 <system.web>
  <caching>
   <outputCacheSettings>
    <outputCacheProfiles>
     <!-- Süreleri farklı olan iki ayrı Cache profili tanımlanmıştır -->
     <add name="ShortCache" duration="20" varyByParam="none"/>
     <add name="LongCache" duration="600" varyByParam="none"/>
    </outputCacheProfiles>
   </outputCacheSettings>
  </caching>
 </system.web>
 <system.serviceModel>
  <serviceHostingEnvironment aspNetCompatibilityEnabled="true"/>
  <!--WebServiceHostFactory tipini kullandığımız için aşağıdaki davranışı eklememize gerek kalmamıştır-->
  <!--<behaviors>
      <endpointBehaviors>
        <behavior>
          <webHttp enableHelp="true" />
        </behavior>
      </endpointBehaviors>
    </behaviors>-->
  <services>
   <service name="Calculus.BasicMathService">
    <endpoint binding="webHttpBinding" contract="Calculus.IBasicMathService"/>
   </service>
  </services>
 </system.serviceModel>
</configuration>
```

Görüldüğü üzere outputCacheSettings elementi içerisinde iki farklı önbellekleme profili tanımlanmıştır. Buna göre, Sum isimli operasyonumuz 20 saniyelik ön bellekleme yapan ve Querystring parametrelerini hesaba katmayan bir yapı sunmaktadır. (Tabiki önbellekleme profilini oluştururken farklı özellikleride değerlendirebiliriz. Örneğin ön bellekleme lokasyonunu değiştirebilir sunucu tarafı, istemci tarafı veya her iki taraf gibi değerler verilebilir.) Config dosyasında bold olarak işaretlediğimiz diğer kısımlarda önemlidir. WCF 4.0 tarafına getirilen Output Cache özelliği aslında Asp.Net ortamının hazır olarak sahip olduğu Output Cache kabiliyetlerini kullanmaktadır. Bu nedenle Asp.Net uyumluluğu önemlidir.

Uygulama basit bir kaç ayarlamaya sahip olmasına rağmen geliştirme ve testler sırasında beklenmedik pek çok hata ile karşılabiliriz.

![Sealed](/assets/images/2009/smiley-sealed.gif)

İşte karşılaşabileceğimiz bir kaç hata ve önerilen çözümler (ki bu çözümlerin bir kısmı must olarak görülmelidir)

- Web.config dosyasında targetFrameworkMonikor değerinin set edildiğinden emin olmalıyız. Edilmediği takdirde çalışma zamanında alınacak hata mesajı: "The application domain or application pool is currently running version 4.0 or later of the.NET Framework. This can occur if IIS settings have been set to 4.0 or later for this Web application, or if you are using version 4.0 or later of the ASP.NET Web Development Server. The element in the Web.config file for this Web application does not contain the required 'targetFrameworkMoniker'attribute for this version of the.NET Framework (for example, ''). Update the Web.config file with this attribute, or configure the Web application to use a different version of the.NET Framework."
- aspNetCompatibilityEnabled niteliğinin değeri true olmadığı sürece, AspNetCacheProfile özelliğini kullanamayız. Alınacak çalışma zamanı hata mesajı: "AspNetCacheProfileAttribute is supported only in AspNetCompatibility mode. "
- aspNetCompatibilityEnabled niteliğinin true olarak atamış olması yeterli olmayacaktır. Nitekim servis sınıfı için AspNetCompatibilityRequirements niteliğinin de set edilmesi gerekir. Edilmediği takdirde alınacak çalışma zamanı hata mesajı: "The service cannot be activated because it does not support ASP.NET compatibility. ASP.NET compatibility is enabled for this application. Turn off ASP.NET compatibility mode in the web.config or add the AspNetCompatibilityRequirements attribute to the service type with RequirementsMode setting as 'Allowed'or 'Required'"
- Asp.Net Development Server üzerinden geliştirme yapıldığında Output Cache özelliği test edilememektedir. Output Cache özelliğinin çalışabilmesi için, servis uygulamasının IIS üzerine dağıtılması gerekmektedir.
- IIS üzerine atılan WCF servis uygulamasının özelliklerinden Target Framework'ün 4.0 versiyonunu işaret edecek şekilde değiştirilmesi gerekir. (Örneği geliştirdiğim zamanda 4.0.20506 versiyonu idi)
- Güvenlik ile ilişkili bir sıkıntı yaşanabilir. Özellikle varsayılan olarak Anonymous Access ve Integrated Windows Authentication modlarından her ikiside seçili gelebilir. Bu noktada WCF çalışma zamanı sadece bir tanesinin seçili olmasını isteyebilir. Bu durumda sadece Anonymous Access seçeneğini işaretleyerek testleri yapabiliriz. Yapılan değişiklik sonrası yinede hata mesajı alınıyorsa, IIS'in bir kere reset edilmesi gerekebilir.
- varyByParam değerini kullanmıyorsak bile none olarak atamalıyız (Asp.Net tarafından bildiğimiz bir kural). Yapmadığımız takdirde alacağımız çalışma zamanı hata mesajı: "The cache profile, 'ShortCache', must include value for 'VaryByParam'. "

Gelelim çalışma zamanı sonuçlarına. IIS üzerine atılan WCF servisimizi çalıştırdıktan sonra Sum operasyonu için yapılan ilk HTTP GET talebi sonrası aşağıdaki örnek ekran görüntüsü elde edilmiştir.

![blg77_Runtime1.gif](/assets/images/2009/blg77_Runtime1.gif)

Görüldüğü gibi ilk talep karşılanmıştır. Hemen zaman bilgisine dikkat edelim ve 20 saniyelik Output Cache süresi dolmadan önce ikinci bir talepte bulunduğumuzu hatta x ve y değerlerini farklı olarak verdiğimizi düşünelim. Bu durumda aşağıdaki sonuçlar alınacaktır.

![blg77_Runtime2.gif](/assets/images/2009/blg77_Runtime2.gif)

Görüldüğü gibi istemciye gönderilen içerik değişmemiştir. Nitekim şu andaki cevap, Asp.Net Output Cache mekanizması tarafından karışlanmış bir önceki talep için üretilen hazır çıktıdır. Ancak Output Cache süresini aştıktan sonra tekrar talepte bulunursak ikinci talep için güncel sonuçları aşağıdaki örnek çıktıda görüldüğü gibi alabiliriz.

![blg77_Runtime3.gif](/assets/images/2009/blg77_Runtime3.gif)

Böylece sistemin çalıştığını ispat etmiş olduk

![Wink](/assets/images/2009/smiley-wink.gif)

Output Cache özelliği REST tabanlı WCF servislerinde, perfomansı arttırıcı bir unsur olarak görülebilir. Nitekim sunucu ve istemci arasındaki gidiş gelişlerde, sürekli hesaplanması gerekmeyen ve çoğunlukla sabit olan içeriklerin tekrardan üretilmesi yerine belirli zaman dilimleri boyunca ön bellekten karşılanması hem servisin işlem yükünü azaltacak hem de hızlı cevap verme sürelerini doğuracaktır. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[RESTSupport.rar (37,47 kb)](/assets/files/2009/RESTSupport.rar)
