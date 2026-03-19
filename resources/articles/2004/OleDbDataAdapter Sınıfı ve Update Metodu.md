---
title: "OleDbDataAdapter Sınıfı ve Update Metodu."
pubDate: 2004-03-14 12:00:00
categories:
  - Ado.Net
tags:
  - Ado.Net
  - C#
  - Performance
  - DataTable
---

# OleDbDataAdapter Sınıfı ve Update Metodu.
Değerli Okurlarım, Merhabalar.

Bu makalemizde, OleDbDataAdapter sınıfının, veriler üzerindeki güncelleme işlemlerinin, veri kaynağına yansıtılması sırasında nasıl bir rol oynadığını ve kullanıldığını incelemeye çalışacağız. Önceki makalelerimizde belirttiğimiz gibi, OleDbDataAdapter nesnesi yardımıyla veri kaynağından, uygulamalarımızdaki bağlantısız katman nesnelerine veri kümelerini aktarmak amacıyla Fill metodunu kullanıyorduk. Diğer yandan, bağlantısız katman nesnelerimizin temsil ettiği veriler üzerinde yapılan değişiklikleri veritabanına göndermek istersek, Update metodunu kullanırız.

Update metodu çalışma sistemi açısından oldukça ilgi çekici bir metoddur. Bildiğiniz gibi, DataAdapter nesnelerinin, verilerin güncellenmesi için UpdateCommand, verileri eklemek için InsertCommand, veri silmek için DeleteCommand özellikleri vardır. Uygulamamız çalışırken, bağlantısız katman nesnelerimiz verilerin satırsal bazda durumlarını gösteren bir değer içeririr. RowState olarak bilinen bu özellik DataRow sınıfına ait bir özellik olup aşağıdaki tabloda yer alan DataRowState numaralandırıcısı türünden değerlerden birisini almaktadır.

DataRowState Değeri
Açıklama

Added
Yeni bir satır eklendiğini belirtir.

Deleted
Bir satırın silindiğini belirtir.

Modified
Bir satırın düzenlendiğini belirtir.

Detached
Yeni bir satır oluşturulduğunu ama henüz ilgili bağlantısız katman nesnesinin DataRow koleksiyonuna eklenmediğini belirtir.

Unchanged
Satırda herhangibir değişiklik olmadığını belirtir.

Tablo1. RowState özelliğinin DataRowState numaralandırıcısı tipinden alabileceği değerler.

Buradan yola çıkarasak, Update metodu uygulandığında, OleDbDataAdapter nesnesi, parametre olarak belirtilen DataTable nesnesinin tüm satırlarına bakıcaktır. Bu satırlarda yukarıdaki değerleri arıyacaktır. Sonuç olarak, Added satırları için, InsertCommand özelliğindeki sql komutunu, Deleted satırlar için DeleteCommand özelliğindeki sql komutunu, Modified satırlar için ise, UpdateCommand özelliğindeki sql komutunu çalıştıracak, böylece uygun güncellemelerin veritabanına en doğru sql ifadeleri ile aktarılmalarını sağlayacaktır.

Elbette Update komutunun başarıya ulaşması, Fill metodunun geçerli bir SelectCommand sql komutunu çalıştırmasına bağlıdır. Çünkü, diğer komutların parametreleri bu select sorgusu ile elde edilen tablo alanlarından oluşturulacaktır. Ayrıca, DeleteCommand ve UpdateCommand özelliklerinin sahip olduğu sql komutları Where koşuluna sahiptirler ve bu koşul için çoğunlukla tabloya ait Primary Key (Birincil Anahtar) alanını parametre olarak kullanırlar. Bu sebeple, tablonun birincil anahtara sahip olması önemlidir.

Bahsetmiş olduğumuz güncelleme komutlarını elle programlayabileceğimiz gibi, bu işlemi bizim için basitleştiren CommandBuilder sınıfınıda kullanabiliriz. Şimdi dilerseniz, OleDbCommandBuilder sınıfı yardımıyla bu işlemin nasıl gerçekleştirileceğini bir örnek üzerinde inceleyelim. Örneğimizde, basit bir sql tablosunu kullanacağız. Öncelikle programımızın kodlarını yazalım.

```csharp
/* global seviyede gerekli nesnelerimizi tanımlıyoruz. Sql sunucusuna bağlantımız için bir oleDbConnection nesnesi, verileri tablodan çekmek ve DataTable nesnemize aktarmak, güncellemeleride aynı dataTable nesnesi üzerinden veri kaynağına göndermek için bir DataAdapter nesnesi, bağlantısız katmanda verilerimizi tutmak için bir dataTable nesnesi ve OleDbDataAdapter nesnemiz için gerekli Update,Delete,Insert komutlarını oluşturacak bir OleDbCommandBuilder nesnesi. */
OleDbConnection con;
OleDbDataAdapter da;
DataTable dt;
OleDbCommandBuilder cb;

private void btnDoldur_Click(object sender, System.EventArgs e)
{
    con=new OleDbConnection("Provider=SQLOLEDB;data source=localhost;database=Friends;integrated security=sspi"); /* Bağlantımız oluşturuluyor. */
    da=new OleDbDataAdapter("Select * From Kisiler",con); /* DataAdapter nesnesmiz, select sorgusu ile birlikte oluşturuluyor. */
    dt=new DataTable("Kisiler"); /*DataTable nesnemiz oluşturuluyor. */
    da.Fill(dt); /* DataTable nesnemizin bellekte gösterdiği alan Kisiler tablosundaki veriler ile dolduruluyor. */
    dgKisiler.DataSource=dt; /* DataGrid kontrolümüz, bağlantısız katmandaki verileri işaret eden DataTable nesnemize bağlanıyor. */
}

private void btnGuncelle_Click(object sender, System.EventArgs e)
{
    try
    {
        cb=new OleDbCommandBuilder(da); /* CommandBuilder nesnemiz , OleDbDataAdapter nesnemiz için oluşturuluyor. CommandBuilder'a ait new yapılandırıcısı parametre olarak aldığı OleDbDataAdapter nesnesinin SelectCommand özelliğindeki sql komutuna bakarak gerekli diğer UpdateCommand,DeleteCommand ve InsertCommand komutlarını oluşturuyor. */

        da.Update(dt); /* DataTable'daki değişiklikler Update metodu ile, veritabanına gönderiliyor. */
    }
    catch(Exception hata)
    {
        MessageBox.Show(hata.Message.ToString());
    }
}
```

Uygulamamızı çalıştırdığımızda ve Doldur isimli butona tıkladığımızda, tablomuza ait verilerin dataGrid kontrolüne yüklendiğini görürüz.

![mk59_1.gif](images/mk59_1.gif)

Şekil 1. Fill metodunun çalıştırılması sonucu.

Şimdi yeni bir satır ekleyip bir kaç satır üzerinde değişiklik yapalım ve başka bir satırıda silelim.

![mk59_2.gif](images/mk59_2.gif)

Şekil 2. Update komutunu çalıştırmadan önceki hali.

Güncelle başlıklı butona tıkladığımızda, OleDbCommandBuilder nesnemiz, OleDbDataAdapter nesnemiz için gerekli olan komutları oluşturur. Daha sonra Update metodu çalıştırılmaktadır. Update tablomuzda yapmış olduğumuz düzenleme, silme ve ekleme işlemlerini görmek için, DataTable nesnemizin DataRow koleksiyonundaki her bir DataRow nesnesi için RowState özelliklerinin değerlerine bakar ve uygun olan sql komutlarına bu satırlardaki değerleri parametreler vasıtasıyla aktararak veritabanının güncellenmesini sağlar. Bu noktadan sonra veritabanımızdaki tablomuza baktığımızda bağlantısız katman nesnesinin işaret ettiği bellek alanındaki tüm değişikliklerin yansıtıldığını görürüz.

![mk59_3.gif](images/mk59_3.gif)

Şekil 3. Tabloya yansıtılan değişiklikler.

Dilerseniz CommandBuilder nesnemizin bizim için oluşturmuş olduğu komutların nasıl sql ifadeleri içerdiğini inceleyelim. Bu amaçla, OleDbCommandBuilder sınıfına ait aşağıda prototipleri belirtilen metodları kullanacağız.

Metod
Prototipi

GetInsertCommand
public OleDbCommand GetInsertCommand ();

GetDeleteCommand
public OleDbCommand GetDeleteCommand ();

GetUpdateCommand
public OleDbCommand GetUpdateCommand ();

Tablo 2. OleDbCommandBuilder için Get metodlar.

Dikkat edecek olursanız tüm bu metodlar geriye OleDbCommand sınıfı türünden bir nesne değeri döndürmektedir. Uygulamamızdaki btnGuncelle kodlarını aşağıdaki gibi düzenlediğimizde, OleDbCommandBuilder nesnesinin, OleDbDataAdapter nesnesi için oluşturmuş olduğu komutları görebiliriz.

```csharp
OleDbCommand cmdInsert=new OleDbCommand();
cmdInsert=cb.GetInsertCommand();
MessageBox.Show("Insert Sql Ifadesi :"+cmdInsert.CommandText.ToString());

OleDbCommand cmdDelete=new OleDbCommand();
cmdDelete=cb.GetDeleteCommand();
MessageBox.Show("Delete Sql Ifadesi :"+cmdDelete.CommandText.ToString());

OleDbCommand cmdUpdate=new OleDbCommand();
cmdUpdate=cb.GetUpdateCommand();
MessageBox.Show("Update Sql Ifadesi :"+cmdUpdate.CommandText.ToString());
```

Şimdi uygulamamızı çalıştıralım.

![mk59_5.gif](images/mk59_5.gif)

Şekil 4. CommandBuilder nesnemizin oluşturduğu Delete sql komutu.

![mk59_4.gif](images/mk59_4.gif)

Şekil 5. CommandBuilder nesnemizin oluşturduğu Insert sql komutu.

![mk59_6.gif](images/mk59_6.gif)

Şekil 6. CommandBuilder nesnemizin oluşturduğu Update sql komutu.

Görüldüğü gibi işlem bu kadar basittir. Ancak dilersek CommandBuilder nesnesini kullanmayıp, DataAdapter nesnemiz için gerekli sql komutlarını kendimizde yazabiliriz. Bu biraz daha uzun bir yöntem olmakla birlikte, daha çok kontrole sahip olmamızı sağlar. Ayrıca her programlama dilinde olduğu gibi, işleri böylesine kolaylaştırıcı nesneler performans kaybına neden olabilmektedir. Bu nedenlerden ötürü, OleDbDataAdapter nesnemizin ihtiyaç duyduğu komutları kendimiz yazmak isteyebiliriz. Burada önemli olan nokta gerekli parametrelerin doğru bir şekilde oluşturulmasıdır. Şimdi yukarıda CommandBuilder sınıfı yardımıyla geliştirdiğimiz uygulamayı yineleyelim.

```csharp
OleDbConnection con;
OleDbDataAdapter da;
DataTable dt;

private void btnDoldur_Click(object sender, System.EventArgs e)
{
    con=new OleDbConnection("Provider=SQLOLEDB;data source=localhost;database=Friends;integrated security=sspi"); 
    da=new OleDbDataAdapter("Select * From Kisiler",con);
    dt=new DataTable("Kisiler"); 
    da.Fill(dt); 
    dgKisiler.DataSource=dt; 
}

private void btnGuncelle_Click(object sender, System.EventArgs e)
{
    try
    {
        da.InsertCommand=new OleDbCommand("INSERT INTO Kisiler (Ad,Soyad,DogumTarihi,Meslek) VALUES (?,?,?,?)",con);
        da.InsertCommand.Parameters.Add("prmAd",OleDbType.VarChar,50,"Ad");
        da.InsertCommand.Parameters.Add("prmSoyad",OleDbType.VarChar,50,"Soyad");
        da.InsertCommand.Parameters.Add("prmDogum",OleDbType.Date,8,"DogumTarihi");
        da.InsertCommand.Parameters.Add("prmMeslek",OleDbType.VarChar,50,"Meslek");

        da.UpdateCommand=new OleDbCommand("UPDATE Kisiler SET Ad=?,Soyad=?,DogumTarihi=?,Meslek=? WHERE KisiID=?",con);
        da.UpdateCommand.Parameters.Add("prmKID",OleDbType.Integer,4,"KisiID");
        da.UpdateCommand.Parameters.Add("prmSoyad",OleDbType.VarChar,50,"Soyad");
        da.UpdateCommand.Parameters.Add("prmDogum",OleDbType.Date,8,"DogumTarihi");
        da.UpdateCommand.Parameters.Add("prmMeslek",OleDbType.VarChar,50,"Meslek");
        da.UpdateCommand.Parameters.Add("prmKisiID",OleDbType.Integer,4,"KisiID");

        da.DeleteCommand=new OleDbCommand("DELETE FROM Kisiler WHERE KisiID=?",con);
        da.DeleteCommand.Parameters.Add("prmKisiID",OleDbType.Integer,4,"KisiID");

        da.Update(dt); 
    }
    catch(Exception hata)
    {
        MessageBox.Show(hata.Message.ToString());
    }
}
```

Burada tanımladığımız komutlar için gerekli parametreleri oluştururken Parameters koleksiyonunun Add metodunun aşağıdaki prototipini kullandık.

```csharp
public OleDbParameter Add(string parameterName,OleDbType oleDbType,int size, string sourceColumn);
```

Buradaki parametreleri kısaca açıklayacak olursak; ilk parametremiz, komutumuz için kullanacağımız parametre adı. İkinci parametremizde ise tablodaki alanımızın veri tipini belirliyoruz. Buradaki veri tipleri OleDbType türündendir. Üçüncü parametremizde ise alanın büyüklüğünü belirtiyoruz. Son parametremiz ise, sql komutu içindeki bu parametrenin hangi alan için kullanılacağını belirtmektedir ve bu anlamı nedeniylede oldukça önemlidir. Dikkat ederseniz OleDb sınıfında OleDbParameter türündeki parametreleri sql komutları içinde? ile belirttik. Bu nedenle, parametrelerimizi, ilgili sql komutu nesnesinin OleDbParameter koleksiyonuna eklerken? sırasına göre tanımlamalıyız. Şimdi uygulamamızı çalıştıralım ve veriler üzerinde aşağıdaki görünen değişiklikleri yapalım.

![mk59_7.gif](images/mk59_7.gif)

Şekil 7. Değişikliklerimiz yapılıyor.

Şimdi Güncelle başlıklı butonumuza tıklayalım. Değişikliklerin tanımladığımız sql komutları yardımıyla veritabanınada yansıtıldığını görürüz.

![mk59_8.gif](images/mk59_8.gif)

Şekil 8. Değişikliklerimiz veritabanına yansıtıldı.

Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde, OleDbDataAdapter sınıfına ait olayları incelemeye çalışacağız. Şimdilik görüşmek dileğiyle, hepinize mutlu günler dilerim.
