---
title: "DataAdapter Kavramı ve OleDbDataAdapter Sınıfına Giriş"
pubDate: 2004-02-27 08:00:00
categories:
  - Ado.Net
tags:
  - Ado.Net
  - C#
  - .NET
  - Oracle
  - Performance
  - Visual Studio
  - DataSet
  - DataTable
---

# DataAdapter Kavramı ve OleDbDataAdapter Sınıfına Giriş
Değerli Okurlarım, Merhabalar.

Bu makalemizde, Ado.Net'in en çok kullanılan kavramlarından birisi olan DataAdapter kavramını incelemeye çalışacak ve OleDbDataAdapter sınıfına kısa bir giriş yapacağız. Pek çok programcı, veritabanı uygulamaları geliştirirken, kontrol ve performansa büyük önem verir. Ancak aynı zamanda bu kazanımlara kolay yollar ile ulaşmak isterler. Ado.Net modelinde, bağlantısız katman ile bağlantılı katman arasındaki iletişim ve veri alışverişinin, kontrol edilebilir, performanslı ve aynı zamanda kolay geliştirilir olmasında DataAdapter kavramının yeri yadırganamıyacak kadar fazladır. Temel tanım olarak, DataAdapter sınıfları, sahip oldukları veri sağlayıcılarının izin verdiği veri kaynakları ile, sistem belleği üzerinde yer alan bağlantısız katman nesneleri arasındaki veri alışverişinin kolay, güçlü ve verimli bir şekilde sağlanmasından sorumludurlar. Bu tanımdan yola çıkarak, DataAdapter sınıflarının, veri kaynağından verilerin alınıp, bağlantısız katman nesneleri olan DataSet ve DataTable nesnelerine doldurulmasından sorumlu olduğunu; ayrıca, bağlantısız katman nesnelerinin taşıdığı verilerdeki değişikliklerinde veri kaynağına yansıtılmasından sorumlu olduğunu söyleyebiliriz. İşte bu, DataAdapter sınıfının rolünü tam olarak açıklayan bir tanımlamadır.

Veritabanı uygulamalarında en önemli unsurların, verinin taşınması, izlenebilmesi, üzerinde değişikliklerin yapılması ve tekrar veri kaynağına yansıtılması olduğunu söyleyebiliriz. DataAdapter sınıflarının bu unsurların gerçekleştirilmesinde çok önemli rol oynadıkları bir gerçektir. Aşağıdaki şekil DataAdapter sınıflarının işleyişini daha iyi anlamamıza yardımcı olucaktır.

![mk57_1.gif](images/mk57_1.gif)

Şekil 1. DataAdapter Sınıfının Rolü.

Ado.net modeli,.net Framework'ün en son sürümünde, OleDb veri kaynakları için OleDbDataAdapter, Sql veri kaynakları için SqlDataAdapter, Oracle veri kaynakları için OracleDataAdapter ve Odbc veri kaynakları içinde OdbcDataAdapter sınıflarına sahiptir. DataAdapter sınıflarının yapısı aşağıdaki gibidir.

![mk57_2.gif](images/mk57_2.gif)

Şekil 2. DataAdapter sınıflarının genel yapısı.

Şimdi dilerseniz bu üyelerin hangi amaçlar için kullanıldığına kısaca değinelim. Her DataAdapter sınıfı mutlaka bir SelectCommand sorgusu içermek zorundadır. Nitekim, bir DataAdapter nesnesinin yaratılış amacı, veri kaynağından çekilen verileri bağlantısız katmandaki ilgili nesnelere aktarmaktır. İşte bunun için, SelectCommand özelliği geçerli bir sorguya sahip olmalıdır. Veri almak için kullanılan sorgu bir Command nesnesine işaret etmektedir. Çoğu zaman bir DataAdapter oluştururken bu sınıfın kurucusu içinde, select sorgularını string olarak gireriz. Bu string aslında sonradan bir Command nesnesi haline gelerek, DataAdapter sınıfının SelectCommand özelliğine atanır.

Diğer yandan, verileri yüklü olan bir DataSet içerisinde yapılan değişikliklerin veri kaynağına tekrardan aktarılması için, yapılan değişikliğin çeşidine uygun komutların DataAdapter tarafından oluşturulması gerekmektedir. Örneğin, DataSet üzerinde yeni satırlar girilmiş ise, bunlar InsertCommand özelliğinin sahip olduğu sql metni ile veri kaynağına aktarılır. Aynı şekilde, satır silme işlemleri için DeleteCommand özelliğindeki sql söz dizimi, güncelleme işlemleri için ise UpdateCommand özelliğindeki sql ifadeleri kullanılır. Bu özelliklerin tümünü elle oluşturabilieceğiniz gibi, Visual Studio.Net ortamında yada CommandBuilder sınıfını vasıtasıyla otomatik olarak oluşturulmasını sağlayabilirsiniz. Bu özellikleri sonraki makelelerimizde incelemeye çalışacağız.

TableMappings DataTableMappingCollection türünden bir koleksiyondur. Görevi ise oldukça ilginçtir. Bir DataAdapter nesnesi ile veri kaynağındaki bir tabloyu, DataSet'aktardığımızda, aktarma ve güncelleme işlemleri sırasında alan isimlerinin her iki katmandada eşleştirilebilmesi için kullanılır. Nitekim, DataAdapter ile bir tabloyu elde ettiğimizde bu tablo, Table ismi ile işleme sokulur. Çünkü OleDbDataAdapter tablonun ismi ile ilgilenmez. Fakat biz dilersek TableMappings koleksiyonuna bir DataTableMapping nesnesi ekleyerek tablo adının daha uygun bir isme işaret etmesini sağlayabiliriz. Aynı konu, sütun adları içinde geçerlidir. Bazen tablodaki sütun adları istediğimiz isimlerde olmayabilir. İşte bu durumda, DataTableMapping nesnesinin ColumnMappings koleksiyonu kullanılır. DataSet içindeki her tablo DataColumnMapping türünden nesneler içeren bir ColumnMappings koleksiyonuna sahiptir. Bu nesnelerin her biri veri kaynağındaki tabloda yer alan sütun adlarının, DataSet içindeki tabloda hangi isimlere karşılık geldiğinin belirlenmesinde kullanılır. Bu konu şu an için karşışık görünmüş olabilir ancak ilerleyen örneklerimizde bu konuya tekrar değinecek, daha anlaşılır olması için çalışacağız.

DataAdapter sınıflarının genel özelliklerine değindikten sonra dilerseniz OleDbDataAdapter sınıfımızı incelemeye başlayalım. OleDbDataAdapter sınıfı System.Data.OleDb isim uzayı içinde yer almaktadır. Bir OleDbDataAdapter nesnesi yaratmak için kullanabileceğimiz 4 adet aşırı yüklenmiş yapıcı metod bulunmaktadır. Bunlar aşağıdaki tabloda belirtilmiştir.

Yapıcı Metod Prototipi
Açıklaması

public OleDbDataAdapter (string, string);
Select sorgusunu ve bağlantı için gerekli söz dizimini metin şeklinde parametre olarak alır.

public OleDbDataAdapter (string, OleDbConnection);
Sorguyu metin bazında alırken, bağlantı için önceden oluşturulmuş bir OleDbConnection nesnesini parametre olarak alır.

public OleDbDataAdapter (OleDbCommand);
Select sorgusunu ve geçerli bir bağlantıyı işaret eden bir OleDbCommand nesnesini parametre olarak alır.

public OleDbDataAdapter ();
Böyle oluşturulan bir OleDbDataAdapter'ı kullanabilmek için, ilgili özellikler (SelectCommand gibi.) sonradan ayarlanır.

Tablo 1. OleDbDataAdapter sınıfının yapıcı metodları.

Şimdi burada önemli olan bir iki noktayı vurgulamamız gerekiyor. Herşeyden önce bir OleDbDataAdapter nesnesinin kullanılma amacı, bağlantısız katman nesnesini veri kaynağındaki veriler ile doldurmaktır. Bu her DataAdapter sınıfı içinde öncelikli hedeftir. Bu nedenle her DataAdapter nesnesinin mutlaka sahip olması gereken bir select sorgu ifadesi dolayısıyla var olan bir SelectCommand özelliğine ihtiyacı vardır. Ayrıca diğer önemli gereklilik, geçerli bir bağlantının yani bir Connection nesnesinin olmasıdır. Şimdi dilerseniz, örnekler ile OleDbDataAdapter nesnelerinin nasıl oluşturulduğunu ve verilerin, veri kaynağından bağlantısız katman nesnelerine nasıl çekildiğini görelim.

İlk örneğimizde, bir windows uygulamasındaki DataGrid nesnemizi, bağlantısız katmanda çalışacak DataSet nesnemize bağlıyacağız. DataSet nesnemizi veriler ile doldurmak için, OleDbDataAdapter nesnemizi kullanacağız. İşte uygulamamızın kısa kodları.

```csharp
private void btnDoldur_Click(object sender, System.EventArgs e)
{
    OleDbConnection conFriends=new OleDbConnection("Provider=SQLOLEDB;Data Source=localhost;initial catalog=Friends;integrated security=SSPI");
    string sqlIfadesi="Select * From Makale";
    OleDbDataAdapter daFriends=new OleDbDataAdapter(sqlIfadesi,conFriends);
    DataSet ds=new DataSet();
    daFriends.Fill(ds);
    dgMakale.DataSource=ds;
}
```

Kodlarımızı inceleyecek olursak; öncelikle OleDbDataAdapter nesnemiz için gerekli ve olmassa olmaz üyeleri tanımladık. Öncelikle geçerli bir bağlantı hattımızın olması gerekiyor. Bu amaçla bir OleDbConnection nesnesi kullanıyoruz. Diğer yandan, OleDbDataAdapter nesnesinin oluşturduğumuz DataSet nesnesini doldurabilmesi için, veri kaynağından veri çekebileceği bir sql ifadesine ihtiyacımız var. Bu amaçlada bir sql cümleciğini string olarak oluşturuyoruz. Sonraki adımda ise OleDbDataAdapter nesnemizi yaratıyoruz. Burada kullanılan yapıcı metod, sql ifadesini string olarak alıyor ve birde bağlantı hattını temsil edicek olan OleDbConnection nesnesini parametre olarak alıyor. Daha sonra, veri kaynağından bu sql ifadesi ile çekilecek verilerin bellekte tutulduğu bölgeyi referans edicek DataSet nesnemiz oluşturuluyor. Burada Fill metodu, sql ifadesini, geçerli bağlantı hattı üzerinde çalıştırıyor ve elde edilen veri kümesini, metod parametresi olarak aldığı DataSet nesnesinin bellekte gösterdiği adrese yerleştiriyor. Uyglamamızı çalıştırdığımızda aşağıdaki sonucu elde ederiz.

![mk57_3.gif](images/mk57_3.gif)

Şekil 3. Fill Metodu ile DataSet'in doldurulması.

OleDbDataAdapter nesnesinin Fill metodunu çağırdığımızda, nesne, parametre olarak aldığı bağlantıyı otomatik olarak açmaktadır. Yani Fill metodunu çağırmadan önce bağlantı nesnemizi Open metodu ile açmamıza gerek yoktur. Fill metodu ile DataSet nesnesi doldurulduktan sonra ise, OleDbDataAdapter, açık olan bağlantıyı otomatik olarak kapatacaktır. Bu kolaylık birden fazla tabloyu bir DataSet içerisine farklı OleDbDataAdapter nesneleri ile alacağımız zaman dezavantajdır. Nitekim her bir OleDbDataAdapter nesnesi eğer aynı bağlantıyı kullanıyorlarsa, her defasında veri kaynağına olan bağlantıyı açıcak ve kapatıcaklardır. Söz gelimi aşağıdaki kodları göz önüne alalım.

```csharp
private void btnDoldur_Click(object sender, System.EventArgs e)
{
    OleDbConnection conFriends=new OleDbConnection("Provider=SQLOLEDB;Data Source=localhost;initial catalog=Friends;integrated security=SSPI");

    string sqlIfadesi1="Select * From Makale";
    string sqlIfadesi2="Select * From Kitap";

    OleDbDataAdapter daMakale=new OleDbDataAdapter(sqlIfadesi1,conFriends);
    OleDbDataAdapter daKitap=new OleDbDataAdapter(sqlIfadesi2,conFriends);

    DataSet ds=new DataSet();
    daMakale.Fill(ds);
    daKitap.Fill(ds);
    dgMakale.DataSource=ds;
}
```

Bu uygulamada iki OleDbDataAdapter nesnesi aynı bağlantıyı kullanarak farklı veri tablolarını aynı DataSet içerisine yüklemiştir. Her bir Fill metodu çağırıldığında bağlantı açılır, tablodan veriler, sql ifadeleri gereği alınır, DataSet nesnesinin referans ettiği bellek bölgesine yüklenir ve açık olan bağlantı kapatılır. Bu durumda, uygulamamızdaki kodlarda bağlantının iki kere açılıp kapatıldığını söyleyebiliriz. Bu gözle görülür bir performans kaybına yol açmıyabilir ancak programlama tekniği açısından bağlantının bu kadar kısa süreler için tekrardan açılıp kapatılması sistem kaynaklarını boş yere kullanmak manasınada gelmektedir. Peki ne yapılabilir? OleDbDataAdapter sınıfının bir özelliği, eğer kullanılan bağlantı açık ise, OleDbDataAdapter bu bağlantıyı biz kapatana kadar kapatmayacak olmasıdır. Yani yukarıdaki kodu şu şekilde düzenlersek istediğimiz sonuca ulaşabiliriz.

```csharp
private void btnDoldur_Click(object sender, System.EventArgs e)
{
    OleDbConnection conFriends=new OleDbConnection("Provider=SQLOLEDB;Data Source=localhost;initial catalog=Friends;integrated security=SSPI");

    string sqlIfadesi1="Select * From Makale";
    string sqlIfadesi2="Select * From Kitap";

    OleDbDataAdapter daMakale=new OleDbDataAdapter(sqlIfadesi1,conFriends);
    OleDbDataAdapter daKitap=new OleDbDataAdapter(sqlIfadesi2,conFriends);

    DataSet ds=new DataSet();

    conFriends.Open();

    daMakale.Fill(ds);
    daKitap.Fill(ds);

    dgMakale.DataSource=ds;

    conFriends.Close();
}
```

Bu durumda, Fill metodları bağlantı durumunu gözleyecek ve eğer bağlantı açık ise herhangibir müdahalede bulunmadan bu açık bağlantı üzerinden işlemleri gerçekleştirecektir. Fill metodu işi bittiğinde, kullandığı bu açık bağlantıyı kapatmayacaktır. Bu sayede ardından gelen ve aynı bağlantıyı kullanan OleDbDataAdapter nesnesi, yeni bir bağlantı açmaya gerek duymayacak, halen açık olan bağlantıyı kullanacaktır. Burada unutulmaması gereken nokta, bağlantı nesnemiz ile işimiz bittiğinde bu nesneyi Close metodu ile kapatmaktır. Şimdi gelelim Fill metodundaki başka bir noktaya. Yukarıdaki son örneğimizi çalıştırıcak olursak aşağıdaki görüntüyü elde ederiz.

![mk57_4.gif](images/mk57_4.gif)

Şekil 4. İki tabloda aynı isim altında yüklenir.

Görüldüğü gibi iki tablomuzda DataSet'e tek bir tablo ismi altında yüklenmiştir. Bu isme tıkladığımızda, ilk önce Makale verilerinin göründüğünü ardından Kitap tablosuna ait verilerin göründüğünü anlarız.

![mk57_5.gif](images/mk57_5.gif)

Şekil 5. Her iki tabloya ait verilerin görünümü.

Bu elbette istemediğimiz bir durumdur. Bunu düzeltmek için, Fill metodunun aşağıdaki prototipi verilen aşırı yüklenmiş halini kullanırız.

```csharp
public int Fill(DataSet dataSet,string srcTable);
```

Burada ikinci parametre aktarılan tablo için bir ismi string olarak almaktadır. Böylece, DataSet içerisine aktarılan tabloları isimlendirebiliriz. Nitekim OleDbDataAdapter sınıfı, Fill metodu ile tablolardaki verileri DataSet içine alırken, sadece alan adlarını eşleştirmek için alır. Tablo adları ile ilgilenmez. Bu nedenle bir tablo ismi belirtmessek, bu DataSet içerisine Table ismi ile alınacaktır. Biz Fill metoduna bir tablo ismini parametre olarak verdiğimizde, DataAdapter sınıfının TableMappings koleksiyonu, DataSet içinde bizim verdiğimiz tablo ismini, veri kaynağındaki ile eşleştirir. Dolayısıyla yukarıdaki kodları aşağıdaki gibi düzenlersek sonuç istediğimiz gibi olucaktır.

```csharp
private void btnDoldur_Click(object sender, System.EventArgs e)
{
    OleDbConnection conFriends=new OleDbConnection("Provider=SQLOLEDB;Data Source=localhost;initial catalog=Friends;integrated security=SSPI");

    string sqlIfadesi1="Select * From Makale";
    string sqlIfadesi2="Select * From Kitap";

    OleDbDataAdapter daMakale=new OleDbDataAdapter(sqlIfadesi1,conFriends);
    OleDbDataAdapter daKitap=new OleDbDataAdapter(sqlIfadesi2,conFriends);

    DataSet ds=new DataSet();

    conFriends.Open();

    daMakale.Fill(ds,"Makaleler");
    daKitap.Fill(ds,"Kitaplar");

    dgMakale.DataSource=ds;

    conFriends.Close();
}
```

Uygulamamızı çalıştırdığımızda aşağıdaki sonucu alırız.

![mk57_6.gif](images/mk57_6.gif)

Şekil 6. Fill metodunda Tablo isimlerinin verilmesi.

Bazı durumlarda, toplu sorgular (batch queries) çalıştırmak isteyebiliriz. Örneğin aşağıdaki kodları ele alalım. Burada, 3 sorgunun yer aldığı bir toplu sorgu cümleciği yer almaktadır. OleDbDataAdapter nesnemiz için, bu sql cümleciğini kullanıdığımızda sonuç kümelerinin Table, Table1 ve Table2 isimleri ile DataSet içerisine alındığını görürüz.

```csharp
private void btnDoldur_Click(object sender, System.EventArgs e)
{
    OleDbConnection conFriends=new OleDbConnection("Provider=SQLOLEDB;Data Source=localhost;initial             catalog=Friends;integrated security=SSPI");
    string sqlIfadesi="Select * From Makale;Select * From Kitap;Select * From Kisiler"; 
    OleDbDataAdapter daMakale=new OleDbDataAdapter(sqlIfadesi,conFriends); 
    DataSet ds=new DataSet();
    daMakale.Fill(ds);
    dgMakale.DataSource=ds;
}
```

![mk57_7.gif](images/mk57_7.gif)

Şekil 7. Toplu Sorguların Çalıştırılmasının Sonucu.

Bu elbette uygulamamızın görselliği açısından çok hoş bir durum değildir. Yapabileceğimiz işlem ise, OleDbDataAdapter nesnemizin TableMappings koleksiyonuna, tabloların görmek istediğimiz asıl isimlerini eklemek olucaktır. Bu amaçla yazmış olduğumuz kodları aşağıdaki gibi değiştirmeliyiz.

```csharp
private void btnDoldur_Click(object sender, System.EventArgs e)
{
    OleDbConnection conFriends=new OleDbConnection("Provider=SQLOLEDB;Data Source=localhost;initial catalog=Friends;integrated security=SSPI");
    string sqlIfadesi="Select * From Makale;Select * From Kitap;Select * From Kisiler"; 
    OleDbDataAdapter da=new OleDbDataAdapter(sqlIfadesi,conFriends); 

    da.TableMappings.Add("Table","Makaleler");
    da.TableMappings.Add("Table1","Kitaplar");
    da.TableMappings.Add("Table2","Arkadaslarim");

    DataSet ds=new DataSet();
    da.Fill(ds);
    dgMakale.DataSource=ds;
}
```

Burada yapılan işlemi açıklayalım. OleDbDataAdapter nesnemizin, TableMappings koleksiyonu, veri kaynağındaki tablo isimlerinin, uygulama içerisindeki bağlantısız katman nesnesi içerisinden nasıl isimlendirileceğini belirtmektedir. Dolayısıyla Fill metodunu çağırdığımızda ilk sorgu sonucu elde edilen Table isimli tablo, Makaleler olarak, ikinci sorgu sonucu elde edilen sonuç kümesini temsil edilen Table1 tablosu, Kitaplar olarak ve sonundada Table2 ismiyle gelen son tablo Arkadaslarim olarak DataSet içerisine alınacaktır. TableMappings burada, sadece isimlerin eşleştirilmesinde rol oynar. Aynı şekilde, OleDbDataAdapter nesnemize ait Update metodunu kullandığımızda, TableMappings koleksiyonunda bu tablo isimlerini birbirleri ile eşleştirilerek, doğru tabloların doğru kaynaklara yönlendirilmesi sağlanmış olur.

![mk57_8.gif](images/mk57_8.gif)

Şekil 8. TableMappings Koleksiyonunun Önemi.

Şu ana kadarki örneklerimizde, bağlantısız katman nesnesi olarak DataSet'i kullandık. DataSet birden fazla veri tablosunu DataTable nesneleri olarak bünyesinde barındıran kuvvetli bir sınıftır. Ancak çoğu zaman uygulamalarımızda sadece tek tabloyu bağlantısız katmanda kullanmak isteyebiliriz. Böyle bir durumda bu tek tablo verisi için, DataSet nesnesi kullanmak sistem kaynaklarını daha çok harcamak anlamına gelir. Bunu çözmek için, veri kaynağından okunan verileri bir DataTable nesnesine aktarırız. İşte OleDbDataAdapter nesnesinin Fill metodu ile DataTable nesnelerinide doldurabiliriz. Bunun için aşağıdaki kodlarda belirtilen tekniği uygularız.

```csharp
private void btnDoldur_Click(object sender, System.EventArgs e)
{
    OleDbConnection conFriends=new OleDbConnection("Provider=SQLOLEDB;Data Source=localhost;initial catalog=Friends;integrated security=SSPI");
    string sqlIfadesi="Select * From Makale"; 
    OleDbDataAdapter da=new OleDbDataAdapter(sqlIfadesi,conFriends);
    DataTable dt=new DataTable("Makalelerim");
    da.Fill(dt);
    dgMakale.DataSource=dt;
    dgMakale.CaptionText=dt.TableName.ToString();
}
```

Burada DataTable nesnemizi oluştururken parametre olarak String bir değer girdiğimize dikkat edelim. Bu değer, verilerin alındığı kümenin, hangi isimde bir tabloya işaret edeceğini belirtmektedir. Fill metodunun kullanım şeklinde ise parametre olarak DataTable nesnesini alan aşağıdaki prototip kullanılmıştır.

```csharp
public int Fill (DataTable dataTable);
```

Son kodlarımızı çalıştırdığımızda aşağıdaki sonucu elde ederiz.

![mk57_9.gif](images/mk57_9.gif)

Şekil 9. Fill metodu ile verilerin DataTable'a aktarılması.

Fill metodunda dikkati çeken bir diğer nokta, döndürdüğü integer tipteki değerdir. Bu dönüş değeri, OleDbDataAdapter nesnesinin çalıştırdığı sorgu sonucu dönen satır sayısına işaret etmektedir. Söz gelimi yukarıdaki örneğimizi ele alalım. OleDbDataAdapter nesnemizin dönüş değerini kontrol ettiğimizde, tablomuzdan okunan satır sayısının döndürüldüğünü görmüş oluruz.

```csharp
private void btnDoldur_Click(object sender, System.EventArgs e)
{
    OleDbConnection conFriends=new OleDbConnection("Provider=SQLOLEDB;Data Source=localhost;initial catalog=Friends;integrated security=SSPI");
    string sqlIfadesi="Select * From Makale"; 
    OleDbDataAdapter da=new OleDbDataAdapter(sqlIfadesi,conFriends);
    DataTable dt=new DataTable("Makalelerim");
    int SatirSayisi=da.Fill(dt);
    dgMakale.DataSource=dt;
    dgMakale.CaptionText=dt.TableName.ToString();
    MessageBox.Show("Makale Sayısı "+SatirSayisi.ToString());
}
```

![mk57_10.gif](images/mk57_10.gif)

Şekil 10. Fill metodundan dönen değer ile kayıt sayısının öğrenilmesi.

Fill metodunun aşağıda prototipi verilen aşırı yüklenmiş halini kullanarak, belli bir satırdan itibaren belirli bir sayıda kaydın elde edilmesini sağlayabiliriz.

```csharp
public int Fill(DataSet, int, int, string);
```

Burada Fill metodu dört parametre almaktadır. İlk parametremiz verilerin ekleneceği DataSet nesnesi ve son parametrede eklenecek verilerin temsil edileceği tablo adıdır. İkinci ve üçüncü parametreler integer tipte değerler alırlar. İkinci parametre sorgu sonucu elde edilen kayıt kümesinin hangi satırından itibaren okuma yapılacağını, üçüncü parametre ise kaç satır alınacağını belirtmektedir. Örneğin Makale isimli tablomuzdaki verileri, tarih sırasına göre tersten çeken bir sql sorgumuz olduğunu düşünelim. İlk 3 satırı elde edip DataSet içindeki ayrı bir tabloya almak istediğimizi varsayalım. Böylece tabloya eklenen son 3 Makaleye ait bilgilere erişmiş olucağız. Bunun için aşağıdaki tekniği kullanacağız.

```csharp
private void btnDoldur_Click(object sender, System.EventArgs e)
{
    OleDbConnection conFriends=new OleDbConnection("Provider=SQLOLEDB;Data Source=localhost;initial catalog=Friends;integrated security=SSPI");
    string sqlIfadesi="Select * From Makale Order By Tarih Desc"; 
    OleDbDataAdapter da=new OleDbDataAdapter(sqlIfadesi,conFriends);
    DataSet ds=new DataSet("Makaleler");
    int SatirSayisi=da.Fill(ds,0,3,"Son3Makale");
    dgMakale.DataSource=ds;
}
```

Burada Fill metodunda select sorgusu sonucu elde edilen kümede 0 indisli satırdan (yani ilk satır) itibaren 3 satır verinin okunmasını ve Son3Makale isimli tabloya aktarılmasını sağlıyoruz. İşte sonuç.

![mk57_11.gif](images/mk57_11.gif)

Şekil 11. Fill metodu ile belli bir satırdan itibaren belli sayıda satır almak.

Fill metodu ile, veri kaynağındaki verileri bağlantısız katmana çekerken saklı yordamları kullanmak isteyebiliriz. Bu amaçla, OleDbDataAdapter sınıfının SelectCommand özelliğine, bu saklı yordamı çalıştırmak için kullanılacak bir OleDbCommand nesnesini atamak kullanabileceğimiz tekniklerden birisidir. OleDbCommand nesnesini yaratırken kullandığımız new yapılandırıcısına ait aşırı yüklenmiş hallerden birisi aşağıdaki gibiydi.

```csharp
public OleDbDataAdapter(OleDbCommand);
```

Burada OleDbCommand nesnesini, saklı yordamımızı çalıştıracak şekilde oluştururuz. Aşağıdaki örnek saklı yordamımızı göz önüne alalım.

```csharp
CREATE PROCEDURE Makaleler
AS
Select * From Makale 
RETURN
```

Şimdi bu saklı yordamımızı çalıştıracak OleDbDataAdapter nesnemiz için gerekli kodlamaları yapalım.

```csharp
private void btnDoldur_Click(object sender, System.EventArgs e)
{
    OleDbConnection conFriends=new OleDbConnection("Provider=SQLOLEDB;Data Source=localhost;initial catalog=Friends;integrated security=SSPI");
    OleDbCommand cmd=new OleDbCommand("Makaleler",conFriends);
    cmd.CommandType=CommandType.StoredProcedure;
    OleDbDataAdapter da=new OleDbDataAdapter(cmd);
    DataSet ds=new DataSet();
    da.Fill(ds,"TumMakaleler");
    dgMakale.DataSource=ds;
}
```

Burada OleDbDataAdapter nesnemizi, OleDbCommand nesnemizi kullanacak şekilde oluşturduk. Dolayısıyla, OleDbDataAdapter, OleDbCommand nesnesinin belirttiği sql ifadesini, yine OleDbCommand nesnesinin kullandığı bağlantı üzerinden çalıştırmaktadır. OleDbCommand nesnemiz bir saklı yordama işaret ettiği için, OleDbDataAdapter sonuç olarak bu saklı yordamı çalıştırmış olur. Böylece DataSet nesnemizin bellekte işaret ettiği bölge, saklı yordamın çalışması sonucu dönen veriler ile doldurulmuş olucaktır.

![mk57_12.gif](images/mk57_12.gif)

Şekil 12. OleDbDataAdapter ile Saklı yordamın çalıştırılması.

Tabi bu amaçla illede bir OleDbCommand nesnemiz kullanmak şart değildir. Aynı işlem için aşağıdaki söz dizimini sql ifadesi olarak SelectCommand özelliği için belirleyebiliriz.

```csharp
{Call Makaleler}
```

Bu durumda kodlarımızı aşağıdaki gibi değiştirmemiz gerekmektedir.

```csharp
private void btnDoldur_Click(object sender, System.EventArgs e)
{
    OleDbConnection conFriends=new OleDbConnection("Provider=SQLOLEDB;Data Source=localhost;initial catalog=Friends;integrated security=SSPI");
    OleDbDataAdapter da=new OleDbDataAdapter("{CALL Makaleler}",conFriends);
    DataSet ds=new DataSet();
    da.Fill(ds,"TumMakaleler");
    dgMakale.DataSource=ds;
}
```

Burada dikkat edilmesi gereken bir nokta vardır. SqlDataAdapter nesneleri için bu çağırım {EXEC Makaleler} şeklindedir. Buraya kadar anlattıklarımızla OleDbDataAdapter sınıfı ile ilgili olarak bayağı bir yol katettiğimizi düşünüyorum. Bir sonraki makalemizde, OleDbDataAdapter sınıfını incelemeye devam edeceğiz. Öncelikle OleDbDataAdapter nesnelerinin Visual Studio.Net ortamında kolayca nasıl oluşturulduklarını inceleyeceğiz. Böylece OleDbDataAdapter sınıfı için gereken SelectCommand, InsertCommand, DeleteCommand, UpdateCommand özelliklerinin nasıl otomatik olarak oluşturulduğunu anlayacağız. Daha sonra aynı iş için CommandBuilder sınıfının nasıl kullanıldığını inceleyeceğiz. Bir sonraki makelemizde görüşmek dileğiyle hepinize mutlu günler dilerim.
