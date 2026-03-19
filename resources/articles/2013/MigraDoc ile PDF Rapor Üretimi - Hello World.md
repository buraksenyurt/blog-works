---
title: "MigraDoc ile PDF Rapor Üretimi - Hello World"
pubDate: 2013-08-18 11:48:00
categories:
  - C#
tags:
  - C#
  - XML
  - .NET
  - Windows Forms
  - HTTP
  - Async/Await
  - Generics
  - DataTable
---

# MigraDoc ile PDF Rapor Üretimi - Hello World
[![cpdf_10](images/cpdf_10_thumb.png)](images/cpdf_10.png)"Mösyö Reno" dedi, oturduğu yerden Jimmy Carl.

Uzun süredir bu dev şirketi yönetiyordu. Son zamanlarda teknolojiye büyük yatırım yapan firmanın, bundan en iyi şekilde yararlanabilmesini isteyenlerin başında geliyordu. Oldukça meraklı biri olan Carl, bilgisayarına bakarken içeriye orta boylarda, saçlarının bir kısmı ağırmış, numarası büyük olduğu belli olan kalın çerçeveli gözlüklü, hafif de göbekli ama güler yüzlü birisi girdi. Üstünde rengarenk bir hawai t-shirt, altında bermuda şort ve parmak arası terlikleri ile.

"Buyrun" dedi Reno, nefes nefese kalmış bir halde.

Fransız, yazılım alanında çift doktora yapmış birisiydi. Şirketin en kilit projelerinde görev almıştı. Bu yüzden Carl'ın da bir numaralı adamıydı.

"Sizin için ne yapabilirim?" diyerek devam etti sözlerine Fransız.

"Şu son satış rakamlarına ait raporlı diyorum Mösyö; acaba bunları PDF dosyasına kayıt edebilir miyiz?"

Gülümsedi Fransız Reno.

"Neden olmasın? Bana mesai bitimine kadar müddet verin lütfen."

Merhaba Arkadaşlar,

Özellikle veri odaklı (Data-Centric) çalışan uygulamalar düşünüldüğünde çeşitleri ne olursa olsun raporlama, işin oldukça önemli bir parçasını oluşturmaktadır. Ağırlıklı olarak rapora ihtiyaç duyan pozisyonlar, söz konusu raporları çeşitli ortamlarda görmek isteyen elemanlardır. Örneğin bunları Web arayüzünde açabilmeyi, Excel veya Word formatındaki dosyalara çıktı olarak alabilmeyi ve mobil cihazlarından takip edebilmeyi isterler. Günümüzün pek çok modern uygulaması zaten bu tip çıktıların alınmasını standart olarak olanak sunmaktadır.

Elbette çok farklı istekler de gelebilmektedir. Söz gelimi çıktı olarak basılacak veya bir dergi içerisinde kullanılması düşünülen raporlar için PDF, XPS gibi dosya formatlarında üretilmeleri istenebilir. İşte bu yazımızda bir rapor içeriğinin, PDF formatında nasıl oluşturulabileceğini basit bir Hello World uygulaması ile anlamaya çalışacağız. Örnek uygulamamızda açık kaynak olarak sunulan MigraDoc kütüphanelerinden yararlanacağız.

> MigraDoc aslında, PDFSharp and MigraDoc Foundation isimli ürün ailesinin bir parçasıdır. Bu ürünlere ait kaynak kodları veya derlenmiş Binary dosyalarını [Codeplex üzerinden](http://pdfsharp.codeplex.com/releases) indirebilirsiniz.

Senaryo

İlk olarak örnek senaryomuzu ele alalım. Bilindiği üzere Northwind veritabanında aşağıdaki ekran görüntüsünde yer alan standart View nesneleri varsayılan olarak yer almaktadır.

[![cpdf_7](images/cpdf_7_thumb.png)](images/cpdf_7.png)

Bu View nesnelerinde örnek pek çok rapor içeriği bulunmaktadır. Söz gelimi 1997 yılına ait kategori bazlı satışları veya yıldan yıla satış rakamlarının özetini görebilir, listede yer alan tüm ürünlerimizi kategori bazlı olarak elde edebiliriz. Dolayısıyla bu veriler raporlanabilir nitelikte olup çıktı şeklinde değerlendirilebilirler.

Senaryomuza göre basit bir Windows Forms uygulamasında, Northwind veritabanı içerisinde yer alan View nesnelerini kullanıcıya seçilebilir halde sunuyor olacağız. Bu View nesnelerinden her hangi biri seçildiğinde ise, veri içeriğini barındıran bir PDF dokümanının üretilmesini sağlayacağız. Örneğimizde ulaşmak istediğimiz hedef aşağıdaki ekran görüntüsündekine benzer olacaktır.

[![cpdf_2](images/cpdf_2_thumb.png)](images/cpdf_2.png)

Sol üst köşede şirkete ait bir logo, Footer ve Header kısımlarında açık gri formatta bir bilgi, raporun alındığı View nesnesinin adı, üretildiği tarih, detay için URL adresine gönderme yapan bir link ve çok doğal olarak verinin kendisini içeren bir tablo. Peki bu içeriği nasıl üretiyor olacağız?

Kodlama Zamanı

Örneğimiz basit bir Windows Forms uygulaması şeklinde geliştirilecektir. İlk etapta uygulamaya aşağıdaki görselde yer alan MigraDoc.DocumentObjectModel, MigraDoc.Rendering ve MigraDoc.RtfRendering isim Assembly'ları referans etmemiz gerekiyor.

[![cpdf_1](images/cpdf_1_thumb.png)](images/cpdf_1.png)

İlgili referansların eklenmesini takiben, aşağıdaki ekran görüntüsünde yer alan Form içeriğini tasarlayarak devam edebiliriz.

[![cpdf_4](images/cpdf_4_thumb.png)](images/cpdf_4.png)

Pek tabi uygulamamız Northwind veritabanına bağlanacağından bazı SQL işlemlerini de icra ediyor olacak. Söz gelimi View adlarını, sys.Views metadata içeriğini kullanarak çekeceğiz. Diğer yandan bir View nesnesinin sunduğu veriyi almak için de SQL sorgusuna da ihtiyacımız olacak. Uzun lafın kısası temelde bir Connection String bilgisi gerekiyor

![Laughing out loud](images/wlEmoticon-laughingoutloud_9.png)

İşte o bilgiyi App.config dosyasından tedarik edebiliriz.

```xml
<?xml version="1.0" encoding="utf-8" ?> 
<configuration> 
  <connectionStrings> 
    <add name="Northwind" 
         connectionString="data source=localhost;initial catalog=Northwind;integrated security=SSPI" 
         providerName ="System.Data.SqlClient" 
         /> 
  </connectionStrings> 
</configuration>
```

Genel fonksiyonelliklerimizi Utility isimli yardımcı bir static sınıf içerisinde tutabiliriz. Sınıf içeriği biraz uzun olduğundan adım adım ilerlemeye çalışmanızı öneririm. İşte Utility içeriği;

[![cpdf_8](images/cpdf_8_thumb.png)](images/cpdf_8.png)

```csharp
using MigraDoc.DocumentObjectModel; 
using MigraDoc.DocumentObjectModel.Shapes; 
using MigraDoc.DocumentObjectModel.Tables; 
using MigraDoc.Rendering; 
using System; 
using System.Collections.Generic; 
using System.Configuration; 
using System.Data; 
using System.Data.SqlClient; 
using System.IO;

namespace ReportApp 
{ 
    public static class Utility 
    { 
        #region Genel değişkenler

        static Document document = null; 
        static Table table = null;

        #endregion Genel değişkenler

        // Hazır bir SqlConnection nesnesini bize verecek olan private fonksiyonumuz 
        static SqlConnection GetConnection() 
        { 
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Northwind"].ConnectionString); 
            if (conn.State == ConnectionState.Closed) 
                conn.Open(); 
            return conn; 
        }

        // Rapor almak için kullanacağımız View bilgilerini çektiğimiz fonksiyon 
       public static List<string> GetNorthwindViews() 
        { 
            List<string> viewNames = new List<string>();

            string query = "Select name from sys.views order by name"; 
            using(SqlConnection conn=GetConnection()) 
            { 
                using(SqlCommand cmd=new SqlCommand(query,conn)) 
                { 
                    SqlDataReader reader = cmd.ExecuteReader(); 
                    while (reader.Read()) 
                    { 
                        viewNames.Add(reader["name"].ToString()); 
                    } 
                    reader.Close(); 
                } 
            } 
            return viewNames; 
        }

        // PDF Dosyasını oluşturacak olan metodumuz 
        public static bool CreatePDFReportFile(string fileName,string viewName) 
        { 
            bool result = false; 

            document = new Document(); 
            DataTable dataContent=GetViewContent(viewName);

            CreateDefaultStyles(); 
            CreatePDFSection(viewName,fileName, dataContent); 
           FillDataToContent(dataContent); 
            SavePdfFile(fileName);

            result = true;

            return result; 
        }

        // İlgili View içeriğini bir DataTable nesnesi olarak geriye döndüren private metodumuz 
        static DataTable GetViewContent(string viewName) 
        { 
            string query = string.Format("select * from [{0}]", viewName); 
            DataTable table = new DataTable(); 
            using (SqlConnection conn = GetConnection()) 
            { 
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn)) 
                { 
                    adapter.Fill(table); 
                } 
            }

            return table; 
        }

        // PDF içeriğinde kullanılacak olan stiller belirlenir. Bu Style tiplerini HTML içeriğindeki style kavramına benzetebiliriz. 
        static void CreateDefaultStyles() 
        { 
            Style style = document.Styles["Normal"]; 
            style.Font.Name = "Calibri"; 
            style = document.Styles[StyleNames.Header]; 
            style.ParagraphFormat.AddTabStop("12cm", TabAlignment.Right); 
            style = document.Styles[StyleNames.Footer]; 
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // Table isimli bir style oluşturuyoruz. Normal isimli Style' dan türemekte 
            style = document.Styles.AddStyle("Table", "Normal"); 
            style.Font.Name = "Calibri"; 
            style.Font.Size = 9;

            // Normal isimli Style' ı baz alan Reference isimli bir Style oluşturuyoruz 
            style = document.Styles.AddStyle("Reference", "Normal"); 
            style.ParagraphFormat.SpaceBefore = "3mm"; 
            style.ParagraphFormat.SpaceAfter = "3mm"; 
            style.ParagraphFormat.TabStops.AddTabStop("12cm", TabAlignment.Right); 
        }

        // PDF Sayfası üretilir. 
        static void CreatePDFSection(string viewName,string filePath,DataTable dataTable) 
        { 
            string info = string.Format("{0} raporu - Northwind Tarafından Üretilmiştir - Her Hakkı Saklıdır {1}" 
                , viewName 
                , DateTime.Now.Year);

            // Landscape olacak şekilde dokümanın yönünü belirliyoruz 
            document.DefaultPageSetup.Orientation = Orientation.Landscape; 
            // İçeriğimizi koyacağımız bir Section oluşturuyoruz 
            Section section = document.AddSection();

            #region Firma Logosunun Eklenmesi

            Image image = section.AddImage(Path.Combine(Environment.CurrentDirectory, "northwind.jpg"));            
            image.Top = ShapePosition.Top; 
            image.Left = ShapePosition.Left;                     

            #endregion Firma Logosunun Eklenmesi

            #region Header kısmı

            Paragraph paragraph = section.Headers.Primary.AddParagraph(); 
            paragraph.AddText(info); 
            paragraph.Format.Font.Size = 10; 
            paragraph.Format.Font.Color = Colors.LightGray; 
            paragraph.Format.Alignment = ParagraphAlignment.Left;

            #endregion Header kısmı

            #region Footer kısmı

            paragraph = section.Footers.Primary.AddParagraph(); 
            paragraph.AddText(info); 
            paragraph.Format.Font.Size = 10; 
            paragraph.Format.Font.Color = Colors.LightGray; 
            paragraph.Format.Alignment = ParagraphAlignment.Left;

            #endregion Footer kısmı

            #region Adres bildirimi 
                        
            paragraph = section.AddParagraph(); 
            paragraph.Format.SpaceBefore = "3cm"; 
            paragraph.Style = "Reference"; 
            paragraph.AddFormattedText(viewName, TextFormat.Italic); 
            paragraph.AddTab(); 
            paragraph.AddText("Rapor Tarihi, "); 
            paragraph.AddDateField("dd.MM.yyyy"); 
            paragraph.AddLineBreak(); 
            paragraph.AddText("Rapor, Migra Document API ile üretilmiştir"); 
            paragraph.AddLineBreak(); 
            Hyperlink link = paragraph.AddHyperlink("http://www.buraksenyurt.com"); 
            link.Type = HyperlinkType.Url; 
            link.Font.Underline = Underline.Single; 
            link.AddText("Detaylı Bilgi burada");

            #endregion Adres bildirimi

            #region View içeriğinin basılacağı Table ve öğelerinin üretimi

            table = section.AddTable(); 
            table.Style = "Table"; 
            table.Borders.Color = Colors.LightYellow; 
            table.Borders.Width = 0.25; 
            table.Borders.Left.Width = 0.5; 
            table.Borders.Right.Width = 0.5; 
            table.Rows.LeftIndent = 0;

            Column column; 
            foreach (DataColumn col in dataTable.Columns) 
            { 
                column = table.AddColumn(Unit.FromCentimeter(2.5)); 
                column.Format.Alignment = ParagraphAlignment.Center; 
            }

            // Tablonun Header kısmı üretiliyor 
            Row row = table.AddRow(); 
            row.HeadingFormat = true; 
            row.Format.Alignment = ParagraphAlignment.Center; 
            row.Format.Font.Bold = true;

            for (int i = 0; i < dataTable.Columns.Count; i++) 
            { 
                row.Cells[i].AddParagraph(dataTable.Columns[i].ColumnName); 
                row.Cells[i].Format.Font.Color = Colors.White; 
                row.Cells[i].Format.Shading.Color = Colors.Black; 
                row.Cells[i].Format.Font.Bold = true; 
                row.Cells[i].Format.Alignment = ParagraphAlignment.Left; 
                row.Cells[i].VerticalAlignment = VerticalAlignment.Bottom; 
            } 
            table.SetEdge(0, 0, dataTable.Columns.Count, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);

            #endregion View içeriğinin basılacağı Table ve öğelerinin üretimi 
        }

        // Sayfa içeriği parametre olarak gelen DataTable içeriğine göre doldurulur 
        static void FillDataToContent(DataTable dataTable) 
        { 
            Row newRow; 
            for (int i = 0; i < dataTable.Rows.Count; i++) 
            { 
                newRow = table.AddRow(); 
                newRow.TopPadding = 1.5; 
                for (int j = 0; j < dataTable.Columns.Count; j++) 
                { 
                    newRow.Cells[j].Shading.Color = Colors.Gold; 
                    newRow.Cells[j].VerticalAlignment = VerticalAlignment.Center; 
                    newRow.Cells[j].Format.Alignment = ParagraphAlignment.Left; 
                    newRow.Cells[j].Format.FirstLineIndent = 1; 
                    newRow.Cells[j].AddParagraph(dataTable.Rows[i][j].ToString()); 
                    table.SetEdge(0, table.Rows.Count - 2, dataTable.Columns.Count, 1, Edge.Box, BorderStyle.Single, 0.75); 
                } 
            } 
        }

        // PDF Dosyası parametre olarak belirtilen adrese kayıt edilir 
        static void SavePdfFile(string fileName) 
        { 
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);            
            pdfRenderer.Document = document; 
            pdfRenderer.RenderDocument(); 
            pdfRenderer.Save(fileName);                       
        } 
    } 
}
```

Aslında fonksiyonları dikkatlice incelediğimizde PDF içeriğini oluşturma işleminin oldukça basit olduğunu görebiliriz. Baş kahraman MigraDoc.DocumentObjectModel içerisinde yer alan Document tipidir. Tahmin edileceği üzere MigraDoc, PDF içeriğine ait Document Object Model'i kullanmaktadır.

Document tipi aslında sayfa içerisinde kullanılacak global Style'leri ve Section'ları oluşturmak için kullanılır. Bu nedenle PDF içersine bir paragraf, tablo, resim, link ve benzeri materyalleri eklemek istediğimizde bir Section tipinden yararlanmalıyız. Örnekte dikkat edileceği üzere Logo'nun eklenmesi için section nesne örneği üzerinden AddImage metodu çağırılmaktadır. Diğer yandan bir paragraf eklenmek istendiğinde yine section nesne örneği üzerinden ama bu kez AddParagraph fonksiyonuna çağrıda bulunulmaktadır.

Örneğimizin kalbi rapor içeriklerinin bir tablo içerisinde gösterilmesidir. Bu sebepe bir Table tipi kullanılmış ve örneklenmesi için yine section nesnesi üzerinden hareket edilerek, AddTable metoduna başvuruda bulunulmuştur. Bir Table örneklendikten sonra içerisinde yer alan satırların (Rows) veya hücrelerin (Cells) doldurulması işi, aslında bir HTML Table içeriğinin dinamik olarak üretilmesinden pek de farklı değildir. Yeterki doküman nesne modeline hakim olalım

![Winking smile](images/wlEmoticon-winkingsmile_192.png)

Örneğimizin daha kolay anlaşılması açısından aşağıdaki görseli dikkate alabilirsiniz.

[![cpdf_11](images/cpdf_11_thumb.png)](images/cpdf_11.png)

Kodlama sırasında keşfetmemiz gereken veya bizleri zorlayabilecek olan noktalar genellikle ilgili içeriğin (paragraf, tablo, resim vb) sayfa içerisindeki yerleşimlerinin ayarlanmasıdır. Aslında bir kaç deneme yanılma ile düzgün bir şablon oturtabiliriz. Ancak yine de dikkate alınması gereken bazı hususlar vardır. Söz gelimi View'un döndürdüğü sonuç kümesinde yer alan kolon sayısının çok fazla olması halinde sayfaya sığmayacak ve yatay olarak görüntü kayıpları yaşanacaktır ki bu Jimmy Carl'ın pek de hoşuna gitmeyecektir. Bu gibi ileri seviye sayılabilecek hususlar örneğimizde ele alınmamıştır. Dolayısıyla siz kendi örneklerinizi icra ederken daha dikkatli davranmalısınız.

> Satır sayısı çok fazla olan bir içeriğin PDF'e yazılması oldukça uzun sürebilir/sürdüğü gözlemlenmiştir. Bu sebepten ilgili dosya kaydetme operasyonunun aslında asenkron bir düzenek ile icra edilmesi çok daha uygun olabilir. Hatta büyük boyutlu raporlar için bir Progress Bar ile durum bildirimi bile yapabilirsiniz
>
> ![Sarcastic smile](images/wlEmoticon-sarcasticsmile_16.png)
>
> (async ve await kullanmayı deneyiniz)

Form içeriğindeki kodlarımız ise aşağıdaki gibidir. Utility tipi pek çok ağır fonksiyonelliği kapsüllediğinden bu kısımın okunurluğu çok daha kolaydır.

```csharp
using System; 
using System.Diagnostics; 
using System.Windows.Forms;

namespace ReportApp 
{ 
    public partial class Form1 : Form 
    { 
        public Form1() 
        { 
            InitializeComponent(); 
            // Northwind veritabanında yer alan View isimleri ComboBox kontrolüne basıyoruz. 
            cmbViews.DataSource = Utility.GetNorthwindViews(); 
        }

        private void btnCreatePDF_Click(object sender, EventArgs e) 
        { 
            // Kullanıcıdan bir PDF dosya adı istiyoruz 
            if(sfdReportFile.ShowDialog()== DialogResult.OK) 
            {                
                // Dosyanın var olması halinde kullanıcının tepkisini bekliyoruz. Yes düğmesine basarsa üzerine yazacak 
               if(sfdReportFile.CheckFileExists==true) 
                        return;

                string pdfFileName = sfdReportFile.FileName; 
                // PDF oluşturma işlemini üstlenen Utility tipini çağırıyoruz. 
                Utility.CreatePDFReportFile(pdfFileName, cmbViews.SelectedValue.ToString());

                // Üretilen PDF dosyasını sistemde PDF Read edebileceğimiz bir uygulama olduğunu var sayarak Process tipi yardımıyla açtırıyoruz. 
                // Bu sayede üretilen raporu da görebiliriz. 
                Process.Start(pdfFileName);               
            } 
        } 
    } 
}
```

Dikkat edilmesi gereken noktalardan birisi de PDF dosyasının kayıt edilmesinden sonra Process.Start operasyonu ile ilgili içeriğin otomatik olarak açılmasıdır. Bu sayede sonuçları anında görebiliriz.

Çalışma Zamanı

Örneğimizi çalıştırdığımızda Select name from sys.views order by name sorgusunun bir sonucu olarak tüm View nesnelerinin elde edilebildiği görülecektir. Bu şekilde bir kullanım nedeni ile, Northwind veritabanına eklenecek olan yeni View'ları da PDF üretimi sürecine katabiliriz.

[![cpdf_3](images/cpdf_3_thumb.png)](images/cpdf_3.png)

ve bir kaç rapor örneğine ait ekran çıktısına yer vererek devam edelim.

Örnek PDF Çıktıları

Products by Category View'u için örnek ekran çıktısı

[![cpdf_5](images/cpdf_5_thumb.png)](images/cpdf_5.png)

Summary of Sales By Year View'u için örnek ekran çıktısı

[![cpdf_6](images/cpdf_6_thumb.png)](images/cpdf_6.png)

Order Subtotals View'u için örnek ekran çıktısı

[![cpdf_9](images/cpdf_9_thumb.png)](images/cpdf_9.png)

PDF içinde Chart Üretimi

Elbette PDF dosyasına çıktı olarak verilen bu raporlar arasında en etkileyici olanlarından birisi de Chart tipindekilerdir. Şimdi örnek senaryomuzda aşağıdaki görsel de yer alan ve kategori bazlı toplam satış rakamlarını gösteren View nesnesini kullanarak Line tipinde bir raporu üretmeye çalışalım.

[![pdfchart_1](images/pdfchart_1_thumb.png)](images/pdfchart_1.png)

İşte kodlarımız

```csharp
public static bool CreateChart(string fileName) 
{ 
    bool result=false;

    document = new Document(); 
    document.DefaultPageSetup.Orientation = Orientation.Landscape;

    DataTable dataContent = GetViewContent("Total Sales By Category"); 
   DrawChart(dataContent); 
    SavePdfFile(fileName);

    result = true; 
    return result; 
}

private static void DrawChart(DataTable dataTable) 
{ 
    List<double> serieValues = new List<double>(); 
    List<string> xAxisValues = new List<string>();

    Section section = document.AddSection(); 
    Chart chart = new Chart(); 
    chart.Left = 0; 
    chart.Width = Unit.FromCentimeter(24); 
    chart.Height = Unit.FromCentimeter(16); 
    Series series = chart.SeriesCollection.AddSeries(); 
    series.ChartType = ChartType.Line;

    foreach (DataRow row in dataTable.Rows) 
    { 
        serieValues.Add(Convert.ToDouble(row["Total"])); 
        xAxisValues.Add(row["CategoryName"].ToString()); 
    } 
    series.Add(serieValues.ToArray());

    XSeries xSeries = chart.XValues.AddXSeries(); 
    xSeries.Add(xAxisValues.ToArray()); 
    chart.XAxis.Title.Caption = "Kategori"; 
    chart.XAxis.HasMajorGridlines = true; 
    chart.YAxis.Title.Caption = "Toplam Satış"; 
    chart.YAxis.HasMajorGridlines = true;            
    chart.PlotArea.FillFormat.Color = Colors.SandyBrown; 
    chart.PlotArea.LineFormat.Width = 3;

    section.Add(chart); 
}
```

Baş rol oyuncusu bu kez Chart tipinden olan nesne örneğidir. Bu nesne örneği bir Section içerisinde yer almalıdır. Aynen Excel Chart nesnelerinde olduğu gibi X ve Y eksenleri ve bu eksenlere dizilmiş değerler bulunmaktadır. Dolayısıyla bu serileri veri ile doldurmak önemlidir. Bu sebepten DataTable içeriğinde dolaşılmış ve toplam satış rakamları ile kategori adları, sırasıyla XAxis ve YAxis özelliklerine ait koleksiyonlara yerleştirilmiştir. Bu fonksiyonellikleri yeni bir Button arkasında aşağıdaki kod parçasında olduğu gibi deneyebiliriz.

```csharp
private void btnCreateChart_Click(object sender, EventArgs e) 
{ 
    if (sfdReportFile.ShowDialog() == DialogResult.OK) 
    { 
        if (sfdReportFile.CheckFileExists == true) 
            return;

        string pdfFileName = sfdReportFile.FileName; 
        Utility.CreateChart(pdfFileName);

        Process.Start(pdfFileName); 
    } 
}
```

Uygulamayı çalıştırdığımızda aşağıdakine benzer bir sonuç ile karşılaşırız.

[![pdfchart_2](images/pdfchart_2_thumb.png)](images/pdfchart_2.png)

Sonuç

Sonuç olarak en azından işe yarar PDF içeriklerini kolayca üretebildiğimize şahit olduk. Örnekleri geliştirmek tamamen sizin elinizde. Söz gelimi Chart bileşenini kullandığımız senaryoyu daha da ileri götürebilir, örneğin Elma Dilimi raporları işin içersine katarak daha etkileyici çıktılar sunabilirsiniz (Üstelik bu tip görsel raporlar Jimmy Carl'ın da çok hoşuna gidecektir) Buna ilaveten bir ön iletişim kutusundan yararlanarak görsellik üzerine detay bilgilerini (örneğin tablonun arka plan rengi, font büyüklükleri, logonun gösterilip gösterilmeyeceği, bir özet bilginin konulup konulmayacağı, header veya footer kısımlarında ne yazılması istendiği vb) kullanıcıdan alabilirsiniz. Biz bu yazımızda sadece Hello World demeye çalıştık

![Winking smile](images/wlEmoticon-winkingsmile_192.png)

> PDF Sharp ve Migra kütüphanelerinin detaylı kullanımı ile ilişkili olarak [bu Wiki sayfasından](http://www.pdfsharp.net/wiki/MainPage.ashx) da yararlanabilirsiniz. Oldukça geniş kullanım örnekleri olduğunu ifade edebilirim.

Böylece geldik bir makalemizin daha sonuna. Bir diğer yazımızda görüşünce dek hepinize mutlu günler dilerim.

[HowTo_PDFSharpV2.zip (555,18 kb)](assets/HowTo_PDFSharpV2.zip)
