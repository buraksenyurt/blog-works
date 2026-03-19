---
layout: post
title: "WF 4.0 : WorkflowInvoker ile Single Thread, WorkflowApplication ile Multi-Thread [Beta 2]"
date: 2009-11-02 02:35:00 +0300
description: "WF 4.0 öncesinde bir Workflow örneğini çalıştırmak için WorkflowRuntime sınıfından yararlanılmaktadır. Aşağıdaki kod parçasında Visual Studio 2008 üzerinde geliştirilen basit bir WF örneğinin çalıştırılması için otomatik olarak üretilen kod görülmektedir. Ancak Workflow Foundation 4.0 içerisinde bir Workflow örneğini çalıştırmak için iki farklı yol sunulmaktadır. İlk yol daha önceki yazı ve görsel derslerimizde de sıklıkla bahsettiğimiz WorkflowInvoker sınıfına ait static Invoke metodunun kullanılmasıdır. Bu tekniğin en önemli özelliği Workflow örneğinin çalıştığı uygulamaya ait Thread içerisinde senkron olaran yürütülmesini sağlamasıdır. Dilerseniz ne demek istediğimize basit bir örnek yardımıyla bakmaya çalışalım. Visual Studio 2010 Ultimate Beta 2 sürümü üzerinden oluşturduğumuz Workflow Console Application içerisinde aşağıdaki Workflow1 içeriği göz önüne alınmaktadır."
categories:
  - wf-4-0-beta-2
tags:
  - wf-4-0-beta-2
  - csharp
  - xml
  - linq
  - workflow-foundation
  - xaml
  - http
  - threading
  - concurrency
  - delegates
  - generics
  - debugging
  - visual-studio
---
# WF 4.0 : WorkflowInvoker ile Single Thread, WorkflowApplication ile Multi-Thread [Beta 2]
![blg94_Giris.jpg](/assets/images/2009/blg94_Giris.jpg)

Merhaba Arkadaşlar,

Hiç müzik dinlerken bir yandan da kod yazmayı denediniz mi? Üstelik çevre ile olan etkileşiminiz devam ederken

![Wink](/assets/images/2009/smiley-wink.gif)

Söz gelimi hareketli bir parçayı tempo tutarak dinleyip ondan tamamen bağımsız bir şekilde geliştirmeye devam ederken yan masadaki arkadaşınızdan gelen "Dün akşamki maçı seyrettin mi?...Ronaldo ne gol attı öyle..." sorusuna da rakip takımın orta sahasını kattığınız bir yorumda bulunup diğer taraftanda kahve içtiğinizi düşünebilirsiniz.

Tabiki insan beyninin büyülü dünyası ve eş zamanlı olarak çalışma yetenekleri zaman zaman geliştirdiğimiz uygulamalara da yansımaktadır. Böyle bir giriş yaptığımıza göre multi-thread bir takım işlemleri anlatacağımı düşünmüş olmalısınız. İşte bu gün konumuz Workflow Foundation 4.0 üzerindeki Single-Thread ve Multi-Thread çalıştırma modelleri.

WF 4.0 öncesinde bir Workflow örneğini çalıştırmak için WorkflowRuntime sınıfından yararlanılmaktadır. Aşağıdaki kod parçasında Visual Studio 2008 üzerinde geliştirilen basit bir WF örneğinin çalıştırılması için otomatik olarak üretilen kod görülmektedir.

```csharp
using(WorkflowRuntime workflowRuntime = new WorkflowRuntime())
            {
                AutoResetEvent waitHandle = new AutoResetEvent(false);

                workflowRuntime.WorkflowCompleted += delegate(object sender, WorkflowCompletedEventArgs e) {waitHandle.Set();};

                workflowRuntime.WorkflowTerminated += delegate(object sender, WorkflowTerminatedEventArgs e)
                {
                    Console.WriteLine(e.Exception.Message);
                    waitHandle.Set();
                };

                WorkflowInstance instance = workflowRuntime.CreateWorkflow(typeof(WorkflowConsoleApplication1.Workflow1));
                instance.Start();

                waitHandle.WaitOne();
            }
```

Ancak Workflow Foundation 4.0 içerisinde bir Workflow örneğini çalıştırmak için iki farklı yol sunulmaktadır. İlk yol daha önceki yazı ve görsel derslerimizde de sıklıkla bahsettiğimiz WorkflowInvoker sınıfına ait static Invoke metodunun kullanılmasıdır. Bu tekniğin en önemli özelliği Workflow örneğinin çalıştığı uygulamaya ait Thread içerisinde senkron olaran yürütülmesini sağlamasıdır. Dilerseniz ne demek istediğimize basit bir örnek yardımıyla bakmaya çalışalım. Visual Studio 2010 Ultimate Beta 2 sürümü üzerinden oluşturduğumuz Workflow Console Application içerisinde aşağıdaki Workflow1 içeriği göz önüne alınmaktadır.

![blg94_Workflow1Design.gif](/assets/images/2009/blg94_Workflow1Design.gif)

Akışın xaml içeriği ise aşağıdaki gibidir.

```xml
<Activity mc:Ignorable="sap" x:Class="WorkflowConsoleApplication5.Workflow1" mva:VisualBasic.Settings="Assembly references and imported namespaces serialized as XML namespaces" xmlns="http://schemas.microsoft.com/netfx/2009/xaml/activities" xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" xmlns:mv="clr-namespace:Microsoft.VisualBasic;assembly=Microsoft.VisualBasic, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" xmlns:mv1="clr-namespace:Microsoft.VisualBasic;assembly=System" xmlns:mva="clr-namespace:Microsoft.VisualBasic.Activities;assembly=System.Activities" xmlns:s="clr-namespace:System;assembly=mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" xmlns:s1="clr-namespace:System;assembly=mscorlib" xmlns:s2="clr-namespace:System;assembly=System" xmlns:s3="clr-namespace:System;assembly=System.Xml" xmlns:s4="clr-namespace:System;assembly=System.Core" xmlns:sad="clr-namespace:System.Activities.Debugger;assembly=System.Activities" xmlns:sap="http://schemas.microsoft.com/netfx/2009/xaml/activities/presentation" xmlns:scg="clr-namespace:System.Collections.Generic;assembly=System" xmlns:scg1="clr-namespace:System.Collections.Generic;assembly=System.ServiceModel" xmlns:scg2="clr-namespace:System.Collections.Generic;assembly=System.Core" xmlns:scg3="clr-namespace:System.Collections.Generic;assembly=mscorlib" xmlns:sd="clr-namespace:System.Data;assembly=System.Data" xmlns:sd1="clr-namespace:System.Data;assembly=System.Data.DataSetExtensions" xmlns:sl="clr-namespace:System.Linq;assembly=System.Core" xmlns:st="clr-namespace:System.Threading;assembly=mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" xmlns:st1="clr-namespace:System.Text;assembly=mscorlib" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
  <Sequence sad:XamlDebuggerXmlReader.FileName="c:\documents and settings\bsenyurt\my documents\visual studio 10\Projects\WorkflowConsoleApplication5\WorkflowConsoleApplication5\Workflow1.xaml" sap:VirtualizedContainerService.HintSize="232,344">
    <sap:WorkflowViewStateService.ViewState>
      <scg3:Dictionary x:TypeArguments="x:String, x:Object">
        <x:Boolean x:Key="IsExpanded">True</x:Boolean>
      </scg3:Dictionary>
    </sap:WorkflowViewStateService.ViewState>
    <WriteLine sap:VirtualizedContainerService.HintSize="210,59" Text="[String.Format("WF Başlangıç {0} Thread ID:{1}", TimeString, Thread.CurrentThread.ManagedThreadId.ToString())]" />
    <Delay Duration="00:00:05" sap:VirtualizedContainerService.HintSize="210,22" />
    <WriteLine sap:VirtualizedContainerService.HintSize="210,59" Text="[String.Format("WF Bitiş {0}", TimeString)]" />
  </Sequence>
</Activity>
```

Workflow1 içerisinde dikkat çekici noktalardan birisi Delay aktivitesi ile sanal bir geciktirmenin uygulanmış olmasıdır. Bunun yanında ilk WriteLine bileşeni içerisinde güncel ManagedThreadId değerinin yazdırılması sağlanmaktadır. Şimdi bu Workflow örneğini yürütmek için aşağıdaki kod parçasını geliştirdiğimizi düşünelim.

```csharp
using System;
using System.Activities;
using System.Threading;

namespace WorkflowConsoleApplication5
{

    class Program
    {
        static void Main(string[] args)
        {
            #region Senkron çalışma(Aynı Thread içerisinde)

            WorkflowInvoker.Invoke(new Workflow1());
            Console.WriteLine("Invoke çağrısının hemen sonrası {0} Thread ID :{1}",DateTime.Now.ToLongTimeString(),Thread.CurrentThread.ManagedThreadId.ToString());

            #endregion
        }
    }
}
```

İlk olarak WorkflowInvoker sınıfının static Invoke metodu ile Workflow1 nesne örneğinin çalıştırılması sağlanmaktadır. Devam eden kod satırının devreye girmesi için Delay süresi kadar beklenmesi gerekecektir ki bu örnekte önemli olan noktada budur. Diğer taraftan hem Workflow1 için hemde Program sınıfı için aynı ManagedThreadId değerlerinin üretildiği gözlemlenecektir. İşte çalışma zamanı sonuçlarımız.

![blg94_InvokerRuntime.gif](/assets/images/2009/blg94_InvokerRuntime.gif)

Görüldüğü gibi Invoke metodu çağrısı sonrasındaki kod satırına geçmek için Workflow1' in tamamlanması beklenmiştir. Bu senkron çalışmanın bir sonucu ve ispatıdır. Ayrıca bir Workflow örneğini çalıştırmanın en basit yolu olarak düşünülebilir. Ancak bazı zamanlarda özellikle Long Running Process'lerde söz konusu olduğunda, sunucu tarafındaki Workflow örneğinin farklı bir Thread içerisinde yürütülmesi ve buna bağlı olarakta Host uygulamanın paralel olarak çalışmaya devam etmesi istenebilir. Bir başka deyişle Workflow örneklerini host eden uygulamanın söz konusu akışları Multi-Thread olarak yürütmesi istenebilir. İşte bu durumda WorkflowApplication tipinden yararlanılmaktadır.

Bu tip ve üyeleri sayesinde Workflow örneğinin farklı bir Thread içerisine açılması ve ana uygulama ile paralel olarak yürütülmesi sağlanabilir. Üstelik çeşitli olayları sayesinde Workflow örneğinin tamamlanması, ele alınmamış bir istisna (Unhandled Exception) nedeniyle sonlanması gibi durumlar kontrol altına alınabilir. Aslında bir önceki versiyonda yer alan WorkflowRuntime tipinin üstlendiği misyonun aynısıdır diyebiliriz. Şimdi bu çalışmayı farklı bir Workflow örneği üzerinden değerlendirmeye çalışalım. Bu sefer parametre kullanımı ve geri dönüş tiplerinide söz konusu sınıf olayları içerisinde değerlendirmeye çalışacağız. İşte Workflow2 tasarım görüntüsü.

![blg94_Workflow2Design.gif](/assets/images/2009/blg94_Workflow2Design.gif)

Workflow1'deki aktivitemizde olan bileşenleri kullanmakla beraber bu kez X ve Y isimli iki In ve Sum isimli bir Out argümanımız olduğunu belirtelim. Main metodu içeriğimizi ise aşağıdaki gibi geliştirdiğimizi düşünelim.

```csharp
using System;
using System.Activities;
using System.Threading;

namespace WorkflowConsoleApplication5
{

    class Program
    {
        static void Main(string[] args)
        {
            #region Asenkron çalışma

            AutoResetEvent aResetEvent = new AutoResetEvent(false);

            // Workflow2 nesnesi örneklenir ve In argümanlarına ilk değerleri atanır
            Workflow2 wf2 = new Workflow2
            {
                X = 10,
                Y = 12
            };

            // WorkflowApplcation nesnesi örneklenirken parametre olarak çalıştırılmak istenen Workflow nesne örneği verilir.
            WorkflowApplication wfApp = new WorkflowApplication(wf2);

            // Completed olay metodu, WorkflowApplicationCompletedEventArgs tipinden olay parametresi alır.
            // Workflow çalışmasını tamamladığında devreye girecek olay metodudur
            wfApp.Completed = (e) =>
                {
                    // e parametresinden yararlanarak Outputs özelliğine ve dolayısıyla çalıştırılmakta olan Workflow' un Out argümanlarına ilgili Dictionary koleksiyonu üzerinden erişilebilir.
                    Console.WriteLine("Toplam Sonucu {0}, Thread ID : {1}", e.Outputs["Sum"].ToString(), Thread.CurrentThread.ManagedThreadId.ToString());
                    aResetEvent.Set(); // Workflow' un tamamlandığı bilgisi Thread' e sinyal olarak gönderilir.
                };

            // Aborted olayı WorkflowApplicationAbortedEventArgs tipinden parametre almaktadır.
            // Workflow bir sebepten iptal olduğunda devreye giren olay metodudur.(Örneğin, Abort metodu ile yapılan çağrı veya bir Exception nedeniyle)            
            wfApp.Aborted = (e) =>
                {
                    // Akışın bir istisna(Exception) nedeniyle iptal olması halinde Exception tipinden olan Reason özelliği değerlendirilir. 
                    if(e.Reason!=null)
                        Console.WriteLine("Workflow2 {0} sebebiyle iptal oldu", e.Reason.Message);                    
                    aResetEvent.Set(); // Workflow' un tamamlandığı bilgisi Thread' e sinyal olarak gönderilir.
                };

            wfApp.Run(); // İşlemler başlatılır

            Console.WriteLine("WorkflowApplication.Run çağrısının hemen sonrası {0} Thread ID :{1}", DateTime.Now.ToLongTimeString(), Thread.CurrentThread.ManagedThreadId.ToString());

            // Ana Thread içerisindeki işlemlerin paralel yürüdüğünü göstermek için kullanılan hile döngüsü
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Workflow çalışıyor...");
                Thread.Sleep(1000);
            }

            aResetEvent.WaitOne(); // Eğer Workflow halen daha tamamlanmamışsa bekle.

            #endregion
        }
    }
}
```

Örneği bu haliyle çalıştırdığımızda aşağıdaki ekran görüntüsüne benzer çalışma zamanı sonuçlarını alırız.

![blg94_Wf2Runtime.gif](/assets/images/2009/blg94_Wf2Runtime.gif)

Görüldüğü üzere Workflow2 örneği çalıştırıldıktan sonra ana uygulama akmaya devam etmiştir. Üstelik ana uygulama akmaya devam ederken tamamlanan Workflow ile ilişkili Completed olay metodu da devreye girmiştir. Dikkat edilmesi gereken noktalardan biriside ana uygulamaya ait ManagedThreadId ile WorkflowApplication tarafından açılan ManagedThreadId değerlerinin farklı olmasıdır ki istediğimiz sonuçlardan bir diğeride budur. Nitekim Multi-Thread çalışmanın ispatıdır. Aslında WF4.0 öncesindeki WorkflowRuntime kullanımına baktığımızda neredeyse aynı işleyiş modelinin kullanıldığını görebiliriz. Önceki modelde de olay bazlı olarak Workflow örneklerinin tamamlanması ve sonlanması durumları ele alınabilmektedir. Ancak WorkflowApplication tipinin bu şekilde kalıp kalmayacağıda henüz belli değildir. Bunu ancak son sürümde net olarak öğrenebileceğimizi ifade edebilirim. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

Kişisel Not: Tabiki halen Beta 2 sürümünde olduğumuzu hatırlatmak isterim. Yani bu özelliklerde değişiklikler olabilir, bazıları kaldırılabilir veya tekrardan geriye dönüşler yapılabilir.

[WorkflowConsoleApplication5.rar (43,99 kb)](/assets/files/2009/WorkflowConsoleApplication5.rar)