---
layout: post
title: "HTTPHandler ve HttpModule Kavramları"
date: 2006-12-08 04:00:00 +0300
categories:
  - aspnet
tags:
  - aspnet
  - csharp
  - dotnet
  - soap
  - web-service
  - http
  - iis
  - authentication
  - caching
  - visual-studio
  - asmx
---
Çoğumuz herhangibir tarayıcı penceresinden bir AspNet sayfasını çağırdığımızda resmin hep ön yüzünden bakarız. Oysaki resmin arka yüzünde dikkate değer bir mimari bulunmaktadır. Bu mimari içerisinde yer alan önemli yapılardan ikisi, Http Pipeline'ın bir parçası olan Http Handler ve Http Module kavramlarıdır. Bu makalemizde Http Handler ve Http Module kavramlarını kısaca incelemeye ve tanımaya çalışacağız. HttpHandler ve HttpModule kavramlarını derinlemesine incelemeden önce işe, bir web sayfasını talep ettiğimizde web sunucusunun bu talebi nasıl değerlendirdiğini ele almaya çalışarak başlayalım. Çalışma modelindeki başrol oyuncularımız ISS (Microsoft Internet Information Services), ASPNETISAPI, Asp.Net Work Processor ve HTTPPipeLine dır.

Temel olarak IIS, web sunucusuna gelen html, aspx, asp, jsp vb talepleri karşılayıp cevaplamakla yükümlü bir programıdır. Talep edilen sayfalar farklı tipte olduğundan yada farklı programalama sistemlerince işletildiklerinden IIS, gelen talebi asıl işletecek olan sisteme devretmek için bazı durumlarda arada bir program arayüzüne ihtiyaç duyacaktır. Bazı durumlarda diyoruz çünkü bir HTML sayfası için (yada bir resim dosyası vb...).Net Framework ve benzeri ortamlara ihtiyaç yoktur. Bunlar zaten doğrudan karşılanabilirler.

Ancak örneğin bir asp sayfasına gelen talep için asp.dll Isapi extension kullanılır. Bir Asp.Net sayfası söz konusu olduğunda ise bu extension AspNetIsapi.dll kütüphanesidir. AspNet_Isapi unmanaged (yönetimsiz) bir kütüphanedir. Dolayısıyla içerisinde.Net Framework kodları çalıştırılmaz. Bunun yerine AspNet_Isapi.dll gelen talepleri, Asp.Net Work Processor'a iletir. Asp.Net Work Processor ise, bu talepleri işetilmek üzere Http Module ve Http Handler'lara devreder. Çok basit olarak düşündüğümüzde çalışma şeklini aşağıdaki gibi düşünebiliriz.

![mk183_1.gif](/assets/images/2006/mk183_1.gif)

Burada Http Modules ve Http Handler kısmını biraz daha açıklamakta fayda var. Asp.Net Runtime gelen talepleri Http Module'ler üzerinden geçirerek ilgili HttpHandler'a devreder. İlgili HttpHandler diyoruz çünkü Asp.Net çalışma ortamına düşen her talep için ele alınabilecek ayrı ayrı HttpHandler tipleri mevcuttur. Söz gelimi web servisleri için çalışan ayrı bir HttpHandler varken web sayfaları için çalışan başka bir HttpHandler'vardır. Biz bir aspx sayfası için gelen talebi göz önüne aldığımızda ilgili HttpHandler'ın yaptığı bazı işlemler vardır.

Bu işlemler sırasında sayfanın bir örneği (nesne olarak) üzerindeki kontroller ile birlikte oluşturulur. Bir başka deyişle sayfanın yaşam döngüsü çalışır. (PreInit -> Init -> Load -> Change/ Click -> PreRender -> UnLoad -> Dispose) Nihayetinde bir HTML çıktısı üretilir. Bu HTML çıktısı Http Module'ler üzerinden geriye doğru Asp.Net Work Processor'a oradanda AspNet_Isapi'ye iletilir ve son olarak IIS üzerinden talepte bulunan istemciye gönderilir.

Aslında sistemimizde yüklü olan pek çok HttpHandler ve HttpModule tipi vardır. Bunları root klasördeki web.config dosyası içerisinde bulabiliriz. Örneğin makaleyi yazdığım sistemde ki root web.config dosyası D:\WINDOWS\Microsoft.NET\ Framework\v2.0.50727\CONFIG klasörü altında yer almaktadır. Root klasörde yer alan web.config, bu makinedeki web uygulamlarının konfigurasyon dosyalarının kalıtımsal olarak türediği dosyadır. Bu dosya içerisinde yer alan HttpHandlers ve HttpModules sekmelerine baktığımızda aşağıdakine benzer çıktılar elde ederiz.

Örnek httpHandlers elementi ve alt elementleri; (Burada çok daha fazla Handler tanımı vardır. Örnek olması açısından bir kaç Handler gösterilmektedir.)

```text
<httpHandlers>
    <add path="trace.axd" verb="*" type="System.Web.Handlers.TraceHandler" validate="true" />
    <add path="*.aspx" verb="*" type="System.Web.UI.PageHandlerFactory" validate="true" />
    <add path="*.ashx" verb="*" type="System.Web.UI.SimpleHandlerFactory" validate="true" />
    <add path="*.asmx" verb="*" type="System.Web.Services. Protocols.WebServiceHandlerFactory, System.Web.Services, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" validate="false" />
    <add path="*.rem" verb="*" type="System.Runtime.Remoting.Channels. Http.HttpRemotingHandlerFactory, System.Runtime.Remoting, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" validate="false" />
    <add path="*.soap" verb="*" type="System.Runtime.Remoting.Channels. Http.HttpRemotingHandlerFactory, System.Runtime.Remoting, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" validate="false" />
    <add path="*.asax" verb="*" type="System.Web.HttpForbiddenHandler" validate="true" />
    <add path="*.ascx" verb="*" type="System.Web.HttpForbiddenHandler" validate="true" />
    <add path="*.master" verb="*" type="System.Web.HttpForbiddenHandler" validate="true" />
    <add path="*.skin" verb="*" type="System.Web.HttpForbiddenHandler" validate="true" />
    <add path="*.sitemap" verb="*" type="System.Web.HttpForbiddenHandler" validate="true" />
    <add path="*.dll.config" verb="GET,HEAD" type="System.Web.StaticFileHandler" validate="true" />
    <add path="*.exe.config" verb="GET,HEAD" type="System.Web.StaticFileHandler" validate="true" />
    <add path="*.config" verb="*" type="System.Web.HttpForbiddenHandler" validate="true" />
    <add path="*.cs" verb="*" type="System.Web.HttpForbiddenHandler" validate="true" />
    <add path="*.csproj" verb="*" type="System.Web.HttpForbiddenHandler" validate="true" />
    <add path="*.resx" verb="*" type="System.Web.HttpForbiddenHandler" validate="true" />
    <add path="*.mdb" verb="*" type="System.Web.HttpForbiddenHandler" validate="true" />
     ...diğeleri
</httpHandlers>
```

HttpModules elementi ve alt elementlerinin içeriği;

```text
<httpModules>
    <add name="OutputCache" type="System.Web.Caching.OutputCacheModule" />
    <add name="Session" type="System.Web.SessionState.SessionStateModule" />
    <add name="WindowsAuthentication" type="System.Web.Security.WindowsAuthenticationModule" />
    <add name="FormsAuthentication" type="System.Web.Security.FormsAuthenticationModule" />
    <add name="PassportAuthentication" type="System.Web.Security.PassportAuthenticationModule" />
    <add name="RoleManager" type="System.Web.Security.RoleManagerModule" />
    <add name="UrlAuthorization" type="System.Web.Security.UrlAuthorizationModule" />
    <add name="FileAuthorization" type="System.Web.Security.FileAuthorizationModule" />
    <add name="AnonymousIdentification" type="System.Web.Security.AnonymousIdentificationModule" />
    <add name="Profile" type="System.Web.Profile.ProfileModule" />
    <add name="ErrorHandlerModule" type="System.Web.Mobile.ErrorHandlerModule, System.Web.Mobile, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" />
    <add name="ServiceModel" type="System.ServiceModel.Activation.HttpModule, System.ServiceModel, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" />
</httpModules>
```

HttpHandler sekmesine baktığımızda path kısmında bazı dosya uzantıları olduğu görürüz. Bunların çoğu web uygulamalarını tasarlayanlara tanıdık gelecektir.

Uzantı
Tipi

trace.axd
sayfalara ait Trace bilgilerinin tutulduğu dosya

aspx
Asp.Net web sayfalarımız (Web pages)

ascx
Kullanıcı web kontrolleri (Web User Controls)

cs
C# kaynak kod dosyaları (Source Files)

config
Konfigurasyon dosyaları (Configuration Files)

asmx
Web servisi dosyaları (Web Service Files)

csproj
C# proje dosyaları (C# Projects Files)

skin
Temalarda kullanılan skin dosyaları (Skin Files)

master
Master Page dosyaları (Master Pages)

Bu elementlerin her biri için birde verb niteliği (attribute) tanımlanmıştır. Verb niteliği Http protokolüne göre bu elementler içerisinde belirtilen uzanıtıya sahip kaynaklara gelebilecek olan talep çeşitlerini sınırlamak için kullanılır. Örneğin bu niteliğe Get, Head, Post gibi değerler bir arada yada ayrı ayrı verilebilir. * olması halinde tüm Http isteklerinin geçerli olacağı belirtilmektedir. HttpHandler içerisindeki elementler içinde belkide en önemli kısım type niteliğinin işaret ettiği.Net tipidir. Şimdi şu senaryoyu düşünelim. Normal şartlar altında url üzerinden bir cs dosyasını talep ettiğimizde aşağıdakine benzer bir hata alırız. (Testi IIS üzerinden yapabilmek için bilerek cs uzantılı dosya ilgili virtual directory altına atılmıştır. Aspx dosyası ile cs dosyalarının IIS altına atılması, Vs.Net 2005 Copy Web Site dağıtım modelinin varsayılan çalışma şeklidir.)

![mk183_2.gif](/assets/images/2006/mk183_2.gif)

Böyle bir mesaj almamızın nedeni, cs uzantılı dosyalara gelecek olan çağrıların HttpForbiddenHandler tipi tarafından ele alınıyor olmasıdır. Bu handler hata mesajından görebileceğimiz gibi ilgili dosyaya erişilmesini engellemektedir. Bu nedenle HttpForbiddenHandler tarafından ele alınabilen tüm dosya tipleri için yukarıdaki hata mesajını alırız. Tam aksine aspx uzantılı sayfalar PageHandlerFactory tipi tarafından ele alınır.

Ancak PageHandlerFactory aslında bir Http Handler değildir. Bunun yerine çalışma zamanında gerekli olan HttpHandler'ın üretilmesini ve bunu bir arayüz olarak (IHttpHandlerFactory) döndürülmesini sağlayan fonksiyonelliği içerir. Dolayısıyla aspx uzantılı sayfalar için bu fabrika tipinin ürettiği handler sunucu taraflı derleme ve sayfa nesne örneği üretme işlemlerini üstlenecektir. PageHandlerFactory aslında.Net 2.0 ile gelen ve Asp.Net 2.0 çalışma modelini destekleyen bir tiptir. Aşağıda bu sınıfın türediği IHttpHandlerFactory arayüzü üyeleri ile birlikte görülmektedir. Dikkat ederseniz GetHandler isimli üye metod, geriye IHttpHandlerFactory tipinden bir arayüz referansı döndürmektedir ki bu referans çalışma zamanında gereki olan HttpHandler tipini taşıyacaktır.

![mk183_3.gif](/assets/images/2006/mk183_3.gif)

Benzer şekilde örneğin web servislerine gelicek olan talepleride (asmx sayfalarına gelicek olan talepleri) WebServiceHandlerFactory tipi üstlenecektir. Bu Handlerın sahip olduğu fonksiyonellikler arasında gelen taleplerdeki Soap mesajlarını deserialize etmek, cevap (response) olarakta tekrar serileştirilmiş Soap paketkerini hazırlayıp karşı tarafa göndermek gibi görevler sayılabilir.

HttpModules kısmına baktığımızda ise yine web uygulamalarımızda sıkça kullandığımız bazı kavramların var olduğunu görebiliriz. Örneğin Cache (ara belleğe alma), Session, Authentication, Role yönetimi vb. Buradan şu sonuç çıkartılabilir. HttpModules içerisinde tanımlanan elementlerde belirtilen tipler bir web uygulaması için ele alabileceğimiz framework özelliklerini kullanabilmemizi sağlar. Örneğin caching sistemini OutputCacheModule, Session sistemini SessionStateModule, Windows tabanlı doğrulama sistemini WindowsAuthenticationModule, Form tabanlı doğrulama sistemini FormsAuthenticationModule vb... tipleri ele almaktadır.

O halde bu noktada HttpHandler ve HttpModule sekmesindeki elementler arasındaki ilişkiyi daha kolay açıklayabiliriz. Bir sayfa talebi HttpHandler'a ulaşmadan önce HttpModule'lerden geçer. HttpHandler gerekli Html çıktısını ürettiğinde ise sonuçlar yine HttpModule'ler üzerinden Asp.Net çalışma zamanı motoruna iletilir. Böylece bir sayfa için gerekli olan ara belleğe alma, bellekten Html çıktısına dahil etme, session bilgisini oluşturma veya okuma, güvenlik doğrulamalarını yapma gibi işlevsellikler ilgili HttpModule'ler tarafından hem taleplerde (request) hemde cevaplarda (response), ele alınabilirler.

![mk183_4.gif](/assets/images/2006/mk183_4.gif)

Çok düşük bir ihtimallde olsa bazen kendi HttpHandler yada HttpModule'lerimizi yazmak isteyebiliriz. Örneğin istemciden özel bir dosya uzantısı ve parametre ile gelecek olan bir resim isteğini, veritabanından okuyup istemcilere html çıktısı olarak gönderecek bir handler, yada bir cs dosyasına gelecek talep sonrası bu talebi istemciye metin formatında döndürecek olan bir handler göz önüne alınabilir. Makalemizin bundan sonraki kısımlarında kendi HttpHandler'larımızı ve hatta HttpModule'lerimizi nasıl yazabileceğimizi incelemeye çalışacağız.

Her ne kadar kendi HttpHandler veya HttpModule tiplerimizi oluşturmak için düşünülebilecek senaryo sayısı az olsada amacımız temel olarak bunları nasıl yazabileceğimizi ve Asp.Net çalışma ortamı tarafından nasıl ele alınabileceğini incelemektir. Kendi HttpHandler tiplerimizi oluşturabilmek için ilk olarak IHttpHandler arayüzünü (interface) implemente edecek olan bir sınıf yazmamız gerekmektedir. İlk olarak bu ve benzeri HttpHandler yada HttpModule'leri içerisinde barındıracak bir Class Library projesi oluşturalım. Böylece bu HttpHandler veya HttpModule tiplerimizi başka web uygulamaları içinde kullanabiliriz. (Hatta bunu GAC (Global Assembly Cache) içerisine atarsak her web uygulamasının ortaklaşa erişebileceği bir dll halinede getirmiş oluruz.) Class Library projesinde kullanacağımız HttpHandler veya HttpModule tiplerimiz içerisinden güncel web içeriklerine (Http Context) erişebilmek için System.Web referansını açıkça eklememiz gerekmektedir.

![mk183_5.gif](/assets/images/2006/mk183_5.gif)

Şimdi bu sınıf kütüphanesi içerisine IHttpHandler arayüzünü uygulayan bir sınıf dahil edelim.

![mk183_6.gif](/assets/images/2006/mk183_6.gif)

IHttpHandler arayüzü, MyCustomHandler isimli sınıfımız içerisine iki üye dahil eder. Bunlardan ProcessRequest isimli metod, gelen talepleri değerlendirebileceğimiz üyedir. Yani talebe göre Html içeriğini oluşturabileceğimiz bir başka deyişle http isteğini kendi istediğimiz şekilde ele alabileceğimi yerdir. Dikkat ederseniz bu metod parametre olarak HttpContext tipinden bir değişken almaktadır. Bu değişken sayesinde, Response, Request ve Server nesnelerine erişebiliriz. Bu da gelen talepler HttpHandler içerisinde değerlendirebileceğimiz anlamına gelir. IsReusable özelliği ise sadece okunabilir bir özelliktir ve ilgili HttpHandler nesne örneğine ait referansın başka talepler içinde kullanılıp kullanılmayacağını belirler. Şimdi kodumuzu aşağıda görüldüğü gibi biraz daha geliştirelim.

```csharp
using System;
using System.Web;

namespace MyHandlers
{
    public class MyCustomHandler:IHttpHandler
    {
        #region IHttpHandler Members
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            string isim = context.Request["Ad"];
            context.Response.Write("<html><body>");
            context.Response.Write("<b> Adım : " + isim + "</b><br/>");
            context.Response.Write("</body></html>");
        }
        #endregion
    }
}
```

Dikkat ederseniz ProcessRequest metodu içerisinde Request üzerinden gelecek olan Ad isimli bir parametreyi alıyoruz ve basit olarak bir Html çıktısı üretiyoruz. Üretilen Html çıktısı için Response nesnesini kullanmaktayız. Yazmış olduğumuz bu HttpHandler türünün bir web uygulamasında, örneğin mypx uzantılı dosyaları ele almasını istediğimizi düşünelim. Öncelikle geliştirdiğimiz sınıf kütüphanesini web projemize ekleyelim. İlk aşamada dosya tabanlı (file based) bir web sitesi geliştireceğiz.

![mk183_7.gif](/assets/images/2006/mk183_7.gif)

Bu nedenle ilk olarak web.config dosyası içerisinde mypx uzantılı dosyaların az önce yazdığımız sınıf kütüphanesi (class library) içerisindeki MyCustomerHandler tipi tarafından ele alınacağını belirtmemiz gerekiyor. Dolayısıyla web.config içerisinde var olan HttpHandler sekmesindeki elementler arasına yeni bir tanesini eklememiz gerekmektedir.

```text
<system.web>
    <httpHandlers>
        <add path="*.mypx" verb="*" type="MyHandlers.MyCustomHandler,MyHandlers" validate="true"/>
    </httpHandlers>
```

Böylece mypx uzantılı herhangibir isteği MyCustomHandler isimli sınıfa ait nesne örneği karşılayacaktır. Burada path kısmına *.mypx yazdık. Böylece mypx uzantılı herhangibir dosyaya gelecek olan talebi, type parametresi ile belirtilen sınıfa devretmiş oluyoruz. Durumu aşağıdaki görüntüde görüldüğü gibi test edebiliriz.

![mk183_8.gif](/assets/images/2006/mk183_8.gif)

Dikkat ederseniz web sitemizde sayfam.mypx isimli bir dosya bulunmamktadır. Ancak bu dosyaya Ad isimli parametreyide içeren bir talep geldiğinde, MyCustomHandler tarafından karşılanmaktadır. Sonuç olarak MyCustomHandler'a ait ProcessRequest metodu çağırılmış ve buna göre bir Html çıktısı üretilmiştir. Örneğimizi doğrudan visual studio.net 2005 içerisinde F5 ile çalıştıramayacağımızı farketmişsinizdir. Nitekim elimizde sayfam.mypx dosyası zaten yok. Dolayısıyla testimizi yaparken tarayıcı üzerinden manuel olarak Url girmemiz gerekmektedir.

Dikkat edilmesi gereken bir nokta geliştirilen bu web sitesi IIS altına dağıtıldığında ne olacağıdır. Nitekim IIS'e bir şekilde mypx uzantılı dosyalara gelecek olan taleplerin Asp.Net Çalışma ortamına devredilmesi gerektiğini söylememiz gerekmektedir. IIS'e mypx uzantılı dosyaları tanıtmadan önce, bunu bildirmediğimizde ne olacağına bakmamızda fayda var. Bunun için web sitemizi IIS altında yayımladığımızı düşünelim. Örneğin MySite ismiyle yayınladığımızı farz edelim. Bu durumda sayfam.mypx?Ad=burak isimli bir talepte bulunduğumuzda aşağıdaki ekran görüntüsünde yer alan sonucu elde ederiz.

![mk183_9.gif](/assets/images/2006/mk183_9.gif)

Bu hatanın sebebi IIS'in mypx isimli uzantılı dosyaları ne yapacağını bilememesidir. Bunun için IIS üzerinde mypx isimli dosyayı tanıtmamız gerekmektedir. Bir başka deyişle, mypx uzantılı dosyalar için gelecek olan taleplerin AspNet_Isapi.dll'e devredilmesini söylemeliyiz. Bu amaçla, IIS üzerinden MySite isimli virtual directory'nin özelliklerine gidelim ve Directory kısmından Configuration sekmesini açalım.

![mk183_10.gif](/assets/images/2006/mk183_10.gif)

Dikkat ederseniz burada siteye gelebilecek talepler ve bunları değerlendirecek olan programlar ile ilgili eşleştirmeler yapılmaktadır. (Mappings) Bizim tek yapmamız gereken aşağıdaki gibi yeni bir eşleştirme eklemektir.

![mk183_11.gif](/assets/images/2006/mk183_11.gif)

Dikkat ederseniz executable kısmında aspnet_isapi.dll'ini belirtiyoruz. Böylece extension kısmında belirtilen mypx isimli dosyalar için gelicek olan talepler, aspnet_isapi.dll tarafından ele alınabilecek. Buradan da tahmin edeceğiniz üzere kendi yazdığımız HttpHandler'a kadar gelicek. Check that file exists (dosyanın var olup olamadığını kontrol et) seçeneğini kaldırmamızın nedeni ise olmayan bir mypx sayfasına gelecek olan taleperi IIS'in geri çevirmesini engellemektir. (Bu ayarlamalar IIS 5.1 üzerinde yapılmış olup IIS 6.0 için bazı farklılıklar olabilir.) Artık talebimiz çalışacaktır. Örneğin bu kez mypage.mypx?Ad=Selim talebinde bulunduğumuzu düşünelim. Aşağıdaki sonucu elde ederiz.

![mk183_12.gif](/assets/images/2006/mk183_12.gif)

Gelelim kendi HttpModule'lerimizi nasıl yazabileceğimize. Yazımızın başında bahsettiğimiz gibi HttpModule'ler ile Http Handler'lar arasında yakın bir ilişki vardır. IIS tarafından gelen her istek ilgili HttpHandler'a iletilmeden önce bazı HttpModule'ler üzerinden geçer. HttpHandler tarafından üretilen Html çıktıları ise aynı module'ler üzerinden IIS'e doğru gönderilirler. HttpModule'ler çoğunlukla olay tabanlı fonksiyonellikler içerir.

Örneğin bir kullanıcının doğrulanması sırasında veya sonrasında çalışacak olaylar WindowsAuthenticationModule tarafından kontrol altına alınır. Biz kendi HttpModule'lerimizi oluşturduğumuzda gelen taleplerde veya giden cevaplarda ele alınabilecek module olaylarını özelliştirme şansına sahip olabiliriz. Kendi HttpModule nesnelerimizi yazabilmek için IHttpModule arayüzünden türetilmiş bir sınıf yazmamız gerekmektedir. Dilerseniz makalemizde kullandığımız MyHandlers isimli sınıf kütüphanesi içerisine MyCustomHandler isimli bir tipi aşağıdaki gibi ekleyelim.

```csharp
using System;
using System.Web;

namespace MyHandlers
{
    class MyCustomModule:IHttpModule
    {
        #region IHttpModule Members
        public void Dispose()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Init(HttpApplication context)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion
    }
}
```

IHttpHandler, uygulandığı tipe iki metod dahil eder. Init ve Dispose. Init metodu HttpApplication tipinden bir parametre almaktadır ki bu parametre sayesinde var olan HttpModule olaylarına müdahale etme, aktif Http içeriğine ulaşma gibi imkanlara sahip olabiliriz. Dispose metodunu ise, bu sınıfa ait nesne örneği yok edilmeden önce yapmak istediğimiz kaynak temizleme işlemleri için kullanabiliriz. Örneğin Module içerisinden kullanılan unmanaged (managed) kaynakların serbest bırakılması için ele alabiliriz. Şimdi modulümüz içerisine biraz kod yazalım ve sonuçlarını incelemeye çalışalım.

```csharp
using System;
using System.Web;

namespace MyHandlers
{
    class MyCustomModule:IHttpModule
    {
        HttpContext m_Ctx=null;

        public void Init(HttpApplication context)
        {
            m_Ctx = context.Context; 
            context.PreSendRequestContent+= new EventHandler(context_PreSendRequestContent);
        }

        void context_PreSendRequestContent(object sender, EventArgs e)
        {
            m_Ctx.Response.Write("<!--Bu sayfa Z şirketi tarafından üretilmiştir...-->");
        }

        public void Dispose()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
```

Bakın burada uygulama için PreSendRequestContent isimli bir olay yüklenmiştir. Bu olay, HttpHandler tarafından üretilen HTML içeriği gönderilmeden önce çalışır. Böylece bu modülü kullanan bir uygulama içerisindeki her hangibir sayfa talebinde gönderilen Http içeriğine "Bu sayfa Z şirketi tarafından üretilmiştir..." cümlesi bir yorum takısı (comment tag) olarak eklenecektir. Yazdığımız HttpModullerin ilgili web uygulaması içerisinde geçerli olmasını sağlamak için yine web.config dosyasında düzenleme yapmamız gerekmektedir. Bu amaçla, web.config dosyasına aşağıdaki gibi httpModules sekmesini dahil etmemiz ve yeni HttpModule'ümüzü bildirmemiz yeterli olacaktır.

```text
<httpModules>
    <add name="MyModule" type="MyHandlers.MyCustomModule,MyHandlers"/>
</httpModules>
```

Artık bu konfigurasyon ayarlarına sahip bir web uygulamasına gelecek her talep sonrasında, üretilecek olan Html içerikleri için yazmış olduğumuz modül devreye girecek ve ilgili olay metodu çalışacaktır. Bunu test etmek amacıyla herhangibir aspx sayfasını web uygulamamıza dahil edelim ve çalıştıralım. Elde edilen sayfanın içeriğine tarayıcı penceresinden baktığımızda aşağıdakine benzer bir çıktı elde ederiz.

![mk183_13.gif](/assets/images/2006/mk183_13.gif)

Dikkat ederseniz, içeriğe bir yorum satırı eklenmiştir. Bu bilgi mesajı aslında web uygulamamıza gelecek her sayfa talebinde geçerli olacaktır. Init metodu içerisinde yer alan HttpApplication nesne örneği üzerinden yazabileceğimiz pek çok olay metodu vardır. Bunlardan bir kaçı ve ne işe yaradıkları aşağıdaki tabloda listelenmiştir.

Olay (Event)
İşlevi

BeginRequest
Bir talep geldiğinde tetiklenir.

EndRequest
Cevap istemciye gönderilmeden hemen önce tetiklenir.

PreSendRequestHeaders
İstemciye HTTP Header gönderilmeden hemen önce tetiklenir.

PreSendRequestContent
İstemciye içerik gönderilmeden hemen önce tetiklenir.

AcquireRequestState
Session gibi durum nesneleri (state objects) elde edilmeye hazır hale geldiğinde tetiklenir.

AuthenticateRequest
Kullanıcı doğrulanmaya hazır hale geldiğinde tetiklenir.

AuthorizeRequest
Kullanıcının yetkileri kontrol edilmeye hazır hale geldiğinde tetiklenir.

Bu olaylar dışında ele alabileceğimiz başka olaylarda vardır. Örneğin var olan diğer HttpModule'lerin içeriklerine erişmek ve kullanmak isteyebiliriz. Söz gelimi, WindowsAuthenticationModule'ü ele almak istersek aşağıdaki gibi bir kod parçası geliştirebiliriz.

```csharp
using System;
using System.Web;
using System.Web.Security;

namespace MyHandlers
{
    class MyCustomModule:IHttpModule
    {
        HttpContext m_Ctx=null;
        WindowsAuthenticationModule authMod = null;

        public void Init(HttpApplication context)
        {
            m_Ctx = context.Context;

            authMod = (WindowsAuthenticationModule)context.Modules["WindowsAuthentication"];
            authMod.Authenticate += new WindowsAuthenticationEventHandler(authMod_Authenticate);
        }

        void authMod_Authenticate(object sender, WindowsAuthenticationEventArgs e)
        {
            m_Ctx.Response.Write("<!--" + e.User.Identity.Name + "-->");
        }

        public void Dispose()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
```

Dikkat ederseniz, context nesnesi üzerinden Modules isimli bir koleksiyona erişebilmekteyiz. Bu koleksiyon içerisinde var olan tüm HttpModule'ler yer almaktadır. Bunu debug mode'da iken görebiliriz.

![mk183_14.gif](/assets/images/2006/mk183_14.gif)

Dikkat ederseniz kendi geliştirdiğimiz MyCustomModule isimli nesnemiz, web.config dosyasındaki ismiyle (name niteliğinde yazdığımız MyModule) bu koleksiyon içerisinde görülmektedir. Kendi HttpModule'lerimizi yazmak için gerekli senaryoları düşündüğümüzde aklıma çok fazla vakka gelmeyebilir. Ama şu örnekler fikir vermesi açısından ve denenmesi açısından yararlı olabilir. Bir web uygulamasına gelen her hangibir talepte authenticate olan kullanıcıların log dosyalarına kaydedilmesi ele alacak kendi HttpModule sınıfımızı yazabiliriz. Talep edilen url bilgisi çok uzun olduğunda bunu ayrıştırıp (parse) başka ve daha kısa bir url olacak şekilde kullanıcıya gösterebilecek bir olay metodunu barındıracak bir HttpModule yazabiliriz vb...

Bu makalemizde kısaca HttpHandler ve HttpModule kavramlarına değinmeye çalıştık. Her ne kadar kendi Handler yada Module'lerimizi yazmayı pek tercih etmesekte, bazı durumlarda Asp.Net çalışma ortamını alt seviyede özelleştirmek isteyebiliriz. Bu gibi durumlarda IHttpHandler ve IHttpModule arayüzlerinden türeteceğimiz tiplerden yararlanmamız gerekmektedir. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.