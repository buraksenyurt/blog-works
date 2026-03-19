---
title: "WF - ExternalDataExchange, Local Services ve CallExternalMethodActivity"
pubDate: 2009-09-25 07:23:00
categories:
  - WF
tags:
  - WF
  - C#
  - .NET
  - Workflow Foundation
  - HTTP
  - Delegates
  - Generics
  - Visual Studio
---

# WF - ExternalDataExchange, Local Services ve CallExternalMethodActivity
![blg81_Giris.jpg](images/blg81_Giris.jpg)

Merhaba Arkadaşlar,

Artık yazın bittiği, okulların açıldığı, şehrin kalabalığının arttığı bu günlerde birde sağnak yağışlar işin içerisine girince, insan ister istemez tatilde üzerinden denize atladığı bir iskelede olmak istiyor. Artık o iskelenin etrafında fazla insan yok ve yağmur yüzünden tahtaların üzerinde gizemli bir şekilde akan su birikintileri var; diyerek yaptığımız duygusal girişimizin aslında yazımızın ilerleyen kısmı ile bir alakası yok.

![Wink](images/smiley-wink.gif)

Ama yine böyle yağmurlu bir günde cama vuran damlacıkları izlerken Workflow Foundation ile ilişkili düşündüğüm ve aklıma gelen bir konunun çözümünü sizlerle paylaşmak niyetindeyim.

İhtiyaç: Birden fazla aktivitenin aynı fonksiyonları ortaklaşa kullanabilmeleri nasıl sağlanır? Yani bir fonksiyonun birden fazla aktivite içerisinde kullanılması gerektiği durumlarda nasıl bir yol izleyebiliriz?

Çözüm: Böyle bir ihtiyaçta metodların kod içeriklerini tüm aktivitelerde örneğin CodeActivity bileşenleri içerisinde değerlendirebiliriz. Ama bu durumda merkezileştirilmemiş ve güncelleştirmeler sırasında kullanıldığı tüm aktivitelerde düşünülmesi gereken bir çözüm üretmiş oluruz. Aslında bir yol olarak söz konusu fonksiyonellikleri ortak bir kütüphane içerisinde toplayabilir ve yine CodeActivity'ler içerisinden çağırabiliriz. Lakin bu noktada değerlendirebileceğimiz başka bir çözüm daha vardır ve gerçekten araştırılmaya değerdir. Buna göre, Local Service olarak çalışma zamanına eklenmiş bir arayüzden yararlanılabilir ve ortak fonksiyonelliklerin bu arayüz üzerinden aktiviteler ile mesajlaşması sağlanabilir.

Burada kritik olan nokta ExternalDataExchange niteliği (attribute) ile işaretleniş bir arayüzü (Interface) implemente eden bir tipin fonksiyonelliklerinin, herhangibir aktivite tarafından kullanılabilir hale gelmesidir. Tabi bu kullanımı sağlamak için CallExternalMethodActivity aktivite tipinden yararlanılması gerekir. Geliştirici olarak çalışma şeklini iyice kavramak yakalayacağımız kavramlar açısından önemlidir. Öncelikle CallExternalMethodActivity bileşeninin bir aktivite tipi olarak harici bir metodu işaret edebileceğini göz önüne almalıyız. Bu durumda tasarım zamanında (Design Time), CallExternalMethodActivity bileşeninin çağıracağı harici metodun imzasını ve nerede olduğunu bilmesi gerekmektedir ki çalışma zamanında bu bilgilerden yararlanarak, içinde bulunduğu aktivite ile harici metod arasında bir mesajlaşma sağlayabilsin.

Diğer yandan, tasarım zamanında IDE'nin CallExternalMethodActivity bileşenine kullanabileceği tipleri göstermesi, basit bir plug-in düzeneğine benzetilebilir. Söz konusu bileşen kullanabileceği tipleri bulmak konusunda, ExternalDataExchange niteliğini uygulamış interface tiplerini baz almaktadır. Buna göre arayüz tipinin çalışma zamanında gerçek işlevleri içeren bir uygulayıcısı da olmalıdır. Yani söz konusu arayüzü implemente eden bir tipten bahsediyoruz. Aktiviteler birden fazla CallExternalMethodActivity bileşeni içerebileceği gibi, birden fazla ExternalDataExchange nitelikli arayüz implementasyonunu da değerlendirebilir.

Artık konuyu örnekleyerek devam etmekte yarar olacağı kanısındayım. Örneğimizi Visual Studio 2008 ortamında ve.Net Framework 3.5 odaklı olarak geliştiriyor olacağız. İlk olarak System.Workflow.Activities assembly'ını referans eden bir Class Library projesi oluşturarak işe başlayalım. Bir sınıf kütüphanesi tasarladığımızdan, herhangibir Workflow projesinde kullanılabilir ve tek merkezden güncellenebilir bir ürünümüz söz konusudur. Bu kütüphane, ExternalDataExchange nitelikli arayüz ve implementasyonlarını yapan tipleri barındırabilir ki örneğimizde bu amaçla aşağıdaki sınıf diagramında görülen tipler değerlendirilecektir.

![blg81_ClassDiagram.gif](images/blg81_ClassDiagram.gif)

Kod içeriğimiz;

```csharp
using System;
using System.Workflow.Activities;

namespace CommonOperations
{
    // ICommonAccounting arayüz tipinin yerel servislerden(Local Service) birisi olduğu belirtilir
 [ExternalDataExchange]
 public interface ICommonAccounting
 {
        void IncreaseRate(double rate, int categoryId);
        void DecreaseRate(double rate, int categoryId);
 }

    // Yerel servis metodlarının uygulandığı yer
    public class CommonAccounting
        :ICommonAccounting
    {
        // Host uygulamanın değerlendirebileceği basit bir olay
        public event EventHandler<AccountingResultsEventArgs> OnCompleted;

        #region ICommonAccounting Members

        public void IncreaseRate(double rate, int categoryId)
        {
            Console.WriteLine("{0} kategorisindeki maaşlar % {1} oranında arttırılacak",categoryId.ToString(),rate.ToString());
            if (OnCompleted != null)
                OnCompleted(this, new AccountingResultsEventArgs { StepType = "Increase", Rate = rate,StepOk=true });
        }

        public void DecreaseRate(double rate, int categoryId)
        {
            Console.WriteLine("{0} kategorisindeki maaşlar % {1} oranında azaltılacak", categoryId.ToString(), rate.ToString());
            if (OnCompleted != null)
                OnCompleted(this, new AccountingResultsEventArgs { StepType = "Decrease", Rate = rate,StepOk=true });
        }

        #endregion
    }

    // OnCompleted olayı içerisinde kullanılan ve olay metoduna bilgi taşıyan sınıf
    public class AccountingResultsEventArgs
        : EventArgs
    {
        public string StepType { get; set; }
        public double Rate { get; set; }
        public bool StepOk { get; set; }
    }
}
```

ICommonAccounting isimli arayüze ExternalDataExchange niteliği uygulanmıştır. Arayüzümüzde, işlevleri bizim için şu aşamada çok önemli olmayan iki basit operasyon tanımlaması yer almaktadır. Diğer taraftan bu arayüzü implemente eden CommonAccounting tipi içerisinde operasyonların uygulaması yer almaktadır. CommonAccounting sınıf ayrıca, kendisini kullanan aktivitelere bilgi taşıyabilmekte kullanılabilecek bir olay bildirimi de (OnCompleted) içermektedir.

Bu olay içerisinde kullanılan AccountingResultEventArgs isimli EventArgs türevli tip, çalışma zamanındaki CommonAccounting nesne örneğinden, OnCompleted olayına abone olan aktiviteye StepType, Rate ve StepOk gibi bazı yardımcı bilgiler döndürmektedir. IncreaseReate ve DecreaseRate metodları içerisinde, OnCompleted olayının yüklü olması halinde çalıştırılması işlemi gerçekleştirilmektedir.

Kişisel Not: Olayları daha net kavrayabilmek için [eski bir makalemden](http://www.csharpnedir.com/articles/read/?filter=popular&author=&cat=&id=747&title=C) faydalanabilirsiniz.

Artık bu sınıf kütüphanesini kullanacak basit bir Workflow projesi geliştirebiliriz. Bu amaçla bir Sequential Workflow Console Application projesi oluşturduğumuzu ve geliştirdiğimiz CommonOperations isimli sınıf kütüphanesini buraya referans ettiğimizi düşünelim. Boş bir Activity öğesini projeye ekledikten sonra içeriğini aşağıdaki gibi kodlayalım.

```csharp
using System.Workflow.Activities;

namespace HostApp
{
    public partial class Activity1
        : SequenceActivity
    {
        public double IncreaseRate { get; set; }
        public double DecreaseRate { get; set; }
        public int CategoryId { get; set; }

        public Activity1()
        {
            InitializeComponent();
        }
    }
}
```

Burada tanımlanan IncreaseRate, DecreaseRate ve CategoryId özellikleri, CallExternalMethodActivity bileşenlerinin kullanacağı harici metodlara aktarılacak aktivite seviyesindeki değerleri taşımak üzere kullanılmaktadır. Şimdi tasarım zamanında, Activity1 içerisine örnek bir CallExternalMethodActivity bileşenini sürükleyerek devam edebiliriz. Bu işlemin ardından bileşenin InterfaceType özelliğinden yararlanarak hangi arayüzü kullanacağını aşağıdaki şekilden görüldüğü gibi seçebiliriz.

![blg81_InterfaceProp.gif](images/blg81_InterfaceProp.gif)

Görüldüğü gibi ICommonAccounting arayüzü otomatik olarak gelmiştir. Böylece hangi operasyonların kullanılabileceği, bu operasyonlara hangi parametrelerin verilmesi gerektiği bilinmektedir. Bizde akışımıza örnek olarak iki CallExternalMethodActivity bileşeni ekleyip özelliklerini aşağıdaki gibi ayarlayarak devam edebiliriz.

callExternalMethodActivity1 bileşeninin özellikleri;

![blg81_Prop1.gif](images/blg81_Prop1.gif)

callExternalMethodActivity2 bileşeninin özellikleri;

![blg81_Prop2New.gif](images/blg81_Prop2New.gif)

Görüldüğü gibi her iki bileşen için ICommonAccounting arayüzü seçilmiş, buna göre sırasıyla IncreaseRate ve DecreaseRate operasyonlarının kullanılacağı belirtilmiştir. Ayrıca söz konuzu operasyonların parametreleri, otomatik olarak özellikler penceresine gelmiştir (rate ve categoryId). Bu özelliklerde aslında, Activity1 tipi içerisinde tanımlanmış olan IncreaseRate,DecreaseRate ve CategoryId özelliklerini işaret edecek şekilde bizim tarafımızdan ayarlanmaktadır.

Dolayısıyla WF çalışma zamanında, Activity1 içerisindeki ilgili özelliklere atanabilecek olan değerler, CallExternalMethodActivity bileşenleri ile CommonAccounting nesnesinin ilgili metodlarına gönderilerek işlenebilecektir. Eğer WF Çalışma zamanını host eden sınıf, CommonAccounting tarafından tanımlanmış OnCompleted olayınıda yüklerse, CallExternalMethodActivity bileşenlerinin çalıştırdığı harici metodlardan bazı bilgileri kendi ortamına alarak değerlendirebilecektir (AccountingResultEventArgs yardımıyla). Bu yapı için WF çalışma zamanına özel bazı kodlamaların yapılmasıda gerekmektedir. İşte uygulama kodlarımız;

```csharp
using System;
using System.Collections.Generic;
using System.Threading;
using System.Workflow.Activities;
using System.Workflow.Runtime;
using CommonOperations;

namespace HostApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using(WorkflowRuntime workflowRuntime = new WorkflowRuntime())
            {
                #region Yerel Servisi Bildirme İşlemi

                // Yerel servisler için eklenmesi gereken servistir
                ExternalDataExchangeService service = new ExternalDataExchangeService();
                // ExternalDataExchangeService örneği Workflow çalışma zamanına eklenir
                workflowRuntime.AddService(service);

                // ExternalMetadaExchange nitelikli interface tipini implemente eden asıl nesne örneklenir
                CommonAccounting accounter = new CommonAccounting();
                // HostApp uygulamasının ele alacağı OnCompleted olayı yüklenir ve anonymous method yardımıyla değerlendirilir.
                accounter.OnCompleted += delegate(object sender, AccountingResultsEventArgs e)
                {
                    // Örnek metodlardan gelen sonuçlar listelenir.
                    Console.WriteLine("\n\tİşlem tipi {0}\n\tRate {1}\n\tİşlem sonucu {2}", e.StepType, e.Rate.ToString(),e.StepOk.ToString());
                };

                //accounter isimli ExternalMetadaExchange nitelikli interface tipini implemente eden asıl nesne örneği, ExternalDataExchangeService örneğine eklenir.
                service.AddService(accounter);

                #endregion

                AutoResetEvent waitHandle = new AutoResetEvent(false);
                // Workflow tamamlandığında devreye giren olay metodu
                workflowRuntime.WorkflowCompleted += delegate(object sender, WorkflowCompletedEventArgs e) {
                    waitHandle.Set();
                };
                // Exception gibi nedenlerle Workflow sonlandığında devreye giren olay metodu
                workflowRuntime.WorkflowTerminated += delegate(object sender, WorkflowTerminatedEventArgs e)
                {
                    Console.WriteLine(e.Exception.Message);
                    waitHandle.Set();
                };

                // Yerel servis içerisindeki metodların kullanacağı parametrelere işaret eden özellikler yüklenir.
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    {"IncreaseRate",1.12},
                    {"DecreaseRate",2.25},
                    {"CategoryId",1}
                };

                // Aktivite nesnesi örneklenir, özellikleri için ilk değerleri yüklenir.
                WorkflowInstance mathActivity = workflowRuntime.CreateWorkflow(typeof(HostApp.Activity1),parameters);
                // Aktivite başlatılır
                mathActivity.Start();
                // Asenkron işleyişi ispat etmek için
                Console.WriteLine("İşlemler başladı");
                // İşlemler tamamlanmadıysa bekle
                waitHandle.WaitOne();
            }
        }
    }
}
```

Görüldüğü üzere Local Service'in tanımlanmasını takiben, ExternalDataExchange nitelikli tipe ait nesne örneklenmiş ve yerel servise bildirilmiştir. Ayrıca CommonAccounting nesnesinin OnCompleted olayı yüklenmiş ve Program'ın bu olaya abone olması sağlanmıştır. Activity1 nesnesine ait özellikleri set etmek için Dictionary koleksiyonundan yararlanılmış ve son olarak aktivitemiz başlatılmıştır. İşte çalışma zamanı sonuçları.

![blg81_Runtime.gif](images/blg81_Runtime.gif)

Evet...Önce belirli oranda arttırım yapıp sonra azaltım yapmak son derece saçma gözükmektedir

![Undecided](images/smiley-undecided.gif)

Ancak yakalamamız gereken nokta elbetteki bu değildir. Önemli olan, bir aktivite'nin kendi sınırları dışındaki fonksiyonellikleri kullanabilmek için yerel servislerden nasıl yararlanıldığı ve bunun için ExternalDataExchange niteliğinin nasıl değerlendirildiğidir. Üstelik bu değerlendirme, WF tasarım zamanı içinde önem arz eder. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[UsingExternalCode.rar (50,98 kb)](assets/UsingExternalCode.rar)
