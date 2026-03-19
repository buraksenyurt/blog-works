---
layout: post
title: "Bana Bir Struct Yaz. Yok Yok Bana Bir Class Yaz."
date: 2011-02-28 16:30:00 +0300
description: "Bu yazımızda, Struct(Yapı) ile Class(Sınıf) tipleri arasındaki temel farklılıkları irdelemeye çalışıyor olacağız. Ancak benzerlikleri de yakalamaya gayret edeceğiz. (Hemen şunu hatırlatalım; .Net Framework, 5 temel veri tipi tanımlar. Bunlar Class, Struct, Enum, Interface ve Delegate tipleridir) Özellikle Struct tipinin kullanımına ilişkin örnekler geliştireceğiz."
categories:
  - csharp
tags:
  - csharp
  - dotnet
  - http
  - performance
  - delegates
  - visual-studio
---
# Bana Bir Struct Yaz. Yok Yok Bana Bir Class Yaz.
[![blg220_Giris](/assets/images/2011/blg220_Giris_thumb.jpg)](/assets/images/2011/blg220_Giris.jpg) Merhaba Arkadaşlar,

Aralık…2003 yılı. Dışarısı oldukça soğuk ve ben evdeyim. Camdan dışarıya baktığımda dışarıda pek kimseyi göremiyorum. Soğuktan dolayı sakin olan sokağımız daha da bir yanlız. Bu arada askerden döndükten sonra iş aramakla geçirdiğim 8nci ayın içerisindeydim. Neyseki mesleki kariyerimde ilerlemek için yeni bir heyecanım var. [C#Nedir?](http://www.csharpnedir.com/) ile daha ilk günlerimi yaşıyorum.

O zamanlar en çok yaptığım iş, öğrendiklerimi Türkçe yazım dili ile olabildiğince doğru bir şekilde paylaşmaktı. Nitekim öğrenmenin en iyi yolunun öğrenilenleri anlatmakla mümkün olduğuna inanmaktayım. Halen daha bu düşüncemin arkasındayım.

Elbette o zamanların verdiği acemilik nedeniyle, şimdi okuduğumda kaliteli olmadığını düşündüğüm yazılar üretmekteydim. Geçtiğimiz günlerde düşündüm de bu tip konuları arada sırada baştan ele almak ve yeniden örnekleyerek anlatmakta yarar olabilirdi. Hiç olmassa şu anda C# dilinin temellerini öğrenmekte olan arkadaşlarımız için. Öyleyse gelin, hiç vakit kaybetmeden yola koyulalım.

Bu yazımızda, Struct (Yapı) ile Class (Sınıf) tipleri arasındaki temel farklılıkları irdelemeye çalışıyor olacağız. Ancak benzerlikleri de yakalamaya gayret edeceğiz. (Hemen şunu hatırlatalım;.Net Framework, 5 temel veri tipi tanımlar. Bunlar Class, Struct, Enum, Interface ve Delegate tipleridir) Özellikle Struct tipinin kullanımına ilişkin örnekler geliştireceğiz.

Aslında her iki tip arasındaki farklılıklar, uygulama geliştirirken hangi tipi tercih edeceğimiz açısından oldukça önemlidir. C# programlama dilini yeni öğrenen birisi için ilk akla gelen, bellek üzerindeki tutuluş biçimlerinin farklı olmasıdır. Hatta Struct’ ların Değer Türü (Value Type), Class’ ların ise referans türü (Reference Type) olduklarının, temel seviyede bilgi sahibi olan tüm programcılar farkındadır. Ancak fazlası da olabilir

![Wink](/assets/images/2011/smiley-wink.gif)

Bu temel farklılıklardan bazılarını örnek kodlar yardımıyla irdelemeye çalışmaya ne dersiniz? Başlamadan önce örnek bir Struct tipini göz önüne alalım.

[![blg220_StructDiagram](/assets/images/2011/blg220_StructDiagram_thumb.gif)](/assets/images/2011/blg220_StructDiagram.gif)

```csharp
using System;

namespace StructvsClass 
{ 
    struct Location 
    { 
        public int X { get; set; } 
        public int Y { get; set; } 
        public int Z { get; set; }

        public override string ToString() 
        { 
            return String.Format("({0},{1},{2})", X, Y, Z); 
        } 
    } 
}
```

Eminim ki Location isimli Struct tipimiz pek çok deneyimli programcıya.Net Framework içerisindeki Point yapısını hatırlatmaktadır

![Wink](/assets/images/2011/smiley-wink.gif)

Temel olarak bir nesnenin uzaydaki yerini belirtmek için kullanabileceğimiz bu tip ayrıca Object tipinden kalıtsal olarak gelen ToString metodunu da ezmektedir (Override). Şimdi dilerseniz Struct tipi ile ilişkili vakalarımızı analiz etmeye ve Class’ lar ile aradaki farklılıkları görmeye çalışalım.

## Vaka 1: Metod Parametresi Olarak Kullanılmaları Hali

Struct’ lar ile Class’ lar arasındaki farklılıkları anlamak adına akla gelen ilk örnek, bu tiplerin metod parametresi olarak kullanılmaları halidir. Aslında burada farkı oluşturan durum C# tarafında metod parametrelerinin her zaman için değer türü olarak (Pass By Value) bilgi taşımalarıdır. Değer türü olarak bilgi taşınması, metoda aktarılan parametrik değişkenlerin aslında metod içinde kopylanarak iş yapması anlamına gelmektedir. Tabi burada dikkat edilmesi gereken nokta ise şudur; Sınıflar birer referans türüdür ve aslında metodlara değer türü olarak geçirilen bu referanslara ait adres bilgileridir. Sanırım spagetti cümlelere geçiş yaptık. Bu nedenle aşağıdaki örnek kod parçasını göz önüne alarak ilerleyelim.

```csharp
using System;

namespace StructvsClass 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            #region Case 1 : Struct' lar Metod parametresi olduklarında 

            Location planeLocation = new Location { X = 12, Y = 33, Z = 41 }; 
            Console.WriteLine(planeLocation.ToString()); 
            IncreaseLocation(planeLocation); 
            Console.WriteLine(planeLocation.ToString());

            #endregion 
        }

        static void IncreaseLocation(Location loc) 
        { 
            loc.X += 10; 
            loc.Y += 10; 
            loc.Z += 10; 
        } 
    } 
}
```

IncreaseLocation isimli metod Location türünden bir parametre almaktada ve X,Y,Z özelliklerinin değerlerini onar birim arttırmaktadır. Programın giriş noktası olan Main metodu içerisinde ise planeLocation isimli bir Location nesne örneği üretilip bu metoda parametre olarak geçirilmektedir. Örneğin çalışma zamanı çıktısına baktığımızda aşağıdaki sonuç ile karşılaşırız.

[![blg220_Case1_Runtime1](/assets/images/2011/blg220_Case1_Runtime1_thumb.gif)](/assets/images/2011/blg220_Case1_Runtime1.gif)

İşte bir Struct’ ın metod parametresi olarak kullanılmasındaki tipik davranış şekli. Değer türü olarak tüm içeriği ile birlikte metod içinde kopyalanan bir nesne söz konusu olduğundan, IncreaseLocation çağrısından önce ve sonrasındaki planeLocation içeriği değişmemiştir. Bu içerik sadece IncreaseLocation içerisinde değişime uğramıştır. Kısacası, metoda aktarılan değişkenin orjinal veri yapısında bir bozulma söz konusu değildir. Durumu biraz daha dramatize etmek ve özellikle sınıf ile yapı arasındaki farkı ortaya çıkartmak adına, Location tipini class haline getirip örneği tekrardan çalıştırabiliriz. Bu durumda sonuçlar aşağıdaki gibi olacaktır.

[![blg220_Case1_Runtime2](/assets/images/2011/blg220_Case1_Runtime2_thumb.gif)](/assets/images/2011/blg220_Case1_Runtime2.gif)

Görüldüğü üzere planeLocation değişkeninin X,Y ve Z değerlerinin orjinal halleri, metod çağrısından sonra bozulmuştur. Bunun tipik nedeni referans türü olan sınıfların, metod parametrelerine değer türü olarak geçirilmeleri sırasında, adreslerinin taşınmasıdır. Dolayısıyla metod içerisine kullanılan loc isimli değişken ile, planeLocation isimli değişken bellek üzerindeki aynı adres alanlarını işaret etmektedir.

Elbette Struct tipinin metodlara referans türü olarka geçirilmesi de mümkün olabilir. Bu durumda ref veya out parametrelerinden yararlanılması yeterli olacaktır. Aşağıda bu duruma ilişkin örnek bir kullanım söz konusudur.

```csharp
using System;

namespace StructvsClass 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            #region Case 1 : Struct' lar Metod parametresi olduklarında 

            Location planeLocation = new Location { X = 12, Y = 33, Z = 41 }; 
            Console.WriteLine(planeLocation.ToString()); 
            DecreaseLocation(ref planeLocation); 
            Console.WriteLine(planeLocation.ToString());

            #endregion 
        } 
        static void DecreaseLocation(ref Location loc) 
        { 
            loc.X -= 10; 
            loc.Y -= 15; 
            loc.Z -= 12; 
        } 
    } 
}
```

Bu durumda planeLocation değişkeni, Decrease isimli metoda referans olarak geçirilecektir. Dolayısıyla metod içerisinde loc değişkeninin veri içeriğinde yapılan değişiklikler, planeLocation değişkeninin içeriğini de bozacaktır. Aşağıdaki çalışma zamanı görüntüsünde bu durum açık bir şekilde görülmektedir.

[![blg220_Case1_Runtime3](/assets/images/2011/blg220_Case1_Runtime3_thumb.gif)](/assets/images/2011/blg220_Case1_Runtime3.gif)

Vaka 2: Atama İşlemlerindeki Davranışlar

Sınavda yapılar ile sınıflar arasındaki temel farklılıkları sorsalar sanıyorum ki herkesin aklına gelecek ilk seçenek atama işlemlerindeki davranışsal farklılıktır. Durumu anlamak için hemen aşağıdaki kod parçasını göz önüne alalım.

```csharp
Location jhonLocation = new Location { X=3,Y=1 }; 
Location marryLocation = jhonLocation; 

Console.WriteLine("Jhon Burada {0}",jhonLocation.ToString()); 
Console.WriteLine("Marry Burada {0}", marryLocation.ToString()); 

jhonLocation.X++; 

Console.WriteLine("Jhon Burada {0}", jhonLocation.ToString()); 
Console.WriteLine("Marry Burada {0}", marryLocation.ToString());
```

Kodun ilk kritik noktası jhonLocation değişkeninin marryLocation değişkenine atandığı yerdir. Bu atama sonrasında marryLocation değişkenin veri içeriği ile jhonLocation’ ın veri içeriği eş olacaktır. Yani tipik bir değişken kopyalama işlemi söz konusudur. Diğer yandan jhonLocation’ ın X alanının değerinin arttırılması sonrasında marryLocation’ ın X değerinde bir değişim olmadığı görülecektir. Nitekim her ikisi de bellek üzerinde ayrı lokasyonlarda duran değişkenlerdir. Örneğin çalışma zamanı çıktısı aşağıdaki gibidir.

[![blg220_Case2_Runtime1](/assets/images/2011/blg220_Case2_Runtime1_thumb.gif)](/assets/images/2011/blg220_Case2_Runtime1.gif)

Peki ya Location bir sınıf olsaydı?

![Wink](/assets/images/2011/smiley-wink.gif)

Bu durumda çalışma zamanı çıktısı aşağıdaki gibi olacaktı.

[![blg220_Case2_Runtime2](/assets/images/2011/blg220_Case2_Runtime2_thumb.gif)](/assets/images/2011/blg220_Case2_Runtime2.gif)

Görüldüğü üzere X alanının değerinin arttımı, marryLocation değişkeninin X değeri için de geçerli olmuştur. Bu son derece doğaldır nitekim atama sonrası kopyalanan içerik değil referans adresleridir. Bu sebepten atama sonrası jhonLocation değişkeninin veri içeriğinde yapılan değişiklikler çok doğal olarak marryLocation içinde geçerli olacaktır. Bu durumda geliştiricinin karar vermesi gereken soru şudur: Marry ile Jhon birlikte hareket etmeli midir? Yoksa istedikleri noktada birbirlerinde ayrı olarak hareket edebilirler mi?

![Wink](/assets/images/2011/smiley-wink.gif)

## Vaka 3: Struct Tipinden Özellik (Property) Kullanılması Hali

Bu vakayı canlandırmak için ikinci bir Struct tipine daha ihtiyacımız olacak. Plane isimli yapımızı aşağıdaki gibi tasarladığımızı düşünelim.

[![blg220_PlaneStructDiagram](/assets/images/2011/blg220_PlaneStructDiagram_thumb.gif)](/assets/images/2011/blg220_PlaneStructDiagram.gif)

```csharp
struct Plane 
{ 
    public string Pilot; 
    public Location CurrentLocation { get; set; }

    public override string ToString() 
    { 
        return String.Format("{0} şu anda {1} lokasyonundadır", 
            String.IsNullOrEmpty(Pilot)?"Yok":Pilot, 
            CurrentLocation.ToString()); 
    } 
}
```

Plane isimli yapı içerisinde Location tipinden bir özellik (Property) bulunmaktadır. Şimdi Main metodu içerisinde aşağıdaki örnek kod parçasını geliştirdiğimizi düşünelim.

```csharp
Plane redBaron = new Plane 
{ 
    Pilot = "Red Baron", 
    CurrentLocation = new Location {  X=12, Y=28, Z=46} 
}; 
Console.WriteLine(redBaron.ToString()); 
redBaron.CurrentLocation.X += 14; 
Console.WriteLine(redBaron.ToString());
```

İlk olarak Plane tipinden bir nesne örneği üretilmektedir ve CurrentLocation isimli özellik initialize edilirken X,Y ve Z özelliklerine ilk değerleri verilmektedir. Kodun ilerleyen kısımlarında CurrentLocation özelliği üzerinden X alanına gidilerek değerinin 14 birim arttırıldığı görülmektedir. Herhangiri sorun olabilir mi? Evet olabilir…Nitekim kod derlendiğinde, aşağıdaki hata mesajının üretildiği görülecektir.

[![blg220_Case3_Error](/assets/images/2011/blg220_Case3_Error_thumb.gif)](/assets/images/2011/blg220_Case3_Error.gif)

Dikkat edileceği üzere CurrentLocation özelliğinin üzerinden X değeri değiştirilememektedir. Aslında değişim söz konusudur fakat özellik kullanılması nedeniyle yeniden bir initialize işlemi yapılmasını gerektirmektedir. Kod aşağıdaki hale getirildiğinde bir sorun kalmayacaktır.

```csharp
Plane redBaron = new Plane 
{ 
    Pilot = "Red Baron", 
    CurrentLocation = new Location {  X=12, Y=28, Z=46} 
}; 
Console.WriteLine(redBaron.ToString()); 
// redBaron.CurrentLocation.X += 14; 
redBaron.CurrentLocation = new Location 
{ 
    X = redBaron.CurrentLocation.X + 14 
    , Y = redBaron.CurrentLocation.Y 
    , Z = redBaron.CurrentLocation.Z 
}; 
Console.WriteLine(redBaron.ToString());
```

Bu durumda çalışma zamanı çıktısı aşağıdaki gibi olacaktır.

[![blg220_Case3_Runtime1](/assets/images/2011/blg220_Case3_Runtime1_thumb.gif)](/assets/images/2011/blg220_Case3_Runtime1.gif)

Elbette Location yapısı class olarak tanımlanmış olsaydı bu tip bir hata ile karşılaşılmayacaktı.

## Vaka 4: Yapıları Örneklemeden Kullanabilme Hali

Struct tipleri aslında değer türü olduklarından, veri içerikleri başlangıçta mutlaka ilk değerlerine atanmalıdır. Bu durum Stack bellek bölgesinde tutulmalarından kaynaklanmaktadır. Bu sebepten normal şartlarda Struct tipleri için varsayılan yapıcı metodun (Default Constructor) yazılmasına izin verilmemektedir. Aslında değer türü olduklarından bu tipleri başlatırken varsayılan yapıcı metod kullanılması da zorunlu değildir. Bu nedenle sistem otomatik olarak varsayılan yapıcı metodu atamaktadır. Diğer yandan yapılar tipik bir değer türü değişkeni olaraktan da kullanılabilirler. Fakat burada tuhaf bir durum söz konusudur. Location isimli Struct tipimizi bu anlamda göz önüne alalım ve aşağıdaki kodu yazdığımızı düşünelim.

```csharp
Location tankLocation; 
tankLocation.Z = 14; 
tankLocation.Y = 15; 
Console.WriteLine(tankLocation.ToString());
```

Aslında bu son derece mantıklı bir kod parçasıdır. Nitekim Struct bir değer türü olduğundan int, double, bool gibi tanımlanıp kullanılabilmelidir. Yani varsayılan yapıcı metod veya başka bir versiyon ile örneklenmeden kolayca kullanılabilmelidir. Ancak kodumuzu bu haliyle derlediğimizde aşağıdaki derleme zamanı hatasını aldığımızı görürüz.

[![blg220_Case4_Runtime1](/assets/images/2011/blg220_Case4_Runtime1_thumb.gif)](/assets/images/2011/blg220_Case4_Runtime1.gif)

Hımmm! İlginç

![Sealed](/assets/images/2011/smiley-sealed.gif)

Neden böyle bir hata aldık ki? Aslında sınıfların yapamadığı bir şeyi yapıyor olmalıydık. Acaba Struct tipleri kıl mı?

![Laughing](/assets/images/2011/smiley-laughing.gif)

Sorun bu tip bir kullanımın sadece alanlar (Fields) için geçerli olmasıdır. Üstelik public olan alanlar için geçerlidir. Hatta ne kadar public alan var ise her birinin ilk değerinin de atanması gerekmektedir. Dolayısıyla Location tipini aşağıdaki hale getirerek devam edelim. (Elveda özellikler

![Frown](/assets/images/2011/smiley-frown.gif)

)

```csharp
namespace StructvsClass 
{ 
    struct Location 
    { 
        public int X; 
        public int Y; 
        public int Z; 
...
```

Bu sefer de aşağıdaki hata mesajı ile karşılaşırız.

[![blg220_Case4_Runtime2](/assets/images/2011/blg220_Case4_Runtime2_thumb.gif)](/assets/images/2011/blg220_Case4_Runtime2.gif)

Aslında bir önceki hata mesajı ile aynı görünmesine rağmen arada bir fark vardır. İlk hata mesajının verildiği yer ile ikincisi farklıdır. Son hata mesajının sebebi tüm alanlara ilk değer atanmayışıdır. Dolayısıyla kodun en azından aşağıdaki gibi olmasında yarar vardır.

```csharp
Location tankLocation; 
tankLocation.Z = 14; 
tankLocation.Y = 15; 
tankLocation.X = 12; 
Console.WriteLine(tankLocation.ToString());
```

Dikkat edileceği üzere X,Y ve Z alanlarının tamamına ilk değer atanmıştır. Elbette yapıcı metod yardımıyla bir nesne örneklediğimizde bu tip bir atama zorunluluğu bulunmamaktadır.

## Vaka 5: Üyelerin Varsayılan Değerleri (Default Values)

Struct’ lar aslında Class tipleri kadar esnek bir yapı sunmazlar. Bunu içerdikleri alanlara (Fields) varsayılan başlanıç değerlerini atarken çokça görebiliriz. Dilerseniz Location yapısını aşağıdaki gibi tasarladığımızı planlayalım.

```csharp
using System;

namespace StructvsClass 
{ 
    struct Location 
    { 
        public int X { get; set; } 
        public int Y { get; set; } 
        public int Z { get; set; } 

        public string TimeZone = "Atina-Minsk-İstanbul"; 
        public bool InGame; 

        public Location() 
        { 
            InGame = true; 
        }

        public override string ToString() 
        { 
            return String.Format("({0},{1},{2})", X, Y, Z); 
        } 
    } 
}
```

Location yapısına TimeZone isimli string tipinden ve InGame isimli bool tipinden birer alan eklenmiştir. Geliştiricinin amacı, bu iki değişkenin başlangıçta varsayılan değerlere sahip olmasıdır. Ancak kod bu şekliyle derlendiğinde aşağıdaki derleme zamanı hatalarının alındığı görülür.

[![blg220_Case5_Runtime1](/assets/images/2011/blg220_Case5_Runtime1_thumb.gif)](/assets/images/2011/blg220_Case5_Runtime1.gif)

Dikkat edileceği üzere TimeZone değişkenine varsayılan değer atanamamaktadır. Ayrıca varsayılan yapıcı metodun var olamayacağı ifade edilmektedir. Buna göre InGame değişkenin ilk değer atanması için varsayılan yapıcı metodun kullanılması planları da suya düşmüştür. Halbu ki Location tipi bir sınıf olarak tasarlanmış olsaydı!

![Laughing](/assets/images/2011/smiley-laughing.gif)

Bu durumda bir derleme zamanı hatası alınmayacaktı

![Undecided](/assets/images/2011/smiley-undecided.gif)

## Vaka 6: Performans

Aslında Stack bellek bölgesini kullanmalarından dolayı Struct ile çalışmanın performansı arttırıcı bir etken olduğu düşünülebilir. Ne var ki yapıların birbirlerine atanması, metodlara parametre olarak geçirilmesi veya döndürülmesi her zaman için değer yoluyla (By Value) olmaktadır. Bir başka deyişle referans yolu (By Reference) ile olmamaktadır. Bu da zaman içerisinde bellek üzerinde pek çok kopya nesnenin oluşması anlamına gelmektedir. Dolayısıyla sizde pek çok geliştirici gibi büyük bir olaslıkla sınıfları kullanmayı tercih edebilirsiniz.

## Karar Verirken

Peki ya nasıl karar vereceğiz? Struct mı kullanalım, Class mı kullanalım? Hangisini tercih etmeliyiz? Sanıyorum ki cevaplanması en zor sorulardan birisi de bu. Hatta az önce bu yazıyı yazarken yanıma gelen çalışma arkadaşım şunu deyince “Valla Hocam bunca zamandır yazılım geliştiriyorum, hiç bir projemde Struct kullanıldığını görmedim”

![Sealed](/assets/images/2011/smiley-sealed.gif)

Aslında yaygın kanı halen daha devam etmekte. 16 byte’ tan daha küçük veri toplulukları için Struct kullanılması önerilmektedir. Ayrıca sadece veri anlamında bir tipten söz ediyorsak, Struct kullanımı Class tipine nazaran daha anlamlı olabilir.

Sınıfları nesne yönelimli özellikleri tamamıyla destekleyen bir tip olarak düşünmek çok daha doğru bir yaklaşımdır. Nitekim Struct tiplerinin türetilmesi mümkün değildir ve bu OOP (Object Oriented Programming) çervecesinde önemli bir ilke ihlalidir.

Aslında karar vermek için Struct ve Class arasındaki farklılıklar dışında, benzerlikleri de bilmenin yararı vardır. Bu sebepten aşağıdaki tablodan da yararlanabiliriz.

[![blg220_DiscussionSheet](/assets/images/2011/blg220_DiscussionSheet_thumb.gif)](/assets/images/2011/blg220_DiscussionSheet.gif)

Aradan geçen 7 yıldan sonra bir önceki yazıya bakıyorum da

![Laughing](/assets/images/2011/smiley-laughing.gif)

Aslında bazı kuralların hiç değişmediği ap açık ortada..Net Framework’ ün içerisinde pek çok noktada kullanılan Struct veri tipi, halen daha projelerde göz önüne alınabilir, alınmalıdır. Özellikle bu ihtiyacı farkediyor olmakta bir yazılımcı için ve hatta proje için son derece önemlidir. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[StructvsClass.rar (24,56 kb)](/assets/files/2011/StructvsClass.rar) [Örnek Visual Studio 2010 Ultimate Sürümü Üzerinde Geliştirilmiş ve Test Edilmiştir]