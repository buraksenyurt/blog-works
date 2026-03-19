---
layout: post
title: "SqlDataReader Sınıfı 2"
date: 2003-12-29 12:00:00 +0300
description: "Bir önceki makalemizde SqlDataReader sınıfını incelemeye başlamıştık Listeleme amaçlı veri kümelerinin görüntülemesinde performans açısından etkin bir rol oynadığından bahsetmiştik. Bugünkü makalemizde , SqlDataReader sınıfının faydalı diğer özelliklerinden bahsedeceğiz. Öncelikle, bir SqlDataReader nesnesinin, geçerli ve açık bir SqlConnection nesnesi üzerinde çalışan bir SqlCommand nesnesi yardımıyla oluşturulduğunu hatırlayalım."
categories:
  - ado-net
tags:
  - ado-net
  - csharp
  - dotnet
  - performance
---
# SqlDataReader Sınıfı 2
Değerli Okurlarım, Merhabalar.

Bir önceki makalemizde SqlDataReader sınıfını incelemeye başlamıştık Listeleme amaçlı veri kümelerinin görüntülemesinde performans açısından etkin bir rol oynadığından bahsetmiştik. Bugünkü makalemizde, SqlDataReader sınıfının faydalı diğer özelliklerinden bahsedeceğiz. Öncelikle, bir SqlDataReader nesnesinin, geçerli ve açık bir SqlConnection nesnesi üzerinde çalışan bir SqlCommand nesnesi yardımıyla oluşturulduğunu hatırlayalım.

Burada SqlCommand sınıfına ait ExecuteReader metodu kullanılmaktadır. ExecuteReader metoduna değişik parametreler geçirerek uygulamanın performansını dahada arttırabiliriz. Önceki makalemizde, CommandBehavior.CloseConnection parametre değerini kullanmıştık. CommandBehavior, çalıştırılacak olan sql sorgusu için bir davranış belirlememizi sağlar. SqlCommand nesnesinin ExecuteReader metodunun alabileceği parametre değerleri şekil1 de görülmektedir. Bunların ne işe yaradığı kısaca tablo 1 ‘de bahsedilmiştir.

![mk29_1.gif](/assets/images/2003/mk29_1.gif)

Şekil 1. CommandBehavior Davranışları

CommandBehavior Değeri
İşlevi

CommandBehavior.CloseConnection
SqlDataReader nesnesi Close metodu ile kapatıldığında,,ilişkili SqlConnection nesneside otomatik olarak kapatılır. Nitekim, işimiz bittiğinde SqlConnection nesnesinin açık unutulması sistem kaynaklarının gereksiz yere harcanmasına neden olur.

CommandBehavior.SingleRow
En çok kullanılan parametrelerden birisidir. Eğer sql sorgumuz tek bir satır döndürecek tipte ise bu davranışı kullanmak performansı olumlu yönde etkiler. Örneğin PrimaryKey üzerinden yapılan sorgular. (“Select * From Tablo Where ID=3” tarzında.)

CommandBehavior.SingleResult
Tek bir değer döndürecek tipteki sorgular için kullanılır. Örneğin belli bir alandaki sayısal değerlerin toplamı veya tablodaki kayıt sayısını veren sorgular gibi. Bu tekniğe alternatif olan ve daha çok tercih edilen bir diğer yöntem, SqlCommand nesnesinin ExecuteScalar metodudur

CommandBehavior.SchemaOnly
Çalıştırılan sorgu sonucu elde edilen satır (satırların) sadece alan bilgisini döndürür.

CommandBehavior.SequentialAccess
Bazı durumlarda tablo alanları çok büyük boyutlu binary tipte veriler içerebilirler. Bu tarz büyük verilerinin okunması için en kolay yol bunları birer akım (stream) halinde belleğe okumak ve oradan ilgili nesnelere taşımaktır. SequnetialAccess davranışı bu tarz akımların işlenmesine imkan tanırken performansıda arttırmaktadır.

CommandBehavior.KeyInfo
Bu durumda sql sorgusu sonucunda SqlDataReader nesnesi, tabloya ait anahtar alan bilgisini içerir.

Tablo 1. CommandBehavior Davranışları

Şimdi dilerseniz basit Console uygulamaları ile, yukarıdaki davranışların işleyişlerini inceleyelim. CommandBehavior. CloseConnection durumunu önceki makalemizde işlediğimiz için tekrar işleme gereği duymuyorum. Şimdi en çok kullanacağımız davranışlardan birisi olan SingleRow davranışına bakalım. Uygulamamız ID isimli PrimaryKey alanı üzerinden bir sorgu çalışıtırıyor. Dönen veri kümesinin tek bir satırdan oluşacağı kesindir. Bu durum, SingelRow davranışını kullanmak için en ideal durumdur.

```csharp
using System;
using System.Data;
using System.Data.SqlClient; 

namespace SqlDataReader2
{
     class Class1
     {
          static void Main(string[] args)
          {
               SqlConnection conFriends=new SqlConnection("data source=localhost;integrated security=sspi;initial catalog=Friends");

               SqlCommand cmd=new SqlCommand("Select * From Kitaplar Where ID=18",conFriends); /* Sql sorgumuz ID isimli primary key üzerinden bir sorgu çalıştırıyor ve 18 nolu ID değerine sahip satırı elde ediyor. Burada tek satırlık veri olduğu kesin. */

               SqlDataReader dr;
               conFriends.Open(); 
               dr=cmd.ExecuteReader(CommandBehavior.SingleRow); /* Tek satırlık veri için davranışımızı SingleRow olarak belirliyoruz. */

               dr.Read(); /* Elde edilen satırı belleğe okuyoruz. Görüldüğü gibi herhangibir while döngüsü kullanma gereği duymadık.*/

               for(int i=0;i<dr.FieldCount;++i) /* Satırın alan sayısı kadar devam edicek bir döngü kuruyoruz ve her alanın adını GetName, bu alanlara ait değerleride dr[i].ToString ile ekrana yazdırıyoruz. */
               {
                    Console.WriteLine(dr.GetName(i).ToString()+"="+dr[i].ToString());
               }
               dr.Close(); /* SqlDataReader nesnemizi kapatıyoruz. Ardından SqlConnection nesnemizide kapatmayı unutmuyoruz. Böylece bu nesnelere ait kaynaklar serbest kalmış oluyor.*/

               conFriends.Close(); 
          }
     }
} 
```

![mk29_2.gif](/assets/images/2003/mk29_2.gif)

Şekil 2. SingleRow davranışı.

Şimdi SingleResult davranışını inceleyelim. Bu kez Northwind veritabanında yer alan Products tablosundaki UnitPrice alanlarının ortalamasını hesaplayan bir sql sorgumuz var. Burada tek bir değer dönmektedir. İşte SingleResult bu duruma en uygun davranış olucaktır.

```csharp
using System;
using System.Data;
using System.Data.SqlClient; 
namespace SqlDataReader3
{
     class Class1
     {
          static void Main(string[] args)
          {
               SqlConnection conNorthwind=new SqlConnection("data source=localhost;integrated security=sspi;initial catalog=Northwind");

               SqlCommand cmd=new SqlCommand("Select SUM(UnitPrice)/Count(UnitPrice)As [Ortalama Birim Fiyatı] From Products",conNorthwind);

               SqlDataReader dr;
               conNorthwind.Open();                dr=cmd.ExecuteReader(CommandBehavior.SingleResult);
               dr.Read();                Console.WriteLine(dr.GetName(0).ToString()+"="+dr[0].ToString()); 
               dr.Close();
               conNorthwind.Close();
          }
     }
} 
```

![mk29_3.gif](/assets/images/2003/mk29_3.gif)

Şekil 3. SingleResult davranışı.

Şimdie SchemaOnly davranışını inceleyelim. Önce aşağıdaki kodları yazıp çalıştıralım.

```csharp
using System;
using System.Data;
using System.Data.SqlClient; 
namespace SqlDataReader4
{
     class Class1
     {
          static void Main(string[] args)
          {
               SqlConnection conNorthwind=new SqlConnection("data source=localhost;integrated security=sspi;initial catalog=Northwind");

               SqlCommand cmd=new SqlCommand("Select * From Products",conNorthwind);
               SqlDataReader dr;
               conNorthwind.Open(); 
               dr=cmd.ExecuteReader(CommandBehavior.SchemaOnly);

               dr.Read();
               try
               {
                    for(int i=0;i<dr.FieldCount;++i)
                    {
                         Console.WriteLine(dr.GetName(i).ToString()+" "+ dr.GetFieldType(i).ToString()+" "+dr[i].ToString());
                    }
               }
               catch(Exception hata)
               {
                    Console.WriteLine(hata.Message.ToString());
               } 
               dr.Close();
               conNorthwind.Close();
          }
     }
}
```

Yukarıdaki console uygulamasını çalıştırdığımızda aşağıdaki hata mesajını alırız.

![mk29_4.gif](/assets/images/2003/mk29_4.gif)

Şekil 4. Hata.

SchemaOnly davranışı sorgu ne olursa olsun sadece alan bilgilerini döndürür. Herhangibir veri döndürmez. Bu yüzden dr[i].ToString () ifadesi i nolu indexe sahip alan için herhangibir veri bulamayıcaktır. Kodun bu bölümünü aşağıdaki gibi değiştirirsek;

```csharp
Console.WriteLine(dr.GetName(i).ToString()+" "+dr.GetFieldType(i).ToString());  
```

Ve şimdi console uygulamamızı çalıştırırsak aşağıdaki ekran görüntüsünü elde ederiz. GetFieldType metodu i indeksli alanın veri tipinin.NET’teki karşılığını döndürürken GetName ile bu alanın adını elde ederiz.

![mk29_5.gif](/assets/images/2003/mk29_5.gif)

Şekil 5. SchemaOnly davranışı.

Şimdi SequentialAccess davranışını inceleyelim. Bu sefer pubs isimli veritabanında yer alan pub_info isimli tablonun Text tipinde uzun veriye sahip pr_info alanının kullanacağız. Sorgumuz belli bir pub_id değerine sahip satırın pr_info alanını alıyor. Biz GetChars metodunu kullanarak alan içindeki veriyi karakter karakter okuyor ve beleğe doğru bir akım (stream) oluşturuyoruz. GetChars metodu görüldüğü üzere 5 parametre almaktadır. İl parametre ile hangi alanın okunacağını belirtiriz. İkin paramtere bu alanın kaçıncı karakterinden itibaren okunmaya başlanacağı bildirir. Üçüncü parameter ise char tipinden bir diziyi belirtir. Okunan her bir karakter bu diziye aktarılacaktır. Dördüncü parameter ile dizinin kaçıncı elemanından itibaren aktarımın yapılacağı belirtilir. Son parametremiz ise ne kadar karakter okunacağını belirtmektedir. Şimdi kodlarımızı yazalım.

```csharp
using System;
using System.Data;
using System.Data.SqlClient; 
namespace SqlDataReader5
{
     class Class1
     {
          static void Main(string[] args)
          {
               SqlConnection conPubs=new SqlConnection("data source=localhost;integrated security=sspi;initial catalog=pubs");

               SqlCommand cmd=new SqlCommand("Select pr_info From pub_info where pub_id=0736",conPubs);
               SqlDataReader dr;
               conPubs.Open(); 
               dr=cmd.ExecuteReader(CommandBehavior.SequentialAccess);
               dr.Read();
               try
               {
                    char[] dizi=new char[130]; /* 130 char tipi elemandan oluşan bir dizi tanımladık. */
                    dr.GetChars(0,0,dizi,0,130); /* Dizimize pr_info alanından 130 karakter okuduk.*/
                    for(int i=0;i<dizi.Length;++i) /* Dizideki elemanları ekrana yazdırıyoruz. */
                    {
                         Console.Write(dizi[i]);
                    }
                    Console.WriteLine();
               }
               catch(Exception hata)
               {
                    Console.WriteLine(hata.Message.ToString());
               }
                dr.Close();
               conPubs.Close();
          }
     }
}
```

Sonuç olarak ekran görüntümüz aşağıdaki gibi olucaktır.

![mk29_6.gif](/assets/images/2003/mk29_6.gif)

Şekil 6. SequentialAccess davranışı.

Evet değerli Okurlarım. Geldik bir makalemizin daha sonuna. Umarım sizi, SqlDataReader sınıfı ile ilgili bilgilerle donatabilmişimdir. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.