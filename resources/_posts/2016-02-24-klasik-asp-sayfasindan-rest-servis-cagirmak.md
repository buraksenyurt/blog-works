---
layout: post
title: "Klasik ASP Sayfasından REST Servis Çağırmak"
date: 2016-02-24 12:00:00 +0300
categories:
  - rest
tags:
  - rest
  - csharp
  - xml
  - wcf
  - web-api
  - http
  - iis
  - ruby
  - serialization
---
Özellikle kurumsal çözümler üreten/kullanan firmalarda görev alanların sıklıkla dahil olduğu vakalardan birisi de, eski ve yeni teknolojilerin iç içe kullanıldığı senaryolardır. Bazen geliştirilen ürünler yıllara varan yaşam döngüleri boyunca çalışmaya devam eder. Yenileme maliyetlerinin yüksek olması nedeniyle de tekrardan yazılmak yerine var olan yeni teknolojiler ile entegre edilmeye çalışılırlar.

![asprest_0.gif](/assets/images/2016/asprest_0.gif)

İşte geçtiğimiz günlerde yine bizim turuncu bankamızda buna benzer bir ihtiyaç doğdu. Klasik ASP ile yazılmış ve neredeyse 10 yaşından büyük olan bir ürünün yeni nesil bir teknoloji ile entegre olması gerekti. Söz konusu ASP uygulaması bunca yıl çalıştığı için üzerine eklenen kodlar sebebiyle tam bir [Lawa-Flow AntiPattern](https://sourcemaking.com/antipatterns/lava-flow) oluşmasına da sebebiyet vermişti. (İçine giren kayboluyor herhangibir yerine müdahale etmek gerçekten yürek istiyordu) Ancak kullanıcı alışkanlıkları, yenileme maliyetleri ve kaynak sıkıntısı nedeniyle tekrardan yazılamıyordu. Ürünün güncel sıkıntısı ise içerdiği C tabanlı API'nin yeni nesil 64bit sunucularda çalışmamasıydı. İlgili kütüphane yıllarca önce dış kaynak bir firma tarafından yazılmıştı. İlgili firmadan çözüm için destek alınabilirdi. Şayet firma hala var olsaydı. Var olan C kütüphanesi banka dışı kurum ile SNA isimli eski bir protokol üzerinden haberleşme yapan fonksiyonellikler içeriyordu. Ne var ki dış kurum yakın zamanda bu protokolü terk edip TCP/IP tabanlı bir alt yapıya geçeceğini duyurmuştu. Dolayısıyla C kütüphanesinin değiştirilmesi öncelikli bir gereksinim haline gelmişti. Çözüm olarak C kütüphanesinin gerçekleştirdiği bu haberleşmeyi üstlenen REST tipinden bir servisin devreye alınmasına karar verildi. Problem basitti;.Net ile geliştirilecek olan REST servisin, HTTP POST/GET gibi metodlar ile çalışacak operasyonları klasik ASP sayfasından nasıl tüketilebilirdi? (Senaryomuzu şekilsel olarak aşağıdaki gibi özetleyebiliriz. Aslında canlı halini görseniz sarı kağıt üzerinde kurşun kalemli olan bu çizim gayet renkli ve canlı duruyor)

![asprest_1.gif](/assets/images/2016/asprest_1.gif)

## REST Servisin Geliştirilmesi

Şimdi bu problemi nasıl çözebileceğimize bir bakalım. İlk olarak basit bir WCF REST servis geliştireceğiz. Burada Web API, Service Stack veya diğer özel bir çözüm kullanabiliriz ([İlgili servisi Ruby ile yazabiliriz örneğin](https://www.buraksenyurt.com/post/ruby-kod-parcaciklari-20-rest-servis-gelistirmek)) Sonuç itibariyle REST tabanlı çalışan bir servise ihtiyacımız var. Ben basit olması açısından boş bir web uygulaması açıp içerisine aşağıdaki servis içeriğini eklemeyi tercih ettim.

ICommonService isimli servis sözleşmemiz

```csharp
using System.ServiceModel;
using System.ServiceModel.Web;

namespace ServiceSilo
{    
    [ServiceContract
    (Namespace="http://www.buraksenyurt.com/services/silo/common"
    ,Name="CommonOperation")
    ]
    public interface ICommonService
    {
        [OperationContract]
        [WebInvoke(
            Method="POST"
            ,ResponseFormat=WebMessageFormat.Xml
            ,RequestFormat=WebMessageFormat.Xml
            ,BodyStyle=WebMessageBodyStyle.Wrapped
            ,UriTemplate="doWork"
            )]
        string DoWork(string input);
    }
}
```

Servis sözleşmesinde önemli olan noktaların başında WebInvoke niteliğinin (attribute) kullanımı gelir. Dikkat edileceği üzere POST tipinden bir HTTP metod bildirimi söz konusudur (Tabii siz kendi denemelerinizde GET, PUT ve DELETE operasyonlarını da işin içerisine katabilirsiniz) Operasyonumuzun adı doWork olarak ifade edilmiştir. Request ve Response mesaj formatları ise XML şeklindedir. Klasik ASP tarafında XML ile daha rahat çalışabileceğimizi düşündüğümüzden bu yönde bir tercih yaptığımızı belirtebilirim.

> Web uygulamasında REST bazlı servis özelliklerini kullanabilmek için projeye System.ServiceModel.Web assembly'ının referans edilmesi gerekmektedir.

CommonService içeriği

```csharp
using System;

namespace ServiceSilo
{
    public class CommonService 
        : ICommonService
    {
        public string DoWork(string input)
        {
            string result = string.Format("{0}-Incoming Messaage : {1}",DateTime.Now.ToLongTimeString(),input);
            return result;
        }
    }
}
```

DoWork metodu özel bir şey yapmıyor. Sadece gelen içeriği alıp bir string değişkende kullanarak geri döndürüyor. Pek tabii gerçek hayat senaryosunda Data Transfer Object gibi request ve response formatları da kullanılabilir. Yani bir Entity içeriğinin (örneğin bir Kategori tipinin parametre olarak geçilip buna bağlı ürün listesinin döndürülmesi) parametre olarak geçişmesi ve buna göre bir karmaşık tipin gönderilmesi söz konusu olabilir. Şu an için önemli olan bu mesaj yapılarının klasik ASP tarafında nasıl kullanıldığıdır.

web.config

```xml
<?xml version="1.0"?>
<configuration>
    <system.web>
      <compilation debug="true" targetFramework="4.5.1" />
      <httpRuntime targetFramework="4.5.1" />
    </system.web>
    <system.serviceModel>
      <services>
        <service name="ServiceSilo.CommonService" behaviorConfiguration="RestBehavior">
          <endpoint 
            address="" 
            binding="webHttpBinding" 
            contract="ServiceSilo.ICommonService" 
            behaviorConfiguration="web"/>
        </service>
      </services>
        <behaviors>
            <serviceBehaviors>
                <behavior name="RestBehavior">
                    <serviceMetadata httpGetEnabled="true" httpsGetEnabled="true" />
                    <serviceDebug includeExceptionDetailInFaults="true" />
                </behavior>
            </serviceBehaviors>
          <endpointBehaviors>
            <behavior name="web">
              <webHttp helpEnabled="true"/>
            </behavior>
          </endpointBehaviors>
        </behaviors>
    </system.serviceModel>
</configuration>
```

Servis uygulamasına ait konfigurasyon içeriğinde dikkat edilmesi gerekenler webHttp endPoint davranışının uygulanması ve webHttpBinding Binding tipinin kullanılmış olmasıdır. Böylece servis uygulamamız HTTP Post, Put, Get ve Delete gibi metodlara cevap verebilecek çalışma ortamına kavuşmuş olacaktır. Örneği IIS üzerine publish ettikten sonra aşağıdaki ekran görüntüsündekine benzer yardım içeriklerine erişebilmemiz gerekmektedir.

![asprest_2.gif](/assets/images/2016/asprest_2.gif)

## Klasik ASP Sayfasının Geliştirilmesi

Senaryo gereği doWork isimli metoda bir POST çağrısı gerçekleştirmek istiyoruz. Bu nedenle ASP tarafında XML paketi şeklinde oluşturacağımız içeriği göndermenin yolunu bulmamız gerekiyor. Bunun için MSXML2.ServerXMLHTTP nesnesinden yararlanacağız. Örnek olarak aşağıdaki default.asp içeriğini göz önüne alabiliriz.

```text
<html>
<head>
<title>REST Service Call Sample</title>
</head>
<body bgcolor="black" text="green">
	<form method="post" action="default.asp">
		Input : <input type="text" name="txtInput"/>
		<p/>
		<input type="submit" value="HTTP Post"/>
	</form>
	<%
		Dim input
		input=Request.Form("txtInput")
		
		set xmlhttp = CreateObject("MSXML2.ServerXMLHTTP")    
		xmlhttp.open "POST", "http://localhost/ServiceSilo/CommonService.svc/doWork", false 
		xmlhttp.setRequestHeader "Content-Type", "text/xml"    
		xmlhttp.send "<string xmlns='http://schemas.microsoft.com/2003/10/Serialization/'>"&input&"</string>"
		serviceResponse =  xmlhttp.responseText 
		httpResponse = xmlhttp.status 
		httpStatusText = xmlhttp.statustext
		set xmlhttp = nothing 
		
		Response.Write("HTTP Status : " & httpResponse & "<p/>")
		Response.Write("Response : " & httpStatusText& "<p/>")
		Response.Write("Response text : " & serviceResponse& "<p/>")
		
	%>
</body>
```

Aslında ASP formu oldukça basit. Submit işlemi gerçekleştirildiğinde yine kendi üzerine dönen bir sayfa söz konusu. txtInput içeriği Request.Form ile alındıktan sonraki kısım bizim için daha önemli. MSXML2.ServerXMLHTTP nesnesi örneklendikten sonra open metoduna üç parametre veriyoruz. İlki HTTP metodunun tipi, ikincisi talebin gönderileceği adres (ki sondaki doWork uzantısına dikkat edelim) sonuncusu ise işlemin asenkron olarak gerçekleştirilip gerçekleştirilmeyeceğidir. open çağrısı sonrası HTTP Header için içerik tipi (Content-Type) belirlenir. Paketler XML formatında gönderilecektir. Nitekim servis tarafının kabul ettiği format budur (WebInvoke niteliğini bir kontrol edin) İzleyen adımda send metodu ile paket gönderimi gerçekleştirilir. Burada içeriğin XML formatında olduğunda lüften dikkat edelim. XML içeriğinin nasıl olması gerektiği noktasında, REST servisin help sayfasından yardım alabiliriz (http://localhost/ServiceSilo/CommonService.svc/help)

> Yazılan ASP sayfasının IIS 7.5 üzerinde host edildiğini belirteyim. Bu işlem sırasında Application Pool'un Classic modda çalıştırıldığında, IUSR kullanıcısı için Permission ayarlarının yapıldığını belirtmek isterim. IIS üzerinde klasik ASP kullanımı makale sınırlarımız dışında olduğundan derinlemesine detaylandırmıyorum.

Yapılan çağrıya ait sonuçlar xmlhttp değişkeni üzerinden yakalanbilirler. responseText ile gelen cevabı, status ile HTTP durum kodunu, statustext ile de durum koduna ait açıklamayı yakalayabiliriz. Bu işlemin ardından yapılan tek şey elde edilen sonuçları ekrana basmaktır. Örnek başarılı bir şekilde çalıştığında aşağıdakine benzer bir sonuç görülebilir.

![asprest_3.gif](/assets/images/2016/asprest_3.gif)

Özetleyecek olursak eski nesil bir ASP sayfasından, yeni nesil REST tipinden bir servisi çağırmayı başarabildiğimizi ifade edebiliriz. Tabii ki bu benim nasıl yapılır sorusu için uğraştığım ilk kod parçası. Asıl ihtiyacımızda bu servis entegrasyonunun var olan miras uygulamasının hangi ASP sayfalarında yapılacağının bulunması, gelen içeriklerin kodda kullanılabilir kıvama getirilmesi gibi gereksinimler söz konusu. Bize kolay gelsin. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
