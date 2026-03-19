---
title: "Transaction' larda Izolasyon Seviyeleri -2 (IsolationLevel Numaralandırıcısı)"
pubDate: 2004-06-28 12:00:00
categories:
  - Ado.Net
tags:
  - Ado.Net
  - C#
  - .NET
  - SQL Server
  - Concurrency
  - Transactions
  - DataTable
---

# Transaction' larda Izolasyon Seviyeleri -2 (IsolationLevel Numaralandırıcısı)
Değerli Okurlarım, Merhabalar.

Bu makalemizde, Sql izolasyon seviyelerinin,.net uygulamalarında nasıl kullanıldığını incelemeye çalışacağız. Bir önceki makalemizde, izolasyon seviyeleri için söz konusu olabilecek 3 problemi ele almıştık. Bu olası problemler phantoms, non-repeatable read ve dirty read durumlarıdır. Eş zamanlı olarak çalışan Transaction'larda meydana gelebilecek bu problemleri, IsolationLevel numaralandırıcısı yardımıyla kontrol altına alabiliriz. Bu numaralandırıcının alabileceği değerler ve bu değerlerin izin verdiği (vermediği) durumlar aşağıdaki tabloda yer almaktadır.

IsolationLevel Numaralandırıcı Değeri
Olası Problemler

Phantoms
Non-Repeatable Read

Dirty-Read

Chaos
SqlServer tarafından desteklenmez.

ReadCommitted

![mk75_1.gif](images/mk75_1.gif)

![mk75_1.gif](images/mk75_1.gif)

![mk75_2.gif](images/mk75_2.gif)

ReadUncommitted

![mk75_1.gif](images/mk75_1.gif)

![mk75_1.gif](images/mk75_1.gif)

![mk75_1.gif](images/mk75_1.gif)

RepeatableRead

![mk75_1.gif](images/mk75_1.gif)

![mk75_2.gif](images/mk75_2.gif)

![mk75_2.gif](images/mk75_2.gif)

Serializable

![mk75_2.gif](images/mk75_2.gif)

![mk75_2.gif](images/mk75_2.gif)

![mk75_2.gif](images/mk75_2.gif)

Unspecified
SqlServer tarafından desteklenmez.

Görüldüğü gibi IsolationLevel numaralandırıcısının alabileceği değerler altı adettir. Chaos ve Unspecified numaralandırıcı değerleri sql server tarafından desteklenmemektedir. Bununla birlikte, bir Transaction sınıfının IsolationLevel özelliğinin varsayılan değeri ReadCommitted olarak belirtilmiştir. Bu tablonun okunuşuna gelince. Örneğin, RepeatableRead değerini ele alalım. Bir uygulamada, Transaction nesnesine izolasyon seviyesi olarak bu değer atandığı takdirde, eş zamanlı olarak çalışan Transaction'lar arasında sadece Phantoms durumuna izin verilir. Dolayısıyla, Non-repeatable Read ve Dirty-Read durumlarına izin verilmez. Burada tüm olası problemlere izin veren IsolationLevel numaralandırıcı değeri ReadUncommitted değeridir. Aksine, Serializable değeri Transaction'lar arasında doğabilecek bu problemlerin hiç birisinin olmasına izin vermez.

Şimdi dilerseniz, bu değerlerin aynı zamanda çalışan Transaction'lar üzerindeki etkilerini örnek uygulamamız üzerinde incelemeye çalışalım. Bu makalemizde yine bir önceki makalemizde geliştirdiğimiz uygulamayı örnek olarak kullanabiliriz. Bu kez formumuza izolasyon seviyelerini çalışma zamanında belirlememize yarayacak 4 adet RadioButton kontrolü yerleştirdik. Formumuzun görüntüsü aşağıdaki gibi olacaktır.

![mk75_3.gif](images/mk75_3.gif)

Elbette, gerçek uygulamalarda burada olduğu gibi izolasyon seviyelerinin çalışma zamanında bu şekilde belirlenmesi pek doğru olmayabilir. Ancak şu an için amacımız, bu izolasyon seviyelerinde aynı anda çalışan Transaction'larda nelerin olup nelerin olmadığını kontrol edebilmektir. Uygulama kodlarımız ise, aşağıda olduğu gibidir.

```csharp
SqlConnection con;
SqlTransaction trans;
SqlDataAdapter da;

private void btnBegin_Click(object sender, System.EventArgs e)
{
    if(con.State==ConnectionState.Closed)
    {
        con.Open();
    } 
    if(this.rdbReadCommited.Checked==true)
    {
        trans=con.BeginTransaction(IsolationLevel.ReadCommitted);
    }
    else if(this.rdbReadUncommited.Checked==true)
    {
        trans=con.BeginTransaction(IsolationLevel.ReadUncommitted);
    }
    else if(this.rdbRepeatableRead.Checked==true)
    {
        trans=con.BeginTransaction(IsolationLevel.RepeatableRead);
    }
    else if(this.rdbSerializable.Checked==true)
    {    
        trans=con.BeginTransaction(IsolationLevel.Serializable);
    }

    lblDurum.Text=trans.IsolationLevel.ToString();
}

private void Form1_Load(object sender, System.EventArgs e)
{
    con=new SqlConnection("data source=localhost;database=Northwind;integrated security=SSPI"); 
}

private void btnCommit_Click(object sender, System.EventArgs e)
{
    trans.Commit();
}

private void btnRollBack_Click(object sender, System.EventArgs e)
{
    trans.Rollback();
}

private void btnBak_Click(object sender, System.EventArgs e)
{
    try
    {
        SqlCommand cmdBak=new SqlCommand("SELECT * FROM Personel",con);
        cmdBak.Transaction=trans;
        da=new SqlDataAdapter(cmdBak);
        DataTable dt=new DataTable();
        da.Fill(dt);
        dataGrid1.DataSource=dt;
    }
    catch(SqlException hata)
    {
        MessageBox.Show(hata.Message.ToString());
    }    
}

private void btnEkle_Click(object sender, System.EventArgs e)
{
    SqlCommand cmdGir=new SqlCommand("INSERT INTO Personel (ISIM,SOYISIM) VALUES ('"+txtIsim.Text+"','"+txtSoyisim.Text+"')",con);
    cmdGir.Transaction=trans;
    int sonuc=cmdGir.ExecuteNonQuery();
    MessageBox.Show(sonuc+" SATIR GIRILDI");
}

private void btnGuncelle_Click(object sender, System.EventArgs e)
{
    SqlCommand cmdGuncelle=new SqlCommand("UPDATE Personel SET     ISIM='"+txtIsim.Text+"', SOYISIM='"+txtSoyisim.Text+"' WHERE ID="+txtID.Text,con);
    cmdGuncelle.Transaction=trans;
    int sonuc=cmdGuncelle.ExecuteNonQuery();
    MessageBox.Show(sonuc+" SATIR GUNCELLENDI");
}

private void btnBakID_Click(object sender, System.EventArgs e)
{
    try
    {
        SqlCommand cmd=new SqlCommand("SELECT ISIM,SOYISIM FROM Personel WHERE ID="+txtID.Text,con);
        cmd.Transaction=trans;
        da=new SqlDataAdapter(cmd);
        DataTable dt=new DataTable();
        da.Fill(dt);
        dataGrid1.DataSource=dt;
    }
    catch(SqlException hata)
    {
        MessageBox.Show(hata.Message.ToString());
    }
}
```

Kodumuzdaki en önemli nokta, başlatılacak Transaction'lar için IsolationLevel değerlerinin belirlenmesidir. Bu amaçla, SqlConnection sınıfının, BeginTransaction metodunun aşağıdaki prototipi kullanılmaktadır. Bu prototipte BeginTransaction metodu, parametre olarak IsolationLevel numaralandırıcısı türünden bir değer alır. Böylece belirtilen bağlantı için açılacak Transaction, bu parametrede belirtilen izolasyon seviyesini kullanarak çalışacaktır.

```csharp
public SqlTransaction BeginTransaction(IsolationLevel iso);
```

Şimdi ilk olarak IsolationLevel özelliğinin varsayılan değeri olan ReadCommitted değerinden işe başlayalım. ReadCommitted değeri, Phantoms ve Non-Repeatable Read durumlarına izin verirken, Dirty-Read durumuna izin vermez. Şimdi örnek uygumamamızdan iki adet çalıştıralım ve aşağıdaki tabloda yer alan hareketleri sırasıyla uygulayalım. Elbette tüm örneklerimizde ilk olarak Başlat başlıklı butona basarak Transaction'ların başlamasını sağlamalıyız.

IsolationLevel.ReadCommitted

Phantoms
Transaction 1
Transaction 2

Veri Çeker. (Bak başlıklı button)

Veri Çeker. (Bak başlıklı button)

Yeni satır ekler. (Ekle başlıklı button)

Transaction onaylanır. Commit (Onayla başlıklı button)

Tekrardan Veri Çeker. Phantoms durumu oluşur.

![mk75_4.gif](images/mk75_4.gif)

Şimdide, Non-Repeatable Read durumunu inceleyelim. Bunun içinde, yine aynı uygulamadan iki tane başlatabilir yada halen çalışan uygulamalarda açık kalan Transaction'ları onaylayarak yeni baştan oluşturabilirsiniz. Bu kez aşağıdaki tabloda izleyen adımları gerçekleştireceğiz.

IsolationLevel.ReadCommitted

Non-Repeatable Read
Transaction 1
Transaction 2

Belirli bir satır veri çekilir. (Örneğin ID=58)

Aynı satır çekilir. (ID=58)

Bu satırdaki verilerde değişiklikler yapılır.

Transaction onaylanır. Commit.

Aynı satıra tekrar bakılır. Non-Repeatable Read durumu oluşur.

![mk75_5.gif](images/mk75_5.gif)

ReadCommitted seviyesi ile ilgili olarak son durum ise Dirty Read durumudur. Bu izolasyon seviyesi, Dirty Read durumlarının oluşmasına izin vermez. Hatırlayacağınız gibi bu durumda, eş zamanlı olarak çalışan Transaction'larda herhangibir Commit işlemi olmadan söz konusu olan problemler yer almaktadır. Şimdi bu durumu incelemek için aşağıdaki tabloda yer alan işlemleri gerçekleştirelim.

IsolationLevel.ReadCommitted

Dirty Read
Transaction 1
Transaction 2

Belirli bir satır veri çekilir. (Örneğin ID=73)

Aynı satır çekilir. (ID=73)

Bu satırdaki verilerde değişiklikler yapılır. Ancak Transaction Commit edilmez.

Aynı satıra tekrar bakılmak istenir. Hata Oluşur.

Görüldüğü gibi, aşağıdaki hata bildirisi çalışma zamanında oluşur.

![mk75_6.gif](images/mk75_6.gif)

Dolayısıyla şunu söyleyebiliriz. ReadCommitted değerine sahip Transaction'lardan herhangibiri içerisinde yapılan güncelleme, silme veya ekleme gibi işlemlerin, diğer Transaction tarafından görülebilmesi için, bu değişiklikleri içeren Transaction'ın Commit edilmesi veya RollBack edilmesi gerekmektedir. Bu nedenle, ReadCommitted değeri, Dirty Read durumuna izin vermez.

Gelelim, ReadUncommitted değerine. Bu değerin ReadCommitted değerinden tek farkı Dirty Read durumuna izin vermesidir. Bu durumu analiz etmek için, aşağıdaki tablodaki işlemleri sırasıyla uygulayalım. (Transaction'ları Başlat başlıklı Button ile başlatmadan önce, ReadUncommitted RadioButton kontrolünün seçili olmasına dikkat edelim.)

IsolationLevel.ReadUncommitted

Dirty Read
Transaction 1
Transaction 2

Belirli bir satır veri çekilir. (Örneğin ID=70)

Aynı satır çekilir. (ID=70)

Bu satırdaki verilerde değişiklikler yapılır. Ancak Transaction Commit edilmez.

Aynı satıra tekrar bakılmak istenir. (ID=70) Meydana gelen yeni değişiklikler görülür.

Yapılan işlemler geri alınır. RollBack.

Bu Transaction'da gerçekleşmemiş değişiklikler görünmeye devam eder. Dirty Read durumu.

Dolayısıyla ReadUncommitted değerinin en önemli özelliği, Dirty Read durumuna neden olacak işlemlere izin vermesidir. Başka bir deyişle, çalışan iki Transaction'dan herhangibirinde yapılan değişikliklerin diğer Transaction tarafından görülmesi için, işlemlerin mutlaka Commit edilmesi veya RollBack edilmesi gerekmez.

Sırada, RepeatableRead değeri var. Bu değeride ReadCommitted ile kıyaslayarak anlamaya çalışmak daha mantıklıdır. RepeatableRead değeri, sadece phantoms durumlarına izin verir. Diğer durumların oluşması halinde, yine zaman aşımı nedeniyle bir SqlException istisnası firlatılır. IsolationLevel'ların belirlenmesi ile ilgili önemli olan bir diğer değerde Serializable değeridir. Bu değer, Phantoms, Non-Repeatable Read ve Dirty Read durumlarından herhangibirisinin oluşmasına izin vermez. Örneğin aşağıdaki tabloda yer alan işlemleri yapmaya çalıştığımızı düşünelim.

IsolationLevel.Serializable

Phantoms
Transaction 1
Transaction 2

Tablodan veriler çekilir.

Tablodan veriler çekilir.

Yeni bir satır eklenmeye çalışılır. Ancak buna izin verilmez. Çünkü satırlar, Serializable değeri nedeniyle diğer Transaction tarafından kilitlenmiştir.

Dolayısıyla Serializable değerinin eş zamanlı Transaction'lar için sadece veri bakmaya izin verdiğini söyleyebiliriz. Başka bir deyişle, aynı anda çalışan iki Transaction'dan herhangibiri, Transaction'lardan diğeri açık olduğu sürece veri girişi, düzenlemesi veya silme işlemlerini yapamaz. Böylece Sql Server üzerinde çalışan eş zamanlı Transaction'lar için var olabilecek sorunları değerlendiren izolasyon seviyelerini kısaca görmüş olduk. İlerleyen makalelerimizde, Sql Server kilitlerinin Ado.Net içindeki yerini incelemeye çalışacağız. Hepinize mutlu günler dilerim.
