---
title: "WCF - Windows Tabanlı Doğrulama ve Yetkilendirme"
pubDate: 2007-07-12 12:00:00
categories:
  - WCF
tags:
  - WCF
  - C#
  - XML
  - .NET
  - HTTP
  - Authentication
  - Authorization
  - Threading
---

# WCF - Windows Tabanlı Doğrulama ve Yetkilendirme
Değerli Okurlarım Merhabalar,

Servis yönelimli mimaride (Service Oriented Architecture), dağıtık uygulamaların bir kısmı intranet tabanlı olaraktan Windows işletim sistemleri üzerinde konuşlandırılmış olarak tasarlanmaktadır. Bu sebepten ağ üzerinde tüm kullanıcıların daha kolay bir şekilde yönetildiği var olan ve bilinen doğrulama ve yetkilendirme materyallerinden yararlanmak sık olarak başvurulan tekniklerdendir. Bir başka deyişle kullanıcı hesaplarının (Member accounts) yönetimi hazır olan Windows işletim sistemi unsurları tarafından kolayca ele alınabilmektedir. Buda doğal olarak istemcilerin servis tarafında doğrulanması (Authentication) ve yetkilendirilmesi (Authorization) için hazır olan kaynakların kullanılabilmesi anlamına gelir.

Sonuç itibariyle servis ve istemci tarafı açısıdan geliştiricinin yükü biraz daha hafiflemektedir. Windows Communication Foundation uygulamalarındada intranet tabanlı sistemler için Windows tabanlı doğrulama ve yetkilendirme (Windows Based Authentication and Authorization) işlemlerini, bağlayıcı tip (binding type) bazında kolayca gerçekleştirebiliriz. Bu bölümde özellikle basicHttpBinding tipi yardımıyla bu işlemlerin nasıl geliştirilebileceğini adım adım incelemeye çalışacağız. Her zamanki gibi servis tarafından hizmete sunulacak olan WCF Service Library'sini geliştirmekle işe başlanabilir. Bu kütüphane içerisindeki söz konusu tipler (types) aşağıdaki gibi tasarlanmıştır.

![mk213_1.gif](images/mk213_1.gif)

IAritmetik arayüzü (interface);

```csharp
using System;
using System.ServiceModel;

namespace CebirLib
{
    [ServiceContract(Name="AritmetikServisi",Namespace="http://www.bsenyurt.com/AritmetikServisi")]
    public interface IAritmetik
    {
        [OperationContract(Name="ToplamaOperasyonu")]
        int Topla(int x, int y); 
    }
}
```

Aritmetik sınıfı (class);

```csharp
using System;
using System.Threading;
using System.Security.Principal;

namespace CebirLib
{
    public class Aritmetik:IAritmetik
    {
        #region IAritmetik Members

        public int Topla(int x, int y)
        { 
            IPrincipal principal = Thread.CurrentPrincipal;
            string dogrulamaTipi=principal.Identity.AuthenticationType;
            string dogrulandi = principal.Identity.IsAuthenticated ? "Dogrulandi" : "Dogrulanmadi";
            string ad=principal.Identity.Name;

            Console.WriteLine("Kullanıcı : " + ad + "\n" + "Doğrulama Tipi : " + dogrulamaTipi + "\n" + dogrulandi + "\n");
            return x + y;
        }

        #endregion
    }
}
```

Aritmetik sınıfı içerisinde yer alan Topla isimli metoda başlangıç olarak bazı kod satırlar eklenmiştir. IPrincipal arayüzüne ait principal isimli değişkene, Thread sınıfının static CurrentPrincipal özelliği yardımıyla atanan değer aslında çalışma zamanında servise talepte bulunan istemci kimliğini işaret etmektedir. Bu özelliğin dönüş değerinden faydalanarak doğrulama tipini (Authentication Type), kullanıcının adını (Name) ve hatta sonradanda görüleceği gibi kullanıcının hangi rolde olduğunun tespiti yapılabilmektedir. Daha çok Windows kullanıcıların rollerine (Role) bakılarak kod içerisinden yetkilendirme (Authorization) yapılmak istendiği durumlarda kullanılmaktadır.

Sırada servis ve istemci tarafındaki uygulamaların tasarlanması var. Her iki tarafıda olayları daha kolay irdelemek açısından birer Console uygulaması olarak tasarlamakta fayda vardır. Servis tarafındaki Console uygulaması, geliştirilen WCF Servis kütüphanesini (CebirLib) ve System.ServiceModel.dll assembly'ını referans etmektedir. Servis tarafına ait kodlar başlangıç için aşağıdaki gibidir.

```csharp
using System;
using System.ServiceModel;
using CebirLib;

namespace ServerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(Aritmetik));
            host.Open();
            host.Closing += new EventHandler(host_Closing);
            host.Closed += new EventHandler(host_Closed);
            Console.WriteLine("Sunucu dinlemede...\n Kapatmak için bir tuşa basın...");
            Console.ReadLine();
            host.Close();
        }

        static void host_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("Servis kapatıldı...Yine bekleriz...");
        }

        static void host_Closing(object sender, EventArgs e)
        {
            Console.WriteLine("Servis kapatılıyor. Lütfen bekleyiniz...");
        }
    }
}
```

Servis tarafında yer alan konfigurasyon dosyasının başlangıç ayarları ise aşağıdaki gibidir.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel> 
        <bindings>
            <basicHttpBinding>
                <binding name="AritmetikBindingHttpCfg">
                    <security mode="TransportCredentialOnly">
                        <transport clientCredentialType="None" />
                    </security>
                </binding>
            </basicHttpBinding>
        </bindings>
        <services>
            <service name="CebirLib.Aritmetik">
                <endpoint address="http://localhost:1600/AritmetikServisi" binding="basicHttpBinding" bindingConfiguration="AritmetikBindingHttpCfg" name="AritmetikServiceHttpEndPoint" contract="CebirLib.IAritmetik" />
            </service>
        </services>
    </system.serviceModel>
</configuration>
```

Servis tarafında basicHttpBinding bağlayıcı tipi kullanılmaktadır. Üzerinde durulması gereken ilk noktalardan birisi güvenlik modunun (security elementinin mode niteliği yardımıyla) TransportCredentialOnly olarak belirlenmiş olması ve iletişim seviyesinde istemci yetki belgesi tipi olarak None (transport elementi içerisindeki clientCredentialType niteliği yardımıyla belirlenmiştir) değerinin kullanılmış olmasıdır. Buna göre istemcilerin kimlik bilgileri için herhangibir doğrulama yapılmaz. Bir başka deyişle herkes bu servisi kullanabilir.

İstemci tarafındaki Console uygulamasına ait konfigurasyon dosyasının başlangıç içeriği ise aşağıdaki gibi olmalıdır. (İstemci uygulama için gerekli proxy sınıfı svcutil.exe aracı yardımıyla CebirLib.dll'i üzerinden elde edilmiştir. Bu konu daha önceden işlendiğinden burada tekrarlanmamıştır.)

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <bindings>
            <basicHttpBinding>
                <binding name="CebirClientBindingHttpCfg">
                    <security mode="TransportCredentialOnly">
                        <transport clientCredentialType="None" />
                    </security>
                </binding>
            </basicHttpBinding>
        </bindings>
        <client>
            <endpoint address="http://localhost:1600/AritmetikServisi" binding="basicHttpBinding" bindingConfiguration="CebirClientBindingHttpCfg" contract="AritmetikServisi" name="CebirClientHttpEndPoint" />
        </client>
    </system.serviceModel>
</configuration>
```

İstemci tarafındaki console uygulamasına ait kodlar ise aşağıdaki gibidir.

```csharp
using System;
using System.ServiceModel;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            AritmetikServisiClient client = new AritmetikServisiClient("CebirClientHttpEndPoint");

            int sonuc = client.ToplamaOperasyonu(3, 4);
            Console.WriteLine(sonuc.ToString());
            Console.ReadLine();
        }
    }
}
```

Uygulama bu haliyle çalıştırıldığında aşağıdakine benzer bir ekran görüntüsü ile karşılaşılır.

![mk213_2.gif](images/mk213_2.gif)

Görüldüğü gibi kullanıcı adı, doğrulama tipi veya kullanıcı bilgisinin herhangibir mekanizma ile doğrulanıp doğrulanmadığı bilgisi bulunmamaktadır. Bunun sebebi güvenlik ayarlarında mode niteliğinin değerinin None olarak belirlenmiş olmasıdır. Durum Aritmetik sınıfı içerisindeki Topla metodu debug modda incelendiğinde açık bir şekilde izlenebilir. Aşağıdaki ekran görüntüsünde bu durum yer almaktadır.

![mk213_3.gif](images/mk213_3.gif)

Buradan şu sonuca varılabilir. None güvenlik modunda hiç bir şekilde istemciden yetki bilgileri gönderilmez. Her istemci isimsiz kullanıcı (anonymous user) gibi ele alınır.

Şimdi olayı biraz daha farklı bir hale getirelim. mode niteliğinin değerini hem istemci hemde servis tarafında Basic olarak değiştirelim. Ardından servis ve istemci uygulamaları tekrar çalıştıralım. Bu durumda aşağıdaki ekran görüntüsü ile karşılaşırız.

![mk213_4.gif](images/mk213_4.gif)

Basic modda iken istemcinin servis tarafına kendisini tanıtması gerekmektedir. Bu nedenle aynen hata mesajında olduğu gibi proxy sınıfı üzerinden ClientCredentials özelliği kullanılmalıdır. Söz konusu operasyonda örnek olarak iki test kullanıcısı oluşturulmuştur. Garfield ve Rolfield isimli bu kullanıcılar, daha sonradan iki farklı Windows Group altında birleştirilecek ve rol bazlı yetkilendirmelerin (Role Based Authorization) nasıl yapılacağı ele alınacaktır. Söz konusu kullanıcılara ait Username ve Password bilgileri aşağıdaki gibidir. Bu kullanıcılar tamamen hayalidir:)

Kullanıcı Adı
Şifre

Garfield
Garfi1234.?

Rolfield
Garfi1234.?

Bu işlemin ardından istemci tarafındaki kodlar aşağıdaki gibi düzenlenebilir.

```csharp
using System;
using System.ServiceModel;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            AritmetikServisiClient client = new AritmetikServisiClient("CebirClientHttpEndPoint");

            client.ClientCredentials.UserName.UserName = "Garfield";
            client.ClientCredentials.UserName.Password = "Garfi1234.?";

            int sonuc = client.ToplamaOperasyonu(3, 4);
            Console.WriteLine(sonuc.ToString());
            Console.ReadLine();
        }
    }
}
```

Dikkat edilecek olursa client nesne örneği üzerinden ClientCredentials özelliğine geçilmiş ve buradan UserName özelliği yardımıylada kullanıcı adı (UserName) ve şifre (Password) bilgileri belirtilmiştir. Servis ve istemci tarafı tekrar test edilirse aşağıdakine benzer bir ekran görüntüsü ile karşılaşılır.

![mk213_5.gif](images/mk213_5.gif)

Yine çalışma zamanında Topla metodu içerisinde debug işlemi gerçekleştirilirse, aşağıdaki ekran görüntüsünde yer aldığı gibi WindowsPrincipal tipinin örneklendiği ve kullanıcı bilgilerinin WindowsIdentity tipi üzerinden elde edildiği görülür. Bu tipik olarak kullanıcının servis uygulamasının çalıştığı Windows işletim sistemindeki kullanıcılardan arandığınında bir göstergesidir.

![mk213_7.gif](images/mk213_7.gif)

Eğer hatalı kullanıcı bilgisi veya yanlış şifre girilirse aşağıdakine benzer bir ekran görüntüsü ile karşılaşılır. (Örneğin kullanıcı adı Garfieldd olarak çift d ile bitecek şekilde girilirse.)

![mk213_6.gif](images/mk213_6.gif)

Dikkat edilecek olursa çalışma zamanında MessageSecurityException istisnası alınmış ve 403 Forbidden mesajı ile karşılaşılmıştır. Şimdi söz konusu kullanıcıların yetkilendirilmesinin nasıl ele alınabileceğini incelemeye çalışalım. Bu amaçla söz konusu Garfield ve Lorfield isimli kullanıcılar Yonetici ve Calisan isimli iki farklı Windows grubunda toplanmıştır.

Kullanıcı Adı
Windows Grubu

Garfield
Yonetici, Calisan

Rolfield
Calisan

Topla metodunu sadece Yonetici grubundaki kullanıcıların çalıştırması istenirse dekleratif (declarative) olarak PrincipalPermission niteliği ele alınmalıdır. PrincipalPermission niteliği (attribute), System.Security.Permissions isim alanı altında yer almaktadır. Bu sebeple ilgili isim alanının Aritmetik sınıfına eklenmesinde fayda vardır. PrincipalPermission niteliğinin temel kullanımı aşağıdaki gibidir.

```csharp
[PrincipalPermission(SecurityAction.Demand,Role="Yonetici")]
public int Topla(int x, int y)
{
```

PrincipalPermission niteliği metodlar gibi sınıflarada uygulanabilir. Ancak tavsiye edilen metod seviyesinde uygulanmasıdır. Bununla birlikte kendisinden türetilme yapılmasına izin vermeyen (sealed class) bir sınıftır. Bu sınıfın.Net içerisindeki içeriği aşağıdaki gibidir. (Sınıf içeriğinin elde edilmesi için Özcan Değirmenci tarafından geliştirilen Fox Decompiler aracı kullanılmıştır.)

```csharp
[ComVisible(true), AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple=true, Inherited=false), Serializable]
public sealed class PrincipalPermissionAttribute : CodeAccessSecurityAttribute
{
    // Constructors
    public PrincipalPermissionAttribute (SecurityAction action);

    // Methods
    public override IPermission CreatePermission ();

    // Properties
    public string Name { get; set; }
    public string Role { get; set; }
    public bool Authenticated { get; set; }

    // Instance Fields
    private string m_name;
    private string m_role;
    private bool m_authenticated;
}
```

Böylece Topla metodunu sadece Yonetici rolündeki kullanıcıların çağırabileceği belirtilmiş olur. Eğer istemci tarafından Lorfield isimli kullanıcı ile Topla metodu çağırılırsa aşağıdaki ekran görüntüsü ile karşılaşılır.

![mk213_8.gif](images/mk213_8.gif)

Görüldüğü gibi kullanıcı doğrulanmış (Authenticate) ancak Yonetici rolünde olmadığı için yetkisi geçersiz kılınmıştır (Unauthorized). Bu sebepten dolayı "Access is denied" hata mesajı ve SecurityAccessDeniedException tipinden bir çalışma zamanı istisnası (runtime exception) alınmıştır. Oysaki Garfield isimli kullanıcı ile erişilmek istendiğinde bir problem olmadan metod çağrısı gerçekleştirilebilir.

PrincipalPermission niteliği istenirse bir metod için birden fazla kez kullanılabilir. Sadece bu örnekte olduğu gibi rollere yetki vermek amacıyla değil belirli kullanıcıları yetkilendirmek yada birden fazla role izin vermek amacıyla kullanılabilir. Aşağıdaki örnekte bu durum analiz edilmektedir.

```csharp
[PrincipalPermission(SecurityAction.Demand,Role="Yonetici")]
[PrincipalPermission(SecurityAction.Demand,Name="BURAKSENYURT\\Burak Selim Senyurt")]
public int Topla(int x, int y)
{
```

Buradaki tanımlamalara göre BURAKSENYURT alanı içerisinde yer alan Burak Selim Senyurt isimli kullanıcıda Topla metodu için yetkilendirilmiş sayılmaktadır.

Elbette tek bir istemci yerine birden fazla istemci çalıştırıldığında Thread sınıfının CurrentPrincipal özelliği bağlanan kişiye ait bilgileri içeririr. Söz gelimi yukarıdaki örnek kodlara göre birden fazla istemci çalıştırıldığında aşağıdakine benzer bir ekran görüntüsü elde edilebilir.

![mk213_13.gif](images/mk213_13.gif)

Kullanıcılara ait yetki kontrolü istenirse kod içerisindende zorunlu bir şekilde (imperatively) gerçekleştirilebilir. Bunun için Aritmetik sınıfı içerisindeki Topla metodu aşağıdaki gibi düzenlemelidir.

```csharp
public int Topla(int x, int y)
{ 
    IPrincipal principal = Thread.CurrentPrincipal;
    string dogrulamaTipi=principal.Identity.AuthenticationType;
    string dogrulandi = principal.Identity.IsAuthenticated ? "Dogrulandi" : "Dogrulanmadi";
    string ad=principal.Identity.Name;

    #region Kod içerisinden yetki kontrolü

    WindowsPrincipal wp = (WindowsPrincipal)principal;
    if (wp.IsInRole("Yonetici"))
    {
        Console.WriteLine("Kullanıcı : " + ad + "\n" + "Doğrulama Tipi : " + dogrulamaTipi + "\n" + dogrulandi + "\n");
        return x + y;
    }
    else
        throw new FaultException("Geçersiz yetki");

    #endregion
}
```

İlk olarak WindowsPrincipal tipi yakalanır. Bunun için yine Thread sınıfından ve static üyelerinden CurrentPrincipal ile elde edilen referanstan faydalanılır. Elde edilen WindowsPrincipal nesne örneği üzerinden IsInRole metodu yardımıyla talepte bulunan istemci tarafından gelen kullanıcının Yonetici rolünde olup olmadığı kontrol edilebilir. Örnekte kullanıcının Yonetici rolü içerisinde olmaması halinde bir FaultException (FaultException kullanımı için System.ServiceModel isim alanının ilave edilmesi gerekir) istisnası fırlatılmaktadır.

Bu tarz bir kod parçasını kullanmak özellikle rol tabanlı üyelik kontrolü söz konusu olduğunda şart değildir. Nitekim IsInRole metodu zaten o anki Principal üzerinden kolaylıkla elde edilebilir. Bir başka deyişle aşağıdaki gibi bir kod parçasıda aynı işlemi görecektir.

```csharp
if (principal.IsInRole("Yonetici"))
{
```

Yine Garfield isimli kullanıcı ile deneme yapılırsa herhangibir sorun ile karşılaşılmadan Topla metodunun çağırılabildiği görülür. Ancak Lorfield isimli kullanıcı ile Topla metodu çağırılırsa aşağıdaki ekran görüntüleri ile karşılaşılır.

![mk213_9.gif](images/mk213_9.gif)

Basic güvenlik modu genellikle istemcilerin servis ile aynı güvenlik alanına (security domain) girmediği durumlarda ele alınır. Bu sebepten eğer kullanıcılar zaten güvenlik alanına varsayılan olarak dahil oluyorlarsa Windows Authentication Mode kullanılarak istemcilerin var olan ehliyet bilgilerini belirtmeden servise ulaşmaları sağlanabilir. Tek yapılması gereken istemci ve servis tarafındaki security elementlerinde mode niteliğini Windows olarak ayarlamaktır.

Servis tarafı;

![mk213_11.gif](images/mk213_11.gif)

İstemci tarafı;

![mk213_10.gif](images/mk213_10.gif)

Eğer servis tarafında Active Directory kullanılıyorsa bu durumda rol yönetimi için Windows Token Role Provider seçilmelidir. Bu ayarlama sadece servis tarafındaki konfigurasyon dosyasında aşağıdaki gibi yapılmalıdır.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <behaviors>
            <serviceBehaviors>
                <behavior name="AritmetikServiceBehavior">
                    <serviceAuthorization principalPermissionMode="UseWindowsGroups" />
                </behavior>
            </serviceBehaviors>
        </behaviors>
        <bindings>
            <basicHttpBinding>
                <binding name="AritmetikBindingHttpCfg">
                    <security mode="TransportCredentialOnly">
                        <transport clientCredentialType="None" />
                    </security>
                </binding>
            </basicHttpBinding>
        </bindings>
        <services>
            <service behaviorConfiguration="AritmetikServiceBehavior" name="CebirLib.Aritmetik">
                <endpoint address="http://localhost:1600/AritmetikServisi" binding="basicHttpBinding" bindingConfiguration="AritmetikBindingHttpCfg" name="AritmetikServiceHttpEndPoint" contract="CebirLib.IAritmetik" />
            </service>
        </services>
    </system.serviceModel>
</configuration>
```

Görüldüğü gibi serviceBehaviors elementi içerisine serviceAuthorization elementi eklenmiştir. Bu element içerisinde yer principalPermissionMode niteliğinin değeri ise UseWindowsGroups olarak belirlenmiştir. Her iki durumdada istemci tarafından servise kullanıcı bilgileri otomatik olarak gönderilecektir. Elbette domain'e dahil olmuşlarsa. Yanlız Windows modu kullanıldığında ve rol tabanlı bir yetkilendirme söz konusu olduğunda istenirse yine istemci tarafında belirli bir kullanıcı için bağlantı gerçekleştirilmesi sağlanabilir. Lakin böyle bir durumda istemci tarafındaki kodların aşağıdaki gibi ele alınması gerekir.

```csharp
AritmetikServisiClient client = new AritmetikServisiClient("CebirClientHttpEndPoint");
client.ClientCredentials.Windows.ClientCredential.UserName = "Garfield";
client.ClientCredentials.Windows.ClientCredential.Password = "Garfi1234.?";
client.ClientCredentials.Windows.ClientCredential.Domain = "BURAKSENYURT"; // Domain varsayılan ise yazılmak zorunda değildir.

int sonuc = client.ToplamaOperasyonu(3, 4);
Console.WriteLine(sonuc.ToString());
Console.ReadLine();
```

Bu durumda servis ve istemci çalıştırıldığında aşağıdaki ekran görüntüsü elde edilir.

![mk213_12.gif](images/mk213_12.gif)

Bu makalemizde basit olarak intranet tabanlı sistemlerde basicHttpBinding bağlayıcı tipini kullanarak Windows tabanlı doğrulama ve yetkilendirmelerin (Windows Based Authentication and Authorization) nasıl ele alınabileceğini incelemeye çalıştık. netTcpBinding veya wsHttpBinding bağlayıcı tipleri için iletişim seviyesinde güvenlik modunun varsayılan değeri Windows'dur. Dolayısıyla bu tipleri kullanırken config dosyası içerisinde ekstra bir işlem yapılmasına gerek yoktur. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](assets/WindowsDogrulama.zip)
