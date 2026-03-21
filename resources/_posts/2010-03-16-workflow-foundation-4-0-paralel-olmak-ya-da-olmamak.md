---
layout: post
title: "Workflow Foundation 4.0 - Paralel Olmak ya da Olmamak"
date: 2010-03-16 02:15:00 +0300
categories:
  - wf-4-0-rc
tags:
  - workflow-foundation
  - parallel-programming
  - .net-framework
---
Pek çoğumuz ünlü İngiliz şairi Shakespeare'in adını ve eserlerini bir şekilde duymuş, okumuş veya seyretmişizdir. Yaşadığı 1564-1616 yılları arasında yazdığı sayısız Komedi, Trajedi ve Romanesk bulunmaktadır. Bunlar yüz yıllar boyu Tiyatrolarda sergilenmiş ve edebi değeri yüksek eserlerdir. Shakespeare dendiğinde insanın aklına hemen Romeo ve Juliet, Hamlet, Othello, Machbeth gibi eserleri gelmektedir. Aslında Edebiyat'tan çok fazla anlamam. Büyük bir ihtimalle Matematikçi olduğum içindir.

![blg155_Giris.jpg](/assets/images/2010/blg155_Giris.jpg)

Dolayısıyla Shakespeare gibi ünlü şarilerin eserlerini hayatım boyunca çok fazla dikkate almamışımdır. Tabi son zamanlarda bu tip kült klasiklerin NTV Yayınlarından çıkan çizgi serileri yer almakta. En azından çocuklarımızın okuması için bir hamlede bulunabiliriz. Her ne kadar Shakespeare'i çok fazla okumasam da, Mel Gibson'ın oyunculuğuyla parladığı Hamlet filmini seyretmişimdir. Her ne kadar Shakespeare'in eserlerindeki anlamı, derinliği tam olarak bilemesem de, şiirindeki şu meşhur mısrayı hiç unutmam; "Olmak ya da olmamak; işte bütün mesele bu". Bu konuya nereden mi geldik? Kısaca hikayeyi anlatayım.

Geçtiğimiz günlerde Workflow Foundation 4.0 içerisinde NativeActivity türevli bileşenlerde hata yönetiminin nasıl yapılabileceğini incelerken, ne olduysa kendimi ParallelForEach aktivitesini çalıştırmaya uğraşırken buldum. Bir türlü istediğim gibi ayrı Thread parçaları oluşturulmuyor dolayısıyla aktivite içerisine aldığım işler paralel olarak yürütülmüyordu. O sırada şöyle mırıldandığımı çok net hatırlıyorum; "Paralel olmak ya da olmamak. Sanırım tüm mesele bu..."

![Sealed](/assets/images/2010/smiley-sealed.gif)

İşte bu yazımızda ParallelForEach aktivitesinin örnek senaryoya göre neden çalıştırılamadığını ve buna karşın basit olan çözümün ne olduğu görmeye çalışacağız. Öncelikli olarak sorunumuzu örnek bir senaryo üzerinden masaya yatıralım. Bu amaçla aşağıdaki sınıf diagramında görülen CodeActivity türevli bir bileşenimiz olduğunu düşünelim.

![blg155_ClassDia1.gif](/assets/images/2010/blg155_ClassDia1.gif)

Kod içeriği;

```csharp
using System;
using System.Activities;
using System.Threading;

namespace ErrorHandlingForNativeActivities
{
    public sealed class LetterCalculaterActivity 
        : CodeActivity
    {
        public InArgument<char> Letter { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            Thread.Sleep(1000);
            Console.WriteLine(
                "{0} ASCII = {1} | Current Thread Id : {2}"
                , Letter.Get(context)
                ,((byte)Letter.Get(context)).ToString()
                , Thread.CurrentThread.ManagedThreadId.ToString());
        }
    }
}
```

CodeActivity türevli olan bu bileşenimiz InArgument tipinden olan Letter isimli özelliğin çalışma zamanı değerini almakta ve ASCII kodu karşılığı ile o anki yönetimli (Managed) Thread Id değerlerini ekrana yazdırmaktadır. Execute metodunda dikkat edileceği üzere şakacıktan ana Thread'in 1 saniye süreyle duraksatılması söz konusudur. Peki bu Activite bileşeni ne işimize yarayacak? Bu amaçla tasarım görünümü aşağıdaki gibi olan bir Workflow geliştirdiğimizi düşünelim.

![blg155_DesignTime.gif](/assets/images/2010/blg155_DesignTime.gif)

Xaml içeriği;

```xml
<Activity........>
  <x:Members>
    <x:Property Name="vSentence" Type="InArgument(x:String)" />
  </x:Members>
  <sap:VirtualizedContainerService.HintSize>349,370</sap:VirtualizedContainerService.HintSize>
  <mva:VisualBasic.Settings>Assembly references and imported namespaces for internal implementation</mva:VisualBasic.Settings>
  <Sequence sad:XamlDebuggerXmlReader.FileName="D:\Vs 2010\RC\Workflow Foundation\ErrorHandlingForNativeActivities\ErrorHandlingForNativeActivities\ParallelFlow.xaml" sap:VirtualizedContainerService.HintSize="309,330">
    <sap:WorkflowViewStateService.ViewState>
      <scg:Dictionary x:TypeArguments="x:String, x:Object">
        <x:Boolean x:Key="IsExpanded">True</x:Boolean>
      </scg:Dictionary>
    </sap:WorkflowViewStateService.ViewState>
    <ParallelForEach x:TypeArguments="x:Char" DisplayName="ParallelForEach<Char>" sap:VirtualizedContainerService.HintSize="287,206" Values="[vSentence]">
      <ActivityAction x:TypeArguments="x:Char">
        <ActivityAction.Argument>
          <DelegateInArgument x:TypeArguments="x:Char" Name="ltr" />
        </ActivityAction.Argument>
        <local:LetterCalculaterActivity sap:VirtualizedContainerService.HintSize="257,100" Letter="[ltr]" />
      </ActivityAction>
    </ParallelForEach>
  </Sequence>
</Activity>
```

Tasarım zamanından da görüleceği üzere, ParallelFlow.xaml içerisinde ParallelForEach aktivite bileşeni yer almaktadır. ParallelForEach, char tipi ile çalışacak şekilde ayarlanmıştır ve Workflow için vSentence isimli argümanın değerini alıp, söz konusu cümledeki her bir harfi, içerdiği LetterCalculaterActivity bileşenine göndermektedir. Bu akıştan çalışma zamanındaki beklentimiz, söz konusu cümledeki harflerin paralel thread'lerin ele alacağı şekilde yorumlaması ve kullanılmasıdır. Bu arada unutmadan, Main metodunun içeriğinin aşağıdaki gibi olduğunu düşünelim.

```csharp
using System;
using System.Activities;

namespace ErrorHandlingForNativeActivities
{

    class Program
    {
        static void Main(string[] args)
        {
            ParallelFlow pFlow = new ParallelFlow();
            WorkflowInvoker.Invoke(pFlow);
            Console.WriteLine("İşlemler tamamlandı");
            Console.ReadLine();
        }
    }
}
```

ve buna göre çalışma zamanı sonuçlarına kısaca bir bakalım.

![blg155_FirstRuntime.gif](/assets/images/2010/blg155_FirstRuntime.gif)

Uppsss!!!

![Sealed](/assets/images/2010/smiley-sealed.gif)

Enteresan bir durum söz konusu. "Bu gün çok güzel bir gündü" cümlesi ters sırada işlenmiştir. Dahası tüm işlemler 1 numaralı ThreadId'ye bağlı olarak gerçekleştirilmektedir. Bir başka deyişle ParallelForEach aktivitesi istediğimiz/beklediğimiz şekilde çalışmamıştır. Sorun ne olabilir acaba?

![Undecided](/assets/images/2010/smiley-undecided.gif)

Aslında sorundan ziyade yanlış bir çözüm yolu izlediğimizi ifade edebiliriz. Esasında ParallelForEach bileşeninin bu senaryoda işe yarayabilmesi için içerisinde yer alan CodeActivity türevli bileşenin de paralel çalışmaya destek vermesi bir başka deyişle asenkron olarak yürütülebiliyor olması gerekmektedir. Ahaaa!!

![Wink](/assets/images/2010/smiley-wink.gif)

İşte şimdi çözümü bulduk. Buna göre LetterCalculaterActivity bileşeninin CodeActivity yerine AsyncCodeActivity tipinden türetilmesi ve kodlanması yeterlidir. O halde söz konusu bileşenimizi aşağıdaki şekilde değiştirelim.

![blg155_ClassDia2.gif](/assets/images/2010/blg155_ClassDia2.gif)

```csharp
using System;
using System.Activities;
using System.Threading;

namespace ErrorHandlingForNativeActivities
{
    public sealed class LetterCalculaterActivity
            : AsyncCodeActivity
    {
        public InArgument<char> Letter { get; set; }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            Func<char, bool> dlg = c =>
            {
                Thread.Sleep(1000);
                Console.WriteLine("{0} için hesaplamalar| Current Thread Id : {1}", c, Thread.CurrentThread.ManagedThreadId.ToString());
                return true;
            };

            context.UserState = dlg;
            return dlg.BeginInvoke(Letter.Get(context), callback, state);
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            bool r = ((Func<char, bool>)context.UserState).EndInvoke(result);
            Console.WriteLine("\t{0}", r);
        }
    }
}
```

Kod parçasına göre, temsilcilerin (Delegates) BeginInvoke ve EndInvoke metodlarından yararlanılarak aktivite içerisinde asenkron bir işin yürütülmesinin sağlandığını özetleyebiliriz.

> AsyncCodeActivity türevli bileşenlerin nasıl geliştirileceğini daha önceden [Workflow Foundation 4.0 - Custom Async Activity Geliştirmek [Beta 2]](https://www.buraksenyurt.com/post/Workflow-Foundation-40-Custom-Async-Activity) isimli yazımızda değerlendirmiştik.

Buna göre program kodumuzu yeniden test edersek, çalışma zamanında aşağıdakine benzer sonuçlar ile karşılaştığımızı görebiliriz.

![blg155_LastRuntime.gif](/assets/images/2010/blg155_LastRuntime.gif)

Çalışma sırası tam olarak şöyledir; "Bu gnü ç kogz eülbir günüd". Sakın bu cümleyi okumaya çalışmayın.

![Laughing](/assets/images/2010/smiley-laughing.gif)

Görüldüğü üzere farklı yönetimli Thread Id değerleri üretilmiş, üstelik "Bu gün çok güzel bir gündü" cümlesi aynı harf sırasına göre ele alınmamıştır. Bir başka deyişle paralel çalışma sağlanmıştır. Tabi çalışma zamanı ve çevresel donanım şartlarına göre bu sıralama her seferinde farklı sonuçlanabilir veya aynı sonuçlar tekrar tekrar elde edilebilir. Aslında bütün mesele de budur zaten. "Paralel olmak ya da olmamak". Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[ToBeOrNotToBe_RC.rar (48,75 kb)](/assets/files/2010/ToBeOrNotToBe_RC.rar) [Örnek uygulama Visual Studio 2010 Ultimate RC Sürümü Üzerinde Geliştirilmiştir ve Test Edilmiştir]
