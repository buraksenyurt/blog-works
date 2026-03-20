---
layout: post
title: "Web Server Control Yazmak - 2"
date: 2007-01-23 12:00:00 +0300
categories:
  - aspnet-2-0
tags:
  - aspnet-2-0
  - csharp
  - dotnet
  - aspnet
  - serialization
  - reflection
  - visual-studio
---
Bir önceki makalemizde web sunucu kontrollerini nasıl geliştireceğimizi incelemeye başlamıştık. Bu günkü makalemizde, ViewState'lerin web sunucu kontrollerinde nasıl ele alınabileceğinden ve kontrollerin tasarım zamanındaki (design time) yeteneklerinin nitelikler (attributes) yardımıyla nasıl arttırılabileceğinden bahsetmeye çalışacağız.

Herhangibir web sunucu kontrolünün (web server control) bazı özellikleri (properties) sayfa üretilirken belirlenmek (set) istenebilir. Bu işlem için sayfanın PageLoad olay (event) metodu ideal bir noktadır. Hatta burada sayfanın Postback edilip edilmediği kontrol edilerek, özelliklerin sadece sayfanın ilk yüklenişinde belirlenmesi sağlanabilir. Ancak burada ViewState kullanılmadığı takdirde karşılaşılabilecek önemli bir problem vardır. Bu problemi analiz edebilmek için, bir önceki makalemizde geliştirdiğimiz TarihKontrolum isimli bileşenimize üç yeni özellik daha ekleyerek analizimize başlayalım. Bu özellikler ile liste kutularındaki gün, ay, yıl bilgilerinin değerlerini değiştirmeyi ve elde etmeyi hedeflemekteyiz.

```csharp
private string _seciliGun;
private string _seciliAy;
private string _seciliYil;

public string SeciliGun
{
    get{return _seciliGun;}
    set { _seciliGun = value; }
}

public string SeciliAy
{
    get{return _seciliAy;}
    set { _seciliAy= value; }
}

public string SeciliYil
{
    get{ return _seciliYil; }
    set { _seciliYil= value; }
}
```

Çok doğal olarak web sunucu kontrolümüze ilişkin Render metodu içerisinde de bazı değişiklikler yapmalıyız. Nitekim sayfa yüklenirken belirlenen gün, ay ve yıl değerlerinin, render edilen HTML içeriğine selected niteliği olarak aktarılması gerekmektedir. Bu nedenle Render metodu içerisinde, özelliğin o anki değeri ile for döngüsü içerisindeki değerler karşılaştırılmış ve eşleşme olması halinde, ekrana basılacak olan option elementinin yapısı değiştirilmiştir.

```csharp
protected override void Render(HtmlTextWriter writer)
{
    writer.Write("<span id='lblGun'>" + GunMetin + "</span>");
    writer.Write("  ");
    writer.Write("<select name='Gun' id='Gun'>");
    for (int i = 1; i <= 31; i++)
    {
        if (SeciliGun == i.ToString())
            writer.Write("<option selected='selected' value='" + i.ToString() + "'>" + i.ToString() + "</option>");
        else
            writer.Write("<option value='" + i.ToString() + "'>" + i.ToString() + "</option>");
    }
    writer.Write("</select>");
    writer.Write("  ");
    writer.Write("<span id='lblAy'>" + AyMetin + "</span>");
    writer.Write("  ");
    writer.Write("<select name='Ay' id='Ay'>");
    for (int i = 1; i <= 12; i++)
    {
        if (SeciliAy == i.ToString())
            writer.Write("<option selected='selected' value='" + i.ToString() + "'>" + i.ToString() + "</option>");
        else
            writer.Write("<option value='" + i.ToString() + "'>" + i.ToString() + "</option>");
    }
    writer.Write("</select>");
    writer.Write("  ");
    writer.Write("<span id='lblYil'>" + YilMetin + "</span>");
    writer.Write("  ");
    writer.Write("<select name='Yil' id='Yil'>");
    for (int i = 1950; i <= 2050; i++)
    {
        if (SeciliYil == i.ToString())
            writer.Write("<option selected='selected' value='" + i.ToString() + "'>" + i.ToString() + "</option>");
          else
            writer.Write("<option value='" + i.ToString() + "'>" + i.ToString() + "</option>");
    }
    writer.Write("</select>");
    base.Render(writer);
}
```

Dikkat ederseniz, özelliklerimizin değerlerini, for döngüleri içerisinde yakalıyoruz ve buna göre option elementine selected niteliğini (attribute) ekliyoruz. Peki, kontrolümüzü bu son haliyele sayfamızda kullandığımızda ve sayfanın Load olay metodunda aşağıdaki kodları yazdığımızda neler olacaktır?

```csharp
protected void Page_Load(object sender, EventArgs e)
{
    if (!Page.IsPostBack)
    {
        TarihKontrolumOld1.SeciliGun = "3";
        TarihKontrolumOld1.SeciliAy = "4";
        TarihKontrolumOld1.SeciliYil = "2008";
    }
}
```

Load olay metodu içerisind, eğer sayfa ilk kez yükleniyorsa (ki bu kontrolü IsPostBack özelliğinin değerine bakarak yapıyoruz), web sunucu kontrolümüzün ilgili özelliklerine bazı başlangıç değerleri set edilmektedir.

![mk189_1.gif](/assets/images/2007/mk189_1.gif)

Ancak sayfayı tekrardan sunucuya gönderdiğimizde, IsPostBack kontrolü nedeni ile if bloğu içerisindeki kodlar bir kez daha çalıştırılmayacaktır. Bu nedenlede sayfanın bir sonraki talep (request) için çıktısı aşağıdaki gibi olacaktır. Dikkat ederseniz, SeciliGun, SeciliAy ve SeciliYil özellikleri başlangıç değerlerine göre yeniden belirlenmiştir. (Bu durum, kontrolün her sayfa istediğinde yeniden örneklenişinin ve ilk değerlerin atanışının doğal bir sonucu olarak düşünülebilir.)

![mk189_2.gif](/assets/images/2007/mk189_2.gif)

Aslında sorun, istemci (client) ile sunucu (server) arasında web sunucu kontrolü içerisindeki verilerin (data) taşınmıyor oluşudur. Bu amaçla bildiğiniz gibi Asp.Net uygulamalarında ViewState'lerden faydalanabiliriz. ViewState'ler sunucu tarafından yeniden üretilen, sayfalar içerisindeki verileri saklamak amacıyla istemci tarafındaki çıktıya ilave edilen gizli alanlardır (hidden field). Bu alanın içeriği string bazlıdır. Dikkat edilmesi gereken noktalardan birisi, ViewState'in içerisinde çok yüksek boyutlu veri taşımasının, üretilecek olan HTML içeriğini fazlasıyla şişirecek olmasıdır.

Bu nedenle ViewState içerisine veri atarken çoğunlukla string tipine dönüştürülebilen ve çok yüksek boyut içermeyen bilgileri ele almakta fayda vardır. TarihKontrolum isimli bileşenimiz içerisinde GunMetin, AyMetin, YilMetin, SeciliGun, SeciliAy, SeciliYil özelliklerinin içeriklerini ViewState içerisinde saklayabiliriz. Böylece yukarıdaki modele göre sayfanın ilk üretilişinde hazırlanan veri içeriği istemci tarafındaki çıktı içerisinde yer alan gizli alana ilave edilecektir. Bu ihtiyacı cevaplandırabilmek amacıyla, TarihKontrolum bileşeninde aşağıdaki değişiklikleri yapmamız yeterli olacaktır.

![mk189_3.gif](/assets/images/2007/mk189_3.gif)

```csharp
public class TarihKontrolum:Control
{
    public string GunMetin
    {
        get {
            if (ViewState["gunMetin"] != null)
                return ViewState["gunMetin"].ToString();
            else
                return "Gün"; 
        }
        set { ViewState["gunMetin"] = value; }
    }

    public string AyMetin
    {
        get
        {
            if (ViewState["ayMetin"] != null)
                return ViewState["ayMetin"].ToString();
            else
                return "Ay";
        }
        set { ViewState["ayMetin"] = value; }
    }

    public string YilMetin
    {
        get
        {
            if (ViewState["yilMetin"] != null)
                return ViewState["yilMetin"].ToString();
            else
                return "Yıl";
        }
        set { ViewState["yilMetin"] = value; }
    }

    public string SeciliGun
    {
        get
        {
            if (ViewState["seciliGun"] != null)
                return ViewState["seciliGun"].ToString();
            else
                return "";
        }
        set { ViewState["seciliGun"] = value; }
    }

    public string SeciliAy
    {
        get
        {
            if (ViewState["seciliAy"] != null)
                return ViewState["seciliAy"].ToString();
            else
                return "";
        }
        set { ViewState["seciliAy"] = value; }
    }

    public string SeciliYil
    {
        get
        {
            if (ViewState["seciliYil"] != null)
                return ViewState["seciliYil"].ToString();
            else
                return "";
        }
        set { ViewState["seciliYil"] = value; }
    }

    protected override void Render(HtmlTextWriter writer)
    { 
        writer.Write("<span id='lblGun'>" + GunMetin + "</span>");
        writer.Write("  ");
        writer.Write("<select name='Gun' id='Gun'>");
        for (int i = 1; i <= 31; i++)
        {
            if (SeciliGun==i.ToString())
                writer.Write("<option selected='selected' value='" + i.ToString() + "'>" + i.ToString() + "</option>");
            else
                writer.Write("<option value='" + i.ToString() + "'>" + i.ToString() + "</option>");
        }
        writer.Write("</select>"); 
        writer.Write("  ");
        writer.Write("<span id='lblAy'>" + AyMetin + "</span>");
        writer.Write("  ");
        writer.Write("<select name='Ay' id='Ay'>");
        for (int i = 1; i <= 12; i++)
        {
            if (SeciliAy == i.ToString()) 
                writer.Write("<option selected='selected' value='" + i.ToString() + "'>" + i.ToString() + "</option>");
            else
                writer.Write("<option value='" + i.ToString() + "'>" + i.ToString() + "</option>");
        }
        writer.Write("</select>");
        writer.Write("  ");
        writer.Write("<span id='lblYil'>" + YilMetin + "</span>");
        writer.Write("  ");
        writer.Write("<select name='Yil' id='Yil'>");
        for (int i = 1950; i <= 2050; i++)
        {
            if (SeciliYil==i.ToString())
                writer.Write("<option selected='selected' value='" + i.ToString() + "'>" + i.ToString() + "</option>");
            else
                writer.Write("<option value='" + i.ToString() + "'>" + i.ToString() + "</option>");
        }
        writer.Write("</select>");
        base.Render(writer);
    }
}
```

Özelliklerimiz hepsi aynı prensiple çalışmaktadır. Get bloklarında, eğer ViewState içerisinde tutulan bir veri var ise bu veri string tipine dönüştürülerek elde edilir. Set bloğunda ise özelliğe atanan değerler, ViewState içerisine bazı anahtar ifadeler (key) ile eklenmektedir. Örneğin SeciliGun özelliğinin işaret edeceği değer için ViewState içerisinde seciliGun isimli bir anahtara (key), değer ataması yapılmıştır. Set blokları, dikkat ederseniz sayfanın Load olay metodu içerisindeki kodlarda devreye girmektedir.

Bu nedenle sayfa ilk üretilişi sırasında bu özelliklere başlangıç değerleri atanmaktadır. Bu atama sonrasındaki değerler, istemciye gönderilecek olan içerikte yer alan ViewState alanı içerisine dahil edilecektir. Dolayısıyla istemci sayfayı tekrardan sunucuya gönderdiğinde, açılan (yada deserialization işlemine tabi tutulan) ViewState içerisinden, özelliklerin değerleri alınacaktır. Bu değerleride Render metodu içerisinde özelliklerin Get bloklarından aldığımızdan, kontrolün ilgili özellikleri başlangıç değerlerine döndürülmeyecektir. Aşağıdaki video görüntüsünde kontrolümüzün yeni ve eski halleri incelenmeye çalışılmıştır.

Üst taraftaki kontrolümüzü ViewState kullanmayan TarihKontrolumOld isimli sınıfa ait bir örnektir. Alttaki kontrolümüz ise TarihKontrolum sınıfına ait ViewState kullanan nesne örneğimizdir. Halen daha bazı problemlerimiz vardır. Örneğin kullanıcı tarayıcı penceresi içerisindeyken, gün, ay veya yıl bilgilerinden birisini değiştirip sayfayı tekrardan sunucuya gönderirse, değiştirdiği içeriği son hali ile elde edemeyecektir. Bu sayfanın içerisindeki kontrollerin verisinin istemciden yapılan değişiklikler sonucu, sunucuya gönderilmeyişinden kaynaklanmaktadır. Sorunu çözmek için postback işlemi sonucu istemciden gelen veriyi işlememiz gerekmektedir. Bu sorunu nasıl çözeceğimizi yazı dizimizin ilerleyen makalelerinde incelemeye çalıçacağız.

Makalemizin bundan sonraki bölümünde, sunucu kontrollerimizin tasarım zamanındaki (design time) niteliklerini (attribute) nasıl değiştirebileceğimizi görmeye çalışacağız. Nitelikleri (attribute) sınıf (class), özellik (property) veya assemblye bazında uygulayabiliriz. Bu nitelikler, geliştirdiğimiz web sunucu kontrollerini kullanan sayfa geliştiriciler için önemlidir. Nitekim, web sunucu kontrolünün Visual Studio.Net ortamı içerisindeki davranışlarını belirlemektedir.

> Attribute'lar uygulandıkları tip veya üyelerin çalışma zamanındaki davranışlarını belirlememize yarayan tiplerdir. Assembly'ların Metadata'sına eklendikleri için çalışma zamanıda reflection yardımıyla ele alınırlar ve çalışma ortamının gerekli davranış değişikliklerini yapabilmelerini sağlarlar. Çok bilinen attribute'lara örnek olarak Serialization, WebMethod, WebService vb'lerini verebiliriz.

Geliştireceğimiz web sunucu kontrollerinde kullanabileceğimiz sınıf (class), özellik (property) ve assembly seviyesindeki bazı nitelikler (attributes) ve işlevleri aşağıdaki tabloda özetlenmeye çalışılmıştır.

Assembly Bazındaki Nitelikler (Attributes)
Açıklaması

TagPrefix
Kontrol sayfaya sürüklenip bırakıldığında, arka taraftaki elemente ait ön ekin (prefix) ne olacacağını belirleyebilmemizi sağlar.

Sınıf Bazındaki Nitelikler (Attributes)
Açıklaması

ToolBoxData
Bu nitelik ile kontrolün, sayfanın arka tarafında nasıl yazılacağını belirtebiliriz.

DefaultProperty
Kontrol sayfaya ilk sürüklendiğinde ele alınacak olan varsayılan özelliğin (default property) ne olacağını belirtir. Örneğin bir Button bileşeni için Text özelliği varsayılandır.

DefaultEvent
Kontrol sayfaya ilk sürüklendiğinde ele alınacak olan varsayılan olayın (default event) ne olacağını belirtir. Örneğin bir Button bileşeni için Click olayı varsayılandır.

Özellik Bazındaki Nitelikler (Attributes)
Açıklaması

Browsable
Bu nitelik ile, özelliğin Vs.Net içerisindeki Properties penceresinde gösterilip gösterilmeyeceği belirlenir. Varsayılan olarak özelliğin değeri true'dur. Üstelik kontroller içerisinde kullanılan her özellik bu niteliğe varsayılan olarak sahiptir.

Description
Özellik ile ilgili bir açıklama girilmesini sağlar. Bu, özellikle sayfa geliştiriciler (page developers) için önemlidir. Çünkü özelliğin ne amaçla kullanılacağı hakkında bilgilerin verilmesini sağlamaktadır.

Bindable
Eğer özellik, veriye bağlanacaksa (data binding) bu niteliğin kullanılması gerekir.

Themeable
Özelliğe theme ve dolayısıyla skin uygulanabilmesini sağlamak istiyorsak bu özelliği kullanırız.

Category
Özelliğin hangi kategori (Category) başlığı altında sunacağımızı belirtir.

DefaultValue
Özelliğin varsayılan olarak hangi değeri (default value) taşıyacağını belirtmektedir.

Localizable
Eğer bu nitelik false olarak belirtilirse, özelliğin içeriği söz konusu aspx için üretilen resource sayfalarında gösterilmez. True olması halinde ise gösterilir. Buda ilgili özelliğin yerelleştirmede kullanılabilmesini sağlar.

DesignOnly
Özelliğin sadece tasarım zamanında ele alınıp alınmayacağını belirtir.

Dilerseniz bu nitelikleri ve etkilerini incelemeye çalışalım. İlk olarak assembly seviyesindeki TagPrefix niteliğini ele alacağız. Bu niteliği, kontrollerimizi barındıran kütüphane içerisindeki AssemblyInfo.cs dosyası içerisinde kullanmamız gerekmektedir. TagPrefix niteliği System.Web.UI içerisinde yer aldığından, gerekli isim alanınında (namespace) bu sınıf içerisine dahil etmekte fayda vardır. Temel olarak TagPrefix niteliği, kontrolün web sayfası üzerine sürüklenmesi sonrasında arka planda oluşturulacak olan elementin ön ekini belirlemek amacıyla kullanılır. Varsayılan olarak cc1 olan kontrol elementi ön ekini değiştirmemizi sağlayacaktır.

![mk189_4.gif](/assets/images/2007/mk189_4.gif)

Dikkat ederseniz TagPrefix niteliğine ait yapıcı metod (constructor) iki parametre almaktadır. Bu parametrelerden ilki, kontrolün bulunduğu isim alanının adıdır. İkinci parametre ise ön ekin adıdır. Bu değişiklilerden sonra kontrolümüzü herhangibir sayfa üzerine sürükleyip bıraktığımızda aşağıdaki sonucu elde ederiz.

![mk189_5.gif](/assets/images/2007/mk189_5.gif)

Gördüğünüz gibi kontrolümüzün ön eki, assembly dosyasında belirttiğimiz şekilde değiştirilmiştir. Gelelim diğer niteliklere. Sınıf bazında (Class Level) uygulayabileceğimiz nitelikler için aşağıda bir örnek yer almaktadır.

```csharp
[DefaultProperty("SeciliGun")]
[ToolboxData("<{0}:TarihKontrolum runat='Server'><{0}:TarihKontrolum>")]
public class TarihKontrolum:Control
{
```

Burada örnek olarak ToolboxData ve DefaultProperty nitelikleri kullanılmıştır. ToolboxData, yazım biçimindende görüldüğü gibi, kontrolün aspx tarafında nasıl yazılacağını belirtmektedir. Tahmin edeceğiniz gibi {0} yazan yere, AssemblyInfo.cs dosyasında tanımlanan TagPrefix niteliğindeki değer gelecektir. İstenirse runat='Server'yazıldığı yerde başka attribute'lar konularak, örneğin kontrolün özelliklerinin ilk değerlerinin gösterilmesi sağlanabilir. Aşağıdaki kod parçasında bu durum gösterilmiştir.

```csharp
[ToolboxData("<{0}:TarihKontrolum SeciliGun='10' SeciliAy='2' SeciliYil='2007' runat='Server'><{0}:TarihKontrolum>")]
```

Burada tanımlanan SeciliGun, SeciliAy ve SeciliYil niteliklerinin aslında, TarihKontrolum sınıfı içerisindeki özelliklere (properties) işaret ettiğine dikkat edelim. Yaptığımız değer atamaları sonucu kontrolün bu özelliklerinin ilk değerleride otomatik olarak set edilmiş olacaktır.

![mk189_7.gif](/assets/images/2007/mk189_7.gif)

Kontrolün arka plandaki çıktısı ise aşağıdaki gibi olacaktır.

```csharp
<Kontrolum:TarihKontrolum ID="TarihKontrolum1" runat="server" SeciliAy="2" SeciliGun="10" SeciliYil="2007">
</Kontrolum:TarihKontrolum>
```

Sınıf bazında kullandığımız niteliklerden DefaultProperty, kontrol sayfaya sürüklenip bırakıldığında, Vs.Net 2005 IDE'si içerisindeki özellikler penceresinde, hangi özelliğin varsayılan olarak seçili olacağını belirtmektedir. Örneğimizdeki TarihKontrolum bileşeni için varsayılan özellik SeciliGun'dür.

![mk189_6.gif](/assets/images/2007/mk189_6.gif)

Gelelim özellik bazında kullandığımız niteliklere. Kontrolümüzdeki özelliklerin hepsine yukarıda bahsettiğimiz nitelikleri uygulayabiliriz. Örneğin SeciliGun için aşağıdaki örnek kod parçasında tasarım penceresine etki edecek nitelikler eklenmiştir.

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
    get
    {
        if (ViewState["seciliGun"] != null)
            return ViewState["seciliGun"].ToString();
        else
            return "";
    }
    set { ViewState["seciliGun"] = value; }
}
```

Temel olarak Browsable niteliğini yazmak şart değildir. Nitekim özelliklerin hepsi, tasarım zamanında özellik penceresinde görünürler. Ancak bazı özelliklerin burada değiştirilmesini istemediğimiz vakkalar olursa, false olarak işaretlenebilir. DefaultValue özelliğine her ne kadar başlangıç için 1 değerini atamış olsakta, ToolboxData niteliği içerisinde bu özellik için yeni bir değer verilmiştir.

Bunun doğal sonucu olarak, DefaultValue özelliğindeki değer yerine ToolboxData içerisindeki özellik değeri, özellikler penceresinde görünecektir. Themeable niteliği, Asp.Net 2.0 ile birlikte gelmiş olan tema kavramına istinaden kullanılabilir. Eğer false değeri atanırsa özelliğimize tema veya skin uygulanamayacaktır. Localizable özelliğine true değerinin atanması ile, SeciliGun değerinin resource dosyalarında görünmesi sağlanır. Örneğin tüm özelliklerimizde bu niteliği true olarak kullandığımızda aşağıdaki gibi bir resource dosyası üretilebilir.

![mk189_8.gif](/assets/images/2007/mk189_8.gif)

Categroy niteliğini kullandığımız takdirde, özelliklerin properties bölümünde mantıksal isim başlıkları altında yer alması sağlanabilir. Örneğin SeciliGun,SeciliAy, SeciliYil özelliklerini Tarih Degerleri isimli bir grup altında toplayabiliriz. Benzer bir şekilde, AyMetin, GunMetin ve YilMetin özelliklerini Metinler adında bir grup başlığında toplayabiliriz. Sonuç aşağıdakine benzer olacaktır.

![mk189_9.gif](/assets/images/2007/mk189_9.gif)

Gördüğünüz gibi, geliştireceğimiz kontrollerin tasarım zamanındaki davranışlarını niteliklerimiz yardımıyla belirleyebilmekteyiz. Bu makalemizde web kontrolü geliştirmekle ilgili olarak, state yönetimini ViewState'ler yardımıyla nasıl yapabileceğimizi, tasarım zamanındaki davranışları nasıl belirleyebileceğimizi incelemeye çalıştık. Şu ana kadar yaptıklarımız ile ilgili olarak aklımızda kalanları ise aşağıdaki maddeler ile özetleyebiliriz.

Şu Ana Kadar Hatırda Kalanlar

Bir web sunucu kontrolünün sayfanın yüklenişi sırasında set edilen değerlerini korumak için özelliklerin içerisindeki verileri ViewState'ler içerisinde saklamak gerekir.

ViewState kullanmak, kontrolün istemci tarafında değişikliğe uğrayacak değerlerini ele almak için yeterli değildir. Bu durumda işin içerisinde postback sonucu sunucuya gönderilecek dataların ele alınması gibi şartlar girmektedir.

Web sunucu kontrollerine tasarım zamanı desteği sağlanabilir. Böylece Vs.Net IDE'si içerisindeki davranış modelleri ele alınabilir.

Tasarım zamanı davranışlarını belirleyen nitelikleri Assembly, Class ve Property olmak üzere toplam üç seviyede belirleyebiliriz.

Böylece geldik bir makalemizin daha sonuna. Sonraki makalemizde Render metodu ile ilişkili başka düzenlemer yapacak ve PostBack olaylarını nasıl ele alabileceğimizi araştırmaya çalışacağız. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.