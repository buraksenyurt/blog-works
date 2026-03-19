---
layout: post
title: "Sql Tablolarındaki Binary Resimlere Bakmak ve Dosya Olarak Kaydetmek"
date: 2004-01-24 10:00:00 +0300
description: "Hatırlayacağınız gibi bir önceki makalemizde, bir resim dosyasını sql tablosundaki Image veri tipinden bir alana nasıl yazabileceğimizi görmüştük. Bugünkü makalemizde ise, bu tablodaki Image veri tipindeki Resim alanında yer alan byte'lara nasıl okuyabileceğimizi ,( Örneğimizde, PictureBox kontrolünde nasıl görüntüleyebileceğimizi inceledik) ve bu alandaki resmi, jpg uzantılı bir resim dosyası olarak nasıl kaydedebileceğimizi incelemeye çalışacağız."
categories:
  - ado-net
tags:
  - ado-net
  - csharp
  - dotnet
  - performance
  - visual-studio
  - datatable
---
# Sql Tablolarındaki Binary Resimlere Bakmak ve Dosya Olarak Kaydetmek
Değerli Okurlarım, Merhabalar.

Hatırlayacağınız gibi bir önceki makalemizde, bir resim dosyasını sql tablosundaki Image veri tipinden bir alana nasıl yazabileceğimizi görmüştük. Bugünkü makalemizde ise, bu tablodaki Image veri tipindeki Resim alanında yer alan byte'lara nasıl okuyabileceğimizi,(Örneğimizde, PictureBox kontrolünde nasıl görüntüleyebileceğimizi inceledik) ve bu alandaki resmi, jpg uzantılı bir resim dosyası olarak nasıl kaydedebileceğimizi incelemeye çalışacağız.

Image tipindeki binary (ikili) bir alandan verileri okumak için yine stream nesnelerinden ve BinaryWriter sınıfından faydalanacağız. Visual Studio.Net ortamında, SqlClient isim uzayındaki sınıfları kullanarak Wallpapers isimli sql tablomuza eriştiğimizde, PictureBox kontrolüne, Image tipindeki Resim alanımızı bağlayamadığımızı görürüz. Bu nedenle, bu ikili (binary) alanı okuyup, PictureBox kontrolümüzün anlayacağı bir hale getirmeliyiz.

Bu amaçla, bu iki alandaki veriyi okuyucak ve bunu bellekteki bir tampon bölgeye alacağız. Daha sonra bellekte oluşturduğumuz bu MemoryStream alanını, System.Drawing.Image sınıfının FromStream metoduna parametre olarak vereceğiz. Böylece, PictureBox kontrolümüzün Image özelliği, resmin içeriğini bellekteki bu tampon alandan okuyabilecek. Dolayısıyla resmimiz gösterilebilecek.

Ancak burada dikkat etmemiz gereken başka bir husus var. O da bu ikili alanı nasıl okuyacağımız. Bu alanı ikili olarak okuyabilmek için, SqlDataReader nesnesine SequentialAccess parametresini vereceğiz. Bu parametre SqlDataReader nesnesinin, verileri sırasal bir şekilde okuyabilmesine imkan sağlamaktadır. Normalde SqlDataReader okuduğu veri satırını komple alır ve alanların indeksleri sayesinde ilgili verilere ulaşılır. Bununla birlikte SqlDataReader nesnemizin sadece ileri yönlü ve yanlız okunabilir bir veri akışı sağladığınıda hatırladığınızı sanıyorum. Bu sırasal okuma yeteneği sayesinde, makalemize konu olan tablonun, Resim adındaki ikili alanının tüm byte'larını sırasal bir şekilde okuyabilme ikmanına sahip olacağız.

Kullanacağımız teknik ise biraz uzun bir kodlama gerektirmekle birlikte, pek çok konuyada açıklık getirmektedir. Yapacağımız işlem şudur. Sql tablomuzdan kullanıcının seçtiği satıra ait Resim alanını bir SqlDataReader nesnesi ile elde etmek. Daha sonra, bu alanda sırasal bir okuma başlatıp, tüm byte'ları, BinaryWriter nesnesi yardımıyla bloklar halinde, bir MemoryStream nesnesine aktarmak. Son olarakta PictureBox kontrolümüze, bellekteki tampon bölgede tutulan bu akımı aktararak resmin görüntülenebilmesini sağlamak. MemoryStream nesneleri bellekte geçici olarak oluşturulan byte dizilerine işaret eder. Doğrudan bellekte oluşturuldukları için performans açısındanda yüksek verimlilik sağlarlar. Çoğunlukla programlarımızda oluşturduğumuz geçici dosyalar için MemorStream oldukça etkin bir yöntemdir. Diğer bir deyişle programlarımızda geçici depolamalar yapmak için idealdir.

Aynı teknik yardımıyla, kullanıcı seçtiği resmi bir dosya olarakta kaydedebilecek. Bu kez, MemoryStream nesnesi yerine, fiziki bir dosyayı temsil edicek FileStream nesnesini kullanacağız. Bu konular biraz karışık gibi görünsede, kodun içindeki detaylı açıklamalar sayesinde olayı iyice kafanızda canlandırabileceğinize inanıyorum. Şimdi dilerseniz uygulamamızın ekranını tasarlayalım ve ardından kodlarımızı yazalım.

![mk46_1.gif](/assets/images/2004/mk46_1.gif)

Şekil 1. Form Tasarımımız.

```csharp
SqlConnection conResim; /* SqlConnection nesnemizi tanımlıyoruz. */

private void Form1_Load(object sender, System.EventArgs e)
{
    conResim=new SqlConnection("data source=localhost;initial catalog=Northwind;integrated security=sspi"); /* SqlConnection nesnemizi oluşturuyor ve Norhtwind veritabanına bağlanıyoruz. */
	SqlDataAdapter daResim=new SqlDataAdapter("Select WallID,Yorum From Wallpapers",conResim); /* Wallpapers tablosundan, WallID, ve Yorum alanlarının değerlerini almak ve bunları SqlDataAdapter nesnemizin Fill metodu ile DataTable nesnemizin bellekte işaret ettiği alana aktarmak için SqlDataAdapter nesnemizi oluşturuyoruz. */
	DataTable dtResim=new DataTable("Duvarlar"); /* DataTable nesnemizi oluşturuyoruz.*/
	daResim.Fill(dtResim); /* DataTable'nesnemizi select sorgusu sonucu elde edilen veri satırları ile dolduruyoruz. */
	dgResim.DataSource=dtResim; /* DataGrid nesnemizi DataTable veri kaynağımıza bağlıyoruz. */
}

/* Yaz başlıklık buton kontrolüne tıklandığında, kullanıcının seçtiği resim sistemimize, jpg uzantılı bir resim dosyası olarak kaydedilcektir.*/
private void btnYaz_Click(object sender, System.EventArgs e)
{
 /* SqlDataReader nesnemiz, ileri yönlü bir okuma sağlamak için kullanılacak. */
	SqlDataReader drResim;
	int secilen;
	/* Kullanıcının dataGrid kontrolünde seçtiği satırın, ilk sütununu yani WallID değerini seçiyoruz. Bu değeri sql sorgumuzda, ilgili satıra ait resim alanını bulmak için kullanacağız. */
	secilen=System.Convert.ToInt32(dgResim[dgResim.CurrentCell.RowNumber,0].ToString());

	/* Sql Sorgumuzu oluşturuyoruz. Bu sorgu, seçilen satıra ait Resim alanının değerini elde etmemizi sağlıyacak.*/
	string sqlStr="Select Resim From Wallpapers Where WallID="+secilen;
	
	SqlCommand cmdResim=new SqlCommand(sqlStr,conResim); /* SqlCommand nesnemizi, sql sorgumuz ve SqlConnection nesnemiz üzerinden çalıştırılmak üzere oluşturuyoruz. */
	/* Eğer SqlConnection'ımız açık değilse açıyoruz. Nitekim SqlCommand nesnesinin içerdiği sql sorgusunun çalıştırılması açık bir bağlantıyı gerektirmektedir. */

	if(conResim.State!=ConnectionState.Open)
	{
		conResim.Open();
	}
	/* SqlDataReader nesnemizi, SqlCommand nesnemizin, ExecuteReader metodu ile dolduruyoruz. CommandBehavior.SequentialAccess parametresi sayesinde, Resim alanı üzerinde byte seviyesinde sırasal bilgi okuma imkanına sahip oluyoruz. */
	drResim=cmdResim.ExecuteReader(CommandBehavior.SequentialAccess);
	
	/* Resim alanındaki byte'ları taşıyacak bir dizi oluşturuyoruz. Bu dizinin boyutu 50. BinaryWrite nesnemiz , FileStream nesnesinin işaret ettiği dosyaya doğru bu dizideki byte'ları akıtıcak. Yani seçilen Resim alanındaki byte'ları 50 byte'lık bloklar halinde okuyacağız ve bu dizileri sırasıyla, BinaryWriter nesnemiz ile, sistemde yazmak üzere oluşturduğumuz dosyaya aktaracağız. Burada ben 50 byte'lık blokları seçimsel olarak ele aldım. Sizler bu blokları, 100 byte'lık veya 25 byte'lık veya istediğiniz bir miktarda da kullanabilirsiniz. */

	byte[] bytediziResim=new byte[50];

	/* FileStream nesnemiz ile, BinaryWriter nesnesinin okuduğu byte'ları yazıcak dosyayı oluşturuyoruz. Dosyamız sistemde daha önceden var olabilir. Bu durumda terkardan açılıp üstüne yazılır. Yok ise bu dosya oluşturulur.Diğer yandan FileAccess.Write parametresi ile dosyayı, yazmak amacıyla açtığımızı belirtiyoruz. Burada deneme olsun diye Deneme.jpg isimli bir dosya oluşturduk. Ancak dilerseniz siz, bu dosya adına WallID alanının değerinide ekleyerek benzersiz dosyalar oluşturabilirsiniz. Veya kullanıcıdan bir dosya ismi girmesini isteyebilirsiniz. Bunun geliştirilemesini siz değerli okurlarıma bırakıyorum. */
	FileStream fs=new FileStream("c:\\Deneme.jpg",FileMode.OpenOrCreate,FileAccess.Write);

	/* BinaryWriter nesnemiz, veri akışını okuduğu alandan, aldığı fs parametresinin belirttiği dosyaya doğru başlatıyor. */
	BinaryWriter bw=new BinaryWriter(fs);

	long donenBytelar;
	long baslangicIndeksi=0;

	/* SqlDataReader nesnemizin döndürdüğü satırı okumaya başlıyoruz. Sorgumuzun sadece Resim alanının değerini döndürdüğünü hatırlayalım. */
	while(drResim.Read())
	{
		/* Şimdi Resim alanından ilk 50 byte'lık bölümü okuyoruz. GetBytes metodunun aldığı ilk parametre, SqlDataReader'ın döndürdüğü veri kümesindeki Resim alanının indeks değeridir. İkinci parametre bu alanın hangi byte'ından itibaren okunmaya başlayacağıdır. Başlangıç için 0'ncı byte'tan itibaren okumaya başlıyoruz. Üçüncü parametre okunan byte'ların hangi Byte disizine yazılacağını belirtir. Dördüncü parametre bu dizi içerisine dizinin hangi indeksinden itibaren yazılmaya başlıyacağını ve beşinci parametrede okunan byte'ların, bu dizi içinde kaç byte'lık bir alana yazılacağını belirtiyor. */
		donenBytelar=drResim.GetBytes(0,0,bytediziResim,0,50);

		/* GetBytes metodu SqlDataReader nesnesinden okuyup, bytediziResim dizisine aktardığı byte sayısını geri döndürür. Bu dönen değeri donenBytelar isimli Long tipinde değişkenimizde tutuyoruz. Aşağıdaki döngüyle, okunan byte sayısı 50'ye eşit olduğu sürece, Resim alanından 50 byte'lık bloklar okunmaya devam ediyor. Okundukçada, BinaryWriter nesnemiz bu byte'ları FileStream ile açtığımız dosyaya yazıyor. Farz edelimki 386 byte'lık bir alana sahibiz. 350 byte okunduktan sonra, kalan 36 byte'ta son olarak okunur ve bundan sonrada döngünde çıkılmış olur.*/
		while(donenBytelar==50)
		{
			bw.Write(bytediziResim);
			bw.Flush();
			
			baslangicIndeksi+=50;
			donenBytelar=drResim.GetBytes(0,baslangicIndeksi,bytediziResim,0,50);
		}
		/* Bahsettiğimiz 36 bytelık kısımda son olarak buradan yazılır. */
		bw.Write(bytediziResim);
		bw.Flush(); /* Flush metodu, BinaryWriter nesnesinin o an sahip olduğu tampon hafızayı temizler. */

		/* BinaryWriter nesnemiz ve FileStream nesnemiz kapatılıyor. */
		bw.Close();

		fs.Close();
	}
	drResim.Close();
	conResim.Close();
}

/* Seçtiğimiz resmi PictureBox kontrolünde göstermek içini aşağıdaki tekniği kullanıyoruz. Bu teknikte bellekte geçici bir tampon bölgeyi MemoryStream nesnesi yardımıyla oluşturuyoruz. Bunun dışında yaptığımız işlemlerin tümü, Yaz başlıklı butona uyguladığımız kodlar ile aynı.*/

private void btnBak_Click(object sender, System.EventArgs e)
{
	SqlDataReader drResim;
	int secilen;
	
	secilen=System.Convert.ToInt32(dgResim[dgResim.CurrentCell.RowNumber,0].ToString());
	string sqlStr="Select Resim From Wallpapers Where WallID="+secilen;

	SqlCommand cmdResim=new SqlCommand(sqlStr,conResim);
	if(conResim.State!=ConnectionState.Open)
	{
		conResim.Open();
	}
	drResim=cmdResim.ExecuteReader(CommandBehavior.SequentialAccess);
	byte[] bytediziResim=new byte[50];
	MemoryStream ms=new MemoryStream();

	BinaryWriter bw=new BinaryWriter(ms);
	long donenBytelar;
	
	long baslangicIndeksi=0;
	while(drResim.Read())
	{
		donenBytelar=drResim.GetBytes(0,0,bytediziResim,0,50);
		while(donenBytelar==50)
		{
			bw.Write(bytediziResim);
			bw.Flush();
			baslangicIndeksi+=50;
			donenBytelar=drResim.GetBytes(0,baslangicIndeksi,bytediziResim,0,50);
		}

		bw.Write(bytediziResim);
		pbResim.Image=System.Drawing.Image.FromStream(ms); /* Bellekteki tampon bölgeye aldığımız, byte dizisini, PictureBox kontrolünde göstermek için, Image sınıfının FromStream metodunu kullanıyoruz. Bu metod parametre olarak aldığı akımdan gerekli byte'ları okuyarak, resmin PictureBox kontrolünde gösterilebilmesini sağlıyor. */

		bw.Flush();
		bw.Close();
		
		ms.Close();
	}

	drResim.Close();
	conResim.Close();
}
```

![mk46_2.gif](/assets/images/2004/mk46_2.gif)Şimdi uygulamamızı çalıştıralım ve herhangibir resme bakalım. Sonrada bunu kaydedelim.

Şekil 2: WallID değeri 1003 olan satırdaki Resim.

Şimdi Yaz başlıklı butona basıp bu resmi sisteme fiziki bir dosya olarak kaydedelim. Şekildende görüldüğü gibi dosyamız sistemde oluşturulmuştur. Bu dosyaya tıkladığımızda resmimizi görebiliriz.

![mk46_3.gif](/assets/images/2004/mk46_3.gif)

Şekil 3. Fiziki dosyamız oluşturuldu.

Geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.