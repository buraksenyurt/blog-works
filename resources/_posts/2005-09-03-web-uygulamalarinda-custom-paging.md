---
layout: post
title: "Web Uygulamalarında Custom Paging"
date: 2005-09-03 12:00:00 +0300
description: "Geliştirdiğimiz web uygulamalarında özellikle DataGrid kontrollerini kullandığımızda sayfalama işlemini sıkça kullanırız. Genellikle sayfalama işlemlerini var sayılan hali ile kullanırız. Bu modele göre grid üzerinde sayfalama yapabilmek için PageIndexChanged olayını ele almamız gerekir. Burada grid kontrolüne yeni sayfa numarasını DataGridPageChangedEventArgs parametresinin NewPageIndex değeri ile verir ve bilgilerin tekrardan yüklenmesini sağlayacak uygulama kodlarımızı yürütürüz. Tipik olarak bu tarz bir kullanım aşağıdaki kod parçasında olduğu gibi yapılmaktadır."
categories:
  - aspnet
tags:
  - aspnet
  - csharp
  - dotnet
  - performance
  - caching
  - datatable
---
# Web Uygulamalarında Custom Paging
Değerli Okurlarım Merhabalar,

Geliştirdiğimiz web uygulamalarında özellikle DataGrid kontrollerini kullandığımızda sayfalama işlemini sıkça kullanırız. Genellikle sayfalama işlemlerini var sayılan hali ile kullanırız. Bu modele göre grid üzerinde sayfalama yapabilmek için PageIndexChanged olayını ele almamız gerekir. Burada grid kontrolüne yeni sayfa numarasını DataGridPageChangedEventArgs parametresinin NewPageIndex değeri ile verir ve bilgilerin tekrardan yüklenmesini sağlayacak uygulama kodlarımızı yürütürüz. Tipik olarak bu tarz bir kullanım aşağıdaki kod parçasında olduğu gibi yapılmaktadır.

```csharp
private void Doldur()
{
    //Bağlantının açılması, verilerin çekilmesi ve çekilen verilerin DataGrid kontrolüne bağlanması
}

private void DataGrid1_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
{
    DataGrid1.CurrentPageIndex=e.NewPageIndex;
    Doldur();
}
```

Buradaki yaklaşım gerçekten işe yaramaktadır. Ancak aslında performans açısından bazı kayıplar söz konusudur. Çünkü bu tip sayfalama tekniğini kullanırken, sayfa linklerine her basışımızda ilgili veri kaynağındaki tüm veriler çekilmekte ve çekilen veri kümesi üzerinde ilgili sayfaya gidilmektedir. Bu elbetteki büyük boyutlu veri kümeleri ile çalışırken dikkate alınması gereken bir durumdur. Nitekim performansı olumsuz yönde etkileyecektir. Her ne kadar caching (tampon belleğe almak) teknikleri ile sorunun biraz olsun üstesinden gelinebilecek olsa da daha etkili çözümler üretebiliriz.

![dikkat.gif](/assets/images/2005/dikkat.gif)
Büyük boyutlu veri kümeleri ile çalışırken uygulanan varsayılan sayfalama tekniği hız kaybına neden olarak performansı olumsuz yönde etkileyebilir.

İşte bu makalemizde özel sayfalama tekniklerinden bir tanesini incelemeye çalışacağız. Bu teknikte yer alan parçalardan en önemlisi şablon bir tablonun (temporary) kullanılmasıdır. İlk olarak asıl veri kümesini ele alacağımız bir stored procedure (saklı yordam) geliştireceğiz. Bu saklı yordamımız içerisinde asıl veri kümesinin satırlarını alıp temprary bir tabloya aktaracağız. Temporary tablomuzun en büyük özelliği identity tipinde 1 den başlayan ve 1' er artan bir alana sahip olmasıdır. Biz bu alanın değerinden faydalanarak temp tablosu üzerinde sayfalama işlemini uygulayacağız. Buradaki ana fikiri daha iyi anlamak için aşağıdaki şekile bir göz atalım.

![mk134_1.gif](/assets/images/2005/mk134_1.gif)

Senaryomuzda AdventureWorks2000 veritabanı içerisinde yer alan Employee isimli tabloyu kullanacağız. Bu tablo üzerinde aşağıdaki select sorgusu için sayfalama işlemini gerçekleştireceğiz.

```text
SELECT EmployeeID,FirstName,LastName,NationalIDNumber,Title,BirthDate,EmailAddress FROM Employee
```

Teorimizi şekil üzerinden açıklamaya çalışalım. Önce bir temp tablosu oluşturacağız. Temp tablomuzun alanları yukarıdaki select sorgusundaki alanları karşılayacak şekilde olacak. Bir de ekstradan identity alanımız olacak. Sonra select sorgumuzdaki verileri, temp tablomuz içerisine insert edeceğiz. Ardından temp tablomuz üzerinden yeni bir select sorgusu çalıştıracağız. Ancak bu sefer, Where koşulumuz olacak ve burada identity alanımız için bir aralık belirleyeceğiz. İşte bu aralık sayfanın başlangıç ve bitiş satırlarını belirleyerek istediğimiz sayfaya ait verileri elde etmemizi sağlayacak.

Örneğin, verilerimizi 5' er satırlık sayfalara bölmek istediğimizi düşünelim. Bu durumda 2nci sayfadaki ilk satırın id değerini Baslangic isimli formülümüzden 6 olarak bulabiliriz. Yine 2nci sayfanın bitis satırının değerinide Bitis isimli formülü kullanarak 10 olarak bulabiliriz. Gördüğünüz gibi teori son derece basit. Sayfalamayı gerçekleştirebilmek için aslında temp tablodaki identity alanlarını kullanıyoruz. Son olarak bu işlemlerin hepsini bir stored procedure (saklı yordam) içerisinde barındırarak işlemlerin doğrudan sql sunucusu üzerinde ne hızlı şekilde gerçekleştirilmesini sağlıyoruz. İşte sp kodlarımız.

```text
CREATE PROCEDURE WorkSp_Paging 

@SayfaNo INT,
@GosterilecekSatirSayisi INT

AS

DECLARE @BaslangicID AS INT
DECLARE @BitisID AS INT
DECLARE @SelectSorgusu AS NVARCHAR(255)

-- Önce Temporary tablomuzu oluşturuyoruz. ID alanı önemli.
CREATE TABLE #TempOfEmployee 
(
ID INT IDENTITY(1,1),
EmployeeID INT,
FirstName NVARCHAR(50),
LastName NVARCHAR(50),
NationalIDNumber NVARCHAR(15),
Title NVARCHAR(50),
BirthDate DATETIME,
EmailAddress NVARCHAR(50)
)

-- Employee tablosundaki verileri temporary tablomuza aktarıyoruz.
SET @SelectSorgusu='SELECT EmployeeID,FirstName,LastName,NationalIDNumber,Title,BirthDate,EmailAddress FROM Employee'
INSERT INTO #TempOfEmployee EXEC (@SelectSorgusu)

-- Başlangıç ve bitiş satırlarının ID alanlarının değerlerini belirlemek için formülasyonumuzu kullanıyoruz. SayfaNo ve GösterilecekSatirSayisi sp mize dışarıdan gelen parametreler.
SET @BaslangicID=((@SayfaNo-1)*@GosterilecekSatirSayisi)
SET @BitisID=(@SayfaNo*@GosterilecekSatirSayisi)+1

-- Temporary tablomuz üzerinden ilgili ID aralığındaki veri setini çekiyoruz.
SELECT ID,EmployeeID,FirstName,LastName,NationalIDNumber,Title,BirthDate,EmailAddress FROM #TempOfEmployee
WHERE ID>@BaslangicID AND ID<@BitisID

-- Son olarak sistemde bir karmaşıklığa yer vermemek için temporary tablomuzu kaldırıyoruz.
DROP TABLE #TempOfEmployee
GO
```

Şimdi sp'mizi asp.net uygulamamızda kullanalım. Özel sayfalama yaptığımız için artık PageIndexChanged olayını kullanamayacağız. Dolayısıyla sayfa linklerini manuel olarak oluşturmamız gerekiyor. Bu durumda DataGrid kontrolümüze ait AllowPaging ve AllowCustomPaging özelliklerinin değerlerini false olarak bırakabiliriz. Eğer sadece ilk, önceki, sonraki, son tarzında linkler oluşturacak isek işimiz kolay. İlgili metodumuza sayfa numarasını ve göstereceğimiz satır sayısını parametre olarak göndermemiz yeterli olacaktır. Ancak sayfa numalarını link olarak sunmak istiyorsak biraz daha fazla çabalamamız gerekecek. Öncelikle asıl işi yapan metodumuzu aşağıdaki gibi oluşturalım. Bu metodumuzda tanımlamış olduğumuz sp'mizi bir SqlCommand nesnesi yardımıyla yürütüyor ve elde ettiğimiz sonuç kümesini DataGrid kontrolümüze bağlıyoruz.

```csharp
private void Doldur(int sayfaNo,int gosterilecekSatirSayisi)
{
    string sql=@"WorkSp_Paging";
    cmd=new SqlCommand(sql,con);
    cmd.CommandType=CommandType.StoredProcedure;
    cmd.Parameters.Add("@SayfaNo",sayfaNo);
    cmd.Parameters.Add("@GosterilecekSatirSayisi",gosterilecekSatirSayisi);
    da=new SqlDataAdapter(cmd);
    dt=new DataTable();
    da.Fill(dt);
    DataGrid1.DataSource=dt;
    DataGrid1.DataBind();
}
```

Şimdi linklerimizi oluşturalım. Burada kodlama tekniği açısından tamamen serbestsiniz. Ben aşağıdaki gibi kodlamayı tercih ettim. Öncelikle Employee tablosundaki satır sayısını buluyoruz. Sonra sayfalarda gösterilecek satır sayısı ile bunu oranlayarak sayfa sayısını buluyoruz. Sayfa sayısını bulurken dikkat etmemiz gereken nokta artık satırlar için sayfa numarasını bir arttırmamız gerektiğidir. Bunu tespit edebilmek için toplam satır sayısını sayfada gösterilecek satır sayısına bölerken mod operatörü (%) yardımıyla kalan değeri hesaplıyoruz. Eğer kalan değer 0 ise problem yok. Sayfa sayısı tamdır. Ancak 0 değil ise bu durumda sayfa sayısını bir arttırmalıyız ki kalan satırlarıda en sondaki sayfada gösterebilelim.

Bu teknik yardımıyla sayfa sayısını tespit etmemizin ardından her bir sayfa numarası için birer LinkButton kontrolü oluşturuyoruz. Bu kontrollerin Text ve ID özelliklerine ilgili sayfa numarasını set ettikten sonra sayfadaki placeHolder kontrolüne ekliyoruz. Ayrıca LinkButton'lar arasında birer boşluk olmasını sağlamak için Label kontrollerini kullanabiliriz. Şimdi burada önemli olan nokta LinkButton nesnelerinden birisine tıklandığında ilgili sp'mizi çalıştıracak olan metodumuzu çağırabilmek. Bunun için her bir LinkButton nesnesini döngü içerisinde oluştururken aynı Click olay metoduna yönlendiriyoruz. Bu olay metodu içerisinde yaptığımız iş ise ilgili LinkButton kontrolünün ID değerini almak ve Doldur isimli metoda göndermek. Böylece ilgili linke tıklandığında doğruca sp'miz çalıştırılacak ve ilgili sayfaya ait veri seti ekrandaki grid kontrolümüze dolacak.

```csharp
private void SatirSayisiniBul()
{
    cmd=new SqlCommand("SELECT COUNT(*) FROM Employee",con);
    con.Open();
    int toplamSatirSayisi=Convert.ToInt32(cmd.ExecuteScalar());
    con.Close();
    ViewState.Add("TSS",toplamSatirSayisi);
}

private void LinkleriOlustur(int gosterilecekSatirSayisi)
{
    if(ViewState["TSS"]==null)
    {
        SatirSayisiniBul();
    }

    int kalanSatirSayisi=toplamSatirSayisi%gosterilecekSatirSayisi;
    int sayfaSayisi;
    if(kalanSatirSayisi==0)
        sayfaSayisi=(toplamSatirSayisi/gosterilecekSatirSayisi);
    else
        sayfaSayisi=(toplamSatirSayisi/gosterilecekSatirSayisi)+1;

    for(int i=1;i<sayfaSayisi;i++)
    {
        LinkButton link=new LinkButton();
        Label lbl=new Label();
    
        link.Text=i.ToString();
        link.ID=i.ToString();
        link.Click+=new EventHandler(link_Click);
        lbl.Text=" ";
        plhLinkler.Controls.Add(link);
        plhLinkler.Controls.Add(lbl);
    } 
}

private void link_Click(object sender, EventArgs e)
{
    LinkButton currLink=(LinkButton)sender;
    int sayfaNo=Convert.ToInt16(currLink.ID);
    Doldur(sayfaNo,5);
}
```

Son olarak sayfamız yüklenirken Load olayında olmasını istediğimiz kodlarıda aşağıdaki gibi ekleyelim.

```csharp
private void Page_Load(object sender, System.EventArgs e)
{ 
    con=new SqlConnection("data source=localhost;database=AdventureWorks2000;integrated security=SSPI");
    LinkleriOlustur(5);
    if(!Page.IsPostBack)
    { 
        Doldur(1,5); 
    }
}
```

Uygulamamızı çalıştıracak olursak sayfalar arasında başarılı bir şekilde dolaşabildiğimizi görürüz. Görüldüğü gibi özel sayfalama işlemi biraz meşakkatli bir yol gerektirse de performans açısından oldukça iyi verim sunacaktır. Buradaki performans farkını iyi anlayabilmek için normal sayfalama ve özel sayfalama tekniklerini iyice kavramak gerekir. Bir kere daha özetleyecek olursak, normal sayfalama tekniğinde her bir sayfada tüm veri seti tekrardan ilgili bağlantsız katman kontrolüne doldurulmaktadır.

Bizim kullandığımız tekniktede dikkat ederseniz buna benzer bir yaklaşım söz konusudur. Çünkü bizde sp'miz içerisinde tüm veri setini çekip bir temp tablo içerisine alıyoruz. Yanlız biz bu işlemi sql sunucusu üzerinde gerçekleştiriyoruz. Oysaki normal sayfalamada hakikaten tüm veri kümesi bağlantısız katman nesnesine doldurulmaktadır. Bizim tekniğimizde ise sadece ilgili sayfadaki belirtilen satır sayısı kadarlık bir veri seti bağlantısız katman nesnesine doldurulmaktadır. İşte aradaki en büyük fark budur. Ki bu fark bize performans sağlamaktadır. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Kodlar İçin Tıklayınız.](/assets/files/2005/UsingCustomPaging.rar)