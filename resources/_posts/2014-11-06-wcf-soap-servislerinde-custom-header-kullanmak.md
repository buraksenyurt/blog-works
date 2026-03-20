---
layout: post
title: "WCF–SOAP Servislerinde Custom Header Kullanmak"
date: 2014-11-06 12:00:00 +0300
categories:
  - wcf
tags:
  - wcf
  - csharp
  - xml
  - dotnet
  - soap
  - web-service
  - http
  - generics
---
Çok uzun zamandır Sosyal ağlardan uzağım. Ne Facebook, ne Twitter ne de bir başkasını kullanmıyorum. Google+ üzerinde bile sırf Hangout söyleşileri nedeniyle mecburen açık tuttuğum bir hesap var. Sosyal ağları terk ettiğim ilk günlerde çevremdeki pek çok yakın dostumla olan iletişimimi de yeniden tesis etmem gerekmişti. Ne acıdır ki pek çoğunun iletişim bilgisi sadece Tweet ve Facebook mesaj kısmı ile sınırlı kalmıştı. Hani nerede telefon numaraları, nerede e-posta adresleri? Çoğunu tedarik etmek epeyce zor oldu.

[![OldMail](/assets/images/2014/OldMail_thumb.jpg)](/assets/images/2014/OldMail.jpg)


O vakitlerde yine yakın arkadaşlarımdan birisi ile işte bu sosyal ağ mevzularını ve insanlar üzerindeki olumsuz etkilerini konuşmaya başlamıştık. Bir arkadaşımla uzun zamandır e-posta üzerinden böylesine yoğun iletişim kurmamıştım. Heyecan verici ve sürükleyici bir deneyim idi. Şirkette uçuşan e-posta trafiğinden çok daha farklıydı. Duygusal anlamı vardı. Suni, bayağı değildi. İletişim kurmak isteyen iki arkadaşın yazışmalarıydı.

Derken acaba eski günlerdeki gibi postaneden mektup göndererek iletişim kursak nasıl olur diye düşünmeye başladık. İlk okul sıralarına kadar gittik. Yurt dışında mektup arkadaşı bulmaya çalıştığımız günleri hatırladık. Şimdi tabi benim bir şekilde bu mektup meselesinden makalenin konusu olan SOAP Header’ a inmem gerekiyor…Bir deneyelim.

Bazen kurum içerisinde kullanılan SOAP (Simpe Object Access Protocol) bazlı servisler istemcilerin kimliklerine göre operasyonlarını farklılaştırmak isterler. Böyle bir durumda istemcilerin kendilerini servis tarafına bir şekilde tanıtmaları ve özelleştirilmiş operasyon için gerekli bilgileri iletmeleri beklenir. En bilinen yollardan birisi de SOAP zarfının (Hah işte mektup meselesi ile bağlayabildiğimiz nokta) Header kısmına bu tip bilgileri ilave etmektir.

XML Web Service zamanlarından beri var olan bu yaklaşım, WCF tabanlı SOAP servisleri için de geçerlidir. Pek tabi Header bilgisi network trafiğini dinleyenler tarafından yakalansa da sorun teşkil etmeyecek vakalarda ele alınması daha uygundur.

SOAP Hakkında Kısa Bilgi

SOAP, uygulamalar arası mesajlaşmayı tanımlayan iletişim protokollerinden birisidir. W3C tarafından tavsiye edilen, XML tabanlı olan, genişletilebilen bir yapıya sahiptir. Dilden ve platformdan bağımsız olması onu servis dünyasında popüler hale getiren etkenler arasında yer alır. HTTP üzerinde koşan bu standart yıllarca internet/intranet tabanlı uygulamaların birbirleriyle haberleşmesi noktasında ön plana çıkmıştır. Kurumsal çözümler, miras sistemler ile olan iletişim biçimlerinde halen daha popülerliğini korumaktadır. SOAP standardının tanımladığı mesaj içeriği temel olarak aşağıdaki şekilde yer alan şema yapısına sahiptir.

[![wcfch_5](/assets/images/2014/wcfch_5_thumb.png)](/assets/images/2014/wcfch_5.png)

SOAP Header içeriklerinin istemci tarafında oluşturulması ve servis tarafında ele alınması son derece basittir. Kod yoluyla veya konfigurasyon dosyası içerisinden doğrudan bildirilerek kullanılabilir. Nasıl mı? Haydi gelin birlikte inceleyelim.

Senaryo

İstemcilerin, Header bilgisinde gönderdikleri kullanıcı adına göre kurum network’ ü içerisinde yapabilecekleri aksiyonları taşıyan rolleri yükleyecek operasyonelliği sunan bir servise ihtiyacımız olduğunu düşünelim. Burada söz konusu olan operasyonu bir WCF servisi içerisinde konuşlandırmayı planlıyoruz. Buna göre istemcilerin servisten talepte bulunmadan önce SOAP zarfının Header kısmına bir bilgi koyması ve servisin bu bilgiyi ilgili operasyon içerisinde yakalayarak değerlendirmesi gerekiyor. Gelin adım adım bu işi nasıl yapabileceğimize bir bakalım.

Servis Tarafı

Geliştireceğimiz WCF Service Application içerisinde, aşağıdaki sınıf diagramında görülen tiplerin olduğunu varsayalım. (Bunlar bir anlamda senaryomuzu zevkli ve eğlenceli hale getirmek için yaptığımız çabalar aslında)

[![wcfch_4](/assets/images/2014/wcfch_4_thumb.png)](/assets/images/2014/wcfch_4.png)

Sadece örnek senaryomuza hizmet etmek üzere tasarlanmış olan bu Dummy tiplerin içerikleri ise aşağıdaki gibidir.

Bir kullanıcının rollerini ve bu role içerisinde yapabildiklerini temsilen kullanılan MemberRole ve RoleAction sınıflarımız;

```csharp
using System.Collections.Generic;

namespace RoleServer 
{ 
    public class MemberRole 
    { 
        public int MemberRoleId { get; set; } 
        public string Description { get; set; } 
        public List<RoleAction> ActionList{ get; set; } 
    } 
}

namespace RoleServer 
{ 
    public class RoleAction 
    { 
        public int RoleActionID { get; set; } 
        public string Name { get; set; } 
        public string Status { get; set; } 
    } 
}
```

Header içerisinden gelen kullanıcı bilgisine göre rolleri yükleyecek servise ait sözleşme (Service Contract) ve tip içeriğimiz;

```csharp
using System.Collections.Generic; 
using System.ServiceModel;

namespace RoleServer 
{ 
    [ServiceContract] 
    public interface IRoleLoader 
    { 
        [OperationContract] 
        List<MemberRole> GetRoles(); 
    } 
}

using System.Collections.Generic; 
using System.ServiceModel; 
using System.ServiceModel.Channels;

namespace RoleServer 
{ 
    public class RoleLoader 
        : IRoleLoader 
    { 
        public List<MemberRole> GetRoles() 
        { 
            List<MemberRole> roles = new List<MemberRole>();

            OperationContext operationContext = OperationContext.Current; 
            RequestContext requestContext = operationContext.RequestContext; 
            MessageHeaders headers = requestContext.RequestMessage.Headers; 
            int headerValue = headers.FindHeader("Username", string.Empty); 
            string userName = (headerValue < 0) ? "Bulunamadı" : headers.GetHeader<string>(headerValue);

            // memberID bilgisine göre kullanıcının rolleri yüklenir. 
            // Sembolik olarak bazı roller yüklenmiştir. 
            // Gerçek hayat odaklı bir üründe userName bilgisi 
            // bir Repository üzerinden sorgulanarak getirilir 
            if(userName=="bsenyurt") 
            { 
                roles.Add(new MemberRole 
                { 
                    MemberRoleId = 1, 
                    Description = "Geliştirici Rolleri", 
                    ActionList = new List<RoleAction> 
                    { 
                        new RoleAction{ RoleActionID=8, Name="Download_From_NuGet", Status="Grant"}, 
                        new RoleAction{ RoleActionID=8, Name="Access_To_Social",Status="Deny"}, 
                    } 
                }); 
            }

            return roles; 
        } 
    } 
}
```

ve servis tarafındaki standart web.config içeriğimiz.

```xml
<?xml version="1.0" encoding="utf-8" ?> 
<configuration> 
    <system.diagnostics> 
        <sources> 
            <source name="System.ServiceModel.MessageLogging" switchValue="Warning,ActivityTracing"> 
                <listeners> 
                    <add type="System.Diagnostics.DefaultTraceListener" name="Default"> 
                        <filter type="" /> 
                    </add> 
                    <add name="ServiceModelMessageLoggingListener"> 
                        <filter type="" /> 
                    </add> 
                </listeners> 
            </source> 
        </sources> 
        <sharedListeners> 
            <add initializeData="c:\web_messages.svclog" 
                type="System.Diagnostics.XmlWriterTraceListener, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" 
                name="ServiceModelMessageLoggingListener" traceOutputOptions="Timestamp"> 
                <filter type="" /> 
            </add> 
        </sharedListeners> 
    </system.diagnostics> 
    <system.serviceModel>        
        <diagnostics> 
            <messageLogging logEntireMessage="true" logMalformedMessages="true" 
                logMessagesAtServiceLevel="true" logMessagesAtTransportLevel="true" /> 
        </diagnostics> 
        <behaviors> 
            <serviceBehaviors> 
                <behavior name=""> 
                    <serviceMetadata httpGetEnabled="true" httpsGetEnabled="true" /> 
                    <serviceDebug includeExceptionDetailInFaults="true" /> 
                </behavior> 
            </serviceBehaviors> 
        </behaviors> 
        <serviceHostingEnvironment aspNetCompatibilityEnabled="false" 
            multipleSiteBindingsEnabled="true" /> 
    </system.serviceModel> 
</configuration>
```

Konfigurasyon dosyasında, istemci tarafından gelen mesaj içeriklerini görebilmek için Trace mekanizmasının etkinleştirildiğini önceden belirtelim. Böylece SOAP zarfının Header kısmındaki içeriği sunucuya ulaştıktan sonra görme şansımız da olacaktır.

Görüldüğü üzere tipik olarak SOAP tabanlı basit bir WCF servisi söz konusudur. Servisin can alıcı noktası ise GetRoles metodunun içeriğidir.

GetRoles Metodunun İşleyişi

Bu metod içerisindeki kritik planlama, istemciden gelen mesaj içeriğindeki Header elementinin yakalanmasıdır. İlk olarak güncel çalışma zamanı içeriği yakalanır. OperationContext tipinden ele alınan içerik üzerinden RequestContext örneği yakalanır. Bu bilgi içerisinde istemciden gelen mesaj bulunmaktadır. Çok doğal olarak RequestMessage.Headers özelliği, mesaj içerisindeki olası Header elementlerini döndürecektir.

> OperationContext aslında Web uygulamalarından aşina olduğumuz HttpContext kullanımına oldukça benzemektedir. Her ikiside çalışma zamanında oluşan Context özelliklerine ve içeriklerine erişim notkasında değerlendirilir. Burada da benzer bir yaklaşım söz konusu olmuş ve istemciden gelen mesaj içeriği güncel Context üzerinden yakalanmıştır.

FindHeader metoduna yapılan çağrıda iki parametre kullanılmıştır. İlk parametre Header elementinin adıdır. İkinci parametre ile Namespace bildirimi yapılır. Örnekte bir Namespace ele alınmamıştır. FindHeader metodu geriye integer tipinden bir değer döndürür. Aslında bu bilgi ilk parametre ile belirtilen Header’ ın koleksiyon içerisindeki indis değeridir. 0 veya üstü bir değer olması, bulunduğu anlamına gelmektedir. Header içerisinde ki bilgiye ulaşmak için GetHeader fonksiyonundan yararlanılır.

Metodun ilerleyen kısımlarında dummy bir kod parçası uygulanmış ve sembolik olarak rol yükleme işlemi gerçekleştirilmiştir. Gerçek hayat örneğinde buradaki yüklemelerin bir repository üzerinden yapılması elbette daha uygun olacaktır.

İstemci Tarafı

Senaryomuzda istemci tarafının basit bir Console uygulaması olarak geliştirilmesinde her hangi bir sakınca yoktur. WCF servisine ait referansın indirilmesi ve Proxy tipinin hazırlanmasını takiben aşağıdaki kod içeriği geliştirilerek ilerlenebilir.

```csharp
using Consumer.Rolehost; 
using System; 
using System.ServiceModel; 
using System.ServiceModel.Channels;

namespace Consumer 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            RoleLoaderClient client = new RoleLoaderClient();

            EndpointAddressBuilder addressBuilder = new EndpointAddressBuilder(client.Endpoint.Address);

            addressBuilder.Headers.Add(AddressHeader.CreateAddressHeader("Username",string.Empty,"bsenyurt")); 
            client.Endpoint.Address = addressBuilder.ToEndpointAddress();

            var roles=client.GetRoles();

            foreach (var role in roles) 
            { 
                Console.WriteLine(role.Description); 
                foreach (var action in role.ActionList) 
                { 
                    Console.WriteLine("{0} -> {1}" 
                        ,action.Name.PadRight(20) 
                        ,action.Status.PadRight(10)); 
                } 
            }

            client.Close(); 
        } 
    } 
}
```

İstemci açısından bakıldığında yapılması gereken iş Username isimli Header elementini oluşturup buna bir bilgi eklemektir. Bunun için EndpointAddressBuilder nesne örneğinden yararlanılmaktadır. Dikkat edileceği üzere bu örnek hali hazırda var olan servisin Endpoint tanımındaki adres bilgisi kullanılarak oluşturulur. Sonrasında Headers koleksiyonuna bir AddressHeader bilgisi eklenir.

AddressHeader tipinin CreateAddressHeader metodu üç parametre almaktadır.İlk parametre eklenecek Header elementinin adıdır. İkinci parametre Namespace bilgisidir ki bu örnekte boş geçilmiştir. Son parametre ise elementin değeridir.

Hazırlanan bu yeni bilgiler ışığında EndPoint bilgisinin yeniden üretilmesi gerekir. Bu aşamada EndpointAddressBuilder sınıfının ToEndpointAddress metodundan yararlanılmış ve oluşan örnek, proxy tipinin kullanmakta olduğu Endpoint’ in Address bilgisine atanmıştır. Bu kısımdan sonra normal olarak servis üzerindeki GetRoles metodunun çağırılması ve elde edilen Role ve Action listelerinde dolaşılması söz konusudur.

Çalışma Zamanı Sonuçları

Uygulama test edildiğinde istemci tarafının aşağıdaki çalışma zamanı görüntüsüne sahip olması beklenmektedir.

[![wcfch_3](/assets/images/2014/wcfch_3_thumb.png)](/assets/images/2014/wcfch_3.png)

Tahmin edileceği üzere bsenyurt bilgisine ait Rol tanımlamaları elde edilmiştir. Diğer yandan oluşan svclog dosyaları incelenirse aşağıdaki ekran görüntülerinde olduğu gibi Header bilgilerinin sunucu tarafına ulaştığı anlaşılabilir.

Formatlanmış görünüm;

[![wcfch_1](/assets/images/2014/wcfch_1_thumb.png)](/assets/images/2014/wcfch_1.png)

XML içeriği;

[![wcfch_2](/assets/images/2014/wcfch_2_thumb.png)](/assets/images/2014/wcfch_2.png)

Pek tabi bir Header bilgisinin gönderilmemesi veya yanlış olması halinde istemci tarafına bir Role listesi gönderilmeyecektir (Belki bu durumda varsayılan bir listenin gönderilmesi düşünülebilir)

Bir Diğer Yol

Header bilgisi göndermenin bir diğer yolu da konfigurasyon içerisinde bunu doğrudan yazmaktır. İstemci tarafında ki EndPoint elementi içerisinde aşağıdaki gibi bir değişiklik yeterli olacaktır.

```xml
<?xml version="1.0" encoding="utf-8" ?> 
<configuration> 
    <startup> 
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5.1" /> 
    </startup> 
    <system.serviceModel> 
        <bindings> 
            <basicHttpBinding> 
                <binding name="BasicHttpBinding_IRoleLoader" /> 
            </basicHttpBinding> 
        </bindings> 
        <client> 
          <endpoint address="http://localhost:4718/RoleLoader.svc" binding="basicHttpBinding" 
              bindingConfiguration="BasicHttpBinding_IRoleLoader" contract="Rolehost.IRoleLoader" 
              name="BasicHttpBinding_IRoleLoader"> 
            <!-- Alternatif bir yol olarak client header kısmına config dosyasından da bilgi eklenebilir. 
                    Tabi hem bu yöntem hem de kod tarafından Header bilgisinin eklenmesi çalışma zamanında exception 
                    oluşmasına neden olacaktır. İkisinden biri tercih edilmelidir. 
                    --> 
            <headers> 
              <Username>bsenyurt</Username> 
            </headers> 
          </endpoint> 
        </client>      
    </system.serviceModel> 
</configuration>
```

Tabi bu teknik seçildiğinde kod tarafındaki Header ekleme kısımlarının kullanılmaması gerekir. Aksi durumda çalışma zamanında iki yerden birden Header kısmı eklenmeye çalışıldığına yönelik Exception mesajı alınacaktır.

Peki Ya Custom Type Kullanmak İstersek?

Örnek senaryoda istemci tarafından Header bilgisi olarak string bir içeriğin gönderilmesi ele alınmıştır. Çok doğal olarak istemci tarafının örneğin bir POCO (Plain Old CLR Object) tipine ait nesneyi göndermesi de düşünülebilir. Örneğin hem servis hem de istemci tarafı için aşağıdaki gibi ortak bir tipin var olduğunu düşünelim.

```csharp
using System;

namespace Consumer 
{    
    public class CustomMessageHeader 
    { 
        public string Text { get; set; } 
        public DateTime Date { get; set; } 
    } 
}
```

Şimdi istemci tarafında bu tipe ait bir nesne örneğini Header içeriğine gömmek istediğimizi düşünelim. Bu durumda AddressHeader.CreateAddressHeader metodunu biraz daha farklı kullanmamız gerekecektir. Nitekim XML içerisine serileştirilmesi gereken bir tip söz konusudur.

```csharp
XmlObjectSerializer serializer = new DataContractSerializer(typeof(CustomMessageHeader)); 
addressBuilder.Headers.Add( 
    AddressHeader.CreateAddressHeader("ConsumerMessage", 
        "http://RoleServer/Header/ConsumerMessage", 
        new CustomMessageHeader 
        { 
            Text = "Bu gün hava bir harika!" 
            , Date = DateTime.Now 
        } 
        ,serializer) 
    );
```

İlk olarak CustomMessageHeader tipi için XmlIObjectSerializer türevi bir nesne örneği oluşturulmlaktadır. Bu örnek CreateAddressHeader metodunun son parametresinde kullanılmakta olup, istemcinin servis tarafına mesaj gönderdiği noktada devreye girecek ve CustomMessageHeader nesne örneğini Xml formatında serileştirecektir.

Servis Tarafına Gelen Mesaj

Eğer uygulama test edilir ve servis tarafına inen mesaj içeriğine bakılırsa, CustomeMessageHeader sınıfının özelliklerine ait değerlerin eklendiği gözlemlenebilir. Trace log’ ların da bu içerik aşağıda görüldüğü gibidir.

[![wcfch_7](/assets/images/2014/wcfch_7_thumb.png)](/assets/images/2014/wcfch_7.png)

SOAP zarf içeriğine bakıldığında durum daha net fark edilebilir.

```xml
<s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/"> 
    <s:Header> 
        <Username xmlns="">bsenyurt</Username> 
        <ConsumerMessage xmlns="http://RoleServer/Header/ConsumerMessage" xmlns:i="http://www.w3.org/2001/XMLSchema-instance"> 
            <Date xmlns="http://schemas.datacontract.org/2004/07/Consumer">2014-04-03T13:12:48.4474218+03:00</Date> 
            <Text xmlns="http://schemas.datacontract.org/2004/07/Consumer">Bu gün hava bir harika!</Text> 
        </ConsumerMessage> 
        <To s:mustUnderstand="1" xmlns="http://schemas.microsoft.com/ws/2005/05/addressing/none"> http://localhost:4718/RoleLoader.svc</To> 
        <Action s:mustUnderstand="1" xmlns="http://schemas.microsoft.com/ws/2005/05/addressing/none"> http://tempuri.org/IRoleLoader/GetRoles</Action> 
    </s:Header> 
    <s:Body> 
        <GetRoles xmlns="http://tempuri.org/"></GetRoles> 
    </s:Body> 
</s:Envelope>
```

Servis Bunu Nasıl Anlayacak?

Servis tarafına istemcinin özel bir.Net nesne içeriğini Header bilgisi olarak göndermesi bu kadar basittir. Peki ya servis tarafı bu tipi nasıl çözümleyecek? XML olarak gelen bu element yapısının kod tarafında anlaşılabilir olması/değerlendirilmesi için bir kaç yol mevcuttur. Her şeyden önce Header içeriğinin bir XML element ağacından oluştuğunu düşünecek olursak bunu parse ederek yorumlamak çok da zor değil. Bu amaçla GetRoles metodu içerisinde aşağıdaki gibi bir kod parçasını kullanabiliriz.

```csharp
XElement root = XElement.Parse(requestContext.RequestMessage.ToString());

var date= root 
    .Descendants() 
    .Where( 
    n => n.Name == XName.Get("Date","http://schemas.datacontract.org/2004/07/Consumer")) 
    .FirstOrDefault() 
    .Value;

var text = root 
    .Descendants() 
    .Where( 
    n => n.Name == XName.Get("Text", "http://schemas.datacontract.org/2004/07/Consumer")) 
    .FirstOrDefault() 
    .Value;
```

Aslında tek yaptığımız gelen Request mesajının XML içeriği üzerinde Text ve Date isimli elementleri aramaktır. Eğer çalışma zamanı görünümüne bakarsak değişken değerlerini başarılı bir şekilde elde edebildiğimizi görürüz.

[![wcfch_8](/assets/images/2014/wcfch_8_thumb.png)](/assets/images/2014/wcfch_8.png)

Tabi ki söz konusu içeriği servis tarafında bir nesne olarak ele almak çok daha mantıklıdır. Fakat bu senaryoda servis tarafında bir tip bildirimi söz konusu değildir. Yani istemci tarafında Header bilgisini oluşturmak için kullanılan ve XML serileştirmeye tabi tutulan CustomMessageHeader sınıfı servis tarafında oluşturulmamıştır.

Message Contracts

Bu senaryo haricinde Header kullanımları için WCF tarafında farklı bir kavramı da değerlendirmeyi düşünebiliriz. O da MessageContract kullanımıdır. Mesaj sözleşmeleri sayesinde SOAP zarflarının Header ve Body kısımlarının servis tarafında tanımlanması mümkündür. Bir başka deyişle SOAP mesaj içeriklerinin nesnel olarak kod tarafında tanımlanabilmesi söz konusudur. Bu durumda Header içeriklerinin birer POCO gibi ele alınması daha kolay olmaktadır.

> Aslında SOAP zarflarının içeriklerinin kesinleştirilmek istendiği, mesajların interoperability noktasında XSD yerine farklı kurallara göre oluşması gerektiği durumlarda Message Contract kullanımı tercih edilebilir. Mesaj sözleşmeleri ile ilişkili olarak eski bir yazıya [bu adresten ulaşabilirsiniz](https://www.buraksenyurt.com/post/Mesaj-Sozlesmeleri(Message-Contracts)-bsenyurt-com-dan).

Görüldüğü üzere istemciler Header kısımlarını kullanarak da kendilerini servis tarafına tanıtabilir ve operasyonların istemci bazında özelleştirilmesi sağlanabilir. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.