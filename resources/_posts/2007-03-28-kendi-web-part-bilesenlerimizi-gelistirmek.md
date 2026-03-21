---
layout: post
title: "Kendi Web Part Bileşenlerimizi Geliştirmek"
date: 2007-03-28 12:00:00 +0300
categories:
  - aspnet-2-0
tags:
  - asp.net
  - web-parts
  - custom-web-parts
---
Web uygulamalarında var olan bileşenlerin yetersiz kaldığı durumlarda kendi kontrollerimizi geliştirme yoluna gidebiliyoruz. Kendi kontrollerimizi geliştirirken seçebileceğimiz yollar bellidir. Var olan bir web bileşeninden türetme yolunu seçebiliriz (Inherited Controls). Bu durumda kontrolün Html çıktısının ne olacağını bir başka deyişle Render işlemlerini çok fazla düşünmemize gerek kalmaz. Tek yapmamız gereken var olan üyeleri ezmek (override) veya yeni üyeler katmaktır. Bir diğer yol birden fazla kontrolü içeren komposit bir bileşen geliştirmektir (Composite Controls). Bu tekniğe verilebilecek en güzel örnek kullanıcı web kontrolleridir (web user controls). Özel bileşen geliştirmenin belkide en zor seçeneği, kontrolü sıfırdan yazmaktır. Bu durumda, ilgili bileşenin istemci tarayıcılarındaki Html çıktısını düşünmekle kalmayıp, ViewState, Postback, Event Handling gibi temel konularında göz önüne alınması ve düşünülmesi gerekir.

Asp.Net 2.0 ile birlikte WebPart adı verilen bir Framework gelmiştir. Bu Framework istersek kolay bir şekilde kişiselleştirilebilir (Personalizable) web kontrolleri yazabilmemizede olanak sağlamaktadır. Aslında bahsettiğimiz özelleştirilmiş kontrollerin birer WebPart tipi olduğunu söylemek gerekir. Dolayısıyla, kendi WebPart bileşenlerimizi türetme yardımıyla kolay bir şekilde geliştirebilir ve kullanabiliriz. WebPart'lar sıfırdan yazılan bileşen kontrolleri ve web kullanıcı kontrollerine nazaran, WebPart Framework için tam destek sağlarlar. Bu da, kişiselleştirmenin kullanıldığı sayfalarda oldukça önemli bir meziyettir. İşte biz bu makalemizde basit olarak kendi Web Part bileşenlerimizi nasıl geliştirebileceğimizi incelemeye çalışacağız.

Normal şartlar altında sayfada yer alan herhangiri Web Part Zone içerisine alınan her bileşen otomatik olarak Web Part muamalesi görür. Lakin kendi Web Part bileşenlerimizi geliştirdiğimizde, daha öncedende belirttiğimiz gibi Web Part Framework'ü içerisindeki özelliklerin ve fonksiyonelliklerin tamamını kullanabilme şansına sahip oluruz. Öyleyse işe WebPart tipinden türeyen bir sınıf yazmak ile başlamak lazım.

> WebPart tipi abstract bir sınıf olup, bir Web Part bileşeni için gerekli tüm temel alt yapıyı sunmaktadır.

Bildiğiniz gibi, Visual Studio 2005 içerisindeki proje şablonlarından biriside, Web Control Library seçeneğidir. Web Control Library, standart olarak örnek bir özel kontrol (Custom Control) sınıfı içerir. Aynı zamanda Web ortamı için gerekli temel referanslarıda hazır olarak barındırır. (Örneğin System.Web) Biz Web Part bileşenimizi böyle bir kontrol kütüphanesi içerisine dahil edersek, herhangibir web uygulamasında kolayca kullanabiliriz. Dahası, geliştirdiğimiz Web Part bileşenlerini Visual Studio ToolBox içerisinde ele alabiliriz. Bu da Web Part bileşenimizi bir kontrol olarak ToolBox içerisinde tutabileceğimiz anlamına gelmektedir.

Gelelim makalemizin örnek senaryosuna. Kendi Web Part kontrollerimizi geliştirmeyi öğrenirken, standart olarak ele alınan senaryo, RSS bilgilerini tutan bir bileşenin yazılmasıdır. Bizde geleneği bozmayıp bu tip bir Web Part bileşenini nasıl yazabileceğimizi incelemeye çalışacağız. Ancak başlamadan önce RSS ile ilişkili olarak biraz bilgi vermekte fayda olacağı kanısındayım. Günümüzde pek çok web sitesi, güncel olarak yayınlamak istedikleri bilgilerin, başkaları tarafından kolay bir şekilde ele alınabilmesi amacıyla, Xml tabanlı içerikler sunarlar.

RSS bu anlamda Xml verisini standardize etmektedir. Böylece, her RSS içeriğinin aynı şemaya sahip olması sağlanmış olur. Bizde bu yaklaşımı kullanacağız. Özellikle.Net Framework, Xml üzerinde son derece etkili yönetimli tipler (managed types) sunmaktadır. Bu tiplerden faydalanarak her hangibir RSS içeriğini kolay bir şekilde ayrıştırabiliriz (parsing). Aşağıdaki ekran görüntüsünde C#Nedir? sitesinin [http://www.csharpnedir.com/Rss.xml](http://www.csharpnedir.com/Rss.xml) adresinden yayınlanan RSS dökümanının bir parçasını görmektesiniz.

![mk197_1.gif](/assets/images/2007/mk197_1.gif)

Dikkat ederseniz, rss isimli root element içerisinde channel isimli tek bir alt boğum (childe node) vardır. Bu node içerisinde RSS dökümanın sahibi ile ilişkili çeşitli bilgiler yer alır. Örneğin, RSS konusunu anlatan başlık (title) ve açıklama (description) bilgisi, RSS sahibinin web adresi (link) gibi. Diğer taraftan item elementleri içerisindede RSS ile yayınlanmak istenen asıl içerik yer alır. Özetle bu RSS dökümanı ile çalışma zamanında C#Nedir? sitesinde yayınlanan son makalelerin listesini elde edebilir, bağlantıları kullanarak buralara geçiş yapabiliriz. Eğer bu RSS dökümanını internet üzerinden talep edersek aşağıdakine benzer bir ekran görüntüsü alırız.

![mk197_3.gif](/assets/images/2007/mk197_3.gif)

İşte Web Part kontrolümüz, kişi bazında herhangibir RSS bilgisini sunacak şekilde tasarlanacaktır. Burada RSS'in Url bilgisini ve hatta RSS'in sahibi ile ilgili kısa bir bilgiyi, kişi bazında ayrı ayrı saklayabiliriz. Gelin Web Part kontrolümüzü yazarak kişiselleştirilebilir bir RSS okuyucunun nasıl yapılabileceğini görmeye çalışalım. İlk olarak Web Control Library projemize bir sınıf ekleyeceğiz. Sınıfımızın WebPart sınıfından türemiş olması gerekmektedir.

> Bir sınıfı WebPart abstract sınıfından türettiğimizde, söz konusu yeni tip, Web Part Framework'ünü kullanabileceğimiz üyelerede sahip olur. Bu üyeler WebPart sınfınında türediği çeşitli tipler içerisinde toplanmaktadır. Aşağıdaki şekilde, geliştirdiğimiz örnek web part bileşeninin nesne hiyerarşisini görebilirsiniz.

![mk197_2.gif](/assets/images/2007/mk197_2.gif)

İlk olarak RssPart isimli bir sınıfı WebPart tipinden türeyecek şekilde aşağıdaki gibi tanımlayalım.

```csharp
[ToolboxData("<{0}:RssPart runat=server></{0}:RssPart>")]
public class RssPart : WebPart
{
}
```

Geliştirdiğimiz sınıf her ne kadar bir Web Part bileşeni olsada, sunucu taraflı (server side) bir kontroldür. Bu nedenle, aspx dosyalarının kaynak kısmında birer takı (tag) içerisinde ele alınacaktır. ToolboxData isimli niteliğin eklenmesinin sebebi de budur. Buna göre Web Part bileşenimizi herhangibir aspx sayfasına eklediğimizde aşağıdakine benzer bir element içeriği ile karşılaşırız.

```text
<cc1:RssPart ID="RssPart1" runat="server"/>
```

Burada cc1 ön ekinin (prefix) nereden geldiğini merak ediyor olabilirsiniz. Aslında ToolBox'tan bir WebPart kontrolünü sayfaya sürükleyip bıraktığımızda otomatik olarak sayfanın başına Register direktifi eklenecektir. (Bu ilk kontrol sürüklenip bırakıldığında otomatik olarak oluşur) Bu direktif temel olarak, Asp.Net uygulamasında kullanılacak olan Web Part bileşenini içeren sınıf kütüphanesinin (Contol Library) referans bilgisinide içerecektir. Kısaca Register direktifimizinde aşağıdaki gibi olacağını söyleyebiliriz.

```text
<%@ Register Assembly="MyWebParts" Namespace="MyWebParts" TagPrefix="cc1" %>
```

Gelelim Web Part kontrolümüz içerisindeki üyelere. Kontrolümüzün RSS belgesine ait Url bilgisini ve kısa bir başlık bilgisini tutacağını varsayabiliriz. Bu bilgiler için yapacağımız normal şartlar altında birer özellik yazmak olacaktır. Lakin bu sefer özelliklerimizin, kişi bazında özelleştirilebilmesini istiyoruz. Bu nedenle Personalizable niteliğini kullanmamız gerekiyor.

```csharp
private string _Url;
private string _RssOwner;

[WebBrowsable(true)] // PropertyGridEditorPart içerisinde bu özelliğin gösterilip gösterilmeyeceğini belirtir.
[WebDescription("Verilen Url adresine göre Rss bilgisini okur")]
[Personalizable(PersonalizationScope.User)] // Özelliğin değerinin Membership tablolarında kullanıcı bazında tutulacağını belirtir.
[WebDisplayName("Rss Bilgisi Alınacak Url")] // PropertyGridEditorPart içerisinde Url özelliği için gösterilecek bilgi
public string Url
{
    get { return _Url; }
    set { _Url = value; }
}

[WebBrowsable(true)]
[WebDescription("Rss sahibine ait bilgiyi içerir")]
[Personalizable( PersonalizationScope.User)]
[WebDisplayName("Rss Yayımcısı")]
public string RssOwner
{
    set { _RssOwner = value; }
    get{return _RssOwner;}
}
```

Url ve RssOwner isimli özelliklerimiz, kişiselleştirilebilir üyelerdir. Personalizable niteliğine atanan PersonalizableScop.User değeri sayesinde Url ve RssOwner özelliklerinin işaret ettikleri değerlerin, kişi bazında Membership API'si üzerinden ilgili tablolarda tutulabileceği belirtilmektedir. WebBrowsable niteliği ile, özelliğin bir PropertyGridEditorPart bileşeni içerisinde gösterilip gösterilmeyeceğine karar verilebilir. Buna göre Url ve RssOwner isimli özelliklerimizi, sayfayı ziyaret eden kullanıcılar isterlerse PropertyGridEditorPart içerisinden değiştirebilirler. WebDescription niteliği (attribute), PropertyGridEditorPart içerisinde gösterilecek olan özelliklerin üzerlerine gelindiğinde kısa açıklama kutucukları gösterilmesini sağlar. Son olarak WebDisplayName niteliği sayesinde, özelliklerin ProperyGridEditorPart içerisinde hangi adlar ile sunulacağı belirtilmektedir.

Kişi bazında tutulacak özelliklerimizide belirttikten sonra, Web Part kontrolümüzün ekrana nasıl çizileceğini ayarlayabiliriz. Bildiğiniz gibi, her sunucu web bileşeni istemci tarafında Html çıktıları haline getirilirler (Render işlemi). Burada da, belirtilen Url adresindeki RSS dökümanını ayrıştırdıktan (parsing) sonra örnek olarak linkler halinde göstermek isteyebiliriz. Bu çıktının düzenli bir formatta olmasını sağlamak içinde Html tablolarına başvurabiliriz. Bir sunucu kontrolünde, Html çıktısını oluşturmak için Render metodu göz önüne alınabilir. Diğer taratfan CreateChildControls metoduna başvurup daha kolay bir biçimde çıktı üretebiliriz. CreateChildControls metodunu ilerleyen makalelerimizde ele almaya çalışacağız. Şimdi dilerseniz, Web Part bileşenimiz için Render metodunu ezelim (override) ve Html çıktısını, RSS içeriğine göre hazırlayalım.

```csharp
protected override void Render(HtmlTextWriter output)
{
    if (!String.IsNullOrEmpty(Url))
    {
        try
        {
            XmlReader reader = XmlReader.Create(Url);

            DataSet ds = new DataSet();
            ds.ReadXml(reader);

            DataTable items = ds.Tables["item"];

            #region Render Table

            // Table elementi render edilmeden önce gerekli style attribute' ları ekleniyor
            output.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, "WhiteSmoke"); // Arka plan rengi
            output.AddStyleAttribute(HtmlTextWriterStyle.Width, "100%"); // genişlik belirleniyor
            output.RenderBeginTag(HtmlTextWriterTag.Table); // Table için açılış takısı

                output.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, "Gold");
                output.RenderBeginTag(HtmlTextWriterTag.Tr); // Tr açılış takısı (satır)
                    output.RenderBeginTag(HtmlTextWriterTag.Td); // Td açılış takısı (hücre)
                        output.Write(ds.Tables["channel"].Rows[0]["title"].ToString()); // Td içerisine Rss dökümanından title bilgisi alınıyor
                    output.RenderEndTag(); // Td için kapanış takısı
                output.RenderEndTag(); // Tr için kapanış takısı

                // Rss dökümanındaki her bir item için bir Tr (Table Row) ve içerisinde bir Td (hücre) oluşturuluyor
                for (int i = 0; i < items.Rows.Count; i++)
                {
                    output.RenderBeginTag(HtmlTextWriterTag.Tr); // Tr açılış takısı
                        output.RenderBeginTag(HtmlTextWriterTag.Td); // Td açlış takısı
                            output.AddAttribute(HtmlTextWriterAttribute.Href, items.Rows[i]["link"].ToString()); // href isimli attribute sonraki satırda açılacak olan A takısına ilave edilecek. Değeri ise link alanının içeriği olacak
                            output.RenderBeginTag(HtmlTextWriterTag.A);// A takısı açılıyor
                                output.Write(items.Rows[i]["title"].ToString()); // A takısı içine title alanının değeri yazılıyor
                            output.RenderEndTag(); // A takısı kapatılıyor
                        output.RenderEndTag(); // Td takısı kapatılıyor
                    output.RenderEndTag(); // Tr takısı kapatılıyor
                }

            output.RenderEndTag();// Table' ın bitiş takısı </table>
        }
        catch
        {
            output.Write("Adres çözümlenemedi");
        }
    
        #endregion 
    }
}
```

Render metodunda, daha önceden Web sunucu kontrolü yazmak ile ilişkili makalelerimizde kullandığımızdan biraz daha farklı bir yol tercih ettik. Html içeriğini oluştururken,.Net içerisinde yer alan kuvvetli tiplerden faydalanırsak hatalı Html takısı yazma olasılığımız daha da azalacak ve özellikle tarayıcı farklılıklarını bertaraf edeceğizdir. Bu nedenle Render metodu içerisinde, HtmlTextWriter sınıfına ait RenderBeginTag, RenderEndTag, AddAttribute, AddStyleAttribute gibi metodlardan yararlanılmıştır. Örneğimize göre içeriğimiz bir Table elementi içerisinde tek sütundan oluşan bir yapıda olacaktır. Bu amaçla bir table takısı açmak için

```csharp
output.RenderBeginTag(HtmlTextWriterTag.Table);
```

kod satırından faydalanılmıştır. Burada RenderBeginTag, bir takının oluşturulmasını sağlarken ne çeşit bir element olacağını parametre olarak gönderdiğimiz HtmlTextWriterTag sabiti belirtmektedir. Html içerisinde açılan her takının kapatılması gerektiğini biliyoruz. Bunu kod tarafında ifade ederkende yine HtmlTextWriter sınıfının RenderEndTag metodundan faydalanmaktayız. Dikkat edilmesi gereken noktalardan biriside, Html elementlerine niteliklerin nasıl eklendiğidir. Dikkat ederseniz, bu amaçla AddStyleAttribute ve AddAttribute metodlarından yararlanılmıştır. Örneğin tablomuza arka plan rengi ve genişlik vermek için

```csharp
output.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, "WhiteSmoke");
output.AddStyleAttribute(HtmlTextWriterStyle.Width, "100%");
```

kod satırlarından faydalanılmaktadır. Hangi niteliğin ekleneceğini belirlemek için ise, HtmlTextWriterStyle sabitinden yararlanılır. AddAttribute metoduda benzer işlevselliğe sahip olmakla birlikte desteklediği nitelik tipleri daha farklıdır.

Gelelim metodun temel olarak yaptığı işe. İlk olarak kişiselleştirilebilen Url özelliğinden değer alınmakta ve XmlTextReader nesnesi elde edilmektedir. Hataların önüne geçmek amacıyla ilk olarak Url özelliğinin değerinin boş olup olmadığına bakılır. Bunun haricinde internet bağlantısının kesik olması gibi hallerde, istenen Url'den RSS bilgileri çekilemeyeceği için bir çalışma zamanı istisnası (runtime exception) alınacaktır. Bununda önüne geçmek için genel bir try-catch bloğu kullanılmıştır. Bu nedenle yükleme ve Render işlemlerini try bloğu içerisinde gerçekleştirmekteyiz. Burada Url adresinden elde edilen Xml içeriğini okumak için farklı yollarda tercih edilebilir.

Örneğin doğrudan XmlReader üzerinden hareket edilebilir veya XPathNavigotor tipinden faydalanılabilir ki bunlar performans açısından daha etkili yollardır. Biz kodu çok fazla karmaşıklaştırmamak adına doğrudan DataSet kontrolüne alıyor ve pahalı bir maliyetinde altına giriyoruz:) DataSet içerisinde yer alan channel ve item tablolarından faydalanarak, RSS içeriğindeki bilgilere erişebiliriz. Örneğin RSS sahibinin belirttiği başlığı (title), Html tablomuzun ilk satırındaki ilk hücreye alıyoruz. Her bir item elementinin belirttiği, başlık ve adres bilgilerini ise sırasıyla title ve link elementlerinden alıp bir a href elementi içerisinde gösteriyoruz. Bildiğiniz gibi a href elementi bir link oluşturulmasını sağlamaktadır.

Web Part kontrolümüzün, WebPart sınıfından gelen pek çok özelliğinide istersek ezebiliriz (override). Örneğin, Web Part bileşenimizin başlık bilgisi (title), imge bilgisi (title icon image) gibi değerlerini değiştirmek isteyebiliriz. Lakin bu tip üyelerin değerlerinin kontrolün Html çıktısının üretilmesinden önce atanması gerekir. Bu amaçla yapıcı metoddan, özelliğin set bloğundan yada kontrole ait PreInit olay metodundan faydalanabiliriz. Geliştirdiğimiz örnekte biz yapıcı metod ve özellik (property) kullanmayı tercih edeceğiz. Bu amaçla sınıfımıza aşağıdaki üyeleri eklememiz yeterli olacaktır.

```csharp
public override string Title
{
    get
    {
        return _RssOwner;
    }
    set
    {
        _RssOwner= value;
    }
}

public RssPart()
{
    TitleIconImageUrl = "Bilgi.gif"; 
}
```

Yukarıdaki kod parçasında dikkat ederseniz, WebPart sınıfında yer alan Title özelliği ezilmektedir ve kullanıcı tarafından kişiselleştirilebilen RssOwner özelliğinin kullandığı _RssOwner alanının değerini işaret etmektedir. Diğer taraftan, Web Part bileşenimizde hemen başlık kısmının yanında sembolik bir imge göstermek amacıyla yapıcı metod (constructor) içerisinde TitleIconImageUrl özelliğine bir değer ataması yapılmıştır. Örnek bir Asp.Net sayfasında Web Part bileşenimizi kullandığımızda tasarım zamanında iken, aşağıdakine benzer bir görüntü ile karşılaşırız.

![mk197_5.gif](/assets/images/2007/mk197_5.gif)

Burada yer alan Bilgi.gif isimli resim, Web Control Library içerisinde yer almaktadır ve herhangibir Asp.Net projesinde ilgili Web Part bileşeni kullandığında otomatik olarak root klasör içerisine taşınacaktır. Bir başka deyişle eklenen ilk kontrol ile birlikte gelen web kontrol kütüphanesi referansı, beraberinde kaynak olarak bu resim dosyasınıda hedef web uygulaması içerisine taşıyacaktır. Yapılan bu son ekelemeler ile birlikte Web Part bileşenimizin yeni hali aşağıdaki sınıf diagramında gösterildiği gibi olacaktır.

![mk197_4.gif](/assets/images/2007/mk197_4.gif)

Artık Web Part bileşenimizi bu haliyle test edebiliriz. Kişiselleştirmenin tam olarak etkilerini görebilmek için, bileşenimizi Membership ayarları yapılmış örnek bir Asp.Net Web uygulaması üzerinden test etmekte fayda olacaktır. Testleri gerçekleştirmek için ekran görüntüsü aşağıdaki gibi olan bir web sayfasından yararlanabiliriz.

![mk197_6.gif](/assets/images/2007/mk197_6.gif)

Unutulmamalıdır ki, geliştirdiğimiz Web Part bileşeninin, Web Part Framework özelliklerini etkin bir şekilde kullanabilmesi için bir Web Part Zone içerisinde yer alması gerekir. Bu nedenle, RssPart isimli Web Part bileşenimizi zoneRss isimli bir WebPartZone kontrolü içerisinde ele alıyoruz. Diğer taraftan, Web Part kontrolümüz içerisinde yer alan Url ve RssOwner gibi çalışma zamanında kişiselleştirilebilir özelliklerini ele alabilmek için bir Editor Zone kontrolümüzde yer almaktadır. Örneğimizde sadece editör kısmını göz önüne alacağımızdan, sayfanın Page_Load olay metodu içerisinde WebPartManager bileşenimizin DisplayMode özelliği aşağıdaki gibi EditDisplayMode olarak ayarlanmıştır. Gerçek bir projede elbetteki kullanıcının diğer modlarıda seçebileceği şekilde kod yazmak gerekir.

```csharp
wpmYonetici.DisplayMode = WebPartManager.EditDisplayMode;
```

Sonuç olarak uygulamamızı çalıştırdığımızda, aşağıdaki Flash animasyonunda yer alana benzer bir sonuç ile karşılaşırız.(Flash dosyasının boyutu 380 Kb tır. Bu nedenle yüklenmesi zaman alabilir.)

Dikkat ederseniz, iki ayrı kullanıcı için iki ayrı RSS bilgisi ele alınabilmektedir. Burak isimli kullanıcı kendisi için Msdn'e ait RSS içeriğini tutarken, Melike isimli kullanıcıda C#Nedir? sitesine ait RSS bilgisini ele alabilmektedir. Bu bilgilerin arka tarafta nereye yazıldığını kontrol etmek istersek, Membership bilgilerinin tutulduğu veritabanında (ki örneğimizde local AspNetDb.mdf dosyası kullanılmaktadır. Bir başka deyişle üyelik bilgileri web uygulamasın ait AppData klasöründe yer alan AspNetDb.mdf veritabanı içerisinde saklanmaktadır.) yer alan AspNetPersonalizationPerUser tablosuna bakmamız yeterli olacaktır.

![mk197_7.gif](/assets/images/2007/mk197_7.gif)

Kendi Web Part bileşenlerimizi geliştirirken başka üst sınıf özelliklerini ezebilir ve istediğimiz şekilde çalışmalarını sağlayabiliriz. Aynı zamanda, bir Web Part bileşenine çalışma zamanında ele alacağı yeni fiili aksiyonlar (verb) ekleyebiliriz. Bu ve benzeri diğer konuları ilerleyen makalelerimizde ele almaya çalışacacağız. Böylece geldik bir makalemizin daha sonuna. Bu makalemizde basit olarak kendi Web Part bileşenlerimizi nasıl yazabileceğimizi incelemeye çalıştık. Bunun için,

- kontrolümüzü tanımlayacak olan sınıfı WebPart isimli abstract sınıftan türetmemiz gerektiğini,
- içeride kullanıcı bazında kişiselleştirme yapacağımız özellikler için Personalizable niteliğini (attribute) kullanmamız gerektiğini,
- kontrolün kişiselleştirilebilir özelliklerinin çalışma zamanında ele alınması için PropertyGridEditorPart kullanmamız gerektiğini,
- özelliklerin PropertyGridEditorPart içerisinde gözükmesi için WebBrowsable niteliğinin kullanılması gerektiğini,
- istersek var olan Web Part özelliklerini ezerek değiştirebileceğimizi,
- her zaman olduğu gibi kontrolün Html çıktısını manuel olarak düşünememiz ve yazmamız gerektiğini,

öğrendik. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.

[Örnek Uygulama İçin Tıklayınız](/assets/files/2007/CustomWebParts.rar)