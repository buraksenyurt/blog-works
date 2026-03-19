---
title: "Servis Operasyonlarını Kod Yardımıyla İzlemek"
pubDate: 2011-02-27 16:20:00
categories:
  - WCF
  - WCF 4.0
tags:
  - WCF
  - WCF 4.0
  - C#
  - XML
  - Windows Forms
  - HTTP
  - IIS
  - Threading
  - Concurrency
  - Visual Studio
---

# Servis Operasyonlarını Kod Yardımıyla İzlemek
[![blg219_Giris](images/blg219_Giris_thumb.jpg)](images/blg219_Giris.jpg) Merhaba Arkadaşlar,

Savaş meydanlarında ezelden beri uygulanan istihbarat ve haber alma teknikleri, avantaj sağlamak açısından en önemli unsurların başında gelmektedir. Karşı tarafın nerede olduğunu izlemek, ne yaptığını bilmek, istenen bir anda ne kadar ateş gücüne sahip olduğunu tespit etmek çok önemlidir. Örneğin karşı tarafın şu anda denizde hareket etmekte olan veya karasal alanda saklanmakta olan kuvvetlerini görmek istediğimiz durumlarda keşif yapılması ve istihbarat toplanması stratejik anlamda önemli avantajlar sağlayacaktır.

Yakın zamana kadar bu işler için yüksek irtifalardan, ses hızının bir kaç kat üstünde uçan uçaklar görev alırdı ki halen daha pek çok hava kuvvetlerince kullanılmaktadır. Örneğin bir zamanlar Birleşik Devletler ile Sovyetler Birliği arasında krize neden olan U2’ ler, şekli ve harcadığı yakıt ile ün salıp emekli olan SR71 Blackbird’ ler vs…

Ancak artık uzaktan kumanda edilebilen, düştüklerinde pilot kaybı yaşanmasını engelleyen, insansız olmaları nedeni ile çok yüksek irtifalara kadar çıkabilen İnsansız Hava Araçları (Unmanned Aeiral Vehicle) var. Aslında uzun süredir varlar (Üstelik ben Generals oyununda da sıklıkla bunları kullanıyorum ![Wink](images/smiley-wink.gif) Hatta geçtiğimiz günlerde bizimde gurur kaynağımız olan ANKA isimli insanız hava aracımız, ilk kez hangardan çıkarak uçuşunu gerçekleştirdi. Peki bu İHA’ ların bu günkü yazımız ile bir bağlantısı var mı? Pek olduğunu söyleyemeyiz.

Aslında bu günkü yazımızda bir WCF (Windows Communication Foundation) servisinin operasyonlarına gelen çağrılar hakkında istihbarat toplamaya çalışıyor olacağız. Ancak bunun için standart Trace ve Monitoring özellikleri yerine kod yardımıyla ilerleyeceğiz. Normal şartlarda IIS (Internet Information Services) üzerinden yayınlanan bir WCF servisine gelen operasyon çağrılarını izlemek son derece kolaydır. Bu amaçla konfigurasyon dosyasında gerekli ayarların yapılması yeterlidir. Diğer taraftan istenirse Windows Server AppFabric ile IIS üzerine gelen eklentilerden yararlanarak bu tip izleme ayarları kolaylıkla yapılabilir.

Lakin bizim amacımız geliştirme safhasında Self-Hosted tekniğine göre yayınlanan servis operasyonlarına gelen çağrıları yakalamak ve basit bilgiler almaktır. Geliştireceğimiz bu senaryonun bize sağlayacağı önemli artılar da vardır. İlk etapta kod yardımıyla özelleştirilmiş bir servis davranışının çalışma zamanındaki ServiceHost örneğine nasıl entegre edilebileceği öğrenilecektir. Diğer yandan operasyonlara olan taleplerin ve dönüşlerin çok basit mana da izlenmesi sağlanacak ve karmaşık Trace çıktıları arasında kaybolunmayacaktır. Dilerseniz hiç vakit kaybetmeden işlemlerimize başlayalım. Başlangıçta aşağıdaki sınıf diagramında (Class Diagram) görülen yapıya sahip bir servis kütüphanemiz olduğunu düşünelim.

[![blg219_ServiceClassDiagram](images/blg219_ServiceClassDiagram_thumb.gif)](images/blg219_ServiceClassDiagram.gif)

Buna göre IAccountService isimli Servis Sözleşmemizin (Service Contract) içeriği aşağıdaki gibidir.

```csharp
using System.ServiceModel;

namespace AdventureWorksFinance 
{ 
    [ServiceContract(Name="AdventureWorks.FinanceServices", Namespace="http://AdventureWorks/Finance/Services")] 
    public interface IAccountService 
    { 
        [OperationContract] 
        double GetTotalSalaryByDepartment(string departmentCode);

        [OperationContract] 
        double GetTotalGains(); 
    } 
}

Diğer taraftan söz konusu servis sözleşmesini uygulayan AccountService sınıfının içeriği de aşağıdaki gibidir.

namespace AdventureWorksFinance 
{ 
    public class AccountService 
        : IAccountService 
    { 
        #region IAccountService Members

        public double GetTotalSalaryByDepartment(string departmentCode) 
        { 
            return 1245000; 
        }

        public double GetTotalGains() 
        { 
            return 2500000; 
        }

        #endregion 
    } 
}
```

Bu servis içeriğine sahip olan kütüphanemiz (WCF Service Library), bir WinForms uygulamasına referans edilmiş durumdadır. WinForms uygulaması Host program olarak düşünülebilir. Yani ServiceHost tipi yardımıyla AccountService hizmetini dış dünyaya sunmak üzere tasarlanmaktadır. Bu amaçla arayüzümüzü aşağıdaki gibi tasarladığımızı düşünebiliriz.

[![blg219_WinForm](images/blg219_WinForm_thumb.gif)](images/blg219_WinForm.gif)

Buna göre kullanıcı Open başlıklı düğmeye basarak servisi yayına sunacak ve Close düğmesi ile de kapatacaktır. WinForms uygulaması aşağıdaki app.config içeriğine sahiptir.

```xml
<?xml version="1.0" encoding="utf-8" ?> 
<configuration> 
    <system.serviceModel> 
        <behaviors> 
            <serviceBehaviors> 
                <behavior name="TcpServiceBehavior"> 
                    <serviceMetadata/> 
                    <serviceDebug includeExceptionDetailInFaults="false" /> 
                </behavior> 
            </serviceBehaviors> 
        </behaviors> 
        <services> 
            <service name="AdventureWorksFinance.AccountService" behaviorConfiguration="TcpServiceBehavior"> 
               <endpoint address="" binding="netTcpBinding" bindingConfiguration="" 
                    contract="AdventureWorksFinance.IAccountService"> 
                </endpoint> 
                <endpoint address="mex" binding="mexTcpBinding" bindingConfiguration="" 
                    contract="IMetadataExchange" /> 
                <host> 
                    <baseAddresses> 
                        <add baseAddress="net.tcp://localhost:4505/AdventureWorksFinance/AccountService/" /> 
                    </baseAddresses> 
                </host> 
            </service> 
        </services> 
    </system.serviceModel> 
</configuration>
```

Servisimiz TCP protokolüne göre yayın yapmaktadır. Ayrıca MexTcpBinding bağlayıcı tipini kullanarak Metadata yayını da yapmaktadır. Dolayısıyla istemciler kendileri için gerekli Proxy tiplerini üretmek amacıyla WSDL içeriğine ulaşabilirler.

Gelelim WinForms’ un başlangıçta arka plandaki kodlarına (Tam anlamıyla Buton Arkası Kodlarına ![Sealed](images/smiley-sealed.gif))

```csharp
using System; 
using System.ServiceModel; 
using System.Windows.Forms; 
using AdventureWorksFinance;

namespace HostApp 
{ 
    public partial class Form1 
        : Form 
    { 
        ServiceHost host=null;

        public Form1() 
        { 
            InitializeComponent(); 
            btnOpen.Enabled = true; 
            btnClose.Enabled = false; 
        }

        private void btnOpen_Click(object sender, EventArgs e) 
        { 
            host=new ServiceHost(typeof(AccountService)); 
            host.Opened += (s, ea) => 
            { 
                lstOperationCalls.Items.Add(String.Format("Servis açık {0}",DateTime.Now.ToLongTimeString())); 
                btnOpen.Enabled = false; 
                btnClose.Enabled = true; 
            }; 
            host.Closed += (s, ea) => 
            { 
                lstOperationCalls.Items.Add(String.Format("Servis kapatıldı {0}", DateTime.Now.ToLongTimeString())); 
                btnOpen.Enabled = true; 
                btnClose.Enabled = false; 
            }; 
            host.Open(); 
        }

        private void btnClose_Click(object sender, EventArgs e) 
        { 
            host.Close(); 
        } 
    } 
}
```

Çok basit olarak özetlemek gerekirse, ServiceHost sınıfına ait nesne örneği üretilirken AccountService tipini parametre olarak almaktadır. Yani AccountService servisi yayına alınmaktadır. Bu servis üzerinden Open ve Close işlemlerinin uygulanması halinde Opened ve Closed olay metodları devreye girmektedir.

Kahveleriniz yanınızda değil mi?

![Laughing](images/smiley-laughing.gif)

Artık asıl işlemlerimize başlayabiliriz. İlk olarak içeriği aşağıda görülen ve IParameterInspector arayüzünü (Interface) uygulayan bir sınıf geliştirerek yola çıkalım.

[![blg219_TextFileInspector](images/blg219_TextFileInspector_thumb.gif)](images/blg219_TextFileInspector.gif)

```csharp
using System; 
using System.IO; 
using System.ServiceModel.Dispatcher; 
using System.Text;

namespace InspectorLib 
{ 
    public class TextFileInspector 
        :IParameterInspector 
    { 
        public string ServiceName { get; set; } 
        private string _textFilePath = Path.Combine(Environment.CurrentDirectory, "OperationCallLog.txt"); 
        private StringBuilder builder = new StringBuilder();

        public TextFileInspector(string serviceName) 
        { 
            ServiceName = serviceName;            
        } 
        #region IParameterInspector Members

        public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState) 
        { 
            builder.AppendLine(String.Format("After Call -> Service {0}, Time {1}, Operation Name {2}", ServiceName,DateTime.Now.ToLongTimeString(), operationName)); 
            builder.AppendLine("Return Value"); 
            builder.AppendLine(returnValue.ToString());

            StreamWriter writer = File.AppendText(_textFilePath); 
            writer.WriteLine(builder.ToString()); 
            writer.Close(); 
        }

        public object BeforeCall(string operationName, object[] inputs) 
        { 
            builder.AppendLine(String.Format("Before Call -> Service {0}, Time {1}, Operation Name {2}",ServiceName,DateTime.Now.ToLongTimeString(),operationName)); 
            builder.AppendLine("Inputs"); 
            foreach (object input in inputs) 
            { 
                builder.AppendLine(inputs[0].ToString()); 
            }

            StreamWriter writer = File.AppendText(_textFilePath); 
            writer.WriteLine(builder.ToString()); 
            writer.Close();

            return builder.ToString(); 
        }

        #endregion 
    } 
}
```

TextFileInspector sınıfı, IParameterInspector arayüzünü (Interface) uygulamaktadır. Bu arayüz görüldüğü üzere AfterCall ve BeforeCall isimli iki metodun ezilmesini istemektedir. BeforeCall metodu ile operasyona yapılan çağrıya ait bilgiler yakalanabilir. Aslında operasyon başlatılmadan önce, yakalandığı yer olarakta düşünebiliriz. AfterCall metodu hangi operasyona çağrı yapılığına dair operationName parametresini kullanmaktadır. Diğer yandan object dizisi tipinden olan inputs parametresi ile, çağrıda bulunulan operasyona gelen giriş değerleri öğrenilebilir. Benzer şekilde AfterCall fonksiyonunu da, operasyondan istemciye cevap dönerken değerlendirilebilir. Böylece istemciye hangi operasyon için hangi değerin döndürüldüğü yakalanabilir. Her iki metod da, Text tabanlı bir dosyaya bilgi yazmakta ve raporlamayı bu şekilde gerçekleştirmektedir. Ama bu bir zorunluluk değildir. Burada ListBox içeriğine bilgilendirme amaçlı öğe eklenmesi de sağlanabilir, bir veritabanı tablosuna yazma işlemi de gerçekleştirilebilir.

Çok doğal olarak bu sınıfın çalışma zamanına bir şekilde öğretilmesi gerekmektedir. Bu noktada servis metodları ve servise söz konusu tipin bildirilmesi gerektiğini düşünebiliriz. Servis operasyonları için bu tip bir davranış uygulanacağı nitelikler (Attributes) yardımıyla kolayca öğretilebilir

![Wink](images/smiley-wink.gif)

Dolayısıyla ilk olarak servis operasyonları için TextFileInspector tipinin bir davranış olarak bildirilmesi gerekmektedir. Bu amaçla aşağıdaki sınıfı tasarlamamız yeterli olacaktır.

[![blg219_TextFileInspectorOperationBehavior](images/blg219_TextFileInspectorOperationBehavior_thumb.gif)](images/blg219_TextFileInspectorOperationBehavior.gif)

```csharp
using System; 
using System.ServiceModel.Description;

namespace InspectorLib 
{ 
    [AttributeUsage(AttributeTargets.Method)] 
    public class TextFileInspectorOperationBehavior 
        :Attribute,IOperationBehavior 
    { 
        #region IOperationBehavior Members

        public void AddBindingParameters(OperationDescription operationDescription, System.ServiceModel.Channels.BindingParameterCollection bindingParameters) 
        { 
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.ClientOperation clientOperation) 
        { 
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.DispatchOperation dispatchOperation) 
       { 
            if(dispatchOperation.Parent.Type!=null) 
                dispatchOperation.ParameterInspectors.Add(new TextFileInspector(dispatchOperation.Parent.Type.Name));   
        }

        public void Validate(OperationDescription operationDescription) 
        { 
        }

        #endregion 
    } 
}
```

TextFileInspectorOperationBehavior sınıfı, metodlara uygulanabilen bir operasyonel davranış tipi olarak tasarlanmıştır. Bu nedenle IOperationBehavior arayüzünü uygulamaktadır. Bu arayüzün senaryomuza göre en önemli metodu ise ApplyDispatchBehavior fonksiyonudur. Bu fonksiyon içerisinde dikkat edileceği üzere TextFileInspector isimli sınıfımıza ait bir nesne örneğinin ParameterInspectors koleksiyonuna eklendiği görülmektedir. Dolayısıyla operasyonların yakalanması sırasında kullanılacak olan tipin bildirimi sağlanmıştır. Buna göre çalışma zamanında TextFileInspectorOperationBehavior niteliğinin uygulandığı servis operasyonlarına gelen çağrılarda, hangi Inspector tipinin devreye gireceği bildirilmiş olmaktadır.

Aslında bu nitelik bildirimi yeterlidir. Nitekim servis operasyonlarına söz konusu niteliğin uygulaması ile zaten çalışma zamanı bilgilendirilmiş olacaktır. Ancak bir servisin tüm operasyonlarının izlenmesi istendiği bir durum söz konusu ise tüm operasyonların başında TextFileInspectorOperationBehavior uygulamak yerine, sınıfın başında bir nitelik ile servis davranışının bildirilmesi daha doğrudur. Bu nedenle servis sınıfına uygulanabilen bir davranışın bildirilmesi önemlidir. İşte bu amaçla aşağıdaki sınıfı yazmamız yeterli olacaktır.

[![blg219_TextFileInspectorServiceBehavior](images/blg219_TextFileInspectorServiceBehavior_thumb.gif)](images/blg219_TextFileInspectorServiceBehavior.gif)

```csharp
using System; 
using System.ServiceModel.Description;

namespace InspectorLib 
{ 
    [AttributeUsage(AttributeTargets.Class)] 
    public class TextFileInspectorServiceBehavior 
        :Attribute,IServiceBehavior 
    { 
        #region IServiceBehavior Members

        public void AddBindingParameters(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters) 
        { 
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase) 
        { 
            foreach (ServiceEndpoint endpoint in serviceDescription.Endpoints) 
                foreach (OperationDescription operation in endpoint.Contract.Operations) 
                    operation.Behaviors.Add(new TextFileInspectorOperationBehavior()); 
        }

        public void Validate(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase) 
        { 
        }

        #endregion 
    } 
}
```

Attribute olarak sınıflara uygulanabilen bir tip söz konusudur. Buna göre örneğimizde yer alan AccountService sınıfına uygulanabilecek bir tiptir. Ayrıca IServiceBehavior arayüzünün bir implementasyonu söz konusudur ki buna göre yazılması gereken en önemli metod ApplyDispatchBehavior fonksiyonudur. Bu fonksiyon içerisinde servis içerisindeki tüm Endpoint’ ler dolaşılmaktadır. Bu Endpoint’ lerin her birinin de sözleşmelerinde tanımlanmış olan operasyonlarına gidilmektedir. Yani OperationContract niteliği uygulanan metodlarına ulaşılır. Bulunan her bir operasyon için de, az önce tanımlanan TextFileInspectorOperationBehavior davranış örneği bildirimi yapılmaktadır.

[![Exclamation](images/Exclamation_thumb_8.gif)](images/Exclamation_8.gif) Bu arada TextFileInspector, TextFileInspectorOperationBehavior ve TextFileInspectorServiceBehavior sınıflarını ayrı bir kütüphane altında toplamamızda yarar vardır. Nitekim söz konusu tipleri Host uygulama da ve servis kütüphanesinde kullanmamız gerekmektedir.

Peki ne servis sınıfında ne de operasyon metodlarında yukarıda tanımlanmış olan nitelikler kullanılmazsa ve biz yine de TextFileInspector tipini kullandırtmak istersek. Bu durumda örneğin Host uygulama tarafında aşağıdaki kodlamaları yapmamız yeterli olacaktır.

```csharp
TextFileInspectorServiceBehavior tfServiceBehavior = new TextFileInspectorServiceBehavior(); 
TextFileInspectorServiceBehavior sb = host.Description.Behaviors.Find<TextFileInspectorServiceBehavior>(); 
if (sb == null) 
    host.Description.Behaviors.Add(tfServiceBehavior);

host.Open();
```

Görüldüğü üzere ServiceHost nesne örneği üzerinden Open metodu çağırıldıktan sonra servise TextFileInspectorServiceBehavior davranışının uygulanıp uygulanmadığına bakılmaktadır. Eğer uygulanamışsa Behaviors koleksiyonuna Add metodu ile ekleme işlemi gerçekleştirilmektedir.

Şimdi gelelim operasyon izleme işleminin niteliklerimiz yardımıyla nasıl bildirileceğine. Bu amaçla AccountService sınıfını aşağıdaki gibi güncellememiz yeterli olacaktır.

```csharp
using InspectorLib;

namespace AdventureWorksFinance 
{ 
    //[TextFileInspectorServiceBehavior] 
    public class AccountService 
        : IAccountService 
    { 
        #region IAccountService Members

        [TextFileInspectorOperationBehavior] 
        public double GetTotalSalaryByDepartment(string departmentCode) 
        { 
            return 1245000; 
        }

        [TextFileInspectorOperationBehavior] 
        public double GetTotalGains() 
        { 
            return 2500000; 
        }

        #endregion 
    } 
}
```

Bu örnek kullanımda operasyon metodlarının başında TextFileInspectorOperationBehavior niteliği uygulanmıştır. Böylece çalışma zamanına hangi operasyonların izleneceği bildirilmiş olmaktadır. Ancak istersek sınıfın başındaki TextFileInspectorServiceBehavior niteliğini etkinleştirerek, servis içerisindeki tüm metodların, TextFileInspectorOperationBehavior uygulanmadan izlenebilmesi de sağlanabilir

![Wink](images/smiley-wink.gif)

Şimdi senaryomuzu test etmeye başlayabiliriz. Tabi bu amaçla basit bir istemci uygulama yazmamız ve servisimizi referans etmemiz gerekmektedir. Burada Self-Hosted bir servis çalışma zamanı söz konusu olduğundan, proxy üretimi için host uygulamanın öncelikli olarak çalışıtırlması ve servisin açılması gerektiği unutulmamalıdır. Bu işlemin ardından Mex talebi yapılabilir.

[![blg219_AddServiceReference](images/blg219_AddServiceReference_thumb.gif)](images/blg219_AddServiceReference.gif)

Test amacıyla aşağıdaki kodları yazabiliriz.

```csharp
using System; 
using ClientApp.AdventureWorksFinance; 
using System.Threading;

namespace ClientApp 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            Console.WriteLine("Teste başlamak için servisin açık olduğundan emin olunuz ve bir tuşa basınız"); 
            Console.ReadLine(); 
            AdventureWorksFinanceServicesClient proxy = new AdventureWorksFinanceServicesClient("NetTcpBinding_AdventureWorks.FinanceServices"); 
            double result=proxy.GetTotalSalaryByDepartment("IT"); 
            Console.WriteLine("{0}",result.ToString()); 
            Thread.Sleep(1000); 
            double totalGains=proxy.GetTotalGains(); 
            Console.WriteLine("{0}",totalGains.ToString()); 
        } 
    } 
}
```

Öncelikli olarak host ve sonrasında istemci uygulamayı çalıştırdığımızı düşünecek olursak aşağıdaki ekran görüntüsündekine benzer sonuçlarla karşılaşırız.

[![blg219_Runtime](images/blg219_Runtime_thumb.gif)](images/blg219_Runtime.gif)

Ancak bizim için önemli olan ve ulaşmak istediğimiz nokta text dosyası içerisine atılan bilgilerdir. İşte sonuçlar.

[![blg219_RuntimeResult](images/blg219_RuntimeResult_thumb.gif)](images/blg219_RuntimeResult.gif)

Görüldüğü üzere GetTotalSalaryByDepartment ve GetTotalGains metodlarına yapılan çağrıların öncesi ve sonrasına ait bilgiler için OperationCallLog.txt isimli dosya oluşturulmuş ve içeriği üretilmiştir.

Sonuç olarak operasyonlara yapılan çağrıları izlemek ve istihbarat toplamak adına buradaki senaryodan yararlanılabilir. Elbette senaryonun geliştirilmesi gereken pek çok yanı vardır. Söz gelimi şu anki örnekte

- dosya tabanlı IO işlemlerinin sayısı oldukça fazla olabilir. Özellikle eş zamanlı gelecek operasyon çağrılarında StreamWriter tipinin exception verme olasılığı (Paylaşılan dosya kaynağı nedeni ile) an meselesidir ![Sealed](images/smiley-sealed.gif)
- Dosya tabanlı olarak yapılan veri toplama işlemi yerine, veritabanı odaklı bir çözümde geliştirilebilir. Yani operasyon çağrılarına ait bilgiler dosya yerine bir veritabanı tablosuna yazdırılabilir.
- Senaryo tek bir servis örneğini izleyecek şekilde çalışmaktadır. Oysaki host uygulamanın birden fazla servisi dış dünyaya sunma olasılığı bulunmaktadır.

Bu gibi durumları iyice irdeleyerek ilerlemekte yarar vardır. İşte size çalışmak için bir sürü ev ödevi

![Wink](images/smiley-wink.gif)

Bu arada örnek Solution içerisinde ikinci bir servis daha yer almaktadır. Bu serviside devreye alarak operasyon izleme işlemlerini birden fazla servis için gerçeklemeye çalışabilirsiniz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[OperationCallTracing.rar (155,62 kb)](assets/OperationCallTracing.rar) [Örnek Visual Studio 2010 Ultimate Sürümünde Geliştirilmiş ve Test Edilmiştir]
