---
layout: post
title: "Server Side SponsorShip"
date: 2006-04-19 12:00:00 +0300
categories:
  - dotnet-remoting
tags:
  - dotnet-remoting
  - csharp
  - xml
  - threading
---
Bir önceki makalemizde Remoting mimarisinde istemci taraflı destek modelini incelemeye çalışmıştık. İstemci taraflı destek modelinin en büyük problemlerinden birisi, istemcilerin firewall arkasında olması halinde ortaya çıkmaktadır. Bu engel, sunucuların istemcilere erişimini kısıtlayacağından istemci taraflı destek modelinin çalışması garanti altına alınmamış olabilir. Bu nedenle, istemcilerin firewall arkasında olup olmadıklarının bilinmediği durumlarda kesinlikle sunucu taraflı destek (server side sponsorship) modeli kullanılmalıdır. Bu makalemizde sunucu taraflı destek modelinin işleyiş şeklinden bahsedecek ve örnek bir uygulama geliştireceğiz.

İlk olarak modelin teorisinden birazda olsa bahsetmekte fayda var. Sunucu taraflı destek modelinde, uzak nesne haricinde bu nesnenin kiralama süresini kontrol eden ve gerektiğinde otomatik olarak uzatan bir destek nesnesi (Sponsor Object) vardır. Yanlız bu sponsor nesne sunucu tarafında olduğu için, istemci tarafından bir şekilde çağırılabilmeli ve kullanılabilmelidir. Dolayısıyla uzak nesnenin kiralama süresini kontrol eden sponsor nesnemizde aslında bir uzak nesnedir. Yani MarshallByRefObject tipinden türetilmiştir. Diğer taraftan bu sponsor nesnenin, uzak nesne referansına ait kiralama süresini, sunucu tarafında kontrol edebilmesi için ayrıca ISponsor arayüzünüde uygulaması gerekmektedir. Bildiğiniz gibi ISponsor arayüzünün sağladığı Renewal metodu ile kiralama süreleri uzatılabilir.

Peki istemci tarafındaki uygulamalar uzak nesnenin kiralama süresini kontrol eden bu sponsor nesnesini nasıl kullanmalıdır? İstemci, uzak nesnesine ait kiralama süresinin ömrünü kontrol edecek uzak sponsor nesnesinide belirli aralıklarla ele alabilmelidir. Bu amaçla, istemci uygulama üzerinde sadece uzak sponsor'u belirli periyotlarla kontrol eden ve ayrı bir thread içerisinde çalışacak başka bir sınıf daha gereklidir. Bu sınıf içerisinde belirli zaman aralıklarında, uzak sponsor nesnesine ait herhangibir üye metod çağırılır. Buradaki amaç, istemci tarafından bir şekilde uzak sponsor nesnesini aktif olarak tutmaktır. Nitekim, uzak nesne kiralama süresinin yönetecek olan referans sunucu üzerinde duran sponsor nesne referansıdır.

Model biraz karışık görünebilir. Ancak örneğimizi geliştirirken çok daha net anlaşılabileceğini düşünmekteyim. Dilerseniz hiç vakit kaybetmeden örneğimizi geliştirelim. İşe hem sunucu hemde istemci uygulamamızın ortak olarak kullanacağı class library'yi tasarlamakla başlayalım. Bu kütüphane içerisinde, hem uzak nesnemiz, hem de uzak sponsor nesnemiz için gerekli interface tanımlamalarını yapacağız.

![mk157_1.gif](/assets/images/2006/mk157_1.gif)

```csharp
using System;
using System.Runtime.Remoting.Lifetime;

namespace RemoteFace
{
    public interface IRemoteObject
    {
        double Toplam(double x, double y);
    }

    public interface ISponsorObject:ISponsor 
    {
        void CanliKal();
    }
}
```

IRemoteObject isimli arayüzümüz (interface), uzak nesnemiz için gerekli prototipi sunmaktadır. Bu kütüphanede asıl önemli olan arayüz ise ISponsorObject arayüzümüzdür. Bu arayüzümü ayrıca ISponsor arayüzünden de türettik. Böylece sponsor nesnemizin hem ISponsorObject arayüzünü uygulamasını hem de, kiralama yönetimi için gerekli olan ve ISponsor arayüzünden gelen Renewal metodunu uygulamasını sağlamış olacağız. Burada özellikle interface'leri kullanmamızın nedeni bildiğiniz gibi, istemci uygulamanın sunucu tarafındaki uzak nesnelerin sağladığı metodlarda olacak değişikliklerden etklilenmemesini sağlamaktır. Öyleki biz istemci tarafında bu arayüzleri kullanarak uzak nesne ve sponsorumuza ait referansları çağırabileceğiz. Polimirfizim (Polymorphsym) sağolsun. Gelelim sunucu tarafına. Sunucu tarafımızdaki uygulamamızda uzak nesnemizi ve uzak sponsor nesnemizi aşağıdaki gibi oluşturacağız.

![mk157_2.gif](/assets/images/2006/mk157_2.gif)

Uzak Nesne Sınıfımız;

```csharp
class RemoteObj:MarshalByRefObject,IRemoteObject 
{
    public RemoteObj()
    {
        Console.WriteLine("Uzak nesne örneği oluşturuldu " + DateTime.Now.ToString());
    }
    #region IRemoteObject Members

    public double Toplam(double x, double y)
    {
        return x + y;
    }

    #endregion
}
```

Uzak Sponsor Sınıfımız;

```csharp
class ServerSponsor:MarshalByRefObject,ISponsorObject
{
    public ServerSponsor()
    {
        Console.WriteLine("Uzak sponsor nesne oluşturuldu " + DateTime.Now.ToString());
    }

    #region ISponsorObject Members

    public void CanliKal()
    {
        Console.WriteLine("Canli kal metodu çağırıldı " + DateTime.Now.ToString());
    }

    #endregion

    #region ISponsor Members

    public TimeSpan Renewal(System.Runtime.Remoting.Lifetime.ILease lease)
    {
        Console.WriteLine("Uzak nesne için kiralama süresi yenilendi " + DateTime.Now.ToString());
        return TimeSpan.FromSeconds(6);
    }

    #endregion
}
```

Uzak nesne sınıfımızdan ziyade, uzak sponsor nesne sınıfımızın işleyişi bizim için çok daha önemlidir. Dikkat ederseniz, sponsor sınıfımızın içerisinde bir iş yapmayan CanliKal isimli bir metod vardır. Bu metodu istemci tarafında yer alan başka bir sınıfımız kullanacak. Bunu biraz sonra açıklamakta fayda var. Sponsor sınıfımızın Renewal metodu kiralama süresini 6 saniye kadar uzatıyor. Peki bu kimin kiralama süresi? İşte istemci tarafında yer alacak kodlarımızda, bu sponsor nesnesinin ele alacağı Lease Manager'ı seçerken, uzak nesnenin kiralama yöneticisini ele alıp sponsor nesnemize göndereceğiz. Bir başka deyişle, uzak nesnemizin kiralama yöneticisine, sponsor nesnemizi register edeceğiz. Böylece uzak nesnenin kiralama süresini, sunucu üzerindeki sponsor nesne örneğimiz üstlenmiş olacak. Vakit kaybetmeden sunucu uygulamamızın kodlarını ve konfigurasyon dosyasını aşağıdaki gibi geliştirelim.

Server;

```csharp
static void Main(string[] args)
{
    RemotingConfiguration.Configure(@"..\\..\\ServerApp.config",true);
    Console.WriteLine("Sunucu dinlemede...");
    Console.ReadLine();
}
```

ServerApp.config

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.runtime.remoting>
        <application>
            <channels>
                <channel ref="tcp" port="4378">
                    <serverProviders>
                        <formatter ref="binary" typeFilterLevel="Full" />
                    </serverProviders>
                </channel>
            </channels>
            <service>
                <wellknown type="Server.RemoteObj,Server" objectUri="RemObj" mode="Singleton"/>
                    <wellknown type="Server.ServerSponsor,Server" objectUri="RemSpn" mode="Singleton"/>
            </service>
            <lifetime leaseTime="6" renewOnCallTime="2" leaseManagerPollTime="1"/>
        </application>
    </system.runtime.remoting>
</configuration>
```

Dikkat ederseniz, hem uzak nesnemiz hemde sponsor nesnemiz için ayrı ayrı SAO (Server Activated Object) tipinde ve Singleton modeline uygun tanımlamalar yapıyoruz. Sunucu üzerinde çalışacak uzak nesnelerimiz için geçerli kiralama sürelerinide lifetime boğumunda (node) belirtmekteyiz. Elbette, buradaki kiralama süreleri tüm referanslar için geçerli olacaktır. Yani hem uzak nesnemiz hemde sponsor nesnemiz için. Dilerseniz bu referansların kiralama sürelerini ayrı ayrıda tanımlayabilirsiniz. Bunun için tek yapmanız gereken InitializeLifetimeService metodunu uzak nesne sınıfları içerisinde override etmektir.

Gelelim istemci tarafındaki kodlara. Sunucu taraflı sponsor kullanımına ait teoriden bahsederken, istemci tarafında sunucu üzerindeki sponsor referansını düzenli olarak tetikleyecek bir sınıf olacağından bahsetmiştik. İşte bu sınıfımızı istemci uygulamamızda aşağıdaki gibi geliştireceğiz.

![mk157_3.gif](/assets/images/2006/mk157_3.gif)

PollingObject

```csharp
class PollingObject
{
    private bool _canliBirak;
    private ISponsorObject _sponsorObject;

    public bool CanliBirak
    {
        get { return _canliBirak; }
        set { _canliBirak = value; }
    } 

    public PollingObject(ISponsorObject sponsorObject)
    {
        _canliBirak = true;
        _sponsorObject = sponsorObject;
        Thread currTrd=new Thread(this.CanliTut);
        currTrd.Start();
    }

    public void CanliTut()
    {
        while (CanliBirak)
        {
            _sponsorObject.CanliKal();
            Thread.Sleep(2000);
        } 
    }
}
```

PollingObject isimli sınıfımızın en büyük özelliği, örneği oluşturulurken parametre olarak ISponsorObject tipinden bir nesne alması. Bu çalışma zamanında bizim oluşturacağımız uzak sponsor nesne örneğimiz (ServerSponsor) olacaktır. Basit olarak yapıcı metod bu nesne örneğini kullanacak CanliTut isimli metodu, ayrı bir thread içerisinde ve CanliBirak isimli özellik değeri true olduğu müddetçe, çağıracaktır. Bu metodun çağırılması ile, istemci tarafından kullanılan uzak nesneye ait kiralama süresinin, sunucu üzerindeki sponsor nesne örneği tarafından kontrol altına alınması sağlanmış olunur. İşleyişi istemci uygulamamızın kodları çok daha iyi anlatmaktadır.

Client

```csharp
static void Main(string[] args)
{
    RemotingConfiguration.Configure("..\\..\\ClientApp.config", true);
    
    IRemoteObject remObj = (IRemoteObject)Activator.GetObject(typeof(IRemoteObject), "tcp://manchester:4378/RemObj");

    #region Server Side Sponsor Kullanılmaya Başlanır

    ISponsorObject spnObj = (ISponsorObject)Activator.GetObject(typeof(ISponsorObject), "tcp://manchester:4378/RemSpn");

    PollingObject pllObj = new PollingObject(spnObj);

    ILease il = (ILease)((MarshalByRefObject)remObj).GetLifetimeService();
    il.Register(spnObj);

    #endregion

    for (int i = 0; i < 8; i++)
    {
        Console.WriteLine(remObj.Toplam(i, i + 1).ToString());
        System.Threading.Thread.Sleep(4000);
    }
    Console.WriteLine("Metodlar sonlandırıldı...");
    il.Unregister(spnObj);
    pllObj.CanliBirak = false;
    Console.WriteLine("Programı kapatmak için bir tuşa basın...");
    Console.ReadLine();
}
```

İlk olarak uzak nesnemizi (RemoteObject) kullanabilmemizi sağlayacak IRemoteObject referansı elde edilir. Ardından aynı yol ile ISponsorObject referansı elde edilir. Sonrasında ise PollingObject tipimize ait bir nesne örneğini oluşturuyoruz. İşte bu andan itibaren, parametre olarak gönderdiğimiz spnObj arayüzü ile sunucu üzerindeki sponsor nesne referansını kullanmaya başlıyoruz. İzleyen satırda, uzak nesne örneğini referans eden remObj'nin kiralama yöneticisini ILease arayüzüne atıyoruz. İşte can alıcı nokta burası. ILease referansına, sunucu üzerinde yer alan sponsor nesnemizi kayıt ediyoruz. Dolayısıyla, PollingObject içerisinde ayrı bir iş parçacığı olarak çalışan CanliTut isimli metod 2 saniyelik aralıklarla uzak sponsor nesne referansının CanliKal metodunu çağırmaya başlıyor. Bu çağırılar sonucu uzak nesnenin kiralama süreside, ILease referansını unregister edip, CanliTut metodunun işleyişini CanliBirak isimli özelliğe false değerini atayıp kesinceye kadar, uzamaya devam ediyor. Kısacası, istemcinin kullandığı uzak nesnenin kiralama süresinin yönetimini, sunucu üzerinde yer alan ve istemci tarafından belirli periyotlarla kontrol edilen uzak sponsor nesne referansına devretmiş oluyoruz. Uygulamamızı test etmeden önce, istemci tarafındaki konfigurasyon dosyasınıda aşağıdaki gibi oluşturalım.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.runtime.remoting>
        <application>
            <channels>
                <channel ref="tcp">
                    <clientProviders>
                        <formatter ref="binary" />
                    </clientProviders>
                </channel>
            </channels>
            <client>
                <wellknown type="RemoteFace.IRemoteObject,RemoteFace" url="tcp://manchester:4378/RemObj/"/>
                <wellknown type="RemoteFace.ISponsorObject,RemoteFace" url="tcp://manchester:4378/RemSpn/"/>
            </client>
        </application>
    </system.runtime.remoting>
</configuration>
```

Şimdi olayı iyice analiz edebilmek için client tarafındaki kodlarımızda sponsor nesne ile ilgili olan kısımları kaldırıp çalıştıracağız.

```csharp
static void Main(string[] args)
{
    RemotingConfiguration.Configure("..\\..\\ClientApp.config", true);
    
    IRemoteObject remObj = (IRemoteObject)Activator.GetObject(typeof(IRemoteObject), "tcp://manchester:4378/RemObj");

    for (int i = 0; i < 8; i++)
    {
        Console.WriteLine(remObj.Toplam(i, i + 1).ToString());
        System.Threading.Thread.Sleep(4000);
    }
    Console.WriteLine("Metodlar sonlandırıldı...");
    Console.WriteLine("Programı kapatmak için bir tuşa basın...");
    Console.ReadLine();
}
```

Kullandığımız uzak nesne örneğini SAO olarak Singleton modunda tasarlamıştık. Bununla birlikte uzak nesne için varsayılan kiralama sürelerinide belirtmiştik. İstemci tarafı peş peşe uzak nesne metodlarını çağırdığında, her kiralama süresi sonlandıktan sonra sunucu üzerinde yeni bir referansın oluşturulmuş olması gerekmektedir. Yani yapıcı metodların birden fazla kez çağırılması durumu söz konusudur.

![mk157_4.gif](/assets/images/2006/mk157_4.gif)

Gördüğünüz gibi, uzak nesne örneğine ait kiralama süreleri sona erdikçe yeni uzak nesne referansları sunucu üzerinde oluşturulmuştur. Ancak Sponsor kodlarımızı tekrardan uygulamaya dahil edersek aşağıdaki sonuçları elde ederiz.(Aşağıdaki ekran görüntüsü flash formatında olup, flash player yüklemenizi gerektirebilir...)

Bu sefer, uzak nesne örneğimize ait tek bir referans tüm uygulama boyunca yaşamaktadır. Çünkü uzak sponsorumuz kiralama sürelerini otomatik olarak yenilemektedir. Sunucu taraflı destek kontrolü uygulanış açısından zor görünsede istemci taraflı destek modelinin bazı dezavantajlarını ortadan kaldırmaktadır. Örneğin FireWall engellerini. Bu yüzdende remoting uygulamalarında sıkça tercih edilen bir yöntemdir. Böylece geldik bir makalemizin daha sonuna bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek kod için tıklayınız.](/assets/files/2006/ServerSideSponsors.rar)