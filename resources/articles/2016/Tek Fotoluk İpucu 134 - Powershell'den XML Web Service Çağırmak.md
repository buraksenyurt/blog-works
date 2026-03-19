---
title: "Tek Fotoluk İpucu 134 - Powershell'den XML Web Service Çağırmak"
pubDate: 2016-10-06 21:10:00
categories:
  - Powershell
tags:
  - Powershell
  - XML
  - Bash
  - .NET
  - ASP.NET
  - SOAP
  - Web Service
  - HTTP
  - ASMX
---

# Tek Fotoluk İpucu 134 - Powershell'den XML Web Service Çağırmak
Merhaba Arkadaşlar,

Olmaz olmaz demeyin, gün gelir ihtiyacınız olur:) Ortada bir XML Web Service olduğunu düşünelim ve onu çağırmak istediğimiz bir sunucunun başında oturduğumuzu. Sunucu üzerinde SOAP-UI gibi yardımcı araçların olmadığını ve bunları kurma yetkinizin de bulunmadığını düşünün. Öyle bir sunucu ki komut satırı en etkili araç. İşte böyle bir durumda Windows PowerShell gibi araçlar web servislerini çağırma noktasında işimize yarayabilir. Nasıl mı? Gelin bakalım.

Elimizde ASP.Net ortamında geliştirilmiş CalculationService.asmx isimli bir XML Web Service olduğunu ve içerisinde Sum isimli basit bir toplama operasyonu bulunduğunu varsayalım (Sum operasyonuna ait SOAP 1.1 temelli HTTP Post talebinin şablonu aşağıdakine benzerdir)

```xml
POST /CalculationService.asmx HTTP/1.1
Host: localhost
Content-Type: text/xml; charset=utf-8
Content-Length: length
SOAPAction: "http://www.buraksenyurt.com/servicebag/Sum"

<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <Sum xmlns="http://www.buraksenyurt.com/servicebag/">
      <x>double</x>
      <y>double</y>
    </Sum>
  </soap:Body>
</soap:Envelope>
```

PowerShell'de bu tip SOAP tabanlı servisleri Proxy sınıfı tanımlayarak çağırabilir ve operasyonlarını test edebiliriz. Bunun için New-WebServiceProxy komutundan yararlanıyoruz.

![TFI_134.gif](images/TFI_134.gif)

$URI değişkeni çağırmak istediğimiz web servisine ait WSDL (Web Service Description Language) adresini taşıyor. Sonrasında $proxy değişkenine New-WebServiceProxy tipi ile ürettiğimiz proxy nesnesini atıyoruz. Powershell tarafında kullanacağımız namespace ve class adlarını belirtiyoruz. Sonrasında $proxy değişkeni üzerinden web servis operasyonlarını çağırabiliriz. Servise ait diğer operasyonları görmek isterseniz aşağıdaki komutu da kullanabilirsiniz.

```bash
$proxy | Get-Member -MemberType method
```

Görüldüğü gibi sanki.Net ortamında bir istemci kodundaymışız gibi ilgili servisi çağırabildik. Ancak çok daha karmaşık senaryolar olduğunu da ifade edebiliriz. Örneğin servis operasyonları primitive tipler yerine complex tipler ile çalışıyor olabilir. Örneğin kategori bazlı ürün listesini döndüren bir servis operasyonunu Windows PowerShell üzerinden çağırmayı deneyebilirsiniz. Bunu nasıl yapabileceğinizi araştırmanızı öneririm.

Bir başka ipucunda görüşmek dileğiyle hepinize mutlu günler dilerim.
