---
layout: post
title: "WCF Service' lerinde Routing ile Versiyonlama"
date: 2013-03-04 00:50:00 +0300
description: "Geçen gün şöyle eskiden yazmış olduğum makalelere bir bakayım dedim. Derken gözüm WCF 4.0’ ın Beta zamanlarında yazdıklarıma takıldı. O zamanlar .Net Framework 4.0' ün Beta sürümü çıktığında, incelemeye çalıştığım önemli yeniliklerden birisi de yönlendirme servisleri(Routing Service) idi."
categories:
  - wcf
tags:
  - wcf
  - csharp
  - xml
  - dotnet
  - http
---
# WCF Service' lerinde Routing ile Versiyonlama
[![oldbook](/assets/images/2013/oldbook_thumb.jpg)](/assets/images/2013/oldbook.jpg)[Orjinal yazım tarihi 2012 Eylül’ dür]

Merhaba Arkadaşlar,

Geçen gün şöyle eskiden yazmış olduğum makalelere bir bakayım dedim. Derken gözüm WCF 4.0’ ın Beta zamanlarında yazdıklarıma takıldı. O zamanlar.Net Framework 4.0' ün Beta sürümü çıktığında, incelemeye çalıştığım önemli yeniliklerden birisi de yönlendirme servisleri (Routing Service) idi.

İstemciden gelen talepleri analiz ederek, arka planda yer alan asıl servislere mesajların taşınması noktasında göz önüne alınabilecek önemli bir kabiliyet sunulmaktaydı. Aslında RoutingService, buradaki işi güçlü filtreleme özellikleri ile epeyce kolaylaştıran bir tip olarak karşımıza çıkmaktaydı. Hatta bu konuyu bir kaç makale ile de ele almaya çalışmıştım.

[WCF 4.0 Yenilikleri - Routing Service Geliştirmek - Giriş [Beta 1]](https://www.buraksenyurt.com/post/WCF-40-Yenilikleri-Routing-Service)

[WCF 4.0 Yenilikleri - Routing Service Geliştirmek - Hello World [Beta 1]](https://www.buraksenyurt.com/post/WCF-40-Yenilikleri-Routing-Service-Gelistirmek-Hello-World)

[WCF 4.0 Yenilikleri - Routing Service - Hata Yönetimi [Beta 1]](https://www.buraksenyurt.com/post/WCF-40-Yenilikleri-Routing-Service-Hata-Yonetimi)

[WCF 4.0 Yenilikleri - Routing Service - MatchAll Filtresi [Beta 1]](https://www.buraksenyurt.com/post/WCF-40-Yenilikleri-Routing-Service-MatchAll-Filtresi)

[WCF WebHttp Services - Routing](../2010/WCF WebHttp Services - Routing.md)

Tabi bunların hepsi Beta sürümüne ait incelemelerdi. Zaman ilerledikçe yönlendirme servislerinin önemli senaryolarda göz önüne alınabileceğini de gördük. Örneğin Load Balancing gibi.

Service yönelimli mimarinin (Service Oriented Architecture) uygulandığı WCF (Windows Communication Foundation) gibi alt yapılarda karşılaşılan önemli sorunlardan birisi de, aynı servisin birden fazla farklı versiyona sahip sürümlerinin bulunması ve böyle bir durumda istemcilerin istedikleri versiyonu nasıl çağırabilecekleridir. Bu tip bir durum ile pek çok sebepten ötürü karşılaşılabilir. Tabi bir versiyonlamanın söz konusu olması için servis üzerinde bir takım değişikliklerin mevzu bahis olması da gerekmektedir

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_115.png)

Bu değişiklikler aşağıda görülen 4 farklı kategori üzerinden ele alınabilirler.

- Contract (Sözleşme) değişimleri: Örneğin servise bir operasyon ilave edilmiş veya operasyona gelip giden parametre yapısında değişiklikler (ekleme, çıkartma vb) yapılmış olabilir.
- Address (Adres) değişimleri: Servis farklı bir lokasyona taşındığı için yeni bir endpoint adresine gereksinim duyulabilir.
- Binding (Bağlayıcı tip) değişimleri: Örneğin servis endpoint'i için WS-I ile belirtilen bir security değişikliği söz konusu olabilir. Bu durumda yeni bir bağlayıcı tipin (Binding Type) uygulanması gündeme gelebilir.
- Implementation değişimleri: Servis içerisinde yer alan operasyonel bir metodun içeriği değişmiş olabilir.

Tüm bu değişim çeşitleri mümkün olabilir ve bu son derece de olağandır. Ancak uzun vadede bakıldığında, yaşamakta olan bazı sistemlerin yenilenmesi sırasında veya kademeli olarak güncellenmeleri noktalarında, istemcilerin söz konusu değişime uğraşmış servislerin hangi sürümlerini/versiyonlarını kullanması da bir sorun olarak karşımıza çıkmaktadır.

Örneğin yenilenecek olan bir ürünün modüllerinin bir kısmı A servisinin 1.0 versiyonu ile çalışmaya devam edecek iken, diğer bir kısmı da yine aynı A servisinin 2.0 versiyonu ile çalışmak durumunda kalabilir. İşte bu noktada versiyonlanan servislerden hangisinin, istemciler tarafından nasıl ele alınabileceğinin bir çözümünün olması gerekmektedir.

Biz bu yazımızda söz konusu versiyonalama problemini WCF tarafındaki Routing servis aracılığı ile çözümlemeye çalışıyor olacağız. Bildiğiniz üzere Routing servisler sayesinde, istemciden gelen taleplerin karşılanarak arka planda duran asıl servislere yönlendirilmesi mümkün olabilir. Bu, Load Balancing gibi çözümlerde ele alınabilecek bir servis çeşidi olmakla birlikte, biraz sonra göreceğimiz gibi versiyonlama probleminde de değerlendirilebilmektedir. Tabi biz örneğimizde aşağıdaki senaryoyu göz önüne alıyor olacağız.

> Müşteri bilgisini çekmek için kullanılan bir servisin standart olarak kullandığı operasyonların bazılarının (ki örneğimizde basitlik açısından tek bir operasyon olacak) parametrik yapısında değişimler olmuştur. Var olan sürüm müşterinin hesap numarasına göre çalışmaktayken, yeni sürümde söz konusu servis operasyonu TCKN ile çalışıyor olacaktır. Buna göre var olan uygulamalar eski versiyonu kullanmaya devam edecek iken, yeni geliştirilecek olan uygulamalar son sürümü ele alacaktır.
> Ama ilerleyen zamanlarda eski sürümlerin içerisine müdahalelerde bulunarak hangi versiyonu kullanacaklarına çalışma zamanında karar verilmesi yeteneğine sahip olmaları da gündemdedir. İşte burada servis sürümlerinin uygun olan versiyonlarna çalışma zamanında karar verilmesi yeteneği, Routing Service’ lerin önemini biraz daha arttıracaktır.

Görüldüğü üzere servis sözleşmesinde bir değişim söz konusudur. Routing servisin buradaki görevi, istemciden gelecek olan versiyon talebine göre arka planda istenen servislere yönlendirme yapmaktır.

İlk sıkıntılardan birisi şudur. Aslında servisin iki farklı versiyonu olmasına rağmen operasyon adları aynıdır. Bu yüzden Routing servisi yönlendirmeye karar vermek için Action adlarından yararlanamaz (Bir başka deyişle Action Filters kullanılamaz bu senaryoda) Bunun yerine, istemciden gelen mesaj içerisinde özel bir başlık bilgisi (Custom Header) kullanılabilir ve bu bilgi Routing servis tarafında filtrelenebilir. Kafalar karıştı değil mi? Benim de

![Smile](/assets/images/2013/wlEmoticon-smile_47.png)

Gelin basit adımlar ile ilerleyerek bir Solution üzerinden ilgili senaryoyu ele almaya çalışalım. Senaryomuzda aşağıdaki şekilde görülen uygulamalar söz konusudur.

[![rav_1](/assets/images/2013/rav_1_thumb.png)](/assets/images/2013/rav_1.png)

Şekilden de görüleceği üzere versiyonları farklı olan iki servisimiz ve bunlara istemciden gelen talep doğrultusunda yönlendirme yapan bir Router Service uygulamamız bulunmaktadır.

> Senaryomuzda işlemleri olabildiğince basite indirgedik ve tüm servis noktalarında BasicHttpBinding bağlayıcı tipinden yararlandık. Çok doğal olarak arka planda yer alan servislerin sayısı artabilir ve her biri farklı Binding tipleri ile de bezenmiş olabilir ki bu durumda Binding bazlı bir versiyonlama farkı da oluşacaktır.
> Söz gelimi servislerden birisin In-Proc modda erişilebilecek şekilde tasarlanmışken, diğer biri WS Federation 2007 standartlarında kullanılabilecek şekilde geliştirilmiş olabilir.

CustomerServiceV1 ve CustomerServiceV2 temel olarak iki farklı WCF servis uygulaması içerisinde yer almaktadır (Bu tabiki zorunlu değildir. Aynı WCF Service uygulaması içerisinde de birden fazla svc bulunabilir ve bu şekilde bir versiyonlama yaptırılabilir) Senaryoda bu servisler farklı WCF Service Application projeleri içerisine serpiştirilmişlerdir. Her ikisi de aynı servis sözleşmesini (ICustomerService) dışarıya sunmaktadır ve GetCustomer isimli bir operasyon içermektedirler. Lakin ilk versiyon Müşteri numarası ile çalışacak şekilde tasarlanmışken, ikinci versiyon TC Kimlik Numarasını kullanmaktadır. Söz konusu uygulamaların kod ve konfigurasyon içerikleri (web.config) aşağıdaki gibidir.

CustomerServiceV1 için;

```csharp
using System.ServiceModel;

namespace CustomerServiceV1 
{ 
    [ServiceContract] 
    public interface ICustomerService 
    { 
        [OperationContract] 
        string GetCustomer(string customerNumber); 
    } 
}

namespace CustomerServiceV1 
{ 
    public class CustomerService 
        : ICustomerService 
    { 
        public string GetCustomer(string customerNumber) 
        { 
            return "Müşteri numarası ile müşteri bilgisi talep edildi"; 
        } 
    } 
}
```

Web.config içeriği;

```xml
<?xml version="1.0" encoding="utf-8" ?> 

<configuration> 
    <system.serviceModel> 
        <behaviors> 
            <serviceBehaviors> 
                <behavior name=""> 
                    <serviceMetadata httpGetEnabled="true" httpsGetEnabled="true" /> 
                    <serviceDebug includeExceptionDetailInFaults="false" /> 
                </behavior> 
            </serviceBehaviors> 
        </behaviors> 
        <serviceHostingEnvironment aspNetCompatibilityEnabled="true" 
            multipleSiteBindingsEnabled="true" /> 
    </system.serviceModel> 
</configuration>
```

CustomerServiceV2 için;

```csharp
using System.ServiceModel;

namespace CustomerServiceV2 
{ 
    [ServiceContract] 
   public interface ICustomerService 
    { 
        [OperationContract] 
        string GetCustomer(string tckn); 
    } 
}

namespace CustomerServiceV2 
{ 
    public class CustomerService 
        : ICustomerService 
    { 
        public string GetCustomer(string tckn) 
        { 
            return "TCKN ile müşteri bilgisi çekilir"; 
        } 
    } 
}
```

web.config içeriği;

```xml
<?xml version="1.0" encoding="utf-8" ?> 
<configuration> 
    <system.serviceModel> 
        <behaviors> 
            <serviceBehaviors> 
                <behavior name=""> 
                    <serviceMetadata httpGetEnabled="true" httpsGetEnabled="true" /> 
                    <serviceDebug includeExceptionDetailInFaults="false" /> 
                </behavior> 
            </serviceBehaviors> 
        </behaviors> 
        <serviceHostingEnvironment aspNetCompatibilityEnabled="true" 
            multipleSiteBindingsEnabled="true" /> 
    </system.serviceModel> 
</configuration>
```

Buraya kadar ki kısımda görüldüğü üzere standart WCF servislerinin geliştirilmesi söz konusudur. Asıl mühim olan nokta ise Routing Service uygulamasının içeriğidir. Bu servis uygulamasını basit bir Console Application şeklinde tasarlıyor olacağız. Kendisi bir Service Host olarak görev yapıyor olacak. Dolayısıyla açık kaldığı sürece istemcilere hizmet verebilecek. En önemli nokta konfigurasyon içeriğidir. Örnek senaryomuzda app.config içeriği aşağıdaki gibidir.

> Console uygulamasına, System.ServiceModel.Routing ve System.ServiceModel assembly'larının referans edilmesi gerektiğini hatırlatmak isterim.

```xml
<?xml version="1.0" encoding="utf-8" ?> 
<configuration> 
  <system.serviceModel> 
    <services> 
      <service behaviorConfiguration="rtngCnfg" 
               name="System.ServiceModel.Routing.RoutingService"> 
        <host> 
          <baseAddresses> 
            <add baseAddress="http://localhost:62323/routingservice/router/" /> 
          </baseAddresses> 
        </host> 
        <endpoint address="customer" 
                  binding="basicHttpBinding" 
                  name="routerEndpoint" 
                  contract="System.ServiceModel.Routing.IRequestReplyRouter" /> 
      </service> 
    </services> 
    <behaviors> 
      <serviceBehaviors> 
        <behavior name="rtngCnfg"> 
          <routing filterTableName="versioningFilterTable" /> 
        </behavior> 
      </serviceBehaviors> 
    </behaviors>
    <client> 
      <endpoint name="customerServiceV1" 
                address="http://localhost:62292/CustomerService.svc" 
                binding="basicHttpBinding" 
                contract="*" />
      <endpoint name="customerServiceV2" 
                address="http://localhost:62291/CustomerService.svc" 
                binding="basicHttpBinding" 
                contract="*" /> 
    </client> 
    <routing> 
      <namespaceTable> 
        <add prefix="customer" namespace="http://core.services.customer/"/> 
      </namespaceTable> 
      <filters> 
        <filter name="XPathFilterForVersion1" filterType="XPath" 
                filterData="sm:header()/customer:SrvVersion = '1'"/> 
        <filter name="XPathFilterForVersion2" filterType="XPath" 
                filterData="sm:header()/customer:SrvVersion = '2'"/> 
      </filters> 
      <filterTables> 
        <filterTable name="versioningFilterTable"> 
          <add filterName="XPathFilterForVersion1" endpointName="customerServiceV1"/> 
          <add filterName="XPathFilterForVersion2" endpointName="customerServiceV2"/> 
        </filterTable> 
      </filterTables> 
    </routing> 
  </system.serviceModel> 
</configuration>
```

Pek tabi bu uygulama bir Routing servis olduğundan, arka planda kullanacağı servisler için de bir Client gibi düşünülmelidir. Bu sebepten client elementi içerisinde, CustomerServiceV1 ve CustomerServiceV2 için gerekli End Point tanımlamaları bulunmaktadır.

> Router servis uygulaması herhangibir şekilde CustomerService'lerin servis referanslarını içermemektedir.No Add Service Reference yani
>
> ![Smile](/assets/images/2013/wlEmoticon-smile_47.png)

Servisin kendisi System.ServiceModel.Routing.IRequestReplyRouter sözleşme tipini kullanmaktadır. Config dosyası içerisindeki en önemli parça ise routing elementinin içeriği ve tanımlanan filtreleme opsiyonlarıdır.

XPath tekniğine göre, gelen mesajın Header kısmı analiz edilmekte ve SrvVersion kelimesinin karşılığı olan değere bakılmaktadır. Bu değerin 1 olması halinde CustomerService'in 1nci versiyonu, 2 olması halinde ise 2nci versiyonu için bir yönlendirme yapılacağı planlanmıştır. Filtreleme değeri ile, arka plandaki servis uç noktasının eşleştirilmesi ise filterTables elementi içerisinde gerçekleştirilmektedir. İstemci uygulamanın Main metodunun içeriği ise son derece basit ve sadedir.

```csharp
using System; 
using System.ServiceModel; 
using System.ServiceModel.Routing;

namespace RouterServer 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            using (ServiceHost serviceHost =new ServiceHost(typeof(RoutingService))) 
            { 
                serviceHost.Open(); 
                Console.WriteLine("Yönlendirme servisi etkin. Kapatmak için bir tuşa basınız."); 
                Console.ReadLine(); 
            } 
        } 
    } 
}
```

Dikkat edileceği üzere, RoutingService tipinden bir ServiceHost örneği oluşturulmaktadır. Bu,.Net Framework’ ün built-in routing alt yapısının yüklenmesi için yeterlidir. Gelelim istemci tarafına

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_115.png)

İstemci, müşteri servislerinden hangi versiyonu kullanırsa kullansın, bir şekilde sözleşmeden haberdar olmalı ve GetCustomer metoduna erişebilmelidir. Nitekim bu operasyonun çağırılabilmesi için bir de Proxy tipine ihtiyacı vardır. Burada CustomerServiceV1 veya CustomerServiceV2 isimli servislerin herhangibigirisinden yararlanılıp bir servis referansının istemci uygulamaya eklenmesi mümkündür. Şahsen ben bu şekilde bir yolu tercih ettim

![Sarcastic smile](/assets/images/2013/wlEmoticon-sarcasticsmile_6.png)

> Unutmayın! İstemcinin bilmesi gereken sözleşme Router servise ait değildir. Zaten Router servis için Metadata Publishing opsiyonu da kapalıdır ve hatta ortada bir Contract'da yoktur.
> İstemci, GetCustomer operasyonunu kullanacağı CustomerService sözleşmesine gereksinim duymaktadır. Sadece uygun versiyonu çağırmak için Router servisden yararlanacak ama aslında CustomerService'ine ait GetCustomer metodunu çağırıyor olacaktır.

İstemci tarafına Add Service Reference ile ilgili sözleşme eklendikten sonra (ki svcutil komut satırı aracılığı ile de ortak bir isim adı altında üretim yaptırılıp eklenmesi tercih edilebilirdi) Main metodu içerisine aşağıdaki örnek kodları eklememiz yeterli olacaktır.

```csharp
using ClientApp.CustomerServiceReference; 
using System; 
using System.ServiceModel; 
using System.ServiceModel.Channels;

namespace ClientApp 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            Console.WriteLine("Çalıştırmak istediğiniz servise ait versiyon numarasını giriniz. 1, 2"); 
            string versionNumber=Console.ReadLine();

            try 
            { 
                //Routing servis için bir Endpoint bildirimi yapılır 
                EndpointAddress ea = new EndpointAddress("http://localhost:62323/routingservice/router/customer");

                // Proxy nesnesi örneklenir 
                CustomerServiceClient client = new CustomerServiceClient(new BasicHttpBinding(), ea);

                // Scope örneklenir 
                using (OperationContextScope context = new OperationContextScope((client.InnerChannel))) 
               { 
                    //Header bilgisi alınır 
                    MessageHeaders header = OperationContext.Current.OutgoingMessageHeaders; 
                    //Header bilgisine kullanılmak istenen versiyon numarası verilir 
                    header.Add(MessageHeader.CreateHeader("SrvVersion", "http://core.services.customer/", versionNumber)); 
                    // Operasyon çalıştırılır 
                    string result1 = client.GetCustomer("1122");

                    Console.WriteLine(result1); 
                    Console.ReadLine(); 
               } 
            } 
            catch (Exception excp) 
            { 
                Console.WriteLine(excp.Message); 
            } 
        } 
    } 
}
```

İstemci uygulamada dikkat edileceği üzere kod üzerinden bir EndPoint tanımlaması ile işe başlanmakta ve bu uç noktayı BasicHttpBinding ile kullanan bir proxy örneği (CustomerServiceClient) üretilmektedir.

Kodun önemli olan kısmı MessageHeader içerisine istenen versiyon bilgisinin eklenmesidir. Bu, o andaki operasyonel Context'in elde edilmesinin ardından MessageHeaders tipi kullanılarak gerçekleştirilmektedir.

Artık senaryomuzu test edebiliriz. İşte benim uygulamaları test ederken aldığım sonuçlara ait ekran çıktıları.

İlk olarak 1 değerini girerek bir çağrıda bulunuyoruz;

[![rav_2](/assets/images/2013/rav_2_thumb.png)](/assets/images/2013/rav_2.png)

Ardından 2 değerini girerek;

[![rav_3](/assets/images/2013/rav_3_thumb.png)](/assets/images/2013/rav_3.png)

Pek tabi geçerli olmayan bir değerin girilmesi halinde istemci tarafında Exception alınması da son derece doğaldır

![Embarrassed smile](/assets/images/2013/wlEmoticon-embarrassedsmile_3.png)

Görüldüğü gibi bir servisin farklı versiyonlarını host ettiğimiz senaryolarda, istemcilerin istedikleri servislere gitmelerini sağlamak için araya konuşlandıracağımız bir Routing servis uygulamasından kolayca yararlanabiliriz. Senaryoda teknik olarak istemcinin Message Header içerisine koyduğu versiyon bilgisinin, yönlendirme servisi üzerinde XPath ile sorgulanması ile yakalanmaya çalışılması değerlendirilmiştir.

Örnek senaryo istenirse daha da zorlaştırılabilir. Söz gelimi servislerin sözleşmelerinde yapılacak kritik değişiklikler sonucu istemcinin bilmesi gereken contract bilgisinin de güncellenmesi şarttır. Bu konuyu bir düşünmenizi ve böyle bir senaryo oluşması halinde istemcinin istediği versiyonlara nasıl gidebileceğini araştırmanızı ve bir çözüm yolu bulmaya çalışmanızı öneririm. Bu iyi bir ev ödevi gibi geldi bana

![Smile](/assets/images/2013/wlEmoticon-smile_47.png)

Versiyonlama her zaman için zor ve karmaşık bir konudur. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[RoutingAndVersioning.zip (117,79 kb)](/assets/files/2013/RoutingAndVersioning.zip)