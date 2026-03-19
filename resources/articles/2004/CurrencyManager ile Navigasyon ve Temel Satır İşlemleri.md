---
title: "CurrencyManager ile Navigasyon ve Temel Satır İşlemleri"
pubDate: 2004-07-27 06:00:00
categories:
  - Ado.Net
tags:
  - Ado.Net
  - C#
  - DataTable
---

# CurrencyManager ile Navigasyon ve Temel Satır İşlemleri
Değerli Okurlarım, Merhabalar.

Bu makalemizde, CurrencyManager sınıfı yardımıyla, bağlantısız katman nesnelerinin işaret ettiği bellek bölgelerindeki veri satırları arasında navigasyon, satır ekleme, satır silme ve satır güncelleme işlemlerinin nasıl gerçekleştirildiğini incelemeye çalışacağız.

Genellikle bir veritabanı uygulamasını göz önüne aldığımızda, var olan satırlar arasında ileri ve geri yönlü hareketler sıklıkla kullanılan işlemler arasında yer almaktadır. Bir windows uygulamasında, bağlantısız katman nesnelerinin sahip olduğu veri kümelerini, form üzerinde yer alan çeşitli kontroller yardımıyla kullanıcıya sunarız. Ancak zaman zaman, bu veri kümesi içinde hareket ederken, veriye bağlı kontrollerinde güncel satıra ait alan bilgilerini göstermesini isteriz. Form üzerine yerleştirilen kontrollerin her biri, bu çeşit navigasyon işlemlerine izin veren CurrencyManager isimli nesne örneklerine sahiptir. Dolayısıyla navigasyon işlemlerinin bağlı kontrollere yansıması için, bağlı kontrollere ait CurrencyManager nesnelerinin ilgili özelliklerinin kullanılması yeterlidir.

Bir windows uygulamasını düşündüğümüzde, Form üzerinde yer alan bağlı kontrollerinin tümünün aynı navigasyon hareketlerine izin vermesini istememiz son derece doğaldır. Bu zaten arzu edilen durumdur. Windows Form'larının BindingContext özelliği sayesinde, bağlı kontrollerin sahip olduğu CurrencyManager nesne örneklerini yönetebiliriz. Bu yönetim imkanı, satırlar arasında navigasyon, yeni kayıt ekleme, silme veya güncelleştirme gibi işlemleri yapabilme imkanına sahip olmamızı sağlamaktadır. Konuyu çok yüzeysel olarak anlattığımız bu kısa açıklamalardan sonra, olayı daha iyi ve net bir şekilde simüle edebilmek amacıyla aşağıdaki Form görüntüsüne sahip Windows Uygulamasını tasarlayarak işlemlerimize başlayalım.

![mk79_1.gif](images/mk79_1.gif)

Şekil 1. Form tasarımımız.

Bu windows uygulamasında, Sql sunucusunda yer alan Northwind veritabanında yer alan Personel tablosuna ait veriler gösterilecektir. Başlangıç olarak, uygulama kodlarımız aşağıdaki gibidir.

```csharp
SqlConnection con;
SqlDataAdapter da;
DataTable dt;

private void Bagla()
{
    lblPersonelID.DataBindings.Add("Text",dt,"PersonelID");
    txtPersonelAd.DataBindings.Add("Text",dt,"PersonelAd");
    txtPersonelSoyad.DataBindings.Add("Text",dt,"PersonelSoyad");
    txtSaatUcreti.DataBindings.Add("Text",dt,"SaatUcreti");
    txtCalismaSuresi.DataBindings.Add("Text",dt,"CalismaSuresi");
}

private void Form1_Load(object sender, System.EventArgs e)
{
    con=new SqlConnection("data source=localhost;initial catalog=Northwind;integrated security=SSPI");
    da=new SqlDataAdapter("SELECT * FROM PERSONEL",con);
    dt=new DataTable("Personel");
    da.Fill(dt);
    Bagla();
}
```

Uygulama kodlarımıza kısaca baktığımızda, Sql sunucusuna bağlanarak Northwind veritabanındaki Personel tablosuna ait tüm satırları, bir DataTable'a aktardığımızı ve Form üzerindeki kontrolleride bu veri kaynağına bağladığımızı görürüz. Kontrollerimizi, tablonun ilgili alanlarına bağlarken DataBindings özelliğinin Add metodundan yararlanmaktayız.

Add metodu ilk parametre olarak, veri kaynağındaki ilgili verinin, kontrolün hangi özelliğine bağlanacağını belirtir. İkinci parametrede bu ilk parametredeki özellik için hangi veri kaynağının kullanılacağı belirtilir. Son olarak üçüncü parametrede ise, ilk parametrede belirtilen kontrol özelliğine, ikinci parametredeki veri kaynağının hangi alanının bağlanacağı belirlenir. Böylece, DataTable nesnemizin bellekte işaret ettiği bölgedeki satırlara ait alanlar, Form üzerindeki kontrollere bağlanmış olacaktır. Uygulamamızı bu haliyle derleyip çalıştırdığımızda, veri kümesindeki ilk satıra ait bilgilerin Form üzerindeki kontrollere eklendiğini görürüz.

![mk79_2.gif](images/mk79_2.gif)

Şekil 2. Uygulamanın Çalışması.

Şimdi ilk yapmak istediğimiz, veri kümesindeki satırlar arasında hareket edebilme imakanına sahip olmaktır. Şu aşamda 4 temel hareketimiz olabilir. Sonraki satıra geçiş, önceki satıra geçiş, son satıra geçiş ve ilk satıra geçiş. Bu işlemleri gerçekleştirebilmek için, Form'umuza ait BindingContext özelliğini kullanacağız. Şimdi formumuza 4 adet button kontrolü ekleyelim ve uygulama kodlarımızı aşağıdaki şekilde geliştirelim.

```csharp
SqlConnection con;
SqlDataAdapter da;
DataTable dt;
CurrencyManager cm;

private void Bagla()
{
    lblPersonelID.DataBindings.Add("Text",dt,"PersonelID");
    txtPersonelAd.DataBindings.Add("Text",dt,"PersonelAd");
    txtPersonelSoyad.DataBindings.Add("Text",dt,"PersonelSoyad");
    txtSaatUcreti.DataBindings.Add("Text",dt,"SaatUcreti");
    txtCalismaSuresi.DataBindings.Add("Text",dt,"CalismaSuresi");
}

private void Form1_Load(object sender, System.EventArgs e)
{
    con=new SqlConnection("data source=localhost;initial catalog=Northwind;integrated security=SSPI");
    da=new SqlDataAdapter("SELECT * FROM PERSONEL",con);
    dt=new DataTable("Personel");
    da.Fill(dt);
    Bagla();
    cm=(CurrencyManager)this.BindingContext[dt];
    lblPozisyon.Text="Guncel Pozisyon ------> "+Convert.ToString(cm.Position+1);
}

private void btnIlkSatiraGotur_Click(object sender, System.EventArgs e)
{
    cm.Position=0;
    lblPozisyon.Text="Guncel Pozisyon ------> "+Convert.ToString(cm.Position+1);
}

private void btnOncekiSatiraGotur_Click(object sender, System.EventArgs e)
{
    cm.Position--;
    lblPozisyon.Text="Guncel Pozisyon ------> "+Convert.ToString(cm.Position+1);
}

private void btnSonrakiSatiraGotur_Click(object sender, System.EventArgs e)
{
    cm.Position++;
    lblPozisyon.Text="Guncel Pozisyon ------> "+Convert.ToString(cm.Position+1);
}

private void btnSonSatiraGotur_Click(object sender, System.EventArgs e)
{
    cm.Position=dt.Rows.Count-1;
    lblPozisyon.Text="Guncel Pozisyon ------> "+Convert.ToString(cm.Position+1);
}
```

Uygulama kodlarımızda en önemli nokta, Form üzerindeki kontrollerin sahip oldukları CurrencyManager nesnelerini elde edebilmek için, Form'a ait BindingContext özelliğinden yararlanmamızdır.

```csharp
cm=(CurrencyManager)this.BindingContext[dt];
```

Bu satır ile, DataTable veri kaynağına için bir CurrencyManager nesnesi oluşturulur. Artık navigasyon işlemi için tek yapmamız gereken, CurrencyManager sınıfına ait Position özelliğinin kullanılmasıdır. Bu özelliğin değişmesi durumunda, Form üzerinde yer alan ve DataTable'a ait veri kaynağına bağlı olan tüm kontrollerin içeriği güncel satıra ait verileri gösterecek şekilde değişecektir. Örneğin, bir satır ileri gitmek için Position özelliğinin değerini 1 arttırmamız yeterli olurken, son satıra gitmek için, veri kaynağının sahip olduğu toplam satır sayısını 1 eksiltiriz.

```csharp
cm.Position++;
.
.
.
cm.Position=dt.Rows.Count-1;
```

Uygulamamızı çalıştırdığımızda, satırlar arasında istediğimiz şekilde gezebildiğimizi görürüz.

![mk79_3.gif](images/mk79_3.gif)

Şekil 3. Navigasyon.

CurrencyManager sınıfı yardımıyla yapabileceklerimiz sadece Navigasyon işlemleri ile sınırlı değildir. Ayrıca, satır ekleme, satır silme veya satır güncelleştirme gibi temel tablo işlemlerinide gerçekleştirebiliriz. Yeni bir satır eklemek için CurrencyManager sınıfına ait, aşağıda prototipi verilen AddNew metodunu kullanırız.

```csharp
public override void AddNew();
```

Uygulamamızda yeni bir satır eklemek için öncelikle aşağıdaki kodları yazalım.

```csharp
private void Temizle()
{
    lblPersonelID.Text="";

    for(int i=0;i<this.Controls.Count;i++)
    {
        if(this.Controls[i] is TextBox)
        {
            this.Controls[i].Text="";
        }
    }
}

private void btnYeniSatirEkler_Click(object sender, System.EventArgs e)
{
    cm.AddNew();
    Temizle();
}

private void btnVeritabaninaYaz_Click(object sender, System.EventArgs e)
{
    SqlCommandBuilder cmb=new SqlCommandBuilder(da);
    da.Update(dt);
}
```

Burada, önemli olan AddNew metodunun kullanılışından sonra, açılan yeni satırın o anki veri kümesinde yerini alması için ileri veya geri yönlü bir hareket yapılmasının yeterli olduğudur. Elbette, CurrencyManager yardımıyla, güncel veri kümesine eklenen satırların veritabanınada yansıtılabilmesi için, SqlDataAdapter nesnemize ait Update metodunun çalıştırılması gerekmektedir. Güncel satırlar üzerinde değişiklik yapıldığında bu değişikliklerin onaylanabilmesi ve veri kümesine yansıyabilmesi için ya satırlar arasında hareket edilmesi yada aşağıda prototipi verilen EndCurrentEdit metodunun uygulanması gerekir.

```csharp
public override void EndCurrentEdit();
```

Veri kümesinden herhangibir satırı silmek istediğimizde ise, aşağıdaki prototipe sahip olan RemoveAt metodunu kullanabiliriz.

```csharp
public override void RemoveAt( int index);
```

Bu metod parametre olarak, silinecek satırın indeksini almalıdır. Satır güncelleme ve silme işlemleri için ilgili kodlarımız ise aşağıdaki gibidir.

```csharp
private void btnDegisiklikleriKaydet_Click(object sender, System.EventArgs e)
{
    if(MessageBox.Show("Değişiklikler kaydedilsin mi?", "Değişiklik" , MessageBoxButtons.OKCancel, MessageBoxIcon.Question )==DialogResult.OK)
{
    cm.EndCurrentEdit();
    }
    else
    {
        cm.CancelCurrentEdit();
    }
}

private void btnGuncelSatiriSil_Click(object sender, System.EventArgs e)
{
    cm.RemoveAt(cm.Position);
}
```

Elbette yaptığımız satır ekleme, güncelleme işlemlerini iptal etmek istersek, prototipi aşağıdaki gibi olan CancelCurrentEdit metodunu kullanabiliriz. Bu metod ile o anda yapılan yeni satır ekleme veya güncelleme işleminin oluşturduğu değişikliği geri alabiliriz. Tabiki satırdaki değişiklik EndCurrentEdit metodu ile yada navigasyon hareketi ile onaylanmadıysa. Bu durum sadece satır silme işlemi için geçerli değildir. Dolayısıyla satır silindiğinde, CancelCurrentEdit bu işlemi geri almaz.

```csharp
public override void CancelCurrentEdit();
```

CurrencyManager sınıfının işimize yarayacak bir kaç olayıda vardır. Bunlardan birisi PositionChanged olayıdır. Bu olay güncel satır pozisyonu değiştiğinde çalışmaktadır. Uygulamamızdaki pozisyon bilgisini lblPozisyon kontrolünde göstermek için kullandığımız kod satırını bu olay içine yerleştirebiliriz. Elbette öncelikle, CurrencyManager nesnemiz için aşağıdaki kod satırı ile, PositionChanged olayını eklememiz gerekmektedir.

```csharp
cm.PositionChanged+=new EventHandler(cm_PositionChanged);
.
.
.
private void cm_PositionChanged(object sender, EventArgs e)
{
    lblPozisyon.Text="Guncel Pozisyon ------> "+Convert.ToString(cm.Position+1);
}
```

Bu makalemiz ile, CurrencyManager sınıfının temel metodlarını ve özelliklerini inceleyerek, veri kümeleri üzerinde navigasyon, ekleme, güncelleme ve silme işlemlerinin nasıl yapıldığını öğrenmeye çalıştık. Umuyorum ki faydalı bilgiler verebilmişimdir. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek uygulama için tıklayın](assets/Currency.zip)
