---
title: "WF 4.0 Beta 1' den Beta 2' ye"
pubDate: 2009-10-27 06:25:00
categories:
  - WF 4.0 Beta 2
tags:
  - WF 4.0 Beta 2
  - C#
  - XML
  - .NET
  - LINQ
  - Workflow Foundation
  - XAML
  - HTTP
  - Authentication
  - Transactions
  - Generics
  - Debugging
  - Visual Studio
  - Rc
---

# WF 4.0 Beta 1' den Beta 2' ye
![blg93_Giris.jpg](images/blg93_Giris.jpg)

Merhaba Arkadaşlar,

Hatırlıyorumda bundan bir kaç sene önce Visual Studio 2005 in henüz Build sürümleri CD olarak değerli bir Microsoft çalışanı tarafından bana hediye edilmişti. Whidbey kod adlı sürümde pek çok yenilik olduğu gibi IDE'nin zaman zaman istisnalar (Exception) vererek çöktüğüne de sıklıkla şahit olmuştum.

Tabiki Microsoft cephesinden gelen yenilikleri takip etmek, nasıl ilerlenildiğini izlemek adına Alpha, Beta, RC gibi sürümlerle çalışmanın oldukça faydası oluyor. En azından getirilen yeniliklerin neden değiştirildiğini, neden kaldırıldığını veya yeni eklenen bileşenlerin hangi amaçla düşünüldüğünü görme ve araştırma şansını buluyoruz. Bazende anlayamıyor veya bulamıyoruz

![Smile](images/smiley-smile.gif)

Tabi zaman zaman bulmacanın içerisinde kayıp parçalar da olabiliyor. İşte böyle bir durum kısa bir süre önce benim başımada geldi.

Workflow Service'ler ile çalışırken Beta 1 sürümünde geliştirdiğim örneklerin Beta 2 sürümünde ne yazık ki çalışmadığını farkettim. Bu son derece doğaldı çünkü kısa bir süre önce yayınlanan.Net Framework Beta 2 sürümünde, WF tarafında özellikle tipler bazında bazı geri dönüşler ve değişimler meydana geldi. Hal böyle olunca konuyu hemen araştırmaya koyuldum. Pek çok blog yazısında WF 4.0 Beta 1 ve Beta 2 arasındaki farklılıkları bulabilirsiniz.

İşe ilk olarak daha önceden sıkça bahsedilen ata Workflow sınıfının (WorkflowElement) artık kaldırıldığını söyleyerek başlamak gerekiyor. Artık Activity isimlendirilmesine geri dönüş yapmış durumdayız. Aslında Beta 1 sürümünde Workflow tip hiyerarşisinin aşağıdaki gibi olduğunu görmüştük.

![blg93_Beta1.gif](images/blg93_Beta1.gif)

Görüldüğü üzere tüm aktiviteler WorkflowElement veya generic olan versiyonundan türemektedir. Bunların haricinde özellikle paralel geliştirmeye yönelik herhangibir aktivite bileşenine yer verilmediği de görülmektedir. Ancak Beta 2 sürümünde tip hiyerarşisi değişerek aşağıdaki sınıf diagramında görülen hale getirilmiştir.

![blg93_Beta2.gif](images/blg93_Beta2.gif)

İlk dikkati çeken noktalardan birisi WorkflowElement tipinin artık olmayışı ve Activity tipinin devreye alınışıdır. Buna ek olarak Expression kullanımları için Activity ve türevleri getirilmiştir. Ancak belkide en önemli noktalardan birisi paralel işlemler için asenkron aktivite bileşenlerinin getirilişidir. AsyncCodeActivity ve AsyncCodeActivity.

Tip hiyerarşisinin değişmesi elbetteki pek çok noktayı farklı şekilde etkilemektedir. Söz gelimi XAML içerikli bir WF dosyasının çalışma zamanında yüklenip yürütülebilmesi için Beta 1 sürümünde aşağıdaki gibi bir kod parçası kullanılmaktaydı.

```csharp
WorkflowElement workflow1;
using (Stream workflow1Stream= File.OpenRead("Workflow1.xaml"))
{
    workflow1= WorkflowXamlServices.Load(workflow1Stream);
}
var outputs = WorkflowInvoker.Invoke (workflow1);
```

Önce WorkflowElement nesne örneği oluşturulmaktaydı. Sonrasında ise XAML içeriği bir Stream içerisine alınıp WorkflowXamlServices tipinin static Load metodu yardımıyla belleğe yüklenmekteydi. Sonrasında ise WorkflowInvoker tipinin static Invoke metoduna parametre olarak gönderilip, XAML içeriğinden workflow'un yürütülmeye başlanması sağlanmaktaydı. Di'li geçmiş zaman kullandığımı farketmiş olmalısınız

![Wink](images/smiley-wink.gif)

(Gerçi WorkflowInvoker.Invoke halen mevcut.)

Ancak bu kodlama Beta 2 sürümünde değişmiştir. Söz gelimi elimizde aşağıdaki uzun içeriğe sahip Hesaplama.xaml isimli bir Workflow dosyası olduğunu düşünelim.

Kişisel Not: XAML içeriğini kavramak için kısmından okumaya başlarsanız çok kolay bir şekilde anlaşılabileceğini görebilirsiniz ![Wink](images/smiley-wink.gif)

```xml
<Activity mc:Ignorable="sap" x:Class="WorkflowConsoleApplication2.Hesaplama" xmlns="http://schemas.microsoft.com/netfx/2009/xaml/activities" xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" xmlns:mv="clr-namespace:Microsoft.VisualBasic;assembly=System" xmlns:mva="clr-namespace:Microsoft.VisualBasic.Activities;assembly=System.Activities" xmlns:s="clr-namespace:System;assembly=mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" xmlns:s1="clr-namespace:System;assembly=mscorlib" xmlns:s2="clr-namespace:System;assembly=System" xmlns:s3="clr-namespace:System;assembly=System.Xml" xmlns:s4="clr-namespace:System;assembly=System.Core" xmlns:sad="clr-namespace:System.Activities.Debugger;assembly=System.Activities" xmlns:sap="http://schemas.microsoft.com/netfx/2009/xaml/activities/presentation" xmlns:scg="clr-namespace:System.Collections.Generic;assembly=mscorlib" xmlns:scg1="clr-namespace:System.Collections.Generic;assembly=System" xmlns:scg2="clr-namespace:System.Collections.Generic;assembly=System.ServiceModel" xmlns:scg3="clr-namespace:System.Collections.Generic;assembly=System.Core" xmlns:sd="clr-namespace:System.Data;assembly=System.Data" xmlns:sd1="clr-namespace:System.Data;assembly=System.Data.DataSetExtensions" xmlns:sl="clr-namespace:System.Linq;assembly=System.Core" xmlns:st="clr-namespace:System.Text;assembly=mscorlib" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
  <x:Members>
    <x:Property Name="YourName" Type="InArgument(x:String)" />
  </x:Members>
  <mva:VisualBasic.Settings>Assembly references and imported namespaces serialized as XML namespaces</mva:VisualBasic.Settings>
  <Sequence sad:XamlDebuggerXmlReader.FileName="C:\Documents and Settings\bsenyurt\my documents\visual studio 10\Projects\WorkflowConsoleApplication2\WorkflowConsoleApplication2\Hesaplama.xaml" sap:VirtualizedContainerService.HintSize="264,342">
    <sap:WorkflowViewStateService.ViewState>
      <scg:Dictionary x:TypeArguments="x:String, x:Object">
        <x:Boolean x:Key="IsExpanded">True</x:Boolean>
      </scg:Dictionary>
    </sap:WorkflowViewStateService.ViewState>
    <Assign sap:VirtualizedContainerService.HintSize="242,57">
      <Assign.To>
        <OutArgument x:TypeArguments="x:String">[YourName]</OutArgument>
      </Assign.To>
      <Assign.Value>
        <InArgument x:TypeArguments="x:String">Burak Selim Şenyurt</InArgument>
      </Assign.Value>
    </Assign>
    <Delay Duration="00:00:03" sap:VirtualizedContainerService.HintSize="242,22" />
    <WriteLine sap:VirtualizedContainerService.HintSize="242,59" Text="[String.Format("Merhaba {0}", YourName)]" />
  </Sequence>
</Activity>
```

Bu XAML dosyasının çalışma zamanında yürütülmesi için Beta 2 sürümüne ait kodlama aşağıdaki gibi olacaktır.

```csharp
using System.Activities;
using System.Activities.XamlIntegration;

namespace WorkflowConsoleApplication2
{

    class Program
    {
        static void Main(string[] args)
        {
            Activity activity=ActivityXamlServices.Load("..\\..\\Hesaplama.xaml");
            var outputs=WorkflowInvoker.Invoke(activity);
        }
    }
}
```

Öncelikli olarak Activity tipine ait bir nesne örneklenmektedir. Bu örnekleme işleminde ActivityXamlServices sınıfının static Load metodu kullanılır ve parametre olarak XAML dosyasını adres bilgisi verilir. Belleğe yüklenen Activity nesne örneğinin çalıştırılması için yapılan işlem Beta 1 sürümündeki ile benzerdir. Yine WorkflowInvoker sınıfının static Invoke metodu kullanılmaktadır. Ama tabi bu Invoke metodu Activity tipi ile çalışacak şekilde değiştirilmiştir.

Elbette başka farklılıklarda bulunmaktadır. Söz gelimi WF 4.0 içerisinde önceki sürüme ait aktivite bileşenlerinin sarmalanarak çalıştırılmasına destek vermek amacıyla getirilen Interop aktivitesinde doğrulama (Validation) ve Transaction Yönetimi ile ilişkili iyileştirilmeler yapılmıştır.

Özellikle Workflow Service'lerde Send ve Receive gibi aktivitelerin dış ortamdan aldıkları parametreler için Parameters kullanımından vazgeçilmiş bunun yerine ilgili aktivitelerin Content özellikleri içerisinden parametre seçimlerinin yapılabilmesine olanak sağlanmıştır.

Yine Workflow seyivesinde tip güvenli (Type Safe) özelliklerin tanımlanabilmesi ve bunlara Workflow nesne örnekleri üzerinden kod bazında erişilebiliyor olması söz konusudur. Bu konu ile ilişkili olarak ilgili adresteki [görsel derslerimi](https://www.buraksenyurt.com/post/Screencast-WF-35-ve-WF-40-Beta-2-Parametre-Kullanimi)izleyebilirsiniz.

Bunlar ve daha pek çok farklılığı veya yeniliği ilerleyen yazılarımızda veya görsel derslerimizde ele almaya çalışıyor olacağız. Tabiki Release sürüme yaklaştıkça çok şeyin değiştiğini de görebiliriz. Bu nedenle buradaki mimari modellerinde kalıcı olduğunu garanti etmemiz yanlış olacaktır. Ancak tüm bu yenilenmeler ve çalışmalar geliştiric olarak bizlerin iyiliği içindir.

![Wink](images/smiley-wink.gif)

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
