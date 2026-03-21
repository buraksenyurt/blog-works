---
layout: post
title: "Bing Maps WCF Servisleri"
date: 2012-02-13 01:05:00 +0300
categories:
  - wcf
tags:
  - bings
  - bing-maps
  - csharp
  - windows-communication-foundation
  - msdn
  - search
  - geolocation
  - geocode
  - imagery
  - route
---
Eğitimci olarak çalıştığım dönemlerde servis bazlı mimarilerde, XML Web Service’ leri son derece popüler bir kavramdı. Pek tabi öğrencilerimin çoğu, bu servislerin gerçek hayat örneklerini merak ederdi. Haklı olarak söz konusu yapının, nasıl çalıştığını anlamanın en iyi yolu onu saha da görmekti.

[![812522_audio_cassette_template](/assets/images/2012/812522_audio_cassette_template_thumb.jpg)](/assets/images/2012/812522_audio_cassette_template.jpg)


O dönemlerde internet üzerinden yayınlanan bazı ücretsiz servislerden yararlanarak konuyu pekiştirmeye çalışırdım. Klasik hava durumu veya finans servisleri bu anlamda çok işe yaramaktaydı.

Tabi zaman ilerledi ve bildiğiniz üzere Microsoft, servis odaklı geliştirme dünyasına yeni bir kavram getirdi. WCF (Windows Communication Foundation) Şimdi eskisi kadar çok sık olmasa da arada sırada eğitim veriyorum ve özellikle WCF konusuna sıra geldiğinde, öğrencilerime verdiğim gerçek hayat örnekleri arasında Bing Maps’ in ücretsiz servisleri de yer alıyor. Bu yazımızda söz konusu servislerden bazılarını nasıl kullanacağımızı, basit fonksiyonlar üzerinden görmeye çalışıyor olacağız.

Microsoft Bing Map’ in geliştiricilere sunduğu 4 önemli servis bulunmaktadır. Bu servislere ait adresleri aşağıdaki tabloda bulabilirsiniz.

Servis
Adres

Geocode
[http://dev.virtualearth.net/webservices/v1/geocodeservice/geocodeservice.svc?wsdl](http://dev.virtualearth.net/webservices/v1/geocodeservice/geocodeservice.svc?wsdl)

Search
[http://dev.virtualearth.net/webservices/v1/searchservice/searchservice.svc?wsdl](http://dev.virtualearth.net/webservices/v1/searchservice/searchservice.svc?wsdl)

Imagery
[http://dev.virtualearth.net/webservices/v1/imageryservice/imageryservice.svc?wsdl](http://dev.virtualearth.net/webservices/v1/imageryservice/imageryservice.svc?wsdl)

Route
[http://dev.virtualearth.net/webservices/v1/routeservice/routeservice.svc?wsdl](http://dev.virtualearth.net/webservices/v1/routeservice/routeservice.svc?wsdl)

Dikkat edileceği üzere söz konusu servislerin tamamı WCF (Windows Communication Foundation) tabanlı olarak geliştirilmişlerdir (svc uzantısına dikkat)

![Wink](/assets/images/2012/smiley-wink.gif)

Dilerseniz ilk olarak Geocode servisini kullanarak, bir lokasyonun Latitude ve Longtitude değerlerini elde etmeye çalışarak işe başlayalım. Örnek fonksiyonelliklerimizi bir Class Library içerisinde toplayabilir ve her bir metodumuz için birer Unit Test geliştirerek ilerleyebiliriz. Servisleri projeye teker teker referans etmemiz gerektiğini hatırlatmak isterim. Aşağıdaki şekilde Geocode Servisinin eklenişi görülmektedir. [![bmwpf_2](/assets/images/2012/bmwpf_2_thumb.png)](/assets/images/2012/bmwpf_2.png)

Diğer servisleri de projeye referans ettiğimizde Config dosyasında aşağıdaki içeriğin üretildiğine şahit oluruz.

[![bmwpf_3](/assets/images/2012/bmwpf_3_thumb.png)](/assets/images/2012/bmwpf_3.png)

Söz konusu servisler için BasicHttpBinding bağlayıcı tipini (Binding Type) kullanan birer EndPoint eklendiği görülmektedir. Tüm servislere SOAP (Simple Object Access Protocol) bazlı erişmek ve iletişime geçmek mümkündür. Dolayısıyla.Net dışı uygulamaların da ilgili servislerden yararlanabileceği düşünülebilir.

Şimdi ilk fonksiyonelliğimizi yazalım. Bu operasyon ile girilen bir lokasyonun (örneğin bir şehrin) detay bilgisini almaya çalışıyor olacağız. İşte fonksiyonumuz.

```csharp
using BingMaps.Common.GeoCode;

namespace BingMaps.Common 
{ 
    public static class CommonOperations 
    { 
        private const string appKey = "Sizin Maps Developer Account Key değeriniz olmalı";

        public static GeocodeResponse GetLocationPoints(string city) 
        { 
            GeocodeResponse response = null; 
            if (string.IsNullOrEmpty(city)) 
                return response;

            GeocodeRequest geocodeRequest = new GeocodeRequest();

            geocodeRequest.Credentials = new Credentials(); 
            geocodeRequest.Credentials.ApplicationId = appKey;

            geocodeRequest.Query = city;

            ConfidenceFilter[] filters = new ConfidenceFilter[1]; 
            filters[0] = new ConfidenceFilter(); 
            filters[0].MinimumConfidence = Confidence.High;

            GeocodeOptions geocodeOptions = new GeocodeOptions(); 
            geocodeOptions.Filters = filters; 
            geocodeRequest.Options = geocodeOptions;

            GeocodeServiceClient geocodeService = new GeocodeServiceClient(); 
            response = geocodeService.Geocode(geocodeRequest);

            return response; 
        } 
    } 
}
```

Geocode servisinden yararlanmak son derece basittir. İlk olarak bir request hazırlanır ve aranan içerik Query özelliği ile değerlendirilir. Yapılacak olan taleple ilişkili olarak bazı filtreleme seçenekleri ayarlanır ve sonrasında servis üzerinden oluşturulan request nesnesi gönderilir. Servise, Geocode metodu üzerinden yapılan çağrı GeocodeResponse tipinden bir referans örneği döndürecektir. Pek tabi aranan içeriğe göre dünya üzerinde birden fazla lokasyon noktası önerilebilir. Bu sebepten dolayı GeocodeResponse referansı kendi içerisinde dizi bazlı olaraktan bir Result setini saklar. Bu set içerisinde, aranan lokasyon için olabilecek tüm önerilere yer verilir.

Kodda dikkat edilmesi gereken önemli noktalardan birisi de Maps Development Account’ umuza ait bir Application Key değeri kullanıyor olmasıdır. Bu değer servis ile olan iletişimimiz sırasındaki Credential bilgisi için gereklidir.

> Key ve Bing Maps Development için tüm bilgilere [bu adresten](http://www.microsoft.com/maps/developers/mapapps.aspx) ulaşabilirsiniz. Uygulama anahtarının oluşturulması son derece basittir. SSO (Single Sign On) un bir velinimeti olarak Microsoft’ un bu servisinden, var olan Windows Live ID’ miz ile hizmet alabiliriz. Söz konusu key değeri diğer BING servisleri için de gereklidir.

Şimdi yazmış olduğumuz bu metodu bir Unit Test fonksiyonu ile test edelim. Bu amaçla BingMaps.Common.Test isimli bir test projesini göz önüne alabiliriz. Başlangıçta yapacağımız test istanbul şehrini birebir bulmak ile alakalı olacaktır. Buna göre test sınıfı kodlarını aşağıdaki gibi geliştirebiliriz.

```csharp
using BingMaps.Common.GeoCode; 
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BingMaps.Common.Test 
{   
    [TestClass()] 
    public class CommonOperationsTest 
    { 
        [TestMethod()] 
        public void FindIstanbulOkTest() 
        { 
            string city = "istanbul"; 
            string expected = "Istanbul, Turkey"; 
            GeocodeResponse actual; 
            actual = CommonOperations.GetLocationPoints(city); 
            Assert.AreEqual(expected, actual.Results[0].DisplayName); 
        }

        [TestMethod()] 
        public void ZeroDataFailTest() 
        { 
            string city = string.Empty; 
            string expected = null; 
            GeocodeResponse actual; 
            actual = CommonOperations.GetLocationPoints(city); 
            Assert.AreEqual(expected, actual); 
        } 
    } 
}
```

Testleri çalıştırmadan önce dikkat edilmesi gereken önemli noktalardan birisi de, servisler için Library tarafında üretilen App.config dosyasına ait system.serviceModel içeriğinin, Runtime uygulaması içerisinde de yer alması gerekliliğidir. Bir başka deyişle, BingMaps.Common assembly’ ı için üretilen App.config içeriğinin Test projesinde de yer alıyor olması gerekmektedir.

FindIstanbulIsOkTest metodumuzda elde edilen GeocodeResponse içerisindeki Results dizisine gidilmekte ve ilk nesne örneğinin DisplayName parametresinin “Istanbul, Turkey” olup olmadığına bakılmaktadır. Nitekim istanbul için yapılan arama sonucu bu şekilde dönecektir. Testi Pass edebildiysek, herşeyin yolunda olduğunu düşünebiliriz.

Diğer test metodumuz ise boş veri gönderdiğimiz vakayı ele almaktadır. Normal şartlarda boş içerik gönderdiğimizde servis tarafından bize bir FaultException dönmektedir. Bu nedenle, metoda gelen parametre değerinin boş veya null olması durumu tedbir olarak kontrol edilmiş ve ilgili fonksiyonelliğin GeocodeResponse değeri olarak null döndürmesi garanti edilmiştir.

[![bmwpf_4](/assets/images/2012/bmwpf_4_thumb.png)](/assets/images/2012/bmwpf_4.png)

GeocodeResponse nesne örneği üzerinden pek çok değere ulaşılabilmektedir. Söz gelimi aramaya konu olan lokasyon için elde edilecek her önerinin üzerinden ilgili Latitude ve Altitude değerlerine ulaşılabilir. GetLocationPoints metodumuzun çalıştığını gördüğümüz için örnek bir Console uygulamasında bahsettiğimiz senaryoyu değerlendirebiliriz. İşte örnek kod içeriğimiz.

```csharp
using System; 
using BingMaps.Common; 
using BingMaps.Common.GeoCode;

namespace ClientApp 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            Console.WriteLine("Bir lokasyon bilgisi giriniz"); 
            GetValue(Console.ReadLine()); 
        }

        private static void GetValue(string location) 
        { 
            GeocodeResponse response = CommonOperations.GetLocationPoints(location); 
            foreach (GeocodeResult r in response.Results) 
            { 
                Console.WriteLine("{0}", r.DisplayName);

                foreach (GeocodeLocation l in r.Locations) 
                { 
                    Console.WriteLine("\t(Enlem : {0}; Boylam : {1})" 
                                      , l.Latitude 
                                      , l.Longitude 
                        ); 
                } 
            } 
        } 
    } 
}
```

Burada elde edilen GeocodeResponse nesne örneğinin Results dizisinde dolaşılmakta ve içerisinde önerilen ne kadar GeocodeLocation bilgisi varsa, her biri için enlem ve boylam bilgileri ekrana yazdırılmaktadır. Örneğin arena yazdığımızda aşağıdaki sonuçları elde ederiz.

[![bmwpf_5](/assets/images/2012/bmwpf_5_thumb.png)](/assets/images/2012/bmwpf_5.png)

Bunlar Bing Maps servisinden bize dönen ve Arena ismi ile ilişkili olan lokasyonlar ve onlara ait enlem boylam bilgileridir.

Şimdi diğer servislerden Search hizmetine ait bir örnek metodu daha BingMaps.Common isimli kütüphanemize ekleyelim.

```csharp
public static List<string> Search(string keyword,string location) 
{ 
    List<String> results = new List<string>(); 
    if(String.IsNullOrEmpty(keyword)||string.IsNullOrEmpty(location)) 
        return results;

    Src.SearchRequest request = new Src.SearchRequest(); 
    request.Culture = "en-US";

    request.Credentials = new Src.Credentials(); 
    request.Credentials.ApplicationId = appKey;

    Src.StructuredSearchQuery query = new Src.StructuredSearchQuery(); 
    query.Keyword = keyword; 
    query.Location = location; 
    request.StructuredQuery = query;

    Src.FilterExpression atmosphereFilterExpression = new Src.FilterExpression(); 
    atmosphereFilterExpression.PropertyId = 23; // Atmosphere Property ID değeri 23 
    atmosphereFilterExpression.FilterValue = 10; // 10 = Romantic bir ortam arıyoruz 
    atmosphereFilterExpression.CompareOperator = Src.CompareOperator.Equals;

    Src.FilterExpression ratingExpression=new Src.FilterExpression() 
                { 
                    PropertyId = 3, //Rating property ID 
                    CompareOperator = Src.CompareOperator.GreaterThanOrEquals, //Rating değerine göre 3 ve üstünde olan yerler 
                    FilterValue = 7 // Kullanıcıların verdiği rating değeri 
                };

    Src.FilterExpressionClause combinedFilterExpressionClause = new Src.FilterExpressionClause(); 
    combinedFilterExpressionClause.Expressions = 
       new Src.FilterExpressionBase[] 
       { 
           ratingExpression, 
           atmosphereFilterExpression 
       };

    combinedFilterExpressionClause.LogicalOperator = Src.LogicalOperator.And;

    request.SearchOptions = new Src.SearchOptions(); 
    request.SearchOptions.Filters = combinedFilterExpressionClause;

    Src.SearchServiceClient searchService = new Src.SearchServiceClient(); 
    Src.SearchResponse response = searchService.Search(request);

    if (response.ResultSets[0].Results.Length > 0) 
    { 
        for (int i = 0; i < response.ResultSets[0].Results.Length; i++) 
        { 
            results.Add(String.Format("{0}\n({1};{2})\n", 
                response.ResultSets[0].Results[i].Name, 
                response.ResultSets[0].Results[i].LocationData.Locations[0].Latitude, 
                response.ResultSets[0].Results[i].LocationData.Locations[0].Longitude 
                ) 
                ); 
        } 
    }
    return results; 
}
```

Search servisi yardımıyla lokasyon bazlı olarak “ne, nerede?” tadında sorgulamalar yapabiliriz. Örneğin istanbul da yer alan kullanıcı tarafından 7 ve üzerinde değerlendirilmiş olan romantik restoranların listesi veya berlindeki petrol istasyonlarının bilgileri vb…Biz de örnek metodumuzda kullanıcılar tarafından Rating olarak 7 ve üzerinde not almış, romantik bir atmosfere sahip olan ortamları aratıyor olacağız. Burada büyük ihtimalle restoran, bar, coffee gibi ortamlar söz konusu olacaktır. Aslında bu kadar specific bir metod geliştirmek niyetinde değildim ama iki farklı arama kriterini nasıl kombine edebileceğimizi de göstermek istedim. Metodumuzu test etmek için şöyle bir vaka göz önüne alabiliriz.

> Las Vegas kentinde, insanlardan 7 ve üzerinde not almış, romantik ambiyansa sahip restoranları listeyelim.

[![bmwpf_7](/assets/images/2012/bmwpf_7_thumb.png)](/assets/images/2012/bmwpf_7.png)

Dikkat edilmesi gereken noktalardan birisi de şudur. WCF tabanlı bir servis söz konusu olduğundan, gelen cevaba ait veri içeriğinin standart byte boyutlarını aşması durumu oluşabilir. Bu durum çalışma zamanında bir Exception üretilmesine neden olacaktır.

> System.ServiceModel.QuotaExceededException: The maximum message size quota for incoming messages (65536) has been exceeded. To increase the quota, use the MaxReceivedMessageSize property on the appropriate binding element.

Dolayısıyla bu duruma karşılık tedbir olarak MaxReceivedMessageSize ve MaxBufferSize değerlerini, config dosyasından eşit olacak şekilde arttırmak gerekebilir ki örneğimizde ben bu sorunu yaşadığım için ilgili Fix işlemini yapmış bulunmaktayım.

[![bmwpf_8](/assets/images/2012/bmwpf_8_thumb.png)](/assets/images/2012/bmwpf_8.png)

Tabi bir de bunu istanbul için denemek lazım. Bu kutsal görevi de siz değerli okurlarıma bırakıyorum

![Wink](/assets/images/2012/smiley-wink.gif)

İlgili metodumuzu dilerseniz bir de Console uygulaması üzerinden kullanmaya çalışalım ve sonuçları görelim.

```csharp
using System; 
using BingMaps.Common; 
using BingMaps.Common.GeoCode; 
using System.Collections.Generic;

namespace ClientApp 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            Console.WriteLine("Ne arıyorsunuz?"); 
            string keyword = Console.ReadLine(); 
            Console.WriteLine("Nerede arıyorsunuz?"); 
            string location = Console.ReadLine();

            Search(keyword,location); 
        }

        private static void Search(string keyword,string location) 
        { 
            List<string> results = CommonOperations.Search(keyword,location); 
            foreach (string result in results) 
            { 
                Console.WriteLine(result); 
            } 
        } 
    } 
}
```

ve çalışma zamanı sonuçları

[![bmwpf_6](/assets/images/2012/bmwpf_6_thumb.png)](/assets/images/2012/bmwpf_6.png)

Şahsen Eiffel Tower Restaurant’ ı oldukça merak ettim. Kısmet, nasip olur mu bilmem ama bu restoran hakkında detaylı bilgiye [bu adresten](http://www.eiffeltowerrestaurant.com/) ulaşabilirsiniz.

Search servisi çok fazla detay veri içermektedir. Normal şartlarda maps.bing.com adresinden yapabileceğimiz basit sorgulamaları, kod tarafında geliştirici olarak hazır etmek pek de kolay değil. Özellikle filtre nesne örnekleri hazırlanırken bu durum daha da açığa çıkmakta. Servisin etkili kullanımı hakkında daha detaylı bilgiyi [MSDN adresinden](http://msdn.microsoft.com/en-us/library/cc966864.aspx) edinebilirsiniz.

Makalemizde son olarak Route servisini kullanarak iki lokasyon arasındaki rota bazlı yol bilgisinin nasıl elde edilebileceğini göstermeye çalışıyor olacağım. Bu amaçla CommonOperations sınıfımıza aşağıdaki metodu ilave edelim.

```csharp
public static Route.RouteResponse GetRoute(List<string> points) 
{ 
    if (points.Count < 2) 
        return null;

    List<Route.Waypoint> waypoints = new List<Route.Waypoint>(); 
    Route.RouteRequest routeRequest = new RouteRequest(); 
    routeRequest.Credentials = new Route.Credentials(); 
    routeRequest.Credentials.ApplicationId = appKey;

    foreach (string point in points) 
    { 
        Geo.GeocodeResponse response=GetLocationPoints(point); 
        if (response != null) 
        { 
            Waypoint wp = new Waypoint(); 
            wp.Location = new Route.Location(); 
            wp.Location.Latitude = response.Results[0].Locations[0].Latitude; 
            wp.Location.Longitude = response.Results[0].Locations[0].Longitude; 
            waypoints.Add(wp); 
        } 
        else 
        { 
            return null; 
        } 
    } 
    routeRequest.Waypoints = waypoints.ToArray();

    Route.RouteServiceClient routeService = new Route.RouteServiceClient(); 
    Route.RouteResponse routeResponse = routeService.CalculateRoute(routeRequest);

    return routeResponse; 
}
```

Metodumuzda iki ve daha fazla nokta girilmesi halinde bunlar arasındaki yol tarifini bulacak şekilde bir geliştirme yapılmıştır. Elbette noktalar arasındaki yolu sağlıklı bir şekilde tespit edebilmek için enlem ve boylam değerlerine ihtiyaç vardır. İlgili lokasyonlara ait enlem ve boylam değerleri yaklaşık olarak GetLocationPoints metodu yardımıyla bulunabilir. Şunu unutmayalım ki GetLocationPoints metodu geriye aranan kritere göre birden fazla lokasyon bilgisi döndürebilir. Biz her zaman için ilk değeri alıyoruz. İlaveten hiç bir lokasyon bilgisi dönmeyebilir de. Bu durumda zaten metod bir çalışma zamanı istisnasını Locations[0].Latitude değerini eklemeye çalıştığımız sırada verecektir (Index was outside of the bounds) Buna dikkat etmemiz gerekiyor. Rota tarifini bulabilmek için RouteService’ inin CalculateRoute metodundan yararlanılmaktadır. Hemen test metodumuzu geliştirerek devam edelim. Örneğin,

> Sirkeciden Pendiğe karayolu ile nasıl gidebiliriz? Yol tarifini bulmaya çalışalım.

Bu amaçla test metodunu aşağıdaki gibi geliştirebiliriz. Beklentimiz geriye null referans dönmemesi olacaktır.

[![bmwpf_10](/assets/images/2012/bmwpf_10_thumb.png)](/assets/images/2012/bmwpf_10.png)

Peki dönüş tipini gerçek bir uygulamada nasıl kullanabiliriz? Bu amaçla Console projemizde aşağıdaki geliştirmeleri yaptığımızı düşünelim.

```csharp
using System; 
using BingMaps.Common; 
using BingMaps.Common.GeoCode; 
using System.Collections.Generic;

namespace ClientApp 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            Console.WriteLine("Nereden?"); 
            string nereden = Console.ReadLine(); 
            Console.WriteLine("Nereye?"); 
            string nereye = Console.ReadLine(); 
            GetValue(nereye, nereden); 
        }

        private static void GetValue(string nereye, string nereden) 
        { 
            var result = CommonOperations.GetRoute(new List<string> {nereden, nereye});

            foreach (var leg in result.Result.Legs) 
            { 
                foreach (var i in leg.Itinerary) 
                { 
                    Console.WriteLine("{0}\n", i.Text); 
                } 
            } 
        }  
    } 
}
```

ve sonuç

[![bmwpf_11](/assets/images/2012/bmwpf_11_thumb.png)](/assets/images/2012/bmwpf_11.png)

Eh! Fena bir tarif sayılmaz. Tabi istanbul’ da yaşayan birisi olarak, yol durumuna göre bu tarif hiçe sayılabilir. Kestirme sokaklardan gidilebilir. Hatta bana kalsa ben arabalı vapur ile üsküdara geçer oradan pendiğe gitmek için farklı bir güzergah kullanırım

![Wink](/assets/images/2012/smiley-wink.gif)

Dönüş verisi tabi XML formatındadır. Bu sebepten bu içeriği parse ederek daha düzgün bir sonuç almak yerinde olacaktır. Bu size ödev olabilir mi acaba? Olur olur

![Smile](/assets/images/2012/smiley-smile.gif)

Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[BingMapSamples.zip (734,34 kb)](/assets/files/2012/BingMapSamples.zip)