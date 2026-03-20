---
layout: post
title: "WCFden, XML Web Servisine TransactionScope Activity Bileşeni Üzerinden Transaction Aktarmak"
date: 2013-03-25 10:21:00 +0300
categories:
  - wcf
tags:
  - wcf
  - csharp
  - xml
  - dotnet
  - aspnet
  - linq
  - oracle
  - workflow-foundation
  - xaml
  - web-service
  - xml-web-services
  - http
  - java
  - transactions
  - generics
  - debugging
  - visual-studio
  - atomic-operations
---
Bir süre öncesine kadar Composition adı verilen bir katmanda yer alacak çeşitli servisler ile yoğun şekilde güreşmekteydim. Çok fazla faktör, çok fazla farklı sistem ve tabiri yerinde ise oyun ve oyuncu söz konusuydu. WCF servisleri, XML Web Servisleri, Java tabanlı olanları ve belki de yarın gelecek olan çeşitli COM bileşenleri, 3ncü parti uygulamalar, koduna müdahale edemeyeceğimiz programlar vs.

[![1344349583_normalThumb](/assets/images/2013/1344349583_normalThumb_thumb.jpg)](/assets/images/2013/1344349583_normalThumb.jpg)

Sadece bunlar olsa iyi. Bir de bunlar içerisine Oracle üzerinde koşan Transactional veritabanı işlemleri de mevcut olunca, işler ister istemez karışıyor ve künde pozisyonuna geliyorsunuz. Nitekim bu servisler n sayıda kombinasyon ile birbirleriyle etkileşimde bulunabilirler ve bu tip senaryolarda bir şekilde Distributed Transaction terminolojisinin uygulanması ve servisler arasında başarılı bir şekilde akıtılarak, Two Phase Commit ilkesinin gerçekleştirilebiliyor olması gereklidir.

> Daha önceden ele aldığımız bir yazıda, WCF servislerinin Workflow tarafındaki TransactionScope kontrolüne dahil edilme durumlarını incelemeye çalışmıştık hatırlayalım.
> [Workflow Foundation, Oracle, WCF ve TransactionScope](/2013/01/31/workflow-foundation-oracle-wcf-ve-transactionscope/)

Senaryo

Bu seferki senaryomuz biraz daha farklı ve karışık

![Confused smile](/assets/images/2013/wlEmoticon-confusedsmile_22.png)

Elimizde iki adet servis bulunmakta. Her ikisi de kendi içerisindeki operasyonlarında, Oracle tabanlı bir veritabanı üzerine insert işlemi gerçekleştirmekte (Senaryo gereği insert, ama tabiki her tür CRUD operasyonu söz konusu olabilir)

Ne varki bu servislerden birisi Windows Communication Foundation yapısında iken diğeri eski XML Web Service formatında tasarlanmış durumdalar. Senaryomuzda bu iki servisi kendi bünyesinde birleştiren bir de Workflow Activity bulunuyor. Çok doğal olarak bu bileşenin içerisinde ele alacağımız bir TransactionScope kontrolü de yer almakta. Hal böyle olunca TransactionScope içerisinde üretilen Transaction’ ın servisler arasında nasıl akacağı, büyük soru işareti olarak karşımıza çıkıyor. Aslında senaryoyu aşağıdaki şekle bakarak kafamızda daha iyi canlandırabileceğimizi düşünüyorum.

[![WP_000637](/assets/images/2013/WP_000637_thumb.jpg)](/assets/images/2013/WP_000637.jpg)

WCF servislerinin kullanıldığı senaryolarda atomic transaction’ ların servis içerisine nasıl akıtılacağını [bu yazımızda](/2013/01/31/workflow-foundation-oracle-wcf-ve-transactionscope/) incelemiştik hatırlayacağınız üzere. Ancak işin içerisine eski stilde yazılmış bir XML Web Service girince, durum biraz farklılaşıyor

![Who me?](/assets/images/2013/wlEmoticon-whome_7.png)

Ne yazık ki, istemci tarafında başlatılan Transaction’ ın kod yardımıyla XML Web Service içerisindeki ilgili Web Method’ a aktarılması gerekmektedir. Aksi durumda bir çalışma zamanı hatası alınmıyor olmasına karşın, bir dağıtık Transaction’ ın ilgili Web Method içerisindeki CRUD (Create Retrieve Update Delete) işlemini ele alamadığı görülür. Bu görünmez hata fark edilmediği takdirde, kötü sonuçlara neden olabilir tahmin edeceğiniz üzere.

Örnek

Öyleyse gelin yola koyulalım ve adım adım Solution içeriğimizi inşa ederek teste çıkalım. Çözümümüz içerisinde bir ortak fonksiyonellik kütüphanesi, bir WCF Servis uygulaması, bir XML Web Servis içeren Asp.Net Web uygulaması ve son olarak da bir adet Workflow Console projesi bulunmakta.

[![htt_4](/assets/images/2013/htt_4_thumb.png)](/assets/images/2013/htt_4.png)

İlk olarak WCF servis tarafını geliştiriyor olacağız.

Servis sözleşmesi

```csharp
using System.ServiceModel;

namespace WcfService1 
{ 
    [ServiceContract] 
    public interface IService1 
    { 
        [OperationContract] 
        [TransactionFlow( TransactionFlowOption.Mandatory)] 
        int InsertAccount(int accountId, string name, string surname); 
    } 
}
```

Servis kodları

```csharp
using System.Configuration; 
using System.ServiceModel; 
using System.Transactions; 
using CommonLibrary; 
using Oracle.DataAccess.Client;

namespace WcfService1 
{ 
    public class Service1 
        : IService1 
    { 
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)] 
        public int InsertAccount(int accountId,string name,string surname) 
        { 
            int result = 0;

            Utility.Log(Transaction.Current);

            using (OracleConnection conn = new OracleConnection(ConfigurationManager.ConnectionStrings["ConStr"].ConnectionString)) 
            { 
                using (OracleCommand command = new OracleCommand("INSERT INTO ACCOUNT (ACCOUNTID,NAME,SURNAME) VALUES (:pACCOUNTID,:pNAME,:pSURNAME)", conn)) 
                { 
                    command.Parameters.Add(":pACCOUNTID", accountId); 
                    command.Parameters.Add(":pNAME", name); 
                    command.Parameters.Add(":pSURNAME", surname);

                    conn.Open(); 
                    result = command.ExecuteNonQuery(); 
                } 
            }

            return result; 
        } 
    } 
}
```

Servis Konfigurasyon içeriği

```xml
<?xml version="1.0" encoding="utf-8" ?> 
<configuration> 
    <connectionStrings> 
        <add name="ConStr" connectionString="User Id=...;Password=...;Data Source=..." providerName="Oracle.DataAccess.Client"/> 
    </connectionStrings> 
    <system.serviceModel> 
        <services> 
            <service name="WcfService1.Service1"> 
                <endpoint address="http://localhost:12809/Service1.svc" 
                  binding="wsHttpBinding" bindingConfiguration="wsB" contract="WcfService1.IService1" /> 
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
</configuration>
```

Servis metodu Account isimli bir tabloya veri girişi işlemini icra edecek şekilde tasarlanmıştır. wsHttpBinding bağlayıcı tipini kullanmaktadır. Oracle üzerinde bir veri işlemi söz konusu olması nedeniyle ODP.Net’ in ilgili versiyonu ele alınmıştır.

> Bu servis uygulamasında ve takip eden kod satırlarında belirteceğimiz XML Web Service uygulamasında, güncel Transaction bilgilerini fiziki bir dosyaya loglamak amacıyla bir kütüphane fonksiyonundan yararlanılmaktadır. Utility sınıfına ait static Log metodunun içeriği aşağıdaki gibidir.
> Log bilgisinin en önemli parçalarından birisi de DistributedIdentifier özelliğinin değeridir. Bu değer ilk başlatıldığı noktada ne ise, diğer servis çağrıları içerisinde de aynı olmalıdır ki ortak bir dağıtık transaction yapısından söz edilebilsin.

```csharp
using System.IO; 
using System.Transactions;

namespace CommonLibrary 
{ 
    public class Utility 
    { 
        public static void Log(Transaction current) 
        { 
            if (current != null) 
            { 
                var information = Transaction.Current.TransactionInformation; 
                string report = string.Format("Time:{0},Isolation Level:{1},Distributed ID:{2},Local ID:{3},Status:{4}\n" 
                    , information.CreationTime 
                    , Transaction.Current.IsolationLevel.ToString() 
                    , information.DistributedIdentifier.ToString() 
                    , information.LocalIdentifier 
                    , information.Status 
                    ); 
                File.AppendAllLines("c:\\Log.txt", new string[] { report }); 
            } 
        } 
    } 
}
```

Gelelim XML Web Service tarafına. Eminim ki uzun süredir bir XML Web Servis yazmıyor veya tüketmiyorsunuzdur

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_111.png)

Ama çalıştığınız kurum da eski sistemler söz konusu ise bundan kaçış yolunuz olmadığını ifade edebilirim.

XML Web Servis kodlar

```csharp
using System.Configuration; 
using System.EnterpriseServices; 
using System.Transactions; 
using System.Web.Services; 
using CommonLibrary; 
using Oracle.DataAccess.Client;

namespace WebApplication1 
{ 
    [WebService(Namespace = "http://tempuri.org/")] 
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)] 
    [System.ComponentModel.ToolboxItem(false)] 
    public class WebService1 : System.Web.Services.WebService 
    {

        [WebMethod(TransactionOption= TransactionOption.Required)] 
        public int InsertBranch(int branchId, string title, int code,byte[] propToken) 
        { 
            int result = 0;

            Transaction tx = TransactionInterop.GetTransactionFromTransmitterPropagationToken(propToken); 
            using (TransactionScope scope = new  TransactionScope(tx)) 
            { 
                Utility.Log(Transaction.Current);

                using (OracleConnection conn = new OracleConnection(ConfigurationManager.ConnectionStrings["ConStr"].ConnectionString)) 
                { 
                    using (OracleCommand command = new OracleCommand("INSERT INTO BRANCH (BRANCHID,TITLE,CODE) VALUES (:pBRANCHD,:pTITLE,:pCODE)", conn)) 
                    { 
                        command.Parameters.Add(":pBRANCHID", branchId); 
                        command.Parameters.Add(":pTITLE", title); 
                        command.Parameters.Add(":pCODE", code);

                        conn.Open(); 
                        result = command.ExecuteNonQuery(); 
                        //throw new Exception("Some error"); 
                    } 
                } 
                scope.Complete(); 
            }

            return result; 
        }         
    } 
}
```

Branch tablosuna insert işlemini icra eden InsertBranch web metodu içerisindeki en önemli kısım, parametre olarak gelen byte[] tipindeki propToken değişkeninin kullanılış şeklidir. Dikkat edileceği üzere bu değişken değerinden yararlanılarak istemci tarafından gelen dağıtık transaction tipi yakalanmakta ve onu baz alacak şekilde bir TransactionScope bloğu üretilmektedir. Bu sayede istemci tarafındaki Transaction Scope’ un başlattığı dağıtık transaction’ a dahil olunabilecektir.

> Pek tabi kodumuz içerisinde daha sonradan yapacağımız testler için ele alacağımız bir yorum satırı vardır
>
> ![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_111.png)
>
> Amaç olarak ilgili satırı test sırasında açıp bu web servis ve öncesinden gelen WCF servisi içerisindeki veritabanı operasyonlarının rollback edildiğini görmek istiyoruz.

İstemci Tarafı

Öyleyse gelelim istemci tarafına. İstemci uygulamamız bildiğiniz üzere bir Workflow çağrısı gerçekleştiriyor olacak. Bu sebepten bir Workflow Console Application projesinden yararlanabiliriz. Söz konusu projeye hem WCF hem de XML Web servislerini referans olarak eklememiz gerekiyor.

> XML Web Service referansını eklemek için Add Service Reference->Advanced sekmesine geçip oradaki Add Web Reference düğmesini kullanmanız gerektiğini unutmayın. Yoksa Web Servisinizi de WCF servis gibi eklenmiş bulursunuz.

Workflow uygulamamızda aşağıdaki TransactionScope bileşenini içeren bir Flow Chart söz konusudur.

[![htt_1](/assets/images/2013/htt_1_thumb.png)](/assets/images/2013/htt_1.png)

Akışa ait XAML içeriğinde özellikle bold olan alanara dikkat edin. Örneği yazarken işinize yarayacaktır.

```xml
<Activity mc:Ignorable="sads sap" x:Class="WorkflowConsoleApplication1.Activity1" sap:VirtualizedContainerService.HintSize="654,676" mva:VisualBasic.Settings="Assembly references and imported namespaces for internal implementation" 
xmlns="http://schemas.microsoft.com/netfx/2009/xaml/activities" 
xmlns:av="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
xmlns:local="clr-namespace:WorkflowConsoleApplication1" 
xmlns:local1="clr-namespace:WorkflowConsoleApplication1.WcfServiceReference1.Activities" 
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
xmlns:sc="clr-namespace:System.ComponentModel;assembly=System" 
xmlns:scg="clr-namespace:System.Collections.Generic;assembly=System" 
xmlns:scg1="clr-namespace:System.Collections.Generic;assembly=System.ServiceModel" 
xmlns:scg2="clr-namespace:System.Collections.Generic;assembly=System.Core" 
xmlns:scg3="clr-namespace:System.Collections.Generic;assembly=mscorlib" 
xmlns:sd="clr-namespace:System.Data;assembly=System.Data" 
xmlns:sl="clr-namespace:System.Linq;assembly=System.Core" 
xmlns:st="clr-namespace:System.Text;assembly=mscorlib" 
xmlns:ww="clr-namespace:WorkflowConsoleApplication1.WcfServiceReference1" 
xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"> 
  <Flowchart sad:XamlDebuggerXmlReader.FileName="d:\users\bsenyurt\documents\visual studio 2012\Projects\HowTo_AtomicTransactions\WorkflowConsoleApplication1\Activity1.xaml" sap:VirtualizedContainerService.HintSize="614,636"> 
    <Flowchart.Variables> 
      <Variable x:TypeArguments="x:Int32" Name="InsertAccountExceuteResult" /> 
      <Variable x:TypeArguments="x:Int32" Name="InsertBranchExecuteResult" /> 
    </Flowchart.Variables> 
    <sap:WorkflowViewStateService.ViewState> 
      <scg3:Dictionary x:TypeArguments="x:String, x:Object"> 
        <x:Boolean x:Key="IsExpanded">False</x:Boolean> 
        <av:Point x:Key="ShapeLocation">270,2.5</av:Point> 
        <av:Size x:Key="ShapeSize">60,75</av:Size> 
        <av:PointCollection x:Key="ConnectorLocation">300,77.5 300,160.5</av:PointCollection> 
      </scg3:Dictionary> 
    </sap:WorkflowViewStateService.ViewState> 
    <Flowchart.StartNode> 
      <FlowStep x:Name="__ReferenceID0"> 
        <sap:WorkflowViewStateService.ViewState> 
          <scg3:Dictionary x:TypeArguments="x:String, x:Object"> 
            <av:Point x:Key="ShapeLocation">195,160.5</av:Point> 
            <av:Size x:Key="ShapeSize">210,59</av:Size> 
          </scg3:Dictionary> 
        </sap:WorkflowViewStateService.ViewState> 
        <TransactionScope sap:VirtualizedContainerService.HintSize="266,396"> 
          <sap:WorkflowViewStateService.ViewState> 
            <scg3:Dictionary x:TypeArguments="x:String, x:Object"> 
              <x:Boolean x:Key="IsExpanded">True</x:Boolean> 
            </scg3:Dictionary> 
          </sap:WorkflowViewStateService.ViewState> 
          <Sequence sap:VirtualizedContainerService.HintSize="230,315"> 
            <sap:WorkflowViewStateService.ViewState> 
              <scg3:Dictionary x:TypeArguments="x:String, x:Object"> 
                <x:Boolean x:Key="IsExpanded">True</x:Boolean> 
              </scg3:Dictionary> 
            </sap:WorkflowViewStateService.ViewState> 
            <local1:InsertAccount sap:VirtualizedContainerService.HintSize="208,22" InsertAccountResult="[InsertAccountExceuteResult]" mva:VisualBasic.Settings="Assembly references and imported namespaces serialized as XML namespaces" accountId="9000" name="Burak" surname="Senyurt" /> 
            <InvokeMethod sap:VirtualizedContainerService.HintSize="208,129" MethodName="InsertBranchFromWebService" TargetType="local:Excecuter"> 
              <InvokeMethod.Result> 
                <OutArgument x:TypeArguments="x:Int32">[InsertBranchExecuteResult]</OutArgument> 
              </InvokeMethod.Result> 
              <InArgument x:TypeArguments="x:Int32">8888</InArgument> 
              <InArgument x:TypeArguments="x:String">Çınaraltı</InArgument> 
              <InArgument x:TypeArguments="x:Int32">40</InArgument> 
            </InvokeMethod> 
          </Sequence> 
        </TransactionScope> 
      </FlowStep> 
    </Flowchart.StartNode> 
    <x:Reference>__ReferenceID0</x:Reference> 
  </Flowchart> 
</Activity>
```

Ne yazık ki XML Web Service’ ler referans olarak bir Workflow projesine eklendiklerinde, Toolbox üzerinde aynı WCF Servislerinde olduğu gibi bir component olarak görülmemektedir. Tabi bunu Visual Studio 2012 için konuştuğumuzu tekrardan hatırlatalım. (Eskiye olan support’ un kalktığını bu noktada bariz bir şekilde görebiliriz aslında ![Sad smile](/assets/images/2013/wlEmoticon-sadsmile_14.png))

[![htt_5](/assets/images/2013/htt_5_thumb.png)](/assets/images/2013/htt_5.png)

Dolayısıyla ilgili XML Web Servis’ ini çağırmak için belki bir InvokeMethod bileşeninden yararlanabiliriz ki o da içeride static bir tip metodu kullanacaktır. Bu metod Executer isimli sınıf içerisinde aşağıdaki gibi tanımlanmıştır.

```csharp
using System.Transactions; 
using WorkflowConsoleApplication1.localhost;

namespace WorkflowConsoleApplication1 
{ 
    public class Excecuter 
    { 
        public static int InsertBranchFromWebService(int branchId,string title,int code) 
        { 
            WebService1 service = new WebService1(); 
            byte[] propToken = TransactionInterop.GetTransmitterPropagationToken(Transaction.Current); 
            return service.InsertBranch(branchId, title, code, propToken); 
        } 
    } 
}
```

Metoda ait kod içeriği oldukça kritiktir. Görüldüğü üzere TransactionInterop tipinin static GetTransmitterPropagationToken metoduna o anki Transaction nesne örneği gönderilerek bir byte dizi içeriğinin elde edilmesi söz konusudur. Bu şekilde, aslında TransactionScope bileşeni içerisine dahil olan ve WCF Servis çağrısı yoluyla başlatılan Transaction’ ın XML Web Servis metodu içerisine parametre olarak bildirilebilmesi mümkün hale gelmektedir.

Workflow Console uygulamasının Main metoduna ait içerik ise son derece standarttır.

```csharp
using System; 
using System.Activities;

namespace WorkflowConsoleApplication1 
{

    class Program 
    { 
        static void Main(string[] args) 
        { 
            try 
            { 
                Activity wf = new Activity1(); 
                WorkflowInvoker.Invoke(wf); 
            } 
            catch (Exception excp) 
            { 
                Console.WriteLine(excp.Message); 
            } 
        } 
    } 
}
```

## Testler

Artık testlerimize başlayabiliriz. Senaryoyu doğrudan bu şekilde işlettiğimizde, C dizini altında üretilen Log.txt dosyasında aşağıdakine benzer bir içeriğin oluşturulacağı gözlemlenecektir.

```text
Time:09.08.2012 09:28:45,Isolation Level:Serializable,Distributed ID:2cef4851-a7fe-41e3-a42a-cf4aab1aa9fe,Local ID:632e8dc4-5446-4531-9426-05f092527d54:1,Status:Active

Time:09.08.2012 09:28:45,Isolation Level:Serializable,Distributed ID:2cef4851-a7fe-41e3-a42a-cf4aab1aa9fe,Local ID:16135af3-ce46-42c3-9acf-ec5dc5d43275:1,Status:Active
```

Bu çıktılardan ilki WCF servis metodu, ikincisi ise XML Web Servis metodu içerisinde gelmektedir. Dikkat edileceği üzere her iki çağrı içinde aynı Distributed Transaction ID değeri üretilmiştir. Eğer veritabanına gidilirse, icra edilen insert işlemlerinin her iki tablo içinde başarılı bir şekilde yapıldığı görülebilir.

[![htt_2](/assets/images/2013/htt_2_thumb.png)](/assets/images/2013/htt_2.png)

Şimdi XML Web servis metodu içerisindeki sihirli yorum satırımızı açalım ve testimizi tekrardan yapalım. Çalışma ortamına düşen exception mesajı aşağıdaki gibi olacaktır. TransactionScope kontrolünün AbortInstanceOnTransactionFlow özelliğinin değeri varsayılan olarak true olduğundan, Web Servis içerisinden gelen Fault Exception nedeni ile, akışa ait nesne örneğinin çalışması otomatikman durdurulmuştur.

[![htt_3](/assets/images/2013/htt_3_thumb.png)](/assets/images/2013/htt_3.png)

Log içeriğine bakıldığında ise Distribute Transaction’ ın yine başarılı bir şekilde oluşturulduğu ve her iki servis çağrısında da, aynı ID değerlerini kullanıldığı görülebilir.

```text
Time:09.08.2012 09:32:35,Isolation Level:Serializable,Distributed ID:6dd28d89-aa10-4a97-8b32-e3439f0374ff,Local ID:632e8dc4-5446-4531-9426-05f092527d54:2,Status:Active

Time:09.08.2012 09:32:35,Isolation Level:Serializable,Distributed ID:6dd28d89-aa10-4a97-8b32-e3439f0374ff,Local ID:384682c2-e4e1-47af-bc8e-f28c5abd7229:1,Status:Active
```

Ancak veritabanına gidilip ilgili tablolar sorgulandığında, iki Insert işleminin de yapılmadığı gözlemlenecektir. Bir başka deyişle istediğimiz durum oluşmuş ve Transaction işlemleri Abort edilerek o ana kadar yapılan ne kadar veritabanı işlemi var ise onaylanmamıştır

![Open-mouthed smile](/assets/images/2013/wlEmoticon-openmouthedsmile_30.png)

Görüldüğü üzere biraz kodlama yardımıyla WCF ve XML Web Servislerini, TransactionScope bileşeni içerisinde bir arada kullanabildik. Tabiki senaryonun genişletilmesi ve geliştirilmesi gerekiyor. Örneğin Savepoint kullanımları durumu var. Ya da Long Running Process söz konusu ise Persistence sisteminin bu tip vakalarda nasıl tepki vereceği. Hatta daha da zor bir senaryo var. Ya bu XML Web Service’ ler daha önceden yazılmışlar ve sizin müdahale alanınız dışındaysalar

![Confused smile](/assets/images/2013/wlEmoticon-confusedsmile_22.png)

Şimdilik bu kötü kokan vakaları bir kenara bırakıp önümüze bakalım derim. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[HowTo_AtomicTransactions.zip (117,25 kb)](/assets/files/2013/HowTo_AtomicTransactions.zip)

[Örnek Visual Studio 2010,.Net Framework 4.0 tabanlıdır]