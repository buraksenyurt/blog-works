---
layout: post
title: "WCF 4.0 Yenilikleri - Ad Hoc WS-Discovery [Beta 1]"
date: 2009-08-18 09:23:00 +0300
categories:
  - wcf-4-0-beta-1
tags:
  - windows-communication-foundation
---
Uzun süredir macera dolu bir rüya görmüyordum. Geçtiğimiz gece askeri bir birlikte görev yapmaktaydım ve gerideki topçu birliklerime hedeflere ait koordinatları bildiriyordum. Tabi gerçek hayatta yedek subak eğitimimi Topçu sınıfında, Ateş Destek üzerine aldığım için keşif, arama ve bulma gibi konularda azda olsa bilgi sahibiydim. Rüyamda da bu bilgilerimi kullandığımı itiraf edebilirim. Peki konumuz nedir?

![blg67_Giris.jpg](/assets/images/2009/blg67_Giris.jpg)

Bu yazımızdaki konumuzun keşif yapmak ile aslında çok yakın bir ilişkisi bulunmaktadır. WS-Discovery modeli için WCF 4.0 ile birlikte gelen kolaylıklar.

Ağ üzerinde bulunan servis noktalarının çalışma zamanında keşfi (Runtime Discovery), Servis Yönelimli Mimarilerde (Service Oriented Architecture) karşılaşılan en önemli ihtiyaçlardan birisidir. Öyleki; bazı servislerin ağa dahil olması, ağdan ayrılması gibi zaman içerisinde yerlerinin sıklıkla değiştiği durumlarda, söz konusu servislerin istemciler tarafından dinamik olarak keşfedilmesi gerekebilir. WS-Discovery bu tip durumlar için [OASIS](http://www.oasis-open.org/home/index.php)tarafından kabul görmüş bir mesajlaşma standardı sunmaktadır. Bu mesajlaşma standardına göre, servislerin keşfedilmesi (Discovery) için aslında temel olan dört temel operasyon söz konusudur.

- Hello
- Probe
- Resolve
- Bye

Servislerin ağa dahil olmaları sırasında multicast mesajlar yardımıyla "Merhaba, ben geldim,buradayım" demeleri bu operasyonlardan birisidir (Hello). Diğer yandan çok doğal olarak ağa dahil olan bir servisin ağdan ayrılması halinde, "Ben gidiyorum" şeklinde bir multicast yayınlama yapması söz konusudur (Bye). Bu da WS-Discovery içerisinde yer alan ve servisler tarafından gerçekleştirilen operasyonlardan bir diğeridir. İstemciler açısından olaya bakıldığında ise iki farklı operasyon söz konusudur. İstemciler, kullanmak istedikleri servislerin tipi veya kapsamlarına göre arama işlemlerini yine mutlicast mesajlaşma ile gerçekleştirebilirler (Probe). Birde istemcilerin servisi adları ile aramasıda mümkündür (Resolve). Buda istemcilerin ele aldığı ikinci operasyon olarak düşünülebilir.

WS-Discovery ile ilişkili olarak yaptığım araştırmalarda pek çok kaynakta örnek olarak ağ üzerindeki bir yazıcı sürücüsünün (Printer Device) örnek olarak verildiğini gördüm. Sanıyorumki konunun anlaşılması üzerine verilebilecek en güzel örnek...Bu vakaya göre ağa katılan ve hatta bazı durumlarda ağdan ayrılan bir yazıcı sürücüsünün (Printer Device) servis olarak değerlendirildiği düşünülmektedir. Printer ağa katıldığında, ağ üzerindeki tüm boğumlara tek yönlü bir merhaba mesajını (One-way Hello Message) gönderir. Bu andan itibaren istemciler (Clients), ağ üzerine Probe mesajlarını göndererek var olan yazıcı listelerinden örneğin belirli bir alt ağda olanlara talepte bulunabilirler. Tabiki istemci isterse Resolve mesajı ile belirli bir yazıcı sürücüsüne talepte de bulunabilir. Elbetteki bu vakada eksik kalan kısım yazıcının ve dolayısıyla sürücü servisinin ağdan kopartılmasıdır (Örneğin kapatılması). Bu durumda printer sürücü servisi, ağ üzerine bu kez tek-yönlü güle güle (One-Way Bye) mesajı göndererek artık etkin olmadığını bildirmektedir.

WCF 4.0 tarafında istemcilerin özellikle servisleri keşfetmeleri amacıyla değerlendirebilecekleri WS-Discovery protokolünün iki uygulama modeli bulunmaktadır. Ad Hoc ve Managed. Ad Hoc modeline göre istemci, önceden belirlenmiş kriterlerine göre servisi Probe mesajları ile keşfetmeye çalışmaktadır. Eğer Probe mesajına karşılık eşleşen bir servis bulunursa istemci tarafına bir cevap gönderilir. Bu modelde istemcinin multicast mesajlar ile sürekli bir kontrol içerisinde olması (Polling) ağ üzerindeki trafiği arttırıcı bir etken olarak görülebilir. İşte bu noktada servisin announce adı verilen Hello ve Bye mesajları ile kendisinin online olup olmadığına dair bildirimlerde bulunması söz konusu yükün hafifletilmesini sağlamaktadır.

Bu modelin uygulanması son derece kolaydır. Diğer yandan model sadece yerel ağlar için kullanışlıdır. Ancak ağın ötesinde yer alan servislerin keşfedilmesi söz konusu olduğunda ise Managed modelin kullanılması gerekmektedir. Managed modelde, ağda görülen servislerin tamamı için merkezileştirilmiş depolama alanı (Repository) ve bir proxy servisi söz konusudur. Böylece proxy servisi ağın ötesindeki canlı Endpoint listelerini tutarak, ağ içerisindeki diğer istemcilerin söz konusu servislerden yararlanabilmesini sağlamaktadır. Dolayısıyla istemciler doğrudan aradaki proxy servisi ile iletişim kurmaktadır. Managed modelin uygulanması biraz daha komplekstir. O nedenle bu ilk yazımızda kolay olan Ad Hoc modelini inceliyor olacağız.

![Wink](/assets/images/2009/smiley-wink.gif)

Özetle WS-Discovery, OASIS tarafından standart olarak görülmüş ve multicast mesajlaşmayı baz alan, servislerin keşfedilmesi için kullanılan bir protokol bütünü olarak tanımlanabilir. Konuyu daha net kavrayabilmek adına basit bir örnek ile ilerlememizde fayda olacağı kanısındayım. İlk olarak bir servis uygulaması geliştirecek ve bunu WS-Discovery destekli olacak şekilde kuracağız. Sonrasında ise bu servisi keşfetme yeteneğine sahip olan bir istemci uygulamayı geliştireceğiz. Her iki tarafında.Net Framework 4.0 Beta 1 üzerinde ve Visual Studio 2010 Beta 1 yardımıyla geliştirildiğini belirtelim. Servis tarafına ait kod içeriğimizi aşağıdaki gibi geliştirebiliriz.

```csharp
using System;
using System.ServiceModel;

namespace ServerApp
{
    [ServiceContract]
    interface ICalculus
    {
        [OperationContract]
        double Sum(double x, double y);
    }

    class CalculusService
        :ICalculus
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
            ServiceHost host = new ServiceHost(typeof(CalculusService)
                ,new Uri("http://localhost:5002/CalculusService"));

            host.Open();
            Console.WriteLine("Service status {0}",host.State.ToString());
            Console.ReadLine();
            host.Close();
            Console.WriteLine("Service status {0}", host.State.ToString());
        }
    }
}
```

Servis tarafı için belkide en önemli olan kısım App.config dosyasının içeriğidir.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <system.serviceModel>
    <behaviors>
      <serviceBehaviors>
        <behavior>
          <!-- Service Discovery davranışı etkinleştirilir-->
          <serviceDiscovery />
          <serviceMetadata/>
        </behavior>
      </serviceBehaviors>
    </behaviors>    
    <services>
      <service name="ServerApp.CalculusService">
        <endpoint address="" binding="basicHttpBinding" contract="ServerApp.ICalculus" />
        <endpoint address="Mex" kind="mexEndpoint" />
        <endpoint name="udpDiscovery" kind="udpDiscoveryEndpoint" /> <!-- UDP tabanlı standart Endpoint tanımlaması yapılır-->
      </service>
    </services>
  </system.serviceModel>
</configuration>
```

BasicHttpBinding tabanlı bir Endpoint tanımlamasının haricinde iki adet standart endPoint tanımlaması daha bulunmaktadır. Mex tabanlı olan ve metadata publishing açılımına izin veren bir yana, önemli olan udpDiscoveryEndpoint tipinden olanıdır. Bu tanımlamaya ek olarak servis davranışlarında (serviceBehavior) belirtilen serviceDiscovery bildirimi ile, servisin istemciler tarafından keşfedilebilmesi için UDP tabanlı protokol üzerinden destek verebileceği set edilmiş olmaktadır.(Bu örnekte announce mesajlaşma kısmını değerlendirmedik. Bunu ilerleyen kısımlarda ele alacağız. Ancak udpAnnouncementEndpoint tipinin bu amaçla kullanıldığını ipucu olarak verebiliriz.) Gelelim istemci tarafına...

İstemci tarafında önem arz eden konuların başında System.ServiceModel haricinde System.ServiceMode.Discovery assembly'ınında projeye referans edilmesi gelmektedir. Nitekim bu assembly içerisinde Discovery sistemi için gerekli tip tanımlamaları yer almaktadır (DiscoveryClient, FindCriteria vb...) İstemci tarafında bir proxy tipi bulunmaktadır. Bunun üretimi için standart olarak Add Service Reference özelliğinden yararlanılabilir. Ancak bu sefer servis tarafının adresinin ne olduğu bilinmeden istemci tarafında geliştirilme yapılması hedeflenmektedir. Nitekim sistemimize göre istemci uygulama, ağ üzerinde belirli bir kritere uyan servisi arayacak, bulduğunda ise yayınlama yaptığı adresten yararlanarak proxy nesnesini ayağa kaldıracaktır. Dolayısıya config dosyası içeriğini aşağıdaki gibi geliştirmemiz gerekmektedir.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <system.serviceModel>
    <client>
      <endpoint binding="basicHttpBinding" contract="CalculusSpace.ICalculus" name="CalculusEndpoint" />
      <endpoint name="udpDiscovery" kind="udpDiscoveryEndpoint" />      
    </client>
  </system.serviceModel>
</configuration>
```

Servis tarafındaki app.config dosyası içeriğine benzer olaraktan, istemci tarafında da standart endPoint tiplerinden olan udpDiscoveryEndpoint kullanımı söz konusudur. İstemci tarafının uygulama kodlarını ise aşağıdaki gibi geliştirmemiz yeterlidir.

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
            // FindCriteria' nun sonucunu taşıyacak olan FindResponse nesne örneği oluşturuluyor
            FindResponse findResponse = disClient.Find(findCriteria);

            // Kritere göre bulunan ilk EndPoint adresi alınıyor
            EndpointAddress epAddress = findResponse.Endpoints[0].Address;

            // Bulunan endPoint adresi, proxy' nin üretilmesinde kullanılıyor
            CalculusClient client = new CalculusClient("CalculusEndpoint", epAddress);

            Console.WriteLine("{0} adresi üzerinden çağrı yapılacaktır",epAddress.Uri.ToString());
            double result = client.Sum(3, 5);
            Console.WriteLine("{0} + {1} = {2}",3,5,result.ToString());

            Console.ReadLine();
        }
    }
}
```

DiscoveryClient tipine ait nesne örneğinin oluşturulması sırasında UDP tabanlı bir Discovery bağlantı noktasının kullanılacağı belirtilmektedir. Sonrasında ise bir arama kriteri oluşturulur. Bizim arama kriterimize göre servisin arayüz sözleşmesi (Service Contract) kullanılmaktadır. DiscoveryClient nesne örneğine ait olan Find metodu ile belirtilen kritere göre bir arama yapılmasına başlanır. Bir başka deyişle ağ üzerinde bir multicast mesaj yayını ile belirtilen kritere uygun servis uç noktası aranır. Servis noktası bulunduğunda ise Find metodundan geriye dönen FindResponse nesne örneğine ait Endpoints koleksiyonu değerlendirilir ve dizinin ilk elemanının Address özelliğinde servise ulaşılabilecek adres bilgisi elde edilir.

Bundan sonraki kısım ise son derece tanıdıktır. Tek yapılması gereken proxy nesne örneğinin oluşturulması sırasında cevap olarak elde edilen Endpoint adres bilgisinin kullanılması gerekmektedir. Burada gözden kaçırmamamız gereken nokta, istemci tarafında herhangibir şekilde servise ait adres bilgisinin açık bir şekilde verilmemiş olmasıdır. İstemcinin elinde aslında ağ üzerindeki servisler içerisinde arama yapabileceği bir kriter bulunmaktadır (ICalculus isimli servis sözleşmesi). Uygulamayı test ettiğimizde ilk etapta aşağıdaki çıktıyı elde ederiz.

![blg67_FirstRuntime.gif](/assets/images/2009/blg67_FirstRuntime.gif)

Nevarki istemci tarafında sonuç elde edilinceye kadar belirli bir süre beklendiği hemen fark edilebilir. Bu son derece doğaldır. Nitekim istemcinin kriterine uygun olan servisin arama süresi söz konusudur. Ancak şunu belirtelim, istenirse söz konusu arama kısmı asenkron olarak değerlendirilebilir. Şu aşamada bizim için önemli olan nokta, istemcinin yayın yapılan servis adresini bulabilmiş olmasıdır. Hatta bu noktada testimize şu şekilde devam etmemizde yarar vardır. Tüm uygulamaları kapattıktan sonra servisin adresini aşağıdaki gibi değiştirdiğimizi düşünelim.

http://localhost:6002/CalculusService

Servis uygulamasını ve sonrasında istemci uygulamayı tekrardan çalıştırırsak bu kez aşağıdaki ekran görüntüsünü elde ederiz.

![blg67_SecondRuntime.gif](/assets/images/2009/blg67_SecondRuntime.gif)

Mükemmel

![Wink](/assets/images/2009/smiley-wink.gif)

Nitekim servis tarafının adresini değiştirmiş olmamıza rağmen istemci uygulama çalışabilmektedir. Normal şartlarda Discovery gibi bir mekanizma kullanmadığımızda istemci uygulamalarda gerekli adres bilgilendirmelerinin değiştirilmesi gerektiğini unutmayalım.

Şimdide bu işlemin asenkron olarak nasıl yapılabileceğini bakacağız. Web servislerine.Net 2.0 versiyonu ile birlikte getirilmiş olan olay tabanlı asenkron erişim (Event-Based Asynchronous) tekniği burada da kullanılmaktadır. Find metodunun asenkron olan FindAsync versiyonu kullanılarak, arama işleminin asenkron olarak gerçekleştirilmesi sağlanabilir. İşlemler başarılı bir şekilde tamamlandığı takdirde FindCompleted isimli olay metodu devreye girmektedir. İşte asenkron arama ile ilişkili örneğimizde yaptığımız değişiklikler.

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

Bu kez örneğimizin çalışma zamanı çıktısı aşağıdaki gibi olacaktır.

![blg67_Asenkron.gif](/assets/images/2009/blg67_Asenkron.gif)

Görüldüğü üzere FindAsync çağrısından sonra hemen alt satıra geçilerek kodun çalışması devam etmiştir. Servisten gerekli cevap alındıktan sonrada, Sum operasyonu başarılı bir şekilde icra edilmiştir. Discovery mekanizması ile ilişkili olaraktan WCF 4.0 tarafına gelen başka yeniliklerde söz konusudur. Örneğin

- Scope kullanılarak keşif yapılması,
- announcement kullanılması,
- Managed servis keşif tekniği vb...

Bu konularıda ilerleyen yazılarımızda ele almaya çalışıyor olacağım. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[AdHocDiscovery.rar (51,72 kb)](/assets/files/2009/AdHocDiscovery.rar)
