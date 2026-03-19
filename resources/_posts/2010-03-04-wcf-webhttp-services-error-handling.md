---
layout: post
title: "WCF WebHttp Services - Error Handling"
date: 2010-03-04 23:10:00 +0300
description: "Bu yazımızda WCF Eco System' in bir parçası olan WebHttp Service' lerinde hata yönetimini(Error Management) etkili bir şekilde nasıl ele alabileceğimizi incelemeye çalışıyor olacağız."
categories:
  - wcf-eco-system
  - wcf-webhttp-services
tags:
  - wcf-eco-system
  - wcf-webhttp-services
  - csharp
  - dotnet
  - aspnet
  - ado-net
  - entity-framework
  - linq
  - wcf
  - xml
  - rest
  - json
  - http
  - generics
  - visual-studio
  - rc
---
# WCF WebHttp Services - Error Handling
![blg136_Giris.jpg](/assets/images/2010/blg136_Giris.jpg)

Merhaba Arkadaşlar,

Bu yazımızda WCF Eco System'in bir parçası olan WebHttp Service'lerinde hata yönetimini (Error Management) etkili bir şekilde nasıl ele alabileceğimizi incelemeye çalışıyor olacağız. WCF WebHttp Service'leri üzerinden çağırılan bir servis operasyonundan, istemci tarafına kendi insiyatifimizde hata mesajları gönderilmesini sağlayabiliriz.

Üstelik bu mesajları bilinen HTTP durum kodları (HTTP Status Code) çerçevesinde yayınlayabiliriz. Bu tip bir isteğin çeşitli sebepleri olabilir. Hemen bir gerçek hayat senaryosu üzerinden ilerleyerek bu basit konuyu pekiştirmeye çalışalım. Bu amaçla Visual Studio 2010 Ultimate RC sürümü üzerinde geliştirdiğimiz ve aşağıdaki kod içeriğine sahip bir WCF REST Service Application projemiz olduğunu düşünelim.

```csharp
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

namespace ServerApp
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class AzonBookService
    {
        static List<Book> books = new List<Book>
            {
                new Book{ Id=1002, Name="Programming WCF 4.0",ListPrice=12.00M},
                new Book{Id=9034,Name="Pro Asp.Net 4.0",ListPrice=34.90M},
                new Book{Id=4560,Name="Algebra",ListPrice=122.39M},
                new Book{Id=1200,Name="C# 3.5 Cookbook",ListPrice=14.45M},
                new Book{Id=1201,Name="Pro C# 4.0 and .Net Framework 4.0",ListPrice=34.05M},
                new Book{Id=1201,Name="Beginning Ado.Net Entity Framework",ListPrice=14.55M}
            };

        [WebGet(UriTemplate = "/{firstLetter}")]
        public List<Book> GetBooks(string firstLetter)
        {
            return (from book in books
                   where book.Name.StartsWith(firstLetter)
                   select book).ToList();
        }
    }

    public class Book
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal ListPrice { get; set; }
    }
}
```

AzonBookService çok basit ve tek bir operasyona sahiptir. GetBooks isimli operasyon HTTP Get metoduna göre hizmet vermekte olup baş harfleri firstLetter parametresin ile gelen değer ile başlayan bir List koleksiyonunu geriye döndürmektedir. Bu servisi bir tarayıcı uygulama üzerinden test ettiğimizde ve örneğin Pro kelimesi ile başlayan kitapları elde etmek istediğimizde aşağıdaki ekran görütüsündekine benzer sonuçlar ile karşılaşırız.

![blg136_Runtime1.gif](/assets/images/2010/blg136_Runtime1.gif)

Bu noktaya kadar zaten herhangibir sorun bulunmamaktadır. Ancak örneğin Ç harfi ile başlayan kitapların listesini elde etmek istediğimizde, aşağıdaki ekran görüntüsü ile karşılaşırız.

![blg136_Runtime2.gif](/assets/images/2010/blg136_Runtime2.gif)

Şimdi bu noktada geliştirici olarak bir karar verebiliriz. İstemci tarafına bu şekilde boş bir XML içeriği döndürmemiz halinde, istemci tarafının Ç harfi ile başlayan kitapların olmaması durumunu ele alması gerekmektedir. Yani istemci tarafına ek bir iş yükü getirmiş olabiliriz. Nitekim istemcinin dönen listenin eleman sayısına bakara bir karar vermesi gerekmektedir. Diğer yandan istemciyi bilinçli bir şekilde uyarabiliriz de. Nasıl mı? Servis tarafındaki çalışma zamanında üreteceğimiz ve o anki vakaya uygun bir istisna (Exception) ile. Tabiki bu istisna mesajı servis bazlı bir yapı üzerinden ele alınacağı için istemci tarafına gönderilecek paketin içerisine gömülecektir. WCF WebHttp Service'lerinde bu tip istisnaları ele almak için WCF tarafından bildiğimiz FaultException tipinden türeyen WebFaultException sınıfından yararlanılır.

![blg136_WebFaultException.gif](/assets/images/2010/blg136_WebFaultException.gif)

System.ServiceModel.Web isim alanı altında yer alan WebFaultException tipinin normal ve generic olan versiyonları bulunmaktadır. İlgili tipleri kullanımları son derece basittir. Yukarıdaki senaryomuza göre istemciye aradığı kriterlere uygun bir içerik bulunamadığını bildirmek amacıyla, GetBooks isimli servis operasyonunu aşağıdaki gibi değiştirebiliriz.

```csharp
[WebGet(UriTemplate = "/{firstLetter}")]
public List<Book> GetBooks(string firstLetter)
{
	var result=(from book in books
		   where book.Name.StartsWith(firstLetter)
		   select book).ToList();

	if (result.Count == 0)
		throw new WebFaultException<string>("Talep edilen kelime ile başlayan kitaplar sistemde mevcut değiller.", System.Net.HttpStatusCode.NotFound);

	return result;
}
```

Burada dikkat edileceği üzere sonuç listesinin eleman sayısı kontrol edilmiş ve eğer 0 ise WebFaultException tipinden bir istisna mesajı fırlatılması sağlanmıştır. WebFaultException tipinin örneklenmesi sırasında dikkat edilmesi gereken hususlardan biriside HttpStatusCode.NotFound Enum sabiti değerinin verilmesidir. Bu şekilde istemci tarafına hangi HTTP durum kodunun (Status Code) gönderileceği belirlenmektedir. Tahmin edeceğiniz üzere pek çok HTTP Status Code değeri bulunmaktadır.

Aslında tam liste içeriği şudur

![Wink](/assets/images/2010/smiley-wink.gif)

Accepted, Ambiguous, BadGateway, BadRequest, Conflict, Continue, Created, Expectation Failed, Forbidden, Found, GatewayTimeout, Gone, HttpVersionNotSupported, InternalServerError, LengthRequired, MethodNotAllowed, Moved, MovedPermanently, MultipleChoices, NoContent, NonAuthoritativeInformation, NotAcceptable, NotFound, NotImplemented, NotModified, OK, PartialContent, PaymentRequired, PreconditionFailed, ProxyAuthenticationRequired, Redirect, RedirectKeepVerb, RedirectMethod, RequestedRangeNotSatisfiable, RequestEntityTooLarge, RequestTimeout, RequestUriTooLong, ResetContent, SeeOther, ServiceUnavailable, SwitchingProtocols, TemporaryRedirect, Unauthorized, UnsupportedMediaType, Unused, UseProxy

Operasyonumuzu bu yeni haliyle denediğimizde ise tarayıcı uygulama üzerinde aşağıdaki şekilde görülen çıktı ile karşılaşırız.

![blg136_HTTP404.gif](/assets/images/2010/blg136_HTTP404.gif)

ki bu son derece doğaldır.

Nitekim zaten HTTP 404 mesajının istemci tarafına gönderilmesi sağlanmaktadır. WebFaultException sınıfının örneklenmesi sırasında T parametresi de önemlidir. Örneğimizde basit bir string tipi kullanılmıştır. Peki ya kullanıcı tanımlı bir tip buraya dahil edilebilir mi? Gelin hem bu durumu hemde istemci uygulamanın geliştiriciler tarafından yazılması halinde söz konusu hata mesajlarının nasıl ele alındığı örneğimizi güncelleyerek irdelemeye çalışalım. Bu amaçla servisimizi aşağıdaki hale getirelim.

...Kodun diğer kısmı

```csharp
 [WebGet(UriTemplate = "/{firstLetter}")]
        public List<Book> GetBooks(string firstLetter)
        {
            var result=(from book in books
                   where book.Name.StartsWith(firstLetter)
                   select book).ToList();

            if (result.Count == 0)
                throw new WebFaultException<ErrorInformation>(
                    new ErrorInformation { SearchLetter = firstLetter, SearchTime = DateTime.Now, Summary = "Talep edilen kelime ile başlayan kitaplar sistemde mevcut değiller." }
                    , System.Net.HttpStatusCode.NotFound);

            return result;
        }
    }

    public class ErrorInformation
    {
        public string SearchLetter { get; set; }
        public DateTime SearchTime { get; set; }
        public string Summary { get; set; }  
    }
```

Bu kod parçasında WebHttpException tipinden bir nesne örneklenmiştir. Buna göre istemci tarafına bu içeriğin XML veya JSON formatında gönderilmesi mümkündür. Gelelim istemci tarafının kodlarına. Nitekim bir şekilde servis tarafından gelen içeriği kontrol atlına almamız gerekmekte.

```csharp
using System;
using Microsoft.Http;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (HttpClient client = new HttpClient("http://localhost:12654/Azon/"))
            {
                HttpResponseMessage response=client.Get("Ç");

                Console.WriteLine("Http Status Code : {0}\n",response.StatusCode.ToString());
                Console.WriteLine("Content : {0}\n",response.Content.ReadAsString());

            }
        }
    }
}
```

Ç harfi ile başlayan kitapların elde edilmesi için bir talep oluşturulmakta ve bu talep Get metodu yardımıyla servis tarafına gönderilmektedir. Get metodunun çıktısı HttpResponseMessage tipindendir. Elde edilen HttpResponseMessage nesne örneğinin StatusCode özelliği yardımıyla servis tarafında üretilen HttpStatusCode Enum sabitinin değeri elde edilir. Diğer yandan eğer istemci tarafından bir hata oluşuyorsa WebFaultException nesnesinin oluşturulması sırasında kullanılan ErrorInformation nesne örneğinin serileştirilmiş hali Content özelliği üzerinden elde edilebilmektedir. Buna göre çalışma zamanı çıktısı aşağıdaki gibi olacaktır.

![blg136_Client.gif](/assets/images/2010/blg136_Client.gif)

Görüldüğü üzere servis operasyonu içerisinde üretilen ErrorInformation bilgisi istemci tarafına XML formatlı olacak şekilde aktarılmıştır. Sonuç olarak olası veya beklenen hatalar karşısında istemci tarafına uygun bir bildirim yapılması HTTP durum kodları bazında mümkündür. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Lesson4_RC.rar (174,57 kb)](/assets/files/2010/Lesson4_RC.rar) [Örnek Visual Studio 2010 Ultimate Beta 2 Sürümünde geliştirilmiş ancak RC sürümü üzerinde de test edilmiştir]