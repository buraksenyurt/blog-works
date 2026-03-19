---
layout: post
title: "Entity Framework, Data Services, C# 4.0, Excel ve Komple Bir Uygulama"
date: 2010-08-26 04:50:00 +0300
description: "Bildiğiniz üzere bir süre önce Visual Studio 2010 ve .Net Framework ürünlerinin RTM sürümleri yayınlandı. Her iki ürünüde sizlerle birlikte , Microsoft PDC 2008 konferanslarından bu yana gerek yazılarımızla, gerek görsel derslerimizle incelemeye çalışıyoruz. Özellikle .Net Framework 4.0 açısından baktığımızda alet, edevat çantamızın dop dolu olduğunun eminimki hepimiz farkındayız."
categories:
  - csharp-4-0
  - entity-framework
  - wcf-data-services
tags:
  - csharp-4-0
  - entity-framework
  - wcf-data-services
  - xml
  - csharp
  - dotnet
  - ado-net
  - linq
  - wcf
  - workflow-foundation
  - http
  - visual-studio
  - github
---
# Entity Framework, Data Services, C# 4.0, Excel ve Komple Bir Uygulama
![blg181_Giris.jpg](/assets/images/2010/blg181_Giris.jpg)

Merhaba Arkadaşlar,

Bildiğiniz üzere bir süre önce Visual Studio 2010 ve.Net Framework ürünlerinin RTM sürümleri yayınlandı. Her iki ürünüde sizlerle birlikte, Microsoft PDC 2008 konferanslarından bu yana gerek yazılarımızla, gerek görsel derslerimizle incelemeye çalışıyoruz. Özellikle.Net Framework 4.0 açısından baktığımızda alet, edevat çantamızın dop dolu olduğunun eminimki hepimiz farkındayız.

Paralel programlamadan tutun, WCF Eco System'e, C# 4.0 ile birlikte gelen yeniliklerden, WF 4.0 tarafına kadar pek çok noktada ek kabiliyetler, iyileştirmeler ve daha fazlası söz konusu. Aslında sizde benim gibi zaman zaman bu alet kutusu içerisindeki parçalardan bir kısmını alıp, örnek bir uygulamada kullanmaya çalışarak vaktinizi değerlendirmeye ve dolayısıyla offlama sorununa çare bulmaya çalışıyor olabilirsiniz. İşte bende bu düşünceler eşliğinde, havanın çok güzel olduğu şu bahar aylarında dışarıya çıkıp dolaşma şansını bulmama rağmen, evde kalıp örnek bir uygulama geliştirmeye karar verdim. İşte bu yazımız için alet çantası içinden seçtiklerimiz.

- Codeplex üzerinden yayınlanan bir adet [Chinook](http://chinookdatabase.codeplex.com/)veritabanı ([github'a taşındı](https://github.com/lerocha/chinook-database)) ![Wink](/assets/images/2010/smiley-wink.gif),
- Ado.Net Entity Framework 4.0,
- WCF Data Services,
- C# 4.0 Optional, Named Parameters,
- Microsoft.Office.Interop.Excel,
- ve tabiki Visual Studio 2010

Gelelim alet çantasından çıkarttığımız araçlar ile yapmak istediğimize...

![blg181_Dream.gif](/assets/images/2010/blg181_Dream.gif)

Öncelikli olarak Chinook veritabanının içeriğini Ado.Net Entity Framework üzerinden dış dünyaya sunan bir WCF Data Service örneğimiz olduğunu düşünebiliriz. Bu servisin sunduğu verinin istemcisi olan uygulama ise, talep ettiği içeriği alarak bir Excel uygulamas içersinde yayınlıyor olacak. Dolayısıyla Client uygulama tarafında Microsoft.Office.Interop.Excel.dll Assembly'ının referans edilmesi gerektiğini şimdiden söyleyebiliriz. Bu sayede Excel API'si yönetimli kod tarafından rahatlıkla konuşabiliyor olacağız. Diğer taraftan istemci uygulamada C# 4.0 ile birlikte gelen ve Office uygulamaları ile olan etkileşimde büyük avantajlar sağlayan Named ve Optional Parameters kavramlarının ele alınacağını da ifade edebiliriz. İlk etapta hedefimiz örnek olarak Track tablosundan, istemcinin belirttiği AlbumId değerine sahip olan satırları almak ve bunları örnek Excel uygulamasında açılacak Workbook üzerindeki bir Sheet içerisinde göstermek olacak. Projeye ait Solution içeriği herşey tamamlandığında aşağıdaki gibi olacaktır.

![blg181_Solution.gif](/assets/images/2010/blg181_Solution.gif)

İlk olarak ChinookEntityLayer isimli Class Library projesinin geliştirilmesi söz konusudur. Bu Library içerisine eklenen Ado.Net Entity Data Model içerisine, Chinook veritabanında yer alan tüm tabloları ekleyebiliriz. Örneğimizde çok basit bir operasyonu göz önüne alıyor olsakta, sizlerin bu örnekten ilham alarak farklı sorguları da işin içerisine katacağınıza eminim

![Wink](/assets/images/2010/smiley-wink.gif)

ChinookServices isimli WCF Service Application tipinden olan uygulama, içerdiği WCF Data Service sayesinde Chinook veritabanına ait Entity koleksiyonlarını dış ortama sunmaktadır.

Dolayısıyla bu uygulama, ChinookEntityLayer isimli sınıf kütüphanesini de referans etmelidir. Diğer yandan önemli olan noktalardan birisi de, Entity Context nesnesi tarafından kullanılan Connection String bilgisidir. ChinookEntityLayer içerisindeki app.config dosyasına eklenen Connection String içeriğinin aslında ChinookServices isimli WCF Service uygulamasının web.config dosyası içerisinde olması gerekmektedir. Çünkü çalışma zamanında oluşturulan Context nesne örneğinin yer aldığı proje burasıdır ve bu sebepten çalışma zamanı Connection String bilgisini Web.config içerisinde arayacaktyır.

```xml
<?xml version="1.0"?>
<configuration>
  <connectionStrings>
    <add name="ChinookEntities" connectionString="metadata=res://*/ChinookModel.csdl| res://*/ChinookModel.ssdl|res://*/ChinookModel.msl; provider=System.Data.SqlClient;provider connection string="Data Source=.; Initial Catalog=Chinook;Integrated Security=True; MultipleActiveResultSets=True"" providerName="System.Data.EntityClient" />
  </connectionStrings>
  <system.web>
    <compilation debug="true" targetFramework="4.0" />
  </system.web>
  <system.serviceModel>
    <serviceHostingEnvironment aspNetCompatibilityEnabled="true" />
  </system.serviceModel>
</configuration>
```

WCF Service uygulaması içerisinde yer alan ChinookDataService isimli WCF Data Service tipinden olan örneğin kod içeriği ise aşağıdaki gibidir.

```csharp
using System.Data.Services;
using System.Data.Services.Common;
using ChinookEntityLayer;

namespace ChinookServices
{
    public class ChinookDataService 
        : DataService<ChinookEntities>
    {
        public static void InitializeService(DataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V2;
        }
    }
}
```

Buna göre Chinook veritabanı içerisindeki Entity koleksiyonlarının tamamı sadece okunabilir olacak şekilde dış dünyaya sunulmaktadır.

Artık istemci tarafının geliştirilmesine başlanabilir. Console uygulaması tipinden olan istemci tarafına (Neden Console şeklinde tasarladığımı lütfen sormayın ![Smile](/assets/images/2010/smiley-smile.gif)) öncelikle ChinookDataService isimli WCF Data Service örneğinin referans edilmesi gerekmektedir. Söz konusu servis ile istemci uygulama aynı Solution içerisinde yer aldığında Add Service Reference seçeneğini aşağıdaki şekilde görüldüğü gibi kullanmak yeterlidir.(Hatırlanacağı üzere Astoria kod adlı Ado.Net Data Service'lerin Visual Studio 2008 üzerinden kullanılan sürümlerinde, Add Service Reference seçeneği kullanılamamktaydı. Bunun için datasvcutil aracından yararlanmamız gerekiyordu. Tabiki, Data Service için Add Service Reference desteği Visual Studio 2010 içerisinde mevcut)

![blg181_ServiceReference.gif](/assets/images/2010/blg181_ServiceReference.gif)

Artık istemci uygulama geliştirilmeye başlanabilir ki belki de işin en heyacanlı kısmı burasıdır

![Wink](/assets/images/2010/smiley-wink.gif)

İşte Console uygulamamız ait kod içeriğimiz.

```csharp
using System;
using System.Linq;
using ClientApp.ChinookDataServiceReference;
using Excel = Microsoft.Office.Interop.Excel;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // WCF Data Service örneğini kullanabileceğimi şekilde ChinookEntities nesne örneği oluşturulur.
            // URI Satırı söz konusu WCF Data Service örneğini işaret etmektedir.
            ChinookEntities entities = new ChinookEntities(new Uri("http://localhost:4071/ChinookDataService.svc/"));
            
            // Kullanıcıdan AlbumId bilgisi istenir
            Console.WriteLine("Album Id?");
            int albumID;
            if(!Int32.TryParse(Console.ReadLine(), out albumID)) //Eğer dönüşüm başarılı değilse 1 numaralı AlbumId değeri baz alınır
                albumID=1;

            // Sorgu cümlesi
            // AlbumId değerine göre Track örnekleri çekilir. Bu işlem sırasında Genre Entity örneklerine de ihtiyacımız olduğundan Expand metodu ile gerekli çağrı yapılır
            var result = from t in entities.Tracks.Expand("Genre")
                         where t.AlbumId == albumID
                         orderby t.Name
                         select t;

            // Bir Excel Application nesnesi örneklenir.
            Excel.Application excApp = new Excel.Application();
            excApp.Visible = true; // Excel uygulamasının görünebilir olacağı belirtilir
            excApp.Workbooks.Add(); // Yeni bir Workbook eklenir

            // Sütun başlıkları set edilmeye başlanır
            excApp.get_Range("A1").Value = "Track Name";
            excApp.get_Range("B1").Value = "Genre";
            excApp.get_Range("C1").Value = "Composer";
            excApp.get_Range("D1").Value = "Milliseconds";
            // Etkin olan Sheet adı belirlenir
            excApp.ActiveSheet.Name = "Track List for Album Id 1";

            // Elde edilen veri kümesindeki sonuçlar Sheet içerisindeki ilgili hücrelere yazdırlır
            int rowNumber = 2;
            foreach (var t in result)
            {
                excApp.get_Range(String.Format("A{0}", rowNumber.ToString())).Value = t.Name;
                excApp.get_Range(String.Format("B{0}", rowNumber.ToString())).Value = t.Genre.Name;
                excApp.get_Range(String.Format("C{0}", rowNumber.ToString())).Value = t.Composer;
                excApp.get_Range(String.Format("D{0}", rowNumber.ToString())).Value = t.Milliseconds;
                rowNumber++;
            }

            // Tüm sütunların uzunlukları içeriklerine göre otomatik olarak genişletilir
            for (int i = 1; i < 5; i++)
            {
                excApp.Columns[i].AutoFit();
            }
        }
    }
}
```

Console uygulaması kullanıcıdan bir AlbumId değeri istemektedir. Söz konusu AlbumId değerine göre WCF Data Service örneğine bir talep gönderilir ve bu talebin karşılığında dönen sonuç kümesi değerlendirilerek Excel içerisine alınması sağlanır. Uygulamanın çalışma zamanına ait örnek ekran çıktılarından birisi aşağıdaki gibidir.

![blg181_Runtime1.gif](/assets/images/2010/blg181_Runtime1.gif)

Ta taaaaaa!!!

![Laughing](/assets/images/2010/smiley-laughing.gif)

Bence güzel bir örnek oldu. Ancak daha da geliştirilmesi lazım. Her şeyden önce Console tipinde olan istemci uygulamadan kurtulmak ve görsel arayüze sahip bir örnek üzerinden ilermelek daha yararlı olacaktır. Bu size bir ödev olabilir mesela. Yazımızı sonlandırmadan önce benim sizlere bir kaç sorum olacak;

- Örnekte C# 4.0 ile birlikte gelen hangi yeni özellikler kullanılmıştır? (Daha önceki yazılarımızda değindik)
- WCF Data Service örneğine doğru gönderilen sorgu sonucunda elde edilen içeriğe göre, Excel üzerinde oluşturulacak sütun adları dinamik olarak belirlenebilir mi?
- WCF Data Service içerisinde kullanılan DataServiceProtocolVersion.V2 değeri ne anlama gelmektedir? (Daha önceki yazılarımızda değindik)
- Aynı örnek için şu tip bir sorguyu deneyip Excel çıktısını almaya çalışabilir misiniz? "Composer bazlı Track sayıları?"
- Uygulamanın sonunda Excel tablosunun otomatik olarak kayıt edilmesini sağlayabilir misiniz?
- WCF Eco System içerisinde yer alan Data Service dışındaki türler nelerdir? Bu servis türleri hangi amaçlarla kullanılmaktadır? (Daha önceki yazılarımızda değindik)
- İstemci uygulamaya ait exe dosyasının çıkartıldığı yerde Excel ile ilişkili bir Assembly bulunmamaktadır. Neden olduğunu bulup açıklayınız? (Daha önceki yazılarımızda değindik)

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[ServiceBasedExcel_RTM.rar (213,11 kb)](/assets/files/2010/ServiceBasedExcel_RTM.rar) [Örnek Visual Studio 2010 Ultimate RTM sürümü üzerinde geliştirilmiş ve test edilmiştir]