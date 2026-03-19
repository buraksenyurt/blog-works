---
title: "WCF WebHttp Services - Tanışma"
pubDate: 2010-02-01 05:50:00
categories:
  - WCF Eco System
  - WCF WebHttp Services
tags:
  - WCF Eco System
  - WCF WebHttp Services
  - C#
  - .NET
  - ASP.NET
  - Entity Framework
  - LINQ
  - WCF
  - SOAP
  - REST
  - HTTP
  - IIS
  - Generics
  - Visual Studio
  - Rc
---

# WCF WebHttp Services - Tanışma
![blg127_Giris.jpg](images/blg127_Giris.jpg)

Merhaba Arkadaşlar,

Nihayet taşlar yerli yerine oturmaya başladı. 2008 yılında düzenlenen Microsoft PDC'de tanıtılan sürüm ile başlayan macerada Beta 1, Beta 2 versiyonları derken yavaş yavaş RC, RTM sürümlerinin çıkacağı günlere gelmekteyiz. Elbette hepizimin beklentisi bir an önce stabil bir sürüme kavuşabilmek. Bu günlerde çok doğal olarak.Net Framework 4.0 ve Visual Studio 2010 ürünlerinin sınırlarının daha da netleştiğini görmeye başladık. Her ne kadar henüz yayınlanmış yeni bir sürüm olmasa da, pek çok güncel ve geçerli kaynaktan okuduğumuz kadarı ile bu böyle. Taşların yerli yerine oturmaya başladığı ve herşeyin biraz daha belirginleştiği alanlardan biriside Windows Communication Foundation 4.0

Hatırlayacağınız üzere [WCF Eco System'i anlattığımız yazımızda](WCF Eco System.md), WCF alt yapısı üzerine geliştirilen ve amaca yönelik olarak farklılaştırılan servis geliştirme modellerine değinmiştik. Bunlardan biriside WebHttp Services idi. Bu yazımız ile birlikte WebHttp Service'lerini tanımaya çalışacağız. Aslında WCF 3.5 sürümüne kazandırılan Web programlama teknikleri sayesinde zaten uzun bir süredir farkında olduğumuz non-SOAP bazlı bir modelden bahsediyoruz. Bildiğiniz üzere WebGet ve WebInvoke isimli nitelikler (attribute) yardımıyla servis operasyonlarının HTTP Get,Post,Put ve Delete metodlarına cevap verebilecek şekilde tasarlanması mümkün. Ancak zaman ilerledikçe REST (REpresentational State Transfer) modeline göre WCF servislerinin daha kolay geliştirilmesini sağlayan ve WCF 4.0 içerisinde gömülecek yeni özelliklerin bir ön görünümünü bizlere sunan WCF REST Starter Kit ile karşılaştık.

WCF Eco System'in bir parçası olan WebHttp Service'ler,.Net Framework 3.5 ile gelen Web Programlama modeli, REST Starter Kit ile tanıtılan kabiliyetler ve bunlara ek yeni özelliklerin.Net Framework 4.0 içerisinde ele alınmasını sağlayan bir geliştirme alt yapısı olarakta düşünülebilir. Şimdi dilerseniz WebHttp Service'lere bir merhaba demeye çalışalım. Örneğimizi Visual Studio 2010 Ultimate Beta 2 sürümü üzerinden geliştirmeye çalışıyor olacağız. Ancak yazıyı hazırladığım tarihte araştırdığım MSDN, The.NET Endpint vb blog sitelerinde yer alan bilgilere göre Visual Studio 2010' un o anki sürümü üzerinde WebHttp Service'leri için bir proje şablon (project template) bulunmamaktaydı.

Bu nedenle öncelikli olarak online template'lerden WebHttp Service için olanları indirmemiz gerekiyor. Bu amaçla Visual Studio 2010 ortamında Tools->Extensions Manager->Online Gallery kısmına geçiş yapıp WCF ile ilişkili olan şablonlardan WCF Rest Service Template'i indirmemiz gerekiyor. Ben 4.0 versiyonunun C# programlama dili destekli olanını indirdim. Son sürümde büyük ihtimalle şu anda online olarak indirdiğimiz bu şablonun ve başka diğer şablonların Visual Studio 2010 içerisine gömülü olarak geleceğini ümit etmekteyim.

![blg127_Template.gif](images/blg127_Template.gif)

Download ve Install işlemlerinin ardından yolumuza devam edebiliriz. Bu amaçla, Visual Studio 2010 ortamında yeni bir proje oluşturup, Web sekmesinde yer alan WCF Rest Service Application şablonunu seçmemiz yeterli olacaktır. Bu şablonun kurulum işlemi sonrasında çıkmaması olasıdır. Bu durumda New Project iletişim kutusunda yer alan Enable the loading per-user extensions bağlantısını kullanarak etkinleştirme işlemini yapmamız yeterli olacaktır. Visual Studio 2010 ortamımızı tekrardan açtığımızda aşağıdaki ekran görüntüsünde olduğu gibi yeni proje şablonunun kullanabilir olduğunu göreceğiz.

![blg127_NewProject.gif](images/blg127_NewProject.gif)

HelloWebHttp isimli servis uygulamasına ait Solution içeriğinin ilk etapta otmatik olarak aşağıdaki gibi oluşturulduğunu gözlemleyebiliriz.

![blg127_Solution.gif](images/blg127_Solution.gif)

Service1.cs isimli örnek dosya içerisinde servis sözleşmesi (Service Contract) yer almaktadır. Bu sözleşme içerisinde yer alan metodlara WebGet ve WebInvoke niteliklerinin (Attributes) uygulandığı görülmektedir. Bu nitelikler bildiğiniz üzere servis operasyonlarına HTTP Post,Get,Put,Delete çağrılarının yapılabilmesi için gereklidir. Sınıf içerisine Get, Post, Put ve Delete metodlarının her biri için örnek nitelik kullanımları serpiştirilmiştir. Ayrıca serileştirilebilir örnek bir tipte yer almaktadır (SampleItem). GetCollection operasyonu SampleItem tipinden generic bir listeyi HTTP Get metoduna göre döndürmektedir.

Create servis operasyonu ile yeni bir SampleItem nesnesinin HTTP Post metoduna göre örneklenmesi sağlanır. Tek bir SampleItem nesne örneğinin elde edilmesi için Get isimli servis operasyonunun aşırı yüklenmiş diğer bir versiyonu kullanılmaktadır. Update servis operasyonu ile HTTP Put metoduna göre güncelleme işlemi yapılmakta olup, Delete servis operasyonuda bir SampleItem nesnesinin HTTP Delete metoduna göre silinmesini sağlamaktadır. Solution içerisinde dikkat çekici bazı noktalar bulunmaktadır;

- Örneğin web.config içeriği. Burada WCF 4.0 ile birlikte gelen basitleştirilmiş konfigurasyon (Simplified Configuration) özelliklerine yer verilmiştir. Bu sebepten daha sade, okunaklı ve fonksiyonel bir konfigurasyon içeriği oluşmuştur.
- Dikkat çekici bir diğer noktada global.asax dosyasının var olmasıdır. Aslında bu bir web uygulaması olduğu için son derece normaldir. Lakin gözden kaçırılmaması gereken bir gerçek vardır; svc uzantılı bir servis dosyası fiziki olarak yoktur. Çünkü Asp.Net 4.0 Routing özelliği kullanılmaktadır. Çok tabi olarak yönlendirme işlemleri için global.asax dosyasında yapılması gereken bazı işlemler vardır. Bu nedenle hazır olarak global.asax içeriği aşağıdaki gibi üretilmektedir.

![blg127_GlobalAsax.gif](images/blg127_GlobalAsax.gif)

- Yine servis kodlarına baktığımızda OperationContract niteliğinin kullanılmadığını görürüz. Oysaki.Net 3.5 ve WCF Rest Starter Kit sürümlerine baktığımızda WebGet ve WebInvoke nitelikleri dışında OperationContract niteliğininde kullanılması gerektiğini bilmekteyiz. OperationContract servis sözleşmelerinden sunulan operasyonların WCF çalışma zamanına bildirilmesinde rol oynadığı için bu son derece doğaldır. Ne varki WCF WebHttp servislerinde OperationContract niteliği opsiyoneldir.

Şimdi servis uygulamamız üzerinde bir kaç küçük değişiklik yapalım. Öncelikli olarak hayatımızı kolaylaştırmak adına Entity Framework'ten yararlanalım ve meşhur Chinook veritabanını ve işlemleri basit bir biçimde ele almak için sadece Artist tablosunu kullanmak istediğimizi düşünelim. Gerçi bu noktadan sonra biraz WCF Data Service'lere doğru kaymaya başlamış oluyoruz ancak amacımız tabiki HTTP Get,Post,Put ve Delete işlemlerini kendi kontrolümüz altında geliştirmek.

Not: Tam bu noktada geliştirilen uygulamanın Data Service'ten veya RIA Service'ten ne farkı kaldığı sorusu akla gelebilir. ![Wink](images/smiley-wink.gif) WCF WebHttp servislerinde asıl nokta operasyonun non-SOAP olacak şekilde sunulması (yani HTTP Get,Post,Put,Delete) ayrıca URI, format, protocol gibi bilgilerin tamamen geliştirici kontrolü altında olmasıdır.

Şimdi servis kodlarını aşağıdaki gibi geliştirdiğimizi varsayalım.

```csharp
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

namespace HelloWebHttp
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class HelloService
    {
        [WebGet(UriTemplate = "ArtistList")]
        public List<Artist> GetAllArtists()
        {
            ChinookEntities entites = new ChinookEntities();
            return entites.Artist.OrderByDescending(a => a.Name).ToList();
        }

        [WebGet(UriTemplate = "Artist/{name}")]
        public List<Artist> FindArtists(string name)
        {
            ChinookEntities entities = new ChinookEntities();

            return (from artist in entities.Artist
                    where artist.Name.Contains(name)
                    select artist).ToList();
        }

        [WebGet(UriTemplate = "Artist/InRange?idFirst={firstId}&idSecond={secondId}")]
        public List<Artist> GetArtistsInRange(int firstId, int secondId)
        {
            ChinookEntities entities = new ChinookEntities();
            return (from a in entities.Artist
                   where a.ArtistId >= firstId && a.ArtistId <= secondId
                   select a).ToList();
        }
    }
}
```

Dikkat edileceği üzere sadece HTTP Get metodlarının çalışmasına yönelik 3 örnek operasyon yer almaktadır. WebGet niteliklerinde UriTemplate özelliklerine atanan değerler yardımıyla HTTP Get taleplerinin nasıl olması gerektiği belirlenmektedir. Servise gelen taleplerin Routing sürecine dahil olması için global.asax.cs kodlarında da aşağıdaki değişiklikleri yapmamız yeterli olacaktır.

```csharp
using System;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Routing;

namespace HelloWebHttp
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
            RouteTable.Routes.Add(new ServiceRoute("Chinook", new WebServiceHostFactory(), typeof(HelloService)));
        }
    }
}
```

Dikkat edileceği üzere http://makineadı:portnumarası/Chinook üzerine gelen talepler sonrasında, WebServiceHostFactory'nin ayağa kaldırılması ve HelloService sınıfının örneklenerek işleme alınmasının sağlanması gerçekleştirilmektedir. Artık testlerimize başlayabiliriz.

> Dilerseniz Web uygulamasını IIS üzerinden host ederekte deneyebilirsiniz. Ancak ister Asp.Net Development Server ister IIS olsun, URL satırında RegisterRoutes metodunda yer alan Chinook bilgisini kullanmamız servise ulaşmamız için yeterli olacaktır.

Tabi test derken ilk etapta servis operasyonlarını nasıl çağırabileceğimizi bilemeyebiliriz. Yada bulmak için araştırmaya üşenebiliriz.

![Embarassed](images/smiley-embarassed.gif)

İşte bu amaçla WCF tarafına gelen Auto Help yetenekleri sayesinde çalışma zamanında yardım sayfasına gidebilir ve servis operasyonlarını nasıl çağırabileceğimizi, içeriklerinin ne olacağını görebiliriz.. Aynen aşağıdaki ekran görüntüsünde olduğu gibi.

![blg127_Runtime1.gif](images/blg127_Runtime1.gif)

İlk olarak belirli bir kelimeyi içeren Artist listesini elde etmek istediğimizi düşünelim. Örneğin adında Milton kelimesi geçenleri bulmak istiyoruz. Bu durumda URL satırında Chinook/Artist/Milton yazmamız yeterli olacaktır. Sonuçlar aşağıdaki ekran görüntüsünde olduğu gibidir.

![blg127_Runtime2.gif](images/blg127_Runtime2.gif)

Eğer tüm Artist listesini elde etmek istiyorsak bu durumda URL satırından Chinook/ArtistList bilgisini girmemiz yeterli olacaktır. Bu durumda elde edilen sonuçlar aşağıdaki ekran görüntüsünde olduğu gibidir.

![blg127_Runtime3.gif](images/blg127_Runtime3.gif)

Son olarak ArtistId değer aralığına göre Artist listesini elde etmek istediğimizi düşünelim. Bu durumda URL satırından Chinook/Artist/InRange?idFirst=155&idSecond=158 gibi bir URL satırı girmemiz yeterli olacaktır ki buna göre örneğin ArtistId değerleri 155 ile 158 aralığında olanların listesini aşağıdaki ekran görüntüsünde olduğu gibi elde edebiliriz.

![blg127_Runtime4.gif](images/blg127_Runtime4.gif)

Oldukça basit ve etkili değil mi? WCF WebHttp Service'ler ile ilişkili incelemelerimize devam ediyor olacağız. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[HelloWebHttp.rar (31,23 kb)](assets/HelloWebHttp.rar) [Örnek Visual Studio 2010 Ultimate Beta 2 Sürümünde geliştirilmiş ancak RC sürümü üzerinde de test edilmiştir]
