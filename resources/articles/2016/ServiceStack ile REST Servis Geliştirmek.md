---
title: "ServiceStack ile REST Servis Geliştirmek"
pubDate: 2016-02-13 07:00:00
categories:
  - REST
tags:
  - REST
  - C#
  - .NET
  - ASP.NET
  - LINQ
  - Oracle
  - NoSQL
  - WCF
  - XML
  - SOAP
  - JSON
  - Web API
  - Web Service
  - HTTP
  - IIS
  - Generics
---

# ServiceStack ile REST Servis Geliştirmek
![image.axd](images/image.axd)

Merhaba Arkadaşlar,

Geçenlerde üzerinde çalışmakta olduğum proje için kullandığım geliştirme ortamımı bir güzelce patlatıverdim. Oracle'ın yeni sürümüne ait Data Provider'ın,.Net Framework 4.5.2 sürümüne yükseltmiş bir uygulamada neden çalışmadığını araştırıyordum. Sorun sunucu kaynaklı idi. Vakayı geliştirme ortamında da oluşturmaya çalıştım. Nitekim orada her şey yolunda görünüyordu. Lafı fazla uzatmayayım sonunda Production ortamını da kurcalamaya karar verdim...Eee...Şey...Orayı da patlattım:) Neyseki prod sunucusu sadece kurulumu yapılmış ve henüz yayına alınmamış bir makine idi. Zaten uygulamanın orada başarılı şekilde çalışması halinde kullanıma açılacak bir sunucuydu. Durumu ben, sistem ekibindeki bir kaç kişi ve ekip arkadaşım biliyordu. Ah şu andan itibaren de siz. Dolayısıyla hafif stresli günler geçirdiğimi ifade edebilirim. Epeyce sinirlendim ve sakinleşmek için her zaman yaptığım şeyi yaptım. Makale yazdım.

Bu kez gözüme faydalı olduğunu düşündüğüm bir Nuget paketini kestirdim. [DTO servis tasarım desenini](http://www.servicedesignpatterns.com/requestandresponsemanagement/datatransferobject) baz alan ServiceStack.

> Arada sırada NuGet paketlerini indirip kullanmaya çalışmakta fayda var. Nitekim bu paketleri kullanırken uyguladıkları kalıpları veya mimari yaklaşımları da inceleme fırsatı buluyoruz.

ServiceStack, WCF ve Asp.Net Web API'ye alternatif olarak kullanılabilecek bir Web Service Framework'ü. Standart SOAP tabanlı servisler dışında REST tabanlı servislerin geliştirilmesine de izin veren bir çatı. Host uygulamalar IIS, Windows Service ya da Self-Hosted teknikleri ile yayınlanabiliyor. Hatta mono üzerinden de servisleri host etmek mümkün ki bu da farklı platformlar üzerinden servis yayınlanmasına imkan sağlıyor. Çok fazla konfigurasyon gerektirmeden kolayca geliştirme yapılması mümkün.

Aslında Data Transfer Object (DTOs) olarak adlandırılan servis tasarım kalıbını benimsemekte. Bu modele göre servis uç noktalarına gelecek olan talepler (Requests) ve cevaplar (Response) birer nesne olarak tanımlanıyor. Bir servisin tanımlanması için zaten en az bir talep için DTO sınıfının yazılması gerekmekte. Servisler birer Endpoint olarak sunuluyor. Route tanımlamaları ise (yani hangi HTTP talebinin ele alınacağının çalışma ortamına bildirimesi ve uygun yönlendirmenin yapılması) Asp.Net Web API'den farklı olarak service controller yerine DTO seviyesinde uygulanıyor. Tüm bunlar biraz kafanızı karıştırmış olabilir. Gelin basit bir örnek üzerinden konuyu anlamaya çalışalım.

## Senaryo

Console uygulaması üzerinden host edilen bir REST servis geliştireceğiz. Bu servisin şimdilik HTTP Get ve Post için birer operasyonu olacak. Ancak kendi çalışmanız sırasında Put ve Delete gibi operasyonları da senaryoya dahil edebilirsiniz. Servisimiz Product tipinden bir nesne koleksiyonunu kullanacak. Get operasyonları ile adı belirli bir metin ile başlayan ürünlerin listesini veya tümünü döndüreceğiz. Post operasyonu ile de yeni bir ürünü listeye ekleyeceğiz. Host uygulamasını, localhost üzerinde bizim belirleyeceğimiz uygun bir Port'tan ayağa kaldıracağız. Kısacası Self-Hosted bir program söz konusu olacak. İstemci tarafında ise basit bir tarayıcıdan veya SoapUI aracından yararlanabiliriz. Temel hedefimiz Http Get ve Post taleplerini yaparak sonuçları görebilmek.

## Servis Tarafı

İlk olarak bir Console uygulaması açarak işe başlayalım. Sonrasında ServiceStack paketini uygulamamıza dahil etmemiz gerekiyor.

![image.axd](images/image.axd)

Paketi dahil ettikten sonra aşağıdaki sınıf diagramını baz alarak gerekli geliştirmeleri yapmaya başlayabiliriz.

![image.axd](images/image.axd)

ve kodlar

```csharp
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HowToServiceStack
{
    class Program
    {
        static void Main(string[] args)
        {
            var hostAddress = "http://*:4568/";
            var appHost = new AppHost().Init().Start(hostAddress);

            Console.WriteLine("Host is running at {0}",hostAddress);
            Console.ReadLine();               
        }
    }

    #region POCOs

    public class Product
    {
        public int PartNumber { get; set; }
        public string Name { get; set; }
        public decimal ListPrice { get; set; }
    }

    #endregion

    #region DTOs

    [Route("/Products","GET")]
    [Route("/Products/{NameLike}","GET")]
    public class ProductSelectRequest
        :IReturn<ProductSelectResponse>
    {
        public string NameLike { get; set; }
    }
    public class ProductSelectResponse
    {
        public List<Product> Products { get; set; }
    }

    [Route("/products","POST")]
    public class CreateProduct
        :IReturn<ProductSelectResponse>
    {
        public int PartNumber { get; set; }
        public string Name { get; set; }
        public decimal ListPrice { get; set; }
    }

    #endregion

    #region Service Implementation

    public class ProductService
        : Service
    {
        public static List<Product> products = new List<Product>
        {
            new Product{ PartNumber=1001, Name="AC-210",ListPrice=1000},
            new Product{ PartNumber=1002, Name="AC-215",ListPrice=960},
            new Product{ PartNumber=1003, Name="KC-210",ListPrice=850.50M},
            new Product{ PartNumber=1004, Name="BC-210",ListPrice=750},
            new Product{ PartNumber=1005, Name="BD-123",ListPrice=450},
            new Product{ PartNumber=1006, Name="BD-400",ListPrice=900},
            new Product{ PartNumber=1007, Name="ZD-405",ListPrice=250},
            new Product{ PartNumber=1008, Name="CD-505",ListPrice=385}
        };

        public object Get(ProductSelectRequest request)
        {
            if(request.NameLike!=default(string))
            {
                return new ProductSelectResponse
                {
                    Products = (from p in products
                               where p.Name.StartsWith(request.NameLike)
                               select p).ToList()
                };
            }
            else
            {
                return new ProductSelectResponse
                {
                    Products = products
                };
            }
            
        }

        public object Post(CreateProduct request)
        {
            Product newProduct = new Product
            {
                PartNumber = request.PartNumber,
                Name = request.Name,
                ListPrice = request.ListPrice
            };
            products.Add(newProduct);
            return newProduct;
        }
    }    

    #endregion

    public class AppHost
        : AppSelfHostBase
    {
        public AppHost()
            :base("HttpListener Self-Host",typeof(ProductService).Assembly)
        {
        }
        public override void Configure(Funq.Container container)
        {
        }
    }
}
```

## Kodda Neler Yaptık?

İlk olarak AppHost sınıfına bir bakalım. AppSelfHostBase'den türetilmiş olan bu sınıf temel olarak Host uygulama görevini üstleniyor. Ayrıca ProductService isimli servisin hizmete alınması işlemlerini gerçekleştiriyor. Uygulama http://localhost:4568 adresi üzerinden yayın yapacak. Buna göre Main metodu içerisinde Init ().Start (hostAddress) formasyonunu kullanıyoruz (Burada Fluent stilde bir tasarım olduğu gözden kaçmamalıdır. Fleunt kod tasarımı için [bu yazıya](https://www.buraksenyurt.com/post/Fluent-Interface-Prensibi-ile-Daha-Okunabilir-Kod-Gelistirmek.aspx) bakabilirsiniz)

Yukarıda da bahsettiğimiz üzere ServiceStack DTO servis tasarım desenini kullanmaktadır. /products/ ve /products/{NameLike} şeklinde yapılacak HTTP Get talepleri için ProductSelectRequest isimli bir DTO tipi tanımlanmıştır. ProductSelectRequest sınıfı IReturn arayüzünü (Interface) uygulamaktadır. Buna göre çalışma zamanı /Products veya /Products/{NameLike} talepleri karşısında nasıl bir tip döndüreceğini de öğrenmektedir.

ProductSelectRequest sadece NameLike isimli bir özellik içermektedir. Hatta bu özellik Route niteliğinde de birebir kullanılmaktadır. İstemci tarafından gelecek talebin çeşitliliğine göre DTO sınıflarına n sayıda özellik de katılabilir. Get taleplerine olan cevabımız temel olarak bir ürün listesidir. Bu nedenle ProductSalesResponse sınıfı içerisinde List tipinden bir özellik bulunmaktadır.

Bir diğer DTO tipi ise CreateProduct sınıfıdır. Bu sınıf yeni bir ürünü listeye eklemek için gerekli özellikleri içermektedir. Route niteliğinde bir önceki DTO nesnesinden farklı olarak POST bildirimi yapılmıştır. Her iki DTO üzerinde de Route nitelikleri (Attributes) kullanılmıştır. İlk parametreler resource bilgisini, ikinci parametreler ise HTTP metodunu ifade etmektedir.

Uygulamanın can alıcı noktası ise tahmin edileceği üzere ProductService isimli hizmettir. Servis, Get ve Post isimli iki fonksiyon içerir. Her iki metodda dikkat edileceği üzere DTO nesnelerini parametre olarak almaktadır. Get metodu ile ürün koleksiyonu üzerinde Name alanının değerine göre arama yaptırılır. Eğer NameLike değeri verilmemişse tüm ürün listesi döndürülür. Post metodu ise basitçe products değişkenine yeni bir Product nesne örneği ilave etmektedir.

ProductService sınıfnın bulunduğu Assembly dikkat edileceği üzere AppHost sınıfının yapıcı metodunda kullanılmakta ve base sınıfın yapıcı metoduna yönlendirilmektedir. Çalışma zamanının gelen Assembly bildirimine göre hangi tipleri servis olarak devreye alacağı Service sınıfından yapılan türetme (Inheritance) sayesinde bilinmektedir. Bir başka deyişle Application Host çalışma zamanının Service türevli tipleri birer hizmet olarak yayına alacağını ifade edebiliriz.

Aslında yaptıklarımızı aşağıdaki şekil ile kısaca özetleyebiliriz.

![image.axd](images/image.axd)

## Testler

Öncelikli olarak servisi ayağa kaldırmamız gerekiyor. Console uygulamasını başlattığımızda aşağıdakine benzer bir ekran görüntüsü ile karşılaşmalıyız.

![image.axd](images/image.axd)

Sonrasında istemci testlerine başlayabiliriz. Ben, Get ve Post talepleri için SoapUI ve Google Chrome tarayıcısından yararlandım. SoapUI üzerinden elde ettiğim sonuçlar ise şöyle.

http://localhost:4568/products/ HTTP Get için

![image.axd](images/image.axd)

ServiceStack istemci tarafına özel bir HTML içeriği basar. İstenirse çıktılar json,xml,csv ve hatta jsv formatında alınabilir. Tek yapılması gereken URL sonuna?format=json benzeri bir ifade eklemektir. Örneğin ürün adı B harfi ile başlayanların listesini JSON formatında elde etmek istersek http://localhost:4568/Products/B?format=json şeklinde bir talep gönderilmesi yeterlidir.

![image.axd](images/image.axd)

ve http://localhost:4568/products/ HTTP Post içinde aşağıdaki şekilde talepte bulunabiliriz. Eğer Post paketi başarılı bir şekilde gönderilirse servisin yeni üretilen Product nesnesine ait değerleri geri gönderdiğini de görebilmeliyiz. Aynen aşağıdaki ekran görüntüsünde olduğu gibi.

![image.axd](images/image.axd)

Bu işlem sonrasında ürün listesi yeniden talep edilirse 1000 numaralı parçanın da koleksiyona eklendiği görülecektir. Tabii ki bir gerçek hayat senaryosunda veriyi tutmak adına RDBMS veya NoSQL tabanlı bir kaynaktan yararlanmakta yarar vardır. Siz antrenmanlarınızı buna göre yapabilirsiniz.

Bu makalemizde ServiceStack paketini kullanarak Self-Hosted REST servislerini nasıl geliştirebileceğimizi basit bir örnek üzerinden incelemeye çalıştık. Anlamamız gereken önemli noktalardan birisi de DTO servis tasarım kalıbının nasıl kullanıldığıdır. Ayrıca Service türevli tiplerin çalışma zamanında servis şeklinde nasıl sunulabildiğinin, Route niteliklerinin görevlerini nasıl gerçekleştirdiğinin ve gelen talepler ile ilgili DTO nesnelerinin nasıl ele alındığının düşünülmesinde yarar vardır. Siz bu tip bir çatıyı tasarlamaya çalışsanız nasıl bir yol izler ve kodlama yapardınız? Bunu bir düşünün.

Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
