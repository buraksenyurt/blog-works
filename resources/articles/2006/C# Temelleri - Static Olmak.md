---
title: "C# Temelleri : Static Olmak"
pubDate: 2006-09-13 18:00:00
categories:
  - C#
tags:
  - C#
  - .NET
  - HTTP
  - Visual Studio
---

# C# Temelleri : Static Olmak
Değerli Okurlarım Merhabalar,

Static anahtar sözcüğü C# programlama dilinde üstü kapalı yada açık bir biçimde pek çok yerde kullanılır. C# programlama dilini yeni öğrenen birisi için static anahtar sözcüğünün kullanım alanlarını bilmek önemlidir. İşte bu amaçla yola çıktığımız bu makalemizde, static kavramının C# programlama dilindeki yerini incelemeye çalışacağız. Static anahtar sözcüğünün aşağıdaki listede olduğu gibi çeşitli durumlarda kullanabiliriz.

- Bir sınıf (class) içerisinde bulunan metodlar (methods) static olarak tanımlanabilir.
- Bir sınıf (class) içerisinde bulunan alanlar (fields) static olarak tanımlanabilir.
- Bir sınıfa ait static bir yapıcı metod (constructors) tanımlanabilir.
- Değişmezler (constants) bilinçsiz olarak (implicit) static tanımlanmışlardır.
- Readonly referanslar açıkça belirtilmedikçe static değildirler.
- C# 2.0 static sınıflara izin verir.

Şimdi tüm bu maddeleri genişleterek incelemeye çalışalım.

1. Bir sınıf (class) içerisinde bulunan metodlar (methods) static olarak tanımlanabilir.

Static olarak tanımlanan bir metodun kullanılabilmesi için tanımlanmış olduğu tipin nesne örneğini oluşturmaya gerek yoktur. Bu durum çoğunlukla bir tipin asıl iş yapan fonksiyonelliklerin kullanılabilmesi için, tüm nesneyi örneklemenin gereksiz olduğu durumlarda ele alınır. Örneğin aşağıdaki kod parçasını ele alalım. Bu kod parçasında basit olarak Matematik isimli sınıf içerisinde tanımlanmış Toplam isimli static bir metod yer almaktadır.

```csharp
using System;

namespace StaticKavrami
{
    class Matematik
    {
        public static double Toplam(double x, double y)
        {
            return x + y;
        }
    }
}
```

Burada tanımlı olan Toplam isimli static metodu kullanmak için tek yapılması gereken SınıfAdı.StaticMetodAdı notasyonunu kullanmak olacaktır.

![mk174_2.gif](images/mk174_2.gif)

```csharp
using System;

namespace StaticKavrami
{
    class Program
    {
        static void Main(string[] args)
        {
            double result = Matematik.Toplam(4, 5);
        }
    }
}
```

Görüldüğü gibi intelli-sense özelliği Matematik isimli tip yazıldıktan sonra kullanılabilecek static metodumuzu doğrudan göstermektedir..Net Framework içerisinde yukarıdaki örnekte bizim tarafımızdan tanımlanmış olan (user-defined) metod gibi yüzlerce static metod mevcuttur. Bu metodların kullanım amacı çoğunlukla tanımlanmış oldukları tipin nesne örneğine ihtiyaç duyulmayışından ileri gelmektedir. Örneğin Console uygulamalarını geliştirirken çok sık kullandığımız Console sınıfına ait WriteLine, ReadLine vb metodlar static olarak tanımlanmışlardır. Hatta bir console uygulaması geliştirdiğimizde, programın giriş noktası olan Main metodunun static olarak tanımlandığını farketmişizdir.

Static metodların kullanılması sırasında dikkat edilmesi yada bilinmesi gerekin bazı durumlar vardır. Öncelikli olarak, static olarak tanımlanmış sınıf metodlarına static olmayan sınıf üyeleride erişebilir. Örneğin aşağıdaki kod parçasını göz önüne alalım.

```csharp
class Matematik
{
    public static double Toplam(double x, double y)
    {
        return x + y;
    }

    public double Toplamlar(int ustSinir,double x,double y)
    {
        double sonuc=0;
        for (int i = 0; i < ustSinir; i++)
            sonuc+=Toplam(x,y);
        return sonuc;
    }
}
```

Bu kod parçasında static olmayan Toplamlar isimli Matematik sınıfına ait üye metod, static olan Toplam metodunu kullanabilmektedir. Birde bunun tam tersi durumu göz önüne almaya çalışalım. Yani static bir metod içerisinden static olmayan bir metoda erişmeye çalışalım. Aşağıdaki AltToplamlar isimli metod bu amaçla geliştirilmiştir.

```csharp
public static void AltToplamlar(int ustSinir, double x, double y)
{
    Toplamlar(10, 4, 5);
}
```

Bu kod parçasında AltToplamlar isimli static metodumuz içerisinde, static olmayan Toplamlar isimli metod çağırılmaktadır. Ancak bu metodu içeren uygulama kodu derleme zamanında An object reference is required for the nonstatic field, method, or property 'StaticKavrami.Matematik.Toplamlar (int, double, double) hata mesajını verecektir. Eğer Visual Studio.Net kullanarak kodu yazıyorsanız zaten AltToplamlar isimli metod içerisinden, Toplamlar isimli metoda intelli-sense yardımıyla bile erişemiyeceğimizi görebilirsiniz.

![mk174_3.gif](images/mk174_3.gif)

Elbette burada ilginç bir durum daha vardır ki buda this anahtar sözcüğü kullanıldığı takdirde Toplamlar isimli static olmayan metodun erişilebilir gözükmesidir.

![mk174_4.gif](images/mk174_4.gif)

Bu tip bir erişim mümkün olmasına rağmen uygulama kodu derleme zamanında 'this'is not valid in a static property, static method, or static field initializer hatasını verecektir. Dolayısıyla static bir üye metodu içerisinde sadece static üye metodlar çağırılabilir. Yada başka bir deyişle static bir metod içerisinden static olmayan bir metodun çağırılamayacağını söyleyebiliriz.

2. Bir sınıf içerisinde bulunan alanlar (fields) static olarak tanımlanabilir.

Static metodlar gibi, bir sınıf içerisinde kullanılabilecek static alanlarda tanımlayabiliriz. Bir alanın static olarak tanımlanması halinde bellek üzerindeki yerleşim şeklide bilinmesi gereken noktalardan bir tanesidir. Bunu daha iyi ifade edebilmek için aşağıdaki kod parçasını göz önüne alalım.

```csharp
using System;

namespace StaticKavrami
{
    class Matematik
    {
        public static double Pi = 3.14;
    }
    class Program
    {
        static void Main(string[] args)
        {
            Matematik mt1 = new Matematik();
            Matematik mt2 = new Matematik();
            Matematik mt3 = new Matematik();

            Console.WriteLine(Matematik.Pi.ToString());
            Console.WriteLine(Matematik.Pi.ToString());
            Console.WriteLine(Matematik.Pi.ToString());
        }
    }
}
```

Matematik isimli sınıfımızda static olarak yer alan Pi isimli double tipinden bir değişken yer almaktadır. Bu değişkene dikkat ederseniz static metodlardakine benzer bir şekilde SınıfAdı.DeğişkenAdı notasyonu ile erişebilmekteyiz. Ancak burada asıl önemli olan nokta Matematik sınıfına ait 3 nesne örneğinin oluşturulmasına ve kullanılmasına rağmen Pi değerinin hiç bir şekilde değişmediğidir.

![mk174_5.gif](images/mk174_5.gif)

İşte bu etkinin nedeni Pi değerinin static olarak tanımlanmış oluşudur. Dolayısıyla Matematik nesnelerinden kaç tane oluşturulursa oluşturulsun hepsi aynı Pi değerine işaret etmektedir. Aşağıdaki grafikte bu durum sembolize edilmeye çalışılmıştır.

![mk174_1.gif](images/mk174_1.gif)

Static alanlar ile ilgili enteresan bir durum vardır. Bir static alanın değeri çalışma zamanında değiştirilebilir ancak yapıcı metodlar işin içerisine girdiğinde kodun tepkisi çok daha farklı olur. Konuyu daha iyi anlamak için Matematik sınıfımıza static Pi değerini nesne örneği üzerinden değiştirmemize yarayacak aşağıdaki gibi bir metod ekleyelim.

```csharp
public void PiDegistir(double pi)
{
    Pi = pi;
}
```

Sonrasında ise program kodlarımızı aşağıdaki gibi değiştirelim.

```csharp
static void Main(string[] args)
{ 
    Matematik mt1 = new Matematik();
    Matematik mt2 = new Matematik();
    Console.WriteLine(Matematik.Pi.ToString());
    Console.WriteLine(Matematik.Pi.ToString());
    mt2.PiDegistir(3);
    Matematik mt3 = new Matematik();
    Matematik mt4 = new Matematik();
    Console.WriteLine(Matematik.Pi.ToString());
    Console.WriteLine(Matematik.Pi.ToString());
}
```

Tahmin edeceğiniz gibi PiDegistir isimli metod mt2 isimli Matematik sınıfına ait nesne örneği üzerinden çağırıldıktan sonra, oluşturulan mt3 ve mt4 isimli nesne örnekleri static Pi değişkeninin yeni değeri olan 3' e erişeceklerdir.

![mk174_6.gif](images/mk174_6.gif)

Lakin işin içerisine bir yapıcı metodu katarsak durum biraz daha farklı bir hal alacaktır. Bu değişikliğe göre yukarıdaki kod parçasını yeniden çalıştırdığımızda ilginç ama beklenen bir sonuçla karşılaşırız.

```csharp
public Matematik()
{
    Pi = 3.1415;
}
```

![mk174_7.gif](images/mk174_7.gif)

Bu durumda görüldüğü gibi tüm örnekler için Pi static değişkeninin değeri 3.1415 olarak set edilmektedir. Bunun sebebi son derece doğaldır, nitekim yapıcı metod (constructor) her Matematik nesne örneği oluşturulurken çalıştırıldığından static değişkenimizin değeride sürekli olarak set edilmektedir. İşte bu duruma çözüm olacak bir metod çeşidi daha vardır; static yapıcı metod. (static constructor method)

3. Bir sınıfa ait static yapıcılar (constructors) tanımlanabilir.

Static yapıcı metod 2nci maddedeki son örnekte meydana gelen durum için tam bir çözüm olmaktadır. Static yapıcı metodu çoğunlukla bir sınıfın static değişkenlerine ilk nesne örneği oluşturulduğunda bir kereliğine değer atmak için kullanabiliriz. Bu bilgiler ışığında Matematik sınıfımızı aşağıdaki gibi değiştirelim ve static yapıcı metodumuz içerisinde static Pi değişkeninin değerini belirleyelim.

```csharp
class Matematik
{
    public static double Pi=3.14;

    public void PiDegistir(double pi)
    {
        Pi = pi;
    }
    static Matematik()
    {
        Pi = 3.1415;
    }
}
```

Burada dikkate değer bir nokta, Pi isimli static değişken tanımlanırken 3.14 değerini almasına rağmen, Matematik sınıfına ait ilk nesne örneğinin oluşturulması ile birlikte Pi değerinin 3.1415 olarak ele alınmaya başlanmasıdır. Bunun sebebi static yapıcı metodun yaptığı değer atamasının geçerli oluşudur. Şimdi 2nci maddedeki Main metodundaki kodlarımızı Matematik sınıfının bu yeni haline göre çalıştırırsak, sistemin doğru çalıştığını ve yapıcı metodların olumsuz etkisinin gerçekleşmediğini görürüz. Buradan çıkaracağımız en iyi sonuç şudur ki bir sınıfın static üyelerinin initialize edileceği en uygun yer static yapıcı metodudur.

![mk174_8.gif](images/mk174_8.gif)

Static yapıcı metoda ilişkin dikkat edilmesi gereken bir takım kurallar da vardır. Bu kurallara göre;

- Static yapıcı metod erişim belirleyicisi (access modifiers) kullanamaz.
- Static yapıcı metod parametre alamazlar.
- Static yapıcı metod sınıfa ait tüm yapıcılardan önce çalışır.
- Static yapıcı metod kaç nesne örneği oluşturulursa oluşturulsun bir kere çalışır.
- Bir sınıf sadece bir static yapıcı metod içerebilir.
- Static yapıcı metod ya ilk nesne örneği oluşturulduğunda ya da ilk static sınıf üyesi çağırılmadan hemen önce yürütülür.

4. Değişmezler (constants) bilinçsiz olarak static tanımlanırlar.

Constant'lar uygulamanın çalışması boyunca değişmeyecek değerleri saklamak için kullandığımız bir değişken çeşididir. Basit olarak değer türünden (value types) sabit bir değişkeni aşağıdaki gibi tanımlayabiliriz.

```csharp
public const double E = 2.7;
```

Her ne kadar çaktırmasalarda const anahtar sözcüğü ile tanımlanan sabitler aynı zamanda static davranış gösterirler. Bunu hem Visual Studio.Net üzerinde kod yazarken hemde ildasm ile geliştirdiğimiz tipin assembly'ına ait metadata bilgisine bakarken görebiliriz.

Visual Studio.Net Ortamında;

![mk174_9.gif](images/mk174_9.gif)

Ildasm (Intermediate Language DisAssembly) framework aracı yardımıyla assembly içerisine baktığımızda hem Matematik tipi üzerinden hemde metaData kısmından (Ctrl+M ile), E isimli sabit değişkenin static olarak tanımlandığını görebiliriz. Bu da constant'ların aslında içsel olarak bilinçsiz bir şekilde (implicitly) static tanımlandığının kanıtıdır.

![mk174_10.gif](images/mk174_10.gif)

Elbetteki sabitler bilinçsiz (implicit) olarak static tanımlandıklarından, static anahtar sözcüğü kullanıldığında derleme zamanında hata mesajı alırız.

![mk174_11.gif](images/mk174_11.gif)

5. Readonly referanslar açıkça belirtilmedikçe static değildirler.

Constant'lar derleme zamanında (compile time) tanımlanan değişkenler için geçerlidir. Bu nedenlede sadece değer türlerine (value types) uygulanabilirler. Oysaki bazı durumlarda sabit olarak tanımladığımız değişkenlerin değerleri çalışma zamanında belirlenebilir. Bu nedenle referans tiplerini sabit olarak kullanılabilmek için readonly tanımlarız. Örneğin Matematik sınıfımızın readonly versiyonunu kullanan aşağıdaki Islemler isimli sınıfı göz önüne alalım.

```csharp
class Islemler
{
    public readonly Matematik mtIslemler;

    public Islemler()
    {
        mtIslemler = new Matematik();
    }
}
```

Readonly olarak tanımlanıp sabit hale getirilen referans değişkenlerinin constant'larda olduğu gibi çalışma zamanında static bir üye davranışı sergileyeceği düşünelebilir. Ancak aşağıdaki ekran görüntüsündende farkedeceğiniz gibi bu mümkün değildir. Dikkat ederseniz intelli-sense özelliği readonly olarak tanımlanmış mtIslemler isimli üyeye, SınıfAdı.DeğişkenAdı notasyonu ile erişilmesine izin vermemektedir.

![mk174_12.gif](images/mk174_12.gif)

Dolayısıyla readonly olarak tanımlanan referans türleri açıkça belirtilmedikçe static değildirler. Dolayısıyla mtIslemler isimli sabitimizi static olarak tanımladığımızda SınıfAdı.DeğişkenAdı notasyonu ile erişme imkanına sahip oluruz. Elbette yukarıdaki kodu göz önüne aldığımızda, böyle bir değişiklik bir static yapıcı metodunda kullanılmasını gerektirecektir.

![mk174_13.gif](images/mk174_13.gif)

6. C# 2.0 static sınıflara izin verir.

C# 1.1 versiyonunda olmayan özelliklerden birisi bir sınıfın static olarak tanımlanamayışıydı. Oysaki C# 2.0 ile static sınıflar tanımlayabilmekteyiz. Static sınıfların belkide en önemli özelliği sadece static üyeler içerebiliyor olmalarıdır. Static sınıflar ile ilgili daha detaylı bilgiyi [C# 2.0 ve Static Sınıflar](http://www.csharpnedir.com/makalegoster.asp?MId=521) isimli makalemde bulabilirsiniz. Böylece geldik bir makalemizin daha sonuna. Bu makalemizde static kavramının C# dilindeki yerini incelemeye çalıştık. Temel olarak static üyelerin metod ve alan bazında kullanılışını, static yapıcı metodu, sabit olarak tanımlanan değer veya referans türlerindeki yerini incelemeye çalıştık. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.
