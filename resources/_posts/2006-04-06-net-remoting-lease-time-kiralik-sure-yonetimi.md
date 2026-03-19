---
layout: post
title: ".Net Remoting - Lease Time (Kiralık Süre) Yönetimi"
date: 2006-04-06 12:00:00 +0300
description: "Remoting mimarisi göz önüne alındığında dikkat çekici noktalardan bir tanesi, sunucu tarafında oluşturulan uzak nesnelerin (remote object) yaşam süreleridir. İstemciler, uzak nesnelere ait referansları kullanırken bunların yaşam sürelerini sunucu tarafındaki konfigurasyon belirler. Bu istemcilerin sunucu tarafındaki referanslara ait kaynaklara açıkça müdahale edememesinin de bir sonucu olarak görülebilir. Sunucu tarafında yapılan bu yaşam sürelerinin yönetimine kısaca Kiralık Süre Yönetimi (Lease Time Management) denmektedir. Bu makalemizde kısaca bu konuyu incelemeye çalışacağız."
categories:
  - dotnet-remoting
tags:
  - dotnet-remoting
  - csharp
  - xml
  - threading
---
# .Net Remoting - Lease Time (Kiralık Süre) Yönetimi
Değerli Okurlarım Merhabalar,

Remoting mimarisi göz önüne alındığında dikkat çekici noktalardan bir tanesi, sunucu tarafında oluşturulan uzak nesnelerin (remote object) yaşam süreleridir. İstemciler, uzak nesnelere ait referansları kullanırken bunların yaşam sürelerini sunucu tarafındaki konfigurasyon belirler. Bu istemcilerin sunucu tarafındaki referanslara ait kaynaklara açıkça müdahale edememesinin de bir sonucu olarak görülebilir. Sunucu tarafında yapılan bu yaşam sürelerinin yönetimine kısaca Kiralık Süre Yönetimi (Lease Time Management) denmektedir. Bu makalemizde kısaca bu konuyu incelemeye çalışacağız.

Bir istemci, uzak nesneye ait bir referans oluşturduğunda sunucu tarafında bu referans için bir geri sayım süresi başlatılır. Buna çoğunlukla Initial Lease Time (Başlangıç Kiralama Süresi) denir. Bu süre geriye doğru hareket eder ve sunucu tarafında çalışan Lease Manager yardımıyla belirli aralıklarla (varsayılan olarak 10 saniyede 1) kontrol edilir. Eğer bir referansın geriye doğru işleyen kiralama süresi sonlanırsa (ki bu o anki sürenin 0 olması anlamına gelir), garbage collector tarafından toplanılmak üzere işaretlenir. İşte bu andan sonra istemci, aynı referansa ait bir üyeyi çağırdığında çalışma zamanında bir exception ile karşılaşır. Bunun sebebi istemci tarafından oluşturulan uzak nesnenin sunucu üzerindeki kiralama süresinin sıfırlanmış bir başka deyişle yaşam süresinin bitmiş olması ve bu nesneye ait referansın artık sunucuda bulunamayışıdır.

Başlangıçta bir uzak nesnenin InitialLeaseTime süresi 300 saniyedir. Yani 5 dakika kiralama süresi söz konusudur. İstemcileri var olmayan bir referansa ait metodları çağırmaları halinde karşılaşacakları exception'lardan korumanın çeşitli yolları vardır. Bunlardan birisi uzak nesneye ait RenewOnCallTime özelliğinin değeridir. Bu değere verilen süre, bir uzak nesne referansının Lease Time süresi dolmadan önce, istemci tarafından gelebilecek bir metod çağırımının ele alınmasında önemli rol oynamaktadır. Bunu şöyle açıklayabiliriz. Bir uzak nesnenin kiralama süresinin 6 saniye olduğunu ve RenewOnCallTime süresininde 2 saniye olduğunu düşünelim. İstemci uzak nesneye ait bir referansı ilk oluşturduğunda, kiralama süresi geriye doğru azalmaya başlayacaktır. Sunucu üzerindeki güncel kiralama süresi (Current Lease Time) 2 saniyeye geldiğinde, eğer istemci tarafından bir metod çağırısı daha yapılırsa, nesnenin kiralama süresi otomatik olarak RenewOnCallTime'da belirtilen süreye ayarlanır. Bu durumda bu süre 2 saniye olacaktır. RenewOnCallTime süresinin işleyişini örneklerimizde daha yakından analiz edeceğiz ve daha kolay anlayacağız.

Uzak nesnelere ait referansların yaşam sürelerini istersek değiştirebiliriz. Bunu gerçekleştirebilmek için uzak nesneye ait kalıtım yolu ile gelen InitializeLifeTimeService isimli metodu override (ezmek) etmemiz gerekecektir. Bu metod temel olarak uygulandığı tipe ait kiralama sürelerinin yönetiminin sağlanmasından sorumludur. Buradaki ayarlamalar yardımıyla bir uzak nesne referansının InitialLeaseTime ve RenewOnCallTime sürelerini ayarlayabiliriz. Hatta referansın sonsuza dek (elbetteki çevre koşulları uygun olduğu sürece) yaşamasınıda sağlayabiliriz.

![dikkat.gif](/assets/images/2006/dikkat.gif)
Kiralama sürelerinin (Lease Time) yönetimi sadece Client Activated Object'ler için ve Server Activated Object'lerdede sadece Singleton modeli için geçerlidir. Yani SingleCall için kiralama süreleri uygulanamaz. Nitekim SingleCall modelinde uzak nesne referansları metod çağırımlarından sonra zaten yok edilirler.

Şimdi kiralama sürelerini yönetebileceğimiz basit bir örnek ile konumuzun derinlerine inmeye çalışalım. İlk olarak RenewOnCallTime süresinin sağladığı etkiyi incelemeye çalışacağız. Sunucu tarafımızda yer alacak uzak nesne modelinde, kiralama sürelerini yöneteceğiz ve ardından istemci tarafında bu nesneye ait örneklemelerin ve üye çağırımlarının etkilerini incelemeye çalışacağız. Model olarak işimizi kolaylaştırması açısından Client Activated Object tipinden uzak nesneler kullanacağız. İlk olarak uzak nesnemizi aşağıdaki gibi oluşturalım.

```csharp
public class RemoteObj : MarshalByRefObject
{
    public string Info(string prm)
    {
        return "Remote Metod " + prm;
    }

    public RemoteObj()
    {
        Console.WriteLine("Nesne oluşturuldu...");
    }

    public override object InitializeLifetimeService()
    {
        ILease lease = (ILease)base.InitializeLifetimeService();
        if (lease.CurrentState == LeaseState.Initial)
        {
            lease.InitialLeaseTime = TimeSpan.FromSeconds(6);
            lease.RenewOnCallTime = TimeSpan.FromSeconds(2);
        }
        return lease;
    }
}
```

Uzak nesnemizi incelediğimizde bizim için belkide en önemli metod override edilmiş olan InitializeLifeTimeService'dır. Peki bu ezme işlemi ile ne yapıyoruz biz? Bu metod tam olarak çalışma zamanında bir uzak nesnenin ilk oluşturulduğu sırada, sadece o nesne örneğine ait referans için geçerli olacak kiralama sürelerini belirlemektedir. Örneğin buradaki değerlere göre referans 6 saniye boyunca sunucu üzerinde kiralanacaktır. Eğer nesnenin ömrünün bitmesine 2 saniye kaldığında, bu son süre içerisinde de bir metod çağırımı gerçekleşirse kiralama süresi RenewOnCallTime'da belirtildiği üzere 2 saniyeye set edilecektir. Nesnemizin bu yaşam süreleri ile kullanışını incelemek amacıyla sunucu ve istemci tarafındaki kodlarımızı aşağıdaki gibi geliştirelim.

Sunucu;

```csharp
static void Main(string[] args)
{
    TcpServerChannel srvC = new TcpServerChannel(4500);
    ChannelServices.RegisterChannel(srvC,true);
    RemotingConfiguration.RegisterActivatedServiceType(typeof(RemoteObj));
    LifetimeServices.LeaseManagerPollTime=TimeSpan.FromSeconds(1);
    Console.WriteLine("Server dinlemede...");
    Console.ReadLine();
}
```

Sunucu tarafında yer alan kodlarımızda, LeaseManager için LeaseManagerPollTime süresini 1 saniye olarak ayarlamaktayız. Bu, LeaseManager'ın her bir saniyede, sunucu üzerindeki uzak nesne referanslarının kiralama sürelerini kontrol edeceği anlamına gelmektedir.

İstemci;

```csharp
static void Main(string[] args)
{
    try
    {
        TcpClientChannel cliC = new TcpClientChannel();
        ChannelServices.RegisterChannel(cliC, true);
        RemotingConfiguration.RegisterActivatedClientType(typeof(RemoteObj), "tcp://manchester:4500/");
        RemoteObj rmo = new RemoteObj();
        for (int i = 1; i < 6; i++)
        {
            Console.WriteLine(rmo.Info("Test " + i.ToString()));
            ILease currLease = (ILease)rmo.GetLifetimeService();
            Console.WriteLine(currLease.CurrentLeaseTime.ToString());
            System.Threading.Thread.Sleep(2000);
        }
    }
    catch (System.Exception err)
    {
        Console.WriteLine(err.Message);
    }
    Console.ReadLine();
}
```

İstemci tarafında yaptıklarımıza gelince. Burada uzak nesneye ait örnek oluşturulduktan sonra, bu referansa ait Info isimli metod arka arkaya 6 kez çalıştırılmaktadır. Her bir metod çağırımından sonra uzak nesne referansının sunucu üzerinde kalan kiralama süresi ömrü istemci tarafındaki Console penceresine yazılmaktadır. Daha sonrasında ise process 2 saniye kadar uyutulur. Şimdi uygulamanın çalışmasını izleyelim. (Aşağıdaki video görüntüsü flash formatında olup bilgisayarınıza flash player kurmanızı gerektirebilir.)

Burada dikkat ederseniz, metodun 3ncü çağırılışından itibaren nesneye ait kiralama süresi hep 2 saniye olarak kalmıştır. Bunun sebebi, 2nci çağırıştan sonra gelen 3ncü metod çağırışının artık RenewOnCallTime süresi sınırlarına dahil olmasıdır. Bu nedenle referansa ait kiralama süresi sonraki çağırımlarda sürekli olarak 2 saniyeye set edilmiştir. Bu sayede istemci 6 metod çağırımınıda başarılı bir şekilde yapabilmiştir. Oysaki kiralama süreleri aslında çok hassas dengeler üzerinde çalışmaktadır. Yapılacak ufak hatalar istemcileri başına sorun açabilir. Bunu görmek için istemci tarafındaki bekleme süresini 4 saniyeye çıkartalım ve uygulamamızı yeniden test edelim.(Aşağıdaki video görüntüsü flash formatında olup bilgisayarınıza flash player kurmanızı gerektirebilir.)

Dikkat ederseniz, 3ncü metod çağırımında uygulama bir istisna ile sonlanmaktadır. İlk metod çağırımında kiralama süresi yaklaşık olarak 6 saniye civarındadır. İkinci çağırıma geçerken uygulama 4 saniye duraksamıştır. Bu nedenle kiralama süresi bu nesne örneğimiz için yaklaşık 2 saniyeye kadar inmiştir. 3ncü metod çağırımında önce istemci uygulama 4 saniye daha bekler. Oysa bu süre içerisinde, uzak nesne referansı kiralama ömrünü doldurduğundan, garbage collector tarafından toplanılmak üzere işartlenir. Dahası bu 4 saniyelik bekleme süresi içerisinde başka bir metod çağırımı olmadığından RenewOnCallTime süresinin sınırlarıda artık geçerli değildir.

Dolayısıyla istemci artık olmayan bir referansa çağrıda bulunmaya çalışmaktadır. Bunun doğal sonucu olarakta istemci uygulamada bir istisna oluşacaktır. Dolayısıyla çalışma zamanında bu gibi durumların oluşabileceğini düşünmek ve buna göre hareket etmekte fayda vardır. Dilerseniz, bir uzak nesne referansının mümkün olduğunca uzun süre yaşamasını isteyebilirsiniz. Buna genellikle sonsuz kiralama süresi (infinity lease time) denir. Bunun için tek yapmanız gereken uzak nesnede ezdiğimiz InitializeLifetimeService metodundan geriye null bir referans döndürmek olacaktır.

```csharp
public override object InitializeLifetimeService()
{
    return null;
}
```

Bu durumda az önce duraklama süresi 4 saniye olan istemci uygulamayı çalıştırdığımızda bir sorun ile karşılaşmadan tüm metod çağrıların sonuçlandırıldığını görürüz. (Aşağıdaki video görüntüsü flash formatında olup bilgisayarınıza flash player kurmanızı gerektirebilir.)

Tabi burada istemci tarafında dikkat edilmesi gereken bir husus vardır. Buda Infinity Lease Time olması durumunda, istemci üzerinden o anki referansa ait CurrentLeaseTime süresinin okunamayacağıdır. Buda çalışma zamanında istisnaya neden olacak bir durumdur.

Bir uzak nesneye ait kiralama sürelerini sadece kod tarafında değil xml tabanlı konfigurasyon dosyalarında da tutabiliriz. Bildiğiniz gibi, Remoting uygulamalarının daha esnek olması açısından channel, port, type, objectUri gibi bir takım bilgileri konfigurasyon dosyalarında tutmaktayız. Dolayısıyla sunucu uygulamamız için gerekli konfigurasyon dosyasını aşağıdaki gibi oluşturabiliriz.

```xml
<configuration>
    <system.runtime.remoting>
        <application>
            <channels>
                <channel ref="tcp" port="4500" />
            </channels>
            <lifetime leaseTime="6" renewOnCallTime="2" leaseManagerPollTime = "1" />
            <service>
                <activated type="RemoteObjects.RemoteObj,RemoteObjects"/>
            </service>
        </application>
    </system.runtime.remoting>
</configuration>
```

Kiralama sürelerine ait bilgiler eğer aksi belirtilmesse her zaman saniye cinsinden uzunlukları temsil eder. Ancak farklı takılar kullanarak bu süre cinslerini değiştirmemiz mümkündür. Bu değerleri aşağıdaki tabloda bulabilirsiniz.

Harf
Anlamı
Örnek

D
Gün
3D (3 gün)

H
Saat
2H (2 saat)

M
Dakika
45M (45 dakika)

S
Saniye
15 (15 saniye)

MS
Milisaniye
100 (100 milisaniye)

Özetlemek gerekirse, bir uzak nesnenin varsayılan kiralama süresini (lease time) değiştirebilir ve referansların yaşam sürelerini etkilyebiliriz. Bu makalemizde bunu gerçekleştirmek için programatik olarak ve konfigurasyon bazında kullanabileceğimiz yolları inceledik. Bir sonraki makalemizde, Sponsor tekniğini incelemeye çalışacağız. Böylece geldik bir makalemizin daha sonuna bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek kod için tıklayınız.](/assets/files/2006/LeaseManagement.rar)