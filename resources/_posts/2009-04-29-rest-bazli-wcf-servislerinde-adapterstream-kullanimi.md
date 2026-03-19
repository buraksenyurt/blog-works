---
layout: post
title: "REST Bazlı WCF Servislerinde AdapterStream Kullanımı"
date: 2009-04-29 16:03:00 +0300
description: "REST bazlı WCF servislerinde zaman zaman istemcilere içerik boyutları yüksek olan çıktılar veriyor olabiliriz. Bunlara örnek olarak resim veya metin dosyaları verilebilir. Aslında Stream veya TextWriter bazlı içerikler dersek çok daha doğru olacaktır.(Neden TextWriter olarak belirttiğimi yazının sonunda öğrenebileceğiz.) Özellikle istemci/sunucu bazlı uygulamalar göz önüne alındığında, büyük boyutlu içeriklerin karşı tarafa aktarılması sırasında karşılaşılabilecek pek çok performans kaybı söz konusudur."
categories:
  - wcf
tags:
  - wcf
  - xml
  - csharp
  - dotnet
  - linq
  - rest
  - http
  - performance
  - serialization
  - delegates
---
# REST Bazlı WCF Servislerinde AdapterStream Kullanımı
Merhaba Arkadaşlar,

REST bazlı WCF servislerinde zaman zaman istemcilere içerik boyutları yüksek olan çıktılar veriyor olabiliriz. Bunlara örnek olarak resim veya metin dosyaları verilebilir. Aslında Stream veya TextWriter bazlı içerikler dersek çok daha doğru olacaktır.(Neden TextWriter olarak belirttiğimi yazının sonunda öğrenebileceğiz.) Özellikle istemci/sunucu bazlı uygulamalar göz önüne alındığında, büyük boyutlu içeriklerin karşı tarafa aktarılması sırasında karşılaşılabilecek pek çok performans kaybı söz konusudur. Sunucu tarafından bakıldığında, istemcinin talep ettiği içeriğin Stream olarak elde edilmesi sırasında bellek ve işlemci bazında yüklenmeler olabilir. Buda sunucunun performansının olumsuz yönde etkiliyebilir. Nitekim kaynakların israfı söz konusudur. Tabi istemci tarafı açısından bakıldığında da, gelen Stream içeriğinin işlenmesi esnasında bazı sıkıntılar ile karşılaşılabilir.

Biz bu yazımızda sunucu tarafındaki içeriğin Stream olarak elde edilmesi sırasında performans kazanımı için ne yapabileceğimize bakacağız. Neyse ki çok fazla uğraşmamıza gerek yok. Nitekim WCF Rest Starter Kit ile birlikte gelen AdapterStream sınıfı tam bu iş için geliştirilmiş bir tip. Üstelik kit ile birlikte gelen örnek solution içerisinde kaynak kodunu görmenizde mümkün. Bu sınıf yardımıyla bir Stream'in hazırlanması sırasında, içeriğin tamamıyla değil, parça parça aktarılması sağlanabiliyor. Buda bir anlamda sunucunun bellek ve işlemci kaynaklarının daha az yorulması anlamına gelmekte.

Aslında hiç vakit kaybetmeden bu konu ile ilişkili geliştirdiğim örneği sizinle paylaşmak istiyorum. Öncesinde nacizane senaryomdan biraz bahsedeyim. Servis tarafında yer alan basit bir operasyon, talep ile kendisine gelen kelimeye bakarak, istemci için bir duvar kağıdı resminin üretilmesini sağlamakta. Burada gelen kelimeleri çok basit bir düşünce ile bazı resim dosyaları ile eşleştirmekteyim. Eşleştirme için basit bir XML dökümanı kullanıyorum. Böylece servis tarafına yeni resimler ve bu resimlere eş düşecek kelimelerin koda müdahale etmeye gerek kalmadan eklenmesi mümkün olabilir. Kabaca aşağıdaki gibi bir durumdan bahsediyorum aslında.

Servis projesinin klasör yapısı,

![blg10_1.gif](/assets/images/2009/blg10_1.gif)

ve Mapper.xml içeriği,

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Mapper>
  <Map Keyword="ay" Image="images/Ay.jpg"/>
  <Map Keyword="bizimsokak" Image="images/BizimSokak.jpg"/>
  <Map Keyword="cilgin" Image="images/cilgin.jpg"/>
  <Map Keyword="firtinaoncesi" Image="images/firtinaoncesi.jpg"/>
  <Map Keyword="gunes" Image="images/gunes.jpg"/>
  <Map Keyword="kopru" Image="images/kopru.jpg"/>
  <Map Keyword="maldivler" Image="images/maldivler.jpg"/>
  <Map Keyword="meksikasahili" Image="images/meksikasahili.jpg"/>
  <Map Keyword="mistik" Image="images/mistik.jpg"/>
  <Map Keyword="sahil" Image="images/sahil.jpg"/>
  <Map Keyword="tatil" Image="images/tatil.jpg"/>  
</Mapper>
```

Görüldüğü gibi images klasörü altındaki her bir resim ve eş düşen kelime, Xml içeriğinde tanımlanmış durumda. Tabi bu benim minik hayal gücümün bir ürünü. Buradaki sistem dahada etkili geliştirilebilir. Söz gelimi kullanıcının girdiği kelimeye göre, sunucu tarafında çalışacak akıllı bir robot, resim kataloğundan, kelime bire bir uymasa bile en yakın olanı bulup istemciye gönderebilir. Bu kısmı siz değerli okurlarıma bırakayım

![Wink](/assets/images/2009/smiley-wink.gif)

Gelelim projenin kod yapısına. Burada WCF Rest Starter Kit kullandığımız için herhangibir REST şablonuna ait projelerden birisini oluşturmak yeterli. Önemli olan noktalardan birisi servis tarafındaki çalışma zamanı için WebServiceFactory2 isimli fabrikanın (Factory Class) kullanılmasıdır. O yüzden Servis'e ait markup içeriğinin aşağıdaki gibi olmasına özen göstermekte yarar vardır.

```csharp
<%@ ServiceHost Language="C#" Debug="true" Service="Streaming.Service" Factory="Streaming.AppServiceHostFactory"%>

using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Microsoft.ServiceModel.Web;
using Microsoft.ServiceModel.Web.SpecializedServices;

namespace Streaming 
{
    class AppServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return new WebServiceHost2(serviceType, true, baseAddresses);
        }
    }
}
```

Servise ait kod içeriğini ise senaryoya göre aşağıdaki gibi geliştirdim.

```csharp
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Xml.Linq;
using Microsoft.ServiceModel.Web;

[assembly: ContractNamespace("", ClrNamespace = "Streaming")]

namespace Streaming
{ 
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceContract]
    public class Service
    {        
        [WebHelp(Comment="Şansına göre bir duvar kağıdı döndürür")]
        [WebGet(UriTemplate="yourimage?keyword={keyword}")]
        [OperationContract]
        public Stream GetImage(string keyword)
        {
            // Özellikle ilk talep sırasında keyword değeri gelmeyeceği için istemci tarafına BadRequest tipinden Http hatası döndürülür
            // İstenirse varsayılan bir resimde ürettirilebilir
            if (String.IsNullOrEmpty(keyword))
                throw new WebProtocolException(HttpStatusCode.BadRequest);

            // gelen kelimeye eş düşen resim adresi Xml dökümanı içerisinden LINQ sorgusu ile çekilir
            XDocument doc = new XDocument();
            doc = XDocument.Load(System.Web.HttpContext.Current.Server.MapPath("~/Mapper.xml"));
            // TODO: Xml içeriğinde Keyword niteliğinde olmayan bir bilgi gelirse aşağıdaki sorgu patlar. Buna tedbir alınması ve uygun Http hatasının döndürülmesi önerilir.
            string imagePath = (from img in doc.Document.Elements("Mapper").Elements("Map")                             
                            where img.Attribute("Keyword").Value == keyword.ToLower()
                            select img.Attribute("Image").Value).First();

            // eğer kelimeye denk düşen resim yoksa NotFound tipinden Http hatası döndürülür
            if (String.IsNullOrEmpty(imagePath))
                throw new WebProtocolException(HttpStatusCode.NotFound);

            // Image tipi resim adresinden üretilir.
            Image image = Image.FromFile(System.Web.HttpContext.Current.Server.MapPath(imagePath));            
            // Çıktının içerik tipi jpeg formatı olarak belirlenir.
            WebOperationContext.Current.OutgoingResponse.ContentType = "image/jpeg";
            // AdapterStream yapıcısı kullanılarak stream istemci tarafına hazırlanıp gönderilir.
            return new AdapterStream(str => image.Save(str, ImageFormat.Jpeg));
        }
    }
}
```

Kodun belkide en basit ama en önemli kısmı AdapterStream tipinin üretildiği satırdır. Bu satıra kadarki kısımda, gelen talebe göre Xml dökümanı içerisinden, denk düşen resim adresinin elde edilmesi ve buna göre Image nesnesinin örneklenmesi işlemleri yapılır. Sonrasında ise çıktının tipi (image/jpeg) olarak belirlenir ve son adıma gelinir. AdapterStream sınıfının yapıcı metodu parametre olarak Action veya Action tipinden bir temsilci (delegate) almaktadır. Bu temsilci, geriye değer döndürmeyen (void) ve parametre olarak bir Stream veya TextWriter referansı alan metodları işaret edecek şekilde tanımlanmıştır. Elimizde =>(lambda) operatörü gibi bir yardımcıda olduğundan nesne örneklenmesi sırasında temsilcinin tanımlanması, işaret ettiği metodun gövdesinin yazılması aynı ifade içerisinde mümkün olmaktadır.

Peki ya sonuçlar?

Servisi tarayıcıdan ilk seferde parametre kullanmadan talep ettiğimde aşağıdaki ekran görüntüsü ile karşılaştım.

![blg10_2.gif](/assets/images/2009/blg10_2.gif)

Bu son derece doğaldı. Nitekim bu talep sonrasında servis operasyonuna herhangibir keyword değeri gelmemektedir. Ancak tarayıcıdan, http://buraksenyurt:1000/Service.svc/yourimage?keyword=sahil gibi bir talepte bulunduğumda aşağıdaki sonucu elde ettim.

![Cool](/assets/images/2009/smiley-cool.gif)

Sanırım şansıma deniz kıyısında şöyle güzel bir tatil çıktı.

![blg10_3.gif](/assets/images/2009/blg10_3.gif)

Hemen arka planda çalışan Fiddler aracına baktığımdaysa, gelen talebe karşılık üretilen cevabın resim içerikli olarak üretildiğini gördüm.

![blg10_4.gif](/assets/images/2009/blg10_4.gif)

Görüldüğü gibi AdapterStream kullanımı son derece kolay. Özellikle REST bazlı WCF servislerini tüketen web uygulamalarında göz önüne alınabilir. Umarım size faydalı bir bilgi daha aktarabilmişimdir. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Streaming.rar (5,62 mb)](/assets/files/2009/Streaming.rar) (Dosya içerisinde bir TODO var. Bu kısmı gözden kaçırmayın

![Wink](/assets/images/2009/smiley-wink.gif)

)