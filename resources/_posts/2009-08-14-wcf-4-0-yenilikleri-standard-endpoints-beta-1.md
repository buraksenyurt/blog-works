---
layout: post
title: "WCF 4.0 Yenilikleri - Standard Endpoints [Beta 1]"
date: 2009-08-14 03:15:00 +0300
description: "Bir süredir WCF 4.0 ile birlikte gelen yenilikleri tek tek incelemeye çalışıyoruz. İlk incelediğimiz noktalar konfigurasyon ayarları üzerinde yapılmış olan basitleştirmeleri içermektedir. Bu değişimlerden bir diğerini inceleyerek serimize devam ediyor olacağız. Bu anlamda konumuz Standard Endpoints başlığı altında gelen yeniliklerdir. Bu özelliği inceledikten sonra konuyu anlamanın en iyi yolunun bir önceki versiyonda ne olduğuna bakmak olduğuna karar verdim."
categories:
  - wcf-4-0-beta-1
tags:
  - wcf-4-0-beta-1
  - xml
  - csharp
  - dotnet
  - wcf
  - http
  - iis
  - visual-studio
---
# WCF 4.0 Yenilikleri - Standard Endpoints [Beta 1]
Merhaba Arkadaşlar,

Bir süredir WCF 4.0 ile birlikte gelen yenilikleri tek tek incelemeye çalışıyoruz. İlk incelediğimiz noktalar konfigurasyon ayarları üzerinde yapılmış olan basitleştirmeleri içermektedir. Bu değişimlerden bir diğerini inceleyerek serimize devam ediyor olacağız. Bu anlamda konumuz Standard Endpoints başlığı altında gelen yeniliklerdir. Bu özelliği inceledikten sonra konuyu anlamanın en iyi yolunun bir önceki versiyonda ne olduğuna bakmak olduğuna karar verdim.

Senaryomuza göre Http üzerinden sunulan bir servis için mexHttpBinding bağlayıcı tipini (Binding Type) kullanarak Metadata Publishing işlemini gerçekleştiriyoruz. Bir başka deyişle servisi ne yaptığı ve bunu hangi operasyonlar ile tanımladığı bilgisini istemcilere açıyoruz. Metadata üzerinden publishing işleminde anahtar noktanın servis sözleşmesi olarak IMetadataExchange arayüzünü kullanmak olduğunu bilmekteyiz. (Tabi doğrudan IIS üzerinden host edilen bir WCF servisi yazmıyorsak.) Bunu aklımızın bir köşesinde tutalım. Şimdi örneğimize ait konfigurasyon dosyasını aşağıdaki gibi geliştirdiğimizi düşünelim (İlk örneğimizi.Net Framework 3.5 tabanlı olarak geliştirdiğimizi hatırlatalım.)

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <behaviors>
            <serviceBehaviors>
                <behavior name="AdventureServiceBehavior">
                    <serviceMetadata />
                </behavior>
            </serviceBehaviors>
        </behaviors>
        <services>
            <service behaviorConfiguration="AdventureServiceBehavior" name="PreviousVersion.AdventureService">
                <endpoint address="" binding="basicHttpBinding" bindingConfiguration=""
                    name="EndPoint1" contract="PreviousVersion.IAdventure" />
                <endpoint address="Mex" binding="mexHttpBinding" bindingConfiguration="" name="MexEndpoint" contract="IMetadataExchange" />
                <host>
                    <baseAddresses>
                        <add baseAddress="http://localhost:5001/AdventureService" />
                    </baseAddresses>
                </host>
            </service>
        </services>
    </system.serviceModel>
</configuration>
```

Konfigurasyon dosyasında dikkat etmemiz gereken bir takım noktalar olduğu açıktır. MexEndpoint isimli Endpoint içerisinde yer alan contract niteliğinin değerini case-sensitive olarak doğru sözleşme tipini (Contract Type) işaret edecek şekilde yazmalıyız. Bağlayıcı tip olarak mexHttpBinding tipini belirtmeliyiz; nitekim base address ve EndPoint1 bilgilerine göre BasicHttp bazlı bir yayınlama yapmaktayız. Bunlara ek olarak, serviceMetadata niteliğinin bir servis davranışı (Service Behavior) olarak mutlaka bildirilmesi gerekmektedir. Aslında NetTcp kullansaydıkta metadata sözleşmesi olarak IMetadataExchange tipini belirtmemiz gerekiyordu. İlerlemeden önce servisimize ait örnek kod içeriği aşağıdaki gibi geliştirdiğimizi düşünelim.

```csharp
using System;
using System.ServiceModel;

namespace PreviousVersion
{
    [ServiceContract]
    interface IAdventure
    {
        [OperationContract]
        double GetLowestPrice(int category);
    }
    class AdventureService
        : IAdventure
    {
        public double GetLowestPrice(int category)
        {
            return 100;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(AdventureService));

            host.Open();

            Console.WriteLine("Service durumu {0}\nKapatmak için bir tuşa basın.", host.State.ToString());
            Console.ReadLine();

            host.Close();
        }
    }
}
```

Buna göre servis uygulamamız çalışıyorken, istemciler proxy içeriklerini üretebilmek için metadata bilgilerini ulaşabilecektir. Aynen aşağıdaki ekran görüntüsünde olduğu gibi. (Dikkat edileceği üzere servis çalıştırıldıktan sonra Visual Studio üzerinden Add Service Reference iletişim penceresi ile geliştirilen servisin metadata bilgilerine ulaşılabilmektedir.)

![blg63_PreviousAddRef.gif](/assets/images/2009/blg63_PreviousAddRef.gif)

Şimdi WCF 4.0 açısından duruma bakalım. Yeni gelen Standard Endpoints özelliğine göre, önceden tanımlanmış ve pek çok standart özelliği set edilmiş bazı Endpoint tanımlamaları gelmektedir. Örneğin Metadata Publishing sisteminde, IMetadataExchange kullanımı bir standarttır. Bu nedenle mexEndpoint isimli tipi kullanarak yukarıdaki örneği aşağıdaki config içeriği ile WCF 4.0 üzerinde gerçekleyebiliriz.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <system.serviceModel>
    <behaviors>
      <serviceBehaviors>
        <behavior>
          <serviceMetadata/>
        </behavior>
      </serviceBehaviors>
    </behaviors>
    <services>
      <service name="PreviousVersion.AdventureService">
        <endpoint address="" binding="basicHttpBinding" name="EndPoint1" contract="PreviousVersion.IAdventure" />
        <endpoint kind="mexEndpoint" address="Mex" />
        <host>
          <baseAddresses>
            <add baseAddress="http://localhost:5002/AdventureService" />
          </baseAddresses>
        </host>
      </service>
    </services>
  </system.serviceModel>
</configuration>
```

Dikkat edileceği üzere endpoint elementi içerisinde yer alan kind niteliğine mexEndpoint isimli bir değer atanmıştır. Bu değer standart olarak Metadata Exchange yayınlaması yapacak bir endpoint oluşturulmasını WCF çalışma zamanına (Runtime) bildirmektedir. Örneğin bu versiyonunu çalıştırdığımızda (.Net Framework 4.0 ve Visual Studio 2010 üzerinde geliştirilmiştir) bir önceki örnekte olduğu gibi istemci tarafından Metadata bilgisini çekebildiğimizi görürüz.

![blg63_AfterAddRef.gif](/assets/images/2009/blg63_AfterAddRef.gif)

Ne yazıkki, mexEndpoint varsayılan olarak Http bazlı Metadata Publishing'i desteklemektedir. Yani NetTcp kullandığımız durumlarda, binding niteliğini kullanarak bildirim yapmamız şarttır. Bu durumda aslında mexTcpBinding kullanılmasını belirttikten sonra birde IMetadataExchange arayüzünü contract niteliğinde belirtmek çokda fazla bir adım olarak görülmemelidir. Ancak burada bahsedilen standartlaştırılmış endPoint'ler sadece mexEndpoint tipinden mi oluşmaktadır? Tabiki hayır.

announcementEndpoint, discoveryEndpoint, udpAnnouncementEndpoint, udpDiscoveryEndpoint, workflowControlEndpoint gibi başka standart endPoint tipleride tanımlanmıştır. Dikkat çekici noktalardan biriside istenildiğinde bu standart endPoint tipleri içinde bazı özel ayarların yapılabileceğidir (Bu ayarlamaları ve yeni gelen standart endPoint tiplerinden diğerlerini ilerleyen yazılarımızda detaylı bir şekilde incelemeye çalışacağım) Bu noktada konfigurasyon içeriğinde standardEndpoints sekmesini ele almak yeterli olacaktır.

![blg63_StndrdPoints.gif](/assets/images/2009/blg63_StndrdPoints.gif)

WCF 4.0 tarafında basitleştirilmiş konfigurasyon (Simplified Configuration) özellikleri ile ilişkili yenilikleri incelemeye devam ediyor olacağız. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[StandardEndpoints.rar (48,35 kb)](/assets/files/2009/StandardEndpoints.rar)