---
layout: post
title: "Excel ve Entity Framework Konuşuyor"
date: 2013-06-30 17:57:00 +0300
categories:
  - office-development
tags:
  - office-development
  - xml
  - csharp
  - dotnet
  - entity-framework
  - linq
  - windows-forms
  - async-await
  - performance
  - visual-studio
---
Artık uygulamaların birbirleri ile konuşmaları çok ama çok kolay. Bu gerçekten önemli bir mesele. Özellikle farklı segmentlerden insanların bir araya geldiği bilişim toplumlarında. Kimi kullanıcı için Office Excel, Word veya Powerpoint çok şey ifade ederken, kimi kullanıcı içinde SQL Management Studio ortamında hazırlanan karmaşık bir sorguya bakmak daha anlamlı olabiliyor. Ya da bir Web sayfası üzerinden alınan raporlar şirketin Muhasebe Şefi için değerli iken, kimisi SSRS ile elde edilen raporları mobil cihazında görmeyi tercih edebiliyor.

[![handshake](/assets/images/2013/handshake_thumb.jpg)](/assets/images/2013/handshake.jpg)

Ancak Developer gözüyle olaya bakıldığında, her segmenti memnun edecek şekilde geliştirme yapması beklendiği oldukça aşikar. Bu sebepten, farklı uygulamaların birbirleriyle rahatça konuşabilmeleri önemli bir mesele olarak karşımıza çıkıyor. Visual Studio 2012 tarafında olaya baktığımızda bir Office uygulamasının, önceki sürümlere göre.Net Framework ile daha yüksek seviyede etkileşime girerek tasarlanabilmesi/geliştirilebilmesi de pekala mümkün.

Özellikle Sheet’ ler veya Workbook’ lar kodlanabilir birer C#(Vb.Net) dosyası olduğu için, uygulama bazında istediğimiz taklayı atma şansına sahibiz. İşte bu düşünceler ışığında yola çıktığımız ve okumakta olduğunuz yazımızda, Excel’ i, Entity Framework’ ü, C#’ ı işin içerisine katacak ve birbirleri ile konuşmalarını sağlamaya çalışacağız. Haydi hiç vakit kaybetmeden yola koyulalım. Ama önce örnek senaryomuz

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_193.png)

Senaryo

Amacımız, Northwind veritabanında bulunan 3 adet View nesne örneğinin Excel dokümanı içerisindeki Sheet’ ler de gösterilmesini sağlamak. Bu amaçla Category Sales for 1997, Product Sales for 1997 ve Invoices isimli View nesnelerini kullanıyor olacağız. Sheet1 içeriğini kendi kod dosyası içerisinde üretmek istiyoruz. Sheet2 ve Sheet3 içeriklerini ise, Workbook’ a ait kod dosyasından besliyor olacağız. Bu noktada ağırlıklı olarak Workbook ve Sheet sınıflarının ilgili başlangıç noktalarını değerlendireceğiz. Kabaca senaryomuzu işaret eden aşağıdaki görseli göz önüne alabiliriz.

[![ewef_8](/assets/images/2013/ewef_8_thumb.png)](/assets/images/2013/ewef_8.png)

Konuşan Çözüm

Dilerseniz senaryo içerisinde tarafların gözünden olaya bakalım.

## Sheet Konuşuyor

"Merhaba, benim adım Sheet1. Çalıştırıldığımda Sheet1Startup olay metoduna bir çağrıda bulunurum. Ey Startup metodu, haydi işini yap derim. O da kendi içinde Entity Framework tabanlı Context nesnesini kullanır ve 1997 yılına ait kategori bazlı satışların verilerini, sahip olduğum hücrelere teker teker aktarır

![Smile](/assets/images/2013/wlEmoticon-smile_93.png)

Onunla çok iyi anlaşırız.

## Workbook Konuşuyor

Merhaba, ben ThisWorkbook. Ben sahip olduğum tüm Sheet’ leri yönetebilirim. Örneğin kendi Startup metoduma, tüm Sheet’ leri çeşitli yerlerden topladığı veriler ile doldurmasını söyleyebilirim. He-Man ile aramdaki tek fark onun kılıcının olmasıdır.

Hazırlıklar

Dilerseniz senaryomuzu nihai sonuca ulaştırmak için adım adım ilerlemeye başlayalım. İlk olarak Visual Studio 2012 ortamında bir Excel 2010 Workbook projesi oluşturmalıyız. Bu nedenle New Project kısmında ilgili proje tipini işaretliyoruz.

[![ewef_7](/assets/images/2013/ewef_7_thumb.png)](/assets/images/2013/ewef_7.png)

Bu işlemin ardından Northwind veritabanına ait Entity içeriğini barındıracak bir Class Library projesini aynı Solution içerisine ekleyebilir ve aşağıdaki gibi mevzuya konu olan View nesnelerini içeren Entity Data Model öğesini üretebiliriz.

[![ewef_1](/assets/images/2013/ewef_1_thumb.png)](/assets/images/2013/ewef_1.png)

Çok doğal olarak Excel uygulamasının, söz konusu Class Library’ yi ve ayrıca EntityFramework Assembly’ ını referans etmesi gerekmektedir. Solution içeriği şu an itibariyle aşağıdaki gibi olacaktır.

[![ewef_2](/assets/images/2013/ewef_2_thumb.png)](/assets/images/2013/ewef_2.png)

Dikkat edileceği üzere HowToExcelWithEF uygulaması içerisinde Workbook ve söz konusu Workbook’ a ait Sheet’ ler için birer kod dosyası bulunmaktadır. Bu kod dosyaları içerisindeki pek çok olay metoduna müdahale edebilir ve çalışma zamanını kontrol altına alabiliriz ki örneğimizde de benzer bir işi gerçekleştiriyor olacağız.

Kritik noktalardan birisi de, Entity kütüphanesini kullanacak çalıştırılabilir uygulamanın, Connection String bilgisine sahip olması zorunluluğudur. Bu nedenle NorthwindLibrary içerisindeki App.Config dosyasına ait Connection String bilgisini, HowToExcelWithEF uygulamasında da kullanmalıyız.

```xml
<?xml version="1.0" encoding="utf-8" ?> 
<configuration> 
  <connectionStrings> 
    <add name="NorthwindEntities" connectionString="metadata=res://*/NorthwindModel.csdl|res://*/NorthwindModel.ssdl| res://*/NorthwindModel.msl;provider=System.Data.SqlClient;provider connection string="data source=.;initial catalog=Northwind;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework"" providerName="System.Data.EntityClient" /> 
  </connectionStrings> 
</configuration>
```

Artık kodlama tarafını tamamlayabiliriz. İlk olarak Sheet1.cs içeriğini aşağıdaki gibi geliştirdiğimizi düşünelim.

```csharp
using NorthwindLibrary; 
using System.Linq;

namespace HowTo_ExcelWithEF 
{ 
    public partial class Sheet1 
    { 
        private void Sheet1_Startup(object sender, System.EventArgs e) 
        { 
            using(NorthwindEntities context=new NorthwindEntities()) 
            { 
                var categoryBasedSales = from s in context.Category_Sales_for_1997 
                            orderby s.CategorySales descending 
                            select new { 
                                s.CategoryName, s.CategorySales 
                            };

                int rowIndex = 1; 
                
                Cells[rowIndex, 1] = "Kategori"; 
                Cells[rowIndex, 2]= "Satışlar";

                Cells[rowIndex, 1].Font.Bold = true; 
                Cells[rowIndex, 2].Font.Bold = true;

                foreach (var sale in categoryBasedSales) 
                { 
                    rowIndex++; 
                    Cells[rowIndex, 1]= sale.CategoryName; 
                    Cells[rowIndex, 2]= sale.CategorySales; 
                } 
            } 
        }

        private void Sheet1_Shutdown(object sender, System.EventArgs e) 
        { 
        }

        #region VSTO Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor. 
        /// </summary> 
        private void InternalStartup() 
        { 
            this.Startup += new System.EventHandler(Sheet1_Startup); 
            this.Shutdown += new System.EventHandler(Sheet1_Shutdown); 
        }

        #endregion

    } 
}
```

Startup metodunda context nesne örneği kullanılarak ilgili View’ dan Anonymous tipte bir nesne listesi oluşturulmaktadır. Söz konusu liste Sheet1 içerisindeki hücrelere yerleştirilir. Bunun için Cells[rowIndex,columnIndex] ifadesi ele alınmaktadır. Kodu yazarken dikkat edilecek olursa dynamic özelliğinin kullanılmakta olduğu görülebilir. Yani noktadan sonrasındaki atamalar ve bunlara ait tipler çalışma zamanında çözümlenecektir. Bu biraz performans kaybına yol açacak olsa da şu anki senaryodaki gibi küçük veri kümelerinde önemsizdir.

[![ewef_9](/assets/images/2013/ewef_9_thumb.png)](/assets/images/2013/ewef_9.png)

1,1 ve 1,2 hücrelerini başlıklar ile doldurduktan sonra (Excel tarafında ilk hücre satır sütun değeri 0,0 değil 1,1 dir) bunların Bold olması sağlanmıştır. Diğer yandan sonuç listesi üzerinde dönülmekte ve diğer hücreler, View dan gelen veriler ile beslenmektedir. Bu kodlamaya göre veri Sheet1’ in kod dosyası da doldurulmaktadır. Pek tabi Sheet’ lerin Workbook’ a ait olay metodlarında şekillendirilmesi de söz konusudur. Sheet2 ve Sheet3 bu amaçla Workbook.cs içerisinde ele alınmıştır. Aynen aşağıdaki kod parçasında görüldüğü gibi.

```csharp
using NorthwindLibrary; 
using System.Drawing; 
using System.Linq;

namespace HowTo_ExcelWithEF 
{ 
    public partial class ThisWorkbook 
    { 
        private void ThisWorkbook_Startup(object sender, System.EventArgs e) 
        { 
            using(NorthwindEntities context=new NorthwindEntities()) 
            { 
                #region Entity View içeriklerinin çekilmesi

                var invoices = from i in context.Invoices 
                               orderby i.CustomerName 
                               select new 
                              { 
                                   From = i.Country + "," + i.City, 
                                   i.CustomerID, 
                                   i.CustomerName, 
                                   i.OrderDate, 
                                   ShipTo = i.ShipCountry + "," + i.ShipCity 
                               };

                var prdoductSalesFor1997 = from s in context.Product_Sales_for_1997 
                                           orderby s.ProductSales descending 
                                           ,s.CategoryName ascending 
                                           ,s.ProductName ascending 
                                           select new 
                                           { 
                                               Product=s.CategoryName+"-"+s.ProductName, 
                                               s.ProductSales 
                                           };

                #endregion

                #region Sheet2 nin Invoices içeriği ile doldurulması

                int rowIndex = 1;

                Sheets["sheet2"].Cells[rowIndex, 1] = "From"; 
                Sheets["sheet2"].Cells[rowIndex, 1].Font.Bold = true; 
                Sheets["sheet2"].Cells[rowIndex, 2] = "Customer ID"; 
                Sheets["sheet2"].Cells[rowIndex, 2].Font.Bold = true; 
                Sheets["sheet2"].Cells[rowIndex, 3] = "Customer"; 
                Sheets["sheet2"].Cells[rowIndex, 3].Font.Bold = true; 
                Sheets["sheet2"].Cells[rowIndex, 4] = "Order Date"; 
                Sheets["sheet2"].Cells[rowIndex, 4].Font.Bold = true; 
                Sheets["sheet2"].Cells[rowIndex, 5] = "Ship To"; 
                Sheets["sheet2"].Cells[rowIndex, 5].Font.Bold = true;

                foreach (var invoice in invoices) 
                { 
                    rowIndex++; 
                    Sheets["sheet2"].Cells[rowIndex, 1] = invoice.From; 
                    Sheets["sheet2"].Cells[rowIndex, 2] = invoice.CustomerID; 
                    Sheets["sheet2"].Cells[rowIndex, 3] = invoice.CustomerName; 
                    Sheets["sheet2"].Cells[rowIndex, 4] = invoice.OrderDate; 
                    Sheets["sheet2"].Cells[rowIndex, 5] = invoice.ShipTo; 
                }

                #endregion Sheet2 nin Invoices içeriği ile doldurulması

                #region Sheet3 ün 1997 yılına ait satış verileri ile doldurulması

                rowIndex = 1;

                Sheets["sheet3"].Cells[rowIndex, 1] = "Product"; 
                Sheets["sheet3"].Cells[rowIndex, 1].Font.Bold = true; 
                Sheets["sheet3"].Cells[rowIndex, 2] = "Sales"; 
                Sheets["sheet3"].Cells[rowIndex, 2].Font.Bold = true;

                foreach (var sales in prdoductSalesFor1997) 
                { 
                    rowIndex++; 
                    Sheets["sheet3"].Cells[rowIndex, 1] = sales.Product; 
                    if (sales.ProductSales > 30000) 
                    { 
                        Sheets["sheet3"].Cells[rowIndex, 2].Interior.Color = Color.Black; 
                        Sheets["sheet3"].Cells[rowIndex, 1].Interior.Color = Color.Black; 
                        Sheets["sheet3"].Cells[rowIndex, 2].Font.Color = Color.Gold; 
                        Sheets["sheet3"].Cells[rowIndex, 1].Font.Color = Color.Gold; 
                    }

                    Sheets["sheet3"].Cells[rowIndex, 2] = sales.ProductSales; 
                }

                #endregion Sheet3 ün 1997 yılına ait satış verileri ile doldurulması 
            }         
        }

        private void ThisWorkbook_Shutdown(object sender, System.EventArgs e) 
        { 
        }

        #region VSTO Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor. 
        /// </summary> 
        private void InternalStartup() 
        { 
            this.Startup += new System.EventHandler(ThisWorkbook_Startup); 
            this.Shutdown += new System.EventHandler(ThisWorkbook_Shutdown); 
        }

        #endregion

    } 
}
```

> Office uygulamalarını geliştirirken C# 4.0 ile birlikte gelen dynamic, Optional and Named Parameters gibi yeniliklerin işlerimizi nasıl kolaylaştırdığına bir kere daha şahit oluyoruz.

Kodlarda bizi zorlayacak bir karmaşa yoktur. İki View sırasıyla sorgulanmakta ve elde edilen listeler ilgili Sheet’ ler deki hücrelere satır satır doldurulmaktadır. Özellikle 1997 yılına ait satış rakamları alınırken 30000 birimden büyük olunması halinde, o satırın arka plan ve font renkleri değiştirilerek göze batmaları sağlanmaktadır

![Smile](/assets/images/2013/wlEmoticon-smile_93.png)

Dikkate değer noktalar, ilgili hücrelerin nasıl formatlandığı veya belirli bir Sheet’ e nasıl ulaşıldığı ile alakalıdır.

Sonuçlar

Uygulamayı çalıştırdığımızda çalışma zamanında bir Excel Workbook’ un açıldığını ve Sheet1, Sheet2, Sheet3 sayfalarının ilgili View içerikleri ile doldurulduğunu görebiliriz.

Sheet1 içeriği kategori bazlı satış verilerl ile

[![ewef_3](/assets/images/2013/ewef_3_thumb.png)](/assets/images/2013/ewef_3.png)

Sheet 2 içeriği Müşteri faturalarıyla,

[![ewef_4](/assets/images/2013/ewef_4_thumb.png)](/assets/images/2013/ewef_4.png)

son olarak Sheet3 içeriği de ürün bazlı satış rakamlarıyla doldurulacaktır.

[![ewef_5](/assets/images/2013/ewef_5_thumb.png)](/assets/images/2013/ewef_5.png)

Görüldüğü üzere bir Excel içeriğini Entity Framework üzerinden geçerek veri ile doldurmak son derece kolaydır. Elbette gerçek hayat senaryoları düşünüldüğünde, ilgili veri içeriğinin asenkron olarak ve hatta servisler yardımıyla doldurulması daha doğru bir yaklaşım olacaktır. Zaten Excel’ in konuşabildiği pek çok dış dünya aracı servis bazlı veri sunmaktadır (OData servislerinden veri çekilmesi ve Excel içerisinde Pivot table olarak gösterilebilmesi konusunu araştırabilirsiniz)

Tamamlanan uygulamanın herhangibir bilgisayarda veya ortamda çalıştırılabilmesi için aşağıdaki görselde yer alan içeriğin taşınması yeterli olacaktır. Elbette veritabanı bağlantısının olduğunu ve ilgili Northwind içeriklerine ulaşılabildiğini varsayıyoruz.

[![ewef_6](/assets/images/2013/ewef_6_thumb.png)](/assets/images/2013/ewef_6.png)

Böylece geldik bir yazımızın daha sonuna. Size düşen örneği daha da zengilenştirmektir. Örneğin,

- n sayıda Sheet’ in doldurulması noktasında asenkron bir işleyişin ele alınmasını sağlayabilirsiniz.
- Aynı uygulamayı Office 2013 standartlarında ele alıp,.Net Framework 4.5 hedefli düşünüp, async ve await anahtar kelimelerini devreye sokabilirsiniz.
- Excel’ de yapılacak olan değişikliklerin, Save işlemleri sonrasında veri kaynağına doğru yansıtılmasını düşünebilirsiniz.
- Bazı View’ lardan yararlanarak Sheet içerisine çok basit anlamda Chart (örneğin bir Pie Chart çok şık durabilir) çizdirmeyi deneyebilirsiniz

vb

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_193.png)

Burada hayal gücünüzü kullanmadan önce, bir Excel uygulamasının aslında bir Windows uygulaması olduğunu ve Visual Studio 2012 tarafında istenildiği gibi özelleştirilebildiğini düşünmenizde yarar olacaktır. Nitekim gördüğünüz üzere aynen bir Windows Forms uygulamasında olduğu gibi olay metodlarına girebiliyor ve C# kodlarımızı konuşturabiliyoruz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[HowTo_ExcelWithEF.zip (1,20 mb)](/assets/files/2013/HowTo_ExcelWithEF.zip)

[Orjinal Yazım Tarihi 03-05-2013]