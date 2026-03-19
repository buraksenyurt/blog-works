---
layout: post
title: "Dayanıklı WCF Servisleri (Durable WCF Services)"
date: 2009-01-16 10:00:00 +0300
description: "Uzun zamandır Windows Communication Foundation(WCF) konulu bir araştırma üzerinde durmamıştık. Aslında WCF 4.0 ve WF 4.0 ile ilgili yeniliklerin uzun süredir konuşulduğu şu günlerde, WCF tarafında oldukça önemli bir yere sahip olan ve hatta Workflow Foundation açısından da değer arz eden bir konuya değiniyor olacağız. Dayanıklı WCF Servisleri(Durable WCF Services)."
categories:
  - wcf
tags:
  - wcf
  - csharp
  - xml
  - dotnet
  - sql-server
  - workflow-foundation
  - http
  - caching
  - transactions
  - visual-studio
---
# Dayanıklı WCF Servisleri (Durable WCF Services)
Değerli Okurlarım Merhabalar,

Uzun zamandır Windows Communication Foundation (WCF) konulu bir araştırma üzerinde durmamıştık. Aslında WCF 4.0 ve WF 4.0 ile ilgili yeniliklerin uzun süredir konuşulduğu şu günlerde, WCF tarafında oldukça önemli bir yere sahip olan ve hatta Workflow Foundation açısından da değer arz eden bir konuya değiniyor olacağız. Dayanıklı WCF Servisleri (Durable WCF Services).

Durable deyince aklıma gelen ilk şey, uzun ömürlü bir pil markası oluyor:) Sürer...Sürer...Sürerrr...İşin mizahi yanı bir tarafa dursun, Servis Yönelimli Mimari (ServiceOrientedArchitecture) uygulamalarında, bir servisin herhangibir zaman dilimindeki konumunun içeriği ile birlikte uzun süreliğine saklanabiliyor olması bazen elzem olan gereksinimlerden bir tanesidir. Nitekim bazı servis operasyonlarının gerçekleştirilmesi sırasında servisin kapanması veya istemcinin tekrardan başlatılması gibi vakaları olasıdır. Yada çok uzun süren, içerisinde insan faktörü olan bir sürecin ele alındığı herhangibir servisin, belirli zaman dilimlerinde son içeriğinden yüklenerek çalıştırılması gerekebilir.

Normal şartlarda WCF servisleri istemci ile oturum bazlı (Session Based) haberleşmektedir. Buna göre istemciler her proxy nesnesi örnekleyip servise bağlandıklarında bir oturum açmış olurlar. Ne varki servisin herhangibir nedenle düşmesi, istemcinin programı kapatması gibi durumlarda bu oturum bilgileri çok doğal olarak kaybolabilir. Servisin düşmesi sonrasında, üzerinde taşıdığı tüm veriler bir tedbir alınmadığı sürece yok olabilir. İstemci uygulama yeniden başlatıldığında, servis ayakta bile olsa yeni bir proxy örneği oluşturduğundan, daha önceki servis verilerine çok doğal olarak ulaşılamayadabilir. Halbuki belkide istemci tarafından çağırılan operasyonların bazıları, kendisini zaman dilimleri içerisinde koruyan verilerin son haline ihtiyaç duyabilir.

Özellikle WF yapısı içerisinde de, bu tip durağansızlıklar oldukça önemli vakalar arasında yer almaktadır. Öyleki bir WF içerisinde çok uzun süren operasyonların (Long Running Operations) olması çok daha yüksek bir ihtimaldir. Bu nedenle WF'in kendi doğasında olan akışın herhangibir zamandaki durumunu o anki verileri ile saklamak gibi bir misyonuda var (Persistence). Günümüzde WF ile WCF'in özellikle.Net Framework 3.5 sürümünden itibaren birbirleri ile daha sıkı fıkı olduklarıda göz önüne alındığında, konunun önemi bir kat daha artmaktadır. Bu basit bilgiler dahi, bir servisin içerisindeki operasyon bazlı verilerin kalıcı olarak saklanabilmesinin gerekliliğinide ortaya çıkarmaktadır. Öyleyse biraz daha derinlere dalmaya ne dersiniz?

Herşeyden önce servis tarafında kalıcı olarak saklanacak olan nesne nedir? Bu çok doğal olarak servis sınıfına ait sunucu tarafından üretilen örneğin kendisi olmalıdır. Diğer bir noktada, servis nesnesinin nerede saklanacağıdır? Burada akla gelen en pratik çözüm çok doğal olarak bir veritabanıdır. Nitekim veritabanı içerisinde fiziki olarak servis verilerin saklanması, saklanan satırların kolayca yönetilmesi ve daha pek çok avantaj söz konusudur. Çok şükürki veritabanı tarafında gerekli tabloların (Tables) ve saklı yordamların (Stored Procedures) betikleri (scripts).Net Framework 3.5 ile birlikte gelmektedir.(.Net Framework 3.0 içerisindede bu script dosyalarını olduğunu vurgulayalım. Ancak 3.5 tarafında pek çok bug'ın düzeldiğide bir gerçektir.) Her ne kadar buradaki veritabanı yapısını kurmak düşünüldüğünde çok zor olmasada,.Net Framework 3.5 içerisinde bu işlem için gerekli tipler (Types) ve veritabanı oluşturma betiklerinin hazır olarak gelmesi geliştiricilerin işini oldukça kolaylaştırmaktadır.

> Servis nesnelerinin depolanması için bir SQL veritabanının kullanılması şart değildir. Varsayılan olarak System.WorkflowServices.dll assembly'ı içerisindeki System.ServiceModel.Persistence isim alanı (Namespace) altında yer alan SqlPersistenceProviderFactory tipi kullanılmaktadır.Bu tip doğrudan belirtilen SQL bağlantı cümlesindenki depolama alanını kullanmaktadır. Ancak istenirse, PersistenceProviderFactory abstract sınıfından türetme yapılarak özel bir depolama alanının kullanılmasıda sağlanabilir.

Depolama alanının belirtilmesi elbetteki yeterli değildir. Bununla birlikte WCF çalışma zamanının (Runtime), niteliklerden (Attributes) yararlanarak depolama alanına servis örneklerini ekleme veya kaldırma gibi işlemleri hangi operasyonlar gerçekleştiğinde yapacağının bildirilmesi gerekmektedir. Hatta hangi servis tipinin durable olarak bırakılacağınında WCF çalışma zamanına söylenmesi önemlidir. Bu noktada devreye Durable, DurableOperation gibi nitelikler (Attribute) girmektedir.

Aslında SQL depolama alanı varsayılan olarak kullanıldığında, sistem son derece basit bir çalışma mantığına sahiptir. Dayanıklı WCF servislerinde istemcinin servis tipine ait bir proxy oluşturmasının ve Durable hale gelme özelliğini başlatan operasyonu çağırmasının ardından, sunucu tarafındaki depolama alanında servis örneğinin serileştirilmiş bir örneği hemen ilgili tabloya kayıt olarak eklenmektedir. Bu kayıt bir GUID ile ilişkilendirilmekte ve böylece istemci söz konusu GUID'e sahip olduğunda, istediği zaman (elbetteki tablodaki verinin başına bir şey gelmediği, istemci ile servis arasındaki ağ bağlantısında bir aksaklık olmadığı sürece vb...) aynı servis örneğinin verisine erişip operasyonlarında kullanabilir. Anlattığımız bu çalışma sistemi varsayılan olarak SQL depolama alanı kullanıldığı durum için geçerlidir. Nitekim GUID üretimi, serileştirilen servis nesne örneğinin tabloya eklenmesi, bilgisinin güncellenmesi, silinmesi gibi adımlar veritabanı bağımlı işlemlerdir. Bu durumu aslında aşağıdaki şekil ile kafamızda biraz daha netleştirebiliriz.

![mk266_14.png](/assets/images/2009/mk266_14.png)

Bu görselde sadece iki farklı zaman dilimi için örnek bir durum göz önüne alınmaktadır. İlk olarak istemci proxy örneğini oluşturduktan sonra bir operasyon çağrısı gerçekleştirir. Bu operasyon çağrısı sonrasında, servis örneğinin depolama alanına atılması ve bir GUID'in üretilerek sonraki zaman dilimleri için istemciye gönderilmesi söz konusudur. Senaryo gereği ileri bir zaman diliminde istemci aynı servis örneğini daha önceden kaydettiği içeriği ile yeniden kullanmak istemektedir. Bu durumda servisin T zamanındaki örnek içeriği depolama alanında durduğundan ve istemcinin bu servisi bulması için gerekli GUID değeri var olduğundan (ki bu değer aynı zamanda servisin instanceId değeri ile eşittir), T+N zamanında istemci uygulama, daha önceki servis içeriğini son haliyle kullanmaya devam edebilir. Elbette zaman dilimi ilerledikçe servisin durağanlığı devam ettirilebilir. Ne zamanki istemci, servis örneğinin tablodan silinmesi için gerekli talepte bulunursa (ki bunu ilerleyen kısımlarda göreceğiz), ilgili örnek depolama alanından kaldırılır.

Dilerseniz bu kadar konuşmayı bir kenara bırakalım ve adım adım dayanıklı, sapa sağlam bir WCF Servis örneği nasıl yazılabilir öğrenmeye ve en önemliside anlayama çalışalım. İlk olarak veritabanı tarafındaki gerekli hazırlıkları yapmamız gerekiyor. Bir başka deyişle depolama tablolarını, ve CRUD (CreateReadUpdateDelete) işlemleri için gerekli Stored Procedure'lerin oluşturulması lazım. Bu amaçla varsayılan olarak Windows XP tabanlı bir sistemde varsayılan olarak C:\WINDOWS\Microsoft.NET\Framework\v3.5\SQL\EN adresinde bulunan SqlPersistenceProviderSchema ve SqlPersistenceProviderLogic isimli SQL betiklerini sırasıyla çalıştırıyoruz. Hemen belirtelim, ilk olarak şemaları oluşturan SqlPersistenceProviderSchema betiğinin çalıştırılması gerekiyor.

![mk266_1.gif](/assets/images/2009/mk266_1.gif)

Yine önemli bir noktada şu. Aman Master veritabanı altında çalıştırmayın:) Depolama alanının veritabanı adını istediğiniz gibi belirleyebilirsiniz. Örnekte söz konusu betikler, SQL Server 2008 üzerinde (ki 2005 üzerindede yapabilirsiniz) WCFPersistenceStore isimli veritabanı altında çalıştırılmaktadır. Sonuç olarak aşağıdaki şekilde görülen nesneler oluşur.

![mk266_2.gif](/assets/images/2009/mk266_2.gif)

Görüldüğü gibi servis örneklerini tutacak olan InstanceData isimli basit bir tablo ve CRUD operasyonları ile kilit açma işlemleri için birer saklı yordam oluşturulmuştur. Tablo alanları şöyle bir gözden geçirildiğinde uniquidentifier tipinden bir id alanının olduğunu görüyoruz ki bu aslında istemci tarafı içinde önem arz eden instanceId değeri olacaktır. Öyleki istemcinin uygulamayı kapatıp tekrar açtıktan sonra daha önceden var olan bir instance'a ait servis ve verisine erişebilmesi istenebilir. Bu durumu bir sonraki makalemizde ele almaya çalışacağız. Bunlara ek olarak instanceXML isimli XML veri tipindeki alanında bizim için şu aşamada dikkate değer olduğunu söyleyebiliriz. Nitekim servis örneğinin veri içeriği bu alanda serileştirilmiş olarak tutulacaktır. Dolayısıyla servisin serileştirilebilir olmasının gerektiği ortaya çıkmaktadır. (Bu durum göz önüne alındığında aklıma hep web tarafında Session'ların veritabanında tutulması gelmektedir. Nitekim bu durumdada Session içeriğinin veritabanındaki küçük bir alana olduğu gibi aktarılabilmesi için serileştirilebilir olması şarttır:))

Veritabanı tarafındaki hazırlıklarıda bu şekilde tamamlandıktan sonra artık örnek bir servisin yazılmasına başlayabiliriz. Örnek WCF servisimiz.Net Framework 3.5 tabanlı bir WCF Service Library'dir ve kütüphane için belkide en önemli nokta System.WorkflowServices (.Net Framework 3.5) assembly'ını referans edilmesi gerekliliğidir.

![mk266_3.gif](/assets/images/2009/mk266_3.gif)

Bu nedenle sınıf kütüphanesine söz konusu assembly'ı ekleyerek yolumuza devam edebiliriz. Servis kütüphanesi içerisinde yer alan tiplerin kod içerikleri, sınıf diagramı görüntüsü ve app.config içeriği ise aşağıda olduğu gibidir.

Sınıf diagramı;

![mk266_7.gif](/assets/images/2009/mk266_7.gif)

ICommonService isimli servis sözleşmesi içeriği;

```csharp
using System;
using System.ServiceModel;

namespace ServiceLib
{
    [ServiceContract(
        Name="ServiceCommon"
        ,Namespace="http://www.bsenyurt.com/CommonService")]
    interface ICommonService
    {
        [OperationContract]
        void Start();

        [OperationContract]
        void IncreaseValue(int value);
    
        [OperationContract]
        Guid GetInstanceId();

        [OperationContract]
        void Stop();
    }
}
```

Görüldüğü üzere servis sözleşmesi içerisinde sembolik olarak 4 farklı operasyon tanımı bulunmaktadır. Şu anda amacımız sadece dayanıklı bir WCF servisi yazmak olduğundan bize çokda anlamlı gelmeyen operasyonlarımızın mevcut olduğunu söyleyebiliriz. Operasyonların uyarlamasının yapıldığı CommonService isimli sınıf içeriği ise aşağıdaki gibidir.

CommonService sınıfının içeriği;

```csharp
using System;
using System.ServiceModel.Description;

namespace ServiceLib
{
    [Serializable]
    [DurableService]
    class CommonService
        :ICommonService
    {
        int commonValue;

        #region ICommonService Members

        [DurableOperation(CanCreateInstance=true)]
        public void Start()
        {
            commonValue = 1; 
        }

        [DurableOperation()]
        public void IncreaseValue(int value)
        {
            commonValue += value;
        }

        [DurableOperation()]
        public Guid GetInstanceId()
        {
            return System.ServiceModel.Dispatcher.DurableOperationContext.InstanceId;
        }

        [DurableOperation(CompletesInstance=true)]
        public void Stop()
        { 
        }

        #endregion
    }
}
```

Aslında burada daha önceki servis geliştirmelerimizden farklı olarak dikkate değer bir kaç nokta bulunmaktadır. Herşeyden önce sınıfın Serializable ve DurableService nitelikleri ile imzalandığını görüyoruz. Serileştirilebilir olması bir gereklilik. Nitekim servis tipinin çalışma zamanı örneğinin, InstanceData tablosundaki instanceXML veya instance alanlarına serileşmesi söz konusudur. Hangi alana serileşeceği bilgisi ise konfigurasyon dosyasında belirtilebilir. Yani servis nesne örneğinin veri içeriğinin XML olarak yada binary formatta tutulması sağlanabilir.

Dayanıklı bir servis tanımlamasında en önemli noktalar DurableService ve DurableOperation nitelikleridir. Örneğin servis operasyonlarında olan Start metodu çalıştığında SQL üzerinde CommonService örneği için bir satır oluşturulacaktır. Diğer taraftan bu satırın devamlılığı herhangibir istisnai durum oluşmassa Stop operasyonuna yapılan çağrı ile son bulacaktır. Nitekim Stop operasyonundaki DurableOperation niteliğinde CompleteInstance özelliğine true değeri verilmiştir. Dayanıklı servisler tasarlanırken oturum bazlı çalışma önemlidir. Bu nedenle aşağıdaki koşullara dikkat edilmesi gerekmektedir.

- Dayanıklı WCF servislerinde içerik bazlı bağlayıcı tipler (Content Based Binding Types) kullanılmalıdır. Örneğin WSHttpContextBinding, BasicHttpContextBinding veya NetTcpContextBinding gibi.
- InstanceContextMode özelliğinin değeri PerSession olmalıdır ki varsayılan olarak örneğimizde böyledir.
- ConcurrencyMode özelliğinin değeri Multiple olmamalıdır.
- Eğer sözleşmede sessionlar desteklenmiyorsa tüm operasyonlara ait DurableOperation niteliklerinin CanCreateInstance özelliklerine true değeri atanmalıdır ve eğer böyle bir durum söz konusu ise IsOneWay true olarak set edilmemelidir.
- Eğer DurableService niteliğinin SaveStateInOperationTransaction değeri true olarak belirlenirse, tüm operasyonların OperationBehavior niteliği ile imzalanması ve TransactionScopeRequired özelliklerinede true değeri atanması gerekmektedir. Yada, TransactionFlowOption özelliğinin değerinin Mandatory olması gerekir. Tüm bunlara ilavetende ServiceBehavior niteliğinin ConcurrencyMode özelliğinin değerinin Single olarak belirlenmesi gerekmektedir.

Görüldüğü gibi oldukça kafa karıştırıcı bir şartname dizisi ile karşı karşıyayız. Ancak gerçek hayat vakalarında bu durumlar altın değerinde önem taşımaktadır. Gelelim servis tarafındaki konfigurasyon dosyası içeriğine.

Servis tarafındaki App.config içeriği;

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <connectionStrings>
        <add name="WCFPerstConStr" connectionString="data source=.;database=WCFPersistenceStore;integrated security=SSPI"/>
    </connectionStrings>
    <system.web>
        <compilation debug="true" />
    </system.web> 
    <system.serviceModel>
        <services>
            <service name="ServiceLib.CommonService" behaviorConfiguration="ServiceLib.CommonServiceBehavior">
                <host>
                    <baseAddresses>
                        <add baseAddress = "http://localhost:8731/Design_Time_Addresses/ServiceLib/CommonService/" />
                    </baseAddresses>
                </host>
                <endpoint address ="" binding="wsHttpContextBinding" contract="ServiceLib.ICommonService"> 
                    <identity>
                        <dns value="localhost"/>
                    </identity>
                </endpoint> 
                <endpoint address="mex" binding="mexHttpBinding" contract="IMetadataExchange"/>
            </service>
        </services>
        <behaviors>
            <serviceBehaviors>
                <behavior name="ServiceLib.CommonServiceBehavior"> 
                    <serviceMetadata httpGetEnabled="True"/>
                    <serviceDebug includeExceptionDetailInFaults="False" />
                        <persistenceProvider
                            type="System.ServiceModel.Persistence.SqlPersistenceProviderFactory, System.WorkflowServices, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
                            connectionStringName="WCFPerstConStr"
                            persistenceOperationTimeout="00:00:10"
                            lockTimeout="00:01:00"
                            serializeAsText="true"/>
                </behavior>
            </serviceBehaviors>
        </behaviors>
    </system.serviceModel>
</configuration>
```

Bu şekilde bakıldığında kimimize korkutucu gelebilen konfigurasyon içeriğindeki ayarların çoğunu Microsoft Service Configuration Editor aracılığıyla yapabileceğimizi unutmayalım. Konfigurasyon dosyasındanda görüldüğü üzere bağlayıcı tip (Binding Type) olarak içerik tabanlı wsHttpContextBinding örneği kullanılmaktadır. Daha öncedende belirttiğimiz gibi NetTcpContextBinding veya BasicHttpContextBinding bağlayıcı tipleride ele alınabilir.

Önemli noktalardan ilki connectionString elementi değeridir. Hatırlayacağınız gibi örneğimizdeki servis verilerini kalıcı olarak WCFPersistenceStore isimli örnek bir veritabanı üzerinde tutmak amacıyla bir takım ön hazırlıklar yapmıştık. Bu bilgi SqlPersistenceProviderFactory tipi için önemlidir. Nitekim provider'ın hangi bağlantıyı kullanarak işlemler yapacağını bilmesi gerekmektedir. Bu sebepten dolayıda persistenceProvider isimli servis davranışı içerisinde bir connectionStringName niteliği bulunmaktadır.

İkinci önemli nokta elbetteki persistenceProvider isimli servis davranışı ve içeriğidir. Burada dayanıklı depolama alanı için gerekli pek çok ayar yapılmaktadır. Örneğin servis verisinin text tabanlı olarak serileştirilip serileştirilmeyeceği serializeAsText değeri ile belirtilir. Kilitleme ve operasyonlar için zaman aşımı süreleri lockTimeout ve persistenceOperationTimeout niteliklerine atanan değerler ile belirtilir.

Üçüncü önemli nokta ise, persistenceProvider elementi içerisinde yer alan type niteliğine SqlPersistenceProviderFactory değerinin atanmasıdır. Bu varsayılan olarak ve sürekli vurguladığımız üzere SQL üzerinde depolama yapılacağını belirtmektedir. type niteliğindeki bu tanımlama sırasında tipin qualified name değerinin tam olarak verilmesi gerekir (TipAdı, AssemblyAdı, Assembly Versiyonu,Assembly Culture bilgisi, Assembly PublicKeyToken değeri).

> PublicKeyToken değerini kolay bir şekilde Global Assembly Cache (GAC) içerisinden de bulabiliriz. Bunun için aşağıdaki ekran görüntüsünde olduğu gibi Assembly klasörüne gitmemiz yeterlidir.
> ![mk266_8.gif](/assets/images/2009/mk266_8.gif)

Servis tarafında yaptığımız bu hazırlıkların ardından artık istemci tarafını da geliştirmeye başlayabiliriz. Dayanıklı bir WCF servisini test ederken en basit haliyle bir Console Application bizim için biçilmiş kaftan olacaktır. Console uygulamamızı servis ile aynı Solution üzerinde geliştirdiğimizden, servise referansını eklememizde son derece kolay olacaktır. Bu amaçla Console uygulamasında Add Service Reference seçeneğini kullandıktan sonra aşağıdaki ekran görüntüsünde olduğu gibi WCF servisine ulaşılabilinir.

![mk266_5.gif](/assets/images/2009/mk266_5.gif)

Servis referansını istemci tarafına ekledikten sonra otomatik olarak üretilen konfigurasyon dosyası içeriği aşağıdaki gibi olacaktır.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <bindings>
            <wsHttpContextBinding>
                <binding name="WSHttpContextBinding_ServiceCommon" closeTimeout="00:01:00"
                    openTimeout="00:01:00" receiveTimeout="00:10:00" sendTimeout="00:01:00"
                    bypassProxyOnLocal="false" transactionFlow="false" hostNameComparisonMode="StrongWildcard"
                    maxBufferPoolSize="524288" maxReceivedMessageSize="65536"
                    messageEncoding="Text" textEncoding="utf-8" useDefaultWebProxy="true"
                    allowCookies="false" contextProtectionLevel="Sign">
                    <readerQuotas maxDepth="32" maxStringContentLength="8192" maxArrayLength="16384" maxBytesPerRead="4096" maxNameTableCharCount="16384" />
                    <reliableSession ordered="true" inactivityTimeout="00:10:00" enabled="false" />
                    <security mode="Message">
                        <transport clientCredentialType="Windows" proxyCredentialType="None" realm="" />
                        <message clientCredentialType="Windows" negotiateServiceCredential="true" algorithmSuite="Default" establishSecurityContext="true" />
                    </security>
                </binding>
            </wsHttpContextBinding>
        </bindings>
        <client>
            <endpoint address="http://localhost:8731/Design_Time_Addresses/ServiceLib/CommonService/" binding="wsHttpContextBinding" bindingConfiguration="WSHttpContextBinding_ServiceCommon" contract="CommonServiceReference.ServiceCommon" name="WSHttpContextBinding_ServiceCommon">
                <identity>
                    <dns value="localhost" />
                </identity>
            </endpoint>
        </client>
    </system.serviceModel>
</configuration>
```

Dikkat edileceği üzere istemci tarafında oluşturulan EndPoint içerisinde de aynen servis tarafındaki konfigurasyon dosyasında olduğu gibi wsHttpContextBinding bağlayıcı tipi kullanılmaktadır. Sınıf diagramındada izlenebileceği gibi servis referansının eklenmesi ile birlikte proxy üretimi için gerekli tiplerde otomatik olarak istemci tarafına eklenmektedir.

![mk266_9.gif](/assets/images/2009/mk266_9.gif)

Program kodlarımızı ise test amaçlı olarak aşağıdaki gibi geliştirebiliriz.

```csharp
using System;
using ClientApp.CommonServiceReference;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceCommonClient client = new ServiceCommonClient("WSHttpContextBinding_ServiceCommon");

            client.Start();
            Console.WriteLine("Service başlatıldı");
            client.IncreaseValue(10);
            Guid instanceId=client.GetInstanceId();
            Console.WriteLine("Instance değeri {0}",instanceId.ToString());
            client.Stop();
            Console.WriteLine("Uygulamadan çıkmak için bir tuşa basınız");
            Console.ReadLine();
        }
    }
}
```

İlk olarak proxy sınıfına ait nesne örneği oluşturulmaktadır. Hemen ardından Start, IncreaseValue, GetInstanceId ve Stop servis metodları sırasıyla çağırılır. Hatırlanacağı üzere Start metodu ile Servis örneğinin tabloya atılması, Stop metodundan sonra ise tablodan servise ait ilgili satırın silinmesi gerekmektedir. Bu ilk vakayı analiz etmek için servis kütüphanesi ve istemci uygulamayı aynı anda çalıştırmalıyız. Bu nedenle iki ön hazırlık yapılmalıdır. Bildiğiniz gibi Visual Studio 2008 ile birlikte WCF Servis kütüphanelerini başlatılabilir ve built-in gelen servis, istemci uygulamalarını kullanarak testler yapılabilir. Bu örneğimizde istemci tarafını kendimiz geliştirdiğimiz için servis kütüphanesi özelliklerinden WcfTestClient.exe'nin çalıştırıldığı komut satırı parametresini kaldırmamız gerekmektedir.

![mk266_4.gif](/assets/images/2009/mk266_4.gif)

Bu işlemin ardından ise Solution özelliklerine gidip önce sınıf kütüphanesinin sonrasında ise istemci uygulamanın çalıştırılması gerektiğini belirtmeliyiz.

![mk266_6.gif](/assets/images/2009/mk266_6.gif)

Böylece servis kütüphanesine ait test sunucusu önce çalışacak sonrasında ise istemci uygulamamız yürütülecektir ki bu hazırlık özellikle kodu debug etmemizi kolaylaştıracaktır. İlk test için istemci tarafındaki Start metodu çağrısına bir breakpoint koymamız gerekmektedir.

![mk266_10.gif](/assets/images/2009/mk266_10.gif)

Şimdi uygulamayı çalıştırıp Start metodunu Step Over ile geçtiğimizde SQL sunucusu üzerindeki instanceData tablosunda yeni bir satır oluşturulduğunu görebiliriz.

![mk266_11.gif](/assets/images/2009/mk266_11.gif)

Dikkat ederseniz servis örneklendiğinde değil, DurableOperation niteliğinde CanStartCreateInstance değeri true olan Start metodu çağrısından sonra bu satır eklenmiştir. Satırın id değeri, servis çağrısı için üretilen GUID değeridir ve aslında bu değer içerik ile birlikte istemciyede gönderilmektedir. Bu nedenle oturum içerisinde yapılacak olan diğer operasyon çağrılarında bu GUID numarası kullanılır. instanceXml isimli alanın içeriğine bakıldığında (özellikle IncreaseValue metodundan önce) servisin aşağıdaki içeriğe sahip olduğu görülebilir.

![mk266_12.gif](/assets/images/2009/mk266_12.gif)

CommonService içerisinde tanımlanmış olan commonValue alanının ilk atanan değeri burada açık bir şekilde görülmektedir. Elbetteki serileşen tipin içerisinde kaç tane alan (field) varsa, bunların anlık olarak değerlerinin tamamı serileştirilen bu içeriğe atanmaktadır. Eğer kodda F10 (Step Over) ile ilerlenmeye devam edilirse IncreaseValue metodu geçildikten sonra XML içeriğinin aşağıdaki hale geldiği görülür.

![mk266_13.gif](/assets/images/2009/mk266_13.gif)

Bir başka deyişle servis nesne örneğinin depolama alanında tutulan içeriğindeki alanda da güncelleme yapılmıştır. Kod bu şekilde sonuna kadar devam ettirildiğinde ve Stop metodu çağrısıda F10 ile geçildiğinde artık CommonService'in şu anki örneği için açılan satırın artık olmadığı açık bir şekilde gözlemlenebilir. Ki burada Stop metodundaki DurableOperation niteliğinde CompleteInstance özelliğine true değerini atadığımızı hatırlamamızda yarar vardır. Bu basit işleyişi tekrar edip SQL Server Profiler aracı yardımıyla arka plana bakıldığında ise aşağıdaki gibi bir sorgu dizisinin çalıştırıldığı gözlemlenmektedir.

Start Metodu Çağrısında
InsertInstance SP'si için çağrı

declare @p3 xml
set @p3=convert (xml,N'1')
declare @p7 int
set @p7=0
exec InsertInstance @id='1B9F9AA9-4F5C-46F1-97ED-C520716CE7AE', @instance=default, @instanceXml=@p3,@unlockInstance=1,@hostId='97CE292D-D465-49B9-9200-4A55BCE2D7B3',@lockTimeout=60,@result=@p7 output
select @p7

IncreaseValue Metodu Çağrısında
LoadInstance SP'si için çağrı

declare @p5 int
set @p5=0
exec LoadInstance @id='1B9F9AA9-4F5C-46F1-97ED-C520716CE7AE', @lockInstance=1,@hostId='97CE292D-D465-49B9-9200-4A55BCE2D7B3', @lockTimeout=60,@result=@p5 output
select @p5

UpdateInstance SP'si için çağrı

declare @p3 xml
set @p3=convert (xml,N'11')
declare @p7 int
set @p7=0
exec UpdateInstance @id='1B9F9AA9-4F5C-46F1-97ED-C520716CE7AE', @instance=default, @instanceXml=@p3,@unlockInstance=1,@hostId='97CE292D-D465-49B9-9200-4A55BCE2D7B3',@lockTimeout=60,@result=@p7 output
select @p7

GetInstanceId Metodu Çağrısında
LoadInstance SP'si için çağrı

declare @p5 int
set @p5=0
exec LoadInstance @id='1B9F9AA9-4F5C-46F1-97ED-C520716CE7AE', @lockInstance=1, @hostId='97CE292D-D465-49B9-9200-4A55BCE2D7B3',@lockTimeout=60,@result=@p5 output
select @p5

UpdateInstance SP'si için çağrı

declare @p3 xml
set @p3=convert (xml,N'11')
declare @p7 int
set @p7=0
exec UpdateInstance @id='1B9F9AA9-4F5C-46F1-97ED-C520716CE7AE', @instance=default, @instanceXml=@p3,@unlockInstance=1,@hostId='97CE292D-D465-49B9-9200-4A55BCE2D7B3',@lockTimeout=60,@result=@p7 output
select @p7

Stop Metodu Çağrısında
LoadInstance SP'si için çağrı

declare @p5 int
set @p5=0
exec LoadInstance @id='1B9F9AA9-4F5C-46F1-97ED-C520716CE7AE', @lockInstance=1, @hostId='97CE292D-D465-49B9-9200-4A55BCE2D7B3', @lockTimeout=60,@result=@p5 output
select @p5

DeleteInstance SP'si için çağrı

declare @p4 int
set @p4=0
exec DeleteInstance @id='1B9F9AA9-4F5C-46F1-97ED-C520716CE7AE', @hostId='97CE292D-D465-49B9-9200-4A55BCE2D7B3',@lockTimeout=60,@result=@p4 output
select @p4

Biraz karmaşık ve gereksiz gibi görülebilir ama bu sorgular içerisinde çağırılan saklı yordamlara ve atamalara dikkat etmenizi, ayrıca incelemenizi şiddetle tavsiye ederim. İlk testimiz başarılı bir şekilde çalıştı. Depolama alanına WCF servis örneğimizin başarılı bir şekilde eklendiği, operasyon çağrıları sırasında güncellendiğini ve son olarakta silindiğini gördük. Ancak başka vakalarda söz konusudur. Örneğin istemci uygulama süreci başlattıktan sonra herhangibir sebeple sonlanırsa, servis tarafında başlattığı instance'ın içeriğine tekrar nasıl ulaşabilir?

Bir başka deyişle, daha önceden başlattığı servisin verilerine nasıl ulaşabilir? Diğer taraftan, servis tarafında SQL tabanlı bir depolama alanı kullanılmaktadır. Peki ya özel bir depolama yapmak istersek. Yani SQL dışından bir ortam kullanmak istersek. Söz gelimi dosya tabanlı bir sistem kullanılabilir mi? Yada örneğin bir Access tablosu bu iş için göz önüne alınabilir mi? Bu durumda nasıl ayarlamalar yapılması gerekmektedir? Kalıcı servislerde transaction'lar söz konusu olabilir mi, eğer olursa süreçler nasıl kontrol altına alınabilir? İşte bu ve benzer sorularımız cevabını ilerleyen makalelerimizde bulmaya çalışıyor olacağız. Şimdilik hevesimizi burada dayanıklı olarak saklı tutuyor ve bir sonraki makalemizde görüşmek üzere diyoruz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Örneği İndirmek İçin Tıklayın](/assets/files/2009/DurableService.rar)