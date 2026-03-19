---
layout: post
title: "Client Side SponshorShip"
date: 2006-04-12 12:00:00 +0300
description: "Son makalemizde, remoting uygulamalarında uzak nesnelere ait kiralama sürelerinin (Lease Time) nasıl yönetilebileceğinden bahsetmiştik. Bununla birlikte bir uzak nesnenin kiralama süresinin sonlamasının ardından gelecek olan metod çağrılarında istemcilerin olmayan bir referansa erişmeye çalıştığını ve bu nedenlede çalışma zamanı istisnaları ile karşılaşabileceklerini görmüştük. Bu makalemizde, kiralama sürelerini otomatik olarak uzatmak için istemci taraflı destek modelinden (Client Side sponsorShip) nasıl yararlanabileceğimizi incemeleye çalışacağız."
categories:
  - dotnet-remoting
tags:
  - dotnet-remoting
  - csharp
  - xml
  - threading
---
# Client Side SponshorShip
Değerli Okurlarım Merhabalar,

Son makalemizde, remoting uygulamalarında uzak nesnelere ait kiralama sürelerinin (Lease Time) nasıl yönetilebileceğinden bahsetmiştik. Bununla birlikte bir uzak nesnenin kiralama süresinin sonlamasının ardından gelecek olan metod çağrılarında istemcilerin olmayan bir referansa erişmeye çalıştığını ve bu nedenlede çalışma zamanı istisnaları ile karşılaşabileceklerini görmüştük. Bu makalemizde, kiralama sürelerini otomatik olarak uzatmak için istemci taraflı destek modelinden (Client Side sponsorShip) nasıl yararlanabileceğimizi incemeleye çalışacağız.

SponsorShip mimarisi temel olarak, uzak nesnelerin yaşam sürelerini otomatik olarak arttırmak için kullanılır. İki şekilde uygulanabilir. Bunlardan birisi istemci tarafında diğeri ise sunucu tarafında yapılabilen destekleme sistemidir. Her iki destek türününde birbirlerine göre avantajları ve dezavantajları vardır. İstemci taraflı destek modelinde (Client Side Sponsorship), istemcinin kullandığı uzak nesnenin kiralama yöneticisi (Lease Manager) ile, ISponsor arayüzünden türetilen bir sınıfın iş birliği söz konusudur.

Bu işbirliğinin bir sonucu olarak, uzak nesneye ait referansın yaşam süresini doldurması halinde, istemci taraflı sponsor otomatik olarak devreye girecek ve kiralama süresini uzatacaktır. Bu modelin çalışabilmesi için, sunucudan gelen geri bildirimlerin (Callbacks) istemci tarafından ele alınabilmesi gerekmektedir. Bu da istemcinin yeri geldiğinde sunucu isteklerini kabul eden bir davranış sergilemesi demektir. İşte bu nedenle, özellikle firewall gibi güveli sistemlerin arkasında kalan istemcilere sunucunun erişememesi halinde istemci taraflı destek modeli bir işe yaramayacaktır. Bu istemci taraflı destek sistemi açısından bir dezavantaj olarak görülebilir.

Peki istemci taraflı modeli nasıl uygulayacağız? Bunu anlamanın en iyi yolu basit bir örnek üzerinden gitmek ile olacaktır. Teorik olarak yapmak istediğimiz şey, istemcilerin kullandığı uzak nesnelere ait kirlama sürelerini otomatik olarak arttırabilmek ve böylece çalışma zamanında meydana gelecek kayıp referans çağrılarının önüne geçebilmektir. İlk olarak uzak nesnemizi geliştirmekle işe başlayalım. Uzak nesnemizin aşağıda görülen modele sahip olduğunu düşünelim.

![mk156_1.gif](/assets/images/2006/mk156_1.gif)

```csharp
public class RemoteObj : MarshalByRefObject
{
    public RemoteObj()
    {
        Console.WriteLine("Uzak nesne yapıcı metodu çağırıldı...");
    }

    public int GetTotal(int orderID)
    {
        return orderID * 100;
    }
}
```

Şimdi vakit kaybetmeden sunucu uygulamamızıda aşağıdaki gibi geliştirelim.

```csharp
static void Main(string[] args)
{
    RemotingConfiguration.Configure("..\\..\\ServerApp.config",true);
    Console.WriteLine("Sunucu dinlemede...Kapatmak için bir tuşa basın...");
    Console.ReadLine();
}
```

Sunucu uygulamamıza ait remoting ayarlarını bir konfigurasyon dosyasında tutacağız. Dikkat ederseniz, kiralama süresini mümkün olduğunca kısa tutmaya çalıştık. Uzak nesneye ait bir referans oluşturulduktan sonra yaklaşık olarak 3 saniyelik bir ömrü olacaktır. Diğer taraftan remoting mimarimizi CAO (Client Activated Object) modelini baz alarak geliştirdik. Bu nedenle istemci tarafında bir uzak nesne örneği oluşturulur oluşturulmaz, sunucu üzerinde bu nesneye bağlı bir referans hemen oluşturulacaktır.

```xml
<?xml version="1.0" encoding="utf-8" ?>
    <configuration>
        <system.runtime.remoting>
            <application>
                <service>
                    <activated type="RemoteLib.RemoteObj,RemoteLib"/>
                </service>
                <lifetime leaseTime="3" renewOnCallTime="1" leaseManagerPollTime = "1" />
                <channels>
                    <channel ref="tcp" port="4567">
                        <serverProviders>
                            <formatter ref="binary" typeFilterLevel="Full"/>
                        </serverProviders>
                        <clientProviders>
                            <formatter ref="binary"/>
                        </clientProviders>
                    </channel>
                </channels>
            </application>
        </system.runtime.remoting>
    </configuration>
```

Bu konfigurasyon dosyasında standart ayarların yanında özellikle channel boğumu içerisinde yer alan serverProviders ve clientProviders isimli alt boğumlar hemen göze çarpmaktadır. Bu alt boğumlar aynen istemci tarafındada kullanılmak zorundadır.

![dikkat.gif](/assets/images/2006/dikkat.gif)
Konfigurasyon dosyasında serverProviders ve clientProviders boğumlarını kullanmassak (ki istemci destek modelinde bunları hem istemci hem sunucu konfigurasyon dosyalarında kullanmalıyız) güvenlik ile ilgili bir çalışma zamanı hatasını aşağıdaki gibi alırız.

![mk156_4.gif](/assets/images/2006/mk156_4.gif)

Gelelim istemci uygulamamıza. İstemci uygulamamızı ilk olarak aşağıdaki gibi herhangibir destek nesnesi (sponsor object) olmadan geliştireceğiz. İstemci uygulamamız basit olarak, uzak nesneye ait bir nesne örneğini client activated object modelinde oluşturmaktadır. Daha sonra istemci, uzak nesne referansını kullanarak GetTotal isimli metodu arka arkaya 10 defa çağırmaktadır. Bu metod çağırımlarının her birinde istemci uygulama yaklaşık olarak 3 saniye süreyle uyutulmaktadır. Dolayısıyla bu gecikmeler, uzak referansın kiralama süresinin dolmasına ve bu süre dolmadan metod çağırımlarının tamamlanamamasına neden olacaktır. İşte buda bizim sponsor yönetimine gitmemizi sağlayacak etkendir.

```csharp
static void Main(string[] args)
{
    RemoteObj rm = null;

    try
    {
        RemotingConfiguration.Configure("..\\..\\ClientApp.config", true);
        rm = new RemoteObj();
  
        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine(i+"nci çağrı..."+rm.GetTotal(i).ToString());
            System.Threading.Thread.Sleep(3000);
        }
    }
    catch (Exception err)
    {
        Console.WriteLine(err.Message);
    }
    finally
    {
        
    }
    Console.WriteLine("Programı kapatmak için bir tuşa basın...");
    Console.ReadLine();
}
```

İstemci uygulamamıza ait konfigurasyon dosyası ise aşağıdaki gibidir. Dikkat ederseniz, channel bilgisinde tcp tipini ve 0 numaralı bir portu hizmete sunduğumuzu görüyorsunuz. Port bilgisinin 0 olması, bir sunucunun bu istemciye bağlanmaya çalışması sırasında, uygun olan boş portlardan birinin sunucuya hizmet için tahsis edilmesini sağlar. Burada böyle bir channel tipi kullanmamızın tek nedeni, istemcinin destek modelini kullanabilmesini sağlamaktır. Nitekim bu modelde, sunucu uygulama, istemci tarafına geri bildirimlerde bulunabilmelidir. Buda istemcinin yeri geldiğinde sunucu gibi davranmasını gerektirir ki bu ancak uygun bir portun sunucuya verilmesi ile gerçekleşebilir.

```xml
<?xml version="1.0" encoding="utf-8" ?>
    <configuration>
        <system.runtime.remoting>
            <application>
                <client url="tcp://manchester:4567">
                    <activated type="RemoteLib.RemoteObj,RemoteLib"/>
                </client>
                <channels>
                    <channel ref="tcp" port="0">
                        <serverProviders>
                            <formatter ref="binary" typeFilterLevel="Full"/>
                        </serverProviders>
                        <clientProviders>
                            <formatter ref="binary"/>
                        </clientProviders>
                </channel>
            </channels>
        </application>
    </system.runtime.remoting>
</configuration>
```

İstemci uygulamamızı bu haliyle test ettiğimizde çalışma zamanında daha ikinci metod çağırımına geçemeden aşağıdaki ekranda görülen hata mesajı ile karşılaşırız.

![mk156_3.gif](/assets/images/2006/mk156_3.gif)

Bu hatanın nedeni son derece açıktır. Uzak nesneye ait referansın kiralama süresi (Lease Time) dolduğu için istemci uygulamamız, çalışma zamanında olmayan bir referansa ait kayıp metod çağırılarında bulunmaktadır. O halde artık, destek modelini kullanarak bu hatanın önüne nasıl geçebileceğimize bakabiliriz. İlk olarak istemci taraflı destek mimarisini (Client Side SponsorShip) kullandığımız için, istemci tarafında bir takım değişiklikler yapmamız gerekecektir. Bunlardan birincisi, sponsor hizmetini kullanacak olan bir sınıfın tasarlanmasıdır. Bu sınıfı aşağıdaki gibi geliştirdiğimizi düşünebiliriz.

![mk156_2.gif](/assets/images/2006/mk156_2.gif)

```csharp
class ClientSponsor : MarshalByRefObject, ISponsor
{
    #region ISponsor Members

    public TimeSpan Renewal(ILease lease)
    {
        Console.WriteLine("Sponsor süre yenileme metodu çalıştırıldı.");
        return TimeSpan.FromSeconds(5);
    }

    #endregion
}
```

Dikkat ederseniz, istemci taraflı bir sponsor nesnesi MarshalByRefObject ile ISponsor tiplerinden türer. ISponsor tipi, Renewal isimli bir metod sunar. Bu metodun geri dönüş tipi TimeSpan türünden bir süredir ve sponsor nesnesinin ilişkilendirildiği uzak nesne referansının yeni kiralama süresini belirtmektedir. Bu metod Lease Manager ile konuşabilmek için parametre olarak ILease arayüzü tipinden bir nesneyi kullanır.

Örneğimizde kiralama süresini 5 saniye kadar uzatıyoruz. Dolayısıyla, uzak nesne refaransına ait kiralama süresinin sonlanması halinde oluşacak kayıp metod çağırılarının da önüne geçmiş oluyoruz. Ancak henüz işimizi tamamlamış değiliz. Yazmış olduğumuz bu sınıfın görevini yerine getirebilmesi için, çalışma zamanında güncel Lease Manager'a kayıt edilmesi (register) gerekmektedir. Kaydetme işleminin tam tersi olan unregister işlemi ise, sponsor nesnesi ile Lease Manager'ın ortaklığını kesen bir davranış gösterir. İstemci uygulamamızın client side sponsor sınıfını kullanacak yeni hali aşağıdaki gibi olmalıdır.

```csharp
static void Main(string[] args)
{
    ILease il=null;
    ClientSponsor cs=null;
    RemoteObj rm = null;

    try
    {
        RemotingConfiguration.Configure("..\\..\\ClientApp.config", true);
        rm = new RemoteObj();
    
        #region Sponsor register edilir.

        il = (ILease)rm.GetLifetimeService();
        cs = new ClientSponsor();
        il.Register(cs);

        #endregion

        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine(i+"nci çağrı..."+rm.GetTotal(i).ToString());
            System.Threading.Thread.Sleep(3000);
        }
    }
    catch (Exception err)
    {
        Console.WriteLine(err.Message);
    }
    finally
    {
        il.Unregister(cs);
    }
    Console.WriteLine("Programı kapatmak için bir tuşa basın...");
    Console.ReadLine();
}
```

Sponsor sınıfımıza ait nesne örneğini çalışma zamanında, istemci tarafından kullanılan uzak nesne referansına ait Lease Manager ile ilişkilendirebilmek için yine güncel Life Time Servisini elde etmemiz gerekir. Bunun sonucu olarak oluşan ILease arayüzü tipini kullanarak sponsor nesnemizi Lease Manager için kayıt edebiliriz.(Register) Uygulamamızı şimdi bu haliyle çalıştıracak olursak hiç bir problemle karşılaşılmadığını görürüz. Dikkat ederseniz belirli zaman aralıklarında kiralama süreleri sona ermeden otomatik olarak uzatılmaktadır.

Tüm metodlar çalıştırılmasını tamamladıktan sonra, finally bloğu içerisinde sponsor sınıfımıza ait nesne örneğide unregister edilerek Lease Manager ile olan işbirliğine son verilmektedir. Görüldüğü gibi istemci taraflı destek modeli ile, kiralama süreleri sona ermiş olan referansların yol açacağı kayıp metod çağırımlarının önüne geçebilir ve uygulamanın devamlılığını sağlayabiliriz. İstemci taraflı bu modelin dışında bir de sunucu taraflı bir destek modeli olduğundan bahsetmiştik. (Server Side Sponsorship) Bu modelide bir sonraki makalemizde incelemeye çalışacağız. Böylece geldik bir makalemizin daha sonuna bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek kod için tıklayınız.](/assets/files/2006/ClientSponsor.rar)