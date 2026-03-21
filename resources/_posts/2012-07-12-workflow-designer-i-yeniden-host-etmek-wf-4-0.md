---
layout: post
title: "Workflow Designer’ ı Yeniden Host Etmek (WF 4.0)"
date: 2012-07-12 23:05:00 +0300
categories:
  - wf-4-0
tags:
  - workflow-foundation
  - rehosted-workflow-designer
  - xaml
  - activity
  - custom-activity-designer
  - code-activity
  - native-activity
  - workflow-context
---
Çoğu zaman sinemada daha önceden vizyona girmiş olan bir filmin yeniden çekilmiş bir versiyonuna rastlarız. Örneğin Batman Begins veya vizyona bu yaz girecek Total Recall gibi. Hatta bazen Cover olarak adlandırdığımız bir durum söz konusu olur ve çeşitli müzik guruplarının önemli parçalarının tekrardan, aynı ekipçe veya başkalarınca yorumlandığını görür, duyarız.

[![total-recall-2012-official-trailer-teaser-00 (1)](/assets/images/2012/total-recall-2012-official-trailer-teaser-00%20%281%29_thumb.jpg)](/assets/images/2012/total-recall-2012-official-trailer-teaser-00%20%281%29.jpg)


Sonuç itibariyle insanlar zaman zaman yapılmış olan bazı çalışmaları hem teknolojinin yeni nimetleri, hem de farklı şekilde yorumlayabilme isteği nedeni ile tekrardan ele alabilirler.

Hatta bu felsefe yazılım dünyasında da zaman zaman vuku bulan bir senaryodur. Özellikle IDE tarafında. Bir IDE’ nin kabuğu üstüne giyidirilebilen parçaları farklılaştırabildiğinizi veya var olan IDE’ lerden farklı olan alternatiflerini üretebildiğinizi düşünün. Örneğin SharpDevelop

![Smile](/assets/images/2012/wlEmoticon-smile_39.png)

Aslına bakarsanız Visual Studio gerçekten harika bir IDE ortamı sunmaktadır. Hatta UX olarak bilien User eXperience değil de tam anlamıya Developer eXperience’ ın hat safhada olduğu bir geliştirme ortamıdır. Lakin genişletilebilir olması (Extension Manager’ a dikkatiniz çekmek isterim) haricinde çok gelişmiş özellikleri olmakla birlikte, zaman zaman daha hafif bir sürüme ihtiyaç duyabiliriz. Örneğin Workflow Foundation tabanlı olarak bir iş akışı tasarım uygulaması geliştirmek istediğinizi düşünün

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_98.png)

Bu tip uygulamalarda herşeyi baştan ele alıp Amerikayı tekrardan keşfetmeyi deneyebilirsiniz elbette. Ancak zaten elimizde var olan bir Designer ortamı var ise, sadece bunu alıp yeni bir kabuk giydirmeye çalışmak daha etkili ve hızlı bir çözüm olabilir. İşte bu yazımızda çok basit olarak Workflow Designer ortamının Visual Studio dışarısında nasıl kullanılabileceğini öğrenmeye çalışıyor olacağız. Aracımızdan beklediğimiz özellikler temel olarak aşağıdaki maddeler halinde ifade edilebilir.

Var olan Primitive Workflow Component’ leri veya bizim tarafımızdan geliştirilmiş bileşenleri içeren Toolbox’ a sahip olmalıdır.
Workflow içeriğinin tasarlanabileceği Visual Studio içerisindeki Designer bulunmalıdır.
Herhangibir Workflow bileşeni seçildiğinde, buna ait özelliklerin dolacağı ve tabiki değiştirilebileceği bir Properties penceresi yer almalıdır.
Tasarlanan Workflow örnekleri kayıt edilebilmeli veya XAML (eXtensible Application Markup Language) içerikli dosyalardan yüklenebilmelidir.
Tasarlanan veya yüklenen Workflow örnekleri çalıştırılabilmelidir.
Kullanıcı deneyimini yüksek tutmak istediğimizden WPF (Windows Presentation Foundation) tabanlı bir arayüz sunulabilmelidir.

Bu temel özellikleri gerçekleştirdiğimiz takdirde elimizde basit bir Workflow geliştirme aracı oluşacaktır. Söz konusu aracın daha da etkin hale getirilmesi için genişletilebilir bir yapıda tasarlanması önemlidir, ancak bu örneğimizde bu biraz daha göz ardı edilecek bir unsurdur

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_98.png)

Peki bu tip bir uygulama geliştirmek için elimizde neler var bir de buna bakalım dilerseniz.

WorkflowDesigner sınıfı ile tasarım ortamının birerbir kullanılabilmesi mümkün olacaktır.
ToolboxCategory, ToolboxControl, ToolboxItemWrapper tiplerinden yararlanarak Toolbox oluşturulabilir ve içeriğine Workflow bileşenleri atılabilir.
WorkflowDesigner tipinin PropertyInspectorView özelliği ile, Property penceresinin set edilmesi sağlanabilecektir.
WorkflowDesigner’ ın sunduğu Load ve Save metodları ile, bir akışın yüklenmesi veya kayıt altına alınması işlemleri gerçekleştirilebilir. Bu akışlar XAML tabanlı dosyalardan gelebileceği gibi (ki buna göre istediğimiz yerde bir Workflow Repository’ miz olabilir) canlı çalışma zamanı Activity örnekleri de olabilir.
XAML formatında saklanacak olan Workflow içeriklerinin çalışma zamanında (Runtime) yürütülebilmesi için elimizde ActivityXamlServices sınıfı bulunmaktadır.
Yüklenen bir Workflow’ un asenkron olarak çalıştırılabilmesi sağlamak için de WorkflowApplication tipinden yararlanılabilir.

Görüldüğü üzere elimizde hayal ettiğimiz gibi (?) bir Designer’ ın geliştirilebilmesi için gerekli materyaller bulunmaktadır. Tabi ilgili düşüncenin gerçek bir ürün haline getirilmesi için epey bir çaba da sarf etmek gerekecektir.

Biz şu an için sadece giriş noktasını tasarladığımızı ve aslında Workflow Designer’ ı Visual Studio IDE’ si dışında çalıştırabildiğimizi ispatlarsak önemli bir aşamayı geçmiş olduğumuzu var sayabiliriz. Öyleyse gelin hiç vakit kaybetmeden işe koyulalım. Örneğimizi Visual Studio 2012 RC sürümü üzerinde geliştiriyor olacağız ancak Visual Studio 2010 ortamında da test ettiğimizi ve çalıştırdığımızı ifade edebiliriz. Dolayısıyla kod parçalarını aynen Copy-Paste yöntemi ile 2010 ortamında da uygulatabilirsiniz.

WPF uygulaması olarak geliştireceğimiz projemizde, aşağıdaki şekilde görülen ve sarı kutucuk içerisine alınmış referansların bulunması gerekmektedir.

[![rwd_1](/assets/images/2012/rwd_1_thumb.png)](/assets/images/2012/rwd_1.png)

Dikkat edileceği üzere

System.Activities

System.Activities.Core.Presentation

ve System.Activities.Presentation

assembly’ larının yüklenmesi yukarıda bahsettiğimiz temel Designer tipleri için gereklidir. Kendi IDE’ mizin ana ekranını oluşturacak MainWindow.xaml içeriğini ise aşağıdaki kod parçasında görüldüğü gibi tasarlayabiliriz.

```xml
<Window x:Class="Designer.MainWindow" 
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" 
        Title="ING Composition Designer" Height="350" Width="800" WindowState="Maximized"> 
    <Grid x:Name="grdScene" Background="Black"> 
        <Grid.RowDefinitions> 
            <RowDefinition Height="10*"/> 
            <RowDefinition/> 
            <RowDefinition Height="1*"/> 
        </Grid.RowDefinitions> 
        <Grid.ColumnDefinitions> 
            <ColumnDefinition/> 
            <ColumnDefinition Width="4*"/> 
            <ColumnDefinition/> 
        </Grid.ColumnDefinitions> 
        <StackPanel Orientation="Horizontal" Grid.Row="1" Width="auto" Grid.ColumnSpan="3" Background="Black"> 
            <Button Background="Black" BorderBrush="Pink" Content="Yeni" Foreground="Plum" Width="75" x:Name="btnNew" Click="btnNew_Click" Margin="5,5,5,5" /> 
            <Button Background="Black" Foreground="Plum" BorderBrush="Pink" Content="Kaydet" x:Name="btnSave" Click="btnSave_Click" Margin="5,5,5,5" Width="75" /> 
            <Button Background="Black" Foreground="Plum" BorderBrush="Pink" Content="Yükle" x:Name="btnLoad" Click="btnLoad_Click"  Margin="5,5,5,5" Width="75" /> 
            <Button Background="Black" BorderBrush="Pink" Content="Çalıştır" Foreground="Plum" Width="75" Margin="5,5,5,5" x:Name="btnRun" Click="btnRun_Click" /> 
        </StackPanel> 
        <StackPanel Background="Black" Orientation="Horizontal" Grid.Row="2" Width="auto" Grid.ColumnSpan="3"> 
            <Label x:Name="labelStatus" Foreground="PaleVioletRed" FontWeight="Bold" FontStyle="Italic" HorizontalAlignment="Right"/> 
        </StackPanel> 
    </Grid> 
</Window>
```

Aslında tasarım olarak Visual Studio IDE’ sini sadece ucundan andıran bir görselliğimiz bulunmakta

![Embarrassed smile](/assets/images/2012/wlEmoticon-embarrassedsmile_2.png)

Uygulamanın sol tarafında Toolbox’ ımız, ortasında Workflow tasarımının yapılacağı alanımız ve en sağda bileşenlere ait Properties penceremiz bulunmaktadır. Şimdilik basit bir POC (Proof of Concept) çalışması olarak öngördüğümüzden temel fonksiyonelliklerimiz (Load,Save,Run,New) birer Button halinde pencerenin alt kısmında yer alacaktır. Olayı kafamızda daha iyi canlandırmak için uygulamamızın bitmiş halinin çalışma zamanındaki bir görüntüsüne bakalım arzu ederseniz.

[![rwd_2](/assets/images/2012/rwd_2_thumb.png)](/assets/images/2012/rwd_2.png)

Sanırım bu ekran görüntüsüne bakınca biraz daha heyecanlanmış ve iştahlanmış olabilirsiniz yanılıyor muyum?

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_98.png)

O halde kod tarafında neler yaptığımıza bir bakalım. İşte kodlarımız.

```csharp
using System; 
using System.Activities; 
using System.Activities.Core.Presentation; 
using System.Activities.Presentation; 
using System.Activities.Presentation.Toolbox; 
using System.Activities.Statements; 
using System.Activities.XamlIntegration; 
using System.IO; 
using System.Threading; 
using System.Windows; 
using System.Windows.Controls; 
using Microsoft.Win32;

namespace Designer 
{ 
    // Illegal Cross Thread Exception' dan kaçmak için kullandığımız temsilci tipi 
    delegate void BindCompletionStateToLabelDelegate(WorkflowApplicationCompletedEventArgs state);

    public partial class MainWindow 
        : Window 
    { 
        // WorkflowDesigner 
        private WorkflowDesigner wfDesigner; 
        private string currentFileName = String.Empty;

        public MainWindow() 
        { 
            InitializeComponent();

            RegisterMetadata(); 
            AddDesigner(new Flowchart()); 
            AddToolBox(); 
            AddPropertyInspector(); 
        }

        // Designer için Metadata içeriği register edilir 
        private void RegisterMetadata() 
        { 
            DesignerMetadata dMetadata = new DesignerMetadata(); 
            dMetadata.Register(); 
        }

        // Bir instance' dan WorkflowDesigner' ın yüklenilmesi sağlanır. Örneğin bir Flowchart bileşeni veya Sequence designer içerisine açılabilir. 
        private void AddDesigner(object instance) 
        { 
            wfDesigner = new WorkflowDesigner();            
            wfDesigner.Load(instance); 
            BindDesignerEvents(wfDesigner); 
            BindWfDesignerToGrid(); 
        }

        // WorkflowDesigner içeriğinin XAML tabanlı bir dosya içeriğindeki activity ile yüklenmesini sağlar 
        private void AddDesigner(string xamlFileName) 
        { 
            wfDesigner = new WorkflowDesigner(); 
            wfDesigner.Load(xamlFileName); 
            BindDesignerEvents(wfDesigner); 
            BindWfDesignerToGrid(); 
        }

        // WorkflowDesigner' ın örnek olay metodlarını yükler 
        private void BindDesignerEvents(WorkflowDesigner wfDesigner) 
        { 
            wfDesigner.ModelChanged += (obj, e) => 
            { 
                labelStatus.Content = "Model Changed"; 
            }; 
        }

        // WorkflowDesigner bileşeninin WPF içeriğine element olarak eklenmesini sağlar 
        private void BindWfDesignerToGrid() 
        { 
            Grid.SetColumn(wfDesigner.View, 1); 
            if (Grid.GetColumn(wfDesigner.View) == 1) 
                grdScene.Children.Remove(wfDesigner.View); 
            grdScene.Children.Add(wfDesigner.View); 
        }

        // Toolbox içeriğinin WPF penceresine eklenmesini sağlar 
        private void AddToolBox() 
        { 
            ToolboxControl control = GetToolboxControl(); 
            Grid.SetColumn(control, 0); 
            grdScene.Children.Add(control); 
        }

        // Örnek Workflow Component' lerinin Toolbox içeriğine eklenmesini sağlar 
        private ToolboxControl GetToolboxControl() 
        { 
            ToolboxControl tbxControl = new ToolboxControl();

            #region Genel

            ToolboxCategory tbxStandartControls = new ToolboxCategory("Genel");

            ToolboxItemWrapper toolAssign = new ToolboxItemWrapper(typeof(Assign), "Atama"); 
            ToolboxItemWrapper toolFlowDecision = new ToolboxItemWrapper(typeof(FlowDecision), "Karar"); 
            ToolboxItemWrapper toolFlowchart = new ToolboxItemWrapper(typeof(Flowchart), "Akış Diagramı"); 
            ToolboxItemWrapper toolIf = new ToolboxItemWrapper(typeof(If), "Eğer"); 
            ToolboxItemWrapper toolSequence = new ToolboxItemWrapper(typeof(Sequence), "Sekans"); 
            ToolboxItemWrapper toolDelay = new ToolboxItemWrapper(typeof(Delay), "Duraksat"); 
            ToolboxItemWrapper toolDoWhile = new ToolboxItemWrapper(typeof(DoWhile), "Do While Döngü");

            tbxStandartControls.Add(toolAssign); 
            tbxStandartControls.Add(toolSequence); 
            tbxStandartControls.Add(toolDelay); 
            tbxStandartControls.Add(toolDoWhile); 
            tbxStandartControls.Add(toolFlowchart); 
            tbxStandartControls.Add(toolFlowDecision); 
            tbxStandartControls.Add(toolIf);

            #endregion

            #region Adapterler

            ToolboxCategory tbxAdapterControls = new ToolboxCategory("External Adapters");

            tbxControl.Categories.Add(tbxStandartControls); 
            tbxControl.Categories.Add(tbxAdapterControls);

            #endregion

            return tbxControl; 
        }

        // Seçilen Workflow bileşeni veya component için gerekli özelliklerin yüklenmesini sağlar 
        private void AddPropertyInspector() 
        { 
            if (Grid.GetColumn(wfDesigner.PropertyInspectorView) == 2) 
            { 
                grdScene.Children.Remove(wfDesigner.PropertyInspectorView); 
            } 
            else 
            { 
                Grid.SetColumn(wfDesigner.PropertyInspectorView, 2); 
                grdScene.Children.Add(wfDesigner.PropertyInspectorView); 
            } 
        }

        // Tasarlanan Workflow içeriğinin XAML uzantılı olarak kayıt edilmesini sağlar 
        private void btnSave_Click(object sender, RoutedEventArgs e) 
        { 
            SaveFileDialog sfd = new SaveFileDialog(); 
            sfd.Filter = "XAML Files|*.xaml"; 
            sfd.InitialDirectory = Environment.CurrentDirectory; 
            if (sfd.ShowDialog().Value == true) 
            { 
                wfDesigner.Save(sfd.FileName); 
                currentFileName = sfd.FileName; 
                labelStatus.Content = string.Format("{0}(Saved)", sfd.FileName); 
            } 
        }

        // Workflow' un designer içerisine XAML tabanlı bir içerikten okunmasını sağlar 
        private void btnLoad_Click(object sender, RoutedEventArgs e) 
        { 
            OpenFileDialog ofd = new OpenFileDialog(); 
            ofd.Filter = "XAML Files|*.xaml"; 
            ofd.InitialDirectory = Environment.CurrentDirectory; 
            if (ofd.ShowDialog().Value == true) 
            { 
                AddDesigner(ofd.FileName); 
                AddPropertyInspector(); 
                currentFileName = ofd.FileName; 
                labelStatus.Content = string.Format("{0}(Loaded)", ofd.FileName); 
            } 
        }

        // Boş bir Flowchart içerikli Workflow oluşturulmasını sağlar 
        private void btnNew_Click(object sender, RoutedEventArgs e) 
        { 
            AddDesigner(new Flowchart()); 
            AddPropertyInspector(); 
            labelStatus.Content = "Yeni Akışı"; 
            currentFileName = String.Empty; 
        }

        // Workflow' un Run edilmesi için kullanılır 
        private void btnRun_Click(object sender, RoutedEventArgs e) 
        { 
            try 
            { 
                Activity activity = LoadActivity(currentFileName); 
                ExecuteActivity(activity); 
            } 
            catch (Exception excp) 
            { 
                labelStatus.Content = string.Format("{0} nedeni ile activity başarılı bir şekilde çalıştırılamadı", excp.Message); 
            } 
        }

        // Workflow icrasını gerçekleştirir 
        private void ExecuteActivity(Activity activity) 
        { 
            AutoResetEvent aReseter = new AutoResetEvent(false);

            WorkflowApplication wfApp = new WorkflowApplication(activity); 
            wfApp.Completed += ea => 
            { 
                aReseter.Set(); 
                labelStatus 
                    .Dispatcher 
                    .Invoke( 
                    new BindCompletionStateToLabelDelegate(BindCompletaionStateToLabel) 
                    , ea 
                    ); 
            }; 
            wfApp.Run(); 
            aReseter.WaitOne(); 
        }

        // Illegal Cross Thread Exception' dan kaçtığımız ve Label kontrolüne diğer Thread içerisinden değiştirmemizi sağlayan metod 
        private void BindCompletaionStateToLabel(WorkflowApplicationCompletedEventArgs state) 
        { 
            labelStatus.Content = string.Format( 
                "{0} ID li akış için Completion State {1}" 
                , state.InstanceId, state.CompletionState 
                ); 
        }

        // XAML içeriğinden Activity bileşenini üretmek için kullanılır 
        private Activity LoadActivity(string currentFileName) 
        { 
            Activity loadedActivity = null;

            if (!string.IsNullOrEmpty(currentFileName) 
                && Path.GetExtension(currentFileName).ToUpper() == ".XAML") 
            { 
                loadedActivity = ActivityXamlServices.Load(currentFileName); 
                labelStatus.Content = string.Format( 
                    "{0}({2}) run edilmek üzere {1} lokasyonundan yüklendi" 
                    , loadedActivity.DisplayName 
                    , currentFileName 
                    , loadedActivity.Id 
                    ); 
            } 
            return loadedActivity; 
        } 
    } 
}
```

Kodlar uzun görünmesine rağmen çok karmaşık değildir. Designer’ ın yüklenmesi veya designer içeriğinin kayıt edilmesi gibi işlevsellikler WorkflowDesigner tipi üzerinden gerçekleştirilebilmektedir. Toolbox içeriğinin yüklenmesi veya Properties penceresinin üretilmesi için gerekli tipler de başta belirttiğimiz gibidir. Örnek olması açısında basit bir kaç Workflow bileşeni yüklenmiştir (Assign, Delay vb) Burada önemli olan bir diğer nokta da Toolbox üzerinde istediğimiz gibi kategorilendirme yapabilmemizdir. Hatta ToolboxItemWrapper tipinin aşırı yükleniş yapıcılarına bakıldığında farklı şekillerde Component yükleyebileceğimizi de görebiliriz ki bu bize önemli bir esneklikte sağlayacaktır. Component’ lerin herhangibir Repository’ den alınması gibi. Bunun dışında kalan kısımlar aslında bakarsanız Workflow tiplerinin Runtime’ daki davranışları için kullandığımız basit tiplerdir (WorkflowApplication, ActivityXamlServices vb)

> Ne yazık ki yeni oluşturmak istediğimiz her Workflow örneği için WorkflowDesigner tipinin tekrardan örneklenmesi ve Toolbox ile Property penceresi içeriklerinin WPF Grid kontrolüne sıfırdan bağlanması gibi henüz aşmayı başaramadığım bazı zorunluluklar/aksaklıklar da bulunmaktadır.

Aslında bu tip Visual Studio IDE’ si dışında bir Designer geliştirmenin ne gibi artıları olabileceğini de bir düşünmemiz ve masaya koymamız gerekmektedir. Bunları aşağıdaki maddeler halinde sıralayabiliriz.

Visual Studio ürünü dışında daha basit içeriğe sahip olup örneğin sadece İş Analistlerini hedef alan bir araca sahip olabiliriz.
Workflow Foundation içerisindeki tüm Component seti yerine sadece işe ve ihtiyaca yönelik bileşenlerin yer aldığı bir Toolbox’ un ürün bazlı olarak sunulabilmesini sağlayabiliriz.
Söz konusu Toolbox içeriği yetkilendirilebilir (Authorization) ve ürünü kullananların pozisyonlarına göre yapabilecekleri sınırlanabilir.
Bir dezavantaj olarak görebileceğimiz Debug etme güçlüğüne karşılık ürünün Developer profili dışında kullanılacağı tezini öne sürebiliriz ![Smile](/assets/images/2012/wlEmoticon-smile_39.png)
Geliştirilen araç, Visual Studio bağımsız bir ürün olarak düşünülüp lisanslanabilir veya ücretsiz olarak şirket içi çalışmalarda değerlendirilebilir.
Özellikle görsel yeteneğe sahip Workflow Activity bileşenlerinin, bu aracın Visual Studio ile birlikte çalıştırılması halinde, Designer’ a bağlandıklarındaki davranışlarının Debug edilmesi çok daha kolay olacaktır (Acısını çok çektim o yüzden kulak verin bu avantajı yaban atmayın ![Smile](/assets/images/2012/wlEmoticon-smile_39.png))

Görüldüğü üzere Visual Studio IDE’ sinin bir parçası olarak sunulan ve kullanılan Workflow Designer’ ın harici bir ürün haline dönüştürülmesi son derece kolaydır. Umarım vizyonunuza değer katacak bir çalışma olmuştur. Bir başka yazımızda görüşünceye dek hepinize mutlu günler dilerim.

[ReHostedWFDesigner.zip (75,05 kb)](/assets/files/2012/ReHostedWFDesigner.zip)
(Örnek Visual Studio 2012 RC sürümünde geliştirilmiştir ancak kodlar Visual Studio 2010 üzerinde de çalışmaktadır)

