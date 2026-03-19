---
title: "SqlCommandBuilder için 4 Tavsiye"
pubDate: 2006-11-13 12:00:00
categories:
  - Ado.Net
tags:
  - Ado.Net
  - C#
  - SQL Server
  - HTTP
  - DataTable
---

# SqlCommandBuilder için 4 Tavsiye
Değerli Okurlarım Merhabalar,

SqlCommandBuilder sınıfı özellikle bağlantısız katman (disconnected layer) modelinde sıkça kullanılmaktadır. Çoğunlukla, SqlDataAdapter tipine ait nesneler için gerekli olan UpdateCommand, InsertCommand ve DeleteCommand özelliklerine bağlı SqlCommand nesnelerini sıfırdan oluşturmamak için tercih edilebilir. Framework 1.1' de özellikle bağlantısız katman modeline ait bir vakka olan Concurency Violation durumlarındaki yaklaşımı nedeniyle (tüm kolonları where'e dahil etmek) bazen tercih edilmemektedir.

Ancak SqlCommandBuilder, Framework 2.0 ile birlikte kendisine eklenen yeni üyeler sayesinde daha da fonksiyonel hale gelmiştir. Bununla birlikte SqlCommandBuilder tipinin sadece bağlantısız katman (disconnected layer) modeli için yazılmış olduğunu düşünmek haksızlık olacaktır. Nitekim katmanlı mimaride veri erişim katmanı (data access layer) içerisinde son derece kullanışlı olabilecek bir metodada sahiptir. İşte bu makalemizde SqlCommandBuilder tipinin,.Net Framework 2.0 versiyonu ile birlikte güçlendirilmiş olan yönlendiren bahsetmeye çalışacağız. Temel olarak işleyeceğimiz maddeler aşağıdaki gibidir.

- Stored Procedure'lerin parametrik yapısını çalışma zamanında bir SqlCommand nesnesine aktarabilme
- Çakışma durumları için uygun olan yöntemi ConflictOption özelliği ile belirleyebilme
- Update işlemleri sırasında sadece güncellenen parametreleri sql sunucusuna gönderebilme
- SqlDataAdapter için üretilen sql komutlarında kolon adı kullanılmasını tercih edebilme

Şimdi burada bahsettiğimiz özellikleri incelemeye çalışalım. Ancak maddelerimizi incelemeden önce senaryo olarak kullanacağımız tablomuzu aşağıdaki sql script yardımıyla oluşturalım. Personel isimli tablomuz Sql Server 2005 üzerinde AdventureWorks veritabanı altında yer alacaktır. Ancak örneklerimiz için dilerseniz siz kendi tablolarınızıda kullanabilirsiniz.

```text
USE [AdventureWorks]
GO

CREATE TABLE [dbo].[Personel](
    [PersonelId] [int] IDENTITY(1,1) NOT NULL,
    [Adi] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    [Soyadi] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    [Maasi] [money] NOT NULL,
    [IseGirisTarihi] [datetime] NOT NULL,
    [Departmani] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    [Durum] [timestamp] NOT NULL,
    CONSTRAINT [PK_Personel] PRIMARY KEY CLUSTERED 
    (
        [PersonelId] ASC
    )WITH (PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
```

1. Stored Procedure'lerin parametrik yapısını çalışma zamanında bir SqlCommand nesnesine aktarabilme

Öyle bir metod düşünelim ki, sadece çalıştıracağı saklı yordamın (stored procedure) adını, ve sayısını bilmemesine rağmen parametrelerinin değerlerini alsın. Sonrada işaret ettiği bu saklı yordamı yürütsün. Bu tam anlamıyla veri erişim katmanlarında kullanılabilecek bir metod tipidir. Bir saklı yordamı çalıştırırken eğer aldığı giriş parametreleri (input parameter) varsa bunları mutlaka ilgili SqlCommand nesnesinin Parameters koleksiyonuna aynı adlarda olmak şartıyla eklememiz gerekmektedir. Şimdi ilk olarak varsayılan haliyle böyle bir işi nasıl yapacağımızı düşünmeye çalışalım. Bu amaçla Personel tablomuza satır ekleyen aşağıdaki gibi bir saklı yordamımız olduğunu varsayalım.

```text
CREATE PROCEDURE dbo.PersonelEkle 
(
    @Adi nvarchar(50)
    ,@Soyadi nvarchar(50)
    ,@Maasi money
    ,@IseGirisTarihi datetime
    ,@Departmani nvarchar(50)
)
AS
    Insert into Personel (Adi,Soyadi,Maasi,IseGirisTarihi,Departmani)
    Values (@Adi,@Soyadi,@Maasi,@IseGirisTarihi,@Departmani)
RETURN
```

Saklı yordamımız beş adet input parametresi almaktadır. Bu sp'yi çalıştıracak olan bir SqlCommand nesnesi temel olarak aşağıdaki kodda görüldüğü gibi kullanılacaktır. Dikkat ederseniz saklı yordamımız içerisindeki tüm parametreler cmd isimli SqlCommand nesne örneğinin Parameters koleksiyonuna eklenmekte, eklenirkende metoda gelen değerlerini almaktadırlar.

```csharp
class Komutlar
{
private string m_ConStr;

    public Komutlar(string conStr)
    {
        m_ConStr = conStr;
    }
    public void PersonelEkle(string ad,string soyad,decimal maas,DateTime iseGiris,string departman)
    {
        using (SqlConnection con = new SqlConnection(m_ConStr))
        {
            SqlCommand cmd = new SqlCommand("PersonelEkle", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Adi",ad);
            cmd.Parameters.AddWithValue("@Soyadi",soyad);
            cmd.Parameters.AddWithValue("@Maasi",maas);
            cmd.Parameters.AddWithValue("@IseGirisTarihi",iseGiris);
            cmd.Parameters.AddWithValue("@Departmani",departman);
            con.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
class Program
{
    static void Main(string[] args)
    {
        Komutlar kmt = new Komutlar("data source=localhost;database=AdventureWorks;integrated security=SSPI");
        kmt.PersonelEkle("Hey", "Mayk", 1000, new DateTime(2001, 1, 1), "Spor Arabalar");
    }
}
```

Bu kod başarılı bir şekilde çalışacaktır. Ancak dikkat ederseniz PersonelEkle metodu sadece PersonelEkle saklı yordamını çalıştırabilir. Dahası, metodun içerisindeki SqlCommand nesne örneğine ait parametre değerleri, aslında metoda gelen parametrelerin değerleridir. Dolayısıla parametrelerin adları hatta tipleri değişebilir. Bu tarz durumlarda sürekli olarak kod üzerinde düzenlemeler yapmamız ve yeniden derlememiz yada başka bir yol düşünmemiz gerekecektir. Oysaki aşağıdaki metod tam anlamıyla her hangibir Sql saklı yordamının her hangi sayıda parametresine hizmet edebilecek niteliktedir.

```csharp
public void Execute(string spAdi, params object[] degerler)
{
    using (SqlConnection conn = new SqlConnection(m_ConStr))
    {
        using (SqlCommand cmd = new SqlCommand(spAdi, conn))
        {
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            conn.Open();
            // DeriveParameters sadece Sp' lerde işe yarayan bir tekniktir.
            SqlCommandBuilder.DeriveParameters(cmd);
            for (int i = 1; i <= degerler.Length; i++)
                cmd.Parameters[i].Value = degerler[i-1];
            cmd.ExecuteNonQuery();
        }
    } 
}
```

Metodumuz ilk parametre olarak çalıştıracağı saklı yordamın adını alır. Daha sonra ise params anahtar sözcüğünden faydalanarak n sayıda, object tipinden elemanlar taşıyan bir diziyi parametre almaktadır. Metodumuz içerisinde yer alan SqlCommandBuilder sınıfının static DeriveParameters metodu ise, parametre olarak aldığı SqlCommand nesnesinin işaret ettiği saklı yordama gider, bu yordamın parametrelerini (ilk parametresi @RETURN_VALUE olacak şekilde) alır ve ilgili komut nesnesinin Parameters koleksiyonuna ekler. Eğer uygulamayı debug ederseniz aşağıdaki ekran görüntüsünde olduğu gibi saklı yordam parametrelerinin cmd nesnesine eklendiğini görebilirsiniz.

![mk180_1.gif](images/mk180_1.gif)

Execute metodumuzun bu versiyonunu aşağıdaki gibi test edebiliriz.

```csharp
Komutlar kmt = new Komutlar("data source=localhost;database=AdventureWorks;integrated security=SSPI");
kmt.Execute("PersonelEkle","Hey", "Mayk", 1000, new DateTime(2001, 1, 1), "Spor Arabalar");
```

Peki DeriveParameters metodu aslında ne yapmaktadır. Sql Server Profiler yardımıyla bu uygulmanın arka planda çalıştırdığı sql kodlarını incelersek aşağıdaki gibi bir çağrı ile karşılaşırız.

![mk180_11.gif](images/mk180_11.gif)

Dikkat ederseniz spprocedureparamsmanaged isimli bir saklı yordam, parametrelerini elde etmek istediğimi saklı yordamın adını parametre alarak çalıştırılmaktadır. Bu ifadeyi Sql Server 2005 Management Studio üzerinde çalıştırırsak, aşağıdaki gibi bir çıktı elde ederiz.

![mk180_12.gif](images/mk180_12.gif)

Sistem sp'lerinden olan spprocedureparamsmanaged aslında PersonelEkle isimli saklı yordam içerisindeki tüm parametreleri ve bu parametrelere ait detaylı bilgileri bir tablo olarak geriye döndürmektedir. Bu tabloda parametrelerin adlarından tutunda veri tiplerine kadar, null değer içerip içermekyeceklerinden taşıyacakları veri uzunluğuna kadar tüm bilgiler yer almaktadır. Bu tabloyu değerlendiren elbette SqlCommandBuilder nesnesinin kendisidir. Tablo içerisindeki bilgilere göre ilgili SqlCommand nesnesinin parameters koleksiyonuna gerekli eklemeler yapılır.

![dikkat.gif](images/dikkat.gif)
OleDbCommandBuilder, OracleCommandBuilder, ODBCCommandBuilder sınıflarıda DeriveParameters metodunu destekler. Tek şart, ilgili veri tabanı sisteminin saklı yordama ait parametre yapısını getirebiliyor olmasıdır. Unutmayalım; DeriveParameters sadece saklı yordamlar için geçerlidir. Düz Sql sorgu cümlelerini ele alan komutlar için (parametrik bile olsalar) InvalidOperationException istisnası döndürmektedir.

2. Çakışma durumları için uygun olan yöntemi ConflictOption özelliği ile belirleyebilme

Bağlantısız katman nesneleri ile çalışırken, başımızı en çok ağrıtan konulardan biriside, birbirlerinden habersiz olarak bir den fazla kullanıcının aynı veri üzerinde değişiklik yapmasıdır. Böyle bir durumda son güncelleme kazansın (Last Wins) tekniğini tercih edebilir yada DbConcurrencyViolation istisnasını ele alabiliriz. (Konu hakkında detaylı bilgi için [tıklayın](http://www.bsenyurt.com/MakaleGoster.aspx?ID=112).) Hangi tekniği seçersek, Update ve Delete sorgularının where koşullarında değişiklik olacaktır.

SqlCommandBuilder sınıfına yeni katılan ConflictOptions özelliği ile üretilen komutların bizim seçeceğimiz çakışma kuralına göre oluşturulması sağlanabilir. Last Wins tekniği için Where koşuluna, o tabloda yer alan Primary Key alanın orjinal değeri, DBConcurrencyViolation durumda ise tüm alanların orjinal değerleri yada Id alanı ile birlikte varsa TimeStamp gibi alanların orjinal değeleri hesaba katılacaktır. Şimdi bu durumu analiz edeceğimiz örnek bir Windows uygulamasını aşağıdaki gibi geliştirelim.

```csharp
public partial class Form1 : Form
{
    private SqlConnection conn;
    private SqlDataAdapter daPersonel;
    private DataTable dtPersonel;

    public Form1()
    {
        InitializeComponent();
    }
    // DataAdapter hazırlanır.
    private void PrepareDataAdapter()
    {
        conn = new SqlConnection("data source=localhost;database=AdventureWorks;integrated security=SSPI");
        daPersonel = new SqlDataAdapter("Select PersonelId,Adi,Soyadi,Maasi,IseGirisTarihi,Departmani,Durum From Personel", conn);         
    }
    /* SqlCommandBuilder hazırlanır ve Update, Delete, Insert komutlarını hazırlaması sağlanır. Oluşan komutlar bilgi amacıyla StringBuilder kullanılarak bir RichTextBox kontrolüne yazılır. */
    private void PrepareCommands()
    {
        SqlCommandBuilder cmb = new SqlCommandBuilder(daPersonel);
        ConflictOption selectedOption = (ConflictOption)Enum.Parse(typeof(ConflictOption), cmbConflictOptions.SelectedItem.ToString()); 
        cmb.ConflictOption = selectedOption; // Çakışma seçeneği belirlenir.
        daPersonel.InsertCommand = cmb.GetInsertCommand();
        daPersonel.UpdateCommand = cmb.GetUpdateCommand();
        daPersonel.DeleteCommand = cmb.GetDeleteCommand();

        StringBuilder builder = new StringBuilder();
        builder.Append("Insert Command : \n");
        builder.Append(daPersonel.InsertCommand.CommandText + "\n");
        builder.Append("Update Command : \n");
        builder.Append(daPersonel.UpdateCommand.CommandText + "\n");
        builder.Append("Delete Command : \n");
        builder.Append(daPersonel.DeleteCommand.CommandText + "\n");
        txtCommands.Text = "";
        txtCommands.Text = builder.ToString(); 
    }
    /* Select düğmesine basıldığında dataTable oluşturulur, SqlDataAdapter tarafından doldurulur ve DataGridView kontrolüne veri kaynağı olarak bağlanır. */
    private void btnSelect_Click(object sender, EventArgs e)
    {
        dtPersonel = new DataTable();
        daPersonel.Fill(dtPersonel);
        grdPersonel.DataSource = dtPersonel;
        grdPersonel.Columns.Remove("Durum"); // TimeStamp tipi alanlar DataGridView' da problem çıkarttığı için çıkartıldı.
    }
    /* Form yüklenirken DataAdapter hazırlanır, ConflictOption için enum sabiti üzerinden alınan değerleri ComboBox' a dolduran PrepareConflictOption metodu çağırılır.*/
    private void Form1_Load(object sender, EventArgs e)
    {
        PrepareDataAdapter();
        PrepareConflictOptions();
        cmbConflictOptions.SelectedIndex = 0;
    }
    // ConflictOption enum sabiti içerisindeki değerleri ComboBox kontrolüne doldurur.
    private void PrepareConflictOptions()
    {
        string[] conflictOptions = Enum.GetNames(typeof(ConflictOption));
        foreach (string option in conflictOptions)
            cmbConflictOptions.Items.Add(option);
    }
    /* DataTable üzerindeki değişiklikeri asıl veri kaynağına yazmadan önce SqlCommandBuilder oluşturulduğu ve Delete, Update, Insert komutlarının hazırlandığı PrepareCommands metodunu çağırır. */
    private void btnUpdate_Click(object sender, EventArgs e)
    {
        PrepareCommands();
        daPersonel.Update(dtPersonel);
    }
}
```

Kod tarafında bizim için önemli olan kısım PrepareCommand metodu içerisinde yaptıklarımızdır. Bu metod içerisinde SqlDataAdapter nesnesi için gerekli Update,Delete ve Insert komutlarını oluşturmaktayız. Çakışma seçeneğini dikkat ederseniz ConflictOption enum sabiti üzerinden almaktayız. Uygulamada SqlDataAdapter için gerekli komutları oluşturduktan sonra, Update metodunu belirlediğimiz çakışma seçeneğine göre hazırlanan komutlara göre yürütmekteyiz. ConflictOption enum sabiti CompareAllSearchableValues, CompareRowVersion ve OverwriteChanges olmak üzere üç değer alabilmektedir. Buna göre SqlCommandBuilder nesnesinin bu 3 seçenek için ürettiği çıktılar aşağıdaki gibi olacaktır.

CompareAllSearchableValues için;

![mk180_2.gif](images/mk180_2.gif)

Dikkat ederseniz Update ve Delete sorgularında Where cümleciğine TimeStamp tipindeki Durum alanı hariç tüm alanlar katılmıştır.

CompareRowVersion için;

![mk180_3.gif](images/mk180_3.gif)

Dikkat ederseniz Update ve Delete sorgularında Where koşuluna sadece Primary Key olan PersonelId alanı ve TimeStamp tipinden olan Durum alanı katılmıştır. Bu çeşit bir sorgu özellikle DBConcurrency Violation durum için idealdir.

![dikkat.gif](images/dikkat.gif)
CompareRowVersion özellikle DBConcurrencyViolation durumu için biçilmiş kaftan gibi gözüksede, diğer veri sağlayıcıları için geliştirilmiş CommandBuilder nesnelerinde aynı geçerlilik olmayabilir. Nitekim, Timestamp veri türü her veritabanı sisteminde var olan bir tür değildir. Bu tip veri türlerine sahip olmayan sistemlerde mecburen DBConcurrencyViolation durumlarının ele alınmasında, where cümleciğinden sonra mümkün olan tüm alanların hesaba katılması gerekecektir. Bir başka deyişle CompareAllSearchableValues seçeneği seçilecektir.

OverwriteChanges için;

![mk180_4.gif](images/mk180_4.gif)

Dikkat ederseniz Update ve Delete sorgularına ait Where koşullarına sadece Primary Key olan PersonelId alanı katılmıştır. Dolayısıyla Last Wins modeli geçerlidir.

Görüldüğü gibi, SqlCommandBuilder sınıfına Framework 2.0 ile gelen ConflictOption özelliği, çakışma senaryolarına bağlı olarak uygun Delete ve Update komutlarının hazırlanmasını sağlamaktadır.

3. Update işlemleri sırasında sadece güncellenen parametreleri sql sunucusuna gönderebilme

SqlDataAdapter bağlantısız katman nesnelerinde yapılan değişilikleri asıl veri kaynağına yazmak üzere Update metodunu kullanmaktadır. Bağlantısız katmandaki verilerde sadece değişikliğe uğramış alanları update sorgularına dahil etmek istersek SqlCommandBuilder'ın SetAllValues isimli özelliğine false değerini atamamız yeterli olacaktır. SetAllValues özelliği varsayılan olarak true değerine sahiptir. Yani bir satırdaki alanların bazılarında değişiklik olmasada update sorgusuna gönderilmektedir. Bu durumu Sql Server Profiler yardımıyla kolayca analiz edebiliriz. Ama öncesinde kodumuzda aşağıdaki değişikliği yapalım.

```csharp
private void PrepareCommands()
{
    SqlCommandBuilder cmb = new SqlCommandBuilder(daPersonel);
    ConflictOption selectedOption = (ConflictOption)Enum.Parse(typeof(ConflictOption), cmbConflictOptions.SelectedItem.ToString()); 
    cmb.ConflictOption = selectedOption;
    cmb.SetAllValues = false; 
    daPersonel.InsertCommand = cmb.GetInsertCommand();
    daPersonel.UpdateCommand = cmb.GetUpdateCommand();
    daPersonel.DeleteCommand = cmb.GetDeleteCommand();
    // diğer kod satırları
}
```

Şimdi Windows uygulamamızı çalıştırılalım ve satırlar üzerinde, bazı alanlarda (örneğin sadece ad alanında) değişiklik yapıp Update metodunu çalıştıralım.

![mk180_5.gif](images/mk180_5.gif)

Update işlemini başlattıktan sonra Sql Server Profiler ile sql tarafına gönderilen update sorgularına bakarsak aşağıdaki sonuçları elde ederiz.

![mk180_6.gif](images/mk180_6.gif)

Örnek olarak sadece PersonelId değeri 1 olan satırın Adi alanını değiştirdiğimizden ve SqlCommandBuilder nesne örneğinde SetAllValues isimli özelliğe false değerini atadığımızdan, Update sorgusunda sadece Adi alanı kullanılmıştır. Ancak SetAllValues özelliğini hiç değiştirmessek yada bilinçli olarak true değerini atarsak aşağıdaki sonuçları elde ederiz.

![mk180_7.gif](images/mk180_7.gif)

Gördüğünüz gibi bu kez Update sorgusunda bütün alanlar kullanılmıştır.

4. SqlDataAdapter için üretilen command'lerde kolon adı kullanılmasını tercih edebilme

Şu ana kadar incelediğimiz maddelerde SqlCommandBuilder nesnesi, Update, Insert ve Delete komutlarını hazırlarken parametre adları olarak @P[Rakam] notasyonunu kullanmıştır. Yine Framework 2.0 ile gelen bir özellik sayesinde @P[KolonAdı] notasyonunu kullanma şansınada sahibiz. Bunun için GetInsertCommand, GetUpdateCommand ve GetDeleteCommand isimli metodların aşırı yüklenmiş versiyonunlarını kullanmamız gerekiyor.

```csharp
// Insert versiyonu
public SqlCommand GetInsertCommand (bool useColumnsForParameterNames)

// Delete versiyonu
public SqlCommand GetDeleteCommand (bool useColumnsForParameterNames)

// Update versiyonu
public SqlCommand GetUpdateCommand (bool useColumnsForParameterNames)
```

Bu versiyon bool tipinden bir değer almakta olup, parametre adlarında kolon isimlerinin gösterilip gösterilmeyeceğini belirtmektedir. Örnek uygulamamızda basit olarak bu bool değişkeni tutacak bir CheckBox kontrolü ele alıyoruz. Metodumuzu ise aşağıdaki gibi değiştirmemiz yeterli olacaktır.

```csharp
private void PrepareCommands()
{
    SqlCommandBuilder cmb = new SqlCommandBuilder(daPersonel);
    ConflictOption selectedOption = (ConflictOption)Enum.Parse(typeof(ConflictOption), cmbConflictOptions.SelectedItem.ToString()); 
    cmb.ConflictOption = selectedOption;
    cmb.SetAllValues = false; 
    bool useParameterNames = chkUseParameter.Checked; 

    daPersonel.InsertCommand = cmb.GetInsertCommand(useParameterNames);
    daPersonel.UpdateCommand = cmb.GetUpdateCommand(useParameterNames);
    daPersonel.DeleteCommand = cmb.GetDeleteCommand(useParameterNames);
     // diğer kod satırları
}
```

Şimdi uygulamamızı yeniden çalıştırıp farklı çakışma tipleri için oluşan sorgulara bakarsak aşağıdaki sonuçları elde ederiz.

CompareAllSearchableValues için;

![mk180_8.gif](images/mk180_8.gif)

CompareRowVersion için;

![mk180_9.gif](images/mk180_9.gif)

OverwriteChanges için;

![mk180_10.gif](images/mk180_10.gif)

Gördüğünüz gibi parametre adları artık kolon adlarından oluşturulmaktadır.

SqlCommandBuilder nesnesi Framework 2.0 daki ek fonksiyonellikleri ve özellikleri sayesinde artık daha kullanışlı hale gelmiştir. Diğer CommandBuilder nesneleride benzer işlevsellikleri sağlamakla birlikte, kullanılan veritabanı sisteminin sahip olduğu imkanlarda önemlidir. Örneğin OleDbCommandBuilder sınıfınında DeriveParameters metodu vardır ve bir OleDb kaynaklarından sp desteği olmayan veritabanlarına bağlanabilmemiz de mümkündür. Sp desteği olmadığı için böyle bir durumda OleDbCommandBuilder'ın DeriveParameters fonksiyonu bir işe yaramayacaktır. Diğer yandan özellikle bağlantısız katman nesneleri ile çalışırken çeşitli çakışma kritlerlerine göre otomatik olarak sorguların oluşturulabilmesi önemli bir özellik olarak karşımıza çıkmaktadır. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek kod için tıklayın.](assets/CommandBuilder.rar)
