---
layout: post
title: "Asp.Net 2.0 ile Veri Kümelerinde Sayfalama İşlemleri"
date: 2004-09-01 12:00:00 +0300
categories:
  - aspnet-2-0
tags:
  - aspnet-2-0
  - csharp
  - dotnet
  - aspnet
  - sql-server
  - http
  - visual-studio
  - datatable
---
Bu makalemizde, Asp.Net 2.0 ile geliştirilen sayfalarda, veri kümeleri üzerinde sayfalama işlemlerinin nasıl yapıldığını incelemeye çalışacağız. Sayfalama işlemleri, özellikle internet (intranet) uygulamalarında yaygın şekilde kullanılan bir tekniktir. Burada, veri kümesine ati olan satırlar, DataGrid gibi bir kontrolde gösterilirken, sayfalara ayrılırlar. Böylece veri kümesine ait satırlar arasında toplu geçiş yapmamıza imkan sağlayan, navigasyon seçeneklerine sahip olmuş oluruz.

Asp.Net 2.0 içinde aynı imkanlar ve kabiliyetler söz konusudur. Ancak uygulanış şekli ve kullanılan bileşenler çok daha farklıdır. Herşeyden önce, Asp.Net 1.0/1.1 de izlenen yollara nazaran, daha kısa ve etkili bir teknik geliştirilmiştir. Asp.Net'in ilk sürümleri ile geliştirilen uygulamalarda, sayfalama işlemlerinin gerçekleştirilmesi için DataGrid kontrollerinde ekstradan olay prosedürü kodlamamız gerekmektedir. Bu bizim için fazla maliyettir. Nitekim zaman kaybettirici bir işlemdir. Ne demek istediğimi ve Asp.Net 2.0' da hangi noktaya geldiğimizi görmek için Visual Studio.Net 2003 ile geliştirilen aşağıdaki internet sayfasına bir göz atalım.

```text
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=windows-1252">
<title>New Page 1</title>
</head>
<body>
<%@ Page language="c#" Codebehind="WebForm1.aspx.cs" AutoEventWireup="false" Inherits="Sayfalama.WebForm1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<title>WebForm1</title>
<form id="Form1" method="post" runat="server">
<asp:DataGrid id="DataGrid1" runat="server" AllowPaging="True">
<PagerStyle Mode="NumericPages"></PagerStyle>
</asp:DataGrid>
</form>
</body>
</html>
```

Bu aspx sayfasında, DataGrid kontrolü üzerinde sayfalama işlemini gerçekleştirebilmek için AllowPaging özelliğine true değeri atanmıştır. Bununla birlikte sayfalamanın sayısal olarak gerçekleştirilmesi için PagerStyle takısı eklenmiş ve bu takının Mode özelliğine NumericPages değeri atanmıştır. Code-Behind kodlarımıza bakacak olursak, benzer uygulamalarda da muhtemelen aşağıdaki kod satırlarında kullanılan tarzda bir veri elde ediliş yöntemi uygulandığını görürüz.

```csharp
<%@ Page language="c#" Codebehind="WebForm1.aspx.cs" AutoEventWireup="false" Inherits="Sayfalama.WebForm1" %> 
private void veriAl()
{ 
    SqlConnection con=new SqlConnection("data source=localhost;initial catalog=pubs;integrated security=SSPI");
    SqlCommand cmd=new SqlCommand("Select au_lname,au_fname,address From authors",con);
    SqlDataAdapter da=new SqlDataAdapter(cmd);
    DataTable dtYazarlar=new DataTable();
    da.Fill(dtYazarlar);
    DataGrid1.DataSource=dtYazarlar;
    DataGrid1.DataBind();
}
private void Page_Load(object sender, System.EventArgs e)
{
    if(!Page.IsPostBack)
    {
        veriAl();
    }
}

private void DataGrid1_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
{
    DataGrid1.CurrentPageIndex=e.NewPageIndex;
    veriAl();
}
```

Sayfamızı bu haliyle çalıştırdığımızda, veri kümesi içerisinde sayfalama işlemlerinin gerçekleştirilebildiğini görürüz. Elde ettiğimiz küme, sayfalara ayrılarak, DataGrid kontrolünde gösterilecektir. Yaptığımız işlem aslında basittir ancak uzundur. Öncelikle bir SqlConnection kontrolü ile veri kaynağına bir bağlantı hattı çekilir. Ardından, ilgili veri kümesini elde edebileceğimiz, Select sorgusunu taşıyan bir SqlCommand nesnesi oluşturulur.

SqlCommand nesnesini kullanacak olan bir SqlDataAdapter ise, bağlantısız katmandaki DataTable nesnesini veri kümesi ile doldurmaktadır. Ardından elde edilen DataTable bileşeni DataGrid kontrolüne bağlanır ve DataGrid kontrolü için DataBind metod çalıştırılır. Tüm bu işlemlerin yanında sayfanın PostBack olması durumunda kaybolacak paging özelliğinin önüne geçmek için, Load olayında, IsPostBack kontrolü yapılır.

Ayrıca, DataGrid kontrolünde, sayfalama işlemi sonrası oluşan sayfa numarası linklerine basıldığında, veri kümesinin ilgili parseline gidebilmek için, DataGrid1_PageIndexChanged olay metoduda kodlanmıştır. Bu yol her nekadar bir süre Asp.Net ilee uygulama geliştiren biris için anlaşılır ve kolay gözüksede, takdir edersinizki uzun ve aynı zamanda verimsiz bir yapıdadır. Herşeyden önce, çok fazla kaynak tüketimi söz konusudur.

![mk84_1.gif](/assets/images/2004/mk84_1.gif)

Şekil 1. Asp.Net 1.1 Sayfalam işlemi.

Burada takip ettiğimiz yolu düşüncek olursak, aslında gereksiz yere bir kaç adım işlem yaptığımızı ve bir kaç nesne kaynağını gereksiz yere harcadığımızı düşünebiliriz. Bu teknik dışında, SqlDataReader nesnesini kullanacağımız başka bir yol daha geliştirebilirdik. Ancak hangi yol seçilirse seçilsin, her ikiside uygulama geliştiricinin bir kaç satırda olsa fazladan kod yazmasını ve bazı püf noktalara (DataGrid1_PageIndexChanged olay metodu gibi) dikkat etmesini gerektirecektir. Bu elbetteki uygulama geliştiricinin artan tecrübesi ile önemsiz hale gelebilir.

Ancak işlerin dahada kısaltılarak yapılabileceğide gerçektir. İşte Microsoft mimarları, bu eksikliğin farkına varmış olacaklarki, Asp.Net 2.0' da, sayfalama işlemine farklı bir yakaşım ve uygulama tekniği getirmişler. Herşeyden önce, DataSource kavramını kullanan Framework 2.0 için, sayfalama işlemlerini gerçekleştirmek, Asp.Net 1.0/1.1 sürümlerine göre hem daha kolay hemde daha profesyonel bir anlayışa sahip. Bu yeni teknik sayesinde, uygulama geliştiricinin verimliliğinin daha da artacağı kanısındayım. Şimdi dilerseniz, Asp.Net 2.0' daki duruma bir göz atalım. Bu kez aspx sayfamızın kodlarını aşağıdaki gibi oluşturacağız.

```text
<%@ Page Language="C#" CompileWith="Sayfalama.aspx.cs" ClassName="Sayfalama_aspx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<title>Untitled Page</title>
</head>
<body>
<form id="form1" runat="server">
<div>

    <asp:GridView ID="GridView1" Runat="server" AllowPaging="True" DataSourceID="sqlKaynak">
    </asp:GridView>
    <asp:SqlDataSource runat="Server" ProviderName="System.Data.SqlClient" ConnectionString="data source=.;initial catalog=AdventureWorks;integrated security=true" SelectCommand="SELECT [EmployeeID],[Gender],[HireDate] FROM [AdventureWorks].[HumanResources].[Employee]" ID="sqlKaynak"></asp:SqlDataSource>

</div>
</form>
</body>
</html>
```

Bu kodlara sahip Asp.Net 2.0 sayfamızı çalıştırdığımızda aşağıdaki gibi bir ekran görüntüsü elde ederiz.

![mk84_2.gif](/assets/images/2004/mk84_2.gif)

Şekil 2. Asp.Net 2.0 için sayfalama işlemi.

Burada dikkat edecek olursanız tek satır uygulama kodu yazılmamıştır. Bunun yerine tüm işlemler, GridView ve SqlDataSource Asp.Net kontrolleri ile gerçekleştirilmiştir. Bu kontroller, Asp.Net 2.0 ile gelen sayısız yeni bileşenden sadece ikisidir. GridView kontrolümüzün en önemli özelliği, kendisine veri kaynağı olarak bir SqlDataSource nesnesini bildiren DataSourceID özelliğidir. Bu özellik ile, veri kümesini ala belirttiği veri kaynağından çekecek olan SqlDataSource kontrolünün ID değeri belirtilir.

```text
<asp:GridView ID="GridView1" Runat="server" AllowPaging="True" DataSourceID="sqlKaynak">
    </asp:GridView>
```

SqlDataSource bileşenimiz, uygulama geliştiricisinin verimliliğini arttıran yenilikler içeririr. Asp.Net 1.1 ile yazdığımız bir önceki örneğin aksine, burada veri kaynağına bağlanma, veri kümesini çekme ve bunları ilgili kontrol ile ilişkilendirme işlemleri tek bir bileşen içerisindeki özellikle yardımıyla gerçekleştirilebilmektedir. Bu noktada kontrol her nekadar DataAdapter'ı andırsada çok daha farklı olduğunu ProviderName özelliğine bakarak bile anlayabiliriz.

```text
<asp:SqlDataSource runat="Server" ProviderName="System.Data.SqlClient" ConnectionString="data source=.;initial catalog=AdventureWorks;integrated security=true" SelectCommand="SELECT [EmployeeID],[Gender],[HireDate] FROM [AdventureWorks].[HumanResources].[Employee]" ID="sqlKaynak"></asp:SqlDataSource>
```

SqlDataSource kontrolü ile, burada örnek olarak, Yukon (Sql Server 2005) üzerinde yer alan AdventureWorks veritabanındaki Employee tablosuna bağlanılmıştır. ProviderName özelliği, hangi veri sağlayıcının kullanılacağını belirtir. Biz burada doğal sql motorunu kullanmak istediğimizden, System.Data.SqlClient sınıfını kullandık. ConnectionString ile tahmin edeceğiniz gibi, veri kaynağına bir bağlantı hattı tahsis etmekteyiz. SelectCommand özelliği ilede, çalıştırmak istediğimiz Select sorgusunu tanımlıyoruz. SqlDataSource kontrolü burada GridView kontrolü ile beraber çalışmaktadır. Dolayısıyla, sayfalama işlemini gerçekleştirmek için, GridView kontrolünün AllowPaging özelliğine true değerini atamamız ve kullanılcak DataSource bileşenini belirtmemiz yeterlidir.

Böylece geldik bir makalemizin daha sonuna. Bu makalemizde kısaca Asp.Net 2.0 için sayfalama işlemlerinin nasıl gerçekleştirilebildiğini incelemeye çalıştık. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.