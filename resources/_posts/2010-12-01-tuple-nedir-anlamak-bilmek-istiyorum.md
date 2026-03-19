---
layout: post
title: "Tuple Nedir? Anlamak, Bilmek İstiyorum."
date: 2010-12-01 03:55:00 +0300
description: "Tuple’ ın Türkçe' deki kelime karşılığı Demet olarak ifade edilmektedir. Matematikte elementlerin sıralanmış bir liste tasarımı olarak tanımlanmaktadır. Diğer yandan İlişkisel Veritabanı Sistemlerinde(Relational Database Management Systems) tablo içerisindeki bir satır olarak düşünülür. Dolayısıyla burada da sütunların sıralı bir dizisinden oluşan liste şeklinde ifade edildiğinde, Matematiksel tanımını da işaret ettiği ifade edilebilir."
categories:
  - dotnet-framework-4-0
  - csharp-4-0
tags:
  - dotnet-framework-4-0
  - csharp-4-0
  - csharp
  - dotnet
  - linq
  - rest
  - http
  - python
  - task-parallel-library
  - threading
  - generics
  - visual-studio
---
# Tuple Nedir? Anlamak, Bilmek İstiyorum.
[![blg200_Giris](/assets/images/2010/blg200_Giris_thumb.jpg)](/assets/images/2010/blg200_Giris.jpg) Merhaba Arkadaşlar,

Matematik Mühendisliği eğitimi almış birisi olarak hayatımın önemli bir kısmını teorem ispatlarına harcadığımı itiraf edebilirim. Tabi yaşamımın mesleki olarak kırılma anı sanıyorum ki üniversite yıllarında bilgisayara merak salmam ve programlama ile alakalı dersleri daha çok sevmemdi. Kısacası Matematik üzerine eğilmekten vazgeçip (ki bundan biraz pişmanlık duyduğumu ifade edebilirim) yazılım alanında ilerlemeyi kafama koymuştum.

Ama tabiki insanoğlu Matematik’ ten kaçamıyor. Hayatının belirli noktalarında öyle ya da böyle karşılaşıyor. Söz gelimi çalıştığım projelerin bazılarında, müşterinin sorunlarının çözümü için matematiksel modellerin kullandığını söyleyebilirim. Örneğin bir üretim hattının minimum maliyet gibi değerlere ulaşabilmesi için çalıştıracağı optimizasyon modelleri, dünyanın en zor matematik formüllerini içermektedir. Yazıya yaptığımız bu giriş sizleri korkutmasın. Bu günkü konumuzun Matematik ile çok alakası yok. Ancak Matematik dünyasından bir ismin yazılım tarafına geçirlişini inceleyeceğimizi ifade edebilirim. Bu günkü konumuz,.Net Framework 4.0 ile birlikte gelen Tuple.

[Tuple](http://en.wikipedia.org/wiki/Tuple)’ ın Türkçe'deki kelime karşılığı Demet olarak ifade edilmektedir. Matematikte elementlerin sıralanmış bir liste tasarımı olarak tanımlanmaktadır. Diğer yandan İlişkisel Veritabanı Sistemlerinde (Relational Database Management Systems) tablo içerisindeki bir satır olarak düşünülür. Dolayısıyla burada da sütunların sıralı bir dizisinden oluşan liste şeklinde ifade edildiğinde, Matematiksel tanımını da işaret ettiği ifade edilebilir. Bilgisayar literatüründe birden fazla parçadan oluşan yapı anlamında kullanılmaktadır. Python dilinde ise içeriği değiştirilemeyen bir dizidir. Python demişken. Aslında fonksiyonel programlama dillerinde Tuple kavramının uzun süredir var olduğunu da ifade edebiliriz. Hatta F# programlama dilinde de tuple kullanımı mevcuttur.

> Not: Bildiğiniz üzere Pyhton, LISP, Perl gibi dynamic dillerde tip belirtmeden değişken tanımlayabilmek mümkündür. Oysaki C# gibi static dillerde tip belirtme zorunluluğu vardır. Nitekim derleme işlemi sırasında değişkenlerin tiplerinin belli olması gereklidir. Bu dinamik dillerin değişken tanımlamada ve kullanmada sağladığı esnekliğin static dilde olmadığı anlamanı gelmektedir. Ancak static diller bu sayede daha az hata meyillidir.

Diğer yandan Tuple kavramı, C# gibi static dillerde en azından 4.0 versiyonuna kadar geçerli değildir..Net Framework 4.0 ile birlikte gelen önemli yeniliklerden birisi de bildiğiniz üzere dinamik diller ile olan etkileşimdir. Bu etkileşimin bir meyvesi veya ön hazırlığı olarak Tuple kavramının.Net Framework bünyesine katıldığını da ifade edebiliriz.

Bu kadar laf kalablığından sonra dilerseniz örnekler ile Tuple tipini incelemeye çalışalım. System isim alanı altında yer alan Tuple sınıfı, Tuple nesnelerinin üretimi için bir fabrika (Factory) görevi üstlenmektedir. Bunun için [static Create](http://msdn.microsoft.com/en-us/library/system.tuple_members(v=VS.100).aspx) metoduna ve aşırı yüklenmiş (Overload) versiyonlarına sahiptir. Tuple tiplerini kullanmak için çeşitli sebepler ve senaryolar düşünülebilir. Şimdi bu durumları analiz etmeye çalışalım.

## 1- Metodlardan İsimsiz Tip (Anonymous Type) Döndürülemediği Durumlar

Aslında Anonymous tipler özellikle LINQ (Language Integrated Query) sorguları için tasarlanmıştır. LINQ sorgulaması sonucu elde edilen sonuç kümesinden anlık olarak karar verilen bir tipin ele alınması istendiği durumlarda oldukça değerlidir. Lakin bu tipler metodlardan geriye döndürülememektedir. Aşağıdaki örnek kod parçasını göz önüne alalım.

```csharp
using System; 
using System.Collections.Generic; 
using System.Linq;

namespace TupleKavrami 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            List<Person> employees = new List<Person> 
            { 
                new Person{ PersonId=1,Name="Burak", Surname="Şenyurt", City="İstanbul",Salary=1000}, 
                new Person{ PersonId=2,Name="Maykıl", Surname="Cordın", City="Chicago",Salary=1250}, 
                new Person{ PersonId=3,Name="Şakiyıl", Surname="Oniyıl", City="Los Angles",Salary=1100} 
            };

            List<Tuple<string, string,double>> result = GetEmployees(employees); 
            foreach (Tuple<string,string,double> r in result) 
            { 
                Console.WriteLine("{0} {1} Salary : {2}",r.Item1,r.Item2,r.Item3.ToString("C2")); 
            } 
        }

        static List<Tuple<string, string,double>> GetEmployees(List<Person> employees) 
        { 
            var result = (from e in employees 
                         select new Tuple<string, string,double>(e.Name, e.Surname,e.Salary)).ToList<Tuple<string,string,double>>();

            return result; 
        } 
    }

    class Person 
    { 
        public int PersonId { get; set; } 
        public string Name { get; set; } 
        public string Surname { get; set; } 
        public string City { get; set; } 
        public double Salary { get; set; } 
    } 
}
```

Bu kod parçasında kayda değer pek çok yeni özellik bulunmaktadır. GetEmployees isimli metod, parametre olarak aldığı List tipinden bir koleksiyon içeriğini sorgulamaktadır. Bu sorgulama sonucunda aslında bir anonymous tip ile ifade edilebilecek yeni bir yapı değerlendirilmektedir. Person tipi içerisinde PersonId,Name,Surname,City ve Salary özellikleri yer alırken sorgu sonucu elde edilen satırlarda Name,Surname ve Salary özelliklerinin olması istenmektedir. Bu özellikler tahmin edileceği üzere bir anonymous tip içerisinde yer alabilirler. Ancak Anonymous tipin metoddan geriye döndürülemediğini ifade etmiştik. İşte bu noktada devreye Tuple tipini kattık.

Dikkat edilecek olursa LINQ sorgusu içerisinde Tuple tipinden bir tanımlama yapılmaktadır. Bu noktada kodun o anki satırında 3 alandan oluşacak bir tip tasarladığımızı düşünebiliriz. Bu tipin özelliklerine ait değerler ise o anki Person nesne örneği içerisinden alınmaktadır.

Tuple tipi ile ilişkili olarak dikkate değer noktalardan biriside foreach bloğu içerisinde kendisini göstermektedir. Tuple tipinin örneklenen versiyonunda belirtilen generic parametrelere göre özellikler Item1, Item2 ve Item3 olarak ayarlanmıştır. Bu bir handikap olarak düşünülebilir. Nitekim ele aldığımız örnekteki vakka düşünüldüğünde Person tipi içerisinden çekilen Name,Surname ve Salary alan adlarının Item1, Item2 ve Item3 yerine kullanılması daha esnektir. Bildiğiniz üzere Anoynmous tiplerde, oluşacak nesnenin o anki özelliklerinin adları aynen korunabileceği gibi geliştirici tarafından da tanımlanabilmektedir. Bu yeteniğin Tuple tipine de ekleneceğini ümit etmekteyim.

Tabi bir diğer önemli nokta Tuple nesne örneklemesi sırasında kullanılan T tipinin generic’ liğidir. Buna göre Tuple tipinin içereceği diğer tiplerin aynı olması zorunlu değildir. Örnekte string,string,double tipinden bir oluşum söz konusudur. Çalışma zamanı sonuçları aşağıdaki gibi olacaktır.

[![blg200_FirstRuntime](/assets/images/2010/blg200_FirstRuntime_thumb.gif)](/assets/images/2010/blg200_FirstRuntime.gif)

## 2 – Metodlardan Birden Fazla Değer Döndürmek İstediğimiz ve out Parametrelerini Tercih Ettiğimiz Durumlar.

Aslında bunun için pek çok yol mevcuttur. out tipinden metod parametreleri veya dönüş tipi olarak bir koleksiyon ya da dizi kullanılması söz konusu olabilir. Ancak Tuple tipi de bu anlamda değerlendirilebilir. Söz gelimi aşağıdaki kod parçasını göz önüne alalım.

```csharp
using System; 
using System.Collections.Generic; 
using System.Linq;

namespace TupleKavrami 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            int r1, r2, r3, r4; 
            Calculate(4, 2, out r1, out r2, out r3, out r4); 
            Console.WriteLine("{0} {1} {2} {3}",r1,r2,r3,r4); 
        }

        static void Calculate(int x, int y, out int result1, out int result2, out int result3, out int result4) 
        { 
            result1 = x + y; 
            result2 = x - y; 
            result3 = x * y; 
            result4 = x / y; 
        } 
    } 
}
```

Bu kod parçasında yer alan Calculate metodu dört işlemi gerçekleştirmekte ve elde edilen sonuçları out tipinden metod parametreleri ile geriye döndürmektedir. Bu örnekte out parametrelerinin kullanımı yerine geri dönüş tipinin Tuple olarak tanımlaması da düşünülebilir. Aşağıdaki kod parçasında bu durum ele alınmaktadır.

```csharp
using System; 
using System.Collections.Generic; 
using System.Linq;

namespace TupleKavrami 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            Tuple<int, int, int, int> calculation = CalculateV2(4, 2); 
            Console.WriteLine("{0} {1} {2} {3}",calculation.Item1,calculation.Item2,calculation.Item3,calculation.Item4); 
        }

        static Tuple<int, int, int, int> CalculateV2(int x, int y) 
        { 
            return Tuple.Create<int, int, int, int>(x + y, x - y, x * y, x / y); 
        } 
    } 
}
```

[![blg200_SecondRuntime](/assets/images/2010/blg200_SecondRuntime_thumb.gif)](/assets/images/2010/blg200_SecondRuntime.gif)

CalculateV2 isimli metod geriye Tuple tipinden bir nesne örneği döndürecek şekilde programlanmıştır. Bu seferki kod örneğimizde Tuple nesne örneğini oluşturmak için Tuple sınıfının static Create metodu tercih edilmiştir. Tabi bu örnekte Tuple nesne örneği içerisindeki tüm tipler integer olarak tanımlanmıştır. Ancak böyle bir zorunluluk olmadığını da daha önceden belirtmiştik. Yani Tuple tipi, kendi içerisinde farklı tipleri barındırabilmektedir. Tabi sonuçları ele aldığımız kod satırından Tuple nesne örneği içerisindeki değerlere yine Item1, Item2, Item3 ve Item4 isimli özellikler ile erişebildiğimize dikkat edelim.

Aslında Tuple nesne örnekleri oluşturulurken daha kısa kod notasyonları da tercih edilebilir. CalculateV2 metodunun içeriğini bu anlamda aşağıdaki gibi de kullanabiliriz.

```csharp
static Tuple<int, int, int, int> CalculateV2(int x, int y) 
{ 
	//return Tuple.Create<int, int, int, int>(x + y, x - y, x * y, x / y); 
	return Tuple.Create(x + y, x - y, x * y, x / y); 
}
```

Dikkat edileceği üzere burada Create metodunun parametrelerine doğrudan hesaplama sonuçları atanmış ama herhangibir şekilde tip bilgisi belirtilmemiştir.

## 3 – Bir metoda birden fazla parametre göndermek istediğimiz bazı durumlarda

Zaten bir metoda n sayıda parametre gönderebilmekteyiz. Ancak bizim müdahale edemediğimiz önceden tanımlanmış tip metodları, sadece tek parametre alarak n sayıda parametre aktarımını biraz zorlaştırmaktadır. Bu tip durumlar elbetteki çözümsüz değildir. Söz gelimi dizi veya koleksiyon yardımıyla birden fazla parametre taşınması söz konusu olabilir. Ancak bu tip bir vakada Tuple tiplerinden de yararlanabiliriz. Bu anlamda parametre alabilen metodların Thread yönetimlerini göz önüne alabiliriz.

```csharp
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading;

namespace TupleKavrami 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            ParameterizedThreadStart ts = new ParameterizedThreadStart(ThreadMethod); 
            Thread t = new Thread(ts); 
            t.Start(Tuple.Create(1, 9.23F, 2)); 
        }

        static void ThreadMethod(object values) 
        { 
            Tuple<int, float, int> tpl = (Tuple<int,float,int>)values; 
            Console.WriteLine("Gelen değerler {0} {1} {2}",tpl.Item1,tpl.Item2,tpl.Item3); 
        } 
    } 
}
```

ParameterizedThreadStart, Thread'lerin başlatacağı metodlara parametre aktarımına izin vermektedir. Ancak bu noktada object tipinden parametre alan metodlar işaret edilebilmektedir. Örnek kod parçasında t isimli Thread nesne örneği üzerinden Start metodu çağrısı gerçekleştirilirken, bir Tuple nesne örneği gönderilmektedir. ThreadMethod içerisindeki kod bloğunda ise dikkat edileceği üzere object tipinden olan parametre bilinçli bir şekilde Tuple tipine dönüştürüldükten sonra kullanılabilmiştir.

[![blg200_Runtime3](/assets/images/2010/blg200_Runtime3_thumb.gif)](/assets/images/2010/blg200_Runtime3.gif)

Peki Tuple Tipi için Eleman Sayısı Sınırı var mıdır?

Bu örnekler ile Tuple kullanımının nasıl yapılabildiğini ve hangi durumlarda değerlendirilebildiğini kısaca incelemeye çalıştık. Son olarak Tuple tipinin kaç iç tip ile çalışabildiğine bakıyor olacağız. Normal şartlar altında aşırı yüklenmiş static Create veya yapıcı metodlarına (Constructors) baktığımızda en fazla 8 elemanın söz konusu olduğunu görürüz.

static Create metodu için;

```csharp
public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>> Create<T1, T2, T3, T4, T5, T6, T7, T8>( 
    T1 item1, 
    T2 item2, 
    T3 item3, 
    T4 item4, 
    T5 item5, 
    T6 item6, 
    T7 item7, 
    T8 item8 
)
```

yapıcı metodu için;

```csharp
public Tuple( 
    T1 item1, 
    T2 item2, 
    T3 item3, 
    T4 item4, 
    T5 item5, 
    T6 item6, 
    T7 item7, 
    TRest rest 
)
```

Dolayısıya bu notkada sadece 8 elemanlı bir Tuple oluşturabileceğimizi zannedebiliriz. Ancak şöyle bir kural da yoktur; Tuple içerisindeki item değişkenleri Tuple olamaz

![Wink](/assets/images/2010/smiley-wink.gif)

Yani küçük bir hile yapıyor olacağız. Aşağıda görülen örnek kod parçasını ele alalım.

```csharp
var nTuple = Tuple.Create( 
                1 
                , "Burak" 
                , true 
                , Tuple.Create(1, 2, 4, 6, 0) 
                , new Person 
                { 
                    PersonId = 1 
                    , Name = "Burak" 
                    , Surname = "Şenyurt" 
                    , City = "İstanbul" 
                    , Salary = 1000 } 
                , 90.45F 
                , 91023 
                ,'A' 
                );
```

nTuple isimli değişken ilk etapta 8 elemanlı görülmektedir. Ancak 4ncü elemanın kendisi de bir Tuple nesne örneğidir ve 5 eleman içermektedir. Dolayısıyla bu tip bir kullanım sayesinde 8 eleman zorunluluğu aşılabilir. Tabi bu durumda 4ncü elemana ulaşıldığında kendi altındaki elemanlarında yine Item[indis] isimlendirmesi ile sunulduğu görülebilir.

[![blg200_DebugTime](/assets/images/2010/blg200_DebugTime_thumb.gif)](/assets/images/2010/blg200_DebugTime.gif)

Biraz önceki örnekte Create metodu değerlendirilmektedir. Eğer Constructor metod için benzer bir durum ele alınmak istenirse en sonda yer alan Rest isimli parametrenin kullanılması tercih edilir. Aşağıdaki kod parçasında bu durumda ele alınmaktadır.

```csharp
var nTupleV2 = new Tuple<byte, string, bool, Person, float, int, char, Tuple<int,int, int, int, int>> 
            ( 
            1 
            , "Burak" 
            , true                
            , new Person 
            { 
                PersonId = 1, 
                Name = "Burak", 
                Surname = "Şenyurt", 
                City = "İstanbul", 
                Salary = 1000 
            } 
            , 90.45F 
            , 91023 
            , 'A' 
            , Tuple.Create(1, 2, 4, 6, 0) 
            );
```

Burada dikkat edileceği üzere son parametrede yine bir Tuple nesnesi örneklenmiştir. Bu durumda Rest isimli özellik üzerinden, en sonda eklenen Tuple nesne örneği içeriği yakalanabilir.

[![blg200_DebugTime2](/assets/images/2010/blg200_DebugTime2_thumb.gif)](/assets/images/2010/blg200_DebugTime2.gif)

Sanıyorum ki Tuple tipi ve kullanım alanları hakkında az da olsa fikir sahibi olduk..Net Framework 4.0 ile gelen yenilikleri incelemeye devam ediyor olacğaız. Bir sonraki yazımızda görüşünceye dek hepinize mutlu günler dilerim.

[TupleKavrami.rar (25,36 kb)](/assets/files/2010/TupleKavrami.rar) [Örnek Visual Studio 2010 Ultimate sürümünde geliştirilmiş ve test edilmiştir]