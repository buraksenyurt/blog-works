---
layout: post
title: "Asp.Net Temelleri : Tablo Bazlı Resimleri Ele Almak"
date: 2007-07-26 12:00:00 +0300
description: "Bir Windows uygulaması göz önüne alındığında, resimleri gösterebilecek bir PictureBox kontrolünün çeşitli özellikleriden yararlanarak herhangibir tabloda tutulan binary içeriği kullanmak ve bu içeriğin işaret ettiği resmi göstermek son derece kolaydır..."
categories:
  - aspnet
tags:
  - aspnet
  - csharp
  - dotnet
  - sql-server
  - http
  - caching
  - dependency-management
---
# Asp.Net Temelleri : Tablo Bazlı Resimleri Ele Almak
Değerli Okurlarım Merhabalar,

Yazın bu sıcak günlerinde daha hafif konularla web maceralarımıza devam ediyoruz. Geçtiğimiz makalemizde Asp.Net uygulamalarında ektin hata yönetiminin nasıl yapılabileceğini incelemeye çalışmıştık. Bu kez veritabanı tablolarında çoğunlukla binary alanlarda saklanan resimlerin, Asp.Net uygulamalarında nasıl ele alınabileceğini örnek projeler üzerinden incelemeye çalışacağız.

Bir Windows uygulaması göz önüne alındığında, resimleri gösterebilecek bir PictureBox kontrolünün çeşitli özellikleriden yararlanarak herhangibir tabloda tutulan binary içeriği kullanmak ve bu içeriğin işaret ettiği resmi göstermek son derece kolaydır. Ne varki Asp.Net uygulamalarında her zaman için, render edilerek istemciye gönderilen bir sayfa içeriği mevcuttur. Bu içeriğin tipi (Content Type) daha farklıdır. Dolayısıyla binary formatta tutulan resimleri ele almak için farklı bir yaklaşım gerekmektedir.

Tabloda binary formatta tutulabilen resimleri Asp.Net uygulamalarında ele almak amacıyla, gösterilmek istenen resmi tek başına yorumlayan bir Asp.Net sayfası mevcuttur. Bu sayfanın tek bir görevi vardır o da ilgili resmi image formatlarından uygun olana göre sayfaya Render etmektir. Bunu incemelek için örnek bir senaryo göz önüne almakta fayda olacağı kanısındayım. Bu amaçla SQL Server 2005 ile birlikte gelen ve Production şemasında (Schema) bulunan Product, ProductPhoto ve ProductProductPhoto tabloları göz önüne alınabilir. Bu tablolar arasındaki ilişki kısaca aşağıdaki şekilde görüldüğü gibidir.

![mk215_1.gif](/assets/images/2007/mk215_1.gif)

ProductPhoto isimli tabloda yer alan ThumbNailPhoto ve LargePhoto isimli alanlarda binary olarak ürün resimleri saklanmaktadır (tam olarak varbinary tipinde). Buna göre ilk örnek senaryomuzda kullanıcılar ürünlerin listelendiği bir sayfadan detay bilgilerini almak için başka bir sayfaya geçiş yapacaklardır. Detayların verildiği sayfada ürüne ait ThumbNailPhoto içeriğide bir resim olarak sayfa gösterilecektir. Başlamadan önce ProductPhoto tablosundaki herhangibir ThumbNailPhoto alanının içeriğini resim olarak nasıl gösterebileceğimize bakalım. Bu amaçla ResimGoster.aspx isimli aşağıdaki gibi bir sayfa tasarlanarak işe başlanabilir.

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ResimGoster.aspx.cs" Inherits="ResimGoster" %>
```

Eminimki ResimGoster isimli web sayfasının içeriği son derece ilginç gelmiştir. Nitekim herhangibir HTML elementi yer almamaktadır. Aslında bu sayfanın tek amacı yüklenirken (bir başka deyişle PageLoad olay metodu çalışırken), ürün resmini ekrana binary olarak yazdırmaktır. Burada elbetteki hangi resmin gösterileceğide önemlidir. Bunun için sayfaya bir şekilde ProductPhotoID alanının değerinin gelmesi gerekmektedir. Bunun için en güzel yol QueryString kullanımıdır. Öyleyse bu sayfanın kodlarını yazarak işe devam edelim.

```csharp
protected void Page_Load(object sender, EventArgs e)
{ 
    string resimId = Request.QueryString["ResimId"];
    if (!String.IsNullOrEmpty(resimId))
    {
        byte[] resimBytes=null;
        using (SqlConnection conn = new SqlConnection("data source=localhost;database=AdventureWorks;integrated security=SSPI"))
        {
            SqlCommand cmd = new SqlCommand("Select ThumbNailPhoto From Production.ProductPhoto  Where ProductPhotoId=@PhotoId", conn);
            cmd.Parameters.AddWithValue("@PhotoId", resimId);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
            if(reader.Read())
                resimBytes = reader.GetSqlBytes(0).Value;
            reader.Close();
            if (resimBytes != null)
            {
                Response.ContentType = "image/gif";
                Response.BinaryWrite(resimBytes);
            }
            else
                Response.Write("Resim gösterilemiyor.");
        } 
    }
    else
        Response.Write("ResimId parametresi eksik yada hatalı.");
}
```

Page_Load olay metodu içerisinde ilk olarak HttpRequest sınıfının static QueryString özelliği yardımıyla sayfaya gelen ResimId değeri alınmaktadır. Kullanıcıların bu sayfayı doğrudan talep etme ihtimaline göre ResimId değerinin boş gelme olasılığı bulunmaktadır. Bu nedenle String sınıfının IsNullOrEmpty metodu ile bir kontrol gerçekleştirilir. Sonrasında ise Production şemasındaki ProductPhoto tablosunda gelen değere göre ilgili ThumbNailPhoto alanı çekilir.

Eğer gelen ResimId ile eşleşen bir ProductPhotoId alanı var ise bu satırın ThumbNailPhoto alanının değeri SqlDataReader nesne örneğine ait Read metodu yardımıyla okunur. Okuma işlemi sırasında GetSqlBytes metodu kullanılmakta ve Value özelliği ile elde edilen byte dizisi resimBytes isimli alana aktarılmaktadır. Diğer taraftan HttpResponse sınıfının ContentType özelliği ile render edilecek sayfanın içeriği belirlenmektedir. Burada image/gif değeri ile basılacak içeriğin gif formatında bir resim olacağı belirtilmektedir.

ContentType özelliğinin varsayılan değeri text/HTML dir. Bu değer, tahmin edileceği üzere sayfanın çıktısının HTML olarak üretileceğini işaret etmektedir. Yaygın olarak kullanılan diğer versiyonlar aşağıdaki gibidir.

- image/gif
- image/jpeg
- text/plain
- application/vnd.ms-excel (çıktının excel dökümanı olmasını sağlar)
- application/vnd.ms-word (çıktının word dökümanı olmasını sağlar)

Son olarak, elde edilen byte dizisinin çıktıya aktarılmasını sağlamak için yine HttpResponse sınıfının static metodlarından BinaryWrite çağırılmaktadır.

Artık sayfayı test ederek işlemlerimize devam edebiliriz. Elbette doğru sonuçları görebilmek için ResimId parametresini Url satırından göndermekte fayda vardır. Aşağıda örnek olarak 120 numaralı ProductPhotoId değerine sahip satır için elde edilen çıktı görülmektedir.

![mk215_2.gif](/assets/images/2007/mk215_2.gif)

Burada, üretilen HTML sayfasının kaynak kodlarına bakılmak istenirse tarayıcı buna izin vermeyebilir (Örneğin Microsoft Internet Explorer 7.0 View Source buna izin vermemiştir). Bu sebepten çıktıyı Save As ile kaydetmek gerekebilir. Kaydedilen çıktının içeriği aşağıdaki ekran görüntüsünde yer aldığı gibi olacaktır. Dikkat edilecek olursa sayfanın çıktısı sonucu oluşturulan içerikte img elementi ve src niteliği yer almaktadır.

![mk215_5.gif](/assets/images/2007/mk215_5.gif)

Elbette satır olarak karşılığı olmayan bir ResimId değeri girilirse sayfa çıktısı tarayıcı penceresinde aşağıdaki gibi olacaktır. Örneğin ProductPhotoId değeri 17 olan bir satır bulunmamaktadır.

![mk215_4.gif](/assets/images/2007/mk215_4.gif)

Bununla birlikte kullanıcı bu sayfayı doğrudan talep eder ve ResimId parametresini kullanmassa aşağıdaki ekran çıktısını elde eder.

![mk215_3.gif](/assets/images/2007/mk215_3.gif)

Burada olası bazı hataların önüne geçilmek amacıyla basit tedbirler alınmış ve ekrana bilgi mesajları verilmiştir. Gerçek hayat uygulamalarında son kullanıcıların daha doğru ve etkin bir şekilde uyarılması bir başka deyişle oluşan hatalar konusunda bilgilendirilmesi gerekmektedir.

Artık tek yapılması gereken senaryoyu biraz daha kullanışlı hale getirmektir. Bu amaçla ürünlerin gösterildiği Urunler.aspx isimli basit bir web sayfası tasarlanarak devam edilbilir. Bu sayfada ürünlere ait bir kaç temel bilgi bulunacak ama detayları için başka bir sayfaya yönlendirmede bulunulacaktır. Yönlendirilme yapılan sayfa tahmin edileceği üzere ürüne ait resmide içeren bir detay sayfasıdır. Urunler.aspx sayfasında basit olarak bir SqlDataSource kontrolü ve bu kontrolü ele alan bir GridView bileşeni düşünülebilir. Buna göre Urunler.aspx sayfasının kaynak kod tarafı aşağıdaki gibi tasarlanabilir.

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Urunler.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Urunler</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="ProductID" DataSourceID="dsProducts">
                <Columns>
                    <asp:HyperLinkField DataNavigateUrlFields="ProductId" DataNavigateUrlFormatString="UrunDetay.aspx?PrdId={0}" DataTextField="Name" HeaderText="Urun Adı" />
                    <asp:BoundField DataField="ListPrice" DataFormatString="{0:C}" HeaderText="Liste Fiyatı" HtmlEncode="False" SortExpression="ListPrice" />
                </Columns>
            </asp:GridView>
            <asp:SqlDataSource ID="dsProducts" runat="server" ConnectionString="<%$ ConnectionStrings:AdvConStr %>" SelectCommand="SELECT Top 20 ProductID, Name, ListPrice FROM Production.Product Where ListPrice>=1000">
            </asp:SqlDataSource>
        </div>
    </form>
</body>
</html>
```

Urunler.aspx sayfasında yer alan GridView kontrolünde HyperLinkField kontrolü kullanılmaktadır. Bu alan, ürünün adını (Name) göstermekte olup üzerine tıklandığında kullanıcıyı UrunDetay.aspx sayfasına göndermektedir. Bu işlem sırasında da PrdId isimli bir QueryString parametresi ProductId alanının değerini detay sayfasına taşımaktadır. Urunler.aspx isimli sayfanın çalışma zamanındaki çıktısı aşağıdaki ekran görüntüsünde yer aldığı gibidir.

![mk215_6.gif](/assets/images/2007/mk215_6.gif)

Gelelim UrunDetay.aspx sayfasına. Bu sayfada basit olarak Urunler.aspx sayfasında seçilen ürünlere ait detay bilgileri gösterilecektir. Ancak önemli olan, seçilen ürünün resmininde ProductPhoto tablosundan binary olarak çekilerek ekrana bastırılacak olmasıdır. Bu amaçla tasarlanan Urunler.aspx sayfasında yine bir SqlDataSource kontrolü ve detaylar için DetailsView bileşeni aşağıdaki gibi kullanılabilir.

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UrunDetay.aspx.cs" Inherits="UrunDetay" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Urun Detaylari</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>Ürün Detayları :<br />
            <asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" DataSourceID="dsProductDetails" Height="50px" Width="294px">
                <Fields>
                    <asp:BoundField DataField="Name" HeaderText="Ürün Adı" SortExpression="Name" />
                    <asp:BoundField DataField="ProductNumber" HeaderText="Ürün Numarası" SortExpression="ProductNumber" />
                    <asp:BoundField DataField="SafetyStockLevel" HeaderText="Stok Seviyesi" SortExpression="SafetyStockLevel" />
                    <asp:BoundField DataField="ReorderPoint" HeaderText="Sipariş Noktası" SortExpression="ReorderPoint" />
                    <asp:BoundField DataField="ListPrice" HeaderText="Liste Fiyatı" SortExpression="ListPrice" DataFormatString="{0:C}" HtmlEncode="False" />
                    <asp:BoundField DataField="StandardCost" HeaderText="Standart Maliyet" SortExpression="StandardCost" DataFormatString="{0:C}" />
                    <asp:TemplateField HeaderText="Urun Resmi">
                        <ItemTemplate>
                            <img alt="Ürün Resmi" runat="server" src='<%#"ResimGoster.aspx?ResimID="+DataBinder.Eval(Container.DataItem, "ProductPhotoID") %>' id="urunResmi" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Fields>
            </asp:DetailsView>
            <asp:SqlDataSource ID="dsProductDetails" runat="server" ConnectionString="<%$ ConnectionStrings:AdvConStr %>" SelectCommand="SELECT Production.Product.ProductID, Production.Product.Name, Production.Product.ProductNumber, Production.Product.SafetyStockLevel, Production.Product.ReorderPoint, Production.Product.ListPrice, Production.Product.StandardCost, Production.ProductProductPhoto.ProductPhotoID FROM Production.Product INNER JOIN Production.ProductProductPhoto ON Production.Product.ProductID =Production.ProductProductPhoto.ProductID WHERE (Production.Product.ProductID = @PrdId)">
                <SelectParameters>
                    <asp:QueryStringParameter DefaultValue="1" Name="PrdId" QueryStringField="PrdId" />
                </SelectParameters>
            </asp:SqlDataSource> 
        </div>
    </form>
</body>
</html>
```

UrunDetay.aspx isimli web sayfasında dikkat edilmesi ve üzerinde durulması gereken bazı noktalar vardır. Öncelikli olarak SqlDataSource kontrolünde kullanılan sorgu basit olarak aşağıdaki şekilde yer aldığı (Query Builder ile elde edilmiştir) gibi Product ve ProductProductPhoto tablolarının join ile birleştirilmiş bir halidir ve ProductId değerinin where ifadesinde ele almaktadır. Nitekim ürün resminin ProductPhoto tablosundan tedariki için ProductPhotoID değerinin bilinmesi gerekmektedir. Bu nedenle Product ve ProductProductPhoto tabloarı Join ile birleştirilmiştir.

![mk215_7.gif](/assets/images/2007/mk215_7.gif)

Diğer taraftan DetailsView kontrolü içerisindede resmin gösterilebilmesi için bir TemplateField kullanılmış ve ItemTemplate şablonu içerisinde img elementi aşağıdaki gibi kullanılmıştır.

```text
<asp:TemplateField HeaderText="Urun Resmi">
                        <ItemTemplate>
                            <img alt="Ürün Resmi" runat="server" src='<%#"ResimGoster.aspx?ResimID="+DataBinder.Eval(Container.DataItem, "ProductPhotoID") %>' id="urunResmi" />
                        </ItemTemplate>
                    </asp:TemplateField>
```

Burada dikkat edilmesi gereken en önemli nokta src niteliğine (attribute) değer atamasının nasıl yapıldığıdır. DataBinder sınıfının Eval metodunu kullanarak o anki satırın içerisinde yer alan ProdcutPhotoId değeri ResimGoster.aspx sayfasına ResimID adlı parametre ile gönderilmektedir. Buda yazımızın başında tasarladığımız ResimGoster sayfasının çağırılması ve bir resim içeriğinin elde edilerek buradaki img kontrolü içerisinde gösterilmesi anlamına gelmektedir. Sonuç itibariyle UrunDetay.aspx sayfası test edildiğinde aşağıdakine benzer bir ekran çıktısı ile karşılaşılacaktır.

![mk215_8.gif](/assets/images/2007/mk215_8.gif)

Buraya kadar yaptıklarımızı özetleyecek olursak eğer, binary olarak tutulan resimlerin gösterilmesi için izlenebilecek yollardan birisinin adımları aşağıdaki gibi olacaktır.

- İlk olarak resim içeriğini binary olarak tarayıcıya basabilecek bir aspx sayfası tasarlanır.
- Sayfanın amacı gereği aspx kaynağında (Source) sadece Page direktifi bırakılır ve diğer içerik silinir. Bu zorunlu değildir. Ancak tavsiye edilen yoldur.
- Sayfanın PageLoad olay metodu kodlanır.
- Page_Load olay metodunda gösterilmek istenen resme ait satırın bulunabilmesi için QueryString'den yararlanılabilir.
- Resme ait binary içeriğin kod tarafında byte dizisi (byte[]) şeklinde ele alınması sağlanır.
- Elde edilen byte dizisinin sayfaya resim olarak basılmasını sağlamak için önce ContentType özelliğinin değeri image/gif veya image/jpeg olarak belirlenir.
- Resmi yazdırmak içinse BinaryWrite metodu çağırılır.

Buradaki örnek göz önüne alındığında istemci sayısının fazla olacağı düşünelecek olursa performansı arttırmak adına parametre bazlı olacak şekilde ön bellekleme (Caching) yapılması sağlanabilir. Böylece ResimGoster.aspx sayfasının sürekli olarak PageLoad kodlarını çalıştırmasının önüne geçilmiş olunur.

> Resimlerin çok sık değişmediği düşünülüyorsa ve SQL Server kullanılıyorsa tablo bağımlı bir ön belleklemede yapılabilir (Sql Cache Dependency).

Söz gelimi ResimGoster.aspx dosyasının içeriği aşağıdaki gibi değiştirilerek sayfanın çıktısının belirli bir süreliğine (örneğin 60 saniye boyunca) ön bellekte tutulması sağlanabilir.

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ResimGoster.aspx.cs" Inherits="ResimGoster" %>
<%@ OutputCache Duration="60" VaryByParam="ResimID" %>
```

Gelelim resimlerin işlenmesi ile ilgili diğer bir yaklaşıma. Örnekte resmi gösteren img elementinin src niteliğinde bir url adresinden yararlanılmaktadır. Elbetteki bu işlem programatik olarak kod üzerindende geliştirilebilir. Örneğin ürün bilgilerini bir DataList kontrolü üzerinde göstermek istediğimizi varsayalım. Bu kez resim alanlarının DataList kontrolü içerisindeki img elementinin src niteliğine bağlanmasını kod tarafından gerçekleştirmeye çalışacağız. Bu amaçla aşağıdaki gibi UrunListesi.aspx isimli bir web sayfası hazırlanarak işlemlere devam edilebilir.

![mk215_9.gif](/assets/images/2007/mk215_9.gif)

```text
<form id="form1" runat="server">
    <div>
        <asp:DataList ID="DataList1" runat="server" DataSourceID="dsProducts" Width="600px">
            <ItemTemplate>
                <table>
                    <tr>
                       <td colspan="2">
                            <asp:Label ID="NameLabel" runat="server" Font-Italic="True" ForeColor="#000040" Text='<%# Eval("Name") %>'></asp:Label>
                        </td>
                        <td style="width: 100px; text-align: right">
                            <asp:Label ID="ProductIDLabel" runat="server" Font-Bold="True" ForeColor="#C00000" Text='<%# Eval("ProductID") %>'></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td rowspan="3" style="width: 100px">
                            <img runat="server" id="urunResmi" alt="Urun Resmi" src="" />
                        </td>
                        <td style="width: 100px">Standart Maliyet</td>
                        <td style="width: 100px">
                            <asp:Label ID="StandardCostLabel" runat="server" Text='<%# Eval("StandardCost", "{0:C}") %>'></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">Liste Fiyatı</td>
                        <td style="width: 100px">
                            <asp:Label ID="ListPriceLabel" runat="server" Text='<%# Eval("ListPrice", "{0:C}") %>'></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">Sınıf</td>
                        <td style="width: 100px">
                            <asp:Label ID="ClassLabel" runat="server" Text='<%# Eval("Class") %>'></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3"><hr /></td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:DataList>
        <asp:SqlDataSource ID="dsProducts" runat="server" ConnectionString="<%$ ConnectionStrings:AdvConStr %>" SelectCommand="SELECT Top 20 P.ProductID, P.Name, P.StandardCost, P.ListPrice, PP.ProductPhotoID, P.Class FROM Production.Product P INNER JOIN Production.ProductProductPhoto PP ON P.ProductID = PP.ProductID WHERE P.ListPrice>1400">
        </asp:SqlDataSource>
    </div>
</form>
```

UrunListesi.aspx sayfasında yer alan DataList kontrolü Production şemasındaki Product ve ProductProductPhoto tablolarının birleşiminden oluşan sonuç kümesinden ilk 20 satırı göstermek üzere tasarlanmıştır ve hatta ListePrice alanına göre filtreleme eklenmiştir. Burada özellikle üzerinde durmamız gereken nokta, img elementidir. ItemTemplate üzerindeki tablo içerisine yerleştirilen img elementinin src niteliğini kod tarafında ele alabilmek için DataList kontrolünün ItemDataBound olayından yararlanılabilir. Bu olay metodu içerisinde söz konusu img elementi bulunmalı ve src niteliğine o anki satırın ProductPhotoId değeri QueryString parametresi olarak aktarılmalıdır. O halde bu amaçla aşağıdaki kod parçasını yazmamız yeterli olacaktır.

```csharp
protected void DataList1_ItemDataBound(object sender, DataListItemEventArgs e)
{
    if (e.Item.ItemType == ListItemType.Item
        || e.Item.ItemType == ListItemType.AlternatingItem)
    {
        // Önce ProductPhotoId değeri bulunmalı
        string resimId = DataBinder.Eval(e.Item.DataItem, "ProductPhotoId").ToString();
        // img elementi bulunmalı ve src niteliğinin değeri değiştirilmeli.
        HtmlImage resimElementi = (HtmlImage)e.Item.FindControl("urunResmi");
        resimElementi.Src = "~/ResimGoster.aspx?ResimId=" + resimId;
    }
}
```

ItemDataBound olayı DataList içerisindeki her bir satır için çalışacağından, işlemleri sadece Item ve AlternatingItem tipindeki satırlarda yapmakta fayda vardır. Bu amaçla DataListItemEventArgs tipinden olan e isimli parametrenin özelliklerinden faydalanılmaktadır. Sonrasında ise o anki satır ile gelen ProductPhotoId değerinin elde edilmesi gerekmektedir. Bu amaçlada DataBinder sınıfının Eval metodu ele alınmaktadır. İlk parametre ile o anki veri satırı yakalanmakta, ikinci parametre ilede söz konusu veri satırındaki ProductPhotoId değeri istenmektedir. Eval metodu geriye Object tipinden bir değer döndürdüğü için bilinçli olarak string tipine dönüştürülmüştür. Nitekim url katarında kullanılacak bilgi string'dir.

Bu işlemlerin ardından img kontrolünün elde edilmesi sağlanır. Bunun içinde e.Item üzerinden FindControl metodu çağırılmıştır. FindControl metodunun parametresi kontrolün id değerinin işaret etmektedir. Hatırlanacağı üzere img kontrolünün id niteliğine kaynak tarafında urunResmi adı verilmiştir. img kontrolü HtmlImage tipinden bir kontroldür ve FindControl metodu geriye Control tipinden bir referans döndürdüğünden sonuç referansı bilinçli olarak HtmlImage tipine dönüştürülmüştür. Son olarak elde edilen HtmlImage kontrolüne ait referans üzerinden Src niteliğinin değeri değiştirilir. Burada yapılan işlem tüm yazı boyunca üzerinde durduğumuz konudur. Uygulamayı bu haliyle çalıştırdığımızda aşağıdakine benzer bir ekran görüntüsü elde ederiz.

![mk215_10.gif](/assets/images/2007/mk215_10.gif)

Bu makalemizde tablolarda binary olarak tutulan resim alanlarını web sayfalarında küçük bir hile ile nasıl işleyebileceğimizi incelemeye çalıştık. Görsellik hemen hemen tüm uygulamalarda önemli bir faktör olduğundan resim alanlarının bu şekilde ele alınıyor olmasını bilmek önemli bir avantaj sağlamaktadır. Kullanılan teknikte önemli olan nokta binary alanın içeriğini tek başına ele alıp resim formatında çıktı veren bir sayfanın var olmasıdır. Ayrıca resmi gösteren kontrolün basit bir img bileşeni olduğuna ve src niteliğinin önemine dikkat edilmelidir. Bu sayfanın çıktısı örneklerden de görüldüğü gibi pek çok farklı biçimde kullanılabilir ve son kullanıcıya görsel olarak daha doyurucu bir içerik sağlanabilir. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](https://www.buraksenyurt.com/admin/app/editor/makale/http:/www.buraksenyurt.com/makale/images/ImageKullanimi.zip)