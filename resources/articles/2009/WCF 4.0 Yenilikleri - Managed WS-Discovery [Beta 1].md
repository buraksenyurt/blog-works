---
title: "WCF 4.0 Yenilikleri - Managed WS-Discovery [Beta 1]"
pubDate: 2009-08-21 23:13:00
categories:
  - WCF 4.0 Beta 1
tags:
  - WCF 4.0 Beta 1
  - C#
  - LINQ
  - WCF
  - HTTP
  - Threading
  - Concurrency
  - Generics
---

# WCF 4.0 Yenilikleri - Managed WS-Discovery [Beta 1]
![blg70_Giris1.jpg](images/blg70_Giris1.jpg)

Merhaba Arkadaşlar,

Yandaki resimdeki gibi çok çok uzun bir yolun başında ve ulaşmanız gereken yere yüzlerce kilometre mesafede olduğunuzu hayal edin. Sabırlı bir şekilde bu yolu gidebilmek için çok iyi bir disipline sahip olmanız gerekir. Yazılım geliştirme denilen büyük okyanusun içerisinde de bu tip yollar ile karşılaşmaz mıyız? Hemde sıklıkla karşılaşırız. Yılmadan yola devam edenler, nihayetinde mutlu sona ulaşırlar. Ama belkide ulaşmazlar. Bu tamamen zamanın o andaki çevresel koşullarına bağlı olarak değişir. İşte bu yazımızda hakikaten sadece geliştirme aşaması dahi insanı çileden çıkarabilen zahmetli bir yola baş koyuyor olacağız. Hedefimiz, WS-Discovery tabanlı WCF sistemlerinde Managed Discovery modelini uygulayabilmek.

Konuyu MSDN ve diğer internet kaynaklarından araştırırken Ad Hoc modeli ile ilişkili tonlarca yazı olduğunu ama Managed tarafa pek kimsenin bulaşmak istemdiğini farkettim. Nedenini anlamam yaklaşık olarak 2,5 Litre kahve içmeme ve uykusuz bir Cumartesi gecesi geçirmeme neden oldu. Ama sonunda deydi. Aslında teorik olarak Managed modelin açıklaması son derece basit. İstemcilerin kullanmak isteyipte, farklı zamanlarda farklı lokasyonlardan ağa/ağlara dahil olan veya ayrılan servislerin keşfedilmesi görevi, istemci uygulamalardan alınıp istemci ile söz konusu servisler arasındaki başka bir Proxy servisine verilmektedir.

Proxy servisi aslında hem announcement mesajları hemde istemcilerden gelecek olan Probe taleplerini dinlemektedir. Announcement mesajların dinlenmesi, online veya offline olan servislerin, Proxy servisi üzerinde bir saklama alanında tutulmasınıda gerektirir. Nitekim proxy servisi, ağa bağlı olan veya ayrılan tüm servislere ait ortak bir listeyi barındırıp istemci taleplerini bu listedeki durumlara göre karşılamalıdır. Diğer taraftan kendiside, istemciler tarafından keşfedilebilir olmalıdır. Bu nedenle tüm istemciler için ortak bir Discovery Endpoint noktasına sahip olmalıdır. Proxy servisini bu nedenlerden dolayı sürekli online halde kalan bir hizmet olarak düşünebiliriz. Online kalması önemlidir; çünkü online olduğu sürece, ağı dinleyerek katılan servisleri listesine alabilir ve istemcilerden gelen Probe veya Resolve gibi çağrılara cevap verebilir. Peki işi zorlayan nokta nedir?

Herşeyden önce Proxy servisinin, çalışma zamanındaki hareketliliği normal bir servis gibi değildir. Yani standart ServiceHost tipi tek başına yeterli değildir. Bu nedenle, DiscoveryProxyBase isimli abstract sınıftan bir türetme işlemi yapılarak üretilen bir servis tipi kullanılmalıdır. Çok doğal olarak bu base içerisinden override edilmesi gereken bir takım üyelerde gelmektedir. Ayrıca, Proxy servisi tek bir örnek olarak (Single Instance) oluşturulmalı fakat eş zamanlı olarak gelecek istemci ve announcement taleplerine de cevap verebilmelidir.

Bu noktada IAsyncResult arayüzünüde içeren asenkron modeli uygulayıp Thread yönetimini üstlenen yardımcı bir takım tipler kullanması gerekmektedir. Zaten işin zorlaştığı nokta burasıdır. Neyseki [MSDN'](http://msdn.microsoft.com/en-us/library/dd456787(VS.100).aspx) de bu konu ile ilişkili olan örnekte, Asenkron desenin uygulanması için standart olarak sunulan sınıflar hazırdır. Dolayısıyla bu yapının aynısı kullanılarak gerekli geliştirmeler biraz daha kolayca yapılabilir. Biz örneğimizde sadece asenkron iletişimi ele alan tipleri alırken, DiscoveryProxyBase tabanlı türetmeyi kendimize göre düzenleyeceğiz.

Öyleyse başlamaya ne dersiniz. Herkes sıcak kahvesini veya çayını yada yazın şu sıcak günlerinde gidecek serin bir içeceğini alsın ve benimle birlikte adım adım ilerlemeye gayret etsin. Başlamadan önce hedef modelimizin ne olduğunu kabaca aktarmak isterim. Aşağıdaki şekilde görülen senaryoyu ele almaya çalışacağız.

![blg70_Architect.gif](images/blg70_Architect.gif)

Şeklimizden anlaşılacağı üzere Discovery Proxy Servisimiz, Service X ve Service Y'nin online/offline olma durumlarını izlemektedir. Ayrıca istemci uygulama/uygulamalar aramak istediği servise ait talebi doğrudan Discovery Proxy servisine göndermektedir. İşte tam olarak gerçekleştirmek istediğimiz test senaryosu budur. İşe ilk olarak Discovery Proxy Servisinin yazımı ile başlayabiliriz. Dana öncedende belirttiğimiz gibi, bu servis içerisinde asenkron bir yapı kullanılması söz konusu olduğundan işimiz pek kolay değil. Ben sadece Proxy isimli DiscoveryProxyBase abstract sınıfından türeyen tipin uygulanışını burada göstermek istiyorum. Örnek uygulama kodlarını indirdiğinizde OnResolveAsyncResult ve OnFindAsyncResult gibi tiplerin detaylarınıda bulabilirsiniz ki bunlarda standart olarak kullanılan tiplerdir ve MSDN tarafından yayınlanmıştır. Discovery Proxy servisinin içerisindeki sınıf modeli en basit haliyle aşağıdaki şekilde olduğu gibidir.

![blg70_ClassDiagram.gif](images/blg70_ClassDiagram.gif)

Burada bizi daha çok ilgilendiren kısım DiscoveryProxyBase türevli olan Proxy sınıfının kodlamasıdır. İşte kodlarımız;

```csharp
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Discovery;

namespace DiscoveryProxyService
{
    // T anından sadece tek bir Proxy servis nesne örneğinin olabileceğini ve aynı andan birden fazla çağrıyı karşılayabilecek şekilde kullanılabileceğini belirtiyoruz
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    class Proxy
        : DiscoveryProxyBase
    {
        // Endpoint ve metadata bilgilerini tutacağımız bir koleksiyon tanımlanır
        Dictionary<EndpointAddress, EndpointDiscoveryMetadata> _serviceList = null;
        // _serviceList' in tutarlığını(Consistency) sağlamak için tanımlanan yardımcı değişkendir
        object _syncLock = null;

        public Proxy()
        {
            _serviceList = new Dictionary<EndpointAddress, EndpointDiscoveryMetadata>();
            _syncLock = new object();
        }

        #region Yardımcı Metodlar

        // Online olan bir servis bilgisini listeye eklemek için kullanılır
        // Proxy her bir announce mesajı aldığını çalıştırılacaktır.
        void AddService(EndpointDiscoveryMetadata metadata)
        {
            // eş zamanlı thread senkronizasyonunu sağlamak için lock kullanılmıştır
            lock (_syncLock)
            {
                // Address key değerine sahip bir value var ise güncelleme yoksa ekleme yapar.
                _serviceList[metadata.Address] = metadata;
                Console.WriteLine("{0} adresli endpoint eklendi",metadata.Address.ToString());
            }
        }
        // Offline olan bir servisi listeden çıkartmak için kullanılır
        void RemoveService(EndpointDiscoveryMetadata metadata)
        {
            // eş zamanlı thread senkronizasyonunu sağlamak için lock kullanılmıştır
            lock (_syncLock)
            {
                _serviceList.Remove(metadata.Address);
                Console.WriteLine("{0} adresli endpoint çıktı", metadata.Address.ToString());
            }
        }

        // Bu metod ve aşırı yüklenmiş(overload) versiyonu Resolve ve Probe mesajlarında kullanılır
        // FindCriteria tipinden olan parametre ile gelen kriterler uyan servisleri, listeden çekmektedir
        List<EndpointDiscoveryMetadata> MatchFromServiceList(FindCriteria findCriteria)
        {
            List<EndpointDiscoveryMetadata> result = null;

            lock (_syncLock)
            {
                result = (from epMetadata in _serviceList.Values
                          where findCriteria.IsMatch(epMetadata)
                          select epMetadata).ToList<EndpointDiscoveryMetadata>();
            }

            return result;
        }

        // ResolveCriteria tipinden gelen parametrenin Address bilgisine eş düşen servisi listeden bulup Discovery Metadata bilgisini döndürür
        EndpointDiscoveryMetadata MatchFromServiceList(ResolveCriteria rCriteria)
        {
            EndpointDiscoveryMetadata result = null;

            lock (_syncLock)
            {
                result = (from epMetadata in _serviceList.Values
                          where epMetadata.Address == rCriteria.Address
                          select epMetadata).Single();
            }

            return result;
        }

        #endregion

        #region Override edilen metodlar

        // Online Announcement mesajı alındığından devreye giren metoddur
        protected override IAsyncResult OnBeginOnlineAnnouncement(AnnouncementMessage announcementMessage, AsyncCallback callback, object state)
        {
            AddService(announcementMessage.EndpointDiscoveryMetadata);
            return base.OnBeginOnlineAnnouncement(announcementMessage, callback, state);
        }

        // Online announcement mesajının işlenmesi bittiğinde devreye girer
        protected override void OnEndOnlineAnnouncement(IAsyncResult result)
        {
            base.OnEndOnlineAnnouncement(result);
        }

        // Offline announcement mesajı geldiğinde devreye giren metoddur
        protected override IAsyncResult OnBeginOfflineAnnouncement(AnnouncementMessage announcementMessage, AsyncCallback callback, object state)
        {
            RemoveService(announcementMessage.EndpointDiscoveryMetadata);
            return base.OnBeginOfflineAnnouncement(announcementMessage, callback, state);
        }

        // Offline announcement mesajının işlenmesi bittiğinde devreye giren metoddur
        protected override void OnEndOfflineAnnouncement(IAsyncResult result)
        {
            base.OnEndOfflineAnnouncement(result);
        }

        // Bir Find talebi geldiğinde devreye giren metoddur
        protected override IAsyncResult OnBeginFind(FindRequest findRequest, AsyncCallback callback, object state)
        {
            return new OnFindAsyncResult(
                MatchFromServiceList(findRequest.Criteria)
                , callback
                , state);
        }

        // Find talebinin işlenmesi sona erdiğinde devreye giren metoddur
        protected override Collection<EndpointDiscoveryMetadata> OnEndFind(IAsyncResult result)
        {
            return new Collection<EndpointDiscoveryMetadata>(OnFindAsyncResult.End(result));
        }

        // Resolve mesajı geldiğinde devreye giren metoddur
        protected override IAsyncResult OnBeginResolve(ResolveRequest resolveRequest, AsyncCallback callback, object state)
        {
            return new OnResolveAsyncResult(MatchFromServiceList(resolveRequest.Criteria)
                , callback
                , state);
        }

        // Resolve mesajının işlenmesi bittiğinde devreye giren metoddur
        protected override EndpointDiscoveryMetadata OnEndResolve(IAsyncResult result)
        {
            return OnResolveAsyncResult.End(result);
        }

        #endregion
    }
}
```

Sizi bu kod parçası ile bir süre yanlız bırakmak isterim

![Sealed](images/smiley-sealed.gif)

Aslında sınıfımızın görevi basittir. Çevre ağlar üzerinde announcement mesajı yayınlayarak online veya offline olduğunu bildiren servisleri tutmakta ve buna ek olarak, istemciden gelen arama kriterlerine uygun olanlarını yine istemci tarafına yönlendirmektedir. Sınıfımız, yardımcı metodların yanı sıra DiscoveryProxyBase tipinden gelen bazı sanal metodlarıda (Virtual Method) ezmektedir. Özellikle eş zamanlı isteklerde oluşabilecek senkronizasyon sorunlarını aşmak için basit lock tekniğinden yararlanılmaktadır. Proxy servisini geliştirmek tek başına yeterli değildir. Bu servisin bir uygulama tarafından host edilmesi gerekmektedir. Bu anlamda basit bir Console uygulaması aşağıdaki kodlar ile tasarlanabilir.

```csharp
using System;
using System.ServiceModel;
using System.ServiceModel.Discovery;

namespace DiscoveryProxyService
{
    class Program
    {
        static void Main(string[] args)
        {
            // ServiceHost nesnesi DiscoveryProxyBase türevli Proxy tipi ile oluşturulur.
            ServiceHost host = new ServiceHost(new Proxy());

            // İstemcilerin Probe mesajları için bir DiscoveryEndpoint noktası tanımlanır
            DiscoveryEndpoint discoEndpoint = new DiscoveryEndpoint(
                new NetTcpBinding()
                , new EndpointAddress("net.tcp://localhost:4034/Probe"));
            discoEndpoint.IsSystemEndpoint = false;
            // DiscoveryEndpoint host' a eklenir
            host.AddServiceEndpoint(discoEndpoint);

            // Online veya Offline olan servislerin kendilerini Proxy servisine bildirebilmeleri amacıyla bir AnnouncementEndpoint noktası oluşturulur ve servise ilave edilir
            AnnouncementEndpoint announceEndpoint = new AnnouncementEndpoint(
                new NetTcpBinding()
                , new EndpointAddress("net.tcp://localhost:4044/Announcement"));
            host.AddServiceEndpoint(announceEndpoint);

            host.Open();
            Console.WriteLine("Managed Discovery Servis Durumu : ");
            Console.ReadLine();
            host.Close();
        }
    }
}
```

Proxy servisini bu şekilde host ettikten sonra, kendisine bildirimde bulunabilecek bir servisin nasıl tasarlanabileceğine de bakmamız yerinde olacaktır. Bu anlamda örneğimizde ServiceX ve ServiceY isimli iki farklı servis uygulaması bulunmaktadır. Bu servislerin en önemli görevlerinden biriside, ağa dahil olmaları veya ayrılmaları halinde bu durumlarını Proxy servisine bildirmeleridir. Her iki servis arasındaki fark ise tabiki sundukları hizmettir.

ServiceX içeriği;

```csharp
using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;

namespace ServiceX
{
    [ServiceContract]
    interface ICalculus
    {
        [OperationContract]
        double Sum(double x, double y);
    }

    class CalculusService
        : ICalculus
    {
        public double Sum(double x, double y)
        {
            return x + y;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {            
            ServiceHost host = new ServiceHost(
                typeof(CalculusService)
                , new Uri("net.tcp://localhost:9002/CalculusService/" + Guid.NewGuid().ToString()));
            host.AddServiceEndpoint(
                typeof(ICalculus), new NetTcpBinding(), string.Empty);
            // Bir announcement endpoint noktası oluşturulur ve proxy servisine bu sayede bildirim yapılması sağlanır
            AnnouncementEndpoint announcementEndpoint = new AnnouncementEndpoint(
                new NetTcpBinding()
                , new EndpointAddress("net.tcp://localhost:4044/Announcement"));
            // Servisin keşfedilebilir olması sağlanır
            ServiceDiscoveryBehavior serviceDiscoveryBehavior = new ServiceDiscoveryBehavior();
            serviceDiscoveryBehavior.AnnouncementEndpoints.Add(announcementEndpoint);
            host.Description.Behaviors.Add(serviceDiscoveryBehavior);
            
            host.Open();
            Console.WriteLine("Service X açıldı");
            Console.ReadLine();
            host.Close();
        }
    }
}
```

ServiceY içeriği;

```csharp
using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;

namespace ServiceY
{
    [ServiceContract]
    interface IAdventure
    {
        [OperationContract]
        double FindExpensiveProduct(int categoryId);
    }

    class AdventureService
        : IAdventure
    {
        public double FindExpensiveProduct(int categoryId)
        {
            return 1000;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(
                typeof(AdventureService)
                , new Uri("http://localhost:10005/Adventure/ProductService/" + Guid.NewGuid().ToString()));
            host.AddServiceEndpoint(
                typeof(IAdventure), new WSHttpBinding(), string.Empty);
            // Bir announcement endpoint noktası oluşturulur ve proxy servisine bu sayede bildirim yapılması sağlanır
            AnnouncementEndpoint announcementEndpoint = new AnnouncementEndpoint(
                new NetTcpBinding()
                , new EndpointAddress("net.tcp://localhost:4044/Announcement"));
            ServiceDiscoveryBehavior serviceDiscoveryBehavior = new ServiceDiscoveryBehavior();
            // Servisin keşfedilebilir olması sağlanır
            serviceDiscoveryBehavior.AnnouncementEndpoints.Add(announcementEndpoint);
            host.Description.Behaviors.Add(serviceDiscoveryBehavior);
            host.Open();
            Console.WriteLine("Service Y açıldı");
            Console.ReadLine();
            host.Close();
        }
    }
}
```

Piuuuuuvvvvv!!!

![Laughing](images/smiley-laughing.gif)

İşimiz bitti diye düşünebilirsiniz. Ama hayır... Birde istemcilerin nasıl yazılabileceğine bakmamız gerekiyor. İstemci tarafında tabiki olmassa olmazlardan biriside, kullanmak istediği servislere ait proxy referanslarına sahip olmaları gerekliliğidir. Bunu göz önüne alarak ilerlediğimizi düşünürsek istemci tarafında da aşağıdaki gibi bir kodlama yapmamız yeterlidir.

```csharp
using System;
using System.ServiceModel;
using System.ServiceModel.Discovery;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Teste başlamak için bir tuşa basınız");
            Console.ReadLine();

            // Proxy servisini keşfedebilmek için bir DiscoveryEndpoint oluşturulur
            DiscoveryEndpoint disEndpoint = new DiscoveryEndpoint(
                new NetTcpBinding()
                , new EndpointAddress("net.tcp://localhost:4034/Probe")
                );
            // İstemci tarafının kullanılabilir servisleri keşfetmesini kolaylaştıran DiscovertClient tipine ait nesne örneği DiscoveryEndpoint ile oluşturulur
            DiscoveryClient disClient = new DiscoveryClient(disEndpoint);
            disClient.Open();
            // Bir arama kriteri uygulanır ve dönen cevaptan kullanılabilir servis adresi tedarik edilir
            FindResponse response = disClient.Find(new FindCriteria(typeof(ICalculus)));
            EndpointAddress epAddress=response.Endpoints[0].Address;

            // Eğer arama kriterine uygun servisler bulunmuşsa
            if (response.Endpoints.Count > 0)
            {
                // İlkinin adres bilgisini al
                EndpointAddress epAddress = response.Endpoints[0].Address;

                Console.WriteLine("{0} adresi bulundu", epAddress.ToString());

                // İstemci için gerekli proxy referansı örneklenir ve Probe mesajı ile bulunan Endpoint adresi kullanılır.
                CalculusClient client = new CalculusClient(new NetTcpBinding(), epAddress);

                // Servis operasyonu çağrılır
                Console.WriteLine("{0}+{1}={2}", 3, 4, client.Sum(3, 4).ToString());
                Console.ReadLine();
            }

            disClient.Close();
        }
    }
}
```

Nihayet test yapabilmek için gerekli ortamı hazırladığımızı ifade edebilirim.

![Cool](images/smiley-cool.gif)

İlk olarak Discovery Proxy servisinin, sonrasında istemcinin kullanmak istediği servislerin ayağa kaldırılması gerekir. Son olarak istemci uygulamanın çalıştırılması ve test edilmesi yeterlidir. Yapılan ilk testler sonucunda aşağıdaki sonuçlar elde edilmiştir.

![blg70_Runtime.gif](images/blg70_Runtime.gif)

Görüldüğü gibi, ServiceX ve ServiceY isimli servislerin açılmaları ve kapatılmaları, Managed Discovery Proxy servisi tarafından tespit edilebilmiştir. ServiceX'in online olduğu zaman dilimi içerisinde, istemciden gelen talep başarılı bir şekilde karşılanabilmiştir. Farklı bir testide şu şekilde yapmak gerekir. İstemci uygulama, ServiceX için talepte bulunmadan önce, ServiceX kapatılır.

![Wink](images/smiley-wink.gif)

Bu durumda istemcinin aradığı kritere uyan bir servis ayakta olmadığı için, istemcinin bir işlem yapamıyor olması gerekir. Olayı istisna ile sonlandırmayı engellemenin yolu ise if ile yapılan Count kontrolüdür. Bu tip bir testin sonucunda çalışma zamanı görüntüsü aşağıdaki gibi olacaktır.

![blg70_Runtime2.gif](images/blg70_Runtime2.gif)

Her ne kadar sadece iki çalışma zamanı testi yapılmış olsada, örneğin iyi bir şekilde değerlendirilmesi ve olası tüm hataların önüne geçilmesi gerekmektedir. Söz gelimi, Proxy servisinin kapatılmasından sonra, kendisine bağlı olan başka servislerin kapatılmaya çalışılması esnasında, söz konusu servislere ait ortamlarda çalışma zamanı istisnaları (Runtime Exception) oluşması kaçınılmazdır. Bu gibi noktaları dikkat almanızı ve geliştirmenizi buna göre yapmanızı öneririm.

![blg70_Son.jpg](images/blg70_Son.jpg)

Nihayet, uzun saatlerin, gidilen kilometrelerce yolun sonunda gece bastırmış ve şehrin ışıkları görünmüştür. Hepimiz zaman zaman yazılım alanında bir konuyu öğrenirken bu tip zorlu yollardan geçmek zorunda kalabiliriz. Ancak sabırlı olanlarımız, yolun sonuna kadar gitmekten çekinmeyecek ve ödül olarak şehrin parlak ışıkları ile karşılanacaktır. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
