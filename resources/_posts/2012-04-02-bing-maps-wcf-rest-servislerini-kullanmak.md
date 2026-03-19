---
layout: post
title: "BING Maps WCF Rest Servislerini Kullanmak"
date: 2012-04-02 04:00:00 +0300
description: "Hatırlayacağınız üzere BING Maps WCF Servislerini değerlendirdiğimiz bir önceki yazımızda, söz konusu hizmetlerden yararlanabilmek için proxy tiplerinden faydalanmıştık(Add Service Reference). Ancak bu servisleri sadece proxy tipleri üzerinden kullanmak gibi bir zorunluluğumuz bulunmamaktadır."
categories:
  - bing
tags:
  - bing
  - xml
  - json
  - csharp
  - dotnet
  - linq
  - wcf
  - rest
  - http
  - javascript
  - serialization
  - generics
---
# BING Maps WCF Rest Servislerini Kullanmak
[![Garfield-1](/assets/images/2012/Garfield-1_thumb.jpg)](/assets/images/2012/Garfield-1.jpg) Merhaba Arkadaşlar,

Bazen öğrenmek istediklerimiz bize inanılmaz karşık gelir. Ne zaman kitabı açsak ya da bilgisayarın başına geçsek işe zaten demoralize olmuş bir şekilde başlarız. Özellikle tez hazırlıkları safhasındayken veya yazacağımız kitap için gerekli araştırmaları yaparken çok yoğun, ağır, sıkıcı ve uğraştırıcı unsurlarla karşı karşıya kalabiliriz.

Yüksek Lisans yaptığım dönemlerdeki hocalarımdan birisi bu konuda şöyle bir tavsiye de bulunmuştu…

> Araştırmanızda bir nokta da tıkandınız, takıldınız mı?(Bir süre sessizlik)…Tatile çıkın. Bir iki hafta o konu ile hiç uğraşmayın. Döndüğünüzde soruna çok daha farklı bir şekilde bakacağınızı göreceksiniz
>
> ![Wink](/assets/images/2012/smiley-wink.gif)

Doğruyu söylemek gerekirse hangi yolu kullanırsak kullanalım, kendimizi nasıl motive etmek istersek isteyelim, bazen araştırdığımız konu da öyle bir nokta yakalarız ki, gerisi çorap söküğü gibi gelir. İşte bu yazımızda bu çorap söküğünü bulmaya çalışıyor olacağız.

Hatırlayacağınız üzere BING Maps WCF Servislerini değerlendirdiğimiz [bir önceki yazımızda](Bing Maps WCF Servisleri.md), söz konusu hizmetlerden yararlanabilmek için proxy tiplerinden faydalanmıştık (Add Service Reference). Ancak bu servisleri sadece proxy tipleri üzerinden kullanmak gibi bir zorunluluğumuz bulunmamaktadır. Özellikle son yıllarda ön plana çıkan REST tabanlı servis yaklaşımı sayesinde, ilgili hizmetlerden HTTP protokolünün GET metodunu kullanaraktan da yararlanabiliriz. Bu çok doğal olarak platform bağımsızlık avantajını da beraberinde getirecektir. İşte bu yazımızda BING Maps servislerine ait REST (Representational State Transfer) arayüz noktalarını nasıl kullanabileceğimizi çok basit bir örnek üzerinden incelemeye çalışıyor olacağız. Gerisi çorağ söküğü gibi gelecek

BING Maps Rest servisleri de toplamda dört arayüz ile karşımıza çıkmaktadır. Adres, nokta bazlı yer bulma işlemleri vb için Locations API, harita bazlı rota gösterme ve metadata bilgisi elde edebilmek vb için Imagery API, yürüme yolu, araç yolu veya transit geliş gidişlere yönelik rota çıkartılması vb için Routes API, coğrafi bir alandaki trafik durum bilgisini vb öğrenebilmek için de Traffic API REST servislerinden yararlanılabilmektedir.

Aslında tüm API arayüzleri HTTP protokolünün GET metoduna göre talep kabul etmekte ve istemci tarafına buna uygun olacak şekilde XML (eXtensible Markup Language) veya JSON (JavaScript and Object Notation) çıktısı döndürmektedir. Bu iki çıktı içeriği de W3C tarafından kabul görmüş birer standart olduğundan, herhangibir platform tarafından kolaylıkla kullanılabilir. Hatta en basit anlamda tarayıcı uygulamalar tarafından kolayca gösterilebilir. Dolayısıyla bilinmesi gereken, bu API arayüzlerine gönderilecek olan URL formatının nasıl olması gerektiği ve çıktıların kod tarafında nasıl ele alınabileceğidir.

İşe ilk olarak basit bir adım ile başlarsak gerisi çorap söküğü gibi gelecektir. Bu amaçla öncelikle bir adres bazlı lokasyon konumlandırma işinden başlayalım derim. Örneğin herhangibir tarayıcı üzerinden aşağıdaki talepte bulunduğumuzu düşünelim.

http://dev.virtualearth.net/REST/v1/Locations/US/NH/manchester/?o=xml&key={Buraya Developer API Key gelmeli}

Bu URL ifadesinden Locations REST arayüzüne (ve hatta 1.0 versiyonuna) bir talepte bulunduğu görülmektedir. US ile başlayan kısımda tahmin edileceği üzere ülke kodu belirtilir. Bu örnekte Amerika Birleşik Devleteri (United States) seçilmiştir. US kodunu izleyen kısımda ise bölge adının yazıldığı görülmektedir, NH. Sonraki kısımda ise şehir adı gelmektedir ki bu örnekte Manchester aranmaktadır.?o ile başlayan bölümde ise çıktı formatının tipi seçilir ki burada XML biçimi ele alınmıştır. Çok doğal BING Maps hizmetlerinden yararlanabilmek için bir Developer Key olması gerekmektedir. key anahtar kelimesinden sonra gelen kısımda da bu değer yazılır. Ben kendi developer key değişkenimi kullandığımda aşağıdaki XML çıktısını elde ettiğimi gördüm.

```xml
<Response> 
  <Copyright> 
    Copyright © 2012 Microsoft and its suppliers. All rights reserved. This API cannot be accessed and the content and any results may not be used, reproduced or transmitted in any manner without express written permission from Microsoft Corporation. 
  </Copyright> 
  <BrandLogoUri> 
http://dev.virtualearth.net/Branding/logo_powered_by.png 
  </BrandLogoUri> 
  <StatusCode>200</StatusCode> 
  <StatusDescription>OK</StatusDescription> 
  <AuthenticationResultCode>ValidCredentials</AuthenticationResultCode> 
  <TraceId> 
    3c54bda6227b45a58f6c9947a79021d8|LTSM001153|02.00.83.500|LTSMSNVM002004, LTSMSNVM001455, LTSMSNVM001474, LTSMSNVM001465, LTSMSNVM001451, LTSMSNVM002052 
  </TraceId> 
  <ResourceSets> 
    <ResourceSet> 
      <EstimatedTotal>1</EstimatedTotal> 
      <Resources> 
        <Location> 
          <Name>Manchester, NH</Name> 
          <Point> 
            <Latitude>42.991168975830078</Latitude> 
            <Longitude>-71.463088989257812</Longitude> 
          </Point> 
          <BoundingBox> 
            <SouthLatitude>42.936458587646484</SouthLatitude> 
            <WestLongitude>-71.510566711425781</WestLongitude> 
            <NorthLatitude>43.043960571289062</NorthLatitude> 
            <EastLongitude>-71.415084838867188</EastLongitude> 
          </BoundingBox> 
          <EntityType>PopulatedPlace</EntityType> 
          <Address> 
            <AdminDistrict>NH</AdminDistrict> 
            <AdminDistrict2>Hillsborough Co.</AdminDistrict2> 
            <CountryRegion>United States</CountryRegion> 
            <FormattedAddress>Manchester, NH</FormattedAddress> 
            <Locality>Manchester</Locality> 
          </Address> 
          <Confidence>Medium</Confidence> 
          <MatchCode>Good</MatchCode> 
          <MatchCode>UpHierarchy</MatchCode> 
          <GeocodePoint> 
            <Latitude>42.991168975830078</Latitude> 
            <Longitude>-71.463088989257812</Longitude> 
            <CalculationMethod>Rooftop</CalculationMethod> 
            <UsageType>Display</UsageType> 
          </GeocodePoint> 
        </Location> 
      </Resources> 
    </ResourceSet> 
  </ResourceSets> 
</Response>
```

Görüldüğü üzere söz konusu çıktı içerisinde Manchester mevkisinin coğrafik lokasyon bilgileri yer almaktadır. İlgili XML içeriğinin şemasını çıkarttığımızda ağaç yapısını daha kolay bir şekilde görebiliriz ve anlayabiliriz.

[![bngrest1](/assets/images/2012/bngrest1_thumb.png)](/assets/images/2012/bngrest1.png)

Aslında lokasyon ile ilişkili olarak asıl önemli bilgiler Resources/Location elementi altındaki boğumlarda yer almaktadır. Söz gelimi Name elementinde aranan kritere uygun olarak gelen lokasyonun adı, BoundingBox içerisinde kuzey, güney, doğu ve batı koordinatları, Point elementinde enlem ve boylam bilgileri vb yer almaktadır. Aranan içeriğin eşleşme oranı ise (yani bulunan sonucun aranan ile ne kadar yakın olduğu bilgisi de) MatchCode elementi içerisinde belirtilmektedir.

Aranan kritere göre çok daha fazla sonuç gelmesi de olasıdır. Örneğin,

http://dev.virtualearth.net/REST/v1/Locations/manchester/?o=xml&key={developer key}

şeklinde bir URL talebinde bulunduğumuzda bize an itibariyle 5 adet sonuç dönecektir. Nitekim burada ülke veya lokasyonu tam onikiden vurmak için gerekli ekstra bilgiler verilmemiştir. Sadece BING serverlarında kayıtlı olan manchester mevkisine ait veriler getirilmiş ve makalenin yazıldığı tarih itibariyle de 5 yakın sonuç bulunmuştur. (Örnek erkan görüntüsünün bir kısmı aşağıdaki gibidir)

[![bngrest4](/assets/images/2012/bngrest4_thumb.png)](/assets/images/2012/bngrest4.png)

Bir kaç farklı örnek daha ilave ederek REST arayüz içeriklerini incelemeye devam edelim.

http://dev.virtualearth.net/REST/v1/Locations/turkey/kadiköy/?output=xml&key={Developer key}

Yukarıdaki sorgu ile Türkiye’ deki Kadıköy ilçesinin lokasyon bilgisi elde edilebilir.

[![bngrest3](/assets/images/2012/bngrest3_thumb.png)](/assets/images/2012/bngrest3.png)

http://dev.virtualearth.net/REST/v1/Locations?output=xml&countryRegion=DE&key={Developer Key}

Bu seferki sorgu ile de countryRegion=DE anahtar değer çiftini kullanarak Almanya’ nın merkez koordinatlarını elde edebiliriz.

Peki çıktıyı JSON formatında almak istersek? Bu durumda URL sorgusundaki ufak bir değişiklik yapmamız yeterli olacaktır. Özellike WCF Data Service geliştiricileri, URLiçerisinde önceden tanımlı pek çok anahtar kelimenin kullanıldığını bilirler. BING Maps REST servislerinde de benzer bir durum söz konusudur. Nitekim? den sonra gelen kısımlarda anahtar kelime=değer şeklinde key-value çiftleri yer almaktadır. Bu çiftlerlerin sayısı & operatörü ile arttırılabilir ve birden fazla kriterin hesaba katılması sağlanabilir. Eğer o parametresinin değeri xml’ den json’ a çekilirse, bu karşı taraftaki BING Map Locations REST servisi için çıktının JSON formatında hazırlanması gerektiği anlamına gelecektir.

http://dev.virtualearth.net/REST/v1/Locations/us/nh/manchester/?o=json&key={developer key}

Bu URL talebinin sonucunda aşağıdaki içerikte görülen JSON formatlı çıktı elde edilmiştir.

```json
{"authenticationResultCode":"ValidCredentials","brandLogoUri": "http:\/\/dev.virtualearth.net\/Branding\/logo_powered_by.png","copyright":"Copyright © 2012 Microsoft and its suppliers. All rights reserved. This API cannot be accessed and the content and any results may not be used, reproduced or transmitted in any manner without express written permission from Microsoft Corporation.","resourceSets": 
[ 
    {"estimatedTotal":1,"resources": 
    [ 
        {"__type": "Location:http:\/\/schemas.microsoft.com\/search\/local\/ws\/rest\/v1","bbox": 
            [42.936458587646484, -71.510566711425781,43.043960571289062, -71.415084838867188],"name":"Manchester, NH","point": 
            {"type":"Point","coordinates":[42.991168975830078,-71.463088989257812]},"address": 
            {"adminDistrict":"NH","adminDistrict2":"Hillsborough Co.", "countryRegion":"United States","formattedAddress":"Manchester, NH","locality":"Manchester"},"confidence":"High","entityType":"PopulatedPlace","geocodePoints": 
                [{"type":"Point","coordinates":[42.991168975830078,-71.463088989257812] ,"calculationMethod":"Rooftop","usageTypes":["Display"]}],"matchCodes":["Good"] 
        } 
    ]} 
],"statusCode":200,"statusDescription":"OK","traceId":"d9816448d7e340c0a457ba230bd56310| LTSM001158|02.00.83.500|LTSMSNVM001472, LTSMSNVM002206, LTSMSNVM001475, LTSMSNVM001455, LTSMSNVM001465" 
}
```

Pek tabi JSON formatlı çıktılar, XML formatlı çıktılara nazaran çok daha az yer kaplamaktadır.

Diğer servislerinde REST arayüzlerini kullanmak suretiyle çeşitli aramalar yapabiliriz. Örneğin Routes API arayüzüne kısa bir bakış atalım. Bu arayüzü kullanarak yürüyüş, sürüş veya transit geliş gidişler için rota bilgisi elde etmemiz mümkündür. Eğer İstanbul’ dan Ankara’ ya doğru araba ile gideceğimiz bir rota bilgisi istersek, aşağıdaki gibi bir URL sorgusunu göndermemiz yeterli olacaktır.

http://dev.virtualearth.net/REST/V1/Routes/Driving?o=xml&wp.0=istanbul&wp.1=ankara&avoid=minimizeTolls&distanceUnit=km&key={Developer Key}

Sorgu sonucu elde edilen uzun XML çıktısına ait küçük bir ekran görüntüsü

[![bngrest2](/assets/images/2012/bngrest2_thumb.png)](/assets/images/2012/bngrest2.png)

Bu URL sorgusunda önemli olan bazı key’ ler vardır. wp.0 ve wp.1 ile tanımlanan anahtar kelimelere atanan değerler, sırasıyla WayPoint 1 ve WayPoint 2 anlamına gelmektedir. Bir başka deyişle, rota için gerekli başlangıç ve bitiş noktaları bilgileridir. avoid kelimesi seçimliktir ve burada atanan minimizeTolls değeri ile paralı yolların mümkün mertebe rota tanımından çıkartılması talep edilmektedir. distanceUnit=km anahtar değer çifti ile mesafelerin km cinsinden bildirilmesi sağlanmaktadır ki diğer seçenekte mil anlamına gelen mi’ dir.

> Diğer kullanılabilecek key değerleri için [developer sayfasına](http://msdn.microsoft.com/en-us/library/ff701717.aspx) bir göz atmanızı öneririm. Oldukça fazla sayıda alternatif bulunmaktadır.

Diğer API arayüzlerinden olan Static MAP Rest arayüzü ile de standart olarak 350X350 boyutlarında harita elde edilmesi mümkün olmaktadır. Bu haritayı uydu görüntüsü şeklinde, yol haritası şeklinde elde etmemiz de söz konusudur. Aslına bakarsanız BING Maps hizmetlerinin bana kalırsa en eğlencelilerinden birisi de bu API’ dir. Örneğin

http://dev.virtualearth.net/REST/v1/Imagery/Map/AerialWithLabels/istanbul?mapSize=400,300&key={developer key}

URL sorgusu sonucunda aşağıdaki çıktıyı elde ederiz.

[![bngrest5](/assets/images/2012/bngrest5_thumb.png)](/assets/images/2012/bngrest5.png)

Bu sorguda Imagery/Map/AerialWithLabels ile şehrin coğrafik haritasının başlık bilgileri kullanılarak gösterileceği belirtilmektedir. istanbul kelimesini takip eden kısımlarda ise mapSize anahtar kelimesi kullanılmış ve üretilecek olan haritanın 400,300 boyutlarında olması sağlanmıştır.

http://dev.virtualearth.net/REST/v1/Imagery/Map/Road/Routes?wp.0=istanbul&wp.1=kocaeli&format=png&mapSize=800,600&key={developer key}

Yukarıdaki sorguda ise, yol haritası istenmektedir. Map/Road adresine gidilmesinin sebebi budur. Diğer taraftan Routes anahtar kelimesine atanan iki Way Point değeri ile İstanbul ile Kocaeli arası yol haritasının gösterilmesi talep edilmiştir. Söz konusu harita 800X600 pixel boyutlarında olacaktır ve png formatında üretilecektir. İşte sonuç,

[![bngrest6](/assets/images/2012/bngrest6_thumb.png)](/assets/images/2012/bngrest6.png)

Imagery servisindeki diğer anahtar kelimeler için yine BING developer center’ daki [web sayfasını](http://msdn.microsoft.com/en-us/library/ff701724.aspx) ziyaret etmenizi öneririm.

Buraya kadar kı kısımda, söz konusu REST tabanlı servislerin HTTP GET talepleri ile nasıl sorgulanabileceğini anlamaya çalıştık. Görüldüğü gibi sorgu cümlesi içerisinde kullanılabilecek çok fazla sayıda key=value çifti bulunmaktadır. Sorguların sonuçlarını XML veya JSON formatında alabiliyor olmamız tüm platformların desteklenmesi açısından da oldukça önemlidir. Tabi Imagery servisi kullandığımızda çıktılar GIF, JPG veya PNG formatında olmaktadır.

Ancak çıktılar hangi format olurlarsa olsunlar, sonuç itibariyle bu üretimin kod tarafında anlamlı ve işe yarar hale getirilmesi gerektiğinden eminim ki hepimiz hem fikirizdir. Öyleyse dilerseniz basit bir kod parçası yardımıyla, Locations REST arayüzüne nasıl talepte bulunabileceğimize ve sonuçları nasıl değerlendirebileceğimize bakalım. Bu amaçla aşağıdaki kod parças ile işe başlayalım.

```csharp
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace BingRestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // BING Maps servisleri için kullanacağımız Developer Key değerini config dosyasından okuyoruz
            string apiDeveloperKey = ConfigurationManager.AppSettings["DeveloperKey"].ToString();
            // Basit bir sorgu giriyoruz. Birden fazla Location' ı ele almak istediğimizden sadece manchester değerini verdik
            string query = "manchester";
            // Tüm elemenler xmlNamespace değişkeni ile tanımlı Xml Namespace' i kullanmakta
            string xmlNamespace = "http://schemas.microsoft.com/search/local/ws/rest/v1";
            // Buna bağlı olarak gerekli XmlNamespace tanımlamalarını içeren XName değişkenleri oluşturuluyor. Bunlar XLINQ sorgusunda kullanılıyor olacak.
            XName resourceSetsName = XName.Get("ResourceSets", xmlNamespace);
            XName resourceSetName = XName.Get("ResourceSet", xmlNamespace);
            XName resourcesName = XName.Get("Resources", xmlNamespace);
            XName locationName = XName.Get("Location", xmlNamespace);
            XName name = XName.Get("Name", xmlNamespace);
            XName point = XName.Get("Point", xmlNamespace);
            // HTTP GET ile gidecek olan sorgu oluşturuluyor
            string url = String.Format("http://dev.virtualearth.net/REST/v1/Locations/{0}/?o=xml&key={1}", query, apiDeveloperKey);
            XElement locationElement = null;
            try
            {
                // XElement tipinin static Load metodu yardımıyla ilgili URL' e talepte bulunuluyor.
                locationElement = XElement.Load(url);

                // Basit bir XLINQ sorgusu kullanılarak ResourceSet elementlerine kadar iniliyor
                var resourceSet = from n in locationElement
                                 .Elements(resourceSetsName)
                                 .Elements(resourceSetName)
                                 .Elements(resourcesName)
                                 .Elements(locationName)
                                  select n;

                foreach (var r in resourceSet) //Her bir ResourceSet elementi dolaşılıyor
                {
                    Console.WriteLine(r.Element(name).Value); // önce Name elementinin değeri
                    foreach (var p in r.Elements(point)) // ardından her bir Point elementi içerisindeki her bir elementin değeri (ki bunlar longtitude ve latitude değerleri) dolaşılıp ekrana yazdırılıyor.
                    {
                        Console.WriteLine("\t{0}\t", p.Value);
                    }
                }
            }
            catch (WebException excp)
            {
                Console.WriteLine(excp.Message);
            }
        }
    }
}
```

Kodda dikkat edilmesi gereken noktalardan bir tanesi, XLINQ sorgularını gönderirken XmlNamespace kullanılmasıdır. Eğer bu Xml Namespace bilgisini ele almazsak söz konusu elementlerin hiç birisine ulaşamayız. Diğer yandan, URL bazlı olarak okuma işlemini gerçekleştirmek için, XElement tipinin static Load metodundan yararlanılmıştır. Eğer URL geçerli ise dönen sonuç XML formatında, XElement nesne örneğine yüklenecektir. Biz de bu içerik üzerinde dolaşarak sadece lokasyonun adını ve enlem ile boylam değerlerini ekrana yazdırmaya çalıştık. Örneğimizi çalıştırdığımızda aşağıdaki ekran görüntüsü ile karşılaşırız.

![bingrest10.png](/assets/images/2012/bingrest10.png)

Görüldüğü üzere manchester kelimesi için BING servisi dünya üzerinde 5 adet lokasyon bilgisi önermiştir. Çok doğal olarak örneğimiz XML formatını değerlendirecek şekilde geliştirilmiştir. Eğer JSON formatında bir içerik okumak istersek bu durumda JSON ile ilişkili serileştirme tipinden (DataContractJsonSerializer) yararlanabiliriz. (Bu konuda MSDN üzerinden yayınlanan [şu makaleyi](http://msdn.microsoft.com/en-us/library/hh674188.aspx)incelemenizi öneririm.)

Yazımızı sonlandırmadan önce son olarak Imagery servisine ait basit bir kod parçası geliştirelim. Imagery servisi bildiğiniz üzere XML veya JSON formatı yerine, JPEG, PNG veya GIF formatında resim çıktısı sunabilmektedir. Dolayısıyla bu servise yapacağımız talepleri farklı bir şekilde ele almamız gerekmektedir. İşte örnek kod parçamız.

```csharp
string imagerUrl = String.Format("http://dev.virtualearth.net/REST/v1/Imagery/Map/Road/Routes?wp.0=istanbul&wp.1= kocaeli&format=png&mapSize=800,600&key={0}", apiDeveloperKey);

try
{
	HttpWebRequest request = (HttpWebRequest)WebRequest.Create(imagerUrl);
	using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
	{
		List<byte> content = new List<byte>();
		using (Stream stream = response.GetResponseStream())
		{
			int currentByte;
			while ((currentByte = stream.ReadByte()) != -1)
			{
				content.Add((byte)currentByte);
			}
		}
		string filePath = String.Format("{0}.png", Path.Combine(Environment.CurrentDirectory, Guid.NewGuid().ToString()));
		File.WriteAllBytes(filePath, content.ToArray());
	}
}
catch (WebException excp)
{
	Console.WriteLine(excp.Message);
}
```

Bu sefer bir Stream kullanmak durumundayız. Nitekim URL olarak talep ettiğimiz sorgu sonucunda bize okunabilir bir grafik içeriği gelmekte. Talebi oluşturmak veresponse içeriğini almak için HttpWebRequest ve HttpWebResponse tiplerinden yararlandık. HttpWebResponse ile elde ettiğimiz nesne örneği üzerinden hareket ederek bir Stream oluşturduk ve bu Stream nesne örneği üzerinden resim dosyasına ait her bir byte içeriğini okuyarak generic bir List koleksiyonunda topladık. Çok doğal olarak elde edilen byte[] içeriği resim dosyasının kendisini ifade etmektedir. Bunu da sembolik olarak üretilen ve GUID ile isimlendirdiğimiz örnek bir dosyaya File tipininstatic WriteAllBytes metodu ile kaydettik. İşte sonuç;

![bingrest11.png](/assets/images/2012/bingrest11.png)

Artık çorabı yırttık diye düşünüyorum. Bir başka deyişle gerisinin çorap söküğü gibi gelmesi lazım. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim

![Wink](/assets/images/2012/smiley-wink.gif)

[BingRestClient.rar (1,20 mb)](/assets/files/2012/BingRestClient.rar)