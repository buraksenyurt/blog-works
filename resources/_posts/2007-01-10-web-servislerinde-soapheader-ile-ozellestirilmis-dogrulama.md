---
layout: post
title: "Web Servislerinde SoapHeader ile Özelleştirilmiş Doğrulama"
date: 2007-01-10 12:00:00 +0300
categories:
  - xml-web-services
tags:
  - xml-web-services
  - csharp
  - dotnet
  - xml
  - soap
  - web-service
  - http
  - authentication
  - authorization
  - caching
---
Web servislerinde güvenlik söz konusu olduğunda, geliştiricileri en çok zorlayan noktalardan birisi görsel bir arabirimin olmayışıdır. Bu nedenle özellikle web tabanlı uygulamalarda tercih edilen form tabanlı (form-based) veya windows tabanlı (windows based) doğrulama (authentication) sistemlerini uygulamak biraz daha farklıdır. Biz bu makalemizde form tabanlı doğrulama ve yetkilendirme sistemininin iskeletini, web servisleri üzerinde nasıl geliştirebileceğimizi incelemeye çalışacağız.

Konuyu daha iyi anlamak için vakkamızı analiz edelim. Web servisi tarafında yer alan bazı metodların istemciler tarafından kullanılabilmesi için ilgili kullanıcıların doğrulanması gerektiğini düşünelim. Böyle bir durumda kullanıcıların ilgili web metod çağrılarından önce web servisi tarafından doğrulanması (authenticate) gerekecektir. Sırf bu amaç için tasarlanmış bir web metodu göz önüne alınabilir. Kullanıcıların login metodu ile doğrulanmasının en büyük amacı, iş yapan web metodlarına yapılacak olan çağrılarda tekrar tekrar doğrulama yapılmasının da önüne geçmektir. Peki bu nasıl sağlanabilir?

Eğer kullanıcının elinde bir güven belgesi (credential) olursa bu işlevsellik çok kolay bir şekilde sağlanabilir. Login metodu içerisinde hazırlanacak olan bu güven belgesini, web servisinin kendisine bağlanan istemcileri doğrulaması halinde vereceği bir bilet olarakta düşünebiliriz. Bilet bir kere oluşturulduktan sonra istemciye gönderilecektir. Bu noktadan sonra istemci uygulama,web servisi tarafından kendisine verilen bu bilet ile web metodlarını çağıracaktır. Böylece bir kere doğrulanan kullanıcının, bu işlemden sonra yapacağı çağrılarda tekrardan doğrulanmasına gerek kalmayacaktır. Bu elbette bilet bilgisi korunduğu sürece geçerli olacaktır. Istemci uygulamanın kapatılması ve tekrar başlatılması halinde, web servisi tarafından yeni bir biletin hazırlanması ve gönderilmesi gerekebilir.

Burada önemli olan bir sorun vardır. İstemciler için sunucu tarafında oluşturulan biletler, istemci uygulama ve web servisi arasında nasıl taşınacaktır? İşte burada web servisleri ile istemciler arasında hareket eden verinin taşındığı Soap zarfları (Soap Envelope) devreye girmektedir. Soap zarflarının iki önemli parçası vardır. Asıl verinin taşındığı gövde (Soap Body) kısmı ve paket ile birlikte taşınabilecek ekstra bilgilerin yer aldığı başlık (Soap Header) kısmı. Soap zarfının başlığında bilet bilgisini taşıyabiliriz. Böylece kullanıcı, web servisi tarafından ilk doğrulanışından sonra Soap paketindeki başlık bilgisi ile kendisine gelen bileti alabilir. Bundan sonra yapacağı web metodu çağrılarında ise kendisine verilen bileti yine web servisine Soap başlığında gönderecektir.

Şimdi gelin biraz daha teknik detaya inelim ve örnek bir uygulama üzerinden hareket ederek form tabanlı doğrulama iskeletini web servisleri için oluşturmaya çalışalım. Web servisi tarafında düşünmemiz gereken ilk konu kullanıcı bilgilerini nerede saklayacağımızdır. Kullanıcıların sistem tarafından tanınması için bazı bilgilerinin herhangibir depolama alanında saklanması gerekecektir. Bu amaçla.Net 2.0 ile birlikte gelen Membership API'sini kullanabiliriz yada kendi hazırlayacağımız veri saklama ambarlarını göz önüne alabiliriz. Bu ambarlar bir Access veritabanı hatta bir Xml dosyası bile olabilir. Ancak verilerin daha tutarlı ve güvenli bir şekilde saklanması amacıyla ilişkisel veritabanı (Relational Database Management System - RDMS) sistemlerinden birisi olması çok daha ölçeklenebilir çözümler elde etmemizi sağlayacaktır. Makalemize konu olan örneğimizde bu amaçla basit bir sql veritabanı dosyası kullanıyoruz.

![mk187_1.gif](/assets/images/2007/mk187_1.gif)

Azondb.mdf veritabanı dosyamızın içerisinde şu an için tek bir tablo yer almaktadır. Kullanicilar isimli tablomuzun temel amacı, doğrulaması yapılacak olan üyelere ait bazı bilgileri saklamaktır. Örneğin kullanıcıların adı, mail adresi, şifresi gibi.

![mk187_2.gif](/assets/images/2007/mk187_2.gif)

Tablomuzun veri yapısını oldukça basit düşünüyoruz. Nitekim odaklanmamız gereken nokta tablo yapılarından ziyade web servisi üzerinde güvenliği form tabanlı sistem kurallarına göre tasarlamak. Ama elbetteki rol yönetimininin (role management) var olduğu, şifrelerin de şifrelenerek (en azından Hash algoritması ile karıştırılarak) tutulduğu daha güçlü bir veri saklama ortamı düşünülebilir. Var olan Membership API'si buna en güzel örnektir.

Gelelim web servisi tarafımıza. Az öncede bahsettiğimiz gibi, form tabanlı doğrulama sisteminde iskeleti oluşturan önemli noktalar, kullanıcının sunucu tarafından doğrulanması (authentication), bir biletin oluşturulması ve bu biletin Soap başlığında istemciye gönderilmesidir. Öncelikle kullanıcı bilgilerinden bazılarını web metodları arasında bulmamızı sağlayacak bir sınıf tasarlayarak işe başlayalım.

![mk187_3.gif](/assets/images/2007/mk187_3.gif)

```csharp
public class Kullanici
{
    private string _ad;
    private string _email;
    private string _biletNumarasi;

    public string Ad
    {
        get { return _ad; }
    }
    public string Email
    {
        get { return _email; }
    }
    public string BiletNumarasi
    {
        get { return _biletNumarasi; }
    }
    public Kullanici(string kullaniciAdi,string email)
    {
        _ad = kullaniciAdi;
        _email = email;
        _biletNumarasi = Guid.NewGuid().ToString();
    }
}
```

Kullanici isimli sınıfın en can alıcı noktası bir Global Unique Identifier (GUID) değerini sunmasıdır. Bu değer Kullanici isimli sınıfa ait bir nesne örneklendiğinde yapıcı metod içerisinde oluşturulmaktadır. GUID değerleri web servisinin çalıştığı sistemde benzersiz olarak oluşturulduklarından, her kullanıcının ayrı ayrı ele alınmasını sağlayabiliriz. Örneğimizde, Kullanici sınıfına ait nesne örneklerini Application nesnesinde taşımayı tercih edeceğiz. Application nesnesi web servisi uygulaması içerisinde heryerden erişilebilir bir nesnedir.

Dolayısıyla web metodlarımız içerisinden Application nesnelerine çıkıp gelen bilet numarasına ait bir Kullanici nesne örneğinin olup olmadığını kontrol edebiliriz. Yani, doğrulanan kullanıcıları web servisi tarafından Application nesnesinde tutup, istemci ile Soap başlığından gelecek olan biletin numarasına göre kontrol edebiliriz. Böylece web metodları içerisinde eğer istemcideki bilet numarasının karşılığı olan bir kullanıcı var ise işlemlerin yapılmasını sağlayabiliriz. Ama öncesinde Soap zarfının başlığında taşınacak olan sınıfın tasarlanması gerekmektedir.

![mk187_4.gif](/assets/images/2007/mk187_4.gif)

```csharp
public class Bilet:System.Web.Services.Protocols.SoapHeader
{
    private string _biletNumarasi;

    public string BiletNumarasi
     {
        get { return _biletNumarasi; }
        set { _biletNumarasi = value; }
     }  

    public Bilet(string numara)
    {
        _biletNumarasi = numara;
    }

    public Bilet()
    {
    }
}
```

Soap zarflarının başlık kısmında bilgi taşıyabilmenin yolu SoapHeader tipinden türetilmiş bir sınıf yazılmasıdır. Bu türetme sayesinde Bilet isimli sınıfa ait nesne örneklerini soap paketlerinde başlık kısmında taşıyabilme imkanına sahip oluruz. Olusturulan bu sınıf, web servisini referans eden istemci uygulamalarada gönderilecektir. Aynı zamanda Soap paketi içerisine serileştirilebilir olması gerekmektedir. Bu nedenlede varsayılan yapıcı metodu (default constructor) olmak zorundadır. Dikkat ederseniz başlıkta taşınacak olan bu sınıf sadece tek bir üye sunmaktadır. BiletNumarasi. Bilet isimli sınıfın BiletNumarasi isimli özelliğinin bir set bloğuna sahip olması önemlidir. Aksi halde istemci tarafındaki uygulama sisteme login olduktan sonra yapacağı çağrılarda bu bilgiyi tekrardan set edemez ve web servisi tarafına gönderemez. Gelelim web servisimiz içerisinde neler yapacağımıza.

![mk187_5.gif](/assets/images/2007/mk187_5.gif)

```csharp
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class UrunServisi : System.Web.Services.WebService
{
    public Bilet _bilet;

    [WebMethod(Description="Kullanıcı bileti oluşturulmasını sağlar")]
    [SoapHeader("_bilet", Direction=SoapHeaderDirection.Out)]
    public void GirisYap(string kullaniciAdi, string sifre)
    {
        using(SqlConnection conn=new SqlConnection(@"data source=.\SQLEXPRESS;Integrated Security=SSPI;AttachDBFilename=|DataDirectory|AzonDb.mdf;User Instance=true"))
        {
            SqlCommand cmd = new SqlCommand("Select KullaniciAdi,PostaAdresi From Kullanicilar Where KullaniciAdi=@KullaniciAdi and Sifre=@Sifre", conn);
            cmd.Parameters.AddWithValue("@KullaniciAdi", kullaniciAdi.ToString());
            cmd.Parameters.AddWithValue("@Sifre", sifre.ToString());
            conn.Open();
            SqlDataReader dr=cmd.ExecuteReader();
            if (dr.Read())
            {
                Kullanici kln = new Kullanici(dr["KullaniciAdi"].ToString(), dr["PostaAdresi"].ToString());
                Application[kln.BiletNumarasi] = kln;
                _bilet = new Bilet(kln.BiletNumarasi); 
            }
            else
                throw new Exception("Geçersiz kullanıcı");
        }
    }

    private Kullanici KullaniciDogrula(string biletNumarasi)
    {
        Kullanici kln = (Kullanici)Application[biletNumarasi];
        if (kln != null)
            return kln;
        else
            throw new Exception("Geçersiz bilet numarası"); 
    }

    [WebMethod(Description="Ortalama endeks hesaplamaları yapılır.")]
    [SoapHeader("_bilet",Direction=SoapHeaderDirection.In)]
    public double MaliyetHesapla(double parcaNo)
    {
        KullaniciDogrula(_bilet.BiletNumarasi);
        return parcaNo*10;
    }
    public UrunServisi () {
    } 
}
```

Web servisimiz içerisinde GirisYap isimli bir metodumuz bulunmaktadır. Bu aslında makalemizin başından beri belirttiğimiz, kullanıcların doğrulanması sağlayan login metodudur. Parametre olarak aldığı ve istemci uygulamadan gelen kullanıcı adı ve şifre bilgilerine göre doğrulama işlemini üstlenir. Yanlız metodun yaptığı iki önemli atama vardır. Birincisinde, Kullanici sınıfına ait bir nesne örneğinin oluşturulması ve üretilen GUID değerine göre Application nesnesine atanması söz konusudur.

```csharp
Kullanici kln = new Kullanici(dr["KullaniciAdi"].ToString(), dr["PostaAdresi"].ToString());
Application[kln.BiletNumarasi] = kln;
```

Böylece kullanıcı eğer doğrulanırsa web servisi kendisine bağlı olan istemciye ait bilgilere Application nesnesi üzerinden bilet numarasini kullanarak erişebilecektir. İkincisinde ise, üretilen bilet numarasına göre Bilet sınıfından _bilet isimli nesne örneği oluşturulur.

```csharp
_bilet = new Bilet(kln.BiletNumarasi);
```

Dikkat ederseniz Bilet isimli sınıfa ait bu nesne örneği Soap başlığında taşınmaktadır. Ancak dikkat edilmesi gereken bir nokta vardır. GirisYap isimli metod için belirtilen SoapHeader niteliği (attribute). Bu niteliğin ilk parametresi ile, metod içerisinde oluşturulan _bilet isimli nesne örneğinin Soap başlığında taşınacağı belirtmektedir. İkinci parametre ilede, bu bilginin web servisi tarafından istemciye gidecek olan soap paketlerindeki başlık içeriğine ekleneceği belirtilmektedir. Bu yönü belirlemek için SoapHeaderDirection enum sabitinden faydalanılmaktadır.

KullaniciDogrula isimli metodumuz sadece bu servis sınıfı içerisinde kullanılan bir üyedir. Dışarıyla bir bağlantısı olmadığından private olarak tanımlanmıştır. Görevi, metoda parametre olarak gelen bilet numarası değerine sahip bir Application değişkeninin olup olmadığını tespit etmektir. Eğer yoksa bir istisna nesnesi ortama, daha doğrusu istemci uygulamaya fırlatılır. Metodun geriye Kullanici sınıfı tipinden bir değer döndürmesinin tek nedeni, bu metodu kullanan web metodlarından, ilgili işlemi gerçekleştiren kullanıcıya ait bazı temel bilgilere ulaşılabilmesini sağlamaktır. Bu çoğunlukla o işlemi yapan kullanıcı için loglama yapılması istendiği durumlarda yada rolüne veya yetkisine göre ilgili işlemi yapabilip yapamayacağına karar verilmesi gibi durumlarda kullanılabilir.

Gelelim iş yapan web metodumuza. Metodun ne iş yaptığı çok önemli değil şu aşamada. Dikkat etmemiz gereken nokta, metodun hizmeti nasıl verdiği. MaliyetHesapla isimli metodumuzun da SoapHeader niteliği (attribute) tanımlanmıştır. GirisYap metodundakine göre tek fark yönüdür. Dikkat ederseniz SoapDirection.In değeri kullanılmaktadır. Bu, ilk parametrede belirtilen _bilet isimli nesne örneğinin, istemci uygulamadan web servisine gelecek olan Soap paketi içerisindeki başlık bilgisinde yer alacağını belirtir. Eğer kullanıcı bu metodu çağırmadan önce sisteme giriş yapmışsa elinde bir bilet numarası vardır ve Soap paketi ile bunu web servisindeki ilgili metoda ulaştırabilir. Metod içerisinde KullaniciDogrula isimli metod çağırılır ve eğer Application nesnesi üzerinden gelen bilet numarasına sahip bir nesne var ise söz konusu metod yürütülür. Ama böyle bir bilet numarası yoksa zaten istemciye KullaniciDogrula metodu içerisinden bir istisna fırlatılacaktır.

Artık istemci tarafında bir test uygulaması yazarak sistemi sınayabiliriz. Olayı daha basit düşünebilmek için bir Console uygulaması üzerinden hareket edeceğiz. Uygulamamıza öncelikle web servisimizin referansını eklememiz gerektiğini unutmayalım.

![mk187_6.gif](/assets/images/2007/mk187_6.gif)

Web referansının eklenmesi ile birlikte istemci uygulama tarafında Soap başlığında taşınabilecek olan Bilet tipine ait bir sınıfta eklenecektir.

![mk187_7.gif](/assets/images/2007/mk187_7.gif)

Dikkat ederseniz Bilet sınıfına ait nesne örneğini elde etmemizi sağlayan bir özellikte (property) UrunServisi isimli proxy sınıfımıza dahil edilmiştir. BiletValue özelliği Soap başlığında taşınan Bilet nesne örneğini okuyabilmemizi hatta atama yapabilmemizi sağlar. Bu kısa bakışın hemen ardından aşağıdaki kod satırlarını Console uygulamamızın Main metodu içerisine yazarak örneğimizi geliştirmeye devam edelim.

```csharp
static void Main(string[] args)
{
    try
    {
        UrunWebSrv.UrunServisi urnSrv = new Istemci.UrunWebSrv.UrunServisi();
        urnSrv.GirisYap("bsenyurt", "1234");
        Console.WriteLine("Sistem tarafından verilen unique Id \n {0}",urnSrv.BiletValue.BiletNumarasi);
        double sonuc=urnSrv.MaliyetHesapla(1000);
        Console.WriteLine("Maliyet "+sonuc.ToString()); 
    }
    catch(Exception err)
    {
        Console.WriteLine(err.Message.ToString());
    }
    Console.ReadLine();
}
```

Dikkat ederseniz MaliyetHesapla isimli web metodumuzu çalıştırmadan önce sisteme giriş yapıyoruz. Bunun için GirisYap metodunu çağırıyoruz. Örnek olması açısından Kullanicilar tablosunda bsenyurt isimli bir kullanci oluşturduk ve 1234 şifresini verdik. (Şifrelerin bu şekilde açık olarak gitmesi ve hatta tabloda bu şekilde açık olarak saklanması elbette doğru değil. Burada gerekirse encryption mekanizmalarında faydalanılması çok daha güvenli bir sistem oluşturulmasını sağlayacaktır. Ancak böyle bir durumda istemci tarafındaki uygulamayıda bizim geliştirmemiz gerekecektir ki bu da web servisi kullanımını anlamsız kılmaktadır. Çözüm olarak Remoting tercih edilebilir yada WSE 3.0 ile birlikte gelen güvenlik sistemleri göz önüne alınabilir.) Kullanıcı eğer web servisi tarafından doğrulanırsa, üretilen bilet numarasını servise ait nesne örneğinden BiletValue isimli özellik yardımıyla elde ebiliriz. Sonrasında ise MaliyetHesapla isimli metodumuzu çağırmaktayız. Uygulamamızı test ettiğimizde aşağıdakine benzer bir ekran görüntüsü elde ederiz.

![mk187_8.gif](/assets/images/2007/mk187_8.gif)

Eğer programı bir kere daha çalıştırırsak farklı bir GUID değeri ile karşılaşırız. Aşağıdaki ekran çıktısında olduğu gibi.

![mk187_9.gif](/assets/images/2007/mk187_9.gif)

Bu son derece doğaldır. Çünkü uygulama her çalıştığında Login metodu devreye girmekte ve kullanıcıyı bulup yeni bir GUID ürettirmektedir. Dolayısıyla söz konusu doğrulama sistemi uygulamanın açık olması halinde, birden fazla web metodu çağırıldığında daha efektif işleyecektir. Böyle bir durumda login olunduktan sonra üretilen GUID değeri, uygulamanın devamındaki tüm web metod çağırılarında aynı kalacaktır. Elbette sisteme giriş yapamayan bir kullanıcı ile karşılaşıldığında GirisYap metodu, istemci tarafına bir istisna fırlatacaktır. Örneğin var olmayan bir kullanıcı ile console uygulamamızı denediğimizde aşağıdaki gibi bir sonuçla karşılaşabiliriz.

![mk187_10.gif](/assets/images/2007/mk187_10.gif)

Tasarlanan bu sistem her ne kadar kullanışlı görünsede bazı dezavantajları vardır. Herşeyden önce web servisinin çalıştığı sunucunun çökmesi halinde tüm Application nesneleri kaybolacaktır. Hatta çökmesi haricinde, web uygulamasının herhangibir nedenle yeniden başlatılması halinde de aynı durum söz konusudur. Buna sebep olarak yeniden derlemeyi (compiling) ya da yeniden dağıtmayı (publishing) gösterebiliriz. Dolayısıyla sisteme giriş yapan kullanıcı bilgilerini daha sağlam bir yerde saklamak isteyebiliriz.

Bu durumda Application nesnesi yerine belki bir veritabanı tablosu göz önüne alınabilir. Ayrıca performansı arttırmak amacıyla ara bellekleme (caching) tekniklerinden de yararlanılabilir. Sonuç itibariye bu iyileştirmeler yapılsada yapılmasada iskelet aynı kalmak zorundadır. Kullanıcıyı bir şekilde doğrulamak (authenticat) ve metod çağrılarından tekrar kullanıcı bilgisi istememek için bilet kullanmak akıllıca bir yoldur. Buna birde rol tabanlı yetkilendirme gibi özellikler eklendiğinde form tabanlı doğrulama sisteminin iskeleti tamamlanmış olacaktır. Bu makalemizde web servislerinde, soap üzerinden form tabanlı doğrulama sistemini nasıl gerçekleştirebileceğimizi incelemeye çalıştık. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.