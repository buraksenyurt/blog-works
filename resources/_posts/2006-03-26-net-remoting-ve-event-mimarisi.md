---
layout: post
title: ".Net Remoting ve Event Mimarisi"
date: 2006-03-26 12:00:00 +0300
description: "Remoting mimarisinde temel amaç, istemcilerin uzak nesnelere (remote objects) erişmelerini sağlamak ve bu nesneler üzerindeki metodları çalıştırmaktır. .Net Remoting için en çok kullanılan model Marshall By Referance modelidir. Bu modelde istemciler uzak nesneler ile, sunucu üzerinde oluşturulan uzak nesne referansları yoluyla konuşurlar. Ancak bazı durumlarda, uzak nesnelerin yer aldığı sunucu uygulamalar, istemciler üzerinde yer alan metodları çalıştırmak isteyebilir. Böyle bir durumda roller süreç içerisinde istemci ve sunucu arasında değişime uğrar. Yani istemciler sunucudaki uzak nesnelere erişebilirken, sunucuda istemciler üzerindeki nesnelere erişebilmektedir. Bu modelin gerçekleşmesi için özellikle olay güdümlü programlanın can damarı olan temsilci (delegate) ve event (olay) tipleri kullanılmaktadır."
categories:
  - dotnet-remoting
tags:
  - dotnet-remoting
  - csharp
  - xml
  - http
  - threading
  - delegates
---
# .Net Remoting ve Event Mimarisi
Değerli Okurlarım Merhabalar,

Remoting mimarisinde temel amaç, istemcilerin uzak nesnelere (remote objects) erişmelerini sağlamak ve bu nesneler üzerindeki metodları çalıştırmaktır..Net Remoting için en çok kullanılan model Marshall By Referance modelidir. Bu modelde istemciler uzak nesneler ile, sunucu üzerinde oluşturulan uzak nesne referansları yoluyla konuşurlar. Ancak bazı durumlarda, uzak nesnelerin yer aldığı sunucu uygulamalar, istemciler üzerinde yer alan metodları çalıştırmak isteyebilir. Böyle bir durumda roller süreç içerisinde istemci ve sunucu arasında değişime uğrar. Yani istemciler sunucudaki uzak nesnelere erişebilirken, sunucuda istemciler üzerindeki nesnelere erişebilmektedir. Bu modelin gerçekleşmesi için özellikle olay güdümlü programlanın can damarı olan temsilci (delegate) ve event (olay) tipleri kullanılmaktadır.

Teorik olarak bu yaklaşımda, istemciler uzak nesne için geçerli olan bir event'ı yükleyebilir ve uzak nesneleri kullanan sunucuda tetiklenen olay sonrası, istemci tarafında yer alan olay metodlarını çalıştırabilir. Bu olayın gerçekleşebilmesi için, uzak nesnenin istemci tarafından yüklenebilecek bir event'a sahip olması, ayrıca olayın tetiklenmesi sonucu istemcideki uygun metodu işaret edebilecek bir temsilci (delegate) tipininde var olması gerekir. Dolayısıyla sunucu tarafında ve istemci tarafında olması gerekenler belirlidir. Sunucu tarafında, uzak nesne tipimiz, istemcideki olay metodunu işaret edebilecek bir delegate tipimiz ve istemci tarafından erişilebilecek bir event tipimiz var olmalıdır. İstenirse, olay metodu için bilgi taşıyacak başka bir tip daha sunucu tarafında yer alabilir. Bilgi taşıyacak bu tipi örneğin bir windows uygulamasındaki button nesnesine tıklandığında devreye giren click olay metodunun EventArgs parametre tipine benzetebiliriz. İstemci tarafında ise, uzak sunucunun herhangibir olay sonucu çalıştıracağı olay metodunu içeren bir tip yer almalıdır.

![dikkat.gif](/assets/images/2006/dikkat.gif)

Hem sunucunun hemde istemcinin karşılıklı olarak ilgili nesnelerini kullanabilmeleri için, bu nesnelerin MarshallByReference tipinden türetilmeleri şarttır. Bu zaten Referans tabanlı remoting işlemlerin temel prensibidir.

Aşağıdaki şekil sunucu ve istemci tarafında yazmamız gereken tipleri özetlemektedir.

![mk153_1.gif](/assets/images/2006/mk153_1.gif)

Gelelim mimarinin işleyiş şekline. Uzak sunucu aktif olarak hizmet vermeye başladıktan sonra, istemciler uzak nesne referansını elde etmelidir. Burada uzak nesne referansının elde ediliş şekli önemlidir. Client Activated Object yada Server Activated Object mimarisine dayalı bir yaklaşım kullanılabilir ancak SAO mimarisinde yer alan SingleCall modelini stateless olduğu için kullanmak çalışma zamanında istisnalara yol açar. Nitekim SingleCall modelinde istemcilerin çalıştırdığı uzak nesnelere ait referanslar sunucuda oluşturulduktan sonra hemen kullanılır ve yok edilirler. O yüzden SAO modelinde ısrarcı olunacaksa Singleton aktivasyon tipi kullanılmalıdır. Biz örneğimizde Client Activated Object mimarisini kullanacağız.

![dikkat.gif](/assets/images/2006/dikkat.gif)
Client Activated Object (CAO) mimarisinde, istemci uzak nesneye ait bir nesne örneğini oluşturduğu zaman, sunucu üzerinde bu uzak nesneye ait bir referans hemen oluşturulur. Server Activated Object (SAO) modelinde ise sunucu üzerindeki uzak nesneye ait referansın oluşturulması için istemci tarafında uzak nesneye ait bir metodun çağırılması gerekir.

Uzak nesne referansının sunucu tarafında oluşturulmasının ardından istemci, uzak nesne için bir olay (event) kaydeder ve hemen ardından uzak nesne üzerinde kaydettiği olayı ele alan bir metodu tetikler. Uzak sunucuda yer alan bu metod içerisinde, o anki referans için bir olayın yüklenip yüklenmediği kontrol edilir. Eğer olay yüklenmişse sunucu, istemci üzerindeki ilgili olay metodunu çalıştırır ve varsa gerekli olay bilgilerini bu istemciye aktarır. Şimdi gelin bu bahsettiklerimizi uygulama koduna dökelim. Öncelikle uzak sunucu nesnemizin yer aldığı sınıf kütüphanesini tasarlamak ile işe başlamalıyız.

Sunucu, istemci üzerindeki olay metodunu çalıştırabilmek için bir temsilci (delegate) tipi kullanacaktır. Temsilcimizin işaret edeceği metod modelinin aşağıdaki gibi olduğunu düşünelim. Burada, olay metoduna bilgi taşımak amacıyla, parametre olarak kendi tasarladığımız serileştirilebilir (ki remoting sırasında bu şarttır) EventInfo tipini kullanmaktayız.

![mk153_2.gif](/assets/images/2006/mk153_2.gif)

Temsilci tipimiz; dlg

```csharp
public delegate void dlg(EventInfo eInfo);
```

Olay metodumuza bilgi taşıyacak tipimiz; EventInfo

```csharp
[Serializable]
public class EventInfo
{
    private string _info;

    public string Info
    {
        get { return _info; }
        set { _info = value; }
    }
    public EventInfo(string inf)
    {
        _info = inf;
    }
}
```

Gelelim uzak nesnemize ait tipe. Uzak nesne sınıfımızın en önemli noktası bir event'a sahip olması. Bu event'i az önce tanımladığımız temsilci yardımıyla oluşturuyouruz. Diğer tarafan EventTrigger isimli metodumuz ise, istemcilerin uzak nesne üzerine kaydettikleri bir event olup olmadığını denetleyerek olay metodunu çağırıyor.

![mk153_3.gif](/assets/images/2006/mk153_3.gif)

Uzak nesnemize ait sınıf kodlarımız;RemObj

```csharp
public class RemObj:MarshalByRefObject
{
    public event dlg Click;

    public RemObj()
    {
        Console.WriteLine("Uzak nesne için örnek oluşturuldu...");
    }
    public string RemMethod()
    {
        return "Uzak metod";
    }
    public void EventTrigger(string userName)
    {
        if (Click != null)
        {
            Console.WriteLine(userName + " sunucu üzerinde olay tetikledi..."+DateTime.Now.ToString());
            System.Threading.Thread.Sleep(10000);
            EventInfo ei = new EventInfo(DateTime.Now.ToString());
            Click(ei);
        }
    }
}
```

Gelelim istemci tarafında, sunucu tarafından çağırılacak olay metodunun bulunduğu tipe; ClientSide isimli sınıfımız içerisinde sadece uzak nesnenin bulunduğu sunucunun çağıracağı olay metodunu tanımladık. Bu metodun yapısının, istemcilerin erişeceği sunuc üzerindeki uzak nesnede tanımlı delegate tipimizin belirttiği ile aynı olduğuna dikkat edin. Nitekim, uzak nesnedeki EventTrigger metodu istemci tarafından tetiklendiğinde, buradaki kodlara göre Click (ei) isimli metod çağırımı, Click event'i içerisinde tanımlı delegate tipinin işaret ettiği istemci üzerindeki olay metodunu çalıştıracaktır. Diğer yandan sınıfımızı yine MarshallByReference tipinden türetiyoruz. Nitekim sunucu, uzak nesne olarak istemci tarafındaki bu tipin referanslarına ihtiyaç duyacaktır.

![mk153_4.gif](/assets/images/2006/mk153_4.gif)

İstemci tarafındaki uzak nesne tipine ait kodlarımız; ClientSide

```csharp
public class ClientSide:MarshalByRefObject
{
    public void EventMethod(EventInfo eInf)
    {
        Console.WriteLine(eInf.Info);
    }
}
```

Artık sunucu ve istemci uygulamalarımızı kodlayabiliriz. İstemci ve sunucu tarafını basit Console uygulamaları olarak tasarlayacağız. Remoting için gerekli konfigurasyon ayarlarını ise xml bazlı config dosyalarında tutacağız. Önce sunucu tarafının kodlarını ve konfigurasyon dosyasını oluşturmakla işe başlayalım. Elbette hem sunucu hemde istemci tarafında System.Runtime.Remoting ve RemoteObjects assembly'ımıza ait referanslarımızı eklememiz gerekiyor.

Server uygulamamıza ait kodlar; Server

```csharp
using System;
using System.Runtime.Remoting;
using RemoteObject;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            RemotingConfiguration.Configure("..\\..\\ServerApp.config",false);
            Console.WriteLine("Sunucu dinlemede...");
            Console.WriteLine("-------------------");
            Console.ReadLine();
        }
    }
}
```

Server uygulamamıza ait konfigurasyon dosyası; ServerApp.config

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.runtime.remoting>
        <application>
            <service>
                <activated type="RemoteObject.RemObj,RemoteObject" />
            </service>
            <channels>
                <channel ref="tcp" port="4567">
                    <serverProviders>
                        <provider ref="binary" typeFilterLevel="Full" />
                    </serverProviders>
                </channel>
            </channels>
        </application>
    </system.runtime.remoting>
</configuration>
```

Şimdi konfigurasyon dosyamız üzerinde biraz konuşalım. Server tarafında CAO (Client Activated Object) modelini desteklediğimizi belirtiyoruz. Activated node'u içerisinde yer alan type niteliği (attribute) değişken olarak Namespace.SınıfAdı,AssembleAdı na göre gerekli bilgiyi almaktadır. Bildiğiniz gibi istemci tarafındada bu type bilgisi tanımlanacak ve istemciler sunucu üzerindeki uzak referansları talep ederken bu bilgileri kullanacaktır. Ayrıca, Tcp protokolünü kullanacak kanal bilgilerinide channels node'u altında yer alan channel node'unda belirtmekteyiz. Gelelim istemci tarafına;

İstemci uygulamamıza ait kodlar; Client

```csharp
using System;
using System.Runtime.Remoting;
using RemoteObject;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            RemotingConfiguration.Configure("..\\..\\ClientApp.config",false);
            RemObj ro = new RemObj();
            ClientSide cs = new ClientSide();
            ro.Click += new dlg(cs.EventMethod);
            ro.EventTrigger("Burak");
            Console.ReadLine();
        }
    }
}
```

İstemci uygulamamızda uzak nesne (RemObj) ve istemci tarafındaki nesne (ClientSide) için örnekler oluşturuluyor. Daha sonra uzak nesnemize Click event'ini kaydediyoruz (Register). Burada önemli olan nokta event'imizi uzak nesneye kaydederken, uzak nesnenin üzerinde Click olayının tetiklenmesi sonucu, istemci tarafında çalışıtıracak olan metodun (EventMethod) belirtilmesi.

```csharp
ro.Click += new dlg(cs.EventMethod);
```

Bu ifade ile, uzak nesne üzerinde Click olayı tetiklendiğinde (ki biz bunu uzak nesnenin EventTrigger metodu ile canlandırıyoruz), istemci tarafındaki nesnemizin EventMethod'unu çalıştıracağımızı belirtiyoruz.

İstemci uygulamamıza ait konfigursayon dosyası; ClientApp.config

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.runtime.remoting>
        <application>
            <client url="tcp://localhost:4567/">
                <activated type="RemoteObject.RemObj,RemoteObject" />
            </client>
            <channels>
                <channel ref="tcp" port="0">
                    <serverProviders>
                        <provider ref="binary" typeFilterLevel="Full" />
                    </serverProviders>
                </channel>
            </channels>
        </application>
    </system.runtime.remoting>
</configuration>
```

İstemci tarafındada channel bilgisi olarak sunucu tarafında olduğu gibi Tcp kanalını seçtik. Böyle bir seçim yapmamızın nedeni hem sunucun hemde istemcinin birbirleri üzerindeki nesnelerinin referanslarını kullanacak uygun kanallara ihtiyaç olunmasıdır. Normal şartlar altında sunucu tarafında Tcp Server (veya Http Server), istemci tarafında ise Tcp Client (veya Http Client) kanalları seçilerek remoting işlemleri gerçekleştirilir. Ancak burada sunucu uygulama yeri geldiği zaman istemci gibi davranacaktır ve uzak metodu çağıracaktır ki sunucu açısından önemli olan uzak metod bizim olay metodumuzdur.

![dikkat.gif](/assets/images/2006/dikkat.gif)
Delegate tipinin serileştirilebilir olması için (remoting'i destekelemesi için) xml konfigurasyonunda serviceProvider kısmı yer almaktadır.

Artık tek yapmamız gereken uygulamamızı bu haliyle test etmek olacaktır. Öncelikle istemci tarafını çalıştıralım. Bunu yapmamızdaki amaç çalışma zamanında aşağıdaki ekranda görülen istisnayı almak.

![mk153_5.gif](/assets/images/2006/mk153_5.gif)

Bu bizim için, istemci tarafının gerçektende konfigurasyon dosyasında belirtilen ayarlar ile bir uzak sunucuya bağlanmaya çalıştığının ve hakiki bir remoting ortamının oluştuğunun göstergesidir. Bir başka deyişle RemObj sınıfına ait nesne örneğini istemci kendi application domani'i içerisinde oluşturmamaktadır. Gerçektende bu örneğe ait referansı sunucu tarafında aramaktadır. Bunuda gördükten sonra, artık remoting uygulamamızı test edebiliriz. Önce sunucu uygulamamızı çalıştırıyoruz ardınan ise istemci uygulamamızı. Sonuçta, istemci sunucu üzerindeki uzak nesneyi kullanarak bir olayı tetikliyor ve bunun karşılığında sunucu uygulamada olay metodunu istemci tarafında yürütmeye başlıyor.

Aşağıdaki Flash animasyonunda uygulamanın canlı olarak çalışmasını görmektesiniz. (Not: Sisteminizde Flash Player yüklü değilse bu animasyonu göremeyebilirsiniz.)

Peki uygulamada neler oldu şöyle bir analiz etmeye çalışalım. Öncelikle server uygulamamızı çalıştırdık. Hemen ardından istemci uygulamamızı çalıştırdık. İstemci uygulamamız kodları içerisinde uzak nesne örneği oluşturacak kod satırı (RemObj ro=new RemObj (); satırı) geçildikten sonra sunucu tarafında yer alan uzak nesne (RemObj) sınıfına ait bir referans oluşturuldu. Bunu sunucu ekranına gelen mesajdan anlayabiliriz. Nitekim bu mesajı uzak nesnenin constructor metodu içerisinde yazmıştık. Bu işlemi takiben istemci tarafından tetikleyici metod (EventTrigger) çağırıldıktan sonra ise sunucu tarafında, istemcinin olayı tetikleme zamanı yazıldı. Burada olayı daha iyi analiz etmek için olay tetikleyici metodumuz içerisinde kodu 10 saniye kadar uyuttuk. Bu süre sonunda sunucu uygulama, istemci tarafındaki olay metodunu (EventTrigger) çalıştırdı ve istemci tarafında o anki zaman ekrana geldi. Böylece remoting uygulaması için geçerli event modelinin başarılı bir şekilde çalıştığını ispat etmiş oluyoruz.

Gördüğünüz gibi remoting uygulamalarında istemci tarafından olay tetiklemek ve bu olay karşılığında sunucunun kendisine bağlı olan istemciler üzerinde var olan olay metodlarını çalıştırabilmesini sağlamak mümkün. Böylece geldik bir makalemizin daha sonuna bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek uygulamalı indirmek için tıklayın.](/assets/files/2006/EventHandling.rar)