---
title: "WCF–Custom AuthenticationService"
pubDate: 2014-11-30 23:00:00
categories:
  - WCF
tags:
  - WCF
  - XML
  - C#
  - .NET
  - ASP.NET
  - Oracle
  - NoSQL
  - HTTP
  - Authentication
  - Authorization
  - Threading
  - Generics
---

# WCF–Custom AuthenticationService
[![high_ropes_challenge](images/high_ropes_challenge_thumb.jpg)](images/high_ropes_challenge.jpg) Merhaba Arkadaşlar,

Bazen bir yola baş koyarız ama o kadar uzundur ki git git bitmek bilmez. Yolda bin bir türlü zorluğu aşmak zorunda kalırız. Hatta hangi zorluklarla karşılacağımızı da bilemeyebiliriz. Yolun uzunluğuna, karşılaşılan zorluklara bağlı olarak ya geri döneriz ya başka bir yola saparız ya da inat edip sonuna kadar gitmeye çalışırız. Mücadelinin sonunda yolun sonuna varmak da yeterli olmayabilir.

Bazen varacağımız noktaya ulaşırken edindiğimiz tecrübelerin son noktada doğru sonuçların üretilmesine neden olması gerektiğine inanırız. Bu yüzden tüm zorlukları aşıp yanlış sonuçları elde ettiğimizi görürsek harcadığımız tüm emeğin boşa gittiğini düşünebiliriz. Yine de edindiğimiz tecrübleri kar sayıp “hiç olmassa…” diyebilmeliyiz.

Şimdi diyeceksiniz ki neden bunları anlatıyorum. Anlatıyorum çünkü bu yazımız biraz uzun olacak ve yeri geldiğinde oldukça sıkacak, bunaltacak. Yine de sonuna geldiğinizde bir örneği tamamlayıp bazı sonuçlara varmış olacağız ve edindiğimiz tecrübeler yanımıza kar olarak kalacak. Konu oldukça karmaşık ve biraz da zor. Pek çok kaynağı incelemek, araştırmak zorunda kaldım yazarken. Ama sonuçta en azından kendi anladığım dilde birşeyler çıktı ortaya. Haydi başlayalım.

WCF (Windows Communication Foundation) ile servis yazan bir geliştiriciye, “en çok hangi konuların uygulanmasında zorlanıyorsun?” diye sorsak, sanıyorum ki ilk sıralardaki maddelerde şu anahtar kelimeler yer alıyor olacaktır; Security, Authentication, Authorization.

Her ne kadar WCF’ in konfigurasyon bazlı özellikleri ve getirdiği dekleratif yaklaşım bu işlemlerin mümkün mertebe kolay uygulanabilmesini öngörse de, çabuk unutulan konular olduklarından sık sık kitapları ve blogları tekrardan karıştırmak zorunda kalmaktayız.

Asıl Mevzu

Biz bugün kü yazımızda, WCF tarafında doğrulama ve yetkilendirme işlemlerine farklı bir bakış açısı getirmeye çalışıyor olacağız. WCF tarafında ASP.Net Membership Provider odaklı olarak kullanılabilen doğrulama ve yetkilendirme sistemi her ne kadar ideal bir çözüm olsa da, uygulanması için Membership API’ nin getirdiği bazı kısıtlara (SQL veritabanı bağımlılığı gibi) bağımlı bir çözümdür.

Bununla birlikte.Net Framework içerisinde yer alan ve Authentication (doğrulama) için kullanılabilen hazır Built-In bir servis sınıfı da bulunmaktadır; System.Web.ApplicationServices isim alanı altında yer alan AuthenticationService sınıfı. Bu servis aslında özelleştirilerek herhangibir Membership Provider ile çalışabilecek hale getirilebilir. İşte bu yazımızdaki temel gayemiz söz konusu doğrulama servisini özelleştirip kullanabilmektir.

Servis Uygulamasının Geliştirilmesi

Bu amaçla ilk olarak bir WCF Service Application projesi oluşturarak yola koyulalım. Projemiz içerisinde SpecialAuthenticationService isimli bir WCF Servis öğesi yer alıyor olacak. Lakin söz konusu öğeye ait Code Behind ve sözleşmeyi (Service Contract) içeren cs dosyalarını sileceğiz. Nitekim servis dosyamızın aslında System.Web.ApplicationServices isim alanındaki ApplicationService tipini kullanmasını istiyoruz.

[![sas_1](images/sas_1_thumb.png)](images/sas_1.png)

SpecialAuthenticationService.svc dosyasına ait Markup içeriğini ise aşağıdaki şekilde güncellememiz gerekmektedir.

```xml
<%@ ServiceHost Language="C#" Service="System.Web.ApplicationServices.AuthenticationService" Factory="System.Web.ApplicationServices.ApplicationServicesHostFactory" %>
```

Dikkat edilecek olursa Factory niteliğinde ApplicationServiceHostFactory tipi işaret edilerek, servisin üreticisi olan fabrika sınıfı da belirtilmiştir. Aslında burada yapılan, AuthenticationService olarak host edilebilecek ayrı bir servis üretilmesidir. Yapılan bu genişletme ile, standart olarak gelen servis fonksiyonelliklerinden bazılarının özelleştirilmesi mümkündür. Örneğin ValidateUser metodu gibi. Peki bunu nasıl yapabiliriz?

Authentication İşleminin Özelleştirilmesi

Öncelikli olarak WCF Servis uygulamasına ait global.asax içeriğini aşağıdaki gibi düzenleyelim.

```csharp
using System; 
using System.Net; 
using System.ServiceModel; 
using System.ServiceModel.Channels; 
using System.Web; 
using System.Web.ApplicationServices; 
using System.Web.Security;

namespace AzonServices 
{ 
    public class Global 
: System.Web.HttpApplication 
    {

        protected void Application_Start(object sender, EventArgs e) 
        { 
            AuthenticationService.Authenticating +=new EventHandler<AuthenticatingEventArgs>(Authenticating); 
        }

        private void Authenticating(object sender, AuthenticatingEventArgs e) 
        { 
            SpecialValidator validator = new SpecialValidator();

           e.Authenticated = validator.IsUserValid(e.UserName, e.Password); 
            e.AuthenticationIsComplete = true;

            if (e.Authenticated) 
            { 
                HttpCookie newCookie = new HttpCookie(FormsAuthentication.FormsCookieName); 
                newCookie.Value = e.UserName;

                HttpResponseMessageProperty response = new HttpResponseMessageProperty(); 
                response.Headers[HttpResponseHeader.SetCookie] = newCookie.Name + "=" + newCookie.Value; 
                OperationContext.Current.OutgoingMessageProperties[HttpResponseMessageProperty.Name] = response; 
            } 
        } 
    } 
}
```

Dikkat edileceği üzere ApplicationStart metodu içerisinde AuthenticationService tipinin Authenticating olay metodu yüklenmektedir. Bu olay metodu, object tipinden sender ve AuthenticationEventArgs tipinden e isimli parametreleri almaktadır. e parametresi üzerinden ulaşılan Authenticated özelliğine atanan değer, SpecialValidator sınıfı tipinden olan bir nesne örneğinin IsUserValid isimli metodunun çıktısıdır. SpecialValidator sınıfının içeriği aşağıdaki gibidir.

```csharp
namespace AzonServices 
{ 
    public class SpecialValidator 
    { 
        public bool IsUserValid(string userName, string password) 
        { 
            // Burada pek tabi istenen herhangibir Membership Provider kullanımı sağlanabilir. Örneğin bir Oracle veritabanına veya NoSQL dosyasına gidilebilir. Ya da bir SSO hizmetine başvurulabilir. Hayal gücü sizin. Üretin 
           return ((userName == "burak") && (password == "P@ssw0rd!")); 
        } 
    } 
}
```

İşte Membership Provider’ ın bağımsızlaştırıldığı yer aslında bu fonksiyonun içeriğidir. Bu metod bir üyenin doğrulama işlemini gerçekleştirecek şekilde kullanılmalıdır/tasarlanmalıdır. Dolayısıyla söz konusu metod içerisinden pek çok farklı noktaya çıkılabilir ve özel bir doğrulama işlemi uygulatılabilir.

> Yazının çok fazla karmaşıklaşmaması adına şimdilik bu kısım çok basit bir şekilde geçilmektedir. Ancak size tavsiyem söz konusu metod içeriğinde özel bir Membership API kullanmaya çalışmanızdır. Söz gelimi Windows Live veya Google Mail servislerine gitmeyi deneyebilirsiniz ya da bir NoSQL veritabanı sistemini burada devreye alabilirsiniz.

Authenticating metodu içerisinde yapılan önemli işlemlerden birisi de, doğrulama işleminin başarılı olması sonrasında bir Cookie’ nin üretilip çıktı mesajı içerisinde istemciye gönderilmesidir. Tabi bu noktada WCF’ in üreteceği mesaj içeriği yakalanmalıdır ki bunun için OperationContext.Current.OutgoingMessageProperties özelliğinin idenksleyicisi kullanılmaktadır.

Web.Config içerisinde Extension Bildiriminin Yapılması

Gelelim web.config dosyasının içeriğine.

```xml
<?xml version="1.0" encoding="utf-8" ?> 
<configuration> 
  <system.web.extensions> 
    <scripting> 
      <webServices> 
        <authenticationService enabled="true"/> 
      </webServices> 
    </scripting> 
  </system.web.extensions> 
    <system.serviceModel> 
        <behaviors> 
            <serviceBehaviors> 
                <behavior name=""> 
                    <serviceMetadata httpGetEnabled="true" httpsGetEnabled="true" /> 
                    <serviceDebug includeExceptionDetailInFaults="true" /> 
                </behavior> 
            </serviceBehaviors> 
        </behaviors> 
        <serviceHostingEnvironment aspNetCompatibilityEnabled="true" 
            multipleSiteBindingsEnabled="true" /> 
    </system.serviceModel> 
</configuration>
```

WCF servis ayarlarını zaten standart olarak bıraktığımız örnekteki tek özel nokta authenticationService kullanımının etkinleştirilmiş olmasıdır.

İlk Testler

Bu noktadan sonra servis uygulamasını çalıştırıp ilk testlerimizi yapabiliriz. WCF Service Application şablonunu kullandığımız için devreye girecek olan WCF Test Client uygulaması başarılı bir şekilde çalıştığı takdirde, AuthenticationService’ in test edilebildiği görülecektir. Dikkat edileceği üzere AuthenticationService sınıfının dışarıya sunduğu tüm operasyonlar listelenmiştir. ValidateUser, Login, IsLoggedIn ve Logout…

Örneğin yanlış bir kullanıcı adı ve şifre ile servisimize ait Login metodunu çağırdığımızı düşünelim.

[![sas_3](images/sas_3_thumb.png)](images/sas_3.png)

Dikkat edileceği üzere Login metodunun dönüşü false olmuştur. Ancak, doğru kullanıcı adı ve şifre ile bir deneme yaparsak operasyondan true değerinin döndüğünü, bir başka deyişle kullanıcının doğrulandığını görebiliriz.

[![sas_2](images/sas_2_thumb.png)](images/sas_2.png)

Authorization Kabiliyetlerinin Eklenmesi

Artık AuthenticationService hizmetini nasıl customize edebileceğimizi öğrendik. Öyleyse bu servisi yetkilendirme (Authorization) yeteneklerini de işin içerisine katarak örnek bir WCF Servisinde kullanmaya çalışalım. Bu amaçla aynı uygulamaya AlgebraService isimli bir WCF Service öğesi ekleyelim ve içeriğini aşağıdaki gibi düzenleyelim.

[![sas_4](images/sas_4_thumb.png)](images/sas_4.png)

Servis Sözleşmesi;

```csharp
using System.ServiceModel;

namespace AzonServices 
{ 
    [ServiceContract] 
    public interface IAlgebraService 
    { 
        [OperationContract] 
        double Sum(double x, double y); 
    } 
}
```

Servis sınıfı;

```csharp
using System.Security.Permissions; 
using System.Security.Principal; 
using System.ServiceModel; 
using System.ServiceModel.Activation; 
using System.ServiceModel.Channels; 
using System.Threading;

namespace AzonServices 
{ 
    [AspNetCompatibilityRequirements(RequirementsMode =AspNetCompatibilityRequirementsMode.Allowed)] 
    public class AlgebraService 
        : IAlgebraService 
    { 
        public AlgebraService() 
        { 
            var messageProperty = (HttpRequestMessageProperty)OperationContext 
                .Current 
                .IncomingMessageProperties[HttpRequestMessageProperty.Name];

            string cookie = messageProperty.Headers.Get("Set-Cookie"); 
            string[] nameValue = cookie.Split(','); 
            string userName = string.Empty; 
            for(int i=0;i<nameValue.Length;i++) 
            { 
                if(nameValue[i].Contains(".ASPXAUTH")) 
                { 
                    userName = nameValue[i].Split('=')[1]; 
                } 
            } 
           SpecialIdentity customIdentity = new SpecialIdentity 
            { 
                 Name=userName, 
                 IsAuthenticated=true 
            }; 
            GenericPrincipal threadCurrentPrincipal = new GenericPrincipal(customIdentity, new string[] { }); 
            Thread.CurrentPrincipal = threadCurrentPrincipal; 
        }

        [PrincipalPermission(SecurityAction.Demand, Name = "burak")] 
        public double Sum(double x, double y) 
        { 
            return x + y; 
        } 
    } 
}
```

SpecialIdentity sınıfı;

```csharp
using System.Security.Principal;

namespace AzonServices 
{ 
    public class SpecialIdentity 
        :IIdentity 
    { 
        public string Name { get; set; } 
        public bool IsAuthenticated { get; set; } 
        public string AuthenticationType { get; set; } 
    } 
}
```

AlgebraService içerisinde çok basit bir dummy fonksiyon bulunmaktadır. Toplama işlemini icra eden Sum metodunun uygulandığı sınıf içerisindeki işlemler ise oldukça önemlidir. Bunları aşağıdaki maddeler ile özetleyebiliriz.

- Servis, Built-In Asp.Net Authentication alt yapısına ihtiyaç duyduğundan Asp.Net Compatibility Mode uyumlu olarak çalışmalıdır. Bu yüzden sınıf başında bir nitelik (AspNetCompatibilityRequirements) bildirimi ile söz konusu uyumluluk modunun bir gereksinim olduğu ifade edilmiştir.
- Servis içerisindeki Sum metodu senaryo gereği sadece burak isimli kullanıcı tarafından çalıştırılabilmektedir. Bu yetkilendirme bildirimi, dekleratif olarak PrincipalPermission niteliği ile sağlanmaktadır. (Normal şartlarda kullanıcı ad kontrolü yerine Role bazlı bir Permission kontrolüne gidilmesinde yarar vardır. Nitekim fonksiyonellikler ağırlık olarak Role bazlı olacak şekilde yetkilendirilmektedir)
- Sınıfa ait yapıcı metod (Constructor) içerisinde bir dizi işlem yapıldığı görülmektedir. Buna göre, AuthenticationService’ inin istemciye gönderdiği Cookie yakalanmakta ve söz konusu servisin izleyen örneklerinin içerisinde yer alacağı Thread için ortak bir kimlik oluşturulmasında kullanılmaktadır. Bunun için güncel Thread’ in CurrentPrincipal özelliğine generic bir değişken atanmış ve içerisinde IIdentity arayüzü (Interface) türevli bir sınıf örneğine yer verilmiştir.

Örnek İstemci Uygulamanın Geliştirilmesi

Artık örnek bir istemci uygulama geliştirerek özelleştirilen AuthenticationService hizmetinin çalışmasını denetleyebilir ve özellikle Sum fonksiyonu için konulan yetkilendirme sürecini kontrol edebiliriz.

Console Application olarak geliştireceğimiz istemci uygulamanın her iki servisi de referans ediyor olması gerekmektedir. Bu nedenle Add Service Reference seçeneği ile hem SpecialAuthencationService’ in hem de AlgebraService’ in ilgili referanslarının Console uygulamasına dahil edilmesi gerekmektedir. Referans ekleme işlemleri sonrasında istemci tarafındaki App.config içeriği de aşağıdakine benzer bir şekilde üretilmiş olacaktır.

```xml
<?xml version="1.0" encoding="utf-8" ?> 
<configuration> 
    <startup> 
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5" /> 
    </startup> 
    <system.serviceModel> 
        <bindings> 
            <basicHttpBinding> 
                <binding name="BasicHttpBinding_IAlgebraService" /> 
                <binding name="BasicHttpBinding_AuthenticationService" /> 
            </basicHttpBinding> 
        </bindings> 
        <client> 
            <endpoint address="http://localhost:56478/AlgebraService.svc" 
                binding="basicHttpBinding" bindingConfiguration="BasicHttpBinding_IAlgebraService" 
                contract="MathSpace.IAlgebraService" name="BasicHttpBinding_IAlgebraService" /> 
            <endpoint address="http://localhost:56478/SpecialAuthenticationService.svc" 
                binding="basicHttpBinding" bindingConfiguration="BasicHttpBinding_AuthenticationService" 
                contract="MembershipSpace.AuthenticationService" name="BasicHttpBinding_AuthenticationService" /> 
        </client> 
    </system.serviceModel> 
</configuration>
```

Çok doğal olarak SpecialAuthenticationService yardımıyla, bir kullanıcı oturumu açılacak ve arından AlgebraService üzerinden Sum metodu çağırılacaktır. Önemli olan noktalardan birisi, oturum açıldıktan sonra istemci tarafına gönderilen Cookie’ nin varlığıdır. Bu Cookie istemci tarafında ele alınmalı ve sonraki operasyonel işlemde AlgebreService için de kullanılabilmelidir.

```csharp
using ClientApp.MathSpace; 
using ClientApp.MembershipSpace; 
using System; 
using System.Net; 
using System.ServiceModel; 
using System.ServiceModel.Channels;

namespace ClientApp 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            string cookie=string.Empty; 
            // Önce login olmayı deniyoruz 
            if (AuthenticateMember(ref cookie, "burak", "P@ssw0rd!")) 
            { 
                Console.WriteLine("{0}\n",cookie); 
                try 
                { 
                    // Aynı member cookie için arka arkaya 3 test yapmaktayız. 
                    CallSum(cookie, 4, 5); 
                    CallSum(cookie, 6, 7); 
                    CallSum(cookie, 1, -9); 
                } 
                catch (Exception exception) 
                { 
                    Console.WriteLine(exception.Message);                    
                } 
            } 
            else 
            { 
                Console.WriteLine("Doğrulama işlemi başarısız olduğundan uygulama sonlanacaktır"); 
                return; 
            } 
        }

        // Servis metod çağrısı 
        private static void CallSum(string cookie,double x,double y) 
        { 
            // AlgebraService' e ait bir örnek oluşturulur 
            AlgebraServiceClient einstein = new AlgebraServiceClient("BasicHttpBinding_IAlgebraService");

            // Güncel kanal bilgisi üzerinden 
            using (new OperationContextScope(einstein.InnerChannel)) 
            { 
                // Request için gidecek mesaj özelliğinin içerisine az önce doğrulama servisinin gönderdiği member cookie gömülür. 
                HttpRequestMessageProperty request = new HttpRequestMessageProperty(); 
                request.Headers[HttpResponseHeader.SetCookie] = cookie; 
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = request;

               Console.WriteLine("{0}+{1}={2}", x, y, einstein.Sum(x, y).ToString()); 
            } 
        }

        // Login işlemini üstlenen fonksiyon 
        private static bool AuthenticateMember(ref string cookie,string username,string password) 
        { 
            // AuthenticationService proxy örneği üretilir 
            AuthenticationServiceClient authenticator = new AuthenticationServiceClient("BasicHttpBinding_AuthenticationService"); 
            bool result = false;

            // Güncel kanal bilgisi üzerinden 
            using (new OperationContextScope(authenticator.InnerChannel)) 
            { 
                // ValidateUser ile kullanıcı doğrulanmaya çalışılır 
                result=authenticator.ValidateUser(username,password, string.Empty); 
                // Dönen mesajın içerisinden gelen Cookie bilgisi yakalanır 
                var responseMessageProperty = (HttpResponseMessageProperty) 
                             OperationContext 
                             .Current 
                             .IncomingMessageProperties[HttpResponseMessageProperty.Name]; 
                if (result) 
                { 
                    cookie = responseMessageProperty.Headers.Get("Set-Cookie"); 
                } 
            } 
            return result; 
        } 
    } 
}
```

İstemci Tarafı Testleri

Örneğimizi test edersek aşağıdaki sonuçları alıyor olmamız gerekir.

[![sas_5](images/sas_5_thumb.png)](images/sas_5.png)

Eğer hatalı bir kullanıcı bilgisi girilirse (örneğin kullanıcı adı burak yerine burk olsun) pek tabi doğrulama işlemi false değer döneceğinden herhangibir servis çağrısı yapılamayacaktır.

[![sas_6](images/sas_6_thumb.png)](images/sas_6.png)

Eğer kullanıcı doğrulanabilir olmasına karşın yetki verilen kullanıcılar arasında yer almıyorsa, bu durumda istemci tarafına yetkilendirme ile ilişkili bir istisna (Exception) mesajı dönecektir. Söz gelimi servis tarafındaki Sum metodunun niteliğini aşağıdaki gibi değiştirdiğimizi düşünelim.

```csharp
[PrincipalPermission(SecurityAction.Demand, Name = "kim")] 
public double Sum(double x, double y) 
{ 
    return x + y; 
}
```

Bu durumda istemci tarafında aşağıdaki çıktı elde edilecektir.

[![sas_7](images/sas_7_thumb.png)](images/sas_7.png)

Access is denied almak istediğimiz bir hatadır.

Role Bazlı Çalışma Yeteneklerinin Eklenmesi

Şimdi senaryomuzu biraz daha genişletelim ve Role bazlı çalışacak hale getirmeye çalışalım. Nitekim gerçek hayat senaryolarında, servis operasyonlarını kullanıcı adları ile yetkilendirmek her zaman uygun olmayabilir. Bazen bir servis operasyonu çağrısı, Membership API içerisindeki bir grubun yetkisinde olabilir. Hatta birden fazla Role dahi bağlanabilir. Ya da bir servis operasyonu belirli bir Role tarafından asla çalıştıramamalıdır. Bu gibi sebeplerden ötürür Role bazlı yetkilendirme (Role based Authorization) oldukça önemlidir. Peki nasıl uygulayabiliriz? İlk olarak SpecialValidator sınıfı içerisindeki IsUserValid metodunu geliştireceğiz.

```csharp
using System.Collections.Generic; 
namespace AzonServices 
{ 
    public class SpecialValidator 
    { 
        public bool IsUserValid(string userName, string password,out List<string> roles) 
        { 
            bool result = false; 
            roles = new List<string>();

            // Burada pek tabi istenen herhangibir Membership Provider kullanımı sağlanabilir. Örneğin bir Oracle veritabanına veya NoSQL dosyasına gidilebilir. Ya da bir SSO hizmetine başvurulabilir. Hayal gücü sizin. Üretin 
            if(userName == "burak" && password == "P@ssw0rd!") 
            { 
                result=true; 
                roles.Add("Contributor"); 
                roles.Add("Administrator"); 
            }

            return result; 
        } 
    } 
}
```

Aslında yaptığımız tek şey, doğrulanan kullanıcının var olan rollerini çekip, string tipinden Generic bir List koleksiyonuna doldurmaktır (Tabiki gerçek hayat senaryolarında bu tip bir rol çekimi hard coded yapılmamalıdır) Diğer yandan bu rollerin bir şekilde Cookie içerisine yazılması da gerekmektedir. Bunun içinde Global.asax içerisinde bir değişiklik yapılması şarttır. Aşağıdaki gibi…

```csharp
private void Authenticating(object sender, AuthenticatingEventArgs e) 
{ 
    SpecialValidator validator = new SpecialValidator(); 
    List<string> roles = null;

    e.Authenticated = validator.IsUserValid(e.UserName, e.Password,out roles); 
    e.AuthenticationIsComplete = true;

    if (e.Authenticated) 
    { 
        HttpCookie newCookie = new HttpCookie(FormsAuthentication.FormsCookieName); 
        newCookie.Value = e.UserName+"|"+CreateRolesString(roles);

        HttpResponseMessageProperty response = new HttpResponseMessageProperty(); 
        response.Headers[HttpResponseHeader.SetCookie] = newCookie.Name + "=" + newCookie.Value; 
        OperationContext.Current.OutgoingMessageProperties[HttpResponseMessageProperty.Name] = response; 
    } 
}

public string CreateRolesString(List<string> roles) 
{ 
    string result = string.Empty;

    foreach (string role in roles) 
    { 
        result += role + "|"; 
    }

    return result.TrimEnd('|'); 
}
```

Yaptığımız tek şey üretilen Cookie içerisine username bilgisi dışında, role bilgilerini de dahil etmektir. Tabi bir üye birden fazla role dahil olabileceği için, söz konusu rolleri daha sonradan ayrıştırılabilecek şekilde birleştirmek gerekmektedir. Ben örneğimizde bunun için pipe işaretinden yararlandım. Peki ayrıştırma nerede yapılıyor olacak? Tabi ki AlgebraService’ in yapıcı metodunda (Constructor) bu işlemi gerçekleştirmemiz gerekiyor.

```csharp
using System.Security.Permissions; 
using System.Security.Principal; 
using System.ServiceModel; 
using System.ServiceModel.Activation; 
using System.ServiceModel.Channels; 
using System.Threading;

namespace AzonServices 
{ 
    [AspNetCompatibilityRequirements(RequirementsMode =AspNetCompatibilityRequirementsMode.Allowed)] 
    public class AlgebraService 
        : IAlgebraService 
    { 
        public AlgebraService() 
        { 
            var messageProperty = (HttpRequestMessageProperty)OperationContext 
                .Current 
                .IncomingMessageProperties[HttpRequestMessageProperty.Name];

            string cookie = messageProperty.Headers.Get("Set-Cookie"); 
            string[] roles=null; 
            string[] nameValue = cookie.Split(','); 
            string userName = string.Empty; 
            for(int i=0;i<nameValue.Length;i++) 
            { 
                if(nameValue[i].Contains(".ASPXAUTH")) 
                { 
                    string[] content = nameValue[i].Split('=')[1].Split('|'); 
                    userName=content[0]; 
                    roles=new string[content.Length-1]; 
                    for (int j = 1; j < content.Length; j++) 
                   { 
                        roles[j-1]= content[j]; 
                    } 
                } 
            } 
            SpecialIdentity customIdentity = new SpecialIdentity 
            { 
                 Name=userName, 
                 IsAuthenticated=true 
            }; 
            GenericPrincipal threadCurrentPrincipal = new GenericPrincipal(customIdentity,roles); 
            Thread.CurrentPrincipal = threadCurrentPrincipal; 
        }

        [PrincipalPermission(SecurityAction.Demand,Role="Contributor")] 
        public double Sum(double x, double y) 
        { 
            return x + y; 
        } 
    } 
}
```

Dikkat edileceği üzere yakalanan Cookie içerisindeki Username’ i takip eden metin katarı, pipe işaretine göre ayrıştırılmış ve elde edilen role bilgileri bir string dizisi içerisine gömüldükten sonra, GenericPrincipal sınıfının yapıcı metoduna ikinci parametre olarak gönderilmiştir. Dolayısıyla o anda üretilen yetkilendirme bilgisi içerisinde roller de yer almaktadır. Bu sınıf içerisinde yapılan son işlem ise Sum metoduna uygulanan yetkilendirme bildirimidir.

Görüldüğü gibi PrinciplePermission niteliğinden Role özelliğine Contributor değeri verilmiştir. Böylece Sum operasyonunu sadece Contributor rolündekilerin gerçekleştirebileceği ifade edilmektedir. İstemci tarafında yapılması gereken bir değişiklik yoktur. Nitekim rol bilgisi zaten kullanıcı adına göre AuthenticationService tarafından yakalanmaktadır.

Role Bazlı Yetkinlik Testleri

Eğer uygulamayı bu şekilde test edersek aşağıdaki çıktıları alırız.

[![sas_8](images/sas_8_thumb.png)](images/sas_8.png)

.ASPXAUTH cookie içeriğine dikkat edin. KullanıcıAdı|Role1|Role2 formatındadır. Şimdi tam tersi durumu da test edelim. Örneğin burak kullanıcısını Contributor rolünden çıkartıp Administrator rolünde bırakalım. (Ben Hard Coded yaptım rol ve kullanıcı bilgilerini ama siz öyle yapmayın tabi) Bu durumda aşağıdaki çalışma zamanı çıktısını alırız.

[![sas_9](images/sas_9_thumb.png)](images/sas_9.png)

Görüldüğü gibi burak kullanıcısı Administrator rolündedir ama Sum operasyonu sadece Contributor rolündekilere yetkilendirilmiştir. Dolayısıyla istemci tarafı bir Access is denied istisnası alacaktır.

Cookie İçeriğinin Şifrelenmesi

Geliştirdiğimiz bu örnekte herşey iyi görünmesine rağmen eksik olan bazı kısımlar bulunmaktadır. Söz gelimi cookie bilgisi plain text olarak gitmektedir. Bu sebepten Cookie bilgisinin en azından şifrelenmesi önemlidir. Şimdi bu işlemi nasıl icra edebileceğimize bir bakalım. Başlangıçta Cookie’ nin üretildiği yere müdahalede bulunmalı ve bir encryption işlemini gerçekleştirmeliyiz. Buna göre global.asax.cs içerisindeki Authentication metodunu aşağıdaki kod parçasında olduğu gibi kurcalayabiliriz.

```csharp
private void Authenticating(object sender, AuthenticatingEventArgs e) 
{ 
    SpecialValidator validator = new SpecialValidator(); 
    List<string> roles = null;

    e.Authenticated = validator.IsUserValid(e.UserName, e.Password,out roles); 
    e.AuthenticationIsComplete = true;

    if (e.Authenticated) 
    { 
        HttpCookie newCookie = new HttpCookie(FormsAuthentication.FormsCookieName); 
        newCookie.Value = e.UserName+"|"+CreateRolesString(roles);

        FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, e.UserName, DateTime.Now, DateTime.Now.AddHours(24), true, CreateRolesString(roles), FormsAuthentication.FormsCookiePath); 
        string encryptedValue = FormsAuthentication.Encrypt(ticket);

        HttpResponseMessageProperty response = new HttpResponseMessageProperty(); 
        response.Headers[HttpResponseHeader.SetCookie] = FormsAuthentication.FormsCookieName + "=" + encryptedValue; 
        OperationContext.Current.OutgoingMessageProperties[HttpResponseMessageProperty.Name] = response; 
    } 
}
```

Dikkat edileceği üzere ilk olarak bir FormsAuthenticationTicket nesne örneği oluşturulmuştur. Bu nesne örneği içerisinde kullanıcı adı, role, cookie’ nin yaşam süresi gibi bilgiler yer almaktadır. Şifreleme işlemi için FormsAuthentication tipinin static Encrypt metodundan yararlanılmıştır. Bu metod varsayılan olarak machine.config içerisinde belirtilen şifreleme algoritmasına göre bir encrpytion işlemi uygulamaktadır.

Şifrelenmiş Cookie İçeriğinin Çözümlenmesi

Peki şifrelenen Cookie içeriğini nasıl çözümleyeceğiz? Tahmin edileceği üzere bunun için AlgebraService sınıfının yapıcı metodunda bazı işlemler yapmamız gerekiyor ve yine başrollerde FormsAuthenticationTicket ile FormsAuthentication tipleri yer alacak. Aşağıdaki kod değişiklikleri senaryomuz için yeterli olacaktır.

```csharp
using System.Security.Permissions; 
using System.Security.Principal; 
using System.ServiceModel; 
using System.ServiceModel.Activation; 
using System.ServiceModel.Channels; 
using System.Threading; 
using System.Web.Security;

namespace AzonServices 
{ 
    [AspNetCompatibilityRequirements(RequirementsMode =AspNetCompatibilityRequirementsMode.Allowed)] 
    public class AlgebraService 
        : IAlgebraService 
    { 
        public AlgebraService() 
        { 
            var messageProperty = (HttpRequestMessageProperty)OperationContext 
                .Current 
                .IncomingMessageProperties[HttpRequestMessageProperty.Name];

            string[] cookieParts=messageProperty.Headers.Get("Set-Cookie").Split(','); 
            FormsAuthenticationTicket ticket = null; 
            for (int i = 0; i < cookieParts.Length; i++) 
            { 
               if (cookieParts[i].Contains("SecuredCookie")) 
                { 
                    ticket = FormsAuthentication.Decrypt(cookieParts[i].Split('=')[1]); 
               } 
            }            
            
            SpecialIdentity customIdentity = new SpecialIdentity 
            { 
                Name = ticket.Name, 
                 IsAuthenticated=true 
            }; 
            GenericPrincipal threadCurrentPrincipal = new GenericPrincipal(customIdentity,ticket.UserData.Split('|')); 
            Thread.CurrentPrincipal = threadCurrentPrincipal; 
        }

        [PrincipalPermission(SecurityAction.Demand,Role="Contributor")] 
        public double Sum(double x, double y) 
        { 
            return x + y; 
        } 
    } 
}
```

Gelen mesaj içeriğine ait Header kısmında SecuredCookie isimli bir name değeri yer almaktadır. Bu name değerinin karşılığı (value) ise bir FormsAuthenticationTicket nesne örneğine dönüştürülebilir. Dolayısıyla Decrypt metoduna bu parça parametre olarak verilmiştir. Elde edilen ticket nesne örneğine ait Name özelliği kullanıcı adını, UserData ise atanan rol bilgilerini içermektedir. Son olarak web.config dosyasında SecuredCookie için bir tanımlama yapmamız gerektiğini de hatırlatalım.

```xml
<?xml version="1.0"?> 
<configuration> 
  <system.web.extensions> 
    <scripting> 
      <webServices> 
        <authenticationService enabled="true"/> 
      </webServices> 
    </scripting> 
  </system.web.extensions>          
  <system.serviceModel> 
    <behaviors> 
      <serviceBehaviors> 
        <behavior name=""> 
          <serviceMetadata httpGetEnabled="true" httpsGetEnabled="true"/> 
          <serviceDebug includeExceptionDetailInFaults="true"/> 
        </behavior> 
      </serviceBehaviors> 
    </behaviors> 
    <serviceHostingEnvironment aspNetCompatibilityEnabled="true" multipleSiteBindingsEnabled="true"/> 
  </system.serviceModel> 
  <system.web> 
    <compilation debug="true"/> 
  <authentication mode="Forms"> 
            <forms slidingExpiration="true" name="SecuredCookie" protection="All"             timeout="20"/> 
        </authentication> 
  </system.web> 
</configuration>
```

Dilerseniz bu kısımda şifreleme için kullanılan metodu ve gerekli diğer parametreleri belirleyebilirsiniz (decryptionkey, validationkey vb…)

Şifreli Cookie için İstemci Testleri

Örneğimizi bu haliyle çalıştırdığımızda aşağıdaki sonuçları alırız.

[![sas_10](images/sas_10_thumb.png)](images/sas_10.png)

Dikkat edileceği üzere çerez içeriği şifrelelidir diyerek yazıyı kestirmeden sonlandırayım.Piuvvvvv!!!

Bu uzun makalemizde.Net Framework ile gelen Built-In AuthenticationService’ inin değiştirilip özel bir Membership alt yapısı ile nasıl çalışabileceğini incelemeye çalıştık. Adım adım ilerlediğimiz senaryomuzda kullanıcı ve role bazlı yetkilendirmelere değindik. Son olarak da çerez bilgisinin şifrelenerek gönderilmesinin nasıl yapılabileceğini gördük. Bir diğer yazımızda görüşmek dileğiyle hepinize mutlu günler dilerim.

[HowTo_CustomAuthentication.zip (96,45 kb)](assets/HowTo_CustomAuthentication.zip)
