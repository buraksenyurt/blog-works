---
title: "C# Temelleri : Enum Sabitinin Bilinmeyen Yönleri"
pubDate: 2006-10-30 02:00:00
categories:
  - C#
tags:
  - C#
  - Serialization
  - DataTable
---

# C# Temelleri : Enum Sabitinin Bilinmeyen Yönleri
Değerli Okurlarım Merhabalar,

Enum sabitleri geliştirici dostu tipler olarak düşünülebilir. Çoğu zaman uygulamalarımız içerisinde yer alan algoritmaların bazı durumlara göre farklı şekillerde hareket etmesi beklenir. Bu hareket serbestliğini sağlamanın kolay yollarından birisi, koşulların doğru şekilde tespitinden sonra, uygun bir biçimde ele alınabilmesidir. Bu amaçla sayısal değerler ile yapılan karşılaştırmalar son derece yerinde olmaktadır. Ancak algortima işleyişini değiştirmek için sayıları ele almak, eğer bu algoritmalar pek çok yerde kullanılacaksa çeşitli zorluklara neden olabilir. En azından hangi sayının ne anlama geldiğini yada o sayı için koşulun nasıl değiştirilmesi gerektiğini hatırlamak zor olabilir. Neyseki enum sabitleri sayesinde, bu tip sayıların anlamlı şekilde isimlendirilerek kullanılabilmesi sağlanmıştır.

Framework kendi içerisinde kullandığı pek çok yapıda aktif olarak enum sabitlerine başvurmaktadır. Örnek olarak, DataReader'lar için kullanılan CommandBehavior, DataTable içerisindeki satırların durumuna ilişkin bilgiler için kullanılan DataRowState ve benzerleri gibi daha pek çok enum tipini sayabiliriz. Built-in tipler dışında kendimizde enum sabitleri geliştirebiliriz ki buda işin en güzel taraflarından birisidir. Ne varki enum sabitlerini geliştirmek, sadece sayılara anlamlı isimler vermek kadar sade bir konu değildir. İşte bu makalemizde enum konusunun derinlerine inmeye çalışacağız. Temel olarak ele alacağımız konuları aşağıdaki başlıklar altında işlemeye çalışacağız.

- Bir enum sabitine ait içeriğin çalışma zamanında dinamik olarak elde edilmesi.
- String bir verinin, bir enum sabiti içeriğindeki karşılığının elde edilmesi.
- Enum sabitlerinin içeriğinin ToString metodu ile farklı biçimlerde elde edilmesi
- Veyalanmış (Ored) içeriklerin kullanımı.
- IsDefined metodu her zaman yeterli olmayabilir.

1. Bir enum sabitine ait içeriğin çalışma zamanında dinamik olarak elde edilmesi.

Bazı durumlarda enum sabitlerinin tüm içeriğinin çalışma zamanında bilinmesi ve kullanıcıya gösterilmesi gerekebilir. Söz gelimi, geliştirdiğimiz bir grafik uygulama olduğunu düşünelim. Tual üzerine çizilen resimlerin herhangibir şekilde saklandığını ve zaman içerisinde her hangibir noktada açıldıklarında, son kaydedildikleri halleriyle kullanıcıya sunulmak istendiklerini düşünelim. Çizilen şekiller ve bunlara ilişkin lokasyon, derinlik, ölçüler vb bilgileri saklamak için serileştirme gibi tekniklerden tutunda veritabanına kayıt etme yada özel formatlar ile fiziki dosyalara aktarma gibi pek çok yol tercih edilebilir. Lakin bu tip bir uygulamada ekranda olupta hatırlanması gereken şekilleri nerede saklarsak saklayalım nasıl bir veri tipi ile saklayacağımız önem kazanabilir. Çizilebilecek olan şekilleri string olarak adları ile tutmaktansa bunlara karşılık gelen sayısal değerlerden faydalanmak çoğu zaman saklanacak içeriğin boyutunu azaltabilir ve buda verinin daha az yer tutmasını sağlayabilir.

Çizilebilecek şekil tiplerinin sayısal olan karşılıklarının sadece saklama amacıyla ele alınması güzeldir. Peki geliştirici yada son kullanıcı bu sayıları nasıl anlayabilir? Enum sabitleri bu noktada işimize yarayabilecek bir tiptir. Peki bu şekil tiplerinin kullanıcı tarafından etkin bir şekilde kullanılabilmesi için ne yapabiliriz? İşte bu noktada System.Enum sınıfının static GetNames metodu devreye girmektedir. GetNames metodu bir enum sabiti içerisindeki tüm değerlerin string formatından bir dizi ile elde edilebilmesini sağlamaktadır. Aşağıdaki windows uygulamasında çeşitli şekilleri bünyesinde barındıran bir enum sabitinin içeriğinin çalışma zamanında farklı şekillerde elde edilişi gösterilmeye çalışılmaktadır. (Benzer istekler bir Web uygulamasında hatta bir web uygulamasında da göz önüne alınabilir.)

![mk179_1.gif](images/mk179_1.gif)

Uygulamada Sekil isimli kullanıcı tanımlı bir enum sabiti kullanılmaktadır. Örnek olması açısından enum sabiti içerisindeki değerler ComboBox, ListBox kontrollerine birer öğe olarak eklenmiş ayrıca her bir değer için birer RadioButton kontrolü dinamik olarak oluşturulmuştur. Enum sabiti içeriği ve uygulama kodumuz aşağıdaki gibidir.

Sekil Enum sabitimiz ve içeriği;

![mk179_2.gif](images/mk179_2.gif)

```csharp
public enum Sekil
{
    Kare, Dortgen, Daire, Elips, Cizgi, ParalelKenar, Altigen, Ucgen
}
```

Uygulama kodlarımız;

```csharp
/* parametre olarak verilen Sekil enum tipi içerisindeki tüm değerlerin adları string dizisine alınır.*/
string[] sekilTipleri = Enum.GetNames(typeof(Sekil));
foreach (string sekilTipi in sekilTipleri)
{
    // ComboBox kontrolüne eklenir.
    cmbSekiller.Items.Add(sekilTipi);
    // ListBox kontrolüne eklenir.
    lstSekiller.Items.Add(sekilTipi);

    // RadioButton haline getirilip FlowLayoutPanel kontrolüne eklenir.
    RadioButton rdbSekilTipi = new RadioButton();
    rdbSekilTipi.Name = sekilTipi;
    rdbSekilTipi.Text = sekilTipi;
    grpSekiller.Controls.Add(rdbSekilTipi);
}
```

Tekrar etmek gerekirse önemli olan nokta, GetNames metodu ve metodun döndürdüğü string dizinin kullanılış şeklidir. Dikkat ederseniz metoda parametre olarak Sekil isimli enum sabitimizin tipi (type) verilmiştir. Burada dikkat edilmesi gereken bir nokta daha vardır. Kullanıcı ComboBox, ListBox veya RadioButton kontollerinden birini seçtiğinde, seçilen bu bilginin enum sabitimiz içerisindeki hangi değere karşılık geldiğini tespit etmek. İşte bu konuyu ikinci maddemizde irdeleyeceğiz.

2. String bir verinin, bir enum sabiti içeriğindeki karşılığının elde edilmesi.

Belkide ilk akla gelen yöntem bir switch case yada çoklu if kullanımı olacaktır. Söz gelimi yukarıdaki windows uygulamamızda kullanıcının ComboBox, ListBox kontrolleri ile RadioButton kontrolünde seçebileceği bilginin enum tipi içerisinde karşılık geldiği değeri bulmaya çalışalım. Bu amaçla pekala aşağıdaki gibi bir metod yazabiliriz.

```csharp
private Sekil SecilenSekil(string sekilAdi)
{
    Sekil skl=0;
    switch (sekilAdi)
    {
        case "Kare":
            skl = Sekil.Kare;
            break;
        case "Dortgen":
            skl = Sekil.Dortgen;
            break;
        case "Daire":
            skl = Sekil.Daire;
            break;
        case "Elips":
            skl = Sekil.Elips;
            break;
        case "Cizgi":
            skl = Sekil.Cizgi;
            break;
        case "ParalelKenar":
            skl = Sekil.ParalelKenar;
            break;
        case "Altigen":
            skl = Sekil.Altigen;
            break;
        case "Ucgen":
            skl = Sekil.Ucgen;
            break;
    }
    return skl;
}
```

Ancak Enum sınıfının tam bu amaç için kullanılabilecek çok daha etkili bir üyesi vardır. Bu kadar uzun bir kod yazmaktansa özellikle değer türlerinin vazgeçilmez üyelerinden birisi olan Parse metodunun, Enum sınıfı içerisine dahil edilmiş versiyonunu aşağıdaki gibi kullanabiliriz.

![mk179_9.gif](images/mk179_9.gif)

```csharp
private Sekil SecilenSekil(string sekilAdi)
{
    return (Sekil)Enum.Parse(typeof(Sekil), sekilAdi, true);
}
```

Parse metodu sayesinde ikinci parametrede verilen string bilgi, ilk parametre ile belirlenen Enum tipi içerisinde aranır. Son parametre büyük küçük harf duyarlılığının olup olmayacağını belirlemek amacıyla kullanılan boolean bir değer almaktadır. Biz örneğimizde true değerini verdik. Eğer false değerini verseydik örneğin PARALELKENAR gibi bir bilgi için çalışma zamanında ArgumentException istisnasını alacaktık. Bunun nedeni elbetteki harf duyarlılığını kapatmış olmamız ve ParalelKenar ile PARALELKENAR bilgilerinin bu kritere göre eşit olmamasıdır.

Parse metodu sayesinde kullanıcının seçtiği bilgilerin, ilgili enum sabiti içerisindeki karşılıklarını bulabilir ve sayısal değerlerini elde edebiliriz. Aslında bir enum sabiti içerisinde yer alan değerlerin farklı formatta karşılıkları vardır. Örneğin bu değerlerin sadece string karşılıkları değil sayısal karşılıklarıda vardır. İşte bu karşılıkları elde etmek amacıyla ToString metoduna bir takım parametreler verilir ki buda makalemizin 3ncü maddesinin konusudur.

3. Enum sabitlerinin içeriğinin ToString metodu ile farklı biçimlerde elde edilmesi

Bir enum sabiti içerisindeki değerleri ToString metodu yardımıyla farklı formatlarda elde edebiliriz. Bu amaçla aşağıdaki tabloda yer alan değerler kullanılır.

Parametre Değeri
İşlevi

G veya g
Enum sabiti içerisindeki ilgili değeri genel (General) formatta yazar.

D veya d
Enum sabiti içerisindeki ilgili değerin sayısal (Decimal) karşılığını yazar.

X veya x
Enum sabiti içerisindeki ilgili değerin Hexadecimal (16lı sayı sistemindeki) karşılığını yazar.

F veya f
FlagsAttribute'unun kullanılıp kullanılmamasına göre ilgili enum sabiti değerinin string karşılığını yazar.

Örneğin, bir oyunda oyuncuların aldıkları puan gereği oynayabilecekleri seviyleri Yetki isimli enum sabiti ile ele aldığımızı düşünelim.

```csharp
public enum Yetki
{
    Caylak = 1
    ,Uzman = 2
    ,Profesyonel = 4
    ,Tecrubeli = 6 // Bu değer sonradan başımıza iş açabilir.
}
```

Buna göre bu enum sabiti içerisindeki her hangibir değeri ToString metodunun farklı versiyonları ile ekrana yazdırmaya çalıştığımızda aşağıdaki sonuçları alırız.

```csharp
static void Main(string[] args)
{
    Yetki ytk;
    // Tek bir enum değeri için ToString() çalışma şekilleri
    ytk = Yetki.Caylak;
    EnumInfo(ytk);
}

private static void EnumInfo(Yetki ytk)
{
    Console.WriteLine("------------------------------------");
    Console.WriteLine("Varsayılan : " + ytk.ToString());
    Console.WriteLine("General : " + ytk.ToString("G"));
    Console.WriteLine("Flags : " + ytk.ToString("F"));
    Console.WriteLine("Decimal : " + ytk.ToString("D"));
    Console.WriteLine("Hexadecimal : " + ytk.ToString("X"));
}
```

![mk179_3.gif](images/mk179_3.gif)

4. Veyalanmış içeriklerin kullanımı

Bazı durumlarda enum sabitleri içerisindeki değerlerin | işareti ile veyalandığını görürüz. Örneğin CommandBehavior enum sabitini kullanırken SingleRow|CloseConnection ifadesi ile veya benzerleri ile çok sık karşılaşırız (kullanırız). Aynı durum kendi yazmış olduğumuz enum sabitleri içinde geçerlidir. Yetki enum sabitimizi göz önüne alırsak eğer, aşağıdaki gibi bir kullanım söz konusu olabilir. Dikkat ederseniz bit seviyesinde veya (OR) işlemi uygulanmaktadır.

```csharp
Yetki ytk = Yetki.Profesyonel | Yetki.Uzman;
```

Burada ilginç olan bir durum vardır. O da Profesyonel ve Uzman değerlerinin sayısal toplamlarının 6 olmasıdır. 6 değeri dikkat ederseniz Yetki enum sabiti içerisinde yer alan Tecrubeli'nin sayısal değerinede karşılık gelmektedir. Bu nedenlede EnumInfo metoduna bu tip bir parametre gönderildiğinde aşağıdaki ekran çıktısının elde edilmesi son derece doğaldır. Ancak bu tip bir kullanım tercih edilmemelidir. Yani veyalanmış enum sabiti değerlerinin, başka bir enum sabitine denk gelmesi istenen bir durum değildir.

![mk179_4.gif](images/mk179_4.gif)

Yetki isimli enum sabiti içerisinde yer alan sayısal değerlere bakıldığında, veyalanmış olarak mümkün olabilecek pek çok kombinasyon olacağı kesindir. Örneğin aşağıdaki tabloda olası bir kaç kombinasyon verilmektedir.

Caylak | Uzman
3

Caylak | Profesyonel
5

Caylak | Tecrubeli
7

Uzman | Profesyonel
6

Uzman | Tecrubeli
8

Caylak | Uzman | Profesyonel
7

Caylak | Uzman | Tecrubeli
9

Caylak | Uzman | Profesyonel | Tecrubeli
13

Ancak burada dikkat edilmesi gereken önemli bir husus vardır. Bunun için, Enum sabiti içerisindeki kombinasyonların program tarafındaki sonuçlarına bakmamız gerekmektedir. Örneğin 3 ve 5 değerlerini veren kombinasyonlara bakalım.

```csharp
EnumInfo(Yetki.Profesyonel | Yetki.Uzman);
EnumInfo(Yetki.Caylak | Yetki.Profesyonel);
```

![mk179_5.gif](images/mk179_5.gif)

Dikkat ederseniz ToString metodunun varsayılan ile G parametresi kullanılan versiyonları sayısal değerlerin toplamını geriye döndürmüştür. Sadece F parametresi kullanarak elde ettiğimiz sonuçlarda ilgili değerlerin string karşılıkları, aralarına virgül konularak elde edilebilmiştir. Eğer ToString () ve ToString ("G") versiyonlarınında benzer şekilde davranmasını istersek enum sabitimiz üzerinde Flags niteliğini (attribute) uygulamamız gerekecektir.

```csharp
[Flags]
public enum Yetki
{
    Caylak = 1
    ,Uzman = 2
    ,Profesyonel = 4
    ,Tecrubeli = 6
}
```

Bu durumda aynı uygulamada aşağıdaki sonuçları alırız.

![mk179_6.gif](images/mk179_6.gif)

Flags attribute'u ile ilgili bu kısa bilgi ardından veyalanmış enum sabitleri için ele alacağımız bir diğer kritik noktaya gelelim. Özellikle enum sabiti içerisindeki çeşitli değerlerin birlikte veyalanarak kombinasyonlar oluşturulabildiğini gördük. Peki bu kombinasyonlar gerçektende doğru sayısal değerleri mi üretiyorlar? Örneğin bu maddenin başında Uzman | Profesyonel kombinasyonunun Tecrubeli'nin sayısal değerini ürettiğini gördük ve bunun istenmeyen bir durum olacağını vurguladık. Nitekim veyalanmış ifadeler kullanılacaksa sayısal değerlerin farklı veyalanmış kombinasyonlarda çok daha garip sonuçlar üretmesi muhtemeldir. Bu durumu analiz edebilmek için örnek uygulamamızda aşağıdaki kod parçasında görülen kombinasyonları deneyelim.

```csharp
Console.WriteLine("\tCaylak");
EnumInfo(Yetki.Caylak);

Console.WriteLine("\tProfesyonel | Uzman");
EnumInfo(Yetki.Profesyonel | Yetki.Uzman);

Console.WriteLine("\tCaylak | Profesyonel");
EnumInfo(Yetki.Caylak | Yetki.Profesyonel);

Console.WriteLine("\tCaylak | Uzman");
EnumInfo(Yetki.Caylak | Yetki.Uzman);

Console.WriteLine("\tProfesyonel | Tecrubeli");
EnumInfo(Yetki.Profesyonel | Yetki.Tecrubeli);

Console.WriteLine("\tYetki.Caylak | Yetki.Uzman | Yetki.Tecrubeli");
EnumInfo(Yetki.Caylak | Yetki.Uzman | Yetki.Tecrubeli);

Console.WriteLine("\tYetki.Caylak | Yetki.Profesyonel | Yetki.Tecrubeli");
EnumInfo(Yetki.Caylak | Yetki.Profesyonel | Yetki.Tecrubeli);
```

Bu durumda ekran çıktımız aşağıdaki gibi olacaktır.

![mk179_7.gif](images/mk179_7.gif)

Gördüğünüz gibi oldukça ilginç sonuçlar elde ettik. Pek çok kombinasyonda ne toplam sayısal değerleri ne de ToString ve ToString ("G") metodlarının sonuçları doğru olarak gözükmemektedir. O halde kombinasyonların doğru sonuçlar verebilmesini bir şekilde sağlamak gerekmektedir. Çoğunlukla bu durum için enum sabiti içerisindeki herhangibir değişkenin değerinin, kendisinden önce gelen sayısal değerlerin toplamından en azından bir fazla olması tercih edilir. Dolayısıyla enum sabitimizi aşağıdaki gibi değiştirmek yerinde olacaktır.

```csharp
[Flags]
public enum Yetki
{
    Caylak = 1
    ,Uzman = 2
    ,Profesyonel = 4
    ,Tecrubeli = 8
}
```

Programı tekrar çalıştırdığımızda aşağıdaki ekran görüntüsünde yer alan sonuçları alırız.

![mk179_8.gif](images/mk179_8.gif)

Gördüğünüz gibi bu sefer elde ettiğimiz sonuçlar çok daha tutarlı ve doğrudur. Kombinasyonlar doğru sayısal karşılıkları üretmiş ve Flags attribute'u sayesinde ToString () ve ToString ("G") metodları geriye düzgün kombinasyonlar döndürmüştür. O halde enum sabitlerimiz içerisindeki değerlerin veyalanmış hallerine ihtiyacımız olucaksa, Flags niteliğini kullanmakta ve sayısal değerleri bir öncekilerin toplamının en az bir fazlası olacak şekilde belirlemekte fayda vardır.

5. IsDefined metodu her zaman yeterli olmayabilir.

IsDefined, her hangibir enum değişkeninin ilgili enum sabiti içerisinde var olup olmadığına dair bool değer döndüren bir özelliktir. Bazı durumlarda parametrik bir bilginin, herhangibir enum sabiti içerisinde var olup olmadığını tespit etmek isteyebiliriz. Bu amaçla kendi switch..case yada if..else yapılarımızı kurabileceğimiz gibi Enum sınıfına ait static IsDefined metodundan da faydalanabiliriz. Ancak IsDefined metodu her zaman için yeterli olmayacaktır. Neden yeterli olmayacağını daha iyi anlayabilmek için bir örnek üzerinden devam edelim. Parca isimli bir enum sabitimiz olduğunu göz önüne alalım.

```csharp
public enum Parca
{
    Islemci=1
    ,Harddisk=2
    ,Ram=4
    ,EkranKarti=8
    ,SesKarti=16
}
```

Deneme olması açısındanda console uygulaması içerisinde aşağıdaki kodları ele alalım.

```csharp
Console.WriteLine("Islemci var mı ? "+Enum.IsDefined(typeof(Parca),"Islemci"));
Console.WriteLine("ISLEMCI var mı ? " + Enum.IsDefined(typeof(Parca), "ISLEMCI"));
Console.WriteLine("SESKarti var mı ? " + Enum.IsDefined(typeof(Parca), "SESKarti"));
Console.WriteLine("Parca.SesKarti var mı ? " + Enum.IsDefined(typeof(Parca), Parca.SesKarti));
Console.WriteLine("Parca.SesKarti|Parca.Ram|Parca.Islemci var mı ? " + Enum.IsDefined(typeof(Parca),Parca.SesKarti| Parca.Ram| Parca.Islemci));
Console.WriteLine("1 var mı ? " + Enum.IsDefined(typeof(Parca), 1));
Console.WriteLine("1|2 var mı ? " + Enum.IsDefined(typeof(Parca), 3));
```

IsDefined metodu ilk parametre olarak kontrol işleminin yapılacağı Enum sabitinin tipini almaktadır. Bu amaçla typeof parametresi kullanılmıştır. Ikinci parametre ise object tipinden olup aranan bilgiyi temsil etmektedir. Uygulamayı çalıştırdığımızda aşağıdakine benzer bir ekran görüntüsü elde ederiz. Şimdi bu durumu analiz etmeye çalışalım.

![mk179_10.gif](images/mk179_10.gif)

Kod satırlarımızın ilkinde Islemci isimli string bilgiyi kontrol ediyoruz. Islemci değeri, enum sabitimiz içerisinde gerçektende vardır. Dolayısıyla IsDefined metodu true değer döndürmüştür. Lakin burada case sensitive bir durum söz konusudur. Bu nedenle ISLEMCI ve SESKarti isimli bilgiler için IsDefined metodu false değerler döndürmektedir. 4ncü olarak Parca.SesKarti bilgisi IsDefined metodu ile kontrol edilmektedir. Çok doğaldırki Parca isimli enum sabitimiz içerisinde bu değer zaten vardır. Bu nedenle true değeri dönmüştür.

5nci kontrolde bir kombinasyon kullanılmış ve SesKarti, Ram ve Islemci değerlerinin bir arada olup olmaması durumu ele alınmıştır. Her ne kadar bu değerler enum sabiti içerisinde yer alsada sonuç false değer döndürmüştür. Öyleyse buna bir çözüm üretilmesi gerekmektedir. Çözüm üzerinde tartışmadan önce 6ncı ve 7nci çağırılarada göz atmakta fayda var. 6ncı çağırıda 1 sayısal değerini kontrol ediyoruz. Bu değerin karşılığı Islemci olduğu için IsDefined metodu geriye true döndürmektedir. Oysaki aynı durum 7nci kontrolde söz konusu değildir. Enum sabiti içerisinde açıkça belirtilmiş 3 sayısal değerine sahip bir değişken olmamasına rağmen Islemci ve HardDisk değişkenlerinin toplamı 3' tür. Dolayısıyla enum sabiti içerisinde 3' e karşılık gelen bir kombinasyon vardır ve fakat IsDefined metodu bunu analiz edememektedir. Bu tip durumlarda enum sabiti içerisine aşağıdaki gibi yeni bir değişken daha eklenir.

```csharp
public enum Parca
{
    Islemci=1
    ,Harddisk=2
    ,Ram=4
    ,EkranKarti=8
    ,SesKarti=16
    ,Hepsi=(Islemci|Harddisk|Ram|EkranKarti|SesKarti)
}
```

Ne yazıkki bu değişkenin eklenmesi IsDefined metodunun çalışma şeklini etkilemez. Bu yüzden kendi kontrol metodunu yazmamız gerekmektedir. Bu amaçla aşağıdakine benzer bir metod geliştirilebilir.

```csharp
static void Main(string[] args)
{
    Console.WriteLine("Parca.SesKarti var mı ? " + VarMi(Parca.SesKarti));
    Console.WriteLine("Parca.SesKarti|Parca.Ram|Parca.Islemci var mı ? " +VarMi(Parca.SesKarti | Parca.Ram | Parca.Islemci)); 
}

static bool VarMi(Parca prc)
{
    if((prc!=0)&&((Parca.Hepsi & prc)==prc))
        return true;
    else
        return false;
}
```

Bu metod her ne kadar IsDefined metodunda olduğu gibi object tipinden parametreler ile çalışmasada içerisinde yapıtığı kontroller sayesinde çeşitli kombinasyonlara karşı doğru tepkiler verebilecek şekilde tasarlanmıştır. En önemli kısım VarMi metodu içerisindeki if koşuludur. If koşulu içerisinde Parca.Hepsi ile metoda parametre olarak gelen Parca enum sabitinin ilgili değeri bit seviyesinde and (ve) işlemine tabi tutulmaktadır. Bu işlemin sonucunun yine metoda gelen parametre değerine eşit olması halinde true değeri döndürülmektedir. Örneğin Parca.SesKarti | Parca.Ram | Parca.Islemci kombinasyonunu göz önüne alalım. Burada bitsel seviyede veya işlemi gerçekleşmektedir ve sonuçta üretilen çıktı aşağıdaki gibi olacaktır.

Parca.SesKarti
16
0 0 0 1 0 0 0 0

Parca.Ram
4
0 0 0 0 0 1 0 0

Parca.Islemci
1
0 0 0 0 0 0 0 1

Bit seviyesinde | islemi. (Veya)

Parca.SesKarti | Parca.Ram | Parca.Islemci
21
0 0 0 1 0 1 0 1

Metod içerisinde ise gelen bu değer Parca.Hepsi ile bitsel ve işlemine tabi tutulmaktadır.

Parca.SesKarti | Parca.Ram | Parca.Islemci
21
0 0 0 1 0 1 0 1

Parca.Hepsi
31
0 0 0 1 1 1 1 1

Bit seviyesinde & islemi. (Ve)

Parca.SesKarti | Parca.Ram | Parca.Islemci
21
0 0 0 1 0 1 0 1

Elde edilen sonuca bakıldığında tekrar 21 değerini elde ettiğimizi bir başka deyişle metoda parametre olarak gönderilen enum değerinin, Parca enum sabiti içerisinde yer alıp almadığını tespit etmiş oluyoruz. Her ne kadar kendi yazdığımız VarMi metodu parametre olarak IsDefined'da olduğu gibi object tipiyle çalışmıyorsada istersek sayısal değerlerede tepki verebilecek hale getirebiliriz. Tek yapmamız gereken VarMi metodunu aşağıdaki gibi aşırı yüklemektir (overloading).

```csharp
static bool VarMi(int prc)
{
    if ((prc != 0) && (((int)Parca.Hepsi & prc) == prc))
        return true;
    else
        return false;
}
```

Dikkat ederseniz bu sefer parametremiz integer tipindendir. Elbette kontrol işlemi sırasında Parca.Hepsi enum değerinin sayısal karşılığını kullanmak zorundayız. Bu nedenle int cast operatörünü kullanıyoruz. Sonuçları inceleyebilmek için aşağıdaki test kodlarını ele alabiliriz.

```csharp
Console.WriteLine("Parca.SesKarti var mı ? " + VarMi(1));
Console.WriteLine("Parca.SesKarti|Parca.Ram|Parca.Islemci var mı ? " + VarMi(21));
Console.WriteLine("44 var mı ? " + VarMi(44));
```

Uygulamamızı bu haliyle çalıştırdığımızda aşağıdaki sonuçları elde ederiz.

![mk179_11.gif](images/mk179_11.gif)

Dikkat ederseniz 44 değerinin enum sabiti içerisinde bir karşılığı olmadığı için metodumuz başarılı bir şekilde false değerini döndürmektedir. Diğer taraftan Parca.SesKarti | Parca.Ram | Parca.Islemci kombinasyonuda enum sabiti içerisinde yer aldığı için metodumuz true değerini geriye döndürmektedir.

Bu makalemizde enum sabitlerini biraz daha derinlemesine incelemeye çalıştık. Var olan enum sabitlerinin içeriğinin çalışma zamanından nasıl elde edilebileceğini, her hangibir string bilginin bir enum sabiti içerisindeki karşılığının Parse metodu ile nasıl bulunabileceğini gördük. Bunların dışında, enum sabitlerinin içeriklerinin string olarak farklı formatlarda yazılmasını, Flag niteliğinin bu işteki yerine baktık. Son olarakta veyalanmış (bitsel or işlemine tabi tutulmuş enum değerlerinin) içeriklerin kullanımını ve dikkat edilmesi gereken noktaları inceledik. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek kod için tıklayın.](assets/EnumType.rar)
