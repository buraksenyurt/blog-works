---
layout: post
title: "WCF Service' lerine Silverlight İstemcilerinden Channel Bazlı Erişim"
date: 2010-06-02 01:55:00 +0300
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
  - xaml
  - http
  - authentication
  - threading
  - serialization
  - generics
  - visual-studio
  - rc
---
Bir zamanlar (aslında çok uzun zaman olmadı ayrılalı) özel bir eğitim firmasında Yazılım Eğitmeni olarak görev yapmaktaydım. Freelance olarak başladığım ilk dönemlerde kurumda en çok dikkatimi çeken nokta, şirketin gece koruyuculuğunu yapan köpekleri olmuştu. Aslında herkes yandaki resimde görülen ki kadar etkili ve caydırıcı olmasını bekleyebilir ancak son derece sakin ve kendi halinde sevimli bir köpekti. Bilişim sektöründe sistem eğitimleri de veren bu şirketin, geceleri koruma görevi ile barındırdığı köpeğinin adı ise, şirketin yaptığı işle ilintili olarak Proxy olarak verlimişti. Bilenler bilir...Neredeyse gıkı bile (pardon havı bile) çıkmayan bu sevimli köpeğin yersiz serzenişte bulunmamasının da bir nedeni vardı elbette. Proxy...Gelen veriyi süzüp ona göre aksiyon veriyordu çünkü

![blg170_Giris.jpg](/assets/images/2010/blg170_Giris.jpg)

![Laughing](/assets/images/2010/smiley-laughing.gif)

Şaka bir yana tesadüfe bakın ki bu günkü konumuzda Proxy kavramı ile alakalı.

WCF Servislerinin herhangibir istemci uygulama tarafından kullanılmasını sağlamak için tercih ettiğimiz yollardan birisi de Proxy tiplerinden faydalanmaktır. Genellikle Add Service Reference veya Svcutil.exe ya da SlSvcUtil.exe (Silverlight versiyonu) gibi araçlar yardımıyla Proxy üretimi kolayca gerçekleştirilebilir. Proxy tipleri, servislere erişilmesi sırasında istemci tarafında yazılan kodu hafifletmekle kalmaz aynı zamanda çalışma zamanının ayağa kaldırılması gereken ya da iletişim sırasında oluşturulması gereken pek çok nesnenin iş yükünü de üzerine alır. Ancak bazı durumlarda istemci tarafında Proxy tipi kullanımı yerine, servis ile olan iletişimde gerekli olan kanal (Channel) yapısının manuel kod ile oluşturulması ve diğer hazırlıkların yapılarak iletişim kurulması istenebilir.

Bu istek özellikle servis tarafındaki özel serileştirilebilir tiplerin (Serializable Types) çeşitli yardımcı fonksiyonelliklere sahip olduğu durumlarda önem kazanmaktadır. Örneğin sunucu tarafındaki bu tiplerde doğrulama işlemleri (Validation) için yazılmış bazı özel fonksiyonlar yer alabilir ve bunların istemci tarafında üretilen tiplere de alınması istenebilir. Böylece bu fonksiyonelliklerin içerdiği bazı iş kurallarının istemci tarafında da yüklenilmesi istenebilir ki normal Proxy üretiminde bu metodların istemci tarafına taşınmadığı bilinmektedir. İşte bu teoriden yola çıkarak yazdığımız bu blog girdimizde, söz konusu durumun Silverlight uygulamalarında nasıl çözümlenebileceğini incelemeye çalışıyor olacağız. (Aslında çok eskiden.Net Remoting ile uğraşmış birisi olarak, dağıtık uygulama geliştirirken servis tarafında çalışan bileşenlerin, istemci tarafına da referans edildikleri bir yöntem olduğunu ifade edebilirim)

> Silverlight tarafında Proxy tabanlı olarak WCF servislerinin nasıl kullanılabileceğini [Screencast - Silverlight Enabled WCF Services](https://www.buraksenyurt.com/admin/post/Screencast-Silverlight-Enabled-WCF-Services.aspx) görsel dersinden izleyebilirsiniz.

Dilerseniz vakit kaybetmeden örneğimize başlayalım. Visual Studio 2010 RC sürümünde oluşturduğumuz Silverlight 4.0 uygulamamızın içerisinde aşağıdaki IAlbumProducer isimli arayüzün (Interface) olduğunu düşünelim.

```csharp
using System.Runtime.Serialization;
using System.ServiceModel;

namespace WithChannelBased.Web
{
    [ServiceContract]
    public interface IAlbumProducer
    {
        // Silverlight istemcilerin asenkron çağrı yapmalarını zorlamak için aşağıdaki ön işlemci direktifi(Pre Proccesor Directive) eklenmiştir.
#if SILVERLIGHT
        
        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetAlbum(int albumId, AsyncCallback callback, object state);

        Album EndGetAlbum(IAsyncResult result);
        
#else

        [OperationContract]
        Album GetAlbum(int albumId);

#endif
    }

    [DataContract]
    public class Album
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Genre { get; set; }
    }
}
```

Dikkat edileceği üzere Interface tipi aslında bir servis sözleşmesi (Service Contract) tanımlamaktadır. Bu sözleşmenin Siverlight istemcilerinde asenkron çağrıları zorunlu kılması içinse bir ön işlemci direktifi kullanılmıştır. Yanlız bu ön işlemci direktifinin büyük harfler ile yazılması önemlidir. Peki neden böyle bir gereksinimimiz olmuştur?

Öncelikli olarak sunucu tarafında BeginGetAlbum ve EndGetAlbum metodlarının, arayüz zorlaması nedeniyle servis sınıfı içerisinde uygulanması istenmemektedir. Diğer yandan aynı dosyayı istemci tarafına alıyor olacağız ki bu durumda Silverlight istemcisinin asnekron çağrılar için söz konusu BeginGetAlbum ve EndGetAlbum metodlarını da kullanabilmesi gerekmektedir. İşte bu sebepten bir ön işlemci direktifi kullanılması tercih edilmiştir.

Diğer yandan sözleşmenin bulunduğu dosya içerisinde Album isimli serileştirilebilir bir tipin de yer aldığı görülmektedir. Album sınıfı bir veri sözleşmesi (Data Contract) şeklinde tanımlanmıştır. Önemli olan noktalardan birisi servis sözleşmesi ve veri sözleşmesinin aynı fiziki dosya içerisinde tutulmuş olmalarıdır. Nitekim bu dosya Silverlight uygulamasına doğrudan link olarak bağlanacaktır. Servis sözleşmesini uygulayacak tipin aslında Silverlight destekli bir WCF Servisi olduğu düşünüldüğünde projeye Silverlight-enabled WCF Service şablonunda AlbumProducer isimli bir öğenin eklenmesi web.config dosyası içerisinde bazı ön hazırlıkların yapılmasını sağlayacaktır. Elbette eklenen bu tipin yukarıda tanımlı olan servis sözleşmesini uygulaması gerekmektedir. Aşağıdaki kod parçasında görüldüğü gibi;

```csharp
using System.Runtime.Serialization;
using System.ServiceModel;

namespace WithChannelBased.Web
{
    public class AlbumProducer
        :IAlbumProducer
    {
        #region IAlbumProducer Members

        public Album GetAlbum(int albumId)
        {
            return new Album { Id = 1000, Title = "Benim Şarkılarım", Genre="Türkçe Pop" };
        }

        #endregion
    }
}
```

GetAlbum metodunun ne yaptığı çok fazla önemli değildir. Sadece test amacıyla kullanacağımız bir içerik döndürmektedir. Bu işlemlerin ardından servisin bulunduğu sunucu tarafındaki web.config dosyasının içeriğinde bazı düzenlemeler yapılması gerekmektedir.

```xml
<?xml version="1.0"?>
<configuration>
    <system.web>
        <compilation debug="true" targetFramework="4.0" />
    </system.web>
    <system.serviceModel>
        <behaviors>
            <serviceBehaviors>
                <behavior name="">
                    <serviceMetadata httpGetEnabled="true" />
                    <serviceDebug includeExceptionDetailInFaults="false" />
                </behavior>
            </serviceBehaviors>
        </behaviors>
        <bindings>
            <customBinding>
                <binding name="WithChannelBased.Web.AlbumProducer.customBinding0">
                    <binaryMessageEncoding />
                    <httpTransport />
                </binding>
            </customBinding>
        </bindings>
        <!--<serviceHostingEnvironment aspNetCompatibilityEnabled="true" multipleSiteBindingsEnabled="true" />-->
        <services>
            <service name="WithChannelBased.Web.AlbumProducer">
                <endpoint address="" binding="customBinding" bindingConfiguration="WithChannelBased.Web.AlbumProducer.customBinding0"
                    contract="WithChannelBased.Web.IAlbumProducer" />
                <endpoint address="mex" binding="mexHttpBinding" contract="IMetadataExchange" />
            </service>
        </services>
    </system.serviceModel>
</configuration>
```

Aslında Siverlight-enabled WCF Service'i AlbumProcuder.svc adıyla eklediğimizden web.config dosyasında yukarıdaki XML içeriği otomatik olarak oluşturulmaktadır. Bir kaç küçük fark ile...İlk olarak Contract tipinin aslında IAlbumProcuder arayüzü olarak belirtilmesi gerekmektedir. Diğer yandan Asp.Net Compatibility Enabled opsiyonunun pasif olması gerekmektedir. Aksi durumda servisi, herhangibir tarayıcı uygulama üzerinden görüntülemek istediğimizde aşağıda yer alan hata ile karşılaşırız.

![blg170_Exception.gif](/assets/images/2010/blg170_Exception.gif)

Şu aşamdan sunucu tarafındaki hazırlıklar tamamlanmıştır. Buna göre AlbumProducer.svc sayfasının tarayıcı uygulamadan talep edilmesi halinde aşağıdaki görüntü ile karşılaşılması işlerin iyi gittiğinin habercisidir.

![blg170_Ok.gif](/assets/images/2010/blg170_Ok.gif)

Hemen bir hatırlatmada bulunalım. Servisi çalıştıran Asp.Net Development Server uygulamasının açtığı port numarası önemlidir. Nitekim istemci tarafında yazılacak olan kod içerisinde bu port numarası değerlendirilecektir

![Wink](/assets/images/2010/smiley-wink.gif)

İşin belkide kodlama açısından en sıkıcı noktası ise istemci tarafını geliştirmektir. Her şeyden önce istemci tarafının, sunucu tarafında yer alan IAlbumProducer.cs dosyasını referans etmesi gerekmektedir ki bunu ilgili dosyayı ilave ederken Add As Link seçeneğinin kullanılmasında yarar vardır. Böylece dosyanın tek bir noktada durması garanti edilmiş olur. Diğer yandan System.ServiceModel.dll (Servis çalışma zamanının tesisi için gerekli tipleri kullanabilmek için) ve System.Runtime.Serialization.dll (Veri sözleşme nitelikleri için) Assembly'larının Silverlight uygulamasının olduğu projeye referans edilmesi şarttır.

![blg170_ReferencesNew.gif](/assets/images/2010/blg170_ReferencesNew.gif)

MainPage.xaml içeriğimizde basit olarak bir Button bileşenine basıldığında TextBlock kontrolünün içeriğinin sunucundan gelen Album bilgisi ile doldurulması planlanmaktadır. Buna göre kod içeriğini aşağıdaki gibi oluşturmamız yeterli olacaktır.

```csharp
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using WithChannelBased.Web;

namespace WithChannelBased
{
    public partial class MainPage : UserControl
    {
        // User Interface için ayrı bir Thread' in değerlendirileceği nesne tanımlanır
        SynchronizationContext syncContext;

        public MainPage()
        {
            InitializeComponent();
        }

        private void GetAlbumButton_Click(object sender, RoutedEventArgs e)
        {
            // BindingElement listesi tanımlanır
            List<BindingElement> bindings = new List<BindingElement>();
            // Sunucu tarafındaki web.config dosyasından hatırlanacağı üzere BinaryMessageEncoding tipinden bir Binding tipi mevcuttur. Öncelikle bu bağlayıcı listesine eklenir.
            bindings.Add(new BinaryMessageEncodingBindingElement());
            // Yine sunucu tarafındaki Binding listesine bakıldığında HttpTransport tipinden bir bağlayıcının da olduğu görülmektedir. Dolayısıyla bu tipten bir nesne örneğide oluşturulur.
            bindings.Add(new HttpTransportBindingElement());

            // Aynen Web.config dosyasında olduğu gibi, yukarıda tanımlanan Binding nesne örnekleri bir CustomBinding nesne örneği içerisinde toplanır. Bu nedenle parametre olarak bindings isimli liste verilmiştir.
            CustomBinding cBinding = new CustomBinding(bindings);

            // WCF çalışma zamanının bir kanal oluşturması için gerekli fabrika tipi tanımlanır.
            // İlk parametre kanalın kullanacağı bağlayıcı listesidir. İkinci parametre ise EndPoint için gerekli adres bilgisini içermektedir. (Port numarasını saklayın demiştim :) )
            var channelFactory = new ChannelFactory<IAlbumProducer>(
                cBinding,
                new EndpointAddress("http://localhost:57845/AlbumProducer.svc")
                );

            syncContext = SynchronizationContext.Current;

            // Kanal oluşturulu ve dolayısla açılır
            IAlbumProducer cnl = channelFactory.CreateChannel();

            // Servis tarafındaki GetAlbum metodu asenkron olarak çağırılır.
            cnl.BeginGetAlbum(
                1102
                , iar => {
                    Album albm=((IAlbumProducer)iar.AsyncState).EndGetAlbum(iar);
                    syncContext.Post(
                        obj =>
                        {
                            AlbumInfoTextBlock.Text = String.Format("{0}\n{1}\n{2}", albm.Id, albm.Genre, albm.Title);
                        }
                        , albm);
            }
            , cnl
            );
        }
    }
}
```

Volaaa!!! Uygulamayı test ettiğimizde aşağıdaki çalışma zamanı çıktısını almamız beklenmektedir.

![blg170_Runtime.gif](/assets/images/2010/blg170_Runtime.gif)

Dikkat edileceği üzere istemci tarafında bir konfigurasyon dosyası içeriği hazırlanmamıştır. Bir başka deyişe WCF Çalışma Zamanı (WCF Runtime) için gerekli Endpoint, CustomBinding bildirimlerinin tamamı kod içerisinde gerçekleştirilmiştir. Özet olarak Silverlight istemcisinin Proxy tipine ihtiyaç duymadan çalışması sağlanabilmiştir. Böylece geldik bir görsel dersimizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[WithChannelBased_RTM.rar (72,10 kb)](/assets/files/2010/WithChannelBased_RTM.rar) [Örnek Visual Studio 2010 RC sürümü üzerinde geliştirilmiş ve RTM sürümü üzerinden de test edilmiştir. Son sürümle birlikte test etmenizde yarar bulunmaktadır.]
