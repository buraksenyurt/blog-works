---
layout: post
title: "Caching Mekanizmasını Anlamak - 2"
date: 2005-02-07 12:00:00 +0300
categories:
  - aspnet
tags:
  - asp.net
  - caching
---
Hatırlayacağınız gibi bir önceki makalemizde, web uygulamalarında caching mekanizmasını incelemeye başlamış ve ara belleğe alma tekniklerinden Output Cache yapısını incelemiştik. Output Cache tekniğinde bir sayfanın tamamının HTML içeriği ara belleğe alınmaktaydı. Oysa çoğu zaman sayfamızda yer alan belirli veri kümelerinin ara bellekte tutulmasını isteyebiliriz. Örneğin, bir alışveriş sitesinin pek çok kısmı dinamik olarak değişebilirken satışı yapılan ürünlerin yer aldığı kategori listeleri çok sık değişmez. Hatta uzun süreler boyunca aynı kalabilirler. İşte böyle bir durumda sayfanın tamamını ara belleğe almak yerine sadece kategori listesini sunan veri kümesini ara belleğe almak daha mantıklıdır. Data Caching olarak adlandırılan bu teknikte çoğunlukla veri kümeleri ara belleğe alınır. Data Caching tekniğinde verileri ara belleğe almak için System.Web.Caching isim alanında yer alan Cache sınıfı ve üyeleri kullanılmaktadır.

![dikkat.gif](/assets/images/2005/dikkat.gif)
Cache sınıfı sealed tipinde olup kendisinden türetme yapılmasına izin vermez. Bununla birlikte, her bir Application Domain için yanlız bir Cache nesne örneği oluşturulur ve kullanılır.

Cache sınıfına bir veri kümesini eklemek bu veriyi ara belleğe almak demektir. Bunun için aşağıdaki 4 aşırı yüklenmiş prototipe sahip olan ve bize pek çok imkan sağlayan Insert metodunu kullanabiliriz.

public void Insert (string key,object value);

public void Insert (string key, object value,CacheDependency dependencies);

public void Insert (string key,object value,CacheDependency dependencies,DateTime absoluteExpiration,TimeSpan slidingExpiration);

public void Insert (string key, object value, CacheDependency dependencies, DateTime absoluteExpiration,TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback);

Insert metodu ara belleğe alınan veri kümesinin durumuna ilişkin olarak çeşitli imkanlar sunar. Örneğin veri kümesinin ara bellekte ne kadar süre ile tutulacağı veya ne kadar süre bu veriye erişilmez ise ara bellekten silineceğinin belirlenmesi vb...Şimdi bu imkanları test etmeden önce basit olarak bir veri kümesini Cache nesnesine nasıl ekleyeceğimizi inceleyeceğiz.

```csharp
private SqlConnection con;
private SqlCommand cmd;
private SqlDataAdapter da;
private DataTable dt;

/*Categories tablosundan CategoryName alanının değerlerini alıyoruz ve bir DataTable nesnesine aktarıyoruz.*/
private void KategorileriAl()
{
    con=new SqlConnection("data source=BURKI;initial catalog=Northwind;integrated security=SSPI");
    cmd=new SqlCommand("SELECT DISTINCT CategoryName FROM Categories",con);
    da=new SqlDataAdapter(cmd);
    dt=new DataTable(); 
    da.Fill(dt);
}

private void Page_Load(object sender, System.EventArgs e)
{
    //Güncel saat bilgisini label kontrolüne yazdırıyoruz.
    Label1.Text=DateTime.Now.ToLongTimeString();
    /*Eğer ara bellekte kategori isimli Cache nesnesinin içeriği null ise bu durumda verileri çekiyoruz ve DataTable içeriğini ara belleğe Cache sınıfının Insert metodu ile alıyoruz.*/
    if(Cache["kategori"]==null)
    {
        KategorileriAl();
        Cache.Insert("kategori",dt,null,DateTime.Now.AddMinutes(5),Cache.NoSlidingExpiration); /* Veriler 5 dakika süreyle ara bellekte saklanacak daha sonra ise silinecektir.*/
    }
    /*DataGrid kontrolüne veri kaynağı olarak ara bellekteki kategori isimli Cache nesnesinin içeriğini veriyoruz. Bunu yaparken uygun türe dönüştürme işlemini yapıyoruz.*/
    DataGrid1.DataSource=(DataTable)Cache["kategori"];
    DataGrid1.DataBind();
}
```

Uygulamamızda, Northwind veritabanında yer alan Categories isimli tablodan CategoryName değerlerini çekiyoruz ve elde ettiğimiz DataTable nesnesini 5 dakika süreyle ara bellekte tutumak üzere Insert metodu ile Cache nesnesine ekliyoruz. Uygulamayı ilk çalıştırışımızda aşağıdaki ekran görüntüsünü elde ederiz.

![mk114_1.gif](/assets/images/2005/mk114_1.gif)

Sayfayı yenilediğimizde veya başka bir tarayıcı penceresinde tekrardan talep ettiğimizde sürenin düzenli olarak değiştiğini görürüz. Şimdi 5 dakika dolmadan tablodaki verilerde, örneğin Beverages alanının değerini [Beverages] olarak değiştirdiğimizi düşünelim.

![mk114_2.gif](/assets/images/2005/mk114_2.gif)

Daha sonra sayfayı tekrar çağıralım.

![mk114_3.gif](/assets/images/2005/mk114_3.gif)

Görüldüğü gibi Beverages değerini değiştirmemize rağmen yapılan değişiklikler uygulamamıza yansımamıştır. Bu, verinin ara bellekten Cache nesnesi vasıtasıyla çekildiğinin bir ispatıdır. Veri ara belleğe alındıktan 5 dakika sonra aynı sayfa tekrar talep edilir ise bu kez verinin güncel hali ekrana gelecektir. Burada veriyi Cache nesnesine alırken kullandığımız Insert metodunda Absolute Expiration (Tam Süre Sonu) zamanı belirlenmiştir. Bu süre, verinin ara bellekten kesin olarak ne zaman atılacağını söylemektedir. Bununla birlikte dilersek ara bellekte bulunan veri kümesine olan erişim sıklığına göre bir Sliding Expiration (Kayan Süre Sonu) süreside belirleyebiliriz. Buna göre,

![dikkat.gif](/assets/images/2005/dikkat.gif)
Ara bellekteki verilere belirtilen Sliding Expiration ile belirtilen süre zarfında erişilmez ise bu süre sonunda bellekten atılırlar. Eğer süre zarfı içinde ara bellekteki verilere sürekli erişiliyorsa Sliding Expiration süresi geçse dahi veriler ara bellekten atılmaz ve durumlarını korurlar.

Sürekli bellekte kalmak deyimi tabiki sistem kaynakları azalıp ara bellek verileri otomatik olarak atılınca veya web sunucusu herhangibir neden ile restart olunca geçerli değildir. Şimdi dilerseniz Sliding Expiration durumunu incelemeye çalışalım. Bunun için tek yapmamız gereken örneğimizdeki Insert metodunu aşağıdaki ile değiştirmek olacaktır.

```csharp
Cache.Insert("kategori",dt,null,Cache.NoAbsoluteExpiration,TimeSpan.FromMinutes(3));
```

Olayı daha iyi anlayabilmek için aşağıdaki şekli inceleyebiliriz.

![mk114_4.gif](/assets/images/2005/mk114_4.gif)

Sayfa ilk talep edilip veriler ara belleğa alındığında, Sliding Expiration süresinin 5 dakika ilerisini gösterecek şekilde ayarlandığını düşünelim. Bu süre dolmadan önce ara bellekteki veri tekrardan talep edilirse, Cache nesnesinin içeriğinin boşaltılma süresi şekilden de görüldüğü gibi ilerki bir zamana (o anki andan 5 dakika sonrasına) sarkacaktır.

Insert metodunun parametrelerine dikkat edecek olursanız, ara bellekteki verilerin durumlarının CacheDependency sınıfına ait nesne örnekleri ile başka bir nesneye bağlı olabileceğini görürsünüz. Bu bağımılılıkta çoğunlukla fiziki dosyalar göz önüne alınır. Örneğin, bir XML dosyasındaki veriyi ara belleğe alarak kullandığımızı düşünelim. Bu veriler pekala güncel haber başlıklarını veya bir alışveriş sitesindeki ürünlerin kategorilerini gösterebilir. XML dosyasında meydana gelecek olan güncellemeleri anında ara belleğe yansıtmak için, Cache nesnesini bu XML dosyasına bağımlı hale getirebiliriz. Şimdi bunun nasıl yapılabileceğini incelemeye çalışalım. Yukarıda geliştirdiğimiz örneğimizdeki kodlarımızı aşağıdaki gibi değiştirelim.

Kategoriler.xml dosyasının içeriği;

```xml
<?xml version="1.0" encoding="utf-8" ?> 
    <Kategoriler>
        <Tipi>Muzik CD</Tipi>
        <Tipi>Kitap</Tipi>
        <Tipi>Film DVD</Tipi>
        <Tipi>Muzik DVD</Tipi>
        <Tipi>Elektronik Eşyalar</Tipi>
        <Tipi>Bilgisayar</Tipi>
        <Tipi>Laptop</Tipi>
</Kategoriler>
```

default.aspx

```csharp
private SqlConnection con;
private SqlCommand cmd;
private SqlDataAdapter da;
private DataTable dt;
private DataSet ds;

/*KategoriAlXML metodu Kategoriler.xml dosyası içinden verileri alır ve DataSet’ e yükler.*/
private void KategoriAlXML()
{
    ds=new DataSet();
    ds.ReadXml(Server.MapPath("Kategoriler.xml"));
}

private void Page_Load(object sender, System.EventArgs e)
{
    //Güncel saat bilgisini label kontrolüne yazdiriyoruz.
    Label1.Text=DateTime.Now.ToLongTimeString();
    if(Cache["kategori"]==null)
    {
        KategoriAlXML();
        /* Cache nesnesine DataSet içerisindeki xml dosyasından okunan içeriğe ait veri kümesi yüklenir. Bu yükleme işlemi yapılırken bir CacheDependency nesnesi ile Cache nesnesinin içeriğinin güncelliği belirtilen XML dosyasına bağlanır.*/
        Cache.Insert("kategori",ds.Tables[0],new CacheDependency(Server.MapPath("Kategoriler.xml")));
    }
    /*DataGrid kontrolüne veri kaynagi olarak ara bellekteki kategori isimli Cache nesnesinin içerigini veriyoruz. Bunu yaparken uygun türe dönüstürme islemini yapiyoruz.*/
    DataGrid1.DataSource=(DataTable)Cache["kategori"];
    DataGrid1.DataBind();
}
```

Burada, Cache nesnesinin içeriğinin durumunu xml dosyamıza bağlayabilmek için CacheDependency sınıfına ait bir nesne örneği oluşturulmuştur. Burada CacheDependency sınıfına ait nesne örneği oluşturulurken yapıcı metoda xml dosyasının sanal adresi atanmıştır.

```csharp
new CacheDependency(Server.MapPath("Kategoriler.xml"))
```

Uygulamamızı çalıştırdığımızda ilk olarak aşağıdaki ekran görüntüsünü elde ederiz.

![mk114_5.gif](/assets/images/2005/mk114_5.gif)

Şimdi Kategoriler.xml dosyasının içeriğinde değişiklik yapalım ve bu değişiklikleri kaydedelim. Örneğin aşağıdaki gibi bir kaç elemanın bilgisini değştirip yeni bir eleman ekleyelim.

![mk114_6.gif](/assets/images/2005/mk114_6.gif)

Bu değişikliklerden sonra sayfayı tekrardan talep edersek, Cache nesnesinin içerdiği verilerin son yapılan güncellemelere göre yenilendiğini görürüz. Cache nesnesindeki veriyi bir dosyaya yukarıdaki gibi bağımlı hale getirdiğimizde, sayfaya yapılan her talepde dosyanın içeriğinin değişip değişmediği kontrol edilir. Eğer bir değişiklik var ise, Cache nesnesinin içerdiği veri ara bellekten atılır. Biz uygulamamızda Cache nesnesini null olup olmadığına göre yükleme yaptığımız için dosyadaki güncelleme sonucu verinin bu son halini ara belleğe almış ve DataGrid içeriğini yenilemiş oluruz.

![mk114_7.gif](/assets/images/2005/mk114_7.gif)

![dikkat.gif](/assets/images/2005/dikkat.gif)
Asp.Net 2.0’ da bir Cache nesnesinin doğrudan Sql Server üzerindeki bir tabloya bağımlı hale getirilebilmesi ve dolayısıyla veritabanı içindeki bir tabloda meydan gelecek değişiklilerin Cache nesnesine anında yanısıtlabilmesi içinde SqlCacheDependency sınıfına ait nesne örneklerinin kullanılabileceği öngörülmektedir. (Bu durumu gelecek görsel derslerimizden birisinde incelemeye çalışacağız.)

Cache nesnelerinin bellekten atılması zamana, dosyaya bağlanabileceği gibi, sistem kaynaklarının azalması durumunda da gerçekleşen bir olaydır. Sistem kaynaklarının azalması ve ara bellekteki nesnelerin atılması gerektiği durumlarda Cache nesnelerinin sahip olduğu öncelikler göz önüne alınır. Her ne sebep ile olursa olsun bir Cache nesnesinin içeriği ara bellekten atıldığında otomatik olarak çalışmasını istediğimiz metodlar bildirebiliriz. Yani CallBack metod tekniğini Cache nesneleri içinde kullanabiliriz. Örneğin ara bellekte tutulan bir nesnenin zaman aşımına uğraması nedeni ile silindiğinde otomatik olarak callback metodu devreye girerek güncel halinin tekrardan ara belleğe alınması sağlanabilir. Ya da Cache nesnesi Remove metodu ile açıkça ara bellekten atıldığı durumlarda CallBack metodlarını çalıştırabiliriz. Burada bir CallBack metodunun çağırılabilmesi için, CacheItemRemovedReason temsilcisi (delegate) tipinden bir nesne örneğinden faydalanılır. CacheItemRemovedReason temsilcisi aşağıdaki prototipe uyan metodlar işaret edebilir.

```csharp
public delegate void CacheItemRemovedCallback(string anahtar,object deger, CacheItemRemovedReason sebep);
```

anahtar parametresi Cache nesnesinin key değerine karşılık gelir. deger parametresi ise ilgili Cache nesnesinin taşıdığı veriye sahiptir. Son parametre ise CacheItemRemovedReason numaralandırıcısı (enum sabiti) tipinden bir değerdir ve CallBack metodunun çağırılma nedenini bir başka deyişle Cache nesnesinin hangi sebepten dolayı ara bellekten atıldığının belirlenmesinde kullanılır. CacheItemRemovedReason numaralandırıcısının sahip olduğu değerler aşağıdaki tabloda belirtilmektedir.

CacheItemRemovedReason Numaralandırıcı Değeri
Açıklama

DependencyChanged
Herhangibir bağımlılık nedeni ile Cache nesnesinin içeriği ara bellekten atılmıştır. Örneğin Cache nesnesine bağladığımız XML dosyasında yapılan bir değişiklik buna neden olabilir.

Expired
Cache nesnesinin içeriği zaman aşımları nedeni ile ara bellekten atılmıştır.

Removed
Cache nesnesinin içeriği Remove metodu ile açıkça ara bellekten atılmıştır. Yani programatik olarak Cache nesnesinin Remove metodu ilgili öğeye uygulanmıştır.

Underused
Sistem kaynaklarının azalması sonucunda web sunucusu, Cache nesnelerinin içeriğini sahip oldukları önem sıralarına göre ara bellekten atmaya başlamıştır.

Şimdi CallBack tekniğinin nasıl uygulandığını basit bir örnek ile incelemeye çalışalım.

```csharp
private CacheItemRemovedCallback delCallBack=null;
private SqlConnection con;
private SqlCommand cmd;
private SqlDataAdapter da; 
private DataTable dt;
private static string m_Sebep;
private static bool m_Durum=false;

private void KategorileriAl()
{
    con=new SqlConnection("data source=BURKI;initial catalog=Northwind;integrated security=SSPI");
    cmd=new SqlCommand("SELECT DISTINCT CategoryName FROM Categories",con);
    da=new SqlDataAdapter(cmd);
    dt=new DataTable(); 
    da.Fill(dt);
}

private void GeriBildirimMetodu(string anahtar,object deger,CacheItemRemovedReason sebep)
{
    m_Sebep=sebep.ToString();
    m_Durum=true;
} 
private void btnRemove_Click(object sender, System.EventArgs e)
{
    if(Cache["Nesne1"]!=null)
    { 
        Cache.Remove("Nesne1"); 
    }
}
private void btnCacheEkle_Click(object sender, System.EventArgs e)
{
    delCallBack=new CacheItemRemovedCallback(this.GeriBildirimMetodu);
    if(Cache["Nesne1"]==null)
    {
        KategorileriAl();
        Cache.Insert("Nesne1",dt,null,DateTime.Now.AddSeconds(10),Cache.NoSlidingExpiration,CacheItemPriority.High,delCallBack);
        m_Durum=false;
    }
}
private void WebForm1_PreRender(object sender, System.EventArgs e)
{
    DataGrid1.DataSource=Cache["Nesne1"];
    DataGrid1.DataBind();
    if(m_Durum)
        lblDurum.Text=m_Sebep;
    else
        lblDurum.Text="";
}
```

Yukardaki örnekte, Cache nesnesine DataTable içeriğini eklerken Insert metodunun aşağıdaki versiyonu kullanılmıştır.

```csharp
Cache.Insert("Nesne1",dt,null,DateTime.Now.AddSeconds(10),Cache.NoSlidingExpiration,CacheItemPriority.High,delCallBack);
```

Burada delCallBack, CallBack metodumuz olan GeriBildirimMetodunu işaret eden CacheItemRemovedCallback tipindeki temsilcimidir. Kullanıcı Remove işlemini gerçekleştirdiğinde veya Cache nesnesi zaman aşımı nedeni ile ara bellekten atıldığında, geri bildirim metodu çalışacaktır. Örneğimizi çalıştırdığımızda ve 10 saniyelik zaman aşımı süresi dolmadan Ekle ve daha sonra Çıkart başlıklı butonlara tıkladığımızda aşağıdaki ekran görüntüsünü elde ederiz.

![mk114_8.gif](/assets/images/2005/mk114_8.gif)

Eğer 10 saniye süresi dolduktan sonra Çıkart başlıklı butona basarsak Remove metodu çalıştırılmayacak ancak geri bildirim metodumuz çalışarak nesnenin ara bellekten atılma nedeni Expired olarak değişecektir.

![mk114_9.gif](/assets/images/2005/mk114_9.gif)

CallBack tekniğinin uygulanışını daha iyi kavrayabilmek için, örnekleri debug modunda çalıştırıp breakpoint’ ler vasıtısayıla kodları izlemenizi öneririm. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.

[Örnek uygulama için tıklayın.](/assets/files/2005/Caching2.rar)