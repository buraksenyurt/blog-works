---
layout: post
title: "Transaction' larda SavePoint Kullanımı"
date: 2004-06-15 12:00:00 +0300
categories:
  - ado-net
tags:
  - ado.net
  - transaction
  - isolation-levels
  - save-point
---
Bu makalemizde, Ado.Net ile gerçekleştirilen transaction işlemlerinde, sql'de yer alan SavePoint'lerin nasıl uygulandığını incelemeye çalışacağız. Sql'de transaction işlemlerinde, her bir iş parçasından sonra gelinen noktanın birer SavePoint olarak kaydedilmesi sık rastlanan bir tekniktir. Bir transaction birden fazla iş parçasına sahiptir. Her bir iş parçasının başarılı olması halinde, tüm bu işlemler onaylanarak (commit) kesin olarak veritabanına yansıtılır. Diğer yandan, iş parçalarının herhangibirisinde meydana gelebilecek bir aksaklık sonucu transaction RollBack işlemini uygular ve tüm işlemler yapılmamış sayılarak veritabanı, transaction başlamadan hemen önceki haline getirilir.

Ancak çoğu zaman transaction blokları içerisine aldığımız iş parçaları, çok fazla sayıda olup, herhangibir noktada meydana gelebilecek RollBack işlemi sonucu o ana kadar yapılan tüm işlemlerin geçersiz sayılması istenen bir durum olmayabilir. İşte böyle bir durumda, başarılı bir şekilde gerçekleşen işlerden sonraki kod satırlarına dönmek daha mantıklı bir yaklaşımdır. Elbette bu durum havale, eft gibi bankacılık işlerini kapsayan transaction'larda tercih edilmemelidir.

SavePoint'lerin çalışma mantığında, transaction içindeki belirli noktaların işaretlenmesi yatmaktadır. Bu durumda, RollBack işlemi söz konusu olduğunda, bu işaretlenmiş noktalardan birisine dönülebilme imkanına sahip olunur. İşte Sql transaction'larındaki bu tekniği,.net ile geliştirdiğimiz uygulamalarda da simule edebilmek için, Transaction sınıflarının aşağıda SqlTransaction sınıfı için prototipi verilen Save metodu kullanılır.

```csharp
public void Save(string savePointName);
```

Bu metod parametre olarak, SavePoint'in ismini belirten string türde bir değer alır. SavePoint olarak kaydedilmiş bir noktaya RollBack işlemi ile geri dönebilmek için, RollBack metodunun aşağıdaki prototipi kullanılır.

```csharp
public void Rollback(string transactionName);
```

Burada, RollBack metoduna, parametre olarak string türden SavePoint'in adı verilir. Burada önemli olan, RollBack ile herhangibir SavePoint'e dönülmesinden sonra eğer Commit işlemi uygulanırsa, SavePoint'e kadar yapılan iş parçalarının kesin olarak veritabanına yansıtılacağıdır. Şimdi SavePoint kullanımına ilişkin olarak basit bir örnek geliştirelim. Örnek windows uygulmasında, 3 iş parçası içeren bir transaction kullanacağız. Bu transaction işleminde, SavePoint'lere yer verecek ve bu noktaların veritabanına olan etkisini basitçe incelemeye çalışacağız. Bu amaçla aşağıdaki form görünümüne benzer bir windows uygulması tasarlayarak işe başlayalım.

![mk73_1.gif](/assets/images/2004/mk73_1.gif)

Şekil 1. Form Tasarımımız.

Şimdi uygulama kodlarımızı geliştirelim.

```csharp
/* SqlConnection ve SqlTransaction nesnelerimizi tanımlıyoruz.*/
SqlConnection con; 
SqlTransaction trans;

private void frmMain_Load(object sender, System.EventArgs e)
{
    /* Uygulamamız yüklenirken, SqlConnection nesnemizi oluşturuyoruz. Yerel sql sunucusunda yer alan, Northwind veritabanına bağlantı sağlayacağız.*/
    con = new SqlConnection("data source=localhost;initial catalog=Northwind;integrated security=SSPI");
}

private void btnBegin_Click(object sender, System.EventArgs e)
{
    /* Kullanıcı transaction'ı başlatıyor. Önce bağlantımız açılır daha sonra SqlConnection nesnemizin BeginTransaction metod ile, transaction' ımız bu bağlantı için başlatılır. Ardından açılış işlemi listBox kontrolüne yazılır.*/
    con.Open();
    trans=con.BeginTransaction();
    listBox1.Items.Add("Transaction basladi...");
} 

private void btnEkle1_Click(object sender, System.EventArgs e)
{
    /*Burada transaction' ımız içinde çalıştırılacak bir iş parçası uygulanıyor. Bu iş parçasında, Personel isimli tabloya veri girişi yapılmakta.*/
    SqlCommand cmd=new SqlCommand("INSERT INTO Personel (ISIM,SOYISIM) VALUES ('BURAK','SENYURT')",con);
    cmd.Transaction=trans; /* SqlCommand için transaction nesnesi belirtilir.*/
    try
    {
        int sonuc=cmd.ExecuteNonQuery(); /* Komutumuz çalıştırılır.*/
        trans.Save("Insert_1"); /*Transaction' ımız içinde bu noktada bir SavePoint oluşturulur.*/
        listBox1.Items.Add(sonuc+" Kayit eklendi. SavePoint=Insert_1"); /* Durum listBox'a yazılır.*/
    }
    catch
    {
        listBox1.Items.Add("HATA...");
        trans.Rollback(); /* Bir hata oluşursa ilk hale dönülür.*/
    }
}

private void btnEkle2_Click(object sender, System.EventArgs e)
{
    /*Burada transaction' ımız içinde çalıştırılacak bir iş parçası uygulanıyor. Bu iş parçasında, Per isimli tabloya veri girişi yapılmakta. Ancak tablomuzun ismi Personel olmalı ve Per isimli bir tablomuzda yok. Bu nedenle burada bir hata oluşacaktır. İşte bu hata sonucu transaction'ımız RollBack işlemi ile Insert_1 isimli SavePoint noktasına döner.*/
    SqlCommand cmd=new SqlCommand("INSERT INTO Per (ISIM,SOYISIM) VALUES ('SEFER','ALGAN')",con);
    cmd.Transaction=trans;
    try
    {
        int sonuc=cmd.ExecuteNonQuery();
        trans.Save("Insert_2");/* Insert_2 isminde bir SavePoint oluşturulur.*/
        listBox1.Items.Add(sonuc+" Kayit eklendi. SavePoint=Insert_2");
    }
    catch
    {
        listBox1.Items.Add("HATA...DONUS-->Insert_1");
        trans.Rollback("Insert_1"); /* Insert_1 isimli SavePoint noktasına dönülür.*/ 
    }
}

private void btnSil_Click(object sender, System.EventArgs e)
{
    /*Personel tablosundan ID alanının değeri 1 olan satırı silecek komutu içeren bir iş parçası uygulanıyor. Ancak ID=1 olan bir satır yok ise bir istisna oluşacaktır. Bu durumda RollBack işlemi ile, Insert_2 isimli SavePoint noktasına dönülür. */
    SqlCommand cmd=new SqlCommand("DELETE FROM Personel WHERE ID=1",con);
    cmd.Transaction=trans;
    try
    {
        int sonuc=cmd.ExecuteNonQuery();
        listBox1.Items.Add(sonuc+" Kayit silindi.");
    }
    catch
    {
        listBox1.Items.Add("HATA...DONUS-->Insert_2");
        trans.Rollback("Insert_2"); /* Insert_2 SavePoint noktasına dönülür.*/ 
    }
}

private void btnCommit_Click_1(object sender, System.EventArgs e)
{
    /* Transaction onaylanır. Eğer transaction herhangibir SavePoint' te ise, o noktaya kadar olan tüm işlemler onaylanır. */
    trans.Commit();
    listBox1.Items.Add("Islemler Onaylandi...");
    if(con.State==ConnectionState.Open)
    {
        con.Close();
    }
}
```

Burada dikkat edilecek olursa, ikinci Insert işleminde yanlış bir tablo ismi verilmiştir. Ayrıca silme işlemi için kullandığımız sql cümleciğinde, ID alanının değerinin 1 olması garanti değildir. İşte buralarda, istisnalar fırlayacaktır. Ancak catch bloklarında bu istisnalar yakandıktan sonra RollBack işlemi ile daha önceden kaydedilen belirli bir SavePoint'e dönülmektedir. İşte bu noktalardan sonra Commit işlemini uygularsak, bu SavePoint'lere kadar olan tüm iş parçaları onaynalacak ve veritabanına yansıtılacaktır. Bu durumu aşağıdaki şekil ile daha kolay anlayabiliriz.

![mk73_6.gif](/assets/images/2004/mk73_6.gif)

Şekil 2. SavePoint Kullanımı

Şimdi bu durumu analiz etmek için uygulamamızı çalıştıralım.

![mk73_2.gif](/assets/images/2004/mk73_2.gif)

Şekil 3. Ekle 2 işleminde hata.

Önce Ekle 1 başlıklı butona basarak ilk Insert işlemimizi çalıştıralım. Daha sonra Ekle 2 başlıklı butona tıklayarak ikinci insert işlemini çalıştıralım. Burada Per isimli tablo olmadığı için bir istisna oluşacaktır. Bu durumda Catch bloğundaki RollBack metodu ile transaction Insert_1 isimli SavePoint'e döner. Eğer bu anda Onayla başlıklı butona tıklar ve transaction'ı onaylarsak (Commit) tablomuzun görünümünün aşağıdaki gibi olduğunu farkederiz. (Tablonun ilk hali hiç bir kayıt içermemektedir.)

![mk73_3.gif](/assets/images/2004/mk73_3.gif)

Şekil 4. Insert_1 SavePoint'inden Sonra Commit Sonucu Tablonun Durumu.

Görüldüğü gibi, Insert_1 SavePoint'ine kadar olan işlemler (ki burada sadece ilk Insert komutu oluyor) veritabanına yansıtılmıştır. Şimdi kodumuzdaki ikinci insert ifadesinde yer alan Per isimli tablo adını düzeltelim ve bunu Personel yapalım. Bu durumda ikinci Insert işlemimizde geçerli olacaktır. Ancak halen Delete işleminde problem çıkması olasıdır. Nitekim, tablomuzda yeni girdiğimiz ilk satırın ID değeri 51' dir. Identity olarak tanımlanan bu alanın 1 değerine sahip olması şu anki şartlarda imkansızdır. Dolayısıyla Delete işleminde yine bir istisna fırlayacak ve bu kez, Insert_2 isimli SavePoint'e dönülecektir. Şimdi bu son durumu analiz etmek için uygulamızı düzenleyelim ve tekrar çalıştıralım.

![mk73_4.gif](/assets/images/2004/mk73_4.gif)

Şekil 5. Delete işleminde meydana gelen istisna sonrası Commit işlemi uygulanırsa.

Önce, Baslat başlıklı butona tıklayarak Transaction'ımızı başlatalım. Ardından sırasıyla Ekle 1, Ekle 2 ve Sil başlıklı butonlara tıklayalım. Bu işlemler sonucunda Delete işleminde ID'si 1 olan satır olmadığından bir istisna oluşacak ve transaction'ımız RollBack metodu ile, Insert_2 olarak isimlendirdiğimiz SavePoint'e dönecektir. Bu noktada Onayla başlıklı butona tıklarsak, Commit işlemi sonucu Insert_2 noktasına kadar olan iki Insert işlemimizde onaylanacak ve veri tablosuna yansıtılacaktır.

![mk73_5.gif](/assets/images/2004/mk73_5.gif)

Şekil 6. Delete işlemindeki istisna sonrası Commit uygulandığında, tablonun son hali.

Bu makalemizde, Sql SavePoint'lerinin Ado.Net içindeki transaction'larda nasıl kullanıldığını incelemeye çalıştık. Geliştirdiğimiz örnek, sadece SavePoint'lerin kullanımını inceleme amacında olduğundan, farklı türden hatalara açıktır. Ancak önemli olan, SavePoint'lerin bir transaction nesnesi için nasıl kayıt edildikleri ve RollBack işlemleri sonucu bu noktalara nasıl dönülebildiğidir. Ayrıca, bu tip RollBack işlmeleri sonrasında verilen Commit emirlerinin verileri gerçek anlamda nasıl etkilediğinede dikkat etmek gerekir. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.