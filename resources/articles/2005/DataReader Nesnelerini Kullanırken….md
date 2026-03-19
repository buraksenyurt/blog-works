---
title: "DataReader Nesnelerini Kullanırken…"
pubDate: 2005-04-04 12:00:00
categories:
  - Ado.Net
tags:
  - Ado.Net
  - C#
  - .NET
  - ASP.NET
  - Caching
---

# DataReader Nesnelerini Kullanırken…
Değerli Okurlarım, Merhabalar.

Bir önceki makalemizde Command nesnelerini kullanırken dikkat etmemiz gereken noktalara değinmiştik. Bu makalemizde ise DataReader nesnelerini kullanırken bizlere avantaj sağlayacak tekniklere değinmeye çalışacağız. Önceki makalemizde olduğu gibi ağırlık olarak SqlDataReader nesnesini ve Sql veritanını kullanacağız. DataReader nesneleri bildiğiniz gibi, bağlantılı katman (connected-layer) üzerinde çalışmaktadır. Görevleri veri kaynağından, uygulama ortamına doğru belli bir akım üzerinden hareket edecek veri parçalarının taşınmasını sağlamaktır.

DataReader nesneleri ile veri almak bağlantısız katman (disconnected-layer) nesnelerine veri çekmekten çok daha hızlıdır. Çoğunlukla DataReader nesnelerinin kullanılmasının tercih edileceği durumlar vardır. Uygulamalarımız geliştirirken çoğu zaman bağlantılı katman ile bağlantısız katman nesneleri arasında seçim yapmakta zorlanırız. Aşağıdaki tablo "Ne zaman DataReader kullanırız?" sorusuna ışık tutan noktalara değinmektedir.

Eğer Windows veya Asp.Net uygulaması geliştiriyor ve birden fazla form (page) için veri bağlama (data-binding) gerçekleştirmiyorsanız,

Veriyi ara belleğe alma (caching) ihtiyacınız yok ise.

Eğer tablolarınız arasındaki ilişkileri (relations) uygulamalarınızda kullanmıyorsanız.

DataReader Kullanmayı Tercih Edin.

Gelelim DataReader nesnelerini kullanırken dikkat edeceğimiz altın noktalara. Bu teknikler uygulamalarımızın performansını arttıracak nitelikte olup aşağıdaki tabloda belirtilmektedir.

DataReader nesnelerini kullanırken açık Connection'ların kapatılmasını unutmayın.

Sorgu sonucu sadece tek bir satır döneceği kesin ise SingleRow tekniğini kullanın.

Toplu sorgular (Batch Queries) için Next Result tekniğini kullanın.

Binary veya Text bazlı alan verilerini okurken SequentialAccess tekniğini kullanın.

Şimdi bu teknikleri birer birer inceleyelim.

Açık Connection'ları Kapatmayı Unutmamak İçin

DataReader nesneleri açık ve geçerli bir Connection nesnesine ihtiyaç duyarlar. Lakin aynı Connection'ı kullanan DataReader nesneleri söz konusu ise, her bir DataReader'ın kullanılabilmesi için bir önceki DataReader'ın kullandığı Connection nesnesinin kapatılmış olması gerekir. (Bu aynı Connection nesnesini kullanan DataReader'lar var ise geçerlidir.) Örneğin aşağıdaki uygulama kodunu ele alalım;

```csharp
using System;
using System.Data;
using System.Data.SqlClient;

namespace DataReaderDikkat
{
    public class DbWork
    {
        private SqlConnection con;
        private SqlDataReader dr;

        /* CloseConnection kullanımına örnek metod.*/
        public DbWork(string conStr)
        { 
            con=new SqlConnection(conStr);
        }

        public SqlDataReader Results(string selectQuery)
        {
            SqlCommand cmd=new SqlCommand(selectQuery,con);
            con.Open();
            dr=cmd.ExecuteReader(); // Hatalı kullanım.
            return dr; 
        }      
    }
}
```

DbWork sınıfımız constructor metodunda bir SqlConnection nesnesi oluşturur. Results isimli metodumuz ise parametre olarak aldığı sorgu sonucu bir SqlDataReader nesnesi ile geri döndürür. Şimdi bu sınıfı kullanan aşağıdaki uygulamamızı ele alalım.

```csharp
using System;
using System.Data;
using System.Data.SqlClient;

namespace DataReaderDikkat
{
    class ClosingDataReader
    { 
        [STAThread]
        static void Main(string[] args)
        { 
            DbWork worker=new DbWork("data source=BURKI;database=Northwind;integrated security=SSPI");
            #region CloseConnection Kullanın.

            SqlDataReader drOrders=worker.Results("SELECT TOP 10 * FROM Orders");
            while(drOrders.Read())
            {
                Console.WriteLine(drOrders[0].ToString()+" "+drOrders[1].ToString());
            }
            drOrders.Close();
            SqlDataReader drCustomers=worker.Results("SELECT TOP 10 * FROM Customers");
            while(drCustomers.Read())
            {
                Console.WriteLine(drCustomers[0].ToString()+" "+drCustomers[1].ToString());
            }
            drCustomers.Close();
            #endregion
        }
    }
}
```

Uygulamamızı bu haliyle çalıştırdığımızda aşağıdaki istisnayı alırız.

![mk119_2.gif](images/mk119_2.gif)

Sebep ilk drOrders SqlDataReader nesnesinin kullandığı SqlConnection nesnesinin kapatılmamış olması ve bağlantının halen daha açık olarak kalmasıdır. Özellikle yukarıdaki gibi iş nesneleri üzerinden yürütülen sorgularda Connection nesnelerinin otomatik olarak kapatılmasını sağlamak için CommandBehavior numaralandırıcısının CloseConnection değerini kullanmayı unutmamak gerekir. Dolayısıyla DbWork sınıfımızdaki Results metodunu aşağıdaki gibi düzenlersek istediğimiz sonucu elde eder ve istisnanın üstesinden geliriz.

```csharp
public SqlDataReader Results(string selectQuery)
{
    SqlCommand cmd=new SqlCommand(selectQuery,con);
    con.Open();
    dr=cmd.ExecuteReader(CommandBehavior.CloseConnection); // Doğru Kullanım.
    // dr=cmd.ExecuteReader(); // Hatalı kullanım.
    return dr; 
}
```

![mk119_3.gif](images/mk119_3.gif)

Sorgu Sonucu Tek Bir Satır Döndüğü Kesin İse

Bazı durumlarda tablolardan dönen satır sayısının 1 olacağı kesindir. Bu satırlar çoğunlukla belirli key alanı üzerinden elde edilen parametrik sorguların sonucudur. Örneğin, benzersiz değer alan (unique), ve otomatik olarak artan alanların parametre olarak kullanıldığı sorgular göz önüne alabiliriz. Bu tarz sorgularda, bağlantısız katman nesnelerini kullanmak gereksiz yere kaynak tüketimine neden olacaktır. Böyle bir durumda DataReader nesneleri bağlantısız katman nesnelerine oranla çok daha performanslı ve hızlı çalışacaktır. Burada önemli olan DataReader ile dönen satırı okumak için ilgili Command nesnesinin ExecuteReader metoduna verilecek CommandBehavior numaralandırıcısının değeridir. Aşağıda bu tekniğin kullanımına bir örnek verilmiştir. Veritabanı işlemlerimizi topladığımız DbWork sınıfı basit olarak constructor metodu ile bir SqlConnection nesnesi örneklendirir. GetRow metodumuz ise gelen sorguya ve parametre değerine göre bulunan satırı okuyacak bir SqlDataReader nesnesini geriye döndürür.

```csharp
using System;
using System.Data;
using System.Data.SqlClient;

namespace DataReaderDikkat
{
    public class DbWork
    {
        private SqlConnection con;
        private SqlDataReader dr;

        /* CloseConnection kullanımına örnek metod.*/
        public DbWork(string conStr)
        { 
            con=new SqlConnection(conStr);
        }

        public SqlDataReader GetRow(string FindQuery,int orderID)
        {
            SqlCommand cmd=new SqlCommand(FindQuery,con);
            cmd.Parameters.Add("@OrderID",SqlDbType.Int);
            cmd.Parameters["@OrderID"].Value=orderID;
            con.Open();
            dr=cmd.ExecuteReader((CommandBehavior)40);
            return dr;
        }
    }
}
```

Burada ExecuteReader metodunun kullanılışına dikkatinizi çekerim. CommandBehavior numaralandırıcısının alacağı değerlerin sayısal karşılıkları vardır. SingleRow değerinin integer karşılığı 8, CloseConnection numaralandırıcısının integer karşılığı ise 32' dir. Dolayısıyla (CommandBehavior) 40, oluşturulan SqlDataReader nesnesine sadece tek bir satır döndüreceğini ve SqlDataReader nesnesi kapatıldığında SqlConnection nesnesinin de otomatik olarak kapatılacağını belirtir. Gelelim ana program kodlarımıza;

```csharp
using System;
using System.Data;
using System.Data.SqlClient;

namespace DataReaderDikkat
{
    class ClosingDataReader
    { 
        [STAThread]
        static void Main(string[] args)
        { 
            DbWork worker=new DbWork("data source=BURKI;database=Northwind;integrated security=SSPI");

            #region SingleRow Kullanın.
            SqlDataReader drFind=worker.GetRow("SELECT * FROM Orders WHERE OrderID=@OrderID",10248);
            drFind.Read();
            Console.WriteLine("BULUNAN SATIR "+drFind[0].ToString());
            drFind.Close();

            drFind=worker.GetRow("SELECT * FROM Orders WHERE OrderID=@OrderID",10249);
            drFind.Read();
            Console.WriteLine("BULUNAN SATIR "+drFind[0].ToString());
            drFind.Close();
            #endregion
        }
    }
}
```

![mk119_1.gif](images/mk119_1.gif)

Toplu Sorgula İçin DataReader Kullanın

Özellikle birden fazla sonuç kümesini (result set) almak istiyorsanız ve DataReader kullanmaya karar verdiyseniz en uygun yöntem NextResult metodunun uygulanmasıdır. Gerçek şu ki, böyle bir durumda birden fazla DataReader nesnesi peş peşe çalıştırılabilir. Aynı bu makalemizdeki ilk örneğimizde olduğu gibi. Eğer bu tarz sonuç kümelerini gerçekten arka arkaya alıyorsak ve aynı Connection'ı kullanıyorsak, birden fazla DataReader nesnesi kullandığımız için aynı Connection'ı kullanıyor olsakta veritabanına doğru birden fazla sayıda tur atmış oluruz. Çünkü her bir DataReader nesnesinden sonradan gelen DataReader nesnelerinin aynı Connection'ı kullanmalarına imkan sağlamamız için ilgili Connection'ları kapatmak gibi bir zorunluluğumuz vardır.

Oysaki bu sorguları Batch Query olarak hazırlarsak tek bir DataReader nesnesi ve tek bir Connection ile daha hızlı sonuç elde edebiliriz. Aynı aşağıdaki örnekte olduğu gibi. DbWork sınıfımıza bu sefer, Batch Query çalıştıracak bir metot ekledik. Metodumuz gelen string dizisi içindeki sorgu cümlelerini alıp bir StringBuilder yardımıyla birleştiriyor. Bu işlemin sonucu olarak "sorgu cümlesi 1;sorgu cümlesi 2;sorgu cümlesi 3;" tarzında bir query string'i oluşturuyoruz ki bu bizim Batch Qeury'mizdir. Daha sonra ilgili sorgu topluluğunu çalıştıracak bir SqlCommand nesnesi oluşturuyor ve bu komutu yürüterek elde ettiğimiz SqlDataReader nesnesini geriye döndürüyoruz.

```csharp
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace DataReaderDikkat
{
    public class DbWork
    {
        private SqlConnection con;
        private SqlDataReader dr;

        /* CloseConnection kullanımına örnek metod.*/
        public DbWork(string conStr)
        { 
            con=new SqlConnection(conStr);
        }
    
        public SqlDataReader BatchResults(string[] sorgular)
        {
            StringBuilder sbSorgular=new StringBuilder();
            for(int i=0;i<sorgular.Length;i++)
            {
                sbSorgular.Append(sorgular[i]+";"); 
            }
            SqlCommand cmd=new SqlCommand(sbSorgular.ToString(),con);
            con.Open();
            SqlDataReader dr=cmd.ExecuteReader(CommandBehavior.CloseConnection);
            return dr;
        }
    }
}
```

Gelelim uygulama kodlarımıza;

```csharp
using System;
using System.Data;
using System.Data.SqlClient;

namespace DataReaderDikkat
{
    class ClosingDataReader
    { 
        [STAThread]
        static void Main(string[] args)
        { 
            DbWork worker=new DbWork("data source=BURKI;database=Northwind;integrated security=SSPI");

            #region BatchQuery' lerde
            string[] sorguKumesi={"SELECT TOP 5 * FROM Orders","SELECT TOP 5 * FROM Customers","SELECT TOP 5 * FROM [Order Details]"};
            SqlDataReader drToplu=worker.BatchResults(sorguKumesi);
            do
            {
                while(drToplu.Read())
                {
                    Console.WriteLine(drToplu[0].ToString()+" "+drToplu[1].ToString());
                }
                Console.WriteLine("------------");
            }while(drToplu.NextResult());
            drToplu.Close();
            #endregion
        }
    }
}
```

![mk119_4.gif](images/mk119_4.gif)

Binary ve Text Tipindeki Alanları Okurken

Bazı tablolar içerisinde text veya binary tabanlı alanlar tutarız. Örneğin resim dosyalarının tablolarda binary olarak saklanması veya makalelerin html verisinin text tipli alanlar olarak saklanması gibi. Özellikle bu tarz alanları okurken DataReader nesnelerini kullanıyorsak, SequentialAccess tekniğini kullanmak bize avantaj sağlayabilir. Öyle ki bu tekniği uyguladığımızda ilgili satırın tamamı okunacağına bunun yerine bir stream oluşturulur. Siz bu stream'i kullanarak ilgili alana ait binary yada text veriyi okursunuz. Örneğin aşağıdaki kodlar ile Northwind database'inde yer alan Categories tablosundaki Text tipindeki Description alanının ilk 150 karakteri okunmaktadır.

```csharp
public SqlDataReader ReadText(string Query)
{
    SqlCommand cmd=new SqlCommand(Query,con);
    con.Open();
    dr=cmd.ExecuteReader((CommandBehavior)48); // Bu kez hem SequentialAccess hem de CloseConnection seçili.
    return dr; 
}
```

Metodumuzun kullanılışı ise aşağıdaki gibi olacaktır.

```csharp
SqlDataReader drText=worker.ReadText("SELECT CategoryName,Description,Picture FROM Categories");
char[] tampon=new char[150];
while(drText.Read()) 
{ 
    drText.GetChars(1,0,tampon,0,150);
    for(int i=0;i<150;i++)
    {
        Console.Write(tampon[i].ToString());
    }
    Console.WriteLine();
}
```

![mk119_5.gif](images/mk119_5.gif)

Bu makalemizde DataReader nesnelerini kullanırken dikkat edeceğimiz ve bize avantaj sağlayacak teknikleri incelemeye çalıştık. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.

[Örnek için tıklayın.](assets/DataReaderDikkat.zip)
