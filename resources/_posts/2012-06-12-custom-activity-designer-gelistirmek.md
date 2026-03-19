---
layout: post
title: "Custom Activity Designer Geliştirmek"
date: 2012-06-12 07:00:00 +0300
description: "Geçtiğimiz günlerde Workflow Foundation tarafında bir Component Set' in geliştirilmesi üzerine açılan POC(Proof of Concept) projesinde görev aldım. Bu anlamda yoğun bir şekilde Custom Activity Designer konusu ile yakın ilişki içerisinde yer almam gerekti. Workflow Foundation' ın bileşen seti her ne kadar geniş bir yelpazeye sahip olsa da, özellikle uygulama geliştiricilerin hızlı bir şekilde Workflow(Flow Chart, Sequential vb) tasarlaması gerektiği durumlarda, işleri kolaylaştıracak Component setlerinin üretilmesi son derece önemlidir. Ne varki XAML tabanlı çalışan Activity Designer örnekleri, Visual Studio IDE' si ile pek kardeşçe yaşamamaktadır(Bu durumun Visual Studio 2012' de devam etmediğini umuyorum). Dikkat edilmesi gereken pek çok nokta ve ip ucu bulunmakta. Dilerseniz ne demek istediğimi örnek bir senaryo üzerinden görmeye çalışalım."
categories:
  - wf
  - wf-4-0
tags:
  - wf
  - wf-4-0
  - csharp
  - xml
  - workflow-foundation
  - wpf
  - xaml
  - http
  - generics
  - visual-studio
  - datatable
---
# Custom Activity Designer Geliştirmek
![Challenge.jpg](/assets/images/2012/Challenge.jpg)

Merhaba Arkadaşlar,

İnsanın kendisini en çok geliştireceği yer gerçek çalışma sahaları/ortamlarıdır. Ortaya konan ihtiyaçlar ne zaman ki sizin kullanmakta olduğunuz araçların (Tools) sınırlarını zorlamaya başlar, bu noktadan itibaren içerisine gireceğiniz her çeşit mücadele size inanılmaz derece tecrübe ve bilgi katacaktır. Tabi bu know-how bilgisini saklayabilir, kendiniz için dökümante edebilir veya kuralları çerçevesinde paylaşabilirsiniz

![Wink](/assets/images/2012/smiley-wink.gif)

Geçtiğimiz günlerde Workflow Foundation tarafında bir Component Set'in geliştirilmesi üzerine açılan POC (Proof of Concept) projesinde görev aldım. Bu anlamda yoğun bir şekilde Custom Activity Designer konusu ile yakın ilişki içerisinde yer almam gerekti. Workflow Foundation'ın bileşen seti her ne kadar geniş bir yelpazeye sahip olsa da, özellikle uygulama geliştiricilerin hızlı bir şekilde Workflow (Flow Chart, Sequential vb) tasarlaması gerektiği durumlarda, işleri kolaylaştıracak Component setlerinin üretilmesi son derece önemlidir. Ne varki XAML tabanlı çalışan Activity Designer örnekleri, Visual Studio IDE'si ile pek kardeşçe yaşamamaktadır (Bu durumun Visual Studio 2012' de devam etmediğini umuyorum). Dikkat edilmesi gereken pek çok nokta ve ip ucu bulunmakta. Dilerseniz ne demek istediğimi örnek bir senaryo üzerinden görmeye çalışalım.

Senaryomuzda metod adlarını ve bu fonksiyonlara bağlı parametre listelerini gösteren basit bir Workflow Activity bileşenini tasarlamaya çalışıyor olacağız. Bileşenimiz standart bir Code/Native Activity’ den farklı olarak görsel arayüze sahip olacak ve Visual Studio IDE’ si içerisinden de kullanılabilecek. Bir başka deyişle ToolBox sekmesinden designer ortamına sürükleyip bıraktığımızda, IDE kullanıcısı ile etkileşim içerisinde olacak. Dolayısıyla Activity Designer tipini ele alacağımız bir örnek üzerinde çalışıyor olacağız. İlk olarak projelerimizi oluşturarak işe başlayalım. Bu anlamda Solution içeriğini aşağıdaki şekilde görüldüğü gibi tasarlayabiliriz.

![wda_1.png](/assets/images/2012/wda_1.png)

Solution yapısı oldukça önemlidir. Activity projesi NativeActivity türevli tipleri barındırıyor iken, Design kütüphanesinde sadece görsel tasarımlar yer alacaktır. Azon.Workflow.Activity projesi Activity Library tipinden iken Azon.Workflow.Activity.Design, Activity Designer Library tipindendir. Burada Visual Studio 2010 IDE’ sinin beklediği bir isimlendirme standartı bulunmaktadır. Buna göre, Component’ in görsel arayüzünün tasarlanacağı kütüphane adının mutlaka Design kelimesi ile bitmesi gerekmektedir (Bu bilgiyi bulmak oldukça fazla vakit kaybına neden oldu. Ben en başından söylemek istiyorum ![Wink](/assets/images/2012/smiley-wink.gif))

Yolumuza Native Activity bileşenimizi geliştirerek devam edelim. Azon.Workflow.Activity kütüphanesi içerisinde aşağıdaki sınıf diagramında görülen tipleri üretiyor olacağız. Senaryomuza göre bileşenimiz, kaynak bir listede yer alan metod adlarını ve bunlara ait parametreleri gösteriyor olacak. İlk hedefimiz bu.

![wda_2.png](/assets/images/2012/wda_2.png)

Şimdi tiplerimiz içeriklerini biraz değerlendirelim.

InstanceMethodActivity.cs

```csharp
using System.Activities;

namespace Azon.Workflow.Activity
{
    public sealed class InstanceMethodActivity
        :NativeActivity<object>
    {
        public InArgument<string> Description { get; set; }
        public string MethodName { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            //TODO@Burak burada bir takım kodlar işletilir
        }
    }
}
```

InstanceMethodActivity, türetilemeyen (Sealed) ve NativeActivity türevli bir tiptir. CodeActivity türevli tiplere benzer olarak, çalışma zamanındaki işlerini Execute metodu içerisinde icra etmektedir. Örneğimizde söz konusu Activity bileşeni için herhangibir Runtime işlemi uygulatmıyor olacağız. Asıl hedefimiz Visual Studio 2010 IDE'sinde Design Time Support'unu sağlayabilmektir. Tipimizin içerisinde InArgument tipinden Description ve string türünden MethodName isimli iki özellik (Property) yer almaktadır. Designer tarafında işimize yarayacak olan sınıflar ise InstanceMethod, InstanceMethodParameter, ParameterType (Enum sabiti) ve InstanceMethodList'tir.

```csharp
namespace Azon.Workflow.Activity
{
    using System.Collections.Generic;

    public class InstanceMethod
    {
        public string Name { get; set; }
        public List<InstanceMethodParameter> Parameters{ get; set; }
    }
}
```

InstanceMethod, aslında bir metodun adını ve parametrik yapısını taşımak üzere tasarlanmış bir POCO (Plain Old Clr Object) tipidir. Parameters özelliği InstanceMethodParameter tipinden generic bir List koleksiyonudur ve ilgili sınıfın içeriği de aşağıdaki gibidir.

```csharp
namespace Azon.Workflow.Activity
{
    public class InstanceMethodParameter
    {
        public string Name { get; set; }
        public string DotNetType { get; set; }
        public ParameterType ParameterType{ get; set; }
    }
}
```

Bu tip içerisinde ise sembolik olarak metod parametrelerine ait çeşitli bilgiler yer almaktadır. Örneğin parametrenin adı,.Net Framework Common Type System deki karşılığı gibi. ParameterType enum sabiti ile de ilgili parametrenin ne çeşitte olduğu belirtilmektedir.

```csharp
namespace Azon.Workflow.Activity
{
    public enum ParameterType
    {
        Ref,
        Out,
        Standart,
        Return,
        Params
    }
}
```

Bu kütüphane içerisindeki en önemli tip ise ObservableCollection türevli olan InstanceMethodList'dir.

```csharp
namespace Azon.Workflow.Activity
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class InstanceMethodList
        :ObservableCollection<InstanceMethod>
    {
        public InstanceMethodList()
        {
            Add(new InstanceMethod
            {
                Name = "Sum",
                Parameters = new List<InstanceMethodParameter>{
                    new InstanceMethodParameter{ Name="X", DotNetType="System.Int32", ParameterType= ParameterType.Standart},
                    new InstanceMethodParameter{ Name="Y", DotNetType="System.Int32", ParameterType= ParameterType.Standart},
                    new InstanceMethodParameter{ Name="Result", DotNetType="System.Int32", ParameterType= ParameterType.Return}
                }
            });
            Add(new InstanceMethod
            {
                Name = "TotalSum",
                Parameters = new List<InstanceMethodParameter>{
                    new InstanceMethodParameter{ Name="Values", DotNetType="System.Int32[]", ParameterType= ParameterType.Params},
                    new InstanceMethodParameter{ Name="Result", DotNetType="System.Int32", ParameterType= ParameterType.Return}
                }
            });
            Add(new InstanceMethod
            {
                Name = "CallSp",
                Parameters = new List<InstanceMethodParameter>{
                    new InstanceMethodParameter{ Name="SpName", DotNetType="System.String", ParameterType= ParameterType.Standart},
                    new InstanceMethodParameter{ Name="ResultSet", DotNetType="System.Data.DataTable", ParameterType= ParameterType.Ref}
                }
            });
        }
    }
}
```

Bu tip aslında Activity Designer'ın XAML tabanlı içeriğinde ele alacağımız Data Binding işlemleri için kullanılmaktadır. ObservableCollection türevli olmasının sebebi de budur. Amacımız tipin kendisini ComboBox ve DataGrid kontrollerine bağlamaktır. Yapıcı (Constructor) metod içerisinde, örnek metod bilgilerinin eklendiği görülmektedir. Elbetteki bir gerçek hayat senaryosunda ilgili içeriklerin farklı veri ortamlarından tedarik edilmesi de düşünülebilir. Örneğin bu bilgileri bir servis üzerinden veya doğrudan erişilebilen ve InProc modda kullanabildiğimiz bir Assembly içerisinden de getirtebiliriz.

Gelelim bileşenimizin arayüzünü tasarlayacağımız Activity Designer öğesine. Azon.Workflow.Activity.Design kütüphanesinde oluşturacağımız InstanceMethodActivityDesigner.xaml tipinin içeriğini aşağıdaki gibi tasarlayabiliriz.

```xml
<sap:ActivityDesigner x:Class="Azon.Workflow.Activity.Design.InstanceMethodActivityDesigner"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" xmlns:s="clr-namespace:System;assembly=mscorlib" 
    xmlns:sap="clr-namespace:System.Activities.Presentation;assembly=System.Activities.Presentation"
    xmlns:sapv="clr-namespace:System.Activities.Presentation.View;assembly=System.Activities.Presentation"
    xmlns:Model="clr-namespace:System.Activities.Presentation.Model;assembly=System.Activities.Presentation"
    xmlns:sapc="clr-namespace:System.Activities.Presentation.Converters;assembly=System.Activities.Presentation"
    xmlns:activity="clr-namespace:Azon.Workflow.Activity;assembly=Azon.Workflow.Activity"
    mc:Ignorable="d" xmlns:d="http://schemas.microsoft.com/expression/blend/2008" xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006">
    <sap:ActivityDesigner.Resources>
        <ResourceDictionary x:Uid="ResourceDictionary_0">
            <sapc:ModelToObjectValueConverter x:Key="ModelToObjectValueConverter" />
            <ObjectDataProvider x:Key="dsInstanceMethods" ObjectType="{x:Type activity:InstanceMethodList}">
            </ObjectDataProvider>

            <DataTemplate x:Key="Collapsed">
                <StackPanel Orientation="Horizontal">
                    <TextBlock VerticalAlignment="Center" Margin="5" Text="Method Caller" />
                </StackPanel>
            </DataTemplate>

            <DataTemplate x:Key="Expanded">
                <Grid>
                    <Grid.RowDefinitions>
                        <RowDefinition/>
                        <RowDefinition/>
                        <RowDefinition/>
                        <RowDefinition/>
                    </Grid.RowDefinitions>
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition/>
                    </Grid.ColumnDefinitions>
                    <TextBlock Text="Metodlar" Grid.Row="0"/>
                    <ComboBox x:Name="cmbInstanceMethods" Grid.Row="1" ItemsSource="{Binding Source={StaticResource dsInstanceMethods}}" DisplayMemberPath="Name" IsSynchronizedWithCurrentItem="True" />
                    <TextBlock Text="Metod Parametreleri" Grid.Row="2"/>
                    <DataGrid x:Name="grdMethodParameters" Grid.Row="3" IsSynchronizedWithCurrentItem="True" ItemsSource="{Binding Source={StaticResource dsInstanceMethods}, Path=Parameters}"/>
                </Grid>
            </DataTemplate>
            <Style x:Key="ExpandOrCollapsedStyle" TargetType="{x:Type ContentPresenter}">
                <Setter Property="ContentTemplate" Value="{DynamicResource Expanded}" />
                <Style.Triggers>
                    <DataTrigger Binding="{Binding Path=ShowExpanded}" Value="true">
                        <Setter Property="ContentTemplate" Value="{DynamicResource Collapsed}" />
                    </DataTrigger>
                </Style.Triggers>
            </Style>

        </ResourceDictionary>
    </sap:ActivityDesigner.Resources>
    <Grid>
        <ContentPresenter Style="{DynamicResource ExpandOrCollapsedStyle}" Content="{Binding}" />
    </Grid>
</sap:ActivityDesigner>
```

Vuuuuu!!!

![Sealed](/assets/images/2012/smiley-sealed.gif)

Biraz korkutucu bir içerik gibi görünebilir. Ama korkmayın. Tek tek açıklamaya çalışalım.

Herşeyden önce bileşenimiz içerisinde bazı Static Resource'lar tanımlandığı görülmektedir. dsInstanceMethods isimli ObjectDataProvider, InstanceMethodList isimli sınıfa bağlanmaktadır. Dolayısıyla XAML içerisinde yer alan bileşenler bu veri kaynağına bağlanıp InstanceMethod nesne örnekleri ile etkileşimde bulunabilirler. Örneğin cmbInstanceMethods isimli ComboBox kontrolü, ItemsSource özelliğine static bir veri kaynağı olarak bu ObjectDataProvider örneğini bağlamıştır. DisplayMemberPath özelliğine atanan Name değeri ise, InstanceMethod örnekleri içerisindeki Name özelliğini işaret etmekte olup ComboBox'un üzerinde nelerin gösterileceğini belirtmektedir. ComboBox üzerinde hareket edildikçe alt tarafta yer alan grdMethodParameters isimli DataGrid içeriğininde, ilgili metoda ait parametre listesi ile doldurulması beklenmektedir. Bu nedenle her iki bileşenin IsSychnronizedWithCurrentItem özelliği true değerine sahiptir. DataGrid bileşeninin ItemsSource özelliği de static veri kaynağı olan dsInstanceMethods'a bağlanmıştır. Ama!

Path özelliğinin değerine dikkat edelim. Parameters değeri aslında InstanceMethodList sınıfındaki özelliğin adıdır. Dolayısıyla ComboBox kontrolünde bir öğe seçildiğinde, buna bağlı Parameters özelliğinin karşılığı olan liste, DataGrid içerisine basılıyor olacaktır. Görüldüğü üzere tipik olarak bir WPF Data Binding işlevselliği söz konusudur. Bunun dışında kalan kısımlarda bileşenin Collapse veya Expand edilmesi hallerinde nasıl görüneceği ifade edilmiştir. Dikkat edilecek olursa iki adet DataTemplate elementi vardır. Bunlardan birisi Collapsed diğer ise Expanded olarak isimlendirilmiştir. Son satırlarda yer alan Grid elementi içerisindeki ContentPresenter'da buna uygun olacak şekilde bileşenin Collapsed veya Expanded olarak designer üzerinde gösterilebilmesini sağlamaktadır (Ne varki ben Collapsed hale bir türlü getirmeyi başaramadım. Yani örneğimizde şimdiden bir Bug'ımız olduğunu ifade etmek isterim ![Undecided](/assets/images/2012/smiley-undecided.gif))

Bileşenimizin XAML içeriğini bu şekilde oluşturmak yeterli değildir. Ayrıca Visual Studio Designer'ına söz konusu bileşeni bildirmemiz gerekmektedir. Bunun için ilk olarak Activity Designer sınıfının koda tarafını aşağıdaki hale getirmeliyiz.

```csharp
using System.Activities.Presentation.Metadata;
using System.ComponentModel;

namespace Azon.Workflow.Activity.Design
{
    public partial class InstanceMethodActivityDesigner
    {
        #region Constructors and Destructors

        public InstanceMethodActivityDesigner()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Public Methods

        public static void RegisterMetadata(AttributeTableBuilder builder)
        {
            builder.AddCustomAttributes(
                typeof(InstanceMethodActivity),
                new DesignerAttribute(typeof(InstanceMethodActivityDesigner)),
                new DescriptionAttribute("Instance Method Activity"));
        }

        #endregion
    }
}
```

Sınıf içerisindeki en önemli metod RegisterMetadata isimli static fonksiyondur. Bu metod içerisinde parametre olarak gelen Attribute tablosuna bazı bildirimlerde bulunularak yeni niteliklerin (Attribute) ilave edilmesi sağlanmaktadır. Hatta dilerseniz burada Component için bir Icon (16X16 boyutlarında bir PNG olabilir) dahi belirtebilirsiniz. Biz şimdilik bu detayı atlıyor olacağız.

Peki söz konusu static metod nerede çağırılacaktır?

![Undecided](/assets/images/2012/smiley-undecided.gif)

Bunun için Azon.Workflow.Activity.Design kütüphanesine IRegisterMetadata arayüzünü implemente eden bir sınıfın eklenmesi gerekmektedir. IRegisterMetadata arayüzünden gelen Register metodu içerisinde ise, InstanceMethodActivityDesigner sınıfına dahil edilmiş olan RegisterMetadata isimli static metod çağrısı gerçekleştirilmektedir.

```csharp
using System.Activities.Presentation.Metadata;
namespace Azon.Workflow.Activity.Design
{
    public sealed class ActivityLibraryMetadata
        : IRegisterMetadata
    {
        public void Register()
        {
            RegisterAll();
        }

        public static void RegisterAll()
        {
            var builder = new AttributeTableBuilder();
            InstanceMethodActivityDesigner.RegisterMetadata(builder);
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
}
```

Burada kullanılan sınıfın adının çok önemi yoktur. Nitekim Visual Studio IDE'si, kendi çalışma zamanı ortamında, ilgili Activity Designer kütüphanesinde IRegisterMetadata arayüzünü uygulamış olan bir tipe bakmaktadır. Tipik bir Plug-In tasarım mantığı olduğunu rahatlıkla ifade edebiliriz.

Artık bileşenimizi deneyebiliriz demek isterdim ama son olarak yapmamız gereken ufak bir işlem daha var. Adı Design kelimesi ile biten kütüphanenin dll çıktısının, NativeActivity bileşenlerini içeren kütüphanenin olduğu yere doğru yapılması gerekmektedir. Aşağıdaki şekilde görüldüğü gibi.

![wda_3.png](/assets/images/2012/wda_3.png)

Artık basit bir Workflow üzerinden bileşenimizi deneyebiliriz. Bileşenimiz otomatik olarak Toolbox sekmesinde görünecektir. İşte Visual Studio 2010 çalışma ortamına ait bir kaç örnek görüntü.

Sum metodu seçildiğinde

![wda_4.png](/assets/images/2012/wda_4.png)

CallSp metodu seçildiğinde

![wda_5.png](/assets/images/2012/wda_5.png)

Görüldüğü üzere bileşenimiz içerisinde Data Binding tekniklerini de kullanarak bir etkileşim gerçekleştirmeyi başardık. Şimdi bileşenimizi biraz daha geliştirmeyi deniyor olacağız. Buna göre ComboBox kontrolünde bir öğe seçildiğinde, Name alanının değerinin, o anki InstanceMethodActivity'ye ait Property'lerden MethodName alanında gösterilmesini sağlamaya çalışacağız. Bu bonus senaryoda işi zorlaştıran kısım şu;

ComboBox bileşeni içerisinde Binding sebebi ile InstanceMethodList sınıfına ait değerler taşınmaktadır. Bu değerler InstanceMethod türünden nesne örnekleridir aslında. InstanceMethodActivity bileşeninin, MethodName özelliği ise string tipindendir. Dolayısıyla ComboBox kontrolünün SelectedValue özelliği içerisinde XAML tarafında bildirilecek şekilde özel bir Convert işleminin uygulanması gerekmektedir. Bu amaçla öncelikli olarak bir Converter tipini Azon.Workflow.Activity kütüphanesine aşağıdaki gibi ilave edelim.

![wda_6.png](/assets/images/2012/wda_6.png)

```csharp
namespace Azon.Workflow.Activity
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class InstanceMethodToMethodNameConverter
        :IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null; //BURASI SİZE ÖDEV OLSUN
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            InstanceMethod instanceMethod = (InstanceMethod)value;
            return instanceMethod.Name;
        }
    }
}
```

InstanceMethodToMethodNameConverter tipi, System.Windows.Data isim alanında (ki PresentationFramework.dll assembly'ının projede referans edilmesi gerekmektedir) yer alan IValueConverter arayüzünü (Inteface) uygulamaktadır. Buna göre TwoWay Binding'i destekleyecek şekilde Convert ve ConvertBack metodlarının implemantasyonunu istemektedir. Convert metodu, Properties penceresinden girilen değere göre ComboBox içerisinde ilgili öğeye gidilmesini sağlamaktadır. ConvertBack metodu ise tam tersi işlevi üstlenmekte olup, ComboBox'ta seçilen InstanceMethod nesne örneğinin Name özelliğinin değerini Properties penceresindeki MethodName alanına basmaktadır. Tabi söz konusu tipin yazılması yeterli değildir. Bu Converter tipinin XAML tarafında da dekleratif olarak bildirilmesi ve ComboBox bileşeni ile ilişkilendirilmesi gerekmektedir. Bunun için InstanceMethodActivityDesigner.xaml içeriğini aşağıdaki gibi güncellememiz yeterli olacaktır.

```xml
<sap:ActivityDesigner x:Class="Azon.Workflow.Activity.Design.InstanceMethodActivityDesigner"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" xmlns:s="clr-namespace:System;assembly=mscorlib" 
    xmlns:sap="clr-namespace:System.Activities.Presentation;assembly=System.Activities.Presentation"
    xmlns:sapv="clr-namespace:System.Activities.Presentation.View;assembly=System.Activities.Presentation"
    xmlns:Model="clr-namespace:System.Activities.Presentation.Model;assembly=System.Activities.Presentation"
    xmlns:sapc="clr-namespace:System.Activities.Presentation.Converters;assembly=System.Activities.Presentation"
    xmlns:activity="clr-namespace:Azon.Workflow.Activity;assembly=Azon.Workflow.Activity"
    mc:Ignorable="d" xmlns:d="http://schemas.microsoft.com/expression/blend/2008" xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006">
    <sap:ActivityDesigner.Resources>
        <ResourceDictionary x:Uid="ResourceDictionary_0">
            <sapc:ModelToObjectValueConverter x:Key="ModelToObjectValueConverter" />
            <activity:InstanceMethodToMethodNameConverter x:Key="MethodToMethodNameConverter"/>
            <ObjectDataProvider x:Key="dsInstanceMethods" ObjectType="{x:Type activity:InstanceMethodList}">
            </ObjectDataProvider>

            <DataTemplate x:Key="Collapsed">
                <StackPanel Orientation="Horizontal">
                    <TextBlock VerticalAlignment="Center" Margin="5" Text="Method Caller" />
                </StackPanel>
            </DataTemplate>

            <DataTemplate x:Key="Expanded">
                <Grid>
                    <Grid.RowDefinitions>
                        <RowDefinition/>
                        <RowDefinition/>
                        <RowDefinition/>
                        <RowDefinition/>
                    </Grid.RowDefinitions>
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition/>
                    </Grid.ColumnDefinitions>
                    <TextBlock Text="Metodlar" Grid.Row="0"/>
                    <ComboBox x:Name="cmbInstanceMethods" Grid.Row="1" ItemsSource="{Binding Source={StaticResource dsInstanceMethods}}" DisplayMemberPath="Name" IsSynchronizedWithCurrentItem="True" SelectedValue="{Binding Path=ModelItem.MethodName, Mode=TwoWay, Converter={StaticResource MethodToMethodNameConverter}}" />
                    <TextBlock Text="Metod Parametreleri" Grid.Row="2"/>
                    <DataGrid x:Name="grdMethodParameters" Grid.Row="3" IsSynchronizedWithCurrentItem="True" ItemsSource="{Binding Source={StaticResource dsInstanceMethods}, Path=Parameters}"/>
                </Grid>
            </DataTemplate>
            <Style x:Key="ExpandOrCollapsedStyle" TargetType="{x:Type ContentPresenter}">
                <Setter Property="ContentTemplate" Value="{DynamicResource Expanded}" />
                <Style.Triggers>
                    <DataTrigger Binding="{Binding Path=ShowExpanded}" Value="true">
                        <Setter Property="ContentTemplate" Value="{DynamicResource Collapsed}" />
                    </DataTrigger>
                </Style.Triggers>
            </Style>

        </ResourceDictionary>
    </sap:ActivityDesigner.Resources>
    <Grid>
        <ContentPresenter Style="{DynamicResource ExpandOrCollapsedStyle}" Content="{Binding}" />
    </Grid>
</sap:ActivityDesigner>
```

Buna göre ComboBox kontrolünde bir Metod adı seçilirse bu Properties penceresine de bu isim yansıyacaktır. Böylece developer'ın işi biraz daha kolaylaştırılmış olmaktadır.

![wda_7.png](/assets/images/2012/wda_7.png)

Şimdi olayı biraz daha renklendireceğiz. Örneğin Visual Studio Designer'ı üzerinde çalışırken, Activity bileşenlerine ait event methodları kullanmak istediğinizi ve hatta bu event metodlar içerisinde, diğer kontrollerin içeriklerine ulaşmak istediğimizi düşünelim. Bu senaryoyu irdelemek için InstanceMethodActivityDesigner.xaml içeriğine bir Button kontrolü ekleyerek ilerleyebiliriz.

```xml
<DataTemplate x:Key="Expanded">
	<Grid>
		<Grid.RowDefinitions>
			<RowDefinition/>
			<RowDefinition/>
			<RowDefinition/>
			<RowDefinition/>
			<RowDefinition/>
		</Grid.RowDefinitions>
		<Grid.ColumnDefinitions>
			<ColumnDefinition/>
		</Grid.ColumnDefinitions>
		<TextBlock Text="Metodlar" Grid.Row="0"/>
		<ComboBox x:Name="cmbInstanceMethods" Grid.Row="1" ItemsSource="{Binding Source={StaticResource dsInstanceMethods}}" DisplayMemberPath="Name" IsSynchronizedWithCurrentItem="True" SelectedValue="{Binding Path=ModelItem.MethodName, Mode=TwoWay, Converter={StaticResource MethodToMethodNameConverter}}" />
		<TextBlock Text="Metod Parametreleri" Grid.Row="2"/>
		<DataGrid x:Name="grdMethodParameters" Grid.Row="3" IsSynchronizedWithCurrentItem="True" ItemsSource="{Binding Source={StaticResource dsInstanceMethods}, Path=Parameters}"/>
		<Button x:Name="btnCatchParameters" Click="btnCatchParameters_Click" Content="Parametreleri Çek" Grid.Row="4">
		</Button>
	</Grid>
</DataTemplate>
```

btnCatchParameters isimli Button kontrolünün Click olay metodunun yüklendiği görülmektedir. Bu olay metodu çok doğal olarak InstanceMethodActivityDesigner.cs içerisine açılıyor olacaktır. Olay metodu içeriğini aşağıdaki kod parçasında görüldüğü gibi geliştirebiliriz.

```csharp
private void btnCatchParameters_Click(object sender, System.Windows.RoutedEventArgs e)
{
    Grid parent=((Grid)((Button)e.Source).Parent);
    foreach (UIElement control in parent.Children)
    {
        DataGrid grid=control as DataGrid;
        if(grid!=null)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in grid.ItemsSource)
            {
                builder.AppendLine(item.ToString());
            }
            MessageBox.Show(builder.ToString(),"Parametreler");
        }
    }                          
}
```

Olay metodu içerisindeki felsefe oldukça basittir. Button kontrolü aslında bir Grid içerisinde yer almaktadır ve DataGrid bileşeni de aynı seviyede (Level) durmakta olan bir elementtir. Dolayısıyla Button bileşeninin Parent elementine (Container da diyebiliriz) çıkıp, tüm alt kontrolleri dolaşabilir ve Grid tipinde olana vardığımızda da ItemsSource özelliğine ait koleksiyon içeriğini ele alabiliriz. Kulağımızı farklı bir şekilde tuttuğumuzu ifade edebiliriz aslında ama şu anda elimizden en iyi çözüm bu. Böylece Visual Studio Designer'ı içerisindeyken, DataGrid elementlerine ve seçili olan metodun parametre listesine ulaşmamız mümkün olacaktır. Aynen aşağıdaki şekilde görüldüğü gibi.

![wda_8.png](/assets/images/2012/wda_8.png)

Görüldüğü üzere Custom Activity geliştirmek kolay olsa da, bu bileşeni Designer desteğine sahip olacak şekilde genişletmek bir kaç ipucu içeren ve dikkat edilmesi gereken bir süreci gerektirmektedir. Geliştirmiş olduğumuz örnekte bazı eksik kısımlar da bulunmaktadır. Örneğin XAML tarafında dekleratif olarak Event bazlı etkileşimler çok fazla ele alınmamıştır.(Bir veritabanı bağlantısını seçtiren ve hatta design tarafında bir SQL sorgusunu çalıştırtıp sonuçları bir DataGrid kontrolüne basan bir Activity Designer yazmaya çalıştığınızı hayal edin. Üstelik Connection'ı tanımladığınızda Test'de edebilmelisiniz vs ![Wink](/assets/images/2012/smiley-wink.gif)) Bu konuda detaylı ve derinlemesine araştırmalarıma devam ediyorum. Yeni bilgiler edindikçe sizinle paylaşmaya gayret ediyor olacağım. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[WritingDesignerActivityV2.zip (155,91 kb)](/assets/files/2012/WritingDesignerActivityV2.zip)