---
layout: post
title: "Workflow Foundation 4.0 - Custom Async Activity Geliştirmek [Beta 2]"
date: 2010-01-10 15:45:00 +0300
categories:
  - wf-4-0-beta-2
tags:
  - workflow-foundation
---
Hatırlayacağınız üzere bir önceki blog yazımızda Workflow Foundation 4.0 üzerinde özel aktiviteleri nasıl geliştirebileceğimizi incelemeye başlamış ve bu anlamda ilk olarak CodeActivity türevli bir bileşen üretmiştik. Workflow Foundation 4.0 ile gelen önemli yeniliklerden biriside asenkron aktivite bileşenlerini içeriyor olmasıdır. Özellikle.Net Framework 4.0 tarafında üzerinde ağırlıklı olarak durulmaya başlanan paralel programlamanın da bir sonucu olan bu durum karşısında, geliştiricilerin asenkron olarak çalışabilen aktivite bileşenleri yazması pek tabidir.

Asenkron aktivitelerde iki temel operasyon bulunmaktadır. Bu operasyonlardan birisi asenkron olarak çalışacak işlerin başlatıcısı iken, diğerde tamamlandığında devreye girecek olanıdır. Bunlara ilaveten opsiyonel olarak asenkron işlemin iptal edilebilmesi için gerekli operasyonda eklenebilir.

![blg109_Cancel.gif](/assets/images/2010/blg109_Cancel.gif)

Bu operasyonları içeren asenkron aktiviteleri geliştirmek için AsyncCodeActivity tipinden türetme (Inherit) yapmak yeterlidir.

Dilerseniz hiç vakit kaybetmeden basit bir örnek üzerinden devam edelim. Bu amaçla bir önceki projemizde yer alan aktivite kütüphanemize BulkMailActivity isimli yeni bir Code Activity öğesi ekleyelim. Bu bileşen toplu olarak mail gönderme işlemlerini üstlenecektir. Gerçektende sürecin içerisinde bir noktada yer alabilen toplu mail gönderme adımının, sürecin kalan kısmını duraksatmaması istenebilir. Böyle bir vakada asenkron olarak mail gönderme işlemini üstelenecek bir aktivite çok yararlı olacaktır. BulkMailActivity isimli bileşenimizin sınıf diagramı görüntüsü ve kod içeriği ise aşağıdaki gibidir.

![blg109_ClassDiagram.gif](/assets/images/2010/blg109_ClassDiagram.gif)

```csharp
using System;
using System.Activities;
using System.Collections.Generic;
using System.Net.Mail;

namespace ActivityLibrary2
{
    public sealed class BulkMailActivity 
        : AsyncCodeActivity<bool>
    {
        public InArgument<string[]> MailList{ get; set; }
        public InArgument<string> MailBody { get; set; }
        public InArgument<string> MailSubject { get; set; }

        private SmtpClient smtp;

        public BulkMailActivity()
        {
            smtp = new SmtpClient("localhost");
        }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            MailMessage message = new MailMessage
            {                 
                  Body=MailBody.Get(context),
                   Subject=MailSubject.Get(context),
                   From=new MailAddress("admin@wf4.com")
            };
            message.To.Add(String.Join(",", MailList.Get(context)));

            Func<MailMessage, bool> SendDelegate = new Func<MailMessage, bool>(Send);
            context.UserState = SendDelegate;
            return SendDelegate.BeginInvoke(message, callback, state);
        }

        protected override bool EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            Func<MailMessage, bool> SendDelegate = (Func<MailMessage, bool>)context.UserState;
            return SendDelegate.EndInvoke(result);
        }

        bool Send(MailMessage message)
        {
            smtp.Send(message);
            return true;
        }
    }
}
```

Dikkat edileceği üzere AsyncCodeActivity türetmesinden dolayı BeginExecute ve EndExecute isimli iki metodun ezilmesi (override) gerekmektedir. BeginExecute metodu içerisinde ise Func tipinden bir temsilci (delegate) kullanılarak asenkron olarak yürütülecek metodun işaret edilmesi sağlanmaktadır. Çok doğal olarak asenkron kodlamada ana fikir, temsilcilerin (delegate) üzerinden çağırılan BeginInvoke ve EndInvoke metodlarıdır. Func temsilcisi sayesinde parametre ve geri dönüş tipi belli olan metodun işaret edilerek kullanılabilmesi sağlanmaktadır. Func temsilcisinin bu örnekte kullanılan versiyonuna göre ilk parametre in tipindendir ve Send metoduna gönderilebilecek olan parametreyi belirtmektedir. İkinci generic parametre ise out tipinden olup, Send metodundan döndürülecek olan tipi belirtmektedir. Dikkat edilmesi gereken noktalardan bir diğeride, BeginExecute ve EndExecute metodları arasında veri paylaşımının nasıl yapıldığıdır.

![blg109_Context.gif](/assets/images/2010/blg109_Context.gif)

Yukarıdaki şekil sanıyorum ki bu konuda bir fikir vermektedir. Her iki metodda AsyncCodeActivityContext tipinden bir parametre almaktadır. BeginExecute metodunda bu parametrenin UserState özelliğine SendDelegate isimli Func temsilci örneği atanmıştır. Buna göre EndExecute metodu içerisinde SendDelegate referansının yakalanması ve çok doğal olarak EndInvoke metodunun çağırılabilmesi mümkündür. Aktivitemiz dışarıdan mail listesini, mail gövdesini ve konu kısımlarını almaktadır ki çok daha fazla parametre alabilir.(Size önerim MailMessage tipinin alabileceği tüm özellikleri içerecek bir mail gönderme aktivite bileşenini geliştirmeye çalışmanızdır) Gönderme işlemi sırasında makine üzerindeki varsayılan SMTP tipi sunucusu kullanılır. Bu nedenle örneğin kendi bilgisayarınızda çalışması sırasında mail gönderme işleminin gerçekleşmemesi mümkündür.

![Sealed](/assets/images/2010/smiley-sealed.gif)

Gelelim test kısmına. Bu amaçla TestScene.xaml içeriğini aşağıdaki gibi değiştirelim.

![blg109_DesignTime.gif](/assets/images/2010/blg109_DesignTime.gif)

Görüldüğü üzere bileşenimizin ilgili özelliklerine bazı test verileri aktarılmıştır. MailList özelliği aslında bir String dizisi olduğundan değer ataması yapılırken süslü parantezli bir yazım stili kullanılmalıdır. Bunun dışında mail gönderme bileşeni sonuç olarak bool bir değer üretmektedir. Bu değer SendResult isimli aktivite bazındaki argümana atanmaktadır.

Xaml kısmı (Sadece bir kısmı verilmiştir);

```xml
<x:Members>
    <x:Property Name="SendResult" Type="InArgument(x:Boolean)" />
  </x:Members>
  <mva:VisualBasic.Settings>Assembly references and imported namespaces serialized as XML namespaces</mva:VisualBasic.Settings>
  <Sequence sad:XamlDebuggerXmlReader.FileName="C:\Documents and Settings\bsenyurt\my documents\visual studio 10\Projects\ActivityLibrary2\ActivityLibrary2\TestScene.xaml" sap:VirtualizedContainerService.HintSize="222,200">
    <Sequence.Variables>
      <Variable x:TypeArguments="sd1:DataTable" Name="QueryResult" />
    </Sequence.Variables>
    <sap:WorkflowViewStateService.ViewState>
      <scg3:Dictionary x:TypeArguments="x:String, x:Object">
        <x:Boolean x:Key="IsExpanded">True</x:Boolean>
      </scg3:Dictionary>
    </sap:WorkflowViewStateService.ViewState>
    <local:BulkMailActivity sap:VirtualizedContainerService.HintSize="200,22" MailBody="Deneme mailidir" MailList="[{"birmailadresi@bimail.com", "ikincimailadresi@bimail.com"}]" MailSubject="Konu Yok" Result="[SendResult]" />
  </Sequence>
```

Tabiki bu örnekte sadece async tipinden bir code activity bileşeninin nasıl geliştirilebileceği ele alınmıştır. Aslında ilave edilmesi gereken ek özelliklerde yok değildir. Söz gelimi mail gönderme işlemi sırasında oluşabilecek istisnalar (Exception) sonrasında nasıl davranılacaktır. Bunun için bir Exception Handling mekanizmasının kullanılması, bir başka deyişle try...catch bloklarına başvurulması gerekebilir. Diğer yandan mailleri virgül ile ayırıp toplu şekilde göndermek yerine tek tek gönderilmesi yolu da tercih edilebilir. Nitekim şu durumda, herhangibir maile yapılan gönderi sırasında oluşacak istisna sonrası kalan gönderme işlemleride icra edilmeyecektir. Sözün özü gerçek hayat senaryolarında bu tip bir bileşenin çok daha titiz yazılması şarttır. Bizim bu yazı için odaklanmamız gereken noktalar ise şunlardır;

- Asenkron aktivite bileşenleri geliştirmek için AsyncCodeActivity tipinden türetme yapılır.
- Türetme sonrası BeginExecute ve EndExecute metodları ezilir.
- İstenirse iptal işlemlerinin ele alınabilmesi amacıyla Cancel metoduda ezilebilir.
- Asenkron olarak çalışacak işin doğasında BeginInvoke veya EndInvoke kabiliyetleri yoksa Func gibi temsilcilerden yararlanılabilir.
- BeginExecute ve EndExecute metodları arasında veri taşınması gerektiğinde, ortak parametre olan AsyncCodeActivityContext tipinden yararlanılır. Örneğimizdeki gibi illede temsilcinin taşınmasına gerek yoktur. Söz konusu parametre ortak olarak kullanılacak bir referansın taşınması istendiğinde değerlendirilmelidir.
- Çalışma zamanı, Workflow içerisinde bir asenkron bileşen ile karşılaştığında bunu yürütmeye başlar ve hemen sonraki adımdan işleyişine devam eder.

Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[ActivityLibrary2Async.rar (54,25 kb)](/assets/files/2010/ActivityLibrary2Async.rar)