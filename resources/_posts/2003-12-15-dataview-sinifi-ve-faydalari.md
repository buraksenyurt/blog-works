---
layout: post
title: "DataView Sınıfı ve Faydaları"
date: 2003-12-15 04:00:00 +0300
description: "Bugünkü makalemizde getirileri ve performansı ile ADO.NET içerisinde önemli bir yere sahip olan DataView nesnesini incelemeye çalışacağız. Özellikle bu sınıfa ait, RowFilter özelliğinin ne gibi faydalar sağlıyacağına da değineceğiz. DataView sınıfı, veritabanı yönetim sistemlerindeki view nesneleri göz önüne alınarak oluşturulmuş bir sınıftır. Bilindiği gibi veritabanı yönetim sistemlerinde (DBMS-DataBase Management System) , bir veya birden fazla tablo için gerçekleştireceğimiz çeşitli tipteki birleştirici veya ayrı ayrı sorgulamalar sonucu elde edilen veri kümelerini view nesnelerine aktarabilmekteyiz. View nesneleri sahip oldukları veri alt kümelerini temsil eden birer veritabanı nesnesi olarak, önceden derlendiklerinden, süratli ve performansı yüksek yapılardır."
categories:
  - ado-net
tags:
  - ado-net
  - csharp
  - dotnet
  - performance
  - datatable
---
# DataView Sınıfı ve Faydaları
Değerli Okurlarım, Merhabalar.

Bugünkü makalemizde getirileri ve performansı ile ADO.NET içerisinde önemli bir yere sahip olan DataView nesnesini incelemeye çalışacağız. Özellikle bu sınıfa ait, RowFilter özelliğinin ne gibi faydalar sağlıyacağına da değineceğiz.

DataView sınıfı, veritabanı yönetim sistemlerindeki view nesneleri göz önüne alınarak oluşturulmuş bir sınıftır. Bilindiği gibi veritabanı yönetim sistemlerinde (DBMS-DataBase Management System), bir veya birden fazla tablo için gerçekleştireceğimiz çeşitli tipteki birleştirici veya ayrı ayrı sorgulamalar sonucu elde edilen veri kümelerini view nesnelerine aktarabilmekteyiz. View nesneleri sahip oldukları veri alt kümelerini temsil eden birer veritabanı nesnesi olarak, önceden derlendiklerinden, süratli ve performansı yüksek yapılardır.

Söz gelimi her hangibi tabloya ait bir filtreleme işini bir view nesnesinde barındırabiliriz. Bunun sonucu olarak, aynı sorguyu yazıp çalıştırmak, view nesnesinin içeriğine (yani sahip olduğu verilere) bakmaktan çok daha yavaştır. Bununla birlikte bu sorgulamanın birden fazla tabloyu içerdiğini düşünürsek, bu durumda da çalıştırılan sorgu sonucu elde edilecek veri alt kümelerini bir (birkaç) view nesnesinde barındırmak bize performans, hız olarak geri dönecektir.

Gelelim ADO.NET’ e. ADO.NET içersinde de, view lara benzer bir özellik olarak DataView nesneleri yer almaktadır. DataView nesneleri, veritabanı yönetim sistemlerinde yer alan view’lar ile benzerdir. Yine performans ve hız açısından avantajlıdır. Bunların yanında bir DataView nesnesi kullanılabilmek, mutlaka bir DataTable nesnesini gerektirmektedir. Nitekim DataView nesnesinin sahip olacağı veri alt kümeleri bu dataTable nesnesinin bellekte işaret ettiği tablo verileri üzerinden alınacaktır. DataView nesnesinin kullanımının belkide en güzel yeri şudur; bir DataTable nesnesinin bellekte işaret ettiği tablodan, bir den fazla görünüm elde ederekten, bu farklı görünümleri birden fazla kontrole bağlayarak, ekranda aynı anda tek bir tablonun verilerine ait birden fazla veri kümesini izlememiz mümkün olabilmektedir. İşte bu, bence DataView nesnesi (lerini) kullanmanın ne kadar faydalı olduğunu göstermektedir.

Bilindiği gibi DataTable nesnesine ait Select özelliğine ifadeler atayarakta var olan bir tablodan veri alt kümeleri elde edebiliyorduk. Fakat bu select özelliğine atanan ifade sonucu elde edilen veriler bir DataRows dizisine aktarılıyor ve kontollere bağlanamıyordu. Oysaki DataView sonuçlarını istediğiniz kontrole bağlamanız mümkündür.DataTable ile DataView arasında yer alan bir farkta, DataTable’ın sahip olduğu satırlar DataRow sınıfı ile temsil edilirken DataView nesnesinin sahip olduğu satırlar DataRowView sınıfı ile temsil edilirler. Bir DataTable nesnesine nasıl ki yeni satırlar ekleyebiliyor, silebiliyor ve primary key üzerinden arama yapabiliyorsak aynı işlemleri DataView nesnesi içinde yapabiliriz. Bunları AddNew, Delete ve Find yöntemleri ile yapabiliriz. Bir sonraki makalemizde bu metodlar ile ilgili geniş bir örnek daha yapacağız.

Bugünkü makalemizde konuya açıklık getirmesi açısından iki adet örnek yapacağız. Her iki örneğimizde ağırlıklı olarak DataView nesnesinin RowFilter özelliği üzerinde duracak. RowFilter özelliği DataTable sınıfının Select özelliğine çok benzer. Bir süzme ifadesi alır. Oluşturulan ifade içinde, kullanılacak alan (alanların) veri tiplerine göre bazı semboller kullanmamız gerekmektedir. Bunu açıklayan tablo aşağıda belirtilmiştir.

Veri Tipi
Kullanılan Karakter
Örnek

Tüm Metin Değerleri
' (Tek tırnak)
" Adi='Burak' "

Tarihsel Değerler
#
" DogumTarihi=#04.12.1976# "

Sayısal Değerler
Hiçbirşey
" SatisTutari>150000000"

Tablo 1. Veritipine göre kullanılacak özel karakterler

Diğer yandan RowFilter özelliğinde IN, Like gibi işleçler kullanarak çeşitli değişik sorgular elde edebiliriz.Mantıksal birleştiriciler yardımıyla (and,or...) birleşik ifadeler oluşturabiliriz. Aslında RowFilter özelliği sql’de kullandığımız ifadeler ile aynıdır. Örnekler verelim;

Kullanılan İşleç
Örnek
Ne Yapar?

IN
" PUAN IN (10,20,25) "
PUAN isimli alan 10, 20 veya 25 olan satırlar.

LIKE
" ADI LIKE 'A *' "
ADI A ile başlayanlar (* burada asteriks karakterimizdir.)

Tablo 2. İşleçler.

Şimdi gelin konuyu daha iyi anlayabilmek amacıyla örneklerimize geçelim. Konuyu anlayabilmek için iki farklı örnek yapacağız. İlk örneğimizde, aynı DataTable için farklı görünümler elde edip bunları kontrollere bağlayacağız. İlk örneğimizde, çeşitli ifadeler kullanıp değişik alt veri kümeleri alacağız. İşte kodlarımız,

```csharp
SqlConnection conFriends;
SqlDataAdapter da;
DataTable dtKitaplar; 
     /* Baglan metodumuz ile SqlConnection nesnemizi oluşturarak, sql sunucumuza ve Friends isimli veritabanımıza bağlanıyoruz. Daha sonra ise SqlDataAdapter nesnemiz vasıtasıyla Kitaplar isimli tablodan tüm verileri alıp DataTable nesnemize yüklüyoruz. */ 
public void Baglan()
{
     conFriends=new SqlConnection("data source=localhost;initial catalog=Friends;integrated security=sspi");
     da=new SqlDataAdapter("Select * From Kitaplar",conFriends);
     dtKitaplar=new DataTable("Kitap Listesi");
     da.Fill(dtKitaplar);
}
private void Form1_Load(object sender, System.EventArgs e)
{
     Baglan();
     DataView dvTum_Kitaplar=dtKitaplar.DefaultView; /* Bir DataTable nesnesi yaratıldığı zaman, standart olarak en az bir tane görünüme sahiptir. Bu varsayılan görünüm bir DataView nesnesi döndüren DefaultView metodu ile elde edilebilir. Çalıştırdığımız sql sorgusu Kitaplar tablosundaki tüm kayıtları aldığınıdan buradaki DefaultView'da aynı veri kümesini sahip olucak bir DataView nesnesi döndürür. Biz bu dönen veri kümesini dvTum_Kitaplar isimli DataView nesnesine aktardık. Daha sonra ise DataView nesnemizi dgTum_Kitaplar isimli dataGrid nesnemize bağladık.*/
     dgTum_Kitaplar.DataSource=dvTum_Kitaplar; 
     /* Yeni bir DataView nesnesini yapılandırıcısının değişik bir versiyonu ile oluşturuyoruz. Bu yapılandırıcı 4 adet parametre alıyor. İlk parametremiz dataTable nesnemiz, ikinci parametremiz RowFilter ifademiz ki burada Adi alanı B ile başlayanları buluyor, üçüncü parametremiz sıralamanın nasıl yapılacağı ki burada Adi alanında göre tersten sıralama yapıyor. Son parametre ise, DataViewRowState türünden bir parametre. Bu özellik DataView içerisinde ye alan her bir DataRowView'un (yani satırın) durumunun değerini belirtir. Alacağı değerler
      * 1. Added ( Sadece DataView'a eklenen satırları ifade eder)
      * 2. Deleted ( Sadece DataView'dan silinmiş satırları ifade eder)
      * 3. CurrentRows ( O an için geçerli tüm satırları ifade eder)
      * 4. ModifiedCurrent ( Değiştirilen satırların o anki değerlerini ifade eder)
      * 5. ModifiedOriginal ( Değiştirilen satırların orjinal değerlerini ifade eder)
      * 6. Unchanged ( Herhangibir değişikliğe uğramamış satırları ifade eder)
      * 7. OriginalRows ( Tüm satırların asıl değerlerini ifade eder)
      * 8. None (Herhangibir satır döndürmez)
     Buna göre bizim DataView nesnemiz güncel satırları döndürecektir. */ 
     DataView dvBIleBaslayan=new DataView(dtKitaplar,"Adi Like 'B*'","Adi Desc",DataViewRowState.CurrentRows);
     dgA.DataSource=dvBIleBaslayan; 
     /* Şimdi ise 2002 yılı ve sonrası Basım tarihine sahip verilerden oluşan bir DataView nesnesi oluşturuyoruz. Bu kez yapıcı metodumuz sadece DataTable nesnemizi parametre olarak aldı. Diğer ayarlamaları RowFilter,Sort özellikleri ile yaptık. Sort özelliğimiz sıralama kriterimizi belirliyor.*/
     DataView dv2002Sonrasi=new DataView(dtKitaplar);
     dv2002Sonrasi.RowFilter="BasimTarihi>=#1.1.2002#";
     dv2002Sonrasi.Sort="BasimTarihi Asc";
     /* Bu kez DataView nesnemizi bir ListBox kontrolüne bağladık ve sadece Adi alanı değerlerini göstermesi için ayarladık.*/
     lstPahali.DataSource=dv2002Sonrasi;
     lstPahali.DisplayMember="Adi";
} 
```

Çalışma sonucu ekran görüntümüz şekil 1’deki gibi olur.

![mk19_1.gif](/assets/images/2003/mk19_1.gif)

Şekil 1. İlk Programın Sonucu

Şimdi gelelim ikinci uygulamamıza. Bu uygulamamızda yine Kitaplar tablosunu ele alacağız. Bu kez RowFilter özelliğine vereceğimiz ifadeyi çalışma zamanında biz oluşturacağız. Alanımızı seçecek, sıralama kriterimizi belirleyecek,aranacak değeri gireceğiz. Girdiğimiz değerlere göre program kodumuz bir RowFilter Expression oluşturacak. Programın ekran tasarımını ben aşağıdaki gibi yaptım. Sizde buna benzer bir tasarım ile işe başlayın.

![mk19_2.gif](/assets/images/2003/mk19_2.gif)

Şekil2. Form Tasarımı

Şimdide kodlarımızı yazalım.

```csharp
SqlConnection conFriends;
SqlDataAdapter da;
DataTable dtKitaplar;
DataView dvKitaplar; 
/* Bu metod cmbAlanAdi isimli comboBox kontrolünü, dataTable nesnemizin bellekte temsil ettigi tablonun Alanlari ile doldurur. Nitekim bu alanlari, RowFilter özelliginde kullanacagiz. */
public void AlanDoldur()
{
     for(int i=0;i<dtKitaplar.Columns.Count;++i)
     {
          this.cmbAlanAdi.Items.Add(dtKitaplar.Columns[i].ColumnName.ToString());
          this.cmbAlanSira.Items.Add(dtKitaplar.Columns[i].ColumnName.ToString());
     }
}
private void Form1_Load(object sender, System.EventArgs e)
{
     conFriends=new SqlConnection("data source=localhost;initial catalog=Friends;integrated security=sspi");
     da=new SqlDataAdapter("Select Kategori,Adi,Yazar,BasimEvi,BasimTarihi,Fiyat From Kitaplar",conFriends);
     dtKitaplar=new DataTable("Kitap Listesi");
     /* DataTable nesnemizin bellekte temsil ettigi alani,Kitaplar tablosundaki veriler ile, SqlDataAdapter nesnemizin Fill metodu sayesinde dolduruyoruz.*/
     da.Fill(dtKitaplar); 
     dvKitaplar=new DataView(dtKitaplar); /* Dataview nesnemizi yaratiyoruz. Dikkat ederseniz yapici metod, paremetre olarak DataTable nesnemizi aliyor. Dolayisiyla DataView nesnemiz, dataTable içindeki veriler ile dolmus sekilde olusturuluyor.*/ 
     dataGrid1.DataSource=dvKitaplar; /* DataGrid kontrolümüze veri kaynagi olarak, DataView nesnemizi isaret ederek, DataView içindeki verileri göstermesini sagliyoruz.*/ 
     AlanDoldur();
}
     /* Bu butona bastigimizda, kullanıcının seçtigi alan, filtreleme kriteri ve filtreleme için kullanilacak deger verileri belirlenerek, DataView nesnesinin RowFilter metodu için bir syntax belirleniyor.*/
private void btnCreateFilter_Click(object sender, System.EventArgs e)
{
     string secilenAlan=cmbAlanAdi.Text;
     string secilenKriter=cmbKriter.Text;
     string deger=""; 
     /* If kosullu ifadelerinde, seçilen alanin veri tipine bakiyoruz. Nitekim RowFilter metodunda, alan'in veri tipine göre ifademiz degisiklik gösteriyor. Tarih tipindeki verilerde # karakteri aranan metnin basina ve sonuna gelirken, string tipinde degerlerde ' karakteri geliyor. Sayisal tipteki degerler için ise herhangibir karakter ifadenin aranan degerin basina veya sonuna eklenmiyor. */
     if(dtKitaplar.Columns[secilenAlan].DataType.ToString()=="System.String")
          deger="'"+txtDeger.Text+"'";
     if(dtKitaplar.Columns[secilenAlan].DataType.ToString()=="System.DateTime")
          deger="#"+txtDeger.Text+"#";
     if(dtKitaplar.Columns[secilenAlan].DataType.ToString()=="System.Decimal")
          deger=txtDeger.Text; 
     txtFilter.Text=secilenAlan+secilenKriter+deger; /* Olusturulan ifade görmemiz için textBox kontrolümüze yaziliyor. */
} 
private void btnFilter_Click(object sender, System.EventArgs e)
{
     dvKitaplar.RowFilter=txtFilter.Text; /* DataView nesnemizin RowFilter metoduna, ilgili ifademiz atanarak, süzme islemini gerçeklestirmis oluyoruz. */
     dvKitaplar.Sort=cmbAlanSira.Text+" "+cmbSiralamaKriteri.Text; /* Burada ise Sort özelligine siralama yapmak için gerekli veriler ataniyor. */
} 
```

Şimdi uygulamamızı çalıştıralım ve deneyelim.

![mk19_3.gif](/assets/images/2003/mk19_3.gif)

Şekil 3. Programın Çalışması

Örneğin ben, Fiyatı 10 milyon TL’ sının üstünde olan kitapların listesini Adlarına göre z den a ya sıralanmış bir şekilde elde ettim.Değerli okurlarım geldik bir makalemizin daha sonuna. DataView nesnesinin özellikle RowFilter tekniğine ilişkin olarak yazdığımız bu makale ile inanıyorum ki yeni fikirler ile donanmışsınızdır. Hepinize mutlu günler dilerim.