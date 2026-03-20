---
layout: post
title: "WCF WebHttp Services - Server Bazlı Cache"
date: 2010-03-29 15:05:00 +0300
categories:
  - wcf-eco-system
  - wcf-webhttp-services
tags:
  - wcf-eco-system
  - wcf-webhttp-services
  - csharp
  - xml
  - dotnet
  - aspnet
  - linq
  - wcf
  - rest
  - performance
  - caching
  - generics
  - visual-studio
  - rc
---
Şirketimi çok seviyorum. Nazar değmesin ama araştırma yapmam, yeni bir şeyler öğrenmem ve bunları ekip arkadaşlarımla paylaşmam için beni özellikle teşvik eden bir şirkette bulunmaktayım. Çalıştığım şirketin en güzel özelliklerinden biriside, Cuma günleri yapılan minik ikramları.

![blg143_Giris.jpg](/assets/images/2010/blg143_Giris.jpg)

![Wink](/assets/images/2010/smiley-wink.gif)

Her cuma değişik bir yiyecek ile karşılaşıyoruz. Geçtiğimiz Cuma'lardan birisinde ise yanda çektiğim resimde görülen gülen kurabiyelerimiz vardı. E böylesine güler yüzlü kurabiyeler ile gerekli glikozu aldıktan sonra içimden hemen eve gitmek gelmedi. Bunun yerine mesai sonrasında çalışma masamda oturup, etrafın sakinleşmesi ve sessizliğin artması ile birlikte bloğuma bir şeyler yazmaya karar verdim. Bir süredir WCF Eco System'in parçaları üzerinde yazmakta olduğum bir seri bulunmaktaydı. Bunu devam ettirmek ile Cuma gecesini güzelce tamamlayabileceğimi düşündüm. İşte bu günkü konumuz...WCF WebHttp Service'lerinde ön bellekleme (Output Caching)

WCF WebHttp Service'leri bildiğiniz üzere Web ortamı üzerinden sunulan hizmetlerdir. Bu sebepten Web tarafının sunucu ve istemci bazlı bazı yeteneklerini kullanabilirler. Örneğin Asp.Net Compatibility Mode ile çalıştırıldıklarında Asp.Net dünyasının Output Cache yeteneğine sahip olurlar. Bildiğiniz üzere Output Cache mekanizması sayesinde Web içeriklerine gelen taleplerin ön bellekten karşılanması ve bu sayede arka planda ilgili HTML çıktılarının üretilmesi için gerekli işlemlerin otomatikman atlanılması sağlanabilmektedir. Bu, özellikle üretim maliyeti yüksek olan ama belirli bir süre zarfı içerisinde değişmeyen sayfa içeriklerinin üretiminde oldukça performans arttırıcı bir tekniktir. Madem Web tarafında böyle bir yeteneğimiz bulunmaktadır, o halde neden bu kabiliyeti WCF Servislerinde de kullanamayalım? İşte bu yazımızda Asp.Net tarafında hazır olan bu alt yapının WebHttp Service'lerinde nasıl kullanıldığını incelemeye çalışıyor olacağız. İlk olarak konuyu sunucu bazlı ön bellekleme (Server Side Caching) olarak değerlendireceğiz. Serinin sonraki bölümünde ise istemci taraflı ön belleklemeyi ele alacağız. Dilerseniz vakit kaybetmeden kodlamaya başlayalım. Öncelikli olarak aşağıdaki WebHttp Service içeriğine sahip bir WCF REST Service Application geliştirdiğimizi düşünelim.

```csharp
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;

namespace Lesson6
{   
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class EinsteinService
    {
        // GetCategories isimli metodun çıktısının ön bellekleneceği AspNetCacheProfile niteliği ile belirtilir. Nitelikte parametre olarak kullanılan string bilgi ile web.config dosyasında bu metod için nasıl bir ön bellekleme yapılacağı set edilir. Ne kadar süre ile tutulacağı, parametre bazlı olup olmayacağı gibi...
        [AspNetCacheProfile("CategoriesCache")]
        [WebGet(UriTemplate = "Categories")]
        public List<string> GetCategories()
        {            
            string[] categories=File.ReadAllLines(HttpContext.Current.Server.MapPath("~/Kategoriler.txt"));
            return new List<string>(categories);
        }

        [AspNetCacheProfile("CategoriesByFirstLetterCache")]
        [WebGet(UriTemplate = "Categories/{firstLetter}")]
        public List<string> GetCategoriesByFirstLetter(string firstLetter)
        {
            string[] categories = File.ReadAllLines(HttpContext.Current.Server.MapPath("~/Kategoriler.txt"));
            return (from category in categories
                         where category.StartsWith(firstLetter,true,null)
                         select category).ToList();
        }
    }
}
```

Örneğimizde yer alan servisimizde iki operasyon yer almaktadır. Her iki operasyonda aşağıda örnek çıktısı olan Kategoriler.txt isimli text tabanlı dosyayı kullanmaktadır.

![blg143_TextContent.gif](/assets/images/2010/blg143_TextContent.gif)

GetCategories operasyonu text dosyası içerisindeki tüm satırları string tipinden generic List koleksiyonu olarak geriye döndürmektedir. GetCategoriesByFirstLetter operasyonu ise aynı çıktıyı baş harflere göre üretmektedir. Bizim odaklanmamız gereken nokta ise her iki operasyon başında uygulanan AspNetCacheProfile niteliğidir (Attribute). Bu niteliklerin uygulanması ile söz konusu operasyonların çıktılarının ön bellekleneceği, çalışma zamanı ortamına bildirilmektedir. Her iki nitelikte birbirlerinden benzersiz olan takma adlar (Alias) ile işaret edilmektedir.

Peki bu isimleri nerede değerlendireceğiz? Bu sorunun cevabı Web.config dosyasında yer alan Asp.Net Output Cache ayarlarında gizlidir...

![Wink](/assets/images/2010/smiley-wink.gif)

Buradaki ayarlar ile hangi operasyon için nasıl bir ön bellekleme işleminin uygulanacağını belirtebiliriz. Örneğin operasyonların sonuçlarının ne kadar süreyle ön bellekte tutulacaklarını farklılaştırabiliriz. Yada parametre bazlı olanları...Örneğin GetCategoriesByFirstLetter operasyonunun çalışma zamanı HTML çıktılarının firstLetter bilgisine göre ön belleklenebileceğini belirtebiliriz. Tüm bu ayarlamalar için web.config dosyasına aşağıdaki eklemeleri yapmamız yeterlidir.

```xml
<?xml version="1.0"?>
<configuration>  
  <system.web>
    <compilation debug="true" targetFramework="4.0" />
    <caching>
      <outputCache enableOutputCache="true"/>
      <outputCacheSettings>
        <outputCacheProfiles>
          <add name="CategoriesCache" duration="120" location="Server" varyByParam="none" varyByHeader="Accept"/>
          <add name="CategoriesByFirstLetterCache" duration="300" location="Server" varyByParam="firstLetter" varyByHeader="Accept"/>
        </outputCacheProfiles>
      </outputCacheSettings>
    </caching>
  </system.web>
  <system.webServer>
    <modules runAllManagedModulesForAllRequests="true">
      <add name="UrlRoutingModule" type="System.Web.Routing.UrlRoutingModule, System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" />
    </modules>
  </system.webServer>

  <system.serviceModel>
    <!--Asp.Net Output Caching alt yapısını ve yeteneklerini kullanmak istediğimiz için, AspNet Compatibility modun açık olması önemlidir.-->
    <serviceHostingEnvironment aspNetCompatibilityEnabled="true"/>
    <standardEndpoints>
      <webHttpEndpoint>
        <!-- 
            Configure the WCF REST service base address via the global.asax.cs file and the default endpoint 
            via the attributes on the <standardEndpoint> element below
        -->
        <standardEndpoint name="" helpEnabled="true" automaticFormatSelectionEnabled="true"/>
      </webHttpEndpoint>
    </standardEndpoints>
  </system.serviceModel>
</configuration>
```

Buradaki ayarlamalara göre CategoriesCache adı ile belirlenen operasyonlar 120 saniye süreyle ön bellekten getirilecektir. Ön bellekleme ortamının sunucu taraflı olduğu location niteliği ile belirtilmektedir. Bu ön bellek ayarında parametre kullanılmadığı (varyByParam="none") ve talebin (request) Header kısmına göre Accept tipinde olanların göz önüne alındığı ifade edilmektedir. Diğer yandan CategoriesByFirstLetterCache bildirimine bakıldığında sürenin 300 saniye olduğu ama bir öncekinden farklı olarak firstLetter parametresine göre ön bellekte ayrı ayrı görüntüler tutulacağı belirtilmektedir. Buna göre Categories/A gibi bir talep ile Categories/s gibi bir taleb için üretilen HTML çıktıları ön bellekte ayrı ayrı tutulacaktır. Tabi birden fazla parametrenin değerlendirilmesi gerektiği durumlarda; işareti kullanılarak bildirilmeleri gerekmektedir. Örneğin varyByParam="firstLetter;productCount" vb.

Tabi yapımış olduğumuz bu anlatımın sonuçlarını test ederek görmemiz gerekiyor. Burada debug modu kullanarak talepler sırasında operasyon metodlarının içerisine ne zaman girilip ne zaman girilmediğini tespit ederek gerekli kontrolleri yapabiliriz. İşte size örnek test senaryoları;

1 - Önce URL üzerinden Categories talebini gönderin. Bu durumda tüm kategorilerin aşağıdaki şekilde olduğu gibi geldiğini göreceksiniz.

![blg143_FirstRun.gif](/assets/images/2010/blg143_FirstRun.gif)

Şimdi 120 saniyelik süre dolmadan Kategoriler.txt üzerinde bir değişiklik yapın. Örneğin Mücehver isimli yeni bir kategori ekleyin veya bir kaç kategorinin ismini değiştirin yada silin. 120 saniyelik zaman dilimi içerisinde yeniden Categories talebinden bulunursanız yapmış olduğunuz değişiklilerin tarayıcıya getirilmediğini görebilirsiniz. Ancak 120 saniyelik ön bellek süresi dolduktan sonra değişiklikleri görebileceksiniz ve hatta Debug moddaysanız ilgili operasyon kodu içerisine tekrardan girildiğini fark edeceksiniz. Bu zaten ön belleklemenin çalıştığının ispatıdır.

2 - İkinci olarak parametre bazlı ön bellekleme yapıldığını test edebilirsiniz. Bu amaçla tarayıcı üzerinden örneğin Categories/A ve Categories/M taleplerinde bulunun. Debug modda ilerlerseniz testinizi daha başarılı bir şekilde yapabilirsiniz. Özellikle 300 saniyelik ön bellekleme süreleri dolmadan farklı harfler ile denemeler yaptığınızda, ön belleklenenler için operasyon koduna girilmediğini ama daha önceden talep edilmeyenler veya 300 saniyelik ön bellekte kalma süresini dolduranlar için kodun tekrar çalıştırıldığını gözlemleyebilirsiniz.

Bu sayede geliştireceğimiz WebHttp Service'lerin operasyonlarının hızlı sonuçlar üreterek daha performanslı ve verimli olmasını sağlayabiliriz. Bu yazımızdaki örneğimizde sunucu tarafında ön bellekleme işlemlerini gerçekleştirdik. Ancak birde istemci taraflı ön bellekleme (Client Based Caching) işlemlerinin söz konusu olduğunu belirtelim. Bunu serinin sonraki yazısında incelemeye çalışacağız. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Lesson6_RC.rar (20,41 kb)](/assets/files/2010/Lesson6_RC.rar) [Örnek Visual Studio 2010 Ultimate Beta 2 Sürümünde geliştirilmiş ancak RC sürümü üzerinde de test edilmiştir]
