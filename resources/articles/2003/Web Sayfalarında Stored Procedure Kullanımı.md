---
title: "Web Sayfalarında Stored Procedure Kullanımı"
pubDate: 2003-11-12 12:00:00
categories:
  - Ado.Net
tags:
  - Ado.Net
  - C#
  - SQL Server
  - HTTP
  - Performance
---

# Web Sayfalarında Stored Procedure Kullanımı
Bugünkü makalemde sizlere bir Web Sayfası üzerinde, bir tablonun belli bir satırına ait detaylı bilgilerin, bir Stored Procedure yardımıyla nasıl gösterileceğini anlatmaya çalışacağım. Uygulamamızda örnek olması açısından, Kitap bilgileri barındıran bir Sql tablosu kullanacağım. Tablomuzun yapısını aşağıdaki Şekil 1’ de görebilirsiniz. Temel olarak, kitap isimlerini, kitapların kategorilerini, yazar isimlerini, basım evi bilgilerini vb… barındıran bir tablomuz var. Bu tablonun örnek verilerini de Şekil2’ de görebilirsiniz.

![mk3_1.gif](images/mk3_1.gif)

Şekil 1. Kitaplar tablosunun alan yapısı.

![mk3_2.gif](images/mk3_2.gif)

Şekil 2. Kitaplar tablosunun örnek verileri.

Şimdi projemizin en önemli unsuru olan Stored Procedure nesnemizi Sql Server üzerinde oluşturalım. Bu Stored Procedure ile kullanıcının, web sayfasında listbox nesnesi içinden seçtiği kitaba ait tüm verileri döndürecek olan bir Sql cümleciği yazıcağız. Burada aranan satırı belirleyecek olan değerimiz ID isimli aynı zamanda Primary Key olan alanın değeri olucaktır. Kullanıcı listBox nesnesinde yer alan bir kitabı seçtiğinde (yani listBox nesnesine ait lstKitaplar_SelectedIndexChanged olay procedure’ü çalışıtırıldığında) seçili olan öğeye ait id numarası Stored Procedure’ümüze parametre olarak gönderilicek. Elde edilen sonuç kümesine ait alanlar dataGrid nesnemizde gösterilerek kitabımıza ait detaylı bilgilerin görüntülenmesi sağlanmış olucak. Dilerseniz “Kitap Bul” isimli Stored Procedure’ümüzü oluşturarak devam edelim. Şekil 3 yazdığımız Stored Procedure nesnesini göstermekte.

![mk3_3.gif](images/mk3_3.gif)

Şekil 3. Stored Procedure nesnemiz ve Sql ifadesi.

Sıra geldi uygulamamızda yer alan WebForm’umuzu oluşturmaya. Uygulamamızı C# dili ile oluşturmayı tercih ettiğimden New Project kısmında Visual C# Proejct bölümünü seçtim. Dikkat edicek olursanız, uygulmamız bir Web Application dır. Oluşturulduğu yer http://localhost/kitap adlı adrestir.

![mk3_4.gif](images/mk3_4.gif)

Şekil 4. Web Application

Evet gelelim WebFormun tasrımına. Ben aşağıdaki gibi bir tasarım oluşturdum. Sizlerde buna yakın bir tasarım oluşturabilirsiniz veya aynısını kullanamayı tercih edebilirsiniz.

![mk3_6.gif](images/mk3_6.gif)

Şekil 5. Web Form tasarımı.

Sıra geldi kodlarımızı yazmaya. Önce sayfa yüklenirken neler olucağını belirleyeceğimiz kodlarımızı yazmaya başlayalım. Özet olarak Page_Load olay procedure’ünde Sql Server ‘ a bağlanıp, Kitaplar tablosundan yanlızca ID ve Adi alanına ait değerleri alıyoruz ve bunları bir SqlDataReader nesnesi vasıtasıyla, lstKitaplar isimli listBox nesnemize yüklüyoruz. Gelin kodumuzu yazalım, hem de inceleyelim.

```csharp
/* Önce gerekli SqlConnection nesnemizi oluşturuyor ve gerekli ayarlarımızı yapıyoruz.*/

SqlConnection conFriends=new SqlConnection("initial catalog=Friends;Data Source=localhost;integrated security=sspi");
private void Page_Load(object sender, System.EventArgs e)
{
     if (Page.IsPostBack==false)
     {
          /* SqlCommand nesnemizi yaratıyoruz. Bu nesne Select sorgusu ile Kitaplar tablosundan ID ve Adi alanlarının değerlerini alıcak. Alınan bu veri kümesi Adi alanına göre A'dan Z'ye sıralanmış olucak. Bunu sağlayan sql cümleciğindeki, "Order By Adi" ifadesidir. Tersten sıralamak istersek "Order By Adi Asc" yazmamız gerekir. */

          SqlCommand cmdKitaplar=new SqlCommand("Select ID,Adi From Kitaplar Order By Adi",conFriends);

          cmdKitaplar.CommandType=CommandType.Text; // SqlCommand'in command String'inin bir Sql cümleciğine işaret ettiğini belirtiyoruz.

          SqlDataReader dr; // Bir SqlDataReader nesnesi olşuturuyoruz.  

          /* SqlDataReader nesnesi ileri yönlü ve sadece okunabilir bir veri akışı sağlar. (Forward and Readonly) Bu da nesneden veri aktarımlarının (örneğin bir listbox’a veya datagrid’e) hızlı çalışmasına bir nedendir. Uygulamalarımızda, listeleme gibi sadece verilere bakmak amacıyla çalıştıracağımız sorgulamalar için, SqlDataReader nesnesini kullanmak, performans açısından olumlu etkiler yapar. Ancak SqlDataReader nesnesi çalıştığı süre boyunca sunucuya olan bağlantınında sürekli olarak açık olmasını gerektirir.

          Yukarıdaki kod satırında dikkat çekici diğer bir unsur ise, bir new yapılandırıcısı kullanılmayışıdır. SqlDataReader sınıfının bir yapıcı metodu ( Constructor ) bulunmamaktadır. O nedenle bir değişken tanımlanıyormuş gibi bildirilir. Bu nesneyi asıl yükleyen, SqlCommand nesnesinin ExecuteReader metodudur. */ 

          conFriends.Open(); // Bağlantımızı açıyoruz. 

          dr=cmdKitaplar.ExecuteReader(CommandBehavior.CloseConnection); /* Burada ExecuteReader metodu , SqlCommand nesnesine şöyle bir seslenişte bulunuyor. " SqlCommand'cığım, sana verilen Sql Cümleciğini (Select sorgusu) çalıştır ve sonuçlarını bir zahmet eşitliğin sol tarafında yer alan SqlDataReader nesnesinin bellekte referans ettiği alana yükle. Sonrada sana belirttiğim, sağımdaki CommandBehavior.CloseConnection parametresi nedeni ile, SqlDataReader nesnesi Close metodu ile kapatıldığında, yine bir zahmet SqlConnection nesnesininde otomatik olarak kapanmasını sağlayıver". */ 

          lstKitaplar.DataSource=dr; // Elde edilen veri kümesi SqlDataReader nesnesi yardımıyla ListBox nesnesine veri kaynağı olarak gösteriliyor. 

          lstKitaplar.DataTextField="Adi"; // ListBox nesnesinde Text olarak Adi alanının değerleri görünücek. 

          lstKitaplar.DataValueField="ID"; // Görünen Adi değerlerinin sahip olduğu ID değerleri de ValueField olarak belirleniyor. Böylece "Kitap Bul" isimli Stored Procedure'ümüze ID parametresinin değeri olarak bu alanın değeri gitmiş olucak. Kısacası görünen yazı kitabın adı olurken, bu yazının değeri ID alanının değeri olmuş oluyor. 

          lstKitaplar.DataBind(); // Web Sayfalarında , verileri nesnelere bağlarken DataBind metodu kullanılır. 

          dr.Close(); // SqlDataReader nesnemiz kapatılıyor. Tabiki SqlConnection nesnemizde otomatik olarak kapatılıyor.
     }
}
```

Şimdi oluşturduğumuz projeyi çalıştırırsak aşağıdaki gibi bir sonuç elde ederiz. Görüldüğü gibi Kitaplar tablosundaki tüm kitaplara ait Adi alanlarının değerleri listBox nesnemize yüklenmiştir.

![mk3_7.gif](images/mk3_7.gif)

Şekil 6. PageLoad sonrası.

Şimdi ise listBox’ta bir öğeyi seçtiğimizde neler olucağına bakalım. Temel olarak, seçilen öğeye ait ID değeri “Kitap Bul” isimli Stored Procedure’e gidicek ve dönen sonuçları dataGrid nesnesinde gösterceğiz.

ListBox nesnesine tıklandığı zaman, çalışıcak olan lstKitaplar_ SelectedIndexChanged olay procedure’ünde gerekli kodları yazmadan once ListBox nesnesinin AutoPostBack özelliğine True değerini atamamız gerekiyor. Böylece kullanıcı sayfa üzerinde listbox içindeki bir nesneye tıkladığında lstKitaplar_SelectedIndexChanged olay procedure’ünün çalışmasını sağlamış oluyoruz. Nevarki böyle bir durumda sayfanın Page_Load olay procedürünün de tekrar çalışmasını engellemek yada başka bir deyişle bir kere çalışmasını garantilemek için if (Page.IsPostBack==false) kontrolünü Page_Load olay procedure’üne ekliyoruz.

![mk3_9.gif](images/mk3_9.gif)

Şekil 7. AutoPostBack özelliği

```csharp
private void lstKitaplar_SelectedIndexChanged(object sender, System.EventArgs e)
{
     SqlCommand cmdKitapBul=new SqlCommand("Kitap Bul",conFriends); /* SqlCommand nesnemizi oluşturuyoruz ve commandString parametresine Stored Procedure'ün ismini yazıyoruz.*/ 
     cmdKitapBul.CommandType=CommandType.StoredProcedure; /* Bu kez SqlCommand'in bir Stored Procedure çalıştıracağına işaret ediyoruz.*/ 

     cmdKitapBul.Parameters.Add("@id",SqlDbType.Int); /* "Kitap Bul" isimli Stored Procedure'de yer alan @id isimli parametreyi komut nesnemize bildirmek için SqlCommand nesnemizin, Parameters koleksiyonuna ekliyoruz.*/ 

     cmdKitapBul.Parameters[0].Value=lstKitaplar.SelectedValue; /* listBox nesnesinde seçilen öğenin değerini (ki bu değer ID değeridir) SelectedValue özelliği ile alıyor ve SqlCommand nesnesinin 0 indexli parametresi olan @id SqlParameter nesnesine atıyoruz. Artık SqlCommand nesnemizi çalıştırdığımızda , @id paramteresinin değeri olarak seçili listBox öğesinin değeri gönderilicek ve bu değere göre çalışan Select sorgusunun döndürdüğü sonuçlar SqlDataReader nesnemize yüklenecek.*/

     SqlDataReader dr; 
     conFriends.Open(); // Bağlantımız açılıyor.      dr=cmdKitapBul.ExecuteReader(CommandBehavior.CloseConnection); // Komut çalıştırılıyor. 
     dgDetaylar.DataSource=dr; // dataGrid nesnesine veri kaynağı olarak SqlDataReader nesnemiz gösteriliyor. 
     dgDetaylar.DataBind(); // dataGrid verilere bağlanıyor. 
     dr.Close(); // SqlDataReader nesnemiz ve sonrada SqlConnection nesnemiz ( otomatik olarak ) kapatılıyor.
}
```

![mk3_10.gif](images/mk3_10.gif)

Şekil 8. Sonuç.

Geldik bir makalemizin daha sonuna. Yeni makalelerimizde görüşmek dileğiyle, hepinizi mutlu günler.
