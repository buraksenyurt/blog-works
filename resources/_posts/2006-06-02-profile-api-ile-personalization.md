---
layout: post
title: "Profile API ile Personalization"
date: 2006-06-02 12:00:00 +0300
description: "Günümüz web uygulamalarında, kullanıcı bazlı kişiselleştirme (personalization) oldukça önemli ve popüler bir konudur. Örneğin, bir elektronik ticaret sitesini ziyaret ettiğimizi düşünelim. Bu site bir sonraki ziyaretimizde, daha önceden bakmış olduğumuz ürünler ile ilişkili yeni önerilerde bulunabilir. Böylece siteye giren kullanıcıları başka ürünlere yönlendirebilir. Yeni önerilerin sunulabilmesi, sitenin bizim alışkanlıkarımızı takip etmesi ve kendisini buna göre değiştirmesi anlamına gelmektedir."
categories:
  - aspnet-2-0
tags:
  - aspnet-2-0
  - xml
  - csharp
  - dotnet
  - aspnet
  - sql-server
---
# Profile API ile Personalization
Değerli Okurlarım Merhabalar,

Günümüz web uygulamalarında, kullanıcı bazlı kişiselleştirme (personalization) oldukça önemli ve popüler bir konudur. Örneğin, bir elektronik ticaret sitesini ziyaret ettiğimizi düşünelim. Bu site bir sonraki ziyaretimizde, daha önceden bakmış olduğumuz ürünler ile ilişkili yeni önerilerde bulunabilir. Böylece siteye giren kullanıcıları başka ürünlere yönlendirebilir. Yeni önerilerin sunulabilmesi, sitenin bizim alışkanlıkarımızı takip etmesi ve kendisini buna göre değiştirmesi anlamına gelmektedir.

Daha açık bir şekilde söylemek gerekirse bu site, üyelerin özelliklerine göre kişiselleşmektedir. Buna örnek olabilecek en güzel sitelerden birisi amazon.com'dur. Bir sitenin, üyelerinin isteklerine göre davranış göstermesi için kullanılan çeşitli teknikler vardır. Bunlardan belkide en basiti ve ilkel olanı Cookie kullanımıdır. Cookie'ler istemci taraflı bilgi saklama amacıyla tercih edilirler. Ancak istemcilerin Cookie desteği olmadığı durumlarda kişiselleştirme (personalization) mümkün olmayacaktır. Alternatif ve daha sağlıklı bir yol olarak kullanıcıya ilişkin bilgilerin kişiselleştirme (personalization) amacıyla veritabanı sistemlerinde tutulması ve sayfalar arası geçişlerde de Session nesneleri yardımıyla taşınması çok daha profesyonel bir yaklaşımdır.

Asp.Net 2.0, getirdiği MemberShip Management (Üyelik Yönetimi) sistemi içerisine kişiselleştirme (personalization) ile ilişkili bir takım yenilikler eklemiştir. Bunlardan belkide en önemlisi Profile API'dir. Diğer kişiselleştirme (personalization) şekli ise Web Part kullanımıdır. Profile API yardımıyla, kullanıcı bazlı kişiselleştirilmiş ayarlar MemberShip sistemine dahil edilip kullanılabilir. Böylece siteye giren her üye için farklı kişisel bilgiler tutulabilir ve sayfanını o kullanıcıya has olacak şekilde değiştirilmesi dinamik olarak sağlanabilir. Elbette kullanıcı için geçerli olan bu kişisel ayarlar üyelik sistemine ait tablolarda saklanmalıdır. Aksi halde siteye giren kullanıcı ile kişiselleştirilmiş verilerin entegrasyonu çok zor olacaktır.

Profile sisteminde veritabanı tarafında Sql Server 7, 2000 ve 2005 sistemleri kullanılabileceği gibi, Custom Provider'lar yardımıyla farklı depolama ortamlarıda ele alınabilir. Profile API üye bazlı verileri sunucu üzerinde tutmaktadır. Biz bu makalemizde Profile API'sini kullanarak basit olarak kişiselleştirme (personalization) işlemlerinin nasıl yapılabileceğini incelemeye çalışacağız. Profile API'sini kullanırken, kişiselleştirilecek bilgiler birer özellik olarak Web.Config dosyasına eklenirler. Asp.Net 2.0 ' da Web.Config dosyasına eklenen yeni boğumlardan birisi olan Profile boğumu sayesinde o site için geçerli tüm kullanıcılara ait ortak profil deseneleri oluşturulabilir. Profile boğumu içerisinde herhangibir bir üye için kişsel ayarların tutulabileceği özellik tanımlamaları yer alır. Bu özellikler içinde Properties boğumundan faydalanılır. Aşağıdaki örnekte Web.config dosyası içerisine dahil edilmiş örnek Profile özelliklerini görebilirsiniz.

```xml
<system.web>
    <profile>
        <properties>
            <add name="Kategorim"/>
            <add name="KarsilamaMesajim"/>
            <add name="SonGirisZamanim"/>
        </properties>
    </profile>
```

Bu profile bilgisine göre, sisteme giriş yapan her kullanıcı için Kategorim, KarsilamaMesajim ve SonGirisZamanim isimli özelliklerde kişiye göre farklı değerler tutulabilecektir. Profile içerisinde belirtilen özelliklerin kullanılabilmesi için en azından sitenin kullanıcı bazlı çalışacak şekilde organize edilmesi gerekir. Bu makale için geliştirdiğimiz örnekte, Sql Server 2005 Express Edition'ı kullanan MemberShip sistemi çalışmaktadır. Böylece, her bir kullanıcıya özel profil değerlerini saklayabiliriz. Bu değerler varsayılan olarak aspnet_Profile tablosunda saklanmaktadır. Profile için tutulacak özellikleri konfigurasyon dosyasına ekledikten sonra kod içerisinde Profile sınıfını kullanarak çeşitli işlemleri gerçekleştirebiliriz. Web.Config dosyasında, Profile boğumu içerisindeki Properties boğumu altına eklenen her bir elemana Profile sınıfı üzerinden erişebiliriz.

![mk163_1.gif](/assets/images/2006/mk163_1.gif)

Şekildende görebileceğiniz gibi, kod tarafında Profile sınıfı ve intelli-sense yardımıyla, özellik olarak eklediğimiz tüm elemanlara ulaşabilmekteyiz. Şimdi basit olarak bu sınıfı nasıl kullanacağımıza bir bakalım. Örnek senaryo olarak default.aspx sayfamızı aşağıdaki gibi tasarlayalım.

![mk163_2.gif](/assets/images/2006/mk163_2.gif)

Öncelikli olarak sayfamızın çalışma şeklinden kısaca bahsedelim. Uygulamamızda Form tabanlı doğurlama kullandığımız için, default.aspx sayfasına gelinmeden önce isimsiz kullanıcılar UserLogin.aspx sayfasına bir kere uğramak zorundalar. Burada sisteme başarılı bir şekilde Login olan kullanıcılar default.aspx sayfasına geldiklerinde güncel profillerine ait özelliklere erişebilecek ve değiştirebilecekler. Böylece Web.Config dosyasındaProfile boğumu içerisinde belirttiğimiz özelliklerin her birinin değerlerini her kullanıcı için farklı şekillerde tutabileceğiz. Yani kişiye göre özelleştirme yapmış olacağız. Bunları gerçekleştirebilmek için kodumuzda aşağıdaki değişiklikleri yapalım.

```csharp
public partial class _Default : System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (User.Identity.IsAuthenticated)
        {
            ProfileBilgisi();
        }
    }

    private void ProfileBilgisi()
    {
        lblKategorim.Text = Profile.Kategorim;
        lblMesajim.Text = Profile.KarsilamaMesajim;
        lblSonGirisZamanim.Text = Profile.SonGirisZamanim;
    }
    protected void btnSaveProfile_Click(object sender, EventArgs e)
    {
        Profile.SonGirisZamanim = DateTime.Now.ToString();
        Profile.KarsilamaMesajim = txtMesaj.Text;
        Profile.Kategorim = ddlKategoriler.SelectedItem.Text;
        Profile.Save();
        ProfileBilgisi();
    }
}
```

İlk olarak sayfa yüklenirken eğer kullanıcı sisteme giriş yapmış ise ona ait Profile özelliklerinin değerlerini alıyor ve ekrandaki label kontrollerine yazdırıyoruz. Button kontrolüne tıklandığında ise kullanıcının girdiği değerler, bu kullanıcıya özel profile bilgileri olarak veritabanındaki aspnet_profile tablosuna kaydediliyor.

![mk163_3.gif](/assets/images/2006/mk163_3.gif)

Profile içerisinde tutulan özellik değerleri örnek Burak isimli kullanıcı için aspnet_Profile isimli tabloda aşağıdaki şekilde görüldüğü gibi tutulur.

![mk163_4.gif](/assets/images/2006/mk163_4.gif)

Kullanıcıya ait Profile bilgisini oluşturan özellikler ile bunlara karşılık gelen değerlerin nasıl tutulduğuna dikkat ediniz. Profile içerisinde tanımladığımız tüm özellikler PropertyNames alanı içerisinde tutulur. Bu özellikere karşılık gelen değerler ise PropertyValueString alanı içerisinde tutulmaktadır. Burada 3 farklı kullanıcı için 3 farklı Profile satırı yer almaktadır. Profile içerisinde tanımlamış olduğumuz özellikler şu anda varsayılan olarak String tipindendirler. İstersek burada tutulacak özelliklerin tiplerinide açıkça söyleyebilir ve hatta varsayılan değerlerinide ayarlayabiliriz. Özellikte tip güvenliğini sağlamak açısından type niteliğini kullanmakta fayda vardır. Örneğimizdeki Profile bilgisini buna uygun olacak şekilde aşağıdaki gibi düzenlediğimizi düşünelim.

```xml
<profile>
    <properties>
        <add name="Kategorim" type="System.String" defaultValue="Dvd"/>
        <add name="KarsilamaMesajim" type="System.String" defaultValue="Kişisel Mesajınız"/>
        <add name="SonGirisZamanim" type="System.DateTime"/>
        <add name="ButonFontBuyuklugu" type="System.Int32" defaultValue="12"/>
    </properties>
</profile>
```

Burada dikkat ederseniz iki yeni özellike daha ekledik. Bununla birlikte her özelliğimiz için geçerli olabilecek primitive tipleri ve varsayılan değerleri tanımladık. Uygulamamızı bu haliyle derlediğimizde SonGirisZamanim özelliğinin kullanıldığı yerler için hata mesajları alırız.

![mk163_5.gif](/assets/images/2006/mk163_5.gif)

Sebep son derece açıktır. SonGirisZamanim isimli Profile özelliğinin tipini DateTime olarak belirttiğimizden, bu özelliğin kullanıldığı yerlerde uygun tür dönüşümlerini yapmamız gerekmektedir. Bu değişiklikler ışığında kodumuzu yeniden aşağıdaki gibi düzenleyelim.

```csharp
protected void Page_Load(object sender, EventArgs e)
{
    if (User.Identity.IsAuthenticated)
    {
        ProfileBilgisi();
    }
}

private void ProfileBilgisi()
{
    lblKategorim.Text = Profile.Kategorim;
    lblMesajim.Text = Profile.KarsilamaMesajim;
    lblSonGirisZamanim.Text = Profile.SonGirisZamanim.ToLongDateString();
    btnSaveProfile.Font.Size = Profile.ButonFontBuyuklugu;
}
protected void btnSaveProfile_Click(object sender, EventArgs e)
{
    Profile.SonGirisZamanim = DateTime.Now;
    Profile.KarsilamaMesajim = txtMesaj.Text;
    Profile.Kategorim = ddlKategoriler.SelectedItem.Text;
    Profile.Save();
    ProfileBilgisi();
}
```

Elbette primitive tipleri kullanabileceğimiz gibi, kendi yazmış olduğumuz tipleride kullanmak isteyebiliriz. Örnek olarak kullanıcının site içerisinde gezdiği sayfalara ilişkin bilgileri saklayabileceğimiz bir sınıfımız olduğunu düşünelim.

![mk163_6.gif](/assets/images/2006/mk163_6.gif)

```csharp
public class UrlInfo
{
    private string _sayfaAdi;
    public string SayfaAdi
    {
        get { return _sayfaAdi; }
        set { _sayfaAdi = value; }
    }
    private string _url;
    public string Url
    {
        get { return _url; }
        set { _url = value; }
    }
    private DateTime _sonGirisTarihi;
    public DateTime SonGirisTarihi
    {
        get { return _sonGirisTarihi; }
        set { _sonGirisTarihi = value; }
    }
    public UrlInfo()
    {    }
    public UrlInfo(string ad, string url, DateTime giris)
    {
        SayfaAdi = ad;
        Url = url;
        SonGirisTarihi = giris;
    }
}
```

Bu sınıfı Profile içerisinde bir özellik tipi olarak set etmek için aşağıdaki notasyonu kullanmamız gerekmektedir.

```xml
<add name="SonSayfa" type="UrlInfo"/>
```

Bu noktadan sonra SonSayfa isimli Profile özelliğini uygulamamız içerisinde istediğimiz şekilde kullanabiliriz. Özelliğin çalışacağı değer türü artık UrlInfo'dur. Örneğin o anki kullanıcının Profile bilgisine yeni bir UrlInfo örneği eklemek için aşağıdaki kod satırına benzer bir ifade yazabiliriz.

```csharp
Profile.SonSayfa = new UrlInfo("default", "default.aspx", DateTime.Now);
```

Diğer taraftan SonSayfa özelliğinin içeriğini okurken, geri dönecek olan değerin tipi UrlInfo olacağından, aşağıdaki kod parçasında olduğu gibi güncel kullanıcının geçerli olan UrlInfo üyelerine de erişebiliriz.

```csharp
lblSonSayfa.Text = Profile.SonSayfa.SayfaAdi + " " + Profile.SonSayfa.SonGirisTarihi.ToShortDateString();
```

Yanlız burada dikkat edilmesi gereken bir nokta vardır. Şu anda SonSayfa özelliğinin aspnet_Profile içerisinde nasıl tutulduğuna bakarsak Xml şeklinde tutulduğunu görürüz.

![mk163_7.gif](/assets/images/2006/mk163_7.gif)

Dilerseniz özelliğin içeriğini Binary formatında tutarak daha az yer tutmasınıda sağlayabilirsiniz. Bunun için Properties boğumunda ilgili özellik için serializeAs niteliğini kullanmamız gerekmektedir.

```xml
<add name="SonSayfa" type="UrlInfo" serializeAs="Binary"/>
```

Lakin uygulamamızı bu şekilde çalıştırdığımızda ve bir kullanıcı ile sisteme giriş yaptığımızda aşağıdaki istisna mesajını alırız.

![mk163_8.gif](/assets/images/2006/mk163_8.gif)

Bunun sebebi ise, yazmış olduğumuz UrlInfo tipinin ikili formatta serileştirilebilir olarak işaretlenmemesidir. Serializable niteliğini (attribute) kullanarak bu hatanın önüne geçebilir ve tipimizi ikili formatta serileştirilerek ilgili alan içerisine alınmasını sağlayabiliriz.

```csharp
[Serializable]
public class UrlInfo
{
...
```

Tipimizi serileştirilebilir olarak işaretledikten sonra uygulamamız sorunsuz bir şekilde çalışacaktır. Elbette UrlInfo tipinin güncel kullanıcı için geçerli olan içeriği artık tabloda, PropertyValuesString alanında değil, PropertyValueBinary alanında saklanacaktır.

Profile API'si ile ilgili dikkat edilmesi gereken bir diğer hususda, siteye giren isimsiz kullanıcıların nasıl ele alınacağıdır. Siteye giren isimsiz kullanıcılar (anonymous users) içinde Profile bilgisi tanımlanabilir. Bazı alışveriş sitelerinde isimsiz kullanıcılar dahi sepete bir şeyler ekleyebilmektedir. Bu isimsiz kullanıcının bir sonraki girişinde daha önceden oluşturduğu sepeti kullanabilmeside kişiselleştirme ile alakalıdır. Aslında isimsiz bir kullanıcı sisteme girdiğinde onun için bir benzersiz bir id değeri üretilir. Bu id değeri varsayılan olarak oturum açan kullanıcının bilgisayarında cookie şeklinde saklanır. Elbette cookie desteği olmayan istemciler için aynı Session modelinde olduğu gibi cookieless çözümü vardır. Bu yöntemler sayesinde, bir siteye bağlanan her isimsiz kullanıcı için ayrı ayrı kişiselleştirmeler yapabiliriz. Başlamak için öncelikli olarak Web.Config idosyası çerisinden, isimsiz kullanıcılar için Profile kullanımını aktif hale getirmemiz gerekir. Varsayılan olarak bu özellik kapalıdır. Çünkü, isimsiz kullanıcılar için Profile kullanımı özellikle veritabanı kaynaklarını ve doğal olarakta web sunucusu kaynaklarını çok fazla harcayacaktır.

```xml
<anonymousIdentification enabled="True" cookieless="AutoDetect" />
```

Buradaki örnekte cookieless özelliğine AutoDetect değeri atanmıştır. Yani isimsiz kullanıcı için oluşturulan id değerinin istemci bilgisayarında Cookie şeklinde mi, yoksa url ile birlikte taşınacak şekilde mi tutulacağına Asp.Net karar verecektir. Cookieless niteliğinin alabileceği diğer değerler ise UseCookies, UseUri ve UseDeviceProfile ' dir. Böylece artık sistem içerisinde isimsiz kullanıcılar için Profile bilgisinin kullanılabileceğinide söylemiş oluyoruz. Lakin var olan Profile özellikleri içerisinde de hangilerinin isimsiz kullanıcılar için geçerli olacağına karar vermemiz ve bunu belirtmemiz gerekiyor. Bunun için tek yapmamız gereken istediğimiz özelliğin allowAnonymous niteliğine true değerini atamak olacaktır.

```xml
<add name="SonSayfa" type="UrlInfo" serializeAs="Binary" allowAnonymous="true"/>
```

Görüldüğü gibi Asp.Net 2.0 kişiselleştirme adına Framework Class Library içerisine oldukça güçlü bir tip atmıştır. Profile tipinin yukarıda değindiğimiz özellikleri dışında daha pek çok kabiliyetide elbette vardır. Bunların araştırılmasını siz değerli okurlarıma bırakıyorum. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama İçin Tıklayın.](/assets/files/2006/UsingProfilesAPI.rar)