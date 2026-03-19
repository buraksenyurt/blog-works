---
title: "C# Temelleri: 1!=1 Eşitsizliğinden GetHashCode' a Uzun İnce Bir Yol"
pubDate: 2006-10-11 03:00:00
categories:
  - C#
tags:
  - C#
  - .NET
---

# C# Temelleri: 1!=1 Eşitsizliğinden GetHashCode' a Uzun İnce Bir Yol
Değerli Okurlarım Merhabalar,

Eminimki makale başlığı size oldukça ilginç gelmiştir. Matematiksel olarak mümkün olmayan bu durum.Net programlama ortamında acaba gerçekleştirilebilir mi? Elbetteki matematiksel olarak imkansız olan bu durumun programlama ortamımız içerisinde gerçekleşebilir olmasıda pek mantıklı değil. O halde makalemizin asıl konusundan bahsedelim ve durumu açıklığa kavuşturalım.

Bu makalemizde.Net içerisinde yer alan veri tiplerinin (data types) nasıl ve ne şekilde karşılaştırılabileceklerini incelemeye çalışacağız. Bir başka deyişle nesnelerin birbirleri arasındaki eşitliklerini mercek altına alacağız..Net içerisinde nesnelerin içeriklerini karşılaştırmak adına pek çok yöntem bulunmaktadır. Bu anlamda makalemizin çekirdeğini Object sınıfının static ReferneceEquals, Equals metodları ile sanal (virtual- yani ezilebilir) Equals metodu oluşturmaktadır. Sonrasında bu metodlara Object sınıfının GetHashCode metodunun ezilmesini de ekleyeceğiz. Nitekim bazı koşullar bizi GetHashCode'a doğru götürecektir.

Bildiğiniz gibi.Net içerisinde iki temel veri türü yer almaktadır. Bunlar Referans türleri (Reference Types) ve Değer türleridir (Value Types). Referans ve Değer türlerinin bellek üzerindeki farklı tutuluş şekilleri, bunlara ait nesne örneklerininde farklı şekillerde karşılaştırılabileceği sonucunu doğurmaktadır. Dolayısıyla iki referans tipinin eşitliği veya iki değer tipinin eşitliği söz konusu olduğunda göz önüne alınması gereken modeller vardır..Net içerisinde iki çeşit eşitlik teorisi vardır. Referans tabanlı ve değer tabanlı.

![mk177_1.gif](images/mk177_1.gif)

Referans tabanlı eşitlik modelinde, nesnelerin birbirlerine eşit olması için, adreslerinin eşit olması yeterlidir. Bu çoğunlukla nesne kimliğinin (object identity) eşit olması olarakta belirtilir. Değer tabanlı eşitlik modelinde ise nesnelerin sadece kimlikleri değil, içerikleride (contents) ele alınmaktadır..Net içerisinde yer alan tipler bu iki eşitlik modelinden birisini tercih etmektedir.

Örneğin string sınıfı referans tipi olmasına rağmen değer tabanlı eşitlik modelini (value semantics equality) tercih ederken, DataRowView sınıfı referans tabanlı eşitlik modelini (reference semantics equality) tercih etmektedir. Dolayısıyla iki string değişken birbiriyle karşılaştırılırken sadece nesne kimlikleri (object identity) değil, nesne içerikleride (contents) değerlendirilmektedir. DataRowView sınıfına ait iki nesne örneğinde ise söz konusu olan bu nesnelerin bellekte gösterdikleri adreslerin eşit olup olmadığıdır. Bu iki farklı eşitlik modeli, kendi yazdığımız tipler içinde bir takım ön hazırlıkları gerektirebilir. Örneğin bazı durumlarda yazdığımız referans tipleri için değer tabanlı eşitlik modelini kullanmak isteyebiliriz. Yada bunun tam tersi bir durum söz konusu olabilir.

![dikkat.gif](images/dikkat.gif)
Yaygın Kurallar:
1 - İki referans tipi değişken aynı veriyi işaret ediyorlarsa eşittirler.
2 - İki değer türü değişken aynı tipteyseler ve içerikleri aynı ise eşittirler.

Bu yaygın kurallar elbetteki geliştirici tarafından bozulabilir.

Object sınıfının bazı üye metodları ile yukarıda bahsettiğimiz eşitlik teorileri arasında sıkı bir ilişki vardır. Object sınıfı nesnelerin eşit olup olmadıklarını yukarıda bahsedilen modellere göre kontrol etmemizi sağlayan bir takım üyeler içerir. Bu üyelerden static olan ReferenceEquals ve Equals metodlarının davranışları değiştirilemez. Ancak kendi tiplerimizin eşitlik modellerini değiştirebilmemiz için Object sınıfı sanal (virtual) Equals metodunu içermektedir. Şimdi kod yazarak eşitlik modellerini biraz daha detaylandırmaya çalışalım. Aşağıdaki örnek kod parçası makale başlığımızı birazda aldatmaca ile gerçeklemektedir.

```csharp
double deger_1 = 1;
double deger_2 = 1;
if (Object.ReferenceEquals(deger_1, deger_2))
    Console.WriteLine("Eşitler");
else
    Console.WriteLine("Eşit değiller");
```

![mk177_2.gif](images/mk177_2.gif)

Dikkat ederseniz deger_1 ve deger_2 double tipinden iki farklı değişken olmalarına rağmen içerikleri aynıdır. Bu işin şaka tarafı olmakla birlikte böyle bir sonuç alınması son derece doğaldır. Nitekim referans tabanlı eşitlik modelini kullanan ReferenceEquals metodunu kullanıyoruz. Ancak aynı kodu Equals metodu yardımıyla çalıştırırsak Eşitler sonucunu alacağımız kesindir ki buda içimize su serpip 1==1 matematik sonucunu doğrulamaktadır.

```csharp
double deger_1 = 1;
double deger_2 = 1;
if (Object.Equals(deger_1, deger_2))
    Console.WriteLine("Eşitler");
else
    Console.WriteLine("Eşit değiller");
```

![mk177_3.gif](images/mk177_3.gif)

Equals metodunun bu sonucu vermesinin en büyük nedeni içeride yaptığı karşılaştırma işlemleridir. Equals metodu ilk olarak parametre olarak aldığı nesne örneklerinden herhangibirinin null olup olmadığına bakar. Böyle bir durum varsa zaten eşitlikten söz edilemez. (Tabiki her iki örnekte null içeriğine sahipse bu durumda nesne örneklerinin eşit olduğu kabul edilir.) Sonrasında ise == operatörü ile bu nesne örneklerinin kimliklerinin bir başka deyişle bellekte işaret ettikleri veri adreslerinin aynı olup olmadığına bakar. Buda geçerli değilse son olarak gelen nesne örneklerinden ilkinin ezilmiş olan Equals metodunu kullanarak içerik (content) kontrolü yapar. İçerik aynı ise gelen nesne örnekleri eşittir.

![dikkat.gif](images/dikkat.gif)
Object sınıfının static ReferenceEquals metodu aynı içeriğe sahip farklı değer türü değişkenleri için her zaman false döndürecektir.

Bu teoriyi yukarıdaki double tipi kısmen karşılamaktadır. Peki kendi değer türlerimiz söz konusu olduğunda da ReferenceEquals aynı davranışımı sergileyecektir? Hatta, Object sınıfının static Equals metodu nasıl bir davranış sergileyecektir? Aşağıdaki kod parçasında bu durum irdelenmeye çalışılmaktadır.

```csharp
public struct Personel
{
    private double m_Maas;
    private int m_TcNo;
    private string m_Ad;

    public Personel(int tcNo, string ad, double maas)
    {
        m_TcNo = tcNo;
        m_Ad = ad;
        m_Maas = maas;
    }
}

class Program
{
    static void Main(string[] args)
    {
        Personel prs1 = new Personel(1900, "Burak", 20);
        Personel prs2 = new Personel(1900, "Burak", 20);
        
        Console.Write("Reference Equals ile ");
        if (Object.ReferenceEquals(prs1, prs2))
            Console.WriteLine(" Eşitler");
        else
            Console.WriteLine(" Eşit değiller");

        Console.Write("Equals ile ");
        if (Object.Equals(prs1, prs2))
            Console.WriteLine(" Eşitler");
        else
            Console.WriteLine(" Eşit değiller");
    }
}
```

![mk177_4.gif](images/mk177_4.gif)

Görüldüğü gibi ReferenceEquals metodu kendi değer tiplerimiz içinde aynı davranışı sergilemektedir. Örneğimizde geliştirdiğimiz Personel yapısına ait nesne örnekleri aynı içeriğe sahip olmalarına rağmen bellek üzerinde farklı adreslerde yer almaktadırlar ve bu nedenle nesne kimlikleri aynı değildir. Dolayısıyla ReferenceEquals metodunun tam olarak referans tabanlı eşitlik modelini benimsediğini ve ezilebilir bir versiyonu olmadığı için bu davranışının değiştirilemeyeceğini düşünebiliriz. Tam tersine Equals metodu, ilgili yapı değişkenlerinin içeriklerini kıyaslamış ve aynı olduklarına kanaat getirerek geriye true değer döndürmüştür.

Peki referans türlerinde durum nedir? Örneğin aynı içeriğe sahip iki string'i yada kendi yazdığımız iki sınıfa ait nesne örneklerinden aynı içeriğe sahip olanların eşitliklerini ReferenceEquals ile ve Equals metodu ile test ettiğimizde sonuçlar ne olacaktır? Aşağıdaki kod parçaları bu durumu analiz etmektedir. İlk olarak kendi yazdığımız Urun isimli bir sınıf için ReferenceEquals ve Equals davranışlarına bakalım.

```csharp
public class Urun
{
    private int m_Id;
    private string m_Ad;
    private double m_Fiyat;

    public Urun(int id, string ad, double fiyat)
    {
        m_Id = id;
        m_Ad = ad;
        m_Fiyat = fiyat;
    }
}

class Program
{
    static void Main(string[] args)
    {
        Urun urn1 = new Urun(1000, "Balata", 10);
        Urun urn2 = new Urun(1000, "Balata", 10);

        Console.Write("Reference Equals ile");
        if (Object.ReferenceEquals(urn1, urn2))
            Console.WriteLine(" Eşitler");
        else
            Console.WriteLine(" Eşit değiller");

        Console.Write("Equals ile ");
        if (Object.Equals(urn1, urn2))
            Console.WriteLine(" Eşitler");
        else
            Console.WriteLine(" Eşit değiller");
    }
}
```

![mk177_5.gif](images/mk177_5.gif)

Yukarıdaki kod parçasında kendi yazdığımız Urun isimli referans tipine ait iki ayrı örnek yer almaktadır. Lakin bu iki örneğin içerikleri aynıdır. Ancak adresler farklı olduğu için ReferenceEquals metodu karşılaştırmasının sonucu false olarak dönmektedir ki böyle olması doğaldır. Diğer taraftan Object sınıfının static Equals metodu nesne içerikleri aynı olmasına rağmen yine false cevabını vermektedir. Oysaki Personel yapısını kullandığımız örnekte sonuç eşit oldukları yönündedir.

Böyle bir sonuç almamızın en büyük nedeni kendi yazdığımız Urun sınıfı için Equals metodu yazmamış oluşumuzdur. Bir başka deyişle static Equals metodunun kendi içerisindeki son kontrol sırasında yaptığı işi üstlenecek ezilmiş (override) bir Equals metodu, Urun sınıfı içerisinde yer almamaktadır. Öyleyse sonucun bu şekilde olması son derece doğaldır. Dolayısıyla kendi sınıflarımız için Object sınıfının sanal Equals metodunu ezmezsek, Object sınıfının Static olan Equals metodu söz konusu tip için içerik kontrolüde gerçekleştiremeyecektir. Bir başka deyişle Object sınıfının static Equals metodu referans tipli eşitlik modelini gerçeklemiştir. Bunu test etmek için makalemizin ilerleyen kısımlarında Urun sınıfı içerisinde Equals metodunu ezeceğiz.

Elbette gözden kaçırılmaması gereken bir durum vardır. Urun sınıfına ait nesne örneklerinden urn1 ve urn2' yi birbirlerine atadıktan sonra Equals ve ReferenceEquals metodlarını test edersek aşağıdaki ekran görüntüsündeki sonucu alırız.

![mk177_7.gif](images/mk177_7.gif)

Dikkat ederseniz her iki metodda true değerini döndürmektedir. Bu son derece doğaldır nitekim atama sonrası referans tiplerinin aynı adresi işaret etmeleri sağlanmıştır. Aynı adresler işaret edildiği için, içeriklerde aynı kabul edilecektir. Buda metodların true değer döndürmesini açıklamaktadır. (Atama sonrası oluşan durumu değer türleri için düşündüğümüzde ise; Personel yapımızı kullandığımız örnekte, prs1 nesne örneğini prs2 nesne örneğine atadığımız takdirde ReferenceEquals metodu false değer, Equals metodu ise true değer döndürecektir.)

Gelelim.Net içerisinde yer alan önceden tanımlı referans tiplerinden birisi olan string sınıfına. Aşağıdaki kod parçası string sınıfı için geçerli durumu analiz etmektedir.

```csharp
string str1 = "Burak Selim Şenyurt";
string str2 = "Burak Selim Şenyurt";

Console.Write("Reference Equals ile");
if (Object.ReferenceEquals(str1, str2))
    Console.WriteLine(" Eşitler");
else
    Console.WriteLine(" Eşit değiller");

Console.Write("Equals ile");
if (Object.Equals(str1, str2))
    Console.WriteLine(" Eşitler");
else
    Console.WriteLine(" Eşit değiller");
```

![mk177_6.gif](images/mk177_6.gif)

Dikkat ederseniz string tipinden değişkenler söz konusu olduğunda ReferenceEquals metoduda, Equals metoduda true değerini döndürmektedir. Hatta ilginç olan nokta str1 ve str2 değişkenlerinin birbirlerine atanmamış dolayısıyla referansları eşitlenmemiş olmalarına rağmen static ReferenceEquals metodu true değeri döndürmüştür. Oysaki bizim yazdığımız Urun referans tipi için her ikiside false olarak dönmektedir. String değişkenlerimiz için Equals metodunun true değer döndürmesinin en büyük nedenlerinden birisi String sınıfının kendi içerisinde Equals metodunu yazmış olması ve kullanmasıdır. Bu örneklerden şu sonuca varabiliriz.

![dikkat.gif](images/dikkat.gif)
Kendi sınıflarımızı yazdığımız takdirde içeriklere göre eşitlikleri kontrol etmek istiyorsak (yani değer tabanlı eşitlik modelini uygulamak istiyorsak) Object sınıfının sanal olan Equals metodunu ezmemiz (override) gerekecektir.

Gelin, kendi yazdığımız Urun sınıfı içerisinde Equals metodunu ezelim ve Object sınıfının static Equals metodunun nasıl bir davranış sergileyeceğine bakalım. Bu amaçla, Urun sınıfımız içerisinde Equals metodunu aşağıdaki gibi ezelim.

```csharp
public override bool Equals(object obj)
{
    Urun urn = (Urun)obj;
    if ((urn.m_Id == this.m_Id) && (urn.m_Ad == this.m_Ad) && (urn.m_Fiyat == this.m_Fiyat))
        return true;
    else
        return false;
}
```

Equals metodu dikkat ederseniz object tipinden bir parametre almaktadır. Bu parametreyi metod içerisinde Urun tipine çeviriyoruz. Sınıf içerisindeki alan değerleri ile gelen referansın değerlerini karşılaştırıyoruz. Eğet tüm değerleri birbirlerine eşitlerse geriye true değerini döndürerek referansların eşit olduğuna karar veriyoruz. Ezmiş olduğumuz Equals metodu sonrasında kodu yeniden çalıştırırsak artık Object sınıfının Equals metodunun true değer döndürdüğünü görürüz.

Object sınıfı içerisinde yer alan sanal (virtual) Equals metodunun ezilmesiyle artık Urun sınıfına ait nesne örneği üzerindende de Equals metodunu çağırabiliriz..Net içerisinde yer alan pek çok tip kendi içerisinde Object sınıfından gelen Equals metodunu ezmiş ve kendine göre düzenlemiştir. Var olan tüm framework nesnelerinin Object sınıfından türediği düşünüldüğü takdirde, kendi tiplerimiz üzerinden yada var olan tipler üzerinden Equals metodu çağırılabilir. Örneğin bir double tipi üzerinden Equals metodunu çağırılabileceğimiz gibi kendi yazdığımız bir sınıf üzerinden de (eğer Equals metodunu ezdiysek) çağırabiliriz. Bu durumu analiz etmek için console uygulamamızda aşağıdaki kod parçasını deneyebiliriz.

```csharp
if (urn1.Equals(urn2))
    Console.WriteLine(" Eşitler");
else
    Console.WriteLine(" Eşit değiller");
```

Urun sınıfımız içerisinde Equals metodunu override ettiğimiz için true değeri dönecektir ve ekrana Eşitler yazacaktır. Ancak Equals metodunu kendi sınıfımız içerisinde ezmemiş olsaydık, metod çağrısı sonucu false değer dönecek ve ekrana eşit değiller yazacaktı. Tam bu nokta Equals metodunu kendi tipimiz için ezdiğimizde derleme zamanında bir uyarı aldığımızı görürürüz. Uyarıda, sınıfımızın Object'ten gelen GetHashCode metodunu ezmesi (override) önerilmektedir.

![mk177_9.gif](images/mk177_9.gif)

Peki bu uyarı niçin verilmektedir? Neden Object sınıfının GetHashCode metodunun ezilmediğine dair bir uyarı alıyoruz? Bunu anlayabilmek için öncelikli olarak Object sınıfının GetHashCode metodunun görevinden bahsetmemiz gerekmektedir. GetHashCode metodu geriye hash algoritmasına göre üretilmiş sayısal bir değer döndürür. Bu sayısal değer aslında Hashtable ve Dictionary<> gibi koleksiyonlarda önemli bir yere sahiptir.

![dikkat.gif](images/dikkat.gif)
Hashtable ve Dictionary<> gibi koleksiyonlar kendi içlerinde tuttukları satırları hash değerlerine göre sıralamaktadırlar. Hash algoritmasının gücü sayesinde bu koleksiyonlar diğerlerine göre daha hızlı çalışabilmektedir.

Dilerseniz ilk olarak GetHashCode metodu tarafından üretilen hash değerlerini mercek altına almaya çalışalım. Aşağıdaki kod parçasında içerikleri aynı olan tipler için hash değerleri çekilmektedir.

```csharp
string str1 = "Burak Selim Şenyurt";
string str2 = "Burak Selim Şenyurt";
Console.WriteLine("...String tipi için Hashcode...\n");
Console.WriteLine("str1 için Hash Değeri : {0} ",str1.GetHashCode().ToString());
Console.WriteLine("str2 için Hash Değeri : {0} \n", str2.GetHashCode().ToString());

Urun urn1 = new Urun(1000, "Balata", 10);
Urun urn2 = new Urun(1000, "Balata", 10);
Console.WriteLine("...Urun Sınıfı(Class) için Hashcode...\n"); 
Console.WriteLine("urn1 için Hash Değeri : {0} ", urn1.GetHashCode().ToString());
Console.WriteLine("urn2 için Hash Değeri : {0} \n", urn2.GetHashCode().ToString());

Personel prs1 = new Personel(1900, "Burak", 20);
Personel prs2 = new Personel(1900, "Burak", 20);
Console.WriteLine("...Personel Yapısı(Struct) için Hashcode...\n");
Console.WriteLine("prs1 için Hash Değeri : {0} ", prs1.GetHashCode().ToString());
Console.WriteLine("prs2 için Hash Değeri : {0} \n", prs2.GetHashCode().ToString());
```

Kodu çalıştırdığımızda aşağıdaki sonucu alırız.

![mk177_10.gif](images/mk177_10.gif)

Ekran görüntüsündende dikkat edeceğiniz üzere, string, ve Personel Yapısı nesne örnekleri için üretilen hash değerleri aynıdır. İlginç olan Urun sınıfına ait nesne örneklerinin içerikleri aynı olmasına rağmen üretilen hash değerlerinin farklı oluşudur. Oysaki iki nesne örneği eğer değer tipi modeline göre eşit iseler aynı hash değerlerinden bahsediyor olmamız gerekmektedir.

![dikkat.gif](images/dikkat.gif)

İki nesnenin içeriği değer tipi modeli gereğince eşitse, aynı hash değerlerinin üretiliyor olması gerekmektedir. Buda özellikle değer tipinden eşitlik modelini uygulayan tipler için GetHashCode metodunun ezilmesini gerektirir.

Nitekim String referans türü bu teoriyi doğrulamaktadır. Öyleki string tipi kendi içeriğinde değer tipli eşitlik modelini kullanmaktadır. Dolayısıyla yukarıdaki örnekte yer alan str1 ve str2 değişkenlerinin aynı hash kodlarını üretmesi beklediğimiz bir durumdur. Şimdilik, kendi yazmış olduğumuz sınıflardaki bu durumu incelemeden önce yapılarda (structs) farklı içerikler olması halinde farklı hash değerleri alıp alamayacağımızı kontrol etmemizde fayda var.

```csharp
string str1 = "Burak Selim Şenyurt";
string str2 = "Burak S. Şenyurt";
// Diğer kodlar

Personel prs1 = new Personel(1900, "Burak", 20);
Personel prs2 = new Personel(2000, "Burak", 20);
// Diğer kodlar
```

Yukarıdaki kod parçasını console uygulamamızda çalıştırırsak aşağıdaki ekran görüntüsünde yer alan sonuçları elde ederiz.

![mk177_11.gif](images/mk177_11.gif)

Eminimki değişiklik dikkatinizi çekmiştir. String değişkenlerde yapılan değişiklik sonucu üretilen hash değerleri farklıdır. Ancak yazmış olduğumuz Personel Yapısı için içerikte değişiklik (m_TcNo alanı değiştirilmiştir) yapmış olsakta durum değişmemiştir. Hash değerleri aynı olarak üretilmiştir. Oysaki normal şartlarda bir Personelin TC Kimlik Numarasının benzersiz olacağı kesindir. Buna göre hash algoritmasının uygun şekilde davranış göstermesi gerekmez mi? Aslında bizi yanıltan nokta Personel yapısı içerisinde yer alan alanların sıralamasıdır.

```csharp
private double m_Maas;
private int m_TcNo;
private string m_Ad;
```

Dikkat ederseniz ilk alan m_Maas alanıdır. Oysaki örnek kodumuzda sadece m_TcNo alanının değerini değiştirdik. Peki diğer alanların aynı kalması şartıyla m_Maas alanını değiştirirsek ne olur? Örneğin prs2 nesnesi için bu değeri 21 yapalım, prs1 için ise 20 olarak bırakalım.

```csharp
Personel prs1 = new Personel(1900, "Burak", 20);
Personel prs2 = new Personel(1900, "Burak", 21);
```

![mk177_12.gif](images/mk177_12.gif)

Gördüğünüz gibi farklı bir hash değeri elde ettik. Buna göre özellikle kendi değer türlerimiz için şu sonuca varabiliriz.

![dikkat.gif](images/dikkat.gif)

GetHashCode metodu kendi değer türlerimiz için bir hash değeri üretirken varsayılan olarak sadece yapı içerisindeki ilk alanın değerini göz önüne alır.

İlk alanın değil ama yapı içerisindeki benzersiz alanlardan birisinin kesin olarak Hash üretiminde kullanılmasını sağlamak için GetHashCode metodu ezilebilir. Örneğimizde Tc Kimlik Numarasının benzersiz olduğu düşünelecek olursa GetHashCode metodu ezilip içeride m_TcNo alanı üzerinden bir Hash değeri ürettirilebilir. Bu durumda yapımız içerisinde GetHashCode metodunu aşağıdaki şekilde ezme yoluna gidebiliriz.

```csharp
public override int GetHashCode()
{
    return m_TcNo.GetHashCode();
}
```

Uygulamayı bu haliyle çalıştıracak olursak eğer prs1 ve prs2 yapı örnekleri için geriye dönen hash değerlerinin 1900 olacağını görürüz. Oysaki prs1 nesnesinin maaş alanlarının değerleri halen daha farklıdır. Yani bu iki nesne örneği şu an için eşit değiller. Başka bir deyişle aynı içeriğe sahip değiller. Kurala göreyse, iki nesne örneği aynı içeriğe sahip iseler aynı hash değerlerinin üretilmesi gerekmektedir. Bu nedenle daha etkili bir yol olması açısından bu yapı içerisindeki tüm üyelerin string içeriklerini döndürecek bir içeriğin üzerinden Hash kod üretimine gidebiliriz. Aşağıdaki kod parçasında bu işlem ele alınmaya çalışılmıştır.

```csharp
public override string ToString()
{ 
    return m_TcNo.ToString() + " " + m_Ad.ToString() + " " + m_Maas.ToString();
}
public override int GetHashCode()
{
    return this.ToString().GetHashCode();
}
```

String metodu içerisinde yer alan kod ile, Personel yapısındaki tüm alanlar işin içerisine katılmış ve bir GetHashCode değeri elde edilmiştir.

![mk177_13.gif](images/mk177_13.gif)

Şimdi tekrar Urun sınıfımıza dönelim. Hatırlayacağınız gibi aynı içeriğe sahip olan urn1 ve urn2 nesneleri için farklı hash değerleri üretilmişti. Oysaki aynı hash değerlerinin üretilmesi gerektiğini söylemiştik. Buna göre, Personel yapısı içerisinde kullandığımız taktiği Urun sınıfımız içinde ele alabiliriz.

```csharp
public override int GetHashCode()
{
    return this.ToString().GetHashCode();
}
public override string ToString()
{
    return m_Id.ToString() + " " + m_Ad + " " + m_Fiyat.ToString();
}
```

![mk177_14.gif](images/mk177_14.gif)

Gördüğünüz gibi artık aynı içeriğe sahip olan Urun nesne örneklerimiz aynı hash değerlerini üretmektedirler. Object sınıfına ait sanal Equals metodunun ezilmesi ile GetHashCode metodunun ezilmesi arasında yakın bir ilişki yer almaktadır. Makalemizde üzerinde durmaya çalıştığımız konular bir takım kuralların doğmasına neden olmuştur. Buna göre;

- Equals metodu ezildiği takdirde GetHashCode metoduda ezilmelidir.
- == operatörü aşırı yüklendiği takdirde Equals metoduda ezilmelidir ve her ikiside aynı karşılaştırma algoritmasını içermelidir.
- Eğer karşılaştırma işlemleri için IComperable arayüzü uygulandıysa, Equals ezilmelidir ve her ikisi içinde aynı eşitlik algoritmaları kullanılmalıdır.
- Son olarak Equals, GetHashCode ve == operatörleri exception döndürmemelidir.

Bu kurallara dikkat edildiği takdirde eşitlik ilkelerinin sağlanması daha kolay olacak ve Hashtable yada Dictionary<> gibi hash kod tabanlı çalışan algoritmaların düzeni bozulmamış olacaktır. Bir başka deyişle eşitlikler üzerinde tutarlılığı sağlamış oluruz. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.
