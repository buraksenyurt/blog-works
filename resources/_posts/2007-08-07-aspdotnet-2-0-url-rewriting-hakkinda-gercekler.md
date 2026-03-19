---
layout: post
title: "Asp.Net 2.0 URL Rewriting Hakkında Gerçekler"
date: 2007-08-07 12:00:00 +0300
description: "Çok kısa süreliğinede olsa tatilde olduğum şu günlerde yazılım dünyasından kopmak hiç içimden gelmedi. Bu nedenle dinlendiğim zamanlardan arta kalan sürelerde azda olsa bir şeyler karalamak istedim. Sonuç olarak daha hafif ve tatil moduna uygun olacak bir yazı ile yeniden beraberiz. Bu makalemizde Asp.Net 2.0 ile geliştirilen web uygulamalarında, URL eşleştirmelerinin (Url Mapping) nasıl düzenlenebileceğini, bir başka deyişle nasıl özelleştirilebileceğini incelemeye çalışacağız. Son kullanıcılar web ortamında, kendi tarayıcı(browser) uygulamalarında yer alan adres satırlarında zaman zaman karışık ve uzun URL bilgileri ile karşılaşırlar. Genellikle sorgu katarlarının(QueryString) kullanıldığı ve bunların sayılarının çok olduğu durumlarda adres satırlarını okumak gerçekten güçleşebilir. Söz gelimi aşağıdaki URL bilgisini göz önüne alalım."
categories:
  - aspnet
tags:
  - aspnet
  - xml
  - dotnet
  - sql-server
  - t-sql
  - http
  - authentication
---
# Asp.Net 2.0 URL Rewriting Hakkında Gerçekler
Değerli Okurlarım Merhabalar,

Çok kısa süreliğinede olsa tatilde olduğum şu günlerde yazılım dünyasından kopmak hiç içimden gelmedi. Bu nedenle dinlendiğim zamanlardan arta kalan sürelerde azda olsa bir şeyler karalamak istedim. Sonuç olarak daha hafif ve tatil moduna uygun olacak bir yazı ile yeniden beraberiz. Bu makalemizde Asp.Net 2.0 ile geliştirilen web uygulamalarında, URL eşleştirmelerinin (Url Mapping) nasıl düzenlenebileceğini, bir başka deyişle nasıl özelleştirilebileceğini incelemeye çalışacağız. Son kullanıcılar web ortamında, kendi tarayıcı (browser) uygulamalarında yer alan adres satırlarında zaman zaman karışık ve uzun URL bilgileri ile karşılaşırlar. Genellikle sorgu katarlarının (QueryString) kullanıldığı ve bunların sayılarının çok olduğu durumlarda adres satırlarını okumak gerçekten güçleşebilir. Söz gelimi aşağıdaki URL bilgisini göz önüne alalım.

```text
http://www.azonsitesi.com/urunler.aspx?urunKategori=1&urunAdi=Bilgisayar%20Kitaplari&Sinifi=Ingilizce&BasimYili=2006
```

Bunun yerine aşağıdaki gibi bir URL bilgisi çok daha kullanışlı ve son kullanıcı açısından okunaklı olabilir.

```text
http://www.azonsitesi.com/Ingilizce/BilgisayarKitaplari/2006Basimi/Goster.aspx
```

Örnekler çoğaltılabilir. Söz gelimi blog sitelerinde, adres satırlarında okunan içeriğe ait bilgilerin QueryString şeklinde durması yerine örneğinhttp://buraginblogu/Agustos/7/2007/UrlEslestirme/Oku.aspx gibi bir formata sahip olması son kullanıcı açısından çok daha cezbedicidir.

Bu ve benzer durumlarda, adres satırındaki bilginin daha kolay anlaşılabileceği hale getirilmesi son kullanıcı (End User) için önemli bir hizmettir. Peki bu tarz bir ihtiyaç nasıl karışalanabilir. İlk olarak istemciden gelen adres talebine eş düşecek yeni URL bilgisinin sunucu tarafında ele alınıyor olması gerekir. Sonrasında ise sonuçlar istemci tarayıcı programına istenen formatta gönderilir. Asp.Net 1.1 kullanıldığı takdirde bu işin çözümü özel HttpHandler ve HttpModule sınıflarının yazılması ile mümkün olabilir.(Kendi HttpHandler yada HttpModuler tiplerimizi nasıl yazabileceğimize dair bilgileri daha önceden yayınlanan [makalemizden](http://www.bsenyurt.com/MakaleGoster.aspx?ID=183) takip edebilirsiniz) Asp.Net 2.0 mimarisindeyse, sadece URL eşleştirilmelerinin daha kolay yapılabilmesini sağlamak amacıyla web.config dosyasında yer alan system.web boğumu (node) içerisinde ele alınabilecek bir urlMappings elementi ve bunun Configuration API'sinde karşılğı olan UrlMappingsSection sınıfı geliştirilmiştir. Bu sayede konfigurasyon bazında URL eşleştirmeleri yapılabilmekte ve özel HttpHandler yada HttpModule tipleri yazılmasına gerek kalmamaktadır.

> Her ne kadar urlMappings elementi veya UrlMappingsSection tipi sayesinde, URL eşleştirmelerinin yapılması kolaylaşmışsada bazı özel durumlarda yine HttpHandler veya HttpModule tipleri geliştirmek gerekebilir. Söz gelimi, urlMappings elementinin doğrudan birregular expression desteği yoktur. Dolayısıyla benzer yazıma sahip URL bilgileri için ortak eşleştirme yapmak adına element bazında bir hamle yapılması zordur. Bunu sağlamak için HttpModule ve HttpHandler yazmak gerekmektedir. Böyle bir ihtiyaç için kendi HttpModule ve HttpHandler tipinizi yazmaya çalışmanız önerilir.

Aslında Asp.Net 2.0 mimarisinde yer alan URL eşleştirme sistemi aşağıdaki grafikte görüldüğü gibi çalışmaktadır.

![mk217_1.gif](/assets/images/2007/mk217_1.gif)

Buna göre istemciden gelen talepler sonrası, ilgili web uygulaması Asp.Net çalışma zamanı içerisinde normal sürecine devam eder. Taki sayfanın son hali Render işlemine tabi tutulup istemciye gönderilene kadar. Bir başka deyişle Render işleminde önce, Asp.Net çalışma zamanı (Asp.Net RunTime) web.config içerisinde herhangibir eşleştirme olup olmadığına bakar. Eğer talep edilen URL için bir eşleştirme varsa buna göre HttpContext tipinin RewritePath metodu işletilir ve URL adresi değiştirilir. Sonrasında ise sayfa istemciye gönderilir.

Dilerseniz örnek bir senaryo üzerinden hareket ederek URL eşleştirmelerinin Asp.Net 2.0 mimarisinde nasıl yapıldığını yakından incelemeye çalışalım. Senaryo gereği kullanıcının alt kategorisi ve sınıfına göre bazı ürünleri listelediğini düşünebiliriz. Bu amaçla SQL Server 2005 ile gelen AdventureWorks veritabanındaki ProductSubCategories ve Product tablolarını göz önüne alalım. Bu tablolara arasında aşağıdaki şekilde görülen bire-çok (one to many) ilişki vardır.

![mk217_2.gif](/assets/images/2007/mk217_2.gif)

İlk olarak default.aspx sayfamızı aşağıdaki gibi geliştirelim.

```text
<%@ Page Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">
</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>URL Mapping Ornegi</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:GridView ID="GridView1" runat="server" DataSourceID="dsCategories" AllowPaging="True" AutoGenerateColumns="False">
            <Columns>
                <asp:HyperLinkField DataNavigateUrlFields="Name,Class" DataNavigateUrlFormatString="~/Urunler/{0}/{1}/Goster.aspx" DataTextField="Title" HeaderText="Alt Kategori ve Sınıfı" />
            </Columns>
        </asp:GridView>
        <asp:SqlDataSource ID="dsCategories" runat="server" ConnectionString="<%$ ConnectionStrings:AdvConStr %>" SelectCommand="SELECT DISTINCT PSC.ProductSubcategoryID, Replace(PSC.Name,' ','') AS Name, RTRIM(PRD.Class) as Class, PSC.Name+' '+PRD.Class AS Title FROM Production.ProductSubcategory AS PSC INNER JOIN Production.Product AS PRD ON PSC.ProductSubcategoryID = PRD.ProductSubcategoryID WHERE (PRD.Class IS NOT NULL)">
        </asp:SqlDataSource>
    </div>
    </form>
</body>
</html>
```

Default.aspx sayfası içerisinde yer alan GridView kontrolü, Product ve ProductSubCategory tablolarının birleşiminden bir sonuç kümesine ait satırları göstermek üzere tasarlanmıştır. Söz konusu sonuç kümesi elde edilirken Name ve Class alanındaki boşlukların alınması için Replace ve RTrim isimli T-SQL fonksiyonlarına başvurulmaktadır. GridView bileşenine dikkat edilecek olursa içeride HyperLinkField tipinden bir kontrol kullanılmaktadır. Bu kontrolün dikkate değer özelliği ise DataNavigateUrlFormatString niteliğidir. Sayfa çalışma zamanında aşağıdakine benzer bir sonuç verecektir.

![mk217_6.gif](/assets/images/2007/mk217_6.gif)

Dikkat edilecek olursa bağlantıların (Links) hedef URL bilgisi, HyperLinkField kontrolünün DataNavigateUrlFormatString niteliğinin değerine göre şekillenmektedir. Söz gelimi, örnek ekran görüntüsünde yer aldığı gibi M sınıfındaki Bottom Brackets ürünleri için URL bilgisi aşağıdaki gibidir.

```text
http://localhost:1292/UrlRewriting/Urunler/BottomBrackets/M/Goster.aspx
```

Dikkat edilecek olursa bu URL bilgisinin Urunler kelimesinden itibaren olan kısmı çok daha okunaklı ve anlamlıdır. Peki biz bu linke tıkladığımızda Bottom Brackets kategorisinde ve M sınıfında yer alan ürünlerin listesi nasıl elde edilebilir. Bu işlemin Urunler.aspx gibi bir sayfa içerisinde ele alınması düşünüldüğü takdirde, seçilen bağlantı bilgisine göre ilgili kategori ve sınıf bilgilerinin sorgu katarı (QueryString) ile diğer sayfaya gönderilmesi gerekmektedir. Buda çok doğal olarak, default.aspx sayfasında seçilen URL bilgisine eş düşecek asıl URL bilgisinin tanımlanması ile mümkün olabilir. İşte bu noktada, Urunler.aspx sayfasını tasarlamadan önce, web.config içerisinde aşağıdaki ilaveler yapılmalıdır.

```xml
<?xml version="1.0"?>
<configuration>
    <appSettings/>
    <connectionStrings>
        <add name="AdvConStr" connectionString="Data Source=.;Initial Catalog=AdventureWorks;Integrated Security=True" providerName="System.Data.SqlClient"/>
    </connectionStrings>
    <system.web>
        <urlMappings enabled="true">
            <add url="~/Urunler/BottomBrackets/H/Goster.aspx" mappedUrl="~/Urunler.aspx?AltKategoriId=5&AltKategoriAdi=Bottom%20Brackets&Sinifi=H"/>
            <add url="~/Urunler/BottomBrackets/L/Goster.aspx" mappedUrl="~/Urunler.aspx?AltKategoriId=5&AltKategoriAdi=Bottom%20Brackets&Sinifi=L"/>
            <add url="~/Urunler/BottomBrackets/M/Goster.aspx" mappedUrl="~/Urunler.aspx?AltKategoriId=5&AltKategoriAdi=Bottom%20Brackets&Sinifi=M"/>
            <add url="~/Urunler/Cranksets/H/Goster.aspx" mappedUrl="~/Urunler.aspx?AltKategoriId=5&AltKategoriAdi=Cranksets&Sinifi=H"/>
        </urlMappings>
        <compilation debug="true"/>
        <authentication mode="Windows"/>
    </system.web>
</configuration>
```

URL eşleştirmeleri için system.web elementi içerisinde yer alan urlMappings boğumu kullanılmaktadır. Bu boğumda, add elementi içerisinde yer alan url ve mappedUrl nitelikleri ilede gereken eşleştirmeler yapılmaktadır. Buna göre, ~/Urunler/BottomBrackets/M/Goster.aspx gibi bir talep geldiğinde bu, ~/Urunler.aspx?AltKategoriId=5&AltKategoriAdi=Bottom%20Brackets&Sinifi=M olarak algılanacaktır. & operatörünün ele alınması sırasında & ifadesinin kullanılmasına dikkat etmek gerekir. Eğer & işareti kullanılırsa derleme zamanıda hata mesajı alınır.

![mk217_10.gif](/assets/images/2007/mk217_10.gif)

Artık gerekli bildirimler yapıldığına göre Urunler.aspx sayfası aşağıdaki gibi tasarlanabilir.

```text
<%@ Page Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        lblAltKategoriAdi.Text = Request.QueryString["AltKategoriAdi"]!=null?Request.QueryString["AltKategoriAdi"].ToString():"";
        lblSinifi.Text = Request.QueryString["Sinifi"]!=null?Request.QueryString["Sinifi"].ToString():"";
    } 
</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Kategori ve sınıf bazlı ürünler</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="lblAltKategoriAdi" runat="server" Font-Bold="True" Font-Size="X-Large" Font-Underline="True" ForeColor="#C00000"></asp:Label>
        Alt Kategorisi
        <asp:Label ID="lblSinifi" runat="server" Font-Bold="True" Font-Size="X-Large" Font-Underline="True" ForeColor="#C00000"></asp:Label> sınıfı<br />
        <br />
        <asp:GridView ID="grdUrunler" runat="server" AutoGenerateColumns="False" DataKeyNames="ProductId" DataSourceID="dsProducts">
            <Columns>
                <asp:BoundField DataField="ProductId" HeaderText="ProductId" InsertVisible="False" ReadOnly="True" SortExpression="ProductId" />
                    <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                    <asp:BoundField DataField="ListPrice" HeaderText="ListPrice" SortExpression="ListPrice" />
                    <asp:BoundField DataField="Class" HeaderText="Class" SortExpression="Class" />
                    <asp:BoundField DataField="SellStartDate" HeaderText="SellStartDate" SortExpression="SellStartDate" />
            </Columns>
        </asp:GridView>
        <asp:SqlDataSource ID="dsProducts" runat="server" ConnectionString="<%$ ConnectionStrings:AdvConStr %>" SelectCommand="Select ProductId,Name,ListPrice,Class,SellStartDate From Production.Product Where ProductSubCategoryId=@SubCatId and Class=@Class">
            <SelectParameters>
                <asp:QueryStringParameter DefaultValue="1" Name="SubCatId" QueryStringField="AltKategoriId" />
                <asp:QueryStringParameter DefaultValue="M" Name="Class" QueryStringField="Sinifi" />
            </SelectParameters>
        </asp:SqlDataSource>
    </div>
    </form>
</body>
</html>
```

Bu sayfa sadece QueryString ile gelen parametreler değerlendirmekte ve buna göre belirli bir alt kategori ve sınıfa ait ürünlerin bazı alanlarının listelenmesini sağlamaktadır. Bu amaçla yine GridView ve SqlDataSource kontrollerinden yararlanılmış ve uygun sorgu cümleleri kullanılmıştır. Örnek senaryoda yer alan Bottom Brockets alt kategorisi ve M sınıfı seçilirse, Urunler.aspx sayfasında aşağıdaki ekran görüntüsünde yer alan çıktılar elde edilecektir.

![mk217_5.gif](/assets/images/2007/mk217_5.gif)

Yukarıdaki çıktıya dikkat edilecek olursa URL satırı bizim belirlediğimiz şekilde kalmıştır. Bu çıktının aynısını elde etmek için halen daha sorgu katarı (QueryString) ifadeleride açıkça kullanılabilir. Aynı web uygulamasında aşağıdaki ekran görüntüsünde yer alan URL talebi, aynı sonuçları verecektir.

![mk217_4.gif](/assets/images/2007/mk217_4.gif)

Ne varki halen daha bazı problemler vardır. Herşeyden önce en azından söz konusu senaryo göz önüne alındığında, var olabilecek tüm olasılıklar için web.config dosyasına teker teker URL eşleştirmelerinin eklenmesi gerekmektedir. Bir geliştirici olarak bunun kod içerisinden daha etkili bir şekilde ele alınması çok doğal bir istektir. Neyseki Asp.Net 2.0 ile gelen güçlü Configuration API alt yapısı sayesinde istersek, kod içerisinde urlMappings kısmına dinamik olarak yeni elemanlar (elements) ekleyebilir, düzenleyebilir ve çıkartabiliriz. Bu amaçla bir admin sayfası veya farklı bir uygulama olacak şekilde bir admin paneli dahi göz önüne alınabilir.(Configuration API'sinin yönetiminin daha detaylı bir şekilde öğrenmek isterseniz daha önce yazılmış olan bir [makaleden](http://www.bsenyurt.com/MakaleGoster.aspx?ID=163) yararlanabilirsiniz.) Bunun için aşağıdaki gibi bir sayfa tasarladığımızı göz önüne alabiliriz.

```text
<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Web.Configuration" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

    private static void AddMappings()
    {
        Configuration cfg = ConfigurationManager.OpenExeConfiguration("");
        UrlMappingsSection urlMapSct = (UrlMappingsSection)cfg.GetSection("system.web/urlMappings");
        urlMapSct.UrlMappings.Clear();

        string query = "SELECT DISTINCT PSC.ProductSubcategoryID, PSC.Name,PRD.Class FROM Production.ProductSubcategory AS PSC INNER JOIN Production.Product AS PRD ON PSC.ProductSubcategoryID = PRD.ProductSubcategoryID WHERE (PRD.Class IS NOT NULL) ORDER BY PSC.Name, PRD.Class";

        using (SqlConnection conn = new SqlConnection(cfg.ConnectionStrings.ConnectionStrings["AdvConStr"].ConnectionString))
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();
            string url = "", mappedUrl = "";
    
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                url = "~/Urunler/" + reader.GetString(1).Replace(" ","") + "/" + reader.GetString(2).Replace(" ","") + "/Goster.aspx";
                mappedUrl = "~/Urunler.aspx?AltKategoriId=" + reader["ProductSubCategoryID"].ToString() + "&AltKategoriAdi=" + reader["Name"].ToString() + "&Sinifi=" + reader["Class"].ToString();
                UrlMapping map = new UrlMapping(url, mappedUrl);
                urlMapSct.UrlMappings.Add(map);
            }
            reader.Close();
        }
        cfg.Save();
    }

    protected void btnMappEkle_Click(object sender, EventArgs e)
    {
        AddMappings();
    } 

</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <title>Yonetici Sayfasi</title>
    </head>
    <body>
        <form id="form1" runat="server">
            <div>
                <asp:Button ID="btnMappEkle" runat="server" Text="Güncel URL Eşleştirmelerini Ekle" OnClick="btnMappEkle_Click" />
            </div>
        </form>
    </body>
</html>
```

Şimdi burada geliştirdiğimiz kodları kısaca inceleyelim. Configuration alt yapısına göre, web.config dosyası içerisinde bilinen hemen her boğumun (Node) birer yönetimli tip (Managed Type) karşılığı vardır. Buradaki tipimiz UrlMappingsSection sınıfıdır. UrlMappingsSection sınıfıda Asp.Net 2.0 ile birlikte gelmiş bir sınıftır. Sınıf diagramından ele alındığında UrlMappingsSection sınıfının Framework içerisindeki yeri aşağıdaki şekilde görüldüğü gibidir.

![mk217_11.gif](/assets/images/2007/mk217_11.gif)

Görüldüğü gibi UrlMappingsSection sınıfının UrlMappings özelliği aslında UrlMapping tipinden elemanlar taşıyan özel bir koleksiyonu (UrlMappingCollection) işaret etmektedir. Yeni bir UrlMapping nesne örneği eklenmek istendiğinde bu koleksiyondan yararlanılır. Bu koleksiyondaki elemanları oluşturan UrlMapping tipinin Framework içerisindeki yeri ise aşağıdaki gibidir.

![mk217_12.gif](/assets/images/2007/mk217_12.gif)

Bu tipi o anki web.config dosyasından elde etmek amacıyla Configuration nesne örneğinin GetSection metodu kullanılmaktadır. Bu metod bilindiği gibi XPath ifadelerini parametre olarak alır.(XPath ile ilgili detaylı bilgiyi daha önceki bir [makalemizden](http://www.bsenyurt.com/MakaleGoster.aspx?ID=147) edinebilirsiniz) Sonrasında ise veritabanından yapılan sorgu sonucu elde edilen veri kümesine göre url ve mappedUrl değerleri oluşturulur. Bu değişkenler, her bir satır için web.config dosyasına eklenecek UrlMapping tiplerinin yapıcı metodlarına (Constructor) parametre olarak verilmektedir. Son olarak oluşan UrlMapping nesne örneği, Add metodu ile web.config/urlMappings elementi içerisindeki yerini almaktadır.

Elbette, bellek üzerinde yapılan bu değişikliklerin kalıcı olması adına Configuration tipinin Save metodu kullanılmaktadır. Dikkat edilmesi gereken noktalardan biriside URL değeri oluşturulurken Replace metodu ile Name ve Class alanlarındaki boşlukların alınmasıdır. Eğer söz konusu boşluklar alınmassa (özellike Class alanlarındakiler alınmassa) bu linklere tıklandığında çalışma zamanında sayfaların bulunamadığına dair hata mesajları alınabilir. Bu aynı zamanda, URL içerisinde geçersiz olabilecek karakterler var ise bunlarında çıkartılması veya değiştirilmesi gerektiği anlamınada gelmektedir. Örneğin boşlukar çoğunlukla URL satırına %20 şeklinde aktarılırlar. Ancak örnekteki URL bilgisinde klasör tabanlı bir yaklaşım tercih edildiğinden bütün boşlukların çıkartılması yolu tercih edilmiştir. Bu işlemlerin arkasından Admin sayfası çalıştırılır ve düğme tıklanırsa web.config dosyasının aşağıdaki ekran görüntüsünde olduğu gibi değiştiği görülür.

![mk217_7.gif](/assets/images/2007/mk217_7.gif)

Dikkat edilecek olursa, elde edilen veri kümesine göre, söz konusu olabilecek tüm URL eşleştirmeleri ilave edilmiştir. Configuration API'si sağolsun:) Şimdi default.aspx sayfası çağırılırsa, artık GridView kontrolü içerisindeki her bir bağlantının karşılığının olduğu ve çalıştığı görülür. Default.aspx için örnek görüntü aşağıdaki gibidir.

![mk217_8.gif](/assets/images/2007/mk217_8.gif)

İlgili bağlantının tıklanması sonrası ise aşağıdaki sonuçlara benzer çıktılar elde edilebilir.

![mk217_9.gif](/assets/images/2007/mk217_9.gif)

Buraya kadar örnek bir senaryo üzerinden URL eşleştirmesini incelemeye çalıştık. Son teknikte dinamik olarak URL eşleştirmelerini ekledik. Bu teknik her ne kadar göze hoş gelsede dezavantajıda vardır. Öyleki, veri kaynağında değişiklikler yapıldığında örneğin Alt kategori adı değiştiğinde yada yenileri eklendiğinde web.config dosyası içerisinde yeni düzenlemelerin yapılması, bir başka deyişle ilgili fonksiyonun tekrardan çağırılması gerekcektir. Bu çeşitli teknikler ile çözülebilir ama yinede düşünülmesi gereken bir adımdır.

Sonuç olarak Asp.Net 2.0 ile birlikte gelen urlMappings elementi her ne kadar bazı avantajlar sağlasada çeşitli kısıtlamalarda içermektedir. Regular Expression ifadeleri kullanılamadığından, her bir URL için gereken eşleştirmeler teker teker web.config dosyası içerisinde yazılmalıdır. Gerçi Configuration API'si yardımıyla bu bir nebzede olsa aşılabilmektedir ancak Regular Expression'ın yerini tam anlamıyla tutmamaktadır. Regular Expression desteği için geliştiricinin özel HttpModule ve HttpHandler tiplerini geliştirmesi gerekliliği ise bir zorluktur. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](/assets/files/2007/UrlRewriting.zip)