---
layout: post
title: "Sql Tablolarına Resim Eklemek"
date: 2004-01-23 02:00:00 +0300
categories:
  - ado-net
tags:
  - ado-net
  - csharp
---
Bugünkü makalemizde, örnek bir sql tablomuzda yer alan image veri tipinden bir alana, seçtiğimiz resim dosyalarını nasıl kaydedebileceğimizi incelemeye çalışacağız. Öncelikle, sql tablolarında kullanabildiğimiz image tipinden biraz bahsedelim. Bu tip, ikili (binary) formatta verileri tutmak için geliştirilmiştir. Image veri tipi, 0 byte'dan 2,147,483,647 byte'a kadar veri alanını taşıyabilmektedir. Bu alan, verileri bir byte dizisi şeklinde tutmaktadır. Dolayısıyla resim dosyalarını tutmak için ideal bir yapı sergiler. Elbette üst sınırı aşmamaya çalışmak gerekir. Çoğu zaman uygulamalarımızda, resim dosyalarını ikili bir dizi şeklinde sql tablolarımızda, image tipindeki alanlarda tutmak isteyeceğimiz durumlar oluşabilir. (Örneğin şirket çalışanları ile ilgili personel bilgilerini tuttuğumuz tablolarda, personelin vesikalık fotoğraflarını bu alanlarda taşıdığımızı düşünelim.)

İşte şimdi, bu tarz resim dosyalarını, sql tablolarımızdaki ilgili alanlara nasıl yazabileceğimizi inceleyeceğiz. Yapmamız gereken işlem aslında son derece kolay. Resim dosyasını ikili formatta okumak, dosyanın okunan byte'larını bir byte dizisine aktarmak ve oluşan bu diziyi, image tipindeki alanımıza aktarmak. Bu anafikir ışığında işlemlerimizi gerçekleştirebilmek için, öncelikle dosyamızı bir FileStream nesnesine açacağız. Daha sonra, bir BinaryRead nesnesi kullanarak, FileStream nesnesinin işaret ettiği dosyadan tüm byte'ları okuyacak ve bunları bir byte dizisinee aktaracağız. Sonrada oluşturduğumuz bu diziyi, sql tablomuzda yer alan image veri tipindeki alana koyacağız.

Uygulamamızı gerçekleştirmeden önce, FileStream ve BinaryReader sınıfları hakkında da kısaca bilgi verelim. FileStream nesnelerini, sistemimizde yer alan dosyaları okumak veya bu dosyalara yazmak amacıyla kullanırız. BinaryReader nesnesi, FileStream nesnesinden byte türünden bir akış oluşturmamızı sağlar. BinaryReader, FileStream nesnesinin temsil ettiği dosyadan, okumanın yönlendirileceği kaynağa doğru bir akım oluşturur. Bu akış sayesinde, FileStream nesnesinin temsil ettiği dosyadan verileri byte byte okuyabilir ve bir byte dizisine aktarabiliriz. Bunun nasıl yapıldığını örneğimizi geliştirdiğimizde daha iyi anlayacağız.

Şimdi dilerseniz, uygulamamızı geliştirmeye başlayalım. Öncelikle, veri tablomuzu yapalım. Örneğin, internetten indirip bilgisayarımızda bir klasörde topladığımız güzel duvar kağıtlarını tablomuzada kaydetmek istediğimizi varsayalım. Bununla ilgili olarak aşağıdaki örnek tabloyu oluşturdum.

![mk45_1.gif](/assets/images/2004/mk45_1.gif)

Şekil 1. Wallpapers tablomuz

Sıra geldi uygulamamızın ekranını tasarlamaya. Uygulamamızda kullanıcı, istediği resim dosyasını seçecek, bunu aynı zamanda ekranda yer alan bir PictureBox kontrolünde görebilecek ve bunu isterse Wallpapers isimli sql tablomuza yazabilecek. İşte ekran tasarımımız.

![mk45_2.gif](/assets/images/2004/mk45_2.gif)

Şekil 2. Form Tasarımımız.

Şimdi uygulama kodlarımızı yazmaya başlayabiliriz.

```csharp
string resimAdresi; /* OpenFileDialog kontrolünden seçtigimiz dosyanin tam adresini tutacak genel bir degisken. */

/* Bu metodumuzda OpenFileDialog kontrolümüzün temel ayarlarini yapiyoruz. */

public void DialogHazirla()
{
     ofdResim.Title="Duvar Kagidini Seç"; /* Dosya açma iletisim kutumuzun basligini belirliyoruz. */

     ofdResim.Filter="Jpeg Dosyalari(*.jpg)|*.jpg|Gif dosyalari(*.gif)|*.gif"; /* Iletisim kutumuzun, sadece jpg ve gif dosyalarini göstermesini, Filter özelligi ile ayarliyoruz.*/
}

private void Form1_Load(object sender, System.EventArgs e)
{
     DialogHazirla();
}

private void btnResimSec_Click(object sender, System.EventArgs e)
{
     /* Kullanici bu butona tikladiginda, OpenFileDialog kontrolümüz, dosya açma iletisim kutusunu açar. Kullanici bir dosya seçip OK tusunda bastiginda, Picture Box kontrolümüze seçilen resim dosyasi alinarak gösterilmesi sağlanır. Daha sonra seçilen dosyanin tam adresi label kontrolümüze alınır ve resimAdresi degiskenimize atanır. */

     if(ofdResim.ShowDialog()==DialogResult.OK)
     {
         pbResim.Image=System.Drawing.Image.FromFile(ofdResim.FileName); /* Drawing isim uzayinda yer alan Image sinifinin FromFile metodunu kullanarak belirtilen adresteki dosya PictureBox kontrolü içine çizilir. */

          lblDosyaAdi.Text=ofdResim.FileName.ToString();
          resimAdresi=ofdResim.FileName.ToString();
     }
}

private void btnKaydet_Click(object sender, System.EventArgs e)
{
     /* Simdi en önemli kodlarimizi yazmaya basliyoruz. Öncelikle dosyamizi açmamiz gerekli. Çünkü resim dosyasinin içerigini byte olarak okumak istiyoruz. Bu amaçla FileStream nesnemizi olusturuyor ve gerekli parametrelerini ayarliyoruz. Ilk parametre, dosyanin tam yolunu belirtir. Ikinci parametre ise dosyamizi açmak için kullanacagimizi belirtir. Son parametre ise dosyanin okuma amaci ile açildigini belirtir. */

     FileStream fsResim=new FileStream(resimAdresi,FileMode.Open,FileAccess.Read);

     /* BinaryReader nesnemiz, byte dizimiz ile, parametre olarak aldigi FileStream nesnesi arasinda , veri akisini saglamak için olusturuluyor. Akim, FileStream nesnesinin belirttigi dosyadan, dosyadaki byte'larin aktarilacagi diziye dogru olucaktir.*/

        BinaryReader brResim=new BinaryReader(fsResim);

     /* Simdi resim adinda bir byte dizisi olusturuyoruz. brResim isimli BinaryReader nesnemizin, ReadBytes metodunu kullanarak, bu nesnenin veri akisi için baglanti kurdugu FileStream nesnesinin belirttigi dosyadaki tüm byte'lari, byte dizimize akitiyoruz. Böylece resim dosyamizin tüm byte'lari yani dosyamizin kendisi, byte dizimize aktarilmis oluyor.*/

         byte[] resim=brResim.ReadBytes((int)fsResim.Length);
     /* Son olarak, BinaryReader ve FileStream nesnelerini kapatiyoruz. */
     brResim.Close();
     fsResim.Close();
     /* Artik Sql baglantimizi olusturabilir ve sql komutumuzu çalistirabiliriz. Önce SqlConnection nesnemizi olusturuyoruz. */

     SqlConnection conResim=new SqlConnection("data source=localhost;initial catalog=Northwind;integrated security=sspi");

     /* Simdi sql komutumuzu çalistiracak olan SqlCommand nesnemizi olusturuyoruz. Burada alanlarin degerlerini parametreler üzerinden aktardigimiza dikkat edelim. */

     SqlCommand cmdResimKaydet=new SqlCommand("insert into Wallpapers (Yorum,Resim) values (@yorum,@res)",conResim);
   cmdResimKaydet.Parameters.Add("@Yorum",SqlDbType.Char,250).Value=txtYorum.Text; /* Bu parametremiz @Yorum isminde ve Char tipinde. 250 karakter uzunlugunda. Hemen ayni satirda Value özelligini kullanarak parametrenin degerinide belirliyoruz. */
     cmdResimKaydet.Parameters.Add("@res",SqlDbType.Image,resim.Length).Value=resim; /* Seçtigimiz resim dosyasinin byte'larini, tablodaki ilgili alana tasiyacak parametremizi belirtiyoruz. Deger olarak, resim isimli byte dizimizi aktariyoruz. Parametre tipinin, image olduguna dikkat edelim. */

 
     /* Günveli blogumuzda, Sql baglantimizi açiyoruz. Ardindan, sql komutumuzu ExecuteNonQuery metodu ile çalistiriyoruz. Son olarakta herhangibir hata olsada, olmasada, finally blogunda sql baglantimizi kapatiyoruz.*/

     try
     {
          conResim.Open();
          cmdResimKaydet.ExecuteNonQuery();
          MessageBox.Show(lblDosyaAdi.Text+" Tabloya Basarili Bir Sekilde Kaydedildi.");
     }
     catch(Exception ex)
     {
          MessageBox.Show(ex.Message.ToString());
     }
     finally
     {
          conResim.Close();
     }
}
```

Şimdi uygulamamızı çalıştıralım ve tablomuzdaki Resim alanına yazmak için bir resim seçelim.

![mk45_3.gif](/assets/images/2004/mk45_3.gif)

Şekil 3. Resim Seçilir.

Şimdi Kaydet butonuna tıklayalım. Bu durumda aşağıdaki mesajı alırız.

![mk45_4.gif](/assets/images/2004/mk45_4.gif)

Şekil 4. Resim image tipindeki alana kaydedildi.

Son olarak sql tablomuzda bu alanların nasıl göründüğüne bir bakalım.

![mk45_5.gif](/assets/images/2004/mk45_5.gif)

Şekil 5. Tablonun görünümü.

Gördüğünüz gibi, tablomuzda Resim adlı image tipindeki alanımızda yazmaktadır. Acaba gerçekten resmimiz bu alana düzgün bir şekilde kaydedildi mi? Bir sonraki makalemizde, bu kez var olan image alanlarını, tablodan nasıl okuyacağımızı ve bir dosyaya nasıl kaydedebileceğimizi incelemeye çalışacağız. Umuyorumki hepiniz için yararlı bir makale olmuştur. Bir sonraki makalemizde görüşmek dileğiyle mutlu günler, iyi çalışmalar dilerim.