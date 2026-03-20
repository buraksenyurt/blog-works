---
layout: post
title: "Web Bazlı Programlama Modeli"
date: 2008-02-14 12:00:00 +0300
categories:
  - wcf
tags:
  - wcf
  - csharp
  - dotnet
  - linq
  - xml
  - soap
  - rest
  - json
  - web-service
  - xml-web-services
  - http
  - iis
  - javascript
  - serialization
  - delegates
  - generics
  - visual-studio
  - dataset
---
Web programlama modelinin en büyük avantajlarından biriside istemci (Client) tarafındaki uygulamaları düşünmeye gerek kalmadan istemci-sunucu (Client/Server) mimarisine uygun sistemler geliştirilebilmesidir. Basit olarak HTTP protokolünün farklı metodlarına göre işleyen bu sistemde, istemcilerin farklı tipte olabilecek tarayıcı programlar (Browsers) üzerinden talepte bulunmaları söz konusudur.

Özellikle servis yönelimi mimari (Serivce Oriented Architecture) yaklaşımlarına bakıldığında örneğin Xml Web Servislerinde (Xml Web Services) HTTP protokolünün basit GET metoduna göre taleplerde (Requests) bulunulabilmektedir. Bu istemci tarafına bir proxy nesnesi koymadan servis fonksiyonelliklerini HTTP protokolünün basit bir metoduna göre çağırabilme anlamınada gelmektedir. (Bununla birlikte Web servislerinde SOAP (Simple Object Access Protocol) protokolüne uygun olacak şekilde proxy kullanmadan talepte bulunulup cevap alınabileceğide bilinmektedir.)

Hal böyle olunca WCF (Windows Communication Foundation) gibi gelişmiş bir dağıtık mimari (Distributed Architecture) modelinde Web bazlı servis desteği olmaması düşünülemez. Ne varki WCF, özellikle.Net Framework 3.5 ile gelen yeni tipler sayesinde Web bazlı programlama modeli (Web-Based Programming Model) yeteneklerine kavuşmuştur. Bu model temel olarak SOAP bazlı olmayan EndPoint noktalarının tasarlanabilmesini sağlamaktadır. Bu sayede bir WCF servisi istemci tarafını programlamaya gerek kalmadan, basit tarayıcı arayüzleri sayesinde hizmet verebilmektedir.(Tabi burada POST benzeri metodlarda istisnai bazı durumlar olabileceğide göz ardı edilmemelidir). Üstelik web programlama modelinin doğası gereği istemciler servis üzerindeki fonksiyonellikleri kullanırken queryString tarzında parametrik taleplerde (Requests) de bulunabilirler. Web programlama modelinde sağlanması gereken önemli bazı hususlar vardır. Buna göre;

- Modelin URI (Uniform Resource Identifier) desteği olmalıdır. Bu destek söz konusu servis operasyonlarının istemci tarafına parametrik olabilecek şablonlara (template) uyan adresler ile sunulabilmesi için gereklidir.
- HTTP protokolünün farklı metodları için destek olmalıdır. Günümüzde en çok kullanılanları GET ve POST metodlarıdır. Sadece istemci tarafına veri alma gibi işlemler söz konusu olduğunda GET metodu, tam tersine servis tarafına parametreler gönderip düzenleme, veri aktarma yada operasyon çağırma gibi işlemler söz konusu olduğunda POST,PUT gibi metodlar kullanılır.
- Farklı tipte veri formatlarına destek olmalıdır (Multiple Data Format Support). Bu destek sayesinde XML (eXtensible Markup Lanugage), JSON (JavaScript Object Notation), binary içerikli stream (video, resim, ses dosyası gibi), düz metin (Plain Text) bazlı veriler kullanılabilmektedir.

> WCF Web programlama modeli, servislerin REST (REpresentational State Transefer) tipinde geliştirilebilmesini sağlamaktadır. REST, www (World Wide Web) gibi sistemlerin prensiplerini referans etmektedir. Bu prensipler basit olarak bir network mimarisinin kaynakları nasıl adresleyeceği ve ne şekilde tanımlayacağını belirtmektedir. [Detaylı bilgi için Wikipedia](http://en.wikipedia.org/wiki/REST)

Tahmin edileceği üzere WCF mimarisi.Net Framework 3.5 ile gelen tipler sayesinde bu üç temel özelliğede destek verecek şekilde genişletilmiştir. WCF mimarisindeki bu yeni genişlemede rol alan başlıca tipler (Types) aşağıdaki şekilde görüldüğü gibi ele alınabilirler.

![mk242_1.gif](/assets/images/2008/mk242_1.gif)

HTTP üzerinden GET, POST, PUT gibi metodlar ile gelen çağrıları yönetimli kod (Managed Code) tarafında ele almak için WebGetAttribute ve WebInvokeAttribute nitelik (attribute) sınıflarından yararlanılmaktadır. Bu nitelikler temel olarak, operasyonların URI bilgilerine nasıl bağlanacağını ve hangi HTTP metodları ile eşleştirileceğini belirlemekte kullanılırlar. WebGetAttribute niteliği GET operasyonları için ele alınırken, WebInvokeAttribute POST, DELETE, PUT gibi operasyonların kullanımını sağlamaktadır.

HTTP üzerinden POST, GET, PUT, DELETE metodları kullanılır. Bu metodların işleyiş şekli çoğu zaman CRUD tablosunda aşağıdaki gibi ifade edilir.

CRUD

Create, Update, Delete

Read

Delete

Create, Update

WebGetAttribute sınının dört önemli özelliği (property) vardır. WebMessageBodyStyle enum sabiti tipinden olan BodyStyle özelliği yardımıyla parametre ve dönüş değerlerinin XML elementleri içeriğine sarmalanıp (wrap) sarmalanmayacağına karar verilir. WebMessageBodyStyle enum sabitinin içerdiği değerler Bare, WrappedRequest, WrappedResponse ve Wrapped'dır. Bare değerine göre talep (request) ve cevap (response) mesajlarındaki veriler sarmalanmazlar. Wrapped değerine göre her iki haldede sarmalanırlar. WrappedRequest değerine göre sadece taleplerin sarmalanması, WrappedResponse değerine göre ise sadece cevapların sarmalanması söz konusudur. WebGetAttribute sınıfının diğer özellikleri RequestFormat, ResponseFormat ve UriTemplate üyeleridir.

RequestFormat ve ResponseFormat özellikleri WebMessageFormat enum sabiti tipinden değerler almaktadır. WebMessageFormat enum sabitinin değerleri XML veya JSON (JavaScript Object Notation) olabilir. Bir başka deyişle cevapların ve taleplerin hangi formatta oluşturulacağına karar verilir. WebGetAttribute sınıfının belkide en önemli üyeside UriTemplate özelliğidir. Bu özelliğe atanan değer ile GET çağrımı için bir URI şablonu (template) belirlenmiş olur. WebInvokeAttribute sınıfıda WebGetAttribute sınıfı ile aynı özelliklere sahiptir. Nitekim birde ek özelliği vardır ki buda Method isimli üyedir. Method özelliği string bazlıdır ve POST, DELETE yada PUT gibi HTTP metodlarını göstermektedir. Bu özelliğin varsayılan değeri POST olarak belirlenmiştir.

Web programlama modelinde yer alan önemli tiplerden bir diğeride WebHttpBinding sınıfıdır. Bu bağlayıcı tip (Binding Type) ile servis EndPoint noktalarının, SOAP bazlı mesajlaşma yerine POX (Plain Old Xml) bazlı mesajlaşmaya izin verecek şekilde çalışması sağlanmaktadır. Buna göre Web bazlı EndPoint (Web-Based EndPoint) noktalarının POX (Plain Old XML) bazlı mesajlaşma yaptığı söylenebilir. Diğer taraftan bu bağlayıcı tip XML,JSON ve anlaşılması güç binary tipteki (çoğunlukla video, resim gibi) verilerin okunması ve yazılması amacıyla kullanılmaktadır.

> WCF Web programlama modeli SOAP (Simple Object Access Protocol) bazlı mesajlaşmayı kullanmadığından WS- * protokollerini desteklemez. Ancak WCF mimarisinin aynı servis üzerinde birden fazla EndPoint konuşlandırabilme özelliği nedeni ile SOAP destekli olan ve olmayan EndPoint noktalarının bir arada kullanılması mümkündür. Bir başka deyişle bir servisin ilgili operasyonları WS-* destekli olacak şekilde bir EndPoint üzerinden sunulurken, aynı servis üzerindeki diğer bir EndPoint Web programlama modelini ele alabilir.

WebServiceHost sınıfı SOAP bazlı olmayan web stilindeki servislerin host edilmesi için ServiceHost tipinden türetilmiştir. Eğer servisi host eden uygulamada çalıştırılan WebServiceHost nesne örneği, Web bazlı herhangibir EndPoint bulamassa otomatik olarak bir tane üretecektir. Bu üretim sırasında servis adresini (Base Address) baz alan bir EndPoint noktası oluşturulur.

Web programlama modelinde yer alan yardımcı tiplerden UriTemplate ve UriTemplateTable son derece önemli görevler üstlenmektedir. Herşeyden önce HTTP bazlı talepler servis operasyonlarına ulaştıklarında bir URI tarafından karşılanmalıdır. Bu URI bilgilerinin yönetimli kod (Managed Code) tarafında ifade edilmesinde söz konusu tipler görev almaktadır. URI şablonları (URI Templates) temelde path ve query olmak üzere iki temel parçadan oluşurlar. Söz gelimi aşağıdaki şekilde bazı örnek URI şablonları yer almaktadır.

![mk242_2.gif](/assets/images/2008/mk242_2.gif)

Burada sol taraftaki kutucukta gerçek kullanımlar yer alırken sağ taraftada karşılık gelen URI şablonları görülmektedir. Süslü parantezler içerisinde yer alan kısımlar değişken olmakla birlikte diğer kısımlar sabittir. Elbette bu URI'lerin başında birde base URI address bilgisi gelmektedir. UriTemplate sayesinde bir URI şablonunun kolay bir şekilde oluşturulması, var olan bir gerçek URL adresi ile eşleştirilmesi (Match) gibi işlemler yapılabilir. UriTemplate tipi çoğunlukla WebGetAttribute yada WebInvokeAttribute nitelikleri ile kullanılır.

Çok doğal olarak bir servisin sunabileceği birden fazla URI şablonu ve dolayısıyla UriTemplate nesnesi söz konusu olabilir. Birden fazla UriTemplate tipinin bir arada daha kolay yönetebilmek içinde UriTemplateTable isimli sınıf kullanılmaktadır. Bu iki tipin kullanımı önem arz ettiği için basit bir Console uygulması üzerinde fonksiyonelliklerini incelemekte yarar vardır. Bu amaçla Visual Studio 2008 ortamında.Net Framework 3.5 şablonunda bir Console uygulaması açtığımızı ve System.ServiceModel.Web.dll assembly'ını ilgili projeye referans ettiğimizi düşünelim. Bu assembly tahmin edileceği üzere Web programlama modeli için gerekli temel tipleri bünyesinde barındırmaktadır. Aşağıdaki kod parçası basit olarak UriTemplate tipinin kullanımını örneklemektedir.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using System.Collections.Specialized;

namespace YardimciTipler
{
    class Program
    {
        static void Main(string[] args)
        {
            #region UriTemplate Kullanımı

            // Yeni bir URI şablonu oluşturulurken süslü parantezler içerisinde yer alan verilen değişken diğerleri ise sabittir.
            UriTemplate uriTemp = new UriTemplate("satislar/{sehirAdi}/{ilceAdi}?tarih={satisTarihi}"); 

            // değişkenlerin pozisyonuna göre bağlama
            // Bu metod ile süslü parantezler içerisine gelecek veriler sıralı olarak eklenirler
            Uri uri1 = uriTemp.BindByPosition(new Uri("http://localhost"), "Istanbul", "Kadıköy", "2007");

            // değişkenlerin adlarına göre bağlama
            // Bu metodda parametre adı - değeri eşleştirmesi bilgilerini taşıyan NameValueCollection koleksiyonu kullanılır.
            NameValueCollection values = new NameValueCollection()
                    {
                        {"sehirAdi","Ankara"}
                        ,{"ilceAdi","Esenboğa"}
                        ,{"satisTarihi","2007"}
                    };
            Uri uri2 = uriTemp.BindByName(new Uri("http://www.bsenyurt.com/servisler"), values);
        
            // Eşleştirme yapmak ve doğruluğunu tespit etmek
            Uri uri3 = new Uri("http://localhost/satislar/Istanbul/Besiktas?tarih=2008");
            // Eğer Match metodu geriye null değer döndürmemişse URI içeriği şablonda belirtilene uygundur.
            UriTemplateMatch match=uriTemp.Match(new Uri("http://localhost/"), uri3);
            if (match != null)
                Console.WriteLine("Match geçerlidir");

            #endregion
        }
    }
}
```

Örnektende görüldüğü üzere bir UriTemplate nesnesi örneklendiğinde çoğunlukla bir URI şablonuda tanımlar. Sonrasında bu şablonda yer alan parametrelere değer aktarımı için BindByPosition yada BindByName gibi metodlar kullanılabilir. Diğer taraftan bir URI bilgisinin, UriTemplate ile belirtilen şablona uygunluğunu denetlemek için Match metodundan yararlanılabilir. (BindByName metodunun uygulanışı sırasında C# 3.0 object initializers özelliğinden yararlanıldığınada dikkat edelim.) Diğer taraftan aşağıdaki kod parçasında da basit olarak UriTemplateTable kullanımı örneklenmektedir.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using System.Collections.Specialized;

namespace YardimciTipler
{
    class Program
    {
        static void Main(string[] args)
        {
            #region UriTemplateTable Kullanımı

            // UriTemplateTable yardımıyla birden fazla UriTemplate' in tek saklanması ve bir arada tutulması sağlanabilir. 
            UriTemplateTable uriTable = new UriTemplateTable(new Uri("http://localhost"));

            // UriTemplate' ler tamamı KeyValuePairs özelliği ile işaret edilen koleksiyonda tutulurlar.
            uriTable.KeyValuePairs.Add(new KeyValuePair<UriTemplate, object>(new UriTemplate("satislar/{Sehir}/{Ilce}"), "IlceBazli"));
            uriTable.KeyValuePairs.Add(new KeyValuePair<UriTemplate,object>(new UriTemplate("satislar/{Sehir}/{Ilce}?tarih={SatisTarihi}"),"SatisBazli"));
            uriTable.KeyValuePairs.Add(new KeyValuePair<UriTemplate,object>(new UriTemplate("satislar/{Sehir}/{Ilce}?yetkili={Yetkili}"),"YetkiliBazlı"));
            uriTable.KeyValuePairs.Add(new KeyValuePair<UriTemplate, object>(new UriTemplate("satislar/*"), "TumSatislar"));

            foreach (KeyValuePair<UriTemplate, object> kv in uriTable.KeyValuePairs)
            {
                Console.WriteLine("{0} : \t {1}", kv.Value, kv.Key);
            }
            #endregion
        }
    }
}
```

UriTemplateTable sınıfı UriTemplate örneklerini KeyValuePairs isimli özelliğin işaret ettiği koleksiyonda saklamaktadır. Yukarıdaki kod parçasına çalışma zamanında debug modda bakıldığında uriTable isimli değişkenin içeriğinin aşağıdaki gibi olduğu görülebilir.

![mk242_3.gif](/assets/images/2008/mk242_3.gif)

Web stilinde tasarlanan servisler bir tarayıcı uygulama yardımıyla URL bazında çağırılabilirler. URL üzerinden servis tarafına gelecek olan web bazlı taleplerde kullanılan parametrelerinin veri formatlarıda önemlidir. Bunlar aynı zamanda çağırılan operasyonların geri döndürdüğü değer tipleri içinde önemlidir. Kullanılabilecek veri formatları

- Byte
- SByte
- Int16
- Int32
- Int64
- UInt16
- UInt32
- UInt64
- Single
- Double
- Char
- Decimal
- Boolean
- String
- DateTime
- TimeSpan
- Guid
- DateTimeOffset
- Enums
- TypeConverterAttribute niteliğini uygulayan

tipler olabilir.

> WCF Web programlama modelinde dikkat edilmesi gereken noktalardan biriside güvenlik (Security) dir. Bu modele göre güvenlik sadece HTTPS şeklinde yani iletişim (transport) seviyesinde sağlanabilir. Bunun nedeni Web tabanlı WCF modelinin SOAP zarflarını (Envelope) kullanmayışıdır. Normal şartlarda mesaj seviyesinde (Message Level Security) güvenlik uygulandığında güvenlik ile ilişkili bazı bilgiler SOAP zarflarında yer alan başlık (Header) kısmına yazılır. Oysaki web bazlı modelde bu tip bir alan yoktur. Dolayısıyla sadece iletişim seviyesinde güvenlik uygulanabilmektedir.

Artık Web bazlı örnek bir servis geliştirerek devam edebiliriz. İlk olarak servis sözleşmesini (Service Contract) ve uygulayıcı tipi barındıracak olan WCF servis kütüphanesini (WCF Service Library) geliştirmekte yarar vardır. Söz konusu kütüphanenin Web tabanlı model tiplerini kullanabilmesi için doğal olarak System.ServiceModel.Web.dll assembly'ını referans etmesi gerekmektedir. Söz konusu kütüphanedeki tiplerin aşağıdaki gibi tasarlandığı varsayılabilir.

![mk242_4.gif](/assets/images/2008/mk242_4.gif)

IUrunHizmetleri isimli servis sözleşmesi int, double, DataSet, Urun tipinden değerler döndüren operasyonlar tanımlamaktadır. Aynı zamanda WebInvoke niteliğinin (attribute) kullanımını örneklemek amacıyla değer dönürmeyen bir operasyon daha tanımlamaktadır. UrunCek isimli operasyon geriye Urun tipinden bir nesne örneği döndürmektedir. Urun sınıfının içeriği aşağıdaki gibidir.

```csharp
[DataContract]
public class Urun
{
    [DataMember]
    public int Id;
    [DataMember]
    public string Name;
    [DataMember]
    public double ListPrice;
}
```

Urun sınıfı basit olarak Product tablsoundaki belirli bir satırın ProductId,Name ve ListPrice alanlarının değerlerini taşımak üzere tasarlanmıştır. Çok doğal olarak Urun nesne örneği serileşme (Serialization) işlemine tabi tutulacağından DataContract ve DataMember nitelikleri (attributes) ile imzalanmıştır. Bir başka deyişle bir veri sözleşmesi (Data Contract) tanımlanmaktadır. Servis sözleşmesini (Service Contract) taşıyan IUrunHizmetleri arayüzünün (Interface) içeriği ise aşağıdaki gibidir.

```csharp
[ServiceContract]
public interface IUrunHizmetleri
{
    [OperationContract]
    [WebGet(UriTemplate="urunler/{altkategoriId}")] 
    DataSet UrunListesi(string altKategoriId);

    [OperationContract]
    [WebGet]
    Urun UrunCek(int urunId);

    [OperationContract]
    [WebInvoke(UriTemplate="guncelle?sinifi={sinifi}&altKategori={altKategoriId}")]
    void UrunGuncelle(string sinifi, int altKategoriId);

    [OperationContract]
    [WebGet(UriTemplate="toplamaIslemi?sayi1={x}&sayi2={y}")]
    int Toplam(int x, int y);

    [OperationContract]
    [WebGet(UriTemplate="toplam?kategori={altKategoriId}")]
    double ToplamFiyat(int altKategoriId);
}
```

UrunListesi isimli operasyon geriye DataSet döndürmektedir. Bu DataSet içeriği ürünlerin alt kategori değerine göre elde edilmektedir. Söz konusu operasyon HTTP GET metoduna göre talep edilebilir. Dikkat edileceği üzere WebGet niteliğinin UriTemplate özelliğinde süslü parantezler içerisinde metod parametresi ile aynı isimde olacak şekilde bir tanımlama yapılmaktadır. UrunCek operasyonu geriye Urun tipinden bir nesne örneği döndürmekle birlikte urunId değeri queryString üzerinden alınmaktadır. WebGet niteliğinde UriTemplate kullanılmaması nedeniyle talepte UrunCek?urunId=1 gibi bir adres kullanılmalıdır.

Bir başka deyişle WebGet niteliğinde UriTemplate kullanılmadığı durumlarda, MetodAdı?parametreAdı1=değeri1¶metreAdı2=değeri2 tarzı adresler ile talepte bulunulabilinir. UrunGuncelle isimli operasyon, içerisinde sinifi ve altKategori queryString parametrelerini bulunduran adres taleplerine HTTP POST metoduna göre cevap verecek şekilde tanımlanmaktadır. Bu sebepten dolayı URL satırından bir bilgi gönderilmesine gerek yoktur. Toplam operasyonu URI bilgisi içerisinde birden fazla queryString parametresini ele alıp basit bir veri tipini geriye döndürecek şekilde tasarlanmıştır. Benzer şekilde ToplamFiyat operasyonuda belirli bir alt kategerideki ürünlerin toplam liste fiyatı değerini bulacak bir fonksiyonelliği tanımlamaktadır. Söz konusu servis sözleşmesini (Service Contract) uygulayan sınıfın içeriği ise aşağıdaki gibidir.

```csharp
public class UrunHizmetleri 
     : IUrunHizmetleri
{
    string conStr = "data source=.;database=AdventureWorks;integrated security=SSPI";
    #region IUrunHizmetleri Members

    public DataSet UrunListesi(string altKategoriId)
    {
        DataSet set = null;
        using (SqlConnection conn = new SqlConnection(conStr))
        {
            SqlCommand cmd = new SqlCommand("Select ProductId,Name,ListPrice,ProductSubCategoryId,Class,SellStartDate From Production.Product Where ProductSubCategoryId=@SubCatId",     conn);
            cmd.Parameters.AddWithValue("@SubCatId", altKategoriId);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            set = new DataSet();
            adapter.Fill(set);
        }
        return set;
    }

    public Urun UrunCek(int urunId)
    {
        Urun u = null;
        using (SqlConnection conn = new SqlConnection(conStr))
        {
            SqlCommand cmd = new SqlCommand("Select ProductId,Name,ListPrice From Production.Product Where ProductId=@PrdId", conn);
            cmd.Parameters.AddWithValue("@PrdId", urunId);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                u = new Urun()
                        {
                            Id=urunId
                            ,Name=reader["Name"].ToString()
                            , ListPrice=Convert.ToDouble(reader["ListPrice"])
                        };
            }
            reader.Close();
        }
        return u;
    }

    public void UrunGuncelle(string sinifi, int altKategoriId)
    {
        using (SqlConnection conn = new SqlConnection(conStr))
        {
            SqlCommand cmd = new SqlCommand("Update Production.Product Set ListPrice=ListPrice+1 Where Class=@Class and ProductSubCategoryId=@SubCatId", conn);
            cmd.Parameters.AddWithValue("@Class", sinifi);
            cmd.Parameters.AddWithValue("@SubCatId", altKategoriId);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public int Toplam(int x, int y)
    {
        return x + y;
    }

    public double ToplamFiyat(int altKategoriId)
    {
        double result = 0;
        using (SqlConnection conn = new SqlConnection(conStr))
        {
            SqlCommand cmd = new SqlCommand("Select Sum(ListPrice) From Production.Product Where ProductSubCategoryId=@SubCatId", conn);
            cmd.Parameters.AddWithValue("@SubCatId", altKategoriId);
            conn.Open();
            result =Convert.ToDouble(cmd.ExecuteScalar());
        }
        return result;
    }

    #endregion
}
```

UrunHizmetleri isimli sınıf içerisinde yer alan operasyon uyarlamalarının çoğu AdventureWorks veritabanında yer alan Production.Product tablosu üzerinden çalışmaktadır. Dönüş tipleri için önemli olan serileşebilmeleridir. Şimdi bu servis kütüphanesini sunacak olan basit bir host uygulama yazılabilir. Bu amaçla bir Console projesi göz önüne alınabilir. Sunucu uygulamanın System.ServiceModel.Web.dll, System.ServiceModel.dll assembly'ları haricinde servis sözleşmesini (Service Contract) ve uygulayıcı tipi barındıran kütüphaneyide (WCF Service Library) referans etmesi gerekmektedir. Bu işlemlerin ardından Host uygulama kodları aşağıdaki gibi geliştirilebilir.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UrunLib;
using System.ServiceModel.Web;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Sunucu
{
    class Program
    {
        static void Main(string[] args)
        {
            WebServiceHost host = new WebServiceHost(typeof(UrunHizmetleri), new Uri("http://localhost:60001/"));
            ServiceEndpoint ep=host.AddServiceEndpoint(typeof(IUrunHizmetleri), new WebHttpBinding(), "");

            host.Opened += delegate(object sender, EventArgs e)
                                {
                                    Console.WriteLine("Servis açık");
                                };
            host.Closed += delegate(object sender, EventArgs e)
                                {
                                    Console.WriteLine("Servis kapatıldı");
                                };

            host.Open(); 

            Console.WriteLine("Kapatmak için bir tuşa basın");
            Console.ReadLine();
            host.Close();
        }
    }
}
```

İlk olarak bir adet WebServiceHost nesnesi örneklenmektedir. WebServiceHost sınıfının yapıcı metoduna (Constructor) parametre olarak sözleşmeyi uygulayan sınıf tipi (Class Type) ve yayınlanacağı adres bilgisi verilmektedir. Sonrasında zorunlu olmamakla birlikte WebHttpBinding bağlayıcı tipini barındıran bir EndPoint host isimli WebServiceHost nesne örneğine eklenmektedir. Diğer EndPoint tanımlamalarında olduğu gibi önce servis sözleşmesi (Service Contract), sonrasında bağlayıcı tip (Binding Type) bildirilir. Servis adresi (Service Address) ise zaten host nesnesi örneklenirken tanımlandığı için boş geçilebilir. Bu işlemlerin ardından servis Open metodu ile açılır. Eğer yazılan kodlarda herhangibir aksilik yoksa uygulamanın çalışma zamanı görüntüsü aşağıdakine benzer olmalıdır.

![mk242_5.gif](/assets/images/2008/mk242_5.gif)

Artık HTTP GET metodu bazlı taleplerde bulunulabiliriz. Söz konusu talepleri (Request) basit bir tarayıcı (Browser) uygulaması yardımıyla gerçekleştirmek mümkündür. Elbette istemcilerin cevap (Response) alabilmeleri için Host uygulamanında çalışıyor olması şarttır. Aşağıda söz konuzu operasyon çağrıları ve ekran görüntüleri yer almaktadır.

Toplama işlemi için yapılan çağrı; http://localhost:60001/ToplamaIslemi?sayi1=4&sayi2=6 (4 ve 6 sayılarının toplamı geriye döndürülmektedir)

![mk242_6.gif](/assets/images/2008/mk242_6.gif)

UrunListesi operasyonu için yapılan çağrı; http://localhost:60001/urunler/1 (1 numaralı ProductSubCategoryId değerine sahip ürünlerin DataSet içerisinde toplanmış listesini verir)

![mk242_7.gif](/assets/images/2008/mk242_7.gif)

UrunCek isimli metoda yapılan çağrı; http://localhost:60001/UrunCek?urunId=1 (ProductId alanı değeri 1 olan Product tablosu satırının içeriği Urun tipine göre çekilir)

![mk242_8.gif](/assets/images/2008/mk242_8.gif)

ToplamFiyat operasyonu için yapılan çağrı; http://localhost:60001/toplam?kategori=1 (ProductSubCategoryId değeri 1 olan ürünlerin ListPrice alanlarının toplamı elde edilir.)

![mk242_9.gif](/assets/images/2008/mk242_9.gif)

Tabi sunucunun çalışmaması halinde gelecek olan istemci taleplerinde aşağıdaki gibi ekran görüntüleri ile karşılaşılabilir.

![mk242_10.gif](/assets/images/2008/mk242_10.gif)

Görüldüğü gibi URL bazlı olacak şekilde servis operasyonları çağırılabilmekte ve sonuçları alınabilmektedir. Gelelim UrunGuncelle isimli metodun test edilmesine. Bu operasyon bir istemci uygulama üzerinden test edilebilir. POST metoduna göre bir talepte bulunulacağından istemcinin servis sözleşmesini (Service Contract) varsa veri sözleşmesini (Data Contract) bilmesi gerekmektedir. Bunların dışından istemci uygulamanın System.ServiceModel.dll, System.Runtime.Serialization.dll (3.0 versiyonu) ve System.ServiceModel.Web.dll assembly'larını referans etmesi gerekmektedir. Bu amaçla hazırlanan örnek istemci bir Console uygulaması olarak tasarlanabilir. Söz konusu uygulamanın kodları ise aşağıdaki gibidir.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Data;
using System.ServiceModel.Web;
using System.Runtime.Serialization;

namespace Istemci
{
    [DataContract]
    public class Urun
    { 
        [DataMember]
        public int Id; 
        [DataMember]
        public string Name; 
        [DataMember]
        public double ListPrice;
    }

    [ServiceContract]
    public interface IUrunHizmetleri
    {
        [OperationContract]
        [WebGet(UriTemplate = "urunler/{altkategoriId}")]
        DataSet UrunListesi(string altKategoriId);
    
        [OperationContract]
        [WebGet]
        Urun UrunCek(int urunId);
    
        [OperationContract]
        [WebInvoke(UriTemplate = "guncelle?sinifi={sinifi}&altKategori={altKategoriId}")]
        void UrunGuncelle(string sinifi, int altKategoriId);

        [OperationContract]
        [WebGet(UriTemplate = "toplamaIslemi?sayi1={x}&sayi2={y}")]
        int Toplam(int x, int y);

        [OperationContract]
        [WebGet(UriTemplate = "toplam?kategori={altKategoriId}")]
        double ToplamFiyat(int altKategoriId);
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Başlatmak için bir tuşa basın");
            Console.ReadLine();
            using (WebChannelFactory<IUrunHizmetleri> cf = new WebChannelFactory<IUrunHizmetleri>(new Uri("http://localhost:60001")))
            {
                IUrunHizmetleri hiz=cf.CreateChannel();
                hiz.UrunGuncelle("M", 1);
            }
        }
    }
}
```

İlk olarak WebChannelFactory generic tipinden bir nesne örneklenmektedir. Burada generic tip olarak servis sözleşmesi kullanılmalıdır. Dikkat edilecek olursa parametre olarak servisin host edildiği URL adresi verilmekedir. Sonrasında yapılan metod çağrısı HTTP POST tekniğine göre sunucuya ulaşacak ve operasyon sonuçlandırılacaktır. Önce sunucu sonrasında ise istemci çalıştırılırsa istemci uygulama başarılı bir şekilde yürütülecek ve Class değeri M, ProductSubCategoryID değeri 1 olan Product tablosu satırlarının ListPrice değerlerinin 1 birim arttırıldığı görülecektir. Tahmin edileceği gibi istemci tarafında yapılan bu metod çağırısı, sunucuya HTTP protokolünün POST metoduna göre ulaşacaktır. Metod içerisinde kullanılan parametre değerleride servis tarafına vardıklarında aynen alınıp değerlendirilebilecektir. Eğer sunucu çalışmıyorken istemci çalıştırılırsa çalışma zamanında EndPointNotFoundException sınıfı tipinden bir istisna (Exception) alındığı görülebilir.

![mk242_11.gif](/assets/images/2008/mk242_11.gif)

Buraya kadar işlenenler göz önüne alındığında, istemcilerin HTTP üzerinden GET,POST, PUT, DELETE gibi metodları kullanarak WCF operasyonlarını talep (Request) edebileceği görülmüştür. Host uygulama test olması açısından Console olarak tasarlanmış olmakla birlikte IIS (Internet Information Service) üzerindede konuşlandırılabilir. Bu tip bir durumda svc uzantılı servis dosyasında yer alan Service direktifinde Factory isimli niteliği (attribute) kullanmak ve System.ServiceModel.Activation.WebServiceFactory değerini atamak yeterlidir. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](/assets/files/2008/WebStyleServices.rar)