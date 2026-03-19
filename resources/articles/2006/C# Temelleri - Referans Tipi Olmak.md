---
title: "C# Temelleri : Referans Tipi Olmak"
pubDate: 2006-10-02 06:00:00
categories:
  - C#
tags:
  - C#
  - .NET
  - Delegates
---

# C# Temelleri : Referans Tipi Olmak
Değerli Okurlarım Merhabalar,

.Net üzerinde kullanılabilen veri türleri (data types) referans türleri (reference types) ve değer türleri (value types) olmak üzere iki kategoriye ayrılmaktadır. Temel olarak değer türleri (value types) fiziki belleğin stack adı verilen bölgesinde tutulur. Referans türleri ise, veriyi heap bellek bölgesinde tutarken, stack bölgesinden verilerin adresini gösteren işaretçiler kullanır..Net Framework içerisinde yer alan int, double, float, bool gibi ilkel tiplerin çoğu değer türleridir.

Geliştiriciler struct ve enum tiplerini kullanarak kendi değer türlerini tasarlayabilirler. Aslında ilkel değer türlerinin (int, double gibi).Net Framework içerisinde karşılık geldikleri struct ' lar vardır (örneğin int için System.Int32). Bunlar aynı zaman Common Type System'in üyeleridir. Referans tipleri cephesine bakacak olursak; string ve object türleri önceden tanımlı olan referans tipleridir. Geliştiriciler class, delegate gibi tipleri kullanarak kendi referans türlerini yazabilirler. Değer ve referans türleri ile ilgili bu kısa açıklamalardan sonra dilerseniz makalemizde işleyeceğimiz konu başlıklarına bir bakalım.

- Referans tipleri arası atamalar sonrası durumu.
- Struct'lar içerisinde referans tipleri kullanılması halinde, tipler arası atamalar sonrası durum.
- Var olan referans türlerini klonlamak.
- Referans türlerini metod parametrelerinde değer türü gibi kullanmak.

1. Referans tipleri arası atamalar sonrası durumu.

Referans tipleri arasında yapılan atamalar dikkat edilmesi gereken durumlardan birisidir. Özellikle atamalar sonrası tip üzerinde sahip olunan verilerde yapılacak değişiklikler diğer nesne örneklerine ait verileride doğrudan etkileyecektir. Bu tamamıyle referans tiplerinin bellek üzerinde tutuluş şekliyle alakalı bir durumdur. Bu durumu analiz etmek için NoktaBilgisi isimli bir sınıfı ele alalım.

![mk176_1.gif](images/mk176_1.gif)

```csharp
class NoktaBilgisi
{
    private string m_Aciklama;
    public NoktaBilgisi(string aciklama)
    {
        m_Aciklama = aciklama;
    }
    public string Aciklama
    {
        get { return m_Aciklama; }
        set { m_Aciklama = value; }
    }
}
```

NoktaBilgisi isimli sınıfımız Aciklama isimli bir özelliğe ve bu özelliğin kullandığı m_Aciklama isimli alanı set etmekte kullanılan bir yapıcı metoda sahiptir. Bu koddaki amacımı işe yarar sınıftan çok bir referans tipini ele almaya çalışmaktır. Console uygulamamıza ait kodlar aşağıda olduğu gibidir. İlk olarak NoktaBilgisi sınıfına ait nb1 isimli bir örnek oluşturulmaktadır. Sonrasında ise bu örnek nb2 isimiyle tanımlanmış bir NoktaBilgisi referansına eşitlenmektedir. Bu eşitleme işleminden sonra nb2 isimli örnek üzerinden Aciklama isimli özelliğin değeri değiştirilir. İşte her şey bu noktadan sonra karışır. Atama sonrasında artık nesne örnekleri heap üzerindeki aynı veri alanlarını işaret ettiklerinden değişiklikler istenmeyen bir sonuç doğurabilir.

```csharp
Console.WriteLine("\t Nokta 1 nesnesi oluşturulur...");
NoktaBilgisi nb1 = new NoktaBilgisi("Nokta Açıklaması");
Console.WriteLine("\t Nokta 2 nesnesi tanımlanır ve Nokta 1 nesnesi atanır...");
NoktaBilgisi nb2 = nb1;
Console.WriteLine("\t Nokta nesneleri için Açıklama bilgileri...");
Console.WriteLine("Nokta 1.Açıklama ->"+nb1.Aciklama);
Console.WriteLine("Nokta 2.Açıklama ->"+nb2.Aciklama);
Console.WriteLine("\t Nokta 2 nesnesi üzerinde Açıklama bilgisi değiştirilir...");
nb2.Aciklama = "Yeni Açıklama";
Console.WriteLine("\t Nokta nesneleri için Açıklama bilgileri son durum...");
Console.WriteLine("Nokta 1.Açıklama ->"+nb1.Aciklama);
Console.WriteLine("Nokta 2.Açıklama ->"+nb2.Aciklama);
```

Uygulamamızı çalıştırdığımızda elde edeceğimiz ekran görüntüsü aşağıdaki gibi olacaktır.

![mk176_2.gif](images/mk176_2.gif)

Dikkat ederseniz atama işlemi sonrasında nb2 nesne örneği üzerinden yapılan değişiklik nb1 nesne örneğinin içeriğinide doğrudan etkilemiştir. Bu referans tipleri için zaten beklenen davranıştır. Olay aşağıdaki şekilde grafiksel olarak ifade edilmeye çalışılmıştır.

![mk176_4.gif](images/mk176_4.gif)

Ne varki NoktaBilgisi isimli sınıfımızı struct (yapı) haline getirdiğimizde (bunu class anahtar sözcüğü yerine struct yazarak gerçekleştirebiliriz.) çok daha farklı bir sonuçla karşılaşırız ve aşağıdaki ekran görüntüsünü elde ederiz.

![mk176_3.gif](images/mk176_3.gif)

Yapı kullanıldığında atama sonrası bellekte iki farklı NoktaBilgisi nesne örneği oluşmaktadır. Bu nedenlede nb2 nesne örneğine ait Aciklama alanında yapılan değişiklik nb1 nesnesini etkilememiştir. Olay aşağıdaki şekilde grafiksel olarak ifade edilmeye çalışılmıştır.

![mk176_5.gif](images/mk176_5.gif)

Peki gelelim önemli soruya. Yapılar arası atama sonrasında değişiklikler birbirlerini etkilemiyorlarsa, bir struct içerisinde bir referans türü kullanıldığında durum ne olacaktır? Bu soruyu 2nci madde içerisinde incelemeye çalışalım.

2. Struct'lar içerisinde referans tipleri kullanılması halinde, tipler arası atamalar sonrası durum.

Birinci maddedeki örneğimizde, NoktaBilgisi sınıfını struct olarak kullandığımızda, atama işlemi sonrasında stack bellek bölgesinde farklı kopyalar oluşturulduğunu görmüştük. Bu da kopyalanan nesne örnekleri üzerindeki değişikliklerin birbirlerini etkilemeyeceği anlamına gelmekteydi. Ancak yapılar (structs) içerisinde referans tiplerini kullanırsak durum biraz daha farklı olacaktır. Bunun için aşağıdaki gibi bir yapımız (struct) olduğunu düşünelim.

![mk176_6.gif](images/mk176_6.gif)

```csharp
struct Nokta
{
    private int m_X;
    private int m_Y;
    private NoktaBilgisi m_bilgi;

    public int X
    {
        get { return m_X; }
        set { m_X = value; }
    }

    public int Y
    {
        get { return m_Y; }
        set { m_Y = value; }
    } 

    public NoktaBilgisi Bilgi
    {
        get { return m_bilgi; }
        set { m_bilgi = value; }
    }

    public Nokta(string bilgi, int x, int y)
    {
        m_bilgi = new NoktaBilgisi(bilgi);
        m_X = x;
        m_Y = y;
    }
    public override string ToString()
    {
        return m_X.ToString() + " " + m_Y.ToString() + " " + m_bilgi.Aciklama;
    }
}
```

Nokta isimli yapımız (struct) herhangibir noktaya ait x ve y koordinatlarını tutacak ve konumuza örnek teşkil etmesi açısından içerisinde NoktaBilgisi isimli sınıfımıza ait referans tipini kullanacak şekilde tasarlanmıştır. Şimdi teorimize geçmeden önce Nokta yapımızı Console uygulamamız içerisinde aşağıdaki gibi kullanalım.

```csharp
Console.WriteLine("\tNokta 1 oluşturulur...");
Nokta nokta_1 = new Nokta("Başlangıç Noktası", 10, 102);
Console.WriteLine("\tNokta 1 den Nokta 2' ye Atama yapılır...");
Nokta nokta_2 = nokta_1;
Console.WriteLine("\tNokta Nesne bilgileri...");
Console.WriteLine("Nokta 1 : \t" + nokta_1.ToString());
Console.WriteLine("Nokta 2 : \t" + nokta_2.ToString());
Console.WriteLine("\tNokta 2 için alanlar değiştirilir...");
nokta_2.Bilgi.Aciklama = "Yeni Açıklama";
nokta_2.X = 18;
nokta_2.Y = 204;
Console.WriteLine("\tSon Durum");
Console.WriteLine("Nokta 1 : \t" + nokta_1.ToString());
Console.WriteLine("Nokta 2 : \t" + nokta_2.ToString());
```

Her zaman olduğu gibi ilk olarak nokta_1 isimli bir yapı nesnesi örneklenmekte ve sonrasında nokta_2 isimli bir yapı tanımlamasına doğru bir atama gerçekleştirilmektedir. Atama işlemi sonrasında ise nokta_2 üzerindeki üyelerde bir takım değişiklikler yapmaktayız. Burada üzerinde durmamız gereken nokta, NoktaBilgisi referansına ait Aciklama özelliği üzerinde yapılan değişikliktir. Uygulamamızı çalıştırdığımızda aşağıdaki ekran görüntüsünü elde ederiz.

![mk176_7.gif](images/mk176_7.gif)

Normal şartlar altında nokta_1 isimli yapı örneğini nokta_2 isimli yapıya aktardığımızda bellek üzerinde iki farklı değişken alanı oluşturulacaktır. Dolayısıyla nokta_2 isimli nesne örneği üzerinden X ve Y alanları için yaptığımız değişiklikerin nokta_1 nesne örneğini etkilemeyeceği ortadadır. Ancak aynı davranış biçimi, içeride kullandığımız NoktaBilgisi tipi için geçerli olmamıştır. Bu durumu aşağıdaki grafik ile daha net canlandırabiliriz.

![mk176_8.gif](images/mk176_8.gif)

Olayın sebebi gayet açıktır. İki yapı örneği içerisindeki m_bilgi alanları heap üzerindeki aynı bölgeyi referans etmektedir. Bu atamanın bir sonucu olarak karşımıza çıkmaktadır. Çözüm olarak yapımıza IClonable arayüzünü uygulayabilir ve derin kopyalama (deep copy) tekniğini kullanabiliriz.

3. Var olan referans türlerini klonlamak

Bazı durumlarda referans türlerine ait bir nesne örneğini o anki içeriğiyle alıp yeni bir nesne örneği olarak kullanmak, bazı değerlerini değiştirmek ama bunları yaparken atama sırasında kullanılan diğer nesneyi (nesneleri) etkilememek isteyebiliriz. Bir başka deyişle bir referans türünü klonlamak isteyebiliriz. Bu tip işlemler için.Net Framework içerisinde yer alan ICloneable isimli arayüzün ilgili sınıfa uyarlanması gerekmektedir. Bu durumu daha iyi analiz edebilmek için aşağıdaki gibi bir Dortgen sınıfımız olduğunu düşünelim.

![mk176_9.gif](images/mk176_9.gif)

```csharp
public class Dortgen:ICloneable
{
    private int m_En;
    private int m_Boy;

    public int En
    {
        get { return m_En; }
        set { m_En = value; }
    }
    public int Boy
    {
        get { return m_Boy; }
        set { m_Boy = value; }
    }
    public Dortgen(int en, int boy)
    {
        m_En = en;
        m_Boy = boy;
    }
    public override string ToString()
    {
        return "En : " + m_En.ToString() + " Boy : " + m_Boy.ToString();
    }
    public object Clone()
    {
        return new Dortgen(this.m_En, this.m_Boy);
    }
}
```

ICloneable arayüzü (interface) Clone isimli, parametre almayan ve geriye object tipinden değer döndüren bir metod tanımlar. Dortgen isimli sınıfımız içerisinde bu metodu kullanırken, o anki m_En ve m_Boy değerlerini ele alarak yeni bir Dortgen nesne örneğini geriye döndürüyoruz. Dolayısıyla çalışma zamanında, Dortgen sınıfına ait bir nesne örneğini klonlama şansına sahip oluyoruz. Aşağıdaki kod parçasında bu işlemin nasıl gerçekleştirilebileceği gösterilmektedir.

```csharp
Console.WriteLine("\t Dortgen 1 nesnesi oluşturulur...");
Dortgen drt1 = new Dortgen(10, 12);
Console.WriteLine("\t Dortgen 1 nesnesi Dortgen 2 nesnesine atanır...");
Dortgen drt2 = (Dortgen)drt1.Clone();
Console.WriteLine("\t Atama sonrası bilgiler...");
Console.WriteLine("Dortgen 1 için " + drt1.ToString());
Console.WriteLine("Dortgen 2 için " + drt2.ToString());
Console.WriteLine("\t Dortgen 2 nesnesinin eni ve boyu değiştirilir...");
drt2.En = 4;
drt2.Boy = 5;
Console.WriteLine("\t Dortgen 2 değişikliği sonrası bilgiler...");
Console.WriteLine("Dortgen 1 için " + drt1.ToString());
Console.WriteLine("Dortgen 2 için " + drt2.ToString());
```

Uygulamamızı çalıştırdığımızda aşağıdaki sonucu alırız. Gördüğünüz gibi klonlama işleminden sonra, drt2 nesne örneği üzerinde yapılan değişiklikler hiç bir şekilde drt1 nesne örneğini etkilememiştir.

![mk176_10.gif](images/mk176_10.gif)

Klonlama işlemi sonrası bellekte oluşan durumu aşağıdaki grafikte olduğu gibi düşünebiliriz.

![mk176_11.gif](images/mk176_11.gif)

Dortgen sınıfımızın iç üyeleri değer türündendir. Bu sebepten dolayı Clone metodu içerisinde MemberwiseClone fonksiyonu kullanılaraktan da aynı etki sağlanabilir.

```csharp
public object Clone()
{
    return this.MemberwiseClone();
}
```

Ancak Dortgen sınıfının başka referans tipleri içerdiği ve kullandığı bazı durumlarda MemberwiseClone metodu tam bir klonlama işlemi gerçekleştiremeyebilir. Söz konusu durumu analiz edebilmek için Dortgen sınıfı içerisinde, DortgenBilgi isimli bir referans tipi kullanacağız.

```csharp
public class DortgenBilgi
{
    private string m_Bilgi;
    public string Bilgi
    {
        get { return m_Bilgi; }
        set { m_Bilgi = value; }
    }
    public DortgenBilgi(string bilgi)
    {
        m_Bilgi = bilgi;
    }
}
```

Dortgen sınıfı içerisindede aşağıdaki değişiklikleri yapalım.

```csharp
public class Dortgen:ICloneable
{
    private int m_En;
    private int m_Boy;
    public DortgenBilgi DortgenBilgisi=new DortgenBilgi("Dörtgen");

    public int En
    {
        get { return m_En; }
        set { m_En = value; }
    }
    public int Boy
    {
        get { return m_Boy; }
        set { m_Boy = value; }
    }
    public Dortgen(int en, int boy,string bilgi)
    {
        m_En = en;
        m_Boy = boy;
        DortgenBilgisi.Bilgi = bilgi;
    }
    public override string ToString()
    {
        return "En : " + m_En.ToString() + " Boy : " + m_Boy.ToString() + " " + DortgenBilgisi.Bilgi ;
    }
    public object Clone()
    {
        return this.MemberwiseClone();
    }
}
```

Dortgen sınıfımız içerisinde mızıkçılık yapacak olan ve klonlama işleminde sorun çıkartacak olan üye DortgenBilgi isimli alandır. Console uygulamamıza ait kodlarımızıda son olarak aşağıdaki gibi tamamlayalım. Bu sefer drt2 nesne örneği üzerinden DortgenBilgisi referansına gidiyor ve Bilgi isimli alanın değerini değiştiriyoruz. Yukarıdaki satırlarda Clone metodunu kullandığımız için beklentimiz, Bilgi alanındaki değişikliğin drt1 nesnesini etkilememesi olacaktır.

```csharp
Console.WriteLine("\t Dortgen 1 nesnesi oluşturulur...");
Dortgen drt1 = new Dortgen(10, 12,"Dikdörtgen");
Console.WriteLine("\t Dortgen 1 nesnesi Dortgen 2 nesnesine atanır...");
Dortgen drt2 = (Dortgen)drt1.Clone();
Console.WriteLine("\t Atama sonrası bilgiler...");
Console.WriteLine("Dortgen 1 için " + drt1.ToString());
Console.WriteLine("Dortgen 2 için " + drt2.ToString());
Console.WriteLine("\t Dortgen 2 nesnesinin eni ve boyu değiştirilir...");
drt2.En = 4;
drt2.Boy = 4;
drt2.DortgenBilgisi.Bilgi = "Kare"; 
Console.WriteLine("\t Dortgen 2 değişikliği sonrası bilgiler...");
Console.WriteLine("Dortgen 1 için " + drt1.ToString());
Console.WriteLine("Dortgen 2 için " + drt2.ToString());
```

Oysaki uygulamamızı çalıştırdığımızda aşağıdaki ekran görüntüsünü elde ederiz.

![mk176_12.gif](images/mk176_12.gif)

Gördüğünüz gibi drt2 nesnesi üzerinden DortgenBilgisi referansına ait Bilgi özelliğinin değeri değiştirildiğinde aynı etki drt1 içinde meydana gelmiştir. Dolayısıyla Clone metodu tam olarak işlevini yerine getirmemiştir. Bunun sebebi Clone metodu içerisinde kullanılan MemberwiseClone metodunun referans tipi için adres kopyalaması gerçekleştirmiş olmasıdır. Çözüm olarak Clone metodu içerisinde Dortgen sınıfına ait bir nesne örneği, o anki değerleri ile tekrardan örneklenip geriye döndürülebilir.

```csharp
return new Dortgen(this.m_En, this.m_Boy,this.DortgenBilgisi.Bilgi);
```

Uygulamayı bu haliyle çalıştırdığımızda aşağıdaki ekran görüntüsünü elde ederiz. Gördüğünüz gibi bu sefer tam anlamıyla bir derinlemesine kopylama işlemi gerçekleşmiştir. (Dortgen sınıfı için kullandığımız bu klonlama tekniğini 2nci maddede kullandığımız Nokta yapısı içinde kullanabiliriz.)

![mk176_13.gif](images/mk176_13.gif)

Aynı etkiyi aşağıdaki kod parçasında olduğu gibide sağlayabiliriz. Bu tekniğie göre, MemberwiseClone metodu ile güncel Dortgen referansının tüm değer türleri sağlıklı bir şekilde alınmaktadır. Sonrasında ise bir DortgenBilgi sınıfına ait nesne örneği oluşturulmakta ve içeriği güncel Dortgen nesne örneği üzerinden alınmaktadır ki burada söz konusu olan içerik Bilgi isimli alandır. Son olarakta Dortgen sınıfına ait yeni nesne referansı metoddan geriye döndürülmektedir.

```csharp
Dortgen drt = (Dortgen)this.MemberwiseClone();
DortgenBilgi blg = new DortgenBilgi("");
blg.Bilgi = this.DortgenBilgisi.Bilgi;
drt.DortgenBilgisi = blg;
return drt;
```

4. Referans türlerini metod parametrelerinde değer türü gibi kullanmak.

Bildiğiniz gibi değer türlerini (value types) metodlara referans türü olarak olarak geçirebilmekteyiz. Bunun için ref ve out anahtar sözcüklerinden yararlanmaktayız. Lakin bazı durumlarda referans türlerini metodlara değer türü gibi geçirmekde isteyebiliriz. Bu daha çok, metod içerisinde gelen referans üzerinde yapılacak değişikliklerin orjinal referansı değiştirmesini istemediğimiz durumlarda işe yarayacak bir yoldur. Konuyu daha net anlayabilmek için NoktaBilgisi isimli sınıfımıza ait referansı parametre olarak kullanan aşağıdaki metoda sahip olduğumuzu düşünelim.

```csharp
static void NoktaDegistir(NoktaBilgisi noktaBlg)
{
    Console.WriteLine("\t Metod içerisi...Açıklama değiştirilir...");
    noktaBlg.Aciklama = "Yeni Açıklama";
}
```

NoktaDegistir isimli metodumuz NoktaBilgisi tipinden aldığı parametre üzerinden Aciklama alanının değerini değiştirmektedir. Uygulamamıza ait Main metodu içerisinde ise aşağıdaki kodları yazalım.

```csharp
Console.WriteLine("\t NoktaBilgisi Oluşturulur...");
NoktaBilgisi bilgi = new NoktaBilgisi("Özel Mülk");
Console.WriteLine("Bilgi : " + bilgi.Aciklama);
Console.WriteLine("\t Metod çağırılır ve Bilgi nesnesi metoda aktarılır...");
NoktaDegistir(bilgi);
Console.WriteLine("Bilgi : " + bilgi.Aciklama);
```

Uygulamamızı bu haliyle çalıştırdığımızda aşağıdaki ekran görüntüsünü elde ederiz. Gördüğünüz gibi metod içerisinde yapılan değişiklik otomatik olarak orjinal konumdaki nesne örneğinide etkilemiştir.

![mk176_14.gif](images/mk176_14.gif)

Şimdi NoktaDegistir isimli metod içeriğini aşağıdaki gibi değiştirelim.

```csharp
static void NoktaDegistir(NoktaBilgisi noktaBlg)
{ 
    noktaBlg = new NoktaBilgisi("Yeni Açıklama");
    // noktaBlg üzerinden istenilen diğer işlemler gerçekleştirilir.
}
```

Bu kez gelen parametreyi içeride bizzat örneklemekteyiz.(new ile yeni bir örneğini oluşturmaktayız) Bu durumda uygulamamızı yeninden çalıştıracak olursak NoktaDegistir metodu içerisinde yapılan değişikliğin Main metodu içerisinde yer alan NoktaBilgisi nesne örneği üzerinde bir etki yapmadığını görebiliriz. Dolayısıyla orjinal konumdaki nesne örneğinin içeriğini koruyabiliriz.

![mk176_15.gif](images/mk176_15.gif)

Bu makalemizde referans tiplerini daha yakından incelemeye çalıştık. Referans tipleri arası atamaların sonucundan yola çıkarak, struct lar içerisinde referans türlerini kullanmamız halinde neler olabileceğine baktık. Ayrıca bir referans tipinin tam bir kopyasının nasıl çıkarılabileceğini ve bunu yaparkende ICloneable arayüzünün nasıl kullanılabileceğini incelemeye çalıştık. Son olarakta bir referans tipini herhangibir metoda bir değer türü olarak nasıl alabileceğimizi gördük. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama İçin Tıklayın.](assets/UsingReferenceTypes.rar)
