---
title: "WCF - Internet Üzerinden Güvenliği Sağlamak - 2"
pubDate: 2007-07-05 12:17:00
categories:
  - WCF
tags:
  - WCF
  - C#
  - XML
  - .NET
  - ASP.NET
  - SQL Server
  - SOAP
  - HTTP
  - Authentication
  - Authorization
  - Visual Studio
---

# WCF - Internet Üzerinden Güvenliği Sağlamak - 2
Hatırlanacağı gibi bir önceki makalemizde iletişim seviyesinde (Transport Level) güvenliğin sağlanabilmesi için gerekli ayarların nasıl yapılabileceğini incelemiştik. Bu makalemizde kaldığımız yerden devam ederek servis tarafındaki doğrulama işlemleri için üyelik ve rol yönetim (Membership and Role Management) sistemini devreye alacak ve istemci tarafını yazarak test edeceğiz. İlk olarak önceki yazımızda açmış olduğumuz WCF Service uygulamasına dönelim. Her zaman olduğu gibi basit bir arayüzü, servis sözleşmesi (Service Contract) olacak şekilde tasarlayacağız ve bunun uyarlamasını yapacak bir sınıf geliştireceğiz. İşlemlerin kolay bir şekilde anlaşılabilmesi için servis tarafındaki tipler mümkün olduğu kadar basit düşünülmüştür.

![mk212_1.gif](images/mk212_1.gif)

IAritmetik arayüzü (interface) ve Aritmetik sınıfına (class) ait kodlar aşağıdaki gibidir.

```csharp
using System;
using System.ServiceModel;

[ServiceContract(Name="Cebirci",Namespace="http://www.bsenyurt.com/Cebirci")]
public interface IAritmetik
{
    [OperationContract]
    double Topla(double x, double y);
}

public class Aritmetik : IAritmetik
{
    #region IAritmetik Members

    public double Topla(double x, double y)
    {
        return x + y;
    }

    #endregion
}
```

Aritmetik sınıfı basit olarak istemcilere Topla isimli bir işlevsellik sunmaktadır. Standart olarak arayüze ServiceContract ve OperationContract nitelikleri (attributes) uygulanmıştır. WCF servisinin svc uzantılı dosyasının içeriği ise aşağıdaki gibi olmalıdır.

```text
<% @ServiceHost Language=C# Debug="true" Service="Aritmetik" CodeBehind="~/App_Code/Service.cs" %>
```

Burada önemli olan noktalardan bir tanesi web.config dosyasının içeriğidir. Windows Communication Foundation için gerekli ayarları içerek web.config dosyasında bu sefer, iletişim seviyesinde güvenlik ve ehliyet (Credential) kontrol kuralları içinde bazı eklemeler yapılmalıdır. Sonuç itibariyle web.config dosyasının başlangıçtaki içeriği aşağıdaki gibi tasarlanabilir.

```xml
<?xml version="1.0"?>

<configuration>
<system.serviceModel>
    <bindings>
        <wsHttpBinding>
            <binding name="CebirServiceBindingCfg">
                <security mode="TransportWithMessageCredential">
                    <transport clientCredentialType="None" />
                    <message clientCredentialType="UserName" />
                </security>
            </binding>
        </wsHttpBinding>
    </bindings>
    <services>
        <service behaviorConfiguration="CebirServiceBehavior" name="Aritmetik">
            <endpoint binding="wsHttpBinding" bindingConfiguration="CebirServiceBindingCfg" name="CebirServiceEndpoint" contract="IAritmetik" />
        </service>
    </services>
    <behaviors>
        <serviceBehaviors>
            <behavior name="CebirServiceBehavior">
                <serviceMetadata httpsGetEnabled="true" />
            </behavior>
        </serviceBehaviors>
    </behaviors>
</system.serviceModel>

<system.web>
    <compilation debug="true"/>
</system.web>

</configuration>
```

Servis tarafında wsHttpBinding bağlayıcı tipi (Binding Type) kullanılmaktadır. Bağlayıcı tipe ait konfigurasyon ayarlarında dikkat edileceği üzere security elementi içerisinde iletişim ve mesaj seviyesi için gerekli istemci ehliyet tipleri transport ve message alt elementleri yardımıyla tanımlanmaktadır.

security elementi içerisindeki mode niteliği TransportWithMessageCredential olarak ayarlanmıştır. Buna göre, daha öncedende bahsedilen mesaj bütünlüğü (Integrity), mesaj mahremiyeti (Privacy) ve müşterek doğrulama (Mutual Authentication) gibi ilkeler HTTPS tarafından sağlanır. Bu nedenlede servis tarafının HTTPS ile hizmet verecek şekilde tasarlanmış olması bir başka deyişle sertifikalandırılmış olması şarttır (Daha önceden bir sertifika hazırlamamızın nedenide budur).

Diğer taraftan istemcilerin doğrulanması SOAP güvenliğine uygun olacak şekilde yapılır. Bir başka deyişle istemcilerin kendilerini servis tarafına kullanıcı adı, şifre veya sertifika (Certificate) yoluyla tanıtması gerekir. Geliştirilecek olan örnekte kullanıcı adı ve şifre kullanımı ele alınacaktır. Özellikle message elementinde yer alan clientCredentialType niteliğinin değeri istemcinin doğrulanması için kullanılacak istemci yetki belgesinin (Credential) tipini belirtir.

NOT: Windows SDK dökümantasyonuna göre security mode değeri TransportWithMessageCredential olarak ayarlanmışsa, transport alt elementi görmezden gelinir.

Bu işlemlerin tamamlanması ile birlikte servis herhangibir tarayıcı penceresinden talep edilebilir. Ekran çıktısı aşağıdakine benzer olacaktır.

![mk212_2.gif](images/mk212_2.gif)

Bu çıktının elde edilmesi için adres alanına https://localhost/CebirServisi/CebirService.svc yazılması gerekmektedir. Dikkat edilecek olursa http yerine https kullanılmaktadır.

Servis tarafında yapılması gereken işlemlerden biriside kullanıcı hesaplarının saklanması için gerekli Asp.Net üyelik veritabanının oluşturulmasıdır. Bu amaçla Web Site Administraton Tool aracından yararlanılabilir. Öncelikli olarak Security kısmından doğrulama işlemlerinin internet üzerinde yapılacağının bildirilmesi gerekmektedir. Bu nedenle From the Internet seçeneği işaretlenir.

![mk212_3.gif](images/mk212_3.gif)

Ardından bir kaç örnek kullanıcı hesabı aşağıdaki ekran görüntüsünde yer alan Create User kontrolü ile oluşturulur.

![mk212_4.gif](images/mk212_4.gif)

Örneklerde kullanılmak üzere daha sonra farklı rollere atanacak olan buraks ve bulents isimli iki kullanıcı oluşturulmuştur. Bu kullanıcıların şifreleride 123456. olarak belirlenmiştir. İlerleyen bölümlerde örnek rol kullanımlarıda ele alınacağından Personel ve Yonetici isimli iki rol Create New Role kontrolü yardımıyla tanımlanır (Rollerin kullanılabilmesi için Roles kısmından Enable Roles linkine tıklanılmalıdır).

![mk212_5.gif](images/mk212_5.gif)

Bu işlemin ardından örnek olarak oluşturulan bulents ve buraks isimli kullanıcılar farklı rollere atanırlar. buraks isimli kullanıcı Personel rolüne, bulents isimli kullanıcı ise Yonetici rolüne atanır. Bunun için Web Site Administrator Tool içerisindeyken, rollerin eklendiği kontrolde yer alan Manage linki kullanılabilir. Örneğin buraks kullanıcısının Personel rolüne atanması sonrası ekran görüntüsü aşağıdaki gibi olacaktır.

![mk212_6.gif](images/mk212_6.gif)

Bu işlemler sonrasında web.config dosyasına roleManager ve authentication elementleri aşağıdaki gibi eklenecektir.

```xml
<system.web>
    <roleManager enabled="true" />
    <authentication mode="Forms" />
    <compilation debug="true"/>
</system.web>
```

Yerel (Local) veritabanı kullanıldığından aynen web uygulamalarında olduğu gibi ASPNETDB.mdf dosyası App_Data klasörü altına açılır. (Burada kriter her zamanki gibi root web.config dosyasıdır. Nitekim Asp.Net uygulamalarından bilindiği üzere istenirse ASPNETDB veritabanı SQL Server sunucusu üzerinde de tutulabilir. Varsayılan ayar local olarak kullanılmasını sağlamaktadır.)

![mk212_8.gif](images/mk212_8.gif)

Bu işlemlerde tamamlandıktan sonra yetkilendirme (authorization) ve rol (Role) işlemleri için servise ait davranış (behavior) tanımlamaları yapılması gerekmektedir. Bu seferki ayarlamaları Microsoft Service Configuration Editor yardımıyla gerçekleştirebiliriz. Öncelikli olarak CebirServiceBehavior davranışına serviceAuthorization elementi eklenir. Bu işlemin ardından ilk olarak PrincipalPermissonMode değeri UseAspNetRoles olarak belirlenir. Sonrasında ise RoleProviderName değeri AspNetSqlRoleProvider olarak belirlenir.

![mk212_9.gif](images/mk212_9.gif)

Böylece istemcilerin rol yönetiminin.Net Framework ile hazır olarak gelen AspNetSqlRoleProvider tipi yardımıyla gerçekleştirileceği belirtilmiş olur. AspNetSqlRoleProvider tipi machine.config dosyası içerisinde tanımlanmış olup rol yönetimini üstlenen hazır.Net sınıflarından birisidir. Söz konusu tip aynı zamanda Asp.Net Web Site Administrator aracının kullandığı varsayılan rol sağlayıcısıdır (default role provider). Temel görevi kullanıcılar ile roller arasındaki ilişkilerin kurulması ve kontrol edilmesidir. Aşağıdaki ekran görüntüsünde söz konusu sağlayıcının machine.config dosyasında bulunduğu yer gösterilmektedir.

![mk212_10.gif](images/mk212_10.gif)

serviceAuthorization davranışının belirlenmesinden sonra, serviceCredentials isimli bir başka davranışın daha servis tarafında eklenmiş olması gerekmektedir. Bu sefer söz konusu davranış ile istemcilere ait hesap bilgilerinin kim tarafından kontrol edileceği belirlenir. Burada UserNamePasswordValidationMode özelliğinin değeri MembershipProvider ve MembershipProviderName özelliğinin değeride AspNetSqlMembershipProvider olarak seçilir.

![mk212_11.gif](images/mk212_11.gif)

Burada belirtilen AspNetSqlMembershipProvider, machine.config dosyası içerisinde tanımlanmış olan kullanıcı doğrulama işlemlerini üstlenen varsayılan.Net tipidir. Aşağıdaki ekran görüntüsünde bu tipin machine.config dosyası içerisindeki yeri gösterilmektedir.

![mk212_12.gif](images/mk212_12.gif)

Tüm bu işlemlerin ardından web.config dosyasının içeriğinde yer alan serviceBehaviors kısmında aşağıdaki yenilemeler gerçekleşecektir.

```xml
<serviceBehaviors>
    <behavior name="CebirServiceBehavior">
        <serviceMetadata httpsGetEnabled="true" />
        <serviceAuthorization principalPermissionMode="UseAspNetRoles" roleProviderName="AspNetSqlRoleProvider" />
        <serviceCredentials>
            <userNameAuthentication userNamePasswordValidationMode="MembershipProvider" membershipProviderName="AspNetSqlMembershipProvider" />
        </serviceCredentials>
    </behavior>
</serviceBehaviors>
```

Artık istemci tarafı tasarlanmaya başlanabilir. İstemci program, basit bir Console uygulaması olarak tasarlanacaktır. Herşeyden önce istemci için gerekli proxy sınıfının üretilmesi gerekmektedir. Ne varki makaleyi hazırladığım sıralarda gerek svcutil.exe aracı gerek Visual Studio 2005' in Add Service Reference seçeneği https için hazırlanmış olan servise ait proxy sınıfının üretilmesinde hatalar fırlatılmasına neden olmuştur. Burada çözüm olarak servis https'e taşınmadan önce proxy sınıfının üretilmesi sağlanabilir. İkinci bir çözüm yolu ise servisin kullandığı sözleşme ve uyarlama sınıfını içeren ayrı bir sınıf kütüphanesi üzerinden svcutil ile proxy ürettirmektir. Yada üçüncü bir yol olarak aşağıda olduğu gibi proxy sınıfı manuel olarak yazılabilir:)

![mk212_13.gif](images/mk212_13.gif)

Gerekli kodlar aşağıdaki gibi olacaktır;

```csharp
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(Namespace = "http://www.bsenyurt.com/Cebirci")]
public interface CebirServisi
{
    [System.ServiceModel.OperationContractAttribute(Action = "http://www.bsenyurt.com/Cebirci/Cebirci/Topla", ReplyAction = "http://www.bsenyurt.com/Cebirci/Cebirci/ToplaResponse")]
    double Topla(double x, double y);
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
public interface CebirServisiChannel : CebirServisi, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
public partial class CebirServisiClient : System.ServiceModel.ClientBase<CebirServisi>, CebirServisi
{
    public CebirServisiClient()
    {
    }

    public CebirServisiClient(string endpointConfigurationName) : 
        base(endpointConfigurationName)
    {
    }

    public CebirServisiClient(string endpointConfigurationName, string remoteAddress) : 
        base(endpointConfigurationName, remoteAddress)
    {
    }

    public CebirServisiClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
        base(endpointConfigurationName, remoteAddress)
    {
    }

    public CebirServisiClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
        base(binding, remoteAddress)
    {
    }

    public double Topla(double x, double y)
    {
        return base.Channel.Topla(x, y);
    }
}
```

İstemci tarafındaki app.config isimli konfigurasyon dosyasının içeriği ise aşağıdaki gibi olmalıdır.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <bindings>
            <wsHttpBinding>
                <binding name="CebirServiceBindingCfg">
                    <security mode="TransportWithMessageCredential">
                        <transport clientCredentialType="None" />
                        <message clientCredentialType="UserName"/>
                    </security>
                </binding>
            </wsHttpBinding>
        </bindings>
        <client>
            <endpoint address="https://localhost/CebirServisi/CebirService.svc" binding="wsHttpBinding" bindingConfiguration="CebirServiceBindingCfg" contract="CebirServisi" name="CebirServiceEndpoint">
            </endpoint>
        </client>
    </system.serviceModel>
</configuration>
```

Dikkat edilecek olursa istemci tarafında yer alan bağlayıcı tip (Binding Type) ayarlarında da security elementi de yer almaktadır ve servis tarafındaki ile aynı değerlere sahip olacak şekilde tasarlanmıştır. İstemci tarafında gerçekleştirilen bu ayarlardan sonra Main metodunun içeriği aşağıdaki gibi düzenlenebilir.

```csharp
CebirServisiClient client = new CebirServisiClient("CebirServiceEndpoint");

client.ClientCredentials.UserName.UserName = "buraks";
client.ClientCredentials.UserName.Password = "123456.";

double sonuc=client.Topla(3, 4);
Console.WriteLine("Sonuc {0}",sonuc.ToString());
Console.ReadLine();
```

Dikkat edilecek olursa, istemci tarafına ait yetkinlik belgesini gönderirken kullanıcı adı ve şifre bilgileri verilmektedir. Bunun için proxy sınıfına ait nesne örneği üzerinden erişilen ClientCrendetials özelliği kullanılmaktadır. ClientCredentials özelliği ClientCredentials sınıfına ait nesne örneklerini işaret eder. Bu nesne örneği üzerinden UserName özelliği (property) ile UserNamePasswordClientCredential isimli sınıfa erişilebilir. UserNamePasswordClientCredential sınıfının UserName ve Password isimli iki özelliği bulunmaktadır ve bunlar servis tarafında tanımlı olan istemci hesaplarındakiler ile karşılaştırılmak üzere gönderilir.

Geliştirilen örnekte test amaçlı sertifika kullanılmasının oluşturduğu bazı negatif etkiler vardır. Bu nedenle yukarıdaki kod parçasını içeren istemci uygulama yürütüldüğünde çalışma zamanında (Runtime) SecurityNegotiationException tipinden bir istisna mesajı alınması kuvvetle muhtemeldir.

![mk212_14.gif](images/mk212_14.gif)

Az öncede belirtildiği gibi bu istisnanın sebebi gerçek sertifikaların kullanılmayışıdır. Yaptığım araştırmalarda bu tip test sertifikalarının kullanıldığı senaryolar için Microsoft geliştiricileri tarafından yazılmış bir sınıf olduğunu tespit ettim. Bu sınıf yardımıyla yukarıdaki istisna mesajını atlatmak ve yerel makinelerde testleri başarılı bir şekilde gerçekleştirmek mümkündür. Sınıfın içeriği Microsoft tarafından aşağıdaki gibi geliştirilmiştir.

PermissiveCertificatePolicy isimli sınıfın içeriği;

```csharp
using System.Security.Cryptography.X509Certificates;
using System.Net;

// Microsoft' tan alıntıdır.
class PermissiveCertificatePolicy
{
    string subjectName;
    static PermissiveCertificatePolicy currentPolicy;
    PermissiveCertificatePolicy(string subjectName)
    {
        this.subjectName = subjectName;
        ServicePointManager.ServerCertificateValidationCallback +=new System.Net.Security.RemoteCertificateValidationCallback(RemoteCertValidate);
    }

    public static void Enact(string subjectName)
    {
        currentPolicy = new PermissiveCertificatePolicy(subjectName);
    }

    bool RemoteCertValidate(object sender, X509Certificate cert, X509Chain chain, System.Net.Security.SslPolicyErrors error)
    {
        if (cert.Subject == subjectName)
        {
            return true;
        }
        return false;
    }
}
```

Bu sınıfın sadece test sertifikalarının olduğu senaryolarda ele alınmasının, gerçek sertifikaların kullanıldığı senaryolarda kullanılmamasının Microsoft tarafından önerildiğini de hatırlatalım. PermissiveCertificatePolicy sınıfı istemci tarafına eklendikten sonra Main metodunda proxy sınıfı örneklenmeden önce ele alınması gerekmektedir.

```csharp
PermissiveCertificatePolicy.Enact("CN=TestSertifika-HTTPS-Server");
CebirServisiClient client = new CebirServisiClient("CebirServiceEndpoint");
```

Burada dikkat edilirse Enact metoduna sunucu tarafında oluşturulan test sertifikasının adı parametre olarak verilmiştir. Şimdi uygulama bu haliyle test edilirse aşağıdaki ekran görüntüsünde olduğu gibi başarılı bir şekilde çalıştığı görülür.

![mk212_15.gif](images/mk212_15.gif)

Kullanıcı adı ve şifre bilgilerinde hata olması halinde ise uygulama MessageSecurityException tipinden bir istisna fırlatacaktır. Bir başka deyişle istemci servis tarafından doğrulanmamış olacaktır.

Gelelim rollerin ne şekilde ele alınabileceğine. Örneğin Topla isimli metodu sadece Personel rolünde olanların ele alabileceği bir senaryo göz önüne alalım. Buna göre servis tarafındaki Aritmetik sınıfı içerisinde yer alan Topla metodunda aşağıdaki düzenlemeleri yapmak yeterli olacaktır.

```csharp
public double Topla(double x, double y)
{
    IIdentity ulasanKullanici = ServiceSecurityContext.Current.PrimaryIdentity;
    if (Roles.IsUserInRole(ulasanKullanici.Name, "Personel"))
        return x + y;
    else
        throw new FaultException("Yetkiniz yok");
}
```

Buradaki kodlara göre çalışma zamanında metoda çağrıda bulunan istemciye ait kullanıcı bilgileri ServiceSecurityContext sınıfı üzerinden ele alınabilir. Elde edilen referans yetki belgesi gönderen kullanıcının ad bilgisinide içerecektir.

NOT: ServiceSecurityContext ve Roles sınıflarının kullanılabilmesi için System.Security.Principal ve System.Web.Security isim alanlarının uygulamaya dahil edilmesi gerekir.

Bundan sonra tek yapılan, Roles sınıfının IsUserInRole metodunu kullanarak, güncel içerikteki kullanıcının Personel rolünde olup olmadığının tespit edilmesidir. Eğer kullanıcı Personel rolünde ise hesaplama yaptırılır. Aksi halde bir FaultException üretilip istemci tarafına doğru fırlatılır. İstemci tarafında bulents isimli kullanıcı ile Topla metoduna erişmek istediğimizde çalışma zamanında FaultException üretildiğini görürüz.

![mk212_16.gif](images/mk212_16.gif)

Ancak istemci tarafından, Personel rolündeki bir kullanıcı ile servise ulaşırsak Topla metodu sorunsuz bir şekilde çalışacaktır. Bunu buraks kullanıcısı ile deneyebiliriz. Nitekim buraks isimli kullanıcı Personel rolü içerisinde tanımlanmıştırç

Böylece geldik bir makalemizin daha sonuna. Bu tamamlayıcı makalemizde internet üzerinden https ile erişilebilen bir servis üzerinde, Asp.Net rol ve üyelik yönetimini (Role and Membership Management) kullanarak doğrulama (authentication) ve yetkilendirme (authorization) işlemlerinin nasıl yapılabileceğini incelemeye çalıştık. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[SSLClient.zip (25,30 kb)](https://www.buraksenyurt.com/makale/images/SSLClient.zip) (Dosya boyutunun küçük olması için veritabanı çıkartılmıştır. Örneği denerken sertifika eklemeyi ve veritabanını oluşturup örnek kullanıcılar dahit etmeyi unutmayınız.)
