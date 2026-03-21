---
layout: post
title: "SQL Persistence Hizmeti"
date: 2009-03-06 12:00:00 +0300
categories:
  - wf
tags:
  - workflow-foundation
---
Workflow Foundation yardımıyla kod akışlarının modellenebilmesi ve herhangibir.Net uygulaması içerisinden host edilerek çalıştırılabilmesi mümkündür. Bu kavram işin içerisine servisler girdiğinde çok daha genişlemektedir. Nitekim servisler yardımıyla Workflow örneklerinin host edildikleri uygulama dışındaki ortamlar ile haberleşebilmeleri mümkün olmaktadır. Hatta servislerin kendi içlerinde Workflow aktivitilerini kullanabilmeleri ve böylece belirli kod akışlarını yürütebilmeleride mümkündür. Bu cümleden sonra durup düşünüldüğünde Workflow Foundation kavramının akışları, platform bağımsız ortamlara taşıyabileceği sonucuda ortaya çıkmaktadır. Diğer taraftan Workflow uygulamaları sadece dış ortamlar ile haberleşmek için servislerden yararlanmazlar. İlaveten, Workflow çalışma zamanını (Runtime) ilgilendiren ve özellikle aktivitilerin dayanıklı olarak saklanmasını (Durable Persistence), çalışma hayatlarının izlenmesini (Tracking), adımlar arası geçişlerin özel olarak planlanmasını (Scheduling) sağlayan ve.Net Framework içerisinde önceden tanımlanmış servislerde söz konusudur.

> Workflow servisleri, çalışma zamanına eklenerek workflow örneklerine yeni kabiliyetler kazandırılmasını sağlamak amacıyla kullanılırlar. Örneğin PersistenceServices ve TrackingServices, SQL veritabanlarını varsayılan olarak kullanan çalışma zamanı servisleridir. PersistenceServices ile workflow'ların kalıcı olarak saklanabilmesi sağlanabilir. TrackingServices yardımıylada çalışma zamanı workflow örneklerinin izlenebilmesi mümkündür. Bir başka örnek olarak workflow çalışma zamanı davranışlarını değiştirmemizi sağlayan Manual Scheduler servisi göz önüne alınabilir. Bu servislerin bir kısmının kullanılabilmesi için, çalışma zamanına bilinçli olarak eklenmeleri gerekmektedir. Bir başka deyişle etkinleştirilmeleri gerekir.

İşte bu yazımızdaki konumuz, uzun süreli çalışma ihtimali olan bir Workflow'un belirli koşullarda kalıcı olarak fiziki bir ortamda saklanmasının, SQL Persistence Service yardımıyla nasıl gerçekleştirilebileceğidir. SQL Persistence Service sayesinde, bir Workflow'un faaliyetsiz kalması (Idle) halinde bellek yerine, tablo bazlı bir ortamda saklanabilmesi ve bu durum sona erdiğinde söz konusu depolama alanından tekrar ayağa kaldırılarak çalışmaya devam etmesi mümkün olmaktadır. Konuyu daha kolay kavrayabilmek adına ilk önce Workflow çalışma zamanının kendi ve yönettiği WF örnekleri ile ilişkili yaşam döngüsünü incelemekte yarar vardır. Bu yaşam döngüsünün kolayca ele alınabilmesi için WorkflowRuntime sınıfı içerisine çeşitli olaylar eklenmiştir.

Bilindiği üzere WorkflowRuntime sınıfı çalışma zamanında WF örneklerinin yönetiminden sorumludur. Bu yönetim işlemi sırasında WorkflowRuntime sınıfı üzerinden ele alınabilecek 14 farklı olay metodu vardır. Söz konusu olayların bir kısmı sadece çalışma zamanını ilgilendirirken, bir kısmıda WF örneklerinin yaşam döngülerine (LifeCycle) adanmıştır. Buna göre ServicesExceptionNotHandled, Started ve Stopped olayları Workflow çalışma zamanı olayları olarak düşünülebilir.

Workflow Çalışma Zamanı Olayları

ServicesExceptionNotHandled
Workflow çalışma zamanı servislerinden herhangibirinde kontrol altına alınmamış bir istisna (Exception) oluştuğunda devreye giren olaydır.

Started
Workflow çalışma zamanı motoru, üzerine eklenmiş servisler ile başarılı bir şekilde başlatıldığında devreye girer. Burada çalışma zamanına eklenen servislerin başarılı bir şekilde başlatıldıklarına dair bir bilgilendirme yapması söz konusudur.

Stopped
Started olayına benzer olaraktan, WF çalışma zamanı motorunun, kendi üzerinde yer alan ve çalışmakta olan tüm servislerin başarılı bir şekilde durdurulması sonrasında tetiklenir. Servisler başarılı bir şekilde durdurulduklarına dair WF çalışma zamanı motoruna bilgilendirmede bulunurlar.

Aşağıdaki tabloda açıklamaları verilmiş olan olaylar ise, WF çalışma zamanının yönettiği Workflow örneklerinin durumlarının (State) değiştiği hallerde tetiklenmektedir.

Workflow Örneğine Adanmış Olaylar

WorkflowAborted
Workflow örneği devre dışı bırakıldığında tetiklenir. Özellikle Persistence servisi kullanıldığında önem kazınır. Nitekim devre dışı bırakılan servisin kalıcı olarak saklanması ve tekrar kaldığı yerden ayağa kaldırılması (Resume) mümkün olabilmektedir.

WorkflowCompleted
Bir Workflow örneği tamamlanıp bellekten henüz kaldırılmadan önce devreye giren olaydır. Bu olaya ait metod yardımıyla host uygulamaya, tamamlanan Workflow örneğinin output parametrelerini döndürmek mümkündür.

WorkflowCreated
Workflow örneği oluşturulduğunda ancak Start metodu ile çalıştırılmadan az önce tetiklenir.

WorkflowIdled
Bir workflow örneği dışarıdan beklediği bir etki veya Delay aktivitesi nedeni ile içerisinde yer alan herhangibir aktiviteyi işletmediği durumlarda tetiklenir. Özellikle Idle olma durumunda Persistence hizmetlerinin kullanımı önem kazanır.

WorkflowLoaded
Persistence servisi kullanıldığı durumlarda, Workflow örneğinin herhangibir aktivitesi çalıştırılmadan önce ve belleğe yüklenmesi sonrasında devreye giren olaydır.

WorkflowPersisted
Persistence servisin kullanılması halinde bir workflow örneğinin saklanmak üzere kaydedilmesi sonrasında tetiklenir. Workflow persistence servisi varsayılan olarak SQL veritabanını kullandığından, söz konusu kaydetme işlemi tablo üzerinde gerçekleştirilmektedir.

WorkflowResumed
Bir erteleme nedeni ile Suspended moda geçen bir örneğin tekrar ayağa kalkması sonrasında ve kaldığı yerden devam ederken herhangibir aktivite çalıştırılmadan önce devreye giren olaydır.

WorkflowStarted
Workflow örneği yürütülmeye başlatıldığında tetiklenir. Bu başlangıç kök aktivitenin (Root Activity) çalıştırılması sonrasında meydana gelmektedir.

WorkflowSuspended
Workflow örneği, Suspend metoduna yapılan çağrı veya Suspend aktivitesine gelinmesi nedeniyle Suspended moduna geçtiğinde tetiklenen olaydır.

WorkflowTerminated
Workflow örneği yok edildikten ama bellekten atılmadan az önce çalışan olaydır. Workflow, Terminate metodu yardımıyla, Terminate aktivitesine gelinmesi nedeniyle veya ele alınmamış bir istisna (Unhandled Exception) yüzünden Terminated durumuna geçebilir. Eğer persistence servis kullanılıyorsa, Terminate edilen workflow örneğine ait tüm kayıtlar ilgili depolama alanından kaldırılır. SQL tabanlı persistence servisi göz önüne alındığında bu, tablolar üzerinde gerekli silme işlemlerinin yapılması anlamına gelmektedir.

WorkflowUnloaded
Persistence servisleri kullanıldığında, Workflow örneği depolama alanına kaydedildikten sonra ama bellekten kaldırılmadan az önce tetiklenen olaydır.

Aslında WF Çalışma Zamanı Motorunun (WF Runtime Engine) kendisinin bir State Machine olduğu rahatlıkla düşünülebilir. Nitekim, yönetmekte olduğu örneklerin durumları arasındaki geçişleri kontrol altına almakta ve bununla ilişkili olayları yönetmektedir. Temel olarak workflow örnekleri Created, Running, Suspended, Completed ve Terminated olmak üzere 5 farklı duruma sahip olabilir. Bu durum aşağıdaki diyagram ile özetlenebilir.

![mk270_1.gif](/assets/images/2009/mk270_1.gif)

İlk etapta, Persistence servisinin devrede olmadığı durumlarda standart olarak çalışan olay metodlarını ele alacağımız bir örnek geliştirerek devam edebiliriz. Bu amaçla örnek bir Sequential Workflow Console Application oluşturarak başladığımızı düşünebiliriz. Söz konusu örnek içerisinde Costflow isimli bir Sequential Activity kullanılmakta olup adımları aşağıdaki şekilde görüldüğü gibidir.

![mk270_2.gif](/assets/images/2009/mk270_2.gif)

Söz konusu aktivite aynı zamanda dışarıdan parametre alıp, bir sonuç üretmektedir. Aktivitenin faaliyetsiz (Idle) moda geçtiğini görmek için sembolik olarak Delay aktivitesinden yararlanılmaktadır. Söz konusu aktivititede sadece duraksama süresi özelliği 10 saniye olarak set edilmiştir.

![mk270_3.gif](/assets/images/2009/mk270_3.gif)

Costflow aktivitesine ait kod içeriği aşağıdaki gibi tasarlanabilir.

```csharp
using System;
using System.Workflow.Activities;

namespace WFCostFactory
{
    public enum WorkType
    {
        Consumer,
        Corporate
    }
    public sealed partial class Costflow 
            : SequentialWorkflowActivity
    {
        #region Workflow özellikleri(Properties)

        // Dış ortamdan gelen parametreler
        public int TotalDays { get; set; }
        public decimal CostValue { get; set; } 
        public WorkType WorkT { get; set; } // Dış ortama sonuç olarak döndürülen parametre

        #endregion

        public Costflow()
        {
            InitializeComponent();
        }

        // CodeActivity tarafından çalıştırılan örnek fonksiyonellik
        private void Calculate(object sender, EventArgs e)
        {
            Console.WriteLine("Calculate Metodu. Maliyet hesaplama işlemleri yapılır");
            switch (WorkT)
            {
                case WorkType.Consumer:
                    CostValue = TotalDays * 1.10M;
                    break;
                case WorkType.Corporate:
                    CostValue = TotalDays * 1.15M;
                    break;
                default:
                    CostValue = 1;
                    break;
            }
        }
    }
}
```

Söz konusu aktiviteyi host eden Console uygulamasına ait kod içeriği ise aşağıdaki gibidir. Burada dikkat edilmesi gereken nokta WorkflowRuntime örneğine ait tüm olayların yüklenmiş olmasıdır. Bu olayların çoğu örneğimizde devreye girmeyecektir. Ancak hangi durumlarda devreye gireceği yukarıdaki tablolarda belirtilmiştir.

```csharp
using System;
using System.Collections.Generic;
using System.Threading;
using System.Workflow.Runtime;

namespace WFCostFactory
{
    class Program
    {
        static WorkflowRuntime wfRuntime = null;
        static AutoResetEvent wHandle = null;

        static void Main(string[] args)
        {
            // Workflow Runtime nesnesi örneklenir
            using(wfRuntime = new WorkflowRuntime())
            {
                wHandle = new AutoResetEvent(false);

                #region Event Tanımlamaları

                wfRuntime.Started += new EventHandler<WorkflowRuntimeEventArgs>(wfRuntime_Started);
                wfRuntime.ServicesExceptionNotHandled += new EventHandler<ServicesExceptionNotHandledEventArgs>(wfRuntime_ServicesExceptionNotHandled);
                wfRuntime.Stopped += new EventHandler<WorkflowRuntimeEventArgs>(wfRuntime_Stopped);
                wfRuntime.WorkflowAborted += new EventHandler<WorkflowEventArgs>(wfRuntime_WorkflowAborted);
                wfRuntime.WorkflowCreated += new EventHandler<WorkflowEventArgs>(wfRuntime_WorkflowCreated);
                wfRuntime.WorkflowIdled += new EventHandler<WorkflowEventArgs>(wfRuntime_WorkflowIdled);
                wfRuntime.WorkflowLoaded += new EventHandler<WorkflowEventArgs>(wfRuntime_WorkflowLoaded);
                wfRuntime.WorkflowPersisted += new EventHandler<WorkflowEventArgs>(wfRuntime_WorkflowPersisted);
                wfRuntime.WorkflowResumed += new EventHandler<WorkflowEventArgs>(wfRuntime_WorkflowResumed);
                wfRuntime.WorkflowStarted += new EventHandler<WorkflowEventArgs>(wfRuntime_WorkflowStarted);
                wfRuntime.WorkflowSuspended += new EventHandler<WorkflowSuspendedEventArgs>(wfRuntime_WorkflowSuspended);
                wfRuntime.WorkflowTerminated += new EventHandler<WorkflowTerminatedEventArgs>(wfRuntime_WorkflowTerminated);
                wfRuntime.WorkflowUnloaded += new EventHandler<WorkflowEventArgs>(wfRuntime_WorkflowUnloaded);
                wfRuntime.WorkflowCompleted+=new EventHandler<WorkflowCompletedEventArgs>(wfRuntime_WorkflowCompleted);

                #endregion

                // Workflow nesne örneği oluşturulur
                // TotalDays ve WorkT özellikleri için ilk değerler set edilir
                WorkflowInstance instance = wfRuntime.CreateWorkflow(
                    typeof(WFCostFactory.Costflow)
                    , new Dictionary<string,object>
                        { 
                            {"TotalDays",20}
                            ,{"WorkT",WorkType.Corporate}
                        }
                    );
                // Workflow örneği başlatılır
                instance.Start();
                // İşlemler tamamlana kadar bekle
                wHandle.WaitOne();
            }
        }

        static void wfRuntime_WorkflowUnloaded(object sender, WorkflowEventArgs e)
        {
            Console.WriteLine("{0} : Event : {1}, InstanceId : {2}", DateTime.Now, "WorkflowUnloaded", e.WorkflowInstance.InstanceId.ToString());
        }
    
        static void wfRuntime_WorkflowTerminated(object sender, WorkflowTerminatedEventArgs e)
        {
            Console.WriteLine("{0} : Event : {1}, InstanceId : {2} Exception Message : {3}", DateTime.Now, "WorkflowTerminated",             e.WorkflowInstance.InstanceId.ToString(), e.Exception.Message);
            wHandle.Set();
        }

        static void wfRuntime_WorkflowSuspended(object sender, WorkflowSuspendedEventArgs e)
        { 
            Console.WriteLine("{0} : Event : {1}, InstanceId : {2}", DateTime.Now, "WorkflowSuspended", e.WorkflowInstance.InstanceId.ToString());
        }

        static void wfRuntime_WorkflowStarted(object sender, WorkflowEventArgs e)
        {
            Console.WriteLine("{0} : Event : {1}, InstanceId : {2}", DateTime.Now, "WorkflowStarted", e.WorkflowInstance.InstanceId.ToString());
        }

        static void wfRuntime_WorkflowResumed(object sender, WorkflowEventArgs e)
        {
            Console.WriteLine("{0} : Event : {1}, InstanceId : {2}", DateTime.Now, "WorkflowResumed", e.WorkflowInstance.InstanceId.ToString());
        }

        static void wfRuntime_WorkflowPersisted(object sender, WorkflowEventArgs e)
        {
            Console.WriteLine("{0} : Event : {1}, InstanceId : {2}", DateTime.Now, "WorkflowPersisted", e.WorkflowInstance.InstanceId.ToString());
        }

        static void wfRuntime_WorkflowLoaded(object sender, WorkflowEventArgs e)
        {
            Console.WriteLine("{0} : Event : {1}, InstanceId : {2}", DateTime.Now, "WorkflowLoaded", e.WorkflowInstance.InstanceId.ToString());
        }

        static void wfRuntime_WorkflowIdled(object sender, WorkflowEventArgs e)
        {
            Console.WriteLine("{0} : Event : {1}, InstanceId : {2}", DateTime.Now, "WorkflowIdled", e.WorkflowInstance.InstanceId.ToString());
        }

        static void wfRuntime_WorkflowCreated(object sender, WorkflowEventArgs e)
        {
            Console.WriteLine("{0} : Event : {1}, InstanceId : {2}", DateTime.Now, "WorkflowCreated", e.WorkflowInstance.InstanceId.ToString());
        }

        static void wfRuntime_WorkflowCompleted(object sender, WorkflowCompletedEventArgs e)
        { 
            Console.WriteLine("{0} : Event : {1}, InstanceId : {2}", DateTime.Now, "WorkflowCompleted", e.WorkflowInstance.InstanceId.ToString());
            Console.WriteLine("Maliyet : {0}", e.OutputParameters["CostValue"].ToString());
            wHandle.Set();
        }

        static void wfRuntime_WorkflowAborted(object sender, WorkflowEventArgs e)
        {
            Console.WriteLine("{0} : Event : {1}, InstanceId : {2}", DateTime.Now, "WorkflowAborted", e.WorkflowInstance.InstanceId);
        }

        static void wfRuntime_Stopped(object sender, WorkflowRuntimeEventArgs e)
        {
            Console.WriteLine("{0} : Event : {1}, IsStarted : {2}", DateTime.Now, "WFRuntime_Stopped", e.IsStarted.ToString());
        }
    
        static void wfRuntime_ServicesExceptionNotHandled(object sender, ServicesExceptionNotHandledEventArgs e)
        {
            Console.WriteLine("{0} : InstanceId : {1} Event : {2}, Exception Message : {3}", DateTime.Now, e.WorkflowInstanceId.ToString(),"WF        Runtime_ServicesExceptionNotHandled",e.Exception.Message);
        }

        static void wfRuntime_Started(object sender, WorkflowRuntimeEventArgs e)
        {
            Console.WriteLine("{0} : Event : {1}, IsStarted : {2}", DateTime.Now, "WFRuntime_Started", e.IsStarted.ToString());
        }
    }
}
```

Örneği ilk etapta bu haliye çalıştırdığımızda aşağıdaki ekran çıktısı ile karşılaşırız.

![mk270_4.gif](/assets/images/2009/mk270_4.gif)

İlk analizimi yapabiliriz artık. Dikkat edileceği üzere ilk olarak Workflow çalışma zamanına ait Started olayı tetiklenmiş ve IsStarted değeri true olarak set edilmiştir. Hatırlanacağı üzere Workflow motorunun kullandığı veya başlattığı tüm servislerin IsStarted özelliğine etkisi vardır ve bu özelliğin değeri true kalmadığı sürece çalışma zamanı başlatılamayacaktır. Bu işlemin arkasından Workflow nesnesi örneklendiği için WorkflowCreated ve WorkflowStared olayları sırasıyla çalışmaktadır. Süreç devam etmekteyken Delay aktivitesi devreye girmiştir. İşte bu noktada Workflow örneği faaliyetsiz kalarak, çalışma zamanı moturu tarafından bellekte tutulmaya devam edilmektedir. Bu anda WorkflowIdled olayı tetiklenmiştir.

> Özellikle olay metodlarının bazıları içerisindeden GUID tipinden InstanceId değerlerinin elde edilebiliyor olması önemlidir ki bu sayede hangi WF örneğinin faaliyetsiz kaldığı kolayca anlaşılabilmektedir. Tahmin edeceğiniz üzere bu Id değeri persistence ortamları içinde benzersiliği sağlamak açısından önemlidir.

Faaliyetsiz kalma durumu Delay aktitivitesinde belirtilen süre sonlanıncaya kadar devam eder. Süre sonunda ise Workflow örneği çalışmasına kaldığı yerden tekrar başlayacaktır. İşte bizim en büyük amacımız bu faaliyetsiz kalma anında söz konusu WF örneğini SQL Persistence Service yardımıyla fiziki bir ortama kaydetmektir.

Varsayılan olarak WF örneklerinin durumu bellekte saklanır. Bir başka deyişle, workflow herhangibir sebeple faaliyetsiz duruma geçtiğinde söz konusu örnek bellekte asılı olarak kalır ve beklemeye başlar. Ancak gerçek hayat senaryolarında çalışmakta olan Workflow örneklerinin uzun süre asılı kalmasıda söz konusu olabilir. Bu, özellikle faaliyetine devam etmesi için onu bekleten operasyona bağlıdır. Dolayısıyla bu tip vakalarda Workflow örneklerinin kalıcı olarak saklanmaları tercih edilebilir. Kalıcılıkta esas olan Workflow örneğinin o anki durumu ve değerleri ile fiziki bir depolama alanına atılmasıdır. Bu noktada çoğunlukla SQL gibi veritabanı kaynaklarının kullanılması tercih edilir. Diğer taraftan elbetteki Persistence servisleri özelleştirilebilir ve farklı veri kaynaklarına kaydetme işlemleri gerçekleştirilebilir.

Workflow Foundation, çalışma zamanındaki WF örneklerinin bellekten kaldırılıp çalışmasının durdurulması sonrasında, kalıcı olarak saklanabilmeleri için önceden geliştirilmiş bir Persistence servisi sunmaktadır. Hangi tip Persistence kullanılırsa kullanılsın, Workflow çalışma zamanı, kalıcı olarak saklanan Workflow örneğine (örneklerine) gelen mesajları takip de eder. Bu sayede gerektiği anda, üzerinde yer alan Persistence servisi devreye alarak, ilgili WF örneğinin tekrardan belleğe yüklenmesini sağlayabilir. Workflow çalışma zamanı, Persistence servisini belirli durumlar gerçekleştirildiğinde çağırmaktadır. Bu durumlar;

- WF örneği belirli bir nedenden askıya alındığında yani faaliyetsiz hale geçtiğinde (Idle).
- WF örneği yok edilmeden (Terminate) önce.
- WF örneği tamamlanmadan (Complete) önce.
- Workflow örneği üzerinde Unload, TryUnload metodları çağırıldığında.
- PersistOnCloseAttribute niteliği ile imzalanmış olan bir aktivitenin tamamlanması sonrasında. Özellikle transaction kullanan aktivitiler bu niteliğe sahiptir. Diğer taraftan içerisinde transaction kullanılacak olan aktivitilerinde bu nitleği uygulaması gerekir.

WorkflowPersistenceService abstract sınıfından türetme yapılarak istenirse özel persistence sınıflarıda yazılabilmektedir. Biz örneğimizde SqlWorkflowPersistenceService tipinden yararlanarak WF örneklerini SQL veritabanı üzerinde saklamaya çalışacağız. Şimdi bu durumu incelememiz gerekiyor. Ancak depolama alanı için SQL tarafında gerekli hazırlıkların yapılması gerekmektedir. Bu noktada.Net Framework ile birlikte hazır olarak gelen WF SQL betikleri (Scripts) kullanılabilir. Söz konusu SQL betikleri varsayılan olarak örneğin Windows XP işletim sisteminin kurulu olduğu bir makinede C:\WINDOWS\Microsoft.NET\Framework\v3.0\Windows Workflow Foundation\SQL\EN klasöründe yer almaktadır.

![mk270_5.gif](/assets/images/2009/mk270_5.gif)

Burada yer alan SqlPersistenceServiceLogic ile SqlPersistenceServiceSchema betikleri, Persistence depolama alanı için gerekli tablo (Table), saklı yordam (Stored Procedure) gibi veritabanı nesnelerini oluşturmakla görevlidir. Söz konusu betikler, bir SQL sunucusu üzerinde çalıştırılıp kullanılabileceği gibi, istenirse ilgili Workflow çalışma zamanını Host eden uygulamanın erişebileceği dosya bazlı bir veritabanı üzerindende çalıştırılabilir. Biz örneğimizde ikinci seçeneği kullanacağız. Yani, söz konusu depolama alanı için SQL Express Edition temelli bir veritabanı dosyasını ele alacağız. Bu amaçla ilk olarak Solution'ımıza bir Database Project ekleyerek devam edebiliriz. Proje eklenmesi sırasında bize aşağıdaki ekran görüntüsünde olduğu gibi kullanmak istediğimiz veritabanı sorulacaktır.

![mk270_6.gif](/assets/images/2009/mk270_6.gif)

Elimizde böyle bir veritabanı olmadığını göz önüne alaraktan Add New Reference seçeneğine tıklayalım. Sürekli kullandığımız standart bağlantı ekleme iletişim kutusu ile karşılacağız. Burada önemli olan veri kaynağı olarak Microsoft SQL Server Database File (SqlClient) tipinin seçilmesidir. Sonrasında ise veritabanımıza bir isim vererek devam edebiliriz.

![mk270_7.gif](/assets/images/2009/mk270_7.gif)

Ok düğmesine bastığımızda söz konusu veritabanı yoksa eğer, oluşturmak isteyip istemediğimize dair bir soru sorulacaktır. Bu oluşturma işlemi sırasında unutulmaması gereken noktalardan biriside SQL Express servisinin çalışıyor olması zorunluluğudur. Eğer servis çalışmıyorsa tahmin edileceği üzere söz konusu veritabanı oluşturulamayacaktır. Database projesi oluşturulduktan sonra yukarıda değindiğimiz SQL betiklerini Create Scripts klasörü altına ekleyerek devam edebiliriz. Bu işlemler sonrasında proje içeriği aşağıdakine benzer olacaktır.

![mk270_8.gif](/assets/images/2009/mk270_8.gif)

(Buradaki gibi bir veritabanı projesinin oluşturulması aslında şart değildir. Bu sadece söz konusu veritabanının yönetimin kolaylaştırılmasını sağlayan ve belirli bir düzeni tesis eden bir opsiyon olarak görülmelidir. Genel olarak gerçek hayat uygulamalarında depolama alanı olarak sunucu bazlı veritabanları tercih edilir. Sizlere tavsiyem aynı örneği SQL sunucusu üzerinde gerçekleştirmeye çalışmanızdır.)

Sıradaki işlem, söz konusu SQL betiklerinin çalıştırılmasıdır. Bu betikler çalıştırıldıktan sonra CostFactoryPersistenceDb isimli veritabanının içeriği aşağıdaki şekilde görüldüğü gibi oluşturulacaktır.

![mk270_9.gif](/assets/images/2009/mk270_9.gif)

Burada temel olarak Workflow örneklerinin saklanması (Insert), kilitlenmesi (lock), elde edilmesi (retrieve) veya silinmesi (delete) ile ilişkili gerekli Stored Procedure'ler ve tablolar yer almaktadır. Artık depolama alanıda tanımlandığına göre, Workflow uygulamamız için gerekli kod değişikliklerini yapabiliriz. Bu amaçla host uygulama üzerinde SqlWorkflowPersistenceService'in oluşturulması ve çalışma zamanına eklenmesi gerekmektedir. İşte örnek kodlarımız;

```csharp
using System;
using System.Collections.Generic;
using System.Threading;
using System.Workflow.Runtime;
using System.Workflow.Runtime.Hosting;

namespace WFCostFactory
{
    class Program
    {
        static WorkflowRuntime wfRuntime = null;
        static AutoResetEvent wHandle = null;

        static void Main(string[] args)
        {
            // Workflow Runtime nesnesi örneklenir
            using(wfRuntime = new WorkflowRuntime())
            {
                // Varsayılan ayarları ile persistence servisi örneklenir
                SqlWorkflowPersistenceService persistenceService = new SqlWorkflowPersistenceService
                (
                    @"Data Source=.\SQLEXPRESS;AttachDbFilename=C:\Documents and Settings\Burak Selim Senyurt\My Documents\CostFactoryPersistenceDb.mdf;Integrated Security=True;Connect Timeout=30;User Instance=True"
                );
                // Oluşturulan servis çalışma zamanına bildirilir.
                wfRuntime.AddService(persistenceService);
                //Diğer kod satırları...
```

İlk olarak System.Workflow.Runtime.Hosting isim alanında (namespace) yer alan SqlWorkflowPersistence hizmetine ait bir nesne örneği oluşturulur. Nesne örneklenirken yapıcı metod (Constructor) içerisinde persistence için kullanılacak veri depolama alanın bağlantı bilgisi verilmektedir. Bu en basit yapıcı metodu versiyonudur. Diğer versiyonlarını kullanarak farklı başlangıç ayarlamaları yapılabilir. Söz gelimi Workflow örneklerinin Idle moda geçtiklerinde bellekten kaldırılıp kaldırılmayacakları, birden fazla Workflow çalışma zamanı moturunun aynı WF örneklerini kullanmaları halinde, birbirlerini kesmemeleri için kilit sürelerinin (Lock Time) ne olacağı gibi kriterlerde yapıcı metod parametreleri ile belirlenebilir.

> Servis tanımlaması ile ilgili ayarlar istenirse konfigurasyon dosyasında da yapılabilir. Bunun için host uygulamaya ait konfigurasyon dosyasında örneğin aşağıdaki tanımlamaların yapılması yeterlidir.
> ![mk270_13.gif](/assets/images/2009/mk270_13.gif)
> Tabi host uygulama içerisinde WorkflowRuntime nesne örneği oluşturulurken wfRuntime=new WorkflowRuntime ("WorkflowRuntime"); şeklinde bir kullanım söz konusudur. Burada parametre olarak app.config dosyasındaki section adı verilmektedir. Bu ad benzersizdir. Yani farklı bir isim olamaz. Diğer taraftan yapıcı metodun bu versiyonunun çalıştırılabilmesi için (örneğin geliştirdiğimiz Console uygulamasında) mutlaka System.Configuration assembly'ının projeye referans edilmesi gerekmektedir.

Artık uygulamamızı test etmeye başlayabiliriz. Konuyu kolay takip edebilmek amacıyla geliştirdiğiniz örneği Debug ederken adım adım ilerlemenizi öneririm. Öncelikle programın çalışması sonrasındaki ekran görüntüsüne bakalım.

![mk270_10.gif](/assets/images/2009/mk270_10.gif)

Dikkat edileceği üzere Workflow örneği faaliyetsiz hale geçtikten sonra (WorkflowIdled olayının tetiklenmesi sonrası) sırasıyla WorkflowPersisted, WorkflowUnloaded olayları çalışmıştır. Bir başka deyişle faaliyetsiz kalan Workflow örneği veritabanındaki ilgili tablolara yazılmıştır. Diğer taraftan faaliyetsiz kalma süresi dolduğunda ve Workflow akışı tekrar devam etmek istediğinde sırasıyla WorkflowLoaded ve WorkflowPersisted olayları tetiklenmiştir. Yani WF örneği tablodan tekrar yüklenerek yürütülmeye devam etmiş ve son olarakta tamamlanmıştır. Özellikle Workflow faaliyetsiz hale geldiğinde InstanceState isimli tabloda aşağıdaki ekran görüntüsüne benzer olacak şekilde bir satır açıldığı ve Delay süresi sona erdikten sonra ise WF örneğinin tekrar ayağa kaldırılmasıyla birlikte söz konusu satırın silindiği görülür.

![mk270_11.gif](/assets/images/2009/mk270_11.gif)

Tahmin edileceği üzere bu satır saklanan WF örneğine ait bilgileri serileştirerek tutmaktadır. Bu nedenle özellikle WF içerisinde kullanılan tiplerin, eğer SQL Persistence hizmeti kullanılıyorsa serileştirilebilir olmalarına dikkat etmek gerekmektedir. Bu durumu analiz etmek için Costflow aktivitesine Customer isimli tipten bir özellik eklenmiştir.

```csharp
using System;
using System.Workflow.Activities;

namespace WFCostFactory
{
    public enum WorkType
    {
        Consumer,
        Corporate
    }
    public class Customer
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
    public sealed partial class Costflow : SequentialWorkflowActivity
    {
        #region Workflow özellikleri(Properties)

        // Dış ortamdan gelen parametreler
        public int TotalDays { get; set; }
        public decimal CostValue { get; set; } 
        public WorkType WorkT { get; set; } // Dış ortama sonuç olarak döndürülen parametre
        public Customer Owner { get; set; }

        #endregion

        public Costflow()
        {
            InitializeComponent();
        }
        // Diğer kod satırları
```

Burada hemen bir noktayı vurgulamak isterim. Söz konusu örnek bu haliyle çalıştırıldığında serileştirme ile ilişkili herhangibir hata mesajının alınmadığı görülecektir. Bunun nedeni Customer tipine ait nesne örneğinin WF içerisinde kullanılmamış olmasıdır. Bu nedenle Workflow nesnesi host uygulamada örneklenirken aşağıdaki kod değişikliğini yapmamız çalışma zamanında serileştirme hatasını almamız için gerekli ve yeterlidir.

```csharp
WorkflowInstance instance = wfRuntime.CreateWorkflow(
        typeof(WFCostFactory.Costflow)
        , new Dictionary<string,object>
        { 
            {"TotalDays",20}
            ,{"WorkT",WorkType.Corporate}
            ,{"Owner",new Customer{ Id=1000, Name="Burak Selim Şenyurt"}}
        }
);
```

Örnek bu haliyle çalıştırıldığında aşağıdaki görüntü ile karşılaşılır.

![mk270_12.gif](/assets/images/2009/mk270_12.gif)

Dikkat edileceği üzere, WorkflowPersisted olay metodunun hemen arkasından WorkflowTerminated olayı tetiklenmiş ve oluşan istisna (Exception) mesajı ekrana yazdırılmıştır. Bir başka deyişle Workflow örneği tabloya yazdırılamamış ve istisna fırlatarak sonlanmıştır. İşte bu durumun sebebi Owner isimli Customer tipinin binary formatta serileştirilebilir tanımlanmamasıdır. Bu nedenle Customer tipinin Serializable niteliği (Attribute) ile aşağıdaki kod parçasında görüldüğü gibi işaretlenmesi gerekir.

```csharp
[Serializable]
public class Customer
{
    public string Name { get; set; }
    public int Id { get; set; }
}
```

Uygulama tekrar denendiğinde sorunsuz olarak çalıştığı hatta WF örneği oluşturulurken parametre olarak verilen Customer nesnesinin, WorkflowCompleted olayında (tabloda saklanıp tekrar elde edilmesi ile birlikte) tedarik edilebildiği görülebilir.

![mk270_14.gif](/assets/images/2009/mk270_14.gif)

Görüldüğü üzere Workflow örneklerinin, çalışma zamanında belirli koşulların sağlanması şartıyla bir depolama alanında saklanması ve sonradan tekrardan ayağa kaldırılıp yürütülmesi SQL Persistence Service yardımıyla son derece kolay bir şekilde gerçekleştirilebilmektedir. Elbetteki gerçek hayat koşullarında SQL dışı kaynakların kullanılmasıda istenebilir. Bu gibi vakalarda söz konusu hizmet için özel bir geliştirme yapılması gerekmektedir. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örneği İndirmek İçin Tıklayın](/assets/files/2009/PersistenceInceleme.rar)