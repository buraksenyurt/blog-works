---
layout: post
title: "WCF 4.0 Yenilikleri - Default Protocol Mapping [Beta 1]"
date: 2009-08-10 23:09:00 +0300
categories:
  - wcf-4-0-beta-1
tags:
  - wcf-4-0-beta-1
  - csharp
  - xml
  - dotnet
  - wcf
  - http
  - authorization
  - visual-studio
---
Bir önceki blog yazımızda, WCF 4.0 ile birlikte gelebilecek özelliklerden birisi olan Default EndPoints kavramına değinmeye çalışmıştık. Durumu kısaca özetleyip, bu konu ile bağlantılı olan başka bir yenileğe bakarak devam edelim. Default EndPoints özelliği sayesinde, WCF çalışma zamanına (Runtime) açık bir şekilde EndPoint bildirimi yapma zorunluluğumuz ortadan kalkmaktaydı. Bir başka deyişle config dosyalarında veya kod bazında herhangibir EndPoint bildirimi yapmasak dahi, WCF çalışma zamanı, ServiceHost nesnesinde bildirilen Uri bilgilerine göre varsayılan iletişim noktalarını üretmekteydi. Ancak bir geliştirici gözü ile olaya yaklaştığımızda, Uri içerisinde yer alan string bilgiden nasıl yararlanılabildiği, yararlanıldıysada neye göre varsayılan bağlayıcı tiplerin (Binding Type) seçildiği bir soru işareti oluşturmaktadır.

.Net gibi gelişmiş Framework altyapılarında, sistemin geneline yönelik olarak kullanılan pek çok ayarlama bilindiği üzere basit konfigurasyon dosyalarında saklanmaktadır..Net Framework tarafında machine.config dosyası ile makinedeki tüm.Net uygulamaları için geçerli olan konfigurasyon ayarları saklanır. Diğer taraftan uygulamalarımızda kullanabileceğimiz config dosyaları ile (app.config, web.config gibi), machine.config üzerinden gelen bilgilerin bazıları ezilebilir. Hatta bildiğiniz üzere Web uygulamalarında hiyerarşik olarak yerleştirilebilen web.config dosyalarından yararlanılarak en alttan üste doğru ezme (Override) işlemleri gerçekleştirilebilmektedir (Bir klasöre ayrı authorization uygulanması için web.config dosyasını nasıl kullandığımızı hatırlayalım). Peki buradan nasıl bir sonuca varmamız gerekiyor...

Tahmin edeceğiniz üzere Default EndPoints özelliğinin kullanılabilmesi için gerekli olan tüm tanımlamalar ve ayarlamalar aslında.Net Framework 4.0' a ait olan maching.config dosyası içerisinde tutulmaktadır.

![blg60_ProtocolMapping.gif](/assets/images/2009/blg60_ProtocolMapping.gif)

Şekildende görüleceği üzere scheme niteliğinde çeşitli iletişim protokollerine göre bazı anahtar kelimelere yer verilmiştir. Buna karşılık olarak çalışma zamanında varsayılan olarak hangi bağlayıcı tipin kullanılacağı ise binding niteliğinde belirlenmektedir. Dolayısıyla WCF çalışma zamanı, config içerisinde veya kod tarafında bilinçli olarak tanımlanmış EndPoint verileri ile karşılaşmassa, maching.config içerisindeki protocolMapping eşleşme tablosunu baz alarak varsayılan EndPoint bildirimlerini, söz konusu servis için oluşturacaktır. Tam bu noktada akıllara gelen soru şu olacaktır; Acaba maching.config dosyasındaki bu içeriği değiştirebilir yada uygulamalarda ezebilir miyiz?

Bir geliştirici olarak bunun olmasını bekleriz. Gerçektende her iki durumda mümkündür. Machine.config içerisinde yer alan protocolMapping elementine ait alt boğumları (Child Nodes) değiştirebilir yenilerini ekleyebiliriz. Örneğin tüm http tabanlı adreslerin aslında WebHttpBinding tarafından ele alınmasını sağlayabiliriz (O makinede sadece WCF RESTful servislerin barındırıldığını düşünün). Tabiki machine.config içerisinde yapılan tüm ayarlamalar, bu dosyayı kullanan makinedeki tüm WCF servis uygulamaları için geçerli olacaktır. Bir diğer seçenek olarakta kendi uygulamalarımız içerisinde, machinbe.config üzerinde tanımlı makine bazlı protokol eşleştirmelerini ezebiliriz. Şimdi bu durumu anlamak için aşağıdaki Console uygulamasını geliştirdiğimizi düşünelim.

> Not: Uygulamamızı.Net Framework 4.0 Beta 1 yüklü bir sistemde Visual Studio 2010 Beta 1 ile geliştirdiğimizi ve son sürümlerde farklılıkar olabileceğini hatırlatalım.

```csharp
using System.ServiceModel;
using System;

namespace DefaultBindings
{
    // Servis sözleşmesi
    [ServiceContract]
    interface ICalculus
    {
        [OperationContract]
        double Sum(double x, double y);
    }

    // Servis
    class Aynstayn
        :ICalculus
    {
        public double Sum(double x, double y)
        {
            return x + y;
        }
    }

    // Client
    class Program
    {
        static void Main(string[] args)
        {
            // Http ve Tcp bazlı iki adres bildirilir.
            ServiceHost host = new ServiceHost(typeof(Aynstayn),
                new Uri("net.tcp://localhost:5000/Calculus"),
                new Uri("http://localhost:5001/Calculus")
                );
            // Servis açılır
            host.Open();

            Console.WriteLine("Host {0}", host.State);

            // Varsayılan olarak eklenen EndPoint tipleri listelenir
            foreach (var endPoint in host.Description.Endpoints)
            {
                Console.WriteLine("Name {0}\n\tAddress : {1}\n\tBinding : {2}\n\tContract : {3}",endPoint.Name,endPoint.Address,endPoint.Binding.Name,endPoint.Contract.Name);
            }

            Console.WriteLine("Çıkış için bir tuşa basınız.");
            Console.ReadLine();

            // Servis kapatılır
            host.Close();
        }
    }
}
```

Bu haliyle uygulamamızı çalıştırdığımızda bir önceki blog yazımızda olduğu gibi varsayılan EndPoint'lerin eklendiği gözlemlenecektir.

![blg60_DefaultRun.gif](/assets/images/2009/blg60_DefaultRun.gif)

Şimdi uygulamamıza bir app.config dosyası eklediğimizi ve içeriğini aşağıdaki gibi geliştirdiğimizi varsayalım.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <system.serviceModel>
    <protocolMapping>
      <!-- Eğer http protokolü ile karşılaşılırsa wsHttpBinding bağlayıcı tipini varsayılan olarak kullan. Machine.config içerisindeki http tanımlaması ezilmiştir.-->
      <!--<clear/>--> <!-- Clear kullanıdığımız takdirde, machine.config içerisindeki tüm protocolMapping eşleştirmeleri geçersiz kılınır. Dolayısıyla bu örnekte yer alan net.tcp bazlı Uri için, bu config ayarlarına göre otomatik bir EndPoint noktası üretilmez -->
      <add scheme="http" binding="wsHttpBinding"/>      
    </protocolMapping>
  </system.serviceModel>
</configuration>
```

Bu tanımlamaya göre, Uri bilgisinde http protokolü ile karşılaşıldığında varsayılan olarak WsHttpBinding bağlayıcı tipinin kullanılması söylenmektedir. Aynı örneği, bu config ayarına göre çalıştırdığımızda aşağıdaki sonucu alırız.

> Not: binding niteliğinin (attribute) değeri case-sensitive'dir. Yani wsHttpBinding yerine örneğin WsHttpBinding yazıldığı takdirde çalışma zamanında System.ServiceModel.CommunicationObjectFaultedException tipinden bir istisna (Exception) alınır.

![blg60_AfterMapping.gif](/assets/images/2009/blg60_AfterMapping.gif)

Görüldüğü gibi, http protokolü için varsayılan olarak WsHttpBinding bağlayıcı tipini kullanan bir EndPoint üretilmiştir. Tabiki bu ayarlamalar sırasında dikkat edilmesi gereken bazı noktalarda vardır. Söz gelimi WsHttpBinding ile BasicHttpBinding arasındaki farklar göz önüne alındığında, servis tarafınında buna göre geliştiriliyor olması gerekir. Bunu en güzel açıklayan durumlardan birisi RESTful için WebHttpBinding kullanılması olarak düşünülebilir. Nitekim varsayılan bağlayıcı tipi olarak WebHttpBinding seçildiğinde, servis operasyonlarında WebGet veya WebInvoke gibi niteliklerin kullanılması önemlidir. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[DefaultBindings.rar (24,20 kb)](/assets/files/2009/DefaultBindings.rar)