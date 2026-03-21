---
layout: post
title: "Sabit Değer Tipleri (Immutable Value Types) Analizi"
date: 2006-02-08 10:00:00 +0300
categories:
  - csharp
tags:
  - csharp
  - type-systems
  - Immutable-Types
---
Immutable (sabit) tipler basit olarak tanımlandıktan sonra varlıkları asla değişmeyen türler olarak nitelendirilebilirler. Sabit bir tipe ait bir nesne örneğini oluşturduğunuzda, bu tipin içeriği asla değişmez. Ancak bir tipin sabit olup olmayacağına karar vermek gerçekten zordur. Bu karar mekanizmasında, tipin sahip olduğu veri içeriğinin atomik (atomic) yapısı oldukça önemlidir. Atomiklik, bir tipin sahip olduğu verisel bütünlüğü oluşturan her bir elemanın aralarındaki ilişki olarak tanımlanabilir. Bu noktada bir tipin atomik olup olmaması, sabit bir tip haline getirilip getirilmemesinde önemli bir karar mekanizmasıdır.

Atomik yapıyı anlamak için, bir tipin içerisinde yer alan alanların aralarındaki ilişkiyi kavramamız çok önemlidir. Örneğin, Muhendis isimli bir tipimiz olduğunu düşünelim. Tipimizin ID, Ad, Soyad, Pozisyon gibi bilgileri bardındırdığını göz önüne alalım. Bir mühendisin şirket için geçerli olan ID bilgisi değişebilir. Ancak bu değişikliğin Ad,Soyad veya Pozisyon alanları üzerinde herhangibir etkisi yoktur. Benzer şekilde, bir mühendisin pozisyonuda değişebilir. Ancak bu değişikliğin Id,Ad,Soyad alanları üzerinde bir etkisi yoktur. Kısacası, Muhendis tipinin veri içeriğini oluşturan alanların birbirleri üzerinde her hangibir bağlayıcı etkisi yoktur. Bu nedenle Muhendis tipinin atomik olmayan bir veri içeriği sunduğunu söyleyebiliriz. Bu da ilgili tipin verileri arasında tutarlılık olmasını gerektirmeyecek bir olgudur.

![dikkat.gif](/assets/images/2006/dikkat.gif)
Bir tipin verisel içeriğini oluşturan alanarının birbirleri üzerinde herhangibir etkisi yok ise, atomik olmayan bir yapı söz konusudur.

Şimdi bir de Saat, Dakika ve Saniye bilgilerini barındırdan Zaman isimli başka bir nesne modelini ele almaya çalışalım. Zaman tipi içerisinde yer alan Saat, Dakika ve Saniye alanlarının herhangibirinde yapılacak olan değişiklik, diğerlerinide etkileyebilecek cinstendir. Örneğin saniyenin 60' dan büyük olması halinde, dakika alanı üzerindede güncelleştirme yapılması gerekir. Aynı durum dakika alanı üzerinde yapılan değişiklikler içinde geçerlidir. Dakikanın 60' tan büyük olması halinde bu kez saat alanının güncelleştirilmesi gerekir. Dolayısıyla, Zaman tipi içerisinde yer alan alanların bir birlerini doğrudan etkilediğini söyleyebiliriz. İşte bu etkileşim nedeni ile Zaman tipinin Atomik bir veri içeriği sunduğunu söyleyebiliriz. Öyleki, verilerin tutarlı olması gerekmektedir.

![dikkat.gif](/assets/images/2006/dikkat.gif)
Bir tipin verisel içeriğini oluşturan alanlarının birbirleri üzerinde etkisi var ise, atomik bir yapı söz konusudur.

![mk147_1.gif](/assets/images/2006/mk147_1.gif)

Peki bir tipin içerisinde yer alan veriler arasındaki atomik yapının sabit (Immutable) bir tip için önemi nedir? Herşeyden önce, atomik veri yapısına sahip bir tip için, çalışma zamanında nesne örneği üzerinde yapılacak her değişiklik önemlidir. Nitekim nesne örneği üzerinden hareket ettiğimizde, her hangibir alanın değerinin değiştirilmesi, atomik zincir içerisinde yer alan diğer alanlarıda etkileyebilir. Bu ise özellikle çok kanallı uygulamalarda, farklı kullanıcıların değişik zamanlarda aynı nesne örneğinin tutarsız farklı veri görüntülerine bakmasına neden olabilir. Özellikle bu gibi durumlarda, tipin içeriğinin referans yolu ilede etkilenmesine izin vermemeye çalışırız. Bu amaçla kullanılabilecek en uygun tür struct tipidir. Durumu daha iyi analiz edebilmek için, Zaman isimli aşağıdaki gibi bir yapımız (struct) olduğunu düşünelim.

![mk147_2.gif](/assets/images/2006/mk147_2.gif)

```csharp
struct Zaman
{
    private int _Saat;
    private int _Dakika;
    private int _Saniye;
    
    public int Saat
    {
        get { return _Saat; }
        set { _Saat = value; }
    }
    public int Dakika
    {
        get { return _Dakika; }
        set { _Dakika = value; }
    }
    public int Saniye
    {
        get { return _Saniye; }
        set { _Saniye = value; }
    }
}
```

Zaman isimli tipimize ait herhangibir nesne örneğini çalışma zamanında aşağıdaki gibi kullanmaya çalıştığımızı düşünelim.

```csharp
static void Main(string[] args)
{
    Zaman zmn = new Zaman();
    zmn.Saat = 12;
    zmn.Dakika = 30;
    zmn.Saniye = 56;

    // Var olan nesne örneği üzerinde değişiklik yapılıyor.
    zmn.Saniye = 64;
    zmn.Dakika = 66;
}
```

Burada ilk olarak Zaman tipinden bir nesne örneğini oluşturuyoruz. Daha sonra var olan nesne örneği üzerindeki verilerde tip içerisindeki özelliklerimizi (properties) kullanarak değişiklikler yapıyoruz. İşte bu noktada verinin tutarlılığını farklı süreçlerdeki kullanıcılar için bozmuş oluyoruz. Başka bir deyişle, farklı kullanıcların farklı zamanlarda bu tipin farklı görüntülerine bakabilecekleri bir durumla karşı karşıyayız. (Burada saniyenin veya dakikanın 60' dan büyük olması sonrası yapılması gerekenler ele alınmamıştır.)

Özellikle üzerinde durmamız gereken nokta Zaman tipine ait nesne örneğinin veri tutarlılığının nasıl korunabileceğidir. Çok kanallı ortamlarda eş zamanlı çalışan kullanıcılar açısından uygulamanın çalışmasının değişik zamanlarında Zaman nesnesinin tutarlılığını korumak için neler yapılabilir? Bunu sağlamanın bir kaç yolu olabilir. Ancak en garantisi var olan yapımızı, sabit bir değer türü (Immutable Value Type) haline getirmektir. Zaman yapımızı sabit değer tipi haline getirmek için ilk olarak dışarıdan özellikler vasıtasıyla yapılan değer atamalarını ortadan kaldırmamız ve kontrolü tek bir noktada, dolayısıyla bir yapıcı metod içerisinde toplamamız gerekmektedir. Buna göre yapımızı aşağıdaki hale getirebiliriz.

![mk147_3.gif](/assets/images/2006/mk147_3.gif)

```csharp
struct Zaman
{
    private readonly int _Saat;
    private readonly int _Dakika;
    private readonly int _Saniye;

    public int Saat
    {
        get { return _Saat; }
    } 
    public int Dakika
    {
        get { return _Dakika; }
    }
    public int Saniye
    {
        get { return _Saniye; }
    }
    public Zaman(int sa, int da, int sn)
    {
        _Saat = sa;
        _Dakika = da;
        _Saniye = sn;
        ZamanKontrol();
    }
    private void ZamanKontrol()
    {
        // Artık sürelerin kontrolü ve değerlendirilmesi
    }
}
```

Dikkat ederseniz, alanlarımızı sadece okunabilir (read only) olarak tanımladık. Diğer taraftan özelliklerimize ait set bloklarını kaldırarak dışarıdan değer atanmasını engelledik. Nesne alanlarına değer atayabileceğimiz en uygun yer olaraktanda aşırı yüklenmiş yapıcı metodumuzu kullandık. ZamanKontrol isimli metodumuz artık süreleri hesap ederek alanların atomik yapısı çerçevesinde gerekli veri tutarlılığını sağlayacak kodları içermektedir. Sonuç itibariyle artık elimizde kullanışlı bir sabit değer türü (Immutable Value Type) vardır. Artık Zaman yapısına ait bir nesne örneğini oluşturduğumuzda var olan nesne içeriğini değiştirebilmemizin tek yolu nesne örneğini yeniden oluşturmaktır.

```csharp
Zaman zmn = new Zaman(10, 12, 16);

// Var olan nesne örneğinin içeriğini ancak yeniden oluşturarak değiştirebiliriz.
// zmn.Saniye = 64;
// zmn.Dakika = 66;

zmn = new Zaman(4, 5, 6);
```

Burada ilk olarak Zaman yapısına ait bir nesne örneği oluşturulmuştur. Artık bu nesne var olan süreçler içerisinde tektir ve içeriği sabittir. Dahası, içeriğin veri tutarlılığı bu nesne örneğine bakan herkes için sağlanmıştır. Eğer, var olan içerik üzerinde değişiklik yapmak istersek tek yolumuz vardır ki buda nesneyi yeniden örneklendirmekten geçmektedir.

Sabit değer türlerinde dikkat edilmesi gereken bir diğer hususda içeride yer alan referans türlü alanların kontrolüdür. Nitekim referans türleri üzerinde yapılan değişikliklerin etkileri, var olan atomik yapı içerisinde, tutarsızlıklara yol açarak veri bütünlüğünü bozabilir. Örneğin, Zaman yapısını bir dizi olarak başka bir sabit değer tipi içerisinde kullandığımızı düşünelim. Diziler Array sınıfından gelen referans tipleridir. Bu tipe ait bir nesne örneğinin her hangibir elemanı üzerinde yapılacak değişiklikler, dizinin kullanıldığı herhangibir tip içerisindede gerçekleşebilir. İşte bu nedenle sabit değer türleri içerisinde özellikle diziler kullanıldığında dikkat edilmesi gerekir. Bunu daha iyi anlayabilmek için aşağıdaki örneği göz önüne alalım. Burada SporcuListesi isimli sabit değer türümüz, içeride Zaman yapısı tipinden (Zaman yapımızda sabit bir değer tipidir) bir diziyide kullanmaktadır.

![mk147_4.gif](/assets/images/2006/mk147_4.gif)

```csharp
struct SporcuListesi
{
    private readonly Zaman[] zamanlar;

    public SporcuListesi(Zaman[] zmn)
    {
        zamanlar = zmn;
    }
    public IEnumerator ZamanListesi
    {
        get { return zamanlar.GetEnumerator(); }
    }
}
```

Bu haliyle SporcuListesinin sahip olduğu veri içeriğinin tutarlılığının garantiye alınmış olduğu düşünülebilir. Oysaki aşağıdaki uygulama kodları veri tutarlılığını kaybettiğimizi gösteremektedir. Zaman yapısını her ne kadar sabit değer tipi olarak tanımlamış olsakta, SporcuListesi yapısının verisel bütünlüğü ve tutarlılığı içeride bir referans tipi kullanılmasından dolayı bozulmuştur.

```csharp
static void Listele(SporcuListesi liste)
{
    Console.WriteLine("---Varış Zamanları---");
    IEnumerator numb = liste.ZamanListesi;
    while (numb.MoveNext())
    {
        Zaman z = (Zaman)numb.Current;
        Console.WriteLine(z.Saat + " " + z.Dakika + " " + z.Saniye);
    }
}

static void Main(string[] args)
{ 
    Zaman zmn1 = new Zaman(3, 42, 45);
    Zaman zmn2 = new Zaman(3, 43, 31);
    Zaman zmn3 = new Zaman(3, 56, 1);

    Zaman[] zmn = new Zaman[] { zmn1, zmn2, zmn3 };

    SporcuListesi liste = new SporcuListesi(zmn);
    Listele(liste);
    zmn[0] = new Zaman(0, 0, 0);
    Listele(liste);
}
```

Uygulamayı çalıştırdığımızda aşağıdaki ekran görüntüsünü elde ederiz. Dikkat ederseniz SporcuListesi isimli yapıya ait nesne örneğimizi oluşturduktan sonra, bu yapı dışında bir yerde zmn isimli dizinin ilk elamanına ait verilerde değişiklik yapılmıştır. SporcuListesi isimli yapımıza ait nesne örneği oluşturulduğunda, zmn isimli dizinin referansı, bu yapımız içerisindeki zamanlar isimli dizinin referansıyla eşitlenmiştir. Dolayısılya SporcuListesi yapısına ait nesne örneği oluşturulduktan sonra, zmn dizisi ile SporcuListesi isimli sınıf içerisinde kullanılan zamanlar isimli dizi artık aynı referansa sahiptir. Bu nedenle SporcuListesi yapısı dışında zmn dizisi üzerinde yapılan değişiklikler, zamanlar isimli diziyide etkilyeceğinden, sabit değer türü olarak tanımlanmış SporcuListesi yapısının verisel tutarlılığı bozulmuştur.

![mk147_5.gif](/assets/images/2006/mk147_5.gif)

Peki sorunu nasıl çözebiliriz? Bu sorunu aşmak için, SporcuListesi isimli sabit değer türü içerisinde Zaman tipi tabanlı dizimizi kopyalama yolunu seçebiliriz. SporcuListesi yapısında, yapıcı metodumuzu aşağıdaki kod parçasında olduğu gibi değiştirdiğimizi düşünelim. Bu kez yapıcı metodumuza parametre olarak gelen diziyi doğrudan atamak yerine içeride önce zamanlar isimli dizimizi gelen dizinin eleman sayısını baz alarak oluşturuyor ve gelen dizinin elemanlarını bu dizi içerisine 0ncı indeksten itibaren kopyalıyoruz. Böylece SporcuListesi yapısını oluştururken, içeride kullanılan dizinin veri tutarlılığını sağlamış oluyor bir başka deyişle dışarıdan yapılan değişikliklerin etkisini ortadan kaldırıyoruz.

```csharp
public SporcuListesi(Zaman[] zmn)
{
    zamanlar = new Zaman[zmn.Length];
    zmn.CopyTo(zamanlar, 0);
}
```

Uygulamamızı tekrardan çalıştırırsak, dışarıda yapılan değişikliğin bu kez sabit değer tipimiz içerisinde etkili olmadığını görürüz.

![mk147_6.gif](/assets/images/2006/mk147_6.gif)

Görüldüğü gibi bu kez uygulama kodu içerisinde zmn isimli dizideki değişikliğimiz, SporcuListesi örneğindeki zamanlar isimli diziyi etkilememiştir. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.