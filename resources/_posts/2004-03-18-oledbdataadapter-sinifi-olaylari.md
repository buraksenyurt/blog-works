---
layout: post
title: "OleDbDataAdapter Sınıfı Olayları"
date: 2004-03-18 12:00:00 +0300
categories:
  - ado-net
tags:
  - ado.net
  - oledb
  - oledbdataadapter
  - dataadapter
  - csharp
  - sql
  - query
  - aggregation
---
Bu makalemizde, OleDbDataAdapter sınıfının olaylarını incelemeye çalışacağız. OleDbDataAdapter sınıfı aşağıdaki tabloda belirtilen üç önemli olayı içermektedir.

Olay
Prototipi
Açıklama

FillError
public event FillErrorEventHandler FillError;
OleDbDataAdapter'ın fill metodu kullanıldığında oluşabilecek bir hata durumunda bu olay çalışır.

RowUpdating
public event OleDbRowUpdatingEventHandler RowUpdating;
Update metodu çalıştırılarak, veritabanındaki tabloya yapılan değişiklikler (satır ekleme, satır silme, satır güncelleme gibi) gerçekleştirilemden önce bu olay çalışır.

RowUpdated
public event OleDbRowUpdatedEventHandler RowUpdated;
Veritabanında yapılacak olan değişiklikler, Update metodu ile gerçekleştirildikten sonra bu olay çalışır.

Tablo 1. OleDbDataAdapter olayları.

Şimdi dilerseniz bu olayları kısaca incelemeye çalışalım. RowUpdating olayından başlayalım. Bu olay, OleDbRowUpdatingEventArgs sınıfı türünden bir parametre almaktadır. Bu paramterenin sahip olduğu özellikleri kullanarak, bağlantısız katmandaki veriler, veritabanına yazılmadan önce değişik işlevleri yerine getirme imkanına sahip olmuş oluruz. OleDbRowUpdatingEventArgs sınıfının özellikleri aşağıdaki tabloda yer almaktadır.

OleDbRowUpdatingEventArgs Sınıfı Özellikleri

Özellik
Görevi
Prototipi

Command
Update metodu çalıştırılıdığında çalışacak olan OleDbCommand tipindeki komutu işaret eder. Bu nedenle özelliğin değerinin tipi OleDbCommand'dır.
public new OleDbCommand Command {get; set;}

Row
Update metodu çağırıldığında, veritabanına gönderilecek olan satırı işaret eder. Bu satır DataRow tipindedir ve bu nedenle özelliğin veri tipi DataRow'dur.
public DataRow Row {get;}

Status
Bu özellik ile çalışacak olan komut nesnesinin durumu elde edilir veya değiştirilir. Özellik UpdateStatus numaralandırıcısı türünden bir değeri belirtir. Bu numaralandırıcı Continue, ErrorsOccurred, SkipAllRemainingRows, SkipCurrentRow değerlerinden birisini alır.
public UpdateStatus Status {get; set;}

StatementType
Bu özellik Update metodu ile çalıştırılacak olan sql ifadesini işaret etmektedir. StatementType numaralandırıcısı tipinden bir değeri belirtir. Bu numaralandırıcı sql ifadesinin tipini belirten select, insert, delete ve update değerlerinden birisini alır.
public StatementType StatementType {get;}

TableMapping
Bu özellik, update metodu çalıştırıldığında, bağlantısız katman nesnesindeki tablo haritası ile, veritabanındaki tablo arasındaki eşleştirme ilişkisini DataTableMapping sınıfı örneği olan bir nesne ile ifade eder.
public DataTableMapping TableMapping {get;}

Errors
Update metodu çalıştırılıdığında işletilen sql komutunun çalışmasında bir hata oluşması durumunda, oluşan hatayı temsil eden bir özelliktir. Bu sebepler özelliğin tipi Exception'dır.
public Exception Errors {get; set;}

Tablo 2. OleDbRowUpdatingEventArgs Sınıfı Özellikleri

Bu özelliklerin, RowUpdating metodu içinde nasıl işlendiğini örnekler ile incelemeden önce, RowUpdating olayının işleme geçme sürecini ve yerini incelemekte fayda olduğu kanısındayım. RowUpdating olayının, Update metodu içindeki sql komutları çalıştırılıp, gerekli değişiklikler veritabanına yansıtılmadan önce gerçekleştiğini söyleyebiliriz. Aşağıdaki şekil bu konuda bizlere daha iyi bir fikir verecektir.

![mk60_1.gif](/assets/images/2004/mk60_1.gif)

Şekil 1. RowUpdating ve RowUpdated olaylarının devreye girdiği noktalar.

Şimdi RowUpdating olayını incelemeye başlayalım. Öncelikle basit bir windows uygulaması oluşturalım. Bu uygulamada, Friends isimli veritabanındaki, Kisişer isimli tablomuzun verilerini kullanacağız. Şimdi uygulamamızın başlangıç kodlarını oluşturalım. OleDbDataAdapter nesnemiz için, RowUpdating olayının nasıl eklendiğine dikkatinizi çekmek isterim.

```csharp
/* Sql veritabanındaki Friends isimli veritabanındaki Kisiler isimli tabloya bağlanabilmek için bize gerekli olan nesneleri tanımlıyoruz. Bir OleDbConnection nesnesi, sql veritabanına bağlantı hattı çekmek için; bir OleDbDataAdapter nesnesi, Kisiler tablosundaki verileri, bağlantısız katman nesnemiz olan DataTable'ın veritabanında gösterdiği alana yüklemek ve verilerdeki değişiklikleri veritabanına yazmak için.*/
OleDbConnection con;
OleDbDataAdapter da;
DataTable dt; 

private void Form1_Load(object sender, System.EventArgs e)
{
    con=new OleDbConnection("provider=SQLOLEDB;data source=localhost;integrated security=sspi;database=Friends"); /* Bağlantı hattımız oluşturuluyor.*/
    da=new OleDbDataAdapter("Select * From Kisiler",con); /* OleDbDataAdapter nesnemiz oluşturuluyor. */
    da.RowUpdating+=new OleDbRowUpdatingEventHandler(RowUpdatingOlayi); /*OleDbDataAdapter nesnemiz için, RowUpdating olayını tanımlıyor ve oluşturuyoruz.*/
    dt=new DataTable("Kisiler"); /* DataTable nesnemizin oluşturuluyor. */
}

private void btnDoldur_Click(object sender, System.EventArgs e)
{
    da.Fill(dt); /* DataTable nesnemizin bellekte işaret ettiği bölge, Kisiler tablosundaki veriler ile dolduruluyor.*/
    dataGrid1.DataSource=dt; /* DataGrid nesnemize veri kaynağı olarak DataTable nesnemiz atanıyor.*/
}
private void btnGuncelle_Click(object sender, System.EventArgs e)
{         OleDbCommandBuilder cb=new OleDbCommandBuilder(da); /* OleDbDataAdapter nesnemiz için gerekli insert,delete ve update sql komutlarını otomatik olarak OleDbCommandBuilder yardımıyla oluştuyuroz.*/
    da.Update(dt); /* DataTable'daki değişiklikler veritabanına gönderiliyor.*/
}

/* RowUpdating olayımız tetiklendiğinde bu yordamımız çalıştırılacak. */
private void RowUpdatingOlayi(object Sender,OleDbRowUpdatingEventArgs arg)
{

}
```

Şimdi RowUpdating olayımızı incelemeye başlayalım. Örneğin, Row ve Status özelliklerini bir arada inceleyelim. Farzedelimki, KisiID numarası 1000 olan satırının hiç bir şekilde güncellenmesini istemiyoruz. Bu durumu değerlendirebileceğimiz en güzel yer, Update işlemi başarı ile gerçekleşmeden önceki yerdir. Yani RowUpdating metodu.

```csharp
private void RowUpdatingOlayi(object sender,OleDbRowUpdatingEventArgs arg)
{
    if(arg.StatementType==StatementType.Update) /* Eğer şu an yapılan işlem OleDbDataAdapter nesnesinin UpdateCommand metodunun içerdiği OleDbCommand'ı çalıştıracaksa bu kod bloğu devreye giriyor.*/
    {
        if(arg.Row["KisiID"].ToString()=="1000") /* Şu an işlemde olan satırın KisiID alanının değerine bakıyoruz.*/
        {
            listBox1.Items.Add("1000 nolu kaydı güncelleyemessiniz.");
            arg.Status=UpdateStatus.SkipCurrentRow; /* SkipCurrentRow ile bu satırın güncellenmesini engelliyoruz.*/
        } 
    } 
}
```

Bu örnek kodlar ile uygulamamızı çalıştırdığımızda, KisiID alanının değerinin 1000 olduğu satırın DataTable üzerinde değiştirilsede, veritabanı üzerinde değiştirilmediğini görürüz. Ancak diğer satırlardaki değişiklikler veritabanına yansıtılır.

![mk60_2.gif](/assets/images/2004/mk60_2.gif)

Şekil 2. Uygulamanın Çalışması.

Burada Ad alanındaki Burak Selim değerini Burak S. olarak değiştirdik. Bu değişiklik DataTable üzerinde gerçekleşmiştir. Ancak bunu veritabanınada yansıtmak istediğimizde, RowUpdating olayındaki karşılaştırma ifadeleri devreye girecek ve değişiklik veritabanına yansıtılmayacaktır. Visual Studio.NET ortamından tablo içeriğinde baktığımızda bu değişikliğin gerçekleşmediğini görürüz.

![mk60_3.gif](/assets/images/2004/mk60_3.gif)

Şekil 3. Değişiklik veritabanındaki tabloya yansıtılmadı.

RowUpdating olayı ile ilgili verilebilecek bir diğer güzel örnek ise, henüz güncellenmiş olan bir satırın başka bir kullanıcı tarafından güncellenmek istenmesi gibi bir durumu kontrol altına almaktır. Bu olayda, o anki satıra ait Orjinal değerlere bakarak, güncellenmek istenen değerler ile aynı olup olmadığı araştırılır. Böyle bir sonuç çıkarsa kullanıcıya bu satırın zaten güncellendiği tekrar güncellemek isteyip istemeyeceği sorulabilir. Bu işlevi yerine getirmek için RowUpdating olayımızı aşağıdaki gibi şekillendirebiliriz.

```csharp
private void RowUpdatingOlayi(object sender,OleDbRowUpdatingEventArgs arg)
{
    if(arg.StatementType==StatementType.Update)
    {
        string sqlKomutu="SELECT * FROM Kisiler WHERE KisiID='"+arg.Row["KisiID",DataRowVersion.Original]+"' AND Ad='"+arg.Row["Ad",DataRowVersion.Original]+"' AND Soyad='"+arg.Row["Soyad",DataRowVersion.Original]+"' AND DogumTarihi='"+arg.Row["DogumTarihi",DataRowVersion.Original]+"' AND Meslek='"+arg.Row["Meslek",DataRowVersion.Original]+"'"; 
        OleDbCommand cmd=new OleDbCommand(sqlKomutu,con);
        con.Open();
        if(cmd.ExecuteNonQuery()==0)
        {
            listBox1.Items.Add("Bu satır zaten güncellenmiş.");
            arg.Status=UpdateStatus.SkipCurrentRow;
        }
    } 
}
```

Burada öncelikle bir select sorgusu oluşturuyoruz. Bu sorgu, Update komutu çağırıldığında, OleDbDataAdapter nesnesinin ilgili komutlarına gönderilen satıra ait alanların, en son Fill metodunun çağırılışından sonraki hallerine bakıyor. Eğer aynı güncellemeler başka bir kullanıcı tarafından yapılmış ise, bu güncel satırın o anki değeri ile veritabanındaki aynı olmayacaktır. Dolayısıyla, orjinal değerler değişmiş olacağından select sorgusunun çalışması sonucu geriye 0 değeri dönecektir. Bu başka bir kullanıcının bu satırı güncellediğini göstermektedir. Bu halde iken kullanıcı uyarılır. İstersek buraya, bir soru kutucuğu açarak kullanıcının bu satırı tekrardan güncellemek isteyip istemediği sorulabilir. Ben bunun geliştirilmesini siz değerli okurlarıma bırakıyorum. If döngümüz içindede bu satır eğer daha önceden güncellenmiş ise, SkipCurrentRow değerini, OleDbRowUpdatingEventArgs sınıfının Status özelliğine atayarak bu satırın güncellenmemesini sağlıyoruz. Şimdi uygulamamızı çalıştırıp deneyelim. Bunun için aynı programı kendi bilgisayarınızda iki kez açmanız yeterli olucaktır.

![mk60_5.gif](/assets/images/2004/mk60_5.gif)

Şekil 4. İlk hali.

Önce, soldaki pencerede görülen uygulamada Nihat Ali Demir'in soyadını D. olarak değiştiriyoruz ve Guncelle başlıklı butona basarak bu satırı veri tabanında da güncelliyoruz. Şimdi ekranın sağındaki kullanıcınında aynı soyadını aynı şekilde değiştirmek istediğini düşünelim. Bu amaçla sağdaki programda yine Nihat Ali Demir'in soyadını D. yapıyoruz ve Guncelle başlıklı butona tıklıyoruz. Bu andan itibaren bizim RowUpdating olayına yazdığımız kodlar devreye giriyor. Satırın, Soyad alanının, Fill metodunun çağırılması ile birlikte orjinal değeri halen Demir dir. Şimdi bunu D. nokta yapmak istediğimizde, öncelikle orjinal alan değeri olan Demir select sorgumuza girer. Bu sorgu çalıştığında böyle bir satır bulunamayacaktır. Çünkü Demir, D. ile değiştirilmiştir. Ancak ikinci program fill metodunu bu son güncellemeden sonra çağırmadığı için durumdan habersizdir. Bu nedenle ikinci programın yapmak istediği değişiklik zaten yapılmış olduğundan geri alınacaktır.

![mk60_6.gif](/assets/images/2004/mk60_6.gif)

Şekil 5. İkinci programın aynı güncellemeyi yapması engellenir.

Bununla birlikte ikinci kullanıcının bu noktadan sonra, D. ismini Demirci olarak değiştirmek istediğini yani farklı bir veri girdiğini farzedelim. Bu değişiklik gerçekleşecektir. Bu durumu şöyle açıklayabiliriz. İkinci program, Demir alanını D. nokta yapmaya çalıştığında, bu güncelleme diğer program tarafından yapılmış olduğundan, satırın güncellenmesi geri alınır. Ancak bu noktada Soyad alanının orjinal değeride değişir ve D. olur. İşte bu nedenle bu noktadan sonra ikinci program bu alanın değerini başka bir değer ile değiştirebilecektir.

Gelelim RowUpdated olayına. Bu olay ise, Şekil 1'de görüldüğü gibi, veritabanına olan güncelleme işlemleri tamamıyla gerçekleştirildikten sonra oluşur ve eklenen, silinen, yada güncellenen her satır için tetiklenir. Bu olayın OleDbDataRowUpdatedEventArgs sınıfı türünden bir parametresi vardır. Bu sınıfın özellikleri OleDbDataRowUpdatingEventArgs sınıfının özellikleri ile aynıdır. Bununla birlikte kullanabileceğimiz ekstradan bir özelliği daha vardır. Bu özellik, RecordsAffected özelliğidir. Bu özellik ile, yapılan güncelleştirmeler sonucu etkilenen satır sayısını elde edebiliriz. Dilerseniz, bu olayı kodumuza uygulayalım. Örneğin yaptığımız güncelleştirmeler sonucu, bu güncelleştirmelerden etkilenen satır sayısını elde etmeye çalışalım. Öncelikle OleDbDataAdapter nesnemize, bu olayımızı ekliyoruz.

```csharp
da.RowUpdated+=new OleDbRowUpdatedEventHandler(RowUpdatedOlayi);
```

Şimdide program kodlarımızı aşağıdaki gibi güncelleyelim.

```csharp
private void btnGuncelle_Click(object sender, System.EventArgs e)
{
    listBox1.Items.Clear();
    OleDbCommandBuilder cb=new OleDbCommandBuilder(da); 
    da.Update(dt); 
    dt.AcceptChanges();

    listBox1.Items.Add("Girilen :"+girilen.ToString());
    listBox1.Items.Add("Silinen :"+silinen.ToString());
    listBox1.Items.Add("Guncellenen :"+guncellenen.ToString());
}

public int girilen=0,silinen=0,guncellenen=0;

private void RowUpdatedOlayi(object sender,OleDbRowUpdatedEventArgs arg)
{
    if(arg.StatementType==StatementType.Insert)
    {
        girilen+=arg.RecordsAffected;
    }
    else if (arg.StatementType==StatementType.Delete)
    {
        silinen+=arg.RecordsAffected;
    }
    else if (arg.StatementType==StatementType.Update)
    {
        guncellenen+=arg.RecordsAffected;
    } 
}
```

Burada yaptığımız son derece basit. RowUpdated olayı, veritabanına girilecek, güncellenecek veya veritabanından silinecek her bir satır için tetiklendiğinden, bu olay yordamı içinde, o anki satır için çalıştırılacak sql ifadesinin ne olduğunu temin ediyoruz. Bunun içinde, OleDbRowUpdatedEventArgs parametresinin StatementType özelliğinin değerine bakıyoruz. Uygun değerlere görede, public integer sayaçlarımızın değerlerini arttıyoruz. Böylece insert,update ve delete işlemlerinin kaç satıra uygulandığını tespit etmiş oluyoruz.

![mk60_4.gif](/assets/images/2004/mk60_4.gif)

Şekil 6. Güncelleme sayılarının elde edilmesinin sonucu.

Makalemizde son olarak FillError olayını ele almaya çalışacağım. Bu olay bahsettiğimiz gibi Fill metodu uygulandığında oluşabilecek hatalarda devreye girmektedir.FillError olayı FillErrorEventArgs sınıfı türünden bir parametre alır. FillErrorEventArgs sınıfının, FillError olayı için kullanabileceğimiz özellikleri aşağıdaki tabloda yer almaktadır.

FillErrorEventArgs Özelliği
Açıklama

Continue
Fill metodu ile karşılaşıldığında bir hata oluşduğu takdirde Continue özelliği kullanılırsa, bu hatalar görmezden gelinerek işleme devam edilir.

Values
Bir hata oluştuğunda bu hata ile ilgili alanların değerlerini belirtir.

DataTable
Hatanın oluştuğu DataTable nesnesine işaret eder.

Errors
Meydana gelen hatayı exception türünden belirtir.

Tablo 3. FillErrorEventArgs Sınıfının Özellikleri

FillError olayının devreye girmesine örnek olarak, veri kaynağındaki veri tiplerinin,.net framework'tekiler ile aynı olmaması durumunu gösterebiliriz.

Böylece geldik bir makalemizin daha sonuna. İlerleyen makalelerimizde Ado.net'in temel kavramlarını incelemeye devam edeceğiz. Hepinize mutlu günler dilerim.