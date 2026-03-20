---
layout: post
title: "WF 4.0 - WCF Servislerini Kullanmak"
date: 2009-04-01 12:00:00 +0300
categories:
  - wf-4-0-beta-1
tags:
  - wf-4-0-beta-1
  - csharp
  - xml
  - dotnet
  - wcf
  - workflow-foundation
  - xaml
  - http
  - iis
  - authentication
  - transactions
  - serialization
  - debugging
  - visual-studio
---
Bir [önceki](https://www.buraksenyurt.com/post/Windows-Workflow-Foundation-4-0-Ilk-Izlenimler-bsenyurt-com-dan.aspx) yazımızda Windows Workflow Foundation 4.0 (WF 4.0) ile birlikte gelmesi muhtemel (yüksek bir olasılıkla çok az değişikle gelecekler) kavramları incelemeye çalışmıştık ve pek çok yeni aktivite tipinin alt yapıya dahil edilmiş olduğunu gördük. WF örnekleri bilindiği üzere çoğu zaman servisler ile haberleşmek durumundadır. Bu özellikle gerçek hayat senaryolarında çok sık karşılaşına ve ihtiyaç duyulan bir durumdur. Nitekim WF içerisinde yer alan akışların dış ortamlara olan bir bağımlılığı söz konusu olabilir. Bir Bankacılık sisteminde yer alan akışlarda, servisler yardımıyla ulaşılabilen bazı operasyonlar bu bağımlılığa örnek gösterilebilir örneğin.

WF alt yapısı bu anlamda WCF (Windows Communication Foundation) servisleri ile haberleşilebilmesini kolaylaştırmak amacıyla.Net Framework 3.5 ile birlikte yeni aktivite bileşenlerine sahip olmuştur. SendActivity ve ReceiveActivity isimli bu tipler temel olarak servislere ait operayonların çağırılması veya WF içerisinde servis bazlı operasyonların dış dünyaya sunulmasında etkin olarak kullanılmaktadır. Ancak WF 4.0 ile birlikte servisler ile olan iletişimde daha yetenekli aktivite tipleri yer almaktadır. Özellikle görsel açıdan geliştiriciye kolaylıklar sağlayan ama asıl etkisini XAML bazlı servis tanımlamalarının yapılabilmesinde gösteren aktiviteler söz konusudur. Zaten WF ve WCF 4.0 içerisinde XAML tabanlı deklerafit tanımlamaların son derece etkin bir şekilde kullanıldığı bir gerçektir. WF 4.0 açısında bakıldığında bir WCF operasyonu ile sağlanan istek/cevap (Request/Response) odaklı iletişim temel olarak aşağıdaki şekilde görüldüğü gibidir.

![mk272_1.gif](/assets/images/2009/mk272_1.gif)

Buna göre WF içerisinde herhangibir noktada servis operasyonlarını çağırmak için ClientOperation isimli aktivite kullanılmaktadır. Bu operasyonun yer aldığı WCF servisi bir veya daha fazla EndPoint üzerinden istemcilere hizmet verebilir. WF örnekleri kendi içerisinde, bu EndPoint'in belirttiği Address, Binding ve Contract tipine göre uygun bir mesajlaşma trafiği başlatabilir. Buna göre servis operasyonları çağırabilir ve sonuçlarını ele alabilir. ClientOperation, Workflow Foundation 4.0 ile birlikte gelen yeni aktivite tiplerinden birisidir. Peki bu yeni tipin.Net Framework 3.5 sürümü ile gelen ve aynı amaçla kullanılan SendActivity tipine göre farklılıkları, özellikle avantakları neler olabilir? İşte bu yazımızda kısaca bu sorulara cevap bulmaya çalışacak ve aynı zamanda bizleri bekleyen yenilikleri göreceğiz. Bu nedenle bir örnek üzerinden adım adım ilerleyerek devam etememizde yarar olacağı kanısındayım.

Senaryomuza göre basit ve herzamanki gibi gerçek hayatta kullanılmayacak bir Sequential Workflow uygulaması geliştireceğiz. Akışımız yine geliştirici tarafından tasarlanmış özel bir aktivite tipini kullanarak istemciden iki sayısal değer alacaktır. Bu değerler bir servise gönderilerek işlenecek ve sonuçlar yine akış içerisine yönlendirilerek diğer bir özel aktivite tipi yardımıyla ekrana yazdırılacaktır. İlk olarak servis uygulamasının tasarlanmasında yarar vardır.

> Yazımızda geliştirmekte olduğumuz örnek henüz Relase olmamış.Net Framework 4.0 sürümü üzerinde geliştirilmekte ve bu amaçla Visual Studio 2010' un PDC 2008' de yayımlanan Virtual PC versiyonu kullanılmaktadır. Dolayısıyla yazılan ve işlenen kavramlarda veya Visual Studio 2010 sürümünde köklü değişiklikler olabilir, olması muhtemeldir.

CalculusService isimli WCF uygulamamız WCF Service Application tipinde geliştirilmektedir.

![mk272_2.gif](/assets/images/2009/mk272_2.gif)

Serviste kullanılan sözleşme içeriği ise aşağıdaki kod parçasında görüldüğü gibidir.

```csharp
using System.ServiceModel;

namespace CalculusService
{
    [ServiceContract]
    public interface IMatService
    {
        [OperationContract]
        double Sum(double x, double y);
    }
}
```

Servis sözleşmesinde (Service Contract), çok basit olarak double tipinden iki sayısal değerin toplamını alarak geriye sonucunu döndüren Sum isimli bir operasyon yer almaktadır. IMatService isimli servis arayüz tipini (Interface) uygulayan sınıfa ait kod içeriği ise aşağıdaki gibidir.

```csharp
namespace CalculusService
{
    public class MatService 
        : IMatService
    {
        public double Sum(double x, double y)
        {
            return x + y;
        }
    }
}
```

Servis üzerinde tanımlanmış olan operasyonlar basit HTTP protokolüne göre sunulmaktadır. Bu nedenle bağlayıcı tip olarak BasicHttpBinding kullanılmaktadır. Servis için birde HTTP üzerinden metadata bilgisinin verilebilmesi amacıyla MexHttpBinding bazlı bir EndPoint daha söz konusudur.(Gerçi biraz sonra görebileceğimiz gibi, WF örneğinin servise ait bir Metadata indirmesine gerek kalmayacaktır:)) Örnek WCF servis uygulamamıza EndPoint ayarları aşağıdaki Web.config dosyasının içeriğinde olduğu gibi tanımlanabilir.

```xml
<?xml version="1.0"?>
<configuration>
    <appSettings/>
    <connectionStrings/>
    <system.web>
        <compilation debug="true"></compilation>
        <authentication mode="Windows"/>
    </system.web>
    <system.serviceModel>
        <services>
            <service behaviorConfiguration="CalculusService.MatServiceBehavior" name="CalculusService.MatService">
                <endpoint address="" binding="basicHttpBinding" bindingConfiguration="" contract="CalculusService.IMatService">
                    <identity>
                        <dns value="localhost" />
                    </identity>
                </endpoint>
                <endpoint address="mex" binding="mexHttpBinding" contract="IMetadataExchange" />
            </service>
        </services>
        <behaviors>
            <serviceBehaviors>
                <behavior name="CalculusService.MatServiceBehavior">
                    <serviceMetadata httpGetEnabled="true" />
                    <serviceDebug includeExceptionDetailInFaults="true" />
                </behavior>
            </serviceBehaviors>
        </behaviors>
    </system.serviceModel>
</configuration>
```

WCF servis uygulamamız test amacıyla geliştirildiğinden IIS (Internet Information Services) üzerine atılmamıştır. Bu nedenle Solution içerisinde yer alan istemci uygulamalar ile haberleşmesi sırasındaki geliştirme sürecini kolaylaştırmak adına, sabit bir HTTP portu kullanması sağlanabilir. Sabit port ayarlaması için WCF projesinin özelliklerinden (Properties) aşağıdaki ekran görüntüsünde olduğu gibi Specific Port özelliğine bir değer atamak yeterli olacaktır. (Elbetteki bu WCF uygulamasını, IIS altına atmak kolay bir şekilde özellikler penceresinde yer alan Use Local IIS Web server seçeneği ile mümkün olabilir. Yada bu amaçla Publish işlemlerinden yararlanılabilir.)

![mk272_4.gif](/assets/images/2009/mk272_4.gif)

Bu aşamada ilerlemeden önce servisin HTTP üzerinden çağırılabildiğinden emin olmak gerekir. Bu amaçla, MatService.svc dosyasının bir tarayıcı içerisinde açılması yeterlidir. Eğer aşağıdaki ekran görüntüsünde yer alan sonuçlar elde edilebiliyorsa WCF servisinin çağırılabilir olduğu sonucuna varılabilir.

![mk272_3.gif](/assets/images/2009/mk272_3.gif)

Gelelim WF tarafına. Her zamanki gibi basit bir Sequential Workflow Console Application üzerinden ilerliyor olacağız. Servise gönderilecek parametreleri ekrandan almak ve yazdırmak içinse bir önceki yazı dizimizdekilere benzer iki basit aktivite tipi kullanıyor olacağız. Console ekranından bilgi okumak için kullanılan Read aktivitesine ait kod içeriği aşağıdaki gibi geliştirilebilir.

```csharp
using System;
using System.WorkflowModel;

namespace CalculusWF
{
    public class Read
        :WorkflowElement
    {
        public OutArgument<double> Value1 { get; set; }
        public OutArgument<double> Value2 { get; set; }

        protected override void Execute(ActivityExecutionContext context)
        {
            Console.WriteLine("Value 1 ?");
            Value1.Set(context, Convert.ToDouble(Console.ReadLine()));
            Console.WriteLine("Value 2 ?");
            Value2.Set(context, Convert.ToDouble(Console.ReadLine()));
        }
    }
}
```

Read aktivitesi görüldüğü üzere iki adet OutArgument tipinden özellik kullanmakta ve override ettiği Execute metodu içerisinde ekrandan aldığı değerleri dış ortama sunmaktadır. Yine dikkat edilmesi gereken noktalardan birisi, aktivitenin WF Base Library içerisindeki yeni ata sınıf olan WorkflowElement tipinden türemiş olmasıdır. Write aktiviteside Read aktivitesi gibi WorkflowElement türevlidir ve WF içeriğinden (ActivityExecutionContext yardımıyla) gelen sonuç değerini ekrana yazdırmak için kullanılmaktadır. Write aktivitesine ait kod içeriği aşağıda görüldüğü gibidir.

```csharp
using System;
using System.WorkflowModel;

namespace CalculusWF
{
    class Write
        :WorkflowElement
    {
        public InArgument<double> Result { get; set; }

        protected override void Execute(ActivityExecutionContext context)
        {
            double r=Result.Get(context);
            Console.WriteLine("İşlem sonucu {0} dır",r.ToString());
        }
    }
}
```

Workflow içerisinde toplama işlemi için kullanılacak değerler ile işlem sonucuna tüm aktivite boyunca ulaşılması istendiğinden aşağıdaki ekran görüntüsünde yer aldığı gibi üç adet Variable tanımlaması yapılmaktadır. Bu tanımlamalar Matflow.xaml isimli Sequential Activity tipinin tamamı için geçerlidir.

![mk272_6.gif](/assets/images/2009/mk272_6.gif)

A, B ve Total isimli değişkenler double tipinden tanımlanmıştır. Bu değerler Read, Write aktiviteleri ile ClientOperation tarafından kullanılabilecektir. Bu işlemin ardından artık aktivite dizisinin oluşturulmasına başlanabilir. İlk olarak Read aktivitesi sürüklenir. Söz konusu aktivitenin özellikleri aşağıdaki gibi ayarlanabilir.

![mk272_7.gif](/assets/images/2009/mk272_7.gif)

Dikkat edileceği üzere Read aktivitesi içerisinde tanımlanmış olan Value1 ve Value2 isimli özelliklere, Sequence aktivitesi içerisinde tanımlanan A ve B değişkenleri atanmıştır. Bir başka deyişle Read aktivitesi ile komut satırından okunup set edilen Value1 ve Value2 değerleri diğer aktiviteler tarafından kolayca ele alınabileceklerdir. Nitekim A ve B isimli global değişkenleri taşınmaktadırlar. Read ve Write aktiviteleri arasına ClientOperation aktivitesini eklemeden önce, Write aktivitesi içinde aşağıda görülen özellik ayarlamalarını yapmamız yeterli olacaktır.

![mk272_8.gif](/assets/images/2009/mk272_8.gif)

Dikkat edileceği üzere Write aktivitesi içerisinde tanımlanmış olan Result isimli özelliğe global değişkenlerden Total atanmıştır. Artık Write aktivitesi ile okunarak global seviyedeki A ve B değişkenlerine atanan değerleri, kullanılmak üzere ele alacak ve toplam sonucunu Total isimli değişkene verecek olan ClientOperation aktivitesini geliştirmeye başlayabiliriz.

![mk272_5.gif](/assets/images/2009/mk272_5.gif)

ClientOperation aktivitesini Read ve Write aktiviteleri arasına sürükleyip bıraktığımızda ilk etapta aşağıdaki ekran görüntüsü ile karşılaşırız. Dikkat edileceği üzere Operation Contract, Binding ve EndPoint Address isimli 3 önemli özellik göze çarpmaktadır.

![mk272_9.gif](/assets/images/2009/mk272_9.gif)

Tahmin edileceği üzere bu özelliklerin değerleri ile istemci için gerekli bir EndPoint bilgisi oluşturulabilir. Bir başka deyişle bir EndPoint tanımını oluşturan adresleme (Address), bağlayıcı tip (Binding Type) ve sözleşme (Contract) bilgilerinin tamamı bu aktivite tipi içerisinde belirlenmektedir. İlk olarak aşağıdaki ekran görüntüsünde yer alan adımlar takip edilerekten servis sözleşmesinin (Service Contract) adı girilir.

![mk272_10.gif](/assets/images/2009/mk272_10.gif)

Visual Studio 2010 ürün olarak sunulduğunda servis sözleşmesi gibi kısımların elle değil otomatik olarak girilebilecek şekilde ayarlanabileceğini düşünmekteyim. Şimdilik servis tarafındaki sözleşme arayüzü tipinin adını elle (büyük küçük harf duyarlılığına da dikkat ederekten) yazmamız gerekmektedir. Servis sözleşmesinin belirlenmesi tek başına yeterli değildir. ClientOperation aktivitesinin bu servis operasyonu üzerinde hangi operasyonu çağıracağınında belirlenmesi gerekmektedir. Burada New ServiceContract düğmesi yardımıyla sözleşme tanımlandığında, başlığın New OperationContract olarak değiştiği gözlemlenir. Tahmin edileceği gibi New OperationContract düğmesi ve takip eden adımlar ile Sum operasyonuna ait tanımlamalar görsel olarak yapılabilmektedir. Aynen aşağıdaki ekran görüntüsünde yer aldığı gibi.

![mk272_11.gif](/assets/images/2009/mk272_11.gif)

Burada çok detaylı operasyon ayarlamaları yapılabilmektedir. Güvenlik ile ilişkili işlemler (Protection Level), Transaction aktarma opsiyonları, operasyonun tek yönlü (One-Way) olup olmadığı, operasyondan dönebilecek Fault tipleri vb... Geliştirdiğimiz örnekte sadece operasyon parametrelerinin girilmesi yeterlidir. x ve y isimli argümanlar, Sum operasyonuna ait girdi parametreleri olduklarından Direction özellikleri In olarak belirlenmiştir. Diğer taraftan SumResult isimli argüman, operasyonun dönüş değerini işaret etmekte olduğundan, Direction özelliğine Out değeri verilmiştir. Şu anda çağırılacak servis operasyonu ile ilişkili tanımlamalar yapılmıştır. Artık bu servis ile hangi bağlayıcı tip ve adres ile iletişime geçileceğine dair özelliklerin belirlenmesi gerekmektedir. Bu amaçla aşağıdaki ekran görüntüsünde yer alan atamaların yapılması yeterlidir.

![mk272_12.gif](/assets/images/2009/mk272_12.gif)

Unutulmaması gereken noktalardan biriside WCF servisleri ile haberleşecek olan istemci uygulamaların, servis tarafında belirtilen EndPoint içeriğine uygun EndPoint bildirimlerine sahip olması zorunluluğudur. Geliştirilen örnekte servis tarafında BasicHttpBinding tipi kullanıldığından istemci tarafındaki EndPoint içinde aynı tipte bir bağlayıcının kullanılması gerekmektedir. Benzer olarak servis tarafındaki operasyon HTTP bazlı olaraktan localhost isimli makineden ve 49100 nolu port üzerinden sunulmaktadır. Bu nedenle istemci tarafından bu kritere uygun bir adres için talepte bulunulmalıdır. Dikkat çekici noktalardan biriside adres kısmında C# notasyonuna pek uymayan bir şekilde büyük harfle başlayan bir New anahtar kelimesi olmasıdır. (WF 4.0 ile ilişkili yenilikleri öğrendiğim Microsoft LAB dökümanlarında bu durumun final sürümünde değişeceğine dair bir bilgi yer almaktadır. Tabi final sürümünde bizleri neler bekliyor neler...) Artık istemci tarafındaki EndPoint için gerekli ABC (AddressBindingContract) bilgileri tanımlanmış durumdadır.

Yapılması gereken önemli işlemlerden biriside WF içerisindeki değişkenleri ClientOperation ile servise aktarmak ve servisten dönen değeride tekrardan WF içerisine yönlendirmektir. Bunun için ClientOperation elementinin özelliklerinde aşağıdaki ekran görüntüsünde yer alan ayarlamaların yapılması yeterli olacaktır.

![mk272_13.gif](/assets/images/2009/mk272_13.gif)

Görüldüğü üzere SumResult isimli servis operasyon argümanı Total isimli WF değişkenine, x isimli servis operasyon argümanı A isimli WF değişkenine ve son olarak y isimli servis operasyonu argümanı B isimli WF değişkenine set edilmiştir. İşte bu kadar. Görüldüğü üzere görsel olarak pek çok ayarlama yapılmıştır. Sonuç olarak MatFlow.xaml adlı Workflow örneğimiz için aşağıdaki XAML (eXtensible Application Markup Language) içeriği üretilmektedir.

```xml
<p:Activity x:Class="CalculusWF.MatFlow" xmlns:c="clr-namespace:CalculusWF;assembly=CalculusWF" xmlns:p="http://schemas.microsoft.com/netfx/2009/xaml/workflowmodel" xmlns:p1="http://schemas.microsoft.com/netfx/2008/xaml/schema" xmlns:p2="http://schemas.microsoft.com/netfx/2009/xaml/servicemodel" xmlns:s="clr-namespace:System;assembly=System" xmlns:swd="clr-namespace:System.WorkflowModel.Debugger;assembly=System.WorkflowModel" xmlns:swdx="clr-namespace:System.WorkflowModel.Design.Xaml;assembly=System.WorkflowModel.Design" xmlns:sx="clr-namespace:System.Xml;assembly=System.Runtime.Serialization" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <p:Sequence swd:XamlDebuggerXmlReader.FileName="C:\Orneklerim\CalculusService\CalculusWF\MatFlow.xaml">
        <p:Sequence.Variables>
            <p:Variable x:TypeArguments="p1:Double" Name="A" />
            <p:Variable x:TypeArguments="p1:Double" Name="Total" />
            <p:Variable x:TypeArguments="p1:Double" Name="B" />
        </p:Sequence.Variables>
        <c:Read DisplayName="Read Parameters" Value1="[A]" Value2="[B]" />
        <p2:ClientOperation EndpointAddress="[New s:Uri("http://localhost:49100/MatService.svc")]" OperationName="Sum">
            <p2:ClientOperation.Endpoint>
                <p2:Endpoint Name="ClientOperationEndpoint">
                    <p2:Endpoint.Binding>
                        <p2:BasicHttpBinding TextEncoding="utf-8">
                            <p2:BasicHttpBinding.ReaderQuotas>
                                <sx:XmlDictionaryReaderQuotas />
                            </p2:BasicHttpBinding.ReaderQuotas>
                            <p2:BasicHttpBinding.Security>
                                <p2:BasicHttpSecurity Mode="None">
                                    <p2:BasicHttpSecurity.Message>
                                        <p2:BasicHttpMessageSecurity AlgorithmSuite="Default" ClientCredentialType="UserName" />
                                    </p2:BasicHttpSecurity.Message>
                                    <p2:BasicHttpSecurity.Transport>
                                        <p2:HttpTransportSecurity />
                                    </p2:BasicHttpSecurity.Transport>
                            </p2:BasicHttpSecurity>
                        </p2:BasicHttpBinding.Security>
                    </p2:BasicHttpBinding>
                </p2:Endpoint.Binding>
                <p2:Endpoint.ContractProjection>
                    <p2:SoapContractProjection>
                        <p2:SoapContractProjection.Contract>
                            <p2:ServiceContract Name="IMatService">
                                <p2:OperationContract Name="Sum">
                                    <p2:OperationArgument Name="x" Type="p1:Double" />
                                    <p2:OperationArgument Name="y" Type="p1:Double" />
                                    <p2:OperationArgument Direction="Out" Name="SumResult" Type="p1:Double" />
                                </p2:OperationContract>
                        </p2:ServiceContract>
                    </p2:SoapContractProjection.Contract>
                </p2:SoapContractProjection>
            </p2:Endpoint.ContractProjection>
        </p2:Endpoint>
    </p2:ClientOperation.Endpoint>
        <p2:ClientOperation.OperationArguments>
            <p:OutArgument x:Key="SumResult" x:TypeArguments="p1:Double">[Total]</p:OutArgument>
            <p:InArgument x:Key="x" x:TypeArguments="p1:Double">[A]</p:InArgument>
            <p:InArgument x:Key="y" x:TypeArguments="p1:Double">[B]</p:InArgument>
        </p2:ClientOperation.OperationArguments>
    </p2:ClientOperation>
    <c:Write DisplayName="Write Result" Result="[Total]" />
  </p:Sequence>
</p:Activity>
```

Burada durup aslında biraz soluklanmak belki bir yudum kahve içmek ve sonrasında XAML içeriğine yoğunlaşarak düşünmek gerekmektedir. Dikkat edileceği üzere WF'in tüm içeriği, Write, Read, ClientOperation aktiviteleri, global WF değişkenleri bu XAML içeriğinde bildirilmektedir. Tabiki bu XAML içeriği çalışma zamanı tarafında değerlendirilmektedir. Dekleratif (Declerative) bir yaklaşımdan ziyade ClientOperation aktivitesinin XAML içerisine nasıl gömüldüğüne dikkat etmemizde yarar vardır. Öyleki EndPoint ve buna ait AddressBindingContract bilgilerinin tamamı XAML elementleri içerisinde oluşturulmuştur.

Buna göre XAML içeriği basit bir editor yardımıyla değiştirildiği takdirde Workflow örneğinin yeni ortam şartlarına göre adapte edilmesi kolayca sağlanabilir. Söz gelimi, servis adresinin değişmesi veya operasyon adında yada parametrik yapısında oluşabilecek değişiklikleri koda girmeden düzenleyebilir ve WF'in güncellenmesini sağlayabiliriz. Bunun sağlanmasının en büyük etkenlerinden birisi Workflow bazlı WCF servislerinin kod yazmadan XAML tabanlı geliştirilip kullanılabiliyor olmasıdır. (Bu alt yapıyı ve Workflow bazlı bir WCF servisinin XAML ile dekleratif olarak nasıl tanımlanabileceğini ilerleyen makalelerimizde veya görsel derslerimizde incelemeye çalışacağız.) Artık örneğimizi test etmeye başlayabiliriz. Bundan önce Solution'ımızın son halinin aşağıdaki ekran görüntüsüne benzer olacağını düşünebiliriz.

![mk272_15.gif](/assets/images/2009/mk272_15.gif)

Programın çalışmasının sonucu aşağıdakine benzer bir ekran çıktısı oluşacaktır.

![mk272_14.gif](/assets/images/2009/mk272_14.gif)

Dikkat edilmesi ve unutulmaması gereken noktalardan biriside programın çalışması için servisinde çalışıyor olması gerekliliğidir. Nitekim servisin ayakta olmaması halinde istemcilerin taleplerine karşılık çalışma zamanı istisnaları (Runtime Exception) alması söz konusudur.

Peki ClientOperation kullandığımızda.Net 3.5 sürümü ile Workflow Foundation alt yapısına kazandırılan SendActivity tipine göre en büyük farklılık (farklılıklar) nedir?

> SendActivity tarafından kullanılan WCF servisinin Host uygulama üzerinde ele alınabilmesi için Add Service Reference (veya svcutil ile komut satırından) ile proxy üretiminin yapılması gerekmektedir.
> ![mk249_3.gif](/assets/images/2009/mk249_3.gif)

Herşeyden önce servis kullanan bir istemci geliştirilirken, Add Service Reference seçeneği ile proxy tiplerinin eklenmesi gerekmektedir. Oysaki geliştirdiğimiz örnekte böyle bir işleme başvurulmamıştır. Bunun yerine ClientOperation elementi için XAML tabanlı tanımlamalar yapılmıştır. Dolayısıyla yeni çalışma zamanı motorunun bu içeriğe bakaraktan servis ile iletişime geçtiğini ve fiziki bir proxy'ye ihtiyaç duymadığını söyleyebiliriz. Bir başka deyişle örneğin servis tarafında oluşabilecek bazı değişikliklerin WF tarafına bildirilmesi için bir referans güncellemesi yapılmasına gerek kalmamaktadır. Buda önemli bir avantajdır.

Böylece geldik bir makalemizin daha sonuna. Bu makalemizde yeni WF 4.0 ve WCF 4.0 kabiliyetlerinin örnek bir sonucunu değerlendirmeye çalıştık. Bu amaçla yeni gelen aktivite tiplerinden ClientOperation tipini ele aldık ve bir servis operasyonunun referansını eklemeden, dekleratif olarak tanımlanıp XAML ile oluşturulmasını ve kullanılmasını gördük. Tabi bu konuda konuşulabilecek farklı vakalarda vardır. En önemli sorunlardan biriside servis operasyonlarının uzun süreli (Long Running Workflows) olabileceğidir. Bu durumda WF'in kalıcı olarak saklanması (Persistence) gibi durumlar söz konusudur ki bunu WF 4.0 üzerinde kurmak ve yönetmek (Management) son derece kolaydır. Özellikle yönetim ve izleme safhasında devreye giren Dublin kod adlı Windows Application Server'ın yeteneklerini en kısa sürede sizinle paylaşmaya çalışıyor olacağım. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.