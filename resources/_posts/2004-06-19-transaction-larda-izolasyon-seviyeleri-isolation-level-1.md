---
layout: post
title: "Transaction' larda Izolasyon Seviyeleri (Isolation Level) - 1"
date: 2004-06-19 12:00:00 +0300
categories:
  - ado-net
tags:
  - ado.net
  - transaction
  - isolation-levels
---
Bu makalemizde, Transaction'larda kullanılan izolasyon seviyelerini incelemeye başlayacağız. Izolasyon seviyeleri, eşzamanlı olarak çalışan Transaction'ların birbirlerini nasıl etkilemesi gerektiğini belirtmekte kullanılır. Yani bir başka deyişle, bir Transaction içinde meydana gelen değişikliklerin, başka eş zamanlı Transactionlar tarafından nasıl ele alınması gerektiğini belirlememize olanak sağlar. Izolasyon seviylerini anlamanın en iyi yolu, eş zamanlı olarak çalışan Transaction'larda meydana gelebilecek sorunları iyi anlamaktan geçer.

Eşzamanlı olarak çalışan iki Transaction göz önüne alındığında, oluşabilecek durumlar üç tanedir. Phantoms, Non-Repeatable Read, Dirty Read. Her bir durumun, çalışan Transaction'lara etkisi farklıdır. Şimdi bu problemleri incelemeye çalışalım. Bu amaçla basit bir windows uygulaması geliştireceğiz. Uygulamamız, Sql sunucusu üzerinde çalışan veritabanı üzerinde ekleme, güncelleme, veri çekme gibi işlemler yapacak. Tüm bu işlemleri birer Transaction içerisinde gerçekleştireceğiz. Sonuç olarak, aynı anda bu uygulamalardan iki proses çalıştırıp, bahsettiğimiz problemleri simüle etmeyi deneyeceğiz. Öncelikle, Visual Studio.Net ortamında bir windows uygulamasını aşağıdakine benzer forma sahip olacak şekilde oluşturalım.

![mk74_1.gif](/assets/images/2004/mk74_1.gif)

Şekil 1. Form Tasarımımız.

Sıra geldi uygulama kodlarımızı yazmaya.

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
    trans=con.BeginTransaction();
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
    SqlCommand cmdBak=new SqlCommand("SELECT * FROM Personel",con);
    cmdBak.Transaction=trans;
    da=new SqlDataAdapter(cmdBak);
    DataTable dt=new DataTable();
    da.Fill(dt);
    dataGrid1.DataSource=dt;
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
    SqlCommand cmdGuncelle=new SqlCommand("UPDATE Personel SET ISIM='"+txtIsim.Text+"',     SOYISIM='"+txtSoyisim.Text+"' WHERE ID="+txtID.Text,con);
    cmdGuncelle.Transaction=trans;
    int sonuc=cmdGuncelle.ExecuteNonQuery();
    MessageBox.Show(sonuc+" SATIR GUNCELLENDI");
}

private void btnBakID_Click(object sender, System.EventArgs e)
{
    SqlCommand cmd=new SqlCommand("SELECT ISIM,SOYISIM FROM Personel WHERE ID="+txtID.Text,con);
    cmd.Transaction=trans;
    da=new SqlDataAdapter(cmd);    
    DataTable dt=new DataTable();
    da.Fill(dt);
    dataGrid1.DataSource=dt;
}
```

Kodlarımızda özel olarak yaptığımız bir şey yok. Ado.Net'i kullanarak, bizim için gerekli satırları oluşturduk. Şu an için odaklanmamız gereken, eş zamanlı Transaction'lar arasındaki problemlerin irdelenmesi.

Phantoms;

İki Transaction olduğunu ve bunların eş zamanlı olarak çalıştığını düşünelim. Transaction1, veri tabanından herhangibir tabloya ait belirli bir veri kümesini çekmiş olsun. Aynı veri kümesine Transaction2' ninde baktığını düşünelim. Transaction2, tabloya yeni satırlar girsin ve Transaction'ı onaylansın (Commit). Bu noktadan sonra, halen daha çalışan Transaction1, aynı veri kümesini tekrardan Transaction içerisinde talep ettiğinde, yeni eklenmiş satırlar olduğunu görecektir. Bu satırlar, hayalet olarak düşünülür ve bu nedele Phantom olarak adlandırılır. Çünkü, Transaction1 işlemini sonlandırmadan veri kümesinin yeni bir şekline sahip olmuştur. Yeni eklenen satırlar onun için bir anda ortaya çıkan hayalet satırlar gibidir. Şimdi dilerseniz, bu olayı simüle etmeye çalışalım. Öncelikle, geliştirdiğimiz uygulamadan iki adet çalıştırmamız gerekiyor. Sonra bu uygulamalardan Başlat başlıklı butonlara tıklayarak Transaction'larımızı çalıştırmalıyız. Artık elimizde çalışan iki eş zamanlı Transaction var. Şimdi, yukarıda bahsettiğimiz senaryoyu uygulayalım. Önce Transaction'lardan birisi ile veri çekelim.

![mk74_2.gif](/assets/images/2004/mk74_2.gif)

Şekil 2. Transaction1 veri çekiyor.

Ardından diğer uygulamada çalışan Transaction üzerinden yeni bir kaç kayıt girelim ve bu Transaction'ı Commit edelim.

![mk74_3.gif](/assets/images/2004/mk74_3.gif)

Şekil 3. Eşzamanlı çalışan Transaction2 yeni satır girdi ve işlem onaylandı (Commit).

Şimdi çalışmaya devam eden yani açık olan Transaction'ın olduğu uygulamaya geçelim ve veri kümesini yeniden Bak isimli butona tıklayarak isteyelim.

![mk74_4.gif](/assets/images/2004/mk74_4.gif)

Şekil 4. Transaction1 için, Phantom Satırlar Oluştu.

İşte burada oluşan Ahmet Nacaroğlu satırı hayalet satırdır. Bu durumun meydana gelmesi için, giriş işlemlerini yapan Transaction'ın onaylanmış olması gerektiğini hatırlatmak isterim. Aksi takdirde, diğer Transaction ile veriler çekilmek istendiğinde, belirli bir süre diğer Transaction'ın bu işlemleri tamamlayıp (Commit) tamamlamıyacağı (Rollback) beklenir ve bu süre sonunda bekelenen olmassa uygulama bir istisna fırlatır. (Bu nedenle, Transaction2' de girdiğimiz satırlardan sonra, Onayla başlıklı butona basmayı unutmayalım.)

Non-Repeatable Reads;

Yine iki eş zamanlı çalışan Transaction'ımız olduğunu düşünelim. Transaction'lardan birisi yine veri çekmiş olsun. Özellikle çektiği belirli bir satır olabilir. Diğer Transaction'da bu belirli satırı veya başkalarını güncellesin ve işlemi onaylansın (Commit). İşte bu noktadan sonra, diğer çalışmakta olan Transaction aynı satırlara yeniden baktığında verilerin değişmiş olduğunu görecektir. İşte bu satırlardaki ani ve bilinmeyen değişiklikler nedeni ile, bu durum Non-Repeatable Read olarak adlandırılmıştır. Şimdi bu durumu simüle edebilmek için, yine uygulamamızdan iki tane çalıştıralım ve Transaction'larımızı başlatalım. İlk Transaction ile okuduğumuz verilerden belirli bir satır üzerinde, ikinci Transaction'da değişiklik yapalım ve bu Transaction'ı onaylayalım (Commit).

![mk74_5.gif](/assets/images/2004/mk74_5.gif)

Şekil 5. Transaction2' de Transaction1' de aynen görünen satırlardan birisinde güncelleme yapıldı.

Şimdi bu Transaction'ı onaylayalım ve diğer Transaction'da verileri tekrardan çekelim. Bu durumda, ID değeri 58 olan satırın verilerinin değişmiş olduğunu görülür.

![mk74_6.gif](/assets/images/2004/mk74_6.gif)

Şekil 6. Transaction1 için NonRepeatable-Read durumu.

Dirty Reads;

Bu durum Phantoms ve Non-Repeatable Read durumlarına göre biraz daha farklıdır. Eş zamanlı olarak çalışan Transaction'lardan birisi, diğerinin okuduğu aynı veriler üzerinde değişiklik yapar. Aynen Non-Repeatable Read'e neden olan durumda olduğu gibi. Ancak önemli bir fark vardır. Değişiklik yapan Transaction Commit edilmeden, diğer Transaction aynı satırıları tekrar okur ve değişiklikleri görür. Bu andan sonra, değişiklikleri yapan Transaction yaptığı güncellemeleri RollBack ile geri alır. İşte bu noktada, verileri okuyan Transaction'da geri alınmış (RollBack) yani veri tabanına yansıtılmamış değişiklikler halen daha var olacaktır ki aslında bu değişiklikler mevcut değildir. İşte bu durum Dirty-Reads olarak adlandırılır. Şimdi bu durumu simüle etmeye çalışalım. Ancak burada söz konusu olan Transaction'lar eş zamanlı çalışmakla birlikte, birisi Commit veya RollBack edilmeden diğerininde veri çekme gibi işlemleri yapabilmesine izin veren yapıdadırlar. O nedenle bir sonraki makalede üzerinde duracağımız IsolationLevel numaralandırıcısı türünden bir değer ile Transaction'ları çalıştırmamız lazım. Bu amaçla aşağıdaki satırı;

```csharp
trans=con.BeginTransaction();
```

aşağıdaki ile değiştirelim.

```csharp
trans=con.BeginTransaction(IsolationLevel.ReadUncommitted);
```

Şu aşamada bunun ne anlama geldiğinin çok önemi yok. Bunu ve diğer numaralandırıcı değerlerinin etkilerini bir sonraki makalemizde incelemeye çalışacağız. Şimdi yine iki uygulamamızı aynı anda çalıştıralım ve Transaction'larımızı başlatalım. Önce Transaction1 için verileri çekelim. Transaction2' de belli bir satır üzerinde değişikliklik yapalım.

![mk74_7.gif](/assets/images/2004/mk74_7.gif)

Şekil 7. Eşzamanlı çalışan Transaction2' de belirli bir satır üzerinde güncelleme yapıldı.

Şimdi Transaction1 içinde, verileri tekrardan çekelim. Bu durumda Transaction2 ile, ID değeri 70 olan satır üzerinde yapılmış olan değişikliği görürüz.

![mk74_8.gif](/assets/images/2004/mk74_8.gif)

Şekil 8. Transaction1 verileri tekrardan çekiyor ve değişiklikleri görüyor.

Şimdi Transaction2' de yaptığımız güncelleme işlemini geri alalım. İşte bu durumda, Transaction1 içinde, Transaction2 tarafından satırın güncellenmiş fakat onaylanmamış, daha da önemlisi geri alınmış hali kalır. İşte bu durum Dirty Reads olarak adlandırılmaktadır. Bu üç durum ile ilgili olarak alınabilecek tedbirler, IsolationLevel numaralandırıcısı ile belirlenir. Bu numaralandırıcıyı ve kullanım şekillerini bir sonraki makalemizde incelemeye çalışacağız. Hepinize mutlu günler dilerim.