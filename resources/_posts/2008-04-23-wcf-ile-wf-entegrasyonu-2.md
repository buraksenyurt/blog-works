---
layout: post
title: "WCF ile WF Entegrasyonu - 2"
date: 2008-04-23 12:00:00 +0300
categories:
  - wcf
  - wf
tags:
  - windows-communication-foundation
  - workflow-foundation
  - windows-workflow-foundation
---
Bir önceki yazımızda WCF (Windows Communication Foundation) servislerinin, WF (Windows WorkFlow) uygulamaları içerisinden nasıl çağırıldığını incelemiştik. Bu yazımızda ise tam tersine, bir Workflow örneğinin servis olarak nasıl sunulabileceğini analiz ediyor olacağız. Bazı durumlarda kod akışlarının birer servis olarak istemcilere sunulması gerekebilir. Burada söz konusu kod akışlarının Servis Yönelimli Mimarinin (Service Oriented Architecture) imkanlarından yararlanıyor olması isteği ön plana çıkmatadır. Çok doğal olarak servis gibi yayınlanan akış tipleri (Workflow Instance), istemci ile olan mesajlaşmalarında SOA temelli olanakları kullanabilir hale gelmektedir. Bu noktada WCF ile WF entegrasyonu göz önüne alınmalıdır.

WCF servisleri WF uygulamaları içerisinden çağırılırken ağırlıklı olarak.Net Framework 3.5 ile birlikte gelen SendActivity bileşeni kullanılmaktadır. WF örneklerinin servis olarak yayınlamasında ise başrol oyuncusu yine.Net Framework 3.5 ile birlikte gelen ReceiveActivity isimli activity bileşenidir. Özellikle Visual Studio 2008 kullanılarak servis destekli Workflow kütüphaneleri kolay bir şekilde geliştirilebilmektedir. Bu amaçla Visual Studio 2008 ortamına Sequential Workflow Service Library ve State Machine Workflow Service Library proje şablonları eklenmiştir. Tabi çok doğal olarak geliştirilen iş akışı servislerinin bir uygulama tarafından barındırılması (Hosting) ve yayınlanmasıda gerekmektedir. Host seçenekleri WF servisleri içinde aynıdır. IIS üzerinde, WAS (Windows Activation Service) yardımıyla veya Self-Hosting seçeneklerine göre barındırma ve yayınlama yapılabilir. Önemli olan noktalardan birisie normal WCF servislerinden farklı olaraktan Host çalışma ortamını WorkflowServiceHost tipinin yönetmesidir.

WF uygulamasının servis olarak yayınlanması için istemcilere bir sözleşme (Contract) bildirimi yapılması gerekmektedir. Çok doğal olarak bu sözleşme (Contract) bir arayüz (Interface) olarak tanımlanmalıdır. Arayüz içerisinde yer alan operasyonlar dışarıya ReceiveActivity bileşeni ile sunulabilirler. Buna göre sözleşme içerisinde tanımlanan her operasyon için (OperationContract niteliği ile imzalanmış metodlar) birer ReceiveActivity oluşturulmalıdır. Bu noktada karşılaşılan önemli sorulardan biriside, istemcinin bu operasyona nasıl parametre göndereceği veya cevap alacağıdır. Nitekim bir WCF kütüphanesinde çoğunlukla asıl işi yapan bileşen, sözleşme tipinin uygulandığı bir sınıftır (Class). WF açısından bakıldığında ise bu görevi Workflow sınıfı içerisinde tanımlanan özellikler (Properties) yada alanlar (fields) üstlenmektedir. Bir başka deyişle, ReceiveActivity irtibatta olduğu operasyon için gerekli parametreler ile haberleşmek adına söz konusu özellik veya alanlardan yararlanır. Dolayısıyla asıl iş ReceiveActivity içerisinden yapılmalıdır. ReceiveActivity bileşeninin composite bir bileşen olarak tanımlanmasının sebebide budur. ReceiveActivity içerisine örneğin CodeActivity gibi bileşenler dahil edilerek operasyonun asıl işinin yapılacağı kod bloklarının işletilmesi sağlanabilir. İstemci açısından olaya bakıldığında yine bir proxy nesnesinin servis ile olan haberleşmeyi sağladığı ortadadır. Aslında aşağıdaki şekil durumu biraz daha kolay bir şekilde açıklamaktadır.

![mk250_1.gif](/assets/images/2008/mk250_1.gif)

Şekle göre WorkflowActivity sınıfının ReceiveActivity bileşenleri, dışarıya servis üzerinden sunulan operasyonlar ile ilişkili talep alma ve cevap gönderme işlemlerini üstlenmektedir. Çok doğal olarak servisi bir EndPoint üzerinden dışarıya sunmak gerekmektedir. EndPoint tanımında dikkat edilmesi gereken noktalardan biriside, bağlayıcı tipin (Binding Type) basicHttpContextBinding, netTcpContextBinding yada wsHttpContextBinding bileşenlerinden birisi olmaslıdır. Bu bağlayıcı tiplerin ortak özelliği servis destekli iş akışları (Service-Enabled Workflow) ile haberleşilebilmesini sağlamalarıdır. Ancak istenirse özel bağlayıcılara ContextBindingElement tipi uygulanarak workflow destekli hale getirilmeleride sağlanabilir. İstemci uygulama çok doğal olarak servis ile olan haberleşme sırasında proxy sınıfından yararlanmak durumundadır.

> WF ve WCF entegrasyonun bir sonucu olarak istemciler bir Workflow aktivitesini servis bazlı olacak şekilde çağırıp kullanabilmektedir. Böylece istemciler bir kod akışı süreciniservis tabanlı düşünerek ele alabilirler. Buna JSON (JavaScriptObjectNotation) gibi mesajlaşma desteklerinin eklenebileceği düşünüldüğünde bir Workflow örneğinin herhangibir platform üzerinden kullanılabilmeside mümkün hale gelmektedir. Bu WCF tanımında yer alan "her hangibir CLR tipinin servis olarak yayınlanabilmesi" ilkesinin bir sonucu olarak görülebilir.

Bu kadar karmaşık ve teorik bilgiden sonra örnek bir uygulama üzerinden hareket ederek WF uygulamaları içerisinde servis yayınlamasının nasıl yapılabileceğini adım adım incelemeye çalışalım. İşe ilk olarak servis sözleşmesini ve operasyonlar için gerekli aktivite bileşenlerini içerecek olan kütüphaneyi tasarlamak ile başlamak doğru olacaktır. Bu amaçla Visual Studio 2008 ortamında bir Sequential Workflow Service Library projesi açtığımızı düşünelim.(Bu şablon New Project->WCF sekmesi altında yer almaktadır.)

![mk250_2.gif](/assets/images/2008/mk250_2.gif)

Söz konusu servis kütüphanesine (WFSiparisKutuphanesi) bakıldığında ilk dikkati çeken nokta referanslar kısmına Workflow desteği için eklenen assembly'lardır.

![mk250_3.gif](/assets/images/2008/mk250_3.gif)

Bu noktada şablon olarak getirilen IWorkflow1.cs, Workflow1.cs ve App.config isimli dosyalar silinmiştir. Örnekte kullanılan servis sözleşmesi içeriği ise aşağıdaki sınıf diagramında görüldüğü gibidir.

![mk250_6.gif](/assets/images/2008/mk250_6.gif)

```csharp
using System;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace WFSiparisKutuphanesi
{
    [ServiceContract(Namespace = "http://www.bsenyurt.com/UrunSiparisServisi", Name = "UrunSiparisServisi")]
    public interface ISiparisSozlesmesi
    {
        [OperationContract]
        string SiparisVer(UrunBilgisi urun);
    }

    [DataContract]
    public class UrunBilgisi
    {
        [DataMember]
        public string UrunKodu { get; set; }
        [DataMember]
        public int Adet { get; set; }
        [DataMember]
        public DateTime SiparisTarihi { get; set; }
    } 
}
```

ISiparisSozlesmesi isimi servis sözleşmesi (Service Contract) SiparisVer isimli tek bir operayon sunmaktadır. SiparisVer metodu parametre olarak UrunBilgisi sınıfına ait bir nesne örneği almaktadır. Bu nesneye ait veri, çalışma zamanında istemci tarafından servise doğru geleceğinden serileştirilebilir olması gerekmektedir. Bu sebepten doğal olarak veri sözleşmesi (Data Contract) olacak şekilde tasarlanmıştır. SiparisVer isimli operasyon aynı zamanda geriye string tipinden bir değerde döndürmektedir. Böylece dışarıya sunulacak olan servis sözleşmesi tanımlanmıştır. Artık bu sözleşmeyi kullanacak olan aktivite sınıfının yazılması gerekmektedir. Örnekte bunun için bir adet Sequential Workflow Activity sınıfı ele alınacaktır. Bunun için projeye Add->Sequential Workflow seçeneği ile yeni bir akış sınıfı eklenmelidir.

![mk250_4.gif](/assets/images/2008/mk250_4.gif)

Bu adımda Sequential Workflow (with code seperation) seçeneği işaretlenerek devam edilebilir. Böylece dizayn tarafı ile kodu birbirinden ayrı tutacak bir akış tipi oluşturulacaktır.

![mk250_5.gif](/assets/images/2008/mk250_5.gif)

WFSiparis.xoml (XOML-eXtensible Object Markup Language) tasarım zamanında gerekli aktivite tiplerini barındıracaktır. WFSiparis isimli sınıf SequentialWorkflowActivity tipinden türemektedir. Servis sözleşmesi içerisinde yer alan SiparisVer metodu bu akış nesnesi içerisinde ReceiveActivity bileşeni tarafından ele alınmalıdır. Bununla birlikte SiparisVer metodunun UrunBilgisi ve döndüreceği değer için birer özellik/alan (Property/Field) içermesi gerekmektedir.

Öncelikli olarak bir ReceiveActivity bileşeni tasarım ekranına sürüklenip bırakılmalıdır. ReceiveActivity bileşeni SendActivity bileşenine benzer olaraktan ServiceOperationInfo özelliğini içermektedir. Çok doğal olarak bu özellikten yararlanılarak hangi operasyonun ele alınacağı ve kullanılacak (yada otomatik olarak üretilecek) olan parametreler belirlenmelidir. ServiceOperationInfo özelliğinde yer alan üç nokta düğmesine basıldıktan sonra aşağıdaki arayüz ile karşılaşılır.

![mk250_7.gif](/assets/images/2008/mk250_7.gif)

Burada yine Import seçeneği kullanılarak aşağıdaki ekran görüntüsünde olduğu gibi ilgili servis sözleşmesinin seçilmesi gerekmektedir.

![mk250_8.gif](/assets/images/2008/mk250_8.gif)

Çok doğal olarak ISiparisSozlesmesi otomatik olarak gelecektir. Bu noktada Workflow içerisinde birden fazla servis sözleşmesi tutulabileceği de unutulmamalıdır. Bu seçim işlemini takiben aşağıdaki ekran görüntüsü elde edilir ve artık operasyon seçimi ve bunun için gerekli sınıf özelliklerinin oluşturulması adımına geçilebilir.

![mk250_9.gif](/assets/images/2008/mk250_9.gif)

Dikkat edileceği üzere Parameters kısmında String tipinden yönü Out olan ve UrunBilgisi tipinden yönü In olan birer parametre görülmektedir. Bu parametrelerin akış içerisinde ele alınması için karşılığı olan özelliklerin veya alanların sınıf içerisine ya manuel yada otomatik olarak dahil edilmesi şarttır. Sonuç olarak aşağıdaki ekran görüntüsü ile karşılaşılacaktır.

![mk250_10.gif](/assets/images/2008/mk250_10.gif)

Burada ReturnValue ve urun özellikleri için otomatik özellik ürettirilmesi sağlanabilir. Söz gelimi aşağıdaki ekran görüntüsünde örnek olarak SiparisVer metodunun geri dönüş tipi için otomatik özellik ürettirilmesinin nasıl sağlandığı gösterilmektedir.

![mk250_11.gif](/assets/images/2008/mk250_11.gif)

Aynı işlem urun özelliği içinde yapıldıktan sonra receiveActivity1 için son durum aşağıdaki gibi olacaktır.

![mk250_12.gif](/assets/images/2008/mk250_12.gif)

Çok doğal olarak bu yapılan değişiklikler sonrasında WFServis isimli sınıfın içeriği aşağıdaki gibi değişecektir.

```csharp
namespace WFSiparisKutuphanesi
{
    public partial class WFSiparis : SequentialWorkflowActivity
    {
        public static DependencyProperty receiveActivity1_urun1Property = DependencyProperty.Register("receiveActivity1_urun1", typeof(WFSiparisKutuphanesi.UrunBilgisi), typeof(WFSiparisKutuphanesi.WFSiparis));
        public static DependencyProperty receiveActivity1__ReturnValue_1Property = DependencyProperty.Register("receiveActivity1__ReturnValue_1", typeof(System.String), typeof(WFSiparisKutuphanesi.WFSiparis));

        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(true)]
        [CategoryAttribute("Parameters")]
        public UrunBilgisi receiveActivity1_urun1
        {
            get
            {
                return ((WFSiparisKutuphanesi.UrunBilgisi)(base.GetValue(WFSiparisKutuphanesi.WFSiparis.receiveActivity1_urun1Property)));
            }
            set
            {
                base.SetValue(WFSiparisKutuphanesi.WFSiparis.receiveActivity1_urun1Property, value);
            }
        }

        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(true)]
        [CategoryAttribute("Parameters")]
        public string receiveActivity1__ReturnValue_1
        {
            get
            {
                return ((string)(base.GetValue(WFSiparisKutuphanesi.WFSiparis.receiveActivity1__ReturnValue_1Property)));
            }
            set
            {
                base.SetValue(WFSiparisKutuphanesi.WFSiparis.receiveActivity1__ReturnValue_1Property, value);
            }
        }
    }
}
```

Artık ReceiveActivity içerisine istemcilere döndürülecek olan cevap için gerekli activity bileşeninin eklenmesi adımına geçilebilir. Sonuç itibariyle istemciler SiparisVer metoduna çağrıda bulunduktan sonra bir aktivitenin işletilmesi gerekmektedir. Bu aktivite ReceiveActivity içerisinde tanımlanabilir. Söz gelimi CodeActivity bu işlem için idealdir. Operasyona istemciden gelen bilgi, aktivite sınıfının receiveActivity1urun1 özelliği üzerinden elde edilebilir.

Metoddan istemciye döndürülecek olan string değer ise receiveActivity1ReturnValue1 özelliğine set edilmelidir. Bu atama işlemide CodeActivity bileşeni içerisinde yapılabilir. (Burada CodeActivity kullanılması şart değildir. Önemli olan aktivite sınıfı içerisine eklenen özellikler yardımıyla istemciden operasyona gelen parametre bilgilerinin alınabilmesi veya geriye döndürelecek bir sonucun üretilebilmesi ve istemci tarafından ele alınabilmesidir.) Şimdi ReceiveActivity içerisine bir CodeActivity eklediğimizi düşünelim.

![mk250_13.gif](/assets/images/2008/mk250_13.gif)

CodeActivity1 bileşeninin ExecuteCode özelliğine örnek olarak SiparisiIsle değerini verip kod içeriğini aşağıdaki gibi geliştirdiğimizi düşünelim. (Şu anda amaç WF servislerinin nasıl yazılacağını görmek olduğundan Console tabanlı host ve istemci uygulamalar yazılacaktır. Bu nedenle CodeActivity bileşeninin işaret edeceği metod içerisinden o andaki talep bilgilerini değerlendirmek amacıyla Console ekranına bilgi yazdırılmaktadır.)

```csharp
private void SiparisiIsle(object sender, EventArgs e)
{
    // Operasyona gelen istemci çağrısında UrunBilgisi nesne örneğine ait veriler bulunmaktadır. Bu verilere aktivite sınıfının özellikleri üzerinden erişilebilir.
    Console.WriteLine("Adet talebi : {0} Urun Numarası : {1} İstek Tarihi : {2}", receiveActivity1_urun1.Adet, receiveActivity1_urun1.UrunKodu, receiveActivity1_urun1.SiparisTarihi.ToString());
    receiveActivity1__ReturnValue_1 = "Istek Alınmıştır"; // İstemciye operasyonda dönecek olan sonuç
}
```

Artık servisi barındıracak olan Host uygulamanın yazılmasına başlanabilir. Host uygulama daha öncedende bahsedildiği gibi WCF mimarisinin izin verdiği herhangibir çeşitte olabilir (IIS, Console, WPF, WAS, Windows Service...). Örnekte Host uygulama basit bir Console projesi olarak geliştirilmektedir. Host uygulamada önemli olan noktalardan birisi, Workflow Service kütüphanesi ile birlikte, Workflow ve WCF çalışma ortamları için gerekli assembly'lara ihtiyaç olduğudur.

Bu nedenle ilk etapta Console uygulamasında System.ServiceModel, System.Workflow.Activites, System.Workflow.ComponentModel, System.WorkflowServices kütüphanelerinin referans edilmesi gerekmektedir. Host uygulama için önem arz eden noktalardan biriside çalışma zamanı ortamı için gerekli konfigurasyon ayarlarıdır. Aynen WCF servislerinin yazılmasında olduğu gibi config dosyalarından yararlanılabilir yada kod bazında gerekli ayarlamalar yapılabilir. Örnekte kullanılan config dosyası içeriği aşağıdaki gibidir.

Host Uygulama App.config;

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <services>
            <service behaviorConfiguration="WFSiparisBehavior" name="WFSiparisKutuphanesi.WFSiparis">
                <endpoint address="" binding="wsHttpContextBinding" name="WfSiparisWsHttpEndPoint" contract="WFSiparisKutuphanesi.ISiparisSozlesmesi" />
                <endpoint address="mex" binding="mexHttpBinding" name="MexEndPoint" contract="IMetadataExchange" />
                <host>
                    <baseAddresses>
                        <add baseAddress="http://localhost:10001/WFSiparisServisi" />
                    </baseAddresses>
                </host>
            </service>
        </services>
        <behaviors>
            <serviceBehaviors>
                <behavior name="WFSiparisBehavior" >                    
                    <serviceMetadata httpGetEnabled="true" />
                         <serviceDebug includeExceptionDetailInFaults="true"/>
                </behavior>
            </serviceBehaviors>
        </behaviors>
    </system.serviceModel>
</configuration>
```

Konfigurasyon dosyasında görüldüğü gibi iki adet EndPoint tanımlaması yapılmaktadır. Bunlardan birisi bağlayıcı olarak wsHttpContextBinding tipini kullanmaktadır. Diğer EndPoint ise base address üzerinden metadata erişimine izin veren bir Mex (Metadata Exchange) EndPoint olarak tanımlanmıştır. (Elbette host uygulamanın IIS üzerinde tutulduğu bir senaryoda Mex EndPoint bildirimine gerek yoktur.) Host uygulamanın Main metoduna ait kodlar ise aşağıdaki gibi geliştirilebilir.

```csharp
using System;
using System.ServiceModel;
using System.Workflow.Runtime;
using WFSiparisKutuphanesi;

namespace Sunucu
{
    class Program
    {
        static void Main(string[] args)
        {
            // Workflow servisi için gerekli çalışma ortamının hazırlanmasını WorkflowServiceHost tipi üstlenir
            // Parametre olarak yayınlanacak servis bazlı kullanılacak olan aktivite sınıfı belirtilir.
            WorkflowServiceHost host = new WorkflowServiceHost(typeof(WFSiparis));
            // Host uygulama açıldığından devreye girecek olay metodu
            host.Opened += delegate(object sender, EventArgs arg) 
            {
                Console.WriteLine("Host opened");
            };
            // Host uygulama kapatıldığında devreye girecek olan olay metodu
            host.Closed += delegate(object sender, EventArgs arg)
            {
                Console.WriteLine("Host Closed");
            };
            host.Open(); // Host açılır
            Console.WriteLine("Servis çalışıyor. Kapatmak için bir tuşa basın");
            Console.ReadLine();
            host.Close(); // Host kapatılır
        }
    }
}
```

Artık istemci uygulamanın yazılmasına geçilebilir. Ama öncesinde istemci için gerekli proxy tipinin ve konfigurasyon dosyası içeriğinin üretilmesi gerekmektedir. İki seçenek vardır. Svcutil aracı ve Visual Studio 2008 ortamında ele alınabilen Add Service Reference. Svcutil aracı komut satırından aşağıdaki ekran görüntüsünde yer aldığı gibi kullanılabilir. Tabiki bu işlem sırasında Host uygulamanın çalışıyor olması ve servisin dışarıdan erişilebilir durumda bulunması gerekmektedir.

![mk250_14.gif](/assets/images/2008/mk250_14.gif)

Visual Studio 2008 ortamında Add Service Reference seçeneği yardımıyla da Proxy ve config üretimi gerçekleştirilebilir. Elbette bu yaklaşımda da Host uygulamanın çalışıyor olması gerekmektedir.

![mk250_15.gif](/assets/images/2008/mk250_15.gif)

Örneğimizde yer alan proxy ve config dosyalarının üretimi için Add Service Reference yaklaşımı kullanılmıştır. Yukarıdaki ekran görüntüsündende takip edilebileceği gibi istemci uygulama base address ile belirtilen Url adresi üzerinden servis sözleşmesine erişebilmekte ve yayınlanan servis operasyonlarını görebilmektedir. İstemci uygulamada basit bir Console projesi olarak tasarlanmıştır ve Main metodunun kod içeriği aşağıdaki gibidir.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Istemci.WFSiparisServisi;

namespace Istemci
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Sipariş için bir tuşa basın");
            Console.ReadLine();

            // SiparisVer metodu için gerekli parametre üretilir
            UrunBilgisi urn = new UrunBilgisi() 
                { 
                    Adet = 10, 
                    UrunKodu = "AB-100", 
                    SiparisTarihi = DateTime.Now 
                };

            // Proxy üretimi gerçekleştirilir
            UrunSiparisServisiClient servis = new UrunSiparisServisiClient();
            Console.WriteLine("Talep gönderiliyor");
            // WF Servis operasyonu çağırılır
            string cevap=servis.SiparisVer(urn);
            // Operasyon sonucu gösterilir
            Console.WriteLine(cevap);
    
            Console.ReadLine();
        }
    }
}
```

Önce Host uygulama sonrasında istemci uygulama çalıştırılarak test edildiğinde aşağıdaki ekran görüntüsü elde edilir. Görüldüğü gibi istemci uygulama Workflow operasyonunu başarılı bir şekilde kullanabilmiştir.

![mk250_16.gif](/assets/images/2008/mk250_16.gif)

Hatta birden fazla istemci uygulama çalıştırıldığındada WF servisinin başarılı bir şekilde her istemciye cevap verdiği görülmektedir.

![mk250_18.gif](/assets/images/2008/mk250_18.gif)

Ancak dikkat edilmesi gereken önemli bir durum vardır. Aynı istemci uygulama tarafından ikinci bir sipariş isteği geldiğinde, bir başka deyişle SiparisVer metodu aynı proxy örneği üzerinden ikinci bir kez çağırıldığında ne olacaktır? Bunun için koda aşağıdaki satırları eklediğimizi düşünelim.

```csharp
cevap = servis.SiparisVer(new UrunBilgisi() { Adet = 8, SiparisTarihi = DateTime.Now, UrunKodu = "KL-450" });
Console.WriteLine(cevap);
```

Uygulama test edildiğinde çalışma zamanında ikinci SiparisVer metodu çağrısında aşağıdaki istisna (Exception) mesajının alındığı görülür.(Detaylı hata mesajının istemci tarafındanda görülebilmesi için servis tarafında bilgisinin eklenmiş olması gerekmektedir.)

![mk250_17.gif](/assets/images/2008/mk250_17.gif)

Bunun sebebi gelen talep sonrasında sunucu tarafında ilgili istemci için üretilen Workflow servis örneğinin cevap verdikten sonra artık olmayışıdır. Bu nedenle ikinci talep aslında istemcinin daha önceden kullandığı Workflow servis örneği için yaptığı istektir. Oysaki host uygulama tarafında bu Workflow servis örneğinin işi önceki talebin sonuçlanması ile bitmiştir. (Tabi burada kolaya kaçılarak pratik bir çözüm olarak istemci uygulama içerisinde her operasyon çağrısı öncesinde yeni bir proxy üretimi yoluna gidilebilir ki bu tavsiye edilen bir yol değildir.)

İşte burada uzun süreli durağan olması gereken bir Workflow servisi örneği söz konusudur. Yani Workflow örneğinin durumunu koruması gerekmektedir. Persistence servisleri kullanılarak bu sorun çözümlenebilir. Bu amaçla SqlWorkflowPersistenceService tipinden yararlanılarak ilgili durağanlığın gerçekleştirilmesi sağlanabilir. Tabi bu amaçla öncelikli olarak SQL üzerinde ilgili veritabanının oluşturulması ve tabloların hazırlanması gerekmektedir. Sonrasında ise ilgili PersistenceService'in örneklenip Workflow servisi çalışma ortamına kod yardımıyla yada konfigurasyon bazında bildirilmesi gerekmektedir. Bu konu yazının kapsamı dışına çıktığından burada ele alınmayacaktır.

Böylece geldik bir makalemizin daha sonuna. Bu makalede basit olarak Workflow örneklerinin birer servis olarak nasıl yayımlanabileceğini adım adım incelemeye çalıştık. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](/assets/files/2008/WFServisleri.rar)