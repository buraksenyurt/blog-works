---
title: "XML Web Service Paketlerini Yakalamak"
pubDate: 2016-03-30 23:00:00
categories:
  - XML Web Services
tags:
  - XML Web Services
  - C#
  - XML
  - .NET
  - ASP.NET
  - Oracle
  - WCF
  - SOAP
  - Web Service
  - HTTP
  - ASMX
---

# XML Web Service Paketlerini Yakalamak
![wslog_0.gif](images/wslog_0.gif)

Merhaba Arkadaşlar,

Yine karşımıza çıkan bir problem çözümü ile birlikteyiz. Öncelikle senaryomuzu anlatarak işe başlayalım. Şirket içerisinde kullanılan bir XML Web Service'in Oracle tarafındaki bir Stored Procedure içerisinden çağırılması gerekiyordu. Burada kullanılacak teknikten ziyade XML Web Service'e gidecek olan SOAP mesajının içeriği daha önemliydi. Nitekim giden örnek bir mesaj elimizde olduğu takdirde Stored Procedure tarafındaki string içeriğin (SOAP XML yapısını ifade eden) oluşturulması daha kolay olacaktı. Haliyle bizim XML Web Service'e istemci tarafından gönderilen mesaj içeriklerine ait örneklere ihtiyacımız vardı. Bunun için Fiddler gibi araçları da kullanabilirdik. Ama bilin bakalım ne var? Şirketteki bilgisayarlara bu tip araçları indirip kurmamız mümkün değil. Dolayısıyla kendi kodumuzu yazarak servise giden paketlerin içeriğini görmemiz gerekiyor.

Bir XML Web Service'e giden ve dönen SOAP içeriklerini görmek için kullanılabilecek bir kaç yol var. Bunlardan birisi de konfigurasyon tabanlı. Nasıl yapacağımızı basit bir örnek üzerinden inceleyelim. Öncelikle bir XML Web Service'e ihtiyacımız var. Boş bir Asp.Net uygulaması oluşturduktan sonra (CommonWebService isimli bir uygulama olabilir) Operations.asmx isimli bir Web Service ekleyerek devam edelim.

```csharp
using System;
using System.Web.Services;

namespace CommonWebService
{
    [WebService(Namespace = "http://www.buraksenyurt.com/services/common/road/operations")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class Operations : 
        WebService
    {
        [WebMethod]
        public double FindOptimalDuration(Location[] locations)
        {
            //Do Something
            Random rnd=new Random();
            return rnd.Next(1, 60);
        }
    }
}
```

ve içeride kullandığımız Location tipi.

```csharp
namespace CommonWebService
{
    public class Location
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
}
```

Servisimiz içerisinde anlamsız bir operasyon var aslına bakarsanız. Location tipinden diziyi parametre olarak alan FindOptimalDuration metodu geriye güya en optimal yol süresini döndürüyor. Tabii bizim için önemli olan içerik alıp veren bir servis olması.

Çalışmamızın ikinci aşamasında ise bir istemciye ihtiyacımız var. Basit bir Console uygulaması bu aşamada işimizi görecektir. Kullandığımız servis eski stilde bir XML Web Service. Dolayısıyla istemci tarafına Add Service Reference seçeneği ile eklenirken aslında Add Web Reference kısmında ilerlememiz gerektiğini unutmamalıyız. Çünkü bir WCF Service söz konusu değil.

![wslog_1.gif](images/wslog_1.gif)

Sonrasında aynı Solution içerisindeki servisimizi ekleyerek ilerleyebiliriz.

![wslog_2.gif](images/wslog_2.gif)

İstemci tarafındaki kodlarımızı aşağıdaki şekilde yazmamız şu an için yeterli.

```csharp
using CommonWebClient.localhost;
using System;

namespace CommonWebClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Operations proxy = new Operations();
            Location[] locations = new Location[3];
            locations[0] = new Location { X = 10, Y = 10, Z = 10 };
            locations[0] = new Location { X = 20, Y = 5, Z = 40 };
            locations[0] = new Location { X = 30, Y = -10, Z = 50 };

            var result=proxy.FindOptimalDuration(locations);
            Console.WriteLine(result.ToString());
        }
    }
}
```

İstemci tarafında servise bir çağrı gönderiyor ve sonucu ekrana basıyoruz. Yazının can alıcı kısmı ise app.config dosyasının içeriği.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <configSections>
        <sectionGroup name="applicationSettings" type="System.Configuration.ApplicationSettingsGroup, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" >
            <section name="CommonWebClient.Properties.Settings" type="System.Configuration.ClientSettingsSection, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false" />
        </sectionGroup>
    </configSections>
    <startup> 
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5" />
    </startup>
    <applicationSettings>
        <CommonWebClient.Properties.Settings>
            <setting name="CommonWebClient_localhost_Operations" serializeAs="String">
                <value>http://localhost:51499/Operations.asmx</value>
            </setting>
        </CommonWebClient.Properties.Settings>
    </applicationSettings>
  <system.diagnostics>
    <trace autoflush="true"/>
    <sources>
      <source name="System.Net">
        <listeners>
          <add name="TraceFile"/>
        </listeners>
      </source>
      <source name="System.Net.Sockets">
        <listeners>
          <add name="TraceFile"/>
        </listeners>
      </source>
    </sources>
    <sharedListeners>
      <add name="TraceFile" type="System.Diagnostics.TextWriterTraceListener"
        initializeData="service.log"/>
    </sharedListeners>
    <switches>
      <add name="System.Net" value="Verbose"/>
      <add name="System.Net.Sockets" value="Verbose"/>
    </switches>
  </system.diagnostics>
</configuration>
```

Önemli olan kısım system.diagnostics sekmesinin içeriği. Burada servis tarafı ile ilgili mesajlaşma trafiğini dinleyeceğimizi belirtiyoruz. Aslında soket haberleşmesini yakaladığımızı ifade edebiliriz. initilalizeData niteliğinin değerine göre istemci uygulama çalıştırıldığında exe'nin olduğu klasörde service.log isimli bir dosya oluşacaktır.

![wslog_3.gif](images/wslog_3.gif)

Dosya içeriğine baktığımızda ise soket giriş ve çıkışlarının olduğu gibi indirildiğini görebiliriz.

![wslog_5.gif](images/wslog_5.gif)

Burada XML mesajlarını okumak biraz zahmetli olabilir ancak sonuç itibariyle istediğimiz XML paketlerini yakalamış bulunuyoruz. Gidip gelen XML içerikleri aşağıdakine benzer olacaktır.

İstemciden çıkan SOAP içeriği

```xml
<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
<soap:Body>
	<FindOptimalDuration xmlns="http://www.buraksenyurt.com/services/common/road/operations">
		<locations>
			<Location>
				<X>10</X>
				<Y>10</Y>
				<Z>10</Z>
			</Location>
			<Location>
				<X>20</X>
				<Y>5</Y>
				<Z>40</Z>
			</Location>
			<Location>
				<X>30</X>
				<Y>-10</Y>
				<Z>50</Z>
			</Location>
		</locations>
	</FindOptimalDuration>
</soap:Body>
</soap:Envelope>
```

Servisten istemciye gelen SOAP içeriği ise aşağıdaki gibidir.

```xml
<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
	<soap:Body>
		<FindOptimalDurationResponse xmlns="http://www.buraksenyurt.com/services/common/road/operations">
			<FindOptimalDurationResult>52</FindOptimalDurationResult>
		</FindOptimalDurationResponse>
	</soap:Body>
</soap:Envelope>
```

Bu adımdan sonra artık elimizde Oracle tarafı için gerekli örnek bir SOAP içeriğinin oluştuğunu ifade edebiliriz. Dilerseniz üretilen log dosyasının daha insancıl okunabilmesi amacıyla bir kod parçası geliştirmeyi deneyebilirsiniz. Bu mümkün. Araştırın ve deneyin. Böylece geldik bir yazımızın daha sonuna. Tekardan görüşünceye dek hepinize mutlu günler dilerim.
