---
layout: post
title: "LINQ: Daha Fazla Sorgu"
date: 2007-02-21 10:00:00 +0300
categories:
  - linq
tags:
  - csharp
  - language-integrated-query
---
Bu günlerde hepimiz.Net Framework 3.0 ve getirileri üzerine yoğunlaşmış durumdayız. Özellikle mimari anlamda yapılan köklü değişimler söz konusu. Bu köklü değişiklikler; Windows uygulamalarının yeni yüzü olan WPF (Windows Presentation Foundation) ve XAML (eXtensible Application Markup Language), dağıtık mimariyi tek çatı altında toplamayı başaran WCF (Windows Communication Foundation), akış şemaları ve iş süreçlerinin.Net plaformuna dahil edilmesini sağlayan WF (Workflow Foundation) ve CardSpace olarak sıralanabilir. Ancak bunların dışında Microsoft'un gelecek vizyonu içerisinde yer alan en önemli konulardan biriside C# 3.0 konusudur. Bildiğiniz gibi C#, sıfırdan geliştirilmiş ve atası olan nesne yönelimli dillerin en iyi özelliklerini bünyesinde birleştirerek bunu güçlü bir Framework üzerinde kullanabilmemizi sağlayan bir dildir. Zaman içerisinde C# 2.0 ile gelen yenilikler şu anda tüm C# geliştiricilerin hayatının bir parçası haline gelmiştir. Şimdi herkesin gözü C# 3.0 üzerinde.

C# 3.0, beraberinde LINQ (Language Integrated Query), DLINQ (Database Language Integrated Query) ve XLINQ (Xml Language Integrated Query) gibi yeni teknolojileride getirmekte ve desteklemektedir. Biz bu makalemizde daha fazla LINQ ifadesi yazmaya çalışacağız. Onbir basit LINQ ifadesi ile dil tabanlı sorguları daha yakından tanımaya başlıyacak ve elimizdeki gücün farkına varacağız. Bildiğiniz gibi LINQ (Language Integrated Query) özellikle dil içerisinde, Sql tarzı sorgular yazabilmemizi ve bunları var olan IEnumerable türevli tipler üzerinde kullanabilmemizi sağlamaktadır. Ancak özellikle LINQ içerisinde kullanılabilen operatörler göz önüne alındığında, oldukça etkili sonuçlar alabileceğimiz ortadır. Temel olarak LINQ içerisindeki operatörler aşağıdaki başlıklar altında toplanmıştır. (Elbetteki bu bilgiler hala deneme aşamasında olan bir sürece aittir ve değişebilir.)

- Kısıtlama Operatörleri (Restriction Operators) -> Where
- Gruplama Operatörleri (Grouping Operators) -> Group
- Sıralama Operatörleri (Ordering Operators) -> OrderBy, ThenBy, Reverse
- Bölümleme Operatörleri (Partitioning Operators) -> Take, Skip, TakeWhile, SkipWhile
- Seçme Operatörleri (Projection Operators) -> Select
- Set Operatörleri (Set Operators) -> Distinct, Union, Intersect, Except
- Dönüştürme Operatörleri (Conversion Operators) -> ToArray, ToList, ToDictionary, OfType
- Eleman Operatörleri (Element Operators) -> First, FirstOrDefault, ElementAt
- Üretim Operatörleri (Generation Operators) -> Range, Repeat
- Gruplama Fonksiyonu Operatörleri (Aggregate Operators) -> Count, Sum, Min, Max, Averaga, Fold
- Ölçüm Operatörleri (Quantifiers Operators) -> Any, All
- Çeşitli Operatöler (Miscellaneous Operators) -> Concat, EqualAll
- Özel Seri Operatörleri (Custom Sequence Operators) -> Combine

Şimdi gelin bu operatörlerin bir kısmını incelemeye çalışalım. Öncesinde program ortamında ele alabileceğimiz bazı veri kümelerine ihtiyacımız olacak. Bu veri kümeleri tamamıyla test amaçlı olacaktır. Bunun için AdventureWorks veritabanında yer alan Product ve ProductSubCategory tablolarından faydalanabiliriz. Amacımız ilk olarak buradaki tablolardan test amacıyla kullanabileceğimiz veri kümelerini program ortamı içerisinde yer alan generic koleksiyonlara aktarmaktır. LINQ konusu söz konusu olduğu içinde, C# 3.0 dili özelliklerinden de faydalanmaya çalışacağız. Yardımcı sınıfımızın kodları aşağıdaki gibidir.

![mk192_1.gif](/assets/images/2007/mk192_1.gif)

Product Sınıfı;

```csharp
public class Product
{
    public int ProductId;
    public string Name;
    public double ListPrice;
    public DateTime SellStartDate;
    public DateTime SellEndDate;
    public int ProductSubCategoryId;

    public override string ToString()
    {
        return ProductId.ToString() + " " + Name + " " + ListPrice.ToString("C2") + " " + SellStartDate.ToString() + " " + SellEndDate.ToString() + " " + ProductSubCategoryId.ToString();
    }
}
```

ProductSubCategory Sınıfı;

```csharp
public class ProductSubCategory
{
    public int ProductSubCategoryId;
    public string Name;

    public override string ToString()
    {
        return ProductSubCategoryId.ToString() + " " + Name;
    }
}
```

Product ve ProductSubCategory sınıflarımızda dikkat ederseniz özellik (Property) kullanmadık. Ayrıca içerideki elemanlara ilk değerlerini kolayca atayabilmek için yapıcı metod (constructor) da yazmadık. Burada C# 3.0 ile gelen nesne başlatıcılarını (object initializers) kullanacağımız için özellike ve yapıcı metod kullanımını terk ettik. Şimdi bu tiplerden üyelere yükleme yapacak olan Helper sınıfımızın kodlarınıda aşağıdaki gibi geliştirelim.

Helper.cs

```csharp
public class Helper
{
    public static List<Product> UrunleriYukle()
    {
        List<Product> urunler = new List<Product>();
        using (SqlConnection conn = new SqlConnection("data source=localhost;database=AdventureWorks;integrated security=SSPI"))
        {
            SqlCommand cmd = new SqlCommand("SELECT ProductID, Name, ListPrice, SellStartDate, SellEndDate, ProductSubcategoryID FROM Production.Product WHERE (Name IS NOT NULL) AND (ListPrice IS NOT NULL) AND (SellStartDate IS NOT NULL) AND (SellEndDate IS NOT NULL) AND (ProductSubcategoryID IS NOT NULL)", conn);

            conn.Open();
            SqlDataReader dr=cmd.ExecuteReader();
            while (dr.Read())
            { 
                // Object Initializers kullanarak nesneye ilk değerlerini atıyoruz.
                Product urn=new Product{ProductId=Convert.ToInt32(dr["ProductID"]),Name=dr["Name"].ToString(),ListPrice=Convert.ToDouble(dr["ListPrice"]),SellStartDate=Convert.ToDateTime(dr["SellStartDate"]),SellEndDate=Convert.ToDateTime(dr["SellEndDate"]),ProductSubCategoryId=Convert.ToInt32(dr["ProductSubCategoryId"])};
                urunler.Add(urn);
            }
        }
        return urunler;
    }

    public static List<ProductSubCategory> AltKategorileriYukle()
    {
        List<ProductSubCategory> altKategorileri = new List<ProductSubCategory>();
        using (SqlConnection conn = new SqlConnection("data source=localhost;database=AdventureWorks;integrated security=SSPI"))
        {
            SqlCommand cmd = new SqlCommand("SELECT ProductSubCategoryId,Name From Production.ProductSubCategory", conn);

            conn.Open();
            SqlDataReader dr=cmd.ExecuteReader();
            while (dr.Read())
            { 
                // Object Initializers kullanarak nesneye ilk değerlerini atıyoruz.
                ProductSubCategory subCat=new ProductSubCategory{ProductSubCategoryId=Convert.ToInt32(dr["ProductSubCategoryId"]),Name=dr["Name"].ToString()};
                altKategorileri.Add(subCat);
            }
        }
        return altKategorileri;
    }
}
```

UrunleriYukle ve AltKategorilerYukle isimli metodlarımız sadece bize yardımcı olacak üyelerdir. Dikkat ederseniz her ikiside generic List koleksiyonu tipinden değişkenler döndürmektedirler. Biz bu koleksiyonlar üzerinde LINQ sorgularımızı denemeye çalışacağız. Testleri daha kolay yapabilmek için şimdilik bir Console uygulamasından devam edeceğiz. Dilerseniz ısınma turlarımızda basit Select ve Where kullanımları ile başlayalım.

Sorgu 1: Ürünlerden belirli bir alt kategoride olanların bulunması.

```csharp
var products = Helper.UrunleriYukle();
var subCategories=Helper.AltKategorileriYukle();

Console.WriteLine("\nProductSubCategoryId' si 1 olan Urunler\n");

var resultSet=from p in products 
                          where p.ProductSubCategoryId==1 
                            select p;

foreach(Product prd in resultSet)
    Console.WriteLine(prd.ToString());
```

İlk olarak kaynak verilerimizi Helper sınıfı içerisine dahil ettiğimiz static metodlarımız yardımıyla yüklüyoruz. (Buradan sonraki örnek kodlarımızda products ve subCategories isimli değişkenlerin taşıdığı koleksiyon verilerini kullanacaktır.) Bu işlemler sırasındada veri tipini kolayca kullanabilmek için var anahtar sözcüğünden faydalanmaktayız. Sorgu ifademiz içerisinde select, where ve from anahtar sözcükleri kullanılmaktadır.

İfademize göre products isimli koleksiyon içerisindeki her bir nesne örneği p adıyla tanımlanmaktadır. Sonrasında ise bu p adlı nesnelerden ProductSubCategoryId özelliğinin değeri 1 olanların çekilmesi sağlanmıştır. Dikkat ederseniz, p nesnesinin işaret ettiği tür Product tipi olduğu için p.ProductSubCategoryId gibi bir ifade yazılabilmiştir. Uygulamamızı çalıştırdığımızda aşağıdaki ekran görüntüsüne benzer bir sonuç alırız.

![mk192_2.gif](/assets/images/2007/mk192_2.gif)

Şimdi elimizde LINQ gibi bir seçenek olmadığını düşünelim. Bu durumda yukarıdaki ihtiyacı karşılamak için aşağıdakine benzer bir kod parçası yazabilirdik. Önce, generic koleksiyon içerisinde dolaşır, her bir Product nesnesinin ProductSubCategoryId değerine bakar ve 1 olanları, başka bir generic List koleksiyonu içerisinde toplardık.

```csharp
List<Product> urunler = new List<Product>();

foreach (Product prd in products)
    if (prd.ProductSubCategoryId == 1)
        urunler.Add(prd);
```

Lakin daha karmaşık sorgulama ifadeleri göz önüne alındığında durum dahada zorlaşabilir. İmkansız değildir ama daha fazla kod yazmamızı, efor sarfetmemizi ve zaman zaman optimizasyonu zor olacak kodlar üretmemize neden olacak durumlarla karşılaşabiliriz. Örneğin, Alt Kategorilerin ve Urunlerin birbirleriyle birleştirileceğini, birleştirilen küme üzerinde gruplama yapılacağını ve hatta bu gruplamalara göre toplam fiyat, toplam ürün sayısı gibi değerlerin elde edileceğini düşünebiliriz. (Bu pek çok Sql programcısına tanıdık gelecek bir ihtiyaçtır.) Bu tarz bir ihtiyacı karşılayacak kod parçasını elbetteki yazabiliriz. Ama LINQ ile bunu ve benzerlerini çok daha basit bir biçimde, tek satırlık ifadelerde gerçekleştirme şansına sahibiz. Dilerseniz yeni örnekler ile alıştırmalarımıza devam edelim.

Sorgu 2: Liste fiyatı 3000 birimin üzerinde olan ürünleri isimlerine göre tersten sıralı olacak şekilde elde etmek.

```csharp
var resultSet2=from p in products 
                        where p.ListPrice>=3000
                            orderby p.Name descending
                                select p;

Console.WriteLine("\nFiyatı 3000' den büyük olan Urunler. Name alanına göre tersten sıralı. \n");

foreach(Product prd in resultSet2)
    Console.WriteLine(prd.ToString());
```

Dikkat ederseniz Liste fiyatına göre kontrol işlemini gerçekleştirmek için yine where operatöründen faydalanıyoruz. Sıralamayı Product tipindenki Name alanına göre tersten sıralatmak istediğimiz içinde, orderby ve descending anahtar sözcüklerinden faydalanmaktayız. Kodu bu şekilde denediğimizde aşağıdaki gibi bir ekran çıktısını elde ederiz.

![mk192_3.gif](/assets/images/2007/mk192_3.gif)

Sorgu 3: Fiyatı 1000 ile 1500 birim arasında olan ürünleri isimlerine göre tersten sıralayarak elde etmek.

```csharp
Console.WriteLine("\nFiyatı 1000 ile 1500 arasında olan ürünlerin Name alanına göre tersten sıralanmış listesi\n");

var resultSet3=from p in products
                        where p.ListPrice>=1000 && p.ListPrice<=1500
                            orderby p.Name descending
                                select p;

foreach(Product prd in resultSet3)
    Console.WriteLine(prd.ToString());
```

Sorgu 2' dekine benzer şekilde yazılan bu kod parçasında tek fark && (ve) operatörünün kullanılmasıdır. Kod çalıştırıldığında aşağıdaki ekran görüntüsündekine benzer bir sonuç elde ederiz. Bu seferki sorgu cümlemiz ListPrice alanı için bir değer aralığı belirlemekte ve bu kritere uyan tüm ürünleri elde etmemizi sağlamaktadır.

![mk192_4.gif](/assets/images/2007/mk192_4.gif)

Sorgu 4: Urunler listesindeki elemanları elde ederken isimsiz bir tipten yararlanmak.

C# 3.0 beraberinde isimsiz tip (Anonymous Type) adı verilen yeni bir kavram ile birlikte gelmektedir. Diyelim ki LINQ ifadesi sonrasıda çekilen veri kümesi içerisindeki elemanları taşıyan yeni bir tipi kullanmak istemiş olalım. Çok doğal olarak, sorgulamak istediğimiz veri kümesi ne olursa olsun, önceden planlamadığımız şekilde bir tipe ihtiyacımız olması doğaldır. İşte isimsiz tip bize bu konuda yardımcı olabilmektedir. Örneğimizde bunun basit bir kullanımı gösterilmektedir. Dikkat ederseniz, Product tipine ait Name ve ListPrice alanlarını sırasıyla UrunAdi ve Fiyat olarak taşıyan yeni bir tip tanımlanmış ve foreach iterasyonu içerisinde kullanılmıştır. (İsimsiz tipler aslında tanımlandıkları yerde oluşturulan tipler olarak da düşünülebilir. Dolayısıyla proje derlendiğinde aşağıda karşılaşılan satır için yeni bir tip CIL tarafına yazılmaktadır.)

```csharp
Console.WriteLine("\nSelect sorgularında Anonymous Type Kullanımı\n");

var resultSet4=from p in products
                        select new {UrunAdi=p.Name,Fiyat=p.ListPrice.ToString("C2")};

foreach(var prd in resultSet4)
    Console.WriteLine(prd.UrunAdi+" "+prd.Fiyat);
```

Örneğimizi çalıştırdığımızda aşağıdakine benzer bir sonuç elde ederiz.

![mk192_5.gif](/assets/images/2007/mk192_5.gif)

Sorgu 5: Sql'de Join sorgusu olurda LINQ içerisinde olmaz mı?

Hepimiz Sql tarafında farklı tabloları birleştirmek için Join ifadelerinden faydalanırız, faydalanmaktayız. Aynı özellik LINQ ile,.Net platformuna da taşınmıştır. İşte aşağıda örnek bir sorgu. Bu sorguda products ve subCategories isimli değişkenlerde tutulan koleksiyonların içerdikleri verileri ProductSubCategoryId alanlarının değerlerine göre birleştiriyoruz. Elde edilen sonuç kümesinde, her iki koleksiyonda da var olan özelliklerden kombine edilecek yeni bir tip olsa hiç de fena olmaz aslında. İşte isimsiz tipimizin devreye gireceği yer burası olacaktır. (Bu tip bir ihtiyacı birde LINQ eklentilerini kullanmadan yapmayı denersek, sorgu ifadelerinin gelecekte hayatımızı oldukça kolaylaştıracağını daha kolay anlayabiliriz.) Birleştirme operasyonu için join anahtar kelimesinden yararlanılmaktadır. on anahtar kelimesinden sonra ise bildiğimiz kriter uygulaması gerçekleştirilmektedir.

```csharp
var resultSet6=from prd in products 
                         join ctg in subCategories
                             on prd.ProductSubCategoryId equals ctg.ProductSubCategoryId
                                 select new{prd.ProductId,prd.Name,prd.ListPrice};
 
 foreach(var p in resultSet6) 
     Console.WriteLine(p.ToString());
```

Programımızın ekran çıktısı aşağıdakine benzer olacaktır.

![mk192_6.gif](/assets/images/2007/mk192_6.gif)

Ekrandaki çıktı tesadüf değildir. İsimsiz tiplerin ToString metodunun bir sonucudur.

> Bir isimsiz tip tanımlandığında bu tip için CIL tarafına eklenen kodlar içerisinde ToString metodu ezilip yukarıdakine benzer bir string döndürmesi sağlanmıştır. Bu kodu herhangibir decompiler aracı ile açtığınızda da görebilirisiniz.
> ![mk192_7.gif](/assets/images/2007/mk192_7.gif)

Sorgu 6: Urunler listesini alt kategorilere göre gruplamak, her bir gruptaki toplam ürün sayısı, ortalama, en yüksek, en düşük fiyatları bulmak.

Böyle bir sorgu için öncelikle ürünleri, alt kategorilerine göre gruplamamız gerekmektedir. Sonra gruplanan veri kümesi üzerinde bazı gruplama fonksiyonlarını kullanmalıyız. Bu cümleleri sarfettikçe aslında bir T-Sql sorgusunu ifade etmeye çalıştığımı düşünüyorum. Ama artık Sql değil LINQ tarafında ve daha tanıdık topraklardayız. Öyleyse hiç vakit kaybetmeden örnek kodumuzu aşağıdaki gibi geliştirelim.

```csharp
var result7=from prd in products
                     group prd by prd.ProductSubCategoryId into g
                         orderby g.Key
                             select new{
                                 Kategori=g.Key
                                 ,UrunSayisi=g.Count()
                                 ,ToplamFiyat=g.Sum(prd=>prd.ListPrice)
                                 ,EnDusuk=g.Min(prd=>prd.ListPrice)
                                 ,EnYuksek=g.Max(prd=>prd.ListPrice)
                                 ,OrtalamaFiyat=g.Average(prd=>prd.ListPrice)
                             };
 foreach(var result in result7)
 {
     Console.WriteLine("Kategori Id {0}",result.Kategori.ToString());
     Console.WriteLine("\t Toplam Ürün Sayısı {0}",result.UrunSayisi.ToString());
     Console.WriteLine("\t Toplam Birim Fiyat {0}",result.ToplamFiyat.ToString("C2"));
     Console.WriteLine("\t En Düşük Birim Fiyat {0}",result.EnDusuk.ToString("C2"));
     Console.WriteLine("\t En Yüksek Birim Fiyat {0}",result.EnYuksek.ToString("C2"));
     Console.WriteLine("\t Ortalama Fiyat {0}",result.OrtalamaFiyat.ToString("C2"));
     Console.WriteLine();
 }
```

İlk olarak group anahtar sözcüğünü kullanarak products içerisindeki Product tiplerini ProductSubCategoryId alanlarına göre gruplayacağımızı ve grupladığımız verileri g takma isimli tip içerisinde saklayacağımızı belirtiyoruz. Hatta elde edilen sonuç kümesini, grupladığımız verilerin Key özelliğine göre (ki burada Key özelliği ProductSubCategoryId alanının değerini işaret etmektedir) sıralatmaktayız. Sıralatma işlemi için bildiğimiz orderby anahtar kelimesinden faydalanıyoruz. Sonrasında ise yine bir isimsiz tip karşımıza çıkıyor ki sonuç kümesinde üretilen satırları farklı bir nesne örneği olarak kullanabilmek için tam aradığımız yapı.

Burada dikkate değer bazı noktalar da vardır. Özellikle gruplama fonksiyonlarının nasıl kullanıldığına dikkat edelim. Burada => gibi bir operatörle karşılaşmaktayız. Bu operatör Lambda operatörü olarak adlandırılan ve C# 3.0 ile birlikte gelen bir yeniliktir. Gruplama fonksiyonları aslında, elde edilen küme içerisindeki elemanları dolaşıp bunlar üzerinde gerekli fonksiyonları çalıştıracak şekilde tasarlanmışlardır. Örneğin g.Sum (prd=>prd.ListPrice) çağrısı, sonuç kümesindeki prd takma isimli her bir Product tipini dolaşıp ListPrice alanlarını toplayarak bir sonuç üretmek üzere tasarlanmıştır. Diğer gruplama fonksiyonlarıda buna benzer şekilde çalışmaktadır. Sonuç olarak örnek kodumuz çalıştırdığımızda aşağıdakine benzer bir ekran çıktısı elde ederiz.

![mk192_8.gif](/assets/images/2007/mk192_8.gif)

Sorgu 7: Urunler içerisinde baş harfi H olanların liste fiyatına göre tersten sıralanmış halini seçip bir diziye taşımak.

Sanırım, LINQ ifadeleri sonrası elde edilen sonuç kümelerini işe yarayabilecek başka tipte nesnelere aktarabilmek oldukça işe yarar bir fonksiyonellik olurdu. LINQ bu amaçla elde edilen sonuçların bir diziye (Array), List veya Dictionary koleksiyonlarına aktarılmasını sağlayan fonksiyonelliklerde içermektedir. Örneğin aşağıdaki kod parçası, products koleksiyonunda baş harif H olan ürünlerin liste fiyatına göre tersten sırlanmış halinin bir diziye aktarılmasını sağlamaktadır. Baş harfe göre karşılaştırma yapabilmek için indeksleyici operatörünü Name alanı üzerinde nasıl kullandığımıza dikkat edelim. Sonuçta Name alanı string tipte bir değişkendir ve bir karakter dizisini işaret etmektedir. Bu nedenle C# dilinin tüm sürümlerinden bildiğimiz gibi 0 indisli eleman aslında Name alanının işaret ettiği verinin birinci karakteri olacaktır.

```csharp
var result8=from prd in products
                    where prd.Name[0]=='H'
                        orderby prd.ListPrice descending
                            select prd;

var result9=result8.ToArray();

for(int i=0;i<result9.Length;i++)
    Console.WriteLine(result9[i].ToString());
```

Burada başrol oyuncumuz ToArray metodudur. ToArray metodu, elde edilen sonuç kümesinin bir diziye aktarılmasını sağlar. Böylece sonuç kümesindeki elemanlara indeks numaraları yardımıyla erişmemizde mümkün olmaktadır. Diğer taraftan bir dizinin sunacağı avantajları değerlendirme şansınada sahip olabiliriz. Aynı işlemleri LINQ olmadan yapmaya çalışırsak, yukarıdaki kod parçasına dair yüzümüzde bir tebessüm oluşmaması için hiç bir neden kalmayacaktır. Uygulamanın çalışmasının sonucunda aşağıdakine benzer bir ekran görüntüsü elde ederiz.

![mk192_9.gif](/assets/images/2007/mk192_9.gif)

Sorgu 8: Join ile birleştirilmiş bir sorgu sonucunu generic Dictionary koleksiyonuna aktarmak.

Join sorgusu ile urunler ve altKategorileri birleştirdiğimizi düşünelim. Bu sonuç kümesinde ProductSubCategoryId alanı Key ve Product nesne örnekleride Value olacak şekilde bir Dictionary koleksiyonu oldukça işimize yarayabilir. Bildiğiniz gibi Dictionary bazlı koleksiyonlar (Hashtable<>, Dictionary<>, Hashtable vb...) verileri key-value çiftleri şeklinde tutmaktadırlar. Dolayısıyla özellikle birleştirilmiş veri kümelerinde elde edilen verileri key ve value olacak şekilde tutmak işe yarayabilir. Bunu gerçekleştirmek için aşağıdaki gibi bir kod parçasını kullanabiliriz.

```csharp
var result11=from prd in products
                        join ktg in subCategories 
                            on prd.ProductSubCategoryId equals ktg.ProductSubCategoryId
                                select new {Kategori=ktg.Name,Urun=prd};

var result12=result11.ToDictionary(ktgName=>ktgName.Urun.ProductId);

IEnumerator<int> numerator=result12.Keys.GetEnumerator();
while(numerator.MoveNext())
{
    var currentProduct=result12[numerator.Current];
    Console.WriteLine(currentProduct.Urun.ProductId.ToString()+" "+currentProduct.Urun.Name+" "+currentProduct.Urun.ListPrice.ToString("C2")+" "+currentProduct.Kategori);
}
```

Bu sefer başrol oyuncumuz ToDictionary metodudur. Yanlız metodumuz parametresine dikkat edelim. Burada ktgName adıyla her bir urunun ProductId değerleri alınmakta ve Dictionary bazlı koleksiyonun içerisindeki anahtarlara (key) atanmaktadır. Değer (Value) kısmında ise sorgu sonucu elde edilen tipler yani Kategori ve Urun alanlarından oluşan isimsiz tiplerimiz eklenmektedir. Son olarak test amacıyla elde edilen Dictionary koleksiyonu içerisindeki anahtarlarda bir numarator yardımıyla dolaşılmaktadır. Burada bildiğimiz ileri yönlü iterasyon deseni uygulanmaktadır. Aslında uygulamayı debug edersek ve result12 değişkenine QuickWatch penceresinden bakarsak, sorgu sonuçlarının gerçektende key-value çiftleri şeklinde eklendiği generic bir Dictionary koleksiyonu ile karşılaşmış oluruz.

![mk192_10.gif](/assets/images/2007/mk192_10.gif)

Uygulamamızı çalıştırdığımızda ise aşağıdakine benzer bir çıktı elde ederiz.

![mk192_11.gif](/assets/images/2007/mk192_11.gif)

Sonuç kümelerini Dictionary koleksiyonlarına nasıl alabiliyorsak List tipinden generic koleksiyonlarada alabiliriz. Tek yapılması gereken başrol oyuncusunu değiştirmek olacaktır. Yani ToDictionary yerine ToList metodunu kullanamak.

Sorgu 9: Ürünler içerisinde kaç farklı liste fiyatı olduğunu bulup bunları küçükten büyüğe doğru elde etmek.

Eminimki Sql bilen herkes bu iş için distinct operatörünün kullanılması gerektiğini söyleyecektir. Aynı operatör LINQ içerisinde yer almaktadır. Örneğin, ürünlerin tutulduğu koleksiyon içerisindeki liste fiyatlarını tekrarsız olarak elde etmek istediğimizi düşünecek olursak aşağıdaki kod parçasından faydalanabiliriz.

```csharp
var result13=(from prd in products
                        orderby prd.ListPrice
                            select prd.ListPrice).Distinct();

foreach(var result in result13)
    Console.WriteLine(result.ToString());
```

Dikkat ederseniz Distinct metodunu kullanmadan önce, tüm LINQ ifadesi paranetez içerisine alınmıştır. Nitekim Distinct operasyonu elde edilen sonuç kümesi üzerinden uygulanmaktadır. Kodun çalışması sonucu programın çıktısı aşağıdaki ekran görüntüsündekine benzer olacaktır.

![mk192_12.gif](/assets/images/2007/mk192_12.gif)

Sorgu 10: Ürünleri önce adlarına göre küçükten büyüğe sonrada liste fiyatlarına göre büyükten küçüğe sıralatarak elde etme.

Tipik olarak bahsettiğimiz, birden fazla alan üzerinde Order By işlemini uygulamaktan başka bir şey değildir. Bunu gerçekleştirmek için, LINQ ifadelerinde orderby operatöründen faydalanabiliriz. Aşağıdaki kod parçası bu işlemi gerçekleştirmektedir.

```csharp
var result17=from prd in products
                        orderby prd.Name,prd.ListPrice descending
                            select prd;

foreach(var result in result17)
    Console.WriteLine(result.ToString());
```

Dikkat ederseniz orderby anahtar kelimesinden sonra, sıralama için ele alınacak alanlar virgül ile ayrılarak yazılmıştır. Bununla birlikte liste fiyatına göre tersten sıralatma yaptırmak istediğimiz için descending anahtar sözcüğünden yararlanılmıştır. Özellikle kendi geliştirdiğimiz tipleri bir koleksiyonda kullandığımızda ve bu koleksiyon içerisindeki elemanları sıralatmak istediğimizde, genellikle IComparer yada IComparable gibi arayüzlerin ilgili sınıflara uygulanmasını sağlamamız gerekir. Oysaki LINQ ile bu tarz bir ihtiyaç çok daha kolay bir şekilde ele alınabilmektedir. Programı çalıştırdığımızda aşağıdakine benzer bir ekran çıktısı elde ederiz. Dikkat ederseniz her ürün alfabetik olarak sıraya dizildikten sonra her ürün için ilk harflerine göre kendi içerisinde liste fiyatına göre tersten sıralanmıştır.

![mk192_13.gif](/assets/images/2007/mk192_13.gif)

Sorgu 11: Ürünlerin isimlerini karakter uzunluklarına göre tersten sıralayarak elde etme.

Bu oldukça enteresan olacak. products koleksiyonu içerisindeki her bir Product tipinin Name alanlarını ele alacağız. Bunların karakter uzunluklarına göre tersten sıralatacağız. İşte bu eğlenceli sorgunun LINQ karşılığı.

```csharp
var result18=from prd in products
                        orderby prd.Name.Length descending
                            select new{UrunAdi=prd.Name,ListeFiyati=prd.ListPrice};

foreach(var result in result18)
    Console.WriteLine(result.UrunAdi);
```

Dikkat ederseniz tek yaptığımız orderby anahtar kelimesinden sonra prd örneklerinin Name alanlarının Length özelliklerini ele almak. descending anahtar kelimeside tersten sıralatma işleminin gerçekleştirilmesini sağlıyor. Kodu çalıştırdığımızda gerçektende isimlerin uzunluklarına göre azalan bir formasyonda sıralandığını görebiliriz.

![mk192_14.gif](/assets/images/2007/mk192_14.gif)

Bu makalemizde 11 değişik sorguda LINQ'yu daha yakından tanımaya çalıştık. Makalemizin başında belirttiğimiz gibi LINQ içerisinde oldukça fazla sayıda operatör yer almaktadır. Anders Hejlsberg, LINQ teknolojilsini özellikle veritabanı programcılığı yapanların dil içerisindede aynı felsefeyi kullanabilmeleri için geliştirildiğini belirtmiştir. Gerçektende yukarıdaki dil içi sorgular buna verilebecek örneklerden sadece bir kaçıdır. İlerleyen makalelerimizin birisinde diğer operatörleride ele almaya çalışacağız. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.