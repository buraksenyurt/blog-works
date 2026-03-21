---
layout: post
title: "Command Kavramı ve OleDbCommand Sınıfı"
date: 2004-02-23 12:00:00 +0300
categories:
  - ado-net
tags:
  - ado.net
  - oledb
  - sql
  - sql-server
  - oledbcommand
  - database
---
Bu makalemizde, Ado.Net mimarisi içinde çok önemli bir yere sahip olan Command kavramını ve OleDbCommand sınıfına ait en temel üyeleri incelemeye çalışacağız. Veritabanı uygulamaları geliştiren her programcı mutlaka, veri kaynağına doğru bir takım sorgu komutlarına ihtiyaç duymaktadır. Örneğin, veri kaynağındaki bir tabloya yeni bir satır eklemek için, veri kaynağı üzerinde bir tablo yaratmak için veya veri kaynağından belli şartlara uyan veri kümelerini çekmek için vb... Tüm bu işlemler için Ado.Net mimarisi bize, sql sorgularını barındırabileceğimiz ve geçerli bir bağlantı hattı üzerinden çalıştırabileceğimiz Command sınıfını sunmaktadır. Şu an itibariyle, Ado.Net mimarisi 4 temel Command sınıfı içerir. Bunlar, OleDbCommand, SqlCommand, OracleCommand ve OdbcCommand sınıflarıdır.

Bahsetmiş olduğumuz bu 4 Command sınıfıda temelde aynı görevler için tasarlanmışlardır. Farklılık sadece işaret ettikleri veri kaynkalarından ibarettir. Hepsinin görevleri ortaktır. Kullanıldıkları veri kaynakları üzerinde sql ifadelerinden oluşturulan komutları çalıştırmak. Bunun için elbette ihtiyacımız olan en önemli kaynak geçerli bir bağlantı hattının ve bu hattın eriştiği veri kaynağı için geçerli bir sql ifadesinin olmasıdır. Command sınıfları, çalıştıracakları sql ifadelerini temsil eden nesneler oldukları için Command (Komut) terimini bünyelerinde barındırırlar.

Bu makalemizde, Command sınıflarından OleDbCommand sınıfını incelemeye çalışacağız. OleDbCommand sınıfı, OleDb veri sağlayıcısı tarafından erişilebilen kaynaklar üzerinde sql ifadelerini çalıştırmamıza izin verir. OleDbCommand sınıfına ait nesne örneğini oluşturmak için kullanabileceğimiz üç farklı yol vardır. Bunlardan ilki OleDbCommand nesnesini new yapılandırıcısı ile her hangibir komut sözcüğü içermeden oluşturmaktır. Bu teknik için kullanılan yapıcı metodun prototipi aşağıdaki gibidir.

```csharp
public OdbcCommand();
```

Bu teknik ile oluşturulan bir komut nesnesi için bir takım özellikleri sonradan belirleyebiliriz. Öncelikle geçerli bir bağlantı nesnesine yani bir OleDbConnection nesnesine ihtiyacımız vardır. Diğer gereklilik ise, OleDbCommand sınıfı nesne örneğinin, çalıştıracağı sql ifadesidir. Bu amaçlar için OleDbCommand sınıfının Connection ve CommandText özelliklerini kullanırız. Yukarıdaki teknik ile oluşturduğumuz bir OleDbCommand nesnesi için Connection özelliği null, CommandText özelliği ise boş bir string'e işaret etmektedir. Bu tekniği daha iyi anlamak için basit bir örnek geliştirelim. Örneğin, Sql Sunucumuzda yer alan Friends isimli veri tabanındaki taboya bir satır veri gireceğimiz aşağıdaki sql ifadesini çalıştıracak bir komut tasarlamak istediğimizi varsayalım.

```csharp
Insert Into Siteler (Baslik,Adres,Resim,Icerik) Values('C#','www.csharpnedir.com','images/resim1.jpg','C# üzerine her türlü makale.')
```

Bunun için yazacağımız kodlar aşağıdadır.

```csharp
using System;

using System.Data.OleDb; /* OleDbCommand sınıfı bu isim uzayında yer almaktadır. */

namespace OleDbCmd1
{
  class Class1
  {
    static void Main(string[] args)
    {
      /* Önce geçerli bir bağlantı hattı oluşturmamız gerekiyor. */

      OleDbConnection con=new OleDbConnection("Provider=SQLOLEDB;data source=localhost;initial catalog=Friends;integrated security=sspi");

      OleDbCommand cmd=new OleDbCommand(); /* Komut nesnemiz oluşturuluyor. */

      cmd.Connection=con; /* Komut nesnesinin hangi bağlantı hattını kullanacağı belirleniyor. */

        cmd.CommandText="Insert Into Siteler (Baslik,Adres,Resim,Icerik) Values ('C#','www.csharpnedir.com','images/resim1.jpg','C# üzerine her türlü makale.')"; /* Komutun çalıştıracağı sql ifadesi belirleniyor. */
.
.
.
    }
  }
}
```

Görüldüğü gibi OleDbCommand sınıfını oluşturduktan sonra Connection ve CommandText özellikleri belirleniyor. Diğer yandan, bir OleDbCommand nesnesini aşağıda prototipi olan diğer yapıcı metodu ilede oluşturabiliriz.

```csharp
public OleDbCommand(string cmdText, OleDbConnection connection);
```

Bu yapıcı metodumuz parametre olarak sql ifadesini string veri tipinde ve bağlantı nesnesinide OleDbConnection sınıfı tipinde almaktadır. Bu haliyle yukarıda yazdığımız kodları dahada kısaltabiliriz.

```csharp
using System;

using System.Data.OleDb; /* OleDbCommand sınıfı bu isim uzayında yer almaktadır. */

namespace OleDbCmd1
{
  class Class1
  {
    static void Main(string[] args)
    {
      /* Önce geçerli bir bağlantı hattı oluşturmamız gerekiyor. */

      OleDbConnection con=new OleDbConnection("Provider=SQLOLEDB;data source=localhost;initial catalog=Friends;integrated security=sspi");

      OleDbCommand cmd=new OleDbCommand("Insert Into Siteler (Baslik,Adres,Resim,Icerik) Values('C#','www.csharpnedir.com','images/resim1.jpg','C# üzerine her türlü makale.')",con);

.
.
.
    }
  }
}
```

OleDbCommand nesnesinin oluşturmak için bahsettiğimiz bu iki yol dışında kullanabileceğimiz iki aşırı yüklenmiş metod daha vardır. Bunların protoipleride aşağıdaki gibidir.

```csharp
public OleDbCommand(string cmdText);
```

Bu prototip sadece sql ifadesini almaktadır. Komut nesnesine ait diğer özellikler (Connection gibi) sonradan belirlenir.

```csharp
public OleDbCommand(string cmdText, OleDbConnection connection, OleDbTransaction transaction);
```

Bu prototip ise, komut ifadesi ve bağlantı nesnesi haricinde birde OleDbTransaction nesnesi tipinden bir parametre alır. Bu prototip çoğunlukla, bir iş parçacığı içine alınmak istenen komut nesneleri için idealdir. Bir OleDbCommand nesnesi oluşturabilmek için kullanabileceğimiz son yol ise OleDbConnection sınıfına ait CreateCommand metodunun aşağıdaki örnekte olduğu gibi kullanılmasıdır.

```csharp
using System;

using System.Data.OleDb; /* OleDbCommand sınıfı bu isim uzayında yer almaktadır. */

namespace OleDbCmd1
{
  class Class1
  {
    static void Main(string[] args)
    {
      /* Önce geçerli bir bağlantı hattı oluşturmamız gerekiyor. */

      OleDbConnection con=new OleDbConnection("Provider=SQLOLEDB;data source=localhost;initial catalog=Friends;integrated security=sspi");

      OleDbCommand cmd=con.CreateCommand();
      cmd.CommandText="Insert Into Siteler (Baslik,Adres,Resim,Icerik) Values('C#','www.csharpnedir.com','images/resim1.jpg','C# üzerine her türlü makale.')";
.
.
.
    }
  }
}
```

Bu teknikte, geçerli olan bağlantı nesnesinin tesis ettiği hat üzerinde çalışacak OleDbCommand sınıfı nesne örneği, CreateCommand metodu ile oluşturulmuştur. Bu adımdan sonra tek yapılması gereken CommandText özelliğine sql cümleciğini atamak olucaktır.

Tüm bu teknikler bir OleDbCommand sınıfı nesne örneğini oluşturmak içindir. Ancak komutu çalıştırmak için henüz bir adım atmış değiliz. Bu haliye program kodlarımız derlense dahi hiç bir işe yaramıyacaktır. Nitekim, OleDbCommand nesnelerinin temsil ettiği sql cümleciklerini çalıştırmamız gerekmektedir. Bu amaçla kullanabileceğimiz üç OleDbCommand metodu vardır. Bunlar; ExecuteNonQuery, ExecuteReader ve ExecuteScalar metodlarıdır. Üç metodda farklı amaçlar ve performanslar için kullanılır. Bu amaçlar, çalıştırmak istediğimiz sql ifadesine göre değişiklik göstermektedir. Söz gelimi, yukarıdaki kod parçalarında yer alan sql ifadesi DDL (Data Defination Language- Veri Tanımlama Dili) komutlarından birisidir. Benzer şekilde update, delete sorgularıda böyledir. Diğer taraftan DML (Data Manipulation Language- Veri İdare Dili) komutları dediğimiz Create Table, Alter Table gibi komutlarda mevcuttur. Bu iki kategoriye ait komutlar, etki komutları olarakta adlandırılırlar. Hepsinin ortak özelliği geriye sonuç döndürmemeleridir. Tamamiyle veri kaynağı üzerinde bir takım sonuçların doğmasına yardımcı olurlar. İşte bu tip komut cümlecikleri için, ExecuteNonQuery metodu kullanılır. Bu metodun prototipi aşağıdaki gibidir.

```csharp
public virtual int ExecuteNonQuery();
```

Bu metod görüldüğü gibi int veri tipinden bir tamsayıyı geri döndürür. Bu sayı komutun çalıştırılması sonucu etkilenen kayıt sayısını ifade etmektedir. Dolayısıyla bu metod, DDL ve DML komutları için geliştirilmiştir diyebiliriz. Örneğin, yukarıdaki kod parçalarını hayat geçirelim. Bunun için aşağıdaki kodları yazacağız.

```csharp
using System;

using System.Data.OleDb; /* OleDbCommand sınıfı bu isim uzayında yer almaktadır. */

namespace OleDbCmd1
{
  class Class1
  {
    static void Main(string[] args)
    {
      /* Önce geçerli bir bağlantı hattı oluşturmamız gerekiyor. */

      OleDbConnection con=new OleDbConnection("Provider=SQLOLEDB;data source=localhost;initial catalog=Friends;integrated security=sspi");

      OleDbCommand cmd=new OleDbCommand("Insert Into Siteler (Baslik,Adres,Resim,Icerik) Values('C#','www.csharpnedir.com','images/resim1.jpg','C# üzerine her türlü makale.')",con);

      try
      {
          con.Open(); /* Bağlantımızı açıyoruz.*/
          int sonuc=cmd.ExecuteNonQuery(); /* Komutumuzu çalıştırıyoruz.ExecuteNonQuery metodunun döndüreceği değeri tam sayı tipindeki sonuc değişkenine atıyoruz.*/

          Console.WriteLine(sonuc.ToString()+" Kayıt Girildi...");
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

Uygulamamızı çalıştırdığımızda aşağıdaki sonucu alırız.

![mk56_1.gif](/assets/images/2004/mk56_1.gif)

Şekil 1. ExecuteNonQuery sonucu geri dönen değer.

Şimdi birde aşağıdaki örneğe bakalım. Bu kez elimizde, tablomuzun tüm satırlarındaki Resim alanlarının değerlerinin sonuna img ifadesini eklyecek sql ifadesini içeren bir OleDbCommand nesnemiz olsun.

```csharp
using System;

using System.Data.OleDb; /* OleDbCommand sınıfı bu isim uzayında yer almaktadır. */

namespace OleDbCmd1
{
  class Class1
  {
    static void Main(string[] args)
    {
      OleDbConnection con=new OleDbConnection("Provider=SQLOLEDB;data source=localhost;initial catalog=Friends;integrated security=sspi");

      OleDbCommand cmdUpdate=new OleDbCommand("Update Siteler Set Resim=Resim+'img' ",con);

      try
      {
         con.Open();
         int Guncellenen=cmdUpdate.ExecuteNonQuery();
         Console.WriteLine(Guncellenen.ToString()+" Kayıt Güncellendi");
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

Bu durumda aşağıdaki sonucu alırız.

![mk56_2.gif](/assets/images/2004/mk56_2.gif)

Şekil 2. Update komutu sonucu ExecuteNonQuery'nin döndürdüğü değer.

Görüldüğü gibi tablomuzdaki 5 kayıdın hepsi güncellenmiş ve ExecuteNonQuery geriye 5 değerini döndürmüştür. Bu çalıştırılan komut sonucu etkilenen kayıt sayısını öğrenmek için iyi ve etkili bir yoldur. ExecuteNonQuery metodu ile ilgili unutulmaması gereken nokta, bu metodun geriye her hangibir sonuç kümesi, her hangibir çıkış parametresi veya çıkış değeri döndürmemesidir. Elbette uygulamalarımızda, veri kaynaklarından veri kümeleri çekme ihtiyacını hissederiz. Böyle bir durumda ise, ExecuteReader metodunu kullanabiliriz. ExecuteReader metodu, çalıştırılan komut sonucu elde edilen sonuç kümesinden bir OleDbDataReader nesnesi için veri akışını sağlar. OleDbDataReader nesnesinin benzeri olan SqlDataReader nesnesi ve ExecuteReader metodunun kullanımını, SqlDataReader nesneleri ile ilgili makelelerimizde incelediğimiz için bu metodun nasıl kullanıldığına tekrar değinmiyorum.

OleDbCommand sınıfına ait bir diğer veri elde etme metodu ExecuteScalar metodudur. Prototipi aşağıdaki gibi olan bu metod sadece tek alanlık veri döndüren sql sorguları için kullanılır.

```csharp
public virtual object ExecuteScalar();
```

Örneğin tablomuzdaki kayıt sayısının öğrenmek istiyoruz veya tablomuzdaki ucretler adlı alanda yer alan işçi ücretlerinin ortalamasının ne olduğunu bilmek istiyoruz yada primary key alanı üzerinden arama yaptığımız bir satıra ait tek bir sütunun değerini elde etmek istiyoruz. Bu tarz durumlarda, çalışıtırılacak olan komut için bilgileri ExecuteReader metodu ile almak veya bilgileri bir DataSet kümesi içine almak vb... sistem kaynaklarının gereksiz yer harcanmasına ve perfrormansın olumsuz şekilde etkilenerek azalmasına neden olur. Çare ExecuteScalar metodunu kullanmaktır. Örneğin;

```csharp
using System;

using System.Data.OleDb; /* OleDbCommand sınıfı bu isim uzayında yer almaktadır. */

namespace OleDbCmd1
{
  class Class1
  {
    static void Main(string[] args)
    {
      OleDbConnection con=new OleDbConnection("Provider=SQLOLEDB;data source=localhost;initial catalog=Friends;integrated security=sspi");

      OleDbCommand cmd=new OleDbCommand("Select Baslik From Siteler Where ID=8",con);

      OleDbCommand cmdToplamSite=new OleDbCommand("Select Count(*) From Siteler",con);

      try
      {
         con.Open();
         Console.WriteLine("ID=8 olan satırın Baslik alanının değeri: "+cmd.ExecuteScalar().ToString());

         Console.WriteLine("Site Sayısı: "+cmdToplamSite.ExecuteScalar().ToString());
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

![mk56_3.gif](/assets/images/2004/mk56_3.gif)

Şekil 3. ExecuteScalar Sonucu.

Bu örnekte, Siteler isimli tablomuza ID değeri 8 olan satırın sadece Baslik isimli alanının değerini veren bir komut nesnesi ve Siteler tablsundaki satır sayısını veren başka bir komut nesnesi kullanılmıştır. Her iki sql ifadeside tek bir hücreyi sonuç olarak döndürmektedir. Eğer sql ifadenizden birden fazla sütun alıyorsanız ve bu ifadeyi ExecuteScalar ile çalıştırıyorsanız, ilk satırın ilk sütunu haricindeki tüm veriler göz ardı edilecektir. Söz gelimi yukarıdaki örneğimizde, cmd OleDbCommand nesnesinin CommandText ifadesini aşağıdaki gibi değiştirelim.

```csharp
OleDbCommand cmd=new OleDbCommand("Select * From Siteler",con);
```

Bu durumda aşağıdaki sonucu elde ederiz.

![mk56_4.gif](/assets/images/2004/mk56_4.gif)

Şekil 4. ExecuteScalar sadece ilk hücreyi döndürür.

Görüldüğü gibi sonuç olarak, ilk satırın ilk alanının değeri elde edilmiştir. (ID alanının değeri.) OleDbCommand sınıfı ile veri kaynağında yer alan bir saklı yordamıda (Stored Procedure) çalıştırabiliriz. Bu durumda CommandText olarak bu saklı yordamın adını girmemiz yeterli olucaktır. Ancak, çalıştırılacak olan komutun bir saklı yordamı çalıştıracağını belirtmemiz gerekmektedir. İşte bu noktada OleDbConnection sınıfı nesne örneğinin CommandType özelliğinin değerini belirtmemiz gerekir.

```csharp
public virtual CommandType CommandType {get; set;}
```

Prototipi yukarıdaki gibi olan bu özellik, CommandType numaralandırıcısı türünden 3 değer alabilir. Bu değerler ve ne işe yaradıkları aşağıdaki tabloda belirtilmiştir.

CommandType Değeri
Açıklaması

Text
Sql ifadelerini çalıştırmak için kullanılır. Bu aynı zamanda OleDbCommand sınıfına ait nesne örnekleri için varsayılan değerdir.

StoredProcedure
Veri kaynağında yer alan bir Saklı Yordam çalıştırılmak istendiğinde, CommandType değerine StoredProcedure verilir.

TableDirect
CommandType özelliğine bu değer atandığında, CommandText özelliği tablo adını alır. Komut çalıştırıldığında çalışan sql ifadesi "Select * From tabloadi" ifadesidir. Böylece belirtilen tablodaki tüm kayıtlar döndürülmüş olur.

Tablo 1. CommandType numaralandırıcısının değerleri.

Şimdi bu özelliklerin nasıl kullanılacağını tek tek incelemeye çalışalım. Öncelikle TableDirect değerinden başlayalım. Tek yapmamız gereken tüm satırlarını elde etmek istediğimiz tablo adını CommandText olarak belirtmek olucaktır. İşte örneğimiz.

```csharp
using System;
using System.Data.OleDb;
using System.Data;
namespace OleDbCmd1
{
  class Class1
  {
    static void Main(string[] args)
    {
      OleDbConnection con=new OleDbConnection("Provider=SQLOLEDB;data source=localhost;initial catalog=Friends;integrated security=sspi");

      OleDbCommand cmd=new OleDbCommand("Makale",con); /* Komut söz dizimi olarak tüm satırlarını almak istediğimi veri tablosunun adını giriyoruz. */
      cmd.CommandType=CommandType.TableDirect; /* Komut tipimizi TableDirect olarak ayarlayıp, komut nesnemizin sql ifadesinin Select * From Makale olmasını sağlıyoruz. */

      try
      {
         con.Open();
      /* Bir OleDbDataReader nesnesi tanımlayıp, komutumuzu ExecuteReader metodu ile çalıştırarak, sonuç kümesine ait satırlardaki Konu alanının değerlerini ekrana yazdırıyoruz. */

         OleDbDataReader dr;
         dr=cmd.ExecuteReader();
         while(dr.Read())
         {
            Console.WriteLine(dr["Konu"].ToString());
         }
         dr.Close();
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

Uygulamamızı çalıştırdığımızda, Makale tablosundaki tüm satırların alındığı sonuç kümesi içinden, Konu alanlarının değerlerinin ekrana yazdırıldığını görürüz.

![mk56_5.gif](/assets/images/2004/mk56_5.gif)

Şekil 5. TableDirect sonucu.

Ancak burada istisnai bir durum vardır. Bazı tablo isimleri içinde boşluklar olabilir. Örneğin "Site Adlari" isminde bir tablomuz olduğunu düşünelim. Böyle bir durumda TableDirect değerinin sonucunda bir istisnanın fırlatıldığını görürüz. Yukarıdaki örneğimizde tablo adı olarak Site Adlari verdiğimizi düşünelim.

```csharp
using System;
using System.Data.OleDb;
using System.Data;

namespace OleDbCmd1
{
  class Class1
  {
    static void Main(string[] args)
    {
      OleDbConnection con=new OleDbConnection("Provider=SQLOLEDB;data source=localhost;initial catalog=Friends;integrated security=sspi");

      OleDbCommand cmd=new OleDbCommand("Site Adlari",con);

      cmd.CommandType=CommandType.TableDirect;
      try
      {
         con.Open();
         OleDbDataReader dr;
         dr=cmd.ExecuteReader();

         while(dr.Read())
         {
            Console.WriteLine(dr["Baslik"].ToString());
         }
         dr.Close();
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

![mk56_6.gif](/assets/images/2004/mk56_6.gif)

Şekil 6. Tablonun olmadığı söyleniyor.

Bunun sebebi OleDbCommand nesnesinin tablo isminde yer alan boşlukları anlamamış olmasıdır. Bu nedenle aynı ifadeyi aşağıdaki şekilde değiştirmemiz gerekmektedir.

```csharp
OleDbCommand cmd=new OleDbCommand("[Site Adlari]",con);
```

Bu durumda uygulamanın sorunsuz çalıştığını görürüz. OleDbCommand sınıfının CommandType özelliğinin diğer değeri ise StoredProcedure'dür. Bu veri kaynağındaki saklı yordamlarının çağırılması için kullanılmaktadır. Bir saklı yordam kendisi için parametreler alabileceği gibi geriye değerlerde döndürebilir. Örneğin, Primary Key alanları üzerinden arama yapılan sorgularda Saklı Yordamların kullanılması son derece verimlidir. Nitekim kullanıcıların aramak için girdikleri her ID değeri için ayrı bir select sorgusu oluşturmak yerine, veri kaynağında bir nesne olarak yer alan ve ID değerini parametre olarak alan hazır, derlenmiş bir select ifadesini çalıştırmak daha verimli olucaktır. Örneğin Makale isimli tablomuzdan ID alanı 41 olan bir satırı elde etmek istiyoruz. Bu durumda, buradaki saklı yordamımıza bu ID değerini geçirmemiz ve dönen sonuçları almamız gerekiyor. Öncelikle saklı yordamımıza bir göz atalım.

```text
ALTER PROCEDURE dbo.MakaleBul
(
@MakaleID int
)
AS
SELECT * FROM Makale Where ID=@MakaleID
RETURN
```

Bu saklı yordam ID değerine @MakaleID isminde bir parametre alır. Bu parametre değeri ile tablodaki ilgili satır aranır.Bu satır bulunduğunda, geriye bu satırdaki tüm alanlar aktarılır. Şimdi uygulamamızda bunun nasıl kullanacağımızı görelim. Öncelikle OleDbCommand sınıfı nesne örneğimizi bu saklı yordam ismini CommandText değeri olacak şekilde oluştururuz. Daha sonra, CommandType özelliğine StoredProcedure değerini veririz. Böylece, CommandText özelliğindeki söz diziminin bir saklı yordamı temsil ettiğini ifade etmiş oluruz. Geriye parametre kalır. OleDbCommand sınıfı, parametrelerini OleDbParameterCollection koleksiyonunda birer OleDbParameter nesnesi olarak tutmaktadır. Dikkat etmemiz gereken nokta parametre adının, saklı yordamdaki ile aynı olmasıdır. Dilerseniz kodlarımızı yazalım ve bu işlemin nasıl yapıldığını görelim.

```csharp
using System;
using System.Data.OleDb;
using System.Data;

namespace OleDbCmd1
{
  class Class1
  {
    static void Main(string[] args)
    {
      OleDbConnection con=new OleDbConnection("Provider=SQLOLEDB;data source=localhost;initial catalog=Friends;integrated security=sspi");

      OleDbCommand cmd=new OleDbCommand("MakaleBul",con); /* Çalıştırılacak sql ifadesi olarak saklı yordamımızın ismini giriyoruz. */

      cmd.CommandType=CommandType.StoredProcedure; /* CommandText ifadesinin, geçerli bağlantı nesnesinin temsil ettiği veri kaynağındaki bir saklı yordamı ifade ettiğini belirtiyor. */
      cmd.Parameters.Add("@MakaleID",OleDbType.Integer); /* Parametremiz oluşturuluyor. Adı @MakaleID, saklı yordamımızdaki ile aynı. Parametre tipi integer, nitekim Saklı Yordamımızdaki tipide int.*/
      cmd.Parameters["@MakaleID"].Value=41; /* Parametremizin değeri veriliyor. */

      try
      {
         con.Open();
         OleDbDataReader dr;
         dr=cmd.ExecuteReader();
         while(dr.Read())
         {
            Console.WriteLine(dr["ID"].ToString()+"-"+dr["Konu"].ToString()+"-"+dr["Tarih"].ToString());
         }
         dr.Close();
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

Bu uygulamayı çalıştırdığımızda, aşağıdaki sonucu elde ederiz.

![mk56_7.gif](/assets/images/2004/mk56_7.gif)

Şekil 7. Saklı Yordamın çalışmasının sonucu.

CommandType özelliğinin Text değeri varsayılandır. Text değeri, CommandText için yazılan sql ifadelerinin çalıştırılmasında kullanılır. Aslında bir saklı yordamı bu şekildede çağırabiliriz. Yani bir saklı yordamı, OleDbCommand sınıfının CommandType özelliğini Text olarak bırakarakta çağırabiliriz. Bunun için "{CALL MakaleBul (?)}" söz dizimini aşağıdaki örnekte olduğu gibi kullanırız. Sonuç aynı olucaktır. Burada, CALL ifadesinde parametrenin? işareti ile temsil edildiğine dikkat edin. SqlCommand sınıfında bu parametreler @ParametreAdı olarak kullanılır.

```csharp
OleDbConnection con=new OleDbConnection("Provider=SQLOLEDB;data source=localhost;initial catalog=Friends;integrated security=sspi");

OleDbCommand cmd=new OleDbCommand("{CALL MakaleBul(?)}",con);

cmd.Parameters.Add("@MakaleID",OleDbType.Integer);
cmd.Parameters["@MakaleID"].Value=41;
try
{
  con.Open();
  OleDbDataReader dr;
  dr=cmd.ExecuteReader();
  while(dr.Read())
  {
    Console.WriteLine(dr["ID"].ToString()+"-"+dr["Konu"].ToString()+"-"+dr["Tarih"].ToString());
  }
  dr.Close();
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

Gelelim, OleDbCommand sınıfının diğer önemli üyelerine. Bu üyelerden birisi CommandTimeOut özelliğidir. Bir sql ifadesi, OleDbCommand nesnesi tarafından çalıştırılıdığında, ilk sonuçlar döndürülene kadar belli bir süre geçer. İşte bu sürenin uzunluğu CommandTimeOut özelliği tarafından belirlenir. Başlangıç değeri olarak 30 saniyeye ayarlanmıştır. Bu süre zarfında komut çalıştırılması ile birlikte herhangibir sonuç döndürülemez ise, bir istisna fırlatılır. Bu özellik aslında, ilk sonuçların dönmeye başlaması için ne kadar süre bekleneceğini belirtir. Düşününkü, bir OleDbAdapter nesnesinin çalıştırdığı bir OleDbCommand nesnemiz var. Bu komutun işaret ettiği sql ifadesi çalıştırıldıktan sonra, CommandTimeout'ta belirtilen sürede bir sonuç dönmez ise, süre aşımı nedeni ile bir istisna oluşur. Ancak burada özel bir durum vardır. Eğer, bu süre içinde belli bir sayıda kayıt (en azından tek satırlık veri) geri dönmüş ise, CommandTimeout süresi geçersiz hale gelir. Böyle bir durumda OleDbDataAdapter tarafından döndürülmeye çalışılan kayıtların hepsi elde edilene kadar komut çalışmaya devam eder. Bu bazen bir ömür boyu sürebilir. Buda tam anlamıyla bir ironidir.

Daha önceki makalelerimizde hatırlayacağınız gibi, iş parçacıklarının çok katlı mimaride önemli bir yeri vardır. Bir OleDbCommand sınıfı nesne örneğini bir iş parçacığı olarak çalıştırmak için, Transaction özelliğine, iş parçacığını üstlenen OleDbTransaction nesnesini atamamız gerekir. Bununla ilgili basit bir örnek aşağıda verilmiştir.

```csharp
using System;
using System.Data.OleDb;
using System.Data;

namespace OleDbCmd1
{
  class Class1
  {
    static void Main(string[] args)
    {
      OleDbConnection con=new OleDbConnection("Provider=SQLOLEDB;data source=localhost;initial catalog=Friends;integrated security=sspi");

      OleDbCommand cmd=new OleDbCommand("Insert Into [Site Adlari] (Baslik,Adres,Resim,Icerik) Values('C#','www.csharpnedir.com','images/resim1.jpg','C# üzerine her türlü makale.')",con);

      con.Open();
      OleDbTransaction trans=con.BeginTransaction(); /* Transaction'ımız, geçerli bağlantımızı için yaratılıyor. */
      cmd.Transaction=trans; /* Komutumuzun tanımlanan bağlantı için açılmış transaction içinde bir iş parçacığı olarak çalışacağı belirleniyor. */
      int sonuc=cmd.ExecuteNonQuery();
      if(sonuc==1)
      {
         trans.Commit(); /* Komut başarılı bir şekilde çalıştırılmışsa Commit ile tüm işlemler onaylanıyor. */
         Console.WriteLine(sonuc.ToString()+" Kayıt Girildi...");
      }
      else
      {
         trans.Rollback(); /* Komut başarısız ise tüm işlemler geri alınıyor. */
      }
    }
  }
}
```

OleDbCommand sınıfının kullanıldığı diğer önemli bir yerde OleDbDataAdapter sınıfıdır. Nitekim bu sınıfın içerdiği SelectCommand, UpdateCommand, DeleteCommand, InsertCommand özellikleri, OleDbCommand sınıfı türünden nesneleri değer olarak alırlar. Bu konuyu ilerliyen makalelerimizde OleDbDataAdapter sınıfını işlerken incelemeye çalışacağız.

Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.