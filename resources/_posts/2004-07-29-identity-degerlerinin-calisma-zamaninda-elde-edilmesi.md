---
layout: post
title: "Identity Değerlerinin Çalışma Zamanında Elde Edilmesi"
date: 2004-07-29 12:00:00 +0300
description: "Bu makalemizde, veritabanlarında otomatik olarak artan identity alanlarının değerlerinin, çalışma zamanında uygulama ortamlarına nasıl yansıtılabileceğini incelemeye çalışacağız. Çoğunlukla, tablolarımızda yer alan satırların birbirlerinden kolayca ayırt edilebilmelerini sağlamak için, primary key alanlarını kullanırız."
categories:
  - ado-net
tags:
  - ado-net
  - csharp
  - dotnet
  - visual-studio
  - datatable
---
# Identity Değerlerinin Çalışma Zamanında Elde Edilmesi
Değerli Okurlarım, Merhabalar.

Bu makalemizde, veritabanlarında otomatik olarak artan identity alanlarının değerlerinin, çalışma zamanında uygulama ortamlarına nasıl yansıtılabileceğini incelemeye çalışacağız. Çoğunlukla, tablolarımızda yer alan satırların birbirlerinden kolayca ayırt edilebilmelerini sağlamak için, primary key alanlarını kullanırız. Genellikle bu alanları otomatik olarak artan sayısal değerler üzerinde yapılandırırız. Örnek olarak aşağıdaki tabloyu göz önüne aldığımızda, PersonelID alanının 1 sayısal değerinden başlayarak 1'er artan ve primary key özelliğine sahip olduğunu görürüz.

![mk80_1.gif](/assets/images/2004/mk80_1.gif)

Şekil 1. PersonelID alanının özellikleri

Bu alanın değeri, tablonun bulunduğu sql sunucusu tarafından otomatik olarak arttırılmaktadır. Buraya kadar her şey zaten bildiğimiz olaylardır. Sorun, bir uygulamada bu tabloya yeni bir alan eklendiğinde, o an veritabanı tarafından otomatik olarak arttırılan alanın değerinin ortama yansıtılmasında ortaya çıkmaktadır. Nitekim bu alanın yeni oluşan değerini elde etmek için, ilgili satırın uygulama ortamına yeniden çekilmesi gerekmektedir. Bu ise özellikle tabloya ait tüm satırların çekildiği durumlarda gereksiz zaman kaybının yaşanmasına neden olmaktadır. Oysaki uygulayacağımız basit kodlamalar ile, yeni eklenen satırlara ait otomatik artan değerleri uygulama ortamına, minimum eforu sarfederek kolaylıkla alabiliriz. Sorunu daha iyi anlayabilmek amacıyla basit bir windows uygulaması geliştirelim. Form görüntümüz aşağıdakine benzer olmalıdır.

![mk80_2.gif](/assets/images/2004/mk80_2.gif)

Şekil 2. Uygulama Formumuz.

Uygulamamız basit olarak, Sql sunucusunda yer alan Northwind veritabanındaki Personel tablosuna ait verileri çekmekte ve yeni satırlar eklenmesine izin vermektedir. Uygulamamıza ait ilk kodlar ise aşağıdaki gibidir.

```csharp
SqlConnection con;
SqlDataAdapter da;
DataTable dt;

private void btnVeriCek_Click(object sender, System.EventArgs e)
{
    con=new SqlConnection("data source=localhost;initial catalog=Northwind;integrated security=SSPI");
    da=new SqlDataAdapter("SELECT * FROM PERSONEL",con); 
    dt=new DataTable("Personel");
    da.Fill(dt);
    dataGrid1.DataSource=dt;
}

private void btnEkle_Click(object sender, System.EventArgs e)
{ 
    DataRow dr;
    dr=dt.NewRow();
    dr[1]=txtPersonelAd.Text;
    dr[2]=txtPersonelSoyad.Text;
    dr[3]=txtSaatUcreti.Text;
    dr[4]=txtCalismaSuresi.Text;
    dt.Rows.Add(dr);

    SqlCommandBuilder cmb=new SqlCommandBuilder(da);
    da.Update(dt);
}
```

Şimdi uygulamamızı çalıştıralım, Veri Çek başlıklı butona basalım ve dataGrid kontrolümüzün Personel verileri ile dolduğunu görelim. Yeni bir personel kaydı girdiğimizde ve bu bilgileri Ekle butonuna basarak veritabanına gönderdiğimizde ekran görüntümüzün aşağıdaki gibi oluştuğunu farkederiz.

![mk80_3.gif](/assets/images/2004/mk80_3.gif)

Şekil 3. Yeni satır eklenişi ve Identity alanının değeri.

Dikkat edilecek olursa, yeni satırımız bağlantısız katman nesnemiz olan DataTable'ın Rows koleksiyonuna ve ayrıca sql sunucusu üzerindeki Personel tablosuna başarılı bir şekilde eklenmiştir. Sorun şudurki, Veri Çek butonuna basmadığımız ve Personel tablosuna ait verileri DataTable nesnesine tekrardan doldurmadığımız sürece, PersonelID alanının veritabanı tarafından otomatik olarak atanan değerini göremeyiz. Oysaki böyle bir değer, gerçek bazlı bir projede başka bir işlem için veri olarak kullanılmak istenebilir.

Örneğin, bir call center'daki operatör, müşterisi için yeni açtığı bir dosyaya ait Identity değerini, dosya referans numarası olarak vermek durumunda olabilir. Kaldıki, eklediğimiz satır için geçerli olan Identity değeri otomatik olarak oluşturulmuştur. Ancak bağlantısız katman nesnesinin henüz bundan haberi yoktur. Bu noktada programı sonlandırmadan Vs.Net ortamında, Personel tablosu bilgilerine bakıldığında, sql sorgusunu yeniden execute edersek Identity değerinin oluşturulmuş olduğunu gözlemleriz.

![mk80_4.gif](/assets/images/2004/mk80_4.gif)

Şekil 4. Identity değeri.

İşte makalemize konu olan sorun bu alanın değerinin, satır veritabanına eklendiği zaman nasıl uygulama ortamına alınacağıdır. İki alternatif çözüm yolumuz vardır. Bunlardan birincisi, SqlDataAdapter sınıfının RowUpdated olayının kullanılmasıdır. Diğer alternatif yol ise, yeni identity değerini uygulama ortamına çekebileceğimiz bir Stored Procedure yardımıyla, satır ekleme işleminin yapılmasıdır. Hangi yolu seçersek seçelim ikiside ortak bir anahtar sözcüğü kullanmaktadır. Bu anahtar sözcük sql'e ait olan @@IDENTITY 'dir. Bu anahtar söcük, eklenen satıra ait, veritabanı tarafından otomatik olarak üretilen identity değerini temsil etmektedir. İlk olarak RowUpdated olayı ile bu işin nasıl sağlanacağına bakalım. Tek yapmamız gereken SqlDataAdapter nesnemize, RowUpdated olayını eklemek ve bu olay metodunu kodlamaktır.

```csharp
private void btnVeriCek_Click(object sender, System.EventArgs e)
{
    .
    .
    .
    da.RowUpdated+=new SqlRowUpdatedEventHandler(da_RowUpdated);
}

private void btnEkle_Click(object sender, System.EventArgs e)
{ 
    .
    .
    .
}

private void da_RowUpdated(object sender, SqlRowUpdatedEventArgs e)
{
    SqlCommand cmd=new SqlCommand("SELECT @@IDENTITY",con);
    if((e.Status==UpdateStatus.Continue) && (e.StatementType==StatementType.Insert))
    {
        e.Row["PersonelID"]=cmd.ExecuteScalar();
        e.Row.AcceptChanges();
    }
}
```

Burada RowUpdated metodunda, veritabanına yeni bir satır eklendiğinde, @@IDENTITY değerini alacak sorguyu çalıştıran bir SqlCommand nesnesi kullanılmaktadır. Böylece, bağlantısız katmana ait yeni eklenen satırın ilgili alanına (yani PersonelID alanına), sorgunun sonucunu alabiliriz. Uygulamamızı bu haliyle çalıştırdığımızda ve yeni bir satır eklediğimizde, PersonelID alanı için veritabanı tarafından üretilen otomatik değerinde, uygulama ortamına alındığını gözlemleriz.

![mk80_5.gif](/assets/images/2004/mk80_5.gif)

Şekil 5. Identity değerinin elde edilmesi.

Gelelim ikinci yola. İkinci yolumuz ise, ekleme işleminin bir Stored Procedure yardımıyla yapılmasıdır. Bu Stored Procedure, ekleme işlemini yaparken, @@IDENTITY değerinide ortama bir Output parametresi vasıtasıyla gönderecektir. Bu amaçla Personel tablomuz için aşağıdaki Stored Procedure'ü oluşturalım.

```text
ALTER PROCEDURE dbo.PersonelEkle
(
    @PersonelID int OUTPUT,
    @PersonelAd varchar(50),
    @PersonelSoyad varchar(50),
    @SaatUcreti decimal,
    @CalismaSuresi decimal
)
AS
INSERT INTO Personel (PersonelAd,PersonelSoyad,SaatUcreti,CalismaSuresi) VALUES (@PersonelAd,@PersonelSoyad,@SaatUcreti,@CalismaSuresi)

SELECT @PersonelID=@@IDENTITY

RETURN
```

Bu Stored Procedure'de en önemli nokta Select sorgusunda @@IDENTITY değerinin, bir Output parametresine aktarılmış olmasıdır. Bu sayede, SP'yi çalıştırdığımızda, veri tabanı tarafından oluşturulan otomatik değeri, uygulama ortamımızda elde edebiliriz. Tek yapmamız gereken aşağıdaki kodları yazmaktır.

```csharp
private void btnSPileEkle_Click(object sender, System.EventArgs e)
{
    SqlCommand cmd=new SqlCommand("dbo.PersonelEkle",con);
    cmd.CommandType=CommandType.StoredProcedure;

    cmd.Parameters.Add("@PersonelID",SqlDbType.Int);
    cmd.Parameters.Add("@PersonelAd",SqlDbType.VarChar,50); 
    cmd.Parameters.Add("@PersonelSoyad",SqlDbType.VarChar,50);
    cmd.Parameters.Add("@SaatUcreti",SqlDbType.Decimal); 
    cmd.Parameters.Add("@CalismaSuresi",SqlDbType.Decimal);

    cmd.Parameters["@PersonelID"].Direction=ParameterDirection.Output;

    cmd.Parameters["@PersonelAd"].Value=txtPersonelAd.Text.ToString();
    cmd.Parameters["@PersonelSoyad"].Value=txtPersonelSoyad.Text.ToString();
    cmd.Parameters["@SaatUcreti"].Value=Convert.ToDecimal(txtSaatUcreti.Text);
    cmd.Parameters["@CalismaSuresi"].Value=Convert.ToDecimal(txtCalismaSuresi.Text);

    con.Open();
    cmd.ExecuteNonQuery();

    /* Yeni satırı DataTable' ımızada ekliyoruz.*/
    DataRow dr;
    dr=dt.NewRow();
    dr[0]=cmd.Parameters["@PersonelID"].Value; /* Veritabanında henüz oluşturulan otomatik değeri alıp, satırın PersonelID alanına aktarıyoruz.*/
    dr[1]=txtPersonelAd.Text;
    dr[2]=txtPersonelSoyad.Text;
    dr[3]=txtSaatUcreti.Text;
    dr[4]=txtCalismaSuresi.Text;
    dt.Rows.Add(dr);
    dt.AcceptChanges();
}
```

Uygulamamızı bu haliyle çalıştırdığımızda yeni ekelenen satır için, veritabanı tarafından otomatik olarak arttırılan identity değerinin kolayca elde edilebildiğini görürüz.

![mk80_6.gif](/assets/images/2004/mk80_6.gif)

Şekil 6. SP ile Identity değerinin elde edilmesi.

Gelelim bu iki seçenekten hangisinin tercih edileceğine. Microsoft'un bu konuda yaptığı testlere göre, Stored Procedure yardımıyla Identity değerlerinin elde edilmesi, RowUpdated olayının kullanıldığı tekniğe nazaran daha performanslı ve verimli. Dolayısıyla SP kullanımını tercih etmek daha doğru bir seçenek gibi gözükmektedir. Bir başka konu ise, Access tipi tablolar için aynı senaryonun nasıl işleyeceğidir. Access tipi tablolarda, Output parametresi desteklenmediğinden, SP kullanımı ile identity değerinin elde edilmesi gerçekleşmeyecektir. Bu nedenle tek seçenek, RowUpdated olayında @@IDENTITY değerinin elde edilmesidir. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle hepinizde mutlu günler dilerim.

[Örnek uygulama için tıklayın](/assets/files/2004/Identity.zip)