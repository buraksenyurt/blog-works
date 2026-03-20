---
layout: post
title: "Stored Procedure Yardımıyla Yeni Bir Kayıt Eklemek"
date: 2003-11-08 12:00:00 +0300
categories:
  - ado-net
tags:
  - ado-net
  - csharp
  - dotnet
  - sql-server
  - t-sql
---
Bu yazımızda Sql Server üzerinde, kendi yazdığımız bir Saklı Yordam (Saklı Yordam) ile, veritabanındaki ilgili tabloya nasıl kayıt ekleyeceğimizi incelemeye çalışacağız.

Öncelikle, Saklı Yordamlar hakkında kısa bir bilgi vererek hızlı bir giriş yapalım. Saklı yordamlar derlenmiş sql cümlecikleridir. Bunlar birer veritabanı nesnesi oldukları için, doğrudan veritabanı yöneticisi olan programda (örneğin Sql Server) yer alırlar. Bu nedenle veritabanınızı bir yere taşıdığınızda otomatik olarak, saklı yordamlarınızıda taşımış olursunuz. Bu Saklı Yordam'lerin tercih edilme nedenlerinden sadece birisidir.

Diğer yandan, derlenmiş olmaları aslında bu sql cümleciklerinin doğrudan makine diline dönüştürüldüğü anlamına gelmez. Aslında, çalıştırmak istediğimiz sql cümleciklerini bir Saklı Yordam içine yerleştirerek, bunun bir veritabanı nesnesi haline gelmesini ve çalışıtırıldığında doğrudan, veritabanı yöneticisini üzerinde barındıran sunucu makinede işlemesini sağlarız. Bu doğal olarak, istemci makinelerdeki iş yükünü azaltır ve performansı arttırır. Nitekim bir program içinde çalışıtırılan sql cümleleri, Saklı Yordam’ lardan çok daha yavaş sonuç döndürür. Dolayısıyla Saklı Yordamlar özellikle çok katlı mimariyi uygulamak isteğimiz projelerde faydalıdır. Saklı Yordamların faydalarını genel hatları ile özetlemek gerekirse;

![mk1_1.gif](/assets/images/2003/mk1_1.gif)

Şekil 1. Saklı Yordam Kullanmanın Avantajları.

İşte bizim bugünkü uygulamamızda yapacağımız işlemde budur. Bu uygulamamızda basit bir Saklı Yordam yaratacak, SqlCommand nesnesinin CommandType özelliğini, SqlParameters koleksiyonunu vb. kullanarak geniş bir bilgi sahibi olucağız. Öncelikle üzerinde çalışacağımız tablodan bahsetmek istiyorum. Basit ve konuyu hızlı öğrenebilmemiz açısından çok detaylı bir kodlama tekniği uygulamıyacağım. Amacımız Saklı Yordamımıza parametreler göndererek doğrudan veritabanına kaydetmek olucak. Dilerseniz tablomuzu inceleyelim ve oluşturalım.

![mk1_2.gif](/assets/images/2003/mk1_2.gif)

Şekil 2. Tablonun Yapısı.

Şekil 2' de tablomuzda yer alan alanlar görülmekte. Bu tabloda arkadaşlarımızın doğum günlerini, işlerini, isim ve soyisim bilgilerini tutmayı planlıyoruz. Tablomuzda FriendsID isminde Primary Key olan ve otomatik olarak artan bir alanda yer alıyor. Şimdi ise insert sql deyimini kullandığımız Saklı Yordamımıza bir göze atalım.

![mk1_3.gif](/assets/images/2003/mk1_3.gif)

Şekil 3. Insert Friend Saklı Yordamının Kodları.

Şekil 3 kullanacağımız Saklı Yordamın T-SQL (Transact SQL) deyimlerini gösteriyor. Burada görüldüğü gibi Sql ifademizin 4 parametresi var. Bu parametrelerimiz;

Parametre Adı
Veri Tipi
Veri Uzunluğu
Açıklama

@fn
Nvarchar
50
First Name alanı için kullanılacak.

@ln
Nvarchar
50
Last Name alanı için kullanılacak.

@bd
Datetime
-
BirthDay alanı için kullanılacak.

@j
Nvarchar
50
Job alanı için kullanılacak.

Tablo 1. Saklı Yordamımızda Kullanılan Giriş Parametreleri.

```csharp
Insert Into Base (FirstName,LastName,BirthDay,Job) values (@fn,@ln,@bd,@j)
```

cümleciği ile standart bir kayıt ekleme işlemi yapıyoruz. Tek önemli nokta values (değerler) olarak, parametre değerlerini gönderiyor olmamız. Böylece, Saklı Yordamımız,.net uygulamamızdan alacağı parametre değerlerini bu sql cümleciğine alarak, tablomuz üzerinde yeni bir satır oluşturulmasını sağlıyor. Peki bu parametre değerlerini.net uygumlamamızdan nasıl vereceğiz? Bunun için uygulamamızda bu Saklı Yordamı kullanan bir SqlCommand nesnesi oluşturacağız. Daha sonra, Saklı Yordamımızda yer alan parametreleri, bu SqlCommand nesnesi için oluşturacak ve Parameters koleksiyonuna ekleyeceğiz. Bu işlemin tamamlanamasının ardından tek yapacağımız Saklı Yordama geçicek parametre değerlerinin, SqlCommand nesnesindeki uygun SqlParameter nesnelerine aktarılması ve Saklı Yordamın çalıştırılması olucak.

Öncelikle C# için yeni bir Windows Application oluşturalım ve formumuzu aşağıdaki şekilde düzenleyelim. Burada 3 adet textBox nesnemiz ve tarih bilgisini girmek içinde bir adet DateTimePicker nesnemiz yer alıyor. Elbette insert işlemi içinde bir Button kontrolü koymayı ihmal etmedik. Kısaca formun işleyişinden bahsetmek istiyorum. Kullanıcı olarak biz gerekli bilgileri girdikten sonra insert başlıklı Button kontrolüne bastığımızda, girdiğimiz bilgiler Saklı Yordam’ daki parametre değerleri olucak. Ardından Saklı Yordamımız çalıştırılıacak ve girdiğimiz bu parametre değerleri ile, sql sunucumuzda yer alan veritabanımızdaki Base isimli tablomuzda yeni bir satır oluşturulacak.

![mk1_4.gif](/assets/images/2003/mk1_4.gif)

Şekil 4. Formun Tasarım Zamanındaki Görüntüsü.

Şimdide kodumuzu inceleyelim. Her zaman olduğu gibi SQLClient sınıfına ait nesneleri kullanacağımız için bu sınıfı using ile projemizin en başına ekliyoruz.

```csharp
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
```

Sırada veritabanına olan bağlantımızı referans edicek olan SQLConnection nesnemiz var.

```csharp
SqlConnection conFriends = new SqlConnection("initial catalog=Friends;data source=localhost;integrated security=sspi;packet size=4096");
```

Kısaca anlatmak gerekirse, SQL Sunucumuz'daki Friends isimli Database’ e bağlantı sağlıyacak bir SqlConnection nesnesi tanımladık. Burada SqlConnection sınıfının prototipi aşağıda verilen Constructor (yapıcı metodunu) metodunu kullandık. Bildiğiniz gibi SqlConnection nesnesi, Sql Sunucusu ile ado.net nesneleri arasında iletişimin sağlanabilmesi için bir bağlantı hattı tesis etmektedir.

```csharp
public SqlConnection(string connectionString);
```

Şimdi btnInsert isimli butonumuzun click olay procedure'ündeki kodumuzu yazalım.

```csharp
private void btnInsert_Click(object sender, System.EventArgs e)
{
     conFriends.Open();/* Baglanti açiliyor. SqlCommand nesnesi ile ilgili ayarlamalara geçiliyor. Komut SQL Server’ da Friends database’inde yazili olan "Insert Friend" isimli Saklı Yordam’ı çalistiracak. Bu Procedure’ ün ismini, CommandText parametresine geçirdikten sonar ikinci parameter olarak SqlConnection nesnemizi belirtiyoruz.*/
     SqlCommand cmdInsert = new SqlCommand("Insert Friend",conFriends);

     /* SqlCommand nesnesinin CommandType degerinide CommandType.StoredProcedure yapiyoruz. Bu sayede CommandText’e girilen değerin bir Saklı Yordam’e işaret ettiğini belirtmiş oluyoruz.*/
     cmdInsert.CommandType=CommandType.StoredProcedure;
     /* Şimdi bu Saklı Yordam için gerekli parametreleri olusturacagiz. Bunun için SqlCommand nesnesininin parameters koleksiyonunun Add metodunu kullaniyoruz. Parametreleri eklerken, parametre isimlerinin SQL Server’da yer alan Saklı Yordamlardaki parametre isimleri ile ayni olmasina ve baslarina @ isareti gelmesine dikkat ediyoruz. Bu Add metodunun ilk parametresinde belirtiliyor. Add metodu ikinci parametre olarak bu parametrenin veri tipini alıyor. Üçüncü parametresi ise bu parametrik degiskenin boyutu oluyor.*/
     SqlParameter paramFirstName=cmdInsert.Parameters.Add("@fn",SqlDbType.NVarChar,50);
     /* Burada SqlCommand nesnesine @fn isimli nvarchar tipinde ve uzunluğu 50 karaketerden olusan bir parametre ekleniyor. Aynı şekilde diğer parametrelerimizi de belirtiyoruz.*/

     SqlParameter paramLastName=cmdInsert.Parameters.Add("@ln",SqlDbType.NVarChar,50);
     SqlParameter paramBirthDay=cmdInsert.Parameters.Add("@bd",SqlDbType.DateTime);
     SqlParameter paramJob=cmdInsert.Parameters.Add("@j",SqlDbType.NVarChar,50);
     // Şimdide paremetrelerimize degerlerini verelim.
     paramFirstName.Value=txtFirstName.Text;
     paramLastName.Value=txtLastName.Text;
     paramBirthDay.Value=dtBirthDay.Text;
     paramJob.Value=txtJob.Text;
     // Böylece ilgili paremetrelere degerleri geçirilmis oldu. simdi komutu çalistiralim.
     cmdInsert.ExecuteNonQuery();
     /* Böylece Saklı Yordamimiz, paremetrelerine atanan yeni degerler ile çalisitirlir. Bunun sonucu olarak SQL Server’ daki Saklı Yordama burada belirttiğimiz parametre değerleri gider ve insert cümleciği çalıştırılarak yeni bir kayit eklenmis olur.*/
     conFriends.Close(); // Son olarak SqlConnection’ ımızı kapatıyoruz.
}
```

Şimdi bir deneme yapalım.

![mk1_5.gif](/assets/images/2003/mk1_5.gif)

Şekil 5. Programın Çalışması.

![mk1_6.gif](/assets/images/2003/mk1_6.gif)

Şekil 6. Saklı Yordam'ün işlemesinin Sonucu.

Görüldüğü gibi Saklı Yordamlar yardımıyla tablolarımıza veri eklemek son derece kolay, hızlı ve etkili. Bununla birlikte Saklı Yordamlar sağladıkları güvenlik kazanımları nedeni ilede tercih edilirler. Saklı Yordamları geliştirmek son derece kolaydır. İstediğini sql işlemini gerçekleştirebilirisiniz. Satır silmek, satır aramak gibi. Saklı Yordamlar ile ilgili bir sonraki makalemizde, tablolardan nasıl satır silebileceğimizi incelemeye çalışacağız. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.