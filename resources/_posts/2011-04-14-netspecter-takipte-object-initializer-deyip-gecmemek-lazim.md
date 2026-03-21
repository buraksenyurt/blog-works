---
layout: post
title: "Netspecter Takipte - Object Initializer Deyip Geçmemek Lazım"
date: 2011-04-14 14:34:00 +0300
categories:
  - csharp
  - csharp-3-0
tags:
  - csharp
---
Puslu bir sonbahar akşamında detektif Netspecter odasında sessiz sakin oturmaktadır. Loş bir ortama neden olan gece lambasının yeşil cam aksamı altından oda içerisindeki tozların sessiz ve sakin akışı bir yana, Netspecter’ ın kafasında masasına yeni gelen dosya ile ilişkili soru işaretleri koşup durmaktadır.

[![blg229_Giris](/assets/images/2011/blg229_Giris_thumb.gif)](/assets/images/2011/blg229_Giris.gif)


Sıkıntılı geçen bir kaç saat sonrasında aniden telefon çalar. Ölüm sessizliği içerisinde olan odanın neredeyse canlanmasına neden olan bir çalıştır bu. Ahizeyi ancak bir kaç seferden sonra fark edip kulağına götüren Netspecter, karşısında acı çektiği belli olan bir inleme ile irkilir.

Diğer ses: Objjj….eeeccttt!!!

Netspector: Kimsiniz

Diğer ses: Obb…ect!!!

Netspector: Oba makarnası mı? Nalo, nalooo…Anlamıyorum. Etecer mi?

Diğer ses: Aghhh!!!

Netspector: Ha ağrı kesici?

Diğer ses: Object Iniiittt….rrrr!!!

Diğer taraf: Dıt dıt dıt dıtttt!!!

Derken telefon sesi aniden kesilir. Netspector hemen sandalyesinde çabucak doğrulur, fotör şapkasını takar ve kapıdan hızla çıkar. Sevgili kedisi CAD ise bu telaşı umursamadan yemek kabındaki sütünü içmeye devam etmekte ve her içişten sonra patilerini temizlemektedir. Aslında Netspector’ ın kafasındaki güzergah bellidir. Şehir merkezinde ki büyük MSDN kütüpahnesine uğrayacak ve Object Initializer ile ilgili bir kaç soru sorup olayı çözmeye çalışacaktır.

## Asıl Mevzu

C# 3.0 ile birlikte gelen önemli yeniliklerden birisi de Object Initializers kullanımı idi. Bu kullanım sayesinde özellikle LINQ (Language Integrated Query) sorgularında Anonymous Type üretiminin mümkün hale gelmesi de sağlanmaktaydı. Dolayısıyla her zaman ifade ettiğimiz gibi bu yenilik, başka bir yeniliğin yapılabilmesi için getirilmiş bir yenilikti

![Wink](/assets/images/2011/smiley-wink.gif)

Tabi o zamandan beri Object Initializers kavramı üzerinde çok fazla yazıp çizdim ama yine de pek fazla derinlerine girmediğimi ya da olaya farklı gözeler ile bakmadığımı farkettim. Bu nedenle konuyu farklı bir yaklaşım ile ele almanın daha doğru olacağına karar verdim. Dilerseniz kavramın derinliklerine inerken örnek bir kod parçası ile küçük bir başlangıç yapmaya çalışalım. Bu amaçla Visual Studio 2010 ortamında aşağıdaki Converter isimli sınıfı kullanan bir Console uygulamasını göz önüne alıyor olacağız.

[![blg229_ConverterClassDiagram](/assets/images/2011/blg229_ConverterClassDiagram_thumb.gif)](/assets/images/2011/blg229_ConverterClassDiagram.gif)

Örnek program kodunu ise aşağıdaki gibi geliştirebiliriz.

```csharp
namespace CaseObjectInitializers 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            //Object Initializer kullanarak nesneyi başlatmak 
            Converter cvrtr = new Converter { Source = "c:\\source.xls", Target = "c:\\target.txt" };

            // Klasik yöntem. Önce varsayılan yapıcı metod çağrısı, ardından özelliklere değer atama yolu ile başlatmak 
            Converter cvrt2 = new Converter(); 
            cvrt2.Source = "c:\\source.xls"; 
            cvrt2.Target = ":\\target.text"; 
        } 
    }

    class Converter 
    { 
        public string Source { get; set; } 
        public string Target { get; set; } 
    } 
}
```

Bu kod parçasında dikkat edileceği üzere Converter tipinden iki farklı nesne örneklenmesi yapılmaktadır. cvrtr isimli ilk değişkenin üretimi sırasında object initializer tekniği kullanılmış ve süslü parantezler içerisinde public olan söz konusu tipe ait Source ve Target özelliklerine bazı değerler verilmiştir. cvrt2 isimli değişkenin oluşturulma şekli ise tam bir klasiktir

![Smile](/assets/images/2011/smiley-smile.gif)

İlk olarak varsayılan yapıcı (Default Constructor) metoddan yararlanılmış ve sonrasında örnek nesne üzerinden Source ve Target özelliklerine değerleri atanmıştır. Çok güzel. Güzel ama ilk örnekleme işleminde kullanılan Object Initializer tekniği aslında bir sihir mi uygulamıştır?

> Detektifin konuyu çözmesi için ipuçlarını daha iyi görebileceği bir aygıta ve örneğe ihtiyacı vardır. Karanlık odayı loş bir şekilde aydınlatan ışığın altında düşünürken, aklına parlak bir fikir gelir. Mikroskop olarak daha bir kaç gün öncesinde kermesten aldığı eski pentium işlemcili makinede yer alan ILDASM aracını, örnek olarakta makdulun salonda bıraktığı Console uygulamasına ait exe çıktısını kullanacaktır.

ILDASM (Intermediate Language DisASeMbler Tool) aracı yardımıyla gerekli IL çıktısına baktığımızda aşağıdaki sonuçlar ile karşılaşırız.

```text
.method private hidebysig static void  Main(string[] args) cil managed 
{ 
  .entrypoint 
  // Code size       64 (0x40) 
  .maxstack  2 
  .locals init ([0] class CaseObjectInitializers.Converter cvrtr, 
           [1] class CaseObjectInitializers.Converter cvrt2, 
           [2] class CaseObjectInitializers.Converter '<>g__initLocal0') 
  IL_0000:  nop 
  IL_0001:  newobj     instance void CaseObjectInitializers.Converter::.ctor() 
  IL_0006:  stloc.2 
  IL_0007:  ldloc.2 
  IL_0008:  ldstr      "c:\\source.xls" 
  IL_000d:  callvirt   instance void CaseObjectInitializers.Converter::set_Source(string) 
  IL_0012:  nop 
  IL_0013:  ldloc.2 
  IL_0014:  ldstr      "c:\\target.txt" 
  IL_0019:  callvirt   instance void CaseObjectInitializers.Converter::set_Target(string) 
  IL_001e:  nop 
  IL_001f:  ldloc.2 
  IL_0020:  stloc.0 
  IL_0021:  newobj     instance void CaseObjectInitializers.Converter::.ctor() 
  IL_0026:  stloc.1 
  IL_0027:  ldloc.1 
  IL_0028:  ldstr      "c:\\source.xls" 
  IL_002d:  callvirt   instance void CaseObjectInitializers.Converter::set_Source(string) 
  IL_0032:  nop 
  IL_0033:  ldloc.1 
  IL_0034:  ldstr      ":\\target.text" 
  IL_0039:  callvirt   instance void CaseObjectInitializers.Converter::set_Target(string) 
  IL_003e:  nop 
  IL_003f:  ret 
} // end of method Program::Main
```

Dikkat edileceği üzere IL0001 satırından Converter tipine ait varsayılan yapıcı metod (Default Constructor) çağırılmıştır. IL000d numaralı satırda ise Source özelliği için set metodunun çağırıldığı görülmektedir. Diğer yandan benzer işlem IL0019 numaralı satırda da yapılmaktadır. Özetlemek gerekirse Object Initializer kullandığımızda aslında klasik yöntem de uygulanan önce yapıcı metodun çağırılması ve sonrasında özelliklere değer atanması işlemi IL tarafında aynen yapılmaktadır. Hatta koleksiyonları initializer ile başlatttığımız durumlarda da benzer bir üretim modeli söz konusu olacaktır. Bu durumu da analiz edersek yerinde olur mu? Olur

![Laughing](/assets/images/2011/smiley-laughing.gif)

Bu amaçla uygulamamıza aşağıdaki kod satırlarını eklediğimizi düşünelim.

```csharp
List<Converter> converters = new List<Converter> 
{ 
    new Converter{Source="c:\\source1.xls",Target="C:\\target1.txt"}, 
    new Converter{Source="c:\\source2.xls",Target="C:\\target2.txt"}, 
    new Converter{Source="c:\\source3.xls",Target="C:\\target3.txt"}, 
};
```

Bu sefer hem List tipi için hem de içerisine eklenen her bir Converter tipi için yapılan örnekleme işlemlerinde Object Initializer tekniği kullanılmıştır. Bildiğiniz üzere IEnumerable arayüzünü uygulayan ve Add metodunu içeren koleksiyon tipleri için de Object Initializer tekniğinden yararlanılabilmektedir. Peki ya IL çıktısı?

![Sealed](/assets/images/2011/smiley-sealed.gif)

```text
.method private hidebysig static void  Main(string[] args) cil managed 
{ 
  .entrypoint 
  // Code size       203 (0xcb) 
  .maxstack  3 
  .locals init ([0] class CaseObjectInitializers.Converter cvrtr, 
           [1] class CaseObjectInitializers.Converter cvrt2, 
           [2] class [mscorlib]System.Collections.Generic.List`1<class CaseObjectInitializers.Converter> converters, 
           [3] class CaseObjectInitializers.Converter '<>g__initLocal0', 
           [4] class [mscorlib]System.Collections.Generic.List`1<class CaseObjectInitializers.Converter> '<>g__initLocal1', 
           [5] class CaseObjectInitializers.Converter '<>g__initLocal2', 
           [6] class CaseObjectInitializers.Converter '<>g__initLocal3', 
           [7] class CaseObjectInitializers.Converter '<>g__initLocal4') 
  IL_0000:  nop 
  IL_0001:  newobj     instance void CaseObjectInitializers.Converter::.ctor() 
  IL_0006:  stloc.3 
  IL_0007:  ldloc.3 
  IL_0008:  ldstr      "c:\\source.xls" 
  IL_000d:  callvirt   instance void CaseObjectInitializers.Converter::set_Source(string) 
  IL_0012:  nop 
  IL_0013:  ldloc.3 
  IL_0014:  ldstr      "c:\\target.txt" 
  IL_0019:  callvirt   instance void CaseObjectInitializers.Converter::set_Target(string) 
  IL_001e:  nop 
  IL_001f:  ldloc.3 
  IL_0020:  stloc.0 
  IL_0021:  newobj     instance void CaseObjectInitializers.Converter::.ctor() 
  IL_0026:  stloc.1 
  IL_0027:  ldloc.1 
  IL_0028:  ldstr      "c:\\source.xls" 
  IL_002d:  callvirt   instance void CaseObjectInitializers.Converter::set_Source(string) 
  IL_0032:  nop 
  IL_0033:  ldloc.1 
  IL_0034:  ldstr      ":\\target.text" 
  IL_0039:  callvirt   instance void CaseObjectInitializers.Converter::set_Target(string) 
  IL_003e:  nop 
  IL_003f:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class CaseObjectInitializers.Converter>::.ctor() 
  IL_0044:  stloc.s    '<>g__initLocal1' 
  IL_0046:  ldloc.s    '<>g__initLocal1' 
  IL_0048:  newobj     instance void CaseObjectInitializers.Converter::.ctor() 
  IL_004d:  stloc.s    '<>g__initLocal2' 
  IL_004f:  ldloc.s    '<>g__initLocal2' 
  IL_0051:  ldstr      "c:\\source1.xls" 
  IL_0056:  callvirt   instance void CaseObjectInitializers.Converter::set_Source(string) 
  IL_005b:  nop 
  IL_005c:  ldloc.s    '<>g__initLocal2' 
  IL_005e:  ldstr      "C:\\target1.txt" 
  IL_0063:  callvirt   instance void CaseObjectInitializers.Converter::set_Target(string) 
  IL_0068:  nop 
  IL_0069:  ldloc.s    '<>g__initLocal2' 
  IL_006b:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class CaseObjectInitializers.Converter>::Add(!0) 
  IL_0070:  nop 
  IL_0071:  ldloc.s    '<>g__initLocal1' 
  IL_0073:  newobj     instance void CaseObjectInitializers.Converter::.ctor() 
  IL_0078:  stloc.s    '<>g__initLocal3' 
  IL_007a:  ldloc.s    '<>g__initLocal3' 
  IL_007c:  ldstr      "c:\\source2.xls" 
  IL_0081:  callvirt   instance void CaseObjectInitializers.Converter::set_Source(string) 
  IL_0086:  nop 
  IL_0087:  ldloc.s    '<>g__initLocal3' 
  IL_0089:  ldstr      "C:\\target2.txt" 
  IL_008e:  callvirt   instance void CaseObjectInitializers.Converter::set_Target(string) 
  IL_0093:  nop 
  IL_0094:  ldloc.s    '<>g__initLocal3' 
  IL_0096:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class CaseObjectInitializers.Converter>::Add(!0) 
  IL_009b:  nop 
  IL_009c:  ldloc.s    '<>g__initLocal1' 
  IL_009e:  newobj     instance void CaseObjectInitializers.Converter::.ctor() 
  IL_00a3:  stloc.s    '<>g__initLocal4' 
  IL_00a5:  ldloc.s    '<>g__initLocal4' 
  IL_00a7:  ldstr      "c:\\source3.xls" 
  IL_00ac:  callvirt   instance void CaseObjectInitializers.Converter::set_Source(string) 
  IL_00b1:  nop 
  IL_00b2:  ldloc.s    '<>g__initLocal4' 
  IL_00b4:  ldstr      "C:\\target3.txt" 
  IL_00b9:  callvirt   instance void CaseObjectInitializers.Converter::set_Target(string) 
  IL_00be:  nop 
  IL_00bf:  ldloc.s    '<>g__initLocal4' 
  IL_00c1:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class CaseObjectInitializers.Converter>::Add(!0) 
  IL_00c6:  nop 
  IL_00c7:  ldloc.s    '<>g__initLocal1' 
  IL_00c9:  stloc.2 
  IL_00ca:  ret 
} // end of method Program::Main
```

Haydi gelin bir çılgınlık yapalım ve bu IL kodunu okumaya çalışalım

![Wink](/assets/images/2011/smiley-wink.gif)

Dikkat edileceği üzere IL003f satırında List tipi için varsayılan yapıcı metod çağrısı gerçekleştirilmektedir. Dolayısıyla bu satırda ilgili koleksiyona ait bir nesne örneğinin ürettirildiğini düşünebiliriz. Diğer yandan IL0048 numaralı satırda bir Converter tipi örneklemesi için varsayılan yapıcı metod çağrısı söz konusudur. IL0056 ve IL0063 numaralı satırlarda ise, IL0048’ de üretilen Converter nesnesine ait özelliklerin (Source ve Target) değerleri atanmaktadır. IL006b satırında ise IL0048’ de üretilen ve IL0056 ile IL0063 satırlarında sırasıyla Source ve Target özelliklerine değer atanan Converter nesne örneğinin IL003f satırında örneklenen List tipli koleksiyon örneğine Add metodu ile eklendiği gözlemlenmektedir. Bu IL çağrı akışı diğer iki Converter nesne örneği için de geçerli olacaktır.

Sanıyorum ki Object Initializer tekniği hakkında biraz daha derin fikir sahibi olmaya başladık. Yeter mi? Yetmez. Bakın daha neler var?

beforefieldinit

Örneğimize aşağıdaki sınıf örneğini eklediğimi düşünelim.

[![blg229_FileManagerClassDiagram](/assets/images/2011/blg229_FileManagerClassDiagram_thumb.gif)](/assets/images/2011/blg229_FileManagerClassDiagram.gif)

```csharp
static class FileManager 
{ 
    static List<Converter> _converters = new List<Converter>();

    static FileManager() 
    { 
        Converter c1 = new Converter(); 
        c1.Source = "c:\\sourcex.xls"; 
        c1.Target = "c:\\targetx.txt"; 
        _converters.Add(c1);

        Converter c2 = new Converter(); 
        c2.Source = "c:\\sourcez.xls"; 
        c2.Target = "c:\\targetz.txt"; 
        _converters.Add(c2); 
    } 
}
```

FileManager static tipli bir sınıf olmakla birlikte converters isimli List koleksiyonu türünden bir alan içermekedir. Söz konusu alan tanımlandığı yerde new operatörü ile örneklenmiş ve ilk değerlerinin atanması için FileManager static yapıcı metodu kullanılmıştır. Söz konusu sınıfın IL çıktısına baktığımızda özellikle sınıf tanımının aşağıdaki şekilde olduğu gibi yapıldığı görülecektir.

[![blg229_FileManagerIL1](/assets/images/2011/blg229_FileManagerIL1_thumb.gif)](/assets/images/2011/blg229_FileManagerIL1.gif)

Şimdi sınıf kodunu biraz daha değiştirelim.

```csharp
static class FileManager 
{ 
    static List<Converter> _converters = new List<Converter> 
    { 
        new Converter{Source="c:\\sourcesx.xls",Target="c:\\targetx.txt"}, 
        new Converter{Source="c:\\sourcesz.xls",Target="c:\\targetz.txt"} 
    }; 
}
```

FileManager tipinin yeni versiyonunda converters isimli koleksiyon örneğini oluşturmak için static yapıcı metod yerine object initializer kullandığımızı görmekteyiz. Bu durumda uygulamanın IL çıktısına baktığımızda FileManager tipi için aşağıdaki tanımlamanın yapıldığına şahit oluruz.

[![blg229_FileManagerIL2](/assets/images/2011/blg229_FileManagerIL2_thumb.gif)](/assets/images/2011/blg229_FileManagerIL2.gif)

İki örneğe ait tanımlamalara baktığımızda beforefieldinit isimli bir değerin kullanıldığı fark edilmektedir. Aslında bu değerin uygulama performansını arttırıcı bir etkisi olduğunu ifade edebiliriz. Normalde static yapıcı metod kullandığımızda, tip içerisinde yer alan tüm static değişkenlerin ilk erişimlerinden önce örneklenip örneklenmediklerine dair bir kontrol işlemi uygulanmaktadır. Ancak son kod parçamızda olduğu gibi Object Initializer kullanır ve static yapıcı metodu dışarıda bırakırsak, bu durumda sınıf söz konusu beforrefiledinit özelliği ile işaretlenerek ilgili kontrol işleminin atlanması sağlanır. Bu da azıcık bile olsa performans kazanımı anlamına gelmektedir

![Wink](/assets/images/2011/smiley-wink.gif)

Görüldüğü üzere Object Initializer karvramını sadece kodun yazımını kısaltan bir yenilik şeklinde düşünmemek gerekir. Ancak yazım ve kod okunurluğunu kolaylaştırdığı da bir gerçektir. Bunun için aşağıdaki tabloya bakmanız sanırım yeterli olacaktır

![Wink](/assets/images/2011/smiley-wink.gif)

Klasik Yaklaşım
Object Initializer ile Yaklaşım

using System.Collections.Generic;
namespace CaseObjectInitializers
{
class Program
{
static void Main (string[] args)
{
List books = new List ();
Book newBook = new Book ();
newBook.Id = 1;
newBook.Name = "Dick Tracy Maceraları 1";
newBook.Summary = "İlk bölüm maceraları";
newBook.ListPrice = 10;
newBook.Authors = new List ();
Author newAuthor = new Author ();
newAuthor.Id = 1;
newAuthor.Name = "Dick";
newAuthor.Surname = "Tracy";
newBook.Authors.Add (newAuthor);
books.Add (newBook);
newBook = new Book ();
newBook.Id = 2;
newBook.Name = "Uygulamalı WCF";
newBook.Summary = "İlk denemeler";
newBook.ListPrice = 34.49M;
newBook.Authors = new List ();
newAuthor = new Author ();
newAuthor.Id = 3;
newAuthor.Name = "Burak S.";
newAuthor.Surname = "Şenyurt";
newBook.Authors.Add (newAuthor);
newAuthor = new Author ();
newAuthor.Id = 4;
newAuthor.Name = "Ingo";
newAuthor.Surname = "Rammer";
newBook.Authors.Add (newAuthor);
books.Add (newBook);
}
}
class Book
{
public int Id { get; set; }
public string Name { get; set; }
public List Authors{ get; set; }
public string Summary { get; set; }
public decimal ListPrice { get; set; }
}
class Author
{
public int Id { get; set; }
public string Name { get; set; }
public string Surname { get; set; }
}
}

using System.Collections.Generic;
namespace CaseObjectInitializers
{
class Program
{
static void Main (string[] args)
{
List books = new List
{
new Book{Id=1, Name="Dick Tracy Maceraları 1", Summary="İlk bölüm maceraları", ListPrice=10, Authors=new List{
new Author{Id=1, Name="Dick", Surname="Tracy"}
}
},
new Book{Id=2,Name="Uygulamalı WCF", Summary="İlk denemeler", ListPrice=34.59M,Authors=new List{
new Author{Id=3,Name="Burak S",Surname="Şenyurt"},
new Author{Id=4,Name="Ingo",Surname="Rammer"}
}
}
};
}
}
class Book
{
public int Id { get; set; }
public string Name { get; set; }
public List Authors{ get; set; }
public string Summary { get; set; }
public decimal ListPrice { get; set; }
}
class Author
{
public int Id { get; set; }
public string Name { get; set; }
public string Surname { get; set; }
}
}

book koleksiyonunun oluşturulması için 36 satır
book koleksiyonunun oluşturulması için 13 satır

Her iki örnek kod parçasında da Book tipinden nesne örnekleri taşıyan birer List koleksiyonu oluşturulmakta ve doldurulmaktadır. Book tipi içerisinde Author tipinden de bir koleksiyon da yer almaktadır. Tabi Book ve Author sınıfları için aşırı yüklenmiş yapıcı metodlar (Overloaded Constructors) olmadığını varsayıyoruz bu senaryo da. Kodun daha da kısalması, performans yönündeki minik fark göz önüne alındığında biraz daha önemsiz gibi duruyor. Yine de çok fazla sayıda kod parçası içeren projelerde kodun okunurluğunun da geliştiricinin psikolojisi üzerinde doğrudan etkili olduğunu ifade etmek isterim. Böylece geldik bir yazımızın daha sonuna. Netspecter’ ın bir sonraki macerasında görüşünceye dek hepinize mutlu günler dilerim.

[CaseObjectInitializers.rar (23,71 kb)](/assets/files/2011/CaseObjectInitializers.rar)