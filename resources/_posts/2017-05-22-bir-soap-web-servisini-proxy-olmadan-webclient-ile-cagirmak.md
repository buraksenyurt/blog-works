---
layout: post
title: "Bir SOAP Web Servisini Proxy Olmadan WebClient ile Çağırmak"
date: 2017-05-22 13:13:00 +0300
description: "Geçtiğimiz günlerde şirket dışı bir kurumun web servislerini çağırma ihtiyacımız oldu. Lakin Header bilgisinde bir OAuth Token değeri de geçmemiz gerekiyordu. Bu Header bilgisini SOAP bazlı Web servisine nasıl ekleyeceğimizi düşünürken WebClient sınıfı ile SOAP bazlı servisleri de çağırabileceğimizi öğrendim."
categories:
  - xml-web-services
tags:
  - xml-web-services
  - csharp
  - dotnet
  - aspnet
  - xml
  - soap
  - rest
  - json
  - web-service
  - http
  - oauth
  - java
  - asmx
---
# Bir SOAP Web Servisini Proxy Olmadan WebClient ile Çağırmak
Merhaba Arkadaşlar,

Geçtiğimiz günlerde şirket dışı bir kurumun web servislerini çağırma ihtiyacımız oldu. Lakin Header bilgisinde bir OAuth Token değeri de göndermemiz gerekiyordu. Bu Header bilgisini SOAP bazlı Web servisine nasıl ekleyeceğimizi düşünürken WebClient sınıfı ile de bu işi yapabileceğimizi öğrendik. Üstelik çağrılacak servisin WSDL üretimi referansını projeye eklemeye gerek kalmadan. Bu kısa ipucunda WebClient sınıfının bu amaçla nasıl kullanıldığını incelemeye çalışacağız. Öncelikle eski nesil bir Asp.Net Web Service geliştirdiğimizi düşünelim. İçinde iki değerin toplamını hesap eden basit bir metod yer alacak. Servis kodunu aşağıdaki gibi geliştirebiliriz.

```csharp
using System.Web.Services;

namespace CalculateService
{
    [WebService(Namespace = "http://azon.com/services/math")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class Common 
        : System.Web.Services.WebService
    {
        [WebMethod]
        public double Sum(double x,double y)
        {
            return x+y;
        }
    }
}
```

Önemli olan nokta bu servis operasyonunu çağırmak için göndereceğimiz SOAP Header ve Body içerikleri. Yazılan servisin ilgili operasyonunu herhangibir tarayıcıdan çağırırsak bu içerikleri görebiliriz.

![wslook.gif](/assets/images/2017/wslook.gif)

> Olur da kurumun web servisilerinin yardımcı bir ekran sayfası yoktur (JAVA servisleri gibi) ve sadece WSDL içeriğini görebiliyorsunuzdur, bu durumda SOAP UI gibi bir araçtan yararlaranak ilgili servisi çağırmak için kullanılacak SOAP Body ve Header içeriklerinin otomatik olarak üretilmesini sağlayabilir ve oradan destek alabilirsiniz. Dikkat edilmesi gereken nokta SOAPAction değeridir.

Header bilgisinde servis operasyonunun adını (SOAPAction bilgisi), HTTP'nin hangi metodu ile çağırım yapılması gerektiğini (bu örnekte POST), kullanılması gereken SOAP versiyonunu (bu örnekte 1.1) gönderilecek içeriğin tipini (text/xml; charset=utf-8) bulabiliriz. Body kısmında yer alan string içeriği aynen kullanabiliriz. Sadece x ve y için gerekli sayısal değerleri vermemiz yeterlidir. Kodu basit bir Console uygulamasında aşağıdaki gibi deneyebiliriz.

```csharp
using System;
using System.Net;
using System.Text;

namespace CallWSWithWebClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string content = @"<?xml version=""1.0"" encoding=""utf-8""?>
                                <soap:Envelope 
                                    xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
                                    xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
                                    xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                                    <soap:Body>
                                    <Sum xmlns=""http://azon.com/services/math"">
                                        <x>4</x>
                                        <y>5</y>
                                    </Sum>
                                    </soap:Body>
                                </soap:Envelope>";

            try
            {
                using (WebClient wClient = new WebClient())
                {
                    wClient.Headers.Add("Content-Type", "text/xml; charset=utf-8");
                    wClient.Headers.Add("SOAPAction", "\"http://azon.com/services/math/Sum\"");
                    var result = wClient.UploadData("http://localhost:52523/Common.asmx"
                        , "POST"
                        , Encoding.UTF8.GetBytes(content));
                    var response = System.Text.Encoding.Default.GetString(result);
                    Console.WriteLine(response);
                }
            }
            catch(WebException excp)
            {
                Console.WriteLine(excp.Message);
            }
        }
    }
}
```

WebClient sınıfını oluşturduktan sonra Content-Type ve SOAPAction bilgilerini Headers isimli WebHeaderCollection nesnesine eklemekteyiz. UploadData metodu ile Common.asmx servisine POST tekniğini kullanarak son parametre ile geçilen byte[] içeriğini yolluyoruz. Bu içerik aslında content ile belirtilen XML bilgisi. Bu noktada WebClient metodu hangi EndPoint'e hangi HTTP metodu ve içerik tipi ile veri göndereceğini biliyor. Eğer WebException üretilmesine neden olacak bir sorun yoksa gelen byte[] dizisi şeklindeki cevap ekrana basılıyor. Burada string olarak bir veri alımı söz konusu olsada gelen içerik aslında XML tabanlı. Yani XDocument (XmlDocument) gibi tipler yardımıyla daha kolay kullanılabilir. Nitekim daha fazla değer döndürecek sonuç kümelerinde string ile çağırmak çok da mantıklı değil (Tabii içerik JSON dönüyorsa çok daha güzel olur) Kodun çalışma zamanı çıktısı aşağıdaki gibidir.

![wsresult.gif](/assets/images/2017/wsresult.gif)

Gördülüğü gibi başarılı bir sonuç aldık. WebClient sınıfını ağırlıklı olarak REST tabanlı servisleri çağırmak için kullansak da örnekte görüldüğü gibi SOAP tabanlı servisler için de ele alabiliriz. Tekrar hatırlatmakta fayda var ki referans eklemeden bunu yapmamız mümkün. Eğer servis JSON içerik dönüyorsa sonuçları NewtonSoft'un JsonConvert sınıfından yararlanarak JObject olarak deserialize edebilir, servisin ihtiyaçlarına göre ekstra Header bilgilerini (OAuth Token vb) kolaylıkla gönderebiliriz. Böylece geldik ihtiyaç sonrası ortaya çıkan bir yazının daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.