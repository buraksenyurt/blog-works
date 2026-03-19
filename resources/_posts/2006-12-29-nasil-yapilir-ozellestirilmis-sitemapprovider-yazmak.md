---
layout: post
title: "Nasıl Yapılır : Özelleştirilmiş SiteMapProvider Yazmak"
date: 2006-12-29 12:00:00 +0300
description: "Asp.Net 2.0 ile web uygulamalarını geliştirmek artık çok daha kolaylaştı. Bunun en büyük nedenlerinden biriside kontrol paneline gelen çok sayıda bileşen olmasıdır. Her ne kadar bir yazılımcının hayatını kolaylaştıran yenilikler olsada zaman zaman var olan bu yapıları özelleştirme yoluna gitmek isteyebiliriz."
categories:
  - aspnet-2-0
tags:
  - aspnet-2-0
  - csharp
  - xml
  - dotnet
  - aspnet
  - sql-server
  - performance
  - caching
  - datatable
---
# Nasıl Yapılır : Özelleştirilmiş SiteMapProvider Yazmak
Değerli Okurlarım Merhabalar,

Asp.Net 2.0 ile web uygulamalarını geliştirmek artık çok daha kolaylaştı. Bunun en büyük nedenlerinden biriside kontrol paneline gelen çok sayıda bileşen olmasıdır. Her ne kadar bir yazılımcının hayatını kolaylaştıran yenilikler olsada zaman zaman var olan bu yapıları özelleştirme yoluna gitmek isteyebiliriz. Bugunkü makalemizin konusunu oluşturan SiteMapProvider bu durumda ele alınabilecek yeniliklerden birisidir. Asp.Net 2.0 ile geliştirilen web uygulamalarında site haritası çıkartmak ve bunu ele alacak kontrollerle çalışmak son derece kolaydır.

Yeni gelen SiteMapPath, Menu ve TreeView kontrolleri site haritasının etkin bir şekilde kullanılmasını sağlayan bileşenlerdir. Bu kontroller sayesinde kullanıcıların site içerisinde hareket etmesi, nerede olduklarını görmesi son derece kolaylaşmıştır. Aslında olayın özünde web uygulamasına dahil edilen web.sitemap isimli dosya ve içeriği yer almaktadır. Xml tabanlı olan bu dosya siteMapNode isimli elementlerden oluşmaktadır. Aşağıda örnek bir web.sitemap dosya içeriği yer almaktadır.

![mk186_1.gif](/assets/images/2006/mk186_1.gif)

Söz konusu olan bu xml kaynağını SiteMapPath kontrolü doğrudan kullanırken, Menu ve TreeView kontrolleri SiteMapDataSource bileşeni üzerinden ele alırlar. Dikkat ederseniz siteMapNode elementleri iç-içe (nested) bir yapıya sahiptir. Bu hiyerarşik yapı, doğal olarak site içerisindeki sayfaları ve aralarındaki bağlantıları tanımlar. Aynı zamanda ilgili sayfanın yol (url), açıklama (description), başlık (title) gibi bilgileride ilgili kontrollere siteMapNode elementleri ile sunulmaktadır. Ne varki burada çalışan sistem tamamen Xml tabanlı açık bir dosyaya bağımlıdır.

Oysaki site haritasını, gelen kullanıcıların rollerine göre değiştireceğimiz, buna bağlı olaraktanda menülerde hangi linklerin gösterileceğine karar vermek isteyeceğimiz senaryolar söz konusu olabilir. En basit anlamda site haritasını Xml kaynağı yerine örneğin bir veritabanı tablosundan getirmek isteyebiliriz. İşte bu ve benzeri ihtiyaçlar bizim SiteMapProvider tipini özelleştirmemize neden olmaktadır. SiteMapProvider özelleştirmesi için StaticSiteMapProvider isimli abstract sınıfından türetme yapacağımız bir tipi ele almamız gerekmektedir. Bu özelleştirme işlemine başlamadan önce bizim için gerekli ön hazırlıkları yapalım. Senaryomuz gereği site haritasını Sql Server üzerinden SiteMaps isimli bir tabloda tutacağız. Tablomuzu oluşturmak için gerekli sql kodu aşağıdaki gibidir.

```csharp
USE [AdventureWorks]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SiteMaps](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Baslik] [nvarchar](50) NOT NULL,
    [Aciklama] [nvarchar](50) NOT NULL,
    [Url] [nvarchar](50) NOT NULL,
    [Ust] [int] NULL,
    CONSTRAINT [PK_SiteMaps] PRIMARY KEY CLUSTERED 
    (
        [Id] ASC
    )WITH (PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
```

Tablomuzun kolonları temel olarak site haritası içerisinde yer alacak her bir öğenin sunması gereken nitelikleri içermektedir. Baslik bilgisi ile bir sayfanın navigasyon kontrollerinde gösterilecek olan ismini, Aciklama alanı ile istemci tarayıcı penceresinde çıkacak olan tip kutucuklarında gösterilecek içeriği, Url ile ilgili öğeye tıklandığına gidilmesi gereken sayfanın adresini belirtiyoruz. Ama belkide en önemli kısım tablo içerisindeki satırların kime bağlı olduğunu belirten Ust isimli alandır.

Bu alan sayesinde bu tablo üzerinde self-referencing tipinden bir ilişki tanımlanmaktadır. Bu ilişki sayesinde site içerisinde hangi sayfanın (sayfaların) kimlere bağlı olduğu (bir başka deyişle hangi sayfanın alt dalı olduğu) bilgisini elde edebiliriz. Kısacası bu tablonun tasarımındaki amaç, web.sitemap içerisinde Xml tabanlı olarak oluşturulan hiyerarşinin gerçekleştirilmesidir. Tablomuzu oluşturduktan sonra örnek olması açısından aşağıdaki ekran görüntüsünde yer alan veriler ile doldurabiliriz.

![mk186_2.gif](/assets/images/2006/mk186_2.gif)

Site haritasına ait bilgileri taşıyan bu tablonun bilgilerine çalışma zamanında ihtiyacımız olacaktır. Bu amaçla kod içerisinde SiteMaps tablosundan veri çekmek için gerekli sorgu cümlesini tutabileceğimiz gibi bir saklı yordamdan da (stored procedure) faydalanabiliriz. Aşağıdaki saklı yordam bu amaçla tasarlanmıştır ve çalışma zamanında site haritasının belleğe yüklenmesi amacıyla kullanılacaktır.

```text
USE AdventureWorks
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE SiteHaritasiCek
AS
BEGIN
SET NOCOUNT ON;
    SELECT Id,Baslik,Aciklama,Url,Ust From SiteMaps
END
GO
```

Artık kod tarafında neler yapacağımıza bakabiliriz. Her şeyden önce var olan SiteMapPath, Menu ve TreeView kontrollerinin bakacağı SiteMapProvider nesnesini değiştirmek istiyoruz. Dolayısıyla bu değişiklik için uygulamanın web.config dosyası içerisinde varsayılan sağlayıcısını aşağıdaki gibi değiştirmemiz gerekecektir.

```xml
<system.web>
    <siteMap defaultProvider="SiteHaritaSaglayicisi">
        <providers>
            <clear/>
            <add name="SiteHaritaSaglayicisi" type="SiteHaritaYoneticisi" baglantiBilgisi="Data Source=localhost;Initial Catalog=AdventureWorks;Integrated Security=SSPI" spAdi="SiteHaritasiCek"/>
        </providers>
    </siteMap>
...
```

providers isimli elementin niteliklerinden name ve type mutlaka yazılmak zorundadır. Type niteliğinde (attribute) özelleştireceğimiz SiteMapProvider tipini belirtmekteyiz. Name niteliğinde belirtilen değer ise özellikle, kullanılması için her hangibir SiteMapDatasource istemeyen SiteMapPath kontrolü için önemlidir. Nitekim bu kontrol varsayılan olarak sitede web.sitemap isimli bir dosyayı aramaktadır. Ancak senaryomuzda kendi SiteMapProvider tipimizi yazıyoruz. Dolayısıyla SiteMapPath kontrolüne bunu bir şekilde söylememiz gerekecektir. İşte bu noktada name niteliğinin içeriği önem kazanmaktadır. baglantiBilgisi ve spAdi isimli nitelikler ise SiteHaritaYoneticisi isimli sınıfımız içerisinde yer alan Initialize metodundan yakalayabileceğimiz niteliklerdir. Burada amaç, yazmış olduğumuz sağlayıcı sınıfın, bağlantı bilgisi ve saklı yordam adından bağımsız olacak şekilde kullanılabilmesini sağlamaktır. Artık web uygulamamızda kullanacağımız SiteMapPath, Menu ve TreeView kontrolleri hangi Site Map sağlayıcısına bakacağını bilmektedir. Gelin şimdi özelleştirilmiş site haritası sınıfımızı yazalım. Sınıfımızı herhangibir web site uygulamasına dahil edebiliriz.

```csharp
using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public class SiteHaritaYoneticisi:StaticSiteMapProvider
{
    private string _spAdi;
    private string _baglantiBilgisi;
    private bool _olusturuldu;
    private SiteMapNode _rootNode;

    public bool Olusturuldu
    {
        set { 
            _olusturuldu = value; 
        }
        get { 
            return _olusturuldu; 
        }
    }

    /* eğer _spAdi ve baglantiBilgisi değerleri siteMap providers boğumundan alınmamışlarsa bunların sınıf içerisindeki değişkenlere atanması işlemlerini gerçekleştirir.*/
    public override void Initialize(string name, System.Collections.Specialized.NameValueCollection attributes)
    {
        if (!Olusturuldu)
        {
            base.Initialize(name, attributes);
    
            // NameValueCollection tipinden metoda gelen attributes isimli parametre üzerinden, spAdi ve baglantiBilgisi niteliklerinin değerleri alınır.
            _spAdi = attributes["spAdi"];
            _baglantiBilgisi = attributes["baglantiBilgisi"];
    
            Olusturuldu = true;
        }
    }

    protected override void Clear()
    {
        lock (this)
        {
            _rootNode = null;
            base.Clear();
        }
    }

    /* Site haritasını taşıyan _rootNode isimli SiteMapNode nesnesinin oluşturulmasını sağlar. */
    public override SiteMapNode BuildSiteMap()
    {
        lock (this)
        {
            // Eğer _rootNode oluşturulmamışsa,
            if (_rootNode == null)
            {
                // Öncelikli olarak eskiden kalan elementler varsa bunları koleksiyondan çıkart
                Clear();
                using (SqlConnection conn = new SqlConnection(_baglantiBilgisi))
                {
                    SqlCommand cmd = new SqlCommand(_spAdi, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dtNodes = new DataTable();
                    // Tablo bilgisi DataTable içerisine alınır.
                    da.Fill(dtNodes);
                    // Ust alanının değeri Null olan alan yani site haritasında en üstte duran satır bilgisi alınır.
                    DataRow drRoot = dtNodes.Select("Ust is null")[0];
                    // _rootNode oluşturulur. 
                    _rootNode = new SiteMapNode(this, drRoot["Url"].ToString(), drRoot["Url"].ToString(), drRoot["Baslik"].ToString(), drRoot["Aciklama"].ToString());
        
                    // o anki node(boğum)' un ID alanın değeri alınır ve recursive (yinelemeli) çalışan AltNodeEkle metodu tetiklenir.
                    string rootID = drRoot["ID"].ToString();
                    AltNodEkle(_rootNode, rootID, dtNodes);
                }
            }
            return _rootNode;
        }
    }

    /* Recursive (yinelemeli) olarak çalışan bu metod, site hiyerarşisinin root node(boğum)' undan en alt node(boğum)' una kadar dolaşmakla ve ilgili boğumları birbirlerinin     altına eklemekle görevlidir. */
    private void AltNodEkle(SiteMapNode ustNod, string rootID, DataTable dt)
    {
        // Gelen satırın altındaki satırları tespit etmek için dataTable içerisinde select atılır.
        DataRow[] altNodlar = dt.Select("Ust = " + rootID);
        // Her bir alt satır dolaşılır
        foreach (DataRow row in altNodlar)
        {
            // O anki node(boğum) oluşturulur ve yine o anki satırın ID değeri alınır.
            SiteMapNode cocukNod = new SiteMapNode(this,row["Url"].ToString(), row["Url"].ToString(),row["Baslik"].ToString(), row["Aciklama"].ToString());
            string rowID = row["ID"].ToString();
    
            // node(boğum) üst node(boğum)' a eklenir.
            AddNode(cocukNod, ustNod);
            // Recursive metodumuz tekrardan o anki satır ve node(boğum) için çalıştırlır.
            AltNodEkle(cocukNod, rowID, dt);
        }
    }

    // SiteMapNode' un elde edilmesini, bellekte oluşturulmasını sağlar bir SiteMapNode referansı olarak geriye döndürülmesini sağlar.
    protected override SiteMapNode GetRootNodeCore()
    {
        return BuildSiteMap();
    }

    // Bellekte oluşturulan siteMapNode' un elde edilmesini sağlayan özellik. (Readonly)
    public override SiteMapNode RootNode
    {
        get { 
            return BuildSiteMap(); 
        }
    }    
}
```

SiteHaritaYoneticisi isimli sınıfımız StaticSiteMapProvider abstract sınıfından türemiştir. Aslında StaticSiteMapProvider sınıfıda SiteMapProvider sınıfından türemiştir. StaticSiteMapProvider sınıfı, türediği SiteMapProvider sınıfının biraz daha ufaltılmış bir sürümüdür. Bu nedenle kendi SiteMapProvider tiplerimizi geliştirirken çok fazla kod düşünmemizi engelleyici nitelikte olduğundan tercih edilmektedir. Öyleki sınıfımızdan gördüğünüz gibi bir kaç temel üye ile istediğimiz özelleştirilmiş sağlayıcı sınıfı yazabiliyoruz. Aşağıdaki şekil söz konusu tipler arasındaki ilişkisel durumu daha net açıklamaktadır.

![mk186_3.gif](/assets/images/2006/mk186_3.gif)

Biz örnek sınıfımız içerisinde, üst sınıflarda abstract olarak tanımlanmış BuildSiteMap ve GetRootNodeCore isimli metodları eziyoruz. Bu metodlar sınıfın can alıcı noktalarıdır. Görevleri ilgili kaynaktan belleğe çekilen veriye göre oluşturulan Root Node'un (ki bu boğumda hiyerarşik olarak alt boğumları ile birlikte gelecektir) ilgili kontrollere verilmesini sağlamaktır. Burada söz konusu olan kontrollerimiz ise SiteMapPath, Menu ve TreeView kontrolleridir. Initialize metodu, çalışma zamanında BuildSiteMap ve GetRoodNodeCore üyelerden önce çalıştığı için, bağlantı bilgisinin ve sp adının web.config dosyasındaki yerlerinden alınmaları için en ideal yerdir. Sınıfımızı kullanacak olan web sitemizde aşağıdaki sayfalarımızın yer aldığını düşünebiliriz. Sayfalarımızı tablomuzdaki bilgilere göre tasarlamamız önemlidir. Aksi takdirde kırık linkler ile karşılabiliriz.

![mk186_4.gif](/assets/images/2006/mk186_4.gif)

Tüm sayfalarda navigasyon kontrollerimizi kolayca ele alabilmek için bir web user control (kullanıcı web kontrolü) bileşenini aşağıdaki gibi geliştireceğiz. Kullanıcı web kontrolümüz içerisinde SiteMapPath, Menu ve TreeView kontrollerimizi kullanacağız. Ancak dikkat etmemiz gereken bazı noktalar var. Bunlardan birisi SiteMapPath kontrolünü sürükleyip bıraktığımızda yazmış olduğumuz SiteHaritaYoneticisi tipine bağlanmadığıdır. Bunu sağlamak için SiteMapPath bileşenimizin SiteMapProvider özelliğine SiteHaritaSaglayicisi provider değerini vermemiz gerekmektedir. Bu provider web.config dosyasında kullanılacak olan SiteHaritaYoneticisi tipini işaret ettiği için SiteMapPath kontrolümüzün veriyi nereden alacağı belirlenmektedir.

```text
<asp:SiteMapPath ID="SiteMapPath1" runat="server" Font-Names="Verdana" Font-Size="0.8em" PathSeparator=" : " SiteMapProvider="SiteHaritaSaglayicisi">
    <PathSeparatorStyle Font-Bold="True" ForeColor="#990000" />
    <CurrentNodeStyle ForeColor="#333333" />
    <NodeStyle Font-Bold="True" ForeColor="#990000" />
    <RootNodeStyle Font-Bold="True" ForeColor="#FF8000" />
</asp:SiteMapPath>
```

Benzer problem TreeView kontrolü içinde geçerli olacaktır. Normal şartlarda TreeView kontrolü ve Menu kontrollerini kullanırken bir SiteMapDataSource bileşenine bağlamamız yeterlidir. SiteMapDataSource bileşeni otomatik olarak web.config dosyasındaki ayarlara bakacağından, ilgili kontrollere site haritasını bağlamak için hangi tipin ele alınması gerektiğini bilmektedir. Lakin TreeView kontrolü bu ayarlamalara rağmen site haritası içerisindeki üyeleri gösteremeyecektir. Bu sorunu çözmek için veri bağlama işlemini (TreeNode DataBindings) tasarım tarafından yada kaynak kod tarafından doğru şekilde ayarlamak gerekmektedir.

![mk186_5.gif](/assets/images/2006/mk186_5.gif)

```text
<asp:TreeView ID="TreeView1" runat="server" DataSourceID="SiteMapDataSource1" ImageSet="Arrows" ShowLines="True">
    <ParentNodeStyle Font-Bold="False" />
    <HoverNodeStyle Font-Underline="True" ForeColor="#5555DD" />
    <SelectedNodeStyle Font-Underline="True" ForeColor="#5555DD" HorizontalPadding="0px" VerticalPadding="0px" />
    <NodeStyle Font-Names="Verdana" Font-Size="8pt" ForeColor="Black" HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="0px" />
    <DataBindings>
        <asp:TreeNodeBinding DataMember="SiteMapNode" NavigateUrlField="Url" SelectAction="Expand" TextField="Title" ToolTipField="Description" />
    </DataBindings>
</asp:TreeView>
```

Burada dikkati çeken noktalardan birisi TreeNodeBinding elementi içerisinde NavigateUrlField, TextField, ToolTipField niteliklerinin baktıkları alan adlarıdır. Bizim tablomuzdaki alan adları hatırlayacağınız gibi, Baslik, Aciklama ve Url olarak isimlendirilmişti. Peki nasıl oluyorda bunlar birbirleriyle eşleşiyorlar? Bunu sağlayan elbetteki SiteMapNode sınıfının yapıcı metodudur. SiteHaritaYoneticisi sınıfı içerisinde boğumları oluştururken bu sınıfın yapıcı metoduna o anki tablo satırından ilgili değerler aktarılmaktadır. İşte eşleşmenin gerçekleştiği yer burasıdır. Aşağıdaki ekran görüntüsünde yapıcı metodun alacağı parametreler açık bir şekilde görülmektedir.

![mk186_6.gif](/assets/images/2006/mk186_6.gif)

Artık yapmamız gereken tüm hazırlığı tamamlamış durumdayız. Uygulamamızı test edersek eğer, kontrollerimizin başarılı bir şekilde ilgili tabloya bağlandığını görebiliriz. Örneğin aşağıdaki ekran görüntüsünde Bayan Giyim sayfasına gidilmiştir.

![mk186_7.gif](/assets/images/2006/mk186_7.gif)

Gördüğünüz gibi, Site haritasını Xml bağımlılığından kurtarma şansına sahibiz. Elbette yazılan bu tip uygulamalarda performasıda düşünecek nitelikte tedbirler almak gerekebilir. Örneğin site haritasını tutan SiteMapNode'un arabelleğe alınması düşünülerek daha hızlı çalışan bir menü sistemi tasarlanabilir. Nitekim uygulamamızda sayfanın istemciden sunucuya her postalanışında, sunucu tarafından ilgili saklı yordam sayfanın yaşam döngüsü içerisinde çalıştırılacaktır. İşte buradaki performans kayıplarının önüne geçmek için ara bellek sistemi (caching) düşünülebilir. Benzer şekilde ilgili sınıf içerisinde siteye dahil olan kullanıcının yetkilerinin kontrol edilmesi ve datanın buna göre çekilerek ilgili boğumların düzenlenmesi düşünelebilir. Ancak çıkış noktamız yukarıda bahsettiğimiz gibidir. Bu makalemizde kısaca site haritamızı nasıl özelleştirebileceğimizi gördük. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek uygulama için tıklayın.](/assets/files/2006/CustomSiteMap.rar)