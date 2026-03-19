---
title: "Data Access Application Block Nedir? (.Net 2.0 için)"
pubDate: 2006-05-05 12:00:00
categories:
  - Ado.net 2.0
tags:
  - Ado.net 2.0
  - C#
  - XML
  - .NET
  - ADO.NET
  - SQL Server
  - HTTP
  - Memory Management
  - Performance
  - Transactions
  - Generics
  - DataSet
  - DataTable
---

# Data Access Application Block Nedir? (.Net 2.0 için)
Değerli Okurlarım Merhabalar,

Microsoft tarafından serbest olarak dağıtılan Data Access Application Block (Veri Erişimi Uygulama Bloğu) özellikle n katmanlı mimarilerde, Data Access Layer (veri erişim katmanı) için gerekli işlevselliği sağlayan, performans ve bellek yönetimi konusunda iyi sonuçlar veren bir Enterprise Solution Pattern'dir. Bu block sayesinde, özellikle Business Layer (iş katmanındaki) katmanındaki işimiz oldukça kolaylaşmaktadır. Özellikle Sql sunucusu üzerinde uzmanlaşmış olan bu block'un.Net 2.0 için olan sürümü Ocak ayı içerisinde yayınlandı.

Data Access Application Block (Veri Erişimi Uygulama Bloğu) ve diğer Enterprise Solution Pattern'lerini bu adresten [indirebilirsiniz](http://www.microsoft.com/downloads/details.aspx?familyid=5A14E870-406B-4F2A-B723-97BA84AE80B5&displaylang=en). Data Access Application Block (Veri Erişimi Uygulama Bloğu), diğer blocklar gibi bir solution olarak gelmektedir. Dolayısıyla ilk kullanımından önce mutlaka bu solution'ı açıp derlememiz gerekir. Böylece kullanılabilir assembly dosyalarımızı (dll'leri) elde etmiş oluruz. Bunun sonucu olarakta herhangibir projede, Data Access Application Block (Veri Erişimi Uygulama Bloğu) için oluşturulan assembly'ımızı referans edebilir ve kullanmaya başlayabiliriz. Aynı uygulama mantığı diğer block'lar içinde geçerlidir.

Data Access Application Block (Veri Erişimi Uygulama Bloğu)' unu kısaca inceleyeceğimiz bu makalede örneklere geçmeden önce, Data Access Layer 'ın (veri erişim katmanı) sağladığı avantajlardan kısaca bahsetmekte fayda olacağı kanısındayım. Katlı mimariler, özellike Enterprise Solution'ların olmazsa olmaz parçalarından birisidir. Temel olarak en basit mimari model üç katmandan oluşmaktadır.

![mk160_3.gif](images/mk160_3.gif)

Bu modelde veri ile ilgili temel işlemleri üstlenen bir veri erişim katmanı (Data Access Layer), uygulamanın mantığını üstlenen bir iş katmanı (Business Layer) ve uygulama arabiriminin tutulduğu bir sunum katmanı (Presentation Layer) mevcuttur. Data Access Layer (veri erişim katmanı) genellikle bağlantı oluşturma, sql komutlarını çalıştırma gibi temel yapılar için gerekli kodları, diğer katmanlardan soyutlayan bir görev üstlenir. Net tarafından baktığımızda, DataSet, DataTable, Xml, DataReader gibi veri türlerini veya kendi veri türlerimizi geri döndüren işlemler ve daha bir çoğu Data Access Layer (veri erişim katmanı) içerisindeki metodlarda toplanmaktadır. Eğer elinizde hazır bulunan bir Data Access Layer (veri erişim katmanı) yoksa veya tembellik edip yazmaya üşeniyorsanız (ya da var olan bir tanesini inceleyip en azından vizyonunuzu geliştirmek istiyorsanız), Data Access Application Block (Veri Erişimi Uygulama Bloğu) gerçekten büyük bir fırsattır. Özelliklede ücretsiz olarak dağıtıldığı düşünülürse.

Normal şartlar altında özellikle Business (İş) katmanında yer alan metodlarımız içerisinde, Data Access Layer (veri erişim katmanı) içerisindeki metodlar sıklıkla kullanılmaktadır. Örneğin web tabanlı bir hizmet programını ele alalım ve sayfalar üzerindeki GridView kontrollerinin doldurulması gibi temel bir işlem için gerekli olan materyalleri düşünelim. İlk olarak Data Access Layer (veri erişim katmanı) üzerinde gerekli sorguları çalıştırıp geriye DataSet döndürecek aşırı yüklenmiş (overload) metodlar yazılması gerekir. Aşırı yüklenmiş bu versiyonlarda text bazlı query çalıştıracak, birden fazla sayıda parametre alacak yada bir stored procedure'ü ele alacak hatta bunların transaction bazlı versiyonlarınıda tutacak tipte seçenekler yer alabilir. Üstelik bu metodun kullanacağı bağlantı nesnesinide bilmesi, gerekirse oluşturması, açması ve hatta işi bitince bellek yönetiminide gerçeleştirerek kaynakları en iyi şekilde idare etmesi beklenir. Sonuç itibariyle uygulamanın iş mantığını kapsayan Business (İş) katmanının, buradaki kod kalabalığından etkilenmemesi amaçlanmaktadır. Öyleki veri erişim katmanı bir kez yazıpı pek çok projede kullanılabilirken, iş katmanı projeden projeye farklılık gösterecektir. İşte bu nedenle bu tip karmaşık ve kendi içerisinde dallanarak modülleşebilen işlemler, pek çok projede kullanılabilmeleri amacıyla Data Access Layer (veri erişim katmanı) içerisinde tutulurlar.

Data Access Application Block (Veri Erişimi Uygulama Bloğu)' un sağladığı etkinlikleri görebilmek amacıyla aşağıdaki kod parçasını göz önüne alabiliriz. Bu kod parçası pek çok yerde kullanılabilecek tipte bir metod olarak veri erişim katmanında yer alacak nitelikte bir yapıya sahiptir.

```csharp
using (SqlConnection con = new SqlConnection(conStr)
{
    using (SqlCommand cmd = new SqlCommand(queryString, con))
    {
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataSet ds = new DataSet();
        da.Fill(ds);
    }
}
```

Kodumuz parametre olarak gelecek sorgunun sonuçlarını bir DataSet içerisine aktarmaktayız. Bu işlemi gerçekleştirmek için ihtiyacımız olan tüm materyal kod satırlarında yer almaktadır. Bağlantıyı oluşturmak için bir SqlConnection nesne örneği, komutu çalıştırmak için bir SqlCommand nesne örneği ve bu komutu alıp DataSet'i dolduran bir SqlDataAdapter nesne örneği. Aslında yapılan iş son derece basittir. Bir sorgu sonucu elde edilen veri kümesinin DataSet nesne örneğine aktarılması.

Çeşitli uygulamların pek çok noktasında belirli bir sorgu ve bağlantı ile çalışıp geriye DataSet döndürecek metodlarımız olacaktır. Bunları her seferinde tekrardan yazmak nesne yönelimli bir dilin imkanları göz önüne alındığında doğal olarak yanlış olacaktır. Dolayısıyla bu ve benzeri işlevselliklere sahip kod parçaları, bir katman dahilinde toplanabilir ve yönetimin daha kolay olması sağlanabilir. Üstelik bu, verileri hazırlamak için kullanılan kod karmaşasınında diğer katmanlardan soyutlanması anlamına gelmektedir. İşte bu nedenlede, uygulamaların mantığı ile görsel arabirimlerinden ayrıştırılmış ve temel olarak veri erişim tekniklerini ele almış bir katman söz konusudur ki biz bunu Data Access Layer (veri erişim katmanı) olarak nitelendiriyoruz. Microsoft buradaki ihtiyacı ele alaraktan herkesin kolayca kullanabileceği bir veri erişim katmanı çözümü üretmiştir. Data Access Application Block (Veri Erişimi Uygulama Bloğu).

![mk160_4.gif](images/mk160_4.gif)

Eğer sisteminize Data Access Application Block (Veri Erişimi Uygulama Bloğu)' u başarılı bir şekilde yüklediyseniz, kendi Data Access Layer (veri erişim katmanı) kütüphanenize kolayca sahip olmuşsunuz demektir. Örneğin yukarıdaki kodun yer aldığı bir Data Access Layer (veri erişim katmanı) metodunun, Data Access Application Block (Veri Erişimi Uygulama Bloğu)' taki karşılığını ele almaya çalışalım. Ancak öncesinde, uygulamamıza Data Access Application Block (Veri Erişimi Uygulama Bloğu)' tan gerekli referansları almamız gerekecektir. (Eğer.Net 2.0 için indirdiğiniz Enterprise Library'yi standart olarak kurduysanız, Enterprise Solution'unuzu derledikten sonra oluşan assembly'lara C:\Program Files\Microsoft Enterprise Library January 2006\bin klasöründen erişebilirsiniz.)

![mk160_1.gif](images/mk160_1.gif)

Bu referansları aldıktan sonra yukarıdaki kod parçasını aşağıdaki haliyle yazabiliriz. Önce bizim için gerekli referanslar;

```csharp
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Microsoft.Practices.EnterpriseLibrary.Data;
```

Kodumuzun aynısını ve hatta daha iyisinide yapan Data Access Application Block (Veri Erişimi Uygulama Bloğu) karşılıkları,

```csharp
Database db = DatabaseFactory.CreateDatabase("conn");
db.ExecuteDataSet(CommandType.Text, "Select * From Production.Product");
```

Görüldüğü gibi Data Access Application Block (Veri Erişimi Uygulama Bloğu)' un kullanımı son derece kolay ve esnektir. Koda dikkat edecek olursanız, connection nesnesinin nasıl oluşturulduğunu, dataSet nesnesinin nasıl doldurulduğunu görmemekteyiz. Bu işlemler Data Access Layer (veri erişim katmanı) içerisinde kapsüllenerek bizden soyutlandırılmış durumdalar. Benim en çok beyendiğim özelliklerden birisi, kilit nesnelerin oluşturulmasında çeşitli factory nesnelerinin görev alıyor olması. Örneğimizde, Database nesnesini oluşturmak için DatabaseFactory isimli başka bir fabrika nesnesi kullanılmıştır. Özellikle CreateDatabase metodunun bu versiyonu varsayılan olarak uygulamanın konfigurasyon dosyasına bakıp uyun connectionString node'unu kullanmıştır.

.Net 2.0 için geliştirilen bu versiyonda önceki Enterprise Block'lara göre bir takım temel farklılıklarda mevcuttur. Örneğin, burada oluşturduğumuz Database nesne örneğini çeşitli metodları yürütmek (execute) için kullanmaktayız. Ancak bunu oluştururken App.config dosyasında tuttuğumuz connection string bilgisini doğrudan kullanıyoruz. Bu ve benzeri pek çok yenilik var. Pek çok isim alanı tamamen değişmiş ve daha iyi ayrıştırılmış durumda. Ama ilk göze çarpanlar arasında Ado.Net 2.0 olan tam destek ve sınıfların biraz daha derlenip bir önceki versiyona göre karmaşıklıktan uzaklaştırılmış olması var.

App.config içeriği;

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <connectionStrings>
        <add name="conn" connectionString="Data source=manchester;database=AdventureWorks;integrated security=sspi" providerName="System.Data.SqlClient"/>
    </connectionStrings>
</configuration>
```

CreateDatabase static metodu konfigurasyon dosyası içerisindeki conn isimli anahtarı nasıl bulacağını bilmektedir. Biz bununla da uğraşmamaktayız. Buda uygulamanın yeniden derlemeye gerek duymayacak teknikleri (ki burada xml tabanlı bir konfigurasyon dosyası bunu karşılıyor) kolayca uygulayabilecek bir katmana sahip olduğu anlamına gelmektedir. Şimdi gelin başka bir senaryoyu ele alalım. Örneğin, sistemde yer alan parametrik bir stored procedure'ün sonuçlarını ortama bir DataReader vasıtasıyla almak istediğimiz bir işlevselliği Data Access Layer (veri erişim katmanı) içerisine katmak istediğimizi düşünelim. Bu senaryo için Sql Server 2005 üzerinde, AdventureWorks veritabanında yer alacak aşağıda oluşturma script'i verilen sp'yi kullanabiliriz.

Kullandığımız Stored Procedure;

```text
USE [AdventureWorks]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetStoreByPersonID] 
(
    @PersonID int
)
AS
SELECT * FROM Sales.Store WHERE SalesPersonID=@PersonID
RETURN
```

Şimdi bu sp'yi kullanacak örnek bir kod parçasını kendi DAL'ımız içerisinde aşağıdaki gibi yazdığımızı düşünelim.

```csharp
using (SqlConnection con = new SqlConnection(conStr))
{
    using (SqlCommand cmd = new SqlCommand(queryStr, con))
    {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue(prmName, prmValue);
        con.Open();
        SqlDataReader dr = cmd.ExecuteReader();
    }
}
```

Elbette buraki kod yazım şekli yazılımcıdan yazılımcıya farklılıkta gösterebilir. Oysaki hangi yol ile olursa olsun, Data Access Application Block (Veri Erişimi Uygulama Bloğu)' un sunduğu çizgi bellidir. Bu da aslında, büyük çaplı projelerde farklı yazılımcıların aynı standart üzerinde geliştirme yapabilmesini olanaklı kılmaktadır. Dolayısıyla tek yapmamız gereken eğer bir stored procedure'ü, konfigurasyon dosyasında tutulan bir bağlantı bilgisi üzerinden çalıştırmak ve geriye bir DataReader almak ise, Business (İş) katmanında kullanacağımız metod sadece aşağıdaki iki satır kod parçasını içerecektir.

![mk160_2.gif](images/mk160_2.gif)

```csharp
Database db = DatabaseFactory.CreateDatabase("conn");
db.ExecuteReader("GetStoreByPersonID", 277);
```

Görüldüğü gibi, ExecuteReader metodunun kullandığımız versiyonunda ilk parametre olarak stored procedure'ün adı verilmiştir. İkinci parametre olarak ise, params anahtar sözcüğü kullanılarak n sayıda object tipinden değer girilebilmesi sağlanmıştır. Buna göre ExecuteReader metodu gelen değer sayısına göre içeride parametre oluşturacak ve sp'ye gönderecektir (Bu işlemlerin Data Access Application Block (Veri Erişimi Uygulama Bloğu) içerisindeki kodlarda nasıl yapıldığını görmek için, ExecuteReader satırına Breakpoint koymanızı ve F11 ile adım adım ilerlemenizi öneririm.) Böylece sadece iki kod satırı ile ne kadar çok kod kalabılığından soyutlandığımızı daha kolay görebilir ve veri erişim katmanının faydalarını fark edebilirsiniz. İşte Data Access Layer (veri erişim katmanı) ' ın faydası burada bir kez daha ortaya çıkmaktadır. Tüm kod kalabalığını iş mantığımızdan soyutlamıştır. Ayrıca yönetiminide kendi içerisinde gerçekleştirmektedir.

Başka bir örnek daha ele alalım. Bazen uygulamalarımız n-katlı mimariye sahip olabilir. Örneğin veritabanındaki çeşitli tiplerin sınıfsal karşılığının tutulduğu bir entity katmanımız olduğunu düşünelim. Bu katmanda pekala veri erişim katmanını aktif olarak kullanacaktır. Örneğin, Urun isimli aşağıdaki tipi ele alalım.

![mk160_5.gif](images/mk160_5.gif)

```csharp
public class Urun
{
    private int _id;
    private string _ad;
    private double _fiyat;
    public int Id
    {
        get { return _id; }
        set { _id = value; }
    }
    public string Ad
    {
        get { return _ad; }
        set { _ad = value; }
    }
    public double Fiyat
    {
        get { return _fiyat; }
        set { _fiyat = value; }
    }
    public Urun()
    {
    }
    public void Load(int id)
    {
        Database db = DatabaseFactory.CreateDatabase("conn");
          IDataReader dr = db.ExecuteReader("GetProductById", id);
        dr.Read();
        _id = Convert.ToInt32(dr["ProductID"]);
        _ad = dr["Name"].ToString();
        _fiyat = Convert.ToDouble(dr["StandardCost"]);
    }
}
```

Bu kod parçasında örnek olarak Urun isimli bir entitiy'nin çalışma zamanında karşılık geleceği herhangibir veri satırı için yüklenebilmesini (Load) sağlarken veri erişim katmanından nasıl yararlanabildiğimizi görmektesiniz.

Data Access Application Block (Veri Erişimi Uygulama Bloğu)' un sunduğu imkanlara ait pek çok örnek geliştirebiliriz. Bir diğer örnek ile makalemize devam edelim. Web tabanlı uygulamalarda veri bağlama işlemleri sırasında Business (İş) katmanının veri erişim katmanından nasıl yararlanabileceğini göreceğimiz bir örneğe bakalım.

Business (İş) sınıfımız;

```csharp
public class OurLogic
{
    private Database _db;
    public OurLogic()
    {
        _db = DatabaseFactory.CreateDatabase("conn"); 
    }

    // Ürünleri parametre olarak gelen GridView kontrolüne yükleyen iş katmanı metodu.
    public void BindProducts(GridView dg)
    { 
        dg.DataSource = _db.ExecuteDataSet(CommandType.Text, "Select * From Production.Product");
        dg.DataBind();
    }

    // Ürün kategorilerini parametre olarak gelen DropDownList kontrolüne bağlayan, ve value ile text özelliklerinide ayarlayan bir iş katmanı metodu. Burada ListControl kullanılmasının tek sebebi, sayfalarda yer alan DropDownList ve ListBox kontrollerine de destek verilmesini sağlamaktır. Bu yine ListControl' un polimorfik yapısının sağladığı bir avantajdır.
    public void BindPrdCategories(ListControl dl)
    {
        dl.DataSource=_db.ExecuteReader(CommandType.Text,"SELECT ProductCategoryID, Name FROM Production.ProductCategory");
        dl.DataValueField = "ProductCategoryID";
        dl.DataTextField = "Name";
        dl.DataBind();
    }

    // Ürünlerin sayısını int tipinde döndüren bir iş katmanı metodu. Bu metodda generic bir yapıda kullanılabilir. Böylece presentation katmanından çağırılırken herhangibir tipe karşılık gelecek şekilde ele alınabilir.
    public int GetCategoryCount()
    {
        return (int)_db.ExecuteScalar(CommandType.Text,"SELECT COUNT(*) AS CategoryCount FROM Production.ProductCategory");
    }

    // Urun entity tipini verilen id' ye göre yükleyen ve geri döndüren bir iş katmanı metodu.
    public Urun GetUrun(int id)
    {
        Urun urn = new Urun();
        urn.Load(id);
        return urn;
    }
}
```

Şimdi iş katmanının basit olarak sunum katmanından nasıl kullanıldığına bakalım.

```csharp
public partial class _Default : System.Web.UI.Page 
{
    protected OurLogic _ol;

    protected void Page_Load(object sender, EventArgs e)
    {
        _ol = new OurLogic();
    }
    protected void btnProducts_Click(object sender, EventArgs e)
    {
        _ol.BindProducts(grdProducts);
    }
    protected void btnCategories_Click(object sender, EventArgs e)
    {
        _ol.BindPrdCategories(ddlProductCategories);
    }
    protected void btnGetCount_Click(object sender, EventArgs e)
    {
        lblCatCount.Text=_ol.GetCategoryCount().ToString();
    }
    protected void btnGet_Click(object sender, EventArgs e)
    {
        Urun urn = _ol.GetUrun(Convert.ToInt32(txtId.Text));
        lstLoad.Items.Add(urn.Ad);
        lstLoad.Items.Add(urn.Fiyat.ToString());
    }
}
```

![mk160_6.gif](images/mk160_6.gif)

Görüldüğü gibi Data Access Application Block (Veri Erişimi Uygulama Bloğu) sayesinde uygulamalarımızı geliştirirken sadece iş katmanını ve sunum katmanını düşünmemiz yeterli olmaktadır. Bunun bir avantaj olup olmadığı düşünülebilir. Sonuç itibariyle yazılım mühendisliğine yeni başlayan arkadaşlar için, veri erişim katmanının nasıl olduğunun farkına varmak veya nasıl yazıldığını anlamak için Data Access Application Block (Veri Erişimi Uygulama Bloğu)' u incelemek bile vizyonumuzu geliştirecek önemli bir etkendir. Özellikle kendi projelerimizde, projenin büyüklüğü ile orantılı olacak şekilde kendi veri erişim katmanlarımızı yazmayı tercih edebiliriz. Ancak performans, bellek yönetimi, tutarlılık gibi kriterlerin önemi göz önüne alındığında, Data Access Application Block (Veri Erişimi Uygulama Bloğu)' un en azından incelenmesi gerektiği kanısındayım. Bu makalemizde kısaca Microsoft'un ücretsiz olarak sunduğu enterprise çözüm desenlerinden birisi olan Data Access Application Block (Veri Erişimi Uygulama Bloğu)' un ne olduğunu ve hangi amaçlar ile kullanılabildiğini temel düzeyde incelemeye çalıştık ve geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.
