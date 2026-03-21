---
layout: post
title: "C# 4.0 - ExpandoObject"
date: 2009-10-21 07:14:00 +0300
categories:
  - csharp-4-0
tags:
  - csharp
  - dynamic-language-runtime
  - .net-framework
---
Bildiğiniz üzere.Net Framework 4.0 ile birlikte gelmesi muhtemel en köklü yenilikler arasında Dynamic Language Runtime alt yapısı yer almaktadır. Bu anlamda [daha önceden](https://www.buraksenyurt.com/post/C-40-Dynamic-Olmak)dynamic anahtar kelimesini inceleyerek tiplerin dinamik olarak oluşturulup kullanılmasını kavramaya çalışmıştık. Bu yazımızda nasıl bir yenilikten bahsedeceğimizi anlatabilmek için öncelikle aşağıdaki kod parçasına odaklanmanızı istiyorum.

Not: Örnek henüz yayınlanmış olan Visual Studio 2010 Ultimate Beta 2 sürümü üzerinde geliştirilmiş bir Console uygulamasıdır.

```csharp
Console.WriteLine("İlk Bakış\n");

dynamic employee = new ExpandoObject();

employee.Name = "Burak";
employee.Salary = 1000.23F;
employee.Birth = new DateTime(1976, 12, 4);
employee.WorkingArea = new ExpandoObject();
employee.WorkingArea.City = "Istanbul";
employee.WorkingArea.Degree = 1;
employee.WorkingArea.CustomerCount = 190;

Console.WriteLine(String.Format("\tName:{0} City:{1} Degree:({2})", employee.Name, employee.WorkingArea.City, employee.WorkingArea.Degree));
```

Şimdi bu kod parçası ile ilişkili olara bir kaç ipucu vermek istiyorum;

Öncelikli olarak uygulamamızda Name,Salary ve diğer özelliklere sahip Employee gibi ismi olan herhangibir tip (type) tanımı bulunmamaktadır ![Wink](/assets/images/2009/smiley-wink.gif)
İkinci kodu yazdığımız sırada employee isimli değişkene herhangi bir özellik eklemek istediğimizde aşağıdaki bilgi penceresi ile karşılaşırız.

![blg92_DynamicExpression.gif](/assets/images/2009/blg92_DynamicExpression.gif)

Ahaaaa!!!

![Wink](/assets/images/2009/smiley-wink.gif)

Sanıyorum olayın sizde farkına varmış durumdasınız. Aslında employee ismiyle tanımlamış olduğumuz değişkeni bir sınıf nesne örneği olarak inşa ettiğimizi düşünebiliriz. Ancak bunun için Emplyee tipini tanımlamış değiliz. Tamamen kodlama zamanında verdiğimiz bazı kararlar uygulamaktayız. Örneğin employee ismiyle tanımlanan tipin ve içeriğinin oluşturulması aşaması tamamen çalışma zamanına (Runtime) bırakılmış durumdadır. employee isimli değişkenin Name, Salary, Birth gibi özellikleri dışında başka bir ExpandoObject tipine işaret eden WorkingArea isimli bir özelliği daha bulunmaktadır.

Buna göre WorkingArea özelliği de bir tip olarak düşünülebilir ki City, Degree, CustomerCount gibi özellikleri yer almaktadır. Tabiki çalışma zamanında, özelliklere (Properties) atanan değerlere göre tiplerin ne olacağı da belirlenmektedir. Buna göre örneğin Name özelliğinin çalışma zamanında string tipinden olacağı açık ve nettir. Ancak WorkingArea özelliğinin kendiside aslında bir ExpandoObject tipidir. Kodun ilerleyen kısımlarına baktığımızda, employee değişkeninin özelliklerini kullanabildiğimizi de görebiliriz. Örnek kod parçamızın çalışma zamanı çıktısı aşağıdaki gibi olacaktır.

![blg92_FirstRun.gif](/assets/images/2009/blg92_FirstRun.gif)

ExpandoObject tipinin aslında hangi amaçla kullanıldığını ilk etapta görmüş olduk. Peki daha neler yapabiliriz? Örneğin bir liste veya dizi oluşturulmasında, ExpandoObject nesnelerini kullanabilir miyiz? İşte cevabımız;

```csharp
Console.WriteLine("\nNesne Topluluğu\n");

dynamic personList = new List<dynamic>();

personList.Add(new ExpandoObject());
personList[0].Name = "Burak";
personList[0].Salary = 1000;

personList.Add(new ExpandoObject());
personList[1].Name = "Bill";
personList[1].Salary = 1250;

personList.Add(new ExpandoObject());
personList[2].Name = "Eva";
personList[2].Salary = 1050;

personList.Add(new ExpandoObject());
personList[3].Name = "Mayk";
personList[3].Salary = 900;

foreach (var person in personList)
    Console.WriteLine("\t{0}\t{1}", person.Name, person.Salary);
```

Bu sefer List tipinden bir koleksiyon oluşturulduğu ve koleksiyonun her bir nesnesinin ExpandoObject tipinden tanımlandığı görülmektedir. Buna göre örnek olarak Name ve Salary özellikleri olan nesnelerin dynamic listeye eklendiği görülebilir. Tabiki her öğe oluşturulma işleminden önce koleksiyona ExpandoObject tipinden bir nesne örneği eklenmesi gerekmektedir. Örneğin çalışma zamanındaki çıktısı aşağıdaki gibidir.

![blg92_SecondRun.gif](/assets/images/2009/blg92_SecondRun.gif)

Hatta istersek çalışma zamanında oluşturulacak bu nesneler üzerinde LINQ sorgularının çalıştırılmasını da sağlayabiliriz. Aşağıdaki kod parçasında yukarıdaki listenin bir LINQ ifadesi ile sorgulandığı görülmektedir.

```csharp
Console.WriteLine("\nLINQ Kullanımı\n");

var result = from person in (personList as List<dynamic>)
    where person.Salary <= 1000
    select person;

foreach (var r in result)
    Console.WriteLine("\t{0}\t{1}", r.Name, r.Salary);
```

Bu kez dynamic liste içerisindeki nesnelerden Salary özelliği 1000 birime eşit ve az olanların çekilmesi için bir LINQ ifadesi kullanılmaktadır. Çalışma zamanındaki sonuç aşağıdaki gibi olacaktır.

![blg92_ThridRun.gif](/assets/images/2009/blg92_ThridRun.gif)

Daha neler yapabilir? Acaba tanımladığımız ExpandoObject tipinden nesne için bir olay (Event) ekleyebilir miyiz? Hımmm...

![Undecided](/assets/images/2009/smiley-undecided.gif)

Bu oldukça ilginç olabilir. Aşağıdaki kod parçası ile bir deneyelim bakalım;

```csharp
static void Main(string[] args)
{
   #region Event Kullanımı

   dynamic file = new ExpandoObject();

   file.Name = "DynamicOlmak.txt";
   file.Size = 1024;

   file.SizeChanged = null; // Bu satır olmadığı takdirde hata mesajı alınmakta.
   file.SizeChanged += new EventHandler(OnSizeChanged);
   EventHandler e = file.SizeChanged;
   
   Random rnd = new Random();
   file.Size += rnd.Next(1000, 10000);

   if (file.Size>=5000 && e != null)
      e(file, null);

   #endregion
}

public static void OnSizeChanged(object sender, EventArgs e)
{
   Console.WriteLine("Boyut değişti olayı tetiklendi");
}
```

Bu sefer file isimli değişkenin işaret ettiği tipe SizeChanged isimli bir olay (event) bildirimi yapılmaktadır. Event'ler bildiğiniz üzere delegate tipleri ile ilişkilidir. Örnekte basitlik açısından standart EventHandler temsilcisi kullanılmıştır ki dilerseniz farklı handler'ları veya kendi tanımladığınız delegate tiplerini de ele alabilirsiniz. Olayın file nesnesine eklenmesi sırasında birde olay metodunun işaret edildiği görülmektedir. Bildiğiniz üzere bu olay metodu, EventHandler temsilcisinin belirttiği parametrelere ve dönüş tipine sahip olmalıdır.

Buna göre OnSizeChanged metodu olayın gerçekleşmesi sonrası devreye girecek olan fonksiyonumuzdur. Çok doğal olarak olayın bir sebepten tetikleniyor olması gerekir. Örnekte Size değerinin 5000' in üzerinde olması durumu değerlendirilmeye çalışılmıştır. Eğer böyle bir koşul oluşur ve e ile ifade edilen olay file nesnesine daha önceden ilave edilirse söz konu olay metodu tetiklenecektir. Aslında uygulamayı Debug ettiğimizde söz konusu olay metodu için MulticastDelegate kullanıldığını rahatlıkla görebiliriz.

![blg92_Watch.gif](/assets/images/2009/blg92_Watch.gif)

Dikkat edileceği üzere dinamik olarak üretilen nesne içerisinde yer alan IDictionary koleksiyonunda Name, Size özellikleri ile birlikte MulticastDelegate tipinden tanımlanmış bir metod bildirimde yer almaktadır.

Peki ExpandoObject örneklerini metodlara parametre olarakta geçirebilir miyiz? Elbette

![Wink](/assets/images/2009/smiley-wink.gif)

Aşağıdaki kod parçasını göz önüne alalım;

```csharp
static dynamic file = new ExpandoObject();

static void Main(string[] args)
{
   #region Metoda Parametre Aktarımı

   file.Name = "DynamicOlmak.txt";
   file.Size = 1024;

   Console.WriteLine("Metod çağrısı öncesi dosya boyutu {0}",file.Size);
   Action(file);
   Console.WriteLine("Metod çağrısı sonrası dosya boyutu {0}", file.Size);

   #endregion
}

static void Action(dynamic obj)
{
   obj.Size += 456;
}
```

Burada file değişkeni sınıf seviyesinde tanımlanmıştır. Tabi örneği Console uygulamasında geliştirdiğimizden dynamic olarak tanımlanan ExpandoObject tipinin başına static anahtar kelimesininde gelmesi gerekmektedir. Bu kodu okurken biraz tuhaflığa neden olmaktadır.

![Undecided](/assets/images/2009/smiley-undecided.gif)

ExpandoObject örneği Action metoduna dynamic tipi üzerinden aktarılmaktadır. Dolayısıyla Action metodu içerisinde kullanılan obj değişkeninin file değişkeni olduğunu metod çağrısını yaptığımız yerde belirtmekteyiz. Bu nedenle Action metodu içerisine zaten file değişkeninin geldiğini biliyor olmalıyız. (Peki hangi tipin geldiğini anlayabilir miyiz? İşte size süper bir araştırma konusu ![Wink](/assets/images/2009/smiley-wink.gif)) Metod içerisinde sembolik olarak Size değerini arttırmaktayız. İşte çalışma zamanı sonucu;

![blg92_FourthRun.gif](/assets/images/2009/blg92_FourthRun.gif)

ExpandoObject tipi görüldüğü üzere dinamik olarak tiplerin oluşturulması, onlara üye tanımlanması (Özellik, Event gibi), özelliklerine değer atanması gibi işlemleri yapabilmemize olanak sağlamaktadır. Bu anlamda test senaryolarında stub veya moc nesneler için kullanılabilecek bir yenilik olarak düşünebiliriz belkide. Aklıma ilk gelen kullanım alanın bu olması, belkide [The Art of Unit Testing](http://www.amazon.com/Art-Unit-Testing-Examples-NET/dp/1933988274/ref=sr_1_1?ie=UTF8&s=books&qid=1256110465&sr=8-1) kitabını okuyor olmamadan da kaynaklanabilir.

![Wink](/assets/images/2009/smiley-wink.gif)

Aslında söz konusu yeniliği tanımak, kavramak ve gerçek hayat senaryolarındaki yerini anlamak için biraz daha araştırma yapmamız, çalışmamız gerektiği ortadadır. En azından benim...

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[ConsoleApplication1.rar (25,51 kb)](/assets/files/2009/ConsoleApplication1.rar)