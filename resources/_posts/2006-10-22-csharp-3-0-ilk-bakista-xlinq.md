---
layout: post
title: "C# 3.0 - İlk Bakışta XLINQ"
date: 2006-10-22 12:00:00 +0300
categories:
  - csharp
tags:
  - csharp
  - language-integrated-query
  - xlinq
---
XLINQ (Xml Language Integrated Query) temel olarak LINQ modelinin Xml üzerine uyarlanabilmesini hedeflemektedir. Bildiğiniz gibi LINQ projesi ile, IEnumerable arayüzünü uygulamış.Net nesneleri üzerinde dil ile tümleştirilmiş sorgulamalar gerçekleştirilebilmektedir. Microsoft aynı sorgu yapısını, veritabanı objelerinin programlama ortamında nesnel olarak ifade edilebildiği varlıklar (entities) üzerinde kullanılabilmesini de DLINQ (Database Language Integrated Query) ile sağlamaktadır. (DLINQ ile ilgili özet bilgileri bu [makalemden](http://www.bsenyurt.com/MakaleGoster.aspx?ID=174) bulabilirsiniz.)

XLINQ ise, yine dil tabanlı sorgulama özelliklerini alıp bunların XML verileri üzerinden gerçkeştirilebilmesini sağlmayayı amaçlamaktadır. Xml verilerinde dil tabanlı sorgular yapılabilmesinin dışında, var olan döküman nesne modeline (Document Object Model -DOM), XPath ve Xslt kavramlarına ek olacak şekilde yeni fonksiyonelliklerde gelmektedir. Örneğin bir Xml verisini bellek üzerinde oluşturmak için XDocument isimli yeni bir tipin yapıcı metodlarından faydalanabiliriz. Dahada ileri gidersek, XLINQ ' yu DLINQ ile tümleşik olacak şekilde kullanabiliriz. Bunun anlamı, veritabanı objelerinin nesnel olarak ifade edildiği varlıkları LINQ ile sorgulayıp, Xml formatında sonuç kümelerini çalışma zamanında (runtime) elde edebileceğimizdir. DLINQ'da olduğu gibi, XLINQ'da kendi içerisinde C# 3.0' ın yeniliklerini barındırmakta ve kullanmaktadır.

![dikkat.gif](/assets/images/2006/dikkat.gif)
XLINQ ile ilgili denemeleri [LINQ Preview](http://msdn.microsoft.com/data/ref/linq/) ekini kullanarak Vs.Net 2005 üzerinden deneyebilirsiniz.

XLINQ için gerekli tipler, System.Xml.XLinq isim alanı altında yer almaktadır. Bir Xml dökümanı ve içerisinde yer alabilecek elemanlar göz önüne alındığında bunların karşılıklarının yönetimli kod (managed code) tarafında X harfi ile başlayan tiplerle ifade edildiğini görebiliriz. Örneğin Xml elementlerini XElement tipi, nitelikleri (Attribute) XAttribute tipi karşılamaktadır. System.Xml.XLinq isim alanında (namespace) bu iki tipin dışında aşağıdaki tabloda bir kısmı verilen diğer yönetimli tiplerde yer almaktadır.

Tip Adı
Xml Karşılığı

XDocument
Bir Xml dökümanı içermekle sorumludur. XDocument sayesinde sıfırdan Xml içerikleri bellek üzerinde oluşturulabilir, kaydedilebilir yada var olan bir Xml içeriği belleğe yüklenebilir.

XElement
Xml dökümanlarında yer alan element'lerin yönetimli kod tarafındaki karşılığıdır.

XAttribute
Xml dökümanlarında yer alan niteliklerin (attribute) yönetimli kod tarafındaki karşılığıdır.

XComment
Xml dökümanlarında kullandığımız yorum satırlarını yönetim kod tarafında kullanımamızı sağlar.

XNode
Xml dökümanı içerisinde, node (boğum) olarak ifade edilebilecek her türden elemanı yönetimli kod tarafında ele alabilmek amacıyla kullanılır.

XProcessingInstruction
Xml dökümanlarının başında yer alan processing instruction'ların managed tarafta ele alınabilmesini sağlar. Bu iş için XDecleration sınıfındanda yararlanılmaktadır. Örneğin Xml dökümanları başına standard olarak gelen versiyon numarası, encoding gibi bilgileri bu XDecleration yardımıyla kod tarafında tanımlayabiliriz.

XText
Çoğunlukla CData (Character Data) bölümlerinin yada birleştirimiş metinlerin yönetimli kod tarafında ele alınabilmesi için kullanılır.

XNamespace
Xml içerisinde yer alan isim alanlarının yönetimli kod tarafında ele alınabilmesini sağlar.

XContainer
XDocument ve XElement sınıflarının türetildikleri abstract sınıftır. Özellikle Xml dökümanı üzerinde sorgulamalar yapabilmek için gerekli fonksiyonelikleride sağlar.

XDocumentType
DTD (Document Type Definitions) yapısının yönetimli kod tarafındaki karşılığıdır.

XElementSequence
LINQ için geliştirilmiş, genişletişmiş metodları (extension methods) barındıran static sınıftır. Extension Methods kavramı C# 3.0 ile birlikte gelmiştir ve var olan framework tiplerine ek metodlar yazılabilmesini sağlamaktadır.

Dilerseniz XLINQ ile geliştiricilerin hayatına girecek bir kaç yeniliği örnekler ile uygulayarak makalemize devam edelim. İlk olarak bir Xml verisinin bellek üzerinde oluşturulması sırasında XDocument tipinden nasıl yararlanabileceğimizi göreceğiz. Bu amaçla yeni bir LINQ Console Application projesi oluşturalım ve aşağıdaki kod satırlarını yazalım.

```csharp
using System;
using System.Collections.Generic;
using System.Text;
using System.Query;
using System.Xml.XLinq;
using System.Data.DLinq;

namespace UsingXLINQ
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Basit XLINQ Kullanımı

            XDocument xmlBook = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes")
                , new XElement("Books"
                    , new XElement("Book",
                        new XAttribute("ID", 1000)
                        , new XElement("Name", new XText("Her Yönüyle C#"))
                        , new XElement("ListPrice", new XText("50"))
                                            )
                    , new XElement("Book",
                        new XAttribute("ID", 1001)
                        , new XElement("Name", new XText("C# CookBook"))
                            , new XElement("ListPrice", new XText("45"))
                                            )
                                    )
                            ); 

            xmlBook.Save("BookWithXLinq.xml");

            XElement element = XElement.Load("BookWithXLinq.xml");
            Console.WriteLine(element.ToString()); 

            #endregion
        }
    }
}
```

İlk dikkatimizi çeken sanıyorumki XDocument nesnesinin oluşturuluş şekli olsa gerek. XDocument nesnesine ait yapıcı metod bir Xml verisinin sıfırdan oluşturulabilmesini sağlayacak şekilde tasarlanmıştır. Dolayısıyla yapıcı metodumuz içerisinde Xml verimiz için gerekli elemanları teker teker oluşturabiliriz. Örneğimizdeki XDocument nesne örneğinin yapıcı metodu (constructor) içerisinde ilk olarak her Xml içeriğinin sahip olması gereken processing instruction kısmını oluşturuyoruz. Bunu gerçekleştirmek için XDeclaration sınıfını kullanmaktayız.

Her Xml verisinin bir root elementi olması gerektiğinden XElement sınıfı ile ilk olarak Books isimli boğumu tanımlıyoruz. Burada dikkat etmemiz gereken bir nokta var. XElement sınıfına ait yapıcı metod (constructor) içerisinden yeni XElement yada XAttribute nesne örneklerini oluşturabiliyoruz. Dolayısıyla hiyerarşik olarak bir Xml veri ağacını yapıcı metod içerisinde tek seferde oluşturabilmekteyiz. Çünkü her elementin içereceği diğer elementleri yapıcıları içerisinde tanımlıyoruz.

Oluşturulan bu Xml içeriği bellek üzerinde yer alacaktır. Ancak dilersek bu içeriği fiziki bir kaynağada kaydedebiliriz. Bu amaçlada XDocument sınıfının Save metodu kullanılır. Tam tersine, fiziki bir Xml dosyasından bir XElement tipi içerisine yükleme işlemide gerçekleştirebiliri. Örneğimizde içeriği bir XElement tipine yüklemek amacıyla yine XElement tipinin static Load metodunu kullanıyoruz. Aslında bir xml içeriğini fiziki bir dosyadan okumak veya yazmak gibi işlemler zaten.Net içerisindeki bilinen Xml tiplerininde yapabileceği fonksiyonelliklerdir. Ancak bir Xml içeriğini yapıcı metod yardımıyla tek seferde oluşturabilme yeteneği XLINQ'nun vadettiği yeniliklerden birisidir. Sonuç itibariyle uygulamamızı çalıştırdığımızda aşağıdaki ekran görüntüsünü elde ederiz.

![mk178_1.gif](/assets/images/2006/mk178_1.gif)

Bununla birlikte BookWithXLinq.xml isimli dosyanında aşağıdaki gibi oluşturulduğunu görebiliriz.

![mk178_2.gif](/assets/images/2006/mk178_2.gif)

Bellek üzerinde XDocument ile oluşturulan yada bir XElement tipi içerisine yüklenen Xml verilerine sonradan eleman eklemek var olan elemanları değiştirmek hatta silmek gibi işlemleride gerçekleştirebiliriz. Örneğin aşağıdaki kod parçasında belleğe aldığımız Xml içeriğine yeni bir Book elemanı, alt elamanları ile niteliği eklenmektedir.

```csharp
XElement newBook = new XElement("Book",
                                    new XAttribute("ID", 1005)
                                        , new XElement("Name", new XText("Developing XLINQ"))
                                            , new XElement("ListPrice", new XText("80"))
                                    );
xmlBook.Element("Books").Add(newBook);

Console.WriteLine(xmlBook.ToString());
```

Books Xml içeriğine yeni bir kitap eklemek için XElement tipinden ve yapıcı metodundan faydalanmaktayız. Dikkat ederseniz Book boğumunu oluştururken içerisine ID attribute'unu, Name ve ListPrice elemanları ile bunların içeriklerini eklemekteyiz. Text tabanlı içeriğin eklenebilmesi içinde XText tipinden yararlanıyoruz. Bu tarz bir kullanım görsel arabirime sahip windows veya web tabanlı uygulamalarda hatta mobil uygulamalarda oldukça işe yarayacaktır. Öyleki Xml verisinin değerleri kullanıcı tarafından ele alınan bileşenlerden seçilerek elde edilebilir.

XElement tipi ile oluşturulan Xml elemanını var olan XDocument nesnesinin işaret ettiği Xml içeriğine eklemek için ise önce Books elemanına gidilmektedir. Bu amaçla Element metodu kullanılmış ve parametre olarak gidilecek elemanın adı verilmiştir ki buda root'tur. Element metodu geriye bir XElement döndürmektedir. Yani çalışma zamanında Books root elemanını işaret etmektedir. Bunun arkasından gelen Add metodu yardımıyla, bir üst satırda oluşturulan yeni Book elemanı root elamının içeriğine dahil edilmektedir. Programı bu haliyle çalıştırdığımızda aşağıdaki sonucu elde ederiz. Gördüğünüz gibi yeni oluşturduğumuz eleman var olan Xml içeriğinin sonuna eklenmiştir.

![mk178_3.gif](/assets/images/2006/mk178_3.gif)

Gelelim dil tabanlı sorgulama özelliklerinin XLINQ içerisindeki yerine. LINQ getirdiği imkanlar sayesinde veriler üzerinde sorgulamalar yapmamızı kolaylaştıracak yenilikler getirmektedir. Buna göre özellikle Sql'den aşina olduğumuz select, where, orderby, from, sum gibi pek çok kavramı, IEnumerable dan türemiş.Net tiplerinin sunduğu veriler üzerinde, veritabanı kaynaklarının nesnel karşılıklarının sunulduğu varlıklar (entities) üzerinde kullanabilmemiz mümkündür. Bu yenilikler kendisini Xml verilerinin sorgulanmasında da göstermektedir. Örneğin, aşağıdaki gibi örnek bir Xml içeriğimiz olduğunu düşünelim.

```csharp
<?xml version="1.0" encoding="utf-8" ?>
<Personelimiz>
    <Personel ID="1">
        <SicilNo>190002</SicilNo>
        <Ad>Burak Selim</Ad>
        <Soyad>Şenyurt</Soyad>
        <Maas>1000</Maas>
    </Personel>
    <Personel ID="2">
        <SicilNo>1903402</SicilNo>
        <Ad>Elma</Ad>
        <Soyad>Soz</Soyad>
        <Maas>5000</Maas>
    </Personel>
    <Personel ID="3">
        <SicilNo>1401202</SicilNo>
        <Ad>Kevın</Ad>
        <Soyad>Dankin</Soyad>
        <Maas>4500</Maas>
     </Personel>
    <Personel ID="4">
        <SicilNo>230002</SicilNo>
        <Ad>Carim Abdul</Ad>
        <Soyad>Cabbar</Soyad>
        <Maas>500</Maas>
    </Personel>
    <Personel ID="5">
        <SicilNo>121302</SicilNo>
        <Ad>Mayk</Ad>
        <Soyad>Cordın</Soyad>
        <Maas>2000</Maas>
    </Personel>
</Personelimiz>
```

Personel.xml isimli dosyada tutulan bu içerikte bir Personelin ID, Sicil Numarası, Ad, Soyad, Maas bilgileri tutulmaktadır. Bu Xml içeriğini belleğe alan bir uygulamada çok doğal olarak bazı sorgulamalar yapmak isteyebiliriz. Örneğin maaşı belirli bir değerin üzerinde olanları bu Xml dökümanı içerisinden çekmek istediğimizi düşünelim. Aşağıdaki kod parçası bu işlemin nasıl gerçekleştirilebileceğini göstermektedir.

```csharp
Console.WriteLine("Maaş değeri ");
double maas=Convert.ToDouble(Console.ReadLine());

XDocument docPersonel=XDocument.Load("..\\..\\Personel.xml");

XElement elements=new XElement("PersonelListe", 
    from pers in docPersonel.Elements("Personelimiz").Elements("Personel") 
        where (double)pers.Element("Maas")>=maas select pers
                            );

Console.WriteLine(elements.ToString());
```

Kod içerisinde LINQ ifadesinin nasıl kullanıldığına dikkat edelim. Uygulamamız kullanıcından maaş bilgisini aldıktan sonra ilk olarak Personel.xml isimli dosya içeriğini belleğe bir XDocument nesnesi yardımıyla alıyor. Sonrasında ise docPersonel nesnesi üzerinden bir LINQ sorgusu gerçekleştiriliyor ve sonuçlar bir XElement nesne örneğinde tutulacak şekilde alınıyor. Uygulama çalıştırıldığında ve maaş değeri olarak 2000 değeri girildiğinde aşağıdaki ekran görüntüsü ile karşılaşırız.

![mk178_4.gif](/assets/images/2006/mk178_4.gif)

LINQ sorgusu, XElement tipine ait nesne örneği oluşturulurken ikinci parametre içerisinde kullanılmıştır. Sql kullanıcıları için from, where ve select gibi anahtar kelimelerinin yeri biraz tuhaf gelebilir. Aslında yukarıdaki işlemi Sql dilinde düşündüğümüzde, "Select From Personel Where Maas>=2000" gibi bir ifadeyi göz önüne alabiliriz. LINQ da ise durum biraz daha farklı olmakla birlikte oldukça anlaşılır sorgular oluşturulabilmektedir. Dolayısıyla Xml verilerini LINQ'nun sağladığı bütün imkanlar ile sorgulama şansına sahibiz. Şüphesizki bu imkanlar, özellikle XPath ve XQuery gibi sorgulama teknolojilerin üzerine gelen oldukça esnek ve güçlü bir yapı olarak karşımıza çıkmaktadır. İşin güzel yanı burada ele aldığımız LINQ sorgularının anahtar kelimelerinin ve kullanım standardlarının, LINQ, XLINQ, DLINQ için aynı olmasıdır.

Şimdi XLINQ'nun DLINQ ile olan yakın ilişkisine bir göz atalım. Bildiğiniz gibi DLINQ mimarisinde, veritabanı nesnelerini uygulama tarafında temsil ettiğimiz objeler üzerinde LINQ sorguları çalıştırabilmekteyiz. Yani bir veritabanı tablosunu uygulama ortamında nesnel olarak ifade eden bir veri üzerinde dil tabanlı sorgular çalıştırabilmekteyiz. XLINQ ilede, DLINQ nun sunduğu varlıklar (entity) üzerinde yapacağımız LINQ sorguları sonucu elde edilen sonuç kümelerini çalışma zamanında Xml içeriği haline getirebilme şansına sahibiz. Yani DLINQ ve XLINQ özelliklerini bir arada ele alabiliriz. Bu konuyu daha net anlayabilmek için Northwind veritabanının entity karşılıklarını ele alacağımız bir örnek ile devam edeceğiz. LINQ Preview ile birlikte, DLINQ içerisinde kullanılan varlıkları (entities) yazmak yerine kolayca hazırlayabileceğimiz bir araç (tool) gelmektedir. SqlMetal isimli bu aracı aşağıdaki gibi komut satırından çalıştırabiliriz.

```csharp
D:\Program Files\LINQ Preview\Bin>sqlmetal /server:localhost /database:Northwind /code:NorthwindBase.cs
```

Bu durumda NorthwindBase.cs isimli bir sınıf oluşturulacak ve bizim için gerekli tüm sınıflar bu kaynak kod içerisine dahil edilecektir. SqlMetal aracı yardımıyla oluşturulan bu sınıfın içeriğine Class Diagram yardımıyla baktığımızda aşağıdaki ekran görüntüsüne benzer bir içerik ile karşılaşırız.

![mk178_5.gif](/assets/images/2006/mk178_5.gif)

Hatırlayacağınız gibi DLINQ konusunu incelediğimiz makalemizde yukarıdakine benzer bir yapıyı kendimiz yazarak geliştirmeye çalışmıştık. SqlMetal aracı yardımıyla oluşturulan NorthwindBase.cs dosyasını uygulamamıza ekledikten sonra aşağıdaki kodları yazalım.

```csharp
Console.WriteLine("Ürün Fiyatı ");
decimal fiyat=Convert.ToDecimal(Console.ReadLine());

Northwind nrth = new Northwind("data source=localhost;database=Northwind;integrated security=SSPI");

XElement urunler =new XElement("Urunler",
        from urn in nrth.Products where urn.UnitPrice> fiyat orderby urn.ProductName select
            new XElement("Urun",
                new XAttribute("ID",urn.ProductID),
                new XAttribute("Ad", urn.ProductName),
                new XAttribute("BirimFiyat", urn.UnitPrice)
            )
        );

urunler.Save("Urunler.xml"); 
Console.WriteLine(urunler.ToString());
```

Uygulamamızda kullanıcından bir fiyat bilgisi alıyoruz. Sonrasında ise Northwind sınıfımızı bağlantı için gerekli bilgi ile oluşturuyoruz. Bu işlemin arkasında Urunler isimli bir root eleman içerecek bir XElement nesnesi örnekliyoruz ve ikinci parametre içerisinde yine dil tabanlı bir sorgus cümleciği kullanıyoruz. Bu sefer Products tablosundan gelen (ki bu tablo Northwind sınıfı içerisinde Products isimli bir sınıfa karşılık geliyor) verilerden, UnitPrice alanı belli bir değerin üzerinde olan satırları ProductName alanına göre sıralayıp çekiyoruz. Çekilen verileri Urun isimli bir XElement içerisinde XAttribute sınıfı yardımıyla nitelik olarak oluşturuyoruz. Sonrasında ise deneme olması amacıyla elde edilen XElement'i fiziki bir dosyaya kaydediyor ve uygulama ekranına basıyoruz. Buna göre uygulama çalıştığında aşağıdaki ekran görüntülerini elde ederiz.

![mk178_6.gif](/assets/images/2006/mk178_6.gif)

Üretilen Xml dosyası içeriği ise aşağıdaki gibi olacaktır. Dikkat ederseniz, XElement sınıfı ile tanımladığımız nesne örneğinin sunduğu Xml içeriğinde ID, Ad, BirimFiyat isimli nitelikler (attributes) değerlerini Products sınıfına ait ProductID, ProductName, UnitPrice özelliklerinden alacak şekilde oluşturulmuş ve Urun isimli eleman (element) içerisinde toplanmışlardır.

![mk178_7.gif](/assets/images/2006/mk178_7.gif)

Görüldüğü gibi Xml dosyası ve ekran çıktısında UnitPrice alanının değeri 90 birimin üzerinde olanları elde ettik. Burada önemli olan nokta, dil tabanlı sorgulamayı kullanarak, varlıklar (entities) üzerinden bir sorgulama yapmamız ve sonuçları çekerken bunları XElement, XAttribute gibi tipler yardımıyla bir Xml içeriğine dönüştürmemizdir. Elde edilen bu Xml içeriği platformlar arasında hareket edebilecek, fiziki kaynaklar üzerine yazılabilecek, hatta uygulamaların görsel bileşenlerine bağlanabilecek (örneğin XmlDataSource yardımıyla bir TreeView kontrolüne yada bir GridView kontrolüne) hale getirilmektedir. XLINQ yukarıdaki örneklerde üzerinde durmaya çalıştığımız yenilikler dışında pek çok özellik daha içermektedir. Bunlar LINQ Preview yardımıyla gelen dökümantasyondan da bulabilirsiniz. Özetle XLINQ, dil tabanlı sorguları ve Xml ile ilgili yeni fonksiyonellikleri biz geliştiricilerin kullanımına sunmayı hedeflemektedir. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek kod için tıklayın.](/assets/files/2006/UsingXLINQ.rar)