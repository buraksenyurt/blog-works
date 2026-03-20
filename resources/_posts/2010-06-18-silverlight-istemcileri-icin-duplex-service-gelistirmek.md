---
layout: post
title: "Silverlight İstemcileri için Duplex Service Geliştirmek"
date: 2010-06-18 04:50:00 +0300
categories:
  - silverlight-4-0
  - wcf
tags:
  - silverlight-4-0
  - wcf
  - csharp
  - xml
  - dotnet
  - aspnet
  - silverlight
  - http
  - iis
  - threading
  - delegates
  - generics
  - visual-studio
  - rc
---
Lost dizisinin müptelası olan arkadaşlar "Push The Button" repliğini bilirler. Hikayeye göre DHARMA girişimin labaratuvarında yer alan ve 108 dakikadan geriye doğru sayan bir numarator vardır. Zaman sayacı sıfırlanmadan önce toplamları 108 olan 4,8,15,16,23,42 sayı dizisinin klavyeden girilmesi ve Enter tuşuna basılması gerekmektedir. Tabi ben Lost dizisinin tüm sezonlarını takip etmemiş ve hatta sonunu getirememiş birisi olarak ne olduğunu tam olarak anlayabilmiş değilim.

![blg172_Giris.jpg](/assets/images/2010/blg172_Giris.jpg)

Lakin bu Push The Button mevzusunda düşündüğüm genelde, sunucu üzerindeki bir servisin Pusher Service olarak hizmet vermesi olmuştur. Buna göre, servis kendisine bağlı olan istemci modundaki lokasyonlara bir bildiri yapmaktadır diyerek konuyu bir şekilde bağlmaya çalışayım. Bu günkü yazımızda Silverlight istemcilerinin Duplex iletişim üzerinden hizmet verebilen servisler yardımıyla nasıl tetiklenebileceğini incelemeye çalışıyor olacağız. Sanıyorum Siverlight tabanlı chat programları geliştirmek isteyenlerin ilgisini çekecek en azından biraz bilgi verecek bir yazı olacaktır.

Bilindiği üzere WCF (Windows Communication Foundation) tarafında geliştirilen servislerin Duplex iletişimi kullanaraktan istemciler üzerinde operasyonlar gerçekleştirmesi, bir başka deyişle metod çağrılarında bulunabilmeleri mümkündür. Burada çift kanallı olarak gerçekleştirilen bir iletişim söz konusudur. Daha çok chat uygulamalarında veya istemcinin her hangibir durum değişikliğinde uyarılması gerektiği vakalarda bu tip servislerden yararlanılabilir. Söz gelimi bu yazımızda geliştireceğimiz WCF Servis örneği, istemcilerden aldığı şehir bilgisine göre anlık hava durumu bilgisini döndürecektir. Bu cümle ilk bakışta istemcinin yapacağı normal bir servis çağrısından ve sonucunun alınmasından farksız bir operasyonmuş gibi görünebilir. Ancak gözden kaçırılmaması gereken bir husus vardır; o da hava durumu bilgisinin bildirilme işleminin, servis tarafından istemci üzerindeki bir operasyon çağrısı ile yapılacağıdır.

Tabi yazımızın başlığından da anlayacağınız üzere söz konusu WCF Servisini bir Silverlight istemcisi üzerinden test etmeye çalışıyor olacağız. Duplex WCF Servisinin geliştirilmesi başlı başına karmaşık bir süreç gerektiğinden yazımızı iki seriye bölüyor olacağız. İlk bölümdeki hedefimiz Duplex iletişimi sağlayacak olan WCF Servisini geliştirmek olacak. İşe Visual Studio 2010 Ultimate RC ortamında WorldWeatherService isminde bir WCF Service Application uygulaması açarak ve hemen C:\Program Files (x86)\Microsoft SDKs\Silverlight\v4.0\Libraries\Server adresinde yer alan System.ServiceModel.PollingDuplex.dll assembly'ını referans ederek başlayabiliriz. Nitekim bu referans içerisindeki tiplere sunucu tarafında ihtiyacımız olacaktır.

![blg172_ServerReference.gif](/assets/images/2010/blg172_ServerReference.gif)

Şimdi servis için gerekli sözleşmeleri yazabiliriz. WCF Duplex servisleri iki sözleşme (Contract) içermektedir. Bunlardan birisi istemci tarafından çağırılacak olan operasyonları içeren sözleşmedir. Ancak diğer sözleşme geri bildirim (Callback) sırasında kullanılacak sözleşmedir. Aşağıda söz konusu sözleşmelere ait arayüz (Interface) içerikleri yer almaktadır.

```csharp
using System.ServiceModel;

namespace WorldWeatherService
{
    // Servis sözleşmesinin kullanacağı geri bildirim sözleşmesi CallbackContract özelliği ile bildirilir.
    [ServiceContract(CallbackContract=typeof(IWeatherDuplexClient))]
    public interface IWeatherDuplexService
    {
        [OperationContract]
        void SetCity(string cityName);
    }

    [ServiceContract]
    public interface IWeatherDuplexClient
    {
        // Servisin istemci tarafında tetikleyeceği Notice operasyonunun geriye bir şey döndürmeyeceği bir başka deyişle tek yönlü çalışan bir metod olduğu IsOneWay niteliği sayesinde bildirilir.
        [OperationContract(IsOneWay=true)]
        void Notice(WeatherStatus weather);
    }
}
```

Dikkat edileceği üzere IWeatherDuplexService sözleşmesi için kullanılan ServiceContract niteliğinde, geri bildirim sözleşmesi CallbackContract özelliği yardımıyla bildirilmiştir. Bu sayede servis sözleşmesini uygulayan tip, çalışma zamanında gerekli geri bildirim nesne örneğini değerlendirebilecektir. Diğer yandan servisin istemci üzerinde tetiklemede bulunacağı sözleşme, IWeatherDuplexClient adlı interface tipi olarak tanımlanmıştır. Bu sözleşme için önem arz eden konu ise Notice metodunun OperationContract niteliğinde yer alan IsOneWay özelliğinin true değere sahip olmasıdır. Nitekim servisin yaptığı istemci bazlı geri bildirimlerden bir sonuç beklenmemelidir. Bu da ilgili operasyonun tek yönlü çalışacak şekilde işaretlenmesi ile mümkün olacaktır. Gelelim servis sözleşmesinin uygulandığı tipe.

```csharp
using System;
using System.ServiceModel;
using System.Threading;

namespace WorldWeatherService
{
    public class WeatherDuplexService 
        : IWeatherDuplexService
    {
        #region Variables

        IWeatherDuplexClient client;
        string[] posibilities ={"Güneşli",
                                  "Yağmurlu",
                                  "Parçalı Bulutlu",
                                  "Kar Yağışlı",
                                  "Tipi",
                                  "Fırtına",
                                  "Bulutlu",
                                  "Rüzgarlı",
                                  "Zaman zaman yağışlı"
                              };

        #endregion

        #region IWeatherDuplexService Members

        public void SetCity(string cityName)
        {
            // İstemci kanalı yakalanıyor ki geri bildirim sırasında hangi kanal üzerinden gidileceği bilinsin.
            client = OperationContext.Current.GetCallbackChannel<IWeatherDuplexClient>();
            // Geri bildirimlerde devreye girecek metod bloğunu işaret edecek TimerCallback tipinden bir temsilci kullanılır.

            Random rnd = new Random();
            TimerCallback tCallback = o => {
                // Sembolik olarak bir hava durumu bilgisi oluşturulur                
                string summary=posibilities[rnd.Next(0,posibilities.Length-1)];
                int heat = rnd.Next(0, 30);

                // İstemci tarafındaki Notice metodu çağırılır ve söz konusu şehir için anlık hava durumu bilgisi gönderilir(Tabiki hayali olarak)
                client.Notice(new WeatherStatus { City = cityName, Heat = heat, Summary = summary });
            };
            // Olayı simule etmek için,
            // Belirli süre duraksatma yapılır ve sonunda tCallback isimli temsilcinin işaret ettiği metod bloğunun çalıştırılması sağlanır.
            using (Timer tmr = new Timer(tCallback, null, 500, 500))
            {
                Thread.Sleep(2000);
            }
        }

        #endregion
    }
}
```

WeatherDuplexService sınıfı IWeatherDuplexService arayüzünü (Interfaca) uygulamaktadır. Bu uyarlamaya göre SetCity metodunu ezmektedir. SetCity metodu içerisinde en can alıcı nokta ise, Callback Channel nesne referansının yakalanmasıdır. Dikkat edileceği üzere GetCallbackChannel metodu generic olarak IWeatherDuplexClient arayüzünü kullanmaktadır. Buna göre, çalışma zamanında istemcinin servise gönderdiği çağrıya göre yakalayacağı kanalın, IWeatherDuplexClient arayüzünü uygulamış bir tip olacağı ortadadır. E haliyle bu arayüzün içerisinde tanımlanmış bir de operasyonumuz bulunmaktadır. Notice isimli metod.

![Wink](/assets/images/2010/smiley-wink.gif)

Dolayısıyla yakalanan kanal üzerinden yapılabilecek olan bir Notice operasyon çağrısı mevcuttur ve bu çağrı servis tarafından istemci üzerinde gerçekleştirilecektir. Kod içerisinde sembolik olarak bir gecikme işlemi uygulanmış ve bunun sonucunda TimerCallback temsilci tipinin (Delegate) işaret ettiği bir metod gövdesinin de devreye girmesi sağlanmıştır. Bu metod bloğu içerisindeyse Notice metod çağrısı gerçekleştirilmekte ve aşağıdaki içeriğe sahip olan WeatherStatus tipinden bir nesne örneği üretilmektedir.

```csharp
namespace WorldWeatherService
{
    public class WeatherStatus
    {
        public int Heat { get; set; }
        public string Summary { get; set; }
        public string City { get; set; }
    }
}
```

Sırada servis tarafının çalışma zamanını ilgilendiren konfigurasyon ayarlarının yapılması yer almakta. Burada işler biraz karışıyor.

![Sealed](/assets/images/2010/smiley-sealed.gif)

Neyseki MSDN üzerinden konu ile ilişkili yardımcı dökümanların fazlasıyla yararı olduğunu ifade edebilirim. İşte sunucu uygulamamıza ait web.config içeriğimiz.

```xml
<?xml version="1.0"?>
<configuration>
    <system.serviceModel>
        <extensions>
            <bindingExtensions>
                <add name="duplexHttpBinding" type="System.ServiceModel.Configuration.PollingDuplexHttpBindingCollectionElement, System.ServiceModel.PollingDuplex, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"/>
            </bindingExtensions>
        </extensions>
        <bindings>
            <duplexHttpBinding>
                <binding name="duplexHttpBindingConfiguration" duplexMode="MultipleMessagesPerPoll" maxOutputDelay="00:00:05"/>
            </duplexHttpBinding>
        </bindings>
        <behaviors>
            <serviceBehaviors>
                <behavior name="">
                    <serviceMetadata httpGetEnabled="true"/>
                    <serviceDebug includeExceptionDetailInFaults="false"/>
                </behavior>
            </serviceBehaviors>
        </behaviors>
        <serviceHostingEnvironment multipleSiteBindingsEnabled="true"/>
        <services>
            <service name="WorldWeatherService.WeatherDuplexService">
                <endpoint address="" binding="duplexHttpBinding" bindingConfiguration="duplexHttpBindingConfiguration" contract="WorldWeatherService.IWeatherDuplexService"/>
                <endpoint address="mex" binding="mexHttpBinding" contract="IMetadataExchange"/>
            </service>
        </services>
    </system.serviceModel>
    <system.web>
        <compilation debug="true"/>
  </system.web>
</configuration>
```

Anneciğimmm!!! Sakın korkmayın. Ezberlemeye gerek yok. Ancak bir özet geçmemizde yarar olduğu kanısındayım. Öncelikli olarak Silverlight tarafı için gerekli bir takım işlemler yapıldığını söyleyebiliriz. Bunlardan ilki bir bindingExtension bildirimidir. Sanırım projeyi oluşturduktan sonra neden System.ServiceModel.PollingDuplex.dll assembly'ını referans ettiğimizi anlamışsınızdır. Söz konusu extension ile yeni bir bağlayıcı tip (Binding Type) tanımlayarak kullanıma sunuyoruz. duplexHttpBinding olarak isimlendirdiğimiz bağlayıcı tipin bir takım özellikleri de (duplexMode, maxOutputDelay) belirtilmiş durumdadır.

Peki ya bundan sonrası? Örneğimizi Asp.Net Development Server yerine IIS altında konuşlandıracak şekilde tesis edebiliriz. Aslında projeyi doğrudan IIS altına Publish ettikten sonra servisi bir tarayıcı uygulama ile açarak sorunsuz bir şekilde çağırılıp çağırılmadığını görmekte yarar olacağı kanısındayım. Publish seçenekleri aşağıdakine resimde görüldüğü gibi yapılabilir.

![blg172_PublishSettingsLast.gif](/assets/images/2010/blg172_PublishSettingsLast.gif)

Örneğimizi test ettiğimiz aşağıdaki gibi bir sonuç ile karşılaşırsak her şey yoldundadır diyebiliriz. Diyebiliriz çünkü asıl test Silverlight istemcisi tarafından servise erişmeye ve kullanmaya çalıştığımızda oluşacaktır.

![blg172_ServiceOnBrowser.gif](/assets/images/2010/blg172_ServiceOnBrowser.gif)

Servis tarafında dikkat edilmesi gereken noktalardan birisi de Client Access Policy konusudur. Servisimizi IIS altına Publish etsek bile herhangibir Silverlight istemcisinin kullanabilmesi için ClientAccessPolicy.xml içeriğinin Domain Root altında yer alması gerekmektedir. Söz konusu dosyasnın içeriğini etkileyen pek çok faktör vardır ve açıkçası bu kadar detaya girmemize şimdilik gerek yoktur. Ancak en geçerli kaynaklardan birisi olarak Time Heuer'in [blog girdisinden](http://timheuer.com/blog/archive/2008/04/06/silverlight-cross-domain-policy-file-snippet-intellisense.aspx) ve tabiki Microsoft'un [Network Security Access Restricions in Silverlight](http://msdn.microsoft.com/en-us/library/cc645032(VS.95).aspx) yazısından yararlanabilirsiniz. Örneğimiz için aşağıdaki gibi bir içerik yeterli olacaktır.

```xml
<?xml version="1.0" encoding ="utf-8"?>
<access-policy>
  <cross-domain-access>
    <policy>
      <allow-from>
        <domain uri="*" />
      </allow-from>
      <grant-to>
        <resource path="/WorldWeatherService" include-subpaths="true"/>
      </grant-to>
    </policy>
  </cross-domain-access>
</access-policy>
```

Bu içeriğe sahip olan ClientAccessPolicy.xml dosyasının ise WCF Servisimizi Publish ettiğimiz IIS sunucusundaki ilgili Domain'e ait root klasörde yer alması gerekmektedir. Aşağıdaki şekilde görüldüğü gibi.

![blg172_ClientAccessPolicy.gif](/assets/images/2010/blg172_ClientAccessPolicy.gif)

Artık farklı bir domainde yer alan herhangibir Silverlight istemcisi WorldWeatherService'ini kullanabilecektir. Artık geride istemci tarafının yazılması ve test edilmesinden başka bir şey kalmamıştır. Ancak biraz nefes alalım ve bunu bir sonraki yazımıza bırakalım. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[WorldWeatherService.rar (157,92 kb)](/assets/files/2010/WorldWeatherService.rar) [Örnek Visual Studio 2010 Ultimate RC sürümü üzerinde geliştirilmiş ve test edilmiştir.]
