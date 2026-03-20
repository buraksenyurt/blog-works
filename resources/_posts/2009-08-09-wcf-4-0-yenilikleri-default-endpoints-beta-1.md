---
layout: post
title: "WCF 4.0 Yenilikleri - Default EndPoints [Beta 1]"
date: 2009-08-09 16:51:00 +0300
categories:
  - wcf-4-0-beta-1
tags:
  - wcf-4-0-beta-1
  - csharp
  - dotnet
  - wcf
  - xml
  - web-service
  - xml-web-services
  - http
  - visual-studio
---
Çok eskinden.Net Remoting ile ilişkili uygulamalarda çalışırken, servis ve istemci taraflarının konfigurasyon dosyası bazlı ayarlamaları sırasında, Visual Studio.Net 2003 intelli-sense özelliğinin kaybolduğuna şahit olmuştum. Gerçektende config dosyası içerisindeki intelli-sense özelliği belirli bir elemente kadar destek veriyor ama sonrasında kayboluyordu. Böyle bir durumda pek çok ayarlamayı ezbere yapmak zorunda kaldığımı hatırlıyorum.

![blg59_Giris.jpg](/assets/images/2009/blg59_Giris.jpg)

Bu durum,.Net Remoting tabanlı dağıtık uygulamaların (Distributed Applications) TCP bazlı hızlı bir iletişim sağlama avantajını kimi zaman göz ardı ettirebilen bir zorluktur. Nitekim ezbere kod yazmak, hiç bir zaman iyi bir şey değildir. Özellikle işlerin arap saçına dönmesine neden olabilir.

Gel gelelim bir başka dağıtık uygulama geliştirme modeli olan Xml Web Servislerinde,.Net Remoting için karşılaştığımız ayarlama zorluklarını göremeyiz. Öyleki, WebService ve WebMethod niteliklerinin (Attributes) kullanılması yeterli olmaktadır. Çünkü çalışma zamanı, bu niteliklere göre otomatik olarak servis tarafını ayağa kaldırır.

Ancak WCF (Windows Communication Foundation) tarafına geçtiğimizde konfigurasyon tarafında göz önüne alınması gereken çok fazla şey olduğunu gördük. WCF'in pek çok dağıtık uygulama geliştirme modelini tek bir çatı altında birleştirmesinin oluşturduğu zorluklardan biriside, çok fazla ince ayarı içermesi olarak düşünülebilir. Hal böyle olunca WCF takımı boş durmamış ve 4.0 versiyonunda daha kolay konfigrasyon yapılabilmesini sağlamak adına bir takım geliştirmelerde bulunmuştur.(Örneklerimizi Visual Studio 2010 Beta 1 ve.Net Framework 4.0 Beta 1 üzerinde geliştirdiğimizden, Release sürümde bir takım değişiklikler veya farklılıklar olabileceğini hatırlatmak isterim)

Bu geliştirmelerden birisi DefaultEndPoints kavramıdır. Bu özelliği, varsayılan olarak EndPoint adreslerinin biz söylemeden çalışma zamanına entegre edilmesinin sağlanması olarak düşünebiliriz. Aslında olaya.Net 3.0/3.5 açısından bakmamızda yarar vardır. Bu nedenle.Net 3.5 tabanlı geliştirilmiş aşağıdaki Console uygulamasını göz önüne alalım.

```csharp
using System;
using System.ServiceModel;

namespace OldStyle
{
    [ServiceContract]
    interface IProductService
    {
        [OperationContract]
        double GetExpensiveProduct(int categoryId);
    }

    class ProductService
        : IProductService
    {
        public double GetExpensiveProduct(int categoryId)
        {
            Random rnd = new Random();
            double price = rnd.NextDouble() * 100;
            return price;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(
                typeof(ProductService),
                new Uri("net.tcp://localhost:1500/adventure/"),
                new Uri("http://localhost:1400/adventure/")
                );

            // Herhangibir configurasyon tanımlaması ve özellikle EndPoint bildirilmeleri yapılmadığı için Open metodundan sonra çalışma zamanı istisnası alınır(Runtime Exception)
            // Dolayısıyla ya kod tarafında yada config dosyasında EndPoint bildirimleri yapılmalıdır.
            host.Open();
        }
    }
}
```

Uyulamada basit bir servis kullanılmaktadır. ServiceHost nesne örneğinin oluşturulması sırasında, iki farklı Uri bilgisi verildiğine dikkat etmeliyiz. Bunlardan birisi Tcp bazlı diğeri ise Http bazlı iletişimleri desteklemektedir. ServiceHost nesnesi, örneklenmesinin ardından istemcilerden gelecek talepleri dinlemek üzere Open metodu ile açılmaktadır. Ancka örneği çalıştırdığımızda aşağıdaki ekran görüntüsü ile karşılaşırız.

![blg59_Exception.gif](/assets/images/2009/blg59_Exception.gif)

Hemen şunu belirteyim; uygulamamızda herhangibir konfigurasyon dosyası kullanmadık. Hal böyle olunca WCF çalışma zamanı belirtilen Uri'ler için hangi EndPoint tanımlamalarını kullanması gerektiğini bulamadı ve bir istisna fırlatarak uygulamanın sonlanmasına neden oldu. EndPoint kavramı WCF tarafının olmazsa olmaz bütünlerinden birisidir. Basit olarak servisin nerede durduğu, hangi hizmeti ve nasıl sunacağı ile ilişkili temel bilgileri içermektedir. Yani AddressBindingContract kavramından (WCF'in ABC'si) bahsediyoruz. Bir servis birden fazla EndPoint içerebilir. Her ne olursa olsun, EndPoint'lerin ya config dosyası içerisinde yada kod tarafında tanımlanıp eklenmesi gerekir. Peki aynı örneği.Net 4.0 tabanlı olarak Visual Studio 2010 üzerinde geliştirseydik.

```csharp
using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace DefaultEndPoints
{
    [ServiceContract]
    interface IProductService
    {
        [OperationContract]
        double GetExpensiveProduct(int categoryId);
    }

    class ProductService
        : IProductService
    {
        public double GetExpensiveProduct(int categoryId)
        {
            Random rnd = new Random();
            double price=rnd.NextDouble()*100;
            return price;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // ServiceHost nesnesi örneklenir
            // İki farklı adres bilgisi verilmiştir. Bunlardan birisi tcp bazlı diğeri ise http bazlıdır
            ServiceHost host = new ServiceHost(
                typeof(ProductService),
                new Uri("net.tcp://localhost:1500/adventure/"),
                new Uri("http://localhost:1400/adventure/")
                );

            // Host açılır.
            host.Open(); 
            Console.WriteLine("Servis durumu {0}\n",host.State.ToString());
            
            // Ne config içerisinde nede kod tarafında açık bir şekilde EndPoint bildirilimi yapılmamıştır. Buna rağmen çalışma zamanı ServiceHost nesnesinin yapıcı metodundaki Uri bilgilerinden yararlanarak varsayılan EndPoint bilgilerini oluşturmuştur.

            // Servis için oluşturulan EndPoint' lerin listesi alınır
            ServiceEndpointCollection endPoints = host.Description.Endpoints;

            Console.WriteLine("{0} EndPoint oluşturuldu.\n",endPoints.Count.ToString());

            // EndPoint nesnelerinin her biri dolaşılır
            foreach (var endPoint in endPoints)
            {
                // EndPoint adı, adres(Address), bağlayıcı tip(Binding Type) adı, sözleşme(Contract) adı yazdılır.
                Console.WriteLine("Name : {0} , Address : {1} , Binding : {2} , Contract : {3}\n", endPoint.Name,endPoint.Address.Uri,endPoint.Binding.Name,endPoint.Contract.Name);
            }

            Console.WriteLine("Kapatmak için bir tuşa basın");
            Console.ReadLine();

            host.Close();
        }
    }
}
```

Bu kez 4.0 ile birlikte gelen varsayılan EndPoint kavramına güvenerek, yüklenen EndPoint'lere ait bilgileride ekrana yazdırıyoruz. Ancak yine bilinçli olarak EndPoint oluşturmadığımızı veya config dosyası kullanmadığımızı belirtelim. Uygulamayı çalıştırdığımızda aşağıdaki sonuçlar ile karşılaşırız.

![blg59_Scenario1.gif](/assets/images/2009/blg59_Scenario1.gif)

Hımmm...

![Wink](/assets/images/2009/smiley-wink.gif)

Harika! Uri bilgisindeki protokol tanımalamarına bakılarak, çalışma zamanı bizim için iki farklı EndPoint bilgisini otomatik olarak oluşturmuştur. Tcp bazlı adresleme için varsayılan olarak NetTcpBinding, Http bazlı adresleme içinse varsayılan olarak BasicHttpBinding bağlayıcı tipleri oluşturulmuştur. Diğer yandan Address özelliklerinde, Uri bilgisi sonuna sözleşme tipi (Contract Type) adının eklendiğine dikkat edilmelidir. Buradan şu sonuca varabiliriz. Servis tarafında kaç sözleşme ve adres sunuluyorsa bunların çarpanı kadar EndPoint otomatik olarak oluşturulacaktır. Elbetteki biz EndPoint bildirimlerini bilinçli olarak yapmassak. Peki ya servis tarafında EndPoint bilgisini eklemişsek? Örneğin aşağıdaki kod parçasında olduğu gibi ServiceHost nesnesinin örneklenmesinden sonra AddServiceEndpoint metodunu kullanırsak...

host.AddServiceEndpoint (typeof (IProductService), new WSHttpBinding (), "");

Bu durumda aynı örneğin çalışma zamanı çıktısı aşağıdaki gibi olacaktır.

![blg59_Scenario2.gif](/assets/images/2009/blg59_Scenario2.gif)

Görüldüğü üzere çalışma zamanı sadece bizim eklediğimiz EndPoint bilgisini kullanmaktadır. Tam bu noktada WCF 4.0 ile birlikte gelen AddDefaultEndpoints metodunu değerlendirmeye çalışalım. Normal şartlarda servis tarafına EndPoint bilgilerini eklemessek, WCF çalışma zamanı, yeni gelen AddDefaultEndpoints metodunu kullanmakta ve Uri bilgilerine göre varsayılan atamaları yapmaktadır. Peki yukarıdaki gibi AddServiceEndpoint metodundan sonra birde AddDefaultEndpoints metodunu yukarıdaki örneğe göre aşağıdaki gibi kullanırsak...

host.AddDefaultEndpoints ();

Bu durumda çalışma zamanı çıktısı aşağıdaki gibi olacaktır.

![blg59_Scenario3.gif](/assets/images/2009/blg59_Scenario3.gif)

Görüldüğü üzere hem bizim bilinçli olarak eklediğimiz hemde AddDefaultEndpoints metodu nedeniyle eklenen EndPoint bilgileri yer almaktadır. Yani WCF Çalışma ortamı 3 EndPoint noktasını kullanıma açmaktadır.

Şüphesizki bu yenilik, varsayılan olarak standart EndPoint bilgilerini kullandığımız vakalarda son derece işe yarardır. Geliştiricinin işi kolaylaştırılmaktadır. Ama elbetteki pek çok gerçek hayat senaryosunda; örneğin WSDL çıktılarının yasaklandığı, iletişim seviyesinde güvenliğin (Transport Layer Security) sağlanması gerektiği veya çift taraflı haberleşmenin (Duplex Communication) olduğu durumlarda varsayılan olarak atanan EndPoint'ler dışındakilerin kullanılması gerekmektedir.

Bu kısa yazımızda WCF 4.0 tarafında, basitleştirilmiş konfigurasyon (Simplified Configuration) ayarlamalarının özelliklerinden birisi olan Default EndPoints kavramına değinmeye çalıştık. İlerleyen yazılarımızda diğer WCF 4.0 yeniliklerinede değinmeye çalışıyor olacağız. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[DefaultEndPoints.rar (40,08 kb)](/assets/files/2009/DefaultEndPoints.rar)
