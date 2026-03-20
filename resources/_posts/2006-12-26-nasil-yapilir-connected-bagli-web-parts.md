---
layout: post
title: "Nasıl Yapılır : Connected(Bağlı) Web Parts"
date: 2006-12-26 12:00:00 +0300
categories:
  - aspnet-2-0
tags:
  - aspnet-2-0
  - csharp
  - dotnet
  - aspnet
  - sql-server
  - delegates
  - visual-studio
  - datatable
---
Web uygulamalarımızda kullanabileceğimiz sayısız sunucu tabanlı kontrol vardır. Geliştirdiğimiz uygulamalarda bazı durumlarda birden fazla kontrolün kullanıldığı web sayfalarımız söz konusu olabilir. Bu tarz durumlarda kodları merkezileştirmek, istenen kontrolleri tek bir noktadan güncelleyebilmek amacıyla kullanıcı web kontrollerine (web user control) başvurabilir yada kendi web sunucu kontrollerimizi yazabiliriz. Özellikle kendi web sunucu kontrollerimizi var olan bir kontrolden türetme (inheritance) yardımıyla geliştirebileceğimiz gibi sıfırdan da yazabiliriz. (Kendi web kontrollerimizi oluşturmak ile ilgili olarak ilerleyen zamanlarda bir makale hazırlamayı düşünüyorum.)

Asp.Net 2.0 mimarisi ile birlikte sayfaya bağlı olan kullanıcılara göre özelleştirebileceğimiz Web Part kavramı ile karşılaştık. Web Part kontrolleri birer sunucu kontrolü olmakla birlikte, özelliklerini kişileştirebilme ve farklı kullanıcılar için farklı veriler taşıyabilme yeteneğine sahip kontroller olarak karşımıza çıkmaktadır. Aslında SharePoint mimarisini kendi web uygulamalarımızda kullanabilme amacıyla tasarlanmış kontrollerdir.

Örneğin web part kontrolleri sayfa üzerinde sürükleme, minimize etme gibi görsel işlemleri gerçekleştirebiliriz ki buda SharePoint tarzı portal uygulamalarının en büyük özelliklerindendir. Web Part kontrolleri, kişiselleştirme (Personalization) içinde önemli kabiliyetler sunmasına rağmen bazı noktalarda kullanımı oldukça karmaşık olabilmektedir. Örneğin web part kontrolleri arasında veri iletişiminde bulunmak bir başka deyişle, birbirleriyle haberleşebilen web part kontrolleri geliştirmek. İşte bu makalemizde web part kontrolleri arası bağlantıların nasıl sağlanabileceğini adım adım incelemeye çalışacağız.

Özellikle kendi geliştireceğimiz Web Part kontrollerinde, kontroller arasında bilgi taşımak isteyeceğimiz durumlar söz konusu olabilir. Bu istek, var olan kontroller düşünüldüğünde çok da özel bir konu değildir. Ama işin içerisine kişiselleştirilebilir, bir Web Part Zone'dan diğerine sürüklenebilir, minimize edilebilir vb Web Part kontrolleri girdiğinde farklı bir teknik kullanmak gerekmektedir. Örneğin herhangibir veri içeriği üzerinde arama yapmamızı sağlayacak bir web part kontrolünün başka bir web part kontrolüne aranacak değeri aktarmasını istediğimizi düşünelim. Verinin aktarıldığı Web Part kontrolü üzerinde de arama yapılan değere göre ilgili sonuçların gösterilmesini isteyebiliriz. Peki bu tarz bir ihtiyaç için gerekenler nelerdir?

Kendi Web Part kontrollerimizden bahsettiğimiz için WebPart tipinden türeyen sınıflar geliştirmemiz gerekiyor. Bu web part kontrollerimizden birisi diğer web part kontrolü için veri sağlayıcı nitelik olucakken diğeride bu veriyi tüketici nitelikte olacaktır. Genellikle veriyi sağlayacak olan web part kontrolü Provider olarak adlandırılmaktadır. Provider Web Part kontrolünün sunacağı içeriği kullanacak olan diğer Web Part kontrolümüz ise Consumer (tüketici) olarak adlandırılmaktadır. Provider görevini üstlenen bir Web Part kontrolünü birden fazla Consumer kullanabilir. Provider Web Part kontrolünün göndereceği veriyi, Consumer niteliğinde olan bir Web Part kontrolünün (lerinin) alabilmesi için, Provider Web Part kontrolüne bir arayüz (interface) uygulanması gerekmektedir.

Bu arayüz (Interface), Provider Web Part kontrolünden Consumer Web Part kontrolüne nesne referansı taşınmasında rol almaktadır. Böylece ilgili referans üzerinden Provider Web Part kontrolü ile gelen veriyi taşıma ve Consumer Web Part kontrolünden (lerinden) alma şansına sahip olabiliriz. Son olarak her iki web part kontrolü arasında arayüz yardımıyla refarans taşıyabilmek için bağlantı noktalarına (Connection Part) ihtiyacımız olacaktır. Bağlantı noktaları aslında özel nitelikler (attribute) ile imzalanmış metodlardır. Aşağıdaki şekilde ihtiyacımız olanlar özetlenmeye çalışılmıştır.

![mk185_1.gif](/assets/images/2006/mk185_1.gif)

Artık bir örnek üzerinden adım adım ilerleyerek Web Part kontrolleri arası nasıl veri taşıyabileceğimizi incelemeye başlayabiliriz. İlk olarak örnek bir senaryo göz önüne alalım. Provider (Sağlayıcı) görevini üstlenecek kontrolümüzde arama işlemi için veri girilebilecek bir kutucuk olduğunu ve buraya bir ürün adı yazılabileceğini düşünelim. (Ürünlerimizi Sql Server 2005 ile gelen AdventureWorks veritabanındaki Product tablosundan çekebiliriz.) Bu noktadan sonra arama sözcüğü Consumer görevini üstlenecek olan Web Part kontrolüne aktarılacak ve bulunan bilgiler bir GridView kontrolü içerisinde gösterilecektir. Senaryo gereği en az iki web part kontrolüne ihtiyacımız olacak. Bu kontrollerin en büyük özelliği WebPart sınıfından türeyecek olmalarıdır. Böylece içlerine kişiselleştirilebilir üyeler dahil edebilir, aynı zamanda birbirlerine bağlayabiliriz. Web Part kontrollerimizi ve aradaki bağlantıyı tesis etmekte kullanacağımız arayüz (interface) tipimizi ayrı bir kütüphane (library) halinde tasarlarsak, Visual Studio.Net ToolBox'ına ekleme ve farklı web projelerinde kullanma şansına da sahip oluruz. Bu nedenle basit bir Web Control Library projesi geliştirerek işe başlayabiliriz. Yazacağımız tipler aşağıda yer almaktadır.

![mk185_2.gif](/assets/images/2006/mk185_2.gif)

IcerikSaglayici ve IcerikKullanici isimli Web Part kontrollerimiz, WebPart abstract sınıfından türetilmiştir. Böylece kişiselleştirme gibi özelliklere sahip olabilirler. En önemliside birbirlerine bağlanabilirler. IAnlasma isimli arayüzümüz, IcerikSaglayici'dan, IcerikKullanici kontrolüne aktarılacak olan veriyi basit bir özellik (property) olarak tanımlamaktadır. Şu anda bu veri bizim için ArananBilgi isimli özelliktir. Hatırlarsanız veri sağlayıcı Web Part kontrolünün, taşınacak veriyi içeren referansı aktarabilecek bir arayüz kullanması gerektiğinden bahsetmiştik. İşte bu nedenle IcerikSaglayici kontrolümüze IAnlasma arayüzü uygulanmaktadır.

Web Part kontrollerimizdeki belkide en önemli üyeler bağlantı noktası (Connection Point) metodlarıdır. IcerikSaglayici Web Part kontrolümüzde bu görevi SaglayiciBaglantiNoktasi isimli metodumuz üstlenmektedir. IcerikKullanici Web Part kontrolümüzde ise TuketiciBaglantiNoktasi bu görevi üstlenir. SaglayiciBaglantiNoktasi metodu geriye IAnlasma arayüzünün taşıyacağı bir referansı göndermektedir. Söz konusu referans çalışma zamanında, IcerikSaglayici kontrolünün nesne örneğinden başkası değildir. Bu referansı alacak olan tüketici (consumer) üyemiz TuketiciBaglantiNoktasi ise, IAnlasma tipinden bir arayüzü parametre olarak almaktadır. Dolayısıyla çalışma zamanında bu iki metod birbirleri ile haberleşme (şu an için tek yönlü bir haberleşme söz konusudur) yeteneğini kazanmaktadır. Tabiki bu üyelerin, birbirlerine ulaşabilmelerini sağlamak için ilgili niteliklerle (attributes) imzalanmaları ve Web Part'ların bir birlerine ya dinamik olarak yada statik olarak bağlanmaları gerekecektir. Ama öncesinde kod tarafında yaptıklarımıza kısaca bakalım.

IAnlasma arayüzü (interface);

```csharp
public interface IAnlasma
{
    string ArananBilgi
    {
        get;
        set;
    }
}
```

IcerikSaglayici Web Part kontrolü;

```csharp
public class IcerikSaglayici:WebPart,IAnlasma
{
    private string _urunAdi;
    private TextBox _aramaAlani;
    private Button _btnAra;

    /* Web Part içerisindeki kontrolleri oluşturmak için CreateChildControls metodunu eziyoruz.*/
    protected override void CreateChildControls()
    {
        // Önce Web Part üzerindeki kontrolleri temizleyelim.
        Controls.Clear();

        // Web Part kontrolünün başlık bilgisini değiştiriyoruz.
        Title = "Ürün Arama";

        // Aranacak bilginin girileceği TextBox kontrolünü tanımlayıp Controls koleksiyonuna ekliyoruz.
        _aramaAlani = new TextBox();
        Controls.Add(_aramaAlani);

        // Arama emrini verecek olan Button kontrolümüzü oluşturuyoruz.
        _btnAra = new Button(); 
        _btnAra.Text = "Bakacağım Ürün";
        _btnAra.BackColor = System.Drawing.Color.Gold;
        _btnAra.BorderColor = System.Drawing.Color.Gray;
        _btnAra.BorderStyle = BorderStyle.Dashed;
        _btnAra.BorderWidth = 2;
        _btnAra.Font.Bold = true;
        // Anonymous(isimsiz) metod yardımıyla Button' a basıldığında yapılması gerekenleri belirtiyoruz.
        _btnAra.Click += delegate(object sender, EventArgs e)
        {
            // TextBox' a girilen bilgiyi ArananBilgi isimli özelliğe atıyoruz.
            ArananBilgi = _aramaAlani.Text;
        };
        Controls.Add(_btnAra);
    }

    #region IAntlasmaYuzu Members

    /* ArananBilgi isimli özelliği kişiselleştirilebilir olarak tanımlıyoruz. Böylece siteye giren her farklı kullanıcı için farklı şekilde tutulabilecektir. */
    [Personalizable( PersonalizationScope.User)]
    public string ArananBilgi
    {
        get {
            return _urunAdi;
        }
        set { 
            _urunAdi = value; 
        }
    }

    #endregion

    // Bağlantı noktamızı ConnectionProvider niteliği ile belirtiyoruz.
    [ConnectionProvider("Arama Bağlantı Noktası","SaglayiciNokta")]
    public IAnlasma SaglayiciBaglantiNoktasi()
    {
        return (IAnlasma)this; // O anki IcerikSaglayici nesne örneğinin referansını IAnlasma tipinden olacak şekilde geri döndürüyoruz.
    }
}
```

IcerikKullanici Web Part kontrolü;

```csharp
public class IcerikKullanici : WebPart
{
    private string _arananUrun;
    private Label _lblGelenUrunAdi;
    private GridView _grdUrunler;

    // Bağlantı noktası olması için ConnectionConsumer niteliği ile imzalıyoruz.
    [ConnectionConsumer("Tüketici Bağlantı Noktası","TuketiciNokta")]
    public void TuketiciBaglantiNoktasi(IAnlasma anls)
    { 
        _arananUrun= anls.ArananBilgi; // Sağlayıcıdan gelen referansın üzerinden ArananBilgi özelliğinin değerini alıyoruz.
        CreateChildControls(); // Alınan değer göre web part üzerindeki kontrollerin tekrardan oluşturulmasını sağlıyoruz.
    }

    protected override void CreateChildControls()
    {
        Controls.Clear(); // Önce kontrolleri kaldırıp sahayı temizliyoruz.

        // Web Part' ın başlık bilgisini değiştiriyoruz.
        Title = "Arama Sonuçları"; 

        // Aranan bilgiyi gösterecek Label kontrolünü oluşturup Web Part' a ekliyoruz.
        _lblGelenUrunAdi = new Label();
        if (!String.IsNullOrEmpty(_arananUrun)) // Eğer _arananUrun alanının değeri boş yada null değilse eklemesini sağlıyoruz
            _lblGelenUrunAdi.Text = _arananUrun;
        _lblGelenUrunAdi.ForeColor = System.Drawing.Color.Red;
        _lblGelenUrunAdi.Font.Bold = true;
        _lblGelenUrunAdi.Font.Size = 12;
        Controls.Add(_lblGelenUrunAdi);

        // Bir alt satıra geçme için Literal kontrol kullanıyoruz.
        Literal ltr = new Literal();
        ltr.Text = "<br/>";
        Controls.Add(ltr);

        // gelen arama bilgisine göre veri çekme ve GridView kontrolüne bağlama işlemlerini gerçekleştiriyoruz.
        if (!String.IsNullOrEmpty(_arananUrun))
        {
            using (SqlConnection conn = new SqlConnection("data source=localhost;database=AdventureWorks;integrated security=SSPI"))
            {
                SqlDataAdapter da = new SqlDataAdapter("Select ProductId,Name,ListPrice,SellStartDate From Production.Product Where Name Like '%" + _arananUrun + "%'", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                _grdUrunler = new GridView();
                _grdUrunler.DataSource = dt;
                _grdUrunler.DataBind();
                Controls.Add(_grdUrunler);
            }
        }
    }
}
```

Şimdi geliştirmiş olduğumuz bu kütüphaneyi herhangibi web uygulamasında kullanmak üzere aşağıdaki gibi ToolBox'a ekleyelim. Bunun için ToolBox'ta Choose Items'a geçmemiz ve assembly'ımızı bulup IcerikSaglayici ve IcerikKullanici kontrollerini seçmemiz yeterli olacaktır. Örnek olarak ben General sekmesi altına ekledim.

Assembly'ın seçildiği iletişim kutusu;

![mk185_3.gif](/assets/images/2006/mk185_3.gif)

ToolBox'ın görünümü;

![mk185_4.gif](/assets/images/2006/mk185_4.gif)

Geliştirdiğimiz kontroller birer Web Part kontrolüdür. Bu sebeptende ilgili web sayfasında Web Part Zone'lar içerisinde kullanılıp birbirlerine bağlanabilmeleri için statik veya dinamik bağlama tekniklerinden birisini tercih etmemiz gerekmektedir. Dinamik bağlama tekniğine göre, web part kontrollerinin birbirlerine bağlanmasını sayfa üzerinden gerçekleştirme imkanına sahip oluruz. Bunun için web sayfası üzerinde bir ConnectionsZone olması yeterlidir. Ayrıca kullanıcının dinamik bağlama seçeneğini kullanması halinde ilgili Web Part Zone'lar üzerinden Connect Verb'ünün kullanılabilir olması gerekecektir. Bu nedenlede sayfada yer alan WebPartManager kontrolünün DisplayMode özelliğinin ConnectDisplayMode olması gerekir. Çoğunlukla Web Part kullanıldığında, Display Mode'ların çalışma zamanında değiştirilebilmesi için Menu kullanımı tercih edilmektedir. Bizim amacımız sadece ConnectionsZone'u kullanmak olduğu için, fazla uğraşmadan sayfanın PageLoad kısmında aşağıdaki kod parçasında gösterildiği gibi DisplayMode'u belirliyoruz.

```csharp
public class IcerikKullanici : WebPart
{
    private string _arananUrun;
    private Label _lblGelenUrunAdi;
    private GridView _grdUrunler;

    // Bağlantı noktası olması için ConnectionConsumer niteliği ile imzalıyoruz.
    [ConnectionConsumer("Tüketici Bağlantı Noktası","TuketiciNokta")]
    public void TuketiciBaglantiNoktasi(IAnlasma anls)
    { 
        _arananUrun= anls.ArananBilgi; // Sağlayıcıdan gelen referansın üzerinden ArananBilgi özelliğinin değerini alıyoruz.
        CreateChildControls(); // Alınan değer göre web part üzerindeki kontrollerin tekrardan oluşturulmasını sağlıyoruz.
    }

    protected override void CreateChildControls()
    {
        Controls.Clear(); // Önce kontrolleri kaldırıp sahayı temizliyoruz.

        // Web Part' ın başlık bilgisini değiştiriyoruz.
        Title = "Arama Sonuçları"; 

        // Aranan bilgiyi gösterecek Label kontrolünü oluşturup Web Part' a ekliyoruz.
        _lblGelenUrunAdi = new Label();
        if (!String.IsNullOrEmpty(_arananUrun)) // Eğer _arananUrun alanının değeri boş yada null değilse eklemesini sağlıyoruz
            _lblGelenUrunAdi.Text = _arananUrun;
        _lblGelenUrunAdi.ForeColor = System.Drawing.Color.Red;
        _lblGelenUrunAdi.Font.Bold = true;
        _lblGelenUrunAdi.Font.Size = 12;
        Controls.Add(_lblGelenUrunAdi);

        // Bir alt satıra geçme için Literal kontrol kullanıyoruz.
        Literal ltr = new Literal();
        ltr.Text = "<br/>";
        Controls.Add(ltr);

        // gelen arama bilgisine göre veri çekme ve GridView kontrolüne bağlama işlemlerini gerçekleştiriyoruz.
        if (!String.IsNullOrEmpty(_arananUrun))
        {
            using (SqlConnection conn = new SqlConnection("data source=localhost;database=AdventureWorks;integrated security=SSPI"))
            {
                SqlDataAdapter da = new SqlDataAdapter("Select ProductId,Name,ListPrice,SellStartDate From Production.Product Where Name Like '%" + _arananUrun + "%'", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                _grdUrunler = new GridView();
                _grdUrunler.DataSource = dt;
                _grdUrunler.DataBind();
                Controls.Add(_grdUrunler);
            }
        }
    }
}
```

Sayfamızın tasarımını ise aşağıdaki gibi geliştirebiliriz.

![mk185_5.gif](/assets/images/2006/mk185_5.gif)

Artık uygulamamızı test edebiliriz. Sağlıklı sonuçlar alabilmek için web uygulamasında Form tabanlı güvenlik sistemi uygulanmıştır. Amaç hem Web Part kontrolleri arası bilgi taşımak hemde bu Web Part'ların giren kullanıcılara göre kişiselleştirilebilmesini sağlamaktır. Biz dinamik bağlama tekniğini tercih ettiğimiz için öncelikli olarak kullanıcıların ilgili Web Part kontrollerini birbirlerine bağlamaları gerekecektir. Örneğimizde bu bağlantı yönü IcerikSaglayici kontrolden, IcerikKullanici kontrole doğru olmalıdır. Bu sebepten çalışma zamanında IcerikSaglayici Web Part kontrolümüzün bulunduğu Zone'da Connect menü öğesi seçilmelidir.

![mk185_6.gif](/assets/images/2006/mk185_6.gif)

Connect linkine tıkladığımızda ConnectionsZone bileşeni görünür hale gelecektir. Henüz bir bağlantı tanımlamadığımız için No active connections mesajını almaktayız.

![mk185_7.gif](/assets/images/2006/mk185_7.gif)

Şimdi Create a Connection to a Consumer linkine tıklarsak, IcerikSaglayici Web Part kontrolünden, IcerikKullanici Web Part kontrüne doğru bir bağlantı tanımlamamız yeterli olacaktır.

![mk185_8.gif](/assets/images/2006/mk185_8.gif)

Artık Web Part kontrollerimiz arasında veri taşıyabiliriz. Aşağıdaki Flash animasyonunda uygulamanın örnek çalışması gösterilmektedir. (Animasyonu izleyebilmek için en azından sisteminizde Flash 6 Player'ının yüklü olması geremektedir.)

Buraksenyurt isimli kullanıcı sisteme girdikten sonra Ürün Arama başlıklı Web Part kontrolünden, aramak istediği kelimeleri giriyor. Button kontrolüne basıldığında ise bulunan sonuçlar Arama Sonuçları başlıklı Web Part kontrolünde GridView nesnesinde gösteriliyor. Şu anda buradaki tüm ayarlar buraksenyurt kullanıcısı için kişiselleştirilmiştir. Özellikle arama alanı bilgisi kişiselleştirildiğinden, buraksenyurt isimli kullanıcı sisteme tekrar girdiğinde son aradığı ürün bilgisi ve sonuçları ile karşılaşacaktır. Dolayısıyla farklı kullanıcılar için farklı arama bilgisi ve sonuçları tutulabilecektir.

Örneğimizde kullandığımız dinamik bağlama tekniği dışında, Web Part kontrollerini birbirlerine statik olarakta bağlayabiliriz. Tek yapmamız gereken Web Part Manager elementi içeriğini aşağıdaki gibi değiştirmektir.

```csharp
<asp:WebPartManager ID="wpManager" runat="server">
    <StaticConnections>
        <asp:WebPartConnection ID="StatikBaglayici" ProviderConnectionPointID="SaglayiciNokta" ProviderID="IcerikSaglayici1" ConsumerConnectionPointID="TuketiciNokta" ConsumerID="IcerikKullanici1" /> 
    </StaticConnections>
</asp:WebPartManager>
```

Dikkat ederseniz WebPartConnection elementi içerisinde icerik sağlayıcı ve tüketici için gerekli Id tanımlamaları yapılmaktadır. ProviderConnectionPointID niteliği (attribute), IcerikSaglayici Web Part kontrolündeki bağlantı noktası metodunda yer alan ConnectionProvider niteliğinin ikinci parametresinin değeridir.

```csharp
[ConnectionProvider("Arama Bağlantı Noktası","SaglayiciNokta")]
public IAnlasma SaglayiciBaglantiNoktasi()
{
```

Benzer şekilde ConsumerConnectionPointID niteliğinin değeride, IcerikKullanici Web Part kontrolündeki bağlantı noktası metodunda yer alan ConnectionConsumer niteliğinin ikinci parametresinin değeridir.

```csharp
[ConnectionConsumer("Tüketici Bağlantı Noktası","TuketiciNokta")]
public void TuketiciBaglantiNoktasi(IAnlasma anls)
{
```

ProviderID ve ConsumerID niteliklerinin değerleri ise, ilgili Web Part kontrollerimizin ID niteliklerinin değerleridir. Uygulamamızı bu haliyle test ettiğimizde sonuçların değişmediğini görürüz. Tabiki test amacıyla sayfadaki ConnectionsZone nesnesini kaldırmak ve Web Part Manager kontrolü için DisplayMode özelliğini set etmemek gerekmektedir.

Bu makalemizde kendi Web Part kontrollerimiz arasında veri aktarımı amacıyla bağlantıların (Connections) nasıl kurulabileceğini incelemeye çalıştık. Burada gördüğümüz teknik sadece tek yönlü bir bağlama seçeneği sunmaktadır. Birde Consumer'dan Provider'a doğru veri aktarmak isteyebileceğimiz vakkalar olduğunu düşünmek gerekir. Bu durumda iki yönlü bağlantılar söz konusu olacaktır. Bu konunun araştırmasınıda siz değerli okurlarımıza bırakıyorum. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek uygulama için tıklayın.](/assets/files/2006/BaglantiliWebParts.rar) (Önemli Not: Dosya boyutunun büyük olması nedeniyle, kişiselleştirme amacıyla kullanılan ve AppData klasöründe yer alan Aspnetdb.mdf veritabanı dosyası silinmiştir. Test ederken lütfen Asp.Net Configuration Tool'unda ilgili veritabanının oluşturulmasını sağlayınız ve en azından örnek iki kullanıcıyı sisteme dahil ediniz.)