---
layout: post
title: "Struct, Class ve Default Constructors - İnanmak İstiyorum"
date: 2010-09-27 06:50:00 +0300
categories:
  - csharp
tags:
  - csharp
  - bash
  - dotnet
  - http
  - reflection
  - visual-studio
---
Sanıyorum 90 yıllarda en çok izlediğim dizilerden birisi The X Files (Gizli Dosyalar) idi. Her bölümde olağan dışı konuların ele alındığı ve birbirlerine tamamen zıt iki karekter olan ama bu sayede gizli dosyaların işlenmesinde birbirlerini sürekli olarak dengeleyen Ajan Dana Scully ve Fox Mulder'ın maceraları gerçeklikten uzak olsa da, benim gibi bilim kurgu severleri ekrana bağlamak için yeterliydi.

![blg192_Giris.jpg](/assets/images/2010/blg192_Giris.jpg)

Diziyi her izleyişimde en çok hoşuma giden sahneler ise genellikle Fox Mulder'ın bir soru sorup ortağı dahil herkesi bir kaç saniyeliğine de olsa düşünmeye itmesiydi (yeni nesil argoda ifade etmek istersek dumura uğratmasıydı). Geçtiğimiz günlerde yine X Files dizisinin bir kaç bölümünü heyecanlı bir şekilde tekrardan izlerken bazen yaptığımız işte de, Ajan Fox Mulder'ın ürettiği gibi şüphe uyandırıcı soruların oluştuğunu düşünmeye başladım. Tabi soruyu hemen sormak istemiyorum. Olaya biraz daha heyecan katmakta yarar var. Bakalım bu yazımızın Fox Mulder'ı olabilecek miyim?

Konumuz C# programlama dilinin temelleri arasında sayılmaktadır. Class ve Struct tiplerinde Default Constructor (Varsayılan Yapıcı Metod) kullanımı. Bildiğiniz üzere.Net Framework Base Class Library üzerinde tiplere en üst seviyeden baktığımızda değer (Value) ve referans (Reference) türleri olarak ikiye ayırmaktayız. Struct tipleri değer türü, Class'lar ise referans türleri olarak ele alınmaktadır. Bu iki veri türününün bellekte tutuluş şekilleri, aralarındaki atamalarda gösterdikleri davranışsal farklılıklar bir kenara, özellikle kendi geliştirdiğimiz taslaklarında dikkat etmemiz gereken noktaların başında da, yapıcı metod kullanımları gelmektedir. Dilerseniz aşağıdaki kod parçasını göz önüne alarak işe koyulalım.

```csharp
namespace ClassStructAndConstructors
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    class Person
    {
        public int PersonId;
        public string Name;
        public double Salary;

        public Person()
        {
            
        }
    }

    struct Product
    {
        public int ProductId;
        public string Title;
        public double ListPrice;

        public Product()
        {

        }
    }
}
```

Bu kod parçasında Person isimli bir Class ve Product isimli bir Struct tanımlanmıştır. Her iki tip içerisinde de varsayılan yapıcı metod (Default Constructor) kullanımı söz konusudur. Ancak kodu derlediğimizde bir hata mesajı ile karşılaştığımızı görürüz.

![blg192_Error1.gif](/assets/images/2010/blg192_Error1.gif)

Kural 1 geliyor: Bu Struct olmanın kurallarından birisidir. Buna göre bir Struct'ın, Class'ların aksine parametresi olmayan yapıcı metod içermesi söz konusu değildir. Çok doğal olarak aşağıdaki gibi bir kullanım tercih edilebilir.

```csharp
namespace ClassStructAndConstructors
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    class Person
    {
        public int PersonId;
        public string Name;
        public double Salary;

        public Person(int pId,string name,double salary)
        {
            
        }
    }

    struct Product
    {
        public int ProductId;
        public string Title;
        public double ListPrice;

        public Product(int pId,string title,double listPrice)
        {          
        }
    }
}
```

Bu kez hem Class hem de Struct için parametre alan yapıcı metodlar söz konusudur. Ancak yine bir Compile Time hatası alınılması kaçınılmazdır.

![blg192_Error2.gif](/assets/images/2010/blg192_Error2.gif)

Dikkat edileceği üzere Class tipi için parametreli kullanım söz konusu iken, Struct için aynı durum gerçeklenememektedir.

Kural 2 geliyor: Bir Struct için parametreli yapıcı metod kullanılması halinde, içerideki tüm alanlara ilk değerlerinin atanması gerekmektedir. Dolayısıyla Struct'ımıza ait parametreli yapıcı metod içeriği aşağıdaki gibi düzenlenirse sorun kalmayacaktır.

```csharp
public Product(int pId,string title,double listPrice)
{
   ProductId = pId;
   Title = title;
   ListPrice = listPrice;
}
```

Şimdi kod parçasını aşağıdaki gibi değiştirdiğimizi düşünelim.

```csharp
namespace ClassStructAndConstructors
{
    class Program
    {
        static void Main(string[] args)
        {
            Person burak = new Person();
            burak.PersonId = 1001;
            burak.Name = "Burak Selim Şenyurt";
            burak.Salary = 1;

            Product mouse = new Product();
            mouse.ProductId = 2001;
            mouse.Title = "Microsoft Optical Mouse";
            mouse.ListPrice = 9.99;
        }
    }

    class Person
    {
        public int PersonId;
        public string Name;
        public double Salary;
    }

    struct Product
    {
        public int ProductId;
        public string Title;
        public double ListPrice;
    }
}
```

Bu sefer ne Class ne de Struct için bir yapıcı metod bildiriminde bulunulmamıştır. Kod başarılı bir şekilde derlenmiştir. Main metodu içerisinde ise her iki tipe ait nesne örnekleri oluşturulmuş ve alanlarına bazı değerler atanmıştır. Hımmm...İşte Fox Mulder sorusu geliyor?

Struct'lar için açık bir şekilde varsayılan yapıcı metod tanımlayamıyorsak eğer, nasıl oluyorda new Struct () şeklinde bir metod çağrısı yapabiliyoruz?

Çok doğal olarak burada Dana Scully olsaydı söz konusu soruyu cevaplamak için hasta üzerinde otopsi yapmaya, bir başka deyişle IL (Intermediate Language) koduna bakmaya karar verirdi

![Wink](/assets/images/2010/smiley-wink.gif)

Biz de böyle yapıyor olacağız. (Bu amaçla ILDASM veya Red Gate's.Net Reflector gibi araçlardan yararlanabiliriz)

Person sınıfı için üretilen IL çıktısı aşağıdaki gibidir.

```bash
.class private auto ansi beforefieldinit Person
    extends [mscorlib]System.Object
{
    .method public hidebysig specialname rtspecialname instance void .ctor() cil managed
    {
        .maxstack 8
        L_0000: ldarg.0 
        L_0001: call instance void [mscorlib]System.Object::.ctor()
        L_0006: ret 
    }

    .field public string Name
    .field public int32 PersonId
    .field public float64 Salary

}
```

Dikkat edileceği üzere varsayılan bir yapıcı metod otomatik olarak üretilmiştir. Ajan Dana Scully bu durum karşısında Product isimli Struct için de aynı durumun söz konusu olacağını düşünmektedir. Yüzünde bir tebessümle Mulder'a döner ve "Tamam Buldum" der gibi bakar. Oysaki mercekten tekrar baktığında aşağıdaki IL çıktısı ile karşılaşacaktır.

```bash
.class private sequential ansi sealed beforefieldinit Product
    extends [mscorlib]System.ValueType
{
    .field public float64 ListPrice
    .field public int32 ProductId
    .field public string Title
}
```

Fark edileceği üzere burada ctor gibi bir metod tanımı bulunmamaktadır. Demek ki yanlış bir yerde arama yapılmaktadır. Scully derhal merceği ayarlar ve bu kez Main metodu içeriğine odaklanır.

```bash
.class private auto ansi beforefieldinit Program
    extends [mscorlib]System.Object
{
    .method public hidebysig specialname rtspecialname instance void .ctor() cil managed
    {
        .maxstack 8
        L_0000: ldarg.0 
        L_0001: call instance void [mscorlib]System.Object::.ctor()
        L_0006: ret 
    }

    .method private hidebysig static void Main(string[] args) cil managed
    {
        .entrypoint
        .maxstack 2
        .locals init (
            [0] class ClassStructAndConstructors.Person burak,
            [1] valuetype ClassStructAndConstructors.Product mouse)
        L_0000: nop 
        L_0001: newobj instance void ClassStructAndConstructors.Person::.ctor()
        L_0006: stloc.0 
        L_0007: ldloc.0 
        L_0008: ldc.i4 0x3e9
        L_000d: stfld int32 ClassStructAndConstructors.Person::PersonId
        L_0012: ldloc.0 
        L_0013: ldstr "Burak Selim \u015eenyurt"
        L_0018: stfld string ClassStructAndConstructors.Person::Name
        L_001d: ldloc.0 
        L_001e: ldc.r8 1
        L_0027: stfld float64 ClassStructAndConstructors.Person::Salary
        L_002c: ldloca.s mouse
        L_002e: initobj ClassStructAndConstructors.Product
        L_0034: ldloca.s mouse
        L_0036: ldc.i4 0x7d1
        L_003b: stfld int32 ClassStructAndConstructors.Product::ProductId
        L_0040: ldloca.s mouse
        L_0042: ldstr "Microsoft Optical Mouse"
        L_0047: stfld string ClassStructAndConstructors.Product::Title
        L_004c: ldloca.s mouse
        L_004e: ldc.r8 9.99
        L_0057: stfld float64 ClassStructAndConstructors.Product::ListPrice
        L_005c: ret 
    }
}
```

new Person () çağrısı için IL tarafında newobj çağrısını gerçekleştirilirken, new Product () satırına karşılık olarak initobj isimli bir çağrının gerçekleştirildiği görülmektedir. initobj için MSDN kaynaklarında yapılan açıklama şu şekildedir.

[![blg192_InitobjDefinition.gif](/assets/images/2010/blg192_InitobjDefinition.gif)](http://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.initobj%28VS.100%29.aspx)

Bir başka deyişle initobj çağrısının uygulandığı Struct içerisindeki Primitive tipler için, varsayılan ilk değer atamaları gerçekleştirilmektedir. Buna göre sayısal değerler için 0 veya 0.0, bool için false ve referans türleri için de null değerlerin atanması söz konusudur. Açıkça ifade etmek gerekirse Struct'lar için varsayılan bir yapıcı metod söz konusu olmasa dahi, IL tarafında bu fonksiyonelliği üstelenen bir çağrı yapılmaktadır (Base Class Library Team tarafından bu durum implicit default constructor) olarak adlandırılmaktadır.

Ajan Scully gözlerini mercekten kaldırır ve Fox'a döner. Bu kez yüzünde haklı bir tebessüm vardır ve "Endişelerin boşunaymış Mulder. Uzaylıların bu işle hiç bir alakası yok" der

![Laughing](/assets/images/2010/smiley-laughing.gif)

Başka bir macerada görüşünceye dek hepinize mutlu günler dilerim.

[ClassStructAndConstructors.rar (21,32 kb)](/assets/files/2010/ClassStructAndConstructors.rar) [Örnek Visual Studio 2010 Ultimate RTM sürümü üzerinde geliştirilmiş ve test edilmiştir]
