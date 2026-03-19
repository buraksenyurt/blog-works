---
title: "Boxing ve Unboxing Performans Kritiği"
pubDate: 2005-09-26 09:00:00
categories:
  - C#
tags:
  - C#
  - HTTP
  - Performance
---

# Boxing ve Unboxing Performans Kritiği
Değerli Okurlarım Merhabalar,

Bundan yaklaşık olarak iki sene önce boxing ve unboxing kavramları ile ilgili bir [makale](http://www.buraksenyurt.com/post/Boxing-(Kutulamak)-ve-Unboxing-(Kutuyu-Kald%C4%B1rmak)-bsenyurt-com-dan) (30.12.2003) yazmıştım. Aradan uzun süre geçti. Ancak boxing ve unboxing kavramları ile ilgili olarak halen daha dikkat edilmesi gereken hususlar var. Bunlardan bizim için en önemlisi elbetteki performans üzerine etkileri. Uygulamalarımızda çok sık olarak farkında olmadan veya farkında olaraktan boxing ve unboxing işlemlerinin yer aldığı kod parçalarını kullanıyoruz.

Bildiğiniz gibi boxing, bir değer türünün, referans türünü atanması sırasında gerçekleşen işleme verilen isimdir. Unboxing ise bunun tam tersi olmakta ve referans türünün tekrar değer türüne dönüştürülmesini kapsamaktadır. Hangisi olursa olsun, değer türlerinin tutulduğu stack bellek bölgesi ile, referans türlerinin tutulduğu heap bellek bölgesi arasında yer değiştirme ve kopyalama işlemleri söz konusudur.

![dikkat.gif](images/dikkat.gif)
İster boxing ister unboxing işlemi söz konusu olsun, bellek üzerinde stack ve heap bölgeleri arasında yeniden adresleme ve kopyalama işlemi söz konusudur.

İşte bu adresleme ve kopylama işlemlerinin uygulama içerisinde sayısız defa tekrar ediyor olması performansı olumsuz yönde etkileyen en önemli nedenlerden birisidir. Bunu daha iyi anlamadan önce, boxing ve unboxing işlemlerini biraz daha alt seviyede incelemek gerekir. Çok basit olarak aşağıdaki console uygulamasının MSIL (Microsoft Intermediate Language) koduna bir göz atalım.

```csharp
using System;

namespace InvestigateOfBoxingUnBoxing
{
    class Class1
    {
        static void Main(string[] args)
        {
            int deger=25;
            object obj=deger; // Boxing
            int sonuc=(int)obj; // Unboxing
        }
    }
}
```

İlk olarak integer tipinden bir değer türünü, object tipinden bir referans türünü atıyoruz. Daha sonra ise artık kutulanmış (boxing işlemine tabi tutulmuş) referans türünün değerini bilinçli bir tip dönüşüm (explicitly cast) işlemi ile tekrar değer türünden bir değişkene atıyoruz. Bu kodun IL çıktısı aşağıdaki gibi olacaktır.

```csharp
.method private hidebysig static void Main(string[] args) cil managed
{
    .entrypoint
    // Code size 19 (0x13)
    .maxstack 1
    .locals init ([0] int32 deger,
    [1] object obj,
    [2] int32 sonuc)
    IL_0000: ldc.i4.s 25
    IL_0002: stloc.0
    IL_0003: ldloc.0
    IL_0004: box [mscorlib]System.Int32
    IL_0009: stloc.1
    IL_000a: ldloc.1
    IL_000b: unbox [mscorlib]System.Int32
    IL_0010: ldind.i4
    IL_0011: stloc.2
    IL_0012: ret
} // end of method Class1::Main
```

Gördüğünüz gibi, IL_0004 ve IL_000b segmentlerinde box ve unbox komutları çalıştırılarak referans ve değer türleri arası bellek konumlandırma ve kopyalama işlemleri yapılmıştır. Bunun zaten böyle olacağını kodlarımızdan da biliyoruz. Peki IL kodunun bu çıktısının bizim için önemi nedir? Herşeyden önce bunu aşağıdaki masumane kod parçasını ele alaraktan anlamaya çalışmakta fayda var.

```csharp
using System;

namespace InvestigateOfBoxingUnBoxing
{
    class BoxUnBox
    {
        public static void EkranaYaz(int yaricap,double pi)
        {
            double alan=yaricap*yaricap*pi;
            Console.WriteLine("Yaricapi {0} olan dairenin alanı = {1} dır.",yaricap,alan);
        }
    }
    class Class1
    {
        static void Main(string[] args)
        {
            BoxUnBox.EkranaYaz(10,3.14);
        }
    }
}
```

Bu örnekte odaklanmamız gereken yer BoxUnBox sınıfımız içerisinde int tipinden yariçap değerini ve double tipinden pi değerini alan EkranaYaz isimli metodtur. Bu metod içerisinde standart olarak alan hesabını yaptıktan sonra sonuçları ekrana yazdırmak için Console sınıfının WriteLine metodunu kullanıyoruz. Şimdi uygulamanın IL koduna tekrar bakalım. EkranaYaz metodunun içerisinde yer alan aşağıdaki satırlar bizim için oldukça önemlidir.

```csharp
IL_000c: ldarg.0
IL_000d: box [mscorlib]System.Int32
IL_0012: ldloc.0
IL_0013: box [mscorlib]System.Double
IL_0018: call void [mscorlib]System.Console::WriteLine(string, object, object)
```

Gördüğünüz gibi değer türlerimiz boxing işlemine tabi tutulmuş ve WriteLine metoduna object tipinden geçirilmiştir. Halbuki biz kodumuzda basit olarak sonuçları ekrana yazdırmaya çalışıyoruz. Şu noktada bellek üzerinde, stack ve heap arasında bir veri değiştokuşu olacağını düşünmeyebiliriz. Ancak IL kodlarının da söylediği gibi box ve unbox komutları çağırılmıştır. Oysaki aynı kodu aşağıdaki stilde yazsaydık eğer;

```csharp
public static void EkranaYaz(int yaricap,double pi)
{
    double alan=yaricap*yaricap*pi;
    Console.WriteLine("Yaricapi {0} olan dairenin alanı = {1} dır.",yaricap.ToString(),alan.ToString());
}
```

bu durumda değer türlerimiz için bir boxing işlemi uygulanmayacaktı. Aslında WriteLine metodunun beklediği object türünden bir atama söz konusudur. Biz bunu daha parametreyi geçirirken değer türünün ToString () metodu ile sağlamış oluyoruz. Dolayısıyla, IL kodlarına tekrar bakacak olursak box ve unbox komutarının çağırılmadığını, bir başka deyişle boxing ve unboxing işlemlerinin yapılmadığını görürüz.

```csharp
IL_000c: ldarga.s yaricap
IL_000e: call instance string [mscorlib]System.Int32::ToString()
IL_0013: ldloca.s alan
IL_0015: call instance string [mscorlib]System.Double::ToString()
IL_001a: call void [mscorlib]System.Console::WriteLine(string, object, object)
```

Gördüğünüz gibi her hangibir box komutu çağırılmamıştır. İyi herşey hoşta, aynı sonuçları elde ettiğimiz her iki kod örneğinden hangisini tercih etmeliyiz. Bu durumu analiz edebilmek için aşağıdaki örnek uygulamayı göz önüne almakta fayda var. Amacımız boxing uygulandığı ve uygulanmadığı durumlarda süresel farkları tespit ederek performans değerlendirmesi yapabilmek.

```csharp
static void Main(string[] args)
{
    #region Boxing içeren kod kısmı
    DateTime suAn=DateTime.Now;
    for(int i=1;i<50000;i++)
    {
        double alan=i*i*3.14;
        Console.WriteLine("Yaricapi {0} olan dairenin alanı = {1} dır.",i,alan); // Boxing var...
    }
    TimeSpan tsBox=DateTime.Now-suAn;
    #endregion

    #region boxing içermeyen kod kısmı
    suAn=DateTime.Now;
    for(int i=1;i<50000;i++)
    {
        double alan=i*i*3.14;
        Console.WriteLine("Yaricapi {0} olan dairenin alanı = {1} dır.",i.ToString(),alan.ToString()); // Boxing yok...
    }
    TimeSpan tsNoBox=DateTime.Now-suAn;
    #endregion

    Console.WriteLine("------------");
    Console.WriteLine("Boxing olduğunda..."+tsBox.TotalMilliseconds.ToString());
    Console.WriteLine("Boxing olmadığında..."+tsNoBox.TotalMilliseconds.ToString());
}
```

Uygulama kodumuz her ne kadar anlamsız görünse de sonuç gerçekten çok ilginçtir. Uygulamanın tespit ettiği süreler aslında ortalama değerlerdir. Bu genellikle kullandığınız makinenin donanımsal yeteneklerine göre değişiklik gösterebilir. Ancak tabiki önemli olan hangisinin daha hızlı olduğudur.

![mk137_1.gif](images/mk137_1.gif)

Her iki region altındaki kodlarda aynı işi yapar. 50000 kez i değeri üzerinden alan hesabı yaparak, sonuçları ekrana yazar. Ancak her iki teknik arasında özellikle de WriteLine metodları içerisinde az önce bahsettiğimiz ToString () kullanımı farkı vardır. İlk region içerisindeki kodlarımızda ToString metodunu kullanmadık. Bu sebeplede, değerler ekrana yazdırılmadan önce boxing işlemi söz konusu olacaktır. Ancak ikinci region bölgesindeki kodlarımızda yer alan WriteLine metodunda ise değer türlerimiz için ToString metodunu kullanıyoruz. Sonuçta süre farkı önemsenecek derecede yüksektir. İkinci teknik daha hızlı sonuç almamızı sağlamıştır. Her ne kadar yukarıdaki gibi bir örneği pek kullanmayacak olsanızda, geniş çaplı uygulamalar düşünüldüğünde gereksiz yere yapılan boxing ve unboxing işlemleri, uygulamanın genelinde önemli oranda performans ve hız kaybına neden olabilir.

Boxing ve Unboxing işlemlerinin sık olarak görüldüğü diğer bir durum ise koleksiyonların kullanıldığı uygulamalarda göze çarpmaktadır. Özellikle koleksiyonlara eleman aktarılırken veya koleksiyon içerisindeki bir eleman okunurken boxing ve unboxing işlemleri ile karşılaşılmaktadır. Burada eleman sayısının yükselmesi, gerçekleşen boxing ve unboxing işlemlerinin sayısını arttıracaktır. Dolayısıyla stack ve heap arasındaki kopyalama ve yer değiştirme işlemleride oldukça fazlalaşacaktır ki bu da uygulamanın yavaşlamasına neden olan bir faktördür. Söz gelimi aşağıdaki örnek uygulamayı göz önüne alalım. Burada Urun isimli struct (yapı) tipinden bir nesnemizi ilk önce bir ArrayList koleksiyonunda, ardından object tipinden bir dizide ve son olarakta kendi tipinden bir dizide kullanıyoruz.

```csharp
using System;
using System.Collections;

namespace InvestigateOfBoxingUnBoxing
{
    public struct Urun
    {
        private int m_Fiyat;
        public int Fiyat
        {
            get    
            {
                return m_Fiyat;
            }
            set
            {
                m_Fiyat=value;
            }
        }

        public Urun(int fiyat)
        {
            m_Fiyat=fiyat;
        }
    }
    class Class1
    {
        static void Main(string[] args)
        {
            #region ArrayList koleksiyonu kullanıldığında
            ArrayList alUrun=new ArrayList();
            DateTime dtSuan=DateTime.Now;
            for(int i=1;i<500000;i++)
            {
                alUrun.Add(new Urun(i*1000)); // boxing olacaktır
            }
            TimeSpan tsFark=DateTime.Now-dtSuan;
            Console.WriteLine("ArrayList Kullanımı........."+tsFark.TotalMilliseconds.ToString());
            #endregion

            #region object dizisi kullanıldığında
            object[] objUrunler=new object[500000];
            dtSuan=DateTime.Now;
            for(int i=1;i<500000;i++)
            {
                objUrunler[i]=new Urun(i*1000); // boxing olacaktır
            }
            tsFark=DateTime.Now-dtSuan;
            Console.WriteLine("Object Dizisi Kullanımı........."+tsFark.TotalMilliseconds.ToString());
            #endregion

            #region Struct tipinden bir dizi kullanıldığında
            Urun[] urunList=new Urun[500000];
            dtSuan=DateTime.Now;
            for(int i=1;i<500000;i++)
            {
                urunList[i]=new Urun(i*1000); // değer türüne aktarma var. Yani boxing yok...
            }
            tsFark=DateTime.Now-dtSuan;
            Console.WriteLine("Struct Dizisi Kullanımı........."+tsFark.TotalMilliseconds.ToString());
            #endregion
        
        }
    }
}
```

Eğer Main metodumuzun IL koduna bakacak olursak, ArrayList koleksiyonunu ve object dizisini kullandığımız döngüler için box komutunun çağırıldığını kolayca görebilirsiniz. Bunun sebebi struct tipimizin değer türü olmasıdır. ArrayList ve object dizilerimizin elemanları ise object tipinden bir başka deyişle referans türündendir. Dolayısıyla ArrayList'e ve object tipinden olan dizimize, Urun isimli struct'ımıza ait nesne örneklerini eklemeye çalıştığımızda, değer türünden referans türüne geçiş (boxing) işlemi söz konusu olacaktır. Tahmin edeceğiniz üzere bu performans ve hız kaybına neden olan bir durumdur.

![mk137_3.gif](images/mk137_3.gif)

Öyleki uygulamayı çalıştırdığımızda Urun isimli struct tipinden dizinin kullanıldığı döngünün, diğerlerine göre belirgin olarak daha hızlı çalıştığını görebiliriz.

![mk137_2.gif](images/mk137_2.gif)

Elbette bu tip koleksiyonları kullandığınız durumlarda sadece Urun tipinden nesneler taşınacak ise, yine Urun tipinden bir nesne dizisini kullanmak en mantıklı seçimdir. Ama çoğu zaman kod yazarken bu gibi durumları gözden kaçırırız. Burada belkide en büyük problem elimizdekiler ile tam olarak ne istediğimizi bilemememizdir. Urun tipinden bir diziye ihtiyacım var ise bir koleksiyona gerek var mıdır? Yoksa bir koleksiyonun sağladığı avantajları kullanamıyacağım bir diziyi tercih etmek için performansı ne kadar düşünmeliyim? vb...Bu sorulara doğru yanıtları vererek en uygun kullanımı seçebiliriz. Bu gibi kullanımlar uygulamanın pek çok yerinde var olabilir. İşte bu sebepten özellikle değer türlerini ve referans türlerini bir arada kullanırken boxing ve unboxing işlemlerini minimize edecek tekniklere gidilirse performans olarak büyük kazanımlar sağlanılabilir. İlk zamanlarda bunu aşmak için IL kodu ile biraz daha fazla haşırneşir olmamız gerekebilir. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.
