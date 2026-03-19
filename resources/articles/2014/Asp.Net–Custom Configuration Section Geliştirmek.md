---
title: "Asp.Net–Custom Configuration Section Geliştirmek"
pubDate: 2014-05-19 16:50:00
categories:
  - Asp.Net
tags:
  - Asp.Net
  - C#
  - XML
  - .NET
  - WCF
  - HTTP
  - Authentication
---

# Asp.Net–Custom Configuration Section Geliştirmek
[![Configuration](images/Configuration_thumb.png)](images/Configuration.png) Merhaba Arkadaşlar,

Konfigurasyon tabanlı geliştirme modeli, uygulama kodlarına girilmeden çalışma zamanına yönelik değişiklikler yapabilmemizi sağlar. Bu sayede pek çok programın kodsal müdahale yapmadan davranışları değiştirilebilir..Net dünyasında baktığımızda da, App.Config, Web.config gibi dosyalar içerisinde Framework’ ün geneline yönelik pek çok konfigurasyon ayarı bulunduğu görülür. appSettings, connectionStrings, httpHandler vb…

> Eskilerden: [Asp.Net 2.0 ile Configuration Management (Konfigurasyon Yönetimi)](https://www.buraksenyurt.com/post/Asp-Net-2-0-ile-Configuration-Management-(Konfigurasyon-Yonetimi)-bsenyurt-com-dan)

Söz konusu konfigurasyon içerikleri aslında XML tabanlı bir dosya şemasının parçalarıdır ve doğal olarak element ile attribute’ lardan oluşmaktadır. Konfigurasyon dosyalarının daha iyi yönetilebilmesi için Asp.Net 2.0 ile birlikte Configuration API alt yapısı geliştirilmiştir. Bu kütüphane sayesinde konfigurasyon içerisindeki elementlere sınıf bazında erişmek ve yönetebilmek mümkündür. Pek tabi XML elementlerinin sahip oldukları nitelikler, sınıfların özellikleri (Property) olarak ele alınmaktadır.

Peki konfigurasyon dosyası içerisine kendi özel kısımlarımızı (section) ilave etmek ve hatta bunları çalışma zamanında (Runtime) kullanmak istersek, nasıl bir yol izlememiz gerekir?

![Thinking smile](images/wlEmoticon-thinkingsmile_3.png)

Konfigurasyon Yapısı

Aklımıza ilk gelen belirli tip türetmeleri veya arayüz (interface) uyarlamaları ile bu işin halledilebilecek olmasıdır. Aslında olayı çözümlemek için var olan konfigurasyon parçalarının örnek tip yapısını incelemek yerinde bir hareket olacaktır. Söz gelimi system.web kısımı içerisindeki compilation ve pages elementlerini incelediğimizi düşünelim.

[![ccs_1](images/ccs_1_thumb.png)](images/ccs_1.png)

Eğer object browser üzerinden ilgili elementlerin karşılık geldiği sınıfları incelersek şu sonuçlara varırız.

- system.web elementi SystemWebSectionGroup isimli bir sınıf ile işaret edilmekte olup ConfigurationSectionGroup türevlidir.
- compilation elementi system.web in alt elementidir ve CompilationSection sınıfı ile işaret edilmektedir. Bu sınıf ise ConfigurationSection’ dan türemiştir.
- Benzer şekilde pages elementi de ConfigurationSection türevli PageSection sınıfı ile temsil edilmektedir.
- namespaces elementine baktığımızda aynı tipten birden fazla elementi içerecek şekilde kullanılabildiği görülmektedir. Bunun için NamespaceColletion sınıfı ConfigurationElementCollection türevli olarak tasarlanmıştır.
- namespaces segmenti içerisinde yer alan add elementleri aslında NamespaceInfo tipini işaret etmekte olup yine ConfigurationElement türevlidir.
- alt elementler genellikle üst elementlerin birer özelliği (sınıf bazında düşünüldüğünde) olarak karşımıza çıkmaktadır.

Dolayısıyla kendi geliştireceğimiz özel Section elementleri için de bu tip bir yol izlememiz ve ilgili türetmeleri yapmamız yeterli olacaktır.

Örnek Senaryo

Basit bir senaryo üzerinden ilerleyebiliriz. Örneğin bir web uygulamasının web.config dosyası içerisinde aşağıdaki gibi bir konfigurasyon kısmı oluşturmak istediğimizi düşünelim.

[![ccs_3](images/ccs_3_thumb.png)](images/ccs_3.png)

serviceConnection ve altında yer alan definition elementlerinin işlevselliği çok önemli değildir aslında. Sadece bir kaç nitelik ve alt element içeren bir XML yapısı söz konusu. Bizim yapacağımız basit olarak bu konfigurasyon içeriğini türlendirmek ve çalışma zamanında yönetebilir hale getirmek. Öyleyse işe koyulalım ne duruyoruz.

Sınıfların İnşa Edilmesi

Boş bir Web uygulamasında aşağıdaki sınıf çizelgesinde yer alan tipleri ürettiğimizi düşünelim.

[![ccs_2](images/ccs_2_thumb.png)](images/ccs_2.png)

ServiceConnectionSection tipi dikkat edileceği üzere ConfigurationSection türevlidir ve içerisinde Type isimli ServiceType enum sabiti tipinden bir özellik ile ConfigurationElement türevli olan DefinitionSection tipinden başka bir özellik yer almaktadır.

ServiceConnectionSection sınıfı;

```csharp
using System; 
using System.Configuration;

namespace HowTo_WritingCustomConfigSection 
{ 
    public class ServiceConnectionSection 
       :ConfigurationSection 
    { 
        // Save işlemine izin vermesi için false döndürecek şekilde ezdik 
        public override bool IsReadOnly() 
        { 
            return false; 
        }

        [ConfigurationProperty("type", DefaultValue = ServiceType.WCF, IsRequired = false)] 
        public ServiceType Type 
        { 
            get 
            { 
                return (ServiceType)Enum.Parse(typeof(ServiceType), this["type"].ToString()); 
            } 
            set 
            { 
                this["type"] = value.ToString(); 
            } 
        }

        [ConfigurationProperty("definition",IsRequired=true)] 
        public DefinitionSection Definition 
        { 
            get 
            { 
                return (DefinitionSection)this["definition"]; 
            } 
            set 
            { 
                this["definition"]=value; 
            } 
        } 
    } 
}
```

> Konfigurasyon içeriğinde çalışma zamanında da değişiklik yapılması mümkündür ama konfigurasyon yöneticisinin Save metoduna tepki verebilmesi için IsReadOnly özelliğinin ezilmesi (override) ve false döndürmesi gerekmektedir.

Dikkat edileceği üzere Definition ve Type isimli özelliklere ConfigurationProperty niteliği (Attribute) uygulanmıştır. Bu niteliğe ait özelliklerden yararlanılarak element adı (konfigurasyon dosyasında görünecek olan isim) ve gereklilik (IsRequired) gibi değerler belirtilebilir.

Özelliklerin get ve set bloklarında fark edileceği gibi this anahtar kelimesinden yararlanılmakta ve üst tipin indeksleyicisine (Indexer) gidilerek değer ataması veya okunması işlemi gerçekleştirilmektedir.

> F12 ile ConfigurationSection elementine gidiliğinde bu indexleyici görülebilir.
> [![ccs_6](images/ccs_6_thumb.png)](images/ccs_6.png)

DefinitionSection sınıfı;

```csharp
using System.Configuration;

namespace HowTo_WritingCustomConfigSection 
{ 
    public class DefinitionSection 
        :ConfigurationElement 
    { 
        // Save işlemine izin vermesi için false döndürecek şekilde ezdik 
        public override bool IsReadOnly() 
        { 
            return false; 
        }

        [ConfigurationProperty("name", IsRequired = true)] 
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\,çşöğüı")] 
        public string Name 
        { 
            get 
            { 
                return this["name"].ToString(); 
            } 
            set 
            { 
                this["name"] = value; 
            } 
        }

        [ConfigurationProperty("wsdlAddress", IsRequired = true)] 
        public string WsdlAddress 
        { 
            get 
            { 
                return this["wsdlAddress"].ToString(); 
            } 
            set 
            { 
                this["wsdlAddress"] = value; 
            } 
        } 
    } 
}
```

DefinitionSection sınıfı aslında bir alt elementtir ve bu sebepten ConfigurationElement sınıfından türetilmiştir. Save operasyonuna cevap verebilmesi için IsReadOnly özelliği ezilmiştir. Name özelliğinde ConfigurationProperty dışında StringValidator niteliği de kullanılmış ve kullanılması istenmeyen bir karakter seti belirtilmiştir.

Web.config Bildirimleri

Artık konfigurasyon dosyası içerisinde kullanacağımız serviceConnection section için gerekli tip desteğine sahip bulunmaktayız. Peki web.config dosyası içerisinde bu bildirimleri nasıl gerçekleştirebiliriz?

Bazen 3ncü parti araçları sisteme dahil ettiğimizde, konfigurasyon dosyası içerisine koyacakları elementler için ekstra bildirimler eklediklerine şahit olmuşuzdur. Yani bir şekilde çalışma zamanına, “izleyen config içeriğinde şu tipe ait elementler kullanılabilir” denilebilmelidir. Bunun için configSections elementi içerisinde sectionGroup ve section tanımlamalarını yapmamız yeterli olacaktır. Aşağıda görüldüğü gibi.

```xml
<?xml version="1.0"?> 
<configuration> 
  <configSections> 
    <sectionGroup name="serviceConnectionGroup"> 
    <section 
       name="serviceConnection" 
       type="HowTo_WritingCustomConfigSection.ServiceConnectionSection" 
       allowLocation="true" 
       allowDefinition="Everywhere" 
     /> 
    </sectionGroup> 
  </configSections> 
  <serviceConnectionGroup> 
    <serviceConnection type="MSMQ"> 
      <definition 
        name="Some MSMQ Service" 
        wsdlAddress="msmq://www.azon.com/someservicequeue/inbox" /> 
    </serviceConnection> 
  </serviceConnectionGroup> 
  <system.web> 
      <compilation debug="true" targetFramework="4.5" /> 
      <httpRuntime targetFramework="4.5" />    
    </system.web> 
</configuration>
```

Kural son derece basittir. serviceConnectionGroup, serviceConnection ve definition elementlerinin kullanılabilmesi için bir sectionGroup tanımlaması yapılması yeterlidir. Bu tanımlama içerisinde ki type kısmı ise ConfigurationSection veya ConfigurationSectionGroup türevli tipi işaret etmektedir.

Test Uygulaması

Şimdi dilerseniz basit bir aspx sayfası hazırlayıp section içeriğini ekrana bastıralım ve hatta üzerinde değişiklik yapıp web.config dosyasına kayıt edelim. Senaryonun bu kısmını gerçekleştirmek için aşağıdaki basit aspx sayfasını tasarlayabiliriz.

[![ccs_4](images/ccs_4_thumb.png)](images/ccs_4.png)

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="HowTo_WritingCustomConfigSection.Default" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml"> 
<head runat="server"> 
    <title></title> 
</head> 
<body> 
    <form id="form1" runat="server"> 
    <div>     
        <table> 
            <tr> 
                <td><strong>Service Type</strong></td> 
                <td> 
                    <asp:DropDownList ID="ddlServiceType" runat="server"> 
                    </asp:DropDownList> 
                </td> 
            </tr> 
            <tr> 
                <td><strong>Definition</strong> <strong>Name</strong></td> 
                <td> 
                    <asp:TextBox ID="txtName" runat="server" Width="220px"></asp:TextBox> 
                </td> 
            </tr> 
            <tr> 
                <td><strong>Definition WSDL</strong> <strong>Address</strong></td> 
                <td> 
                    <asp:TextBox ID="txtAddress" runat="server" Width="300px"></asp:TextBox> 
  </td> 
            </tr> 
            <tr> 
                <td> </td> 
                <td> 
                    <asp:Button ID="btnSave" runat="server" 
                        OnClick="btnSave_Click" Text="Save" Width="60px" /> 
                </td> 
            </tr> 
            </table>     
    </div> 
    </form> 
</body> 
</html>
```

Gelelim kod tarafına. Sayfa yüklenirken serviceSection içeriğini göstermek arzusundayız. Ayrıca Save düğmesine basıldığında, yaptığımız değişiklikleri kaydetmek ve web.config dosyasını güncellemek istiyoruz.

> Senaryomuzda exceptional durumları göz ardı ettiğimizi ifade etmek isterim. Örneğin, boş değer geçilmesi, geçeriz bir url bildirimi yapılması vb. Ancak gerçek hayat senaryolarında bu tip veri doğrulama opsiyonlarını da işin içerisine katmanız önem arz etmektedir.

```csharp
using System; 
using System.Configuration; 
using System.Web.Configuration; 
using System.Web.UI;

namespace HowTo_WritingCustomConfigSection 
{ 
    public partial class Default 
        : System.Web.UI.Page 
    { 
        protected void Page_Load(object sender, EventArgs e) 
        { 
            if (!Page.IsPostBack) 
            { 
                ddlServiceType.DataSource = Enum.GetNames(typeof(ServiceType)); 
                ddlServiceType.DataBind();

                ServiceConnectionSection scs = 
                    ConfigurationManager.GetSection("serviceConnectionGroup/serviceConnection") 
                    as ServiceConnectionSection;

                if (scs != null) 
                { 
                    txtAddress.Text = scs.Definition.WsdlAddress; 
                    txtName.Text = scs.Definition.Name; 
                    ddlServiceType.SelectedValue = scs.Type.ToString(); 
                } 
            } 
        }

        protected void btnSave_Click(object sender, EventArgs e) 
        { 
            Configuration manager = WebConfigurationManager.OpenWebConfiguration("/");

            ServiceConnectionSection scs = 
                manager.GetSection("serviceConnectionGroup/serviceConnection") 
                as ServiceConnectionSection; 
            if (scs != null) 
            { 
                scs.Type = (ServiceType)Enum.Parse(typeof(ServiceType),  ddlServiceType.SelectedValue.ToString()); 
                scs.Definition.Name = txtName.Text; 
                scs.Definition.WsdlAddress = txtAddress.Text;

                manager.Save(); 
            } 
        } 
    } 
}
```

serviceSection elementinin managed karşılığını elde edebilmek için Configuration veya ConfigurationManager tipinin GetSection metodundan yararlanılmaktadır. Söz konusu metodun dönüşü ServiceConnectionSection tipine dönüştürüldükten sonra ise Type ve Definition gibi özelliklere erişilebilinir. Hatta Definition özelliği üzerinden Name ve WsdlAddress değerleri de yakalanabilir. Pek tabi Save işleminin gerçekleştirilebilmesi için WebConfigurationManager ile açılan web.config dosyasını işaret eden manager isimli Configuration tipinden yararlanılmaktadır.

Çalışma Zamanı Sonuçları

Artık çalışma zamanına geçebilir ve sonuçları irdeleyebiliriz. Uygulamayı ilk olarak başlattığımızda PageLaod içerisindeki kodlar devreye girecektir.

[![ccs_5](images/ccs_5_thumb.png)](images/ccs_5.png)

Görüldüğü gibi varsayılan olarak belirtilen değerler çekilebilmiştir. Eğer bu noktada tip, ad ve adres bilgilerinde değişiklik yapıp Save düğmesine basılırsa, kod web.config dosyasında da gerekli etkiyi yapacaktır.

Tabi bu senaryoda çok basit bir section içeriği ele alınmış ve yönetilmiştir. Size tavsiyem connectionStrings gibi bir den fazla aynı tipden element içerebilen bir bölüm geliştirmeye çalışmanız olacaktır. Böyle bir senaryoda devreye ConfigurationElementCollection tipi girecektir. Bu örnek senaryoyu geliştirmeye çalışarak kendinizi bu yönde daha ileri bir noktaya taşıyabilirsiniz. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[HowTo_WritingCustomConfigSection.zip (25,98 kb)](assets/HowTo_WritingCustomConfigSection.zip)
