---
layout: post
title: "Numaralandırıcıları Kullanmak İçin Bir Sebep"
date: 2005-08-28 12:00:00 +0300
categories:
  - csharp
tags:
  - csharp
  - sql-server
  - dataset
---
Bildiğiniz gibi numaralandırıcılar (enum sabitleride diyebiliriz) yardımıyla sayısal değerleri kod içerisinde daha anlamlı isimlendirmelerle ifade edebiliriz. Uygulama geliştirirken çoğunlukla framework'ün parçası olan pek çok enum sabitini kullanmaktayız. Örneğin veritabanı uygulamalarında sıkça kullandığımız CommandBehavior, DataRowState, DataRowVersion sabitleri gibi. Bu sistemin temel amacı, bu tiplerin sahip oldukları değerlerin sayısal karşılıklarına ihtiyacımızın olmasıdır.

Öyle ki, uygulama içerisinde yer alan her hangi bir fonksiyonun davranışı için sayısal bir karşılaştırma yapmamız gereken yerlerde, bu sayısal değerin karşılığı olan bir ismi kullanmak çok daha mantıklıdır. Bu geliştirme açısından zaman kazandırıcı bir teknikten de ötedir. Çoğu zaman projelerimizde kendi numaralandırıcı tiplerimizi tanımlama ihtiyacı duyarız. İşte bu günkü makalemizde bizi numaralandırıcı kullanmaya itecek bir nedeni incelemeye çalışacağız.

![dikkat.gif](/assets/images/2005/dikkat.gif)
Numaralandırıcılar sayısal değerleri anlamlı isimler ile ifade etmemize yardımcı olurken, gerek kodlama zamanında gerekse geliştirme aşamasında programcıya büyük kolaylık ve esneklik sağlarlar.

İlk olarak numaralandırıcıları kullanmamız için gerekli senaryomuzdan kısaca bahsedelim. Örneğimizi bir web uygulaması olarak geliştireceğiz. Bu web uygulmasında AdventureWorks2000 (Sql Server 2000 Reporting Service ile birlikte gelen) isimli veritabanında yer alan Products ve ProductSubCategory tablolarından faydalanacağız. Temel olarak bir dataGrid kontrolü üzerinde veri gösterme ve güncelleme işlemlerini ele alacağız. Uygulamamızı en başından itibaren tasarlayacağız. İlerleyen kısımlarında ise numaralandırıcı ihtiyacımızı keşfedecek ve uygun çözüm yollarını geliştirmeye çalışacağız. İlk olarak uygulamamızın tablolar ile ilgili temel işlemlerini yapacak yonetici sınıfını yazalım. DBYonetici sınıfımız şu an için sadece veri çekme işlemlerini üstlenecek. İlerleyen kısımlarda diğer bazı işlevsellikleri de ekleyeceğiz.

DBYonetici.cs

```csharp
using System;
using System.Data;
using System.Data.SqlClient;

namespace UsingEnumerators
{
    public class DBYoneticisi
    {
        private SqlConnection con;
        private SqlDataAdapter da;

        public DBYoneticisi()
        {
            con=new SqlConnection("data source=localhost;database=AdventureWorks2000;integrated security=SSPI");
        }

        public DataSet SelectCategories()
        {
            DataSet resultSet=new DataSet();
            string sql=@"SELECT ProductSubCategoryID, Name FROM ProductSubCategory Order By ProductSubCategoryID";
            da=new SqlDataAdapter(sql,con);
            da.Fill(resultSet);
            return resultSet; 
        } 

        public DataSet SelectProducts()
        {
            DataSet resultSet=new DataSet();
            string sql=@"SELECT TOP 100 ProductID, Name, ProductNumber, StandardCost, ListPrice, StandardCost / ListPrice * 100 AS IncRate, Class, DealerPrice, SellStartDate, Size, Weight,ProductSubCategoryID, (SELECT Name FROM ProductSubCategory WHERE ProductSubCategoryID = P.ProductSubCategoryID) AS ProductSubCategory FROM Product P WHERE (StandardCost IS NOT NULL) AND (ListPrice IS NOT NULL) ORDER BY ProductID";
            da=new SqlDataAdapter(sql,con);
            da.Fill(resultSet);
            return resultSet; 
        }
    }
}
```

DBYonetici isimli sınıfımızdaki metodlardan kısaca bahsetmekte yarar var. Uygulamamızda AdventureWorks2000 veritabanında yer alan Products isimli tabloyu kullanıyoruz. Bu tablodaki bir kaç alanı ele alacağız. Products tablosuna ilişkin sorgumuza dikkat ederseniz, standart maliyet ile liste fiyatı arasındaki oranı ifade eden ek bir alan olduğunu farkedebilirsiniz. Biz bu alandaki değere bakarak dataGrid'imiz üzerinde renklendirme işlmelerini yapmaya çalışacağız.

SelectCategories isimli metodumuz ise ürünlerimizin ait olabileceği kategorileri elde etmemizi sağlıyor. Aslında bu metodu, dataGrid üzerinde her hangibir satır için güncelleme işlemi yapacağımız zaman, bir ListBox kontrolünü dinamik olarak doldurmak amacıyla kullanacağız. Şimdi DBYonetici sınıfımızı web uygulamamızda kullanalım. İlk olarak WebForm'umuz üzerinde yer alacak dataGrid kontrolümüzün içeriğini oluşturacağız.

```text
<asp:datagrid id="dgProducts" runat="server" Font-Size="Smaller" Font-Names="Verdana" AutoGenerateColumns="False" DataKeyField="ProductID" AllowPaging="True">
    <AlternatingItemStyle BackColor="LightSteelBlue"></AlternatingItemStyle>
    <ItemStyle BackColor="White"></ItemStyle>
    <HeaderStyle Font-Bold="True" ForeColor="Brown" BackColor="Gold"></HeaderStyle>
    <Columns>
        <asp:BoundColumn Visible="False" DataField="ProductID"></asp:BoundColumn>
        <asp:TemplateColumn HeaderText="Urun Adı">
            <ItemTemplate>
                <%#DataBinder.Eval(Container.DataItem,"Name")%>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txtName" Runat="server" Width="100" Text='<%#DataBinder.Eval(Container.DataItem,"Name")%>'></asp:TextBox>
            </EditItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Urun Numarası">
            <ItemTemplate>
                <%#DataBinder.Eval(Container.DataItem,"ProductNumber")%>
            </ItemTemplate>
            <EditItemTemplate>
                <%#DataBinder.Eval(Container.DataItem,"ProductNumber")%>
            </EditItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Maliyet">
            <ItemTemplate>
                <%#DataBinder.Eval(Container.DataItem,"StandardCost","{0:C}")%>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txtStandardCost" Runat="server" Width="100" Text='<%#DataBinder.Eval(Container.DataItem,"StandardCost")%>'></asp:TextBox>
            </EditItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Liste Fiyati">
            <ItemTemplate>
                <%#DataBinder.Eval(Container.DataItem,"ListPrice","{0:C}")%>
            </ItemTemplate>
            <EditItemTemplate> 
                <asp:TextBox ID="txtListPrice" Runat="server" Width="100" Text='<%#DataBinder.Eval(Container.DataItem,"ListPrice")%>'></asp:TextBox> 
            </EditItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Artis Orani">
            <ItemTemplate>
                <asp:Label ID="lblIncRate" Runat="Server" Text='<%#DataBinder.Eval(Container.DataItem,"IncRate","{0:F}")%>'></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <%#DataBinder.Eval(Container.DataItem,"IncRate","{0:F}")%>
            </EditItemTemplate>
        </asp:TemplateColumn> 
        <asp:TemplateColumn HeaderText="Alt Kategori ID" Visible="False">
            <ItemTemplate>
                <%#DataBinder.Eval(Container.DataItem,"ProductSubCategoryID")%>
            </ItemTemplate>
            <EditItemTemplate>
                 <asp:Label ID="lblProductSubCategoryID" Text='<%#DataBinder.Eval(Container.DataItem,"ProductSubCategoryID")%>' runat="server"></asp:Label>
            </EditItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Alt Kategori">
            <ItemTemplate>
                <%#DataBinder.Eval(Container.DataItem,"ProductSubCategory")%>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:DropDownList ID="lstProductSubCategory" runat="server" DataSource = "<%# AltKategorileriAl() %>" DataTextField="Name" DataValueField="ProductSubCategoryID" />
            </EditItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Boyutu">
            <ItemTemplate>
                <%#DataBinder.Eval(Container.DataItem,"Size","{0:F}")%>
            </ItemTemplate>
            <EditItemTemplate>
                <%#DataBinder.Eval(Container.DataItem,"Size","{0:F}")%>
            </EditItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Agirlik">
            <ItemTemplate>
                <%#DataBinder.Eval(Container.DataItem,"Weight","{0:F}")%>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txtWeight" Runat="server" Width="100" Text='<%#DataBinder.Eval(Container.DataItem,"Weight")%>'></asp:TextBox>
            </EditItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn>
            <ItemTemplate>
                <asp:Button id="btnDuzenle" CommandName="Duzenle" runat="server" Text="Düzenle"></asp:Button>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:Button id="btnGuncelle" CommandName="Guncelle" runat="server" Text="Güncelle"></asp:Button>
                <asp:Button id="btnVazgec" CommandName="Vazgec" runat="server" Text="Vazgeç"></asp:Button>
            </EditItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn>
            <ItemTemplate>
                <asp:Button id="btnDelete" CommandName="Sil" runat="server" Text="Sil"></asp:Button>
            </ItemTemplate>
            </asp:TemplateColumn>
    </Columns>
    <PagerStyle Mode="NumericPages"></PagerStyle>
</asp:datagrid>
```

Şu anda aklınızdan geçenleri duyar gibiyim. Bu kadar uzun kodlardan sonra numarlandırıcıları nerede kullanacağımızı merak ediyorsunuz. Ama biraz daha sabredelim. İlk olarak yukarıdaki dataGrid kontrolümüz hakkında kısaca bilgi vermekte fayda var. DataGrid kontrolümüz ekranda Products tablosuna ait verileri gösterdiği gibi, satırlar üzerinde veri güncelleme ve silme işlemlerine de izin verecek şekilde oluşturuldu. Bu sebepten üzerinde güncelleme yapılmasını istediğimiz alanlara ait EditItemTemplate kısımlarında TextBox veya DropDownList gibi kontrollere yer verdik. Konumuz dataGrid kontrolünün özelliklerini incelemek olmadığı için çok fazla detaya girmiyorum. Şimdi sayfamızın kodlarını yazalım.

```csharp
private static DBYoneticisi yonetici;

private void Page_Load(object sender, System.EventArgs e)
{
    yonetici=new DBYoneticisi();
}

private void btnYukle_Click(object sender, System.EventArgs e)
{ 
    dgProducts.DataSource=yonetici.SelectProducts();
    dgProducts.DataBind();
}

private void dgProducts_ItemDataBound(object sender, DataGridItemEventArgs e)
{
    if((e.Item.ItemType==ListItemType.Item) || (e.Item.ItemType==ListItemType.AlternatingItem))
    {
        double incRate;
        Label lbl=(Label)e.Item.Cells[5].Controls[1];
        incRate=Convert.ToDouble(lbl.Text);
        if(incRate>=100)
        {
            e.Item.Cells[5].BackColor=Color.Red;
            e.Item.Cells[5].ForeColor=Color.White;
        }
    }
}

private void dgProducts_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
{
    dgProducts.CurrentPageIndex=e.NewPageIndex;
    dgProducts.DataSource=yonetici.SelectProducts();
    dgProducts.DataBind();
}

public DataSet AltKategorileriAl()
{
    return yonetici.SelectCategories(); 
} 

private void dgProducts_ItemCommand(object source, DataGridCommandEventArgs e)
{
    switch (e.CommandName)
    {
        case "Duzenle":
            dgProducts.EditItemIndex=e.Item.ItemIndex;
            dgProducts.DataSource=yonetici.SelectProducts();
            dgProducts.DataBind();
            break;
    
        case "Sil":
            break;
    
        case "Vazgec":
            dgProducts.EditItemIndex=-1;
            dgProducts.DataSource=yonetici.SelectProducts();
            dgProducts.DataBind();
            break;

        default :
            break; 
    }
}
```

Dilerseniz kodlarımızda neler yaptığımıza kısaca değinelim. Sayfamız yüklenirken dataGrid kontrolü, Products tablosu için çalıştırdığımız sorgudan dönen sonuç kümesi ile dolduruluyor. Bu sırada dataGrid kontrolünün ItemDataBound olayında her bir satır için IncRate isimli alanın değerini kontrol ediyoruz. Bunun sonucuna göre eğer artış oranı 100' den büyük ise o hücreinin arka plan rengini ve font rengini değiştirerek kullanıcıyı uyarıyoruz.

Kullanıcı dataGrid kontrolünde sayfalama işlemi yapabilir. Sayfalama işlemini sağlamak için her zamanki gibi PageIndexChanged isimli olay metodumuzu kullanıyoruz. Her hangibir satır için güncelleme işlemi ve silme işlemi de yapabilir. Güncelleme ve Silme işlmeleri için yine DataGrid kontrolümüzün ItemCommand metodunu kullanıyoruz. Eğer düzenleme modu seçilirse bu durumda ürünlerin dahil olduğu kategorileri listelemek için DropDownList kontrolümüzü dolduran SelectProducts metodumuzu çağırıyoruz. Böylece sistemde kayıtlı olan ürünlerin kullanıcı tarafından bir combo kontrolü içerisinden seçilebilmesini sağlamış oluyoruz. Bu hızlı açıklamalardan sonra geldiğimiz noktayı aşağıdaki şekilden görebilirisiniz.

![mk133_1.gif](/assets/images/2005/mk133_1.gif)

Şimdi bu uygulamada odaklanmamız gereken bir nokta var. O da, IncRate'in rengini belirlediğimiz ItemDataBound metodu. Gelin bu metodu mercek altına alalım.

```csharp
private void dgProducts_ItemDataBound(object sender, DataGridItemEventArgs e)
{
    if((e.Item.ItemType==ListItemType.Item) || (e.Item.ItemType==ListItemType.AlternatingItem))
    {
        double incRate;
        Label lbl=(Label)e.Item.Cells[5].Controls[1];
        incRate=Convert.ToDouble(lbl.Text);
        if(incRate>=100)
        {
            e.Item.Cells[5].BackColor=Color.Red;
            e.Item.Cells[5].ForeColor=Color.White;
        }
    }
}
```

Bu metodda dikkat ederseniz if döngümüz içerisinde 5 numaralı hücrenin içerisinde ki 1 numaralı kontrolü ele alıyoruz. Peki bu 5 numaralı hücredeki 1 numaralı kontrol'de neyin nesi oluyor. Aslında dataGrid nesnemizin aspx sayfasındaki kodlarını göz önüne aldığınızda 5 numaralı hücrenin, IncRate alanına ait hücre olduğunu ve 1 numaralı kontrolünde bu hücredeki Label kontrolü olduğunu kolayca görebilirisiniz.

```text
<asp:TemplateColumn HeaderText="Artis Orani">
            <ItemTemplate>
                <asp:Label ID="lblIncRate" Runat="Server" Text='<%#DataBinder.Eval(Container.DataItem,"IncRate","{0:F}")%>'></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <%#DataBinder.Eval(Container.DataItem,"IncRate","{0:F}")%>
            </EditItemTemplate>
        </asp:TemplateColumn>
```

Herşey buraya kadar gayet güzel. Şimdi default.aspx sayfamızda IncRate alanını dataGrid içerisinde kullanıdığımız ItemTemplate takısının hemen önüne yeni bir tane daha ekleyelim. Bu sefer query'den çektiğimiz Class alanını buraya ekliyoruz. Aynen aşağıdaki kod parçasında olduğu gibi.

```text
<asp:TemplateColumn HeaderText="Sınıf">
    <ItemTemplate>
        <%#DataBinder.Eval(Container.DataItem,"Class","{0:F}")%>
    </ItemTemplate>
    <EditItemTemplate>
        <%#DataBinder.Eval(Container.DataItem,"Class","{0:F}")%>
    </EditItemTemplate>
</asp:TemplateColumn>
```

Şimdi uygulamamızı tekrardan çalıştıralım. Bu sefer sizinde benim de beklediğimiz gibi bir şeylerin yolunda gitmemesi gerekiyor. Aşağıdaki ekran görüntüsü korktuğumuzun başımıza geldiği andır.

![mk133_2.gif](/assets/images/2005/mk133_2.gif)

Hata gayet açık ve net. Artık 5 numaralı hücremiz IncRate alanına ait hücre değil. Burada şimdi Class isimli alanımız yer almakta. Ayrıca 5 numaralı alandaki 1 numaralı kontrolümüzde bir Label kontrolü değil. Hatanın nedenide doğru hücre üzerinde olmayışımız. Daha da kötüsü olabilir elbette. Burada çalışma zamanında aldığımız hata ile sorununun ne olduğunu anlayabildik. Oysaki, yeni eklediğimiz hücre pekala sayısal bir değer içeren bir Label kontrolü taşıyabilirdi. Bu durumda uygulama çalışacaktı ama hatalı sonuçlar verecekti.

Peki ya çözüm? Çözüm sonunda bu makalenin başından beridir bahsetmeye çalıştığımız numaralandırıcılardan geçiyor. Sorgudan dönen alanların dataGrid'de denk geldiği indeks numaralarını temsil edecek bir numarlandırıcımız olsaydı daha iyi olmaz mıydı? En azından yeni bir hücre eklediğimizde tek yapmamız gereken enum sabitimizde araya bir eleman daha eklemek olacaktı. Çünkü kodun kaç yerinde aynı numaralı indeksi kullandığımızı bilemeyebilirdik. Her ne kadar arama-bulma yöntemi ile 5' lerin geçtiği yerleri 6 yapabilecek olsakta bu hiçde profesyonelce bir çözüm olmazdı. İşte basit bir enum sabitini kullanarak çok daha profesyonel bir çözüm üretebilirdik.

Gördüğünüz gibi enum sabitlerini kullanmak için son derece güzel bir sebebimiz oldu. Şimdi bu düşüncemizi uygulamamız ile bütünleştirmemiz gerekiyor. İlk olarak yapmamız gereken enum sabitimizi oluşturmak. Eğer bir solution içerisinde pek çok proje ile birlikte çalışıyorsanız bu durumda grid başlıkları gibi indekslere ait enum sabitlerini saklayabileceğiniz genel bir katman dahi oluşturmayı düşünebilirsiniz. Bizim uygulamamız son derece küçük olduğundan geçerli isim alanımız altında bir enum sabiti yapmamız yeterli olacaktır.

enum sabitimiz ProductsGridHeaders

```csharp
namespace UsingEnumerators
{
    enum ProductsGridHeaders
    {
        PRODUCTID,
        NAME,
        PRODUCTNUMBER,
        STANDARDCOST,
        LISTPRICE,
        CLASS,
        INCRATE,
        PRODUCTSUBCATEGORYID,
        PRODUCTSUBCATEGORY,
        SIZE,
        WEIGHT
    }
}
```

İşte altın yumruğu vurduğumuz an. Artık tek yapmamız gereken, enum sabitimizi DataGrid nesnemizin ItemDataBound olay metodunda aşağıdaki gibi kullanmak.

![dikkat.gif](/assets/images/2005/dikkat.gif)
Numaralandırıc içindeki elemanları isimlendirirken gerçekten anlamlı, bir şeyleri kulağımıza çağrıştıran isimler vermeye özen göstermeliyiz.

![mk133_3.gif](/assets/images/2005/mk133_3.gif)

```csharp
private void dgProducts_ItemDataBound(object sender, DataGridItemEventArgs e)
{
    if((e.Item.ItemType==ListItemType.Item) || (e.Item.ItemType==ListItemType.AlternatingItem))
    {
        double incRate;
        Label lbl=(Label)e.Item.Cells[(int)ProductsGridHeaders.INCRATE].Controls[1];
        incRate=Convert.ToDouble(lbl.Text);
        if(incRate>=100)
        {
            e.Item.Cells[(int)ProductsGridHeaders.INCRATE].BackColor=Color.Red;
            e.Item.Cells[(int)ProductsGridHeaders.INCRATE].ForeColor=Color.White;
        }
    }
}
```

Artık uygulamamız sorunsuz şekilde çalışacaktır ve istediğimiz yeni satırı dataGrid kontrolümüze ekleyebiliriz. Unutmamamız gereken tek şek, yeni bir satır eklendiğinde bunu ilgili enum sabitinede yansıtmaktır. Üstelik kodun okunurluğuda inanılmaz derecede kolaylaşmıştır. Ancak elbette ki kodumuzda enum sabitlerini kullanacağımız tek yer burası değil.

Örneğin DropDownList kontrolümüz doldurulduğunda o an aktif olan satıra ait ürün kategori adınında seçili olarak gelmesi gerekmektedir. Şu anki kodlarımıza göre DropDownList kontrolümüz herhangibir satır edit modunda açıldığında ürünleri başarılı bir şekilde listeliyor ama her zaman için ilk elemana konumlanıyor. Bu problemi aşmak için düzenleme moduna geçildiğinde DropDownList kontrolününün SelectedValue özelliğini uygun değere (ki bu değer ProductSubCategoryId olacaktır) atamamız yeterli olacaktır. Dolayısıla dataGrid kontrolümüzün ItemDataBound olay metodunda aşağıdaki düzenlemeyi yapmamız bu ihtiyacımızı karşılayacaktır. Artık kodu yazarken, hangi indisli hücreyi düşünmemize gerekte yoktur. Çünkü numaralandırıcımız anlamlı isimleri ile bunu bize söylemektedir.

```csharp
if(e.Item.ItemType==ListItemType.EditItem)
{
    Label lblKatId=(Label)e.Item.Cells[(int)ProductsGridHeaders.PRODUCTSUBCATEGORYID].Controls[1];
    DropDownList lstKategoriler=(DropDownList)e.Item.Cells[(int)ProductsGridHeaders.PRODUCTSUBCATEGORY].Controls[1];    
    lstKategoriler.SelectedIndex=Convert.ToInt16(lblKatId.Text)-1;
}
```

Oluşturduğumuz enum sabiti özellikle güncellenecek satır için yazacağımız kod satırlarında da çok işe yarayacaktır. Son olarak güncelleme işlemimizi numaralandırıcımızı kullanarak nasıl gerçekleştirdiğimizi görelim. Uygulamamıza bu fonksiyonelliği kazandırmak için yine ItemDataBound olay metodunu kullanacağız. Bu sefer CommandName özelliğinin Guncelle değerini alması halinde gerçekleştireceğimiz işlemler var.

Biz numaralandırıcımızı Kaydetme işleminin gerçekleştirileceği metodu çağırmadan önce, gridde seçili olan satırdaki kontroller üzerindeki değerleri okuduğumuz satırlarda kullanıyoruz. Örneğin dataGrid kontrolünde Name alanının gösterildiği hücrede yer alan TextBox kontrolüne girilen değeri almak için, ProductsGridHeaders numaralandırıcısının Name elemanının işaret ettiği integer değere sahip olan hücredeki 1 numaralı kontrolü ele alıyoruz. Daha sonra bu kontrolü bir TextBox nesnesine dönüştürerek güncel değerini ilgili kaydetme işlemini gerçekleştirecek metodumuza taşıyoruz. Bu değer alma işlemini diğer kontrollerimiz içinde yapıyoruz. Ancak önemli olan e parametresi üzerinden dataGrid kontrolündeki ilgili alanlara nasıl eriştiğimiz. Yani numaralandırıcımızı nasıl kullandığımız.

```csharp
private void Kaydet(DataGridCommandEventArgs e)
{ 
    int productID=Convert.ToInt32(dgProducts.DataKeys[e.Item.ItemIndex]);
    TextBox txtName=(TextBox)e.Item.Cells[(int)ProductsGridHeaders.NAME].Controls[1];
    TextBox txtStandardCost=(TextBox)e.Item.Cells[(int)ProductsGridHeaders.STANDARDCOST].Controls[1];
    TextBox txtListPrice=(TextBox)e.Item.Cells[(int)ProductsGridHeaders.LISTPRICE].Controls[1];
    TextBox txtWeight=(TextBox)e.Item.Cells[(int)ProductsGridHeaders.WEIGHT].Controls[1];
    DropDownList lstSubCategories=(DropDownList)e.Item.Cells[(int)ProductsGridHeaders.PRODUCTSUBCATEGORY].Controls[1];

    string Name=txtName.Text;
    double StandardCost=Convert.ToDouble(txtStandardCost.Text);
    double ListPrice=Convert.ToDouble(txtListPrice.Text);
    double Weight=Convert.ToDouble(txtWeight.Text);
    int SCategoryID=Convert.ToInt16(lstSubCategories.SelectedValue);

    yonetici.UpdateProducts(productID,Name,StandardCost,ListPrice,Weight,SCategoryID);
}

private void dgProducts_ItemCommand(object source, DataGridCommandEventArgs e)
{
    switch (e.CommandName)
    {
        case "Duzenle":
            dgProducts.EditItemIndex=e.Item.ItemIndex;
            dgProducts.DataSource=yonetici.SelectProducts();
            dgProducts.DataBind();
            break;

        case "Guncelle":
            Kaydet(e);
            dgProducts.EditItemIndex=-1;
            dgProducts.DataSource=yonetici.SelectProducts();
            dgProducts.DataBind();
            break;

        case "Sil":
            break;

        case "Vazgec":
            dgProducts.EditItemIndex=-1;
            dgProducts.DataSource=yonetici.SelectProducts();
            dgProducts.DataBind();
            break;

        default :
            break; 
    }
}
```

DbYoneticisi sınıfımızda güncelleme işlemini gerçekleştiren metodumuz ise aşağıdaki gibidir.

```csharp
public int UpdateProducts(int productID, string name, double standardCost, double listPrice, double weight, int subCategoryID)
{
    string sql=@"UPDATE Product SET Name=@Name,StandardCost=@StandardCost, ListPrice=@ListPrice,Weight=@Weight, ProductSubCategoryID=@SubCategoryID WHERE ProductID=@ProductID";
    SqlCommand cmd=new SqlCommand(sql,con);
    cmd.Parameters.Add("@Name",name);
    cmd.Parameters.Add("@StandardCost",standardCost);
    cmd.Parameters.Add("@ListPrice",listPrice);
    cmd.Parameters.Add("@Weight",weight);
    cmd.Parameters.Add("@SubCategoryID",subCategoryID);
    cmd.Parameters.Add("@ProductID",productID);
    con.Open();
    int result=cmd.ExecuteNonQuery();
    con.Close();
    return result;
}
```

Gördüğünüz gibi enum sabitlerini kullanarak kod geliştirmek daha da kolaylaşmaktadır. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Kodlar İçin Tıklayınız.](https://www.buraksenyurt.com/makale/images/UsingEnumerators.rar)