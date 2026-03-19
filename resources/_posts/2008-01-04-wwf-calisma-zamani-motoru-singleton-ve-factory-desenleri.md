---
layout: post
title: "WWF : Çalışma Zamanı Motoru, Singleton ve Factory Desenleri"
date: 2008-01-04 12:00:00 +0300
description: "Bir önceki makalemizde iş akışı(Workflow) kavramını anlamaya çalışmış, Windows Workflow Foundation(WWF) mimarisini yüzeysel olarak incelemiş ve basit bir kaç Sequential Workflow örneği geliştirmiştik. Bu makalemizde ise WWF mimarisinin sunduğu çalışma zamanı ortamını derinlemesine kavramaya çalışacağız. Ağırlıklı olarak üzerinde durmaya çalışacağımız konu ise WorkflowRuntime sınıfı olacak."
categories:
  - wf
tags:
  - wf
  - csharp
  - dotnet
  - aspnet
  - workflow-foundation
  - wpf
  - threading
  - performance
  - transactions
  - delegates
  - visual-studio
---
# WWF : Çalışma Zamanı Motoru, Singleton ve Factory Desenleri
Değerli Okurlarım Merhabalar,

Bir önceki makalemizde iş akışı (Workflow) kavramını anlamaya çalışmış, Windows Workflow Foundation (WWF) mimarisini yüzeysel olarak incelemiş ve basit bir kaç Sequential Workflow örneği geliştirmiştik. Bu makalemizde ise WWF mimarisinin sunduğu çalışma zamanı ortamını derinlemesine kavramaya çalışacağız. Ağırlıklı olarak üzerinde durmaya çalışacağımız konu ise WorkflowRuntime sınıfı olacak.

İş akışları herhangibir.Net uygulaması tarafından host edilebilmektedir. Burada amaç ana uygulamanın belirli iş kurallarını ve mantığını barındıran süreç veya süreçleri tutarlı bir şekilde ele alabilmesi ve kullanabilmesidir. Çok doğal olarak iş akışları, host uygulama göz önüne alınırken ortaya bir iş akışı çalışma zamanı (Workflow Runtime) çıkmaktadır. Buda Windows Workflow Foundation içerisinde yer alan önemli bileşenlerden birisi anlamına gelmektedir. WWF mimarisi içerisinde pek çok bileşen (Components) yer almaktadır. Büyük resme bakıldığında başrol oynayan bileşenler aşağıdaki şekilde görüldüğü gibidir.

![mk238_1.gif](/assets/images/2008/mk238_1.gif)

Sınıf kütüphanaleri (Class Libraries) iş akışı (workflow) uygulamaları geliştirebilmek (Develop), yönetebilmek (Manage), çalışma zamanı hareketliliklerini izleyebilmek (Trace) için gerekli tüm tip tipleri içermektedir. Bunların dışında ana bileşenler için gerekli tiplerde burada yer almaktadır. Söz gelimi önceden tanımlanmış çalışma zamanı servisleri (Runtime Services) gibi. WF tasarım zamanı araçları sayesinde, iş akışlarının Visual Studio ortamında kolay bir şekilde geliştirilebilmesi sağlanmaktadır. WF Çalışma zamanı servisleri (WF Runtime Services) iş akışları için hayati önemi olan yada opsiyonel olarak ek avantajlar kazandıran bazı hizmetleri içermektedir. Bu yapıya yazımızın ilerleyen kısımlarında yeniden bakıyor olacağız.

WWF içerisinde yer alan çekirdek çalışma zamanı motoru (Core Runtime) WorkflowRuntime sınıfı tarafından temsil edilmektedir. Ne varki WorkflowRuntime sınıfı tek başına anlamlı olabilecek bir yapıda değildir. Bu nedenle iş akışlarının (Workflows) çalışma zamanını yönetebilmesi için, bir host uygulama tarafından ele alınması gerekmektedir. Bu host uygulama tahmin edileceği gibi herhangibir.Net uygulaması olabilir.

- WPF (Windows Presentation Foundation) Uygulamaları
- Windows Uygulamaları
- Windows Servisleri
- Asp.Net Web Uygulamaları
- Console Uygulamaları

Buna göre host uygulama iş akışı çalışma zamanını (Workflow Runtime) barındırırken, iş akışı çalışma zamanıda iş akışı örneklerini (Workflow Instances) barındırmaktadır. Söz konusu durumu aşağıdaki şekil ile daha net bir biçimde görebiliriz.

![mk238_2.gif](/assets/images/2008/mk238_2.gif)

Bir WWF ortamı oluşturulduğunda host uygulama içerisinde ele alınan pek çok çalışma zamanı kavramıda söz konusudur. İş akışı örnekleri, bu örneklerin içerisinde yer alan aktiviteler (Activities), kurallar (Rules), iş akışı çalışma zamanı içerisinde yer alan bazı unsurlar, servisler vb...Aşağıdaki şekilde host uygulama açısından bakıldığında, çalışma zamanı ortamı içerisinde söz konusu olan unsurlar temsil edilmeye çalışılmaktadır.

![mk238_3.gif](/assets/images/2008/mk238_3.gif)

Dikkat edilmesi gereken noktalardan bir taneside host uygulamanın birden fazla WF çalışma zamanı ortamına sahip olabilmesidir. Buna göre tek bir uygulama alanı (Single Application Domain) kendi içerisinde birden fazla WF çalışma zamanı ortamı barındırabilir. Bu WF çalışma zamanı ortamlarının her biride, kendi iş akışlarını bağımsız olacak şekilde yönetebilir. Nitekim WF çalışma zamanı motoru (WF Runtime Engine) birden fazla iş akışı örneğini (Workflow Instance) yönetecek şekilde tasarlanmıştır.

> Burada vurgulanması gereken bir husus vardır. Windows Workflow Foundation ile ilgili yazılmış bazı kitaptlarda tek bir uygulama alanı (Single Application Domain) içerisinde sadece tek bir WF çalışma zamanı (WF Runtime) olabileceği belirtilmektedir. Ancak bu durum WWF'in relase edilmiş sürümünde ortadan kaldırılmıştır.

Host uygulama bir WorkflowRuntime nesne örneğini oluşturduktan sonra temel olarak aşağıdaki şekilde görülen olaylar meydana gelmektektedir.

![mk238_4.gif](/assets/images/2008/mk238_4.gif)

Görüldüğü gibi ilk olarak Host uygulama WorkflowRuntime örneğini oluşturmaktadır. Aynı zamanda çalışma zamanı için gerekli olan servislerde oluşturulmakta ve WF Runtime ortamına kayıt edilmektedir (Register). Buradaki servislerin bazıları mecburidir. Bazıları ise opsiyonel olarak ele alınmaktadır. Host uygulama ayrıca, WF Runtime tarafından tetiklenen olayları ele alabilmekte ve bu şekilde WF çalışma zamanını daha kolay izleyebilmektedir.

Workflow çalışma motorunun en önemli görevlerinden birisi elbetteki iş akışlarını başlatmaktır. Zaten iş akışların kendiliğinden başlatılmasında mümkün değildir. Bu noktada devreye WorkflowRuntime girmektedir. Başlatılan iş akışları WF çalışma zamanı motoru (Runtime Engine) tarafından ayrı bir thread içerisinde asenkron (Asynchronous) olarak yürütülürler. Birden fazla iş akışı WF çalışma zamanı motoru tarafından başlatılıp yönetilebilmektedir. Kaç iş akışı olursa olsun, host uygulamadaki ana thread'e paralel olacak şekilde asenkron (asynchronous) olarak yürütülebilmektedir. Başlatılan her iş akışı WF çalışma zamanı motoru (WF Runtime Engine) tarafından izlenmektedir. Buna bağlı olarak çalışma zamanında bir iş akışının durumlarının ele alınabileceği bir takım olaylar söz konusudur. Aşağıdaki tabloda yer alan olaylar WorkflowRuntime sınıfına aittir.

Olay (Event)
Açıklama

WorkflowCreated
Bir Workflow nesne örneği oluşturulduğunda tetiklenen olaydır. Bu olay, iş akışına ait aktiviteler çalışmaya başlamadan önce, yapıcı metod (Constructor) çalıştıktan sonra tetiklenmektedir.

WorkflowStarted
Bir iş akışı çalışmaya başladığında tetiklenen olaydır. Bir başka deyişle iş akışı içerisindeki root aktivite (Activity) icrasına başladığında tetiklenir.

WorkflowLoaded
WF Çalışma zamanı motoru tarafından bir iş akışı belleğe yüklendiğinde, iş akışına ait aktiviteler çalışmaya başlamadan önce tetiklenen olaydır.

WorkflowUnloaded
İş akışı örneği WF çalışma zamanı motoru tarafından bellekten kaldırıldığında tetiklenen olaydır. Çoğunlukla bir iş akışı uzun süre bir şey yapmadan kaldığında (Idle) ve WorkflowPersistenceService hizmeti eklenip UnloadOnIdle metodu geriye true döndürüyorsa otomatik olarak tetiklenmektedir.

WorkflowCompleted
İş akışı örneği tamamlandığında devreye giren olaydır. Bu olay geliştirici tarafından mutlaka ele alınmalıdır.

WorkflowAborted
İş akışı iptal edildiğinde tetiklenir. Bir iş akışı WorkflowInstance sınıfının Abort metodu yardımıyla manuel olaraktanda iptal edilebilir.

WorkflowTerminated
Bir iş akışı ele alınmayan (alınamayan) bir istisna (Exception), Terminate metoduna yapılan bir çağrı yada TerminateActivity aktivitesi nedeniyle sonlandığında tetiklenir. Bu olayda geliştirici tarafından mutlaka ele alınmalıdır.

WorkflowSuspended
Bir iş akışı Suspend metoduna yapılan çağrı ile, SuspendActivity aktivitesine gelinmesi nedeniyle duraksayabilir. Ama gereken durumlarda WF çalışma zamanı motoru dinamik iş akışları arasında geçişleri yaparken geçici olarak duraksatmalara başvurabilir. Bu durumlarda WorkflowSuspended olayı tetiklenir.

WorkflowIdled
İş akışları çalışma zamanı motoru tarafından iş yapmadan bırakıldığında tetiklenir. Söz gelimi DelayActivity aktivitesi ile karşılaşıldığında iş akışı idle durumuna geçecektir. Bu noktada WorkflowIdled isimli olay tetiklenmektedir.

WorkflowResumed
Suspend modda olan bir iş akışı tekrar çalışmaya başladığında tetiklenen olaydır. Resume metoduna yapılan çağrı sonucu yada WF çalışma zamanı motoru tarafından geçici olarak Suspend moda alınan bir iş akışı tekrar kaldığı yerden devam etmeye başladığında bu olay tetiklenir.

WorkflowPersisted
Eğer WF çalışma zamanına eklenmiş bir kalıcı bırakma hizmeti (Persistence Service) yüklüyse, iş akışının güncel durumu belleğe kaydedildikten sonra bu olay tetiklenebilir. Ancak bu söz konusu olayın tetiklenme nedenlerinden sadece birisidir.

Started
WF çalışma zamanı motoru nesnesi başlatıldığında tetiklenen olaydır.

Stopped
WF çalışma zamanı motoru nesnesi durdurulduğunda tetiklenen olaydır.

WF çalışma zamanı motoru, iş akışları üzerindeki yönetimini daha moduler bir şekilde gerçekleştirmek için bazı servislerden yararlanmaktadır. WF ortamında çalışma zamanı servisleri (WF Runtime Services) olarak tanımlanan söz konusu hizmetler çekirdek (Core) ve yerel (Local) olmak üzere iki ana kategoride ele alınırlar. Çekirdek servisler (Core Services) 4 ana hizmetten oluşmaktadır ve WF sınıf kütüphanelerinde (Class Libraries) içerisinde tanımlanmış olan sınıflar ile ifade edilmektedir. Bu hizmetler aşağıdaki grafikte görüldüğü gibidir.

![mk238_5.gif](/assets/images/2008/mk238_5.gif)

WF çalışma zamanı motoru tarafından ele alınan bu çekirdek servislerden Commit Work Batch ve Scheduling hizmetleri mutlaka olmak zorundadır. Ancak Tracking ve Persistence hizmetlerinin kullanımı tamamen opsiyoneldir.

> Opsiyonel olan servisleri WF çalışma ortamına yüklemek için şu adımlar izlenir;
> - Servise ait nesne örneği oluşturulur.
> - WorkflowRuntime sınıfına ait nesne örneğinin AddService metodu kullanılarak, ilgili servisin çalışma ortamına eklenmesi sağlanır.

Scheduling hizmeti temel olarak çalışma zamanında iş akışlarının icra edileceği thread'lerin oluşturulması ve yönetilmesi ile ilgilidir. Bu hizmetler WorkflowSchedulerService isimli taban sınıftan (base class) türemektedir. WorkflowSchedulerService abstract bir sınıtfır.

> Bilindiği gibi abstract sınıflar normal sınıflar gibi metodlar içerebilen, örneklenemeyen, türetme amacıyla kullanılan ve türeyen sınıfların mutlaka ezmesi gereken üyeleri barındırabilen, polimorfik (polymorphic) özellik gösterebilen.Net tiplerindendir.

Eğer aksi söylenmesse WF çalışma zamanı motoru (WF Workflow Runtime Engine) varsayılan olarak DefaultWorkflowSchedulerService sınıfına ait bir örneği kullanılır. Bu örnek sayesinde birden fazla iş akışının thread havuzu (Pool) içerisinde kuyruk temelli olarak yönetilmesi otomatik olarak sağlanmaktadır. Elbette burada ikinci bir seçenek daha vardır ki buda ManualWorkflowSchedulerService sınıfıdır. Bu servis kullanıldığında iş akışlarının yürüyeceği thread'in oluşturulması host uygulamaya aittir. Ayrıca burada tek bir thread söz konusu olduğundan işlemler senkron (Synchronous) olarak yürümektedir. Bu nedenle ManualWorkflowSchedulerService çoğunlukla host uygulama Asp.Net ile geliştirilmişse tercih edilmektedir.

Commit Work Batch hizmeti dahili iş akışları ile harici veri saklama ortamları arasındaki tutarlılığı (Consistency) sağlamak adına WF çalışma zamanı motorunun transaction'ları yönetmesini sağlayan bir servistir. Temel olarak WorkflowCommitWorkBatchService isimli abstract sınıftan türeyen tiplerce karşılanırlar. Çekirdek olarak yazılmış iki tip vardır. DefaultWorkflowCommitWorkBatchService ve SharedConnectionWorkflowCommitWorkBatchService. Tahmin edileceği üzere varsayılan olarak kullanılan hizmet DefaultWorkflowCommitWorkBatchService sınıfına ait örnek ile sağlanmaktadır. SharedConnectionWorkflowCommitBatchService sınıfı kullanıldığında, Persistence ve Tracking servisleri için aynı SQL bağlantısının kullanılabilmesi sağlanmaktadır. Böylece farklı nesneler (objects) arasında aynı SQL bağlantısını kullanan transaction'ların yönetilmesi sağlanabilmektedir.

Tracking servisi yardımıyla iş akışlarının başından geçenlerin izlenmesi sağlanabilmektedir. Taban sınıf (Base Class) TrackingService'dir. Abstract olan bu sınıfın WF çalışma ortamı için yazılmış herhangibir varsayılan implemantasyonu yoktur. Nitekim bilinçli olarak SqlTrackingService sınıfı kullanılabilir. SqlTrackingService sealed bir sınıftır. Bu nedenle kendisinden türetme yapılamamaktadır. Dolayısıyla geliştirici tarafından genişletilmesi mümkün değildir. Bu tip bir amaç varsa doğrudan TrackingService sınıfından türetme yapılmasında yarar vardır. SqlTrackingService hizmeti ile iş akışlarına ait çalışma zamanı izlenimleri SQL veritabanında saklanabilmektedir.

Persistence servisi ile çalışma zamanındaki iş akışlarının herhangibir noktada kalıcı olarak saklanabilmeleri hedeflenmektedir. Burada taban sınıf rolünü abstract WorkflowPersistenceService üstlenmektedir. Tracking hizmetinde olduğu gibi bu servisinde varsayılan bir implementasyonu yoktur. Hizmetin kullanılabilmesi için SqlWorkflowPersistenceService sınıfının çalışma zamanında WF ortamına eklenmesi gerekmektedir. Bu hizmet yardımıyla bir iş akışının çalışma zamanındaki durumu SQL veritabanlarına kaydedilebilir ve istenildiği zaman ilgili tablolardan çalışma ortamına yüklenebilir. (Çekirdek servislerinin detaylarını ve kullanımını ilerleyen makalalerimizde incelemeye çalışacağız.)

> Geliştiriciler çekirdek servislere ait taban sınıfları (Base Class) kullanarak özelleştirilmiş hizmetlerde yazabilirler. Bunun için abstract olan çekirdek hizmet (Core Service) sınıflarından türetme (Inherit) yapmak yeterlidir.

Gelelim yerel hizmetlere (Local Services). Çoğunluka veri değiş tokuş hizmetleri (Data Exchange Services) olarak adlandırılan bu servislerde amaç, host uygulama ile iş akışı örnekleri arasında bir iletişim kanalı oluşturmaktır. Bu anlamda bazı yardımcı aktivite tipleri kullanılır. Söz gelimi CallExternalMethodActivity bileşeni yardımıyla bir iş akışı içerisinden yerel bir servise ait metod çağrılabilir. Yada HandleExternalEventActivity bileşeni yardımıyla iş akışının yerel servis içerisindeki bir olayın tetiklenmesini beklemesi sağlanabilir.

Yazılmış olan bir yerel servisin çalışma ortamına eklenmesi için ExternalDataExchangeService sınıfından yararlanılır. Önce bu sınıfa ait bir örnek oluşturlur ve WorkflowRuntime nesnesine eklenir. Kullanılmak istenen yerel servis örnekleri ExternalDataExchangeService örneğine ilave edilirler. (Kendi yerel servislerimizi nasıl geliştirebileceğimizi ilerleyen makalelerde incelemeye çalışacağız.)

Buraya kadar WF çalışma zamanının başrol oyuncularını çok kısada olsa tanımaya çalıştık. Artık ağırlıklı olarak WF çalışma zamanı motoru görevini üstlenen WorkflowRuntime sınıfını örnek üzerinde irdelemeye çalışacağız. WorkflowRuntime sınıfının, çalışma zamanı ortamını yönetmek için kullanılan pek çok üyesi (Member) bulunmaktadır. Olaylarından (Events) daha önceden bahsettiğimiz WorkflowRuntime sınıfının tüm üyeleri aşağıdaki sınıf diagramında (Class Diagram) görüldüğü gibidir.

![mk238_6.gif](/assets/images/2008/mk238_6.gif)

AddService metodu yardımıyla WF çalışma zamanı ortamına çekirdek (Core) veya yerel (Local) servislerin eklenmesi mümkündür. AddService metodu ile WF çalışma zamanı ortamına yüklenen servisler RemoveService fonksiyonu kullanılaraktan kaldırılabilirler. Çalışma ortamında, WorkflowRuntime nesnesi tarafından kullanılan servislerin tamamı istenirse GetAllServices metodunun aşırı yüklenmiş versiyonları yardımıyla elde edilebilir.

En çok kullanılan üyelerden olan CreateWorkflow metodu ile bir iş akışının o andaki WF çalışma ortamı içerisinde oluşturulması sağlanır. Bu metod çağrısının önemli bir özelliği daha vardır. İş akışı örneği oluşturulurken eğer WF çalışma zamanı çalışmıyorsa, başlatılmasını sağlamaktadır. Başlatma işlemi aslında StartRuntime metodu ile gerçekleştirilmektedir. Dolayısıyla CreateWorkflow metodunun çalışmayan bir WF ortamına rastladığında yaptığı çağrıda StartRuntime fonksiyonu içindir. StartRuntime bilinçli olarak dışarıdan da çağırılabilir.

StartRuntime nasılki bir WF çalışma ortamının başlatılmasını sağlıyorsa StopRuntime metoduda durdurulmasında rol oynamaktadır. StartRuntime metoduna yapılan çağrılar WF çalışma ortamı için kullanılan servislerin başlatılmasını sağlarken, StopRuntime metoduna yapılan çağrılarda söz konusu servislerin durdurulmasında rol oynar. WF çalışma zamanı ortamında istenirse yürümekte (Executing), saklanmakta (Persisted) veya asılı halde (Idled) olan bir iş akışının (Workflow) referansının elde edilmesi sağlanabilir. Metodları dışında WorkflowRuntime sınıfının sadece iki özelliği bulunmaktadır. Bunlardan IsStarted özelliği yardımıyla, WF çalışma zamanı ortamının başlatılıp başlatılmadığı bilgisi true veya false olarak öğrenilebilmektedir.

Şimdi WorkflowRuntime nesnesini kod üzerinden incelemeye başlayabiliriz. İlk olarak yazımızın başında bahsettiğimiz gibi tek bir uygulama alanı (Single Application Domain) içerisinde birden fazla WorkflowRuntime nesnesi olabileceğini ispat etmeye çalışarak başlayacağız. Örneklerimizde herhangibir iş akışı (Workflow) örneği kullanılmayacaktır. Bu nedenle basit bir Console uygulaması üzerinden hareket edebiliriz. Sonuç itibaryle WorkflowRuntime nesnesi bilindiği gibi bir host uygulamada kullanıldığı takdirde anlamlı olmaktadır. Bu amaçla Visual Studio 2008 ortamında bir Console uygulaması açalım ve.Net Framework 3.0 assembly'larından System.Runtime.Workflow.dll'ini projeye referans edelim.

![mk238_9.gif](/assets/images/2008/mk238_9.gif)

Sonrasında ise ana uygulama kodlarını aşağıdaki gibi geliştirelim.

```csharp
using System;
using System.Workflow.Runtime;

namespace WorkflowRuntimeInceleme
{
    class Program
    {
        static void Main(string[] args)
        {
            // WorkflowRuntime nesne örneklerinden ilki oluşturulur.
            WorkflowRuntime rt1 = new WorkflowRuntime();
            rt1.Name = "WF1"; // WorkflowRuntime için bir isim belirlenir
    
            // Started ve Stopped olayları yüklenir
            rt1.Started += new EventHandler<WorkflowRuntimeEventArgs>(WfStarted);
            rt1.Stopped += new EventHandler<WorkflowRuntimeEventArgs>(WFStopped);

            // WorkflowRuntime nesne örneklerinden ikincisi oluşturulur.
            WorkflowRuntime rt2 = new WorkflowRuntime();
            // WF Runtime için isim belirlenir.
            rt2.Name = "WF2";
            // Started ve Stopped olayları yüklenir
            rt2.Started+=new EventHandler<WorkflowRuntimeEventArgs>(WfStarted);
            rt2.Stopped+=new EventHandler<WorkflowRuntimeEventArgs>(WFStopped);

            // WorkflowRuntime örnekleri StartRuntime metodu ile başlatılır
            rt1.StartRuntime(); 
            rt2.StartRuntime();

            // WorkflowRuntime örnekleri StopRuntime metodu ile durdurulur
            rt1.StopRuntime();
            rt2.StopRuntime();
        }
    
        // WorkflowRuntime başlatıldığında devreye giren olay metodu
        static void WfStarted(object sender, WorkflowRuntimeEventArgs e)
        {
            // İki WorkflowRuntime nesne örneğide aynı Started olayına bağlandığından hangisi olduğunun tespiti için sender' dan yararlanılır
            WorkflowRuntime guncelWF = sender as WorkflowRuntime;
            // IsStarted özelliği ile WF Çalışma zamanının başlatılıp başlatılmadığı öğrenilir.
            Console.WriteLine(guncelWF.Name + " " + e.IsStarted);
        }

        // WorkflowRuntime durdurulduğunda devreye giren olay metodu
        static void WFStopped(object sender, WorkflowRuntimeEventArgs e)
        {
            // İki WorkflowRuntime nesne örneğide aynı Started olayına bağlandığından hangisi olduğunun tespiti için sender' dan yararlanılır
            WorkflowRuntime guncelWF = sender as WorkflowRuntime;
            Console.WriteLine(guncelWF.Name+" "+e.IsStarted);
        }
    }
}
```

Örnekte iki adet WorkflowRuntime nesnesi oluşturulmakta ve önce StartRuntime metod çağrıları ile başlatılmaktadır. Sonrasında ise StopRuntime metod çağrıları ile ilgili WF çalışma zamanı ortamları durdurulmaktadır. Bu durum geçişlerini kolay takip edebilmek için her iki WorkflowRuntime nesne örneği aynı Started ve Stopped olaylarına bağlanmıştır. Uygulama çalıştırıldığında aşağıdaki ekran görüntüsü elde edilir.

![mk238_7.gif](/assets/images/2008/mk238_7.gif)

Görüldüğü gibi aynı uygulama içerisinde birden fazla WorkflowRuntime nesne örneğinin çalıştırılabileceği ortadadır. Her ne kadar birden fazla WorkflowRuntime oluşturulması avantajlı görünsede bazı durumlarda çalışma zamanında tek bir örneğin olması istenebilir. Bu çoğunlukla performans kazanımları sağlayacak bir durumdur. Nitekim söz konusu ihtiyaç hazırlanma maliyeti yüksek olan nesnelerin bir tane olmasını sağlamakta kendini göstermektedir.

Bu noktada WWF'i bir kenara bırakıp "Bir nesnenin çalışma zamanında tek (Single) olmasını sağlamak için ne yapılır?" sorusuna cevap aramak gerekmektedir. Neyseki Gang-Of-Four (GOF) sayesinde yıllar önce bu tip bir ihtiyaç için Singleton Tasarım Deseni (Singleton Design Pattern) ortaya çıkmıştır. Şimdi tekrar WWF tarafında dönersek WorkflowRuntime nesnelerinin tek olmasını sağlamak için Singleton desenini kullanabileceğimizi düşünebiliriz. Lakin bu sadece nesnenin tek olmasını sağlayan bir desendir. Oysaki WorkflowRuntime nesnesinin üretiminide düşünmemiz gerekmektedir. Bu sebepten Factory tasarım deseninide işin içerisine katmak anlamlı olacaktır. Bir başka deyişle Singleton ve Factory tasarım desenlerinin bir karışımı burada göz önüne alınabilir.

> Singleton ve Factory tasarım desenlerinin bir arada kullanılması çalışma zamanında sadece tek bir Singleton nesnesinin üretilmesini sağlamaktadır. Esasında Abstract Factroy, Builder ve Prototype gibi desenler kendi içlerinde Singleton desenini barındıran çözümler sunmaktadır.

Bu amaçla uygulamaya aşağıdaki gibi bir sınıf eklediğimizi düşünebiliriz.

![mk238_8.gif](/assets/images/2008/mk238_8.gif)

```csharp
using System;
using System.Workflow.Runtime;

namespace WorkflowRuntimeInceleme
{
    public static class WorkflowRuntimeFabrikasi
    {
        // Singleton WorkflowRuntime örneği hazırlanır.
        private static WorkflowRuntime _wfRt = null;
        // Lock işleminde kullanılmak üzere static bir object oluşturulur.
        private static object _kilit = new object();

        public static WorkflowRuntime WorkflowRuntimeUret()
        {
            // Multi-Thread uygulamalar göz önüne alınarak senkron çalıştırma için lock kullanılır.
            lock (_kilit)
            {
                // Eğer daha önceden bir WorkflowRuntime nesnesi oluşturulmamışsa
                if (_wfRt == null)
                {
                    _wfRt = new WorkflowRuntime(); // WorkflowRuntime nesnesini üret 
                    // Process' den çıkıldığında
                    AppDomain.CurrentDomain.ProcessExit+=delegate(object obj,EventArgs e)
                                                                {
                                                                    WorkflowKapat(_wfRt); // Workflow ortamını kapatma metodunu çalıştır
                                                                };
                    // Application Domain Unload edildiğinde
                    AppDomain.CurrentDomain.DomainUnload += delegate(object obj, EventArgs e)
                                                                {
                                                                    WorkflowKapat(_wfRt); // Workflow ortamını kapatma metodunu çalıştır
                                                                };
                    _wfRt.StartRuntime(); // WorkflowRuntime başlatılır
                }
                return _wfRt;
            }
        }
        private static void WorkflowKapat(WorkflowRuntime rt)
        {
            // Eğer WorkflowRuntime nesnesi null değilse ve başlatılmışsa
            if (rt != null
                && rt.IsStarted)
            {
                try
                {
                    rt.StopRuntime();// WorkflowRuntime' ı durdurmayı dene
                }
                catch // istisna(Exception) oluşursa görmezden gel
                {
                }
            }
        }
    }
}
```

WorkflowRuntimeFabrikası isimli sınıf static olarak tasarlanmıştır. Bu nedenle çalışma zamanında kendisine ait bir örnek oluşturulması mümkün değildir. Buda standart Singleton deseninde yer alan private yapıcı metodun (Constructor) yazılma zorunluluğunu ortadan kaldırmaktadır. Bununla birlikte static sınıfların (static classes) bir özelliği olarak sadece static üyeler içerebilir. Sınıf içerisinde dikkat edileceği üzere WorkflowRuntime nesnesi static bir alan (Field) olarak tanımlanmaktadır. Host uygulamanın ihtiyacı olan WorkflowRuntime nesne örneğinin üretim işlemini static olarak tanımlanmış olan WorkflowRuntimeUret metodu üstlenmektedir.

Dikkat edileceği üzere söz konusu metod içerisinde wfRt isimli WorkflowRuntime nesnesinin null olup olmadığı kontrol edilemektedir. Eğer null ise üretim yapılmakta ve metoddan geriye döndürülmektedir. Buda zaten aynı metodun ikinci veya diğer çağrılarında wfRt'nin tek olmasını garanti etmektedir. Singleton deseninin kullanımında multi-thread uygulamalar söz konusu ise üretim yapan metod içerisinde senkronize etme işlemleri yapılması önerilmektedir. Bu sebepten dolayı çok basit olarak lock kullanımı ile thread'in senkronize edilerek çalıştırılması sağlanmaktadır. Herhangibir host uygulama içerisinde bu sınıf kullanılmak istendiği takdirde, WorkflowRuntime nesnelerini üretmek için aşağıdaki gibi bir kod parçasını yazmak yeterli olacaktır.

```csharp
WorkflowRuntime wfRt1 = WorkflowRuntimeFabrikasi.WorkflowRuntimeUret();
```

Elbette ikinci bir WorkflowRuntime nesneside üretilmek istenebilir. Bu yapıldığı takdirde aşağıdaki flash animasyonunda olduğu gibi ilk oluşturulan örneğin kullanılmasına devam edilmesi sağlanmış olunur.(Flash versiyon 7.0 olup boyut 650 Kb'tır)

Böylece geldik bir makalemizin daha sonuna. Bu makalemizde Workflow çalışma zamanı ortamını tanımaya çalışırken, WorkflowRuntime sınıfına ait nesne örnekleri için Singleton ve Factroy desenlerini karma olarak nasıl kullanabileceğimizi inceledik. Ağırlıklı olarak WorkflowRuntime sınıfı, üyeleri ve kullanım şekli üzerinde durmaya çalıştık. Öğrendiklerimizi aşağıdaki maddeler ilede göz önüne alabiliriz.

- Workflow çalışma zamanı motoru (WF Runtime Engine), iş akışlarının başlatılması, yönetilmesi, bunlarla ilişkili servislerin yüklenmesi ve daha pek çok fonksiyonellikten sorumlu ana WWF bileşenlerinden birisidir.
- WF Çalışma zamanı motorunun kullanımı ancak bir Host uygulama söz konusu olduğunda anlamlıdır.
- WF Runtime Engine'in sınıf karşılığı olan WorkflowRuntime tipine ait birden fazla nesne örneği aynı uygulama alanı (Application Domain) içerisinde kullanılabilir.
- Bir Application Domain içerisinde tek bir Singleton WorkflowRuntime nesnesinin kullanımı garanti edilebilir. Bunun için Singleton ve Factory desenelerinden yararlanılır.
- WF Çalışma zamanı ortamını içerisinde gerçekleşen durumları daha iyi ele alabilmek için WorkflowRuntime sınıfına pek çok olay (Event) ilave edilmiştir.
- WorkflowRuntime sınıfına ait metodlar ile, servis başlatmak, servisleri elde etmek, iş akışı başlatmak, servis kaldırmak gibi pek çok WF çalışma zamanı işlevselliği gerçekleştirilebilir.

İlerleyen makalalerimizde Windows Workflow Foundation ile ilgili farklı konulara değinmeye devam ediyor olacağız. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](/assets/files/2008/WorkflowRuntimeInceleme.rar)