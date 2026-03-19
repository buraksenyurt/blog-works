---
layout: post
title: "Ado.Net 2.0 ve Bulk-Data Kopyalama Mekanizması"
date: 2004-09-17 12:00:00 +0300
description: "Sql Server'da bir veritabanı tablosundan, başka bir hedef tabloya veri taşıma işlemi bulk-data kopyalama olarak adlandırılır. Veritabanı yöneticileri çoğunlukla bu operasyonu gerçekleştirmek amacıyla, BCP adı verilen komut satırı aracını kullanırlar. Burada amaç, kaynak tablodaki satırların veya bir satır kümesinin farklı konumda olabilecek bir tabloya taşınmasıdır. Hedef tablo aynı veritabanında olabileceği gibi, diğer bir sql sunucusu üzerindeki başka bir veritabanında da yer alabilir. Ado.Net 2.0' da SqlClient isim alanına eklenen yeni sınıflar yardımıyla bu işlemleri yönetimli kodda (managed-code) gerçekleştirme imkanına da artık sahibiz. Bu makalemizde, bu işlemleri gerçekleştirmek için kullanabileceğimiz yeni Ado.Net 2.0 sınıflarını incelemeye çalışacağız."
categories:
  - ado-net-2-0
tags:
  - ado-net-2-0
  - bash
  - csharp
  - dotnet
  - ado-net
  - sql-server
  - xml
  - transactions
  - delegates
  - generics
  - datatable
---
# Ado.Net 2.0 ve Bulk-Data Kopyalama Mekanizması
Değerli Okurlarım Merhabalar,

Sql Server'da bir veritabanı tablosundan, başka bir hedef tabloya veri taşıma işlemi bulk-data kopyalama olarak adlandırılır. Veritabanı yöneticileri çoğunlukla bu operasyonu gerçekleştirmek amacıyla, BCP adı verilen komut satırı aracını kullanırlar. Burada amaç, kaynak tablodaki satırların veya bir satır kümesinin farklı konumda olabilecek bir tabloya taşınmasıdır. Hedef tablo aynı veritabanında olabileceği gibi, diğer bir sql sunucusu üzerindeki başka bir veritabanında da yer alabilir. Ado.Net 2.0' da SqlClient isim alanına eklenen yeni sınıflar yardımıyla bu işlemleri yönetimli kodda (managed-code) gerçekleştirme imkanına da artık sahibiz. Bu makalemizde, bu işlemleri gerçekleştirmek için kullanabileceğimiz yeni Ado.Net 2.0 sınıflarını incelemeye çalışacağız.

Bulk-Data kopyalama işlemi için Ado.Net 2.0 ile gelen en önemli sınıf, SqlClient isim alanında yer alan SqlBulkCopy sınıfıdır. Bu sınıfa ait nesne örnekleri yardımıyla, kaynak tablodan hedef tabloya veri transferi işlemleri kolayca gerçekleştirilebilir. Bu işlemler sırasında SqlBulkCopy nesne örnekleri, taşıma işlemini varsayılan olarak açtığı bir transaction içerisinde gerçekleştirmektedir. Yani, hedef tabloya yapılan taşıma işlemleri sırasında oluşabilecek olan hatalar sonrasında, transaction işlemi iptal edilerek roll-back operasyonu gerçekleşir ve hedef tabloya o ana kadar girilen satırlar geri alınır.

Burada önemli olan noktalardan birisi, kaynak ve hedef tabloların aynı şema yapısına sahip olmalarının önemli olmayışıdır. Yani, aynı alan adları, eşit alan sayıları ve aynı alan sıraları olmak zorunda değildir. Nitekim, hedef tablo ile kaynak tablo arasında alan eşleşmelerinde uyumsuzluk olabilir. Örneğin, alan adları ve sayılar birbirinden farklı olabilir. İşte bu durumda, kaynak ve hedef tablodaki alanları birbirleriyle eşleştirmekte kullanılan SqlBulkCopyColumnMapping sınıfına ait nesne örnekleri kullanılır. Bu sınıf yardımıyla kaynak ve hedef alanların kolayca eşleştirilmesi sağlanmış olur. Bulk-Data kopylama işlemi için önemli olan bir diğer sınıf ise, SqlBulkCopyColumnMappingCollection'dır. Bu sınıf ise, tahmin edeceğiniz gibi SqlBulkCopyColumnMapping sınıfı türünden nesne örneklerinin bir koleksiyonunu ifade etmektedir.

Bulk-Data kopyalama operasyonunda, kaynak veriler için yine SqlClient isim alanındaki standart nesneler kullanılabilir. Esasen bu tip bir operasyonda, kaynak verileri herhangibir SqlDataReader nesnesinden, bir DataTable'dan veya bir DataRow dizisinden alabiliriz. Hatta bir Xml dökümanınıda kaynak olarak kullanabiliriz. Böylece yönetimsel koda kazandırılan bu imkanlar ile veri taşıma işlemi için büyük esneklik kazanmış olmaktayız.

![mk93_10.gif](/assets/images/2004/mk93_10.gif)

Şekil 1. SqlBulkCopy Sınıfının Çalışma Şekli.

Bulk-Data kopylama işleminin daha kolay anlaşılabilmesi için elbette örnekler ile olayı incelememiz daha faydalı olacaktır. Şimdi aşağıdaki basit Console uygulamasını göz önüne alalım.

```bash
#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

#endregion

namespace BulkCopy
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection con = new SqlConnection("data source=localhost;initial catalog=AdventureWorks;integrated security=SSPI;MultipleActiveResultSets=true");
            
            /*Hedef tablodaki satırları önlem olarak siliyoruz.*/
            SqlCommand cmd = new SqlCommand("DELETE FROM YEDEK_MailList", con);
            con.Open();
            cmd.ExecuteNonQuery();

            /* Kaynak tablodan verileri SqlDataReader yardımıyla çekiyoruz.*/
            SqlCommand cmdCek = new SqlCommand("SELECT * FROM MailList", con);
            SqlDataReader dr;
            dr = cmdCek.ExecuteReader();

            /*SqlBulkCopy nesnemizi hedef bağlantıyı belirterek oluşturuyoruz.*/
            SqlBulkCopy bc = new SqlBulkCopy(con);
            
            /* Hedef tabloyu belirtiyoruz.*/
            bc.DestinationTableName = "YEDEK_MailList";
           
             /*WriteToServer metodu ile SqlDataReader' dan okuduğumuz satırları, hedefe insert ediyoruz.*/
            bc.WriteToServer(dr);

            /* Nesneleri kapatıyoruz.*/
            bc.Close();
            dr.Close();
            con.Close();
        }
    }
}
```

Örneğimizi incelemeden önce, Yukon sunucumuz üzerinde, AdventureWorks veritabanı altında MailList ve YEDEK_MailList isimli iki tablomuz olduğunu ve bu tabloların şema yapılarının birbirleriyle tamamen aynı olduklarını belirtelim.

![mk93_1.gif](/assets/images/2004/mk93_1.gif)

![mk93_2.gif](/assets/images/2004/mk93_2.gif)

Şekil 2. Her iki tablonunda şema yapısı aynıdır.

Burada gerçekleştirdiğimiz operasyon, bulk-data kopyalama işleminin en basit halidir. Şimdi neler yaptığımıza biraz daha yakından bakalım. İlk olarak Yukon sunucumuzdaki AdventureWorks veritabanına bir bağlantı açtık. Daha sonra, tedbir olarak YEDEK_MailList tablosundaki tüm satırları sildik. Nitekim böyle bir işlem yapmasaydık YEDEK_MailList tablosuna taşınan satırlar sürekli arka arkaya eklenecekti. Daha sonra ise, klasik olarak kaynak tablomuzdan verileri bir SqlDataReader nesnesi yardımıyla çektik. Bizi asıl ilgilendiren kısım aşağıdaki kod satırlarının yer aldığı kısımdır.

```csharp
SqlBulkCopy bc = new SqlBulkCopy(con);
            
bc.DestinationTableName = "YEDEK_MailList";

bc.WriteToServer(dr);
```

Burada ilk satırda, SqlBulkCopy nesnemizi o anki geçerli SqlConnection nesnesi ile oluşturuyoruz. Yapıcı metoda ait parametre hedef bağlantıyı temsil etmektedir. Nitekim, hedef tablomuz aynı sunucuda olmayabilir. Ya da, aynı veritabanı üzerinde olmayabilir. Bu nedenle burada bağlantıyı dikkatli seçmek gerekmektedir. Sonraki satırda ise, taşıma işleminin hedef alındığı tablo, SqlBulkCopy nesnemize DestinationTableName özelliği ile bildirilir. Buradaki tablo, SqlBulkCopy nesnesi örneklendirilirken parametre olarak verdiğimiz SqlConnection nesnesinin belirttiği bağlantı üzerinde aranır. Son satırımızda ise, WriteToServer metodu kullanılmıştır. Bu metoda da parametre olarak, kaynak tabloya ait veri kümesini taşıyan SqlDataReader nesnesi verilmiştir. Böylece WriteToServer metodu, SqlDataReader nesnesi ile MailList tablosundan okuduğumuz satırları, SqlBulkCopy nesnesi oluşturulurken belirtilen bağlantı üzerindeki hedef tabloya yazmaktadır.

Örneği çalıştırdığımızda, kaynak verilerin hedef tabloya taşındığını görürüz. Burada enteresan bir nokta vardır. Bu işlemi üst üste bir kaç kez uyguladığımızda, hedef tablodaki ID alanının değerlerinin sürekli olarak arttığını görürüz. Öyleki kaynak tablomuzda 1,2,3 olarak giden ID alanı değerleri, uygulama bir kaç kez çalıştırıldıktan sonra hedef tabloda aşağıdaki gibi olabilir.

![mk93_3.gif](/assets/images/2004/mk93_3.gif)

Şekil 3. ID alanının durumu.

Her ne kadar biz ID alanını her iki tabloda otomatik olarak artan identity değeri ile belirtsekte, Bulk-Data kopyalama işleminde, ID değerlerinin bozulmadan hedef tabloya yansıtılabilmeside mümkündür. Bunun için, SqlBulkCopy nesnemizin bir diğer yapıcı metodunu kullanırız.

```csharp
public SqlBulkCopy(connectionString, SqlBulkCopyOptions copyOptions);
```

Bu yapıcı metod SqlBulkCopyOptions numaralandırıcısı türünden bir parametre daha alır. Bu numaralandırıcının alabileceği değerler şunlardır.

SqlBulkCopyOptions Numaralandırıcı Değeri

Açıklaması

CheckConstraints
Hedef tablodaki kısıtlamalar var ise bunlar göz önüne alınarak veri girişi gerçekleşir.

Default
Varsayılan değerler kullanılır.

KeepIdentity
Kaynak tablodaki identity değerleri hedef tablodada korunur. Yani değiştirilmeden eklenir.

KeepNulls
Null değerlerin hedef tabloya korunarak geçirilmesini sağlar.

TableLock
Bulk-Data kopylama işlemi sırasında tabloya kilit koyar.

Bizim örneğimizde kullanmamız gereken değer, KeepIdentity'dir. Şimdi kodumuzdaki SqlBulkCopy nesnemizin yapıcı metodunu aşağıdaki parametreleri ile çağıralım.

```csharp
SqlBulkCopy bc = new SqlBulkCopy("data source=localhost;initial catalog=AdventureWorks;integrated security=SSPI", SqlBulkCopyOptions.KeepIdentity);
```

Uygulamamızı şimdi tekrar çalıştıracak olursak, hedef tablomuz olan YEDEK_MailList içindeki ID alanlarının değerlerinin, kaynak tablodaki ile aynı olduğunu bir başka deyişle korunduğunu görürüz. Uygulamamızı bir kaç sefer üst üste çalıştırsakta sonuç öncekinde olduğu gibi değişmeyecek ve kaynak tablodaki ID alanlarının değerleri hedef tabloya korunarak geçecektir.

Veri taşımalarındaki diğer bir noktada, null değerlerin hedef tabloya taşınmasıdır. Dikkat edecek olursanız, SqlBulkCopyOptions numaralandırıcısı Null değerlerin hedef tabloya korunarak geçirilmesini sağlayan KeepNulls değerine sahiptir. Bu özelliği daha iyi anlayabilmek amacıyla hedef tablomuzda ufak bir değişiklik yapalım. MAIL alanının null değer içerebildiğini biliyoruz. Hedef tablomuzda bu alan için bir default değer verelim.

![mk93_4.gif](/assets/images/2004/mk93_4.gif)

Şekil 4. Default Değer belirledik.

Şimdi bu koşullarda MailList isimli kaynak tablomuzda yeni bir satır oluşturalım ancak MAIL alanını null olarak bırakalım. Uygulamayı çalıştırdığımızda, bu satırın null değerinin hedef tabloya taşınmadığını ve hedef tabloya "MAIL TANIMLI DEGIL" değerinin yazıldığını görürüz.

![mk93_5.gif](/assets/images/2004/mk93_5.gif)

Şekil 5. Null değer taşınmadı.

Ancak SqlBulkCopy nesnemizi aşağıdaki gibi oluşturduğumuzda, null değerin hedef tablodaki alana taşındığını görürüz.

```csharp
SqlBulkCopy bc = new SqlBulkCopy("data source=localhost;initial catalog=AdventureWorks;integrated security=SSPI", SqlBulkCopyOptions.KeepNulls);
```

Şimdiye kadar ki kodlarımızda, SqlBulkCopy nesnesi için iki yapıcı metod kullandık. SqlBulkCopy nesnelerini oluşturabileceğimiz yapıcı metodların tamamı aşağıdaki tabloda yer almaktadır.

Name
Description

SqlBulkCopy (SqlConnection)
Hedef bağlantıyı bir SqlConnection nesnesi belirtir.

SqlBulkCopy (String)
Hedef bağlantı için SqlConnection String kullanılır.

SqlBulkCopy (String, SqlBulkCopyOptions)
Hedef bağlantıyı string bilgisi olarak alır ve SqlBulkCopy nesnesini, SqlBulkCopyOptions numaralandırıcısı ile bertilen şartlara göre oluşturulur.

SqlBulkCopy (SqlConnection, SqlBulkCopyOptions, SqlTransaction)
SqlBulkCopy nesnesini hedef bağlantıda, SqlBulkCopyOptions ile belirtilen şartlarda, SqlTransaction nesnesi ile belirtilen Transaction içinde oluşturur.

Bulk-Data kopyalama işlemini asıl gerçekleştiren WriteToServer metodununda çeşitli aşırı yüklenmiş (overload) verisyonları vardır. Bu versiyonlar aşağıdaki tabloda olduğu gibidir.

Overload Versiyonu
Açıklaması

SqlBulkCopy.WriteToServer (DataRow[])
Bir DataRow dizisini hedef tabloya taşır.

SqlBulkCopy.WriteToServer (DataTable)
Bir DataTable'ı hedef tabloya taşır.

SqlBulkCopy.WriteToServer (IDataReader)
Bir SqlDataReader'dan okuduğu veri kümesini hedef tabloya taşır.

SqlBulkCopy.WriteToServer (DataTable, DataRowState)
Bir DataTable'dan DataRowState numaralandırıcısını belirttiği kriterlere uyan satırlarını, hedef tabloya taşır.

Gördüğünüz gibi, WriteToServer metodunu etkili versiyonları vardır. Örneğin, bir DataTable üzerinde sadece değiştirilmiş olan satırların hedef tabloya yazılmasını sağlayabiliriz. Bunun için tek yapmamız gereken, DataRowState numaralandırıcısının Modified değerini kullanmak olacaktır. Bildiğiniz gibi DataRowState numaralandırıcısı, Modified, Unchanged, Inserted ve Deleted değerlerinden birisini alabilir. Bu durumu dilerseniz bir örnek üzerinde inceleyelim.

```csharp
static void Main(string[] args)
{
    SqlConnection con = new SqlConnection("data source=localhost;initial catalog=AdventureWorks;integrated security=SSPI");
/*Hedef tablodaki satırları önlem olarak siliyoruz.*/
    SqlCommand cmd = new SqlCommand("DELETE FROM YEDEK_MailList", con);
    con.Open();
    cmd.ExecuteNonQuery();

    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM MailList", con);
    DataTable dt = new DataTable();
    da.Fill(dt);
    DataRow drYeni;
    drYeni = dt.NewRow();
    drYeni["ID"] = "10";
    drYeni["AD"] = "Maykıl";
    drYeni["SOYAD"] = "Cordın";
    drYeni["MAIL"] = "michael@jordan.com";
    dt.Rows.Add(drYeni);

    drYeni = dt.NewRow();
    drYeni["ID"] = "11";
    drYeni["AD"] = "Kerim Abdul";
    drYeni["SOYAD"] = "Cabbar";
    drYeni["MAIL"] = "nba@nba.com";
    dt.Rows.Add(drYeni);

    /*SqlBulkCopy nesnemizi hedef bağlantıyı belirterek oluşturuyoruz.*/
    SqlBulkCopy bc = new SqlBulkCopy("data source=localhost;initial catalog=AdventureWorks;integrated security=SSPI",SqlBulkCopyOptions.KeepIdentity);
    /* Hedef tabloyu belirtiyoruz.*/
    bc.DestinationTableName = "YEDEK_MailList";
    /*WriteToServer metodu ile DataTable içinde sadece yeni eklenmiş olan satırları, hedef tabloya yazdırıyoruz.*/
    bc.WriteToServer(dt, DataRowState.Added);

    /* Nesneleri kapatıyoruz.*/
    bc.Close();
    con.Close();
}
```

Bu kodda gördüğünüz gibi, MailList tablomuzun içeriğini bağlantısız katman nesnelerinden birisi olan DataTable'a aktardık. Daha sonra ise basit bir şekilde iki adet satır ekledik. Bu satırlar yeni eklendikleri için, DataRowState değerleri Added olarak belirlenmiştir. SqlBulkCopy nesnemizin WriteToServer metodunu uygularken,

```csharp
bc.WriteToServer(dt, DataRowState.Added);
```

ikinci parametreye DataRowState.Added değerini verdik. Böylece hedef tabloya, dataTable nesnesinin sahip olduğu veri kümesinden sadece yeni eklenen satırlar yazılacaktır. Uygulamamızı çalıştırdığımızda YEDEK_MailList tablomuzun aşağıdaki satırlara sahip olduğunu görürüz. Bu satırlar uygulamada eklenen satırlardır.

![mk93_6.gif](/assets/images/2004/mk93_6.gif)

Şekil 5. Sadece DataTable'a eklenen satırların hedef tabloya aktarılması sonucu.

Görüldüğü gibi SqlBulkCopy nesnesi sayesinde, verilerin başka tablolara aktarılması son derece kolaydır. Bununla birlikte, bu tekniği yedekleme işlemleri için sıklıkla kullanabiliriz. Herşeyden önce, var olan bir tablonun tüm satırılarını, satırların belirli bir kümesini, yada son kullanıcı tarafından bağlantısız katmanda yeni eklenmiş, silinmiş, güncellenmiş satırların tamamını başka bir hedef tabloya kolayca aktarabilme yeteneğine sahip olmuş oluyoruz.

Şu ana kadar gerçekleştirdiğimiz örnekler, aynı SqlConnection'ı kullamaktadır. Oysaki gerçek hayatta bu tip taşıma işlemlerini, farklı sunucularda yer alan farklı tablolara doğru gerçekleştirmek isteyebiliriz. Bununla birlikte, hedef tablomuz nerede olursa olsun farklı bir şema yapısınada sahip olabilir. Böyle bir durumda özellikle alanların doğru ve uygun bir şekilde eşleştirilmesi önem kazanmaktadır. Şimdi bu durumu incelemek amacıyla öncelikle, YEDEK_MailList tablomuzu farklı alan isimleri ve sıralama ile aynı sunucuda bulunan farklı bir veritabanı altına koyalım.

![mk93_7.gif](/assets/images/2004/mk93_7.gif)

Şekil 7. YEDEKMailList Tablomuz.

Bu sefer tablomuzu hem farklı bir veritabanı altına koyduk, hemde şema yapısını biraz daha değiştirdik. Herşeyden önemlisi, alan adlarını, sıralarını değiştirdik. Bununla birlikte fazladan bir alanımız (AccountStatus) daha mevcuttur. Bu nedenle, kaynak ve hedef tablolardaki alanları birbirleriyle eşleştirmemiz gerekmektedir. İşte bu amaçla, kodlarımızda SqlBulkCopyColumnMapping sınıfına ait nesne örneklerini kullanmalıyız. Şimdi uygulama kodlarımızı aşağıdaki gibi değiştirelim.

```csharp
static void Main(string[] args)
{
    try
    {
        SqlConnection con = new SqlConnection("data source=localhost;initial catalog=Dukkan;integrated security=SSPI");
        /*Hedef tablodaki satırları önlem olarak siliyoruz.*/
        SqlCommand cmd = new SqlCommand("DELETE FROM YEDEK_MailList", con);
        con.Open();
        cmd.ExecuteNonQuery();
        con.Close();

        /* Kaynak tablodan verileri SqlDataReader yardımıyla çekiyoruz.*/
        SqlConnection con1 = new SqlConnection("data source=localhost;initial catalog=AdventureWorks;integrated security=SSPI");
        SqlCommand cmdCek = new SqlCommand("SELECT * FROM MailList",con1);
        SqlDataReader dr;
        con1.Open();
        dr = cmdCek.ExecuteReader();

        /*SqlBulkCopy nesnemizi hedef bağlantıyı belirterek oluşturuyoruz.*/
        SqlBulkCopy bc = new SqlBulkCopy("data source=localhost;initial catalog=Dukkan;integrated security=SSPI", SqlBulkCopyOptions.KeepIdentity);
        /* Hedef tabloyu belirtiyoruz.*/
        bc.DestinationTableName = "YEDEK_MailList";

        /* Kaynak ve Hedef tabloların alanları eşleştiriliyor.*/
        SqlBulkCopyColumnMapping cm1 = new SqlBulkCopyColumnMapping("ID", "UserID");
        SqlBulkCopyColumnMapping cm2 = new SqlBulkCopyColumnMapping("AD", "UserName");
        SqlBulkCopyColumnMapping cm3 = new SqlBulkCopyColumnMapping("SOYAD", "UserLastName");
        SqlBulkCopyColumnMapping cm4 = new SqlBulkCopyColumnMapping("MAIL", "MailAddress");
        /* Eşleştirmeler için oluşturulan SqlBulkCopyColumnMapping nesneleri, SqlBulkCopy nesnemizin ColumnMappings koleksiyonuna ekleniyor.*/
        bc.ColumnMappings.Add(cm1);
        bc.ColumnMappings.Add(cm2);
        bc.ColumnMappings.Add(cm3);
        bc.ColumnMappings.Add(cm4);
    
        /*WriteToServer metodu ile SqlDataReader' dan okuduğumuz satırları, hedefe insert ediyoruz.*/
        bc.WriteToServer(dr);
        
        /* Nesneleri kapatıyoruz.*/
        bc.Close();
        dr.Close();
        con1.Close();
    }
    catch (SqlException ex)
    {
        Console.WriteLine(ex.Message);
        Console.ReadLine();
    }
}
```

Burada bizim için önemli olan, kaynak ve hedef tablolardaki alan eşleştirmesini nasıl bildirdiğimizdir. Bunun için, aşağıdaki örnek söz dizimini kullandık.

```csharp
SqlBulkCopyColumnMapping cm1 = new SqlBulkCopyColumnMapping("ID", "UserID");
```

Örneğimizde, SqlBulkCopyColumnMapping sınıfının aşağıdaki prototipe sahip olan yapıcısını kullandık.

```csharp
public SqlBulkCopyColumnMapping(string sourceColumn, string destinationColumn);
```

Bu yapıcı haricinde kullanabileceğimiz diğer yapıcılarda aşağıdaki tabloda yer almaktadır.

Yapıcı Metod
Açıklama

SqlBulkCopyColumnMapping ()

SqlBulkCopyColumnMapping (Int32, Int32)
Kaynak ve Hedef alanların indeksini alır.

SqlBulkCopyColumnMapping (Int32, String)
Kaynak alanın indeksini, hedef alanın ismini alır.

SqlBulkCopyColumnMapping (String, Int32)
Kaynak alanın ismini, hedef alanın indeksini alır.

SqlBulkCopyColumnMapping nesnelerimizi yarattıktan sonra bu nesnelerin, SqlBulkCopy nesnemizin ilgili ColumnMappings koleksiyonuna eklenmesi gerekmektedir. Bunun için Add metodunu kullandık. Böylece SqlBulkCopy nesnemiz, kaynak tablodaki hangi alanın, hedef tablodaki hangi alana denk geleceğini bilmektedir. Aynı örneği, aşağıdaki kodlar ilede gerçekleştirebiliriz.

```csharp
bc.ColumnMappings.Add("ID", "UserID");
bc.ColumnMappings.Add("AD", "UserName");
bc.ColumnMappings.Add("SOYAD", "UserLastName");
bc.ColumnMappings.Add("MAIL", "MailAddress");
```

Burada daha kısa bir çözüm uygulanmıştır. Bu sefer doğrudan ColumnMappings özelliğinin Add metodu kullanılarak, ilgili alan eşleştirmeleri SqlBulkCopy nesnesine eklenmiştir. Bir SqlBulkCopyColumnMapping nesnesinin oluşturulmasında kullanılan aşırı yüklenmiş yapıcılardaki parametreler buradaki Add metodu içinde geçerlidir. Yani dilersek, Add metodunda alanların indeks değerlerinide kullanabiliriz. Örneğimizi çalıştırdığımızda, kaynak tablodaki satırların yeni tablomuza başarılı bir şekilde eklendiğini görürüz.

![mk93_8.gif](/assets/images/2004/mk93_8.gif)

Şekil 8. Alanların eşleştirilmesi sonucu.

SqlBulkCopy sınıfının SqlRowsCopied isimli tek bir olayı vardır. Bu olay, SqlBulkCopy sınıfının NotifyAfter özelliğine verilen değeri baz alır ve bu değeri kullanarak Bulk-Data kopyalama işlemleri sırasında periyodik olarak çalışır. Yani, eğer biz NotifyAfter özelliğine 20 değerini atarsak, kaynaktan hedefe taşınan her 20 satırda bir SqlRowsCopied olayı çalışacaktır. Bu olayın prototipi aşağıdaki gibidir.

```csharp
public event SqlRowsCopiedEventHandler SqlRowsCopied;
```

Olayın gerçekleşmesi sonucu çalıştırılacak metodu, SqlRowsCopiedEventHandler delegesi aşağıdaki gibi tanımlar.

```csharp
public sealed delegate void SqlRowsCopiedEventHandler(object sender, SqlRowsCopiedEventArgs e);
```

Şimdi örneğimizdeki SqlBulkCopy nesnesi için SqlRowsCopied olayını kullanalım.

```csharp
static void Main(string[] args)
{
    try
    {
        .
        .
        .
        SqlBulkCopy bc = new SqlBulkCopy("data source=localhost;initial catalog=Dukkan;integrated security=SSPI", SqlBulkCopyOptions.KeepIdentity);
        bc.DestinationTableName = "YEDEK_MailList";
        bc.ColumnMappings.Add("ID", "UserID");
        bc.ColumnMappings.Add("AD", "UserName");
        bc.ColumnMappings.Add("SOYAD", "UserLastName");
        bc.ColumnMappings.Add("MAIL", "MailAddress");

        bc.NotifyAfter = 1;
          bc.SqlRowsCopied+=new SqlRowsCopiedEventHandler(bc_SqlRowsCopied);
        bc.WriteToServer(dr);
        .
        .
        .
    }
    catch (SqlException ex)
    {
        Console.WriteLine(ex.Message);
        Console.ReadLine();
    }
}

static void bc_SqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
{
    Console.WriteLine("ŞU ANA KADAR "+e.RowsCopied.ToString() + " SATIR TAŞINDI...");
}
```

Burada, SqlBulkCopy nesnemize SqlRowsCopied olayını ekledikten sonra bu olayın gerçekleşmesi sonucu çalıştırılacak olan bc_SqlRowsCopied metodunda, o ana kadar taşınmış olan satır sayısını ekrana yazdırıyoruz. NotifyAfter özelliğine 1 değerini atadığımız için, olay metodumuz her bir satır hedef tabloya başarılı bir şekilde taşındıktan sonra tetiklenecektir. Sonuç aşağıdaki gibi olur.

![mk93_9.gif](/assets/images/2004/mk93_9.gif)

Şekil 9. SqlRowsCopied olayının işlenmesi sonucu.

Bu makalemizde, Ado.Net 2.0 ile gelen yeni kabiliyetlerden birisine değinmeye çalıştık. İlerleyen makalelerimizde Ado.Net 2.0' ın yeni özelliklerini incelemeye devam edeceğiz. Hepinize mutlu günler dilerim.