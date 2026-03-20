---
layout: post
title: "WCF Data Services– Annotations Builder"
date: 2011-10-14 14:00:00 +0300
categories:
  - wcf-data-services
tags:
  - wcf-data-services
  - csharp
  - xml
  - dotnet
  - ado-net
  - entity-framework
  - linq
  - wcf
  - http
  - authentication
  - generics
  - visual-studio
  - rc
---
Yağmurlu bu sonbahar günlerinde eminim ki kimse hüzünlenmek, kara kara düşünmek istemez. Ama bazen o kadar garip sorunlar ile karşı karşıya kalırız ki…Ne yapacağımızı bilemeyiz ve kara kara düşünürüz. Mesela ben bu girişi yazdığım sırada, aslında tamamlamış olduğum makalenin sonucunda bir yere varamamış ve beklentilerimi karşılayamamış birisi modundayım. Neden mi? Gelin anlatayım.

[![Huzun](/assets/images/2011/Huzun_thumb.jpg)](/assets/images/2011/Huzun.jpg)


Gün geçmiyor ki Microsoft ürünlerinde yeni bir sürüm, yeni bir güncelleme görmeyelim

![Gülümseme](/assets/images/2011/wlEmoticon-smile_18.png)

Şurada daha iki gün oldu Entity Framework 4.2 RC ile ilişkili bir şeyler yazmaya çalışmıştım. Bu gün de ne duyduk dersiniz? [WCF Data Services October 2011 CTP](http://www.microsoft.com/download/en/details.aspx?id=27728) yayınlanmış

![Gülümseme](/assets/images/2011/wlEmoticon-smile_18.png)

Biraz sitemkar bir giriş oldu ama boşverin…

Çok doğal olarak hemen arkasında söz konusu yeni CTP ile ilişkili ilk blog girişleri de Team Blog üzerinden yayınlanmaya başladı. Ben de bunun üzerine gelen yeniliklerden birisi olan Vocabularies kavramını özellikle getirildiği Annotations yapısı ile birlikte incelemeye ve anlamaya çalıştım. Dilerseniz yine tümden gelimden hareket edelim ve önce senaryomuzu geliştirip sonunda elde ettiğimiz (ve hatta elde edemediğimiz) bulguları değerlendirelim. (Bundan sonraki kısımda daha önceden WCF Data Service ile geliştirme yaptığınızı ve biraz aşina olduğunuzu varsayarak ilerleyeceğim)

Tabi ilk olarak [WCF Data Services October 2011 CTP](http://www.microsoft.com/download/en/details.aspx?id=27728) adresinden son sürümü indirmeniz gerekiyor. Bunu yaptığımız takdirde Visual Studio 2010 ortamına aşağıdaki ekran görüntüsünde yer alan yeni bir proje öğesinin eklendiğini görebiliriz.

[![bei_25](/assets/images/2011/bei_25_thumb.gif)](/assets/images/2011/bei_25.gif)

Bildiğiniz üzere WCF Data Service’ ler veri odaklı çalışan, HTTP protokolünün GET,POST,PUT,DELETE metodlarına cevap verebilen ve günümüzde OData formatına da destek sağlayan özelleştirilmiş WCF (Windows Communication Foundation) Servisleridir. Data Centric özellikleri nedeniyle de genellikle Ado.Net Entity Framework tabanlı ORM modelleri üzerinden sunulmaktadırlar. Bu nedenle örnek uygulamamızda çok basit olarak AdventureWorks veritabanındaki Product tablosunu modelleyen bir Entity Data Model olduğunu düşünerek ilerleyeceğiz. Söz konusu ekleme işlemi sonrasında Web uygulamamıza dahil edeceğimiz WCF DataService OCT 2011 CTP öğesinin kod içeriğini de aşağıdaki gibi geliştirebiliriz.

```csharp
using System.Collections.Generic; 
using System.Data.Services; 
using System.Data.Services.Common; 
using System.Xml; 
using Microsoft.Data.Edm; 
using Microsoft.Data.Edm.Csdl; 
using Microsoft.Data.Edm.Validation;

namespace AdventureWorksDataServices 
{ 
    public class ProductDataService 
: DataService<AdventureWorksEntities> 
    { 
        public static void InitializeService(DataServiceConfiguration config) 
        { 
            config.SetEntitySetAccessRule("Products", EntitySetRights.All); 
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;

            config.AnnotationsBuilder = (model) => 
                { 
                    IEdmModel edmModel; 
                    IEnumerable<EdmError> errors;

                    XmlReader[] readers =new XmlReader[] { 
                        XmlReader.Create("f:\\ProductsAnnotations.xml") 
                    };

                    bool parsed = CsdlReader.TryParse(readers, out edmModel, out errors, model); 
                    return parsed ? new IEdmModel[] { edmModel } : null; 
                }; 
        } 
    } 
}
```

Piuvvvv

![Kafası karışmış gülümseme](/assets/images/2011/wlEmoticon-confusedsmile_9.png)

Burada neler oldu böyle? Daha önceden yaptığımız Ado.Net Data Service geliştirmelerine hiç ama hiç benzemiyor gibi. Daha önceden sadece ilgili Entity tiplerini belirli erişim kuralları çerçevesinde (Örneğin sadece okuma amaçı açılsınlar) dış dünyaya açan bir InitializeService metodu bulunurdu aslında

![Göz kırpan gülümseme](/assets/images/2011/wlEmoticon-winkingsmile_69.png)

Aslında olayı kısaca açıklayayım.

DataServiceConfiguration tipine eklenen yeni AnnotationsBuilder özelliği aslında

public Func> AnnotationsBuilder

ile işaret edilen bir metodu göstermektedir. Bu metod içerisinde biz, Entity Data Model’ in CSDL (Conceptual Schema Definition Language) içeriğine müdahale ederek örnek bazı değişikliklerde bulunuyoruz. Bu değişiklikler koda göre XmlReader yardımıyla okunan ProductsAnnotations.xml isimli dosyada yer almaktadır. Dikkat edecek olursak eğer CsdlReader tipinin TryParse metodu bir den fazla XmlReader referansı alabilmektedir. Bir başka deyişle bir CSDL içeriğine n adet Annotation kuralının enjekte edilmesi de mümkündür. Örneğimizde kullandığımız dosya içeriği ise aşağıdaki gibidir.

```xml
<Schema Namespace="AdventureWorksModel" Alias="AdventureWorksModel" xmlns="http://schemas.microsoft.com/ado/2009/11/edm"> 
  <Using NamespaceUri="http://vocabularies.odata.org/Validation" Alias="Validation"/> 
  <Annotations Target="AdventureWorksModel.Product.ListPrice"> 
    <TypeAnnotation Term="Validation.Range"> 
      <PropertyValue Property="Min" Decimal="0" /> 
      <PropertyValue Property="Max" Decimal="5000" /> 
    </TypeAnnotation> 
  </Annotations> 
</Schema>
```

Burada son CTP ile gelen önemli bir nokta Using elementi ve içerisindeki NamespaceUri niteliğinin değeridir. Bu detaylar bir kenara dursun aslında bu dosyanın ne söylediğini anlamak şu anda bizim için çok daha önemlidir

![Göz kırpan gülümseme](/assets/images/2011/wlEmoticon-winkingsmile_69.png)

Dikkat edileceği üzere Annotations elementine ait Target niteliğinde bir tanımlama yapılmıştır. Buna göre AdventureWorksModel şemasında yer alan Product Entity tipinin ListPrice özelliği için Validation.Range formatında bir TypeAnnotation bildirimi yapılmaktadır. Bir başka deyişle ListPrice özelliği için çalışma zamanında üretilen metadata içeriğine enjekte edilecek bir doğrulama kalıbı sunulmaktadır.

Bu kalıba göre ListPrice özelliğinin Decimal veri tipinden olan minimum ve maximum alan değerleri belirtilmektedir. Senaryomuza göre ListPrice özelliği 0 ile 5000 birim arasında olmalıdır. İşte kod tarafında kullanılan TryParse metodu eğer XML dosyasını okuyabilme işlemi başarılı olursa, ilgili validasyonu çalışma zamanındaki metadata içeriğine dahil edecektir. Eğer Data Service örneği herhangibir tarayıcıda açılır ve metadata içeriğine ulaşılırsa aşağıdaki çıktı ile karşılaşıldığı görülecektir. (Benim örneğimde söz konusu Data Service adresi [http://localhost:4860/ProductDataService.svc/$metadata](http://localhost:4860/ProductDataService.svc/$metadata) şeklindedir)

[![bei_27](/assets/images/2011/bei_27_thumb.gif)](/assets/images/2011/bei_27.gif)

![Gülümseme](/assets/images/2011/wlEmoticon-smile_18.png)

Bakın burası çok önemli. Bir WCF Data Service’ in çalışma zamanındaki metadata çıktısına bir doğrulama kuralı enjekte edilmektedir. (Bu tip bir çalışma zamanı alt yapısını sıfırdan nasıl yazabileceğinizi, hangi tasarım kalıplarına veya yazılım prensiplerine ihtiyacınız olacağını bir düşünün ![Göz kırpan gülümseme](/assets/images/2011/wlEmoticon-winkingsmile_69.png))

Peki ama bu ne işimize yarayacak? Bunun için öncelikli olarak basit bir Client uygulaması geliştirmeyi düşünebiliriz. Console Application şeklinde tasarlayabileceğimiz bu uygulama çok basit olarak ilgili WCF Data Service’ i referans etmelidir. Sonrasında test amacıyla herhangibir Product nesne örneğinin ListPrice özelliğinin değeri 0-5000 aralığı dışında set edilerek nasıl bir sonuç elde edildiğine bakılabilir. Ben bu amaçla istemci tarafına ilgili servis referansını eklemeye çalışarak işe başladım. İşe başladım diyorum çünkü Visual Studio 2010’ a ait Add Service Reference ekleme aracı bir Namespace’ i bulamadığı için söz konusu proxy tipini üretmeyi başaramadı

![Üzgün gülümseme](/assets/images/2011/wlEmoticon-sadsmile_7.png)

[![bei_28](/assets/images/2011/bei_28_thumb.gif)](/assets/images/2011/bei_28.gif)

Ben de bunun üstüne söz konusu servis referansını komut satırından DataSvcUtil aracı ile üretmeyi denedim. İşte sonuç

![Üzgün gülümseme](/assets/images/2011/wlEmoticon-sadsmile_7.png)

[![bei_29](/assets/images/2011/bei_29_thumb.gif)](/assets/images/2011/bei_29.gif)

Hal böyle olunca tabi heyecanlı bir şekilde makaleyi yazmaya çalışan bir yazılım sevdalısının nasıl da hüsrana uğradığını zannediyorum ki anlayabilirsiniz

![Gülümseme](/assets/images/2011/wlEmoticon-smile_18.png)

Eğer Proxy üretimi başarılı bir şekilde gerçekleşseydi bu durumda aşağıdaki ilüsturasyon da yer alan bir kod parçasını denemeye tabi tutacaktık.

```csharp
using System; 
using System.Linq; 
using ConsoleApp.AdventureWorks;

namespace ClientApp 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            AdventureWorksEntities works = new AdventureWorksEntities(new Uri("http://localhost:4860/ProductDataService.svc"));

            var product = (from p in works.Products 
                           where p.ProductID == 1 
                           select p).FirstOrDefault(); 
            if (product != null) 
            { 
                product.ListPrice = 5001M; 
            } 
            works.UpdateObject(product);

            var response = works.SaveChanges(); 
        } 
    } 
}
```

ve özellikle örnek bir Product nesne örneğinin ListPrice özelliğinin değerini, Annotations’ da belirttiğimiz aralığın dışına çıkartmaya çalışacak ve SaveChanges çağrısı sonucu servis tarafından gelen Response’ a bakacaktık. Bu noktada tahmin edeceğiniz üzere beklentimiz validasyon ihlali nedeniyle Update işleminin yapılmaması ve buna uygun bir Response kodunun istemci tarafında döndürülmesi olacaktı. En azından benim beklentim bu yöndeydi.

Yine de buraya kadar yazdıklarımız ile aslında WCF Data Service tarafındaki Annotations bildirimlerinin, yeni CTP sürümü ile gelen Vocabularies namespace’ i ile ilişkilendirildiğini düşünebiliriz. Amaç ise Entity, Property, Navigation Property gibi bazı yapılarda doğrulama kurallarını uygulatabilmektir. Bu konu ile ilişkili olarak ben de [şu adresteki](http://blogs.msdn.com/b/astoriateam/archive/2011/10/13/vocabularies-in-wcf-data-services.aspx) girdiye ait yorumları takip etmeye çalışıyor olacağım. Malum istemci tarafı için gerekli bir testi gerçekleştiremedik/gerçekleştiremedim. Dolayısıyla şimdilik yazımızı sonlandırmak durumundayız. Buruk bir son oldu ama ne yapalım.

![Üzgün gülümseme](/assets/images/2011/wlEmoticon-sadsmile_7.png)

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.