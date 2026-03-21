---
layout: post
title: "OleDbDataAdapter Sınıfı - 2"
date: 2004-03-02 10:00:00 +0300
categories:
  - ado-net
tags:
  - ado.net
  - oledb
  - rdbms
  - sql
  - csharp
  - data-adapter
  - data
  - constraints
  - primaryKey
---
Önceki makalemizde, OleDbDataAdapter sınıfının ne işe yaradığından bahsetmiş ve kısa bir giriş yapmıştık. Bu makalemizde, OleDbDataAdapter sınıfının diğer önemli unsurlarını incelemeye devam edeceğiz. İncelemek istediğim ilk konu, OleDbDataAdapter nesnesi yardımıyla, ilişkisel veritabanı modellerinden bağlantısız katmana aktarılan tabloların, sahip olduğu birincil anahtar (Primary Key) ve kısıtlamaların (Constraints) ne şekilde irdelendiği olucak. Konuyu iyice kavrayabilmek amacıyla aşağıdaki basit örnek ile incelememize başlayalım. Bu örneğimizde, sql sunucumuzda yer alan bir tabloya ait verileri DataSet üzerine alıyor ve alınan alanların bir takım bilgilerini okuyoruz. Örneğin, alanların veri tipi, boyutu, null değerler içerip içermediği ve alan adları bilgilerini ekrana yazdırıyoruz.

```csharp
using System;
using System.Data;
using System.Data.OleDb;

namespace OleDbDA2
{
    class Class1
    {
        static void Main(string[] args)
        {
            OleDbConnection con=new OleDbConnection("Provider=SQLOLEDB;data source=localhost;initial catalog=Friends;integrated security=sspi"); // Bağlantı nesnemiz tanımlanıyor.
            string sqltext="Select * From Deneme"; // Tablodaki tüm verileri çekicek sql ifademiz.
            OleDbDataAdapter da=new OleDbDataAdapter(sqltext,con); // OleDbDataAdapter nesnemiz oluşturuluyor.
            DataSet ds=new DataSet(); // DataSet bağlantısız katman nesnemiz oluşturuluyor.
            da.Fill(ds,"Makale"); // DataSet nesnemiz, tablomuza ait veriler ile dolduruluyor. 
            /* Tablodaki alanlara ait temel bilgileri edinmek için foreach döngüsünü kullanıyoruz. 0 indisli tablomuz yani deneme tablomuza ait tüm alanlar tek tek DataColumn tipindeki c nesnemiz ile dolaşıyoruz. */
            foreach(DataColumn c in ds.Tables[0].Columns)
            {
                Console.WriteLine("_"+c.ColumnName.ToString()+"_"); // Alanın adı.
                Console.WriteLine("Alan genisligi _"+c.MaxLength.ToString()); /* Alan text değer içeriyorsa maksimum uzunluk. */
                Console.WriteLine("Veri türü _"+c.DataType.ToString()); // Alanın veri türü.
                Console.WriteLine("Null durumu _"+c.AllowDBNull.ToString()); // Alanın null değer içerebilip içeremeyeceği.
                Console.WriteLine("");
            }
        }
    }
}
```

Bu kodlarda ne yaptığımızı kısaca anlatalım. Öncelikle, Sql sunucumuzda yer alan Friends isimli veritabanına OleDb veri sağlayıcısı üzerinden bir bağlantı hattı açıyoruz. Daha sonra, bu veritabanındaki Deneme isimli tabloya ait tüm satırları elde edebileceğimiz sql söz dizimi ile bir OleDbDataAdapter nesnesi oluşturuyoruz. Bu nesnenin Fill metodunu kullanarak, DataSet nesnemizi ilgili tabloya ait veriler ile dolduruyoruz. Aynı zamanda sql sunucusundaki deneme isimli tabloya ait alan bilgilerinide elde etmiş oluyoruz. Her bir alanın ismini, bu alan text veri içeriyorsa maksimum karakter uzunluğunu, veri tipini ve null değer içerip içermediğini öğrenmek için, bir döngü kuruyor ve bu döngü içerisinden bu alan bilgilerine, DataColumn sınıfından bir nesne örneğini kullanarak erişiyoruz. Uygulamamızı çalıştırdığımızda aşağıdaki sonucu elde ettiğimizi görürüz.

![mk58_1.gif](/assets/images/2004/mk58_1.gif)

Şekil 1. Uygulamanın çalışmasının sonucu.

Oysa sql sunucumuzda yer alan tablomuzu incelediğimizde bilgilerin daha farklı olduğunu görürüz. İlk göze çarpan, alanların null değer içerebilip içeremeyeceğini gösteren AllowDBNull özelliklerinin doğru bir şekilde elde edilememiş olmalarıdır. Tablomuzun sql sunucusundaki asıl görünümü aşağıdaki gibidir. Kodumuz sonucu tüm alanların null değer içerebileceği belirtilmektedir. Ama durum gerçekte böyle değildir. Diğer yandan string tipteki Deger1 alanının maksimum uzunluk değeri 50 olmasına rağmen uygulamamızın ekran çıktısında -1 olarak görülmektedir.

![mk58_2.gif](/assets/images/2004/mk58_2.gif)

Şekil 2. Deneme tablomuzun yapısı.

Burada alanlara ait asıl bilgilerin elde edilebilmesi için tabloya ait şema bilgilerinide DataSet nesnemize yüklememiz gerekiyor. İşte bu amaçla, OleDbDataAdapter sınıfına ait FillSchema metodunu kullanırız. FillSchema metodu ilgili tabloya ait alan bilgilerini örneğin Primary Key verisini, bağlantısız katman nesnesine eklememizi sağlar. Bunun için aşağıdaki kodlamayı kullanırız.

```csharp
using System;
using System.Data;
using System.Data.OleDb;

namespace OleDbDA2
{
    class Class1
    {
        static void Main(string[] args)
        {
            OleDbConnection con=new OleDbConnection("Provider=SQLOLEDB;data source=localhost;initial catalog=Friends;integrated security=sspi");
            string sqltext="Select * From Deneme";
            OleDbDataAdapter da=new OleDbDataAdapter(sqltext,con);
            DataSet ds=new DataSet();
            da.FillSchema(ds,SchemaType.Source); // Şema bilgileri ekleniyor.
            da.Fill(ds,"Makale");
            foreach(DataColumn c in ds.Tables[0].Columns)
            {
                Console.WriteLine("_"+c.ColumnName.ToString()+"_");
                Console.WriteLine("Alan genisligi _"+c.MaxLength.ToString());
                Console.WriteLine("Veri türü _"+c.DataType.ToString());
                Console.WriteLine("Null durumu _"+c.AllowDBNull.ToString());
                Console.WriteLine("");
            }
            Console.WriteLine("Birincil anahtar alanımız:"+ds.Tables[0].PrimaryKey[0].ColumnName.ToString());
        }
    }
}
```

Şimdi uygulamamızı çalıştırırsak aşağıdaki sonucu elde ederiz.

![mk58_3.gif](/assets/images/2004/mk58_3.gif)

Şekil 3. Uygulamanın çalışmasının sonucu.

Artık alanların null değer içerip içermeyeceklerine ait kıstaslar doğru bir şekilde görünmektedir. Aynı zamanda, text bazındaki alanların maksimum uzunluklarıda elde edilebilmektedir. (Bu noktada, sayısal alanların genişlik değerlerinin -1 çıkması normaldir. Nitekim DataColumn sınıfının MaxLength özelliği yanlızca text bazlı alanlar için geçerlidir.) Diğer yandan, tablonun birincil anahtar sütununun varlığı elde edilebilmiştir. Primary Key alanının önemini aşağıdaki örnek ile incelemeye çalışalım. Bu örnekte, windows uygulamamızda, bir DataSet nesnesini deneme tablosunun verileri ile dolduruyor ve sonuçları DataGrid kontrolünde gösteriyoruz. İlk etapta FillSchema metodunu kullanmayalım.

```csharp
private void Form1_Load(object sender, System.EventArgs e)
{
    OleDbConnection con=new OleDbConnection("Provider=SQLOLEDB;data source=localhost;initial catalog=Friends;integrated security=sspi");
    string sqltext="Select * From Deneme";
    OleDbDataAdapter da=new OleDbDataAdapter(sqltext,con);
    DataSet ds=new DataSet();
    da.Fill(ds,"Makale");
    dataGrid1.DataSource=ds.Tables["Makale"];
}
```

Uygulamayı çalıştırıp yeni bir satır girelim. ID alanının değerini istediğimiz bir sayı ile değiştirebildiğimizi görürüz.

![mk58_4.gif](/assets/images/2004/mk58_4.gif)

Şekil 4. ID alanının değerini değiştirebiliyoruz.

Oysaki ID alanımız veri kaynağımızda birincil anahtar olarak tanımlanmıştır ve otomatik olarak artmaktadır. Şimdi uygulamamızda FillSchema metodunu kullanalım.

```csharp
private void Form1_Load(object sender, System.EventArgs e)
{
    OleDbConnection con=new OleDbConnection("Provider=SQLOLEDB;data source=localhost;initial catalog=Friends;integrated security=sspi");
    string sqltext="Select * From Deneme";
    OleDbDataAdapter da=new OleDbDataAdapter(sqltext,con);
    DataSet ds=new DataSet();
    da.FillSchema(ds,SchemaType.Source,"Makale");
    da.Fill(ds,"Makale");
    dataGrid1.DataSource=ds.Tables["Makale"]; 
}
```

Uygulamamızı tekrar çalıştırıp yeni bir satır eklediğimizde ID alanının değerini değiştiremediğimizi ve bu değerin yeni bir satırın eklenmesi ile otomatik olarak 1 arttığını görürüz.

![mk58_5.gif](/assets/images/2004/mk58_5.gif)

Şekil 5. ID alanına ait kısıtlamanın eklenmesi sonucu.

Bu FillSchema metodunun, veri kaynağındaki tabloya ait ID alanının birincil anahtar kısıtlamasını bağlantısız katmana aktarması sonucu gerçekleşmiştir. Aynı işlemi FillSchema metodunu kullanmadanda gerçekleştirebiliriz. Bunun için, birncil anahtar olucak alana ait temel özelliklerin, ilgili DataTable nesnesinin PrimaryKey özelliğince belirlenmesi gerekir. Yukarıdaki örneğimizi aşağıdaki şekildede geliştirebiliriz.

```csharp
private void Form1_Load(object sender, System.EventArgs e)
{
    OleDbConnection con=new OleDbConnection("Provider=SQLOLEDB;data source=localhost;initial         catalog=Friends;integrated security=sspi");
    string sqltext="Select * From Deneme";
    OleDbDataAdapter da=new OleDbDataAdapter(sqltext,con);
    DataSet ds=new DataSet();
    da.Fill(ds,"Makale"); 
    /* Birincil anahtarımız olan ID alanının otomatik artan, null değer içermeyen, 1'den başlayıp 1'er artan ve benzersiz değerler alan bir alan olduğunu belirtiyoruz. */
    ds.Tables["Makale"].Columns["ID"].AutoIncrement=true; // Alanın değerleri otomatik artıcak.
    ds.Tables["Makale"].Columns["ID"].AutoIncrementSeed=1; // Başlama değeri 1 olucak.
    ds.Tables["Makale"].Columns["ID"].AutoIncrementStep=1; // Artış değeri 1 olucak.
    ds.Tables["Makale"].Columns["ID"].AllowDBNull=false; // Alan null değer içeremiyecek.
    ds.Tables["Makale"].Columns["ID"].Unique=true; // Alan benzersiz değerler almak zorunda olucak.
    /* Bu tanımlamaların ardından yapmamız gereken, Makale isimli DataTable nesnemiz için PrimaryKey alanının ID alanı olduğunu belirtmektir. */
    ds.Tables["Makale"].PrimaryKey=new DataColumn[]{ds.Tables["Makale"].Columns["ID"]}; /* ID alanının tanımladığımız özellikleri ile birlikte, Makale tablosunun birincil anahtarı olacağını belirtiyoruz. */
    dataGrid1.DataSource=ds.Tables["Makale"]; 
}
```

Bu durumda da aynı sonucu elde ederiz. Diğer yandan birbirleri ile ilişkili olan tabloların bu ilişkilerini belirten ForeingKeyConstraint kısıtlamalarınıda DataSet nesnelerine aktarmamız gerekir. Bu konuyu ilerleyen makalelerimizde DataSet kavramını işlerken incelemeye çalışacağız. Şimdi OleDbDataAdapter ile ilgili diğer konularımıza devam edelim.

OleDbDataAdapter sınıfıları ile ilgili incelemek istediğim ikinci konu bu nesnelerin, Visual Studio.Net ortamında nasıl oluşturulduğudur. Visual Studio.Net ortamında bu işlem oldukça kolaydır. Bunun için pek çok yöntemimiz var. Bunlardan birisi Server Explorer alanından tabloyu, Form üzerine sürüklemektir. Bu işlem sonucunda, bu tablo için gerekli Connection nesnesi ve DataAdapter nesnesi otomatik olarak oluşturulacaktır. Bir diğer yol ise, OleDbDataAdapter Componentini kullanmaktır. Ben makalemde, bu ikinci yolu incelemeye çalışacağım. Visual Studio.Net ortamında yeni bir windows uygulaması açın ve Formun üzerine, OleDbDataAdapter componentini sürükleyin.

![mk58_6.gif](/assets/images/2004/mk58_6.gif)

Şekil 6. OleDbDataAdapter aracı diğer Ado.Net araçları gibi ToolBox'ın Data kısmında yer alır.

Bu durumda karşımıza DataAdapter Configuration Wizard penceresi çıkacaktır. Bu pencereyi next butonuna basarak geçelim.

![mk58_7.gif](/assets/images/2004/mk58_7.gif)

Şekil 7. Başlangıç noktamız.

Bu adımdan sonra, OleDbDataAdapter nesnemizin kullanacağı bağlantı hattı için gerekli bağlantı bilgilerini ayarlarız. Burada halen var olan bir bağlantıyı kullanabileceğimiz gibi New Connection seçeneği ile yeni bir bağlantı bilgisede oluşturabiliriz. Bu adımda oluşturacağımız bağlantı bilgisi, uygulamamız için gerekli olan OleDbConnection nesnesinin oluşturulmasındada kullanılacaktır. Ben burada New Connection diyerek yeni bir bağlantı bilgisi oluşturdum. Artık OleDbDataAdapter nesnemiz bu bağlantı katarını kullanıcak.

![mk58_8.gif](/assets/images/2004/mk58_8.gif)

Şekil 8. Bağlantı bilgimizi tanımlıyoruz.

Next ile bu adımıda geçtikten sonra, sorgu tipini seçeceğimiz bölüme geliriz. Burada üç seçeneğimiz vardır. Use Sql Statements ile, OleDbDataAdapter nesnesi yardımıyla bilgilerini alacağımız tablo (lar) için gerekli sql ifadelerinin sihirbaz yardımıyla oluşturulmasını sağlarız. Bu işlem sonucunda bizim için gerekli olan SelectCommand, InsertCommand, UpdateCommand ve DeleteCommand özelliklerine ait sql ifadeleri otomatik olarak oluşturulacaktır. Diğer yandan ikinci seçenek ile, bu sql ifadeleri için saklı yordamların oluşturulmasını sağlayabiliriz. Son seçeneğimiz ise, sistemde var olan saklı yordamların kullanılmasını sağlar. Biz şu an için ilk seçeneği seçiyoruz.

![mk58_9.gif](/assets/images/2004/mk58_9.gif)

Şekil 9. Sorgu tipinin seçilmesi.

Bu adımdan sonra Select sorgusunu oluşturacağımız bölüm ile karşılaşırız. Burada SelectCommand için kullanılacak sql sorgusunu kendimiz elle yazabileceğimiz gibi Query Builder yardımıylada bu işlemi daha kolay bir şekilde yapabiliriz. Biz Query Builder seçeneği ile işlemlerimize devam edelim.

![mk58_10.gif](/assets/images/2004/mk58_10.gif)

Şekil 10. Sql ifadesinin oluşturulması.

Query Builder kısmında, işlemlerimizin başında belirttiğimiz bağlantı bilgisi kullanılır ve bağlanılan veri kaynağına ilişkin kullanılabiliecek tablolar veya görünümler ekrana otomatik olarak gelir. Tek yapmamız gereken kullanmak istediğimiz tabloyu (tabloları) seçmek ve eklemektir. Daha sonra seçtiğimiz tablo veya tablolardaki hangi alanların select sorgusu ile elde edileceğini ve bağlantısız katmana aktarılacağını belirleriz.

![mk58_11.gif](/assets/images/2004/mk58_11.gif)

Şekil 11. Tabloların eklenmesi.

Ben burada Makale isimli tablomuzu seçtim ve sorguya ekledim. Daha sonra sadece görünmesini istediğim alanları belirttim. Burada istersek alanların bağlantısız katman nesnesine hangi isimler ile aktarılacağını Alias sütununa yazdığımız değerler ile belirleyebiliriz. Diğer yandan, Sort Type sütununu kullanarak hangi alanlara göre sırlama yapılmasını istediğimizi belirtebiliriz. Ayrıca alanlara ait çeşitli kriterler girebileceğimiz Criteria sütunuda yer almaktadır. Bu sütunu kullanmamız Where anahtar kelimesi için bir koşul bildirmek anlamına gelmektedir. Bu işlemler boyunca, sorgu ifademizin otomatik olarak oluşturulduğunu ve geliştirildiğini görürüz. Oluşturulan sorgunun sonucunu görmek için bu alan üzerinde sağ tuşla girdiğimiz menuden Run komutunu verebiliriz. Böylece sorgu sonucu elde edilcek tablo verileri içinde bir öngörünüm elde etmiş oluruz.

![mk58_12.gif](/assets/images/2004/mk58_12.gif)

Şekil 12. Query Builder sorgularımızın kolayca oluşturulmasını sağlar.

Bu pencereyi kapttığımızda OleDbDataAdapter nesnemizin, SelectCommand özelliği için gerekli Command nesnesi oluşturulmuş olur. Tekrar Next düğmesine bastığımızda aşağıdaki ekranı elde ederiz. Burada görüldüğü gibi Insert, Update ve Delete sorgularıda otomatik olarak oluşturulmuştur. Ayrıca tablo alanlarımız için kullandığımız eşleştirme işlemleride gerçekleştirilmiş ve TableMappings koleksiyonuda başarılı bir şekilde oluşturulmuştur.

![mk58_13.gif](/assets/images/2004/mk58_13.gif)

Şekil 13. İşlem Tamam.

Finish'e bastığımızda Formumuzda bir OleDbConnection nesnesinin ve birde OleDbDataAdapter nesnesinin oluşturulmuş olduğunu görürüz.

![mk58_14.gif](/assets/images/2004/mk58_14.gif)

Şekil 14. Nesnelerimiz oluşturuldu.

Şimdi bize, bu OleDbDataAdapter nesnemizin çalışması sonucu elde edilecek verilerin aktarılacağı bir bağlantısız katman nesnesi lazım. Yani bir DataSet nesnesi. Bunun için, OleDbDataAdapter nesnesine ait, Generate DataSet seçeneğini kullanabiliriz.

![mk58_15.gif](/assets/images/2004/mk58_15.gif)

Şekil 15. Generate DataSet seçeneği, OleDbDataAdapter nesnesinin özelliklerinin altında yer alır.

Bu durumda karşımıza çıkan pencerede var olan bir DataSet'i seçebilir yada otomatik olarak yeni bir tane oluşturulmasını sağlayabiliriz. Bu işlemin ardından DataSet nesnemizinde oluşturulduğunu görürüz.

![mk58_16.gif](/assets/images/2004/mk58_16.gif)

Şekil 16. Generate DataSet penceresi.

Artık yapmamız gerekenler, formumuza bir DataGrid koymak, OleDbDataAdapter nesnemize Fill metodunu uygulamak ve DataSet'imizi doldurmak, son olarakta DataGrid kontrolümüze bu veri kümesine bağlamaktır.

```csharp
private void Form1_Load(object sender, System.EventArgs e)
{
    oleDbDataAdapter1.Fill(dataSet11.Tables["Makale"]);
    dataGrid1.DataSource=dataSet11.Tables["Makale"];
}
```

Uygulamamızı çalıştırdığımızda aşağıdaki sonucu elde ederiz.

![mk58_17.gif](/assets/images/2004/mk58_17.gif)

Şekil 17. Uygulamanın çalışmasının sonucu.

Burada yaptığımız işlem ile, OleDbDataAdapter nesnesi ile bağlantısız katmandaki veriler üzerindeki değişiklikleri, veri kaynağına gönderirken Update metodunun kullanacağı UpdateCommand, DeleteCommand, InsertCommand gibi özelliklerin sql ifadelerini otomatik olarak oluşturulmasını sağlamış olduk. Diğer yandan aynı işlevselliği kazanmak için CommandBuilder nesnesinide kullanabiliriz. Bu nesnenin kullanılmasını ve OleDbDataAdapter sınıfına ait Update metodunu bir sonraki makalemizde incelemeye çalışacağız. Hepinize mutlu günler dilerim.