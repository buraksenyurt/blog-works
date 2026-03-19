---
title: "Asp.Net Routing – Hatırlamak"
pubDate: 2013-06-20 05:07:00
categories:
  - Asp.Net
  - Asp.Net 4.0
  - Asp.Net 4.5
tags:
  - Asp.Net
  - Asp.Net 4.0
  - Asp.Net 4.5
  - C#
  - XML
  - .NET
  - ASP.NET MVC
  - Entity Framework
  - LINQ
  - REST
  - HTTP
  - Reflection
---

# Asp.Net Routing – Hatırlamak
[![oblivious](images/oblivious_thumb.jpg)](images/oblivious.jpg) Merhaba Arkadaşlar,

Geçtiğimiz günlerde şirkette çok küçük bir web uygulamasına ihtiyaç duyuldu. Neredeyse bir günlük geliştirme maliyeti olan, küçük bir departmanın önemli bir gereksinimi karşılayacaktı. Tabi insan uzun zaman kodlama yapmayınca veya kodlamaya ara verince bazı temel bilgileri de unutabiliyor.

Ben de kafayı Team Foundation Server entegrasyonu, SOA mimarisi ve Scrum gibi metodolojiler ile bozunca, zihinsel diskimdeki ana partition’ a yeni bilgilerin yazıldığına ve eskilerinin yerinde yeller estiğine şahit oldum. Ama malum, günümüz teknolojilerinde bilginin tamamını ezberlemeye çalışmak yerine, en doğrusuna en hızlı şekilde nasıl ulaşabileceğimizi bilmek daha önemli. İşte bu felsefeden yola çıkıp dedim ki, şu Asp.Net Routing konusunu bir hatırlayayım ve hatta kayıt altına alayım. İşte hikayemiz böyle başladı

![Smile](images/wlEmoticon-smile_96.png)

Asp.Net MVC’ nin en cazip yanlarından birisi sanırım sağladığı URL eşleştirme (Routing) sistemidir. Özellikle Search Engine Optimization (SEO) kriterleri göz önüne alındığında, orders.aspx?categoryName=Beverages&shipCity=istanbul&orderNumber=12903 gibi bir ifade yerine, orders/beverages/istanbul/12903 şeklinde bir URL çok daha değerlidir.

Bilindiği üzere Asp.Net 4.0 sürümü ile birlikte, URL ve asıl kaynak (aslında yönlendirme sonucu gidilmesi gereken bir aspx sayfa kodu düşünebiliriz) eşleştirmelerinde kullanılan yönlendirme işlemleri oldukça kolaylaştırılmıştır. İşte bu yazımızda, biraz temelleri hatırlamaya çalışacak ve SEO’cu arama motorlarının olmassa olmaz isterlerinden birisi olan Routing konusunu dört basit örnek üzerinden inceleyeceğiz. İlk olarak senaryomuza bir göz atalım.

Senaryo

Senaryomuzda veri kaynağı olarak emektar Northwind veritabanını kullanacağız. Örneklerimizde hem Entity Framework kullanacağımız hem de doğrudan SQL sorgusu çalıştıracağımız bir vakamız yer alacak. Temel olarak amacımız aşağıdaki ekran görüntüsünde yer alan URL eşleştirmelerini web uygulaması üzerinden işlettirmek.

[![route_2](images/route_2_thumb.png)](images/route_2.png)

Dikkat edileceği üzere URL satırından girilecek olan anlamlı ifadeler, aslında arka planda bir eşleştirme tablosuna uygun olacak şekilde ilgili kaynaklara yönlendirilmekteler. Örneğin beverages isimli kategoride yer alan ürünlerin listelenmesi için yazılan urunler/beverages sorgusu, sisteme daha önceden öğretilen Urunler/{CategoryName} üzerinden geçerek urun.aspx sayfasına yönlendiriliyor. Çok doğal olarak ilgili sayfa içerisinde, CategoryName değerine bakılarak bir sonuç kümesinin sunulması gerekiyor.

Route Eşleştirmelerinin Ayarlanması

Bu işlem için global.asax.cs dosyasında aşağıdaki kodlamaları yapmamız gerekmektedir.

```csharp
using System; 
using System.Web; 
using System.Web.Routing;

namespace HowTo_EasyRouting 
{ 
    public class Global 
        : HttpApplication 
    { 
        private void SetRouteMaps() 
        { 
            RouteTable.Routes.MapPageRoute("Varsayilan", "", "~/kategori.aspx"); 
            RouteTable.Routes.MapPageRoute("Kategoriler", "kategoriler", "~/kategori.aspx"); 
            RouteTable.Routes.MapPageRoute("KategoriBazliUrunler","urunler/{CategoryName}","~/urun.aspx"); 
            RouteTable.Routes.MapPageRoute("SehirBazliSiraliMusteriler", "musteriler/{City}$orderby={FieldName}", "~/musteri.aspx"); 
            RouteTable.Routes.MapPageRoute("SehirBazliSiparisler", "siparisler/{ShipCity}", "~/siparis.aspx"); 
        }

        protected void Application_Start(object sender, EventArgs e) 
        { 
            SetRouteMaps(); 
        } 
    } 
}
```

ApplicationStart olay metodu bilindiği üzere, Web uygulaması ayağa kalktığında devreye girmektedir. Dolayısıyla uygulamanın başladığı bir yerde, URL eşleştirme tanımlamalarını yapmak son derece mantıklıdır. Olayın ana kahramanı RouteTable sınıfıdır. Söz konusu tipin static olarak erişilebilen Routes özelliği bir RouteCollection referansını işaret etmektedir. Bu koleksiyon tahmin edileceği üzere URL ile asıl kaynak eşleştirmelerini taşımaktadır. Bu nedenle MapPageRoute metodundan da yararlanılarak gerekli eşleştirme bilgileri koleksiyona eklenir.

İlk satır ile Root URL adresine gelen bir talebin doğrudan kategori.aspx sayfasına yönlendirilmesi gerektiği ifade edilmektedir. İkinci satırda ise web kök adresini takiben kategoriler şeklinde gelen bir ifadenin gelmesi halinde yine, kategori.aspx sayfasına gidilmesi gerektiği belirtilmektedir.

KategoriBazliUrunler ismi ile tanımlanmış eşleştirmeye göre, urunler/{CategoryName} şeklinde gelen talepler urun.aspx sayfasına yönlendirilmektedir. İlginç kullanımlardan birisi de SehirBazliSiraliMusteriler isimli eşleştirmedir. Burada City ve FieldName isimli iki Route parametresi söz konusudur. İfade ise size sanıyorum tanıdık gelecektir. Neredeyse bir REST servis sorgusuna (örneğin OData sorgusuna) oldukça yakın değil mi?

![Nerd smile](images/wlEmoticon-nerdsmile_1.png)

Şimdi bu durumları kod tarafında nasıl karşılayacağımızı örnek bir Asp.Net uygulaması üzerinden incelemeye çalışalım.

Birinci Durum

İlk olarak kategori.aspx sayfasına doğru yapılacak yönlendirmeleri ele almaya çalışacağız. Bunun için web uygulamamıza kategori.aspx isimli bir sayfa ekleyip içeriği ile kod tarafını aşağıdaki gibi geliştirelim.

```xml
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Kategori.aspx.cs" Inherits="HowTo_EasyRouting.Kategori" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml"> 
<head runat="server"> 
    <title></title> 
</head> 
<body> 
    <form id="form1" runat="server"> 
    <div id="divCategories" runat="server" style="background-color:lightcyan">         
    </div> 
    </form> 
</body> 
</html>
```

kod tarafı

```csharp
using System; 
using System.Linq; 
using System.Web.UI; 
using System.Web.UI.WebControls;

namespace HowTo_EasyRouting 
{ 
    public partial class Kategori 
        : Page 
    { 
        protected void Page_Load(object sender, EventArgs e) 
        { 
            if (!Page.IsPostBack) 
            { 
                using (NorthwindEntities context = new NorthwindEntities()) 
                { 
                    var categories = from c in context.Categories 
                                     orderby c.CategoryName 
                                     select new 
                                     { 
                                         c.CategoryID, 
                                         c.CategoryName 
                                     };

                    foreach (var category in categories) 
                    { 
                        HyperLink categoryLink = new HyperLink(); 
                        categoryLink.NavigateUrl = GetRouteUrl("KategoriBazliUrunler", new { CategoryName = category.CategoryName }); 
                        categoryLink.Text = string.Format("[{0}]-{1}<br/>", category.CategoryID.ToString(), category.CategoryName); 
                        divCategories.Controls.Add(categoryLink); 
                    } 
                } 
            } 
        } 
    } 
}
```

Aslında kategori.aspx sayfasında tipik olarak Entity Framework odaklı bir sorgulama gerçekleştirilmekte ve kategori adları birer HyperLink bileşeni olarak div içerisine eklenmektedir. Konu itibariyle işin önemli olan kısmı ise HyperLink bileşeninin NavigateUrl özelliğine GetRouteUrl metodu sonucunun atanmasıdır.

GetRouteUrl metodu dikkat edileceği üzere iki parametre alır. İlk parametre route adıdır. Yazdığımız değere göre urunler/{CategoryName} şeklindeki atama değerlendirilir. İkinci parametre ise bu Route içerisinden kullanılmak istenen değişken adı ve değerini içeren nesnenin örneklendiği kısımdır. CategoryName tahmin edileceği üzere Route tanımı içerisindeki parametre adıdır. Değeri ise zaten LINQ (Language INtegrated Query) sorgusu içerisinden elde edilmektedir. İkinci parametre object tipinden olduğundan bir isimsiz tip (anonymous type) ataması yapılabilmiştir. Bu nedenle Route içerisinde birden fazla parametre olması halinde, isimsiz tipin de birden fazla özellik içermesi gerektiğini ifade edebiliriz.

İlk durumda herhangibir sayfa talep edilmediğinde veya kök web adresi ardından /kategoriler şeklinde bir URL ifadesi kullanıldığında, aşağıdaki ekran görüntüsünde yer alan sonuçlar ile karşılaşırız.

[![route_3](images/route_3_thumb.png)](images/route_3.png)

Dikkat edilmesi gereken en önemli nokta, her hangi bir bağlantı üstüne gelindiğinde oluşan sorgu adresidir. Örneğin Condiments için http://localhost:54605/urunler/condiments şeklinde bir URL tanımı oluşmuştur. Peki bu bağlantıya tıklanırsak ne olur?

![Who me?](images/wlEmoticon-whome_10.png)

İkinci Durum

kategori.aspx sayfasında bir bağlantıya tıklandığında, HyperLink bileşeninin NavigateUrl özelliğinin sahip olduğu değerin Route tablosundaki eşleniğine bakılmalıdır. Yaptığımız tanımlamalara göre urun.aspx sayfasına gidilmesi beklenmelidir (KategoriBazliUrunler isimli Route tanımına dikkat edin) Buna göre urun.aspx sayfasının içeriğini aşağıdaki gibi düzenleyebiliriz.

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Urun.aspx.cs" Inherits="HowTo_EasyRouting.Urun" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml"> 
<head runat="server"> 
    <title></title> 
</head> 
<body> 
    <form id="form1" runat="server"> 
    <div> 
        <h1 style="color:purple"> 
         <asp:Label ID="lblCategoryName" runat="server" /></h1> 
        <br /> 
        <asp:GridView ID="grdUrunler" runat="server" /> 
    </div> 
    </form> 
</body> 
</html>
```

kod tarafı

```csharp
using System; 
using System.Linq; 
using System.Web.UI;

namespace HowTo_EasyRouting 
{ 
    public partial class Urun 
        : Page 
    { 
        protected void Page_Load(object sender, EventArgs e) 
        { 
            if (RouteData.Values["CategoryName"] != null) 
            { 
                string categoryName = RouteData.Values["CategoryName"].ToString(); 
                using (NorthwindEntities context = new NorthwindEntities()) 
                { 
                    var products = from p in context.Products.Include("Product") 
                                   where p.Category.CategoryName == categoryName 
                                   orderby p.ProductName 
                                   select new 
                                   { 
                                       p.ProductID, 
                                       p.ProductName, 
                                       p.UnitPrice 
                                   };

                    grdUrunler.DataSource = products.ToList(); 
                    grdUrunler.DataBind(); 
                } 
            } 
            else 
                Response.Redirect(GetRouteUrl("Kategoriler", null)); 
        } 
    } 
}
```

Tabi bu sayfaya gelindiğinde aslında Route tanımlaması içerisinde yer alan parametre değerinin okunması gerekmektedir. Bu sebepten sayfanın RouteData özelliğinden hareket edilerek RouteValueDictionary tipinden olan Values özelliğine gidilir ve indeksleyiciye verilen CategoryName alanının var olup olmadığına bakılır. Malum urun.aspx sayfasına farklı bir şekilde erişilmek istenebilir ve CategoryName değeri null olarak gelebilir. Bu nedenle bir null değer kontrolü ardından Entity sorgulama işlemi yapılmıştır. RouteData.Values[“CategoryName”] ile URL satırındaki kategori adı bilgisi alındıktan sonra standart olarak bir Entity sorgusu icra edilmektedir. Eğer kategori adı null olarak gelirse bu durumda varsayılan URL eşleştirilmesi nedeniyle kategorilerin gösterildiği sayfaya gidilir.

[![route_4](images/route_4_thumb.png)](images/route_4.png)

Üçüncü Durum

URL eşleştirmelerinden SehirBazliSiraliMusteriler isimli olanı, iki adet route parametresi içermektedir. Burada başta da belirttiğimiz üzere OData sorgularına benzer bir ifade tanımlanmıştır. Eşleştirme bilgisine göre musteri.aspx sayfasına doğru bir yönlendirme söz konusudur. musteri.aspx içeriğini aşağıdaki gibi geliştirdiğimizi düşünelim.

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Musteri.aspx.cs" Inherits="HowTo_EasyRouting.Musteri" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml"> 
<head runat="server"> 
    <title></title> 
</head> 
<body> 
    <form id="form1" runat="server"> 
    <div> 
        <h1 style="color:purple">Customers</h1> 
        <asp:GridView ID="grdCustomer" runat="server" /> 
    </div> 
    </form> 
</body> 
</html>
```

kod tarafı

```csharp
using System; 
using System.Linq; 
using System.Reflection; 
using System.Web.Routing;

namespace HowTo_EasyRouting 
{ 
    public partial class Musteri : System.Web.UI.Page 
    { 
        protected void Page_Load(object sender, EventArgs e) 
        { 
           if(RouteData.Values["City"]!=null 
                || RouteData.Values["FieldName"]!=null) 
            { 
                string cityName=RouteData.Values["City"].ToString(); 
                string fieldName=RouteData.Values["FieldName"].ToString(); 
                
                using(NorthwindEntities context=new NorthwindEntities()) 
                { 
                    var customers = context 
                        .Customers 
                        .Where(c => c.City == cityName) 
                        .OrderBy(GetField<Customer>(fieldName)) 
                        .Select(c => new 
                        { 
                            ID=c.CustomerID, 
                            Title=c.ContactTitle,                          
                            Contact=c.ContactName, 
                            Company=c.CompanyName, 
                            c.City 
                        });

                    grdCustomer.DataSource = customers.ToList(); 
                    grdCustomer.DataBind(); 
                } 
            } 
        }

        public static Func<T, string> GetField<T>(string fieldName) 
        { 
            PropertyInfo pInfo=typeof(T).GetProperty(fieldName); 
            if (pInfo == null) 
                pInfo = typeof(T).GetProperty("CustomerID");

            return o => Convert.ToString(pInfo.GetValue(o, null));         
        } 
    } 
}
```

RouteData.Values özelliğinden yararlanılarak CityName ve FieldName değerlerinin null olup olmamasına göre bir kod parçası çalıştırılmaktadır. Bir önceki örnekten farklı bir durum olmasa da Entity sorgusunda OrderBy extension metodunu nasıl kullandığımıza dikkat etmenizi rica ederim. İşte bu vakaya ait örnek ekran çıktıları.

Londra’ daki müşterilerin CustomerId bilgilerini göre sıralı olarak çekilmesi

[![route_5](images/route_5_thumb.png)](images/route_5.png)

Londra’ daki müşterilerin ContactName bilgisine göre sıralı olarak çekilmesi

[![route_6](images/route_6_thumb.png)](images/route_6.png)

Dördüncü Durum

Son vakada bir Route parametrenin her hangi bir veri bağlı kontrol ile nasıl ilişkilendirilebileceğini görmeye çalışacağız. Örneğin bir SqlDataSource bileşenindeki Select sorgusuna ait Where koşullarını Route parametreler ile ilişkilendirebiliriz. Bu durumu siparis.aspx sayfası içerisinde ele almaya çalışalım. Siparis sayfasına gelinebilmesi için Route tablo tanımlamalarına göre /siparisler/{ShipCity} şeklinde bir URL talebinin gönderilmesi gerekmektedir.

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Siparis.aspx.cs" Inherits="HowTo_EasyRouting.Siparis" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml"> 
<head runat="server"> 
    <title></title> 
</head> 
<body> 
    <form id="form1" runat="server"> 
    <div> 
        <h1 style="color:purple">Siparişler</h1> 
        <asp:GridView ID="grdOrders" runat="server" AllowPaging="True" AutoGenerateColumns="False" DataKeyNames="OrderID" DataSourceID="SqlDataSource1" > 
            <Columns> 
                <asp:BoundField DataField="OrderID" HeaderText="OrderID" InsertVisible="False" ReadOnly="True" SortExpression="OrderID" /> 
                <asp:BoundField DataField="CustomerID" HeaderText="CustomerID" SortExpression="CustomerID" /> 
                <asp:BoundField DataField="EmployeeID" HeaderText="EmployeeID" SortExpression="EmployeeID" /> 
                <asp:BoundField DataField="ShippedDate" HeaderText="ShippedDate" SortExpression="ShippedDate" /> 
                <asp:BoundField DataField="ShipCity" HeaderText="ShipCity" SortExpression="ShipCity" /> 
            </Columns> 
        </asp:GridView> 
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
            ConnectionString="<%$ ConnectionStrings:NorthwindConnectionString %>" 
            SelectCommand="SELECT [OrderID], [CustomerID], [EmployeeID], [ShippedDate], [ShipCity] FROM [Orders] WHERE ([ShipCity] = @ShipCity)"> 
            <SelectParameters> 
                <asp:RouteParameter DefaultValue="" Name="ShipCity" RouteKey="ShipCity" Type="String" /> 
            </SelectParameters> 
        </asp:SqlDataSource> 
    </div> 
    </form> 
</body> 
</html>
```

Önemli olan, SqlDataSource bileşenine ait SelectCommand ifadesindeki where koşulunda yer alan ShipCity isimli parametrenin bir RouteParameter ile ilişkilendirilmiş olmasıdır. RouteParameter bileşenine ait RouteKey özelliği, Route Table’ daki ile aynı olmalıdır. Çok doğal olarak aspx kaynak tarafında yapılabilen bu eşleştirme, Wizard üzerinden de kolayca belirlenebilir. Aynen aşağıdaki ekran görüntüsünde olduğu gibi

![Winking smile](images/wlEmoticon-winkingsmile_198.png)

[![route_1](images/route_1_thumb.png)](images/route_1.png)

Buna göre örneğin Stuttgart’ a yapılan sevkiyatları aşağıdaki gibi elde edebiliriz.

[![route_7](images/route_7_thumb.png)](images/route_7.png)

Sonuç

Görüldüğü üzere URL eşleştirme işlemleri klasik sunucu tabanlı Asp.Net uygulamalarında da etkili bir şekilde kullanılabilir. Hatta bu felsefeden yola çıkarak OData sorgularının daha fazla gelişmişlerini destekleyecek web uygulamaları yazılması da pekala olasıdır. Yazımıza konu olan basit örneklerimizde ki anahtar noktalar, RouteTable sınıfı, RouteData.Values özelliği ve GetRouteUrl metodudur. Örneği geliştirmek tamamen sizin elinizde. İşe /siparisler/stuttgart/10301/ şeklinde bir sorguyu ele alıp, 10301 numaralı siparişe ait detay bilgileri göstereceğiniz bir sayfayı üretmeyi deneyerek başlayabilirsiniz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[HowTo_EasyRouting.zip (605,23 kb)](assets/HowTo_EasyRouting.zip)
