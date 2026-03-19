---
title: "Tablo Değişikliklerini GetChanges ile İzlemek"
pubDate: 2004-01-29 12:00:00
categories:
  - Ado.Net
tags:
  - Ado.Net
  - C#
  - Web Service
  - DataSet
  - DataTable
---

# Tablo Değişikliklerini GetChanges ile İzlemek
Değerli Okurlarım, Merhabalar.

Bugünkü makalemizde, bağlantısız olarak veri tabloları ile çalışırken, bu tablolar üzerinde meydana gelen değişiklikleri nasıl izleyebileceğimizi ve davranışlarımızı bu değişikliklere göre nasıl yönledirebileceğimizi incelemeye çalışacağız.

Hepimizin bildiği gibi, bağlantısız veriler ile çalışırken, bir veri kaynağında, makinemizin belleğine tablo veya tabloları alırız. Bu tablolar üzerinde, yeni satırlar oluşturur, var olan satırlar üzerinde değişiklikler yapar, her hangibir satırı siler ve bunlar gibi bir takım işlemler gerçekleştiririz. Tüm bu işlemler, bellek bölgesine aldığımız veriler üzerinde bir DataTable nesnesinde yada bir DataSet kümesinde gerçekleşir. Bununla birlikte, bahsettiğimiz bu değişiklikleri, asıl veri kaynağınada yansıtarak, güncellenmelerinide sağlarız.

Ancak, network trafiğinin önemli ve yoğun olduğu uygulamalarda, veri kaynağından aldığımız bir veri kümesinin üzerindeki değişiklikleri, asıl veri kaynağına güncellerken karşımıza iki durum çıkar. İlk olarak, makinemizin belleğinde bulunan tüm veriler asıl veri kaynağına gönderilir ki bu veriler içinde hiç bir değişikliğe uğramamış olanlarda vardır. Diğer yandan istersek, sadece yeni eklenen satırları veya düzenlenen satırları vb., veri kaynağına gönderek daha akılcı bir iş yapmış oluruz. İşte makalemizin ana konusunu teşkil eden bu ikinci durumu gerçekleştirebilmek için GetChanges metodunu kullanırız. GetChanges metodu, DataSet ve DataTable sınıfları içinde kullanılabilmektedir. DataTable ve DataSet sınıfları için, GetChanges metodunun ikişer aşırı yüklenmiş şekli vardır.

DataTable İçin
DataSet İçin

public DataTable GetChanges ();

public DataSet GetChanges ();

public DataTable GetChanges (DataRowState rowStates);

public DataSet GetChanges (DataRowState rowStates);

Görüldüğü gibi her iki sınıf içinde metodlar aynı şekilde işlemektedir. Sadece metodların geri dönüş tipleri farklıdır. DataSet için, GetChanges metodu başka bir DataSet geri döndürürken, DataTable'ın GetChanges metodu ise geriye DataTable türünden bir nesne referansını döndürür. Peki GetChanges metodunun görevi nedir? GetChanges metodunun parametresiz kullanılan hali, DataTable veya DataSet için, AcceptChanges metodu çağırılana kadar meydana gelen tüm değişiklikleri alır. Örneğin bir DataTable nesnesinin referans ettiği bellek bölgesinde yer alan bir veri kümesi üzerinde, satır ekleme, satır silme ve satır düzenleme işlemlerini yaptığımızı farzedelim. Bu durumda, bu DataTable nesnesi için AcceptChanges metodunu çağırıp tüm değişiklikleri onaylamadan önce, GetChanges metodunu kullanırsak, tablo üzerindeki tüm değişiklikleri izleyebiliriz. Bunu daha iyi görmek için aşağıdaki örneği inceleyelim. Bu örnekte Sql sunucumuz üzerinde yer alan bir tablo verilerini DataTable nesnemizin bellekte referans ettiği bölgeye yüklüyor ve verilerin görüntüsünü DataGrid kontrolümüze bağlıyoruz. Programdaki önemli nokta, GetChanges metodu ile meydana gelen değişiklikleri başka bir DataTable nesnemize almamızdır. Bu DataTable nesnesinin verileride ikinci DataGrid kontrolümüzde görüntülenecektir. Ancak kullanıcı, DataTable'da meydana gelen bu değişiklikleri DataTable nesnesinin AcceptChanges metodunu kullanarak onayladığında, GetChanges geriye boş bir DataTable nesne referansı döndürecektir. Yani değişiklikleri, AcceptChanges metodu çağırılıncaya kadar elde edebiliriz. Öncelikle aşağıdaki Formu tasarlayalım.

![mk48_1.gif](images/mk48_1.gif)

Şekil 1. Form Tasarımımız.

Şimdide program kodlarımızı oluşturalım.

```csharp
SqlConnection conNorthwind; /*Sql sunucumuza yapıcağımız bağlantıyı sağlıyacak SqlConnection nesnemizi tanımlıyoruz.*/

SqlDataAdapter daPersonel; /* Personel tablosundaki verileri, dtPersonel tablosuna yüklemek için SqlDataAdapter nesnemizi tanımlıyoruz.*/

DataTable dtPersonel; /* Personel tablosundaki verilerin bellek görüntüsünü referans edicek DataTable nesnemizi tanımlıyoruz.*/

DataTable dtDegisiklikler; /* dtPersonel, DataTable nesnesi için AcceptChanges metodu uygulanana kadar meydana gelen değişikliklerin kümesini referans edicek DataTable nesnemizi tanımlıyoruz.*/

/* Kullanıcı bu butona bastığında, Sql sunucumuzdaki Personel tablosunun tüm satıları, dtPersonel DataTable nesnesinin bellekte işaret ettiği alana yüklenecek ve bu veriler DataGrid kontrolüne bağlanacak.*/

private void btnYukle_Click(object sender, System.EventArgs e)
{
     conNorthwind=new SqlConnection("data source=localhost;initial catalog=Northwind;integrated security=sspi");

     daPersonel=new SqlDataAdapter("Select * From Personel",conNorthwind);

     dtPersonel=new DataTable();
     daPersonel.Fill(dtPersonel);
     dgVeriler.DataSource=dtPersonel;
}

private void btnOnayla_Click(object sender, System.EventArgs e)
{
     dtPersonel.AcceptChanges(); /* dtPersonel tablosunda meydana gelen değişiklikleri onaylıyoruz.*/
}

private void btnDegisiklikler_Click(object sender, System.EventArgs e)
{
     dtDegisiklikler=new DataTable();
     dtDegisiklikler=dtPersonel.GetChanges(); /* GetChanges metodu ile dtPersonel DataTable nesnesinin işaret ettiği bellek bölgesinde yer alan veri kümesinde meydana gelen değişiklileri, dtDegisiklikler DataTable nesnesinin bellekte referans ettiği bölgeye alıyoruz. */
     dgDegisiklikler.DataSource=dtDegisiklikler; 
     /* Bu değişiklikleri DataGrid kontrolünde gösteriyoruz. */
}
```

Şimdi programımızı çalıştıralım ve tablomuzdaki veriler üzerinde değişiklik yapalım. Örneğin yeni bir satır girelim ve bir satır üzerinde de değişiklik yapalım.

![mk48_2.gif](images/mk48_2.gif)

Şekil 2. Yeni bir satır ekleyip bir satır üzerinde değişiklik yaptık.

Şimdi Değişiklikleri Al başlıklı butona tıkladığımızda, GetChanges metodu devreye girecek ve yaptığımız bu değişikliklerin meydana geldiği satırlar aşağıdaki gibi, dtDegisiklikler DataTable nesnesini bağladığımız DataGrid kontrolünde görünecek.

![mk48_3.gif](images/mk48_3.gif)

Şekil 3. Yapılan Değişikliklerin Görüntüsü.

Şimdi bu noktadan sonra, dtDegisiklikler isimli DataTable nesnesi üzerinden, SqlDataAdapter nesnesini Update metodunu kullanmak daha akıllıca bir yaklaşım olucaktır. Diğer yandan, GetChanges metodunun bu kullanımı, DataTable (DataSet) de meydana gelen her tür değişikliği almaktadır. Ancak dilersek, sadece yeni eklenen kayıtları ya da sadece değişiklik yapılan kayıtlarıda elde edebiliriz. Bunu gerçekleştirmek için, GetChanges metodunun DataRowState numaralandırıcısı türünden parametre aldığı versiyonunu kullanırız. Bu parametre her bir satırın yani DataRow nesnesinin durumunu belirtmektedir ve alabileceği değerler aşağıdaki tabloda verilmiştir.

DataRowState Değeri
Açıklama

Added

DataRowCollection koleksiyonuna yeni bir satır yani DataRow eklenmiş ve AcceptChanges metodu henüz çağırılmamıştır.

Deleted

Delete metodu ile bir satır silinmiştir.

Detached

Yeni bir satır, DataRowCollection için oluşturulmuş ancak henüz Add metodu ile bu koleksiyona dolayısıyla DataTable'a eklenmemiştir.

Modified

Satırda değişiklikler yapılmış ve henüz AcceptChanges metodu çağırılmamıştır.

Unchanged

Son AcceptChanges çağrısından bu yana, satırda herhangibir değişiklik olmamıştır.

Tablo 1. DataRowState Numaralandırıcısının Değerleri

Şimdi GetChanges metodunun, DataRowState numaralandırıcısı kullanılarak nasıl çalıştığını incelemeye çalışalım. Bunun için aşağıdaki örnek formu tasarlayalım. Bu kez programımızda, değişiklik olan satırları alıcak ve bunların durumlarınıda gösterecek bir uygulama oluşturacağız.

![mk48_4.gif](images/mk48_4.gif)

Şekil 4. Yeni Formumuz.

Formumuza bir ComboBox ekledik. Kullanıcı bu ComboBox'tan DataRowState değerini seçicek ve GetChanges metodumuz buna göre çalışacak. ComboBox'ımızın öğeleri ise şunlar olucak;

![mk48_5.gif](images/mk48_5.gif)

Şekil 5. ComboBox öğelerimiz.

Uygulamamıza sadece, Duruma Göre Değişiklikleri Al başlıklı butonumuzun kodlarını ekleyeceğiz.

```csharp
private void btDurumaGoreDegisiklikler_Click(object sender, System.EventArgs e)
{
     dtDegisiklikler=new DataTable();
     if(cmbRowState.SelectedIndex==0)
     {          dtDegisiklikler=dtPersonel.GetChanges(DataRowState.Detached); /* Yeni açılan ancak henüz DataRowCollection'a eklenmeyen satırlar.*/

          dgDegisiklikler.DataSource=dtDegisiklikler;
     }
     else if(cmbRowState.SelectedIndex==1)
     {          dtDegisiklikler=dtPersonel.GetChanges(DataRowState.Added); /* DataRowCollection'a yeni eklenen satırlar.*/
          dgDegisiklikler.DataSource=dtDegisiklikler;
     }
     else if(cmbRowState.SelectedIndex==2)
     {          dtDegisiklikler=dtPersonel.GetChanges(DataRowState.Deleted); /* DataTable'dan silinen satırlar.*/
          dgDegisiklikler.DataSource=dtDegisiklikler;
     }
     else if(cmbRowState.SelectedIndex==3)
     {          dtDegisiklikler=dtPersonel.GetChanges(DataRowState.Modified); /* Değişikliğe uğrayan satırlar. */
          dgDegisiklikler.DataSource=dtDegisiklikler;
     }
     else if(cmbRowState.SelectedIndex==4)
     {          dtDegisiklikler=dtPersonel.GetChanges(DataRowState.Unchanged); /* Son AcceptChanges'den sonra değişikliğe uğramamış satırlar.*/
          dgDegisiklikler.DataSource=dtDegisiklikler;
}
```

Şimdi uygulamamızı çalıştıralım ve deneyelim. Tablomuzda yine birtakım değişiklikler yapalım. Örneğin satırlar ekleyelim, satırları güncelleyelim (Satır eklemek ve satır güncellemek en çok yaptığımız işlemlerdir dikkat ederseniz). Sonrada ComboBox kontrolümüzden istediğimiz durumu seçip elde ettiğimiz sonucu görelim. Örneğin ben yeni bir satır ekledim ve bir satır üzerinde değişiklik yaptım. Daha sonra sadece yeni eklenen satırları görmek için ComboBox kontrolünde Yeni Ekelenen Satılar (Added) seçeneğini seçip düğmeye bastım. Bu durumda if koşumuz durumu değerlendirir ve GetChanges metodunu DataRowState.Added parametresi ile uygular. Sonuç olarak, değişiklik yaptığım satır görünmez sadece yeni eklenen satır dtDegisiklikler tablosuna alınır.

![mk48_6.gif](images/mk48_6.gif)

Şekil 6. DataRowState.Added Sonrası.

Bu noktadan sonra artık bir veri tablosunu güncellerken, GetChanges yaklaşımını kullanarak, örneğin sadece yeni eklenen satırların veri kaynağına gönderilmesini sağlamış oluruz. Buda bize daha hızlı ve rahat bir ağ trafiği sağlayacaktır. Bu durum özellikle web servisleri için çok idealdir. Uzak sunuculardan ilgili verileri bilgisayarına bağlantısız olarak işlemek için alan bir istemci uygulama, veri kümesinin tamamını geri göndermek yerine, sadece yeni eklenen veya güncellenen satırları temsil eden bir veri kümesini (dataTable veya DataSet) geri göndererek sınırlı internet kapasitesi için en uygun başarımı elde edebilir. Geldik bir makalemizin daha sonuna. Hepinize mutlu günler dilerim.
