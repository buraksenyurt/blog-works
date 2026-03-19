---
layout: post
title: "RSS, Atom Formatlı İçerik Paylaşımı(Syndication)"
date: 2008-02-08 12:00:00 +0300
description: "Windows Communication Foundation, Net Framework 3.5 ile gelen bazı yeni CLR(Common Language Runtime) tipleri sayesinde RSS 2.0 ve Atom 1.0 formatlarında yada diğer özel formatlarda içerik paylaşımı(Syndication) yapılmasına izin veren özelliklere sahip olmuştur. Bu tipler sayesinde bir WCF servisi(Service) üzerinden özellikle HTTP protokolünün GET, POST, HEAD ve benzeri metodlarına göre talep-cevap(Request-Response) işlemleri gerçekleştirilebilmektedir. Bir başka deyişle EndPoint noktaları üzerinden farklı tipte verilerin, dünya standartlarına uygun olacak şekilde yayınlanması mümkündür. İşte bu makalemizde bir WCF servisi üzerinden en basit haliyle RSS veya Atom formatında içerik paylaşımlarının nasıl yapılabileceğini incelemeye çalışacağız."
categories:
  - wcf
tags:
  - wcf
  - csharp
  - dotnet
  - linq
  - xml
  - http
  - generics
  - visual-studio
---
# RSS, Atom Formatlı İçerik Paylaşımı(Syndication)
Değerli Okurlarım Merhabalar,

Windows Communication Foundation, Net Framework 3.5 ile gelen bazı yeni CLR (Common Language Runtime) tipleri sayesinde RSS 2.0 ve Atom 1.0 formatlarında yada diğer özel formatlarda içerik paylaşımı (Syndication) yapılmasına izin veren özelliklere sahip olmuştur. Bu tipler sayesinde bir WCF servisi (Service) üzerinden özellikle HTTP protokolünün GET, POST, HEAD ve benzeri metodlarına göre talep-cevap (Request-Response) işlemleri gerçekleştirilebilmektedir. Bir başka deyişle EndPoint noktaları üzerinden farklı tipte verilerin, dünya standartlarına uygun olacak şekilde yayınlanması mümkündür. İşte bu makalemizde bir WCF servisi üzerinden en basit haliyle RSS veya Atom formatında içerik paylaşımlarının nasıl yapılabileceğini incelemeye çalışacağız.

RSS veya Atom gibi formatların ortak noktası platform bağımsız (Interoperability) veri içerikleri sunulabilmesi için gerekli standartları içeriyor olmalarıdır. Bu sayede yayınlanan veriyi alacak olan istemcilerin (Clients) farklı özelliklerinin düşünülmesine gerek kalmamaktadır. Burada söz konusu olan platform bağımsız yayınlanabilen veriler genellikle Feed olarak adlandırılırlar. Feed yapısı kendi içerisinde, içerik yayınlaması ile ilgili olaraktan yazar (author), başlık (title), adres (url) ve bunlar ile ilişkili olan metadata bilgilerini barındırır. Ayrıca kendi içerisinde birden fazla öğe (Item) barındırabilir. Bu öğelerin her biride kendi içerisinde başlık (title), adres (url), oluşturulma tarihi (creation date), açıklama (description), kategori (category) gibi bilgileri barındırmaktadır. Bu içeriklerin şu anda popüler olan iki farklı sunuş şekli RSS (Really Simple Syndication) ve Atom teknikleridir. Her ikiside XML (eXtensible Markup Languge) tabanlı olacak şekilde içerik paylaşımı standartları sunarlar.

> RSS/Atom gibi içerik yayınlama formatlarının ortak özellikleri, platform bağımsız bir içerik için gerekli olan metadata standartlarını sağlıyor olmalarıdır.

Söz konusu formatların her ikisde Windows Communication Foundation tarafından desteklenmektedir. WCF, söz konusu Feed ve Feed Item'lar ile çalışılabilmesini kolaylaştırmak adına SyndicationFeed, SyndicationItem, SyndicationLink, SyndicationPerson gibi pek çok CLR tipi içermektedir. Tahmin edileceği üzere bu tiplerin isimlendirilmeleri, içerik paylaşım formatlarında kullanılan element adları ile benzerdir. Ancak en önemli avantaj, RSS yada Atom gibi formatlara yönetimli kod tarafından (Managed Code) doğrudan destek veriliyor olmasıdır. Şu anda WCF içerisinde farklı içerik paylaşımı formatlarına destek verebilmek amacıyla Atom10FeedFormatter, RSS20FeedFormatter vb tipler yer almaktadır.

Peki söz konusu RSS veya Atom formatlı içerikler hangi amaçlarla kullanılmaktadırlar? Söz gelimi haber sitelerinin hemen hepsi güncel başlıkları ortak bir standartta yayınlayabilmek için XML tabanlı olan RSS veya Atom formatlı içerikler sunarlar. Aynı sistem blog siteleri içinde geçerlidir. Pek çok blog sitesinde en güncel girişler (Entry) RSS veya Atom formatında (yada iki formatta birden olacak şekilde) yayınlanmaktadır. Elbetteki bu yayınlanan içerikler XML formatlı olduğundan, başka sistemler tarafından alınıp yorumlanabilirler. Söz gelimi haber başlıkları yada blog girişleri yada bir topluluk sitesinde yayınlanan son makalelerin listesi XML içeriklerinden alınıp işlenebilirler. İşlenen bu veriler uygulama bağımsız olacak şekilde değişik kontroller ile son kullanıcılara sunulabilirler.

Örneğin farklı haber sitelerinin RSS/Atom içeriklerini kullanarak istemcilere birer özet şeklinde sunan intranet tabanlı web siteleri son derece yaygındır. Bu tip bir sistemde istemciler doğrudan internete çıkamasalarda, intranet üzerinde erişebildikleri ortak bir portal üzerinden çeşitli haber sitelerinin güncel konu başlıklarına bakabilir ve bilgi alabilirler. Tabiki burada bahsetmiş olduğumuz senaryolar en yaygın kullanılanlarıdır. Geliştirici (Developer) olarak baktığımızda paylaşılabilen herhangibir veri topluluğunu RSS/Atom formatlarında yayınlayabileceğimiz sonucu ortaya çıkmaktadır.

> RSS/Atom gibi formatlarda sunulan içerikler standartlaştırılmış XML verileridir. Bu veriler versiyonlara göre farklılık gösterebilir. Ancak her haldede, XML içeriklerinin ayrıştırılıp (Parse) kullanılması mümkündür..Net içerisinde ezelden beri gelen XML tipleri ile bu işlemler gerçekleştirilebilir. Ancak WCF açısından olaya bakıldığında göze çarpan noktalar şunlardır;
> - RSS veya Atom formatlı verilerin yönetimli kod (Managed Code) tarafında kolayca ele alınmasını sağlayan tipler.Net Framework 3.5 içerisinde gelmektedir. Böylece RSS veya Atom formatlı içeriklerin oluşturulması veya okunması (ayrıştırılması) dahada kolaylaşmaktadır.
> - WCF,.Net 3.5 içerisinde gelen destekler ile HTTP Get gibi bir metod yardımıyla EndPoint'ler üzerinde RSS/Atom desteği verebilecek şekilde kullanılabilmektedir.

Söz gelimi bir veri yönetim sistemi üzerinde çalışan bir WCF servisi, log bilgilerini yetkili kişilere RSS/Atom formatında sunacak şekilde URL desteği verebilir. Burada URL desteğinden kasıt http://localhost:5001/VeriYonetimSistemi/LogServisi?kullaniciId=5 gibi bir adrestir. Dikkat edilecek olursa URL üzerinden yapılacak olan bu talep (request) sonrasında, WCF servisi kullanıcıID değeri 5 olan kişiyi bulup, log bilgilerini RSS/Atom formatında hazırlayarak email olarak gönderebilir. Hemen bu noktada aşağıdaki şekil ile olayı daha net kavrayabiliriz.

![mk241_1.gif](/assets/images/2008/mk241_1.gif)

Şekilden de anlaşılacağı üzere servisin HTTP Get metoduna göre RSS/Atom desteği bulunmaktadır. Bu sistemin gerçekleştirilebilmesi için.Net Framework 3.5 içerisinde WebHttpBinding ve WebHttpBehavior isimli yeni tipler yer almaktadır. Tahmin edileceği üzere WebHttpBinding yeni bağlayıcı tiplerdendir (Binding Type) ve HTTP protokolünün Get gibi metodlarına EndPoint üzerinden destek verilmesini sağlamak amacıyla geliştirilmiştir. Burada önemli olan noktalardan biriside servisin RSS yada Atom formatında veri içeriklerini nasıl hazırlayacağıdır. Yine daha öncedende belirtildiği gibi bu aslında XML formatlı bir metin içeriğinin hazırlanmasından başka bir şey değildir. Ne varki.Net Framework 3.5 içerisinde gelen yardımcı tipler sayesinde bu işlemlerin yönetimli kod (Managed Code) tarafında yapılması mümkündür. Bu amaçla SyndicationFeedFormatter ve SyndicationItemFormatter abstract tiplerinden türemiş olan çeşitli sınıflar bulunmaktadır. Aşağıdaki sınıf diagramlarında temel olarak kullanılabilecek formatlama tipleri gösterilmektedir.

Feed formatlama için kullanılan tipler;

![mk241_2.gif](/assets/images/2008/mk241_2.gif)

Görüldüğü gibi Atom 1.0 ve RSS 2.0 tarzındaki Feed formatları için ikişer tip yer almaktadır. Söz konusu sınıfların generic versiyonları olduğunada dikkat edelim.

Item formatlama için kullanılan tipler;

![mk241_3.gif](/assets/images/2008/mk241_3.gif)

Görüldüğü gibi Atom 1.0 ve RSS 2.0 standartlarına uygun olacak şekilde Item formatlaması için kullanılan ikişer farklı tip vardır. Bu tiplerden hepsi SyndicationItemFormatter abstract sınıfından türemekte ve XML serileştirmesi için gerekli IXmlSerializable arayüzünü (Interface) uygulamaktadır.Bu bilgiler ışığında kendi formatlama modellerimizide geliştirebileceğimizi söyleyebiliriz. Bu tarz bir işlem için Feed veya Item formatlamasının farklı bir versiyonunu yazmak istiyorsak SyndicationItemFormatter, SyndicationFeedFormatter abstract sınıfları ile IXmlSerializable arayüzünü göz önüne almamız yeterlidir.

.Net 3.5 içerisindeki nesne modeline bakıldığında SyndicationFeed, SyndicationItem, SyndicationCategory, SyndicationPerson, SyndicationContent gibi pek çok CLR tipinin (Common Language Runtime Type) yer aldığı görülür. Bu tiplerden belkide en çok kullanılanları SyndicationFeed ve SyndicationItem sınıflarıdır. Söz konusu sınıflar System.ServiceModel.Web.dll assembly içerisinde yer alan System.ServiceModel.Syndication isim alanında (Namespace) bulunmaktadır.

Bu kadar teorik bilgiden sonra bir kaç örnek ile konuyu genişletmeye çalışalım. İlk olarak basit bir Console uygulaması geliştirecek ve RSS 2.0/ Atom 1.0 formatlarında içeriklerin nasıl hazırlanabileceğini incelemeye çalışacağız. Bu amaçla Visual Studio 2008 üzerinden.Net 3.5 modeline uygun olacak şekilde bir Console uygulaması açmamız ve System.ServiceModel.Web.dll'ini projeye referans etmemiz yeterlidir.

![mk241_4.gif](/assets/images/2008/mk241_4.gif)

Bu işlemin ardından kodları aşağıdaki gibi geliştirebiliriz.

```csharp
using System;
using System.ServiceModel.Syndication;
using System.Collections.ObjectModel;
using System.Xml;

namespace SyndicationFormatlama
{
    class Program
    {
        static void Main(string[] args)
        {
            // Bir Feed oluşturulur. Parametreler title, description ve Uri bilgileridir.
            SyndicationFeed feed=new SyndicationFeed("Makaleler","Burak Senyurt .Net Makaleleri",new Uri("http://www.bsenyurt.com"));
            // Feed yazarı tanımlanır. Yazarlar SyndicationPerson tipi ile temsil edilebilirler.
            feed.Authors.Add(new SyndicationPerson("selim(at)buraksenyurt.com"));
            // Feed için bir kategori tanımalası yapılır. Bu kategori tanımlaması SyndicationCategory tipi ile temsil edilebilir.
            feed.Categories.Add(new SyndicationCategory(".Net Teknolojileri"));
            // Feed içeriğinin dili belirtilir.
            feed.Language = "TR-TR";
            // Son güncelleme tarihi atanır.
            feed.LastUpdatedTime = DateTime.Now;

            // Feed içerisinde yer alacak öğelerin her biri SyndicationItem tipindendir.
            // SyndicationFeed tipinin Items özelliği bu nesne örneklerini barındırır.
            // Items özelliği Collection<SyndicationItem> tipinden koleksiyonları kullanır.
            // C# 3.0 Object Initializers yardımıyla koleksiyon oluşturulur ve örnek öğeler eklenir.
            Collection<SyndicationItem> items = new Collection<SyndicationItem>()
                                {
                                    // Feed içerisindeki öğeler(Items) SyndicationItem tipi ile temsil edilirler.
                                    // Parametreler title,content,uri,id,lastUpdatedTime
                                    new SyndicationItem("WCF - Front End Service Geliştirmek","WCF içerisinde içerik yayınlama",new Uri("http://www.bsenyurt.com/MakaleGoster.aspx?ID=241"),"1",new DateTime(2008,1,30))
                                    ,new SyndicationItem("Adım Adım State Machine Worflow Geliştirmek","Finite State Machine nasıl geliştirilir.",new Uri("http://www.bsenyurt.com/MakaleGoster.aspx?ID=240"),"2",new DateTime(2008,1,15))
                                }; 

            feed.Items = items; // oluşturulan öğelere ait koleksiyon Feed için set edilir.

            // Atom 1.0 notasyonunda formatlama için SyndicationFeed nesne örneğinin GetAtom10Formatter metodu ile Atom10FeedFormatter nesnesi örneklenir.
            Atom10FeedFormatter atom10Formatter = feed.GetAtom10Formatter();
        
            // Rss 2.0 notasyonunda formatlama için SyndicationFeed nesne örneğinin GetRss20Formatter metodu ile Rss20FeedFormatter nesnesi örneklenir.
            Rss20FeedFormatter rss20Formatter = feed.GetRss20Formatter();
        
            // Atom içeriğinin kaydedileceği Xml dosyası XmlWriter tipi ile oluşturulur
            XmlWriter atom10writer = XmlWriter.Create("MakalelerAtom.xml");
            // Formatter nesnesinin WriteTo metodu ile Atom 1.0 notasyonunda formatlanan veri içeriği XmlWriter nesnesinin işaret ettiği fiziki dosyaya yazılır.
            atom10Formatter.WriteTo(atom10writer);
            // XmlWriter nesnesi kapatılır
            atom10writer.Close();
        
            // Atom 1.0 için yapılan formatlama işlemi Rss 2.0 için benzer şekilde yapılır.
            XmlWriter rss20writer = XmlWriter.Create("MakalelerRss.xml");
            rss20Formatter.WriteTo(rss20writer);
            rss20writer.Close();
        }
    }
}
```

Yukarıdaki kod parçasında örnek olarak oluşturulan Feed içerikleri Atom 1.0 ve RSS 2.0 formatlarına uygun olacak şekilde fiziki XML dosyalarına yazdırılmaktadır. Feed oluşturulması için SyndicationFeed sınıfına ait nesne örnekleri kullanılır. Bununla birlikte içerik paylaşımı ile ilgili ekstra metadata bilgileri söz konusu nesne örneğinin çeşitli özellikleri yardımıyla belirlenebilir. Örneğin içeriği paylaşan yazar Author özelliği ile belirlenebilir. Yazar gibi bilgiler içerisinde mail adresi tarzında ek verilerde olabileceğinden SyndicationPerson sınıfına ait nesne örnekleri ile author elementinin yönetimli kod tarafında ele alınması sağlanabilmektedir. Benzer durum kategori bilgileri içinde geçerlidir. Kategori için SyndicatioCategory sınıfından yararlanılmaktadır. Feed içerisinde yer alan bilgilendirici öğeler (Items), SyndicationFeed sınıfı içerisinde yer alan Items özelliği (Property) ile tutulmaktadır. Items özelliği her bir elemanı SyndicationItem sınıfından olan generic Collection tipi ile ele alınabilir.

Feed ve içeriği oluşturulduktan sonra bu verinin Atom 1.0 veya RSS 2.0 formatında üretilmesi için Atom10FeedFormatter ve Rss20FeedFormatter sınıflarına ait nesne örneklerinden yararlanılmaktadır. Bu nesne örneklerinin üretimi için SyndicationFeed sınıfına ait GetAtom10Formatter ve GetRss20Formatter metodları kullanılır. Veri içerikleri XML formatlı olarak yazılabileceklerinden fiziki kaynağı işaret etmek adına XmlWriter tipinden yararlanılır. Söz konusu uygulama çalıştırıldığında MakalelerRss.xml ve MakalelerAtom.xml dosyalarının içeriklerinin aşağıdaki gibi oluştuğu görülür.

MakalelerRss.xml içeriği;

![mk241_5.gif](/assets/images/2008/mk241_5.gif)

MakalelerAtom.xml içeriği;

![mk241_6.gif](/assets/images/2008/mk241_6.gif)

Her iki yapının içeriğini aşağıdaki grafik ile daha kolay bir şekilde de karşılaştırabiliriz.

![mk241_7.gif](/assets/images/2008/mk241_7.gif)

Görüldüğü üzere arada bazı farklılıklar mevcuttur. Ancak her iki formatta genel standarttır. Gelelim bu içerikleri bir WCF servisi üzerinden nasıl yayınlayabileceğimize. Öncelikli olarak RSS formatında yayınlama yapılmasına izin veren bir WCF Servis Kütüphanesi (WCF Service Library) geliştiriyor olacağız. Bu amaçla Visual Studio 2008 ortamında.Net Framework 3.5 şablonları içerisinde yer alan WCF Service Library proje tipi seçilerek işe başlanabilir. Elbette Synidaction için gerekli tipleri kullanacağımızdan servis kütüphanesinin System.ServiceModel.Web.dll assembly'ınıda referans etmesi gerekmektedir.

![mk241_9.gif](/assets/images/2008/mk241_9.gif)

Servis kütüphanesi içerisinde yer alacak olan sözleşme (Contract) ve uygulayıcı sınıfa ait sınıf diyagramı (Class Diagram) ile içerikleri ise aşağıdaki gibidir.

![mk241_8.gif](/assets/images/2008/mk241_8.gif)

IPaylasim arayüzünün (Interface) içeriği;

```csharp
using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Collections.Generic;
using System.ServiceModel.Syndication;

namespace RssAtomLibrary
{
    [ServiceContract]
    public interface IPaylasim
    {
        [OperationContract]
        [WebGet]
        Rss20FeedFormatter RssCiktisi();
    }
}
```

Arayüz (Interface) tanımlamasında belkide dikkati çeken en önemli noktalardan biriside WebGet isimli niteliğin (attribute) kullanıllmasıdır. Bu nitelik söz konusu servise HTTP Get metoduna göre talepte (Request) bulunabileceğini göstermektedir.

Paylasim sınıfının (Class) içeriği;

```csharp
using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Collections.Generic;
using System.ServiceModel.Syndication;

namespace RssAtomLibrary
{

    public class Paylasim : IPaylasim
    {
        #region IPaylasim Members

        public Rss20FeedFormatter RssCiktisi()
        {
            // Öncelikle bir Feed oluşturulur
            SyndicationFeed feed = new SyndicationFeed();
    
            // İçerik paylaşımı yapan yazarlar SyndicationPerson sınıfı yardımıyla tanımlanırlar ve Authors koleksiyonuna dahil edilirler.
            feed.Authors.Add(new SyndicationPerson("selim(at)buraksenyurt.com", "Burak Selim Senyurt", "http://www.bsenyurt.com"));
            feed.Authors.Add(new SyndicationPerson("kariim@bsenyurt.com", "Kariim Abdul Cabbar", "http://www.bsenyurt.com"));

            // İçeriğin ilgili olduğu kategoriler SyndicationCategory sınıf yardımıyla örneklenir ve Categories koleksiyonuna dahil edilir.
            feed.Categories.Add(new SyndicationCategory(".Net"));
            feed.Categories.Add(new SyndicationCategory("C#"));

            // İçerik il ilgili açıklama TextSyndicationContent sınıfı yardımıyla eklenir
            feed.Description = new TextSyndicationContent(".Net ve C# ağırılıklı makaleler", TextSyndicationContentKind.Html);
            feed.Language = "Tr-Tr"; // İçerik dili belirtilir
            feed.LastUpdatedTime = DateTime.Now; // Son güncelleme tarihi belirtilir
            feed.Title=new TextSyndicationContent(".Net ile ilgili Herşey"); // İçeriğin başlığı TextSyndicationContent sınıfı yardımıyla belirtilir.
    
            // Ogeler eklenir
            List<SyndicationItem> items = new List<SyndicationItem>()
                    {
                        // Feed içerisindeki öğeler(Items) SyndicationItem tipi ile temsil edilirler.
                        // Parametreler title,content,uri,id,lastUpdatedTime
                        new SyndicationItem("WCF - Front End Service Geliştirmek","WCF içerisinde içerik yayınlama",new Uri("http://www.bsenyurt.com/MakaleGoster.aspx?ID=241"),"1",new DateTime(2008,1,30))
                        ,new SyndicationItem("Adım Adım State Machine Worflow Geliştirmek","Finite State Machine nasıl geliştirilir.",new Uri("http://www.bsenyurt.com/MakaleGoster.aspx?ID=240"),"2",new DateTime(2008,1,15))
                    };

               feed.Items = items;
            return new Rss20FeedFormatter(feed);    
        }

        #endregion
    }
}
```

Paylasim sınıfının RssCiktisi isimli metodu Rss20FeedFormatter tipinden bir nesne örneğini döndürmektedir. Bu nesne örneklenirken parametre olarak SyndicationFeed tipinden oluşturulan nesne örneği parametre olarak verilmektedir. Servis kütüphanesinin (Service Library) bu şekilde hazırlanmasının ardından artık Host uygulamanın yazılmasına geçilebilir. Host uygulama servis kütüphanesini, System.ServiceMode.dll ve yine System.ServiceModel.Web.dll assembly'larını referans etmelidir.

![mk241_10.gif](/assets/images/2008/mk241_10.gif)

Host uygulama basit olarak bir Console programı şeklinde tasarlanabilir.

```csharp
using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using RssAtomLibrary;
using System.ServiceModel.Syndication;
using System.Xml;

namespace Sunucu
{
    class Program
    {
        static void Main(string[] args)
        {
            WebServiceHost host = new WebServiceHost(typeof(Paylasim), new Uri("http://localhost:65001/MakalePaylasimServisi"));

            host.Open();
            
            Console.WriteLine("Servis durumu {0} ", host.State);
            Console.WriteLine("Host dinlemede...Çıkmak için bir tuşa basınız...");
            Console.ReadLine();
            
            host.Close(); 
        }
    }
}
```

Host uygulamada ilk dikkati çeken noktalardan birisi WebServiceHost sınıfına ait bir nesne örneğinin kullanılmasıdır. Bilindiği gibi normal şartlarda ServiceHost sınıfından yararlanılmaktadır. WebServiceHost nesnesi örneklenirken ilk parametre olarak servis sözleşmesini (Service Contract) uygulayan sınıfın tipini almaktadır. Sonraki parametrede ise servis adresi belirlenmektedir. Host'un açılması için yine Open metoduna başvurulmaktadır. Benzer şekilde kapatma işlemi içinde Close fonksiyonundan yararlanılır. Bu işlemlerin ardından servis uygulaması çalıştırılabilir. Servis uygulaması çalışıyorken herhangibir tarayıcı penceresinden http://localhost:65001/MakalePaylasimServisi/RssCiktisi adresi talep edilirse aşağıdaki ekran çıktısında yer alan görüntü elde edilecektir.

![mk241_11.gif](/assets/images/2008/mk241_11.gif)

Görüldüğü gibi servisteki RssCiktisi isimli metodun sonucu HTTP üzerinden elde edilebilmektedir. Burada dikkat edilmesi gereken önemli bir nokta vardır. URL adresinin sonu, RSS çıktısı veren metodun adı ile aynıdır. Eğer farklı bir adres girilirse Service EndPoint bulunamayacak ve aşağıdakine benzer bir ekran ile karşılaşılacaktır.

![mk241_12.gif](/assets/images/2008/mk241_12.gif)

Bu durum istemci tarafınada bir çalışam zamanı istisnası (Runtime Exception) olarak yansıyacaktır. Peki istemci kod tarafından bu tip bir içeriği nasıl ele alabilir? Bu amaçla basit bir istemci uygulamayı Console projesi olacak şekilde geliştirip devam edelim. Istemci tarafında standart XmlReader veya XmlDocument nesneleri yardımıyla HTTP Get ile talep edilen servis içeriği çekilebilir. Ancak.Net Framework 3.5 içerisinde gelen Syndication tipleri yardımıyla bu işlemi gerçekleştirmek çok daha kolaydır. Bu nedenle istemci tarafındaki uygulamaya System.ServiceModel.Web.dll assembly'ının referans edilmesi gerekmektedir.

![mk241_14.gif](/assets/images/2008/mk241_14.gif)

Dikkat edileceği üzere herhangibir şekilde servis referansı eklenmemiş yada proxy sınıfı oluşturulmamıştır. Nitekim servise gönderilecek olan talep (request) aslında EndPoint davranışı sergileyen bir metoda doğru HTTP Get üzerinden sağlanacaktır. Bu sebeplerden istemci tarafındaki kodlar çok basit olarak aşağıdaki gibi geliştirilebilirler.

```csharp
using System;
using System.Xml;
using System.ServiceModel.Syndication;

namespace Istemci
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("RSS içeriğini çekmek için bir tuşa basınız");
            Console.ReadLine();

            XmlReader reader = XmlReader.Create("http://localhost:65001/MakalePaylasimServisi/RssCiktisi");
            SyndicationFeed feed = SyndicationFeed.Load(reader);

            Console.WriteLine(feed.Title.Text);
            foreach (SyndicationPerson yazar in feed.Authors)
            {
                Console.WriteLine("\t{0} \t {1}",yazar.Name,yazar.Email);
            }

            foreach (SyndicationItem item in feed.Items)
            {
                Console.WriteLine(String.Format("{0} : {1} {2}",item.Id,item.Title.Text,item.Links[0].Uri));
            }
        }
    }
}
```

İlk olarak XmlReader sınıfından yararlanılarak http://localhost:65001/MakalePaylasimServisi/RssCiktisi adresinden talepte bulunulmaktadır. Bu talep sonrası elde edilen XML çıktısının SyndicationFeed sınıfı tarafından daha kolay bir şekilde ele alınabilmesini sağlamak amacıyla static Load metoduna XmlReader nesne örneği parametre olarak verilir. Bu işlemin ardından Feed ile ilgili olarak başlık (title) bilgisi elde edilir. Bununla birlikte SyndicationPerson sınıfı ve Authors özelliklerinden yararlanılarak yazarlara ait isim (Name) ve elektronik posta (e mail) bilgileri çekilir. Son olarakta Items koleksiyonu dolaşılarak var olan tüm öğeler SyndicationItem sınıfına ait nesne örnekleri yardımıyla ele alınır. Örnek olarak öğelerin Id, başlık (Title) ve url bilgileri verilir. İstemcilerin talepte bulunabilmesi ve Feed içeriklerini çekebilmesi için çok doğal olarak servis tarafınının çalışıyor olması gerekir. Eğer bu şart sağlanırsa aşağıdakine benzer bir ekran görüntüsü ile karşılaşılacaktır.

![mk241_13.gif](/assets/images/2008/mk241_13.gif)

Görüldüğü gibi Feed içeriği istemci tarafına başarılı bir şekilde aktarılmıştır.

İçerik paylaşımı (Syndication) ile ilişkili bir diğer önemli konuda bir servisin yeri geldiğinde RSS 2.0 formatında yeri geldiğinde de Atom 1.0 formatında içerik paylaşımına izin vermesidir. Bu amaçla servis tarafında bazı değişiklier yapmak gerekecektir. Bu amaçla az önce geliştirilen servis kütüphanesindeki (WCF Service Library) arayüzü (Interface) aşağıdaki gibi değiştirerek işe başlayalım.

```csharp
[ServiceContract]
[ServiceKnownType(typeof(Rss20FeedFormatter))]
[ServiceKnownType(typeof(Atom10FeedFormatter))]
public interface IPaylasim
{
    [OperationContract]
    [WebGet]
    Rss20FeedFormatter RssCiktisi();

    [OperationContract]
    [WebGet(UriTemplate="IcerikOzeti?icerikTipi={icerikTipi}")]
    SyndicationFeedFormatter IcerikOzeti(string icerikTipi);
}
```

Öncelikli olarak ServiceKnownType niteliği (attribute) ile servisin Rss20 ve Atom10 formatlarından nesne örnekleri döndürebileceği belirtilmektedir. Nitekim içerik paylaşım modelinde ilgili servis metodlarının SyndicationFeedFormatter sınıfı ile taşınabilecek tipler döndürmeleri şarttır. Diğer taraftan IcerikOzeti metodunda kullanılan WebGet niteliğinde bir querystring bildirimi yapılmaktadır. Bu querystring bildirimi metoda HTTP Get üzerinden gelecek olan parametre bilgisinin şablonunu tanımlamaktadır. Arayüz sözleşmesini (Interface Contract) uygulayan sınıf içerisindeki metodun ise aşağıdaki gibi geliştirilmesi yeterlidir.

```csharp
public SyndicationFeedFormatter IcerikOzeti(string icerikTipi)
{
    SyndicationFeed feed = new SyndicationFeed();

    feed.Authors.Add(new SyndicationPerson("selim(at)buraksenyurt.com", "Burak Selim Senyurt", "http://www.bsenyurt.com"));
    feed.Authors.Add(new SyndicationPerson("kariim@bsenyurt.com", "Kariim Abdul Cabbar", "http://www.bsenyurt.com"));

    feed.Categories.Add(new SyndicationCategory(".Net"));
    feed.Categories.Add(new SyndicationCategory("C#"));

    feed.Description = new TextSyndicationContent(".Net ve C# ağırılıklı makaleler", TextSyndicationContentKind.Html);
    feed.Language = "Tr-Tr"; 
    feed.LastUpdatedTime = DateTime.Now; 
    feed.Title = new TextSyndicationContent(".Net ile ilgili Herşey"); 

    List<SyndicationItem> items = new List<SyndicationItem>()
        {
            new SyndicationItem("WCF - Front End Service Geliştirmek","WCF içerisinde içerik yayınlama",new Uri("http://www.bsenyurt.com/MakaleGoster.aspx?ID=241"),"1",new DateTime(2008,1,30))
            ,new SyndicationItem("Adım Adım State Machine Worflow Geliştirmek","Finite State Machine nasıl geliştirilir.",new Uri("http://www.bsenyurt.com/MakaleGoster.aspx?ID=240"),"2",new DateTime(2008,1,15))
        };
    feed.Items = items;

    if (icerikTipi == "atom")
        return new Atom10FeedFormatter(feed);
    else if (icerikTipi == "rss")
        return new Rss20FeedFormatter(feed);
    else
        return null;
}
```

Bu metod içerisinde dikkat edilmesi gereken en önemli nokta icerikTipi değerine göre Atom10FeedFormatter veya Rss20FeedFormatter tipinden nesne örnekleri döndürülmesidir. Bu sebepten dolayıda metodun dönüş tipi SyndicationFeedFormatter tipindendir. Artık istemciler servise talepte bulunurlarken rss veya atom formatında içerik isteyebilirler. Aşağıdaki ekran çıktılarında bu durum açık bir şekilde görülmektedir.

Rss talebi;

![mk241_16.gif](/assets/images/2008/mk241_16.gif)

Atom talebi;

![mk241_15.gif](/assets/images/2008/mk241_15.gif)

Böylece geldik bir makalemizin daha sonuna. Bu makalemizde basit olarak.Net Framework 3.5 ile gelen tipler sayesinde bir WCF (Windows Communication Foundation) servisi üzerinden RSS 2.0 veya Atom 1.0 formatında içerik paylaşımının nasıl yapılabileceğini incelemeye çalıştık. İlerleyen makalelerimizde WCF in.Net Framework 3.5 ile gelen yeniliklerini incelemeye devam edeceğiz. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](/assets/files/2008/WCFSyndication.rar)