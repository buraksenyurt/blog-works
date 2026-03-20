---
layout: post
title: "WCF 4.0 Yenilikleri - Default Binding Configuration [Beta 1]"
date: 2009-08-12 03:30:00 +0300
categories:
  - wcf-4-0-beta-1
tags:
  - wcf-4-0-beta-1
  - xml
  - csharp
  - dotnet
  - linq
  - wcf
  - visual-studio
---
WCF 4.0 ile birlikte gelmesi muhtemel yenilikleri incelemeye kaldığımız yerden devam ediyoruz. Bu yazımızda ele alacağımız konu, config dosyası içerisinde kullanılan bağlayıcı tipe (Binding Type) özel konfigurasyon ayarları ile ilişkili olacak. Konuyu net bir şekilde anlayabilmek için.Net Framework 3.5 tabanlı olarak geliştirilmiş basit bir servis uygulaması ile işe başlamamız gerekiyor. Uygulamamıza ait App.config dosyasının içeriği aşağıdaki gibidir.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <bindings>          
            <netTcpBinding>
              <binding name="TcpBindingConfig1" receiveTimeout="00:01:00" sendTimeout="00:00:30" maxConnections="5">
                <reliableSession enabled="true" />
              </binding>
            </netTcpBinding>
        </bindings>
        <services>
            <service name="PreviousVersion.Aynstayn">
                <endpoint address="net.tcp://localhost:5001/Calculus" binding="netTcpBinding"
                    bindingConfiguration="TcpBindingConfig1" name="TcpEndPoint1"
                    contract="PreviousVersion.ICalculus" />
                <endpoint address="net.tcp://localhost:5002/Calculus" binding="netTcpBinding"
                    bindingConfiguration="TcpBindingConfig1" name="TcpEndPoint2"
                    contract="PreviousVersion.ICalculus" />
            </service>
        </services>
    </system.serviceModel>
</configuration>
```

Bu config dosyasında iki adet Tcp bazlı EndPoint bildirimi yapıldığı görülmektedir. TcpEndPoint1 ve TcpEndPoint2. Bizim üzerinde duracağımız nokta netTcpBinding elementi ve içeriğidir. Dikkat edileceği üzere, NetTcpBinding bağlayıcı tipleri için bazı ayarlamalar yapılmıştır. Bu ayarlamalara göre ReceiveTimeout, SendTimout, MaxConnections ve ReliableSession değerleri belirlenmiştir. Söz konusu değişiklikler, config dosyası içerisinde tanımlı NetTcpBinding bağlayıcı tipini kullanan tüm EndPoint'ler için geçerli kılınabilir. Peki ama nasıl?

Dikkat edileceği üzere netTcpBinding altındaki binding elementinin name niteliği, hem TcpEndPoint1 hemde TcpEndPoint2 için bindingConfiguration elementlerinde kullanılmıştır. Böylece WCF çalışma zamanı, tanımlı olan EndPoint'lerin hangi binding ayarlarına bakacağını bindingConfiguration elementinin değerinden bulabilmektedir.

> Not: Esasında, bağlayıcı tipler için uygulamalar üzerinde configuration değerleri set edilmese dahi, WCF çalışma zamanı varsayılan bağlayıcı ayarlarını built-in olarak çekmektedir. Ancak built-in ayarlar istenirse uygulamalara ait config dosyalarında veya kod tarafında ezilebilir.

Bu config içeriğini kullanan servis uygulaması kodlarını ise aşağıdaki gibi geliştirdiğimizi düşünelim.

```csharp
using System;
using System.Linq;
using System.ServiceModel;

namespace PreviousVersion
{
    [ServiceContract]
    interface ICalculus
    {
        [OperationContract]
        double Sum(params double[] values);
    }

    class Aynstayn
        :ICalculus
    {
        public double Sum(params double[] values)
        {
            return values.Sum();
        }
    }

    // Client
    class Program
    {
        static void Main(string[] args)
        {
            // Service nesnesi örneklenir
            ServiceHost host = new ServiceHost(typeof(Aynstayn));
            
            // Servis açılır
            host.Open();

            // Tüm EndPoint' ler dolaşılır
            foreach (var endPoint in host.Description.Endpoints)
            {
                // Bu örnekte sadece NetTcpBinding kullanıldığın için Binding kontrolü yapılmadan dönüştürme işlemi yapılmıştır.
                NetTcpBinding binding = (NetTcpBinding)endPoint.Binding;              

                // Bağlayıcı tipi için ReceiveTimeout, SendTimeout, ReliableSession ve MaxConnections değerleri ekrana yazdırılır
                Console.WriteLine(
                    "Binding Name: {0}\n\tReceive Timeout : {1}\n\tSend Timeout : {2}\n\tMax Connections {3}\n\tReliable Session:{4}"
                    , binding.Name
                    , binding.ReceiveTimeout.ToString()
                    , binding.SendTimeout.ToString()
                    ,binding.MaxConnections.ToString()
                    ,binding.ReliableSession.Enabled.ToString());
            }

            Console.WriteLine("Çıkmak için bir tuşa basın");
            Console.ReadLine();

            // Servis kapatılır
            host.Close();
        }
    }
}
```

Uygulamamızı çalıştırdığımızda aşağıdaki sonuçları elde ederiz.

![blg61_PreRun.gif](/assets/images/2009/blg61_PreRun.gif)

Peki WCF 4.0 ile gelen yenilik nedir? Sakın gülmeyin ama son derece basit ve kolay

![Wink](/assets/images/2009/smiley-wink.gif)

Kolaylaştırılmış konfigurasyon (Simplified Configuration) yeniliklerine göre artık endpoint tanımlamalarında bindingConfiguration niteliğinin kullanılmasına gerek yoktur. Durumu daha net değerlendirebilmek için, yukarıdaki config içeriğini bu kez.Net Framework 4.0 örneğine göre aşağıdaki gibi değiştirdiğimizi düşünelim.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <bindings>          
            <netTcpBinding>
              <binding receiveTimeout="00:01:00" sendTimeout="00:00:30" maxConnections="5">
                <reliableSession enabled="true" />
              </binding>
            </netTcpBinding>
        </bindings>
        <services>
            <service name="PreviousVersion.Aynstayn">
                <endpoint address="net.tcp://localhost:5001/Calculus" binding="netTcpBinding"
                    name="TcpEndPoint1"
                    contract="PreviousVersion.ICalculus" />
                <endpoint address="net.tcp://localhost:5002/Calculus" binding="netTcpBinding"
                    name="TcpEndPoint2"
                    contract="PreviousVersion.ICalculus" />
            </service>
        </services>
    </system.serviceModel>
</configuration>
```

Dikkat edileceği üzere binding elementinde name veya endpoint elementinde bindingConfiguration nitelikleri kullanılmamıştır. WCF çalışma zamanı Uri bilgilerinden yola çıkarak uygun binding konfigurasyonunu bulmakta ve uygulamaktadır. Örneğimizi bu config ayarlarına göre.Net Framework 4.0 üzerinden çalıştırırsak bir önceki örnek ile aynı sonuçlar aldığımız görebiliriz.

![blg61_NextRun.gif](/assets/images/2009/blg61_NextRun.gif)

Evet. Son derece basit bir yenilik. Ancak konfigurasyon içeriğini daha okunur hale getirdiği ve basitleştirdiği ortada. Bu tekniği dilersek machine.config içerisindede kullanabiliriz. Yani machine.config içerisindeki bağlayıcı tipe özgü ayarlamalarda name niteliğini kullanmayabilir ve o makinedeki tüm uygulamaların da, bindingConfiguration niteliğini düşünmeden ilgili ayarları otomatik olarak almalarını sağlayabiliriz. WCF 4.0 ile birlikte gelen temel yeniliklere devam ediyor olacağız. Bu arada örneğimizi.Net Framework 4.0 Beta 1 ve Visual Studio 2010 Beta 1 üzerinde geliştirdiğimizi hatırlatalım. Dolayısıyla relase sürünmde bazı farklılıklar olabilir. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[DefaultBindingConfiguration.rar (47,83 kb)](/assets/files/2009/DefaultBindingConfiguration.rar)