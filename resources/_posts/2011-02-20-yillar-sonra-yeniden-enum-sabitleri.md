---
layout: post
title: "Yıllar Sonra Yeniden Enum Sabitleri"
date: 2011-02-20 16:05:00 +0300
categories:
  - csharp
tags:
  - csharp
  - dotnet
  - aspnet
  - http
  - delegates
  - generics
---
Aslında çok şanslı bir insanım. Çünkü çocukluğumdan beri tatile gidebiliyorum. Yazlığımızın olduğu [Avşa](http://www.avsa.com.tr/) adasına her sene iki günlüğüne dahi olsa gitmekteyim. İlk yıllarda adaya hareket eden nostaljik gemiler vardı. Bu gemiler yaklaşık olarak altı saat gibi bir sürede İstanbul’ dan adaya ulaşmaktaydı.

[![blg216_Giris](/assets/images/2011/blg216_Giris_thumb.jpg)](/assets/images/2011/blg216_Giris.jpg)


İlerleyen yıllarda hızlandıkça deniz yüzeyinin üstünde yükselen kanatlı gemilerle karşılaştık. Ne yazık ki bu hızlı gemiler daha çok dalgasız göller için yapıldıklarından her tür havada gidemiyorlardı.

Derken bu günlerde çok meşhur olan Mavi Marmara gemisinin seferlere başladığını gördük. Ancak bunun çok öncesinde deniz otobüsü seferleri de başlamıştı. Deniz otobüsleri ile adaya 3 saat gibi bir sürede gidilebiliyordu. Tabi kötü hava koşullarında (özellikle fırtınalı günlerde), hırçın Marmara Denizinde bu gemilerle seyahet etmekte ayrı bir maceraydı. Çok sık başıma gelen bir durum du. Hemen her yaz, en az bir veya iki kere fırtınaya yakalanıyordum.

[![Exclamation](/assets/images/2011/Exclamation_thumb_7.gif)](/assets/images/2011/Exclamation_7.gif)

Tabi Avşa adasına ulaşmanın tek yolu gemi ve deniz otobüsü değil. Tekirdağ ve Silivri üzerinden de araba taşıyan motorlar ile de gidebilirsiniz. Üstelik bu motorların ağırlık merkezleri ve tasarımları nedeniyle, katamaran tipinden olan deniz otobüslerine göre daha az salladıklarını ifade edebilirim. Bir diğer yolda Yenikapı’ dan hızlı feribot ile Bandırma’ ya geçmek, oradan yarım saatlik bir yol ile Erdek limanına gelmek ve yine araba taşıyan gemilerle Avşa adasına ulaşmaktır.

Yine böyle fırtınalı bir gündü ve ben Bizitek firmasında henüz Junior geliştiriciydim. Şirketimin bana tahsis ettiği IBM R-51 diz üstü bilgisayarımı kullanmaktaydım. Avşa’ dan döndüğüm bir pazar günü yine çok fırtınalı bir havaya denk geldim. Kusanlar, bağırıp çağıranlar, panikleyenler…Açıkçası benim de çok hoşuma giden bir durum değil di (Zaten deniz otobüsüne bindiğinizde kaptanınız “Açıkta Deniz vardır!Rahatsız olanların binmemesi önemle rica olunur!” derse, ve bunu sinirli bir şekilde söylerse durum vahimdir) Derken koltuğunda dimdik oturup, sağ sola bakmadan ince bir kitap okuyan bir yolcu ile göz göze geldim. Bana “Sürükleyici bir kitap…Fırtınayı unutturuyor…” dedi. Bu sözden etkilenmiştim. Ne yaptım dersiniz? Laptop’ umu açtım ve [Numaralandırıcıları Kullanmak İçin Bir Sebep](https://www.buraksenyurt.com/post/Numaraland%C4%B1r%C4%B1c%C4%B1lar%C4%B1-Kullanmak-Icin-Bir-Sebep-bsenyurt-com-dan) başlıklı makalemi yazdım.

Bu makalemde Asp.Net tabanlı bir web uygulamasında DataGrid ve Enum sabitlerinin kullanımına değinmekteydim. Bu aslında,.Net Framework’ ün 5 temel tipinden birisi olan Enum sabitleri ile (Diğerleri Class, Struct, Interface, Delegate tipleridir) belki de en önemli haşır neşir oluşumdu. Derken bir sene sonrasında bu kez Netron firmasında eğitmen olarak görev yapmaktayken [C# Temelleri: Enum Sabitinin Bilinmeyen Yönleri](https://www.buraksenyurt.com/post/C-Temelleri-Enum-Sabitinin-Bilinmeyen-Yonleri-bsenyurt-com-dan.aspx) isimli makalemi yayınladım. Bu sefer Enum sabitlerini daha detaylı inceliyor ve özellikle Enum tipi üzerinden erişilen üyeler yardımıyla nasıl işlemler yapılabileceğini ele alıyordum.

Sene oldu 2010. Hatta 2010 yılının ikinci yarısının başındayız. Enum sabitleri ile uğraşmayalı da hayli zaman olmuş aslında. İşte bu yazımızda Enum sabitlerinin başımıza dert olabileceği bir kaç vakayı ele almaya çalışıyor olacağız. Öyleyse hızlı bir başlangıç yapalım ve aşağıdaki kod parçasını göz önüne alalım.

```csharp
using System;

namespace HelloAgainEnums 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            for (int i = 0; i < 5; i++) 
            { 
                Test(); 
            } 
        }

        private static void Test() 
        { 
            Console.WriteLine("StockLevel değerini giriniz"); 
            int userInput = Int32.Parse(Console.ReadLine()); 
            StockLevel sLevel = (StockLevel)userInput; 
            Console.WriteLine(sLevel); 
        } 
    }

    enum StockLevel 
    { 
        Empty, 
        Low, 
        High 
    } 
}
```

Örnek kod parçasında StockLevel isimli bir enum sabiti kullanılmaktadır. Bu sabitin Empty, Low ve High isimli 3 değeri bulunmaktadır. Bilindiği üzere enum sabiti değerleri varsayılan olarak 0 sayısalından başlamaktadır. Kodun kritik olan kısmı Test metodunun içeriğidir. Dikkat edileceği üzere kullanıcıdan sayısal bir değer girilmesi istenmektedir. Bu değer userInput isimli int tipinden değişken içerisine alınmaktadır. Önemli olan nokta userInput değişkeninin bilinçli (Explicitly) olarak StockLevel tipine dönüştürülmesidir. Bu mümkündür. Derleme (Compile) veya Çalışma Zamanında (Runtime) herhangibir hata oluşmayacaktır. Mı acaba? Gelin örnek bir çalışma zamanı çıktısına bakalım.

[![blg216_Case1Runtime1](/assets/images/2011/blg216_Case1Runtime1_thumb.gif)](/assets/images/2011/blg216_Case1Runtime1.gif)

İlk üç denemede 2, 1 ve 0 değerleri girilmiştir. Ancak son iki denemede aslında enum sabiti içerisinde olmayan değerlerin girildiği görülmektedir. Kod hata vermemiştir. Dönüştürme işlemi başarılı olmuştur. Ancak kavramsal olarak bir hata vardır. Enum sabitine ait olmayan bir takım değerler ile çalışılmaktadır. Çok tabi olarak enum sabitlerini kullanan pek çok geliştirici bu detayı göz ardı etmiş olabilir. Peki ya çözüm nedir?. Test metodunu aşağıdaki gibi değiştirdiğimizi düşünelim.

```csharp
private static void Test() 
{ 
	Console.WriteLine("StockLevel değerini giriniz"); 
	int userInput = Int32.Parse(Console.ReadLine()); 
	if (Enum.IsDefined(typeof(StockLevel), userInput)) 
	{ 
		StockLevel sLevel = (StockLevel)userInput; 
		Console.WriteLine(sLevel); 
	} 
	else 
	{ 
		Console.WriteLine("Girilen sayısal değer StockLevel enum sabiti içerisinde yer almamaktadır."); 
	} 
}
```

Enum sınıfı üzerinden erişilebilen IsDefined metodu ilk parametre olarak enum sabiti tipini almaktadır. Bu nedenle typeof operatörü kullanılmıştır. İkinci parametre object tipindendir ve kullanıcının girdiği int değeri taşımaktadır. Metod, StockLevel enum sabiti içerisinde userInput ile girilen değerin var olup olmadığına bakmakta ve buna göre true veya false değer döndürmektedir. Tabiki if kontrolü içerisinde bilinçli olarak dönüştürme işlemi yapılması şart değildir. Enum sınıfının Parse metodu kullanılabilir. Hatta TryParse metodundan yararlanılarak, dönüştürme başarılı olduğu takdirde değer atanması yapılması da sağlanabilir.

[![Question](/assets/images/2011/Question_thumb_1.gif)](/assets/images/2011/Question_1.gif) Aslında Enum sınıfının Parse ve TryParse kullanımları ile IsDefined çağrımına gereksinimin ortadan kalkabileceğini ifade edebilir miyiz? Bunu bir düşünün ve araştırın.

Gelelim diğer bir vakaya. Normal şartlarda Enum sabitleri içerisine yazılan değerler için sayısal atamada bulunulmaz. Varsayılan değerler ne ise bunlar kullanılmaktadır. Buna göre ilk sabit değeri 0 ile başlar ve diğerleri de otomatik olarak artarak devam eder. Ne varki 0 değerinin kullanımı ile ilişkili bir durum vardır. Aşağıdaki kod örneğini göz önüne alalım.

```csharp
using System;

namespace HelloAgainEnums 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            Product prd = new Product(); 
            Console.WriteLine(prd.ToString()); 
        } 
    }

    class Product 
    { 
        public int ProductId { get; set; } 
        public string Name { get; set; } 
        public decimal ListPrice { get; set; } 
        public bool InStock { get; set; }

        public override string ToString() 
        { 
            return String.Format("{0} {1}  ({2}) {3}", ProductId, Name, ListPrice.ToString("C2"),InStock); 
        } 
    } 
}
```

Bu kod parçasında Product sınıfına ait bir nesne örneğinin varsayılan yapıcı metod (Default Constructor) ile üretildiği ve ezilmiş (Override) ToString fonksiyonu ile de içeriğinin ekrana yazdırıldığı görülmektedir. Temelleri hatırlayacak olursak, bu tip bir durumda sınıf üyelerine varsayılan değerler atanacaktır. Sayısal değerler için 0, kayan noktalı sayısal değerler için 0.0, bool değerler için false, referans tipleri içinse null. Dolayısıyla çalışma zamanı çıktısı aşağıdaki gibi olacaktır.

[![blg216_ClassDefaultValues](/assets/images/2011/blg216_ClassDefaultValues_thumb.gif)](/assets/images/2011/blg216_ClassDefaultValues.gif)

Peki ya aşağıdaki gibi bir kod söz konusu olduğunda.

```csharp
using System;

namespace HelloAgainEnums 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            Product prd = new Product(); 
            Console.WriteLine(prd.ToString()); 
        } 
    }

    class Product 
    { 
        public int ProductId { get; set; } 
        public string Name { get; set; } 
        public decimal ListPrice { get; set; } 
        public bool InStock { get; set; } 
        public StockLevel Level { get; set; }

        public override string ToString() 
        { 
            return String.Format("{0} {1}  ({2}) {3} {4}", ProductId, Name, ListPrice.ToString("C2"),InStock,Level); 
        } 
    }

    enum StockLevel 
    { 
        Empty=10, 
        Low=12, 
        High=18 
    } 
}
```

Bu sefer StockLevel isimli Enum sabitinde varsayılan değerler kullanılmamıştır. Bunun yerine 10, 12 ve 18 değerleri verilmiştir. Diğer yandan Product isimli sınıfta StockLevel tipinden Level özelliğinin (Property) bulunduğu gözden kaçmamalıdır. Az önceki kod parçasında sınıfların üyelerinin, aksine bir işlem yapılmadıkça varsayılan değerlere atandığını görmüştük. Bu durumda Level özelliğinin değeri aşağıdaki ekran çıktısındaki gibi olacaktır.

[![blg216_Case2Runtime](/assets/images/2011/blg216_Case2Runtime_thumb.gif)](/assets/images/2011/blg216_Case2Runtime.gif)

İşte istemediğimiz bir durum. Bu nedenle 0 varsayılan değerinin enum sabitleri içerisinde mutlaka kullanılması gerektiğini ifade edebiliriz. Tabi çözümlerden birisi, enum içerisinde Unknown gibi 0 değerine set edilmiş bir sabit kullanmaktır.

Yazımızda Enum sabitleri ile alakalı iki önemli vakayı incelemeye çalıştık. Şimdi ilk vakamıza tekrar dönüş yapıyor olacağız ve konuyu farklı bir açıdan değerlendirmeye çalışacağız. Enum sabitleri ile int tipleri arasındaki dönüşümlerde cast operatörlerinin kullanılması aslında birbirlerine kuvvetle bağlı yapılar oldukları için sorunlara neden olabilmektedir. Söz gelimi enum sabitlerinin int karşılıklarının bir veri kaynağında tutulduğu durumlarda, enum sabiti içerisinde veya veri kaynağındaki sayısal değerlerde yapılacak değişikliker, istenmeyen sonuçlara neden olabilmektedir. Bu nedenle belki de enum sabitleri ile int değerler arasındaki dönüştürmelerde farklı bir çözüm yoluna gidilmelidir. Hatta olayı sadece int ile enum sabitleri arasında bir dönüştürme olarak düşünmeyebiliriz de. Söz gelimi sayı ve harflerden oluşan string bir ifadenin de, bir enum sabiti değerine karşılık gelmesini isteyebiliriz. Durumu daha net bir şekilde kavrayabilmek adına dilerseniz aşağıdaki kod içeriğini bir göz önüne alalım.

[![blg216_Case3ClassDiagram](/assets/images/2011/blg216_Case3ClassDiagram_thumb.gif)](/assets/images/2011/blg216_Case3ClassDiagram.gif)

```csharp
using System; 
using System.Collections.Generic;

namespace ClientApp 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            for (int i = 0; i < 4; i++) 
            { 
                Test(); 
            } 
        } 
        static void Test() 
        { 
            Console.WriteLine("Kategori kodunu giriniz"); 
            string categoryCode = Console.ReadLine(); 
            Category ctgry = CategoryInterpretter.GetCategory(categoryCode); 
            Console.WriteLine(ctgry.ToString()); 
        } 
    }

    public static class CategoryInterpretter 
    { 
        private static readonly Dictionary<string, Category> _categories =new Dictionary<string,Category> 
        { 
            {"ALF-1",Category.Book}, 
            {"ALF-2",Category.Electronic}, 
            {"ALF-3",Category.Computer}, 
            {"ALF-4",Category.Software}, 
            {"ALF-5",Category.Toys}, 
            {"ALF-6",Category.Hardware} 
        }; 
        public static Category GetCategory(string code) 
        { 
            Category result; 
            if (_categories.TryGetValue(code, out result)) 
                result = _categories[code]; 
            else 
                result = Category.Unknown; 
            return result; 
        } 
    }

    public enum Category 
    { 
        Unknown, 
        Electronic, 
        Computer, 
        Software, 
        Hardware, 
        Book, 
        Toys, 
    } 
}
```

Şimdi örnek uygulamamızda neler yaptığımıza bir bakalım. CategoryInterpretter isimli static sınıf içerisinde değişmez olarak tanımlanmış private erişim belirleyicisine sahip bir Dictionary koleksiyonu bulunmaktadır. Bu koleksiyonunun key verileri string olup, Category enum tipi içerisindeki birer sabite karşılık gelmektedir. Bunun dışında GetCategory isimli static metod, parametre olarak gelen string bilginin karşılığını enum sabiti içerisinden getirmek üzere oluşturulmuştur. Bu sayede ALF-1=Category.Book gibi bir eşleştirme yapılabilmesi kavramsal olarak mümkündür. Uygulamanın çalışma zamanı çıktısına baktığımızda aşağıdaki örnek sonuçlar ile karşılaşırız.

[![blg216_Case3Runtime](/assets/images/2011/blg216_Case3Runtime_thumb.gif)](/assets/images/2011/blg216_Case3Runtime.gif)

Görüldüğü üzere ilk 3 deneme de başarılı çevirmeler yapılmıştır. Son denemede girilen string bilgi (AADAD), Dictionary tipi içerisinde var olmadığından, varsayılan olarak Category.Unknown değeri elde edilmiştir. Bu GetCategory metodu içerisinde alınmış bir tedbirdir. Aslında TryGetValue metodunun kullanılmaması halinde çalışma zamanına KeyNotFoundException tipinden bir istisna fırlatılacaktır ki bu da hatalı veri girişlerine karşılık bir çözüm olarak düşünülebilir.

[![blg216_Case3Exception](/assets/images/2011/blg216_Case3Exception_thumb.gif)](/assets/images/2011/blg216_Case3Exception.gif)

Böylece geldik bir yazımızın daha sonuna. Bu yazımızda yıllar sonrasında Enum sabiti kavramına tekrardan dönüş yaparak farklı açılardan ele almaya çalıştık. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[HelloAgainEnums.rar (44,39 kb)](/assets/files/2011/HelloAgainEnums.rar)