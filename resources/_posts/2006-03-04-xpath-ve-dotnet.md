---
layout: post
title: "XPath ve .Net"
date: 2006-03-04 06:00:00 +0300
description: "XPath, XML dökümanları üzerinde basit tipte sorgulamalar yapmamıza izin veren bir dildir. Yapısı gereği kullanıldığı pek çok alan vardır. Örneğin XQuery içinde yada bir XSLT dökümanında bu dilin izlerine rahatlıkla rastlayabilirsiniz. Tam olarak yaptığı işin, XML dökümanı içerisinde lokasyon aramak ve bulmak olduğunu söyleyebiliriz. XPath, herhangibir Xml verisi üzerinde arama ve konumlandırma işlemini gerçekleştirmek için Document Object Model' i kullanılır. Yani, xml verisinin bellekteki hiyerarşik ağaç yapısını ele alır. Bunun içinde Xml verisinin yapısal (structural) metadata bilgisine bakar. XPath başlı başına bir dildir ve bu konu ile ilgili yazılmış kitaplar vardır. ( Size eski olmasına rağmen Sams' ın XPath Kick Start : Navigating XML with XPath 1.0 and 2.0 kitabını tavsiye edebilirim.)"
categories:
  - xml
tags:
  - xml
  - csharp
  - dotnet
  - http
  - performance
---
# XPath ve .Net
Değerli Okurlarım Merhabalar,

XPath, XML dökümanları üzerinde basit tipte sorgulamalar yapmamıza izin veren bir dildir. Yapısı gereği kullanıldığı pek çok alan vardır. Örneğin XQuery içinde yada bir XSLT dökümanında bu dilin izlerine rahatlıkla rastlayabilirsiniz. Tam olarak yaptığı işin, XML dökümanı içerisinde lokasyon aramak ve bulmak olduğunu söyleyebiliriz. XPath, herhangibir Xml verisi üzerinde arama ve konumlandırma işlemini gerçekleştirmek için Document Object Model'i kullanılır. Yani, xml verisinin bellekteki hiyerarşik ağaç yapısını ele alır. Bunun içinde Xml verisinin yapısal (structural) metadata bilgisine bakar. XPath başlı başına bir dildir ve bu konu ile ilgili yazılmış kitaplar vardır. (Size eski olmasına rağmen Sams'ın [XPath Kick Start: Navigating XML with XPath 1.0 and 2.0](http://www.amazon.com/gp/product/0672324113/sr=8-3/qid=1141335716/ref=pd_bbs_3/102-6694554-3383307?_encoding=UTF8) kitabını tavsiye edebilirim.)

Biz bu makalemizde çok fazla detaya inmeyecek ve XPath'i.Net üzerinde nasıl kullanabileceğimizi basit olarak incelemeye çalışacağız. Konuyu iyi anlayabilmek amacıylada XPath ile oluşturulmuş çeşitli tipteki sorguları örnek bir xml dökümanı üzerinde kullanacağız. Örneğin elimizde Kitaplara ait yazar, isim, fiyat gibi bilgileri tutan bir Xml dökümanı olduğunu düşünelim. Aşağıda bu amaçla kullanabileceğimiz Kitaplar.xml adlı xml veri dosyasının bir parçasını görüyorsunuz. (Xml dosyasını ve örnek kodları buradan [indirebilirsiniz](/assets/files/2006/UsingXPath.rar).)

![mk150_1.gif](/assets/images/2006/mk150_1.gif)

Bu Xml verisini göz önüne alacak olursak, XPath'in ilgili veri kümesini bellekte ağaç (tree) modeli şekline tasvir edeceğini ve buna göre yer belirleme işlemini gerçekleştireceğini söyleyebiliriz. Kitaplar.xml dosyamızın içeriğini ele aldığımızda, XPath modeli, bellek üzerinde aşağıdaki şekile benzer bir ağaç yapısını kullanacaktır. Bu hiyerarşik yapı zaten Document Object Model'in bir uyarlamasıdır.

![mk150_2.gif](/assets/images/2006/mk150_2.gif)

Dikkat ederseniz Root element üzerinden xml verisi içerisinde tüm alt elementler bu elementlerin var ise atrribute'ları ele alınmaktadır. Kitaplar.xml verisinin tamamını grafiğe sığdırmamız zor olacağından sadece bir kısmı burada gösterilmetedir. İşte XPath, herhangibir kritere göre bir node listesi, tek bir node veya değer (örneğin toplam gibi) döndürmek için bu ağaç yapısını kullanacaktır. Bu modele göre XPath dili ile aşağıdaki tabloda da belirtilen örnek sorgular oluşturabiliriz.

XPath Sorgusu
Anlamı

Kitaplar/Kitap
Kitaplar içerisindeki her bir Kitap elementini, alt elemenetleri ile birlikte elde etmemizi sağlar.

Kitaplar/Kitap/@ID
Kitaplar içerisindeki her bir Kitap elementinin içerdiği ID attribute'larının tamamını geri döndürür.

Kitaplar/Kitap[@ID=1000]
Kitaplar içersinde Kitap element'lerinden, ID attribute'unun değeri 1000 olan elementi (elementleri) elde etmemizi sağlar.

Kitaplar/Kitap[Fiyat<=50]
Kitaplar içerisinde Kitap element'lerinden Fiyat element'inin değeri 50 veya daha az olanları elde etmemizi sağlar.

Kitaplar/Kitap[Fiyat>50 and Fiyat<80]
Kitaplar içerisinde, Kitap elementler'inden Fiyat elementinin değeri 50 ile 80 arasında olanları elde etmemizi sağlar.

count (/Kitaplar/Kitap)
Toplam Kitap sayısını verir. Bu tekil bir sonuç değeri döndürür. Üstteki sorgularda olduğu gibi bir node listesi döndürmez.

sum (/Kitaplar/Kitap/Fiyat)
Kitapların fiyatlarının toplamını geriye döndürür. Yani Kitap node'ları içerisindeki Fiyat node'larının value özelliklerinin değerlerinin toplamını verir.

sum (/Kitaplar/Kitap/Fiyat[.>70])
Fiyat element'inin değeri 70' den yüksek olan Kitap element'lerindeki Fiyat element'lerinin toplamını verir.

count (/Kitaplar/Kitap/Fiyat[.>70])
Fiyat element'inin değeri 70' den yüksek olan Kitap element'lerinin sayısını verir.

Buradaki sorgularda kısaltmalar (abbreviation) kullanılmıştır.(/ veya. gibi) Bunun birde kısaltma kullanılmayan (unabbreviation) şekli vardır. Genellikle kıslatmaların yer aldığı kullanım şekli daha yaygındır ve esnektir.

Gelelim yukarıdakine benzer XPath ifadelerini.Net üzerinde nasıl kullanacağımıza. Framework Class Library aslında tamamıyla XPath mimarisini kullanmaya yarayan tipleri barındıran System.Xml.XPath isim alanına sahiptir. Buradaki temel sınıflar yardımıyla XML dökümanları üzerinde XPath ile arama ve yer belirleme işlemlerini gerçekleştirebiliriz. Bununla birlikte bir XmlDocument nesnesinin SelectNodes veya SelectSingleNode gibi metodlarında parametre olarak XPath ifadelerini kullanabiliriz. Hangisini tercih edeceğimiz, Xml dökümanı üzerinde ne gibi işlemler yapmak istediğimize bağlıdır. Eğer Xml dökümanında belirli bir lokasyona erişip elde ettiğimiz node listelerinde değişiklikler yapmak istiyorsak XmlDocument sınıfının SelectNodes veya SelectSingleNode metodlarını tercih edebiliriz. Bununla birlikte sadece arama ve arama sonuçlarını gösterme amaçlı uygulamalarda, XPath isim alanında yer alan XPathDocument gibi sınıfları kullanmak performans ve hız açısından daha efektif sonuçlar üretecektir. Nitekim XPathDocument, XPathNavigator ve XPathNodeIterator gibi sınıflar XmlDocument sınfına göre daha performanslıdır.

Aşağıdaki örnek Console uygulamasında çok basit olarak XPathDocument sınıfından yararlanılarak fiyatı 50 ile 80 arasında olan kitaplara ilişkin bilgiler xml verisi içerisinde bulunarak ekrana yazdırılmaktadır. Ayrıca, fiyatı 70' den büyük olan kitapların sayısıda ekrana farklı bir teknik ile (Evaluate) yazdırılmaktadır.

```csharp
using System;
using System.Xml;
using System.Xml.XPath;

namespace UsingXPathNavigator
{
    class Class1
    {
        static void UsingXPath()
        {
            XPathDocument doc = new XPathDocument("..\\..\\Kitaplar.xml");
            XPathNavigator navigator = doc.CreateNavigator();

            XPathNodeIterator nodes = navigator.Select("Kitaplar/Kitap[Fiyat>50 and Fiyat<80]");
    
            while (nodes.MoveNext())
            {
                Console.WriteLine(nodes.Current.ToString());
            }

            Console.WriteLine("Toplam Kitap Sayisi (Fiyat>70) "+navigator.Evaluate("count(/Kitaplar/Kitap/Fiyat[.>70])").ToString());
        }

        static void Main(string[] args)
        {
            UsingXPath();
        }
    }
}
```

Programı çalıştırdığımızda aşağıdakine benzer bir ekran görüntüsü elde ederiz.

![mk150_3.gif](/assets/images/2006/mk150_3.gif)

Dilerseniz kısaca kodlarımızı inceleyelim ve ne yaptığımızı daha iyi anlamaya çalışalım. XPathDocument sınıfı, herhangibir Xml dökümanını XPath dilini kullanarak sorgulayabilmemizi sağlayan çeşitli fonksiyonelliklere ve özelliklere sahiptir. Aslında XPath, Xml dökümanlarını DOM (Document Object Model)' e uygun olan bir ağaç yapısı şeklinde inceler. Dolayısıyla XPathDocument sınıfı xml verisini bu modele uygun bir bellek görüntüsünü oluşturmak amacıyla kullanılır. XPathDocument sınıfına ait bir nesne örneği ile belleğe alınan ağaç modeli üzerinde hareket edebilmek için, XPathNavigator sınıfına ait metodlar kullanılabilir. XPathNavigator, abstract bir sınıf olduğundan (yani nesne örneği oluşturulamayan bir sınıf), XPathDocument sınıfının CreateNavigator metodu yardımıyla oluşturulur.

XPathNavigator sınıfı iki önemli metod sunar. Bunlar Select ve Evaluate metodlarıdır. Select metodu çoğunlukla geriye birden fazla sayıda sonuç döneceği zaman kullanılır. Örneğimizde olduğu gibi. Select metodu aslında geriye XPathNodeIterator tipinden bir nesne örneği döndürmektedir. Bu sınıf basit olarak, Select metodu sonucu elde edilen veri seti üzerinde, ileri yönlü hareket etmemizi sağlar. Biz bu iterasyon ile Kitap node'larının tüm içeriğini ekrana yazdırıyoruz. Ancak gördüğünüz gibi ekran çıktısı çokta mükemmel değil. Aşağıdaki kod parçasıda, Kitap node'larının alt node'larındada ileri yönlü hareket ediyor ve içeriğin daha okunabilir olmasını sağlıyoruz. Öyleki XML dökümanımızda Kitap node'larının alt node'larından olan Yazarlar node'ununda alt node'ları var. Bu yüzden iç içe 3 while iterasyonu yazmamız gerekiyor.

```csharp
static void UsingXPathSecond()
{
    XPathDocument doc = new XPathDocument("..\\..\\Kitaplar.xml");
    XPathNavigator navigator = doc.CreateNavigator();

    XPathNodeIterator nodes = navigator.Select("Kitaplar/Kitap[Fiyat>50 and Fiyat<80]");

    // İlk olarak sorgu sonucu elde edile fiyatı 50 ile 80 arasındaki Kitap node' larında hareket ediyoruz.
    while (nodes.MoveNext())
    {
        // Her bir Kitap elementinin alt node' larını alıyoruz ve bu node' lar içerisinde ileri yönlü hareket ediyoruz.
        XPathNodeIterator childNodes=nodes.Current.SelectChildren(XPathNodeType.Element);
        while(childNodes.MoveNext())
        {
            // Eğer o anki alt node'un adı Yazarlar ise bu kez güncel Kitap node' unun altındaki Yazarlar node' unun alt node' ları içerisinde ileri doğru hareket ediyoruz. Eğer güncel alt node Yazarlar node' u değilse o node' un adını ve değerini yazdırıyoruz.
            if(childNodes.Current.Name=="Yazarlar")
            {
                XPathNodeIterator yazarlarNodes=childNodes.Current.SelectChildren(XPathNodeType.Element);
                Console.Write("Yazar(lar) : ");
                while(yazarlarNodes.MoveNext())
                {
                    Console.Write(yazarlarNodes.Current.Value+", ");
                }
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine(childNodes.Current.Name+":"+childNodes.Current.Value);
            }
        }
        Console.WriteLine();
    }
    Console.WriteLine("Toplam Kitap Sayisi (Fiyat>70) "+navigator.Evaluate("count(/Kitaplar/Kitap/Fiyat[.>70])").ToString());
}
```

![mk150_4.gif](/assets/images/2006/mk150_4.gif)

XPathNavigator sınıfımızın yukarıdaki örneklerde kullandığımız diğer önemli fonksiyonelliğinin Evaluate metodu ile sağlandığını söylemiştik. Bu metod geriye object tipinden bir değer döndürmektedir. Bu yüzden örneğimizde olduğu gibi count ile hesaplanan tek bir değeri almak için birebirdir. Evaluate metodunu, Command sınıfının ExecuteScalar metoduna benzetebilirisiniz. Her ikiside bir veri seti üzerinden geriye tek bir değer döndürmek amacıyla kullanılır.

XPath dilini kullanabileceğimiz tek tip XPatDocument sınıfı ve bununla ilişkili diğer sınıflar değildir. XPath dili ile yazılan sorguları XmlDocument sınıfına ait herhangibir nesne örneği içinde de kullanabiliriz. XmlDocument sınıfının SelectNodes ve SelectSingleNode isimli metodları parametre olarak XPath tipinden ifadelerde almaktadır. Örneğin aşağıdaki kod parçasında XmlDocument sınıfından nesne örneğimizin bellekte tuttuğu xml verisi içerisinde Bill Evjen isimli yazara ait Kitap node'larının listesi elde edilmektedir. SelectNodes metodu ile bulunan node'ları aldıktan sonra bunların sayısına bakaraktan üst node'lara (parent node) nasıl çıktığımıza dikkat edin.

```csharp
static void UsingXmlDoc()
{
    XmlDocument xmldoc = new XmlDocument();
    xmldoc.Load("..\\..\\Kitaplar.xml");

    XmlNodeList resultNodes = xmldoc.SelectNodes("/Kitaplar/Kitap/Yazarlar[Yazar='Bill Evjen']");
    XmlNode currNode;
    XmlNode parentNode;

    if(resultNodes.Count==1)
    {
        currNode=resultNodes[0];
        parentNode=currNode.ParentNode;
        Console.WriteLine(parentNode.InnerText);
    }
    else
    {
        foreach(XmlNode aNode in resultNodes)
        {
            parentNode=aNode.ParentNode;
            Console.WriteLine(parentNode.InnerText);
        }
    }
}

static void Main(string[] args)
{
    UsingXmlDoc();
}
```

![mk150_5.gif](/assets/images/2006/mk150_5.gif)

Böylece geldik bir makalemizin daha sonuna. Bu makalemizde XPath dili ile geliştirdiğimiz sorguları.Net sınıfları ile nasıl kullanabileceğimizi inceledik. Özetlemek gerekirse, XPath modelinin sunduğu imkanları kullanabileceğimiz yerler XPathDocument sınıfı ve XmlDocument sınıflarıdır. Her iki sınıf ile XPath ifadelerini etkin bir şekilde kullanabiliyoruz. Bunlara ek olaraktan ileridede göreceğimiz gibi XPath ifadelerini XQuery veya XSLT içerisinde de aktif olarak kullanmaktayız. Bu modelleri daha sonraki yazılarımızda incelemeye çalışacağız. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.