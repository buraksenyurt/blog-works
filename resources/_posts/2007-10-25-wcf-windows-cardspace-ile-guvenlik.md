---
layout: post
title: "WCF - Windows CardSpace ile Güvenlik"
date: 2007-10-25 12:00:00 +0300
description: "WCF(Windows Communication Foundation) mimarisini baz alan SOA(Service Oriented Applications) uygulamaları geliştirilirken, güvenlik(security) başlığı altında ele alınmakta olan pek çok konu vardır. Geliştirilen bir WCF servisinin sadece izin verilen istemciler(clients) tarafından kullanılmasıda bu konulardan bir tanesidir. Bu amaçla istemci uygulamaların veya onları kullanan hesapların servis tarafında doğrulanması(authenticate) ve yetkilendirilmesi(authorize) adına bazı teknikler ele alınır. Temel olarak bir istemcinin doğrulanması ve yetkilendirilmesi, onun kim olduğunun bilinmesine bağlıdır(Identification). Kimlik tespiti için kullanıcı adı-şifre, sertifika(certificate) yada Kerberos kartı(token) gibi elemanlar göz önüne alınır. Doğrulama işlemi sırasında kullanılan kimlik tespiti tekniklerinden biriside .Net Framework 3.0 ile birlikte gelen Windows Cardspace teknolojisidir."
categories:
  - wcf
tags:
  - wcf
  - csharp
  - bash
  - xml
  - dotnet
  - linq
  - http
  - iis
  - authentication
  - authorization
  - generics
  - visual-studio
---
# WCF - Windows CardSpace ile Güvenlik
Değerli Okurlarım Merhabalar,

WCF (Windows Communication Foundation) mimarisini baz alan SOA (Service Oriented Applications) uygulamaları geliştirilirken, güvenlik (security) başlığı altında ele alınmakta olan pek çok konu vardır. Geliştirilen bir WCF servisinin sadece izin verilen istemciler (clients) tarafından kullanılmasıda bu konulardan bir tanesidir. Bu amaçla istemci uygulamaların veya onları kullanan hesapların servis tarafında doğrulanması (authenticate) ve yetkilendirilmesi (authorize) adına bazı teknikler ele alınır. Temel olarak bir istemcinin doğrulanması ve yetkilendirilmesi, onun kim olduğunun bilinmesine bağlıdır (Identification). Kimlik tespiti için kullanıcı adı-şifre, sertifika (certificate) yada Kerberos kartı (token) gibi elemanlar göz önüne alınır. Doğrulama işlemi sırasında kullanılan kimlik tespiti tekniklerinden biriside.Net Framework 3.0 ile birlikte gelen Windows Cardspace teknolojisidir.

> Windows CardSpace teknolojisi Windows Vista ile birlikte doğrudan gelmektedir. Nitekim Vista varsayılan olaran.Net Framwork 3.0 yüklü olarak yayınlanmaktadır. Diğer taraftan.Net Framework 3.0 yüklendiğinde Windows XP sürümlerinde de Windows CardSpace teknolojisi kullanılabilmektedir.

CardSpace teknolojisi sayesinde, istemciler kendi hazırladıkları kart bilgilerini güvenli bir şekilde servis uygulamasına iletebilirler. Eğer istemcilerin gönderdiği kart bilgileri içerisinde servisin ilgilendiği bilgiler var ise doğrulama (authentication) gerçekleşmiş olur. Bu aşamadan sonra ise doğrulanan kullanıcı için yine yetkilendirme (authorization) işlemlerine geçilir.

İşte bu makalemizde WCF (Windows Communication Foundation) ile geliştirilen servis yönelimli uygulamalarda CardSpace teknolojisini nasıl kullanabileceğimizi incelemeye çalışıyor olacağız. Her zamanki gibi konuyu daha iyi kavrayabilmek adına örnek bir senaryo ve uygulama üzerinden adım adım ilerliyor olacağız. Geliştireceğimiz örnekler Windows Vista Business işletim sistemi yüklü bir makine üzerinde geliştirilecektir. Ancak başlamadan önce kimlik (Identity) kontrolünü daha iyi kavrayabilmek adına aşağıdaki gerçek dünya senaryoları göz önüne alınabilir.

Birinci Senaryo; Kimlik bilgilerindeki detayların önemli olmadığı durumlar.

![mk228_5.gif](/assets/images/2007/mk228_5.gif)

Yukarıdaki şekildeki senaryoya göre bir spor klübüne girmek isteyen bir birey yer almaktadır. Kapıdaki iri kıyım görevlinin bu kişiyi içeri almasının tek şartı üye kartının (Membership Card) mevcut olmasıdır. Bunun dışında üye kartında yer alan detay bilgilerinin iri kıyım kapı görevlisi için hiç bir önemi bulunmamaktadır. Söz gelimi kart üzerinde yer alan üyelik numarası, üye adı veya üye sahibinin doğum tarihi gibi bilgiler çokda önemli değildir. Geçerli bir kartın olması yeterlidir. Bu senaryodaki yaklaşım maç günlerinde sadece kendi üyelerine açık olan klüp fanları içinde düşünülebilir.

İkinci Senaryo; Kimlik bilgisinin üçüncü bir sistem tarafından sağlandığı ve kontrol edildiği durumlar.

![mk228_6.gif](/assets/images/2007/mk228_6.gif)

Bu senaryoda çicek dükkanından kredi kartı ile alışveriş yapmakta olan bir kişi yer almaktadır. Kredi kartı ile yapılan alışverişlerde kartın üzerinde yer alan bilgiler kasa görevlisi tarafından olmasada banka merkezine bağlanan pos cihazı açısından önemlidir. Bu tip bir durumda kartın gerçekten kullanan kişiye ait olduğunun anlaşılması gerekmektedir. Diğer taraftan kart sahibinin bu alışverişi yapması için yeterli hakka sahip olup olmadığıda önemlidir. Eğer bu haklara sahip ise son aşamada kart sahibi olduğunu ispat etmek için uzun zamandır ülkemizdede uygulanan pin numarasınıda girmesi gerekmektedir. Burada kartın orjinal ve bakiyesinin yeterli olup olmadığını banka sistemi tarafından kontrol etmektedir.

Bu senaryolar kulağa hoş ve mantıklı gelsede, acaba WCF (Windows Communication Foundation) ve Windows CardSpace ile aralarında nasıl bir ilişkileri vardır? Burada bahesedilen senaryolar hak-tabanlı güvenlik (claims-based security) vakkalarına örnek olabilecek gerçek dünya yansımalarıdır. Bu tip bir güvenlik sisteminde kişilerin kim olduğundan ziyade, yapılması istenen işlemler için ilgili kişinin hakkı olup olmadığının tespit edilmesi önemlidir. İşte WCF mimarisi altında geliştirilen uygulamalarda Windows CardSpace teknolojisini sayesinde hak-tabanlı güvenlik (claims-based security) altyapısı (infrastructure) sağlanabilir. Hak-tabanlı güvenlik (Claim Based Security) aslında üç ana unsurdan oluşmaktadır. Bunlar aşağıdaki listede belirtildiği gibidir.

- Kullanıcı (Subject): Servise erişmek ve fonksiyonelliklerini çalıştırmak isteyen kullanıcı veya farklı bir sistem asıl öznenin (subject) kendisidir. Kullanıcı, erişmek istediği servise, onun kabul edebileceği haklar sunmakla yükümlüdür. Yukarıdaki senaryolar göz önüne alındığında, kredi kartı ile klüp kapısından girmek ne kadar mantıksızsa, klüp kartı ile çiçekçiden çiçek satın almakta o kadar mantıksızdır. Bir başka deyişle istemci tarafından sunulan hakların, servis tarafında kabul edilebilir nitelikte olması gerekmektedir.
- Kimlik Sağlayıcı (Identity Provider): Adındanda anlaşılacağı üzere, haklar için gerekli kimliği sağlamakta olan organizasyon veya varlıktır (entity). Yukarıdaki senaryolar göz önüne alındığında kredi kartını veren banka veya üye griş kartını veren fan klübü, kimlik sağlayıcı konumundadır.
- Güvenilir Şahıs veya Grup (Relying Party): Koruma altına alınmış hizmeti (Protected Service) sunan organizasyon yada varlıktır. Kimlik sağlayıcısına (Identity Provider), kullanıcının verdiği kartın, kullanıcının yapmak istediği işlem ile ilişkili haklara sahip olup olmadığını sormakla yükümlüdür. Söz gelimi kredi kartı ile alışveriş işlemini tasvir eden senaryoda çiçekçi yada bir başka deyişle satıcı (vendor) güvenilir şahıs (Relying Party) rolündedir.

Windows CardSpace kullanılaraktan istemciler farklı bilgiler içeren çeşitli bilgi kartları (Information Card) oluşturabilirler. Servis tarafında yer alan uygulamanın kendisi, doğrulayacağı kullanıcılardan gelecek olan bilgi kartlarını kendi belirleyeceği politikalara (Policy) göre kontrol edebilir. Bilgi kartları içerisinde çok farklı veriler yer alabilir. Kullanıcının adı, email adresi, yaşı, doğum tarihi, hatta sürücü belgesi veya pasaportu ile ilgili bilgiler dahi olabilir. Bu noktada Windows CardSpace ile hak tabanlı güvenlik (Claim-Based Security) ortamı sunulan bir WCF uygulamasında, istemcinin bir servis talebi sonrası neler olacağına bakmakta yarar vardır. Aşağıdaki maddelerde, istemci (client) ve WCF servisi (Service) arasında hak-tabanlı güvenlik (Claim-Based Security) gerçekleştirildiğinde izlenen sürece ait adımlar yer almaktadır.

- İlk olarak istemci (Client), servisten (WCF Service) bir talepte (Request) bulunur.
- Sonrasında, istemci uygulama üzerinde çalışan WCF çalışma zamanı (runtime), Windows CardSpace içerisinden kimlik seçici uygulamayı (Identity Selector) çağırır.
- Kimlik seçici (Identity Selector), servisten hak çeşidini (Claim Type) talep eder. Bir başka deyişle hakların neye göre doğrulanacağını ister. Örneğin kullanıcının pin numarası, email adresi, ev telefonu vb... bir hak çeşidi olarak elde edilebilir. (Bu bilgi elbetteki WCF servis uygulaması ve istemci yazılırken konfigursayon içerisinde belirtilir.)
- İstemci uygulamada çalışan Identity Selector, servisten gelen hak çeşidini bünyesinde barındırdan kartları kullanıcıya görsel bir arayüz ile sunar.
- İstemci uygulamayı çalıştırmakta olan kullanıcı bir kart seçimi gerçekleştirir. (Kullanıcılar istemci uygulamanın çalıştığı sistemde Windows CardSpace'i kullanarak istedikleri biçimde kart bilgileri oluşturabilirler. Bunu kart seçim aşamasında dahi yapabilirler.)
- İstemci uygulamada çalışan Identity Selector programı, kimlik sağlayıcı (Identity Provider) ile iletişim kurar ve istemciye ait kart bilgisi içinden hak çeşidi (Claim Type) ile ilişkili olanını gönderir.
- Kimlik sağlayıcısı (Identity Provider) gelen metadata bilgisini alır ve bir fiş (token) üreterek bunu Kimlik seçiciye (Identity Selector) gönderir.
- Identity Selector istemci programı çalıştıran kullanıcıya, oluşturulan fişin WCF servisine gönderilmesini onaylayıp onaylamadığını sorar. Eğer kullanıcı onaylarsa seçtiği bilgilere göre oluşturulan fiş (token) WCF servisine gönderilir.
- WCF Servisi gelen fiş bilgisini alır ve kullanıcının hakkının doğruluğunu kontrol eder. Eğer fiş (token) bilgisi geçerli ise içerisinde yer alan kimlik bilgisine (Identity Information) bakarak kullanıcının talepte bulunduğu operasyonu yapıp yapamayacağına karar verir. Bir başka deyişle bu adımda yetkilendirme (authorization) durumu ele alınır. Eğer yetki verilirse istemcinin talep ettiği fonksiyonellik çalıştırılır.

Durumu biraz daha görselleştirmek adına aşağıdaki akış diagramından da yararlanılabilir.

![mk228_7.gif](/assets/images/2007/mk228_7.gif)

Bu kadar teorik bilgiden sonra örnek bir senaryo üzerinden hareket etmekte yarar vardır. Örnekte servis tarafında basit bir fonksiyonellik sunulmaktadır. Söz gelimi standart toplama fonksiyonunu içeren bir WCF servis uygulaması tasarlanabilir. Servis ve istemci uygulamalar aynı makine üzerinde yer almaktadır. Bunun dışında servis ve istemcilerin mesaj seviyesinde güvenli (Message Level Security) bir şekilde haberleşmeleri önemlidir. Burada mesaj seviyesindeki haberleşmenin güvenli olabilmesi için sertifika (Certificate) kullanımı tercih edilmiştir. İlk önce işe, istemci tarafında Windows CardSpace teknolojisini kullanarak basit bir bilgi kartı (Information Card) hazırlayarak başlamakta yarar vardır. Bu amaçla Windows Vista üzerinde Control Panel içerisinde yer alan Windows CardSpace programının kullanılması yeterlidir. (Windows XP tabanlı sistemlerde de Windows CardSpace uygulamasına yine Control Panel üzerinde erişilebilir)

![mk228_8.gif](/assets/images/2007/mk228_8.gif)

Eğer daha önceden yüklenmiş bir bilgi kartı yok ise aşağıdaki gibi bir ekran ile karşılaşılacaktır.

![mk228_9.gif](/assets/images/2007/mk228_9.gif)

Buradan Add a Card opsiyonunu işaretlenerek devam edilir.

![mk228_10.gif](/assets/images/2007/mk228_10.gif)

Sıradaki adımda ise Create a Personel card opsiyonunu seçilir. Şu aşamada, kredi kartı yada pasaport bilgisi gibi verileri saklayacak bir bilgi kartı (Information Card) oluşturulmadığından Install a Managed Card seçeneğini ele alınmamaktadır. İzleyen adımda personel kart bilgilerinin girilmesi gerekmektedir. Bu ekran örnek olarak aşağıdaki gibi doldurulabilir.

![mk228_11.gif](/assets/images/2007/mk228_11.gif)

Hak (Claim) işlemleri email adresi üzerinden yapılacağı için bu bilginin mutlaka girilmesi önemlidir. İşlemler tamamlandıktan sonra hazırlanan kartın aşağıdaki gibi eklendiği görülecektir.

![mk228_12.gif](/assets/images/2007/mk228_12.gif)

Test amacıyla Garfi isimli hayali kişi için bir test kartı daha oluşturulmuştur. Bu kişinin email adresindeki bilgiye göre, servis üzerinde gerekli fonksiyonelliği çalıştırma yetkisi olmayacaktır. Amaç böyle bir durumda servisin nasıl bir davranış sergileyeceğinin izlenmesidir.

Artık servis tarafı tasarlanmaya başlanabilir. Servis uygulaması web üzerinden WsFederationHttpBinding tipini baz alacak şekilde tasarlanacaktır. Burada WsHttpBinding tipide göz önüne alınabilir. Federasyon yada birlik (Federation), farklı sistemler arasında doğrulama (authentication) ve yetkilendirme (authorization) adına kimlik bilgilerinden (örnekteki Windows CardSpace ile oluşturulanlar gibi) yararlanılmasını sağlayan bir kavramdır. Federation tarafından göz önüne alınan kimlik (Identity) bilgileri bir bilgisayar yada kullanıcıyı işaret edebilir. WsFederationHttpBinding bunun için gerekli olan alt yapıyı sunan hazır bir bağlayıcı tiptir (Binding Type) ve WS-Federation protokolünü desteklemektedir. Bu sebepten WsFederationHttpBinding tipi iletişim seviyesinde güvenliği (transport level security) desteklemez ve HTTP üzerinden iletişimi zorunlu kılar. Tekrardan uygulamaya dönülecek olursa; servis kütüphanesi (WCF Service Library) içerisinde basit olarak toplama fonksiyonelliği sunulmaktadır. WCF servis kütüphanesindeki tiplerin şematik gösterimi ve kod içerikleri aşağıda olduğu gibidir.

Sınıf Diagramı (Class Diagram);

![mk228_13.gif](/assets/images/2007/mk228_13.gif)

Sözleşme (IMatematik);

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace MatematikKutuphanesi
{
    [ServiceContract(Name="Matematik Servisi",Namespace="http://www.bsenyurt.com/Matematik/MatematikServisi")]
    public interface IMatematik
    {
        [OperationContract]
        double Topla(double x, double y);
    }
}
```

Matematik.cs;

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// System.IdentityModel.dll referans edilmelidir.
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.ServiceModel;

namespace MatematikKutuphanesi
{
    public class Matematik:IMatematik
    {
        #region IMatematik Members

        public double Topla(double x, double y)
        {
            // Önce talepte bulunan istemcinin ilgili işlem için hakkı olup olmadığı kontrol edilir.
            if (KontrolEt())
                return x + y;
            else
                throw new Exception("Doğrulanmadı");
        }

        private static bool KontrolEt()
        {
            // Doğrulama içeriği çekilir
            AuthorizationContext ctx = OperationContext.Current.ServiceSecurityContext.AuthorizationContext;
            // Doğrulama içeriğindeki her bir ClaimSets gezilir
            foreach (ClaimSet cSet in ctx.ClaimSets)
            {
                // ClaimSet' ler içerisinde Email hakkını içeren Claim' ler gezilir
                foreach (Claim clm in cSet.FindClaims(ClaimTypes.Email, Rights.PossessProperty))
                {
                    // O anki Claim' in Email değeri selim(at)buraksenyurt.com ise metod geriye true döndürür.
                    if (clm.Resource.ToString() == "selim(at)buraksenyurt.com")
                        return true;
                }
            }
            return false;
        }

        #endregion
    }
}
```

Bu sınıf içerisinde yer alan Topla metodunu, gönderdiği kart bilgisinde yer alan email adresi selim (at) buraksenyurt.com olan kullanıcı çalıştırabilir. Bunun kontrolü için KontrolEt isimli geriye bool değer döndüren bir metod geliştirilmiştir. Bu metod kendi içerisinde, istemciden gelen fiş (token) bilgisi ve içerisindeki Claim Set'lerin elde edilmesi işlemlerini gerçekleştirilir. İstemciden gelen fiş (token) bilgilerinin içeriğine bakabilmek için elde edilen AuthorizationContext referansının ClaimSets koleksiyonuna gidilir.

ClaimSets koleksiyonu içerisinde yer alan ClaimSet bilgilerinden hak tipi (Claim Type) email adresi olanların yakalanabilmesi içinde FindClaims metodu kullanılır. FindClaims metodunun döndüreceği koleksiyonun her bir elemanıda Claim tipindendir. Claim referanslarının Resource özelliklerinin değerleri object tipindendir. Bunun sebebi hak tipine göre gelen verinin farklı tiplerde olabilmesidir. Bunu geçerli bir mail adresi ile kıyaslamak için ToString metodu ile string tipine dönüştürme işlemi yapılmıştır. Burada elbetteki email adresi kontrolünü bir veritabanı (database) kaynağından yada hakkı olan email adreslerinin tutulduğu bir XML dosyasından yapmak çok daha mantıklıdır. Örneğin amacı şu aşamada sadece test olduğundan bu işlemler göz ardı edilmiştir.

> AuthorizationContext tipi System.IdentityModel.Policy isim alanı (Namespace) altında, ClaimSets ve Claim tipleri ise System.IdentityModel.Claims isim alanı altındadır. Her iki isim alanıda System.IdentityModel.dll assembly'ında yer aldıklarından söz konusu dll referansının projeye açıkça eklenmesi gerekmektedir. Aşağıdaki şekillde System.IdentityModel referansının MatematikKutuphanesi isimli WCF Servis Kütüphanesine eklenmiş hali görülmektedir.
> ![mk228_14.gif](/assets/images/2007/mk228_14.gif)

Artık servis tarafında, ilgili sözleşmeyi ve fonksiyonelliği sunacak olan uygulama tasarlanabilir. Burada istemci ve servis arasında kart bilgileri taşınacağından güvenilir bir ortam (Reliable Session) hazırlanması gerekmektedir. Bu nedenle servisin kart bilgisi gönderecek olan istemcileri ile, sertifika (certificate) aracılığıyla haberleşmesi gerekmektedir. Bir başka deyişle servis uygulamasının taleplerde (requests) kullanacağı ve istemcilerinde referans edeceği bir sertifika tanımlamasının yapılması gerekmektedir.

Bu amaçla ilk olarak, servis uygulaması için gerekli test sertifikasını üreterek işe başlanabilir. Bu sertifikanın üretimi ve servisin çalıştığı makine hesabına kaydı için Visual Studio 2008 Beta 2 Command Prompt üzerinden makecert.exe aracının aşağıdaki gibi kullanılması yeterlidir. Makecert aracı X.509 tabanlı test sertifikalarının üretilmesinde kullanılmaktadır.

```bash
makecert -sr LocalMachine -ss My -n CN=MatematikServisi -sky exchange
```

sr ile sertifikanın yükleneceği yer (location) belirtilirken, ss sonrasında gelen My ilede Certificate Store bilgisi tanımlanır. CN ifadesinden sonra oluşturulacak olan sertifikanın adı belirlenir. sky parametresinden sonra gelen değer ilede subject'in anahtar tipinin (Key Type) ne olacağı belirtilir. Burada exchange dışında signature değeride verilebilir. (Makecert aracının kullanımı ile ilişkili olarak daha detaylı bilgi için [http://msdn2.microsoft.com/en-us/library/bfsktky3 (VS.80).aspx](http://msdn2.microsoft.com/en-us/library/bfsktky3(VS.80).aspx) adresinden yardım alınabilir.) Bu komutun çalıştırılmasının ardından Microsoft Management Console (MMC) yardımıyla sertifika ayarlarına bakıldığında aşağıdaki ekran görüntüsünde olduğu gibi MatematikServisi isimli test sertifikasının başarılı bir şekilde Local Computer altında yer alan Personal kısmı altına eklendiği görülür.

![mk228_3.gif](/assets/images/2007/mk228_3.gif)

Sertifika tanımlaması yapıldığına göre servis tarafındaki uygulamanın yazılması ile işlemlere devam edilebilir. Servis uygulaması IIS üzerinde host edilmek üzere tasarlanmalıdır. Bu amaçla yeni bir WCF Service şablonu oluşturulur. Servis uygulaması, MatematikKutuphanesi isimli servis sınıf kütüphanesini (WCF Service Library) referans etmelidir.

![mk228_15.gif](/assets/images/2007/mk228_15.gif)

Servis tarafında yer alan Matematik.svc dosyasının içeriği aşağıdaki gibidir.

```text
<%@ ServiceHost Language="C#" Debug="true" Service="MatematikKutuphanesi.Matematik" %>
```

Servis tarafındaki belkide en önemli kısım konfigurasyon dosyasının içeriğidir. Nitekim burada kullanılacak olan sertifikanın bildirimi, hak (Claim) tipinin ne olacağı gibi ayarlamaların yapılması gerekmektedir. Bu noktada Microsoft Service Configuration Editor yardımıyla görsel olarak hazırlanan web.config dosyasındaki ServiceModel elementinin içeriği aşağıdaki gibidir.

```xml
<system.serviceModel>
    <bindings>
        <wsFederationHttpBinding>
            <binding name="MatematikServisiBindingConf" transactionFlow="true">
                <reliableSession enabled="true" />
                    <security>
                        <message issuedTokenType="urn:oasis:names:tc:SAML:1.0:assertion">
                            <claimTypeRequirements>
                                <add claimType="http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress" isOptional="false" />
                            </claimTypeRequirements>
                        </message>
                    </security>
                </binding>
            </wsFederationHttpBinding>
        </bindings>
    <behaviors>
        <serviceBehaviors>
            <behavior name="MatematikServisiBehavior">
                <serviceMetadata httpGetEnabled="true" />
                <serviceCredentials>
                    <serviceCertificate findValue="MatematikServisi" x509FindType="FindBySubjectName" />
                    <issuedTokenAuthentication allowUntrustedRsaIssuers="true" />
                </serviceCredentials>
                <serviceDebug includeExceptionDetailInFaults="true" />
            </behavior>
        </serviceBehaviors>
    </behaviors>
    <services>
        <service behaviorConfiguration="MatematikServisiBehavior" name="MatematikKutuphanesi.Matematik">
            <endpoint address="http://localhost/MatematikServisi/Matematik.svc" binding="wsFederationHttpBinding" bindingConfiguration="MatematikServisiBindingConf" name="MatematikServisiEndPoint" contract="MatematikKutuphanesi.IMatematik" />
        </service>
    </services>
</system.serviceModel>
```

Oluşturulan dosyada dikkat edilmesi gereken bir kaç nokta vardır. İlk olarak istemci ile servis arasında güvenilir bir oturum açılması gerekmektedir. Bu nedenle bağlayıcı tipe ait olan relaiableSession elementinin enabled niteliğinin (attribute) değeri true olarak belirlenmiştir. Bir sertifika kullanımı söz konusu olduğundan servis davranışlarından serviceCredentials ayarlarının yapılması gerekmektedir. Bunu sağlayabilmek için serviceCredential elementi aşağıdaki gibi oluşturulmuştur.

```xml
<serviceCredentials>
                    <serviceCertificate findValue="MatematikServisi" x509FindType="FindBySubjectName" />
```

Burada findValue niteliğine verilen değer, daha önceden oluşturulan MatematikServisi isimli sertifikadır. Bu X.509 tipindeki sertifikasının bulunabilmesi içinde x509FindType niteliğine FindBySubjectName değeri verilmiştir. Buna göre ilgili sertifika, nesne adına göre aranacaktır.

Servis tarafı, istemcinin talepte bulunduğu hizmetler için hakkı olup olmadığını, kart bilgisi ile gelen email adreslerine göre yapmaktadır. Burada politika (policy) olarak kart bilgisindeki email adresine bakılacağının söylenmesi gerekmektedir. Bu amaçla security elementi içerisinde aşağıdaki ayarlamalar yapılmıştır.

```xml
<security>
    <message issuedTokenType="urn:oasis:names:tc:SAML:1.0:assertion">
        <claimTypeRequirements>
            <add claimType="http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress" isOptional="false" />
        </claimTypeRequirements>
    </message>
</security>
```

Burada en önemli nokta claimType niteliğinin değeridir. Bu niteliğe (attribute) well-known URI formatında bir değer atanmıştır. Değerin sonunda yer alan emailaddress, hak için gerekli politikayıda (Claim Policy) belirlemektedir.

Burada yazılan URI bilgisi programatik olarak static olarak tanımlanmış ClaimTypes sınıfındaki özellikler (properties) üzerindende elde edilebilir. ClaimTypes sınıfının üyelerinin bir kısmının sınıf diagramındaki görüntüsü aşağıdaki gibidir. ClaimTypes sınıfı static bir sınıftır. Bu nedenle içerisindeki üyelerin tamamı static olarak tanımlanmıştır. Dolayısıyla bu üyelere nesne örneği olmadan tip adı ile erişilebilmektedir.

![mk228_16.gif](/assets/images/2007/mk228_16.gif)

Söz gelimi çalışma zamanında aşağıdaki kod parçası kulanıldığında,

```csharp
Console.WriteLine(ClaimTypes.DateOfBirth);
Console.WriteLine(ClaimTypes.HomePhone);
Console.WriteLine(ClaimTypes.PostalCode);
Console.WriteLine(ClaimTypes.Surname);
Console.WriteLine(ClaimTypes.Webpage);
```

Claim Tipi olarak kullanılabilecek URI bilgileride şu şekilde elde edilecektir. Burada örnek olarak doğum günü, ev telefonu, posta kodu, soyadı ve web sayfası gibi bilgiler için gereken Well-Known URI bilgileri gösterilmektedir.

![mk228_17.gif](/assets/images/2007/mk228_17.gif)

Diğer taraftan message elementi içerisinde issuedTokenType niteliğinede urn:oasis:names:tc:SAML:1.0:assertion değeri atanmıştır. Bu tanımlamaya göre WCF servisinin, Secure Application Markup Language 1.0 uyumlu bir fiş (token) beklediği belirtilmektedir. SAML, kimlik sağlayıcı (Identity Provider) ile servis sağlayıcı (Service Provider) arasında doğrulama (authentication) ve yetkilendirme (authorization) verilerinin değiş tokuş şeklini belirleyen bir XML standardıdır. Örnek uygulamada, aynı bilgisayardaki Windows CardSpace ile hazırlanmış kimlik bilgileri baz alınmaktadır. Bir başka deyişle üçüncü parti bir kimlik sağlayıcı (Identity Provider) tarafından üretilmiş bir kart mevcut değildir. Bu sebepten servisin güvenilmez (untrusted) kaynaklardan gelecek SAML fişlerinş (tokens) kabul edecek şekilde ayarlanması gerekir. Bunu sağlamak için serviceCredentials elementi altındaki issuedTokenAuthentication elemeninin allowUntrustedRsaIssuers seçeneğine true değeri atanmıştır.

İstemci uygulamaya geçmeden önce servis uygulaması herhangibir tarayıcı penceresinden talep edilirse KeySet Does Not Exist mesajlı bir çalışma zamanı istisnası alınabilir. Böyle bir durumda WinHttpCertCfg.exe aracı kullanılarak NetworkService yada AspNet hesaplarına (accounts), uygulamada kullanılan sertifika (Certificate) için kabul edilmiş erişim (grant access) haklarının verilmesi gerekmektedir. WinHttpCertCfg aracı [http://www.microsoft.com/downloads/details.aspx?familyid=c42e27ac-3409-40e9-8667-c748e422833f](http://www.microsoft.com/downloads/details.aspx?familyid=c42e27ac-3409-40e9-8667-c748e422833f) adresinde tedarik edilebilir. WinHttpCertCfg aracı aşağıdaki ekran görüntüsünde olduğu gibi kullanılmalıdır.

![mk228_18.gif](/assets/images/2007/mk228_18.gif)

Yukarıdaki komut ifadesine göre NetworkService hesabı için, MatematikServisi üzerine grant access hakkı verilmiştir. Artık servis tarafı internet tarayıcısı üzerinden elde edilebilir.

![mk228_19.gif](/assets/images/2007/mk228_19.gif)

Artık istemci tarafını yazmak için gerekli hazırlıklara başlanabilir. Sistemin çalışabilmesi için, üretilen test sertifikasının öncelikli olarak o anki kullanıcı için Trusted People olarak eklenmesi gerekmektedir. Bunu sağlayabilmek için Visual Studio 2008 Beta 2 Command Prompt üzerinden certmgr aracı aşağıdaki görüldüğü gibi kullanılmalıdır.

![mk228_1.gif](/assets/images/2007/mk228_1.gif)

İlk olarak söz konusu sertifikanın bir kopyası sadece public key değerini içerecek şekilde MatematikServisi.cer isimli dosyaya yazdırılır. Sonrasında çalıştırılan komut ilede, güncel kullanıcı (Current User) için Trusted People olacak şekilde eklenir. Bu işlemlerin ardından Microsoft Management Console programından yararlanılarak Current User altındaki Personel bölümüne bakıldığında aşağıdaki ekran görüntüsünde yer aldığı gibi ilgili sertifika bildiriminin yapıldığı görülür.

![mk228_2.gif](/assets/images/2007/mk228_2.gif)

Bu ön hazırlıkları tamamladıktan sonra istemci uygulamanın geliştirilmesine başlanabilir. İstemci program basit bir Console uygulaması olarak ele alınmaktadır. Console uygulamasında, geliştirilen servisin Add Service Reference seçeneği ile aşağıdaki ekran görüntüsünde olduğu gibi eklenmesi gerekmektedir.

![mk228_20.gif](/assets/images/2007/mk228_20.gif)

Sonrasında ise yine üretilen App.config dosyası istenirse görsel olarak istenirse doğrudan yazılarak aşağıdaki hale getirilmelidir.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <behaviors>
            <endpointBehaviors>
                <behavior name="ClientEndPointBehavior">
                    <clientCredentials>
                        <serviceCertificate>
                            <authentication certificateValidationMode="PeerTrust" revocationMode="NoCheck" />
                        </serviceCertificate>
                    </clientCredentials>
                </behavior>
            </endpointBehaviors>
        </behaviors>
        <bindings>
            <wsFederationHttpBinding>
                <binding name="MatematikServisiEndPoint" transactionFlow="true">
                    <reliableSession enabled="true" />
                    <security mode="Message">
                        <message algorithmSuite="Default" issuedKeyType="SymmetricKey" issuedTokenType="urn:oasis:names:tc:SAML:1.0:assertion" negotiateServiceCredential="true">
                            <claimTypeRequirements>
                                <add claimType="http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress" isOptional="false" />
                            </claimTypeRequirements>
                        </message>
                    </security>
                </binding>
            </wsFederationHttpBinding>
        </bindings>
        <client>
            <endpoint address="http://localhost/MatematikServisi/Matematik.svc" behaviorConfiguration="ClientEndPointBehavior" binding="wsFederationHttpBinding" bindingConfiguration="MatematikServisiEndPoint" contract="ServiceReference.MatematikServisi" name="MatematikServisiEndPoint">
                <identity>
                    <certificateReference x509FindType="FindBySubjectName" findValue="MatematikServisi" />
                </identity>
            </endpoint>
        </client>
    </system.serviceModel>
</configuration>
```

İstemci tarafındaki konfigurasyon dosyasının pek çok özelliği servis referansı eklendikten sonra otomatik olarak oluşturulur. Herşeyden önce istemci tarafında da bağlayıcı tip olarak wsFederationHttpBinding ele alınmaktadır. Diğer taraftan istemci ile servis arasında güvenilir bir oturum için (reliable session) gerekli ayarlamalar yapılmıştır. Aynen servis tarafında olduğu gibi hak tipi (Claim Type) email olacak şekilde belirlenmiş ve issuedTokenType niteliğine atanan değer ile SAML standardında bir fiş (token) yayınlanacağı bildirilmiştir. Sertifika (Certificate) bildirimi Identity elementi içerisinde yapılmaktadır. Servis tarafındakine benzer olarak sertifika adı ve neye göre aranacağı belirtilmektedir findValue ve x509FindType nitelikleri ile belirtilmektedir.

Konfigurasyon dosyasında gerekli değişiklikler yapıldıktan sonra Main metodu içerisinde aşağıdaki gibi bir kod bloğu geliştirilerek servisin kullanılması sağlanabilir.

```csharp
using System;
using System.Collections.Generic;
namespace CardSpaceIstemci
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ServiceReference.MatematikServisiClient cli = new CardSpaceIstemci.ServiceReference.MatematikServisiClient();
                Console.WriteLine(cli.Topla(3, 4).ToString());
            }
            catch (Exception excp)
            {
                Console.WriteLine(excp.Message);
            }
        }
    }
}
```

Servis tarafında includeExceptionDetailsInFaults özelliğinin değerini true olarak belirlenmiş olduğundan, servis tarafından fırlatılacak olan istisna mesajları (Exception Messages) istemci uygulama üzerinden kolaylıkla ele alınabilecektir. Tüm bu işlemlerin ardından istemci uygulama çalıştırıldığında, ekrana kart seçimi yapılması için bir sorgu penceresi açılacaktır. Bu sorgu penceresinde var olan kartlardan yararlanılabilir yada yeni bir kart oluşturularak gönderilmesi sağlanabilir.

![mk228_21.gif](/assets/images/2007/mk228_21.gif)

İlk olarak Burak Senyurt Personel Kartı isimli bilgi kartı (Information Card) seçildiğinden, kullanıcıya bir soru sorulacaktır. Bu soruda kullanıcının ilgili kart bilgisini servise göndermek isteyip istemediği belirtilir. Kullanıcı bunu kabul etmesse bir başka deyişle örneğin Esc tuşu ile arabirimden çıkarsa istemci tarafında yine bir çalışma zamanı hatası oluşacak ve bununla ilişkili bilgiler Log dosyasına aktarılacaktır.

![mk228_22.gif](/assets/images/2007/mk228_22.gif)

Send başlıklı düğmeye bastıktan sonra aşağıdaki ekran görüntüsünde olduğu gibi servis tarafında yapılan toplama işleminin sonucunun elde edilebildiği görülür.

![mk228_23.gif](/assets/images/2007/mk228_23.gif)

Ancak aynı uygulamada Garfi isimli diğer kart bilgisi gönderildiğinde servis tarafında üretilen istisna (Exception) mesajının alındığını görürüz. Bir başka deyişle istemci uygulamadan gönderilen kart bilgisi içerisindeki email adresine göre, servis tarafında toplama fonksiyonunun çalıştırılması için gereken hak koşulları sağlanamamıştır.

![mk228_24.gif](/assets/images/2007/mk228_24.gif)

Görüldüğü gibi Windows CardSpace kullanılarak, WCF uygulamalarında hak tabanlı güvenliği (Claim Based Security) sağlamak oldukça kullanışlı ve etkili bir yoldur. Burada, teknik detaylara çok fazla girilmeyerek adım adım bu tarz bir sistemin nasıl kurulabileceğinden bahsedilmeye çalışılmıştır. Nevarki kullanılan sertifika bir test sertifikası olup üçüncü parti bir sağlayıcı tarafından üretilen kart bilgileri ele alınmamıştır. Bu tarz gerçek senaryo uygulamalarındada sistemin tasarlanması ve kuralları çok fazla değişiklik göstermeyecektir.

Bu konu ile ilişkili detaylı bilgiyi benimde yakından takip ettiğim ve faydalandığım John Sharp imzalı MsPress yayınlarına ait [Microsoft Communication Foundation Step by Step](http://www.microsoft.com/MSPress/books/10022.aspx) kitabından da bulabilirsiniz. Sanılanın aksine Step by Step olarak belirtilmesine rağmen konu WCF olunca oldukça zor ve sıkı çalışılması gereken bir kitap olduğunuda vurgulamak isterim. Böylece geldik uzun bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](/assets/files/2007/WCFCardSpace.rar)