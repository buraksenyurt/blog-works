---
layout: post
title: "Workflow Services 4.0 - Transaction Flow [Beta 2]"
date: 2010-01-20 01:00:00 +0300
categories:
  - wf-4-0-beta-2
tags:
  - wf-4-0-beta-2
  - xml
  - csharp
  - dotnet
  - aspnet
  - ado-net
  - wcf
  - workflow-foundation
  - wpf
  - xaml
  - transactions
  - visual-studio
  - rc
---
Geçen gece ilginç bir rüya gördüm. Bir su birikintisine damlacıklar düşüyordu. Önceleri yavaş yavaş ve uzun aralıklarla düşen damlalar söz konusuydu. Zaman ilerledikçe her bir damlanın suya değdiği noktada bir isim bıraktığını görmeye başladım. int i, for, if derken damlalar hızlanmaya başladı. Daha sık daha çok damla düşüyordu. Bazıları kocaman boyutlardaydı ve düştükleri su birikintisinde neredeyse fırtına koparıyorlardı..Net, C#, parallel, WPF, Ajax, ASP.Net, WCF, WF derken damlaların artık nerelerden geldiğini takip edemez olmaya başladım. Ama damlalar iz bırakmaya devam ediyordu. 1.1, 2.0, 3.5, 4.0, Beta, RC, RTM...derken terler içerisinde uyanmıştım

![blg123_Giris.jpg](/assets/images/2010/blg123_Giris.jpg)

![Wink](/assets/images/2010/smiley-wink.gif)

Bir kaç ay içerisinde eğer büyük bir aksilik olmassa.Net Framework 4.0 ve Visual Studio 2010 ürünlerinin son sürümleri yayınlanmış olacak. Şu anda gelişmeleri Beta 2 sürümü üzerinden takip etmekteyiz. Ancak yakında RC ve sonrasında RTM sürümlerininde çıkacağını ve önemli iyileştirmeler olacağını biliyoruz. Yinede gelebilecek yenilikleri takip etmek adına araştırmalarıma devam etmekteyim. Bir süredir Workflow Foundation 4.0 üzerine araştırma yapmıyordum. Geçtiğimiz günlerde Transaction yönetimi ile ilişkili olarak önemli bir açığın kapatıldığını öğrendim. Buna göre Workflow Foundation 4.0 öncesinde, Workflow Service'lerde istemci tarafından başlatılan Transaction'ların, servis tarafına akması mümkün olmuyordu. Şimdi bir dakika...Transaction, Flow, İstemciden Sunucuya...Ihmmmm

![Sealed](/assets/images/2010/smiley-sealed.gif)

Biraz kafamız karışmış olabilir. Başlamadan önce bu konu hakkında biraz bilgi vermeye çalışalım dilerseniz.

Bildiğiniz üzere bir Transaction içerisinde baştan sona başarılı bir şekilde tamamlanması beklenen işlemler bütünü yer alır. Transaction başlatıldıktan sonra içerisinde ceyran eden işlemlerin kalıcı olarak kabul görmesi, ancak tüm adımlardaki işlemlerin başarılı olmasına bağlıdır. Herhangibir adımda bir hata oluştuğunda Transaction'a dahil olan herkesin, Transaction başlamadan önceki durumlarına (State) dönebilmesi gerekir. Üstelik bu geri dönüşlerde (Rollback) verilerin tutarlılığını korumak önemlidir. Hatta Transaction başarılı bir şekilde sonuçlandırıldığında, çıkan verilerin de anlamlı olması beklenir. Kısaca size ACID (Atomicity,Consistensy,Isolation,Durability) kavramını aktarmaya çalıştım. Tabi zaman ilerledikçe bir Transaction'ın sadece tek bir program alanı içerisinde değil, birden fazla program alanı içerisinde ele alınmasının gerektiği durumlar oluşmuştur. Bu noktada Dağıtık Transaction (Distributed Transaction) kavramına geçilmektedir. Buna göre bir program alanı içerisinde başlatılan Transaction, başka bir program alanı içerisinde de ele alınabilir. Elbette bu tip bir senaryoda, program alanlarının farklı makineler üzerinde konuşlandırılmış olması da kuvvetle muhtemeldir. İşte bu gibi durumlarda Transaction'ın koordinasyonu için genellikle 3ncü parti araçların devreye girdiği görülmektedir (Distributed Transaction Coordinator vb...)

Gel gelelim zaman içerisinde söz konusu Transaction'ların servisler üzerinden akması ihtiyacı doğmuştur. Buna göre bir servis tarafından başlatılan bir Transaction'a, diğer bir serviste başlatılan operasyonun da dahil olması gerekebilir. Bu durumda ilgili servis operasyonlarının tamamının aynı Transaction Scope içerisinde ele alınması gerekmektedir. Transaction Scope denilince aklıma gelen ilk şey ise Ado.Net 2.0 ile birlikte gelen TransactionScope tipidir. Bu tip sayesinde, blok içerisine dahil olan farklı bağlantılar (Connection) için aynı Transaction Scope'un oluşturulması ve yönetilmesi son derece kolaylaşmıştır.

Şimdi gelelim bu güne. Artık elimizin altında bir Workflow'un servis bazlı olarak sunulabilmesi imkanı bulunmakta. Uzun süredir. Buna göre istemcilerin, söz konusu Workflow'ları servis bazlı olaraktan talep edebilmesi mümkün. Hal böyle olunca istemci tarafından başlatılacak bir Transaction'ın, çağrıda bulunulan Workflow Service tarafından'da ele alınabiliyor olması istenen bir özelliktir. Dolayısıyla istemcide açılan Transaction'ın Workflow Service tarafına akabiliyor (Flow) olması gerekmektedir.

Workflow Foundation 4.0 Beta 2 sürümünde söz konusu işlevselliği sağlamak için Messaging kontrollerinde yer alan TransactedReceiveScope isimli aktivite bileşeninden yararlanılmaktadır. Bu bileşen içerisinde istemcilerin çağrıda bulunacağı operasyon bildirimi yer alır. Bunun içinde Receive aktivite bileşeninden yararlanılmaktadır. Dilerseniz olayı kavramak için basit bir örnek geliştirelim. Örneğimizde aşağıdaki XAML içeriğine sahip bir Workflow Service oluşturduğumuzu göz önüne alalım.

![blg123_Flow.gif](/assets/images/2010/blg123_Flow.gif)

Xaml içeriğimiz;(Sadece Sequence içeriği belirtilmiştir)

```xml
<p:Sequence DisplayName="Sequential Service" sad:XamlDebuggerXmlReader.FileName="G:\Projects\Workflow Foundation\Transactions\TransactionFlow\MathFlowService.xamlx" sap:VirtualizedContainerService.HintSize="325,797">
    <p:Sequence.Variables>
      <p:Variable x:TypeArguments="CorrelationHandle" Name="handle" />
      <p:Variable x:TypeArguments="x:Int32" Name="XValue" />
      <p:Variable x:TypeArguments="x:Int32" Name="YValue" />
      <p:Variable x:TypeArguments="x:Int32" Name="SumResult" />
    </p:Sequence.Variables>
    <sap:WorkflowViewStateService.ViewState>
      <scg3:Dictionary x:TypeArguments="x:String, x:Object">
        <x:Boolean x:Key="IsExpanded">True</x:Boolean>
      </scg3:Dictionary>
    </sap:WorkflowViewStateService.ViewState>
    <TransactedReceiveScope Request="{x:Reference __ReferenceID0}" sap:VirtualizedContainerService.HintSize="303,673">
      <p:Sequence sap:VirtualizedContainerService.HintSize="277,474">
        <sap:WorkflowViewStateService.ViewState>
          <scg3:Dictionary x:TypeArguments="x:String, x:Object">
            <x:Boolean x:Key="IsExpanded">True</x:Boolean>
          </scg3:Dictionary>
        </sap:WorkflowViewStateService.ViewState>
        <p:InvokeMethod sap:VirtualizedContainerService.HintSize="255,127" MethodName="WriteTransactionInfo" TargetType="t:Helper" />
        <p:Assign sap:VirtualizedContainerService.HintSize="255,57">
          <p:Assign.To>
            <p:OutArgument x:TypeArguments="x:Int32">[SumResult]</p:OutArgument>
          </p:Assign.To>
          <p:Assign.Value>
            <p:InArgument x:TypeArguments="x:Int32">[XValue + YValue]</p:InArgument>
          </p:Assign.Value>
        </p:Assign>
        <SendReply DisplayName="SendResponse" sap:VirtualizedContainerService.HintSize="255,86">
          <SendReply.Request>
            <Receive x:Name="__ReferenceID0" CanCreateInstance="True" DisplayName="ReceiveRequest" sap:VirtualizedContainerService.HintSize="277,86" OperationName="Sum" ServiceContractName="p1:IMathFlowService">
              <Receive.CorrelatesOn>
                <MessageQuerySet />
              </Receive.CorrelatesOn>
              <Receive.CorrelationInitializers>
                <RequestReplyCorrelationInitializer CorrelationHandle="[handle]" />
              </Receive.CorrelationInitializers>
              <ReceiveParametersContent>
                <p:OutArgument x:TypeArguments="x:Int32" x:Key="x">[XValue]</p:OutArgument>
                <p:OutArgument x:TypeArguments="x:Int32" x:Key="y">[YValue]</p:OutArgument>
              </ReceiveParametersContent>
            </Receive>
          </SendReply.Request>
          <SendMessageContent DeclaredMessageType="x:Int32">
            <p:InArgument x:TypeArguments="x:Int32">[SumResult]</p:InArgument>
          </SendMessageContent>
        </SendReply>
      </p:Sequence>
    </TransactedReceiveScope>
  </p:Sequence>
```

Akışımız içerisinde birde yardımcı sınıf bulunmaktadır. Helper isimli sınıf içerisinde InvokeMethod aktivitesi tarafından çalıştırılan ve güncel Transaction ile ilişkili bilgileri dosyaya aktaran basit bir fonksiyonellik yer almaktadır.

```csharp
using System;
using System.Text;
using System.Transactions;
using System.IO;

namespace TransactionFlow
{
    public class Helper
    {
        public static void WriteTransactionInfo()
        {
            StringBuilder builder = new StringBuilder();

            TransactionInformation currentTrx = Transaction.Current.TransactionInformation;
            builder.AppendLine(String.Format("Oluşturulma zamanı {0}", currentTrx.CreationTime.ToString()));
            builder.AppendLine(String.Format("Local Identifier değeri {0}", currentTrx.LocalIdentifier.ToString()));
            builder.AppendLine(String.Format("Distributed Identifier değeri {0}", currentTrx.DistributedIdentifier.ToString()));

            File.WriteAllText("c:\\TransctionInformations.txt", builder.ToString());
        }
    }
}
```

Workflow çok basit olarak istemciden gelen iki sayının toplamını geriye döndürmek üzerine tasarlanmıştır. Ancak dikkat edilmesi gereken nokta kullandığı Transaction bilgileridir. Tabi şu noktada unutulmamalıdır. Hem Workflow Service hemde istemci uygulama System.Transactions.dll assembly'ını referans etmelidir. Helper sınıfı içerisinde yer alan WriteTransactionInfo metodu text tabanlı bir dosya içerisine eğer varsa güncel transaction bilgilerini yazdırmaktadır. Bunlardan ilki transaction oluşturulma zamanıdır (CreationTime). Sonrasında yerel transaction değeri (LocalIdentifier) ve dağıtık transaction değerleri (DistributedIdentifier) yazdırılır. Bu değerler GUID tipindendir.

Workflow Service tarafında istemciden gelecek Transaction akışına izin vermek için sadece TransectedReceiveScope bileşeninin kullanılması yeterli değildir. Konfigurasyon içerisinde de transaction akışına izin verileceğinin bildirilmesi gerekir. Bu amaçla Workflow Service uygulamasındaki web.config içeriği aşağıdaki gibi düzenlenebilir.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <system.web>
    <compilation debug="true" targetFramework="4.0" />
  </system.web>
  <system.serviceModel>
    <bindings>
      <wsHttpBinding>
        <binding transactionFlow="true"/>
      </wsHttpBinding>      
    </bindings>
    <services>
      <service name="MathFlowService">
        <endpoint address="" binding="wsHttpBinding" contract="IMathFlowService"/>                  
      </service>
    </services>
    <behaviors>
      <serviceBehaviors>
        <behavior>          
          <serviceMetadata httpGetEnabled="true"/>
          <serviceDebug includeExceptionDetailInFaults="false"/>
        </behavior>
      </serviceBehaviors>
    </behaviors>
  </system.serviceModel>
  <system.webServer>
    <modules runAllManagedModulesForAllRequests="true"/>
  </system.webServer>
</configuration>
```

Dikkat edileceği üzere wsHttpBinding bağlayıcı tipi (Binding Type) için transactionFlow niteliğine true değeri atanmıştır. Bu zaten Windows Communication Foundation (WCF) tarafından bildiğimiz bir ilkedir. Şimdi gelelim istemci uygulama tarafına. Console projesi şeklinde tasarlanan istemci uygulamaya öncelikli olarak Workflow Service için gerekli proxy içeriği Add Service Reference seçeneği ile eklenmelidir. Tabi buna uygun olarak istemci tarafında üretilen app.config içerisinde de transaction akışı için gerekli bildirimler otomatik olarak üretilecektir. Bunu takiben Main metodunda aşağıdaki örnek kodların geliştirildiğini düşünelim.

```csharp
using System;
using System.Text;
using System.Transactions;
using ClientApp.MathFlowSpace;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                MathFlowServiceClient proxy = new MathFlowServiceClient();

                Console.WriteLine("Çağrı öncesi Transaction bilgileri");
                WriteTransactionInfo();

                int? result = proxy.Sum(new Sum { x = 1, y = 4 });

                Console.WriteLine("Çağrı sonrası Transaction bilgileri");
                WriteTransactionInfo();
            }
        }

        public static void WriteTransactionInfo()
        {
            StringBuilder builder = new StringBuilder();

            TransactionInformation currentTrx = Transaction.Current.TransactionInformation;
            builder.AppendLine(String.Format("Oluşturulma zamanı {0}", currentTrx.CreationTime.ToString()));
            builder.AppendLine(String.Format("Local Identifier değeri {0}", currentTrx.LocalIdentifier.ToString()));
            builder.AppendLine(String.Format("Distributed Identifier değeri {0}", currentTrx.DistributedIdentifier.ToString()));

            Console.WriteLine(builder.ToString());
        }
    }
}
```

İstemci uygulama tarafında en önemli nokta TransactionScope kullanımıdır. Ayrıca, Workflow Service operasyonunun çağırılmasından hemen önce ve sonra ortamdaki güncel Transaction bilgilerinin yazdırılması sağlanmıştır. Buna göre örnek bir çalışma zamanı çıktısı aşağıdaki gibi olacaktır.

![blg123_Runtime.gif](/assets/images/2010/blg123_Runtime.gif)

Dikkat edileceği üzere istemci tarafında servis çağrısının yapılmasından sonra oluşan ve servis tarafında dosyaya yazılan Transaction bilgilerindeki DistributedIdentifier değerleri aynıdır. Bir başka deyişle istemci ve Workflow Service tarafı aynı Transaction alanı içerisinde çalıştırılmıştır. Hemen tersi durumu ispat etmeye çalışalım. Bunun için her iki taraftaki config dosyalarında yer alan transactionFlow niteliklerine false değer verdiğimizi düşünelim. Örneği tekrardan çalıştıralım. İşte sonuç;

![blg123_Runtime2.gif](/assets/images/2010/blg123_Runtime2.gif)

Görüldüğü gibi operasyon çağrısı sonucu istemcide üretilen ve servis tarafında dosyaya yazılan DistributedIdentifier değerleri 0' dır. 0 olması zaten bir dağıtık transaction oluşturulmadığı/oluşturulamadığı anlamına gelmektedir. İşte bu kadar.

![Wink](/assets/images/2010/smiley-wink.gif)

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Transactions.rar (54,45 kb)](/assets/files/2010/Transactions.rar)
