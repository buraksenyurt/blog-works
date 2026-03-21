---
layout: post
title: "Microsoft PDC İçeriğini OData Servisi Üzerinden Elde Etmek"
date: 2010-11-17 22:18:00 +0300
categories:
  - wcf-data-services
tags:
  - windows-communication-foundation
  - odata
  - wcf-data-services
  - open-data-protocol
---
Bildiğiniz üzere bir süre önce Microsoft PDC 2010 etkinlikleri gerçekleştirildi. Online olaraktan da canlı izleyebildiğimiz sunumlarda Microsoft’ un çok değerli sunumlarına ve anlatımlarına şahit olduk. Her PDC konferansında olduğu gibi bu sene yapılan etkinliklere ait görüntü kayıtları, Download edilmeye açıldıkları andan itibaren de ilgi odağı oldular

[![blg240_Giris](/assets/images/2010/blg240_Giris_thumb.gif)](/assets/images/2010/blg240_Giris.gif)


![Winking smile](/assets/images/2010/wlEmoticon-winkingsmile_10.png)

İlginç olan noktalardan birisi ise, PDC’ de sunulan içeriklerin ve detaylı bilgilerinin Open Data Protocol (ODATA) formatında ve bir WCF Data Service aracılığıyla dış dünyaya sunuluyor olmasıydı.

Aslına bakarsanız bu tip bir veri paylaşımı benim gibi servis tarafı ile ilgilenen pek çok geliştirici için tek bir anlama gelmektedir: “Git kendi uygulamanı yaz ve PDC Session bilgilerini servis aracılığıyla çek”

![Open-mouthed smile](/assets/images/2010/wlEmoticon-openmouthedsmile_8.png)

İşte Kurban bayramının ortasında olduğumuz şu günlerde ele aldığımız blog yazımızın konusu da bu olacak.

İlk olarak PDC 2010’ a ait bilgilerin nereden yayınlandığına bakarak başlamamızda yarar olacağı kanısındayım. Şu an itibariyle [http://odata.microsoftpdc.com/ODataSchedule.svc/](http://odata.microsoftpdc.com/ODataSchedule.svc/) adresinden bir paylaşım yapılmaktadır. Hatta her hangibir tarayıcı uygulaması ile baktığımızda aşağıdaki Atom veri içeriğinin üretildiğini görebiliriz.

[![blg240_Browser](/assets/images/2010/blg240_Browser_thumb.gif)](/assets/images/2010/blg240_Browser.gif)

Servis tarafından çekilen bu içerik sanıyorum ki WCF Data Service geliştiren veya kullananlara tanıdık gelecektir. Aslında bir anlamda tarayıcı üzerinden sorgulanabilir veri içeriğinin söz konusu olduğunu ifade edebiliriz. Söz gelimi

[http://odata.microsoftpdc.com/ODataSchedule.svc/Sessions?$select=ShortTitle,ShortUrl,FullDescription,Tags&$orderby=ShortTitle](http://odata.microsoftpdc.com/ODataSchedule.svc/Sessions?$select=ShortTitle,ShortUrl,FullDescription,Tags&$orderby=ShortTitle)

şeklinde bir URL sorgulamasının sonucu olarak ShortTitle, ShortUrl, FullDescription ve Tags bilgilerinden oluşan, ayrıca ShortTitle içeriğine göre A’ dan Z’ ye sıralı olarak gelen bir listeyi elde ederiz. Aynen aşağıdaki ekran görüntüsünde olduğu gibi.

[![blg240_SampleQuery](/assets/images/2010/blg240_SampleQuery_thumb.gif)](/assets/images/2010/blg240_SampleQuery.gif)

Dikkat edileceği üzere gerçekleştirilen oturumlara ait detaylı bilgileri bu servis üzerinden tedarik edebiliriz. Söz gelimi Download edilebilir materyallere ait bağlantı adreslerini (Powerpoint Sunumlar, WMV ve MP4 dosyaları), konuşmacılara ait kısa öz geçmişleri, oturumlar ile ilişkili başlık, açıklama, kategori ve daha pek çok bilgiyi elde etme şansına sahibiz. Yazıyı hazırlarkenki ilk amacımız Download edilebilir içeriklere ulaşmaktır. Bunun için Sessions koleksiyonundan yararlanmamız yeterli olacaktır. Ancak tabiki de diğer Entity Set içeriklerini de değerlendirip çok daha detaylı bir arayüz uygulaması geliştirebilirsiniz

![Winking smile](/assets/images/2010/wlEmoticon-winkingsmile_10.png)

(Hatta sıkı takipçilerinden olduğum Mike Taulty’ nin daha geçen günlerde [yayınlamış](http://feedproxy.google.com/~r/mtaulty/~3/xQwMPWbDX9w/pdc-2010-session-downloader-in-silverlight.aspx) olduğu oldukça iddiali bir Silverlight uygulaması da söz konusudur)

Örneği bir Web uygulaması olarak geliştirebiliriz. (Web uygulamasını tercih etmemin en büyük sebeplerinden birisi de, Download Link’ lerine doğal destek verecek olmasıdır) İlk etapta PDC için geliştirilmiş WCF Data Service’ inin projeye referans edilmesi gerekmektedir. Aynen aşağıdaki ekran görüntüsünde olduğu gibi.

[![blg240_AddSrvRef](/assets/images/2010/blg240_AddSrvRef_thumb.gif)](/assets/images/2010/blg240_AddSrvRef.gif)

Dikkat edileceği üzere servis tarafından ScheduleModel isimli bir Context tipi getirilmektedir. PDC isimli namespace altında yer alacak Proxy bileşeninin üretimi sonucu Solution içerisine açılan tiplerin sınıf diagramı görüntüsü de aşağıdaki gibi olacaktır.

[![blg240_ClassDiagram](/assets/images/2010/blg240_ClassDiagram_thumb.gif)](/assets/images/2010/blg240_ClassDiagram.gif)

DataServiceContext türevli olan ScheduleModel içerisinden standart LINQ (Language INtegrated Query) sorgularını kullanarak ilerleyebilir ve özellikle Sessions özelliği ile ifade edilen koleksiyon içeriğini ele alabiliriz. Bu andan itibaren kodlama tarafında farklı şekillerde ilerlememiz de mümkündür. Söz gelimi istediğimiz veri içeriklerini bir Web User Control üzerinde toplayabilir veya GridView gibi veri-bağlı kontrollerden birisine servis yardımıyla çekebiliriz.

Biz örneğimizde Web User Control tipini kullanarak ilerlemeye gayret edeceğiz. Hatta iki adet Web User Control tasarlayacağımızı ifade edebilirim. Bunlardan birisi Sessions ile ilişkili Title, Description,Tags, Thumbnail Photo bilgilerini taşıyor olacak. Diğer kontrolümüz ise Download edilebilir içeriğe ait Title ve en önemlisi de indirme işlemi için gerekli bağlantı bilgilerini barındırıyor olacak. İşte SessionInfo isimli ilk ascx bileşenimiz.

SessionInfo.ascx

```text
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SessionInfo.ascx.cs" Inherits="PDC2010.SessionInfo" %> 
<style type="text/css"> 
    .style1 
    { 
        width: 100%; 
    } 
    .style3 
    { 
        width: 84px; 
        font-weight: bold; 
    } 
</style>

<table cellpadding="3" cellspacing="1" class="style1" frame="box" 
    style="font-size: small"> 
    <tr> 
        <td class="style3" rowspan="4" valign="top"> 
            <asp:Image ID="imageThumbnailPhoto" runat="server" /> 
        </td> 
        <td class="style3"> 
            Title</td> 
        <td> 
            <asp:Label ID="labelTitle" runat="server" ForeColor="#FF3300" Text="Label"></asp:Label> 
        </td> 
    </tr> 
    <tr> 
        <td class="style3"> 
            Description</td> 
        <td> 
            <asp:Label ID="labelDescription" runat="server" Text="Label"></asp:Label> 
        </td> 
    </tr> 
    <tr> 
        <td class="style3" valign="top"> 
            Tags</td> 
        <td> 
            <asp:Label ID="labelTags" runat="server" ForeColor="#999966" Text="Label"></asp:Label> 
        </td> 
    </tr> 
    <tr> 
        <td class="style3" valign="top"> 
            Downloads</td> 
        <td> 
            <asp:PlaceHolder ID="holderLinks" runat="server"></asp:PlaceHolder> 
        </td> 
    </tr> 
</table>
```

ve kod

```csharp
using System.Web.UI.WebControls;

namespace PDC2010 
{ 
    public partial class SessionInfo 
        : System.Web.UI.UserControl 
    { 
        public string Title 
        { 
            set { labelTitle.Text = value; } 
        } 
        public string Description 
        { 
            set { labelDescription.Text = value; } 
        } 
        public string ThumbnailLink 
        { 
            set { imageThumbnailPhoto.ImageUrl = value; } 
        } 
        public string Tags 
        { 
            set { labelTags.Text = value; } 
        } 
        public PlaceHolder LinkHolder 
        { 
            get{return holderLinks;} 
        } 
    } 
}
```

SessionInfo kontrolü içerisinde Title, Description, Tags bilgilerini tuttuğumuz Label bileşenlerinin Text değerlerine erişmek için bir kaç Property kullanılmaktadır. Bununla birlikte Thumbnail resmi için de bir Image kontrolü kullanılmakta ve bu kontrolün ImageUrl özelliğine ThumbnailLink üzerinden değer atanması sağlanmaktadır.

Her bir Session için n sayıda Download Link söz konusu olabilir. MP4, WMV gibi formatlardaki içeriklere ait bağlantı bilgileri Content Entity tipi üzerinde tutulmaktadır. Bu bilgilere erişildikten sonra yine bir Web User Control bileşeninden yararlanılmakta ve söz konusu bileşene ait çalışma zamanı kontrol örnekleri, LinkHolder özelliği ile, SessionInfo üzerindeki PlaceHolder bileşeninin Controls koleksiyonuna eklenmektedir. ContentLink olarak adlandırdığımız bu bileşenin içeriği ise aşağıdaki gibidir.

```text
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContentLink.ascx.cs" Inherits="PDC2010.ContentLink" %> 
<style type="text/css"> 
    .style1 
    { 
        width: 100%; 
    } 
    .style2 
    {} 
    .style6 
    { 
        font-style: italic; 
        font-weight: bold; 
        width: 78px; 
    } 
    .style7 
    { 
        width: 87%; 
    } 
</style>

<table cellpadding="3" cellspacing="1" class="style1"> 
    <tr> 
        <td class="style6"> 
            Title</td> 
        <td class="style7"> 
            <i> 
            <asp:Label ID="labelTitle" runat="server" Text="Label"></asp:Label> 
            </i> 
        </td> 
    </tr> 
    <tr> 
        <td class="style2" colspan="2"> 
            <asp:HyperLink ID="linkDownloadUrl" runat="server">Download</asp:HyperLink> 
        </td> 
    </tr> 
</table>
```

ve kod içeriği;

```csharp
using System;

namespace PDC2010 
{ 
    public partial class ContentLink : System.Web.UI.UserControl 
    { 
        public string Title 
        { 
            set { labelTitle.Text = value; } 
        } 
        public string Url 
        { 
            set {linkDownloadUrl.NavigateUrl= value;} 
        } 
    } 
}
```

Görüldüğü üzere ContentLink bileşeni üzerinde Title ve Url bilgileri tutulmakta olup bunların ilgili özelliklerine erişim için yine Property’ lerden yararlanılmaktadır.

Bu kontroller sayesinde WCF Data Service tarafına gönderilen LINQ sorguları sonrası yüklenecek olan veri içeriklerinin, görsel bileşen bazındaki karşılıkları da tasarlanmış olmaktadır. Artık Default.aspx sayfasının tasarımsal ve kodsal içeriğini kodlayabiliriz

![Winking smile](/assets/images/2010/wlEmoticon-winkingsmile_10.png)

Default.aspx içeriği;

```text
<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" 
    CodeBehind="Default.aspx.cs" Inherits="PDC2010._Default" %> 
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent"> 
</asp:Content> 
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent"> 
    <h2> 
        Microsoft PDC 2010 
    </h2> 
    <p> 
        <asp:PlaceHolder ID="holderSessions" runat="server"></asp:PlaceHolder> 
    </p>

</asp:Content>
```

kod kısmı;

```csharp
using System; 
using System.Linq; 
using PDC2010.PDC;

namespace PDC2010 
{ 
    public partial class _Default : System.Web.UI.Page 
    { 
        protected void Page_Load(object sender, EventArgs e) 
        { 
            ScheduleModel model = new ScheduleModel(new Uri("http://odata.microsoftpdc.com/ODataSchedule.svc/")); 
            var sessions = from p in model.Sessions 
                           orderby p.FullTitle 
                           select new 
                           { 
                              p.FullTitle, 
                               p.FullDescription, 
                              p.Tags, 
                              p.ThumbnailUrl, 
                               p.DownloadableContent 
                          };

           foreach (var s in sessions) 
            { 
                SessionInfo sInfo=LoadControl("~/SessionInfo.ascx") as SessionInfo; 
                sInfo.Title = s.FullTitle; 
                sInfo.Description = s.FullDescription; 
                sInfo.ThumbnailLink = s.ThumbnailUrl; 
                sInfo.Tags = s.Tags;

                if (s.DownloadableContent.Count > 0) 
                { 
                   foreach (var c in s.DownloadableContent) 
                    { 
                        ContentLink cLink = LoadControl("/ContentLink.ascx") as ContentLink; 
                        cLink.Title = c.Title; 
                        cLink.Url = c.Url; 
                        sInfo.LinkHolder.Controls.Add(cLink); 
                    } 
                }

                holderSessions.Controls.Add(sInfo); 
            } 
        } 
    } 
}
```

İlk olarak ScheduleModule tipine ait bir nesne örneği oluşturulduğu görülmektedir ki yapıcı metodu (Constructor) parametre olarak WCF Data Service adresini almaktadır. Sonrasında standart bir LINQ sorgusu yazılmış ve Sessions Entity içeriğine gidilerek bazı bilgilerin alınması ve bunların bir anonymous type (İsimsiz Tip) içerisinde birleştirilmesi sağlanmıştır. Bu akılcı bir yaklaşımdır nitekim Sessions tipi içerisindeki tüm özelliklere ihtiyacımız yoktur

![Winking smile](/assets/images/2010/wlEmoticon-winkingsmile_10.png)

Her bir Sessions nesne örneği üzerinden DownloadableContent özelliğine giderek indirilebilir içerik bilgilerinin Title ve Url bilgilerine ulaşılmaktadır. Elbette her bir Sessions nesne örneği için bu işlem söz konusudur. Her bir Sessions için bir SessionInfo Web User Control nesnesi örneklenirken, her bir Content nesne örneği için de ContentLink Web User Control’ üne ait örneklemeler yapılmakta ve sayfaya eklenmeleri sağlanmaktadır.

Çok doğal olarak sayfaya ait Load metodunda yaptığımız bu işlemler bir kaç saniyelik zaman kaybına neden olacaktır. Burada servisten verinin alınıp indirilmesi ve işlenmesi, süre kaybına neden olan etkenlerin başında gelmektedir. Dolayısıyla asenkron olarak verinin yüklenmesi ve hatta AJAX tabanlı bir Web Control içerisinde bu yükleme işleminin yapılması çok daha doğru bir yaklaşımdır. Bu kritik noktayı bir kenara bırakıp uygulamamızı çalıştırdığımızda ise aşağıdaki ekran görüntüsündekine benzer sonuçlar ile karşılaştığımızı görürüz.

[![blg240_Result](/assets/images/2010/blg240_Result_thumb.gif)](/assets/images/2010/blg240_Result.gif)

Bu noktada dilerseniz Windows veya WPF tabanlı bir Desktop uygulaması ya da Silverlight tabanlı bir Rich Internet Application’ da geliştirebilirsiniz. Servis dünyasını seviyorum

![Smile](/assets/images/2010/wlEmoticon-smile_2.png)

Bu yazıda ele aldığımız PDC servisinin OData formatında veri içeriği sunuyor olması sayesinde tamamen platform bağımsız istemciler geliştirebilir ve tüm PDC içeriğini bu uygulamalar üzerinde değerlendirebiliriz. Ben kapıyı gösterdim, geçecek olan sizsiniz

![Winking smile](/assets/images/2010/wlEmoticon-winkingsmile_10.png)

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[PDC2010.rar (200,14 kb)](/assets/files/2010/PDC2010.rar)