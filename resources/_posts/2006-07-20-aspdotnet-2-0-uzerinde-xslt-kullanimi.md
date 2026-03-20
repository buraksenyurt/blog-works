---
layout: post
title: "Asp.Net 2.0 Üzerinde Xslt Kullanımı"
date: 2006-07-20 12:00:00 +0300
categories:
  - aspnet-2-0
tags:
  - aspnet-2-0
  - xml
  - csharp
  - dotnet
  - aspnet
  - http
  - performance
---
Xml (eXtensible Markup Language - genişletilebilir işaretleme dili), farklı platformlar arasında kolayca bilgi taşınmasına izin veren, veri kümelerini kendi kurallarımızla oluşturmamızı sağlayan önemli standartlardan birisidir. Lakin zaman zaman Xml dökümanlarını okumak çok kolay olmamaktadır. En azından oluşturulan Xml dökümanlarında tutulan içeriği, son kullanıcıya farklı şekillerde göstermek ihtiyacını duyabiliriz. İşte bu noktada, var olan Xml verisinin farklı bir formata dönüştürülebiliyor olması gerekmektedir. XSLT (eXtensible Stylesheet Language Transformation) standardı tam bu noktada devreye girmektedir. Xslt herhangibir Xml içeriğini farklı bir Xml, Html, Csv (Comma Seperated Values) veya Text formatına dönüştürme işlemi ile ilgili materyalleri sağlayan bir işaretleme dilidir.

![mk168_1.gif](/assets/images/2006/mk168_1.gif)

Bu sayede verileri sakladığımız bir Xml içeriğini son kullanıcıya daha uygun formatlarda sunabilme imkanını kazanmış oluruz. Bu özellikle web tabanlı sistemlerde son derece önemlidir. Dönüştürme işlemi sırasında sıradan bir tarayıcı kullanılabileceği gibi,.Net Framework içerisinde yer alan tiplerdende (types) faydalanılabilir. Yani Xslt ile çarpıştırılmış bir Xml içeriğine basit bir tarayıcı ile bakabilir yada bu çarpıştırma işlemini yönetimli kod (managed code) üzerinden.Net Framework tiplerini kullanarak gerçekleştirebiliriz. Xslt kendi içerisinde XPath dilini kullanmaktadır. Bu sayede kaynak Xml dökümanı içerisinde eşleştirme yaparak, dönüştürme işlemini uygulayacağı parçaları kolayca bulabilir. Xslt sadece Xml içeriğini farklı formatta göstermekle kalmaz dönüşüm işlemi sırasında aşağıdaki tabloda belirtilen işlemlerinde yapılabilmesini sağlar.

Xslt Dönüştürme Operasyonunda Yapılabilecek Bazı İşlemler

Sıralama işlemleri uygulatabilmek (Sorting).

Filtreleme işlemleri.

Koşullu ayrıştırmalar (if ve choose kullanımı).

Parametre kullanımı sayesinde farklı dönüştürme işlemlerinin sağlanabilmesi.

Xslt dökümanı içerisinde harici kullanıcı tanımlı fonksiyonları çağırabilmek.

Xslt dökümanı içerisinde script çalıştırabilmek.

Xslt'nin yukarıda sayılanlar haricinde de sahip olduğu pek çok yetenek vardır elbette. Ancak bunlar makalemizin konusunu şu an için aşmaktadır. Biz bu makalemizde temel olarak Xslt'yi tanıyacak ve.Net Framework 2.0 içinde etkin bir şekilde nasıl kullanabileceğimizi görmeye çalışacağız. İlk olarak aşağıdaki örnek xml dökümanını ele alalım.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Muzikler>
    <Muzik ID="1">
        <Soyleyen>Coldplay</Soyleyen>
        <AlbumAdi>X and Y</AlbumAdi>
        <CikisTarihi>2005</CikisTarihi>
        <Fiyat>28</Fiyat>
        <Tip>CD</Tip>
    </Muzik>
    <Muzik ID="2">
        <Soyleyen>Depeche Mode</Soyleyen>
        <AlbumAdi>Playing The Angel</AlbumAdi>
        <CikisTarihi>2005</CikisTarihi>
        <Fiyat>25</Fiyat>
        <Tip>CD</Tip>
    </Muzik>
.
.
.
<Muzikler>
```

MuzikDukkanim.xml isimli fiziki dosyada tutulan bu içerikte Muzikler isimli root node (ana boğum) altında Muzik isimli child node'lar (alt boğumlar) yer almaktadır. Muzik boğumu içerisinde ID isimli bir attribute (nitelik) ve Soyleyen, AlbumAdi, CikisTarihi, Fiyat, Tip isimli alt boğumlar yer almaktadır. Bu dökümanı herhangibir tarayıcı üzerinde açarsak aşağıdakine benzer bir ekran görüntüsü elde ederiz.

![mk168_2.gif](/assets/images/2006/mk168_2.gif)

Gördüğünüz gibi bu Xml içeriği her ne kadar bir geliştirici (developer) için anlamlıda olsa, son kullanıcı için aynı şeyi söylemek pek doğru olmayacaktır. Bu içeriği örneğin bir Html çıktısı haline getirebiliriz. Böylece son kullanıcı için daha okunabilir bir yapı sağlamış oluruz. Xslt kendi içerisinde Html takılarına izin verdiği için üretilecek çıktının görsel formatını istediğimiz gibi ayarlayabiliriz. Aşağıdaki örnek Xslt içeriği bu amaçla yazılmıştır.

XstlForHtml.xsl dökümanı içeriği;

```xml
<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:template match="/">
    <html>
    <body>
    <h1>Müzik Dükkanım</h1>
        <table border="1" borderColor="Black" cellpadding="0" cellspacing="0">
            <tr bgcolor="#FFCC66">
                <th>ID</th>
                <th>Söyleyen</th>
                <th>Albüm Adı</th>
                <th>Çıkış Tarihi</th>
                <th>Satış Fiyatı</th>
                <th>Cd/Dvd/Kaset</th>
            </tr>
    <xsl:for-each select="Muzikler/Muzik">
            <tr>
                <td>
                    <xsl:value-of select="@ID"/>
                </td>
                <td>
                    <font size="2" color="Blue">
                        <xsl:value-of select="Soyleyen"/>
                    </font>
                </td>
                <td>
                    <b>
                        <xsl:value-of select="AlbumAdi"/>
                    </b>
                </td>
                <td align="right">
                    <xsl:value-of select="CikisTarihi"/>
                </td>
                <td>
                    <b>
                        <xsl:value-of select="Fiyat * 1.5"/> Ytl
                    </b>
                </td>
                <td>
                    <xsl:value-of select="Tip"/>
                </td>
            </tr>
    </xsl:for-each>
    </table>
    </body>
    </html>
</xsl:template>
</xsl:stylesheet>
```

Bir Xslt içeriği ilk zamanlarda geliştiricilere korkutucu gelebilir. Ancak adım adım incelendiğinde son derece kolay ve anlaşılırdır. Xslt dökümanları mutlaka stylesheet elemanı ile başlar ve biterler. Bu elemanda Xslt dökümanının standardlarında yer alan isim alanı (namespace) ve versiyon numarası (version number) gibi bilgiler yer alır. Dökümanımızın içeriğinde yer alan en önemli kısımlardan birisi boğumudur. Xslt dökümanlarının, kaynak olarak gösterildiği xml dökümanı üzerinde dönüştürme işlemi yapabilmesi için burada bir şablon (template) eşleştirilmesinde bulunması gerekir.

Bu eşleştirme işleminde ilgili kümeyi seçebilmek için XPath ifadeleri kullanılır. Dikkat ederseniz template elemanında (element) match değeri / olarak belirlenmiştir. Bu, Xslt'yi uygulanan Xml dökümanındaki tüm boğumların (nodes) dönüştürme işleminde ele alınacağını belirtir. Dönüştürme işleminin hangi üyelere, nasıl uygulanacağı ve / şablonuna uyan parçaların çıktıda nasıl gözükeceğine dair gerekli format ayarları ise template elemanına ait boğum içerisinde yapılmaktadır.

Xslt dökümanımızda yer alan bir diğer önemli eleman (element) boğumunda geçen for-each ' tir. for-each elemanı sayesinde select niteliğinde (attribute) belirtilen küme üzerinde ileri yönlü hareket edebiliriz. Dikkat ederseniz select niteliğinin değeri Muzikler/Muzik olarak belirlenmiştir. Bu da tipik bir XPath ifadesidir. for-each elemanı programlamada kullandığımız foreach döngüsünden farksızdır. Bu eleman sayesinde Muzikler isimli ana boğum (root node) içerisindeki tüm Muzik alt boğumlarını (child nodes) dolaşacak bir ifade elde etmiş oluruz.

Dikkat ederseniz Xslt dökümanı içerisinde yer yer Html takıları geçmektedir. Eğer Html'e aşina iseniz, burada bir tablo oluşturulduğunu, başlıklarının, arka plan renginin vb. belirlendiğini görebilirsiniz. Peki ilgili Html tablosunun satırlarında (TR) yer alan sütunlar (TD) içerisine gelecek olan değerleri bir Xml dökümanı içerisinden nasıl almaktayız? Yani Xml içeriğindeki hangi alanların nereye geleceğini nasıl belirtebiliriz? Bunun için Xslt'nin value-of elemanından yararlanmaktayız.

Örneğin, boğumu, Xslt dökümanının uygulandığı Xml içeriğinde yer alan Muzikler/Muzik boğumu (node) altındaki Tip isimli alt boğumun (child nodes) içeriğini bulunduğu yere yazdıracak işlevselliği sağlamaktadır. Gelelim yazmış olduğumuz bu Xslt dökümanının ilgili Xml içeriğine nasıl uygulanacağına. Bunun için xml dökümanımızın başına xsl-stylesheet işlemci direktifini (processing directive) uygulamamız gerekmektedir.

![mk168_4.gif](/assets/images/2006/mk168_4.gif)

Bu işlemin ardından MuzikDukkanim.xml dosyasını herhangibir tarayıcı penceresinde açarsak aşağıdakine benzer bir çıktı elde ederiz.

![mk168_3.gif](/assets/images/2006/mk168_3.gif)

Görüldüğü üzere son kullanıcı için çok fazla anlam ifade etmeyen bir Xml içeriği, Xslt kullanılarak çok daha düzenli ve okunabilir bir Html formatına dönüştürülmüştür. Xslt dönüştürme işlemi sırasında kullanabileceğimiz başka fonksiyonelliklerde vardır. Örneğin, sonuç olarak elde ettiğimiz kümede sıralama işlemleri yaptırabilir veya çeşitli koşullar uygulatabiliriz. Örneğin, MuzikDukkanim.xml dosyasının içeriğinde AlbumAdi alanına göre tersten sıralatma yapmak için Xslt dökümanımızda sort elemanını aşağıdaki gibi kullanmamız yeterli olacaktır.

```xml
<xsl:for-each select="Muzikler/Muzik">
    <xsl:sort select="AlbumAdi" order="descending" data-type="text"/>
```

Sort elemanı (element) select niteliği (attribute) ile hangi alana göre sıralama yapılacağını belirtmektedir. order niteliğinde sırlamanın yönünü belirtiyoruz (ki burada descending ile tersten sıralama yapacağımızı belirttik). data-type niteliği ise sıralama kriteri için ele alınacak veri tipini belirtiyor. AlbumAdi alanı string bazlı bir içeriğe sahip olduğundan sıralamanında karakter bazlı olması, bir başka deyişle text tipinde olması gerekiyor. Eğer sayısal bir sıralama söz konusu ise data-type değerini number olarak belirtmemiz gerekecektir. Bu değişikliklerden sonra xml dökümanımızı yeniden bir tarayıcı penceresinde açarsak aşağıdakine benzer bir ekran görüntüsü elde ederiz.

![mk168_5.gif](/assets/images/2006/mk168_5.gif)

Sıralama dışında karşılaştırma ifadelerinide ele alabiliriz. Bunun için çoğunlukla Xslt'nin if yada choose elemanlarından faydalanılır. If elemanını özellikle filtreleme yaparken kullanabiliriz. Bu eleman içerisine dahil edilen value-of elemanları belirtilen koşula uyuyorsa çıktı olarak elde edilebilir. Örneğin Xml içeriğimizde Tip alanının değeri DVD olanları listelemek istediğimizi düşünelim. Bu durumda Xslt dökümanımızda for-each elemanı içerisinde yer alan value-of elemanlarını if elemanına ait boğumlar (nodes) arasına almamız gerekecektir.

```xml
<xsl:for-each select="Muzikler/Muzik">
    <xsl:sort select="AlbumAdi" order="descending" data-type="text"/> 
        <xsl:if test="Tip='DVD'">
            <tr>
                <td><xsl:value-of select="@ID"/></td>
                <td><font size="2" color="Blue"><xsl:value-of select="Soyleyen"/></font></td>
                <td><b><xsl:value-of select="AlbumAdi"/></b></td>
                <td align="right"><xsl:value-of select="CikisTarihi"/></td>
                <td><b><xsl:value-of select="Fiyat * 1.5"/> Ytl</b></td>
                <td><xsl:value-of select="Tip"/></td>
            </tr>
        </xsl:if>
</xsl:for-each>
```

![mk168_6.gif](/assets/images/2006/mk168_6.gif)

Dikkat ederseniz Tip alanında DVD yazan elemanları elde ettik. If elemanı bunu gerçekleştirebilmek için test isimli niteliğini kullanmaktadır. Elbette burada mantıksal operatorleri kullanarak çoklu karşılaştırmalarda yapabiliriz. Örneğin tipi Dvd veya Cd olanları elde etmek istediğimizde if elemanını şu şekilde kullanabiliriz.

```xml
<xsl:if test="Tip='DVD' or Tip='CD' ">
```

If dışında kullanılabilecek karşılaştırma komutlarından biriside choose elemanıdır. Choose aslında swicth-case'e benzeyen bir yapıdadır. Örneğin Xml içeriğimizde Satış Fiyatı 35 Ytl'den küçük olanların arka plan rengini yeşil, 35 Ytl ile 55 Ytl arasında olanların arka plan rengini mavi ve kalan kısıma ait arka plan renklerinide kırmızı yapmak istediğimizi düşünelim. Üstelik bu işlemde sadece Dvd ve Cd tiplerini ele almak istediğimizi varsayalım. Bu durumda Xslt içeriğimizde aşağıdaki değişiklikleri yapmamız yeterli olacaktır.

```xml
<xsl:for-each select="Muzikler/Muzik">
    <xsl:sort select="AlbumAdi" order="descending" data-type="text"/> 
        <xsl:if test="Tip='DVD' or Tip='CD' ">
            <tr>
            <td><xsl:value-of select="@ID"/></td>
            <td><font size="2" color="Blue"><xsl:value-of select="Soyleyen"/></font></td>
            <td><b><xsl:value-of select="AlbumAdi"/></b></td>
            <td align="right"><xsl:value-of select="CikisTarihi"/></td>
            <xsl:choose>
                <xsl:when test="Fiyat >35 and Fiyat <55">
                    <td bgColor="Blue"><font color="White" size="3"><b><xsl:value-of select="Fiyat"/> ytl</b></font></td>
                </xsl:when>
                <xsl:when test="Fiyat <35">
                    <td bgColor="Green"><font color="White" size="3"><b><xsl:value-of select="Fiyat"/> ytl</b></font></td>
                </xsl:when>
                <xsl:otherwise>
                    <td bgColor="Red"><font color="White" size="3"><b><xsl:value-of select="Fiyat"/> ytl</b></font></td>
                </xsl:otherwise>
            </xsl:choose>
            <td><xsl:value-of select="Tip"/></td>
            </tr>
        </xsl:if>
</xsl:for-each>
```

![mk168_7.gif](/assets/images/2006/mk168_7.gif)

Dikkat ederseniz choose elemanı içerisinde when ve otherwise isimli iki alt eleman kullanılmıştır. When elemanı if elemanı gibi test isimli bir niteliğe sahiptir ki buraya koşulumuzu yazarız. (Choose elemanı içerisinde birden fazla when ifadesi kullanılabilir.) otherwise ise when elemanında belirtilen koşullara uymayan durumlarda devreye girmektedir.

Şu ana kadar geliştirdiğimiz örneklerde Xslt uyguladığımız Xml içeriğini görmek için, tarayıcı pencersinde ilgili xml dosyasını açma yolunu seçtik. Lakin Asp.Net uygulamalarında çok sık Xml verileri ile çalışmaktayız. Bu nedenle bir Xslt dökümanını herhangibir Xml içeriğine, yönetimli kod (managed code) tarafında da uyarlayabilmeliyiz. Framework bir Xml dökümanını bir Xslt dökümanı ile çarpıştırmak için pek çok kullanışlı tip içermektedir. Framework 1.1' de transform işlemleri için XslTransform tipi kullanılmaktaydı. Bu tip Framework 2.0 içinde geçerlidir. Ancak modası geçmiş (obsolute) olarak kabul ediliyor ve yerine gelen yeni XslCompiledTransform tipinin kullanılması öneriliyor. Kaynaklarda yeni gelen XslCompiledTransform tipinin eskisine göre çok daha iyi bir performans sağladığı söylenmekte. Aşağıdaki kod parçası tipik olarak yukarıda gerçekleştirdiğimiz işlemlerin yönetimli (managed) tarafta nasıl yapılabileceğini içeren basit bir kod parçası sunmaktadır.

```csharp
XslCompiledTransform xsltran = new XslCompiledTransform();
xsltran.Load(Server.MapPath("XsltForHtml.xsl"));
xsltran.Transform(Server.MapPath("MuzikDukkanim.xml"), null, Response.Output);
```

XslCompiledTransform tipinin iki önemli üyesi Load ve Transform metodlarıdır. Load metodu ile dönüştürme işleminde kullanılacak olan kaynak Xslt dökümanı belirtilir. Load metodunun altı adet aşırı yüklenmiş (overload) versiyonu vardır. Yukarıdaki örnek kod parçasında Xsl dosyasının bulunduğu yol (path) bilgisi kullanılmıştır. Transform işlemleri sırasında eğer performans önemli ise Load ve Transform medorlarında IXPathNavigable arayüzü (interface) tipinden parametre alan versiyonlarının kullanılması önerilmektedir. Aşağıdaki kod parçasında IXPathNavigable tipine atanabilen XPathDocument referansları kullanılmıştır.

```csharp
XslCompiledTransform xsltran = new XslCompiledTransform();
XPathDocument doc = new XPathDocument(Server.MapPath("MuzikDukkanim.xml"));
XPathDocument xslDoc = new XPathDocument(Server.MapPath("XsltForHtml.xsl"));
xsltran.Load(xslDoc);
xsltran.Transform(doc, null, Response.Output);
```

Hangi teknik kullanılırsa kullanılsın, dönüştürme işlemini Transform isimli metod gerçekleştirmektedir. Transform metodunun ondört farklı versiyonu bulunmaktadır. Bizim kullandığımız örneklerdeki versiyonlarda ilk parametre olarak dönüştürme işleminin uygulanacağı Xml dökümanı belirtilmektedir. İkinci parametrede ise, ortamdan Xslt dökümanına göndereceğimiz çeşitli parametreler varsa bunlara ait bilgilere yer verilir. Biz şu anki kod parçamızda parametre göndermediğimizden burayı null olarak belirttik. Son parametre ise dönüştürülen çıktının nereye yapılacağını belirtmektedir. Bizim örneğimizde bu çıktı web sayfasının html içeriğine doğru yapılmaktadır ki bu parametre bir stream de alabilir. Bu da çıktının fiziki bir kaynağa doğru yapılabileceğini gösterir. Yukarıdaki kod parçalarından herhangibirini örnek bir web uygulamasında kullandığımızda aşağıdakine benzer bir çıktıyı o anki güncel aspx sayfası üzerinde elde edebiliriz.

![mk168_8.gif](/assets/images/2006/mk168_8.gif)

Bazı durumlarda Xslt dökümanı içerisinde değerini dış ortamdan alacak parametreler kullanmak isteyebiliriz. Örneğin geliştirdiğimiz Xslt dosyasında fiyatı dış ortamdan gelen değerden büyük olanların üzerinde bir koşul çalıştırmak istediğimizi düşünelim. Öncelikli olarak dış ortamdan gelecek olan parametreyi Xslt dökümanımıza bildirmeli, onu içeride kullanmalı ve daha sonra yönetimli taraftan bu parametre değerini aktarmalıyız. İlk olarak içeride kullanacağımız parametreyi ekleyerek işe başlayalım. Bunun için param elemanından aşağıdaki gibi faydalanabiliriz.

![mk168_9.gif](/assets/images/2006/mk168_9.gif)

Param elemanına ait name niteliği parametre adını belirtir. select niteliğinde ise bu parametre için varsayılan bir değer kullanılır. Bu parametreyi örnek olarak when elemanı içerisinde kullanabiliriz.

![mk168_10.gif](/assets/images/2006/mk168_10.gif)

Peki bu parametreye yönetimli kod içerisinden nasıl değer göndereceğiz? Söz konusu parametre XslCompiledTransform sınıfının Transform metodu için aslında bir argümandır. Xslt dökümanı içerisinde birden fazla parametre tanımlanmış olabilir. Tüm bu parametreler ve değerleri XsltArgumentList isimli bir tip ile saklanırlar. Aşağıdaki kod parçasında web sayfasındaki bir TextBox kontrolünden alınan sayısal değer, yukarıda tanımlamış olduğumuz Price isimli parametreye değer olarak aktarılmaktadır. Dikkat ederseniz XsltArgumentList tipine ait nesne örneğimize AddParam metodu ile Price parametresi eklenmiş ve değeri verilmiştir. Sonrasında ise Transform metodunun ikinci parametresine argList isimli nesnemiz bildirilmiştir.

```csharp
private void Parametric(decimal price)
{
    XslCompiledTransform xsltran = new XslCompiledTransform();
    xsltran.Load(Server.MapPath("XsltForHtml.xsl"));
    XsltArgumentList argList = new XsltArgumentList();
    argList.AddParam("Price", "", price);
    xsltran.Transform(Server.MapPath("MuzikDukkanim.xml"), argList, Response.Output);
}
```

![mk168_11.gif](/assets/images/2006/mk168_11.gif)

Bazı durumlarda Xslt içerisinden dış ortama parametre aktarıp bir takım hesaplamalar yaptırmak isteyebiliriz. Bu tipik olarak, Xslt dökümanı içerisinden dışarıdaki bir metodu çağırmak anlamına gelmektedir. Dolayısıyla, Xslt dökümanı içerisinde kapsülleyemiyeceğimiz bir takım kodları, dış ortamda ele alabilir ve Xslt dökümanı içerisinden buraya parametre gönderebiliriz. İlk olarak aşağıdaki gibi basit bir sınıf tasarlayalım.

```csharp
public class PriceManager
{
    public decimal LastPrice(string fiyat)
    {
        decimal fyt = Convert.ToDecimal(fiyat);
        return fyt - 1;
    }
}
```

Bu sınıf içerisinde yer alan LastPrice isimli metod bir parametre almaktadır. İşte biz bu parametrenin değerini Xslt dökümanımız içerisinden göndereceğiz. Böylece Xslt dökümanının ele aldığı alan değerini ele alacak iş mantığını kendi yazdığımız bir tip içerisine almış bulunuyoruz. Metodun dönüş değeri ise, Xslt dökümanında bu metodu çağırdığımız yerde kullanılabilecektir. Bu aşamadan sonra kodlarımızıda aşağıdaki gibi değiştirmemiz gerekiyor.

```csharp
XslCompiledTransform xsltran = new XslCompiledTransform();
XPathDocument doc = new XPathDocument(Server.MapPath("MuzikDukkanim.xml"));
XPathDocument xslDoc = new XPathDocument(Server.MapPath("XsltForHtml.xsl"));
XsltArgumentList argLst = new XsltArgumentList();
PriceManager tip = new PriceManager();
argLst.AddExtensionObject("urn:MyType", tip);
xsltran.Load(xslDoc);
xsltran.Transform(doc, argLst, Response.Output);
```

Burada yine başrol oyuncumuz XsltArgumentList tipidir. Ancak bu sefer, Xslt dökümanının dış ortama gönderdiği parametreyi işleyecek ve kullanacak bir nesne söz konusudur. Bu sebepten AddExtensionObject metodu kullanılmıştır. Bu metod ilk parametre olarak bir xml namespace (isim alanı) bilgisi almaktadır. İkinci parametre ise, Xslt dökümanının kullanacağı tipe ait nesne örneği referansıdır. Gelelim Xslt dökümanımıza. Burada herşeyden önce urn:MyType isim alanının belirtilmesi gereklidir. Bunun için stylesheet elemanına bir isim alanı bildirimini aşağıdaki gibi eklememiz gerekecektir.

```xml
<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:MyType="urn:MyType">
```

Artık isim alanını tanımladığımıza göre, Xslt dökümanımız içerisinde PriceManager tipine ait nesne örneğini ve üye metodunu çağırabilir ve parametre gönderebiliriz. Aşağıdaki örnek kullanım bu işin nasıl yapılabileceğini göstermektedir.

```xml
<xsl:when test="Fiyat <=$Price">
    <td bgColor="Green">
        <font color="White" size="3"><b> <xsl:value-of select="MyType:LastPrice(Fiyat)"/> ytl</b></font>
    </td>
</xsl:when>
```

Dikkat ederseniz value-of elemanı içerisinde yer alan select niteliğinde metod çağırısı yapılmıştır. MyType isim alanı kod tarafında tip isimli PriceManager sınıfına ait nesne örneğini işaret etmektedir. Dolayısıya bu bilgi üzerinden kolayca LastPrice metoduna erişilmiş ve Fiyat isimli Xml alanının değeri parametre olarak gönderilebilmiştir. Elbetteki metodun dönüş değeri, select elemanının bulunduğu hücreye yazılacaktır.

![mk168_12.gif](/assets/images/2006/mk168_12.gif)

Bu makalemizde kısaca Xslt'nin ne olduğunu, ne işe yaradığını,.Net Framework içerisinden nasıl kullanılabileceğini kısaca incelemeye çalıştık. Aslında Xslt başlı başına bir konudur ve ayrıca zaman ayrılması gerekmektedir. Xml, Xslt, Xpath gibi popüler konuların.Net ile olan ilişkilerini daha iyi anlayabilmek için size Wrox'un [Professional ASP.NET 2.0 XML (Programmer to Programmer)](http://www.amazon.com/gp/product/0764596772/sr=8-5/qid=1153400205/ref=sr_1_5/103-4704336-3222215?ie=UTF8) kitabını tavsiye ederim. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama İçin Tıklayın.](/assets/files/2006/UsingXSLT.rar)
