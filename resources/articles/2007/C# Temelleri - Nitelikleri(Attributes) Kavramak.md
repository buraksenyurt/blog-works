---
title: "C# Temelleri : Nitelikleri(Attributes) Kavramak"
pubDate: 2007-04-11 03:00:00
categories:
  - C#
tags:
  - C#
  - Bash
  - .NET
  - ASP.NET
  - LINQ
  - SQL Server
  - WCF
  - XML
  - SOAP
  - Web Service
  - HTTP
  - Serialization
  - Reflection
  - Delegates
  - Generics
  - Visual Studio
---

# C# Temelleri : Nitelikleri(Attributes) Kavramak
Değerli Okurlarım Merhabalar,

Nitelik (Attribute) eninde sonunda her dotNet programcısının kullandığı ve karşılaştığı bir kavramdır. Özellikle yansıma (Reflection) konusu ile birlikte anıldığından,.Net Framework içerisinde önemli bir yere sahiptir..Net Framework içerisinde pek çok modelde niteliklerden aktif olarak faydalanılmaktadır. Web servislerinden windows kontrollerini geliştirmeye, kendi web part bileşenlerimizi yazmaktan serileştirmeye kadar pek çok alanda işe yaramaktadır. Hatta çok popüler olarak, katmanlı mimarilerde ve nitelik bazlı (attribute based) programlama modellerinde de ele alınmaktadır. İşte bu makalemizde nitelikleri incelemeye çalışacak ve özellikle kendi niteliklerimizi nasıl geliştirebileceğimize değineceğiz.

Herşeyden önce, niteliği (Attribute) tanımlamakta fayda vardır. Nitelikler, uygulandıkları tiplerin (types) yada üyelerin (members) çalışma zamanındaki davranışlarının değiştirilmesine olanak sağlayan sınıflardır. Niteliklerin sınıf (class) olduğu rahatlıkla söylenebilir. Nitekim var olan veya bizim tarafımızdan geliştirilen nitelikler daima Attribute sınıfından türemek zorundadırlar. Attribute, abstract bir sınıftır. Dolayısıyla örneklenemez ancak bir nitelik sınıfının içermesi gereken temel üyeleri bünyesinde barındırır.

Aslında niteliklerin belkide en önemli özelliği, üretilen assembly içerisinde yer alan tip ve üyelere ekstra bilgiler katabilmeleridir. Bir başka deyişle metadata içerisine ilave bilgiler eklenebilmesini sağlamaktadır. Bu noktada ortaya önemli bir soru çıkar. Söz konusu ekstra veriler kim tarafından ve nasıl değerlendirilecektir? İşte bu noktada yansıma (Reflection) konusu çok büyük önem taşımaktadır. Öyleki, çalışma zamanında (run-time) herhangibir tipin ve üyelerinin hakkında bilgi sahibi olabilme imkanı aynı zamanda metadata içeriğinide elde edebilme anlamına gelmektedir.

> Nitelikler (Attributes),.Net Framework'de var olan veya geliştiriciler tarafından yazılan tip (type) veya üyelere (members) çalışma zamanında davranışlarının farklı şekillerde ele alınabilmelerini sağlayan ekstra metadata (veri hakkında veri) bilgileri ekler. Bu metadata bilgileri üretilen assembly'lar içerisinde yer alır ve yansıma (Reflection) teknikleri ile çalışma zamanında değerlendirilebilir.

Niteliklerin faydasını ve ne işe yaradıklarını daha net bir şekilde anlayabilmek için aşağıdaki örnek senaryolar göz önüne alınabilir.

Asp.Net Web Uygulamalarında Kendi Kontrollerimizi Geliştirirken

Daha önceki makalelerimizde kendi web server kontrollerimizi nasıl yazacağımıza kısaca değinmiştik. Şimdi şöyle düşünelim. Yazdığımız bu kontrollerin ele alındığı bir geliştirme ortamı var mı? Cevabın Visual Studio IDE ortamı olduğunu gayet iyi biliyoruz. Peki nasıl oluyorda, bir kontrolü ToolBar üzerinden alıp sayfaya bıraktığımızda, özellikler (Properties) pencersinde, o kontrol sınıfına ait bazı üyeler (özellikler, olaylar) getiriliyor? Demekki, IDE bir çalışma zamanı ortamı olarak sürüklenip bırakılan kontrolün hangi üyelerinin Properties penceresinde görünmesi gerektiğini anlayabiliyor. Hatırlayacağınız gibi özelliklerin başına, hatta kontrol sınıfının başına atılan bazı nitelikler (attributes) vardı.

```csharp
[Browsable(true)]
[Description("Hangi Gün?")]
[Bindable(true)]
[Themeable(true)]
[Category("Tarih Degerleri")]
[DefaultValue("1")]
[Localizable(true)]
public string SeciliGun
{
```

Çok basit olarak SeciliGun isimli özelliğin üzerine yazılmış olan nitelikler, Visual Studio IDE'si için anlamlıdır. Nitekim Visual Studio çalışan bir uygulama olaraktan, çalışma zamanında (run-time) ilgili niteliklerin değerlerine bakarak bazı hamlelerde bulunur. Örneğin Browsable niteliğinin true değerine sahip olması, SeciliGun özelliğinin Visual Studio IDE'sinde Properties penceresine eklenmesi gerektiği anlamına gelir. Description niteliği içerisindeki metinsel bilgiler, IDE tarafından değerlendirilip yine Properties penceresinde gösterilir.

Kendi Web Part Bileşenlerimizi Geliştirdiğimizde

Bundan bir önceki makalemizde kendi Web Part bileşenlerimizi nasıl geliştirebileceğimiz incelemiştik. Geliştirdiğimiz Web Part bileşenlerinin bazı özelliklerinin kişiselleştirilebilmesi (personalizable) ve çalışma zamanında istemcinin bilgisayarında yer alan tarayıcı pencersindeki bir PropertyGridEditorPart içerisinde açılıp değiştirilebilmesi için aşağıdaki niteliklerden faydalandık.

```csharp
[WebBrowsable(true)] 
[WebDescription("Verilen Url adresine göre Rss bilgisini okur")]
[Personalizable(PersonalizationScope.User)]
[WebDisplayName("Rss Bilgisi Alınacak Url")]
public string Url
{
    get { return _Url; }
    set { _Url = value; }
}
```

İşte buradaki nitelikleri değerlendiren kişi, Asp.Net Runtime Host'un ta kendisidir. Yine çalışma zamanındaki bir ortamın karar mekanizmalarında ihtiyaç duyacağı bazı bilgiler metadata içerisine nitelikler yardımıyla eklenmektedir. Buna göre örneğin, Asp.Net Runtime Host, Personalizable niteliğinde PersonalizationScope isim enum sabitinin değerini User olarak gördüğünde takip eden özelliğin kişiselleştirme amaçlı olarak her kullanıcı için ayrı olacak şekilde veritabanına yazılması gerektiğini anlayacaktır. Yine WebBrowsable niteliğine true değeri verilmesi sayesinde, ilgili özelliğinde istemcilerin tarayıcı penceresinde görülecek olan PropertyGridEditorPart içerisinde ele alınabileceğinide anlayacak ve sayfanın sunucundan istemciye olan hareketinde, render işlemini bu kritere göre değiştirecektir.

Nesneleri Binary Formatta Serileştirmekte

Bildiğiniz gibi bir nesneyi ikili (binary) formatta serileştirmek için BinaryFormatter sınıfının Serialize metodundan yararlanırız. Benzer şekilde ters serileştirme işlemi içinde Deserialize metodunu kullanırız. Ancak hepimizin yakından tanıdığı bir kural vardır. Bir tipin ikili formatta (binary) serileştirilebilmesi için Serializable niteliği ile işaretlenmiş olması gerekir. Binary formatta serileştirmenin olduğu yerler göz önüne alındığında söz konusu niteliğin önemi ortaya çıkmaktadır. Örneğin web uygulamalarında session bilgilerinin veritabanından tutulmasına karar verildiğinde veya Profile bilgilerinde kendi tiplerimizi yada var olan tipleri tablodaki binary alanda tutmak istediğimizde...

Windows Communication Foundation'da Kontratları (Contrats) Hazırlarken

Yakın zamanda,.Net Framework 3.0 ile gelen ve Microsoft tabanlı dağıtık mimari (distributed architectures) modellerini tek bir çatı altında toplayan Windows Communication Foundation'da, bir sınıfın servis olarak yayınlanması için ve sınıf içinden dış dünyaya açılabilecek fonksiyonellikler için yine nitelikleri kullanmaktadır. Aşağıdaki örnek kod parçasında ServiceContract ve OperationContract isimleriye geliştirilen niteliklerin örnek uygulanış şeklini görmektesiniz.

```csharp
[ServiceContract]
    public interface IMatematikServis
    {
        [OperationContract]
        double Toplam(double x, double y);

        void DahiliMetod();
    }
```

Web Servislerinde

Bir web servisinin istemci tarafından tüketilebilmesi için çoğunlukla proxy sınıflarını kullanıyoruz. Elbette istisnai olarak doğrudan HTTP veya SOAP üzerinden talepte de bulunabilmekteyiz. Nitekim proxy sınıflarının üretilebilmesi içinde, web servisine ait bir WSDL dökümanının ele alınması gerekiyor. WSDL (Web Service Description Language) dökümanı bildiğiniz gibi bir web servisinin tanımlamalarının ve fonksiyonelliklerin bir XML içeriği olarak üretilmesini sağlıyor. Peki biz bu belgeyi herhangibir şekilde talep ettiğimizde, bu talebe karşılık XML dökümanı içerisine hangi sınıfların ve hangi metodların koyulacağını sistem nereden biliyor? İşte bu noktada devreye WebService ve WebMethod gibi nitelikler (attributes) girmektedir. Böylece WSDL dökümanını hazırlayacak olan HttpHandler hangi sınıfı ve hangi metodu xml içerisine alacağını bilecektir.

Katmanlı Mimaride Entity Tiplerinde

Özellikle katmanlı mimaride nitelikler çok faydalı olabilmektedir. Örneğin, veritabanında yer alan tabloların karşılıklarının tutulduğu sınıflar için otomatik olarak select, insert,update ve delete gibi sorguların hazırlanması istendiği durumlarda çalışma zamanı için ekstra bilgilere (additional metadata) ihtiyaç vardır. İşte çalışma zamanındaki bu ihtiyçaları nitelikler yardımıyla karşılayabiliriz. Söz gelimi entity tipi içerisindeki alan adlarının tablolardaki karşılıklarını, identity olup olmadıklarını yada farklı entity tipleri arasında, tablolar arasındaki ilişkilerin nasıl gerçekleştirilebileceğini belirlemek vb konularda ele alınabilir.

DLINQ (Database Language Integrated Query) de Yer Alan Entity Tiplerinde

Şu anda C# 3.0 ile birlikte adı en çok anılan modellerden biriside DLINQ (Database Language Integrated Query) dir. DLINQ temel olarak veritabanından nesnelere indirgenen kümeler üzerinde LINQ sorgularının çalıştırılmasına izin veren bir modeldir. Aslında model tipik olarak entity katmanlarına dayanan bir yapıya sahiptir. Tablo, alan ve ilişki (relation) eşleştirmeleri vb... için niteliklerden (attributes) faydalanılmaktadır.

```csharp
[Table(Name="Calisanlar")]
class Calisan
{
    [Column(Name="Id",Id=true)]
    public int Id;
```

Bir Assembly Hakkında Bilgi Vermek için AssemblyInfo.cs İçeriğinin Değişitirlmesi

Geliştirdiğimiz uygulamaların ürettiği assembly'lara ait genel bilgileri AssemblyInfo.cs dosyası içerisindeki assembly seviyesinde kullanılabilen nitelikler sayesinde metadata içerisine alabiliriz. Bu sayede geliştirdiğimiz Assembly'ın hangi kültüre destek verdiğini (Culture), versiyonunu (Version), varsa strong key bilgisini, başlığını (Title), açıklamasını (Description) belirtebiliriz. Bu tip bilgiler metadata içerisine alındıktan sonra örneğin ClickOnce gibi mimariler tarafından kullanılıp setup sayfalarının oluşturulması sırasında kullanılabilir.

Gördüğünüz gibi, nitelikler çalışma zamanında bir takım uygulama parçaları tarafından değerlendirilmekte ve buna göre sonuçlar üretilmektedir.

Bu kısa bilgilerden sonra gelin kendi niteliklerimizi (Custom Attributes) nasıl yazabileceğimize bakalım. Kendi niteliklerimizin kıymetlenebilmesi için onları ele alacak bir modelede ihtiyacımız olacaktır. Burada devreye yansıma (Reflection) girecek. Örneğin Sql Server 2005 ile birlikte gelen AdventureWorks veritabanındaki Production şemasında (Schema) yer alan Product tablosunun programımız içerisinde bir tip ile ifade edildiğini düşünebiliriz. Bu tip için gerekli insert, update, delete ve select işlemlerinin bu tip içerisindeki metodlar ile yapılmak istendiğini düşünelim. Bu durumda yansımadan faydalanarak özelliklerin adlarından ve o anki değerlerinden yararlanıp bizim için gereken sorguları otomatik olarak hazırlatabiliriz.

Ancak dikkat edilmesi gereken noktalar vardır. Örneğin ProductId alanı identity tipindendir ve bu nedenlede otomatik olarak artmaktadır. Dolayısıyla otomatik oluşturulacak insert sorgusuna dahil edilmemesi gerekir. Peki çalışma zamanında bu alanı işaret eden sınıf özelliğinin, insert sorgusuna dahil edilmemesi gerektiğini nereden bilebiliriz? İşte bu özellikler için yazacağımız bir nitelik yardımıyla çalışma zamanında davranış değiştirilmesini sağlayabiliriz. Gelin ne demek istediğimiz örnek üzerinden incelemeye çalışalım. Bu amaçla öncelikli olarak bir Product nesnesini temsil edecek bir sınıf tasarlayacağız. Amacımız insert, update, delete ve select sorgularının çalışıp çalışmadığını kontrol etmekten ziyade, bunların oluşturulması sırasında niteliklerin değerini anlamak olduğundan sadece bir kaç temel özelliğin sınıfa dahil edildiğini hatırlatalım. UrunEntity isimli sınıfımızın genel tasarımı şu şekilde olacaktır.

![mk199_2.gif](images/mk199_2.gif)

Sınıfımız içerisindeki niteliklerin uygulanmasını ve metodlarımızı ilerleyen kısımlarda geliştireceğiz. Gelelim niteliklerimize. Makalemizin başındada belirttiğimiz gibi bir nitelik mutlaka Attribute sınıfından türemelidir ki metadata içerisine eklenebilsin. Bu nedenle sınıfımızın eşleştiği tablo ve kolonları için kullanılacak TabloAttribute ve AlanAttribute sınıflarını Attribute sınıfından türeterek geliştireceğiz.

> İsimlendirme standartları oldukça önemlidir. Bu tüm geliştiricilerin aynı tarzda kodlama yapmasını ve kooridanasyon kolaylığını sağlar. Örneğin tüm Exception sınıflarının adları Exception kelimesi ile biter veya tüm arayüzlerin (interfaces) adları I harfi ile başlar. Benzer durum nitelikler içinde geçerlidir. Öyleki nitelik sınıflarının adlarıda Attribute kelimesi ile bitmektedir. Bu nedenle kendi niteliklerimizi isimlendirirken adlarının Attribute kelimesi ile bitmelerine özen gösterilmelidir.

Niteliklerimize ait sınıf diagramı ve kodlarımız ise aşağıdaki gibidir.

![mk199_1.gif](images/mk199_1.gif)

TabloAttribute.cs;

```csharp
// TabloAttribute isimli niteliğimiz sadece sınıf veya yapılara uygulanabilecektir.
[AttributeUsage(AttributeTargets.Class|AttributeTargets.Struct)]
class TabloAttribute:Attribute
{
    private string _tabloAdi;
    private string _schemaAdi; 
  
    public string TabloAdi
    {
        get { return _tabloAdi; }
        set { _tabloAdi = value; }
    }
    public string SchemaAdi
    {
        get { return _schemaAdi; }
        set { _schemaAdi = value; }
    } 
  
    public TabloAttribute(string tablonunAdi, string schemaninAdi)
    {
        TabloAdi = tablonunAdi;
        SchemaAdi = schemaninAdi;
    }
    public TabloAttribute(string tablonunAdi)
        : this(tablonunAdi, "dbo")
    {
    }
    public TabloAttribute()
    {    
    } 
}
```

AlanAttribute;

```csharp
[AttributeUsage(AttributeTargets.Property)]
class AlanAttribute:Attribute
{
    private string _alanAdi;
    private bool _identity;
    private bool _nullIcerebilir; 

    public bool NullIcerebilir
    {
        get { return _nullIcerebilir; }
        set { _nullIcerebilir = value; }
    }

    public string AlanAdi
    {
        get { return _alanAdi; }
        set { _alanAdi = value; }
    }
    public bool Identity
    {
        get { return _identity; }
        set { _identity = value; }
    } 

    public AlanAttribute(string alaninAdi, bool identityMi, bool nullIcerirmi)
    {
        AlanAdi = alaninAdi;
        Identity = identityMi;
        NullIcerebilir = nullIcerirmi;
    }
    public AlanAttribute(string alaninAdi, bool identityMi)
        : this(alaninAdi, identityMi, true)
    {
    }
    public AlanAttribute(string alaninAdi)
        : this(alaninAdi, false)
    {
    }
    public AlanAttribute()
    {    
    } 
}
```

Şu andaki amacımız kendi niteliklerimizi nasıl yazacağımızı görmek olduğundan tam anlamıyla bir entity tipi oluşturmayı hedeflemiyoruz. Bu nedenle TabloAttribute isimli sınıfımız temel olarak eşleştirme amacıyla tablo adı ve bulunduğu şema adını taşıyacak özelliklere sahip. Benzer şekilde AlanAttribute isimli sınıfımızda alan adını, null değer taşınabilip taşınamıyacağını, alanın identity tipinde olup olmadığını belirten özellikler içermektedir. Gördüğünüz gibi Attribute'tan türettiğimiz sınıfların normal sınıflardan farklı bir yazım tarzı bulunmamaktadır.

Ancak dikkat ederseniz yazmış olduğumuz nitelik sınıflarımıza AttributeUsage isimli başka bir nitelik daha uygulanmaktadır. Bu niteliğin amacı, ilgili niteliğin hangi seviyelere uygulanabileceğini belirlemektir. Bu seviylerin belirtilmesi içinse AttributeTargets isimli bir enum sabitini ele almaktadır. Örneğin TabloAttribute niteliğimizi sadece sınıf (class) ve yapılara (struct) uygulayabilirken, AlanAttribute isimli niteliğimizi sadece özelliklere (Property) uygulanabilir. Böylece ilgili niteliğin sadece belirtilen tip veya üyelere uygulanabilmesi adına bir zorlama getirilmiş olunur. AttributeTargets isimli enum sabitinin alabileceği tüm değerler ve kısa açıklamaları aşağıdaki tabloda görüldüğü gibidir.

Değer
Açıklama

All
Nitelik istenilen tipe veya üyeye uygulanabilir.

Assembly
Nitelik sadece assembly seviyesinde uygulanabilir.

Class
Nitelik sadece sınıflara uygulanabilir.

Constructor
Nitelik sadece yapıcı metoda uygulanabilir.

Delegate
Nitelik sadece temsilci tipine uygulanabilir.

Enum
Nitelik sadece enum sabitine uygulanabilir.

Event
Nitelik sadece olaya uygulanabilir.

Field
Nitelik sadece alana uygulanabilir.

GenericParameter
Nitelik sadece generic bir parametreye (T) uygulanabilir.

Interface
Nitelik sadece arayüze uygulanabilir.

Method
Nitelik sadece metoda uygulanabilir.

Module
Nitelik sadece modül'e uygulanabilir. Burada dikkat edilmesi gereken nokta module'ün bir Visual Basic module'ü olmayışıdır. Yani kastedilen.dll veya.exe uzantılı module'lerdir.

Parameter
Nitelik sadece parametreye uygulanabilir.

Property
Nitelik sadece özelliğe uygulanabilir.

ReturnValue
Nitelik sadece dönüş tipine uygulanabilir.

Struct
Nitelik sadece bir değer türüne bir başka deyişle yapıya uygulanabilir.

Şimdi bu nitelikleri UrunEntitiy sınıfı içerisinde kullanmaya çalışalım. İlk olarak sınıfımızı aşağıdaki gibi geliştirelim.

```csharp
[Tablo(SchemaAdi="Production",TabloAdi="Product")]
class UrunEntity
{
    private int _urunId;
    private decimal _fiyat;
    private string _urunAdi;
    private DateTime _sonSatisTarihi; 

    [Alan(AlanAdi = "ProductID", Identity = true, NullIcerebilir = false)]
    public int UrunId
    {
        get { return _urunId; }
        set { _urunId = value; }
    }

    [Alan("Name", false, false)]
    public string UrunAdi
    {
        get { return _urunAdi; }
        set { _urunAdi = value; }
    }
    [Alan("ListPrice", Identity = false, NullIcerebilir = false)]
    public decimal Fiyat
    {
        get { return _fiyat; }
        set { _fiyat = value; }
    }

    [Alan("SellStartDate", false, true)]
    public DateTime SonSatisTarihi
    {
        get { return _sonSatisTarihi; }
        set { _sonSatisTarihi = value; }
    } 

    public UrunEntity(int idsi, string adi, decimal fiyati)
    {
        UrunId = idsi;
        UrunAdi = adi;
        Fiyat = fiyati;
    } 
    public UrunEntity()
    {
    }
}
```

Gördüğünüz gibi UrunEntity sınıfına ve UrunId,UrunAdi, Fiyat ve SonSatisTarihi isimli özelliklerimize TabloAttribute ve Alan Attribute niteliklerimiz uygulanmıştır. Buna göre UrunEntity sınıfının aslında Production şemasındaki Product tablosuna işaret ettiğini anlayabiliriz. Ya da, UrunId isimli özelliğin ProductId isimli alana işaret ettiğini, null değer içeremeyeceğini ve en önemliside Identity bir alan olduğunu anlayabiliriz. Böylece reflection tekniklerini kullanan kodlarımız insert sorgusunu oluştururken ProductId alanını hesabe katmayacağını anlayabilecektir.

Elbette bunun geliştirici tarafından kodlanması gerektiğinide unutmayalım. Diğer özellikler içinde benzer uygulamalar yapılmıştır. AlanAttribute ve TabloAttribute isimli sınıflarımız içersinde birden fazla aşırı yüklenmiş yapıcı metod (constructor) kullandığımızdan, niteliklerimizi söz konusu üyelere farklı biçimlerde uygulayabiliriz. Bunlardan birisi Name=Value ataması şeklinde olan versiyondur. Bu versiyon doğrudan public olan özelliklere (Property) değer atanabilmesini sağlar. Yani özel olarak aşırı yüklenmiş yapıcı metodlar olmasada ilgili niteliğin özellikleri değiştirilebilir.

![mk199_3.gif](images/mk199_3.gif)

Artık bundan sonra niteliklerimizi ele alacağımız kodlarımızı yazmamız gerekmektedir. Bu basit kod parçası ile, çalışma zamanında var olan nitelikleride nasıl okuyabileceğimizi ve buna göre nasıl davranış değiştirebileceğimizi görmüş olacağız. Bu amaçla UrunEntity isimli sınıfımızın Insert metodunu aşağıdaki gibi geliştirelim.

```csharp
public int Insert()
{
    Type tip = this.GetType();
    TabloAttribute tblAtr = ((TabloAttribute[])tip.GetCustomAttributes(typeof(TabloAttribute), false))[0];
    string tabloAdi=tblAtr.TabloAdi;
    string schemaAdi =tblAtr.SchemaAdi;
    StringBuilder insertBuilder = new StringBuilder();
    insertBuilder.Append("Insert into ");
    insertBuilder.Append(schemaAdi);
    insertBuilder.Append(".");
    insertBuilder.Append(tabloAdi);
    insertBuilder.Append(" (");

    // Insert sorgusundaki alan adları çekiliyor.
    foreach (PropertyInfo prp in tip.GetProperties())
    {
        AlanAttribute atr=((AlanAttribute[])prp.GetCustomAttributes(typeof(AlanAttribute), false))[0];
        if (!atr.Identity)
        {
            string alanAdi = atr.AlanAdi;
            insertBuilder.Append(alanAdi);
            insertBuilder.Append(",");
        }
    }
    // Son eklenen virgülü kaldırmak için.
    insertBuilder.Remove(insertBuilder.Length-1, 1);
    insertBuilder.Append(") Values (");
        
    // insert sorgusundaki değerleri çekiliyor.
    foreach (PropertyInfo prp in tip.GetProperties())
    {
        AlanAttribute atr=((AlanAttribute[])prp.GetCustomAttributes(typeof(AlanAttribute), false))[0];
        if (!atr.Identity)
        {
            object alanDegeri = prp.GetValue(this, null);
            if ((prp.PropertyType.Name == "String")
                || (prp.PropertyType.Name == "DateTime"))
                    insertBuilder.Append("'" + prp.GetValue(this, null).ToString() + "',");
            else
                insertBuilder.Append(prp.GetValue(this, null).ToString() + ",");
        }
    }
    insertBuilder.Remove(insertBuilder.Length - 1, 1);
    insertBuilder.Append(")");

    //Insert işlemi için gerekli sorgula çalıştırılır.

    return 0;
}
```

Kod parçası biraz arap saçına dönmüş olabilir. Gelin ne yaptığımıza ve nitelikleri çalışma zamanında nasıl ele alabildiğimize yakından bakalım. Insert metodunun amacı, UrunEntity sınıfı için gerekli olan insert sql sorgu cümlesini otomatik olarak oluşturmaktır. Bu işlemin çalışma zamanında yapılmasını hedeflediğimizden yoğun olarak reflection işlemleri kullanılmaktadır. Bununla birlikte otomatik olarak artan, bir başka deyişle identity tipinde olan bir alanın insert sorgusuna dahil edilmemesi gerekmektedir.

Öyleyse bu bilgiyi içeren nitelik (attribute) ele alınmalıdır. Benzer şekilde özelliklerin hangi tablo alanlarına denk geldiği veya sınıfın hangi şemadaki hangi tabloya denk geldiği bilgileride niteliklerimizden alınmalıdır. Bu ihtiyaçlar doğrultusunda geliştirilen kodlar göz önüne alındığında herhangibir tipin çalışma zamanında uygulanan niteliğini elde etmek amacıyla GetCustomAttributes isimli metod kullanılmaktadır. Bu metod ilk parametre olarak elde edilmek istenen niteliğin tipini alır. Metodun belkide en önemli özelliği geriye object tipinden bir dizi döndürüyor oluşudur. Dönen bu dizi içerisinden niteliğe ait özellikleri çekebilmek için bir dönüştürme (cast) işlemi yapılmalıdır.

GetCustomAttributes metodunun geriye dizi döndürmesinin sebebi, bir tipe veya üyeye birden fazla niteliğin uygulanabilecek olmasıdır. Elde edilen dizi tekrardan uygun nitelik (Attribute) tipine dönüştürüldüğünde indisleme operatörü sayesinde okunmak istenen özelliklere erişilebilir. Ki örneğimizde 0 indisli referanslar çekilmiştir. Insert metodunda kullandığımız aşağıdaki kod parçasında UrunEntity sınıfına uygulanan TabloAttribute'tipinin referansı yakalanmaktadır.

```csharp
TabloAttribute tblAtr = ((TabloAttribute[])tip.GetCustomAttributes(typeof(TabloAttribute), false))[0];
```

Burada kullanılan GetCustomAttributes metodu tipe aittir. Sınıf içerisindeki üyelere uygulanan nitelikleri elde etmek içinde aynı yol kullanılır. Nitekim, nitelik (attribute) uygulanabilen tüm üyelerin GetCustomAttributes metodu bulunmaktadır. Söz gelimi, UrunEntity sınıfındaki özelliklere (properties) uygulanan nitelikleri çalışma zamanında elde edebilmek için GetProperties metodu ile gezilen PropertyInfo referanslarına GetCustomAttributes metodu aşağıdaki gibi uygulanmıştır.

```csharp
AlanAttribute atr=((AlanAttribute[])prp.GetCustomAttributes(typeof(AlanAttribute), false))[0];
```

Bu şekilde nitelik referansları elde edildikten sonra söz konusu niteliğin üyelerinin değerlerine bakılabilir.

> Her ne kadar makalemizin konusu nitelikleri yazmak olsada reflection ile ilgili bazı noktalara da değinmek gerekir. Örneğin Insert metodu içerisinde çalışma zamanında o anki UrunEntity nesne örneğinin özelliklerinin değerlerinin elde edilmesi için, GetValue isimli metod kullanılmıştır. Bu metodun ilk parametresi, değerleri taşıyan nesne örneğinin referansıdır.

Örneğimizi herhangibir program içerisinde test etmek için aşağıdaki gibi bir kod parçasından faydalanabiliriz. Bu amaçla örnek bir Console uygulamasını test programı amacıyla kullanabiliriz. Tek yapmamız gereken bir UrunEntity nesne örneği oluşturmak sonrasında ilgili özelliklerinde bazı değerler atamak ve Insert metodunu çağırmaktır.

```csharp
UrunEntity urn = new UrunEntity();
urn.UrunAdi = "Pentium CPU";
urn.Fiyat = 90;
urn.SonSatisTarihi = DateTime.Now.AddDays(30);
urn.Insert();
```

Amacımız nitelikleri kavramak olduğu için, Insert metodunun içerisinde insert sorgusunu çalıştırmak için gereken Data Access Layer çağrıları yazılmamıştır. Ancak sonuçları görmek adına çalışma zamanında Insert metodunun çağırıldığı satıra bir breakpoint koyarak adım adım (step into) ilerlemekte fayda vardır. Bunun sonucunda aşağıdaki ekran görüntüsünde olduğu gibi Insert sorgusunun doğru bir şekilde oluşturulduğunu görebiliriz. Dikkat ederseniz UrunId özelliği hiç bir şekilde hesaba katılmamıştır. Ayrıca özelliklerin tabloda karşılık olan adlarına bakılarak bu sorgu cümlesi oluşturulmuştur.

![mk199_4.gif](images/mk199_4.gif)

Program kodumuzdan üretilen assembly içerisine ildasm.exe aracı yardımıyla bakmakta fayda vardır. Bunu yaptığımızda yazdığımız niteliklerin o anki bilgileri ile birlikte assembly'ın metadata'sına eklendiğini görebiliriz. (Ildasm aracında metadata'yı görebilmek için Ctrl+M tuş kombinasyonunu kullanırız.) Örnek olark UrunId isimli özelliğimiz için eklenen nitelik metadata içerisinde aşağıdaki şekilde görünecektir.

AlanAttribute'unun Metadata izi;

```bash
Property #1 (17000006)
-------------------------------------------------------
Prop.Name : UrunId (17000006)
Flags : [none] (00000000)
CallCnvntn: [PROPERTY]
hasThis 
ReturnType: I4
No arguments.
DefltValue: 
Setter : (06000015) set_UrunId
Getter : (06000014) get_UrunId
0 Others
CustomAttribute #1 (0c000011)
    -------------------------------------------------------
    CustomAttribute Type: 06000013
    CustomAttributeName: AttributeTemelleri.AlanAttribute :: instance void .ctor()
    Length: 54
    Value : 01 00 03 00 54 0e 07 41 6c 61 6e 41 64 69 09 50 > T AlanAdi P<
                : 72 6f 64 75 63 74 49 44 54 02 08 49 64 65 6e 74 >roductIDT Ident<
                : 69 74 79 01 54 02 0e 4e 75 6c 6c 49 63 65 72 65 >ity T NullIcere<
                : 62 69 6c 69 72 00 >bilir <
    ctor args: ()
```

Burada açık bir şekilde niteliğin (attribute) özelliklerine atanan değerler de görülebilmektedir. Benzer şekilde UrunEntity isimli sınıfımıza uygulanan TabloAttribute niteliğinin metadata içerisine yaptığı katkıyıda görebiliriz.

TabloAttribute'unun Metadata izi;

```bash
CustomAttribute #1 (0c000010)
-------------------------------------------------------
CustomAttribute Type: 06000009
CustomAttributeName: AttributeTemelleri.TabloAttribute :: instance void .ctor()
Length: 46
Value : 01 00 02 00 54 0e 09 53 63 68 65 6d 61 41 64 69 > T SchemaAdi<
            : 0a 50 72 6f 64 75 63 74 69 6f 6e 54 0e 08 54 61 > ProductionT Ta<
            : 62 6c 6f 41 64 69 07 50 72 6f 64 75 63 74 >bloAdi Product <
    ctor args: ()
```

Yine gördüğünüz gibi SchemaAdi özelliğine atanan Production değeri ile TabloAdi'na atanan Product değerleri buraya Value olarak alınmıştır.

Program kodlarımızı daha da geliştirmek siz değerli okurlarımızın elindedir. Örneğin, Insert, Update, Delete ve Load metodların geliştirebilir bunları gerekirse bir base sınıf içerisinde toplanabilir. Böylece geldik bir makalemizin daha sonuna. Bu makalemizde, nitelikleri (attribute) daha yakından tanımaya çalıştık ve kendi niteliklerimizimi (Custom Attributes) nasıl yazabileceğimizi incelemeye çalıştık. Gördüğünüz gibi niteliklerde birer sınıf olarak düşünüldüklerinde geliştirilmeleri son derece kolay tiplerdir. Ne varki niteliklerin asıl gücü çalışma zamanında reflection kullanıldığında ortaya çıkmaktadır. Tekrardan hatırlatmak gerekirse, amaç çalışma zamanında tiplere ve üyelere nasıl davranılacağına dair kararların verilmesinde assembly içerisindeki ekstra metadata bilgilerinde faydalanılmasıdır. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.

[Örnek Uygulama İçin Tıklayınız.](assets/AttributeKullanimi.rar)
