---
layout: post
title: "Sql 2005 XML Veri Tipini XSD ile Doğrulamak"
date: 2006-04-21 12:00:00 +0300
description: "Sql Server 2005 ile birlikte gelen en büyük yeniliklerden birisi, yeni XML veri tipidir. XML veri tipini tablolarda alanlar, stored procedure' lerde ve fonksiyonlarda parametreler veya değişkenler için kullanabilmekteyiz. Ancak asıl iyi olan nokta, XML veri tipinden herhangibir içeriğin, XSD şemaları yardımıyla doğruluğunun kontrol altına alınabilmesidir. Bir XML şeması ile ilişkilendirilmiş ve doğruluğu bu şema bilgisinde verilen kriterlere göre sağlanacak olan XML verisine, türlendirilmiş XML (Typed XML) adı verilmektedir. (Tam karşıtı olan Untyped XML verisi ise sadece well-formed olarak tanımlanmış XML içeriğini işaret etmektedir.)"
categories:
  - t-sql
tags:
  - t-sql
  - xml
  - csharp
  - dotnet
  - sql-server
  - http
  - performance
  - visual-studio
---
# Sql 2005 XML Veri Tipini XSD ile Doğrulamak
Değerli Okurlarım Merhabalar,

Sql Server 2005 ile birlikte gelen en büyük yeniliklerden birisi, yeni XML veri tipidir. XML veri tipini tablolarda alanlar, stored procedure'lerde ve fonksiyonlarda parametreler veya değişkenler için kullanabilmekteyiz. Ancak asıl iyi olan nokta, XML veri tipinden herhangibir içeriğin, XSD şemaları yardımıyla doğruluğunun kontrol altına alınabilmesidir. Bir XML şeması ile ilişkilendirilmiş ve doğruluğu bu şema bilgisinde verilen kriterlere göre sağlanacak olan XML verisine, türlendirilmiş XML (Typed XML) adı verilmektedir. (Tam karşıtı olan Untyped XML verisi ise sadece well-formed olarak tanımlanmış XML içeriğini işaret etmektedir.)

> Typed XML verileri, çalışma zamanında Untyped XML verilerine göre daha yüksek performans sağlar. Çünkü Untyped XML içeriğinde, elementlere ve niteliklere ait veriler string formatında tutulmakta olup çalışma zamanında gereksiz yere tür dönüşümlerinin olmasına neden olmaktadır. Oysaki Typed XML verisinin içeriğinde yer alan element ve niteliklerin veri türleri zaten şemada belirtilen türlerden olmak zorunadır. Bu da çalışma zamanında gereksiz tür dönüşümlerini engelleyerek yüksek performans sağlamaktadır.

Peki Sql Server 2005 üzerinde, özellikle bir tablo alanının veri tipini XML olarak belirttiğimizde, bu alanın içeriğini bir XML şeması ile (XML Schema) nasıl ilişkilendirebiliriz. Herşeyden önce, XML veri tipi ile ilişkili olan şema bilgilerinin nasıl ve ne şekilde tutulduğunu bilmekte fayda var. Sql Server 2005 üzerinde bir XML şeması çoğunlukla bir şema koleksiyonuna (Schema Collection) eklenerek kullanılmaktadır. Diğer taraftan sistemde yer alan bir kaç tablo üzerinde de, bu şema içerisinde yer alan isim alanlarına (namespaces), elementlere (elements), niteliklere (attributes) vb... ait bilgiler saklanmaktadır. Dolayısıyla çalışma zamanında bir XML veri alanının içeriğini kontrol etmek için, Sql Server 2005 sistemine kayıt edilmiş (register) şema koleksiyonlarından faydalalanılmaktadır. Öncelikle bir şema bilgisini Sql Server 2005 sistemine nasıl kayıt edebileceğimize bakalım. Elimizde aşağıdaki gibi bir şema olduğunu düşünelim. (Var olan bir XML dökümanının şema bilgisini Visual Studio.Net ortamında Create Schema seçeneği yardımıylada oluşturabilirsiniz. Ben aşağıdaki örnek şemayı bu teknik ile oluşturdum ve Sql Server 2005 sistemine kayıt edilebilecek hale getirdim.)

![mk158_4.gif](/assets/images/2006/mk158_4.gif)

```xml
<?xml version="1.0" encoding="utf-16"?>
    <xs:schema attributeFormDefault="unqualified" elementFormDefault="qualified" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bsenyurt.com/Kitaplar" xmlns="http://www.bsenyurt.com/Kitaplar">
        <xs:element name="Kitaplar">
            <xs:complexType>
                <xs:sequence>
                    <xs:element name="Kitap">
                        <xs:complexType>
                            <xs:sequence>
                                <xs:element name="Ad" type="xs:string" />
                                <xs:element name="Fiyat" type="xs:int" />
                                <xs:element name="Basim" type="xs:date" />
                                <xs:element name="Yazarlar">
                                    <xs:complexType>
                                        <xs:sequence>
                                            <xs:element name="Yazar" type="xs:string" />
                                        </xs:sequence>
                                    </xs:complexType>
                                </xs:element>
                        </xs:sequence>
                        <xs:attribute name="ID" type="xs:unsignedShort" use="required" />
                    </xs:complexType>
                </xs:element>
            </xs:sequence>
        </xs:complexType>
    </xs:element>
</xs:schema>
```

Bu şema bilgisinde Kitaplar root elementi içerisinde yer alan Kitap elementi tipinden boğumlar (nodes) yer almaktadır. Her bir Kitap boğumu içerisinde ID isimli ve unsignedShort tipinden nitelikler (attributes) olmak zorundadır. Kitap boğumları (nodes) içerisinde string tipinden Ad, integer tipinden Fiyat ve date veri tipinden Basim elementleri yer almaktadır. Ayrıca her Kitap boğumu (node) içerisinde string tipinden Yazar elementlerini taşıyan, Yazarlar isimli alt boğumlarda (Childe Nodes) yer almaktadır. Kısacası aşağıdaki örnek XML içeriğine ait bir şema yapısı söz konusudur.

```xml
<Kitaplar>
            <Kitap ID="1000">
                <Ad>Her Yönüyle C#</Ad>
                <Fiyat>50</Fiyat>
                <Basim>2001-01-01Z</Basim>
                <Yazarlar>
                    <Yazar>Sefer Algan</Yazar>
                </Yazarlar>
            </Kitap>
        </Kitaplar>
```

Yukarıdaki gibi bir şema (Schema) bilgisini sisteme kayıt edebilmek için Sql Server 2005 üzerinde aşağıdaki sorgu cümlesini çalıştırmamız gerekmektedir. Bu cümle ile yukarıdaki şema bilgisini sisteme KitapSchema XML şema koleksiyonu olacak şekilde eklemekteyiz.

```text
IF EXISTS (SELECT schema_id FROM sys.XML_schema_collections WHERE name='KitapSchema')
BEGIN
     RAISERROR('Şema zaten var...',16,1)
END;

CREATE XML SCHEMA COLLECTION KitapSchema
AS
N'<?xml version="1.0" encoding="utf-16"?>
    <xs:schema attributeFormDefault="unqualified" elementFormDefault="qualified" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bsenyurt.com/Kitaplar" xmlns="http://www.bsenyurt.com/Kitaplar">
        <xs:element name="Kitaplar">
            <xs:complexType>
                <xs:sequence>
                    <xs:element name="Kitap">
                        <xs:complexType>
                            <xs:sequence>
                                <xs:element name="Ad" type="xs:string" />
                                <xs:element name="Fiyat" type="xs:int" />
                                <xs:element name="Basim" type="xs:date" />
                                <xs:element name="Yazarlar">
                                    <xs:complexType>
                                        <xs:sequence>
                                            <xs:element name="Yazar" type="xs:string" />
                                        </xs:sequence>
                                    </xs:complexType>
                                </xs:element>
                        </xs:sequence>
                        <xs:attribute name="ID" type="xs:unsignedShort" use="required" />
                    </xs:complexType>
                </xs:element>
            </xs:sequence>
        </xs:complexType>
    </xs:element>
</xs:schema>'
```

Bu sql cümlesini çalıştırdığımız takdirde, sistemde yer alan belirli tablolara şemamız ile ilgili bilgiler eklenecektir. Bunların bir kısmını görmek için aşağıdaki ekran görüntüsünde yer alan sorguları çalıştırabilirsiniz. Bu bilgiler gördüğünüz gibi sistem tablolarında tutulmaktadır. Özetle şemamızın içeriği ayrıştırılarak tablolara dağıtılmıştır.

![mk158_1.gif](/assets/images/2006/mk158_1.gif)

Dikkat ederseniz en tepede şema koleksiyonumuzun (KitapSchema) yer aldığı sistem tablosuna bakıyoruz. Burada şema koleksiyonumuz için oluşturulan xml_collection_id alanının, namespaces, elements ve attributes sistem tablolarında nasıl yer aldığına dikkat ediniz. Gördüğünüz gibi, şemamız içerisindeki her bir ayrıntı sistem tablolarına yazılmaktadır. Özellikle namespaces sistem tablosundaki isim alanı bizim için önemlidir. Buradaki isim alanını, şema bilgisini uygulamak istediğimiz XML veri tiplerinde kullanacağız. Varsayılan olarak, Visual Studio.Net gibi bir ortamda şema dosyanızı hazırladıysanız eğer (ki ben böyle yaptım) encoding formatının utf-8 olduğunu ve buradaki gibi http://www.bsenyurt.com/Kitaplar adında bir isim alanının eklenmediği görürsünüz. Burada Utf-8 formatını mutlaka Utf-16 olarak çevirmeliyiz. Nitekim Sql Server 2005 özellikle şema bilgilerinde sadece Utf-16 formatını desteklemektedir. Ayrıca, Sql Server 2005 içerisindeki XML verilerinin bu şemayı kullanabilmesi içinde, namespaces sistem tablosuna bir adın eklenmiş olması gerekmektedir. Bu amaçlada ayrıca bir xmlns'i eklememiz gerekti. Aksi takdirde, namespaces sistem tablosunda name alanı boş olan bir satır elde ederiz.

```xml
<?xml version="1.0" encoding="utf-16"?>
    <xs:schema attributeFormDefault="unqualified" elementFormDefault="qualified" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bsenyurt.com/Kitaplar" xmlns="http://www.bsenyurt.com/Kitaplar">
```

Artık sistemde, KitapSchema isminde bir XML şema koleksiyonumuz mevcuttur. Sıra geldi bunu nasıl kullanacağımıza. Sql Server 2005 Management Studio'da görsel olarak tabloları oluştururken alana ait veri tipini XML olarak seçtikten sonra bu alana ait özelliklerden Schema Collection'ı kullanarak şema bilgisini ekleyebiliriz. Örneğin aşağıdaki resimde görüldüğü gibi Kitap isimli alanın veri tipi XML olarak belirlenmiştir. Daha sonra ise Schema Collection özelliğinde bizim az önce eklediğimiz KitapSchema şema koleksiyonu seçilmiştir.

![mk158_2.gif](/assets/images/2006/mk158_2.gif)

Benzer kurallar, tablomuzu sorgu cümlesi ile oluştururken de geçerlidir. Örneğin aşağıdaki sql cümlesinde, yukarıdaki tabloya ait script yer almaktadır. Gördüğünüz gibi XML veri tipini belirlerken içeriğin dbo.KitapSchema nesnesi tarafında denetleneceği belirtilmektedir.

```text
CREATE TABLE dbo.BookBase
(
    ID int IDENTITY(1,1) NOT NULL,
    Kitap xml(CONTENT dbo.KitapSchema) NOT NULL,
    CONSTRAINT PK_BookBase PRIMARY KEY CLUSTERED
    (
        ID ASC
    )WITH (IGNORE_DUP_KEY = OFF) ON PRIMARY
) 
ON PRIMARY
```

Şimdi gelin, Kitap alanımıza örnek bir XML verisini eklemeye çalışalım. Elbette, eklenecek olan verinin buraya konulabilmesi için KitapSchema şemasının söylediği kurallara uyması beklenmektedir. Bunu sağlamak için, eklemek istediğimiz XML içeriğinde mutlaka ve mutlaka XML isim alanımız uygulanmalıdır. Aksi takdirde aşağıdaki gibi bir hata mesajı alırız.

![mk158_5.gif](/assets/images/2006/mk158_5.gif)

Aşağıdaki insert sorgusunda geçerli bir veri girişi yapılmaktadır.

```text
INSERT INTO dbo.BookBase VALUES 
    (N'<Kitaplar xmlns="http://www.bsenyurt.com/Kitaplar">
            <Kitap ID="1000">
                <Ad>Her Yönüyle C#</Ad>
                <Fiyat>50</Fiyat>
                <Basim>2001-01-01Z</Basim>
                <Yazarlar>
                    <Yazar>Sefer Algan</Yazar>
                </Yazarlar>
            </Kitap>
        </Kitaplar>')
```

Şimdi şema bilgimizin çalışıp çalışmadığını kontrol edebileceğimiz örnek bir insert sorgusu çalıştıralım. Örneğin Basim elementine geçerli olmayan bir tarih bilgisi girelim. Bu durumda, çalışma zamanında bir istisna alırız ve satırın tabloya eklenmediğini görürüz.

![mk158_3.gif](/assets/images/2006/mk158_3.gif)

Son olarak sistemde yer alan bir şema koleksiyonunu kaldırmak istediğimizde her zamanki gibi drop anahtar sözcüğünü aşağıdaki gibi kullanmamız gerekecektir.

```csharp
IF EXISTS (SELECT schema_id FROM sys.XML_schema_collections WHERE name='KitapSchema')
BEGIN
    DROP XML SCHEMA COLLECTION KitapSchema
END;
```

Görüldüğü gibi, Sql Server 2005 üzerinde yer alan XML tipindeki alanların bir şema yardımıyla kontrolü son derece kolay ve etkilidir. Çalışma zamanında sağlanan performansın yanı sıra, XML içeriğinin bizim belirleyeceğimiz şema kurallarına uygun bir biçimde doğrulanması oldukça önemlidir. Böylece geldik bir makalemizin daha sonuna bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.