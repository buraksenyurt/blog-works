---
layout: post
title: "SqlDataReader Sınıfı 1"
date: 2003-12-28 12:00:00 +0300
categories:
  - ado-net
tags:
  - ado.net
  - sqldatareader
  - csharp
  - oop
  - .net
  - database
---
Bugünkü makalemizde, SqlDataReader sınıfını incelemeye çalışacağız. ADO.NET’in bilgisayar programcılığına getirdiği en büyük farklıklardan birisi bağlantısız veriler ile çalışılabilmemize imkan sağlamasıydı. DataSet sınıfıını ve buna bağlı diğer teknikleri kastettiğimi anlamışsınızdır. Bu teknikler ile, bir veritabanı içinde yer alan tabloları, tablolar arasındaki ilişkileri, içerdikleri verileri vb… istemci makinenin belleğinde tutmamız mümkün olabiliyor.

Bu sayede, bu veriler istemci makine belleği üzerinde tutulduğundan, bir veritabanına sürekli bağlantısı olan bir sisteme gore daha hızlı çalışabiliyoruz. Ayrıca veritabanı sunucusuna sürekli olarak bağlı olmadığımız için network trafiğinide hafifletmiş oluyoruz. Tüm bu hoş ayrıntılar dışında hepimiz için nahoş olan ayrıntı, her halikarda bellek tüketiminin artması. Öyleki bazen kullanıdığımız programlarda sadece listeleme amacı ile, sadece görsellik amacı ile küçük veriler ile çalışmak durumunda kaldığımızda, bu elde edilebilirlik için fazlasıyla kaynak tüketmiş oluyoruz. İşte ADO.NET ‘te bu tip veriler düşünülerek, listeleme amacı güden, küçük boyutlarda olan veri kümeleri için DataReader sınıfları tasarlanmış. Biz bugün SqlDataReader sınıfını inceleyeceğiz.

Bir SqlDataReader sınıfı bir veri akımını (data stream) temsil eder. Bu nedenle herhangibir anda bellekte sadece bir veri satırına erişebilir. İşte bu performansı ve hızı arttıran bir unsurdur. Bu veri akımını elde etmek için sürekli açık bir sunucu bağlantısı ve verileri getirecek sql sorgularını çalıştıracak bir SqlCommand nesnesi gereklidir. Dikkat edicek olursanız sürekli açık olan bir sql sunucu bağlantısından yani bir SqlConnection’dan bahsettik. SqlDataReader nesnesi sql sunucu bağlantısının sürekli açık olmasını gerektirdiği için network trafiğini meşgul eder, aynı zamanda kullandığı SqlConnection nesnesinin başka işlemler için kullanılmasınıda engeller. İşte bu eksiler nedeni ile, onun sadece okuma amaçlı veya listeleme amaçlı ve yanlız okunabilir veri kümeleri için kullanılması önerilir. Bu nedenlede, read-only (yanlız okunabilir) ve forward-only (sadece ileri yönlü) olarak tanımlanır. Şimdi SqlDataReader nesnesi ile bir veri akımının elde edilmesi konusunu aşağıdaki şekil yardımıyla incelemeye çalışalım.

![mk28_1.gif](/assets/images/2003/mk28_1.gif)

Şekil 1. SqlDataReader nesnesinin çalışma şekli.

Görüldüğü gibi sql sunucusuna sürekli bağlı durumdayız. Bunun yanında SqlCommand sorgusunun çalıştırılması sonucu elde edilen veri akımı SqlDataReader nesnesince temsil edilmektedir. İstemci bilgisayarımız, her hangibir t zamanında kendi belleğinde SqlDataReader’ın taşıdığı veri akımının sadece tek bir satırını temsil etmektedir. SqlCommand nesnesinin çalıştırdığı sql cümleciğinin sonucunu (sonuç kümesini) SqlDataReader nesnemize aktarmak için ExecuteReader metodu kullanılır. Burada bir noktaya daha değinelim. SqlDataReader sınıfı herhangibi yapıcı metoda sahip değildir. Yani bir SqlDataReader nesnesini bir new metodu ile oluşturamayız.

Sadece bir SqlDataReader nesnesi tanımlayabiliriz. Maharet aslında SqlCommand nesnesinin ExecuteReader metodundadır. Bu metod ile, daha önceden tanımladığımız SqlDataReader nesnesini oluşturur ve SqlCommand sorgusunun sonucu olan veri kümesini temsil etmesini sağlarız. Ancak bir DataTable veya bir DataSet’te olduğu gibi sorgu sonucu elde edilen veri kümesinin tamamı bellekte saklanmaz. Bunun yerine SqlDataReader nesnesi kendisini, çalıştırılan sorgu sonuçlarını tutan geçici bir dosyaının ilk satırının öncesine konumlandırır.

İşte bu noktadan sonraki satırları okuyabilmek için bu sınıfa ait Read metodunu kullanırız. Read metodu, her zaman bir sonraki satıra konumlanılmasını sağlar. Tabi bir sonrasında kayıt olduğu sürece bu işlemi yapar. Böylece bir While döngüsü ile Read metodunu kullanırsak, sorgu sonucu elde edilen veri kümesindeki tüm satırları gezebiliriz. Konumlanılan her bir satır bellekte temsil edilir. Dolayısıyla bir sonraki t zamanında, bellekte eski satırın yerini yeni satır alır. Bu sorgu sonucu elde edilen veri kümesinden belleğe doğru devam eden bir akım (stream) dır. Geriye hareket etmek gibi bir lüksümüz olmadığı gibi, t zamanında bellekte yer alan satırları değiştirmek gibi bir imkanımızda bulunmamaktadır. İşte performansı bu arttırmaktadır. Ama elbetteki çok büyük boyutlu ve satırlı verilerle çalışırken listeleme amacımız yok ise veriler üzerinde değişiklikler yapılabilmesinide istiyorsak bu durumda bağlantısız veri elemanlarını kullanmak daha mantıklı olucaktır.

Şimdi dilerseniz konumuz ile ilgili bir örnek yapalım. Özellikle ticari sitelerde, çoğunlukla kullanıcı olarak ürünleri inceleriz. Örneğin kitap alacağız. Belli bir kategorideki kitaplara bakmak istiyoruz. Kitapların sadece adları görünür olsun. Kullanıcı bir kitabı seçtiğinde, bu kitaba ait detaylarıda görebilsin. Tüm bu işlemler sadece izlemek ve bakmaktan ibaret. Dolayısıyla bu veri listelerini, örneğin bir listBox kontrolüne yükleyecek isek ve sonuç olarak bir listeleme yapıcak isek, SqlDataReader nesnelerini kullanmamız daha avantajlı olucaktır. Dilerseniz uygulamamızı yazmaya başlayalım. Öncelikle Visual Studio.Net ortamında bir Web Application oluşturalım. Default.aspx isimli sayfamızı ben aşağıdaki gibi tasarladım.

![mk28_2.gif](/assets/images/2003/mk28_2.gif)

Şekil 2. WebForm tasarımımız.

Şimdi de kodlarımızı yazalım.

```csharp
SqlConnection conFriends; 

public void baglantiAc()
{
	/* Sql Sunucumuza bağlanmak için SqlConnection nesnemizi oluşturuyoruz ve bağlantıyı açıyoruz. */
	conFriends=new SqlConnection("data source=localhost;integrated security=sspi;initial catalog=Friends");
	conFriends.Open();
} 
private void Page_Load(object sender, System.EventArgs e)
{
	if(!Page.IsPostBack) /* Eğer sayfa daha önce yüklenmediyse bu if bloğu içindeki kodlar çalıştırılıyor. */
	{
		baglantiAc(); /* Sql sunucumuza olan SqlConnection nesnesini tanımlayan ve bağlantıyı açan metodumuzu çağırıyoruz. */ 
		/* SqlCommand nesnemize çalıştıracağımız sql sorgusunu bildiriyoruz. Bu sorgu Kitaplar tablosundan Kategori alanına ait verileri Distinct komutu sayesinde, benzersiz şekilde alır. Nitekim aynı Kategori isimleri tabloda birden fazla sayıda. Biz Distinct sasyesinde birbirinden farklı kategorileri tek tek elde ediyoruz. Böylece veri kümemizizde mümkün olduğunca küçültmüş ve bir SqlDataReader nesnesinin tam dişine göre hazırlamış oluyoruz. */
		SqlCommand cmdKategori=new SqlCommand("Select distinct Kategori From Kitaplar Order By Kategori",conFriends);
		SqlDataReader drKategori; /* SqlDataReader nesnemizi tanımlıyoruz. Lütfen tanımlama şeklimize dikkat edin. Sanki bir değişken tanımlıyoruz gibi. Herhangibir new yapıcı metodu yok. */ 
		drKategori=cmdKategori.ExecuteReader(CommandBehavior.CloseConnection); /* SqlCommand nesnemizin ExecuteReader yardımıyla sorgumuzu çalıştırıyor ve sonuç kümesini temsil edicek SqlDataReader nesnemizi atıyoruz. Bu aşamada SqlDataReader nesnemiz sorgu sonucu oluşan veri kümesinin ilk satırının öncesine konumlanıyor. */
		/* Bahsetmiş olduğumuz while döngüsü ile satırları teker teker ileri doğru olucak şekilde belleğe alıyoruz. Her bir t zamanında bir satır belleğe geliyor ve oradanda ListBox kontrolüne iligli satırın, 0 indexli alanına ait değer GetString metodu ile alınıyor. */
		while(drKategori.Read())
		{
		   lstKategori.Items.Add(drKategori.GetString(0));
		}
		drKategori.Close(); /* SqlDataReader nesnemiz kapatılıyor. Biz ExecuteReader metodunu çalıştırırken parametre olarak, CommandBehavior.CloseConnection değerini verdik. Bu bir SqlDataReader nesnesi kapatıldığında, açık olan bağlantının otomatik olarak kapatılmasını, yani SqlConnection bağlantısının otomatik olarak kapatılmasını sağlar. Buda system kaynaklarının serbest bırakılmasını sağlar.*/
	}
} 

private void btnKitaplar_Click(object sender, System.EventArgs e)
{
	string kategori=lstKategori.SelectedItem.ToString(); 
	baglantiAc(); 
	SqlCommand cmdKitaplar=new SqlCommand("Select distinct Adi,ID From Kitaplar Where Kategori='"+kategori+"' order by Adi",conFriends);
	SqlDataReader drKitaplar;
	drKitaplar=cmdKitaplar.ExecuteReader(CommandBehavior.CloseConnection); 
	int KitapSayisi=0; 
	lstKitaplar.Items.Clear(); 
	while(drKitaplar.Read())
	{
		lstKitaplar.Items.Add(drKitaplar.GetString(0));
		KitapSayisi+=1;
	}
	drKitaplar.Close();
	lblKitapSayisi.Text=KitapSayisi.ToString();
	/* Yukarıdaki döngüde elde edilen kayıt sayısını öğrenmek için KitapSayisi isimli bir sayacı döngü içine koyduğumuzu farketmişsinizdir. SqlDataReader nesnesi herhangibir t zamanında bellekte sadece bir satırı temsil eder. Asıl veri kümesinin tamamını içermez, yani belleğe almaz. Bu nedenle veri kümesindeki satır sayısını temsil edicek, Count gibi bir metodu yoktur. İşte bu nedenle kayıt sayısını bu teknik ile öğrenmek durumundayız. */
} 
private void lstKitaplar_SelectedIndexChanged(object sender, System.EventArgs e)
{
	string adi=lstKitaplar.SelectedItem.ToString();
	baglantiAc(); 
	SqlCommand cmdKitapBilgisi=new SqlCommand("Select * From Kitaplar Where Adi='"+adi+"'",conFriends);
	SqlDataReader drKitapBilgisi;
	drKitapBilgisi=cmdKitapBilgisi.ExecuteReader(CommandBehavior.CloseConnection); 
	lstKitapBilgisi.Items.Clear(); 
	while(drKitapBilgisi.Read())
	{
		/* FieldCount özelliği SqlDataReader nesnesinin t zamanında bellekte temsil etmiş olduğu satırın kolon sayısını vermektedir. Biz burada tüm kolonlardaki verileri okuyacak dolayısıyla t zamanında bellekte yer alan satırın verilerini elde edebileceğimiz bir For döngüsü oluşturduk. Herhangibir alanın değerine, drKitapBilgisi[i].ToString() ifadesi ile ulaşıyoruz. */
		for(int i=0;i<drKitapBilgisi.FieldCount;++i)
		{
		   lstKitapBilgisi.Items.Add(drKitapBilgisi[i].ToString());
		}
		lstKitapBilgisi.Items.Add("------------------");
	}
	drKitapBilgisi.Close();
}
```

Şimdi programımızı bir çalıştıralım ve sonuçlarını bir görelim.

![mk28_3.gif](/assets/images/2003/mk28_3.gif)

Şekil 3. Programın çalışmasının sonucu.

Geldik bir makalemizin daha sonuna. Bir sonraki makalemizde SqlDataReader sınıfını incelemeye devam edeceğiz. Özellikle SqlCommand sınıfının ExecuteReader metodunun aldığı parameter değerlerine gore nasıl sonuçlar elde edebileceğimizi incelemeye çalışacağız. Bunun yanında SlqDataReader sınıfının diğer özelliklerinide inceleyeceğiz. Hepinize mutlu günler dilerim.