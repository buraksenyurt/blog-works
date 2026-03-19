---
layout: post
title: "Kendi Web Part Bilesenlerimizi Gelistirmek - 2"
date: 2007-04-20 12:00:00 +0300
description: "Kendi web partlarımızı nasıl geliştirebileceiğimizi ve bu sayede kişiselleştirilebilir web sunucu kontrollerini nasıl yazabileceğimizi bu konu ile ilgili bir önceki makalemizde incelemeye çalışmıştık. Bu makalemizde ise kendi Web Part bileşenlerimize özel fiillerin (Web Part Verbs) nasıl eklenebileceğini ve söz konusu fillerin ne şekilde ele alınabileceğini incelemeye çalışacağız."
categories:
  - aspnet-2-0
tags:
  - aspnet-2-0
  - csharp
  - xml
  - threading
  - delegates
---
# Kendi Web Part Bilesenlerimizi Gelistirmek - 2
Değerli Okurlarım Merhabalar,

Kendi web partlarımızı nasıl geliştirebileceiğimizi ve bu sayede kişiselleştirilebilir web sunucu kontrollerini nasıl yazabileceğimizi bu konu ile ilgili bir önceki makalemizde incelemeye çalışmıştık. Bu makalemizde ise kendi Web Part bileşenlerimize özel fiillerin (Web Part Verbs) nasıl eklenebileceğini ve söz konusu fillerin ne şekilde ele alınabileceğini incelemeye çalışacağız. Web Part kontrollerini herhangibir WebPartZone altında kullandığımızda standart olarak bazı fiilere (Verbs) sahip oluruz. Tahmin edeceğiniz gibi bu değerler aslında WebPartManager tarafından ele alınmakta ve sayfa üzerinde uygun olan web part alanlarının (Web Part Zone) gösterilmesini sağlamaktadır.

Söz gelimi kullanıcı Edit isimli fiili (Verb) seçtiğinde WebPartManager bileşeni, EditorZone kontrolünü aktif hale getirmekte ve söz konusu Web Part kontrolünün değiştirilebilen yada düzenlenebilen özelliklerinin (Properties) bulunduğu bir bileşeni (örneğin PropertyGridEditorPart kontrolü) göstermektedir. Bu açıdan bakıldığında fiillerin (Verbs) varsayılan olarak WebPartManager bileşenine ait Display Mode değerleri ile yakın bir ilişkide olduklarını söyleyebiliriz. Elbette farklı şekilde davranabilen fiillerde (Verbs) vardır. Örneğin Minimize fiili, bulunduğu Web Part'ın içerisinde yer aldığı WebPartZone alanının küçülmesini sağlarken, Restore fiili tekrardan eski haline getirilmesine olanak vermektedir.

Kendi Web Part bileşenlerimizi geliştirdiğimizde var olan fiillerin (Verbs) bize yetmediği durumlar söz konusu olabilir. Bu sebepten dolayı istersek kendi Web Part fiillerimizi (Web Part Verbs) oluşturabilir ve kod tarafında ele alarak farklı aksiyonların gerçekleştirilmesini sağlayabiliriz. Burada temel dayanak noktası WebPartVerb isimli sınıftır (class). Bu sınıf IStateManager isimli arayüzü uyarlayan (implement) bir tiptir. Kullanım amacı, Web Part bileşeni için özel bir fiili tanımlamaktır.

Fiile ait isim (Name), açıklama (Description), seçilme durumu (Checked), resmi (ImageUrl) vb gibi bilgileri içerisinde barındıran bir sınıftır. Kullanıcılar bu fiili seçtiklerinde bir aksiyonun gerçekleştirilmesi gerektiği açık bir şekilde ortadadır. Buda doğal olarak bir metodun çalışma zamanında (run time) tetiklenmesi anlamına gelmektedir. İşte bu sebeple tanımlanan fiilerin gerçekleşmesi halinde çalıştırılacak olan metodları işaret eden WebPartEventHandler temsilcilerinden (delegates) faydalanılmaktadır. Bu temsilcinin prototipi ise aşağıdaki gibidir.

```csharp
public delegate void WebPartEventHandler(object sender, WebPartEventArgs e);
```

Dikkat ederseniz WebPartEventHandler temsilcisi standart bir olay temsilcisidir. İkinci paramete olay metoduna bazı bilgileri taşımakta olan WebPartEventArgs sınıfına ait bir nesne örneğidir. Bu tipte doğal olarak EventArgs sınıfından türetilmiştir. İlk parametre ise fiili gerçekleştiren referansın bir başka deyişle WebPartVerb nesne örneğinin taşıyıcısıdır.

> Hatırlayalım; Temsilciler (Delegates) çalışma zamanında metodların bellek üzerindeki başlangıç adreslerini işaret eden tiplerdir (types). Tanımlandıklarında, işaret edebilecekleri metodun yapısınıda (parametreleri ve dönüş tipi) belirtirler. Olay tabanlı programlamada (Event based programming), Asenkron (Asynchronous) mimaride yer alan Polling, Callback, WaitHandle gibi modellerde, çok kanallı uygulamalarda (Multi Thread Applications) kullanılmaktadırlar

Bize gereken tiplerin neler olduğunu öğrendik. Peki bunları kendi Web Part kontrolümüzde nasıl ele alacağız. Bunun için WebPart sınıfından türetme yoluyla kendi Web Part kontrol sınıfımıza gelen Verbs isimli özelliğin ezilmesi (override) gerekmektedir.

```csharp
public virtual WebPartVerbCollection Verbs { get; }
```

Yukarıda prototipi görünen bu özellik, yanlız okunabilir (read only) bir özelliktir ve geriye WebPartVerbCollection tipinden bir referans döndürmektedir. WebPartVerbCollection sınıfı türlendirilmiş (strongly typed) bir koleksiyonu temsil etmekte ve Web Part bileşenine eklenecek ekstra fiileri taşımaktadır. Dolayısıyla bu özelliğin get bloğu içerisinde istediğimiz fiileri (Verbs) oluşturmamız ve gereken olay metodu yüklemelerini yapmamız gerekecektir.

Bu makalemizde özellikle üzerinde duracağımız konu kendi fiillerimizi nasıl yazacağımızdır. Konuyu daha iyi anlayabilmek için örnek bir senaryo üzerinden hareket edeceğiz ve göze hoş gelecek bir Web Part kontrolü geliştirmeye çalışacağız. Bu sefer bir önceki Web Part bileşenimizden farklı olarak, Render metodu yerine CreateChildControls metodunu ezip, bileşen içindeki kontrollerin daha kolay bir şekilde nasıl oluşturulabileceğini de göreceğiz. Dilerseniz amacımızdan bahsederek örneğimizi geliştirelim. Sitemizde çeşitli kategorilerde duvar kağıtları (Wallpapers) olduğunu düşünelim. Siteye giren kullanıcılar seçtikleri kategorideki duvar kağıtlarından kaç tane istiyorlarsa görebilecekler ve istediklerine tıkladıklarında büyük versiyonlarına bakıp bilgisayarlarına indirebilecekler.

Bu senaryda Web Part kullanacağımız için resim sayısı ve kategori gibi bilgileri kişselleşetirme (Personalization) şansınada sahip olacağız. Web Part kontrolümüz, seçilen kriterlere göre, kontrolün sayfaya her çizilişinde rastgele resimler seçecek ve bunları gösterecektir. Peki kendi fiillerimizi bu senaryo içerisine nasıl katabiliriz? Kullanıcıların isterlerse Web Part kontrolüne eklenen resimleri yatay veya dikey düzende görebileceklerini göz önüne alalım. Bunun için Yatay Diz ve Dikey Diz başlıklı örnek fiilleri Web Part kontrolümüze ekleyip, resimlerin sayfa üzerindeki diziliş yönlerini belirleyebiliriz. Üstelik bu fiillerin değerlerini kişiselleştirirsek, sayfayı son bıraktığımız haliyle elde edebiliriz. Örnek Web Part kontrolümüzü bitirdiğimizde aşağıdaki ekran görüntülerindekine benzer sonuçlar elde edeceğiz.

Örnek olarak Uçak kategorisinde her ziyaretimizde rastgele 3 resmin yanyana gösterilmesi;

![mk200_1.gif](/assets/images/2007/mk200_1.gif)

Web Part kontrolümüz için geliştireceğimiz fiiller (Verbs);

![mk200_2.gif](/assets/images/2007/mk200_2.gif)

Dikey Diz başlıklı Verb seçildiğindeki durum;

![mk200_3.gif](/assets/images/2007/mk200_3.gif)

Artık kontrolümüzü geliştrimeye başlayabiliriz. Web Part bileşenimizi yine bir Web Control Library kütüphanesinde ele alabiliriz. ResimPart adlı Web Part sınıfımızın WebPart sınıfından türemesi (Inherit) gerektiğini hatırlayalım. Kontrolümüz kendi içerisinde kişiselleştirilebilir (Personalizable) 3 özellik barındırmalıdır. Bunlardan birisi ziyaretçinin görmek istediği resim sayısını tutan GosterilecekResimSayisi özelliğidir.

Ziyaretçinin görmek istediği kategorinin bilgisini ise ResimKategori isimli özellik ile tutabiliriz. Son olarak ziyaretçinin seçtiği fiile (Verb) uygun olacak şekilde bir değişkenin de kişiselleştirilmesi önemlidir ki bir sonraki ziyarette son bıraktığımız haliyle bir dizilim elde edebilelim. Bu amaçlada ziyaretçinin son seçtiği fiili kişiselleştirilebilir bir özellik olacak şekilde CizimYonu ismiyle saklayacağız. Dilerseniz bahsetmiş olduğumuz özellikleri (Property) aşağıdaki gibi yazarak makalemize devam edelim.

```csharp
[ToolboxData("<{0}:ResimPart runat=server></{0}:ResimPart>")]
public class ResimPart:WebPart
{
    #region Kişiselleştirilebilir özellikler için alan tanımlamaları

    private ResimKategorisi _kategori;
    private int _gosterilecekResimSayisi;
    private Yon _cizimYonu;

    #endregion

    #region Kişiselleştirilebilir Özellikler

    [WebBrowsable(true)]
    [WebDescription("Bakmak istediğimiz resimlerin kategorisi")]
    [WebDisplayName("Resim Kategorisi")]
    [Personalizable(PersonalizationScope.User, false)]
    public ResimKategorisi Kategori
    {
        get { return _kategori; }
        set { _kategori = value; }
    }

    [WebBrowsable(true)]
    [WebDescription("Seçilen kategoride gösterilecek resim sayısı")]
    [WebDisplayName("Resim Sayısı")]
    [Personalizable(PersonalizationScope.User, false)]
    public int GosterilecekResimSayisi
    {
        get{return _gosterilecekResimSayisi <= 0 ? 1 : _gosterilecekResimSayisi;}
        set{_gosterilecekResimSayisi = value <= 0 ? 1 : value;}
    }

    [WebBrowsable(true)]
    [WebDisplayName("Resimlerin Yönü")]
    [WebDescription("Resimler dikey veya yatay gösterilebilmesini sağlar")]
    [Personalizable(PersonalizationScope.User, false)]
    public Yon CizimYonu
    {
        get{return _cizimYonu;}
        set{_cizimYonu = value;}
    } 

    #endregion
}
```

Bir önceki makalemizden de hatırlayacağınız gibi, özelliklerimizi kişiselleştirmek için gerekli nitelikler ile işaretliyoruz. Resimlerin yönünü ve var olan resim kategorilerini birer enum sabiti içerisinde saklamaktayız.

> Enum sabitleri bu senaryoda oldukça işe yarar. Ne varki kategori isimlerinin değiştiği yada yeni kategorilerin eklendiği durumlarda koda girip güncelleme yapmak ve uygulamayı tekrardan build etmek gerekecektir. Alternatif bir yol olarak bu tip bir verinin kod dışında bir ortamda, örneğin bir Xml dosyasında veya veritabanındaki bir parametre tablosunda saklanması göz önüne alınabilir.

Normal şartlarda resimlerin kategorisi için daha farklı bir yöntem izlemek sağlıklı olacaktır. Kendi sistemimizde, aşağıdaki ekran görüntüsünde yer alan klasör yapısını baz alacak bir enum sabiti ele alınmaktadır. Resimlerin bir küçük birde orjinal hallerini tutmak için klasörleme mantığını kullanıyoruz. Buna göre söz konusu resimleri kategori adları şeklinde olan klasörler içerisinde yer alan big ve small alt klasörlerinde ayrıştırmış durumdayız. Web Part kontrolümüz küçük boyutlu resimleri small isimli klasörler altından çekerken, üzerlerine tıklandığında orjinal büyüklüklerindeki versiyonları ise boş bir tarayıcı penceresinde açacak şekilde big isimli alt klasörlerden çekmektedir.

![mk200_4.gif](/assets/images/2007/mk200_4.gif)

Buna göre kullanıcının enum sabiti yardımıyla seçtiği kategorideki resimleri görebilmesi için, klasör adı ile enum sabiti adının aynı olması gerekir. Bu durumda yanlışlıkla klasörün isminin değiştirilmesi sonucu sistem beklediğimiz şekilde çalışmayacaktır. Özellikle istenen kategoriye bağlı klasör bulunamayacağından çalışma zamanı istisnaları (run time exception) alınması kaçınılmazdır. Bu durumun önüne geçmek için neler yapılabileceğini düşünmekte fayda olacağı kanısındayım. Bize yardımcı olacak enum sabitlerimiz aşağıdaki gibidir.

```csharp
public enum ResimKategorisi
{
    araba,
    ucak,
    manzara,
    komik
}

public enum Yon
{
    DikeyYon,
    YatayYon
}
```

Şimdide Web Part kontrolümüz için gerekli fiillerimizi (Verbs) geliştirelim. Hatırlayacağınız gibi makalemizin başında bu iş için Verbs özelliğini ezmemiz (override) gerektiğini söylemiştik. Bu özellik içerisinde tanımlayacağımız WebPartVerb tipinden nesne örneklerinin yapıcı metodları (constructors) içerisinde, ilgili fiil (Verb) seçildiği zaman çalıştırılacak olan metodu işaret edecek bir temsilci tanımı yapılmaktadır. Bunları hesaba katarak Web Part kontrolümüzün içeriğini ilk aşamada aşağıdaki gibi geliştirebiliriz.

```csharp
WebPartVerb vrbYatay, vrbDikey;

public override WebPartVerbCollection Verbs
{
    get
    {
        vrbDikey = new WebPartVerb("DikeyDizilim", new WebPartEventHandler(DikeyDiz));
        vrbYatay = new WebPartVerb("YatayDizilim", new WebPartEventHandler(YatayDiz));

        vrbDikey.Text = "Dikey Diz";
        vrbYatay.Text = "Yatay Diz";

        WebPartVerb[] verbs = new WebPartVerb[2];
        verbs[0] = vrbDikey;
        verbs[1] = vrbYatay;
        WebPartVerbCollection verbCollection = new WebPartVerbCollection(verbs);

        return verbCollection;
    }
}

public void YatayDiz(object sender, WebPartEventArgs e)
{
    CizimYonu = Yon.YatayYon;
}
public void DikeyDiz(object sender, WebPartEventArgs e)
{
    CizimYonu = Yon.DikeyYon;
}
```

Şimdi neler yaptığımıza kısaca bakalım. Kendi yazacağımız fiillerimizi WebPartVerb tipinden tanımladıktan sonra get bloğu içerisinde oluşturmaktayız. Bu işlemi yaparken ikinci parametre ile bir WebPartEventHandler temsilci örneği tanımladığımıza ve DikeyDiz ile YatayDiz isimli metodları işaret ettiğimize dikkat edelim. Buna göre, kullanıcılar bu fiillerden birisine tıkladığında YatayDiz ve DikeyDiz isimli metodlar çalışacaktır. Bu metodların içerisinde Web Part kontrolümüz için tanımladığımız CizimYonu özelliğinin değerini berlilemekteyiz. Oluşturulan WebPartVerb kontrollerini bir dizi içerisinde topladıktan sonra bir WebPartVerbCollection koleksiyonunun üretilmesinde kullanıyoruz. Son olarak get bloğundan bu koleksiyonu geri döndürmekteyiz. Peki fiilin seçilmesi sonucunda resimleri yatay veya dikey olarak nasıl yerleştireceğiz?

Sonuç itibariyle seçilen resimlerin ekrana alınması ve belirli bir yöne doğru çizilmesi demek, Web Part kontrolünün ekrana çizilmesi sırasında (Render) uygun HTML takılarının (Tag) oluşturulması demektir. Hatırlayacağınız gibi bir önceki makalemizde bu iş için Render metodunu kullanmıştık. Render metodu dışında var olan Web sunucu kontrollerinden yararlanaraktanda aynı işlemi gerçekleştirebilmekteyiz. Sonuç itibariyle Web Part kontrolleride birer taşıyıcı (Container) olduğundan bir Controls koleksiyonuna sahiptir. Dolayısıyla sunucu kontrollerini oluşturup bu koleksiyona ekleyerek HTML elementleri ile fazla uğraşmadan render işlemlerini gerçekleştirebiliriz. Web Part kontrollerinde bu işlem için tek yapmamız gereken CreateChildControls metodunu ezmek olacaktır. Kendi örneğimiz için bu metodu aşağıdaki gibi ezebiliriz.

```csharp
protected override void CreateChildControls()
{
    string sanalAdres = HttpContext.Current.Request.Url.ToString();
    sanalAdres = sanalAdres.Substring(0, sanalAdres.LastIndexOf('/'));
    string sanalResimAdresi = sanalAdres + ("/images/") + Kategori.ToString();
    string fizikiKlasor = HttpContext.Current.Request.PhysicalPath;
    fizikiKlasor = fizikiKlasor.Substring(0, fizikiKlasor.LastIndexOf('\\'));
    int siraNo = 0;

    try
    { 
        FileInfo[] resimDosyalari = DosyalariAl(fizikiKlasor);
        Random rnd = new Random();
        Table tablo = new Table();

        if (CizimYonu == Yon.DikeyYon)
        {
            for (int i = 0; i < GosterilecekResimSayisi; i++)
            {
                TableRow satir = new TableRow();
                siraNo = rnd.Next(0, resimDosyalari.Length);
                TableCell hucre = HucreOlustur(sanalResimAdresi, siraNo, resimDosyalari);
                satir.Cells.Add(hucre);
                tablo.Rows.Add(satir);
            } 
        }
        else if (CizimYonu == Yon.YatayYon)
        {
            TableRow satir = new TableRow();
            for (int i = 0; i < GosterilecekResimSayisi; i++)
            {
                siraNo = rnd.Next(0, resimDosyalari.Length);
                TableCell hucre=HucreOlustur(sanalResimAdresi, siraNo, resimDosyalari);
                satir.Cells.Add(hucre);
            }
            tablo.Rows.Add(satir);
        }
        Controls.Add(tablo);
    }
    catch
    {
    }
}

private static TableCell HucreOlustur(string sanalResimAdresi, int siraNo, FileInfo[] resimDosyalari)
{
    string miniResimDosyaAdresi = sanalResimAdresi + "/small/" + resimDosyalari[siraNo].Name;
    string buyukResimDosyaAdresi = sanalResimAdresi + "/big/" + resimDosyalari[siraNo].Name;
    TableCell hucre = new TableCell();

    string resim = "<a href='" + buyukResimDosyaAdresi + "' target='_blank'><img src='" + miniResimDosyaAdresi + "'/></a>";
    hucre.Text = resim;
    return hucre;
}

private FileInfo[] DosyalariAl(string fizikiKlasor)
{
    DirectoryInfo fizikiKlasorBilgisi = new DirectoryInfo(fizikiKlasor + "\\images\\" + Kategori.ToString() + "\\small\\");
    FileInfo[] resimDosyalari = fizikiKlasorBilgisi.GetFiles();
    return resimDosyalari;
}
```

Burada kendimize göre bir algoritma geliştirdik. Temel olarak CreateChildControls metodu seçilen fiile göre yatay veya dikey dizilime uygun olacak şekilde bir HTML Table üretmekte ve Controls koleksiyonuna eklemektedir. Burada söz konusu olan Table, TableRow ve TableCell tipleri yönetimli kod (managed code) tarafında geliştirilmiş sunucu bileşenleridir ve çalışma zamanında üretilen sayfa içerisinde HTML Table, HTML TR (Satır), HTML TD (Hücre) elementlerine dönüştürülmektedir.

Özel olarak, resimlerin küçük hallerini göstermek ve üzerlerine tıklandığında orjinal boyutlarında açmak için a href ve img HTML elementlerinden faydalanılmaktadır. Sizler bu kod parçasını daha efektif hale getirerek (örneğin optimize ederek) daha ölçeklenebilir bir şekle döndürebilirsiniz. Artık geliştirdiğimiz WebPart bileşenini örnek bir web uygulaması üzerinde deneyebiliriz. Bu amaçla geliştireceğimiz web uygulamasının kişiselleştirmeye destek verebilmesi amacıyla Membership ayarlarını içermesi doğru olacaktır. Sonuç olarak aşağıdaki Flash animasyonunda görülen çıktıyı elde ederiz. (Flash dosyasının boyutu 180 Kb olup yüklenmesi zaman alabilir)

Yukarıdaki Flash animasyonundanda gördüğünüz gibi, sisteme giren bir kullanıcı kendisine göre istediği kategorideki resimleri gösterebilmekte ve bunları yatay veya dikey olarak dizebilmektedir. Bizim dikkat etmemiz gereken nokta kendi fiillerimizin burada işlenebilir olmasıdır. Yukarıda geliştirdiğimiz ResimPart isimli Web Part kontrolümüzde fiillerimizi ayrı olay metodlarına yönlendirdik. Dilersek tüm fiillerimizi aynı metod içerisinde ele alabiliriz. Bunun için öncelikli olarak WebPartVerb bileşenlerimizi oluştururken kullanıdığımız WebPartEventHandler temsilcilerini (delegates) aynı metodu işaret edecek şekilde oluşturmalıyız.

```csharp
vrbDikey = new WebPartVerb("DikeyDizilim", new WebPartEventHandler(VerbUygula));
vrbYatay = new WebPartVerb("YatayDizilim", new WebPartEventHandler(VerbUygula));
```

Daha sonra VerbUygula isimli metodumuzu aşağıdaki gibi kodlamamız yeterli olacaktır.

```csharp
public void VerbUygula(object sender, WebPartEventArgs e)
{
    string tetiklenenVerbId=((WebPartVerb)sender).ID;
    if (tetiklenenVerbId == "DikeyDizilim")
        CizimYonu = Yon.DikeyYon;
    else if (tetiklenenVerbId == "YatayDizilim")
        CizimYonu = Yon.YatayYon;
}
```

Tüm fiiller aynı olay metodu içerisinde ele alınacaklarından, olayı meydana getiren WebPartVerb nesne örneğinin kim olduğunun bilinmesi gerekmektedir. Bunun için olay metodunun ilk parametresinden yararlanılmıştır. sender isimli değişken, olay metodu içerisinde WebPartVerb tipine dönüştürülmüş ve ID özelliğinin değerine bakılarak CizimYonu isimli enum sabitine uygun değeri atanmıştır. Uygulamayı bu haliyle test ettiğimizde ilk versiyondaki ile aynı sonuçları elde ederiz.

Çözmemiz gereken bir durum daha vardır. Bu da hangi Verb seçildiyse bunun başına bir check işaret konulmasının sağlanmasıdır. Bu amaçla WebPartVerb sınıfının Checked özelliğinin değerinin true veya false olarak değiştirilmesi yeterlidir. Lakin asıl problem bununla ilişki kodun nereye yazılması gerektiğidir. Web sayfaları ve içerisinde yer alan kontrollerin yaşam döngüsü olayı biraz karmaşık hale getirmektedir. Senaryomuzda, işaretleme operasyonu için gerekli kod parçasının CreateChildControls metodu içerisine konulması düşünülebilir. Yada bu işlemi fiilleri ele aldığımız olay metodu içerisine koyabiliriz. Ancak hiç birisi işe yaramayacaktır. Bunun sebebi Web Part bileşenimizin, sunucu tarafındaki yaşam döngüsüdür. Dilerseniz yaşam döngüsünü inceleyerek devam edelim. Sayfa ilk kez talep edildiğinde dolayısıyla Web Part kontrolüde ilk kez oluşturulduğundaki olay işleyiş sırası göz önüne alındığında, Web Part kontrolümüzdeki bazı üyelerin işleyiş sırası aşağıdaki gibi olacaktır.

![mk200_5.gif](/assets/images/2007/mk200_5.gif)

Kullanıcı bir fiil (Verb) seçtikten sonra sayfa sunucuda tekrar oluşturulup, web part kontrolümüzde yeniden oluşturulacaktır. Bu ikinci request sonrasında üylerin işleyiş sırası ise aşağıdaki gibidir.

![mk200_6.gif](/assets/images/2007/mk200_6.gif)

Görüldüğü gibi her iki yaşam döngüsü sırasında son olarak Verbs isimli özelliğe girilmektedir. Dolayısıyla işaretleme kodlarını buraya dahil edebiliriz. Bununla birlikte göze çarpan bir diğer durum, ikinci talep sonrasında Verb özelliğinin iki kez devreye giriyor olmasıdır. Bu sebepten, Verb özelliğinin get bloğundaki kodlar aşağıdaki gibi optimize edilmeli ve WebPartVerb bileşenleri ile ilgili koleksiyonun oluşturulma işlemlerinin sadece bir kez yapılması garanti altına alınmalıdır.

```csharp
WebPartVerb vrbYatay, vrbDikey;
WebPartVerbCollection verbCollection;

public override WebPartVerbCollection Verbs
{
    get
    {
        if (vrbYatay == null && vrbDikey == null)
        {
            vrbDikey = new WebPartVerb("DikeyDizilim", new WebPartEventHandler(VerbUygula));
            vrbYatay = new WebPartVerb("YatayDizilim", new WebPartEventHandler(VerbUygula));

            vrbDikey.Text = "Dikey Diz";
            vrbYatay.Text = "Yatay Diz";
    
            WebPartVerb[] verbs = new WebPartVerb[2];
            verbs[0] = vrbDikey;
            verbs[1] = vrbYatay;
            verbCollection = new WebPartVerbCollection(verbs);
        }
        if (CizimYonu == Yon.DikeyYon)
        {
            vrbDikey.Checked = true;
            vrbYatay.Checked = false;
        }
        else if (CizimYonu == Yon.YatayYon)
        {
            vrbDikey.Checked = false;
            vrbYatay.Checked = true;
        }

        return verbCollection;
    }
}
```

Artık çalışma zamanında seçtiğimiz fiillerin yanında tik işaret aşağıdaki resimdekine benzer bir şekilde çıkacaktır.

![mk200_7.gif](/assets/images/2007/mk200_7.gif)

Böylece geldik bir makalemizin daha sonuna. Bu makalemizde kendi Web Part kontrollerimize özel fiilleri (WebPartVerb tipinden nesne örneklerini) nasıl ekleyebileceğimizi örnek bir senaryo üzerinden incelemeye çalıştık. Makalemizin sonlarında kontrollerin yaşam döngüsünün ne kadar önemli olabileceğini gördük. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.

[Örnek Uygulama İçin Tıklayınız.](/assets/files/2007/VerbSample.rar) (Resimlerin boyutlarının büyük olması nedeni ile Big isimli kasörlerlerdeki resim dosyaları ve AppData altındaki ASPNETDb.mdf dosyaları silinmiştir. Test ederken bunları göz önüne almayı unutmayınız.)