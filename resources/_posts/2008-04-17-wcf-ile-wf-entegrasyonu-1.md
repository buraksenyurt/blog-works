---
layout: post
title: "WCF ile WF Entegrasyonu - 1"
date: 2008-04-17 12:00:00 +0300
categories:
  - wcf
  - wf
tags:
  - windows-communication-foundation
  - workflow-foundation
  - windows-workflow-foundation
---
Bilindiği üzere Window Communication Foundation ve Windows Workflow Foundation,.Net Framework 3.0 ile birlikte gelen önemli teknolojilerdendir. WCF servis yönelimli mimariye (Service Oriented Architecture) yeni bir yaklaşım getirip, dağıtık mimari uygulama geliştirme kavramlarını bir çatı altında toplayarak güçlü, daha platform bağımsız ve güvenilir bir ortamda geliştirme yapılabilmesini olanaklı kılan bir alt yapı sunmaktadır. WF ise, birden fazla adımdan oluşan kod süreçlerinin iş akışı (Workflow) tarzında olay güdümlü (Event-Driven) yada sırasal (Sequential) olacak şekilde tasarlanarak çeşitli.Net uygulamalarında kullanılabilmelerini mümkün kılan bir alt yapı tesit etmektedir.

Workflow, süreçlere ait akışların hazırlanmasında ağırlıklı olarak aktivite tiplerini (Activity Types) kullanmaktadır. Bu aktivitelerin dallara ayrılması, çatallanması, birbirlerine katılması gibi pek çok işlem de, Workflow ortamı tarafından sunulmaktadır. WF ile geliştirilen iş akışları kısa süreceği gibi uzunda (Long-Running Workflows) sürebilir. Bu nedenle WF ortamı özellikle uzun süren akışların, sistem yeniden başlatmaları (reboot) gibi vakalara karşı ayakta durabilmesi için kalıcı olarak saklama işlemlerine destek de vermektedir. WF mimarisinin yetenekleri sadece bununlada sınırlı değildir. Örneğin Transaction desteği diğer yetenekleri arasında gösterilebilir.

> Windows Workflow Foundation ile Windows Communication Foundation arasındaki entegrasyon gerçek anlamda.Net Framework 3.5 ve Visual Studio 2008 ile birlikte sağlanmıştır.

WF aslında,.Net Framework 3.5 ve Visual Studio 2008 ile gelen yenilikler sayesinde WCF ortamının gerçek anlamda bir tamamlayıcısı teknoloji olarak görülmektedir. Bu anlamda WF uygulamaları içerisinden WCF servislerinin çağırılması ve kod akışının ilgili servis noktaları (Service EndPoint) üzerinden yürütülmesi sağlanabilmektedir. Tam tersine WF içerisindende servis yayınlaması yapılabilmektedir. Bu entegrasyon sayesinde süreçlerin istemcilere güçlü (Robust), güvenilir (Reliable) ve güvenli (Secure) bir şekilde sunulmasıda sağlanmaktadır.

Öyleki bu entegrasyonun doğal sonucu olarak iş mantığının (Logic) farklı formatlarda (MTOM, SOAP, Binary, JSON, X509 vb...) dolaşımı, IIS (Internet Information Services), WAS (Windows Activation Service) ve Windows Service gibi ortamlar üzerinden host edilme imkanı gibi pek çok fonksiyonellik ele alınabilmektedir.

WCF ile WF entegrasyonu sırasında servis ile olan etkileşimin modellenebilmesi için Send, Receive gibi aktivitelerden yararlanılmaktadır. Send aktivitesi sayesinde WF içerisinden bir WCF servisine mesaj gönderilmesi sağlanabilir ki bu noktada Proxy nesneleride devreye girmektedir. Diğer taraftan Receive aktivitesi sayesinde workflow'un kendisinin bir servis gibi sunulabilmesi olanaklı hale gelmektedir.

> SendActivity ve ReceiveActivity tipleri.Net Framework 3.5 ile birlikte gelmiştir.
> ![mk249_5.gif](/assets/images/2008/mk249_5.gif)

Aktivasyon alt yapısının host edilmesi içinse ServiceHostBase abstract sınıfından türeyen ve.Net Framework 3.5 ile birlikte gelen WorkflowServiceHost tipinden yararlanılmaktadır. Servis ve istemci arasındaki önemli konulardan biriside korelasyonun sağlanmasıdır. Bu bir anlamda istemcinin doğru servis örneği ile iletişime geçebilmesi demektir ki bunun sağlanabilmesi için eklenmiş olan yeni davranış (Behavior) ve bağlayıcılar (Bindings) söz konusudur.

Bu teorik bilgilerden sonra örnekler ile devam edelim. İlk olarak bir WF uygulaması içerisinde bir WCF servisinin nasıl çağırılabileceğini incelemeye çalışacağız. WF içerisinden bir WCF servis noktasına ulaşmak için SendActivity, InvokeWebServiceActivity, CodeActivity aktivite tiplerinden yararlanılabilir. Bunlardan en güçlü olanı SendActivity dir. InvokeWebServiceActivity Web servislerinin proxy sınıfı aracılığıyla çağırılmasını sağlamaktadır. WCF tarafından asmx modeline uygun yayınlama yapılabildiğinden bu aktivite tipide tercih edilebilir.

Nevarki SendActivity tipine göre herhangibir üstünlüğü bulunmamaktadır. SendActivity WCF servisi ile senkron (synchronous) olarak haberleşilmesini sağlar ve tek yönlü (One-Way), talep-cevap (Request-Response), talep-hata (Request-Fault) desenlerini ele alır. Tek yönlü desene göre servise talepte bulunulduktan sonra bir cevap beklenmez. Talep-Cevap desenine göre ise servisten yapılan isteğe bir sonuç gelinceye kadar beklenir (Synchronous). Son desene göre ise ya cevap gelir yada hata mesajı (Fault Message). Bunların dışında özel aktiviteler (Custom Activity) yazılarak iş mantığının söz konusu aktivite tip içerisine gömülmesi ve servisin ele alınmasıda sağlanabilir.

Elbette ilk olarak bir servis kütüphanesinin (WCF Service Library) ve host uygulamanın tasarlanması gerekmektedir. Örnekte kullanılacak olan servis kütüphanesinin içeriği aşağıdaki gibidir.

![mk249_1.gif](/assets/images/2008/mk249_1.gif)

Product sınıfı;

```csharp
[DataContract]
public class Product
{
    [DataMember]
    public int ProductId { get; set; }
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public double ListPrice { get; set; }
}
```

Product sınıfı Production.Product tablosundaki herhangibir satıra ait ProductID,Name ve ListPrice bilgilerini taşımak üzere tasarlanmış ve bu sebepten bir veri sözleşmesi (DataContract) olacak şekilde tanımlanmıştır.

IProductManager arayüzü (Interface);

```csharp
[ServiceContract(Name="Urun Servisi",Namespace="http://www.bsenyurt.com/UrunServisi")]
public interface IProductManager
{
    [OperationContract]
    Product GetProduct(int productId);
}
```

Servis sözleşmesi (Service Contract) Product tipinden nesne örnekleri döndüren tek bir operasyon tanımı içermektedir.

ProductManager sınıfı;

```csharp
public class ProductManager
            : IProductManager
{
    #region IProductManager Members

    public Product GetProduct(int productId)
    {
        Product prd = null;
        using (SqlConnection conn = new SqlConnection("data source=.;database=AdventureWorks;integrated security=SSPI"))
        {
            SqlCommand cmd = new SqlCommand("Select ProductId,Name,ListPrice From Production.Product Where ProductId=@PrdId", conn);
            cmd.Parameters.AddWithValue("@PrdId", productId);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
                prd = new Product { ProductId = Convert.ToInt32(reader["ProductId"]), Name = reader["Name"].ToString(), ListPrice = Convert.ToDouble(reader["ListPrice"]) };
            reader.Close();
        }
        return prd;
    }

    #endregion
}
```

ProductManager sınıfında yazılan GetProduct metodu, parametre olarak gelen değere göre Production.Product tablosundan bir satır verisini çekmektedir. Eğer parametre olarak gelen id değerine bağlı bir satır varsa ProductId,Name,ListPrice değerlerinin toplandığı Product nesne örneği geri döndürülmektedir. Sınıf kütüphanesinin tanımlanmasından sonra host servis uygulamasının yazılması gerekmektedir. Örnekte Host, basit bir Console uygulaması olacak şekilde aşağıdaki gibi tasarlanmıştır.

Program sınıfı içeriği;

```csharp
using System;
using System.ServiceModel;
using ProductServices;

namespace Sunucu
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(ProductManager));
            host.Open();
            Console.WriteLine("Sunucu dinlemede...");
            Console.ReadLine();
            host.Close();
        }
    }
}
```

App.config içeriği;

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <behaviors>
            <serviceBehaviors>
                <behavior name="UrunServisiBehavior">
                    <serviceMetadata/>
                </behavior>
            </serviceBehaviors>
        </behaviors>
        <services>
            <service behaviorConfiguration="UrunServisiBehavior" name="ProductServices.ProductManager">
                <endpoint address="" binding="netTcpBinding" bindingConfiguration="" name="UrunServisiTcpEndPoint" contract="ProductServices.IProductManager" />
                <endpoint address="mex" binding="mexTcpBinding" name="UrunServisiMexEndPoint" contract="IMetadataExchange" />
                <host>
                    <baseAddresses>
                        <add baseAddress="net.tcp://localhost:9001/UrunServisi" />
                    </baseAddresses>
                </host>
            </service>
        </services>
    </system.serviceModel>
</configuration>
```

İlerlemeden önce App.config dosyası içeriğinin incelenmesinde yarar vardır. Servis uygulaması iki adet EndPoint sunmaktadır. UrunServisiTcpEndPoint isimli servis noktası TCP (netTcpBinding bağlayıcısını kullandığına dikkat edelim) bazlı olacak şekilde bir yayınlama yapmaktadır. address bilgisi verilmemiş olmasına rağmen host elementi içerisinde tanımlanan baseAdress bilgisi kullanılmaktadır. Diğer EndPoint ise IMetadaExchange arayüzünü servis sözleşmesi olarak kullanan ve mexTcpBinding bağlayıcısını baz alarak mex isimli bir adres üzerinden yayınlama yapan bir servis noktası sunmaktadır.

Bu EndPoint sayesinde baseAddress elementi ile bildirilen TCP adresi üzerinden Metadata Exchange işlemi gerçekleştirilebilir. Bir başka deyişle çalışmakta olan servis uygulamasına bağlı adres üzerinden proxy nesnesi üretimi gerçekleştirilebilir. Önemli olan noktalardan biriside proxy üretiminin sağlanmasıdır. Bu nedenle servise metadata yayınlaması için bir servis davranışı (Service Behavior) eklenmiştir. Böylece servis operasyonlar servis dışına sunulabilir hale gelmiştir.

> WF uygulaması, dışarıdan bir WCF servisini çağırmak için proxy nesnesinden yararlanmaktadır. Bu proxy nesnesi Visual Studio 2008 ortamında Add Service Reference ile kolay bir şekilde eklenebileceği gibi svcutil aracı yardımıylada aşağıdaki şekilde olduğu gibi çekilebilirde.
> ![mk249_2.gif](/assets/images/2008/mk249_2.gif)
> Hangisi tercih edilirise edilsin örneğe göre servis uygulamasının çalışır durumda olması gerekmektedir.

Artık WF uygulamasının yazılmasına başlanabilir. Örnekte Sequential Workflow Console Application şablonu kullanılmaktadır. Proje açıldıktan sonra servis uygulamasının çalışıyor olmasına dikkat edereken, Add Service Reference seçeneği ile proxy sınıfının üretilmesi ve WF uygulamasına eklenmesi sağlanabilir.

![mk249_3.gif](/assets/images/2008/mk249_3.gif)

Bu işlemin arkasından WF uygulaması içerisinde aşağıdaki gibi servisa ait bilgilerin indirildiği ve proxy sınıfının (Reference.cs içerisinde yer almaktadır) üretildiği görülebilir.

![mk249_4.gif](/assets/images/2008/mk249_4.gif)

Örnekte WF içerisinden servisi çağırmak için SendActivity tipi kullanılacaktır. İlk olarak Workflow1 tasarım ortamına SendActivity bileşeni sürüklenmelidir.

![mk249_6.gif](/assets/images/2008/mk249_6.gif)

SendActivity bileşeninin önemli olan bazı üyeleri vardır. Bunlardan ServiceOperationInfo özelliği yardımıyla etkileşimde bulunulacak olan servisin ve ilgili operasyonunun seçilmesi sağlanır. AfterResponse alanının işaret ettiği olay sayesinde, servis tarafından cevap geldikten sonra işletilmesi istenen kodların icra edilmesi sağlanmaktadır. Benzer şekilde BeforeSend alanının işaret ettiği olay sayesinde, servise mesaj gönderilmeden önce işletilmesi gereken bir kod mantığı var ise, bunun icra edilmesi sağlanmaktadır. İstenirse özel servis adresleri CustomAdress özelliği ile tanımlanabilir. Örnekte ilk olarak ServiceOperationInfo özelliğinden yararlanılarak kullanılacak operasyon seçilmelidir.

![mk249_7.gif](/assets/images/2008/mk249_7.gif)

Import düğmesine basıldıktan sonra ekrana gelen ara birimde, projeye az önce referans edilmiş olan serviste görülecektir (ServiceReference1). Bu işlemin hemen arkasından UrunServisi tipine çift tıklanırsa aşağıdaki ekran görüntüsü elde edilir.

![mk249_8.gif](/assets/images/2008/mk249_8.gif)

Görüldüğü gibi servise ait GetProduct operasyonu eklenmiştir. Parameters kısmında, operasyonun aldığı ve geri döndürdüğü değişkenlere ait bir takım bilgiler yer almaktadır. Buna operasyon productId isimli Int32 tipinden bir parametre almakta ve Product tipinden bir sonuç döndürmektedir. OK düğmesine basıldıktan sonra Parameters kısmında yer alan değişkenlerin Properties penceresine birer özellik olaraktan eklendiği izlenebilir.

![mk249_9.gif](/assets/images/2008/mk249_9.gif)

Bu özelliklerin yanında yer alan üç nokta düğmelerine basıldığında, ilgili alanların servis operasyonuna bağlanması için gerekli özellik/alan (Property/Field) tanımlamalarının yapılacağı bir ekran ile karşılaşılır. Bu ekrandan yararlanılarak Workflow sınıfı içerisinde yazılmış var olan özelliklere/alanlara bağlama yapılabileceği gibi Bind to a new member sekmesinden faydalanılarak yeni özelliklerin anında oluşturulmasıda sağlanabilir. Örneğin ReturnValue özelliği için aşağıdaki ekran görüntüsünde yer alan seçimler kullanılmış ve anında sendActivity1ReturnValue1 isimli bir üyenin oluşturulması sağlanmıştır.

![mk249_10.gif](/assets/images/2008/mk249_10.gif)

Aynı işlem productId isimli aktivite özelliği içinde yapılmalıdır. Bu işlemlerin ardından Workflow1 sınıfı içerisine DependencyProperty tipinden iki yeni özelliğin aşağıdaki gibi eklendiği görülür.

```csharp
namespace WfdenServis
{
    public sealed partial class Workflow1: SequentialWorkflowActivity
    {
        public Workflow1()
        {
            InitializeComponent();
        }

        public static DependencyProperty sendActivity1__ReturnValue_1Property = DependencyProperty.Register("sendActivity1__ReturnValue_1", typeof(WfdenServis.ServiceReference1.Product), typeof(WfdenServis.Workflow1));

        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(true)]
        [CategoryAttribute("Parameters")]
        public WfdenServis.ServiceReference1.Product sendActivity1__ReturnValue_1
        {
            get
            {
                return ((WfdenServis.ServiceReference1.Product)(base.GetValue(WfdenServis.Workflow1.sendActivity1__ReturnValue_1Property)));
            }
            set
            {
                base.SetValue(WfdenServis.Workflow1.sendActivity1__ReturnValue_1Property, value);
            }
        }

        public static DependencyProperty sendActivity1_productId1Property = DependencyProperty.Register("sendActivity1_productId1", typeof(System.Int32), typeof(WfdenServis.Workflow1));

        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(true)]
        [CategoryAttribute("Parameters")]
        public Int32 sendActivity1_productId1
        {
            get
            {
                return ((int)(base.GetValue(WfdenServis.Workflow1.sendActivity1_productId1Property)));
            }
            set
            {
                base.SetValue(WfdenServis.Workflow1.sendActivity1_productId1Property, value);
            }
        }
    }
}
```

Son olarak SendActivity bileşeninin ChannelToken özelliği ayarlanarak EndPoint bilgilerinin verilmesi ve WF'in hangi servis ile nasıl mesajlaşacağının ele alınması gerekmektedir. Bu bilgiler çok doğal olarak Add Service Reference işlemi sonrası gelen App.config dosyası içerisinde yer almaktadır. Bu amaçla ChannelToken özelliğine bir isim verildikten sonra gelecek olan EndpointName özelliğine App.config dosyası içerisindeki ilgili servis noktasının adı yazılmalıdır. Sonrasında ise ChannelToken nesnesinin kapsamı (Scope) belirlenir. Bu kapsam OwnerActivityName özelliği yardımıyla set edilmektedir. Örnekte söz konusu özellikler aşağıdaki ekran görüntüsünde olduğu gibi belirlenebilir.

![mk249_11.gif](/assets/images/2008/mk249_11.gif)

Artık test işlemleri başlatılabilir. Console formatında bir Workflow uygulaması geliştirildiğinden Main metodu içerisinde gerekli parametre tanımlama ve sonuç alma işlemleri kolaylıkla gerçekleştirilebilir. Bu amaçla Main metodu aşağıdaki gibi genişletilebilir.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Workflow.Runtime;
using System.Workflow.Runtime.Hosting;
using WfdenServis.ServiceReference1;

namespace WfdenServis
{
    class Program
    {
        static void Main(string[] args)
        {
            using(WorkflowRuntime workflowRuntime = new WorkflowRuntime())
            {
                AutoResetEvent waitHandle = new AutoResetEvent(false);
                workflowRuntime.WorkflowCompleted += delegate(object sender, WorkflowCompletedEventArgs e) 
                    { 
                        waitHandle.Set();
                        Product result = e.OutputParameters["sendActivity1__ReturnValue_1"] as Product;
                        if(result!=null)
                            Console.WriteLine("{0} : {1} {2}", result.ProductId.ToString(), result.Name, result.ListPrice.ToString("C2"));
                        else
                            Console.WriteLine("Ürün bulunamadı");
                    };

                workflowRuntime.WorkflowTerminated += delegate(object sender, WorkflowTerminatedEventArgs e)
                    {
                        Console.WriteLine(e.Exception.Message);
                        waitHandle.Set();
                    };

                    Dictionary<string, object> parametreler = new Dictionary<string, object>() { { "sendActivity1_productId1", 680 } };
                    WorkflowInstance instance = workflowRuntime.CreateWorkflow(typeof(WfdenServis.Workflow1),parametreler);
    
                    instance.Start();
        
                    waitHandle.WaitOne();
            }
        }
    }
}
```

Bilindiği gibi WorkflowInstance nesne örneği oluşturulurken Dictionary tipinden olan ikinci parametre ile iş akışı içerisindeki özelliklere değer aktarımı gerçekleştirilebilmektedir. Bu sebepten örnekte parametreler isimli Dictionary tipinden bir koleksiyon tanımlanmış ve sendActivity1productId1 ismi ile 680 değeri verilmiştir. sendActivity1productId1 isimli özellik hatırlanacağı gibi SendActivity bileşeninin bir parçasıdır ve GetProduct metodunun aldığı productId alanına eş düşmektedir. Bu sayede servis tarafındaki ilgili operasyona parametre gönderimi gerçekleştirilmektedir. Servis tarafındaki işlemler sonlandığında tasarlanan iş akışına göre otomatik olarak WorkflowCompleted olayı tetiklenmektedir.

Bu olayın WorkflowCompletedEventArgs tipinden olan e parametresine ait OutputParameters özelliğinin işaret ettiği koleksiyon, iş akışına ait tüm özelliklere ulaşılabilmesini sağlamaktadır ki bunlardan biriside GetProduct isimli servis metodunun dönüş değerini işaret eden ve SendActivity bileşenine ait olan sendActivity1ReturnValue1 isimli anahtardır (key). Bu anahtarın sonucu Product tipinden olacağı için as anahtar kelimesi ile bir dönüştürme işlemi yapılmakta ve sonuç null değilse elde edilen değişkene ait bilgiler ekrana yazdırılmaktadır. Uygulamanın test edilebilmesi için öncelikli olarak servis tarafının çalışıyor olması gerekmektedir. Buna göre iş akışı uygulamasının 680 productId değeri için vereceği çıktı aşağıdaki gibi olacaktır.

![mk249_12.gif](/assets/images/2008/mk249_12.gif)

Elbette tasarlanan akışın tipine bağlı olaraktan, servise ait operasyonların adımların arasında herhangibir yerde tetiklenmesi ve arkasından sonuçların alınması için başka aktivitelerin (örneğin CodeActivity) iş akışına eklenmeside söz konusudur. Söz gelimi yazılan son örnekte ek bir CodeActivity bileşeni kullanılabilir. Bunun için tasarım zamanında aşağıdaki gibi bir CodeActivity bileşeni eklendiğini ve ExecuteCode özelliğindede UrunuGetir isimli bir metod bildirimi yapıldığını varsayalım.

![mk249_13.gif](/assets/images/2008/mk249_13.gif)

Buna göre UrunuGetir metodunun içeriği aşağıdaki gibi tasarlanıp SendActivity çağrısından sonra GetProduct operasyonunun sonuçlarının alınması sağlanabilir. (Burada sendActivity1ReturnValue1 değerine özelliğine doğrudan erişilmesi son derece doğaldır nitekim metodun tanımlandığı yer Workflow1 sınıfının içidir.)

```csharp
private void UrunuGetir(object sender, EventArgs e)
{
    WfdenServis.ServiceReference1.Product prd = sendActivity1__ReturnValue_1 as WfdenServis.ServiceReference1.Product;
    Console.WriteLine(prd.Name + " " + prd.ListPrice.ToString("C2"));
}
```

Görüldüğü gibi SendActivity bileşeni sayesinde bir iş akışı nesnesi içerisinden WCF tabanlı servis çağırılması, bu servisten sunulan operasyonların icra edilmesi, varsa operasyon sonuçlarının alınması gibi aksiyonlar kolay bir şekilde gerçekleştirilebilmektedir. Bu yazıda bir WCF servisinin, WF içerisinden nasıl çağırılabileceğinin temelleri üzerinde durulmuş ve basit bir örnek adım adım işlenmeye çalışılmıştır. Bir sonraki makalede ise bir WF uygulamasının servis olarak nasıl sunulacağı konularına değinilmeye çalışılacaktır. Böylece geldik bir makalemizin daha sonunda. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](/assets/files/2008/WFileWCF.rar)