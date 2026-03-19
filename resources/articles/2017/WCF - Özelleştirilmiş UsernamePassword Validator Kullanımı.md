---
title: "WCF - Özelleştirilmiş UsernamePassword Validator Kullanımı"
pubDate: 2017-10-24 14:18:00
categories:
  - WCF
tags:
  - WCF
  - Bash
  - C#
  - XML
  - HTTP
  - Authentication
---

# WCF - Özelleştirilmiş UsernamePassword Validator Kullanımı
![custa_8.gif](images/custa_8.gif)

Merhaba Arkadaşlar,

Yeni ekibimdeki çalışmalar doğrultusunda bir süredir servis tabanlı mimarimizde WCF üzerine oturan hafif bir çatı oluşturmaya çalışmaktayız. Önemli ölçüde ilerleme kaydettik. Tabii WCF'in en temel fonksiyonelliklerini kullanırken ne kadar geniş bir alan olduğunun da farkına varıyoruz. Bizi zorlayan pek çok nokta var. Bunlardan birisi de güvenlik. Neredeyse sayısız kombinasyon seçeneği ile WCF tarafındaki güvenlik yetenekleri çok geniş (Bazen ne istediğimizi bile bilemez duruma geldiğimizi itiraf etmek isterim) Hal böyle olunca benim de eski bilgilerimi tazeleyip bazı şeyleri yeniden araştırmam ve öğrenmeye çalışmam gerekti. En çok takıldığım noktalardan birisi de geliştirme ortamında sertifikalar üretip bunları WCF tarafında kullanmak. Not aldığım konular üzerinden adım adım giderken belki benim yolumdan geçen veya geçecek olan arkadaşlar da vardır diye vakalardan birisini bloğumda kaleme almak istedim.

Senaryomuz şu; istemcilerin özel bir doğrulama mekanizması ile ele alınmasını sağlamak istiyoruz. Ancak bunu mesaj seviyesinde güvenli olan bir iletişim hattı üzerinde gerçekleştirmeliyiz. Binding konusunda serbestiz. Bir başka deyişle WsHttpBinding'i Message Based güvenlik modunda kullanıp özel bir UserNamePasswordValidator tipi ele alacağız. Mesaj tabanlı güvenlik söz konusu olduğu için sunucu ve istemcinin birbirlerine olan güvenini sertifikalarıyla sağlamamız gerekiyor. Biliyorum terimlerle kavramlar birbirine girdi ve kafalar karıştı. O zaman gelin adım adım ilerlemeye çalışalım.

Sertifikaların Oluşturulması

Aynı ortamda geliştirme yapmaktayız. Sunucu ve istemcinin birbirlerini doğrulaması noktasında iki adet sertifikaya ihtiyacımız olacak. Bu sertifikaları makecert aracını kullanarak üretebiliriz.

İlk olarak sunucu sertifikasını oluşturalım.

```bash
C:\C\Certificates>makecert -sr CurrentUser -ss My -a sha1 -n "CN=AzonServer" -sky exchange -pe
```

Benzer şekilde istemci sertifikasını...

```bash
C:\C\Certificates>makecert -sr CurrentUser -ss My -a sha1 -n "CN=AzonServer" -sky exchange -pe
```

Komutlarda kullanılan anahtarların belli anlamları var. Örneğin -sr ile kayıt lokasyonunu (Registry Location), -ss ile sertifika deposunu (Certificate Store), -a ile hangi kriptografi algoritmasını kullanacağımızı (MD5, SHA1 gibi), -n ile üreteceğimiz sertifikanın genel adını (Common Name), -sky ile anahtar tipini (Exchange, Signature gibi), -pe ile ilgili anahtarın ihraç edilip edilemeyeceğini (Exportable) belirtmekteyiz. Oluşturulan sertifikalar o anki kullanıcı için Personal - Certificates deposuna eklenecektir. Bunu Microsoft Management Console (MMC) aracı ile görebiliriz. Komut satırından MMC aracını açıp File - Add Remove/Snap-in ile Certificates sekmesini ekleyelim. Bu durumda AzonServer ve AzonClient sertifikalarının aşağıdaki ekran görüntüsünde olduğu gibi ilgili depoya dahil edildiklerini görebiliriz.

![custa_1.gif](images/custa_1.gif)

Sertifika üretimleri geliştireceğimiz örnekte ilerlememiz için yeterli değil. İlgili sertifikaları WCF çalışma zamanında hem sunucu hem de istemci tarafında kullanabilmek için Trusted People sekmesi altına da kopyalamamız gerekiyor. Dolayısıyla bir gerçek hayat senaryosunda bu sertifikaların birbirleriyle konuşacak olan uygulama sunucularında da yüklü olması lazım.

![custa_2.gif](images/custa_2.gif)

Sunucu Tarafının Geliştirilmesi

Sunucu tarafında basit bir servisimiz bulunacak. Bu servisi WsHttpBinding destekli olacak şekilde konuşlandıracağız. Kullanacağı güvenlik ayarlarını bu örnek özelinde programatik olarak düzenleyeceğiz. Tabii bir de özel kullanıcı doğrulama sınıfımızı ilave edeceğiz. Host uygulamasını Console tabanlı bir proje olarak geliştirebiliriz. System.ServiceModel ve System.IdentityModel kütüphanelerinin projeye referans edilmesi önemli. Önce servis sözleşmesini ve ilgili servis tipini yazalım.

Servis Sözleşmesi:

```csharp
using System.ServiceModel;

namespace AzonHostApp
{
    [ServiceContract]
    public interface IMathService
    {
        [OperationContract]
        double Sum(double x, double y);
    }
}
```

Servis tipi:

```csharp
namespace AzonHostApp
{
    public class MathService
        : IMathService
    {
        public double Sum(double x, double y)
        {
            return x + y;
        }
    }
}
```

Kobay sınıfımız toplama işlemi içeren bir servis sunmakta. Şimdi özel kullanıcı doğrulama (Custom Authentication) işlemini üstlenecek sınıfı yazalım.

```csharp
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;

namespace AzonHostApp
{
    public class AzonUsernamePasswordValidator
        :UserNamePasswordValidator
    {
        public override void Validate(string userName, string password)
        {
            if(userName!="barbarian"||password!="P@ssw0rd")
                throw new SecurityTokenException("");
        }
    }
}
```

AzonUsernamePasswordValidator sınıfı UserNamePasswordValidator tipinden türemekte. Üst tipten gelen Validate fonksiyonunun ezildiğine (override) dikkat edelim. Örneği oldukça basit bir şekilde ele almak istediğimizden tek yaptığımız belli bir kullanıcı ve şifresini kontrol etmekten ibaret. Önemli olan ise geçersiz oldukları takdirde bir SecurityTokenException fırlatıyor olmamız. Gerçek hayat senaryosunda buradaki kontrol operasyonunun bir Identity Server üzerinden gerçekleştirilmesi de düşünülebilir.

Host Uygulamanın Geliştirilmesi

Gelelim host tarafına. Burada standart olarak servis çalışma zamanını ayağa kaldıracak işlemler yapacağız. Normalde konfigurasyon bazlı olarak da ilerleyebiliriz. Ne var ki projelerimizde standart.config seçenekleri dışında kod yoluyla bir takım yetenekleri ortama dahil ediyoruz. Aslında tasarlanacak IoC yapısındaki konfigurasyon seçenekleri ile WCF çalışma ortamını genişletmeyi planladığımızı itiraf edebilirim. Lafı fazla uzatmadan Main metodundaki kodları aşağıdaki gibi düzenleyerek devam edelim.

```csharp
using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;

namespace AzonHostApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(MathService), new Uri[] { new Uri("http://localhost:6002") });
            host.Description.Behaviors.Add(new ServiceMetadataBehavior() { HttpGetEnabled = true, HttpsGetEnabled = true });
            host.Description.Behaviors.Find<ServiceDebugBehavior>().IncludeExceptionDetailInFaults = true;
            host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.PeerOrChainTrust;
            host.Credentials.ServiceCertificate.SetCertificate(StoreLocation.CurrentUser, StoreName.My, X509FindType.FindBySubjectName, "AzonServer");

            WSHttpBinding binding = new WSHttpBinding();
            binding.Security.Mode = SecurityMode.Message;
            binding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            host.AddServiceEndpoint(typeof(IMathService), binding, "soap12");
            host.Credentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
            host.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new AzonUsernamePasswordValidator();

            host.Open();
            if (host.State == CommunicationState.Opened)
            {
                Console.WriteLine("Host dinlemede. Kapatmak için bir tuşa basın");
                Console.ReadLine();

                host.Close();
                Console.WriteLine("Host kapatıldı");
                Console.ReadLine();
            }
        }
    }
}
```

Şimdi neler yaptığımıza bir bakalım. ServiceHost nesnesini üretirken hangi tipi kullanacağımızı ve adres bilgisini veriyoruz. Buna göre servisimiz http://localhost:6002 adresinden yayınlanacak. WSDL ve Exception detayı paylaşımı için çalışma zamanına varsayılan olarak eklenen ServiceMetadataBehavior ve ServiceDebugBehavior niteliklerini yakalayıp gerekli özelliklerini true olarak belirliyoruz. Sonrasında ise istemci ve sunucu arasındaki sertifika doğrulama işlemlerinin hangi modda yapılacağını belirtmekteyiz. Örnekte PeerOrChainTrust kullandık. Aslında farklı Trust modları bulunuyor (Detaylar için [şu adrese](https://msdn.microsoft.com/en-us/library/system.servicemodel.security.x509certificatevalidationmode(v=vs.110).aspx) bakabilirsiniz) Devam eden kodda Comman Name değerini AzonServer olarak verdiğimiz sertifikanın bildirimi gerçekleştiriliyor. Sertifikanın CurrentUser deposunda SubjectName'e göre aranacağı belirtilmekte.

Örnekte Ws standartlarını destekleyen bir binding tipi kullanılmakta. Bu tipin güvenlik modunu mesaj tabanlı olacak şekilde belirliyoruz. İstemcinin de kullanıcı adı ve şifre doğrulamasına tabi tutulacağını ClientCredentialType özelliğine atadığımız değerle işaret etmekteyiz. Bu ayarlamalardan sonra ilgili ServiceEndpoint tipinin eklenmesi söz konusu. Host tarafında kullanıcı doğrulama işlemi için AzonUsernamePasswordValidator isimli bir sınıf yazmıştık. Bu tipin kullanılacağını belirtmemiz lazım. Bu nedenle öncelikle UserNamePasswordValidationMode değerini Custom'a çekip CustomUserNamePasswordValidator özelliğine de kendi nesne örneğimizi ekliyoruz. Tabii burada işin sırrı bu atamanın gerçekleşmesi için AzonUsernamePasswordValidator tipinin System.IdentityModel.Selectors isim alanındaki UserNamePasswordValidator tipinden türemiş olması (İşte size bir çalışma zamanının basit genişletilebilirlik tasarımı)

Son olarak Open ve Close metodları kullanılarak gerekli açma ve kapatma işlemlerinin tatbik edildiğini belirtelim. Console uygulamasını bu haliyle çalıştırdığımızda aşağıdaki sonuçları görmemiz gerekiyor.

![custa_3.gif](images/custa_3.gif)

İstemci Tarafının Geliştirilmesi

Artık istemci tarafını yazmaya başlayabiliriz. Onu da basitlik olması açısından bir Console uygulaması olarak geliştirelim. Host uygulaması açıkken aşağıdaki ekran görüntüsünde olduğu gibi servis referansını istemci tarafına ekleyebiliriz.

![custa_4.gif](images/custa_4.gif)

İstemci tarafına ait kodları da aşağıdaki gibi yazabiliriz.

```csharp
using AzonClientApp.Azon;
using System;

namespace AzonClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test için bir tuşa basın");
            Console.ReadLine();

            MathServiceClient client = new MathServiceClient("WSHttpBinding_IMathService");
                      
            client.ClientCredentials.UserName.UserName = "barbarian";
            client.ClientCredentials.UserName.Password = "P@ssw0rd";
            double result = client.Sum(4.12, 3.41);

            Console.WriteLine(result);
        }
    }
}
```

Dikkat edilmesi gereken nokta MathServiceClient nesne örneği üzerinden ClientCredentials bilgisinin doldurulması. UserName üzerinden kullanıcı adı ve şifre bilgilerini belirttikten sonra Sum operasyonunu çağırmaktayız. Şimdi test sürüşüne çıkabiliriz. Önce sunucu sonra da istemci uygulamaları çalıştıralım. Ne yazık ki aşağıdakine benzer bir hata ile karşılaşma olasılığımız yüksek.

![custa_5.gif](images/custa_5.gif)

Sertifikanın doğrulanması sırasında bir hata oluştuğu ortada. Servis referansının eklenmesi sonrası oluşan web.config içeriğine biraz müdahalede bulunmamız gerekiyor. İçeriği aşağıdaki hale getirerek devam edelim.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <startup> 
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.6.1" />
    </startup>
    <system.serviceModel>
        <bindings>
            <wsHttpBinding>
                <binding name="WSHttpBinding_IMathService">
                    <security>
                        <message clientCredentialType="UserName" />
                    </security>
                </binding>
            </wsHttpBinding>
        </bindings>
      <behaviors>
        <endpointBehaviors>
          <behavior name="EndpointBehaviorForCertificate">
            <clientCredentials>
              <clientCertificate findValue="AzonClient" x509FindType="FindBySubjectName"
                storeLocation="CurrentUser" storeName="My" />
              <serviceCertificate>
                <authentication certificateValidationMode="PeerOrChainTrust"/>
              </serviceCertificate>
            </clientCredentials>
          </behavior>
        </endpointBehaviors>
      </behaviors>
        <client>
            <endpoint address="http://localhost:6002/soap12" binding="wsHttpBinding"
                bindingConfiguration="WSHttpBinding_IMathService" contract="Azon.IMathService"
                name="WSHttpBinding_IMathService" behaviorConfiguration="EndpointBehaviorForCertificate">
                <identity>
                  <dns value="AzonServer"/>
                </identity>
            </endpoint>
        </client>
    </system.serviceModel>
</configuration>
```

Aslında bir endpointBehavior ekledik. EndpointBehaviorForCertificate kısmında istemci tarafına ait sertifika bildirimini yapmaktayız. AzonClient isimli sertifikanın kullanılacağını ifade ediyoruz. Diğer yandan servis sertifikasının doğrulama modelini de PeerOrChainTrust olarak verdiğimize dikkat edelim. Gerçi sunucu tarafında da bu şekilde belirttiğimizden istemci tarafında sadece PeerTrust (Bir Chain Store olmadığından ChainStore değil) da kullanabiliriz. İkinci değişiklik ise endpoint'e ait identity elementinde yer alan dns değeri. Burada servise ait sertifikanın Common Name bilgisinin verildiği görülmekte. Uygulamaları tekrar çalıştırdığımızda aşağıdaki gibi başarılı bir çağırım gerçekleştirdiğimizi görebiliriz.

![custa_6.gif](images/custa_6.gif)

Elbette hatalı kullanıcı bilgisi ile ilerlenirse bir istisna alınacağı aşikardır.

![custa_7.gif](images/custa_7.gif)

Sonuç

Bu örnekte istemci ve servis arasında WS standartlarında mesaj tabanlı güvenlikle sağlanan bir iletişim gerçekleştirildiğini gördük. Ayrıca istemciyi kendi doğrulama modelimize dahil ettik. Kritik nokta bu örnekte yer alan sunucu ve istemcinin farklı makinelerde birer uygulama olması hali. Böyle bir vakada AzonServer ve AzonClient isimli sertifikaların her iki makinenin Trusted People kısmında yüklü olması gerekecektir. İstemci ve sunucuyu ayrı birer uygulama sunucusu olarak da düşünebiliriz. Örneği farklı güvenlik modları ile denemenizi öneririm. Örneğin Transport seviyesinde güvenlik moduna geçmeyi deneyebilirsiniz. Bu durumda https şemasını destekleyecek bir sunucuya da sahip olabilirsiniz. İlk başta da belirttiğim üzere WCF tarafındaki güvenlik bazlı senaryolar ve kullanılabilecek kombinasyon oldukça fazla. Benim bu örnekte yaptığım gibi denemelerden yararlanarak kendiniz keşfetmeye çalışırsanız öğrendiklerinizin daha kalıcı olacağını görebilirsiniz. Böylece geldik bir makalemizin daha sonuna. Tekrarda görüşünceye dek hepinize mutlu günler dilerim.
