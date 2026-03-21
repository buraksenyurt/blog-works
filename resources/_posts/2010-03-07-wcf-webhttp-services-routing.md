---
layout: post
title: "WCF WebHttp Services - Routing"
date: 2010-03-07 21:45:00 +0300
categories:
  - wcf-eco-system
  - wcf-webhttp-services
tags:
  - windows-communication-foundation
  - webhttp-services
  - rest-api
  - non-soap
  - wcf-webhttp-services
---
Geçtiğimiz aylarda yurdumuzun en güzel şehirlerinden birisi olan İstanbul'da, oldukça soğuk ve kar yağışlı günler geçirdik. Yandaki resimde görülen manzara çalıştığım ARI 1 binasının yürüyüş yolu üzerinde çok da yeni olmayan, dokunmatik ekran, Windows Mobile gibi ileri özellikleri bulunmayan ancak Carl Zeiss mercekli Sony Ericsson K810i marka cep telefonum tarafından çekilmiştir.

![blg138_Giris.jpg](/assets/images/2010/blg138_Giris.jpg)

3.1 Megapiksel ölçekli çekilen fotoğrafı kaliteli yapan ise elbetteki Carl Zeiss mercek. İnsan böyle zamanlarda eğer şartlar yerindeyse (çay, kahve, sıcak ve sessiz bir ortam, hızlı bir internet bağlantısı) işiyle ilgili pek çok konuda araştırma fırsatı bulabiliyor. Malum böyle havalarda sizde benim gibi evden çıkmamayı veya mesai sonrası şirkette bir kaç saat durmayı tercih edinenlerdenseniz, yapılacaklar listesinin belkide en başında yenilikleri araştırmak vardır diye düşünüyorum. Malum bir süredir de WCF Eco System'in önemli parçalarından birisi olan WebHttp Service'lerini incelemekte olduğumuza göre yine bu alanda ilerleyerek devam edebiliriz. Bu günkü konumuz ise WCF WebHttp servislerinde yönlendirme (Routing) işlemleri olacak.

Pek tabi, bir WCF REST Service Application içerisinde birden fazla WebHttp Service sınıfı konuşlandırılabilir. Bu çoğunlukla tek bir sınıfın var olduğu hallerde zaman içerisinde artan fonksiyon sayısı nedeniyle kavramsal bütünün bozulmasını engellemek amacıyla alınan bir tedbir olarak düşünülebilir. Aslında gerçek hayatta söz konusu kavramsal bütünlüğün ayrıştırılması ile ilişkili pek çok başarılı örnek yer almaktadır. Örneğin Sql Server 2005 ile gelen meşhur Adventure Works veritabanını göz önüne alalım. Burada bildiğiniz üzere çok sayıda tablo çeşitli şemalara (Schema) ayrılmıştır. Production, HumanResource, Sales vb...Böylece veritabanı alan modelinin (Domain Model) kavramsal olarak kolay bir şekilde ayrıştırılması mümkün olmaktadır. İşte benzer durum servis sınıflarının içeriğini oluşturan fonksiyonlar için de geçerli olabilir. Buna göre operasyonların ayrı servis sınıfları altında toplanıyor olması Adventure Works örneğindekine benzer kavramsal bir ayrımın yapılabilmesi manasına gelmektedir. Peki bir servisin yürüttüğü operasyonları sınıf bazında ayrıştırmanın uygulanışında nelere dikkat edilmesi gerekmektedir?

Herşeyden önce WebHttp Service'leri URI üzerinden gelen HTTP taleplerini değerlendirmektedir. Yani servise gelen bir talep, servis sınıfı içerisindeki bir operasyon ile ilişkilendirilmektedir. Bu noktada WCF WebHttp Service'lerinin konuşlandırıldığı Web uygulamasının global.asax içeriğinin büyük önemi vardır. Nitekim bu dosya içerisinde gelen talep için bir yönlendirme yapılması mümkündür. Bu amaçla RegisterRoutes isimli metoddan yaralanılır. Bir başka deyişle servisleri barındıran Web uygulaması ayağa kaldırıldığında, hangi URI taleplerinin hangi servislere yönlendirileceği bellidir. URI içerisinde yer alan operasyonel yönlendirmeler ise ilgili servis sınıfının metodlarına ait WebGet veya WebInvoke nitelikleri yardımıyla belirlenmektedir. Dolayısıyla istemcilerden gelecek olan taleplerin hangi servise yönlendirileceği sorusunun cevabı global.asax dosyası içerisinde verilmektedir. Hemen küçük bir not düşelim; bildiğiniz üzere Web taleplerinin daha etkin bir şekilde yönlendirilebilmesi yeteneği Asp.Net 4.0 ile birlikte gelmektedir. Bu sebepten geliştirilen WCF WebHttp servisinin Asp.Net uyumluluğu modunda (Asp.Net Compatibility Mode) çalıştırılıyor olması önemlidir.

Dilerseniz konuyu daha net kavrayabilmek adına basit bir örnek ile devam edelim. Bu amaçla Visual Studio 2010 Ultimate RC sürümü üzerinden geliştireceğimiz WCF REST Service Application'a ait Solution içeriğini aşağıdaki gibi tasarladığımızı düşünelim.

![blg138_Solution.gif](/assets/images/2010/blg138_Solution.gif)

Görüldüğü üzere CalculationService ve ProductionService isimli iki sınıfımız bulunmaktadır. Bu servis sınıfları içerisinde çok basit iki operasyon yer almakta ve her ikiside HTTP Get metodlarına göre cevap vermektedir. ProductionService sınıfı içerisinde Product tipinden değer döndüren bir arama operasyonu yer almaktadır. Diğer yandan CalculationService sınıfı içerisinde bir ürün fiyatına hangi günde bulunulduğuna göre indirim yapan ve sonuç değerini gösteren bir operasyon yer almaktadır. Yönlendirme teorimize göre servis uygulamasına gelen talepleri CalculationService ve ProductionService tipleri üzerine dağıtmak istiyoruz. Bunun için global.asax dosyasında gerekli kodlamaları yapmamız gerekecektir. Ama öncesinde CalculationService, ProductionService ve Product tipi içeriklerimize bir bakalım.

Sınıf diagramımız;

![blg138_ClassDiagram.gif](/assets/images/2010/blg138_ClassDiagram.gif)

Product sınıfımız;

```csharp
namespace Lesson5
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal ListPrice { get; set; }
    }
}
```

ProductionService sınıfımız;

```csharp
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

namespace Lesson5
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class ProductionService
    {
        private static List<Product> Products = new List<Product>
        {
            new Product{ ProductId=1023, Name="Microsoft Optical Mouse", ListPrice=34.95M},
            new Product{ ProductId=1045, Name="Logitech Optical Mouse", ListPrice=35.45M},
            new Product{ ProductId=1029, Name="KeyMaster Wireless Keyboard", ListPrice=55.99M},
            new Product{ ProductId=9802, Name="Obi Wan Kneobi Jedi Light Saber", ListPrice=334.45M},
        };

        [WebGet(UriTemplate = "Products/{ProductId}")]
        public Product GetProduct(string ProductId)
        {
            Product prd=null;

            try
            {
                prd = (from p in Products
                       where p.ProductId.ToString() == ProductId
                       select p).First();
            }
            catch
            {           
                // Bir Product bulunamama olasılığına karşın HTTP statü kodu 404 döndürülür
                throw new WebFaultException(System.Net.HttpStatusCode.NotFound);
            }

            return prd;
        }
    }
}
```

CalculationService sınıfımız;

```csharp
using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

namespace Lesson5
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class CalculationService
    {
        [WebGet(UriTemplate = "Discount/{ListPrice}")]
        public decimal CalculateDiscountListPrice(string ListPrice)
        {
            decimal decreasedListPrice = 0;
            decimal currentListPrice = 0;

            // Eğer string olarak gönderilen ListPrice değeri decimal tipe dönüşütürülemesse BadRequest tipinden bir statü kodu döndürülmesi sağlanır.
            if (!decimal.TryParse(ListPrice, out currentListPrice))
                throw new WebFaultException(System.Net.HttpStatusCode.BadRequest);

            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Friday:
                    decreasedListPrice = currentListPrice-(currentListPrice * 0.1M);
                    break;
                case DayOfWeek.Monday:
                    decreasedListPrice = currentListPrice-(currentListPrice * 0.2M);
                    break;
                case DayOfWeek.Saturday:
                    decreasedListPrice = currentListPrice;
                    break;
                case DayOfWeek.Sunday:
                    decreasedListPrice = currentListPrice;
                    break;
                case DayOfWeek.Thursday:
                    decreasedListPrice = currentListPrice-(currentListPrice * 0.3M);
                    break;
                case DayOfWeek.Tuesday:
                    decreasedListPrice = currentListPrice-(currentListPrice * 0.5M);
                    break;
                case DayOfWeek.Wednesday:
                    decreasedListPrice = currentListPrice-(currentListPrice * 0.2M);
                    break;
            }

            return decreasedListPrice;
        }
    }
}
```

Ve gelelim projemizin en önemli kısmına. Yönlendirme ile ilişkili olarak global.asax dosyasında yer alan RegisterRoutes metodunun içeriğini aşağıdaki gibi değiştirmemiz yeterli olacaktır.

```csharp
using System;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Routing;

namespace Lesson5
{
    public class Global 
        : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes();
        }

        private void RegisterRoutes()
        {
            // WebServiceHostFactory nesnesi örneklenir.
            WebServiceHostFactory hostFactory = new WebServiceHostFactory();

            // Route tablosuna gerekli eşleştirme bilgileri eklenir.
            // Gelen talep Productions içinse ProductionService servis tipi ile ilişkilendirilir.
            // Gelen talep Calculations içinse CalculationService servis tipi ile ilişkilendirilir.
            RouteTable.Routes.Add(new ServiceRoute("Productions", hostFactory, typeof(ProductionService)));
            RouteTable.Routes.Add(new ServiceRoute("Calculations", hostFactory, typeof(CalculationService)));
        }
    }
}
```

Uygulama başlatıldığında ApplicationStart metoduna girilecektir. Bu metod içerisinde ise RegisterRoutes fonksiyonu çağırılmaktadır. RegisterRoutes metodu içerisinde dikkat edileceği üzere iki ServiceRoute tipinin örneklenmesi ve bunların RouteTable üzerindeki Tables koleksiyonuna eklenmesi sağlanmaktadır. Buna göre servise gelen talepler aşağıdaki şekilde görüldüğü üzere eşleşen tiplere yönlendirilecektir.

![blg138_Case.gif](/assets/images/2010/blg138_Case.gif)

Hemen çalışma zamanı sonuçlarına bir bakalım. Örneğin http://localhost:10860/Productions/Products/1029 talebinde bulunduğumuzda aşağıdaki sonuçlar ile karşılaşırız. Görüldüğü üzere talep ProductionService'e yönlendirilmiştir. (Tabi var olmayan bir ProductID talebi girildiğinde HTTP Status Code 404 NotFound ile karşılaşırız. WebFaultException tipinin kullanımının anlatıldığı yazımızı hatırlayalım lütfen ![Wink](/assets/images/2010/smiley-wink.gif))

![blg138_Runtime1.gif](/assets/images/2010/blg138_Runtime1.gif)

Diğer yandan http://localhost:10860/Calculations/Discount/120,45 şeklinde bir talepte bulunduğumuzda ise CalculationService'e yönlendirildiğimizi görebiliriz. (Yine decimal tipe dönüştürülemeyen bir talep gönderildiğinde tahmin edileceği üzere HTTP Status Code 400 Bad Reuqest ile karşılaşırız.)

![blg138_Runtime2.gif](/assets/images/2010/blg138_Runtime2.gif)

Sonuç olarak UriTemplate'lerin ilgili servis operasyonları ile eşleştirilmesi çok daha kolaylaştırılmıştır. Öncelikle olarak gelen talebin hangi servis ile ilişkili olduğu noktasında Route Table devreye girmektedir. Ardından talep servise gelir. Servis ise URI içeriğine bakara uygun operasyonun çağırılması ile ilgilenir. Böylece geldik bir konumuzun daha sonuna. Bir sonraki yazımızda görüşünceye dek hepinize mutlu günler dilerim.

[Lesson5_RC.rar (24,11 kb)](/assets/files/2010/Lesson5_RC.rar) [Örnek Visual Studio 2010 Ultimate Beta 2 Sürümünde geliştirilmiş ancak RC sürümü üzerinde de test edilmiştir]
