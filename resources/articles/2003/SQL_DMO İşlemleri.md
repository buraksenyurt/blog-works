---
title: "SQL_DMO İşlemleri"
pubDate: 2003-12-12 12:00:00
categories:
  - T SQL
tags:
  - T SQL
  - C#
  - .NET
  - ADO.NET
  - SQL Server
  - Authentication
  - Visual Studio
---

# SQL_DMO İşlemleri
Değerli Okurlarım, Merhabalar.

Bugünkü makalemizde, Sql Distributed Management Objects (SQL Dağıtık Yönetim Nesneleri) kütüphanesini incelemeye çalışacağız. SQL_DMO kütüphanesi bir COM uygulaması olup, SQL sunucusu ile birlikte sisteme kurulmaktadır. Bu kütüphanedeki sınıflar yardımıyla, var olan bir sql sunucusu üzerinde yönetimsel işlemler gerçekleştirebiliriz. Örneğin, kullanıcı tanımlayabilir, yeni bir veritabanı yaratabilir bu veritabanına ait tablolar oluşturabilir, var olan bir veritabanı için yedekleme işlemleri gerçekleştirebilir, yedeklenmiş veritabanlarını geri yükleyebilir ve bunlar gibi pek çok yönetsel işlemi gerçekleştirebiliriz.

Uygulamalarımızda bu tip işlemleri kodlayabilmek için, Microsoft SQL Distribution Control’un projemize referans edilmesi gerekmektedir. Bir uygulamaya bunu referans etmek için VS.NET ortamında, Add Reference kısmında, COM nesneleri altında Microsoft SQL Distribution Control 8.0 seçilmelidir. Aşağıdaki şekilde bunun nasıl yapıldığını görebilirsiniz.

![mk18_1.gif](images/mk18_1.gif)

Şekil 1. Microsoft SQL Distribution Control 8.0 ‘in eklenişi.

Bu noktadan sonra uygulamamızda SQLDMO isimli temel sınıfı kullanarak bahsetmiş olduğumuz işlemleri gerçekleştirebiliriz. Konuyu daha iyi kavrayabilmek amacıyla dilerseniz, hemen basit bir uygulama gerçekleştirelim. Bu uygulamamızda, Sql sunucumuz üzerinde, bir veritabanı yaratacak bu veritabanı içinde çok basit bir tablo oluşturacak, Sql sunucusunda yer alan veritabanlarını görücek ve bu veritabanlarına ait tablolara bakabileceğiz. Kodların işleyişini incelediğinizde, işin püf noktasının SQLDMO sınıfında yer alan SQLServerClass, DatabaseClass, TableClass, ColumnClass sınıfılarında olduğunu görebilirisiniz. Buradaki matnık aslında ADO.NET mantığı ile tamamen aynıdır. SQL Sunucunuza bağlanmak için kullanacağınız bir SQLServerClass sınıfı, bir veritabanını temsil eden DatabaseClass sınıfı, bir tablo için TableClass sınıfı ve tabloya ait alanları temsil edicek olan ColumnClass sınıfı vardır.

Matnık aynı demiştik. Bir veritabanı yaratmak için, DatabaseClass sınıfından örnek bir nesne oluşturursunuz. Bunu var olan Sql sunucusuna eklemek demek aslında bu nesneyi SQLServerClass nesnenizin Databases koleksiyonuna eklemek demektir. Aynı şekilde bir tablo oluşturmak için TableClass sınıfı örneğini kullanır,ve bunu bu kez DatabaseClass nesnesinin Tables koleksiyonuna eklersini. Tahmin edeceğiniz gibi bir tabloya bir alan eklemek için ColumnClass sınıfından örnek bir nesne kullanır ve bunun özelliklerini ayarladıktan sonra tablonun Columns koleksiyonuna eklersiniz. Kodlarımızı incelediğiniz zaman konuyu çok daha net bir şekilde anlayacaksınız. Uygulamamız aşağıdakine benzer bir formdan oluşmakta. Sizde buna benzer bir form oluşturarak işe başlayabilirsiniz.

![mk18_2.gif](images/mk18_2.gif)

Şekil 2. Form tasarımımız.

Şimdi kodlarımızı yazalım.

```csharp
SQLDMO.SQLServerClass srv;
SQLDMO.DatabaseClass db;
SQLDMO.TableClass tbl; 

private void btnConnect_Click(object sender, System.EventArgs e)
{
     srv=new SQLDMO.SQLServerClass(); /* SQL Sunucusu üzerinde, veritabani yaratma gibi islemler için, Sql Sunucusunu temsil edicek ve ona baglanmamizi sagliyacak bir nesneye ihtiyacimiz vardir. Bu nesne SQLDMO sinifinda yer alan SQLServerClass sinifinin bir örnegi olucaktir. */ 
     srv.LoginSecure=false; /* Bu özellik true olarak belirlendiginde, Sql Sunucusuna Windows Authentication seklinde baglanilir. Eger false degerini verirsek bu durumda Sql Server Authentication geçerli olur. Iste bu durumda SQLServerClass nesnesini Connect metodu ile Sql Sunucusuna baglanirken geçerli bir kullanici adi ve sifre girmemiz gerekmektedir. */
     try
     {
          srv.Connect("BURKI","sa","CucP??80."); /* Baglanti kurmak için kullandigimiz Connect metodu üç parametre almaktadir. Ilk parametre sql sunucusunun adidir. Ikinci parametre kullanici adi ve üçüncü parametrede sifresidir. Eger LoginSecure=true olarak ayarlasaydik, kullanici adini ve sifreyi bos birakicaktik, nitekim Windows Authentication (windows dogrulamasi) söz konusu olucakti.*/
          durumCubugu.Text="Sunucuya baglanildi..."+srv.Status.ToString();
     }
     catch(Exception hata)
     {
          MessageBox.Show(hata.Message);
     }
} 
private void btnVeritabaniOlustur_Click(object sender, System.EventArgs e)
{
     try
     {
          db=new SQLDMO.DatabaseClass(); /* SQL-DMO kütüphanesinde, veritabanlarini temsil eden sinif DatabaseClass sinifidir. */
          db.Name=this.txtVeritabaniAdi.Text; /* Veritabani nesnemizin name özelligi ile veritabaninin adi belirlenir.*/
          srv.Databases.Add(db); /* olusturulan DatabaseClass nesnesi SQLServerClass sinifinin Databases koleksiyonuna eklenerek Sql Sunucusu üzerinde olusturulmasi saglaniyor. */
          durumCubugu.Text=db.Name.ToString()+" veritabani "+srv.Name.ToString()+" SQL Sunucusunda olusturuldu";
     }
     catch(Exception hata)
     {
          MessageBox.Show(hata.Message);
     }
} 
private void btnTabloOlustur_Click(object sender, System.EventArgs e)
{
     try
     {
          tbl=new SQLDMO.TableClass(); /* Yeni bir tablo olusturabilmek için SQL-DMO kütüphanesinde yer alan, TableClass sinifi kullanilir.*/
          tbl.Name=txtTabloAdi.Text; /* Tablomuzun ismini name özelligi ile belirliyoruz.*/ 
          SQLDMO.ColumnClass dc; /* Tabloya eklenecek alanlarin her birisi birer ColumnClass sinifi nesnesidir. */
          dc=new SQLDMO.ColumnClass();/* Bir ColumnClass nesnesi yaratiliyor ve bu nesnenin gerekli özellikleri belirleniyor. Name özelligi ile ismi, Datatype özelligi ile veri türü      belirleniyor. Biz burada ID isimli alanimizin otomatik olarak artan ve 1000 den baslayarak 1'er artan bir alan olmasini istedik. */
          dc.Name="ID";
          dc.Datatype="Int";
          dc.Identity=true;
          dc.IdentitySeed=1000;
          dc.IdentityIncrement=1;
          tbl.Columns.Add(dc); /* Olusturulan bu alan TableClass nesnemizin Columns koleksiyonuna eklenerek tabloda olusturulmasi saglanmis oluyor. */ 
          dc=new SQLDMO.ColumnClass();
          dc.Name="ISIM";
          dc.Datatype="char"; /* String tipte bir alan */
          dc.Length=50;
          tbl.Columns.Add(dc); 
          dc=new SQLDMO.ColumnClass();
          dc.Name="SOYISIM";
          dc.Datatype="char";
          dc.Length=50;
          tbl.Columns.Add(dc); 
          /* Son olarak olusturulan TableClass nesnesi veritabanimizin tables koleksiyonuna ekleniyor. Böylece Sql sunucusunda yer alan veritabani içinde olusturulmasi saglanmis      oluyor. */
          db.Tables.Add(tbl); 
          durumCubugu.Text=tbl.Name.ToString()+" olusturuldu...";
     }
     catch(Exception hata)
     {
          MessageBox.Show(hata.Message);
     } 
} 
private void btnSunucuVeritabanlari_Click(object sender, System.EventArgs e)
{
     this.lstDatabases.Items.Clear();
     /* Öncelikle listBox nesnemize Sql Sunucusunda yer alan veritabanlarinin sayisini aktariyoruz.*/
     this.lstDatabases.Items.Add("Sunucudaki veritabani sayisi="+srv.Databases.Count); 
     /* Simdi bir for döngüsü ile, srv isimli SQLServerClass nesnemizin Databases koleksiyonunda geziniyoru ve her bir databaseClass nesnesinin adini alip listBox nesnemize aktariyoruz. Burada index degerinin 1 den basladigina sifirdan baslamadigina dikkat edelim. */
     for(int i=1;i<srv.Databases.Count;++i)
     {
          this.lstDatabases.Items.Add(srv.Databases.Item(i,srv).Name.ToString());
     }
} 
private void btnTablolar_Click(object sender, System.EventArgs e)
{
     /* Burada seçilen veritabanına ait tablolar listBox kontrolüne getiriliyor */
     this.lstTabels.Items.Clear();
     this.lstTabels.Items.Add("Tablo Sayisi="+srv.Databases.Item(this.lstDatabases.SelectedIndex,srv).Tables.Count.ToString()); 
     /* Döngümüz Sql Suncusundan yer alan veritabanı sayısı kadar süren bir döngü. */
     for(int i=1;i<srv.Databases.Item(this.lstDatabases.SelectedIndex,srv).Tables.Count;++i)
     {
          this.lstTabels.Items.Add(srv.Databases.Item(this.lstDatabases.SelectedIndex,srv).Tables.Item(i,srv). Name.ToString());
     }
} 
```

Şimdi uygulamamızı çalıştıralım ve öncelikle Sql Sunucumuza bağlanalım.

![mk18_3.gif](images/mk18_3.gif)

Şekil 3. Sunucumuza Bağlandık.

Şimdi bir veritabanı oluşturalım. Ben veritabanı adı olarak DENEME1 yazdım.

![mk18_4.gif](images/mk18_4.gif)

Şekil 4. Veritabanımız Oluşturuldu.

Şimdi ise tablo ismimizi yazalım. Ben deneme olarak Personel yazdım. Şimdi dilerseniz Sql Sunucumuzu bir açalım ve bakalım veritabanımız ve ilgili tablomu yaratılmışmı.

![mk18_5.gif](images/mk18_5.gif)

Şekil 5. Veritabanımız ve Tablomuz oluşturuldu.

Ve tablomuza baktığımızda oluşturduğumuz alanlarıda görebiliriz.

![mk18_6.gif](images/mk18_6.gif)

Şekil 6. Tablomuzdaki alanlar.

Son olarak suncumuzda yer alan veritabanlarının ve Deneme veritabanu altındaki tablolarında programımızda nasıl göründüğüne bakalım. Dikkat ederseniz tablolar ekrana geldiğinde sistem tablolarıda gelir. Bununla birlikte veritabanları görünürken sadece TempDBd veritabanı görünmez.

![mk18_7.gif](images/mk18_7.gif)

Şekil 7. Program ekranını son görüntüsü

Evet geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle, hepinize mutlu günler dilerim.
