---
layout: post
title: "Workflow Foundation 4.0 - Persistence [Beta 2]"
date: 2009-11-30 23:29:00 +0300
description: "Workflow Foundation modeli ile geliştirilen uzun süreli işlemlerde(Long Running Process) en önemli konulardan biriside, Workflow örneğinin herhangibir t anında kalıcı olarak saklanabilmesi(Persist) ve istenildiğinde saklandığı yerdeki içeriği ile birlikte tekrardan ayağa kaldırılabilmesidir..."
categories:
  - wf-4-0-beta-2
tags:
  - wf-4-0-beta-2
  - xml
  - csharp
  - dotnet
  - sql-server
  - workflow-foundation
  - xaml
  - transactions
  - visual-studio
---
# Workflow Foundation 4.0 - Persistence [Beta 2]
![blg104_Giris.jpg](/assets/images/2009/blg104_Giris.jpg)

Merhaba Arkadaşlar,

Bundan bir kaç yıl önce, eşimle birlike İtalya'daki Amca'sını ziyarete gitmiştik. Amcamız, Milano şehrinde uzun yıllar restoran işiyle uğraşmış oldukça yetenekli bir aşçı ve işletmeciydi. Bir gün İtalya'da bizi davet ettiği bir restoranda yemek yerken güzel bir tavsiyede bulunduğunu hatırlıyorum; "Yemekte bitiremediğiniz salatalar mı var? Yemekten sonra onları tavada ısıtın ve buzdolabına koyun. Ertesi gün yine soslayarak yiyebilirsiniz. Tabiki ağır soslu salatalar değilde hafif olanlardan bahsediyorum". Bir anlamda salatayı herhangibir anda o anki haliyle saklayıp (ki ben buna Persist etmek demek istiyorum) sonra t zamanında yeniden yemek. Her ne kadar bu senaryoda salatayı yıllarca saklamak zor olsada (ki bu durumdada bazı sonuçlarına katlanmak gerekebilir ![Sealed](/assets/images/2009/smiley-sealed.gif)) yazılım dünyasında uzun süreli bir iş akışının yıllarca saklanabilmesi mümkündür. İşte bu günkü konumuz; Workflow Foundation 4.0 ile Persistence işlemleri nasıl gerçekleştirilebilir...

Workflow Foundation modeli ile geliştirilen uzun süreli işlemlerde (Long Running Process) en önemli konulardan biriside, Workflow örneğinin herhangibir t anında kalıcı olarak saklanabilmesi (Persist) ve istenildiğinde saklandığı yerdeki içeriği ile birlikte tekrardan ayağa kaldırılabilmesidir. Persist edilecek verilerin nerede saklanacağına ilişkin olarak çalışma zamanının varsayılan tutumu belleği kullanmaktır. Ancak çok sayıda Workflow örneğinin Long Running Process olarak değerlendirildiği gerçek hayat vakalarında hem yönetim (Administration) hemde kalıcılığın daha kuvvetli olması adına SQL veritabanı ortamının değerlendirilmesi çok daha doğru bir davranıştır.

İlk versiyonlarından bu yana, Workflow tarafında persistence alanı olarak SQL veritabanının kullanılması bazı SQL Script'lerinin çalıştırılıp gerekli veritabanının oluşturulması ile başlamaktadır. Bu veritabanının Workflow çalışma zamanı tarafından kullanılacağının belirtilmesi sırasında bağlantı bilgisi verilmesi yeterlidir. Buda önemli bir noktadır ki veritabanını, Workflow uygulamalarını host edip çalıştıran sunucunun dışında bir yerde tutma şansına sahip olabiliriz. Yazımızın bundan sonraki kısımlarında, Workflow Foundation 4.0 Beta 2 üzerinden Persistence sistemini nasıl kullanabileceğimize çok basit bir örnek üzerinden bakmaya çalışacağız.

İşe ilk olarak WFPersistenceStore (ki siz istediğiniz bir veritabanı adını verebilirsiniz) isimli bir veritabanını oluşturarak başlayabiliriz. Bu işlemin ardından varsayılan olarak C:\WINDOWS\Microsoft.NET\Framework\v4.0.21006\SQL\en\ adresinde duran SqlWorkflowInstanceStoreSchema, SqlWorkflowInstanceStoreLogic sql script'lerini sırasıyla oluşturulan veritabanı üzerinde çalıştırmamız gerekmektedir. Söz konusu işlem sonrasında Workflow örneklerinin tutulması ile ilişkili olaraktan gerekli veritabanı nesnelerinin üretildiği görülebilir (Tables, Stored Procedures, Views vb...).

Workflow tarafında Persistence işlemleri için birden fazla yol kullanılabilir. Bunların içerisinde belkide en basiti Persist isimli aktivite bileşeninin Workflow içerisinde bir noktada değerlendirilmesidir. Bu yol dışında istenirse WorkflowApplication nesne örneğinin PersistableIdle özelliğide değerlendirilebilir. Ancak unutulmaması gereken noktalardan birisi de Workflow örneklerinin saklanma durumlarının aslında uzun süreli süreçler için geçerli oluşudur. Bu nedenle ilgili özellik ve konfigurasyon işlemleri esas itibariyle WorkflowApplication tipi üzerinden yapılmaktadır.

Dilerseniz daha fazla vakit kaybetmeden örneğimizi geliştirmeye başlayalım. İlk olarak basit bir Workflow Console Application projesi oluşturarak yolumuza devam edebiliriz. Workflow uygulamamızda SQL tabanlı bir Persistence sistemi kullanacağımızdan gerekli assembly'larında projeye referans edilmesi gerekmektedir. Bu amaçla uygulamamıza System.Activities.DurableInstancing.dll'ini eklememiz gerekmektedir. Ayrıca System.Runtime.dll assembly'ının 4.0 versiyonun da eklenmesi gerekmektedir. Söz konusu referans eklemeleri sonrasında tasarım zamanındaki görüntüsü aşağıdaki gibi olan çok basit bir Workflow geliştirerek devam edebiliriz.

![blg104_DesignerLast.gif](/assets/images/2009/blg104_DesignerLast.gif)

Workflow örneğine ait XAML içeriğinin sadece Sequence bölümünden oluşan parçası aşağıdaki gibidir.

```xml
<Sequence sad:XamlDebuggerXmlReader.FileName="c:\documents and settings\bsenyurt\my documents\visual studio 10\Projects\WFPersistence\WFPersistence\Workflow1.xaml" sap:VirtualizedContainerService.HintSize="486,614">
    <Sequence.Variables>
      <Variable x:TypeArguments="x:Int32" Name="CurrentNumber" />
    </Sequence.Variables>
    <sap:WorkflowViewStateService.ViewState>
      <scg3:Dictionary x:TypeArguments="x:String, x:Object">
        <x:Boolean x:Key="IsExpanded">True</x:Boolean>
      </scg3:Dictionary>
    </sap:WorkflowViewStateService.ViewState>
    <WriteLine sap:VirtualizedContainerService.HintSize="464,59" Text="[String.Format("Workflow başlangıcı {0}", DateTime.Now.ToLongTimeString())]" />
    <While sap:VirtualizedContainerService.HintSize="464,391" Condition="True">
      <Sequence sap:VirtualizedContainerService.HintSize="438,280">
        <sap:WorkflowViewStateService.ViewState>
          <scg3:Dictionary x:TypeArguments="x:String, x:Object">
            <x:Boolean x:Key="IsExpanded">True</x:Boolean>
          </scg3:Dictionary>
        </sap:WorkflowViewStateService.ViewState>
        <Assign sap:VirtualizedContainerService.HintSize="242,57">
          <Assign.To>
            <OutArgument x:TypeArguments="x:Int32">[CurrentNumber]</OutArgument>
          </Assign.To>
          <Assign.Value>
            <InArgument x:TypeArguments="x:Int32">[CurrentNumber + 1]</InArgument>
          </Assign.Value>
        </Assign>
        <WriteLine sap:VirtualizedContainerService.HintSize="242,59" Text="[String.Format("CurrentNumber : {0} ", CurrentNumber.ToString())]" />
      </Sequence>
    </While>
  </Sequence>
```

Tasarım ekranından görebileceğiniz üzere Workflow örneğimiz bir WriteLine ile başlamakta ve sonrasında sonsuz bir döngüye girmektedir. Bu sonsuz döngü için While bileşeninden yararlanılmaktadır. Sonsuz döngü içerisinde yer alan Assign bileşeni sayesinde, CurrentNumber isimli Sequence seviyesindeki Variable'ın değeri sürekli olarak arttırılmaktadır. Buna göre yazacağımız program kodunun önemi vardır. Workflow örneğini Host eden Console uygulamasını öyle yazmalıyızki, uygulama kapandığında Workflow örneğinin o anki hali Persistence tablolarına yazılabilsin. Bu nedenle Program.cs içeriğini aşağıdaki gibi geliştirebiliriz.

```csharp
using System;
using System.Activities;
using System.Activities.DurableInstancing;

namespace WFPersistence
{

    class Program
    {
        static void Main(string[] args)
        {
            string conStr = "data source=.;database=WFPersistenceStore;integrated security=SSPI";

            // Persistence işleminin yapılacağı veritabanını işaret edecek ve yönetecek nesne örneklenir
            SqlWorkflowInstanceStore instanceStore = new SqlWorkflowInstanceStore(conStr);

            // Workflow nesnesi örneklenir
            Workflow1 wf1 = new Workflow1();

            // WorkflowApplication nesnesi örnekleni ve wf1' i çalıştıracağı belirtilir
            WorkflowApplication wfApp = new WorkflowApplication(wf1);
            // WorkflowApplication nesne örneğinin persistence store olarak instanceStore isimli SqlWorkflowInstanceStore nesne örneğini kullanacağı ve dolayısıyla saklama işlemlerinin WFPersistenceStore veritabanında gerçekleştirileceği belirtilir
            wfApp.InstanceStore = instanceStore;

            // WorkflowApplication başlatılır
            wfApp.Run();

            Console.WriteLine("Uygulamadan çıkmak için bir tuşa basın");
            Console.ReadLine();

            // Unload metodunun çalıştırılması nedeniyle Workflow örneğinin o anda bulunduğu içerik itibariyle Persist edilmeside sağlanmaktadır.
            wfApp.Unload();

            Console.WriteLine("Uygulamadan çıkış zamanı {0}",DateTime.Now.ToLongTimeString());
        }
    }
}
```

Örneğimizi çalıştırmadan önce SQL Server Profiler aracınında açık olduğundan emin olalım. Böylece arka planda gerçekleşen insert işlemlerini görme şansımız olacaktır. Ben örneğimizi bu haliyle test ederken aşağıdaki sonuçlar ile karşılaştım. (Bilinçli olarak belirli süre geçtikten sonra uygulamayı kapattım)

![blg104_RuntimeLast2.gif](/assets/images/2009/blg104_RuntimeLast2.gif)

Dikkat edileceği üzer While döngüsünde CurrentNumber değeri 4158 iken çıkılmıştır. Tabiki sizin testlerinizde bu değer çok daha farklı olabilir. Uygulama Unload metodu çağrısını gerçekleştirdikten sonra SQL Server Profiler aracından yakalan sorgu aşağıdaki gibidir (SQL İçeriği çok uzun olduğundan son kısmı kırpılmıştır)

```csharp
exec sp_executesql N'begin transaction
declare @result int
exec @result = [System.Activities.DurableInstancing].[SaveInstance] @instanceId, @surrogateLockOwnerId, @handleInstanceVersion, @handleIsBoundToLock,
@primitiveDataProperties, @complexDataProperties, @writeOnlyPrimitiveDataProperties, @writeOnlyComplexDataProperties, @metadataProperties,
@metadataIsConsistent, @encodingOption, @pendingTimer, @suspensionStateChange, @suspensionReason, @keysToAssociate, @keysToComplete,
@keysToFree, @concatenatedKeyProperties, @unlockInstance, @isReadyToRun, @isCompleted,
@lastMachineRunOn, @executionStatus, @blockingBookmarks, @operationTimeout ;
if (@result = 0)
begin
commit transaction
end
else
rollback transaction
',N'@instanceId uniqueidentifier,@surrogateLockOwnerId bigint,@handleInstanceVersion bigint, @handleIsBoundToLock bit, @pendingTimer datetime,@unlockInstance bit,@suspensionStateChange tinyint,@suspensionReason nvarchar(450),@isCompleted bit,@isReadyToRun bit, @operationTimeout int,@keysToAssociate xml,@keysToComplete xml, @keysToFree xml,@concatenatedKeyProperties varbinary(8000),@primitiveDataProperties varbinary(8000), @complexDataProperties varbinary(1664), @writeOnlyPrimitiveDataProperties varbinary(470) ,@writeOnlyComplexDataProperties varbinary(860),@metadataProperties varbinary(8000), @metadataIsConsistent bit,@encodingOption tinyint, @lastMachineRunOn varchar(900),@executionStatus varchar(900),@blockingBookmarks varchar(900)',

@instanceId='074A5D57-C268-43E6-B516-DFE49894D7A7',

@surrogateLockOwnerId=26,@handleInstanceVersion=-1, @handleIsBoundToLock=0,@pendingTimer=NULL,@unlockInstance=1, @suspensionStateChange=0,@suspensionReason=NULL,@isCompleted=0,@isReadyToRun=1, @operationTimeout=29922, @keysToAssociate=NULL,@keysToComplete=NULL ,@keysToFree=NULL, @concatenatedKeyProperties=NULL,@primitiveDataProperties=NULL, @complexDataProperties=...
```

Görüldüğü üzere Transaction içerisine alınmış olan ve SaveInstance isimli Stored Procedure için yapılan bir çağrısı söz konusudur. Yapılan çağrıda şu an için dikkat edilmesi gereken nokta parametre olarak gelen instanceId değeridir. Bu işlemin ardından Instances isimli View içeriğine bakılırsa söz konusu InstanceId değerini içeren bir satırın üretildiği görülmektedir.

![blg104_SqlTableLast.gif](/assets/images/2009/blg104_SqlTableLast.gif)

Peki ya şimdi ne olacak?

T zaman sonra (Gün, Ay, Yıl bile olabilir. Yeterki SQL tarafındaki bilgilere zarar gelmesin), bu Workflow örneğinin kaldığı yerden ayağa kaldırılarak devam etmesi istenebilir. İşte bu durumda Persist edilmiş olan örneğin, kaydedildiği haliyle tekrardan yüklenmesi gerekecektir. Geliştirdiğimiz örnek senaryoya göre, CurrentNumber değerinin kaldığı yerden başlayarak devam etmesi gerekmektedir. Persist edilmiş olan bir Workflow örneğinin tekrardan ayağa kaldırılması için GUID tipinden olan InstanceId değerinin Load metoduna parametre olarak geçirilmesi yeterlidir. İşte Persist edilmiş olan Workflow örneğini ayağa kaldırmak için kullanacağımız kodlarımız.

![blg104_Code.gif](/assets/images/2009/blg104_Code.gif)

Tam Load metodunun olduğu yerde breakpoint koyarak ilerlemenizi öneririm. Persist edilen Workflow örneğinin canlandırılması esnasında SQL tarafında LoadInstance isimli bir Stored Procedure'ün çalıştırılması sağlanmaktadır. İşte çalıştırılan SQL sorgusu.

```text
exec [System.Activities.DurableInstancing].[LoadInstance] @surrogateLockOwnerId=27,@operationType=3, @keyToLoadBy='00000000-0000-0000-0000-000000000000',

@instanceId='074A5D57-C268-43E6-B516-DFE49894D7A7',

@handleInstanceVersion=-1,@handleIsBoundToLock=0,@keysToAssociate=NULL, @encodingOption=1,@concatenatedKeyProperties=NULL, @operationTimeout=29812
```

Hatta bu noktada iken IsLocked alanın söz konusu Workflow örneği için 1 olarak set edildiği gözlemlenebilir. Bir başka deyişle Workflow örneği bu işlem sırasında diğer bir talebe kapatılmış durumdadır.

![blg104_SqlTable2.gif](/assets/images/2009/blg104_SqlTable2.gif)

Peki ya uygulamanın durumu? Uygulama yeniden çalıştırıldığında CurrentNumber değerinin kaldığı yerden devam ettiği gözlemlenecektir. Bunu görme zevkini siz değerli okurlarıma bırakıyorum

![Wink](/assets/images/2009/smiley-wink.gif)

Persistence işlemi bu örneğimizde WorkflowApplication nesnesi tarafından ele alınmıştır. Ancak Workflow Service isimli önemli bir gerçekde vardır. Bir Workflow Service'in uzun süreli olarak saklanması da, gerçek hayat senaryolarında yaşanan vakalardan birisidir. Bu durumu bir sonraki yazımızda ele almaya çalışacağız. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

> Kişisel Not: Kalıcı olarak saklama işlemini yazımızın başında belirttiğimiz üzere, Persist aktivitesi ilede kolayca gerçekleştirebiliriz. Bu konu ile ilişkili bir görsel dersi ilerleyen tarihlerde yayınlamaya çalışıyor olacağım.

[WFPersistence.rar (37,63 kb)](/assets/files/2009/WFPersistence.rar)