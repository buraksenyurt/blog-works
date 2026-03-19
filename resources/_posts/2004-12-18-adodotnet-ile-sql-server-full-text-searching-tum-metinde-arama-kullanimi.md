---
layout: post
title: "Ado.Net ile Sql Server Full-Text Searching (Tüm Metinde Arama) Kullanımı"
date: 2004-12-18 12:00:00 +0300
description: "Bu makalemizde, Sql sunucu üzerindeki tablolarda text tabanlı arama işlemlerinin, Full-Text Searching (Tüm Metinde Arama) servisi yardımıyla nasıl gerçekleştirildiğini incelemeye çalışacağız. Konunun pekişmesi açısından basit bir web uygulması ile de bu hizmeti kullanıp text tabanlı arama işlemlerini irdelemeye çalışacağız. Özellikle dikkatinizi çekmiştir ki, internette yer alan pek çok arama motoru aradığımız kelimelerin geçtiği web sayfalarını bulup bize getirir. Çoğunlukla arama motoruna kayıtlı web sayfasının içeriğinde yapılan text tabanlı aramalar da Full-Text Searching (Tüm Metinde Arama) hizmetinden faydalanılır. Bu tip bir arama genellikle alanları içerisinde çok geniş text tabanlı içeriğe sahip olan tablolar üzerinde yapılmaktadır. Sql sunucusu, 7.0 versiyonundan itibaren bu hizmeti vermeye başlamıştır. Elbetteki arama işleminin gerçekleştirilebilmesi için Full-Text Searching (Tüm Metinde Arama) servisinin Sql sunucusunda yüklü olması gerekmektedir."
categories:
  - ado-net
tags:
  - ado-net
  - csharp
  - dotnet
  - aspnet
  - sql-server
  - t-sql
  - datatable
---
# Ado.Net ile Sql Server Full-Text Searching (Tüm Metinde Arama) Kullanımı
Değerli Okurlarım, Merhabalar.

Bu makalemizde, Sql sunucu üzerindeki tablolarda text tabanlı arama işlemlerinin, Full-Text Searching (Tüm Metinde Arama) servisi yardımıyla nasıl gerçekleştirildiğini incelemeye çalışacağız. Konunun pekişmesi açısından basit bir web uygulması ile de bu hizmeti kullanıp text tabanlı arama işlemlerini irdelemeye çalışacağız. Özellikle dikkatinizi çekmiştir ki, internette yer alan pek çok arama motoru aradığımız kelimelerin geçtiği web sayfalarını bulup bize getirir. Çoğunlukla arama motoruna kayıtlı web sayfasının içeriğinde yapılan text tabanlı aramalar da Full-Text Searching (Tüm Metinde Arama) hizmetinden faydalanılır. Bu tip bir arama genellikle alanları içerisinde çok geniş text tabanlı içeriğe sahip olan tablolar üzerinde yapılmaktadır. Sql sunucusu, 7.0 versiyonundan itibaren bu hizmeti vermeye başlamıştır. Elbetteki arama işleminin gerçekleştirilebilmesi için Full-Text Searching (Tüm Metinde Arama) servisinin Sql sunucusunda yüklü olması gerekmektedir.

Dilerseniz hiç vakit kaybetmeden bir tablo için Full-Text Index'in nasıl oluşturulacağına kısaca bakalım. Öncelikle, Full-Text Indeksleme yapacağımız tablonun adına enterprise manager'dan sağ tıklayoruz ve aşağıdaki gibi Define Full-Text Indexing On a Table'ı seçiyoruz. Bu işleme başlamadan önce tablomuzda bir primary key alan olması gerektiğinide belirtelim. Nitekim indeksleme işlemlerinde bu alan, sonuçların döndürülmesinde anahtar alan olarak kullanılmaktadır.

![mk110_1.gif](/assets/images/2004/mk110_1.gif)

Bu işlemin ardından karşımıza çıkan sihirbazdaki adımları birer birer işlemeye başlıyoruz. Az öncede belirttiğim gibi bu ilk olarak tablomuzdaki primary alan için bir index seçiliyor.

![mk110_2.gif](/assets/images/2004/mk110_2.gif)

Bu işlemin ardından, hangi kolonlarda arama yapılacağını belirtiyoruz. Full-Text aramalar çoğunlukla uzun text verilerin tutulduğu alanlarda kullanılır. Örneğin tablomuzdaki Icerik alanı makalelere ait Html içeriği barındırdan ntext veri tipindedir. Burada sadece karakter içerikli veya binary içerikli aramalara müsade eden alanların göründüğünü söyleyelim. Yani primary key olan alanlar veya tarih formatındaki alanlar burada görünmez. Nitekim bu yapıdaki alanlarda Full-Text Indexing yapılması çok da anlamlı değildir. Artık Full-Text Searching (Tüm Metinde Arama) işlemimiz seçmiş olduğumuz alanlar üzerinde yapılabilecektir.

![mk110_3.gif](/assets/images/2004/mk110_3.gif)

Sıradaki adımda ise, arama işlemi için gerekli katalog tanımlanır. Yani Full-Text Indexing aslında bir katalogda tutulmaktadır. Dolayısıyla bizim gerçekleştireceğimiz arama işlemleride bu kataloğu kullanacaktır. Kataloglar standart olarak d:\Program Files\Microsoft SQL Server\MSSQL\ftdata\ fiziki adresinde tutulur.

![mk110_4.gif](/assets/images/2004/mk110_4.gif)

Daha sonraki adımlarda ise kataloğun belirli periyotlarda tekrardan doldurulmasını sağlayacak Schedule (Takvim) ayarlarınıda yapabiliriz. Bu ve izleyen adımları geçtikten sonra tek yapmamız gereken kataloğun doldurulması işlemidir. Bunun içinde, iki seçeneğimiz var. Birisi Start Full Population. Bu seçenek ile katalog baştan itibaren indekslenerek oluşturulur. Bir diğer seçeneğimiz ise Start Incremental Population dır. Bu seçenek sayesinde sadece tablonun eski hali ile yeni hali arasındaki farklar indeksleme işlemine katılarak katalog güncellenir. Bu daha önceden kataloglanmış indeksler için geçerli bir seçenektir. Şu an için biz kataloğu yeni oluşturduğumuzdan Start Full Population seçeneğini kullanacağız.

![mk110_6.gif](/assets/images/2004/mk110_6.gif)

Bu işlemlerin ardınan dikkat ederseniz, tablomuzun yer aldığı database içinde, Full-Text Catalog sekmesinde tanımladığımız kataloğun oluşturulduğunu görürsünüz.

![mk110_5.gif](/assets/images/2004/mk110_5.gif)

Eğer tabloda oluşan güncellemelerden sonra katalog bilgisinin otomatik olarak yenilenmesini istiyorsak bu durumda Change Tracking özelliğin aktif hale getirmeliyiz. Bununla birlikte katalog güncelleme işleminin arka planda yapılmasını sağlamak için Update Index in Background seçeneğini aktif hale getirmeliyiz. Böylece, tablomuza yeni satırlar eklendiğinde, Full-Text Searching (Tüm Metinde Arama) işlemi için kullanılan katalog bilgilerinin otomatik olarak yenilenmesini sağlamış oluruz.

![mk110_12.gif](/assets/images/2004/mk110_12.gif)

Artık tek yapmamız gereken Full-Text Searching (Tüm Metinde Arama) işlemini kullanmak. Bunun için T-SQL'de bir kaç komut var. Bunlardan ikisi, prototipleri aşağıdaki gibi olan FREETEXT ve FREETEXTTABLE anahtar sözcükleridir.

```text
FREETEXT ( Arama Yapılacak Alan , 'Aranacak Kelime' )
FREETEXTTABLE ( Tablo Adi , Arama Yapılacak Alan , 'Aranacak Kelime' )
```

Her iki anahtar sözcükte, belli bir alan üzerinde Full-Text Searching (Tüm Metinde Arama) işlemini gerçekleştirmemizi sağlar. Ancak FreeTextTable sonuçları farklı bir tablo ile geri döndürür. Bu tabloda Rank ve Key isimli iki alan vardır. Key alanı, arama işleminin yapıldığı catalog oluşturulurken kullanılan primary key değerlerini alır. Rank alanı ise, aranan kelimelerin bulunduğu satırlar arasında bir derecelendirme yapılmasına imkan tanır.

![dikkat.gif](/assets/images/2004/dikkat.gif)
Bu derecelendirme sayesinde aranan kelimenin daha çok geçtiği satırlardan, az geçtiği satırlara doğru (yada aranan kelimenin en uygun olarak eşleştirilebildiği alanlardan en az eşleştirildiği alanlara doğru) sıralanmış bir tablo görüntüsü elde etmemiz mümkündür.

İlk olarak Query Analyzer yardımıyla bu sorguların çalıştırılmasını inceleyelim.

![mk110_7.gif](/assets/images/2004/mk110_7.gif)

Burada görüldüğü gibi, FREETEXT anahtar sözcüğü, Icerik isimli alanda overload anahtar kelimesinin geçtiği satırların olduğu sonuç kümesini elde etmemizi sağlamıştır.

![mk110_8.gif](/assets/images/2004/mk110_8.gif)

FreeTextTable anahtar sözüğünün kullanımı ise biraz daha karmaşıktır. Burada arama sonucu oluşan tabloya ait verileri aramanın yapıldığı tablo ile birleştirebilmek amacıyla Join tekniği kullanılmıştır. Join tekniğinde arama işleminde elde edilen tablonun KEY alanı ile Makale isimli tablonun primary key alanı olan ID alanları eşleştirilmiştir. Bu sorgu sonucunda RANK alanına göre tersten sıralı bir veri kümesi elde ederiz. Kümeyi Rank alanına göre tersten sıraladığımız takdirde, aranan kelimenin en uygun şartlarda eşleştirildiği satırlar en üste gelmiş olacaktır.

Şimdi Full-Text Searching (Tüm Metinde Arama) işlemini bir asp.net uygulamasında kullanalım. Bu uygulamada basit olarak bir arama işlemi sonucunda elde edilecek sonuçlar bir DataGrid kontrolünde gösterilecektir. Amacımız bir kelimeyi makalelerin içeriğinde aramak ve bulunan sonuçları kullanıcıya sunmak. Gerçekleştireceğimiz uygulamada aranan kelimenin girileceği TextBox'ta oluşturulabilecek Sql Injection'ların önüne geçmek amacıyla bir Stored Procedure kullanacağız. Öncelikle sp'mizi aşağıdaki gibi oluşturalım.

```text
CREATE PROCEDURE dbo.sp_AraBul
(
      @Aranan nvarchar(255) 
)
AS

SELECT A.*,M.Konu,M.Tarih,M.[ID] FROM Makale AS M 
INNER JOIN
FREETEXTTABLE(Makale,Icerik,@Aranan) AS A 
ON M.[ID]=A.[KEY]
ORDER BY A.RANK DESC

RETURN
```

Sp'miz dışarıdan aranan kelimeyi parametre olarak alacak ve sonuçları çağırıldığı ortama döndürecek. Şimdide asp.net sayfamızı aşağıdaki şekilde oluşturalım.

![mk110_9.gif](/assets/images/2004/mk110_9.gif)

Yanlız burada amacımız sadece aranan kelimenin geçtiği makaleleri bulmak değil. Aynı zamanda bulunan makalelerin olduğu sayfalara link vermek. Bu nedenle dataGrid kontrolümüzün içeriğini aşağıdaki gibi oluşturmamız gerekiyor.

```text
<asp:DataGrid id="dgSonuclar" style="Z-INDEX: 105; LEFT: 48px; POSITION: absolute; TOP: 120px" runat="server" AutoGenerateColumns="False">
<AlternatingItemStyle BackColor="#FFE0C0"></AlternatingItemStyle>
<ItemStyle ForeColor="Black" BackColor="#CCCCCC"></ItemStyle>
<Columns>
<asp:HyperLinkColumn DataNavigateUrlField="ID" DataNavigateUrlFormatString="MakaleGoster.aspx?ID={0}" DataTextField="Konu" HeaderText="Makale Konusu"></asp:HyperLinkColumn>
<asp:BoundColumn DataField="Tarih" SortExpression="Tarih" ReadOnly="True" HeaderText="Yayin Tarihi" DataFormatString="{0:dd-MM-yy}"></asp:BoundColumn>
</Columns>
</asp:DataGrid>
```

Şimdide uygulama kodlarımızı yazalım.

```csharp
private SqlConnection con;
private SqlCommand cmd;
private SqlDataAdapter da;
private DataTable dt;

private void BaslangicAyarlari()
{
    con=new SqlConnection("data source=localhost;initial catalog=bsenyurt;integrated security=SSPI");
    cmd=new SqlCommand("sp_AraBul",con);
    cmd.CommandType=CommandType.StoredProcedure;
    cmd.Parameters.Add("@Aranan",SqlDbType.NVarChar,255);
    da=new SqlDataAdapter(cmd);
    dt=new DataTable();
}
private void Page_Load(object sender, System.EventArgs e)
{
    BaslangicAyarlari();
} 

private void btnBul_Click(object sender, System.EventArgs e)
{
    cmd.Parameters["@Aranan"].Value=txtArananKelime.Text;
    try
    {
        if(con.State==ConnectionState.Closed)
            con.Open();
        da.Fill(dt);
        dgSonuclar.DataSource=dt;
        dgSonuclar.DataBind();
    }
    catch(SqlException hata)
    {
        Label1.Text=hata.Message.ToString();
    }
    finally
    {
        if(con.State==ConnectionState.Open)
        con.Close();
    }
}
```

Uygulamamızı çalıştırdığımızda aşağıdaki sonucu elde ederiz.

![mk110_10.gif](/assets/images/2004/mk110_10.gif)

Full-Text Searching (Tüm Metinde Arama) tekniğinde sadece tek bir kelime üzerinden arama yapmak zorunda değiliz. Örneğin "Matematik Mühendisi" kelimelerinin ardışık olarak geçtiği yerleride kolayca bulabiliriz. Tabi böyle bir arama sonucunda "Matematik Mühendisi" kelimesi ile bire bir eşleşen metinlerin bulunduğu satırlar ile "mühendisi" kelimesinin olduğu ama "Matematik" kelimesinin olmadığı satırlarda elde edilcektir.

Diğer yandan içerik alanında hem overload kelimesi hemde interface kelimesi geçen makaleleri bulmak istediğimiz bir örnek ile karşılaşırsak ne yaparız? İşte böyle bir durumda And, Or gibi mantıksal operatorlerin sunduğu imkanlardan faydalanmamız gerekecektir. Bunun için Full-Text Searching (Tüm Metinde Arama) işlemlerinde kullanabileceğimiz iki yeni sql anahtar sözcüğü vardır. Bunlar CONTAINS ve CONTAINSTABLE anahtar sözcükleridir. Bu ifadelerde AND, OR, NEAR, gibi mantıksal operatorler kullanılarak arama işlemleri daha detaylı bir şekilde gerçekleştirilebilir. Örneğin aşağıdaki sorgu, overload ve interface kelimelerinin bir arada geçtiği alanları arar.

```text
SELECT A.*,M.Konu,M.[ID] FROM Makale AS M INNER JOIN
CONTAINSTABLE(Makale,Icerik,'overload AND interface') AS A ON M.[ID]=A.[KEY]
ORDER BY A.RANK DESC
```

Diğer yandan aynı aramayı overload veya interface kelimelerinden herhangibirinin geçtiği alanlar üzerinde de aşağıdaki sorgu ile gerçekleştirebiliriz. Tek yapmamız gereken aranan kelimeler arasına Or operatorunu koymak olacaktır.

![mk110_11.gif](/assets/images/2004/mk110_11.gif)

And ve Or operatörleri dışında kullanabileceğimiz bir diğer kullanışlı operatörde, * asteriks karakteridir. Örneğin,

```csharp
SELECT A.*,M.Konu,M.[ID] FROM Makale AS M INNER JOIN
CONTAINSTABLE(Makale,Icerik,' "Datarela*" ') AS A ON M.[ID]=A.[KEY]
ORDER BY A.RANK DESC
```

sorgusu yardımıyla Datarela ile başlayan sözcükleri içeren alanların olduğu veri kümesini elde edebiliriz. CONTAINS ve CONTAINSTABLE aramalarında kullanabileceğimiz diğer operatörler için Sql Help'e bakabilirsiniz. Bu makalemizde Full-Text Searching (Tüm Metinde Arama) işlemlerinin nasıl yapıldığını incelemeye çalıştık. Umuyorum ki yararlı bir makale olmuştur. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.