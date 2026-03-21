---
layout: post
title: "Self Referencing Relations ve Recursive(Yinelemeli) Metodlar"
date: 2005-03-14 12:00:00 +0300
categories:
  - ado-net
tags:
  - ado.net
  - relations
  - recursive-method
  - self-referencing
---
Çoğumuz çalıştığımız projelerde müdür,müdür yardımcısı gibi ast üst ilişkisine sahip olan organizasyonel yapılarla karşılamışızdır. Örneğin işletmelerin Genel Müdür’ den başlayarak en alt kademedeki personele kadar inen organizasyonel yapılar gibi. Burada söz konusu olan ilişkiler çoğunlukla pozisyonel bazdadır. Yani çok sayıda personel birbirlerine pozisyonel bazda bağımlıdır ve bu bağımlılık en üst pozisyondan en alt pozisyona kadar herkesi kapsar. Bu tarz bir sistemin uygulama ortamında canlandırılabilmesi için pozisyonel ilişkileri ifade edebilecek tablo yapılarına başvurulur. Özellikle pozisyonlar arasındaki ilişkiyi kendi içerisinde referans edebilen tablolar bu tarz ihtiyaçların karşılanması için biçilmiş kaftandır. Örneğin aşağıdaki tabloyu göz önüne alalım.

![mk117_1.gif](/assets/images/2005/mk117_1.gif)

Tabloda, X şirketinin çalışan personelleri arasındaki pozisyonel hiyerarşiyi temsil eden bir yapı kullanılmıştır. Dikkat edilecek olursa, her satırın Amiri alanının değeri yine tablo içinde yer alan bir PersonelNo alanını işaret etmektedir. Bu ilişki Self Referencing Relations olarak adlandırılır. Elbette bu tarz bir sistemde hiyerarşinin nereden başladığının bir şekilde bilinmesi gerekir. Çünkü tepeden aşağıya doğru inmek için en üst birimin diğerlerinden tamamen benzersiz bir şekilde ifade edilmesine ihtiyaç vardır. Buradaki gibi Amiri alanının değeri Null olan bir satır kimseye bağlı değildir. Ancak kendisine bağlı olan bir organizasyonel hiyerarşi söz konusudur. İşte bunu sağlayabilmek için en tepede yer alacak satırın Amiri field’ ının değeri Null olarak belirlenmiştir. Tabloyu daha yakından analiz edecek olursak aşağıdaki hiyerarşik yapının oluşturulabileceğini kolayca görürüz.

![mk117_2.gif](/assets/images/2005/mk117_2.gif)

İşte bu makalemizde yukarıda görmüş olduğumuz hiyerarşik yapıyı bir TreeView kontrolünde nasıl ifade edebileceğimizi incelemeye çalışacağız. Burada anahtar noktalar, Self Referencing Relations oluşturmak ve bu ilişkileri Recursive (Yinelemeli) bir Metod ile uygulayabilmektir. Öncelikle Self Referencing Relation tablomuz üzerinde görmeye çalışalım. Örneğin 5 numaralı PersonelNo değerine sahip satırımızı ele alalım. Bu satırın Amiri field’ ının değeri 2 dir. Yani organizasyonel yapıda, 5 numaralı satır aslında 2 numaralı satırın altında yer almaktadır. Kaldı ki, 2 numaralı satırda 1 numaralı satırın altındadır. Bu şekilde tüm satırların birbirleri ile olan bağlantılarını tespit edebiliriz.

![mk117_3.gif](/assets/images/2005/mk117_3.gif)

![dikkat.gif](/assets/images/2005/dikkat.gif)
Self Referencing Relation özelliğini sağlayan tablolarda bir satır (satırların) bağlı olduğu satırın yine bu tabloda var olmasını sağlamak veri tutarlılığı (consistency) açısından önemlidir.

Buradaki ilişkiyi uygulamalarımızda tanımlamak için DataRelation nesnelerini kullanabiliriz. Örneğin;

```csharp
DataRelation dr=new DataRelation("self",ds.Tables[0].Columns["PersonelNo"],ds.Tables[0].Columns["Amiri"],false);
```

Bizim için ikinci önemli nokta bu ilişkiyi kullanacak olan Recursive (Yinelemeli) bir metodun geliştirilmesidir. Yinelemeli metodumuzu geliştirirken tablo içerisindeki her bir satırı ele alacak ve bu satırların var ise alt satırlarına da bakacak bir algoritmayı göz önüne almamız gerekiyor. Örneğin aşağıdaki Console uygulamasını ele alalım.

```csharp
class Worker
{
    SqlConnection con=new SqlConnection("data source=BURKI;database=Veritabanim;integrated security=SSPI");
    SqlDataAdapter da;
    DataSet ds; 
    DataRelation dr;

    public void Baglan()
    {
        if(con.State==ConnectionState.Closed)
        con.Open();
    }

    public void VeriCek()
    {
        da=new SqlDataAdapter("SELECT * FROM Kadro",con);
        ds=new DataSet(); 
        da.Fill(ds);
        dr=new DataRelation("self",ds.Tables[0].Columns["PersonelNo"],ds.Tables[0].Columns["Amiri"],false);
        ds.Relations.Add(dr);
    }
    //Recursive metodumuz.
    public void DetayiniAl(DataRow dr,string giris)
    {
        Console.WriteLine(giris+dr["Personel"].ToString()); 
        foreach(DataRow drChild in dr.GetChildRows("self"))
        {
            DetayiniAl(drChild,giris+"...");
        }
    }

    public void AgacOlustur()
    {
        foreach(DataRow dr in ds.Tables[0].Rows)
        {
            if(dr.IsNull("Amiri"))
            {
                DetayiniAl(dr,"");
            }
        }
    }
}

class Class1
{
    [STAThread]
    static void Main(string[] args)
    {
        Worker wrk=new Worker();
        wrk.Baglan();
        wrk.VeriCek();
        wrk.AgacOlustur();
    }
}
```

Bu uygulamada basit olarak tablomuzdaki verileri çekiyor ve organizasyonel yapıyı hiyerarşik olarak elde ediyoruz. Ağacımızı oluşturduğumuz AgacOlustur metodu, DataTable içerisindeki tüm satırları gezen bir foreach döngüsünü kullanıyor. Herşeyden önce bizim en tepedeki satırı bir başka deyişle en üstteki pozisyonu bulmamız gerekiyor.

Tablo yapımızdan Amiri alanının değeri Null olan satırın hiyerarşinin tepesinde olması gerektiğini biliyoruz. Bu nedenle if koşulu ile bu alanı buluyoruz. Ardından bulduğumuz satırı DetayiniAl isimli Recursive (Yinelemeli) metodumuza gönderiyoruz. Yinelemeli metodumuz gelen DataRow nesnesinin Child satırlarını gezen başka bir foreach döngüsü kullanıyor. Eğer Child satırlar var ise yinelemeli metodumuz tekrardan çağırılıyor. Bu işlem tüm satırların okunması bitene kadar devam edecektir. Sonuç itibariyle Console uygulamamızı çalıştırdığımızda aşağıdaki ekran görüntüsünü elde ederiz.

![mk117_4.gif](/assets/images/2005/mk117_4.gif)

Gelelim, Windows uygulamamızda bu hiyerarşiyi nasıl şekillendirebileceğimize. İzleyeceğimiz yol Console uygulamamızdaki ile tamamen aynıdır. Tek fark bu kez bir DataRow’ a bağlı alt satırları alırken yinelemeli metodumuza parent node’ un (ki burada bir TreeNode nesnesidir) geçirilişidir. Örnek uygulamayı aşağıda bulabilirsiniz. (Uygulamanın çalışmasını daha iyi anlayabilmek için Trace etmenizi öneririm.) Burada özellike, child satırların hangi TreeNode’ nesnesine eklenmesi gerektiğinin tespiti son derece önemlidir. Dikkat ederseniz AgacOlustur metodumuzda ilk olarak Amiri alanının değeri Null olan satırı temsil edecek bir TreeNode nesnesi oluşturulmuş ve TreeView kontrolüne eklenmiştir. Daha sonra yinelemeli metodumuza bu TreeNode ve o anki DataRow nesneleri gönderilmiştir. Böylece DetayiniAl metodu içerisinde child satırların hangi TreeNode içerisine alınacağı tespit edilebilir.

TreeView kullanımı;

```csharp
SqlConnection con=new SqlConnection("data source=BURKI;database=Veritabanim;integrated security=SSPI");
SqlDataAdapter da;
DataSet ds; 
DataRelation dr;

private void Baglan()
{
    if(con.State==ConnectionState.Closed)
    con.Open();
}

private void VeriCek()
{
    da=new SqlDataAdapter("SELECT * FROM Kadro",con);
    ds=new DataSet(); 
    da.Fill(ds);
    dr=new DataRelation("self",ds.Tables[0].Columns["PersonelNo"],ds.Tables[0].Columns["Amiri"],false);
    ds.Relations.Add(dr);
}

private void DetayiniAl(DataRow dr,TreeNode t)
{
    foreach(DataRow drChild in dr.GetChildRows("self"))
    {
        TreeNode tnChild=new TreeNode(drChild["Personel"].ToString());
        t.Nodes.Add(tnChild);
        DetayiniAl(drChild,tnChild);
    }
}

private void AgacOlustur()
{
    foreach(DataRow dr in ds.Tables[0].Rows)
    {
        if(dr.IsNull("Amiri"))
        {
            TreeNode tn=new TreeNode(dr["Personel"].ToString());
            treeView1.Nodes.Add(tn);
            DetayiniAl(dr,tn);
        }
    }
}

private void Form1_Load(object sender, System.EventArgs e)
{
    Baglan();
    VeriCek();
    AgacOlustur();
}
```

Uygulamamızı çalıştırdığımızda aşağıdaki ekran görüntüsünü elde ederiz.

![mk117_2.gif](/assets/images/2005/mk117_2.gif)

Bu makalemizde kendi satırlarını işaret eden satırların var olduğu ilişkileri taşıyan tablolarda, satır arasındaki hiyerarşik yapının Recursive (Yinelemeli) metodlar ile uygulama ortamına nasıl aktarılabileceğini incelemeye çalıştık. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek kodlar için tıklayın.](/assets/files/2005/SelfRefOrn.zip)
