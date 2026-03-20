---
layout: post
title: "WCF 4.0 Yenilikleri - Automatic Help Page [Beta 1]"
date: 2009-09-16 16:02:00 +0300
categories:
  - wcf-4-0-beta-1
tags:
  - wcf-4-0-beta-1
  - csharp
  - xml
  - dotnet
  - wcf
  - rest
  - json
  - http
  - caching
  - visual-studio
---
WCF 4.0 tarafında beklenen gelmesi yüksek olan yenilikleri sizlere aktarmaya çalıştığım yazılarımızın yavaş yavaş sonlarına gelmekteyiz. Elbette incelemeyemediğimiz bir çok detay var. Bunları ilerleyen dönemlerde ürün son halini alırken tartışma ve araştırma fırsatımız olacak. Bu yazımızda WCF 4.0 tarafına entegre olarak gelen REST geliştirme modeline yönelik yeteneklerden bahsedeceğiz.

![blg76_Giris.jpg](/assets/images/2009/blg76_Giris.jpg)

Aslında bu yeniliklerin çoğunu WCF Rest Starter Kit ile birlikte,.Net Framework 3.5 platformu üzerinde kullanabiliyoruz. Ne varki, ek bir pakete ihtiyaç duyulmadan kullanılabilen iki özellik, WCF 4.0 içerisine entegre edilmiş durumda. Bunlardan birisi otomatik yardım sayfaları (Automatic Help Page). WCF 4.0 içerisindeki WebServiceHost fabrikasını kullandığımızda otomatik olarak her RESTful servis için gelen ve varsayılan olarak açık olan yardım sayfaları, istenirse konfigurasyon dosyasındaki bir nitelik yardımıyla kapatılabilirde.

Yardım sayfaları özellikle HTTP protokolünün GET,POST,PUT veya DELETE gibi metodları yardımıyla erişilen RESTful servis operasyonlarının tüketiciler tarafından kolayca anlaşılmasını hedeflemektedir. Nitekim, tüketici tarafını yazan geliştiricilerin bu modelindeki bir servisin operasyonlarını çağırırken, HTTP metoduna göre nasıl bir paket içeriği veya URL hazırlayıp göndermeleri gerektiğini bilmeleri gelmektedir. Otomatik olarak üretilen yardım sayfalarının tek ve yegane amacı bu ihtiyacı karşılamaktır.

Şimdi bu konuyu basit bir örnek üzerinden değerlendirmeye çalışarak sonuçları görmeyi hedefleyeceğiz. Bu amaçla Visual Studio 2010 Beta 1 ortamında ve.Net Framework 4.0 Beta 1 odaklı olarak hazırlayacağımız bir WCF Service Application üzerinden ilerliyor olacağız. Bu örnekte System.ServiceModel.Web.Activation isim alanında yer alan WebServiceHostFactory fabrikasından yararlanmayacağız (Bir sonraki konumuz olan HTTP Cache desteğine ait örnekte ise kullanacağız). Servisimize ait sözleşmemizi (Service Contract) aşağıda görüldüğü gibi geliştirdiğimizi düşünelim.

```csharp
using System.ServiceModel;
using System.ServiceModel.Web;

namespace GeoService
{
    [ServiceContract(Namespace="http://GeoServices/LocationService")]
    public interface ILocationService
    {
        [OperationContract]
        [WebGet]
        string FindLocation(string gsmNumber);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string FindLocationInJson(string gsmNumber);

        [OperationContract]
        [WebInvoke(Method = "DELETE")]
        string Delete(string location);

        [OperationContract]
        [WebInvoke(Method="POST")]
        bool Insert(Customer customer);

        [OperationContract]
        [WebInvoke(Method = "PUT")]
        bool Move(Customer customer);
    }
}
```

Servis sözleşmesinde örnek olarak HTTP GET, POST, PUT ve DELETE metodlarına göre kullanılabilen bazı operasyonlar tanımlanmıştır. FindLocationInJson operasyonu, JSON formatında cevaplar (Response) üretmektedir. Bilindiği üzere WebGet metodu için yapılan çağrılarda URL satırından gelen talepler söz konusudur. WebInvoke niteliği ile imzalanmış operasyonlarda ise HTTP içeriğinin paket olarak gönderilmesi söz konusudur. Bu operasyonlardan Insert ve Move metodları, parametre olarak serileştirilebilir bir tip kullanmaktadır. Dolayısıyla, servisi REST modele göre kullanmak isteyen geliştiricilerin bu kritik noktalara göre paket içeriği veya URL bilgilerini nasıl hazırlayacaklarını bilmeleri son derece yararlı olacaktır. Gelelim servis sözleşmesini uygulayan tipimize;

```csharp
using System;

namespace GeoService
{
    public class LocationService 
        : ILocationService
    {
        public string FindLocation(string gsmNumber)
        {
            return String.Format("({0}:{1})-({2}:{3})", 36, 42, 26, 45);
        }

        public string FindLocationInJson(string gsmNumber)
        {
            return String.Format("({0}:{1})-({2}:{3})", 36, 42, 26, 45);
        }

        public string Delete(string location)
        {
            return string.Format("{0} ----> {1}",location);
        }

        public bool Insert(Customer customer)
        {
            return false;
        }

        public bool Move(Customer customer)
        {
            return true;
        }
    }

    public class Customer
    {
        public string GsmNo { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
    }
}
```

Operasyonalrın uygulanışında herhangibir özel durum söz konusu değildir aslında. Asıl üzerinde duracağımız nokta Web.config dosyasının içeriğidir. Web.config dosyasında yer alan system.ServiceModel element içeriğini aşağıdaki gibi düzenleyebiliriz.

```xml
<system.serviceModel>
    <services>
      <service name="GeoService.LocationService">
        <endpoint address="" binding="webHttpBinding" contract="GeoService.ILocationService"/>
      </service>
    </services>
    <behaviors>
      <endpointBehaviors>
        <behavior>
          <webHttp enableHelp="true"/>
        </behavior>
      </endpointBehaviors>
    </behaviors>
  </system.serviceModel>
```

Bağlayıcı tip (Binding Type) olarak webHttpBinding kullanmamızın sebebi elbetteki servisimizin RESTful özelliklerine göre hizmet vermesinin sağlanmasıdır. Diğer yandan endPoint noktasına eklenen webHttp isimli davranışın enableHelp özelliğine true değeri atanmıştır. Buna göre çalışma zamanında help takısı ile servis talep edildiğinde aşağıdaki çıktı ile karşılaşılır.

URL: http://localhost:2166/LocationService.svc/help

![blg76_Runtime.gif](/assets/images/2009/blg76_Runtime.gif)

Görüldüğü üzere tüm operasyonlar için nasıl çağırılacaklarına, ne tür cevaplar döndüreceklerine dair bilgiler ve hatta kullanılan serileştirilebilir tipler varsa bunların şemalarına ait detaylar bu yardım sayfasında yer almaktadır. Tabi şu anda help sayfası için, Internet Explorer'ın Feed özelliğine göre bir çıktı elde edilmektedir. Normal şartlarda kaynağa baktığımızda aslında varsayılan olarak ATOM formatında bir feed içeriğinin üretildiği görülebilir.

![blg76_RuntimeAtom.gif](/assets/images/2009/blg76_RuntimeAtom.gif)

Hemen hatırlalatım örneğimizde WebServiceHostFactory kullanmadığımızdan, yardım sayfaları enableHelp niteliğine true değerini atamadığımız sürece çalışmayacaktır. GET metodlarının çağrıları dikkat edileceği üzere doğal olarak bir URL formatındadır. Diğer taraftan örneğin Insert operasyonunu çağırmak istediğimizi göz önüne alalım. Bu durumda geliştirici olarak tek yapmamız gereken Request ve örnek talep formatı için Example isimli bağlantılardan elde edilen sonuçlara bakmak olacaktır. Insert operasyonu için üretilen şema aşağıdaki gibidir.

URL: http://localhost:2166/LocationService.svc/help/request/schema

![blg76_InsertSchema.gif](/assets/images/2009/blg76_InsertSchema.gif)

Dikkat edileceği üzere şema bilgisinden yararlanılarak operasyona parametre olarak nasıl bir tip gönderilmesi gerektiği, üyelerinin ne olacağı açık bir şekilde görülmektedir. Insert operasyonu için Example linkine tıkladığımızda ise örnek bir POST çağrısının içeriğinin nasıl olması gerektiğinin örneklendiği görülecektir.

URL: http://localhost:2166/LocationService.svc/help/Insert/request/example?format=Xml

![blg76_InsertExample.gif](/assets/images/2009/blg76_InsertExample.gif)

İşte bu kadar. Son derece basit bir özellik olmakla birlikte otomatik yardım sayfaları aslında tüketiciyi yazan geliştiriciler için bulunmaz bir nimettir. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[HelpSupport.rar (18,55 kb)](/assets/files/2009/HelpSupport.rar)
