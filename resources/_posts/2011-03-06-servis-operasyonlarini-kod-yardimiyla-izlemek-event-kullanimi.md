---
layout: post
title: "Servis Operasyonlarını Kod Yardımıyla İzlemek – Event Kullanımı"
date: 2011-03-06 18:00:00 +0300
categories:
  - wcf
tags:
  - wcf
  - csharp
  - wpf
  - xml
  - iis
  - visual-studio
---
Suyun dibinden bir baloncuk yükselmeye başlar. Taze ve temiz hava ile temas etmek için can atmaktadır. Ancak kat etmesi gereken epey bir mesafe vardır. Daha da hızlanabilmek ister. Yükselirken karşısına bir baloncuk daha çıkar ve doğrudan ona koşar. Artık ikisi birlikte, daha hızlı yükselmektedirler.

[![blg223_Giris](/assets/images/2011/blg223_Giris_thumb.jpg)](/assets/images/2011/blg223_Giris.jpg)


“Birlikten kuvvet doğar” der, alttan gelen ötekine. Ama artık havasızlıktan boğulmak üzeredirler. Neyse ki yollarına başka bir baloncuk daha çıkar. Üç baloncuk birleşerek daha büyük bir tane oluşturur. Süratler artmıştır. Gün ışığına, taze havaya çok, çok az kalmıştır. Ve nihayettt…Hımmmm!!! Temiz hava…

Bu şairane diyemeyeceğim kadar kötü girişten sonra

![Sealed](/assets/images/2011/smiley-sealed.gif)

baloncukların konumuzla ne alakası olduğunu düşünebilirsiniz elbette

![Smile](/assets/images/2011/smiley-smile.gif)

Aslında bu gün irdeleyeceğimiz konunun özünde Olaylar (Events) yer almakta. Ayrıca söz konusu bir olay tanımının, üst tarafta doğru (ama kodun içerisinden baktığımızda da alt tarafa doğru yürüdüğünü düşünebiliriz) aktarılması söz konusu. (WPF tarafında buna sıkça rastlandığını ve Event Bubbling şeklinde ifadeler ile tanımlandığını belirtelim)

[Servis Operasyonlarını Kod Yardımıyla İzlemek](/2011/02/27/servis-operasyonlarini-kod-yardimiyla-izlemek/) başlıklı yazımızda, IIS (Internet Information Services) dışında Self-Hosted modelinde yayınlanan servislere ait operasyon çağrılarının nasıl izlenebileceğini incelemeye çalışmıştık. Geliştirmiş olduğumuz tipler yardımıyla, istemcilerden servis operasyonlarına gelen çağrı öncesi ve sonrası durumları, Text tabanlı olarak dosyaya kaydetmiştik. Text tabanlı dosyaya kaydetme işlemi aslında bir zorlama olarak düşünülebilir. Nitekim söz konusu yazıda uygulanan çözüme göre, operasyon çağrıları ancak text tabanlı dosyaya bakılarak takip edilebilir

![Undecided](/assets/images/2011/smiley-undecided.gif)

Oysaki operasyon çağrılarını işletim sisteminin Event Log’ larına yazdırtmak ya da, XML tabanlı bir dosyaya aktarılmasını sağlamak isteyebiliriz. Hatta söz konusu izlerin veritabanı üzerindeki bir tabloya yazdırılması da düşünülebilir vb… Bir başka deyişle operasyon çağrılarının izlenmesi sırasında oluşan log verilerini, herhangibir kaynağa doğru yazdırmak isteyebiliriz. Böyle bir durumda, tasarlamış olduğumuz tiplerin, onları kullanan object user’ lara alternatif bir yol sunması gerekmektedir. Nitekim oluşturulan log verisinin nereye yazılacağına object user’ ın karar vermesi, çok daha esnek bir izleme yapısı oluşturulmasını sağlayacaktır. Peki bunu nasıl gerçekleştirebiliriz?

Aslında elimizde önemli ve büyük bir koz vardır. Olaylar (Events). Object User’ lar (bir başka deyişle servis operasyonlarını izlemek ile ilişkili kodlamaları yapanlar) tasarladığımız servis davranışı içerisinde tanımlayacağımız olaya/olaylara (Events) abone olaraktan, üretilen log’ ları nereye isterlerse yazdırabilirler.

[![Exclamation](/assets/images/2011/Exclamation_thumb_11.gif)](/assets/images/2011/Exclamation_11.gif) Dolayısıyla yazımızın bundan sonraki kısımlarında olay tabanlı bir geliştirme yapacağımızı belirtmek isterim. Olaylar ile ilişkili olarak [C# Temelleri - Olayları (Events) Kavramak](https://www.buraksenyurt.com/post/C-Temelleri-Olaylar%C4%B1(Events)-Kavramak-bsenyurt-com-dan) başlıklı yazıya bakmanızı ve eski bilgilerinizi tazeleminizi önerebilirim

![Wink](/assets/images/2011/smiley-wink.gif)

Senaryomuzda olay metodlarına bilgi aktarmamız da son derece yerinde olacaktır. Bu amaçla EventArgs tipinden türeyen ve aşağıdaki içeriğe sahip olan iki tip tasarladığımızı düşünebiliriz. Nitekim operasyon çağrısı öncesi ve sonrası durumlara ait bilgileri taşımak niyetinde olduğumuzdan iki ayrı argüman tipi işimizi görebilir (veya ortak üyeleri bir üst tip içerisinde toplayacak bir yol da izleyebilirsiniz)

[![blg223_EventArgsDiagram](/assets/images/2011/blg223_EventArgsDiagram_thumb.gif)](/assets/images/2011/blg223_EventArgsDiagram.gif)

OptAfterCallEventArgs sınıfı EventArgs tipininden türetilmiştir ve özellikle IParameterInspector arayüzü üzerinden gelen AfterCall metodunun parametrik bilgilerini ve ek diğer verileri taşımak üzere tasarlanmıştır. Bu anlamda, operasyona yapılan çağrı sonrasında, hangi servisin hangi operasyonunun icra edilmekte olduğu, hangi değerin geriye döndürüldüğü, output parametrelerinin içeriği ve çağrı zamanı gibi bilgiler ele alınmaktadır. Sınıfın basit içeriği aşağıdaki gibidir.

```csharp
using System;

namespace InspectorLib 
{ 
    public class OptAfterCallEventArgs 
        :EventArgs 
    { 
        public string ServiceName { get; set; } 
        public string OperationName { get; set; } 
        public object ReturnValue { get; set; } 
        public object CorrelationState { get; set; } 
        public object[] Outputs { get; set; } 
        public DateTime CallTime { get; set; } 
    } 
}
```

OptBeforeCallEventArgs sınıfı da benzer şekilde EventArgs tipinden türetilmiştir. Bu sınıfa ait nesne örnekleri ile de, IParameterInspector arayüzü dolayısıyla gelen BeforeCall metodunun parametrik bilgileri ve ek verilerin, ilgili olay metoduna aktarılması hedeflenmektedir. Buna göre hangi servis için hangi operasyon çağrısının yapılacağı, operasyona gelen parametre bilgileri ve operasyonun icra zamanı gibi veriler toplanabilmektedir. Söz konusunu sınıfın basit içeriği de aşağıdaki gibidir.

```csharp
using System;

namespace InspectorLib 
{ 
    public class OptBeforeCallEventArgs 
        :EventArgs 
    { 
        public string ServiceName { get; set; } 
        public string OperationName { get; set; } 
        public object[] Inputs { get; set; } 
        public DateTime CallTime { get; set; } 
    } 
}
```

Bu işlemin ardından IParameterInspector arayüzünü implemente eden yeni izleyicimizin (OptDetective sınıfı) içeriğini de aşağıdaki gibi yazabiliriz.

[![blg223_OptDetectiveDiagram](/assets/images/2011/blg223_OptDetectiveDiagram_thumb.gif)](/assets/images/2011/blg223_OptDetectiveDiagram.gif)

```csharp
using System; 
using System.ServiceModel.Dispatcher;

namespace InspectorLib 
{ 
    public class OptDetective 
        :IParameterInspector 
    { 
        public string ServiceName { get; set; } 
        // Olay tanımlamaları yapılır 
        public event EventHandler<OptAfterCallEventArgs> AfterCalled; 
        public event EventHandler<OptBeforeCallEventArgs> BeforeCalled;

        // Olay metodları 
        public void OnAfterCalled(OptAfterCallEventArgs args) 
        { 
            // Eğer OptAfterCalled olayına abone(Subscribe) olunmuşsa çağır 
            if (AfterCalled != null) 
                AfterCalled(this, args); // Olayı tetikleyen çalışma zamanındaki OptDetective nesne örneği this ile ifade edilirken, olay metoduna taşınacak bilgiler de OptAfterCallEventArgs tipinden olan args değişkeni ile taşınır. 
        } 
        public void OnBeforeCalled(OptBeforeCallEventArgs args) 
        { 
            // Eğer OptBeforeCalled olayına abone(Subscribe) olunmuşsa çağır 
            if (BeforeCalled != null) 
                BeforeCalled(this, args); // Olayı tetikleyen çalışma zamanındaki OptDetective nesne örneği this ile ifade edilirken, olay metoduna taşınacak bilgiler de OptBeforeCallEventArgs tipinden olan args değişkeni ile taşınır. 
        }

        public OptDetective(string serviceName) 
        { 
            ServiceName = serviceName;            
        } 
        #region IParameterInspector Members

        public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState) 
        { 
            // Olay metodu çağırılır ve parametre olarak OptAfterCallEventArgs tipinden nesne örneği verilir. Bu nesne ile gerekli bilgilerin olay metodu içerisine taşınması sağlanmış olunur. 
            OnAfterCalled( 
                new OptAfterCallEventArgs 
                { 
                     ServiceName=this.ServiceName, 
                      CallTime=DateTime.Now, 
                       CorrelationState=correlationState, 
                        OperationName=operationName, 
                         Outputs=outputs, 
                         ReturnValue=returnValue 
                } 
                ); 
        }

        public object BeforeCall(string operationName, object[] inputs) 
        { 
            // Olay metodu çağırılır ve parametre olarak OptBeforeCallEventArgs tipinden nesne örneği verilir. Bu nesne ile gerekli bilgilerin olay metodu içerisine taşınması sağlanmış olunur. 
            OnBeforeCalled( 
                new OptBeforeCallEventArgs 
                { 
                     ServiceName=this.ServiceName, 
                      Inputs=inputs, 
                       CallTime=DateTime.Now, 
                        OperationName=operationName 
                } 
                ); 
            return null; 
        }

        #endregion 
    } 
}
```

OptDetective sınıfı içerisinde iki adet event tanımı yapıldığı görülmektedir. AfterCalled ve BeforeCalled. Ayrıca bu olayların manuel olarak tetiklenmesi için de, OnAfterCalled ve OnBeforeCalled isimli metodlar yazılmıştır. Her iki metod içerisinde de, bağlı oldukları olayların yüklenip yüklenmediği kontrol edilmekte ve eğer yüklüyseler çağırılmaları sağlanmaktadır.

Olaylarların tetiklenmesi sonucu AfterCall ve BeforeCall isimli arayüz metodları çağırılır. Bu çağrılar sırasında da uygun olan argüman tipleri örneklenmekte ve kullanılmaktadır. Örneğin BeforeCall fonksiyonu çağırıldığında, OnBeforeCalled metodu devreye girmekte ve OptBeforeCallEventArgs tipinden nesne örneği ile servis adı, operasyon adı, çağrı zamanı ve operasyon input değerleri gibi bilgiler, olay metoduna aktarılmaktadır.

Daha önceki yazımızdan da hatırlayacağınız üzere, operasyon ve servis seviyesinde kullanılan özel davranış tiplerimiz bulunmaktadır. Bunlarda özel operasyon davranışı olanı, OptDetective tipini kullanacaktır. Ancak daha da önemlisi, özel operasyon davranışının (Custom Operation Behavior), OptDetective üzerindeki olaylara abone olmasıdır. Bu tipi aşağıdaki gibi tasarlayabiliriz.

[![blg223_OptDetectiveOperationDiagram](/assets/images/2011/blg223_OptDetectiveOperationDiagram_thumb.gif)](/assets/images/2011/blg223_OptDetectiveOperationDiagram.gif)

```csharp
using System; 
using System.ServiceModel.Description;

namespace InspectorLib 
{ 
    public class OptDetectiveOperationBehavior 
        :IOperationBehavior 
    { 
        // Olay tanımlamaları 
        public event EventHandler<OptAfterCallEventArgs> ServiceOperationAfterCalled; 
        public event EventHandler<OptBeforeCallEventArgs> ServiceOperationBeforeCalled;

        // Olay metodlar 
        public void OnServiceOperationAfterCalled(OptAfterCallEventArgs args) 
        { 
            // ServiceOperationAfterCalled olayı yüklenmişse, bu olay sonrası devreye girecek metodu çağır 
            if (ServiceOperationAfterCalled != null) 
                ServiceOperationAfterCalled(this, args); 
        } 
        public void OnServiceOperationBeforeCalled(OptBeforeCallEventArgs args) 
        { 
            // ServiceOperationBeforeCalled olayı yüklenmişse, bu olay sonrası devreye girecek metodu çağır 
            if (ServiceOperationBeforeCalled != null) 
                ServiceOperationBeforeCalled(this, args); 
        }

        #region IOperationBehavior Members

        public void AddBindingParameters(OperationDescription operationDescription, System.ServiceModel.Channels.BindingParameterCollection bindingParameters) 
        { 
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.ClientOperation clientOperation) 
        { 
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.DispatchOperation dispatchOperation) 
        { 
            string serviceName = null; 
            if (dispatchOperation.Parent.Type != null) 
            { 
                // Service adı yakalanır 
                serviceName = dispatchOperation.Parent.Type.Name; 
                //OptDetective tipine ait nesne örneği oluşturulur ve servis adı yapıcı metoduna parametre olarak gönderilir. 
                OptDetective optDtcv = new OptDetective(serviceName); 
                // OptDetective nesne örneğinin AfterCalled ve BeforeCalled olaylarına abone olunur. Anonymouse Method' lar yardımıyla da ilgili olay metodları tetiklenir 
                optDtcv.AfterCalled += (snd,arg) => { OnServiceOperationAfterCalled(arg); }; 
                optDtcv.BeforeCalled += (snd, arg) => { OnServiceOperationBeforeCalled(arg); }; 
                // Operasyon için gerekli inspector tipi bildiririlir 
                dispatchOperation.ParameterInspectors.Add(optDtcv);  
            } 
        }

        public void Validate(OperationDescription operationDescription) 
        { 
        }

        #endregion 
    } 
}
```

Dikkat edileceği üzere özel operasyon davranışı içerisinde, OptDetective tarafından sunulan AfterCalled ve BeforeCalled olaylarına abonelik işlemi gerçekleştirilmektedir. Bunun için ApplyDispatchBehavior metodu içerisinde gerekli düzenlemeler yapılmıştır. Diğer yandan OptDetectiveOperationBehavior tipinin kendisi de, iki adet olay bildirimi yapmaktadır. ServiceOperationAfterCalled ve ServiceOperationBeforeCalled. Benzer bir olay bildirim ve uygulama şekli özel servis davranışı için de yapılmalıdır. Aynen aşağıda görüldüğü gibi.

[![blg223_OptDetectiveServiceDiagram](/assets/images/2011/blg223_OptDetectiveServiceDiagram_thumb.gif)](/assets/images/2011/blg223_OptDetectiveServiceDiagram.gif)

```csharp
using System; 
using System.ServiceModel.Description;

namespace InspectorLib 
{ 
        public class OptDetectiveServiceBehavior 
        :IServiceBehavior 
    { 
        // Olay tanımlamaları 
        public event EventHandler<OptAfterCallEventArgs> ServiceOperationAfterCalled; 
        public event EventHandler<OptBeforeCallEventArgs> ServiceOperationBeforeCalled;

        // Olay metodlar 
        public void OnServiceOperationAfterCalled(OptAfterCallEventArgs args) 
        { 
            // ServiceOperationAfterCalled olayı yüklenmişse, bu olay sonrası devreye girecek metodu çağır 
            if (ServiceOperationAfterCalled != null) 
                ServiceOperationAfterCalled(this, args); 
        } 
        public void OnServiceOperationBeforeCalled(OptBeforeCallEventArgs args) 
        { 
            // ServiceOperationBeforeCalled olayı yüklenmişse, bu olay sonrası devreye girecek metodu çağır 
            if (ServiceOperationBeforeCalled != null) 
                ServiceOperationBeforeCalled(this, args); 
        }

        #region IServiceBehavior Members

        public void AddBindingParameters(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters) 
        { 
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase) 
        { 
            foreach (ServiceEndpoint endpoint in serviceDescription.Endpoints) 
                foreach (OperationDescription operation in endpoint.Contract.Operations) 
                { 
                    // Custom Operation Behavior nesne örneği tanımlanır. 
                    OptDetectiveOperationBehavior optBevahior = new OptDetectiveOperationBehavior(); 
                    // Bu nesne örneğinin ServiceOperationAfterCalled ve ServiceOperationBeforeCalled olaylarına abone olunur 
                    optBevahior.ServiceOperationAfterCalled += (snd, args) => { OnServiceOperationAfterCalled(args); }; 
                    optBevahior.ServiceOperationBeforeCalled += (snd, args) => { OnServiceOperationBeforeCalled(args); };                    
                    operation.Behaviors.Add(optBevahior); 
                } 
        }

        public void Validate(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase) 
        { 
        }

        #endregion 
    } 
}
```

OptDetectiveServiceBehavior tipi daha önceki yazımızdaki örnekten de bilindiği üzere servis davranışı olarak uygulanmaktadır. Bu tip içerisinde de AfterCall ve BeforeCall takipleri için birer olay metodu tanımlandığı görülmektedir. Ayrıca, ApplyDispatchBehavior metodu içerisinde yer alan döngüde, her bir servis operasyonu için gerekli operasyon davranışı bildirimi yapılırken, bu tipe ait olaylara da abone olunduğu görülmektedir.

Tanımladığımız bu yeni tipler olay bazlı bir modeli kullanmaktadır. Teorimize göre OptDetectiveServiceBehavior tipini kullanarak yeni bir davranış yükleyen servisler, OptDetective içerisinde yer alan olay metodlarına erişebiliyor olmalıdır. Tabi bir önceki yazımızdan farklı olarak özel servis ve operasyon davranış tiplerinin nitelik (Attribute) bazlı olmadıkları görülmektedir. Bu nedenle ServiceHost nesne örneği için kod tarafında bilinçli olarak gerekli davranış bildirimleri yapılmalıdır. Daha önceki yazımızda kullandığımız örnek üzerinden ilerlediğimiz için aynı servis host uygulamasını ve servis tiplerini kullanabiliriz. Bu amaçla Form kodlarını aşağıdaki gibi geliştirdiğimizi düşünebiliriz.

```csharp
using System; 
using System.ServiceModel; 
using System.Windows.Forms; 
using AdventureWorksFinance; 
using InspectorLib;

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

            OptDetectiveServiceBehavior behavior = new OptDetectiveServiceBehavior(); 
            if (host.Description.Behaviors.Find<OptDetectiveServiceBehavior>() == null) 
            { 
                host.Description.Behaviors.Add(behavior); 
            } 
            // OptDetectiveServiceBehavior nesne örneği oluşturulup, servis davranışı olarak eklendikten sonra ilgili olay metodlarına abone olunur. 
            behavior.ServiceOperationAfterCalled += (snd, arg) => { 
                string info = String.Format("{0} {1} {2} {3} [{4}]", arg.ServiceName, arg.OperationName, arg.ReturnValue, arg.CallTime,"After Call"); 
                lstOperationCalls.Items.Add(info); 
            }; 
            behavior.ServiceOperationBeforeCalled += (snd, arg) => { 
                string info = String.Format("{0} {1} {2} {3} [{4}]", arg.ServiceName, arg.OperationName, arg.Inputs.Length, arg.CallTime,"Before Call"); 
                lstOperationCalls.Items.Add(info); 
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

Dikkat edileceği üzere servis davranışı yüklendikten sonra gerekli olay metodları da bağlanmıştır. Burada AfterCall veya BeforeCall olayları gerçekleştirildiğinde sembolik olarak ListBox kontrolüne bir takım bilgiler yazdırılmıştır. Bu bilgilerin OptAfterCallEventArgs ve OptBeforeCallEventArgs tiplerinden geldiği unutulmamalıdır

![Wink](/assets/images/2011/smiley-wink.gif)

Geliştirici bu noktada tamamen özgürdür. Sonuçta olayla ilişkili bilgiler, en alttaki OptDetective tipinden, buradaki olay metodlarına kadar taşınmaktadır. Bu bilgiler istenilen şekilde kullanılabilir. Yani ListBox içeriğine yazdırılmaları şart değildir. Daha önceden de belirttiğimiz gibi veritabanına, XML dosyasına, sistem Event Log’ larına vb yerlere yazılabilirler.

Uygulamanın çalışma zamanı sonuçlarına baktığımızda aşağıdaki ekran görüntüsünde yer alan çıktı ile karşılaşırız. Yuppiii!!!

![Laughing](/assets/images/2011/smiley-laughing.gif)

[![blg223_Runtime](/assets/images/2011/blg223_Runtime_thumb.gif)](/assets/images/2011/blg223_Runtime.gif)

Görüldüğü gibi olay metodları başarılı bir şekilde devreye girmiş ve istemcinin GetTotalSalaryByDepartment ve GetTotalGains operasyonlarına yaptığı çağrılara ilişkin bazı bilgiler elde edilmiştir. Peki bu işleyiş sırasındaki çağrı hiyerarşisi nedir? Aslında olaylar nesneler üzerinden birbirlerine aktarılmaktadır. Bu aktarım işlemlerinin gerçekleşmesi içinse, OptDetectiveServiceBehavior isimli servis davranışı içerisinden OptDetectiveOperationBehavior isimli operasyon davranışı tipinin olaylarına ve OptDetectiveOperationBehavior içerisinden de OptDetective tipi içerisindeki olaylara abone olunmuştur. Bu noktada kodu debug ederek ilerleminizde yarar olacaktır. Bu şekilde geçişleri çok rahar takjp edebileceğinizi görebilirsiniz. Bu kutsal görevi de siz değerli okurlarıma bırakıyorum

![Wink](/assets/images/2011/smiley-wink.gif)

Biraz uzun bir anlatım oldu sanırım. Ama umuyorum ki işinize yarayacaktır. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[OperationCallTracingV2.rar (181,95 kb)](/assets/files/2011/OperationCallTracingV2.rar) [Örnek Visual Studio 2010 Ultimate sürümünde geliştirilmiş ve test edilmiştir]