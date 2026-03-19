---
layout: post
title: "WCF Known Types Analizi"
date: 2009-10-20 13:36:00 +0300
description: "Bilindiği üzere WCF aslında SOA(Service Oriented Architecture) mimarisinin uygulama modellerinden birisidir. İşin içerisinde servisler söz konusu olduğunda ağlar ve sistemler arası mesajlaşlamalar söz konusudur."
categories:
  - wcf
tags:
  - wcf
  - csharp
  - xml
  - dotnet
  - http
  - performance
  - serialization
  - reflection
  - generics
  - visual-studio
---
# WCF Known Types Analizi
![blg84_Thinking.jpg](/assets/images/2009/blg84_Thinking.jpg)

Merhaba Arkadaşlar,

Bilindiği üzere WCF aslında SOA (Service Oriented Architecture) mimarisinin uygulama modellerinden birisidir. İşin içerisinde servisler söz konusu olduğunda ağlar ve sistemler arası mesajlaşlamalar söz konusudur. Mesajlaşmalar söz konusu olduğundaysa, servis ve istemci arasında hareket eden verinin serileşebilir olması önem arz eden konuların başında gelmektedir.

Ne varki serileşen veri içeriklerinin, platform bağımsızlık adına her iki tarafında kullanabileceği tiplerden (Types) oluşmasının sağlanması bir avantajdır. İşte bu noktada biz WCF geliştiricileri için anlaşılması zor olan ve dikkatle üzerinde durumlası gereken kıyıda köşede kalmış konulardan biriside Known Types kavramıdır.

WCF ile birlikte gelen ve serileştirmede kullanılan tipler esas itibariyle Shared Contracts kategorisindendir. Nitekim serileşen tipin ve kullandığı içeriğin Interoperabilitiy kuralları çevresinde değerlendirilebiliyor olması gerekir. DataContractSerializer, JsonDataContractSerializer ve XmlSerializer gibi serileştirici tipler bu kategoride yer almaktadır. Diğer yandan BinaryFormatter, SoapFormatter veya NetDataContractSerializer gibi tipler, Shared Types kategorisinde yer alan serileştiricilerdir. Genellikle serileşen tiplerin içerdiği tip bilgilerinin tanımlandığı Assembly'lar, uygulama ile aynı makinede yer alır.

Örneğin serileştirilebilir tipin içeriğinin.Net CLR tiplerinden oluştuğunu düşünelim. Bu durumda ters serileştirme işlemini üstelenen uygulamanında bu CLR (Common Language Runtime) tiplerini biliyor olması, bir başka deyişle uygun Framework Assembly'larına sahip olması yeterlidir. Ancak SOA tabanlı bir sistemde serileştirme (Serialize) ve ters-serileştirme (Deserialize) yapan tarafların aralarında taşıdıkları tiplerle ilişkili olarak ortak bir noktada buluşmaları gerekmektedir. Nitekim hem sağlayıcı hemde tüketici taraflar kendi platformlarında kendi özel veri tiplerine sahiptir ve sadece bu tipleri kullanabilir. Bu noktada tipin XSD (XmlSchemaDefinition) şemaları içerisinde tanımlandığını (dolayısıyla serileşen paketler içerisinde taşındığını) ve örneğin Proxy'yi üreten tarafta bile kullanılabilecek bir tipe karşılık gelmesi gerektiğini söyleyebiliriz. Tabiki buradaki veri tipi bilgileri arada transfer edilen XML içeriklerinde ortaktır.

Aslında bu teorik bilgiler, yazarken bile insanın kafasını allak bullak etmeye neden olabilir. Bu yüzden kafamızda sorunsal haline gelebilecek bu konuyu örnekler üzerinden açıklamakta yarar olacağı kanısındayım. İşe ilk olarak Visual Studio 2008 ortamında oluşturulumuş bir Console uygulaması ve aşağıdaki kod parçası ile başlayalım.(System.Runtime.Serialization referansının eklenmiş olması gerektiğini unutmayın)

```csharp
using System.IO;
using System.Runtime.Serialization;

namespace KnownTypes
{
    class Program
    {
        static void Main(string[] args)
        {
            // DataContractSerializer nesnesi örneklenirken parametre olarak serileştirilecek tip bilgisi verilir.
            XmlObjectSerializer serializer = new DataContractSerializer(typeof(Product));
            // Serileştirme işlemi yapılır. İlk parametre ile çıktının Product.xml isimli dosyaya yaplacağı belirtilir. 
            // İkinci parametrede Product nesnesi örneklenir. Dikkat edilecek nokta Information özelliğine object tipinden bir nesne örneğinin aktarılmış olmasıdır.            
            serializer.WriteObject(
                new FileStream("Product.xml", FileMode.Create, FileAccess.Write)
                , new Product { Information = new object() }
                );

            // Object tipine string atama
            serializer.WriteObject(
                new FileStream("ProductV2.xml", FileMode.Create, FileAccess.Write)
                , new Product { Information = "Ürün hakkında çeşitli bilgiler" }
                );

            // Exception durumu
            //serializer.WriteObject(
            //    new FileStream("ProductV3.xml", FileMode.Create, FileAccess.Write)
            //    , new Product { Information = new ProductInformation { Id=1, Summary="Ürün için çeşitli bilgiler" } }
            //    );  
        }
    }

    [DataContract]
    class Product
    {
        [DataMember]
        public object Information { get; set; }
    }

    // [DataContract]
    // class ProductInformation
    // {
    //     [DataMember]
    //     public string Summary { get; set; }
    //     [DataMember]
    //     public int Id { get; set; }
    // }
}
```

Örneğimizde Product isimli serileştirilebilir bir tip tanımlandığı görülmektedir. DataContract niteliği ile işaretlenmiş olan Product tipinin object tipinden Information isimli bir özelliği de bulunmaktadır ki işleri karıştıracak olan nokta burasıdır. Program kodu içerisindeki amaç, Product tipinden nesne örneklerinin nasıl serileştirildiğine bakmaktır. serializer isimli XmlObjectSerializer nesne örneği üzerinden yapılan ilk WriteObject çağrısındai Information özelliğine object tipinden bir değer atandığı görülmektedir. İkinci WriteObject çağrısında ise bir string değer atanmıştır ki bu son derece doğaldır. (Hatırlayalım.Net tipleri en üstte Object tipinden türemektedir. Bu nedenle her nesnenin Object tipi ile ifade edilebilmesi mümkündür.) Bu kod parçasını çalıştırdığımızda Product.xml ve ProductV2.xml isimli iki dosya üretildiğini görürüz. Önce Product.xml içeriğine ve şemasına bir bakalım.

Product.xml içeriği;

```xml
<Product xmlns="http://schemas.datacontract.org/2004/07/KnownTypes" xmlns:i="http://www.w3.org/2001/XMLSchema-instance"><Information/></Product>
```

Product.xsd içeriği;

```xml
<?xml version="1.0" encoding="windows-1254"?>
<xs:schema xmlns:i="http://www.w3.org/2001/XMLSchema-instance" attributeFormDefault="unqualified" elementFormDefault="qualified" targetNamespace="http://schemas.datacontract.org/2004/07/KnownTypes" xmlns:xs="http://www.w3.org/2001/XMLSchema">
  <xs:element name="Product">
    <xs:complexType>
      <xs:sequence>
        <xs:element name="Information" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>
```

Tahmin ettiğimiz ve beklediğimiz gibi bir çıktı üretilmiştir. Ancak Information özelliğine string bir değer atanmasının sonucu oluşan çıktı biraz farklıdır. Bu noktada dikkatle duralım

![Sealed](/assets/images/2009/smiley-sealed.gif)

ProductV2.xml içeriği;

```xml
<Product xmlns="http://schemas.datacontract.org/2004/07/KnownTypes" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
  <Information i:type="a:string" xmlns:a="http://www.w3.org/2001/XMLSchema">Ürün hakkında çeşitli bilgiler</Information>
</Product>
```

ProductV2.xsd içeriği;

```xml
<?xml version="1.0" encoding="utf-8"?>
<a:schema xmlns:i="http://www.w3.org/2001/XMLSchema-instance" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:a="http://www.w3.org/2001/XMLSchema" attributeFormDefault="unqualified" elementFormDefault="qualified" targetNamespace="http://schemas.datacontract.org/2004/07/KnownTypes">
  <xs:element name="Product">
    <xs:complexType>
      <xs:sequence>
        <xs:element name="Information" type="xs:string" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</a:schema>
```

XSD şemasında, Information elementi için type niteliğinde xs:string kullanıldığı görülmektedir. Üstelik üretilen veri içeriğinde de i:type niteliğinde Information elementinin içeriğinin string veri tipinden olduğu işaret edilmektedir. Yani Information özelliğine atadığımız string değişken, primitive bir tip tanımana göre XML içerisinde bildirilmiştir. Zaten primitive tiplere dönüşüm yapıldığı takdirde pek bir sıkıntı yoktur. Söz gelimi Information özelliğine örneğin 12 değerini atadığımızda buna uygun olarak XML içeriğinde de int tipinin kullanıldığı görülebilir. Sorun yorum satırı kodlarını çalışır hale getirdiğimizde ortaya çıkmaktadır. Yeni ProductInformation tipini etkinleştirip aşağıdaki kodları çalıştırdığımızda...

![Undecided](/assets/images/2009/smiley-undecided.gif)

```csharp
// Exception durumu
            serializer.WriteObject(
                new FileStream("ProductV3.xml", FileMode.Create, FileAccess.Write)
                , new Product { Information = new ProductInformation { Id=1, Summary="Ürün için çeşitli bilgiler" } }
                ); 
```

Bu sefer Information özelliğine serileştirilebilir (ki DataContract ve DataMember nitelikleri nedeni ile) ProductInformation tipinden bir nesne örneği atanmaktadır. Bu durumda çalışma zamanında aşağıdaki ekran görüntüsünde yer alan SerializationException istisnasının alındığı görülür.

![blg84_Exception.gif](/assets/images/2009/blg84_Exception.gif)

Bu istisna mesajından anlamamız gereken özlü söz şudur;

"Serileştirici tiplerin, serileştirecekleri nesne örneklerinin özelliklerinin tiplerinin neler olabileceğini açık bir şekilde bilmeye ihtiyaçları vardır. " ![Wink](/assets/images/2009/smiley-wink.gif)

Her ne kadar primitive bir tip kullanıldığında sorun olmasa da, yukarıdaki örnekte görüldüğü gibi bilinmeyen bir tipin atanmasında sorunlar yaşanabilir. Üstelik belkide atanan verinin tipinin, serileştirici tarafından varsayılan olarak atanan bir tip olmaması da gerekebilir. Söz gelimi Information özelliğine noktasız sayısal bir değer atandığında büyüklüğüne göre varsayılan olarak int tipi göz önüne alınacak ve XML içeriğinde bu yönde bir tanımlama olacaktır. Ki karşı tarafta belkide bu sayısal değerin string olarak değerlendirilmesi isteniyor olabilir! Upsss...

Peki serileştiricinin serileştirdiği tipin içeriğinde kullanılabilecek tipleri kesin olarak bilmesi nasıl sağlanabilir?

Yöntemlerden birisi ve belkide en basiti, aşağıdaki kod parçasında olduğu gibi serileştirilecek tip için KnownType niteliğini kullanmaktır.

Kişisel Not: Başka yöntemlerde bulunmaktadır. Örneğin tip içerisinde Type[] dizisi döndüren bir metodun adı KnownType niteliğinde kullanılarak birden fazla tipin bildirimi yapılabilir. Ya da servis sözleşmesinde ServiceKnownType niteliğinde bu bildirim yapılabilir. İşte size güzel bir araştırma konusu. Bu tekniklerin nasıl uygulanabileceğini araştırabilirsiniz. Özellikle ilk teknik dikkate değerdir. Yani bir metod aracılığıyla, KnowType niteliğine birden fazla tipin Type[] dizisi olarak bildirilmesi. Dikkat edilmesi gereken tek nokta Type[] dizisi döndüren metodun static olmasını sağlamaktır. Hatta buradan bir adım öteye gidip generic bir modelin KnownType niteliğine söz konusu metod yardımıyla aktarılması dahi sağlanabilir. Bu kadar ipucu yeter. Haydi klavye başına ![Wink](/assets/images/2009/smiley-wink.gif)

```csharp
[DataContract]
[KnownType(typeof(ProductInformation))]
class Product
{
   [DataMember]
   public object Information { get; set; }
}
```

Bu şekildeki kullanım sonrasında artık SerializationException istisnası üretilmeyecek ve aşağıdaki çıktılar oluşacaktır.

ProductV3.xml içeriği;

```xml
<Product xmlns="http://schemas.datacontract.org/2004/07/KnownTypes" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
  <Information i:type="ProductInformation">
    <Id>1</Id>
    <Summary>Ürün için çeşitli bilgiler</Summary>
  </Information>
</Product>
```

Dikkat edileceği üzere ProductInformation nesne örneği içeriği ile birlikte Product elementi içerisine alınmıştır. Bu durumda şema içeriğinde gerekli bilgilendirmelerin aşağıdaki gibi yapıldığı görülecektir.

ProductV3.xsd içeriği

```xml
<?xml version="1.0" encoding="utf-8"?>
<xs:schema xmlns:i="http://www.w3.org/2001/XMLSchema-instance" attributeFormDefault="unqualified" elementFormDefault="qualified" targetNamespace="http://schemas.datacontract.org/2004/07/KnownTypes" xmlns:xs="http://www.w3.org/2001/XMLSchema">
  <xs:element name="Product">
    <xs:complexType>
      <xs:sequence>
        <xs:element name="Information">
          <xs:complexType>
            <xs:sequence>
              <xs:element name="Id" type="xs:unsignedByte" />
              <xs:element name="Summary" type="xs:string" />
            </xs:sequence>
          </xs:complexType>
        </xs:element>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>
```

Known Type sorunsalının bir sorunsal olarak değerlendirilmesinin ise iki sebebi vardır.

Birincisi, SOA düşünce tarzına aykırı olduğu görüşününün yaygın olmasıdır. Nitekim Shared Contract'ların söz konusu olduğu senaryolarda Interoperability sağlanırken, XML içerisindeki tipin karşı tarafça anlaşılabilir olması gerekmektedir. İkinci olarak serileştirme ve ters serileştirme işlemleri sırasında Known Type'a göre Reflection mekanizmasının devreye girmesi, XML üzerinde tip ile ilişkili bilgi edinme ve yazma gibi operasyonların söz konusu olmasından kaynaklanan performans kayıpları değerlendirilmektedir. Bu iki sebep nedeniyle zorunlu kalınmadıkça Known Type kullanımından kaçınılması önerilmektedir. Peki neden böyle bir konuya değindik? Aslında bir sonraki yazımıza zemin hazırlamaya çalışıyoruz. Nitekim WCF 4.0 tarafında bu konu ile ilişkili bir yenilik gelmesi muhtemeldir. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[KnownTypes.rar (22,64 kb)](/assets/files/2009/KnownTypes.rar)