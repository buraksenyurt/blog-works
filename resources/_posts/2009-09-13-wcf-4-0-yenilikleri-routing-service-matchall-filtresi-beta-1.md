---
layout: post
title: "WCF 4.0 Yenilikleri - Routing Service - MatchAll Filtresi [Beta 1]"
date: 2009-09-13 21:00:00 +0300
categories:
  - wcf-4-0-beta-1
tags:
  - windows-communication-foundation
---
Bundan önceki yazılarımızda WCF 4.0 için yönlendirme servislerinin (Router Service) nasıl yazılabileceğini incelemeye çalışmıştık. Fark edeceğiniz üzere yönlendirme servislerinin en önemli noktaları arasında filtreleme tablosu ve filtrelerin olduğunu gördük. Bununla birlikte sadece Action tipinde bir filtreleme kullanıp, istemciden gelen SOAP paketinin Action kısmından yararlanılarak bir yönlendirme yapılmasını inceledik.

![blg75_Giris.jpg](/assets/images/2009/blg75_Giris.jpg)

Oysaki filtreleme tipi olarak Action dışında, Address, AddressPrefix, StrictAnd, EndpointName, MatchAll, XPath gibi seçeneklerimiz de bulunmaktadır. İşte bu yazımızda MatchAll seçeneğini incelemeye çalışıyor olacağız. MatchAll seçeneğine göre, istemciden gelen mesajın içeriği ne olursa olsun, söz konusu talebin tanımlanan birden fazla DownStream servise yönlendirilmesi mümkündür. Ancak önemli bir kısıtlama vardır.

Bu kısıtlamaya göre sadece One-Way veya Duplex modeldeki iletişim (Communication) desteklenir. Dolayısıyla Request/Reply modelde olan iletişimi ele alan tipleri yönlendirme servisi üzerinde kullanamayız. Bu kısıtlamaya rağmen bazı senaryolarda (örneğin asenkron modellerde), istemciden gelen talebin birden fazla DownStream'e aktarılmasının WCF 4.0 ile gelen özellikler sayesinde kolaylaştırılmış olması, geliştiriciler açısından oldukça heyecan vericidir.

![Wink](/assets/images/2009/smiley-wink.gif)

Öyleyse vakit kaybetmeden basit bir örnek üzerinden ilerlemeye ne dersiniz. Ben yazıyı gecenin geç bir vaktinde yazdığım için yanımda bir adet sıcak kahveyi bulundurmayı ihmal etmedim.

![Cool](/assets/images/2009/smiley-cool.gif)

Örnek senaryomuzda aynı servis sözleşmesini (Service Contract) implemente eden 3 farklı alt servisimiz bulunmaktadır. Router Service, istemciden gelen talebi alıp ne olduğu ile ilgilenmeden doğrudan bu 3 servise aktarma işlemini üstlenmektedir. Dolayısıyla ispat etmemiz gereken noktalardan birisi, istemciden gelen talebin sonrasında operasyonun 3 servis üzerinde de çalışıyor olmasıdır. OneWay veya Duplex kısıtlamasından dolayı biz örneğimizde OneWay olarak imzalanmış basit bir servis operasyonu kullanıyor olacağız. Öncelikle örneğimize ait mimari modelimize bir göz atalım.

![blg75_Architecture.gif](/assets/images/2009/blg75_Architecture.gif)

Görüldüğü üzere sadece Endpoint tanımlamaları açısından farklı olan (ve gerçek hayat senaryolarında istenirse farklı makinelerde bulunabilecek olan) ama aynı sözleşmeyi uygulayan 3 Downstream servisimiz bulunmaktadır. Servislerimizin uyguladığı sözleşme aşağıdaki kod parçasında olduğu gibidir.

```csharp
using System.ServiceModel;
using System.Runtime.Serialization;

namespace AdventureContracts
{
    [ServiceContract(Namespace="http://adventure/productService")]
    public interface IAdventureContract
    {
        [OperationContract(IsOneWay=true)]
        void ProcessProduct(Product product);
    }

    [DataContract]
    public class Product
    {
        [DataMember]
        public int ProductId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public double ListPrice { get; set; }
        [DataMember]
        public int Amount { get; set; }
    }
}
```

Servislerimizin üçünün kodlarını burada ayrı ayrı yazmamıza gerek olmadığını düşünüyorum. Nitekim hem odaklanmamız gereken nokta Router servis tarafıdır hemde yazımızın okunurluğunun zorlaşmaması gerekmektedir. Tabiki örneği indirip incelemenizi şiddetle öneririm. Gelelim yönlendirme servisimize. Yönlendirme servisimizin App.config dosyası içeriği aşağıdaki gibidir.

Router Service konfigurasyon içeriği (App.config);

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <system.serviceModel>
    <behaviors>
      <serviceBehaviors>
        <behavior>
          <routing routingTableName="RTable"/>
          <serviceDebug includeExceptionDetailInFaults="true"/>
        </behavior>
      </serviceBehaviors>
    </behaviors>
    <client>
      <endpoint address="net.tcp://localhost:60001/Adventure/BackendMirror/ProductService1" binding="netTcpBinding" contract="*" name="ProductServiceEndpoint1" />
      <endpoint address="http://localhost:60002/Adventure/Backend/ProductService2" binding="wsHttpBinding" contract="*" name="ProductServiceEndpoint2"/>
      <endpoint address="http://localhost:60003/Adventure/Internal/ProductService3" binding="wsHttpBinding" contract="*" name="ProductServiceEndpoint3" />
    </client>
    <services>
      <service name="System.ServiceModel.Routing.RoutingService">
        <host>
          <baseAddresses>
            <add baseAddress="http://localhost:60005/Adventure/ProductService"/>
          </baseAddresses>
        </host>
        <endpoint binding="wsHttpBinding" contract="System.ServiceModel.Routing.ISimplexSessionRouter"/>
      </service>
    </services>
    <routing>
      <filters>
        <filter filterType="MatchAll" name="ProductFilter"/>
      </filters>
      <routingTables>
        <table name="RTable">
          <entries>
            <add endpointName="ProductServiceEndpoint1" filterName="ProductFilter"/>
            <add endpointName="ProductServiceEndpoint2" filterName="ProductFilter"/>
            <add endpointName="ProductServiceEndpoint3" filterName="ProductFilter"/>
          </entries>
        </table>
      </routingTables>
    </routing>
  </system.serviceModel>
</configuration>
```

Dikkat edileceği üzere fiterType niteliğine MatchAll değeri verilmiştir. Bu değere göre, istemciden gelecek olan talep (Request) name niteliğine atanan değere eş düşen endPoint noktalarına iletilmelidir ki bu bildirimlerde yine entries elementi içerisinde yapılmaktadır. entries elementi içerisindeki filterName niteliğinin değeri ile, filter elementi içerisindeki name niteliğinin değerlerinin aynı olduğuna lütfen dikkat edelim.

Konfigurasyon dosyasında önem arz eden noktalardan bir diğeride, yönlendirme servisi için kullanılan sözleşme tipidir (ISimplexSessionRouter). Hatırlayacağınız gibi, Request/Reply modelin desteklenmediğinden bahsetmiştik. Bu nedenle daha önceki örneklerimizde kullandığımız IRequestRepyleRouter built-in sözleşme tipini bu senaryoda kullanamayız.

Örneğimizi test ettiğimizde çalışma zamanında aşağıdaki ekran görüntüsüne benzer sonuçları alırız.

![blg75_Runtime.gif](/assets/images/2009/blg75_Runtime.gif)

Görüldüğü gibi tüm DownStream servisleri, istemciden gelen Product tipini ele almış ve basit bir şekilde kullanmıştır. Biz örneğimizde sadece gelen bilgiyi ekrana yazdırıyoruz. Aynı istemci paketinin, n sayıda DownStream servisi tarafından değerlendirilip üzerlerinde farklı şekillerde işlemler uygulanması söz konusu olduğunda, MatchAll filtereleme modelini göz önüne alabiliriz. Tabiki Request/Reply kısıtlamasını unutmamak gerekir. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Router Project 3.rar (249,84 kb)](https://www.buraksenyurt.com/pics/2009%2f8%2fRouter+Project+3.rar)
