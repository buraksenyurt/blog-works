---
layout: post
title: "Spring Boot ile Eski Usül Soap Based XML Web Service Yazmak Ne Kadar Zor Olabilir?"
date: 2020-10-26 17:51:00 +0300
description: "Kısa bir süre önce değerli bir çalışma arkadaşım kullanmaya çalıştığı Java tabanlı XML Web servis ile epeyce sorun yaşadı. Söz konusu servisi .Net tarafından tüketmeye çalışıyordu ancak XML şeması da epeyce karmaşık olan servis iletişim noktasında şema adlarına kızıyor, header içeriğini beğenmiyor sürekli naz yapıyordu. Arkadaşım allem etti kallem etti sorunun altından girip üstünden çıktı ve nihayetinde çözdü. Bu olaylara kısmen tanıklık ettikten sonra \"yahu Java tarafında XML Web Service geliştirmek şimdilerde daha bir kolay değil midir!?\" diye söylenmeye başladım. Yol doğal olarak beni Spring Boot'a ve resmi dokümantasyonuna götürdü..."
categories:
  - spring-boot
tags:
  - spring-boot
  - bash
  - csharp
  - xml
  - dotnet
  - mongodb
  - soap
  - rest
  - web-api
  - web-service
  - xml-web-services
  - http
  - grpc
  - java
  - visual-studio
  - github
  - dependency-management
---
# Spring Boot ile Eski Usül Soap Based XML Web Service Yazmak Ne Kadar Zor Olabilir?
![xmlheimdall.png](/assets/images/2020/xmlheimdall.png)

Kısa bir süre önce değerli bir çalışma arkadaşım kullanmaya çalıştığı Java tabanlı XML Web servis ile epeyce sorun yaşadı. Söz konusu servisi.Net tarafından tüketmeye çalışıyordu ancak XML şeması da epeyce karmaşık olan servis iletişim noktasında şema adlarına kızıyor, header içeriğini beğenmiyor sürekli naz yapıyordu. Arkadaşım allem etti kallem etti sorunun altından girip üstünden çıktı ve nihayetinde çözdü. Bu olaylara kısmen tanıklık ettikten sonra "yahu Java tarafında XML Web Service geliştirmek şimdilerde daha bir kolay değil midir!?" diye söylenmeye başladım. Yol doğal olarak beni Spring Boot'a ve resmi dokümantasyonuna götürdü.

Şimdiki amacımız yönergeleri takip ederek basit bir XML Web servisin Spring Boot çerçevesinde nasıl yazıldığını deneyimlemek. Günümüzde neredeyse tüm servisler REST, gRPC, OData ve benzeri kavramlar üzerinde konuşuyor olsalar da özellikle kurumsal çapta uzun yıllardır var olan pek çok uygulama halen daha SOAP (Simple Object Access Protocol) temelli XML Web servislerini kullanıyor. Nitekim SOAP protokolünün de dezavantajları kadar avantajlı olduğu taraflar var. Ancak konumuz bunu tartışmak yerine bir pratik yapmak.

Örneğimizde bir firmanın belli bir bölgedeki müşteri segmentine ait birkaç özet bilgisinin döndürüleceği kobay bir servis operasyonu söz konusu olacak. Mesela bölge ve müşteri segmenti seviyesini alan ve geriye o bölgedeki toplam işlem hacmi, müşteri memnuniyeti oranı, strateji özeti, temsilci sayısı gibi detayları döndüren bir operasyon pekala işe yarayabilir. Daha önceden baktığımız diğer Spring Boot çalışmalarında da olduğu gibi işe [https://start.spring.io/](https://start.spring.io/) adresine gidip projeyi üreterek başlamak gerekiyor. Uygulamanın birkaç bağımlılığı var. Spring Web, Spring Web Services ve wsdl4j (Java için WSDL kullanıcığımızı bildireceğimiz dependency) Detaylar için kodların da ye aldığı SkyNet [github reposuna](https://github.com/buraksenyurt/skynet/tree/master/No%2039%20-%20SOAP%20Based%20Web%20Service%20with%20Spring/src/services) uğrayabilir ve pom.xml dosya içeriğine bakabilirsiniz.

Yine belirtmem de yarar var; örneği Heimdall (Ubuntu-20.04) üzerine ve Visual Studio Code arabirimiyle geliştirmekteyim;)

![skynet_39_Screenshot_01.png](/assets/images/2020/skynet_39_Screenshot_01.png)

Şimdi de gerekli dosyalarımızı oluşturalım.

```bash
# SOAP Based Web Servisinin olmassa olmazı tabii ki içereceği operasyon
# ve bu operasyonların kullanacağı veri tiplerinin tanımlandığı
# XSD - XML Schema Definition dosyası (uygulama klasöründeyken)
# Firmanın belli bir şehirdeki müşterilerine ait istatistiki özet getirecek bir servisi olduğunu varsayalım
# XSD ile biraz haşırneşir olmak gerekiyor
touch src/main/resources/customer.xsd

# XSD şeması hazır. Request, Response verimizi tanımladık
# Kodda kolay kullanabilmek için onu karşılayacak bir Java sınıfı da gerekiyor
# Bunun için JAXB (Java Architecture for XML Binding) plug-in'ini kullanıyoruz
# XSD yi karşılayacak POJO (Plain Old Java Object) sınıfını otomatik üretiyor
# Ancak bu destek için pom.xml dosyasında bir plug-in tanımı eklemeliyiz (jaxb2-maven-plugin artifact'ini bulun)
# plug-in eklendikten sonra target klasöründe XSD'ye bağlı sınıflar otomatik olarak üretilir

# Şema hazır. Şema karşılığı POJO hazır. Peki ya asıl işi yapan sınıf.
# Veri odaklı bir iş söz konusu olduğu için bir Repository tasarlanması öneriliyor
touch src/main/java/com/bemewe/services/CustomerRepository.java

# Serivse gelecek SOAP talepleri çok doğal olarak kod tarafında bir sınıfla karşılanmalı
touch src/main/java/com/bemewe/services/CustomerEndpoint.java

# Repository ve Endpoint sınıfları hazır ancak yeterli değil. 
# Web Service konfigurasyonu için de bir dosya kullanacağız
touch src/main/java/com/bemewe/services/ServiceConfiguration.java
```

XSD şeması aslında okunabilir bir formatta. Servisin request ve response mesajları getCustomerUsageRequest ve getCustomerUsageResponse elementleri ile tanımlanıyor. Bu mesajların içeriğinde ilkel (primitive, xs önekli olanlar) ve karmaşık (complex, tns önekli alanlar) tipler kullanıyoruz. Yani mesajın hangi veri yapılarından oluşacağını XSD üzerinde tanımlıyoruz. Aynen aşağıdaki kod parçasında görüldüğü gibi.

```text
<xs:schema xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:tns="http://bemewe.com/services"
           targetNamespace="http://bemewe.com/services" elementFormDefault="qualified">

    <xs:element name="getCustomerUsageRequest">
        <xs:complexType>
            <xs:sequence>
                <xs:element name="region" type="xs:string"/>
                <xs:element name="size" type="tns:size"/>
            </xs:sequence>
        </xs:complexType>
    </xs:element>

    <xs:simpleType name="size">
        <xs:restriction base="xs:string">
            <xs:enumeration value="Light"/>
            <xs:enumeration value="Average"/>
            <xs:enumeration value="AverageHigh"/>
            <xs:enumeration value="MVP"/>
        </xs:restriction>
    </xs:simpleType>

    <xs:element name="getCustomerUsageResponse">
        <xs:complexType>
            <xs:sequence>
                <xs:element name="usageSummary" type="tns:usageSummary"/>
            </xs:sequence>
        </xs:complexType>
    </xs:element>

    <xs:complexType name="usageSummary">
        <xs:sequence>
            <xs:element name="totalTransactionVolume" type="xs:float"/>
            <xs:element name="size" type="tns:size"/>
            <xs:element name="avgCustomerSatisfaction" type="xs:float"/>
            <xs:element name="numberOfRepresentetive" type="xs:int"/>
            <xs:element name="evaluation" type="xs:string"/>
        </xs:sequence>
    </xs:complexType>
</xs:schema>
```

XSD içerisindeki tanımlamarın kod tarafında karşılığı olmazsa kullanmak oldukça zor olabilir. Bu nedenle pom.xml dosyası içerisinde jaxb2-maven-plugin tanımı söz konusu. Bu eklenti sonrası XSD'den otomatik olarak gerekli sınıflar üretilecektir. Örnekte yer alan servis operasyonumuzu CustomerRepository tipinde aşağıdaki gibi yazabiliriz.

```csharp
package com.bemewe.services;

import org.springframework.stereotype.Component;

/*
    Sembolik repository sınıfı. Bu bileşeni Service Endpoint sınıfı kullanıyor.
*/
@Component
public class CustomerRepository {

    /*
     * Servisin sunduğu fonksiyonelliklerden birisi belli bir bölge için müşteri
     * segmentine göre istatistiki bilgi döndürmek.
     * 
     * UsageSummary ve Size tipleri tahmin edeceğiniz üzere XSD'den otomatik
     * üretilen JAXB türleri.
     */
    public UsageSummary GetSummaryByRegion(String region, Size size) {
        UsageSummary summary = new UsageSummary();

        summary.setAvgCustomerSatisfaction(76.50F);
        summary.setNumberOfRepresentetive(12);
        summary.setTotalTransactionVolume(15000000.99F);
        summary.setSize(size);
        summary.setEvaluation("Daha agresit satış stratejilerine ihtiyacımız var");

        return summary;
    }
}
```

Aslında Repository için Interface kullanımını tercih etsek DI kurallarına daha uygun olur mu dersiniz?. Gelelim bu Repository'yi kullanan ve SOAP isteklerini karşılayacak olan Endpoint sınıfına.

```csharp
package com.bemewe.services;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.ws.server.endpoint.annotation.Endpoint;
import org.springframework.ws.server.endpoint.annotation.PayloadRoot;
import org.springframework.ws.server.endpoint.annotation.RequestPayload;
import org.springframework.ws.server.endpoint.annotation.ResponsePayload;

/*
    Endpoint annotation ile sınıfın bir servis endpoint olarak çalışacağını belirttik.
    (Kime belirttik derseniz cevap Spring WS modülü)
    Autowired ise constructor'ın Spring'in DI mekanizmasına otomatik olarak bağladı.
*/
@Endpoint
public class CustomerEndpoint {

    private CustomerRepository _repository;

    @Autowired
    public CustomerEndpoint(CustomerRepository repository) {
        _repository = repository;
    }

    /*
     * Bu Endpoint üzerinden sunduğumuz operasyonu karşılayacak olan metot. 
     * Gelen XML paketindeki hangi namespace ve operasyon bilgisine cevaben çalışacağını @PayloadRoot ile belirttik.
     * 
     * Gelen XML paketi metodun request parametresine bağlanacak. Bunu @RequestPayload ile bildiriyoruz.
     * 
     * Tam tersi metod çıktısınında cevaben dönecek XML paketinde map edileceğiniz @ResponsePayload ile belirtmekteyiz.
     * 
     * setUsageSummary'nin UsageSummary'yi otomatik olarak nasıl alabildiğini merak etmiş olabilirsiniz. 
     * Plug-In ile üretilen GetCustomerUsageResponse sınıfı bu bilgiyi XSD içeriğinden alıp gerekli sınıf üretimini gerçekleştirdi.
     */
    @PayloadRoot(namespace = "http://bemewe.com/services", localPart = "getCustomerUsageRequest")
    @ResponsePayload
    public GetCustomerUsageResponse getUsageSummary(@RequestPayload GetCustomerUsageRequest request) {
        UsageSummary summary = _repository.GetSummaryByRegion(request.getRegion(), request.getSize());
        GetCustomerUsageResponse response = new GetCustomerUsageResponse();
        response.setUsageSummary(summary);
        return response;
    }
}
```

Endpoint'te hazır ancak yeterli değil. Konfigurasyon için bir adaptör eklememiz de lazım. Adaptörümüz XSD dosyasını da baz alarak servisin çalışma zamanı kaydını açmakta. Bunu yaparken koddanda göreceğiniz gibi WSDL standardını da belirleyebiliyoruz. Örnekte 1.1 standardını baz almaktayız. 1.2'de sorun yaşadığımı hatırlıyorum. Ancak çözüm üreten olursa lütfen yorumlarını esirgemesin.

```csharp
package com.bemewe.services;

import org.springframework.boot.web.servlet.ServletRegistrationBean;
import org.springframework.context.ApplicationContext;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.core.io.ClassPathResource;
import org.springframework.ws.config.annotation.EnableWs;
import org.springframework.ws.config.annotation.WsConfigurerAdapter;
import org.springframework.ws.transport.http.MessageDispatcherServlet;
import org.springframework.ws.wsdl.wsdl11.DefaultWsdl11Definition;
import org.springframework.xml.xsd.SimpleXsdSchema;
import org.springframework.xml.xsd.XsdSchema;

/*
	WS Config dosyası olduğunu belirtiyoruz.
*/
@EnableWs
@Configuration
public class ServiceConfiguration extends WsConfigurerAdapter {
	@Bean
	public ServletRegistrationBean messageDispatcherServlet(ApplicationContext applicationContext) {
		MessageDispatcherServlet servlet = new MessageDispatcherServlet();
		servlet.setApplicationContext(applicationContext);
		servlet.setTransformWsdlLocations(true);
		return new ServletRegistrationBean(servlet, "/stats/*");
	}

	/*
	 * WSDL 1.1 standartlarına göre schema özelliklerini ayarlıyoruz. PortTipinin
	 * adını, Uri, namespace...
	 */
	@Bean(name = "customers")
	public DefaultWsdl11Definition defaultWsdl11Definition(XsdSchema serviceSchema) {
		DefaultWsdl11Definition wsdl11Definition = new DefaultWsdl11Definition();
		wsdl11Definition.setPortTypeName("CustomerPort");
		wsdl11Definition.setLocationUri("/stats");
		wsdl11Definition.setTargetNamespace("http://bemewe.com/services");
		wsdl11Definition.setSchema(serviceSchema);
		// wsdl11Definition.setCreateSoap12Binding(true); //SOAP 1.2 Binding oluşturmayı deneyeyim dedim
		return wsdl11Definition;
	}

	@Bean
	public XsdSchema countriesSchema() {
		return new SimpleXsdSchema(new ClassPathResource("customer.xsd"));
	}
}
```

Kodlama faslı tamamlandıktan sonra örneği maven üzerinden çalıştırıp http://localhost:8080/stats/customers.wsdl adresinden ilgili WSDL dosyasına (yani servis sözleşmesine) ulaşmayı deneyebiliriz.

```bash
./mvnw spring-boot:run
```

İlk etapta WSDL içeriğine ulaşabilmek beni olduğu kadar sizi de mutlu edecektir diye düşünüyorum. Artık elimizde SOAP standartlarında operasyon desteği sunabilen bir service var.

![skynet_39_Screenshot_02.png](/assets/images/2020/skynet_39_Screenshot_02.png)

Tabii bunu birde tüketmek gerekiyor. SoapUI bu anlamda ideal ve pratik bir çözüm. Ne varki ben ilk Request denemesinden sonra "Implementation of JAXB-API has not been found on module path or classpath" şeklinde bir hata aldım. Çözümü araştırdığımda konuya çalıştığım tarih itibariyle jaxb'nin aşağıdaki paketini kullanmam önerildi. Tabii siz bunu denerken aynı problemle karşılaşmayabilirsiniz.

```xml
<dependency>
	<groupId>com.sun.xml.bind</groupId>
	<artifactId>jaxb-impl</artifactId>
	<version>2.3.3</version>
</dependency>
```

POM.xml üzerinde gerekli değişikliği yaptıktan sonra ise ilk talebimizi gönderebiliriz.

```xml
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://bemewe.com/services">
	<soapenv:Header/>
	<soapenv:Body>
		<ser:getCustomerUsageRequest>
		<ser:region>East Dublin</ser:region>
		<ser:size>Average</ser:size>
		</ser:getCustomerUsageRequest>
	</soapenv:Body>
</soapenv:Envelope>
```

Aşağıdaki sonucu almamız gerekiyor.

![skynet_39_Screenshot_03.png](/assets/images/2020/skynet_39_Screenshot_03.png)

## Peki Ya.Net Core Tarafı!?

Aslında SOAP talebini HttpClient ve benzeri tipler ile yollayıp almak mümkün. Ancak ilk sürümlerinden beri.Net tarafında geliştirme yapanlar wsdl ve svcutil gibi araçların getirdiği kolaylıkları bilirler. Kullanmak istediğimiz servis için.Net tarafında proxy sınıfını oluşturmak kodlamada işimizi epey kolaylaştırıyor (du). Sorun şu ki SOAP tabanlı web servisler özellikle protokolün hantallığı dolayısıyla artık daha az kullanılmakta. Kurumsal projelerde var olanlar genellikle evrilmeye çalışıyor ve yerlerini hafif orta siklet Web API'lere bırakıyorlar. Yine de.Net Core tarafında bu tip servisleri tüketmek istediğimizde elimizde Visual Studio varsa servis referansını ekleyerek ilerlemek mümkün. Bununla birlikte dotnet-svcutil aracına başvurulabiliriz ama o da sistemde.Net Core 2.1 gerektiriyor (.Net 5 dünyasında durum nasıl bunu henüz incelemedim. Bilen varsa lütfen yorumlarını esirgemesin) Bununla birlikte SoapCore gibi üçüncü parti Nuget paketleri de alternatifler arasında yer alıyor.

Şimdilik benden bu kadar ancak sizin için iki güzel sorum var;) Sizce Soap Response mesajında N sayıda nesne içerecek bir koleksiyon döndürebilir miyiz; döndürebilirsek bunun için XSD'de nasıl bir değişiklik yapmamız gerekir. İkinci olarak customer.xsd için üretilen JAXB dosyalarının nerede olduğuna bakar mısınız? Bu sorulara ek olarak örneği zenginleştirmek elbette elinizde. Söz gelimi CustomerRepository üstünden gerçek bir veritabanına (MongoDB Container olabilir) bağlanıp ilgili istatistiklerin oradan gelmesini sağlayabilir veya servisi SOAP 1.2 standardına uyumlu hale getirebilirsiniz. Böylece geldik bir SkyNet derlememizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.