---
title: "Soap Bazlı WCF Servislerini REST Modeline Taşımak"
pubDate: 2009-04-17 17:19:00
categories:
  - WCF
tags:
  - WCF
  - C#
  - .NET
  - ASP.NET
  - ADO.NET
  - XML
  - SOAP
  - REST
  - HTTP
  - IIS
  - Serialization
  - Visual Studio
---

# Soap Bazlı WCF Servislerini REST Modeline Taşımak
.Net Framework 3.5 ile birlikte, [WCF (Windows Communication Foundation)](http://msdn.microsoft.com/en-us/netframework/aa663324.aspx) tarafına kazandırılan önemli yeteneklerden biriside Web programlama modelidir. Bu modelin getirileri arasında, WCF servislerinin REST ([Representational State Transfer](http://en.wikipedia.org/wiki/Representational_State_Transfer)) tekniğine göre yazılıp, kullanılabilmesi de vardır. Özellikle SOAP (Simple Object Access Procotol) bazlı WCF Servisleri ile REST modeline göre tasarlanmış servisler arasındaki en büyük fark, HTTP metodunun çeşididir.

SOAP bazlı modelde istemciler proxy'ler aracılığıyla HTTP protokolünün POST metoduna göre isteklerini gönderirler. REST modeline göre tasarlanmış bir servise ise HTTP protokolünün GET metoduna göre talepte bulunulmaktadır. URL bazlı Querystring parametreleri ele alınabilir, URL Rewriting/URL Routing tarzında taleplerin istenmesi sağlanabilir (İlerleyen bir yazımda nasıl ele alındığını göstereceğim). Bu yazımızda SOAP bazlı olarak tasarlanmış bir WCF servisinin, basitçe REST modeline nasıl taşınabileceğini incelemeye çalışacağız. Öncelikli olarak elimizde aşağıdaki kod parçalarında yer alan sözleşme (Service Contract) ve tiplere sahip bir WCF Service Application'ımız olduğunu düşünelim.

Servis sözleşmemiz;

```csharp
using System.ServiceModel;

namespace NorthwindServices
{    
    [ServiceContract]
    public interface IProductService
    {
        [OperationContract]
        Product GetProduct(int id);
    }
}
```

Uygulayıcı tip;

```csharp
namespace NorthwindServices
{    
    public class ProductService 
        : IProductService
    {
        #region IProductService Members

        public Product GetProduct(int id)
        {
            return new Product
            {
                 Id=id,
                 Name="Bisiklet(Hemde Kırmızı)",
                 ListPrice=10.45
            };
        }

        #endregion
    }
}
```

Veri sözleşmeli tipi;

```csharp
using System.Runtime.Serialization;

[DataContract]
public class Product
{
    [DataMember]    
    public int Id { get; set; }
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public double ListPrice { get; set; }
}
```

Söz konusu servis SOAP bazlı tasarlandığı için web.config dosyasındaki system.serviceModel içeriğide aşağıdaki gibidir.

![blog3_1.gif](images/blog3_1.gif)

Bu servis test edilmek istenirse eğer, yine 3.5 sürümü ile birlikte gelen WcfTestClient aracı kullanılabilir. Tabi, servisin IIS veya Asp.Net Development Server üzerinden host ediliyor olması gerekmektedir. Aşağıdaki ekran görüntüsünde servisin ilgili GetProduct metodunun test edilişi görülmektedir.

![blog3_2.gif](images/blog3_2.gif)

Görüldüğü üzere istemci ve servis aralarında, SOAP zarflarını (SOAP Envelope) kullanarak haberleşmektedir. Eğer Fiddler gibi bir aracı kullanırsanız bu durumda HTTP Post metoduna göre bir talepte bulunulduğunu görebilirsiniz. Diğer tarafan istemcinin servisi kullanabilmesi için proxy tipinede ihtiyacı vardır. Ancak REST modeli ele alındığında ve HTTP Get ile talepte bulunulduğunda arada buna gerek yoktur ki buda her iki model arasındaki ikinci önemli fark olarak görülebilir. Şu anda yapmak istediğimiz söz konusu servisi REST modeline taşımaktadır.

İlk olarak projeye System.ServiceMode.Web assembly'ının referans edilmesi gerekmektedir. İkinci adımda ise HTTP Get metodu ile erişilmesi istenen operasyonların WebGet niteliği (attribute) ile imzalanması yeterli olacaktır.

```csharp
using System.ServiceModel;
using System.ServiceModel.Web;

namespace NorthwindServices
{    
    [ServiceContract]
    public interface IProductService
    {
        [OperationContract]        
        [WebGet]
        Product GetProduct(int id);
    }
}
```

Bu işlem ile GetProduct operasyonunun HTTP Get metoduna göre çağırlabileceği bildirilmektedir. Peki kime?

Bir nitelik söz konusu ise eğer, bunu çalışma zamanında ele alan bir yapının olması gerekmektedir. Bu senaryoda, WCF çalışma zamanı ortamının bu niteliği göz önüne alarak, operasyona gelecek olan çağrılarda HTTP Get metodunu değerlendiriyor olması gerekir. Var olan ServiceHost fabrikası bu işlemi yapmamaktadır. Bu sebepten servise ait Markup kodları içerisinde aşağıdaki değişikliğin yapılması üçüncü adımdır.

```text
<%@ ServiceHost Language="C#" Debug="true" Service="NorthwindServices.ProductService" CodeBehind="ProductService.svc.cs" Factory="System.ServiceModel.Activation.WebServiceHostFactory" %>
```

Buradaki en önemli değişiklik WebServiceHostFactory fabrika tipinin kullnılıyor olmasıdır. Dolayısıyla artık ProductService.svc isimli hizmete gelecek olan HTTP Get talepleri karşılanabilecektir. Buna göre Üçüncü adımdan sonra servis bir tarayıcı uygulama yardımıyla test edilebilir. Ancak bu durumda aşağıdaki görüntü ile karşılaşılması muhtemeldir.

![blog3_3.gif](images/blog3_3.gif)

Bu son derece doğaldır. Nitekim halen web.config dosyası içerisinde service ile ilişkili ayarlar SOAP modeline göre durmaktadır.(BasicHttpBinding kullandığımız hatırlayalım) İki alternatif vardır. web.config içeriğinde yer alan servis ilişkili bildirimler tamamen kaldırılabilir. Yada WebHttpBinding bağlayıcı tipinin kullanılması tercih edilebilir. Ben web.config içerisindeki system.ServiceModel içeriğini tamamen kaldırmayı tercih ettim. Buda bizim 4ncü adımımız oldu. Peki bu adımdan sonra servisi tekrar denersek...

![blog3_4.gif](images/blog3_4.gif)

Sonuç yine hüsran.

![Cry](images/smiley-cry.gif)

Aslında problem servisten nasıl istekte bulunacağımızı bilmiyor oluşum. Nitekim WebGet niteliği ile imzalanmış olan GetProduct operasyonuna HTTP Get modeline göre parametre ile birlikte talepte bulunmamız gerekiyor. Aynen aşağıdaki şekilde olduğu gibi.

![blog3_5.gif](images/blog3_5.gif)

Dikkat edileceği üzere /GetProduct?id=1001 ile gönderilen talep, başarılı bir şekilde servis tarafından ele alınmış ve geriye, Product nesne örneği için oluşturulan XML içeriği döndürülmüştür.

REST modeli öylesine tutuldu ki WCF 4.0 içerisinde zaten gömülü olarak daha fazla eklenti ile birlikte gelecek. (Ado.Net Data Service'leri bu modelin güzel bir açılımı olarak görebiliriz aslında) Bu eklentileri bir süre önce CodePlex'te yayımlanan [WCF Rest Starter Kit](http://aspnet.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=24644)yardımıyla Visual Studio 2008 ortamı üzerinde test etmemiz ve geliştirmemizde mümkün. Hatta geçtiğimiz Mart ayında bu geliştirme kitinin 2nci versiyonuda yayınladı. REST modeli ile ilişkili olaraktan yeni yazılar eklemeye devam ediyor olacağım. Hatta Rest modeli ile ilişkili iki görsel dersimede aşağıdaki linklerden ulaşabilirsiniz.

Atom Feed Service [NedirTv?](http://www.nedirtv.com/video/WCF-REST-Bolum-1---Atom-Feed-Service.aspx)

Read-Only Collection Service [NedirTv?](http://www.nedirtv.com/video/WCF-REST-Bolum-2---Readonly-Collection-Service.aspx)

[NorthwindServices.rar (16,34 kb)](assets/NorthwindServices.rar)

Artık dinlenmeye çekilebilir miyim acaba?

![Cool](images/smiley-cool.gif)
