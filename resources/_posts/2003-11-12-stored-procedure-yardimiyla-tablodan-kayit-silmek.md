---
layout: post
title: "Stored Procedure Yardımıyla Tablodan Kayıt Silmek"
date: 2003-11-12 12:00:00 +0300
categories:
  - ado-net
tags:
  - ado.net
  - stored-procedures
---
Bugün ki makalemde Stored Procedure yardımıyla bir veritabanı tablosundan, bizim seçtiğimiz herhangi bir satırı nasıl sileceğimizi sizlere anlatmaya çalışacağım.Her zaman olduğu gibi örneğimizi geliştirmek için, SQL Server üzerinde yer alan Northwind veritabanını kullanmak istiyorum. SQL Server üzerinde çalışan örnekler geliştirmek istememin en büyük nedeni, bir veritabanı yönetim sistemi (Database Management System;DBMS) üzerinde.NET ile projeler geliştirmenin gerçekçiliğidir. Güncel yaşantımızda ağ üzerinde çalışan uygulamalar çoğunlukla, iyi bir veritabanı yönetim sistemi üzerinde yazılmış programlar ile gerçekleştirilmektedir.

Çok katlı mimari olarak hepimizin kulağına bir şekilde gelmiş olan bu sistemde, aslında yazmış olduğumuz programlar, birer arayüz niteliği taşımakta olup kullanıcı ile veritabanı arasındaki iletişimi görsel anlamda kolaylaştıran birer araç haline gelmiştir. İşte bu sunum katmanı (presantation layer) denen yerdir. Burada veri tablolarını ve veritabanlarını üzerinde barındıran yer olarak veritabanı katmanı (Database Layer) büyük önem kazanmaktadır.

İşte bir önceki makalemde belirttiğim gibi Stored Procedure'leri kulanmamın en büyük amacı performans, hız ve güvenlik kriterlerinin önemidir. Dolayısıyla, örneklerimizi bu şekilde gerçek uygulamalara yakın tutarak, çalışırsak daha başarılı olucağımız inancındayım.Evet bu kadar laf kalabalığından sonra dilerseniz uygulamamıza geçelim.Uygulamamızın kolay ve anlaşılır olması amacıyla az satırlı bir tablo üzerinde işlemlerimizi yapmak istiyorum. Bu amaçla Categories tablosunu kullanacağım.

![mk2_1.gif](/assets/images/2003/mk2_1.gif)

Şekil 1. Categories tablosunda yer alan veriler.

Tablomuzun yapısını da kısaca incelersek;

![mk2_2.gif](/assets/images/2003/mk2_2.gif)

Şekil 2. Categories tablosunun alan yapısı.

Burada CategoryID alanı bizim için önemlidir. Nitekim silme işlemi için kullanacağımız Stored Procedure içerisinde, belirleyici alan olarak bir parametreye dönüşecektir. Şimdi dilerseniz, Stored Procedure’ümüzü yazalım.

![mk2_3.gif](/assets/images/2003/mk2_3.gif)

Şekil 3. Stored Procedure Kodlari

```text
CREATE PROCEDURE [Delete Category]
@kid int
AS
DELETE FROM Categories WHERE CategoryID=@kid
GO
```

Görüldüğü gibi burada son derece kolay bir T-SQL (Transact SQL) cümleciği var. Burada yapılan işlem aslında @kid parametresine geçilen değeri CategoryID alanı ile eşleştirmek. Eğer bu parametre değerine karşılık gelen bir CategoryID değeri varsa; bu değeri taşıyan satır Categories isimli tablodan silinecektir.

Evet şimdi de.NET ortamında formumuzu tasarlayalım. New Project ile yeni bir C# projesi açarak işe başlıyoruz. Formumuzun tasarımını ben aşağıdaki şekilde yaptım. Sizde buna uygun bir form tasarlayabilir yada aynı tasarımı kullanabilirsiniz. Visual Studio.NET ile program geliştirmenin belkide en zevkli ve güzel yanı form tasarımları. Burada gerçekten de içimizdeki sanatçı ruhunu ortaya çıkartma imkanına sahibiz. Ve doğruyu söylemek gerekirse Microsoft firmasıda artık içimizdeki sanatçı çocuğu özellikle bu tarz uygulamalarda, daha kolay açığa çıkartabilmemiz için elinden geleni yapıyor. Doğal olarakta çok da güzel sonuçlar ortaya çıkıyor. Birde o eski bankalardaki (halen daha varya) siyah ekranlarda, incecik, kargacık, burgacık tasarımları ve arayüzleri düşünün. F12 ye bas geri dön. Tab yap. Şimdi F4 kodu gir.

![mk2_4.gif](/assets/images/2003/mk2_4.gif)

Şekil 4. Formun Ilk Yapisi

Formumuzda bir adet dataGrid nesnesi ve bir adetde button nesnesi yer alıyor. DataGrid nesnesini Categories tablosu içersinde yer alan bilgileri göstermek için kullanacağız. Datagrid verileri gösterirken kullanıcının kayıt eklmek, düzenlemek, ve seçtiği satırı buradan silmesini egellemek istediğimden ReadOnly özelliğine True değerini aktardım. Örneğimizin amacı gereği silme işlemini Sil textine sahip btnSil button nesnesinin Click olay procedure’ünden yapıcağız. Elbette burada database’deki bilgileri dataGrid içersinde göstermek amacıyla bir SqlDataAdapter nesnesi kullanacağım. Bu sadece Categories isimli tablo içerisindeki tüm satırları seçicek bir Select sorgusuna sahip olucak ve bunları dataGrid ile ilişkili olan bir DataTable nesnesine aktarıcak.

Dilerseniz kodlarımızı yazmaya başlayalım. Önceliklie SqlConnection nesnemiz yardımıyla, Northwind veritabanına bir bağlantı açıyoruz. Daha sonra SqlDataAdapter nesnemizi oluşturuyoruz. SqlDataAdapter nesnesini yaratmak için new anahtar sözcüğü ile kullanabileceğimiz 4 adet overload constructor var. Overload constructor, aynı isme sahip yapıcı metodlar anlamına geliyor. Yani bir SqlDataAdapter nesnesini yaratabileceğimiz 4 kurucu (constructor) metod var ve bunların hepside aynı isme sahip (Overload;aşırı yüklenmiş) metodlar. Yeri gelmişken bunlardan da bahsederek bilgilerimizi hem tazeleyelim hem de arttırmış olalım. İşte bu yapıcı metodların prototipleri.

```csharp
public SqlDataAdapter();
public SqlDataAdapter(string selectCommandText,string connectionString);  
public SqlDataAdapter(string selectCommandText,SqlConnection selectConnection);  
public SqlDataAdapter(SqlCommadn selectCommand);
```

Ben uygulamamda ilk yapıcı metodu baz almak istiyorum. Evet artık kodlarımızı yazalım.

NOT: Her zaman olduğu gibi projemizin başına System.Data.SqlClient namespace ini eklemeyi unutmayalım.

```csharp
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient; 
/* Önce SqlConnection nesnesi yardimiyla SQL Server üzerinde yer alan, Northwind isimli veritabanina bir baglanti nesnesi tanimliyoruz. Ilk parametre yani initial catalog, veritabaninin ismini temsil ediyor. Şu anda SQL Server’ in üzerinde yer alan makinede çalisitigimizdan Data Source parametresine localhost degerini aktardik. */

SqlConnection conNorthwind=new SqlConnection("initial catalog=Northwind;Data Source=localhost;integrated security=sspi;packet size=4096");
/* Şimdi Categories tablosunu bellekte temsil edicek olan DataTable nesnemizi yaratiyoruz. Dikkat edersek bellekte dedik. DataTable nesnesi Categories tablosundaki verileri programin çalistigi bilgisayar üzerindeki bellekte sakliyacaktir. Bu durumda SqlConnection nesnemizin açik kalmasina gerek yoktur. Bu da elbetteki sunucu üzerindeki yükü azaltan bir etkendir.*/
DataTable dtbCategories=new DataTable("Kategoriler");
private void Form1_Load(object sender, System.EventArgs e)
{
     conNorthwind.Open();//Bağlantımızı açıyoruz. 

     SqlDataAdapter da=new SqlDataAdapter();//Bir SqlDataAdapter nesnesi tanimladik.  

     /* Asagidaki satir ile yarattigimiz SqlDataAdapter nesnesine bir Select sorgusu eklemis oluyoruz. Bu sorgu sonucu dönen deger kümesi, SqlDataAdapter nesnesinin Fill metodunu kullandigimizda DataTable' in içerisini hangi veriler ile dolduracagimizi belirtecek önemli bir özelliktir. Bir SqlDataAdapter nesnesi yaratildiginda, SelectCommand özelligine SqlCommand türünden bir nesne atanarak bu islem gerçeklestirilir. Burada aslinda, SelectCommand özelliginin prototipinden dolayi new anahtar sözcügü kullanilarak bir SqlCommand nesnesi parametre olarak verilen select cümlecigi ile olusturulmus ve SelectCommand özelligine atanmistir.

      *

      * public new SqlCommand SelectCommand
      * {
      * get;
      * set;
      * }
      * Prototipten de görüldügü gibi SelectCommand özelliginin tipi SqlCommand nesnesi türündendir. Bu yüzden new SqlCommand("....") ifadesi kullanilmistir.
      * */

     da.SelectCommand=new SqlCommand("SELECT * FROM Categories"); 
     da.SelectCommand.Connection=conNorthwind; /* Select sorgusunun çalistirilacagi baglanti belirlenir.*/
     da.FillSchema(dtbCategories,SchemaType.Mapped);/* Burada dataTable nesnemize, veritabanında yer alan Categories isimli tablonun Schema bilgilerinide yüklüyoruz. Yani primaryKey bilgileri, alanların bilgileri yükleniyor. Bunu yapmamızın sebebi, Stored Procedure ile veritabanındaki Categories tablosundan silme işlemini yapmadan önce , bellekteli tablodan da aynı satırı silip dataGrid içindeki görüntünün ve DataTable nesnesinin güncel olarak kalmasını sağlamak. Nitekim silme işleminde DataTable nesnesinden seçili satırı silmek içim kullanacağımız Remove metodu PrimaryKey alanının değerini istemektedir. Bunu verebilmek için tablonun PrimaryKey bilgisininde belleğe yani bellekteki DataTable nesnesine yüklenmiş olması gerekir. İşte bu amaçla Schema bilgilerinide alıyoruz*/ 

     da.Fill(dtbCategories);/* Burada SqlDataAdapter nesnesinin Fill metodu çagirilir. Fill metodu öncelikle SelectCommand.CommandText in degeri olan Select sorgusunu çalistirir ve dönen veri kümesini dtbCategories isimli DataTable nesnesinin bellekte referans ettigi alana yükler. Artik baglantiyida kapatabiliriz.*/

     conNorthwind.Close(); 
     /* Simdi dataGrid nesnemize veri kaynagi olarak DataTable nesnemizi gösterecegiz. Böylece DataGrid, Categories tablosundaki veriler ile dolucak. */
     dgCategories.DataSource=dtbCategories;
} 

private void btnDelete_Click(object sender, System.EventArgs e)
{
     /* Şİmdi silme işlemini gerçekleştireceğimiz Stored Procedure'e DataGrid nesnesi üzerinde kullanıcının seçmiş olduğu satırın CategoryID sütununun değerini göndereceğiz. Bunun için kullanıcının seçtiği satırın numarasını CurrentCell.RowNumber özelliği ile alıyoruz. Daha sonra, CategoryID sütunu dataGrid'in 0 indexli sütunu olduğundan CategoryID değerini elde ederken dgCategories[currentRow,0] metodunu kullanıyoruz.*/

     int currentRow; 
     int selectedCategoryID; 
     currentRow=dgCategories.CurrentCell.RowNumber; 
     selectedCategoryID=(int)dgCategories[currentRow,0]; /* Burada dgCategories[currentRow,0] aslında object tipinden bir değer döndürür. Bu yüzden açık olarak dönüştürme dediğimiz (Explicit) bir Parse(dönüştürme) işlemi yapıyoruz. */

     /* Şimdi de Stored Procedure'ümüzü çalıştıracak olan SqlCommand nesnesini tanımlayalım*/
     SqlCommand cmdDelete=new SqlCommand(); 
     cmdDelete.CommandText="Delete Category";/* Stored Procedure'ün adı CommandText özelliğine atanıyor. Ve bu stringin bir Stored Procedure'e işaret ettiğini belirtmek için CommandType değerini CommandType.StoredProcedure olarak belirliyoruz.*/ 

     cmdDelete.CommandType=CommandType.StoredProcedure; 
     cmdDelete.Connection=conNorthwind;//Komutun çalıştırılacağı bağlantı belirleniyor. 

     /* Şimdi ise @id isimli parametremizi oluşturacağız ve kullanıcının seçmiş olduğu satırın CategoryID değerini bu parametre ile Stored Proecedure'ümüze göndereceğiz.*/ 
     cmdDelete.Parameters.Add("@kid",SqlDbType.Int); 
     cmdDelete.Parameters["@kid"].Value=selectedCategoryID;
     /* Ve önemli bir nokta. Kullanıcıyı uyarmalıyız. Gerçekten seçtiği satırı silmek istiyor mu?* Bunun için MessageBox nesnesni ve Show metodunu kullanacağız. Bu metodun dönüş değerini DialogResult tipinde bir değişkenle kontrol ettiğimize dikkat edin.*/ 

     DialogResult result; 
     result=MessageBox.Show("CategoryID : "+selectedCategoryID.ToString()+". Bu satırı silmek istediğinizden emin misiniz?","Sil",MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
     if (result==DialogResult.Yes ) /*Eğer kullanıcının cevabı evet ise aşağıdaki kod bloğundaki kodlar çalıştırılır ve satır once DataTable nesnesinden sonrada kalıcı olarak databaseden silinir.*/
     {
          conNorthwind.Open();// Bağlantımızı açıyoruz.
          /* Elbette veritabanından doğrudan sildiğimiz satırı bellekteki DataTable nesnesinin referans ettiği yerdende siliyoruz ki datagrid nesnemiz güncelliğini korusun. Bunun için seçili olan dataTable satırınu bir DataRow nesnesine aktarıyoruz. Bunu yaparkende seçili kaydı Find metodu ile CategoryID isimli Primary Key alanı üzerinden arama yapıyoruz. Kayıt bulunduğunda tüm satırbilgisi bir DataRow türü olarak geri dönüyor ve bunu DataRow nesnemize atıyoruz. Remove metodu silinmek istenen satır bilgisini parameter olarak alır. Ve bu parameter DataRow tipinden bir parametredir.*/
          DataRow drSelectedRow;           drSelectedRow=dtbCategories.Rows.Find(selectedCategoryID);           dtbCategories.Rows.Remove(drSelectedRow); 
          cmdDelete.ExecuteNonQuery();/ * Artık Stored Procedure de çalıştırılıyor ve slime işlemi doğrudan veritabanındaki tablo üzerinden gerçekleştiriliyor. ExecuteNonQuery bu Stored Procedure'ü çalıştıracak olan metoddur. Delete,Update,Insert gibi kayıt döndürmesi beklenmeyen (Select sorguları gibi) sql cümlecikleri için ExecuteNonQuery metodu kullanılır.*/
          conNorthwind.Close();
     } 
}
```

Şimdi dilerseniz programımızı çalıştırıp sonuçlarına bir bakalım. Öncelikle Categories isimli tabloya doğrudan SQL Server üzerinden örnek olması açısından bir kaç kayıt ekleyelim.

![mk2_5.gif](/assets/images/2003/mk2_5.gif)

Şekil 5. Categories tablosuna 3 yeni kayıt ekledik.

Şimdi uygulamamızı çalıştıralım. Bu durumda ekran görüntüsü aşağıdaki gibi olucaktır. Şu anda dataGrid içindeki bilgiler veritabanından alınıp, bellekteki dataTable nesnesinin referans ettiği bölgedeki verilerden oluşmaktadır. Dolayısıyla Sql Server’a olan bağlantımız açık olmadığı halde verileri izleyebilmekteyiz. Hatta bunların üzerinde değişiklilkler yapıp normal tablo işlemlerinide (silme,kayıt ekleme,güncelleme vb... gibi) gerçekleştirebiliriz. Bu bağlantısız katman olarak adlandırdığımız olaydır. Bu konuya ilerliyen makalelerimizide daha detaylı olarak inceleyeceğiz.

![mk2_6.gif](/assets/images/2003/mk2_6.gif)

Şekil 6. Load Procedure’ünün çalıştırılmasından sonraki görünüm.

Şimdi seçtiğimiz 17 CategoryID satırını silelim. Ekrana bir soru çıkacaktır.

![mk2_7.gif](/assets/images/2003/mk2_7.gif)

Şekil 7. MessageBox.Show (.....) metodunun sonucu.

Şimdi Yes butonuna basalım. Bu durumda 17 CategoryID li satır dataTable’dan dolayısıyla dataGrid’den silinir. Aynı zamanda çalıştırdığımız Stored Procedure ile veritabanından da doğrudan silinmiştir.

![mk2_8.gif](/assets/images/2003/mk2_8.gif)

Şekil 8. Silme işlemi sonrası.

Şimdi SQL Server’a geri dönüp tablonun içeriğini kontrol edicek olursak aşağıdaki sonucu elde ederiz.

![mk2_9.gif](/assets/images/2003/mk2_9.gif)

Şekil 9. Sonuç.

Görüldüğü gibi CategoryID=17 olan satır veritabanındaki tablodanda silinmiştir. Bir sonraki makalemizde görüşmek dileğiyle.