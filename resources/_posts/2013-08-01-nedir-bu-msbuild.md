---
layout: post
title: "Nedir Bu MSBuild?"
date: 2013-08-01 07:47:00 +0300
description: "Microsoft Build Engine aslında başlı başına bir platformdur. Kısaca MSBuild olarak anılmaktadır ve bir uygulamanın inşa edilmesi noktasında devreye giren XML(eXtensible Markup Language) tabanlı bir Script bütününü esas alır. Kısacası uygulamanın inşa edilmesi sırasındaki aşamalar XML tabanlı bir akış olarak ifade edilebilmektedir. MSBuild platformunun en önemli özelliği ise, inşa sürecinde Visual Studio gibi bir araca ihtiyaç duymuyor oluşudur..."
categories:
  - dotnet-framework-4-0
  - dotnet-framework-4-5
  - visual-studio
tags:
  - dotnet-framework-4-0
  - dotnet-framework-4-5
  - visual-studio
  - xml
  - dotnet
  - linq
  - http
  - performance
---
# Nedir Bu MSBuild?
[![msbuild_9](/assets/images/2013/msbuild_9_thumb.jpg)](/assets/images/2013/msbuild_9.jpg)[Orjinal Yazım Tarihi – 25/02/2013]

Merhaba Arkadaşlar,

Yıllar öncesinde bir kaç seneliğine de olsa saygın bir eğitim kurumunda eğitmen olarak görev alma şansını yakalamıştım. Özellikle C#’ ın öğretilmeye çalışıldığı başlangıç niteliğindeki seanslarda dilin temel özelliklerini anlatırken, tüm dış çevre ile olan bağlantıyı kesip, sadece anahtar kelime (keyword), ifade ve materyale odaklanmaya çalışırdık. Bu sebepten genellikle ilk örneklerimiz ve Hello World uygulamamız, Notepad gibi bir program ve komut satırındaki csc (C# Compiler) ile inşa edilirdi.

O zamanlar bu bizim için yeterli görünüyordu ama tabi.Net Framework 2.0 ile birlikte hayatımıza yeni bir inşa süreci de girdi. Aslında bu günkü konumuzda da, Notepad (tam olarak Notepad 2) ve komut satırı aracını kullanarak ilerlemeye çalışıyor olacağız. Amacımız MSBuild platformunu çok kısaca tanımaya ve anlamaya çalışmak.

Microsoft Build Engine aslında başlı başına bir platformdur. Kısaca MSBuild olarak anılmaktadır ve bir uygulamanın inşa edilmesi noktasında devreye giren XML (eXtensible Markup Language) tabanlı bir Script bütününü esas alır. Kısacası uygulamanın inşa edilmesi sırasındaki aşamalar XML tabanlı bir akış olarak ifade edilebilmektedir. MSBuild platformunun en önemli özelliği ise, inşa sürecinde Visual Studio gibi bir araca ihtiyaç duymuyor oluşudur.

Evet Visual Studio’ nun kendisi, Build işlemlerinde bu platformu kullanmaktadır doğru ama, tam tersi durum geçerli değildir. Yani istersek MSBuild aracını kullanarak, bir uygulamanın veya uygulama ortamının üretilmesi sırasındaki aşamaları, basit bir Notepad aracı ile tasarlayabilir ve MSBuild.exe’ den yararlanarak hayata geçirebiliriz (Ancak bu gün şanslısınız çünkü Notepad2 isimli ürünü kullanacağız. [Sourceforge adresinden indirebilirsiniz](http://sourceforge.net/projects/notepad2/) ![Smile](/assets/images/2013/wlEmoticon-smile_91.png))

Bu fikir tabi ki otomatize edilmiş Build işlemlerinin de icra edilebileceği anlamına gelmektedir. Ki Team Foundation Server ürünü de MSBuild’ un etkin bir şekilde kullanılmasına olanak tanımaktadır. Özellikle Team Foundation Build olarak anılan platform içerisinde Build Server’ un kurulduğu ortam MSBuild Script’ lerini kullanarak üretim işlemlerini gerçekleştirmektedir. Söz gelimizi TFS tarafında eğer Continous Integration gibi bir strateji tercih edilmişse, kodlamacıların Check-In işlemleri sonrası Team Foundation Build devreye girecek ve MSBuild script’ leri otomatik olarak çalıştırılarak inşa işlemleri icra edilecektir. (Team Foundation Build ayrıca incelenmesi gereken bir konu olduğundan bu yazımızda detaylandırılmamıştır)

Visual Studio ortamında geliştirdiğimiz projeleri göz önüne aldığımızda, oluşturulan proje dosyaları içerisinde, MSBuild’ un kullanacağı ayarlar (Settings) ve bazı koşul bağlı işlevsellikler konuşlandırılmaktadır. Tabi geliştirici olarak bu kısımlar ile pek fazla uğraşmayız ve nihayetinde Visual Studio bizim için bu akışları otomatik olarak inşa eder. Ancak Release Manager gibi pozisyonlar özellikle bu proje dosya içeriklerini değerlendirerek genişletmeler yapabilir ve MSBuild sürecini farklılaştırabilirler.

> Visual Studio ile geliştirilen proje dosyaları C# tarafı için csproj, Visual Basic tarafı için vbproj, Managed C++ tarafı içinse vcxproj uzantılı olanlardır.

Hangi Durumlarda MSBuild

Peki MSBuild ağırlık olarak hangi hallerde ele alınır.

- En bilinen sebep elimizde Visual Studio olmayan bir ortamda inşa etme işlemlerinde değerlendirilebiliyor olmasıdır.
- Compiler devreye girmeden önce bazı dosyaların Process edilmesi gereken durumlarda ele alınabilir.(Pre-Processing Steps)
- Benzer şekilde Process sonrası yapılması istenen işlemler içinde kullanılabilir (Post-Processing Steps)
- Build işlemi sonucu çıktıların farklı bir klasöre taşınması sağlanabilir. (Ki bu klasör pek çok projenin ortaktaşa kullandığı bir assembly için paylaşımdaki bir makine bile olabilir)
- Çıktıların sıkıştırılması istendiği bir durumda ele alınabilir.
- İnşa işleminin birden fazla Process’ e bölünerek daha hızlı tamamlanması istendiği durumlarda değerlendirilebilir. (Özellikle Enterprise çözümlerde, n sayıda Branch’ in ve dolayısıyla çıktının söz konusu olabileceği senaryolarda, uzun sürebilecek Build işlemleri için kritik bir özelliktir)
- MSBuild 64bitlik bir sistem için inşa işlemini icra edebilir.
- Build işleminin herhangibir noktasında harici bir aracın kullanılması istendiği hallerde göz önüne alınabilir.

ve benzeri pek çok durumda MSBuild’ u açık bir şekilde özelleştirerek kullanabiliriz.

Proje Dosyasının İçine Bakalım

Hızlı bir şekilde uygulama geliştirme işine giren pek çok yazılımcı çoğunlukla MSBuild gibi programların ürettiği çıktıları göz ardı etmektedir. Nasıl olsa binlerce dolar verilerek satın alınan Visual Studio bizim yerimize pek güzel bu işi halletmektedir. Ancak gerçek hayatta öyle vakalar ve senaryolara vuku bulmaktadır ki, bunların üstesinden gelebilmek için özelleştirmelere gidilmesi şart olmaktadır. Bu özelleştirme konsepti MSBuild tarafı için de geçerlidir. Bu sebepten proje dosyalarının içeriğinin az da olsa bilinmesi en azından şema yapısının anlaşılması yararlıdır. Tipik olarak bir Console uygulaması dahi açsak, üretilen proje dosyası içerisinde (ki örneğimiz csproj uzantılı olandır) aşağıdakine benzer bir içerik oluştuğu görülecektir.

```xml
<?xml version="1.0" encoding="utf-8"?> 
<Project ToolsVersion="4.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003"> 
  <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" /> 
  <PropertyGroup> 
    <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration> 
    <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform> 
    <ProjectGuid>{21E86FBF-5A78-4175-8522-A6FC854BB637}</ProjectGuid> 
    <OutputType>Exe</OutputType> 
    <AppDesignerFolder>Properties</AppDesignerFolder> 
    <RootNamespace>ConsoleApplication33</RootNamespace> 
    <AssemblyName>ConsoleApplication33</AssemblyName> 
    <TargetFrameworkVersion>v4.5</TargetFrameworkVersion> 
    <FileAlignment>512</FileAlignment> 
  </PropertyGroup> 
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "> 
    <PlatformTarget>AnyCPU</PlatformTarget> 
    <DebugSymbols>true</DebugSymbols> 
    <DebugType>full</DebugType> 
    <Optimize>false</Optimize> 
    <OutputPath>bin\Debug\</OutputPath> 
    <DefineConstants>DEBUG;TRACE</DefineConstants> 
    <ErrorReport>prompt</ErrorReport> 
    <WarningLevel>4</WarningLevel> 
  </PropertyGroup> 
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' "> 
    <PlatformTarget>AnyCPU</PlatformTarget> 
    <DebugType>pdbonly</DebugType> 
    <Optimize>true</Optimize> 
    <OutputPath>bin\Release\</OutputPath> 
    <DefineConstants>TRACE</DefineConstants> 
    <ErrorReport>prompt</ErrorReport> 
    <WarningLevel>4</WarningLevel> 
  </PropertyGroup> 
  <ItemGroup> 
    <Reference Include="System" /> 
    <Reference Include="System.Core" /> 
    <Reference Include="System.Xml.Linq" /> 
    <Reference Include="System.Data.DataSetExtensions" /> 
    <Reference Include="Microsoft.CSharp" /> 
    <Reference Include="System.Data" /> 
    <Reference Include="System.Xml" /> 
  </ItemGroup> 
  <ItemGroup> 
    <Compile Include="Program.cs" /> 
    <Compile Include="Properties\AssemblyInfo.cs" /> 
  </ItemGroup> 
  <ItemGroup> 
    <None Include="App.config" /> 
  </ItemGroup> 
  <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" /> 
  <!-- To modify your build process, add your task inside one of the targets below and uncomment it. 
       Other similar extension points exist, see Microsoft.Common.targets. 
  <Target Name="BeforeBuild"> 
  </Target> 
  <Target Name="AfterBuild"> 
  </Target> 
  --> 
</Project>
```

Bu XML içeriğinin şema yapısına bakıldığında aşağıdaki grafikte görülen iskeletin söz konusu olduğu görülecektir.

[![msbuild_2](/assets/images/2013/msbuild_2_thumb.png)](/assets/images/2013/msbuild_2.png)

Aslında içerik okunduğunda, Visual Studio Proje Özelliklerinden de ayarladığımız pek çok öğenin buraya yazıldığı görülebilir. Örneğin PropertyGroup elementlerinde Debug, Debug-Any CPU ve Release-Any CPU için bazı atamalar söz konusudur.

Debug ile alakalı PropertyGroup’ a bakıldığında uygulamanın vereceği çıktının exe olacağı,.Net Framework 4.5 platformunu hedef aldığı, Assembly adının ConsoleApplication33 olduğu ve buna bağlı olarak da Root Namespace’ in yine ConsoleApplication33 şeklinde set edildiği görülebilir. Condition niteliklerinde belirtilen kısımlar, MSBuild uygulamasına bazı kriterlere göre nasıl çıktı üretmesi gerektiğini de söylemektedir. Örneğin uygulama Release modda inşa edildiğinde, çıktının bin\Release klasörüne doğru yapılması gerektiği yine bir PropertyGroup içerisinde ifade edilmiştir.

> Condition niteliklerinde kullanılan $ notasyonu mutlaka dikkatinizi çekmiştir. Aslında burada $[PropertyName] şeklinde bir kullanım söz konusudur. Burada inanılmaz geniş bir esneklik vardır.
> Örneğin $(registery:Hive\SomeKey\SomeSubKey@Value) gibi bir ifade ile Registery’ deki bir değeri okuyabilir ve Build içerisinde kullanabiliriz. Ya da $([System.DateTime]::Now.ToString ("yyyy.MM.dd")) gibi bir kullanım ile o anki zamanı istediğimiz formatta elde edebiliriz vb…
> [PropertyName] yerine gelecek MSBuild özelliklerinin neler olabileceğini [MSDN adresinden](http://msdn.microsoft.com/en-us/library/ms171458.aspx) öğrenebilirsiniz.

ItemGroup elementleri içerisine bakıldığında, uygulamanın referans ettiği diğer Assembly’ ların adları, Compile işlemi sırasında derlemeye tabi olacak C# dosyaları gibi bilgiler yer almaktadır. Visual Studio’ nun ürettiği XML içeriğine bakıldığında, son kısımda yorum satırları içerisine dahil edilmiş Target isimli bir element daha olduğu görülmektedir. Bu elementi kullanarak MSBuild için bazı Task’ lar tanımlanabilir ve inşa işlemi sırasında devreye girmeleri sağlanabilir.

Task olarak yapılan tanımlamalar paylaşılabilir ve farklı geliştirme Build’ larında da kullanılabilir. Bu yüzden bir yazılım evinin Build işlemlerinde standart olarak gerçekleştirdiği bazı yürütmeler, Task olarak tanımlanıp ilgili Build paketlerine gömülebilir. Ayrıca Task’ lar Reusable özelliği taşımaktadır. (Target elementleri genellikle yazıldıkları sırada çalışmaktadır. Yani belirli bir sırada icra edilmesi istenen Task’ lar var ise, Target elementinin buna uygun olacak şekilde kullanılması gerekir)

> Schema yapısı bu kadar basit değildir. Kullanılabilecek tüm elementler için [MSDN üzerindeki şu adrese gitmenizi](http://msdn.microsoft.com/en-us/library/5dy88c2e.aspx) öneririm.

Klavye Başına

Yazımızın bu bölümünde basit bir örnek geliştirmeye çalışıyor olacağız. Amacımız temel seviyede MSBuild aracını kullanmak ve konsepti anlamaya çalışmak olacaktır. İlk olarak basit bir C# koduna ihtiyacımız var. Bu amaçla örneğin C:\Samples\HowToMSBuild\ isimli klasör altında, aşağıdaki içeriğe sahip bir C# dosyası oluşturduğumuzu düşünelim.

[![msbuild_3](/assets/images/2013/msbuild_3_thumb.png)](/assets/images/2013/msbuild_3.png)

Bu adımdan sonra yine Notepad2 aracını kullanarak aşağıdaki içeriğe sahip bir csproj dosyası üreterek devam edelim.

```xml
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003"> 
  <ItemGroup> 
    <Compile Include="MSBuildHowTo.cs" /> 
  </ItemGroup> 
  <Target Name="Build"> 
    <Csc Sources="@(Compile)"/>  
  </Target> 
</Project>
```

Project içerisinde bir adet ItemGroup ve Target elementi yer almaktadır. ItemGroup içerisinde yer alan Compile elementi, Include niteliğinde (attribute), derleme işlemine tabi olacak dosyayı belirtmektedir. Target altındaki Csc elementinin Sources niteliğinde ise Compile isimli bir komut yer almaktadır. Dolayısıyla bu Task, MSBuild aracına, Compile elementine dahil edilen dosyanın derlenmesi gerektiğini söylemektedir.

Bu içeriği Builder.csproj adı ile kaydettikten sonra ise sıradaki operasyon, MSBuild komut satırı aracını kullanarak inşa işlemini yürüttürmektir. Bunun için MSBuild’ u aşağıdaki gibi kullanabiliriz.

msbuild Builder.csproj /t: Build /verbosity: detailed

[![msbuild_5](/assets/images/2013/msbuild_5_thumb.png)](/assets/images/2013/msbuild_5.png)

Görüldüğü gibi çalışma sonrasında MSBuildHowTo isimli bir exe dosyası oluşmuştur. Söz konusu exe dosyasını doğrudan çalıştırdığımızda ise, pre-precessor direktifi dışında kalan kod parçasının yürütüldüğü gözlemlenecektir.

> MSBuild özellikle kurulu olan.Net Framework versiyonuna bağlı olaraktan Microsoft.Net\Framework\vX.X.XXXXX altında yer almaktadır. Örneğin ben kendi sistemimde aşağıdaki klasörde yer alan sürümü kullandım.
> [![msbuild_4](/assets/images/2013/msbuild_4_thumb.png)](/assets/images/2013/msbuild_4.png)
> Bunu sistem’ de Path olarak belirtebiliriz ama dilerseniz doğrudan Visual Studio Command Prompt’ tan da yararlanabiliriz.

CSPROJ İçeriğini Genişletelim

Şimdi csproj dosyasının içeriğini biraz daha genişletelim ve aşağıdaki hale getirelim.

```xml
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003"> 
  <PropertyGroup> 
    <AssemblyName>MSBuildHowTo</AssemblyName> 
    <OutputPath>Bin\</OutputPath> 
  </PropertyGroup> 
  <ItemGroup> 
    <Compile Include="MSBuildHowTo.cs" /> 
  </ItemGroup> 
  <Target Name="Build"> 
    <MakeDir Directories="$(OutputPath)" Condition="!Exists('$(OutputPath)')" /> 
    <Csc Sources="@(Compile)" OutputAssembly="$(OutputPath)$(AssemblyName).exe" /> 
  </Target> 
</Project>
```

Bu sefer PropertyGroup içerisinde assembly adını (AssemblyName elementi) ve build işlemi sonrası ortaya çıkacak olan çıktının konuşlandırılacağı klasörü de belirttik (OutputPath). Diğer yandan bu örneğimizde yer alan en önemli kısım Build isimli Target elementinin içeriğidir. Dikkat edilecek olursa ilk sırada MakeDir isimli element yer almaktadır. Burada $(OutputPath) ile çıktının yapılacağı klasör işaretlenirken aslında PropertyGroup>OutputPath elementinin içeriği ifade edilmektedir. Önemli olan kısım ise Condition niteliğinde yazılan değerdir.!Exists (‘$(OutputPath)’) ifadesi ile şu söylenmektedir. “OutputPath yoksa…”. Buna göre eğer Condition sağlanırsa, MakeDir OutputPath’ in oluşturulması gerektiğini belirtecektir.

Csc elementinde ise derleme işlemi için kullanılacak olan kaynak Sources elementi ile ifade edilirken, aslında ItemGroup içerisindeki Compile elementinde yer alan Include niteliğinin değeri işaret edilmektedir. OutputAssembly niteliğine atanan değer ise, OutputhPath özelliğinin belirttiği klasörün altına AssemblyName’ in işaret ettiği isimle bir exe üretilmesi gerektiğini belirtmektedir. Buna göre komut satırından yapılan MSBuild çağrısı sonucu aşağıdaki gibi olacaktır.

msbuild Builder.csproj /t: Build /verbosity: detailed

[![msbuild_6](/assets/images/2013/msbuild_6_thumb.png)](/assets/images/2013/msbuild_6.png)

Oldukça zevkli öyle değil mi?

![Smile](/assets/images/2013/wlEmoticon-smile_91.png)

Target Belirtmek

Öyleyse gelin olayı biraz daha genişleterek devam edelim.

```xml
<Project DefaultTargets="Build" xmlns=" http://schemas.microsoft.com/developer/msbuild/2003"> 
  <PropertyGroup> 
    <AssemblyName>MSBuildHowTo</AssemblyName> 
    <OutputPath>Bin\</OutputPath> 
  </PropertyGroup> 
  <ItemGroup> 
    <Compile Include="MSBuildHowTo.cs" /> 
  </ItemGroup> 
  <Target Name="Build"> 
    <MakeDir Directories="$(OutputPath)" Condition="!Exists('$(OutputPath)')" /> 
    <Csc Sources="@(Compile)" OutputAssembly="$(OutputPath)$(AssemblyName).exe" /> 
  </Target> 
  <Target Name="Clean" > 
    <Delete Files="$(OutputPath)$(AssemblyName).exe" /> 
  </Target> 
  <Target Name="Rebuild" DependsOnTargets="Clean;Build" /> 
</Project>
```

Bu seferki çalışmada 3 adet Target elementi söz konusudur. Ancak her birinin Name niteliğinin değerleri farklıdır. Buna göre, Build, Clean ve Rebuild isimli Target’ lar olduğunu ve aslında bunların 3 ayrı Task’ ı ifade ettiğini düşünebiliriz. Yani MSBuild aracını kullanırken /t: [TargetNameValue] şeklinde bir yaklaşım ile, istediğimiz Target’ ın yürütülmesini sağlayabiliriz. Söz gelimi;

msbuild Builder.csproj /t: Clean /verbosity: detailed

şeklinde yapılan çağrı sonucunda, proje dosyasındaki Target elementlerinden Clean isimli olanı çalıştırılacaktır. Bu elementte ise Delete isimli bir alt element yer almakta olup Files niteliğinde belirtilen kritere göre, Bin klasöre altında assembly adı ile duran exe dosyasının silinmesi gerektiği ifade edilmektedir.

[![msbuild_7](/assets/images/2013/msbuild_7_thumb.png)](/assets/images/2013/msbuild_7.png)

Eğer Rebuild takısını kullanırsak bu durumda Target->Name niteliği Rebuild olan bölüm devreye girecektir. Bu vakada dikkat edilmesi gereken ise DependsOnTargets niteliği içerisinde yazılan Clean;Build ifadesidir. Yani şunu ifade etmiş oluruz;

> Ey MSBuild!…Önce Clean isimli Target, sonrasında ise Build isimli Target içeriğini icra et

ki bu durumda Bin klasörü içeriği silinecek ve tekrardan bir derleme işlemi yapılarak, orada ilgili exe çıktısının üretilmesi sağlanacaktır.

msbuild Builder.csproj /t: Rebuild /verbosity: detailed

[![msbuild_8](/assets/images/2013/msbuild_8_thumb.png)](/assets/images/2013/msbuild_8.png)

Örnekler daha da çoğaltılabilir. Yapılabilecek pek çok şey var

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_189.png)

Bunun için mutlaka MSDN orjinli bir kaynağa başvurmanızı veya yazımızın sonunda belirttiğimiz tarzdaki bir kitabı tedarik etmenizi öneririm. Ancak temel mantığı ifade edebildiğimizi varsayıyorum. Bundan sonrasında Condition’ lara ve kullanılabilecek element/attribute tiplerine bakılması yeterli olur düşüncesindeyim.

.Net Framework 4.5 ile Gelen Yenilikler

MSBuild ürününe Framework 4.5 sürümü ile birlikte bazı geliştirmeler ve yeni özellikler de katılmıştır. Henüz inceleme fırsatı bulamadığım bu özellikleri aşağıdaki maddeler ile özetleyebiliriz.

- ARM (Advanced RISC Machine) desteği gelmiştir. Yani Build çıktıları ARM işlemcilerini hedef alacak şekilde üretilebilir.
- Bir Task (Target elementi ile belirtilen bir görev diyelim) süreç dışı (Out of Process) modda çalışmaya zorlanabilir.
- Yeni bazı XML element ve nitelikleri (Attributes) gelmiştir.
- Klasik bir cümle olacak ama, Performans (Performance) ve Ölçeklendirme (Scabilitiy) noktasında iyileştirmeler vardır ![Smile](/assets/images/2013/wlEmoticon-smile_91.png)

> MSBuild’ un etkili kullanımı üzerine geliştirilmiş bazı araçlar da vardır. Görsel arabirimleri olan bu araçlar yardımıyla MSBuild akışlarını daha kolay yönetebiliriz. Attrice firmasının bu alanda ön plana çıkan [Microsoft Build Sidekick](http://www.attrice.info/msbuild/index.htm) isimli ürünü gibi.

Öneri Kitap

[![msbuild_1](/assets/images/2013/msbuild_1_thumb.jpg)](/assets/images/2013/msbuild_1.jpg) Microsoft’ un Visual Studio ailesi ve geliştirme platformu oldukça geniş bir alana yayılmakta olup, pek çok notkasında uzmanlık gerektiren yapılar içermektedir. Bu sebepten söz konusu yapılara yönelik pek çok yayın da (kitap, official site, blog vb) mevcuttur.

Örneğin MSBuild tarafında daha önceden yayınlanmış olan Inside The Microsoft Build Engine isimli kitabın Nisan ayı içerisinde yayınlanan yeni bir tamamlayıcı baskısı mevcuttur. Yaklaşık olarak 120 sayfalık bir kitap olmasına rağmen odaklandığı konu özünde MSBuild ürünüdür. Kitaba [Amazon üzerinden bu adres yardımıyla](http://www.amazon.com/Supplement-Inside-Microsoft%C2%AE-Build-Engine/dp/0735678162/ref=sr_1_1?s=books&ie=UTF8&qid=1361775657&sr=1-1&keywords=msbuild) erişebilirsiniz.

Böylece geldik bir yazımızın daha sonuna. Bu yazımızda kısada olsa, MSBuild platformunu ve XML tarafındaki betikleri (Scripts) anlamaya çalıştık. Ağırlıklı olarak bir inşa sürecine müdahale edebildiğimizi, bunun için platformun sunduğu bazı standart element ve niteliklerin olduğunu gördük. Devamı sizde artık

![Smile](/assets/images/2013/wlEmoticon-smile_91.png)

Bir başka yazımızda görüşünceye dek hepinize mutlu günler dilerim.

[Orjinal Yazım Tarihi – 25/02/2013]

[[Güncel bilgi –> MS Build is now part of Visual Studio!]](http://blogs.msdn.com/b/visualstudio/archive/2013/07/24/msbuild-is-now-part-of-visual-studio.aspx)