---
layout: post
title: "Ado.Net 2.0 ile Mars' a Ayak Basıyoruz"
date: 2004-09-20 12:00:00 +0300
categories:
  - ado-net-2-0
tags:
  - ado.net
  - asynchronous-programming
  - async
---
Bu makalemizde, MARS (Multiple Active Results Sets) kavramını incelemeye çalışacağız. MARS kavramı Ado.Net 2.0 mimarisine monte edilmiş yeni bir yapıdır. Bu yapının bize sağladığı avantajları anlayabilmek için, Ado.Net 1.0/1.1 sürümlerinin kabiliyetlerine ve kısıtlamalarına kısaca bir göz atmak gerekmektedir.

> Editörün Notu: Final sürümünde, ConnectionString katarı içerisinde MultipleActiveResultSets=true kullanılmaması halinde, MARS etkisi görülmemektedir. Varsayılan olarak MultipleActiveResultSets özelliğinin değeri false'dur. MARS tekniği, MultipleActiveResultSets değeri açıkça true'ya set edildiği takdirde çalışmaktadır.

Ado.Net'in önceki sürümlerinde, özellikle veri kümelerini uygulama ortamına çekmek istediğimizde kullanabileceğimiz iki temel teknik vardır. Birincisi, bağlantısız katmana veri alabilmek için DataAdapter, DataTable, DataSet gibi nesnelerin kullanılmasıdır. İkincisi ise, daha süratli veri çekmemizi sağlayan ve veri kaynağına olan bağlantının sql komutunun işleyişi boyunca açık olmasını gerektiren, DataReader nesnelerinin kullanıldığı tekniktir.

Bu tekniklerin hangisi kullanılırsa kullanılısın özellikle aynı bağlantı üzerinden eş zamanlı olarak yürütülen sql komutlarının çalıştırılmasına izin veren bir yapı yoktur. Yani, veri kaynağından bir DataReader nesnesi ile veri çekerken, aynı açık bağlantı üzerinden güncelleme, ekleme, silme gibi işlemleri içeren sql komutlarını çalıştıramayız. İşte bu imkansızlığı gidermek amacıyla Ado.Net mimarisi içine MARS, Asenkron Komut Yürütme ve ObjectSpace gibi yeni ve köklü değişiklikler eklenmiştir. Biz bu günkü makalemizde, aynı açık bağlantı üzerinden birden fazla kayıt kümesini uygulama ortamına çekmemize ve onlara erişmemize imkan sağlayan MARS (Multiple Active Results Sets) yapısını tanımaya çalışacağız.

MARS, özellikle aynı açık bağlantı üzerinden birden fazla satır kümesini elde edebileceğimiz sql komutlarının eş zamanlı olarak çalıştırılmasına imkan verir. Örneğin Ado.Net'in eski sürümlerinde şu senaryoyu göz önüne alalım; aynı veri kaynağında yer alan ve aynı bağlantı üzerinden erişeceğimiz üç kayıt kümesi olsun. Bu kayıt kümelerini 3 farklı SqlDataReader nesnesi ile ortama çekmek istediğimizi düşünelim. Bu işlemi simüle edebilmek için VS.NET 2003' de basit bir Console uygulaması geliştireceğiz. Uygulamamızın kodları aşağıdaki gibi olacaktır.

```csharp
using System;
using System.Data.SqlClient;
using System.Data;

namespace MultipleReader
{
    class AdoNet1nokta1de
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                SqlConnection con=new SqlConnection("data source=localhost;initial catalog=Northwind;integrated security=SSPI");

                SqlDataReader dr1;
                SqlDataReader dr2;
                SqlDataReader dr3;

                SqlCommand cmd1=new SqlCommand("SELECT * FROM Customers",con);
                SqlCommand cmd2=new SqlCommand("SELECT * FROM Orders",con);
                SqlCommand cmd3=new SqlCommand("SELECT * FROM [Order Details]",con);

                con.Open();
            
                dr1=cmd1.ExecuteReader();
                dr2=cmd2.ExecuteReader();
                dr3=cmd3.ExecuteReader();

                con.Close();
            }
            catch(Exception hata)
            {
                Console.WriteLine(hata.Message);
            }
        }
    }
}
```

Uygulamamızı çalıştırdığımızda çalışma zamanında bir istisnanın fırlatıldığını görürüz.

![mk95_1.gif](/assets/images/2004/mk95_1.gif)

Şekil 1. Ado.Net 1.0./1.1' deki durum.

Sorun şudur ki, ilk çalıştırılan SqlDataReader nesnesini kapatmadan diğerlerini çalıştırmamız da mümkün değildir. Nitekim SqlDataReader nesnesi, çalıştırdığı Sql komutunu ile verileri çekebilmek için ilgili bağlantının kendisi için tahsis edilmesini ve işi bitene kadar da başkaları tarafından kullanılmamasını gerektirir. Bu, tamamıyla Ado.Net'in mimarisinden kaynaklanan bir güçlüktür. Bununla birlikte, akla şöyle bir çözüm yolu gelebilir. Her bir SqlDataReader nesnesini kendi SqlConnection havuzu içinde çalıştırmak. Yani yukarıdaki kodu aşağıdaki gibi yazmak.

```csharp
SqlConnection con1=new SqlConnection("data source=localhost;initial catalog=Northwind;integrated security=SSPI");
SqlConnection con2=new SqlConnection("data source=localhost;initial catalog=Northwind;integrated security=SSPI");
SqlConnection con3=new SqlConnection("data source=localhost;initial catalog=Northwind;integrated security=SSPI");
SqlDataReader dr1;
SqlDataReader dr2;
SqlDataReader dr3;

SqlCommand cmd1=new SqlCommand("SELECT * FROM Customers",con1);
SqlCommand cmd2=new SqlCommand("SELECT * FROM Orders",con2);
SqlCommand cmd3=new SqlCommand("SELECT * FROM [Order Details]",con3);

con1.Open();
con2.Open();
con3.Open();

dr1=cmd1.ExecuteReader();
dr2=cmd2.ExecuteReader();
dr3=cmd3.ExecuteReader();

con1.Close();
con2.Close();
con3.Close();
```

Bu haliyle uygulama çalışacak ve herhangibir hata mesajı yada istisna vermeyecektir. Uygulanan bu teknik her ne kadar çözümmüş gibi görünsede, gereksiz yere kaynak tüketimine neden olmaktadır. Nitekim aynı bağlantı için üç kez SqlConnection nesnesi örneklendirilmiş ve sql sunucuna üç adet ayrı isimli ama aynı özellikte bağlantı hattı tesis edilmiştir. Dahası bu teknik kullanıldığı takdirde her çalışan Sql komutunun işleyişi sonlanana kadar, bir sonraki sql komutuna geçilemiyecektir. Bunu çözmek içinde, bu veri çekme işlemlerini ayrı metodlar halinde tanımlayıp çok-kanallı (multi-threading) programlama tekniklerini uygulayabiliriz. Ancak bu yaklaşımlar elbetteki çok verimli yada ölçeklenebilir değildir.

Çözüm,.Net mimarları tarafından Ado.Net 2.0' a yerleştirilmiştir ve MARS olarak adlandırılmıştır. Şimdi ilk yazdığımız uygulamayı birde.Net framework 2.0 ortamında geliştirelim. Kodlarımızı hiç değiştirmeyeceğiz. Bu kez Yukon üzerinde yer alan AdventureWorks isimli veritabanındaki iki tabloya aynı açık bağlantı üzerinden aynı zamanda erişmeye çalışacağız. İşte kodlarımız,

```bash
#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;

#endregion

namespace MarsEtkisi
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                SqlConnection con = new SqlConnection("data source=localhost;initial catalog=AdventureWorks;integrated security=SSPI");

                SqlCommand cmd1 = new SqlCommand("SELECT TOP 10 * FROM Person.address", con);
                SqlCommand cmd2 = new SqlCommand("SELECT TOP 3 * FROM Person.contact", con);

                SqlDataReader dr1;
                SqlDataReader dr2;

                con.Open();
                
                dr1 = cmd1.ExecuteReader();
                dr2 = cmd2.ExecuteReader();

                while (dr1.Read())
                {
                    Console.WriteLine(dr1["AddressLine1"]);
                }
                Console.WriteLine("-------------");
                while (dr2.Read())
                {
                    Console.WriteLine(dr2["FirstName"]);
                }
    
                con.Close();
                Console.WriteLine("-------------");
                Console.ReadLine();
            }
            catch (SqlException hata)
            {
                Console.WriteLine(hata.Message);
            }
        }
    }
}
```

Uygulamayı bu haliyle çalıştırdığımızda aşağıdaki ekran görüntüsünü elde ederiz. Görüldüğü gibi, SqlDataReader nesneleri, diğerinin kapatılmasına gerek duymadan veri kaynağından veri çekebilmiştir.

![mk95_2.gif](/assets/images/2004/mk95_2.gif)

Şekil 2. MARS Etkisi.

Bu örnek basit olarak MARS kavramını açıklamaktadır. Olayı daha iyi kavrayabilmek amacıyla aşağıdaki şekildende faydalanabiliriz.

![mk95_5.gif](/assets/images/2004/mk95_5.gif)

Şekil 3. MARS (Multiple Active Results Sets)

Şekildende görülebileceği gibi, bir veritabanında yer alan çeşitli sayıdaki tabloya ait veri kümelerini aynı uygulama ortamına, farklı ama tek bir ortak bağlantıyı kullanan DataReader nesneleri yardımıyla erişebilmemiz mümkündür.

Şimdi dilerseniz, MARS etkisini daha iyi görebileceğimiz başka bir örneği göz önüne alalım. Bu kez bir birleriyle bire-çok ilişkisi olan iki tabloyu ele alacağız. Bu tablolara ait verileri uygulama ortamına yine DataReader nesneleri yardımıyla çekeceğiz. Ancak işin ilginç yanı, bir DataReader nesnesi çalışırken ve satırları arasında iterasyon yaparken, diğerine parametre göndererek her bir satır için diğer DataReader'ında çalıştırılmasının sağlanmış olacağıdır. Bu örneği gerçekleştirmek için aşağıdaki kodları yazarak, Visual Studio.Net 2005' de basit bir Console uygulaması geliştirelim.

```csharp
static void Main(string[] args)
{
    try
    {
        SqlConnection con = new SqlConnection("data source=localhost;initial catalog=Dukkan;integrated security=SSPI");

        SqlCommand cmd1 = new SqlCommand("SELECT * FROM DepartmanSinif", con);
        SqlCommand cmd2 = new SqlCommand("SELECT * FROM PersonelDetay WHERE PersonelID=@PersonelID", con);
        cmd2.Parameters.Add("@PersonelID", SqlDbType.Int);

        SqlDataReader dr1;
        SqlDataReader dr2;

        con.Open();
        dr1 = cmd1.ExecuteReader();
        while (dr1.Read())
        {
            Console.WriteLine(dr1["Departman"]);
            Console.WriteLine(" ALTINDAKI ELEMANLARI....");
            cmd2.Parameters["@PersonelID"].Value = dr1["PersonelID"];
            dr2 = cmd2.ExecuteReader();
            while (dr2.Read())
            {
                Console.WriteLine(dr2["Ad"]+" "+dr2["Soyad"]+" "+dr2["Mail"]);
            }
            dr2.Close();
            Console.WriteLine("------------");
        }

        con.Close();
        Console.ReadLine();
    }
    catch (Exception hata)
    {
        Console.WriteLine(hata.Message);
        Console.ReadLine();
    }
}
```

Uygulamamızı çalıştırdığımızda ilişkili tablolara ait verilerin ekrana yazdırıldığını görürüz.

![mk95_3.gif](/assets/images/2004/mk95_3.gif)

Şekil 4. MARS Etkisi sayesinde içiçe çalışan SqlDataReader'lar.

Örneğimizde, SqlDataReader nesnelerini kullanarak Yukon sunucusu üzerinde yer alan ilişkili iki tabloya ait verileri console uygulamasından ekrana yazdırmaktayız. dr1 isimli SqlDataReader nesnesi yardımıyla DepartmanSinif isimli master tablodan satırları bir while döngüsü ile okumaktayız. Bu döngünün içerisinde ceyran eden olaylar ise bizim için MARS etkisinin gücünü bir kere daha göstermektedir. Nitekim, dr1 nesnesi ile master tablodan her bir satır okunduğunda bu satıra ait olan PersonelID alanının değeri, başka bir komuta parametre olarak gitmekte ve en önemliside bu komutu bir SqlDataReader nesnesi, diğeri halen daha açıkken yürüterek uygulama ortamına sonuç kümesini döndürmektedir. Aynı kod mantığını Ado.Net 1.1 verisyonunda Sql 2000 sunucusu üzerinde gerçekeştirmeye çalıştığımız aşağıdaki örneği göz önüne aldığımızda ise;

```csharp
try
{
    SqlConnection con = new SqlConnection("data source=localhost;initial catalog=Northwind;integrated security=SSPI");

    SqlCommand cmd1 = new SqlCommand("SELECT * FROM Orders", con);
    SqlCommand cmd2 = new SqlCommand("SELECT * FROM [Order Details] WHERE OrderID=@OrderID", con);
    cmd2.Parameters.Add("@OrderID", SqlDbType.Int);

    SqlDataReader dr1;
    SqlDataReader dr2;

    con.Open();
    dr1 = cmd1.ExecuteReader();
    while (dr1.Read())
    {
        Console.WriteLine(dr1["OrderDate"]);
        Console.WriteLine(" ALTINDAKI ELEMANLARI....");
        cmd2.Parameters["@OrderID"].Value = dr1["OrderID"];
        dr2 = cmd2.ExecuteReader();
        while (dr2.Read())
        {
            Console.WriteLine(dr2["ProductID"]+" "+dr2["UnitPrice"]+" "+dr2["Quantity"]);
        }
        dr2.Close();
        Console.WriteLine("------------");
    }

    con.Close();
    Console.ReadLine();
}
catch (Exception hata)
{
    Console.WriteLine(hata.Message);
    Console.ReadLine();
}
```

Bu kez Ado.Net 1.1' in mimarisi bu tarz bir işleme izin vermeyecek ve while döngüsü içinden elde edilen ilk tablo satırından sonra, ikinci SqlDataReader nesnesi yürütülmeye çalışıldığında, halen çalışmakta olan SqlDataReader'ın işlemini sonlandırması gerektiğini söyleyecektir.

![mk95_4.gif](/assets/images/2004/mk95_4.gif)

Şekil 5. Ado.Net 1.1'deki durum.

Görüldüğü gibi, MARS yapısı var olan sınıfların kabiliyetlerini arttırarak aynı açık bağlantı üzerinden birden fazla sayıda kayıt kümesine erişebilme imkanına sahip olmamızı sağlamıştır. İşin güzel yanı MARS etkisini uygulamalarımıza yansıtabilmek için, sınıflarımıza has çeşitli ayarlamalar yapamamıza gerek olmayışıdır. Aynen Ado.Net 1.1 uygulamaları yazar gibi yazabiliriz kodlarmızı. Elbetteki bu mimari değişiklikler sayesinde Ado.Net sınıflarının imkan ve kabiliyetleri önemli derecede artmıştır. Ado.Net 2.0 ile gelen tek yenilik MASR değildir. Bunun yanında, aynı anda birden fazla sql komutunu çalıştırmamıza yarayan asenkron komut yürütme teknikleride eklenmiştir. Bu konuyuda bir sonraki makalemizde incelemeye çalışacağız. Hepinize mutlu günler dilerim.