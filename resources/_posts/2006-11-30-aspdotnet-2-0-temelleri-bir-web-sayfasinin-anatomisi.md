---
layout: post
title: "Asp.Net 2.0 Temelleri : Bir Web Sayfasının Anatomisi"
date: 2006-11-30 10:00:00 +0300
categories:
  - aspnet
tags:
  - asp.net
---
Bu makalemizde, bir web sayfasının (.aspx uzantılı dosyalar) anatomosini incelemeye çalışacak, kaynak koddaki özel noktaları, in-line coding, code-behind modelini, yaşam döngüsünü ve çalışma zamanında olay bağlanması gibi temel kavramlara değineceğiz. Böylece basit olarak bir web sayfasının anatomisini öğrenmek için gerekli ip uçlarını değerlendirme fırsatını bulmuş olacağız. İlk olarak basit bir web sayfasını göz önüne alarak başlayalım.

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <title>Untitled Page</title>
            </head>
                <body>
                    <form id="form1" runat="server">
                        <div>
        
                        </div>
                    </form>
                </body>
</html>
```

Bu sayfa üzerinden konuşulabileceğimiz oldukça fazla özellik bulunmaktadır. Yukarıdaki kod parçası Visual Studio.Net 2005 tarafından üretilen defualt.aspx isimli bir web sayfasının içeriğini göstermektedir. Her web sayfası mutlaka Page direktifi ile başlar. Page direktifinin sahip olduğu nitelikler sayfa için gerekli olan çalışma zamanı ve geliştirme zamanı davranışlarını belirleyen değerler içerir. Örneğin CodeFile niteliği, bu sayfa için çalışma zamanında değerlendirilecek ve özellikle arka kodların (code-behind) tutulacağı dosyanın adını belirtir.

Inherits isimli nitelik ile, üretilecek olan çalışma zamanı sayfasının hangi tipten türetileceği söylenmektedir. Language niteliği bu sayfada C# dili ile geliştirme yapılacağını vurgular. Dikkatimizi çeken bir diğer nitelik olan AutoEventWireup'a ise makalemizin sonunda değineceğiz. Ama öncesinde sayfamıza bir Button kontrolü alıp buna ait bir Click olay metodu yazalım. Web sayfasına tasarım zamanında bir Button kontrolü sürüklersek, ilgili kontrolün form takısı içerisine alındığını görürüz. Eğer Button kontrolümüze çift tıklarsak (yada, event'lerinden Click olayına çift tıklarsak), default.aspx.cs isimli code-behind dosyasında, Button bileşenine ait ilgili olay metodunun otomatik olarak oluşturulduğunu görürüz.

default.aspx içerisindeki değişiklik

```text
<form id="form1" runat="server">
    <div>
        <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click"/>
    </div>
</form>
```

defaulf.aspx.cs

```csharp
public partial class _Default : System.Web.UI.Page 
{
    protected void Button1_Click(object sender, EventArgs e)
    {
    }
}
```

Burada en çok dikkat etmemiz gereken nokta, _Default sınıfının Page'den türemesi ve partial bir tip olmasıdır. Bunun önemini ve oluşturduğu farkı görmek için aynı uygulamanın Asp.Net 1.1 versiyonuna bakmamız gerekecektir. Aşağıdaki kod parçaları Visual Studio.Net 2003 ile tasarlanmıştır ve aynı senaryoyu ele almaktadır.

WebForm1.aspx

```text
<%@ Page language="c#" Codebehind="WebForm1.aspx.cs" AutoEventWireup="false" Inherits="HelloAspNet1.WebForm1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
<HEAD>
<title>WebForm1</title>

<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
<meta name="CODE_LANGUAGE" Content="C#">
<meta name="vs_defaultClientScript" content="JavaScript">
<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
</HEAD>
<body MS_POSITIONING="GridLayout">
    <form id="Form1" method="post" runat="server">
        <asp:Button id="Button1" runat="server" Text="Button"></asp:Button>
    </form>
</body>
</HTML>
```

WebForm1.aspx.cs code behind dosyası

```csharp
public class WebForm1 : System.Web.UI.Page
{
    protected System.Web.UI.WebControls.Button Button1;

    private void Page_Load(object sender, System.EventArgs e)
    {
    }

    #region Web Form Designer generated code
    override protected void OnInit(EventArgs e)
    {
        InitializeComponent();
        base.OnInit(e);
    }

    private void InitializeComponent()
    { 
        this.Button1.Click += new System.EventHandler(this.Button1_Click); 
    }
    #endregion

    private void Button1_Click(object sender, System.EventArgs e)
    {
    }
}
```

Gördüğünüz gibi Asp.Net 1.1' de sayfaya bir kontrol eklendiğinde, code-behind dosyası içerisinde Asp.Net 2.0' dakine göre daha farklı işlemler yapılmaktadır. Herşeyden önce Code-Behind sayfamızda, Button1 adında bir Button nesnesi tanımlanmıştır. Oysaki Asp.Net 2.0 ile geliştirdiğimiz sayfa içerisinde bu tarz bir tanımlama söz konusu değildir.

```csharp
protected System.Web.UI.WebControls.Button Button1;
```

Diğer taraftan Button1 isimli nesne için bir Click event'i Initialize Component metodu içerisinde yüklenmiş ve Button1_Click isimli olay metoduna bağlanmıştır. Initialize Component metodu aslında windows formlarında da benzer bir görev üstlenir. Yani form üzerindeki kontrollerin oluşturulması gerekli olaylarının bağlanması ve formun controls koleksiyonuna dahil edilmesi gibi. Elbette web de durum biraz daha farklı olmasına rağmen yinede kontrollerin oluşturulması gibi bir durum söz konusudur. Asp.Net 1.1 bu işi gerçekleştirmek için sayfanın OnInit isimli olay metodunu göz önüne almaktadır.

```csharp
this.Button1.Click += new System.EventHandler(this.Button1_Click);
```

Oysaki Asp.Net 2.0 mimarisinde her sayfanın çalışma zamanında (run-time) derlenmesi (compile) söz konusudur. Bu işlem nedeni ile, bir nesne ve olaylarına ait metodlar çalışma zamanında dll üretimi sırasında otomatik olarak birbirlerine bağlanırlar. Bu Asp.Net 2.0 ile web sayfalarına gelen en önemli yeniliklerden birisidir. Dolayısıyla, sayfa üzerine yerleştireceğimiz bileşenleri ayrıca tanımlamamıza, bunalara ait olayları yüklemek için temsilcilere kadar gitmemize gerek kalmamaktadır. Özellikle.Net 2.0 ile gelen tiplerin parçalara ayrılabilmesinin (Partial Types Modeli) bu yeni çalışma modelinde önemli bir yeri vardır. Bu çalışma sistemini daha net anlayabilmek amacıyla aşağıdaki şekli göz önüne alabiliriz.

![mk182_1.gif](/assets/images/2006/mk182_1.gif)

Asp.Net 2.0 için çalışma zamanında üretilen parçalı bir sınıf söz konusudur. Bu sınıf var olan aspx sayfasından üretilir ve code-behind dosyasında yer alan sınıf ile birleştirilir. Her iki sınıf partial olarak tanımlanmış olduğundan aslında ortada tek bir sınıf vardır. Sonuç olarak üretilen asıl sınıf, bileşenlerinin tanımlamalarını, ilgili olayları ile olan bağlantılarını vb... içeren bir yapıya sahiptir. Bu sınıfa ait nesne örnekleri istemciden gelen talepler sonrası üretilir, yaşam döngüsünü geçer ve istemciye gönderilemek üzere bir html çıktısının üretilmesinde kullanılır.

![dikkat.gif](/assets/images/2006/dikkat.gif)
Asp.Net 2.0' da sayfa üzerindeki kontroller ve bu kontrollere ait olayların bağlanması gibi işlemler çalışma zamanında gerçekleştirilir. Böylece kod tarafında olay yükleme ve kontrol tanımlama gibi kodu kalabalıklaştıran işlemlerden uzaklaşılabilinmektedir.

Gelelim web sayfalarının anatomisindeki bir diğer konuya. Kodlamayı nerelerde yapabiliriz? Asp.Net iki tür kodlama şekli sunmaktadır. Bunlardan birisi Inline-Coding diğeri ise Code-Behind modelidir. Inline-Coding modelinde, aspx sayfasının kaynak içeriği ile kod kısmı aynı fiziki dosya içerisinde yer alırlar. Örneğin aşağıdaki kod parçasında Inline-Coding modeli kullanılmaktadır.

```csharp
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">
    protected void Button1_Click(object sender, EventArgs e)
    {
    } 
</script>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click"/>
        </div>
    </form>
</body>
</html>
```

Inline coding modelinde dikkat ederseniz, sayfanın kodlarını aspx sayfası içerisinde yer alan script blokları içerisinde yapmaktayız. Script bloğu içerisinde runat="server" niteliğinin kullanılmasının en büyük nedeni ise bu blok içerisinde çalıştırılacak olan kodların sunucu tarafında ele alınacak olmasıdır. Visual Studio.Net 2005 bir önceki sürümüne göre, Inline-Coding sırasında intelli-sense için tam destek vermektedir. Aşağıdaki ekran çıktısına dikkat ederseniz script bloğu içerisinde.net tipleri için intelli-sense özelliğinin tam olarak çalıştığını görebiliriz.

![mk182_2.gif](/assets/images/2006/mk182_2.gif)

Inline-Coding'in en önemli avantajlarından birisi kaynak html içeriği ile kodların tek ve aynı fiziki dosya içerisinde yer almasıdır. Bu sayfanın dağıtılmasını (deployement) son derece kolaylaştırır. Ayrıca sayfanın yeninden isimlendirilmesi başka bir kod sayfasına bağımlılık olmadığından daha kolaydır.

![dikkat.gif](/assets/images/2006/dikkat.gif)
Microsoft, kodlama işlemlerinde Code-Behind modelinin kullanılmasını önermektedir.

Gelelim Code-Behind modeline. Bu modelde, kodlarımızı içerisinde barındırıdan ayrı bir fiziki dosya söz konusdur. Aşağıdaki kod parçasında bu model gösterilmektedir.

Aspx tarafında

```text
<form id="form1" runat="server">
        <div>
            <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click"/>
        </div>
    </form>
```

Code Behind tarafında

```csharp
public partial class _Default : System.Web.UI.Page 
{
    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Write("Deneme");
    } 
}
```

Code-Behind programlamanın en önemli artısı, kod tarafı ile sunum tarafının kesin olarak birbirlerinden ayrılıyor olmasıdır. Bu nedenle grafik departmanı ile kodlamacıların aynı sayfa üzerinde çalışmaları çok daha kolay olabilmektedir.

Web sayfalarının anatomisinde inceleyeceğimiz bir diğer konuda form takısının görevidir. Bildiğiniz gibi bir istemci web sunucusu üzerinden herhangibir sayfayı ilk talep ettiğinde, sunucu tarafında bir dizi işlem gerçekleştirilir.

![dikkat.gif](/assets/images/2006/dikkat.gif)

Aslında ilk talep Http protokolünün Get metoduna göre gerçekleşir. Talep edilen sayfa sunucuda önce IIS tarafından karşılanır ve Asp.Net sayfası olup olmadığına bakılır. Eğer Asp.Net sayfası ise talep ASPNETISAPI.dll'ine devredilir. Bu dll, Asp.Net web uygulamalarını çalıştıran ve yöneten Asp.Net work processor ile IIS arasındaki iletişimi sağlamakla görevlidir. AspNet Work Processor, talep edilen sayfanın bulunduğu web uygulamasının bir application domain içerisine yüklenmesinden, CLR'a açılmasından sorumludur. Eğer talep edilen sayfaya ait web uygulaması için bir Application Domain açılmamışsa, Asp.Net Work Processor bu işlemide üstlenir. Bu şu anlamada gelir; eğer Application Domain yüklü ise bu adım otomatikman atlanır.
Gelen talebe ait sayfanın çalıştırılması, belleğe alınması, html çıktısının üretilmesi gibi işlemler HTTPPIPE adı verilen bir.Net sınıflar koleksiyonun sorumluluğu altındadır. Tabi talep edilen sayfaya ait bir dll daha önceden işletim sisteminin ilgili temp klasörlerine atılmışsa buradaki dll üretme gibi işlemler otomatikman atlanır ve sayfanın doğrudan çalıştırılmasına geçilir. Yani HTTPPIPE talep edilen içerik için dll oluşturma işleminide kontrol altına alır. Üretilen çıktı istemcinin görmesi gereken içeriktir ve aynı yollar ile IIS'e dönerek buradan istemciye gönderilir.

Talep sonrası gerçekleşen arka plan işlemlerinin sonucunda ilgili sayfanın HTML çıktısının istemciye gönderildiğini düşünebiliriz. İstemci bu sayfa üzerinden gelen kontrollerde veri girişleri yapabilir. Bu veri girişlerinden sonra sayfayı tekrardan sunucuya gönderebilir-ki biz buna post-back işlemi diyoruz. İşte sayfanın içerisindeki veriler ile birlikte sunucuya gönderilmesi aşamasında standard olarak HTTP protokolünün Post metodu kullanılır.

Bu işlemde HTTP paketi içerisinde sunucuya doğru hareket edecek olan içerik, sayfadaki form takısı içerisinde kullanılan elemanlar için geçerli olacaktır. Kısacası, HTTP Post tekniğine göre sayfa içerisindeki veriler istemciden sunucuya doğru HTTP paketi içerisinde gönderilirler. Oysaki HTTP protkolünün Get isimli bir başka metodu daha vardır. Bu metoda göre form takısı içerisindeki elemanlara ait içerik Url üzerinden bir başka deyişle tarayıcı pencersinin adres satırı üzerinden sunucuya gönderilecektir. Şimdi bu durumu analiz etmek için default.aspx sayfamızı aşağıdaki gibi değiştirelim.

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<script runat="server">

    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Write(txtAd.Text);
    } 

</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server" method="get">
        <div>
            <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click"/>
            <asp:TextBox ID="txtAd" runat="server" Text="Adınız" />
        </div>
    </form>
</body>
</html>
```

İstemci bu sayfayı talep ettikten sonra TextBox kontrolüne veri girişi yapıp, sayfayı tekrar sunucuya gönderdiğinde ViewState, TextBox ve Button nesnelerine ait içerikler url üzerinden sunucuya gönderilmektedir. Bunu aşağıdaki ekran görüntüsünden ve Url satırındaki kod kısmından daha net görebilirsiniz.

![mk182_3.gif](/assets/images/2006/mk182_3.gif)

```text
http://localhost:1503/Anatomi/Default.aspx?__VIEWSTATE=%2FwEPDwUKMTM0MDE0NzQwOGRkccXHiCjUXf6SyoYO18eFOly9GKU%3D&Button1=Button&txtAd=Burak&__EVENTVALIDATION=%2FwEWAwKYvOnjDAKM54rGBgKM%2B%2FaQCjAN0uAoGcNSpMV1ikOzE6pJ9Fnd
```

Dikkat ederseniz, TextBox kontrolünün içeriğini Url içerisinde net olarak görebilmekteyiz. Http Get modeline göre sayfa içeriklerini sunucuya göndermek,özellikle bilgilerin açık olarak Url üzerinden gidiyor olması ve tarayıcıların Url üzerinden gönderebilecekleri karakter sayısının bir sınırının olması gibi nedenlerden dolayı çok tercih edilen bir yol değildir. Lakin, parametrik değerlerin Url satırından değiştirilerek farklı sonuçların elde edilebilmesi gibi bir imkanda söz konusudur ki güvenlik göz önüne alındığında bununda bir avantaj olmadığı düşünülebilir.

Web sayfamıza ait anatomide inceleyeceğimiz bir diğer önemli unsurda yaşam döngüleridir. Aslında her web sayfası sunucu tarafında bir.Net sınıfıdır. Çünkü içeriklerinde başka sınıflara ait örnekler, üye metodlar, olaylar, özellikler vb... vardır. Dahası her web sayfası sunucu tarafında üretilmekte ve işlenmektedir. Yani istemciler bir web sayfasını talep ettiklerinde sunucunun yapacağı iş, talep edilen sayfanın örneğini oluşturmak ve yürütmek olacaktır. Dolayısıyla bu süreç sırasında her web sayfasının ele alınabilecek bir yaşam döngüsü olduğu sonucuna varabiliriz. Standard olarak bir web sayfasının üretilmesi aşamasındaki yaşam döngüsünü tüm ayrıntılarıyla izlemek istersek Trace mekanizmasını kullanabiliriz. Trace konusuna detaya girmeyecek olsakta, yaşam döngüsü içerisindeki adımları görmemiz açısından bir örnek yapmamız gerektiğini düşünüyorum. Trace'i aktif hale getirmek için Web.config dosyasına aşağıdaki elemanı eklememiz yeterli olacaktır.

![mk182_4.gif](/assets/images/2006/mk182_4.gif)

Buna göre Trace bilgileri ilgili web uygulamasındaki her sayfanın sonunda (pageOutput=true) ve yanlızca sunucu bilgisayar üzerindeki isteklerde (localOnly="true") görünecek şekilde aktif hale getirilmektedir. Şimdi default.aspx sayfamızı tarayıcı pencersinden talep edersek aşağıdaki çıktıyı elde ederiz.

![mk182_5.gif](/assets/images/2006/mk182_5.gif)

Bu çıktyı sayfamızı ilk talep ettiğimizde elde ederiz. Peki Button kontrolümüze basıp sayfayı istemciden sunucuya gönderdiğimizde (post-back) yaşam döngüsü nasıl işleyecektir? Herşeyden önce sayfa üzerindeki verilerin istemciden gelmesiyle birlikte post edilen dataların ve viewstate verilerinin işlendiği başka olaylarda tetiklenecek ve işletilecektir. Bu durumda trace bilgilerimiz aşağıdaki gibi olacaktır.

![mk182_6.gif](/assets/images/2006/mk182_6.gif)

Temel olarak buradaki olaylardan bazılarını sayfamız içerisinde ele alabilir ve sayfanın yaşam döngüsü içerisinde kendi isteklerimizi kodlayarak yürütebiliriz. Bir web sayfasının yaşam döngüsünde ele alınabilecek olayları daha basit olarak düşündüğümüzde aşağıdaki sıralamayı ele alabiliriz.

```text
Page_PreInit
Page_Init
Page_Load
   Button için Click / Change Olay Metodları (Eğer tetiklenmişler ise)
Page_PreRender
Page_UnLoad
Dispose (Eğer override edilmişse)
```

Burada dikkat ederseniz PageLoad ve PagePreRender olayları arasında sayfa üzerindeki bileşenlere ait Click ve Change olaylarının yer aldığını görürüz. Yukarıdaki Trace çıktısında bunları tam olarak göremesekte Button_Click olay metodu içerisine aşağıdaki kodu yazarak izleme şansına sahip olabiliriz.

```csharp
Trace.Warn("Button Click olay metodu çalışıyor..."); // Trace' e kırmızı renkte bilgi yazdırıyoruz.
```

Bu kodu çalıştırdığımızda Trace çıktısında aşağıdaki görüntüyü elde ederiz.

![mk182_7.gif](/assets/images/2006/mk182_7.gif)

Buradaki çıktının Button kontrolüne basıp sayfayı sunucuya gönderdikten sonra oluştuğuna dikkat edelim.

![dikkat.gif](/assets/images/2006/dikkat.gif)
Sonuç olarak talep edilen her web sayfası, sunucu tarafında bir nesne olarak üretilir, çalıştırılır, HTML çıktısı alınır ve yok edilir (Dispose).

Bu olaylar içerisinde özellikle dikkate değer olan iki tanesi vardır. Page_UnLoad ve Dispose. Page_UnLoad sayfaya ait Html çıktısı üretildikten sonra çalışır. Bu nedenle burada HTML çıktısına müdahale etme şansımız artık kalamamaktadır. Dolayısıyla, sayfa içerisinde kullanılan başka managed kaynakların kapatılması ve sisteme iade edilmesi için ideal bir olay metodudur. Diğer taraftan Dispose metodu Page sınıfı içerisinde override edildiği takdirde, Unmanaged kaynakların sisteme iade edilmesi için biçilmiş kaftandır.

Yaşam döngüsünde yer alan bazı olayları kod tarafında ele alabileceğimizden bahsetmiştik. Web sayfasına ait olay metodları ile (ki bunların yaşam döngüsü içerisinde önemli bir rolü vardır), sayfanın Page direktifinde yer alan AutoEventWireup niteliğinin değeri arasında önemli bir ilişki vardır. Örneğin aşağıdaki kod parçasını ele alalım.

```text
<%@ Page Language="C#" AutoEventWireup="false" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<script runat="server">

    protected void Page_PreInit(object sender,EventArgs e)
    {
        Response.Write("PreInit olay metodu çalıştı...");
    } 

</script>
```

Default.aspx sayfasını çalıştırdığımızda PreInit isimli olay metodunun işlemediğini görürüz. Bunun sebebi varsayılan olarak true değerine sahip olan AutoEventWireup özelliğine false değerini atamamış olmamızdır. Buradan şu sonuca varabiliriz. Sayfaya ait olay metodlarının otomatik olarak bağlanmasını sağlamak için AutoEventWireup özelliğinin değerinin true olması gerekmektedir. Nitekim bu değeri true yaptığımızda PreInit olay metodunun çalıştığını görebiliriz.

![mk182_8.gif](/assets/images/2006/mk182_8.gif)

Peki AutoEventWireup niteliğine false değerinin atanabilmesinin nasıl bir katma değeri olabilir. Bunun için aşağıdaki kod parçasını göz önüne alabiliriz.

![mk182_9.gif](/assets/images/2006/mk182_9.gif)

Bu kod parçasına göre InitOncesi isimli olay metodumuz sayfanın PreInit olayı gerçekleştiği zaman çalıştırılacak metod olacaktır. AutoEventWireup niteliği Asp.Net 1.1 versiyonunda varsayılan olaran false değerine sahip olan bir niteliktir. Sayfa olaylarını kendi isimlendirdiğimiz metodlar ile ilişkilendirmekten başka bir avantaj sağladığını düşünmemiz ne yazıkki pek mümkün değildir.

Gördüğünüz gibi herhangibir tasarımı olmayan hatta önemli bir iş yapmayan bir web sayfası üzerinde konuşulabilecek oldukça fazla konu vardır. Bu makalemizde bir web sayfasının anatomisini ve göze çarpan noktalarını incelemeye çalıştık. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.