---
title: "DataRelation Sınıfı ve Çoğa-Çok (Many-to-many) İlişkiler"
pubDate: 2004-04-01 09:00:00
categories:
  - Ado.Net
tags:
  - Ado.Net
  - C#
  - .NET
  - DataSet
  - DataTable
---

# DataRelation Sınıfı ve Çoğa-Çok (Many-to-many) İlişkiler
Değerli Okurlarım, Merhabalar.

Bugünkü makalemizde, DataRelation sınıfı yardımıyla, veritabanlarındaki many-to-many (Çoğa-çok) ilişkilerin, bağlantısız katmanda nasıl kullanılabildiğini incelemeye çalışacağız. İlişkisel veri tabanı modelinde, tablolar arası ilişkilerde çoğunlukla bire-çok (one-to-many) ilişkilere rastlarız. Ancak azda olsa, çoğa-çok ilişkilerin kullanıldığı durumlarda söz konusudur. Bu ilişkiye örnek olarak çoğunlukla, Sql sunucusunda yer alan Pubs veritabanındaki Authors ve Titles tabloları gösterilir. Bu iki tablo arasındaki ilişki şöyledir; bir yazara ait birden fazla kitap titles tablosunda yer alabilir. Aynı şekilde, bir kitap birden fazla yazar tarafından kaleme alınmış olabilir. Bu bahsedilen iki ilişkide ayrı ayrı bire-çok ilişkilerdir. Yani bir yazarın birden fazla kitabı yazmış olması bire çok ilişki olarak düşünülebilirken, bir kitabın birden fazla yazara ait olmasıda bire-çok ilişki olarak gözlemlenebilir.

Ancak, bu iki tablo arasında ilişkiyi bu şekilde yansıtmamız mümkün değildir. Nitekim, bire-çok ilişkilerde, çok ilişkiyi temsil eden tablodaki yabancı anahtar (foreign key), ebeveyn (parent) tabloda unique özellikte bir alana ihtiyaç duyar. Dolayısıyla iki yönlü ilişkinin olduğu authors ve titles gibi tablolar için bu tarz bir ilişkiyi oluşturmak biraz daha farklıdır. Bunun için üçüncü bir tablo kullanılır ve bu tabloda, her iki tablonun primary key alanlarına yer verilir. Aşağıdaki şekil, pubs veritabanında yer alan authors ve titles tabloları için çoğa-çok ilişkiyi sağlayacak bu tarz bir tablonun yapısını ve aralarındaki ilişkiyi göstermektedir.

![mk62_1.gif](images/mk62_1.gif)

Şekil 1. titleauthor tablosu yardımıyla çoğa-çok ilişkinin tanımlanması.

Authors tablosunda yer alan her au_id alanı, titleauthors tablosunda yer alır. Aynı durum titles tablosundaki title_id alanı içinde geçerlidir. Böylece, author ve titles tabloları arasındaki çoğa-çok ilişki, titleauthor tablosu üzerinden gerçekleştirilebilmektedir.

Şimdi dilerseniz, kendimiz çoğa-çok ilişkiye sahip iki tablo ve bu tabloların arasındaki ilişkiyi gerçekleştirecek üçüncü bir ara tabloyı oluşturalım. Örnek olarak, büyük bir yazılım şirketindeki proje elemanlarını ve gerçekleştirilen projeleri ele alabiliriz. Bir proje mühendisi pek çok proje gerçekleştirebileceği gibi, yapılan, halen çalışılan veya planlanan projelerde birden fazla proje mühendiside görev alabilir. İşte bu çoğa-çok ilişki için gösterebileceğimiz güzel bir örnektir. Bu amaçla sql sunucumuzda aşağıdaki yapılara sahip tabloları oluşturalım.

![mk62_2.gif](images/mk62_2.gif)

Şekil 2. Projedeki mühendislere ait genel bilgileri taşıyan Muhendisler tablosu.

![mk62_3.gif](images/mk62_3.gif)

Şekil 3. Projelere ait yüzeysel bilgileri tutacak olan Projeler tablosu.

Son olarakta çoka-çok ilişkiyi taşıyacak ara tablomuz.

![mk62_4.gif](images/mk62_4.gif)

Şekil 4. MuhendisProje Tablomuz Çoka-çok ilişkiyi taşıyacak.

MuhendisProje tablosunu oluşturduğumuzda, Muhendisler tablosundan bu tabloya bire-çok ilişki ve yine Projeler tablosundan bu tabloya bire-çok ilişkileri aşağıdaki gibi oluşturmamızda gerekiyor.

![mk62_5.gif](images/mk62_5.gif)

Şekil 5. MuhendisProje tablosu üzerinden gerçekleştirilen çoka-çok ilişki.

Gelelim işin.net kısmına. Tasarladığımız bu yapıyı uygulamalarımızda kullanabilmek için, özellikle bağlantısız katman nesneleri üzerinde kullanabilmek için DataRelation sınıfını kullanmamız gerekiyor. Yukarıdaki işlemler ile sql sunucumuzda oluşturduğumuz düzenin aynısını, sistemimizdeki bağlantısız katman uygulamasında gerçekleştirmek istediğimiz senaryoyu göz önüne alalım.

Öncelikle, sahip olduğumuz üç tabloyuda bir DataSet nesnesine aktarmamız gerekiyor. Daha sonra, sql sunucusundaki bu tablolar arasındaki ilişkileri, DataSet içerisindeki tablolarımız arasındada gerçekleştirmemiz gerekli. İşte bu noktada DataRelation sınıfı devreye giriyor. Önce, Muhendisler tablosundan, MuhendisProje tablosuna olan bire-çok ilişkiyi oluşturuyoruz. Ardından ise, Projeler tablosundan, MuhendisProje tablosuna olan bire-çok ilişkiyi tasarlıyoruz. Bu ilişkilerin DataRelation nesneleri olarak tanımlanmasının ardından, DataSet sınıfının DataRelation nesnelerini taşıyan Relations koleksiyonunada eklenmeleri gerekiyor. İşte bu son adım ile birlikte, veritabanı sunucusundaki çoğa-çok ilişkinin aynısını bağlantısız katman nesnemiz olan DataSet üzerinde de gerçekleştirmiş oluyoruz.

Dilerseniz, yukarıda özetlediğimiz işin uygulamada nasıl gerçekleştirilebileceğini incelemeye çalışalım. Bunu için bir windows uygulaması geliştirebiliriz. Bu uygulamada bir proje mühendisi seçildiğinde, bu mühendisin yer aldığı projeleri ve bu projelerdeki ekip arkadaşlarını gösterecek olan bir uygulama geliştirelim. Bu amaçla aşağıdakine benzer tarzda bir form hazırlayalım.

![mk62_6.gif](images/mk62_6.gif)

Şekil 6. Form Tasarımımız.

Sıra geldi uygulamamızın kodlarını yazmaya. Uygulamayı iki kısımda yazarsak daha kolay anlaşılır olucaktır. Öncelikle, bir mühendisi seçtiğimizde bu mühendisin görev aldığı projeleri elde edebileceğimiz kodu uygulamamıza ekleyelim. Bu aşamada uygulamamızın kodları aşağıdaki gibi olacaktır.

```csharp
SqlConnection con;
SqlDataAdapter da;
DataSet ds;
DataTable dtMuhendisler;
DataTable dtProjeler;
DataTable dtMuhendisProje;

private void Form1_Load(object sender, System.EventArgs e)
{
    /* Sql sunucumuza bir bağlantı açıyoruz. */
    con=new SqlConnection("Data source=localhost;initial catalog=Friends;integrated security=SSPI");

    /* Mühendisler, MuhendisProje ve Projeler tablolarını referans edicek DataTable nesneleri ile bu DataTable nesnelerini bünyesinde barındıracak DataSet nesnemizi oluşturuyoruz.*/
    DataSet ds=new DataSet(); 
    dtMuhendisler=new DataTable();
    dtProjeler=new DataTable();
    dtMuhendisProje=new DataTable();

    /* SqlDataAdapter nesnemiz ile ilk aşamada, Muhendisler tablosundaki bilgileri alıyor ve dtMuhendisler DataTable nesnesine yüklüyoruz. */
    da=new SqlDataAdapter("Select * From Muhendisler",con);
    da.Fill(dtMuhendisler);
    /* Tanımladığımız primaryKey alanını, listBox kontrolündeki bir Muhendisi seçtiğimizde bu Muhendise ait satırı bulucak Find metodunda kullanabilmek için, PersonelID üzerinden oluşturuyoruz. */
    dtMuhendisler.PrimaryKey=new DataColumn[]{dtMuhendisler.Columns["PersonelID"]};
    /* dtMuhendisler DataTable nesnesini, DataSet in Tables koleksiyonuna ekliyoruz. */
    ds.Tables.Add(dtMuhendisler);

    /* Şimdi Projeler tablosundaki verileri, dtProjeler DataTable nesnesine ekliyor ve bu dataTable'ıda DataSet nesnemizin Tables koleksiyonuna ekliyoruz.*/
    da=new SqlDataAdapter("Select * From Projeler",con);
    da.Fill(dtProjeler);
    ds.Tables.Add(dtProjeler);

    /* Sıra MuhendisProje tablosundaki verilerin eklenmesinde. */
    da=new SqlDataAdapter("Select * From MuhendisProje",con);
    da.Fill(dtMuhendisProje);
    ds.Tables.Add(dtMuhendisProje);

    /* Şimdi işin önemli kısmı. Muhendisler tablosundan MuhendisProje tablosuna olan bire-çok ilişkiyi DataSet nesnemizin Relations koleksiyonuna bir DataRelation nesnesi olarak ekliyoruz.*/
    ds.Relations.Add("Muhendis_MuhendisProje",dtMuhendisler.Columns["PersonelID"],dtMuhendisProje.Columns["PersonelID"],false);

    /* Burada ise, Projeler tablosunda, MuhendisProje tablosuna olan bire-çok ilişkiyi tanımlıyor ve DataSet nesnemizin Relations koleksiyonuna DataRelation olarak ekliyoruz.*/
    ds.Relations.Add("Projeler_MuhendisProje",dtProjeler.Columns["ProjeID"],dtMuhendisProje.Columns["ProjeID"],false);

    /* lbMuhendisler ListBox kontrolünün dtMusteriler DataTable'ındaki verileri göstereceğini belirtiyoruz.*/
lbMuhendisler.DataSource=dtMuhendisler;
    /* Buradaki satırlarda, Form üzerindeki lbMuhendisler ListBox kontrolü için, Muhendislerin adlarını gösterecek DisplayMember ve her bir mühendisin PersonelID alanının değerini indis olarak alıcak ValueMember özelliklerini belirliyoruz.*/
    lbMuhendisler.DisplayMember=dtMuhendisler.Columns["Ad"].ToString();
    lbMuhendisler.ValueMember=dtMuhendisler.Columns["PersonelID"].ToString();
}

private void btnGetir_Click(object sender, System.EventArgs e)
{ 
    lbProjeler.Items.Clear();

    /* Öncelikle lbMuhendisler ListBox kontrolünden seçilen Mühendise ait PersonelID alanının değerini alıyor ve DataTable sınıfının Rows koleksiyonunun Find metodu yardımıyla bu PrimaryKey değerini içeren satırı dtMuhendisler tablosundan buluyor ve DataRow nesnesine aktarıyoruz.*/
    DataRow dr; 
    dr=dtMuhendisler.Rows.Find(lbMuhendisler.SelectedValue);

    /* Foreach döngüsünde ilk aşamada, seçilen Muhendisin çocuk satırlarını(child rows) MuhendisPersonel tablosundan alıyoruz. Bir veya birden fazla satır geldiğini düşünürsek her bir satır içinde, Projeler ve MuhendisProje tabloları arasındaki ilişkiyi kullanarak, GetParentRow metodu yardımıyla, Mühendislerin çalıştığı Projelere ait satırları elde ediyoruz. Sonrada bu satırlardaki ProjeAdi alanın değerini lbPorjeler isimli ListBox kontrolümüze ekliyoruz.*/
    foreach(DataRow drProjeNo in dr.GetChildRows("Muhendis_MuhendisProje"))
    {
        DataRow drProje=drProjeNo.GetParentRow("Projeler_MuhendisProje");
        lbProjeler.Items.Add(drProje["ProjeAdi"]);
    }
}
```

Kodumuzda neler olduğunu anlayabilmek için aşağıdaki şekil bize daha fazla yardımcı olucaktır. Burada, foreach döngüsü içerisinde meydana gelen olaylar tasvir edilmeye çalışılmıştır.

![mk62_7.gif](images/mk62_7.gif)

Şekil 7. Bir Mühendisin üzerinde çalıştığı projelerin elde edilmesi.

Uygulamayı çalıştırıp herhangibir Mühendis için Getir başlıklı butona tıkladığımızda, bu mühendisin çalıştığı projelerin elde edildiğini görürüz.

![mk62_8.gif](images/mk62_8.gif)

Şekil 8. Uygulamanın çalışmasının sonucu.

Şimdi gelelim ilkinci kısma. Foreach döngüsü içinde, seçilen Mühendis satırının, MuhendisPersonel tablosundaki alanlarını GetChildRows metodu ile elde ettik. Daha sonra elde edilen her satırın, Projeler tablosunda karşılık geldiği satırlara ulaştık. Bu işlemi gerçekleştirmemizde, MuhendisProje tablosunun ve bu tablo ile Muhendis ve Projeler tablolarının aralarındaki bire-çok ilişkilerin büyük bir önemi vardır. Şimdi ise amacımız elde edilen projelere kimlerin katıldığı. İşte bunun için, elde edilen her proje satırından geriye doğru gidecek ve yine ilişkileri kullanarak bu problemi sonuca kavuşturacağız. Bunun için btnGetir olay prosedüründeki kodları aşağıdaki şekilde değiştirmemiz yeterlidir.

```csharp
private void btnGetir_Click(object sender, System.EventArgs e)
{ 
    lbProjeler.Items.Clear();
    lbEkip.Items.Clear();

    DataRow dr; 
    dr=dtMuhendisler.Rows.Find(lbMuhendisler.SelectedValue);

    foreach(DataRow drProjeNo in dr.GetChildRows("Muhendis_MuhendisProje"))
    {
        DataRow drProje=drProjeNo.GetParentRow("Projeler_MuhendisProje");
        lbProjeler.Items.Add(drProje["ProjeAdi"]);
        lbEkip.Items.Add(drProje["ProjeAdi"]);
            foreach(DataRow drMuh in drProje.GetChildRows("Projeler_MuhendisProje"))
            {
                DataRow drMuhBilgi=drMuh.GetParentRow("Muhendis_MuhendisProje");
                lbEkip.Items.Add(drMuhBilgi["Ad"]+" "+drMuhBilgi["Soyad"]);
            }
    }
}
```

Yeni düzenlemeler ile uygulamamızı çalıştırdığımızda aşağıdaki ekran görüntüsünü elde ederiz.

![mk62_9.gif](images/mk62_9.gif)

Şekil 9. Bir Mühendisin çalıştığı projeler ve projelerdeki takım arkadaşlarının elde edilmesi.

Burada yaptıklarımız değerlendirirsek kafa karıştırıcı tek unsurun foreach döngüsü içerisindeki yaklaşımlar olduğunu görürüz. Bunun yanında, ara tablomuz olan MuhendisProje tablosunun nasıl oluşturulduğu, verileri nasıl tuttuğu ve diğer tablolar arasındaki ilişkilerin.net ortamında bağlantısız katman üzerinde nasıl simüle edildiği önemlidir. Bu olgulara dikkat etmenizi ve iyice incelemenizi öneririm. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde buluşmak dileğiyle hepinize mutlu günler dilerim.
