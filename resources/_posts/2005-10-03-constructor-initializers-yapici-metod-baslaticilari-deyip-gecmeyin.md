---
layout: post
title: "Constructor Initializers (Yapıcı Metod Başlatıcıları) Deyip Geçmeyin"
date: 2005-10-03 09:00:00 +0300
categories:
  - csharp
tags:
  - csharp
  - constructor
  - oop
  - class
---
Yapıcı metodlar nesne yönelimli programlamada çok büyük öneme sahiptir. Uygulamada oluşturduğumuz her bir nesnenin en az bir yapıcı metodu (ki bu varsayılan yapıcı metodtur) vardır. Kuşkusuz ki yapıcı metodlar (constructors), bir nesne örneğinin kapsüllediği verilere başlangıç değerlerinin atanabileceği en elverişli elemanlardır.

![dikkat.gif](/assets/images/2005/dikkat.gif)
Yapıcıları, nesneleri başlangıç konumlarına getirmek, bir başka deyişle nesne ilk oluşturulduğunda sahip olması gereken değerleri belirlemek amacıyla kullanırız.

Uygulamalarımızda çoğunlukla yapıcı metodların aşırı yüklenmiş versiyonlarına ihtiyaç duyarız. Bu gereksinim genellikle, bir nesnenin verilerinin parametrik olarak birden fazla şekilde başlatılabileceği durumlarda oluşmaktadır. Örneğin veri işlemlerini üstlenen bir sınıfın yapıcı metodunda bu işlemler için temel teşkil edecek bir bağlantı (connection) nesnesini oluşturmaya çalıştığımızı düşünelim. En az iki versiyon kullanabiliriz. Bağlantı cümleciğinin (Connection string) parametrik olarak yapıcı metoda geçirildiği bir versiyon ve varsayılan bağlantı cümleciğinin kullanılacağı başka bir versiyon. Elbetteki aşırı yüklenmiş yapıcı metod versiyonlarını daha da çoğaltabiliriz. Lakin burada dikkate değer bir durum vardır. O da aşırı yüklenmiş yapıcı metodların içerideki değerlere atamaları nasıl yapacağıdır. Genellikle burada iki tip versiyon kullanılır. Acem programcıların ilk zamanlarda en çok kullandığı teknik başlangıç değer atamalarının her bir yapıcı metod içerisinde ayrı ayrı yapıldığı durumu kapsar. Diğer teknik ise this anahtar sözcüğü kullanılarak uygulanır ve değer atamaları merkezi bir yapıcı metod içerisine yönlendirilir.

Bu versiyonları daha iyi kavrayabilmek amacıyla basit bir örnek üzerinde tartışacağız. Farzedelim ki Dortgenleri temsil edecek tipte bir sınıf tasarlıyoruz. Dörtgen tipinden nesneleri temsil edecek bu sınıfın en azından en ve boy gibi iki değeri kapsülleyeceği aşikardır. Peki dörtgen sınıfına ait nesne örnekleri kaç şekilde başlatılabilir? Başka bir deyişle bir Dortgen nesnesi oluşturulduğunda, en ve boy değişkenlerinin değerleri kaç şekilde belirlenebilir? Bu soruya cevap ararken Dortgen sınıfımızı aşağıdaki gibi tasarlayalım.

![mk138_1.gif](/assets/images/2005/mk138_1.gif)

Sınıfımızın kodlarına gelince;

```csharp
class Dortgen
{
    private int m_AKenari;
    private int m_BKenari;

    public Dortgen() // Varsayılan yapıcı (default constructor)
    {
        m_AKenari=1;
        m_BKenari=1;
    }
    public Dortgen(int akenari) // Kare olma durumu
    {
        m_AKenari=akenari;
        m_BKenari=akenari;
    }
    public Dortgen(int akenari,int bkenari) // dikdörtgen olma durumu
    {
        m_AKenari=akenari;
        m_BKenari=bkenari;
    }
    public double Alan()
    {
        return m_AKenari*m_BKenari;
    }
}
```

Gördüğünüz gibi son derece basit bir tasarımımız var. Her bir yapıcı metod içerisinde, Dortgen sınıfının kapsüllediği a ve b kenarlarına ait değerlere atamalar yapıyoruz. Bu daha önceden de vurguladığımız gibi uygulamalarımızda sıkça kullandığımız bir tekniktir. Aslında durumu biraz daha derinden incelemekte fayda var. Hemen ILDASM aracı yardımıyla, bu sınıfı kullandığımız her hangibir uygulamanın intermediate language (ara dil) kodlarına bakalım.

![mk138_2.gif](/assets/images/2005/mk138_2.gif)

Üç yapıcı metodumuzda kendi içlerinde ojbect tipinden bir nesne örneğini oluşturmaktadır. Aslında bu en tepedeki sınıf olan Object sınıfının yapıcı metoduna bir çağrıdır. Bu başka bir açıdan bakıldığında, Framework içerisinde yer alan nesne hiyerarşisinin ve kalıtımın (inheritance) bir sonucudur. Öyleki,.net bünyesinde yer alan her nesne mutlaka en tepede yer alan Object sınıfından türeyerek gelmektedir.

![dikkat.gif](/assets/images/2005/dikkat.gif)
Bir yapıcı metodun IL kodu içerisinde, türediği temel sınıfa ait varsayılan yapıcı metodu çağırıyor olması, kalıtım (inheritance) için önemli bir faktördür. Bir türeyen sınıf nesne örneği oluşturulduğunda, içerideki kapsüllenmiş tiplere ilgili değerler atanmadan daha önce, temel sınıfın yapıcısı (default constructor) çağırılır. Bu mekanizma, türeyen sınıfa ait nesne örneği oluşturulurken, base anahtar sözcüğü yardımıyla ortak değişkenlere ait değerlerin temel sınıfa (lara) kolayca aktarılabilmelerini sağlar.

Buradaki kodlama aslında yapıcı metod başlatıcıları (constructor initializers) kullanılarak daha sade ve merkezileştirilmiş bir hale getirilebilir. Merkezileştirilmeden kasıt, atamaların tek bir yerde toplanmasıdır. Bunun en büyük faydası kodun bakımı ve onarımı sırasında ortaya çıkar. Sonuç itibariyle yapıcı metodlarımızın ortak noktası, gelen parametreleri aynı şekilde atamaya çalışmalarıdır. Dolayısıyla Dortgen sınıfını this anahtar sözcüğünü de kullanarak aşağıdaki gibi de geliştirebiliriz.

```csharp
class Dortgen_ // :Dortgen_Taban
{
    private int m_AKenari;
    private int m_BKenari;

    public Dortgen_():this(1,1)
    { 
    }
    public Dortgen_(int akenari):this(akenari,akenari)
    {
    }
    public Dortgen_(int akenari,int bkenari)
    {
        m_AKenari=akenari;
        m_BKenari=bkenari;
    }
    public double Alan()
    {
        return m_AKenari*m_BKenari;
    }
}
```

Bu sefer Dortgen sınıfının her bir yapıcı metodu aslında en genel yapıcı metodu çağırıp, this anahtar sözcüğü yardımıyla ortamdan gelen parametreleri tek bir noktaya aktarmaktadır. Bu teknik kod okunurluğunu daha da kolaylaştırır. Ayrıca, değer atamalarının bakımını olumlu yönde etkiler. Çünkü her hangibir değişiklik için tüm yapıcı metodları gezmektense, sadece atamaların yapıldığı asıl yapıcı metodu değiştirmek çok daha akılcı bir yoldur. Ancak her iki uygulama tekniği arasındaki farklar bunlar ile sınırlı değildir. Asıl farkı görebilmek için yine IL kodlarına bakmamız gerekir.

![mk138_3.gif](/assets/images/2005/mk138_3.gif)

Gördüğünüz gibi bu sefer Object sınıfına ait varsayılan yapıcı metod sadece this anahtar sözcükleri ile parametreleri yönlendirdiğimiz merkezi yapıcı metod içerisinden çağırılmaktadır. Bu kodun içerisinde çalışma zamanında object nesneleri için tahsis edilecek bellek miktarını biraz da olsa azaltan bir faktördür. Tabi durumu bir de hız açısından incelemek gerekir. Her iki teknik uygulanabilirlik, merkezileştirme, idareli bellek kullanımı açısından oldukça farklıdır aslında. Ama aşağıdaki örnek kodu uyguladığımızda çok daha farklı bir sonucu elde edeceğimizi görürüz.

```csharp
class TestUygulama
{
    static void Main(string[] args)
    {
        DateTime dtSimdi;
        TimeSpan fark;

        #region birinci tip constructor kullanımı (initializers ile)

        dtSimdi=DateTime.Now;
        for(int i=1;i<500000;i++)
        {
            Dortgen_ d1=new Dortgen_();
               d1.Alan();
        }
        fark=DateTime.Now-dtSimdi;
        Console.WriteLine("Birinci tip constructor kullanımı {0} (Initializers ile)",fark.TotalMilliseconds.ToString());
    
        #endregion

        #region ikinci tip constructor kullanımı

        dtSimdi=DateTime.Now;
        for(int i=1;i<500000;i++)
        {
            Dortgen d2=new Dortgen();
            d2.Alan();
        }
        fark=DateTime.Now-dtSimdi;
        Console.WriteLine("İkinci tip constructor kullanımı {0} ",fark.TotalMilliseconds.ToString());
    
        #endregion
    }
}
```

Buradaki ilk döngümüz, yapıcı başlatıcıları (constructor initializers) kullanan Dortgen_ nesnesine ait 500000 nesne örneğini oluşturur ve alan hesabı yapar. İkinci döngümüz ise, her bir değer atamasının kendi yapıcı metodu içerisinde yapıldığı ilk tekniğimizi kullanır. Uygulamayı test ettiğimizde her iki döngünün tamamlanma süreleri arasında belirgin bir fark vardır.

![mk138_4.gif](/assets/images/2005/mk138_4.gif)

Sanılanın aksine this kullanılan teknik, diğerine göre daha yavaş çalışmaktadır. Bu elbetteki göz ardı edilebilecek bir süre farkıdır. Alternatif bir yöntem olarak değer atamlarının yapıldığı ortak bir metod, her bir constructor içinden ayrı ayrı çağırılabilir. Yani aşağıdaki Dortgen_2 sınıfının kodlarında olduğu gibi.

```csharp
public class Dortgen_2
{
    private int m_AKenari;
    private int m_BKenari;

    public Dortgen_2()
    {    
        Atama(1,1);
    }
    public Dortgen_2(int akenari)
    {
        Atama(akenari,akenari);
    }
    public Dortgen_2(int akenari,int bkenari)
    {
        Atama(akenari,bkenari);
    }
    public double Alan()
    {
        return m_AKenari*m_BKenari;
    }
    private void Atama(int akenari,int bkenari)
    {
        m_AKenari=akenari;
        m_BKenari=bkenari;
    }
}
```

Burada atama işlemleri sadece bu sınıf içerisinden erişilebilen (private) bir metod ile sağlanmaktadır. Bu merkezileştirmeyi ve bakımın kolaylaştırılabilmesini sağlar. Ancak this kullanımı terk edildiği için IL kodunda yine her bir yapıcı metod içerisinde, Object sınıfına ait varsayılan yapıcı metodun çağırılması durumu devam etmektedir.

![mk138_5.gif](/assets/images/2005/mk138_5.gif)

Elbette bu şartlar göz önüne alındığında seçim yapmak zorlaşmaktadır. Hangisi olursa olsun çalışacaktır. Ancak ben performans açısından kayba neden olsa da, merkezileştirme, bakım ve onarım kolaylığı sağladığı düşünüldüğünde yapıcı başlatıcılarını (constructor initializers) kullanmayı tercih ediyorum. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.