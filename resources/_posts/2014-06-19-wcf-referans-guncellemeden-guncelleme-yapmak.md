---
layout: post
title: "WCF - Referans Güncellemeden Güncelleme Yapmak"
date: 2014-06-19 17:20:00 +0300
description: "Bildiğiniz üzere bir servis yazıldığında genellikle bunu tüketen(Consume) en az bir taraf bulunur. İstemci olarak düşündüğümüz bu taraflar her hangi bir uygulama olabilir. WCF(Windows Communication Foundation) ile geliştirdiğimiz bu servislerin, söz konusu istemciler tarafından kullanılması noktasında ise izlenebilecek bir kaç yol bulunmaktadır. Bunlardan en yaygını, servislerin projeye referans olarak eklenmesi ve üretilen Proxy sınıfının kullanılmasıdır(Add Service Reference). Visual Studio gibi gelişmiş IDE’ lerin ilgili arabirimleri, bu noktada büyük kolaylık sağlamaktadır."
categories:
  - wcf
tags:
  - wcf
  - csharp
  - xml
  - dotnet
  - aspnet
  - json
  - http
  - iis
  - javascript
  - generics
  - visual-studio
---
# WCF - Referans Güncellemeden Güncelleme Yapmak
[![wcfrf_8](/assets/images/2014/wcfrf_8_thumb.png)](/assets/images/2014/wcfrf_8.png) Merhaba Arkadaşlar,

Bildiğiniz üzere bir servis yazıldığında genellikle bunu tüketen (Consume) en az bir taraf bulunur. İstemci olarak düşündüğümüz bu taraflar her hangi bir uygulama olabilir. WCF (Windows Communication Foundation) ile geliştirdiğimiz bu servislerin, söz konusu istemciler tarafından kullanılması noktasında ise izlenebilecek bir kaç yol bulunmaktadır. Bunlardan en yaygını, servislerin projeye referans olarak eklenmesi ve üretilen Proxy sınıfının kullanılmasıdır (Add Service Reference). Visual Studio gibi gelişmiş IDE’ lerin ilgili arabirimleri, bu noktada büyük kolaylık sağlamaktadır.

İkinci bir yol ise Proxy üretimi zorunluluğu olmadan servislerin kullanılmasıdır. Ancak söz konusu servislerin Web programlama modeline uygun olacak şekilde, HTTP protokolünün ilgili metodlarına (GET,POST,PUT,DELETE…) destek vermek üzere geliştirilmesi gerekir. Daha çok veri odaklı (Data-Centric) servisler için geçerli olan bu senaryoda, içerik de çoğunlukla JSON (JavaScriptObjectNotation), BSON (Binary Javascript Object Notation) ve XML (eXensibleMarkupLanguage) gibi veri formatlarında sunulmaktadır. Bu tip servislerin tüketilmesinde istemcinin her hangi bir Proxy üretimine gereksinimi bulunmamaktadır. Yine de işleri kolaylaştırıcı tiplerden yararlanıldığı da görülmektedir.

Çok doğal olarak her iki kullanım şeklinin de bazı handikapları vardır. Özellikle Proxy bazlı servis kullanımında yaşanan sıkıntılardan birisi, servislerin çeşitli nedenler ile güncellenmeleri sonrasında, istemcilerin de sahip oldukları referansları güncelleme gerekliliği (Update Service Reference) ya da farklı versiyonların nasıl ele alınacağıdır.

Vaka

Ancak burada özel bir vaka vardır. Bazen servis tüketicileri ile servis tarafının geliştiricileri aynı projeye dahil edilmiş ve bir arada çalışan ekiplerdir ve hatta aynı Solution üzerinde çalışmaktadırlar. Ayrıca buna bir de istemci ve servis tarafının IIS tabanlı birer Web Site projesi olması şartını da eklediğimiz de ortaya farklı bir bakış açısı çıkmaktadır. O da, istemci tarafının servis referansını güncellemeden (ve pek tabi proxy tipi ürettirmeden) çalışabilmesi ve güncellemeleri anında uygulama şansına sahip olabilmesidir.

Bu tip bir vaka genellikle istemci tarafının, servise yeni bir operasyon ilave edilmesini istediği durumlarda ortaya çıkmaktadır. İşte bu yazımızda söz konusu vakayı nasıl gerçekleştirebileceğimizi incelemeye çalışıyor olacağız.

Solution İçeriği

İlk olarak Solution içerisinde aşağıdaki proje iskeletinin söz konusu olduğunu düşünelim.

[![wcfrf_1](/assets/images/2014/wcfrf_1_thumb.png)](/assets/images/2014/wcfrf_1.png)

Dikkat edileceği üzere Client ve servisi Host eden uygulamalar aslında birer Asp.Net Web Site olarak yer almaktadır. Bununla birlikte servis sözleşmesi (Service Contract) ile sözleşme implementasyonu ayrı projeler olarak düşünülmüş ve birer WCF Service Library şeklinde tasarlanmışlardır.

Projelerin Oluşturulması

Pek tabi servisi host eden uygulama her iki kütüphaneyi de referans etmek durumundadır. İstemci tarafından olaya baktığımızda ise sadece sözleşmeyi (Contract) barındıran kütüphaneye bir referans içerdiği gözden kaçmamalıdır. Bu yaklaşıma göre Solution’ ımızı yavaş yavaş oluşturmaya çalışalım. Bunun için aşağıdaki şekilde görülen ağaç yapısını inşa etmemiz yeterlidir.

[![wcfrf_2](/assets/images/2014/wcfrf_2_thumb.png)](/assets/images/2014/wcfrf_2.png)

Karma.HostApp, WCF Service tipinden bir Asp.Net Web Site’ dır (Add New Web Site ile eklerken WCF Service tipi seçilmelidir). Karma.ClientApp, Empty tipinden bir Asp.Net Web Site’ dır (Add New Web Site ile eklerken Asp.Net Empty Web Site seçilmelidir). Her iki Web Site’ de IIS üzerinde host edilecek şekilde üretilmişlerdir (Eklerken HTTP tipini seçip IIS altında Application klasörü oluşturulması yeterlidir) Karma.Contract ve Karma.Implementation’ da birer WCF Service Library’ dir.

Burada senaryo için anahtar nokta Web Site proje şablonunun kullanılmış olmasıdır. Dikkat edilirse, Solution içerisinde yapılacak Build işlemlerinin ardından Web Site projelerinin Bin klasörlerinde, referans edilen dll’ lerin otomatik olarak eklendiği görülecektir.

Servis Tarafının Kodlanması

Şimdi örnek senaryo içerisinde ele alacağımız diğer tipleri de ilave etmeye başlayalım. Karma.Contract kütüphanesinde aşağıdaki servis sözleşmesinin yazıldığını düşünelim.

[![wcfrf_3](/assets/images/2014/wcfrf_3_thumb.png)](/assets/images/2014/wcfrf_3.png)

```csharp
using System.ServiceModel;

namespace Karma.Contract 
{ 
   [ServiceContract] 
    public interface IAlgebra 
    { 
        [OperationContract] 
        double Sum(double x, double y); 
    } 
}
```

Şu aşamada servis sözleşmesinin içeriği çok da önemli değil. Sadece bir sözleşme olması yeterli senaryo gereği. Bu sözleşmeyi implemente edecek sınıfın konuşlandırılacağı yer ise, Karma.Implementation isimli WCF kütüphanesidir. Bu kütüphane içeriğini de aşağıdaki gibi geliştirdiğimizi düşünelim.

[![wcfrf_4](/assets/images/2014/wcfrf_4_thumb.png)](/assets/images/2014/wcfrf_4.png)

```csharp
using Karma.Contract;

namespace Karma.Implementation 
{ 
    public class Algebra 
       :IAlgebra 
    { 
        public double Sum(double x, double y) 
        { 
            return x + y; 
        } 
    } 
}
```

Servis tarafı için gerekli sözleşme (Contract) ve uygulayıcı tip hazır. Çok doğal olarak bu sözleşmeyi bir yerde host ederek sunmalıyız. Bu amaçla projemizde yer alan Karma.HostApp uygulamasına bir WCF Service ekleyeceğiz, ancak sadece svc uzantılı dosyanın durmasına izin vermeliyiz. Bir başka deyişle AppCode klasörü içerisine atılan dosyaları (büyük ihtimalle IService1.cs ve Service1.cs) silmeliyiz. Diğer yandan AlgebraService.svc dosyasının içeriğinin de aşağıdaki şekilde düzenlenmesi gerekmektedir.

```text
<%@ ServiceHost Language="C#" Debug="true" Service="Karma.Implementation.Algebra" %>
```

Dikkat edileceği üzere ServiceHost elementindeki Service niteliğinin değeri, Karma.Implementation kütüphanesinde yer alan Algebra tipi olarak işaret edilmiştir.

Buraya kadar ki işlemleri tamamladıktan sonra servisin çalışıp çalışmadığının kontrol edilmesinde yarar vardır. Eğer aşağıdaki çıktıyı alabiliyorsak ve WSDL çıktılarına da sorunsuz ulaşabiliyorsa süper!

[![wcfrf_5](/assets/images/2014/wcfrf_5_thumb.png)](/assets/images/2014/wcfrf_5.png)

İstemci Tarafının Kodlanması

Gelelim istemci tarafına. Yani Karma.ClientApp uygulamasına. Senaryomuzda belirttiğimiz üzere bu projede her hangi bir şekilde Add Service Reference ile Proxy üretimi söz konusu olmamalıdır/olmayacaktır. Nitekim kurtulmak istediğimiz nokta referans güncelleme işlemleridir. Ancak diğer taraftan istemcinin uygun servis sözleşmesini kullanarak bir şekilde ilgili host uygulamaya talepte bulunabilmesi de gerekmektedir. Bu nedenle istemci tarafında bu kullanımı sağlayabilecek ilave bir sınıfa daha ihtiyacımız vardır. Aşağıdaki kod parçasında olduğu gibi.

```csharp
using System.ServiceModel;

public class AlgebraServiceClient<T> 
    : ClientBase<T> 
    where T : class 
{ 
    private bool _disposed = false; 
    public AlgebraServiceClient() 
        : base(typeof(T).FullName) 
    { 
    } 
    public AlgebraServiceClient(string endpointConfigurationName) 
        : base(endpointConfigurationName) 
    { 
    } 
    public T Proxy 
    { 
        get { return this.Channel; } 
    } 
    protected virtual void Dispose(bool disposing) 
    { 
        if (!this._disposed) 
        { 
            if (disposing) 
            { 
                if (this.State == CommunicationState.Faulted) 
                { 
                    base.Abort(); 
                } 
                else 
                { 
                    try 
                    { 
                        base.Close(); 
                    } 
                    catch 
                    { 
                        base.Abort(); 
                    } 
                } 
                _disposed = true; 
            } 
        } 
    } 
}
```

Aslında ClientBase türevli olarak geliştirilen AlgebraServiceClient, Add Service Reference işlemi sonrası üretilen Proxy sınıfı içerisinde yer alan tiplere benzemektedir. Ancak daha generic bir tip söz konusudur. Özellikle bu tipin konfigurasyon bazlı çalışabilmesi için yapıcı metodlardan birisinde, üst sınıftaki yapıcıya (Constructor) paramete gönderildiğine dikkat edilmelidir.

Tabi istemci tarafında ilgili WCF servisinin kullanılabilmesi için web.config dosyasında da bazı düzenlemelerin yapılması gerekmektedir. Bu sebeple web.config içeriğini aşağıdaki gibi değiştirmeliyiz.

```xml
<?xml version="1.0"?> 
<configuration> 
  <system.web> 
    <compilation debug="true" targetFramework="4.5"/> 
    <httpRuntime targetFramework="4.5"/> 
  </system.web> 
  <system.serviceModel> 
    <bindings> 
      <basicHttpBinding> 
        <binding name="BasicHttpBinding_IAlgebra" closeTimeout="00:01:00" 
            openTimeout="00:01:00" receiveTimeout="00:10:00" sendTimeout="00:01:00" 
            allowCookies="false" bypassProxyOnLocal="false" hostNameComparisonMode="StrongWildcard" 
            maxBufferSize="65536" maxBufferPoolSize="524288" maxReceivedMessageSize="65536" 
            messageEncoding="Text" textEncoding="utf-8" transferMode="Buffered" 
            useDefaultWebProxy="true"> 
          <readerQuotas maxDepth="32" maxStringContentLength="8192" maxArrayLength="16384" 
              maxBytesPerRead="4096" maxNameTableCharCount="16384" /> 
        </binding> 
      </basicHttpBinding> 
    </bindings> 
    <client> 
      <endpoint address="http://localhost/Karma.HostApp/AlgebraService.svc" 
          binding="basicHttpBinding" bindingConfiguration="BasicHttpBinding_IAlgebra" 
          contract="Karma.Contract.IAlgebra" name="BasicHttpBinding_AlgebraService" /> 
    </client> 
  </system.serviceModel> 
</configuration>
```

Bu içeriği oluştururken var olan bir client web.config içeriğinden yararlanmanızı önerebilirim.

Örneğimizde servis tarafında WCF’ in varsayılan konfigurasyon ayarları kullanıldığından, iletişim BasicHttpBinding üzerinden yapılmaktadır. Ancak servis tarafında seçilen EndPoint içeriğine göre istemci tarafındaki tanımlamaların da (Binding gibi) değiştirilmesi gerekebilir.

Test Kodları

Artık servisi kullanmayı deneyebiliriz. Bu amaçla Karma.ClientApp içerisindeki Default.aspx sayfasına basit bir Button ekleyelim ve Click olay metodunda aşağıdaki kodları yazarak geliştirmemize devam edelim.

```csharp
using Karma.Contract; 
using System;

public partial class _Default : System.Web.UI.Page 
{ 
    protected void Page_Load(object sender, EventArgs e) 
    {

    } 
    protected void btnProcess_Click(object sender, EventArgs e) 
    { 
        using (AlgebraServiceClient<IAlgebra> dynamicProxy = 
            new AlgebraServiceClient<IAlgebra>("BasicHttpBinding_AlgebraService")) 
        { 
            double result=dynamicProxy.Proxy.Sum(3.2, 4.5); 
            Response.Write(result.ToString()); 
        } 
    } 
}
```

İstemci uygulama servis sözleşmesini içeren Karma.Contract kütüphanesini referans ettiğinden, IAlgebra üzerinde tanımlı olan Sum metoduna erişebilmektedir. Ancak çalışma zamanında bunun servis tarafına yönlendirilebilmesi, ClientBase türevli AlgebraServiceClient nesne örneğinin kullanılmasına bağlıdır.

Servise Yeni Bir Operasyon Dahil Edilmesi

Peki buraya kadar yazdıklarımız ile asıl hedefimize ulaştık mı dersiniz? Elbette ki hayır. Amacımız; servis tarafında yapılan bir güncelleme sonrasında, istemci tarafının bir Proxy güncellemesine gitmeye gerek duymamasını sağlamaktır. Şimdi bu durumu test etmek amacıyla servis tarafına yeni bir operasyon ilave edildiğini ve bunun uygulandığını göz önüne alalım.

Sözleşme tarafı;

```csharp
using System.ServiceModel;

namespace Karma.Contract 
{ 
    [ServiceContract] 
    public interface IAlgebra 
    { 
        [OperationContract] 
        double Sum(double x, double y);

        [OperationContract] 
        double Multiply(double x, double y); 
    } 
}
```

İmplementasyon tarafı;

```csharp
using Karma.Contract;

namespace Karma.Implementation 
{ 
    public class Algebra 
        :IAlgebra 
    { 
        public double Sum(double x, double y) 
        { 
            return x + y; 
        }

        public double Multiply(double x,double y) 
        { 
            return x * y; 
        } 
    } 
}
```

Güzelll! Peki ya şimdi ne olacak? Normal şartlarda Add Service Reference tekniğine göre bir Proxy üretmiş olsaydık, istemci tarafında bu yeniliklerin görülebilmesi/kullanılabilmesi için söz konusu Proxy üzerinde bir güncelleme işlemi yapmamız gerekirdi. Oysa ki bu senaryoda durum biraz daha farklıdır. Bir Build işlemi yaptığımızda, istemci tarafındaki kodlarda, intellisense’ in devreye girdiği yerlerde yeni operasyonun da ele alınabildiğini görürüz.

[![wcfrf_6](/assets/images/2014/wcfrf_6_thumb.png)](/assets/images/2014/wcfrf_6.png)

Görüldüğü üzere yapılan yeni güncelleme build işlemi sonrasında istemci tarafında da görülmekte ve etkin olarak kullanılabilmektedir.

> Bütün Solution’ ın Build edilmesine gerek yoktur. Sadece istemci uygulamanın refereans ettiği Karma.Contract kütüphanesinin derlenmesi yeterli olacaktır. Bu derleme, güncellemelerin istemci tarafındaki kodlarda kullanılabilmesini sağlayacaktır, nitekim derleme sonrası DLL’ in yeni hali otomatik olarak Web Site’ ın Bin klasörüne yansıyacaktır.

Son Test

Çalışma zamanında ki sonuçlara bakarak testimizi tamamlayalım ve senaryonun çalıştığından emin olalım.

[![wcfrf_7](/assets/images/2014/wcfrf_7_thumb.png)](/assets/images/2014/wcfrf_7.png)

Bu senaryoda dikkat edilmesi gereken nokta, Add Service Reference veya Update Service Reference gibi seçeneklerin kullanılmamış olmasıdır. Diğer yandan size düşen görev neden bu senaryo için Asp.Net Web Site şablonunun tercih edildiğinin bulunmasıdır? Aynı durum Asp.Net Web Application tipleri için söz konusu olamaz mı? Söz konusu olamazsa, iki uygulama şekli arasındaki farklılıklar nelerdir? Yani Web Site’ ın Web Application’ dan farkı nedir? Peki ya bu senaryo Visual Studio 2013 ile gelen yeni nesil web projesi şablonlarında nasıl ele alınabilir? Elbette bu soruları da araştırmak gerekmektedir. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

> İpucu: Özellikle Web Site ile Web Application arasındaki farklara hızlıca bir göz atmak için [şu adresteki blog girdisinden](http://daron.yondem.com/tr/post/WebSite_ile_Web_Application_Arasindaki_Fark_Nedir) yararlanabilirsiniz.

[Karma.zip (48,65 kb)](/assets/files/2014/Karma.zip)