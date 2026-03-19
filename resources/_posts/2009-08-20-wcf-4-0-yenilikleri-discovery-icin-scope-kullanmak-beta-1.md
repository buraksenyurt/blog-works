---
layout: post
title: "WCF 4.0 Yenilikleri - Discovery için Scope Kullanmak [Beta 1]"
date: 2009-08-20 16:57:00 +0300
description: "Bir önceki yazımızda WCF 4.0 tabanlı servislerde WS-Discovery protokolünün, Ad Hoc modeline göre nasıl uygulanabileceğini görmüştük. Ad Hoc modelinde istemcinin, yerel ağ üzerine dahil olan bir servisi aramak için kullanabileceği kriterleri önceden belirlemesi ve bunları kullanması gerektiğinden bahsetmiştik. Bu amaçla kod tarafında FindCriteria tipinden yararlanılmaktadır. Bir önceki örneğimizde, arama kriterinde sadece servis sözleşmesini(Service Contract) kullanmıştık."
categories:
  - wcf-4-0-beta-1
tags:
  - wcf-4-0-beta-1
  - xml
  - csharp
  - wcf
  - http
  - delegates
---
# WCF 4.0 Yenilikleri - Discovery için Scope Kullanmak [Beta 1]
Merhaba Arkadaşlar,

Bir [önceki](https://www.buraksenyurt.com/post/wcf40-Yenilikleri-Ad-Hoc-WS-Discovery)yazımızda WCF 4.0 tabanlı servislerde WS-Discovery protokolünün, Ad Hoc modeline göre nasıl uygulanabileceğini görmüştük. Ad Hoc modelinde istemcinin, yerel ağ üzerine dahil olan bir servisi aramak için kullanabileceği kriterleri önceden belirlemesi ve bunları kullanması gerektiğinden bahsetmiştik. Bu amaçla kod tarafında FindCriteria tipinden yararlanılmaktadır. Bir önceki örneğimizde, arama kriterinde sadece servis sözleşmesini (Service Contract) kullanmıştık.

Ancak, arama alanını biraz daha dar tutmak amacıyla Scope bildirimlerinde de bulunabiliriz. Bir başka deyişle, ağ üzerinde birden fazla servisin arandığı durumlarda kapsama alanımızı, ekleyeceğimiz Scope kriterlerine göre azaltma şansımız bulunmaktadır. Bir şekilde istemcinin ilgi alanınıda daha kesin çizgilerle belirlemiş olmaktayız. Peki bunu nasıl uygulayabiliriz?

Konunun servis tarafında yine konfigurasyon seviyesinde değerlendirilmesi gerekmektedir. Bu amaçla endpointDiscovery isimli bir endpoint davranışının kullanılması ve içerisinde gerekli scope tanımlamasının yapılması gerekmektedir. Bu amaçla daha önceden geliştirmiş olduğumuz servis örneğinde yer alan app.config dosyasına aşağıdaki davranış eklemelerini yaptığımızı düşünelim.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <system.serviceModel>
    <behaviors>
      <serviceBehaviors>
        <behavior>
          <serviceDiscovery />
          <serviceMetadata/>
        </behavior>
      </serviceBehaviors>
      <endpointBehaviors>
        <behavior name="epBehavior">
          <endpointDiscovery>
            <scopes>
              <add scope="http://www.adventure.com/Math/Calculus"/>
            </scopes>
          </endpointDiscovery>
        </behavior>
      </endpointBehaviors>
    </behaviors>    
    <services>
      <service name="ServerApp.CalculusService">
        <endpoint address="" binding="basicHttpBinding" contract="ServerApp.ICalculus" behaviorConfiguration="epBehavior" />
        <endpoint address="Mex" kind="mexEndpoint" />
        <endpoint name="udpDiscovery" kind="udpDiscoveryEndpoint" />
      </service>
    </services>
  </system.serviceModel>
</configuration>
```

Dikkat edileceği üzere endpoint davranışlarının tanımlandığı alanda, endpointDiscovery elementi içerisinde basit bir scope tanımlaması yapılmaktadır. scopes elementi içerisinde n sayıda scope tanımlaması olabilir. Tanımlamaların artması elbetteki kapsama alanının aranması için daha dar bir kriterin oluşturulmasına neden olacaktır. Bu daralma istemcinin arama operasyonu için aslında bir avantaj olarak düşünülebilir. Tanımlanan bu discovery davranışının hangi endpoint için ele alınacağı ise yine endpoint elementi içerisindeki behaviorConfiguration niteliği (attribute) yardımıyla sağlanmaktadır. Peki buna göre istemci tarafını nasıl kodlamalıyız? İşte istemci tarafındaki kod yapısının yeni hali...

```csharp
using System;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using ClientApp.CalculusSpace;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Başlamak için tuşa basın");
            Console.ReadLine();

            DiscoveryClient disClient = new DiscoveryClient("udpDiscovery");

            // Arama kriteri oluşturuluyor. Parametre olarak servis sözleşmesini içeren Interface verilmekte
            FindCriteria findCriteria = new FindCriteria(typeof(ICalculus));
            findCriteria.Scopes.Add(new Uri("http://www.adventure.com/Math/Calculus"));

            #region Asenkron erişim

            // Standart olay bazlı asenkron erişim tekniği kullanılır.

            disClient.FindCompleted += delegate(object sender, FindCompletedEventArgs e)
            {
                // Hata varsa bildir
                if (e.Error != null)
                {
                    Console.WriteLine(e.Error.Message);
                }
                else if (e.Cancelled == true) // İşlem iptal edilmişse bildir
                {
                    Console.WriteLine("İşlem iptali");
                }
                else // Aksi durumda işlemleri yürüt ve servis operasyonunu elde edilen adres üzerinden çalıştır
                {
                    FindResponse findResponse = e.Result;
                    EndpointAddress epAddress = findResponse.Endpoints[0].Address;

                    // Bulunan endPoint adresi, proxy' nin üretilmesinde kullanılıyor
                    CalculusClient client = new CalculusClient("CalculusEndpoint", epAddress);

                    Console.WriteLine("{0} adresi üzerinden çağrı yapılacaktır", epAddress.Uri.ToString());
                    double result = client.Sum(3, 5);
                    Console.WriteLine("{0} + {1} = {2}", 3, 5, result.ToString());
                }
            };

            disClient.FindAsync(findCriteria);
            Console.WriteLine("Arama işlemi başladı");
            Console.ReadLine();

            #endregion
        }
    }
}
```

Görüldüğü gibi tek yaptığımız Scopes koleksiyonuna yeni bir Uri bilgisini, FindCriteria nesne örneği üzerinden eklemektir. Aslında buradaki metod parametresinin Uri tipinden olması, servis tarafındaki scope niteliğine neden bir url formatı yazdığımızı açıklamaktadır. Uygulamamızı bu haliyle çalıştırdığımızda yine bir önceki örnekte olduğu gibi, servisin keşfedilip bulunduğunu ve başarılı bir şekilde çalıştırıldığını görürüz.

![blg68_Runtime.gif](/assets/images/2009/blg68_Runtime.gif)

Özetle Scope eklentileri sayesinde istemcinin, servis keşfi yapması için gerekli ayarları tabir yerinde ise akord etmesi ve gerçekten ilgilendiği alanlara ait servisleri araması mümkün hale gelebilmektedir. Konu ile ilişkili olarak örneğin son halini link olarak vermiyorum. Lütfen burada yazılanları oraya taşıyıp denemekten üşenmeyiniz

![Laughing](/assets/images/2009/smiley-laughing.gif)

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.