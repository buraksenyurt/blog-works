---
layout: post
title: "Büyük Nesneler(Large Objects) ile Çalışmak"
date: 2006-07-11 12:00:00 +0300
categories:
  - csharp
tags:
  - Ado.Net-2
  - BLOB
  - sql
---
Veritabanı programcılığında zaman zaman büyük nesneler (large objects) ile çalışmak zorunda kalabiliriz. Görüntü, ses, resim, text dökümanı, çalıştırılabilir uygulamalar gibi dosyalar bir veritabanı için büyük nesne (large objects) olabilecek kaynaklardır. Bu gibi dosyaların veritabanı üzerinde alanlar (fields) içerisinde saklanabilmesi bazı özel veritabanı türleri ile mümkün olabilmektedir. Sql Server temel olarak büyük nesneleri iki kategoriye ayırmaktadır.

BLOBs adı verilen ikili formatta büyük nesneler (Binary Large Objects - BLOBs) ve karakter tabanlı büyük nesneler (Character Large Objects - CLOBs). Bununla birlikte Sql Server 2005 ile büyük nesneleri saklamak için gelen yeni veri türleride vardır. Bunlarıda genel olarak değer türünden büyük nesneler (Value Types Large Objects - VTLOs) olarak adlandırılmaktadır. Aşağıdaki tabloda Sql Server 2000 ve 2005 sürümlerinde geçerli olan büyük nesne türleri (Large Objects Types) listelenmektedir.

Nesne Çeşidi
Sql Server 2000 Veri Türü
Sql Server 2005 Veri Türü

BLOBs (Binary Large Objects)
image (maksimum 2 Gb)
image (maksimum 2 Gb)
varbinary (max)

CLOBs (Character Large Objects)
text (maksimum 2 Gb, non-unicode)
ntext (maksimum 1 Gb, unicode)
text (maksimum 2 Gb, non-unicode)
varchar (max)
ntext (maksimum 1 Gb, unicode)
nvarchar (max)
xml

Tablodan görebileceğiniz gibi Sql Server 2005 ile birlikte gelen üç yeni büyük nesne (large object) türü vardır. Varbinary (max) türü Sql Server 2000' deki image türünün benzeridir. Benzer şekilde varchar (max) ve nvarchar (max) türleride sırasıyla text ve ntext veri türlerinin yeni karşılıklarıdır. Sql Server 2005' in image, text, ntext gibi veri türlerini desteklemeye devam edeceği ancak yeni Sql sürümlerinde artık bunları kaldıracağına dair bir takım bilgiler mevcuttur. Bu nedenle Sql Server 2005 üzerinde yer verilen image, text ve ntext veri türlerinin geriye doğru uyumluluğu sağlamak amacıyla tutulduğunu düşünebiliriz. Yeni eklenen VTLOs'ların en büyük özelliklerinden birisi, büyük nesne için belirli bir boyut bildirilebilmesidir. Dolayısıyla varbinary (3000) gibi bir tanımlama yapabiliriz. Söz konusu yeni türleri birleştirebilir (concatenate), sorgulayabilir, hatta sp'lere parametre olarak geçirebiliriz. Bu imkanlar düşünüldüğünde, VLTOs eski büyük nesne türlerine nazaran önemli avantajlar içermektedir.

Dikkat edersek, büyük nesnelerin (large objects) kapladığı alanlar 2 Gb seviyesine kadar çıkmaktadır. Acaba bir veritabanı sistemi içerisinde bu denli büyük alanlar için yer ayrılabilmesinin avantajları ve dezavantajları neler olabilir? Örneğin bir elektronik ticaret sitesini göz önüne alalım. Kullanıcının etkileşimde bulunduğu ürünlere ait resim bilgileri çoğunlukla web sunucusu üzerindeki fiziki bir adreste tutulur. Sayfada resmin görünebilmesi ve en önemlisi bulunduğu tablo satırı ile ilişkilendirilmesi için çoğunlukla resmin sanal yolu (virtual path) bir field içerisinde saklanır. Bu son derece mantıklıdır. Nitekim bu tip sitelerde yer alan resimlerin boyutları genellikle çok yer tutmayacak ve tarayıcı pencerelerine hızla yüklenebilecek seviyede olurlar. Bu açıdan bakıldığında resimleri bu şekilde yol bilgileri ile tutmak son derece mantıklıdır.

Lakin bazı sistemlerde tutulan resim bilgileri çok yüksek boyutlu olabilir ve aynı zamanda tutulan içeriğin güvenliği önem arz edebilir. Örneğin, yüksek boyutlu ve sıkça değişebilen CAD çizimlerinin tutulduğu bir sistem olduğunu düşünelim. Bu çizimler sıklıkla güncelenebileceği gibi belirli kişisel dışında görülmemesi istenebilir. Ayrıca veritabanın yedeğinin alındığı durumlarda CAD çizimlerininde veritabanı ile birlikte taşınması zor olacağından, bu çizimlerin satır dahilinde tutulması ve söz konusu tablo satırları içerisinde taşınabilir olması önemli bir avantaj sağlayacaktır. Her iki örnek senaryo göz önüne alındığında dikkat edilmesi gereken bir takım ortak noktalar oluştuğu açıkça gözlemlenebilir. Önemli olan hususlar verinin taşınma kolaylığı, güvenliği ve nesnel olarak kapladığı alanı dır.

Örneğin belirli kullanıcıların asla görmemesi gereken büyük nesneler olduğunu düşünecek olursak, bunları veritabanı sisteminin güvenliğine emanet etmek son derece mantıklı bir seçim olacaktır. Diğer yandan büyük nesneler veritabanının boyutunu arttıracak ve erişim hızlarını eğer kodlamada çeşitli taktikler uygulamassak yavaşlatacaktır. Genellike 8000 byte'ın üzerine çıkılması halinde büyük nesneleri okurken veya veritabanına doğru yazarken parça parça işlemek uygulama performansını doğrudan olumlu yönde etkileyecek bir çözümdür. Öyleki çoğu istemci, çok yüksek kapasiteli nesneleri belleğe almakta sorunlar ile karşılaşabilir.

![dikkat.gif](/assets/images/2006/dikkat.gif)
Özellikle boyutu 8000 byte'ın üzerinde olan büyük nesneleri okurken veya yazarken parçalı olarak ele almak uygulama performansını arttırıcı bir etkendir. Nitekim uygulamanın çalıştığı sistem her zaman çok yüksek boyuta sahip nesneleri taşıyabilecek kapasitede donanıma sahip olamayabilir. Bu konu ile ilgili olarak resim dosyalarının sql server 2000 sisteminde [nasıl okunacağı](http://www.bsenyurt.com/MakaleGoster.aspx?ID=45) ve [nasıl yazılacağına](http://www.bsenyurt.com/MakaleGoster.aspx?ID=44) dair bilgi veren makaleleri takip etmenizi öneririm.

Microsoft.Net 2.0, Sql Server tabloları üzerindeki büyük nesneleri (large objects) ele alabilmek için SqlDataReader tipine iki yeni metod daha ilave etmiştir. GetSqlBytes ve GetSqlChars isimli metodlar, bir tablo satırındaki herhangibir büyük nesne (large object) alanını okumak için gerekli stream'leri kolayca elde etmemizi sağlarlar. Biz bu makalemizde bir örnek üzerinden giderek Sql Server 2005 üzerinde yeni veri türleri ile büyük nesnelerin nasıl saklanabileceğini ve okunabileceğini kısaca incemeleye çalışacağız.

Okuma işlemlerini gerçekleştirirken SqlDataReader nesnesi bize etkin bir performans ve verim sunmaktadır. SqlDataReader nesnesinin okuma işlemlerinde hızlı olduğunu zaten biliyoruz. Bununla birlikte SqlCommand sınıfının CommandBehaviour numaralandırıcısının (enum) değerinin SequentialAccess olması halinde, okunan satır içerisindeki büyük nesnelerin (large objects) bir akım (stream) üzerinden ister parça parça ister tamamen okunabilmesi sağlanmaktadır ki buda büyük nesnelerin okunabilmesinin en hızlı yoludur. Normal şartlarda bir satırdan veri okurken SequentialAccess davranışının verilmesi, büyük nesne alanları söz konusu olduğunda çok önemlidir. Çünkü bir SqlDataReader nesne örneği çalışma zamanında genellikle select sorgusundan dönen satırları uygulamanın çalıştığı sistemdeki belleğe anlık olarak okur ve sonraki satırdan devam eder. Dolayısıyla büyük nesne (large object) içeren bir satırın tamamını belleğe almak performans kaybına neden olabilir. SequentialAccess davranışı sayesinde, satırın tamamının belleğe alınmasının önüne geçilebilir. Çünkü alanların içeriğinin ardışıl olarak (sequential) okunabilmesine imkan sağlanır.

Gelelim Framework 2.0 ile gelen yeni metodları nasıl kullanacağımıza. Aşağıdaki akış şeması tipik olarak büyük nesne (large object) içeren bir alanın nasıl okunabileceğini özetlemektedir.

![mk167_1.gif](/assets/images/2006/mk167_1.gif)

İlk olarak yukarıdaki akış şemasında belirtilen büyük nesne (large object) okuma işlemini basit bir örnek kod üzerinden incelemeye çalışalım. Senaryo olarak Sql Server 2005 ile birlikte gelen Production isim alanında yer alan Product ve ProductPhoto tablolarını ele alacağız. Product tablosunda ürün bilgileri yer almaktadır. ProductPhoto tablosunda ise bu ürünlere ilişkin resim bilgileri varbinary (Max) tipinden alanlarda saklanmaktadır.

![mk167_4.gif](/assets/images/2006/mk167_4.gif)

Geliştireceğimiz örnek bir windows uygulaması olacak. Uygulamamızın kodlarını aşağıda bulabilirsiniz.

Form1.cs için önemli kod satırları;

```csharp
private LobsWorker wrk;

private void Form1_Load(object sender, EventArgs e)
{
    wrk = new LobsWorker();
    wrk.LoadProductInfos(grdProducts);
}

private void btnShowPicture_Click(object sender, EventArgs e)
{
    if (grdProducts.SelectedRows.Count > 0)
    {
        int photoId = Convert.ToInt32(grdProducts["ProductPhotoID", grdProducts.CurrentCell.RowIndex].Value);
        wrk.ShowPicture(pcbProductPhoto, photoId);
    }
}
```

İşleri yüklenen LobsWorker sınıfımız;

```csharp
using System;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;

namespace LOBs
{
    class LobsWorker
    {
        SqlConnection _con;
        SqlCommand _cmd;
        SqlDataReader _dr;
        SqlDataAdapter _da;

        public void LoadProductInfos(DataGridView grid)
        {
            using (_con = new SqlConnection(ConfigurationManager.ConnectionStrings["AdvConStr"].ConnectionString))
            {
                _cmd=new SqlCommand("SELECT Production.Product.ProductID, Production.Product.Name,     Production.Product.ListPrice, Production.Product.StandardCost,Production.ProductProductPhoto.ProductPhotoID FROM Production.Product INNER JOIN Production.ProductProductPhoto ON Production.Product.ProductID = Production.ProductProductPhoto.ProductID Where Production.Product.Size is not Null",_con);
                _da = new SqlDataAdapter(_cmd);
                DataTable dt = new DataTable();
                _da.Fill(dt);
                grid.DataSource = dt;
            }
        }
        public void ShowPicture(PictureBox pb, int photoId)
        {
            using (_con = new SqlConnection(ConfigurationManager.ConnectionStrings["AdvConStr"].ConnectionString))
            {
                using (_cmd = new SqlCommand("Select ProductPhotoID,LargePhotoFileName,LargePhoto From         Production.ProductPhoto Where ProductPhotoID=@PhotoID", _con))
                {
                    _cmd.Parameters.AddWithValue("@PhotoID", photoId);
                    _con.Open();
                    _dr = _cmd.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess);
                    if (_dr.Read())
                    {
                        pb.Image = System.Drawing.Image.FromStream(_dr.GetSqlBytes(2).Stream);
                    }
                    _dr.Close();
                }
            }
        }
    }
}
```

Kullanıcı DataGridView kontrolüden bir satır seçip Resim Göster isimli düğmeye tıkladığında, seçilen satırdaki ProductPhotoID alanının değeri ele alınaraktan ürün resmi bir PictureBox kontrolünde gösterilmektedir. Yardımcı sınıfımız olan LobsWorker içerisindeki en önemli üye ShowPicture isimli metoddur. Bu metod parametre olarak resmin gösterileceği PictureBox bileşeninin referansını ve DataGridView kontrolünde seçilen satırdaki ProductPhotoID alanının değerini almaktadır.

Metod içerisinde SqlDataReader sınıfına ait nesne örneğinin ExecuteReader metodu çalıştırıldıktan sonra elde edilen satırdaki LargePhoto isimli alanın değeri GetSqlBytes metodu ile alınmaktadır. GetSqlBytes isimli metodun döndürüğü referans tipinin Stream isimli bir özelliği vardır. Dolayısıyla ürün resmini göstermek için tek yapmamız gereken elde edilen Stream'i PictureBox kontrolünün Image isimli özelliğine atamak olacaktır. Uygulamanın bu haliyle örnek çıktısını aşağıda görebilirsiniz.

![mk167_3.gif](/assets/images/2006/mk167_3.gif)

Benzer süreç döküman tipindeki kaynaklar içinde geçerlidir. Tek fark GetSqlBytes metodu yerine GetSqlChars metodunu kullanmak olacaktır. Ancak GetSqlChars'ın Stream isimli bir özelliği yoktur. Bunun yerine doğrudan string olarak içeriği elde edebiliriz. Bununla birlikte GetSqlChars metodu ile elde edilen içeriği karakter dizisi şeklinde ele almakta mümkün olabilir. Böylece okuma işlemlerinin örnek olarak bir resim formatı için nasıl yapılabileceğini kısaca incelemiş olduk. Benzer işlemleri ikili formatta kaydedilebilecek pek çok dosya için yapabilirsiniz.

Gelelim yazma işlemlerine. Tipik olarak görüntü, resim, ses veya döküman formatındaki kaynakları tablolar üzerindeki ilgili alanlara yazmak için aşağıdaki akış şemasındakine benzer bir yol izleyebiliriz.

![mk167_2.gif](/assets/images/2006/mk167_2.gif)

Yazma işleminide inceleyeceğimiz basit bir örnek ile makalemize devam ediyoruz. İçerisinde bir şehire ait resim ve tarihçe bilgilerini içeren bir tablomuz olduğunu düşünelim. Tablomuzun yapısı aşağıdaki şekilde görüldüğü gibi olacaktır. Dikkat ederseniz Resim, Tarihce alanlarının veri tipleri sırasıyla varbinary (Max) ve nvarchar (Max) olarak seçilmiştir.

![mk167_5.gif](/assets/images/2006/mk167_5.gif)

Örnek uygulamamızda LobsWorker sınıfı içerisinde yer alan aşağıdaki InsertCity isimli metodumuz, seçilen resim dosyası ve döküman bilgilerini (örnek olarak text formatı baz alınmıştır) ele alarak Sehirlerimiz isimli tabloya satır ekleme işlemini gerçekleştirmektedir.

```csharp
public void InsertSehir(string sehir,string imagePath, string docPath)
{
    using (_con = new SqlConnection(ConfigurationManager.ConnectionStrings["AdvConStr"].ConnectionString))
    {
        using(_cmd=new SqlCommand("Insert into Sehirlerimiz (Sehir,Resim,Tarihce) Values (@Sehir,@Resim,@Tarihce)",_con))
        {
            _cmd.Parameters.AddWithValue("@Sehir", sehir);
            _cmd.Parameters.Add("@Resim", SqlDbType.VarBinary);
            _cmd.Parameters.Add("@Tarihce", SqlDbType.NVarChar);
            
            FileStream fsPic = new FileStream(imagePath,FileMode.Open,FileAccess.Read);
            BinaryReader br = new BinaryReader(fsPic);
            _cmd.Parameters["@Resim"].Value=br.ReadBytes((int)fsPic.Length);
            
            FileStream fsDoc = new FileStream(docPath, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fsDoc);
            _cmd.Parameters["@Tarihce"].Value = sr.ReadToEnd();
            
            _con.Open();
            _cmd.ExecuteNonQuery();
        }
    }
}
```

![mk167_6.gif](/assets/images/2006/mk167_6.gif)

InsertCity metodunu inceleyecek olursak jpg uzantılı resim dosyalarını varbinary (max) tipinden olan @Resim isimli parametreye aktarırken bir BinaryReader sınıfı nesne örneğinden yararlanıyoruz. Nitekim varbinary (max) tipinin beklediği verinin byte dizisi şeklinde olması gerekmektedir. Diğer taraftan txt uzantılı döküman dosyamızı nvarchar (max) tipinden alana eklemek içinse bu kez bir StreamReader sınıfı nesne örneğinden yararlanıyoruz. Bu kez aktarmamız gereken verinin string tipinden olması yeterlidir. Örneğimizi denediğimizde seçilen verilerin tabloya başarılı bir şekilde eklenmiş olduklarını görebiliriz.

![mk167_7.gif](/assets/images/2006/mk167_7.gif)

Görüldüğü gibi yeni gelen metodlar, büyük nesneler ile çalışırken oldukça büyük kolaylıklar getirmiştir. Buna rağmen 8000 byte sınırını bizim için önemli bir çizgidir. Genellikle bu değeri aşan kaynakları saklarken parça parça okuma ve yazma yöntemleri tercih edilmelidir. Bu yöntemlerden biriside Sql Server üzerinde UpdateText ve TextPtr anahtar sözcüklerinin kullanıldığı tekniktir. Bu tekniği ilerleyen yazılarımızda (yada bir görsel dersimizde) incelemeye çalışacağız. Son olarak, büyük nesnelerin veritabanında saklanmasının güvenlik, kontrol edilebilirlik ve taşınabilirlik açısından büyük avantajlar sağladığını ancak optimum performans için ekstra çaba gerektirdiğinide göz ardı etmemekte fayda olacağını söyleyelim. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama İçin Tıklayın.](/assets/files/2006/LOBs.rar)