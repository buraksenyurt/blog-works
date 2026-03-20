---
layout: post
title: "Tek Fotoluk İpucu 160 - Bir Sertifikanın Base64 Encoded Değerini Bulmak"
date: 2017-10-25 09:04:00 +0300
categories:
  - tek-fotoluk-ipucu
tags:
  - tek-fotoluk-ipucu
  - xml
  - dotnet
  - wcf
  - soap
---
[Önceki yazımızda](https://www.buraksenyurt.com/post/wcf-ozellestirilmis-usernamepasswordvalidator-kullanimi) WsHttpBinding kullandığımız sertifika tabanlı bir WCF senaryo çalışmamız vardı. Aynı örneği göz önüne alarak BasicHttpBinding kullanabileceğimizi de belirtelim. Nitekim bu bağlayıcı tipi ile de Message tabanlı güvenliği sertifika bazlı gerçekleştirebiliriz. Bunun en gerekli sebeplerinden birisi de servis tüketicilerinin eski nesil uygulamalar olabilmesi sebebiyle sadece SOAP 1.1 haberleşme kurmasıdır. Olmaz demeyin oluyor. Geliştirmekte olduğumuz projede buna benzer bir ihtiyaçla karşılaştık. Bazı servis tüketicilerimiz sadece SOAP 1.1 paketi gönderebilir durumdalar. Tabii öncelikle bizim.Net ortamında bu senaryoyu test edebilmemiz gerekmekteydi. Bağlayıcı tipini belirledik, Message güvenliğini ayarladık, sertifika tanımlamalarını yükledik ve servisi ayağa kaldırıp istemciye proxy tipini indirttik. İstemci web.config dosyasında gerekli ayarlamaları yaptık. Ne varki istemci tarafındaki endpoint bildiriminde yaptığımız aşağıdaki örnek tanımlama işe BasicHttpBinding tipi özelinde işe yaramadı ve çalışma zamanında "The request message must be protected" şeklinde hata aldık.

```xml
<identity>
	<dns value="AzonServer"/>
</identity>
```

Bunun üzerine istemci tarafında belirtilmesi gereken sertifikanın base64 formatında encode edilmiş halini denemeye karar verdik. Yani aşağıdaki şekilde.

```xml
<identity>
	<certificate encodedValue=""/>
</identity>
```

Tabii bir sertifikanın base64 bazlı halini nasıl elde edeceğimizi bilmiyorduk. Öğrendik. Nasıl mı oluyormuş? Aynen aşağıdaki ekran görüntüsündeki gibi.

![tfi_160.gif](/assets/images/2017/tfi_160.gif)

Koddaki AzonServer isimli sertifika CurrentUser->My deposu altında yer alıyor. Bu depoya ulaşmak için bir X509Store nesnesi örnekliyoruz. Sonrasında Certificates koleksiyonuna gidiyor ve SubjectName değerine göre (ki bu Comman Name'e denk gelecektir) arama yapıyoruz. Tabii ilgili sertifikanın olduğunu varsayıyoruz. Yoksa array için çalışma zamanı hatası alacağımız aşikar. Sertifikayı elde ettikten sonra bunu ihraç ediyor ve ToBase64String fonksiyonundan yararlanarak base64 encode edilmiş halini elde ediyoruz. Elde ettiğimiz içeriği de Clipboard'a (System.Windows.Forms kütüphanesini referans etmeli ve STAThread niteliğini kullanmalıyız) kopyalamayı ihmal etmiyoruz ki sonrasında tek yapacağımz şey Ctrl+V olsun. Bu hızlı ve ani çözümü uygulamak için Powershell'den de yararlanabilirsiniz bunu da belirteyim. Araştırın derim.

Bir başka ipucunda görüşmek üzere hepinize mutlu günler dilerim.