---
layout: post
title: "C#’ ın Enteresan Yanları"
date: 2013-06-09 02:44:00 +0300
categories:
  - csharp
tags:
  - csharp
  - static-fields
  - field-ordering
  - extension-methods
  - enums
  - intern-pool
  - string-intern
  - indexers
---
Yazılım sektöründe yer alan bizler, mutlak suretle en az bir programlama dilini çok iyi seviyede öğrenmeye çalışır ve bunun için epey yoğun çaba sarf ederiz (Hatta değerli bir büyüğümüzün sözüne göre, hayatımızın her hangibir noktasında C veya C++ gibi bir dili öğrenmeye çalışmış ama hiç bir zaman iyi bir C/C++ geliştiricisi olamamışızdır) Ne varki bazen dilin kullanılmayan pek çok özelliğini, zamanında öğrenmiş olsak dahi unutabiliriz. Hatta bazı ilginç olan yanlarını bugüne kadar hiç görmemiş, denememiş ya da duymamış olabiliriz.

[![helpful_tips_image](/assets/images/2013/helpful_tips_image_thumb.jpg)](/assets/images/2013/helpful_tips_image.jpg)

İşte size C# dili ile ilişkili olarak pek çoğumuzun hatırından giden bir kaç enteresan vaka…

> Bu arada C# diline ait geniş bir dökümantasyonu C:\Program Files\Microsoft Visual Studio 11.0\VC#\Specifications\1033 klasörü altında bulabilirsiniz
>
> ![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_145.png)
>
> 527 sayfalık CSharp Language Specification isimli bu döküman, elinizin altındaydı her zaman. Başka bir kitaba ihtiyacınız yok. En azından başlangıç seviyesinde. Visual Studio 2012 kurulumu sonrası gelen bu dökümantasyon aslında C# 3.0 sürümünden beri de mevcut.

Bu yazımızda örnek 5 vaka çalışması üzerinde duruyor olacağız.

## Vaka 1 – private olarak tanımlanmış bir alana (field), tanımlandığı sınıf dışından erişilemez.

Hep bu şekilde öğrendik. Genel olarak cümle kalıbı böyleydi. C# tarafından baktığımızda temel olarak 5 erişim belirleyicisi (Access Modifier) olduğunu biliyoruz. private, public, internal, protected ve son olarak da protected internal. private olarak tanımlanış üyelerin de (members), tanımlı oldukları yer dışından erişilemez olduklarını biliyoruz. Tabi istisnai durumlarda yok değil. Söz gelimi aşağıdaki kod parçasını göz önüne alalım.

[![csmyth_1](/assets/images/2013/csmyth_1_thumb.png)](/assets/images/2013/csmyth_1.png)

```csharp
class Vehicle 
{ 
   private Guid _vehicleId;

    public Vehicle() 
    { 
        _vehicleId = Guid.NewGuid(); 
    }

    public bool IsEqual(Vehicle vehicle) 
    { 
        return _vehicleId == vehicle._vehicleId; 
    } 
}
```

Vehicle sınıfı içerisinde tanımlanmış olan _vehicleId alanı private erişim belirleyicisi ile işaretlenmiştir. Ancak dikkat edilmesi gereken bir nokta vardır. Sınıf içerisinde tanımlanan IsEqual metodu parametre olarak başka bir Vehicle nesne örneğini almakta ve içerisinde bu örneğe ait _vehicleId alanını kullanmaktadır.

![Sarcastic smile](/assets/images/2013/wlEmoticon-sarcasticsmile_12.png)

Kullanabilmektedir. Derleyici buna kızmamaktadır. Peki uygulama tarafına geçelim ve aşağıdaki test kodlarını değerlendirelim.

```csharp
static void Main(string[] args) 
{ 
    #region Case 1 Test (Private Fields)

    Vehicle v1 = new Vehicle(); 
    Vehicle v2 = new Vehicle(); 
    Console.WriteLine(v1.IsEqual(v2));

    #endregion 
}
```

Sonuç false dönecektir elbette.

[![csmyth_2](/assets/images/2013/csmyth_2_thumb.png)](/assets/images/2013/csmyth_2.png)

Dikkat edileceği üzere parametre olarak gelen vehicle değişkeni üzerinden, private olarak tanımlanmış _vehicleId alanına erişilebilmiştir. Tabi _vehicleId’ nin değeri, v2 isimli değişkene ait olarak üretilen Guid değeridir.

> Dolayısıyla private alan kullanımları ile ilişkili olarak şunu da ifade edebiliriz. private tanımlanmış bir üyeye tanımlandığı sınıfa ait başka nesne örnekleri (Instance) üzerinden erişilebilinir.

## Vaka 2 – Çok yerde faydasını gördüğümüz genişletme metodları (Extension Methods), Enum sabitlerine de uygulanabilir.

Genişletme metodları (Extension Methods) özellikle elimize kodları kapalı olarak gelen assembly dosyaları düşünüldüğünde, bunları ek fonksiyonellikler ile genişletmede kullanılan önemli kavramlardan birisidir. Çoğunlukla türetilemeyen veya az önce de bahsettiğimiz gibi kodları kapalı gelen sınıflar için kullanıldığına sıklıkla şahit oluru. ([Extension Method’ lar ile ilişkili bir internet sitesi dahi vardır](http://www.extensionmethod.net/csharp) ![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_145.png)) Ancak bu özelliğin Enum sabitleri için de kullanılabildiğini fark etmiş miydiniz? Örneğin,

[![csmyth_3](/assets/images/2013/csmyth_3_thumb.png)](/assets/images/2013/csmyth_3.png)

```csharp
enum VehicleType 
{ 
	Tank, 
	MLRS, 
	Artillary, 
	Hummvy, 
	HeavyAnrtillary, 
	Destroyer, 
	Carrier 
}

static class EnumExtensions 
{ 
	public static string GetDescription(this VehicleType vType) 
	{ 
		string result = String.Empty;

		switch (vType) 
		{ 
			case VehicleType.Tank: 
				result= "Paletli zırhlı tank. 135mm top"; 
				break; 
			case VehicleType.MLRS: 
				result = "Kundağı motorlu çoklu roket atar sistemi"; 
				break; 
			case VehicleType.Artillary: 
				result = "75mm - 205mm arası hafif topçu"; 
				break; 
			case VehicleType.Hummvy: 
				result = "Hummer jeep"; 
				break; 
			case VehicleType.HeavyAnrtillary: 
				result = "205mm üstü ağır kara topçusu"; 
				break; 
			case VehicleType.Destroyer: 
				result = "Güneş sınıfı yeni nesil zırhlı destroyer"; 
				break; 
			case VehicleType.Carrier: 
				result = "Nimitz sınıfı nükleer Uçak gemisi"; 
				break; 
		}

		return result; 
	} 
}
```

VehicleType enum sabiti için EnumExtensions sınıfı içerisinde GetDescription isimli bir genişletme metodu tanımlanmıştır. Bu metod, enum sabiti ile ilişkili bir açıklamayı geri döndürecek şekilde tasarlanmıştır. (İlk parametre pek tabi this anahtar kelimesi ile başlamalıdır)

Enum sabitinin kodlarının kapalı bir Assembly içerisinde olduğunu farz edeceğimiz bir senaryoda, bu tip bir yaklaşım önemli esneklikler sağlayacaktır. Örneğin uygulanmasında da, bildiğimiz sınıf bazlı extension metodların kullanımından farklı bir yaklaşım söz konusu değildir.

[![csmyth_4](/assets/images/2013/csmyth_4_thumb.png)](/assets/images/2013/csmyth_4.png)

## Vaka 3 – static alanların tanımlanma sıraları önemli midir?

Bunu yazdığımızda göre önemli olsa gerek

![Smile](/assets/images/2013/wlEmoticon-smile_65.png)

Haydi gelin aşağıdaki kod parçasını göz önüne alalım.

```csharp
using System; 
using System.Linq; 
using System.Collections.Generic;

namespace HowTo_CSharp_Myths 
{ 
    class Program 
    { 
        #region Case 3 : static alanların sırası önemli midir?

        static double r1 = Math.PI; 
        static double r2 = r1;

        #endregion

        static void Main(string[] args) 
        {             
            #region Case 3 Test (static Field order)

            Console.WriteLine(r2.ToString());

            #endregion 
        } 
    } 
}
```

Program sınıfı içerisinde tanımlanmış olan r1 ve r2 static alanlarının şu an pek dikkati çekmeyen sıraları aslında önemlidir. Yukarıdaki kod çalıştırıldığında, tahmin edileceği üzere ekrana pi değeri yazılacaktır. Nitekim r2 tanımlanırken, r1’ in kendisine atandığı görülmektedir. r1’ de zaten Pi değerini taşımaktadır. İşte çalışma zamanı çıktısı.

[![csmyth_5](/assets/images/2013/csmyth_5_thumb.png)](/assets/images/2013/csmyth_5.png)

Peki static alanların sırasını değiştirirsek?

![Sarcastic smile](/assets/images/2013/wlEmoticon-sarcasticsmile_12.png)

```csharp
using System; 
using System.Linq; 
using System.Collections.Generic;

namespace HowTo_CSharp_Myths 
{ 
    class Program 
    { 
        #region Case 3 : static alanların sırası önemli midir?

        // static double r1 = Math.PI; 
        // static double r2 = r1; 

        // Sırayı değiştirdik 
        static double r2 = r1; 
        static double r1 = Math.PI;

        #endregion

        static void Main(string[] args) 
        {             
            #region Case 3 Test (static Field order)

            Console.WriteLine(r2.ToString());

            #endregion 
        } 
    } 
}
```

Yine ekrana Pi değerinin yazılmasını bekleyebiliriz öyle değil mi? Ama,

[![csmyth_6](/assets/images/2013/csmyth_6_thumb.png)](/assets/images/2013/csmyth_6.png)

sıfır yazmıştır.

Görüldüğü gibi sıralama static alanların kullanıldığı durumda önemlidir. Nitekim r2 ilk tanımlandığında r1 değerini alırken, r1’ in o anki değeri varsayılan int için 0’ dır. Dolayısıyla, sonraki sırada yapılan r1 tanımlanması ve atamasında verilen Pi değeri sadece r1 için söz konusudur.

> Peki bu durum static olmayan alanlar için de geçerli midir acaba? Bunu denediğimizde sizce ne olur?
> [![csmyth_7](/assets/images/2013/csmyth_7_thumb.png)](/assets/images/2013/csmyth_7.png)
> Böyle bir atamaya zaten bu Console uygulaması açısından bakıldığında, derleme zamanı izin vermeyecektir. Sıralamayı değiştirip double r2=r1; ifadesini bir alt satıra geçirseniz dahi durum değişmez.

## Vaka 4 – Indeksleyicilerde params anahtar kelimesi de kullanılabilir

Indeksleyiciler (Indexers) bildiğiniz üzere sınıf örnekleri üzerinden köşeli parantez operatörünü kullanarak iç üyelere erişimde yardımcı olmaktadırlar. Özelliklere (Properties) benzer şekilde get ve set blokları vardır ve çoğunlukla içsel dizi bazlı sınıf üyelerini ele alırlar. Fakat istenirse indeksleyicilerde params anahtar sözcüğü de kullanılabilir. Aşağıdaki örneği göz önüne alalım.

[![csmyth_8](/assets/images/2013/csmyth_8_thumb.png)](/assets/images/2013/csmyth_8.png)

ve kod parçamız

```csharp
class Company 
{ 
    private List<Person> _workers = new List<Person>(8);

    public Person this[int id] 
   { 
        get { 
            return 
                _workers 
               .Where(p=>p.PersonId==id) 
                .FirstOrDefault(); 
        } 
        set { 
            _workers.Add(value); 
        } 
    }

   public IQueryable<Person> this[params int[] workerIds] 
    { 
        get 
       { 
           return workerIds 
               .Select(id => _workers.Where(p=>p.PersonId==id).FirstOrDefault()) 
                .AsQueryable(); 
        } 
    } 
}

class Person 
{ 
    public int PersonId { get; set; } 
    public string Nickname { get; set; }

    public override string ToString() 
    { 
        return string.Format("[{0}]-{1}", PersonId.ToString(), Nickname); 
    } 
}
```

Bu örnekte Person tipinden generic bir List alanının indeksleyiciler ile kullanımına yer verilmiştir. Sadece tek bir int parametre alan versiyon, standart bir indeksleyici kullanımını göstermektedir. Diğer yandan params anahtar kelimesinin kullanıldığı ikinci versiyon, asıl odaklanacağımız yerdir. Company sınıfının örnek kullanımını göz önüne alırsak konuyu daha iyi irdeleyebiliriz.

```csharp
static void Main(string[] args) 
    { 
        #region Case 4 (Indeksleyicilerde params)

        Company company = new Company();

        company[0] = new Person { PersonId = 1, Nickname = "Şimşir mek cin" }; 
        company[1] = new Person { PersonId = 3, Nickname = "Sir Axelroad" }; 
        company[2] = new Person { PersonId = 4, Nickname = "Raul Şarul" }; 
        company[3] = new Person { PersonId = 2, Nickname = "William" }; 
        company[4] = new Person { PersonId = 6, Nickname = "Meytır" };

        var subSet = company[1, 3, 4]; 
        foreach (var person in subSet) 
        { 
            Console.WriteLine(person.ToString()); 
        }

        var singlePerson = company[6]; // params kullanılmayan indeksleyici çağırılır 
        Console.WriteLine(singlePerson.ToString()); 

        #endregion 
    } 
}
```

Dikkat edileceği üzere 0, 1, 2, 3 ve 4 numaralı indislere farklı Person nesne örnekleri atanmıştır. subSet değişkeninin elde ediliş şekline dikkat ettiniz mi

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_145.png)

İşte burada params anahtar kelimesinin etkisi görülmektedir. Senaryomuza göre burada PersoneId değeleri gönderilmiş ve ona uygun olacak bir sonuç alınmıştır.

singlePerson değişkeninin elde edilmesi sırasında ise, params anahtar kelimesinin kullanılmadığı indeksleyici versiyonu çalışacaktır. İşte uygulamanın çalışma zamanı sonuçları.

[![csmyth_9](/assets/images/2013/csmyth_9_thumb.png)](/assets/images/2013/csmyth_9.png)

## Vaka 5 – Hiç String sınıfının Intern veya IsInterned metodlarını kullandınız mı/duydunuz mu? (.Net odaklı bir fark ama olsun)

string değişkenler bilindiği üzere System.String sınıfı ile temsil edilirler. Referans tipi olan string değişkenler esasında maliyetleri zaman zaman yüksek olabilecek örneklerdir. Bazı durumlarda (ve hatta çok nadiren de olsa) programlarımızın içerisinde aynı veriyi tutan string değişkenlerin n sayıda örneğine ihtiyaç duyabiliriz. Milyonlarca string değişkeniniz olduğunu ve her birinin aslında aynı veri içeriğini tuttuğunu düşünün. Bu çok doğal olarak CLR (Common Language Runtime) tarafında önemli bir yük anlamına gelmektedir.

Aslında string tipteki değişkenler için CLR bu performans handikapını ortadan kaldırmak adına akıllı bir yol izler ve söz konusu referansları bir havuz da (Intern pool) tutar. Çalışma zamanında yeni bir string değişken gündeme geldiğinde içeriği bu havuzdan kontrol edilir. Kısacası CLR aynı içeriğe sahip olan string değişkenler için unique bir referans tutar.

Peki ya elimizdeki bir string’ in eğer var ise unique olan referansını buradan nasıl alabiliriz? İşte bu noktada devreye String.Intern metodu girer.

Eğer söz konusu içerik Intern Pool içerisinde var ise, onun referansı ile bir string değişken elde edilir. Yoksa da bu içerik, Intern Pool’ a atılır ve unique bir referans olarak tutulmaya devam edilir. IsInterned metodu ise söz konusu içerik eğer Intern Pool içerisinde yer alıyorsa yine referansı döndürecek ama yoksa null değerini verecektir.

İşte örnek bir kod parçası ve çalışma zamanında elde edilen sonuçlar.

```csharp
string name1 = "burak"; // name1 Intern Pool içerisinde set edildi 
string name2 = String.Intern("burak");

if(name1==name2) 
    Console.WriteLine(true); 
else 
    Console.WriteLine(false);

string name3 = String.Intern("burak s."); 
if(name2==name3) 
    Console.WriteLine(true); 
else 
    Console.WriteLine(false);

string name4 = new string(new char[]{'s', 'e', 'l', 'i', 'm'}); 
Console.WriteLine("selim pool {0}",String.IsInterned(name4)==null?"da değil":"da"); 
String.Intern(name4); 
Console.WriteLine("Intern çağrısı sonrası. selim pool {0}", String.IsInterned(name4) == null ? "da değil" : "da");
```

[![csmyth_10](/assets/images/2013/csmyth_10_thumb.png)](/assets/images/2013/csmyth_10.png)

Bu yazımızda şöyle kıyıda köşede kalmış olabilecek bir kaç dil kabiliyetine yer vermeye çalıştık. Kimbilir gözümüzden kaçan, dikkat etmediğimiz veya kullanmadığımız için zamanla unuttuğumuz neler var. Biraz ilham vermiş olabilirim. Siz de araştırın bakalım. Bir başka yazımızda görüşmek dileğiyle hepinize mutlu günler dilerim

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_145.png)

[HowTo_CSharp_Myths.zip (42,82 kb)](/assets/files/2013/HowTo_CSharp_Myths.zip)

[İlk yazım tarihi 31 ekim 2012]