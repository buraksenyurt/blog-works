---
title: "İlişkili Tabloları DataSet İle Kullanmak - 1"
pubDate: 2003-12-09 10:00:00
categories:
  - Ado.Net
tags:
  - Ado.Net
  - C#
  - .NET
  - Visual Studio
  - DataSet
  - DataTable
---

# İlişkili Tabloları DataSet İle Kullanmak - 1
Değerli Okurlarım, Merhabalar.

Bugünkü makalemizde, aralarında relationship (ilişki) bulunan tabloların, bir DataSet nesnesinin bellekte temsil ettiği alanda nasıl saklandığını incelemeye çalışıcacağız. Bunu yaparken de, geliştireceğimiz uygulama ile parant-child (ebeveyn-çocuk) yada master-detail (efendi-detay) adı verilen ilişkileri taşıyan tablolarımızı bir windows application’da bir dataGrid nesnesi ile nasıl kolayca göstereceğimizi göreceğiz.

İşin sırrı Olin’de diye bir reklam vardı eskiden. Şimdi aklıma o reklam geldi. Burada da işin sırrı DataRelation adı verilen sınıftadır. DataRelation sınıfına ait nesneler, aralarında ilişkisel bağ olan tablolarının, aralarındaki ilişkiyi temsil ederler. Bir DataRelation nesnesi kullandığımızda, bu nesneyi mutlaka bir DataSet sınıfı nesnesine eklememiz gerekmektedir. Dolayısıyla DataSet sınıfımız, aralarında ilişki olan tabloları temsil eden DataTable nesnelerini ve bu tablolar arasındaki ilişkiyi temsil eden DataRelation nesnesini (lerini) taşımak durumundadır.

Aşağıdaki şekil ile, bu konuyu zihnimizde daha kolay canlandırabiliriz. Söz konusu tablolar, yazacağımız uygulamayada da kullanacağımız tablolardır. Dikkat edilecek olursa buradaki iki tablo arasında Siparis isimli tablodan, Sepet isimli tabloya bire-çok (one to many) bir ilişki söz konusudur. DataRelation nesnemiz bu ilişkiyi DataSet içinde temsil etmektedir.

![mk16_1.gif](images/mk16_1.gif)

Şekil 1. DataRelation

Bir DataRelation nesnesi oluşturmak için kullanabileceğimiz Constructor metodlar şunlardır.

1 - public DataRelation (string, DataColumn, DataColumn);

2 - public DataRelation (string, DataColumn[], DataColumn[]);

3 - public DataRelation (string, DataColumn, DataColumn, bool);

4 -public DataRelation (string, DataColumn[], DataColumn[], bool);

Tüm yapıcı metodlar ilk parametre olarak DataRelation için string türde bir isim alırlar. İl yapıcı metodumuz, iki adet DataColumn tipinde parametre almaktadır. İlk parametre master tabloya ati primary key alanını, ikinci DataColumn parametresi ise detail tabloya ait secondary key alanını temsil etmektedir. İkinci yapıcı metodu ise aralarındaki ilişkiler birden fazla tablo alanına bağlı olan tablo ilişkilerini tanımlamak içindir. Dikkat edilicek olursa, DataColumn[] dizileri söz konusudur.Üçüncü ve dördüncü yapıcılarında kullanım tarzaları bir ve ikinci yapıcılar ile benzer olmasına karşın aldıkları bool tipinde dördüncü bir parametre daha vardır. Dördüncü parametre, tablolar arası kullanılacak veri bütünlüğü kuralları uygulanacak ise True değerini alır eğer bu kurallar uygulanmayacak ise false değeri verilir.

Şimdi gelin kısa bir uygulama ile bu konuyu işleyelim. Uygulamamızda kullanılan tablolara ait alanlar ve özellikleri şöyledir. İlk tablomuz Siparis isimli tablomuz. Bu tabloda kullanıcının vermiş olduğu siparişin numarası ve tarihi ile ilgili bilgiler tutuluyor. Bu tablo bizim parent (master) tablomuzdur.

![mk16_2.gif](images/mk16_2.gif)

Şekil 2. Siparis Tablosu

Diğer tablomuzda ise, verilen siparişin hangi ürünlerden oluştuğuna dair bilgiler yer almakta.Bu tablomuz ise bizim child tablomuzdur.

![mk16_3.gif](images/mk16_3.gif)

Şekil 3. Sepet Tablosu

Uygulamamızı bir windows application olarak geliştireceğim. Bu nedenle vs.net ortamında, yeni bir windows application oluşturuyoruz. Sayfanın tasarımı son derece basit. Bir adet dataGrid nesnemiz var ve bu nesnemiz ilişkil tabloların kayıtlarını gösterecek. Dilerseniz kodlarımızı yazmaya başlayalım.

```csharp
private void Form1_Load(object sender, System.EventArgs e)
{
     /* Önce sql sunucumuzda yer alan Friends isimli veritabanımız için bir bağlantı nesnesi oluşturuyoruz. */

     SqlConnection conFriends=new SqlConnection("data source=localhost;initial catalog=Friends;integrated security=sspi"); 
     /* SqlDataAdapter nesneleri yardımıyla, Friends veritabanında yer alan Siparis ve Sepet tablolarındaki verileri alıyoruz ve sonrada bunları DataTable nesnelerimize aktarıyoruz.*/

     SqlDataAdapter daSiparis=new SqlDataAdapter("Select * From Siparis",conFriends);
     SqlDataAdapter daSepet=new SqlDataAdapter("Select * From Sepet",conFriends);
     DataTable dtSiparis=new DataTable("Siparisler");
     DataTable dtSepet=new DataTable("SiparisDetaylari"); 
     daSiparis.Fill(dtSiparis);
     daSepet.Fill(dtSepet); 
     /* Şimdi ise bu iki tablo arasındaki bire çok ilişkiyi temsil edecek DataRelation nesmemizi oluşturuyoruz. */ 

     DataRelation drSiparisToSepet=new DataRelation("Siparis_To_Sepet",dtSiparis.Columns["SiparisID"],dtSepet.Columns["SiparisID"]);
     /* Artık oluşturduğumuz bu DataTable nesnelerini ve DataRelation nesnemizi DataSet nesnemize ekleyebiliriz. Dikkat edicek olursanız, DataRelation nesnemizi dataSet nesnemizin Relations koleksiyonuna ekledik. DataRelation nesneleri DataTable nesneleri gibi DataSet'e ait ilgili koleksiyonlarda tutulmaktadırlar. Dolayısıyla bir DataSet'e birden fazla tabloyu nasıl ekleyebiliyorsak birden fazla ilişkiyide ekleyebiliriz. */

     DataSet ds=new DataSet();
     ds.Tables.Add(dtSiparis);
     ds.Tables.Add(dtSepet);
     ds.Relations.Add(drSiparisToSepet);
     /* Şimdi ise dataGrid nesnemizi dataSet nesnemiz ile ilişkilendirelim */

     dataGrid1.DataSource=ds.Tables["Siparisler"];
}  
```

Uygulamamızı çalıştrıdığımızda aşağıdaki ekran görüntüsünü elde ederiz. Görüldüğü gibi Siparis tablosundaki veriler görünmektedir. Lütfen satırların yanlarındaki + işaretine dikkat edelim.(Şekil 4) Bu artı işaretine tıkladığımızda oluşturmuş olduğumuz DataRelation’ın adını görürüz.(Şekil 5)

![mk16_4.gif](images/mk16_4.gif)

Şekil 4.

![mk16_5.gif](images/mk16_5.gif)

Şekil 5.

Bu isimler birer link içermektedir. Bu linklerden birine tıkladığımızda bu satıra ait detaylı bilgiler child tablodan (Sepet) gösterilirler. (Şekil 6)

![mk16_6.gif](images/mk16_6.gif)

Şekil 6.

Bir sonraki makalemizde tablolar arasındaki veri bütünlüğünü sağlayan Constraint kurallarının nasıl DataSet’e aktarıldığını inceleyeceğiz. Geldik bir makalemizin daha sonuna. Hepinize mutlu günler dilerim.
