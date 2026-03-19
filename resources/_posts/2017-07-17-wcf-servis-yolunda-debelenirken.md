---
layout: post
title: "WCF Servis Yolunda Debelenirken"
date: 2017-07-17 21:47:00 +0300
description: "Geçtiğimiz günlerde çalıştığım turuncu bankada bölümüm değişti. Yeni bölümümdeki ilk görevim ise ServiceStack yerini alabilecek bir çatının oluşturulması konusunda bir takım POC çalışmalarının yapılması. Önemli hedeflerden birisi Servislerin IIS bağımsız dinamik olarak ayağa kaldırılması ve servislere gidip gelen mesajların yakalanabildiğinin görülmesi. Epey zamandır WCF alt tarafında çalışmamıştım. Pas tuttuğumu itiraf edeyim. Tabii yazının konusu devam etmekte olan POC çalışmasının tamamını anlatmak değil ancak dinamik olarak host edilen servislere gelen ve servisten dönen mesajları nasıl yakalayabiliriz bunun bir yolunu göstermeye çalışacağım."
categories:
  - wcf
tags:
  - wcf
  - csharp
  - dotnet
  - xml
  - json
  - iis
  - authentication
  - authorization
  - reflection
  - generics
  - testing
---
# WCF Servis Yolunda Debelenirken
![wcf_message_8.gif](/assets/images/2017/wcf_message_8.gif)

Merhaba Arkadaşlar,

Geçtiğimiz günlerde çalıştığım turuncu bankadaki bölümüm değişti. İsmini halen ezberleyemediğim Yazılım Geliştirme Sistemleri ve Platform Uygulamaları bölümünde yaşamımı sürdürmeye devam ediyorum. Yeni bölümümdeki ilk görevim ise ServiceStack yerini alabilecek bir çatının oluşturulması konusunda bir takım POC çalışmalarının yapılması. Önemli hedeflerden birisi WCF (Windows Communication Foundation) servislerinin IIS (Internet Information Services) bağımsız olarak dinamik bir şekilde ayağa kaldırılması ve istemci ile sunucu arasındaki mesajların yakalanarak kayıt altına alınabildiğinin görülmesi.

Epey zamandır WCF ile çalışmadığımdan baya pas tuttuğumu itiraf etmek isterim. Yazının konusu, devam etmekte olan POC (Proof of Concept) çalışmasının tamamını anlatmak değil ancak dinamik olarak host edilen servislere gelen ve servisten dönen mesajları nasıl yakalayabiliriz bunun bir yolunu bulmaya çalışmak. Hatta bu konuda çok yakın bir zamanda sevdiğim bir dostumun da sorusu olmuştu. Entegre olunan bir servise gelip giden mesajları nasıl yakalayabiliriz. Normal şartlarda WCF'in Trace ve Logging mekanizmalarını kullanarak bu mümkün ve oldukça kolay ama hedef buradaki takibi kontrol atlına almak. Yani mesajları yakaladığımız yerlerde araya girerek başlangıç için sadece loglamak (örneğin Console'a yazdırmak)

Çözümün Kısa Bir Özeti

Solution içeriği genel hatları ile aşağıdaki gibi.

![wcf_message_1.gif](/assets/images/2017/wcf_message_1.gif)

SDK klasörü içerisinde diğer servis geliştiriciler için temel bir sözleşme sunmayı planladım. Aşağıdaki gibi bir arayüz (Interface) tipim var örneğin.

```csharp
using System.ServiceModel;

namespace ING.ServiceFabric.SDK
{
    [ServiceContract]
    public interface ITunnelContract
    {
        [OperationContract]
        TunnelResponse Execute(TunnelRequest request);
    }
}
```

Hatta ISV klasöründeki projeler bu SDK'yı kullanarak geliştirilmiş örnek servis kütüphaneleri de içermekte. Aşağıdaki kod parçasında örnek bir uygulamasını görebilirsiniz. ITunnelContract arayüzü ServiceContract ve OperationContract nitelikleri sayesinde FraudCheckService tipine WCF Servis özelliğini kazandırmakta.

```csharp
using ING.ServiceFabric.SDK;

namespace DAEXServiceLibrary
{
    public class FraudCheckService
        :ITunnelContract
    {
        public TunnelResponse Execute(TunnelRequest request)
        {
            return new TunnelResponse
            {
                 Output="Fraud check for customer"
            };
        }
    }
}
```

TEST klasöründe tahmin edileceği üzere Unit Test ve benzeri Console uygulamaları yer almakta.

JSON Bazlı Konfigurasyon

HOST isimli klasörde yer alan ServiceFabric projesinde bir Assembly içerisinde duran servislerin ayağa kaldırılması ile ilgili işlemler yer alıyor. Ama nasıl? Kısaca neler yapmaya çalıştığımı anlatayım.

WCF'in standart konfigurasyon sistemi config uzantılı dosyaları kullanmakta. Bir Web uygulaması söz konusu ise web.config diğerleri içinse app.config ağırlıklı olarak kullanılıyor. Bu davranışı değiştirmenin bir yolu var mı henüz bilmiyorum ama ServiceHost tipi ile servisleri dinamik olarak çalışma zamanında ayağa kaldırabildiğimizi ve bir takım ayarları kod tarafında yapabildiğimizin farkındayım. Bu nedenle servislere ait çalışma zamanı ayarlarını JSON formatında bir konfigurasyon dosyası olarak tutmaya çalıştım. Aşağıdaki gibi örnek bir JSON içeriğini kullanıyorum.

![wcf_message_3.gif](/assets/images/2017/wcf_message_3.gif)

İçeriği [jsoneditoronline.org](https://www.buraksenyurt.com/admin/app/editor/jsoneditoronline.org) üzerinden oluşturmaya çalıştım. Nesne yapısını kurgulamam şimdilik yeterliydi.

![wcf_message_2.gif](/assets/images/2017/wcf_message_2.gif)

Tabii projenin ilerleyen günlerinde bu JSON içeriğini oluşturacak ve okuyacak sınıfları sisteme dahil etmeyi de ihmal etmedim.

```csharp
using ING.ServiceFabric.ConfigurationTypes;
using Newtonsoft.Json;
using System.IO;

namespace ING.ServiceFabric
{
    public class HostPackManager
    {
        public static HostPack ReadPack(string packFile)
        {
            HostPack pack = JsonConvert.DeserializeObject<HostPack>(File.ReadAllText(packFile));
            Environment environment = JsonConvert.DeserializeObject<Environment>(File.ReadAllText(pack.EnvironmentConfig));
            pack.AssemblyName = Path.Combine(environment.DllRootPath, pack.AssemblyName);

            return pack;
        }

        public static string WritePack(HostPack pack, string packFilePath)
        {
            string jsonContent=JsonConvert.SerializeObject(pack);
            File.WriteAllText(packFilePath,jsonContent);
            return jsonContent;
        }
    }
}
```

Burada yine detaya girmeyeceğim ancak JSON içeriğini yönetimli kod tarafında daha kolay idare etmek için HostPack ve Environment gibi sınıflar da yer almakta. Aynen.Net'in XML odaklı konfigurasyon dosyalarına olan yaklaşımı gibi. Her section'a karşılık gelecek bir sınıf.

Konfigurasyon dosyasının içeriğinde tutulan bilgileri nasıl kullanmak istediğime gelince. Her şeyden önce çalışma zamanında yüklenecek olan servisleri bir klasördeki dll'lerden almak istiyorum. Yani Host uygulama kullanacağı servisleri projeye referans etmeye gerek duymadan ayağa kaldıracak. Bu nedenle içerde kullanılacak dll bilgisini ve başka çevresel değişkenleri tutan bir dosya bilgisini tutmayı düşündüm. Kullanacağım ServiceHost tipinin bir BaseAddress ihtiyacı da olacak. Bunların dışında host'un sunacağı servisleri de bir şekilde tanımlamam gerekiyor. Servisin tip adı dışında Address Binding Contract üçlemesini de burada tutuyorum. Her servis için Metadata paylaşımı olacak mı, çalışma zamanındaki Exception detayları basılacak mı gibi aşina olduğumuz bilgileri de ilgili alanlarda tutmaktayım. Bu içeriğe göre FraudCheckService WSHttpBinding ile host edilecek. Diğer yandan henüz sertifika tanımlamamalarını entegre edecek kodları yazamadığımdan BasicHttpsBinding kullanan servisi test edememekteyim.

ServiceHost Türevli TowerHost

Genel hatları ile konfigurasyon bilgisini tutmayı bu şekilde kurgulamaya çalıştım. ServiceHost türevli tipin içeriği ise aşağıdaki şekilde.

```csharp
using System;
using System.ServiceModel;

namespace ING.ServiceFabric
{
    public class TowerHost
        :ServiceHost
    {
        public TowerHost(Type serviceType,params Uri[] baseAddresses)
            :base(serviceType,baseAddresses)
        {
        }
    }
}
```

TowerHost sınıfı tipik olarak ServiceHost tipinden türemekte ve base kullanımı ile yapıcı metoduna gelen parametreleri doğrudan ServiceHost tipinin uygun yapıcısına aktarılmakta. Burada sonradan override etmeyi düşündüğüm üst sınıf üyeleri olacak. Şimdilik bu sade haliyle kalması yeterli. Gelelim asıl işi yapan TowerHostFactory sınıfına.

```csharp
using ING.ServiceFabric.ConfigurationTypes;
using ING.ServiceFabric.EndpointBehaviors;
using ING.ServiceFabric.SDK;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace ING.ServiceFabric
{
    public class TowerHostFactory
    {
        public List<TowerHost> CreateTowerHost(string packFile)
        {
            List<TowerHost> hostList = new List<TowerHost>();
            HostPack pack=HostPackManager.ReadPack(packFile);            
            var assembly = Assembly.LoadFile(pack.AssemblyName);
            foreach (var service in pack.Services)
            {
                ServiceInfo sInfo = GetServiceInfo(service);
                var host=CreateTowerHost(assembly, sInfo);
                hostList.Add(host);
            }

            return hostList;
        }

        private TowerHost CreateTowerHost(Assembly assembly,ServiceInfo serviceInfo)
        {
            object service = assembly.CreateInstance(serviceInfo.TypeName);
            
            var host = new TowerHost(service.GetType(),new Uri(serviceInfo.Address));
            var bindingTypeName = string.Format("System.ServiceModel.{0}", serviceInfo.BindingName);
            var serviceModelAssembly = Assembly.GetAssembly(typeof(BasicHttpBinding));
            Binding bindingInstance = (Binding)serviceModelAssembly.CreateInstance(bindingTypeName);             
            var endPoint=host.AddServiceEndpoint(typeof(ITunnelContract), bindingInstance,serviceInfo.Address);
            endPoint = SetMetadataBehavior(serviceInfo, host, bindingInstance, endPoint);
            //endPoint = SetServerCertificate(serviceInfo, host,endPoint);
            host.Description.Behaviors.Find<ServiceDebugBehavior>().IncludeExceptionDetailInFaults = serviceInfo.IncludeExceptionDetails;
            endPoint.EndpointBehaviors.Add(new EndpointMessageInspectorBehavior());

            return host;
        }

        private ServiceEndpoint SetServerCertificate(ServiceInfo serviceInfo,TowerHost host,ServiceEndpoint endpoint)
        {
            //host.Credentials.ServiceCertificate.SetCertificate()            
            throw new NotImplementedException();           
        }

        private static ServiceEndpoint SetMetadataBehavior(ServiceInfo serviceInfo, TowerHost host, Binding bindingInstance, ServiceEndpoint endPoint)
        {
            ServiceMetadataBehavior metadataBehavior = new ServiceMetadataBehavior();
            host.Description.Behaviors.Add(metadataBehavior);

            if (serviceInfo.BindingName != "System.ServiceModel.NetTcpBinding")
            {
                if (bindingInstance.Scheme == "https")
                {
                    metadataBehavior.HttpsGetEnabled = serviceInfo.MetadataEnabled;
                    metadataBehavior.HttpsGetUrl = new Uri(string.Format("{0}/mex", serviceInfo.Address));
                }
                else
                {
                    metadataBehavior.HttpGetEnabled = serviceInfo.MetadataEnabled;
                    metadataBehavior.HttpGetUrl = new Uri(string.Format("{0}/mex", serviceInfo.Address));
                }
            }
            else
            {
                endPoint = host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), serviceInfo.Address);
            }
            return endPoint;
        }

        private static ServiceInfo GetServiceInfo(ServiceInfo service)
        {
            ServiceInfo sInfo = new ServiceInfo();
            sInfo.Address = service.Address;
            sInfo.BindingName = service.BindingName;
            sInfo.IncludeExceptionDetails = service.IncludeExceptionDetails;
            sInfo.MetadataEnabled = service.MetadataEnabled;
            sInfo.TypeName = service.TypeName;
            return sInfo;
        }
    }
}
```

Bu sınıfta yapılan bazı kritik işler var ama kod epey dağınık halde diyebilirim. List döndüren CreateTowerHost metodunun görevi oldukça basit. Parametre olarak gelen packFile bilgisini alıyor, JSON konfigurasyon içeriğini okuyor, tanımlı olan Assembly'ı yüklüyor ve konfigurasyon da belirtilen her bir servis tipi için birer TowerHost nesne örneği üretip listeye ekliyor. TowerHost tipini döndüren ikinci fonksiyon biraz daha karmaşık. Az biraz reflection ile parametre olarak gelen servis tipini örnekleyip, JSON dosyasından okunup ServiceInfo sınıfına alınan değerlere bakarak ayarlamalar yapmakta. Söz gelimi gerekli Binding tipini üretiyor, EndPoint oluşturuyor, Metadata Publishing değerlerini ve IncludeExceptionDetailsInFault bilgisini set ediyor. Metadata davranışının eklenmesi üzerine de halen çalışmaktayım. Nitekim NetTcpBinding söz konusu olduğunda IMetadataExchange arayüzünün kullanılarak bir publishing yapmak gerekiyor. Başka Binding tiplerinde farklı davranışlar sergilenmesi de gerekebilir. Her ne kadar if kullanmayı sevmesemde, POC olmasının verdiği rahatlıkla böyle bir kod parçası da eklemiş bulundum (:

Mesajların Yakalanması

Yazının ana konusu olan mesaj yakalama kısmı ise şu satırda gerçekleştiriliyor.

```csharp
endPoint.EndpointBehaviors.Add(new EndpointMessageInspectorBehavior());
```

O anki EndPoint bilgisine, EndpointMessageInspectorBehavior tipinden bir nesne örneği davranış olarak ekleniyor. Yani Endpoint'e özel bir davranış ekleyerek genişletiyoruz. İçeriği basitçe aşağıdaki gibi.

```csharp
using ING.ServiceFabric.Dispatchers;
using System.ServiceModel.Description;

namespace ING.ServiceFabric.EndpointBehaviors
{
    public class EndpointMessageInspectorBehavior
        : IEndpointBehavior
    {

        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {            
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new MessageInspector());
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }
}
```

Henüz sadece ApplyDispatchBehavior metodu kullanılmakta. Bu metoda gelen endpointDispatcher nesnesi üzerinden çalışma zamanında oluşan servis kanalına gidip araya giriyoruz. Bunu yaparken de MessageInspectors koleksiyonuna yeni bir dinleyici ekliyoruz.

```csharp
using System;
using System.ServiceModel.Dispatcher;

namespace ING.ServiceFabric.Dispatchers
{
    public class MessageInspector
        :IDispatchMessageInspector
    {
        public object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel, System.ServiceModel.InstanceContext instanceContext)
        {            
            Console.WriteLine("In AfterReceiveRequest");
            Console.WriteLine("\t{0}",request.ToString());

            return null;
        }

        public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            Console.WriteLine("In BeforeSendReply");
            Console.WriteLine("\t{0}",reply.ToString());
        }
    }
}
```

IDispatchMessageInspector arayüzünden türeyen MessageInspector sınıfının uyguladığı iki operasyon var. AfterReceiveRequest ve BeforeSendReply. AfterReceiveRequest ile servisin ilgili EndPoint'inden geçen mesajı yakalıyoruz. BeforeSendReply ise istemciye dönen mesaj gitmeden önce devreye girmekte. Ben sonuçları görmek için ilgili bilgileri Console'a basıyorum. Hedef pek tabii etkili bir Log mekanizması ile ilgili mesajları kayıt altına almak. Burada mesaj içeriğine bakılarak daha pek çok aksiyon da alınabilir gibime geliyor.

> Aslında WCF'in çalışma zamanındaki işleyişini gösteren [Microsoft dokümanının](https://opbuildstorageprod.blob.core.windows.net/output-pdf-files/en-us/VS.core-docs/live/articles/framework/wcf/extending.pdf) 19ncu sayfasındaki grafiğe bakınca olay daha kolay anlaşılıyor. Burada EndpointDispatcher'ın yaşamı boyunca enjekte edilebilecek bir çok enstrüman görülmekte.
> ![wcf_message_4.gif](/assets/images/2017/wcf_message_4.gif)

Çalışma Zamanı

Unit Test projesi içerisinde pek çok test metodu var tabii ama benim için en güzel test ortamı tabii ki sevimsiz Console penceresi. Bu Console projelerinden birisi JSON dosyasından okuduğu bilgileri kullanarak servisleri ayağa kaldırırken diğeri istemci rolünü üstlenmekte ve örnek bir servise mesaj atıp cevap almakta. Host uygulamayı şu şekilde geliştirdim.

```csharp
using ING.ServiceFabric;
using System;

namespace StandAloneHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var packPath = "c:\\c\\ISV\\daexHost.json";
            var hostFactory = new TowerHostFactory();
            var hostList = hostFactory.CreateTowerHost(packPath);
            foreach (var host in hostList)
            {
                host.Open();
                Console.WriteLine("{0},{1}",host.Description.Name,host.State);
            }
            Console.WriteLine("{0} adet host dinlemede. Host'ları kapatmak için bir tuşa basınız",hostList.Count);
            Console.ReadLine();
            foreach (var host in hostList)
            {
                host.Close();
                Console.WriteLine("{0},{1}",host.Description.Name,host.State);
            }            
        }
    }
}
```

Tabii önce bu uygulamayı çalıştırıp ayağa kalkan bir servise ait WSDL içeriği geliyor mu bir bakmak ve bu içeriği kullanarak istemciye Proxy üretmek gerekiyordu. localhost:5000/daex/FraudCheckService adresinden yayın yapan WSHttpBinding bazlı servisi ayağa kaldırdığımda servise ulaşabildiğimi ve WSDL içeriğini yakalayabildiğimi gözlemledim.

![wcf_message_5.gif](/assets/images/2017/wcf_message_5.gif)

ve wsdl içeriği

![wcf_message_6.gif](/assets/images/2017/wcf_message_6.gif)

Nihayetinde bir klasörde tutulan dll içerisindeki servisleri ayağa kaldırıp bunlara gelen istemci taleplerini ve dönen cevapları yakalayabilmeyi başardığımı ifade edebilirim.

![wcf_message_7l.gif](/assets/images/2017/wcf_message_7l.gif)

POC çalışması üzerinde halen devam etmekteyim. Yapmam gereken çok şey var. WCF'in standart konfigurasyon yapısı düşünüldüğünde çok daha hafif bir çatı kurmaya çalışıyorum. Sıradaki hedefler arasında Authentication ve Authorization gibi Cross Cutting'lerin çalışma zamanındaki servis yoluna nasıl enjekte edilebileceği konusu var. Özetle yazılımcıların geliştireceği her bir servis kütüphanesinin kendi HostPack.json içeriğine sahip olacağı bir dünyanın peşinden koştuğumu ifade edebilirim. Sadece ihtiyaç duyduğumuz çalışma zamanı davranışlarının var olan standart WCF çatısından farklılaştırılarak entegre edildiği hafif bir çatı. İşin aslı burada daha yeni dünyaları denemek isterdim. Söz gelimi bu servis çatısını GO dilini kullanarak geliştirmek ve performansın gerçekten de söylendiği kadarı yüksek olup olmadığını görmek isterdim. Bakalım nelerle karşılaşacağım. POC üzerinde ilerledikçe pek çok sorunla karşılaşıyor ve çözmeye çalışırken yeni yeni şeyler öğreniyorum. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.