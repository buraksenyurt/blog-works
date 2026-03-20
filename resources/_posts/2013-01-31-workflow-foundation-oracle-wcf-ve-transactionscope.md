---
layout: post
title: "Workflow Foundation, Oracle, WCF ve TransactionScope"
date: 2013-01-31 22:36:00 +0300
categories:
  - wf-4-0
tags:
  - wf-4-0
  - csharp
  - xml
  - dotnet
  - linq
  - oracle
  - wcf
  - workflow-foundation
  - xaml
  - http
  - transactions
  - generics
  - debugging
  - windows-phone
  - visual-studio
  - atomic-operations
---
Yandaki fotoğrafta görülen buluşa baktığınızda aslında gerçekten bu pilotun o koca pervaneler ile uçup uçamayacağına pek kanaat getiremiyoruz öyle değil mi? Sonuçta en azından kağıt üstünde ve teorik olarak da bu tip bir uçuş aracının çalışacağının ispat edilmesi ve sonrasında pratikteki kullanımı için teste çıkılması beklenir (Tabi buna cesaret edecek de bir pilotun olması gerekir) Bir dostumuzun söylediği üzere "tasarlanan her uçak uçmuş ama her yazılım çalışmamıştır"

[![Proof-of-Concept-Prototypes-Dont-Have-to-Be-as-Complicated-as-This](/assets/images/2013/Proof-of-Concept-Prototypes-Dont-Have-to-Be-as-Complicated-as-This_thumb.jpg)](/assets/images/2013/Proof-of-Concept-Prototypes-Dont-Have-to-Be-as-Complicated-as-This.jpg)

Bir başka deyişle yazılım tarafında bir şeylerin ispatını yaparken bir uçağı uçuracakmış gibi düşünerek hareket etmeyiz genelde. İstesek de edemiyoruz sanırım. Yine de elimizden geldiğince titiz çalışmamız da yarar var. Öyleyse gelelim bu günün konusuna.

Geçtiğimiz günlerde Workflow Foundation tabanlı bir uygulama içerisinde Transaction Scope kullanımına ihtiyacım oldu. Transaction'a dahil olan işlemler Oracle tabloları üzerinde gerçekleştirilecekti. Senaryoyu zorlaştıran noktalardan birisi ise, akış içerisinde harici bir WCF servis çağrısının yapılacak olmasıydı. Nitekim söz konusu WCF servisi içerisindeki operasyonda da, yine Oracle veritabanı üzerinde yapılması planlanan Transactional bir işlem söz konusuydu.

Dolayısıyla Oracle'ın ve WCF servisinin işin içerisinde yer aldığı bir Workflow senaryosunda, TransactionScope bileşeninin işe yarayıp yaramadığının araştırılması ve gerekli ispatların yapılması gerekmekteydi. Bir başka deyişle bir POC (Proof Of Concept) çalışması ile karşı karşıyaydık. Ben de Visual Studio 2010 un başına oturdum ve yola koyuldum.

Senaryoda kendi oluşturduğum iki Oracle tablosu söz konusu idi. Bu tabloların içerdiği alanların çok fazla önemi yoktu aslında. Account ve Branch olarak adlandırdığım tablolar üzerinde iki basit Insert işlemi gerçekleştirecektim. Buna ek olarak bir de WCF servisi söz konusu olmalıydı. İlgili WCF servisi de yine benzer bir Insert işlemi gerçekleştirilmek üzere tasarlanmalıydı.

Teorik olarak Workflow Activity'si içerisinde kullanılacak TransactionScope Component'i servis tarafına Transaction'ı aktarabilmeliydi. Tam bu noktada Distributed Transaction Coordinator servisinin de işlevselliği söz konusuydu. Elbette servis tarafının da, Transaction akışına izin verecek şekilde tesis edilmesi şarttı.

> Bu noktada özellikle Binding tipinin Transaction Flow'a destek veriyor olması önemlidir. Nitekim varsayılan bağlayıcı tip olan BasicHttpBinding, istemciden gelen Transaction akışına izin vermez. Ama örneğin wsHttpBinding buna olanak tanır.

Dilerseniz işe ilk olarak bu WCF servisini geliştirerek başlayalım. Servisimize ait basit kod içeriği aşağıdaki gibidir.

```csharp
using System.ServiceModel;

namespace CRUDer 
{ 
    [ServiceContract] 
    public interface IAccountService 
    { 
        [OperationContract] 
        [TransactionFlow(TransactionFlowOption.Mandatory)] 
        int DoWork(int AccountId,string Name,string Surname); 
    } 
}
```

Sözleşme tipi içerisindeki en önemli nokta TransactionFlow niteliğinin kullanılması ve Mandatory değerinin verilmesidir. Bu nitelik Allowed, NotAllowed şeklinde iki farklı değer daha alabilir. Mandatory, istemci tarafında bir Transaction Scope başlatılma zorunluluğunu ifade etmektedir. Nitekim operasyon içerisindeki işlemin bir Transaction Scope'a dahil olması şarttır.

```csharp
using System.Configuration; 
using System.ServiceModel; 
using System.Transactions; 
using Oracle.DataAccess.Client;

namespace CRUDer 
{ 
    //[ServiceBehavior(TransactionIsolationLevel=IsolationLevel.Serializable)] 
    public class AccountService 
        : IAccountService 
    { 
       [OperationBehavior(TransactionScopeRequired=true, TransactionAutoComplete=true)] 
        public int DoWork(int AccountId, string Name, string Surname) 
        { 
            int result = -1;

            var currentTransaction=Transaction.Current; 
            using (OracleConnection conn = new OracleConnection(ConfigurationManager.ConnectionStrings["ConStr"].ConnectionString)) 
            { 
                using (OracleCommand command = new OracleCommand("INSERT INTO ACCOUNT (ACCOUNTID,NAME,SURNAME) VALUES (:pACCOUNTID,:pNAME,:pSURNAME)", conn)) 
                { 
                    command.Parameters.Add(":pACCOUNTID",AccountId); 
                    command.Parameters.Add(":pNAME", Name); 
                    command.Parameters.Add(":pSURNAME", Surname);

                    conn.Open(); 
                    result = command.ExecuteNonQuery(); 
                } 
            }

            return result; 
        } 
    } 
}
```

Servisin asıl iş yapan kodlarında yine dikkat çekici ve önemli olan nokta OperationBehavior niteliği ile atanan özellik (Property) değerleridir. Operasyonun bir TransactionScope gerektirdiği ve ayrıca unhandled exception oluşması halinde operasyona ait transaction örneğinin otomatik olarak tamamlanıp tamamlanmayacağı belirtilmektedir.

> Servis tarafında System.Transactions assembly'ının referans edilmesi unutulmamalıdır. Ayrıca hem servis hem de istemci tarafı Oracle fonksiyonellikleri için [ODP.Net'e (Oracle DataProvider for.Net)](http://www.oracle.com/technetwork/topics/dotnet/index-085163.html) ait assembly'ları kullanmaktadır. Dolayısıyla Oracle.DataAccess assembly’ ının uygun olan versiyonunun referans edilmelidir.

Servis tarafındaki config dosyasının içeriği de oldukça önemlidir. Daha önceden de belirttiğimiz üzere Binding tipi, Transaction akışına izin verir nitelikte olmalıdır. Ancak buna ek olarak Transaction akışına izin verileceğinin de konfigurasyon dosyası içerisinde belirtilmesi şarttır. Çünkü varsayılan olarak Transaction akışı etkin değildir.

```xml
<?xml version="1.0"?> 
<configuration> 
    <connectionStrings> 
        <add name="ConStr" connectionString="User Id=bir kullanıcı adı;Password=bir şifre;Data Source=oracle veri kaynağı" providerName="Oracle.DataAccess.Client"/> 
    </connectionStrings> 
  <system.serviceModel> 
    <services> 
      <service name="CRUDer.AccountService"> 
        <endpoint address="http://localhost:35662/AccountService.svc" 
          binding="wsHttpBinding" bindingConfiguration="wsB" contract="CRUDer.IAccountService" /> 
      </service> 
    </services> 
      <bindings> 
          <wsHttpBinding> 
              <binding transactionFlow="true" name="wsB"/> 
          </wsHttpBinding> 
      </bindings> 
    <behaviors> 
      <serviceBehaviors> 
        <behavior name=""> 
          <serviceMetadata httpGetEnabled="true"/> 
          <serviceDebug includeExceptionDetailInFaults="true"/> 
        </behavior> 
      </serviceBehaviors> 
    </behaviors> 
    <serviceHostingEnvironment multipleSiteBindingsEnabled="false"/> 
  </system.serviceModel> 
  <system.web> 
    <compilation debug="true"/> 
  </system.web> 
</configuration>
```

Dikkat edileceği üzere wsHttpBinding elementi içerisinde yer alan binding alt elementinin transactionFlow niteliğinin değeri true olarak set edilmiştir. Servis tarafında yapılması gerekenler aslında bu kadardır. Öyleyse Workflow tasarımına başlayabiliriz. Workflow Console Application olarak tasarlanan uygulamamızda çalışma zamanını izleyebilmek için basit de bir Tracker tipine yer verilmektedir. Bu tipin içeriği aşağıdaki gibidir.

```csharp
using System; 
using System.Activities.Tracking; 
using System.Collections.Generic; 
using System.Text;

namespace HowToTransaction 
{ 
    public class CustomTracker 
        : TrackingParticipant 
    { 
        protected override void Track(TrackingRecord record, TimeSpan timeout) 
        { 
            WorkflowInstanceRecord instanceRecord = record as WorkflowInstanceRecord; 
            if (instanceRecord != null) 
                Console.WriteLine(instanceRecord.ToString());

            ActivityStateRecord activity = record as ActivityStateRecord; 
            if (activity != null) 
            {                
                var variables = activity.Variables; 
                StringBuilder builder = new StringBuilder(); 
                if (variables.Count > 0) 
                { 
                    builder.AppendLine(" Variables:"); 
                    foreach (KeyValuePair<string, object> variable in variables) 
                    { 
                        builder.AppendLine(String.Format(" {0} Value: [{1}]", variable.Key, variable.Value)); 
                    } 
                } 
                Console.WriteLine( 
                    String.Format(" Activity: {0} State: {1} {2}", 
                    activity.Activity.Name 
                    , activity.State                    
                    , builder.ToString()) 
                    ); 
            } 
        } 
    } 
}
```

Tracker sınıfı içerisinde hem Workflow Actvitiy'si hem de içeride yürümekte olan Activity örneklerinin çalışmalarının izlenmesi işlemi gerçekleştirilmektedir. Bu, TrackingRecord tipinden olan parametrenin WorkflowInstanceRecord ve ActivityStateRecord nesne örneklerine dönüştürülmesi ile gerçekleştirilmektedir. Tracker tipi dışında Account ve Branch tablolarına insert işlemi yapan iki Code Activiy bileşeni de bulunmaktadır. Söz konusu Activity bileşenlerinin kodları aşağıdaki gibidir.

Account insert işlemini üstlenen Activity,

```csharp
namespace HowToTransaction 
{ 
    using System.Activities; 
    using System.Configuration; 
    using Oracle.DataAccess.Client;

    public class InsertAccountActivity 
        :CodeActivity 
    { 
        public InArgument<int> AccountId { get; set; } 
        public InArgument<string> Name { get; set; } 
        public InArgument<string> Surname { get; set; } 
        public OutArgument<int> ExecuteNonQueryResult { get; set; }

        protected override void Execute(CodeActivityContext context) 
        { 
            using (OracleConnection conn = new OracleConnection(ConfigurationManager.ConnectionStrings["ConStr"].ConnectionString)) 
            { 
                using (OracleCommand command = new OracleCommand("INSERT INTO ACCOUNT (ACCOUNTID,NAME,SURNAME) VALUES (:pACCOUNTID,:pNAME,:pSURNAME)", conn)) 
                { 
                    command.Parameters.Add(":pACCOUNTID",context.GetValue(AccountId)); 
                    command.Parameters.Add(":pNAME",context.GetValue(Name)); 
                    command.Parameters.Add(":pSURNAME",context.GetValue(Surname));

                    conn.Open(); 
                    int result=command.ExecuteNonQuery(); 
                    context.SetValue(ExecuteNonQueryResult, result); 
                } 
            } 
        } 
    } 
}
```

ve Branch insert işlemini üstlenen Code Activity

```csharp
namespace HowToTransaction 
{ 
    using System.Activities; 
    using System.Configuration; 
    using Oracle.DataAccess.Client; 
    using System;

    public class InsertBranchActivity 
        :CodeActivity 
    { 
        public InArgument<int> BranchId { get; set; } 
        public InArgument<string> Title { get; set; } 
        public InArgument<int> Code { get; set; } 
        public OutArgument<int> ExecuteNonQueryResult { get; set; }

        protected override void Execute(CodeActivityContext context) 
        { 
            using (OracleConnection conn = new OracleConnection(ConfigurationManager.ConnectionStrings["ConStr"].ConnectionString)) 
            { 
                using (OracleCommand command = new OracleCommand("INSERT INTO BRANCH (BRANCHID,TITLE,CODE) VALUES (:pBRANCHD,:pTITLE,:pCODE)", conn)) 
                { 
                    command.Parameters.Add(":pBRANCHID",context.GetValue(BranchId)); 
                    command.Parameters.Add(":pTITLE",context.GetValue(Title)); 
                    command.Parameters.Add(":pCODE",context.GetValue(Code));

                    conn.Open(); 
                    int result = command.ExecuteNonQuery(); 
                    context.SetValue(ExecuteNonQueryResult, result); 
                   //throw new Exception("Some error"); 
                } 
            } 
        } 
    } 
}
```

Burada yorum satırı olarak bırakılmış kısım daha sonradan yapılacak testler sırasında açılacak ve Transaction Scope'un çalışması izlenecektir.

Gelelim test amaçlı kullanacağımız Workflow Activity içeriğine.

[![wfts_1](/assets/images/2013/wfts_1_thumb.png)](/assets/images/2013/wfts_1.png)

FlowChart şeklinde tasarladığımız akışın içerisindeki en kritik yer TryCatch bileşenin içerisidir.

[![wfts_2](/assets/images/2013/wfts_2_thumb.png)](/assets/images/2013/wfts_2.png)

Try bloğundan TransactionScope bileşeni altında sırasıyla Account Insert işlemi, DoWork ile WCF servis çağrısı ve tekrar Branch Insert işlemi gerçekleştirilmektedir.

> WCF servisinin Workflow uygulamasına Add Service Reference ile eklenmesi sonrası Component sekmesine çıkan aktivite bileşeni kullanılmaktadır (DoWork bileşeni)
> [![wfts_3](/assets/images/2013/wfts_3_thumb.png)](/assets/images/2013/wfts_3.png)

Workflow un XAML (eXtensibleApplicationMarkupLanguage) içeriği aşağıdaki gibidir. Burada, kullanılan variable’ lar daha net bir şekilde görülebilmektedir.

```xml
<Activity mc:Ignorable="sads sap" x:Class="HowToTransaction.Workflow1" sap:VirtualizedContainerService.HintSize="654,676" mva:VisualBasic.Settings="Assembly references and imported namespaces for internal implementation" 
xmlns="http://schemas.microsoft.com/netfx/2009/xaml/activities" 
xmlns:av="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
xmlns:hc="clr-namespace:HowToTransaction.CRUDerReference" 
xmlns:local="clr-namespace:HowToTransaction" 
xmlns:local1="clr-namespace:HowToTransaction.CRUDerReference.Activities" 
xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" 
xmlns:mv="clr-namespace:Microsoft.VisualBasic;assembly=System" 
xmlns:mva="clr-namespace:Microsoft.VisualBasic.Activities;assembly=System.Activities" 
xmlns:p="http://schemas.microsoft.com/netfx/2009/xaml/servicemodel" 
xmlns:s="clr-namespace:System;assembly=mscorlib" 
xmlns:s1="clr-namespace:System;assembly=System" 
xmlns:s2="clr-namespace:System;assembly=System.Xml" 
xmlns:s3="clr-namespace:System;assembly=System.Core" 
xmlns:s4="clr-namespace:System;assembly=System.ServiceModel" 
xmlns:sa="clr-namespace:System.Activities;assembly=System.Activities" 
xmlns:sad="clr-namespace:System.Activities.Debugger;assembly=System.Activities" 
xmlns:sads="http://schemas.microsoft.com/netfx/2010/xaml/activities/debugger" 
xmlns:sap="http://schemas.microsoft.com/netfx/2009/xaml/activities/presentation" 
xmlns:scg="clr-namespace:System.Collections.Generic;assembly=System" 
xmlns:scg1="clr-namespace:System.Collections.Generic;assembly=System.ServiceModel" 
xmlns:scg2="clr-namespace:System.Collections.Generic;assembly=System.Core" 
xmlns:scg3="clr-namespace:System.Collections.Generic;assembly=mscorlib" 
xmlns:sd="clr-namespace:System.Data;assembly=System.Data" 
xmlns:sl="clr-namespace:System.Linq;assembly=System.Core" 
xmlns:st="clr-namespace:System.Text;assembly=mscorlib" 
xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"> 
  <Flowchart sad:XamlDebuggerXmlReader.FileName="d:\users\bsenyurt\documents\visual studio 2010\Projects\Windows Phone\HowToTransaction\HowToTransaction\Workflow1.xaml" sap:VirtualizedContainerService.HintSize="614,636"> 
    <sap:WorkflowViewStateService.ViewState> 
      <scg3:Dictionary x:TypeArguments="x:String, x:Object"> 
        <x:Boolean x:Key="IsExpanded">False</x:Boolean> 
        <av:Point x:Key="ShapeLocation">270,2.5</av:Point> 
        <av:Size x:Key="ShapeSize">60,75</av:Size> 
        <av:PointCollection x:Key="ConnectorLocation">300,77.5 300,139.5</av:PointCollection> 
      </scg3:Dictionary> 
    </sap:WorkflowViewStateService.ViewState> 
    <Flowchart.StartNode> 
      <FlowStep x:Name="__ReferenceID0"> 
        <sap:WorkflowViewStateService.ViewState> 
          <scg3:Dictionary x:TypeArguments="x:String, x:Object"> 
            <av:Point x:Key="ShapeLocation">194.5,139.5</av:Point> 
            <av:Size x:Key="ShapeSize">211,59</av:Size> 
            <av:PointCollection x:Key="ConnectorLocation">300,198.5 300,228.5 160,228.5 160,245.5</av:PointCollection> 
          </scg3:Dictionary> 
        </sap:WorkflowViewStateService.ViewState> 
        <WriteLine sap:VirtualizedContainerService.HintSize="211,59" Text="Account ve Branch tabloları için Insert işlemleri başlıyor"> 
          <sap:WorkflowViewStateService.ViewState> 
            <scg3:Dictionary x:TypeArguments="x:String, x:Object"> 
              <x:Boolean x:Key="IsExpanded">True</x:Boolean> 
            </scg3:Dictionary> 
          </sap:WorkflowViewStateService.ViewState> 
        </WriteLine> 
        <FlowStep.Next> 
          <FlowStep x:Name="__ReferenceID2"> 
            <sap:WorkflowViewStateService.ViewState> 
              <scg3:Dictionary x:TypeArguments="x:String, x:Object"> 
                <av:Point x:Key="ShapeLocation">60,245.5</av:Point> 
                <av:Size x:Key="ShapeSize">200,49</av:Size> 
                <av:PointCollection x:Key="ConnectorLocation">160,294.5 160,324.5 290,324.5 290,340.5</av:PointCollection> 
              </scg3:Dictionary> 
            </sap:WorkflowViewStateService.ViewState> 
            <TryCatch sap:VirtualizedContainerService.HintSize="418,529"> 
              <TryCatch.Variables> 
                <Variable x:TypeArguments="x:Int32" Name="ServiceCallResult" /> 
              </TryCatch.Variables> 
              <sap:WorkflowViewStateService.ViewState> 
                <scg3:Dictionary x:TypeArguments="x:String, x:Object"> 
                  <x:Boolean x:Key="IsExpanded">True</x:Boolean> 
                </scg3:Dictionary> 
              </sap:WorkflowViewStateService.ViewState> 
              <TryCatch.Try> 
                <TransactionScope AbortInstanceOnTransactionFailure="False" sap:VirtualizedContainerService.HintSize="258,351"> 
                  <sap:WorkflowViewStateService.ViewState> 
                    <scg3:Dictionary x:TypeArguments="x:String, x:Object"> 
                      <x:Boolean x:Key="IsExpanded">True</x:Boolean> 
                    </scg3:Dictionary> 
                  </sap:WorkflowViewStateService.ViewState> 
                  <Sequence sap:VirtualizedContainerService.HintSize="222,270"> 
                    <sap:WorkflowViewStateService.ViewState> 
                      <scg3:Dictionary x:TypeArguments="x:String, x:Object"> 
                        <x:Boolean x:Key="IsExpanded">True</x:Boolean> 
                      </scg3:Dictionary> 
                    </sap:WorkflowViewStateService.ViewState> 
                    <local:InsertAccountActivity ExecuteNonQueryResult="{x:Null}" AccountId="2" sap:VirtualizedContainerService.HintSize="200,22" Name="Delinin" Surname="Biri"> 
                      <sap:WorkflowViewStateService.ViewState> 
                        <scg3:Dictionary x:TypeArguments="x:String, x:Object"> 
                          <x:Boolean x:Key="IsExpanded">True</x:Boolean> 
                        </scg3:Dictionary> 
                      </sap:WorkflowViewStateService.ViewState> 
                    </local:InsertAccountActivity> 
                    <local1:DoWork AccountId="99" DoWorkResult="[ServiceCallResult]" EndpointConfigurationName="WSHttpBinding_IAccountService" sap:VirtualizedContainerService.HintSize="200,22" Name="Maykıl" mva:VisualBasic.Settings="Assembly references and imported namespaces serialized as XML namespaces" Surname="Cordın" /> 
                    <local:InsertBranchActivity ExecuteNonQueryResult="{x:Null}" BranchId="34" Code="4" sap:VirtualizedContainerService.HintSize="200,22" Title="germany"> 
                      <sap:WorkflowViewStateService.ViewState> 
                        <scg3:Dictionary x:TypeArguments="x:String, x:Object"> 
                          <x:Boolean x:Key="IsExpanded">True</x:Boolean> 
                        </scg3:Dictionary> 
                      </sap:WorkflowViewStateService.ViewState> 
                    </local:InsertBranchActivity> 
                  </Sequence> 
                </TransactionScope> 
              </TryCatch.Try> 
              <TryCatch.Catches> 
                <Catch x:TypeArguments="s:Exception" sap:VirtualizedContainerService.HintSize="404,20"> 
                  <sap:WorkflowViewStateService.ViewState> 
                    <scg3:Dictionary x:TypeArguments="x:String, x:Object"> 
                      <x:Boolean x:Key="IsExpanded">False</x:Boolean> 
                      <x:Boolean x:Key="IsPinned">False</x:Boolean> 
                    </scg3:Dictionary> 
                  </sap:WorkflowViewStateService.ViewState> 
                  <ActivityAction x:TypeArguments="s:Exception"> 
                    <ActivityAction.Argument> 
                      <DelegateInArgument x:TypeArguments="s:Exception" Name="exception" /> 
                    </ActivityAction.Argument> 
                    <WriteLine sap:VirtualizedContainerService.HintSize="211,59" Text="[exception.Message]" /> 
                  </ActivityAction> 
                </Catch> 
              </TryCatch.Catches> 
            </TryCatch> 
            <FlowStep.Next> 
              <FlowStep x:Name="__ReferenceID1"> 
                <sap:WorkflowViewStateService.ViewState> 
                  <scg3:Dictionary x:TypeArguments="x:String, x:Object"> 
                    <av:Point x:Key="ShapeLocation">184.5,340.5</av:Point> 
                    <av:Size x:Key="ShapeSize">211,59</av:Size> 
                    <av:PointCollection x:Key="ConnectorLocation">310,210.5 310,309.5</av:PointCollection> 
                  </scg3:Dictionary> 
                </sap:WorkflowViewStateService.ViewState> 
                <WriteLine sap:VirtualizedContainerService.HintSize="211,59" Text="İşlemler tamamlandı"> 
                  <sap:WorkflowViewStateService.ViewState> 
                    <scg3:Dictionary x:TypeArguments="x:String, x:Object"> 
                      <x:Boolean x:Key="IsExpanded">True</x:Boolean> 
                    </scg3:Dictionary> 
                  </sap:WorkflowViewStateService.ViewState> 
                </WriteLine> 
              </FlowStep> 
            </FlowStep.Next> 
          </FlowStep> 
        </FlowStep.Next> 
      </FlowStep> 
    </Flowchart.StartNode> 
    <x:Reference>__ReferenceID0</x:Reference> 
    <x:Reference>__ReferenceID1</x:Reference> 
    <x:Reference>__ReferenceID2</x:Reference> 
  </Flowchart> 
</Activity>
```

Workflow Application tarafındaki en önemli ayarlardan birisi de config dosyasında yer almaktadır.

```xml
<?xml version="1.0"?> 
<configuration> 
    <startup>          
       <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.0"/>                
    </startup> 
    <connectionStrings> 
        <add name="ConStr" connectionString="User Id=bir kullanıcı adı;Password=bir şifre;Data Source=bir veri kaynağı" providerName="Oracle.DataAccess.Client"/> 
    </connectionStrings> 
    <system.serviceModel> 
        <bindings> 
            <wsHttpBinding> 
                <binding name="WSHttpBinding_IAccountService" transactionFlow="true" /> 
            </wsHttpBinding> 
        </bindings> 
        <client> 
            <endpoint address="http://localhost:35662/AccountService.svc" 
                binding="wsHttpBinding" bindingConfiguration="WSHttpBinding_IAccountService" 
                contract="IAccountService" name="WSHttpBinding_IAccountService"> 
                <identity> 
                    <userPrincipalName value="XXXXXXXXX" /> 
                </identity> 
            </endpoint> 
        </client> 
    </system.serviceModel> 
</configuration>
```

İstemci tarafındaki Binding ayarlarında da transactionFlow niteliğinin mutlak suretle true olması gerekmektedir. Gelelim Main metodu içerisindeki kodlarımıza.

```csharp
using System.Activities; 
using System.Threading;

namespace HowToTransaction 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            var activity = new Workflow1();

            AutoResetEvent arEvent = new AutoResetEvent(false); 
            WorkflowApplication wfApp = new WorkflowApplication(activity); 
            wfApp.Extensions.Add(new CustomTracker());

            wfApp.Completed = (o) => 
            { 
                arEvent.Set(); 
           };

            wfApp.Run(); 
           arEvent.WaitOne(); 
        } 
    } 
}
```

WorkflowApplication tipinden yararlanılarak Workflow1 örneğinin başlatılması işlemi gerçekleştirilmektedir. Tracking işlemi için yazılmış olan CustomTracker sınıfının da, bir Extension olarak WorkflowApplication örneğine bildirilmesi gerekir.

Örneği bu hali ile çalıştırdığımda aşağıdaki sonuçları elde ettiğimi gördüm.

[![wfts_4](/assets/images/2013/wfts_4_thumb.png)](/assets/images/2013/wfts_4.png)

[![wfts_5](/assets/images/2013/wfts_5_thumb.png)](/assets/images/2013/wfts_5.png)

[![wfts_6](/assets/images/2013/wfts_6_thumb.png)](/assets/images/2013/wfts_6.png)

[![wfts_7](/assets/images/2013/wfts_7_thumb.png)](/assets/images/2013/wfts_7.png)

Ekran çıktısını okumak biraz zahmetli olabilir (Özellikle WCF servis çağrısının yapıldığı aktivite mesajlaşma içeriğini de bastığından…) ama işlemlerin başarılı bir şekilde yapıldığı görülmektedir ve doğal olarak veritabanı tarafındaki insert işlemleri de başarılı olmuştur. Özellikle activity tipleri için yapılan State bildirimlerine dikkatinizi çekerim.

[![wfts_8](/assets/images/2013/wfts_8_thumb.png)](/assets/images/2013/wfts_8.png)

Hatta Debug işlemi yapıldığında servis tarafındaki operasyon içerisinde, istemci tarafından gelen Transaction bilgileri de açık bir şekilde görülebilmektedir.

[![wfts_9](/assets/images/2013/wfts_9_thumb.png)](/assets/images/2013/wfts_9.png)

Dikkat edileceği üzere DistributedIdentifier özelliğinin GUID tipinden bir değeri mevcuttur. Bir başka deyişle DTC devreye girmiş ve şu andaki servis operasyonu içerisinde yapılacak işlemler, Workflow Application tarafında açılan TransactionScope’ a dahil edilmiştir.

Eğer InsertBranch Code Activity bileşeni içerisindeki Exception fırlatılan satır etkinleştirilirse, Transaction işlemlerinin Commit edilmediği görülecektir. Bu kısım oldukça önemlidir. Çünkü Workflow1 akışında önce akış içi bir Transaction işlemi, sonrasında servis tarafında bir Transaction işlemi ve son olarak da yine akış içerisinde bir Transaction işlemi söz konusudur.

Son işlemde oluşan Exception nedeni ile bir adım önceki servis Transaction işleminin iptal edilmesi ve edildiğinin görülmesi (tabi o ana kadarki tüm Insert’ lerin de iptal edildiğinin görülmesi) son derece önemlidir. Bu, TransactionScope’ un başarılı çalıştığının bir ispatı olarak düşünülebilir.

Görüldüğü üzere bir Workflow içerisinden başlatılan Transaction’ ın, bir WCF servis operasyonuna aktarılabilmesi ve söz konusu operasyonun ilgili Transaction Scope’ a dahil hale gelerek Two Phase Commit metodolojisine uygun biçimde sisteme dahil edilmesi mümkündür. Bu, tipik anlamda bir Atomic Transaction senaryosudur. Sadece bir kaç küçük detaya ve ayarlamaya dikkat etmek gerekmektedir. Böylece geldik bir yazımızın daha sonuna. Tekrarda görüşünceye dek hepinize mutlu günler dilerim

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_109.png)

[HowToTransaction.zip (457,18 kb)](https://www.buraksenyurt.com/pics/2012%2f8%2fHowToTransaction.zip)

[Örnek Visual Studio 2010,.Net Framework 4.0 tabanlıdır]