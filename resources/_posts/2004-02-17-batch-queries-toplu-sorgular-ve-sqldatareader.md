---
layout: post
title: "Batch Queries (Toplu Sorgular) ve SqlDataReader"
date: 2004-02-17 12:00:00 +0300
categories:
  - ado-net
tags:
  - ado.net
  - batch-queries
  - sqldatareader
---
Bu makalemizde, toplu sorguların, SqlDataReader sınıfı ile nasıl okunabileceğini incelemeye çalışacağız. Bildiğiniz gibi SqlDataReader nesneleri, bir select sorgusunu çalıştıran SqlCommand sınıfına ait, ExecuteReader metodu ile oluşturulmaktaydı. Çalıştırılan sorgu sonucu elde edilen kayıt kümesinde sadece okunabilir ve ileri yönlü hareket etmemize imkan sağlayan SqlDataReader sınıfı, belli bir t anında veri kanağından sadece tek bir satırı okumamıza izin vermektedir. Bu yönden bakıldığında, SqlDataReader sınıfı, verileri hızlı ve verimli bir şekilde okumamıza imkan sağlamaktadır. Örneğin aşağıdaki kod satırları ile, Sql sunucumuzda yer alan makale isimli tablodaki tüm satırlar okunarak ekrana yazdırılmıştır.

```csharp
using System;
using System.Data.SqlClient;
using System.Data;
namespace BatchQueries
{
    class Class1
    {
        static void Main(string[] args)
        {
            SqlConnection conFriends=new SqlConnection("data source=localhost;initial catalog=Friends;integrated security=sspi"); /* Sql sunucumuza olan bağlantı hattını tesis edicek SqlConnection nesnemiz tanımlanıyor.*/
            SqlCommand cmdMakale=new SqlCommand("Select * From Makale",conFriends); /* Makale tablosundaki tüm satırları alıcak sql sorgusunu çalıştıracak SqlCommand nesnemiz oluşturuluyor. */
            SqlDataReader drMakale; /* SqlDataReader nesnemiz tanımlanıyor. */
            conFriends.Open(); /*Bağlantımız açılıyor. */
            drMakale=cmdMakale.ExecuteReader(CommandBehavior.CloseConnection); /* Komutumuz çalıştırılıyor ve sonuç kümesinin başlangıcı SqlDataReader nesnemize aktarılıyor. */
            /* İleri yünlü olarak, SqlDataReader nesnemiz ile, sonuç kümesindeki satırlar okunuyor ve 1 indisli alanın değeri ekrana yazdırılıyor. */
            while(drMakale.Read())
            {
                Console.WriteLine(drMakale[1]);
            }
            drMakale.Close(); /* Bağlantımız kapatılıyor. */
        }
    }
}
```

Bu kodları çalıştırdığımızda karşımızıza aşağıdakine benzer bir sonuç çıkacaktır.

![mk55_1.gif](/assets/images/2004/mk55_1.gif)

Şekil 1. Programın Çalışmasının Sonucu.

SqlDataReader nesnesinin kullanımına kısaca değindikten sonra makelemizin asıl konusu olan toplu sorgulara değinelim. Toplu sorgular birbirlerinden noktalı virgül ile ayrılmış sorgulardır. Birden fazla sorguyu bu şekilde bir araya getirerek işlemlerin tek bir hamlede başlatılmasını ve gerçekleştirilmesini sağlamış oluruz. Bu sorgu topluluklarını, eski dostumuz Ms-Dos işletim sistemindeki bat uzantılı Batch dosyalarına benzetebilirsiniz. Sorun şu ki, yukarıdaki tarzda bir SqlDataReader kullanımını bir toplu sorguya uyguladığımızda, sadece ilk sorgunun çalışıtırılacak olmasıdır. Örneğin aşağıdaki biri bir toplu sorgumuz olduğunu düşünelim;

Select * From Makale;Select * From Kitap;Select * From Siteler

Bu toplu sorguda arka arkaya üç select sorgusu yer almaktadır. Makale, Kitap ve Siteler tablolarının tüm sütunları talep edilmektedir. Yukarıdaki kod tekniğini böyle bir toplu sorguya uyguladığımızı düşünelim.

```csharp
using System;
using System.Data.SqlClient;
using System.Data;
namespace BatchQueries
{
    class Class1
    {
        static void Main(string[] args)
        {
            SqlConnection conFriends=new SqlConnection("data source=localhost;initial catalog=Friends;integrated security=sspi");
            string sorgu="Select * From Makale;Select * From Kitap;Select * From Siteler";
            SqlCommand cmdMakale=new SqlCommand(sorgu,conFriends);
            SqlDataReader drMakale;
            conFriends.Open();
            drMakale=cmdMakale.ExecuteReader(CommandBehavior.CloseConnection);
            while(drMakale.Read())
            {   
                Console.WriteLine(drMakale[1]);
            }
            drMakale.Close();
        }
    }
}
```

Uygulamamızı çalıştırdığımıza sadece Makale isimli tabloya ait verileri okuyabildiğimizi ve ekrana yazdırıldıklarını görürüz.

![mk55_1.gif](/assets/images/2004/mk55_1.gif)

Şekil 2. Sadece ilk kayıt kümesi okundu.

Problem şudur. Toplu sorgumuz, birbirinden farklı üç kayıt kümesi getirmektedir. Bu nedenle, her bir kayıt kümesinin ayrı ayrı okunması gereklidir. Bunu gerçekleştirmek için ise, SqlDataReader nesnesini, Read metodu false değerini döndürdükten, yani geçerli kayıt kümesindeki tüm satırların okunması bittikten sonra, başka kayıt kümesinin olup olmadığı kontrol edilmelidir. Bize bu imkanı aşağıda prototipi verilen, NextResult metodu sağlamaktadır.

public virtual bool NextResult ();

Bu metod geriye bool tipinde bir değer döndürür. Eğer güncel kayıt kümesinin okunması bittikten sonra başka bir kayıt kümesi var ise, true değerini döndürecektir. Bu durumda, toplu sorgularda bir sonraki kayıt kümesinin var olup olmadığını belirlemek için başka bir while döngüsünü kullanırız. İşte yukarıdaki toplu sorgumuz sonucu elde edilen tüm kayıt kümelerini okuyabileceğimiz kodlar;

```csharp
using System;
using System.Data.SqlClient;
using System.Data;
namespace BatchQueries
{
    class Class1
    {
        static void Main(string[] args)
        {
            SqlConnection conFriends=new SqlConnection("data source=localhost;initial catalog=Friends;integrated security=sspi");
            string sorgu="Select * From Makale;Select * From Kitap;Select * From Siteler";
            SqlCommand cmdMakale=new SqlCommand(sorgu,conFriends);
            SqlDataReader drMakale;
            conFriends.Open();
            drMakale=cmdMakale.ExecuteReader(CommandBehavior.CloseConnection);
            do
            {
                Console.WriteLine("---------");
                while(drMakale.Read())
                {
                    Console.WriteLine(drMakale[1]);
                }
                Console.WriteLine("---------");
            }
            while(drMakale.NextResult());
            drMakale.Close();
        }
    }
}
```

Burada do while döngümüz bizim anahtar noktamızdır. While do döngüsü içinde, o an geçerli olan kayıt kümesindeki tüm satırlar okunur. Okunacak satırlar bittikten sonra, sqlDataReader nesnemize ait Read metodu false değerini döndürür ve bu while-do döngüsü sonra erer. Bu işlemin ardından do-while döngüsünde yer alan NextResult metodu çalıştırılır. Eğer arkada başka bir kayıt kümesi varsa, whilde-do döngümüz bu kayıt kümesi için çalıştırılır. Do-while döngümüz, tekniği açısından en az bir kere çalıştırılır. Zaten ExecuteReader metodu sonucu dönen toplu kayıt kümeleri söz konusu olduğunda, SqlDataReader nesnemiz okumak üzere hemen ilk kayıt kümesine konumlanır. İşte sonuç;

![mk55_2.gif](/assets/images/2004/mk55_2.gif)

Şekil 3. Toplu sorgunun çalıştırılması sonucu.

Böylece, SqlDataReader nesnesi ile, toplu sorguların çalıştırılması sonucu elde edilen kayıt kümeleri üzerinden nasıl veri okuyabileceğimizi incelemiş olduk. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.