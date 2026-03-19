---
layout: post
title: "WPF, Xaml, Baml ve Dahası"
date: 2007-03-21 12:00:00 +0300
description: "Windows Presentation Foundation (WPF), windows tabanlı uygulama geliştirme modeline yeni ve çok farklı bir soluk getirmiştir. Daha çok web uygulama geliştirme sistematiğini andıran bu yeni model, .Net Framework 2.0 ile birlikte yapabildiklerimizi tek bir çatı altında toplamıştır. Hatırlayacağınız üzere, benzer bir yaklaşım modelinin Windows Communication Foundation (WCF) içerisinde de yer aldığından daha önceki makalelerimizde bahsetmiştik. Öncelikle bu noktaya nasıl gelindiğini vurgulamakta fayda var."
categories:
  - csharp
tags:
  - csharp
  - xml
  - dotnet
  - aspnet
  - wcf
  - wpf
  - windows-forms
  - xaml
  - http
  - delegates
  - visual-studio
---
# WPF, Xaml, Baml ve Dahası
Değerli Okurlarım Merhabalar,

Windows Presentation Foundation (WPF), windows tabanlı uygulama geliştirme modeline yeni ve çok farklı bir soluk getirmiştir. Daha çok web uygulama geliştirme sistematiğini andıran bu yeni model,.Net Framework 2.0 ile birlikte yapabildiklerimizi tek bir çatı altında toplamıştır. Hatırlayacağınız üzere, benzer bir yaklaşım modelinin Windows Communication Foundation (WCF) içerisinde de yer aldığından daha önceki makalelerimizde bahsetmiştik. Öncelikle bu noktaya nasıl gelindiğini vurgulamakta fayda var..Net Framework 2.0 açısından baktığımızda windows uygulamalarını geliştirirken yaşanan bazı sıkıntılar vardır.

Örneğin iki boyutlu grafik işlemleri için System.Drawing.dll kütüphanesinin etkin bir şekilde bilinmesi gerekir. Ancak windows uygulamalarımıza üç boyutlu grafik modeller katmak istediğimizde,.Net Framework'ün dışına çıkmalı ve yönetimli (Managed) DirectX API'sini öğrenmeliyizdir. Bunlara ek bir diğer örnek olarakta, akıcı görsel öğeleri verebiliriz. Stream Video'ları işleyebilmek için, en azından Media Player API'sinin yönetimli (Managed) halinin ele alınması gerekmektedir. Peki Windows Presentation Foundation bize bu anlamda nasıl bir getiri sağlamaktadır. WPF, yukarıda bahsetmiş olduğumuz özellikleri.Net Framework 3.0 içerisinde tek bir çatı altında ele alan bir model sunmaktadır. Böylece,.Net Framework 2.0 içerisinde ayrı ayrı ele almak zorunda kaldığımız API ve benzeri parçaları,.Net Framework 3.0 içerisinde tek ve benzersiz (unique) bir nesne modeli içerisinde ele alma şansına sahib oluruz. Temel olarak kazanımlarımızı aşağıdaki tablo ile özetleyebiliriz.

Kullanılmak İstenen Fonksiyonellik.Net 2.0 Yaklaşımı.Net 3.0 Yaklaşımı

İki boyutlu grafik desteği (2D Graphics)
GDI+
WPF

Üç boyutlu grafik desteği (3D Graphics)
Managed Directx API

Akıcı görüntü desteği (Streaming Video)
Media Player API

Bileşen ve Windows Form Desteği
Windows Forms

Windows Presentation Foundation (WPF), Windows XP, Windows Server 2003 ve Vista sistemlerinde çalışabilmektedir. Ancak en iyi performası Vista üzerinde göstermektedir.

> Vista'nın var olan 3D ve animasyon desteği nedeni ile WPF uygulamalarını, Windows XP veya Windows Server 2003 sistemlerindekine göre daha performanslı çalıştırdığı, teorik olarak belirtilmektedir.

Windows Presentation Foundation ile Windows tabanlı uygulamları geliştirirken karşılaştığımız pek çok yeni kavramda vardır. Bunlar birisi de XAML dir. XAML, (eXtensible Application Markup Language- yada bir başka deyişle genişletilebilir uygulama işaretleme dili), özellikle windows tabanlı uygulamalar için geliştirilmiş yeni bir işaretleme dilidir. Bu işaretleme dili sayesinde.Net Framework içerisinde yer alan tipleri birer element olarak ifade edebilme şansına sahip oluruz.

> XAML,.Net Framework içerisinde Abstract olmayan ve varsayılan yapıcı (default constructor) metoda sahip olan tüm tipleri işaretlemeler (markup) ile temsil edebilmektedir.

Bu gelişmeler nedeniyle windows uygulamalarınında Asp.Net tarzı web uygulamalarına son derece benzediğini düşünebiliriz. Öyleki, sunu katmanının görsel bileşenlerinin ele alındığı taraf ile kod bölümü tamamen birbirlerinden ayrılabilmektedir. XAML'in en büyük özelliği,.Net içerisindeki abstract olmayan varsayılan yapıcı metoda sahip tipleri işaret edebiliyor olmasıdır. Bu durum aşağıdaki şekilde temsil edilmeye çalışılmıştır.

![mk196_1.gif](/assets/images/2007/mk196_1.gif)

Dikkat ederseniz,.Net tipi (type) XAML tarafında bir element olarak temsil edilirken, özellik (property) veya olay (event) gibi üyeler birer nitelik (attribute) olarak ifade edilebilmektedir. Biz bu makalemizde sadece XAML'i kullanarak Visual Studio 2005 desteği olmadan, basit bir Windows Presentation Foundation uygulamasını nasıl geliştirebileceğimizi görmeye ve XAML'in arka tarafta bıraktığı parmak izlerini analiz etmeye çalışacağız.

İlk olarak var olan Windows programlama modelimizi hatırlamakta fayda var..Net Framework 3.0 öncesi geliştirdiğimiz Windows uygulamaları exe uzantılı Protable Executable tipinde Assembly dosyalarıdır. Exe uzantılı olmaları nedeni ile mutlaka bir giriş noktasına (entry point) sahiptirler. Bu giriş noktasının Main metodu olduğunu hepimiz biliyoruz. Diğer taraftan bir windows uygulaması en azından bir adet görsel form içerir. Bu form, System.Windows.Form sınıfından türemiştir ve çalışma zamanında örneklenip ekrana çıkartılabilmesi için Main metodu içerisinde Application sınıfına ait static Run metodunun çalıştırılması gerekmektedir. Main metodunu içeren tipte aslında static bir sınıftır. Bu durumu hatırlamak ve zihinlerimizi tazelemek için aşağıdaki sınıf diagramından faydalanabiliriz.

![mk196_2.gif](/assets/images/2007/mk196_2.gif)

Main metodumuzun içeriği ise başlangıçta aşağıdakine benzer bir şekilde olacaktır.

```csharp
static void Main()
{
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);
    Application.Run(new Form1());
}
```

Kurallar WPF içerisindede değişmemiştir. Ancak uygulama tarzı oldukça farklı bir hal almıştır:) Dilerseniz XAML kullanarak bir windows uygulamasını nasıl yazabileceğimizi adım adım inceleyelim. İlk örneğimizde herhangibir kod dosyası kullanmayacağız. Nitekim WPF, aynen web tabanlı uygulama mimarisinde olduğu gibi inline coding ve code-behind modellerini desteklemektedir. Kurallarımızı düşündüğümüzde bize formu temsil edecek bir eleman ve bu formu ekrana basacak olan başka bir eleman gerektiği ortadadır. İşte bu elemanları birer XAML dosyası şeklinde oluşturacağız. İlk olarak formumuzu aşağıdaki gibi hazırlayıp Giris.xaml isimli bir dosya olarak sistemimize kaydedelim.

```xml
<Window x:Class="XamlGiris.Giris" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Xaml Giris" Width="200" Height="200" Loaded="XamlGiris_Loaded">
<Grid>
    <ComboBox Name="cmbSehirler" Height="26" VerticalAlignment="Top"></ComboBox>
    <Button Name="MerhabaDe" Width="100" Height="50" Click ="btnMerhabaDe_Clicked">Merhaba De</Button>

    <x:Code>
        <![CDATA[
            protected void XamlGiris_Loaded(object sender,EventArgs e) 
            {
                cmbSehirler.Items.Add("Istanbul");
                cmbSehirler.Items.Add("Ankara");
                cmbSehirler.Items.Add("Izmir");
            }

            protected void btnMerhabaDe_Clicked(object sender, RoutedEventArgs e)
            {
                string adim=cmbSehirler.Text;
                MessageBox.Show("Merhaba "+adim);
            }
        ]]>
    </x:Code>
</Grid>
</Window>
```

Herşeyden önce karşımızda Xml türevli bir dil vardır. Bu nedenle Xml için geçerli olan kurallar XAML içinde söz konusudur. Örneğin büyük küçük harf duyarlılığı (case sensitive) bunlardan birisidir. WPF açısından baktığımızda ise artık pencerelerimizi Form sınıfı ile değil Window sınıfı ile temsil ettiğimizi söyleyebiliriz. Diğer taraftan çalışma zamanında örneklenecek olan tipin tanımı Window elementi içerisindeki x:class (x isimalanı altında) niteliği ile yapılmaktadır. Bu tanımlamada dikkat ederseniz XamlGiris, tipin içerisinde yer aldığı isim alanını (namespace) temsil etmektedir.

Makalemizin giriş kısmındada bahsettiğimiz gibi, elementlerimizin.Net tarafındaki özellik (property) veya olayları (events) XAML içerisinde elementlere ait nitelikler (attributes) yardımıyla ele alınmaktadır. Söz gelimi, Button nesne örneğimiz için bir Click olayını yüklemek istediğimizde tek yapmamız gereken aynı Asp.Net 2.0' da olduğu gibi niteliğe metod adını atamak olacaktır. Örnek XAML dosyamız ekrana basıldığında 200' e 200 piksel boyutlarında bir pencere oluşturulmasını sağlayacaktır. Aynı zamanda bu ekran üzerinde içerisinde Istanbul, Ankara, Izmir değerleri olan bir ComboBox ve birde "Merhaba Istanbul" benzeri bir metni mesaj kutusu ile gösterebilecek bir Button kontrolü yer almaktadır. Dikkat ederseniz Button ve ComboBox tipleri burada birer element olarak işaretlenmektedir. Örneğimizde inline coding tekniğini kullandığımız için, kod bloklarımızının tamamını isimli element içerisine alıyoruz. Özellikle XAML'in buradaki kod satırlarını işaretleme dili gibi yorumlamasını engellemek içinde CDATA sekmesi içerisine alıyoruz.

> Bir Windows Presentation Foundation (WPF) uygulamasını sadece geçerli XAML dosyalarından (ister inline coding ister code-behind olsun) oluşturabilir ve geçerli bir winexe assembly'ı haline getirebiliriz.

Gelelim bu pencereyi örnekleyip ekrana bastırılmasını sağlayacak olan elemanımıza. WPF haricinde varsayılan olarak Program.cs adı altında ifade edebileceğimiz bu dosyayı, geleneği bozmamak adına Program.xaml olarak adlandırıyor ve aşağıdaki gibi geliştiriyoruz.

```xml
<Application x:Class="XamlGiris.Program" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
StartupUri="Giris.xaml">
</Application>
```

Görüldüğü üzere Program.xaml dosyamız çok daha basittir. Giris.xaml ile karşılaştırıldığında belkide en önemli fark Application isimli bir elementin oluşudur. Uygulamanın giriş noktasınıda taşıyan bu tipin çalışma zamanındaki karşılığı ise Class niteliğinde (attribute) belirtilen XamlGiris.Program isimli sınıfın bir örneği olacaktır. Application elementi içerisinde yer alan StartupUri niteliği, uygulama çalıştırıldığına ekran yüklenecek olan window tipini tanımlayan xaml dosyasını işaret etmektedir. Dikkat ederseniz Application içerisinde tanımlanmış herhangibir Main metodu görülmemektedir. Bu işi üstlenen kişi StartupUri niteliğidir. Peki ama nasıl?

Yazmış olduğumuz XAML dosyaları tek başlarına hiç bir anlam ifade etmez. Özellikle C# derleyicisi XAML kodlarını anlamayacağı için derleyemez. Oysaki bu XAML kaynaklarının ve kodlarının bir Winexe Assembly içerisinde ele alınmaları şarttır. Bunun için bir ön işlemden geçirilmeleri, birer sınıf (Class) haline getirilmeleri, hatta XAML'deki bileşenlerin konumları gibi niteliklerin tutulacağı kaynakların (resources) binary hale getirilip ilgili assembly içerisine atılmaları gerekmektedir. İşte bu noktada MSBuild.exe (Microsoft Build Engine) programı devreye girer.

> MSBuild.exe programını varsayılan olarak, D:\WINDOWS\Microsoft.NET\Framework\v2.0.50727 klasörü altında bulabilirsiniz.

Ne varki MSBuild proje dosyası tabanlı çalışan bir sistemdir. Bu sebepten, öncelikli olarak csproj uzantılı ve xml tabanlı bir proje dosyası aşağıdaki gibi oluşturulmalıdır.

```xml
<Project DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
<PropertyGroup>
    <RootNamespace>XamlGiris</RootNamespace>
    <AssemblyName>XamlGiris</AssemblyName>
    <OutputType>winexe</OutputType>
</PropertyGroup>
<ItemGroup>
    <Reference Include="System" />
    <Reference Include="WindowsBase" />
    <Reference Include="PresentationCore" />
    <Reference Include="PresentationFramework" />
</ItemGroup>
<ItemGroup>
    <ApplicationDefinition Include="Program.xaml" />
    <Page Include="Giris.xaml" />
</ItemGroup>
<Import Project="$(MSBuildBinPath)\Microsoft.CSharp.targets" />
<Import Project="$(MSBuildBinPath)\Microsoft.WinFX.targets" />
</Project>
```

Proje dosyamızda PropertyGroup isim element içerisinde, uygulamanın ana isim alanı (namespace) RootNameSpace elementi ile belirtilmektedir. Her assembly'ın bir adı vardır. Bu adı belirtmek için proje dosyası içerisinde AssemblyName elementi kullanılmıştır. OutputType elementi, üretilecek olan assembly tipini belirtmektedir ki burada winexe çıktının Portable Executable bir windows assembly'ı olduğunu göstermektedir. İlk ItemGroup boğumu altında, program içerisinde referans edilen temel assembly bilgilerine yer verilmiştir. İkinci ItemGroup boğumunda, assembly'ın static program sınıfı rolünü üstlenen tipi temsil eden Program.xaml dosyası ApplicationDefinition elementi ile tanımlanır. Page elementi ise, uygulamanın içerdiği pencereleri tanımlayan XAML dosyalarını belirtmek için kullanılmaktadır. Örneğimizde bu dosya Giris.xaml'dir. Son olarak Import elementleri ile proje içerisindeki kaynaklardan üretilecek çıktılara ait bilgiler verilir. Öyleki MSBuild.exe, XAML dosyalarımızdan otomatik olarak cs uzantılı kaynak dosyalar üretecektir (AutoGenerated C# Source Files). Benzer şekilde XAML dosyalarından faydalanılarak yine otomatik olarak üretilecek bir BAML (Binary Application Markup Language) dosyasıda söz konusudur.

> BAML (Binary Application Markup Language) kaynak XAML dosyasında tanımlanan nesne hiyerarşisinin ve özelliklerinin ikili (Binary) karşılığıdır. İkili olması nedeni ile insan gözüyle okunabilir değildir. Baml, çalışma zamanında görsel bileşen ve özelliklerin assembly içerisinden çok daha hızlı bir şekilde yüklenmesini sağlamaktadır.

Bu dosya, temel olarak XAML elementlerinin ekran yerleşimleri ile ilişkili bilgilerin assembly içerisine resource olarak dahil edilebilecek ikili düzendeki karşılıkları olarak düşünülebilir. Temel olarak inşa (build) ve derleme (compile) aşamaları aşağıdaki şekilde olduğu gibi düşünülebilir.

![mk196_3.gif](/assets/images/2007/mk196_3.gif)

Dikkat ederseniz MSBuild.exe ile üretilen dosyalar sonrasında, C# derleyicisi sürece dahil olmakta ve otomatik olarak türetilen *.g.cs,.baml ve *.g.resources dosyalarından faydalanarak winexe tipinde bir Assembly'ın üretilmesini sağlamaktadır. Şimdi gelin MSBuild.exe aracımızı kullanalım ve nasıl bir sonuç elde edeceğimize bakalım. Bu amaçla, MSBuild.exe aracını Visual Studio 2005 komut penceresinden (Command Prompt) aşağıdaki gibi çalıştırmamız yeterli olacaktır.

![mk196_5.gif](/assets/images/2007/mk196_5.gif)

Eğer kodlarda bir hata yoksa klasör yapımız aşağıdakine benzer şekilde olacaktır.

![mk196_6.gif](/assets/images/2007/mk196_6.gif)

Dikkat ederseniz bildiğimiz iki adet klasör oluşturulmuştur. Bunlardan bin klasörü içerisinde üretilen winexe assembly'ı vardır. obj klasörü içerisinde yer alan debug alt klasöründe ise az önce bahsettiğimiz *.g.cs,.baml ve *.resources dosyaları yer almaktadır.

![mk196_7.gif](/assets/images/2007/mk196_7.gif)

Exe dosyamızı çalıştırdığımızda aşağıdakine benzer bir görüntü elde ederiz.

![mk196_8.gif](/assets/images/2007/mk196_8.gif)

Elbetteki burada şunu söylemekte fayda var. XAML içerisinde kullanabileceğimiz pek çok Layout var. Biz örneğimizde Grid kullandığımız için kontrollerimiz pencere üzerine buna uygun olacak şekilde yerleştirilmiştir. Şimdilik amacımız görsel tarafı kavramak olmadığından makalemize üretilen assembly'ın içeriğini inceleyerek devam edebiliriz. İlk olarak üretilen *.g.cs uzantılı C# dosyalarına bakmakta fayda var. Aşağıda, XamlGiris.g.cs dosyasının kısaltılmış içeriği yer almaktadır.

```csharp
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace XamlGiris 
{
    public partial class Program : System.Windows.Application 
    {
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() 
        {
            #line 4 "..\..\Program.xaml"
            this.StartupUri = new System.Uri("Giris.xaml", System.UriKind.Relative);

            #line default
            #line hidden    
        }
        [System.STAThreadAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public static void Main() 
        {
            XamlGiris.Program app = new XamlGiris.Program();
            app.InitializeComponent();
            app.Run();
        }
    }
}
```

İşte aradığımız Main metodu. Dikkat ederseniz üretilen XamlGiris.g.cs dosyası içerisinde bir Main metodu hatta InitializeComponent isimli başka bir metod daha yer almaktadır. StartupUri özelliğine Uri tipinden bir nesne örneği atandığını ve Giris.xaml dosyasını işaret edecek şekilde oluşturulduğuna dikkat edelim. Böylece programın giriş noktasının C# derleyicisi tarafından anlaşılabilecek hali üretilmiş olmaktadır. Gelelim, Giris.g.cs dosyasının içeriğine.

```csharp
using System;
// Burada isim alanlarına ait tanımalamaları vardır.
namespace XamlGiris 
{
    public partial class Giris : System.Windows.Window, System.Windows.Markup.IComponentConnector 
    {
        internal System.Windows.Controls.ComboBox cmbSehirler;
        internal System.Windows.Controls.Button MerhabaDe;
        private bool _contentLoaded;

        protected void XamlGiris_Loaded(object sender,EventArgs e) 
        {
            cmbSehirler.Items.Add("Istanbul");
            cmbSehirler.Items.Add("Ankara");
            cmbSehirler.Items.Add("Izmir");
        }

        protected void btnMerhabaDe_Clicked(object sender, RoutedEventArgs e)
        {
            string adim=cmbSehirler.Text;
            MessageBox.Show("Merhaba "+adim);
        }

        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() 
        {
            if (_contentLoaded) 
            {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/XamlGiris;component/giris.xaml", System.UriKind.Relative);
            System.Windows.Application.LoadComponent(this, resourceLocater);
        }

        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) 
        {
            switch (connectionId)
            {
                case 1:
                    ((XamlGiris.Giris)(target)).Loaded += new System.Windows.RoutedEventHandler(this.XamlGiris_Loaded);                    
                    return;
                case 2:
                    this.cmbSehirler = ((System.Windows.Controls.ComboBox)(target));
                    return;
                case 3:
                    this.MerhabaDe = ((System.Windows.Controls.Button)(target));
                    this.MerhabaDe.Click += new System.Windows.RoutedEventHandler(this.btnMerhabaDe_Clicked);
                    return;
            }
            this._contentLoaded = true;
        }
    }
}
```

İlk başta bildiğimiz windows tabanlı uygulama modelinden çok farklı bir sınıf ile karşı karşıya olduğumuzu ifade edebiliriz. Ancak biraz dikkatli baktığımızda, sınıfın XAML kaynak dosyası içerisinde yer alan görsel bileşenleri tanımladığını, bunlara ait olay metodlarını uygun temsilciler (delegate) yardımıyla yüklediğini, olaylara ilişkin kod bilgilerini içerdiğini görebiliriz. Üretilen bu cs uzantılı dosyalar, artık C# derleyicisi (csc.exe) tarafından yorumlanabilirler. Zaten MSBuild.exe'de gerekli üretim işlemlerinden sonra süreci csc.exe'ye devredecektir. Üretilen Assembly'ın içeriğini herhangibir.Net Decompiler aracı ile açacak olursak Resources olarak giris.baml isimli bir dosyanın atıldığını görürüz. Dikkatinizi çekerim XAML değil BAML diyoruz.

![mk196_4.gif](/assets/images/2007/mk196_4.gif)

Şu ana kadar geliştirdiğimiz WPF örneğinde, kod parçalarını XAML dökümanları içerisine Inline-Coding tekniğine göre aldık. Microsoft'un ve profesyonel yazılım mimarlarının önerisi, kod tarafını XAML tarafından ayrımak yönündedir. Bu zaten web tarafındanda bildiğimiz en optimum yoldur. Bu öneriyi dikkate alıp örneğimizi code-behind modeline göre tasarlamak istersek, XAML dosyaları içerisinde yer alan kodları xaml.cs uzantılı dosyalar içerisine koymalıyız. Bununla birlikte proje dosyası içerisinde de Compile isimli elementleri kullanıp code-behind dosyalarının ne olacağını bildirmeliyiz. Buna göre Giris.xaml ve Program.Xaml içerisindeki tüm kod parçalarını (CDATA sekmesi içerisinde yer alanlar) ayrı cs dosyaları içerisine aşağıdaki gibi almamız yeterli olacaktır.

Giris.xaml.cs

```csharp
using System;
using System.Windows;
using System.Windows.Controls;

namespace XamlGiris
{
    public partial class Giris: Window
    {
        public Giris()
        {
            InitializeComponent();
        }

        protected void XamlGiris_Loaded(object sender,EventArgs e) 
        {
            cmbSehirler.Items.Add("Istanbul");
            cmbSehirler.Items.Add("Ankara");
            cmbSehirler.Items.Add("Izmir");
        }

        protected void btnMerhabaDe_Clicked(object sender, RoutedEventArgs e)
        {
            string adim=cmbSehirler.Text;
            MessageBox.Show("Merhaba "+adim);
        }
    }
}
```

Dikkat etmemiz gereken nokta, sınıfımızın Window sınıfından türemesi gerektiğidir. Bununla birlikte pencere üzerindeki bileşenlerin yüklenebilmesi için yapıcı metod (constructor) içerisinde InitializeComponent fonksiyonunun çağırılması gerekir.

Program.xaml.cs

```csharp
using System;
using System.Windows;
using System.Windows.Controls;

namespace XamlGiris
{
    public partial class Program: Application
    {
    }
}
```

Program isimli sınıfımız içinde önemli olan nokta Application sınıfından türemesi gerektiğidir. Bu işlemlerin ardında tek yapmamız gereken proje dosyasını aşağıdaki gibi güncellemektir.

```xml
<Project DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
<PropertyGroup>
    <RootNamespace>XamlGiris</RootNamespace>
    <AssemblyName>XamlGiris</AssemblyName>
    <OutputType>winexe</OutputType>
</PropertyGroup>
<ItemGroup>
    <Reference Include="System" />
    <Reference Include="WindowsBase" />
    <Reference Include="PresentationCore" />
    <Reference Include="PresentationFramework" />
</ItemGroup>
<ItemGroup>
    <ApplicationDefinition Include="Program.xaml" />
        <Compile Include = "Giris.xaml.cs" />
        <Compile Include = "Program.xaml.cs" />
    <Page Include="Giris.xaml" />
</ItemGroup>
    <Import Project="$(MSBuildBinPath)\Microsoft.CSharp.targets" />
    <Import Project="$(MSBuildBinPath)\Microsoft.WinFX.targets" />
</Project>
```

Dikkat ederseniz Compile elementleri içerisinde code-behind dosyaları tek tek belirtilmektedir. Bu sayede MSBuild.exe aracı, inşa işlemi sırasında hangi XAML dosyası ile hangi cs dosyasını çarpıştıracağını anlayabilecektir. Artık MSBuild aracımızı kullanabilir ve winexe assembly'ının üretilmesini sağlayabiliriz. Uygulamamız bir önceki örneğimizde olduğu gibi aynı şekilde çalışacaktır.

Böylece geldik bir makalemizin daha sonuna. Bu makalemizde, XAML'in Windows Presentation Foundation (WPF) içerisindeki yerini anlayabilmek için bir giriş yapmaya çalıştık. XAML kullanarak bir WPF Assembly'ının Visual Studio'ya başvurmadan nasıl geliştirilebileciğini, bu geliştirme işleminin aşamalarını ve aslında arka tarafta meydana gelen değişiklikleri görmeyi amaçladık. Ek olarak, Xaml ve Baml kavramlarının ne anlama geldiğini anlamaya çalıştık. Her zaman olduğu gibi DeCompiler araçları burada işimize oldukça yaradı. İlerleyen makalelerimizde Windows Presentation Foundation (WPF) uygulamalarına daha derinlemesine bakmaya çalışacağız. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.