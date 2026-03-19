---
layout: post
title: "DataSet ve WriteXml Metodunun Kullanımı"
date: 2003-11-30 12:00:00 +0300
description: "Bugünkü makalemizde, bir dataset nesnesinin içerdiği tabloların ve bu tablolardaki alanlara ait bilgilerin xml formatında nasıl yazdırıldığını göreceğiz."
categories:
  - ado-net
tags:
  - ado-net
  - csharp
  - xml
  - dataset
---
# DataSet ve WriteXml Metodunun Kullanımı
Değerli Okurlarım, Merhabalar.

Bugünkü makalemizde, bir dataset nesnesinin içerdiği tabloların ve bu tablolardaki alanlara ait bilgilerin xml formatında nasıl yazdırıldığını göreceğiz. Örneğimiz son derece basit. Örnek uygulamamızda, Sql sunucusu üzerinde yer alan, Friends isimli database’den Kitaplar isimli tabloya ait verileri taşıyan bir dataset nesnesini kullanacağız. DataSet sınıfına ait WriteXml metodu dataset içerisinde yer alan bilgilerin bir xml dokumanına Schema bilgisi ile birlikte aktarılmasına imkan sağlmakatadır. Bu metoda ait 8 adet yapıcı (Constructor) metod bulunmakta olup biz örneğimizde,

Public void WriteXml (string dosyaadi, XmlWriteMode mod);

Yapıcısını kullanacağız. Burada yer alan ilk parametre xml içeriğini kaydedeciğimiz dosyanın tam yol adını taşımaktadır. İkinci parametre ise;

XmlWriteMode.IgnoreSchema

XmlWriteMode.WriteSchema

XmlWriteMode.DiffGram

Değerlerinden birini alır. IgnoreSchema olarak belirtildiğinde, DataSet nesnesinin içerdiği veriler, Schema bilgileri olmadan (örneğin alan adları, veri tipleri, uzunlukları vb...) xml dokumanı haline getirilir. WriteSchema olması halinde ise, Schema bilgileri aynı xml dosyasının üst kısmına ile tagları arasında yazılır. DiffGram durumunda ise, veriler üzerinde yapılan değişikliklerin takip edilebilmesi amaçlanmıştır. Dilerseniz vakit kaybetmeden örnek uygulamamıza geçelim. Basit bir Console uygulaması oluşturacağız.

```csharp
using System;
using System.Data;
using System.Data.SqlClient; 
namespace WriteXml
{
     class Class1
     {
          [STAThread]
          static void Main(string[] args)
          {
               SqlConnection conFriends=new SqlConnection("data source=localhost;initial catalog=Friends;integrated security=sspi");
               SqlDataAdapter da=new SqlDataAdapter("Select Kategori,Adi,Yazar,BasimEvi From Kitaplar",conFriends);
               DataSet ds=new DataSet();
               conFriends.Open();
               da.Fill(ds); 
               /*yukarıdaki adımlarda, Sql sunucumuz üzerinde yer alan Friends isimli database'e bir bağlantı açıyor, SqlDataAdapter nesnemiz yardımıyla bu database içindeki Kitaplar isimli tablodan verileri alıyor ve bunları bir dataset nesnesine yüklüyoruz.*/ 
              ds.WriteXml("D:\\Kitaplar.xml",XmlWriteMode.WriteSchema); /* Bu adımda ise, dataset nesnesini içerdiği verileri Schema bilgisi ile birlikte Kitaplar.xml isimli xml dokumanına yazıyoruz. */

               conFriends.Close(); // Bağlantımızla işimiz bittiğinden kapatıyoruz.
          }
     }
}
```

Bu uygulamayı çalıştırdığımızda, D:\Kitaplar.xml isimli bir dosyanın oluştuğunu görürüz. Bu dosyayı açtığımızda ise aşağıda yer alan xml kodlarını elde ederiz. Gördüğünüz gibi verilerin yanında alanlara ait bilgilerde aynı xml dosyası içine yüklenmiştir.

![mkx1.gif](/assets/images/2003/mkx1.gif)

Kodu şimdide aşağıdaki gibi değiştirelim. IgnoreSchema seçimini kullanalım bu kezde.

```csharp
using System;
using System.Data;
using System.Data.SqlClient; 
namespace WriteXml
{
     class Class1
     {
          [STAThread]
          static void Main(string[] args)
          {
               SqlConnection conFriends=new SqlConnection("data source=localhost;initial catalog=Friends;integrated security=sspi");

               SqlDataAdapter da=new SqlDataAdapter("Select Kategori,Adi,Yazar,BasimEvi From Kitaplar",conFriends);

               DataSet ds=new DataSet();
               conFriends.Open();
               da.Fill(ds); 
               ds.WriteXml("D:\\Kitaplar.xml",XmlWriteMode.IgnoreSchema);
               conFriends.Close();
          }
     }
} 
```

Bu durumda, Kitaplar.xml dosyamızın içeriğine bakıcak olursak schema bilgilerinin eklenmediğini sadece tablonun içerdiği verilerin yer aldığını görürüz.

![mkx2.gif](/assets/images/2003/mkx2.gif)

Bir sonraki makalemizde görüşmek dileğiyle hepinizi mutlu günler dilerim.