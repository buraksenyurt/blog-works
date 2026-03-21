---
layout: post
title: "WCF 4.5–ChannelFactory Tipi için Caching Kullanımı"
date: 2014-12-22 19:00:00 +0300
categories:
  - wcf
  - wcf-4-5
tags:
  - windows-communication-foundation
  - channel
  - channel-factory
  - caching
  - service-contract
  - iclientchannel
  - clientbase
  - generic
---
Vaktiyle üniversitedeki diferansiyel denklemler dersi hocamızın anlattığı bir efsane vardı (Sene 94 bu arada). Ne kadar gerçektir bilinmez ama beni oldukça etkilemişti. Hikayeye göre üniversite hocaları arasında belirli aralıklarla düzenlenen bir yarışma varmış. Bu yarışmada hocaların tahtaya kalkıp seçtikleri teoremlere ait geometrik şekilleri çizmeleri istenirmiş. En güzel çizim ise mükafatlandırılırmış.

[![PerfectCircle](/assets/images/2014/PerfectCircle_thumb.jpg)](/assets/images/2014/PerfectCircle.jpg)


Bir gün hocalar ardı ardına kalkıp tahtada hünerlerini göstermeye başlamışlar. Hemen hepsi rengarenk tebeşirler kullanıyormuş. Sarmallar, hiperboller, spiraller, üç boyutlu grafikler vb…

Derken sıra son yarışmacıya gelmiş. Hoca ayağa kalkmış yavaş adımlarla boş olan tahtalardan birine doğru yürümeye başlamış. Kısa bir süre durmuş ve diğer çizimlere imrenerek bakmış. Sonra önündeki boş tahtaya bir çember çizmiş ve ortasına da bir nokta yerleştirmiş.

Jüri diğer tahtaları dolaşıp puanladıktan sonra bu çemberin başında toplanmış. Çemberi ve ortadaki noktayı ölçüp biçmişler. Nokta, çemberin tam merkezindeymiş ve çember mükemmelmiş

![Smile](/assets/images/2014/wlEmoticon-smile_101.png)

Bu hikayenin yazımızın konusu ile doğrudan bir alakası yok tabi. Şöyle şehir efsanesi tadında bir giriş yapayım istedim. Gelelim asıl sıkıcı olan mevzumuza.

Bir WCF (Windows Communication Foundation) servisi ile onun tüketicisi olan istemci arasındaki iletişimde önem arz eden konulardan birisi de kanaldır (Channel). Bu kanalın oluşturulması görevini ChannelFactory tipi üstlenmektedir. İstemci açısından bakıldığında bir kanalın oluşturulması aslında servisin bir Proxy tipinin üretilmesi ve uzak metod çağrıları için gerekli iletişim ortamının sağlanması anlamına gelmektedir. Bir kanal esas itibariyle EndPoint odaklı üretilir. Dolayısıyla WCF'in ABC'si olarak nitelendirilen Address Binding Contract üçlemesi üzerine inşa olunur (ki bu da WCF Service EndPoint tanımıdır)

Tabi kanal üretimi sırasında devreye giren bir süreç de söz konusudur. Buna göre Sözleşme Tanımlama (Contract Description) ağacının üretilmesi, gerekli CLR (Common Language Runtime) tiplerinin reflecting ile açılması, kanal yığınının (Channel Stack) inşa edilmesi ve üretilen kaynaklardan sonlanması gerekenlerin sonlandırılması (Dispose işlemleri olarak düşünebiliriz). İşte bu süreç özellikle çalışma zamanına bir maliyet getirmektedir. Bu maliyeti aza indirgemek içinse genellikle özelleştirilmiş ChannelFactory tiplerinin yazılması yolu tercih edilir. Oysaki WCF 4.5 bu hazırlıkların kayıt altına alınıp yeniden ihtiyaç duyulduklarında hazır olarak sunulması için caching desteğini ClientBase tipi yardımıyla kullanıma açmıştır. İşte bu yazımızdaki amacımız ChannelFactory tipi için ön bellekleme işleminin nasıl yapılabileceğini incelemektir.

Ön Hazırlıklar

Elbette konuyu incelemek için basit bir WCF servisine ihtiyacımız olacak. Konumuz ChannelFactory ve bazı ön hazırlıkların ön belleklenmesi olduğundan servis tarafı oldukça sade ve basit şekilde inşa edilmiştir. Aşağıda servis sözleşmesi (Service Contract) ve uygulayıcı tipini içeren bir WCF Service Application kod içeriği bulunmaktadır.

[![cfcache_1](/assets/images/2014/cfcache_1_thumb.png)](/assets/images/2014/cfcache_1.png)

Servis sözleşmesi;

```csharp
using System.ServiceModel;

namespace Calculus 
{     
    [ServiceContract] 
    public interface IMathService 
    { 
        [OperationContract] 
        double Sum(int x,int y); 
    } 
}

Uygulayıcı tip;

using System;

namespace Calculus 
{ 
    public class MathService 
        : IMathService 
    { 
        public double Sum(int x, int y) 
        { 
            return x + y; 
        } 
    } 
}
```

web.config dosyası içeriğini ise standart ayarları ile bırakabiliriz. İstemci tarafını basit bir Console uygulaması şeklinde tasarlayabilir ve ilgili servisi Add Service Reference ile ekleyebiliriz. İstemci tarafındaki app.config dosyası otomatik olarak aşağıdakine benzer biçimde oluşturulacaktır.

```xml
<?xml version="1.0" encoding="utf-8" ?> 
<configuration> 
    <startup> 
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5.1" /> 
    </startup> 
    <system.serviceModel> 
        <bindings> 
            <basicHttpBinding> 
                <binding name="BasicHttpBinding_IMathService" /> 
            </basicHttpBinding> 
        </bindings> 
        <client> 
            <endpoint address="http://localhost:54837/MathService.svc" binding="basicHttpBinding" 
                bindingConfiguration="BasicHttpBinding_IMathService" contract="clcls.IMathService" 
                name="BasicHttpBinding_IMathService" /> 
        </client> 
    </system.serviceModel> 
</configuration>
```

Standart Proxy Kullanımı

Bu ön hazırlıkların ardından standart bir proxy kullanımı ile ilgili servisi kullanarak konumuza devam edelim. WCF servislerini çağırırken genellikle proxy tipinin doğrudan üretilmesi yolunu tercih ederiz. Söz gelimi aşağıdaki kod parçasında basit bir WCF servis çağrısı görülmektedir.

```csharp
using Student.clcls; 
using System;

namespace Student 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            MathServiceClient proxy = new MathServiceClient("BasicHttpBinding_IMathService"); 
            double result=proxy.Sum(3, 4); 
            Console.WriteLine(result.ToString()); 
            proxy.Close(); 
        } 
    } 
}
```

Hepimizin aşina olduğu bir kullanım şekli.

Client son eki ile biten sınıfa ait bir nesne örneği oluştur.
Parametre olarak konfigurasyon dosyası içerisindeki endPoint adını ver.
Örneklenen nesne üzerinden gerekli servis metodunu çağır.
Tüm işlemler bittikten sonra ise proxy nesnesinin ömrünü sonlandır.

ChannelFactory Kullanımı

Aynı örneğin ChannelFactory tipi ile olan kullanımı ise aşağıdaki gibidir.

```csharp
using Student.clcls; 
using System; 
using System.ServiceModel;

namespace Student 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            #region Channel Factory kullanımı

            ChannelFactory<IMathService> factory= new ChannelFactory<IMathService>("BasicHttpBinding_IMathService"); 
            IMathService channel = factory.CreateChannel(); 
            double result = channel.Sum(3, 4); 
            Console.WriteLine(result.ToString()); 
            ((IClientChannel)channel).Close();

            #endregion 
        } 
    } 
}
```

İlk olarak generic ChannelFactory tipinden bir nesne örneklendiğini görüyoruz. Bu nesnenin generic parametresi ise servis sözleşmesini ifade eden IMathService arayüzüdür (Interface). Nesnenin örneklenmesi sırasında yapıcı metoda (constructor) parametre olarak app.config içerisindeki ilgili EndPoint adı verilmektedir. Bu sayede servis için gerekli AddressBindingContract bilgileri alınmış olur.

İkinci satırda bir kanal nesnesi örneklendiğini görmekteyiz. Bunun için fabrika tipinin CreateChannel metodu kullanılmış durumdadır. CreateChannel metodu aslında generic olarak ChannelFactory örneklenmesi sırasında verilen arayüzün taşıyabileceği bir sınıfın üretiminde kullanılmaktadır.

Bundan sonraki kısım ise oldukça basittir. Üretilen proxy tipi üzerinden servis metoduna bir çağrı yapılır. Son olarak da oluşturulan kanalın kapatılması işlemi uygulanır.

Caching

Çok doğal olarak istemci tarafında kanal nesnesinin üretilmesinin ve iletişimin açılmasının çalışma zamanını ilgilendiren bir maliyeti söz konusudur. Yazımızın başında belirttiğimiz süreç nedeniyle oluşmaktadır. İşte bu sebepten geliştiriciler ChannelFactory tipinin özelleştirilmiş hallerini yazmak durumundadır. Lakin bu WCF 4.5’e kadar böyleydi. Artık WCF 4.5 ile birlikte bir kanalın inşa edilmesi işlemi için ön tanımlamaların ön belleklenerek kullanılması söz konusu. Peki ama nasıl?Aşağıdaki örnek kod parçasını inceleyerek devam edelim.

```csharp
using Student.clcls; 
using System; 
using System.ServiceModel;

namespace Student 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            #region Caching Kullanımı

            ClientBase<IMathServiceChannel>.CacheSetting = CacheSetting.AlwaysOn; 
            for (int i = 0; i < 5; i++) 
            { 
                MathServiceClient client = new MathServiceClient( 
                    new BasicHttpBinding() 
                    , new EndpointAddress("http://localhost:54837/MathService.svc") 
                    ); 
                double result = client.Sum(i, 5); 
                // İlk kullanımda kanal bilgisi cache' lenmiş durumda. Dolayısıyla sonraki çağrıda maliyet minimuma indirgenmiş olacak. 
                Console.WriteLine("{0}", result.ToString()); 
            }

            #endregion 
        } 
    } 
}
```

Caching ile ilişkili ayarlamaları yapmak için ClientBase tipine ait CacheSettings özelliğinden yararlanılmaktadır. Dikkat edileceği üzere söz konusu tipin kullanımı sırasında IMathServiceChannel parametre olarak verilmiştir.(IMathServiceChannel içeriğine bakıldığında bir IClientChannel türevi olduğu görülebilir)

Kodun bundan sonraki kısımlarında ise MathServiceClient sınıfının örneklendiğini görüyoruz. Binding ve Endpoint bilgileri ile oluşturulan nesne üzerinden de servis metoduna çağrıda bulunulmaktadır. for döngüsü nedeniyle aynı proxy tipinin bir kaç kez üretimi söz konusudur. İşte bu üretimlerin ilkinde ClientBase ile belirtilen ayar nedeniyle bir ön bellekleme işlemi söz konusudur.

Aslında proxy tipi olarak üretilen MathServiceClient sınıfı, generic ClientBase abstract sınıfından türemektedir. svcutil aracı proxy tipini bu şekilde üretmektedir (Doğal olarak Add Service Reference için de aynı durum söz konusudur)

[![cfcache_2](/assets/images/2014/cfcache_2_thumb.png)](/assets/images/2014/cfcache_2.png)

Bu sınıf açılımı nedeniyle CacheSetting özelliği proxy tipi üzerinden de doğrudan uygulanabilir.

CacheSetting Modları

CacheSetting özelliğine atanabilecek üç değer bulunmaktadır. AlwaysOn, Default ve AlwaysOff. Bu değerler ve aralarındaki farklılıklar aşağıdaki tabloda özetlenmeye çalışılmıştır.

CacheSetting Değeri
Anlamı

Default
Aynı Application Domain içerisinde olmak kaydıyla, konfigurasyon dosyasında tanımlanmış olan EndPoint bilgilerinden üretilmiş ClientBase örnekleri için Caching özelliği etkinleştirilir.

Bu Application Domain içerisinde oldukları halde programatik olarak üretilen ClientBase türevli tipler ise Caching’ e dahil edilmezler.

Ayrıca Credential bilgisi gibi Security özellikleri söz konusu olan ClientBase türevli örnekler de Caching dışında tutulurlar. Nitekim security bilgileri sürekli olarak değişebilir ve Cache tutulması bu anlamda doğru değildir.

AlwaysOn
Aynı Application Domain içerisinde yer alan tüm ClientBase türevli tipler için Caching özelliği etkinleştirilir.

Burada dikkat edilmesi gereken nokta security-sensitive bilgiler olması halinde Caching’ in pasifleştirilmeyeceğidir. Default moddakine aykırı bir durum söz konusudur.

AlwaysOff
Application Domain içerisindeki ClientBase türevli tipler için Caching özelliği kapatılır.

Bu yeni kabiliyet küçük çaplı servis örnekleri göz önüne alındığında pek fark edilmese de, enterprise çözümlerde kullanılan ve üretim maliyetleri de yüksek olan versiyonlar düşünüldüğünde ciddi olarak dikkate alınması gerekmektedir. Tabi burada söz konusu olan modlar arasındaki farklılıklara da bakılmalıdır. Özellikle security-sensitive tanımlamalarda Default ve AlwaysOn modların farklı davranışlar gösterdiği unutulmamalıdır. Konu ile ilişkili daha detaylı bilgiyi [MSDN sayfasından](http://msdn.microsoft.com/tr-tr/library/hh314046.aspx) alabilirsiniz. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[ChannelFactoryCaching.zip (66,14 kb)](/assets/files/2014/ChannelFactoryCaching.zip)