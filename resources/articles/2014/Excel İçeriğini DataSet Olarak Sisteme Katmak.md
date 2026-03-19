---
title: "Excel İçeriğini DataSet Olarak Sisteme Katmak"
pubDate: 2014-03-18 19:13:00
categories:
  - C#
tags:
  - C#
  - SQL Server
  - XML
  - HTTP
  - Authentication
  - JavaScript
  - DataSet
  - DataTable
---

# Excel İçeriğini DataSet Olarak Sisteme Katmak
[![production-bug](images/production-bug_thumb.jpg)](images/production-bug.jpg) Merhaba Arkadaşlar,

Bazen geliştirme ortamımız ile yazılan uygulamanın taşınacağı ortamlar arasında ciddi ve keskin farklılıklar bulunur. İki resim arasındaki 9 farkı bulunuzdan öte, geliştirici ekiplerinin bu farkları bilerek kodlama yapmasında yarar vardır. Tabi bazı yazılım ekiplerinde Development sunucularının sanallaştırılmış versiyonları üzerinde geliştirme yapabilme imkanı da vardır.

Bu tip bir yaklaşım ortak ortam standartlarının geliştirme de kullanılmasına olanak tanımaktadır. Ancak geliştiriciler uygulamanın host edileceği ortamlar hakkında fazla düşünceli davranmazsa (aynen benim gibi), özellikle kendi ortamında elinde yer alan her aracın orada sorunsuz çalışabileceğini düşünürse büyük hata yapar. İşte benim içine düştüğüm durum ve uyguladığım basit çözüm.

Vaka

Geçtiğimiz günlerde şirkette geliştirmekte olduğumuz projede şöyle bir ihtiyaç oluştu;

Yetkili kullanıcılar erişim hakkı olan bir sayfadan belirli kurallara uygun olacak şekilde bir veya daha fazla Excel dosyası yükleyecekti. Yüklenen Excel dosya içeriği yine çeşitli iş kurallarından geçirilerek denetlenecek ve uygun olan satır içeriklerinin daha sonradan işlenmek üzere veritabanına yazılması söz konusuydu. Sonrasında bu veri içeriği, sistem içerisinde takvimlendirilmiş bir iş parçacağında değerlendirilecekti.

Aslında ihtiyaç tipik anlamda bir toplu sorgulama ve işleme üzerine kuruluydu (Batch Process). Olayın taraflarından birisinin İş Birimi olduğunu düşünüldüğünde, bu şekilde bir Excel dosya içeriğinin sisteme bir noktadan kolayca sokulabilmesi son derece doğal bir ihtiyaçtı.

Hal böyle olunca ben de uygulamanın geliştiricisi olarak kolları sıvadım. İlk aklıma gelen çok doğal olarak Excel'in makinede yüklü olan Interop Assembly'larını kullanmaktı..Net Framework 4.0 sonrası gelen Optional Parameters ve dynamic gibi materyaller işimi kolaylaştırabilirdi.

Ne var ki olayın farklı bir boyutu daha vardı. Söz konusu işlevsellik ister bir servis metodun ister bir web sayfasının buton arkası kod parçası olsun, firmanın çeşitli sunucularında host edilecek bir uygulamanın içerisinde yer alacaktı.

Ben geliştirmemi tamalayıp sunucuya test için gönderdikten sonra acı gerçekle karşılaştım. Sunucu da kullandığım Microsoft.Office.Interop.Excel.dll’ i kayıtlı değildi. Yani Register ayarlarında yer almıyordu. Sunucu da doğal olarak Excel yüklü olmadığından sonuç kaçınılmazdı. Durumu düzeltmek için doğal olarak Sistem Yöneticisindeki yakın arkadaşımın kapısını çaldım. Aldığım cevap acı verici idi. “ Hocam sunuculara bu dll için gerekli kurulumları yapmıyoruz. Lisans sorunu var”

Kurumsal bir firma da sunucular için uyulması gereken pek çok kural vardır. Geliştirilen bir ürün için Development, Test, PreProduction ve Production gibi farklı ortamlar söz konusudur. Birbirleriyle bire bir aynı olması arzulanan bu ortamlara erişim kısıtlıdır. Çoğunlukla geliştiricilerin erişimine tamamen kapalıdır. Böyle bir durumda ilgili sunuculara üretilen kodların atılış şekilleri de önemlidir ki bu aslında makalemizi aşan bir konudur. Nitekim pek çok dağıtım stratejisi ve buna bağlı araç bulunmaktadır. Bizim için bu vakada önemli olan ise firma politikası ve lisanslama modeli gereği sunucuya Microsoft.Office.Interop.Excel.dll'lerinin kayıt edilememesi gerçeğidir.

Tabi geliştirici makinelerinde Excel gibi Office ürünleri yüklü olduğundan bu Interop'ların kullanılabilmesi son derece kolaydır. Ancak sunucu ortamlarında işler farklı yürür.

Uygun Çözümü Keşfetmek

İşte bu yüzden farklı bir yol bulmam gerekiyordu. En pratik olan eski stilde Excel içeriğine ulaşmaya çalışmaktı. Yani OleDbConnection sınıfını uygun provider bilgisini içeren bir bağlantı ifadesi ile tesis ederek işe başlamam gerekiyordu.

> Aslında bir Excel dosyasını okumak için kullanılabilecek bir kaç yol bulunmaktadır. Aşağıdaki şekilde görüldüğü gibi pek çok Provider söz konusudur.
> [![exltods_5](images/exltods_5_thumb.png)](images/exltods_5.png)

Bu noktada epeyce şanslı olduğumu ifade edebilirim. Nitekim sunucular üzerinde ücretsiz olarak dağıtılabilen Microsoft Access Database Engine 2010 Redistributable paketi kuruluydu. Hatta bunun 4.0 versiyonu da yer alıyordu. [Bu adresten](http://www.microsoft.com/en-us/download/details.aspx?id=13255) indirebileceğiniz paket ile Excel'in var olan tüm sürümlerine bağlantı kurulabilmekte.

Örneğin aşağıdaki ifade ile Excel 2007, 2010 ve 2013 sürümlerini açabiliriz.

Provider=Microsoft.ACE.OLEDB.12.0;Data Source=c:\GameBook.xlsx;Extended Properties="Excel 12.0 Xml;HDR=YES";

Daha önceki Excel sürümlerine göre de Provider'ı set edebiliriz. Tek yapılması gereken Extended Properties kısmında Excel 8.0 ifadesine yer vermektir. Bu sayede Excel 97, 2000, 2002 ve 2003 sürümlerine bağlantı sağlayabiliriz.

Provider=Microsoft.ACE.OLEDB.12.0;Data Source=c:\GameBook.xlsx;Extended Properties="Excel 8.0;HDR=YES";

Her iki bağlantı ifadesinde yer alan HDR ifadesi, Excel dosyası içerisindeki Sheet'ler ilk satırın başlık kolonları olduğunu ifade etmektedir. Extended Properties kısmında kullanılabilecek başka key-value değerleri de mevcuttur. Bunları araştırmanızı öneririm. Diğer yandan Excel için farklı Provider'larca kullanılabilecek bağlantı ifadeleri için [şu adrese bakabilirsiniz](http://www.connectionstrings.com/excel/).

Bağlantı Cümlesi Tamam da Sonrası

Bağlantı cümlesi hazır olduğuna göre artık kodlamaya başlayabilirdim. Burada da aslında izlenebilecek epeyce farklı yol vardı. Ben Excel içeriklerini sistem içerisinde bir DataSet olarak taşımayı düşünmüştüm. Uygulama da çeşitli tiplerin genişletilmiş fonksiyonelliklerinin tutulduğu Cross-Cutting vari bir kütüphaneye odaklandım ve DataSet tipine bir LoadFromExcel isimli bir Extension Metod ilave etmeye karar verdim.

> Takip eden örnek kod parçasında yer alan pek çok kısım kasıtlı olarak çıkartılmıştır. Çıkartılan kısımlar yazının sonundaki maddelerde belirtilen ve siz değerli okurlarıma ödev olanlardır.

Kodlama

İşte DataSet için eklediğimiz genişletme metodunun kod parçası.

```csharp
using System; 
using System.Data; 
using System.Data.OleDb;

namespace Demo 
{ 
    public static class DataSetExtensions 
    { 
        public static int LoadFromExcel(this DataSet DataSet, string FileName) 
        { 
            int sheetCount = 0; 
            string connectionString = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;\"", FileName);

            using (OleDbConnection connection = new OleDbConnection(connectionString)) 
            { 
                OleDbCommand command = new OleDbCommand(); 
                command.Connection = connection; 
                connection.Open();

                DataTable sheets = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                foreach (DataRow dr in sheets.Rows) 
                { 
                    string sheetName = dr["TABLE_NAME"].ToString(); 
                    command.CommandText = "SELECT * FROM [" + sheetName + "]"; 
                    OleDbDataAdapter adapter = new OleDbDataAdapter(command); 
                    DataTable dataTable = CreateDataTable(sheetName); 
                   adapter.Fill(dataTable); 
                    DataSet.Tables.Add(dataTable); 
                    sheetCount++; 
                } 
            } 
            return sheetCount; 
        }

        private static DataTable CreateDataTable(string SheetName) 
        { 
            DataTable newTable = new DataTable(); 
            newTable.TableName = SheetName; 
            DataColumn rowNumber = new DataColumn("RowNumber", typeof(int)); 
            rowNumber.AutoIncrement = true; 
            rowNumber.AutoIncrementSeed = 1; 
            rowNumber.AutoIncrementStep = 1; 
            newTable.Columns.Add(rowNumber); 
            return newTable; 
        } 
    } 
}
```

Kodlarda Ne Yaptım

Pek tabi genişletme metodları (Extension Methods) static olup ilk parametresinde this keyword'ünü kullanmak durumundadır. Bilindiği üzere buradaki ilk parametre genişletme metodunun uygulandığı tipi ifade etmektedir.

Metodun ilk parametresi aynı zamanda LoadFromExcel fonksiyonunu çalışma zamanında çağıracak olan nesne örneğini işaret eder. İkinci parametre ile gelen dosya adını doğru bağlantı bilgisini oluşturmak için kullanmaktayız. İlk yapılması gereken Excel içerisindeki Sheet'leri yakalamak. Bunun için OleDbConnection tipinin GetOleDbSchemaTable metodu kullanılmaktadır. OleDbSchemaGuid.Tables enum sabiti nedeniyle Sheet adlarını da barındıran bir DataTable elde edilir. Aynen aşağıdaki çalışma zamanı görüntüsünde yer aldığı gibi.

[![exltods_1](images/exltods_1_thumb.png)](images/exltods_1.png)

Kod bu Sheet'leri tek tek sorgulamak için OldDbCommand nesnesinin CommandText özelliğini değiştirmektedir. Aslında burada tipik bir SQL Select ifadesi çalıştırılır.

Select from [Oyuncular$]

gibi.

Bu ifadeyi içeren OleDbCommand nesne örnekleri bir OleDbDataAdapter'dan yararlanılarak DataTable haline getirilir. Elde edilen DataTable örnekleri ise, metoda ilk parametre olarak giren DataSet nesnesinin Tables koleksiyonuna eklenir.

Mutlaka dikkatinizi çekmiştir ki bir de CreateDataTable isimli bir fonksiyon kullanılmaktadır. Aslında bu fonksiyon doldurulacak Table'lara otomatik olarak artan bir sütun eklemekle görevlidir. Bu otomatik artan ID değerleri özellikle Table içeriklerinin görsel Web bileşenlerine bağlandığı yerlerde kritik bir rol üstlenebilir. Dahası, okunan Table içeriklerine ekstradan kolon eklenmesi gibi talepleri de bu fonksiyon içerisinde karşılayabiliriz.

Son olarak metodun geriye, eklenen DataTable sayısını döndürdüğünü belirtebiliriz. Bu bilgi object user tarafından değerlendirilebilir. Bir nevi ExecuteNonQuery metodunun işlem gören satır sayısını döndürmesi felsefesini kullanmaya çalıştık.

Testler

Uygulamayı test etmek için ben aşağıdaki içeriklere sahip basit bir Excel dosyasından yararlandım. GameBook isimli dosya Excel 2013 formatında ama diğer formatları da deneyebiliriz.

[![exltods_2](images/exltods_2_thumb.png)](images/exltods_2.png)

[![exltods_3](images/exltods_3_thumb.png)](images/exltods_3.png)

Çalışma zamanında DataSet içeriğini Visualizer ile incelediğimizde aşağıdaki gibi Excel Sheet içeriklerinin yüklendiğini görebiliriz.

[![exltods_4](images/exltods_4_thumb.png)](images/exltods_4.png)

Peki Ya Yapmadıklarımız

Önceden de belirttiğim üzere bu gerçek hayat senaryosunda ki bazı maddeler çıkartılmış durumdadır. Aşağıdakileri yapmaya çalışarak kendinizi bu konuda geliştirmeye devam edebilirsiniz.

- Her şeyden önce kullanıcı aslında Excel tipinde olmayan bir dosyayı da yüklemeye çalışabilir. Ya da sistemin aslında ele almaması gereken sınır bir dosya boyutunun üzerinde olabilir. Bunun önüne geçmek için elbette arayüz tarafında bir takım tedbirler almak daha mantıklıdır. Örneğin Javascript ile. Fakat fonksiyonun genel bir Extension olduğunu düşünecek olursak, bir şekilde bu tip doğrulama stratejilerini öğrenebilmesi güzel olabilir. Belki Attribute’ lar ile dekore edilebilir.
- Extension Method’ un ayrı bir Library içerisine alınması sağlanabilir. Bu örnekte sadece metodun yazımını ve basit kullanımını inceledik. Pek tabi Enterprise bir çözümde bu tip fonksiyonelliklerin farklı bir kütüphanede konuşlandırılması daha doğrudur.
- Excel içeriklerinin çeşitli doğrulama (Validation) stratejilerine göre taranabilmesi iş birimi için önemli olabilir. Mutlaka okunan Excel içeriğinde insan hatası içerikler yer alabilir. Özellikle dosya şema yapısı işlenirken sorun çıkartabilecek formatta bulunabilir. Bu tip olası şema ve veri hatalarının denetlenmesi uygun olacaktır. Nitekim ilgili doğrulama kriterlerine yenileri de eklenebilir. Bu yeni kriterlerin ilgili fonskiyonellik içerisine de kolayca ve zahmetsizce (kod açmaya gerek duymadan) enjekte edilmesi önemlidir. Yani DataSet çalışma zamanında bir strateji benimseyebilmeli ve hatta davranış değiştirebilmelidir.
- Metodun farklı Excel Provider’ ları ile çalışacak şekilde dekore edilmesi gerekebilir. Firmanın farklı kullanıcılarının farklı Excel sürümleri ile çalışmak istemesi bir yana, ürünün dağıtılacağı sunucuların Provider kısıtları olabilir. Buna dikkat ederek bir geliştirme yapmak faydalı olabilir. Her ne kadar bu tip bir Container yazmak zor görünsede IoC araçlarına bir bakılabilir.
- Exception Handling mekanizması konulamalıdır. Testler de Provider'ın nadiren de olsa exception verdiği durumlar ile karşılaştığımı ifade edebilirim. Bu genellikle Provider'ın kullandığı dll'den kaynaklansa da ele alınması ve object user tarafna uygun bir bilgilendirme yapılması gerekmektedir. Aslında ödevlere başlanabilecek en basit madde sanıyorum ki budur.

Sonuç

Sonuç olarak bir Excel içeriğini sisteme almanın birden fazla yolu olduğunu ifade edebiliriz. Ancak bunu etkileyen bazı faktörler olduğunu da bilmeliyiz. En önemlisi de içeriğin alınacağı ortama ait sistemsel parametrelerdir. Çok doğal olarak bu tip bir senaryoda SSIS paketlerinin kullanılmasını da düşünebilirsiniz. Hatta daha önceden çalıştığım bir finans kurumunda tam da bu işler için SSIS (SQL Server Integration Services) paketleri hazırlamakta olduğumu ifade edebilirim. Ama işte gün geliyor SSIS paketi yazacak ortam veya ürün bulamayabiliyorsunuz. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
