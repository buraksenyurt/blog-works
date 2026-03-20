---
layout: post
title: "Text Template ve VSIX Project Template Kullanımı"
date: 2012-07-17 08:04:00 +0300
categories:
  - visual-studio
tags:
  - visual-studio
  - csharp
  - xml
  - dotnet
  - aspnet
  - linq
  - windows-forms
  - http
  - generics
  - rc
  - dependency-management
---
Çoğu zaman geliştiricilerin karşısına zaman kısıtı olan projelerde, sıklıkla tekrar eden çözümsel ihtiyaçlar çıkar. Örneğin, ürünün içerisinde n sayıda ekran kullanıldığını ve bunların aslında belirli bir noktaya kadar bir kaç parametre ile değişen ama standart kod içeriklerine sahip olduğunu düşünün. Hatta bu tip ekranları bir kaç proje için aynı şekilde ürettiğinizi.

[![deadline](/assets/images/2012/deadline_thumb.jpg)](/assets/images/2012/deadline.jpg)

Yazılımcılar bu gibi durumlarda yükü azaltmak ve özellikle Deadline sürelerini eritmek adına, parçaları otomatik olarak üreten kodlar ile çözümleme yoluna gitmeye gayret ederler. Bir başka deyişle otomatik kod üreticilerini (Auto Code Generator) yazmak için çaba gösterirler. Bu oldukça etkili ve önemli bir yaklaşımdır. Tekrarlı işleri azaltmakla kalmaz, aynı zamanda yeniden yapılması gereken üretimlerde veya toplu güncellemelerde işleri merkezi bir noktadan kolaylaştırır.

Ancak yazılımcıların bu gibi ihtiyaçlarda yine de yaptıkları bazı temel hatalar vardır. Söz gelimi dosya üretme ve devreye alma işlemlerini yapmak için herşeyi sıfırdan yazma yoluna gidebilirler. Oysaki kullanılan geliştirme ortamlarının bu gibi noktalarda ürettikleri bazı kolaylaştırıcı çözüm yolları da bulunmaktadır. Söz gelimi Visual Studio tarafından bakıldığında, Text Template'ler kod dosyalarının otomatik üretiminde kullanılabilir. Hatta Visual Studio SDK ile birlikte gelen VSIX Project Template'ler ile bu gibi üretimlerin birer Extension olarak şablonlaştırılması da mümkündür.

İşte bu yazımızda bu tip bir ihtiyacı göz önüne alarak Text Template ve bununla ilişkili VSIX Project Template üretimi işinin içerisinde yer almaya çalışıyor olacağız. Text Template ve VSIX Project Template öğeleri ile ilişkili teknik detayları [MSDN](http://msdn.microsoft.com/en-us/library/dd885119)’ de bulabilirsiniz. Biz bu yazımızda çok daha basit bebek adımları ile ilerleyerek çözüme ulaşmaya çalışacak ve adımlarımız arasında, aslında neleri yapıp neleri elde ettiğimizi göreceğiz.

İşe ilk olarak Class Library projesi oluşturup, Text Template tipinden bir öğeyi içeri dahil ederek başlayabiliriz.

[![tutorialtt1](/assets/images/2012/tutorialtt1_thumb.png)](/assets/images/2012/tutorialtt1.png)

Öğemize DbClassTemplate ismini verebiliriz. TT (Text Template) uzantılı olan bu dosya aslında metinsel içerik ile C#/Vb.Net tabanlı kod parçalarını bir arada ele alıp üretimi gerçekleştirmek üzere kullanılmaktadır. Visual Studio ile entegre çalışmakta olup yaptığımız her Save işlemi sonrası bir üretim gerçekleşmektedir. Söz konusu dosyanın içeriğini ise aşağıdaki gibi oluşturduğumuzu düşünelim.

{% raw %}
```csharp
<#@ template debug="True" hostspecific="True" language="C#" #> 
<#@ output extension=".cs" #> 
<#@ Assembly Name="System.Data" #> 
<#@ Import Namespace="System.Data.SqlClient" #> 
<#@ Import Namespace="System.Data" #> 
<#@ Import Namespace="System.Text" #> 
// Özet        :Bu dosya içerisinde seçilen veritabanı içerisindeki tablolara eş düşen birer sınıf söz konusudur 
// Yazan    :Unknown Developer 
// Yazım tarihi    :Uzay Tarihi 2012 
<# 
var connectionString = "data source=.;database=Chinook;integrated security=SSPI;MultipleActiveResultSets=True"; 
var builder = new StringBuilder(); 
var dbName = "Chinook"; 
using (SqlConnection connection = new SqlConnection(connectionString)) 
{ 
    var commandTables = new SqlCommand("select object_id,name from sys.tables where type='U' order by name",connection); 
    var commandColumns = new SqlCommand("select C.name, (select top 1 name from sys.types T where T.system_type_id=C.system_type_id) as TypeName from sys.columns C where object_id=@ObjectId order by C.name",connection); 
    commandColumns.Parameters.Add("@ObjectId", SqlDbType.Int); 
    connection.Open(); 
    SqlDataReader readerTables = commandTables.ExecuteReader(); 
    while (readerTables.Read()) 
    { 
        builder.AppendLine(string.Format("\tpublic class {0}", readerTables["name"].ToString())); 
        builder.AppendLine("\t{"); 
        commandColumns.Parameters["@ObjectId"].Value = Convert.ToInt32(readerTables["object_id"]); 
        SqlDataReader readerColumns = commandColumns.ExecuteReader(); 
        while (readerColumns.Read()) 
        { 
            // Bu kısımda daha etkili bir tip dönüşüm fonksiyonelliği kullanılmalıdır. 
            string dbType=readerColumns["TypeName"].ToString(); 
            string dotNetType = "object"; 
            if (dbType.Contains("varchar") || dbType.Contains("text")) dotNetType = "string"; 
            if (dbType.Contains("int") || dbType == "number" || dbType=="numeric") dotNetType = "int"; 
            if (dbType == "decimal" ||dbType == "money") dotNetType = "decimal"; 
            if (dbType.Contains("date") || dbType.Contains("Date")) dotNetType = "DateTime"; 
            if (dbType.Contains("binary")) dotNetType = "byte[]"; 
            builder.AppendLine(string.Format("\t\tpublic {0} {1}{{get;set;}}", dotNetType, readerColumns["name"].ToString())); 
        } 
        readerColumns.Close(); 
        builder.AppendLine("\t}"); 
    } 
    readerTables.Close(); 
} 
#> 
using System; 
using System.Linq; 
namespace <#= dbName #> 
{ 
<#= builder.ToString() #> 
}
```
{% endraw %}

Şimdi bu kod parçasında ne yaptığımıza bir bakalım.

<#@ ile başlayıp biten kısımlarda, üretimi yapılacak olan çıktıya ve TT dosyası içeriğine ait bir takım bilgiler vermekteyiz. Örneğimizde çıktının cs uzantılı bir CSharp dosyası olacağı belirtiliyor. Bunun dışında System.Data Assembly'ının çıktının bulunacağı yerde var olması gerektiği ifade ediliyor. Ayrıca kod içerisinde kullanılacak olan isim alanları (Namespaces) da belirtiliyor.

<# ile başlayıp biten kısımların haricinde kalan bölümler, tipik olarak çıktı dosyası içerisine yazılan metinsel bilgiler olarak da düşünülebilirler. Her ne zaman bir kod parçasını çalıştırmak istersek, <# ile başlayan blokları ele almamız gerekmektedir.

Örneğimizde yer alan ilk bloğun yaptığı iş, veritabanına bağlanıp Chinook tablolarını ve bu tablolara ait kolonların karşılığı olan class ve property içeriklerini üretmektir. Bu amaçla içeride StringBuilder tipinden yararlanıldığı görülmektedir. Yanlız şu kod parçasına dikkat etmeliyiz

![Sarcastic smile](/assets/images/2012/wlEmoticon-sarcasticsmile_5.png)

```csharp
using System; 
using System.Linq; 
namespace <#= dbName #>  
{ 
<#= builder.ToString() #>  
}
```

Burada <#=builder.ToString ()#> yazan kısmın yerine, yukarıdaki kod parçasının içerisinde kullanılan builder isimli StringBuilder değişkeninin çıktısı yerleştiriliyor olacaktır. Hatta dbName değişkeni de namespace adı olarak alınmaktadır. Geri kalan kısımlar sabit metinsel bilgilerdir.

Dosyamızı bu haliyle kayıt ettiğimizde, Visual Studio IDE'si otomatik olarak içeriği çalıştıracak ve bir C# kod dosyasını üreterek içeriğini aşağıdaki şekilde görülen hale getirecektir.

[![tutorialtt2](/assets/images/2012/tutorialtt2_thumb.png)](/assets/images/2012/tutorialtt2.png)

Dikkat edileceği üzere Chinook veritabanında yer alan tablolara karşılık gelen sınıf dosyaları üretilmiş olup, Column’ ların veri tiplerine göre de uygun Property’ ler ilgili tipler içerisine dahil edilmiştir. Artık elimizde cs tabanlı otomatik olarak üretilmiş bir içerik mevcuttur. Bu haliyle de içeriği herhangibir.Net projesine ekleyip kullanabilme şansına sahibiz.

Şu anda yapmak istediğimiz ise söz konusu Text Template içeriğini ve üretimini bir Extension haline getirmektir. Bu sayede bu TT öğesini herhangibir projeye ekleyebilir ve gerekli otomatik üretimleri yaptırtabiliriz. Tabi bu yapının parametrik olarak çalışması çok daha önemlidir. Nitekim geliştiriciler üretimini yapacakları veritabanına ait Connection String bilgisini isterlerse seçebilmelidir. Şimdi yazımızın ikinci kısmını ele almaya başlayabiliriz

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_99.png)

Bu amaçla ilk olarak File menüsünden Export Template seçeneğini işaretleyip DbClassGenerator’ ü sıkıştırılmış bir paket haline getirmemiz gerekmektedir. Söz konusu paketin üretimi sırasında Visual Studio bir Wizard yardımıyla bizden bir kaç adımı tamamlamamızı isteyecektir. Söz konusu adımları aşağıdaki sırayla icra edebiliriz.

Adım 1de Item Template seçimi yapılabilir. Nitekim TT’ mizi bir proje öğesi olarak kullandırtmak istiyoruz.

[![tutorialtt3](/assets/images/2012/tutorialtt3_thumb.png)](/assets/images/2012/tutorialtt3.png)

Adım 2 de paket içerisine almak istediğimiz Text Tempalte dosyalarını seçebiliriz.

[![tutorialtt4](/assets/images/2012/tutorialtt4_thumb.png)](/assets/images/2012/tutorialtt4.png)

3ncü adımda, Text Template’ in çalışması sonrası ortaya çıkacak ürünün dahil olduğu projenin referans etmesi gereken Assembly’ lar var ise bunu belirtebiliriz. Örneğimizde temel olan System ve System.Data assembly’ ları seçilmiştir.

[![tutorialtt5](/assets/images/2012/tutorialtt5_thumb.png)](/assets/images/2012/tutorialtt5.png)

4ncü adımda Template ile ilişkili isim ve açıklama bilgilerini verip dilersek bir de Icon belirleyerek öğemizin görünümünü göz alıcı bir hale getirebiliriz.

[![tutorialtt6](/assets/images/2012/tutorialtt6_thumb.png)](/assets/images/2012/tutorialtt6.png)

Bu işlemleri tamamladığımızda ise aşağıdaki ekran görüntüsünde yer alan sıkıştırılmış dosyanın üretildiğini görmüş olacağız.

[![tutorialtt7](/assets/images/2012/tutorialtt7_thumb.png)](/assets/images/2012/tutorialtt7.png)

Üretilen dosya adresi bizim için önemlidir nitekim VSIX Project Template’ de bunu kullanıyor olacağız. Çünkü VSIX projesi ilgili dosyayı alıp kendi içerisine entegre ediyor olacak. Öyleyse Visual C# – Extensibility sekmesinden bir VSIX Project öğesi seçerek ilerlemeye devam edelim.

> Buradaki VSIX Project öğesinin kullanılabilir olması için Visual Studio 2012 SDK’ nın yüklü olması gerektiğini unutmayalım. Aynı durum Visual Studio 2010 sürümü için de geçerlidir.

[![tutorialtt8](/assets/images/2012/tutorialtt8_thumb.png)](/assets/images/2012/tutorialtt8.png)

Bu işlem sonrasında Visual Studio IDE’ si karşımıza detaylı bir özellik penceresi çıkartacaktır. Bu özellikler temel olarak vsixmanifest uzantılı dosya içerisinde bulunmaktadır. Designer tarafında ki en önemli kısımlardan birisi de Assets bölümüdür. Burada daha önceden Zip çıktısı haline getirdiğimiz Template içeriğinin işaretlenmesi esasına dayalı bazı seçimler yapmamız gerekmektedir. Örneğimizde bu kısmı aşağıdaki gibi doldurmamız yeterli olacaktır.

[![tutorialtt10](/assets/images/2012/tutorialtt10_thumb.png)](/assets/images/2012/tutorialtt10.png)

Type kısmında ItemTemplate seçildiği görülmektedir (Template’ i hangi tipte Export ettiğimizi hatırlayın ![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_99.png)) Kaynak olarak File on filesystem seçeneği işaretlenmiştir. Path bölümünde zip uzantılı dosyanın lokasyonunun verilmesi yeterli olacaktır. İlgili adres bilgisi verildikten sonra Asset kısmına tekrardan girilirse yol bilgisinin artık projedeki ItemTemplates klasörünü işaret ettiği gözlemlenebilir. Bunun sebebi ilk adımda seçilen adreste yer alan içeriğin VSIX projesine kopyalanmış olmasıdır.

[![tutorialtt11](/assets/images/2012/tutorialtt11_thumb.png)](/assets/images/2012/tutorialtt11.png)

Manifest bilgilerini düzenlediğimiz kısımda set ettiğimiz özellikler çok doğal olarak arka plana XML tabanlı olarak yazılmaktadır. Sonuç itibariyle vsixmanifest dosyasının içeriği de aşağıdaki gibi olabilir.

```xml
<?xml version="1.0" encoding="utf-8"?> 
<PackageManifest Version="2.0.0" xmlns="http://schemas.microsoft.com/developer/vsx-schema/2011" xmlns:d="http://schemas.microsoft.com/developer/vsx-schema-design/2011"> 
    <Metadata> 
        <Identity Id="25970a0b-c664-4112-8739-3e2bbc1fea61" Version="1.0" Language="en-US" Publisher="BurakSenyurt" /> 
        <DisplayName>Db POCO Generator</DisplayName> 
        <Description>Veritabanı tabloları için otomatik olarak POCO tip üretimi gerçekleştirmek üzere tanımlanmış bir Extension dır.</Description> 
    </Metadata> 
    <Installation> 
        <InstallationTarget Id="Microsoft.VisualStudio.Pro" Version="11.0" /> 
    </Installation> 
    <Dependencies> 
        <Dependency Id="Microsoft.Framework.NDP" DisplayName="Microsoft .NET Framework" d:Source="Manual" Version="4.5" /> 
    </Dependencies> 
   <Assets> 
        <Asset Type="Microsoft.VisualStudio.ItemTemplate" d:Source="File" Path="ItemTemplates" d:TargetPath="ItemTemplates\Db Class Generator.zip" /> 
    </Assets> 
</PackageManifest>
```

Biz örneğimizde çok basit olarak Display Name, Description, Id, Version gibi kısımlara müdahale ettik. Ancak daha ileri seviyede müdahalelerde de bulunabiliriz. Söz gelimi Framework hedef versiyonlarını değiştirebilir ki örnemğizde bu bağımlılık 4.5 sürümüne göre yapılmıştır.

Yaptığımız çalışma sonucu projeyi build edersek eğer aşağıdaki gibi vsix uzantılı Extension Installer dosyasının yer aldığı bir sonuca ulaşmış oluruz.

[![tutorialtt12](/assets/images/2012/tutorialtt12_thumb.png)](/assets/images/2012/tutorialtt12.png)

Bir başka deyişle söz konusu installer dosyasını çalıştırıp template’ imizin Visual Studio ortamına entegre edilmesini sağlayabiliriz. Lakin halen eksik olan bir kaç şey bulunmaktadır. Projemizde kullandığımız Text Template içerisinde yer alan Connection String ve DbName gibi bilgiler hard coded olarak tutulmaktadır. Oysaki bağlantı bilgisini dışarıdan verebilmiş olsaydık çok daha generic bir proje öğemiz olurdu. Şimdi bu parametreleri dışarıdan nasıl alabileceğimize bir bakalım

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_99.png)

İlk olarak DbClassGenerator.Design isimli bir Windows uygulaması oluşturalım ve söz konusu projeye aşağıdaki referansları ekleyelim. Bu proje öğeyi eklediğimiz sırada devreye girecek bir arabirimi geliştiricinin karşısına çıkartmak için kullanılacaktır.

> Projenin VSIX dosyasının install edilmesi sırasında devreye girebilmesi için, output olarak C:\Program Files\Microsoft Visual Studio 11.0\Common7\IDE\ adresine çıktı vermesi gerekmektedir. Bu nedenle projenin özelliklerinden Build->Output kısmında ilgili lokasyon bildirilmelidir.
> Ayrıca projenin output tipinin Class Library olarak set edilmesi gerekmektedir.

[![tutorialtt13](/assets/images/2012/tutorialtt13_thumb.png)](/assets/images/2012/tutorialtt13.png)

EnvDTE ve Microsoft.VisualStudio.TemplateWizardInterface assembly’ larının referans edilmesinin ardından, aşağıdaki basit Windows Forms tasarımını ve kod içeriğini geliştirerek ilerleyebilir.

> Örneğimizde çok basit bir arabirim söz konusudur. Ancak burada daha kompleks bir Connection String designer’ ı da söz konusu olabilir. Bu tamamen tasarlanan Template’ in kullanacağı parametrelerinin nasıl ve ne şekilde set edileceğine bağlı olarak değişir.
> Yine örnekte hata yönetimi çok fazla dikkate alınmamıştır. Zero Data testleri yapılmamıştır. Bu gibi hususları kendi geliştireceğiniz örnek içerisinde dikkatli bir şekilde ele almalısınız.

[![tutorialtt17](/assets/images/2012/tutorialtt17_thumb.png)](/assets/images/2012/tutorialtt17.png)

```csharp
using System; 
using System.Windows.Forms;

namespace DbGenerator.Design 
{ 
    public partial class Form1 
        : Form 
    { 
        public string ConnectionString { get; set; } 
       public string NamespaceName { get; set; }

        public Form1() 
        { 
            InitializeComponent(); 
        }

        private void Form1_Load(object sender, EventArgs e) 
        {

        }

        private void btnSet_Click(object sender, EventArgs e) 
        { 
            if (!String.IsNullOrEmpty(txtConnectionString.Text)) 
                ConnectionString = txtConnectionString.Text; 
            if (!string.IsNullOrEmpty(txtDbName.Text)) 
                NamespaceName = txtDbName.Text;

            DialogResult = System.Windows.Forms.DialogResult.OK; 
        } 
    } 
}
```

Dikkat edileceği üzere bu basit formdan TT içerisindeki parametrelere set edeceğimiz değişken değerlerini almaktayız. Tabi işin önemli kısmı bu pencerenin, öğe Visual Studio projesine eklenirken karşımıza çıkmasını sağlamaktır. Bunun için IWizard arayüzünü (interface) implemente edecek bir sınıfı projemize dahil etmemiz gerekiyor. GeneratorDesigner olarak adlandırabileceğimiz sınıfın kod içeriğini ise aşağıdaki gibi geliştirdiğimizi düşünelim.

```csharp
using System.Collections.Generic; 
using System.Windows.Forms; 
using EnvDTE; 
using Microsoft.VisualStudio.TemplateWizard;

namespace DbGenerator.Design 
{ 
    public class GeneratorDesigner 
        :IWizard 
    { 
        private bool CanAddProjectItem;

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams) 
        { 
            Form1 frm = new Form1(); 
            if (frm.ShowDialog() == DialogResult.OK) 
            { 
                replacementsDictionary.Add("$connectionString$", frm.ConnectionString); 
                replacementsDictionary.Add("$dbnameString$", frm.NamespaceName); 
            } 
            else 
            { 
                // Default bir değer kısımlarına geçerli bir takım bilgiler yazmakta yarar olabilir. Nitekim geçerli bir ConnectionString bilgisi olmaması halinde uygulama hata verecek ve cs dosyaları başarılı bir şekilde üretilmeyecektir. 
                replacementsDictionary.Add("$connectionString$", "default bir değer"); 
                replacementsDictionary.Add("$dbnameString$", "Default bir değer"); 
            } 
            CanAddProjectItem = true; 
        }

        public bool ShouldAddProjectItem(string filePath) 
        { 
            return CanAddProjectItem; 
        }

        public void BeforeOpeningFile(ProjectItem projectItem) 
        { 
        }

        public void ProjectFinishedGenerating(Project project) 
        { 
        }

        public void ProjectItemFinishedGenerating(ProjectItem projectItem) 
        { 
        }

        public void RunFinished() 
        { 
        } 
    } 
}
```

IWizard türevli bu sınıf içerisinde basit bir Application Life Cycle söz konusudur aslında. Biz ilk girişte bazı parametrelerin set edilmesi için araya girerek müdahale de bulunuyoruz. Bu nedenle dikkat edileceği üzere RunStarted metodunda, $ sembolleri arasında tanımlanmış olan bazı parametrelerin değiştirilmesi işlemi söz konusudur. Bunun için tasarladığımız Windows Form’ una ait bir örnek oluşturulmuş, ardından form içerisine girilen metinsel bilgiler alınarak koleksiyona dahil edilmiştir. Tabi çok doğal şu anda kafamızda bir soru oluşması muhtemeldir. Acaba bu değişkenler nerede tanımlanmalıdır

![Thinking smile](/assets/images/2012/wlEmoticon-thinkingsmile_2.png)

> Application Life Cylce içerisindeki metodlara bakıldığında Loglama işlemlerinin de yapılabileceği uygun noktalar olduğu görülmektedir.

Tahmin edeceğiniz üzere Text Template içeriğini kullanan kısım VSIX projemizdir. Bu proje ItemTemplates klasörüne zip olarak ilgili Template içeriğini ve dosyalarını indirmiştir. Parametreleri sisteme entegre etmek için bu zip içeriğindeki bazı dosyaları değiştirmemiz şarttır (Ya da en baştan TT’ yi bunu düşünerek tasarlamak gerekmektedir). İlk olarak vstemplate uzantılı dosya içeriğinde aşağıdaki değişiklikleri yapalım.

```xml
<VSTemplate Version="3.0.0" xmlns="http://schemas.microsoft.com/developer/vstemplate/2005" Type="Item"> 
  <TemplateData> 
    <DefaultName>Db Class Generator.tt</DefaultName> 
    <Name>Db Class Generator</Name> 
    <Description>Veritabanı için otomatik POCO Sınıf üretimlerini gerçekleştirir.</Description> 
    <ProjectType>CSharp</ProjectType> 
    <SortOrder>10</SortOrder> 
    <Icon>__TemplateIcon.ico</Icon> 
  </TemplateData> 
  <TemplateContent> 
    <References> 
      <Reference> 
        <Assembly>System</Assembly> 
      </Reference> 
      <Reference> 
        <Assembly>System.Data</Assembly> 
      </Reference> 
    </References> 
    <ProjectItem SubType="" TargetFileName="$fileinputname$.tt" ReplaceParameters="true">DbClassTemplate.tt</ProjectItem> 
   <!--<ProjectItem SubType="Code" TargetFileName="$fileinputname$.cs" ReplaceParameters="true">DbClassTemplate.cs</ProjectItem>--> 
  </TemplateContent>    
    <WizardExtension> 
        <Assembly>DbClassGenerator.Design</Assembly> 
        <FullClassName>DbClassGenerator.Design.GeneratorDesigner</FullClassName> 
    </WizardExtension> 
</VSTemplate>
```

3 önemli değişiklik vardır.

- cs dosyasının üretimine ait bildirimde bulunan ProjectItem elementi kaldırılmıştır.(Örnekte yorum satırı yaptık)
- İkinci olarak tt için kullanılan ProjectItem elementindeki ReplaceParameters değeri true yapılmıştır. Nitekim $ işaretleri arasına aldığımız parametrelerin değiş tokuş edileceğinin belirtilmesi gerekmektedir.
- 3ncü ve en önemli değişiklik ise WizardExtension bloğunun eklenmiş olmasıdır. Bu blok tahmin edileceği üzere $ işareti ile belirlenmiş parametrelerin alınması için gerekli arabirimin devreye alınması sırasında rol oynamakta olup, hangi IWizard türevli tipin değerlendirileceğini belirtmektedir.

Tabi tt uzantılı dosyamızda da bazı değişikliklerin yapılması şarttır. Yani $ işaretli parametrelerin eklenmesi gerekmektedir.

```xml
<# 
    var connectionString = "$connectionString$"; 
    var builder = new StringBuilder(); 
    var dbName = "$dbnameString$";
```

Bu son işlemlerden sonra VSIX Projesini bir kere daha derlememiz gerekir. (Hatta daha önceden install etmişsek uninstall edip son bir build yapıp tekrardan install etmemiz gerekmektedir) Tüm bu işlemlerin arından VSIX dosyasını çalıştırıp template’ in yüklenmesini sağlayabiliriz. Eğer işler yolunda giderse aşağıdaki iki pencereyi görmemiz şu an mutlu olmamız için yeterli olacaktır

![Open-mouthed smile](/assets/images/2012/wlEmoticon-openmouthedsmile_28.png)

[![tutorialtt14](/assets/images/2012/tutorialtt14_thumb.png)](/assets/images/2012/tutorialtt14.png)

[![tutorialtt15](/assets/images/2012/tutorialtt15_thumb.png)](/assets/images/2012/tutorialtt15.png)

Şimdi her hangibir proje açalım ve daha sonra Add New Item ile ilerleyelim. Listenin en başında gurur kaynağımız olan öğeyi (Db Class Generator) görebiliriz.

[![tutorialtt16](/assets/images/2012/tutorialtt16_thumb.png)](/assets/images/2012/tutorialtt16.png)

Öğeyi seçtikten sonra ise karşımıza tasarladığımız Windows Forms penceresi çıkacaktır. Eğer doğru Connection String bilgisini girersek (bu örnekte MultipleActiveResultSets değeri için true vermeyi unutmayın) cs içeriğinin otomatik olarak üretildiğine şahit olabilirsiniz

![Smile](/assets/images/2012/wlEmoticon-smile_40.png)

Görüldüğü üzere bir TT projesi VSIX ile bir arada değerlendirildiğinde Visual Studio tarafına Extension yazılması söz konusudur. Biraz uzun ve yorucu bir makale olduğu kadar uygulama sırasında da oldukça dikkat ve titizlik isteyen bir çalışma söz konusudur. Umarım istediğiniz sonuçları sizlerde elde edersiniz. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

> Size tavsiyem otomatik Windows Forms veya ASP.Net Web Pages ya da MVC öğeleri üretecek bir TT üzerinde çalışmanızdır. Ayrıca bu örnekteki Windows Forms’ u hatalara neden olmayacak ve developer’ ın hayatını daha da kolaylaştıracak şekilde geliştirmeyi ciddi anlamda düşünmelisiniz.

[Aşağıdaki örnek Visual Studio 2012 RC sürümü üzerinde ele alınmıştır]

[DbClassGenerator.zip (105,75 kb)](/assets/files/2012/DbClassGenerator.zip)