---
layout: post
title: "XML Verilerini XSD ile Yönetimli Kod Üzerinden Doğrulamak"
date: 2006-09-01 09:00:00 +0300
categories:
  - xml
tags:
  - xml
  - xsd
---
Xml içeriğini kullandığımız pek çok platformda, verinin belirli kurallara göre yazılmış olmasını istediğimiz durumlar söz konusu olabilir. Bu durum özellikle, farklı platformlar arasında taşınacak Xml tabanlı verilerin aynı kurallar dizisine uygun olacak şekilde kullanılması istendiği durumlarda karşımıza çıkmaktadır. Xml verilerinin belirli kurallara göre doğruluğunun tespitinde şu anda DTD (Document Type Definitions), XDR (Xml Data Reduced) ve XSD (Xml Schema Definitions) gibi teknolojilerden yararlanılmaktadır.

Bu teknolojiler yardımıyla bir Xml verisi içerisindeki elemanlar üzerinde çeşitli kurallar tanımlayabiliriz. Örneğin tutulan veri tiplerini sınırlayabilir, Xml ağacının yapısını tanımlayabiliriz vb. Böylece verilerin tutarlılığınıda sağlamış oluruz. Günümüzde, Xml verilerinin doğrulanması için kullanılan en yaygın teknoloji XSD ' dir. XSD'yi daha önceki türevi olan XDR'ın mükemmelleştirilmiş hali olarakta düşünebiliriz. Zaten WC3' da XSD şemalarının kullanılmasını önermektedir. Biz bu makalemizde.Net tarafında yer alan yönetimli tipleri (Managed Types) kullanarak Xml verilerini XSD ile çarpıştırarak doğrulama işlemlerini nasıl yapabileceğimizi incelemeye çalışacağız. İlk olarak aşağıdaki gibi bir Xml içeriğimiz olduğunu düşünelim.

```csharp
<?xml version="1.0" encoding="utf-8"?>
<Dukkanim>
    <Kitap ID="1000">
        <Adi>Her Yönüyle C#</Adi>
        <Fiyat>Elli YTL</Fiyat>
        <StokDurumu>40</StokDurumu>
        <Yazarlar>
            <Yazar>Sefer Algan</Yazar>
        </Yazarlar>
        <BasimTarihi>10/11/2003</BasimTarihi>
        <Aciklama>C# ile ilgili yazılmış en iyi Türkçe kaynaklardan birisidir. </Aciklama>
    </Kitap>
    <Kitap ID="1001">
        <Adi>Kahraman Asker</Adi>
        <Fiyat>30</Fiyat>
        <StokDurumu>Bin Adet</StokDurumu>
        <Yazarlar>
            <Yazar>Anonim</Yazar>
        </Yazarlar>
        <BasimTarihi>10/11/2004</BasimTarihi>
        <Aciklama>C# ile ilgili yazılmış en iyi Türkçe kaynaklardan birisidir. </Aciklama>
    </Kitap>
</Dukkanim>
```

Bu Xml içeriğinde Kitaplar ile ilgili bir takım bilgileri tutmaktayız. Örneğin Kitabın adını, fiyatını, stoktaki durumunu, basım tarihi vb. Bu Xml dökümanını herhangibir tarayıcı yardımıyla açtığımızda geliştiriciler için okunabilir ve anlaşılabilir bir içerik elde ederiz.

![mk173_1.gif](/assets/images/2006/mk173_1.gif)

Lakin bu Xml içeriğinin platformlar arasında taşındığını ve işlendiğini düşündüğümüzde hataya neden olacak pek çok problemin olduğunu görebiliriz. Örneğin Fiyat alanlarında bir standart yoktur. Öyleki ilk Kitap elemanı (element) içerisinde Fiyat değeri Elli YTL olarak string bazlı yazılmışken, ikinci Kitap elemanındaki Fiyat elemanının değeri 30 olarak sayısal belirtilmiştir. Başka bir deyişle Fiyat alanının aslında uygun bir veri tipi olma zorunluğu yoktur. Dolayısıla bu Xml dökümanını alıp fiyatlar üzerinde sayısal işlemler yapacak olan bir uygulama kodu sorunlar ile karşılaşacaktır.

Benzer durum StokDurumu elemanı içinde geçerlidir. Bu tip veri uyuşmazlıklarını şemalarda yapacağımız tanımlamalar ile düzeltebiliriz. Diğer taraftan dikkat ederseniz her Kitap elemanı içerisinde Yazarlar isimli bir eleman bulunmaktadır ki bu elemanda en az bir Yazar elemanını içermelidir. Bu da bir kural olarak şema bilgisi içerisine alınabilir. XSD şemalarını sıfırdan herhangibir editor kullanmadan yazmak her zaman keyfi verici bir iş olmayabilir. Lakin Vs.Net 2003 sürümünden bu yana, görsel olaraktan XSD şemalarını kolayca hazırlayabilmemizi sağlayan bir arabirim sunmaktadır. Buna göre yukarıdaki Xml dökümanımız için aşağıdaki gibi bir XSD şemasını kolayca hazırlayabiliriz. (XSD'nin öğrenilebilecek daha çok özelliği olduğundan ve makalemizin sınırlarını aştığından çok fazla derinine inmeyeceğiz.)

![mk173_2.gif](/assets/images/2006/mk173_2.gif)

```xml
<?xml version="1.0" encoding="utf-8"?>
<xs:schema attributeFormDefault="unqualified" elementFormDefault="qualified" xmlns:xs="http://www.w3.org/2001/XMLSchema">
    <xs:element name="Dukkanim">
        <xs:complexType>
            <xs:sequence>
                <xs:element name="Kitap" minOccurs="1" maxOccurs="unbounded">
                    <xs:complexType>
                        <xs:sequence>
                            <xs:element name="Adi" type="xs:string" minOccurs="1" maxOccurs="1"/>
                            <xs:element name="Fiyat" type="xs:double" minOccurs="1" maxOccurs="1"/>
                            <xs:element name="StokDurumu" type="xs:integer" minOccurs="1" maxOccurs="1"/>
                            <xs:element name="Yazarlar" minOccurs="1" maxOccurs="1">
                                <xs:complexType>
                                    <xs:sequence>
                                        <xs:element name="Yazar" type="xs:string" minOccurs="1" maxOccurs="unbounded" />
                                    </xs:sequence>
                                </xs:complexType>
                            </xs:element>
                            <xs:element name="BasimTarihi" type="xs:dateTime" minOccurs="1" maxOccurs="1" />
                            <xs:element name="Aciklama" type="xs:string" minOccurs="1" maxOccurs="1" />
                        </xs:sequence>
                        <xs:attribute name="ID" type="xs:integer" use="required" />
                     </xs:complexType>
                </xs:element>
            </xs:sequence>
        </xs:complexType>
    </xs:element>
</xs:schema>
```

İlk olarak şema dosyamızda belirttiğimiz kuralları kısaca incelemeye çalışalım. Örneğin Adi, Fiyat, StokDurumu, Yazarlar, BasimTarihi, Aciklama gibi elemanlardan sadece ve yanlız bir adet girilebileceği belirtilmektedir. Bu kural minOccurs ve maxOccurs isimli özellikler ile belirtiliyor. Dolayısıyla Yazarlar isimli elemanımız, kendi içerisinde en az bir olacak şekilde istenildiği kadar (unbounded sayesinde) Yazar elemanı içerebilir. XSD dökümanımız içerisinde yer alan her eleman (ve hatta ID isimli attribute-nitelik) için ayrı veri tipi tanımlamaları yapılmıştır. Örneğin ID isimli attribute (nitelik) integer tipinden olmalıdır. Fiyat isimli elemanımız double türünden olmalıdır vb.

Peki bu ve benzeri şema bilgisini (bilgilerini) kullanarak yazdığımız Xml içeriğini yönetimli kod tarafından (managed-code) nasıl denetleyebiliriz?.Net 2.0 içerisinde bu tip işlemler için getirilmiş yeni tipler vardır. Bunlar XmlReaderSettings, XmlSchemaSet isimli tiplerdir. Konuyu daha iyi anlayabilmek için örnek kodlar ile devam etmemizde fayda olduğu kanısındayım. Aşağıdaki kod parçasında bir Asp.Net formu üzerinde, Dukkan.xml isimli Xml dökümanının Dukkan.xsd isşmli xml şema dökümanı ile doğrulanmasının nasıl yapılabileceği gösterilmektedir.

```csharp
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Text;

public partial class _Default : System.Web.UI.Page 
{
    StringBuilder hatalar;

    protected void Page_Load(object sender, EventArgs e)
    {
        string xmlFile = Server.MapPath("Dukkan.xml");
        string XSDFile =Server.MapPath( "Dukkan.XSD");
        hatalar = new StringBuilder();

        XmlReaderSettings xrs = new XmlReaderSettings();
        xrs.ValidationEventHandler += new System.Xml.Schema.ValidationEventHandler(xrs_ValidationEventHandler);
        xrs.Schemas.Add(null, XmlReader.Create(XSDFile));
        xrs.ValidationType = ValidationType.Schema;
        XmlReader reader = XmlReader.Create(xmlFile, xrs);
        while (reader.Read())
        {}
        if (hatalar.ToString() == "")
            Response.Write("Hata yok");
        else
            Response.Write(hatalar.ToString());
    }

    void xrs_ValidationEventHandler(object sender, System.Xml.Schema.ValidationEventArgs e)
    {
        if (e.Severity == System.Xml.Schema.XmlSeverityType.Error)
            hatalar.Append(e.Message+"<br><br>");
    }
}
```

Örneğimizi çalıştırdığımızda aşağıdaki ekran görüntüsünde yer alan hata mesajlarını elde ederiz.

![mk173_3.gif](/assets/images/2006/mk173_3.gif)

Dikkat ederseniz XSD şemasında belirtilen kurallara uymayan durumlar için çeşitli hata mesajları üretilmiş ve ekrana yazılmıştır. Kodumuzdaki kilit nokta XmlReaderSettings isimli tipin XmlReader tipi ile entegrasyonudur. XmlReaderSettings sınıfına ait tek olay (event) olan ValidationEventHandler, doğrulaması yapılan Xml dökümanı içerisinde meydana gelen hatalar (errors) yada uyarılar (Warnings) sonucunda otomatik olarak tetiklenmektedir. Xml verisini kontol etmek için kullanılacak XSD şemaları, XmlReaderSettings sınıfına Schemas özelliği yardımıyla yüklenir.

Schemas özelliği.Net 2.0 ile gelen yeni tiplerden birisi olan XmlSchemaSet tipinden nesne örnekleri ile de çalışabilmektedir. Burada önemli olan noktalardan birisi Schemas özelliğinin aslında bir koleksiyon sunması ve Add metodu nedeni ile XmlReaderSettings için birden fazla şema bilgisinin bir arada tutulabilmesidir. Öyleki farklı xml isim alanlarına ait farklı sayıda şema bilgisini XmlReaderSettings tipleri içerisinde barındırabiliriz. XmlReaderSettings sınıfına ait bir diğer önemli üye ise ValidationType özelliğidir. Bu özelliği atadığımız değer ile (ValidationType.Schema), XSD şema tipine göre doğrulama yapacağımızı belirtmiş oluruz. Kodun ilerleyen kısımlarındaki adımlarımızda ise XmlReader sınıfımıza ait nesnemizi Create isimli statik metod ile oluşturmaktayız.

Örnek kodumuzda XmlReader sınıfına ait nesne örneği ile okuma yapmak için gerekli reader isimli nesneyi oluştururken static Create metodunun ilk parametresi olarak Xml dökümanımızı, ikinci parametresi olarakta doğrulama işlemleri ile ilgili bilgileri içeren XmlReaderSettings nesne örneğimizi belirtmekteyiz. Bu işlemlerin ardından tek yapılması gereken XmlReader nesne örneğinin taşıdığı içeriği ileri yönlü okumaktır. Bu amaçla kullandığımız while döngüsü ile Xml içeriğinde ileri yönlü boş bir öteleme gerçekleştirirken, XSD dosyamızın içerdiği şemada belirtilen kurallara uymayan her noktada, ValidationEventHandler olayı tetiklenir. Bizde bu olaya ilişkin metodumuz içerisinde doğrulama sonucu ortaya çıkan hataları, ValidationEventArgs tipi ile ele alabiliriz. Örnek olarak incelediğimiz Xml dökümanımızı aşağıdaki gibi düzenlersek (yani şema hatalarını ortadan kaldırırsak) uygulamamız doğrulamayı başarılı bir şekilde geçecektir.

```xml
<?xml version="1.0" encoding="utf-8"?>
<Dukkanim>
    <Kitap ID="1000">
        <Adi>Her Yönüyle C#</Adi>
        <Fiyat>50</Fiyat>
        <StokDurumu>40</StokDurumu>
        <Yazarlar>
            <Yazar>Sefer Algan</Yazar>
        </Yazarlar>
        <BasimTarihi>2003-10-11T12:00:00</BasimTarihi>
        <Aciklama>C# ile ilgili yazılmış en iyi Türkçe kaynaklardan birisidir. </Aciklama>
    </Kitap>
    <Kitap ID="1001">
        <Adi>Kahraman Asker</Adi>
        <Fiyat>30</Fiyat>
        <StokDurumu>1000</StokDurumu>
        <Yazarlar>
            <Yazar>Anonim</Yazar>
        </Yazarlar>
        <BasimTarihi>2004-10-11T12:00:00</BasimTarihi>
        <Aciklama>C# ile ilgili yazılmış en iyi Türkçe kaynaklardan birisidir. </Aciklama>
    </Kitap>
</Dukkanim>
```

XSD ile doğrulama işlemleri özellikle Xml verisi üzerinde XmlDocument tipi ile çalışırken daha büyük önem taşır. XmlDocument sınıfı bildiğiniz gibi bir Xml içeriğini bellekte DOM (Document Object Model) sistemine göre referans eder. Buda ilgili döküman üzerinde yükleme zamanında bir doğrulama işleminin yapılmasını gerektirir. Eğer bir Xml verisini, XmlDocument sınıfına ait nesneler yardımıyla ele alıyorsak, döküman içerisine yapılacak müdahalelerinde belirleyeceğimiz şema kuralları çerçevesinde olmasını garanti etmek isteyebiliriz. Durumu daha iyi analiz etmek için aşağıdaki kod parçasını ele alalım.

```csharp
string xmlFile = Server.MapPath("Dukkan.xml");
string XSDFile =Server.MapPath( "Dukkan.XSD");
hatalar = new StringBuilder();

XmlDocument doc = new XmlDocument();
doc.Load(xmlFile);
XmlElement currElement =(XmlElement)doc.DocumentElement.SelectSingleNode("//Dukkanim/Kitap[@ID='1000']");
XmlNode newNode = doc.CreateNode(XmlNodeType.Element, "SayfaSayisi", null);
newNode.InnerText = "600";
currElement.AppendChild(newNode);

doc.Save(xmlFile);
```

Örnek kodumuzda, XmlDocument sınıfına ait doc isimli nesne örneğimizi Dukkan.xml dosyasına ait xml içeriği ile yükledikten sonra ID niteliğinin değeri 1000 olan node (boğum) kısmını elde ediyoruz. Sonrasında ise döküman üzerinde yeni bir eleman (element) oluşturuyoruz. Amacımız oluşturduğumuz yeni elemanı (element) 1000 ID numaralı Kitap boğumu altına eklemek. Bunun için SayfaSayisi isimli bu yeni xml elemanına bir değer atayıp chile node olarak dökümandaki ilgili boğumun altına ekliyoruz.

Bu aslında XSD şemasında tanımlanmamış bir eleman. Dolayısıylada eklenmemesi gereken bir eleman. Ancak XSD doğrulamasını hesaba katmadığımızdan bu yeni eleman Xml içeriğine gayet güzel bir şekilde eklenecektir. Öyleki örneğimizi çalıştırdığımızda Xml dökümanımızın içeriğinin aşağıdaki gibi değiştiğini görürüz. Her nekadar Vs.Net 2005 IDE'si bizi bu yeni eleman konusunda uyarsada, sonuç olarak değişiklik kod tarafından fiziki dosyaya aktarılmıştır.

![mk173_4.gif](/assets/images/2006/mk173_4.gif)

O halde gelin XSD doğrulamasını işin içerisine katıp bu değişikliklerin yapılmasını engelleyelim. Bunun için yine XmlReaderSettings ve XmlReader sınıflarından yararlanacağız. Ancak bu kez XmlDocument içerisinde doğrulama işlemini gerçekleştirmek için,başka bir deyişe bellekteki Xml verisi üzerinde doğrulama yapabilmek için, XmlNodeReader isimli sınıftan faydalanacağız. Bu nedenle uygulama kodumuzu aşağıdaki gibi değiştirelim.

```csharp
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Text;

public partial class _Default : System.Web.UI.Page 
{
    StringBuilder hatalar;

    protected void Page_Load(object sender, EventArgs e)
    {
        string xmlFile = Server.MapPath("Dukkan.xml");
        string XSDFile =Server.MapPath( "Dukkan.XSD");
        hatalar = new StringBuilder();

        XmlDocument doc = new XmlDocument();
        doc.Load(xmlFile);

        XmlElement currElement =(XmlElement)doc.DocumentElement.SelectSingleNode("//Dukkanim/Kitap[@ID='1000']");
        XmlNode newNode = doc.CreateNode(XmlNodeType.Element, "SayfaSayisi", null);
        newNode.InnerText = "600";
        currElement.AppendChild(newNode);

        XmlNodeReader nodeReader = new XmlNodeReader(doc);
        XmlReaderSettings readerSettings = new XmlReaderSettings();
        readerSettings.ValidationEventHandler+=new System.Xml.Schema.ValidationEventHandler(readerSettings_ValidationEventHandler);
        readerSettings.ValidationType= ValidationType.Schema;
        readerSettings.Schemas.Add(null,XmlReader.Create(XSDFile));
        XmlReader reader = XmlReader.Create(nodeReader, readerSettings);

        while (reader.Read())
        {}

        if (hatalar.ToString() == "")
            doc.Save(xmlFile);
        else
            Response.Write(hatalar.ToString()+"<b>Hatası Nedeniyle Xml Dökümanının yeni içeriği kayıt edilememiştir.</b>");
    
    }

    void readerSettings_ValidationEventHandler(object sender, System.Xml.Schema.ValidationEventArgs e)
    {
        if (e.Severity == System.Xml.Schema.XmlSeverityType.Error)
            hatalar.Append(e.Message + "<br><br>");
    }
}
```

![mk173_5.gif](/assets/images/2006/mk173_5.gif)

Dilerseniz uygulama kodumuzda neler yaptığımıza kısaca bakalım. Bu sefer XmlDocument sınıfına ait doc isimli nesne örneğinin işaret ettiği bellek görüntüsünü, ilgili XSD şeması ile kontrol edebilmek amacıyla XmlNodeReader sınıfından yardım alıyoruz. Bu nedenle XmlNodeReader sınıfımıza ait nodeReader isimli nesne örneğimizi doc parametresini vererek oluşturuyoruz. Her zamanki gibi, XSD şema dosyası (Dukkan.XSD) için gerekli ayarların taşınacağı XmlReaderSettings sınıfına ait nesne örneğimizi oluşturmaktayız. Burada dikkat edilmesi gereken noktalar; doğrulama tipi (ValidationType) seçimi, doğrulama sırasında hatalar veya uyarılar sonrası tetiklenecek olayın ve metodunun hazırlanması (ValidationEventHandler) ve XSD şema dosyasının Schemas koleksiyonuna yüklenişi (Schemas.Add).

XSD dökümanımız ile doc isimli nesnemizin bellekte işaret ettiği Xml içeriğini çarpıştırmak dolayısıyla şema denetimi altına doğrulama kontrollerini yapmak içinse, yine XmlReader sınıfı nesne örneğinden yararlanıyoruz. Ancak bu sefer ilk parametremiz, XmlNodeReader sınıfımıza ait nodeReader isimli nesne örneğimizdir. While döngüsü ile yaptığımız boş öteleme hareketi sırasında da oluşan hata mesajlarını topluyor ve hiçbir hata yok ise Xml dökümanımızı son haliyle kaydediyoruz (doc.Save). Aslında döngü içerisinde ilerlerken, XmlReader.Create metodunda belirtiğimiz kuralları uyguluyoruz. Yani XmlNodeReader'ın taşıdığı içerik üzerinde ilerlerken XSD yi taşıyan XmlReaderSettings'teki kurallara göre doğrulama kontrolleri gerçekleştiriyoruz. Böylece geldik bir makalemizin daha sonuna. Bu makalemizde kısaca XSD şemalarını, Xml dökümanları ile çarpıştırarak yapılan doğrulama işlemlerinde.Net yönetimli tiplerinin (managed code) nasıl kullanılabileceğini incelemeye çalıştık. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama İçin Tıklayın](/assets/files/2006/XmlValidations.rar)