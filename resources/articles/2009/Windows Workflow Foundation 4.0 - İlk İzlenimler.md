---
title: "Windows Workflow Foundation 4.0 - İlk İzlenimler"
pubDate: 2009-03-26 12:00:00
categories:
  - WF 4.0
tags:
  - WF 4.0
  - C#
  - XML
  - .NET
  - LINQ
  - WCF
  - Workflow Foundation
  - WPF
  - XAML
  - HTTP
  - IIS
  - Delegates
  - Generics
  - Debugging
  - Visual Studio
---

# Windows Workflow Foundation 4.0 - İlk İzlenimler
Değerli Okurlarım Merhabalar,

Bundan sadece bir kaç sene önce.Net Framework 3.0 versiyonu ile birlikte iş akışlarının (Workflows) kod içerisinde modellenerek farklı uygulamalarda kullanılabilmesini sağlamak amacıyla Windows Workflow Foundation (WWF) alt yapısı duyurulmuştu. Paralelinde ise, Servis Odaklı Mimarilere (Service Oriented Architecture) yeni bir yaklaşım, Windows Communication Foundation (WCF) ile birlikte getirilmişti. Workflow Foundation her ne kadar iş akışlarının (çoğu zaman kod akışlarının) kendi içinde modellenmesini sağlasa da, zaman içerisinde dış ortamlar ile olan haberleşmesinde WCF ile birlikte hareket etmeye başlamıştır. Bu nedenle.Net Framework 3.5 ile birlikte her iki alt yapınında birbirleriyle daha kolay haberleşebilmesi için bazı eklentiler yapılmıştır.

Bu genişletmelerden en önemlileri Workflow Activity Library içerisine eklenen SendActivity ve ReceiveActivity aktivite tipleridir. Böylece bir Workflow uygulamasının servisler yardımıyla dış dünya ile tek yönlü (One-Way) mesajlaşabilmesi yada kendi içerisinden dış dünyaya servis bazlı operasyonlar sunabilmesi mümkün hale gelmiştir. Ama belkide en önemli genişletme WorkflowServiceHost sınıfıdır. Bu sınıf sayesinde, WF uygulamalarının servis olarak host edilebilmesi, bir başka deyişle IIS/WAS (Internet Information Services/Windows Process Activation Service) içerisinde sunulması mümkün kılınmaktadır. Sonuç olarak artık günümüzde, Workflow Tabanlı Servisler (Workflow Based Services) kavramı hayatımızın bir parçası haline gelmeye başlamıştır. Yinede özellikle geliştirici açısından bakıldığında halen daha eksiklikler bulunmaktadır. Özellikle WF ile WCF entegrasyonunda karşılaşılan bu zorlukların üstesinden gelebilmek için.Net Framework 4.0 içerisinde önemli yenilikler bulunmaktadır. İşte bu yazımızda halen son sürümü ile yayınlanmamış olsada WF 4.0 ile gelmesi muhtemel yeniliklere değinilmeye çalışılacaktır.

> Yazımızda yer alan örnekler ve teknik terimlerin çoğu PDC 2008' de yayımlanan Virtual PC imajı üzerinde gerçekleştirilmektedir. Mayıs ayında beta sürümünün yayımlanması planlanan Visual Studio 2010' un ilk görüntüsü ele alınmaktadır. Bu nedenle son sürümler yayımlandıktan sonra makalede yer alan kavramların bir kısmının değiştiği görülebilir ve muhtemeldir.

WF uygulamaları 3.5 sürümünde XAML (eXtensible Application Markup Language) yardımıyla yapılabilsede yinede geliştirici açısından tam anlamıyla oturmuş bir yapı değildi. Örneğin debug edilmelerinde sorundu. Ancak 4.0 sürümünde sadece XAML bazlı Workflow örneklerinin geliştirilmesi çok daha kolay bir hale getirilmektedir. Aslında buradaki en büyük amaçlardan biriside, Workflow tabanlı WCF servislerinin, XAML bazlı olaraktan dekleratif (Declerative) tanımlanabilmesinin sağlanmasıdır. Dekleratif şekilde yapılan tanımlamalar, kodlamaya girmeden çalışma zamanında bazı değişikliklerin kolayca yapılabilmesini sağlamaktadır. Dolayısıyla 4.0 sürümünde WF tarafında ve WCF tarafında XAML yapısını çok daha yaygın bir şekilde görüyor ve kullanıyor olacağız.

XAML tabanlı bu içerikler herhangibir depolama alanında (Muhtemelen Oslo kod adlı yapının değerlendireceği saklama-Repository alanlarında) saklanabileceği gibi, çalışma zamanına devredilerek yürütülebileceklerde. Yani bir WF uygulamasının (hatta bir WCF servisinin ve çok doğal olarak bir Workflow servisinin) bulunduğu ortamdan Export edilerek başka bir platforma taşınması ve orada Import edilerek yürütülmeye başlanması mümkün olabilecek. Diğer taraftan depolanarak saklanan bu Workflow Servislerinin veya diğer WCF servislerinin kolayca yönetilebilmesi (Management) içinde Dublin (Windows Application Server) kod adlı bir çalışma yürütülmektedir. Bu sunucu ve IIS'e gelen eklentiler sayesinde, servislerin kolay bir şekilde Import/ Export edilmesi, izlenebilmesi (Tracking), durumlarının denetlenmesi (Monitoring) gibi Admin seviyesindeki işlemler kolaylıkla gerçekleştirilebilecektir. Tabiki bu konular ile ilişkili detayları önümüzdeki zamanlarda işlemeye çalışıyor olacağız. XAML tabanlı WF örnekleri, 4.0 ile gelen yeniliklerden sadece bir tanesi. Bunun dışında aşağıdaki şekildende görebileceğiniz gibi pek çok yeni aktivite tipi ile karşı karşıyayız.

![mk271_1.gif](images/mk271_1.gif)

Görüldüğü üzere pek çok farklı aktivite tipi yer almakta. Bu şekilde altı mavi çizgi ile işaretlenmiş olan aktiviteler (veya Workflow elementleri) WCF odaklı bileşenlerdir. Örneğin ClientOperation bileşeni ile, bir WCF operasyonunu SendMessage/ReceiveMessage yapısına uygun olacak şekilde çağırmak mümkün olmaktadır. ServiceOperation bileşeni ile WF içerisinden dışaryı bir WCF operasyonu (Operation) yine SendMessage/ReceiveMessage yapısına uygun olaraktan sunulabilmektedir. SendMessage aynen 3.5' teki SendActivity aktivitesine benzer olacak şekilde tek yönlü mesaj (One Way Message) gönderilmesinde kullanılır. Tahmin edileceği üzere ReceiveMessage bileşenide tek yönlü olaraktan WCF mesajlarının alınmasında kullanılmaktadır.

WCF tabanlı örnek bu bileşenler dışında dikkat çekici noktalardan biriside FlowChart aktivite tipidir. Bu bileşen yardımıyla akış diagramı mantığında basit karar yapıları ve anahtar adımlar ile süreçlerin kolayca tasarlanması mümkündür. Geliştiriciler açısından çok yaygın olarak kullanılabilecek bir akış tipidir. Bu akış tipi Sequential ve State Machine aktivite tiplerinin bazı yanlarını kendi içerisinde barındırmaktadır. Buna ek olarak örneğin Assign bileşeni ile workflow seviyesindeki bir değişkene değer atanması sağlanabilir. İlgi çekici diğer aktiviteler ise DbQuery'1, DbUpdate ve Persist bileşenleridir.

Aslında DbQuery'1 bileşeni yardımıyla SQL sorgularının çalıştırılması ve DbUpdate ile veri kaynağına doğru güncelleştirmelerin yapılması mümkündür. Bu belki çok ekstra bir özellikmiş gibi gelmeyebilir ama önemli olan nokta söz konusu aktivitelere ait ayarlamaların dekleratif olarak (yani XAML bazlı) yapılabilmesidir. Persist bileşeni ise, Workflow'un herhangibir noktasında persistence hizmetinin devreye alınarak akışın uzun süreliğine korunabilmesini sağlamaktadır ki Long Running Workflow Service tipleri için önemli bir özelliktir. Bunu 3.5 versiyonunda kod yardımıyla yaptığımız düşünülürse bir aktivite bileşeninin olması geliştirime safhasını kolaylaştırmakta ve yönetimi dahada güçlendirmektedir. (Bu aktivite tiplerini ve nasıl kullanıldıklarını ilerleyen görsel derslerimizde ele almaya çalışıyor olacağım)

Gelelim aktivite kütüphanesine. Artık aktivitelerimiz WorkflowElement isimli yeni bir tipten türetilmektedir. Bir başka deyişle Base Activity Library'de yer WF elementlerin ata sınıfı WorkflowElement tipi olmaktadır. Yine Visual Studio 2010 Object Browser'dan alınan ekran görüntüsünde bu durum aşağıdaki şekildende görüldüğü gibi tespit edilebilir.

![mk271_2.gif](images/mk271_2.gif)

Dikkat edileceği üzere DbQuery aktivitesi, Activity tipinden türemiş olmasına rağmen, Activity tipinin kendisi WorkflowElement tipinden türediği için dolaylı olarak bir WorkflowElement'tir. Assign bileşeni ise doğrudan WorkflowElement tipinden türemektedir. Burada aslında önemli bir noktayı daha vurgulamak gerekiyor. Eğer var olan bir aktivite türünden genişletme yapılarak yeni bir bileşen üretilecekse, Activity tipinden türetilme yapılması ve bu yeni sınıf içerisinde, geriye WorkflowElement referansı döndüren CreateBody metodunun ezilmesi (override) önerilmektedir. Aynen aşağıdaki ekran görüntüsünde olduğu gibi.

![mk271_3.gif](images/mk271_3.gif)

Diğer taraftan eğer sıfırdan bir aktivite tipi tasarlanacaksa WorkflowElement tipinden türetip Execute metodunun override edilmesi gerekmektedir. WorkflowElement aslında abstract bir sınıf ve Execute metodu abstract olarak tanımlanmıştır. Bu nedenle implemente eden tip içerisinde Execute metodunun mutlaka ezilmesi şarttır.(C# Object Oriented kurallarını hatırlayalım)

![mk271_4.gif](images/mk271_4.gif)

Execute metodu parametre olarak ActivityExecutionContext tipinden bir referans değeri almaktadır. Bu sayede söz konusu aktivitenin çalışma zamanında içinde bulunduğu Activite ile konuşabilmesi mümkün olmaktadır. Aktivite tasarlanması ile ilişkili önemli noktalardan biriside dış ortama değişken aktarımı veya tam tersidir. Bu noktada InArgument ve OutArgument tiplerinden yararlanılarak, aktivite içerisine veri girişi veya aktiviteden dış ortama veri çıkışı sağlanabilmektedir. Biraz sonra geliştireceğimiz örneğimizde iki özel aktivite tipi yazarken bu generic sınıflardan yararlanılacaktır.

WF 4.0 akışlarının kolay bir şekilde tasarlanabilmesi için Visual Studio 2010 içerisinde WPF (Windows Presentation Foundation) tabanlı bir arayüz sunulmaktadır. Bu arayüz sayesinde akışların daha zengin bir görsellikle hazırlanması mümkündür. Söz gelimi zoom özelliği ile büyük akışların ekran içerisinde daha verimli şekilde görülebilmesi sağlanmaktadır. Zannediyorum aşağıdaki ekran görüntüsünde bu durumun kafamızda biraz daha netleşmesi için yeterlidir.

![mk271_5.gif](images/mk271_5.gif)

Dikkat çekici noktalardan bir diğeride örnekte yer alan ProductActivity bileşeni yanında bir Breakpoint yer almasıdır. Dolayısıyla debug işlemleri için Workflow elementlerinin kendisinin designer içerisinde doğrudan işaretlenebildiğini söyleyebiliriz. Diğer dikkat çekici kısımda sol alt tarafta yer alan Arguments ve Variables bölümleridir. Bugün yazımızda Variables kısmını kullanarak Sequence aktivitesinin tamamını ilgilendiren değişkenlerin nasıl tanımlanabileceğini ve kullanılabileceğinide görmüş olacağız. Önemli noktalarda biriside Arguments veya Variables gibi bölümler ile Workflow içerisinde çeşitli tanımlamaların koda girmeye gerek duymadan kolay bir şekilde görsel olarak yapılabilmesidir. (Designer tarafının kullanımını daha kolay kavramak için yayınlanacak görsel derslerimizi takip etmenizi öneririm.)

> Variables ve Arguments;
> WF 4.0' da Variables kavramı verinin depolanmasını ifade eder. Değişkenler (Variables) Workflow'un (yada Activity bileşeninin) çalışma zamanı örneği boyunca veri depolamak amacıyla kullanılırlar. Tanımlanırken adları ve tipleri belirtilir. Yaşamları, Workflow veya Activity örneğinin bellekte kaldığı süre boyunca geçerlidir. Yani Workflow sonlandığında veya Activity bileşeni çalışmasını tamamladığında referansları bellekten kaldırılmaktadır.
>
> Arguments kavramı ise verinin aktivite içerisine veya aktivite içerisinden dışarıya aktarılması anlamında kullanılmaktadır. Her argümanın bir yönü (Direction) vardır.(Input, Output, Input/Output) Argümanlar aktivite içerisinde tanımlanırken InArgument, OutArgument veya InOutArgument tipinden tanımlanırlar. T ile kullanılacak verinin tipi belirtlir.

Dilerseniz basit bir örnek geliştirerek yenilikleri daha yakından tanımaya çalışalım. Öncelikle Visual Studio 2010 ortamında.Net Framework 4.0 şablonunda bir Activity Library geliştirerel işe başlayacağız. Bu kütüphane örneğimizde kullanacağımız özel aktivite tiplerini barındırıyor olacak.

![mk271_6.gif](images/mk271_6.gif)

Bu işlemin ardından projemize bir adet sınıf ekleyerek devam edebiliriz. Sınıfımız içeriği ise aşağıdaki gibidir.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.WorkflowModel;

namespace SampleActiviyLibrary
{
    public class ProductActivity
        :WorkflowElement
    {
        public OutArgument<int> ProductId { get; set; }
        public OutArgument<int> Count { get; set; }

        protected override void Execute(ActivityExecutionContext context)
        {
            Console.WriteLine("ProductId ?");
            ProductId.Set(context, Convert.ToInt32(Console.ReadLine()));
            Console.WriteLine("Requested count ?");
            Count.Set(context, Convert.ToInt32(Console.ReadLine()));
        }
    }
}
```

Burada dikkat edeceğiniz üzere kod yardımıyla özel bir aktivite tipi (Custom Activity) geliştirilmektedir. ProductActivity sınıfı WorkflowElement tipinden türemektedir ve içerisinde OutArgument tipinden ProductId ve Count isimli özellikler (Properties) yer almaktadır. Override edilen Execute metodu içerisinde, OutArgument tipinin Set metodundan yararlanılarak, Console penceresinden okunan değerlerin ProductId ve Count özelliklerine aktarılması sağlanır. OutArgument kullanılması nedeniyle, özelliklerin değerleri ProductActivity aktivitesinin kullanıldığın Workflow ortamına aktarılabilmektedir. Yani bu aktivite içerisinde Console penceresinden alınan bir takım değerler (ki bu sadece örnek olarak verilmiştir, gerçek hayat senaryolarında bu değerlerin farklı kaynaklardan gelmesi muhtemeldir.), aktivitenin kullanıldığı Workflow içeriğine ProductId ve Count isimleriyle taşınabilir. Gelelim ikinci özel aktivitemize.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.WorkflowModel;

namespace SampleActiviyLibrary
{
    public class OrderInformationActivity
        :WorkflowElement
    {
        public InArgument<int> OrderedProductId { get; set; }
        public InArgument<int> OrderedProductCount { get; set; }

        protected override void Execute(ActivityExecutionContext context)
        {
            int orderedId=OrderedProductId.Get<int>(context);
            int orderedCount = OrderedProductCount.Get<int>(context);

            // TODO: Mail gönderme işlemleri.
            Console.WriteLine("{0} numaralı üründen {1} adet siparis edilmistir",orderedId,orderedCount);
        }
    }
}
```

OrderInformationActivity bileşenide ProductActivity sınıfı gibi WorkflowElement tipinden türemektedir. Ancak bu kez InArgument tipinden OrderedProductId ve OrderedProductCount isimli özellikler kullanılmaktadır. Yani, Workflow içerisinden bu aktiviteye, InArgument tipinden tanımlanan özellikler üzerinden veri taşınabilir. Girilen değerlerin Execute metodu içerisinde elde edilişi sırasında InArgument tipinin generic Get metodu kullanılır. Dikkat edileceği üzere metodlar parametre olarak ActivityExecutionContext tipinden referans almaktadır. Yani, aktivitenin kullanıldığı Workflow ortamına ait bir takım bilgiler (örneğin InstanceId gibi) Execute metodu içerisinde taşınabilmektedir. Bu aktivite ilede, sembolik olarak Console penceresine sipariş edilen ürün numarası ve miktarı bilgileri yazdırılmakta ve belkide mail gönderme işlemleri gerçekleştirilmektedir. Workflow örneğimizi tasarlamadan önce arada XAML bazlı bir aktivite tipinin nasıl geliştirileceğine de değinmek isterim. Bu amaçla Visual Studio 2010 ortamında, projeye Add New Item ile bir Activity öğesi aşağıdaki şekilde görüldüğü gibi eklenir.

![mk271_7.gif](images/mk271_7.gif)

Dikkat edileceği üzere aktivitenin uzantısı XAML'dır. Bununla birlikte açılan pencereden görüleceği gibi Activity tipi görsel olarak tasarlanabilir. Buradaki aktivitede aslında birde özellik tanımlanmaktadır. DiscountRate isimli bu özellik Argument penceresi kullanılaraktan aşağıdaki gibi oluşturulmuştur! Süper.

![mk271_8.gif](images/mk271_8.gif)

Görüldüğü gibi herhangibir şekilde kod yazılmamıştır. Ardından tasarım ortamında basit bir Assign bileşeni sürüklenerek, Discount aktivitemizde aşağıda görüldüğü gibi kullanılması sağlanmaktadır.

![mk271_9.gif](images/mk271_9.gif)

Assign aktivitesinin yaptığı tek şey DiscountRate isimli özelliğe sabit bir değerin atanmasının sağlanmasıdır. Ancak burada önemli olan noktalar Discount bileşeninin tamamen görsel olarak tasarlanması ve sonucun XAML olarak aşağıdaki gibi üretilmesidir.

```xml
<p1:Activity x:Class="SampleActiviyLibrary.Discount" xmlns:p="http://schemas.microsoft.com/netfx/2008/xaml/schema" xmlns:p1="http://schemas.microsoft.com/netfx/2009/xaml/workflowmodel" xmlns:s="clr-namespace:System;assembly=mscorlib" xmlns:swdx="clr-namespace:System.WorkflowModel.Design.Xaml;assembly=System.WorkflowModel.Design" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <p:SchemaType.Members>
        <p:SchemaProperty Name="DiscountRate" Type="s:Decimal" />
    </p:SchemaType.Members>
    <p1:Assign>
        <p1:Assign.To>
            <p1:OutArgument x:TypeArguments="s:Decimal">[DiscountRate]</p1:OutArgument>
        </p1:Assign.To>
        <p1:Assign.Value>
            <p1:InArgument x:TypeArguments="p:Double">[1.12R]</p1:InArgument>
        </p1:Assign.Value>
    </p1:Assign>
</p1:Activity>
```

Görüldüğü gibi, tasarım zamanında geliştirdiğimiz bu aktivite tipi sadece XAML (Only XAML) içerikli olacak şekilde üretilmiştir. İşte dekleratif tanımla dediğimizde tam olarak budur. Üretilen bu XAML içeriği herhangibir ortamda depolanabilir ve dahada önemlisi WF çalışma zamanı tarafından yürütülerek başka Workflow'lar tarafından ele alınıp kullanılabilir.

Bu kısa bilgilerden sonra tekrar örneğimize dönebiliriz. Artık geliştirdiğimiz Workflow kütüphanesini basit bir Sequential Workflow Console Application projesinde referans ederek kullanacağız. Console uygulamasına ekleyeceğimiz ProductOrderFlow.xaml isimli aktivite içerisinde bir Sequence bileşeni bulunmaktadır. Bu bileşen içinde ise ürün numarası ve sipariş adedi bilgileri için iki değişken tanımlanmaktadır. Bu sefer Variables kısmını kullanaraktan aşağıdaki ekran görüntüsünde olduğu gibi gerekli değişkenleri kolayca belirleyebiliriz.

![mk271_10.gif](images/mk271_10.gif)

UrunNo ve SiparisAdedi isimli Int32 tipinden olan değişkenler Sequence aktivitesi içerisinde tanımlandıklarından, alt aktiviteler tarafındanda erişilip kullanılabilmektedirler. Şimdi Sequence aktivitesi içerisine önce ProductActivity sonrada OrderInformationActivity bileşenlerini ekleyeceğiz. ProductActivity bileşeni hatırlayacağınız gibi Console penceresinden okuduğu değerleri ProductId ve Count isimli output özelliklerine aktarmaktaydı. Dolayısıyla bu değerleri Sequence içerisinde tanımlanan UrunNo ve SiparisAdedi isimli değişkenler ile eşleştirmemiz mümkündür. Bunun için tek yapılması gereken, ProductActivity bileşeninin özelliklerinden ilgili atamaların aşağıdaki ekran görüntüsünde olduğu gibi yapılmasıdır.

![mk271_12.gif](images/mk271_12.gif)

ProductActivity bileşeninin hemen arkasına OrderInformationActivity bileşenini ekleyerek devam edebiliriz. Bu bileşende hatırlayacağınız gibi kullanıldığı ortamdan girdi (Input) değerleri almak üzere iki özelliğe sahiptir. Bu nedenle söz konusu aktivitenin özelliklerinde aşağıdaki ekran görüntüsünde yer alan ayarları yapmamız yeterlidir.

![mk271_11.gif](images/mk271_11.gif)

Böylece bir önceki aktivite ile, Sequence aktivitesindeki SiparisAdedi ve UrunNo isimli değişkenlere taşınan değerler, OrderInformationActivity içerisinden elde edilebilirler. İşte bu kadar. Tabi şimdilik:) Tüm bu işlemlerin arından ProductOrderFlow.xaml aktivitesine ait XAML içeriğine bakıldığında, aşağıdaki kod parçasında yer alan çıktının elde edildiği görülebilir.

```xml
<p:Activity x:Class="HostWFApplication.ProductOrderFlow" xmlns:p="http://schemas.microsoft.com/netfx/2009/xaml/workflowmodel" xmlns:p1="http://schemas.microsoft.com/netfx/2008/xaml/schema" xmlns:s="clr-namespace:SampleActiviyLibrary;assembly= SampleActiviyLibrary" xmlns:swd="clr-namespace:System.WorkflowModel.Debugger;assembly= System.WorkflowModel" xmlns:swdx="clr-namespace:System.WorkflowModel.Design.Xaml;assembly= System.WorkflowModel.Design" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <p:Sequence swd:XamlDebuggerXmlReader.FileName="C:\Orneklerim\SampleActiviyLibrary \HostWFApplication\ProductOrderFlow.xaml">
        <p:Sequence.Variables>
            <p:Variable x:TypeArguments="p1:Int32" Name="UrunNo" />
            <p:Variable x:TypeArguments="p1:Int32" Name="SiparisAdedi" />
        </p:Sequence.Variables>
        <s:ProductActivity Count="[SiparisAdedi]" ProductId="[UrunNo]" />
        <s:OrderInformationActivity OrderedProductCount="[SiparisAdedi]" OrderedProductId="[UrunNo]" />
    </p:Sequence>
</p:Activity>
```

Harika değil mi? Tüm akış içeriği XAML bazlı olacak şekilde tanımlanmış durumda. Burada durup bu XAML içeriğini yorumlayacak programları düşünmek gerekiyor. Daha karmaşık akışlara ait bu içerikler çeşitli araçlar yardımıyla yönetilebilir ve akışların değiştirilerek yeni halleriyle devreye alınması sağlanabilir.

Peki akışı devreye sokacak olan çalışma zamanı ortamının hazırlayacısı nerededir? Sonuç itibariyle yazılan Workflow örneklerinin mutlaka bir Host uygulama içerisinde ele alınıyor olması şarttır. Örneğimiz Visual Studio 2010 içerisine gömülmüş hazır bir Console şablonu olduğundan,.Net 3.0 ve.Net 3.5' teki WF projelerinde olduğu gibi tüm gerekli kodlar otomatik olarak üretilmektedir. Geliştirdiğimiz örnekte bu kodlar Program.cs dosyasının bir parçası olarak aşağıdaki gibi üretilir.

```csharp
namespace HostWFApplication
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.WorkflowModel;
    using System.WorkflowModel.Activities;

    class Program
    {
        static void Main(string[] args)
        {
            Sequence s;
            AutoResetEvent syncEvent = new AutoResetEvent(false);

            WorkflowInstance myInstance = WorkflowInstance.Create(new ProductOrderFlow());
            myInstance.Completed += delegate(object sender, WorkflowCompletedEventArgs e) 
                { 
                    syncEvent.Set(); 
                };
            myInstance.Resume();

            syncEvent.WaitOne();
    
        }
    }
}
```

Görüldüğü gibi ilk olarak ProductOrderFlow tipinden bir WorkflowInstance referansı üretilmektedir. Workflow tamamlandığında Completed olay metodu devreye girer. Her zamanki gibi WorkflowCompletedEventArgs parametresinden yararlanarak örneğin WF tarafından üretilen Output değerleri ele alınabilir. (Bu arada s isimli Sequence tipinden bir değişken yer aldığı görülmektedir. Hiç kullanılmayan bu değişkenin final sürümünde zaten ortadan kaldırılacağı söyleniyor. Burada kazayla kaldığını sanıyorız;)) Uygulamamızı bu son haliyle çalıştırdığımızda aşağıdaki ekran görüntüsü ile karşılaşırız.

![mk271_13.gif](images/mk271_13.gif)

Dikkat edileceği üzere kullanıcıdan ürün numarası ve sipariş adedi istenmiş sonrasında ise buna uygun bir işlem yürütülmüş ve tasarlanmış olan akış başarılı bir şekilde tamamlanmıştır.

Buraya kadar anlattıklarımız ile aslında Workflow Foundation 4.0 ile gelen yeniliklerin sadece bir kısmını inceleme şansını bulduk. Durumu değerlendirdiğimizde aşağıdaki maddeler ile kısa bir özet geçebiliriz;

- WF 4.0 içerisinde XAML kullanımı daha etkili ve yaygın hale getirilmiş, bu sayede Workflow Based Service'lerin dekleratif olarak geliştirilmesinin yolu tam olarak açılmıştır. Bu konuyu bir sonraki makalemizde (veya görsel dersimizde) incelemeye çalışıyor olacağım.
- WF 4.0 içerisinde WCF 4.0 ile daha iyi anlaşılmasını sağlayacak yeni aktiviteler eklenmiştir.
- Sayısız pek çok yeni aktivite dışında FlowChart tipide göz ardı edilmemelidir. Bu tip özellikle geliştiricilerin bildiği akış şemaları mantığına uygun olacak şekilde süreç tasarlanmasını olanaklı kılmaktadır.
- Activity Base Library içerisinde yer alan ata aktivite tipi artık WorkflowElement olarak tasarlanmıştır. Buna göre sıfırdan tasarlanacak aktivitelerin WorkflowElement abstract sınıfından türemesi, var olanları genişletecek olanların ise Activity sınıfından türeyerek CreatBody metodunu ezmesi önerilir.
- Geliştirici tanımlı bir aktivite, istenirse kod yerine tasarım aracı yardımıyla da üretilebilir. Bu durumda bir XAML içeriği oluşturulmaktadır.
- Visual Studio 2010 tarafında WPF tabanlı yeni tasarım aracı sayesinde, WF geliştirilmesi son derece kolay ve zevkli bir hal almaktadır. Özellikle aktivitelerin arguman (Argument) veya değişkenlerinin (Variables) görsel olarak belirlenebilmesi önemli noktalardan birisidir.
- XAML tabanlı WF yapıları, Oslo gibi modellerin sağladığı depolama alanlarında (Repositories) saklanabilir, başka uygulamalar tarafından (Quadrant gibi) yönetilip değiştirilebilir, diğer WF çalışma zamanı motorlarına aktarılabilmeleri (Import) için bulundukları ortamlardan çıktı (Export) olarak verilebilirler.

Elbette bahsettiğimiz konuların tamamında değişiklikler olabilir, olması muhtemeldir. Kesinlik ancak son sürümler ile ortaya çıkacaktır. Böylece geldik bir makalemizin daha sonuna. Bu makalemizde ilk bakışta WF 4.0 ile gelen yeniliklerin bir kısmına değinmeye çalıştık ve konuyu pekiştirmek amacıyla bir örnek geliştirdik. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.
