---
layout: post
title: "WCF Web Http Services - ETags"
date: 2010-04-09 05:05:00 +0300
categories:
  - wcf-eco-system
  - wcf-webhttp-services
tags:
  - windows-communication-foundation
  - wcf-webhttp-services
  - rest-api
---
WCF WebHttp Service'leri ile ilişkili yazılarımıza kaldığımız yerden devam ediyoruz. Bu yazımızda ETag (Entity Tag) kullanarak sunucu ile istemci arasındaki veri trafiğini nasıl azaltabileceğimizi incelemeye çalışacağız. Öncelikle istemci ile servis arasındaki iletişimi düşünerek ilerlemeye çalışalım. İstemci, sunucu üzerinde yer alan bir operasyon için talepte bulunduğunda bir cevap üretilecek ve buna bağlı bir içerik verisi istemci tarafına indirilecektir.

![blg159_Giris.jpg](/assets/images/2010/blg159_Giris.jpg)

Bu süreç tipik olarak Request-Response senaryosundan farklı bir işleyiş değildir. İstemci sonraki bir zaman diliminde aynı operasyona yeni bir talepte bulunduğunda ise, üretilecek olan sunucu cevabının (Response) bir öncekine göre hiç değişmemiş olma ihtimalide bulunmaktadır. Eğer istemci tarafı bir şekilde gönderdiği talebin karşılığı olan cevabın değişmediğini anlayabilirse ve kendisinde bu içerik zaten tampon alanda duruyorsa, aynı içeriğin sunucudan istemci tarafına bir kere daha indirilmesine gerek yoktur. İşte ETag takısının devreye girdiği nokta burasıdır.

Peki [ETag (Entity Tag)](http://en.wikipedia.org/wiki/HTTP_ETag) tam olarak nedir? Entity Tag, sunucudan istemci tarafına gönderilen Response paketlerinin Header kısmında kullanılabilen bir takıdır. Bu takı yardımıyla bilginin değişikliğe uğrayıp uğramadığı kolayca anlaşılabilir. Bu ayrım bize performans açısından bir kazanım sağlayabilir. Öyleki, sunucu aynı ETag verisine sahip iki Response ürettiğinde, aslında istemcinin talebinin karşılığının bir öncekisi ile aynı olduğu sonucuna varılabilir. Bu noktada istemcinin içeriği tampon bir bölgede tuttuğu düşünüldüğünde, bir önceki ile aynı olan veri içeriğini sunucudan indirmesine gerek kalmayacaktır. Böyle bir durumda sunucun istemci tarafına HTTP 304 Not Modified bilgisi göndermesi söz konusudur. Tabi burada ETag içerisine yazılacak verinin nasıl üretileceği de önemlidir. Genellikle Entity ile alakalı olaraktan son güncelleme zamanı veya checksum kullanılabilir. Hatta SQL veritabanında kullanılabilen Timestamp tipide ETag verisi olarak ciddi anlamda düşünülebilir. Sonuç itibariyle ETag kavramının Caching modeli için çok önemli bir kriter olduğunu söyleyebiliriz.

Şimdi konuyu aşağıdaki içeriğe sahip bir WebHttp Service örneği üzerinden değerlendirmeye çalışalım.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

namespace Lesson2
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class ProductService
    {
        static List<Product> products = new List<Product>
        {
            new Product{ ProductId="1",Name="Zedojen 500 Gb HDD", Version=Guid.NewGuid()},
            new Product{ ProductId="2",Name="Zedojen 750 Gb HDD", Version=Guid.NewGuid()},
            new Product{ ProductId="3",Name="Leveno Laptop XK 5301", Version=Guid.NewGuid()}
        };

        [WebGet(UriTemplate = "Products/{productId}")]
        public Product GetProduct(string productId)
        {
            var product = (from p in products
                           where p.ProductId == productId
                           select p).FirstOrDefault();
            // Eğer bir Product nesne örneği mevcutsa...
            if (product != null)
            {
                // İstemciden gelen paket Header' ındaki If-None-Match değeri alınır ve sunucu tarafında bulunan Product nesne örneğinin Version özelliğinin değeri ile kıyaslanır. Eğer aynı ise bu istemci tarafına HTTP 304 Not Modified döndürüleceği anlamına gelir.
                WebOperationContext.Current.IncomingRequest.CheckConditionalRetrieve(product.Version);
                // Response içerisinde yer alan HTTP Header içerisindeki ETag değeri sunucu tarafından bulunan Product nesne örneğinin Version özelliği ile set edilir.
                WebOperationContext.Current.OutgoingResponse.SetETag(product.Version);
            }

            return product;
        }                
    }

    public class Product
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public Guid Version { get; set; }
    }
}
```

Servisimizde yer alan GetProduct isimli operasyon geriye Product tipinden bir nesne içeriği döndürmektedir. Product tipinin en önemli özelliklerinden birisi ise Guid tipinden olan Version'dur. Burada veritabanında yer alan bir ürünün ETag için kullanılabilecek veri tipi simüle edilmeye çalışılmaktadır. Yazımızın başında da belirttiğimiz gibi tablo bazlı kaynağın söz konusu olması halinde, Guid yerine Timestamp tipinden bir alan da tercih edilebilir.

GetProduct metodu içerisinde Exception kontrolü yapılmamaktadır. Daha çok üzerinde durmak istediğimiz nokta ETag veri kontrolü ve üretimidir. Bu nedenle WebOperationContext.Current üzerinden çağırılan CheckConditionalRetrieve ve SetETag metodlarına konsantre olmamızda yarar vardır. CheckConditionalRetrieve metodu istemciden gelen talebe ait içerikteki If-None-Match değeri kontrolünü yapmaktadır. Eğer istemci aynı içeriği bir kere daha talep etmişse, bu durumda istediği Product tipinin Version değeri ile gönderdiği If-None-Match değeri aynı olmalıdır. Tabiki buradaki örnek senaryomuzda şu an için veri tarafında yer alan Version alanının değişmediğini düşünüyoruz.

Özellikle tarih bazlı olarak tutulan veri içeriklerinde ve örneğin son güncelleme tarihinin ETag olarak kullanıldığı durumlarda ya da herhangibir değişiklik sonrası ilgili versiyon kontrolü alanlarının değerlerinin değiştirildiği hallerde, sunucu tarafından istemciye doğru veri indirilmesi (Download) işlemi yinelenecektir. SetETag metodu ise ilk gelen talep sonrası veya içeriğin istemciye indirilmesi gerektiği talep sonrası, Response'a ait içeriğe o anki Product nesne örneğinin Version değerini atayacaktır. Her iki metodunda farklı tipte aşırı yüklenmiş (Overload) versiyonları bulunmaktadır. Bu versiyonlardan birisi de örneğimizde ele aldığımız Guid veri tipi ile çalışanıdır. Dilerseniz durumu daha iyi anlamak için hemen testlerimize başlayalım. Örneğimizi tarayıcı uygulama üzerinden talep ettiğimizde ilk etapta aşağıdaki örnek sonuçlar ile karşılaşırız.

![blg159_Runtime.gif](/assets/images/2010/blg159_Runtime.gif)

> Burada ipv4.fiddler:1000 şeklindeki kök adres kullanımı mutlaka dikkatinizi çekmiştir. Bunu Fiddler üzerinden örneğimize ait HTTP paketlerini debug edebilmek için kullandığımızı belirtmek isterim.

Örnekte ProductId değeri 1 olan ürüne ait bilgilerin elde edildiği görülmektedir. Bundan sonra 1 numaralı ürünü tekrardan talep edecek olursak Fiddler tarafından aşağıdaki HTTP hareketliliklerinin yakalandığını görebiliriz.

![blg159_Fiddler1.gif](/assets/images/2010/blg159_Fiddler1.gif)

Dikkatinizi çeken bir şey var mı?

![Wink](/assets/images/2010/smiley-wink.gif)

İlk talep sonrasında sunucudan istemiye HTTP 200 Ok bilgisi dönmüş ve 237 Byte'lık bir Body içeriği indirilmiştir. Diğer yandan aynı talebin ikinci kez yapılması sonrasında istemci tarafına HTTP 304 Not Modified mesajının döndürüldüğü görülmektedir. Üstelik ikinci talep sonrası Body içeriği 0 byte uzunluğundadır. Volaaa!!!

![Laughing](/assets/images/2010/smiley-laughing.gif)

Yani ikinci talebin ilki ile aynı veri üretimine sahip olduğu anlaşılmış ve bu sebepten üretilen paketin istemci tarafına yeniden indirilmesine gerek kalınmamıştır. Örneğimize göre byte seviyesinde bu çok önemli bir performans kazanımına neden olmamaktadır.

Ne varki video, resim, müzik gibi büyük boyutlu binary içeriklerin yer aldığı paketlerde 304 döndürülmesinin büyük önemi vardır. Nitekim az önce üretilip istemci tarafına indirilen büyük boyutlu içeriğin, ikinci talep sonrası zaten istemci tarafındaki tamponda duran versiyonu ile aynı olması nedeniyle, yeniden gönderilmesi durumu ortadan kaldırılmakta ve böylece istemci ile sunucu arasındaki ağ trafiğinden akan veri boyutu minimize edilmektedir.

Şimdi Fiddler aracı yardımıyla paketlerin içeriklerine biraz daha yakından bakalım. İlk talep sonrası istemcinin gönderdiği içerik aşağıdaki gibidir.

```text
GET http://127.0.0.1:1000/Adventures/Products/1 HTTP/1.1
Accept: application/x-ms-application, image/jpeg, application/xaml+xml, image/gif, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, application/x-shockwave-flash, */*
Accept-Language: tr
User-Agent: Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; InfoPath.2; OfficeLiveConnector.1.4; OfficeLivePatch.1.3; .NET4.0C; .NET4.0E; MS-RTC LM 8)
Accept-Encoding: gzip, deflate
Connection: Keep-Alive
Host: 127.0.0.1:1000
```

Bu talebe karşılık sunucunu cevabı ise aşağıdaki gibi olacaktır.

```text
HTTP/1.1 200 OK
Server: ASP.NET Development Server/10.0.0.0
Date: Fri, 26 Feb 2010 09:43:00 GMT
X-AspNet-Version: 4.0.30128
Content-Length: 237
ETag: "2c1cd636-3058-4985-8f10-3d3cb8c9e5fa"
Cache-Control: private
Content-Type: application/xml; charset=utf-8
Connection: Close

<Product xmlns="http://schemas.datacontract.org/2004/07/Lesson2" xmlns:i="http://www.w3.org/2001/XMLSchema-instance"><Name>Zedojen 500 Gb HDD</Name><ProductId>1</ProductId><Version>2c1cd636-3058-4985-8f10-3d3cb8c9e5fa</Version></Product>
```

Dikkat edileceği üzere Response içerisinde bir ETag değeri olduğu görülmektedir. Sizce bu değerin 1 numaralı Product'ın güncel Version değeri ile aynı olması bir tesadüf müdür?

![Wink](/assets/images/2010/smiley-wink.gif)

Ayrıca içeriğin uzunluğu 237 byte'tır.

Gelelim ikinci talebe. İstemci tarafından sunucuya gönderilen ikinci talebin içeriği aşağıdaki gibidir.

```text
GET http://127.0.0.1:1000/Adventures/Products/1 HTTP/1.1
Accept: application/x-ms-application, image/jpeg, application/xaml+xml, image/gif, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, application/x-shockwave-flash, */*
Accept-Language: tr
User-Agent: Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; InfoPath.2; OfficeLiveConnector.1.4; OfficeLivePatch.1.3; .NET4.0C; .NET4.0E; MS-RTC LM 8)
Accept-Encoding: gzip, deflate
If-None-Match: "2c1cd636-3058-4985-8f10-3d3cb8c9e5fa"
Host: 127.0.0.1:1000
Connection: Keep-Alive
```

Dikkat edileceği üzere Guid değerine sahip olan If-None-Match isimli element bulunmaktadır. Bu değer biraz önceki talep sonucu istemciye gönderilen ETag değeridir aslında. Şimdi bu noktada sunucu üzerinde yer alan 1 numaralı ürünün içeriğinin değiştirilmediği ve bu nedenle Guid değerinin de aynı olduğu düşünülmektedir. Bu sebepten sunucu tarafından istemciye gönderilen cevabın içeriği aşağıdaki gibi olacaktır.

```text
HTTP/1.1 304 Not Modified
Server: ASP.NET Development Server/10.0.0.0
Date: Fri, 26 Feb 2010 09:43:05 GMT
X-AspNet-Version: 4.0.30128
ETag: "2c1cd636-3058-4985-8f10-3d3cb8c9e5fa"
Cache-Control: private
Connection: Close
```

Her hangibir içerik (Content) yoktur. Hatta 0 byte uzunluğunda bir Content mevcuttur. Ama daha önemlisi yine ETag elementi vardır ve Guid değerini içermektedir. Ayrıca HTTP 304 Not Modified bilgisinin döndürüldüğü dikkatlerden kaçmamalıdır.

Peki sunucu tarafındaki Product içeriğinde ve dolayısıyla Version değerinde bir değişme olursa? Bir veritabanı örneği geliştirmediğimiz için bu durumu şu şekilde simüle edebiliriz; 1 numaralı ProductId değerine sahip ürünün adını Visual Studio 2010 ortamında değiştirip örneği tekrardan build ederek. Yeniden build işlemi sonucu static olarak tanımlanan List koleksiyon içeriğinin üretimi yinelenecektir. Bu da yeni Guid değerlerinin üretimi anlamına gelmektedir. Bu durumda servis operasyonuna yeniden talepte bulunursak aşağıdaki cevabı aldığımız görürüz.

```text
HTTP/1.1 200 OK
Server: ASP.NET Development Server/10.0.0.0
Date: Fri, 26 Feb 2010 11:52:38 GMT
X-AspNet-Version: 4.0.30128
Content-Length: 237
ETag: "2fadf1be-d5c3-4fe0-a9c6-ecf20437ffe4"
Cache-Control: private
Content-Type: application/xml; charset=utf-8
Connection: Close

<Product xmlns="http://schemas.datacontract.org/2004/07/Lesson2" xmlns:i="http://www.w3.org/2001/XMLSchema-instance"><Name>Zedojen 250 Gb HDD</Name><ProductId>1</ProductId><Version>2fadf1be-d5c3-4fe0-a9c6-ecf20437ffe4</Version></Product>
```

Dikkat edileceği üzere Guid değeri bir öncekinden farklıdır ve istemciye HTTP 200 Ok koduyla Product içeriği tekrardan gönderilmiştir. Tabi bunun sonrasında 1 numaralı ürünü yeninden talep edersek yine HTTP 304 Not Modified durumu ile karşılaşırız.

Buraya kadar her şey iyi gitti. Ancak testlerimizi farkettiğiniz üzere Internet Explorer gibi tarayıcı uygulamalar üzerinden gerçekleştirdik. Oysaki istemci uygulamayı biz yazıyorsa ETag kullanımı için de yapmamız gereken ekstra işlemler söz konusudur. Bu amaçla az önce geliştirdiğimiz servis uygulamasını test edeceğimiz basit bir Console Application geliştirdiğimizi düşünelim. Kod içeriğini aşağıdaki gibi yazmamız ETag desteği için yeterli olacaktır.

```csharp
using System;
using Microsoft.Http;
using Microsoft.Http.Headers;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Uri serviceAddress = new Uri(@"http://ipv4.fiddler:1000/Adventures/");
            using (HttpClient client = new HttpClient(serviceAddress))
            {
                EntityTag eTag=null;
                Process(client, "1",ref eTag);
                Process(client, "1",ref eTag);
            }
        }

        static void Process(HttpClient client, string ProductId,ref EntityTag ETag)
        {
            Console.WriteLine("***{0} için Talep***\n",ProductId);
            // Talebin hazırlanması ve gönderilmesi
            using (HttpRequestMessage request = new HttpRequestMessage("GET", "Products/"+ProductId))
            {
                // Metoda referans olarak gelen EntityTag tipinden olan ETag değerine bakılır. Eğer null değil ise ki ilk talep sonrası sunucu tarafından ürünün Version değeri ile doldurulacaktır; bu durumda Header kısmına If-None-Match değerinin eklenmesi sağlanır.
                if (ETag != null)
                    request.Headers.IfNoneMatch.Add(ETag);

                // If-None-Match değeri içeren talep gönderilir
                using (HttpResponseMessage response = client.Send(request))
                {
                    // ETag değeri gelen cevaptan alınır ve ref tipinden olan metod parametresine aktarılır. Böylece Process metoduna yapılacak olan sonraki çağrılarda aynı ETag değerinin taşınması kolaylaşmaktadır.
                    ETag = response.Headers.ETag;
                    // Sonuçlar ekran yazdırılır.
                    Console.WriteLine("StatusCode : {0}\n", response.StatusCode);
                    Console.WriteLine("Content : {0}\n",response.Content.ReadAsString());
                    Console.WriteLine("ETag Değeri : {0}\n", ETag.Tag);
                }
            }
            Console.WriteLine("*******");
        }
    }
}
```

Uygulamamızı çalıştırdığımızd aşağıdaki sonuçlar ile karşılaşırız.

![blg159_Runtime2.gif](/assets/images/2010/blg159_Runtime2.gif)

Görüldüğü gibi, ikinci talep sonrasında istemci tarafına HTTP 304 Not Modified bilgisi ve 0 Byte uzunluğunda içerik gönderilmiştir. Böylece WCF WebHttp Service'leri ile ilişkili bir yazımızın daha sonuna geldik. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Lesson9_RC.rar (175,04 kb)](/assets/files/2010/Lesson9_RC.rar) [Örnek Visual Studio 2010 Ultimate RC sürümü üzerinde geliştirilmiş ve test edilmiştir]
