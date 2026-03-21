---
layout: post
title: "Ado.Net 2.0 ve Data Provider-Independent Mimari"
date: 2005-04-24 12:00:00 +0300
categories:
  - ado-net-2-0
tags:
  - ado.net
  - data-providers
  - dbconnection
  - dbcommand
  - dbdataadapter
  - dbproviderfactory
---
Veritabanı uygulamalarında başımızı ağrıtan noktalardan bir tanesi farklı tipte veritabanı sistemleri kullanan uygulamaların geliştirilmesi sırasında ortaya çıkar. Çoğu zaman geliştirdiğimiz bir ürün Sql sunucları üzerinde yüksek performans gösterecek şekilde çalışmak zorunda iken, aynı ürünün Oracle üzerinde çalıştırılması da istenebilir. Bu durumda ortak bir çözüm olarak OleDb isim alanı altındaki sınıfları kullanmak oldukça mantıklıdır. Çünkü OleDb üzerinden her iki veri sunucusu için gerekli olan veri sağlayıcılarını kullanabiliriz.

Diğer yandan böyle bir yol izlendiğinde direkt olarak OracleClient veya SqlClient isim alanını kullanarak kazanılacak performans avantajı ortadan kaybolacaktır. Peki.Net Framework için geliştirilen başka bir veri sağlayıcı (data-provider) işin içine girerse ne olacaktır. Bu durumda uygulama kodunda ilgili veri sağlayıcısına destek verecek şekilde düzenlemeler yapmamız gerekecektir. Öyleki, SqlClient isim alanı için gerekli Connection nesnesi ile OracleClient isim alanı için gerekli Connection nesnelerinin isimleri farklıdır. Aynı durum Command nesnelerinden tutun da DataReader nesnelerine kadar geçerlidir. İşte bu durum kodlarımızı her veri sağlayıcı için ayrı şekilde düzenlememizi gerektirebilir.

Diğer yandan System.Data.Common isim alanındaki sınıfları kullanarak farklı veri sağlayıcılarına destek verebilecek katmanları geliştirebiliriz. Bu Ado.Net 1.1' de çok esnek olmayan bir mimari üzerinde gerçekleştirilmektedir. Oysa ki Ado.Net 2.0, System.Data.Common isim alanına bir takım yeni özellikler eklemiştir. Bu özellikler arasında yeni eklenen iki sınıf büyük öneme sahiptir. DbProviderFactories ve DbProviderFactory sınıfları. Ado.Net 2.0' da bu sınıfların System.Data.Common isim alanına eklenmelerinin en büyük nedeni, ilgili sistemlerde yüklü olan veri sağlayıcılarının öğrenilebilmesi ve seçilen herhangi bir veri sağlayıcısına özel Command, Connection, DataAdapter gibi veri üzerinde iş yapmamızı sağlayacak sınıfların tekil isimler altında örneklendirilebilmesidir.

Yeni eklenen özelliklerin sağladığı bu imkanlar sayesinde, geliştirilmiş olan bir ürünün farklı veri sağlayıcılar için destek verebilecek şekilde inşa edilmesi daha da kolaylaştırılmıştır. İşin güzel yanı, bu mimarinin performans olarak asıl veri sağlayıcılarına oranla oldukça iyi sonuçlar veriyor olmasıdır. İşte bu makalemizde System.Data.Common isim alanına eklenen bu yeni sınıflar ile neler gerçekleştirebileceğimizi incelemeye çalışacağız. Yeni eklenen sınıfların önemini daha iyi anlayabilmek için aşağıdaki şekli göz önüne alabiliriz.

![mk121_3.gif](/assets/images/2005/mk121_3.gif)

Aslında uygulamamıza katacağımız veri sağlayıcıdan bağımsız mimari için, yukarıdaki yolu izlememiz yeterli olacaktır. İlk olarak sistemde yüklü olan veri sağlayıcılarını elde edebiliriz. Ya da bunun seçimini kullanıcıya bırakabiliriz. Hangisinin kullanılacağına biz, kullanıcı veya sistemin kendisi karar verebilir. Son olarak seçilen veri sağlayıcı üzerinden gerçekleştireceğimiz veritabanı işlemleri için (örneğin kaynağa bağlantı açmak, komut yürütmek gibi) gerekli olan üreticiyi (ki bu DbProviderFactory sınıfıdır) hazırlarız. Daha sonra bu yeni sınıf yardımıyla gerekli olan nesneleri üretip kullanırız. Dilerseniz System.Data.Common isim alanına eklenen bu yeni iki sınıfı incelemekle işe başlayalım.

Öncelikle DbProviderFactories sınıfnı ele alalım. Bu sınıf sistemde yüklü olan veri sağlayıcılarını elde etmemizi sağlayan GetFactoryClasses isimli static bir metoda sahiptir. Bu sınıfın diğer bir static metodu ise GetFactory'dir. GetFactory metodu geriye DbProviderFactory tipinden bir nesne örneğini döndürür. DbProviderFactory tipinden nesne örnekleri yoluyla, DbConnection, DbCommand gibi nesneleri elde edebiliriz. Dolayısıyla DbProviderFactories sınıfı yardımıyla bir sistemdeki veri sağlayıcılarını elde edebilir ve seçilen bir veri sağlayacısı için gerekli nesneleri üretmemizi sağlayacak DbProviderFactory sınıfını örnekleyebiliriz. İlk olarak aşağıdaki kod parçasını ele alalım.

```csharp
using System.Data;
using System.Data.Common;

#endregion

namespace UsingDbProviderFactories
{
    class Program
    {
        static void Main(string[] args)
        {
            DataTable dtProviders = new DataTable();
            dtProviders = DbProviderFactories.GetFactoryClasses();
            foreach (DataRow drProvider in dtProviders.Rows)
            {
                for (int i = 0; i < dtProviders.Columns.Count; i++)
                {
                    Console.WriteLine(drProvider[i].ToString());
                }
            Console.WriteLine("----------");
            }
            Console.ReadLine();
        }    
    }
}
```

![mk121_1.gif](/assets/images/2005/mk121_1.gif)

Bu kod ile, sistemimizde yüklü olan veri sağlayıcılarını elde etmiş olduk. Dikkat ederseniz GetFactoryClasses metodundan dönen değer DataTable tipinden bir nesne örneğidir. Aynı örneği bir windows uygulamasında ele aldığımızda bu geri dönüş tipinden yararlanarak sonuçları veri bağlı kontroller üzerinde (datagridview gibi) gösterebiliriz.

```csharp
DataTable dtProviders = new DataTable();
dtProviders = DbProviderFactories.GetFactoryClasses();
grdProviders.DataSource=dtProviders;
```

![mk121_2.gif](/assets/images/2005/mk121_2.gif)

Peki sistemdeki veri sağlayıcılarının elde edilmesinin bize sağlayacağı avantajlar neler olabilir? Herşeyden önce ürünümüzün yüklendiği sistemlerdeki veri sağlayıcılarını görmek ve bunlardan seçişi olan ile uygulamayı çalıştırmak steyebiliriz. Böyle bir durumda GetFactoryClasses metodu işimizi oldukça kolaylaştıracaktır. Diğer yandan, ürünümüzü sisteme yüklerken kurulan herhangi bir konfigurasyon ayarı ile de bir veri sağlayıcıyı seçebiliriz. Çoğunlukla bunu xml içerikli konfigürasyon dosyalarında belirtiriz. (Örneğin app.config dosyası içinde). Uygulamanın hangi veri sağlayıcısını baz alarak devam edeceğine bu dosyadaki ilgili konfigürasyon ayarından karar verebiliriz.

Elbette böyle bir durumda uygulamanın yüklendiği sistemde seçilen veri sağlayıcısının olup olmadığına bakmak için yine yukarıdaki teknik ile elde edilen DataTable nesnesinden faydalanabiliriz. Bu sayede sistemde yüklü olmayan bir veri sağlayıcısı ile devam edilmesini de henüz kurulum aşamasında engellemiş oluruz. Bunun sonrasında kullanıcıya kullanabileceği veri sağlayıcıları alternatif olarak sunabiliriz ve uygun olanı ile devam etmesini sağlayabiliriz. Gelelim, DbProviderFactory sınıfına. Bu sınıf, veritabanına bağlantı açma, sql komutu çalıştırmak gibi işlemleri yürütmemizi sağlayacak DbConnection, DbCommand gibi sınıfların üretilmesini sağlar. Bu sınıfın prototipi aşağıdaki gibidir.

```csharp
public abstract class DbProviderFactory
```

Görüldüğü gibi DbProviderFactory abstract (soyut) bir sınıftır. Dolayısıyla bu sınıfa ait bir nesne örneğini üretemeyiz. Ancak DbProviderFactories sınıfımıza ait olan GetFactory static metodu bizim kullanabileceğimiz bir DbProviderFactory nesnesini sağlayacaktır. GetFactory metodunun aşırı yüklenmiş (overload) iki versiyonu vardır.

```csharp
public static DbProviderFactory GetFactory(DataRow data-provider için dataRow);
public static DbProviderFactory GetFactory(string data-provider için invariant name);
```

Bu metodlardan ilki DataRow tipinden bir nesne örneğini alır. Bu DataRow nesnesinin temsil ettiği satır aslında DbProviderFactories sınıfının GetFactoryClasses metodundan dönen DataTable üzerindeki satırlardan herhangi biridir. Diğer yandan ikinci versiyonda string tipinden parametrenin alacağı değer, ilgili veri sağlayıcısının sistemdeki sabit adıdır (invariant-name).

```csharp
DbProviderFactory fakto = DbProviderFactories.GetFactory("System.Data.SqlClient");
```

Elde ettiğimiz DbProviderFactory nesnesi vasıtasıyla artık veritabanı uygulamamız için gerekli nesneleri örnekleyebiliriz. Örneğin aşağıdaki kod parçası ile bir DbConnection nesnesi elde edilmektedir. DbConnection nesnesi tahmin edeceğiniz gibi ilgili veri kaynağına doğru bağlantı hattı tesis etmemizi sağlar.

![dikkat.gif](/assets/images/2005/dikkat.gif)
DbConnection, DbCommand, DbDataAdapter vb. abstract sınıflardır. Yani aslında bu sınıflara ait nesne örneklerini new operatörü yardımıyla oluşturamayız. Bu sınıflardan faydalanabilmek için DbProviderFactories sınıfının ilgili Create metodlarını kullanırız.

Kod parçamız;

```csharp
DbProviderFactory fakto = DbProviderFactories.GetFactory("System.Data.SqlClient");
DbConnection con = fakto.CreateConnection();
con.ConnectionString="data source=localhost;database=AdventureWorks;integrated security=SSPI";
con.Open();
con.Close();
Console.Read();
...
```

Bu kod parçasında SqlClient veri sağlayıcısını kullanacak şekilde bir DbConnection nesnesi elde edilmektedir. DbConnection nesnesini elde edebilmek için DbProviderFactory sınıfına ait CreateConnection metodu kullanılmaktadır. Ne yazık ki veri sağlayıcı bağımsız mimarinin de içinden şu an için çıkamayacağı sorunlar var. Bunlardan birisi ConnectionString'in bir veri sağlayıcıdan ötekine farklılık göstermesidir.

Yani Sql sunucularına SqlConnection nesnesi ile bağlantı kurarken kullandığımız Connection String ifadesi, OleDbConnection için olandan farklıdır. Bu sorunu çözmek için DbConnectionStringBuilder sınıfı kullanılmaktadır. Bu sınıf bir connection string içine yazılan özellikleri anahtar-değer (key-value) çiftleri şeklinde temsil eder. Böylece uygun Connection String elde edilebilir. Ancak tabiki öncesinde seçilen veri sağlayıcısının her durumda kontrol edilmesi gerekecektir. Aşağıdaki kod parçası hem SqlClient hem de OleDb için gerekli DbConnection nesnesinin doğru bir şekilde elde edilebilmesini sağlamaktadır. (Burada veri sağlayıcısının seçimi için app.config dosyasını kullandığımıza dikkat edin.)

![mk121_4.gif](/assets/images/2005/mk121_4.gif)

```bash
#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Configuration;

#endregion

namespace UsingDbProviderFactory
{
    class Program
    {
        static void Main(string[] args)
        {    
            DbProviderFactory fakto;
            DbConnection con;
            DbConnectionStringBuilder conStr = new DbConnectionStringBuilder();
            string secilenProvider = ConfigurationSettings.AppSettings["ProviderTipi"];
            fakto = DbProviderFactories.GetFactory(secilenProvider);
            con = fakto.CreateConnection();
            if (secilenProvider == "System.Data.SqlClient")
            { 
                conStr.Add("data source", "localhost");
                    conStr.Add("database", "AdventureWorks");
                    conStr.Add("integrated security", "SSPI"); 
            }
            else if (secilenProvider == "System.Data.OleDb")
            {
                conStr.Add("Provider", "SqlOleDb");
                conStr.Add("data source", "localhost");
                conStr.Add("database", "AdventureWorks");
                conStr.Add("integrated security", "SSPI");
            }
            else
            {
                Console.WriteLine("Doğru provider seçilmedi...");
            }
            con.ConnectionString = conStr.ConnectionString;
            try
            {
                con.Open();
                Console.WriteLine("Bağlantı açıldı...");
                Console.ReadLine();
            }
            catch(Exception hata)
            {
                Console.WriteLine(hata.Message.ToString());
            }
            finally
            {
                con.Close();
            }
        }
    }
}
```

Uygulamamızda başlangıç olarak veri sağlayıcısını Sql sunucusuna direkt erişim sağlayan SqlClient olarak belirledik. If koşullarında app.config dosyasına eklediğimiz ProviderTipi anahtarının değerine bakarak uygun Connection String ifadesinin oluşturulmasını sağlıyoruz. Eğer OleDb kaynağını kullanarak erişim sağlamak istersek tek yapmamız gereken app.config dosyasında ProviderTipi anahtarının değerini System.Data.OleDb olarak değiştirmek olacaktır.

![dikkat.gif](/assets/images/2005/dikkat.gif)
Seçilen veri sağlayıcısının (Data-Provider) ismi sistem de yüklü olan sabit ismidir. (Invariant Name)

Buradaki anahtarların değerlerinin sistemde tanımlı olan invariant-name değerleri olduğunu hatırlatalım. Aslında sistemde yüklü olan veri sağlayıcılarının özellikleri elde edilirken machine.config dosyasındaki ayarlara bakılır. Eğer D:\WINDOWS\Microsoft.NET\Framework\v2.0.40607\CONFIG (Windows 2003 için) adresinden machine.config dosyasına bakılırsa sistemde yüklü olan veri sağlayıcılarının listesinin DbProviderFactories takısında yer aldığını görebiliriz. İşte veri sağlayıcılarının sabit isimleri buradan alınmaktadır.

![mk121_5.gif](/assets/images/2005/mk121_5.gif)

DbConnection nesnesi gibi DbCommand nesneside veri sağlayıcıdan bağımsız komut setlerinin yürütülmesine imkan sağlamaktadır. Bir DbCommand nesnesinin oluşturuluş biçimi DbConnection nesnesinde olduğu gibidir. Yani bu nesneyi elde edebilmek için DbProviderFactory sınıfının ilgili Create metodunu aşağıdaki gibi kullanırız.

```csharp
DbProviderFactory fakto;
DbConnection con;
DbCommand cmd;
DbConnectionStringBuilder conStr = new DbConnectionStringBuilder();
string secilenProvider = ConfigurationSettings.AppSettings["ProviderTipi"];
fakto = DbProviderFactories.GetFactory(secilenProvider);
con = fakto.CreateConnection();
cmd = fakto.CreateCommand();
if (secilenProvider == "System.Data.SqlClient")
{ 
    conStr.Add("data source", "localhost");
    conStr.Add("database", "AdventureWorks");
    conStr.Add("integrated security", "SSPI"); 
}
else if (secilenProvider == "System.Data.OleDb")
{
    conStr.Add("Provider", "SqlOleDb");
    conStr.Add("data source", "localhost");
    conStr.Add("database", "AdventureWorks");
    conStr.Add("integrated security", "SSPI");
}
else
{
    Console.WriteLine("Doğru provider seçilmedi...");
}
con.ConnectionString = conStr.ConnectionString;
try
{
    con.Open();
    cmd.Connection = con;
    cmd.CommandText = "INSERT INTO Personel (AD,SOYAD,MAIL) VALUES ('Burak Selim','Şenyurt','selim(at)buraksenyurt.com')";
    int eklenen=cmd.ExecuteNonQuery();
    Console.WriteLine("Bağlantı açıldı...");
    Console.WriteLine(eklenen + " SATIR EKLENDI");
    Console.ReadLine();
}
catch(Exception hata)
{
    Console.WriteLine(hata.Message.ToString());
}
finally
{
    con.Close();
}
```

Görüldüğü gibi tek yapmamız gereken DbProviderFactory sınıfının CreateCommand metodunu kullanarak bir DbCommand nesnesini elde etmektir. Daha sonra bu komut nesnesinin kullanacağı sorgu ve bağlantı her zamanki yöntemlerimiz ile belirlenmiştir. Elbetteki, DbConnection nesnesi oluşturulurken yaşanan problemin benzeri burada da söz konusudur. Bu kez Command nesnelerinin parametre isimlendirmeleri bir veri sağlayıcıdan ötekine farklılık göstermektedir.

Örneğin SqlCommand nesnesinde kullanılan parametreler @ ile başlarken, OleDbCommand nesnelerinde sorgu cümleciğindeki parametreler? ile tanımlanmak zorundadır. Bu sorunu yine yukarıdaki if yapısı ile halledebiliriz. Söz konusu olan parametreleri DbParameter sınıfı ile tanımlayabiliriz. Dolayısıyla yukarıdaki örneği aşağıdaki gibi yazarsak parametre farklılığını bu örnekte geçerli olan veri sağlayıcıları için çözmüş oluruz.

```csharp
static void Main(string[] args)
{
    DbProviderFactory fakto;
    DbConnection con;
    DbCommand cmd;
    DbParameter prmAd;
     DbParameter prmSoyad;
     DbParameter prmMail;
    DbConnectionStringBuilder conStr = new DbConnectionStringBuilder();
    string secilenProvider = ConfigurationSettings.AppSettings["ProviderTipi"];
    string cmdQuery;
    fakto = DbProviderFactories.GetFactory(secilenProvider);
    con = fakto.CreateConnection();
    cmd = fakto.CreateCommand();
     prmAd = fakto.CreateParameter();
     prmSoyad = fakto.CreateParameter();
     prmMail = fakto.CreateParameter();
    if (secilenProvider == "System.Data.SqlClient")
    { 
        conStr.Add("data source", "localhost");
        conStr.Add("database", "AdventureWorks");
        conStr.Add("integrated security", "SSPI");
        cmdQuery = "INSERT INTO Personel (AD,SOYAD,MAIL) VALUES (@AD,@SOYAD,@MAIL)";
        prmAd.ParameterName = "@AD";
          prmAd.DbType = DbType.String;
          prmAd.Size = 50;
        cmd.Parameters.Add(prmAd);
        prmSoyad.ParameterName = "@SOYAD";
        prmSoyad.DbType = DbType.String;
        prmSoyad.Size = 50;
        cmd.Parameters.Add(prmSoyad);
        prmMail.ParameterName = "@MAIL";
        prmMail.DbType = DbType.String;
        prmMail.Size = 50;
        cmd.Parameters.Add(prmMail);
        cmd.Connection = con;
        cmd.CommandText = cmdQuery;
    }
    else if (secilenProvider == "System.Data.OleDb")
    {
        conStr.Add("Provider", "SqlOleDb");
        conStr.Add("data source", "localhost");
        conStr.Add("database", "AdventureWorks");
        conStr.Add("integrated security", "SSPI");
        cmdQuery = "INSERT INTO Personel (AD,SOYAD,MAIL) VALUES (?,?,?)";
        prmAd.DbType = DbType.String;
          prmAd.Size = 50;
        cmd.Parameters.Add(prmAd);
        prmSoyad.DbType = DbType.String;
        prmSoyad.Size = 50;
        cmd.Parameters.Add(prmSoyad);
        prmMail.DbType = DbType.String;
        prmMail.Size = 50;
        cmd.Parameters.Add(prmMail);
        cmd.Connection = con;
        cmd.CommandText = cmdQuery;
    }
    else
    {
        Console.WriteLine("Doğru provider seçilmedi...");
    }
    con.ConnectionString = conStr.ConnectionString;
    try
    {
        con.Open();
        prmAd.Value = "Burak Selim";
        prmSoyad.Value = "Şenyurt";
          prmMail.Value = "selim(at)buraksenyurt.com";
        int eklenen=cmd.ExecuteNonQuery();
        Console.WriteLine("Bağlantı açıldı...");
        Console.WriteLine(eklenen + " SATIR EKLENDI");
        Console.ReadLine();
    }
    catch(Exception hata)
    {
        Console.WriteLine(hata.Message.ToString());
    }
    finally
    {
        con.Close();
    }
}
```

DbProviderFactory sınıfı ayrıca DbDataAdapter nesnelerini elde edebilmemizi sağlayan CreatedDataAdapter isimli bir metoda sahiptir. Bu sayede bağlantısız katman nesneleri ile veri kaynağı arasındaki veri taşıma işlemlerini gerçekleştirebileceğimiz DataAdapter nesne örneklerini veri sağlaycısından bağımsız olacak şekilde elde edebiliriz. Aşağıdaki kod parçası ile bu işlemin nasıl gerçekleştirilebileceğini görmektesiniz. Bu örnekte kullanıcının seçtiği veri sağlayıcısının sistemde yüklü olup olmadığınıda kontrol ediyoruz. Örneğimizde veri sağlayıcılarını OleDb ve SqlClient ile sınırladık.

```bash
#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace UsingDbDataAdapter
{
    partial class Form1 : Form
    {
        private DbProviderFactory fakto;
        private DbConnection con;
        private DbConnectionStringBuilder conStr;
        private DbCommand cmd;
        private DbDataAdapter da;
        private DataTable dt;

        public Form1()
        {
            InitializeComponent();
        }
        private bool VeriSaglayiciKontrol(string invariantName)
        {
            int varmi = DbProviderFactories.GetFactoryClasses().Select("InvariantName='" + invariantName + "'").Length;
            if (varmi == 0)
                return false;
            else
                return true;
        }
        private void DbConnectionOlustur(DbConnectionStringBuilder connectionString)
        {
            con = fakto.CreateConnection();
            con.ConnectionString = connectionString.ConnectionString;
        }
        private void DbCommandOlustur(string sqlQuery)
        {
            cmd = fakto.CreateCommand();
            cmd.Connection = con;
            cmd.CommandText = sqlQuery;
        }
        private void FactoryOlustur(string veritabaniAdi)
        {
            if (radOleDb.Checked == true)
            {
                if (VeriSaglayiciKontrol("System.Data.OleDb"))
                {
                    fakto = DbProviderFactories.GetFactory("System.Data.OleDb");
                    conStr = new DbConnectionStringBuilder();
                    conStr.Add("provider", "SqlOleDb");
                    conStr.Add("data source", "localhost");
                    conStr.Add("database", veritabaniAdi);
                    conStr.Add("integrated security", "SSPI");
                    DbConnectionOlustur(conStr);
                }
            }
            if (radSql.Checked == true)
            {
                if (VeriSaglayiciKontrol("System.Data.SqlClient"))
                {
                    fakto = DbProviderFactories.GetFactory("System.Data.SqlClient");
                    conStr = new DbConnectionStringBuilder();
                    conStr.Add("data source", "localhost");
                    conStr.Add("database", veritabaniAdi);
                    conStr.Add("integrated security", "SSPI");
                    DbConnectionOlustur(conStr);
                }
            }
        }
        private void DbDataAdapterOlustur()
        {
            da = fakto.CreateDataAdapter();
            da.SelectCommand = cmd;
        }
        private void btnVeriCek_Click(object sender, EventArgs e)
        {
            FactoryOlustur("AdventureWorks");
            DbCommandOlustur("SELECT TOP 5 FirstName,MiddleName,LastName FROM Person.Contact");
            DbDataAdapterOlustur();
            dt = new DataTable();
            da.Fill(dt);
            grdVeriler.DataSource = dt;
        }
    }
}
```

![mk121_6.gif](/assets/images/2005/mk121_6.gif)

Uygulamamızı çalıştırdığımızda ister Sql veri sağlayıcısını ister OleDb veri sağlayıcısını seçelim aynı DbConnection, DbCommand ve DbDataAdapter nesneleri üzerinden işlem yapıyor olacağız. İşte bu yeni mimarinin bize sağladığı en büyük avantajdır.

Son olarak bir DataReader nesnesini veri sağlayıcıdan bağımsız mimaride nasıl kullanabileceğimizi inceleyeceğiz. Aslında şu an için bu mimaride geliştirilmiş bir DbDataReader sınıfı yok. Bunun yerine IDataReader arayüzünü kullanacağız. Dolayısıyla arayüzü kullanacağımız yere kadar yaptığımız işlemler yukarıdaki örnektekinden farksız olacak.

```csharp
private void btnIDataReaderIleCek_Click(object sender, EventArgs e)
{
    FactoryOlustur("AdventureWorks");
    DbCommandOlustur("SELECT TOP 5 FirstName,MiddleName,LastName FROM Person.Contact");
    using (con)
    {
        con.Open();
        using (IDataReader dr = cmd.ExecuteReader())
        {
            while (dr.Read())
            {
                //bir takım okuma işlemleri.
            }
        }
    }
}
```

Ado.Net 2.0 için veri sağlayıcıdan bağımsız mimariyi kullanmak oldukça kolay ve yararlı görünüyor. Özellikle yeni eklenen sınıflar bu tarz yapıları oluşturmamızı son derece kolaylaştırmış. Burada kafamızı kurcalayan tek konu performans farklılıkları. Microsoft kaynaklarına ve otoritelerine göre çok büyük performans kayıpları olmadığı (olmayacağı) söyleniyor. Açıkçası bu konunun ilerleyen zamanlarda daha da netleşeceği görüşündeyim. Yine de yeni kolaylıkların özellikle farklı veri sunucularına bağlanmamızı gerektiren durumlarda çok başarılı olacağını söyleyebilirim.

Herşeyden önemlisi, hangi veri sağlayıcısı olursa olsun aynı DbConnection, DbCommand veya DbDataAdapater nesnelerini kullanmak son derece büyük bir avantajdır. Mimarinin şu an için sadece başında olduğumuzu düşünüyorum. System.Data.Common isim alanına katılacak daha bir çok özellik olabilir. Örneğin buraya özel bir DbDataReader sınıfının oluşturulması gibi. Şunu da hatırlatmakta fayda var. Buradaki kod örnekleri ve kullanılan sınıflar, beta sürümüne aittir. Ürün piyasaya çıktığında bu sınıfların isimlerinde veya kullanılış şekillerinde değişiklikler olabilir. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.