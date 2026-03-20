---
layout: post
title: "Bir Arayüz, Bir Sınıf ve Bir Tablo"
date: 2004-01-14 12:00:00 +0300
categories:
  - csharp
tags:
  - csharp
  - dotnet
  - sql-server
  - visual-studio
---
Bugünkü makalemizde, bir arayüzü uygulayan sınıf nesnelerinden faydalanarak, bir Sql tablosundan nasıl veri okuyacağımızı ve değişiklikleri veritabanına nasıl göndereceğimizi incelemeye çalışacağız. Geliştireceğimiz örnek, arayüzlerin nasıl oluşturulduğu ve bir sınıfa nasıl uygulandığını incelemekle yetinmeyecek, Sql veritabanımızdaki bir tablodaki belli bir kayda ait verilerin bu sınıf nesnelerine nasıl aktarılacağını da işleyecek. Kısacası uygulamamız, hem arayüzlerin hem sınıfların hemde Sql nesnelerinin kısa bir tekrarı olucak.

Öncelikle uygulamamızın amacından bahsedelim. Uygulamamızı bir Windows uygulaması şeklinde geliştireceğiz. Kullanacağımız veri tablosunda arkadaşlarımızla ilgili bir kaç veriyi tutuyor olacağız. Kullanıcı, Windows formunda, bu tablodaki alanlar için Primary Key niteliği taşıyan bir ID değerini girerek, buna karşılık gelen tablo satırına ait verilerini elde edicek. İstediği değişiklikleri yaptıktan sonra ise bu değişiklikleri tekrar veritabanına gönderecek. Burada kullanacağımız teknik makalemizin esas amacı olucak. Bu kez veri tablosundan çekip aldığımız veri satırının programdaki eşdeğeri, oluşturacağımız sınıf nesnesi olucak. Bu sınıfımız ise, yazmış olduğumuz arayüzü uygulayan bir sınıf olucak. Veriler sınıf nesnesine, satırdaki her bir alan değeri, aynı isimli özelliğe denk gelicek şekilde yüklenecek. Yapılan değişiklikler yine bu sınıf nesnesinin özelliklerinin sahip olduğu değerlerin veri tablosuna gönderilmesi ile gerçekleştirilecek.

Uygulamamızda, verileri Sql veritabanından çekmek için, SqlClient isim uzayında yer alan SqlConnection ve SqlDataReader nesnelerini kullanacağız. Hatırlayacağınız gibi SqlConnection nesnesi ile, bağlanmak istediğimiz veritabanına, bu veritabanının bulunduğu sunucu üzerinden bir bağlantı tanımlıyoruz. SqlDataReader nesnemiz ile de, sadece ileri yönlü ve yanlız okunabilir bir veri akımı sağlayarak, aradığımız kayda ait verilerin elde edilmesini sağlıyoruz.

Şimdi uygulamamızı geliştirmeye başlayalım. Öncelikle vs.net ortamında bir Windows Application oluşturalım. Burada aşağıdaki gibi bir form tasarlayalım.

![mk41_1.gif](/assets/images/2004/mk41_1.gif)

Şekil 1. Form tasarımımız.

Kullanıcı bilgilerini edinmek istediği kişinin ID'nosunu girdikten sonra, Getir başlıklı butona tıklayarak ilgili satırın tüm alanlarına ait verileri getirecek. Ayrıca, kullanıcı veriler üzerinde değişiklik yapabilecek ve bunlarıda Güncelle başlıklı butona tıklayarak Sql veritabanındaki tablomuza aktarabilecek. Sql veritabanında yer alan Kisiler isimli tablomuzun yapısı aşağıdaki gibidir.

![mk41_2.gif](/assets/images/2004/mk41_2.gif)

Şekil 2. Tablomuzun yapısı.

Şimdi gelelim işin en önemli ve anahtar kısımlarına. Program kodlarımız. Öncelikle arayüzümüzü tasarlayalım. Arayüzümüz, sonra oluşturacağımız sınıf için bir rehber olucak. Sınıfımız, veri tablomuzdaki alanları birer özellik olarak taşıyacağına göre arayüzümüzde bu özellik tanımlarının yer alması gerektiğini söyleyebiliriz. Ayrıca ilgili kişiye ait verileri getirecek bir metodumuzda olmalıdır. Elbette bu arayüze başka amaçlar için üye tanımlamalarıda ekleyebiliriz. Bu konuda tek sınır bizim hayal gücümüz. İşin gerçeği bu makalemizde hayal gücümü biraz kısdım konunun daha fazla dağılmaması amacıyla:). (Ama siz, örneğin kullanıcının yeni girdiği verileri veritabanına yazıcak bir metod tanımınıda bir üye olarak ekleyebilir ve gerekli kodlamaları yapabilirsiniz.) İşte arayüzümüzün kodları.

```csharp
using System;

public interface IKisi
{
    /* Öncelikle tablomuzdaki her alana karşılık gelen özellikler için tanımlamalarımızı yapıyoruz.*/

    int KisiID /* KisiID, tablomuzda otomatik artan ve primary key olan bir alandır. Dolayısıyla programcının var olan bir KisiID'sini değiştirmemesi gerekir. Bu nedenle sadece okunabilir bir özellik olarak tanımlanmasına izin veriyoruz. */
    {
        get;
    }
    string Ad /* Tablomuzdaki char tipindeki Ad alanımız için string tipte bir alan.*/
    {
        get; set;
    }
    string Soyad
    {
        get; set;
    }
    DateTime DogumTarihi
    /* Tablomuzda, DogumTarihi alanımız datetime tipinde olduğundan, DateTime tipinde bir özellik tanımlanmasına izin veriyoruz.*/
    {
        get; set;
    }
    string Meslek
    {
        get; set;
    }
    void Bul(int KID); /* Bul metod, KID parametresine göre, tablodan ilgili satıra ait verileri alıcak ve alanlara karşılık gelen özelliklere atayacak metodumuzdur.*/
}
```

Şimdide bu arayüzümüzü uygulayacağımız sınıfımızı oluşturalım. Sınıfımız IKisi arayüzünde tanımlanan her üyeyi uygulamak zorundadır. Bu bildiğiniz gibi arayüzlerin bir özelliğidir.

```csharp
using System.Data.SqlClient;
using System.Data;
using System;

public class CKisi : IKisi /* IKisi arayüzünü uyguluyoruz.*/
{
    /* Öncelikle sınıftaki özelliklerimiz için, verilerin tutulacağı alanları tanımlıyoruz.*/

    private int kisiID;
    private string ad;
    private string soyad;
    private DateTime dogumTarihi;
    private string meslek;
    /* Arayüzümüzde yer alan üyeleri uygulamaya başlıyoruz.*/
    public int KisiID
    {
        get { return kisiID; }
    }
    public string Ad
    {
        get { return ad; }
        set { ad = value; }
    }
    public string Soyad
    {
        get { return soyad; }
        set { soyad = value; }
    }
    public DateTime DogumTarihi
    {
        get { return dogumTarihi; }
        set { dogumTarihi = value; }
    }
    public string Meslek
    {
        get { return meslek; }
        set { meslek = value; }
    }
    public void Bul(int KID)
    {
        /* Öncelikle Sql Veritabanımıza bir bağlantı açıyoruz.*/
        SqlConnection conFriends =
new SqlConnection("data source=localhost;integrated security=sspi;initial catalog=Friends");
        /* Tablomuzdan, kullanıcının bu metoda parametre olarak gönderdiği KID değerini baz alarak, ilgili KisiID'ye ait verileri elde edicek sql kodunu yazıyoruz.*/
        string sorgu = "Select * From Kisiler Where KisiID=" + KID.ToString();
        /* SqlCommand nesnemiz yardımıyla sql sorgumuzu çalıştırılmak üzere hazırlıyoruz.*/
        SqlCommand cmd = new SqlCommand(sorgu, conFriends);
        SqlDataReader rd;
        /* SqlDataReader nesnemizi yaratıyoruz.*/
        conFriends.Open();
        /* Bağlantımızı açıyoruz. */
        rd = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        /* ExecuteReader ile sql sorgumuzu çalıştırıyoruz ve sonuç kümesi ile SqlDataReader nesnemiz arasında bir akım(stream) açıyoruz. CommandBehavior.CloseConnection sayesinde, SqlDataReader nesnemizi kapattığımızda, SqlConnection nesnemizinde otomatik olarak kapanmasını sağlıyoruz.*/

        while (rd.Read())
        {
            /* Eğer ilgili KisiID'ye ait bir veri satırı bulunursa, SqlDataReader nesnemizin Read metodu sayesinde, bu satıra ait verileri sınıfımızın ilgili alanlarına aktarıyoruz. Böylece, bu alanların atandığı sınıf özellikleride bu veriler ile dolmuş oluyor.*/
            kisiID = (int)rd["KisiID"];
            ad = rd["Ad"].ToString();
            soyad = rd["Soyad"].ToString();
            dogumTarihi = (DateTime)rd["DogumTarihi"];
            meslek = rd["Meslek"].ToString();
        }
        rd.Close();
    }
    public CKisi()
    {
    }
}
```

Artık IKisi arayüzünü uygulayan, CKisi isimli bir sınıfımız var.Şimdi Formumuzun kodlarını yazmaya başlayabiliriz. Öncelikle module düzeyinde bir CKisi sınıf nesnesi tanımlayalım.

CKisi kisi=new CKisi ();

Bu nesnemiz veri tablosundan çektiğimiz veri satırına ait verileri taşıyacak. Kullanıcı Getir başlıklı button kontrolüne bastığında olucak olayları gerçekleştirecek kodları yazalım.

```csharp
using System;

private void btnGetir_Click(object sender, System.EventArgs e)
{
    int id = Convert.ToInt32(txtKisiID.Text.ToString()); /* Kullanıcının TextBox kontrolüne girdiği ID değeri Convert sınıfının ToInt32 metodu ile Integer'a çeviriyoruz.*/
    kisi.Bul(id);
    /* Kisi isimli CKisi sınıfından nesne örneğimizin Bul metodunu çağırıyoruz.*/
    Doldur();
    /* Doldur Metodu, kisi nesnesinin özellik değerlerini, Formumuzdaki ilgili kontrollere alarak, bir nevi veri bağlama işlemini gerçekleştirmiş oluyor.*/
}
```

Şimdide Doldur metodumuzun kodlarını yazalım.

```csharp
public void Doldur()
{
    txtAd.Text = kisi.Ad.ToString(); /* txtAd kontrolüne, kisi nesnemizin Ad özelliğinin şu anki değeri yükleniyor. Yani ilgili veri satırının ilgili alanı bu kontrole bağlamış oluyor.*/
    txtSoyad.Text = kisi.Soyad.ToString();
    txtMeslek.Text = kisi.Meslek.ToString();
    txtDogumTarihi.Text = kisi.DogumTarihi.ToShortDateString();
    lblKisiID.Text = kisi.KisiID.ToString();
}
```

Evet görüldüğü gibi artık aradığımız kişiye ait verileri formumuzdaki kontrollere yükleyebiliyoruz. Şimdi TextBox kontrollerimizin TextChanged olaylarını kodlayacağız. Burada amacımız, TextBox'larda meydana gelen değişikliklerin anında, CKisi sınıfından türettiğimiz Kisi nesnesinin ilgili özelliklerine yansıtılabilmesi. Böylece yapılan değişiklikler anında nesnemize yansıyacak. Bu nedenle aşağıdaki kodları ekliyoruz.

```csharp
/* Metodumuz bir switch case ifadesi ile, aldığı ozellikAdi parametresine göre, CKisi isimli sınıfımıza ait Kisi nesne örneğinin ilgili özelliklerini değiştiriyor.*/

using System;

public void Degistir(string ozellikAdi, string veri)
{
    switch (ozellikAdi)
    {
        case "Ad":
            {
                kisi.Ad = veri;
                break;
            }
        case "Soyad":
            {
                kisi.Soyad = veri;
                break;
            }
        case "Meslek":
            {
                kisi.Meslek = veri;
                break;
            }
        case "DogumTarihi":
            {
                kisi.DogumTarihi = Convert.ToDateTime(veri);
                break;
            }
    }
}

private void txtAd_TextChanged(object sender, System.EventArgs e)
{
    Degistir("Ad", txtAd.Text);
}

private void txtSoyad_TextChanged(object sender, System.EventArgs e)
{
    Degistir("Soyad", txtSoyad.Text);
}

private void txtDogumTarihi_TextChanged(object sender, System.EventArgs e)
{
    Degistir("DogumTarihi", txtDogumTarihi.Text.ToString());
}

private void txtMeslek_TextChanged(object sender, System.EventArgs e)
{
    Degistir("Meslek", txtMeslek.Text);
}

private void btnGuncelle_Click(object sender, System.EventArgs e)
{
    int id;
    id = Convert.ToInt32(lblKisiID.Text.ToString());
    Guncelle(id);
}
```

Görüldüğü gibi kodlarımız gayet basit. Şimdi güncelleme işlemlerimizi gerçekleştireceğimiz kodları yazalım. Kullanıcımız, TextBox kontrollerinde yaptığı değişikliklerin veritabanınada yansıtılmasını istiyorsa Guncelle başlıklı button kontrolüne tıklayacaktır. İşte kodlarımız.

```csharp
using System.Data.SqlClient;
using System;

private void btnGuncelle_Click(object sender, System.EventArgs e)
{

    /* Güncelleme işlemi, şu anda ekranda olan Kişi için yapılacağından, bu kişiye ait KisiID sini ilgili Label konrolümüzden alıyoruz ve Guncelle isimli metodumuza parametre olarak gönderiyoruz. Asıl güncelleme işlemi Guncelle isimli metodumuzda yapılıyor. */

    int id;
    id = Convert.ToInt32(lblKisiID.Text.ToString());
    Guncelle(id);
}

public void Guncelle(int ID)
{

    /* Sql Server'ımıza bağlantımızı oluşturuyoruz.*/

    SqlConnection conFriends = new SqlConnection("data source=localhost;integrated security=sspi;initial catalog=Friends");

    /* Update sorgumuzu oluşturuyoruz. Dikkat edicek olursanız alanlara atanacak değerler, kisi isimli nesnemizin özelliklerinin değerleridir. Bu özellik değerleri ise, TextBox kontrollerinin TextChanged olaylarına ekldeğimiz kodlar ile sürekli güncel tutulmaktadır. En ufak bir değişiklik dahi buraya yansıyabilecektir.*/
    string sorgu = "Update Kisiler Set Ad='" + kisi.Ad + "',Soyad='" + kisi.Soyad + "',Meslek='" + kisi.Meslek + "',DogumTarihi='" + kisi.DogumTarihi.ToShortDateString() + "' Where KisiID=" + ID;

    SqlCommand cmd = new SqlCommand(sorgu, conFriends); /* SqlCommand nesnemizi sql cümleciğimiz ve geçerli bağlantımız ile oluşturuyoruz. */

    conFriends.Open(); /* Bağlantımızı açıyoruz.*/

    try
    {
        cmd.ExecuteNonQuery(); /* Komutumuzu çalıştırıyoruz.*/
    }
    catch
    {
        MessageBox.Show("Başarısız");
    }
    finally /* Update işlemi herhangibir neden ile başarısız olsada, olmasada sonuç olarak(finally) açık olan SqlConnection bağlanıtımızı kapatıyoruz. */
    {
        conFriends.Close();
    }
}
```

İşte uygulama kodlarımız bu kadar. Şimdi gelin uygulamamızı çalıştırıp deneyelim. Öncelikle KisiID değeri 1000 olan satıra ait verileri getirelim.

![mk41_3.gif](/assets/images/2004/mk41_3.gif)

Şekil 3. KisiID=1000 Kaydına ait veriler Kisi nesnemize yüklenir.

Şimdi verilerde bir kaç değişiklik yapalım ve güncelleyelim. Ben Ad alanında yer alan "S." değerini "Selim" olarak değiştirdim. Bu durum sonucunda yapılan değişikliklerin veritabanına yazılıp yazılmadığını ister programımızdan tekrar 1000 nolu satırı getirerek bakabiliriz istersekde Sql Server'dan direkt olarak bakabiliriz. İşte sonuçlar.

![mk41_4.gif](/assets/images/2004/mk41_4.gif)

Şekil 4. Güncelleme işleminin sonucu.

Programımız elbette gelişmeye çok, ama çok açık. Örneğin kodumuzda hata denetimi yapmadığımız bir çok ölü nokta var. Bunların geliştirilmesini siz değerli okurlarımıza bırakıyorum. Bu makalemizde özetle, bir arayüzü bir sınıfa nasıl uyguladığımızı, bu arayüzü nasıl yazdığımızı hatırlamaya çalıştık. Ayrıca, sınıfımıza ait bir nesne örneğine, bir tablodaki belli bir veri satırına ait verileri nasıl alabileceğimizi, bu nesne özelliklerinde yaptığımız değişiklikleri tekrar nasıl veri tablosuna gönderebileceğimizi inceledik. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.