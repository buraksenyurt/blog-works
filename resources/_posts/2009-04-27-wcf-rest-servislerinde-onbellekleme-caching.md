---
layout: post
title: "WCF Rest Servislerinde Önbellekleme(Caching)"
date: 2009-04-27 12:28:00 +0300
categories:
  - wcf
tags:
  - wcf
  - csharp
  - dotnet
  - aspnet
  - rest
  - http
  - iis
  - performance
  - caching
  - generics
  - visual-studio
  - dependency-management
---
REST (REpresentational State Transfer) modelini uygulayan WCF servislerinin geliştirilmesinde, WCF Rest Starter Kit ile birlikte gelen kolaylıklardan biriside, önbellekleme (Caching) işlemlerinin dekleratif (Declarative) olarak yapılabilmesidir. Burada dekleratiflikten kastımız, önbellekleme bildirimlerinin çalışma zamanına nitelik (Attribute) yoluyla bildirilmesidir. Web programlama modeline göre geliştirilen servislerin çeşidi ne olursa olsun, performans kriterleri söz konusu olduğunda önemli olan noktalardan biriside verilerin istemciye gönderilmeden önce gerekiyorsa belirli süreler boyunca veya bazı koşullar sağlanıncaya kadar sunucu ön belleğinde (hatta bazen istemci tarafında tarayıcı uygulama için ayrılan özel bölgelerde) tutulmasıdır.

Özellikle Asp.Net web uygulamalarını göz önüne alırsak, ön bellekleme ile ilişkili olarak kullanılan modüllerin pek çok farklı yapıyı desteklediğini görürüz. Söz gelimi, absolute caching (Kesin bir süre kadar önbellekte tutulması), sliding expiration caching (belirli süre içerisinde talep geldikçe önbellekte tutulma süresinin ileriye ötelenmesi), file dependency caching (önbellekteki verinin yenilenme koşulunun dosyadaki değişimlere bağlanması),sql dependency caching (ön bellekteki verinin yenilenme koşulunun bir tablodaki değişimlere bağlanması) vb... Hal böyle olunca, genellikle Asp.Net destekli sunucularda host edilen WCF servislerindende bu hazır Caching modüllerinden yararlanılması kaçınılmazdır. Startet Kit ise, Microsoft.ServiceModel.Web assembly'ı içerisinde sunduğu WebCache niteliği ile bu özelliğin REST bazlı WCF servislerinede uygulanabilmesini, olanaklı kılmaktadır.

![blg8_1.gif](/assets/images/2009/blg8_1.gif)

Bu yazımızda söz konusu ön bellekleme sisteminin Atom formatında içerik yayınlaması (Atom Feed Syndication) yapan bir WCF Rest servisinde nasıl ele alınabileceğini incelemeye çalışacağız. İşe ilk olarak Atom Feed WCF Service şablonunda bir proje açarak başlayabiliriz.

![blg8_2.gif](/assets/images/2009/blg8_2.gif)

Atom Feed WCF Service proje tipi, tahmin edeceğiniz üzere WCF Rest Starter Kit ile birlikte Visual Studio 2008 ortamına yüklenen hazır şablonlardan birisidir. Şablon içeriği zaten hazır bir kod yapısına sahiptir ve gerekli TODO işaretlemeleri ile geliştiriciye neler yapması gerektiğini varsayılan ölçülerde bildirmektedir. Ancak bundan daha da önemlisi şablonun kullanım amacıdır. Bu şablon, ATOM formatında içerik yayınlaması yapan bir WCF servis operasyonunun REST bazlı olacak şekilde sunulabilmesini sağlamaktadır. Bu sebeten hazır olarak sunulan GetFeed isimli operasyonun dönüş tipi Atom10FeedFormatter nesne örneğindendir. Servis kodlarını tamamlamadan önce web.config dosyası içerisinde önbellekleme için gerekli profil özelliklerini aşağıdaki şekilde görüldüğü gibi belirleyebiliriz.

![blg8_6.gif](/assets/images/2009/blg8_6.gif)

Buradaki ayarlamalara göre iki farklı önbellekleme profili tanımlanmaktadır. Cache1 isimli profile göre, önbellekleme süresi 60 saniyedir ve sunucu taraflı bir önbellekleme yapılacağı location niteliğine atanan Server değeri ile belirtilmektedir. Diğer taraftan Cache2 isimli profilde ise sadece süre ve lokasyon bilgileri farklıdır. Konfigurasyon dosyasında önem arz eden konulardan biriside aspNetCompatibilityEnabled niteliğine atanan true değeridir. Asp.Net ortamının önbellekleme modülünden yararlanılacağı için bu değerin true olması önemlidir.

Artık bizim yapmamız gereken tek şey, servis tarafında kullanacağımız WebGet niteliğine hangi Cache profilini kullanacağını bildirmektir. Buna göre başlangıçta FeedService adıyla oluşturulmuş olan ama örneğimizde ProductFeedService ismiyle tanımlanan servisimize ait kod içeriğini aşağıdaki gibi değiştirebiliriz.

```csharp
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Syndication;
using System.ServiceModel.Web;
using Microsoft.ServiceModel.Web;

namespace Caching
{    
    [ServiceBehavior(IncludeExceptionDetailInFaults = true), AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed), ServiceContract]
    public partial class ProductFeedService
    {
        [WebHelp(Comment = "Stok bilgisine göre tüm ürün bilgileri Atom formatında getirilir.")]
        [WebGet(UriTemplate = "?size={stockSize}")]
        [WebCache(CacheProfileName="Cache1")] // web.config dosyasında, Cache1 isimli önbellekleme profilininin takip eden operasyon için kullanılacağı belirtilir.
        [OperationContract]
        public Atom10FeedFormatter GetFeed(short stockSize)
        {            
            SyndicationFeed feed;

            // varsayılan kontroller

            if (stockSize < 0)  // stok değeri parametresi 0' ın altında ise
                throw new WebProtocolException(HttpStatusCode.BadRequest, "Stok miktarı eksi değer olamaz.", null);
            // stok değeri parametresi girilmemişse(ki servis querystring kullanılmadan talep edilirse bu çalışır)
            if (stockSize == 0) 
                stockSize = 10;

            // Kritere uyan her bir Product için birer SyndicationItem örneği oluşturulup, SyndicationItem tipinden generic List koleksiyonuna eklenir.
            List<SyndicationItem> items = new List<SyndicationItem>();
            foreach(Product product in GetProducts(stockSize))
            {
                items.Add(new SyndicationItem()
                {
                    // Syndication içeriğinde olması gereken standart bazı özelliklerin değerleri set edilir.

                    Id = String.Format(CultureInfo.InvariantCulture, "http://northwind.com/ProductID{0}", product.ProductId),
                    Title = new TextSyndicationContent(String.Format("'{0}' ürünü", product.Name)),                    
                    LastUpdatedTime = DateTime.Now,                    
                    Authors = {new SyndicationPerson() {Name = ""} // Yazar bilgisi kullanılmadığı için boş bırakıldı
                    },
                    
                    Content = new TextSyndicationContent(String.Format("{0}, {1}, {2}, {3}",product.ProductId.ToString(),product.Name,product.UnitPrice.ToString("C2"),product.UnitsInStock.ToString())),
                     PublishDate=DateTime.Now,
                      Summary=new TextSyndicationContent(String.Format("{0} isimli üründen stokta {1} adet bulunmaktadır",product.Name,product.UnitsInStock.ToString())),                      
                });
            }
            
            // Feed hazrılanır ve Items özelliğine, List<SyndicationItem> tipinden olan ve yukarıda hazırlanan itemsi isimli koleksiyon atanır.
            feed = new SyndicationFeed()
            {
                Id = "http://northwind.com/ProductsWithStock",
                Title = new TextSyndicationContent("Stok miktarı bazlı ürün listesi"),
                Items = items
            };
            feed.AddSelfLink(WebOperationContext.Current.IncomingRequest.GetRequestUri());        
            // Operasyonun çıktı formatının Atom olacağı ContentType özelliğine atanan değer ile belirlenir
            WebOperationContext.Current.OutgoingResponse.ContentType = ContentTypes.Atom;            
            // syndication içeriği operasyondan geriye döndürülür.
            return feed.GetAtom10Formatter();
        }

        // Yardımcı metod. UnitsInStock değeri stockSize ile gelen değerin altında olan ürünleri Product tipinden bir koleksiyon olarak geriye döndürmektedir.
        private List<Product> GetProducts(short stockSize)
        {
            List<Product> products = new List<Product>();
            using (SqlConnection conn = new SqlConnection("data source=.;database=Northwind;integrated security=SSPI"))
            {
                SqlCommand cmd = new SqlCommand("Select ProductID,ProductName,UnitPrice,UnitsInStock From Products Where UnitsInStock<@UnitsInStock", conn);
                cmd.Parameters.AddWithValue("@UnitsInStock", stockSize);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                while (reader.Read())
                {
                    products.Add(
                        new Product
                        {
                             ProductId=Convert.ToInt32(reader["ProductID"]),
                              Name=reader["ProductName"].ToString(),
                               UnitPrice=Convert.ToDecimal(reader["UnitPrice"]),
                                UnitsInStock=Convert.ToInt16(reader["UnitsInStock"])
                        }
                        );
                }
                reader.Close();
            }
            return products;
        }
    } 
}
```

GetFeed metodu Products tablosundan, stok miktarı parametre olarak gelen stockSize değerinden düşük olan ürün bilgilerini Atom formatından bir içerik olarak istemciye göndermektedir. Burada verinin çekilmesi sırasında yardımcı bir metod olarak GetProducts fonksiyonu kullanılmaktadır. Elbetteki yazımızın konusu itibariyle önem arz eden tek satır WebGet niteliğinin kullanıldığı yerdir. Niteliğin CacheProfileName özelliğine atanan değer ile, GetFeed metodunun üreteceği çıktının hangi kriterlere göre nasıl ön bellekleneceği belirtilmektedir. Servis bu haliyle talep edildiğinde ve örneğin stok miktarı 15 birimin altında olan ürün listesi istendiğinde, aşağıdaki ekran görüntüsüne benzer bir çıktı ile karşılaşılabilir.

![blg8_5.gif](/assets/images/2009/blg8_5.gif)

Tabi bu görüntü yazımız için yeterli değildir. Nitekim amacımız ön bellekleme sistemini test edebilmek. Burada izlenebilecek bir kaç yol var. En kolayı geliştiricinin GetFeed metodu başına breakpoint koyarak debug modunda ilerlemesidir. Bu durumda, servise gelen ilk talep ile birlikte GetFeed metodunun üreteceği içerik 60 saniyeliğine (duration=60) sunucunun ön belleğinde tutulamaya başlancaktır. Bu sırada tarayıcı üzerinden aynı sayfa tekrardan talep edilirse metodun içerisinde düşülmediği ve az önce üretilen çıktının geldiği gözlemlenecektir. Tabi burada çok dikkat edilmesi gereken bir nokta vardır.

Servis bu haliyle talep edildiğinde, stockSize değeri 0 olduğundan if koşuluna göre 10 birimin altında olanlar getirilmektedir. Şayet bundan sonra tarayıcıdan http://localhost:1000/Service.svc/?size=100 gibi bir talep girilirse yine stok miktarı 10 birimin altında olanlar getirilecektir. Neden? Tabiki ilk talep önbellekte tutulduğu için

![Wink](/assets/images/2009/smiley-wink.gif)

Elbette, 60 saniyelik ön bellekleme süresi beklendikten sonra bu talep gönderilirse, stok miktarı 100' ün altında olan ürünlerin elde edilebildiği ve doğal olarak debug moddayken,GetFeed metodu içerisine düşülebildiği gözlemlenecektir. Bu tam olarak istenen bir şey olmayabilidir aslında. Belkide size parametresine göre ayrı ayrı ön bellekleme yapılabilmesi daha doğru olabilir ki buda son derece basittir. Tek yapılması gereken ön bellekleme profilini aşağıdaki gibi güncelleştirmektir.

![blg8_7.gif](/assets/images/2009/blg8_7.gif)

Görüldüğü gibi tek yaptığımız varyByParam niteliğine size değerini vermektir. Önbellekleme ile ilişkili detayları web.config dosyası içerisinde taşımanın en büyük avantajlarından biriside, koda girmeden değiştirilebilme olanağı sağlamasıdır. Örneğin ürünün IIS altına atılmasından sonra, size parametresine göre önbellekleme yapılacağına karar verildiyse eğer, kodu tekrardan açmadan web.config dosyasına müdahale edilerek bu güncelleme gerçekleştirilebilir. Diğer yandan istenirse web.config dosyası kullanılmadan, WebGet niteliğinin özellikleri içerisindede önbellekleme bilgileri tanımlanabilir.

```csharp
[WebHelp(Comment = "Stok bilgisine göre tüm ürün bilgileri Atom formatında getirilir.")]
[WebGet(UriTemplate = "?size={stockSize}")]
[WebCache(Duration=60, Location=System.Web.UI.OutputCacheLocation.Any, VaryByParam="size")]
[OperationContract]
public Atom10FeedFormatter GetFeed(short stockSize)
{
```

şeklinde...

Evettt..Böylece geldik bir blog yazımızın daha sonuna. Bu kısa yazımızda WCF REST Starter Kit'i kullanarak bir Atom Feed WCF Service'in nasıl geliştirilebileceğini gördük. Ama dahada önemlisi, WebGet niteliği yardımıyla ön bellekleme işlemlerinin nasıl yapılabileceğine bakmaya çalıştık. Görüşmek dileğiyle...

Örnek Dosya; [Caching.rar (107,66 kb)](/assets/files/2009/Caching.rar)